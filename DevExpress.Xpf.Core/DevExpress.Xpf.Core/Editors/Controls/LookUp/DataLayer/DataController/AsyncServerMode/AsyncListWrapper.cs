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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Data;
using DevExpress.Data.Async;
using DevExpress.Data.Async.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncListWrapper : ITypedList, IBindingList, IAsyncResultReceiver, IDisposable {
		IAsyncListServer originalServer;
		IAsyncListServer server;
		int count;
		int groupCount;
		ICollection<ServerModeOrderDescriptor> actualSorting;
		CriteriaOperator filterCriteriaOperator;
		AsyncRowsInfo rows;
		IAsyncListServerDataView view;
		public AsyncRowsInfo Rows { get { return rows; } }
		public AsyncState State { get; set; }
		public bool IsReady { get { return State == AsyncState.Valid; } }
		public AsyncListWrapper(IAsyncListServer server) {
			this.originalServer = server;
			State = AsyncState.Invalid;
		}
		public void Initialize(IAsyncListServerDataView view) {
			this.view = view;
			this.server = (IAsyncListServer)((IDXCloneable)originalServer).DXClone();
			this.rows = new AsyncRowsInfo();
			SetInvalidState();
		}
		public void SetReceiver(IAsyncResultReceiver receiver) {
			Server.SetReceiver(receiver);
		}
		public virtual void Dispose() {
			IDisposable d = Server as IDisposable;
			if (d != null)
				d.Dispose();
			this.view = null;
			originalServer = null;
			SetInvalidState();
		}
		public IAsyncListServerDataView View {
			get { return view; }
		}
		public IAsyncListServer Server {
			get { return server; }
		}
		public virtual void Invalidate() {
			if (State != AsyncState.Invalid)
				return;
			this.State = AsyncState.Requested;
			Server.GetTotals();
		}
		public virtual void ResetValidate() {
			this.State = AsyncState.Invalid;
			rows.MakeAllRowsInvalid();
		}
		void SetInvalidState() {
			ResetValidate();
		}
		public void FindRowByText(CriteriaOperator expression, string text, int startItemIndex, bool searchNext, bool ignoreStartIndex, OperationCompleted completed) {
			if (!IsReady) {
				return;
			}
			FindIncremental(expression, text, startItemIndex, !searchNext, ignoreStartIndex, false, completed);
		}
		public void FindRowByValue(CriteriaOperator expression, object value, OperationCompleted completed) {
			if (!IsReady)
				return;
			FindByValue(expression, value, completed: completed);
		}
		internal void CancelAllGetRows() {
			List<KeyValuePair<int, AsyncRowInfo>> list = rows.GetLoadingRows();
			if (list == null)
				return;
			for (int n = 0; n < list.Count; n++) {
				KeyValuePair<int, AsyncRowInfo> pair = list[n];
				if (pair.Value.IsLoading) {
					rows.Remove(pair.Key);
					Server.Cancel(pair.Value.LoadingCommand);
				}
			}
		}
		public bool IsRowLoaded(int index) {
			if (!IsReady)
				return false;
			AsyncRowInfo rowInfo = GetRowInfo(index);
			return rowInfo != null && rowInfo.IsLoaded;
		}
		AsyncRowInfo GetRowInfo(int index) {
			if (index >= Count)
				return null;
			return rows.GetRow(index);
		}
		internal object GetLoadedRowKey(int index) {
			AsyncRowInfo rowInfo = GetRowInfo(index);
			return rowInfo == null ? null : rowInfo.Key;
		}
		internal AsyncRowInfo GetLoadedValidRowInfo(int index) {
			AsyncRowInfo rowInfo = GetRowInfo(index);
			return rowInfo != null && rowInfo.IsLoaded && rowInfo.IsValid ? rowInfo : null;
		}
		public void CancelRow(int listSourceIndex) {
			AsyncRowInfo row = GetRowInfo(listSourceIndex);
			if (row == null)
				return;
			if (row.IsLoading) {
				server.Cancel(row.LoadingCommand);
				rows.Remove(listSourceIndex);
			}
		}
		internal object GetRowInfo(int index, OperationCompleted completed) {
			AsyncRowInfo rowInfo = GetLoadedValidRowInfo(index);
			if (rowInfo != null)
				return rowInfo;
			object res = GetRow(index, (a) => {
				rowInfo = GetLoadedValidRowInfo(index);
				if (completed != null)
					completed(rowInfo);
			});
			if (res == AsyncServerModeDataController.NoValue)
				return AsyncServerModeDataController.NoValue;
			return GetLoadedValidRowInfo(index);
		}
		internal object GetRow(int index, OperationCompleted completed) {
			AsyncRowInfo rowInfo = GetRowInfo(index);
			if (rowInfo == null) {
				if (State == AsyncState.Invalid)
					return AsyncServerModeDataController.NoValue;
				if (index < 0 || index >= Count)
					return AsyncServerModeDataController.NoValue;
				rows.Add(index, new AsyncRowInfo(Server.GetRow(index, AsyncOperationCompletedHelper2.GetCommandParameter(completed))));
				return AsyncServerModeDataController.NoValue;
			}
			if (rowInfo.IsLoading) {
				if (completed != null)
					AsyncOperationCompletedHelper2.AppendCompletedDelegate(rowInfo.LoadingCommand, completed);
				if (rowInfo.Row == null)
					return AsyncServerModeDataController.NoValue;
				return rowInfo.Row;
			}
			if (State == AsyncState.Invalid)
				return rowInfo.Row;
			if (rowInfo.IsValid)
				return rowInfo.Row;
			rowInfo.MakeLoading(Server.GetRow(index, AsyncOperationCompletedHelper2.GetCommandParameter(completed)));
			return rowInfo.Row;
		}
		object IList.this[int index] {
			get { return GetRow(index, null); }
			set { throw new Exception("The method or operation is not implemented."); }
		}
		internal void ClearInvalidRowsCache() {
			rows.CheckRemoveInvalidRows(true);
		}
		public virtual int Count {
			get {
				Invalidate();
				return count;
			}
		}
		#region IBindingList Members
		event ListChangedEventHandler listChanged;
		event ListChangedEventHandler IBindingList.ListChanged {
			add { listChanged += value; }
			remove { listChanged -= value; }
		}
		protected void RaiseListChanged(ListChangedEventArgs e) {
			if (listChanged != null)
				listChanged(this, e);
		}
		void IBindingList.AddIndex(PropertyDescriptor property) {
			throw new Exception("The method or operation is not implemented.");
		}
		object IBindingList.AddNew() {
			throw new Exception("The method or operation is not implemented.");
		}
		bool IBindingList.AllowEdit {
			get { return false; }
		}
		bool IBindingList.AllowNew {
			get { return false; }
		}
		bool IBindingList.AllowRemove {
			get { return false; }
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new Exception("The method or operation is not implemented.");
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new Exception("The method or operation is not implemented.");
		}
		bool IBindingList.IsSorted {
			get { return false; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IBindingList.RemoveSort() {
			throw new Exception("The method or operation is not implemented.");
		}
		ListSortDirection IBindingList.SortDirection {
			get { return ListSortDirection.Ascending; }
		}
		PropertyDescriptor IBindingList.SortProperty {
			get { return null; }
		}
		bool IBindingList.SupportsChangeNotification {
			get { return false; }
		}
		bool IBindingList.SupportsSearching {
			get { return false; }
		}
		bool IBindingList.SupportsSorting {
			get { return false; }
		}
		#endregion
		#region IList Members
		int IList.Add(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IList.Clear() {
			throw new Exception("The method or operation is not implemented.");
		}
		bool IList.Contains(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		int IList.IndexOf(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IList.Insert(int index, object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		bool IList.IsFixedSize {
			get { return false; }
		}
		bool IList.IsReadOnly {
			get { return true; }
		}
		void IList.Remove(object value) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IList.RemoveAt(int index) {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			throw new Exception("The method or operation is not implemented.");
		}
		bool ICollection.IsSynchronized {
			get { return true; }
		}
		object ICollection.SyncRoot {
			get { return this; }
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
		#region IAsyncResultReceiver Members
		void IAsyncCommandVisitor.Visit(CommandGetTotals result) {
			if (count != result.Count) {
				this.count = result.Count;
				view.NotifyCountChanged();
			}
		}
		void IAsyncCommandVisitor.Visit(CommandGetRow result) {
			if (State != AsyncState.Valid)
				return;
			this.rows.CheckRemoveInvalidRows(false);
			int rowIndex = result.Index;
			rows.OnLoaded(rowIndex, result.Row, result.RowKey);
			OperationCompleted completed = AsyncOperationCompletedHelper2.GetCompletedDelegate(result);
			if (completed != null)
				completed(result);
		}
		void IAsyncCommandVisitor.Visit(CommandApply result) {
			this.State = AsyncState.Valid;
			view.NotifyApply();
		}
		void IAsyncCommandVisitor.Visit(CommandRefresh result) {
		}
		void IAsyncCommandVisitor.Visit(CommandGetRowIndexByKey result) {
			OperationCompleted completed = AsyncOperationCompletedHelper2.GetCompletedDelegate(result);
			if (completed != null)
				completed(result);
		}
		void IAsyncCommandVisitor.Visit(CommandGetGroupInfo result) {
		}
		void IAsyncCommandVisitor.Visit(CommandGetUniqueColumnValues result) {
			OperationCompleted completed = AsyncOperationCompletedHelper2.GetCompletedDelegate(result);
			if (completed != null)
				completed(result.Values);
		}
		void IAsyncCommandVisitor.Visit(CommandFindIncremental result) {
			OperationCompleted completed = AsyncOperationCompletedHelper2.GetCompletedDelegate(result);
			if (completed != null)
				completed(result);
		}
		void IAsyncCommandVisitor.Visit(CommandLocateByValue result) {
			OperationCompleted completed = AsyncOperationCompletedHelper2.GetCompletedDelegate(result);
			if (completed != null)
				completed(result);
		}
		void IAsyncCommandVisitor.Canceled(Command canceled) {
			CommandGetUniqueColumnValues uniq = canceled as CommandGetUniqueColumnValues;
			if (uniq != null) {
				var complete = AsyncOperationCompletedHelper2.GetCompletedDelegate(canceled);
				if (complete != null)
					complete(null);
			}
		}
		void IAsyncResultReceiver.Notification(NotificationInconsistencyDetected notification) {
			View.NotifyInconsistencyDetected(new ServerModeInconsistencyDetectedEventArgs(notification.Notification));
		}
		void IAsyncResultReceiver.Notification(NotificationExceptionThrown exception) {
			View.NotifyExceptionThrown(new ServerModeExceptionThrownEventArgs(exception.Notification));
		}
		void IAsyncResultReceiver.BusyChanged(bool busy) {
			View.BusyChanged(busy);
		}
		void IAsyncResultReceiver.Refreshing(CommandRefresh refreshCommand) {
		}
		void IAsyncResultReceiver.PropertyDescriptorsRenewed() {
		}
		void IAsyncCommandVisitor.Visit(CommandGetAllFilteredAndSortedRows command) {
		}
		void IAsyncCommandVisitor.Visit(CommandPrefetchRows command) {
		}
		#endregion
		public void Reset() {
			SetInvalidState();
			Invalidate();
		}
		void FindByValue(CriteriaOperator expression, object value, int startIndex = -1, bool searchUp = false, OperationCompleted completed = null) {
			Server.LocateByValue(expression, value, startIndex, searchUp, AsyncOperationCompletedHelper2.GetCommandParameter(completed));
		}
		void FindIncremental(CriteriaOperator expression, string text, int startRow, bool searchUp, bool ignoreStartRow, bool allowLoop, OperationCompleted completed) {
			Server.WeakCancel<CommandFindIncremental>();
			Server.FindIncremental(expression, text, startRow, searchUp, ignoreStartRow, allowLoop, AsyncOperationCompletedHelper2.GetCommandParameter(completed));
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return string.Empty;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			ITypedList server = (ITypedList)Server;
			SetReceiver(this);
			return server.GetItemProperties(listAccessors);
		}
		public void ApplySortGroupFilter(CriteriaOperator filterCriteria, List<ServerModeOrderDescriptor> sorting, int groupCount) {
			this.filterCriteriaOperator = filterCriteria;
			this.actualSorting = sorting;
			this.groupCount = groupCount;
			ApplySortGroupFilterToServer(filterCriteria, sorting, groupCount);
		}
		private void ApplySortGroupFilterToServer(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sorting, int groupCount) {
			Server.Apply(filterCriteria, sorting, groupCount, null, null);
			Reset();
		}
#if DEBUGTEST
		Func<AsyncListServer2DatacontrollerProxy, AsyncListServerCore> pumpAllHandler;
		Func<AsyncListServer2DatacontrollerProxy, AsyncListServerCore> PumpAllHandler {
			get {
				return pumpAllHandler ?? (pumpAllHandler = ReflectionHelper.CreateFieldGetter<AsyncListServer2DatacontrollerProxy, AsyncListServerCore>(
			  typeof(AsyncListServer2DatacontrollerProxy), "Nested", BindingFlags.Instance | BindingFlags.NonPublic));
			}
		}
		public void PumpAll() {
			PumpAllHandler((AsyncListServer2DatacontrollerProxy)Server).PumpAll();
		}
#endif
	}
}
