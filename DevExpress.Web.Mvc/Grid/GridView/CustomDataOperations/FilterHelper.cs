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
using System.Linq;
using System.Text;
using DevExpress.Web.Internal;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using System.Collections;
namespace DevExpress.Web.Mvc.Internal {
	public class MVCxGridViewCustomBindingFilterHelper : GridViewFilterHelper {
		ICollection searchPanelColumnInfos;
		GridSearchPanelHighlighHelper hightlightHelper;
		public MVCxGridViewCustomBindingFilterHelper(MVCxGridView grid)
			: this(grid.CustomOperationViewModel) {
		}
		public MVCxGridViewCustomBindingFilterHelper(GridViewModel viewModel)
			: base((ASPxGridView)viewModel.Grid) {
			Initialize(viewModel);
		}
		GridViewModel GridViewModel { get; set; }
		void Initialize(GridViewModel viewModel) {
			GridViewModel = viewModel;
			viewModel.SearchPanelFilterParseResult = viewModel.SearchPanelFilterParseResult ?? ParseSearchPanelFilter();
			this.hightlightHelper = new GridSearchPanelHighlighHelper(viewModel.SearchPanelFilterParseResult);
		}
		protected internal CriteriaOperator GetSearchFilterCriteria() {
			return CreateSearchFilterCriteria();
		}
		protected override bool IsServerMode { get { return false; } }
		protected override string SearchPanelFilter { get { return GridViewModel.SearchPanel.Filter; } }
		protected override ICollection SearchPanelColumnInfos {
			get {
				if(searchPanelColumnInfos == null)
					searchPanelColumnInfos = GridViewModel.GetSearchableColumns();
				return searchPanelColumnInfos;
			}
		}
		protected override GridSearchPanelHighlighHelper HightlightHelper { get { return hightlightHelper; } set { } }
		protected override GridViewSearchPanelGroupOperator SearchPanelGroupOperator { get { return GridViewModel.SearchPanel.GroupOperator; } }
		public static string GetColumnAutoFilterText(GridBaseColumnState column) {
			return GetColumnAutoFilterText(column.AutoFilterCondition, column.FieldName, CriteriaOperator.Parse(column.FilterExpression), IsDateEditColumn(column));
		}
		public static CriteriaOperator CreateAutoFilter(GridBaseColumnState column, string value) {
			var condition = GetColumnAutoFilterCondition(column.AutoFilterCondition, column.FilterMode, column.EditKind, column.DataType, false);
			return CreateAutoFilter(condition, column.FieldName, column.DataType, value, IsDateEditColumn(column));
		}
		public static CriteriaOperator CreateHeaderFilter(GridBaseColumnState column, string[] objectValues) {
			return CreateHeaderFilter(column.FieldName, column.DataType, objectValues, IsDateEditColumn(column));
		}
		static bool IsDateEditColumn(GridBaseColumnState column) {
			return Type.Equals(column.DataType, typeof(DateTime));
		}
		protected override FindSearchParserResults ParseSearchPanelFilter() {
			return ParseSearchPanelFilter(GridViewModel.SearchPanel.Filter, SearchPanelColumnInfos, GridViewModel.SearchPanel.GroupOperator, false);
		}
		protected override CriteriaOperator CreateSearchFilterCriteria() {
			if(string.IsNullOrEmpty(GridViewModel.SearchPanel.Filter))
				return null;
			return CreateCriteriaOperator(GridViewModel.SearchPanelFilterParseResult, FilterCondition.Contains, true);
		}
		protected internal static CriteriaOperator ParseSearchPanelFilter(GridBaseViewModel viewModel) {
			var parseResult = ParseSearchPanelFilter(
				viewModel.SearchPanel.Filter, viewModel.GetSearchableColumns(), viewModel.SearchPanel.GroupOperator, false);
			return CreateCriteriaOperator(parseResult, FilterCondition.Contains, true);
		}
	}
}
