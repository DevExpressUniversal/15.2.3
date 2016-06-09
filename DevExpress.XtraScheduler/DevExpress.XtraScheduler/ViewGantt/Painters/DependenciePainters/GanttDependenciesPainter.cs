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

using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class GanttDependenciesPainter : IViewInfoItemPainter {
		protected const int ViewInfoMinLenght = 3;
		Image defaultArrowImage;
		Color baseColor = Color.Gray;
		Color actualColor;
		protected GanttDependenciesPainter() {
			this.defaultArrowImage = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraScheduler.Images.dependencyArrow.png", System.Reflection.Assembly.GetExecutingAssembly());
		}
		internal Color ActualColor { get { return actualColor; } set { actualColor = value; } }
		public virtual void DrawDependencies(GraphicsCache cache, GanttViewInfo ganttViewInfo) {
			DependencyViewInfoCollection copiedDependencyViewInfos;
			lock (ganttViewInfo.DependencyViewInfos)
				copiedDependencyViewInfos = new DependencyViewInfoCollection(ganttViewInfo.DependencyViewInfos);
			ISupportCustomDraw customDrawProvider = ganttViewInfo.View.Control;
			int count = copiedDependencyViewInfos.Count;
			for (int i = 0; i < count; i++)
				DrawDependency(cache, copiedDependencyViewInfos[i], customDrawProvider);
		}
		protected internal virtual void DrawDependency(GraphicsCache cache, DependencyViewInfo viewInfo, ISupportCustomDraw customDrawProvider) {
			DefaultDrawDelegate defaultDraw = delegate() { DrawDependencyCore(cache, viewInfo); };
			if (RaiseCustomDrawDependency(cache, viewInfo, customDrawProvider, defaultDraw))
				return;
			defaultDraw();
		}
		protected internal virtual bool RaiseCustomDrawDependency(GraphicsCache cache, DependencyViewInfo viewInfo, ISupportCustomDraw customDrawProvider, DefaultDrawDelegate defaultDrawDelegate) {
			viewInfo.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(viewInfo, viewInfo.Bounds, defaultDrawDelegate);
				customDrawProvider.RaiseCustomDrawDependency(args);
				return args.Handled;
			} finally {
				viewInfo.Cache = null;
			}
		}
		protected internal virtual void DrawDependencyCore(GraphicsCache cache, DependencyViewInfo viewInfo) {
			ActualColor = GetActualColor(viewInfo);
			DrawLine(cache, viewInfo);
			DrawDependencyItems(cache, viewInfo.Items);
		}
		protected internal virtual void DrawLine(GraphicsCache cache, DependencyViewInfo viewInfo) {
			if (!viewInfo.LineVisible)
				return;
			Point start = viewInfo.LineStart;
			Point end = viewInfo.LineEnd;
			if (start == end)
				DrawPixel(cache, start);
			else
				DrawLineCore(cache, start, end);
		}
		protected internal virtual void DrawPixel(GraphicsCache cache, Point point) {
			Brush brush = cache.GetSolidBrush(ActualColor);
			cache.Graphics.FillRectangle(brush, point.X, point.Y, 1, 1);
		}
		protected internal virtual void DrawLineCore(GraphicsCache cache, Point start, Point end) {
			Pen pen = new Pen(ActualColor);
			cache.Graphics.DrawLine(pen, start, end);
		}
		protected internal virtual Color GetActualColor(DependencyViewInfo viewInfo) {
			if (viewInfo.Selected)
				return viewInfo.SelectedAppearance.ForeColor;
			return viewInfo.Appearance.ForeColor;
		}
		protected internal virtual void DrawDependencyItems(GraphicsCache cache, ViewInfoItemCollection items) {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				items[i].Draw(cache, this);
		}
		#region IViewInfoItemPainter Members
		void IViewInfoItemPainter.DrawTextItem(GraphicsCache cache, ViewInfoTextItem item) {
		}
		void IViewInfoItemPainter.DrawVerticalTextItem(GraphicsCache cache, ViewInfoVerticalTextItem item) {
		}
		void IViewInfoItemPainter.DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
			this.DrawImageItem(cache, item);
		}
		void IViewInfoItemPainter.DrawHorizontalLineItem(GraphicsCache cache, ViewInfoHorizontalLineItem item) {
		}
		#endregion
		protected internal virtual Image GetArrowImage() {
			return defaultArrowImage;
		}
		protected internal virtual Image GetCornerImage() {
			return null;
		}
		protected internal virtual Color GetBaseColor() {
			return baseColor;
		}
		protected internal virtual Color GetDependencyColor() {
			return Color.Empty;
		}
		protected internal virtual Color GetSelectedDependencyColor() {
			return Color.Empty;
		}
		protected internal virtual int GetConnectorIndent() {
			return GetDistanceFromAppointment();
		}
		protected internal virtual int GetDistanceFromAppointment() {
			return GetArrowWidth() + ViewInfoMinLenght;
		}
		protected internal virtual int GetArrowWidth() {
			return defaultArrowImage.Width;
		}
		protected internal virtual void DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
			Image image = PaintImageWithColor(item.Image, ActualColor);
			using (IntersectClipper clipper = new IntersectClipper(cache, item.Bounds)) {
				cache.Paint.DrawImage(cache.Graphics, image, item.Bounds.Location);
			}
		}
		protected internal virtual Image PaintImageWithColor(Image image, Color color) {
			ColorMatrix colorMatrix = GetColorMatrixConsiderAlphaChannel(GetBaseColor(), color);
			return PaintImageWithColor(image, colorMatrix);
		}
		ColorMatrix GetColorMatrixConsiderAlphaChannel(Color baseColor, Color currentColor) {
			ColorMatrix result = DevExpress.Utils.Paint.XPaint.GetColorMatrix(baseColor, currentColor);
			result.Matrix33 = ((float)currentColor.A) / 255.0f;
			return result;
		}
		Bitmap PaintImageWithColor(Image originalImage, ColorMatrix colorMatrix) {
			Bitmap newImage = new Bitmap(originalImage.Width, originalImage.Height);
			using (Graphics g = Graphics.FromImage(newImage)) {
				g.Clear(Color.Transparent);
				using (ImageAttributes attributes = new ImageAttributes()) {
					attributes.SetColorMatrix(colorMatrix);
					g.DrawImage(originalImage, new Rectangle(Point.Empty, newImage.Size), 0, 0, newImage.Width, newImage.Height, GraphicsUnit.Pixel, attributes);
				}
			}
			return newImage;
		}
	}
}
