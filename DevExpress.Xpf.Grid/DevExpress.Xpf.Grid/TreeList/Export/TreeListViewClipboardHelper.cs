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

using System.Linq;
using DevExpress.XtraExport.Helpers;
using DevExpress.Data.Summary;
using DevExpress.Data;
using DevExpress.Xpf.Data;
using DevExpress.XtraPrinting;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Printing;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using System.Reflection;
using DevExpress.Data.Linq;
using System.ComponentModel;
using DevExpress.Data.Async;
using DevExpress.Xpf.Core;
using System.Threading;
using DevExpress.Xpf.Grid.Printing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using DevExpress.Export.Xl;
using System.Collections;
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListViewClipboardHelper : TreeListViewExportHelperBase, IClipboardGridView<ColumnWrapper, TreeListNodeWrapper> {
		TreeListViewClipboardSelectionProvider provider;
		public TreeListViewClipboardHelper(TreeListView view, ExportTarget target = ExportTarget.Xlsx)
			: base(view, target) {
				this.provider = new TreeListViewClipboardSelectionProvider(view);
		}
		public override object GetRowCellValue(int rowHandle, int visibleIndex) {
			if(visibleIndex < 0 || visibleIndex >= View.VisibleColumns.Count)
				return null;
			ColumnBase column = View.VisibleColumns[visibleIndex];
			if((View.IsMultiRowSelection && View.IsRowSelected(rowHandle)) || provider.IsCellSelected(rowHandle, column))
				return View.GetExportValue(rowHandle, column) ?? string.Empty;
			return string.Empty;
		}
		#region IClipboardGridView<TCol,TRow> Members
		public bool CanCopyToClipboard() {
			return View.ClipboardMode == DevExpress.Xpf.Grid.ClipboardMode.Formatted;
		}
		public Export.Xl.XlCellFormatting GetCellAppearance(TreeListNodeWrapper row, ColumnWrapper col) {
			if(col != null && row != null && provider.IsCellSelected(row.LogicalPosition, col.Column))
				return AppearanceHelper.GetCellAppearance(row.LogicalPosition, col.Column, View, FormatConditionsCore);
			return new XlCellFormatting() { Font = new XlFont(), Fill = new XlFill(), Alignment = new XlCellAlignment() };
		}
		public string GetRowCellDisplayText(TreeListNodeWrapper row, string columnName) {
			int rowHandle = row.LogicalPosition;
			if((View.IsMultiRowSelection && View.IsRowSelected(rowHandle)) || provider.IsCellSelected(rowHandle, DataControl.ColumnsCore[columnName]))
				return DataControl.GetCellDisplayText(rowHandle, columnName);
			return string.Empty;
		}
		public IEnumerable<TreeListNodeWrapper> GetSelectedRows() {
			return provider.GetSelectedRows();
		}
		public IEnumerable<ColumnWrapper> GetSelectedColumns() {
			return provider.GetSelectedColumnsList();
		}
		public int GetSelectedCellsCount() {
			return provider.GetSelectedCellsCountCore(null);
		}
		public void ProgressBarCallBack(int progress) {
		}
		#endregion
		public bool UseHierarchyIndent(TreeListNodeWrapper row, ColumnWrapper col) {
			if(col.VisibleIndex != 0) return false;
			if(row.Node.ParentNode == null || !row.Node.ParentNode.DataProvider.TreeListSelection.GetSelected(row.Node.ParentNode)) return false;
			return true;
		}
	}
	public class TreeListViewClipboardSelectionProvider : ClipboardSelectionProvider<ColumnWrapper, TreeListNodeWrapper> {
		public TreeListViewClipboardSelectionProvider(TreeListView view) : base(view) {
		}
		protected new TreeListView View { get { return base.View as TreeListView; } }
		public override IEnumerable<TreeListNodeWrapper> GetSelectedRows() {
			List<TreeListNodeWrapper> result = new List<TreeListNodeWrapper>();
			foreach(TreeListNode node in View.TreeListDataProvider.TreeListSelection.GetSelectedNodes()) {
				if(node.ParentNode == null || !node.DataProvider.TreeListSelection.GetSelected(node.ParentNode))
					result.Add(new TreeListNodeWrapper(node));
			}
			return result;
		}
		public override IList<ColumnWrapper> GetColumns(IEnumerable collection, bool isGroupColumn = false) {
			List<ColumnWrapper> res = new List<ColumnWrapper>();
			int index = 0;
			foreach(ColumnBase column in collection)
				res.Add(DataViewExportHelperBase<ColumnWrapper, TreeListNodeWrapper>.CreateColumn(column, index++));
			return res;
		}
		public override int GetSelectedCellsCountCore(IGroupRow<TreeListNodeWrapper> groupRow) {
			if(View.IsMultiCellSelection)
				return View.GetSelectedCells().Count;
			else
				return GetColumns(View.VisibleColumns).Count() * View.GetSelectedRowHandlesCore().Length;
		}
	}
}
