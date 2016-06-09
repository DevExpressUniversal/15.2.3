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

using DevExpress.XtraSpreadsheet.Layout;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DrawingGroupProcessor
	public abstract class DrawingGroupProcessor {
		#region fields
		GroupItem group;
		Worksheet sheet;
		List<int> dotIndices;
		List<PageGrid> grids;
		PageGrid levelGrid;
		int buttonSize;
		int groupLineWidth;
		int groupTailLenght;
		int lastModelIndex;
		int firstModelIndex;
		#endregion
		protected DrawingGroupProcessor(Worksheet sheet, List<int> dotIndices, List<PageGrid> grids, PageGrid levelGrid, int buttonSize, int groupLineWidth, int groupTailLenght) {
			this.sheet = sheet;
			this.dotIndices = dotIndices;
			this.grids = grids;
			this.levelGrid = levelGrid;
			this.buttonSize = buttonSize;
			this.groupLineWidth = groupLineWidth;
			this.groupTailLenght = groupTailLenght;
			this.firstModelIndex = grids[0].ActualFirst.ModelIndex;
			this.lastModelIndex = grids[grids.Count - 1].ActualLast.ModelIndex;
		}
		#region Properties
		protected PageGridItem StartItem { get; set; }
		protected PageGridItem EndItem { get; set; }
		protected Worksheet Sheet { get { return sheet; } }
		protected PageGridItem ButtonItem { get; set; }
		protected PageGrid FirstGrid { get { return grids[0]; } }
		protected PageGrid LastGrid { get { return grids[grids.Count - 1]; } }
		protected PageGridItem LevelGridItem { get; set; }
		public OutlineLevelBox GroupEndButton { get; set; }
		public OutlineLevelLine Line { get; set; }
		#endregion
		public void CalculateDrawingElements(GroupItem group) {
			this.group = group;
			int buttonCorrection = group.ButtonBeforeStart ? 1 : 0;
			if (group.Start > lastModelIndex + buttonCorrection || group.End + 1 < firstModelIndex + buttonCorrection) {
				GroupEndButton = null;
				Line = null;
				return;
			}
			ButtonItem = GetButtonGridItem();
			LevelGridItem = levelGrid[group.Level - 1];
			GroupEndButton = CreateGroupEndButton(buttonSize);
			if (!group.Collapsed) {
				StartItem = GetStartLineGridItem();
				EndItem = GetEndLineGridItem();
				if (GroupIsHiden()) {
					GroupEndButton = null;
					return;
				}
				Rectangle bounds = GetLineBounds(groupTailLenght, groupLineWidth);
				Size tailSize = CalculateTailSize();
				Line = new OutlineLevelLine(bounds, tailSize, GroupEndButton);
			}
			else Line = null;
		}
		public List<Point> GetDots() {
			List<Point> result = new List<Point>();
			if (dotIndices.Count == 0)
				return result;
			foreach (PageGrid grid in grids) {
				for (int i = grid.ActualFirstIndex; i <= grid.ActualLastIndex; i++) {
					PageGridItem item = grid[i];
					int modelIndex = item.ModelIndex;
					if (dotIndices.Contains(modelIndex)) {
						PageGridItem levelItem = levelGrid[GetOutlineLevel(modelIndex)];
						result.Add(GetDotPoint(item, levelItem));
					}
				}
			}
			return result;
		}
		OutlineLevelBox CreateGroupEndButton(int buttonSize) {
			bool invisibleButton = ButtonItem.Extent == 0 || ButtonItem.ModelIndex != group.ButtonPosition;
			int buttonOffset = (LevelGridItem.Extent - buttonSize) / 2;
			int buttonPageItemOffset = invisibleButton ? 0 : (ButtonItem.Extent - buttonSize) / 2;
			Point location = GetButtonLocation(buttonOffset, buttonPageItemOffset);
			OutlineLevelBox button = new OutlineLevelBox(group, new Rectangle(location, invisibleButton ? Size.Empty : new Size(buttonSize, buttonSize)), group.RowGroup);
			return button;
		}
		PageGridItem GetButtonGridItem() {
			foreach (PageGrid grid in grids) {
				int index = grid.LookupFarItem(group.ButtonPosition);
				if (index >= 0) {
					PageGridItem item = grid[index];
					int modelIndex = item.ModelIndex;
					if (modelIndex != group.ButtonPosition && !IsCollapsed(modelIndex)) {
						item = grid.ActualFirst;
						item.Extent = 0;
					}
					if (item.Index < grid.ActualFirstIndex)
						item.Extent = 0;
					return item;
				}
			}
			return grids.Count > 0 ? grids[0].ActualFirst : new PageGridItem();
		}
		bool GroupIsHiden() {
			if (StartItem.ModelIndex == group.Start || EndItem.ModelIndex == group.End)
				return false;
			if (StartItem.ModelIndex > EndItem.ModelIndex && group.ButtonPosition != ButtonItem.ModelIndex)
				return true;
			for (int i = group.Start; i <= group.End; i++)
				if (!IsHiden(i))
					return false;
			return true;
		}
		PageGridItem GetStartLineGridItem() {
			if (grids.Count <= 0)
				return new PageGridItem();
			int lineStartModelIndex = group.Start;
			if (lineStartModelIndex <= firstModelIndex)
				return FirstGrid.ActualFirst;
			for (int i = 0; i < grids.Count; i++) {
				PageGrid grid = grids[i];
				int index = grid.CalculateExactOrFarItem(lineStartModelIndex);
				if (index >= 0) {
					PageGridItem item = grid[index];
					if (IsGridItemVisible(grid, item) && item.ModelIndex <= group.End)
						return item;
				}
			}
			return LastGrid.ActualFirst;
		}
		PageGridItem GetEndLineGridItem() {
			if (grids.Count <= 0)
				return new PageGridItem();
			int lineEndModelIndex = group.End;
			if (lineEndModelIndex >= lastModelIndex)
				return LastGrid.ActualLast;
			for (int i = 0; i < grids.Count; i++) {
				PageGrid grid = grids[i];
				int index = grid.CalculateExactOrNearItem(lineEndModelIndex);
				if (index >= 0) {
					PageGridItem item = grid[index];
					if (IsGridItemVisible(grid, item) && item.ModelIndex >= group.Start)
						return item;
				}
			}
			return FirstGrid.ActualLast;
		}
		bool IsGridItemVisible(PageGrid grid, PageGridItem item) {
			int index = item.Index;
			return index >= grid.ActualFirstIndex && index <= grid.ActualLastIndex;
		}
		Size CalculateTailSize() {
			bool haveTail = IsHaveTail();
			if (!haveTail)
				return Size.Empty;
			return group.RowGroup ? new Size(groupTailLenght, groupLineWidth) : new Size(groupLineWidth, groupTailLenght);
		}
		bool IsHaveTail() {
			bool buttonBeforeStart = group.ButtonBeforeStart;
			if (buttonBeforeStart)
				return EndItem.ModelIndex >= group.End;
			return StartItem.ModelIndex <= group.Start;
		}
		protected abstract bool SearchIsVisible(int modelIndex);
		protected abstract bool IsCollapsed(int modelIndex);
		protected abstract bool IsHiden(int modelIndex);
		protected abstract int GetOutlineLevel(int modelIndex);
		protected abstract Point GetButtonLocation(int buttonOffset, int buttonPageItemOffset);
		protected abstract Rectangle GetLineBounds(int groupTailLenght, int groupLineWidth);
		protected abstract Point GetDotPoint(PageGridItem gridItem, PageGridItem levelItem);
	}
	#endregion
	#region RowDrawingGroupProcessor
	public class RowDrawingGroupProcessor : DrawingGroupProcessor {
		public RowDrawingGroupProcessor(Worksheet sheet, List<int> dotIndices, List<PageGrid> grids, PageGrid levelGrid, int buttonSize, int groupLineWidth, int groupTailLenght)
			: base(sheet, dotIndices, grids, levelGrid, buttonSize, groupLineWidth, groupTailLenght) {
		}
		protected override bool IsCollapsed(int modelIndex) {
			Row row = Sheet.Rows.TryGetRow(modelIndex);
			return row != null && row.IsCollapsed;
		}
		protected override Point GetButtonLocation(int buttonOffset, int buttonPageItemOffset) {
			return new Point(LevelGridItem.Near + buttonOffset, ButtonItem.Near + buttonPageItemOffset);
		}
		protected override Rectangle GetLineBounds(int groupTailLenght, int groupLineWidth) {
			int lineOffset = (LevelGridItem.Extent - groupTailLenght) / 2 + 2;
			int x = LevelGridItem.Near + lineOffset;
			int y = StartItem.Near;
			int width = groupLineWidth;
			int height = EndItem.Far - StartItem.Near;
			return new Rectangle(x, y, width, height);
		}
		protected override bool IsHiden(int modelIndex) {
			Row row = Sheet.Rows.TryGetRow(modelIndex);
			return row != null && row.IsHidden;
		}
		protected override int GetOutlineLevel(int modelIndex) {
			Row row = Sheet.Rows.TryGetRow(modelIndex);
			return row == null ? 0 : row.OutlineLevel;
		}
		protected override Point GetDotPoint(PageGridItem gridItem, PageGridItem levelItem) {
			return new Point(levelItem.Near + levelItem.Extent / 2, gridItem.Near + gridItem.Extent / 2);
		}
		protected override bool SearchIsVisible(int modelIndex) {
			Row row = Sheet.Rows.TryGetRow(modelIndex);
			return row == null ? true : row.IsVisible;
		}
	}
	#endregion
	#region ColDrawingGroupProcessor
	public class ColDrawingGroupProcessor : DrawingGroupProcessor {
		public ColDrawingGroupProcessor(Worksheet sheet, List<int> dotIndices, List<PageGrid> grids, PageGrid levelGrid, int buttonSize, int groupLineWidth, int groupTailLenght)
			: base(sheet, dotIndices, grids, levelGrid, buttonSize, groupLineWidth, groupTailLenght) {
		}
		protected override bool IsCollapsed(int modelIndex) {
			Column col = Sheet.Columns.TryGetColumn(modelIndex);
			return col != null && col.IsCollapsed;
		}
		protected override Point GetButtonLocation(int buttonOffset, int buttonPageItemOffset) {
			return new Point(ButtonItem.Near + buttonPageItemOffset, LevelGridItem.Near + buttonOffset);
		}
		protected override Rectangle GetLineBounds(int groupTailLenght, int groupLineWidth) {
			int lineOffset = (LevelGridItem.Extent - groupTailLenght) / 2 + 2;
			int x = StartItem.Near;
			int y = LevelGridItem.Near + lineOffset;
			int width = EndItem.Far - StartItem.Near;
			int height = groupLineWidth;
			return new Rectangle(x, y, width, height);
		}
		protected override bool IsHiden(int modelIndex) {
			Column col = Sheet.Columns.TryGetColumn(modelIndex);
			return col != null && col.IsHidden;
		}
		protected override int GetOutlineLevel(int modelIndex) {
			Column column = Sheet.Columns.TryGetColumn(modelIndex);
			return column == null ? 0 : column.OutlineLevel;
		}
		protected override Point GetDotPoint(PageGridItem gridItem, PageGridItem levelItem) {
			return new Point(gridItem.Near + gridItem.Extent / 2, levelItem.Near + levelItem.Extent / 2);
		}
		protected override bool SearchIsVisible(int modelIndex) {
			Column column = Sheet.Columns.TryGetColumn(modelIndex);
			return column == null ? true : column.IsVisible;
		}
	}
	#endregion
}
