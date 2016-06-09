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
using DevExpress.Data.WcfLinq.Helpers;
namespace DevExpress.Xpf.Editors.Helpers {
	public class SyncListServerDefaultDataView : ListServerDataViewBase, IListServerDataView {
		new SyncDataProxyViewCache View { get { return (SyncDataProxyViewCache)base.View; } }
		string filterCriteria;
		readonly IEnumerable<SortingInfo> actualSorting;
		readonly int groupCount;
		public SyncListWrapper Wrapper { get { return ListSource as SyncListWrapper; } }
		public SyncListServerDefaultDataView(IListServer server, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, string filter)
			: base(new SyncListWrapper(server), valueMember, displayMember) {
			this.filterCriteria = filter;
			List<SortingInfo> resultSorting = groups != null ? groups.Select(x => new SortingInfo(x.FieldName, ListSortDirection.Ascending)).ToList() : new List<SortingInfo>();
			resultSorting.AddRange(sorts);
			actualSorting = resultSorting;
			groupCount = groups.Count();
			Wrapper.Initialize(this);
		}
		public override CurrentDataView CreateCurrentDataView(object handle, IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			bool createFiltered = GetIsCurrentViewFIltered(groups, sorts, filterCriteria);
			return createFiltered
				? new SyncServerModeCurrentFilteredSortedDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember, groups, sorts, filterCriteria, displayFilterCriteria) as CurrentDataView
				: new SyncServerModeCurrentPlainDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember, groups, sorts, filterCriteria, displayFilterCriteria);
		}
		protected internal override bool GetIsAsyncServerMode() {
			return false;
		}
		protected internal override bool GetIsOwnSearchProcessing() {
			return true;
		}
		protected override void InitializeView(object source) {
			SetView(new SyncDataProxyViewCache(this));
			InitializeSource();
		}
		protected virtual void InitializeSource() {
			ApplySortFilterForWrapper(Wrapper);
		}
		protected virtual void ApplySortFilterForWrapper(SyncListWrapper wrapper) {
			wrapper.ApplySortGroupFilter(null, null, 0);
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			Wrapper.Dispose();
		}
		protected override object DataControllerAdapterGetRowInternal(int visibleIndex) {
			var proxy = GetProxyByIndex(visibleIndex);
			if (proxy == null)
				return -1;
			return GetItemByProxy(proxy);
		}
		protected override int FindListSourceIndexByValue(object value) {
			return View.FindIndexByValue(CreateCriteriaForValueColumn(DataAccessor), value);
		}
		public override bool ProcessChangeFilter(string filterCriteria) {
			this.filterCriteria = filterCriteria;
			InitializeSource();
			return true;
		}
		public override bool ProcessInconsistencyDetected() {
			InitializeSource();
			return base.ProcessInconsistencyDetected();
		}
		public override bool ProcessRefresh() {
			Wrapper.Refresh();
			View.Reset();
			ItemsCache.Reset();
			return true;
		}
		void IListServerDataView.NotifyInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			RaiseInconsistencyDetected();
		}
		void IListServerDataView.NotifyExceptionThrown(ServerModeExceptionThrownEventArgs e) {
		}
		void IListServerDataView.Reset() {
			ItemsCache.Reset();
			View.Reset();
		}
	}
}
