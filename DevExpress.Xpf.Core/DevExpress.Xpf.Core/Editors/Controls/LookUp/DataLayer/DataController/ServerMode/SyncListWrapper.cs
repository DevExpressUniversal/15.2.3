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
using System.Linq;
using System.Reflection;
using DevExpress.Data;
using DevExpress.Data.Async;
using DevExpress.Data.Async.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class SyncRowInfo {
		public object RowKey { get; private set; }
		public object Row { get; private set; }
		public SyncRowInfo(object rowKey, object row) {
			RowKey = rowKey;
			Row = row;
		}
	}
	public class SyncListWrapper : ITypedList, IBindingList, IDisposable {
		IListServer originalServer;
		IListServer server;
		int count;
		Dictionary<int, SyncRowInfo> rows;
		IListServerDataView view;
		string lastFilterCriteria;
		int lastGroupCount;
		IList<ServerModeOrderDescriptor> lastSorting;
		public Dictionary<int, SyncRowInfo> Rows { get { return rows; } }
		public SyncListWrapper(IListServer server) {
			this.originalServer = server;
		}
		public void Initialize(IListServerDataView view) {
			this.view = view;
			this.server = (IListServer)((IDXCloneable)originalServer).DXClone();
			this.rows = new Dictionary<int, SyncRowInfo>();
			server.InconsistencyDetected += ServerOnInconsistencyDetected;
			server.ExceptionThrown += ServerOnExceptionThrown;
		}
		void ServerOnExceptionThrown(object sender, ServerModeExceptionThrownEventArgs e) {
			view.NotifyExceptionThrown(e);
		}
		void ServerOnInconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs e) {
			Reset();
			view.NotifyInconsistencyDetected(e);
		}
		public virtual void Dispose() {
			IDisposable d = Server as IDisposable;
			if (d != null)
				d.Dispose();
			server.InconsistencyDetected -= ServerOnInconsistencyDetected;
			server.ExceptionThrown -= ServerOnExceptionThrown;
			this.view = null;
			originalServer = null;
		}
		public IListServerDataView View {
			get { return view; }
		}
		public IListServer Server {
			get { return server; }
		}
		public int FindRowByText(CriteriaOperator expression, string text, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			return FindIncremental(expression, text, startItemIndex, !searchNext, ignoreStartIndex, false);
		}
		public int FindRowByValue(CriteriaOperator expression, object value) {
			return FindByValue(expression, value);
		}
		public void CancelRow(int listSourceIndex) {
		}
		SyncRowInfo GetRowInfo(int index) {
			SyncRowInfo info;
			if (rows.TryGetValue(index, out info))
				return info;
			return null;
		}
		internal object GetRow(int index) {
			SyncRowInfo info = GetRowInfo(index);
			if (info == null) {
				if (index < 0 || index >= Count)
					return AsyncServerModeDataController.NoValue;
				object rowKey = Server.GetRowKey(index);
				object row = Server[index];
				info = new SyncRowInfo(rowKey, row);
				rows[index] = info;
			}
			return info.Row;
		}
		object IList.this[int index] {
			get { return GetRow(index); }
			set { throw new Exception("The method or operation is not implemented."); }
		}
		public virtual int Count {
			get { return count; }
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
		public void Reset() {
			count = Server.Count;
			rows.Clear();
			view.Reset();
		}
		int FindByValue(CriteriaOperator expression, object value, int startIndex = -1, bool searchUp = false) {
			return Server.LocateByValue(expression, value, startIndex, searchUp);
		}
		int FindIncremental(CriteriaOperator expression, string text, int startRow, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			return Server.FindIncremental(expression, text, startRow, searchUp, ignoreStartRow, allowLoop);
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return string.Empty;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			ITypedList server = (ITypedList)Server;
			return server.GetItemProperties(listAccessors);
		}
		public void ApplySortGroupFilter(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sorting, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo = null,
			ICollection<ServerModeSummaryDescriptor> totalSummaryInfo = null) {
			ApplySortGroupFilterToServer(filterCriteria, sorting, groupCount, groupSummaryInfo, totalSummaryInfo);
		}
		void ApplySortGroupFilterToServer(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sorting, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo = null,
			ICollection<ServerModeSummaryDescriptor> totalSummaryInfo = null) {
			if (AreSameSettings(filterCriteria, sorting, groupCount, groupSummaryInfo, totalSummaryInfo))
				return;
			UpdateLastSortGroupFilterSettings(filterCriteria, sorting, groupCount);
			Server.Apply(filterCriteria, sorting, groupCount, groupSummaryInfo, totalSummaryInfo);
			Reset();
		}
		void UpdateLastSortGroupFilterSettings(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sorting, int groupCount) {
			lastSorting = sorting as IList<ServerModeOrderDescriptor> ?? sorting.Return(x => x.ToList(), () => new List<ServerModeOrderDescriptor>());
			lastGroupCount = groupCount;
			lastFilterCriteria = filterCriteria.With(x => x.ToString());
		}
		bool AreSameSettings(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sorting, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			if (groupSummaryInfo.If(x => x.Count > 0).ReturnSuccess() || totalSummaryInfo.If(x => x.Count > 0).ReturnSuccess())
				return false;
			string currentFilterCriteria = filterCriteria.With(x => x.ToString());
			if (lastFilterCriteria != currentFilterCriteria)
				return false;
			if (lastGroupCount != groupCount)
				return false;
			var currentSorting = sorting as IList<ServerModeOrderDescriptor> ?? sorting.Return(x => x.ToList(), () => new List<ServerModeOrderDescriptor>());
			if (lastSorting.Return(x => x.Count, () => -1) != currentSorting.Return(x => x.Count, () => -1))
				return false;
			for (int i = 0; i < currentSorting.Count; i++) {
				if (lastSorting[i].ToString() != currentSorting[i].ToString())
					return false;
			}
			return true;
		}
		public object GetLoadedRowKey(int index) {
			return index;
		}
		public void Refresh() {
			server.Refresh();
			Reset();
		}
	}
}
