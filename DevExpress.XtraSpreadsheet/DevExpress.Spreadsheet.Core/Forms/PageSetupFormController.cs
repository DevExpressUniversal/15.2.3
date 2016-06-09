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

using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region PageSetupFormInitialTabPage
	public enum PageSetupFormInitialTabPage {
		Page = 0,
		Margins = 1,
		HeaderFooter = 2,
		Sheet = 3
	}
	#endregion
	#region PageSetupViewModel
	public class PageSetupViewModel : ViewModelBase {
		#region StaticMembers
		static List<PaperKind> CreateDefaultPaperKindList() {
			List<PaperKind> result = new List<PaperKind>();
			result.Add(PaperKind.Letter);
			result.Add(PaperKind.Legal);
			result.Add(PaperKind.Folio);
			result.Add(PaperKind.A4);
			result.Add(PaperKind.B5);
			result.Add(PaperKind.Executive);
			result.Add(PaperKind.A5);
			result.Add(PaperKind.A6);
			return result;
		}
		static Dictionary<int, string> CreatePrintQualityList() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			result.Add(600, "600 dpi");
			return result;
		}
		public static List<PaperKind> DefaultPaperKindList = CreateDefaultPaperKindList();
		public static Dictionary<int, string> DefaultPrintQualityList = CreatePrintQualityList();
		static Dictionary<ModelCommentsPrintMode, string> PopulateCommentsPrintMode() {
			Dictionary<ModelCommentsPrintMode, string> result = new Dictionary<ModelCommentsPrintMode, string>();
			result.Add(ModelCommentsPrintMode.None, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.PageSetupForm_CommentsPrintModeNone));
			result.Add(ModelCommentsPrintMode.AtEnd, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.PageSetupForm_CommentsPrintModeAtEndOfSheet));
			result.Add(ModelCommentsPrintMode.AsDisplayed, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.PageSetupForm_CommentsPrintModeAsDisplayedOnSheet));
			return result;
		}
		static Dictionary<ModelErrorsPrintMode, string> PopulateErrorPrintMode() {
			Dictionary<ModelErrorsPrintMode, string> result = new Dictionary<ModelErrorsPrintMode, string>();
			result.Add(ModelErrorsPrintMode.Displayed, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.PageSetupForm_ErrorPrintModeDisplayed));
			result.Add(ModelErrorsPrintMode.Blank, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.PageSetupForm_ErrorPrintModeBlank));
			result.Add(ModelErrorsPrintMode.Dash, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.PageSetupForm_ErrorPrintModeDash));
			result.Add(ModelErrorsPrintMode.NA, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.PageSetupForm_ErrorPrintModeNA));
			return result;
		}
		public Dictionary<string, string> PredefinedHFStrings() {
			string noneString = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_None);
			string pageString = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_Page);
			string ofString = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_Of);
			string preparedByString = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterPredefinedString_PreparedBy);
			Dictionary<string, string> result = new Dictionary<string, string>();
			result.Add(String.Empty, noneString);
			result.Add("&C" + pageString + " " + "&P", pageString + " " + provider.CurrentPage);
			result.Add("&C" + pageString + " " + "&P" + " " + ofString + " " + "&N", pageString + " " + provider.CurrentPage + " " + ofString + " " + provider.TotalPages);
			result.Add("&C&A", provider.SheetName);
			if (!String.IsNullOrEmpty(provider.FileName))
				result.Add("&C&F", provider.FileName);
			if (!String.IsNullOrEmpty(provider.FilePath) && !String.IsNullOrEmpty(provider.FileName))
				result.Add("&C&Z&F", provider.FilePath + provider.FileName);
			result.Add("&C&A&R" + pageString + " " + "&P", provider.SheetName + "; " + pageString + " " + provider.CurrentPage);
			if (!String.IsNullOrEmpty(provider.FileName))
				result.Add("&C&F&R" + pageString + " " + "&P", provider.FileName + "; " + pageString + " " + provider.CurrentPage);
			if (!String.IsNullOrEmpty(provider.FilePath) && !String.IsNullOrEmpty(provider.FileName))
				result.Add("&C&Z&F&R" + pageString + " " + "&P", provider.FilePath + provider.FileName + "; " + pageString + " " + provider.CurrentPage);
			result.Add("&C" + pageString + " " + "&P&R&A", pageString + " " + provider.CurrentPage + "; " + provider.SheetName);
			if (!String.IsNullOrEmpty(provider.FileName))
				result.Add("&C" + pageString + " " + "&P&R&F", pageString + " " + provider.CurrentPage + "; " + provider.FileName);
			if (!String.IsNullOrEmpty(provider.FilePath) && !String.IsNullOrEmpty(provider.FileName))
				result.Add("&C" + pageString + " " + "&P&R&Z&F", pageString + " " + provider.CurrentPage + "; " + provider.FilePath + provider.FileName);
			result.Add("&L" + Author + "&C" + pageString + " " + "&P&R&D", Author + "; " + pageString + " " + provider.CurrentPage + "; " + provider.CurrentDate);
			result.Add("&C" + preparedByString + " " + Author + " " + "&D&R" + pageString + " " + "&P", preparedByString + " " + Author + " " + provider.CurrentDate + "; " + pageString + " " + provider.CurrentPage);
			return result;
		}
		#endregion
		#region Fields
		List<PaperKind> paperTypes = CreateDefaultPaperKindList();
		Dictionary<int, string> printQualityList = CreatePrintQualityList();
		Dictionary<ModelCommentsPrintMode, string> commentsPrintModeList = PopulateCommentsPrintMode();
		Dictionary<ModelErrorsPrintMode, string> errorsPrintModeList = PopulateErrorPrintMode();
		ISpreadsheetControl control;
		readonly PageSetupFormInitialTabPage initialTabPage;
		Dictionary<string, string> predefinedHeaderFooterList;
		bool orientationPortrait = false;
		int scale = 0;
		bool fitToPage = false;
		int fitToWidth = 0;
		int fitToHeight = 0;
		PaperKind paperType;
		int defaultDpi = 600;
		int horizontalDpi = 0;
		int verticalDpi = 0;
		string firstPageNumber = String.Empty;
		bool useFirstPageNumber = false;
		float topMargin = 0.0F;
		float bottomMargin = 0.0F;
		float leftMargin = 0.0F;
		float rightMargin = 0.0F;
		float headerMargin = 0.0F;
		float footerMargin = 0.0F;
		bool centerHorizontally = false;
		bool centerVertically = false;
		HeaderFooterFormatTagProvider provider;
		bool differentOddEven = false;
		bool differentFirstPage = false;
		bool scaleWithDocument = false;
		bool alignWithMargins = false;
		string oddHeader = String.Empty;
		string oddFooter = String.Empty;
		string evenHeader = String.Empty;
		string evenFooter = String.Empty;
		string firstHeader = String.Empty;
		string firstFooter = String.Empty;
		string printArea = String.Empty;
		List<CellRange> printRanges = new List<CellRange>();
		bool isIncorrectRange = false;
		bool printGridlines = false;
		bool blackAndWhite = false;
		bool draft = false;
		bool printHeadings = false;
		ModelCommentsPrintMode commentsPrintMode;
		ModelErrorsPrintMode errorsPrintMode;
		bool downThenOver = false;
		#endregion
		public PageSetupViewModel(ISpreadsheetControl Control, PageSetupFormInitialTabPage initialTabPage) {
			this.initialTabPage = initialTabPage;
			this.control = Control;
			GetPrintArea();
			provider = new HeaderFooterFormatTagProvider(DocumentModel.ActiveSheet);
			predefinedHeaderFooterList = PredefinedHFStrings();
		}
		#region Properties
		public ISpreadsheetControl Control { get { return control; } }
		public string Author { get { return Control.Document.DocumentProperties.Author; } }
		public DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		public PageSetupFormInitialTabPage InitialTabPage { get { return initialTabPage; } }
		public DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		public Model.DefinedNameCollection DefinedNames { get { return DocumentModel.ActiveSheet.DefinedNames; } }
		#endregion
		#region PageSetup
		public bool OrientationPortrait {
			get { return orientationPortrait; }
			set {
				if (OrientationPortrait == value)
					return;
				orientationPortrait = value;
				OnPropertyChanged("OrientationPortrait");
			}
		}
		public int Scale {
			get { return scale; }
			set {
				if (Scale == value)
					return;
				scale = value;
				OnPropertyChanged("Scale");
			}
		}
		public bool FitToPage {
			get { return fitToPage; }
			set {
				if (FitToPage == value)
					return;
				fitToPage = value;
				OnPropertyChanged("FitToPage");
			}
		}
		public int FitToWidth {
			get { return fitToWidth; }
			set {
				if (FitToWidth == value)
					return;
				fitToWidth = value;
				OnPropertyChanged("FitToWidth");
			}
		}
		public int FitToHeight {
			get { return fitToHeight; }
			set {
				if (FitToHeight == value)
					return;
				fitToHeight = value;
				OnPropertyChanged("FitToHeight");
			}
		}
		public List<PaperKind> PaperTypes {
			get { return paperTypes; }
		}
		public PaperKind PaperType {
			get { return paperType; }
			set {
				if (PaperType == value)
					return;
				paperType = value;
				OnPropertyChanged("PaperType");
			}
		}
		public int HorizontalDpi {
			get { return horizontalDpi; }
			set {
				if (HorizontalDpi == value)
					return;
				horizontalDpi = value;
				OnPropertyChanged("HorizontalDpi");
			}
		}
		public int VerticalDpi {
			get { return verticalDpi; }
			set {
				if (VerticalDpi == value)
					return;
				verticalDpi = value;
				OnPropertyChanged("VerticalDpi");
			}
		}
		#region PrintQuality
		public List<string> PrintQuality {
			get { return new List<string>(printQualityList.Values); }
		}
		public string PrintQualityMode {
			get { return GetPrintQualityStringByint(); }
			set {
				if (PrintQualityMode == value)
					return;
				HorizontalDpi = GetPrintQualityIntByString(value);
				VerticalDpi = GetPrintQualityIntByString(value);
				OnPropertyChanged("CommentsPrintMode");
			}
		}
		public string GetPrintQualityStringByint() {
			if (VerticalDpi == defaultDpi && HorizontalDpi == defaultDpi)
				return printQualityList[defaultDpi];
			return String.Empty;
		}
		public int GetPrintQualityIntByString(string printQuality) {
			foreach (int key in printQualityList.Keys)
				if (printQualityList[key] == printQuality)
					return key;
			Exceptions.ThrowInternalException();
			return 0;
		}
		#endregion
		public string FirstPageNumber {
			get { return firstPageNumber.ToString(); }
			set {
				if (FirstPageNumber == value)
					return;
				int parseFirstPageNumberValue;
				if (Int32.TryParse(value, out parseFirstPageNumberValue)) {
					firstPageNumber = parseFirstPageNumberValue.ToString();
					UseFirstPageNumber = true;
				}
				else {
					if (value.ToLower() == "auto" || String.IsNullOrEmpty(value)) {
						firstPageNumber = "Auto";
						UseFirstPageNumber = false;
					}
					else {
						firstPageNumber = value;
					}
				}
				OnPropertyChanged("FirstPageNumber");
			}
		}
		public bool UseFirstPageNumber {
			get { return useFirstPageNumber; }
			set {
				if (UseFirstPageNumber == value)
					return;
				useFirstPageNumber = value;
				OnPropertyChanged("UseFirstPageNumber");
			}
		}
		#endregion
		#region MarginsSetup
		public float TopMargin {
			get { return topMargin; }
			set {
				if (TopMargin == value)
					return;
				topMargin = value;
				OnPropertyChanged("TopMargin");
			}
		}
		public float BottomMargin {
			get { return bottomMargin; }
			set {
				if (BottomMargin == value)
					return;
				bottomMargin = value;
				OnPropertyChanged("BottomMargin");
			}
		}
		public float LeftMargin {
			get { return leftMargin; }
			set {
				if (LeftMargin == value)
					return;
				leftMargin = value;
				OnPropertyChanged("LeftMargin");
			}
		}
		public float RightMargin {
			get { return rightMargin; }
			set {
				if (RightMargin == value)
					return;
				rightMargin = value;
				OnPropertyChanged("RightMargin");
			}
		}
		public float HeaderMargin {
			get { return headerMargin; }
			set {
				if (HeaderMargin == value)
					return;
				headerMargin = value;
				OnPropertyChanged("HeaderMargin");
			}
		}
		public float FooterMargin {
			get { return footerMargin; }
			set {
				if (FooterMargin == value)
					return;
				footerMargin = value;
				OnPropertyChanged("FooterMargin");
			}
		}
		public bool CenterHorizontally {
			get { return centerHorizontally; }
			set {
				if (CenterHorizontally == value)
					return;
				centerHorizontally = value;
				OnPropertyChanged("CenterHorizontally");
			}
		}
		public bool CenterVertically {
			get { return centerVertically; }
			set {
				if (CenterVertically == value)
					return;
				centerVertically = value;
				OnPropertyChanged("CenterVertically");
			}
		}
		#endregion
		#region HeaderFooterSetup
		public List<string> PredefinedHeaderFooterList {
			get { return new List<string>(predefinedHeaderFooterList.Values); }
		}
		public string PredefinedHeaderValueForList {
			get { return GetValueHeaderFooterDictionary(OddHeader); }
			set {
				if (PredefinedHeaderValueForList == value)
					return;
				OddHeader = GetKeyHeaderFooterDictionary(value);
				OnPropertyChanged("PredefinedHeaderValueForList");
			}
		}
		public string PredefinedFooterValueForList {
			get { return GetValueHeaderFooterDictionary(OddFooter); }
			set {
				if (PredefinedFooterValueForList == value)
					return;
				OddFooter = GetKeyHeaderFooterDictionary(value);
				OnPropertyChanged("PredefinedFooterValueForList");
			}
		}
		public string GetValueHeaderFooterDictionary(string value) {
			if (predefinedHeaderFooterList.ContainsKey(value))
				return predefinedHeaderFooterList[value];
			return String.Empty;
		}
		public string GetKeyHeaderFooterDictionary(string commentsMode) {
			foreach (string key in predefinedHeaderFooterList.Keys)
				if (predefinedHeaderFooterList[key] == commentsMode)
					return key;
			Exceptions.ThrowInternalException();
			return String.Empty;
		}
		public bool PredefinedHeaderFooterListEnabled {
			get { return !DifferentFirstPage && !DifferentOddEven; }
		}
		public HeaderFooterFormatTagProvider Provider {
			get { return provider; }
			set {
				if (Provider == value)
					return;
				provider = value;
				OnPropertyChanged("Provider");
			}
		}
		public bool DifferentOddEven {
			get { return differentOddEven; }
			set {
				if (DifferentOddEven == value)
					return;
				differentOddEven = value;
				OnPropertyChanged("DifferentOddEven");
				OnPropertyChanged("PredefinedHeaderFooterListEnabled");
			}
		}
		public bool DifferentFirstPage {
			get { return differentFirstPage; }
			set {
				if (DifferentFirstPage == value)
					return;
				differentFirstPage = value;
				OnPropertyChanged("DifferentFirstPage");
				OnPropertyChanged("PredefinedHeaderFooterListEnabled");
			}
		}
		public bool ScaleWithDocument {
			get { return scaleWithDocument; }
			set {
				if (ScaleWithDocument == value)
					return;
				scaleWithDocument = value;
				OnPropertyChanged("ScaleWithDocument");
			}
		}
		public bool AlignWithMargins {
			get { return alignWithMargins; }
			set {
				if (AlignWithMargins == value)
					return;
				alignWithMargins = value;
				OnPropertyChanged("AlignWithMargins");
			}
		}
		public string OddHeader {
			get { return oddHeader; }
			set {
				if (OddHeader == value)
					return;
				oddHeader = value;
				OnPropertyChanged("OddHeader");
				OnPropertyChanged("PredefinedHeaderValueForList");
			}
		}
		public string OddFooter {
			get { return oddFooter; }
			set {
				if (OddFooter == value)
					return;
				oddFooter = value;
				OnPropertyChanged("OddFooter");
				OnPropertyChanged("PredefinedFooterValueForList");
			}
		}
		public string EvenHeader {
			get { return evenHeader; }
			set {
				if (EvenHeader == value)
					return;
				evenHeader = value;
				OnPropertyChanged("EvenHeader");
			}
		}
		public string EvenFooter {
			get { return evenFooter; }
			set {
				if (EvenFooter == value)
					return;
				evenFooter = value;
				OnPropertyChanged("EvenFooter");
			}
		}
		public string FirstHeader {
			get { return firstHeader; }
			set {
				if (FirstHeader == value)
					return;
				firstHeader = value;
				OnPropertyChanged("FirstHeader");
			}
		}
		public string FirstFooter {
			get { return firstFooter; }
			set {
				if (FirstFooter == value)
					return;
				firstFooter = value;
				OnPropertyChanged("FirstFooter");
			}
		}
		#endregion
		#region SheetSetup
		public string PrintArea {
			get { return printArea; }
			set {
				if (PrintArea == value)
					return;
				SetPrintArea(value);
				OnPropertyChanged("PrintArea");
			}
		}
		public List<CellRange> PrintRanges { get { return printRanges; } }
		public bool IsIncorrectRange {
			get { return isIncorrectRange; }
			set {
				if (IsIncorrectRange == value)
					return;
				isIncorrectRange = value;
			}
		}
		public bool PrintGridlines {
			get { return printGridlines; }
			set {
				if (PrintGridlines == value)
					return;
				printGridlines = value;
				OnPropertyChanged("PrintGridlines");
			}
		}
		public bool BlackAndWhite {
			get { return blackAndWhite; }
			set {
				if (BlackAndWhite == value)
					return;
				blackAndWhite = value;
				OnPropertyChanged("BlackAndWhite");
			}
		}
		public bool Draft {
			get { return draft; }
			set {
				if (Draft == value)
					return;
				draft = value;
				OnPropertyChanged("Draft");
			}
		}
		public bool PrintHeadings {
			get { return printHeadings; }
			set {
				if (PrintHeadings == value)
					return;
				printHeadings = value;
				OnPropertyChanged("PrintHeadings");
			}
		}
		#region CommentsPrintMode
		public List<string> CommentsPrintModeList {
			get { return new List<string>(commentsPrintModeList.Values); }
		}
		public string CommentsPrintMode {
			get { return GetCommentModeStringByEnum(commentsPrintMode); }
			set {
				if (CommentsPrintMode == value)
					return;
				commentsPrintMode = GetCommentModeByString(value);
				OnPropertyChanged("CommentsPrintMode");
			}
		}
		public string GetCommentModeStringByEnum(ModelCommentsPrintMode commentsMode) {
			return commentsPrintModeList[commentsMode];
		}
		public ModelCommentsPrintMode GetCommentModeByString(string commentsMode) {
			foreach (ModelCommentsPrintMode key in commentsPrintModeList.Keys)
				if (commentsPrintModeList[key] == commentsMode)
					return key;
			Exceptions.ThrowInternalException();
			return ModelCommentsPrintMode.None;
		}
		#endregion
		#region ErrorsPrintMode
		public List<string> ErrorsPrintModeList {
			get { return new List<string>(errorsPrintModeList.Values); }
		}
		public string ErrorsPrintMode {
			get { return GetErrorModeStringByEnum(errorsPrintMode); }
			set {
				if (ErrorsPrintMode == value)
					return;
				errorsPrintMode = GetErrorModeByString(value);
				OnPropertyChanged("ErrorsPrintMode");
			}
		}
		public string GetErrorModeStringByEnum(ModelErrorsPrintMode errorsMode) {
			return errorsPrintModeList[errorsMode];
		}
		public ModelErrorsPrintMode GetErrorModeByString(string errorsMode) {
			foreach (ModelErrorsPrintMode key in errorsPrintModeList.Keys)
				if (errorsPrintModeList[key] == errorsMode)
					return key;
			Exceptions.ThrowInternalException();
			return ModelErrorsPrintMode.Displayed;
		}
		#endregion
		public bool DownThenOver {
			get { return downThenOver; }
			set {
				if (DownThenOver == value)
					return;
				downThenOver = value;
				OnPropertyChanged("DownThenOver");
			}
		}
		#endregion
		public void SetPrintArea(string value) {
			CellRange range;
			printArea = value;
			printRanges.Clear();
			IsIncorrectRange = false;
			foreach (string printRange in PrintArea.Split(';')) {
				range = CellRange.TryParse(printRange, DocumentModel.DataContext) as CellRange;
				if (range == null) 
					IsIncorrectRange = true;
				else
					printRanges.Add(range);
			}
		}
		public void GetPrintArea() {
			DefinedNameBase definedName = null;
			if (!DefinedNames.TryGetItemByName(PrintAreaCalculator.PrintAreaDefinedName, out definedName)) {
				PrintArea = String.Empty;
				return;
			}
			CellRangeBase range = definedName.GetReferencedRange();
			if (range == null)
				PrintArea = String.Empty;
			else
				PrintArea = range.GetWithModifiedPositionType(PositionType.Relative).ToString();
		}
		public void UpdateHeaderFooter(HeaderFooterViewModel viewModel) {
			DifferentOddEven = viewModel.DifferentOddEven;
			DifferentFirstPage = viewModel.DifferentFirstPage;
			ScaleWithDocument = viewModel.ScaleWithDocument;
			AlignWithMargins = viewModel.AlignWithMargins;
			OddHeader = viewModel.OddHeader;
			OddFooter = viewModel.OddFooter;
			EvenHeader = viewModel.EvenHeader;
			EvenFooter = viewModel.EvenFooter;
			FirstHeader = viewModel.FirstHeader;
			FirstFooter = viewModel.FirstFooter;
		}
		public bool PageValidate() {
			ShowPageSetupCommand command = new ShowPageSetupCommand(Control);
			return command.Validate(this);
		}
		public bool MarginsValidate() {
			ShowPageSetupMarginsCommand command = new ShowPageSetupMarginsCommand(Control);
			return command.Validate(this);
		}
		public bool HeaderFooterValidate() {
			ShowPageSetupHeaderFooterCommand command = new ShowPageSetupHeaderFooterCommand(Control);
			return command.Validate(this);
		}
		public bool SheetValidate() {
			ShowPageSetupSheetCommand command = new ShowPageSetupSheetCommand(Control);
			return command.Validate(this);
		}
		public void ApplyChanges() {
			ShowPageSetupCommand command = new ShowPageSetupCommand(Control);
			command.ApplyChanges(this);
		}
	}
	#endregion
	public static class FormatTagHFConverter {
		public static string PageNumberAnalog = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterFormatTag_PageNumberAnalog);
		public static string PageTotalAnalog = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterFormatTag_PageTotalAnalog);
		public static string DateAnalog = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterFormatTag_DateAnalog);
		public static string TimeAnalog = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterFormatTag_TimeAnalog);
		public static string WorkbookFilePathAnalog = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterFormatTag_WorkbookFilePathAnalog);
		public static string WorkbookFileNameAnalog = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterFormatTag_WorkbookFileNameAnalog);
		public static string WorksheetNameAnalog = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HeaderFooterFormatTag_WorksheetNameAnalog);
		public static string ConvertToAnalogTag(string text) {
			text = text.Replace(HeaderFooterCode.PageNumber, FormatTagHFConverter.PageNumberAnalog);
			text = text.Replace(HeaderFooterCode.PageTotal, FormatTagHFConverter.PageTotalAnalog);
			text = text.Replace(HeaderFooterCode.Date, FormatTagHFConverter.DateAnalog);
			text = text.Replace(HeaderFooterCode.Time, FormatTagHFConverter.TimeAnalog);
			text = text.Replace(HeaderFooterCode.WorkbookFilePath, FormatTagHFConverter.WorkbookFilePathAnalog);
			text = text.Replace(HeaderFooterCode.WorkbookFileName, FormatTagHFConverter.WorkbookFileNameAnalog);
			text = text.Replace(HeaderFooterCode.WorksheetName, FormatTagHFConverter.WorksheetNameAnalog);
			return text;
		}
		public static string ConvertToOriginalTag(string text) {
			text = text.Replace(FormatTagHFConverter.PageNumberAnalog, HeaderFooterCode.PageNumber);
			text = text.Replace(FormatTagHFConverter.PageTotalAnalog, HeaderFooterCode.PageTotal);
			text = text.Replace(FormatTagHFConverter.DateAnalog, HeaderFooterCode.Date);
			text = text.Replace(FormatTagHFConverter.TimeAnalog, HeaderFooterCode.Time);
			text = text.Replace(FormatTagHFConverter.WorkbookFilePathAnalog, HeaderFooterCode.WorkbookFilePath);
			text = text.Replace(FormatTagHFConverter.WorkbookFileNameAnalog, HeaderFooterCode.WorkbookFileName);
			text = text.Replace(FormatTagHFConverter.WorksheetNameAnalog, HeaderFooterCode.WorksheetName);
			return text;
		}
	}
}
