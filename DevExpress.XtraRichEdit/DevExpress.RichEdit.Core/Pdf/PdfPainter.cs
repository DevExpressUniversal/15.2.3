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

#if !DXPORTABLE
using System;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Pdf;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.Layout;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Compatibility.System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit {
	public abstract class PdfPainterBase : Painter {
		readonly List<Tuple<Point, float>> transforms;
		readonly DocumentLayout layout;
		int clipBoundsTransformIndex = -1;
		RectangleF rectangularClipBounds; 
		PainterCache cache;
		PdfGraphics graphics;
		List<string> links = new List<string>();
		protected PdfPainterBase(DocumentLayout layout) {
			Guard.ArgumentNotNull(layout, "layout");
			this.layout = layout;
			this.transforms = new List<Tuple<Point, float>>();
		}
		public PainterCache Cache {
			get {
				if (cache == null) return PainterCache.DefaultCache;
				return cache;
			}
			set { cache = value; }
		}
		protected internal PdfGraphics PdfGraphics { get { return graphics; } }
		protected DocumentLayoutUnitConverter LayoutUnitConverter { get { return layout.DocumentModel.LayoutUnitConverter; } }
		protected BoxMeasurer Measurer { get { return layout.Measurer; } }
		public float Dpi { get { return layout.DocumentModel.LayoutUnitConverter.Dpi; } }
		public override bool HyperlinksSupported { get { return true; } }
		public override int DpiY { get { return (int)Dpi; } }
		protected override RectangleF RectangularClipBounds { get { return rectangularClipBounds; } }
		public void SetGraphics(PdfGraphics graphics, RectangleF clipBounds) {
			Guard.ArgumentNotNull(graphics, "graphics");
			this.graphics = graphics;
			graphics.IntersectClip(clipBounds);
			this.rectangularClipBounds = clipBounds;
			this.clipBoundsTransformIndex = 0;
		}
		protected override void SetClipBounds(RectangleF bounds) {
			this.rectangularClipBounds = bounds;
			this.clipBoundsTransformIndex = this.transforms.Count;
			UpdateGraphicsState();
		}
		void UpdateGraphicsState() {
			this.graphics.RestoreGraphicsState();
			this.graphics.SaveGraphicsState();
			ApplyTransforms();
		}
		void ApplyTransforms() {
			int count = this.transforms.Count;
			for (int i = 0; i < count; i++) {
				if (i == this.clipBoundsTransformIndex)
					this.graphics.IntersectClip(this.rectangularClipBounds);
				Tuple<Point, float> transform = this.transforms[i];
				this.graphics.TranslateTransform(transform.Item1.X, transform.Item1.Y);
				this.graphics.RotateTransform(transform.Item2);
				this.graphics.TranslateTransform(-transform.Item1.X, -transform.Item1.Y);
			}
			if (this.clipBoundsTransformIndex == count)
				this.graphics.IntersectClip(this.rectangularClipBounds);
		}
		public override void DrawImage(OfficeImage img, Rectangle bounds) {
			if (CanGetImageStream(img))
				this.graphics.DrawImage(GetImageStream(img), bounds);
			else
				this.graphics.DrawImage(img.NativeImage, bounds);
		}
		protected abstract bool CanGetImageStream(OfficeImage img);
		protected abstract Stream GetImageStream(OfficeImage img);
		public override void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			this.graphics.DrawLine(pen, x1, y1, x2, y2);
		}
		public override void DrawLines(Pen pen, PointF[] points) {
			this.graphics.DrawLines(pen, points);
		}
		public override void DrawRectangle(Pen pen, Rectangle bounds) {
			this.graphics.DrawRectangle(pen, bounds);
		}
		public override void DrawSpacesString(string text, FontInfo fontInfo, Rectangle rectangle) {
			DrawString(text, fontInfo, rectangle);
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle rectangle, StringFormat stringFormat) {
			DrawString(text, fontInfo, rectangle);
		}
		public override void DrawString(string text, Brush brush, Font font, float x, float y) {
			FontInfo fontInfo = GetFontInfo(font);
			if (fontInfo != null){
				TextViewInfo textInfo = GetTextViewInfo(text, fontInfo);
				DrawStringCore(text, fontInfo, textInfo, x, y);
			}
			else
				this.graphics.DrawString(text, font, Cache.GetSolidBrush(TextForeColor), x, y);
		}
		public override void DrawString(string text, FontInfo fontInfo, Rectangle rectangle) {
			TextViewInfo textInfo = GetTextViewInfo(text, fontInfo);
			DrawStringCore(text, fontInfo, textInfo, rectangle.X, rectangle.Y);
		}
		protected abstract void DrawStringCore(string text, FontInfo fontInfo, TextViewInfo textInfo, float x, float y);
		protected FontInfo GetFontInfo(Font font) {
			int fontIndex = layout.DocumentModel.FontCache.CalcFontIndex(font.Name, (int)(font.Size * 2), font.Bold, font.Italic, CharacterFormattingScript.Normal, false, false);
			return layout.DocumentModel.FontCache[fontIndex];
		}
		TextViewInfo GetTextViewInfo(string text, FontInfo fontInfo) {
			TextViewInfo textInfo = Measurer.TextViewInfoCache.TryGetTextViewInfo(text, fontInfo);
			if (textInfo == null) {
				textInfo = Measurer.CreateTextViewInfo(null, text, fontInfo);
				Measurer.TextViewInfoCache.AddTextViewInfo(text, fontInfo, textInfo);
			}
			return textInfo;
		}
		public override void FillEllipse(Brush brush, Rectangle bounds) {
			this.graphics.FillEllipse((SolidBrush)brush, bounds);
		}
		public override void FillPolygon(Brush brush, PointF[] points) {
			this.graphics.FillPolygon((SolidBrush)brush, points);
		}
		public override void FillRectangle(Color color, Rectangle bounds) {
			if (DXColor.IsTransparentOrEmpty(color))
				return;
			graphics.FillRectangle(Cache.GetSolidBrush(color), bounds);
		}
		public override void FillRectangle(Brush brush, Rectangle bounds) {
			graphics.FillRectangle((SolidBrush)brush, bounds);
		}
		public override Brush GetBrush(Color color) {
			return Cache.GetSolidBrush(color);
		}
		public override Pen GetPen(Color color, float thickness) {
			return Cache.GetPen(color, (int)thickness);
		}
		public override Pen GetPen(Color color) {
			return Cache.GetPen(color);
		}
		public override SizeF MeasureString(string text, Font font) {
			FontInfo fontInfo = GetFontInfo(font);
			if (fontInfo == null)
				return SizeF.Empty;
			return GetTextViewInfo(text, fontInfo).Size;
		}
		public override void PushRotationTransform(Point center, float angleInDegrees) {
			this.transforms.Add(new Tuple<Point, float>(center, angleInDegrees));
			UpdateGraphicsState();
		}
		public override void PopTransform() {
			this.transforms.RemoveAt(this.transforms.Count - 1);
#if DEBUGTEST
			Debug.Assert(this.clipBoundsTransformIndex <= this.transforms.Count);
#endif
			UpdateGraphicsState();
		}
		public override void PopPixelOffsetMode() {
		}
		public override void PopSmoothingMode() {
		}
		public override void PushPixelOffsetMode(bool highQualtity) {
		}
		public override void PushSmoothingMode(bool highQuality) {
		}
		public override void ReleaseBrush(Brush brush) { }
		public override void ReleasePen(Pen pen) { }
		public override void ExcludeCellBounds(Rectangle rect, Rectangle rowBounds) {
		}
		public override void ResetCellBoundsClip() {
		}
		protected override PointF[] TransformToLayoutUnits(PointF[] points) {
			return points;
		}
		protected override PointF[] TransformToPixels(PointF[] points) {
			return points;
		}
		public override PointF GetSnappedPoint(PointF point) {
			return point;
		}
		public override void SnapHeights(float[] heights) {
		}
		public override void SnapWidths(float[] widths) {
		}
		public override void SetUriArea(string uri, RectangleF bounds) {
			Uri target;
			if (!Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out target))
				return;
			PdfGraphics.AddLinkToUri(bounds, target);
		}
		public string SetLink(RectangleF bounds) {
			return PdfGraphics.AddLinkToPage(bounds);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (this.cache != null) {
					this.cache.Dispose();
					this.cache = null;
				}
			}
			base.Dispose(disposing);
		}
		internal RectangleF GetRectangularClipBounds() {
			return this.rectangularClipBounds;
		}
	}
}
#endif
