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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Data;
using System.Collections.Generic;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data;
namespace DevExpress.Xpf.Grid {
	public interface IColumnOwnerBase {
		DataTemplate GetActualCellTemplate();
		DataTemplateSelector CellTemplateSelector { get; }
		bool AllowColumnsResizing { get; }
		void RebuildVisibleColumns();
		void RebuildColumnChooserColumns();
		void CalcColumnsLayout();
		IList<SummaryItemBase> GetTotalSummaryItems(ColumnBase column);
		object GetTotalSummaryValue(SummaryItemBase item);
		void UpdateContentLayout();
		HorizontalAlignment GetDefaultColumnAlignment(ColumnBase column);
		BaseEditSettings CreateDefaultEditSettings(IDataColumnInfo column);
		void UpdateCellDataValues();
		bool AllowSortColumn(ColumnBase column);
		ColumnBase GetColumn(string fieldName);
		bool AllowSorting { get; }
		bool AllowColumnMoving { get; }
		bool AllowResizing { get; }
		bool UpdateAllowResizingOnWidthChanging { get; }
		bool AllowEditing { get; }
		bool AutoWidth { get; }
		bool AllowColumnFiltering { get; }
		bool ShowAllTableValuesInCheckedFilterPopup { get; }
		bool ShowAllTableValuesInFilterPopup { get; }
		IList<ColumnBase> VisibleColumns { get; }
		Style GetActualCellStyle(ColumnBase column);
		Style AutoFilterRowCellStyle { get; }
		Style NewItemRowCellStyle { get; }
		Style ColumnHeaderContentStyle { get; }
		Style TotalSummaryContentStyle { get; }
		void ApplyColumnVisibleIndex(BaseColumn column);
		void UpdateShowEditFilterButton(DefaultBoolean newAllowColumnFiltering, DefaultBoolean oldAllowColumnFiltering);
		void ChangeColumnSortOrder(ColumnBase column);
		void ClearColumnFilter(ColumnBase column);
		bool CanClearColumnFilter(ColumnBase column);
		DataTemplate ColumnHeaderTemplate { get; }
		DataTemplateSelector ColumnHeaderTemplateSelector { get; }
		DataTemplate ColumnHeaderCustomizationAreaTemplate { get; }
		DataTemplateSelector ColumnHeaderCustomizationAreaTemplateSelector { get; }
		Type GetColumnType(ColumnBase column, DataProviderBase dataProvider);
		bool LockEditorClose { get; set; }
		bool ShowValidationAttributeErrors { get; }
		ActualTemplateSelectorWrapper ActualGroupValueTemplateSelector { get; }
		bool AllowGrouping { get; }
		void GroupColumn(string fieldName, int index, ColumnSortOrder sortOrder);
		void UngroupColumn(string fieldName);
		void ClearBindingValues(ColumnBase column);
	}
}
