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
using System.Collections;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Data.Helpers;
#if SL 
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Printing.Stubs;
using DevExpress.Xpf.Drawing.Drawing2D;
using System.Windows.Media;
using DevExpress.Xpf.Windows.Forms;
using DevExpress.Xpf.Collections;
using DevExpress.Xpf.Drawing.Imaging;
using DevExpress.Xpf.Security;
using Matrix = DevExpress.Xpf.Printing.Stubs.Matrix;
using Brush = DevExpress.Xpf.Drawing.Brush;
#else
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Security.Permissions;
using DevExpress.Pdf.Common;
#endif
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfGraphics : GraphicsBase, IGraphics, IPdfDocumentOwner {
		public static bool EnableAzureCompatibility { get; set; }
		public static bool RenderMetafileAsBitmap { get; set; }
		PdfGraphicsImpl pdfGraphicsImpl;
		PdfDocument document;
		PdfPageInfo pageInfo;
		PdfStreamWriter writer;
		bool flashed;
		GraphicsUnit pageUnit = GraphicsUnit.Document;
		SmoothingMode smoothingMode = SmoothingMode.None;
		Color imageBackColor = DXColor.Empty;
		PdfImageCache imageCache = new PdfImageCache();
		PdfNeverEmbeddedFonts neverEmbeddedFonts = new PdfNeverEmbeddedFonts();
		PdfHashtable pdfHashtable = new PdfHashtable();
		bool scaleStrings;
		Measurer metafileMeasurer;
		public bool ScaleStrings {
			get { return scaleStrings; }
			set { scaleStrings = value; }
		}
		PdfImageCache IPdfDocumentOwner.ImageCache { get { return imageCache; } }
		PdfNeverEmbeddedFonts IPdfDocumentOwner.NeverEmbeddedFonts { get { return neverEmbeddedFonts; } }
		Measurer IPdfDocumentOwner.MetafileMeasurer {
			get {
				if(metafileMeasurer == null)
					metafileMeasurer = new GdiPlusMeasurer();
				return metafileMeasurer;
			}
		}
		public PdfGraphics(Stream stream, PrintingSystemBase ps)
			: this(stream, ps.ExportOptions.Pdf, ps) {
		}
		public PdfGraphics(Stream stream, PdfExportOptions options, PrintingSystemBase ps)
			: base(ps) {
			document = new PdfDocument(options.Compressed, options.ShowPrintDialogOnOpen);
			ApplyExportOptions(options);
			document.Calculate();
			writer = new PdfStreamWriter(stream, document);
			document.WriteHeader(writer);
		}
		public Color ImageBackColor { get { return imageBackColor; } set { imageBackColor = value; } }
		public void Flush() {
			if(!flashed) {
				WriteDocument();
				if(this.document != null) {
					this.document.Dispose();
					this.document = null;
				}
				pdfHashtable.Clear();
				flashed = true;
				if(metafileMeasurer != null) {
					metafileMeasurer.Dispose();
					metafileMeasurer = null;
				}
			}
		}
		public void AddPage(float width, float height) {
			AddPage(new SizeF(width, height));
		}
		public void AddPage(SizeF pageSize) {
			if(pageSize.Width <= 0)
				throw new PdfGraphicsException("Invalid page width");
			if(pageSize.Height <= 0)
				throw new PdfGraphicsException("Invalid page height");
			ClosePage();
			OpenPage(pageSize);
			this.pdfGraphicsImpl = new PdfGraphicsImpl(this, this.pageInfo.Context, this.pageInfo.SizeInPoints, this.document, this.pageInfo);
			this.pdfGraphicsImpl.PageUnit = pageUnit;
			ClipBounds = new RectangleF(PointF.Empty, pageSize);
		}
		public void SetUriArea(string uri, RectangleF bounds) {
			bounds = PdfCoordinate.CorrectRectangle(pageUnit, bounds, pageInfo.SizeInPoints);
			PdfLinkAnnotation annot = document.CreateLinkAnnotation(new PdfURIAction(uri, this.document.Compressed), Utils.ToPdfRectangle(bounds));
			this.pageInfo.Page.AddAnnotation(annot);
		}
		public void SetGoToArea(int destPageIndex, float destTop, RectangleF bounds) {
			this.document.AddDestinationInfo(new DestinationInfo(destPageIndex, destTop, this.pageInfo.Page, bounds));
		}
		public PdfOutlineEntry SetOutlineEntry(PdfOutlineItem parent, string title, int destPageIndex, float destTop) {
			DestinationInfo destInfo = new DestinationInfo(destPageIndex, destTop);
			this.document.AddDestinationInfo(destInfo);
			PdfOutlineItem parentItem = parent != null ? parent : this.document.Catalog.Outlines;
			PdfOutlineEntry entry = new PdfOutlineEntry(parentItem, title, destInfo, this.document.Compressed);
			parentItem.Entries.Add(entry);
			return entry;
		}
		public void SetNeverEmbeddedFont(Font font) {
			if(font == null)
				throw new ArgumentNullException("font");
			this.neverEmbeddedFonts.RegisterFont(font);
		}
		public void SetNeverEmbeddedFontFamily(string fontFamilyName) {
			this.neverEmbeddedFonts.RegisterFontFamily(fontFamilyName);
		}
		void ApplyExportOptions(PdfExportOptions pdfOptions) {
			document.ConvertImagesToJpeg = pdfOptions.ConvertImagesToJpeg;
			document.JpegImageQuality = pdfOptions.ImageQuality;
			document.Info.Author = pdfOptions.DocumentOptions.Author;
			document.Info.Application = pdfOptions.DocumentOptions.Application;
			document.Info.Title = pdfOptions.DocumentOptions.Title;
			document.Info.Subject = pdfOptions.DocumentOptions.Subject;
			document.Info.Keywords = pdfOptions.DocumentOptions.Keywords;
			document.Catalog.Metadata.Author = pdfOptions.DocumentOptions.Author;
			document.Catalog.Metadata.Application = pdfOptions.DocumentOptions.Application;
			document.Catalog.Metadata.Title = pdfOptions.DocumentOptions.Title;
			document.Catalog.Metadata.Subject = pdfOptions.DocumentOptions.Subject;
			document.Catalog.Metadata.Keywords = pdfOptions.DocumentOptions.Keywords;
			document.Catalog.Metadata.AdditionalMetadata = pdfOptions.AdditionalMetadata;
			document.Catalog.Attachments = pdfOptions.Attachments;
			document.Info.CreationDate = document.Catalog.Metadata.CreationDate = DateTimeHelper.Now;
			string[] neverEmbeddedFonts = pdfOptions.NeverEmbeddedFonts.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach(string fontFamily in neverEmbeddedFonts)
				SetNeverEmbeddedFontFamily(fontFamily);
			PdfPasswordSecurityOptions passwordSecurityOptions = pdfOptions.PasswordSecurityOptions;
			document.Encryption.OpenPassword = passwordSecurityOptions.OpenPassword;
			document.Encryption.PermissionsPassword = passwordSecurityOptions.PermissionsPassword;
			if(!string.IsNullOrEmpty(passwordSecurityOptions.PermissionsPassword)) {
				document.Encryption.PrintingPermissions = passwordSecurityOptions.PermissionsOptions.PrintingPermissions;
				document.Encryption.ChangingPermissions = passwordSecurityOptions.PermissionsOptions.ChangingPermissions;
				document.Encryption.EnableCoping = passwordSecurityOptions.PermissionsOptions.EnableCopying;
				document.Encryption.EnableScreenReaders = passwordSecurityOptions.PermissionsOptions.EnableScreenReaders;
			}
			document.Signature.Certificate = pdfOptions.SignatureOptions.Certificate;
			document.Signature.Reason = pdfOptions.SignatureOptions.Reason;
			document.Signature.Location = pdfOptions.SignatureOptions.Location;
			document.Signature.ContactInfo = pdfOptions.SignatureOptions.ContactInfo;
			document.PdfACompatible = pdfOptions.PdfACompatible && !document.Encryption.IsEncryptionOn;
		}
		void TestPage() {
			if(this.pageInfo == null)
				throw new PdfGraphicsException("The current page undefined");
		}
		void ClosePage() {
			if(this.pageInfo != null) {
				if(pdfGraphicsImpl != null)
					pdfGraphicsImpl.Finish();
				this.pageInfo.WriteAndClose(this.writer);
				this.pageInfo = null;
				ProgressReflector.RangeValue++;
			}
		}
		void OpenPage(SizeF pageSize) {
			this.pageInfo = new PdfPageInfo(GetValidPageSize(PdfCoordinate.TransformValue(pageUnit, pageSize)), this.document, this.pdfHashtable);
		}
		static SizeF GetValidPageSize(SizeF pageSizeInPoints) {
			const float maxPageSize = 14400f; 
			return new SizeF(Math.Min(pageSizeInPoints.Width, maxPageSize), Math.Min(pageSizeInPoints.Height, maxPageSize));
		}
		void OpenDefaultPageIfNeeded() {
			if(this.pageInfo == null)
				OpenPage(new SizeF(0, 0));
		}
		void WriteDocument() {
			OpenDefaultPageIfNeeded();
			ClosePage();
			ValidateDestinations();
			document.BeforeWrite();
			document.Write(writer, ProgressReflector);
			writer.Flush();
			document.AfterWrite();
		}
		void ValidateDestinations() {
			foreach(DestinationInfo item in document.DestinationInfos) {
				PdfPage page = document.GetPage(item.DestPageIndex);
				if(page != null) {
					item.DestTop = PdfCoordinate.CorrectPoint(pageUnit, new PointF(0, item.DestTop), page.MediaBox.Size).Y;
				}
				if(item.LinkPage != null)
					item.LinkArea = PdfCoordinate.CorrectRectangle(pageUnit, item.LinkArea, item.LinkPage.MediaBox.Size);
			}
		}
		#region IDisposable implementation
		void IDisposable.Dispose() {
			Flush();
		}
		#endregion
		#region IGraphics implementation
		public float Dpi {
			get { return GraphicsDpi.Point; }
		}
		public RectangleF ClipBounds {
			get { return pdfGraphicsImpl.ClipBounds; }
			set { pdfGraphicsImpl.ClipBounds = value; }
		}
		public Region Clip { get { return null; } set { } }
		public GraphicsUnit PageUnit {
			get { return pageUnit; }
			set { 
				pageUnit = value;
				if(pdfGraphicsImpl != null)
					pdfGraphicsImpl.PageUnit = pageUnit;
			}
		}
		public Matrix Transform { get { return null; } set { } }
		public SmoothingMode SmoothingMode { get { return smoothingMode; } set { smoothingMode = value; } }
		public void FillRectangle(Brush brush, RectangleF bounds) {
			TestPage();
			pdfGraphicsImpl.FillRectangle(brush, bounds);
		}
		public void FillRectangle(Brush brush, float x, float y, float width, float height) {
			FillRectangle(brush, new RectangleF(x, y, width, height));
		}
		public void DrawString(string s, Font font, Brush brush, RectangleF bounds, StringFormat format) {
			pdfGraphicsImpl.DrawString(s, font, brush, bounds, format);
		}
		public void DrawString(string s, Font font, Brush brush, RectangleF bounds) {
			DrawString(s, font, brush, bounds, null);
		}
		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format) {
			pdfGraphicsImpl.DrawString(s, font, brush, point, format);
		}
		public void DrawString(string s, Font font, Brush brush, PointF point) {
			DrawString(s, font, brush, point, null);
		}
		public void DrawLine(Pen pen, PointF pt1, PointF pt2) {
			TestPage();
			pdfGraphicsImpl.DrawLine(pen, pt1, pt2);
		}
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
		}
		public void DrawLines(Pen pen, PointF[] points) {
			TestPage();
			pdfGraphicsImpl.DrawLines(pen, points);
		}
		public void DrawImage(Image image, Point point) {
			DrawImage(image, new RectangleF(point, image.Size));
		}
		public void DrawImage(Image image, RectangleF bounds) {
			DrawImage(image, bounds, this.imageBackColor);
		}
		public void DrawImage(Image image, RectangleF bounds, Color underlyingColor) {
			TestPage();
			pdfGraphicsImpl.DrawImage(image, bounds, underlyingColor);
		}
		public void DrawCheckBox(RectangleF rect, CheckState state) {
			DrawImage(Native.CheckBoxImageHelper.GetCheckBoxImage(state), rect);
		}
		public void DrawRectangle(Pen pen, RectangleF bounds) {
			TestPage();
			pdfGraphicsImpl.DrawRectangle(pen, bounds);
		}
		public void DrawPath(Pen pen, GraphicsPath path) {
			TestPage();
			pdfGraphicsImpl.DrawPath(pen, path);
		}
		public void FillPath(Brush brush, GraphicsPath path) {
			TestPage();
			pdfGraphicsImpl.FillPath(brush, path);
		}
		public void DrawEllipse(Pen pen, RectangleF rect) {
			TestPage();
			pdfGraphicsImpl.DrawEllipse(pen, rect);
		}
		public void DrawEllipse(Pen pen, float x, float y, float width, float height) {
			DrawEllipse(pen, new RectangleF(x, y, width, height));
		}
		public void FillEllipse(Brush brush, RectangleF rect) {
			TestPage();
			pdfGraphicsImpl.FillEllipse(brush, rect);
		}
		public void FillEllipse(Brush brush, float x, float y, float width, float height) {
			FillEllipse(brush, new RectangleF(x, y, width, height));
		}
		public SizeF MeasureString(string text, Font font, GraphicsUnit graphicsUnit) {
			return Measurer.MeasureString(text, font, graphicsUnit);
		}
		public SizeF MeasureString(string text, Font font, float width, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			return Measurer.MeasureString(text, font, width, stringFormat, graphicsUnit);
		}
		public SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			return Measurer.MeasureString(text, font, size, stringFormat, graphicsUnit);
		}
		public SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit graphicsUnit) {
			return Measurer.MeasureString(text, font, location, stringFormat, graphicsUnit);
		}
		public void ResetTransform() {
			pdfGraphicsImpl.ResetTransform();
		}
		public void MultiplyTransform(Matrix matrix) {
		}
		public void MultiplyTransform(Matrix matrix, MatrixOrder order) {
		}
		public void RotateTransform(float angle) {
			RotateTransform(angle, MatrixOrder.Prepend);
		}
		public void RotateTransform(float angle, MatrixOrder order) {
			pdfGraphicsImpl.RotateTransform(angle, order);
		}
		public void ScaleTransform(float sx, float sy) {
			ScaleTransform(sx, sy, MatrixOrder.Prepend);
		}
		public void ScaleTransform(float sx, float sy, MatrixOrder order) {
			pdfGraphicsImpl.ScaleTransform(sx, sy, order);
		}
		public void TranslateTransform(float dx, float dy) {
			TranslateTransform(dx, dy, MatrixOrder.Prepend);
		}
		public void TranslateTransform(float dx, float dy, MatrixOrder order) {
			pdfGraphicsImpl.TranslateTransform(dx, dy, order);
		}
		public void SaveTransformState() {
			pdfGraphicsImpl.SaveTransformState();
		}
		public void ApplyTransformState(MatrixOrder order, bool removeState) {
			pdfGraphicsImpl.ApplyTransformState(order, removeState);
		}
		#endregion
	}
	public interface IPdfDocumentOwner {
		PdfNeverEmbeddedFonts NeverEmbeddedFonts { get; }
		PdfImageCache ImageCache { get; }
		Measurer Measurer { get; }
		Measurer MetafileMeasurer { get; }
		bool ScaleStrings { get; }
	}
	public interface IXObjectsOwner {
		void AddNewXObject(PdfXObject xObject);
		void AddExistingXObject(PdfXObject xObject);
		void AddTransparencyGS(PdfTransparencyGS gs);
		void AddShading(PdfShading shading);
		void AddPattern(PdfPattern pattern); 
	}
	public class PdfPageInfo : IXObjectsOwner {
		PdfDocument document;
		SizeF sizeInPoints;
		PdfPage page;
		PdfContents contents;
		PdfXObjects xObjects = new PdfXObjects();
		public PdfPageInfo(SizeF sizeInPoints, PdfDocument document, PdfHashtable pdfHashtable) {
			this.sizeInPoints = sizeInPoints;
			this.document = document;
			this.page = this.document.Catalog.Pages.CreatePage();
			this.page.MediaBox = new PdfRectangle(0, 0, (int)Math.Round(this.sizeInPoints.Width, MidpointRounding.AwayFromZero) , (int)Math.Round(this.sizeInPoints.Height, MidpointRounding.AwayFromZero));
			this.contents = new PdfContents(this.page, this.document.Compressed, pdfHashtable);
			this.contents.Register(this.document.XRef);
			this.page.InitializeContents(this.contents);
		}
		public SizeF SizeInPoints { get { return sizeInPoints; } }
		public PdfPage Page { get { return page; } }
		public PdfDrawContext Context { get { return contents.DrawContext; } }
		public void AddNewXObject(PdfXObject xObject) {
			this.xObjects.Add(xObject);
			this.page.XObjects.Add(xObject);
			xObject.Register(this.document.XRef);
		}
		public void AddExistingXObject(PdfXObject xObject) {
			this.page.XObjects.AddUnique(xObject);
		}
		public void WriteAndClose(StreamWriter writer) {
			this.contents.FillUp();
			this.contents.Write(writer);
			this.contents.Close();
			this.xObjects.FillUp();
			this.xObjects.Write(writer);
			this.xObjects.CloseAndClear();
		}
		void IXObjectsOwner.AddTransparencyGS(PdfTransparencyGS gs) {
			page.AddTransparencyGS(gs);
		}
		void IXObjectsOwner.AddShading(PdfShading shading) {
			page.AddShading(shading);
		}
		void IXObjectsOwner.AddPattern(PdfPattern pattern) {
			page.AddPattern(pattern);
		}
	}
	public class PdfNeverEmbeddedFonts {
		ArrayList list = new ArrayList();
		bool Find(string fontName) {
			foreach(string name in this.list)
				if(name == fontName)
					return true;
			return false;
		}
		void AddFontInfo(string fontName) {
			if(!Find(fontName))
				this.list.Add(fontName);
		}
		public void RegisterFont(Font font) {
			AddFontInfo(PdfFontUtils.GetFontName(font));
		}
		public void RegisterFontFamily(string familyName) {
			AddFontInfo(PdfFontUtils.GetFontName(familyName, false, false));
			AddFontInfo(PdfFontUtils.GetFontName(familyName, true, false));
			AddFontInfo(PdfFontUtils.GetFontName(familyName, false, true));
			AddFontInfo(PdfFontUtils.GetFontName(familyName, true, true));
		}
		public bool FindFont(Font font) {
			return Find(PdfFontUtils.GetFontName(font));
		}
	}
	public class PdfImageCache {
		#region inner classes
		public class Params {
			Image image;
			Color backgroundColor;
			public Params(Image image, Color backgroundColor) {
				if(image == null)
					throw new ArgumentNullException("image");
				this.image = image;
				this.backgroundColor = backgroundColor;
			}
			public override bool Equals(object obj) {
				Params imageParams = obj as Params;
				return
					imageParams != null &&
					this.image == imageParams.image &&
					this.backgroundColor == imageParams.backgroundColor;
			}
			public override int GetHashCode() {
				return
					this.image.GetHashCode() ^
					this.backgroundColor.GetHashCode();
			}
		}
		#endregion
		Hashtable hash = new Hashtable();
		public PdfImageBase this[Params imageParams] { get { return hash[imageParams] as PdfImageBase; } }
		public void AddPdfImage(PdfImageBase pdfImage, Params imageParams) {
			this.hash.Add(imageParams, pdfImage);
		}
	}
	public class PdfGraphicsException : Exception {
		public PdfGraphicsException() {
		}
		public PdfGraphicsException(string message)
			: base(message) {
		}
		public PdfGraphicsException(string message, Exception innerEx)
			: base(message, innerEx) {
		}
	}
}
