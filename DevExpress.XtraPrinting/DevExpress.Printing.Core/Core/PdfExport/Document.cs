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
using System.Collections;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Localization;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfDocument : IDisposable {
		PdfHeader header;
		PdfXRef xRef = new PdfXRef();
		PdfTrailer trailer;
		PdfCatalog catalog;
		PdfFonts fonts = new PdfFonts();
		PdfAnnotations annotations = new PdfAnnotations();
		PdfTransparencyGSCollection transparencyGS;
		PdfShadingCollection shading;
		PdfPatternCollection patterns;
		PdfInfo info;
		IList destinationInfos = new ArrayList();
		int imageCount;
		bool compressed;
		bool pdfACompatible;
		bool convertImagesToJpeg;
		PdfJpegImageQuality jpegImageQuality = PdfJpegImageQuality.Highest;
		PdfEncryption encryption;
		PdfSignature signature;
		byte[] id;
		public PdfDocument(bool compressed, bool showPrintDialog) {
			this.compressed = compressed;
			this.trailer = new PdfTrailer(xRef);
			this.catalog = PdfCatalog.CreateInstance(compressed, showPrintDialog);
			this.info = new PdfInfo(compressed);
			this.encryption = new PdfEncryption(this);
			this.signature = new PdfSignature();
			this.header = new PdfHeader(this);
			this.transparencyGS = new PdfTransparencyGSCollection(compressed);
			this.shading = new PdfShadingCollection(compressed);
			this.patterns = new PdfPatternCollection(compressed);
		}
		public PdfHeader Header { get { return header; } }
		public PdfCatalog Catalog { get { return catalog; } }
		public PdfXRef XRef { get { return xRef; } }
		public PdfTrailer Trailer { get { return trailer; } }
		public int PageCount { get { return catalog.Pages.LeafCount; } }
		public IList DestinationInfos { get { return destinationInfos; } }
		public PdfFonts Fonts { get { return fonts; } }
		public PdfInfo Info { get { return info; } }
		public PdfEncryption Encryption { get { return encryption; } }
		public PdfSignature Signature { get { return signature; } }
		public bool Compressed { get { return compressed; } }
		public bool ConvertImagesToJpeg { get { return convertImagesToJpeg; } set { convertImagesToJpeg = value; } }
		public PdfJpegImageQuality JpegImageQuality { get { return jpegImageQuality; } set { jpegImageQuality = value; } }
		public byte[] ID { get { return id; } }
		internal PdfTransparencyGSCollection TransparencyGS { get { return transparencyGS; } }
		internal PdfShadingCollection Shading { get { return shading; } }
		internal PdfPatternCollection Patterns { get { return patterns; } }
		public bool PdfACompatible { 
			get { return pdfACompatible; }
			set { pdfACompatible = Catalog.PdfACompatible = value; }  
		}
		void FillXRef() {
			catalog.Register(xRef);
			fonts.Register(xRef);
			annotations.Register(xRef);
			info.Register(xRef);
			if(encryption.IsEncryptionOn)
				encryption.Register(xRef);
			transparencyGS.Register(xRef);
			shading.Register(xRef);
			patterns.Register(xRef);
		}
		void FillUp() {
			info.FillUp();
			catalog.FillUp();
			fonts.FillUp();
			annotations.FillUp();
			encryption.FillUp();
			transparencyGS.FillUp();
			shading.FillUp();
			patterns.FillUp();
		}
		void PreWrite() {
			FillXRef();
			FillUp();
			trailer.Attributes.Add("Size", new PdfNumber(xRef.Count));
			trailer.Attributes.Add("Info", info.Dictionary);
			trailer.Attributes.Add("Root", catalog.Dictionary);
			if(encryption.IsEncryptionOn)
				trailer.Attributes.Add("Encrypt", encryption.Dictionary);
			if(encryption.IsEncryptionOn || PdfACompatible) {
				PdfArray idArray = new PdfArray();
				idArray.Add(new PdfHexadecimalString(id));
				idArray.Add(new PdfHexadecimalString(id));
				trailer.Attributes.Add("ID", idArray);
			}
		}
		void CreateGotoActions() {
			foreach(DestinationInfo item in DestinationInfos) {
				PdfPage destPage = GetPage(item.DestPageIndex);
				if(destPage == null || item.LinkPage == null)
					continue;
				PdfDestination dest = new PdfDestination(destPage, item.DestTop);
				PdfAction action = new PdfGoToAction(dest, this.compressed);
				PdfAnnotation annot = CreateLinkAnnotation(action, Utils.ToPdfRectangle(item.LinkArea));
				item.LinkPage.AddAnnotation(annot);
			}
		}
		string CreateImageName() {
			return String.Format("Img{0}", imageCount++);
		}
		byte[] CreateDocumentId() {			
			MD5 md5 = new MD5CryptoServiceProvider();
			long time = DateTime.Now.Ticks + Environment.TickCount;
			long mem = GC.GetTotalMemory(false);
			String s = time + "+" + mem;
			return md5.ComputeHash(DXEncoding.ASCII.GetBytes(s));
		}
		internal PdfLinkAnnotation CreateLinkAnnotation(PdfAction action, PdfRectangle rect) {
			PdfLinkAnnotation annot = new PdfLinkAnnotation(action, rect, this.compressed);
			annot.PdfACompatible = this.PdfACompatible;
			RegisterAnnotation(annot);
			return annot;
		}
		protected internal void BeforeWrite() {
			catalog.PrepareOutlines();
			CreateGotoActions();
			PrepareSignature();
		}
		void PrepareSignature() {
			if(signature.Active) {
				int pageIndex = 0;
				PdfPage page = this.catalog.Pages.GetPage(ref pageIndex);
				if(page == null)
					return;
				PdfSignatureWidgetAnnotation annotation = new PdfSignatureWidgetAnnotation(page, signature, Compressed);
				page.AddAnnotation(annotation);
				annotations.AddUnique(annotation);
				catalog.AddFormField(annotation);
			}
		}
		protected internal void AfterWrite() {
			destinationInfos.Clear();
		}
		protected internal PdfDestination CreatePdfDestination(DestinationInfo info) {
			PdfPage destPage = GetPage(info.DestPageIndex);
			return (destPage != null) ? new PdfDestination(destPage, info.DestTop) : null;
		}
		public void Write(StreamWriter writer, ProgressReflector progressReflector) {
			PreWrite();
			catalog.Write(writer);
			progressReflector.RangeValue++;
			fonts.Write(writer);
			progressReflector.RangeValue++;
			annotations.Write(writer);
			transparencyGS.Write(writer);
			shading.Write(writer);
			patterns.Write(writer);
			info.Write(writer);
			if(encryption.IsEncryptionOn)
				encryption.Write(writer);
			if(signature.Active)
				signature.Write(writer);
			xRef.Write(writer);
			trailer.Write(writer);
			if(signature.Active)
				signature.Finish(writer);
		}
		public void WriteHeader(StreamWriter writer) {
			header.Write(writer);
		}
		public PdfPage GetPage(int index) {
			if(index < 0)
				return null;
			return catalog.Pages.GetPage(ref index);
		}
		public PdfAnnotation RegisterAnnotation(PdfAnnotation annotation) {
			annotations.AddUnique(annotation);
			return annotation;
		}
		public int AddDestinationInfo(DestinationInfo info) {
			if(info != null && !destinationInfos.Contains(info))
				return destinationInfos.Add(info);
			return -1;
		}
		public PdfFont RegisterFontSmart(Font original, ref string actualString, ref Font newFont) {
			Font font = original;
			if(original.Unit != GraphicsUnit.Point) {
				font = newFont = new Font(original.FontFamily, DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(original), original.Style);
			}
			PdfFont res = FindFont(font);
			if(res != null) return res;
			Font actualFont = font;
			try {
				res = RegisterFont(actualFont);
			}
			catch {
				actualString = PreviewLocalizer.GetString(PreviewStringId.Msg_NotSupportedFont);
				using(Font tahoma = new Font("Tahoma", 8)) {
					res = RegisterFont(tahoma);
				}
			}
			return res;
		}
		PdfFont FindFont(Font font) {
			return this.fonts.FindFont(font);
		}
		public PdfFont RegisterFont(Font font) {
			return this.fonts.RegisterFont(font, this.compressed);
		}
		public PdfImageBase CreatePdfImage(IPdfDocumentOwner documentInfo, IXObjectsOwner xObjectsOwner, Image image, Color actualBackColor) {
			PdfImageBase pdfImage = CreatePdfImage(documentInfo, image, actualBackColor);
			xObjectsOwner.AddNewXObject(pdfImage);
			if(pdfImage.MaskImage != null)
				xObjectsOwner.AddNewXObject(pdfImage.MaskImage);
			return pdfImage;
		}
		PdfImageBase CreatePdfImage(IPdfDocumentOwner documentInfo, Image image, Color actualBackColor) {
			if(DXColor.IsEmpty(actualBackColor))
				return CreatePdfImage(documentInfo, image);
			else {
				using(Image img = BitmapCreator.CreateBitmapWithResolutionLimit(image, actualBackColor)) {
					return CreatePdfImage(documentInfo, img);
				}
			}
		}
		PdfImageBase CreatePdfImage(IPdfDocumentOwner documentInfo, Image image) {
			return PdfImageBase.CreateInstance(documentInfo, image, this, CreateImageName(), this.compressed, this.convertImagesToJpeg, this.jpegImageQuality);
		}
		public void Dispose() {
			this.fonts.DisposeAndClear();
		}
		public void Calculate() {
			if(encryption.IsEncryptionOn || PdfACompatible)
				id = CreateDocumentId();
			if(encryption.IsEncryptionOn)
				encryption.Calculate();
		}
	}	
	public class PdfHeader {
		PdfDocument document;
		public PdfHeader(PdfDocument document) {
			this.document = document;
		}
		public void Write(StreamWriter writer) {
			string header = "%PDF-1.4";
			if(document.Encryption != null && document.Encryption.IsEncryptionOn)
				header = "%PDF-1.6";
			writer.WriteLine(header);
			if(document.PdfACompatible) {
				writer.Write("%");
				writer.Flush();
				writer.BaseStream.WriteByte((byte)0xCA);
				writer.BaseStream.WriteByte((byte)0xE0);
				writer.BaseStream.WriteByte((byte)0xFB);
				writer.BaseStream.WriteByte((byte)0xAC);
				writer.WriteLine("");
			}
		}
	}
	public class PdfTrailer {
		PdfDictionary attributes = new PdfDictionary();
		PdfXRef xRef;
		public PdfDictionary Attributes { get { return attributes; } }
		public PdfXRef XRef { get { return xRef; } }
		public PdfTrailer(PdfXRef xRef) {
			this.xRef = xRef;
		}
		long GetXRefByteOffset() {
			return (xRef != null)? xRef.ByteOffset: 0;
		}
		public void Write(StreamWriter writer) {
			writer.WriteLine("trailer");
			attributes.WriteToStream(writer);
			writer.WriteLine();
			writer.WriteLine("startxref");
			writer.WriteLine(Convert.ToString(GetXRefByteOffset()));
			writer.Write("%%EOF");
		}
	}
	public class PdfStreamWriter : StreamWriter {
		PdfDocument document;
		public PdfStreamWriter(Stream stream, PdfDocument document)
			: base(stream) {
			this.document = document;
		}
		public string EncryptString(string text, PdfObject pdfObject) {
			if(document.Encryption == null || pdfObject.ChainingIndirectReference == null)
				return text;
			return document.Encryption.EncryptString(text, pdfObject.ChainingIndirectReference.Number, PdfIndirectReference.Generation);
		}
		public MemoryStream EncryptStream(MemoryStream stream, PdfObject pdfObject) {
			if(document.Encryption == null || pdfObject.ChainingIndirectReference == null)
				return stream;
			return document.Encryption.EncryptStream(stream, pdfObject.ChainingIndirectReference.Number, PdfIndirectReference.Generation);
		}
	}
}
