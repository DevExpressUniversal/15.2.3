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
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Async;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using DevExpress.Data.Details;
using System.Windows.Forms;
#else
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DictionaryEntry = System.Collections.Generic.KeyValuePair<object, object>;
#endif
namespace DevExpress.Data.Helpers {
public enum AsyncState { Invalid, Requested, Valid }
	public class AsyncRowsInfo {
		int invalidRowCount = 0;
		DateTime? rowsClearTime = null;
		Dictionary<int, AsyncRowInfo> rows;
		public int Count { get { return Rows.Count; } }
		public void MakeAllRowsInvalid() {
			this.invalidRowCount = 0;
			this.rowsClearTime = DateTime.Now;
			List<int> indexesToRemove = null;
			foreach(KeyValuePair<int, AsyncRowInfo> row in Rows) {
				if(row.Value.IsLoading) {
					if(row.Value.Row == null) {
						if(indexesToRemove == null) indexesToRemove = new List<int>();
						indexesToRemove.Add(row.Key);
						continue;
					}
				}
				invalidRowCount++;
				row.Value.MakeInvalid();
			}
			if(invalidRowCount == 0) this.rowsClearTime = null;
			if(indexesToRemove != null) {
				foreach(int i in indexesToRemove) rows.Remove(i);
			}
		}
		public void CheckRemoveInvalidRows(bool force) {
			if(invalidRowCount == 0) return;
			if(DateTime.Now.Subtract(rowsClearTime.Value).TotalSeconds < 5 && !force) return;
			Dictionary<int, AsyncRowInfo> newRows = new Dictionary<int, AsyncRowInfo>();
			foreach(KeyValuePair<int, AsyncRowInfo> row in Rows) {
				if(row.Value.IsValid) {
					newRows[row.Key] = row.Value;
				}
			}
			this.rows = newRows;
			this.invalidRowCount = 0;
		}
		protected internal Dictionary<int, AsyncRowInfo> Rows {
			get {
				if(rows == null) rows = new Dictionary<int, AsyncRowInfo>();
				return rows;
			}
		}
		public AsyncRowInfo GetRow(int index) {
			AsyncRowInfo value;
			if(!Rows.TryGetValue(index, out value)) return null;
			return value;
		}
		public bool IsRowLoaded(int index) {
			AsyncRowInfo value = GetRow(index);
			return value != null && value.IsValid;
		}
		public void Add(int index, AsyncRowInfo info) {
			Remove(index);
			rows[index] = info;
		}
		public void Remove(int index) {
			AsyncRowInfo value = GetRow(index);
			if(value == null) return;
			Rows.Remove(index);
			if(!value.IsValid) this.invalidRowCount --;
		}
		public void OnLoaded(int index, object rowObject, object key) {
			Remove(index);
			Add(index, new AsyncRowInfo(rowObject, key));
		}
		public List<KeyValuePair<int, AsyncRowInfo>> GetLoadingRows() {
			List<KeyValuePair<int, AsyncRowInfo>> list = null;
			foreach(KeyValuePair<int, AsyncRowInfo> info in Rows) {
				if(info.Value.IsLoading) {
					if(list == null) list = new List<KeyValuePair<int, AsyncRowInfo>>();
					list.Add(info);
				}
			}
			return list;
		}
	}
	public class AsyncRowInfo {
		bool isValid;
		Command loadingCommand;
		object row, key;
		public AsyncRowInfo(Command loadingCommand) {
			this.loadingCommand = loadingCommand;
			this.isValid = true;
			this.row = null;
			this.key = null;
		}
		public AsyncRowInfo(object row, object key) {
			this.loadingCommand = null;
			this.row = row;
			this.isValid = true;
			this.key = key;
		}
		public object Row {
			get { return row; }
			set {
				row = value;
				isValid = true;
			}
		}
		public object Key {
			get {
				if(!IsLoaded) return null;
				return key;
			}
		}
		public void MakeLoading(Command loading) {
			this.loadingCommand = loading;
		}
		public Command LoadingCommand { get { return loadingCommand; } }
		public bool IsLoading { get { return loadingCommand != null; } }
		public bool IsValid { get { return isValid; } }
		public bool IsLoaded { get { return !IsLoading && IsValid; } }
		public void MakeInvalid() { 
			isValid = false;
			this.loadingCommand = null;
		}
	}
	public class AsyncListWrapper : IBindingList, ITypedList, IAsyncResultReceiver, IDisposable {
		IAsyncListServer server;
		int count = 0;
		AsyncRowsInfo rows;
#if DEBUGTEST
		public AsyncRowsInfo Rows { get { return rows; } }
#endif
		AsyncState state = AsyncState.Invalid;
		AsyncServerModeDataController controller;
		public AsyncListWrapper(AsyncServerModeDataController controller, IAsyncListServer server) {
			this.controller = controller;
			this.server = server;
			this.rows = new AsyncRowsInfo();
			this.server.SetReceiver(this);
		}
		public virtual void Dispose() {
			if(server != null) server.SetReceiver(null);
		}
		public AsyncServerModeDataController Controller { get { return controller; } }
		public IAsyncListServer Server { get { return server; } }
		public virtual void Invalidate() {
			if(state != AsyncState.Invalid) return;
			this.state = AsyncState.Requested;
			Server.GetTotals();
			Controller.OnTotalsRequested();
		}
		public virtual bool IsValidState { get { return state == AsyncState.Valid; } }
		public virtual void ResetValidate() {
			this.state = AsyncState.Invalid;
			rows.MakeAllRowsInvalid();
		}
		void SetInvalidState() {
			ResetValidate();
			VisualClient.UpdateLayout();
		}
		internal void CancelAllGetRows() {
			List<KeyValuePair<int, AsyncRowInfo>> list = rows.GetLoadingRows();
			if(list == null) return;
			for(int n = 0; n < list.Count; n++) {
				KeyValuePair<int, AsyncRowInfo> pair = list[n];
				if(pair.Value.IsLoading) {
					rows.Remove(pair.Key);
					Server.Cancel(pair.Value.LoadingCommand);
				}
			}
		}
		public bool IsRowLoaded(int index) {
			if(!IsValidState) return false;
			AsyncRowInfo rowInfo = GetRowInfo(index);
			return rowInfo != null && rowInfo.IsLoaded;
		}
		AsyncRowInfo GetRowInfo(int index) {
			if(index >= Count) return null;
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
		internal object GetRowInfo(int index, OperationCompleted completed) {
			AsyncRowInfo rowInfo = GetLoadedValidRowInfo(index);
			if(rowInfo != null) return rowInfo;
			object res = GetRow(index, (a) => {
				rowInfo = GetLoadedValidRowInfo(index);
				if(completed != null) completed(rowInfo);
			});
			if(res == AsyncServerModeDataController.NoValue) return AsyncServerModeDataController.NoValue;
			return GetLoadedValidRowInfo(index);
		}
		internal object GetRow(int index, OperationCompleted completed) {
			AsyncRowInfo rowInfo = GetRowInfo(index);
			if(rowInfo == null) {
				if(state == AsyncState.Invalid) return AsyncServerModeDataController.NoValue;
				rows.Add(index, new AsyncRowInfo(Server.GetRow(index, AsyncOperationCompletedHelper.GetCommandParameter(completed))));
				return AsyncServerModeDataController.NoValue;
			}
			if(rowInfo.IsLoading) {
				if(completed != null)
					AsyncOperationCompletedHelper.AppendCompletedDelegate(rowInfo.LoadingCommand, completed);
				if(rowInfo.Row == null) return AsyncServerModeDataController.NoValue;
				return rowInfo.Row;
			}
			if(state == AsyncState.Invalid) return rowInfo.Row;
			if(rowInfo.IsValid) return rowInfo.Row;
			rowInfo.MakeLoading(Server.GetRow(index, AsyncOperationCompletedHelper.GetCommandParameter(completed)));
			return rowInfo.Row;
		}
		object IList.this[int index] {
			get {
				return GetRow(index, null);
			}
			set {
				throw new Exception("The method or operation is not implemented.");
			}
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
		protected IDataControllerVisualClient VisualClient { get { return Controller.VisualClient; } }
		protected virtual void NotifyRowReceived(int rowIndex) {
			Controller.OnAsyncRowReceived(rowIndex);
		}
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return ((ITypedList)Server).GetItemProperties(listAccessors);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return ((ITypedList)Server).GetListName(listAccessors);
		}
		#endregion
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) {
			throw new Exception("The method or operation is not implemented.");
		}
		object IBindingList.AddNew() {
			throw new Exception("The method or operation is not implemented.");
		}
		bool IBindingList.AllowEdit { get { return false; } }
		bool IBindingList.AllowNew { get { return false; } }
		bool IBindingList.AllowRemove { get { return false; } }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new Exception("The method or operation is not implemented.");
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			throw new Exception("The method or operation is not implemented.");
		}
		bool IBindingList.IsSorted { get { return false; } }
		event ListChangedEventHandler IBindingList.ListChanged {
			add { throw new Exception("The method or operation is not implemented."); }
			remove { throw new Exception("The method or operation is not implemented."); }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
			throw new Exception("The method or operation is not implemented.");
		}
		void IBindingList.RemoveSort() {
			throw new Exception("The method or operation is not implemented.");
		}
		ListSortDirection IBindingList.SortDirection { get { return ListSortDirection.Ascending; } }
		PropertyDescriptor IBindingList.SortProperty { get { return null; } }
		bool IBindingList.SupportsChangeNotification { get { return false; } }
		bool IBindingList.SupportsSearching { get { return false; } }
		bool IBindingList.SupportsSorting { get { return false; } }
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
		bool IList.IsFixedSize { get { return false; } }
		bool IList.IsReadOnly { get { return true; } }
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
		bool ICollection.IsSynchronized { get { return true; } }
		object ICollection.SyncRoot { get { return this; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
		int lastCount = 0;
		#region IAsyncResultReceiver Members
		void IAsyncCommandVisitor.Visit(CommandGetTotals result) {
			this.state = AsyncState.Valid;
			this.count = result.Count;
			Controller.OnAsyncTotalsReceived(result);
			if(this.lastCount != this.count) Controller.RaiseVisibleRowCountChanged();
			this.lastCount = this.count;
		}
		void IAsyncCommandVisitor.Visit(CommandGetRow result) {
			if(state != AsyncState.Valid) return;
			this.rows.CheckRemoveInvalidRows(false);
			int n = 0;
			for(; ; ) {
				OperationCompleted completed = AsyncOperationCompletedHelper.GetCompletedDelegate(result);
				int rowIndex = result.Index;
				rows.OnLoaded(rowIndex, result.Row, result.RowKey);
				NotifyRowReceived(rowIndex);
				if(completed != null) {
					completed(result.Row);
				}
				if(n++ > 100) break;
				result = Server.PullNext<CommandGetRow>();
				if(result == null) break;
			}
		}
		void IAsyncCommandVisitor.Visit(CommandApply result) {
			Controller.LastErrorText = string.Empty;
			Invalidate();
		}
		void IAsyncCommandVisitor.Visit(CommandRefresh result) {
			Controller.VisualClientUpdateLayout();
		}
		void IAsyncCommandVisitor.Visit(CommandGetRowIndexByKey result) {
			OperationCompleted completed = AsyncOperationCompletedHelper.GetCompletedDelegate(result);
			if(completed != null) completed(result);
		}
		void IAsyncCommandVisitor.Visit(CommandGetGroupInfo result) {
			List<CommandGetGroupInfo> results = new List<CommandGetGroupInfo>();
			results.Add(result);
			for(int n = 0; n < 100; n++) {
				result = Server.PullNext<CommandGetGroupInfo>();
				if(result == null) break;
				results.Add(result);
			}
			if(results[0].ParentGroup != null && !Controller.IsAllowRequestMoreAutoExpandGroups()) return;
			Controller.OnAsyncGroupInfoReceived(results);
		}
		void IAsyncCommandVisitor.Visit(CommandGetUniqueColumnValues result) {
			OperationCompleted completed = AsyncOperationCompletedHelper.GetCompletedDelegate(result);
			if(completed != null)
				completed(result.Values);
		}
		void IAsyncCommandVisitor.Visit(CommandFindIncremental result) {
			OperationCompleted completed = AsyncOperationCompletedHelper.GetCompletedDelegate(result);
			if(completed != null) 
				completed(result.RowIndex < 0 ? DataController.InvalidRow : Controller.GetControllerRow(result.RowIndex));
		}
		void IAsyncCommandVisitor.Visit(CommandLocateByValue result) {
			OperationCompleted completed = AsyncOperationCompletedHelper.GetCompletedDelegate(result);
			if(completed != null)
				completed(result.RowIndex < 0 ? DataController.InvalidRow : Controller.GetControllerRow(result.RowIndex));
		}
		void IAsyncCommandVisitor.Canceled(Command canceled) {
			CommandGetUniqueColumnValues uniq = canceled as CommandGetUniqueColumnValues;
			if(uniq != null) {
				var complete = AsyncOperationCompletedHelper.GetCompletedDelegate(canceled);
				if(complete != null)
					complete(null);
			}
		}
		void IAsyncResultReceiver.Notification(NotificationInconsistencyDetected notification) {
			this.isGroupExpandInProgress = Controller.requireExpandAll;
		}
		bool? isGroupExpandInProgress = null;
		void IAsyncResultReceiver.Notification(NotificationExceptionThrown exception) {
			this.isGroupExpandInProgress = Controller.requireExpandAll;
			Controller.LastErrorText = exception.Notification.Message;
		}
		void IAsyncResultReceiver.BusyChanged(bool busy) {
			Controller.OnAsyncBusyChanged(busy);
		}
		void IAsyncResultReceiver.Refreshing(CommandRefresh refreshCommand) {
			SetInvalidState();
			bool prevRequireExpandAll = Controller.requireExpandAll;
			Controller.LastErrorText = string.Empty;
			Controller.DoRefresh();
			if(prevRequireExpandAll && isGroupExpandInProgress.HasValue && isGroupExpandInProgress.Value) {
				Controller.requireExpandAll = true;
			}
			this.isGroupExpandInProgress = null;
		}
		void IAsyncResultReceiver.PropertyDescriptorsRenewed() {
			Controller.RePopulateColumns();
			Controller.VisualClientRequestSynchronization();
			Controller.DoRefresh();
			Controller.VisualClient.UpdateLayout();
			Controller.VisualClient.ColumnsRenewed();
		}
		void IAsyncCommandVisitor.Visit(CommandGetAllFilteredAndSortedRows command) {
		}
		void IAsyncCommandVisitor.Visit(CommandPrefetchRows command) {
		}
		#endregion
		internal void ApplySort() {
			Controller.LastErrorText = "";
			if(Controller.Helper.lastPropertyDescriptorCount > 0) {
				Server.Apply(Controller.ClientUserSubstituteFilter(UnboundCriteriaInliner.Process(Controller.FilterCriteria, Controller.Columns)),
					AsyncServerModeDataController.GetSortCollection(Controller),
					Controller.SortInfo.GroupCount,
					AsyncServerModeDataController.ListSourceSummaryItemsToServerModeSummaryDescriptors(Controller.GroupSummary.GetSummaryItems(Controller.AllowSortUnbound)),
					AsyncServerModeDataController.ListSourceSummaryItemsToServerModeSummaryDescriptors(Controller.TotalSummary.GetSummaryItems(Controller.AllowSortUnbound)));
			}
			SetInvalidState();
			Invalidate();
		}
		internal void FindIncremental(CriteriaOperator expression, string text, int startRow, bool searchUp, bool ignoreStartRow, bool allowLoop, OperationCompleted completed) {
			Server.FindIncremental(expression, text, startRow, searchUp, ignoreStartRow, allowLoop, AsyncOperationCompletedHelper.GetCommandParameter(completed));
		}
		internal int FindRowByValue(DataColumnInfo colInfo, object value) {
			try {
				value = colInfo.ConvertValue(value);
			}
			catch {
				return DataController.InvalidRow;
			}
			foreach(KeyValuePair<int, AsyncRowInfo> pair in rows.Rows) {
				AsyncRowInfo row = pair.Value;
				if(!row.IsLoaded) continue;
				object cellValue = Controller.Helper.GetRowValue(pair.Key, colInfo.Index, null);
				if(ValueComparer.Equals(cellValue, value)) return pair.Key;
			}
			return DataController.InvalidRow;
		}
	}
}
