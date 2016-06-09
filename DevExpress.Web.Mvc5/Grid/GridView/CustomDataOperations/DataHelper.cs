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
namespace DevExpress.Web.Mvc.Internal {
	public class GridViewCustomOperationHelper : GridBaseCustomOperationHelper {
		public GridViewCustomOperationHelper(GridViewModel viewModel)
			: base(viewModel) { 
		}
		public new GridViewModel GridViewModel { get { return (GridViewModel)base.GridViewModel; } set { base.GridViewModel = value; } }
		public override IGridSummaryItemStateCollection GroupSummary { get { return GridViewModel.GroupSummary; } }
		public GridViewCustomBindingGetDataRowCountHandler GetDataRowCountMethod { get; protected set; }
		public GridViewCustomBindingGetDataHandler GetDataMethod { get; protected set; }
		public GridViewCustomBindingGetGroupingInfoHandler GetGroupingInfoMethod { get; protected set; }
		public GridViewCustomBindingGetUniqueHeaderFilterValuesHandler GetUniqueHeaderFilterValuesMethod { get; protected set; }
		public GridViewCustomBindingGetSummaryValuesHandler GetSummaryValuesMethod { get; protected set; }
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				GridViewCustomBindingGetGroupingInfoHandler getGroupingInfoMethod,
				GridViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod) {
			AssignDataHandlers(getDataRowCountMethod, getDataMethod, getSummaryValuesMethod, getGroupingInfoMethod, getUniqueHeaderFilterValuesMethod);
			ProcessCustomBindingCore();
		}
		void AssignDataHandlers(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				GridViewCustomBindingGetGroupingInfoHandler getGroupingInfoMethod,
				GridViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod) {
			if(getDataRowCountMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getDataRowCountMethod parameter.");
			if(getDataMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getDataMethod parameter.");
			if(GridViewModel.GroupedColumns.Count > 0 && getGroupingInfoMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getGroupingInfoMethod parameter.");
			if(GridViewCustomOperationCallbackHelper.IsHeaderFilterPopupAction && getUniqueHeaderFilterValuesMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getUniqueHeaderFilterValuesMethod parameter.");
			if((GridViewModel.GroupSummary.Count > 0 || GridViewModel.TotalSummary.Count > 0) && getSummaryValuesMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getSummaryValuesMethod parameter.");
			GetDataRowCountMethod = getDataRowCountMethod;
			GetDataMethod = getDataMethod;
			GetGroupingInfoMethod = getGroupingInfoMethod;
			GetUniqueHeaderFilterValuesMethod = getUniqueHeaderFilterValuesMethod;
			GetSummaryValuesMethod = getSummaryValuesMethod;
		}
		protected override IEnumerable GetHeaderFilterValues(string fieldName, string filterExpression) {
			var args = new GridViewCustomBindingGetUniqueHeaderFilterValuesArgs(GridViewModel, fieldName, filterExpression);
			GetUniqueHeaderFilterValuesMethod(args);
			return args.Data;
		}
		protected override int GetDataRowCount(bool ignoreFilter) {
			var args = ignoreFilter ? new GridViewCustomBindingGetDataRowCountArgs(GridViewModel, string.Empty) : new GridViewCustomBindingGetDataRowCountArgs(GridViewModel);
			GetDataRowCountMethod(args);
			return args.DataRowCount;
		}
		protected override IEnumerable<GridViewGroupInfo> GroupingInfo(GridViewGroupNode node) {
			var column = GroupedColumns[node.Level];
			var args = new GridViewCustomBindingGetGroupingInfoArgs(GridViewModel, GetGroupInfoList(node), column.FieldName, column.SortOrder);
			GetGroupingInfoMethod(args);
			return args.Data;
		}
		protected override IEnumerable GetSummaryValue(GridViewGroupNode node, IGridSummaryItemStateCollection items) {
			var args = new GridViewCustomBindingGetSummaryValuesArgs(GridViewModel, GetGroupInfoList(node), items.OfType<GridViewSummaryItemState>().ToList());
			GetSummaryValuesMethod(args);
			return args.Data;
		}
		protected override IEnumerable GetData(GridViewGroupNode node) {
			var args = CreateLoadDataArgs(node);
			if(args.DataRowCount <= 0)
				return null;
			node.StartDataRowIndex = args.StartDataRowIndex;
			GetDataMethod(args);
			return args.Data;
		}
		GridViewCustomBindingGetDataArgs CreateLoadDataArgs(GridViewGroupNode node) {
			var infoList = GetGroupInfoList(node);
			if(node.IsRoot) {
				var pageSize = GridViewModel.Pager.PageIndex < 0 ? FilteredTotalRowCount : GridViewModel.Pager.PageSize;
				return new GridViewCustomBindingGetDataArgs(GridViewModel, infoList, StartVisibleIndex, pageSize);
			}
			var startDataVisibleIndex = node.VisibleIndex + 1;
			var endDataVisibleIndex = startDataVisibleIndex + node.TotalDataRowCount - 1;
			if(StartVisibleIndex > endDataVisibleIndex || EndVisibleIndex < startDataVisibleIndex)
				return new GridViewCustomBindingGetDataArgs(GridViewModel, infoList, 0, 0);
			var viewModelStartVisibleIndex = GridViewModel.Pager.PageIndex * GridViewModel.Pager.PageSize;
			var viewModelEndVisibleIndex = viewModelStartVisibleIndex + GridViewModel.Pager.PageSize - 1;
			var startRequiredPartVisibleIndex = Math.Max(viewModelStartVisibleIndex, startDataVisibleIndex);
			var endRequiredPartVisibleIndex = Math.Min(viewModelEndVisibleIndex, endDataVisibleIndex);
			var start = startRequiredPartVisibleIndex - startDataVisibleIndex;
			var count = endRequiredPartVisibleIndex - startRequiredPartVisibleIndex + 1;
			return new GridViewCustomBindingGetDataArgs(GridViewModel, infoList, start, count);
		}
	}
}
