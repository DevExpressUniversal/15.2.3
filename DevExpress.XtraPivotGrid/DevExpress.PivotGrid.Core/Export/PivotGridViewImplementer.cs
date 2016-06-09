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
using System.IO;
using System.Linq;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
namespace DevExpress.PivotGrid.Export {
	public class PivotGridViewImplementer : IGridView<PivotColumnImplementer, PivotRowImplementer>, IDisposable {
		public static IDataAwareExportOptions CreateDefaultExportOptions(ExportTarget target, ExportOptionsBase options, PivotGridOptionsPrint optionsPrint, string name) {
			IDataAwareExportOptions result = DataAwareExportOptionsFactory.Create(target, options as IDataAwareExportOptions);
			result.ExportTarget = target;
			XlsExportOptionsBase xlsEO = options as XlsExportOptionsBase;
			result.SheetName = xlsEO != null ? xlsEO.SheetName : name;
			result.AllowHorzLines = optionsPrint.PrintHorzLines;
			result.AllowVertLines = optionsPrint.PrintVertLines;
			result.AllowHyperLinks = DefaultBoolean.False;
			result.AllowLookupValues = DefaultBoolean.False;
			result.AllowSortingAndFiltering = DefaultBoolean.False;
			result.ShowGroupSummaries = DefaultBoolean.False;
			result.ShowTotalSummaries = DefaultBoolean.False;
			result.ShowColumnHeaders = DefaultBoolean.False;
			result.ShowPageTitle = DefaultBoolean.False;
			result.AllowCellMerge = DefaultBoolean.False;
			result.AllowFixedColumnHeaderPanel = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowFixedColumnHeaderPanel, true);
			result.AllowFixedColumns = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowFixedColumns, true);
			result.AllowGrouping = DataAwareExportOptionsFactory.UpdateDefaultBoolean(result.AllowGrouping, true);
			return result;
		}
		static FormatSettings GetGeneralFormatSettings() {
			return new FormatSettings() { ActualDataType = typeof(object) };
		}
		const string DateTimeFormatString = "m/d/yyyy h:mm"; 
		readonly List<string> headerRows = new List<string>();
		readonly IDataAwareExportOptions exportOptions;
		readonly IPivotGridExportOptions pivotOptions;
		readonly PivotColumnsCreator columnsCreator;
		readonly PivotRowsCreator rowsCreator;
		readonly int columnAreaHeight;
		readonly bool isDisplayTextMode;
		readonly bool useCellFormatting;
		readonly bool rawDataMode;
		readonly string caption;
		PivotVisualItemsBase visualItems;
		string IGridView<PivotColumnImplementer, PivotRowImplementer>.FilterString { get { return string.Empty; } }
		string IGridView<PivotColumnImplementer, PivotRowImplementer>.GetViewCaption { get { return caption; } }
		bool IGridView<PivotColumnImplementer, PivotRowImplementer>.ReadOnly { get { return false; } }
		int IGridView<PivotColumnImplementer, PivotRowImplementer>.RowHeight { get { return 0; } }
		bool IGridView<PivotColumnImplementer, PivotRowImplementer>.ShowFooter { get { return false; } }
		bool IGridView<PivotColumnImplementer, PivotRowImplementer>.ShowGroupedColumns { get { return false; } }
		IAdditionalSheetInfo IGridView<PivotColumnImplementer, PivotRowImplementer>.AdditionalSheetInfo { get { return new AdditionalSheetInfoWrapper(); } }
		bool IGridView<PivotColumnImplementer, PivotRowImplementer>.ShowGroupFooter { get { return false; } }
		bool IGridView<PivotColumnImplementer, PivotRowImplementer>.IsCancelPending { get { return false; } }
		long IGridView<PivotColumnImplementer, PivotRowImplementer>.RowCount { get { return rowsCreator.GetItemsCount(); } }
		string IGridViewBase<PivotColumnImplementer, PivotRowImplementer, PivotColumnImplementer, PivotRowImplementer>.GetRowCellHyperlink(PivotRowImplementer row, PivotColumnImplementer col) {
			return null;
		}
		public int FixedRowsCount { get { return columnAreaHeight + headerRows.Count; } }
		XlCellFormatting IGridView<PivotColumnImplementer, PivotRowImplementer>.AppearanceGroupRow { get { return null; } }
		XlCellFormatting IGridView<PivotColumnImplementer, PivotRowImplementer>.AppearanceEvenRow { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<PivotColumnImplementer, PivotRowImplementer>.AppearanceOddRow { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<PivotColumnImplementer, PivotRowImplementer>.AppearanceGroupFooter { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<PivotColumnImplementer, PivotRowImplementer>.AppearanceHeader { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<PivotColumnImplementer, PivotRowImplementer>.AppearanceFooter { get { return new XlCellFormatting(); } }
		XlCellFormatting IGridView<PivotColumnImplementer, PivotRowImplementer>.AppearanceRow { get { return new XlCellFormatting(); } }
		IGridViewAppearancePrint IGridView<PivotColumnImplementer, PivotRowImplementer>.AppearancePrint { get { return null; } }
		IEnumerable<FormatConditionObject> IGridView<PivotColumnImplementer, PivotRowImplementer>.FormatConditionsCollection { get { return null; } }
		IEnumerable<IFormatRuleBase> IGridView<PivotColumnImplementer, PivotRowImplementer>.FormatRulesCollection { get { return new List<IFormatRuleBase>(); } }
		IEnumerable<ISummaryItemEx> IGridView<PivotColumnImplementer, PivotRowImplementer>.GridGroupSummaryItemCollection { get { return new List<ISummaryItemEx>(); } }
		IEnumerable<ISummaryItemEx> IGridView<PivotColumnImplementer, PivotRowImplementer>.GridTotalSummaryItemCollection { get { return new List<ISummaryItemEx>(); } }
		IEnumerable<ISummaryItemEx> IGridView<PivotColumnImplementer, PivotRowImplementer>.FixedSummaryItems { get { return new List<ISummaryItemEx>(); } }
		IEnumerable<ISummaryItemEx> IGridView<PivotColumnImplementer, PivotRowImplementer>.GroupHeaderSummaryItems { get { return new List<ISummaryItemEx>(); } }
		PivotGridViewImplementer(IDataAwareExportOptions exportOptions, PivotGridData pivotData, string caption) {
			this.exportOptions = exportOptions;
			useCellFormatting = pivotData.DataFieldArea == PivotDataArea.RowArea;
			visualItems = pivotData.VisualItems;
			PivotFieldValueItemsCreator columnCreator = visualItems.GetItemsCreator(true);
			PivotFieldValueItemsCreator rowCreator = visualItems.GetItemsCreator(false);
			int rowAreaWidth = rowCreator.UnpagedLevelCount;
			columnAreaHeight = columnCreator.UnpagedLevelCount;
			columnsCreator = new PivotColumnsCreator(pivotData, exportOptions, rowAreaWidth, columnAreaHeight - 1, columnCreator.LastLevelUnpagedItemCount);
			rowsCreator = new PivotRowsCreator(pivotData, exportOptions, columnAreaHeight, rowAreaWidth - 1, rowCreator.LastLevelUnpagedItemCount);
			this.caption = caption;
		}
		public PivotGridViewImplementer(ExportTarget target, ExportOptionsBase options, PivotGridOptionsPrint optionsPrint, PivotGridData pivotData, string caption)
			: this(CreateDefaultExportOptions(target, options, optionsPrint, caption), pivotData, caption) {
			XlsExportOptionsBase xlsExportOptions = options as XlsExportOptionsBase;
			if(xlsExportOptions != null) {
				rawDataMode = xlsExportOptions.RawDataMode;
				isDisplayTextMode = xlsExportOptions.TextExportMode == TextExportMode.Text;
			}
			IPivotGridExportOptions pivotOptions = options as IPivotGridExportOptions;
			if(pivotOptions != null) {
				this.pivotOptions = pivotOptions;
				SubscribeOptionsEvents();
				if(pivotOptions.ExportFilterAreaHeaders == DefaultBoolean.True)
					headerRows.Add(GetHeaderString(pivotData, PivotArea.FilterArea));
				if(pivotOptions.ExportColumnAreaHeaders == DefaultBoolean.True)
					headerRows.Add(GetHeaderString(pivotData, PivotArea.ColumnArea));
				if(pivotOptions.ExportRowAreaHeaders == DefaultBoolean.True)
					headerRows.Add(GetHeaderString(pivotData, PivotArea.RowArea));
				if(pivotOptions.ExportDataAreaHeaders == DefaultBoolean.True)
					headerRows.Add(GetHeaderString(pivotData, PivotArea.DataArea));
			}
		}
		string GetHeaderString(PivotGridData pivotData, PivotArea area) {
			List<PivotGridFieldBase> fields = pivotData.GetFieldsByArea(area, true);
			int count = fields.Count;
			string text = string.Format("{0}Headers: ", area);
			for(int i = 0; i < count; i++) {
				text += fields[i].ToString();
				if(i != count - 1)
					text += ", ";
			}
			return text;
		}
		public void Export(string fileName) {
			new PivotGridExcelExporter<PivotColumnImplementer, PivotRowImplementer>(this, exportOptions).Export(fileName);
		}
		public void Export(Stream stream) {
			new PivotGridExcelExporter<PivotColumnImplementer, PivotRowImplementer>(this, exportOptions).Export(stream);
		}
		public void Dispose() {
			if(pivotOptions != null)
				UnsubscribeOptionsEvents();
			visualItems = null;
			columnsCreator.Dispose();
			rowsCreator.Dispose();
		}
		bool IGridViewBase<PivotColumnImplementer, PivotRowImplementer, PivotColumnImplementer, PivotRowImplementer>.GetAllowMerge(PivotColumnImplementer col) {
			return false;
		}
		int IGridViewBase<PivotColumnImplementer, PivotRowImplementer, PivotColumnImplementer, PivotRowImplementer>.RaiseMergeEvent(int startRow, int rowLogicalPosition, PivotColumnImplementer col) {
			return 0;
		}
		IEnumerable<PivotColumnImplementer> IGridView<PivotColumnImplementer, PivotRowImplementer>.GetAllColumns() {
			return columnsCreator.GetExportItems();
		}
		IEnumerable<PivotColumnImplementer> IGridView<PivotColumnImplementer, PivotRowImplementer>.GetGroupedColumns() {
			return new List<PivotColumnImplementer>();
		}
		IEnumerable<PivotRowImplementer> IGridView<PivotColumnImplementer, PivotRowImplementer>.GetAllRows() {
			return rowsCreator.GetExportItems();
		}
		FormatSettings CreateRowCellFormatSettings(PivotFieldValueItem item) {
			if(item != null && item.ItemType == PivotFieldValueItemType.Cell && item.Field != null) {
				Type actualDataType = visualItems.Data.GetField(item.Field).ActualDataType;
				string formatString = actualDataType == typeof(DateTime) ? DateTimeFormatString : null;
				return new FormatSettings() { FormatString = formatString, ActualDataType = actualDataType };
			}
			return GetGeneralFormatSettings();
		}
		FormatSettings IGridViewBase<PivotColumnImplementer, PivotRowImplementer, PivotColumnImplementer, PivotRowImplementer>.GetRowCellFormatting(PivotRowImplementer irow, PivotColumnImplementer icol) {
			bool isRowAreaColumn = icol.IsRowAreaColumn;
			bool isColumnAreaRow = irow.IsColumnAreaRow;
			if(isColumnAreaRow && isRowAreaColumn)
				return null;
			if(isDisplayTextMode)
				return GetGeneralFormatSettings();
			if(isColumnAreaRow)
				return CreateRowCellFormatSettings(visualItems.GetUnpagedItem(true, icol.ExportIndex, irow.ExportIndex));
			if(isRowAreaColumn)
				return CreateRowCellFormatSettings(visualItems.GetUnpagedItem(false, irow.ExportIndex, icol.ExportIndex));
			return null;
		}
		object IGridViewBase<PivotColumnImplementer, PivotRowImplementer, PivotColumnImplementer, PivotRowImplementer>.GetRowCellValue(PivotRowImplementer row, PivotColumnImplementer col) {
			bool isRowAreaColumn = col.IsRowAreaColumn;
			bool isColumnAreaRow = row.IsColumnAreaRow;
			if(isColumnAreaRow && isRowAreaColumn)
				return null;
			if(isRowAreaColumn)
				return GetValue(false, row.ExportIndex, col.ExportIndex);
			if(isColumnAreaRow)
				return GetValue(true, col.ExportIndex, row.ExportIndex);
			return GetCellValue(col.ExportIndex, row.ExportIndex);
		}
		object GetCellValue(int columnIndex, int rowIndex) {
			PivotGridCellItem cell = visualItems.CreateUnpagedCellItem(columnIndex, rowIndex);
			return isDisplayTextMode ? cell.Text : cell.Value;
		}
		object GetValue(bool isColumn, int index, int position) {
			PivotFieldValueItem item = visualItems.GetUnpagedItem(isColumn, index, position);
			if(rawDataMode || !rawDataMode && item.MinLastLevelIndex == index && item.Level == position)
				return !isDisplayTextMode && item.ItemType == PivotFieldValueItemType.Cell ? item.Value : item.DisplayText;
			return null;
		}
		void SubscribeOptionsEvents() {
			exportOptions.CustomizeCell += OnExportOptionsCustomizeCell;
			exportOptions.CustomizeSheetHeader += OnExportOptionsCustomizeSheetHeader;
			exportOptions.CustomizeSheetSettings += OnExportOptionsCustomizeSheetSettings;
		}
		void UnsubscribeOptionsEvents() {
			exportOptions.CustomizeCell -= OnExportOptionsCustomizeCell;
			exportOptions.CustomizeSheetHeader -= OnExportOptionsCustomizeSheetHeader;
			exportOptions.CustomizeSheetSettings -= OnExportOptionsCustomizeSheetSettings;
		}
		void OnExportOptionsCustomizeCell(CustomizeCellEventArgs e) {
			CustomizeCellEventArgsExtended extendedArgs = (CustomizeCellEventArgsExtended)e;
			PivotColumnImplementer col = (PivotColumnImplementer)extendedArgs.Column;
			PivotRowImplementer row = (PivotRowImplementer)extendedArgs.Row;
			CustomizePivotCellEventArgsCore args = new CustomizePivotCellEventArgsCore();
			if(!col.IsRowAreaColumn && !row.IsColumnAreaRow) {
				args.ExportArea = PivotExportArea.Data;
				args.RowType = extendedArgs.Row.IsGroupRow ? ExportCellType.GroupHeader : ExportCellType.Value;
				args.ColumnType = col.IsGroupColumn ? ExportCellType.GroupHeader : ExportCellType.Value;
				if(!extendedArgs.Row.IsGroupRow && !col.IsGroupColumn)
					args.CellItem = visualItems.CreateUnpagedCellItem(col.ExportIndex, row.ExportIndex);
			}
			if(row.IsColumnAreaRow && !col.IsRowAreaColumn) {
				args.ExportArea = PivotExportArea.Column;
				if(col.IsGroupColumn) {
					args.ColumnType = ExportCellType.GroupHeader;
					args.ValueItem = visualItems.GetItemsCreator(true).GetUnpagedItem(col.ExportIndex);
				}
				else {
					args.ColumnType = ExportCellType.Value;
					args.ValueItem = visualItems.GetUnpagedItem(true, col.ExportIndex, row.ExportIndex);
				}
			}
			if(col.IsRowAreaColumn && !row.IsColumnAreaRow) {
				args.ExportArea = PivotExportArea.Row;
				if(extendedArgs.Row.IsGroupRow) {
					args.RowType = ExportCellType.GroupHeader;
					args.ValueItem = visualItems.GetItemsCreator(false).GetUnpagedItem(row.ExportIndex);
				}
				else {
					args.RowType = ExportCellType.Value;
					args.ValueItem = visualItems.GetUnpagedItem(false, row.ExportIndex, col.ExportIndex);
				}
			}
			args.Value = e.Value;
			args.Formatting = e.Formatting;
			pivotOptions.RaiseCustomizeCellEvent(args);
			e.Handled = args.Handled;
			e.Value = args.Value;
			e.Formatting = args.Formatting;
		}
		void OnExportOptionsCustomizeSheetHeader(ContextEventArgs e) {
			foreach(string str in headerRows)
				e.ExportContext.AddRow(new object[] { str });
		}
		void OnExportOptionsCustomizeSheetSettings(CustomizeSheetEventArgs e) {
			if(exportOptions.AllowFixedColumnHeaderPanel == DefaultBoolean.True) {
				int rowIndex = FixedRowsCount - 1;
				e.ExportContext.SetFixedHeader(rowIndex);
			}
		}
	}
	class AdditionalSheetInfoWrapper : IAdditionalSheetInfo {
		public string Name { get { return "Additional Data"; } }
		public XlSheetVisibleState VisibleState { get { return XlSheetVisibleState.Hidden; } }
	}
}
