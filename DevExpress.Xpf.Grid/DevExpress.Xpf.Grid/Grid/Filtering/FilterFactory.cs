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
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
namespace DevExpress.Xpf.Grid.Filtering {
	public class FilterFactory {
		readonly DataControlBase grid;
		readonly string fieldName;
		readonly OperandProperty property;
		readonly FilterNames filterNamesMap;
		public FilterFactory(DataControlBase grid, string fieldName) {
			Guard.ArgumentNotNull(grid, "grid");
			Guard.ArgumentNotNull(fieldName, "fieldName");
			this.grid = grid;
			this.fieldName = fieldName;
			this.property = new OperandProperty(fieldName);
			this.filterNamesMap = new FilterNames(grid);
		}
		public FilterData CreateNoneData() {
			string caption = GetName(FilterDateType.None);
			return new FilterData(caption, null, string.Empty, FilterDateType.None);
		}
		public FilterData CreateEmptyData() {
			return CreateFilter(FilterDateType.Empty);
		}
		public FilterData CreateSpecificDateData() {
			string caption = GetName(FilterDateType.SpecificDate);
			return new FilterData(caption, null, string.Empty, FilterDateType.SpecificDate);
		}
		public FilterData CreateFilter(FilterDateType type) {
			string caption = GetName(type);
			var criteria = GetCriteria(type);
			string tooltip = GetTooltip(type, criteria);
			return new FilterData(caption, criteria, tooltip, type);
		}
		string GetName(FilterDateType type) {
			return filterNamesMap[type];
		}
		CriteriaOperator GetCriteria(FilterDateType type) {
			if(type == FilterDateType.None) return null;
			return new[] { type }.ToCriteria(this.fieldName);
		}
		string GetTooltip(FilterDateType type, CriteriaOperator criteria) {
			if(!type.IsFilterValid()) return string.Empty;
			var expanded = criteria.ExpandDates();
			var info = this.grid.GetInfoFromCriteriaOperator(expanded);
			return info.FilterText;
		}
		class FilterNames {
			static readonly Dictionary<FilterDateType, GridControlStringId> TypeToStringIdMap;
			static readonly Dictionary<FilterDateType, string> TypeToNameMap;
			readonly DataControlBase grid;
			static FilterNames() {
				TypeToStringIdMap = new Dictionary<FilterDateType, GridControlStringId> {
					{ FilterDateType.None, GridControlStringId.DateFiltering_ShowAllFilterName },
					{ FilterDateType.SpecificDate, GridControlStringId.DateFiltering_FilterBySpecificDateFilterName },
					{ FilterDateType.PriorThisYear, GridControlStringId.DateFiltering_PriorToThisYearFilterName },
					{ FilterDateType.EarlierThisYear, GridControlStringId.DateFiltering_EarlierThisYearFilterName },
					{ FilterDateType.EarlierThisMonth, GridControlStringId.DateFiltering_EarlierThisMonthFilterName },
					{ FilterDateType.LastWeek, GridControlStringId.DateFiltering_LastWeekFilterName },
					{ FilterDateType.EarlierThisWeek, GridControlStringId.DateFiltering_EarlierThisWeekFilterName },
					{ FilterDateType.Yesterday, GridControlStringId.DateFiltering_YesterdayFilterName },
					{ FilterDateType.Today, GridControlStringId.DateFiltering_TodayFilterName },
					{ FilterDateType.Tomorrow, GridControlStringId.DateFiltering_TomorrowFilterName },
					{ FilterDateType.LaterThisWeek, GridControlStringId.DateFiltering_LaterThisWeekFilterName },
					{ FilterDateType.NextWeek, GridControlStringId.DateFiltering_NextWeekFilterName },
					{ FilterDateType.LaterThisMonth, GridControlStringId.DateFiltering_LaterThisMonthFilterName },
					{ FilterDateType.LaterThisYear, GridControlStringId.DateFiltering_LaterThisYearFilterName },
					{ FilterDateType.BeyondThisYear, GridControlStringId.DateFiltering_BeyondThisYearFilterName },
					{ FilterDateType.Earlier, GridControlStringId.DateFiltering_EarlierFilterName },
					{ FilterDateType.ThisMonth, GridControlStringId.DateFiltering_ThisMonthFilterName },
					{ FilterDateType.ThisWeek, GridControlStringId.DateFiltering_ThisWeekFilterName },
					{ FilterDateType.Beyond, GridControlStringId.DateFiltering_BeyondFilterName },
					{ FilterDateType.Empty, GridControlStringId.DateFiltering_EmptyFilterName },
				};
				TypeToNameMap = new Dictionary<FilterDateType, string> {
					{ FilterDateType.MonthAgo6, DateFiltersHelper.SixMonthsAgoFilterName },
					{ FilterDateType.MonthAgo5, DateFiltersHelper.FiveMonthsAgoFilterName },
					{ FilterDateType.MonthAgo4, DateFiltersHelper.FourMonthsAgoFilterName },
					{ FilterDateType.MonthAgo3, DateFiltersHelper.ThreeMonthsAgoFilterName },
					{ FilterDateType.MonthAgo2, DateFiltersHelper.TwoMonthsAgoFilterName },
					{ FilterDateType.MonthAgo1, DateFiltersHelper.MonthAgoFilterName },
					{ FilterDateType.MonthAfter1, DateFiltersHelper.MonthAfterFilterName },
					{ FilterDateType.MonthAfter2, DateFiltersHelper.TwoMonthsAfterFilterName },
				};
			}
			public FilterNames(DataControlBase grid) {
				Guard.ArgumentNotNull(grid, "grid");
				this.grid = grid;
			}
			DataViewBase View {
				get { return this.grid.viewCore; }
			}
			public string this[FilterDateType type] {
				get { return GetName(type); }
			}
			public string GetName(FilterDateType type) {
				if(View == null) return string.Empty;
				if(TypeToNameMap.ContainsKey(type)) {
					return TypeToNameMap[type];
				}
				if(!TypeToStringIdMap.ContainsKey(type)) {
					return string.Empty;
				}
				var stringId = TypeToStringIdMap[type];
				string localized = View.GetLocalizedString(stringId);
				TypeToNameMap[type] = localized;
				return localized;
			}
		}
	}
	public static class FilterFactoryExtensions {
		public static FilterData[] CreateFilters(this FilterFactory factory, params FilterDateType[] filters) {
			return filters.Select(x => factory.CreateFilter(x)).ToArray();
		}
	}
}
