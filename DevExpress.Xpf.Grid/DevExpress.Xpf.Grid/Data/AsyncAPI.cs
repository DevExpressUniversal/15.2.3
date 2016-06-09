#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using DevExpress.Data;
using DevExpress.Xpf.Grid;
namespace DevExpress.Xpf.Data {
	class AsyncOperationWaitHandle {
		private bool isInterrupted;
		readonly List<ManualResetEvent> handlers = new List<ManualResetEvent>();
		public bool IsInterrupted {
			get { return this.isInterrupted; }
			set {
				this.isInterrupted = value;
				if(!value) return;
				foreach(var item in this.handlers)
					item.Set();
			}
		}
		public T Run<T>(Func<ManualResetEvent, T> action) {
			ManualResetEvent handler = new ManualResetEvent(true);
			this.handlers.Add(handler);
			return action(handler);
		}
	}
	abstract class AsyncOperationBase<T> {
		private readonly GridControl grid;
		protected GridControl Grid { get { return this.grid; } }
		protected AsyncOperationWaitHandle WaitHandle { get { return Grid.AsyncWaitHandle; } }
		protected DataController DataController { get { return Grid.DataController; } }
		protected virtual T FallbackValue { get { return default(T); } }
		protected bool IsAsyncServerMode { get { return Grid.DataProviderBase.IsAsyncServerMode; } }
		protected AsyncOperationBase(GridControl grid) {
			this.grid = grid;
		}
		protected bool IsValidOperation() {
			return DataController != null && IsValidOperationCore();
		}
		Task<T> RunDataControllerTask(Task<T> task) {
			if(IsAsyncServerMode)
				task.Start();
			else
				task.RunSynchronously();
			return task;
		}
		public Task<T> GetTask() {
			Task<T> result = IsValidOperation() ? GetTaskCore() : GetDefaultTask();
			return RunDataControllerTask(result);
		}
		protected virtual bool IsValidOperationCore() { return true; }
		protected Task<T> GetDefaultTask() {
			return new Task<T>(() => FallbackValue);
		}
		protected abstract Task<T> GetTaskCore();
	}
	abstract class SingleRowAsyncOperation<T> : AsyncOperationBase<T> {
		private readonly Func<T> getValueCallback;
		private bool isLoaded;
		private T result;
		protected SingleRowAsyncOperation(GridControl grid, Func<T> getValueCallback)
			: base(grid) {
			this.getValueCallback = getValueCallback;
		}
		protected SingleRowAsyncOperation(GridControl grid) : base(grid) { }
		protected T AsyncResult { get { return this.result; } }
		protected virtual bool CanCompleteWithoutSynchronization { get { return false; } }
		protected override Task<T> GetTaskCore() {
			return WaitHandle.Run(wh => LoadRowAsyncCore(wh));
		}
		Task<T> LoadRowAsyncCore(ManualResetEvent localWaitHandle) {
			SynchronizationContext uiContext = new DispatcherSynchronizationContext(Grid.Dispatcher);
			BeginLoadIfNeeded(localWaitHandle);
			return new Task<T>(() => {
				localWaitHandle.WaitOne();
				if(!this.isLoaded && WaitHandle.IsInterrupted) return FallbackValue;
				T result = default(T);
				if(IsAsyncServerMode && CanCompleteWithoutSynchronization) return GetValueCore();
				uiContext.Send(o => result = GetValueCore(), result);
				return result;
			});
		}
		void BeginLoadIfNeeded(ManualResetEvent localWaitHandle) {
			if(!NeedLoad()) {
				this.isLoaded = true;
				return;
			}
			this.isLoaded = false;
			localWaitHandle.Reset();
			BeginLoadCore(localWaitHandle);
		}
		protected void OnLoaded(ManualResetEvent localWaitHandle, object result) {
			this.isLoaded = true;
			this.result = (T)result;
			localWaitHandle.Set();
		}
		bool NeedLoad() {
			return IsAsyncServerMode && NeedLoadCore();
		}
		protected virtual T GetValueCore() {
			return this.getValueCallback();
		}
		protected virtual bool NeedLoadCore() {
			return true;
		}
		protected abstract void BeginLoadCore(ManualResetEvent localWaitHandle);
	}
	class LoadRowAsyncOperation<T> : SingleRowAsyncOperation<T> {
		private readonly int rowHandle;
		public LoadRowAsyncOperation(GridControl grid, int rowHandle, Func<T> getValueCallback)
			: base(grid, getValueCallback) {
			this.rowHandle = rowHandle;
		}
		protected override bool IsValidOperationCore() {
			return Grid.IsValidRowHandle(this.rowHandle);
		}
		protected override bool NeedLoadCore() {
			int controllerRow = Grid.IsGroupRowHandle(rowHandle) ? DataController.GetControllerRowByGroupRow(rowHandle) : rowHandle;
			return !DataController.IsRowLoaded(controllerRow);
		}
		protected override void BeginLoadCore(ManualResetEvent localWaitHandle) {
			DataController.GetRow(rowHandle, o => OnLoaded(localWaitHandle, o));
		}
	}
	class GetRowsAsyncOperation : AsyncOperationBase<IList> {
		private readonly int startFrom;
		private readonly int count;
		private Task<object>[] tasks;
		int EndRowHandle { get { return this.startFrom + this.count - 1; } }
		public GetRowsAsyncOperation(GridControl grid, int startFrom, int count)
			: base(grid) {
			this.count = count;
			this.startFrom = startFrom;
			PopulateTasks();
		}
		private void PopulateTasks() {
			if(!IsValidOperation()) return;
			tasks = new Task<object>[this.count];
			for(int i = 0; i < this.count; i++)
				tasks[i] = Grid.GetRowAsync(i + this.startFrom);
		}
		protected override Task<IList> GetTaskCore() {
			return new Task<IList>(() => {
				Task.WaitAll(this.tasks);
				List<object> results = this.tasks.Select(t => t.Result).ToList();
				return results.Contains(null) ? null : results;
			});
		}
		protected override bool IsValidOperationCore() {
			return Grid.IsValidRowHandle(this.startFrom) && Grid.IsValidRowHandle(EndRowHandle);
		}
	}
	class FindRowByValueAsyncOperation : SingleRowAsyncOperation<int> {
		private readonly string columnName;
		private readonly object value;
		public FindRowByValueAsyncOperation(GridControl grid, string columnName, object value)
			: base(grid) {
			this.value = value;
			this.columnName = columnName;
		}
		protected override int FallbackValue { get { return DataControlBase.InvalidRowHandle; } }
		protected override void BeginLoadCore(ManualResetEvent localWaitHandle) {
			int handle = DataController.FindRowByValue(this.columnName, this.value, o => OnLoaded(localWaitHandle, o));
			if(handle != AsyncServerModeDataController.OperationInProgress)
				OnLoaded(localWaitHandle, handle);
		}
		protected override bool IsValidOperationCore() {
			if(string.IsNullOrEmpty(this.columnName) || !Grid.Columns.Any(col => col.FieldName == this.columnName))
				return false;
			return DataController.FindRowByValue(this.columnName, this.value) != DataControlBase.InvalidRowHandle;
		}
		protected override bool CanCompleteWithoutSynchronization { get { return true; } }
		protected override int GetValueCore() {
			return IsAsyncServerMode ? AsyncResult : DataController.FindRowByValue(this.columnName, value);
		}
	}
}
