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
using System.IO;
using System.Security;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
using DevExpress.Data.Helpers;
namespace DevExpress.Pdf {
	public class PdfGraphics : PdfDisposableObject {
		public const float DefaultDpi = 96f;
		const float defaultPageDpi = 72f;
		internal static void CheckDpiValue(float value, string parameterName) {
			if (value <= 0)
				throw new ArgumentOutOfRangeException(parameterName, PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectDpi));
		}
		internal static void CheckDpiValues(float dpiX, float dpiY) {
			CheckDpiValue(dpiX, "dpiX");
			CheckDpiValue(dpiY, "dpiY");
		}
		readonly IList<PdfGraphicsCommand> exportCommands = new List<PdfGraphicsCommand>();
		readonly PdfImageCache imageCache;
		readonly PdfEditableFontDataCache fontCache;
		ImageCodecInfo jpegCodec;
		bool useKerning = true;
		bool convertImagesToJpeg = false;
		PdfGraphicsTextOrigin textOrigin = PdfGraphicsTextOrigin.TopLeftCorner;
		PdfGraphicsJpegImageQuality jpegImageQuality = PdfGraphicsJpegImageQuality.Highest;
		ImageCodecInfo JpegCodec {
			get {
				if (jpegCodec == null) {
					ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
					foreach (ImageCodecInfo codec in codecs) {
						if (codec.FormatID == ImageFormat.Jpeg.Guid) {
							jpegCodec = codec;
							break;
						}
					}
				}
				return jpegCodec;
			}
		}
		public bool UseKerning {
			get { return useKerning; }
			set { useKerning = value; }
		}
		public bool ConvertImagesToJpeg {
			get { return convertImagesToJpeg; }
			set { convertImagesToJpeg = value; }
		}
		public PdfGraphicsTextOrigin TextOrigin {
			get { return textOrigin; }
			set { textOrigin = value; }
		}
		public PdfGraphicsJpegImageQuality JpegImageQuality {
			get { return jpegImageQuality; }
			set { jpegImageQuality = value; }
		}
		[Obsolete("The PdfGraphics constructor is now obsolete. Use the PdfDocumentProcessor.CreateGraphics method instead.")]
		public PdfGraphics() {
		}
		internal PdfGraphics(PdfImageCache imageCache, PdfEditableFontDataCache fontCache) {
			this.imageCache = imageCache;
			this.fontCache = fontCache;
		}
		public void RotateTransform(float degree) {
			exportCommands.Add(new PdfGraphicsRotateTransformCommand(degree));
		}
		public void TranslateTransform(float x, float y) {
			exportCommands.Add(new PdfGraphicsTranslateTransformCommand(x, y));
		}
		public void IntersectClip(RectangleF rect) {
			exportCommands.Add(new PdfGraphicsIntersectClipCommand(rect));
		}
		public void SaveGraphicsState() {
			exportCommands.Add(new PdfGraphicsSaveGraphicsStateCommand());
		}
		public void RestoreGraphicsState() {
			exportCommands.Add(new PdfGraphicsRestoreGraphicsStateCommand());
		}
		public void DrawRectangle(Pen pen, RectangleF bounds) {
			AddIsolatedCommand(new PdfGraphicsDrawRectangleCommand(ConvertGdiPen(pen), bounds));
		}
		public void FillRectangle(Brush brush, RectangleF bounds) {
			AddIsolatedCommand(new PdfGraphicsFillRectangleCommand(ConvertGdiBrush(brush), bounds));
		}
		public SizeF MeasureString(string text, Font font) {
			return MeasureString(text, font, PdfStringFormat.GenericDefault, DefaultDpi, DefaultDpi);
		}
		public SizeF MeasureString(string text, Font font, PdfStringFormat format) {
			return MeasureString(text, font, format, DefaultDpi, DefaultDpi);
		}
		public SizeF MeasureString(string text, Font font, PdfStringFormat format, float dpiX, float dpiY) {
			return MeasureString(font, format, dpiX, dpiY, formatter => formatter.FormatString(text, new PdfPoint(), format, useKerning));
		}
		public SizeF MeasureString(string text, Font font, SizeF layoutSize) {
			return MeasureString(text, font, layoutSize, PdfStringFormat.GenericDefault, DefaultDpi, DefaultDpi);
		}
		public SizeF MeasureString(string text, Font font, SizeF layoutSize, PdfStringFormat format) {
			return MeasureString(text, font, layoutSize, format, DefaultDpi, DefaultDpi);
		}
		public SizeF MeasureString(string text, Font font, SizeF layoutSize, PdfStringFormat format, float dpiX, float dpiY) {
			PdfRectangle layout = new PdfRectangle(0, 0, layoutSize.Width / dpiX * defaultPageDpi, layoutSize.Height / dpiY * defaultPageDpi);
			return MeasureString(font, format, dpiX, dpiY, formatter => formatter.FormatString(text, layout, format, useKerning));
		}
		public void DrawString(string text, Font font, SolidBrush brush, PointF point) {
			DrawString(text, font, brush, point.X, point.Y, PdfStringFormat.GenericDefault);
		}
		public void DrawString(string text, Font font, SolidBrush brush, PointF point, PdfStringFormat format) {
			DrawString(text, font, brush, point.X, point.Y, format);
		}
		public void DrawString(string text, Font font, SolidBrush brush, float x, float y) {
			DrawString(text, font, brush, x, y, PdfStringFormat.GenericDefault);
		}
		public void DrawString(string text, Font font, SolidBrush brush, float x, float y, PdfStringFormat format) {
			if (font == null)
				throw new ArgumentNullException("font");
			format = format ?? PdfStringFormat.GenericDefault;
			if (!String.IsNullOrEmpty(text)) {
				PointF location = new PointF(x, y);
				bool kerning = useKerning && SecurityHelper.IsUnmanagedCodeGrantedAndHasZeroHwnd;
				AddIsolatedCommand(new PdfGraphicsDrawStringWithLocationCommand(new PdfSolidBrushContainer(brush), font, text, location, format, textOrigin, kerning));
			}
		}
		public void DrawString(string text, Font font, SolidBrush brush, RectangleF layout, PdfStringFormat format) {
			if (font == null)
				throw new ArgumentNullException("font");
			format = format ?? PdfStringFormat.GenericDefault;
			if (!String.IsNullOrEmpty(text)) {
				bool kerning = useKerning && SecurityHelper.IsUnmanagedCodeGrantedAndHasZeroHwnd;
				AddIsolatedCommand(new PdfGraphicsDrawFormattedStringCommand(new PdfSolidBrushContainer(brush), font, layout, text, format, kerning));
			}
		}
		public void DrawString(string text, Font font, SolidBrush brush, RectangleF layout) {
			DrawString(text, font, brush, layout, PdfStringFormat.GenericDefault);
		}
		public void DrawImage(byte[] data, PointF location) {
			PdfImageInfo info = DetectImage(data);
			DrawImage(data, new RectangleF(location.X, location.Y, info.Width, info.Height), info);
		}
		public void DrawImage(Stream data, PointF location) {
			PdfImageInfo info = DetectImage(data);
			DrawImage(data, new RectangleF(location.X, location.Y, info.Width, info.Height), info);
		}
		public void DrawImage(byte[] data, RectangleF bounds) {
			DrawImage(data, bounds, DetectImage(data));
		}
		public void DrawImage(Stream data, RectangleF bounds) {
			DrawImage(data, bounds, DetectImage(data));
		}
		public void DrawImage(Image image, PointF location) {
			DrawImage(image, new RectangleF(location, image.Size));
		}
		public void DrawImage(Image image, RectangleF bounds) {
			DrawImage(image, bounds, true);
		}
		public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit) {
			if (image == null)
				throw new ArgumentNullException("image");
			float zoomX = destRect.Width / srcRect.Width;
			float zoomY = destRect.Height / srcRect.Height;
			float x = srcRect.X;
			float y = srcRect.Y;
			float k = 1;
			switch (srcUnit) {
				case GraphicsUnit.Point:
					k = 72;
					goto case GraphicsUnit.Inch;
				case GraphicsUnit.Document:
					k = 300;
					goto case GraphicsUnit.Inch;
				case GraphicsUnit.Millimeter:
					k = 25.4F;
					goto case GraphicsUnit.Inch;
				case GraphicsUnit.Inch:
					float kx = image.HorizontalResolution / k;
					float ky = image.VerticalResolution / k;
					x *= kx;
					y *= ky;
					zoomX /= kx;
					zoomY /= ky;
					break;
				case GraphicsUnit.Pixel:
					break;
				case GraphicsUnit.Display:
				case GraphicsUnit.World:
					throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnsupportedGraphicsUnit), "srcUnit");
			}
			RectangleF sourceRectangle = new RectangleF(destRect.X - zoomX * x, destRect.Y - zoomY * y, zoomX * image.Width, zoomY * image.Height);
			SaveGraphicsState();
			IntersectClip(destRect);
			DrawImage(image, sourceRectangle, false);
			RestoreGraphicsState();
		}
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			AddIsolatedCommand(new PdfGraphicsDrawLineCommand(ConvertGdiPen(pen), x1, y1, x2, y2));
		}
		public void DrawLines(Pen pen, PointF[] points) {
			if (points == null)
				throw new ArgumentNullException("points");
			AddIsolatedCommand(new PdfGraphicsDrawLinesCommand(ConvertGdiPen(pen), points));
		}
		public void DrawPath(Pen pen, GraphicsPath path) {
			if (path == null)
				throw new ArgumentNullException("path");
			AddIsolatedCommand(new PdfGraphicsDrawPathCommand(ConvertGdiPen(pen), path));
		}
		public void FillPath(Brush brush, GraphicsPath path) {
			if (path == null)
				throw new ArgumentNullException("path");
			AddIsolatedCommand(new PdfGraphicsFillPathCommand(ConvertGdiBrush(brush), path));
		}
		public void DrawPolygon(Pen pen, PointF[] points) {
			if (points == null)
				throw new ArgumentNullException("points");
			AddIsolatedCommand(new PdfGraphicsDrawPolygonCommand(ConvertGdiPen(pen), points));
		}
		public void FillPolygon(Brush brush, PointF[] points) {
			if (points == null)
				throw new ArgumentNullException("points");
			AddIsolatedCommand(new PdfGraphicsFillPolygonCommand(ConvertGdiBrush(brush), points));
		}
		public void DrawEllipse(Pen pen, RectangleF rect) {
			AddIsolatedCommand(new PdfGraphicsDrawEllipseCommand(ConvertGdiPen(pen), rect));
		}
		public void FillEllipse(Brush brush, RectangleF rect) {
			AddIsolatedCommand(new PdfGraphicsFillEllipseCommand(ConvertGdiBrush(brush), rect));
		}
		public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4) {
			AddIsolatedCommand(new PdfGraphicsDrawBezierCommand(ConvertGdiPen(pen), pt1, pt2, pt3, pt4));
		}
		public void DrawBeziers(Pen pen, PointF[] points) {
			if (points == null)
				throw new ArgumentNullException("points");
			AddIsolatedCommand(new PdfGraphicsDrawBeziersCommand(ConvertGdiPen(pen), points));
		}
		public void AddLinkToUri(RectangleF linkArea, Uri uri) {
			if (uri == null)
				throw new ArgumentNullException("uri");
			exportCommands.Add(new PdfGraphicsAddLinkToUriCommand(linkArea, uri));
		}
		public void AddLinkToPage(RectangleF linkArea, int pageNumber) {
			exportCommands.Add(new PdfGraphicsAddLinkToPageCommand(linkArea, pageNumber, null, null, null));
		}
		public void AddLinkToPage(RectangleF linkArea, int pageNumber, float zoom) {
			exportCommands.Add(new PdfGraphicsAddLinkToPageCommand(linkArea, pageNumber, null, null, zoom));
		}
		public void AddLinkToPage(RectangleF linkArea, int pageNumber, float destinationX, float destinationY) {
			exportCommands.Add(new PdfGraphicsAddLinkToPageCommand(linkArea, pageNumber, destinationX, destinationY, null));
		}
		public void AddLinkToPage(RectangleF linkArea, int pageNumber, float destinationX, float destinationY, float zoom) {
			exportCommands.Add(new PdfGraphicsAddLinkToPageCommand(linkArea, pageNumber, destinationX, destinationY, zoom));
		}
		public void AddToPageBackground(PdfPage page) {
			AddToPageBackground(page, DefaultDpi, DefaultDpi);
		}
		public void AddToPageBackground(PdfPage page, float dpiX, float dpiY) {
			CheckDpiValues(dpiX, dpiY);
			AddToPage(page, (fontCache, imageCache) => new PdfGraphicsBeforePageContentsCommandConstructor(page, exportCommands, dpiX / defaultPageDpi, dpiY / defaultPageDpi, fontCache, imageCache));
		}
		public void AddToPageForeground(PdfPage page) {
			AddToPageForeground(page, DefaultDpi, DefaultDpi);
		}
		public void AddToPageForeground(PdfPage page, float dpiX, float dpiY) {
			CheckDpiValues(dpiX, dpiY);
			AddToPage(page, (fontCache, imageCache) => new PdfGraphicsAfterPageContentsCommandConstructor(page, exportCommands, dpiX / defaultPageDpi, dpiY / defaultPageDpi, fontCache, imageCache));
		}
		internal string AddLinkToPage(RectangleF linkArea) {
			string destinationName = null;
			if (imageCache != null){
				PdfDocumentCatalog documentCatalog = imageCache.DocumentCatalog ;
				if (documentCatalog != null) {
					destinationName = documentCatalog.Names.ReserveDestinationName();
					exportCommands.Add(new PdfGraphicsAddLinkToPageCommand(linkArea, destinationName));
				}
			}
			return destinationName;
		}
		internal void AddToPage(PdfPage page, float dpiX, float dpiY) {
			if (page == null)
				throw new ArgumentNullException("page");
			ValidateCaches(page.DocumentCatalog.Objects.Id);
			CheckDpiValues(dpiX, dpiY);
			new PdfGraphicsAfterPageContentsCommandConstructor(page, exportCommands, dpiX / defaultPageDpi, dpiY / defaultPageDpi, fontCache, imageCache).Execute();
		}
		internal void DrawString(string text, string fontFamily, FontStyle fontStyle, float fontSize, SolidBrush brush, float x, float y, ushort[] glyphIndices, float[] glyphPositions, int[] order) {
			if (!String.IsNullOrEmpty(text))
				AddIsolatedCommand(new PdfGraphicsDrawStringWithGlyphPositioningCommands(new PdfSolidBrushContainer(brush), fontFamily, fontStyle, fontSize, new PointF(x, y), textOrigin, text, glyphIndices, glyphPositions, order));
		}
		void AddToPage(PdfPage page, Func<PdfEditableFontDataCache, PdfImageCache, PdfGraphicsPageContentsCommandConstructor> createConstructor) {
			if (page == null)
				throw new ArgumentNullException("page");
			PdfObjectCollection objects = page.DocumentCatalog.Objects;
			ValidateCaches(objects.Id);
			PdfEditableFontDataCache fontCache = this.fontCache ?? new PdfEditableFontDataCache(objects);
			PdfImageCache imageCache = this.imageCache ?? new PdfImageCache(objects);
			createConstructor(fontCache, imageCache).Execute();
			fontCache.UpdateFonts();
			if (this.fontCache == null)
				fontCache.Dispose();
		}
		void DrawImage(Image image, RectangleF bounds, bool isIsolatedCommand) {
			if (image == null)
				throw new ArgumentNullException("image");
			Metafile metafile = image as Metafile;
			if (metafile == null)
				DrawImage(image, () => PdfImageCache.CreatePdfImage(image, convertImagesToJpeg, JpegCodec, (long)jpegImageQuality), bounds, true);
			else {
				if (isIsolatedCommand)
					AddIsolatedCommand(new PdfGraphicsDrawMetafileWithBoundsCommand(metafile, bounds));
				else
					exportCommands.Add(new PdfGraphicsDrawMetafileWithBoundsCommand(metafile, bounds));
			}
		}
		void ValidateCaches(Guid id) {
			if ((imageCache != null && !imageCache.CheckCollectionId(id))
				|| (fontCache != null && fontCache.ObjectsId != id))
				throw new NotSupportedException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnsupportedGraphicsOperation));
		}
		void AddIsolatedCommand(PdfGraphicsCommand command) {
			SaveGraphicsState();
			exportCommands.Add(command);
			RestoreGraphicsState();
		}
		PdfImageInfo DetectImage(byte[] data) {
			using (Stream stream = new MemoryStream(data))
				return DetectImage(stream);
		}
		void DrawImage(byte[] data, RectangleF bounds, PdfImageInfo info) {
			if (info.Type == PdfImageType.Metafile)
				DrawMetafile(new MemoryStream(data), bounds);
			else {
				Func<PdfImage> createImage = () => PdfImageCache.CreatePdfImage(data, convertImagesToJpeg, JpegCodec, (long)jpegImageQuality, info.JpegSize);
				DrawImage(data, createImage, bounds, true);
			}
		}
		void DrawImage(Stream data, RectangleF bounds, PdfImageInfo info) {
			if (info.Type == PdfImageType.Metafile)
				DrawMetafile(data, bounds);
			else {
				Func<PdfImage> createImage = () => PdfImageCache.CreatePdfImage(data, convertImagesToJpeg, JpegCodec, (long)jpegImageQuality, info.JpegSize);
				DrawImage(data, createImage, bounds, true);
			}
		}
		void DrawMetafile(Stream stream, RectangleF bounds) {
			using (Image image = Image.FromStream(stream)) {
				Metafile metafile = image as Metafile;
				if (metafile != null)
					AddIsolatedCommand(new PdfGraphicsDrawMetafileWithBoundsCommand(metafile, bounds));
			}
		}
		void DrawImage<T>(T source, Func<PdfImage> create, RectangleF bounds, bool isIsolatedCommand) where T : class {
			PdfGraphicsDrawImageCommand command;
			if (imageCache == null)
				command = new PdfGraphicsDrawImageObjectCommand(create(), bounds);
			else
				command = new PdfGraphicsDrawImageReferenceCommand(imageCache.GetPdfImageObjectNumber(source, create), bounds);
			if (isIsolatedCommand)
				AddIsolatedCommand(command);
			else
				exportCommands.Add(command);
		}
		PdfPen ConvertGdiPen(Pen pen) {
			PdfBrushContainer brushContainer = ConvertGdiBrush(pen.Brush);
			PdfPen pdfPen = new PdfPen(brushContainer, pen.Width);
			pdfPen.Alignment = pen.Alignment;
			pdfPen.StartCap = pen.StartCap;
			pdfPen.EndCap = pen.EndCap;
			pdfPen.MiterLimit = pen.MiterLimit;
			pdfPen.DashCap = pen.DashCap;
			pdfPen.DashStyle = pen.DashStyle;
			pdfPen.DashOffset = pen.DashOffset;
			if (pen.DashStyle != DashStyle.Solid)
				pdfPen.DashPattern = pen.DashPattern;
			pdfPen.LineJoin = pen.LineJoin;
			using (Matrix transform = pen.Transform) {
				float[] elements = transform.Elements;
				pdfPen.Transform = new PdfTransformationMatrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
			}
			return pdfPen;
		}
		PdfBrushContainer ConvertGdiBrush(Brush brush) {
			if (brush == null)
				throw new ArgumentNullException("brush");
			PdfBrushContainer result = PdfBrushContainer.CreateFromGdiBrush(brush);
			if (result == null)
				throw new ArgumentException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnsupportedBrushType), "brush");
			return result;
		}
		PdfImageInfo DetectImage(Stream data) {
			data.Position = 0;
			try {
				JBIG2StreamHelper stream = new JBIG2StreamHelper(data);
				switch (stream.ReadByte()) {
					case 0x01:
						if (stream.ReadByte() == 0x00) {
							switch (stream.ReadByte()) {
								case 0x09:
								case 0x00:
									if (stream.ReadByte() == 0x00)
										return new PdfImageInfo(PdfImageType.Metafile);
									break;
							}
						}
						break;
					case 0x02:
						if (stream.ReadByte() == 0x00 && stream.ReadByte() == 0x09 && stream.ReadByte() == 0x00)
							return new PdfImageInfo(PdfImageType.Metafile);
						break;
					case 0xD7 :
						if (stream.ReadByte() == 0xCD && stream.ReadByte() == 0xC6 && stream.ReadByte() == 0x9A)
							return new PdfImageInfo(PdfImageType.Metafile);
						break;
					case 0xFF:
						if (stream.ReadByte() == 0xD8) {
							while (!stream.Finish) {
								int head = stream.ReadInt16();
								int length = stream.ReadInt16();
								if (head == 0xFFC0) {
									stream.ReadByte();
									int height = stream.ReadInt16();
									int width = stream.ReadInt16();
									return new PdfImageInfo(PdfImageType.Jpeg, width, height);
								}
								if (length > 2)
									stream.ReadBytes(length - 2);
							}
						}
						break;
				}
				return new PdfImageInfo(PdfImageType.Bitmap);
			}
			finally {
				data.Position = 0;
			}
		}
		SizeF MeasureString(Font font, PdfStringFormat format, float dpiX, float dpiY, Func<PdfStringFormatter, IList<PdfStringGlyphRun>> formatterAction) {
			const float measurementPrecisionFactor = 1.00005f;
			if (font == null)
				throw new ArgumentNullException("font");
			CheckDpiValues(dpiX, dpiY);
			using (PdfEditableFontData fontData = PdfEditableFontData.Create(font.Style, font.FontFamily.Name, false)) {
				double fontSize = font.SizeInPoints;
				PdfFontMetrics metrics = fontData.Metrics;
				double lineSpacing = metrics.GetLineSpacing(fontSize);
				PdfStringFormatter formatter = new PdfStringFormatter(fontData, fontSize);
				IList<PdfStringGlyphRun> lines = formatterAction(formatter);
				if (lines.Count > 0) {
					double width = 0;
					double height = metrics.GetAscent(fontSize) + (lines.Count - 1) * lineSpacing + metrics.GetDescent(fontSize);
					double emFactor = fontSize / 1000;
					foreach (PdfStringGlyphRun line in lines)
						width = Math.Max(width, line.Width * emFactor);
					width += (format.LeadingMarginFactor + format.TrailingMarginFactor) * lineSpacing;
					height += format.LeadingMarginFactor * lineSpacing;
					return new SizeF((float)(width / defaultPageDpi * dpiX * measurementPrecisionFactor), (float)(height / defaultPageDpi * dpiY * measurementPrecisionFactor));
				}
				return SizeF.Empty;
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				foreach (PdfGraphicsCommand command in exportCommands)
					command.Dispose();
			}
		}
	}
}
