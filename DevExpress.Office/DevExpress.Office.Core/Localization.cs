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
using System.Reflection;
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Office.Localization.Internal;
namespace DevExpress.Office.Localization {
	#region OfficeStringId
	public enum OfficeStringId {
		Msg_IsNotValid,
		Msg_InternalError,
		Msg_InvalidBeginUpdate,
		Msg_InvalidEndUpdate,
		Msg_InvalidBeginInit,
		Msg_InvalidEndInit,
		Msg_InvalidCopyFromDocumentModel,
		Msg_Loading,
		Msg_MagicNumberNotFound,
		Msg_InvalidFontSize,
		Msg_InvalidNumber,
		Msg_InvalidNumberConverterValue,
		Msg_InvalidRemoveDataSource,
		FileFilterDescription_AllFiles,
		FileFilterDescription_AllSupportedFiles,
		FileFilterDescription_BitmapFiles,
		FileFilterDescription_JPEGFiles,
		FileFilterDescription_PNGFiles,
		FileFilterDescription_GifFiles,
		FileFilterDescription_TiffFiles,
		FileFilterDescription_EmfFiles,
		FileFilterDescription_WmfFiles,
		UnitAbbreviation_Inch,
		UnitAbbreviation_Centimeter,
		UnitAbbreviation_Millimeter,
		UnitAbbreviation_Pica,
		UnitAbbreviation_Point,
		UnitAbbreviation_Percent,
		Caption_UnitPercent,
		Caption_UnitInches,
		Caption_UnitCentimeters,
		Caption_UnitMillimeters,
		Caption_UnitPoints,
		MenuCmd_NewEmptyDocument,
		MenuCmd_NewEmptyDocumentDescription,
		MenuCmd_LoadDocument,
		MenuCmd_LoadDocumentDescription,
		MenuCmd_SaveDocument,
		MenuCmd_SaveDocumentDescription,
		MenuCmd_SaveDocumentAs,
		MenuCmd_SaveDocumentAsDescription,
		MenuCmd_Undo,
		MenuCmd_UndoDescription,
		MenuCmd_Redo,
		MenuCmd_RedoDescription,
		MenuCmd_ClearUndo,
		MenuCmd_ClearUndoDescription,
		MenuCmd_Print,
		MenuCmd_PrintDescription,
		MenuCmd_QuickPrint,
		MenuCmd_QuickPrintDescription,
		MenuCmd_PrintPreview,
		MenuCmd_PrintPreviewDescription,
		MenuCmd_CutSelection,
		MenuCmd_CutSelectionDescription,
		MenuCmd_CopySelection,
		MenuCmd_CopySelectionDescription,
		MenuCmd_Paste,
		MenuCmd_PasteDescription,
		MenuCmd_ShowPasteSpecialForm,
		MenuCmd_ShowPasteSpecialFormDescription,
		MenuCmd_AlignmentLeft,
		MenuCmd_AlignmentLeftDescription,
		MenuCmd_AlignmentCenter,
		MenuCmd_AlignmentCenterDescription,
		MenuCmd_AlignmentRight,
		MenuCmd_AlignmentRightDescription,
		MenuCmd_AlignmentJustify,
		MenuCmd_AlignmentJustifyDescription,
		MenuCmd_ChangeFontName,
		MenuCmd_ChangeFontNameDescription,
		MenuCmd_ChangeFontSize,
		MenuCmd_ChangeFontSizeDescription,
		MenuCmd_ToggleFontBold,
		MenuCmd_ToggleFontBoldDescription,
		MenuCmd_ToggleFontItalic,
		MenuCmd_ToggleFontItalicDescription,
		MenuCmd_ToggleFontUnderline,
		MenuCmd_ToggleFontUnderlineDescription,
		MenuCmd_ToggleFontDoubleUnderline,
		MenuCmd_ToggleFontDoubleUnderlineDescription,
		MenuCmd_ToggleFontStrikeout,
		MenuCmd_ToggleFontStrikeoutDescription,
		MenuCmd_IncreaseFontSize,
		MenuCmd_IncreaseFontSizeDescription,
		MenuCmd_DecreaseFontSize,
		MenuCmd_DecreaseFontSizeDescription,
		MenuCmd_ChangeFontColor,
		MenuCmd_ChangeFontColorDescription,
		MenuCmd_ZoomIn,
		MenuCmd_ZoomInDescription,
		MenuCmd_ZoomOut,
		MenuCmd_ZoomOutDescription,
		MenuCmd_Zoom100Percent,
		MenuCmd_Zoom100PercentDescription,
		MenuCmd_Hyperlink,
		MenuCmd_HyperlinkDescription,
		MenuCmd_InsertComment,
		MenuCmd_InsertCommentDescription,
		MenuCmd_EditComment,
		MenuCmd_EditCommentDescription,
		MenuCmd_DeleteComment,
		MenuCmd_DeleteCommentDescription,
		MenuCmd_InsertFloatingObjectPicture,
		MenuCmd_InsertFloatingObjectPictureDescription,
		MenuCmd_InsertSymbol,
		MenuCmd_InsertSymbolDescription,
		MenuCmd_EditHyperlink,
		MenuCmd_EditHyperlinkDescription,
		MenuCmd_InsertHyperlink,
		MenuCmd_InsertHyperlinkDescription,
		MenuCmd_RemoveHyperlink,
		MenuCmd_RemoveHyperlinkDescription,
		MenuCmd_RemoveHyperlinks,
		MenuCmd_RemoveHyperlinksDescription,
		MenuCmd_OpenHyperlink,
		MenuCmd_OpenHyperlinkDescription,
		MenuCmd_PageOrientationCommandGroup,
		MenuCmd_PageOrientationCommandGroupDescription,
		MenuCmd_PageOrientationLandscape,
		MenuCmd_PageOrientationLandscapeDescription,
		MenuCmd_PageOrientationPortrait,
		MenuCmd_PageOrientationPortraitDescription,
		MenuCmd_PageMarginsNormal,
		MenuCmd_PageMarginsNormalDescription,
		MenuCmd_PageMarginsNarrow,
		MenuCmd_PageMarginsNarrowDescription,
		MenuCmd_PageMarginsWide,
		MenuCmd_PageMarginsWideDescription,
		MenuCmd_PageMarginsModerate,
		MenuCmd_PageMarginsModerateDescription,
		MenuCmd_FloatingObjectBringForwardCommandGroup,
		MenuCmd_FloatingObjectBringForwardCommandGroupDescription,
		MenuCmd_FloatingObjectBringForward,
		MenuCmd_FloatingObjectBringForwardDescription,
		MenuCmd_FloatingObjectBringToFront,
		MenuCmd_FloatingObjectBringToFrontDescription,
		MenuCmd_FloatingObjectSendBackwardCommandGroup,
		MenuCmd_FloatingObjectSendBackwardCommandGroupDescription,
		MenuCmd_FloatingObjectSendBackward,
		MenuCmd_FloatingObjectSendBackwardDescription,
		MenuCmd_FloatingObjectSendToBack,
		MenuCmd_FloatingObjectSendToBackDescription,
		Caption_EditHyperlinkForm,
		Caption_EditHyperlinkFormDescription,
		Caption_InsertHyperlinkForm,
		Caption_InsertHyperlinkFormDescription
	}
	#endregion
	#region OfficeLocalizer
	public class OfficeLocalizer : OfficeLocalizerBase<OfficeStringId> {
		static OfficeLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<OfficeStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(OfficeStringId.Msg_IsNotValid, "'{0}' is not a valid value for '{1}'");
			AddString(OfficeStringId.Msg_InternalError, "An internal error occurred");
			AddString(OfficeStringId.Msg_InvalidBeginUpdate, "Error: call to BeginUpdate inside BeginInit");
			AddString(OfficeStringId.Msg_InvalidEndUpdate, "Error: call to EndUpdate or CancelUpate without BeginUpdate or inside BeginInit");
			AddString(OfficeStringId.Msg_InvalidBeginInit, "Error: call to BeginInit inside BeginUpdate");
			AddString(OfficeStringId.Msg_InvalidEndInit, "Error: call to EndInit or CancelInit without BeginInit or inside BeginUpdate");
			AddString(OfficeStringId.Msg_InvalidCopyFromDocumentModel, "Error: source and destination document models are different");
			AddString(OfficeStringId.Msg_InvalidRemoveDataSource, "A data source cannot be deleted during the mail-merge process.");
			AddString(OfficeStringId.Msg_Loading, "Loading...");
			AddString(OfficeStringId.Msg_MagicNumberNotFound, "The file you are trying to open is in different format than specified by the file extension.");
			AddString(OfficeStringId.Msg_InvalidFontSize, "The number must be between {0} and {1}.");
			AddString(OfficeStringId.Msg_InvalidNumber, "This is not a valid number.");
			AddString(OfficeStringId.Msg_InvalidNumberConverterValue, "Value must be between {0} and {1}.");
			AddString(OfficeStringId.FileFilterDescription_AllFiles, "All Files");
			AddString(OfficeStringId.FileFilterDescription_AllSupportedFiles, "All Supported Files");
			AddString(OfficeStringId.FileFilterDescription_BitmapFiles, "Windows Bitmap");
			AddString(OfficeStringId.FileFilterDescription_JPEGFiles, "JPEG File Interchange Format");
			AddString(OfficeStringId.FileFilterDescription_PNGFiles, "Portable Network Graphics");
			AddString(OfficeStringId.FileFilterDescription_GifFiles, "Graphics Interchange Format");
			AddString(OfficeStringId.FileFilterDescription_TiffFiles, "Tag Image File Format");
			AddString(OfficeStringId.FileFilterDescription_EmfFiles, "Microsoft Enhanced Metafile");
			AddString(OfficeStringId.FileFilterDescription_WmfFiles, "Windows Metafile");
			AddString(OfficeStringId.UnitAbbreviation_Inch, "\"");
			AddString(OfficeStringId.UnitAbbreviation_Centimeter, " cm");
			AddString(OfficeStringId.UnitAbbreviation_Millimeter, " mm");
			AddString(OfficeStringId.UnitAbbreviation_Pica, " pc");
			AddString(OfficeStringId.UnitAbbreviation_Point, " pt");
			AddString(OfficeStringId.UnitAbbreviation_Percent, "%");
			AddString(OfficeStringId.Caption_UnitPercent, "Percent");
			AddString(OfficeStringId.Caption_UnitInches, "Inches");
			AddString(OfficeStringId.Caption_UnitCentimeters, "Centimeters");
			AddString(OfficeStringId.Caption_UnitMillimeters, "Millimeters");
			AddString(OfficeStringId.Caption_UnitPoints, "Points");
			AddString(OfficeStringId.MenuCmd_NewEmptyDocument, "New");
			AddString(OfficeStringId.MenuCmd_NewEmptyDocumentDescription, "Create a new document.");
			AddString(OfficeStringId.MenuCmd_LoadDocument, "Open");
			AddString(OfficeStringId.MenuCmd_LoadDocumentDescription, "Open a document.");
			AddString(OfficeStringId.MenuCmd_SaveDocument, "Save");
			AddString(OfficeStringId.MenuCmd_SaveDocumentDescription, "Save the document.");
			AddString(OfficeStringId.MenuCmd_SaveDocumentAs, "Save As");
			AddString(OfficeStringId.MenuCmd_SaveDocumentAsDescription, "Open the Save As dialog box to select a file format and save the document to a new location.");
			AddString(OfficeStringId.MenuCmd_Undo, "Undo");
			AddString(OfficeStringId.MenuCmd_UndoDescription, "Undo");
			AddString(OfficeStringId.MenuCmd_Redo, "Redo");
			AddString(OfficeStringId.MenuCmd_RedoDescription, "Redo");
			AddString(OfficeStringId.MenuCmd_ClearUndo, "ClearUndo");
			AddString(OfficeStringId.MenuCmd_ClearUndoDescription, "Clear Undo Buffer");
			AddString(OfficeStringId.MenuCmd_Print, "&Print");
			AddString(OfficeStringId.MenuCmd_PrintDescription, "Select a printer, number of copies, and other printing options before printing.");
			AddString(OfficeStringId.MenuCmd_QuickPrint, "&Quick Print");
			AddString(OfficeStringId.MenuCmd_QuickPrintDescription, "Send the document directly to the default printer without making changes.");
			AddString(OfficeStringId.MenuCmd_PrintPreview, "Print Pre&view");
			AddString(OfficeStringId.MenuCmd_PrintPreviewDescription, "Preview pages before printing.");
			AddString(OfficeStringId.MenuCmd_CutSelection, "Cut");
			AddString(OfficeStringId.MenuCmd_CutSelectionDescription, "Cut the selection from the document and put it on the Clipboard.");
			AddString(OfficeStringId.MenuCmd_CopySelection, "Copy");
			AddString(OfficeStringId.MenuCmd_CopySelectionDescription, "Copy the selection and put it on the Clipboard.");
			AddString(OfficeStringId.MenuCmd_Paste, "Paste");
			AddString(OfficeStringId.MenuCmd_PasteDescription, "Paste the contents of the Clipboard.");
			AddString(OfficeStringId.MenuCmd_ShowPasteSpecialForm, "Paste Special");
			AddString(OfficeStringId.MenuCmd_ShowPasteSpecialFormDescription, "Show the Paste Special dialog box.");
			AddString(OfficeStringId.MenuCmd_AlignmentLeft, "Align Text Left");
			AddString(OfficeStringId.MenuCmd_AlignmentLeftDescription, "Align text to the left.");
			AddString(OfficeStringId.MenuCmd_AlignmentCenter, "Center");
			AddString(OfficeStringId.MenuCmd_AlignmentCenterDescription, "Center text.");
			AddString(OfficeStringId.MenuCmd_AlignmentRight, "Align Text Right");
			AddString(OfficeStringId.MenuCmd_AlignmentRightDescription, "Align text to the right.");
			AddString(OfficeStringId.MenuCmd_AlignmentJustify, "Justify");
			AddString(OfficeStringId.MenuCmd_AlignmentJustifyDescription, "Align text to both left and right margins, adding extra space between words as necessary.\r\n\r\nThis creates a clean look along the left and right side of the page.");
			AddString(OfficeStringId.MenuCmd_ChangeFontName, "Font");
			AddString(OfficeStringId.MenuCmd_ChangeFontNameDescription, "Change the font face.");
			AddString(OfficeStringId.MenuCmd_ChangeFontSize, "Font Size");
			AddString(OfficeStringId.MenuCmd_ChangeFontSizeDescription, "Change the font size.");
			AddString(OfficeStringId.MenuCmd_ToggleFontBold, "Bold");
			AddString(OfficeStringId.MenuCmd_ToggleFontBoldDescription, "Make the selected text bold.");
			AddString(OfficeStringId.MenuCmd_ToggleFontItalic, "Italic");
			AddString(OfficeStringId.MenuCmd_ToggleFontItalicDescription, "Italicize the selected text.");
			AddString(OfficeStringId.MenuCmd_ToggleFontUnderline, "Underline");
			AddString(OfficeStringId.MenuCmd_ToggleFontUnderlineDescription, "Underline the selected text.");
			AddString(OfficeStringId.MenuCmd_ToggleFontDoubleUnderline, "Double Underline");
			AddString(OfficeStringId.MenuCmd_ToggleFontDoubleUnderlineDescription, "Double underline");
			AddString(OfficeStringId.MenuCmd_ToggleFontStrikeout, "Strikethrough");
			AddString(OfficeStringId.MenuCmd_ToggleFontStrikeoutDescription, "Draw a line through the middle of the selected text.");
			AddString(OfficeStringId.MenuCmd_IncreaseFontSize, "Grow Font");
			AddString(OfficeStringId.MenuCmd_IncreaseFontSizeDescription, "Increase the font size.");
			AddString(OfficeStringId.MenuCmd_DecreaseFontSize, "Shrink Font");
			AddString(OfficeStringId.MenuCmd_DecreaseFontSizeDescription, "Decrease the font size.");
			AddString(OfficeStringId.MenuCmd_ChangeFontColor, "Font Color");
			AddString(OfficeStringId.MenuCmd_ChangeFontColorDescription, "Change the text color.");
			AddString(OfficeStringId.MenuCmd_ZoomIn, "Zoom In");
			AddString(OfficeStringId.MenuCmd_ZoomInDescription, "Zoom In");
			AddString(OfficeStringId.MenuCmd_ZoomOut, "Zoom Out");
			AddString(OfficeStringId.MenuCmd_ZoomOutDescription, "Zoom Out");
			AddString(OfficeStringId.MenuCmd_Zoom100Percent, "100%");
			AddString(OfficeStringId.MenuCmd_Zoom100PercentDescription, "Zoom the document to 100% of the normal size.");
			AddString(OfficeStringId.MenuCmd_Hyperlink, "Hyperlink");
			AddString(OfficeStringId.MenuCmd_HyperlinkDescription, "Create a link to a Web page, a picture, an e-mail address, or a program.");
			AddString(OfficeStringId.MenuCmd_EditHyperlink, "Edit Hyperlink...");
			AddString(OfficeStringId.MenuCmd_EditHyperlinkDescription, "Edit Hyperlink...");
			AddString(OfficeStringId.MenuCmd_InsertHyperlink, "Hyperlink...");
			AddString(OfficeStringId.MenuCmd_InsertHyperlinkDescription, "Add a new hyperlink.");
			AddString(OfficeStringId.MenuCmd_RemoveHyperlink, "Remove Hyperlink");
			AddString(OfficeStringId.MenuCmd_RemoveHyperlinkDescription, "Remove Hyperlink");
			AddString(OfficeStringId.MenuCmd_RemoveHyperlinks, "Remove Hyperlinks");
			AddString(OfficeStringId.MenuCmd_RemoveHyperlinksDescription, "Remove Hyperlinks");
			AddString(OfficeStringId.MenuCmd_OpenHyperlink, "Open Hyperlink");
			AddString(OfficeStringId.MenuCmd_OpenHyperlinkDescription, "Open Hyperlink");
			AddString(OfficeStringId.MenuCmd_InsertComment, "New Comment");
			AddString(OfficeStringId.MenuCmd_InsertCommentDescription, "Add a note about this part of the document.");
			AddString(OfficeStringId.MenuCmd_EditComment, "Edit Comment");
			AddString(OfficeStringId.MenuCmd_EditCommentDescription, "Add a note about this part of the document.");
			AddString(OfficeStringId.MenuCmd_DeleteComment, "Delete");
			AddString(OfficeStringId.MenuCmd_DeleteCommentDescription, "Delete the selected comment.");
			AddString(OfficeStringId.MenuCmd_InsertFloatingObjectPicture, "Picture");
			AddString(OfficeStringId.MenuCmd_InsertFloatingObjectPictureDescription, "Insert a picture from a file.");
			AddString(OfficeStringId.MenuCmd_InsertSymbol, "Symbol");
			AddString(OfficeStringId.MenuCmd_InsertSymbolDescription, "Insert symbols that are not on your keyboard, such as copyright symbols, trademark symbols, paragraph marks and Unicode characters.");
			AddString(OfficeStringId.MenuCmd_PageOrientationCommandGroup, "Orientation");
			AddString(OfficeStringId.MenuCmd_PageOrientationCommandGroupDescription, "Switch the pages between portrait and landscape layouts.");
			AddString(OfficeStringId.MenuCmd_PageOrientationLandscape, "Landscape");
			AddString(OfficeStringId.MenuCmd_PageOrientationLandscapeDescription, " ");
			AddString(OfficeStringId.MenuCmd_PageOrientationPortrait, "Portrait");
			AddString(OfficeStringId.MenuCmd_PageOrientationPortraitDescription, " ");
			AddString(OfficeStringId.MenuCmd_PageMarginsNormal, "Normal\r\nTop:\t{1,10}\tBottom:\t{3,10}\r\nLeft:\t{0,10}\tRight:\t\t{2,10}");
			AddString(OfficeStringId.MenuCmd_PageMarginsNormalDescription, " ");
			AddString(OfficeStringId.MenuCmd_PageMarginsNarrow, "Narrow\r\nTop:\t{1,10}\tBottom:\t{3,10}\r\nLeft:\t{0,10}\tRight:\t\t{2,10}");
			AddString(OfficeStringId.MenuCmd_PageMarginsNarrowDescription, " ");
			AddString(OfficeStringId.MenuCmd_PageMarginsWide, "Wide\r\nTop:\t{1,10}\tBottom:\t{3,10}\r\nLeft:\t{0,10}\tRight:\t\t{2,10}");
			AddString(OfficeStringId.MenuCmd_PageMarginsWideDescription, " ");
			AddString(OfficeStringId.MenuCmd_PageMarginsModerate, "Moderate\r\nTop:\t{1,10}\tBottom:\t{3,10}\r\nLeft:\t{0,10}\tRight:\t\t{2,10}");
			AddString(OfficeStringId.MenuCmd_PageMarginsModerateDescription, " ");
			AddString(OfficeStringId.Caption_EditHyperlinkForm, "Edit Hyperlink");
			AddString(OfficeStringId.Caption_EditHyperlinkFormDescription, " ");
			AddString(OfficeStringId.Caption_InsertHyperlinkForm, "Insert Hyperlink");
			AddString(OfficeStringId.Caption_InsertHyperlinkFormDescription, " ");
			AddString(OfficeStringId.MenuCmd_FloatingObjectBringForwardCommandGroup, "Bring Forward");
			AddString(OfficeStringId.MenuCmd_FloatingObjectBringForwardCommandGroupDescription, "Bring the selected object forward one level, or bring it in front of all the other objects.");
			AddString(OfficeStringId.MenuCmd_FloatingObjectBringForward, "Bring Forward");
			AddString(OfficeStringId.MenuCmd_FloatingObjectBringForwardDescription, "Bring the selected object forward so that it is hidden by fewer objects that are in front of it.");
			AddString(OfficeStringId.MenuCmd_FloatingObjectBringToFront, "Bring to Front");
			AddString(OfficeStringId.MenuCmd_FloatingObjectBringToFrontDescription, "Bring the selected object in front of all other objects so that no part of it is hidden behind other objects.");
			AddString(OfficeStringId.MenuCmd_FloatingObjectSendBackwardCommandGroup, "Send Backward");
			AddString(OfficeStringId.MenuCmd_FloatingObjectSendBackwardCommandGroupDescription, "Send the selected object back one level, or send it behind all the other objects.");
			AddString(OfficeStringId.MenuCmd_FloatingObjectSendBackward, "Send Backward");
			AddString(OfficeStringId.MenuCmd_FloatingObjectSendBackwardDescription, "Send the selected object backward so that it is hidden by the objects that are in front of it.");
			AddString(OfficeStringId.MenuCmd_FloatingObjectSendToBack, "Send to Back");
			AddString(OfficeStringId.MenuCmd_FloatingObjectSendToBackDescription, "Send the selected object behind all other objects.");
		}
		#endregion
		public static XtraLocalizer<OfficeStringId> CreateDefaultLocalizer() {
			return new OfficeResLocalizer();
		}
		public static string GetString(OfficeStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<OfficeStringId> CreateResXLocalizer() {
			return new OfficeResLocalizer();
		}
	}
	#endregion
	#region OfficeResLocalizer
	public class OfficeResLocalizer : XtraResXLocalizer<OfficeStringId> {
		public OfficeResLocalizer()
			: base(new OfficeLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
#if DXPORTABLE
			return new ResourceManager("DevExpress.Office.Core.LocalizationRes", typeof(OfficeResLocalizer).GetTypeInfo().Assembly);
#else
			return new ResourceManager("DevExpress.Office.LocalizationRes", typeof(OfficeResLocalizer).Assembly);
#endif
		}
	}
	#endregion
}
namespace DevExpress.Office.Localization.Internal {
	public abstract class OfficeLocalizerBase<T> : XtraLocalizer<T> where T : struct {
		protected override void AddString(T id, string str) {
			Dictionary<T, string> table = XtraLocalizierHelper<T>.GetStringTable(this);
			table[id] = str;
		}
	}
}
