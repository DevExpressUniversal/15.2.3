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
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class LineBrickExporter : VisualBrickExporter {
		static Pair<Rectangle, Rectangle> SplitVert(Rectangle rect, int dx) {
			return new Pair<Rectangle, Rectangle>(new Rectangle(rect.Left, rect.Top, dx, rect.Height), new Rectangle(rect.Left + dx, rect.Top, rect.Width - dx, rect.Height));
		}
		static Pair<Rectangle, Rectangle> SplitHorz(Rectangle rect, int dy) {
			return new Pair<Rectangle, Rectangle>(new Rectangle(rect.Left, rect.Top, rect.Width, dy), new Rectangle(rect.Left, rect.Top + dy, rect.Width, rect.Height - dy));
		}
		static Rectangle AlignHorz(Rectangle rect, Rectangle baseRect, BrickAlignment alignment) {
			switch (alignment) {
				case BrickAlignment.Near:
					rect.X = baseRect.X;
					break;
				case BrickAlignment.Center: {
						Rectangle r = rect;
						rect = CenterInt(rect, baseRect);
						rect.Y = r.Y;
						break;
					}
				case BrickAlignment.Far:
					rect.X = baseRect.Right - rect.Width;
					break;
			}
			return rect;
		}
		static Rectangle AlignVert(Rectangle rect, Rectangle baseRect, BrickAlignment alignment) {
			switch (alignment) {
				case BrickAlignment.Near:
					rect.Y = baseRect.Y;
					break;
				case BrickAlignment.Center: {
						Rectangle r = rect;
						rect = CenterInt(rect, baseRect);
						rect.X = r.X;
						break;
					}
				case BrickAlignment.Far:
					rect.Y = baseRect.Bottom - rect.Height;
					break;
			}
			return rect;
		}
		static Rectangle CenterInt(Rectangle rect, Rectangle baseRect) {
			rect.Offset(baseRect.Left - rect.Left, baseRect.Top - rect.Top);
			int dx = (baseRect.Width - rect.Width) / 2;
			int dy = (baseRect.Height - rect.Height) / 2;
			rect.Offset(dx, dy);
			return rect;
		}
		LineBrick LineBrick { get { return Brick as LineBrick; } }
		bool ShouldExportAsImage { get { return LineBrick.ShouldExportAsImage; } }
		HtmlLineDirection HtmlLineDirection { get { return LineBrick.HtmlLineDirection; } }
		int PixLineWidth { get { return (int)Math.Ceiling(GraphicsUnitConverter.DocToDip(LineBrick.DocWidth)); } }
		protected override void DrawObject(IGraphics gr, RectangleF rect) {
			BrickPaint.DrawRect(gr, rect, gr.PrintingSystem.Gdi);
			DrawObjectCore(gr, rect);
		}
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			if (clientRect.IsEmpty)
				return;
			PointF pt1 = LineBrick.GetPoint1(clientRect);
			PointF pt2 = LineBrick.GetPoint2(clientRect);
			using (Pen pen = new Pen(LineBrick.ForeColor, Math.Min(clientRect.Width, LineBrick.DocWidth))) {
				if (LineBrick.LineStyle != DashStyle.Custom) {
					if (LineBrick.LineStyle == DashStyle.Solid)
						pen.DashStyle = DashStyle.Solid;
					else {
						pen.DashStyle = DashStyle.Custom;
						pen.DashPattern = VisualBrick.GetDashPattern(LineBrick.LineStyle);
					}
				}
				pen.Alignment = PenAlignment.Center;
				gr.DrawLine(pen, pt1, pt2);
			}
		}
		protected override object[] GetSpecificKeyPart() {
			return new object[] { LineBrick.LineDirection, LineBrick.LineStyle, LineBrick.LineWidth };
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			if(!exportContext.CanPublish(LineBrick) || exportContext.RawDataMode)
				return new BrickViewData[0];
			return DrawContentToViewData(exportContext, GraphicsUnitConverter.Round(rect), TextAlignment.MiddleCenter);
		}
		protected override void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
			FillTableCellWithImage(exportProvider);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			if(!(exportProvider is DevExpress.XtraPrinting.Export.XLS.XlsExportProvider))
				FillXlsxTableCellWithShape(exportProvider, LineBrick.ForeColor, LineBrick.LineDirection, LineBrick.LineStyle, LineBrick.LineWidth, LineBrick.Padding);
			else
				FillTableCellWithImage(exportProvider);
		}
		void FillTableCellWithImage(ITableExportProvider exportProvider) {
			FillTableCellWithImage(exportProvider, ImageSizeMode.Normal, ImageAlignment.TopLeft, exportProvider.CurrentData.Bounds);
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			if (ShouldExportAsImage)
				FillHtmlTableCellWithImage(exportProvider);
			else
				base.FillHtmlTableCellCore(exportProvider);
		}
		protected internal override BrickViewData[] GetHtmlData(ExportContext htmlExportContext, RectangleF bounds, RectangleF clipBoundsF) {
			if(!htmlExportContext.CanPublish(LineBrick))
				return new BrickViewData[0];
			if (ShouldExportAsImage)
				return base.GetHtmlData(htmlExportContext, bounds, clipBoundsF);
			else
				return GetHtmlDataCore(htmlExportContext, bounds, clipBoundsF);
		}
		BrickViewData[] GetHtmlDataCore(ExportContext htmlExportContext, RectangleF bounds, RectangleF clipBoundsF) {
			Rectangle clipBounds = GraphicsUnitConverter.Round(clipBoundsF);
			Rectangle firstRect;
			Rectangle secondRect;
			Rectangle thirdRect;
			Rectangle lineBounds = clipBounds;
			if (HtmlLineDirection == HtmlLineDirection.Horizontal ||
				(HtmlLineDirection != HtmlLineDirection.Vertical && clipBounds.Width >= clipBounds.Height)) {
				lineBounds.Height = PixLineWidth;
				lineBounds = AlignVert(lineBounds, clipBounds, BrickAlignment.Center);
				Pair<Rectangle, Rectangle> rects = SplitHorz(clipBounds, lineBounds.Top - clipBounds.Top);
				firstRect = rects.First;
				rects = SplitHorz(rects.Second, lineBounds.Height);
				secondRect = rects.First;
				thirdRect = rects.Second;
			} else {
				lineBounds.Width = PixLineWidth;
				lineBounds = AlignHorz(lineBounds, clipBounds, BrickAlignment.Center);
				Pair<Rectangle, Rectangle> rects = SplitVert(clipBounds, lineBounds.Left - clipBounds.Left);
				firstRect = rects.First;
				rects = SplitVert(rects.Second, lineBounds.Width);
				secondRect = rects.First;
				thirdRect = rects.Second;
			}
			BrickStyle baseStyle = (BrickStyle)this.Style.Clone();
			baseStyle.Padding = PaddingInfo.Empty;
			BrickStyle firstStyle = VisualBrick.GetAreaStyle(LineBrick.PrintingSystem.Styles, baseStyle, firstRect, clipBounds);
			BrickStyle thirdStyle = VisualBrick.GetAreaStyle(LineBrick.PrintingSystem.Styles, baseStyle, thirdRect, clipBounds);
			baseStyle.BackColor = baseStyle.ForeColor;
			BrickStyle secondStyle = VisualBrick.GetAreaStyle(LineBrick.PrintingSystem.Styles, baseStyle, secondRect, clipBounds);
			List<BrickViewData> data = new List<BrickViewData>();
			AddBrickViewData(data, htmlExportContext, firstRect, firstStyle);
			AddBrickViewData(data, htmlExportContext, secondRect, secondStyle);
			AddBrickViewData(data, htmlExportContext, thirdRect, thirdStyle);
			return data.ToArray();
		}
		void AddBrickViewData(List<BrickViewData> data, ExportContext htmlExportContext, RectangleF rect, BrickStyle style) {
			if (!rect.IsEmpty)
				data.Add(htmlExportContext.CreateBrickViewData(style, rect, NullTableCell.Instance));
		}
		internal override void ProcessLayout(PageLayoutBuilder layoutBuilder, PointF pos, RectangleF clipRect) {
			Brick brick = (Brick)Brick;
			RectangleF brickRect = brick.GetViewRectangle();
			brickRect = layoutBuilder.ValidateLayoutRect(brick, brickRect);
			brickRect.Offset(pos);
			ProcessLayoutCore(layoutBuilder, brickRect, RectangleF.Intersect(clipRect, brickRect));
		}
	}
}
