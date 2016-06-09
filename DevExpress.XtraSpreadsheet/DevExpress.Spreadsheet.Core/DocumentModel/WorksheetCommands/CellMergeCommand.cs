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
using DevExpress.Export.Xl;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	[Flags]
	public enum MergeCellsMode {
		Default = 0,
		ByRows = 1,
		ByColumns = 2,
	}
	#region CellMergeCommand
	public class CellMergeCommand : CellUnmergeCommand {
		readonly bool expandWithIntersectedMergedCells;
		readonly MergeCellsMode mode;
		public CellMergeCommand(Worksheet worksheet, CellRangeBase range, bool expandWithIntersectedMergedCells, MergeCellsMode mode, IErrorHandler errorHandler)
			: base(worksheet, range, errorHandler) {
			Debug.Assert(object.ReferenceEquals(worksheet, range.Worksheet));
			this.expandWithIntersectedMergedCells = expandWithIntersectedMergedCells;
			this.mode = mode;
		}
		protected internal override bool Validate() {
			IModelErrorInfo error = ValidateCore();
			return HandleError(error);
		}
		protected internal IModelErrorInfo ValidateCore() {
			if (Worksheet.Tables.ContainsItemsInRange(Range, true))
				return new ModelErrorInfo(ModelErrorType.TableOverlap);
			if (Worksheet.ArrayFormulaRanges.CheckMultiCellArrayFormulasInRange(Range))
				return new ModelErrorInfo(ModelErrorType.ErrorChangingPartOfAnArray);
			if (Worksheet.PivotTables.ContainsItemsInRange(Range, true))
				return new ModelErrorInfo(ModelErrorType.PivotTableCanNotBeChanged);
			return null;
		}
		protected internal override void ExecuteCore() {
			if (Range.CellCount == 1)
				return;
			CellRangeBase range = Range;
			if (expandWithIntersectedMergedCells) {
				range = SheetViewSelection.ExpandRangeToMergedCellSize(range);
				PerformUnmerging(range);
			}
			MergeRangeBase(range);
		}
		void MergeRangeBase(CellRangeBase range) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)range;
				foreach (CellRangeBase innerCellRange in union.InnerCellRanges)
					MergeCells((CellRange)innerCellRange);
			}
			else
				MergeCells((CellRange)range);
		}
		void MergeCells(CellRange range) {
			switch (mode) {
				case MergeCellsMode.Default:
					MergeCellsCore(range);
					break;
				case MergeCellsMode.ByColumns:
					MergeCellsByColumns(range);
					break;
				case MergeCellsMode.ByRows:
					MergeCellsByRows(range);
					break;
			}
		}
		void MergeCellsByColumns(CellRange range) {
			int startRowIndex = range.TopLeft.Row;
			int endRowIndex = range.BottomRight.Row;
			int startColumnIndex = range.TopLeft.Column;
			int endColumnIndex = range.BottomRight.Column;
			ICellTable sheet = range.Worksheet;
			for (int i = startColumnIndex; i <= endColumnIndex; i++) {
				CellRange mergeRange = new CellRange(sheet, new CellPosition(i, startRowIndex), new CellPosition(i, endRowIndex));
				MergeCellsCore(mergeRange);
			}
		}
		void MergeCellsByRows(CellRange range) {
			int startRowIndex = range.TopLeft.Row;
			int endRowIndex = range.BottomRight.Row;
			int startColumnIndex = range.TopLeft.Column;
			int endColumnIndex = range.BottomRight.Column;
			ICellTable sheet = range.Worksheet;
			for (int i = startRowIndex; i <= endRowIndex; i++) {
				CellRange mergeRange = new CellRange(sheet, new CellPosition(startColumnIndex, i), new CellPosition(endColumnIndex, i));
				MergeCellsCore(mergeRange);
			}
		}
		void MergeCellsCore(CellRange range) {
			ICell contentCell = GetContentCell(range);
			ICell hostCell = Worksheet[range.TopLeft];
			MergedCellOuterBordersFormatInfo bordersFormatInfo;
			if (contentCell != null)
				bordersFormatInfo = new MergedCellOuterBordersFormatInfo(contentCell.FormatInfo, range);
			else
				bordersFormatInfo = new MergedCellOuterBordersFormatInfo(hostCell.FormatInfo, range);
			ApplyContentToHostCell(contentCell, hostCell);
			foreach (Model.ICellBase cellInfo in range.GetExistingCellsEnumerable()) {
				ICell currentCell = cellInfo as ICell;
				if (currentCell != hostCell)
					currentCell.ClearContent();
				bordersFormatInfo.ApplyBorders(currentCell);
			}
			ProcessHyperlinksBeforeMergingRange(hostCell, range);
			ProcessDataValidations(range);
			ProcessComments(hostCell, range);
			Worksheet.MergedCells.Add(range);
			Worksheet.Workbook.InternalAPI.OnAfterMergedCellsInserted(Worksheet, range);
		}
		void ProcessHyperlinksBeforeMergingRange(ICell hostCell, CellRange range) {
			for (int i = Worksheet.Hyperlinks.Count - 1; i > -1; --i) {
				CellRange linkRange = Worksheet.Hyperlinks[i].Range;
				if (linkRange.Intersects(range) && !linkRange.ContainsCell(hostCell))
					Worksheet.RemoveHyperlinkAt(i, false);
			}
		}
		void ProcessDataValidations(CellRange range) {
			if (range.CellCount == 1)
				return;
			CellRange topLeft = new CellRange(Worksheet, range.TopLeft, range.TopLeft);
			DataValidationClearCommand command = new DataValidationClearCommand(Worksheet, ErrorHandler, range.ExcludeRange(topLeft));
			command.Execute();
		}
		void ProcessComments(ICell hostCell, CellRange range) {
			if (range.CellCount == 1)
				return;
			List<Comment> comments = Worksheet.GetComments(range);
			foreach (Comment comment in comments) {
				if (!comment.Reference.EqualsPosition(hostCell.Position))
					Worksheet.RemoveComment(comment);
			}
		}
		ICell GetContentCell(CellRange rangeToMerge) {
			foreach (CellBase cellInfo in rangeToMerge.GetExistingCellsEnumerable())
				if (!cellInfo.Value.IsEmpty)
					return (ICell)cellInfo;
			return null;
		}
		void ApplyContentToHostCell(ICell contentCell, ICell hostCell) {
			if (contentCell == null)
				return;
			if (contentCell != hostCell) {
				var operation = new DevExpress.XtraSpreadsheet.Model.CopyOperation.CopyCellOperation(
					ModelPasteSpecialFlags.All & ~ModelPasteSpecialFlags.Borders,
					contentCell,
					hostCell);
				operation.DisabledHistory = false;
				operation.Execute();
			}
		}
	}
	#endregion
	#region CellUnmergeCommand
	public class CellUnmergeCommand : ErrorHandledWorksheetCommand {
		readonly CellRangeBase range;
		public CellUnmergeCommand(Worksheet worksheet, CellRangeBase range, IErrorHandler errorHandler)
			: base(worksheet, errorHandler) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
		}
		public CellRangeBase Range { get { return range; } }
		protected internal override void ExecuteCore() {
			PerformUnmerging(Range);
		}
		protected void PerformUnmerging(CellRangeBase range) {
			List<CellRange> includedMergings = Worksheet.MergedCells.GetMergedCellRangesIntersectsRange(range);
			foreach (CellRange mergedRange in includedMergings)
				UnMergeSingleRangeWithoutChecks(mergedRange);
		}
		public static void UnMergeSingleRangeWithoutChecks(CellRange singleMergedCellRange) {
			Worksheet sheet = singleMergedCellRange.Worksheet as Worksheet;
			sheet.Workbook.InternalAPI.OnBeforeMergedCellsRemoved(sheet, singleMergedCellRange);
			sheet.MergedCells.Remove(singleMergedCellRange);
		}
	}
	#endregion
	public class SideBorderInfo {
		public XlBorderLineStyle LineStyle { get; set; }
		public int ColorIndex { get; set; }
	}
	public class MergedCellOuterBordersFormatInfo {
		readonly CellFormat baseFormat;
		readonly CellRange range;
		SideBorderInfo innerBorder;
		SideBorderInfo leftBorder;
		SideBorderInfo topBorder;
		SideBorderInfo rightBorder;
		SideBorderInfo bottomBorder;
		int[] formatIndices = new int[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1, };
		public MergedCellOuterBordersFormatInfo(CellFormat baseFormat, CellRange range) {
			Guard.ArgumentNotNull(baseFormat, "baseFormat");
			Guard.ArgumentNotNull(range, "range");
			this.baseFormat = baseFormat;
			this.range = range;
		}
		#region Properties
		SideBorderInfo InnerBorder {
			get {
				if (innerBorder == null)
					innerBorder = CalculateInnerBorder();
				return innerBorder;
			}
		}
		SideBorderInfo LeftBorder {
			get {
				if (leftBorder == null)
					leftBorder = CalculateLeftBorder(range);
				return leftBorder;
			}
		}
		SideBorderInfo TopBorder {
			get {
				if (topBorder == null)
					topBorder = CalculateTopBorder(range);
				return topBorder;
			}
		}
		SideBorderInfo RightBorder {
			get {
				if (rightBorder == null)
					rightBorder = CalculateRightBorder(range);
				return rightBorder;
			}
		}
		SideBorderInfo BottomBorder {
			get {
				if (bottomBorder == null)
					bottomBorder = CalculateBottomBorder(range);
				return bottomBorder;
			}
		}
		#endregion
		public void ApplyBorders(ICell cell) {
			int index = CalculateCellIndex(cell);
			int formatIndex = formatIndices[index];
			if (formatIndex < 0) {
				formatIndex = CalculateFormatIndex(index);
				formatIndices[index] = formatIndex;
			}
			cell.ChangeFormatIndex(formatIndex, cell.GetBatchUpdateChangeActions());
		}
		int CalculateCellIndex(ICell cell) {
			return CalculateCellRowOffset(cell) + CalculateCellColumnOffset(cell);
		}
		int CalculateCellColumnOffset(ICell cell) {
			if (cell.ColumnIndex == range.TopLeft.Column)
				return 0;
			else if (cell.ColumnIndex == range.BottomRight.Column)
				return 2;
			else
				return 1;
		}
		int CalculateCellRowOffset(ICell cell) {
			if (cell.RowIndex == range.TopLeft.Row)
				return 0;
			else if (cell.RowIndex == range.BottomRight.Row)
				return 6;
			else
				return 3;
		}
		int CalculateFormatIndex(int index) {
			CellFormat result = new CellFormat(baseFormat.DocumentModel);
			result.CopyFrom(baseFormat);
			if (range.Height == 1)
				CalculateFormatIndexHorizontalVector(index, result);
			else if (range.Width == 1)
				CalculateFormatIndexVerticalVector(index, result);
			else
				CalculateFormatIndexCore(index, result);
			return baseFormat.DocumentModel.Cache.CellFormatCache.AddItem(result);
		}
		void CalculateFormatIndexHorizontalVector(int index, CellFormat result) {
			ApplyBorder(result, BorderSideAccessor.Top, TopBorder);
			ApplyBorder(result, BorderSideAccessor.Bottom, BottomBorder);
			if (index == 0) {
				ApplyBorder(result, BorderSideAccessor.Left, LeftBorder);
				ApplyBorder(result, BorderSideAccessor.Right, InnerBorder);
			}
			else if (index == 1) {
				ApplyBorder(result, BorderSideAccessor.Left, InnerBorder);
				ApplyBorder(result, BorderSideAccessor.Right, InnerBorder);
			}
			else {
				ApplyBorder(result, BorderSideAccessor.Left, InnerBorder);
				ApplyBorder(result, BorderSideAccessor.Right, RightBorder);
			}
		}
		void CalculateFormatIndexVerticalVector(int index, CellFormat result) {
			ApplyBorder(result, BorderSideAccessor.Left, LeftBorder);
			ApplyBorder(result, BorderSideAccessor.Right, RightBorder);
			if (index == 0) {
				ApplyBorder(result, BorderSideAccessor.Top, TopBorder);
				ApplyBorder(result, BorderSideAccessor.Bottom, InnerBorder);
			}
			else if (index == 3) {
				ApplyBorder(result, BorderSideAccessor.Top, InnerBorder);
				ApplyBorder(result, BorderSideAccessor.Bottom, InnerBorder);
			}
			else {
				ApplyBorder(result, BorderSideAccessor.Top, InnerBorder);
				ApplyBorder(result, BorderSideAccessor.Bottom, BottomBorder);
			}
		}
		void CalculateFormatIndexCore(int index, CellFormat result) {
			switch (index) {
				default:
				case 0:
					ApplyBorder(result, BorderSideAccessor.Top, TopBorder);
					ApplyBorder(result, BorderSideAccessor.Left, LeftBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Right, InnerBorder);
					break;
				case 1:
					ApplyBorder(result, BorderSideAccessor.Top, TopBorder);
					ApplyBorder(result, BorderSideAccessor.Left, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Right, InnerBorder);
					break;
				case 2:
					ApplyBorder(result, BorderSideAccessor.Top, TopBorder);
					ApplyBorder(result, BorderSideAccessor.Left, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Right, RightBorder);
					break;
				case 3:
					ApplyBorder(result, BorderSideAccessor.Top, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Left, LeftBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Right, InnerBorder);
					break;
				case 4:
					ApplyBorder(result, BorderSideAccessor.Top, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Left, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Right, InnerBorder);
					break;
				case 5:
					ApplyBorder(result, BorderSideAccessor.Top, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Left, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Right, RightBorder);
					break;
				case 6:
					ApplyBorder(result, BorderSideAccessor.Top, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Left, LeftBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, BottomBorder);
					ApplyBorder(result, BorderSideAccessor.Right, InnerBorder);
					break;
				case 7:
					ApplyBorder(result, BorderSideAccessor.Top, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Left, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, BottomBorder);
					ApplyBorder(result, BorderSideAccessor.Right, InnerBorder);
					break;
				case 8:
					ApplyBorder(result, BorderSideAccessor.Top, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Left, InnerBorder);
					ApplyBorder(result, BorderSideAccessor.Bottom, BottomBorder);
					ApplyBorder(result, BorderSideAccessor.Right, RightBorder);
					break;
			}
		}
		void ApplyBorder(CellFormat result, BorderSideAccessor accessor, SideBorderInfo value) {
			accessor.SetLineColorIndex(result.Border, value.ColorIndex);
			accessor.SetLineStyle(result.Border, value.LineStyle);
		}
		SideBorderInfo CalculateHorizontalBorder(CellRange primaryCellRange, int rowOffset, int limitRow, BorderSideAccessor primaryAccessor, BorderSideAccessor secondaryAccessor) {
			Worksheet sheet = (Worksheet)primaryCellRange.Worksheet;
			IEnumerator<ICell> primaryCells = HorizontalBordersCalculator.GetCellsSimple(primaryCellRange.TopLeft.Row, sheet, primaryCellRange.TopLeft.Column, primaryCellRange.BottomRight.Column, sheet.ActiveView.DefaultGridBorder);
			if (primaryCells == null)
				return new SideBorderInfo();
			if (primaryCellRange.TopLeft.Row == limitRow) {
				IEnumerator<BorderLineBox> enumerator = new SingleBorderLineEnumerator(SpreadsheetDirection.Horizontal, primaryCells, primaryAccessor, sheet.MaxColumnCount, sheet.MaxColumnCount);
				return CalculateBorderSideCore(enumerator, primaryCellRange.TopLeft.Column, primaryCellRange.BottomRight.Column);
			}
			else {
				CellRange secondaryCellRange = new CellRange(sheet, new CellPosition(primaryCellRange.TopLeft.Column, primaryCellRange.TopLeft.Row + rowOffset), new CellPosition(primaryCellRange.BottomRight.Column, primaryCellRange.TopLeft.Row + rowOffset));
				IEnumerator<ICell> secondaryCells = HorizontalBordersCalculator.GetCellsSimple(secondaryCellRange.TopLeft.Row, sheet, secondaryCellRange.TopLeft.Column, secondaryCellRange.BottomRight.Column, sheet.ActiveView.DefaultGridBorder);
				if (secondaryCells == null) {
					IEnumerator<BorderLineBox> enumerator = new SingleBorderLineEnumerator(SpreadsheetDirection.Horizontal, primaryCells, primaryAccessor, sheet.MaxColumnCount, sheet.MaxColumnCount);
					return CalculateBorderSideCore(enumerator, primaryCellRange.TopLeft.Column, primaryCellRange.BottomRight.Column);
				}
				else {
					IEnumerator<BorderLineBox> enumerator = new BorderLineEnumerator(SpreadsheetDirection.Horizontal, primaryCells, primaryAccessor, secondaryCells, secondaryAccessor, sheet.MaxColumnCount, sheet.MaxColumnCount);
					return CalculateBorderSideCore(enumerator, primaryCellRange.TopLeft.Column, primaryCellRange.BottomRight.Column);
				}
			}
		}
		SideBorderInfo CalculateVerticalBorder(CellRange primaryCellRange, int columnOffset, int limitColumn, BorderSideAccessor primaryAccessor, BorderSideAccessor secondaryAccessor) {
			Worksheet sheet = (Worksheet)primaryCellRange.Worksheet;
			using (IEnumerator<ICell> primaryCells = VerticalBordersCalculator.GetCellsSimple(primaryCellRange.TopLeft.Column, sheet, primaryCellRange.TopLeft.Row, primaryCellRange.BottomRight.Row, sheet.ActiveView.DefaultGridBorder).Enumerator) {
				if (primaryCells == null)
					return new SideBorderInfo();
				if (primaryCellRange.TopLeft.Column == limitColumn) {
					IEnumerator<BorderLineBox> enumerator = new SingleBorderLineEnumerator(SpreadsheetDirection.Vertical, primaryCells, primaryAccessor, sheet.MaxRowCount, sheet.MaxRowCount);
					return CalculateBorderSideCore(enumerator, primaryCellRange.TopLeft.Row, primaryCellRange.BottomRight.Row);
				}
				else {
					CellRange secondaryCellRange = new CellRange(sheet, new CellPosition(primaryCellRange.TopLeft.Column + columnOffset, primaryCellRange.TopLeft.Row), new CellPosition(primaryCellRange.TopLeft.Column + columnOffset, primaryCellRange.BottomRight.Row));
					using (IEnumerator<ICell> secondaryCells = VerticalBordersCalculator.GetCellsSimple(secondaryCellRange.TopLeft.Column, sheet, secondaryCellRange.TopLeft.Row, secondaryCellRange.BottomRight.Row, sheet.ActiveView.DefaultGridBorder).Enumerator) {
						if (secondaryCells == null) {
							IEnumerator<BorderLineBox> enumerator = new SingleBorderLineEnumerator(SpreadsheetDirection.Vertical, primaryCells, primaryAccessor, sheet.MaxRowCount, sheet.MaxRowCount);
							return CalculateBorderSideCore(enumerator, primaryCellRange.TopLeft.Row, primaryCellRange.BottomRight.Row);
						}
						else {
							IEnumerator<BorderLineBox> enumerator = new BorderLineEnumerator(SpreadsheetDirection.Vertical, primaryCells, primaryAccessor, secondaryCells, secondaryAccessor, sheet.MaxRowCount, sheet.MaxRowCount);
							return CalculateBorderSideCore(enumerator, primaryCellRange.TopLeft.Row, primaryCellRange.BottomRight.Row);
						}
					}
				}
			}
		}
		SideBorderInfo CalculateTopBorder(CellRange range) {
			Worksheet sheet = (Worksheet)range.Worksheet;
			CellRange primaryCellRange = new CellRange(sheet, range.TopLeft, new CellPosition(range.BottomRight.Column, range.TopLeft.Row));
			return CalculateHorizontalBorder(primaryCellRange, -1, 0, BorderSideAccessor.Top, BorderSideAccessor.Bottom);
		}
		SideBorderInfo CalculateBottomBorder(CellRange range) {
			Worksheet sheet = (Worksheet)range.Worksheet;
			CellRange primaryCellRange = new CellRange(sheet, new CellPosition(range.TopLeft.Column, range.BottomRight.Row), range.BottomRight);
			return CalculateHorizontalBorder(primaryCellRange, 1, sheet.MaxRowCount - 1, BorderSideAccessor.Bottom, BorderSideAccessor.Top);
		}
		SideBorderInfo CalculateLeftBorder(CellRange range) {
			Worksheet sheet = (Worksheet)range.Worksheet;
			CellRange primaryCellRange = new CellRange(sheet, range.TopLeft, new CellPosition(range.TopLeft.Column, range.BottomRight.Row));
			return CalculateVerticalBorder(primaryCellRange, -1, 0, BorderSideAccessor.Left, BorderSideAccessor.Right);
		}
		SideBorderInfo CalculateRightBorder(CellRange range) {
			Worksheet sheet = (Worksheet)range.Worksheet;
			CellRange primaryCellRange = new CellRange(sheet, new CellPosition(range.BottomRight.Column, range.TopLeft.Row), range.BottomRight);
			return CalculateVerticalBorder(primaryCellRange, 1, sheet.MaxColumnCount - 1, BorderSideAccessor.Right, BorderSideAccessor.Left);
		}
		SideBorderInfo CalculateInnerBorder() {
			return new SideBorderInfo();
		}
		SideBorderInfo CalculateBorderSideCore(IEnumerator<BorderLineBox> borders, int firstIndex, int lastIndex) {
			SideBorderInfo result = new SideBorderInfo();
			int index = firstIndex;
			foreach (BorderLineBox box in new Enumerable<BorderLineBox>(borders)) {
				if (box.ColorIndex != result.ColorIndex || box.LineStyle != result.LineStyle) {
					if (index == firstIndex) {
						result.ColorIndex = box.ColorIndex;
						result.LineStyle = box.LineStyle;
					}
					else
						return new SideBorderInfo();
				}
				index++;
			}
			if (index != lastIndex + 1)
				return new SideBorderInfo();
			return result;
		}
	}
}
