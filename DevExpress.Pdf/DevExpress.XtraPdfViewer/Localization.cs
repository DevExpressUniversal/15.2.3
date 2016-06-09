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
using System.Globalization;
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraPdfViewer.Localization {
	public enum XtraPdfViewerStringId {
		CommandExportFormDataCaption,
		CommandExportFormDataDescription,
		CommandImportFormDataCaption,
		CommandImportFormDataDescription,
		CommandOpenFileCaption,
		CommandOpenFileDescription,
		CommandSaveAsFileCaption,
		CommandSaveAsFileDescription,
		CommandPrintFileCaption,
		CommandPrintFileDescription,
		CommandPreviousPageCaption,
		CommandPreviousPageDescription,
		CommandNextPageCaption,
		CommandNextPageDescription,
		CommandFindTextCaption,
		CommandFindTextDescription,
		CommandZoomInCaption,
		CommandZoomInDescription,
		CommandZoomInShortcutCaption,
		CommandZoomOutCaption,
		CommandZoomOutDescription,
		CommandZoomOutShortcutCaption,
		CommandViewExactZoomListCaption,
		CommandViewExactZoomListDescription,
		CommandZoom10Caption,
		CommandZoom25Caption,
		CommandZoom50Caption,
		CommandZoom75Caption,
		CommandZoom100Caption,
		CommandZoom125Caption,
		CommandZoom150Caption,
		CommandZoom200Caption,
		CommandZoom400Caption,
		CommandZoom500Caption,
		CommandSetActualSizeZoomModeCaption,
		CommandSetPageLevelZoomModeCaption,
		CommandSetFitWidthZoomModeCaption,
		CommandSetFitVisibleZoomModeCaption,
		CommandRotatePageClockwiseCaption,
		CommandRotatePageCounterclockwiseCaption,
		CommandPreviousViewCaption,
		CommandPreviousViewDescription,
		CommandNextViewCaption,
		CommandNextViewDescription,
		CommandHandToolCaption,
		CommandHandToolDescription,
		CommandSelectToolCaption,
		CommandSelectToolDescription,
		CommandSelectAllCaption,
		CommandSelectAllDescription,
		CommandCopyCaption,
		CommandCopyDescription,
		CommandShowDocumentProperties,
		CommandOutlinesWrapLongLinesCaption,
		CommandOutlinesTextSizeCaption,
		CommandOutlinesTextSizeToLargeCaption,
		CommandOutlinesTextSizeToMediumCaption,
		CommandOutlinesTextSizeToSmallCaption,
		CommandOutlinesGotoCaption,
		CommandOutlinesHideAfterUseCaption,
		CommandOutlinesExpandCurrentCaption,
		CommandOutlinesPrintPagesCaption,
		CommandOutlinesPrintSectionsCaption,
		CommandOutlinesExpandTopLevelCaption,
		CommandOutlinesCollapseTopLevelCaption,
		BarCaption,
		FormDataBarCaption,
		FileRibbonGroupCaption,
		NavigationRibbonGroupCaption,
		SelectionRibbonGroupCaption,
		ZoomRibbonGroupCaption,
		FormDataRibbonPageGroupCaption,
		DesignerActionMethodLoadPdf,
		DesignerActionMethodUnloadPdf,
		DesignerActionMethodCreateRibbon,
		DesignerActionMethodCreateBars,
		DesignerActionMethodUndockInParentContainer,
		DesignerActionMethodDockInParentContainer,
		DesignerActionListCreateRibbonTransaction,
		DesignerActionListCreateBarsTransaction,
		PDFFileFilter,
		FormDataFileFilter,
		PRNFileFilter,
		UnitKiloBytes,
		UnitMegaBytes,
		UnitGigaBytes,
		UnitTeraBytes,
		UnitPetaBytes,
		UnitExaBytes,
		UnitZettaBytes,
		FileSizeInBytes,
		FileSize,
		PageSize,
		FindControlCaseSensitive,
		FindControlWholeWordsOnly,
		MessageErrorCaption,
		MessageLoadingError,
		MessageSaveAsError,
		MessageImportError,
		MessageExportError,
		MessageClipboardError,
		MessageAddCommandConstructorError,
		MessageCurrentPageNumberOutOfRange,
		MessageIncorrectPageNumber,
		MessageIncorrectMaxPrintingDpi,
		MessageSearchCaption,
		MessageSearchFinished,
		MessageSearchFinishedNoMatchesFound,
		MessageDocumentIsProtected,
		MessageIncorrectPassword,
		MessageSecurityWarningCaption,
		MessageSecurityWarningUriOpening,
		MessageSecurityWarningFileAttachmentOpening,
		MessageDocumentClosing,
		MessageDocumentClosingCaption,
		MessagePrintError,
		MessageIncorrectNavigationPaneVisibility,
		NavigationPaneOutlinesPageCaption,
		NavigationPaneAttachmentsPageCaption
	}
	[DXToolboxItem(false)]
	public class XtraPdfViewerLocalizer : XtraLocalizer<XtraPdfViewerStringId> {
		static XtraPdfViewerLocalizer() {
			if (GetActiveLocalizerProvider() == null)
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraPdfViewerStringId>(new XtraPdfViewerResLocalizer()));
		}
		public static new XtraLocalizer<XtraPdfViewerStringId> Active {
			get { return XtraLocalizer<XtraPdfViewerStringId>.Active; }
			set {
				if (GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<XtraPdfViewerStringId> == null) {
					SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<XtraPdfViewerStringId>(value));
					RaiseActiveChanged();
				}
				else
					XtraLocalizer<XtraPdfViewerStringId>.Active = value;
			}
		}
		public static string GetString(XtraPdfViewerStringId id) {
			return Active.GetLocalizedString(id);
		}
		protected override void PopulateStringTable() {
			AddString(XtraPdfViewerStringId.CommandExportFormDataCaption, "Export");
			AddString(XtraPdfViewerStringId.CommandExportFormDataDescription, "Export form data to a file.");
			AddString(XtraPdfViewerStringId.CommandImportFormDataCaption, "Import");
			AddString(XtraPdfViewerStringId.CommandImportFormDataDescription, "Import form data from a file.");
			AddString(XtraPdfViewerStringId.CommandOpenFileCaption, "Open");
			AddString(XtraPdfViewerStringId.CommandOpenFileDescription, "Open a PDF file.");
			AddString(XtraPdfViewerStringId.CommandSaveAsFileCaption, "Save As");
			AddString(XtraPdfViewerStringId.CommandSaveAsFileDescription, "Save the PDF file.");
			AddString(XtraPdfViewerStringId.CommandPrintFileCaption, "Print");
			AddString(XtraPdfViewerStringId.CommandPrintFileDescription, "Print file.");
			AddString(XtraPdfViewerStringId.CommandPreviousPageCaption, "Previous");
			AddString(XtraPdfViewerStringId.CommandPreviousPageDescription, "Show previous page.");
			AddString(XtraPdfViewerStringId.CommandNextPageCaption, "Next");
			AddString(XtraPdfViewerStringId.CommandNextPageDescription, "Show next page.");
			AddString(XtraPdfViewerStringId.CommandFindTextCaption, "Find");
			AddString(XtraPdfViewerStringId.CommandFindTextDescription, "Find text.");
			AddString(XtraPdfViewerStringId.CommandZoomInCaption, "Zoom In");
			AddString(XtraPdfViewerStringId.CommandZoomInDescription, "Zoom in to get a close-up view of the PDF document.");
			AddString(XtraPdfViewerStringId.CommandZoomInShortcutCaption, " (Ctrl + Plus)");
			AddString(XtraPdfViewerStringId.CommandZoomOutCaption, "Zoom Out");
			AddString(XtraPdfViewerStringId.CommandZoomOutDescription, "Zoom out to see more of the page at a reduces size.");
			AddString(XtraPdfViewerStringId.CommandZoomOutShortcutCaption, " (Ctrl + Minus)");
			AddString(XtraPdfViewerStringId.CommandViewExactZoomListCaption, "Zoom");
			AddString(XtraPdfViewerStringId.CommandViewExactZoomListDescription, "Change the zoom level of the PDF document.");
			AddString(XtraPdfViewerStringId.CommandZoom10Caption, "10%");
			AddString(XtraPdfViewerStringId.CommandZoom25Caption, "25%");
			AddString(XtraPdfViewerStringId.CommandZoom50Caption, "50%");
			AddString(XtraPdfViewerStringId.CommandZoom75Caption, "75%");
			AddString(XtraPdfViewerStringId.CommandZoom100Caption, "100%");
			AddString(XtraPdfViewerStringId.CommandZoom125Caption, "125%");
			AddString(XtraPdfViewerStringId.CommandZoom150Caption, "150%");
			AddString(XtraPdfViewerStringId.CommandZoom200Caption, "200%");
			AddString(XtraPdfViewerStringId.CommandZoom400Caption, "400%");
			AddString(XtraPdfViewerStringId.CommandZoom500Caption, "500%");
			AddString(XtraPdfViewerStringId.CommandSetActualSizeZoomModeCaption, "Actual Size");
			AddString(XtraPdfViewerStringId.CommandSetPageLevelZoomModeCaption, "Zoom to Page Level");
			AddString(XtraPdfViewerStringId.CommandSetFitWidthZoomModeCaption, "Fit Width");
			AddString(XtraPdfViewerStringId.CommandSetFitVisibleZoomModeCaption, "Fit Visible");
			AddString(XtraPdfViewerStringId.CommandRotatePageClockwiseCaption, "Rotate Clockwise");
			AddString(XtraPdfViewerStringId.CommandRotatePageCounterclockwiseCaption, "Rotate Counterclockwise");
			AddString(XtraPdfViewerStringId.CommandPreviousViewCaption, "Previous View");
			AddString(XtraPdfViewerStringId.CommandPreviousViewDescription, "Previous View");
			AddString(XtraPdfViewerStringId.CommandNextViewCaption, "Next View");
			AddString(XtraPdfViewerStringId.CommandNextViewDescription, "Next View");
			AddString(XtraPdfViewerStringId.CommandSelectAllCaption, "Select All");
			AddString(XtraPdfViewerStringId.CommandSelectAllDescription, "Select All");
			AddString(XtraPdfViewerStringId.CommandHandToolCaption, "Hand Tool");
			AddString(XtraPdfViewerStringId.CommandHandToolDescription, "Hand Tool");
			AddString(XtraPdfViewerStringId.CommandSelectToolCaption, "Select Tool");
			AddString(XtraPdfViewerStringId.CommandSelectToolDescription, "Select Tool");
			AddString(XtraPdfViewerStringId.CommandCopyCaption, "Copy");
			AddString(XtraPdfViewerStringId.CommandCopyDescription, "Copy PDF content to the clipboard");
			AddString(XtraPdfViewerStringId.CommandShowDocumentProperties, "Document properties");
			AddString(XtraPdfViewerStringId.CommandOutlinesWrapLongLinesCaption, "Wrap Long Bookmarks");
			AddString(XtraPdfViewerStringId.CommandOutlinesTextSizeCaption, "Text Size");
			AddString(XtraPdfViewerStringId.CommandOutlinesTextSizeToLargeCaption, "Large");
			AddString(XtraPdfViewerStringId.CommandOutlinesTextSizeToMediumCaption, "Medium");
			AddString(XtraPdfViewerStringId.CommandOutlinesTextSizeToSmallCaption, "Small");
			AddString(XtraPdfViewerStringId.CommandOutlinesGotoCaption, "Go to Bookmark");
			AddString(XtraPdfViewerStringId.CommandOutlinesHideAfterUseCaption, "Hide After Use");
			AddString(XtraPdfViewerStringId.CommandOutlinesExpandCurrentCaption, "Expand Current Bookmark");
			AddString(XtraPdfViewerStringId.CommandOutlinesPrintPagesCaption, "Print Page(s)");
			AddString(XtraPdfViewerStringId.CommandOutlinesPrintSectionsCaption, "Print Section(s)");
			AddString(XtraPdfViewerStringId.CommandOutlinesCollapseTopLevelCaption, "Collapse Top-Level Bookmarks");
			AddString(XtraPdfViewerStringId.CommandOutlinesExpandTopLevelCaption, "Expand Top-Level Bookmarks");
			AddString(XtraPdfViewerStringId.BarCaption, "PDF Viewer");
			AddString(XtraPdfViewerStringId.FormDataBarCaption, "Interactive Form");
			AddString(XtraPdfViewerStringId.FileRibbonGroupCaption, "File");
			AddString(XtraPdfViewerStringId.NavigationRibbonGroupCaption, "Navigation");
			AddString(XtraPdfViewerStringId.SelectionRibbonGroupCaption, "Selection");
			AddString(XtraPdfViewerStringId.ZoomRibbonGroupCaption, "Zoom");
			AddString(XtraPdfViewerStringId.FormDataRibbonPageGroupCaption, "Form Data");
			AddString(XtraPdfViewerStringId.DesignerActionMethodLoadPdf, "Load PDF file...");
			AddString(XtraPdfViewerStringId.DesignerActionMethodUnloadPdf, "Unload PDF file");
			AddString(XtraPdfViewerStringId.DesignerActionMethodCreateRibbon, "Create Ribbon");
			AddString(XtraPdfViewerStringId.DesignerActionMethodCreateBars, "Create Bars");
			AddString(XtraPdfViewerStringId.DesignerActionMethodUndockInParentContainer, "Undock in parent container");
			AddString(XtraPdfViewerStringId.DesignerActionMethodDockInParentContainer, "Dock in parent container");
			AddString(XtraPdfViewerStringId.DesignerActionListCreateBarsTransaction, "Create Bars");
			AddString(XtraPdfViewerStringId.DesignerActionListCreateRibbonTransaction, "Create Ribbon");
			AddString(XtraPdfViewerStringId.PDFFileFilter, "PDF Files (.pdf)|*.pdf");
			AddString(XtraPdfViewerStringId.FormDataFileFilter, "FDF Files (.fdf)|*.fdf|XFDF Files (.xfdf)|*.xfdf|XML Files (.xml)|*.xml|TXT Files (.txt)|*.txt");
			AddString(XtraPdfViewerStringId.PRNFileFilter, "Printable Files (.prn)|*.prn|All Files (.*)|*.*");
			AddString(XtraPdfViewerStringId.UnitKiloBytes, "KB");
			AddString(XtraPdfViewerStringId.UnitMegaBytes, "MB");
			AddString(XtraPdfViewerStringId.UnitGigaBytes, "GB");
			AddString(XtraPdfViewerStringId.UnitTeraBytes, "TB");
			AddString(XtraPdfViewerStringId.UnitPetaBytes, "PB");
			AddString(XtraPdfViewerStringId.UnitExaBytes, "EB");
			AddString(XtraPdfViewerStringId.UnitZettaBytes, "ZB");
			AddString(XtraPdfViewerStringId.FileSizeInBytes, "{0} Bytes");
			AddString(XtraPdfViewerStringId.FileSize, "{0:0.00} {1} ({2} Bytes)");
			AddString(XtraPdfViewerStringId.PageSize, "{0:0.00} x {1:0.00} in");
			AddString(XtraPdfViewerStringId.FindControlCaseSensitive, "Case Sensitive");
			AddString(XtraPdfViewerStringId.FindControlWholeWordsOnly, "Whole Words Only");
			AddString(XtraPdfViewerStringId.MessageErrorCaption, "Error");
			AddString(XtraPdfViewerStringId.MessageLoadingError, "Unable to load the PDF document because the following file is not available or it is not a valid PDF document.\r\n{0}\r\nPlease ensure that the application can access this file and that it is valid, or specify a different file.");
			AddString(XtraPdfViewerStringId.MessageSaveAsError, "Cannot save the PDF with the following name: {0}.");
			AddString(XtraPdfViewerStringId.MessageImportError, "Unable to import the specified data into the document form.\r\n{0}\r\n Please ensure that the provided data meets the {1} specification.");
			AddString(XtraPdfViewerStringId.MessageExportError, "An error occurred while exporting the form data from the document.");
			AddString(XtraPdfViewerStringId.MessageClipboardError, "Unable to copy data to the clipboard.");
			AddString(XtraPdfViewerStringId.MessageAddCommandConstructorError, "Cannot find a constructor with a PdfViewer type parameter in the {0} class");
			AddString(XtraPdfViewerStringId.MessageCurrentPageNumberOutOfRange, "The current page number should be greater than 0.");
			AddString(XtraPdfViewerStringId.MessageIncorrectPageNumber, "The page number should be greater than 0, and less than or equal to the document page count.");
			AddString(XtraPdfViewerStringId.MessageIncorrectMaxPrintingDpi, "The maximum printing DPI value should be greater than or equal to 0.");
			AddString(XtraPdfViewerStringId.MessageSearchCaption, "Find");
			AddString(XtraPdfViewerStringId.MessageSearchFinished, "Finished searching throughout the document. No more matches were found.");
			AddString(XtraPdfViewerStringId.MessageSearchFinishedNoMatchesFound, "Finished searching throughout the document. No matches were found.");
			AddString(XtraPdfViewerStringId.MessageDocumentIsProtected, "{0} is protected");
			AddString(XtraPdfViewerStringId.MessageIncorrectPassword, "The password is incorrect. Please make sure that Caps Lock is not enabled.");
			AddString(XtraPdfViewerStringId.MessageSecurityWarningCaption, "Security Warning");
			AddString(XtraPdfViewerStringId.MessageSecurityWarningUriOpening, "The document requests to access an external resource to open the following URI (uniform resource identifier):\r\n'{0}'\r\nDo you want to open it?");
			AddString(XtraPdfViewerStringId.MessageSecurityWarningFileAttachmentOpening, "The document requests to access an external application to open this file:\r\n'{0}'\r\nDo you want to open it?");
			AddString(XtraPdfViewerStringId.MessageDocumentClosing, "Do you want to save the changes to the document before closing it?");
			AddString(XtraPdfViewerStringId.MessageDocumentClosingCaption, "Document Closing");
			AddString(XtraPdfViewerStringId.MessagePrintError, "Unable to print the document. Please contact your system administrator.");
			AddString(XtraPdfViewerStringId.MessageIncorrectNavigationPaneVisibility, "The current navigation pane visibility cannot be set to PdfNavigationPaneVisibility.Default.");
			AddString(XtraPdfViewerStringId.NavigationPaneOutlinesPageCaption, "Bookmarks");
			AddString(XtraPdfViewerStringId.NavigationPaneAttachmentsPageCaption, "Attachments");
		}
		public override XtraLocalizer<XtraPdfViewerStringId> CreateResXLocalizer() {
			return new XtraPdfViewerResLocalizer();
		}
	}
	public class XtraPdfViewerResLocalizer : XtraPdfViewerLocalizer {
		ResourceManager manager;
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		public XtraPdfViewerResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if (manager != null)
				manager.ReleaseAllResources();
			manager = new ResourceManager("DevExpress.XtraPdfViewer.LocalizationRes", GetType().Assembly);
		}
		public override string GetLocalizedString(XtraPdfViewerStringId id) {
			return Manager.GetString("XtraPdfViewerStringId." + id) ?? String.Empty;
		}
	}
}
