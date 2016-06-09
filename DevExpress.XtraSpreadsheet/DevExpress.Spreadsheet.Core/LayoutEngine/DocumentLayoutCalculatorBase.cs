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
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	public class DocumentLayoutCalculatorBase {
		readonly DocumentLayout layout;
		readonly Worksheet sheet;
		readonly RowVerticalBorderBreaks verticalBorderBreaks;
		float scaleFactor;
		public DocumentLayoutCalculatorBase(DocumentLayout layout, Worksheet sheet) {
			Guard.ArgumentNotNull(layout, "layout");
			Guard.ArgumentNotNull(sheet, "sheet");
			this.layout = layout;
			this.sheet = sheet;
			this.verticalBorderBreaks = new RowVerticalBorderBreaks();
		}
		public DocumentLayout Layout { get { return layout; } }
		public Worksheet Sheet { get { return sheet; } }
		public RowVerticalBorderBreaks VerticalBorderBreaks { get { return verticalBorderBreaks; } }
		public virtual void CalculateLayout(CellRange layoutRange) {
			PreliminaryPage preliminaryPage = CalculatePreliminaryLayout(layoutRange);
			if (preliminaryPage != null) {
				FinalDocumentLayoutCalculatorBase calculator = CreateFinalLayoutCalculator(preliminaryPage);
				int headerWidth = 0;
				int headerHeight = 0;
				if (sheet.PrintSetup.PrintHeadings) {
					IColumnWidthCalculationService columnWidthCalculator = sheet.Workbook.GetService<IColumnWidthCalculationService>();
					headerWidth = CalculateHeaderWidth(preliminaryPage.GridRows.ActualLast.ModelIndex);
					headerHeight = columnWidthCalculator.CalculateDefaultRowHeight(sheet);
				}
				calculator.GeneratePages(headerWidth, headerHeight);
			}
		}
		protected internal virtual FinalDocumentLayoutCalculatorBase CreateFinalLayoutCalculator(PreliminaryPage preliminaryPage) {
			return new FinalDocumentLayoutCalculatorBase(Layout, Sheet, preliminaryPage, verticalBorderBreaks);
		}
		protected PreliminaryGridInfo GetPreliminaryInfo(PageGridCalculator gridCalculator, CellRange printRange, CellRange totalRange, float scaleFactor, bool forceCreateNonEmptyGrid, CellRange previousLongTextBoxesRange) {
			PreliminaryGridInfo gridInfo = new PreliminaryGridInfo();
			gridInfo.PrintRange = printRange;
			gridInfo.MergedCells = Sheet.MergedCells.GetMergedCellRangesIntersectsRange(totalRange);
			gridInfo.DrawingObjects = Sheet.DrawingObjectsByZOrderCollections.GetDrawingObjects(totalRange);
			gridInfo.Comments = Sheet.Comments.InnerList;
			gridInfo.ExtendedPrintRange = CalculateExtendedRange(gridInfo.MergedCells, gridInfo.DrawingObjects, gridInfo.PrintRange, previousLongTextBoxesRange);
			gridInfo.TotalRowGrid = gridCalculator.CreateTotalRowGrid(gridInfo.ExtendedPrintRange, scaleFactor, forceCreateNonEmptyGrid);
			gridInfo.TotalRowGrid.DetectActualRange(printRange.TopLeft.Row, printRange.BottomRight.Row);
			gridInfo.TotalColumnGrid = CreateTotalColumnGrid(gridCalculator, gridInfo, scaleFactor, forceCreateNonEmptyGrid);
			gridInfo.TotalColumnGrid.DetectActualRange(printRange.TopLeft.Column, printRange.BottomRight.Column);
			return gridInfo;
		}
		protected internal PreliminaryPage CalculatePreliminaryLayout(CellRange printRange) {
			if (printRange == null)
				return null;
			PageGridCalculator gridCalculator = CreatePageGridCalculator(Rectangle.Empty);
			scaleFactor = CalculateScalingFactor(printRange, gridCalculator);
			PreliminaryGridInfo gridInfo = GetPreliminaryInfo(gridCalculator, printRange, printRange, scaleFactor, false, null);
			PreliminaryPage page = GenerateContent(gridInfo);
			if(page != null)
				page.ScaleFactor = scaleFactor;
			return page;
		}
		protected virtual int CalculateHeaderWidth(int lastRow) {
			int result = Math.Max(0, this.Sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnits(this.Sheet.Workbook.ViewOptions.RowHeaderWidth, DocumentModel.DpiY));
			if (result == 0)
				result = (int)(Sheet.Workbook.MaxDigitWidth * Math.Max((lastRow + 1).ToString().Length + 1, 3));
			return result;
		}
		protected virtual PageGrid CreateTotalColumnGrid(PageGridCalculator gridCalculator, PreliminaryGridInfo gridInfo, float scaleFactor, bool forceCreateNonEmptyGrid) {
			return gridCalculator.CreateTotalColumnGrid(gridInfo.ExtendedPrintRange, scaleFactor, forceCreateNonEmptyGrid);
		}
		protected virtual float CalculateScalingFactor(CellRange printRange, PageGridCalculator gridCalculator) {
			PrintSetup printSetup = sheet.PrintSetup;
			if (!printSetup.FitToPage)
				return printSetup.Scale / 100f;
			PageGrid totalColumnGrid = gridCalculator.CreateTotalColumnGrid(printRange, 1);
			PageGrid totalRowGrid = gridCalculator.CreateTotalRowGrid(printRange, 1);
			int maxWidth = totalColumnGrid.Last.Far - totalColumnGrid.First.Near;
			int maxHeight = totalRowGrid.Last.Far - totalRowGrid.First.Near;
			Size pageSize = CalculateActualPageSize();
			float widthScalingFactor = Math.Min((float)pageSize.Width / (float)maxWidth, 1);
			float heightScalingFactor = Math.Min((float)pageSize.Height / (float)maxHeight, 1);
			if (printSetup.FitToWidth != 1 || printSetup.FitToHeight != 1) {		 
				if (printSetup.FitToWidth > 1)
					widthScalingFactor = Math.Min(widthScalingFactor * printSetup.FitToWidth, 1);
				if (printSetup.FitToHeight > 1)
					heightScalingFactor = Math.Min(heightScalingFactor * printSetup.FitToHeight, 1);
				if (printSetup.FitToHeight == 0) 
					return widthScalingFactor;
				if (printSetup.FitToWidth == 0)  
					return heightScalingFactor;
			}
			return Math.Min(widthScalingFactor, heightScalingFactor);
		}
		Size CalculateActualPageSize() {
			PrintSetup printSetup = sheet.PrintSetup;
			Size size = PaperSizeCalculator.CalculatePaperSize(printSetup.PaperKind);
			size = new Size(sheet.Workbook.LayoutUnitConverter.TwipsToLayoutUnits(size.Width), sheet.Workbook.LayoutUnitConverter.TwipsToLayoutUnits(size.Height));
			if (printSetup.Orientation == ModelPageOrientation.Landscape) {
				int value = size.Width;
				size.Width = size.Height;
				size.Height = value;
			}
			Margins margins = sheet.Margins;
			size.Width -= sheet.Workbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(margins.Left + margins.Right);
			size.Height -= sheet.Workbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(margins.Top + margins.Bottom);
			return size;
		}
		protected internal virtual PageGridCalculator CreatePageGridCalculator(Rectangle pageClientBounds) {
			return new PageGridCalculator(sheet, pageClientBounds);
		}
		protected PreliminaryPage GenerateContent(PreliminaryGridInfo gridInfo) {
			return GenerateContent(gridInfo, false, null);
		}
		protected PreliminaryPage GenerateContent(PreliminaryGridInfo gridInfo, bool hasRightPage, PreliminaryPage previousPage) {
			PageGrid columnGrid = gridInfo.TotalColumnGrid;
			PageGrid rowGrid = gridInfo.TotalRowGrid;
			int rowCount = rowGrid.Count;
			int columnCount = columnGrid.Count;
			if (rowCount == 0 || columnCount == 0)
				return null;
			PreliminaryPage page = CreatePreliminaryPage(columnGrid, rowGrid, hasRightPage);
			if (previousPage != null && previousPage.LongTextBoxes.Count > 0)
				page.IntegratePreviousPageLongTextBoxes(previousPage);
			GeneratePageContent(page, gridInfo);
			Rectangle bounds = new Rectangle(0, 0, columnGrid.Last.Far - columnGrid.First.Near, rowGrid.Last.Far - rowGrid.First.Near);
			page.Bounds = bounds;
			page.ClientBounds = bounds;
			return page;
		}
		void GeneratePageContent(PreliminaryPage page, PreliminaryGridInfo gridInfo) {
			CellRange range = gridInfo.PrintRange;
			List<CellRange> mergedCells = gridInfo.MergedCells;
			page.DrawingObjects.AddRange(gridInfo.DrawingObjects);
			page.Comments.AddRange(gridInfo.Comments);
			if (mergedCells.Count <= 0)
				GeneratePageWithoutMergedCells(page, range);
			else {
				List<ComplexCellTextBox> mergedCellsLayout = GeneratePageMergedCells(page, range, mergedCells);
				GeneratePageIgnoreMergedCells(page, range, gridInfo.MergedCellTree, mergedCellsLayout);
			}
		}
		protected CellRange CalculateExtendedRange(List<CellRange> mergedCells, List<IDrawingObject> drawingObjects, CellRange printRange, CellRange previousLongTextBoxesRange) {
			CellRange mergedCellsExtendedRange = CalculateMergedCellsExtendedRange(mergedCells, printRange, previousLongTextBoxesRange);
			return CalculatePicturesExtendedRange(drawingObjects, mergedCellsExtendedRange);
		}
		protected virtual CellRange CalculateMergedCellsExtendedRange(List<CellRange> mergedCells, CellRange printRange, CellRange previousLongTextBoxesRange) {
			CellPosition nearPosition = printRange.TopLeft;
			CellPosition farPosition = printRange.BottomRight;
			if (previousLongTextBoxesRange != null)
				nearPosition = CellPosition.UnionPosition(nearPosition, previousLongTextBoxesRange.TopLeft, true);
			int count = mergedCells.Count;
			for (int i = 0; i < count; i++) {
				CellRange mergedCell = mergedCells[i];
				nearPosition = CellPosition.UnionPosition(nearPosition, mergedCell.TopLeft, true);
				farPosition = CellPosition.UnionPosition(farPosition, mergedCell.BottomRight, false);
			}
			return new CellRange(printRange.Worksheet, nearPosition, farPosition);
		}
		CellRange CalculatePicturesExtendedRange(List<IDrawingObject> drawingObjects, CellRange printRange) {
			CellPosition nearPosition = printRange.TopLeft;
			CellPosition farPosition = printRange.BottomRight;
			int count = drawingObjects.Count;
			for (int i = 0; i < count; i++) {
				AnchorPoint from = drawingObjects[i].From;
				AnchorPoint to = drawingObjects[i].To;
				if (from.Col < nearPosition.Column)
					nearPosition = new CellPosition(from.Col, nearPosition.Row);
				if (from.Row < nearPosition.Row)
					nearPosition = new CellPosition(nearPosition.Column, from.Row);
				if (to.Col > farPosition.Column)
					farPosition = new CellPosition(to.Col, farPosition.Row);
				if (to.Row > farPosition.Row)
					farPosition = new CellPosition(farPosition.Column, to.Row);
			}
			return new CellRange(printRange.Worksheet, nearPosition, farPosition);
		}
		void GeneratePageIgnoreMergedCells(PreliminaryPage page, CellRange range, CellRangesCachedRTree mergedCells, List<ComplexCellTextBox> mergedCellsLayout) {
			RowCellsController rowController = new RowCellsController(page);
			rowController.RowModelIndex = -1;
			foreach (ICellBase info in range.GetLayoutVisibleCellsEnumerable()) { 
				ICell cell = (ICell)info;
				if (cell.RowIndex != rowController.RowModelIndex) {
					rowController.ApplyComplexCellsClip(mergedCellsLayout);
					FinishRow(rowController);
					rowController = new RowCellsController(page);
					rowController.RowModelIndex = cell.RowIndex;
					rowController.GridRowIndex = page.GridRows.TryCalculateIndex(cell.RowIndex);
					if (rowController.GridRowIndex < 0)
						continue;
				}
				CellRange mergedCellRange = mergedCells.Search(cell.ColumnIndex, cell.RowIndex);
				if (mergedCellRange == null && cell.ShouldUseInLayout)
					rowController.AppendSingleCellBox(cell);
			}
			rowController.ApplyComplexCellsClip(mergedCellsLayout);
			FinishRow(rowController);
			FinishPage(rowController);
		}
		void GeneratePageWithoutMergedCells(PreliminaryPage page, CellRange range) {
			RowCellsController rowController = new RowCellsController(page);
			rowController.RowModelIndex = -1;
			foreach (ICellBase info in GetLayoutVisibleCellsEnumerable(range)) { 
				ICell cell = info as ICell;
				if (cell.RowIndex != rowController.RowModelIndex) {
					FinishRow(rowController);
					rowController = new RowCellsController(page);
					rowController.RowModelIndex = cell.RowIndex;
					rowController.GridRowIndex = page.GridRows.CalculateExactOrNearItem(cell.RowIndex);
					List<ComplexCellTextBox> deletedBoxes = GetDeletedLongTextBoxes(page, rowController, verticalBorderBreaks);
					foreach (ComplexCellTextBox box in deletedBoxes) {
						page.ComplexBoxes.Remove(box);
						page.RowComplexBoxes[rowController.GridRowIndex].Remove(box);
					}
				}
				if (cell.ShouldUseInLayout)
					rowController.AppendSingleCellBox(cell);
			}
			FinishRow(rowController);
			FinishPage(rowController);
		}
		protected virtual IEnumerable<ICellBase> GetLayoutVisibleCellsEnumerable(CellRange range) {
			return range.GetLayoutVisibleCellsEnumerable(true);
		}
		List<ComplexCellTextBox> GeneratePageMergedCells(PreliminaryPage page, CellRange pageRange, List<CellRange> mergedCells) {
			List<ComplexCellTextBox> result = new List<ComplexCellTextBox>();
			int count = mergedCells.Count;
			for (int i = 0; i < count; i++) {
				ComplexCellTextBox box = TryCreateMergedCellBox(page, mergedCells[i]);
				if (box != null) {
					page.ComplexBoxes.Add(box);
					if (!page.RowComplexBoxes.ContainsKey(box.ClipFirstRowIndex))
						page.RowComplexBoxes.Add(box.ClipFirstRowIndex, new ComplexCellTextBoxList());
					page.RowComplexBoxes[box.ClipFirstRowIndex].Add(box);
					result.Add(box);
				}
			}
			return result;
		}
		void FinishRow(RowCellsController controller) {
			Page page = controller.Page;
			List<SingleCellTextBox> boxes = controller.SingleBoxes;
			int count = boxes.Count;
			for (int i = count - 1; i >= 0; i--) {
				SingleCellTextBox iBox = boxes[i];
				if (!page.RowComplexBoxes.ContainsKey(iBox.GridRowIndex))
					continue;
				IList<ComplexCellTextBox> complexBoxes = page.RowComplexBoxes[iBox.GridRowIndex];
				for (int j = complexBoxes.Count - 1; j >= 0; j--) {
					ComplexCellTextBox jBox = complexBoxes[j];
					if (jBox.ClipLastColumnIndex >= iBox.GridColumnIndex)
						if (jBox.ClipFirstColumnIndex <= iBox.GridColumnIndex) {
							if (iBox.GridColumnIndex != page.GridColumns.ActualFirstIndex)
								jBox.ClipLastColumnIndex = iBox.GridColumnIndex - 1;
							else {
								page.ComplexBoxes.Remove(jBox);
								complexBoxes.Remove(jBox);
							}
						}
				}
			}
			controller.CalculateVerticalBorderBreaks(this, verticalBorderBreaks);
		}
		List<ComplexCellTextBox> GetDeletedLongTextBoxes(Page page, RowCellsController rowController, RowVerticalBorderBreaks verticalBorderBreaks) {
			List<ComplexCellTextBox> result = new List<ComplexCellTextBox>();
			if (verticalBorderBreaks.ContainsKey(rowController.RowModelIndex)) {
				List<int> borderBreaks = verticalBorderBreaks.GetValue(rowController.RowModelIndex);
				int firstIndex = page.GridColumns.ActualFirst.Index;
				if (rowController.GridRowIndex > -1 && borderBreaks.Contains(firstIndex) && page.RowComplexBoxes.ContainsKey(rowController.GridRowIndex))
					foreach (ComplexCellTextBox box in page.RowComplexBoxes[rowController.GridRowIndex])
						if (box.ClipFirstRowIndex == box.ClipLastRowIndex && box.ClipFirstRowIndex == rowController.GridRowIndex && box.ClipFirstColumnIndex < firstIndex)
							result.Add(box);
			}
			return result;
		}
		void FinishPage(RowCellsController controller) {
		}
		PreliminaryPage CreatePreliminaryPage(PageGrid gridColumns, PageGrid gridRows, bool hasRightPage) {
			PreliminaryPage page = new PreliminaryPage(layout, sheet, gridColumns, gridRows, hasRightPage);
			Rectangle bounds = new Rectangle(0, 0, int.MaxValue / 4, int.MaxValue / 4);
			page.Bounds = bounds;
			page.ClientBounds = bounds;
			page.Index = 0;
			return page;
		}
		ComplexCellTextBox TryCreateMergedCellBox(PreliminaryPage page, CellRange mergedCellRange) {
			ICell cell = page.Sheet.MergedCells.TryGetCell(mergedCellRange);
			if (cell == null || (!cell.IsVisible() && !IsMergedRangeContainsComment(mergedCellRange, page.Comments)))
				return null; 
			ComplexCellTextBox box = new ComplexCellTextBox(cell); 
			if (!CalculateClipBounds(box, mergedCellRange, page))
				return null;
			Rectangle bounds = CalculateMergedCellBounds(box, page);
			bounds.X += page.ClientLeft;
			bounds.Y += page.ClientTop;
			box.Bounds = bounds;
			return box;
		}
		bool IsMergedRangeContainsComment(CellRange mergedCellRange, List<Comment> comments) {
			foreach (Comment comment in comments) {
				CellPosition position = comment.Reference;
				if (mergedCellRange.ContainsCell(position.Column, position.Row))
					return true;
			}
			return false;
		}
		bool CalculateClipBounds(ComplexCellTextBox box, CellRange mergedCellRange, Page page) {
			CellPosition topLeftPosition = mergedCellRange.TopLeft;
			CellPosition bottomRightPosition = mergedCellRange.BottomRight;
			box.ClipFirstColumnIndex = page.GridColumns.CalculateExactOrFarItem(topLeftPosition.Column);
			if (box.ClipFirstColumnIndex < 0 || page.GridColumns[box.ClipFirstColumnIndex].ModelIndex > bottomRightPosition.Column)
				return false;
			box.ClipFirstRowIndex = page.GridRows.CalculateExactOrFarItem(topLeftPosition.Row);
			if (box.ClipFirstRowIndex < 0 || page.GridRows[box.ClipFirstRowIndex].ModelIndex > bottomRightPosition.Row)
				return false;
			if (bottomRightPosition.Column >= page.GridColumns.Last.ModelIndex)
				box.ClipLastColumnIndex = page.GridColumns.Count - 1;
			else
				box.ClipLastColumnIndex = Math.Max(box.ClipFirstColumnIndex, page.GridColumns.CalculateExactOrNearItem(bottomRightPosition.Column));
			if (bottomRightPosition.Row >= page.GridRows.Last.ModelIndex)
				box.ClipLastRowIndex = page.GridRows.Count - 1;
			else
				box.ClipLastRowIndex = Math.Max(box.ClipFirstRowIndex, page.GridRows.CalculateExactOrNearItem(bottomRightPosition.Row));
			return true;
		}
		Rectangle CalculateMergedCellBounds(ComplexCellTextBox box, Page page) {
			int left = page.GridColumns[box.ClipFirstColumnIndex].Near;
			int top = page.GridRows[box.ClipFirstRowIndex].Near;
			int right = page.GridColumns[box.ClipLastColumnIndex].Far;
			int bottom = page.GridRows[box.ClipLastRowIndex].Far;
			return new Rectangle(left, top, right - left, bottom - top);
		}
		protected internal virtual int MeasureText(ICell cell, string text) {
			CellFormatStringMeasurer measurer = new CellFormatStringMeasurer(cell);
			return (int)(measurer.MeasureStringWidth(text) * scaleFactor);
		}
		public int CalculateCellSingleLineTextWidth(ICell cell, Page page, Rectangle availableTextBounds) {
			NumberFormatParameters parameters = new NumberFormatParameters();
			parameters.Measurer = new CellFormatStringMeasurer(cell);
			parameters.AvailableSpaceWidth = (int)Math.Round(availableTextBounds.Width / page.ScaleFactor);
			NumberFormatResult formatResult = cell.GetFormatResult(parameters);
			string text = formatResult.Text;
			if (String.IsNullOrEmpty(text))
				return 0;
			if (!cell.ActualAlignment.WrapText)
				text = SingleLineTextHelper.GetSingleLine(text);
			return MeasureText(cell, text);
		}
	}
	public static class SingleLineTextHelper {
		public static string GetSingleLine(string text) {
			string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.None);
			if (lines.Length > 1) {
				StringBuilder sb = new StringBuilder();
				bool needSpace = false;
				foreach (string line in lines) {
					if (!string.IsNullOrEmpty(line)) {
						if (needSpace)
							sb.Append(" ");
						sb.Append(line);
						needSpace = !line.EndsWith(" ");
					}
				}
				text = sb.ToString();
			}
			return text;
		}
	}
	#region CellFormatStringMeasurer
	public class CellFormatStringMeasurer : INumberFormatStringMeasurer {
		readonly ICell cell;
		FontInfo fontInfo;
		public CellFormatStringMeasurer(ICell cell) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
		}
		FontInfo FontInfo {
			get {
				if (fontInfo == null)
					fontInfo = cell.ActualFont.GetFontInfo();
				return fontInfo;
			}
		}
		public float MaxDigitWidth { get { return FontInfo.MaxDigitWidth; } }
		public int MeasureStringWidth(string text) {
			return cell.Worksheet.Workbook.FontCache.Measurer.MeasureString(text, FontInfo).Width;
		}
	}
	#endregion
}
