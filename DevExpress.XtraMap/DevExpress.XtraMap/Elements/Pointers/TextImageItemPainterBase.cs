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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing {
	public abstract class TextImageItemPainterBase {
		const int NormalStateImageIndex = 0;
		const int HighlightedStateImageIndex = 1;
		const int SelectedStateImageIndex = 2;
		protected const int StrokeSize = 4;
		public static readonly MapPoint DefaultRenderOrigin = new MapPoint(0.5, 0.5);
		ImageGeometry geometry;
		readonly object imageLocker = new object();
		ISupportImagePainter item;
		ISkinProvider skinProvider;
		Image sourceImage;
		Size sourceImageSize;
		protected virtual MapItemStyle ActualStyle { get { return Item.ActualStyle; } }
		protected virtual Color BaseColor { get { return Color.Empty; } }
		protected ImageGeometry Geometry { get { return geometry; } }
		protected ISupportImagePainter Item { get { return item; } }
		protected MapPointer Pointer { get { return Item as MapPointer; } }
		protected ISkinProvider SkinProvider { get { return skinProvider; } }
		protected Rectangle SourceImageRect { get; set; }
		protected Image TextImage { get; set; }
		protected Rectangle TextRect { get; set; }
		protected Size SourceImageSize { get { return sourceImageSize; } }
		public virtual bool AllowUseAntiAliasing { get { return string.IsNullOrEmpty(SourceText); } } 
		public Image FinalImage { get; set; }
		public Rectangle FinalImageRect { get; set; }
		public bool IsExternalImage{ get; protected set; }
		public Image SourceImage {
			get {
				lock(imageLocker)
					return sourceImage;
			}
			set {
				lock(imageLocker) {
					sourceImage = value;
					sourceImageSize = ImageSafeAccess.GetSize(sourceImage);
				}
			}
		}
		public string SourceText { get; set; }
		protected TextImageItemPainterBase(ISupportImagePainter item) {
			this.item = item;
			this.geometry = (ImageGeometry)item.Geometry;
		}
		protected abstract void CalcLayout();
		protected abstract void Draw();
		protected abstract SkinElement GetColoredSkinElement(SkinElement el, Color color);
		protected bool AllowDrawSkinBackground() {
			IInteractiveElement el = Item as IInteractiveElement;
			if(el != null) {
				if(el.IsSelected && ShouldUseBackgroundImageOneState(ElementState.Selected))
					return true;
				if(el.IsHighlighted && ShouldUseBackgroundImageOneState(ElementState.Highlighted))
					return true;
			}
			return ShouldUseBackgroundImageOneState(ElementState.Normal);
		}
		protected void ArrangeElements(Rectangle sourceImageRect, Rectangle textRect, Padding padding, int textPadding, TextAlignment textAlignment) {
			PointerLayoutCalculator layoutCalculator = new PointerLayoutCalculator();
			PointerLayoutResult layout = layoutCalculator.Calculate(sourceImageRect, textRect, padding, textPadding, textAlignment);
			TextRect = layout.TextRect;
			SourceImageRect = layout.ImageRect;
			FinalImageRect = layout.Rect;
		}
		protected Size CalcTextSize(string text, Size imageSize) {
			PrepareHtmlText(text);
			Size size = CalculateTextSizeInPixels(text);
			size.Width = Math.Min(size.Width, imageSize.Width);
			return size;
		}
		protected virtual void PrepareHtmlText(string text) {
		}
		protected virtual Size CalculateTextSizeInPixels(string text) {
			return MapUtils.CalcStringPixelSize(text, ActualStyle.Font);
		}
		protected internal virtual int CalculateImageIndex() {
			IInteractiveElement el = Item as IInteractiveElement;
			if(el != null) {
				if(el.IsSelected) return SelectedStateImageIndex;
				if(el.IsHighlighted) return HighlightedStateImageIndex;
			}
			return NormalStateImageIndex;
		}
		protected internal virtual Rectangle CalculateTextImageRect(string text) {
			if(!CanDrawText(text))
				return Rectangle.Empty;
			Size size = CalcTextSize(text, new Size(int.MaxValue, int.MaxValue));
			return new Rectangle(Point.Empty, size);
		}
		protected virtual Rectangle CalculateTextRect(string text, Size imageSize) {
			Rectangle result = CalculateTextImageRect(text);
			if(!string.IsNullOrEmpty(text)) {
				result = Rectangle.Inflate(result, StrokeSize / 2, StrokeSize / 2);
				result.Offset(StrokeSize / 2, StrokeSize / 2);
			}
			return result;
		}
		protected virtual bool CanDrawText(string text) {
			if(string.IsNullOrEmpty(text))
				return false;
			return MapUtils.CanDrawColor(GetTextColor()) || MapUtils.CanDrawColor(GetTextGlowColor());
		}
		protected Bitmap CreateSkinBackgroundImage(SkinElement backgroundElement, Rectangle bounds) {
			Color actualFill = Item.ActualStyle.Fill;
			SkinElement coloredEl = ShouldColorizeSkinElement(actualFill) ? GetColoredSkinElement(backgroundElement, actualFill) : backgroundElement;
			SkinElementInfo el = new SkinElementInfo(coloredEl, bounds);
			el.ImageIndex = CalculateImageIndex();
			Bitmap result = new Bitmap(bounds.Width, bounds.Height);
			using(Graphics gr = Graphics.FromImage(result)) {
				using(GraphicsCache cache = new GraphicsCache(gr)) {
					DrawBackground(cache, el, coloredEl.Image);
				}
			}
			return result;
		}
		void DrawBackground(GraphicsCache cache, SkinElementInfo el, SkinImage skinImage) {
			if(skinImage != null) {
				lock(skinImage.Image) {
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
				}
			}
			else
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected virtual Image CreateTextImage(string text, Rectangle textRect) {
			Bitmap image = null;
			if(!CanDrawText(text) || textRect.IsEmpty)
				return image;
			image = new Bitmap(textRect.Width, textRect.Height);
			using(Graphics gr = Graphics.FromImage(image)) {
				InitTextImageGraphics(gr);
				Rectangle bounds = new Rectangle(Point.Empty, textRect.Size);
				DrawText(gr, text, ActualStyle.Font, bounds);
			}
			return image;
		}
		protected void DrawImageTextImages(Image finalImage, Graphics gr) {
			if(finalImage == null)
				return;
			if(SourceImage != null) {
				ImageSafeAccess.Draw(gr, SourceImage, SourceImageRect);
			}
			if(TextImage != null)
				ImageSafeAccess.Draw(gr, TextImage, TextRect);
		}
		protected virtual void DrawText(Graphics gr, string text, Font font, Rectangle outputRect) {
			using(GraphicsPath path = new GraphicsPath()) {
				float emSize = gr.DpiY * font.Size / 72;
				path.AddString(text, font.FontFamily, (int)font.Style, emSize, outputRect, MapUtils.EllipsisStringFormat);
				Color textGlowColor = GetTextGlowColor();
				if(MapUtils.CanDrawColor(textGlowColor))
					using(Pen pen = new Pen(textGlowColor, StrokeSize)) {
						pen.LineJoin = LineJoin.Round;
						gr.DrawPath(pen, path);
					}
				Color textColor = GetTextColor();
				if(MapUtils.CanDrawColor(textColor))
					using(SolidBrush brush = new SolidBrush(textColor))
						gr.FillPath(brush, path);
			}
		}
		protected virtual Color GetTextGlowColor() {
			return ActualStyle.TextGlowColor;
		}
		protected virtual Color GetTextColor() {
			return ActualStyle.TextColor;
		}
		protected void InitTextImageGraphics(Graphics gr) {
			gr.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
			gr.SmoothingMode = SmoothingMode.AntiAlias;
			gr.InterpolationMode = InterpolationMode.High;
			gr.Clear(Color.Transparent);
		}
		protected virtual void OnDrawComplete() {
		}
		protected bool ShouldColorizeSkinElement(Color color) {
			return !MapUtils.IsColorEmpty(color) && Color.Transparent != color;
		}
		protected virtual bool ShouldUseBackgroundImage() {
			return false;
		}
		protected bool ShouldUseBackgroundImageOneState(ElementState state) {
			return (Pointer.SkinBackgroundVisibility & state) == state;
		}
		protected internal virtual void ResetFinalImage() {
			if(FinalImage != null) {
				FinalImage.Dispose();
				FinalImage = null;
			}
		}
		public void Draw(ISkinProvider skinProvider) {
			this.skinProvider = skinProvider;
			CalcLayout();
			Draw();
			OnDrawComplete();
		}
		public void ClearSourceImage() {
			if(this.sourceImage == null)
				return;
			this.sourceImage.Dispose();
			this.sourceImage = null;
		}
	}
	public class PushpinPainter : TextImageItemPainterBase {
		public static readonly MapPoint DefaultSkinImageOrigin = new MapPoint(0.5, 1.0);
		protected MapPushpin Pushpin { get { return (MapPushpin)Item; } }
		public override bool AllowUseAntiAliasing { get { return false; } } 
		public PushpinPainter(MapPointer pointer)
			: base(pointer) {
		}
		protected override void CalcLayout() {
			if(SourceImage == null)
				return;
			Size imageSize = ImageSafeAccess.GetSize(SourceImage);
			TextRect = CalculateTextRect(SourceText, imageSize);
			TextImage = CreateTextImage(SourceText, TextRect);
			FinalImageRect = new Rectangle(Point.Empty, imageSize);
		}
		protected override SkinElement GetColoredSkinElement(SkinElement el, Color color) {
			return el;
		}
		protected override void Draw() {
			lock(SourceImage) {
				if (string.IsNullOrEmpty(SourceText)) {
					FinalImage = SourceImage;
					IsExternalImage = true;
				}
				else
					FinalImage = new Bitmap(SourceImage);
			}
			DrawToFinalImage(FinalImage);
		}
		protected override Rectangle CalculateTextRect(string text, Size imageSize) {
			if(string.IsNullOrEmpty(text))
				return Rectangle.Empty;
			Point textOrigin = Pushpin.ActualTextOrigin;
			Point location = new Point(textOrigin.X - imageSize.Width / 2, textOrigin.Y - imageSize.Height / 2);
			return new Rectangle(location, imageSize);
		}
		protected override void OnDrawComplete() {
			Pushpin.RecalculateImageOrigin();
		}
		protected internal override void ResetFinalImage() {
			if(IsExternalImage)
				FinalImage = null;
			else
				base.ResetFinalImage();
		}
		void DrawToFinalImage(Image image) {
			if(TextImage == null || TextRect.IsEmpty)
				return;
			using(Graphics gr = Graphics.FromImage(image)) {
				ImageSafeAccess.DrawUnscaled(gr, TextImage, TextRect.X, TextRect.Y);
			}
		}
	}
	public abstract class PointerPainterBase : TextImageItemPainterBase {
		protected bool AllowHtmlText { get { return Pointer.AllowHtmlTextCore; } }
		protected bool UseHtmlText { get { return Pointer.HtmlStringInfo != null && !Pointer.HtmlStringInfo.SimpleString; } }
		protected PointerPainterBase(ISupportImagePainter pointer) : base(pointer) {
		}
		protected Color CalculateBaseColor(MapPointerStyle style) {
			MapPointer pointer = (MapPointer)Item;
			if (pointer.IsSelected) return style.SelectedBaseColor;
			if (pointer.IsHighlighted) return style.HighlightedBaseColor;
			return style.BaseColor;
		}
		protected override Size CalculateTextSizeInPixels(string text) {
			if(UseHtmlText)
				return Pointer.HtmlStringInfo.Bounds.Size;
			return base.CalculateTextSizeInPixels(text);
		}
		protected override Image CreateTextImage(string text, Rectangle textRect) {
			if(!UseHtmlText)
				return base.CreateTextImage(text, textRect);
			return null;
		}
		protected virtual void DrawHtmlText(Graphics gr, Rectangle outputRect) {
			DevExpress.Utils.Text.StringInfo htmlStringInfo = Pointer.HtmlStringInfo;
			if(htmlStringInfo == null || htmlStringInfo.IsEmpty)
				return;
			using(GraphicsCache cache = new GraphicsCache(new PaintEventArgs(gr, Rectangle.Empty), Native.MapUtils.TextPainter)) {
				htmlStringInfo.Offset(outputRect.Location.X, outputRect.Location.Y);
				StringPainter.Default.DrawString(cache, htmlStringInfo);
			}
		}
		protected override void PrepareHtmlText(string text) {
			if(AllowHtmlText) Pointer.UpdateHtmlStringInfo();
		}
	}
	public class CustomElementPainter : PointerPainterBase {
		static readonly Padding defaultPadding = new Padding(16);
		public static Padding DefaultPadding { get { return defaultPadding; } }
		const int DefaultPanelCornerRadius = 4;
		protected MapCustomElement Element { get { return (MapCustomElement)Item; } }
		protected MapPointerStyle CustomElementStyle { get { return Element.DefaultStyleProvider.CustomElementStyle; } }
		protected override Color BaseColor { get { return CalculateBaseColor(CustomElementStyle); } }
		public CustomElementPainter(MapPointer pointer)
			: base(pointer) {
		}
		protected override bool ShouldUseBackgroundImage() {
			return CustomElementStyle.BackgroundElement != null && (AllowDrawSkinBackground() || AllowHtmlText);
		}
		protected override void CalcLayout() {
			TextRect = CalculateTextRect(SourceText, new Size());
			TextImage = CreateTextImage(SourceText, TextRect);
			Rectangle sourceImageRect = new Rectangle(Point.Empty, SourceImageSize);
			ArrangeElements(sourceImageRect, TextRect, Element.ActualPadding, Element.TextPadding, Element.TextAlignment);
		}
		protected override void OnDrawComplete() {
			Element.SoureImageLocation = SourceImageRect.Location;
			Element.RecalculateImageOrigin(SourceImageRect.Size, FinalImageRect.Size, SourceImageRect.Location);
		}
		protected override void Draw() {
			FinalImage = CreateBackgroundImage();
			DrawToFinalImage(FinalImage);
		}
		protected void DrawToFinalImage(Image finalImage) {
			if(finalImage == null)
				return;
			using(Graphics gr = Graphics.FromImage(finalImage)) {
				using(Pen pen = new Pen(ActualStyle.Stroke, ActualStyle.StrokeWidth)) {
					if(CustomElementStyle.BackgroundElement == null) 
						DrawRoundedRectangle(gr, FinalImageRect, DefaultPanelCornerRadius, pen, ActualStyle.Fill);
				}
				DrawImageTextImages(finalImage, gr);
				if(UseHtmlText) DrawHtmlText(gr, TextRect);
			}
		}
		Bitmap CreateBackgroundImage() {
			Bitmap result;
			if(ShouldUseBackgroundImage()) {
				result = CreateSkinBackgroundImage(CustomElementStyle.BackgroundElement, FinalImageRect);
				if(result != null) return result;
			}
			return FinalImageRect.Width > 0 && FinalImageRect.Height > 0 ? new Bitmap(FinalImageRect.Width, FinalImageRect.Height) : null;
		}
		protected override SkinElement GetColoredSkinElement(SkinElement el, Color color) {
			LayerColoredSkinElementCache cache = Pointer.LayerColoredSkinElementCache;
			return (cache != null) ? cache.GetCustomElementSkinElement(el, BaseColor, color) : el;
		}
		void DrawRoundedRectangle(Graphics gr, Rectangle bounds, int cornerRadius, Pen drawPen, Color fillColor) {
			gr.SmoothingMode = SmoothingMode.AntiAlias;
			int strokeOffset = Convert.ToInt32(Math.Ceiling(drawPen.Width));
			bounds = Rectangle.Inflate(bounds, -strokeOffset, -strokeOffset);
			drawPen.EndCap = drawPen.StartCap = LineCap.Round;
			using(GraphicsPath gfxPath = new GraphicsPath()) {
				gfxPath.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
				gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90);
				gfxPath.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
				gfxPath.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
				gfxPath.CloseAllFigures();
				using(Brush brush = new SolidBrush(fillColor)) {
					gr.FillPath(brush, gfxPath);
				}
				if(MapUtils.CheckValidPen(drawPen))
					gr.DrawPath(drawPen, gfxPath);
			}
		}
	}
	public class CalloutPainter : PointerPainterBase {
		static readonly Size defaultCursorSize = new Size(8, 12);
		static readonly Point defaultBaseOffset = new Point(DefaultCursorSize.Width, 0);
		static readonly Padding defaultPadding = new Padding(8);
		protected internal static Size DefaultCursorSize { get { return defaultCursorSize; } }
		protected internal static Point DefaultBaseOffset { get { return defaultBaseOffset; } }
		protected internal static Padding DefaultPadding { get { return defaultPadding; } }
		Size cursorSize = DefaultCursorSize;
		protected MapCallout Callout { get { return (MapCallout)Item; } }
		protected MapCalloutStyle CalloutStyle { get { return Callout.DefaultStyleProvider.CalloutStyle; } }
		protected override Color BaseColor { get {return CalculateBaseColor(CalloutStyle); } }
		protected Size CursorSize { get { return cursorSize; } }
		public CalloutPainter(MapCallout pointer)
			: base(pointer) {
		}
		void ApplyCursorSize(Size cursorSize) {
			Rectangle result = FinalImageRect;
			result.Width = Math.Max(FinalImageRect.Width, cursorSize.Width);
			result.Height += cursorSize.Height;
			FinalImageRect = result;
		}
		void DrawDefaultBackground(Graphics gr, Rectangle bounds) {
			using(Pen pen = new Pen(ActualStyle.Stroke, ActualStyle.StrokeWidth)) {
				pen.EndCap = pen.StartCap = LineCap.Round;
				using(GraphicsPath gfxPath = new GraphicsPath()) {
					bounds = Rectangle.Inflate(bounds, -ActualStyle.StrokeWidth, -ActualStyle.StrokeWidth);
					gfxPath.AddLines(CalculatePathPoints(bounds));
					using(Brush brush = new SolidBrush(ActualStyle.Fill)) {
						gr.FillPath(brush, gfxPath);
					}
					if(MapUtils.CheckValidPen(pen))
						gr.DrawPath(pen, gfxPath);
				}
			}
		}
		protected override SkinElement GetColoredSkinElement(SkinElement el, Color color) {
			LayerColoredSkinElementCache cache = Pointer.LayerColoredSkinElementCache;
			return (cache != null) ? cache.GetCalloutSkinElement(el, BaseColor, color) : el;
		}
		Point[] CalculatePathPoints(Rectangle bounds) {
			int bottom = bounds.Y + bounds.Height - CursorSize.Height;
			Point LeftTop = bounds.Location;
			Point RightTop = RectUtils.GetRightTop(bounds);
			Point LeftBottom = new Point(LeftTop.X, bottom);
			Point RightBottom = new Point(RightTop.X, bottom);
			Point CursorLeftTop = new Point(CursorSize.Width, bounds.Bottom - CursorSize.Height);
			Point CursorBottom = new Point(CursorSize.Width, bounds.Bottom);
			Point CursorRightTop = new Point(2 * CursorSize.Width, CursorLeftTop.Y);
			return new Point[] { LeftTop, LeftBottom, CursorLeftTop, CursorBottom, CursorRightTop, RightBottom, RightTop, LeftTop };
		}
		protected void DrawToFinalImage(Image finalImage) {
			if(finalImage == null)
				return;
			using(Graphics gr = Graphics.FromImage(finalImage)) {
				DrawImageTextImages(finalImage, gr);
				if(UseHtmlText) DrawHtmlText(gr, TextRect);
			}
		}
		protected void DrawDefault() {
			FinalImage = new Bitmap(FinalImageRect.Width, FinalImageRect.Height);
			using(Graphics gr = Graphics.FromImage(FinalImage)) {
				DrawDefaultBackground(gr, FinalImageRect);
				DrawImageTextImages(FinalImage, gr);
			}
		}
		protected override void OnDrawComplete() {
			Callout.RecalculateImageOrigin(FinalImageRect.Size, CalloutStyle.BaseOffset);
		}
		protected override void DrawText(Graphics gr, string text, Font font, Rectangle outputRect) {
			using(GraphicsCache cache = new GraphicsCache(gr)) {
				using(SolidBrush brush = new SolidBrush(ActualStyle.TextColor)) {
					MapUtils.TextPainter.DrawString(cache, text, font, brush, outputRect, StringFormat.GenericTypographic);
				}
			}
		}
		protected override bool ShouldUseBackgroundImage() {
			return CalloutStyle.BackgroundElement != null && AllowDrawSkinBackground();
		}
		protected override void Draw() {
			if(RectUtils.IsBoundsEmpty(FinalImageRect)) {
				FinalImage = null;
				return;
			}
			if(ShouldUseBackgroundImage()) {
				FinalImage = CreateSkinBackgroundImage(CalloutStyle.BackgroundElement, FinalImageRect);
			}
			if(FinalImage != null)
				DrawToFinalImage(FinalImage);
			else
				DrawDefault();
		}
		protected override void CalcLayout() {
			TextRect = CalculateTextImageRect(SourceText);
			TextImage = CreateTextImage(SourceText, TextRect);
			Size imageSize = ImageSafeAccess.GetSize(SourceImage);
			Rectangle sourceImageRect = new Rectangle(Point.Empty, imageSize);
			Padding padding = CalloutStyle.ContentPadding;
			ArrangeElements(sourceImageRect, TextRect, padding, Callout.TextPadding, Callout.TextAlignment);
			ApplyCursorSize(CalloutStyle.PointerSize);
		}
	}
	public class ShapeTitlePainter : TextImageItemPainterBase {
		Color textColor;
		Color textGlowColor;
		Color TextColor { get { return textColor; } }
		Color TextGlowColor { get { return textGlowColor; } }
		protected override MapItemStyle ActualStyle {
			get { return Title.Shape.ActualStyle; }
		}
		protected ShapeTitle Title { get { return (ShapeTitle)Item; } }
		public ShapeTitlePainter(ISupportImagePainter shape)
			: base(shape) {
			this.textColor = Title.Shape.GetTitleColor();
			this.textGlowColor = Title.Shape.GetTitleGlowColor();
		}
		protected override void CalcLayout() {
			TextRect = CalculateTextRect(SourceText, new Size());
		}
		protected override void Draw() {
			FinalImage = CreateTextImage(SourceText, TextRect);
			FinalImageRect = TextRect;
		}
		protected override Color GetTextGlowColor() {
			return TextGlowColor;
		}
		protected override Color GetTextColor() {
			return TextColor;
		}
		protected override SkinElement GetColoredSkinElement(SkinElement el, Color color) {
			return el;
		}
	}
}
