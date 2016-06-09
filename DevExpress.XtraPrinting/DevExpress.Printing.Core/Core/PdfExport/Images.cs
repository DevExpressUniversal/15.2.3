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
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using DevExpress.Utils;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfImageBase : PdfXObject {
		static bool Is8bppIndexedFormat(Image image) {
			return
				image.PixelFormat == PixelFormat.Format8bppIndexed ||
				(image.PixelFormat == PixelFormat.Indexed && Image.GetPixelFormatSize(image.PixelFormat) == 8);
		}
		static bool Is4bppIndexedFormat(Image image) {
			return
				image.PixelFormat == PixelFormat.Format4bppIndexed ||
				(image.PixelFormat == PixelFormat.Indexed && Image.GetPixelFormatSize(image.PixelFormat) == 4);
		}
		static bool Is1bppIndexedFormat(Image image) {
			return
				image.PixelFormat == PixelFormat.Format1bppIndexed ||
				(image.PixelFormat == PixelFormat.Indexed && Image.GetPixelFormatSize(image.PixelFormat) == 1);
		}
		static bool Is24bppRGBFormat(Image image) {
			return image.PixelFormat == PixelFormat.Format24bppRgb;
		}
		static bool Is32bppARGBFormat(Image image) {
			return image.PixelFormat == PixelFormat.Format32bppArgb;
		}
		static bool IsPixelFormat32bppCMYK(Image image) {
			return image.PixelFormat == (PixelFormat)8207;
		}
		static Bitmap ConvertImageTo(Image source, PixelFormat destFormat) {
			Bitmap result = new Bitmap(source.Width, source.Height, destFormat);
			result.SetResolution(source.HorizontalResolution, source.VerticalResolution);
			using(Graphics g = Graphics.FromImage(result)) {
				g.PageUnit = GraphicsUnit.Pixel;
				g.DrawImageUnscaled(source, 0, 0);
			}
			return result;
		}
		public static PdfImageBase CreateInstance(IPdfDocumentOwner documentInfo,Image image, PdfDocument document, string name, bool compressed, bool convertImageToJpeg, PdfJpegImageQuality jpegQuality) {
			if(Is4bppIndexedFormat(image))
				return new Pdf4bppIndexedImage(image, name, compressed);
			if(Is1bppIndexedFormat(image))
				return new Pdf1bppIndexedImage(image, name, compressed);
			if(IsPixelFormat32bppCMYK(image))
				image = ConvertImageTo(image, PixelFormat.Format24bppRgb);
			if(convertImageToJpeg)
				return new PdfJpegImage(image, name, jpegQuality, compressed);
			if(Is8bppIndexedFormat(image))
				return new Pdf8bppIndexedImage(image, name, compressed);
			if(Is24bppRGBFormat(image))
				return new Pdf24bppRgbImage(image, name, compressed);
			if(image.RawFormat.Equals(ImageFormat.Jpeg))
				return new PdfJpegImage(image, name, jpegQuality, compressed);
			if(Is32bppARGBFormat(image))
				return new Pdf32bppArgbImage(image, name, compressed);
			if(image is Metafile && !PdfGraphics.RenderMetafileAsBitmap)
				return new PdfVectorImage(documentInfo, image, document, name, compressed);
			using(Image bitmap = BitmapCreator.CreateBitmapWithResolutionLimit(image, DXColor.White)) {
				return new Pdf32bppArgbImage(bitmap, name, compressed);
			}
		}
		public virtual PdfImageBase MaskImage { get { return null; } }
		public PdfImageBase(string name, bool compressed)
			: base(name, compressed) {
		}
		public virtual Matrix Transform(RectangleF correctedBounds) {
			return new Matrix(correctedBounds.Width, 0, 0, correctedBounds.Height, correctedBounds.X, correctedBounds.Y);
		}
	}
	public class PdfBitmap : PdfImageBase {
		Size size;
		protected Size Size { get { return size; } }
		public PdfBitmap(string name, Size size, bool compressed)
			: base(name, compressed) {
			this.size = size;
		}
		public override void FillUp() {
			base.FillUp();
			Attributes.Add("Subtype", "Image");
			Attributes.Add("Width", this.size.Width);
			Attributes.Add("Height", this.size.Height);
		}
	}
	public class PdfJpegImage : PdfBitmap {
		static ImageCodecInfo GetEncoderInfo(String mimeType) {
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
			for(int i = 0; i < encoders.Length; i++) {
				if(encoders[i].MimeType == mimeType)
					return encoders[i];
			}
			return null;
		}
		protected override bool UseFlateEncoding { get { return false; } }
		Pdf8bppGrayscaleAlpaImage maskImage;
		public override PdfImageBase MaskImage {
			get { return maskImage; }
		}
		public PdfJpegImage(Image image, string name, PdfJpegImageQuality quality, bool compressed)
			: base(name, image.Size, false) {
			if(image.RawFormat.Equals(ImageFormat.Jpeg) && quality == PdfJpegImageQuality.Highest && !IsCmykColorSpace(image) && !IsGrayColorSpace(image)) {
				SaveImage(image, Stream.Data, ImageFormat.Jpeg);
				return;
			}
			ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
			if(imageCodecInfo == null) {
				SaveImage(image, Stream.Data, ImageFormat.Jpeg);
				return;
			}
			if(image.PixelFormat == PixelFormat.Format32bppArgb) {
				Pdf8bppGrayscaleAlpaImage mask = new Pdf8bppGrayscaleAlpaImage(image, name + "Mask", compressed, false);
				if(mask.HasTransparentPixels)
					maskImage = mask;
			}
			using(EncoderParameters encoderParameters = new EncoderParameters(1)) {
				EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, (long)quality);
				encoderParameters.Param[0] = encoderParameter;
				SaveImage(image, Stream.Data, imageCodecInfo, encoderParameters);
			}
		}
		static void SaveImage(Image image, Stream stream, ImageCodecInfo encoder, EncoderParameters encoderParams) {
			long position = stream.Position;
			try {
				image.Save(stream, encoder, encoderParams);
			}
			catch(System.Runtime.InteropServices.ExternalException e) {
				Tracer.TraceInformation(DevExpress.XtraPrinting.Native.NativeSR.TraceSource, "image.Save");
				Tracer.TraceData(DevExpress.XtraPrinting.Native.NativeSR.TraceSource, System.Diagnostics.TraceEventType.Verbose, e);
				if(stream.CanSeek)
					stream.Position = position;
				using(Bitmap tempImage = new Bitmap(image))
					tempImage.Save(stream, encoder, encoderParams);
			}
		}
		static void SaveImage(Image image, Stream stream, ImageFormat format) {
			long position = stream.Position;
			try {
				image.Save(stream, format);
			}
			catch(System.Runtime.InteropServices.ExternalException e) {
				Tracer.TraceInformation(DevExpress.XtraPrinting.Native.NativeSR.TraceSource, "image.Save");
				Tracer.TraceData(DevExpress.XtraPrinting.Native.NativeSR.TraceSource, System.Diagnostics.TraceEventType.Verbose, e);
				if(stream.CanSeek)
					stream.Position = position;
				using(Bitmap tempImage = new Bitmap(image))
					tempImage.Save(stream, format);
			}
		}
		static bool IsCmykColorSpace(Image image) {
			return
				(image.Flags & (int)ImageFlags.ColorSpaceCmyk) != 0 ||
				(image.Flags & (int)ImageFlags.ColorSpaceYcck) != 0;
		}
		static bool IsGrayColorSpace(Image image) {
			return (image.Flags & (int)ImageFlags.ColorSpaceGray) != 0;
		}
		void AddImageAttributes() {
			Attributes.Add("BitsPerComponent", 8);
			Attributes.Add("ColorSpace", "DeviceRGB");
			PdfArray filter = new PdfArray();
			filter.Add("DCTDecode");
			Attributes.Add("Filter", filter);
			if(maskImage != null)
				Attributes.Add("SMask", maskImage.InnerObject);
		}
		public override void FillUp() {
			base.FillUp();
			AddImageAttributes();
		}
	}
	public abstract class PdfIndexedImage : PdfBitmap {
		static int CalculateByteWidth(int imageWidth, int bitsPerComponent) {
			int pointsInByte = 8 / bitsPerComponent;
			int byteWidth = imageWidth / pointsInByte;
			if(imageWidth % pointsInByte > 0)
				byteWidth++;
			return byteWidth;
		}
		PdfStream colorTable;
		int paletteLength;
		int maskColorIndex = -1;
		protected abstract int BitsPerComponent { get; }
		protected PdfIndexedImage(Image image, string name, bool compressed)
			: base(name, image.Size, compressed) {
			CreateColorTable(image);
			FillStream(image);
		}
		void CreateColorTable(Image image) {
			int bpp = Image.GetPixelFormatSize(image.PixelFormat);
			if(bpp > 8 || image.Palette.Entries.Length == 0)
				return;
			this.paletteLength = image.Palette.Entries.Length;
			string str = "";
			for(int i = 0; i < this.paletteLength; i++) {
				Color paletteColor = image.Palette.Entries[i];
				if(paletteColor.A == 0 && this.maskColorIndex == -1)
					this.maskColorIndex = i;
				str += paletteColor.R.ToString("X2", CultureInfo.CurrentCulture);
				str += paletteColor.G.ToString("X2", CultureInfo.CurrentCulture);
				str += paletteColor.B.ToString("X2", CultureInfo.CurrentCulture);
				if(i < this.paletteLength - 1)
					str += " ";
			}
			str += ">\n";
			colorTable = new PdfStream();
			colorTable.Attributes.Add("Filter", "ASCIIHexDecode");
			colorTable.SetString(str);
		}
		void FillStream(Image image) {
			byte[] tempBuffer = BitmapUtils.ImageToByteArray(image);
			int startIndex = tempBuffer.Length;
			int byteWidth = CalculateByteWidth(image.Width, BitsPerComponent);
			int correctedByteWidth = BitmapUtils.CalculateCorrectedByteWidth(byteWidth);
			for(int lineNumber = 0; lineNumber < image.Height; lineNumber++) {
				startIndex -= correctedByteWidth;
				Stream.SetBytes(tempBuffer, startIndex, byteWidth);
			}
		}
		protected override void WriteContent(StreamWriter writer) {
			if(this.colorTable != null)
				this.colorTable.WriteIndirect(writer);
		}
		protected override void RegisterContent(PdfXRef xRef) {
			if(this.colorTable != null)
				xRef.RegisterObject(this.colorTable);
		}
		public override void FillUp() {
			base.FillUp();
			Attributes.Add("BitsPerComponent", BitsPerComponent);
			PdfArray colorSpace = new PdfArray();
			colorSpace.Add("Indexed");
			colorSpace.Add("DeviceRGB");
			colorSpace.Add(this.paletteLength - 1);
			colorSpace.Add(colorTable);
			Attributes.Add("ColorSpace", colorSpace);
			if(this.maskColorIndex != -1) {
				PdfArray maskArray = new PdfArray();
				maskArray.Add(this.maskColorIndex);
				maskArray.Add(this.maskColorIndex);
				Attributes.Add("Mask", maskArray);
			}
		}
		public override void Close() {
			if(this.colorTable != null) {
				this.colorTable.Close();
				this.colorTable = null;
			}
			base.Close();
		}
	}
	public class Pdf8bppIndexedImage : PdfIndexedImage {
		protected override int BitsPerComponent { get { return 8; } }
		public Pdf8bppIndexedImage(Image image, string name, bool compressed)
			: base(image, name, compressed) {
		}
	}
	public class Pdf4bppIndexedImage : PdfIndexedImage {
		protected override int BitsPerComponent { get { return 4; } }
		public Pdf4bppIndexedImage(Image image, string name, bool compressed)
			: base(image, name, compressed) {
		}
	}
	public class Pdf1bppIndexedImage : PdfIndexedImage {
		protected override int BitsPerComponent { get { return 1; } }
		public Pdf1bppIndexedImage(Image image, string name, bool compressed)
			: base(image, name, compressed) {
		}
	}
	public abstract class PdfTrueColorImage : PdfBitmap {
		Color maskColor = Color.Empty;
		protected virtual bool UseMaskColor { get { return true; } }
		public PdfTrueColorImage(Image image, string name, bool compressed)
			: base(name, image.Size, compressed) {
			if(UseMaskColor)
				InitilizeMaskColor(image as Bitmap);
		}
		void InitilizeMaskColor(Bitmap bitmap) {
			if(bitmap == null)
				return;
			if(bitmap.Width == 0 || bitmap.Height == 0)
				return;
			Color leftTopPixel = bitmap.GetPixel(0, 0);
			if(leftTopPixel.A == 0) {
				this.maskColor = leftTopPixel;
				return;
			}
			Color rightTopPixel = bitmap.GetPixel(bitmap.Width - 1, 0);
			if(rightTopPixel.A == 0) {
				this.maskColor = rightTopPixel;
				return;
			}
			Color leftBottomPixel = bitmap.GetPixel(0, bitmap.Height - 1);
			if(leftBottomPixel.A == 0) {
				this.maskColor = leftBottomPixel;
				return;
			}
			Color rightBottomPixel = bitmap.GetPixel(bitmap.Width - 1, bitmap.Height - 1);
			if(rightBottomPixel.A == 0) {
				this.maskColor = rightBottomPixel;
				return;
			}
		}
		public override void FillUp() {
			base.FillUp();
			Attributes.Add("BitsPerComponent", 8);
			Attributes.Add("ColorSpace", "DeviceRGB");
			if(Compressed) {
				PdfDictionary decodeParms = new PdfDictionary();
				decodeParms.Add("Predictor", 15);
				decodeParms.Add("BitsPerComponent", 8);
				decodeParms.Add("Colors", 3);
				decodeParms.Add("Columns", Size.Width);
				Attributes.Add("DecodeParms", decodeParms);
			}
			if(maskColor != DXColor.Empty) {
				PdfArray maskArray = new PdfArray();
				maskArray.Add(maskColor.R);
				maskArray.Add(maskColor.R);
				maskArray.Add(maskColor.G);
				maskArray.Add(maskColor.G);
				maskArray.Add(maskColor.B);
				maskArray.Add(maskColor.B);
				Attributes.Add("Mask", maskArray);
			}
		}
	}
	public class Pdf24bppRgbImage : PdfTrueColorImage {
		public Pdf24bppRgbImage(Image image, string name, bool compressed)
			: base(image, name, compressed) {
			PixelConverter converter = new ColorChannels24PixelConverter();
			ImageStreamBuilder.Create(Compressed, converter).Build(image, Stream);
		}
	}
	public class Pdf32bppArgbImage : PdfTrueColorImage {
		Pdf8bppGrayscaleAlpaImage maskImage;
		protected override bool UseMaskColor {
			get { return false; }
		}
		public override PdfImageBase MaskImage {
			get { return maskImage; }
		}
		public Pdf32bppArgbImage(Image image, string name, bool compressed)
			: base(image, name, compressed) {
			ColorChannels32PixelConverter converter = new ColorChannels32PixelConverter();
			ImageStreamBuilder.Create(Compressed, converter).Build(image, Stream);
			if(converter.HasTransparentPixels)
				maskImage = new Pdf8bppGrayscaleAlpaImage(image, name + "Mask", compressed, true);
		}
		public override void FillUp() {
			base.FillUp();
			if(maskImage != null)
				Attributes.Add("SMask", maskImage.InnerObject);
		}
	}
	public class Pdf8bppGrayscaleAlpaImage : PdfBitmap {
		public bool HasTransparentPixels { get; private set; }
		bool useMatte;
		public Pdf8bppGrayscaleAlpaImage(Image image, string name, bool compressed, bool useMatte)
			: base(name, image.Size, compressed) {
			this.useMatte = useMatte;
			AlphaChannelPixelConverter converter = new AlphaChannelPixelConverter();
			ImageStreamBuilder.Create(Compressed, converter).Build(image, Stream);
			HasTransparentPixels = converter.HasTransparentPixels;
		}
		public override void FillUp() {
			base.FillUp();
			Attributes.Add("BitsPerComponent", 8);
			Attributes.Add("ColorSpace", "DeviceGray");
			if(useMatte) 
				Attributes.Add("Matte", new PdfArray() { 0, 0, 0 });
			if(Compressed) {
				PdfDictionary decodeParms = new PdfDictionary();
				decodeParms.Add("Predictor", 15);
				decodeParms.Add("BitsPerComponent", 8);
				decodeParms.Add("Colors", 1);
				decodeParms.Add("Columns", Size.Width);
				Attributes.Add("DecodeParms", decodeParms);
			}
		}
	}
	public static class BitmapUtils {
		const int byteCorrection = 4;
		public static byte[] ImageToByteArray(Image image) {
			using(MemoryStream stream = new MemoryStream()) {
				image.Save(stream, ImageFormat.Bmp);
				return stream.ToArray();
			}
		}
		public static int CalculateCorrectedByteWidth(int byteWidth) {
			int correctedByteWidth = byteWidth / byteCorrection * byteCorrection;
			if(byteWidth % byteCorrection > 0)
				correctedByteWidth += byteCorrection;
			return correctedByteWidth;
		}
	}
	class ImageStreamBuilder {
		public static ImageStreamBuilder Create(bool compressed, PixelConverter converter) {
			return compressed ?
			   new CompressedImageStreamBuilder(converter) :
			   new ImageStreamBuilder(converter);
		}
		protected int pdfBpp = 3;
		protected byte[] line;
		PixelConverter converter;
		public ImageStreamBuilder(PixelConverter converter) {
			pdfBpp = converter.TargetBytesPerPixel;
			this.converter = converter;
		}
		public void Build(Image image, PdfStream stream) {
			int bytesPerPixel = converter.SourceBytesPerPixel;
			byte[] tempBuffer = BitmapUtils.ImageToByteArray(image);
			Initialize(image.Width * pdfBpp);
			int byteWidth = image.Width * bytesPerPixel;
			int startIndex = tempBuffer.Length;
			int correctedByteWidth = BitmapUtils.CalculateCorrectedByteWidth(byteWidth);
			for(int lineNumber = 0; lineNumber < image.Height; lineNumber++) {
				startIndex -= correctedByteWidth;
				int j = 0;
				for(int i = startIndex; i < startIndex + byteWidth; i += bytesPerPixel) {
					converter.ExtractPixel(line, ref j, tempBuffer, i);
				}
				PutLineToStream(stream);
			}
		}
		protected virtual void Initialize(int pdfByteWidth) {
			line = new byte[pdfByteWidth];
		}
		protected virtual void PutLineToStream(PdfStream stream) {
			stream.SetBytes(line);
		}
	}
	class CompressedImageStreamBuilder : ImageStreamBuilder {
		enum LineTag : byte {
			None = 0,
			Sub = 1,
			Up = 2,
			Average = 3,
			Paeth = 4
		}
		byte[] prior, sub, up, average, paeth;
		HashSet<byte> noneSet = new HashSet<byte>();
		HashSet<byte> subSet = new HashSet<byte>();
		HashSet<byte> upSet = new HashSet<byte>();
		HashSet<byte> averageSet = new HashSet<byte>();
		HashSet<byte> paethSet = new HashSet<byte>();
		public CompressedImageStreamBuilder(PixelConverter converter)
			: base(converter) {
		}
		protected override void Initialize(int pdfByteWidth) {
			base.Initialize(pdfByteWidth);
			prior = new byte[pdfByteWidth];
			sub = new byte[pdfByteWidth];
			up = new byte[pdfByteWidth];
			average = new byte[pdfByteWidth];
			paeth = new byte[pdfByteWidth];
		}
		protected override void PutLineToStream(PdfStream stream) {
			for(int i = 0; i < line.Length; i++) {
				noneSet.Add(line[i]);
				byte subValue = (i < pdfBpp) ? (byte)0 : line[i - pdfBpp];
				sub[i] = (byte)(line[i] - subValue);
				subSet.Add(sub[i]);
				up[i] = (byte)(line[i] - prior[i]);
				upSet.Add(up[i]);
				average[i] = (byte)(line[i] - ((subValue + prior[i]) / 2));
				averageSet.Add(average[i]);
				paeth[i] = (byte)(line[i] - PaethPredictor(subValue, prior[i], (i < pdfBpp) ? (byte)0 : prior[i - pdfBpp]));
				paethSet.Add(paeth[i]);
			}
			int min = noneSet.Count;
			LineTag tag = LineTag.None;
			byte[] result = line;
			Action<int, LineTag, byte[]> changeTagIfLess = (int value, LineTag tagValue, byte[] lineValue) => {
				if(min > value) {
					tag = tagValue;
					min = value;
					result = lineValue;
				}
			};
			changeTagIfLess(subSet.Count, LineTag.Sub, sub);
			changeTagIfLess(upSet.Count, LineTag.Up, up);
			changeTagIfLess(averageSet.Count, LineTag.Average, average);
			changeTagIfLess(paethSet.Count, LineTag.Paeth, paeth);
			stream.SetByte((byte)tag);
			stream.SetBytes(result);
			byte[] tmp = prior;
			prior = line;
			line = tmp;
			noneSet.Clear();
			subSet.Clear();
			upSet.Clear();
			averageSet.Clear();
			paethSet.Clear();
		}
		static int PaethPredictor(int a, int b, int c) {
			int p = a + b - c;
			int pa = Math.Abs(p - a);
			int pb = Math.Abs(p - b);
			int pc = Math.Abs(p - c);
			if((pa <= pb) && (pa <= pc)) {
				return a;
			}
			else if(pb <= pc) {
				return b;
			}
			else {
				return c;
			}
		}
	}
	interface PixelConverter {
		int TargetBytesPerPixel { get; }
		int SourceBytesPerPixel { get; }
		void ExtractPixel(byte[] line, ref int j, byte[] tempBuffer, int i);
	}
	class ColorChannels24PixelConverter : PixelConverter {
		public int TargetBytesPerPixel {
			get { return 3; }
		}
		public int SourceBytesPerPixel {
			get { return 3; }
		}
		public void ExtractPixel(byte[] line, ref int j, byte[] tempBuffer, int i) {
			line[j++] = tempBuffer[i + 2];
			line[j++] = tempBuffer[i + 1];
			line[j++] = tempBuffer[i];
		}
	}
	class ColorChannels32PixelConverter : PixelConverter {
		bool hasTransparentPixels;
		public bool HasTransparentPixels { get { return hasTransparentPixels; } }
		public int TargetBytesPerPixel {
			get { return 3; }
		}
		public int SourceBytesPerPixel {
			get { return 4; }
		}
		public void ExtractPixel(byte[] line, ref int j, byte[] tempBuffer, int i) {
			byte alpha = tempBuffer[i + 3];
			if(alpha == 255) {
				line[j++] = tempBuffer[i + 2];
				line[j++] = tempBuffer[i + 1];
				line[j++] = tempBuffer[i];
				return;
			}
			hasTransparentPixels |= true;
			if(alpha != 0) {
				float a = alpha / 255.0f;
				line[j++] = (byte)(a * tempBuffer[i + 2]);
				line[j++] = (byte)(a * tempBuffer[i + 1]);
				line[j++] = (byte)(a * tempBuffer[i]);
			} else {
				line[j++] = 0;
				line[j++] = 0;
				line[j++] = 0;
			}
		}
	}
	class AlphaChannelPixelConverter : PixelConverter {
		bool hasTransparentPixels;
		public bool HasTransparentPixels { get { return hasTransparentPixels; } }
		public int TargetBytesPerPixel {
			get { return 1; }
		}
		public int SourceBytesPerPixel {
			get { return 4; }
		}
		public void ExtractPixel(byte[] line, ref int j, byte[] tempBuffer, int i) {
			byte alpha = tempBuffer[i + 3];
			hasTransparentPixels |= alpha < 255;
			line[j++] = alpha;
		}
	}
}
