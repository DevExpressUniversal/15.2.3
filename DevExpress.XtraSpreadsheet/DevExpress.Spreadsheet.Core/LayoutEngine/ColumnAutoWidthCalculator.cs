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
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Layout;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	public class ColumnAutoFitCalculator {
		readonly Worksheet sheet;
		readonly IColumnWidthCalculationService service;
		readonly DocumentLayout emptyLayout;
		HashSet<CellKey> cellsWithFilterButton;
		HashSet<CellKey> cellsWithPivotIndent; 
		readonly int filterButtonWidthInLayoutUnits;
		public ColumnAutoFitCalculator(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.service = DocumentModel.GetService<IColumnWidthCalculationService>();
			if (service == null)
				Exceptions.ThrowInternalException();
			this.emptyLayout = new DocumentLayout(DocumentModel);
			this.filterButtonWidthInLayoutUnits = sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnits(AutoFilterLayout.ImageSize);
		}
		Worksheet Sheet { get { return sheet; } }
		DocumentModel DocumentModel { get { return sheet.Workbook; } }
		void AcquireCellsWithAdditionInfo(CellRangeBase autoFitRange) {
			AquireCellsWithFilterButton(autoFitRange);
			AquireCellsWithPivotTableItems(autoFitRange);
		}
		void AquireCellsWithFilterButton(CellRangeBase autoFitRange) {
			this.cellsWithFilterButton = new HashSet<CellKey>();
			AquireCellsWithFilterButton(Sheet.AutoFilter, autoFitRange);
			TableCollection tables = Sheet.Tables;
			int count = tables.Count;
			for (int i = 0; i < count; i++) {
				Table table = tables[i];
				if (table.ShowAutoFilterButton)
					AquireCellsWithFilterButton(table.AutoFilter, autoFitRange);
			}
		}
		void AquireCellsWithFilterButton(AutoFilterBase filter, CellRangeBase autoFitRange) {
			if (!filter.Enabled)
				return;
			ProcessAdditionInfo(filter.Range, autoFitRange, (e) => {
				int from = e.TopLeft.Column;
				int to = e.BottomRight.Column;
				for (int i = from; i <= to; i++) {
					int filterColumnIndex = i - from;
					AutoFilterColumn filterColumn = filter.FilterColumns[filterColumnIndex];
					if (!filterColumn.ShowFilterButton || filterColumn.HiddenAutoFilterButton)
						continue;
					CellKey key = new CellKey(Sheet.SheetId, i, filter.Range.TopRowIndex);
					if (!cellsWithFilterButton.Contains(key))
						cellsWithFilterButton.Add(key);
				}
			});
		}
		void AquireCellsWithPivotTableItems(CellRangeBase autoFitRange) {
			this.cellsWithPivotIndent = new HashSet<CellKey>();
			foreach (PivotTable pivot in Sheet.PivotTables) {
				ProcessAdditionInfo(pivot.WholeRange, autoFitRange, (e) => {
					IEnumerator<CellKey> enumerator = e.GetAllCellKeysEnumerator();
					while (enumerator.MoveNext()) {
						PivotLayoutCellInfo info;
						pivot.LayoutCellCache.TryGetButtonInfo(enumerator.Current.GetPosition(), out info);
						if (info == null)
							continue;
						if (info.FilterButton)
							cellsWithFilterButton.Add(enumerator.Current);
						else if (info.Indent)
							cellsWithPivotIndent.Add(enumerator.Current);
					}
				});
			}
		}
		void ProcessAdditionInfo(CellRangeBase dataRange, CellRangeBase autoFitRange, Action<CellRangeBase> process) {
			VariantValue value = autoFitRange.IntersectionWith(dataRange);
			if (value.CellRangeValue != null)
				value.CellRangeValue.ForEach(process);
		}
		public void TryBestFitColumn(CellRangeBase range, ColumnBestFitMode mode) {
			DocumentModel.BeginUpdate();
			try {
				AcquireCellsWithAdditionInfo(range);
				TryBestFitColumnCore(range, mode);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void TryBestFitColumn(int columnIndex, ColumnBestFitMode mode) {
			CellRangeBase autoFitRange = CellIntervalRange.CreateColumnInterval(Sheet, columnIndex, PositionType.Relative, columnIndex, PositionType.Relative);
			AcquireCellsWithAdditionInfo(autoFitRange);
			TryBestFitColumnCore(columnIndex, mode, 0, sheet.MaxRowCount - 1);
		}
		public void TryBestFitColumn(ICell cell, ColumnBestFitMode mode) {
			if (CanBestFitColumnWidth(cell, mode)) {
				CellRangeBase autoFitRange = CellIntervalRange.CreateColumnInterval(Sheet, cell.ColumnIndex, PositionType.Relative, cell.ColumnIndex, PositionType.Relative);
				AcquireCellsWithAdditionInfo(autoFitRange);
				float newColumnWidth = CalculateColumnWidthInLayoutUnits(cell, mode, service.CalculateColumnWidthTmp(Sheet, cell.ColumnIndex));
				if (newColumnWidth > 0)
					SetBestFitColumnWidth(cell.ColumnIndex, ConvertToCharacters(newColumnWidth), mode);
			}
		}
		void TryBestFitColumnCore(CellRangeBase range, ColumnBestFitMode mode) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)range;
				foreach (CellRangeBase innerRange in union.InnerCellRanges)
					TryBestFitColumnCore(innerRange, mode);
			}
			else {
				if (range.RangeType == CellRangeType.IntervalRange && range.Width >= Sheet.MaxColumnCount) {
					VariantValue intersection = range.IntersectionWith(Sheet.GetDataRange());
					if (intersection.IsCellRange)
						range = intersection.CellRangeValue;
				}
				int firstColumnIndex = range.TopLeft.Column;
				int lastColumnIndex = range.BottomRight.Column;
				int topRow = range.TopLeft.Row;
				int bottomRow = range.BottomRight.Row;
				for (int i = range.TopLeft.Column; i <= lastColumnIndex; i++)
					TryBestFitColumnCore(i, mode, topRow, bottomRow);
				Model.History.InvalidateAnchorDatasByColumnHistoryItem historyItem = new DevExpress.XtraSpreadsheet.Model.History.InvalidateAnchorDatasByColumnHistoryItem(Sheet, firstColumnIndex, lastColumnIndex);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void TryBestFitColumnCore(int columnIndex, ColumnBestFitMode mode, int topRow, int bottomRow) {
			float newColumnWidth = -1;
			int columnWidth = service.CalculateColumnWidthTmp(Sheet, columnIndex);
			foreach (ICell cell in ColumnCollection.GetExistingCells(Sheet, columnIndex, topRow, bottomRow, false)) {
				if (CanBestFitColumnWidth(cell, mode))
					newColumnWidth = Math.Max(newColumnWidth, CalculateColumnWidthInLayoutUnits(cell, mode, columnWidth));
			}
			if (newColumnWidth > 0)
				SetBestFitColumnWidth(columnIndex, ConvertToCharacters(newColumnWidth), mode);
		}
		float ConvertToCharacters(float value) {
			return service.ConvertLayoutsToCharacters(Sheet, value, (int)Math.Ceiling(emptyLayout.UnitConverter.LayoutUnitsToPixelsF(value, DocumentModel.DpiX)));
		}
		bool CanBestFitColumnWidth(ICell cell, ColumnBestFitMode mode) {
			if ((mode & ColumnBestFitMode.OnlyIfColumnPermits) != 0) {
				IColumnRange column = Sheet.Columns.GetReadonlyColumnRange(cell.ColumnIndex);
				if (column.IsCustomWidth && !column.BestFit)
					return false;
			}
			CellRange range = Sheet.MergedCells.GetMergedCellRange(cell);
			if (range != null && range.Width > 1)
				return false;
			if ((mode & ColumnBestFitMode.IgnoreNonNumericValues) != 0) {
				VariantValue value = cell.Value;
				return value.IsNumeric;
			}
			else
				return true;
		}
		int CalculateColumnWidthInLayoutUnits(ICell cell, ColumnBestFitMode mode, int columnWidth) {
			CellFormatStringMeasurer measurer = new CellFormatStringMeasurer(cell);
			NumberFormatParameters parameters = new NumberFormatParameters();
			parameters.Measurer = measurer;
			parameters.AvailableSpaceWidth = Int32.MaxValue;
			NumberFormatResult formatResult = cell.GetFormatResult(parameters);
			if (formatResult.IsError)
				return -1;
			int buttonPadding = CalculateButtonPadding(cell);
			int textWidth;
			IActualCellAlignmentInfo alignment = cell.ActualAlignment;
			if (alignment.WrapText)
				textWidth = CalculateWrappedTextWidth(cell, formatResult.Text, columnWidth - buttonPadding, measurer);
			else
				textWidth = CalculateSingleTextWidth(formatResult.Text, measurer);
			bool hasPivotIndent = cellsWithPivotIndent.Contains(cell.Key);
			int indent = CellTextBoxBase.CalculateIndentValue(DocumentModel, Math.Max(0, (int)alignment.Indent), hasPivotIndent);
			textWidth += (indent + buttonPadding);
			if (textWidth <= columnWidth && (mode & ColumnBestFitMode.ExpandOnly) != 0)
				return -1;
			return textWidth;
		}
		int CalculateButtonPadding(ICell cell) {
			int result = 0;
			if (cellsWithFilterButton.Contains(cell.Key)) {
				XlHorizontalAlignment horizontalAlignment = cell.ActualAlignment.Horizontal;
				if (horizontalAlignment == XlHorizontalAlignment.Left || horizontalAlignment == XlHorizontalAlignment.General)
					result = filterButtonWidthInLayoutUnits;
				else if (horizontalAlignment == XlHorizontalAlignment.Center)
					result = filterButtonWidthInLayoutUnits * 2;
			}
			return result;
		}
		int CalculateWrappedTextWidth(ICell cell, string text, int columnWidth, CellFormatStringMeasurer measurer) {
#if SL || DXPORTABLE
			PrecalculatedMetricsFontInfoMeasurer gdiPlusMeasurer = cell.Worksheet.Workbook.FontCache.Measurer as PrecalculatedMetricsFontInfoMeasurer;
			FontInfo fontInfo = cell.ActualFont.GetFontInfo();
			Size size = gdiPlusMeasurer.MeasureMultilineText(text, fontInfo, columnWidth);
			if (size.Width <= 0)
				return -1;
			return size.Width + emptyLayout.TwoPixelsPadding * 3;
#else
			GdiPlusFontInfoMeasurer gdiPlusMeasurer = cell.Worksheet.Workbook.FontCache.Measurer as GdiPlusFontInfoMeasurer;
			FontInfo fontInfo = cell.ActualFont.GetFontInfo();
			Size size = DevExpress.Utils.Text.TextUtils.GetStringSize(gdiPlusMeasurer.MeasureGraphics, text, fontInfo.Font, StringFormat.GenericTypographic, columnWidth, DocumentModel.WordBreakProvider);
			if (size.Width <= 0)
				return -1;
			return size.Width + emptyLayout.TwoPixelsPadding * 3;
#endif
		}
		int CalculateSingleTextWidth(string text, CellFormatStringMeasurer measurer) {
			int width = measurer.MeasureStringWidth(text);
			if (width <= 0)
				return -1;
			return width + emptyLayout.TwoPixelsPadding * 3; 
		}
		void SetBestFitColumnWidth(int columnIndex, float newColumnWidth, ColumnBestFitMode mode) {
			newColumnWidth = Math.Min(255, newColumnWidth);
			DocumentModel.BeginUpdate();
			try {
				Column column = Sheet.Columns.GetIsolatedColumn(columnIndex);
				column.SetCustomWidthCore(newColumnWidth, false);
				bool applyBestFit = (mode & ColumnBestFitMode.IgnoreSetBestFit) == 0;
				if (applyBestFit)
					column.BestFit = true;
				Sheet.WebRanges.ChangeRange(column.GetCellIntervalRange());
				System.Diagnostics.Debug.Assert(column.IsCustomWidth && (column.BestFit || !applyBestFit));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
	}
	[Flags]
	public enum ColumnBestFitMode {
		None = 0x0,
		ExpandOnly = 0x1,
		IgnoreNonNumericValues = 0x2,
		OnlyIfColumnPermits = 0x4,
		IgnoreSetBestFit = 0x8,
		InplaceEditorMode = ExpandOnly | IgnoreNonNumericValues | OnlyIfColumnPermits,
	}
}
