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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc.Internal {
	public static class GridViewCustomOperationCallbackHelper {
		const string CallbackSeparator = ":";
		public static string GetHeaderFilterExpression(GridBaseViewModel state, out string fieldName) {
			fieldName = string.Empty;
			string commandName;
			var args = GetCallbackArguments(out commandName, true);
			if(commandName != GridViewCallbackCommand.FilterPopup)
				return string.Empty;
			if(args.Length != 3)
				return string.Empty;
			var column = GetColumn(state, int.Parse(args[1]));
			if(column == null)
				return string.Empty;
			fieldName = column.FieldName;
			var includeFilteredOut = args[2] == "T";
			if(includeFilteredOut)
				return string.Empty;
			return GetFilterByColumn(state, column, null);
		}
		public static bool IsHeaderFilterPopupAction { get { return FunctionalCallbackCommandName == GridViewCallbackCommand.FilterPopup; } }
		public static bool IsExpandRowAction { get { return CallbackCommandName == GridViewCallbackCommand.ExpandRow; } }
		public static bool IsCollapseRowAction { get { return CallbackCommandName == GridViewCallbackCommand.CollapseRow; } }
		public static bool IsExpandAllAction { get { return CallbackCommandName == GridViewCallbackCommand.ExpandAll; } }
		public static bool IsCollapseAllAction { get { return CallbackCommandName == GridViewCallbackCommand.CollapseAll; } }
		public static bool ExpandCollapseRecursive {
			get {
				if(!IsExpandRowAction && !IsCollapseRowAction)
					return false;
				bool recursive;
				GetExpandCollapseVisibleIndex(out recursive);
				return recursive;
			}
		}
		public static int ExpandCollapseRowIndex {
			get {
				if(!IsExpandRowAction && !IsCollapseRowAction)
					return -1;
				bool recursive;
				return GetExpandCollapseVisibleIndex(out recursive);
			}
		}
		static int GetExpandCollapseVisibleIndex(out bool recursive) {
			string commandName;
			var args = GetCallbackArguments(out commandName, false);
			recursive = false;
			int index;
			if(!Int32.TryParse(args[0], out index))
				return -1;
			recursive = bool.Parse(args[1]);
			return index;
		}
		static string CallbackCommandName {
			get {
				string commandName;
				GetCallbackArguments(out commandName, false);
				return commandName;
			}
		}
		internal static string FunctionalCallbackCommandName {
			get {
				string commandName;
				GetCallbackArguments(out commandName, true);
				return commandName;
			}
		}
		static string ModelStateKey {
			get {
				return MvcUtils.CallbackName.Replace(GridViewExtension.FilterControlCallbackPostfix, string.Empty) + GridViewExtension.FilterControlCallbackPostfix.Replace('_', '$');
			}
		}
		public static void ProcessModelBinding(GridBaseColumnState column, GridBaseViewModel state) {
			if(state == null || column == null)
				return;
			string commandName;
			var args = GetCallbackArguments(out commandName, false);
			var action = GetColumnCallbackMethod(commandName);
			if(action == null)
				return;
			var preparedColumn = action(state, args);
			if(preparedColumn != null)
				column.Assign(preparedColumn);
		}
		public static void ProcessModelBinding(GridBasePagerState pager, GridBaseViewModel state) {
			if(state == null || pager == null)
				return;
			string commandName;
			var args = GetCallbackArguments(out commandName, false);
			var action = GetPagerCallbackMethod(commandName);
			if(action == null)
				return;
			var preparedPager = action(state, args);
			if(preparedPager != null)
				pager.Assign(preparedPager);
		}
		public static void ProcessModelBinding(GridBaseFilteringState filteringState, Func<string, GridBaseViewModel> createViewModelMethod) {
			ProcessModelBinding(filteringState, MvcUtils.CallbackName, createViewModelMethod);
		}
		public static void ProcessModelBinding(GridBaseFilteringState filteringState, string callbackName, Func<string, GridBaseViewModel> createViewModelMethod) {
			string gridName = callbackName.Replace(GridViewExtension.FilterControlCallbackPostfix, string.Empty);
			filteringState.GridViewModel = filteringState.GridViewModel ?? createViewModelMethod(gridName);
			if(filteringState.GridViewModel == null)
				return;
			string commandName;
			var args = GetCallbackArguments(out commandName, false);
			var columnFilteringAction = GetColumnFilteringCallbackMethod(commandName);
			if(columnFilteringAction != null) {
				var modifiedColumn = columnFilteringAction(filteringState.GridViewModel, args);
				filteringState.AddModifiedColumn(modifiedColumn);
				return;
			}
			if (commandName == GridViewCallbackCommand.ApplySearchPanelFilter) {
				filteringState.SearchPanelFilter = args[0];
				return;
			}
			var totalFilteringAction = GetTotalFilteringCallbackMethod(commandName);
			if(totalFilteringAction != null)
				totalFilteringAction(filteringState, args);
		}
		internal static string[] GetCallbackArguments(out string commandName, bool isFunctionalCallback) {
			var args = new List<string>();
			commandName = string.Empty;
			if(!MvcUtils.IsCallback())
				return args.ToArray();
			var reader = new GridCallbackArgumentsReader(GetCallbackArgs());
			if(!(reader.InternalCallbackIndex == -1 ^ isFunctionalCallback))
				return args.ToArray();
			string serializedArgs = MvcUtils.CallbackName.Contains(GridViewExtension.FilterControlCallbackPostfix) ? GetCallbackArgs() : reader.CallbackArguments;
			args = CommonUtils.DeserializeStringArray(serializedArgs);
			if (args.Count() > 0) {
				commandName = args[0];
				args.RemoveAt(0);
			}
			return args.ToArray();
		}
		static string GetCallbackArgs() {
			var index = MvcUtils.CallbackArgument.IndexOf(CallbackSeparator);
			return MvcUtils.CallbackArgument.Substring(index + 1);
		}
		static Func<GridBaseViewModel, string[], GridBaseColumnState> GetColumnCallbackMethod(string command) {
			var columnFilteringCallbackMethod = GetColumnFilteringCallbackMethod(command);
			if(columnFilteringCallbackMethod != null)
				return columnFilteringCallbackMethod;
			switch(command) {
				case GridViewCallbackCommand.Sort:
					return CBSort;
				case GridViewCallbackCommand.Group:
					return CBGroup;
				case GridViewCallbackCommand.ColumnMove:
					return CBColumnMove;
			}
			return null;
		}
		static GridBaseColumnState CBApplyColumnFilter(GridBaseViewModel state, string[] args) {
			var column = GetColumn(state, int.Parse(args[0]));
			if(column == null)
				return null;
			var criteria = MVCxGridViewCustomBindingFilterHelper.CreateAutoFilter(column, args[1]);
			ApplyFilterByColumn(state, column, criteria);
			return column;
		}
		static GridBaseColumnState CBApplyHeaderColumnFilter(GridBaseViewModel state, string[] args) {
			var column = GetColumn(state, int.Parse(args[0]));
			if(column == null)
				return null;
			var values = HtmlConvertor.FromJSON<ArrayList>(args[1]);
			var criteria = MVCxGridViewCustomBindingFilterHelper.CreateHeaderFilter(column, values.OfType<string>().ToArray());
			ApplyFilterByColumn(state, column, criteria);
			return column;
		}
		static GridBaseColumnState CBFilterRowMenu(GridBaseViewModel state, string[] args) {
			var column = GetColumn(state, int.Parse(args[0]));
			if(column == null)
				return null;
			var filterValue = MVCxGridViewCustomBindingFilterHelper.GetColumnAutoFilterText(column);
			column.AutoFilterCondition = (AutoFilterCondition)Int32.Parse(args[1]);
			var criteria = MVCxGridViewCustomBindingFilterHelper.CreateAutoFilter(column, filterValue);
			ApplyFilterByColumn(state, column, criteria);
			return column;
		}
		static GridBaseColumnState CBSort(GridBaseViewModel state, string[] args) {
			var column = GetColumn(state, int.Parse(args[0]));
			if(column == null)
				return null;
			var sortIndex = args[1] == string.Empty ? -2 : Int32.Parse(args[1]);
			var sortOrder = GetSortOrder(column, args[2]);
			var reset = bool.Parse(args[3]);
			if(sortIndex == -2) {
				if(sortOrder == ColumnSortOrder.None)
					sortIndex = -1;
				else
					sortIndex = column.SortIndex < 0 ? state.SortedColumns.Count : column.SortIndex;
			}
			column.SortIndex = sortIndex;
			column.SortOrder = sortOrder;
			state.SortBy(column, reset);
			return column;
		}
		static GridBaseColumnState CBGroup(GridBaseViewModel state, string[] args) {
			var column = GetColumn(state, int.Parse(args[0]));
			if(column == null)
				return null;
			var groupIndex = args[1] == string.Empty ? -2 : Int32.Parse(args[1]);
			var order = GetSortOrder(column, args[2]);
			if(groupIndex == -2)
				groupIndex = column.GroupIndex < 0 ? state.GroupedColumnsInternal.Count : column.GroupIndex;
			column.GroupIndex = groupIndex;
			column.SortOrder = order;
			return column;
		}
		static GridBaseColumnState CBColumnMove(GridBaseViewModel state, string[] args) {
			var column = GetColumn(state, int.Parse(args[0]));
			if(column == null)
				return null;
			var moveToColumn = GetColumn(state, int.Parse(args[1]));
			bool moveBefore = bool.Parse(args[2]),
				 moveToGroup = bool.Parse(args[3]),
				 moveFromGroup = bool.Parse(args[4]);
			if(!moveToGroup && !moveFromGroup)
				return null;
			int groupIndex = -1;
			if(moveToGroup) {
				groupIndex = state.GroupedColumnsInternal.Count;
				if(moveToColumn != null && moveToColumn.GroupIndex > -1) {
					groupIndex = moveToColumn.GroupIndex;
					if(!moveBefore)
						groupIndex++;
				}
			}
			column.GroupIndex = groupIndex;
			column.SortIndex = column.GroupIndex;
			return column;
		}
		static GridBaseColumnState GetColumn(GridBaseViewModel state, int index) {
			return state.Columns.Where(c => c.Index == index).FirstOrDefault();
		}
		public static void ApplyFilterByColumn(GridBaseViewModel state, GridBaseColumnState column, CriteriaOperator criteria) {
			state.FilterExpression = GetFilterByColumn(state, column, criteria);
		}
		public static string GetFilterByColumn(GridBaseViewModel state, GridBaseColumnState column, CriteriaOperator criteria) {
			var op = new OperandProperty(column.FieldName);
			var columnCriterias = CriteriaColumnAffinityResolver.SplitByColumns(CriteriaOperator.Parse(state.FilterExpression));
			if (columnCriterias.ContainsKey(op))
				columnCriterias[op] = criteria;
			else
				columnCriterias.Add(op, criteria);
			return CriteriaOperator.ToString(GroupOperator.And(columnCriterias.Values));
		}
		public static string GetColumnFilterExpression(GridBaseViewModel state, GridBaseColumnState column) {
			var op = new OperandProperty(column.FieldName);
			var columnCriterias = CriteriaColumnAffinityResolver.SplitByColumns(CriteriaOperator.Parse(state.FilterExpression));
			if(!columnCriterias.ContainsKey(op))
				return string.Empty;
			return CriteriaOperator.ToString(columnCriterias[op]);
		}
		static ColumnSortOrder GetSortOrder(GridBaseColumnState column, string order) {
			switch(order) {
				case "NONE": return ColumnSortOrder.None;
				case "DSC": return ColumnSortOrder.Descending;
				case "ASC": return ColumnSortOrder.Ascending;
			}
			if(column.SortOrder == ColumnSortOrder.Ascending)
				return ColumnSortOrder.Descending;
			return ColumnSortOrder.Ascending;
		}
		static Func<GridBaseViewModel, string[], GridBasePagerState> GetPagerCallbackMethod(string command) {
			switch(command) {
				case GridViewCallbackCommand.NextPage:
					return CBNextPage;
				case GridViewCallbackCommand.PreviousPage:
					return CBPreviousPage;
				case GridViewCallbackCommand.GotoPage:
					return CBGotoPage;
				case GridViewCallbackCommand.PagerOnClick:
					return CBPagerOnClick;
			}
			return null;
		}
		static GridBasePagerState CBNextPage(GridBaseViewModel state, string[] args) {
			var pager = state.Pager;
			MovePageOnCallback(pager, pager.PageIndex + 1, false);
			return pager;
		}
		static GridBasePagerState CBPreviousPage(GridBaseViewModel state, string[] args) {
			var pager = state.Pager;
			MovePageOnCallback(pager, pager.PageIndex - 1, false);
			return pager;
		}
		static GridBasePagerState CBGotoPage(GridBaseViewModel state, string[] args) {
			if(args.Length < 1)
				return null;
			int pageIndex;
			if(!Int32.TryParse(args[0], out pageIndex))
				return null;
			var pager = state.Pager;
			MovePageOnCallback(pager, pageIndex, PagerIsValidPageIndex(pager, -1));
			return pager;
		}
		static GridBasePagerState CBPagerOnClick(GridBaseViewModel state, string[] args) {
			var pager = state.Pager;
			string command = args[0];
			int oldIndex = pager.PageIndex;
			int oldSize = pager.ShowAllItemSelected ? -1 : pager.PageSize;
			int newIndex = oldIndex;
			int newSize = oldSize;
			if(DevExpress.Web.ASPxPagerBase.IsChangePageSizeCommand(command))
				newSize = DevExpress.Web.ASPxPagerBase.GetNewPageSize(command, pager.PageSize);
			else
				newIndex = DevExpress.Web.ASPxPagerBase.GetNewPageIndex(command, pager.PageIndex, () => { return pager.PageCount; }, false);
			if(newSize <= 0) {
				newIndex = -1;
				newSize = oldSize;
			}
			else if(newSize != oldSize && oldIndex == -1)
				newIndex = 0;
			if(oldSize != newSize && PagerIsValidPageSize(pager, newSize))
				pager.PageSize = newSize;
			if(oldIndex != newIndex && PagerIsValidPageIndex(pager, newIndex))
				pager.PageIndex = newIndex;
			return pager;
		}
		static void MovePageOnCallback(GridBasePagerState pager, int newIndex, bool allowNegative) {
			if(newIndex >= pager.PageCount)
				newIndex = Math.Max(0, pager.PageCount - 1);
			if(newIndex < -1)
				newIndex = -1;
			if(!allowNegative && newIndex < 0)
				newIndex = 0;
			pager.PageIndex = newIndex;
		}
		static bool PagerIsValidPageIndex(GridBasePagerState pager, int pageIndex) {
			if(pageIndex != -1)
				return true;
			if(!pager.Visible)
				return false;
			return pager.ShowPageSizeItem && pager.ShowAllItem;
		}
		static bool PagerIsValidPageSize(GridBasePagerState pager, int pageSize) {
			if(pageSize == -1)
				return PagerIsValidPageIndex(pager, -1);
			if(!pager.ShowPageSizeItem)
				return false;
			return pageSize == pager.PageSize || pager.PageSizeItems.Contains(pageSize);
		}
		static Func<GridBaseViewModel, string[], GridBaseColumnState> GetColumnFilteringCallbackMethod(string command) {
			switch(command) {
				case GridViewCallbackCommand.ApplyColumnFilter:
					return CBApplyColumnFilter;
				case GridViewCallbackCommand.ApplyHeaderColumnFilter:
					return CBApplyHeaderColumnFilter;
				case GridViewCallbackCommand.FilterRowMenu:
					return CBFilterRowMenu;
			}
			return null;
		}
		static Action<GridBaseFilteringState, string[]> GetTotalFilteringCallbackMethod(string command) {
			switch(command) {
				case FilterControlCallbackCommand.Apply:
					return CBApplyFilterBuilderFilter;
				case GridViewCallbackCommand.ApplyFilter:
					return CBApplyFilter;
				case GridViewCallbackCommand.SetFilterEnabled:
					return CBSetFilterEnabled;
				case GridViewCallbackCommand.ApplyMultiColumnFilter:
					return CBApplyMultiColumnFilter;
			}
			return null;
		}
		static void CBApplyFilterBuilderFilter(GridBaseFilteringState filteringState, string[] args) {
			filteringState.FilterExpression = MVCxPopupFilterControl.GetFilterState(ModelStateKey);
		}
		static void CBApplyFilter(GridBaseFilteringState filteringState, string[] args) {
			filteringState.FilterExpression = string.Join("|", args);
		}
		static void CBSetFilterEnabled(GridBaseFilteringState filteringState, string[] args) {
			if(args.Length < 1)
				return;
			bool isFilterEnabled;
			if(bool.TryParse(args[0], out isFilterEnabled))
				filteringState.IsFilterApplied = isFilterEnabled;
		}
		static void CBApplyMultiColumnFilter(GridBaseFilteringState filteringState, string[] args) {
			for(int i = 0; i < args.Length; i += 3) {
				int columnIndex;
				if(!int.TryParse(args[i], out columnIndex)) continue;
				GridViewColumnState column = (GridViewColumnState)filteringState.GridViewModel.Columns.Where(c => c.Index == columnIndex).FirstOrDefault();
				if(column != null) {
					if(!string.IsNullOrEmpty(args[i + 2]))
						column.AutoFilterCondition = (AutoFilterCondition)Int32.Parse(args[i + 2]);
					CriteriaOperator columnCriteria = MVCxGridViewCustomBindingFilterHelper.CreateAutoFilter(column, args[i + 1]);
					column.FilterExpression = !CriteriaOperator.Equals(columnCriteria, null) ? columnCriteria.ToString() : string.Empty;
					filteringState.AddModifiedColumn(column);
				}
			}
		}
		internal static ExpandCollapseInfo GetExpandCollapseInfo() {
			if(!IsExpandRowAction && !IsExpandAllAction && !IsCollapseRowAction && !IsCollapseAllAction)
				return null;
			var info = new ExpandCollapseInfo();
			info.ExpandRow = IsExpandRowAction;
			info.ExpandAll = IsExpandAllAction;
			info.CollapseRow = IsCollapseRowAction;
			info.CollapseAll = IsCollapseAllAction;
			info.RowIndex = ExpandCollapseRowIndex;
			info.Recursive = ExpandCollapseRecursive;
			return info;
		}
	}
	public class ExpandCollapseInfo {
		public bool Expanded { get { return ExpandRow || ExpandAll; } }
		public bool ExpandRow { get; set; }
		public bool ExpandAll { get; set; }
		public bool CollapseRow { get; set; }
		public bool CollapseAll { get; set; }
		public int RowIndex { get; set; }
		public bool Recursive { get; set; }
	}
}
