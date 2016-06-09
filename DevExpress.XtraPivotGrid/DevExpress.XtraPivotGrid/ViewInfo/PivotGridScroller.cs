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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	abstract class PivotGridScroller {
		public static PivotGridScroller CreateInstance(PivotGridViewInfoBase view) {
			if(WindowsFormsSettings.IsAllowPixelScrolling)
				return new PivotGridPixelScroller(view);
			else
				return new PivotGridLineScroller(view);
		}
		PivotGridViewInfoBase view;
		Size scrollableSize;
		Point maxLeftTopCoord = Point.Empty;
		public int VisibleRowCount { get { return MaxVisibleCoordX - LeftTopCoord.X; } }
		public int VisibleColumnCount { get { return MaxVisibleCoordX - LeftTopCoord.X; } }
		public Point MaximumLeftTopCoord { get { return maxLeftTopCoord; } }
		public Rectangle ScrollableBounds { get { return new Rectangle(new Point(ScrollableBoundsLeft, ScrollableBoundsTop), scrollableSize); } }
		public abstract int ScrollSmallChange { get; }
		public abstract int MaxVisibleCoordX { get; }
		public abstract int MaxVisibleCoordY { get; }
		internal abstract int BoundsOffsetY { get; }
		internal abstract int BoundsOffsetX { get; }
		protected PivotGridViewInfoBase View { get { return view; } }
		protected PivotCellsViewInfoBase Cells { get { return view.CellsArea; } }
		protected Point LeftTopCoord { get { return view.LeftTopCoord; } }
		Rectangle RowAreaBounds { get { return view.RowAreaFields.ControlBounds; } }
		Rectangle ColumnAreaBounds { get { return view.ColumnAreaFields.ControlBounds; } }
		Rectangle ControlBounds { get { return view.PivotScrollableRectangle; } }
		Size ScrollBarSize { get { return view.ScrollBarSize; } }
		bool IsHorzScrollControl { get { return view.IsHorzScrollControl; } }
		int ScrollableBoundsLeft { get { return IsHorzScrollControl ? ControlBounds.Left : RowAreaBounds.Right; } }
		int ScrollableBoundsTop { get { return ColumnAreaBounds.Bottom; } }
		Size CellAreaSize {
			get {
				Size cellAreaSize = new Size(scrollableSize.Width, scrollableSize.Height);
				if(view.ScrollBarOverlap) {
					if(view.IsHScrollBarVisible) cellAreaSize.Height += view.ScrollBarSize.Height;
					if(view.IsVScrollBarVisible) cellAreaSize.Width += view.ScrollBarSize.Width;
				}
				return cellAreaSize;
			}
		}
		protected PivotGridScroller(PivotGridViewInfoBase view) {
			this.view = view;
		}
		public Point GetMaximumLeftTopCoord(Size size) {
			return new Point(GetMaximumLeftTopCoord(true, size), GetMaximumLeftTopCoord(false, size));
		}
		public void Recalculate() {
			scrollableSize = ControlBounds.Size;
			if(!IsHorzScrollControl) scrollableSize.Width -= RowAreaBounds.Width;
			scrollableSize.Height -= RowAreaBounds.Top - ControlBounds.Top;
			maxLeftTopCoord.X = GetMaximumLeftTopCoordX();
			bool hasXScroll = maxLeftTopCoord.X > 0;
			if(hasXScroll) scrollableSize.Height -= ScrollBarSize.Height;
			maxLeftTopCoord.Y = GetMaximumLeftTopCoordY();
			if(maxLeftTopCoord.Y > 0) {
				scrollableSize.Width -= ScrollBarSize.Width;
				maxLeftTopCoord.X = GetMaximumLeftTopCoordX();
				if(maxLeftTopCoord.X > 0 && !hasXScroll) {
					scrollableSize.Height -= ScrollBarSize.Height;
					maxLeftTopCoord.Y = GetMaximumLeftTopCoordY();
				}
			}
			if(scrollableSize.Height < 0) scrollableSize.Height = 0;
			if(scrollableSize.Width < 0) scrollableSize.Width = 0;
			view.LeftTopCoord = CorrectLeftTopCoord(view.LeftTopCoord);
		}
		public void SetLeftTopCoordOffset() {
			int horizontalOffset = -BoundsOffsetX;
			if(view.IsHorzScrollControl) {
				view.BoundsOffset = new Size(horizontalOffset, 0);
				if(view.FilterHeaders != null)
					view.SetFilterAndColumnHeadersWidth();
				view.ColumnAreaFields.OnBoundsChanged();
			} else
				view.ColumnAreaFields.BoundsOffset = new Size(horizontalOffset, 0);
			view.RowAreaFields.BoundsOffset = new Size(0, -BoundsOffsetY);
		}
		public Point CorrectLeftTopCoord(Point value) {
			Point coord = value;
			if(coord.X >  maxLeftTopCoord.X)
				coord.X = maxLeftTopCoord.X;
			if(coord.X < 0)
				coord.X = 0;
			if(coord.Y > maxLeftTopCoord.Y)
				coord.Y = maxLeftTopCoord.Y;
			if(coord.Y < 0)
				coord.Y = 0;
			return coord;
		}
		public void ScrollToCell(Point cell) {
			if(!view.VisualItems.IsCellValid(cell))
				return;
			Point newLeftTopCoord = CalculateLeftTopCoordByCell(cell);
			if(view.LeftTopCoord != newLeftTopCoord)
				view.LeftTopCoord = newLeftTopCoord;
		}
		protected int GetMaximumLeftTopCoord(bool isColumn, Size clientSize) {
			int totalSize = Cells.GetTotalSize(isColumn, int.MaxValue);
			int size = isColumn ? clientSize.Width : clientSize.Height;
			if(size >= totalSize) return 0;
			return GetMaximumLeftTopCoordCore(isColumn, clientSize, totalSize - size);
		}
		protected abstract int GetMaximumLeftTopCoordCore(bool column, Size clientSize, int unvisibleSize);
		protected abstract Point CalculateLeftTopCoordByCell(Point cell);
		int GetMaximumLeftTopCoordX() {
			if(IsHorzScrollControl) {
				int totalSize = Cells.GetTotalSize(true, int.MaxValue) + RowAreaBounds.Width - view.BoundsOffset.Width;
				if(CellAreaSize.Width > totalSize)
					return 0;
				if(WindowsFormsSettings.IsAllowPixelScrolling) {
					return totalSize - CellAreaSize.Width;
				} else {
					int maxLeftTopCoordX = Cells.ColumnCount + view.RowAreaFields.LevelCount;
					int unusedWidth = CellAreaSize.Width;
					maxLeftTopCoordX -= GetLastVisibleColumnCount(ref unusedWidth);
					if(unusedWidth > 0)
						maxLeftTopCoordX -= view.RowAreaFields.GetLastVisibleColumnCount(ref unusedWidth);
					return maxLeftTopCoordX;
				}
			}
			return GetMaximumLeftTopCoord(CellAreaSize).X;
		}
		int GetLastVisibleColumnCount(ref int width) {
			int count = 0;
			for(int col = Cells.ColumnCount - 1; col >= 0; col--) {
				width -= Cells.GetCellWidth(Cells.GetColumnValue(col));
				if(width < 0) break;
				count++;
			}
			return count;
		}
		int GetMaximumLeftTopCoordY() {
			return GetMaximumLeftTopCoord(CellAreaSize).Y;
		}
	}
	class PivotGridPixelScroller : PivotGridScroller {
		public override int ScrollSmallChange { get { return View.FieldMeasures.DefaultFieldValueHeight; } }
		public override int MaxVisibleCoordX {
			get {
				int maxVisibleCoordX = View.CellsArea.Width + View.LeftTopCoord.X;
				if(View.IsHorzScrollControl)
					maxVisibleCoordX += View.RowAreaFields.ControlBounds.Width;
				return maxVisibleCoordX;
			}
		}
		public override int MaxVisibleCoordY { get { return View.CellsArea.Height; } }
		internal override int BoundsOffsetY { get { return View.LeftTopCoord.Y; } }
		internal override int BoundsOffsetX { get { return View.LeftTopCoord.X ; } }
		public PivotGridPixelScroller(PivotGridViewInfoBase view)
			: base(view) {
		}
		protected override Point CalculateLeftTopCoordByCell(Point cell) {
			Point newLeftTopCoord = View.LeftTopCoord;
			PivotCellViewInfoBase cellViewInfo = View.CellsArea.GetCellViewInfoByCoord(cell.X, cell.Y);
			if(cellViewInfo != null) {
				Rectangle cellBounds = cellViewInfo.Bounds;
				if(cellBounds.Left <= View.ColumnAreaFields.PaintBounds.Left)
					newLeftTopCoord.X -= cellBounds.Width;
				if(cellBounds.Right >= View.ScrollableBounds.Right)
					newLeftTopCoord.X += cellBounds.Width;
				if(cellBounds.Top <= View.RowAreaFields.PaintBounds.Top)
					newLeftTopCoord.Y -= cellBounds.Height;
				if(cellBounds.Bottom >= View.ScrollableBounds.Bottom)
					newLeftTopCoord.Y += cellBounds.Height;
			}
			return newLeftTopCoord;
		}
		protected override int GetMaximumLeftTopCoordCore(bool column, Size clientSize, int unvisibleSize) {
			return unvisibleSize;
		}
	}
	class PivotGridLineScroller : PivotGridScroller {
		public override int ScrollSmallChange { get { return 1; } }
		public override int MaxVisibleCoordX {
			get {
				int maxVisibleCoordX = View.CellsArea.ColumnCount;
				if(View.IsHorzScrollControl)
					maxVisibleCoordX += View.RowAreaFields.ScrollColumnCount;
				return maxVisibleCoordX;
			}
		}
		public override int MaxVisibleCoordY { get { return View.CellsArea.RowCount; } }
		internal override int BoundsOffsetY { get { return Cells.GetTotalSize(false, LeftTopCoord.Y); } }
		internal override int BoundsOffsetX { 
			get {
				int cellScrollColumn = LeftTopCoord.X;
				if(View.IsHorzScrollControl) {
					cellScrollColumn -= View.RowAreaFields.ScrollColumnCount;
					if(cellScrollColumn < 0) cellScrollColumn = 0;
				}
				int offset = Cells.GetTotalSize(true, cellScrollColumn);
				if(View.IsHorzScrollControl)
					offset += View.RowAreaFields.ScrollOffset;
				return offset;
			} 
		}
		public PivotGridLineScroller(PivotGridViewInfoBase view)
			: base(view) {
		}
		protected override Point CalculateLeftTopCoordByCell(Point cell) {
			return new Point(
				CalcCellVisibleIndex(cell.X, View.LeftTopCoord.X, GetVisibleColumnCount(View.ScrollableBounds.Width)),
				CalcCellVisibleIndex(cell.Y, View.LeftTopCoord.Y, GetVisibleRowCount(View.ScrollableBounds.Height))
			);
		}
		protected override int GetMaximumLeftTopCoordCore(bool isColumn, Size clientSize, int unvisibleSize) {
			return isColumn ?
				GetMaxCoord(clientSize.Width, true, Cells.ColumnCount) :
				GetMaxCoord(clientSize.Height, false, Cells.RowCount);
		}
		int GetMaxCoord(int length, bool width, int count) {
			int max = Math.Max(0, count - 1);
			for(; count > 0 && length > 0; count--)
				length -= Cells.FieldMeasures.GetLevelLength(width, count - 1);
			if(length < 0)
				count++;
			return Math.Min(max, Math.Max(0, count));
		}
		int CalcCellVisibleIndex(int newLeftTop, int curLeftTop, int visibleCount) {
			int result = curLeftTop;
			if(newLeftTop < curLeftTop)
				result = newLeftTop;
			if(visibleCount == 0) {
				visibleCount = 1;
			}
			if(newLeftTop >= curLeftTop + visibleCount)
				result = newLeftTop - visibleCount + 1;
			return result;
		}
		int GetVisibleRowCount(int height) {
			int count = 0;
			int topCoord = LeftTopCoord.Y > Cells.RowCount ? Cells.RowCount : LeftTopCoord.Y;
			for(int row = topCoord; row < Cells.RowCount; row++) {
				height -= Cells.GetCellHeight(Cells.GetRowValue(row));
				if(height <= 0) break;
				count++;
			}
			for(int row = topCoord - 1; row >= 0; row--) {
				height -= Cells.GetCellHeight(Cells.GetRowValue(row));
				if(height <= 0) break;
				count++;
			}
			return count;
		}
		int GetVisibleColumnCount(int width) {
			int count = 0;
			int leftCoord = LeftTopCoord.X > Cells.ColumnCount ? Cells.ColumnCount : LeftTopCoord.X;
			for(int col = leftCoord; col < Cells.ColumnCount; col++) {
				width -= Cells.GetCellWidth(Cells.GetColumnValue(col));
				if(width <= 0) break;
				count++;
			}
			for(int col = leftCoord - 1; col >= 0; col--) {
				width -= Cells.GetCellWidth(Cells.GetColumnValue(col));
				if(width <= 0) break;
				count++;
			}
			return count;
		}
	}
}
