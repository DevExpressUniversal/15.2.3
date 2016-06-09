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

using DevExpress.Web.Rendering;
using DevExpress.XtraExport.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Summary;
using DevExpress.Web.Data;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using System.Collections;
using System.Drawing;
using DevExpress.Export.Xl;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public class GridViewXlsExportTextBuilder : GridXlsExportTextBuilder {
		public GridViewXlsExportTextBuilder(ASPxGridBase grid, GridExcelDataPrinter printer)
			: base(grid, printer) {
			GridViewTextBuilder = new GridViewTextBuilder(Grid);
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		protected GridViewTextBuilder GridViewTextBuilder { get; private set; }
		public string GetGroupRowText(int rowHandle) {
			var visibleIndex = Printer.GetVisibleIndex(rowHandle);
			var column = Grid.SortedColumns[DataProxy.GetRowLevel(visibleIndex)] as GridViewDataColumn;
			return GridViewTextBuilder.GetGroupRowText(column, visibleIndex);
		}
	}
	public class GridViewExcelDataPrinter : GridExcelDataPrinter {
		public GridViewExcelDataPrinter(ASPxGridViewExporter exporter)
			: base(exporter) {
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public new GridViewXlsExportTextBuilder TextBuilder { get { return base.TextBuilder as GridViewXlsExportTextBuilder; } }
		protected override bool HasGrouping { get { return !IsForceDataSourcePaging && Grid.GroupCount > 0; } }
		protected override bool ShowFooter { get { return Grid.Settings.ShowFooter; } }
		protected override bool ShowGroupFooter { get { return Grid.Settings.ShowGroupFooter != GridViewGroupFooterMode.Hidden; } }
		protected override bool ShowGroupedColumns { get { return Grid.Settings.ShowGroupedColumns; } }
		protected override GridXlsExportTextBuilder CreateTextBuilder() { return new GridViewXlsExportTextBuilder(Grid, this); }
		protected override void LoadAllRows() {
			if(HasGrouping)
				LoadGroupedControllerRows();
			else
				base.LoadAllRows();
		}
		protected virtual void LoadGroupedControllerRows() {
			var indices = new DataControllerVisibleIndexCollection(DataController);
			indices.BuildVisibleIndexes(DataController.VisibleListSourceRowCount, true, true);
			var stackCapacity = Grid.GroupCount > 0 ? Grid.GroupCount : 0;
			var groupStack = new Stack<GridXlsExportRowBase>(stackCapacity);
			var groupInfo = DataController.GroupInfo;
			Rows.Capacity = groupInfo.Count > 0 ? groupInfo.Count : indices.Count;
			var prevLevel = 0;
			foreach(int handle in indices) {
				var currentLevel = DataController.GetRowLevel(handle);
				if(prevLevel > currentLevel) groupStack.Pop();
				prevLevel = currentLevel;
				var isGroupRow = DataController.IsGroupRowHandle(handle);
				var row = isGroupRow ? CreateGroupRow(handle, currentLevel) : CreateDataRow(handle, handle, currentLevel);
				var list = groupStack.Count > 0 ? (groupStack.Peek() as GridViewXlsExportGroupRow).Children : Rows;
				list.Add(row);
				if(isGroupRow)
					groupStack.Push(row);
			}
		}
		protected virtual GridXlsExportRowBase CreateGroupRow(int handle, int level) {
			var info = DataController.GroupInfo.GetGroupRowInfoByControllerRowHandle(handle);
			return new GridViewXlsExportGroupRow(this, handle, level, DataController.GroupInfo.GetChildCount(info), info.Expanded);
		}
	}
	public class GridViewXlsExportColumn : GridXlsExportColumn {
		public GridViewXlsExportColumn(GridViewExcelDataPrinter printer, IWebGridDataColumn column, int index)
			: base(printer, column, index) {
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public new GridViewDataColumn DataColumn { get { return base.DataColumn as GridViewDataColumn; } }
		protected override int GetColumnWidth() {
			return DataColumn.ExportWidth > 0 ? DataColumn.ExportWidth : base.GetColumnWidth();
		}
		protected override bool IsFixedLeft(IWebGridDataColumn column) {
			return Grid.ColumnHelper.FixedColumns.Contains(DataColumn);
		}
	}
	public class GridViewXlsExportGroupRow : GridXlsExportRowBase, IGroupRow<GridXlsExportRowBase> {
		public GridViewXlsExportGroupRow(GridViewExcelDataPrinter printer, int handle, int level, int childCount, bool expanded)
			: base(printer, handle, -1, level) {
			Expanded = expanded;
			Children = new List<GridXlsExportRowBase>(childCount);
		}
		public new GridViewExcelDataPrinter Printer { get { return base.Printer as GridViewExcelDataPrinter; } }
		public List<GridXlsExportRowBase> Children { get; private set; }
		public bool Expanded { get; private set; }
		protected override bool IsGroupRow { get { return true; } }
		IEnumerable<GridXlsExportRowBase> IGroupRow<GridXlsExportRowBase>.GetAllRows() { return Children; }
		bool IGroupRow<GridXlsExportRowBase>.IsCollapsed { get { return !Expanded; } }
		string IGroupRow<GridXlsExportRowBase>.GetGroupRowHeader() { return Printer.TextBuilder.GetGroupRowText(Handle); }
	}
	public class GridViewXlsExportSummaryItem : GridXlsExportSummaryItem {
		public GridViewXlsExportSummaryItem(ASPxGridView grid, ASPxSummaryItem item)
			: base(grid, item) {
		}
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public new ASPxSummaryItem Item { get { return base.Item as ASPxSummaryItem; } }
		protected override string ShowInColumnFooterName {
			get {
				if(!string.IsNullOrEmpty(Item.ShowInColumn)) {
					var col = Grid.DataColumns[Item.ShowInColumn];
					if(col != null)
						return col.FieldName;
				}
				return base.ShowInColumnFooterName;
			}
		}
	}
}
