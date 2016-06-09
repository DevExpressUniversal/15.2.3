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
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using DevExpress.Printing;
using DevExpress.Printing.Native.PrintEditor;
using DevExpress.Data.Utils;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf.Drawing {
	public class PdfPrintDialogViewModel : PdfDisposableObject, INotifyPropertyChanged, IDataErrorInfo {
		internal const string PrinterIndexPropertyName = "PrinterIndex";
		internal const string PrinterStatusPropertyName = "PrinterStatus";
		internal const string PrinterLocationPropertyName = "PrinterLocation";
		internal const string PrinterCommentPropertyName = "PrinterComment";
		internal const string PrinterDocumentsInQueuePropertyName = "PrinterDocumentsInQueue";
		internal const string PrintingDpiPropertyName = "PrintingDpi";
		internal const string MaxDpiPropertyName = "MaxDpi";
		internal const string CopiesPropertyName = "Copies";
		internal const string AllowCollatePropertyName = "AllowCollate";
		internal const string CollatePropertyName = "Collate";
		internal const string PrintRangePropertyName = "PrintRange";
		internal const string PageRangeTextPropertyName = "PageRangeText";
		internal const string AllowSomePagesPropertyName = "AllowSomePages";
		internal const string PageNumbersPropertyName = "PageNumbers";
		internal const string EnablePageNumberPreviewPropertyName = "EnablePageNumberPreview";
		internal const string ScaleModePropertyName = "ScaleMode";
		internal const string AllowCustomScalePropertyName = "AllowCustomScale";
		internal const string ScalePropertyName = "Scale";
		internal const string ShowFitScaleTextPropertyName = "ShowFitScaleText";
		internal const string FitScalePropertyName = "FitScale";
		internal const string PageOrientationPropertyName = "PageOrientation";
		internal const string PaperSourcesPropertyName = "PaperSources";
		internal const string PaperSourceIndexPropertyName = "PaperSourceIndex";
		internal const string PrintToFilePropertyName = "PrintToFile";
		internal const string PrintFileNamePropertyName = "PrintFileName";
		internal const string PagePreviewIndexPropertyName = "PagePreviewIndex";
		internal const string CurrentPreviewPageNumberPropertyName = "CurrentPreviewPageNumber";
		internal const string PreviewImagePropertyName = "PreviewImage";
		internal const string PreviewImageBytesPropertyName = "PreviewImageBytes";
		internal const string EnableToPrintPropertyName = "EnableToPrint";
		readonly int currentPageNumber;
		readonly int maxPrintingDpi;
		readonly int documentPageCount;
		readonly Func<string, bool> showFileReplacingRequest;
		readonly IList<PrinterItem> printerItems;
		readonly PdfPrintDialogPreview dialogPreview;
		PdfPrinterSettings pdfPrinterSettings;
		PrinterSettings printerSettings;
		int printerIndex;
		string pageRangeText;
		IList<string> paperSources;
		int paperSourceIndex;
		string printFileName;
		int pagePreviewIndex;
		int currentPreviewPageNumber;
		byte[] previewImageBytes;
		bool hasPrinterException;
		bool enableToPrint = true;
		public event PropertyChangedEventHandler PropertyChanged;
		public IList<PrinterItem> PrinterItems { get { return printerItems; } }
		public PdfPrinterSettings PrinterSettings { get { return pdfPrinterSettings; } }
		public string PrinterStatus { get { return CurrentPrinterItem.Status; } }
		public string PrinterLocation { get { return CurrentPrinterItem.Location; } }
		public string PrinterComment { get { return CurrentPrinterItem.Comment; } }
		public string PrinterDocumentsInQueue { get { return CurrentPrinterItem.PrinterDocumentsInQueue; } }
		public int PrinterIndex {
			get { return printerIndex; }
			set { SetProperty(ref printerIndex, value, PrinterIndexPropertyName, () => OnPrinterIndexChanged()); }
		}
		public int MaxDpi {
			get {
				int maxDpi = pdfPrinterSettings.PrinterMaxDpi;
				return (maxPrintingDpi > 0 && maxPrintingDpi < maxDpi) ? maxPrintingDpi : maxDpi;
			}
		}
		public int PrintingDpi {
			get {
				try {
					return pdfPrinterSettings.PrintingDpi;
				}
				catch {
					hasPrinterException = true;
					return 600;
				}
			}
			set {
				if (value <= 0)
					throw new ArgumentOutOfRangeException();
				value = Math.Min(value, MaxDpi);
				if (value != PrintingDpi)
					try {
						pdfPrinterSettings.PrintingDpi = value;
						RaisePropertyChanged(PrintingDpiPropertyName);
					}
					catch {
					}
			}
		}
		public short Copies {
			get { return printerSettings.Copies; }
			set {
				value = Math.Max(value, (short)1);
				if (value != Copies) {
					printerSettings.Copies = value;
					RaiseCopiesPropertyChanged();
				}
			}
		}
		public bool AllowCollate { get { return Copies > 1; } }
		public bool Collate {
			get { return printerSettings.Collate; }
			set {
				if (value != Collate) {
					printerSettings.Collate = value;
					RaisePropertyChanged(CollatePropertyName);
				}
			}
		}
		public PrintRange PrintRange {
			get { return printerSettings.PrintRange; }
			set { SetPrintRange(value); }
		}
		public bool AllowSomePages {
			get { return PrintRange == PrintRange.SomePages; }
			set {
				if (value && PrintRange != PrintRange.SomePages)
					SetPrintRange(PrintRange.SomePages);
			}
		}
		public string PageRangeText {
			get { return pageRangeText; }
			set { SetProperty(ref pageRangeText, value, PageRangeTextPropertyName, () => UpdatePreview()); }
		}
		public PdfPrintScaleMode ScaleMode {
			get { return pdfPrinterSettings.ScaleMode; }
			set {
				PdfPrintScaleMode scaleMode = ScaleMode;
				if (value != scaleMode) {
					bool shouldRaiseAllowCustomScale = scaleMode == PdfPrintScaleMode.CustomScale || value == PdfPrintScaleMode.CustomScale;
					pdfPrinterSettings.ScaleMode = value;
					RaisePropertyChanged(ScaleModePropertyName);
					if (shouldRaiseAllowCustomScale)
						RaisePropertyChanged(AllowCustomScalePropertyName);
					RefreshPreviewImage();
				}
			}
		}
		public bool ShowFitScaleText { get { return ScaleMode == PdfPrintScaleMode.Fit && EnablePageNumberPreview; } }
		public float FitScale { get { return dialogPreview.Scale; } }
		public bool AllowCustomScale { get { return ScaleMode == PdfPrintScaleMode.CustomScale; } }
		public float Scale {
			get { return pdfPrinterSettings.Scale; }
			set {
				float scale = Scale;
				if (value != scale) {
					pdfPrinterSettings.Scale = value;
					if (Scale != scale)
						RefreshPreviewImage();
					RaisePropertyChanged(ScalePropertyName);
				}
			}
		}
		public PdfPrintPageOrientation PageOrientation {
			get { return pdfPrinterSettings.PageOrientation; }
			set {
				if (pdfPrinterSettings.PageOrientation != value) {
					pdfPrinterSettings.PageOrientation = value;
					RaisePropertyChanged(PageOrientationPropertyName);
					RefreshPreviewImage();
				}
			}
		}
		public IList<string> PaperSources { get { return paperSources; } }
		public int PaperSourceIndex {
			get { return paperSourceIndex; }
			set { SetProperty(ref paperSourceIndex, value, PaperSourceIndexPropertyName, () => {}); }
		}
		public string PaperSource {
			get { 
				PrinterSettings.PaperSourceCollection paperSources = printerSettings.PaperSources;
				return paperSourceIndex < paperSources.Count ? paperSources[paperSourceIndex].SourceName : null; 
			}
			set {
				if (value != PaperSource) {
					PrinterSettings.PaperSourceCollection paperSources = printerSettings.PaperSources;
					int count = paperSources.Count;
					for (int i = 0; i < count; i++)
						if (value == paperSources[i].SourceName) {
							paperSourceIndex = i;
							RaisePropertyChanged(PaperSourceIndexPropertyName);
						}
				}
			}
		}
		public string DefaultPaperSourceName {
			get {
				try {
					return printerSettings.DefaultPageSettings.PaperSource.SourceName;
				}
				catch {
					return String.Empty;
				}
			}
		}
		public bool PrintToFile {
			get { return printerSettings.PrintToFile; }
			set {
				if (value != PrintToFile) {
					printerSettings.PrintToFile = value;
					RaisePropertyChanged(PrintToFilePropertyName);
					UpdatePrinterSettingsPrintFileName();
					RaisePropertyChanged(PrintFileNamePropertyName);
				}
			}
		}
		public string PrintFileName {
			get { return printFileName; }
			set {
				if (value != printFileName && PrintToFile) {
					string message;
					if (PrintEditorController.ValidateFilePath(value, out message)) {
						if (String.IsNullOrEmpty(message) || showFileReplacingRequest(message)) {
							printFileName = value;
							UpdatePrinterSettingsPrintFileName();
							RaisePropertyChanged(PrintFileNamePropertyName);
						}
					}
					else {
						printFileName = value;
						RaisePropertyChanged(PrintFileNamePropertyName);
					}
				}
			}
		}
		public int PagePreviewIndex {
			get { return pagePreviewIndex; }
			set {
				if (value < PageCount && value >= 0)
					SetProperty(ref pagePreviewIndex, value, PagePreviewIndexPropertyName, () => SetCurrentPreviewPage());
			}
		}
		public int CurrentPreviewPageNumber {
			get { return currentPreviewPageNumber; }
			set { PagePreviewIndex = value - 1; }
		}
		public int[] PageNumbers { get { return pdfPrinterSettings.PageNumbers; } }
		public int PageCount {
			get {
				int[] pageNumbers = PageNumbers;
				return pageNumbers == null ? 0 : pageNumbers.Length;
			}
		}
		public bool EnablePageNumberPreview { get { return (PrintRange != PrintRange.SomePages || (PageCount != 0 && !String.IsNullOrEmpty(pageRangeText))); } }
		public Image PreviewImage { get { return dialogPreview.PreviewImage; } }
		public byte[] PreviewImageBytes {
			get {
				if (previewImageBytes == null)
					previewImageBytes = new ImageTool().ToArray(PreviewImage);
				return previewImageBytes;
			}
		}
		public Color PreviewPrintAreaBorderColor {
			get { return dialogPreview.PreviewPrintAreaBorderColor; }
			set { dialogPreview.PreviewPrintAreaBorderColor = value; } 
		}
		public bool HasException { get { return dialogPreview.HasException || hasPrinterException; } }
		public bool EnableToPrint {
			get { return enableToPrint && !HasException && EnablePageNumberPreview && IsValidPrintToFile; }
			set { enableToPrint = value; }
		}
		PrinterItem CurrentPrinterItem { get { return printerItems[printerIndex]; } }
		int CurrentPageNumber { get { return PageCount > 0 ? pdfPrinterSettings.PageNumbers[pagePreviewIndex] : 0; } }
		string InvalidPageRangeMessage { get { return EnablePageNumberPreview ? String.Empty : String.Format(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectPageNumber), documentPageCount); } }
		bool IsValidPrintToFile { 
			get { 
				string message;
				return !PrintToFile || PrintEditorController.ValidateFilePath(printFileName, out message); 
			} 
		}
		string InvalidFilePathMessage { 
			get { 
				string message;
				return (PrintToFile && !PrintEditorController.ValidateFilePath(printFileName, out message)) ? PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectPrintableFilePath) : String.Empty;
			} 
		}
		string IDataErrorInfo.Error { 
			get { 
				string pageRangeValidationMessage = InvalidPageRangeMessage;
				return String.IsNullOrEmpty(pageRangeValidationMessage) ? InvalidFilePathMessage : pageRangeValidationMessage; 
			} 
		}
		string IDataErrorInfo.this[string columnName] {
			get {
				switch (columnName) {
					case PageRangeTextPropertyName:
						return InvalidPageRangeMessage;
					case PrintFileNamePropertyName:
						return InvalidFilePathMessage;
					default: 
						return String.Empty;
				}
			}
		}
		public PdfPrintDialogViewModel(PdfDocumentState documentState, Size previewSize, int currentPageNumber, int maxPrintingDpi, Func<string, bool> showFileReplacingRequest, PdfPrinterSettings pdfPrinterSettings) {
			this.currentPageNumber = currentPageNumber;
			this.maxPrintingDpi = maxPrintingDpi;
			this.showFileReplacingRequest = showFileReplacingRequest;
			this.pdfPrinterSettings = pdfPrinterSettings;
			printerSettings = pdfPrinterSettings.Settings;
			printFileName = printerSettings.PrintFileName;
			documentPageCount = documentState.Document.Pages.Count;
			dialogPreview = new PdfPrintDialogPreview(documentState, pdfPrinterSettings, 0, previewSize);
			using (PrinterItemContainer container = new PrinterItemContainer()) {
				printerItems = container.Items;
				if (!printerSettings.IsValid) {
					string defaultPrinterName = container.DefaultPrinterName;
					if (!String.IsNullOrEmpty(defaultPrinterName))
						printerSettings.PrinterName = defaultPrinterName;
				}
			}
			string printerName = printerSettings.PrinterName;
			int count = printerItems.Count;
			for (int i = 0; i < count; i++)
				if (printerItems[i].FullName == printerName) {
					printerIndex = i;
					break;
				}
			int[] pageNumbers = PageNumbers;
			if (pageNumbers != null) {
				int pageNumberCount = pageNumbers.Length;
				if (pageNumberCount > 0) {
					StringBuilder sb = new StringBuilder();
					int prevNumber = pageNumbers[0];
					sb.Append(prevNumber);
					for (int index = 1; index < pageNumberCount; index++) {
						int pageNumber = pageNumbers[index];
						int difference = pageNumber - prevNumber;
						if (Math.Abs(difference) == 1) {
							prevNumber = pageNumber;
							for (++index; index < pageNumberCount; index++) {
								pageNumber = pageNumbers[index];
								if (pageNumber - prevNumber != difference)
									break;
								prevNumber = pageNumber;
							}
							sb.Append("-");
							sb.Append(prevNumber);
							if (index == pageNumberCount)
								break;
						}
						sb.Append(',');
						sb.Append(pageNumber);
						prevNumber = pageNumber;
					}
					pageRangeText = sb.ToString();
				}
			}
			if (String.IsNullOrEmpty(pageRangeText))
				pageRangeText = "1-" + documentPageCount;
			if (PrintRange != PrintRange.SomePages)
				UpdatePageNumbers();
			UpdatePaperSources();
			pagePreviewIndex = PageCount > 0 ? 0 : -1;
			currentPreviewPageNumber = CurrentPageNumber;
			dialogPreview.PageIndex = currentPreviewPageNumber - 1;
		}
		public void UpdatePreviewSize(int width, int height) {
			dialogPreview.PreviewSize = new Size(width, height);
			RefreshPreviewImage();
		}
		public void ShowPreferences(IntPtr handle) {
			try {
				int copies = printerSettings.Copies;
				bool collate = printerSettings.Collate;
				new PrinterPreferences().ShowPrinterProperties(printerSettings, handle);
				if (PageOrientation != PdfPrintPageOrientation.Auto)
					PageOrientation = printerSettings.DefaultPageSettings.Landscape ? PdfPrintPageOrientation.Landscape : PdfPrintPageOrientation.Portrait;
				if (printerSettings.Copies != copies)
					RaiseCopiesPropertyChanged();
				if (printerSettings.Collate != collate)
					RaisePropertyChanged(CollatePropertyName);
				RefreshPreviewImage();
			}
			catch { 
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing && dialogPreview != null)
				dialogPreview.Dispose();
		}
		void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		void RaiseCopiesPropertyChanged() {
			RaisePropertyChanged(CopiesPropertyName);
			RaisePropertyChanged(AllowCollatePropertyName);
		}
		bool SetProperty<T>(ref T storage, T value, string propertyName, Action changedCallback) {
			if (Object.Equals(storage, value))
				return false;
			storage = value;
			changedCallback();
			RaisePropertyChanged(propertyName);
			return true;
		}
		void RefreshPreviewImage() {
			dialogPreview.PageIndex = currentPreviewPageNumber - 1;
			dialogPreview.DisposePreviewImage();
			previewImageBytes = null;
			RaisePropertyChanged(PreviewImagePropertyName);
			RaisePropertyChanged(ShowFitScaleTextPropertyName);
			RaisePropertyChanged(FitScalePropertyName);
			RaisePropertyChanged(PreviewImageBytesPropertyName);
		}
		void SetCurrentPreviewPage() {
			if (PageCount > 0) {
				if (pagePreviewIndex == -1) {
					pagePreviewIndex = 0;
					RaisePropertyChanged(PagePreviewIndexPropertyName);
				}
			}
			else if (pagePreviewIndex != -1) {
				pagePreviewIndex = -1;
				RaisePropertyChanged(PagePreviewIndexPropertyName);
			}
			int pageNumber = CurrentPageNumber;
			if (pageNumber != currentPreviewPageNumber) {
				currentPreviewPageNumber = pageNumber;
				RaisePropertyChanged(CurrentPreviewPageNumberPropertyName);
				RefreshPreviewImage();
			}
		}
		void UpdatePageNumbers() {
			if (PageNumbers != null && String.IsNullOrEmpty(pageRangeText) && PrintRange == PrintRange.SomePages)
				pdfPrinterSettings.PageNumbers = new int[0];
			else
				pdfPrinterSettings.SetPageNumbers(currentPageNumber, documentPageCount, pageRangeText);
		}
		void UpdatePaperSources() {
			paperSources = new List<string>();
			foreach (PaperSource paperSource in printerSettings.PaperSources) {
				string paperSourceName = paperSource.SourceName;
				if (!String.IsNullOrEmpty(paperSourceName))
					paperSources.Add(paperSourceName);
			}
		}
		void UpdatePreview() {
			UpdatePageNumbers();
			RaisePropertyChanged(PageNumbersPropertyName);
			int pageCount = PageCount;
			if (pagePreviewIndex >= pageCount)
				PagePreviewIndex = pageCount - 1;
			SetCurrentPreviewPage();
		}
		void SetPrintRange(PrintRange printRange) {
			PrintRange previousPrintRange = PrintRange;
			if (printRange != previousPrintRange) {
				printerSettings.PrintRange = printRange;
				UpdatePreview();
				RaisePropertyChanged(PrintRangePropertyName);
				if (printRange == PrintRange.SomePages || previousPrintRange == PrintRange.SomePages) {
					RaisePropertyChanged(AllowSomePagesPropertyName);
					RaisePropertyChanged(PageRangeTextPropertyName);
				}
			}
		}
		void UpdatePrinterSettingsPrintFileName() {
			if (!String.IsNullOrEmpty(printFileName))
				printerSettings.PrintFileName = printFileName;
		}
		void OnPrinterIndexChanged() {
			PrinterSettings newPrinterSettings = new PrinterSettings();
			newPrinterSettings.PrinterName = printerItems[printerIndex].FullName;
			newPrinterSettings.Collate = printerSettings.Collate;
			newPrinterSettings.Copies = printerSettings.Copies;
			newPrinterSettings.FromPage = printerSettings.FromPage;
			newPrinterSettings.ToPage = printerSettings.ToPage;
			newPrinterSettings.PrintRange = printerSettings.PrintRange;
			newPrinterSettings.PrintToFile = printerSettings.PrintToFile;
			printerSettings = newPrinterSettings;
			UpdatePrinterSettingsPrintFileName();
			pdfPrinterSettings = new PdfPrinterSettings(printerSettings, pdfPrinterSettings.PageNumbers, pdfPrinterSettings.PageOrientation, pdfPrinterSettings.Scale, pdfPrinterSettings.ScaleMode);
			dialogPreview.PrinterSettings = pdfPrinterSettings;
			RaisePropertyChanged(PrinterStatusPropertyName);
			RaisePropertyChanged(PrinterLocationPropertyName);
			RaisePropertyChanged(PrinterCommentPropertyName);
			RaisePropertyChanged(PrinterDocumentsInQueuePropertyName);
			RaisePropertyChanged(PrintingDpiPropertyName);
			RaisePropertyChanged(MaxDpiPropertyName);
			UpdatePaperSources();
			RaisePropertyChanged(PaperSourcesPropertyName);
			RefreshPreviewImage();
		}
	}
}
