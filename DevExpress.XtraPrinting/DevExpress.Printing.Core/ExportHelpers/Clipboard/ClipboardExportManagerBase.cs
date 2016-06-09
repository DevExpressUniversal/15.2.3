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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Export;
namespace DevExpress.XtraExport.Helpers {
	public abstract class ClipboardExportManagerBase<TCol, TRow> :IClipboardManager<TCol, TRow>
	 where TRow : class, IRowBase
	 where TCol : class, IColumn {
		ClipboardOptions exportOptionsCore;
		public ClipboardOptions ExportOptions { get { return exportOptionsCore; } }
		IClipboardGridView<TCol, TRow> gridViewImplementor;
		public ClipboardExportManagerBase(IClipboardGridView<TCol, TRow> gridView, ClipboardOptions exportOptions) {
			gridViewImplementor = gridView;
			exportOptionsCore = exportOptions;
		}
		public ClipboardExportManagerBase(IClipboardGridView<TCol, TRow> gridView)
			: this(gridView, new ClipboardOptions()) {
		}
		protected void ForAllRows(IClipboardGridView<TCol, TRow> view, Action<TRow> action) {
			ForAllRows(null, view, action);
		}
		internal void ForAllRows(IClipboardGroupRow<TRow> parent, IClipboardGridView<TCol, TRow> view, Action<TRow> action) {
			IEnumerable<TRow> iterator;
			if(parent == null)
				iterator = view.GetSelectedRows();
			else {
				if(parent.IsCollapsed && exportOptionsCore.CopyCollapsedData == DefaultBoolean.True)
					iterator = parent.GetAllRows();
				else
					iterator = parent.GetSelectedRows();
			}
			foreach(TRow row in iterator) {
				action(row);
				if(view.IsCancelPending) break;
				IClipboardGroupRow<TRow> igr = row as IClipboardGroupRow<TRow>;
				if(igr != null) ForAllRows(igr, view, action);
			}
		}
		double rowsCount;
		double rowIterator;
		public void SetClipboardData(DataObject dataObject) {
			try {
				IEnumerable<TCol> selectedColumns = gridViewImplementor.GetSelectedColumns()
						.Where(e => e.IsVisible == true)
						.OrderBy(e => e.VisibleIndex);
				int columnCount = selectedColumns.Count();
				if(columnCount == 0) return;
				List<IClipboardExporter<TCol, TRow>> exporters = CreateExporters(columnCount);
				if(exporters.Count == 0) return;
				BeginExport(exporters, columnCount);
				ExportHeaders(selectedColumns, exporters);
				ExportDataRows(selectedColumns, columnCount, exporters);
				EndExport(dataObject, exporters);
			} catch { }
		}
		static void EndExport(DataObject dataObject, List<IClipboardExporter<TCol, TRow>> exporters) {
			for(int i = 0; i < exporters.Count; i++) {
				exporters[i].EndExport();
				exporters[i].SetDataObject(dataObject);
			}
		}
		void ExportDataRows(IEnumerable<TCol> selectedColumns, int ColumnsCount, List<IClipboardExporter<TCol, TRow>> exporters) {
			List<ClipboardCellInfo> rowInfo = new List<ClipboardCellInfo>();
			ForAllRows(gridViewImplementor, selectedRow => {
				rowIterator++;
				gridViewImplementor.ProgressBarCallBack((int)Math.Round(rowIterator * 100 / rowsCount));
				if(selectedRow.IsGroupRow && !((selectedRow as IClipboardGroupRow<TRow>).IsTreeListGroupRow())) {
					Export.Xl.XlCellFormatting appearance = gridViewImplementor.GetCellAppearance(selectedRow, null);
					for(int i = 0; i < exporters.Count; i++) {
						exporters[i].AddGroupHeader((selectedRow as IGroupRow<TRow>).GetGroupRowHeader(), appearance, ColumnsCount);
					}
				}
				else {
					rowInfo = CreateRowInfo(rowInfo, selectedRow, selectedColumns, ColumnsCount);
					for(int i = 0; i < exporters.Count; i++) {
						exporters[i].AddRow(rowInfo);
					}
				}
			});
		}
		void ExportHeaders(IEnumerable<TCol> selectedColumns, List<IClipboardExporter<TCol, TRow>> exporters) {
			if(ExportOptions.CopyColumnHeaders != DefaultBoolean.False) {
				IEnumerable<Export.Xl.XlCellFormatting> appearance = selectedColumns.Select(e => { return gridViewImplementor.GetCellAppearance(null, e); });
				for(int i = 0; i < exporters.Count; i++) {
					exporters[i].AddHeaders(selectedColumns, appearance);
				}
			}
		}
		void BeginExport(List<IClipboardExporter<TCol, TRow>> exporters, int columnCount) {
			rowsCount = gridViewImplementor.GetSelectedCellsCount() / columnCount;
			rowIterator = 0;
			for(int i = 0; i < exporters.Count; i++) {
				exporters[i].BeginExport();
			}
		}
		List<ClipboardCellInfo> CreateRowInfo(List<ClipboardCellInfo> rowInfo, TRow selectedRow, IEnumerable<TCol> selectedColumns, int ColumnsCount) {
			rowInfo.Clear();
			for(int n = 0; n < ColumnsCount; n++) {
				TCol column = selectedColumns.ElementAt(n);
				object value = gridViewImplementor.GetRowCellValue(selectedRow, column);
				string displayValue = gridViewImplementor.GetRowCellDisplayText(selectedRow, column.FieldName);
				Export.Xl.XlCellFormatting Formatting = GetXlCellFormatting(selectedRow, column);
				rowInfo.Add(new ClipboardCellInfo(value, displayValue, Formatting));
			}
			return rowInfo;
		}
		Export.Xl.XlCellFormatting GetXlCellFormatting(TRow selectedRow, TCol column) {
			Export.Xl.XlCellFormatting Formatting = gridViewImplementor.GetCellAppearance(selectedRow, column);
			if(column.FormatSettings.ActualDataType == typeof(DateTime) || column.FormatSettings.FormatType == FormatType.DateTime) {
				Formatting.NumberFormat = Export.Xl.XlNumberFormat.ShortDate;
				Formatting.IsDateTimeFormatString = true;
			} else {
				Formatting.IsDateTimeFormatString = false;
			}
			Formatting.NetFormatString = column.FormatSettings.FormatString;
			if(gridViewImplementor.UseHierarchyIndent(selectedRow, column)) Formatting.Alignment.Indent = (byte)selectedRow.GetRowLevel();
			return Formatting;
		}
		public void AssignOptions(ClipboardOptions options) {
			exportOptionsCore.Assign(options);
		}
		protected virtual List<IClipboardExporter<TCol, TRow>> CreateExporters(int columnsCount) {
			int cellCount = gridViewImplementor.GetSelectedCellsCount();
			int rowsCount = cellCount / columnsCount;
			List<IClipboardExporter<TCol, TRow>> exporters = new List<IClipboardExporter<TCol, TRow>>();
			if(ExportOptions.AllowTxtFormat != DefaultBoolean.False) {
				exporters.Add(new ClipboardTxtExporter<TCol, TRow>());
			}
			if((ExportOptions.AllowRtfFormat != DefaultBoolean.False || ExportOptions.AllowHtmlFormat == DefaultBoolean.True)) {
				IClipboardExporter<TCol, TRow> rtfExporter = CreateRTFExporter(ExportOptions.AllowRtfFormat == DefaultBoolean.False ? false : true, ExportOptions.AllowHtmlFormat == DefaultBoolean.True ? true : false);
				if(rtfExporter != null) exporters.Add(rtfExporter);
			}
			if(ExportOptions.AllowExcelFormat != DefaultBoolean.False && rowsCount < XlsConsts.MaxRow)
				exporters.Add(new ClipboardXlsExporter<TCol, TRow>());
			if(ExportOptions.AllowCsvFormat != DefaultBoolean.False && rowsCount < XlsConsts.MaxRow)
				exporters.Add(new ClipboardCsvExporter<TCol, TRow>());
			return exporters;
		}
		protected virtual IClipboardExporter<TCol, TRow> CreateRTFExporter(bool exportRtf, bool exportHtml) {
			return null;
		}
	}
}
