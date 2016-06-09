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
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid.Events;
using System.IO;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPivotGrid {
	public delegate void AsyncCompletedHandler(AsyncOperationResult result);
	public class AsyncOperationResult {
		internal static AsyncOperationResult Create(object result, Exception error) {
			return new AsyncOperationResult(result, error);
		}
		object value;
		Exception exception;
		protected AsyncOperationResult()
			: this(null, null) {
		}
		protected AsyncOperationResult(object result)
			: this(result, null) {
		}
		protected AsyncOperationResult(object value, Exception exception) {
			this.value = value;
			this.exception = exception;
		}
		public object Value { 
			get {
				RaiseExceptionIfNecessary();
				return value; 
			} 
		}
		public Exception Exception { get { return exception; } }
		void RaiseExceptionIfNecessary() {
			if(Exception != null) {
				throw Exception;
			}
		}
	}
}
namespace DevExpress.XtraPivotGrid.Data {
	public interface ILoadingPanelOwner {
		event EventHandler ShowMainLoadingPanel;
		event EventHandler HideMainLoadingPanel;
		event EventHandler ShowFilterPopupLoadingPanel;
		event EventHandler HideFilterPopupLoadingPanel;
		bool IsMainLoadingPanelVisible { get; }
		bool IsFilterPopupLoadingPanelVisible { get; }
	}
	public class ActionDelayer : IDisposable {
		readonly Timer timer;
		readonly Action action;
		bool isInDelay = false;
		 public ActionDelayer(Action action) : this(action, null) {
		 }
		public ActionDelayer(Action action, int? delay) {
			this.action = action;
			timer = new Timer();
			timer.Enabled = false;
			timer.Tick += OnTimerTick;
			if(delay != null)
#if !SL
				timer.Interval = (int)delay;
#else
				timer.Interval = TimeSpan.FromMilliseconds((int)delay);
#endif
		}
		public bool IsInDelay { get { return isInDelay; } }
		public void Invoke(int delay) {
			isInDelay = true;
			if(delay > 0)
				InvokeDelayed(delay);
			else
				InvokeAction();
		}
		public void Cancel() {
			timer.Stop();
			isInDelay = false;
		}
		protected virtual void InvokeDelayed(int delay) {
			if(timer.Enabled) {
				timer.Stop();
#if !SL
				timer.Interval = delay;
#else
				timer.Interval = TimeSpan.FromMilliseconds(delay);
#endif
			}
			timer.Start();
		}
		protected void InvokeAction() {
			if(isInDelay) {
				if(action != null)
					action();
			}
			isInDelay = false;
		}
		void OnTimerTick(object sender, EventArgs e) {
			timer.Stop();
			InvokeAction();
		}
		#region IDisposable Members
		public void Dispose() {
			if(timer != null) {
				timer.Stop();
				timer.Tick -= OnTimerTick;
				timer.Dispose();
			}
		}
		#endregion
	}
	public delegate void AsyncCompletedInternal();
	public delegate void AsyncCompletedInternalWrapper(object result, Exception ex);
	public delegate object AsyncProcess();
#if DOTNET
	public class PivotGridAsyncOperationHelper : IDisposable {
		bool isBusy;
		public bool IsBusy { get { return this.isBusy; } }
		public void Clear() {
		}
		public void RunAsync(AsyncProcess process, AsyncCompletedInternalWrapper completed) {
			try {
				this.isBusy = true;
				object result = process();
				completed.Invoke(result, null);
			}
			catch (Exception e) {
				completed.Invoke(null, e);
			}
			finally {
				this.isBusy = false;
			}
		}
		public void Dispose() {
		}
	}
#else
	public class PivotGridAsyncOperationHelper : IDisposable {
		BackgroundWorker bgWorker;
		AsyncProcess process;
		AsyncCompletedInternalWrapper completed;
		bool isBusy;
		public PivotGridAsyncOperationHelper() {
			bgWorker = new BackgroundWorker();
			SubscribeEvents();
		}
		public void RunAsync(AsyncProcess process, AsyncCompletedInternalWrapper completed) {
			this.process = process;
			this.completed = completed;
			RunAsyncCore();
		}
		protected virtual void RunAsyncCore() {
			CurrentThreadSettings settings = new CurrentThreadSettings(System.Threading.Thread.CurrentThread, CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);
			BgWorker.RunWorkerAsync(settings);
		}
		protected BackgroundWorker BgWorker {
			get { return bgWorker; }
		}
		public bool IsBusy { get { return this.isBusy; } }
		void SubscribeEvents() {
			BgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
			BgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
		}
		void UnsubscribeEvents() {
			BgWorker.DoWork -= new DoWorkEventHandler(bgWorker_DoWork);
			BgWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
		}
		protected void bgWorker_DoWork(object sender, DoWorkEventArgs e) {
			if(process == null)
				throw new Exception("Incorrect process delegate");
			this.isBusy = true;
			try {
				CurrentThreadSettings settings = e.Argument as CurrentThreadSettings;
				if(settings != null) {
					if(CultureInfo.CurrentCulture != settings.CurrentCulture)
						CultureInfoExtensions.SetCurrentCulture(settings.CurrentCulture);
					if(CultureInfo.CurrentUICulture != settings.CurrentUICulture)
						CultureInfoExtensions.SetCurrentUICulture(settings.CurrentUICulture);
				}
				e.Result = process.Invoke();
			} finally {
				this.isBusy = false;
			}
		}
		protected void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if(completed == null)
				throw new Exception("Incorrect completed delegate");
			Exception error = e.Error;
			object result = error != null ? null : e.Result;
			completed.Invoke(result, error);
		}
		public void Clear() {
			ClearAsyncOperations();
		}
		void ClearAsyncOperations() {
			this.process = null;
			this.completed = null;
		}
		#region IDisposable Members
		public void Dispose() {
			UnsubscribeEvents();
			IDisposable disposable = bgWorker as IDisposable;
			if(disposable != null) 
				disposable.Dispose();
			bgWorker = null;
			Clear();
		}
		#endregion
		class CurrentThreadSettings {
			System.Threading.Thread currentThread;
			CultureInfo currentCulture;
			CultureInfo currentUICulture;
			internal CurrentThreadSettings(System.Threading.Thread currentThread, CultureInfo currentCulture, CultureInfo currentUICulture) {
				this.currentThread = currentThread;
				this.currentCulture = currentCulture;
				this.currentUICulture = currentUICulture;
			}
			public CultureInfo CurrentCulture {
				get {
#if DXRESTRICTED
					return currentCulture;
#else
					return this.currentThread.CurrentCulture;
#endif
				}
			}
			public CultureInfo CurrentUICulture {
				get {
#if DXRESTRICTED
					return currentUICulture;
#else
					return this.currentThread.CurrentUICulture;
#endif
				}
			}
		}
	}
#endif
	public enum PivotLoadingPanelType { MainLoadingPanel = 1, FilterPopupLoadingPanel = 2 };
	public class PivotGridDataAsync : PivotGridData, ILoadingPanelOwner, IActionsQueue {
		int asyncOperationCounter;
		int layoutChangedCounter;
		int endRefreshCounter;
		int lockCount;
		PivotGridAsyncOperationHelper helper;
		PivotGridEventRaiserBase eventRaiser;
		ActionDelayer delayer;
		public const int AsyncOperationWaitInterval = 200;
		internal PivotGridAsyncOperationHelper Helper {
			get { return helper; }
			set { helper = value; }
		}
		public PivotGridDataAsync()
			: base() {
			this.asyncOperationCounter = 0;
			this.helper = CreateAsyncOperationHelper();
			this.eventRaiser = CreateEventRaiser(base.EventsImplementor);
			SetDefaultLoadingPanelType();
		}
		protected virtual PivotGridAsyncOperationHelper CreateAsyncOperationHelper() {
			return new PivotGridAsyncOperationHelper();
		}
		protected virtual PivotGridEventRaiserBase CreateEventRaiser(IPivotGridEventsImplementorBase eventsImplementor) {
			return eventsImplementor == null ? null : new PivotGridEventRaiserBase(eventsImplementor);
		}
		public override IPivotGridEventsImplementorBase EventsImplementor {
			get { return (IPivotGridEventsImplementorBase)eventRaiser; }
			set {
				base.EventsImplementor = value;
				eventRaiser = CreateEventRaiser(base.EventsImplementor);
			}
		}
		protected PivotGridEventRaiserBase EventRaiser {
			get { return eventRaiser; }
		}
		public override void DoActionInMainThread(Action action) {
			if(eventRaiser != null)
				eventRaiser.DoActionInMainThread(action);
			else
				action();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				WaitForBackgroundThread();
				helper.Dispose();
				helper = null;
				if(delayer != null)
					delayer.Dispose();
			}
			base.Dispose(disposing);
		}
		void WaitForBackgroundThread() {
			while(helper.IsBusy)
				System.Threading.Thread.Sleep(AsyncOperationWaitInterval);
		}
		public override bool IsLocked {
			get { return lockCount > 0; }
		}
		public void EndUpdateAsync(bool forceAsync) {
			EndUpdateAsync(forceAsync, DoEmptyComplete);
		}
		public void EndUpdateAsync(bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				EndUpdate();
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void ChangeExpandedAsync(PivotFieldValueItem item, bool forceAsync) {
			ChangeExpandedAsync(item, forceAsync, DoEmptyComplete);
		}
		public void ChangeExpandedAsync(PivotFieldValueItem item, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				ChangeExpanded(item);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void ChangeExpandedAsync(bool isColumn, int visibleIndex, bool expanded, bool forceAsync) {
			ChangeExpandedAsync(isColumn, visibleIndex, expanded, forceAsync, DoEmptyComplete);
		}
		public void ChangeExpandedAsync(bool isColumn, int visibleIndex, bool expanded, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return ChangeExpanded(isColumn, visibleIndex, expanded);
			}, asyncCompleted, forceAsync);
		}
		public void ChangeExpandedAsync(bool isColumn, object[] values, bool forceAsync) {
			ChangeExpandedAsync(isColumn, values, forceAsync, DoEmptyComplete);
		}
		public void ChangeExpandedAsync(bool isColumn, object[] values, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return ChangeExpanded(isColumn, values);
			}, asyncCompleted, forceAsync);
		}
		public void ChangeExpandedAsync(bool isColumn, object[] values, bool expanded, bool forceAsync) {
			ChangeExpandedAsync(isColumn, values, expanded, forceAsync, DoEmptyComplete);
		}
		public void ChangeExpandedAsync(bool isColumn, object[] values, bool expanded, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return ChangeExpanded(isColumn, values, expanded);
			}, asyncCompleted, forceAsync);
		}
		public void ChangeExpandedAllAsync(bool expanded, bool forceAsync) {
			ChangeExpandedAllAsync(expanded, forceAsync, DoEmptyComplete);
		}
		public void ChangeExpandedAllAsync(bool expanded, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				ChangeExpandedAll(expanded);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void ChangeExpandedAllAsync(bool isColumn, bool expanded, bool forceAsync) {
			ChangeExpandedAllAsync(isColumn, expanded, forceAsync, DoEmptyComplete);
		}
		public void ChangeExpandedAllAsync(bool isColumn, bool expanded, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				ChangeExpandedAll(isColumn, expanded);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void ChangeFieldExpandedAsync(PivotGridFieldBase field, bool expanded, bool forceAsync) {
			ChangeFieldExpandedAsync(field, expanded, forceAsync, DoEmptyComplete);
		}
		public void ChangeFieldExpandedAsync(PivotGridFieldBase field, bool expanded, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				ChangeFieldExpanded(field, expanded);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void ChangeFieldExpandedAsync(PivotGridFieldBase field, bool expanded, object value, bool forceAsync) {
			ChangeFieldExpandedAsync(field, expanded, value, forceAsync, DoEmptyComplete);
		}
		public void ChangeFieldExpandedAsync(PivotGridFieldBase field, bool expanded, object value, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				ChangeFieldExpanded(field, expanded, value);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void GetDrillDownDataSourceAsync(int columnIndex, int rowIndex, int dataIndex, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return GetDrillDownDataSource(columnIndex, rowIndex, dataIndex);
			}, asyncCompleted, forceAsync);
		}
		public void GetDrillDownDataSourceAsync(int columnIndex, int rowIndex, int dataIndex, int maxRowCount, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return GetDrillDownDataSource(columnIndex, rowIndex, dataIndex, maxRowCount);
			}, asyncCompleted, forceAsync);
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, -1, forceAsync, asyncCompleted);
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount);
			}, asyncCompleted, forceAsync);
		}
		[Obsolete("This method is now obsolete. Use the CreateQueryModeDrillDownDataSourceAsync method instead.")]
		public void CreateOLAPDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			CreateQueryModeDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, customColumns, forceAsync, asyncCompleted);
		}
		public void CreateQueryModeDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
			}, asyncCompleted, forceAsync);
		}
		public void RetrieveFieldsAsync(bool forceAsync) {
			RetrieveFieldsAsync(forceAsync, DoEmptyComplete);
		}
		public void RetrieveFieldsAsync(bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				RetrieveFields();
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void RetrieveFieldsAsync(PivotArea area, bool visible, bool forceAsync) {
			RetrieveFieldsAsync(area, visible, forceAsync, DoEmptyComplete);
		}
		public void RetrieveFieldsAsync(PivotArea area, bool visible, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				RetrieveFields(area, visible);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void ReloadDataAsync(bool forceAsync) {
			ReloadDataAsync(forceAsync, DoEmptyComplete);
		}
		public void ReloadDataAsync(bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				ReloadData();
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void SetFieldAreaPositionAsync(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex, bool forceAsync) {
			SetFieldAreaPositionAsync(field, newArea, newAreaIndex, forceAsync, DoEmptyComplete);
		}
		public void SetFieldAreaPositionAsync(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex, 
				bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				SetFieldAreaPosition(field, newArea, newAreaIndex);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void SetFieldSortingAsync(PivotGridFieldBase field, PivotSortOrder sortOrder,
				PivotSortMode? actualSortMode, PivotSortMode? sortMode, bool reset, bool forceAsync) {
			SetFieldSortingAsync(field, sortOrder, actualSortMode, sortMode, reset, forceAsync, DoEmptyComplete);
		}
		public void SetFieldSortingAsync(PivotGridFieldBase field, PivotSortOrder sortOrder,
				PivotSortMode? actualSortMode, PivotSortMode? sortMode, bool reset, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				SetFieldSorting(field, sortOrder, actualSortMode, sortMode, reset);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void ClearFieldSortingAsync(PivotGridFieldBase field, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			SetFieldSortingAsync(field, PivotSortOrder.Ascending, null, PivotSortMode.None, false, forceAsync, asyncCompleted);
		}
		public void ChangeFieldSortOrderAsync(PivotGridFieldBase field, bool forceAsync) {
			ChangeFieldSortOrderAsync(field, forceAsync, DoEmptyComplete);
		}
		public void ChangeFieldSortOrderAsync(PivotGridFieldBase field, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				ChangeFieldSortOrder(field);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void GetSortedUniqueValuesAsync(PivotGridFieldBase field, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return GetSortedUniqueValues(field);
			}, asyncCompleted, forceAsync);
		}
		public void GetAvailableFieldValuesAsync(PivotGridFieldBase field, bool forceAsync, AsyncCompletedHandler asyncCompleted, bool deferUpdates) {
			InvokeProcess(() => GetAvailableFieldValues(field, deferUpdates), asyncCompleted, forceAsync);
		}
		public void GetOLAPColumnMembersAsync(string fieldName, AsyncCompletedHandler asyncCompleted, bool forceAsync) {
			InvokeProcess(() => GetOLAPColumnMembers(fieldName), asyncCompleted, forceAsync);
		}
		public void GetSortedUniqueGroupValuesAsync(PivotGridGroup group, object[] parentValues, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				return GetSortedUniqueGroupValues(group, parentValues);
			}, asyncCompleted, forceAsync);
		}
		public void SetFieldVisibleAsync(PivotGridFieldBase field, bool visible, bool forceAsync) {
			SetFieldVisibleAsync(field, visible, forceAsync, DoEmptyComplete);
		}
		public void SetFieldVisibleAsync(PivotGridFieldBase field, bool visible, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				field.Visible = visible;
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void DoRefreshAsync(bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				DoRefresh();
				return null;
			}, asyncCompleted, forceAsync);
		}
		public virtual void OnFieldFilteringChangedAsync(PivotGridFieldBase field, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			BeforeFieldFilteringChanged(field);
			DoRefreshAsync(forceAsync, delegate(AsyncOperationResult result) {
				AfterFieldFilteringChanged(field);
				asyncCompleted.Invoke(result);
			});
		}
		public virtual void OnGroupFilteringChangedAsync(PivotGridGroup group, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			DoRefreshAsync(forceAsync, delegate(AsyncOperationResult result) {
				AfterGroupFilteringChanged(group);
				asyncCompleted.Invoke(result);
			});
		}
		public void ChangeFieldExpandedInFieldsGroupAsync(PivotGridFieldBase field, bool expandedInFieldsGroup, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				field.ExpandedInFieldsGroup = expandedInFieldsGroup;
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void ChangeFieldExpandedInFieldsGroupAsync(PivotGridFieldBase field, bool expandedInFieldsGroup, bool forceAsync) {
			ChangeFieldExpandedInFieldsGroupAsync(field, expandedInFieldsGroup, forceAsync, DoEmptyComplete);
		}
		public void LoadCollapsedStateFromStreamAsync(Stream stream, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(delegate() {
				LoadCollapsedStateFromStream(stream);
				return null;
			}, asyncCompleted, forceAsync);
		}
		public void LoadCollapsedStateFromStreamAsync(Stream stream, bool forceAsync) {
			LoadCollapsedStateFromStreamAsync(stream, forceAsync, DoEmptyComplete);
		}
		public void InvertGroupFilterAsync(PivotGroupFilterValues values, PivotFilterType filterType, bool forceAsync, AsyncCompletedHandler asyncCompleted) {
			if(IsLocked) {
				values.FilterType = filterType;
				return;
			}
			InvokeProcess(delegate() {
				values.InvertFilterValues(filterType);
				return null;
			}, asyncCompleted, forceAsync);
		}
		protected internal void InvokeAsync(AsyncProcess process, AsyncCompletedHandler asyncCompleted, bool forceAsync) {
			InvokeProcess(process, asyncCompleted, forceAsync);
		}
		protected internal void InvokeAsync(AsyncProcess process, AsyncCompletedHandler asyncCompleted) {
			InvokeAsync(process, asyncCompleted, false);
		}
#region Async operations logic
		internal void InvokeProcess(AsyncProcess process, AsyncCompletedHandler asyncCompleted) {
			InvokeProcess(process, asyncCompleted, false);
		}
		internal void InvokeProcess(AsyncProcess process, AsyncCompletedHandler asyncCompleted, bool forceAsync) {
			if(UseAsyncMode || forceAsync) {
				InvokeProcessAsync(process, asyncCompleted);
			} else {
				InvokeProcessSync(process, asyncCompleted);
			}
		}
#if DEBUGTEST
		protected virtual
#endif
		void InvokeProcessAsync(AsyncProcess process, AsyncCompletedHandler asyncCompleted) {
			DoBeforeAsyncProcessStarted();
			LockData();
			Helper.RunAsync(process, GetAsyncCompletedHandlerInternalWrapperDelegate(asyncCompleted));
		}
#if DEBUGTEST
		protected virtual
#endif
		void InvokeProcessSync(AsyncProcess process, AsyncCompletedHandler asyncCompleted) {
			object result = null;
			Exception error = null;
			try {
				result = process.Invoke();
			} catch(Exception e) {
				error = e;
			} finally {
				asyncCompleted.Invoke(AsyncOperationResult.Create(result, error));
			}
		}
		AsyncCompletedInternalWrapper GetAsyncCompletedHandlerInternalWrapperDelegate(AsyncCompletedHandler asyncCompleted) {
			return delegate(object result, Exception error) {
				InvokeInMainThread(delegate() {
					UnlockData();
					DecAsyncOperationCounter();
					DoBeforeAsyncProcessCompleted();
					asyncCompleted.Invoke(AsyncOperationResult.Create(result, error));
					if(IsAsyncOperationCompleted && !IsQueueRunning)
						Helper.Clear();
				});
			};
		}
		protected virtual void InvokeInMainThread(AsyncCompletedInternal completed) {
			completed.Invoke();
		}
		public bool IsInBackgroundAsyncOperation {
			get { return IsLocked && !IsInMainThread; }
		}
		protected bool IsLayoutChangedRequired {
			get { return layoutChangedCounter > 0; }
		}
		bool IsEndRefreshRequired {
			get { return endRefreshCounter > 0; }
		}
		internal bool IsAsyncOperationCompleted {
			get { return this.asyncOperationCounter == 0; }
		}
		void IncAsyncOperationCounter() {
			this.asyncOperationCounter++;
		}
		void DecAsyncOperationCounter() {
			this.asyncOperationCounter--;
		}
		protected bool UseAsyncMode {
			get { return OptionsBehavior.UseAsyncMode; }
		}
		protected virtual void AsyncProcessStarting() {
		}
		protected virtual void AsyncProcessFinishing() {
		}
		protected virtual void DoBeforeAsyncProcessStarted() {
			AsyncProcessStarting();
		}
		protected virtual void DoBeforeAsyncProcessCompleted() {
			RaiseEventsAfterAsync();
			if(!IsQueueRunning)
				AsyncProcessFinishing();
		}
		protected
#if !DEBUGTEST
		 sealed
#endif
		override void LayoutChangedCore() {
			if(!IsLocked)
				base.LayoutChangedCore();
			else 
				layoutChangedCounter++;
		}
		protected override void RaiseEndRefresh() {
			if(!IsLocked)
				base.RaiseEndRefresh();
			else
				endRefreshCounter++;
		}
		void ResetLayoutChangedCounter() {
			layoutChangedCounter = 0;
		}
		void ResetEndRefreshCounter() {
			endRefreshCounter = 0;
		}
		void ForceLayoutChangedCore() {
			ResetLayoutChangedCounter();
			LayoutChanged();
		}
		void ForceRaiseEndRefresh() {
			ResetEndRefreshCounter();
			base.RaiseEndRefresh();
		}
		void RaiseLayoutChangedAfterAsync() {
			if(IsLayoutChangedRequired)
				ForceLayoutChangedCore();
		}
		void RaiseEndRefreshAfterAsync() {
			if(IsEndRefreshRequired)
				ForceRaiseEndRefresh();
		}
		void RaiseEventsAfterAsync() {
			FinishEventsRecordingAndRaiseEvents();
			RaiseLayoutChangedAfterAsync();
			RaiseEndRefreshAfterAsync();
		}
		protected void DoEmptyComplete(AsyncOperationResult operationResult) {
			if(operationResult.Exception != null)
				throw operationResult.Exception;
		}
		void LockData() {
			if(++lockCount > 1)
				ThrowAsyncCallException();
			FreezeVisualItems();
			IncAsyncOperationCounter();
			if(!IsQueueRunning)
				ShowLoadingPanel();
			StartEventsRecording();
		}
		void UnlockData() {
			if(--lockCount < 0)
				ThrowAsyncCallException();
			UnfreezeVisualItems();
			if(!IsQueueRunning)
				HideLoadingPanel();
		}
		private void FinishEventsRecordingAndRaiseEvents() {
			if(EventRaiser != null)
				EventRaiser.FinishRecordingAndRaiseEvents();
		}
		void StartEventsRecording() {
			if(EventRaiser != null)
				EventRaiser.StartRecording();
		}
		void ThrowAsyncCallException() {
			throw new IncorrectAsyncOperationCallException();
		}
		void FreezeVisualItems() {
			VisualItemsInternal.IsReadOnly = true;
		}
		void UnfreezeVisualItems() {
			VisualItemsInternal.IsReadOnly = false;
			if(!IsQueueRunning) {
				InvalidateFieldItems();
			}
		}
#endregion
#region ILoadingPanelOwner Members
		PivotLoadingPanelType loadingPanelType;
		protected PivotLoadingPanelType LoadingPanelType {
			get { return loadingPanelType; }
			set { loadingPanelType = value; }
		}
		internal void SetLoadingPanelType(PivotLoadingPanelType type) {
			LoadingPanelType = type;
		}
		internal PivotLoadingPanelType GetLoadingPanelType() {
			return LoadingPanelType;
		}
		protected void SetDefaultLoadingPanelType() {
			SetLoadingPanelType(PivotLoadingPanelType.MainLoadingPanel);
		}
		protected virtual ActionDelayer LoadingPanelDelayer {
			get {
				if(delayer == null)
					delayer = new ActionDelayer(ShowLoadingPanelOnTimer, OptionsBehavior.LoadingPanelDelay);
				return delayer;
			}
		}
		void ShowMainLoadingPanel() {
			if(showMainLoadingPanel != null)
				showMainLoadingPanel.Raise(this, EventArgs.Empty);
		}
		void HideMainLoadingPanel() {
			if(hideMainLoadingPanel != null)
				hideMainLoadingPanel.Raise(this, EventArgs.Empty);
		}
		void ShowFilterPopupLoadingPanel() {
			if(showFilterPopupLoadingPanel != null)
				showFilterPopupLoadingPanel.Raise(this, EventArgs.Empty);
		}
		void HideFilterPopupLoadingPanel() {
			if(hideFilterPopupLoadingPanel != null)
				hideFilterPopupLoadingPanel.Raise(this, EventArgs.Empty);
		}
		protected void ShowLoadingPanel() {
			StartLoadingPanelTimer();
		}
		protected virtual void StartLoadingPanelTimer() {
			if(!IsQueueRunning)
				LoadingPanelDelayer.Invoke(OptionsBehavior.LoadingPanelDelay);
		}
		protected virtual void ShowLoadingPanelOnTimer() {
			ShowLoadingPanelInternal();
		}
		protected virtual void ShowLoadingPanelInternal() {
			switch(LoadingPanelType) {
				case PivotLoadingPanelType.FilterPopupLoadingPanel:
					ShowFilterPopupLoadingPanel();
					break;
				case PivotLoadingPanelType.MainLoadingPanel:
					ShowMainLoadingPanel();
					break;
			}
		}
		protected void HideLoadingPanel() {
			if(!LoadingPanelDelayer.IsInDelay)
				HideLoadingPanelInternal();
			else
				LoadingPanelDelayer.Cancel();
		}
		protected virtual void HideLoadingPanelInternal() {
			switch(LoadingPanelType) {
				case PivotLoadingPanelType.FilterPopupLoadingPanel:
					HideFilterPopupLoadingPanel();
					break;
				case PivotLoadingPanelType.MainLoadingPanel:
					HideMainLoadingPanel();
					break;
			}
			SetDefaultLoadingPanelType();
		}
		WeakEventHandler<EventArgs, EventHandler> showMainLoadingPanel;
		event EventHandler ILoadingPanelOwner.ShowMainLoadingPanel {
			add { this.showMainLoadingPanel += value; }
			remove { this.showMainLoadingPanel -= value; }
		}
		WeakEventHandler<EventArgs, EventHandler> hideMainLoadingPanel;
		event EventHandler ILoadingPanelOwner.HideMainLoadingPanel {
			add { this.hideMainLoadingPanel += value; }
			remove { this.hideMainLoadingPanel -= value; }
		}
		WeakEventHandler<EventArgs, EventHandler> showFilterPopupLoadingPanel;
		event EventHandler ILoadingPanelOwner.ShowFilterPopupLoadingPanel {
			add { this.showFilterPopupLoadingPanel += value; }
			remove { this.showFilterPopupLoadingPanel -= value; }
		}
		WeakEventHandler<EventArgs, EventHandler> hideFilterPopupLoadingPanel;
		event EventHandler ILoadingPanelOwner.HideFilterPopupLoadingPanel {
			add { this.hideFilterPopupLoadingPanel += value; }
			remove { this.hideFilterPopupLoadingPanel -= value; }
		}
		bool ILoadingPanelOwner.IsMainLoadingPanelVisible {
			get { return IsLoadingPanelVisible && LoadingPanelType == PivotLoadingPanelType.MainLoadingPanel; }
		}
		bool ILoadingPanelOwner.IsFilterPopupLoadingPanelVisible {
			get { return IsLoadingPanelVisible && LoadingPanelType == PivotLoadingPanelType.FilterPopupLoadingPanel; }
		}
		bool IsLoadingPanelVisible { get { return (IsQueueRunning || IsInProcessing) && !LoadingPanelDelayer.IsInDelay; } }
#endregion
#region Actions Queue
		ActionsQueue queue;
		public event EventHandler QueueStarting {
			add { Queue.QueueStarting += value; }
			remove { Queue.QueueStarting -= value; }
		}
		public event EventHandler QueueCompleted { 
			add { Queue.QueueCompleted += value; }
			remove { Queue.QueueCompleted -= value; }
		}
		public event EventHandler ActionStarting {
			add { Queue.ActionStarting += value; }
			remove { Queue.ActionStarting -= value; }
		}
		public event EventHandler ActionCompleted {
			add { Queue.ActionCompleted += value; }
			remove { Queue.ActionCompleted -= value; }
		}
		protected ActionsQueue Queue {
			get {
				if(queue == null) {
					queue = new ActionsQueue();
					queue.QueueStarting += (s, e) => ShowLoadingPanel();
					queue.QueueCompleted += (s, e) => {
						Helper.Clear();
						AsyncProcessFinishing();
						HideLoadingPanel();
					};
				}
				return queue;
			}
		}
		public bool IsQueueRunning { get { return Queue.IsQueueRunning; } }
		public void SetQueueContext(IActionContext context) {
			Queue.SetQueueContext(context);
			Queue.SetIsQueueAsync(UseAsyncMode);
		}
		public void SetIsQueueAsync(bool isAsync) {
			Queue.SetIsQueueAsync(UseAsyncMode);
		}
		public void EnqueueAction(Action action) {
			Queue.EnqueueAction(action);
		}
		public void EnqueueDelayed(Action action) {
			Queue.EnqueueDelayed(action);
		}
		public void RunQueue() {
			Queue.RunQueue();
		}
		public void CompleteAction() {
			Queue.CompleteAction();
		}
#endregion
	}
}
