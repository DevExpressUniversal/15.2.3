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
using System.ComponentModel;
using System.Collections;
using DevExpress.Printing.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using System.IO;
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Media;
using DevExpress.Xpf.Collections;
using DevExpress.Xpf.Drawing.Imaging;
#else
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.Native.TextRotation;
#endif
namespace DevExpress.XtraPrinting.Drawing {
	[
	TypeConverter(typeof(DevExpress.Utils.Design.EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder)),
	]
	public enum DirectionMode {
		Horizontal = 0,
		ForwardDiagonal = 1,
		BackwardDiagonal = 2,
		Vertical = 4 
	};
	[
	TypeConverter(typeof(DevExpress.Utils.Design.EnumTypeConverter)),
	ResourceFinder(typeof(DevExpress.Data.ResFinder)),
	]
	public enum ImageViewMode {
		Clip = 0,
		Stretch = 1,
		Zoom = 2 
	};
#if SL
	public class PageWatermark {
#else
	public class PageWatermark : IDisposable {
#endif
		#region static
		protected static Font CreateDefaultFont() {
			try {
				return new Font("Verdana", 36);
			} catch {
				return new Font("Tahoma", 36);
			}
		}
		internal static Image CloneImage(Image img) {
			return img != null ? (Image)img.Clone() : null;
		}
		#endregion
		#region fields & properties
		string text = "";
		int transparency = 50;
		int imageTransparency;
		bool imageTiling;
		Color foreColor = DXColor.Red;
		ImageViewMode imageViewMode = ImageViewMode.Clip;
		DirectionMode textDirection = DirectionMode.ForwardDiagonal;
		ContentAlignment imageAlign = ContentAlignment.MiddleCenter;
		Font font;
		Image image;		
		bool showBehind = true;
#if !SL
		Image actualImage;
		StringFormat sf;	 
		internal StringFormat StringFormat { get { return sf; } }
#endif
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkShowBehind"),
#endif
		XtraSerializableProperty, 
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.ShowBehind"),
		]
		public bool ShowBehind { get { return showBehind; } set { showBehind = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkImage"),
#endif
		XtraSerializableProperty,
		DefaultValue(null),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.Image"),
		RefreshProperties(RefreshProperties.All),
#if !SL
		TypeConverter(typeof(DevExpress.Utils.Design.ImageTypeConverter)),
#endif
		]
		public virtual Image Image {
			get { return image; }
			set {
				image = value;
#if !SL
				DisposeActualImage();
#endif
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkImageAlign"),
#endif
		XtraSerializableProperty, 
		DefaultValue(ContentAlignment.MiddleCenter),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.ImageAlign"),
		TypeConverter(typeof(ContentAlignmentTypeConverter)),
		]
		public ContentAlignment ImageAlign { get { return imageAlign; } set { imageAlign = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkImageViewMode"),
#endif
		XtraSerializableProperty, 
		DefaultValue(ImageViewMode.Clip),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.ImageViewMode"),
		]
		public ImageViewMode ImageViewMode { get { return imageViewMode; } set { imageViewMode = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkImageTiling"),
#endif
		XtraSerializableProperty, 
		DefaultValue(false),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.ImageTiling"),
		TypeConverter(typeof(BooleanTypeConverter)),
		]
		public bool ImageTiling { get { return imageTiling; } set { imageTiling = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkTextDirection"),
#endif
		XtraSerializableProperty, 
		DefaultValue(DirectionMode.ForwardDiagonal),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.TextDirection"),
		]
		public DirectionMode TextDirection { get { return textDirection; } set { textDirection = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkText"),
#endif
		XtraSerializableProperty,
		DefaultValue(""),
		TypeConverter(typeof(Design.WMTextConverter)),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.Text"),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public string Text { get { return text; } set { text = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkFont"),
#endif
		XtraSerializableProperty,
		DefaultValue(typeof(Font), "Verdana, 36pt"),		
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.Font"),
#if !SL
		TypeConverter(typeof(FontTypeConverter)),
#endif
		]
		public Font Font {
			get { return font; }
			set {
				if (value != null) {
					if (font != null)
						font.Dispose();
					font = (Font)value.Clone();
				}
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkForeColor"),
#endif
		XtraSerializableProperty, 
		DefaultValue(typeof(Color), "Red"),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.ForeColor"),
		]
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkTextTransparency"),
#endif
		XtraSerializableProperty, 
		DefaultValue(50),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.TextTransparency"),
		]
		public int TextTransparency { get { return transparency; } set { transparency = Math.Max(0, Math.Min(value, 255)); } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageWatermarkImageTransparency"),
#endif
		XtraSerializableProperty, 
		DefaultValue(0),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.PageWatermark.ImageTransparency"),
		]
		public int ImageTransparency {
			get { return imageTransparency; }
			set {
				imageTransparency = Math.Max(0, Math.Min(value, 255));
#if !SL
				DisposeActualImage();
#endif
			}
		}
#if !SL
#if DEBUGTEST
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public
#endif
		Image ActualImage {
			get {
				if (actualImage == null)
					actualImage = CreateTransparentBitmap(image, ImageTransparency);
				return actualImage;
			}
		}
#endif
		#endregion
		public PageWatermark() {
#if !SL
			sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
#endif
			font = CreateDefaultFont();
		}
		public void Dispose() {
#if !SL
			sf.Dispose();
			DisposeGdiResources();
#endif
		}
#if !SL
		void DisposeGdiResources() {
			if (font != null) {
				font.Dispose();
				font = null;
			}
			if (image != null) {
				image.Dispose();
				image = null;
			}
			DisposeActualImage();
		}
		void DisposeActualImage() {
			if (actualImage != null) {
				actualImage.Dispose();
				actualImage = null;
			}
		}
		Bitmap CreateTransparentBitmap(Image original, int imageTransparency) {
			if (original == null)
				return null;
			if(imageTransparency == 0 && original is Bitmap)
				return (Bitmap)original;
			Bitmap bitmap;
			try {
				bitmap = BitmapCreator.CreateClearBitmap(original, 600);
			} catch {
				bitmap = BitmapCreator.CreateClearBitmap(original, GraphicsDpi.Pixel);	   
			}
			ImageAttributes attributes = BitmapCreator.CreateTransparencyAttributes(imageTransparency);
			try {
				BitmapCreator.TransformBitmap(original, bitmap, attributes);
			} catch {
				bitmap = BitmapCreator.CreateClearBitmap(original, GraphicsDpi.Pixel);
				BitmapCreator.TransformBitmap(original, bitmap, attributes);
			}
			return bitmap;
		}
#endif
		public override bool Equals(object obj) {
			PageWatermark wm = obj as PageWatermark;
			if (wm != null) {
				return Comparer.Equals(text, wm.text) &&
					Comparer.Equals(showBehind, wm.showBehind) &&
					Comparer.Equals(transparency, wm.transparency) &&
					Comparer.Equals(imageTransparency, wm.ImageTransparency) &&
					Comparer.Equals(imageTiling, wm.imageTiling) &&
					Comparer.Equals(foreColor, wm.foreColor) &&
					Comparer.Equals(imageViewMode, wm.imageViewMode) &&
					Comparer.Equals(imageAlign, wm.imageAlign) &&
					Comparer.Equals(textDirection, wm.textDirection) &&
					Comparer.Equals(font, wm.font) &&
					Native.ImageComparer.Equals(image, wm.image);
			}
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		internal void CopyFromInternal(PageWatermark watermark) {
			if (watermark == null)
				return;
#if !SL
			DisposeGdiResources();
#endif
			text = watermark.text;
			showBehind = watermark.showBehind;
			transparency = watermark.transparency;
			imageTransparency = watermark.imageTransparency;
			imageTiling = watermark.imageTiling;
			foreColor = watermark.foreColor;
			imageViewMode = watermark.imageViewMode;
			textDirection = watermark.textDirection;
			imageAlign = watermark.imageAlign;
			font = (Font)watermark.font.Clone();
			image = CloneImage(watermark.image);
		}
#if !SL
		protected internal virtual void Draw(IGraphics gr, RectangleF rect, int pageIndex, int pageCount) {
			try {
				DrawPictureWatermark(gr, rect);
			} catch(OutOfMemoryException) {
			} catch(ArgumentException) {
			}
			DrawTextWatermark(gr, rect);
		}
		protected void DrawPictureWatermark(IGraphics gr, RectangleF rect) {
			if (ActualImage == null)
				return;
			RectangleF imageRect = new RectangleF(Point.Round(rect.Location), PSNativeMethods.GetResolutionImageSize(ActualImage, GraphicsDpi.Document));
			switch (imageViewMode) {
				case ImageViewMode.Clip:
					RectangleF oldClip = gr.ClipBounds;
					RectangleF clip = rect;
					clip.Intersect(oldClip);
					gr.ClipBounds = clip;
					try {
						if (!imageTiling) {
							gr.DrawImage(ActualImage, RectHelper.AlignRectangleF(imageRect, rect, imageAlign));
						}
						else {
							WatermarkHelper.DrawTileImage(gr, ActualImage, 1.0f, rect);
						}
					} finally {
						gr.ClipBounds = oldClip;
					}
					break;
				case ImageViewMode.Stretch:
					gr.DrawImage(ActualImage, rect);
					break;
				case ImageViewMode.Zoom:
					if (imageTiling) {
						float scale = WatermarkHelper.GetAdjustedScale(rect.Size, imageRect.Size);
						WatermarkHelper.DrawTileImage(gr, ActualImage, scale, rect);
					}
					else {
						Size size = WatermarkHelper.GetAdjustedSize(rect.Size, imageRect.Size);
						ContentAlignment alignment = WatermarkHelper.GetAdjustedAlignment(rect.Size, imageRect.Size, imageAlign);
						gr.DrawImage(ActualImage, Native.RectHelper.AlignRectangleF(new Rectangle(Point.Empty, size), rect, alignment));
					}
					break;
			}
		}
		protected void DrawTextWatermark(IGraphics gr, RectangleF rect) {
			if(string.IsNullOrEmpty(text))
				return;
			RectangleF oldClip = gr.ClipBounds;
			int alpha = 255 - Math.Max(0, Math.Min(255, transparency));
			using(Brush brush = new SolidBrush(Color.FromArgb(alpha, foreColor))) {
				RectangleF clip = rect;
				clip.Intersect(oldClip);
				gr.ClipBounds = clip;
				try {
					WatermarkHelper.DrawWatermark(textDirection, gr, text, font, brush, Rectangle.Round(rect), sf);
				} catch {
				} finally {
					gr.ClipBounds = oldClip;
				}
			}
		}
#endif
	}
	[
	TypeConverter(typeof(Design.WatermarkConverter))
	]
	public class Watermark : PageWatermark {
		private string pageRange = "";
		[
		XtraSerializableProperty, 
#if !SL
	DevExpressPrintingCoreLocalizedDescription("WatermarkPageRange"),
#endif
		DefaultValue(""),
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Drawing.Watermark.PageRange"),
		]
		public string PageRange {
			get { return pageRange; }
			set { pageRange = PageRangeParser.ValidateString(value); }
		}
		public Watermark()
			: base() {
		}
		public virtual void CopyFrom(Watermark watermark) {
			Guard.ArgumentNotNull(watermark, "watermark");
			CopyFromInternal(watermark);
			pageRange = watermark.pageRange;
		}
#if !SL
#if !DEBUGTEST
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#endif
		public bool NeedDraw(int pageIndex, int pageCount) {
			int[] items = PageRangeParser.GetIndices(pageRange, pageCount);
			return Array.IndexOf(items, pageIndex) >= 0;
		}
		protected internal override void Draw(IGraphics gr, RectangleF rect, int pageIndex, int pageCount) {
			if(NeedDraw(pageIndex, pageCount)) {
				base.Draw(gr, rect, pageIndex, pageCount);
			}
		}
#endif
		public override bool Equals(object obj) {
			Watermark wm = obj as Watermark;
			return wm != null && Comparer.Equals(pageRange, wm.pageRange) && base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected byte[] GetImageAsArray() {
			if(Image == null)
				return null;
			MemoryStream stream = new MemoryStream();
			Image.Save(stream, ImageFormat.Png);
			return stream.ToArray();
		}
		#region Save Restore        
#if !SL
		public void SaveToXml(string xmlFile) {
			SaveCore(new XmlXtraSerializer(), xmlFile);
		}
		public void RestoreFromXml(string xmlFile) {
			RestoreCore(new XmlXtraSerializer(), xmlFile);
		}
		public void SaveToRegistry(string path) {
			SaveCore(new RegistryXtraSerializer(), path);
		}
		public void RestoreFromRegistry(string path) {
			RestoreCore(new RegistryXtraSerializer(), path);
		}		
#endif
		void SaveCore(XtraSerializer serializer, object path) {
			serializer.SerializeObject(this, path, "XtraPrintingWatermark");
		}
		void RestoreCore(XtraSerializer serializer, object path) {
			serializer.DeserializeObject(this, path, "XtraPrintingWatermark");
		}		
		public void SaveToStream(System.IO.Stream stream) {
			SaveCore(new XmlXtraSerializer(), stream);
		}
		public void RestoreFromStream(System.IO.Stream stream) {
			RestoreCore(new XmlXtraSerializer(), stream);
		}
		#endregion
	}
#if !SL
	internal class WatermarkHelper {
		public static float GetAdjustedScale(SizeF baseSize, SizeF size) {
			return Math.Min(baseSize.Width / size.Width, baseSize.Height / size.Height);
		}
		public static ContentAlignment GetAdjustedAlignment(SizeF baseSize, SizeF size, ContentAlignment originalAlignment) {
			if(FitToWidth(baseSize, size)) {
				switch(originalAlignment) {
					case ContentAlignment.TopLeft:
					case ContentAlignment.TopCenter:
					case ContentAlignment.TopRight:
						return ContentAlignment.TopCenter;
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.MiddleRight:
						return ContentAlignment.MiddleCenter;
					case ContentAlignment.BottomLeft:
					case ContentAlignment.BottomCenter:
					case ContentAlignment.BottomRight:
						return ContentAlignment.BottomCenter;
				}
			} else {
				switch(originalAlignment) {
					case ContentAlignment.TopLeft:
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.BottomLeft:
						return ContentAlignment.MiddleLeft;
					case ContentAlignment.TopCenter:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.BottomCenter:
						return ContentAlignment.MiddleCenter;
					case ContentAlignment.TopRight:
					case ContentAlignment.MiddleRight:
					case ContentAlignment.BottomRight:
						return ContentAlignment.MiddleRight;
				}
			}
			return ContentAlignment.MiddleCenter;
		}
		static bool FitToWidth(SizeF baseSize, SizeF size) {
			return baseSize.Width / size.Width < baseSize.Height / size.Height;			
		}
		public static Size GetAdjustedSize(SizeF baseSize, SizeF size) {
			float mult = GetAdjustedScale(baseSize, size);
			return Size.Round(new SizeF(mult * size.Width, mult * size.Height));
		}
		public static void DrawWatermark(DirectionMode textDirection, IGraphics gr, string text, Font font, Brush brush, Rectangle rect, StringFormat sf) {
			switch (textDirection) {
				case DirectionMode.Horizontal:
					RotatedTextPainter.DrawRotatedString(gr, text, font, brush, Rectangle.Round(rect),
						sf, 0, false, -1, TextAlignment.MiddleCenter);
					break;
				case DirectionMode.ForwardDiagonal:
					RotatedTextPainter.DrawRotatedString(gr, text, font, brush, Rectangle.Round(rect),
						sf, 50, false, -1, TextAlignment.MiddleCenter);
					break;
				case DirectionMode.BackwardDiagonal:
					RotatedTextPainter.DrawRotatedString(gr, text, font, brush, Rectangle.Round(rect),
						sf, -50, false, -1, TextAlignment.MiddleCenter);
					break;
				case DirectionMode.Vertical:
					RotatedTextPainter.DrawRotatedString(gr, text, font, brush, Rectangle.Round(rect),
						sf, 90, false, -1, TextAlignment.MiddleCenter);
					break;
			}
		}
		public static void DrawTileImage(IGraphics gr, Image image, float scale, RectangleF dstRect) {
			RectangleF oldClip = gr.ClipBounds;
			RectangleF clip = dstRect;
			clip.Intersect(oldClip);
			gr.ClipBounds = clip;
			gr.SaveTransformState();
			float scaleX = scale * GraphicsDpi.Document / image.HorizontalResolution;
			float scaleY = scale * GraphicsDpi.Document / image.VerticalResolution;
			Size size = image.Size;
			if (gr is Export.Pdf.PdfGraphics) {
				size.Width = Convert.ToInt32(size.Width * scaleX);
				size.Height = Convert.ToInt32(size.Height * scaleY);
			}
			else {
				gr.ScaleTransform(scaleX, scaleY);
				dstRect.X /= scaleX;
				dstRect.Y /= scaleY;
				dstRect.Width /= scaleX;
				dstRect.Height /= scaleY;
			}
			try {
				int x = (int)dstRect.Left;
				while (x < dstRect.Right) {
					int y = (int)dstRect.Top;
					while (y < dstRect.Bottom) {
						gr.DrawImage(image, new RectangleF(x, y, size.Width, size.Height));
						y += size.Height - 1;
					}
					x += size.Width - 1;
				}
			} finally {
				gr.ResetTransform();
				gr.ApplyTransformState(System.Drawing.Drawing2D.MatrixOrder.Append, true);
				gr.ClipBounds = oldClip;
			}
		}
	}
#endif
}
namespace DevExpress.XtraPrinting.Design {
	using System.ComponentModel.Design;
	using System.Collections;
	using System.Globalization;
	using DevExpress.XtraPrinting.Drawing;
	using DevExpress.XtraPrinting.Native;
#if SL
	using DevExpress.Xpf.ComponentModel;
#endif
	public class WatermarkConverter : ExpandableObjectConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				Watermark watermark = value as Watermark;
				if (watermark != null) {
					if (!String.IsNullOrEmpty(watermark.Text))
						return PreviewLocalizer.GetString(PreviewStringId.WatermarkTypeText);
					if (watermark.Image != null)
						return PreviewLocalizer.GetString(PreviewStringId.WatermarkTypePicture);
				}
				return PreviewLocalizer.GetString(PreviewStringId.NoneString);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class WMTextConverter : StringConverter {
		public static string[] StandardValues {
			get {
				return new String[] {	
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Asap),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Confidential),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Copy),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_DoNotCopy),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Draft),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Evaluation),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Original),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Personal),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Sample),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_TopSecret),
										PreviewLocalizer.GetString(PreviewStringId.WMForm_Watermark_Urgent) };
			}
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(StandardValues);
		}
	}
}
