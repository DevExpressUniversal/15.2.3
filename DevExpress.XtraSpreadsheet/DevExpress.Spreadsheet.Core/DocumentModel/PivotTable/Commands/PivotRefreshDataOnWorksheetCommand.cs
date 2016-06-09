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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotRefreshDataOnWorksheetCommand
	public class PivotRefreshDataOnWorksheetCommand : PivotTableTransactionCommand {
		public const int MaxIndent = 250;
		public const int NoIndent = -1;
		delegate FormattedVariantValue HeaderCaptionGenerator(int i, IPivotLayoutItem item, PivotTableColumnRowFieldIndices indices);
		readonly ShowValuesAsCache calcCache;
		readonly PivotCellKey key;
		readonly PivotReportHeadersBuilder columnBuilder;
		readonly PivotReportHeadersBuilder rowBuilder;
		CellRange newRange;
		CellUnion pageFieldsRange;
		CellRangeBase wholeRange;
		int firstHeaderRow;
		int firstDataRow;
		int firstDataColumn;
		int rowPageCount;
		int columnPageCount;
		public PivotRefreshDataOnWorksheetCommand(IPivotTableTransaction transaction)
			: base(transaction) {
			this.key = new PivotCellKey(transaction.PivotTable.RowFields.KeyIndicesCount + transaction.PivotTable.ColumnFields.KeyIndicesCount);
			this.calcCache = new ShowValuesAsCache();
			this.columnBuilder = new PivotReportColumnHeadersBuilder(PivotTable);
			this.rowBuilder = new PivotReportRowHeadersBuilder(PivotTable);
		}
		protected internal override bool Validate() {
			this.columnBuilder.PrepareCache();
			this.rowBuilder.PrepareCache();
			CalculateLocation();
			if (!Validate(newRange, pageFieldsRange, wholeRange, PivotTable.Location.WholeRange, Transaction.SuppressRefreshDataOnWorksheetValidation, ErrorHandler))
				return false;
			if (rowBuilder.IndentOverflow)
				if (!(ErrorHandler is DevExpress.XtraSpreadsheet.API.Native.Implementation.ApiErrorHandler))
					ErrorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotTableIndentOverflow));
			return true;
		}
		public static bool Validate(CellRangeBase newRange, CellRangeBase newPageFieldsRange, CellRangeBase newWholeRange, CellRangeBase oldWholeRange, bool suppressValidation, IErrorHandler errorHandler) {
			bool willFitOnWorksheet = RangeWillFitOnWorksheet(ref newWholeRange);
			if (!willFitOnWorksheet) {
				RangeWillFitOnWorksheet(ref newRange);
				if (newPageFieldsRange != null)
					RangeWillFitOnWorksheet(ref newPageFieldsRange);
				if (!suppressValidation && errorHandler.HandleError(new ClarificationErrorInfo(ModelErrorType.PivotTableWillNotFitOnTheSheet)) == ErrorHandlingResult.Abort)
					return false;
			}
			if (suppressValidation)
				return true;
			Worksheet sheet = newRange.Worksheet as Worksheet;
			if (sheet.Tables.ContainsItemsInRange(newWholeRange, true))
				if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotTableReportCanNotOverlapTable)) == ErrorHandlingResult.Abort)
					return false;
			if (!sheet.ArrayFormulaRanges.CanRangeRemove(newWholeRange, RemoveCellMode.Default))
				if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray)) == ErrorHandlingResult.Abort)
					return false;
			CellRangeBase newRangeExcludeOldRange = oldWholeRange == null ? newWholeRange : newWholeRange.ExcludeRange(oldWholeRange);
			if (newRangeExcludeOldRange != null) {
				if (sheet.PivotTables.ContainsItemsInRange(newRangeExcludeOldRange, true))
					if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotTableCanNotOverlapPivotTable)) == ErrorHandlingResult.Abort)
						return false;
				if (newRangeExcludeOldRange.Exists(x => sheet.MergedCells.HasMergedCellRangesIntersectsButNotCoversByRange(x)))
					if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.CantDoThatToAMergedCell)) == ErrorHandlingResult.Abort)
						return false;
				if (newRangeExcludeOldRange.Exists(x => x.HasData()))
					if (errorHandler.HandleError(new ClarificationErrorInfo(ModelErrorType.PivotTableWillOverrideSheetCells)) == ErrorHandlingResult.Abort)
						return false;
			}
			return true;
		}
		static bool RangeWillFitOnWorksheet(ref CellRangeBase rangeBase) {
			if (rangeBase.RangeType == CellRangeType.UnionRange) {
				bool will = true;
				CellUnion union = (CellUnion)rangeBase;
				for (int i = union.InnerCellRanges.Count - 1; i >= 0; --i) {
					CellRangeBase range = union.InnerCellRanges[i];
					will &= RangeWillFitOnWorksheet(ref range);
					if (range == null || range.AreasCount < 1)
						union.InnerCellRanges.RemoveAt(i);
				}
				return will;
			}
			else {
				CellRange range = (CellRange)rangeBase;
				if (!rangeBase.TopLeft.IsValid) {
					rangeBase = null;
					return false;
				}
				if (!range.BottomRight.IsValid) {
					range.BottomRight = range.GetResizedLimited(0, 0, 0, 0).BottomRight;
					return false;
				}
				return true;
			}
		}
		void CalculateLocation() {
			CellRange oldRange = PivotTable.Location.Range;
			if (PivotTable.PageFields.Count == 0 && PivotTable.RowFields.Count == 0 && PivotTable.ColumnFields.Count == 0 && PivotTable.DataFields.Count == 0) {
				firstHeaderRow = 1;
				firstDataRow = 1;
				firstDataColumn = 0;
				rowPageCount = 0;
				columnPageCount = 0;
				newRange = PivotTableLocation.GetDefaultCellRange(oldRange);
				pageFieldsRange = null;
				wholeRange = newRange.Clone();
			}
			else {
				CalculateLocationIndices();
				CalculatePageCounts();
				CalculateNewRange(oldRange);
			}
		}
		void CalculateLocationIndices() {
			bool hasCaption = ShouldFillCaption(PivotTable);
			bool hasColumnLabels = PivotTable.ColumnFields.Count > 0;
			bool hasRowLabels = PivotTable.RowFields.Count > 0;
			bool hiddenCaption = PivotTable.RowFields.Count > 0 && PivotTable.ColumnFields.Count > 0 && PivotTable.DataFields.Count == 0;
			bool hiddenColumnLabels = !PivotTable.ShowHeaders || !PivotTable.ShowValuesRow && PivotTable.ColumnFields.HasValuesField && PivotTable.ColumnFields.Count == 1;
			bool hiddenRowLabels = !PivotTable.ShowHeaders;
			bool hasColumnLabelsRow = hasCaption && !hiddenCaption || hasColumnLabels && !hiddenColumnLabels;
			bool hasRowLabelsRow = hasCaption && !hiddenCaption || hasRowLabels && !hiddenRowLabels;
			bool hiddenColumnLabelsRow = !hasCaption && hasColumnLabels && hiddenColumnLabels || !hasColumnLabels && hasCaption && hiddenCaption;
			bool hiddenRowLabelsRow = hiddenRowLabels && PivotTable.ColumnFields.Count == 0 && PivotTable.DataFields.Count == 0;
			bool containsOnlyPageFields = PivotTable.PageFields.Count > 0 && PivotTable.RowFields.Count == 0 && PivotTable.ColumnFields.Count == 0 && PivotTable.DataFields.Count == 0;
			this.firstHeaderRow = hiddenColumnLabelsRow || hiddenRowLabelsRow || containsOnlyPageFields ? 0 : 1;
			this.firstDataRow = GetHeaderLength(columnBuilder);
			this.firstDataColumn = GetHeaderLength(rowBuilder);
			if (hasColumnLabelsRow)
				++this.firstDataRow;
			if (hasRowLabelsRow)
				this.firstDataRow = Math.Max(1, this.firstDataRow);
		}
		void CalculatePageCounts() {
			if (PivotTable.PageFields.Count == 0) {
				this.rowPageCount = 0;
				this.columnPageCount = 0;
			}
			else
				if (PivotTable.PageWrap == 0) {
					this.rowPageCount = PivotTable.PageFields.Count;
					this.columnPageCount = 1;
				}
				else {
					this.rowPageCount = Math.Min(PivotTable.PageWrap, PivotTable.PageFields.Count);
					this.columnPageCount = (int)Math.Ceiling((double)PivotTable.PageFields.Count / this.rowPageCount);
				}
			if (PivotTable.PageOverThenDown) {
				this.rowPageCount += this.columnPageCount;
				this.columnPageCount = this.rowPageCount - this.columnPageCount;
				this.rowPageCount -= this.columnPageCount;
			}
		}
		void CalculateNewRange(CellRange oldRange) {
			int dataColumnsCount = GetDataLength(columnBuilder);
			int dataRowsCount = GetDataLength(rowBuilder);
			int pivotWidth = this.firstDataColumn + Math.Max(dataColumnsCount, columnBuilder.LabelFieldIndices.Count);
			int pivotHeight = this.firstDataRow + dataRowsCount;
			CellPosition newTopLeft = oldRange.TopLeft;
			CellPosition newBottomRight;
			if (pivotWidth == 0 || pivotHeight == 0)
				newBottomRight = newTopLeft;
			else
				newBottomRight = new CellPosition(newTopLeft.Column + pivotWidth - 1, newTopLeft.Row + pivotHeight - 1);
			this.newRange = new CellRange(oldRange.Worksheet, newTopLeft, newBottomRight);
			if (this.rowPageCount > 0) {
				int topRow = this.newRange.TopRowIndex - this.rowPageCount - 1;
				if (topRow < 0)
					this.newRange.Resize(0, -topRow, 0, -topRow);
			}
			this.pageFieldsRange = PivotTableLocation.TryGetPageFieldsRange(newRange, columnPageCount, rowPageCount, PivotTable.PageFields.Count, PivotTable.PageOverThenDown);
			this.wholeRange = PivotTableLocation.GetWholeRange(newRange, pageFieldsRange, PivotTable);
		}
		int GetHeaderLength(PivotReportHeadersBuilder builder) {
			if (builder.Fields.Count == 0) {
				if (PivotTable.DataFields.Count == 1)
					return builder.OtherFieldsCount > 0 ? 1 : builder.DefaultHeaderLength;
				return 0;
			}
			int length = 1;
			for (int i = 0; i < builder.Fields.Count - 1; ++i)
				if (!builder.FieldIsCompact(i))
					++length;
			return length;
		}
		int GetDataLength(PivotReportHeadersBuilder builder) {
			if (PivotTable.DataFields.Count > 0 || builder.Fields.Count > 0)
				return builder.ItemsCount;
			return 0;
		}
		protected internal override void ExecuteCore() {
			Worksheet.ClearCellsNoShift(PivotTable.WholeRange);
			Worksheet.UnMergeCells(PivotTable.WholeRange, ErrorHandler);
			PivotTable.Location.FirstHeaderRow = firstHeaderRow;
			PivotTable.Location.FirstDataRow = firstDataRow;
			PivotTable.Location.FirstDataColumn = firstDataColumn;
			PivotTable.Location.RowPageCount = rowPageCount;
			PivotTable.Location.ColumnPageCount = columnPageCount;
			PivotTable.Location.SetRange(newRange, pageFieldsRange, wholeRange);
			PivotTable.CalculationInfo.InvalidateStyleFormatCache();
			PivotTable.LayoutCellCache.Invalidate();
			Worksheet.ClearCellsNoShift(PivotTable.WholeRange);
			Worksheet.UnMergeCells(PivotTable.WholeRange, ErrorHandler);
			Worksheet.RemoveArrayFormulasFromCollectionInsideRange(PivotTable.WholeRange);
			if (PivotTable.PageFields.Count == 0 && PivotTable.RowFields.Count == 0 && PivotTable.ColumnFields.Count == 0 && PivotTable.DataFields.Count == 0) { 
				FillDefault();
				return;
			}
			FillPageHeaders();
			FillPivotTableCaption();
			FillLabels(columnBuilder);
			FillLabels(rowBuilder);
			FillHeaders(columnBuilder);
			FillHeaders(rowBuilder);
			FillValues();
			FillFormats();
			if (PivotTable.UseAutoFormatting)
				Worksheet.TryBestFitColumn(PivotTable.WholeRange, Layout.Engine.ColumnBestFitMode.None);
		}
		void FillDefault() {
			CellRange range = PivotTable.Location.Range;
			int whiteIndex = ColorModelInfo.GetColorIndex(DocumentModel.Cache.ColorModelInfoCache, new ColorModelInfo() { ColorIndex = Palette.DefaultBackgroundColorIndex });
			int grayIndex = ColorModelInfo.GetColorIndex(DocumentModel.Cache.ColorModelInfoCache, Color.FromArgb(unchecked((int)0xFF999999)));
			int formatIndex = GetCellFormatIndex(grayIndex, grayIndex, 0, 0);
			Worksheet[range.LeftColumnIndex, range.TopRowIndex].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
			formatIndex = GetCellFormatIndex(grayIndex, whiteIndex, 0, grayIndex);
			Worksheet[range.LeftColumnIndex, range.BottomRowIndex].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
			formatIndex = GetCellFormatIndex(whiteIndex, whiteIndex, grayIndex, grayIndex);
			Worksheet[range.RightColumnIndex, range.BottomRowIndex].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
			formatIndex = GetCellFormatIndex(whiteIndex, grayIndex, grayIndex, 0);
			Worksheet[range.RightColumnIndex, range.TopRowIndex].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
			formatIndex = GetCellFormatIndex(grayIndex, whiteIndex, 0, 0);
			for (int i = range.TopRowIndex; ++i < range.BottomRowIndex; )
				Worksheet[range.LeftColumnIndex, i].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
			formatIndex = GetCellFormatIndex(whiteIndex, whiteIndex, grayIndex, 0);
			for (int i = range.TopRowIndex; ++i < range.BottomRowIndex; )
				Worksheet[range.RightColumnIndex, i].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
			formatIndex = GetCellFormatIndex(whiteIndex, grayIndex, 0, 0);
			for (int i = range.LeftColumnIndex; ++i < range.RightColumnIndex; )
				Worksheet[i, range.TopRowIndex].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
			formatIndex = GetCellFormatIndex(whiteIndex, whiteIndex, 0, grayIndex);
			for (int i = range.LeftColumnIndex; ++i < range.RightColumnIndex; )
				Worksheet[i, range.BottomRowIndex].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
			formatIndex = GetCellFormatIndex(whiteIndex, whiteIndex, 0, 0);
			for (int i = range.TopRowIndex; ++i < range.BottomRowIndex; )
				Worksheet[range.LeftColumnIndex + 1, i].ChangeFormatIndex(formatIndex, DocumentModelChangeActions.None);
		}
		int GetCellFormatIndex(int leftColorIndex, int topColorIndex, int rightColorIndex, int bottomColorIndex) {
			CellFormat cellFormat = DocumentModel.StyleSheet.DefaultCellFormat.Clone() as CellFormat;
			BorderInfo borderInfo = cellFormat.BorderInfo.Clone();
			if (leftColorIndex > 0) {
				borderInfo.LeftColorIndex = leftColorIndex;
				borderInfo.LeftLineStyle = DevExpress.Export.Xl.XlBorderLineStyle.Thin;
			}
			if (topColorIndex > 0) {
				borderInfo.TopColorIndex = topColorIndex;
				borderInfo.TopLineStyle = DevExpress.Export.Xl.XlBorderLineStyle.Thin;
			}
			if (rightColorIndex > 0) {
				borderInfo.RightColorIndex = rightColorIndex;
				borderInfo.RightLineStyle = DevExpress.Export.Xl.XlBorderLineStyle.Thin;
			}
			if (bottomColorIndex > 0) {
				borderInfo.BottomColorIndex = bottomColorIndex;
				borderInfo.BottomLineStyle = DevExpress.Export.Xl.XlBorderLineStyle.Thin;
			}
			int borderIndex = DocumentModel.Cache.BorderInfoCache.GetItemIndex(borderInfo);
			cellFormat.ApplyBorder = true;
			cellFormat.AssignBorderIndex(borderIndex);
			return DocumentModel.Cache.CellFormatCache.GetItemIndex(cellFormat);
		}
		public static void FillPageHeaders(PivotTable pivotTable, Action<int, int, int> fillPageHeader) {
			CellRangeBase pagesRange = pivotTable.Location.PageFieldsRange;
			if (pagesRange == null)
				return;
			int sheetStartColumn = pagesRange.TopLeft.Column;
			int sheetStartRow = pagesRange.TopLeft.Row;
			Func<int, int> getRelativeColumn;
			Func<int, int> getRelativeRow;
			if (pivotTable.PageOverThenDown) {
				getRelativeColumn = (i) => i % pivotTable.Location.ColumnPageCount;
				getRelativeRow = (i) => i / pivotTable.Location.ColumnPageCount;
			}
			else {
				getRelativeColumn = (i) => i / pivotTable.Location.RowPageCount;
				getRelativeRow = (i) => i % pivotTable.Location.RowPageCount;
			}
			for (int i = 0; i < pivotTable.PageFields.Count; ++i) {
				int column = sheetStartColumn + getRelativeColumn(i) * 3;
				int row = sheetStartRow + getRelativeRow(i);
				fillPageHeader(column, row, i);
			}
		}
		void FillPageHeaders() {
			FillPageHeaders(PivotTable, FillPageHeader);
		}
		void FillPageHeader(int sheetColumn, int sheetRow, int pageIndex) {
			PivotPageField pageField = PivotTable.PageFields[pageIndex];
			string header = PivotTable.GetFieldCaption(pageField.FieldIndex);
			SetPageHeader(sheetColumn, sheetRow, header);
			FormattedVariantValue filterValue = GetFilterCaption(pageField);
			SetPageValue(sheetColumn + 1, sheetRow, filterValue.Value, filterValue.NumberFormatId);
		}
		FormattedVariantValue GetFilterCaption(PivotPageField pageField) {
			if (pageField.ItemIndex >= 0)
				return GetItemHeaderCaption(PivotTable, pageField.FieldIndex, pageField.ItemIndex, DataContext);
			XtraSpreadsheetStringId captionId = XtraSpreadsheetStringId.PivotPageFieldAllItemsCaption;
			PivotField field = PivotTable.Fields[pageField.FieldIndex];
			if (field.MultipleItemSelectionAllowed) {
				bool hasHided = false;
				int singleItemIndex = -1;
				for (int i = 0; i < field.Items.DataItemsCount; ++i) {
					PivotItem item = field.Items[i];
					if (item.IsHidden || item.HasMissingValue && !field.ShowItemsWithNoData) {
						hasHided = true;
						continue;
					}
					singleItemIndex = singleItemIndex >= 0 ? int.MaxValue : i;
				}
				if (singleItemIndex >= 0 && singleItemIndex != int.MaxValue)
					return GetItemHeaderCaption(PivotTable, pageField.FieldIndex, singleItemIndex, DataContext);
				if (PivotTable.ShowMultipleLabel && hasHided)
					captionId = XtraSpreadsheetStringId.PivotPageFieldMultipleItemsCaption;
			}
			return new FormattedVariantValue(XtraSpreadsheetLocalizer.GetString(captionId), 0);
		}
		void FillValues() {
			if (PivotTable.DataFields.Count == 0)
				return;
			int sheetStartRow = PivotTable.Location.Range.TopLeft.Row + PivotTable.Location.FirstDataRow;
			int count = Math.Min(IndicesChecker.MaxRowIndex - sheetStartRow + 1, PivotTable.RowItems.Count);
			for (int i = 0; i < count; i++) {
				int sheetRowIndex = sheetStartRow + i;
				IPivotLayoutItem row = PivotTable.RowItems[i];
				key.AddRowKey(row, PivotTable);
				if (row.Type == PivotFieldItemType.Blank)
					continue;
				if (row.Type == PivotFieldItemType.Grand)
					key.ClearRowKey(PivotTable);
				if (row.Type != PivotFieldItemType.Data || key.RowKey.Count == PivotTable.RowFields.Count) {
					FillRow(sheetRowIndex, row.DataFieldIndex, row.Type); 
					continue;
				}
				int fieldIndex = PivotTable.RowFields[key.RowKey.Count - 1].FieldIndex;
				if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
					continue;
				PivotField field = PivotTable.Fields[fieldIndex];
				int itemIndex = key.RowKey[key.RowKey.Count - 1];
				PivotItem item = field.Items[itemIndex];
				if (item.HideDetails) {
					FillRow(sheetRowIndex, row.DataFieldIndex, row.Type); 
					continue;
				}
				if (field.SubtotalTop) {
					if (field.Subtotal == PivotFieldItemType.Blank)
						continue;
					item = field.Items[field.Items.Count - 2];
					bool hasOnlyOneSubtotal = item.ItemType == PivotFieldItemType.Data;
					if (hasOnlyOneSubtotal && (!PivotTable.RowFields.HasValuesField || PivotTable.RowFields.ValuesFieldIndex < key.RowKey.Count - 1)) { 
						item = field.Items[field.Items.Count - 1];
						FillRow(sheetRowIndex, row.DataFieldIndex, item.ItemType); 
					}
				}
			}
		}
		void FillRow(int sheetRowIndex, int rowDataFieldIndex, PivotFieldItemType rowType) {
			int sheetStartColumn = PivotTable.Location.Range.TopLeft.Column + PivotTable.Location.FirstDataColumn;
			int count = Math.Min(IndicesChecker.MaxColumnIndex - sheetStartColumn + 1, PivotTable.ColumnItems.Count);
			for (int i = 0; i < count; i++) {
				int sheetColumnIndex = sheetStartColumn + i;
				IPivotLayoutItem column = PivotTable.ColumnItems[i];
				key.AddColumnKey(column, PivotTable);
				if (column.Type == PivotFieldItemType.Grand)
					key.ClearColumnKey(PivotTable);
				FillCell(sheetRowIndex, sheetColumnIndex, rowType, column.Type, rowDataFieldIndex, column.DataFieldIndex);
			}
		}
		void FillCell(int sheetRowIndex, int sheetColumnIndex, PivotFieldItemType rowType, PivotFieldItemType columnType, int rowDataFieldIndex, int columnDataFieldIndex) {
			int dataFieldIndex = Math.Max(columnDataFieldIndex, rowDataFieldIndex);
			PivotDataField dataField = PivotTable.DataFields[dataFieldIndex];
			IShowValuesAsCalculator calculator = ShowValuesAsCalculatorFactory.GetCalculator(dataField.ShowDataAs);
			VariantValue value = calculator.Calculate(key, rowType, columnType, dataField, calcCache, PivotTable, dataFieldIndex);
			if (value.IsMissing)
				if (PivotTable.ShowMissing)
					value = PivotTable.MissingCaption;
				else
					value = 0;
			else
				if (value.IsError && PivotTable.ShowError)
					value = PivotTable.ErrorCaption;
			SetDataValue(sheetColumnIndex, sheetRowIndex, value, dataField.NumberFormatIndex);
		}
		void FillPivotTableCaption() {
			if (ShouldFillCaption(PivotTable))
				SetLabelValue(PivotTable.Location.Range.LeftColumnIndex, PivotTable.Location.Range.TopRowIndex, PivotTable.DataFields[0].Name);
		}
		public static bool ShouldFillCaption(PivotTable pivotTable) {
			return pivotTable.DataFields.Count == 1 && pivotTable.ColumnFields.Count > 0 && pivotTable.RowFields.Count > 0;
		}
		void FillLabels(PivotReportHeadersBuilder builder) {
			int sheetColumnIndex = PivotTable.Location.Range.TopLeft.Column + builder.FirstColumn;
			int sheetRowIndex = PivotTable.Location.Range.TopLeft.Row + builder.FirstRow;
			for (int i = 0; i < builder.LabelFieldIndices.Count; ++i) {
				int fieldIndex = builder.LabelFieldIndices[i];
				string caption;
				if (fieldIndex == PivotReportHeadersBuilder.DefaultLabelCaptionIndex)
					caption = builder.GetHeadersCaption();
				else
					if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
						caption = PivotTable.DataCaption;
					else
						caption = PivotTable.GetFieldCaption(fieldIndex);
				SetLabelValue(builder.GetLabelColumnIndex(i), builder.GetLabelRowIndex(), caption);
			}
		}
		void FillHeaders(PivotReportHeadersBuilder builder) {
			List<FormattedVariantValue> headers = new List<FormattedVariantValue>(builder.Fields.Count);
			int sheetColumnIndex = PivotTable.Location.Range.TopLeft.Column + builder.FirstColumn;
			int sheetRowIndex = PivotTable.Location.Range.TopLeft.Row + builder.FirstRow;
			if (builder.HasLabelsRow)
				sheetRowIndex += 1;
			int[] mergedCellsStartIndices = new int[builder.Fields.Count];
			for (int i = 0; i < mergedCellsStartIndices.Length; ++i)
				mergedCellsStartIndices[i] = -1;
			for (int i = 0; i < builder.ItemsCount; ++i) {
				IPivotLayoutItem item = builder.GetItem(i);
				FillRepeatedItems(builder, item, i, headers, mergedCellsStartIndices, sheetColumnIndex, sheetRowIndex);
				FillNewItems(builder, item, i, headers, mergedCellsStartIndices, sheetColumnIndex, sheetRowIndex);
			}
			if (builder.MergeItems)
				for (int j = 0; j < mergedCellsStartIndices.Length; ++j) {
					int startIndex = mergedCellsStartIndices[j];
					if (startIndex >= 0) {
						int row = builder.GetMergedRowIndex(sheetRowIndex, sheetColumnIndex, builder.ItemsCount - 1, 0);
						if (row - startIndex > 0) {
							int column = builder.GetMergedColumnIndex(sheetRowIndex, sheetColumnIndex, 0, j);
							MergeCell(column, startIndex, column, row, builder);
						}
					}
				}
		}
		void FillRepeatedItems(PivotReportHeadersBuilder builder, IPivotLayoutItem item, int itemIndex, List<FormattedVariantValue> headers, int[] mergedCellsStartIndices, int sheetColumnIndex, int sheetRowIndex) {
			int headerIndex = 0;
			for (int fieldRefIndex = 0; fieldRefIndex < item.RepeatedItemsCount; ++fieldRefIndex) {
				if (builder.CompactFormList[fieldRefIndex])
					continue;
				if (builder.FillDownLabelsList[fieldRefIndex])
					WriteCell(sheetColumnIndex, sheetRowIndex, itemIndex, headerIndex, headers[headerIndex], builder.IndentList[fieldRefIndex], builder.AllTabular, builder);
				else
					headers[headerIndex] = FormattedVariantValue.Empty;
				++headerIndex;
			}
			if (builder.MergeItems) {
				int prevRow = builder.GetMergedRowIndex(sheetRowIndex, sheetColumnIndex, itemIndex - 1, 0);
				for (int j = headerIndex; j < headers.Count; ++j) {
					int mergedCellStart = mergedCellsStartIndices[j];
					if (mergedCellStart > 0 && prevRow - mergedCellStart > 0) {
						int columnIndex = builder.GetMergedColumnIndex(sheetRowIndex, sheetColumnIndex, 0, j);
						MergeCell(columnIndex, mergedCellStart, columnIndex, prevRow, builder);
					}
					mergedCellsStartIndices[j] = -1;
				}
			}
			headers.RemoveRange(headerIndex, headers.Count - headerIndex);
		}
		void FillNewItems(PivotReportHeadersBuilder builder, IPivotLayoutItem item, int itemIndex, List<FormattedVariantValue> headers, int[] mergedCellsStartIndices, int sheetColumnIndex, int sheetRowIndex) {
			if (item.Type == PivotFieldItemType.Blank)
				return;
			if (item.PivotFieldItemIndices.Length == 0 && PivotTable.DataFields.Count == 1) {
				FormattedVariantValue header = GetValuesHeaderCaption(item);
				WriteCell(sheetColumnIndex, sheetRowIndex, itemIndex, headers.Count, header, NoIndent, false, builder);
				headers.Add(header);
			}
			else {
				HeaderCaptionGenerator getHeaderCaption;
				if (item.Type == PivotFieldItemType.Data)
					getHeaderCaption = GetHeaderCaption;
				else
					if (item.Type == PivotFieldItemType.Grand)
						getHeaderCaption = GetGrandTotalHeaderCaption;
					else
						getHeaderCaption = GetSubtotalHeaderCaption;
				int currentRow = builder.GetMergedRowIndex(sheetRowIndex, sheetColumnIndex, itemIndex, 0);
				for (int j = 0; j < item.PivotFieldItemIndices.Length; ++j) {
					FormattedVariantValue header;
					int fieldRefIndex = item.RepeatedItemsCount + j;
					if (item.PivotFieldItemIndices[j] == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex) {
						if (builder.CompactFormList[fieldRefIndex])
							continue;
						headers.Add(FormattedVariantValue.Empty);
					}
					else {
						header = getHeaderCaption(j, item, builder.Fields);
						WriteCell(sheetColumnIndex, sheetRowIndex, itemIndex, headers.Count, header, builder.IndentList[fieldRefIndex], builder.AllTabular, builder);
						headers.Add(header);
					}
					if (mergedCellsStartIndices[j] < 0)
						mergedCellsStartIndices[j] = currentRow;
				}
			}
			if (builder.MergeItems) {
				int leftColumn = builder.GetMergedColumnIndex(sheetRowIndex, sheetColumnIndex, 0, headers.Count - 1);
				int rightColumn = builder.RightColumn();
				if (rightColumn - leftColumn > 0) {
					int row = builder.GetMergedRowIndex(sheetRowIndex, sheetColumnIndex, itemIndex, 0);
					MergeCell(leftColumn, row, rightColumn, row, builder);
				}
			}
		}
		void MergeCell(int leftColumn, int topRow, int rightColumn, int bottomRow, PivotReportHeadersBuilder builder) {
			CellRange mergedRange = builder.GetCellRange(Worksheet, leftColumn, topRow, rightColumn, bottomRow);
			Worksheet.MergedCells.Add(mergedRange);
		}
		void WriteCell(int sheetColumnIndex, int sheetRowIndex, int itemIndex, int headerIndex, FormattedVariantValue value, int indent, bool allTabular, PivotReportHeadersBuilder builder) {
			if (object.ReferenceEquals(value, FormattedVariantValue.Empty))
				return;
			int columnIndex = builder.GetHeaderColumnIndex(sheetColumnIndex, itemIndex, headerIndex);
			int rowIndex = builder.GetHeaderRowIndex(sheetRowIndex, itemIndex, headerIndex);
			SetHeaderValue(columnIndex, rowIndex, value.Value, value.NumberFormatId, indent, value.NumberFormatId > 0, allTabular);
		}
		FormattedVariantValue GetValuesHeaderCaption(IPivotLayoutItem item) {
			int fieldIndex = item.DataFieldIndex;
			PivotDataField dataField = PivotTable.DataFields[fieldIndex];
			return new FormattedVariantValue(dataField.Name, 0);
		}
		FormattedVariantValue GetHeaderCaption(int i, IPivotLayoutItem item, PivotTableColumnRowFieldIndices indices) {
			int fieldIndex = indices[item.RepeatedItemsCount + i].FieldIndex;
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return GetValuesHeaderCaption(item);
			int itemIndex = item.PivotFieldItemIndices[i];
			if (itemIndex == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex)
				return FormattedVariantValue.Empty;
			return GetItemHeaderCaption(PivotTable, fieldIndex, itemIndex, DocumentModel.DataContext);
		}
		public static FormattedVariantValue GetItemHeaderCaption(PivotTable pivotTable, int fieldIndex, int itemIndex, WorkbookDataContext DataContext) {
			IPivotCacheField cacheField = pivotTable.Cache.CacheFields[fieldIndex];
			PivotField field = pivotTable.Fields[fieldIndex];
			PivotItem item = field.Items[itemIndex];
			VariantValue headerValue;
			int numberFormatIndex = field.NumberFormatIndex;
			if (!string.IsNullOrEmpty(item.ItemUserCaption))
				headerValue = item.ItemUserCaption;
			else {
				int sharedItemIndex = item.ItemIndex;
				IPivotCacheRecordValue value = pivotTable.Cache.CacheFields.GetValue(sharedItemIndex, fieldIndex);
				headerValue = value.ToVariantValue(cacheField, DataContext);
				headerValue = VariantValueToText.ToText(headerValue, DataContext);
				if (numberFormatIndex == 0)
					numberFormatIndex = value.ValueType == PivotCacheRecordValueType.DateTime ? 14 : 0;
			}
			return new FormattedVariantValue(headerValue, numberFormatIndex);
		}
		FormattedVariantValue GetSubtotalHeaderCaption(int i, IPivotLayoutItem item, PivotTableColumnRowFieldIndices indices) {
			FormattedVariantValue customSubtotalCaption = GetCustomSubtotalCaption(i, item, indices);
			if (!customSubtotalCaption.IsEmpty)
				return customSubtotalCaption;
			FormattedVariantValue header = GetHeaderCaption(i, item, indices);
			if (header.Value.IsEmpty)
				return header;
			VariantValue headerValue = header.Value;
			if (header.NumberFormatId > 0) {
				NumberFormat numberFormat = DocumentModel.Cache.NumberFormatCache[header.NumberFormatId];
				NumberFormatResult numberFormatResult = numberFormat.Format(headerValue, DocumentModel.DataContext);
				headerValue = numberFormatResult.Text;
			}
			string headerCaption = VariantValueToText.ToStringCore(headerValue, DataContext);
			string subtotalCaption = XtraSpreadsheetLocalizer.GetString(GetSubtotalCaptionId(item.Type));
			if (indices.HasValuesField) {
				PivotDataField dataField = PivotTable.DataFields[item.DataFieldIndex];
				if (item.Type == PivotFieldItemType.DefaultValue)
					subtotalCaption = dataField.Name;
				else {
					subtotalCaption += " " + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_PartOfCustomName);
					subtotalCaption += " " + PivotTable.GetFieldCaption(dataField.FieldIndex);
				}
			}
			int fieldIndex = indices[item.RepeatedItemsCount + i].FieldIndex;
			PivotField field = PivotTable.Fields[fieldIndex];
			return new FormattedVariantValue(headerCaption + " " + subtotalCaption, field.NumberFormatIndex);
		}
		FormattedVariantValue GetCustomSubtotalCaption(int i, IPivotLayoutItem item, PivotTableColumnRowFieldIndices indices) {
			int referenceIndex = item.RepeatedItemsCount + i;
			int fieldIndex = indices[referenceIndex].FieldIndex;
			if (fieldIndex != PivotTable.ValuesFieldFakeIndex) {
				PivotField field = PivotTable.Fields[fieldIndex];
				string caption = field.SubtotalCaption;
				if (!string.IsNullOrEmpty(caption) && referenceIndex != indices.LastKeyFieldIndex && !indices.HasValuesField &&
					Object.ReferenceEquals(columnBuilder.Fields, indices) && field.Subtotal == PivotFieldItemType.DefaultValue)
					return new FormattedVariantValue(caption, field.NumberFormatIndex);
			}
			return FormattedVariantValue.Empty;
		}
		FormattedVariantValue GetGrandTotalHeaderCaption(int i, IPivotLayoutItem item, PivotTableColumnRowFieldIndices indices) {
			string headerCaption;
			if (indices.HasValuesField) {
				string dataFieldName = PivotTable.DataFields[item.DataFieldIndex].Name;
				headerCaption = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionDefault) + " " + dataFieldName;
			}
			else if (!string.IsNullOrEmpty(PivotTable.GrandTotalCaption))
					headerCaption = PivotTable.GrandTotalCaption;
				else
					headerCaption = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotGrandTotal);
			int numberFormatIndex = 0;
			int fieldIndex = indices[0].FieldIndex;
			if (fieldIndex != PivotTable.ValuesFieldFakeIndex) {
				PivotField field = PivotTable.Fields[fieldIndex];
				numberFormatIndex = field.NumberFormatIndex;
			}
			return new FormattedVariantValue(headerCaption, numberFormatIndex);
		}
		public static XtraSpreadsheetStringId GetSubtotalCaptionId(PivotFieldItemType subtotal) {
			switch (subtotal) {
				case PivotFieldItemType.DefaultValue:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionDefault;
				case PivotFieldItemType.Count:
				case PivotFieldItemType.CountA:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCount;
				case PivotFieldItemType.Sum:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionSum;
				case PivotFieldItemType.Avg:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionAverage;
				case PivotFieldItemType.Max:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMax;
				case PivotFieldItemType.Min:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMin;
				case PivotFieldItemType.Product:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionProduct;
				case PivotFieldItemType.StdDev:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionStdDev;
				case PivotFieldItemType.StdDevP:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionStdDevp;
				case PivotFieldItemType.Var:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionVar;
				case PivotFieldItemType.VarP:
					return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionVarp;
				default:
					Exceptions.ThrowInternalException();
					return XtraSpreadsheetStringId.Msg_ErrorInternalError;
			}
		}
		ICell SetCellValueNoChecks(int columnIndex, int rowIndex, VariantValue value) {
			ICell cell = Worksheet[columnIndex, rowIndex];
			cell.ReplaceValueByValueCore(value);
			return cell;
		}
		ICell SetCellValueCore(int columnIndex, int rowIndex, VariantValue value) {
			if (columnIndex > IndicesChecker.MaxColumnIndex || rowIndex > IndicesChecker.MaxRowIndex)
				return null;
			return SetCellValueNoChecks(columnIndex, rowIndex, value);
		}
		void SetPageHeader(int columnIndex, int rowIndex, VariantValue value) {
			ICell cell = SetCellValueCore(columnIndex, rowIndex, value);
			if (cell == null)
				return;
			cell.ChangeFormatIndex(DocumentModel.StyleSheet.DefaultCellFormatIndex, DocumentModelChangeActions.None);
		}
		void SetPageValue(int columnIndex, int rowIndex, VariantValue value, int numberFormatIndex) {
			ICell cell = SetCellValueCore(columnIndex, rowIndex, value);
			if (cell == null)
				return;
			CellFormat format = (CellFormat)DocumentModel.StyleSheet.DefaultCellFormat.Clone();
			if (numberFormatIndex > 0) {
				format.ApplyNumberFormat = true;
				format.AssignNumberFormatIndex(numberFormatIndex);
			}
			if (value.IsNumeric) {
				CellAlignmentInfo alignment = format.AlignmentInfo.Clone();
				alignment.HorizontalAlignment = DevExpress.Export.Xl.XlHorizontalAlignment.Left;
				alignment.Indent = 0;
				int alignmentIndex = DocumentModel.Cache.CellAlignmentInfoCache.GetItemIndex(alignment);
				format.ApplyAlignment = true;
				format.AssignAlignmentIndex(alignmentIndex);
			}
			int cellFormatIndex = DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
			cell.ChangeFormatIndex(cellFormatIndex, DocumentModelChangeActions.None);
		}
		void SetLabelValue(int columnIndex, int rowIndex, VariantValue value) {
			ICell cell = SetCellValueCore(columnIndex, rowIndex, value);
			if (cell == null)
				return;
			if (PivotTable.MergeItem) {
				CellFormat format = (CellFormat)DocumentModel.StyleSheet.DefaultCellFormat.Clone();
				CellAlignmentInfo alignment = format.AlignmentInfo.Clone();
				alignment.HorizontalAlignment = DevExpress.Export.Xl.XlHorizontalAlignment.Center;
				int alignmentIndex = DocumentModel.Cache.CellAlignmentInfoCache.GetItemIndex(alignment);
				format.ApplyAlignment = true;
				format.AssignAlignmentIndex(alignmentIndex);
				int cellFormatIndex = DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
				cell.ChangeFormatIndex(cellFormatIndex, DocumentModelChangeActions.None);
			}
			else
				cell.ChangeFormatIndex(DocumentModel.StyleSheet.DefaultCellFormatIndex, DocumentModelChangeActions.None);
		}
		void SetDataValue(int columnIndex, int rowIndex, VariantValue value, int numberFormatIndex) {
			ICell cell = SetCellValueNoChecks(columnIndex, rowIndex, value);
			CellFormat format = (CellFormat)DocumentModel.StyleSheet.DefaultCellFormat.Clone();
			format.ApplyNumberFormat = true;
			format.AssignNumberFormatIndex(numberFormatIndex);
			int cellFormatIndex = DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
			cell.ChangeFormatIndex(cellFormatIndex, DocumentModelChangeActions.None);
		}
		void SetHeaderValue(int columnIndex, int rowIndex, VariantValue value, int numberFormatIndex, int indent, bool applyNumberFormat, bool allTabular) {
			ICell cell = SetCellValueCore(columnIndex, rowIndex, value);
			if (cell == null)
				return;
			CellFormat format = (CellFormat)DocumentModel.StyleSheet.DefaultCellFormat.Clone();
			if (applyNumberFormat) {
				format.ApplyNumberFormat = true;
				format.AssignNumberFormatIndex(numberFormatIndex);
			}
			if (indent != NoIndent) {
				CellAlignmentInfo alignment = format.AlignmentInfo.Clone();
				alignment.HorizontalAlignment = DevExpress.Export.Xl.XlHorizontalAlignment.Left;
				alignment.Indent = (byte)indent;
				int alignmentIndex = DocumentModel.Cache.CellAlignmentInfoCache.GetItemIndex(alignment);
				format.ApplyAlignment = true;
				format.AssignAlignmentIndex(alignmentIndex);
			}
			else
				if (PivotTable.MergeItem) {
					CellAlignmentInfo alignment = format.AlignmentInfo.Clone();
					alignment.HorizontalAlignment = DevExpress.Export.Xl.XlHorizontalAlignment.Center;
					if (allTabular) {
						alignment.VerticalAlignment = DevExpress.Export.Xl.XlVerticalAlignment.Center;
						alignment.WrapText = true;
					}
					int alignmentIndex = DocumentModel.Cache.CellAlignmentInfoCache.GetItemIndex(alignment);
					format.ApplyAlignment = true;
					format.AssignAlignmentIndex(alignmentIndex);
				}
			int cellFormatIndex = DocumentModel.Cache.CellFormatCache.GetItemIndex(format);
			cell.ChangeFormatIndex(cellFormatIndex, DocumentModelChangeActions.None);
		}
		void FillFormats() {
		}
	}
	#endregion
	#region VariantValueToText
	public static class VariantValueToText {
		public static VariantValue ToText(VariantValue value, WorkbookDataContext context) {
			switch (value.Type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotEmptyValue);
				case VariantValueType.Error:
					return CellErrorFactory.GetErrorName(value.ErrorValue, context); 
				case VariantValueType.Boolean:
					if (value.BooleanValue)
						return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotTrueValue);
					else
						return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotFalseValue);
				default:
					return value;
			}
		}
		public static string ToString(VariantValue value, WorkbookDataContext context) {
			value = ToText(value, context);
			return ToStringCore(value, context);
		}
		public static string ToStringCore(VariantValue value, WorkbookDataContext context) {
			return value.ToText(context).InlineTextValue;
		}
	}
	#endregion
	#region PivotReportHeadersBuilder
	public abstract class PivotReportHeadersBuilder {
		PivotTable pivotTable;
		protected PivotReportHeadersBuilder(PivotTable pivotTable) {
			this.pivotTable = pivotTable;
		}
		public PivotTable PivotTable { get { return pivotTable; } }
		public abstract bool HideValues { get; }
		public abstract bool IsColumn { get; } 
		public abstract int FirstColumn { get; }
		public abstract int FirstRow { get; }
		public abstract int ItemsCount { get; }
		public abstract PivotTableColumnRowFieldIndices Fields { get; }
		public abstract int OtherFieldsCount { get; }
		public abstract int DefaultHeaderLength { get; }
		public abstract bool FieldIsCompact(int fieldReferenceIndex);
		public abstract IPivotLayoutItem GetItem(int index);
		public abstract string GetHeadersCaption();
		public abstract int GetMergedRowIndex(int sheetRowIndex, int sheetColumnIndex, int itemIndex, int headerIndex);
		public abstract int GetMergedColumnIndex(int sheetRowIndex, int sheetColumnIndex, int itemIndex, int headerIndex);
		public abstract int RightColumn();
		public abstract CellRange GetCellRange(Worksheet sheet, int leftColumn, int topRow, int rightColumn, int bottomRow);
		public abstract CellPosition GetHeaderPosition(int column, int row);
		protected abstract bool FieldIsCompactCore(int fieldReferenceIndex);
		public abstract int GetHeaderColumnIndex(int sheetColumnIndex, int itemIndex, int headerIndex);
		public abstract int GetHeaderRowIndex(int sheetRowIndex, int itemIndex, int headerIndex);
		public int GetLabelColumnIndex(int labelIndex) {
			return PivotTable.Location.Range.LeftColumnIndex + FirstColumn + labelIndex;
		}
		public int GetLabelRowIndex() {
			return PivotTable.Location.Range.TopRowIndex + FirstRow;
		}
		public const int DefaultLabelCaptionIndex = -1;
		bool mergeItems;
		bool hasCompact;
		bool allTabular;
		bool indentOverflow;
		bool[] fillDownLabelsList;
		bool[] compactFormList;
		int[] indentList;
		List<int> labelFieldIndices;
		bool hasLabelsRow;
		public bool MergeItems { get { return mergeItems; } }
		public bool HasCompact { get { return hasCompact; } }
		public bool AllTabular { get { return allTabular; } }
		public bool IndentOverflow { get { return indentOverflow; } }
		public bool[] FillDownLabelsList { get { return fillDownLabelsList; } }
		public bool[] CompactFormList { get { return compactFormList; } }
		public int[] IndentList { get { return indentList; } }
		public List<int> LabelFieldIndices { get { return labelFieldIndices; } }
		public bool HasLabelsRow { get { return hasLabelsRow; } }
		public void PrepareCache() {
			int count = Fields.Count;
			int indent = 0;
			hasCompact = IsColumn ? false : pivotTable.Compact;
			allTabular = true;
			fillDownLabelsList = new bool[count];
			compactFormList = new bool[count];
			indentList = new int[count];
			for (int i = 0; i < count; ++i) {
				indentList[i] = indent;
				bool fillDownLabels = false;
				bool compactForm = FieldIsCompactCore(i);
				if (compactForm) {
					indent += pivotTable.Indent;
					if (indent > PivotRefreshDataOnWorksheetCommand.MaxIndent && i < count - 1) {
						indentOverflow = true;
						compactForm = false;
						int fieldIndex = Fields[i].FieldIndex;
						if (fieldIndex != PivotTable.ValuesFieldFakeIndex) {
							PivotField field = pivotTable.Fields[fieldIndex];
							if (field.FillDownLabels)
								fillDownLabels = true;
							if (field.Outline)
								allTabular = false;
						}
						indent = 0;
					}
					else {
						allTabular = false;
						hasCompact = true;
					}
				}
				else {
					int fieldIndex = Fields[i].FieldIndex;
					if (fieldIndex != PivotTable.ValuesFieldFakeIndex) {
						PivotField field = pivotTable.Fields[fieldIndex];
						if (field.FillDownLabels)
							fillDownLabels = true;
						if (field.Outline)
							allTabular = false;
					}
					indent = 0;
				}
				compactFormList[i] = compactForm;
				fillDownLabelsList[i] = fillDownLabels;
			}
			if (!hasCompact || allTabular)
				for (int i = 0; i < count; ++i)
					indentList[i] = PivotRefreshDataOnWorksheetCommand.NoIndent;
			if (IsColumn)
				allTabular = true;
			mergeItems = allTabular && pivotTable.MergeItem && Fields.Count > 0;
			labelFieldIndices = new List<int>();
			if (!pivotTable.ShowHeaders || Fields.Count == 0)
				hasLabelsRow = IsColumn ? PivotRefreshDataOnWorksheetCommand.ShouldFillCaption(pivotTable) : true;
			else {
				hasLabelsRow = true;
				if (Fields.KeyIndicesCount == 0)
					if (!HideValues)
						labelFieldIndices.Add(PivotTable.ValuesFieldFakeIndex);
					else
						hasLabelsRow = false;
				else {
					bool hasCompactLabel = false;
					for (int i = 0; i < Fields.Count; ++i) {
						int fieldIndex = Fields[i].FieldIndex;
						labelFieldIndices.Add(fieldIndex);
						if (pivotTable.FieldIsCompact(fieldIndex)) {
							hasCompactLabel = true;
							if (pivotTable.FieldIsOutline(fieldIndex))
								for (++i; i < Fields.Count; ++i)
									if (!CompactFormList[i])
										break;
						}
					}
					if (hasCompactLabel || pivotTable.CompactData) {
						if (IsColumn) {
							labelFieldIndices.Clear();
							labelFieldIndices.Add(DefaultLabelCaptionIndex);
						}
						else
							if (labelFieldIndices.Count == 0)
								labelFieldIndices.Add(DefaultLabelCaptionIndex);
							else
								labelFieldIndices[0] = DefaultLabelCaptionIndex;
					}
				}
			}
		}
	}
	#endregion
	#region PivotReportRowHeadersBuilder
	public class PivotReportRowHeadersBuilder : PivotReportHeadersBuilder {
		public PivotReportRowHeadersBuilder(PivotTable pivotTable)
			: base(pivotTable) {
		}
		public override bool HideValues { get { return false; } }
		public override bool IsColumn { get { return false; } }
		public override int FirstColumn { get { return 0; } }
		public override int FirstRow { get { return PivotTable.Location.FirstDataRow - 1; } }
		public override int ItemsCount { get { return PivotTable.RowItems.Count; } }
		public override PivotTableColumnRowFieldIndices Fields { get { return PivotTable.RowFields; } }
		public override int OtherFieldsCount { get { return PivotTable.ColumnFields.Count; } }
		public override int DefaultHeaderLength { get { return 0; } }
		public override bool FieldIsCompact(int fieldReferenceIndex) {
			return CompactFormList[fieldReferenceIndex];
		}
		protected override bool FieldIsCompactCore(int fieldReferenceIndex) {
			int fieldIndex = Fields[fieldReferenceIndex];
			return PivotTable.FieldIsCompactForm(fieldIndex);
		}
		public override IPivotLayoutItem GetItem(int index) {
			return PivotTable.RowItems[index];
		}
		public override string GetHeadersCaption() {
			string header = PivotTable.RowHeaderCaption;
			if (string.IsNullOrEmpty(header))
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotDefaultRowHeader);
			return header;
		}
		public override int GetHeaderColumnIndex(int sheetColumnIndex, int itemIndex, int headerIndex) {
			return sheetColumnIndex + headerIndex;
		}
		public override int GetHeaderRowIndex(int sheetRowIndex, int itemIndex, int headerIndex) {
			return sheetRowIndex + itemIndex;
		}
		public override int GetMergedColumnIndex(int sheetRowIndex, int sheetColumnIndex, int itemIndex, int headerIndex) {
			return GetHeaderColumnIndex(sheetColumnIndex, itemIndex, headerIndex);
		}
		public override int GetMergedRowIndex(int sheetRowIndex, int sheetColumnIndex, int itemIndex, int headerIndex) {
			return GetHeaderRowIndex(sheetRowIndex, itemIndex, headerIndex);
		}
		public override int RightColumn() {
			return PivotTable.Location.Range.LeftColumnIndex + PivotTable.Location.FirstDataColumn - 1;
		}
		public override CellRange GetCellRange(Worksheet sheet, int leftColumn, int topRow, int rightColumn, int bottomRow) {
			return new CellRange(sheet, new CellPosition(leftColumn, topRow), new CellPosition(rightColumn, bottomRow));
		}
		public override CellPosition GetHeaderPosition(int column, int row) {
			return new CellPosition(PivotTable.Location.Range.LeftColumnIndex + column, PivotTable.Location.Range.TopRowIndex + PivotTable.Location.FirstDataRow + row);
		}
	}
	#endregion
	#region PivotReportColumnHeadersBuilder
	public class PivotReportColumnHeadersBuilder : PivotReportHeadersBuilder {
		public PivotReportColumnHeadersBuilder(PivotTable pivotTable)
			: base(pivotTable) {
		}
		public override bool HideValues { get { return !PivotTable.ShowValuesRow; } }
		public override bool IsColumn { get { return true; } }
		public override int FirstColumn { get { return PivotTable.Location.FirstDataColumn; } }
		public override int FirstRow { get { return 0; } }
		public override int ItemsCount { get { return PivotTable.ColumnItems.Count; } }
		public override PivotTableColumnRowFieldIndices Fields { get { return PivotTable.ColumnFields; } }
		public override int OtherFieldsCount { get { return PivotTable.RowFields.Count; } }
		public override int DefaultHeaderLength { get { return 1; } }
		public override bool FieldIsCompact(int index) {
			return false;
		}
		protected override bool FieldIsCompactCore(int fieldReferenceIndex) {
			return false;
		}
		public override IPivotLayoutItem GetItem(int index) {
			return PivotTable.ColumnItems[index];
		}
		public override string GetHeadersCaption() {
			string header = PivotTable.ColHeaderCaption;
			if (string.IsNullOrEmpty(header))
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PivotDefaultColumnHeader);
			return header;
		}
		public override int GetHeaderColumnIndex(int sheetColumnIndex, int itemIndex, int headerIndex) {
			return sheetColumnIndex + itemIndex;
		}
		public override int GetHeaderRowIndex(int sheetRowIndex, int itemIndex, int headerIndex) {
			return sheetRowIndex + headerIndex;
		}
		public override int GetMergedColumnIndex(int sheetRowIndex, int sheetColumnIndex, int itemIndex, int headerIndex) {
			return GetHeaderRowIndex(sheetRowIndex, itemIndex, headerIndex);
		}
		public override int GetMergedRowIndex(int sheetRowIndex, int sheetColumnIndex, int itemIndex, int headerIndex) {
			return GetHeaderColumnIndex(sheetColumnIndex, itemIndex, headerIndex);
		}
		public override int RightColumn() {
			return PivotTable.Location.Range.TopRowIndex + PivotTable.Location.FirstDataRow - 1;
		}
		public override CellRange GetCellRange(Worksheet sheet, int leftColumn, int topRow, int rightColumn, int bottomRow) {
			return new CellRange(sheet, new CellPosition(topRow, leftColumn), new CellPosition(bottomRow, rightColumn));
		}
		public override CellPosition GetHeaderPosition(int column, int row) {
			int firstHeaderRow = PivotTable.Location.Range.TopRowIndex + PivotTable.Location.FirstHeaderRow; 
			return new CellPosition(PivotTable.Location.Range.LeftColumnIndex + PivotTable.Location.FirstDataColumn + row, firstHeaderRow + column);
		}
	}
	#endregion
}
