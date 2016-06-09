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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotGridScrollBarsViewInfo {
		static Size scrollBarSize = new Size(SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
		static ScrollArgs GetScrollBarInfo(int value, int largeChange, int maximum, int smallChange) {
			ScrollArgs e = new ScrollArgs();
			if(largeChange > 0) {
				e.Value = value;
				e.Maximum = maximum - 1;
				e.SmallChange = smallChange;
				e.LargeChange = largeChange;
			}
			return e;
		}
		public static Size ScrollBarSize { get { return scrollBarSize; } }
		PivotGridViewInfoBase viewInfo;
		Rectangle RowAreaBounds { get { return viewInfo.RowAreaFields.ControlBounds; } }
		Rectangle ColumnAreaBounds { get { return viewInfo.ColumnAreaFields.ControlBounds; } }
		Rectangle ControlBounds { get { return viewInfo.PivotScrollableRectangle; } }
		PivotGridScroller Scroller { get { return viewInfo.Scroller; } }
		bool ScrollBarOverlap { get { return viewInfo.ScrollBarOverlap; } }
		bool IsHorzScrollControl { get { return viewInfo.IsHorzScrollControl; } }
		public bool HScrollVisible { get { return Scroller.MaximumLeftTopCoord.X > 0; } }
		public bool VScrollVisible { get { return Scroller.MaximumLeftTopCoord.Y > 0; } }
		public ScrollArgs HScrollBarInfo {
			get {
				return GetScrollBarInfo(viewInfo.LeftTopCoord.X, Math.Max(1, Scroller.MaxVisibleCoordX - Scroller.MaximumLeftTopCoord.X), Scroller.MaxVisibleCoordX, Scroller.ScrollSmallChange);
			}
		}
		public ScrollArgs VScrollBarInfo {
			get {
				return GetScrollBarInfo(viewInfo.LeftTopCoord.Y, Math.Max(1, Scroller.MaxVisibleCoordY - Scroller.MaximumLeftTopCoord.Y), Scroller.MaxVisibleCoordY, Scroller.ScrollSmallChange);
			}
		}
		public PivotGridScrollBarsViewInfo(PivotGridViewInfoBase pivotGridViewInfoBase) {
			viewInfo = pivotGridViewInfoBase;
		}
		public void PaintEmptyScrollFieldValueCell(ViewInfoPaintArgs e) {
			if(HScrollVisible && !IsHorzScrollControl) {
				Rectangle rowVisibleBounds = Rectangle.Intersect(RowAreaBounds, ControlBounds);
				if(!rowVisibleBounds.IsEmpty) {
					int cellTop = rowVisibleBounds.Bottom;
					int cellHeight =  ControlBounds.Bottom - cellTop + 1;
					if(cellHeight <= ScrollBarSize.Height && !ScrollBarOverlap) {
						if(rowVisibleBounds.Height > ScrollBarSize.Width) {
							cellHeight = ScrollBarSize.Height;
							cellTop = ControlBounds.Bottom - ScrollBarSize.Height;
						} else {
							cellHeight = rowVisibleBounds.Height;
							cellTop = rowVisibleBounds.Top;
						}
					}
					DrawEmptyArea(e, viewInfo.RightToLeftRect(new Rectangle(ControlBounds.Left, cellTop, RowAreaBounds.Width, cellHeight)));
				}
			}
			if(VScrollVisible) {
				Rectangle emptyCellBounds = new Rectangle(ColumnAreaBounds.Right, ColumnAreaBounds.Top, ControlBounds.Right - ColumnAreaBounds.Right + 1, ColumnAreaBounds.Height);
				HeaderPositionKind emptyCellPositionKind = HeaderPositionKind.Right;
				if(!ScrollBarOverlap) {
					int capLeft = Math.Max(ControlBounds.Right - ScrollBarSize.Width, ColumnAreaBounds.Left);
					int capWidth = Math.Min(ControlBounds.Right - capLeft, ScrollBarSize.Width);
					Rectangle scrollCapBounds = new Rectangle(capLeft, emptyCellBounds.Top, capWidth + 1, emptyCellBounds.Height);
					emptyCellBounds.Width -= scrollCapBounds.Width;
					emptyCellPositionKind = HeaderPositionKind.Center;
					DrawEmptyFieldValueCell(e, viewInfo.RightToLeftRect(scrollCapBounds), HeaderPositionKind.Right);
				}
				DrawEmptyFieldValueCell(e, viewInfo.RightToLeftRect(emptyCellBounds), emptyCellPositionKind);
			}
		}		
		void DrawEmptyFieldValueCell(ViewInfoPaintArgs e, Rectangle bounds, HeaderPositionKind positionKind) {
			PivotHeaderObjectPainter painter = new PivotHeaderObjectPainter(viewInfo.Data.ActiveLookAndFeel.Painter.Header);
			PivotHeaderObjectInfoArgs infoArgs = new PivotHeaderObjectInfoArgs();
			Rectangle scrollCap = Rectangle.Empty;
			Rectangle emptyCell = Rectangle.Empty;
			infoArgs.Cache = e.GraphicsCache;
			infoArgs.Bounds = bounds;
			infoArgs.FieldValueTopBorderColor = viewInfo.Data.Appearance.FieldValueTopBorderColor;
			infoArgs.FieldValueLeftRightBorderColor = viewInfo.Data.Appearance.FieldValueLeftRightBorderColor;
			infoArgs.HeaderPosition = positionKind;
			infoArgs.IsTopMost = true;
			infoArgs.RightToLeft = viewInfo.IsRightToLeft;
			painter.DrawObject(infoArgs);
		}
		void DrawEmptyArea(ViewInfoPaintArgs e, Rectangle bounds) {
			Color backColor = SystemColors.Control;
			Skin skin = GridSkins.GetSkin(viewInfo.Data.ActiveLookAndFeel.ActiveLookAndFeel);
			if(skin != null) {
				SkinElement element = skin[GridSkins.SkinGridGroupPanel];
				if(element != null)
					backColor = element.Color.GetBackColor();
			}
			SolidBrush brush = new SolidBrush(backColor);
			e.GraphicsCache.FillRectangle(brush, bounds);
			brush.Dispose();
			Color topColor = viewInfo.Data.Appearance.FieldValueTopBorderColor;
			Color leftColor = viewInfo.Data.Appearance.FieldValueLeftRightBorderColor;
			DrawBorder(e.GraphicsCache, leftColor, bounds.Right - 1, bounds.Top - 1, bounds.Right - 1, bounds.Bottom - 1);
			DrawBorder(e.GraphicsCache, topColor, bounds.Left - 1, bounds.Top - 1, bounds.Right - 1, bounds.Top - 1);
		}
		void DrawBorder(GraphicsCache cache, Color borderColor, int x1, int y1, int x2, int y2) {
			cache.Paint.DrawLine(cache.Graphics, cache.GetPen(borderColor), new Point(x1, y1), new Point(x2, y2));
		}
	}
}
