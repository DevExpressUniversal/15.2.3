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
using System.IO;
using System.Text;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Office.PInvoke;
using DevExpress.Office.Model;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing.Imaging;
using System.Diagnostics;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region PictureSourceType
	public enum PictureSourceType {
		Emf,
		Png,
		Jpeg,
		Mac,
		PmmMetafile,
		Wmf,
		WindowsDib,
		WindowsBmp
	};
	#endregion
	public class PictureDestinationInfo {
		PictureSourceType pictureSourceType = PictureSourceType.WindowsBmp;
		Win32.MapMode wmfMapMode = Win32.MapMode.Text;
		int bmpBitsPerPixel = 1;
		int bmpColorPlanes = 1;
		int bmpBytesInLine;
		int pictureWidth = -1;
		int pictureHeight = -1;
		int desiredPictureWidth = -1;
		int desiredPictureHeight = -1;
		int scaleX = 100;
		int scaleY = 100;
		MemoryStream pictureStream;
		Crc32Stream dataStream;
		string imageUri = String.Empty;
		RtfShapeProperties properties;
		public PictureDestinationInfo() {
			this.pictureStream = new MemoryStream();
			this.dataStream = new Crc32Stream(pictureStream);
			this.properties = new RtfShapeProperties();
		}
		public MemoryStream PictureStream { get { return pictureStream; } }
		public Stream DataStream { get { return dataStream; } }
		public int DataCrc32 { get { return (int)dataStream.WriteCheckSum; } }
		public string ImageUri { get { return imageUri; } set { imageUri = value; } }
		public PictureSourceType PictureSourceType { get { return pictureSourceType; } set { pictureSourceType = value; } }
		public Win32.MapMode WmfMapMode { get { return wmfMapMode; } set { wmfMapMode = value; } }
		public int BmpBitsPerPixel { get { return bmpBitsPerPixel; } set { bmpBitsPerPixel = value; } }
		public int BmpColorPlanes { get { return bmpColorPlanes; } set { bmpColorPlanes = value; } }
		public int BmpBytesInLine { get { return bmpBytesInLine; } set { bmpBytesInLine = value; } }
		public int PictureWidth { get { return pictureWidth; } set { pictureWidth = value; } }
		public int PictureHeight { get { return pictureHeight; } set { pictureHeight = value; } }
		public int DesiredPictureWidth { get { return desiredPictureWidth; } set { desiredPictureWidth = value; } }
		public int DesiredPictureHeight { get { return desiredPictureHeight; } set { desiredPictureHeight = value; } }
		public int ScaleX { get { return scaleX; } set { scaleX = value; } }
		public int ScaleY { get { return scaleY; } set { scaleY = value; } }
		public RtfShapeProperties Properties { get { return properties; } }
		public void ClosePictureStream() {
			if (dataStream != null) {
				dataStream.Dispose();
				dataStream = null;
			}
			if (pictureStream != null) {
				pictureStream.Dispose();
				pictureStream = null;
			}
		}
	}
#region PictureDestination
	public class PictureDestination : HexContentDestination {
#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("emfblip", OnEmfFileKeyword);
			table.Add("pngblip", OnPngFileKeyword);
			table.Add("jpegblip", OnJpegFileKeyword);
			table.Add("macpict", OnMacFileKeyword);
			table.Add("wmetafile", OnWindowsMetafileKeyword);
			table.Add("dibitmap", OnDeviceIndependentBitmapFileKeyword);
			table.Add("wbitmap", OnDeviceDependentBitmapFileKeyword);
			table.Add("wbmbitspixel", OnBitmapBitsPerPixelKeyword);
			table.Add("wbmplanes", OnBitmapPlanesKeyword);
			table.Add("wbmwidthbytes", OnBitmapBytesInLineKeyword);
			table.Add("picw", OnPictureWidthKeyword);
			table.Add("pich", OnPictureHeightKeyword);
			table.Add("picwgoal", OnPictureGoalWidthKeyword);
			table.Add("pichgoal", OnPictureGoalHeightKeyword);
			table.Add("picscalex", OnHorizontalScalingKeyword);
			table.Add("picscaley", OnVerticalScalingKeyword);
			table.Add("picscaled", OnPicScaledKeyword);
			table.Add("piccropt", OnTopCropKeyword);
			table.Add("piccropb", OnBottomCropKeyword);
			table.Add("piccropr", OnLeftCropKeyword);
			table.Add("piccropl", OnRightCropKeyword);
			table.Add("picbmp", OnBitmapMetafileKeyword);
			table.Add("picbpp", OnBitsPerPixelBitmapMetafileKeyword);
			table.Add("dximageuri", OnDxImageUri);
			table.Add("picprop", OnShapePropertiesKeyword);
			return table;
		}
#endregion
#region processing control char and keyword
		static void OnEmfFileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.PictureSourceType = PictureSourceType.Emf;
		}
		static void OnPngFileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.PictureSourceType = PictureSourceType.Png;
		}
		static void OnJpegFileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.PictureSourceType = PictureSourceType.Jpeg;
		}
		static void OnMacFileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.PictureSourceType = PictureSourceType.Mac;
		}
		static void OnWindowsMetafileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.PictureSourceType = PictureSourceType.Wmf;
			if (hasParameter)
				destination.WmfMapMode = (Win32.MapMode)parameterValue;
		}
		static void OnDeviceIndependentBitmapFileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter && parameterValue != 0)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.PictureSourceType = PictureSourceType.WindowsDib;
		}
		static void OnDeviceDependentBitmapFileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (hasParameter && parameterValue != 0)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.PictureSourceType = PictureSourceType.WindowsBmp;
		}
		static void OnBitmapBitsPerPixelKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				parameterValue = 1;
			bool isParameterValueCorrect = parameterValue == 1 ||
				parameterValue == 4 || parameterValue == 8 || parameterValue == 16 || parameterValue == 24 || parameterValue == 32;
			if (!isParameterValueCorrect)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.BmpBitsPerPixel = parameterValue;
		}
		static void OnBitmapPlanesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				RtfImporter.ThrowInvalidRtfFile();
			if (parameterValue != 1)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.BmpColorPlanes = parameterValue;
		}
		static void OnBitmapBytesInLineKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.BmpBytesInLine = parameterValue;
		}
		static void OnPictureWidthKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			int correctedValue = parameterValue;
			if (parameterValue < 0 && parameterValue == (short)parameterValue) 
				correctedValue = FillBytesToConvertFromShortIntToLongInt((short)parameterValue);
			destination.PictureWidth = correctedValue;
		}
		static void OnPictureHeightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			int correctedValue = parameterValue;
			if (CheckIfParameterStoredAsShortIntegerInsteadLongInt(parameterValue)) 
				correctedValue = FillBytesToConvertFromShortIntToLongInt((short)parameterValue);
			destination.PictureHeight = correctedValue;
		}
		static bool CheckIfParameterStoredAsShortIntegerInsteadLongInt(int parameterValue) {
			return parameterValue < 0 && parameterValue == (short)parameterValue;
		}
		static int FillBytesToConvertFromShortIntToLongInt(short parameterValue) {
			return (UInt16)0xffff & ((UInt16)(parameterValue));
		}
		static void OnPictureGoalWidthKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.DesiredPictureWidth = parameterValue;
		}
		static void OnPictureGoalHeightKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.DesiredPictureHeight = parameterValue;
		}
		static void OnHorizontalScalingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.ScaleX = parameterValue;
		}
		static void OnVerticalScalingKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			if (!hasParameter)
				RtfImporter.ThrowInvalidRtfFile();
			PictureDestination destination = (PictureDestination)importer.Destination;
			destination.ScaleY = parameterValue;
		}
		static void OnPicScaledKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnTopCropKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnBottomCropKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnLeftCropKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnRightCropKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnBitmapMetafileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnBitsPerPixelBitmapMetafileKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnDxImageUri(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new DxImageUriDestination(importer, (PictureDestination)importer.Destination);
		}
		static void OnShapePropertiesKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ShapePropertyDestination(importer, ((PictureDestination)importer.Destination).Properties);
		}
#endregion
		static RtfPictureUnitsConverter rtfHundredthsOfMillimeterConverter = new RtfHundredthsOfMillimeterConverter();
		static RtfPictureUnitsConverter rtfPixelsConverter = new RtfPixelsToTwipsConverter(96);
#region Fields
		PictureDestinationInfo info;
		CodePageCharacterDecoder oldDecoder;
#endregion
		public PictureDestination(RtfImporter importer)
			: base(importer) {
			this.info = new PictureDestinationInfo();
			this.oldDecoder = Importer.Position.RtfFormattingInfo.Decoder;
			Importer.Position.RtfFormattingInfo.SetDecoder(new EmptyCharacterDecoder());
		}
#region Properties
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		public PictureDestinationInfo Info { get { return info; } }
		public MemoryStream PictureStream { get { return Info.PictureStream; } }
		public Stream DataStream { get { return Info.DataStream; } }
		public int DataCrc32 { get { return Info.DataCrc32; } }
		PictureSourceType PictureSourceType { get { return Info.PictureSourceType; } set { Info.PictureSourceType = value; } }
		Win32.MapMode WmfMapMode { get { return Info.WmfMapMode; } set { Info.WmfMapMode = value; } }
		int BmpBitsPerPixel { get { return Info.BmpBitsPerPixel; } set { Info.BmpBitsPerPixel = value; }}
		int BmpColorPlanes { get { return Info.BmpColorPlanes; } set { Info.BmpColorPlanes = value; }}
		int BmpBytesInLine { get { return Info.BmpBytesInLine; } set { Info.BmpBytesInLine = value; } }
		int PictureWidth { 
			get { return Info.PictureWidth; } 
			set { Info.PictureWidth = value; } 
		}
		int PictureHeight { get { return Info.PictureHeight; } set { Info.PictureHeight = value; } }
		int DesiredPictureWidth { get { return Info.DesiredPictureWidth; } set { Info.DesiredPictureWidth = value; } }
		int DesiredPictureHeight { get { return Info.DesiredPictureHeight; } set { Info.DesiredPictureHeight = value; } }
		int ScaleX { get { return Info.ScaleX; } set { Info.ScaleX = value; } }
		int ScaleY { get { return Info.ScaleY; } set { Info.ScaleY = value; } }
		RtfShapeProperties Properties { get { return Info.Properties; } }
		public string ImageUri { get { return Info.ImageUri; } set { Info.ImageUri = value; } }
#endregion
		protected override DestinationBase CreateClone() {
			PictureDestination clone = new PictureDestination(Importer);
			clone.info = this.info;
			return clone;
		}
		public override void BeforePopRtfState() {
			Importer.Position.RtfFormattingInfo.SetDecoder(this.oldDecoder);
			base.BeforePopRtfState();
		}
		protected override void ProcessBinCharCore(char ch) {
			DataStream.WriteByte((byte)ch);
		}
		void ValidateImageSize(OfficeImage image) {
			if (PictureWidth <= 0 || PictureHeight <= 0) {
				Size imageSize = image.SizeInHundredthsOfMillimeter;
				if (PictureWidth <= 0)
					PictureWidth = imageSize.Width;
				if (PictureHeight <= 0)
					PictureHeight = imageSize.Height;
			}
		}
		void LoadMetafile(RtfImageInfo info) {
			if (info.RtfImage == null) {
				try {
					info.LoadMetafileFromStream(PictureStream, WmfMapMode, PictureWidth, PictureHeight);
				}
				catch {
					return;
				}
			}
			ValidateImageSize(info.RtfImage);
			OfficeImageFormat actualFormat = info.RtfImage.RawFormat;
			if (actualFormat == OfficeImageFormat.Wmf || actualFormat == OfficeImageFormat.Emf)
				LoadMetafileImageInUnits(info, rtfHundredthsOfMillimeterConverter);
			else
				LoadImageInUnits(info, rtfPixelsConverter);
		}
		void LoadImage(RtfImageInfo info) {
			if (info.RtfImage == null) {
				try {
					info.LoadImageFromStream(new MemoryStream(PictureStream.GetBuffer(), 0, (int)PictureStream.Length));	 
				}
				catch {
					return;
				}
			}
			ValidateImageSize(info.RtfImage);
			LoadImageInUnits(info, rtfHundredthsOfMillimeterConverter);
		}
		void LoadBitmap(RtfImageInfo info) {
			if (info.RtfImage == null) {
				try {
					if (BmpBytesInLine % 2 != 0 || BmpBytesInLine == 0)
						RtfImporter.ThrowInvalidRtfFile();
					info.LoadBitmapFromStream(PictureStream, PictureWidth, PictureHeight, BmpColorPlanes, BmpBitsPerPixel, BmpBytesInLine);
				}
				catch {
					return;
				}
			}
			LoadImageInUnits(info, rtfPixelsConverter);
		}
		void LoadDib(RtfImageInfo info) {
			if (info.RtfImage == null) {
				try {
					if (BmpBytesInLine % 2 != 0 || BmpBytesInLine == 0) {
						Debug.Assert(false);
						BmpBytesInLine = ((PictureWidth + 15) / 16) * 16;
					}
					info.LoadDibFromStream(PictureStream, PictureWidth, PictureHeight, BmpBytesInLine);
				}
				catch {
					return;
				}
			}
			LoadImageInUnits(info, rtfPixelsConverter);
		}
		void LoadImageInUnits(RtfImageInfo imageInfo, RtfPictureUnitsConverter unitsConverter) {						
#if !SL && !DXPORTABLE
			if (imageInfo.RtfImage.NativeImage is Metafile) {
				LoadMetafileImageInUnits(imageInfo, unitsConverter);
				return;
			}
#endif
			PictureWidth = imageInfo.RtfImage.SizeInTwips.Width;
			PictureHeight = imageInfo.RtfImage.SizeInTwips.Height;
			if (DesiredPictureWidth <= 0)
				DesiredPictureWidth = PictureWidth;
			if (DesiredPictureHeight <= 0)
				DesiredPictureHeight = PictureHeight;
			if (ScaleX <= 0)
				ScaleX = 100;
			if (ScaleY <= 0)
				ScaleY = 100;
			if (PictureWidth > 0 && DesiredPictureWidth > 0)
				imageInfo.ScaleX = (int)Math.Round(ScaleX * DesiredPictureWidth / (double)PictureWidth);
			if (PictureHeight > 0 && DesiredPictureHeight > 0)
				imageInfo.ScaleY = (int)Math.Round(ScaleY * DesiredPictureHeight / (double)PictureHeight);
			DocumentModelUnitConverter modelUnitConverter = Importer.UnitConverter;
			int widthInModelUnits = Math.Max(1, modelUnitConverter.TwipsToModelUnits(PictureWidth));
			int heightInModelUnits = Math.Max(1, modelUnitConverter.TwipsToModelUnits(PictureHeight));
			imageInfo.SizeInModelUnits = new Size(widthInModelUnits, heightInModelUnits);
		}
		void LoadMetafileImageInUnits(RtfImageInfo imageInfo, RtfPictureUnitsConverter unitsConverter) {
			if (DesiredPictureWidth <= 0)
				DesiredPictureWidth = unitsConverter.UnitsToTwips(PictureWidth);
			if (DesiredPictureHeight <= 0)
				DesiredPictureHeight = unitsConverter.UnitsToTwips(PictureHeight);
			if (ScaleX <= 0)
				ScaleX = 100;
			if (ScaleY <= 0)
				ScaleY = 100;
			if (PictureWidth > 0 && DesiredPictureWidth > 0)
				imageInfo.ScaleX = (int)Math.Round(ScaleX * DesiredPictureWidth / (float)unitsConverter.UnitsToTwips(PictureWidth));
			if (PictureHeight > 0 && DesiredPictureHeight > 0)
				imageInfo.ScaleY = (int)Math.Round(ScaleY * DesiredPictureHeight / (float)unitsConverter.UnitsToTwips(PictureHeight));
			DocumentModelUnitConverter modelUnitConverter = Importer.UnitConverter;
			int widthInModelUnits = Math.Max(1, unitsConverter.UnitsToModelUnits(PictureWidth, modelUnitConverter));
			int heightInModelUnits = Math.Max(1, unitsConverter.UnitsToModelUnits(PictureHeight, modelUnitConverter));
			imageInfo.SizeInModelUnits = new Size(widthInModelUnits, heightInModelUnits);
		}
		RtfImageInfo LoadPicture() {
#if !SL
			RtfImageInfo info = new RtfImageInfoWin(Importer.DocumentModel);
#else
			RtfImageInfo info = new RtfImageInfoSL(Importer.DocumentModel);
#endif
			LoadPictureCore(info);
			if (info.RtfImage == null)
				return null;
			if (String.IsNullOrEmpty(info.RtfImage.Uri))
				info.RtfImage.Uri = ImageUri;
			Debug.Assert(info.RtfImage != null);
			return info;
		}
		void LoadPictureCore(RtfImageInfo info) {
			int crc32 = DataCrc32;
			info.ImageId = new RtfImageId(crc32);
			ImageCacheBase imageCache = Importer.DocumentModel.ImageCache;
			OfficeReferenceImage cachedImage = imageCache.GetImageById(info.ImageId);
			info.RtfImage = cachedImage;
			switch (PictureSourceType) {
				case PictureSourceType.Emf:
				case PictureSourceType.Wmf:
					LoadMetafile(info);
					break;
				case PictureSourceType.WindowsBmp:
					LoadBitmap(info);
					break;
				case PictureSourceType.WindowsDib:
					LoadDib(info);
					break;
				default:
					LoadImage(info);
					break;
			}
			if (cachedImage == null && info.RtfImage != null)
				info.RtfImage = imageCache.AddImage(info.RtfImage.NativeRootImage);
		}
#region RtfPictureUnitsConverter
		abstract class RtfPictureUnitsConverter {
			public abstract int UnitsToTwips(int val);
			public abstract int UnitsToModelUnits(int val, DocumentModelUnitConverter unitConverter);
		}
		class RtfPixelsToTwipsConverter : RtfPictureUnitsConverter {
			int dpi;
			public RtfPixelsToTwipsConverter(int dpi) {
				this.dpi = dpi;
			}
			public override int UnitsToTwips(int val) {
				return (int)Math.Round(1440 * val / (float)dpi);
			}
			public override int UnitsToModelUnits(int val, DocumentModelUnitConverter unitConverter) {
				return unitConverter.PixelsToModelUnits(val, dpi);
			}
		}
		class RtfHundredthsOfMillimeterConverter : RtfPictureUnitsConverter {
			public override int UnitsToTwips(int val) {
				return (int)Math.Round(1440 * val / 2540.0);
			}
			public override int UnitsToModelUnits(int val, DocumentModelUnitConverter unitConverter) {
				return unitConverter.HundredthsOfMillimeterToModelUnitsRound(val);
			}
		}
#endregion
		public RtfImageInfo GetImageInfo() {
			if (PictureStream.Length <= 0)
				return null;
			PictureStream.Seek(0, SeekOrigin.Begin);
			RtfImageInfo imageInfo = LoadPicture();
			if (imageInfo != null) {
				if (Properties.HasBoolProperty("fPseudoInline"))
					imageInfo.PseudoInline = Properties.GetBoolPropertyValue("fPseudoInline");
			}
			Info.ClosePictureStream();
			return imageInfo;
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (Info != null)
						Info.ClosePictureStream();
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
	}
#endregion
#region DxImageUriDestination
	public class DxImageUriDestination : StringValueDestination {
		readonly PictureDestination pictureDestination;
		StringBuilder stringBuilder = new StringBuilder();
		public DxImageUriDestination(RtfImporter rtfImporter, PictureDestination pictureDestination)
			: base(rtfImporter) {
			Guard.ArgumentNotNull(pictureDestination, "pictureDestination");
			this.pictureDestination = pictureDestination;
		}
		public override string Value { get { return stringBuilder.ToString(); } }
		protected override void ProcessCharCore(char ch) {
			stringBuilder.Append(ch);
		}
		protected override bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			return false;
		}
		protected internal override StringValueDestination CreateEmptyClone() {
			return new DxImageUriDestination(Importer, pictureDestination);
		}
		public override void AfterPopRtfState() {
			pictureDestination.ImageUri = Value;
		}
		protected override DestinationBase CreateClone() {
			DxImageUriDestination clone = (DxImageUriDestination)CreateEmptyClone();
			clone.stringBuilder = new StringBuilder(stringBuilder.ToString());
			return clone;
		}
	}
#endregion
}
