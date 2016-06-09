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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Xpf.Printing {
	public class PrintingLocalizer : DXLocalizer<PrintingStringId> {
		static PrintingLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PrintingStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(PrintingStringId.OK, "OK");
			AddString(PrintingStringId.Cancel, "Cancel");
			AddString(PrintingStringId.ToolBarCaption, "Print Preview");
			AddString(PrintingStringId.StatusBarCaption, "Status Bar");
			AddString(PrintingStringId.Print, "Print...");
			AddString(PrintingStringId.Print_Hint, "Specify page settings and print the document.");
			AddString(PrintingStringId.PrintPdf, "Print via Pdf...");
			AddString(PrintingStringId.PrintPdf_Hint, "Prints the current document via PDF.");
			AddString(PrintingStringId.PrintDirect, "Quick Print");
			AddString(PrintingStringId.PrintDirect_Hint, "Print the document with default page settings using the system's default printer.");
			AddString(PrintingStringId.PageSetup, "Page Setup...");
			AddString(PrintingStringId.PageSetup_Hint, "Adjust the document's page settings.");
			AddString(PrintingStringId.Zoom, "Zoom");
			AddString(PrintingStringId.Zoom_Hint, "Change the current zoom factor of the document preview.");
			AddString(PrintingStringId.DecreaseZoom, "Zoom Out");
			AddString(PrintingStringId.DecreaseZoom_Hint, "Zoom out to see more of the page at a reduced size.");
			AddString(PrintingStringId.IncreaseZoom, "Zoom In");
			AddString(PrintingStringId.IncreaseZoom_Hint, "Zoom in to get a close-up view of the document.");
			AddString(PrintingStringId.ZoomToPageWidth, "Page Width");
			AddString(PrintingStringId.ZoomToPageHeight, "Page Height");
			AddString(PrintingStringId.ZoomToWholePage, "Whole Page");
			AddString(PrintingStringId.ZoomToTwoPages, "Two Pages");
			AddString(PrintingStringId.FirstPage, "First Page");
			AddString(PrintingStringId.FirstPage_Hint, "Navigate to the document first page.");
			AddString(PrintingStringId.PreviousPage, "Previous Page");
			AddString(PrintingStringId.PreviousPage_Hint, "Navigate to the document previous page.");
			AddString(PrintingStringId.NextPage, "Next Page");
			AddString(PrintingStringId.NextPage_Hint, "Navigate to the document next page.");
			AddString(PrintingStringId.LastPage, "Last Page");
			AddString(PrintingStringId.LastPage_Hint, "Navigate to the document last page.");
			AddString(PrintingStringId.ExportPdf, "PDF File");
			AddString(PrintingStringId.ExportHtm, "HTML File");
			AddString(PrintingStringId.ExportMht, "MHT File");
			AddString(PrintingStringId.ExportRtf, "RTF File");
			AddString(PrintingStringId.ExportXls, "XLS File");
			AddString(PrintingStringId.ExportXlsx, "XLSX File");
			AddString(PrintingStringId.ExportCsv, "CSV File");
			AddString(PrintingStringId.ExportTxt, "Text File");
			AddString(PrintingStringId.ExportImage, "Image File");
			AddString(PrintingStringId.ExportXps, "XPS File");
			AddString(PrintingStringId.ExportFile, "Export...");
			AddString(PrintingStringId.ExportFile_Hint, "Export the document in one of the available formats and save it to a file on the disk.");
			AddString(PrintingStringId.Scaling, "Scale");
			AddString(PrintingStringId.Scaling_Hint, "Stretch or shrink the document's content to a percentage of its actual size.");
			AddString(PrintingStringId.Scaling_Adjust_Start_Label, "Adjust to");
			AddString(PrintingStringId.Scaling_Adjust_End_Label, "normal size");
			AddString(PrintingStringId.Scaling_Fit_Start_Label, "Fit to");
			AddString(PrintingStringId.Scaling_Fit_End_Label, "page(s) wide");
			AddString(PrintingStringId.Scaling_ComboBoxEdit_Validation_Error, "The value is not valid");
			AddString(PrintingStringId.Scaling_ComboBoxEdit_Validation_OutOfRange_Error, "The value is out of range");
			AddString(PrintingStringId.Search, "Search");
			AddString(PrintingStringId.Search_Hint, "Shows the Find dialog to search for an occurrence of a specified text throughout the document.");
			AddString(PrintingStringId.SendPdf, "PDF File");
			AddString(PrintingStringId.SendMht, "MHT File");
			AddString(PrintingStringId.SendRtf, "RTF File");
			AddString(PrintingStringId.SendXls, "XLS File");
			AddString(PrintingStringId.SendXlsx, "XLSX File");
			AddString(PrintingStringId.SendCsv, "CSV File");
			AddString(PrintingStringId.SendTxt, "Text File");
			AddString(PrintingStringId.SendImage, "Image File");
			AddString(PrintingStringId.SendXps, "XPS File");
			AddString(PrintingStringId.SendFile, "Send...");
			AddString(PrintingStringId.SendFile_Hint, "Export the document in one of the available formats and attach it to an e-mail.");
			AddString(PrintingStringId.StopPageBuilding, "Stop");
			AddString(PrintingStringId.CurrentPageDisplayFormat, "Page {0} of {1}");
			AddString(PrintingStringId.ZoomDisplayFormat, "Zoom: {0:0}%");
			AddString(PrintingStringId.MsgCaption, "DXPrinting");
			AddString(PrintingStringId.GoToPage, "Page:");
			AddString(PrintingStringId.PrintPreviewWindowCaption, "Print Preview");
			AddString(PrintingStringId.DefaultPrintJobDescription, "Document");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_Title, "PDF Password Security");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_RequireOpenPassword, "Require a password to open the document");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_OpenPassword, "Document open password:");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_OpenPasswordHeader, "Document Open Password");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_RestrictPermissions, "Restrict editing and printing of the document");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_PermissionsPassword, "Change permissions password:");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_PrintingPermissions, "Printing allowed:");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_ChangingPermissions, "Changes allowed:");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_EnableCopying, "Enable copying of text, images and other content");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_EnableScreenReaders, "Enable text access for screen reader devices for the visually impaired");
			AddString(PrintingStringId.PdfPasswordSecurityOptions_Permissions, "Permissions");
			AddString(PrintingStringId.RepeatPassword_OpenPassword_Title, "Confirm Document Open Password");
			AddString(PrintingStringId.RepeatPassword_OpenPassword_Note, "Please confirm the Document Open Password. Be sure to make a note of the password. It will be required to open the document.");
			AddString(PrintingStringId.RepeatPassword_PermissionsPassword_Title, "Confirm Permissions Password");
			AddString(PrintingStringId.RepeatPassword_PermissionsPassword_Note, "Please confirm the Permissions Password. Be sure to make a note of the password. You will need it to change these settings in the future.");
			AddString(PrintingStringId.Watermark, "Watermark");
			AddString(PrintingStringId.Watermark_Hint, "Insert ghosted text and/or image behind the page content.");
			AddString(PrintingStringId.WatermarkTitle, "Watermark");
			AddString(PrintingStringId.PictureWatermarkTitle, "Watermark");
			AddString(PrintingStringId.TextWatermarkTitle, "Text Watermark");
			AddString(PrintingStringId.PictureWatermarkTitle, "Picture Watermark");
			AddString(PrintingStringId.WatermarkText, "Text:");
			AddString(PrintingStringId.WatermarkTextDirection, "Direction:");
			AddString(PrintingStringId.WatermarkTextColor, "Color:");
			AddString(PrintingStringId.WatermarkFontName, "Font:");
			AddString(PrintingStringId.WatermarkFontSize, "Size:");
			AddString(PrintingStringId.WatermarkFontBold, "Bold");
			AddString(PrintingStringId.WatermarkFontItalic, "Italic");
			AddString(PrintingStringId.WatermarkTransparency, "Transparency:");
			AddString(PrintingStringId.WatermarkPosition, "Position");
			AddString(PrintingStringId.WatermarkPositionInFront, "In front");
			AddString(PrintingStringId.WatermarkPositionBehind, "Behind");
			AddString(PrintingStringId.WatermarkPageRange, "Page Range");
			AddString(PrintingStringId.WatermarkPageRangeAllPages, "All");
			AddString(PrintingStringId.WatermarkPageRangePages, "Pages");
			AddString(PrintingStringId.WatermarkPageRangeHint, "For example: 1,3,5-12");
			AddString(PrintingStringId.WatermarkLoadImage, "Image:");
			AddString(PrintingStringId.WatermarkImageSizeMode, "Size mode:");
			AddString(PrintingStringId.WatermarkImageHorizontalAlignment, "Horizontal alignment:");
			AddString(PrintingStringId.WatermarkImageVerticalAlignment, "Vertical alignment:");
			AddString(PrintingStringId.WatermarkImageTiling, "Tiling");
			AddString(PrintingStringId.WatermarkClearAll, "Clear All");
			AddString(PrintingStringId.WatermarkImageLoadError, "File is corrupted");
			AddString(PrintingStringId.RepeatPassword_PermissionsPassword, "Permissions password:");
			AddString(PrintingStringId.RepeatPassword_OpenPassword, "Document open password:");
			AddString(PrintingStringId.RepeatPassword_ConfirmationPasswordDoesNotMatch, "Confirmation password does not match. Please start over and enter the password again.");
			AddString(PrintingStringId.PageSetupMarginsCaptionFormat, "Margins in {0}");
			AddString(PrintingStringId.PageSetupPrinterCaption, "Printer");
			AddString(PrintingStringId.PageSetupPrinter, "Printer:");
			AddString(PrintingStringId.PageSetupPrinterType, "Type:");
			AddString(PrintingStringId.PageSetupPrinterPort, "Port:");
			AddString(PrintingStringId.PageSetupPrinterComment, "Comment:");
			AddString(PrintingStringId.PageSetupPaperCaption, "Paper");
			AddString(PrintingStringId.PageSetupPaperSize, "Paper size:");
			AddString(PrintingStringId.PageSetupOrientationCaption, "Orientation:");
			AddString(PrintingStringId.PageSetupOrientationPortrait, "Portrait");
			AddString(PrintingStringId.PageSetupOrientationLandscape, "Landscape");
			AddString(PrintingStringId.PageSetupMarginsLeft, "Left:");
			AddString(PrintingStringId.PageSetupMarginsTop, "Top:");
			AddString(PrintingStringId.PageSetupMarginsRight, "Right:");
			AddString(PrintingStringId.PageSetupMarginsBottom, "Bottom:");
			AddString(PrintingStringId.PageSetupMillimeters, "Millimeters");
			AddString(PrintingStringId.PageSetupInches, "Inches");
			AddString(PrintingStringId.Parameters, "Parameters");
			AddString(PrintingStringId.Parameters_Hint, "Shows or hides the Parameters panel, where you can customize the values of report parameters.");
			AddString(PrintingStringId.ParametersReset, "Reset");
			AddString(PrintingStringId.ParametersSubmit, "Submit");
			AddString(PrintingStringId.ZoomValueItemFormat, "{0}%");
			AddString(PrintingStringId.Open, "Open");
			AddString(PrintingStringId.Open_Hint, "Open a report document.");
			AddString(PrintingStringId.Save, "Save");
			AddString(PrintingStringId.Save_Hint, "Save the report document.");
			AddString(PrintingStringId.Error, "Error");
			AddString(PrintingStringId.PageSetupWindowTitle, "Page Setup");
			AddString(PrintingStringId.DocumentMap, "Document Map");
			AddString(PrintingStringId.DocumentMap_Hint, "Shows the Document Map panel, which reflects the document's structure, and where you can navigate through the report's bookmarks.");
			AddString(PrintingStringId.Refresh, "Refresh");
			AddString(PrintingStringId.Information, "Information");
			AddString(PrintingStringId.Search_EmptyResult, "Your search did not match any text.");
			AddString(PrintingStringId.PreparingPages, "Preparing pages...");
			AddString(PrintingStringId.PagesArePrepared, "Pages are ready. Continue printing?");
			AddString(PrintingStringId.ExportPdfToWindow, "PDF File");
			AddString(PrintingStringId.ExportHtmToWindow, "HTML File");
			AddString(PrintingStringId.ExportMhtToWindow, "MHT File");
			AddString(PrintingStringId.ExportRtfToWindow, "RTF File");
			AddString(PrintingStringId.ExportXlsToWindow, "XLS File");
			AddString(PrintingStringId.ExportXlsxToWindow, "XLSX File");
			AddString(PrintingStringId.ExportCsvToWindow, "CSV File");
			AddString(PrintingStringId.ExportTxtToWindow, "Text File");
			AddString(PrintingStringId.ExportImageToWindow, "Image File");
			AddString(PrintingStringId.ExportXpsToWindow, "XPS File");
			AddString(PrintingStringId.ExportFileToWindow, "Export Document to Window...");
			AddString(PrintingStringId.ExportFileToWindow_Hint, "Exports the current document, and shows the result in a new browser window.");
			AddString(PrintingStringId.ClosePreview, "Close");
			AddString(PrintingStringId.Exception_NoPrinterFound, "No printer has been found on the machine");
			AddString(PrintingStringId.Msg_EmptyDocument, "The document does not contain any pages.");
			AddString(PrintingStringId.PdfSignatureEditorWindow_Certificate, "Certificate:");
			AddString(PrintingStringId.PdfSignatureEditorWindow_Reason, "Reason:");
			AddString(PrintingStringId.PdfSignatureEditorWindow_Location, "Location:");
			AddString(PrintingStringId.PdfSignatureEditorWindow_ContactInformation, "Contact Information:");
			AddString(PrintingStringId.PdfSignatureEditorWindow_Title, "Signature Options");
			AddString(PrintingStringId.PaperKind_Custom, "Custom");
			AddString(PrintingStringId.PaperKind_Letter, "Letter");
			AddString(PrintingStringId.PaperKind_LetterSmall, "Letter Small");
			AddString(PrintingStringId.PaperKind_Tabloid, "Tabloid");
			AddString(PrintingStringId.PaperKind_Ledger, "Ledger");
			AddString(PrintingStringId.PaperKind_Legal, "Legal");
			AddString(PrintingStringId.PaperKind_Statement, "Statement");
			AddString(PrintingStringId.PaperKind_Executive, "Executive");
			AddString(PrintingStringId.PaperKind_A3, "A3");
			AddString(PrintingStringId.PaperKind_A4, "A4");
			AddString(PrintingStringId.PaperKind_A4Small, "A4 Small");
			AddString(PrintingStringId.PaperKind_A5, "A5");
			AddString(PrintingStringId.PaperKind_B4, "B4");
			AddString(PrintingStringId.PaperKind_B5, "B5");
			AddString(PrintingStringId.PaperKind_Folio, "Folio");
			AddString(PrintingStringId.PaperKind_Quarto, "Quarto");
			AddString(PrintingStringId.PaperKind_Standard10x14, "Standard 10x14");
			AddString(PrintingStringId.PaperKind_Standard11x17, "Standard 11x17");
			AddString(PrintingStringId.PaperKind_Note, "Note");
			AddString(PrintingStringId.PaperKind_Number9Envelope, "Number 9 Envelope");
			AddString(PrintingStringId.PaperKind_Number10Envelope, "Number 10 Envelope");
			AddString(PrintingStringId.PaperKind_Number11Envelope, "Number 11 Envelope");
			AddString(PrintingStringId.PaperKind_Number12Envelope, "Number 12 Envelope");
			AddString(PrintingStringId.PaperKind_Number14Envelope, "Number 14 Envelope");
			AddString(PrintingStringId.PaperKind_CSheet, "C Sheet");
			AddString(PrintingStringId.PaperKind_DSheet, "D Sheet");
			AddString(PrintingStringId.PaperKind_ESheet, "E Sheet");
			AddString(PrintingStringId.PaperKind_DLEnvelope, "DL Envelope");
			AddString(PrintingStringId.PaperKind_C5Envelope, "C5 Envelope");
			AddString(PrintingStringId.PaperKind_C3Envelope, "C3 Envelope");
			AddString(PrintingStringId.PaperKind_C4Envelope, "C4 Envelope");
			AddString(PrintingStringId.PaperKind_C6Envelope, "C6 Envelope");
			AddString(PrintingStringId.PaperKind_C65Envelope, "C65 Envelope");
			AddString(PrintingStringId.PaperKind_B4Envelope, "B4 Envelope");
			AddString(PrintingStringId.PaperKind_B5Envelope, "B5 Envelope");
			AddString(PrintingStringId.PaperKind_B6Envelope, "B6 Envelope");
			AddString(PrintingStringId.PaperKind_ItalyEnvelope, "Italy Envelope");
			AddString(PrintingStringId.PaperKind_MonarchEnvelope, "Monarch Envelope");
			AddString(PrintingStringId.PaperKind_PersonalEnvelope, "Personal Envelope (6 3/4)");
			AddString(PrintingStringId.PaperKind_USStandardFanfold, "US Standard Fanfold");
			AddString(PrintingStringId.PaperKind_GermanStandardFanfold, "German Standard Fanfold");
			AddString(PrintingStringId.PaperKind_GermanLegalFanfold, "German Legal Fanfold");
			AddString(PrintingStringId.PaperKind_IsoB4, "Iso B4");
			AddString(PrintingStringId.PaperKind_JapanesePostcard, "Japanese Postcard");
			AddString(PrintingStringId.PaperKind_Standard9x11, "Standard 9x11");
			AddString(PrintingStringId.PaperKind_Standard10x11, "Standard 10x11");
			AddString(PrintingStringId.PaperKind_Standard15x11, "Standard 15x11");
			AddString(PrintingStringId.PaperKind_InviteEnvelope, "Invite Envelope");
			AddString(PrintingStringId.PaperKind_LetterExtra, "Letter Extra");
			AddString(PrintingStringId.PaperKind_LegalExtra, "Legal Extra");
			AddString(PrintingStringId.PaperKind_TabloidExtra, "Tabloid Extra");
			AddString(PrintingStringId.PaperKind_A4Extra, "A4 Extra");
			AddString(PrintingStringId.PaperKind_LetterTransverse, "Letter Transverse");
			AddString(PrintingStringId.PaperKind_A4Transverse, "A4 Transverse");
			AddString(PrintingStringId.PaperKind_LetterExtraTransverse, "Letter Extra Transverse");
			AddString(PrintingStringId.PaperKind_APlus, "SuperA/SuperA/A4");
			AddString(PrintingStringId.PaperKind_BPlus, "SuperB/SuperB/A3");
			AddString(PrintingStringId.PaperKind_LetterPlus, "Letter Plus");
			AddString(PrintingStringId.PaperKind_A4Plus, "A4 Plus");
			AddString(PrintingStringId.PaperKind_A5Transverse, "A5 Transverse");
			AddString(PrintingStringId.PaperKind_B5Transverse, "JIS B5 Transverse");
			AddString(PrintingStringId.PaperKind_A3Extra, "A3 Extra");
			AddString(PrintingStringId.PaperKind_A5Extra, "A5 Extra");
			AddString(PrintingStringId.PaperKind_B5Extra, "ISO B5 Extra");
			AddString(PrintingStringId.PaperKind_A2, "A2");
			AddString(PrintingStringId.PaperKind_A3Transverse, "A3 Transverse");
			AddString(PrintingStringId.PaperKind_A3ExtraTransverse, "A3 Extra Transverse");
			AddString(PrintingStringId.PaperKind_JapaneseDoublePostcard, "Japanese Double Postcard");
			AddString(PrintingStringId.PaperKind_A6, "A6");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeKakuNumber2, "Japanese Envelope Kaku Number 2");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeKakuNumber3, "Japanese Envelope Kaku Number 3");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeChouNumber3, "Japanese Envelope Chou Number 3");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeChouNumber4, "Japanese Envelope Chou Number 4");
			AddString(PrintingStringId.PaperKind_LetterRotated, "Letter Rotated");
			AddString(PrintingStringId.PaperKind_A3Rotated, "A3 Rotated");
			AddString(PrintingStringId.PaperKind_A4Rotated, "A4 Rotated");
			AddString(PrintingStringId.PaperKind_A5Rotated, "A5 Rotated");
			AddString(PrintingStringId.PaperKind_B4JisRotated, "JIS B4 Rotated ");
			AddString(PrintingStringId.PaperKind_B5JisRotated, "JIS B5 Rotated");
			AddString(PrintingStringId.PaperKind_JapanesePostcardRotated, "Japanese Postcard Rotated");
			AddString(PrintingStringId.PaperKind_JapaneseDoublePostcardRotated, "Japanese Double Postcard Rotated");
			AddString(PrintingStringId.PaperKind_A6Rotated, "A6 Rotated");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeKakuNumber2Rotated, "Japanese Envelope Kaku Number 2 Rotated");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeKakuNumber3Rotated, "Japanese Envelope Kaku Number 3 Rotated");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeChouNumber3Rotated, "Japanese Envelope Chou Number 3 Rotated");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeChouNumber4Rotated, "Japanese Envelope Chou Number 4 Rotated");
			AddString(PrintingStringId.PaperKind_B6Jis, "JIS B6");
			AddString(PrintingStringId.PaperKind_B6JisRotated, "JIS B6 Rotated");
			AddString(PrintingStringId.PaperKind_Standard12x11, "Standard 12x11");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeYouNumber4, "Japanese Envelope You Number 4");
			AddString(PrintingStringId.PaperKind_JapaneseEnvelopeYouNumber4Rotated, "Japanese Envelope You Number 4 Rotated");
			AddString(PrintingStringId.PaperKind_Prc16K, "Prc 16K");
			AddString(PrintingStringId.PaperKind_Prc32K, "Prc 32K");
			AddString(PrintingStringId.PaperKind_Prc32KBig, "Prc 32K Big");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber1, "Prc Envelope Number 1");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber2, "Prc Envelope Number 2");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber3, "Prc Envelope Number 3");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber4, "Prc Envelope Number 4");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber5, "Prc Envelope Number 5");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber6, "Prc Envelope Number 6");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber7, "Prc Envelope Number 7");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber8, "Prc Envelope Number 8");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber9, "Prc Envelope Number 9");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber10, "Prc Envelope Number 10");
			AddString(PrintingStringId.PaperKind_Prc16KRotated, "Prc 16K Rotated");
			AddString(PrintingStringId.PaperKind_Prc32KRotated, "Prc 32K Rotated");
			AddString(PrintingStringId.PaperKind_Prc32KBigRotated, "Prc 32K Big Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber1Rotated, "Prc Envelope Number 1 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber2Rotated, "Prc Envelope Number 2 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber3Rotated, "Prc Envelope Number 3 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber4Rotated, "Prc Envelope Number 4 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber5Rotated, "Prc Envelope Number 5 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber6Rotated, "Prc Envelope Number 6 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber7Rotated, "Prc Envelope Number 7 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber8Rotated, "Prc Envelope Number 8 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber9Rotated, "Prc Envelope Number 9 Rotated");
			AddString(PrintingStringId.PaperKind_PrcEnvelopeNumber10Rotated, "Prc Envelope Number 10 Rotated");
			AddString(PrintingStringId.RibbonPageGroup_File, "File");
			AddString(PrintingStringId.RibbonPageGroup_Print, "Print");
			AddString(PrintingStringId.RibbonPageGroup_Navigation, "Navigation");
			AddString(PrintingStringId.RibbonPageGroup_Zoom, "Zoom");
			AddString(PrintingStringId.RibbonPageGroup_Export, "Export");
			AddString(PrintingStringId.RibbonPageGroup_Document, "Document");
			AddString(PrintingStringId.RibbonPageGroup_Watermark, "Watermark");
			AddString(PrintingStringId.RibbonPageCaption, "Preview");
			AddString(PrintingStringId.True, "True");
			AddString(PrintingStringId.False, "False");
			AddString(PrintingStringId.HandTool, "Hand Tool");
			AddString(PrintingStringId.SelectTool, "Select Tool");
			AddString(PrintingStringId.Copy, "Copy");
			AddString(PrintingStringId.FilePath, "File path");
			AddString(PrintingStringId.OpenFileAfterExport, "Open file after exporting");
			AddString(PrintingStringId.ExportFormat, "Export format");
			AddString(PrintingStringId.MoreOptions, "More Options");
			AddString(PrintingStringId.Printer, "Printer");
			AddString(PrintingStringId.Preferences, "Preferences");
			AddString(PrintingStringId.PrinterStatus, "Status");
			AddString(PrintingStringId.PrinterLocation, "Location");
			AddString(PrintingStringId.PrinterComment, "Comment");
			AddString(PrintingStringId.PrinterQueue, "Document(s) in Queue");
			AddString(PrintingStringId.Copies, "Number of copies");
			AddString(PrintingStringId.Collate, "Collate");
			AddString(PrintingStringId.PageRange, "Page range");
			AddString(PrintingStringId.AllPages, "All pages");
			AddString(PrintingStringId.CurrentPage, "Current");
			AddString(PrintingStringId.SomePages, "Some pages");
			AddString(PrintingStringId.PageRangeHint, "For example: 1,3,5-12");
			AddString(PrintingStringId.PaperSource, "Paper source");
			AddString(PrintingStringId.PrintFilePath, "File path");
			AddString(PrintingStringId.PrintToFile, "Print to file");
			AddString(PrintingStringId.DocumentSourceNotSupported, "This type of document source is not supported.");
			AddString(PrintingStringId.ReportBehavior_RibbonReportsCaption, "Reports");
			AddString(PrintingStringId.ReportBehavior_MenuPrintPreview, "Print Preview...");
			AddString(PrintingStringId.ReportBehavior_MenuDesignReport, "Design Report...");
			AddString(PrintingStringId.ReportBehavior_MenuPrint, "Print...");
			AddString(PrintingStringId.ReportBehavior_MenuEdit, "Edit...");
			AddString(PrintingStringId.ReportBehavior_MenuRename, "Rename...");
			AddString(PrintingStringId.ReportBehavior_MenuDelete, "Delete");
			AddString(PrintingStringId.ReportBehavior_DeleteDialogCaption, "Delete Report");
			AddString(PrintingStringId.ReportBehavior_DeleteDialogMessage, "Are you sure you want to delete report {0}?");
			AddString(PrintingStringId.ReportBehavior_RenameDialogCaption, "Rename Report");
			AddString(PrintingStringId.ReportBehavior_RenameDialogNameLabel, "Name");
			AddString(PrintingStringId.ReportBehavior_SaveDialogCaption, "Save Report");
			AddString(PrintingStringId.ReportManagerServiceWizard_SelectTheme, "Select Theme");
			AddString(PrintingStringId.ReportManagerServiceWizard_WindowTitle, "Report Wizard");
			AddString(PrintingStringId.ReportManagerServiceWizard_SelectPageOptions, "Select Page Options");
			AddString(PrintingStringId.ReportManagerServiceWizard_Paper, "Paper");
			AddString(PrintingStringId.ReportManagerServiceWizard_PaperSize, "Size");
			AddString(PrintingStringId.ReportManagerServiceWizard_ReportUnits, "Units");
			AddString(PrintingStringId.ReportManagerServiceWizard_PageOrientation, "Orientation");
			AddString(PrintingStringId.ReportManagerServiceWizard_PageSetup, "Page Setup");
		}
		#endregion
		public static XtraLocalizer<PrintingStringId> CreateDefaultLocalizer() {
			return new PrintingResXLocalizer();
		}
		public static string GetString(PrintingStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<PrintingStringId> CreateResXLocalizer() {
			return new PrintingResXLocalizer();
		}
	}
}
