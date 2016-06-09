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
using DevExpress.XtraRichEdit.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Snap.Core.UI.Templates;
using DevExpress.Snap.Core.Native;
using DevExpress.Office.Drawing;
namespace DevExpress.Snap.Native.PageDecorators {
	public class TemplateDecoratorItemPainter {
		readonly GdiPlusPainter painter;		
		readonly ILayoutToPhysicalBoundsConverter converter;
		public TemplateDecoratorItemPainter(GdiPlusPainter painter, ILayoutToPhysicalBoundsConverter converter) {
			this.painter = painter;
			this.converter = converter;
		}
		public void DrawWithCaption(TemplateDecoratorItem item, TemplateDecoratorItemBorder itemBorder, Font font, string caption) {
			Point[][] paths = DrawCore(item, itemBorder);
			DrawCaption(paths, itemBorder, font, caption);
		}
		public void Draw(TemplateDecoratorItem item, TemplateDecoratorItemBorder itemBorder) {
			DrawCore(item, itemBorder);
		}
		Point[][] DrawCore(TemplateDecoratorItem item, TemplateDecoratorItemBorder itemBorder) {			
			Point[][] paths = item.GetGraphicsPathPoints(converter);
			foreach (Point[] pts in paths)
				DrawPath(pts, itemBorder);
			return paths;
		}
		public void FillRegionBetweenTemplateItems(TemplateDecoratorItem innerItem, TemplateDecoratorItem outerItem, Brush fill) {			
			Point[][] innerPaths = innerItem.GetGraphicsPathPoints(converter);
			Point[][] outerPaths = outerItem.GetGraphicsPathPoints(converter);
			using (Region innerRegion = CreateRegion(innerPaths)) {
				using (Region outerRegion = CreateRegion(outerPaths)) {
					outerRegion.Exclude(innerRegion);
					painter.Cache.Graphics.FillRegion(fill, outerRegion);
				}
			}
		}
		public void FillRegionInsideTemplateItem(TemplateDecoratorItem item, Brush fill) {
			Point[][] paths = item.GetGraphicsPathPoints(converter);
			using (Region region = CreateRegion(paths))
				painter.Cache.Graphics.FillRegion(fill, region);
		}
		Region CreateRegion(Point[][] paths) {
			Region region = new Region();
			region.MakeEmpty();
			foreach (Point[] pts in paths) {
				using (GraphicsPath path = new GraphicsPath()) {
					path.AddPolygon(pts);
					region.Union(path);
				}
			}
			return region;
		}
		void DrawPath(Point[] pts, TemplateDecoratorItemBorder itemBorder) {			
			for (int i = 0; i < pts.Length - 1; i++) {
				Point from = pts[i];
				Point to = pts[i + 1];
				DrawSegment(from, to, itemBorder);
			}
			DrawSegment(pts[pts.Length - 1], pts[0], itemBorder);
		}
		void DrawSegment(Point from, Point to, TemplateDecoratorItemBorder itemBorder) {
			if (from.X == to.X)
				DrawVerticalSegment(from, to, itemBorder);
			else
				DrawHorizontalSegment(from, to, itemBorder);
		}
		void DrawHorizontalSegment(Point from, Point to, TemplateDecoratorItemBorder itemitemBorderLook) {
			int lineWidth = itemitemBorderLook.BorderWidth;
			Brush brush = itemitemBorderLook.BorderBrush;
			if (from.X < to.X)
				painter.Cache.FillRectangle(brush, Rectangle.FromLTRB(from.X - lineWidth, from.Y - lineWidth, to.X, to.Y));
			else
				painter.Cache.FillRectangle(brush, Rectangle.FromLTRB(to.X, to.Y, from.X + lineWidth, from.Y + lineWidth));
		}
		void DrawVerticalSegment(Point from, Point to, TemplateDecoratorItemBorder itemBorder) {
			int lineWidth = itemBorder.BorderWidth;
			Brush brush = itemBorder.BorderBrush;
			if (from.Y < to.Y)
				painter.Cache.FillRectangle(brush, Rectangle.FromLTRB(from.X, from.Y - lineWidth, to.X + lineWidth, to.Y));
			else
				painter.Cache.FillRectangle(brush, Rectangle.FromLTRB(to.X - lineWidth, to.Y, from.X, from.Y + lineWidth));
		}		
		protected virtual void DrawCaption(Point[][] paths, TemplateDecoratorItemBorder itemBorder, Font font, string caption) {
			if (String.IsNullOrEmpty(caption))
				return;
			Point? position = GetCaptionPosition(paths);
			if (position == null)
				return;
			Size stringSize = MeasureString(painter, font, caption);
			Size padding = TemplateDecoratorItemLookAndFeelProperties.CaptionTextPadding;
			Size backgroundSize = new Size(stringSize.Width + padding.Width * 2, stringSize.Height + padding.Height * 2);
			Point positionValue = position.Value;
			Rectangle bounds = new Rectangle(positionValue.X - backgroundSize.Width, positionValue.Y, backgroundSize.Width, backgroundSize.Height);
			Rectangle textBounds = bounds;
			textBounds.X += padding.Width;
			textBounds.Y += padding.Height;
			textBounds.Size = stringSize;
			painter.Cache.FillRectangle(itemBorder.BorderBrush, bounds);
			painter.Cache.DrawString(caption, font, Brushes.White, textBounds, StringFormat.GenericTypographic);
		}
		protected internal virtual Point? GetCaptionPosition(Point[][] paths) {
			int pathsCount = paths.Length;
			if (pathsCount <= 0)
				return null;
			Point[] path = paths[pathsCount - 1];
			int count = path.Length;
			if (count <= 0)
				return null;
			int maxY = path[0].Y;
			int maxIndex = 0;
			for (int i = 1; i < count; i++) {
				if (path[i].Y > maxY) {
					maxY = path[i].Y;
					maxIndex = i;
				}
			}
			int maxX = path[maxIndex].X;
			for (int i = 0; i < count; i++) {
				if (path[i].Y == maxY && path[i].X > maxX) {
					maxX = path[i].X;
					maxIndex = i;
				}
			}
			return path[maxIndex];
		}
		protected internal virtual Size MeasureString(GdiPlusPainter painter, Font font, string caption) {
			SizeF size = painter.Cache.CalcTextSize(caption, font, StringFormat.GenericTypographic, Int32.MaxValue);
			return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
		}
	}
}
