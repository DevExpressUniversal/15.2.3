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
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System.Drawing;
using DevExpress.Office.Utils;
using System.Text.RegularExpressions;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Design.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ShowPageSetupFormCommandBase (abstract class)
	public abstract class ShowPageSetupFormCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected ShowPageSetupFormCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public abstract PageSetupFormInitialTabPage InitialTabPage { get; }
		public PrintSetup PrintSetup {
			get { return this.DocumentModel.ActiveSheet.PrintSetup; }
		}
		public Model.Margins Margins {
			get { return this.DocumentModel.ActiveSheet.Margins; }
		}
		public HeaderFooterOptions HeaderFooter {
			get { return this.DocumentModel.ActiveSheet.HeaderFooter; }
		}
		public Office.DocumentModelUnitConverter UnitConverter {
			get { return this.DocumentModel.UnitConverter; }
		}
		#endregion
		protected internal override void ExecuteCore() {
			if (InnerControl.AllowShowingForms)
				Control.ShowPageSetupForm(CreateViewModel(), InitialTabPage);
		}
		public PageSetupViewModel CreateViewModel() {
			PageSetupViewModel viewModel = new PageSetupViewModel(Control, InitialTabPage);
			viewModel.OrientationPortrait = PrintSetup.Orientation == ModelPageOrientation.Portrait || PrintSetup.Orientation == ModelPageOrientation.Default ? true : false;
			viewModel.Scale = PrintSetup.Scale;
			viewModel.FitToPage = PrintSetup.FitToPage;
			viewModel.FitToWidth = PrintSetup.FitToWidth;
			viewModel.FitToHeight = PrintSetup.FitToHeight;
			viewModel.PaperType = PrintSetup.PaperKind;
			viewModel.HorizontalDpi = PrintSetup.HorizontalDpi;
			viewModel.VerticalDpi = PrintSetup.VerticalDpi;
			if (!PrintSetup.UseFirstPageNumber)
				viewModel.FirstPageNumber = "Auto";
			else
				viewModel.FirstPageNumber = PrintSetup.FirstPageNumber.ToString();
			viewModel.UseFirstPageNumber = PrintSetup.UseFirstPageNumber;
			UIUnitConverter converter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			viewModel.HeaderMargin = converter.ToUIUnitF(Margins.Header, Control.UIUnit).Value;
			viewModel.FooterMargin = converter.ToUIUnitF(Margins.Footer, Control.UIUnit).Value;
			viewModel.TopMargin = converter.ToUIUnitF(Margins.Top, Control.UIUnit).Value;
			viewModel.BottomMargin = converter.ToUIUnitF(Margins.Bottom, Control.UIUnit).Value;
			viewModel.LeftMargin = converter.ToUIUnitF(Margins.Left, Control.UIUnit).Value;
			viewModel.RightMargin = converter.ToUIUnitF(Margins.Right, Control.UIUnit).Value;
			viewModel.CenterHorizontally = PrintSetup.CenterHorizontally;
			viewModel.CenterVertically = PrintSetup.CenterVertically;
			viewModel.OddHeader = Regex.Replace(HeaderFooter.OddHeader, "(?<!\r)\n", Environment.NewLine);
			viewModel.OddFooter = Regex.Replace(HeaderFooter.OddFooter, "(?<!\r)\n", Environment.NewLine);
			viewModel.EvenHeader = Regex.Replace(HeaderFooter.EvenHeader, "(?<!\r)\n", Environment.NewLine);
			viewModel.EvenFooter = Regex.Replace(HeaderFooter.EvenFooter, "(?<!\r)\n", Environment.NewLine);
			viewModel.FirstHeader = Regex.Replace(HeaderFooter.FirstHeader, "(?<!\r)\n", Environment.NewLine);
			viewModel.FirstFooter = Regex.Replace(HeaderFooter.FirstFooter, "(?<!\r)\n", Environment.NewLine);
			viewModel.DifferentOddEven = HeaderFooter.DifferentOddEven;
			viewModel.DifferentFirstPage = HeaderFooter.DifferentFirst;
			viewModel.ScaleWithDocument = HeaderFooter.ScaleWithDoc;
			viewModel.AlignWithMargins = HeaderFooter.AlignWithMargins;
			viewModel.GetPrintArea();
			viewModel.PrintGridlines = PrintSetup.PrintGridLines;
			viewModel.BlackAndWhite = PrintSetup.BlackAndWhite;
			viewModel.Draft = PrintSetup.Draft;
			viewModel.PrintHeadings = PrintSetup.PrintHeadings;
			viewModel.CommentsPrintMode = viewModel.GetCommentModeStringByEnum(PrintSetup.CommentsPrintMode);
			viewModel.ErrorsPrintMode = viewModel.GetErrorModeStringByEnum(PrintSetup.ErrorsPrintMode);
			viewModel.DownThenOver = PrintSetup.PagePrintOrder == PagePrintOrder.DownThenOver ? true : false;
			return viewModel;
		}
		public abstract bool Validate(PageSetupViewModel viewModel);
		public virtual void ApplyChanges(PageSetupViewModel viewModel) {
			DocumentModel.BeginUpdate();
			try {
				ApplyChangeMargins(viewModel);
				ApplyChangePrintSetup(viewModel);
				ApplyChangeHeaderFooter(viewModel);
				PageSetupSetPrintAreaCommand setPrintAreaCommand = new PageSetupSetPrintAreaCommand(Control);
				setPrintAreaCommand.BeforeModifying();
				foreach (CellRange range in viewModel.PrintRanges) {
					setPrintAreaCommand.Modify(range);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void ApplyChangeMargins(PageSetupViewModel viewModel) {
			UIUnitConverter converter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			UIUnit headerUnit = new UIUnit(viewModel.HeaderMargin, Control.UIUnit, UnitPrecisionDictionary.DefaultPrecisions);
			Margins.Header = converter.ToTwipsUnit(headerUnit, false);
			UIUnit footerUnit = new UIUnit(viewModel.FooterMargin, Control.UIUnit, UnitPrecisionDictionary.DefaultPrecisions);
			Margins.Footer = converter.ToTwipsUnit(footerUnit, false);
			UIUnit topUnit = new UIUnit(viewModel.TopMargin, Control.UIUnit, UnitPrecisionDictionary.DefaultPrecisions);
			Margins.Top = converter.ToTwipsUnit(topUnit, false);
			UIUnit bottomUnit = new UIUnit(viewModel.BottomMargin, Control.UIUnit, UnitPrecisionDictionary.DefaultPrecisions);
			Margins.Bottom = converter.ToTwipsUnit(bottomUnit, false);
			UIUnit leftUnit = new UIUnit(viewModel.LeftMargin, Control.UIUnit, UnitPrecisionDictionary.DefaultPrecisions);
			Margins.Left = converter.ToTwipsUnit(leftUnit, false);
			UIUnit rightUnit = new UIUnit(viewModel.RightMargin, Control.UIUnit, UnitPrecisionDictionary.DefaultPrecisions);
			Margins.Right = converter.ToTwipsUnit(rightUnit, false);
		}
		void ApplyChangePrintSetup(PageSetupViewModel viewModel) {
			int parseFirstPageNumberValue;
			PrintSetup.Orientation = viewModel.OrientationPortrait ? ModelPageOrientation.Portrait : ModelPageOrientation.Landscape;
			PrintSetup.FitToPage = viewModel.FitToPage;
			PrintSetup.FitToWidth = viewModel.FitToWidth;
			PrintSetup.FitToHeight = viewModel.FitToHeight;
			PrintSetup.Scale = viewModel.Scale;
			PrintSetup.PaperKind = viewModel.PaperType;
			if (Int32.TryParse(viewModel.FirstPageNumber, out parseFirstPageNumberValue))
				PrintSetup.FirstPageNumber = parseFirstPageNumberValue;
			PrintSetup.UseFirstPageNumber = viewModel.UseFirstPageNumber;
			PrintSetup.HorizontalDpi = viewModel.HorizontalDpi;
			PrintSetup.VerticalDpi = viewModel.VerticalDpi;
			PrintSetup.CenterHorizontally = viewModel.CenterHorizontally;
			PrintSetup.CenterVertically = viewModel.CenterVertically;
			PrintSetup.PrintGridLines = viewModel.PrintGridlines;
			PrintSetup.BlackAndWhite = viewModel.BlackAndWhite;
			PrintSetup.Draft = viewModel.Draft;
			PrintSetup.PrintHeadings = viewModel.PrintHeadings;
			PrintSetup.CommentsPrintMode = viewModel.GetCommentModeByString(viewModel.CommentsPrintMode);
			PrintSetup.ErrorsPrintMode = viewModel.GetErrorModeByString(viewModel.ErrorsPrintMode);
			PrintSetup.PagePrintOrder = viewModel.DownThenOver ? PagePrintOrder.DownThenOver : PagePrintOrder.OverThenDown;
		}
		void ApplyChangeHeaderFooter(PageSetupViewModel viewModel) {
			HeaderFooter.OddHeader = viewModel.OddHeader;
			HeaderFooter.OddFooter = viewModel.OddFooter;
			HeaderFooter.EvenHeader = viewModel.EvenHeader;
			HeaderFooter.EvenFooter = viewModel.EvenFooter;
			HeaderFooter.FirstHeader = viewModel.FirstHeader;
			HeaderFooter.FirstFooter = viewModel.FirstFooter;
			HeaderFooter.DifferentOddEven = viewModel.DifferentOddEven;
			HeaderFooter.DifferentFirst = viewModel.DifferentFirstPage;
			HeaderFooter.ScaleWithDoc = viewModel.ScaleWithDocument;
			HeaderFooter.AlignWithMargins = viewModel.AlignWithMargins;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive);
		}
	}
	#endregion
	#region ShowPageSetupCommand
	public class ShowPageSetupCommand : ShowPageSetupFormCommandBase {
		public ShowPageSetupCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override PageSetupFormInitialTabPage InitialTabPage { get { return PageSetupFormInitialTabPage.Page; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PageSetup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupDescription; } }
		#endregion
		public override bool Validate(PageSetupViewModel viewModel) {
			string firstPageNumberAutoString = "auto";
			int firstPageNumberParseValue = 0;
			if ((viewModel.FitToHeight > 32767 || viewModel.FitToHeight < 0) || (viewModel.FitToWidth > 32767 || viewModel.FitToWidth < 0)) {
				IModelErrorInfo error = new ModelErrorInfoWithArgs(ModelErrorType.IncorrectNumberRange, new string[2] { "0", "32767" });
				Control.InnerControl.ErrorHandler.HandleError(error);
				return false;
			}
			if ((viewModel.Scale > 32767 || viewModel.Scale < -32765)) {
				IModelErrorInfo error = new ModelErrorInfo(ModelErrorType.InvalidNumber);
				Control.InnerControl.ErrorHandler.HandleError(error);
				return false;
			}
			if (viewModel.Scale < 10 || viewModel.Scale > 400) {
				IModelErrorInfo error = new ModelErrorInfoWithArgs(ModelErrorType.IncorrectNumberRange, new string[2] { "10", "400" });
				Control.InnerControl.ErrorHandler.HandleError(error);
				return false;
			}
			if (Int32.TryParse(viewModel.FirstPageNumber, out firstPageNumberParseValue)) {
				if (firstPageNumberParseValue > 32767 || firstPageNumberParseValue < 0) {
					IModelErrorInfo error = new ModelErrorInfo(ModelErrorType.InvalidNumber);
					Control.InnerControl.ErrorHandler.HandleError(error);
					return false;
				}
			}
			else {
				if (viewModel.FirstPageNumber.ToLower() != firstPageNumberAutoString) {
					IModelErrorInfo error = new ModelErrorInfo(ModelErrorType.InvalidNumber);
					Control.InnerControl.ErrorHandler.HandleError(error);
					return false;
				}
			}
			return true;
		}
		public override void ApplyChanges(PageSetupViewModel viewModel) {
			base.ApplyChanges(viewModel);
		}
	}
	#endregion
	#region ShowPageSetupMarginsCommand
	public class ShowPageSetupMarginsCommand : ShowPageSetupFormCommandBase {
		const int minSpacingMarginsInModelUnits = 288; 
		public ShowPageSetupMarginsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override PageSetupFormInitialTabPage InitialTabPage { get { return PageSetupFormInitialTabPage.Margins; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PageSetupMargins; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupMargins; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupMarginsDescription; } }
		#endregion
		public override bool Validate(PageSetupViewModel viewModel) {
			Size pageSize = PaperSizeCalculator.CalculatePaperSize(viewModel.PaperType);
			UIUnitConverter converter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			float height = converter.ToUIUnitF(pageSize.Height, Control.UIUnit).Value;
			float width = converter.ToUIUnitF(pageSize.Width, Control.UIUnit).Value;
			float minSpacingMargins = converter.ToUIUnitF(minSpacingMarginsInModelUnits, Control.UIUnit).Value;
			if ((height - (viewModel.TopMargin + viewModel.BottomMargin) < minSpacingMargins) ||
				(height - (viewModel.HeaderMargin + viewModel.FooterMargin) < minSpacingMargins) ||
				(width - (viewModel.LeftMargin + viewModel.RightMargin) < minSpacingMargins)) {
				IModelErrorInfo error = new ModelErrorInfo(ModelErrorType.PageSetupMarginsNotFitPageSize);
				Control.InnerControl.ErrorHandler.HandleError(error);
				return false;
			}
			return true;
		}
		public override void ApplyChanges(PageSetupViewModel viewModel) {
			base.ApplyChanges(viewModel);
		}
	}
	#endregion
	#region ShowPageSetupHeaderFooterCommand
	public class ShowPageSetupHeaderFooterCommand : ShowPageSetupFormCommandBase {
		public ShowPageSetupHeaderFooterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override PageSetupFormInitialTabPage InitialTabPage { get { return PageSetupFormInitialTabPage.HeaderFooter; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PageSetupHeaderFooter; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupHeaderFooter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupHeaderFooterDescription; } }
		#endregion
		public override bool Validate(PageSetupViewModel viewModel) {
			return true;
		}
		public override void ApplyChanges(PageSetupViewModel viewModel) {
			base.ApplyChanges(viewModel);
		}
	}
	#endregion
	#region ShowPageSetupSheetCommand
	public class ShowPageSetupSheetCommand : ShowPageSetupFormCommandBase {
		public ShowPageSetupSheetCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override PageSetupFormInitialTabPage InitialTabPage { get { return PageSetupFormInitialTabPage.Sheet; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PageSetupSheet; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupSheet; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupSheetDescription; } }
		#endregion
		public override bool Validate(PageSetupViewModel viewModel) {
			if (!String.IsNullOrEmpty(viewModel.PrintArea)) {
				if (viewModel.IsIncorrectRange) {
					IModelErrorInfo error = new ModelErrorInfo(ModelErrorType.PageSetupProblemFormula);
					Control.InnerControl.ErrorHandler.HandleError(error);
					return false;
				}
			}
			return true;
		}
		public override void ApplyChanges(PageSetupViewModel viewModel) {
			base.ApplyChanges(viewModel);
		}
	}
	#endregion
	#region ShowPageSetupCustomMarginsCommand
	public class ShowPageSetupCustomMarginsCommand : ShowPageSetupMarginsCommand {
		public ShowPageSetupCustomMarginsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PageSetupCustomMargins; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupCustomMargins; } }
		#endregion
	}
	#endregion
	#region ShowPageSetupMorePaperSizesCommand
	public class ShowPageSetupMorePaperSizesCommand : ShowPageSetupCommand {
		public ShowPageSetupMorePaperSizesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PageSetupMorePaperSizes; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PageSetupMorePaperSizes; } }
		#endregion
	}
	#endregion
}
