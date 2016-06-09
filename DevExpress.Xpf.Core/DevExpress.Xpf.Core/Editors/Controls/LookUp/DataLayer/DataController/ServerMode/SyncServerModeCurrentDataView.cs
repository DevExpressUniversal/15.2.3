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
	public abstract class SyncServerModeCurrentDataView : CurrentDataView, IListServerDataView {
		string filterCriteria;
		string displayFilterCriteria;
		IList<SortingInfo> actualSorting;
		int groupCount;
		public bool IsBusy { get; private set; }
		public bool IsVisibleListBusy { get; private set; }
		new SyncDataProxyViewCache View { get { return (SyncDataProxyViewCache)base.View; } }
		readonly SyncListWrapper wrapper;
		public SyncListWrapper Wrapper { get { return wrapper; } }
		protected SyncServerModeCurrentDataView(SyncListServerDefaultDataView view, object handle, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria)
			: base(view, handle, valueMember, displayMember) {
			SetupGroupSortFilter(groups, sorts, filterCriteria, displayFilterCriteria);
			wrapper = CreateWrapper(view);
		}
		protected abstract SyncListWrapper CreateWrapper(IListServerDataView view);
		public virtual void InitializeSource() {
			InitializeVisibleList();
		}
		public virtual void InitializeVisibleList() {
			ApplySortFilterForVisibleListWrapper((SyncVisibleListWrapper)GetVisibleListInternal());
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
			return new SyncVisibleListWrapper(this);
		}
		protected override int FindItemIndexByTextInternal(string text, bool isCaseSensitive, bool autoComplete, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			if (string.IsNullOrEmpty(text))
				return -1;
			var operand = (OperandProperty)ListServerDataViewBase.CreateCriteriaForDisplayColumn(DataAccessor);
			return View.FindIndexByText(operand, GetCompareCriteriaOperator(autoComplete && !string.IsNullOrEmpty(text), operand, new OperandValue(text)), text, isCaseSensitive, startItemIndex, searchNext, ignoreStartIndex);
		}
		public void ProcessVisibleListBusyChanged(bool busy) {
			ProcessBusyChanged(IsBusy, busy);
		}
		void ProcessBusyChanged(bool busy, bool visibleListBusy) {
			bool wasReallyBusy = IsBusy || IsVisibleListBusy;
			bool isReallyBusy = busy || visibleListBusy;
			IsBusy = busy;
			IsVisibleListBusy = visibleListBusy;
			if (wasReallyBusy != isReallyBusy)
				RaiseOnBusyChanged(new ItemsProviderOnBusyChangedEventArgs(isReallyBusy));
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
		}
		public override void ProcessRefreshed() {
			base.ProcessRefreshed();
			ItemsCache.Reset();
		}
		protected virtual void SetupGroupSortFilter(IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			this.filterCriteria = filterCriteria;
			this.displayFilterCriteria = displayFilterCriteria;
			List<SortingInfo> resultSorting = groups != null
				? groups.Select(x => new SortingInfo(x.FieldName, ListSortDirection.Ascending)).ToList()
				: new List<SortingInfo>();
			resultSorting.AddRange(sorts);
			this.actualSorting = resultSorting;
			this.groupCount = groups.Count();
		}
		protected virtual void ApplySortFilterForWrapper(SyncListWrapper wrapper) {
			var criteria = CriteriaOperator.And(new[] {CriteriaOperator.Parse(filterCriteria), CriteriaOperator.Parse(this.displayFilterCriteria)});
			wrapper.ApplySortGroupFilter(criteria, actualSorting.Select(x => new ServerModeOrderDescriptor(new OperandProperty(x.FieldName), x.OrderBy == ListSortDirection.Descending)).ToList(), groupCount);
		}
		protected virtual void ApplySortFilterForVisibleListWrapper(SyncVisibleListWrapper visibleListWrapper) {
			if (visibleListWrapper == null)
				return;
			var criteria = CriteriaOperator.And(new[] { CriteriaOperator.Parse(filterCriteria), CriteriaOperator.Parse(this.displayFilterCriteria) });
			visibleListWrapper.ApplyGroupSortFilter(groupCount, actualSorting, criteria.With(x => x.ToString()));
		}
		public override bool ProcessInconsistencyDetected() {
			var result = base.ProcessInconsistencyDetected();
			ItemsCache.Reset();
			Wrapper.Reset();
			View.Reset();
			return result;
		}
		protected void ProcessInconsistencyDetectedForVisibleListWrapper(SyncVisibleListWrapper visibleListWrapper) {
			if (visibleListWrapper == null)
				return;
			visibleListWrapper.ProcessInconsistencyDetected();
		}
		public override bool ProcessChangeSortFilter(IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			base.ProcessChangeSortFilter(groups, sorts, filterCriteria, displayFilterCriteria);
			SetupGroupSortFilter(groups, sorts, filterCriteria, displayFilterCriteria);
			InitializeSource();
			return true;
		}
		public override bool ProcessRefresh() {
			Wrapper.Refresh();
			View.Reset();
			ItemsCache.Reset();
			ProcessRefreshForVisibleListWrapper((SyncVisibleListWrapper)GetVisibleListInternal());
			return true;
		}
		protected void ProcessRefreshForVisibleListWrapper(SyncVisibleListWrapper visibleListWrapper) {
			if (visibleListWrapper == null)
				return;
			visibleListWrapper.Refresh();
		}
		void IListServerDataView.NotifyInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			ProcessInconsistencyDetected();
			RaiseInconsistencyDetected();
		}
		void IListServerDataView.NotifyExceptionThrown(ServerModeExceptionThrownEventArgs e) {
		}
		void IListServerDataView.Reset() {
			ResetDisplayTextCache();
			ItemsCache.Reset();
		}
	}
}
