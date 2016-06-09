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
using System.Diagnostics;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using CharacterUnit = System.Single;
using LayoutUnit = System.Int32;
using PixelUnit = System.Int32;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IColumnWidthCalculationService {
		CharacterUnit ConvertLayoutsToCharacters(Worksheet sheet, float widthInLayouts, PixelUnit widthInPixels);
		CharacterUnit ConvertPixelsToCharacters(Worksheet sheet, PixelUnit widthInPixels);
		CharacterUnit AddGaps(Worksheet sheet, CharacterUnit widthInCharacters);
		CharacterUnit RemoveGaps(Worksheet sheet, CharacterUnit widthInCharacters);
		CharacterUnit CalculateDefaultColumnWidthInChars(Worksheet sheet, float maxDigitWidthInPixels);
		LayoutUnit CalculateDefaultColumnWidth(Worksheet sheet, float maxDigitWidthInLayouts, float maxDigitWidthInPixels);
		LayoutUnit CalculateColumnWidth(Worksheet sheet, int columnIndex, float maxDigitWitdhInLayouts, float maxDigitWidthInPixels); 
		LayoutUnit CalculateColumnWidthTmp(Worksheet sheet, int columnIndex);
		int CalculateRowHeight(Worksheet sheet, int rowIndex); 
		int CalculateDefaultRowHeight(Worksheet sheet);
		float CalculateDefaultRowHeightInPoints(Worksheet sheet);
		int CalculateRowMaxCellHeight(Row row, float maxDigitWidthInLayouts, float maxDigitWidthInPixels);
		int CalculateRowMaxCellHeight(Row row, float maxDigitWidthInLayouts, float maxDigitWidthInPixels, int leftColumn, int rightColumn);
		float CalculateMaxDigitWidthInLayoutUnits(DocumentModel documentModel);
		float CalculateMaxDigitWidthInPixels(DocumentModel documentModel);
		int CalculateHeaderHeight(Worksheet sheet);
		void ResetCachedValues();
	}
}
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	public class PreciseColumnWidthCalculationService : RowsDimensionCalculatorBase, IColumnWidthCalculationService {
		public CharacterUnit ConvertPixelsToCharacters(Worksheet sheet, PixelUnit widthInPixels) {
			float maxDigitWidthInPixels = CalculateMaxDigitWidthInPixels(sheet.Workbook);
			return ColumnWidthCalculationUtils.PixelToCharWidthNoGaps(widthInPixels, maxDigitWidthInPixels);
		}
		public CharacterUnit ConvertLayoutsToCharacters(Worksheet sheet, float widthInLayouts, PixelUnit widthInPixels) {
			CharacterUnit inCharacters = ColumnWidthCalculationUtils.LayoutsToCharWidthNoGaps(sheet, widthInLayouts, widthInPixels);
			return inCharacters;
		}
		public CharacterUnit CalculateDefaultColumnWidthInChars(Worksheet sheet, float maxDigitWidthInPixels) {
			return ColumnWidthCalculationUtils.CalcDefColumnWidthInChars(sheet, maxDigitWidthInPixels);
		}
		public LayoutUnit CalculateDefaultColumnWidth(Worksheet sheet, float maxDigitWidthInLayouts, float maxDigitWidthInPixels) {
			return ColumnWidthCalculationUtils.CalcDefColumnWidthInLayouts(sheet, maxDigitWidthInLayouts, maxDigitWidthInPixels);
		}
		public LayoutUnit CalculateColumnWidth(Worksheet sheet, int columnIndex, float maxDigitWitdhInLayouts, float maxDigitWidthInPixels) {
			return ColumnWidthCalculationUtils.CalculateColumnWidth(sheet, columnIndex, maxDigitWitdhInLayouts, maxDigitWidthInPixels);
		}
		public LayoutUnit CalculateColumnWidthTmp(Worksheet sheet, int columnIndex) {
			float maxDigitWitdhInLayouts = CalculateMaxDigitWidthInLayoutUnits(sheet.Workbook);
			float maxDigitWidthInPixels = CalculateMaxDigitWidthInPixels(sheet.Workbook);
			return ColumnWidthCalculationUtils.CalculateColumnWidth(sheet, columnIndex, maxDigitWitdhInLayouts, maxDigitWidthInPixels);
		}
		public virtual float CalculateMaxDigitWidthInLayoutUnits(DocumentModel documentModel) {
			return ColumnWidthCalculationUtils.GetMaxDigitWidthInLayoutUnitsF(documentModel);
		}
		public virtual float CalculateMaxDigitWidthInPixels(DocumentModel documentModel) {
			return ColumnWidthCalculationUtils.GetMaxDigitWidthInPixels(documentModel);
		}
		public CharacterUnit AddGaps(Worksheet sheet, CharacterUnit widthInCharacters) {
			return ColumnWidthCalculationUtils.AddGaps(sheet, widthInCharacters);
		}
		public CharacterUnit RemoveGaps(Worksheet sheet, CharacterUnit widthInCharacters) {
			return ColumnWidthCalculationUtils.RemoveGaps(sheet, widthInCharacters);
		}
	}
	public class FastColumnWidthCalculationService : RowsDimensionCalculatorBase, IColumnWidthCalculationService {
		public CharacterUnit ConvertLayoutsToCharacters(Worksheet sheet, float widthInLayouts, PixelUnit widthInPixels) {
			DocumentModel workbook = sheet.Workbook;
			float fixedGapInLayouts = workbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(workbook.UnitConverter.PixelsToModelUnits(5, Model.DocumentModel.Dpi));
			float maxDigitWidthInLayouts = workbook.MaxDigitWidth;
			if (widthInLayouts <= fixedGapInLayouts)
				return (CharacterUnit)0.0f;
			float withoutGaps = widthInLayouts - fixedGapInLayouts;
			int denominator = (int)(withoutGaps / maxDigitWidthInLayouts * 100 + 0.5);  
			double result = denominator / 100.0;
			return (CharacterUnit)result;
		}
		public CharacterUnit ConvertPixelsToCharacters(Worksheet sheet, int widthInPixels) {
			float maxDigitWidthInPixels = sheet.Workbook.MaxDigitWidthInPixels;
			float characters = ((int)((widthInPixels - 5) / maxDigitWidthInPixels * 100 + 0.5) / 100f);
			float widthInCharacters =
				((int)((characters * maxDigitWidthInPixels + 5) / maxDigitWidthInPixels * 256) / 256f);
			return widthInCharacters;
		}
		public virtual CharacterUnit CalculateDefaultColumnWidthInChars(Worksheet sheet, float maxDigitWidthInPixels) {
			float maxDigitWidth = CalculateMaxDigitWidthInLayoutUnits(sheet.Workbook);
			SheetFormatProperties formatProperties = sheet.Properties.FormatProperties;
			CharacterUnit width = formatProperties.DefaultColumnWidth;
			if (width == 0)
				return CalculateDefaultColumnWidthInCharsCore_Old(sheet, maxDigitWidth);
			else
				return width;
		}
		public LayoutUnit CalculateDefaultColumnWidth(Worksheet sheet, float maxDigitWidthInLayouts, float maxDigitWidthInPixels) {
			SheetFormatProperties formatProperties = sheet.Properties.FormatProperties;
			float width = formatProperties.DefaultColumnWidth;
			if (width == 0)
				width = CalculateDefaultColumnWidthCore(sheet, maxDigitWidthInLayouts);
			return MeasureCellWidth(width, maxDigitWidthInLayouts);
		}
		public virtual LayoutUnit CalculateColumnWidthTmp(Worksheet sheet, int columnIndex) {
			float maxDigitWitdhInLayouts = CalculateMaxDigitWidthInLayoutUnits(sheet.Workbook);
			float maxDigitWidthInPixels = CalculateMaxDigitWidthInPixels(sheet.Workbook);
			return this.CalculateColumnWidth(sheet, columnIndex, maxDigitWitdhInLayouts, maxDigitWidthInPixels);
		}
		public virtual LayoutUnit CalculateColumnWidth(Worksheet sheet, int columnIndex, float maxDigitWitdhInLayouts, float maxDigitWidthInPixels) {
			IColumnRange columnRange = sheet.Columns.TryGetColumnRange(columnIndex);
			if (columnRange == null)
				return CalculateDefaultColumnWidth(sheet, maxDigitWitdhInLayouts, maxDigitWidthInPixels);
			if (!columnRange.IsVisible)
				return 0;
			if (!columnRange.IsCustomWidth && columnRange.Width == 0)
				return CalculateDefaultColumnWidth(sheet, maxDigitWitdhInLayouts, maxDigitWidthInPixels);
			return MeasureCellWidth(AddGaps(sheet, columnRange.Width), maxDigitWitdhInLayouts);
		}
		protected virtual PixelUnit CalculateDefaultColumnWidthCore(Worksheet sheet, float maxDigitWidth) {
			float pixelPadding = sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnitsF(5, DocumentModel.DpiX);
			int columnWidthInChars = sheet.Properties.FormatProperties.BaseColumnWidth; 
			return (PixelUnit)((columnWidthInChars * maxDigitWidth + pixelPadding) / maxDigitWidth * 256) / 256;
		}
		CharacterUnit CalculateDefaultColumnWidthInCharsCore_Old(Worksheet sheet, float maxDigitWidth) {
			float pixelPadding = sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnitsF(3, DocumentModel.DpiX);
			CharacterUnit columnWidthInChars = sheet.Properties.FormatProperties.BaseColumnWidth; 
			float paddingInChars = pixelPadding / maxDigitWidth;
			return columnWidthInChars + paddingInChars;
		}
		protected virtual LayoutUnit MeasureCellWidth(float width, float maxDigitWidth) {
			return ColumnWidthCalculationUtils.MeasureCellWidth(width, maxDigitWidth);
		}
		public virtual float CalculateMaxDigitWidthInLayoutUnits(DocumentModel documentModel) {
			return ColumnWidthCalculationUtils.GetMaxDigitWidthInLayoutUnitsF(documentModel);
		}
		public virtual float CalculateMaxDigitWidthInPixels(DocumentModel documentModel) {
			return ColumnWidthCalculationUtils.GetMaxDigitWidthInPixels(documentModel);
		}
		public CharacterUnit AddGaps(Worksheet sheet, CharacterUnit widthInCharacters) {
			return ColumnWidthCalculationUtils.AddGaps(sheet, widthInCharacters);
		}
		public CharacterUnit RemoveGaps(Worksheet sheet, CharacterUnit widthInCharacters) {
			return ColumnWidthCalculationUtils.RemoveGaps(sheet, widthInCharacters);
		}
	}
	#region RowsDimensionCalculatorBase
	public abstract class RowsDimensionCalculatorBase {
		readonly Dictionary<FontInfoKey, int> cachedRowHeightByFontPixelsTable = new Dictionary<FontInfoKey, int>();
		int? cachedDefaultNormalRowHeight;
		int? cachedMaxRowHeight;
		public void ResetCachedValues() {
			cachedDefaultNormalRowHeight = null;
			cachedMaxRowHeight = null;
		}
		public int CalculateRowHeight(Worksheet sheet, int rowIndex) {
			Row row = sheet.Rows.TryGetRow(rowIndex);
			if (row == null)
				return CalculateDefaultRowHeight(sheet);
			if (row.HasValidCachedRowHeight)
				return row.CachedRowHeight;
			row.SetCachedRowHeight(CalculateRowHeightCore(sheet, row));
			System.Diagnostics.Debug.Assert(row.HasValidCachedRowHeight);
			return row.CachedRowHeight;
		}
		int CalculateRowHeightCore(Worksheet sheet, Row row) {
			if (!row.IsVisible)
				return 0;
			if (row.IsCustomHeight)
				return MeasureCellHeight(sheet, row.Height);
			SheetFormatProperties formatProperties = sheet.Properties.FormatProperties;
			if (formatProperties.IsCustomHeight && formatProperties.DefaultRowHeight > 0 && row.Height == 0)
				return MeasureCellHeight(sheet, formatProperties.DefaultRowHeight);
			DocumentModel workbook = sheet.Workbook;
			SetCachedDefaultNormalRowHeight(workbook);
			SetCachedMaxRowHeight(workbook);
			return CalculateRowMaxCellHeight(row, workbook.MaxDigitWidth, workbook.MaxDigitWidthInPixels);
		}
		void SetCachedDefaultNormalRowHeight(DocumentModel workbook) {
			if (cachedDefaultNormalRowHeight.HasValue)
				return;
			RunFontInfo normalInfo = workbook.StyleSheet.CellStyles.Normal.FormatInfo.FontInfo;
			cachedDefaultNormalRowHeight = RowHeightByFontPixels(normalInfo, workbook);
		}
		void SetCachedMaxRowHeight(DocumentModel documentModel) {
			if (cachedMaxRowHeight.HasValue)
				return;
			cachedMaxRowHeight = documentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(documentModel.UnitConverter.TwipsToModelUnits(Row.MaxHeightInTwips));
		}
		protected virtual int MeasureCellHeight(Worksheet sheet, float heightInModelUnits) {
			DocumentModel documentModel = sheet.Workbook;
			return (int)documentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(heightInModelUnits);
		}
		public virtual int CalculateRowMaxCellHeight(Row row, float maxDigitWidthInLayouts, float maxDigitWidthInPixels, int leftColumn, int rightColumn) {
			Worksheet sheet = (Worksheet)row.Sheet;
			MergedCellCollection mergedCells = sheet.MergedCells;
			CellRangesCachedRTree tree = new CellRangesCachedRTree();
			CellRange range = new CellRange(sheet, new CellPosition(leftColumn, row.Index), new CellPosition(rightColumn, row.Index));
			tree.InsertRange(mergedCells.GetMergedCellRangesIntersectsRange(range));
			int result = 0;
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell != null) {
					List<CellRange> mergedRanges = tree.Search(cell.GetRange());
					if (mergedRanges.Count > 0) {
						int count = mergedRanges.Count;
						for (int i = 0; i < count; i++) {
							CellRange mergedRange = mergedRanges[i];
							if (mergedRange.Height == 1 && mergedCells.IsHostMergedCell(cell, mergedRange))
								result = Math.Max(result, CalculateCellHeight(cell, maxDigitWidthInLayouts, maxDigitWidthInPixels, true));
						}
					} else
						result = Math.Max(result, CalculateCellHeight(cell, maxDigitWidthInLayouts, maxDigitWidthInPixels, false));
				}
			}
			SetCachedMaxRowHeight(row.DocumentModel);
			return Math.Min(cachedMaxRowHeight.Value, Math.Max(result, CalculateDefaultRowHeight(row.Sheet as Worksheet)));
		}
		public virtual int CalculateRowMaxCellHeight(Row row, float maxDigitWidthInLayouts, float maxDigitWidthInPixels) {
			Worksheet sheet = (Worksheet)row.Sheet;
			MergedCellCollection mergedCells = sheet.MergedCells;
			CellRangesCachedRTree tree = new CellRangesCachedRTree();
			tree.InsertRange(mergedCells.GetMergedCellRangesIntersectsRange(row.GetCellIntervalRange()));
			int result = 0;
			foreach (ICell cell in row.Cells) {
				List<CellRange> mergedRanges = tree.Search(cell.GetRange());
				if (mergedRanges.Count > 0) {
					int mergedRangesCount = mergedRanges.Count;
					for (int i = 0; i < mergedRangesCount; i++) {
						CellRange mergedRange = mergedRanges[i];
						if (mergedRange.Height == 1 && mergedCells.IsHostMergedCell(cell, mergedRange))
							result = Math.Max(result, CalculateCellHeight(cell, maxDigitWidthInLayouts, maxDigitWidthInPixels, true));
					}
				} 
				else
					result = Math.Max(result, CalculateCellHeight(cell, maxDigitWidthInLayouts, maxDigitWidthInPixels, false));
			}
			DocumentModel workbook = row.DocumentModel;
			SetCachedMaxRowHeight(workbook);
			CellStyleBase style = row.Style;
			if (style != workbook.StyleSheet.CellStyles.Normal) {
				int styleRowHeight = RowHeightByFontPixels(workbook, style.ActualFont.GetFontInfo(), style);
				return Math.Min(cachedMaxRowHeight.Value, Math.Max(result, styleRowHeight));
			}
			SetCachedDefaultNormalRowHeight(workbook);
			int defaultRowHeight = CalculateDefaultRowHeight(row.Sheet as Worksheet);
			return Math.Min(cachedMaxRowHeight.Value, Math.Max(result, defaultRowHeight));
		}
		public virtual int CalculateDefaultRowHeight(Worksheet sheet) { 
			SheetFormatProperties formatProperties = sheet.Properties.FormatProperties;
			if (formatProperties.IsCustomHeight && formatProperties.DefaultRowHeight > 0)
				return MeasureCellHeight(sheet, formatProperties.DefaultRowHeight);
			return CalculateDefaultRowHeightCore(sheet);
		}
		public virtual float CalculateDefaultRowHeightInPoints(Worksheet sheet) { 
			DocumentModel documentModel = sheet.Workbook;
			SheetFormatProperties formatProperties = sheet.Properties.FormatProperties;
			if (formatProperties.IsCustomHeight && formatProperties.DefaultRowHeight > 0)
				return documentModel.UnitConverter.ModelUnitsToPointsF(formatProperties.DefaultRowHeight);
			return documentModel.LayoutUnitConverter.LayoutUnitsToPointsF(CalculateDefaultRowHeightCoreF(sheet));
		}
		public int CalculateHeaderHeight(Worksheet sheet) {
			return CalculateDefaultRowHeightCore(sheet);
		}
		int CalculateCellHeight(ICell cell, float maxDigitWidthInLayouts, float maxDigitWidthInPixels, bool isMergedCell) {
			IActualRunFontInfo actualInfo = cell.ActualFont;
			FontInfo fontInfo = actualInfo.GetFontInfo();
			DocumentModel workbook = cell.Worksheet.Workbook;
#if !SL && !DXPORTABLE
			if(!workbook.SuppressCellValueAssignment && cell.ActualAlignment.WrapText && !isMergedCell)
				return CalculateMultilineCellHeight(fontInfo, cell, maxDigitWidthInLayouts, maxDigitWidthInPixels);
#endif
			Cell modelCell = cell as Cell;
			int rowHeightByFontPixels = modelCell != null ? RowHeightByFontPixels(workbook, fontInfo, modelCell.FontKey) : RowHeightByFontPixels(workbook, fontInfo, actualInfo);
			int rowHeightByFontLayoutUnits = workbook.LayoutUnitConverter.PixelsToLayoutUnits(rowHeightByFontPixels);
			return rowHeightByFontLayoutUnits;
		}
#if !SL && !DXPORTABLE
		int CalculateMultilineCellHeight(FontInfo fontInfo, ICell cell, float maxDigitWidthInLayouts, float maxDigitWidthInPixels) {
			Worksheet sheet = cell.Worksheet;
			DocumentModel documentModel = sheet.Workbook;
			GdiPlusFontInfoMeasurer measurer = documentModel.FontCache.Measurer as GdiPlusFontInfoMeasurer;
			if (measurer == null)
				return fontInfo.LineSpacing;
			IColumnWidthCalculationService columnWidthCalculator = documentModel.GetService<IColumnWidthCalculationService>();
			int width = columnWidthCalculator.CalculateColumnWidth(sheet, cell.ColumnIndex, maxDigitWidthInLayouts, maxDigitWidthInPixels);
			int twoPixelsPadding = (int)Math.Round(documentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(2, DocumentModel.DpiX));
			int fourPixelsPadding = twoPixelsPadding * 2;
			width = Math.Max(0, width - fourPixelsPadding);
			width = Math.Max(0, width - twoPixelsPadding);
			NumberFormatParameters parameters = new NumberFormatParameters();
			parameters.Measurer = new CellFormatStringMeasurer(cell);
			parameters.AvailableSpaceWidth = width;
			NumberFormatResult formatResult = cell.GetFormatResult(parameters);
			string text = formatResult.Text;
			if (String.IsNullOrEmpty(text))
				return fontInfo.LineSpacing;
			Size size;
			int onePixelPadding = (int)documentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(1, DocumentModel.DpiY);
			if (measurer is GdiFontInfoMeasurer) {
				size = DevExpress.Utils.Text.TextUtils.GetStringSize(measurer.MeasureGraphics, text, fontInfo.Font, StringFormat.GenericTypographic, width, DocumentModel.WordBreakProvider);
				int fontHeight = DevExpress.Utils.Text.TextUtils.GetFontHeight(measurer.MeasureGraphics, fontInfo.Font);
				int extraHeight = onePixelPadding * (size.Height / fontHeight);
				if (text.EndsWith("\n"))
					extraHeight += fontHeight + onePixelPadding;
				size = Size.Add(size, new Size(0, extraHeight));
			}
			else {
				size = Size.Ceiling(measurer.MeasureGraphics.MeasureString(text, fontInfo.Font, width, StringFormat.GenericTypographic));
				int fontHeight = fontInfo.LineSpacing;
				int extraHeight = onePixelPadding * (size.Height / fontHeight);
				if (text.EndsWith("\n"))
					extraHeight += fontHeight + onePixelPadding;
				size = Size.Add(size, new Size(0, extraHeight));
			}
			return size.Height;
		}
#else
		int CalculateMultilineCellHeight(FontInfo fontInfo, ICell cell, float maxDigitWidthInLayouts, float maxDigitWidthInPixels) {
			Worksheet sheet = cell.Worksheet;
			DocumentModel documentModel = sheet.Workbook;
			PrecalculatedMetricsFontInfoMeasurer measurer = documentModel.FontCache.Measurer as PrecalculatedMetricsFontInfoMeasurer;
			if (measurer == null)
				return fontInfo.LineSpacing;
			IColumnWidthCalculationService columnWidthCalculator = documentModel.GetService<IColumnWidthCalculationService>();
			int width = columnWidthCalculator.CalculateColumnWidth(sheet, cell.ColumnIndex, maxDigitWidthInLayouts, maxDigitWidthInPixels);
			int twoPixelsPadding = (int)Math.Round(documentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(2, DocumentModel.DpiX));
			int fourPixelsPadding = twoPixelsPadding * 2;
			width = Math.Max(0, width - fourPixelsPadding);
			width = Math.Max(0, width - twoPixelsPadding);
			NumberFormatParameters parameters = new NumberFormatParameters();
			parameters.Measurer = new CellFormatStringMeasurer(cell);
			parameters.AvailableSpaceWidth = width;
			NumberFormatResult formatResult = cell.GetFormatResult(parameters);
			string text = formatResult.Text;
			if (String.IsNullOrEmpty(text))
				return fontInfo.LineSpacing;
			Size size;
			int onePixelPadding = (int)documentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(1, DocumentModel.DpiY);
			size = Size.Ceiling(measurer.MeasureMultilineText(text, fontInfo, width));
			int fontHeight = fontInfo.LineSpacing;
			int extraHeight = onePixelPadding * (size.Height / fontHeight);
			if (text.EndsWith("\n"))
				extraHeight += fontHeight + onePixelPadding;
			size = Size.Add(size, new Size(0, extraHeight));
			return size.Height;
		}
#endif
		protected virtual int CalculateDefaultRowHeightCore(Worksheet worksheet) {
			return (int)Math.Round(CalculateDefaultRowHeightCoreF(worksheet));
		}
		protected virtual float CalculateDefaultRowHeightCoreF(Worksheet worksheet) {
			ColumnCollection columns = worksheet.Columns;
			if(columns.Count > 0)
				return CalculateDefaultMaxRowHeight(columns);
			DocumentModel workbook = worksheet.Workbook;
			SetCachedDefaultNormalRowHeight(workbook);
			float paddingInLayoutUnitsF = GetPaddingInLayoutUnitsF(workbook, GetPaddingInPixels(workbook.StyleSheet.CellStyles.Normal.FormatInfo.BorderInfo.BottomLineStyle));
			float defaultNormalRowHeightLayoutUnitsF = GetRowHeightInLayoutUnitsF(workbook, cachedDefaultNormalRowHeight.Value);
			return paddingInLayoutUnitsF + defaultNormalRowHeightLayoutUnitsF;
		}
		float CalculateDefaultMaxRowHeight(ColumnCollection columns) {
			int columnsCount = columns.Count;
			float result = GetActualRowHeight(columns.InnerList[0]);
			for(int i = 1; i < columnsCount; i++) {
				float currentHeight = GetActualRowHeight(columns.InnerList[i]);
				if(currentHeight > result)
					result = currentHeight;
			}
			return result;
		}
		float GetActualRowHeight(Column column) {
			int paddingInPixels = GetPaddingInPixels(column.ActualBorder.BottomLineStyle);
			DocumentModel documentModel = column.DocumentModel;
			return GetLineSpacingInLayoutUnitsF(documentModel, column.ActualFont) + GetPaddingInLayoutUnitsF(documentModel, paddingInPixels);
		}
		int GetPaddingInPixels(XlBorderLineStyle bottomLineStyle) {
			return BorderInfo.LinePixelThicknessTable[bottomLineStyle];
		}
		float GetLineSpacingInLayoutUnitsF(DocumentModel documentModel, IActualRunFontInfo fontInfo) {
			int rowHeightByFontPixels = RowHeightByFontPixels(documentModel, fontInfo.GetFontInfo(), fontInfo);
			float rowHeightByFontLayoutUnits = GetRowHeightInLayoutUnitsF(documentModel, rowHeightByFontPixels);
			return rowHeightByFontLayoutUnits;
		}
		int RowHeightByFontPixels(DocumentModel workbook, FontInfo fontInfo, IActualRunFontInfo actualInfo) {
			PrecalculatedMetricsFontInfo metricsFontInfo = fontInfo as PrecalculatedMetricsFontInfo;
			if(metricsFontInfo != null)
				return workbook.LayoutUnitConverter.LayoutUnitsToPixels(metricsFontInfo.LineSpacing);
			return RowHeightByFontPixelsCore(actualInfo);
		}
		int RowHeightByFontPixels(DocumentModel workbook, FontInfo fontInfo, FontInfoKey fontKey) {
			PrecalculatedMetricsFontInfo metricsFontInfo = fontInfo as PrecalculatedMetricsFontInfo;
			if(metricsFontInfo != null)
				return workbook.LayoutUnitConverter.LayoutUnitsToPixels(metricsFontInfo.LineSpacing);
			return GetCachedRowHeightByFontPixels(fontKey);
		}
		int RowHeightByFontPixels(RunFontInfo actualInfo, DocumentModel documentModel) {
			FontInfo fontInfo = actualInfo.GetFontInfo(documentModel.FontCache);
			PrecalculatedMetricsFontInfo metricsFontInfo = fontInfo as PrecalculatedMetricsFontInfo;
			if(metricsFontInfo != null)
				return documentModel.LayoutUnitConverter.LayoutUnitsToPixels(metricsFontInfo.LineSpacing);
			return RowHeightByFontPixelsCore(actualInfo);
		}
		int RowHeightByFontPixelsCore(RunFontInfo info) {
			return GetCachedRowHeightByFontPixels(info.Name, info.Size, info.Italic, info.Bold, info.StrikeThrough, info.Underline, info.Script);
		}
		int RowHeightByFontPixelsCore(IActualRunFontInfo actualInfo) {
			return GetCachedRowHeightByFontPixels(actualInfo.Name, actualInfo.Size, actualInfo.Italic, actualInfo.Bold, actualInfo.StrikeThrough, actualInfo.Underline, actualInfo.Script);
		}
		int GetCachedRowHeightByFontPixels(string fontName, double fontSize, bool italic, bool bold, bool strikeThrough, XlUnderlineType underline, XlScriptType script) {
			FontInfoKey fontKey = new FontInfoKey(fontName, fontSize, bold, italic, strikeThrough, underline, script);
			return GetCachedRowHeightByFontPixels(fontKey);
		}
		int GetCachedRowHeightByFontPixels(FontInfoKey fontKey) {
			int rowHeightByFontPixels;
			if (cachedRowHeightByFontPixelsTable.TryGetValue(fontKey, out rowHeightByFontPixels)) {
				return rowHeightByFontPixels;
			}
#if DXPORTABLE
			rowHeightByFontPixels = (LayoutUnit)Math.Min(2047, fontKey.FontSize);
#else
			rowHeightByFontPixels = GDIHeightCalculator.GetRowHeightByFont(fontKey.FontName, fontKey.FontSize, fontKey.Italic, fontKey.Bold, fontKey.StrikeThrough, fontKey.Underline, fontKey.Script);
#endif
			cachedRowHeightByFontPixelsTable.Add(fontKey, rowHeightByFontPixels);
			return rowHeightByFontPixels;
		}
		float GetRowHeightInLayoutUnitsF(DocumentModel documentModel, int rowHeightInPixels) {
			return documentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(rowHeightInPixels, DocumentModel.DpiY);
		}
		float GetPaddingInLayoutUnitsF(DocumentModel documentModel, int paddingInPixels) {
			return documentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(paddingInPixels, DocumentModel.DpiX);
		}
	}
#endregion
#region ColumnWidthCalculationUtils
	public static class ColumnWidthCalculationUtils {
		const double oneCharColWidthWithGaps = 1.71;
		public static CharacterUnit ConvertColumnWidthInChars(Worksheet sheet, CharacterUnit columnWidth) {
			float maxDigitWidthInPixels = GetMaxDigitWidthInPixels(sheet.Workbook);
			return CalcColumnWidthInCharsCore(columnWidth, maxDigitWidthInPixels);
		}
		public static CharacterUnit CalculateColumnWidthInCharsRound(IColumnRange columnRange) {
			float maxDigitWidthInPixels = GetMaxDigitWidthInPixels(columnRange.Sheet.Workbook);
			if (columnRange == null)
				return CalcDefColumnWidthInChars(columnRange.Sheet, maxDigitWidthInPixels);
			if (!columnRange.IsVisible)
				return 0;
			if (!columnRange.IsCustomWidth && columnRange.Width == 0)
				return CalcDefColumnWidthInChars(columnRange.Sheet, maxDigitWidthInPixels);
			return CalcColumnWidthInCharsCore((CharacterUnit)columnRange.Width, maxDigitWidthInPixels);
		}
		public static CharacterUnit CalcColumnWidthInCharsCore(CharacterUnit width, float maxDigitWidthInPixels) {
			PixelUnit columnWidthInPixels = CalculateColumnWidthInPixelsNoGaps(width, maxDigitWidthInPixels);
			CharacterUnit result = PixelToCharWidth(columnWidthInPixels, maxDigitWidthInPixels);
			return result;
		}
		public static LayoutUnit CalculateColumnWidth(Worksheet sheet, int columnIndex, float maxDigitWitdhInLayouts, float maxDigitWidthInPixels) {
			IColumnRange columnRange = sheet.Columns.TryGetColumnRange(columnIndex);
			if (columnRange == null) {
				return CalcDefColumnWidthInLayouts(sheet, maxDigitWitdhInLayouts, maxDigitWidthInPixels);
			}
			if (!columnRange.IsVisible)
				return 0;
			if (!columnRange.IsCustomWidth && columnRange.Width == 0)
				return CalcDefColumnWidthInLayouts(sheet, maxDigitWitdhInLayouts, maxDigitWidthInPixels);
			return MeasureCellWidth(AddGaps(sheet, columnRange.Width), maxDigitWitdhInLayouts);
		}
		public static LayoutUnit MeasureCellWidth(CharacterUnit widthInChars, float maxDigitWidth) {
			return (int)(((256 * widthInChars + (int)(128 / maxDigitWidth)) / 256) * maxDigitWidth);
		}
		public static float GetMaxDigitWidthInPixels(DocumentModel workbook) {
			return workbook.MaxDigitWidthInPixels;
		}
		public static float GetMaxDigitWidthInLayoutUnitsF(DocumentModel workbook) {
			return workbook.MaxDigitWidth;
		}
		public static CharacterUnit CalcDefColumnWidthInChars(Worksheet sheet, float maxDigitWidthInPixels) {
			SheetFormatProperties formatProperties = sheet.Properties.FormatProperties;
			CharacterUnit width = (CharacterUnit)formatProperties.DefaultColumnWidth;
			if (width != 0)
				return width;
			PixelUnit columnWidthInPixels = CalcDefaultColumnWidthInPixelsCore(formatProperties.BaseColumnWidth, maxDigitWidthInPixels);
			CharacterUnit result = PixelToCharWidth(columnWidthInPixels, maxDigitWidthInPixels);
			return result;
		}
		public static LayoutUnit CalcDefColumnWidthInLayouts(Worksheet sheet, float maxDigitWidthInLayouts, float maxDigitWidthInPixels) {
			CharacterUnit inChar = CalcDefColumnWidthInChars(sheet, maxDigitWidthInPixels);
			return CalculateColumnWidthInPixelsNoGaps(AddGaps(sheet, inChar), maxDigitWidthInLayouts);
		}
		public static PixelUnit CalcDefaultColumnWidthInPixelsCore(float baseColumnWidth, float maxDigitWidthInPixels) {
			int gaps = CalcColumnGaps(maxDigitWidthInPixels);
			int columnWidthInPixels = CalculateColumnWidthInPixelsNoGaps(baseColumnWidth, maxDigitWidthInPixels);
			if (baseColumnWidth < 1.0)
				columnWidthInPixels = (int)(columnWidthInPixels + gaps * baseColumnWidth);
			else
				columnWidthInPixels = (columnWidthInPixels + gaps + 7) / 8 * 8;
			return columnWidthInPixels;
		}
		static PixelUnit CalculateColumnWidthInPixelsNoGaps(CharacterUnit widthInChars, float maxDigitWidthInPixels) {
			float newVariable = ((float)(256 * widthInChars) + (float)(128 / maxDigitWidthInPixels)) / (float)256.0;
			return (PixelUnit)(newVariable * maxDigitWidthInPixels);
		}
		public static CharacterUnit LayoutsToCharWidthNoGaps(Worksheet worksheet, float widthInLayouts, PixelUnit widthInPixelsForCheck) {
			float maxDigitWidthInLayouts = GetMaxDigitWidthInLayoutUnitsF(worksheet.Workbook);
			float withoutGaps = widthInLayouts;;
			int denominator = (int)(withoutGaps / maxDigitWidthInLayouts * 100 + 0.5);  
			double result = denominator / 100.0;
			return (CharacterUnit)result;
		}
		public static CharacterUnit PixelToCharWidth(PixelUnit widthInPixels, float maxDigitWidthInPixels) {
			PixelUnit gaps = CalcColumnGaps(maxDigitWidthInPixels);
			if (widthInPixels <= gaps)
				return (CharacterUnit)0.0f;
			return (CharacterUnit)((PixelUnit)((widthInPixels - gaps) / maxDigitWidthInPixels * 100 + 0.5) / 100.0);
		}
		public static CharacterUnit PixelToCharWidthNoGaps(PixelUnit widthInPixels, float maxDigitWidthInPixels) {
			return (CharacterUnit)((PixelUnit)(widthInPixels / maxDigitWidthInPixels * 100 + 0.5) / 100.0);
		}
		static PixelUnit CalcColumnGaps(float maxDigitWidthInPixels) {
			int gap = (int)Math.Ceiling(maxDigitWidthInPixels / 4.0);
			if (gap < 2)
				gap = 2;
			return gap * 2 + 1;
		}
		public static CharacterUnit AddGaps(Worksheet sheet, CharacterUnit widthInCharacters) {
			if (widthInCharacters <= 1.0)
				return (CharacterUnit)(widthInCharacters * oneCharColWidthWithGaps);
			float maxDigitWidthInLayouts = GetMaxDigitWidthInLayoutUnitsF(sheet.Workbook);
			float maxDigitWidthInPixels = GetMaxDigitWidthInPixels(sheet.Workbook);
			PixelUnit gaps = CalcColumnGaps(maxDigitWidthInPixels);
			int gapsInModels = sheet.Workbook.UnitConverter.PixelsToModelUnits(gaps, DocumentModel.Dpi);
			float gapsInLayouts = sheet.Workbook.ToDocumentLayoutUnitConverter.ToLayoutUnits((float)gapsInModels);
			float widthInLayouts = MeasureCellWidth(widthInCharacters, maxDigitWidthInLayouts);
			widthInLayouts += gapsInLayouts;
			return LayoutsToCharWidthNoGaps(sheet, widthInLayouts, 0);
		}
		public static CharacterUnit RemoveGaps(Worksheet sheet, CharacterUnit widthInCharacters) {
			if (widthInCharacters <= oneCharColWidthWithGaps)
				return (CharacterUnit)(widthInCharacters / oneCharColWidthWithGaps);
			float maxDigitWidthInLayouts = GetMaxDigitWidthInLayoutUnitsF(sheet.Workbook);
			float maxDigitWidthInPixels = GetMaxDigitWidthInPixels(sheet.Workbook);
			PixelUnit gaps = CalcColumnGaps(maxDigitWidthInPixels);
			int gapsInModels = sheet.Workbook.UnitConverter.PixelsToModelUnits(gaps, DocumentModel.Dpi);
			float gapsInLayouts = sheet.Workbook.ToDocumentLayoutUnitConverter.ToLayoutUnits((float)gapsInModels);
			float widthInLayouts = MeasureCellWidth(widthInCharacters, maxDigitWidthInLayouts);
			widthInLayouts -= gapsInLayouts;
			return LayoutsToCharWidthNoGaps(sheet, widthInLayouts, 0);
		}
	}
#endregion
#region PageGridCalculator
	public class PageGridCalculator {
#region static
		public static List<int> CreateColumnModelIndicies(CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			IColumnWidthCalculationService columnCalculationService = sheet.Workbook.GetService<IColumnWidthCalculationService>();
			List<int> result = new List<int>();
			int firstColumn = range.TopLeft.Column;
			int lastColumn = range.BottomRight.Column;
			for (int i = firstColumn; i <= lastColumn; i++)
				if (columnCalculationService.CalculateColumnWidthTmp(sheet, i) > 0)
					result.Add(i);
			return result;
		}
		public static List<int> CreateRowModelIndicies(CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			IColumnWidthCalculationService columnCalculationService = sheet.Workbook.GetService<IColumnWidthCalculationService>();
			List<int> result = new List<int>();
			int firstRow = range.TopLeft.Row;
			int lastRow = range.BottomRight.Row;
			for (int i = firstRow; i <= lastRow; i++)
				if (columnCalculationService.CalculateRowHeight(sheet, i) > 0)
					result.Add(i);
			return result;
		}
#endregion
#region Fields
		readonly Worksheet sheet;
		readonly float cachedMaxDigitWidth;
		readonly float cachedMaxDigitWidthInPixels;
		readonly LayoutUnit cachedDefaultColumnWidth;
		readonly CharacterUnit cachedDefaultColumnWidthInChars;
		readonly int actualDefaultRowHeight;
		readonly Rectangle pageClientBounds;
		IColumnWidthCalculationService columnWidthCalculator;
#endregion
		public PageGridCalculator(Worksheet sheet, Rectangle pageClientBounds) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.pageClientBounds = pageClientBounds;
			this.columnWidthCalculator = sheet.Workbook.GetService<IColumnWidthCalculationService>();
			this.actualDefaultRowHeight = columnWidthCalculator.CalculateDefaultRowHeight(sheet);
			this.cachedMaxDigitWidth = columnWidthCalculator.CalculateMaxDigitWidthInLayoutUnits(sheet.Workbook);
			this.cachedMaxDigitWidthInPixels = columnWidthCalculator.CalculateMaxDigitWidthInPixels(sheet.Workbook);
			this.cachedDefaultColumnWidth = columnWidthCalculator.CalculateDefaultColumnWidth(sheet, this.cachedMaxDigitWidth, this.cachedMaxDigitWidthInPixels);
			this.cachedDefaultColumnWidthInChars = columnWidthCalculator.CalculateDefaultColumnWidthInChars(sheet, this.cachedMaxDigitWidthInPixels);
		}
#region Properties
		public Worksheet Sheet { get { return sheet; } }
		public Rectangle PageClientBounds { get { return pageClientBounds; } }
		public int ClientLeft { get { return pageClientBounds.Left; } }
		public int ClientTop { get { return pageClientBounds.Top; } }
		public int ClientWidth { get { return pageClientBounds.Width; } }
		public int ClientHeight { get { return pageClientBounds.Height; } }
		public int ActualDefaultRowHeight { get { return actualDefaultRowHeight; } }
		public float MaxDigitWidth { get { return cachedMaxDigitWidth; } }
		public float MaxDigitWidthInPixels { get { return cachedMaxDigitWidthInPixels; } }
		public LayoutUnit DefaultColumnWidth { get { return cachedDefaultColumnWidth; } }
		public CharacterUnit DefaultColumnWidthInChars { get { return cachedDefaultColumnWidthInChars; } }
		public IColumnWidthCalculationService InnerCalculator { get { return this.columnWidthCalculator; } }
#endregion
		public PageGrid CreateTotalColumnGrid(CellRange range, float scalingFactor, bool forceCreateNonEmptyGrid) {
			PageGrid result = CreateTotalColumnGrid(range, scalingFactor);
			if (forceCreateNonEmptyGrid && result.Count <= 0) {
				PageGridItem item = new PageGridItem();
				item.ModelIndex = range.TopLeft.Row;
				item.Index = 0;
				item.Near = 0;
				item.Extent = 1;
				result.Add(item);
			}
			return result;
		}
		public PageGrid CreateTotalColumnGrid(CellRange range, float scalingFactor) {
			IColumnWidthCalculationService columnCalculationService = Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			PageGrid result = new PageGrid();
			int firstColumn = range.TopLeft.Column;
			int lastColumn = range.BottomRight.Column;
			int currentNear = 0;
			for (int i = firstColumn; i <= lastColumn; i++) {
				int itemExtent = (int)(columnCalculationService.CalculateColumnWidth(sheet, i, cachedMaxDigitWidth, cachedMaxDigitWidthInPixels) * scalingFactor);
				if (itemExtent <= 0)
					continue; 
				PageGridItem item = new PageGridItem();
				item.ModelIndex = i;
				item.Index = result.Count;
				item.Near = currentNear;
				item.Extent = itemExtent;
				result.Add(item);
				currentNear += itemExtent;
			}
			return result;
		}
		public PageGrid CreateTotalRowGrid(CellRange range, float scalingFactor, bool forceCreateNonEmptyGrid) {
			PageGrid result = CreateTotalRowGrid(range, scalingFactor);
			if (forceCreateNonEmptyGrid && result.Count <= 0) {
				PageGridItem item = new PageGridItem();
				item.ModelIndex = range.TopLeft.Row;
				item.Index = 0;
				item.Near = 0;
				item.Extent = 1;
				result.Add(item);
			}
			return result;
		}
		public PageGrid CreateTotalRowGrid(CellRange range, float scalingFactor) {
			PageGrid result = new PageGrid();
			int firstRow = range.TopLeft.Row;
			int lastRow = range.BottomRight.Row;
			int currentNear = 0;
			for (int i = firstRow; i <= lastRow; i++) {
				int itemExtent = (int)(CalculateRowHeight(i) * scalingFactor);
				if (itemExtent <= 0)
					continue; 
				PageGridItem item = new PageGridItem();
				item.ModelIndex = i;
				item.Index = result.Count;
				item.Near = currentNear;
				item.Extent = itemExtent;
				result.Add(item);
				currentNear += itemExtent;
			}
			return result;
		}
		public List<PageGrid> CalculateColumnGrid(PageGrid totalGrid, int headingWidth, bool centerContent) {
			return CalculateGridCore(totalGrid, new PageColumnGridCalculatorController(this, new List<PageGrid>()), ClientWidth, headingWidth, centerContent);
		}
		public List<PageGrid> CalculateRowGrid(PageGrid totalGrid, int headingHeight, bool centerContent) {
			return CalculateGridCore(totalGrid, new PageRowGridCalculatorController(this, new List<PageGrid>()), ClientHeight, headingHeight, centerContent);
		}
		List<PageGrid> CalculateGridCore(PageGrid totalGrid, PageGridCalculatorController controller, int pageClientExtent, int headingOffset, bool centerContent) {
			List<PageGrid> result = controller.Grid;
			controller.BeginPage(headingOffset);
			int count = totalGrid.Count;
			for (int i = 0; i < count; i++) {
				if (!controller.ProcessItem(totalGrid[i])) {
					if (centerContent)
						controller.CenterGrid(pageClientExtent);
					controller.EndPage(pageClientExtent);
					controller.BeginPage(headingOffset);
					controller.ProcessItem(totalGrid[i]);
				}
			}
			if (centerContent)
				controller.CenterGrid(pageClientExtent);
			controller.EndPage(pageClientExtent);
			return result;
		}
		public int CalculateRowHeight(int rowIndex) {																	  
			return columnWidthCalculator.CalculateRowHeight(sheet, rowIndex);
		}
		public int CalculateDefaultRowHeight() {
			return columnWidthCalculator.CalculateDefaultRowHeight(sheet);
		}
		public int EnsureColumnGridExists(PageGrid columnGrid, Rectangle bounds) {
			if (bounds.Right <= columnGrid.Last.Far)
				return columnGrid.Count - 1;
			int firstActualIndex = columnGrid.ActualFirstIndex;
			int lastActualIndex = columnGrid.ActualLastIndex;
			List<PageGrid> grid = new List<PageGrid>();
			PageColumnGridCalculatorController controller = new PageColumnGridCalculatorController(this, grid);
			controller.ContinuePage(columnGrid);
			int modelColumnIndex = columnGrid.Last.ModelIndex;
			int currentNear = columnGrid.Last.Far;
			int lastItemIndex = columnGrid.Count - 1;
			for (; ; ) {
				modelColumnIndex++;
				int itemExtent = columnWidthCalculator.CalculateColumnWidth(Sheet, modelColumnIndex, cachedMaxDigitWidth, cachedMaxDigitWidthInPixels);
				if (itemExtent <= 0)
					continue; 
				PageGridItem item = new PageGridItem();
				item.ModelIndex = modelColumnIndex;
				item.Index = lastItemIndex + 1;
				item.Near = currentNear;
				item.Extent = itemExtent;
				currentNear += itemExtent;
				if (!controller.ProcessItem(item)) {
					controller.EndPage(ClientWidth);
					break;
				}
				lastItemIndex = item.Index;
				if (currentNear >= bounds.Right)
					break;
			}
			columnGrid.SetActualIndicies(firstActualIndex, lastActualIndex);
			return lastItemIndex;
		}
	}
	public class DesignPageColumnGridCalculator : PageGridCalculator {
		public DesignPageColumnGridCalculator(Worksheet sheet, Rectangle pageClientBounds)
			: base(sheet, pageClientBounds) {
		}
		public PageGrid CalculateDesignColumnGrid(PageGrid totalGrid, CellRange range, int currentNear) {
			return CalculateGridCore(totalGrid, new DesignPageColumnGridCalculatorController(this, new List<PageGrid>()), ClientWidth, range, currentNear);
		}
		PageGrid CalculateGridCore(PageGrid totalGrid, PageGridCalculatorController controller, int pageClientExtent, CellRange range, int currentNear) {
			List<PageGrid> result = controller.Grid;
			controller.BeginPage(currentNear);
			for (int i = range.TopLeft.Column; i < totalGrid.Count; i++) {
				if (!controller.ProcessItem(totalGrid[i])) {
					controller.EndPage(pageClientExtent);
					break;
				}
			}
			return result[0];
		}
	}
#endregion
#region PageGridCalculatorController (abstract class)
	public abstract class PageGridCalculatorController {
		readonly PageGridCalculator calculator;
		readonly List<PageGrid> grid;
		PageGrid currentGrid;
		int currentNear;
		protected PageGridCalculatorController(PageGridCalculator calculator, List<PageGrid> grid) {
			Guard.ArgumentNotNull(calculator, "calculator");
			Guard.ArgumentNotNull(grid, "grid");
			this.calculator = calculator;
			this.grid = grid;
		}
		public List<PageGrid> Grid { get { return grid; } }
		protected abstract int ClientExtent { get; }
		protected PageGridCalculator Calculator { get { return calculator; } }
		protected Worksheet Sheet { get { return calculator.Sheet; } }
		protected int CurrentNear { get { return currentNear; } }
		public virtual void BeginPage() {
			BeginPage(0);
		}
		public void BeginPage(int currentNear) {
			this.currentGrid = new PageGrid();
			this.grid.Add(currentGrid);
			this.currentNear = currentNear;
		}
		public virtual void ContinuePage(PageGrid grid) {
			this.currentGrid = grid;
			this.grid.Add(currentGrid);
			this.currentNear = grid.Last.Far;
		}
		public virtual bool ProcessItem(PageGridItem item) {
			int modelIndex = item.ModelIndex;
			if (currentGrid.Count > 0) {
				if (IsBreakBefore(modelIndex))
					return false;
				if (IsBreakItem(item))
					return false;
			}
			PageGridItem newItem = new PageGridItem();
			newItem.ModelIndex = modelIndex;
			newItem.Index = currentGrid.Count;
			newItem.Near = currentNear;
			newItem.Extent = item.Extent;
			currentGrid.Add(newItem);
			currentNear += newItem.Extent;
			return true;
		}
		protected virtual bool IsBreakItem(PageGridItem item) {
			return CurrentNear + item.Extent > ClientExtent;
		}
		public virtual void EndPage(int pageClientExtent) {
		}
		public void CenterGrid(int pageClientExtent) {
			int offset = (pageClientExtent - currentGrid.Last.Far) / 2;
			this.grid[this.grid.IndexOf(currentGrid)] = currentGrid.OffsetGrid(offset);
		}
		protected abstract bool IsBreakBefore(int modelIndex);
	}
#endregion
#region PageColumnGridCalculatorController
	public class PageColumnGridCalculatorController : PageGridCalculatorController {
		public PageColumnGridCalculatorController(PageGridCalculator calculator, List<PageGrid> grid)
			: base(calculator, grid) {
		}
		protected override int ClientExtent { get { return Calculator.ClientWidth; } }
		protected override bool IsBreakBefore(int columnIndex) {
			if (Sheet.PrintSetup.FitToPage && Sheet.PrintSetup.FitToWidth != 0)
				return false;
			return Sheet.ColumnBreaks.Contains(columnIndex);
		}
	}
	public class DesignPageColumnGridCalculatorController : PageColumnGridCalculatorController {
		public DesignPageColumnGridCalculatorController(PageGridCalculator calculator, List<PageGrid> grid)
			: base(calculator, grid) {
		}
		protected override bool IsBreakBefore(int columnIndex) {
			return false;
		}
		protected override bool IsBreakItem(PageGridItem item) {
			return CurrentNear >= ClientExtent;
		}
	}
#endregion
#region PageRowGridCalculatorController
	public class PageRowGridCalculatorController : PageGridCalculatorController {
		public PageRowGridCalculatorController(PageGridCalculator calculator, List<PageGrid> grid)
			: base(calculator, grid) {
		}
		protected override int ClientExtent { get { return Calculator.ClientHeight; } }
		protected override bool IsBreakBefore(int rowIndex) {
			if (Sheet.PrintSetup.FitToPage && Sheet.PrintSetup.FitToHeight != 0)
				return false;
			return Sheet.RowBreaks.Contains(rowIndex);
		}
	}
#endregion
#region PageDimensionsCalculator
	public class PageDimensionsCalculatorBase {
		readonly Worksheet sheet;
		public PageDimensionsCalculatorBase(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
		}
		public Rectangle CalculatePageClientBounds(Size actualPageSize) {
			Point clientOrigin = CalculateClientOrigin();
			Size clientSize = CalculateClientSize(actualPageSize);
			return new Rectangle(clientOrigin, clientSize);
		}
		public virtual Size CalculateActualPageSize() {
			PrintSetup printSetup = sheet.PrintSetup;
			Size size = PaperSizeCalculator.CalculatePaperSize(printSetup.PaperKind);
			DocumentLayoutUnitConverter layoutUnitConverter = sheet.Workbook.LayoutUnitConverter;
			size = new Size(layoutUnitConverter.TwipsToLayoutUnits(size.Width), layoutUnitConverter.TwipsToLayoutUnits(size.Height));
			if (printSetup.Orientation == ModelPageOrientation.Landscape) {
				int value = size.Width;
				size.Width = size.Height;
				size.Height = value;
			}
			return size;
		}
		protected virtual Point CalculateClientOrigin() {
			Margins margins = sheet.Margins;
			DocumentModelUnitToLayoutUnitConverter unitConverter = sheet.Workbook.ToDocumentLayoutUnitConverter;
			return new Point(unitConverter.ToLayoutUnits(margins.Left), unitConverter.ToLayoutUnits(margins.Top));
		}
		protected virtual Size CalculateClientSize(Size size) {
			Margins margins = sheet.Margins;
			DocumentModelUnitToLayoutUnitConverter unitConverter = sheet.Workbook.ToDocumentLayoutUnitConverter;
			size.Width -= unitConverter.ToLayoutUnits(margins.Left + margins.Right);
			size.Height -= unitConverter.ToLayoutUnits(margins.Top + margins.Bottom);
			return size;
		}
	}
	public class DesignPageDimensionsCalculator : PageDimensionsCalculatorBase {
		Rectangle pageBounds;
		public DesignPageDimensionsCalculator(Worksheet sheet, Rectangle pageBounds)
			: base(sheet) {
			this.pageBounds = pageBounds;
		}
		protected override Point CalculateClientOrigin() {
			return Point.Empty;
		}
		public override Size CalculateActualPageSize() {
			return pageBounds.Size;
		}
		protected override Size CalculateClientSize(Size size) {
			return size;
		}
	}
#endregion
}
