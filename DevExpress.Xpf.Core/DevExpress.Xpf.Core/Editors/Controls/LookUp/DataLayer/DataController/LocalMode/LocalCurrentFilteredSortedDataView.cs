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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Editors.Helpers {
	public class LocalCurrentFilteredSortedDataView : LocalCurrentDataView {
		string filterCriteria;
		string displayFilterCriteria;
		IList<SortingInfo> actualSorts;
		new DefaultDataView ListSource { get { return base.ListSource as DefaultDataView; } }
		new LocalDataProxyViewCache View { get { return base.View as LocalDataProxyViewCache; } }
		CriteriaCompiledContextDescriptorTyped cachedDescriptor;
		Func<object, bool> cachedFilterPredicate;
		Func<object, object>[] cachedSortHandlers;
		public LocalCurrentFilteredSortedDataView(object view, object handle, string valueMember, string displayMember,
			IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria)
			: base(view, handle, valueMember, displayMember) {
			this.filterCriteria = filterCriteria;
			this.displayFilterCriteria = displayFilterCriteria;
			List<SortingInfo> resultSorting = groups != null
				? groups.Select(x => new SortingInfo(x.FieldName, ListSortDirection.Ascending)).ToList()
				: new List<SortingInfo>();
			resultSorting.AddRange(sorts);
			actualSorts = resultSorting;
		}
		protected override void FetchDescriptorsInternal(DataAccessor accessor) {
			base.FetchDescriptorsInternal(accessor);
			foreach (var sort in actualSorts)
				accessor.Fetch(sort.FieldName);
			var filter = CriteriaOperator.Parse(this.filterCriteria);
			if (!ReferenceEquals(filter, null)) {
				var visitor = new Visitor();
				filter.Accept(visitor);
				foreach (var property in visitor.RequestedProperties)
					accessor.Fetch(property);
			}
			var displayFilter = CriteriaOperator.Parse(displayFilterCriteria);
			if (!ReferenceEquals(displayFilter, null)) {
				var visitor = new Visitor();
				displayFilter.Accept(visitor);
				foreach (var property in visitor.RequestedProperties)
					accessor.Fetch(property);
			}
		}
		protected override void InitializeView(object source) {
			var view = new DefaultDataViewAsListWrapper((DefaultDataView)source);
			SetView(new LocalDataProxyViewCache(DataAccessor, view.Cast<object>().Select(x => DataAccessor.CreateProxy(x, -1))));
			cachedDescriptor = new CriteriaCompiledContextDescriptorTyped(DataAccessor.ElementType);
			CriteriaOperator filterCriteria = CriteriaOperator.And(new[] { CriteriaOperator.Parse(this.filterCriteria), CriteriaOperator.Parse(displayFilterCriteria)}) ;
			cachedFilterPredicate = CriteriaCompiler.ToUntypedPredicate(filterCriteria, cachedDescriptor);
			if (actualSorts.Count > 0 && View.Any())
				cachedSortHandlers =
					actualSorts.Select(x => ReflectionHelper.CreateInstanceMethodHandler<DataProxy, Func<object, object>>(
						View.First(), "get_" + x.FieldName, BindingFlags.Public | BindingFlags.Instance)).ToArray();
			else
				cachedSortHandlers = null;
			View.ApplySortGroupFilter(PerformSortGroupFilter);
			ItemsCache.Reset();
			ResetDisplayTextCache();
		}
		ChunkList2<DataProxy> PerformSortGroupFilter(IList<DataProxy> view) {
			IEnumerable<DataProxy> result = view.ToList();
			if (cachedFilterPredicate != null)
				result = result.Where(x => cachedFilterPredicate(x));
			if (actualSorts.Count > 0) {
				var first = view.FirstOrDefault();
				if (first == null)
					return new ChunkList2<DataProxy>();
				var sorting = actualSorts.First();
				var list = sorting.OrderBy == ListSortDirection.Ascending
					? result.OrderBy(x => cachedSortHandlers[0](x))
					: result.OrderByDescending(x => cachedSortHandlers[0](x));
				for (int i = 1; i < actualSorts.Count; i++) {
					int index = i;
					var thenSorting = actualSorts[i];
					if (thenSorting.OrderBy == ListSortDirection.Ascending) {
						list = list.ThenBy(x => cachedSortHandlers[index](x));
					}
					else
						list = list.ThenByDescending(x => cachedSortHandlers[index](x));
				}
				result = list;
			}
			return new ChunkList2<DataProxy>(result, "Tag");
		}
		class SearchInFilteredSortedListComparer : IComparer<object> {
			readonly DefaultDataView defaultView;
			readonly CurrentDataView currentView;
			public SearchInFilteredSortedListComparer(DefaultDataView defaultView, CurrentDataView currentView) {
				this.defaultView = defaultView;
				this.currentView = currentView;
			}
			public int Compare(object x, object y) {
				object left = currentView.GetValueFromProxy((DataProxy)x);
				int leftIndex = defaultView.IndexOfValue(left);
				return leftIndex.CompareTo(((DataProxy)y).f_visibleIndex);
			}
		}
		class SearchInSortedListComparer : IComparer<object> {
			readonly Func<object, object> sortHandler;
			public SearchInSortedListComparer(Func<object, object> sortHandler) {
				this.sortHandler = sortHandler;
			}
			public int Compare(object x, object y) {
				object left = sortHandler(x);
				object right = sortHandler(y);
				IComparable leftComparable = left as IComparable;
				if (leftComparable != null)
					return leftComparable.CompareTo(right);
				IComparable rightComparable = right as IComparable;
				if (rightComparable != null)
					return -1 * rightComparable.CompareTo(left);
				return 0;
			}
		}
		public override bool ProcessAddItem(int index) {
			InitializeView(ListSource);
			return true;
		}
		public override bool ProcessChangeItem(int index) {
			InitializeView(ListSource);
			return true;
		}
		public override bool ProcessDeleteItem(int index) {
			InitializeView(ListSource);
			return true;
		}
		public override bool ProcessMoveItem(int oldIndex, int newIndex) {
			InitializeView(ListSource);
			return true;
		}
		public override bool ProcessReset() {
			InitializeView(ListSource);
			return true;
		}
		protected override ListChangedEventArgs ConvertListChangedEventArgs(ListChangedEventArgs e) {
			return new ListChangedEventArgs(ListChangedType.Reset, -1);
		}
	}
}
