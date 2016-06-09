#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfDocumentProcessor : PdfDisposableObject {
		readonly PdfFontStorage fontStorage = new PdfFontStorage();
		PdfEditableFontDataCache fontCache;
		PdfImageCache imageCache;
		long imageCacheSize = 300;
		int passwordAttemptsLimit = 1;
		int maxPrintingDpi;
		PdfDocument document;
		PdfDocumentState documentState;
		PdfDataSelector dataSelector;
		PdfTextSearch documentTextSearch;
		string documentFilePath = String.Empty;
		string documentText = null;
		Stream documentStream;
		PdfWordIterator wordIterator;
#if !SL
	[DevExpressDocsLocalizedDescription("PdfDocumentProcessorImageCacheSize")]
#endif
		public long ImageCacheSize {
			get { return imageCacheSize; }
			set {
				imageCacheSize = value;
				if (documentState != null)
					documentState.ImageDataStorage.UpdateLimit(value);
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("PdfDocumentProcessorPasswordAttemptsLimit")]
#endif
		public int PasswordAttemptsLimit {
			get { return passwordAttemptsLimit; }
			set { passwordAttemptsLimit = value < 1 ? 1 : value; }
		}
#if !SL
	[DevExpressDocsLocalizedDescription("PdfDocumentProcessorMaxPrintingDpi")]
#endif
		public int MaxPrintingDpi {
			get { return maxPrintingDpi; }
			set { maxPrintingDpi = value; }
		}
#if !SL
	[DevExpressDocsLocalizedDescription("PdfDocumentProcessorDocument")]
#endif
		public PdfDocument Document { get { return document; } }
#if !SL
	[DevExpressDocsLocalizedDescription("PdfDocumentProcessorText")]
#endif
		public string Text {
			get {
				if (documentText != null)
					return documentText;
				if (document == null)
					return String.Empty;
				List<PdfPageTextRange> textRange = new List<PdfPageTextRange>();
				for (int i = 0; i < document.Pages.Count; i++)
					textRange.Add(new PdfPageTextRange(i));
				documentText = new PdfTextSelection(new PdfPageDataCache(document.Pages, false), textRange).Text;
				return documentText;
			}
		}
		PdfDataSelector DataSelector {
			get {
				if (dataSelector == null && DocumentState != null)
					dataSelector = new PdfDataSelector(null, documentState);
				return dataSelector;
			}
		}
		PdfDocumentState DocumentState {
			get {
				if (documentState == null && document != null)
					documentState = new PdfDocumentState(document, fontStorage, imageCacheSize);
				return documentState;
			}
		}
#if !SL
	[DevExpressDocsLocalizedDescription("PdfDocumentProcessorPasswordRequested")]
#endif
		public event PdfPasswordRequestedEventHandler PasswordRequested;
		void CheckOperationAvailability() {
			if (document == null)
				throw new InvalidOperationException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnavailableOperation));
		}
		SecureString ProvidePassword(int n) {
			if (n > passwordAttemptsLimit)
				return null;
			PdfPasswordRequestedEventArgs args = new PdfPasswordRequestedEventArgs(documentFilePath, n);
			if (PasswordRequested != null)
				PasswordRequested(this, args);
			return args.Password;
		}
		void FinalizeDocument() {
			if (document != null) {
				fontCache.UpdateFonts();
				document.FinalizeDocument();
			}
		}
		void DisposeDocument() {
			FinalizeDocument();
			fontStorage.Clear();
			if (documentState != null) {
				documentState.Dispose();
				documentState = null;
			}
			documentText = null;
			dataSelector = null;
			documentTextSearch = null;
			ClearDocumentStream();
		}
		void SetDocument(PdfDocument doc) {
			DisposeDocument();
			document = doc;
			documentTextSearch = new PdfTextSearch(document.Pages);
			PdfObjectCollection objects = document.DocumentCatalog.Objects;
			fontCache = new PdfEditableFontDataCache(objects);
			imageCache = new PdfImageCache(objects);
			wordIterator = new PdfWordIterator(document);
		}
		void ClearDocumentStream() {
			if (documentStream != null) {
				if (!String.IsNullOrEmpty(documentFilePath))
					documentStream.Dispose();
				documentStream = null;
			}
		}
		void ResetTextPosition() {
			documentText = null;
			wordIterator.Reset();
		}
		PdfDestination CreateDestination(int pageNumber, float x, float y, float dpiX, float dpiY, float? zoomFactor) {
			CheckOperationAvailability();
			PdfGraphics.CheckDpiValues(dpiX, dpiY);
			IList<PdfPage> pages = document.Pages;
			int pageCount = pages.Count;
			if (pageNumber < 1 || pageNumber > pageCount)
				throw new ArgumentOutOfRangeException("pageNumber", String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectPageNumber), pageCount));
			return PdfDestination.Create(pages[pageNumber - 1], x, y, dpiX, dpiY, zoomFactor);
		}
		public void LoadDocument(Stream stream, bool detachStreamAfterLoadComplete) {
			SetDocument(PdfDocumentReader.Read(stream, detachStreamAfterLoadComplete, (n) => ProvidePassword(n)));
		}
		public void LoadDocument(Stream stream) {
			LoadDocument(stream, false);
		}
		public void LoadDocument(string path, bool detachStreamAfterLoadComplete) {
			FileStream stream = null;
			try {
				stream = new FileStream(path, FileMode.Open, FileAccess.Read);
				LoadDocument(stream, detachStreamAfterLoadComplete);
				documentStream = stream;
				this.documentFilePath = path;
			}
			catch {
				if (!detachStreamAfterLoadComplete && stream != null)
					stream.Dispose();
				throw;
			}
			finally {
				if (stream != null && detachStreamAfterLoadComplete)
					stream.Dispose();
			}
		}
		public void LoadDocument(string path) {
			LoadDocument(path, false);
		}
		public void SaveDocument(Stream stream, PdfSaveOptions options, bool detachStream) {
			CheckOperationAvailability();
			FinalizeDocument();
			PdfObjectCollection objectCollection = document.DocumentCatalog.Objects;
			bool isStreamDetached = objectCollection.IsStreamDetached;
			objectCollection.IsStreamDetached = detachStream;
			PdfObjectCollection objects = PdfDocumentWriter.Write(stream, document, options);
			ClearDocumentStream();
			if (detachStream)
				objects.ResolveAllSlots();
			else
				documentStream = stream;
			documentFilePath = String.Empty;
			if (detachStream && isStreamDetached)
				return;
			document.UpdateObjects(objects);
		}
		public void SaveDocument(Stream stream, bool detachStream) {
			SaveDocument(stream, new PdfSaveOptions(), detachStream);
		}
		public void SaveDocument(Stream stream, PdfSaveOptions options) {
			CheckOperationAvailability();
			SaveDocument(stream, options, document.DocumentCatalog.Objects.IsStreamDetached);
		}
		public void SaveDocument(Stream stream) {
			SaveDocument(stream, new PdfSaveOptions());
		}
		public void SaveDocument(string path, PdfSaveOptions options, bool detachStream) {
			CheckOperationAvailability();
			FinalizeDocument();
			PdfObjectCollection objectCollection = document.DocumentCatalog.Objects;
			bool isStreamDetached = objectCollection.IsStreamDetached;
			bool useTempFile = File.Exists(path);
			string tmpFilePath = null;
			try {
				PdfObjectCollection savedObjects = null;
				Stream stream = null;
				if (useTempFile) {
					tmpFilePath = Path.GetTempFileName();
					new FileInfo(tmpFilePath).Attributes = FileAttributes.Temporary;
					stream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.ReadWrite);
				}
				else
					stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
				objectCollection.IsStreamDetached = detachStream;
				using (stream) {
					savedObjects = PdfDocumentWriter.Write(stream, document, options);
					if (detachStream)
						savedObjects.ResolveAllSlots();
				}
				ClearDocumentStream();
				if (useTempFile)
					File.Copy(tmpFilePath, path, true);
				if (!detachStream) {
					documentStream = new FileStream(path, FileMode.Open, FileAccess.Read);
					savedObjects.UpdateStream(documentStream);
				}
				documentFilePath = path;
				if (detachStream && isStreamDetached)
					return;
				document.UpdateObjects(savedObjects);
			}
			finally {
				if (useTempFile)
					try {
						File.Delete(tmpFilePath);
					}
					catch {
					}
			}
		}
		public void SaveDocument(string path, PdfSaveOptions options) {
			CheckOperationAvailability();
			SaveDocument(path, options, document.DocumentCatalog.Objects.IsStreamDetached);
		}
		public void SaveDocument(string path, bool detachStream) {
			SaveDocument(path, new PdfSaveOptions(), detachStream);
		}
		public void SaveDocument(string path) {
			SaveDocument(path, new PdfSaveOptions());
		}
		public void CloseDocument() {
			DisposeDocument();
			documentFilePath = String.Empty;
			document = null;
			wordIterator = null;
		}
		public void CreateEmptyDocument() {
			SetDocument(new PdfDocument());
		}
		public void CreateEmptyDocument(string path) {
			CreateEmptyDocument(path, new PdfSaveOptions(), new PdfCreationOptions());
		}
		public void CreateEmptyDocument(string path, PdfCreationOptions creationOptions) {
			CreateEmptyDocument(path, new PdfSaveOptions(), creationOptions);
		}
		public void CreateEmptyDocument(string path, PdfSaveOptions saveOptions) {
			CreateEmptyDocument(path, saveOptions, new PdfCreationOptions());
		}
		public void CreateEmptyDocument(string path, PdfSaveOptions saveOptions, PdfCreationOptions creationOptions) {
			Stream stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
			CreateEmptyDocument(stream, saveOptions, creationOptions);
			documentFilePath = path;
			documentStream = stream;
		}
		public void CreateEmptyDocument(Stream stream) {
			CreateEmptyDocument(stream, new PdfSaveOptions(), new PdfCreationOptions());
		}
		public void CreateEmptyDocument(Stream stream, PdfCreationOptions creationOptions) {
			CreateEmptyDocument(stream, new PdfSaveOptions(), creationOptions);
		}
		public void CreateEmptyDocument(Stream stream, PdfSaveOptions saveOptions) {
			CreateEmptyDocument(stream, saveOptions, new PdfCreationOptions());
		}
		public void CreateEmptyDocument(Stream stream, PdfSaveOptions saveOptions, PdfCreationOptions creationOptions) {
			SetDocument(new PdfDocument(stream, creationOptions, saveOptions));
		}
		public void AppendDocument(Stream stream) {
			if (document == null)
				LoadDocument(stream, true);
			else {
				if (documentState != null) {
					documentState.FontStorage.Clear();
					documentState.ImageDataStorage.Clear();
				}
				document.Append(PdfDocumentReader.Read(stream, false, (n) => ProvidePassword(n)));
				ResetTextPosition();
			}
		}
		public void AppendDocument(string path) {
			if (String.IsNullOrEmpty(documentFilePath))
				documentFilePath = path;
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				AppendDocument(stream);
		}
		public PdfPage AddNewPage(PdfRectangle mediaBox) {
			CheckOperationAvailability();
			PdfPage page = document.AddPage(mediaBox);
			ResetTextPosition();
			return page;
		}
		public PdfPage InsertNewPage(int pageNumber, PdfRectangle mediaBox) {
			CheckOperationAvailability();
			PdfPage page = document.InsertPage(pageNumber, mediaBox);
			ResetTextPosition();
			return page;
		}
		public void DeletePage(int pageNumber) {
			CheckOperationAvailability();
			document.DeletePage(pageNumber);
			ResetTextPosition();
		}
		public void Export(Stream stream, PdfFormDataFormat format) {
			PdfFormData formData = GetFormData();
			if (formData == null)
				throw new InvalidOperationException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgUnavailableOperation));
			formData.Save(stream, format);
		}
		public void Export(string fileName, PdfFormDataFormat format) {
			using (Stream stream = File.Create(fileName))
				Export(stream, format);
		}
		public void Import(Stream stream, PdfFormDataFormat format) {
			ApplyFormData(new PdfFormData(stream, format));
		}
		public void Import(string fileName, PdfFormDataFormat format) {
			ApplyFormData(new PdfFormData(fileName, format));
		}
		public void Import(Stream stream) {
			ApplyFormData(new PdfFormData(stream));
		}
		public void Import(string fileName) {
			ApplyFormData(new PdfFormData(fileName));
		}
		public PdfFormData GetFormData() {
			return DocumentState != null ? documentState.FormData : null;
		}
		public void ResetFormData() {
			if (DocumentState != null && documentState.FormData != null)
				documentState.FormData.Reset();
		}
		public void ApplyFormData(PdfFormData data) {
			if (DocumentState == null || documentState.FormData == null)
				return;
			documentState.FormData.Apply(data);
		}
		public void AttachFile(PdfFileAttachment attachment) {
			CheckOperationAvailability();
			if (attachment == null)
				throw new ArgumentNullException("attachment");
			document.AttachFile(attachment);
		}
		public bool DeleteAttachment(PdfFileAttachment attachment) {
			CheckOperationAvailability();
			return document.DeleteAttachment(attachment);
		}
		public PdfDestination CreateDestination(int pageNumber, float x, float y, float dpiX, float dpiY) {
			return CreateDestination(pageNumber, x, y, dpiX, dpiY, null);
		}
		public PdfDestination CreateDestination(int pageNumber, float x, float y) {
			return CreateDestination(pageNumber, x, y, PdfGraphics.DefaultDpi, PdfGraphics.DefaultDpi);
		}
		public PdfDestination CreateDestination(int pageNumber) {
			return CreateDestination(pageNumber, 0, 0);
		}
		public PdfDestination CreateDestination(int pageNumber, float x, float y, float dpiX, float dpiY, float zoomFactor) {
			return CreateDestination(pageNumber, x, y, dpiX, dpiY, (float?)zoomFactor);
		}
		public PdfDestination CreateDestination(int pageNumber, float x, float y, float zoomFactor) {
			return CreateDestination(pageNumber, x, y, PdfGraphics.DefaultDpi, PdfGraphics.DefaultDpi, zoomFactor);
		}
		public PdfDestination CreateDestination(int pageNumber, float zoomFactor) {
			return CreateDestination(pageNumber, 0, 0, zoomFactor);
		}
		public PdfGraphics CreateGraphics() {
			CheckOperationAvailability();
			return new PdfGraphics(imageCache, fontCache);
		}
		public int RenderNewPage(PdfRectangle mediaBox, PdfGraphics graphics) {
			return RenderNewPage(mediaBox, graphics, PdfGraphics.DefaultDpi, PdfGraphics.DefaultDpi);
		}
		public int RenderNewPage(PdfRectangle mediaBox, PdfGraphics graphics, float dpiX, float dpiY) {
			CheckOperationAvailability();
			PdfGraphics.CheckDpiValues(dpiX, dpiY);
			if (mediaBox == null)
				throw new ArgumentNullException("mediaBox");
			if (graphics == null)
				throw new ArgumentNullException("graphics");
			graphics.AddToPage(AddNewPage(mediaBox), dpiX, dpiY);
			return document.Pages.Count;
		}
		public void Print(PdfPrinterSettings pdfPrinterSettings) {
			CheckOperationAvailability();
			if (pdfPrinterSettings == null)
				pdfPrinterSettings = new PdfPrinterSettings();
			if (maxPrintingDpi != 0 && maxPrintingDpi < pdfPrinterSettings.PrintingDpi)
				pdfPrinterSettings.PrintingDpi = maxPrintingDpi;
			if (document != null && document.Pages.Count > 0)
				PdfDocumentPrinter.Print(DocumentState, documentFilePath, 1, pdfPrinterSettings, false);
		}
		[Obsolete("Use the Print(PdfPrinterSettings pdfPrinterSettings) overload of this method instead.")]
		public void Print(PrinterSettings printerSettings) {
			Print(new PdfPrinterSettings(printerSettings));
		}
		public Bitmap CreateBitmap(int pageNumber, int largestEdgeLength) {
			CheckOperationAvailability();
			if (pageNumber < 1 || pageNumber > document.Pages.Count)
				throw new ArgumentOutOfRangeException("pageNumber", String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectPageNumber)));
			if (largestEdgeLength < 1)
				throw new ArgumentOutOfRangeException("largestEdgeLength", String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectLargestEdgeLength)));
			return PdfViewerCommandInterpreter.GetBitmap(DocumentState, pageNumber - 1, largestEdgeLength, PdfRenderMode.Print);
		}
		public string GetPageText(int pageNumber) {
			CheckOperationAvailability();
			PdfPageDataCache pageDataCache = new PdfPageDataCache(document.Pages, false);
			IList<PdfPage> pages = document.Pages;
			if (pageNumber > pages.Count || pageNumber < 1)
				return String.Empty;
			return new PdfTextSelection(pageDataCache, new List<PdfPageTextRange>() { new PdfPageTextRange(pageNumber - 1) }).Text;
		}
		public string GetText(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition) {
			if (startPosition == null)
				throw new ArgumentNullException("startPosition");
			if (endPosition == null)
				throw new ArgumentNullException("endPosition");
			CheckOperationAvailability();
			PdfTextSelection textSelection = DataSelector.GetTextSelection(startPosition, endPosition);
			return textSelection == null ? String.Empty : textSelection.Text;
		}
		public string GetText(PdfDocumentArea area) {
			if (area == null)
				throw new ArgumentNullException("area");
			CheckOperationAvailability();
			PdfTextSelection textSelection = DataSelector.GetTextSelection(area);
			return textSelection == null ? String.Empty : textSelection.Text;
		}
		public IList<Bitmap> GetImages(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition, float imageResolution) {
			if (startPosition == null)
				throw new ArgumentNullException("startPosition");
			if (endPosition == null)
				throw new ArgumentNullException("endPosition");
			PdfGraphics.CheckDpiValue(imageResolution, "imageResolution");
			CheckOperationAvailability();
			List<Bitmap> result = new List<Bitmap>();
			IList<PdfImageSelection> imagesSelection = DataSelector.GetImagesSelection(startPosition, endPosition);
			if (imagesSelection != null) {
				IList<PdfPage> pages = document.Pages;
				PdfImageDataStorage imageDataStorage = DocumentState.ImageDataStorage;
				foreach (PdfImageSelection selection in imagesSelection) {
					Bitmap bmp = PdfImageSelectionCommandInterpreter.GetSelectionBitmap(pages[selection.Highlights[0].PageIndex], selection, imageDataStorage, 0, imageResolution);
					if (bmp != null)
						result.Add(bmp);
				}
			}
			return result;
		}
		public IList<Bitmap> GetImages(PdfDocumentPosition startPosition, PdfDocumentPosition endPosition) {
			return GetImages(startPosition, endPosition, PdfRenderingCommandInterpreter.DefaultDpi);
		}
		public IList<Bitmap> GetImages(PdfDocumentArea area, float imageResolution) {
			if (area == null)
				throw new ArgumentNullException("area");
			PdfGraphics.CheckDpiValue(imageResolution, "imageResolution");
			CheckOperationAvailability();
			List<Bitmap> result = new List<Bitmap>();
			IList<PdfImageSelection> imagesSelection = DataSelector.GetImagesSelection(area);
			if (imagesSelection != null) {
				IList<PdfPage> pages = document.Pages;
				PdfImageDataStorage imageDataStorage = DocumentState.ImageDataStorage;
				int pageIndex = area.PageNumber - 1;
				PdfRectangle selectionRect = area.Area;
				double selectionLeft = selectionRect.Left;
				double selectionRight = selectionRect.Right;
				double selectionBottom = selectionRect.Bottom;
				double selectionTop = selectionRect.Top;
				foreach (PdfImageSelection selection in imagesSelection) {
					PdfPageImageData pageImageData = selection.PageImageData;
					PdfRectangle imageRectangle = pageImageData.BoundingRectangle;
					PdfRectangle trimmedRect = new PdfRectangle(Math.Max(imageRectangle.Left, selectionLeft),
						Math.Max(imageRectangle.Bottom, selectionBottom), Math.Min(imageRectangle.Right, selectionRight), Math.Min(imageRectangle.Top, selectionTop));
					Bitmap bmp = PdfImageSelectionCommandInterpreter.GetSelectionBitmap(pages[pageIndex], new PdfImageSelection(1, pageImageData, trimmedRect), imageDataStorage, 0, imageResolution);
					if (bmp != null)
						result.Add(bmp);
				}
			}
			return result;
		}
		public IList<Bitmap> GetImages(PdfDocumentArea area) {
			return GetImages(area, PdfRenderingCommandInterpreter.DefaultDpi);
		}
		public PdfTextSearchResults FindText(string text, PdfTextSearchParameters parameters) {
			CheckOperationAvailability();
			if (DocumentState == null)
				return new PdfTextSearchResults(null, 0, null, null, PdfTextSearchStatus.NotFound);
			PdfTextSearchResults results = documentTextSearch.Find(text, parameters ?? new PdfTextSearchParameters(), 1);
			if (results.Status == PdfTextSearchStatus.Found) 
				wordIterator = new PdfWordIterator(document, results.PageNumber - 1, documentTextSearch.WordIndex);
			return results;
		}
		public PdfTextSearchResults FindText(string text) {
			return FindText(text, null);
		}
		public PdfPageWord NextWord() {
			CheckOperationAvailability();
			fontCache.UpdateFonts();
			return wordIterator.Next();
		}
		public PdfPageWord PrevWord() {
			CheckOperationAvailability();
			fontCache.UpdateFonts();
			return wordIterator.Prev();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				DisposeDocument();
				fontStorage.Dispose();
			}
		}
	}
}
