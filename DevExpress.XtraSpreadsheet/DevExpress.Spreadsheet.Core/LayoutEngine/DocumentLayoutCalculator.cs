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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	#region RowCellsController
	public class RowCellsController {
		readonly List<SingleCellTextBox> singleBoxes = new List<SingleCellTextBox>();
		readonly Page page;
		int rowModelIndex;
		int gridRowIndex;
		SingleCellTextBox lastNonEmptyCell;
		int initialSingleBoxCount;
		public RowCellsController(Page page) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
			this.initialSingleBoxCount = page.Boxes.Count;
		}
		public Page Page { get { return page; } }
		public int RowModelIndex { get { return rowModelIndex; } set { rowModelIndex = value; } }
		public int GridRowIndex { get { return gridRowIndex; } set { gridRowIndex = value; } }
		public void AppendSingleCellBox(ICell cell) {
			SingleCellTextBox box = new SingleCellTextBox();
			box.GridColumnIndex = page.GridColumns.TryCalculateIndex(cell.ColumnIndex);
			if (box.GridColumnIndex < 0)
				return;
			box.GridRowIndex = gridRowIndex;
			box.ClipLastColumnIndex = page.GridColumns.Count - 1;
			if (!cell.Value.IsEmpty) {
				if (lastNonEmptyCell != null) {
					lastNonEmptyCell.ClipLastColumnIndex = box.GridColumnIndex - 1;
					box.ClipFirstColumnIndex = lastNonEmptyCell.GridColumnIndex + 1;
				}
				lastNonEmptyCell = box;
			}
			singleBoxes.Add(box); 
			page.Boxes.Add(box);
		}
		public void ApplyComplexCellsClip(List<ComplexCellTextBox> complexCells) {
			if (singleBoxes.Count <= 0)
				return;
			int count = complexCells.Count;
			if (count <= 0)
				return;
			for (int i = 0; i < count; i++) {
				ComplexCellTextBox complexCell = complexCells[i];
				if (complexCell.ClipFirstRowIndex <= gridRowIndex && gridRowIndex <= complexCell.ClipLastRowIndex)
					ApplyComplexCellClip(complexCell);
			}
		}
		internal List<SingleCellTextBox> SingleBoxes { get { return singleBoxes; } }
		void ApplyComplexCellClip(ComplexCellTextBox mergedCell) {
			ApplyLeftComplexCellBound(mergedCell);
			ApplyRightComplexCellBound(mergedCell);
		}
		void ApplyLeftComplexCellBound(ComplexCellTextBox complexCell) {
			int index = Algorithms.BinarySearch(singleBoxes, new SingleCellTextBoxGridColumnComparable(complexCell.ClipFirstColumnIndex));
			Debug.Assert(index < 0);
			index = ~index;
			if (index > singleBoxes.Count)
				return;
			if (index > 0) {
				SingleCellTextBox box = singleBoxes[index - 1];
				box.ClipLastColumnIndex = Math.Min(box.ClipLastColumnIndex, complexCell.ClipFirstColumnIndex - 1);
			}
		}
		void ApplyRightComplexCellBound(ComplexCellTextBox complexCell) {
			int index = Algorithms.BinarySearch(singleBoxes, new SingleCellTextBoxGridColumnComparable(complexCell.ClipLastColumnIndex));
			Debug.Assert(index < 0);
			index = ~index;
			if (index >= singleBoxes.Count)
				return;
			SingleCellTextBox box = singleBoxes[index];
			box.ClipFirstColumnIndex = Math.Max(box.ClipFirstColumnIndex, complexCell.ClipLastColumnIndex + 1);
		}
		public void CalculateVerticalBorderBreaks(DocumentLayoutCalculatorBase layoutCalculator, RowVerticalBorderBreaks verticalBorderBreaks) {
			VerticalBorderBreaksCalculator calculator = new VerticalBorderBreaksCalculator(layoutCalculator, (PreliminaryPage)page, verticalBorderBreaks, GridRowIndex, RowModelIndex, initialSingleBoxCount);
			calculator.CalculateBreaks(singleBoxes);
		}
	}
	#endregion
	#region SingleCellTextBoxGridColumnComparable
	internal class SingleCellTextBoxGridColumnComparable : IComparable<SingleCellTextBox> {
		readonly int gridColumnIndex;
		public SingleCellTextBoxGridColumnComparable(int gridColumnIndex) {
			this.gridColumnIndex = gridColumnIndex;
		}
		public int CompareTo(SingleCellTextBox other) {
			return other.GridColumnIndex - gridColumnIndex;
		}
	}
	#endregion
	public class BorderBreaksInfo {
		readonly List<int> breaks = new List<int>();
		public List<int> Breaks { get { return breaks; } }
	}
	#region RowVerticalBorderBreaks
	public class RowVerticalBorderBreaks {
		readonly Dictionary<int, List<int>> breaksByRows = new Dictionary<int, List<int>>();
		public int Count { get { return breaksByRows.Count; } }
		public void Add(int rowModelIndex, List<int> breaks) {
			if (breaksByRows.ContainsKey(rowModelIndex))
				breaksByRows[rowModelIndex].AddRange(breaks);
			else
				breaksByRows.Add(rowModelIndex, breaks);
		}
		public void RemoveContinuousBreaks(int rowModelIndex, int gridColumnIndex) {
			if (!breaksByRows[rowModelIndex].Contains(gridColumnIndex))
				return;
			List<int> breaks = breaksByRows[rowModelIndex];
			int startColumnIndex = breaks.IndexOf(gridColumnIndex);
			int lastColumnIndex = startColumnIndex;
			while (breaks.Count - 1 > lastColumnIndex && breaks[lastColumnIndex + 1] - breaks[lastColumnIndex] == 1)
				lastColumnIndex++;
			breaks.RemoveRange(startColumnIndex, lastColumnIndex - startColumnIndex + 1);
		}
		public bool ContainsKey(int rowModelIndex) {
			return breaksByRows.ContainsKey(rowModelIndex);
		}
		public bool TryGetValue(int rowModelIndex, out List<int> result) {
			return breaksByRows.TryGetValue(rowModelIndex, out result);
		}
		public List<int> GetValue(int rowModelIndex) {
			if (breaksByRows.ContainsKey(rowModelIndex))
				return breaksByRows[rowModelIndex];
			return null;
		}
	}
	#endregion
	#region PreliminaryGridInfo
	public class PreliminaryGridInfo {
		CellRange printRange;
		CellRange extendedPrintRange;
		PageGrid totalColumnGrid;
		PageGrid totalRowGrid;
		List<CellRange> mergedCells;
		CellRangesCachedRTree mergedCellTree;
		List<IDrawingObject> drawingObjects;
		public CellRange PrintRange { get { return printRange; } set { printRange = value; } }
		public CellRange ExtendedPrintRange { get { return extendedPrintRange; } set { extendedPrintRange = value; } }
		public PageGrid TotalColumnGrid { get { return totalColumnGrid; } set { totalColumnGrid = value; } }
		public PageGrid TotalRowGrid { get { return totalRowGrid; } set { totalRowGrid = value; } }
		public List<Comment> Comments { get; set; }
		public List<IDrawingObject> DrawingObjects { get { return drawingObjects; } set { drawingObjects = value; } }
		public List<CellRange> MergedCells {
			get { return mergedCells; }
			set {
				if (mergedCells == value)
					return;
				mergedCells = value;
				mergedCellTree = null;
			}
		}
		public CellRangesCachedRTree MergedCellTree {
			get {
				if (mergedCellTree != null)
					return mergedCellTree;
				this.mergedCellTree = new CellRangesCachedRTree();
				if (mergedCells == null)
					return mergedCellTree;
				mergedCellTree.InsertRange(mergedCells);
				return mergedCellTree;
			}
		}
	}
	#endregion
	public class DocumentLayoutAnchor {
		public CellPosition CellPosition { get; set; }
		public bool HorizontalFarAlign { get; set; }
		public bool VerticalFarAlign { get; set; }
	}
	#region DocumentLayoutCalculator
	public class DocumentLayoutCalculator : DocumentLayoutCalculatorBase {
		#region Fields
		Rectangle viewBounds;
		float zoomFactor;
		Size headerOffset;
		bool customHeaders;
		CellRange printRange;
		#endregion
		public DocumentLayoutCalculator(DocumentLayout layout, Worksheet sheet, Rectangle viewBounds, float zoomFactor)
			: base(layout, sheet) {
			this.zoomFactor = zoomFactor;
			this.viewBounds = viewBounds;
			this.viewBounds.Width = (int)Math.Round((double)viewBounds.Width / (double)zoomFactor);
			this.viewBounds.Height = (int)Math.Round((double)viewBounds.Height / (double)zoomFactor);
			headerOffset = CalculateHeaderOffset(0);
			int groupHeight = GroupItemsPage.CalculateGroupHeightInPixels(this.Sheet);
			int groupWidth = GroupItemsPage.CalculateGroupWidthInPixels(this.Sheet);
			this.viewBounds.Y = (int)Math.Round((double)(headerOffset.Height + groupHeight) / (double)zoomFactor);
			this.viewBounds.Height -= this.viewBounds.Y;
			this.viewBounds.X = (int)Math.Round((double)groupWidth / (double)zoomFactor);
			this.viewBounds.Width -= this.viewBounds.X;
		}
		#region Properties
		protected Rectangle ViewBounds { get { return viewBounds; } set { viewBounds = value; } }
		internal protected Size HeaderOffset {
			get { return headerOffset; }
			set {
				headerOffset = value;
				customHeaders = headerOffset != Size.Empty;
			}
		}
		bool NeedToCorrectMinColumnIndex { get; set; }
		protected CellRange PrintRange { get { return this.printRange; } }
		#endregion
		protected override IEnumerable<ICellBase> GetLayoutVisibleCellsEnumerable(CellRange range) {
			return range.GetLayoutVisibleCellsEnumerable();
		}
		protected virtual bool IsColumnFitToViewBounds(int modelIndex, int currentExtent) {
			return currentExtent <= viewBounds.Right && modelIndex < Sheet.MaxColumnCount;
		}
		protected virtual bool IsRowFitToViewBounds(int modelIndex, int currentExtent) {
			return currentExtent <= viewBounds.Bottom && modelIndex < Sheet.MaxRowCount;
		}
		int CalculateMaxVisibleColumnModelIndex(DocumentLayoutAnchor anchor) {
			if (anchor.HorizontalFarAlign)
				return anchor.CellPosition.Column;
			else
				return CalculateMaxVisibleColumnModelIndex(anchor.CellPosition.Column);
		}
		int CalculateMaxVisibleColumnModelIndex(int left) {
			IColumnWidthCalculationService gridCalculator = this.Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			int currentExtent = viewBounds.Left;
			int firstFrozenColumn = this.Sheet.ActiveView.FrozenCell.Column;
			if (this.Sheet.ActiveView.HorizontalSplitPosition > 0) {
				for (int i = this.Sheet.ActiveView.TopLeftCell.Column; i < firstFrozenColumn; i++) {
					currentExtent += gridCalculator.CalculateColumnWidthTmp(Sheet, i);
					if (!IsColumnFitToViewBounds(i, currentExtent))
						return i;
				}
			}
			int column = left < firstFrozenColumn ? firstFrozenColumn : left;
			while (IsColumnFitToViewBounds(column, currentExtent)) {
				currentExtent += gridCalculator.CalculateColumnWidthTmp(Sheet, column);
				column++;
			}
			if (currentExtent == viewBounds.Left)
				NeedToCorrectMinColumnIndex = true;
			return Math.Max(left, column - 1);
		}
		int CalculateMinVisibleColumnModelIndex(DocumentLayoutAnchor anchor) {
			if (!anchor.HorizontalFarAlign)
				return anchor.CellPosition.Column;
			return CalculateMinVisibleColumnModelIndexCore(anchor);
		}
		int CalculateMinVisibleColumnModelIndexCore(DocumentLayoutAnchor anchor) {
			int right = anchor.CellPosition.Column;
			IColumnWidthCalculationService gridCalculator = this.Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			int currentExtent = viewBounds.Right;
			int horizontalSplitPosition = this.Sheet.ActiveView.HorizontalSplitPosition;
			if (horizontalSplitPosition > 0 && right > horizontalSplitPosition) {
				int firstFrozenColumn = this.Sheet.ActiveView.FrozenCell.Column;
				for (int i = this.Sheet.ActiveView.TopLeftCell.Column; i < firstFrozenColumn; i++)
					currentExtent -= gridCalculator.CalculateColumnWidthTmp(this.Sheet, i);
			}
			if (currentExtent < viewBounds.Left)
				return 0;
			int lastWidth = 0;
			for (int i = right; i >= 0; i--) {
				lastWidth = gridCalculator.CalculateColumnWidthTmp(this.Sheet, i);
				currentExtent -= lastWidth;
				if (currentExtent < viewBounds.Left) {
					CorrectAnchorHorizontal(anchor, right, gridCalculator, lastWidth);
					return i + 1;
				}
			}
			if (lastWidth > 0)
				CorrectAnchorHorizontal(anchor, right, gridCalculator, lastWidth);
			return 0;
		}
		int CalculateMaxVisibleRowModelIndex(int top) {
			IColumnWidthCalculationService gridCalculator = this.Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			int currentExtent = viewBounds.Top;
			int firstFrozenRow = this.Sheet.ActiveView.FrozenCell.Row;
			if (this.Sheet.ActiveView.VerticalSplitPosition > 0) {
				for (int i = this.Sheet.ActiveView.TopLeftCell.Row; i < firstFrozenRow; i++) {
					currentExtent += gridCalculator.CalculateRowHeight(Sheet, i);
					if (!IsRowFitToViewBounds(i, currentExtent))
						return i;
				}
			}
			int row = top < firstFrozenRow ? firstFrozenRow : top;
			while (IsRowFitToViewBounds(row, currentExtent)) {
				currentExtent += gridCalculator.CalculateRowHeight(Sheet, row);
				row++;
			}
			return Math.Max(top, row - 1);
		}
		int CalculateMaxVisibleRowModelIndex(DocumentLayoutAnchor anchor) {
			if (anchor.VerticalFarAlign)
				return anchor.CellPosition.Row;
			return CalculateMaxVisibleRowModelIndex(anchor.CellPosition.Row);
		}
		int CalculateMinVisibleRowModelIndex(DocumentLayoutAnchor anchor) {
			if (!anchor.VerticalFarAlign)
				return anchor.CellPosition.Row;
			int bottom = anchor.CellPosition.Row;
			IColumnWidthCalculationService gridCalculator = this.Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			int currentExtent = viewBounds.Bottom;
			int verticalSplitPosition = this.Sheet.ActiveView.VerticalSplitPosition;
			if (verticalSplitPosition > 0 && bottom > verticalSplitPosition) {
				int firstFrozenRow = this.Sheet.ActiveView.FrozenCell.Row;
				for (int i = this.Sheet.ActiveView.TopLeftCell.Row; i < firstFrozenRow; i++)
					currentExtent -= gridCalculator.CalculateRowHeight(this.Sheet, i);
			}
			if (currentExtent < viewBounds.Top)
				return 0;
			int lastHeight = 0;
			for (int i = bottom; i >= 0; i--) {
				lastHeight = gridCalculator.CalculateRowHeight(this.Sheet, i);
				currentExtent -= lastHeight;
				if (currentExtent < viewBounds.Top) {
					CorrectAnchorVertical(anchor, bottom, gridCalculator, lastHeight);
					return i + 1;
				}
			}
			if (lastHeight > 0)
				CorrectAnchorVertical(anchor, bottom, gridCalculator, lastHeight);
			return 0;
		}
		void CorrectAnchorVertical(DocumentLayoutAnchor anchor, int bottom, IColumnWidthCalculationService gridCalculator, int lastHeight) {
			int count = this.Sheet.MaxRowCount;
			int lastRowIndex = bottom;
			for (int i = bottom + 1; i < count; i++) {
				int rowHeight = gridCalculator.CalculateRowHeight(this.Sheet, i);
				if (rowHeight > 0) {
					lastHeight -= rowHeight;
					if (lastHeight < 0) {
						anchor.CellPosition = new CellPosition(anchor.CellPosition.Column, i);
						return;
					}
					lastRowIndex = i;
				}
			}
			if (lastHeight == 0 && lastRowIndex > bottom)
				anchor.CellPosition = new CellPosition(anchor.CellPosition.Column, lastRowIndex);
		}
		void CorrectAnchorHorizontal(DocumentLayoutAnchor anchor, int right, IColumnWidthCalculationService gridCalculator, int lastWidth) {
			for (int i = right + 1; i < this.Sheet.MaxColumnCount; i++) {
				lastWidth -= gridCalculator.CalculateColumnWidthTmp(this.Sheet, i);
				if (lastWidth <= 0) {
					anchor.CellPosition = new CellPosition(i, anchor.CellPosition.Row);
					break;
				}
			}
		}
		public override void CalculateLayout(CellRange range) {
			CalculateLayoutByAnchor(GetDefaultAnchor(), range);
		}
		DocumentLayoutAnchor GetDefaultAnchor() {
			DocumentLayoutAnchor anchor = new DocumentLayoutAnchor();
			anchor.CellPosition = this.Sheet.ActiveView.ScrolledTopLeftCell;
			anchor.HorizontalFarAlign = false;
			anchor.VerticalFarAlign = false;
			return anchor;
		}
		internal CellRange CalculateBoundingRange(DocumentLayoutAnchor anchor) {
			NeedToCorrectMinColumnIndex = false;
			int maxRowIndex = CalculateMaxVisibleRowModelIndex(anchor);
			Debug.Assert(maxRowIndex >= 0);
			headerOffset = CalculateHeaderOffset(maxRowIndex);
			this.viewBounds.X = (int)Math.Round((double)headerOffset.Width / (double)zoomFactor);
			this.viewBounds.Width -= this.viewBounds.X;
			int minRowIndex = CalculateMinVisibleRowModelIndex(anchor);
			int minColumnIndex = CalculateMinVisibleColumnModelIndex(anchor);
			if (anchor.VerticalFarAlign)
				maxRowIndex = anchor.CellPosition.Row;
			int maxColumnIndex = CalculateMaxVisibleColumnModelIndex(anchor);
			if (NeedToCorrectMinColumnIndex)
				minColumnIndex = CalculateMinVisibleColumnModelIndexCore(anchor);
			CellPosition topLeftCell = new CellPosition(minColumnIndex, minRowIndex);
			CellPosition bottomRightCell = new CellPosition(maxColumnIndex, maxRowIndex);
			return new CellRange(this.Sheet, topLeftCell, bottomRightCell);
		}
		public void CalculateLayoutByAnchor(DocumentLayoutAnchor anchor, CellRange printRange) {
			this.printRange = printRange;
			CellRange boundingRange = CalculateBoundingRange(anchor);
			Layout.VisibleRange = boundingRange;
			GeneratePages(boundingRange.TopLeft, boundingRange.BottomRight);
			Page lastPage = Layout.Pages[Layout.Pages.Count - 1];
			int totalRowCount = printRange.Height;
			if (printRange.TopRowIndex != lastPage.GridRows.ActualFirst.ModelIndex)
				totalRowCount = Math.Max(printRange.Height, lastPage.GridRows.ActualLast.ModelIndex);
			int hiddenRowCount = CalculateHiddenRowCount(Math.Max(printRange.BottomRowIndex, lastPage.GridRows.ActualLast.ModelIndex));
			int totalColumnCount = printRange.Width;
			int hiddenColumnCount = CalculateHiddenColumnCount(Math.Max(printRange.RightColumnIndex, lastPage.GridColumns.ActualLast.ModelIndex));
			Layout.ScrollInfo = new ScrollInfo(this.Sheet, totalRowCount, hiddenRowCount, Layout, this.Sheet.ActiveView.GetSplitPosition(), totalColumnCount, hiddenColumnCount);
		}
		int CalculateHiddenRowCount(int lastModelRowIndex) {
			int result = 0;
			for (int i = printRange.TopRowIndex; i <= lastModelRowIndex; i++) {
				Row row = Sheet.Rows.TryGetRow(i);
				if (row != null && row.IsHidden)
					result++;
			}
			return result;
		}
		int CalculateHiddenColumnCount(int lastModelColumnIndex) {
			int result = 0;
			for (int i = printRange.LeftColumnIndex; i <= lastModelColumnIndex; i++) {
				Column column = Sheet.Columns.TryGetColumn(i);
				if (column != null && column.IsHidden)
					result++;
			}
			return result;
		}
		protected override CellRange CalculateMergedCellsExtendedRange(List<CellRange> mergedCells, CellRange printRange, CellRange previousLongTextBoxesRange) {
			CellPosition nearPosition = printRange.TopLeft;
			CellPosition farPosition = printRange.BottomRight;
			if (previousLongTextBoxesRange != null)
				nearPosition = CellPosition.UnionPosition(nearPosition, previousLongTextBoxesRange.TopLeft, true);
			int count = mergedCells.Count;
			for (int i = 0; i < count; i++) {
				CellRange mergedCell = mergedCells[i];
				ICell cell = printRange.Worksheet.TryGetCell(mergedCell.TopLeft.Column, mergedCell.TopLeft.Row) as ICell;
				if ( (cell != null && cell.IsVisible())) {
					nearPosition = CellPosition.UnionPosition(nearPosition, mergedCell.TopLeft, true);
					farPosition = CellPosition.UnionPosition(farPosition, mergedCell.BottomRight, false);
				}
			}
			return new CellRange(printRange.Worksheet, nearPosition, farPosition);
		}
		protected PreliminaryPage CalculateLimitedLayout(CellRange printRange, float scaleFactor, CellRange previousLongTextBoxesRange, PreliminaryPage previousPage) {
			return CalculateLimitedLayout(printRange, scaleFactor, false, previousLongTextBoxesRange, previousPage);
		}
		PreliminaryPage CalculateLimitedLayout(CellRange printRange, float scaleFactor, bool hasRightPage, CellRange previousLongTextBoxesRange, PreliminaryPage previousPage) {
			return CalculateLimitedLayout(printRange, scaleFactor, false, hasRightPage, previousLongTextBoxesRange, previousPage);
		}
		PreliminaryPage CalculateLimitedLayout(CellRange printRange, float scaleFactor, bool forceCreateEmptyPage, bool hasRightPage, CellRange previousLongTextBoxesRange, PreliminaryPage previousPage) {
			if (printRange == null)
				return null;
			PageGridCalculator gridCalculator = CreatePageGridCalculator(Rectangle.Empty);
			CellRange forcePreviousLongTextBoxesRange = null;
			PreliminaryPage forcePreviousPage = null;
			bool forceHasRightPage = false;
			if (printRange.TopLeft.Column != 0 && previousLongTextBoxesRange == null) { 
				forcePreviousLongTextBoxesRange = new CellRange(printRange.Worksheet, new CellPosition(0, printRange.TopLeft.Row), new CellPosition(printRange.TopLeft.Column - 1, printRange.BottomRight.Row));
				forcePreviousPage = CalculateLimitedLayout(forcePreviousLongTextBoxesRange, scaleFactor, forceCreateEmptyPage, false, null, null);
			}
			forceHasRightPage = !hasRightPage && forcePreviousPage != null && forcePreviousPage.LongTextBoxes.Count > 0;
			PreliminaryGridInfo gridInfo = GetPreliminaryInfo(gridCalculator, printRange, printRange, scaleFactor, forceCreateEmptyPage, forceHasRightPage ? forcePreviousLongTextBoxesRange : previousLongTextBoxesRange);
			if (forceHasRightPage)
				return GenerateContent(gridInfo, true, forcePreviousPage);
			return GenerateContent(gridInfo, hasRightPage, previousPage);
		}
		public Size CalculateHeaderOffset(int lastRow) {
			Size offset = Size.Empty;
			if (this.Sheet.ShowColumnHeaders)
				offset = new Size(offset.Width, CalculateHeaderHeight());
			if (this.Sheet.ShowRowHeaders)
				offset = new Size(CalculateHeaderWidth(lastRow), offset.Height);
			return offset;
		}
		int CalculateHeaderHeight() {
			if (customHeaders)
				return headerOffset.Height;
			else {
				int result = Math.Max(0, this.Sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnits(this.Sheet.Workbook.ViewOptions.ColumnHeaderHeight, DocumentModel.DpiY));
				if (result == 0)
					return Sheet.Workbook.GetService<IColumnWidthCalculationService>().CalculateHeaderHeight(Sheet);
				else
					return result;
			}
		}
		protected override int CalculateHeaderWidth(int lastRow) {
			int result = base.CalculateHeaderWidth(lastRow);
			if (customHeaders)
				result = Math.Max(result, headerOffset.Width);
			return result;
		}
		internal void GeneratePages(CellPosition topLeftCell, CellPosition bottomRightCell) {
			PageRangesInfo pageRanges = CalculatePageRanges(topLeftCell, bottomRightCell);
			List<PreliminaryPage> preliminaryPages = new List<PreliminaryPage>();
			if (!TryAddPage(preliminaryPages, pageRanges.UpperLeft, false, pageRanges.UpperRight != null, ViewPaneType.TopLeft))
				pageRanges.UpperLeft = null;
			if (!TryAddPage(preliminaryPages, pageRanges.UpperRight, true, pageRanges.UpperLeft != null, ViewPaneType.TopRight))
				pageRanges.UpperRight = null;
			if (!TryAddPage(preliminaryPages, pageRanges.LowerLeft, false, pageRanges.LowerRight != null, ViewPaneType.BottomLeft))
				pageRanges.LowerLeft = null;
			if (!TryAddPage(preliminaryPages, pageRanges.LowerRight, true, pageRanges.LowerLeft != null, ViewPaneType.BottomRight))
				pageRanges.LowerRight = null;
			bool verticalPages = preliminaryPages.Count == 2;
			if (verticalPages)
				verticalPages = (pageRanges.LowerLeft == null && pageRanges.UpperLeft == null) || (pageRanges.UpperRight == null && pageRanges.LowerRight == null);
			FinalDocumentLayoutCalculator calculator = new FinalDocumentLayoutCalculator(Layout, Sheet, VerticalBorderBreaks, viewBounds, zoomFactor);
			calculator.GeneratePages(preliminaryPages, verticalPages, headerOffset);
		}
		internal bool TryAddPage(List<PreliminaryPage> pages, CellRange pageRange, bool isRight, bool hasOtherPage) {
			return TryAddPage(pages, pageRange, isRight, hasOtherPage, ViewPaneType.BottomRight);
		}
		internal bool TryAddPage(List<PreliminaryPage> pages, CellRange pageRange, bool isRight, bool hasOtherPage, ViewPaneType paneType) {
			if (pageRange == null)
				return true; 
			int pagesCount = pages.Count;
			PreliminaryPage previousPage = isRight && hasOtherPage && pagesCount > 0 ? pages[pagesCount - 1] : null;
			PreliminaryPage page = CalculateLimitedLayout(pageRange, 1f, !isRight && hasOtherPage, null, previousPage);
			bool result = page != null;
			if (result) {
				page.PaneType = paneType;
				pages.Add(page);
			}
			return result;
		}
		protected internal override int MeasureText(ICell cell, string text) {
			CellFormatStringMeasurer measurer = new CellFormatStringMeasurer(cell);
			return measurer.MeasureStringWidth(text);
		}
		protected override float CalculateScalingFactor(CellRange printRange, PageGridCalculator gridCalculator) {
			return zoomFactor;
		}
		protected internal virtual PageRangesInfo CalculatePageRanges(CellPosition topLeftCell, CellPosition bottomRightCell) {
			PageRangesInfo result = new PageRangesInfo();
			if (this.Sheet.IsFrozen()) {
				CellPosition activeViewTopLeftCell = this.Sheet.ActiveView.TopLeftCell;
				CellPosition frozenCellPosition = this.Sheet.ActiveView.FrozenCell;
				topLeftCell = new CellPosition(Math.Max(frozenCellPosition.Column, topLeftCell.Column), Math.Max(frozenCellPosition.Row, topLeftCell.Row));
				if (frozenCellPosition.Column > bottomRightCell.Column && frozenCellPosition.Row > bottomRightCell.Row) { 
					result.LowerRight = new CellRange(Sheet, new CellPosition(0, 0), bottomRightCell);
					return result;
				}
				if (frozenCellPosition.Row > 0)
					result.UpperRight = new CellRange(Sheet, new CellPosition(topLeftCell.Column, activeViewTopLeftCell.Row), new CellPosition(bottomRightCell.Column, frozenCellPosition.Row - 1));
				if (frozenCellPosition.Column > 0)
					result.LowerLeft = new CellRange(Sheet, new CellPosition(activeViewTopLeftCell.Column, topLeftCell.Row), new CellPosition(frozenCellPosition.Column - 1, bottomRightCell.Row));
				if (result.LowerLeft != null && result.UpperRight != null)
					result.UpperLeft = new CellRange(Sheet, new CellPosition(activeViewTopLeftCell.Column, activeViewTopLeftCell.Row), new CellPosition(frozenCellPosition.Column - 1, frozenCellPosition.Row - 1));
			}
			result.LowerRight = new CellRange(Sheet, topLeftCell, bottomRightCell);
			if (result.UpperLeft != null) {
				if (result.UpperLeft.BottomRight.Column >= result.UpperRight.TopLeft.Column) {
					result.UpperLeft = null;
					result.LowerLeft = null;
				}
				else if (result.UpperLeft.BottomRight.Row >= result.LowerLeft.TopLeft.Row) {
					result.UpperLeft = null;
					result.UpperRight = null;
				}
			}
			return result;
		}
	}
	#endregion
	public class PageRangesInfo {
		public CellRange UpperLeft { get; set; }
		public CellRange UpperRight { get; set; }
		public CellRange LowerLeft { get; set; }
		public CellRange LowerRight { get; set; }
	}
	#region ScrollInfo
	public class ScrollInfo {
		#region fields
		readonly Worksheet sheet;
		int scrollRowIndex;
		int scrollColumnIndex;
		int scrollTopRowModelIndex;
		int scrollLeftColumnModelIndex;
		int scrollBottomRowModelIndex;
		int scrollRightColumnModelIndex;
		int maxScrollRowIndex;
		int maxScrollColumnIndex;
		int largeRowChange;
		int largeColumnChange;
		int totalRowCount;
		int totalColumnCount;
		#endregion
		public ScrollInfo(Worksheet sheet, int totalRowCount, int hiddenRowCount, DocumentLayout layout, CellPosition splitTopLeftCell, int totalColumnCount, int hiddenColumnCount) {
			this.sheet = sheet;
			this.totalRowCount = Math.Max(totalRowCount, 1);
			this.totalColumnCount = totalColumnCount;
			CalculateVerticalScrollInfo(layout, splitTopLeftCell, hiddenRowCount);
			CalculateHorizontalScrollInfo(layout, splitTopLeftCell, hiddenColumnCount);
		}
		#region Properties
		public int ScrollBottomRowModelIndex { get { return scrollBottomRowModelIndex; } }
		public int ScrollRightColumnModelIndex { get { return scrollRightColumnModelIndex; } }
		public int ScrollTopRowModelIndex { get { return scrollTopRowModelIndex; } }
		public int ScrollLeftColumnModelIndex { get { return scrollLeftColumnModelIndex; } }
		public int ScrollRowIndex { get { return scrollRowIndex; } }
		public int ScrollColumnIndex { get { return scrollColumnIndex; } }
		public int MaximumRow { get { return maxScrollRowIndex; } }
		public int MaximumColumn { get { return maxScrollColumnIndex; } }
		public int LargeChangeRow { get { return largeRowChange; } }
		public int LargeChangeColumn { get { return largeColumnChange; } }
		#endregion
		#region CalculateVerticalScrollInfo etc
		void CalculateVerticalScrollInfo(DocumentLayout layout, CellPosition splitTopLeftCell, int hiddenRowCount) {
			this.scrollRowIndex = CalculateScrollRowIndex(layout, splitTopLeftCell);
			this.largeRowChange = CalculateLargeChangeRow(layout, splitTopLeftCell);
			this.maxScrollRowIndex = CalculateMaxScrollRowIndex(layout, splitTopLeftCell, hiddenRowCount);
			PageGrid lastPageGridRows = layout.Pages[layout.Pages.Count - 1].GridRows;
			this.scrollTopRowModelIndex = lastPageGridRows.ActualFirst.ModelIndex;
			this.scrollBottomRowModelIndex = lastPageGridRows.ActualLast.ModelIndex;
		}
		int CalculateScrollRowIndex(DocumentLayout layout, CellPosition splitTopLeftCell) {
			Page lastPage = layout.Pages[layout.Pages.Count - 1];
			if (lastPage.PaneType == ViewPaneType.TopLeft || lastPage.PaneType == ViewPaneType.TopRight)
				return 0;
			int visibleFrozenRowCount = CalculalteVisibleRowCountAbove(sheet.Rows, splitTopLeftCell.Row);
			int invisibleLayoutRowCount = CalculalteVisibleRowCountAbove(sheet.Rows, lastPage.GridRows.ActualFirst.ModelIndex);
			int result = invisibleLayoutRowCount - visibleFrozenRowCount;
			if (result < 0)
				result = totalRowCount - 1;
			return result;
		}
		int CalculateMaxScrollRowIndex(DocumentLayout layout, CellPosition splitTopLeftCell, int hiddenRowCount) {
			Page lastPage = layout.Pages[layout.Pages.Count - 1];
			if (lastPage.PaneType == ViewPaneType.TopLeft || lastPage.PaneType == ViewPaneType.TopRight)
				return 0;
			int frozenRowCount = splitTopLeftCell.Row == 0 ? 0 : layout.Pages[0].GridRows.ActualCount;
			int result = totalRowCount - frozenRowCount - hiddenRowCount;
			if (result <= lastPage.GridRows.Last.ModelIndex - lastPage.GridRows.First.ModelIndex - hiddenRowCount) {
				result = this.ScrollRowIndex + lastPage.GridRows.Count - lastPage.GridRows.ActualFirstIndex;
				if (this.ScrollRowIndex != 0)
					result--;
			}
			else {
				if (this.ScrollRowIndex > result - this.LargeChangeRow + 1)
					result = this.ScrollRowIndex + this.LargeChangeRow - 1;
			}
			return result;
		}
		int CalculateLargeChangeRow(DocumentLayout layout, CellPosition splitTopLeftCell) {
			Page lastPage = layout.Pages[layout.Pages.Count - 1];
			return lastPage.GridRows.ActualCount;
		}
		int CalculalteVisibleRowCountAbove(IRowCollection rows, int rowModelIndex) {
			int index = rows.TryGetRowIndex(rowModelIndex);
			if (index < 0) {
				index = ~index;
				if (index >= rows.Count)
					index = rows.Count;
			}
			int result = rowModelIndex;
			for (int i = index - 1; i >= 0; i--) {
				if (!rows.InnerList[i].IsVisible)
					result--;
			}
			return result;
		}
		#endregion
		#region CalculateHorizontalScrollInfo
		void CalculateHorizontalScrollInfo(DocumentLayout layout, CellPosition splitTopLeftCell, int hiddenColumnCount) {
			this.scrollColumnIndex = CalculateScrollColumnIndex(layout, splitTopLeftCell);
			this.largeColumnChange = CalculateLargeChangeColumn(layout, splitTopLeftCell);
			this.maxScrollColumnIndex = CalculateMaxScrollColumnIndex(layout, splitTopLeftCell, hiddenColumnCount);
			PageGrid lastPageGridColumns = layout.Pages[layout.Pages.Count - 1].GridColumns;
			this.scrollLeftColumnModelIndex = lastPageGridColumns.ActualFirst.ModelIndex;
			this.scrollRightColumnModelIndex = lastPageGridColumns.ActualLast.ModelIndex;
		}
		int CalculateScrollColumnIndex(DocumentLayout layout, CellPosition splitTopLeftCell) {
			Page lastPage = layout.Pages[layout.Pages.Count - 1];
			if (lastPage.PaneType == ViewPaneType.TopLeft || lastPage.PaneType == ViewPaneType.BottomLeft)
				return 0;
			int visibleFrozenColumnCount = CalculalteVisibleColumnCountAtLeft(((Worksheet)sheet).Columns, splitTopLeftCell.Column);
			int invisibleLayoutColumnCount = CalculalteVisibleColumnCountAtLeft(((Worksheet)sheet).Columns, lastPage.GridColumns.ActualFirst.ModelIndex);
			int result = invisibleLayoutColumnCount - visibleFrozenColumnCount;
			if (result < 0)
				result = ((Worksheet)sheet).MaxColumnCount - 1;
			return result;
		}
		int CalculateMaxScrollColumnIndex(DocumentLayout layout, CellPosition splitTopLeftCell, int hiddenColumnCount) {
			Page lastPage = layout.Pages[layout.Pages.Count - 1];
			if (lastPage.PaneType == ViewPaneType.TopLeft || lastPage.PaneType == ViewPaneType.BottomLeft)
				return 0;
			int frozenColumnCount = splitTopLeftCell.Column == 0 ? 0 : layout.Pages[0].GridColumns.ActualCount;
			int result = totalColumnCount - frozenColumnCount - hiddenColumnCount;
			if (result <= (lastPage.GridColumns.Last.ModelIndex - lastPage.GridColumns.First.ModelIndex - hiddenColumnCount)) {
				result = this.ScrollColumnIndex + lastPage.GridColumns.Count - lastPage.GridColumns.ActualFirstIndex;
				if (this.ScrollColumnIndex != 0)
					result--;
			}
			else {
				if (this.ScrollColumnIndex > result - this.LargeChangeColumn + 1)
					result = this.ScrollColumnIndex + this.LargeChangeColumn - 1;
			}
			return result;
		}
		int CalculateLargeChangeColumn(DocumentLayout layout, CellPosition splitTopLeftCell) {
			Page lastPage = layout.Pages[layout.Pages.Count - 1];
			return lastPage.GridColumns.ActualCount;
		}
		int CalculalteVisibleColumnCountAtLeft(ColumnCollection columns, int columnModelIndex) {
			int index = columns.TryGetColumnIndex(columnModelIndex);
			if (index < 0) {
				index = ~index;
				if (index >= columns.Count)
					index = columns.Count;
			}
			int result = columnModelIndex;
			for (int i = index - 1; i >= 0; i--) {
				Column column = columns.InnerList[i];
				if (!column.IsVisible)
					result -= (column.EndIndex - column.StartIndex + 1);
			}
			return result;
		}
		#endregion
	}
	#endregion
	#region SingleCellTextBoxModelColumnIndexComparable
	public class SingleCellTextBoxModelColumnIndexComparable : IComparable<SingleCellTextBox> {
		readonly PreliminaryPage page;
		readonly int modelColumnIndex;
		public SingleCellTextBoxModelColumnIndexComparable(PreliminaryPage page, int modelColumnIndex) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
			this.modelColumnIndex = modelColumnIndex;
		}
		#region IComparable<SingleCellTextBox> Members
		public int CompareTo(SingleCellTextBox other) {
			return page.GridColumns[other.GridColumnIndex].ModelIndex - modelColumnIndex;
		}
		#endregion
	}
	#endregion
	#region SingleCellTextBoxCellPositionComparable
	public class SingleCellTextBoxCellPositionComparable : IComparable<SingleCellTextBox> {
		readonly Page page;
		readonly CellPosition position;
		public SingleCellTextBoxCellPositionComparable(Page page, CellPosition position) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
			this.position = position;
		}
		#region IComparable<SingleCellTextBox> Members
		public int CompareTo(SingleCellTextBox other) {
			int modelRowIndex = page.GridRows[other.GridRowIndex].ModelIndex;
			if (modelRowIndex < position.Row)
				return -1;
			if (modelRowIndex > position.Row)
				return 1;
			int modelColumnIndex = page.GridColumns[other.GridColumnIndex].ModelIndex;
			if (modelColumnIndex < position.Column)
				return -1;
			if (modelColumnIndex > position.Column)
				return 1;
			return 0;
		}
		#endregion
	}
	#endregion
}
