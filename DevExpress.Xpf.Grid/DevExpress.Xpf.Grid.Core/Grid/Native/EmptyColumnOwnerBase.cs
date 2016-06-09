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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Data;
using System.Collections.Generic;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using System;
using DevExpress.Data;
namespace DevExpress.Xpf.Grid.Native {
	public class EmptyColumnOwnerBase : IColumnOwnerBase {
		public static readonly IColumnOwnerBase Instance = new EmptyColumnOwnerBase();
		static readonly SummaryItemBase[] EmptySummaryItemArray = new SummaryItemBase[0];
		bool IColumnOwnerBase.AllowColumnsResizing { get { return false; } }
		DataTemplate IColumnOwnerBase.GetActualCellTemplate() { return null; }
		DataTemplateSelector IColumnOwnerBase.CellTemplateSelector { get { return null; } }
		void IColumnOwnerBase.RebuildVisibleColumns() { }
		void IColumnOwnerBase.RebuildColumnChooserColumns() { }
		void IColumnOwnerBase.CalcColumnsLayout() { }
		IList<SummaryItemBase> IColumnOwnerBase.GetTotalSummaryItems(ColumnBase column) { return EmptySummaryItemArray; }
		object IColumnOwnerBase.GetTotalSummaryValue(SummaryItemBase item) { return null; }
		void IColumnOwnerBase.UpdateContentLayout() { }
		HorizontalAlignment IColumnOwnerBase.GetDefaultColumnAlignment(ColumnBase column) { return HorizontalAlignment.Left; }
		BaseEditSettings IColumnOwnerBase.CreateDefaultEditSettings(IDataColumnInfo column) { return new TextEditSettings(); }
		bool IColumnOwnerBase.AllowSortColumn(ColumnBase column) { return true; }
		void IColumnOwnerBase.UpdateCellDataValues() { }
		ColumnBase IColumnOwnerBase.GetColumn(string fieldName) { return null; }
		bool IColumnOwnerBase.AllowSorting { get { return false; } }
		bool IColumnOwnerBase.AllowColumnMoving { get { return false; } }
		bool IColumnOwnerBase.AllowResizing { get { return false; } }
		bool IColumnOwnerBase.UpdateAllowResizingOnWidthChanging { get { return true; } }
		bool IColumnOwnerBase.AllowEditing { get { return false; } }
		bool IColumnOwnerBase.AllowColumnFiltering { get { return false; } }
		bool IColumnOwnerBase.AutoWidth { get { return false; } }
		IList<ColumnBase> IColumnOwnerBase.VisibleColumns { get { return null; } }
		Style IColumnOwnerBase.GetActualCellStyle(ColumnBase column) { return null; }
		Style IColumnOwnerBase.AutoFilterRowCellStyle { get { return null; } }
		Style IColumnOwnerBase.NewItemRowCellStyle { get { return null; } }
		Style IColumnOwnerBase.ColumnHeaderContentStyle { get { return null; } }
		Style IColumnOwnerBase.TotalSummaryContentStyle { get { return null; } }
		void IColumnOwnerBase.ApplyColumnVisibleIndex(BaseColumn column) { }
		void IColumnOwnerBase.UpdateShowEditFilterButton(DefaultBoolean newAllowColumnFiltering, DefaultBoolean oldAllowColumnFiltering) { }
		void IColumnOwnerBase.ChangeColumnSortOrder(ColumnBase column) { }
		void IColumnOwnerBase.ClearColumnFilter(ColumnBase column) { }
		bool IColumnOwnerBase.CanClearColumnFilter(ColumnBase column) { return false; }
		DataTemplate IColumnOwnerBase.ColumnHeaderTemplate { get { return null; } }
		DataTemplateSelector IColumnOwnerBase.ColumnHeaderTemplateSelector { get { return null; } }
		DataTemplate IColumnOwnerBase.ColumnHeaderCustomizationAreaTemplate { get { return null; } }
		DataTemplateSelector IColumnOwnerBase.ColumnHeaderCustomizationAreaTemplateSelector { get { return null; } }
		Type IColumnOwnerBase.GetColumnType(ColumnBase column, DataProviderBase dataProvider) { return null; }
		bool IColumnOwnerBase.LockEditorClose { get; set; }
		bool IColumnOwnerBase.ShowValidationAttributeErrors { get { return false; } }
		bool IColumnOwnerBase.AllowGrouping { get { return false; } }
		ActualTemplateSelectorWrapper IColumnOwnerBase.ActualGroupValueTemplateSelector { get { return null; } }
		void IColumnOwnerBase.GroupColumn(string fieldName, int index, DevExpress.Data.ColumnSortOrder sortOrder) { }
		void IColumnOwnerBase.UngroupColumn(string fieldName) { }
		void IColumnOwnerBase.ClearBindingValues(ColumnBase column) { }
		bool IColumnOwnerBase.ShowAllTableValuesInFilterPopup { get { return false; } }
		bool IColumnOwnerBase.ShowAllTableValuesInCheckedFilterPopup { get { return true; } }
	}
}
