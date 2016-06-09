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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public abstract class AsyncServerModeCurrentDataView : CurrentDataView, IAsyncListServerDataView {
		string filterCriteria;
		string displayFilterCriteria;
		IList<SortingInfo> actualSorting;
		int groupCount;
		public bool IsBusy { get; private set; }
		public bool IsVisibleListBusy { get; private set; }
		new AsyncDataProxyViewCache View {
			get { return (AsyncDataProxyViewCache)base.View; }
		}
		readonly AsyncListWrapper wrapper;
		public AsyncListWrapper Wrapper {
			get { return wrapper; }
		}
		AsyncListServerDefaultDataView DefaultView {
			get { return (AsyncListServerDefaultDataView)ListSource; }
		}
		protected AsyncServerModeCurrentDataView(AsyncListServerDefaultDataView view, object handle, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups,
			IEnumerable<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria)
			: base(view, handle, valueMember, displayMember) {
			this.filterCriteria = filterCriteria;
			this.displayFilterCriteria = displayFilterCriteria;
			List<SortingInfo> resultSorting = groups != null ? groups.Select(x => new SortingInfo(x.FieldName, ListSortDirection.Ascending)).ToList() : new List<SortingInfo>();
			resultSorting.AddRange(sorts);
			actualSorting = resultSorting;
			groupCount = groups.Count();
			wrapper = CreateWrapper(view);
		}
		protected abstract AsyncListWrapper CreateWrapper(AsyncListServerDefaultDataView view);
		public virtual void InitializeSource() {
			ApplySortFilterForWrapper(Wrapper);
		}
		public virtual void InitializeVisibleList() {
			ApplySortFilterForVisibleListWrapper((AsyncVisibleListWrapper)GetVisibleListInternal());
		}
		protected internal override bool GetIsAsyncServerMode() {
			return true;
		}
		protected internal override bool GetIsOwnSearchProcessing() {
			return true;
		}
		protected override int FindListSourceIndexByValue(object value) {
			return View.FindIndexByValue(ListServerDataViewBase.CreateCriteriaForValueColumn(DataAccessor), value);
		}
		protected override object DataControllerAdapterGetRowInternal(int visibleIndex) {
			var proxy = GetProxyByIndex(visibleIndex);
			if (proxy == null)
				return -1;
			return GetItemByProxy(proxy);
		}
		protected override object CreateVisibleListWrapper() {
			return new AsyncVisibleListWrapper(this);
		}
		public bool IsRowLoaded(int visibleIndex) {
			return View.IsRowLoaded(visibleIndex);
		}
		public object GetTempProxy(int listSourceIndex) {
			return View.TempProxy;
		}
		public void FetchItem(int visibleIndex, OperationCompleted action = null) {
			View.FetchItem(visibleIndex, action);
		}
		public void FetchItem(object value) {
			IndexOfValue(value);
		}
		public void CancelItem(int visibleIndex) {
			View.CancelItem(visibleIndex);
		}
		protected override int FindItemIndexByTextInternal(string text, bool isCaseSensitive, bool autoComplete, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			var operand = (OperandProperty)ListServerDataViewBase.CreateCriteriaForDisplayColumn(DataAccessor);
			return View.FindIndexByText(operand, GetCompareCriteriaOperator(autoComplete, operand, new OperandValue(text)), text, isCaseSensitive, startItemIndex, searchNext, ignoreStartIndex);
		}
		public void ProcessVisibleListBusyChanged(bool busy) {
			ProcessBusyChanged(IsBusy, busy);
		}
		void IAsyncListServerDataView.BusyChanged(bool busy) {
			ProcessBusyChanged(busy, IsVisibleListBusy);
		}
		void ProcessBusyChanged(bool busy, bool visibleListBusy) {
			bool wasReallyBusy = IsBusy || IsVisibleListBusy;
			bool isReallyBusy = busy || visibleListBusy;
			IsBusy = busy;
			IsVisibleListBusy = visibleListBusy;
			if (wasReallyBusy != isReallyBusy)
				RaiseOnBusyChanged(new ItemsProviderOnBusyChangedEventArgs(isReallyBusy));
		}
		void IAsyncListServerDataView.NotifyCountChanged() {
		}
		void IAsyncListServerDataView.NotifyLoaded(int listSourceIndex) {
			if (listSourceIndex < 0)
				return;
			NotifyLoadedInternal(listSourceIndex);
		}
		void IAsyncListServerDataView.NotifyApply() {
			ResetDisplayTextCache();
			ItemsCache.Reset();
			RaiseRefreshNeeded();
		}
		protected virtual void NotifyLoadedInternal(int controllerIndex) {
			ProcessChangeItem(controllerIndex);
			RaiseRowLoaded(new ItemsProviderRowLoadedEventArgs(Handle, controllerIndex));
		}
		void IAsyncListServerDataView.NotifyFindIncrementalCompleted(string text, int startIndex, bool searchNext, bool ignoreStartIndex, int controllerIndex) {
			NotifyFindIncrementalInternal(text, startIndex, searchNext, ignoreStartIndex, controllerIndex);
		}
		protected virtual void NotifyFindIncrementalInternal(string text, int startIndex, bool searchNext, bool ignoreStartIndex, int controllerIndex) {
			UpdateDisplayTextCache(text, true, startIndex, searchNext, controllerIndex, ignoreStartIndex);
			if (!Wrapper.IsRowLoaded(controllerIndex)) {
				var completedHandlerContainer = new GetRowOnFindIncrementalCompleter(this, text, startIndex, searchNext, ignoreStartIndex);
				Wrapper.GetRow(controllerIndex, completedHandlerContainer.Completed);
				return;
			}
			var proxy = View[controllerIndex];
			object value = GetValueFromProxy(proxy);
			RaiseFindIncrementalCompleted(new ItemsProviderFindIncrementalCompletedEventArgs(text, startIndex, searchNext, ignoreStartIndex, value));
		}
		void IAsyncListServerDataView.NotifyInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			RaiseInconsistencyDetected();
		}
		void IAsyncListServerDataView.NotifyExceptionThrown(ServerModeExceptionThrownEventArgs e) {
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			Wrapper.Dispose();
		}
		public override void ProcessRowLoaded(object value) {
			FetchItem(value);
		}
		public override void ProcessRefreshed() {
			base.ProcessRefreshed();
			ResetDisplayTextCache();
			ItemsCache.Reset();
		}
		public override void ProcessFindIncrementalCompleted(string text, int startIndex, bool searchNext, bool ignoreStartIndex, object value) {
			int index = ItemsCache.IndexOfValue(value);
			if (index > -1) {
				IAsyncListServerDataView This = this;
				This.NotifyFindIncrementalCompleted(text, startIndex, searchNext, ignoreStartIndex, index);
			}
		}
		protected virtual void SetupGroupSortFilter(IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria) {
			this.filterCriteria = filterCriteria;
			List<SortingInfo> resultSorting = groups != null
				? groups.Select(x => new SortingInfo(x.FieldName, ListSortDirection.Ascending)).ToList()
				: new List<SortingInfo>();
			resultSorting.AddRange(sorts);
			this.actualSorting = resultSorting;
			this.groupCount = groups.Count();
		}
		protected virtual void ApplySortFilterForWrapper(AsyncListWrapper wrapper) {
			wrapper.ApplySortGroupFilter(CriteriaOperator.Parse(filterCriteria),
				actualSorting.Select(x => new ServerModeOrderDescriptor(new OperandProperty(x.FieldName), x.OrderBy == ListSortDirection.Descending)).ToList(), groupCount);
		}
		protected virtual void ApplySortFilterForVisibleListWrapper(AsyncVisibleListWrapper visibleListWrapper) {
			if (visibleListWrapper == null)
				return;
			var criteria = CriteriaOperator.And(new[] { CriteriaOperator.Parse(filterCriteria), CriteriaOperator.Parse(this.displayFilterCriteria) });
			visibleListWrapper.ApplyGroupSortFilter(groupCount, actualSorting, criteria.With(x => x.ToString()));
		}
		public override void CancelAsyncOperations() {
			base.CancelAsyncOperations();
			Wrapper.CancelAllGetRows();
			ProcessBusyChanged(false, false);
		}
#if DEBUGTEST
		public void PumpAll() {
			Wrapper.PumpAll();
			((AsyncVisibleListWrapper)GetVisibleListInternal()).Do(x => x.PumpAll());
		}
#endif
	}
}
