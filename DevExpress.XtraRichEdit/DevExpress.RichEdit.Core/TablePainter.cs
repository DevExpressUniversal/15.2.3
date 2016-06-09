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
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using LayoutUnitF = System.Single;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout {
	public interface IPainterWrapper {
		void SnapWidths(float[] widths);
		void SnapHeights(float[] heights);
		void FillRectangle(Color color, RectangleF bounds);
		PointF GetSnappedPoint(PointF point);
		ICharacterLinePainter HorizontalLinePainter { get; }
		ICharacterLinePainter VerticalLinePainter { get; }
	}
	public class GraphicsPainterWrapper : IPainterWrapper {
		Painter painter;
		ICharacterLinePainter horizontalLinePainter;
		ICharacterLinePainter verticalLinePainter;
		public GraphicsPainterWrapper(Painter painter, ICharacterLinePainter horizontalLinePainter, ICharacterLinePainter verticalLinePainter) {
			this.painter = painter;
			this.horizontalLinePainter = horizontalLinePainter;
			this.verticalLinePainter = verticalLinePainter;
		}
		#region IPainterWrapper Members
		public ICharacterLinePainter HorizontalLinePainter { get { return horizontalLinePainter; } }
		public ICharacterLinePainter VerticalLinePainter { get { return verticalLinePainter; } }
		public void SnapWidths(float[] widths) {
			painter.SnapWidths(widths);
		}
		public void SnapHeights(float[] heights) {
			painter.SnapHeights(heights);
		}
		public void FillRectangle(Color color, RectangleF bounds) {
			painter.FillRectangle(color, bounds);
		}
		public PointF GetSnappedPoint(PointF point) {
			return painter.GetSnappedPoint(point);			
		}
		#endregion
	}
	public class TableCornerPainter {
		public virtual void DrawCorner(IPainterWrapper painter, int x, int y, CornerViewInfoBase corner) {
			bool[][] pattern = corner.Pattern;
			float[] widths = GetSnappedWidths(painter, corner);
			float[] heights = GetSnappedHeights(painter, corner);
			PointF leftTop = GetCornerSnappedPoint(painter, x, y, corner);			
			int rowCount = pattern.Length;
			float currentY = leftTop.Y;
			float nextY = currentY + heights[0];			
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				bool[] rowPattern = pattern[rowIndex];
				int columnCount = rowPattern.Length;
				currentY = nextY;
				nextY = currentY + heights[rowIndex + 1];
				float currentX = leftTop.X;
				float nextX = currentX + widths[0];
				for (int columnIndex = 0; columnIndex < columnCount; columnIndex++) {
					currentX = nextX;
					nextX = currentX + widths[columnIndex + 1];
					if (rowPattern[columnIndex])
						painter.FillRectangle(GetDrawingColor(corner.Color, corner.CornerType), new RectangleF(currentX, currentY, widths[columnIndex + 1], heights[rowIndex + 1]));
				}
			}
		}
		Color GetDrawingColor(Color color, CornerViewInfoType cornerType) {
			if (color == DXColor.Empty)
				return DXColor.Black;
			return color;
		}
		float[] GetSnappedWidths(IPainterWrapper painter, CornerViewInfoBase corner) {
			float[] result = new float[corner.Widths.Length];
			result[0] = corner.Widths[0] * corner.WidthF;
			for (int i = 1; i < corner.Widths.Length; i++) {				
				result[i] = (corner.Widths[i] - corner.Widths[i - 1]) * corner.WidthF;
			}
			painter.SnapWidths(result);
			return result;
		}
		float[] GetSnappedHeights(IPainterWrapper painter, CornerViewInfoBase corner) {
			float[] result = new float[corner.Heights.Length];
			result[0] = corner.Heights[0] * corner.HeightF;
			for (int i = 1; i < corner.Heights.Length; i++) {
				result[i] = (corner.Heights[i] - corner.Heights[i - 1]) * corner.HeightF;
			}
			painter.SnapHeights(result);
			return result;
		}
		public PointF GetCornerSnappedPoint(IPainterWrapper painter, float x, float y, CornerViewInfoBase corner) {
			switch(corner.CornerType) {
				case CornerViewInfoType.Normal:
					return painter.GetSnappedPoint(new PointF(x - (corner.Width + corner.Width % 2) / 2, y));
				case CornerViewInfoType.OuterHorizontalStart:
					return painter.GetSnappedPoint(new PointF(x - corner.Width, y));
				case CornerViewInfoType.OuterHorizontalEnd:
					return painter.GetSnappedPoint(new PointF(x, y));
				case CornerViewInfoType.OuterVerticalStart:
					return painter.GetSnappedPoint(new PointF(x - (corner.Width + corner.Width % 2) / 2, y - corner.Height));
				case CornerViewInfoType.OuterVerticalEnd:
					return painter.GetSnappedPoint(new PointF(x - (corner.Width + corner.Width % 2) / 2, y));
				case CornerViewInfoType.InnerTopLeft:
					return painter.GetSnappedPoint(new PointF(0, 0));
				case CornerViewInfoType.InnerTopMiddle:
					return painter.GetSnappedPoint(new PointF(x - (corner.Width + corner.Width % 2) / 2, 0));
				case CornerViewInfoType.InnerTopRight:
					return painter.GetSnappedPoint(new PointF(x - corner.Width, 0));
				case CornerViewInfoType.InnerLeftMiddle:
					return painter.GetSnappedPoint(new PointF(0, y - (corner.Height + corner.Height % 2) / 2));
				case CornerViewInfoType.InnerRightMiddle:
					return painter.GetSnappedPoint(new PointF(x - corner.Width, y - (corner.Height + corner.Height % 2) / 2));
				case CornerViewInfoType.InnerBottomLeft:
					return painter.GetSnappedPoint(new PointF(0, y - corner.Height));
				case CornerViewInfoType.InnerBotomMiddle:
					return painter.GetSnappedPoint(new PointF(x - (corner.Width + corner.Width % 2) / 2, y - corner.Height));
				case CornerViewInfoType.InnerBottomRight:
					return painter.GetSnappedPoint(new PointF(x - corner.Width, y - corner.Height));
				case CornerViewInfoType.InnerNormal:
					return painter.GetSnappedPoint(new PointF(x - (corner.Width + corner.Width % 2) / 2, y - (corner.Height + corner.Height % 2) / 2));
				default:
					Exceptions.ThrowInternalException();
					return new PointF(0, 0);
			}
		}
		public float GetCornerWidth(IPainterWrapper painter, CornerViewInfoBase corner) {
			float[] widths = GetSnappedWidths(painter, corner);
			return GetTotalSize(widths);
		}
		public float GetCornerHeight(IPainterWrapper painter, CornerViewInfoBase corner) {
			float[] heights = GetSnappedHeights(painter, corner);
			return GetTotalSize(heights);
		}
		float GetTotalSize(float[] widths) {
			float result = 0;
			for (int i = 0; i < widths.Length; i++)
				result += widths[i];
			return result;
		}
	}
	public abstract class TableBorderPainter {
		readonly LayoutUnitF widthF;
		readonly LayoutUnitF halfBorderWidth;
		readonly IPainterWrapper painter;
		TableCornerPainter cornerPainter;
		protected TableBorderPainter(IPainterWrapper painter, LayoutUnitF width) {
			this.widthF = width;
			this.halfBorderWidth = width / 2;
			this.painter = painter;			
			this.cornerPainter = new TableCornerPainter();
		}
		public LayoutUnitF Width { get { return widthF; } }
		protected LayoutUnitF HalfBorderWidth { get { return halfBorderWidth; } }
		protected IPainterWrapper Painter { get { return painter; } }
		PointF GetStartSnappedPoint(float x, float y) {
			int width = (int)widthF;
			return painter.GetSnappedPoint(new PointF(x - (width + width % 2) / 2, y));
		}
		public virtual void DrawBorder(ITableBorderViewInfoBase borderViewInfo, Rectangle cellBounds) {
			if ((borderViewInfo.BorderType & BorderTypes.Horizontal) != 0) {												
				float x = cellBounds.Left;
				float y = borderViewInfo.BorderType == BorderTypes.Top ? cellBounds.Top : cellBounds.Bottom;
				PointF left = borderViewInfo.HasStartCorner ? cornerPainter.GetCornerSnappedPoint(Painter, x, y, borderViewInfo.StartCorner) : Painter.GetSnappedPoint(new PointF(x, y));
				PointF right = borderViewInfo.HasEndCorner ? cornerPainter.GetCornerSnappedPoint(Painter, x + cellBounds.Width, y, borderViewInfo.EndCorner) : Painter.GetSnappedPoint(new PointF(x + cellBounds.Width, y));
				float startCornerWidth = 0;				
				if (borderViewInfo.HasStartCorner) {
					startCornerWidth = cornerPainter.GetCornerWidth(Painter, borderViewInfo.StartCorner);
				}
				left.X += startCornerWidth;				
				DrawHorizontalBorder(GetDrawingColor(borderViewInfo.Border.Color), left, right.X - left.X);
			}
			if ((borderViewInfo.BorderType & BorderTypes.Vertical) != 0) {
				float x = borderViewInfo.BorderType == BorderTypes.Left ? cellBounds.Left : cellBounds.Right;
				float y = cellBounds.Top;
				PointF top = borderViewInfo.HasStartCorner ? cornerPainter.GetCornerSnappedPoint(Painter, x, y, borderViewInfo.StartCorner) : Painter.GetSnappedPoint(new PointF(x, y));
				if (borderViewInfo.HasStartCorner && borderViewInfo.StartCorner.CornerType <= CornerViewInfoType.OuterVerticalEnd) 
					top.X = GetStartSnappedPoint(x, y).X;
				PointF bottom = borderViewInfo.HasEndCorner ? cornerPainter.GetCornerSnappedPoint(Painter, x, y + cellBounds.Height, borderViewInfo.EndCorner) : Painter.GetSnappedPoint(new PointF(x, y + cellBounds.Height));
				float startCornerHeight = 0;
				if (borderViewInfo.HasStartCorner) {
					startCornerHeight = cornerPainter.GetCornerHeight(Painter, borderViewInfo.StartCorner);
				}
				top.Y += startCornerHeight;
				DrawVerticalBorder(GetDrawingColor(borderViewInfo.Border.Color), top, bottom.Y - top.Y);
			}
		}
		private Color GetDrawingColor(Color color) {
			if (color == DXColor.Empty)
				return DXColor.Black;
			else
				return color;
		}
		protected virtual void DrawHorizontalLines(Color color, float[] offsets, PointF left, float width) {
			DrawHorizontalLines(color, offsets, left, width, null);
		}
		protected virtual void DrawHorizontalLines(Color color, float[] offsets, PointF left, float width, Underline underline) {
			float[] heights = new float[offsets.Length];
			heights[0] = offsets[0];
			for (int i = 0; i < offsets.Length - 1; i++)
				heights[i + 1] = offsets[i + 1] - offsets[i];
			Painter.SnapHeights(heights);
			float y = left.Y + heights[0];
			for (int i = 1; i < heights.Length; i += 2) {
				float height = heights[i];
				if (underline != null) {
					RectangleF bounds = new RectangleF(left.X, y + height / 2, width, height);
					ICharacterLinePainter linePainter = Painter.HorizontalLinePainter;
					underline.Draw(linePainter, bounds, color);
				}
				else {
					RectangleF bounds = new RectangleF(left.X, y, width, height);
					Painter.FillRectangle(color, bounds);
				}
				y += height;
				if (i + 1 < heights.Length)
					y += heights[i + 1];
			}
		}
		protected virtual void DrawVerticalLines(Color color, float[] offsets, PointF top, float height) {
			DrawVerticalLines(color, offsets, top, height, null);
		}
		protected virtual void DrawVerticalLines(Color color, float[] offsets, PointF top, float height, Underline underline) {
			float[] widths = new float[offsets.Length];
			widths[0] = offsets[0];
			for (int i = 0; i < offsets.Length - 1; i++)
				widths[i + 1] = offsets[i + 1] - offsets[i];
			Painter.SnapWidths(widths);
			float x = top.X + widths[0];
			for (int i = 1; i < widths.Length; i += 2) {
				float width = widths[i];
				if (underline != null) {
					RectangleF bounds = new RectangleF(x + width / 2, top.Y, width, height);
					ICharacterLinePainter linePainter = Painter.VerticalLinePainter;
					underline.Draw(linePainter, bounds, color);
				}
				else {
					RectangleF bounds = new RectangleF(x, top.Y, width, height);
					Painter.FillRectangle(color, bounds);
				}
				x += width;
				if (i + 1 < widths.Length)
					x += widths[i + 1];
			}
		}
		public abstract void DrawHorizontalBorder(Color color, PointF left, float width);
		public abstract void DrawVerticalBorder(Color color, PointF top, float height);
	}
	public class SingleBorderPainter : TableBorderPainter {		
		float[] offsets;
		Underline underline;
		public SingleBorderPainter(IPainterWrapper painter, LayoutUnitF width, Underline underline)
			: base(painter, width != 0 ? width : 1f) {
				this.offsets = new float[] { 0, width != 0 ? width : 1f };
			this.underline = underline;
		}
		public override void DrawVerticalBorder(Color color, PointF top, float height) {			
			DrawVerticalLines(color, offsets, top, height, underline);
		}
		public override void DrawHorizontalBorder(Color color, PointF left, float width) {
			DrawHorizontalLines(color, offsets, left, width, underline);
		}
	}
	public class DoubleBorderPainter : SingleBorderPainter {
		float[] offsets;
		public DoubleBorderPainter(IPainterWrapper painter, float[] compoundArray, LayoutUnitF width)
			: base(painter, width, null) {
			this.offsets = new float[compoundArray.Length];
			for (int i = 0; i < compoundArray.Length; i++) {
				offsets[i] = (compoundArray[i] * width);
			}
		}
		public override void DrawHorizontalBorder(Color color, PointF left, float width) {
			DrawHorizontalLines(color, offsets, left, width);
		}
		public override void DrawVerticalBorder(Color color, PointF top, float height) {
			DrawVerticalLines(color, offsets, top, height);
		}
	}
	public class TripleBorderPainter : DoubleBorderPainter {
		public TripleBorderPainter(IPainterWrapper painter, float[] compoundArray, LayoutUnitF width)
			: base(painter, compoundArray, width) {
		}
	}
}
