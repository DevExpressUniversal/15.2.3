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

using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Office.Model;
using DevExpress.Office;
using System;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
#if !SL
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
#else
using System.Windows.Media;
using DevExpress.Xpf.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet.Forms {
	#region FormatCellsFormControllerParameters
	public class FormatCellsFormControllerParameters : FormControllerParameters {
		#region Fields
		readonly CellFormat sourceFormat;
		#endregion
		public FormatCellsFormControllerParameters(ISpreadsheetControl control, CellFormat sourceFormat)
			: base(control) {
			Guard.ArgumentNotNull(sourceFormat, "sourceFormat");
			this.sourceFormat = sourceFormat;
		}
		#region Properties
		public CellFormat SourceFormat { get { return sourceFormat; } }
		#endregion
	}
	#endregion
	#region FormatCellsFormController
	public class FormatCellsFormController : FormController {
		#region Fields
		readonly CellFormat sourceFormat;
		#endregion
		public FormatCellsFormController(FormatCellsFormControllerParameters parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			this.sourceFormat = parameters.SourceFormat;
			InitializeController();
		}
		#region Properties
		protected internal CellFormat SourceFormat { get { return sourceFormat; } }
		#endregion
		protected internal virtual void InitializeController() {
		}
		public override void ApplyChanges() {
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Forms {
	#region FormatCellsFormInitialTabPage
	public enum FormatCellsFormInitialTabPage {
		Number = 0,
		Alignment = 1,
		Font = 2,
		Border = 3,
		Fill = 4,
		Protection = 5
	}
	#endregion
	#region FormatNumberCategory
	public enum FormatNumberCategory {
		General = 0,
		Number = 1,
		Currency = 2,
		Accounting = 3,
		Date = 4,
		Time = 5,
		Percentage = 6,
		Fraction = 7,
		Scientific = 8,
		Text = 9,
		Custom = 11
	}
	#endregion
	#region FormatCellsViewModel
	public class FormatCellsViewModel : ViewModelBase {
		#region Static Members
		static int defaultDecimalPlaces = 2;
		static string baseNumberFormatString = "0";
		static string baseCurrencyFormatString = @"""$""#,##0";
		static string baseAccountingFormatString = @"_(""$""* #,##0{0}_);_(""$""* \(#,##0{1}\);_(""$""* ""-""{2}_);_(@_)";
		static List<string> timeCaptions = CreateTimeCaptions();
		static List<string> CreateTimeCaptions() {
			List<string> result = new List<string>();
			result.Add("*1:30:55 PM");
			result.Add("13:30");
			result.Add("1:30 PM");
			result.Add("13:30:55");
			result.Add("1:30:55 PM");
			result.Add("30:55.2");
			result.Add("37:30:55");
			result.Add("3/14/01 1:30 PM");
			result.Add("3/14/01 13:30");
			return result;
		}
		static List<string> timeFormatCodes = CreateTimeFormatCodes();
		static List<string> CreateTimeFormatCodes() {
			List<string> result = new List<string>();
			result.Add(@"[$-F400]h:mm:ss\ AM/PM");
			result.Add(@"h:mm;@");
			result.Add(@"[$-409]h:mm\ AM/PM;@");
			result.Add(@"h:mm:ss;@");
			result.Add(@"[$-409]h:mm:ss\ AM/PM;@");
			result.Add(@"mm:ss.0;@");
			result.Add(@"[h]:mm:ss;@");
			result.Add(@"[$-409]m/d/yy\ h:mm\ AM/PM;@");
			result.Add(@"m/d/yy\ h:mm;@");
			return result;
		}
		static List<string> fractionCaptions = CreateFractionCaptions();
		static List<string> CreateFractionCaptions() {
			List<string> result = new List<string>();
			result.Add("Up to one digit (1/4)");
			result.Add("Up to two digits (21/25)");
			result.Add("Up to three digits (312/943)");
			result.Add("As halves (1/2)");
			result.Add("As quarters (2/4)");
			result.Add("As eighths (4/8)");
			result.Add("As sixteenths (8/16)");
			result.Add("As tenths (3/10)");
			result.Add("As hundredths (30/100)");
			return result;
		}
		static List<string> fractionFormatCodes = CreateFractionFormatCodes();
		static List<string> CreateFractionFormatCodes() {
			List<string> result = new List<string>();
			result.Add(@"# ?/?");
			result.Add(@"# ??/??");
			result.Add(@"#\ ???/???");
			result.Add(@"#\ ?/2");
			result.Add(@"#\ ?/4");
			result.Add(@"#\ ?/8");
			result.Add(@"#\ ??/16");
			result.Add(@"#\ ?/10");
			result.Add(@"#\ ??/100");
			return result;
		}
		static List<string> dateCaptions = CreateDateCaptions();
		static List<string> CreateDateCaptions() {
			List<string> result = new List<string>();
			result.Add("*3/14/2001");
			result.Add("*Wednesday, March 14, 2001");
			result.Add("3/14");
			result.Add("3/14/01");
			result.Add("03/14/01");
			result.Add("14-Mar");
			result.Add("14-Mar-01");
			result.Add("14-Mar-01");
			result.Add("Mar-01");
			result.Add("March-01");
			result.Add("March 14, 2001");
			result.Add("3/14/01 1:30 PM");
			result.Add("3/14/01 13:30");
			result.Add("M");
			result.Add("M-01");
			result.Add("3/14/2001");
			result.Add("14-Mar-2001");
			return result;
		}
		static List<string> dateFormatCodes = CreateDateFormatCodes();
		static List<string> CreateDateFormatCodes() {
			List<string> result = new List<string>();
			result.Add(@"mm-dd-yy");
			result.Add(@"[$-F800]dddd\,\ mmmm\ dd\,\ yyyy");
			result.Add(@"m/d;@");
			result.Add(@"m/d/yy;@");
			result.Add(@"mm/dd/yy;@");
			result.Add(@"[$-409]d\-mmm;@");
			result.Add(@"[$-409]d\-mmm\-yy;@");
			result.Add(@"[$-409]dd\-mmm\-yy;@");
			result.Add(@"[$-409]mmm\-yy;@");
			result.Add(@"[$-409]mmmm\-yy;@");
			result.Add(@"[$-409]mmmm\ d\,\ yyyy;@");
			result.Add(@"[$-409]m/d/yy\ h:mm\ AM/PM;@");
			result.Add(@"m/d/yy\ h:mm;@");
			result.Add(@"[$-409]mmmmm;@");
			result.Add(@"[$-409]mmmmm\-yy;@");
			result.Add(@"m/d/yyyy;@");
			result.Add(@"[$-409]d\-mmm\-yyyy;@");
			return result;
		}
		static List<SpreadsheetCommandId> commandIds = CreateCommandIds();
		static List<SpreadsheetCommandId> CreateCommandIds() {
			List<SpreadsheetCommandId> result = new List<SpreadsheetCommandId>();
			result.Add(SpreadsheetCommandId.FormatNumberGeneral);
			result.Add(SpreadsheetCommandId.FormatNumberDecimal);
			result.Add(SpreadsheetCommandId.FormatNumberAccountingCurrency);
			result.Add(SpreadsheetCommandId.FormatNumberAccountingRegular);
			result.Add(SpreadsheetCommandId.FormatNumberShortDate);
			result.Add(SpreadsheetCommandId.FormatNumberLongDate);
			result.Add(SpreadsheetCommandId.FormatNumberTime);
			result.Add(SpreadsheetCommandId.FormatNumberPercentage);
			result.Add(SpreadsheetCommandId.FormatNumberFraction);
			result.Add(SpreadsheetCommandId.FormatNumberScientific);
			result.Add(SpreadsheetCommandId.FormatNumberText);
			return result;
		}
		#endregion
		#region Fields
		FormatCellsModel model;
		string fontStyle;
		Color activeBorderColor;
		XlBorderLineStyle activeBorderLineType;
		bool isOutline;
		bool isNoBorder;
		bool isInside;
		bool isFillColorEmpty;
		bool validNumberFormat;
		int decimalPlaces;
		int selectedTypeIndex;
		string customFormatCode;
		string numberCategory;
		FormatNumberCategory numberCategoryId;
		#endregion
		public FormatCellsViewModel(FormatCellsModel model) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
			InitializeNumber();
			InitializeFontStyle();
			InitializeBorderStateFromSheet();
			InitializeActiveBorderStyle();
			InitializeFill();
		}
		#region Properties
		public FormatCellsFormInitialTabPage InitialTabPage { 
			get {
				FormatCellsFormInitialTabPage result = model.InitialTabPage;
				if (IsProtected && result == FormatCellsFormInitialTabPage.Protection)
					result = FormatCellsFormInitialTabPage.Number;
				return result; 
			} 
		}
		public ISpreadsheetControl Control { get { return model.Control; } }
		protected internal DocumentModelUnitConverter UnitConverter { get { return Control.InnerControl.DocumentModel.UnitConverter; } }
		internal FormatCellsFormProperties FormProperties { get { return model.FormProperties; } }
		internal MergedCellFormat SourceFormat { get { return model.FormProperties.SourceFormat; } }
		internal MergedFormatStringInfo FormatString { get { return SourceFormat.FormatString; } }
		internal string FormatCode { get { return FormatString.FormatString; } }
		internal MergedAlignmentInfo Alignment { get { return SourceFormat.Alignment; } }
		internal MergedBorderInfo Border { get { return SourceFormat.Border; } }
		internal MergedFontInfo Font { get { return SourceFormat.Font; } }
		internal MergedFillInfo Fill { get { return SourceFormat.Fill; } }
		internal MergedCellProtectionInfo Protection { get { return SourceFormat.Protection; } }
		internal MergedBorderOptionsInfo BorderOptions { get { return FormProperties.BorderOptions; } }
		public bool IsProtected { get { return Control.ActiveWorksheet.IsProtected; } }
		#region NumberCategory
		public string NumberCategory {
			get { return numberCategory; }
			set {
				if (numberCategory == value)
					return;
				bool oldNumberDecimalControlVisible = NumberDecimalControlVisible;
				bool oldNumberTypeControlVisible = NumberTypeControlVisible;
				bool oldNumberCustomControlVisible = NumberCustomControlVisible;
				bool oldNegativeNumbersVisible = NegativeNumbersVisible;
				bool oldUseThousandSeparatorVisible = UseThousandSeparatorVisible;
				bool oldSymbolListVisible = SymbolListVisible;
				List<string> oldTypeCaptions = TypeCaptions;
				this.numberCategory = value;
				this.numberCategoryId = CalculateNumberCategoryId(value);
				ValidateSelectedTypeIndex();
				UpdateFormatCode();
				OnPropertyChanged("NumberCategory");
				OnPropertyChanged("NumberDescription");
				OnPropertyChanged("CategoryDescription");
				OnNumberSampleTextChanged();
				if (oldNumberDecimalControlVisible != NumberDecimalControlVisible)
					OnPropertyChanged("NumberDecimalControlVisible");
				if (oldNumberTypeControlVisible != NumberTypeControlVisible)
					OnPropertyChanged("NumberTypeControlVisible");
				if (oldNumberCustomControlVisible != NumberCustomControlVisible)
					OnPropertyChanged("NumberCustomControlVisible");
				if (oldNegativeNumbersVisible != NegativeNumbersVisible)
					OnPropertyChanged("NegativeNumbersVisible");
				if (oldUseThousandSeparatorVisible != UseThousandSeparatorVisible)
					OnPropertyChanged("UseThousandSeparatorVisible");
				if (oldSymbolListVisible != SymbolListVisible)
					OnPropertyChanged("SymbolListVisible");
				if (!Object.ReferenceEquals(oldTypeCaptions, TypeCaptions)) {
					OnPropertyChanged("TypeCaptions");
					OnPropertyChanged("SelectedTypeIndex");
					OnPropertyChanged("SelectedType");
				}
			}
		}
		public List<string> NumberCategories {
			get { return model.NumberCategories; }
		}
		public List<string> TypeCaptions {
			get {
				return GetCurrentTypeCaptions();
			}
		}
		public int DecimalPlaces {
			get { return decimalPlaces; }
			set {
				if (decimalPlaces == value)
					return;
				this.decimalPlaces = value;
				UpdateFormatCode();
				OnPropertyChanged("DecimalPlaces");
				OnNumberSampleTextChanged();
			}
		}
		public int SelectedTypeIndex {
			get { return selectedTypeIndex; }
			set {
				if (value < 0 || selectedTypeIndex == value)
					return;
				this.selectedTypeIndex = value;
				UpdateFormatCode();
				OnPropertyChanged("SelectedTypeIndex");
				OnNumberSampleTextChanged();
			}
		}
		public string SelectedType {
			get { return TypeCaptions != null ? TypeCaptions[selectedTypeIndex] : null; }
		}
		public bool NumberDescriptionVisible {
			get {
				return numberCategoryId == FormatNumberCategory.General ||
					numberCategoryId == FormatNumberCategory.Text;
			}
		}
		public string NumberDescription {
			get {
				if (numberCategoryId == FormatNumberCategory.General)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_NumberDescription_General);
				if (numberCategoryId == FormatNumberCategory.Text)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_NumberDescription_Text);
				return String.Empty;
			}
		}
		public bool NumberDecimalControlVisible {
			get {
				return numberCategoryId == FormatNumberCategory.Number ||
					numberCategoryId == FormatNumberCategory.Currency ||
					numberCategoryId == FormatNumberCategory.Accounting ||
					numberCategoryId == FormatNumberCategory.Percentage ||
					numberCategoryId == FormatNumberCategory.Scientific;
			}
		}
		public bool NumberTypeControlVisible {
			get {
				return numberCategoryId == FormatNumberCategory.Date ||
					numberCategoryId == FormatNumberCategory.Time ||
					numberCategoryId == FormatNumberCategory.Fraction;
			}
		}
		public bool NumberCustomControlVisible {
			get {
				return numberCategoryId == FormatNumberCategory.Custom;
			}
		}
		public bool UseThousandSeparatorVisible {
			get {
				return false;
			}
		}
		public bool SymbolListVisible {
			get {
				return numberCategoryId == FormatNumberCategory.Currency ||
					numberCategoryId == FormatNumberCategory.Accounting;
			}
		}
		public bool NegativeNumbersVisible {
			get {
				return false;
			}
		}
		public string CustomFormatCode {
			get { return customFormatCode; }
			set {
				if (customFormatCode == value)
					return;
				customFormatCode = value;
				UpdateFormatCode();
				OnPropertyChanged("CustomFormatCode");
				OnNumberSampleTextChanged();
			}
		}
		public string NumberSampleText {
			get {
				if (FormatString.FormatString == null)
					return String.Empty;
				CellPosition activeCellPosition = SourceFormat.ActiveCellPosition;
				ICell activeCell = Control.InnerControl.DocumentModel.ActiveSheet.GetCellForFormatting(activeCellPosition.Column, activeCellPosition.Row);
				return activeCell.GetFormatResultCore(CreateNumberFormat(), NumberFormatParameters.Empty).Text;
			}
		}
		void OnNumberSampleTextChanged() {
			OnPropertyChanged("NumberSampleText");
		}
		protected internal NumberFormat CreateNumberFormat() {
			return new NumberFormat(-1, FormatString.FormatString);
		}
		public string CategoryDescription {
			get {
				if (numberCategoryId == FormatNumberCategory.Number)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Number);
				if (numberCategoryId == FormatNumberCategory.Currency)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Currency);
				if (numberCategoryId == FormatNumberCategory.Accounting)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Accounting);
				if (numberCategoryId == FormatNumberCategory.Date)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Date);
				if (numberCategoryId == FormatNumberCategory.Time)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Time);
				if (numberCategoryId == FormatNumberCategory.Percentage)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Percentage);
				if (numberCategoryId == FormatNumberCategory.Custom)
					return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_CategoryDescription_Custom);
				return String.Empty;
			}
		}
		#endregion
		#region Alignment
		#region HorizontalAlignment
		public List<string> HorizontalAlignments {
			get { return new List<string>(model.HorizontalAlignmentTable.Values); }
		}
		public string HorizontalAlignment {
			get { return GetHorizontalAlignmentStringByEnum(Alignment.Horizontal); }
			set {
				if (HorizontalAlignment == value)
					return;
				bool oldJustifyDistributedEnabled = JustifyDistributedEnabled;
				bool oldShrinkToFitEnabled = ShrinkToFitEnabled;
				Alignment.Horizontal = GetHorizontalAlignmentByString(value);
				OnPropertyChanged("HorizontalAlignment");
				if (oldJustifyDistributedEnabled != JustifyDistributedEnabled) {
					OnPropertyChanged("JustifyDistributedEnabled");
					if (oldJustifyDistributedEnabled)
						JustifyDistributed = CheckState.Unchecked;
				}
				if (oldShrinkToFitEnabled != ShrinkToFitEnabled)
					OnPropertyChanged("ShrinkToFitEnabled");
			}
		}
		string GetHorizontalAlignmentStringByEnum(XlHorizontalAlignment? alignment) {
			if (!alignment.HasValue)
				return null;
			return model.HorizontalAlignmentTable[alignment.Value];
		}
		XlHorizontalAlignment? GetHorizontalAlignmentByString(string alignment) {
			if (String.IsNullOrEmpty(alignment))
				return null;
			foreach (XlHorizontalAlignment key in model.HorizontalAlignmentTable.Keys)
				if (model.HorizontalAlignmentTable[key] == alignment)
					return key;
			Exceptions.ThrowInternalException();
			return XlHorizontalAlignment.Center;
		}
		#endregion
		#region VerticalAlignment
		public List<string> VerticalAlignments {
			get { return new List<string>(model.VerticalAlignmentTable.Values); }
		}
		public string VerticalAlignment {
			get { return GetVerticalAlignmentStringByEnum(Alignment.Vertical); }
			set {
				if (VerticalAlignment == value)
					return;
				bool oldShrinkToFitEnabled = ShrinkToFitEnabled;
				Alignment.Vertical = GetVerticalAlignmentByString(value);
				OnPropertyChanged("VerticalAlignment");
				if (oldShrinkToFitEnabled != ShrinkToFitEnabled)
					OnPropertyChanged("ShrinkToFitEnabled");
			}
		}
		string GetVerticalAlignmentStringByEnum(XlVerticalAlignment? alignment) {
			if (!alignment.HasValue)
				return null;
			return model.VerticalAlignmentTable[alignment.Value];
		}
		XlVerticalAlignment? GetVerticalAlignmentByString(string alignment) {
			if (String.IsNullOrEmpty(alignment))
				return null;
			foreach (XlVerticalAlignment key in model.VerticalAlignmentTable.Keys)
				if (model.VerticalAlignmentTable[key] == alignment)
					return key;
			Exceptions.ThrowInternalException();
			return XlVerticalAlignment.Center;
		}
		#endregion
		#region TextDirection
		public List<string> TextDirections {
			get { return new List<string>(model.TextDirectionTable.Values); }
		}
		public string TextDirection {
			get { return GetTextDirectiontStringByEnum(Alignment.ReadingOrder); }
			set {
				if (TextDirection == value)
					return;
				Alignment.ReadingOrder = GetTextDirectiontByString(value);
				OnPropertyChanged("TextDirection");
			}
		}
		string GetTextDirectiontStringByEnum(XlReadingOrder? textDirection) {
			if (!textDirection.HasValue)
				return null;
			return model.TextDirectionTable[textDirection.Value];
		}
		XlReadingOrder? GetTextDirectiontByString(string textDirection) {
			if (String.IsNullOrEmpty(textDirection))
				return null;
			foreach (XlReadingOrder key in model.TextDirectionTable.Keys)
				if (model.TextDirectionTable[key] == textDirection)
					return key;
			Exceptions.ThrowInternalException();
			return XlReadingOrder.Context;
		}
		#endregion
		#region JustifyDistributed
		public bool JustifyDistributedIndeterminate {
			get { return !Alignment.JustifyLastLine.HasValue; }
		}
		public CheckState JustifyDistributed {
			get {
				if (Alignment.JustifyLastLine.HasValue)
					return Alignment.JustifyLastLine.Value ? CheckState.Checked : CheckState.Unchecked;
				return CheckState.Indeterminate;
			}
			set {
				if (JustifyDistributed == value)
					return;
				if (value == CheckState.Indeterminate)
					Alignment.JustifyLastLine = null;
				else
					Alignment.JustifyLastLine = value == CheckState.Checked;
				OnPropertyChanged("JustifyDistributed");
			}
		}
		public bool JustifyDistributedEnabled {
			get { return Alignment.Horizontal == XlHorizontalAlignment.Distributed && Indent == 0; }
		}
		#endregion
		#region WrapText
		public bool WrapTextIndeterminate {
			get { return !Alignment.WrapText.HasValue; }
		}
		public CheckState WrapText {
			get {
				if (Alignment.WrapText.HasValue)
					return Alignment.WrapText.Value ? CheckState.Checked : CheckState.Unchecked;
				return CheckState.Indeterminate;
			}
			set {
				if (WrapText == value)
					return;
				bool oldShrinkToFitEnabled = ShrinkToFitEnabled;
				if (value == CheckState.Indeterminate)
					Alignment.WrapText = null;
				else
					Alignment.WrapText = value == CheckState.Checked;
				OnPropertyChanged("WrapText");
				if (oldShrinkToFitEnabled != ShrinkToFitEnabled)
					OnPropertyChanged("ShrinkToFitEnabled");
			}
		}
		#endregion
		#region ShrinkToFit
		public bool ShrinkToFitIndeterminate {
			get { return !Alignment.ShrinkToFit.HasValue; }
		}
		public CheckState ShrinkToFit {
			get {
				if (Alignment.ShrinkToFit.HasValue)
					return Alignment.ShrinkToFit.Value ? CheckState.Checked : CheckState.Unchecked;
				return CheckState.Indeterminate;
			}
			set {
				if (ShrinkToFit == value)
					return;
				if (value == CheckState.Indeterminate)
					Alignment.ShrinkToFit = null;
				else
					Alignment.ShrinkToFit = value == CheckState.Checked;
				OnPropertyChanged("ShrinkToFit");
			}
		}
		public bool ShrinkToFitEnabled {
			get {
				XlHorizontalAlignment? horizontalAlignment = Alignment.Horizontal;
				XlVerticalAlignment? verticalAlignment = Alignment.Vertical;
				return WrapText == CheckState.Unchecked && horizontalAlignment != XlHorizontalAlignment.Fill &&
					horizontalAlignment != XlHorizontalAlignment.Justify && horizontalAlignment != XlHorizontalAlignment.Distributed &&
					verticalAlignment != XlVerticalAlignment.Justify && verticalAlignment != XlVerticalAlignment.Distributed;
			}
		}
		#endregion
		#region MergeCells
		public bool MergeCellsIndeterminate {
			get {
				if (FormProperties.MergeCells == CheckState.Indeterminate)
					return true;
				else
					return false;
			}
		}
		public CheckState MergeCells {
			get {
				return FormProperties.MergeCells;
			}
			set {
				if (MergeCells == value)
					return;
				FormProperties.MergeCells = value;
				OnPropertyChanged("MergedCell");
			}
		}
		#endregion
		public byte? Indent {
			get { return Alignment.Indent; }
			set {
				if (Indent == value)
					return;
				bool oldJustifyDistributedEnabled = JustifyDistributedEnabled;
				Alignment.Indent = value;
				XlHorizontalAlignment? horizontalAlignment = Alignment.Horizontal;
				if (Indent > 0 && (horizontalAlignment != XlHorizontalAlignment.Left && horizontalAlignment != XlHorizontalAlignment.Right
					&& horizontalAlignment != XlHorizontalAlignment.Distributed))
					SourceFormat.Alignment.Horizontal = XlHorizontalAlignment.Left;
				OnPropertyChanged("Indent");
				if (oldJustifyDistributedEnabled != JustifyDistributedEnabled) {
					OnPropertyChanged("JustifyDistributedEnabled");
					if (oldJustifyDistributedEnabled)
						JustifyDistributed = CheckState.Unchecked;
				}
			}
		}
		public bool OrientationEnabled {
			get {
				XlHorizontalAlignment? horizontalAlignment = Alignment.Horizontal;
				return horizontalAlignment != XlHorizontalAlignment.Fill && horizontalAlignment != XlHorizontalAlignment.CenterContinuous;
			}
		}
		#endregion
		#region Font
		public string FontName {
			get {
				return Font.Name;
			}
			set {
				if (FontName == value)
					return;
				Font.Name = value;
				OnPropertyChanged("FontName");
			}
		}
		public List<string> FontNames {
			get { return model.FontNames; }
		}
		public string FontStyle {
			get { return fontStyle; }
			set {
				if (FontStyle == value)
					return;
				if (value == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Regular)) {
					fontStyle = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Regular);
					Font.Bold = false;
					Font.Italic = false;
				}
				else if (value == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Italic)) {
					fontStyle = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Italic);
					Font.Bold = false;
					Font.Italic = true;
				}
				else if (value == XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Bold)) {
					fontStyle = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Bold);
					Font.Bold = true;
					Font.Italic = false;
				}
				else {
					fontStyle = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_BoldItalic);
					Font.Bold = true;
					Font.Italic = true;
				}
				OnPropertyChanged("FontStyle");
			}
		}
		public List<string> FontStyles {
			get { return model.FontStyles; }
		}
		public double? Size {
			get { return Font.Size; }
			set {
				if (Size == value)
					return;
				Font.Size = value; 
				OnPropertyChanged("Size");
			}
		}
		public List<double> FontSizes {
			get { return model.FontSizes; }
		}
		#region StrikeThrough
		public bool StrikeThroughIndeterminate {
			get { return !Font.StrikeThrough.HasValue; }
		}
		public CheckState StrikeThrough {
			get {
				if (Font.StrikeThrough.HasValue)
					return Font.StrikeThrough.Value ? CheckState.Checked : CheckState.Unchecked;
				return CheckState.Indeterminate;
			}
			set {
				if (StrikeThrough == value)
					return;
				if (value == CheckState.Indeterminate)
					Font.StrikeThrough = null;
				else
					Font.StrikeThrough = value == CheckState.Checked;
				OnPropertyChanged("StrikeThrough");
			}
		}
		#endregion
		#region Script
		public bool SuperscriptIndeterminate {
			get { return !Font.Script.HasValue; }
		}
		public CheckState Superscript {
			get {
				if (Font.Script.HasValue)
					return Font.Script == XlScriptType.Superscript ? CheckState.Checked : CheckState.Unchecked;
				return CheckState.Indeterminate;
			}
			set {
				if (Superscript == value)
					return;
				if (value == CheckState.Indeterminate)
					Font.Script = null;
				else
					Font.Script = value == CheckState.Checked ? XlScriptType.Superscript : XlScriptType.Baseline;
				OnPropertyChanged("Superscript");
			}
		}
		public bool Subscript {
			get { return Font.Script == XlScriptType.Subscript; }
			set {
				if (Subscript == value)
					return;
				Font.Script = value ? XlScriptType.Subscript : XlScriptType.Baseline;
				OnPropertyChanged("Subscript");
			}
		}
		#endregion
		#region Underline
		public List<string> Underlines {
			get { return new List<string>(model.UnderlineTable.Values); }
		}
		public string Underline {
			get { return GetUnderlineStringByEnum(Font.Underline); }
			set {
				if (Underline == value)
					return;
				Font.Underline = GetUnderlineByString(value);
				OnPropertyChanged("Underline");
			}
		}
		string GetUnderlineStringByEnum(XlUnderlineType? underline) {
			if (!underline.HasValue)
				return null;
			return model.UnderlineTable[underline.Value];
		}
		protected internal XlUnderlineType? GetUnderlineByString(string underline) {
			if (String.IsNullOrEmpty(underline))
				return null;
			foreach (XlUnderlineType key in model.UnderlineTable.Keys)
				if (model.UnderlineTable[key] == underline)
					return key;
			Exceptions.ThrowInternalException();
			return XlUnderlineType.None;
		}
		public bool IsUnderlineNone() {
			return Font.Underline == XlUnderlineType.None;
		}
		public bool IsUnderlineSingle() {
			return Font.Underline == XlUnderlineType.Single ||
				Font.Underline == XlUnderlineType.SingleAccounting;
		}
		public bool IsUnderlineDouble() {
			return Font.Underline == XlUnderlineType.Double ||
				Font.Underline == XlUnderlineType.DoubleAccounting;
		}
		#endregion
		public Color? ColorFont {
			get { return (!Font.Color.HasValue || Font.Color == DXColor.Empty) ? DXColor.Black : Font.Color; }
			set {
				if (ColorFont == value)
					return;
				Font.Color = value;
				OnPropertyChanged("ColorFont");
			}
		}
		#endregion
		#region Border
		public bool IsOutline {
			get { return isOutline; }
			set {
				if (IsOutline == value)
					return;
				isOutline = value;
				OnPropertyChanged("IsOutline");
			}
		}
		public bool IsNoBorder {
			get { return isNoBorder; }
			set {
				if (IsNoBorder == value)
					return;
				isNoBorder = value;
			}
		}
		public bool IsInside {
			get { return isInside; }
			set {
				if (IsInside == value)
					return;
				isInside = value;
			}
		}
		bool GetBorderOptions(bool? value) {
			return value.HasValue ? value.Value : false;
		}
		public bool ApplyLeftBorder {
			get { return GetBorderOptions(BorderOptions.ApplyLeftBorder); }
			set {
				if (ApplyLeftBorder != value)
					SetApplyBorder("ApplyLeftBorder", LeftLineStyle, LeftColor, IsOutline, SetBorderLeftLineStyle, SetBorderLeftColor, SetApplyLeftBorder);
			}
		}
		public bool ApplyRightBorder {
			get { return GetBorderOptions(BorderOptions.ApplyRightBorder); }
			set {
				if (ApplyRightBorder != value)
					SetApplyBorder("ApplyRightBorder", RightLineStyle, RightColor, IsOutline, SetBorderRightLineStyle, SetBorderRightColor, SetApplyRightBorder);
			}
		}
		public bool ApplyTopBorder {
			get { return GetBorderOptions(BorderOptions.ApplyTopBorder); }
			set {
				if (ApplyTopBorder != value)
					SetApplyBorder("ApplyTopBorder", TopLineStyle, TopColor, IsOutline, SetBorderTopLineStyle, SetBorderTopColor, SetApplyTopBorder);
			}
		}
		public bool ApplyBottomBorder {
			get { return GetBorderOptions(BorderOptions.ApplyBottomBorder); }
			set {
				if (ApplyBottomBorder != value)
					SetApplyBorder("ApplyBottomBorder", BottomLineStyle, BottomColor, IsOutline, SetBorderBottomLineStyle, SetBorderBottomColor, SetApplyBottomBorder);
			}
		}
		public bool ApplyVerticalBorder {
			get { return GetBorderOptions(BorderOptions.ApplyVerticalBorder); }
			set {
				if (ApplyVerticalBorder != value)
					SetApplyBorder("ApplyVerticalBorder", VerticalLineStyle, VerticalColor, IsInside, SetBorderVerticalLineStyle, SetBorderVerticalColor, SetApplyVerticalBorder);
			}
		}
		public bool ApplyHorizontalBorder {
			get { return GetBorderOptions(BorderOptions.ApplyHorizontalBorder); }
			set {
				if (ApplyHorizontalBorder != value)
					SetApplyBorder("ApplyHorizontalBorder", HorizontalLineStyle, HorizontalColor, IsInside, SetBorderHorizontalLineStyle, SetBorderHorizontalColor, SetApplyHorizontalBorder);
			}
		}
		public bool ApplyDiagonalUpBorder {
			get { return GetBorderOptions(BorderOptions.ApplyDiagonalUpBorder); }
			set {
				if (ApplyDiagonalUpBorder != value)
					SetApplyBorder("ApplyDiagonalUpBorder", DiagonalUpLineStyle, DiagonalColor, false, SetBorderDiagonalUpLineStyle, SetBorderDiagonalColor, SetApplyDiagonalUpBorder);
			}
		}
		public bool ApplyDiagonalDownBorder {
			get { return GetBorderOptions(BorderOptions.ApplyDiagonalDownBorder); }
			set {
				if (ApplyDiagonalDownBorder != value)
					SetApplyBorder("ApplyDiagonalDownBorder", DiagonalDownLineStyle, DiagonalColor, false, SetBorderDiagonalDownLineStyle, SetBorderDiagonalColor, SetApplyDiagonalDownBorder);
			}
		}
		public XlBorderLineStyle LineStyle {
			get { return activeBorderLineType; }
			set {
				if (LineStyle == value)
					return;
				activeBorderLineType = value;
				OnPropertyChanged("LineType");
			}
		}
		public Color Color {
			get { return GetNotEmptyColor(activeBorderColor); }
			set {
				if (activeBorderColor == value)
					return;
				activeBorderColor = value;
				OnPropertyChanged("Color");
			}
		}
		public XlBorderLineStyle? LeftLineStyle { get { return Border.LeftLineStyle; } set { SetBorderLeftLineStyle(value); } }
		public XlBorderLineStyle? TopLineStyle { get { return Border.TopLineStyle; } set { SetBorderTopLineStyle(value); } }
		public XlBorderLineStyle? BottomLineStyle { get { return Border.BottomLineStyle; } set { SetBorderBottomLineStyle(value); } }
		public XlBorderLineStyle? RightLineStyle { get { return Border.RightLineStyle; } set { SetBorderRightLineStyle(value); } }
		public XlBorderLineStyle? DiagonalDownLineStyle { get { return Border.DiagonalDownLineStyle; } set { SetBorderDiagonalDownLineStyle(value); } }
		public XlBorderLineStyle? DiagonalUpLineStyle { get { return Border.DiagonalUpLineStyle; } set { SetBorderDiagonalUpLineStyle(value); } }
		public XlBorderLineStyle? HorizontalLineStyle { get { return Border.HorizontalLineStyle; } set { SetBorderHorizontalLineStyle(value); } }
		public XlBorderLineStyle? VerticalLineStyle { get { return Border.VerticalLineStyle; } set { SetBorderVerticalLineStyle(value); } }
		public Color LeftColor { get { return GetNotEmptyColor(Border.LeftColor); } set { SetBorderLeftColor(value); } }
		public Color TopColor { get { return GetNotEmptyColor(Border.TopColor); } set { SetBorderTopColor(value); } }
		public Color BottomColor { get { return GetNotEmptyColor(Border.BottomColor); } set { SetBorderBottomColor(value); } }
		public Color RightColor { get { return GetNotEmptyColor(Border.RightColor); } set { SetBorderRightColor(value); } }
		public Color DiagonalColor { get { return GetNotEmptyColor(Border.DiagonalColor); } set { SetBorderDiagonalColor(value); } }
		public Color HorizontalColor { get { return GetNotEmptyColor(Border.HorizontalColor); } set { SetBorderHorizontalColor(value); } }
		public Color VerticalColor { get { return GetNotEmptyColor(Border.VerticalColor); } set { SetBorderVerticalColor(value); } }
		public SelectedRangeTypeForBorderPreview RangeType {
			get { return FormProperties.RangeType; }
		}
		#region For WPF
		public bool EnableVerticalBorderCheckState {
			get { return RangeType == SelectedRangeTypeForBorderPreview.Row || RangeType == SelectedRangeTypeForBorderPreview.Table; }
		}
		public bool EnableHorizontalBorderCheckState {
			get { return RangeType == SelectedRangeTypeForBorderPreview.Column || RangeType == SelectedRangeTypeForBorderPreview.Table; }
		}
		public bool EnableInsideBorderButtonState {
			get { return RangeType == SelectedRangeTypeForBorderPreview.Column || RangeType == SelectedRangeTypeForBorderPreview.Row || RangeType == SelectedRangeTypeForBorderPreview.Table; }
		}
		#endregion
		#endregion
		#region Fill
		public bool IsFillColorEmpty {
			get { return isFillColorEmpty; }
			set {
				if (IsFillColorEmpty == value)
					return;
				isFillColorEmpty = value;
				if (isFillColorEmpty)
					FillBackColor = DXColor.Empty;
				OnPropertyChanged("IsFillColorEmpty");
			}
		}
		public XlPatternType? PatternType {
			get {
				return Fill.PatternType;
			}
			set {
				if (PatternType == value)
					return;
				Fill.PatternType = value;
				OnPropertyChanged("PatternType");
			}
		}
		public Color? FillForeColor {
			get {
				return Fill.ForeColor;
			}
			set {
				if (Fill.ForeColor == value)
					return;
				Fill.ForeColor = value;
				OnPropertyChanged("FillForeColor");
			}
		}
		public Color? FillBackColor {
			get {
				return Fill.BackColor;
			}
			set {
				if (Fill.BackColor == value)
					return;
				Fill.BackColor = value;
				if (Fill.BackColor != DXColor.Empty)
					IsFillColorEmpty = false;
				else
					IsFillColorEmpty = true;
				OnPropertyChanged("FillBackColor");
			}
		}
		public Dictionary<XlPatternType, HatchStyle> BrushHatchStyle {
			get { return model.BrushHatchStyles; }
		}
		#endregion
		#region Protection
		public bool LockedIndeterminate {
			get { return !Protection.Locked.HasValue; }
		}
		public CheckState Locked {
			get {
				if (Protection.Locked.HasValue)
					return Protection.Locked.Value ? CheckState.Checked : CheckState.Unchecked;
				return CheckState.Indeterminate;
			}
			set {
				if (Locked == value)
					return;
				if (value == CheckState.Indeterminate)
					Protection.Locked = null;
				else
					Protection.Locked = value == CheckState.Checked;
				OnPropertyChanged("Locked");
			}
		}
		public bool HiddenIndeterminate {
			get { return !Protection.Hidden.HasValue; }
		}
		public CheckState Hidden {
			get {
				if (Protection.Hidden.HasValue)
					return Protection.Hidden.Value ? CheckState.Checked : CheckState.Unchecked;
				return CheckState.Indeterminate;
			}
			set {
				if (Hidden == value)
					return;
				if (value == CheckState.Indeterminate)
					Protection.Hidden = null;
				else
					Protection.Hidden = value == CheckState.Checked;
				OnPropertyChanged("Hidden");
			}
		}
		#endregion
		#endregion
		#region SetBorderLineStyle
		void SetBorderLineStyle(string propertyName, Action<XlBorderLineStyle?> setter, XlBorderLineStyle? oldValue, XlBorderLineStyle? newValue) {
			if (oldValue.Equals(newValue))
				return;
			setter(newValue);
			OnPropertyChanged(propertyName);
		}
		void SetBorderLeftLineStyle(XlBorderLineStyle? value) {
			SetBorderLineStyle("LeftLineStyle", SetBorderLeftLineStyleCore, LeftLineStyle, value);
		}
		void SetBorderRightLineStyle(XlBorderLineStyle? value) {
			SetBorderLineStyle("RightLineStyle", SetBorderRightLineStyleCore, RightLineStyle, value);
		}
		void SetBorderTopLineStyle(XlBorderLineStyle? value) {
			SetBorderLineStyle("TopLineStyle", SetBorderTopLineStyleCore, TopLineStyle, value);
		}
		void SetBorderBottomLineStyle(XlBorderLineStyle? value) {
			SetBorderLineStyle("BottomLineStyle", SetBorderBottomLineStyleCore, BottomLineStyle, value);
		}
		void SetBorderDiagonalUpLineStyle(XlBorderLineStyle? value) {
			SetBorderLineStyle("DiagonalUpLineStyle", SetBorderDiagonalUpLineStyleCore, DiagonalUpLineStyle, value);
		}
		void SetBorderDiagonalDownLineStyle(XlBorderLineStyle? value) {
			SetBorderLineStyle("DiagonalDownLineStyle", SetBorderDiagonalDownLineStyleCore, DiagonalDownLineStyle, value);
		}
		void SetBorderHorizontalLineStyle(XlBorderLineStyle? value) {
			SetBorderLineStyle("HorizontalLineStyle", SetBorderHorizontalLineStyleCore, HorizontalLineStyle, value);
		}
		void SetBorderVerticalLineStyle(XlBorderLineStyle? value) {
			SetBorderLineStyle("VerticalLineStyle", SetBorderVerticalLineStyleCore, VerticalLineStyle, value);
		}
		void SetBorderLeftLineStyleCore(XlBorderLineStyle? value) {
			Border.LeftLineStyle = value;
		}
		void SetBorderRightLineStyleCore(XlBorderLineStyle? value) {
			Border.RightLineStyle = value;
		}
		void SetBorderTopLineStyleCore(XlBorderLineStyle? value) {
			Border.TopLineStyle = value;
		}
		void SetBorderBottomLineStyleCore(XlBorderLineStyle? value) {
			Border.BottomLineStyle = value;
		}
		void SetBorderVerticalLineStyleCore(XlBorderLineStyle? value) {
			Border.VerticalLineStyle = value;
		}
		void SetBorderHorizontalLineStyleCore(XlBorderLineStyle? value) {
			Border.HorizontalLineStyle = value;
		}
		void SetBorderDiagonalUpLineStyleCore(XlBorderLineStyle? value) {
			Border.DiagonalUpLineStyle = value;
			if (Border.DiagonalDownLineStyle.HasValue && Border.DiagonalDownLineStyle != value && value.HasValue && Border.DiagonalDownLineStyle.Value != XlBorderLineStyle.None && value != XlBorderLineStyle.None) {
				SetBorderLineStyle("DiagonalDownLineStyle", SetBorderDiagonalDownLineStyleCore, DiagonalDownLineStyle, value);
				BorderOptions.ApplyDiagonalDownBorder = true;
			}
		}
		void SetBorderDiagonalDownLineStyleCore(XlBorderLineStyle? value) {
			Border.DiagonalDownLineStyle = value;
			if (Border.DiagonalUpLineStyle.HasValue && Border.DiagonalUpLineStyle != value && value.HasValue && Border.DiagonalUpLineStyle.Value != XlBorderLineStyle.None && value != XlBorderLineStyle.None) {
				SetBorderLineStyle("DiagonalUpLineStyle", SetBorderDiagonalUpLineStyleCore, DiagonalUpLineStyle, value);
				BorderOptions.ApplyDiagonalUpBorder = true;
			}
		}
		#endregion
		#region SetBorderColor
		void SetBorderColor(string propertyName, Action<Color> setter, Color oldValue, Color newValue) {
			if (oldValue == newValue)
				return;
			setter(newValue);
			OnPropertyChanged(propertyName);
		}
		void SetBorderLeftColor(Color value) {
			SetBorderColor("LeftColor", SetBorderLeftColorCore, LeftColor, value);
		}
		void SetBorderRightColor(Color value) {
			SetBorderColor("RightColor", SetBorderRightColorCore, RightColor, value);
		}
		void SetBorderTopColor(Color value) {
			SetBorderColor("TopColor", SetBorderTopColorCore, TopColor, value);
		}
		void SetBorderBottomColor(Color value) {
			SetBorderColor("BottomColor", SetBorderBottomColorCore, BottomColor, value);
		}
		void SetBorderDiagonalColor(Color value) {
			SetBorderColor("DiagonalColor", SetBorderDiagonalColorCore, DiagonalColor, value);
		}
		void SetBorderHorizontalColor(Color value) {
			SetBorderColor("HorizontalColor", SetBorderHorizontalColorCore, HorizontalColor, value);
		}
		void SetBorderVerticalColor(Color value) {
			SetBorderColor("VerticalColor", SetBorderVerticalColorCore, VerticalColor, value);
		}
		void SetBorderLeftColorCore(Color value) {
			Border.LeftColor = value;
		}
		void SetBorderRightColorCore(Color value) {
			Border.RightColor = value;
		}
		void SetBorderTopColorCore(Color value) {
			Border.TopColor = value;
		}
		void SetBorderBottomColorCore(Color value) {
			Border.BottomColor = value;
		}
		void SetBorderVerticalColorCore(Color value) {
			Border.VerticalColor = value;
		}
		void SetBorderHorizontalColorCore(Color value) {
			Border.HorizontalColor = value;
		}
		void SetBorderDiagonalColorCore(Color value) {
			Border.DiagonalColor = value;
		}
		#endregion
		#region SetApplyBorder
		void SetApplyBorder(string propertyApplyName, XlBorderLineStyle? style, Color color, bool isInsideOrOutline, Action<XlBorderLineStyle?> setBorderLineStyle, Action<Color> setBorderColor, Action<bool> setApplyBorder) {
			if (CheckSetBorder(isInsideOrOutline, style, color)) {
				setApplyBorder(true);
				setBorderLineStyle(LineStyle);
				setBorderColor(Color);
			}
			else if (CheckResetBorder(style, color)) {
				setApplyBorder(false);
				setBorderLineStyle(XlBorderLineStyle.None);
				setBorderColor(DXColor.Empty);
			}
			OnPropertyChanged(propertyApplyName);
		}
		bool CheckSetBorder(bool isInsideOrOutline, XlBorderLineStyle? style, Color color) {
			return !IsNoBorder && LineStyle != XlBorderLineStyle.None && (isInsideOrOutline || LineStyle != style || Color != color);
		}
		bool CheckResetBorder(XlBorderLineStyle? style, Color color) {
			return IsNoBorder || LineStyle == XlBorderLineStyle.None || (LineStyle == style && Color == color);
		}
		void SetApplyLeftBorder(bool value) {
			BorderOptions.ApplyLeftBorder = value;
		}
		void SetApplyRightBorder(bool value) {
			BorderOptions.ApplyRightBorder = value;
		}
		void SetApplyTopBorder(bool value) {
			BorderOptions.ApplyTopBorder = value;
		}
		void SetApplyBottomBorder(bool value) {
			BorderOptions.ApplyBottomBorder = value;
		}
		void SetApplyVerticalBorder(bool value) {
			BorderOptions.ApplyVerticalBorder = value;
		}
		void SetApplyHorizontalBorder(bool value) {
			BorderOptions.ApplyHorizontalBorder = value;
		}
		void SetApplyDiagonalUpBorder(bool value) {
			BorderOptions.ApplyDiagonalUpBorder = value;
		}
		void SetApplyDiagonalDownBorder(bool value) {
			BorderOptions.ApplyDiagonalDownBorder = value;
		}
		#endregion
		protected internal void InitializeNumber() {
			this.decimalPlaces = CalculateDecimalPlaces();
			this.numberCategory = FindNumberCategory(FormatCode);
			this.numberCategoryId = CalculateNumberCategoryId(numberCategory);
			InitializeCustomFormatCode();
			this.validNumberFormat = true;
		}
		int CalculateDecimalPlaces() {
			if (!NumberDecimalControlVisible)
				return defaultDecimalPlaces;
			int separatorIndex = FormatCode.LastIndexOf('.');
			if (separatorIndex == -1)
				return defaultDecimalPlaces;
			return FormatCode.Length - separatorIndex - 1;
		}
		string FindNumberCategory(string formatCode) {
			foreach (SpreadsheetCommandId commandId in commandIds) {
				ChangeSelectedCellsNumberFormatCommand command = Control.CreateCommand(commandId) as ChangeSelectedCellsNumberFormatCommand;
				if (command == null)
					continue;
				if (StringExtensions.CompareInvariantCultureIgnoreCase(command.FormatString, formatCode) == 0) {
					if (command is FormatNumberShortDateCommand || command is FormatNumberLongDateCommand)
						return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_FormatNumberDate);
					return command.MenuCaption;
				}
			}
			return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_FormatNumberCustom);
		}
		FormatNumberCategory CalculateNumberCategoryId(string numberCategory) {
			string general = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberGeneral);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, general) == 0)
				return FormatNumberCategory.General;
			string number = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberDecimal);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, number) == 0)
				return FormatNumberCategory.Number;
			string currency = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingCurrency);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, currency) == 0)
				return FormatNumberCategory.Currency;
			string accounting = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccounting);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, accounting) == 0)
				return FormatNumberCategory.Accounting;
			string date = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_FormatNumberDate);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, date) == 0)
				return FormatNumberCategory.Date;
			string time = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberTime);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, time) == 0)
				return FormatNumberCategory.Time;
			string percentage = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberPercentage);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, percentage) == 0)
				return FormatNumberCategory.Percentage;
			string fraction = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberFraction);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, fraction) == 0)
				return FormatNumberCategory.Fraction;
			string scientific = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberScientific);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, scientific) == 0)
				return FormatNumberCategory.Scientific;
			string text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberText);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, text) == 0)
				return FormatNumberCategory.Text;
			string custom = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_FormatNumberCustom);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(numberCategory, custom) == 0)
				return FormatNumberCategory.Custom;
			Exceptions.ThrowInternalException();
			return FormatNumberCategory.General;
		}
		void InitializeCustomFormatCode() {
			if (numberCategoryId == FormatNumberCategory.Custom)
				this.customFormatCode = NumberFormatParser.Parse(FormatCode).GetFormatCode(this.Control.Document.Options.Culture);
		}
		string CalculateFormatStringWithDecimalPlaces(string baseFormatString) {
			return CalculateFormatStringWithDecimalPlaces(baseFormatString, '0', true);
		}
		string CalculateFormatStringWithDecimalPlaces(string baseFormatString, char symbol, bool addSeparator) {
			string result = baseFormatString;
			if (addSeparator && decimalPlaces > 0)
				result += ".";
			for (int i = 0; i < decimalPlaces; i++) {
				result += symbol;
			}
			return result;
		}
		void ValidateSelectedTypeIndex() {
			if (TypeCaptions != null && TypeCaptions.Count <= selectedTypeIndex)
				SelectedTypeIndex = 0;
		}
		void UpdateFormatCode() {
			string formatString = CalculateFormatString();
			validNumberFormat = formatString != null;
			FormatString.FormatString = formatString;
		}
		string CalculateFormatString() {
			if (numberCategoryId == FormatNumberCategory.General)
				return NumberFormat.Generic.FormatCode;
			if (numberCategoryId == FormatNumberCategory.Number)
				return CalculateNumberFormatCode();
			if (numberCategoryId == FormatNumberCategory.Currency)
				return CalculateCurrencyFormatCode();
			if (numberCategoryId == FormatNumberCategory.Accounting)
				return CalculateAccountingFormatCode();
			if (numberCategoryId == FormatNumberCategory.Date)
				return CalculateDateFormatCode();
			if (numberCategoryId == FormatNumberCategory.Time)
				return CalculateTimeFormatCode();
			if (numberCategoryId == FormatNumberCategory.Percentage)
				return CalculatePercentageFormatCode();
			if (numberCategoryId == FormatNumberCategory.Fraction)
				return CalculateFractionFormatCode();
			if (numberCategoryId == FormatNumberCategory.Scientific)
				return CalculateScientificFormatCode();
			if (numberCategoryId == FormatNumberCategory.Text)
				return CalculateTextFormatCode();
			if (numberCategoryId == FormatNumberCategory.Custom)
				return CalculateCustomFormatCode();
			return String.Empty;
		}
		string CalculateNumberFormatCode() {
			return CalculateFormatStringWithDecimalPlaces(baseNumberFormatString);
		}
		string CalculateCurrencyFormatCode() {
			return CalculateFormatStringWithDecimalPlaces(baseCurrencyFormatString);
		}
		string CalculateAccountingFormatCode() {
			string zeroSymbols = CalculateFormatStringWithDecimalPlaces(String.Empty);
			string questionSymbols = CalculateFormatStringWithDecimalPlaces(String.Empty, '?', false);
			return String.Format(baseAccountingFormatString, zeroSymbols, zeroSymbols, questionSymbols);
		}
		string CalculateDateFormatCode() {
			return dateFormatCodes[selectedTypeIndex];
		}
		string CalculateTimeFormatCode() {
			return timeFormatCodes[selectedTypeIndex];
		}
		string CalculatePercentageFormatCode() {
			string result = CalculateFormatStringWithDecimalPlaces(baseNumberFormatString);
			return result + '%';
		}
		string CalculateFractionFormatCode() {
			return fractionFormatCodes[selectedTypeIndex];
		}
		string CalculateScientificFormatCode() {
			string result = CalculateFormatStringWithDecimalPlaces(baseNumberFormatString);
			return result + "E+00";
		}
		string CalculateTextFormatCode() {
			return Control.InnerControl.DocumentModel.Cache.NumberFormatCache[49].FormatCode;
		}
		string CalculateCustomFormatCode() {
			NumberFormat format = NumberFormatParser.Parse(CustomFormatCode, this.Control.Document.Options.Culture);
			if (format == null)
				return null;
			return format.FormatCode;
		}
		List<string> GetCurrentTypeCaptions() {
			if (numberCategoryId == FormatNumberCategory.Date)
				return dateCaptions;
			if (numberCategoryId == FormatNumberCategory.Time)
				return timeCaptions;
			if (numberCategoryId == FormatNumberCategory.Fraction)
				return fractionCaptions;
			return null;
		}
		protected internal void InitializeFontStyle() {
			if (!Font.Bold.HasValue || !Font.Italic.HasValue) {
				fontStyle = null;
				return;
			}
			bool bold = Font.Bold.Value;
			bool italic = Font.Italic.Value;
			if (!bold && !italic)
				fontStyle = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Regular);
			else if (!bold && italic)
				fontStyle = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Italic);
			else if (bold && !italic)
				fontStyle = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Bold);
			else
				fontStyle = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_BoldItalic);
		}
		void SetActiveBorder(XlBorderLineStyle style, Color color) {
			activeBorderLineType = style;
			activeBorderColor = color;
		}
		protected internal void InitializeActiveBorderStyle() {
			if (BorderOptions.ApplyLeftBorder.HasValue && BorderOptions.ApplyLeftBorder.Value)
				SetActiveBorder(LeftLineStyle.Value, LeftColor);
			else if (BorderOptions.ApplyTopBorder.HasValue && BorderOptions.ApplyTopBorder.Value)
				SetActiveBorder(TopLineStyle.Value, TopColor);
			else if (BorderOptions.ApplyBottomBorder.HasValue && BorderOptions.ApplyBottomBorder.Value)
				SetActiveBorder(BottomLineStyle.Value, BottomColor);
			else if (BorderOptions.ApplyRightBorder.HasValue && BorderOptions.ApplyRightBorder.Value)
				SetActiveBorder(RightLineStyle.Value, RightColor);
			else if (BorderOptions.ApplyVerticalBorder.HasValue && BorderOptions.ApplyVerticalBorder.Value)
				SetActiveBorder(VerticalLineStyle.Value, VerticalColor);
			else if (BorderOptions.ApplyHorizontalBorder.HasValue && BorderOptions.ApplyHorizontalBorder.Value)
				SetActiveBorder(HorizontalLineStyle.Value, HorizontalColor);
			else if (BorderOptions.ApplyDiagonalDownBorder.HasValue && BorderOptions.ApplyDiagonalDownBorder.Value)
				SetActiveBorder(DiagonalDownLineStyle.Value, DiagonalColor);
			else if (BorderOptions.ApplyDiagonalUpBorder.HasValue && BorderOptions.ApplyDiagonalUpBorder.Value)
				SetActiveBorder(DiagonalUpLineStyle.Value, DiagonalColor);
			else
				SetActiveBorder(XlBorderLineStyle.Thin, DXColor.Black);
		}
		protected internal void InitializeBorderStateFromSheet() {
			if (BorderOptions.ApplyLeftBorder.HasValue && BorderOptions.ApplyLeftBorder.Value)
				ApplyLeftBorder = true;
			if (BorderOptions.ApplyRightBorder.HasValue && BorderOptions.ApplyRightBorder.Value)
				ApplyRightBorder = true;
			if (BorderOptions.ApplyTopBorder.HasValue && BorderOptions.ApplyTopBorder.Value)
				ApplyTopBorder = true;
			if (BorderOptions.ApplyBottomBorder.HasValue && BorderOptions.ApplyBottomBorder.Value)
				ApplyBottomBorder = true;
			if (BorderOptions.ApplyDiagonalDownBorder.HasValue && BorderOptions.ApplyDiagonalDownBorder.Value)
				ApplyDiagonalDownBorder = true;
			if (BorderOptions.ApplyDiagonalUpBorder.HasValue && BorderOptions.ApplyDiagonalUpBorder.Value)
				ApplyDiagonalUpBorder = true;
			if (BorderOptions.ApplyVerticalBorder.HasValue && BorderOptions.ApplyVerticalBorder.Value)
				ApplyVerticalBorder = true;
			if (BorderOptions.ApplyHorizontalBorder.HasValue && BorderOptions.ApplyHorizontalBorder.Value)
				ApplyHorizontalBorder = true;
			LeftLineStyle = FormProperties.CalculatedLeftBorderLineStyle;
			RightLineStyle = FormProperties.CalculatedRightBorderLineStyle;
			TopLineStyle = FormProperties.CalculatedTopBorderLineStyle;
			BottomLineStyle = FormProperties.CalculatedBottomBorderLineStyle;
			VerticalLineStyle = FormProperties.CalculatedVerticalBorderLineStyle;
			HorizontalLineStyle = FormProperties.CalculatedHorizontalBorderLineStyle;
			DiagonalDownLineStyle = FormProperties.CalculatedDiagonalDownBorderLineStyle;
			DiagonalUpLineStyle = FormProperties.CalculatedDiagonalUpBorderLineStyle;
			LeftColor = FormProperties.CalculatedLeftBorderColor;
			RightColor = FormProperties.CalculatedRightBorderColor;
			TopColor = FormProperties.CalculatedTopBorderColor;
			BottomColor = FormProperties.CalculatedBottomBorderColor;
			VerticalColor = FormProperties.CalculatedVerticalBorderColor;
			HorizontalColor = FormProperties.CalculatedHorizontalBorderColor;
			DiagonalColor = FormProperties.CalculatedDiagonalsBorderColor;
		}
		protected internal void InitializeFill() {
			isFillColorEmpty = FillBackColor == DXColor.Empty;
		}
		Color GetNotEmptyColor(Color color) {
			return DXColor.IsEmpty(color) ? DXColor.Black : color;
		}
		Color GetNotEmptyColor(Color? color) {
			return color.HasValue ? GetNotEmptyColor(color.Value) : DXColor.Black;
		}
		public bool CheckNumberFormatIsValid() {
			if (!validNumberFormat) {
				this.Control.InnerControl.ErrorHandler.HandleError(new ModelErrorInfo(ModelErrorType.InvalidNumberFormat));
				return false;
			}
			return true;
		}
	}
	#endregion
	#region FormatCellsModel
	public class FormatCellsModel : FormControllerParameters {
		#region Fields
		readonly FormatCellsFormProperties properties;
		readonly FormatCellsFormInitialTabPage initialTabPage;
		readonly List<string> numberCategories;
		readonly List<string> fontNames;
		readonly List<double> fontSizes;
		readonly List<string> fontStyles;
		readonly Dictionary<XlHorizontalAlignment, string> horizontalAlignmentTable;
		readonly Dictionary<XlVerticalAlignment, string> verticalAlignmentTable;
		readonly Dictionary<XlReadingOrder, string> textDirectionTable;
		readonly Dictionary<XlUnderlineType, string> underlineTable;
		readonly Dictionary<XlPatternType, HatchStyle> brushHatchStyles;
		#endregion
		public FormatCellsModel(ISpreadsheetControl control, FormatCellsFormProperties properties, FormatCellsFormInitialTabPage initialTabPage)
			: base(control) {
			Guard.ArgumentNotNull(properties, "properties");
			this.properties = properties;
			this.initialTabPage = initialTabPage;
			this.numberCategories = PopulateNumberCategories();
			this.fontNames = PopulateFontNames();
			this.fontSizes = PopulateFontSizes();
			this.fontStyles = PopulateFontStyles();
			this.horizontalAlignmentTable = PopulateHorizontalAlignmentTable();
			this.verticalAlignmentTable = PopulateVerticalAlignmentTable();
			this.textDirectionTable = PopulateTextDirectionTable();
			this.underlineTable = PopulateUnderlineTable();
			this.brushHatchStyles = CreateBrushHatchStyleTable();
		}
		#region Properties
		public FormatCellsFormInitialTabPage InitialTabPage { get { return initialTabPage; } }
		public FormatCellsFormProperties FormProperties { get { return properties; } }
		public List<string> NumberCategories { get { return numberCategories; } }
		public List<string> FontNames { get { return fontNames; } }
		public List<double> FontSizes { get { return fontSizes; } }
		public List<string> FontStyles { get { return fontStyles; } }
		public Dictionary<XlHorizontalAlignment, string> HorizontalAlignmentTable { get { return horizontalAlignmentTable; } }
		public Dictionary<XlVerticalAlignment, string> VerticalAlignmentTable { get { return verticalAlignmentTable; } }
		public Dictionary<XlReadingOrder, string> TextDirectionTable { get { return textDirectionTable; } }
		public Dictionary<XlUnderlineType, string> UnderlineTable { get { return underlineTable; } }
		public Dictionary<XlPatternType, HatchStyle> BrushHatchStyles { get { return brushHatchStyles; } }
		#endregion
		protected internal List<string> PopulateNumberCategories() {
			List<string> result = new List<string>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberGeneral));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberDecimal));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccountingCurrency));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberAccounting));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_FormatNumberDate));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberTime));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberPercentage));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberFraction));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberScientific));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNumberText));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_FormatNumberCustom));
			return result;
		}
		protected internal List<double> PopulateFontSizes() {
			List<double> result = new List<double>();
			PredefinedFontSizeCollection fontSize = Control.InnerControl.PredefinedFontSizeCollection;
			for (int i = 0; i < fontSize.Count; i++)
				result.Add(fontSize[i]);
			return result;
		}
		protected internal List<string> PopulateFontNames() {
			List<string> result = new List<string>();
#if !SL && !DXPORTABLE
			foreach (FontFamily family in FontFamily.Families)
				if (family.IsStyleAvailable(FontStyle.Regular))
					result.Add(family.Name);
#endif
			return result;
		}
		protected internal List<string> PopulateFontStyles() {
			List<string> result = new List<string>();
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Regular));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Italic));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_Bold));
			result.Add(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FontStyle_BoldItalic));
			return result;
		}
		protected internal Dictionary<XlHorizontalAlignment, string> PopulateHorizontalAlignmentTable() {
			Dictionary<XlHorizontalAlignment, string> result = new Dictionary<XlHorizontalAlignment, string>();
			result.Add(XlHorizontalAlignment.General, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_General));
			result.Add(XlHorizontalAlignment.Left, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_LeftIndent));
			result.Add(XlHorizontalAlignment.Center, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_Center));
			result.Add(XlHorizontalAlignment.Right, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_RightIndent));
			result.Add(XlHorizontalAlignment.Fill, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_Fill));
			result.Add(XlHorizontalAlignment.Justify, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_Justify));
			result.Add(XlHorizontalAlignment.CenterContinuous, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_CenterAcrossSelection));
			result.Add(XlHorizontalAlignment.Distributed, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_HorizontalAlignmentCaption_DistributedIndent));
			return result;
		}
		protected internal Dictionary<XlVerticalAlignment, string> PopulateVerticalAlignmentTable() {
			Dictionary<XlVerticalAlignment, string> result = new Dictionary<XlVerticalAlignment, string>();
			result.Add(XlVerticalAlignment.Top, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Top));
			result.Add(XlVerticalAlignment.Center, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Center));
			result.Add(XlVerticalAlignment.Bottom, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Bottom));
			result.Add(XlVerticalAlignment.Justify, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Justify));
			result.Add(XlVerticalAlignment.Distributed, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_VerticalAlignmentCaption_Distributed));
			return result;
		}
		protected internal Dictionary<XlReadingOrder, string> PopulateTextDirectionTable() {
			Dictionary<XlReadingOrder, string> result = new Dictionary<XlReadingOrder, string>();
			result.Add(XlReadingOrder.Context, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_TextDirectionCaption_Context));
			result.Add(XlReadingOrder.LeftToRight, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_TextDirectionCaption_LeftToRight));
			result.Add(XlReadingOrder.RightToLeft, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_TextDirectionCaption_RightToLeft));
			return result;
		}
		protected internal Dictionary<XlUnderlineType, string> PopulateUnderlineTable() {
			Dictionary<XlUnderlineType, string> result = new Dictionary<XlUnderlineType, string>();
			result.Add(XlUnderlineType.None, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_None));
			result.Add(XlUnderlineType.Single, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_Single));
			result.Add(XlUnderlineType.Double, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_Double));
			result.Add(XlUnderlineType.SingleAccounting, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_SingleAccounting));
			result.Add(XlUnderlineType.DoubleAccounting, XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FormatCellsForm_UnderlineCaption_DoubleAccounting));
			return result;
		}
		protected internal Dictionary<XlPatternType, HatchStyle> CreateBrushHatchStyleTable() {
			Dictionary<XlPatternType, HatchStyle> result = new Dictionary<XlPatternType, HatchStyle>();
			result.Add(XlPatternType.None, HatchStyle.Percent05); 
			result.Add(XlPatternType.Solid, HatchStyle.Percent90); 
			result.Add(XlPatternType.MediumGray, HatchStyle.Percent50);
			result.Add(XlPatternType.DarkGray, HatchStyle.Percent70);
			result.Add(XlPatternType.LightGray, HatchStyle.Percent25);
			result.Add(XlPatternType.DarkHorizontal, HatchStyle.DarkHorizontal);
			result.Add(XlPatternType.DarkVertical, HatchStyle.DarkVertical);
			result.Add(XlPatternType.DarkDown, HatchStyle.DarkDownwardDiagonal);
			result.Add(XlPatternType.DarkUp, HatchStyle.DarkUpwardDiagonal);
			result.Add(XlPatternType.DarkGrid, HatchStyle.SmallCheckerBoard);
			result.Add(XlPatternType.DarkTrellis, HatchStyle.Trellis);
			result.Add(XlPatternType.LightHorizontal, HatchStyle.LightHorizontal);
			result.Add(XlPatternType.LightVertical, HatchStyle.LightVertical);
			result.Add(XlPatternType.LightDown, HatchStyle.LightDownwardDiagonal);
			result.Add(XlPatternType.LightUp, HatchStyle.LightUpwardDiagonal);
			result.Add(XlPatternType.LightGrid, HatchStyle.SmallGrid);
			result.Add(XlPatternType.LightTrellis, HatchStyle.Percent30);
			result.Add(XlPatternType.Gray125, HatchStyle.Percent20);
			result.Add(XlPatternType.Gray0625, HatchStyle.Percent10);
			return result;
		}
	}
	#endregion
	#region FormatCellsFormProperties
	public class FormatCellsFormProperties {
		#region Fields
		readonly MergedCellFormat sourceFormat;
		Worksheet sheet;
		readonly CalculationRangeTypeForBorderPreview calculateRangeTypeForBorderPreview;
		readonly MergedBorderInfo borderInfo;
		readonly MergedBorderOptionsInfo borderOptions;
		CheckState mergeCells;
		bool isMerged;
		bool isNotMerged;
		#endregion
		public FormatCellsFormProperties(MergedCellFormat sourceFormat, DocumentModel documentModel) {
			Guard.ArgumentNotNull(sourceFormat, "sourceFormat");
			this.sourceFormat = sourceFormat;
			this.sheet = documentModel.ActiveSheet;
			this.calculateRangeTypeForBorderPreview = new CalculationRangeTypeForBorderPreview(sheet);
			this.borderInfo = new MergedBorderInfo();
			this.borderOptions = new MergedBorderOptionsInfo();
			this.mergeCells = InitializeMergeCellsState();
			RangeBordersCalculator.CalculateBorderInfo(sheet.Selection.SelectedRanges, borderInfo, borderOptions);
			ConverterBorderInfo(borderInfo, borderOptions);
		}
		#region Properties
		public MergedCellFormat SourceFormat { get { return sourceFormat; } }
		public CheckState MergeCells {
			get {
				return mergeCells;
			}
			set {
				if (MergeCells == value)
					return;
				mergeCells = value;
			}
		}
		public SelectedRangeTypeForBorderPreview RangeType { get { return calculateRangeTypeForBorderPreview.RangeType; } }
		public MergedBorderOptionsInfo BorderOptions { get { return borderOptions; } }
		public XlBorderLineStyle? CalculatedLeftBorderLineStyle { get { return borderInfo.LeftLineStyle.HasValue ? borderInfo.LeftLineStyle : null; } }
		public XlBorderLineStyle? CalculatedRightBorderLineStyle { get { return borderInfo.RightLineStyle.HasValue ? borderInfo.RightLineStyle : null; } }
		public XlBorderLineStyle? CalculatedTopBorderLineStyle { get { return borderInfo.TopLineStyle.HasValue ? borderInfo.TopLineStyle : null; } }
		public XlBorderLineStyle? CalculatedBottomBorderLineStyle { get { return borderInfo.BottomLineStyle.HasValue ? borderInfo.BottomLineStyle : null; } }
		public XlBorderLineStyle? CalculatedVerticalBorderLineStyle { get { return borderInfo.VerticalLineStyle.HasValue ? borderInfo.VerticalLineStyle : null; } }
		public XlBorderLineStyle? CalculatedHorizontalBorderLineStyle { get { return borderInfo.HorizontalLineStyle.HasValue ? borderInfo.HorizontalLineStyle : null; } }
		public XlBorderLineStyle? CalculatedDiagonalDownBorderLineStyle { get { return borderInfo.DiagonalDownLineStyle.HasValue ? borderInfo.DiagonalDownLineStyle : null; } }
		public XlBorderLineStyle? CalculatedDiagonalUpBorderLineStyle { get { return borderInfo.DiagonalUpLineStyle.HasValue ? borderInfo.DiagonalUpLineStyle : null; } }
		public Color CalculatedLeftBorderColor { get { return borderInfo.LeftColor.Value; } }
		public Color CalculatedRightBorderColor { get { return borderInfo.RightColor.Value; } }
		public Color CalculatedTopBorderColor { get { return borderInfo.TopColor.Value; } }
		public Color CalculatedBottomBorderColor { get { return borderInfo.BottomColor.Value; } }
		public Color CalculatedVerticalBorderColor { get { return borderInfo.VerticalColor.Value; } }
		public Color CalculatedHorizontalBorderColor { get { return borderInfo.HorizontalColor.Value; } }
		public Color CalculatedDiagonalsBorderColor { get { return borderInfo.DiagonalColor.Value; } }
		#endregion
		void ConverterBorderInfo(MergedBorderInfo borderInfo, MergedBorderOptionsInfo borderOptions) {
			NoneToNullConvertStateBorder(MergedBorderInfoAccessor.Left, borderInfo, borderOptions);
			NoneToNullConvertStateBorder(MergedBorderInfoAccessor.Right, borderInfo, borderOptions);
			NoneToNullConvertStateBorder(MergedBorderInfoAccessor.Top, borderInfo, borderOptions);
			NoneToNullConvertStateBorder(MergedBorderInfoAccessor.Bottom, borderInfo, borderOptions);
			NoneToNullConvertStateBorder(MergedBorderInfoAccessor.Vertical, borderInfo, borderOptions);
			NoneToNullConvertStateBorder(MergedBorderInfoAccessor.Horizontal, borderInfo, borderOptions);
			NoneToNullConvertStateDiagonalBorders(borderInfo, borderOptions);
		}
		void NoneToNullConvertStateBorder(MergedBorderInfoAccessor accessor, MergedBorderInfo borderInfo, MergedBorderOptionsInfo borderOptions) {
			if (accessor.GetLineStyle(borderInfo) == XlBorderLineStyle.None && accessor.GetLineColor(borderInfo) == Color.Empty && accessor.GetApplyBorder(borderOptions) == false) {
				accessor.SetBorderLine(borderInfo, null, Color.Empty);
				accessor.SetApplyBorder(borderOptions, null);
			}
		}
		void NoneToNullConvertStateDiagonalBorders(MergedBorderInfo borderInfo, MergedBorderOptionsInfo borderOptions) {
			if (borderInfo.DiagonalUpLineStyle == XlBorderLineStyle.None && borderInfo.DiagonalDownLineStyle == XlBorderLineStyle.None && borderInfo.DiagonalColor == Color.Empty && borderOptions.ApplyDiagonalUpBorder == false && borderOptions.ApplyDiagonalDownBorder == false) {
				borderInfo.DiagonalUpLineStyle = null;
				borderInfo.DiagonalDownLineStyle = null;
				borderInfo.DiagonalColor = Color.Empty;
				borderOptions.ApplyDiagonalDownBorder = null;
				borderOptions.ApplyDiagonalUpBorder = null;
			}
			else if (borderInfo.DiagonalUpLineStyle == XlBorderLineStyle.None && borderInfo.DiagonalDownLineStyle != XlBorderLineStyle.None) {
				borderInfo.DiagonalUpLineStyle = null;
				borderOptions.ApplyDiagonalUpBorder = null;
			}
			else if (borderInfo.DiagonalUpLineStyle != XlBorderLineStyle.None && borderInfo.DiagonalDownLineStyle == XlBorderLineStyle.None) {
				borderInfo.DiagonalDownLineStyle = null;
				borderOptions.ApplyDiagonalDownBorder = null;
			}
		}
		CheckState InitializeMergeCellsState() {
			List<CellRange> listOfRange = new List<CellRange>();
			foreach (CellRange range in sheet.Selection.SelectedRanges) {
				if (range.IsMerged)
					isMerged = true;
				else
					isNotMerged = true;
				listOfRange = sheet.MergedCells.GetMergedCellRangesIntersectsRange(range);
				foreach (CellRange mergedRange in listOfRange) {
					if (!mergedRange.Equals(range)) {
						isMerged = true;
						isNotMerged = true;
					}
				}
			}
			if (isMerged && !isNotMerged)
				return CheckState.Checked;
			else if (!isMerged && isNotMerged)
				return CheckState.Unchecked;
			else
				return CheckState.Indeterminate;
		}
		protected internal MergedBorderInfo GetFinalMergedBorderInfo() {
			return SourceFormat.Border.GetFixedInfo(borderOptions);
		}
	}
	#endregion
}
