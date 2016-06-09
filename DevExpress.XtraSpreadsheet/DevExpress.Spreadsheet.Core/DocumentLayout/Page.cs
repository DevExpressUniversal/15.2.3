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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region Page
	public class Page {
		#region Fields
		readonly DocumentLayout documentLayout;
		readonly Worksheet sheet;
		readonly PageGrid gridColumns;
		readonly PageGrid gridRows;
		readonly IList<SingleCellTextBox> boxes;
		readonly IList<ComplexCellTextBox> complexBoxes;
		readonly Dictionary<int, ComplexCellTextBoxList> rowComplexBoxes;
		readonly List<DrawingBox> drawingBoxes;
		readonly List<CommentBox> commentBoxes;
		readonly List<IndicatorBox> indicatorBoxes;
		Rectangle bounds;
		Rectangle clientBounds;
		List<PageBorderCollection> horizontalBorders;
		List<PageBorderCollection> verticalBorders;
		List<PageBorderCollection> horizontalGridBorders;
		List<PageBorderCollection> verticalGridBorders;
		int index;
		float scaleFactor = 1f;
		Size headingSize;
		int hidenRowsCount = 0;
		int hidenColumnsCount = 0;
		#endregion
		public Page(DocumentLayout documentLayout, Worksheet sheet, PageGrid gridColumns, PageGrid gridRows) {
			Guard.ArgumentNotNull(sheet, "sheet");
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			Guard.ArgumentNotNull(gridColumns, "gridColumns");
			Guard.ArgumentNotNull(gridRows, "gridRows");
			this.documentLayout = documentLayout;
			this.sheet = sheet;
			this.gridColumns = gridColumns;
			this.gridRows = gridRows;
			this.boxes = new DevExpress.XtraSpreadsheet.Utils.ChunkedList<SingleCellTextBox>();
			this.complexBoxes = new ComplexCellTextBoxList();
			this.rowComplexBoxes = new Dictionary<int, ComplexCellTextBoxList>();
			this.drawingBoxes = new List<DrawingBox>();
			this.commentBoxes = new List<CommentBox>();
			this.indicatorBoxes = new List<IndicatorBox>();
			this.horizontalBorders = new List<PageBorderCollection>();
			this.verticalBorders = new List<PageBorderCollection>();
			this.horizontalGridBorders = new List<PageBorderCollection>();
			this.verticalGridBorders = new List<PageBorderCollection>();
			this.hidenRowsCount = gridRows.ActualLast.ModelIndex - gridRows.ActualFirst.ModelIndex + 1 - gridRows.Count;
			this.hidenColumnsCount = gridColumns.ActualLast.ModelIndex - gridColumns.ActualFirst.ModelIndex + 1 - gridColumns.Count;
		}
		#region Properties
		[System.ComponentModel.DefaultValue(1f)]
		protected internal float ScaleFactor { get { return scaleFactor; } set { scaleFactor = value; } }
		public DocumentLayout DocumentLayout { get { return documentLayout; } }
		public Worksheet Sheet { get { return sheet; } }
		public PageGrid GridColumns { get { return gridColumns; } }
		public PageGrid GridRows { get { return gridRows; } }
		public IList<SingleCellTextBox> Boxes { get { return boxes; } }
		public IList<ComplexCellTextBox> ComplexBoxes { get { return complexBoxes; } }
		internal Dictionary<int, ComplexCellTextBoxList> RowComplexBoxes { get { return rowComplexBoxes; } }
		public List<DrawingBox> DrawingBoxes { get { return drawingBoxes; } }
		public List<CommentBox> CommentBoxes { get { return commentBoxes; } }
		public List<IndicatorBox> IndicatorBoxes { get { return indicatorBoxes; } }
		public List<PageBorderCollection> HorizontalBorders { get { return horizontalBorders; } }
		public List<PageBorderCollection> VerticalBorders { get { return verticalBorders; } }
		public List<PageBorderCollection> HorizontalGridBorders { get { return horizontalGridBorders; } }
		public List<PageBorderCollection> VerticalGridBorders { get { return verticalGridBorders; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public int ClientLeft { get { return clientBounds.Left; } }
		public int ClientTop { get { return clientBounds.Top; } }
		public int Index { get { return index; } set { index = value; } }
		public Size HeadingSize { get { return headingSize; } set { headingSize = value; } }
		public int HidenRowsCount { get { return hidenRowsCount; } }
		public int HidenColumnsCount { get { return hidenColumnsCount; } }
		public PageAlignment DesignAlignment { get; set; }
		public ViewPaneType PaneType { get; set; }
		#endregion
		public Rectangle GetHitTestClientBounds() {
			return new Rectangle(bounds.X, bounds.Y, clientBounds.Width, clientBounds.Height);
		}
		public bool IsEmpty() {
			return boxes.Count <= 0 && HorizontalBorders.Count <= 0 && VerticalBorders.Count <= 0;
		}
		public Rectangle CalculateBoxBounds(ICellTextBox box) {
			return box.GetBounds(this);
		}
		public Rectangle CalculateRangeBounds(CellRange range) {
			return CalculateRangeBounds(range.TopLeft, range.BottomRight);
		}
		public Rectangle CalculateRangeBounds(CellPosition topLeft, CellPosition bottomRight) {
			return CalculateRangeBounds(topLeft.Column, topLeft.Row, bottomRight.Column, bottomRight.Row);
		}
		public Rectangle CalculateRangeBounds(int left, int top, int right, int bottom) {
			if (IsBoundsNotIntersectsWithVisibleBounds(gridRows, top, bottom) ||
				IsBoundsNotIntersectsWithVisibleBounds(gridColumns, left, right))
				return Rectangle.Empty;
			int topRow = gridRows.TryCalculateExactOrFarActualItem(top);
			if (topRow < 0)
				topRow = gridRows.ActualLastIndex;
			else if (gridRows[topRow].ModelIndex > bottom) 
				return Rectangle.Empty;
			int bottomRow = gridRows.TryCalculateExactOrNearActualItem(bottom);
			if (bottomRow < 0)
				bottomRow = gridRows.ActualLastIndex;
			int leftColumn = gridColumns.TryCalculateExactOrFarActualItem(left);
			if (leftColumn < 0)
				leftColumn = gridColumns.ActualFirstIndex;
			else if (gridColumns[leftColumn].ModelIndex > right) 
				return Rectangle.Empty;
			int rightColumn = gridColumns.TryCalculateExactOrNearActualItem(right);
			if (rightColumn < 0)
				rightColumn = gridColumns.ActualLastIndex;
			return CalculateBoundsCore(topRow, bottomRow, leftColumn, rightColumn);
		}
		protected internal bool IsBoundsNotIntersectsWithVisibleBounds(PageGrid grid, int nearPosition, int farPosition) {
			if (grid.ActualFirst.ModelIndex > farPosition || grid.ActualLast.ModelIndex < nearPosition)
				return true;
			return false;
		}
		Rectangle CalculateBoundsCore(int topRow, int bottomRow, int leftColumn, int rightColumn) {
			PageGridItem leftColumnItem = gridColumns[leftColumn];
			PageGridItem rightColumnItem = gridColumns[rightColumn];
			PageGridItem topRowItem = gridRows[topRow];
			PageGridItem bottomRowItem = gridRows[bottomRow];
			int x = ClientLeft + leftColumnItem.Near;
			int y = ClientTop + topRowItem.Near;
			int width = rightColumnItem.Far - leftColumnItem.Near;
			int height = bottomRowItem.Far - topRowItem.Near;
			return new Rectangle(x, y, width + 1, height + 1); 
		}
	}
	#endregion
	#region HeaderPage
	public class HeaderPage {
		#region Fields
		protected internal static int InvalidActiveColumnIndex = -1;
		protected internal static int InvalidActiveRowIndex = -1;
		readonly Worksheet sheet;
		Size headersOffset;
		Size gridItemsOffset;
		List<HeaderTextBox> columnBoxes;
		List<HeaderTextBox> rowBoxes;
		HeaderTextBox selectAllButton;
		bool isContentValid;
		int activeColumnIndex = InvalidActiveColumnIndex;
		int activeRowIndex = InvalidActiveRowIndex;
		#endregion
		public HeaderPage(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
		}
		public HeaderPage(Page page, Size headerOffset, Size gridItemsOffset) {
			this.sheet = page.Sheet;
			GenerateContent(new List<PageGrid> { page.GridColumns }, new List<PageGrid> { page.GridRows }, headerOffset, gridItemsOffset, false);
		}
		#region Properties
		public Rectangle Bounds { get; private set; }
		public HeaderTextBox SelectAllButton { get { return selectAllButton; } }
		public List<HeaderTextBox> ColumnBoxes { get { return columnBoxes; } }
		public List<HeaderTextBox> RowBoxes { get { return rowBoxes; } }
		public bool IsContentValid { get { return isContentValid; } }
		public int ActiveColumnIndex { get { return activeColumnIndex; } set { activeColumnIndex = value; } }
		public int ActiveRowIndex { get { return activeRowIndex; } set { activeRowIndex = value; } }
		public Size HeadersOffset { get { return headersOffset; } }
		#endregion
		public bool IsValid() {
			return selectAllButton != null && columnBoxes != null && rowBoxes != null && !Bounds.IsEmpty;
		}
		public void GenerateContent(List<PageGrid> columnHeaderGrids, List<PageGrid> rowHeaderGrids, Size headerOffset, Size gridItemsOffset, bool useOffsetForFirstItems) {
			this.headersOffset = headerOffset;
			this.gridItemsOffset = gridItemsOffset;
			GenerateBoxes(columnHeaderGrids, rowHeaderGrids, useOffsetForFirstItems);
			Update();
		}
		public bool ContainsPoint(Point logicPoint) {
			if (logicPoint.X < this.selectAllButton.Bounds.Left || logicPoint.Y < this.selectAllButton.Bounds.Top)
				return false;
			if (logicPoint.X > this.selectAllButton.Bounds.Right && logicPoint.Y > this.selectAllButton.Bounds.Bottom)
				return false;
			return (logicPoint.X <= Bounds.Right && logicPoint.Y <= Bounds.Bottom);
		}
		public void InvalidateContent() {
			this.isContentValid = false;
			this.activeColumnIndex = -1;
			this.activeRowIndex = -1;
		}
		public HeaderTextBox GetHeaderBox(Point logicalPoint) {
			if (selectAllButton.Bounds.Contains(logicalPoint))
				return selectAllButton;
			foreach (HeaderTextBox box in columnBoxes)
				if (box.Bounds.Contains(logicalPoint))
					return box;
			foreach (HeaderTextBox box in rowBoxes)
				if (box.Bounds.Contains(logicalPoint))
					return box;
			return null;
		}
		void GenerateBoxes(List<PageGrid> columnHeaderGrids, List<PageGrid> rowHeaderGrids, bool useOffsetForFirstItems) {
			this.columnBoxes = new List<HeaderTextBox>();
			this.rowBoxes = new List<HeaderTextBox>();
			int currentRight = useOffsetForFirstItems ? headersOffset.Width : 0;
			int currentBottom = useOffsetForFirstItems ? headersOffset.Height : 0;
			currentRight += gridItemsOffset.Width;
			currentBottom += gridItemsOffset.Height;
			this.selectAllButton = useOffsetForFirstItems ? new HeaderTextBox(Rectangle.FromLTRB(gridItemsOffset.Width, gridItemsOffset.Height, currentRight, currentBottom), HeaderBoxType.SelectAllButton, -1, null, null) : null;
			HeaderTextBox previous = selectAllButton;
			foreach (PageGrid columnHeaderGrid in columnHeaderGrids)
				for (int i = columnHeaderGrid.ActualFirstIndex; i <= columnHeaderGrid.ActualLastIndex; i++) {
					PageGridItem item = columnHeaderGrid[i];
					previous = new HeaderTextBox(Rectangle.FromLTRB(currentRight, gridItemsOffset.Height, item.Extent + currentRight, currentBottom), HeaderBoxType.ColumnHeader, item.ModelIndex, previous, columnBoxes);
					columnBoxes.Add(previous);
					currentRight += item.Extent;
				}
			previous = selectAllButton;
			foreach (PageGrid rowHeaderGrid in rowHeaderGrids)
				for (int i = rowHeaderGrid.ActualFirstIndex; i <= rowHeaderGrid.ActualLastIndex; i++) {
					PageGridItem item = rowHeaderGrid[i];
					previous = new HeaderTextBox(Rectangle.FromLTRB(gridItemsOffset.Width, currentBottom, gridItemsOffset.Width + headersOffset.Width, item.Extent + currentBottom), HeaderBoxType.RowHeader, item.ModelIndex, previous, rowBoxes);
					rowBoxes.Add(previous);
					currentBottom += item.Extent;
				}
			Bounds = Rectangle.FromLTRB(0, 0, currentRight, currentBottom);
		}
		public void Update() {
			if (isContentValid || columnBoxes == null || rowBoxes == null)
				return;
			UpdateBoxes(columnBoxes, GetColumnText, GetColumnBoxSelectType);
			UpdateBoxes(rowBoxes, GetRowText, GetRowBoxSelectType);
			this.isContentValid = true;
		}
		void UpdateBoxes(List<HeaderTextBox> boxes, GetTextDelegate GetText, GetBoxSelectTypeDelegate GetBoxSelectType) {
			int count = boxes.Count;
			for (int i = 0; i < count; i++) {
				HeaderTextBox current = boxes[i];
				int index = current.ModelIndex;
				current.Text = GetText(index);
				current.SelectType = GetBoxSelectType(index);
			}
		}
		string GetColumnText(int index) {
			if (sheet.Workbook.Properties.UseR1C1ReferenceStyle)
				return (index + 1).ToString();
			return CellReferenceParser.ColumnIndexToString(index);
		}
		string GetRowText(int index) {
			return (index + 1).ToString();
		}
		HeaderBoxSelectType GetColumnBoxSelectType(int modelIndex) {
			return GetBoxSelectType(modelIndex, activeColumnIndex, IsColumnBoxSingleType);
		}
		bool IsColumnBoxSingleType(CellRange range, int index) {
			return index >= range.TopLeft.Column && index <= range.BottomRight.Column;
		}
		HeaderBoxSelectType GetRowBoxSelectType(int modelIndex) {
			return GetBoxSelectType(modelIndex, activeRowIndex, IsRowBoxSingleType);
		}
		bool IsRowBoxSingleType(CellRange range, int index) {
			return index >= range.TopLeft.Row && index <= range.BottomRight.Row;
		}
		HeaderBoxSelectType GetBoxSelectType(int modelIndex, int activeBoxIndex, IsBoxSingleTypeDelegate IsBoxSingleType) {
			if (sheet.Workbook.BehaviorOptions.Selection.HideSelection)
				return HeaderBoxSelectType.None;
			SheetViewSelection selection = sheet.Selection;
			IList<CellRange> selectedRanges = selection.SelectedRanges;
			int count = selectedRanges.Count;
			for (int i = 0; i < count; i++) {
				CellRange currentRange = selectedRanges[i];
				if (modelIndex == activeBoxIndex)
					return HeaderBoxSelectType.Active;
				if (!selection.IsDrawingSelected && IsBoxSingleType(currentRange, modelIndex))
					return HeaderBoxSelectType.Single;
			}
			return HeaderBoxSelectType.None;
		}
		delegate string GetTextDelegate(int index);
		delegate HeaderBoxSelectType GetBoxSelectTypeDelegate(int index);
		delegate bool IsBoxSingleTypeDelegate(CellRange range, int index);
	}
	#endregion
	#region GroupPage
	public class GroupItemsPage {
		#region const
		const int groupPixelItemSize = 20;
		const int buttonPixelSize = 18;
		const int groupPixelLineWidth = 2;
		const int groupPixelTailLenght = 6;
		#endregion
		#region static
		public static int CalculateGroupHeightInPixels(Worksheet sheet) {
			int columnItemsCount = CalculateColumnItemsCount(sheet);
			return CalculateGroupHeightCore(columnItemsCount, groupPixelItemSize);
		}
		public static int CalculateGroupWidthInPixels(Worksheet sheet) {
			int rowItemsCount = CalculateRowItemsCount(sheet);
			return CalculateGroupWidthCore(rowItemsCount, groupPixelItemSize);
		}
		static int CalculateColumnItemsCount(Worksheet sheet) {
			return sheet.Properties.FormatProperties.OutlineLevelCol + 1;
		}
		static int CalculateGroupHeightCore(int columnItemsCount, int groupItemSize) {
			return columnItemsCount > 1 ? columnItemsCount * groupItemSize : 0;
		}
		static int CalculateRowItemsCount(Worksheet sheet) {
			return sheet.Properties.FormatProperties.OutlineLevelRow + 1;
		}
		static int CalculateGroupWidthCore(int rowItemsCount, int groupItemSize) {
			return rowItemsCount > 1 ? rowItemsCount * groupItemSize : 0;
		}
		static Size InvalidGroupOffset = new Size(-1, -1);
		#endregion
		#region fields
		readonly Worksheet sheet;
		readonly Size headerOffset;
		readonly int groupItemSize;
		readonly int buttonSize;
		readonly int groupLineWidth;
		readonly int groupTailLenght;
		List<PageGrid> columnGrids;
		List<PageGrid> rowGrids;
		PageGrid rowLevelGrid;
		PageGrid colLevelGrid;
		GroupList rowGroupItemList;
		GroupList columnGroupItemList;
		Size groupOffset;
		Rectangle bounds;
		Rectangle columnBounds;
		Rectangle rowBounds;
		List<Point> dots;
		List<OutlineLevelBox> buttons;
		List<OutlineLevelLine> lines;
		OutlineLevelBox activeBox;
		#endregion
		public GroupItemsPage(Worksheet sheet, Size headerOffset) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.headerOffset = headerOffset;
			this.groupItemSize = sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnits(groupPixelItemSize, DocumentModel.Dpi);
			this.buttonSize = sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnits(buttonPixelSize, DocumentModel.Dpi);
			this.groupLineWidth = sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnits(groupPixelLineWidth, DocumentModel.Dpi);
			this.groupTailLenght = sheet.Workbook.LayoutUnitConverter.PixelsToLayoutUnits(groupPixelTailLenght, DocumentModel.Dpi);
			this.groupOffset = InvalidGroupOffset;
		}
		public GroupItemsPage(Page page, Size headerOffset)
			: this(page.Sheet, headerOffset) {
			this.columnGrids = new List<PageGrid> { page.GridColumns };
			this.rowGrids = new List<PageGrid> { page.GridRows };
			GenerateContent();
		}
		#region Properties
		public Size HeaderOffset { get { return headerOffset; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle ColumnBounds { get { return columnBounds; } }
		public Rectangle RowBounds { get { return rowBounds; } }
		public List<OutlineLevelBox> Buttons { get { return buttons; } }
		public List<Point> Dots { get { return dots; } }
		public List<OutlineLevelLine> Lines { get { return lines; } }
		public OutlineLevelBox ActiveBox { get { return activeBox; } set { SetActiveButton(value); } }
		public Size GroupItemsOffset {
			get {
				if (groupOffset == InvalidGroupOffset)
					groupOffset = CalculateGroupItemsOffset();
				return groupOffset;
			}
		}
		#endregion
		public void GenerateContent(List<PageGrid> columnGrids, List<PageGrid> rowGrids) {
			this.columnGrids = columnGrids;
			this.rowGrids = rowGrids;
			GenerateContent();
		}
		public void GenerateContent() {
			CalculateGroups();
			CalculateBounds();
			buttons = new List<OutlineLevelBox>();
			CreateHeaderButtons();
			CreateGroupsElements();
		}
		public bool ContainsPoint(Point logicPoint) {
			return columnBounds.Contains(logicPoint) || rowBounds.Contains(logicPoint);
		}
		public OutlineLevelBox GetGroupBox(Point logicalPoint) {
			foreach (OutlineLevelBox button in buttons)
				if (button.Bounds.Contains(logicalPoint))
					return button;
			foreach (OutlineLevelLine line in lines)
				if (line.Contains(logicalPoint))
					return line.Button;
			return null;
		}
		public bool IsValid() {
			return sheet.Properties.FormatProperties.OutlineLevelCol > 0 || sheet.Properties.FormatProperties.OutlineLevelRow > 0;
		}
		void SetActiveButton(OutlineLevelBox newButton) {
			if (activeBox != null)
				activeBox.OutlineLevelBoxSelectType = OutlineLevelBoxSelectType.None;
			activeBox = newButton;
		}
		void CalculateGroups() {
			GroupCollector collector = new ColumnGroupCollector(sheet);
			if (columnGrids.Count > 0)
				columnGroupItemList = collector.CollectGroups(columnGrids[0].ActualFirst.ModelIndex, columnGrids[columnGrids.Count - 1].ActualLast.ModelIndex);
			else columnGroupItemList = new GroupList(sheet, false);
			collector = new RowGroupCollector(sheet);
			if (rowGrids.Count > 0)
				rowGroupItemList = collector.CollectGroups(rowGrids[0].ActualFirst.ModelIndex, rowGrids[rowGrids.Count - 1].ActualLast.ModelIndex);
			else rowGroupItemList = new GroupList(sheet, true);
			sheet.Properties.FormatProperties.OutlineLevelCol = Math.Max(columnGroupItemList.MaxLevel, sheet.Properties.FormatProperties.OutlineLevelCol);
			sheet.Properties.FormatProperties.OutlineLevelRow = Math.Max(rowGroupItemList.MaxLevel, sheet.Properties.FormatProperties.OutlineLevelRow);
		}
		void CalculateBounds() {
			int gridWidth = columnGrids.Count > 0 ? GetGridsSize(columnGrids) : 0;
			int gridHeight = rowGrids.Count > 0 ? GetGridsSize(rowGrids) : 0;
			CreateLevelGrids();
			Size fullOffset = new Size(groupOffset.Width + headerOffset.Width, groupOffset.Height + headerOffset.Height);
			bounds = new Rectangle(0, 0,
				groupOffset.Height == 0 ? groupOffset.Width : gridWidth + fullOffset.Width,
				groupOffset.Width == 0 ? groupOffset.Height : gridHeight + fullOffset.Height);
			columnBounds = new Rectangle(0, 0, bounds.Width, groupOffset.Height);
			rowBounds = new Rectangle(0, groupOffset.Height, groupOffset.Width, bounds.Height - groupOffset.Height);
		}
		int GetGridsSize(List<PageGrid> grids) {
			int result = 0;
			foreach (PageGrid grid in grids)
				result += grid.ActualLast.Far;
			return result;
		}
		public Size CalculateGroupItemsOffset() {
			int columnItemsCount = CalculateColumnItemsCount(sheet);
			int rowItemsCount = CalculateRowItemsCount(sheet);
			int height = CalculateGroupHeightCore(columnItemsCount, groupItemSize);
			int width = CalculateGroupWidthCore(rowItemsCount, groupItemSize);
			return new Size(width, height);
		}
		void CreateLevelGrids() {
			int columnItemsCount = CalculateColumnItemsCount(sheet);
			int rowItemsCount = CalculateRowItemsCount(sheet);
			CreateLevelGridsCore(columnItemsCount, rowItemsCount);
		}
		void CreateLevelGridsCore(int columnItemsCount, int rowItemsCount) {
			colLevelGrid = new PageGrid();
			for (int i = 0; i < columnItemsCount; i++)
				colLevelGrid.Add(new PageGridItem() { Near = i * groupItemSize, Extent = groupItemSize, Index = i, ModelIndex = i });
			rowLevelGrid = new PageGrid();
			for (int i = 0; i < rowItemsCount; i++)
				rowLevelGrid.Add(new PageGridItem() { Near = i * groupItemSize, Extent = groupItemSize, Index = i, ModelIndex = i });
		}
		void CreateHeaderButtons() {
			if (!sheet.ActiveView.ShowRowColumnHeaders || sheet.Properties.FormatProperties.OutlineLevelCol == 0 && sheet.Properties.FormatProperties.OutlineLevelRow == 0)
				return;
			if (sheet.Properties.FormatProperties.OutlineLevelCol > 0 && sheet.Workbook.ViewOptions.ShowRowHeaders)
				for (int i = 0; i <= sheet.Properties.FormatProperties.OutlineLevelCol; i++)
					buttons.Add(new OutlineLevelBox(columnGroupItemList.Groups, i + 1, new Rectangle(groupOffset.Width, colLevelGrid[i].Near, headerOffset.Width, buttonSize), false));
			if (sheet.Properties.FormatProperties.OutlineLevelRow > 0 && sheet.Workbook.ViewOptions.ShowColumnHeaders)
				for (int i = 0; i <= sheet.Properties.FormatProperties.OutlineLevelRow; i++)
					buttons.Add(new OutlineLevelBox(rowGroupItemList.Groups, i + 1, new Rectangle(rowLevelGrid[i].Near, groupOffset.Height, buttonSize, headerOffset.Height), true));
		}
		void CreateGroupsElements() {
			lines = new List<OutlineLevelLine>();
			dots = new List<Point>();
			DrawingGroupProcessor processor = null;
			if (rowGrids.Count > 0) {
				processor = new RowDrawingGroupProcessor(sheet, rowGroupItemList.DotIndices, rowGrids, rowLevelGrid, buttonSize, groupLineWidth, groupTailLenght);
				foreach (GroupItem group in rowGroupItemList.Groups)
					CreateGroupElements(group, processor);
				dots.AddRange(processor.GetDots());
			}
			if (columnGrids.Count > 0) {
				processor = new ColDrawingGroupProcessor(sheet, columnGroupItemList.DotIndices, columnGrids, colLevelGrid, buttonSize, groupLineWidth, groupTailLenght);
				foreach (GroupItem group in columnGroupItemList.Groups)
					CreateGroupElements(group, processor);
				dots.AddRange(processor.GetDots());
			}
		}
		void CreateGroupElements(GroupItem group, DrawingGroupProcessor processor) {
			if (group.Hiden)
				return;
			processor.CalculateDrawingElements(group);
			if (processor.GroupEndButton != null)
				buttons.Add(processor.GroupEndButton);
			if (!group.Collapsed && processor.Line != null)
				lines.Add(processor.Line);
		}
	}
	#region OutlineLevelBoxSelectType
	public enum OutlineLevelBoxSelectType {
		None = 0,
		Hovered = 1,
		Pressed = 2
	}
	#endregion
	#region OutlineLevelBox
	public class OutlineLevelBox {
		#region const
		const string expandedText = "-";
		const string colapsedText = "+";
		#endregion
		#region fields
		readonly bool isRow;
		readonly int level;
		readonly Rectangle bounds;
		readonly List<GroupItem> groups;
		OutlineLevelBoxSelectType outlineLevelBoxSelectType;
		#endregion
		public OutlineLevelBox(GroupItem group, Rectangle bounds, bool isRow)
			: this(new List<GroupItem> { group }, 0, bounds, isRow) {
		}
		public OutlineLevelBox(List<GroupItem> groups, int level, Rectangle bounds, bool isRow) {
			this.groups = groups;
			this.level = level;
			this.bounds = bounds;
			outlineLevelBoxSelectType = OutlineLevelBoxSelectType.None;
			this.isRow = isRow;
		}
		#region Properties
		public string Text { get { return GetText(); } }
		public Rectangle Bounds { get { return bounds; } }
		public bool IsHeaderButton { get { return level != 0; } }
		public bool IsRowButton { get { return isRow; } }
		public bool IsExpandedButton { get { return level == 0 && !groups[0].Collapsed; } }
		public bool IsCollapsedButton { get { return level == 0 && groups[0].Collapsed; } }
		public OutlineLevelBoxSelectType OutlineLevelBoxSelectType {
			get { return outlineLevelBoxSelectType; }
			set { outlineLevelBoxSelectType = value; }
		}
		protected internal List<GroupItem> Groups { get { return groups; } }
		protected internal int Level { get { return level; } }
		#endregion
		public List<GroupItem> GetExpandingGroups() {
			if (level == 0)
				return groups[0].Collapsed ? groups : new List<GroupItem>();
			else {
				List<GroupItem> result = new List<GroupItem>();
				foreach (GroupItem group in groups)
					if (level > group.Level)
						result.Add(group);
				return result;
			}
		}
		public List<GroupItem> GetCollapsingGroups() {
			if (level == 0)
				return groups[0].Collapsed ? new List<GroupItem>() : groups;
			else {
				List<GroupItem> result = new List<GroupItem>();
				foreach (GroupItem group in groups)
					if (level <= group.Level)
						result.Add(group);
				return result;
			}
		}
		string GetText() {
			if (level > 0)
				return level.ToString();
			return groups[0].Collapsed ? colapsedText : expandedText;
		}
	}
	#endregion
	#region OutlineLevelLine
	public class OutlineLevelLine {
		#region const
		readonly Size hotZoneInflateSize = new Size(2, 2);
		#endregion
		#region fields
		Rectangle lineBounds;
		Rectangle hotZoneBounds;
		Size tailSize;
		Point tailPoint1;
		Point tailPoint2;
		Point linePoint1;
		Point linePoint2;
		OutlineLevelBox button;
		#endregion
		public OutlineLevelLine(Rectangle lineBounds, Size tailSize, OutlineLevelBox button) {
			this.tailSize = tailSize;
			this.lineBounds = lineBounds;
			this.button = button;
			InitializeBounds();
		}
		#region Properties
		public Point TailPoint1 { get { return tailPoint1; } }
		public Point TailPoint2 { get { return tailPoint2; } }
		public Point LinePoint1 { get { return linePoint1; } }
		public Point LinePoint2 { get { return linePoint2; } }
		public OutlineLevelBox Button { get { return button; } }
		#endregion
		void InitializeBounds() {
			if (button == null)
				return;
			GroupItem group = button.Groups[0];
			CalculateTailBounds(group.RowGroup, group.ButtonBeforeStart);
			CalculateBounds(group.RowGroup, group.ButtonBeforeStart);
		}
		void CalculateTailBounds(bool row, bool buttonBeforeStart) {
			Point location = lineBounds.Location;
			if (buttonBeforeStart) {
				if (row)
					location.Y = lineBounds.Bottom - tailSize.Height;
				else
					location.X = lineBounds.Right - tailSize.Width;
			}
			tailPoint1 = location;
			tailPoint2 = location;
			if (row)
				tailPoint2.Offset(tailSize.Width, 0);
			else
				tailPoint2.Offset(0, tailSize.Height);
		}
		void CalculateBounds(bool row, bool buttonBeforeStart) {
			Size offset = Size.Empty;
			Rectangle buttonBounds = button.Bounds;
			if (buttonBounds.Width > 0 && buttonBounds.Height > 0) {
				if (row)
					offset.Height = (buttonBeforeStart ? lineBounds.Top - buttonBounds.Bottom : buttonBounds.Top - lineBounds.Bottom) + 1;
				else
					offset.Width = (buttonBeforeStart ? lineBounds.Left - buttonBounds.Right : buttonBounds.Left - lineBounds.Right) + 1;
			}
			Rectangle bounds = new Rectangle(lineBounds.X, lineBounds.Y, lineBounds.Width + offset.Width, lineBounds.Height + offset.Height);
			if (buttonBeforeStart) {
				bounds.X -= offset.Width;
				bounds.Y -= offset.Height;
			}
			linePoint1 = bounds.Location;
			linePoint2 = new Point(row ? bounds.Left : bounds.Right, row ? bounds.Bottom : bounds.Top);
			hotZoneBounds = bounds;
			hotZoneBounds.Inflate(hotZoneInflateSize);
		}
		public bool Contains(Point logicalPoint) {
			return hotZoneBounds.Contains(logicalPoint);
		}
	}
	#endregion
	#endregion
	#region PageGridItem
	public struct PageGridItem {
		int near;
		int extent;
		int index; 
		int modelIndex; 
		public int Near { get { return near; } set { near = value; } }
		public int Extent { get { return extent; } set { extent = value; } }
		public int Far { get { return near + extent; } }
		public int Index { get { return index; } set { index = value; } }
		public int ModelIndex { get { return modelIndex; } set { modelIndex = value; } }
	}
	#endregion
	#region PageGrid
	public class PageGrid {
		readonly List<PageGridItem> innerList = new List<PageGridItem>();
		readonly Dictionary<int, int> modelToIndexTable = new Dictionary<int, int>();
		int actualFirstIndex;
		int actualLastIndex;
		public PageGridItem this[int index] { get { return innerList[index]; } }
		public int Count { get { return innerList.Count; } }
		public PageGridItem First { get { return this[0]; } }
		public PageGridItem Last { get { return this[Count - 1]; } }
		public PageGridItem ActualFirst { get { return this[actualFirstIndex]; } }
		public PageGridItem ActualLast { get { return this[actualLastIndex]; } }
		public int ActualFirstIndex { get { return actualFirstIndex; } }
		public int ActualLastIndex { get { return actualLastIndex; } }
		public int ActualCount { get { return actualLastIndex - actualFirstIndex + 1; } }
		public void Add(PageGridItem item) {
			actualLastIndex = Count;
			innerList.Add(item);
			modelToIndexTable.Add(item.ModelIndex, item.Index);
		}
		public void AddNear(int index, int value) {
			PageGridItem column = this[index];
			column.Near += value;
			innerList[index] = column;
		}
		public int CalculateIndex(int modelIndex) {
			return modelToIndexTable[modelIndex];
		}
		public int TryCalculateIndex(int modelIndex) {
			int result;
			if (modelToIndexTable.TryGetValue(modelIndex, out result))
				return result;
			else
				return -1;
		}
		public int TryCalculateActualIndex(int modelIndex) {
			int index = TryCalculateIndex(modelIndex);
			if (index < 0)
				return -1;
			return GetActualIndex(index);
		}
		int GetActualIndex(int index) {
			if (index > actualLastIndex)
				return actualLastIndex;
			if (index < actualFirstIndex)
				return actualFirstIndex;
			return index;
		}
		public int CalculateExactOrNearItem(int modelIndex) {
			int result = TryCalculateIndex(modelIndex);
			if (result >= 0)
				return result;
			else
				return LookupNearItem(modelIndex);
		}
		public int CalculateExactOrFarItem(int modelIndex) {
			int result = TryCalculateIndex(modelIndex);
			if (result >= 0)
				return result;
			else
				return LookupFarItem(modelIndex);
		}
		public int TryCalculateExactOrNearActualItem(int modelIndex) {
			int index = CalculateExactOrNearItem(modelIndex);
			return index < 0 ? -1 : GetActualIndex(index);
		}
		public int TryCalculateExactOrFarActualItem(int modelIndex) {
			int index = CalculateExactOrFarItem(modelIndex);
			return index < 0 ? -1 : GetActualIndex(index);
		}
		public int LookupItem(int modelIndex) {
			return Algorithms.BinarySearch(innerList, new PageGridItemModelIndexComparable(modelIndex));
		}
		public int LookupItem(int modelIndex, int from, int to) {
			return Algorithms.BinarySearch(innerList, new PageGridItemModelIndexComparable(modelIndex), from, to);
		}
		public int LookupItemIndexByPosition(int position) {
			return Algorithms.BinarySearch(innerList, new PageGridItemPositionComparable(position));
		}
		public int LookupItemModelIndexByPosition(int position) {
			int index = LookupItemIndexByPosition(position);
			if (index >= 0)
				return innerList[index].ModelIndex;
			return -1;
		}
		public int LookupNearItem(int modelIndex) {
			return LookupNearestItemCore(modelIndex, -1);
		}
		public int LookupFarItem(int modelIndex) {
			return LookupNearestItemCore(modelIndex, 0);
		}
		int LookupNearestItemCore(int modelColumnIndex, int correction) {
			int index = LookupItem(modelColumnIndex);
			if (index < 0) {
				index = ~index;
				if (index >= Count)
					return -1; 
				return index + correction;
			}
			else
				return index;
		}
		public PageGrid OffsetGrid(int offset) {
			PageGrid newGrid = new PageGrid();
			for (int i = 0; i < this.Count; i++) {
				int near = this[i].Near + offset;
				PageGridItem item = new PageGridItem();
				item.Near = near;
				item.Extent = this[i].Extent;
				item.ModelIndex = this[i].ModelIndex;
				item.Index = i;
				newGrid.Add(item);
			}
			if (actualFirstIndex >= newGrid.Count)
				newGrid.actualFirstIndex = newGrid.Count - 1;
			else
				newGrid.actualFirstIndex = actualFirstIndex;
			if (actualLastIndex >= newGrid.Count)
				newGrid.actualLastIndex = newGrid.Count - 1;
			else
				newGrid.actualLastIndex = actualLastIndex;
			return newGrid;
		}
		public void DetectActualRange(int firstModelIndex, int lastModelIndex) {
			actualFirstIndex = LookupFarItem(firstModelIndex);
			if (actualFirstIndex < 0)
				actualFirstIndex = Count - 1;
			if (firstModelIndex == lastModelIndex)
				actualLastIndex = actualFirstIndex;
			else {
				actualLastIndex = LookupNearItem(lastModelIndex);
				if (actualLastIndex < 0)
					actualLastIndex = Count - 1;
			}
		}
		public void SetActualIndicies(int firstIndex, int lastIndex) {
			actualFirstIndex = firstIndex;
			actualLastIndex = lastIndex;
		}
	}
	#endregion
	#region PageGridItemModelIndexComparable
	public class PageGridItemModelIndexComparable : IComparable<PageGridItem> {
		readonly int modelIndex;
		public PageGridItemModelIndexComparable(int modelIndex) {
			this.modelIndex = modelIndex;
		}
		#region IComparable<PageGridItem> Members
		public int CompareTo(PageGridItem other) {
			return -Comparer<int>.Default.Compare(modelIndex, other.ModelIndex);
		}
		#endregion
	}
	#endregion
	#region PageGridItemPositionComparable
	public class PageGridItemPositionComparable : IComparable<PageGridItem> {
		#region Fields
		int position;
		#endregion
		public PageGridItemPositionComparable(int position) {
			this.position = position;
		}
		#region IComparable<PageGridItem> Members
		public int CompareTo(PageGridItem other) {
			if (position < other.Near)
				return 1;
			if (position >= other.Far)
				return -1;
			return 0;
		}
		#endregion
	}
	#endregion
	#region ComplexBoxColecction
	public class ComplexCellTextBoxList : DevExpress.XtraSpreadsheet.Utils.ChunkedList<ComplexCellTextBox> {
		public ComplexCellTextBoxList() {
		}
	}
	#endregion
	#region PageAlignment
	public enum PageAlignment {
		One,
		Top,
		Bottom,
		Left,
		Right,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}
	#endregion
}
