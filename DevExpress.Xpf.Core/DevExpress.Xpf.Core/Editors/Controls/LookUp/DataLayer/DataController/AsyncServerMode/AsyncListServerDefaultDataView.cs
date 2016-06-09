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
using DevExpress.Data.Async;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncListServerDefaultDataView : ListServerDataViewBase, IAsyncListServerDataView {
		public bool IsBusy { get; private set; }
		new AsyncDataProxyViewCache View { get { return (AsyncDataProxyViewCache)base.View; } }
		public AsyncListWrapper Wrapper {
			get { return ListSource as AsyncListWrapper; }
		}
		new IBindingList ListSource {
			get { return base.ListSource as IBindingList; }
		}
		readonly string filterCriteria;
		readonly IEnumerable<SortingInfo> actualSorting;
		readonly int groupCount;
		public AsyncListServerDefaultDataView(IAsyncListServer server, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, string filter)
			: base(new AsyncListWrapper(server), valueMember, displayMember) {
			this.filterCriteria = filter;
			List<SortingInfo> resultSorting = groups != null ? groups.Select(x => new SortingInfo(x.FieldName, ListSortDirection.Ascending)).ToList() : new List<SortingInfo>();
			resultSorting.AddRange(sorts);
			actualSorting = resultSorting;
			groupCount = groups.Count();
			Wrapper.Initialize(this);
		}
		public override CurrentDataView CreateCurrentDataView(object handle, IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			return new AsyncServerModeCurrentPlainDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember, groups, sorts, filterCriteria, displayFilterCriteria);
		}
		protected internal override bool GetIsAsyncServerMode() {
			return true;
		}
		protected internal override bool GetIsOwnSearchProcessing() {
			return true;
		}
		protected override void InitializeView(object source) {
			SetView(new AsyncDataProxyViewCache(this));
			InitializeSource();
		}
		protected virtual void InitializeSource() {
			Wrapper.ApplySortGroupFilter(CriteriaOperator.Parse(filterCriteria), actualSorting.Select(x => new ServerModeOrderDescriptor(new OperandProperty(x.FieldName), x.OrderBy == ListSortDirection.Descending)).ToList(), groupCount);
			Wrapper.Reset();
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			Wrapper.Dispose();
		}
		protected override object DataControllerAdapterGetRowInternal(int listSourceIndex) {
			return GetRow(listSourceIndex);
		}
		object GetRow(int listSourceIndex) {
			var completer = new GetRowCompleter(this);
			return Wrapper.GetRow(listSourceIndex, completer.Completed);
		}
		void IAsyncListServerDataView.NotifyLoaded(int listSourceIndex) {
			ProcessChangeItem(listSourceIndex);
			object value = GetValueFromProxy(this[listSourceIndex]);
			RaiseRowLoaded(new ItemsProviderRowLoadedEventArgs(null, listSourceIndex, value));
		}
		void IAsyncListServerDataView.NotifyApply() {
			ItemsCache.Reset();
			RaiseRefreshNeeded();
		}
		void IAsyncListServerDataView.NotifyFindIncrementalCompleted(string text, int startIndex, bool searchNext, bool ignoreStartIndex, int controllerIndex) {
			NotifyFindIncrementalInternal(text, startIndex, searchNext, ignoreStartIndex, controllerIndex);
		}
		protected virtual void NotifyFindIncrementalInternal(string text, int startIndex, bool searchNext, bool ignoreStartIndex, int controllerIndex) {
			if (!Wrapper.IsRowLoaded(controllerIndex)) {
				var completedHandlerContainer = new GetRowOnFindIncrementalCompleter(this, text, startIndex, searchNext, ignoreStartIndex);
				Wrapper.GetRow(controllerIndex, completedHandlerContainer.Completed);
				return;
			}
			var proxy = View[controllerIndex];
			object value = GetValueFromProxy(proxy);
			RaiseFindIncrementalCompleted(new ItemsProviderFindIncrementalCompletedEventArgs(text, startIndex, searchNext, ignoreStartIndex, value));
		}
		void IAsyncListServerDataView.NotifyCountChanged() {
		}
		void IAsyncListServerDataView.BusyChanged(bool isBusy) {
			if (IsBusy != isBusy) {
				IsBusy = isBusy;
				RaiseOnBusyChanged(new ItemsProviderOnBusyChangedEventArgs(isBusy));
			}
		}
		void IAsyncListServerDataView.NotifyExceptionThrown(ServerModeExceptionThrownEventArgs e) {
		}
		void IAsyncListServerDataView.NotifyInconsistencyDetected(ServerModeInconsistencyDetectedEventArgs e) {
			RaiseInconsistencyDetected();
		}
		public void FetchItem(object value) {
			IndexOfValue(value);
		}
		public void FetchCount() {
			View.FetchCount();
		}
		protected override int FindListSourceIndexByValue(object value) {
			return View.FindIndexByValue(CreateCriteriaForValueColumn(DataAccessor), value);
		}
		public override void ProcessFindIncremental(ItemsProviderFindIncrementalCompletedEventArgs e) {
			base.ProcessFindIncremental(e);
			int index = IndexOfValue(e.Value);
			if (index < 0) {
				var completer = new FindRowIndexByValueOnFindIncrementalCompleter(this, e.Text, e.StartIndex, e.SearchNext, e.IgnoreStartIndex);
				View.FindIndexByValue(CreateCriteriaForValueColumn(DataAccessor), e.Value, completer.Completed);
				return;
			}
			RaiseFindIncrementalCompleted(e);
		}
	}
}
