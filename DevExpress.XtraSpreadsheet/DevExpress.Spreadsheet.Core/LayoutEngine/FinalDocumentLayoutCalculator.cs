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
using DevExpress.Office;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System.Text;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Controls;
#endif
namespace DevExpress.XtraSpreadsheet.Layout.Engine {
	#region FinalDocumentLayoutCalculator
	public class FinalDocumentLayoutCalculatorBase {
		readonly DocumentLayout layout;
		readonly Worksheet sheet;
		PreliminaryPage preliminaryPage;
		readonly List<int> horizontalGridOffsets;
		readonly List<int> verticalGridOffsets;
		readonly List<int> horizontalOffsets;
		readonly List<int> verticalOffsets;
		readonly RowVerticalBorderBreaks verticalBorderBreaks;
		Rectangle pageBounds;
		Rectangle pageClientBounds;
		List<PageGrid> gridColumns;
		List<PageGrid> gridRows;
		PageGrid preliminaryColumnGrid;
		PageGrid preliminaryRowGrid;
		List<IDrawingObject> drawingObjects;
		Size headingSize;
		List<Page> pages;
		public FinalDocumentLayoutCalculatorBase(DocumentLayout layout, Worksheet sheet, PreliminaryPage preliminaryPage, RowVerticalBorderBreaks verticalBorderBreaks) {
			Guard.ArgumentNotNull(layout, "layout");
			Guard.ArgumentNotNull(sheet, "sheet");
			SetPreliminaryProperties(preliminaryPage);
			Guard.ArgumentNotNull(verticalBorderBreaks, "verticalBorderBreaks");
			this.layout = layout;
			this.sheet = sheet;
			this.preliminaryPage = preliminaryPage;
			this.verticalBorderBreaks = verticalBorderBreaks;
			this.horizontalGridOffsets = new List<int>();
			this.verticalGridOffsets = new List<int>();
			this.horizontalOffsets = new List<int>();
			this.verticalOffsets = new List<int>();
			this.pages = new List<Page>();
			gridColumns = new List<PageGrid>();
			gridRows = new List<PageGrid>();
		}
		protected void SetPreliminaryProperties(PreliminaryPage preliminaryPage) {
			if (preliminaryPage != null) {
				this.preliminaryPage = preliminaryPage;
				preliminaryColumnGrid = preliminaryPage.GridColumns;
				preliminaryRowGrid = preliminaryPage.GridRows;
				drawingObjects = preliminaryPage.DrawingObjects;
			}
		}
		public DocumentLayout Layout { get { return layout; } }
		public Worksheet Sheet { get { return sheet; } }
		public virtual bool PrintGridLines {
			get {
				PrintSetup printSetup = sheet.PrintSetup;
				return printSetup.PrintGridLinesSet && printSetup.PrintGridLines;
			}
		}
		public virtual IActualBorderInfo GridLinesBorder { get { return Sheet.ActiveView.PrintGridBorder; } }
		protected Rectangle PageBounds { get { return pageBounds; } set { pageBounds = value; } }
		protected Rectangle PageClientBounds { get { return pageClientBounds; } set { pageClientBounds = value; } }
		internal protected PreliminaryPage PreliminaryPage { get { return preliminaryPage; } }
		internal protected List<PageGrid> GridRows { get { return gridRows; } }
		internal protected List<Page> Pages { get { return pages; } }
		protected internal RowVerticalBorderBreaks VerticalBorderBreaks { get { return verticalBorderBreaks; } }
		public void GeneratePages() {
			GeneratePages(0, 0);
		}
		public void GeneratePages(int headingWidth, int headingHeight) {
			headingSize = new Size(headingWidth, headingHeight);
			PageDimensionsCalculatorBase calculator = CreatePageDimensionsCalculator();
			this.pageBounds = new Rectangle(Point.Empty, calculator.CalculateActualPageSize());
			this.pageClientBounds = calculator.CalculatePageClientBounds(pageBounds.Size);
			CalculateHeaderFooterLayout(pageBounds);
			PageGridCalculator gridCalculator = CreatePageGridCalculator(pageClientBounds);
			this.gridRows = gridCalculator.CalculateRowGrid(preliminaryRowGrid, headingHeight, sheet.PrintSetup.CenterVertically);
			this.gridColumns = gridCalculator.CalculateColumnGrid(preliminaryColumnGrid, headingWidth, sheet.PrintSetup.CenterHorizontally);
			if (gridRows.Count <= 0 || gridColumns.Count <= 0)
				return;
			GeneratePagesCore();
			PageOrder order = null;
			if (sheet.PrintSetup.PagePrintOrder == PagePrintOrder.DownThenOver)
				order = new PageOrderDownThenOver(gridColumns.Count, gridRows.Count);
			else
				order = new PageOrderOverThenDown(gridColumns.Count, gridRows.Count);
			GeneratePagesHorizontalBorders(order, gridRows.Count, gridColumns.Count);
			GeneratePagesVerticalBorders(order, gridRows.Count, gridColumns.Count);
			if (sheet.PrintSetup.PagePrintOrder == PagePrintOrder.DownThenOver)
				order.ReorderPages(pages);
			layout.Pages.AddRange(pages);
		}
		protected internal void CalculateHeaderFooterLayout(Rectangle pageBounds) {
			Layout.CalculateHeaderFooterLayout(Sheet, pageBounds);
		}
		protected internal virtual PageGridCalculator CreatePageGridCalculator(Rectangle pageClientBounds) {
			return new PageGridCalculator(sheet, pageClientBounds);
		}
		protected virtual PageDimensionsCalculatorBase CreatePageDimensionsCalculator() {
			return new PageDimensionsCalculatorBase(sheet);
		}
		protected void SetOffsets(List<Page> pages) {
			this.pages = pages;
			this.verticalGridOffsets.Clear();
			this.horizontalGridOffsets.Clear();
			this.horizontalOffsets.Clear();
			this.verticalOffsets.Clear();
			foreach (Page page in pages) {
				this.verticalGridOffsets.Add(0);
				this.horizontalGridOffsets.Add(0);
				this.verticalOffsets.Add(-page.GridRows.First.Near);
				this.horizontalOffsets.Add(-page.GridColumns.First.Near);
			}
		}
		void GeneratePagesCore() {
			int pageRowCount = gridRows.Count;
			int pageColumnCount = gridColumns.Count;
			Debug.Assert(pageRowCount > 0);
			Debug.Assert(pageColumnCount > 0);
			CalculateOffsets(pageRowCount, pageColumnCount);
			CollectSingleBoxes(pageRowCount, pageColumnCount);
			ProcessComplexBoxes();
			DrawingObjectLayoutCalculatorWalker drawingObjectLayoutCalculatorWalker =
				new DrawingObjectLayoutCalculatorWalker(drawingObjects, this, preliminaryColumnGrid, preliminaryRowGrid, null);
			drawingObjectLayoutCalculatorWalker.Walk();
		}
		void CalculateOffsets(int pageRowCount, int pageColumnCount) {
			int verticalGridOffset = 0;
			int verticalOffset = 0;
			for (int rowIndex = 0; rowIndex < pageRowCount; rowIndex++) {
				verticalGridOffsets.Add(verticalGridOffset);
				verticalOffsets.Add(verticalOffset);
				PageGrid gridRow = gridRows[rowIndex];
				verticalGridOffset += gridRow.Count;
				verticalOffset += gridRow.Last.Far - gridRow.First.Near;
			}
			int horizontalGridOffset = 0;
			int horizontalOffset = 0;
			for (int columnIndex = 0; columnIndex < pageColumnCount; columnIndex++) {
				horizontalGridOffsets.Add(horizontalGridOffset);
				horizontalOffsets.Add(horizontalOffset);
				PageGrid gridColumn = gridColumns[columnIndex];
				horizontalGridOffset += gridColumn.Count;
				horizontalOffset += gridColumn.Last.Far - gridColumn.First.Near;
			}
		}
		void CollectSingleBoxes(int pageRowCount, int pageColumnCount) {
			for (int rowIndex = 0; rowIndex < pageRowCount; rowIndex++) {
				PageGrid gridRow = gridRows[rowIndex];
				List<PreliminaryPageRowInfo> singleBoxRows = CalculateSingleRowInfos(gridRow);
				for (int columnIndex = 0; columnIndex < pageColumnCount; columnIndex++) {
					PageGrid gridColumn = gridColumns[columnIndex];
					Page page = CreatePage(gridColumn, gridRow);
					page.ScaleFactor = preliminaryPage.ScaleFactor;
					AppendSingleBoxes(page, singleBoxRows, horizontalGridOffsets[columnIndex], verticalGridOffsets[rowIndex]);
				}
			}
		}
		void ProcessComplexBoxes() {
			IList<ComplexCellTextBox> complexBoxes = preliminaryPage.ComplexBoxes;
			int count = complexBoxes.Count;
			for (int i = 0; i < count; i++)
				ProcessComplexBox(preliminaryColumnGrid, preliminaryRowGrid, complexBoxes[i]);
		}
		Page CreatePage(PageGrid gridColumns, PageGrid gridRows) {
			Page page = new PreliminaryPage(layout, sheet, gridColumns, gridRows, false);
			page.Bounds = pageBounds;
			page.ClientBounds = pageClientBounds;
			page.Index = pages.Count;
			page.HeadingSize = headingSize;
			pages.Add(page);
			return page;
		}
		int CalculateFirstSingleRowInfoIndex(PageGrid gridRow) {
			List<PreliminaryPageRowInfo> infos = preliminaryPage.SingleBoxRowInfos;
			PreliminaryPageRowInfoModelRowIndexComparable predicate = new PreliminaryPageRowInfoModelRowIndexComparable(preliminaryPage, gridRow.First.ModelIndex);
			int result = Algorithms.BinarySearch(infos, predicate);
			if (result < 0) {
				result = ~result;
				if (result >= infos.Count)
					return -1;
			}
			return result;
		}
		int CalculateLastSingleRowInfoIndex(PageGrid gridRow, int firstIndex) {
			List<PreliminaryPageRowInfo> infos = preliminaryPage.SingleBoxRowInfos;
			PreliminaryPageRowInfoModelRowIndexComparable predicate = new PreliminaryPageRowInfoModelRowIndexComparable(preliminaryPage, gridRow.Last.ModelIndex);
			int result = Algorithms.BinarySearch(infos, predicate, firstIndex + 1, infos.Count - 1);
			if (result < 0) {
				result = ~result;
				if (result >= infos.Count)
					result = infos.Count - 1;
				else
					result--;
			}
			return result;
		}
		protected List<PreliminaryPageRowInfo> CalculateSingleRowInfos(PageGrid gridRow) {
			List<PreliminaryPageRowInfo> result = new List<PreliminaryPageRowInfo>();
			int firstIndex = CalculateFirstSingleRowInfoIndex(gridRow);
			if (firstIndex < 0)
				return result;
			int lastIndex = CalculateLastSingleRowInfoIndex(gridRow, firstIndex);
			Debug.Assert(lastIndex >= firstIndex);
			List<PreliminaryPageRowInfo> infos = preliminaryPage.SingleBoxRowInfos;
			PreliminaryPageRowInfo rowInfo = new PreliminaryPageRowInfo();
			rowInfo.GridRowIndex = infos[firstIndex].GridRowIndex;
			if (firstIndex == 0)
				rowInfo.LastCellIndex = 0;
			else
				rowInfo.LastCellIndex = infos[firstIndex - 1].LastCellIndex;
			result.Add(rowInfo);
			for (int i = firstIndex; i <= lastIndex; i++)
				result.Add(infos[i]);
			return result;
		}
		protected void AppendSingleBoxes(Page page, List<PreliminaryPageRowInfo> singleBoxRows, int offsetX, int offsetY) {
			int count = singleBoxRows.Count;
			if (count <= 0)
				return;
			Debug.Assert(singleBoxRows.Count >= 2);
			for (int row = 1; row < count; row++) {
				PreliminaryPageRowInfo previousRow = singleBoxRows[row - 1];
				PreliminaryPageRowInfo currentRow = singleBoxRows[row];
				int firstIndex = CalculateFirstCellIndex(previousRow.LastCellIndex + 1, currentRow.LastCellIndex, page);
				if (firstIndex < 0 || preliminaryPage.GridRows[currentRow.GridRowIndex].ModelIndex > page.GridRows.ActualLast.ModelIndex)
					continue;
				int lastIndex = CalculateLastCellIndex(previousRow.LastCellIndex + 1, currentRow.LastCellIndex, page, firstIndex);
				Debug.Assert(lastIndex >= firstIndex);
				for (int i = firstIndex; i <= lastIndex; i++) {
					SingleCellTextBox box = preliminaryPage.Boxes[i];
					if (box.GridColumnIndex - offsetX < 0 || box.GridRowIndex - offsetY < 0)
						continue; 
					SingleCellTextBox newBox = new SingleCellTextBox();
					newBox.GridColumnIndex = box.GridColumnIndex - offsetX;
					newBox.GridRowIndex = box.GridRowIndex - offsetY;
					Debug.Assert(newBox.GridColumnIndex >= 0);
					Debug.Assert(newBox.GridColumnIndex < page.GridColumns.Count);
					Debug.Assert(newBox.GridRowIndex >= 0);
					Debug.Assert(newBox.GridRowIndex < page.GridRows.Count);
					newBox.ClipFirstColumnIndex = Math.Max(0, box.ClipFirstColumnIndex - offsetX);
					newBox.ClipLastColumnIndex = Math.Min(page.GridColumns.Count - 1, box.ClipLastColumnIndex - offsetX);
					Debug.Assert(newBox.ClipFirstColumnIndex >= 0);
					Debug.Assert(newBox.ClipFirstColumnIndex < page.GridColumns.Count);
					Debug.Assert(newBox.ClipLastColumnIndex >= 0);
					Debug.Assert(newBox.ClipLastColumnIndex < page.GridColumns.Count);
					SetBindingAttributes(newBox, page);
					SetPivotItemInfo(newBox, page);
					page.Boxes.Add(newBox);
				}
			}
		}
		void SetBindingAttributes(CellTextBoxBase box, Page page) {
			ICell cell = box.GetCell(page.GridColumns, page.GridRows, page.Sheet);
			if (!cell.IsVisible())
				return;
			if (!cell.HasFormula)
				return;
			FormulaBase formula = cell.Formula;
			if (formula.Expression == null || !formula.Expression.ContainsInternalFunction)
				return;
			FunctionFieldFinderVisitor visitor = new FunctionFieldFinderVisitor();
			visitor.Process(formula.Expression);
			if (!visitor.HaveFunctionField && !visitor.HaveFunctionFieldPicture)
				return;
			box.HasBindingField = visitor.HaveFunctionField;
			box.HasBindingPicture = visitor.HaveFunctionFieldPicture;
		}
		void SetPivotItemInfo(CellTextBoxBase box, Page page) {
			ICell cell = box.GetCell(page.GridColumns, page.GridRows, page.Sheet);
			PivotTable pivot = page.Sheet.TryGetPivotTable(cell.Position);
			if (pivot == null)
				return;
			PivotLayoutCellInfo info;
			pivot.LayoutCellCache.TryGetButtonInfo(cell.Position, out info);
			if (info != null) {
				box.PivotLayoutCellInfo = info;
				if (info.CollapseButton)
					box.IsPivotButtonCollapsed = pivot.ItemIsCollapsed(info.FieldIndex, info.ItemIndex);
			}
			else
				box.PivotLayoutCellInfo = null;
		}
		int CalculateFirstCellIndex(int firstCellIndex, int lastCellIndex, Page page) {
			if (firstCellIndex > lastCellIndex)
				return -1;
			SingleCellTextBoxModelColumnIndexComparable predicate = new SingleCellTextBoxModelColumnIndexComparable(preliminaryPage, page.GridColumns.First.ModelIndex);
			int result = Algorithms.BinarySearch(preliminaryPage.Boxes, predicate, firstCellIndex, lastCellIndex);
			if (result < 0) {
				result = ~result;
				if (result > lastCellIndex)
					return -1;
			}
			int modelIndex = preliminaryColumnGrid[preliminaryPage.Boxes[result].GridColumnIndex].ModelIndex;
			if (modelIndex >= page.GridColumns.First.ModelIndex && modelIndex <= page.GridColumns.Last.ModelIndex)
				return result;
			else
				return -1;
		}
		int CalculateLastCellIndex(int firstCellIndex, int lastCellIndex, Page page, int firstIndex) {
			SingleCellTextBoxModelColumnIndexComparable predicate = new SingleCellTextBoxModelColumnIndexComparable(preliminaryPage, page.GridColumns.Last.ModelIndex);
			int result = Algorithms.BinarySearch(preliminaryPage.Boxes, predicate, firstCellIndex, lastCellIndex);
			if (result < 0) {
				result = ~result;
				result--;
				result = Math.Min(result, lastCellIndex);
			}
			int modelIndex = preliminaryColumnGrid[preliminaryPage.Boxes[result].GridColumnIndex].ModelIndex;
			if (modelIndex <= page.GridColumns.Last.ModelIndex)
				return result;
			else
				return firstIndex;
		}
		protected virtual void ProcessComplexBox(PageGrid columnGrid, PageGrid rowGrid, ComplexCellTextBox complexBox) {
			Page topLeftPage = CalculatePage(columnGrid, rowGrid, complexBox.ClipFirstColumnIndex, complexBox.ClipFirstRowIndex);
			int topIndex = topLeftPage.Index / gridColumns.Count;
			int leftIndex = topLeftPage.Index % gridColumns.Count;
			int rightIndex;
			int bottomIndex;
			Page topRightPage = CalculatePage(columnGrid, rowGrid, complexBox.ClipLastColumnIndex, complexBox.ClipFirstRowIndex);
			if (topLeftPage != topRightPage) {
				rightIndex = topRightPage.Index % gridColumns.Count;
				Page bottomLeftPage = CalculatePage(columnGrid, rowGrid, complexBox.ClipFirstColumnIndex, complexBox.ClipLastRowIndex);
				if (topLeftPage != bottomLeftPage)
					bottomIndex = bottomLeftPage.Index / gridColumns.Count;
				else
					bottomIndex = topIndex;
			}
			else {
				rightIndex = leftIndex;
				Page bottomPage = CalculatePage(columnGrid, rowGrid, complexBox.ClipFirstColumnIndex, complexBox.ClipLastRowIndex);
				if (topLeftPage != bottomPage)
					bottomIndex = bottomPage.Index / gridColumns.Count;
				else
					bottomIndex = topIndex;
			}
			for (int y = topIndex; y <= bottomIndex; y++)
				for (int x = leftIndex; x <= rightIndex; x++)
					AppendComplexBox(columnGrid, rowGrid, complexBox, pages[y * gridColumns.Count + x]);
		}
		internal virtual void ProcessPicture(PageGrid gridColumn, PageGrid gridRow, Page page, Picture picture) {
			ProcessDrawing(gridColumn, gridRow, picture);
		}
		internal virtual void ProcessChart(PageGrid gridColumn, PageGrid gridRow, Page page, Chart chart) {
			ProcessDrawing(gridColumn, gridRow, chart);
		}
		internal virtual void ProcessShape(PageGrid gridColumn, PageGrid gridRow, Page page, ModelShape shape) {
			ProcessDrawing(gridColumn, gridRow, shape);
		}
		Rectangle CalculateIndicesRange(PageGrid columnGrid, PageGrid rowGrid, IDrawingObject drawingObject) {
			Page topLeftPage = CalculateNearestPage(columnGrid, rowGrid, drawingObject.From.Col, drawingObject.From.Row);
			int topIndex = topLeftPage.Index / gridColumns.Count;
			int leftIndex = topLeftPage.Index % gridColumns.Count;
			int rightIndex;
			int bottomIndex;
			Page topRightPage = CalculateNearestPage(columnGrid, rowGrid, drawingObject.To.Col, drawingObject.From.Row);
			if (topLeftPage != topRightPage) {
				rightIndex = topRightPage.Index % gridColumns.Count;
				Page bottomLeftPage = CalculateNearestPage(columnGrid, rowGrid, drawingObject.From.Col, drawingObject.To.Row);
				if (topLeftPage != bottomLeftPage)
					bottomIndex = bottomLeftPage.Index / gridColumns.Count;
				else
					bottomIndex = topIndex;
			}
			else {
				rightIndex = leftIndex;
				Page bottomPage = CalculateNearestPage(columnGrid, rowGrid, drawingObject.From.Col, drawingObject.To.Row);
				if (topLeftPage != bottomPage)
					bottomIndex = bottomPage.Index / gridColumns.Count;
				else
					bottomIndex = topIndex;
			}
			return Rectangle.FromLTRB(leftIndex, topIndex, rightIndex, bottomIndex);
		}
		Page CalculateNearestPage(PageGrid columnGrid, PageGrid rowGrid, int columnModelIndex, int rowModelIndex) {
			int column = columnGrid.LookupFarItem(columnModelIndex);
			int row = rowGrid.LookupFarItem(rowModelIndex);
			return CalculatePageByModelIndicies(columnGrid[column].ModelIndex, rowGrid[row].ModelIndex);
		}
		protected void ProcessDrawing(PageGrid columnGrid, PageGrid rowGrid, IDrawingObject drawingObject) {
			float scalefactor = preliminaryPage.ScaleFactor;
			Point topLeftOffset = new Point((int)(layout.UnitConverter.TwipsToLayoutUnits((int)drawingObject.From.ColOffset) * scalefactor),
				(int)(layout.UnitConverter.TwipsToLayoutUnits((int)drawingObject.From.RowOffset) * scalefactor));
			Point bottomRightOffset = new Point((int)(layout.UnitConverter.TwipsToLayoutUnits((int)drawingObject.To.ColOffset) * scalefactor),
				(int)(layout.UnitConverter.TwipsToLayoutUnits((int)drawingObject.To.RowOffset) * scalefactor));
			Size imageSize = GetPreliminaryDrawingSize(columnGrid, rowGrid, drawingObject);
			imageSize.Width += bottomRightOffset.X - topLeftOffset.X;
			imageSize.Height += bottomRightOffset.Y - topLeftOffset.Y;
			Rectangle indicesRange = CalculateIndicesRange(columnGrid, rowGrid, drawingObject);
			for (int y = indicesRange.Top; y <= indicesRange.Bottom; y++)
				for (int x = indicesRange.Left; x <= indicesRange.Right; x++)
					AppendDrawingObjectBox(columnGrid, rowGrid, drawingObject, pages[y * gridColumns.Count + x], imageSize, topLeftOffset, bottomRightOffset);
		}
		protected Page CalculatePage(PageGrid columnGrid, PageGrid rowGrid, int gridColumn, int gridRow) {
			return CalculatePageByModelIndicies(columnGrid[gridColumn].ModelIndex, rowGrid[gridRow].ModelIndex);
		}
		protected Page CalculatePageByModelIndicies(int modelColumnIndex, int modelRowIndex) {
			int pageRow = CalculatePageRow(modelRowIndex);
			int pageColumn = CalculatePageColumn(modelColumnIndex);
			if (pageRow < 0 || pageColumn < 0 || pageColumn + pageRow * gridColumns.Count > pages.Count - 1)
				return null;
			return pages[pageColumn + pageRow * gridColumns.Count];
		}
		class PageGridModelIndexComparable : IComparable<PageGrid> {
			readonly int modelIndex;
			public PageGridModelIndexComparable(int modelIndex) {
				this.modelIndex = modelIndex;
			}
			#region IComparable<PageGrid> Members
			public int CompareTo(PageGrid other) {
				return other.First.ModelIndex - modelIndex;
			}
			#endregion
		}
		int CalculatePageRow(int rowModelIndex) {
			return CalculateGridIndexByModelIndex(gridRows, rowModelIndex);
		}
		int CalculatePageColumn(int columnModelIndex) {
			return CalculateGridIndexByModelIndex(gridColumns, columnModelIndex);
		}
		protected int CalculateGridIndexByModelIndex(List<PageGrid> grid, int modelIndex) {
			PageGridModelIndexComparable predicate = new PageGridModelIndexComparable(modelIndex);
			int index = Algorithms.BinarySearch(grid, predicate);
			if (index < 0) {
				index = ~index;
				index--;
				if (index < 0)
					return -1;
				if (grid[index].First.ModelIndex <= modelIndex && modelIndex <= grid[index].Last.ModelIndex)
					return index;
				else
					return -1;
			}
			return index;
		}
		protected void AppendComplexBox(PageGrid columnGrid, PageGrid rowGrid, ComplexCellTextBox complexBox, int pageIndex) {
			AppendComplexBox(columnGrid, rowGrid, complexBox, layout.Pages[pageIndex], pageIndex, pageIndex);
		}
		protected void AppendComplexBox(PageGrid columnGrid, PageGrid rowGrid, ComplexCellTextBox complexBox, Page page) {
			int pageColumn = page.Index % gridColumns.Count;
			int pageRow = page.Index / gridColumns.Count;
			AppendComplexBox(columnGrid, rowGrid, complexBox, page, pageColumn, pageRow);
		}
		void AppendComplexBox(PageGrid columnGrid, PageGrid rowGrid, ComplexCellTextBox complexBox, Page page, int pageColumn, int pageRow) {
			ComplexCellTextBox box = new ComplexCellTextBox(complexBox.GetCell(columnGrid, rowGrid, Sheet));
			box.IsLongTextBox = complexBox.IsLongTextBox;
			box.ClipFirstColumnIndex = Math.Max(0, complexBox.ClipFirstColumnIndex - horizontalGridOffsets[pageColumn]);
			if (box.ClipFirstColumnIndex >= page.GridColumns.Count)
				return;
			box.ClipLastColumnIndex = Math.Min(page.GridColumns.Count - 1, complexBox.ClipLastColumnIndex - horizontalGridOffsets[pageColumn]);
			box.ClipFirstRowIndex = Math.Max(0, complexBox.ClipFirstRowIndex - verticalGridOffsets[pageRow]);
			if (box.ClipFirstRowIndex >= page.GridRows.Count)
				return;
			box.ClipLastRowIndex = Math.Min(page.GridRows.Count - 1, complexBox.ClipLastRowIndex - verticalGridOffsets[pageRow]);
			Rectangle bounds = complexBox.Bounds;
			bounds.X += pageClientBounds.Left - horizontalOffsets[pageColumn];
			bounds.Y += pageClientBounds.Top - verticalOffsets[pageRow];
			if (page.Sheet.PrintSetup.PrintHeadings)
				bounds.Offset(page.HeadingSize.Width, page.HeadingSize.Height);
			box.Bounds = bounds;
			SetBindingAttributes(box, page);
			SetPivotItemInfo(box, page);
			page.ComplexBoxes.Add(box);
			if (!page.RowComplexBoxes.ContainsKey(box.ClipFirstRowIndex))
				page.RowComplexBoxes.Add(box.ClipFirstRowIndex, new ComplexCellTextBoxList());
			page.RowComplexBoxes[box.ClipFirstRowIndex].Add(box);
		}
		void AppendDrawingObjectBox(PageGrid columnGrid, PageGrid rowGrid, IDrawingObject drawingObject, Page page, Size imageSize, Point topLeftOffset, Point bottomRightOffset) {
			int fromCol = columnGrid.CalculateExactOrFarItem(drawingObject.From.Col);
			int toCol = columnGrid.CalculateExactOrFarItem(drawingObject.To.Col);
			fromCol = Math.Min(fromCol, toCol);
			toCol = Math.Max(fromCol, toCol);
			int fromRow = rowGrid.CalculateExactOrNearItem(drawingObject.From.Row);
			int toRow = rowGrid.CalculateExactOrNearItem(drawingObject.To.Row);
			fromRow = Math.Min(fromRow, toRow);
			toRow = Math.Max(fromRow, toRow);
			int horizontalGridOffset = horizontalGridOffsets[page.Index % gridColumns.Count];
			int verticalGridOffset = verticalGridOffsets[page.Index / gridColumns.Count];
			bool leftOnThisPage = fromCol >= horizontalGridOffset;
			bool topOnThisPage = fromRow >= verticalGridOffset;
			bool rightOnThisPage = toCol >= horizontalGridOffset && toCol < horizontalGridOffset + page.GridColumns.Count;
			bool bottomOnThisPage = toRow >= verticalGridOffset && toRow < verticalGridOffset + page.GridRows.Count;
			int clipFirstColumnIndex = Math.Max(0, fromCol - horizontalGridOffset);
			int clipLastColumnIndex = Math.Min(page.GridColumns.Count - 1, toCol - horizontalGridOffset);
			int clipFirstRowIndex = Math.Max(0, fromRow - verticalGridOffset);
			int clipLastRowIndex = Math.Min(page.GridRows.Count - 1, toRow - verticalGridOffset);
			Point topLeft = new Point(page.GridColumns[clipFirstColumnIndex].Near, page.GridRows[clipFirstRowIndex].Near);
			if (leftOnThisPage)
				topLeft.X += topLeftOffset.X;
			if (topOnThisPage)
				topLeft.Y += topLeftOffset.Y;
			Point bottomRight = new Point(page.GridColumns[clipLastColumnIndex].Near, page.GridRows[clipLastRowIndex].Near);
			if (rightOnThisPage)
				bottomRight.X += bottomRightOffset.X;
			else if (leftOnThisPage)
				bottomRight.X = page.GridColumns[clipLastColumnIndex].Far;
			if (bottomOnThisPage)
				bottomRight.Y += bottomRightOffset.Y;
			else if (topOnThisPage)
				bottomRight.Y = page.GridRows[clipLastRowIndex].Far;
			Rectangle clipBounds = Rectangle.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
			clipBounds.X += pageClientBounds.Left;
			clipBounds.Y += pageClientBounds.Top;
			int horizontalImageOffset = leftOnThisPage ? 0 : GetHorizontalDrawingObjectOffset(columnGrid, drawingObject, horizontalGridOffset, topLeftOffset.X);
			int verticalImageOffset = topOnThisPage ? 0 : GetVerticalDrawingObjectOffset(rowGrid, drawingObject, verticalGridOffset, topLeftOffset.Y);
			Rectangle bounds = new Rectangle(horizontalImageOffset, verticalImageOffset, imageSize.Width, imageSize.Height);
			DrawingEffectWalker walker = new DrawingEffectWalker(drawingObject);
			walker.Walk();
			DrawingBoxFactory factory = new DrawingBoxFactory(bounds, clipBounds, walker.ColorMatrixElements);
			drawingObject.Visit(factory);
			if (factory.Result != null)
				page.DrawingBoxes.Add(factory.Result);
		}
		private class DrawingBoxFactory : IDrawingObjectVisitor {
			readonly Rectangle bounds;
			readonly Rectangle clipBounds;
			readonly float[][] colorMatrixElements;
			public DrawingBoxFactory(Rectangle bounds, Rectangle clipBounds, float[][] colorMatrixElements) {
				this.bounds = bounds;
				this.clipBounds = clipBounds;
				this.colorMatrixElements = colorMatrixElements;
			}
			public DrawingBox Result { get; private set; }
			#region IDrawingObjectVisitor Members
			void IDrawingObjectVisitor.Visit(Picture picture) {
				this.Result = new PictureBox(picture, bounds, clipBounds, colorMatrixElements);
			}
			void IDrawingObjectVisitor.Visit(Chart chart) {
				this.Result = new ChartBox(chart, bounds, clipBounds);
			}
			void IDrawingObjectVisitor.Visit(ModelShape shape) { 
			}
			public void Visit(GroupShape groupShape) {
			}
			public void Visit(ConnectionShape connectionShape) {
			}
			#endregion
		}
		Size GetPreliminaryDrawingSize(PageGrid columnGrid, PageGrid rowGrid, IDrawingObject drawingObject) {
			int width = columnGrid[columnGrid.CalculateExactOrNearItem(drawingObject.To.Col)].Near - columnGrid[columnGrid.CalculateExactOrFarItem(drawingObject.From.Col)].Near;
			int height = rowGrid[rowGrid.CalculateExactOrNearItem(drawingObject.To.Row)].Near - rowGrid[rowGrid.CalculateExactOrFarItem(drawingObject.From.Row)].Near;
			return new Size(Math.Abs(width), Math.Abs(height));
		}
		int GetHorizontalDrawingObjectOffset(PageGrid columnGrid, IDrawingObject drawingObject, int horizontalGridOffset, int topLeftOffsetX) {
			int start = columnGrid[columnGrid.CalculateExactOrFarItem(drawingObject.From.Col)].Near + topLeftOffsetX;
			int end = columnGrid[horizontalGridOffset].Near;
			return start - end;
		}
		int GetVerticalDrawingObjectOffset(PageGrid rowGrid, IDrawingObject drawingObject, int verticalGridOffset, int topLeftOffsetY) {
			int start = rowGrid[rowGrid.CalculateExactOrFarItem(drawingObject.From.Row)].Near + topLeftOffsetY;
			int end = rowGrid[verticalGridOffset].Near;
			return start - end;
		}
		void GeneratePagesHorizontalBorders(PageOrder order, int rowCount, int columnCount) {
			int count = pages.Count;
			Debug.Assert(pages.Count == rowCount * columnCount);
			for (int i = 0; i < count; i++) {
				Page page = pages[i];
				Page previousPage = order.GetPreviousPageVerticalDirection(pages, page);
				Page nextPage = order.GetNextPageVerticalDirection(pages, page);
				GeneratePageHorizontalBorders(page, previousPage, nextPage);
			}
		}
		void GeneratePagesVerticalBorders(PageOrder order, int rowCount, int columnCount) {
			int count = pages.Count;
			Debug.Assert(pages.Count == rowCount * columnCount);
			for (int i = 0; i < count; i++) {
				Page page = pages[i];
				Page previousPage = order.GetPreviousPageHorizontalDirection(pages, page);
				Page nextPage = order.GetNextPageHorizontalDirection(pages, page);
				GeneratePageVerticalBorders(page, previousPage, nextPage);
			}
		}
		void GeneratePageHorizontalBorders(Page page, Page previousPage, Page nextPage) {
			PageBordersCalculator calculator = new HorizontalBordersCalculator(page, null, null);
			calculator.PrintGridLines = PrintGridLines;
			calculator.GridLinesBorder = GridLinesBorder;
			calculator.CalculateBorders();
		}
		void GeneratePageVerticalBorders(Page page, Page previousPage, Page nextPage) {
			RowVerticalBorderBreaks breaks = verticalBorderBreaks;
			if (breaks.Count <= 0)
				breaks = null;
			PageBordersCalculator calculator = new VerticalBordersCalculator(page, null, null, breaks);
			calculator.PrintGridLines = PrintGridLines;
			calculator.GridLinesBorder = GridLinesBorder;
			calculator.CalculateBorders();
		}
	}
	public class FinalDocumentLayoutCalculator : FinalDocumentLayoutCalculatorBase {
		#region fields
		Rectangle viewBounds;
		float scaleFactor;
		float zoomFactor;
		#endregion
		public FinalDocumentLayoutCalculator(DocumentLayout layout, Worksheet sheet, RowVerticalBorderBreaks verticalBorderBreaks, Rectangle viewBounds, float zoomFactor)
			: base(layout, sheet, null, verticalBorderBreaks) {
			this.viewBounds = viewBounds;
			this.zoomFactor = zoomFactor;
		}
		#region Properties
		public override bool PrintGridLines { get { return Sheet.ActiveView.ShowGridlines && (zoomFactor * scaleFactor >= Layout.ScaleFactorLimitForVisibleGridlines); } }
		public override IActualBorderInfo GridLinesBorder { get { return Sheet.ActiveView.DefaultGridBorder; } }
		#endregion
		public void GeneratePages(List<PreliminaryPage> preliminaryPages, bool verticalPages, Size headerOffset) {
			scaleFactor = preliminaryPages[0].ScaleFactor;
			List<Page> pages = new List<Page>();
			CreateGroupItemsPage(headerOffset);
			Size gridItemsOffset = CalculateGroupOffset();
			GenerateHeaderPage(preliminaryPages, verticalPages, headerOffset, gridItemsOffset);
#if !SL
			headerOffset = Size.Add(headerOffset, gridItemsOffset);
#else
			headerOffset.Width += gridItemsOffset.Width;
			headerOffset.Height += gridItemsOffset.Height;
#endif
			for (int pageIndex = 0; pageIndex < preliminaryPages.Count; pageIndex++) {
				PreliminaryPage preliminaryPage = preliminaryPages[pageIndex];
				PageGrid gridColumn = preliminaryPage.GridColumns.OffsetGrid(-preliminaryPage.GridColumns.ActualFirst.Near);
				PageGrid gridRow = preliminaryPage.GridRows.OffsetGrid(-preliminaryPage.GridRows.ActualFirst.Near);
				if (pageIndex == 0) {
					gridColumn = gridColumn.OffsetGrid(headerOffset.Width);
					gridRow = gridRow.OffsetGrid(headerOffset.Height);
				}
				if (pageIndex == 3 || (pageIndex == 1 && !verticalPages)) {
					gridColumn = gridColumn.OffsetGrid(pages[0].GridColumns.ActualLast.Far);
					if (pageIndex != 3)
						gridRow = gridRow.OffsetGrid(headerOffset.Height);
				}
				if (pageIndex >= 2 || (pageIndex == 1 && verticalPages)) {
					gridRow = gridRow.OffsetGrid(pages[0].GridRows.ActualLast.Far);
					if (pageIndex != 3)
						gridColumn = gridColumn.OffsetGrid(headerOffset.Width);
				}
				SetPreliminaryProperties(preliminaryPage);
				pages.Add(GeneratePage(gridColumn, gridRow));
				if (viewBounds.Height > 0 && viewBounds.Width > 0)
					GeneratePageBorders(pages[pages.Count - 1], preliminaryPage.Breaks);
			}
			SetOffsets(pages);
			SetPagesAlignments(pages, verticalPages);
			Layout.Pages.AddRange(pages);
			GenerateGroupItemsPage(pages, verticalPages);
			CalculateComplexBoxes(preliminaryPages, Layout.Pages);
		}
		void GeneratePageBorders(Page page, RowVerticalBorderBreaks breaks) {
			GeneratePageHorizontalBorders(page);
			if (breaks == null || breaks.Count == 0)
				GeneratePageVerticalBordersNoBreaks(page);
			else
				GeneratePageVerticalBorders(page);
		}
		void SetPagesAlignments(List<Page> pages, bool verticalPages) {
			if (pages.Count == 1) {
				pages[0].DesignAlignment = PageAlignment.One;
			}
			else if (pages.Count == 2) {
				if (verticalPages) {
					pages[0].DesignAlignment = PageAlignment.Top;
					pages[1].DesignAlignment = PageAlignment.Bottom;
				}
				else {
					pages[0].DesignAlignment = PageAlignment.Left;
					pages[1].DesignAlignment = PageAlignment.Right;
				}
			}
			else if (pages.Count == 4) {
				pages[0].DesignAlignment = PageAlignment.TopLeft;
				pages[1].DesignAlignment = PageAlignment.TopRight;
				pages[2].DesignAlignment = PageAlignment.BottomLeft;
				pages[3].DesignAlignment = PageAlignment.BottomRight;
			}
		}
		void CalculateComplexBoxes(List<PreliminaryPage> preliminaryPages, IList<Page> pages) {
			for (int pageIndex = 0; pageIndex < pages.Count; pageIndex++) {
				PreliminaryPage preliminaryPage = preliminaryPages[pageIndex];
				Page page = pages[pageIndex];
				CalculateComplexBoxesCore(preliminaryPage, pageIndex);
				ProcessComments(preliminaryPage.Comments, page, page.GridColumns, page.GridRows);
			}
		}
		void CalculateComplexBoxesCore(PreliminaryPage preliminaryPage, int pageIndex) {
			IList<ComplexCellTextBox> complexBoxes = preliminaryPage.ComplexBoxes;
			int count = complexBoxes.Count;
			for (int i = 0; i < count; i++) {
				AppendComplexBox(preliminaryPage.GridColumns, preliminaryPage.GridRows, complexBoxes[i], pageIndex);
			}
		}
		void CreateGroupItemsPage(Size headerOffset) {
			Layout.GroupItemsPage = new GroupItemsPage(Sheet, headerOffset);
		}
		Size CalculateGroupOffset() {
			return Layout.GroupItemsPage.GroupItemsOffset;
		}
		void GenerateGroupItemsPage(List<Page> pages, bool verticalPages) {
			List<PageGrid> columnGrids = new List<PageGrid>();
			List<PageGrid> rowGrids = new List<PageGrid>();
			if (viewBounds.Width > 0) {
				columnGrids.Add(pages[0].GridColumns);
				if (!verticalPages && pages.Count > 1)
					columnGrids.Add(pages[1].GridColumns);
			}
			if (viewBounds.Height > 0) {
				rowGrids.Add(pages[0].GridRows);
				if (verticalPages)
					rowGrids.Add(pages[1].GridRows);
				else if (pages.Count == 4)
					rowGrids.Add(pages[2].GridRows);
			}
			Layout.GroupItemsPage.GenerateContent(columnGrids, rowGrids);
		}
		void GenerateHeaderPage(List<PreliminaryPage> pages, bool verticalPages, Size headerOffset, Size gridItemsOffset) {
			bool showHeadings = Sheet.ShowRowHeaders || Sheet.ShowColumnHeaders; 
			Layout.HeaderPage = showHeadings ? new HeaderPage(Sheet) : null;
			if (!showHeadings)
				return;
			List<PageGrid> columnHeaderGrids = new List<PageGrid>();
			List<PageGrid> rowHeaderGrids = new List<PageGrid>();
			if (viewBounds.Width > 0 && Sheet.ShowColumnHeaders) {
				columnHeaderGrids.Add(pages[0].GridColumns);
				if (!verticalPages && pages.Count > 1)
					columnHeaderGrids.Add(pages[1].GridColumns);
			}
			if (viewBounds.Height > 0 && Sheet.ShowRowHeaders) {
				rowHeaderGrids.Add(pages[0].GridRows);
				if (verticalPages)
					rowHeaderGrids.Add(pages[1].GridRows);
				else if (pages.Count == 4)
					rowHeaderGrids.Add(pages[2].GridRows);
			}
			Layout.GenerateHeadersContent(columnHeaderGrids, rowHeaderGrids, headerOffset, gridItemsOffset);
		}
		protected virtual Rectangle CalculatePageClientBounds(Rectangle pageBounds) {
			int width = Math.Min(pageBounds.Size.Width, viewBounds.Width);
			int height = Math.Min(pageBounds.Size.Height, viewBounds.Height);
			return new Rectangle(0, 0, width, height);
		}
		protected Page GeneratePage(PageGrid gridColumn, PageGrid gridRow) {
			List<PreliminaryPageRowInfo> singleBoxRows = CalculateSingleRowInfos(gridRow);
			Page page = new Page(Layout, Sheet, gridColumn, gridRow);
			page.Bounds = Rectangle.FromLTRB(gridColumn.ActualFirst.Near, gridRow.ActualFirst.Near, gridColumn.ActualLast.Far, gridRow.ActualLast.Far);
			page.ClientBounds = CalculatePageClientBounds(page.Bounds);
			page.PaneType = PreliminaryPage.PaneType;
			AppendSingleBoxes(page, singleBoxRows, 0, 0);
			DrawingObjectLayoutCalculatorWalker drawingObjectLayoutCalculatorWalker =
				new DrawingObjectLayoutCalculatorWalker(PreliminaryPage.DrawingObjects, this, gridColumn, gridRow, page);
			drawingObjectLayoutCalculatorWalker.Walk();
			return page;
		}
		internal override void ProcessPicture(PageGrid gridColumn, PageGrid gridRow, Page page, Picture picture) {
			ProcessDrawing(picture, gridColumn, gridRow, page);
		}
		internal override void ProcessChart(PageGrid gridColumn, PageGrid gridRow, Page page, Chart chart) {
			ProcessDrawing(chart, gridColumn, gridRow, page);
		}
		internal override void ProcessShape(PageGrid gridColumn, PageGrid gridRow, Page page, ModelShape shape) {
			ProcessDrawing(shape, gridColumn, gridRow, page);
		}
		protected void ProcessDrawing(IDrawingObject drawing, PageGrid gridColumn, PageGrid gridRow, Page page) {
			RectangleF? bounds = CalculateAnchorDataBounds(drawing.AnchorData, gridColumn, gridRow);
			if (bounds == null)
				return;
			DrawingBoxFactory factory = new DrawingBoxFactory(bounds.Value);
			drawing.Visit(factory);
			if (factory.Result != null)
				page.DrawingBoxes.Add(factory.Result);
		}
		RectangleF? CalculateAnchorDataBounds(AnchorData anchorData, PageGrid gridColumn, PageGrid gridRow) {
			CellRange pageRange = new CellRange(Sheet, new CellPosition(gridColumn.First.ModelIndex, gridRow.First.ModelIndex), new CellPosition(gridColumn.Last.ModelIndex, gridRow.Last.ModelIndex));
			CellRange pictureRange = new CellRange(Sheet, new CellPosition(anchorData.From.Col, anchorData.From.Row), new CellPosition(anchorData.To.Col, anchorData.To.Row));
			CellRange intersection = pageRange.IntersectionWith(pictureRange).CellRangeValue as CellRange;
			if (intersection == null)
				return null;
			if (anchorData.Height == 0 || anchorData.Width == 0)
				return null;
			var bounds = GetAnchorDataBounds(anchorData, gridColumn, gridRow, intersection);
			Size webOffset = GetWebDrawingBoxOffset(gridColumn, gridRow);
			bounds.Offset(webOffset.Width, webOffset.Height);
			return bounds;
		}
		RectangleF GetAnchorDataBounds(AnchorData anchorData, PageGrid gridColumn, PageGrid gridRow, CellRange intersection) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = Sheet.Workbook.ToDocumentLayoutUnitConverter;
			int fromCol = gridColumn.CalculateExactOrFarItem(intersection.TopLeft.Column);
			int toCol = gridColumn.CalculateExactOrNearItem(intersection.BottomRight.Column);
			if (toCol < fromCol) {
				int temp = fromCol;
				fromCol = toCol;
				toCol = temp;
			}
			int fromRow = gridRow.CalculateExactOrFarItem(intersection.TopLeft.Row);
			int toRow = gridRow.CalculateExactOrNearItem(intersection.BottomRight.Row);
			if (toRow < fromRow) {
				int temp = fromRow;
				fromRow = toRow;
				toRow = temp;
			}
			PointF location = new PointF(gridColumn[fromCol].Near, gridRow[fromRow].Near);
			int right = Sheet.IsColumnVisible(anchorData.To.Col) ? gridColumn[toCol].Near : gridColumn[toCol].Far;
			int bottom = Sheet.IsRowVisible(anchorData.To.Row) ? gridRow[toRow].Near : gridRow[toRow].Far;
			PointF bottomLocation = new PointF(right, bottom);
			location.X += Math.Min(unitConverter.ToLayoutUnits(anchorData.From.ColOffset), gridColumn[fromCol].Extent);
			location.Y += Math.Min(unitConverter.ToLayoutUnits(anchorData.From.RowOffset), gridRow[fromRow].Extent);
			bottomLocation.X += unitConverter.ToLayoutUnits(anchorData.To.ColOffset);
			bottomLocation.Y += unitConverter.ToLayoutUnits(anchorData.To.RowOffset);
			RectangleF bounds = RectangleF.FromLTRB(location.X, location.Y, bottomLocation.X, bottomLocation.Y);
			return bounds;
		}
		protected virtual Size GetWebDrawingBoxOffset(PageGrid columnGrid, PageGrid rowGrid) {
			return Size.Empty;
		}
		#region Comments
		#region CommentPosition
		struct CommentPosition {
			#region Fields
			int near;
			int far;
			#endregion
			internal CommentPosition(int near, int far) {
				this.near = near;
				this.far = far;
			}
			#region Properties
			internal int Near { get { return near; } }
			internal int Far { get { return far; } }
			#endregion
		}
		#endregion
		void ProcessComments(List<Comment> comments, Page page, PageGrid gridColumns, PageGrid gridRows) {
			if (comments.Count == 0)
				return;
			int borderWidth = Layout.UnitConverter.PixelsToLayoutUnits(1, DocumentModel.Dpi);
			foreach (Comment comment in comments) {
				ProcessComment(comment, page, gridColumns, gridRows, borderWidth);
			}
		}
		void ProcessComment(Comment comment, Page page, PageGrid gridColumns, PageGrid gridRows, int borderWidth) {
			if (!comment.Worksheet.IsColumnVisible(comment.Reference.Column) || !comment.Worksheet.IsRowVisible(comment.Reference.Row))
				return;
			Rectangle indicatorBounds = ProcessCommentIndicator(comment, page, gridColumns, gridRows, borderWidth);
			ProcessCommentCore(comment, page, gridColumns, gridRows, borderWidth, indicatorBounds);
		}
		Rectangle ProcessCommentIndicator(Comment comment, Page page, PageGrid gridColumns, PageGrid gridRows, int borderWidth) {
			CellPosition reference = comment.Reference;
			Rectangle bounds = FindCommentIndicatorInComplexBoxes(page, reference, borderWidth);
			if (!bounds.IsEmpty)
				return bounds;
			bool isIndicatorInvisible = page.IsBoundsNotIntersectsWithVisibleBounds(gridRows, reference.Row, reference.Row) ||
				page.IsBoundsNotIntersectsWithVisibleBounds(gridColumns, reference.Column, reference.Column);
			if (isIndicatorInvisible)
				return Rectangle.Empty;
			Rectangle clipBounds = CalculateCommentIndicatorBounds(reference, gridColumns, gridRows, borderWidth);
			IndicatorBox indicatorBox = new IndicatorBox(clipBounds, IndicatorType.Comment);
			page.IndicatorBoxes.Add(indicatorBox);
			return clipBounds;
		}
		Rectangle FindCommentIndicatorInComplexBoxes(Page page, CellPosition reference, int borderWidth) {
			PageGrid gridColumns = page.GridColumns;
			PageGrid gridRows = page.GridRows;
			foreach (ComplexCellTextBox complexTextBox in page.ComplexBoxes) {
				if (complexTextBox.IsLongTextBox)
					continue;
				int leftModelIndex = gridColumns[complexTextBox.ClipFirstColumnIndex].ModelIndex;
				int rightModelIndex = gridColumns[complexTextBox.ClipLastColumnIndex].ModelIndex;
				int topModelIndex = gridRows[complexTextBox.ClipFirstRowIndex].ModelIndex;
				int bottomModelIndex = gridRows[complexTextBox.ClipLastRowIndex].ModelIndex;
				if (reference.Column >= leftModelIndex && reference.Column <= rightModelIndex && reference.Row >= topModelIndex && reference.Row <= bottomModelIndex) {
					Rectangle boxBounds = complexTextBox.Bounds;
					Rectangle indicatorBounds = Rectangle.FromLTRB(boxBounds.Left + borderWidth, boxBounds.Top + borderWidth, boxBounds.Right, boxBounds.Bottom);
					IndicatorBox box = new IndicatorBox(indicatorBounds, IndicatorType.Comment);
					page.IndicatorBoxes.Add(box);
					return indicatorBounds;
				}
			}
			return Rectangle.Empty;
		}
		Rectangle CalculateCommentIndicatorBounds(CellPosition reference, PageGrid gridColumns, PageGrid gridRows, int borderWidth) {
			int leftIndex = gridColumns.LookupNearItem(reference.Column);
			PageGridItem column = gridColumns[leftIndex];
			int left = column.Near + borderWidth;
			int right = column.Far;
			int topIndex = gridRows.LookupNearItem(reference.Row);
			PageGridItem row = gridRows[topIndex];
			int top = row.Near + borderWidth;
			int bottom = row.Far;
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		void ProcessCommentCore(Comment comment, Page page, PageGrid gridColumns, PageGrid gridRows, int borderWidth, Rectangle indicatorBounds) {
			bool needCalculateCommentBounds = NeedCalculateCommentBounds(comment, gridColumns, gridRows);
			if (!needCalculateCommentBounds)
				return;
			Point endLinePoint = CalculateCommentLineEndPoint(indicatorBounds, comment, gridColumns, gridRows, borderWidth);
			Rectangle bounds = CalculateCommentBounds(comment, gridColumns, gridRows, borderWidth);
			CommentBox box = new CommentBox(comment, bounds, endLinePoint);
			page.CommentBoxes.Add(box);
		}
		bool NeedCalculateCommentBounds(Comment comment, PageGrid gridColumns, PageGrid gridRows) {
			AnchorPoint from = comment.Shape.ClientData.Anchor.From;
			AnchorPoint to = comment.Shape.ClientData.Anchor.To;
			int commentColumn = comment.Reference.Column;
			int commentRow = comment.Reference.Row;
			int leftModelIndex = gridColumns.ActualFirst.ModelIndex;
			if (to.Col < leftModelIndex && commentColumn < leftModelIndex)
				return false;
			int rightModelIndex = gridColumns.ActualLast.ModelIndex;
			if (from.Col > rightModelIndex && commentColumn > rightModelIndex)
				return false;
			int topModelIndex = gridRows.ActualFirst.ModelIndex;
			if (to.Row < topModelIndex && commentRow < topModelIndex)
				return false;
			int bottomModelIndex = gridRows.ActualLast.ModelIndex;
			if (from.Row > bottomModelIndex && commentRow > bottomModelIndex)
				return false;
			return true;
		}
		Rectangle CalculateCommentBounds(Comment comment, PageGrid gridColumns, PageGrid gridRows, int borderWidth) {
			AnchorPoint from = comment.Shape.ClientData.Anchor.From;
			AnchorPoint to = comment.Shape.ClientData.Anchor.To;
			CommentPosition horizontal = CalculateHorizontalPositions(comment, from, to, gridColumns, borderWidth);
			CommentPosition vertical = CalculateVerticalPositions(comment, from, to, gridRows, borderWidth);
			return Rectangle.FromLTRB(horizontal.Near, vertical.Near, horizontal.Far, vertical.Far);
		}
		CommentPosition CalculateHorizontalPositions(Comment comment, AnchorPoint from, AnchorPoint to, PageGrid gridColumns, int borderWidth) {
			int left = CalculateLeftPosition(comment, from, gridColumns, borderWidth, true);
			int right = CalculateRightPosition(comment, to, gridColumns, borderWidth, true);
			return new CommentPosition(left, right);
		}
		int CalculateLeftPosition(Comment comment, AnchorPoint from, PageGrid gridColumns, int borderWidth, bool calculateOutside) {
			int index = gridColumns.CalculateExactOrNearItem(from.Col);
			if (index < 0) {
				if (!calculateOutside)
					return -1;
				if (from.Col < gridColumns.ActualFirst.ModelIndex)
					return CalculateLeftPositionOutsideClientBounds(comment, from, gridColumns);
				return CalculateRightPositionOutsideClientBounds(comment, from, gridColumns, borderWidth);
			}
			int result = comment.Worksheet.IsColumnVisible(from.Col) ? gridColumns[index].Near : gridColumns[index].Far;
			result += GetOffsetInLayoutUnits(from.ColOffset);
			return result;
		}
		int CalculateLeftPositionOutsideClientBounds(Comment comment, AnchorPoint from, PageGrid gridColumns) {
			IColumnWidthCalculationService columnWidthCalculator = Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			float maxDigitWidth = columnWidthCalculator.CalculateMaxDigitWidthInLayoutUnits(comment.Workbook);
			float maxDigitWidthInPixels = columnWidthCalculator.CalculateMaxDigitWidthInPixels(comment.Workbook);
			int position = gridColumns.First.Near;
			int start = gridColumns.First.ModelIndex - 1;
			int end = from.Col;
			for (int i = start; i >= end; i--) {
				int width = columnWidthCalculator.CalculateColumnWidth(comment.Worksheet, i, maxDigitWidth, maxDigitWidthInPixels);
				position -= width;
			}
			position += GetOffsetInLayoutUnits(from.ColOffset);
			return position;
		}
		int CalculateRightPosition(Comment comment, AnchorPoint to, PageGrid gridColumns, int borderWidth, bool calculateOutside) {
			int index = gridColumns.CalculateExactOrNearItem(to.Col);
			if (index < 0) {
				if (!calculateOutside)
					return -1;
				if (to.Col > gridColumns.ActualLast.ModelIndex)
					return CalculateRightPositionOutsideClientBounds(comment, to, gridColumns, borderWidth);
				return CalculateLeftPositionOutsideClientBounds(comment, to, gridColumns);
			}
			if (!comment.Worksheet.IsColumnVisible(to.Col))
				index++;
			int result = gridColumns[index].Near;
			result += GetOffsetInLayoutUnits(to.ColOffset) + borderWidth;
			return result;
		}
		int CalculateRightPositionOutsideClientBounds(Comment comment, AnchorPoint to, PageGrid gridColumns, int borderWidth) {
			IColumnWidthCalculationService columnWidthCalculator = Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			float maxDigitWidth = columnWidthCalculator.CalculateMaxDigitWidthInLayoutUnits(comment.Workbook);
			float maxDigitWidthInPixels = columnWidthCalculator.CalculateMaxDigitWidthInPixels(comment.Workbook);
			int position = gridColumns.Last.Far;
			int start = gridColumns.Last.ModelIndex + 1;
			int end = to.Col;
			for (int i = start; i < end; i++) {
				int width = columnWidthCalculator.CalculateColumnWidth(comment.Worksheet, i, maxDigitWidth, maxDigitWidthInPixels);
				position += width;
			}
			position += GetOffsetInLayoutUnits(to.ColOffset) + borderWidth;
			return position;
		}
		CommentPosition CalculateVerticalPositions(Comment comment, AnchorPoint from, AnchorPoint to, PageGrid gridRows, int borderWidth) {
			int top = CalculateTopPosition(comment, from, gridRows, borderWidth, true);
			int bottom = CalculateBottomPosition(comment, to, gridRows, borderWidth, true);
			return new CommentPosition(top, bottom);
		}
		int CalculateTopPosition(Comment comment, AnchorPoint from, PageGrid gridRows, int borderWidth, bool calculateOutside) {
			int index = gridRows.CalculateExactOrNearItem(from.Row);
			if (index < 0) {
				if (!calculateOutside)
					return -1;
				if (from.Row < gridRows.ActualFirst.ModelIndex)
					return CalculateTopPositionOutsideClientBounds(comment, from, gridRows);
				return CalculateBottomPositionOutsideClientBounds(comment, from, gridRows, borderWidth);
			}
			int result = comment.Worksheet.IsRowVisible(from.Row) ? gridRows[index].Near : gridRows[index].Far;
			result += GetOffsetInLayoutUnits(from.RowOffset);
			return result;
		}
		int CalculateTopPositionOutsideClientBounds(Comment comment, AnchorPoint from, PageGrid gridRows) {
			IColumnWidthCalculationService columnWidthCalculator = Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			int position = gridRows.First.Near;
			int start = gridRows.First.ModelIndex - 1;
			int end = from.Row;
			for (int i = start; i >= end; i--) {
				int height = columnWidthCalculator.CalculateRowHeight(comment.Worksheet, i);
				position -= height;
			}
			position += GetOffsetInLayoutUnits(from.RowOffset);
			return position;
		}
		int CalculateBottomPosition(Comment comment, AnchorPoint to, PageGrid gridRows, int borderWidth, bool calculateOutside) {
			int index = gridRows.CalculateExactOrNearItem(to.Row);
			if (index < 0) {
				if (!calculateOutside)
					return -1;
				if (to.Row > gridRows.ActualLast.ModelIndex)
					return CalculateBottomPositionOutsideClientBounds(comment, to, gridRows, borderWidth);
				return CalculateTopPositionOutsideClientBounds(comment, to, gridRows);
			}
			if (!comment.Worksheet.IsRowVisible(to.Row))
				index++;
			int result = gridRows[index].Near;
			result += GetOffsetInLayoutUnits(to.RowOffset) + borderWidth;
			return result;
		}
		int CalculateBottomPositionOutsideClientBounds(Comment comment, AnchorPoint to, PageGrid gridRows, int borderWidth) {
			IColumnWidthCalculationService columnWidthCalculator = Sheet.Workbook.GetService<IColumnWidthCalculationService>();
			int position = gridRows.Last.Far;
			int start = gridRows.Last.ModelIndex + 1;
			int end = to.Row;
			for (int i = start; i < end; i++) {
				int height = columnWidthCalculator.CalculateRowHeight(comment.Worksheet, i);
				position += height;
			}
			position += GetOffsetInLayoutUnits(to.RowOffset) + borderWidth;
			return position;
		}
		int GetOffsetInLayoutUnits(float valueInModelUnits) {
			return (int)Sheet.Workbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(valueInModelUnits);
		}
		Point CalculateCommentLineEndPoint(Rectangle indicatorBounds, Comment comment, PageGrid gridColumns, PageGrid gridRows, int borderWidth) {
			if (indicatorBounds != Rectangle.Empty)
				return new Point(indicatorBounds.Right, indicatorBounds.Top);
			return CalculateCommentLineEndPointOutside(comment, gridColumns, gridRows, borderWidth);
		}
		Point CalculateCommentLineEndPointOutside(Comment comment, PageGrid gridColumns, PageGrid gridRows, int borderWidth) {
			int commentColumn = comment.Reference.Column;
			int commentRow = comment.Reference.Row;
			AnchorPoint anchor = new AnchorPoint(comment.Worksheet.SheetId, commentColumn, commentRow);
			int x;
			if (commentColumn < gridColumns.ActualFirst.ModelIndex)
				x = CalculateLeftPosition(comment, anchor, gridColumns, borderWidth, true);
			else
				x = CalculateRightPosition(comment, anchor, gridColumns, borderWidth, true);
			int y;
			if (commentRow < gridRows.ActualFirst.ModelIndex)
				y = CalculateTopPosition(comment, anchor, gridRows, borderWidth, true);
			else
				y = CalculateBottomPosition(comment, anchor, gridRows, borderWidth, true);
			DocumentModelUnitToLayoutUnitConverter unitConverter = comment.Workbook.ToDocumentLayoutUnitConverter;
			int width = (int)unitConverter.ToLayoutUnits(AccumulatedOffset.GetOneColumnWidth(comment.Worksheet, anchor.Col)) + borderWidth;
			return new Point(x + width, y);
		}
		#endregion
		private class DrawingBoxFactory : IDrawingObjectVisitor {
			readonly RectangleF bounds;
			public DrawingBoxFactory(RectangleF bounds) {
				this.bounds = bounds;
			}
			public DrawingBox Result { get; private set; }
			#region IDrawingObjectVisitor Members
			void IDrawingObjectVisitor.Visit(Picture picture) {
				Image image = picture.Image.NativeImage;
				RectangleF clipBounds = new RectangleF(0, 0, image.Width, image.Height);
				this.Result = new PictureBox(picture, Rectangle.Round(bounds), Rectangle.Round(clipBounds));
			}
			void IDrawingObjectVisitor.Visit(Chart chart) {
				this.Result = new ChartBox(chart, Rectangle.Round(bounds));
			}
			void IDrawingObjectVisitor.Visit(ModelShape shape) { 
#if DEBUG
				this.Result = new ShapeBox(shape, Rectangle.Round(bounds));
#endif
			}
			public void Visit(GroupShape groupShape) { 
			}
			public void Visit(ConnectionShape connectionShape) {
			}
			#endregion
		}
		protected void GeneratePageHorizontalBorders(Page page) {
			PageBordersCalculator calculator = new HorizontalBordersCalculator(page, null, null);
			calculator.PrintGridLines = PrintGridLines;
			calculator.CalculateBorders();
		}
		protected void GeneratePageVerticalBorders(Page page) {
			GeneratePageVerticalBordersCore(page, VerticalBorderBreaks);
		}
		protected void GeneratePageVerticalBordersNoBreaks(Page page) {
			GeneratePageVerticalBordersCore(page, null);
		}
		void GeneratePageVerticalBordersCore(Page page, RowVerticalBorderBreaks breaks) {
			VerticalBordersCalculator calculator = new VerticalBordersCalculator(page, null, null, breaks);
			calculator.PrintGridLines = PrintGridLines;
			calculator.CalculateBorders();
		}
	}
	public class DrawingObjectLayoutCalculatorWalker : IDrawingObjectVisitor {
		#region Fields
		readonly FinalDocumentLayoutCalculatorBase finalDocumentLayoutCalculator;
		readonly PageGrid gridColumn;
		readonly PageGrid gridRow;
		readonly Page page;
		readonly List<IDrawingObject> drawingObjects;
		#endregion
		public DrawingObjectLayoutCalculatorWalker(List<IDrawingObject> drawingObjects, FinalDocumentLayoutCalculatorBase finalDocumentLayoutCalculator, PageGrid gridColumn, PageGrid gridRow, Page page) {
			this.finalDocumentLayoutCalculator = finalDocumentLayoutCalculator;
			this.drawingObjects = drawingObjects;
			this.gridColumn = gridColumn;
			this.gridRow = gridRow;
			this.page = page;
		}
		public void Walk() {
			drawingObjects.ForEach(delegate(IDrawingObject o) { o.Visit(this); });
		}
		public void Visit(Picture picture) { finalDocumentLayoutCalculator.ProcessPicture(gridColumn, gridRow, page, picture); }
		public void Visit(Chart chart) { finalDocumentLayoutCalculator.ProcessChart(gridColumn, gridRow, page, chart); }
		public void Visit(ModelShape shape) {
			finalDocumentLayoutCalculator.ProcessShape(gridColumn, gridRow, page, shape);
		}
		public void Visit(GroupShape groupShape) {
		}
		public void Visit(ConnectionShape connectionShape) {
		}
	}
	#endregion
}
