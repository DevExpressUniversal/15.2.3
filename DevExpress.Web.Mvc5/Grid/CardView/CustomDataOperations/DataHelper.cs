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
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.Web.Mvc.Internal {
	public class CardViewCustomOperationHelper : GridBaseCustomOperationHelper {
		static IGridSummaryItemStateCollection defaultGroupSummary = new CardViewSummaryItemStateCollection();
		public CardViewCustomOperationHelper(CardViewModel viewModel)
			: base(viewModel) {
		}
		public CardViewModel ViewModel { get { return (CardViewModel)base.GridViewModel; } set { base.GridViewModel = value; } }
		public override IGridSummaryItemStateCollection GroupSummary { get { return defaultGroupSummary; } }
		public CardViewCustomBindingGetDataCardCountHandler GetDataCardCountMethod { get; set; }
		public CardViewCustomBindingGetDataHandler GetDataMethod { get; set; }
		public CardViewCustomBindingGetSummaryValuesHandler GetSummaryValuesMethod { get; set; }
		public CardViewCustomBindingGetUniqueHeaderFilterValuesHandler GetUniqueHeaderFilterValuesMethod { get; set; }
		public void ProcessCustomBinding(
				CardViewCustomBindingGetDataCardCountHandler getDataCardCountMethod,
				CardViewCustomBindingGetDataHandler getDataMethod,
				CardViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				CardViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod) {
			AssignDataHandlers(getDataCardCountMethod, getDataMethod, getSummaryValuesMethod, getUniqueHeaderFilterValuesMethod);
			ProcessCustomBindingCore();
		}
		void AssignDataHandlers(
				CardViewCustomBindingGetDataCardCountHandler getDataCardCountMethod,
				CardViewCustomBindingGetDataHandler getDataMethod,
				CardViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				CardViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod) {
			if(getDataCardCountMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getDataCardCountMethod parameter.");
			if(getDataMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getDataMethod parameter.");
			if(GridViewCustomOperationCallbackHelper.IsHeaderFilterPopupAction && getUniqueHeaderFilterValuesMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getUniqueHeaderFilterValuesMethod parameter.");
			if(ViewModel.TotalSummary.Count > 0 && getSummaryValuesMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getSummaryValuesMethod parameter.");
			GetDataCardCountMethod = getDataCardCountMethod;
			GetDataMethod = getDataMethod;
			GetSummaryValuesMethod = getSummaryValuesMethod;
			GetUniqueHeaderFilterValuesMethod = getUniqueHeaderFilterValuesMethod;
		}
		protected override IEnumerable GetHeaderFilterValues(string fieldName, string filterExpression) {
			var args = new CardViewCustomBindingGetUniqueHeaderFilterValuesArgs(ViewModel, fieldName, filterExpression);
			GetUniqueHeaderFilterValuesMethod(args);
			return args.Data;
		}
		protected override IEnumerable<GridViewGroupInfo> GroupingInfo(GridViewGroupNode node) {
			throw new NotImplementedException();
		}
		protected override int GetDataRowCount(bool ignoreFilter) {
			var args = ignoreFilter ? new CardViewCustomBindingGetDataCardCountArgs(ViewModel, string.Empty) : new CardViewCustomBindingGetDataCardCountArgs(ViewModel);
			GetDataCardCountMethod(args);
			return args.DataCardCount;
		}
		protected override IEnumerable GetSummaryValue(GridViewGroupNode node, IGridSummaryItemStateCollection items) {
			var args = new CardViewCustomBindingGetSummaryValuesArgs(ViewModel, items.OfType<CardViewSummaryItemState>().ToList());
			GetSummaryValuesMethod(args);
			return args.Data;
		}
		protected override IEnumerable GetData(GridViewGroupNode node) {
			var pageSize = GridViewModel.Pager.PageIndex < 0 ? FilteredTotalRowCount : GridViewModel.Pager.PageSize;
			var args = new CardViewCustomBindingGetDataArgs(ViewModel, StartVisibleIndex, pageSize);
			if(args.DataCardCount <= 0) return null;
			node.StartDataRowIndex = args.StartDataCardIndex;
			GetDataMethod(args);
			return args.Data;
		}
	}
}
