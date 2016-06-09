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
using System.Threading;
using DevExpress.Data;
using DevExpress.Data.Async;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class SyncServerModeListSource : IListSource {
		readonly SyncVisibleListWrapper syncVisibleListWrapper;
		SyncVisibleListWrapper Wrapper { get { return syncVisibleListWrapper; } }
		public SyncServerModeListSource(SyncVisibleListWrapper syncVisibleListWrapper) {
			this.syncVisibleListWrapper = syncVisibleListWrapper;
			syncVisibleListWrapper.Initialize();
		}
		public IList GetList() {
			return new SyncServerModeListSourceWrapper(Wrapper);
		}
		public bool ContainsListCollection {
			get { return false; }
		}
	}
	public interface IListServerRefreshable {
		event EventHandler Refresh;
	}
	public class SyncServerModeListSourceWrapper : IListServer, IListServerHints, ITypedList, IDisposable, IListServerRefreshable {
		IListServer Server { get; set; }
		IList Wrapper { get; set; }
		IListServerHints Hints { get; set; }
		ITypedList List { get; set; }
		IDisposable Disposable { get; set; }
		public SyncServerModeListSourceWrapper(SyncVisibleListWrapper wrapper) {
			Server = wrapper;
			Wrapper = wrapper;
			Hints = wrapper;
			List = wrapper;
			wrapper.CollectionChanged += wrapper_CollectionChanged;
			Disposable = wrapper;
			SubscribeServer();
		}
		void wrapper_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			RaiseRefresh();
		}
		void SubscribeServer() {
			Server.InconsistencyDetected += ServerOnInconsistencyDetected;
			Server.ExceptionThrown += ServerOnExceptionThrown;
		}
		void UnsubscribeServer() {
			Server.InconsistencyDetected += ServerOnInconsistencyDetected;
			Server.ExceptionThrown += ServerOnExceptionThrown;
		}
		void ServerOnExceptionThrown(object sender, ServerModeExceptionThrownEventArgs args) {
			RaiseExceptionThrown(args);
		}
		void ServerOnInconsistencyDetected(object sender, ServerModeInconsistencyDetectedEventArgs args) {
			RaiseInconsistencyDetected(args);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return Wrapper.GetEnumerator();
		}
		void ICollection.CopyTo(Array array, int index) {
			Wrapper.CopyTo(array, index);
		}
		int ICollection.Count {
			get { return Wrapper.Count; }
		}
		object ICollection.SyncRoot {
			get { return Wrapper.SyncRoot; }
		}
		bool ICollection.IsSynchronized {
			get { return Wrapper.IsSynchronized; }
		}
		int IList.Add(object value) {
			return Wrapper.Add(value);
		}
		bool IList.Contains(object value) {
			return Wrapper.Contains(value);
		}
		void IList.Clear() {
			Wrapper.Clear();
		}
		int IList.IndexOf(object value) {
			return Wrapper.IndexOf(value);
		}
		void IList.Insert(int index, object value) {
			Wrapper.Insert(index, value);
		}
		void IList.Remove(object value) {
			Wrapper.Remove(value);
		}
		void IList.RemoveAt(int index) {
			Wrapper.RemoveAt(index);
		}
		object IList.this[int index] {
			get {
				DataProxy proxy = (DataProxy)Wrapper[index];
				return proxy.f_component;
			}
			set { throw new NotImplementedException(); }
		}
		bool IList.IsReadOnly {
			get { return Wrapper.IsReadOnly; }
		}
		bool IList.IsFixedSize {
			get { return Wrapper.IsFixedSize; }
		}
		void IListServer.Apply(CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo,
			ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			Server.Apply(filterCriteria, sortInfo, groupCount, groupSummaryInfo, totalSummaryInfo);
		}
		List<ListSourceGroupInfo> IListServer.GetGroupInfo(ListSourceGroupInfo parentGroup) {
			return Server.GetGroupInfo(parentGroup);
		}
		object[] IListServer.GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			return Server.GetUniqueColumnValues(expression, maxCount, includeFilteredOut);
		}
		List<object> IListServer.GetTotalSummary() {
			return Server.GetTotalSummary();
		}
		object IListServer.GetRowKey(int index) {
			return Server.GetRowKey(index);
		}
		int IListServer.GetRowIndexByKey(object key) {
			return Server.GetRowIndexByKey(key);
		}
		int IListServer.FindIncremental(CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow,
			bool allowLoop) {
			return Server.FindIncremental(expression, value, startIndex, searchUp, ignoreStartRow, allowLoop);
		}
		int IListServer.LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			return Server.LocateByValue(expression, value, startIndex, searchUp);
		}
		IList IListServer.GetAllFilteredAndSortedRows() {
			return Server.GetAllFilteredAndSortedRows();
		}
		bool IListServer.PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return Server.PrefetchRows(groupsToPrefetch, cancellationToken);
		}
		void IListServer.Refresh() {
			Server.Refresh();
		}
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected;
		void RaiseInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs args) {
			var id = InconsistencyDetected;
			if (id != null)
				id(this, args);
		}
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown;
		void RaiseExceptionThrown(ServerModeExceptionThrownEventArgs args) {
			var id = ExceptionThrown;
			if (id != null)
				id(this, args);
		}
		void IListServerHints.HintGridIsPaged(int pageSize) {
			Hints.HintGridIsPaged(pageSize);
		}
		void IListServerHints.HintMaxVisibleRowsInGrid(int rowsInGrid) {
			Hints.HintMaxVisibleRowsInGrid(rowsInGrid);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return List.GetListName(listAccessors);
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return List.GetItemProperties(listAccessors);
		}
		void IDisposable.Dispose() {
			Disposable.Do(x => {
				UnsubscribeServer();
				x.Dispose();
			});
			Wrapper = null;
			Server = null;
			Hints = null;
			List = null;
			Disposable = null;
		}
		EventHandler refresh;
		event EventHandler IListServerRefreshable.Refresh {
			add { refresh += value; }
			remove { refresh -= value; }
		}
		void RaiseRefresh() {
			if(refresh != null)
				refresh(this, EventArgs.Empty);
		}
	}
}
