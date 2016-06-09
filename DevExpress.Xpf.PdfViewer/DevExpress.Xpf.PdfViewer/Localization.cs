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

using System.Windows.Markup;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;
using System;
using System.Resources;
using System.Globalization;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfViewerStringIdConverter : StringIdConverter<PdfViewerStringId> {
		protected override XtraLocalizer<PdfViewerStringId> Localizer {
			get { return PdfViewerLocalizer.Active; }
		}
	}
	public enum PdfViewerStringId {
		MessageEnterPasswordLabel,
		MessageEnterUrlLabel,
		MessageIncorrectUrl,
		MessageIncorrectPassword,
		MessageDocumentIsProtected,
		MessageSearchFinished,
		MessageSearchFinishedNoMatchesFound,
		MessageSearchCaption,
		MessageDocumentHasNoPages,
		CommandOpenFileCaption,
		CommandOpenFileDescription,
		CommandOpenFileFromWebCaption,
		CommandOpenFileFromWebDescription,
		CommandCloseFileCaption,
		CommandCloseFileDescription,
		CommandPreviousPageCaption,
		CommandPreviousPageDescription,
		CommandNextPageCaption,
		CommandNextPageDescription,
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
		CommandViewCursorModeListGroupCaption,
		CommandCursorModeHandToolCaption,
		CommandCursorModeSelectToolCaption,
		CommandCursorModeMarqueeZoomCaption,
		CommandClockwiseRotateCaption,
		CommandCounterclockwiseRotateCaption,
		RotateRibbonGroupCaption,
		CommandPreviousViewCaption,
		CommandNextViewCaption,
		CommandImportCaption,
		CommandExportCaption,
		CommandImportDescription,
		CommandExportDescription,
		BarCaption,
		BarFormDataCaption,
		FileRibbonGroupCaption,
		NavigationRibbonGroupCaption,
		ZoomRibbonGroupCaption,
		FormDataRibbonGroupCaption,
		RecentDocumentsCaption,
		OpenDocumentCaption,
		SavingDocumentCaption,
		SavingDocumentMessage,
		LoadingDocumentCaption,
		CancelButtonCaption,
		PdfFileFilter,
		FormDataFileFilter,
		PdfFileExtension,
		MessageImportError,
		MessageExportError,
		MessageErrorCaption,
		MessageLoadingError,
		MessageAddCommandConstructorError,
		MessageErrorZoomFactorOutOfRange,
		MessageErrorCurrentPageNumberOutOfRange,
		CommandPrintFileCaption,
		CommandPrintFileDescription,
		CommandShowDocumentProperties,
		PropertiesCaption,
		FileSizeInBytes,
		FileSize,
		PageSize,
		UnitKiloBytes,
		UnitMegaBytes,
		UnitGigaBytes,
		UnitTeraBytes,
		UnitPetaBytes,
		UnitExaBytes,
		UnitZettaBytes,
		MessageCantLoadDocument,
		CommandSelectAllCaption,
		CommandSaveAsCaption,
		CommandCopyCaption,
		OpenDocumentFromWebCaption,
		#region Print
		PrintDialogTitle,
		PrintDialogPrinterName,
		PrintDialogPrinterPreferences,
		PrintDialogStatus,
		PrintDialogLocation,
		PrintDialogComment,
		PrintDialogDocumentsInQueue,
		PrintDialogPrintingDpi,
		PrintDialogNumberOfCopies,
		PrintDialogCollate,
		PrintDialogPageRange,
		PrintDialogPageRangeAll,
		PrintDialogPageRangeCurrent,
		PrintDialogPageRangePages,
		PrintDialogPageRangePagesExample,
		PrintDialogPageSizing,
		PrintDialogPageSizingFit,
		PrintDialogPageSizingActualSize,
		PrintDialogPageSizingCustomScale,
		PrintDialogOrientation,
		PrintDialogPaperSource,
		PrintDialogFilePath,
		PrintDialogPrintToFile,
		PrintFileExtension,
		PrintFileFilter,
		PrintDialogPaginationOf,
		PrintDialogPrintButtonCaption,
		PrintDialogPrintOrientationAuto,
		PrintDialogPrintOrientationLandscape,
		PrintDialogPrintOrientationPortrait,
		#endregion
		#region Properties
		PropertiesDescriptionCaption,
		PropertiesRevisionCaption,
		PropertiesAdvancedCaption,
		PropertiesFile,
		PropertiesTitle,
		PropertiesAuthor,
		PropertiesSubject,
		PropertiesKeywords,
		PropertiesCreated,
		PropertiesModified,
		PropertiesApplication,
		PropertiesProducer,
		PropertiesVersion,
		PropertiesLocation,
		PropertiesFileSize,
		PropertiesNumberOfPages,
		PropertiesPageSize,
		#endregion
		#region OutlinesViewer
		OutlinesViewerExpandCurrentCaption,
		OutlinesViewerExpandTopLevelCaption,
		OutlinesViewerCollapseTopLevelCaption,
		OutlinesViewerGoToCaption,
		OutlinesViewerPrintCaption,
		OutlinesViewerPrintSectionCaption,
		OutlinesViewerWrapLongItemsCaption,
		OutlinesViewerHideAfterUseCaption,
		OutlinesViewerPanelCaption,
		#endregion
		#region AttachmentsViewer
		AttachmentsViewerOpenCaption,
		AttachmentsViewerOpenDescription,
		AttachmentsViewerSaveCaption,
		AttachmentsViewerSaveDescription,
		AttachmentsViewerPanelCaption,
		MessageFileAttachmentOpening,
		#endregion
		SaveChangesMessage,
		SaveChangesCaption,
		MessageSecurityWarningCaption,
		MessageSecurityWarningUriOpening,
		CommandPageLayoutCaption,
		CommandPageLayoutDescription,
		CommandSinglePageView,
		CommandOneColumnView,
		CommandTwoPageView,
		CommandTwoColumnView,
		CommandShowCoverPage,
		MessageSaveAsError,
	}
	public class PdfViewerLocalizer : DXLocalizer<PdfViewerStringId> {
		static PdfViewerLocalizer() {
			if (GetActiveLocalizerProvider() == null)
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PdfViewerStringId>(new PdfViewerControlResXLocalizer()));
		}
		public static new XtraLocalizer<PdfViewerStringId> Active {
			get { return XtraLocalizer<PdfViewerStringId>.Active; }
			set {
				if (GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<PdfViewerStringId> == null) {
					SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PdfViewerStringId>(value));
					RaiseActiveChanged();
				}
				else {
					XtraLocalizer<PdfViewerStringId>.Active = value;
				}
			}
		}
		public static XtraLocalizer<PdfViewerStringId> CreateDefaultLocalizer() {
			return new PdfViewerLocalizer();
		}
		public static string GetString(PdfViewerStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<PdfViewerStringId> CreateResXLocalizer() {
			return new PdfViewerControlResXLocalizer();
		}
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		protected override void PopulateStringTable() {
			AddString(PdfViewerStringId.MessageEnterPasswordLabel, "Please enter a Document Open password");
			AddString(PdfViewerStringId.MessageEnterUrlLabel, "Please enter a Document url address");
			AddString(PdfViewerStringId.MessageIncorrectPassword, "The password is incorrect. Please make sure that Caps Lock is not enabled.");
			AddString(PdfViewerStringId.MessageCantLoadDocument, "There was an error loading a document.");
			AddString(PdfViewerStringId.MessageDocumentHasNoPages, "The document does not contain any pages.");
			AddString(PdfViewerStringId.CommandOpenFileCaption, "Open");
			AddString(PdfViewerStringId.CommandOpenFileDescription, "Open a PDF file.");
			AddString(PdfViewerStringId.CommandOpenFileFromWebCaption, "Open from web");
			AddString(PdfViewerStringId.CommandOpenFileFromWebDescription, "Open a PDF file from web.");
			AddString(PdfViewerStringId.CommandCloseFileCaption, "Close");
			AddString(PdfViewerStringId.CommandCloseFileDescription, "Close a PDF file.");
			AddString(PdfViewerStringId.CommandPreviousPageCaption, "Previous");
			AddString(PdfViewerStringId.CommandPreviousPageDescription, "Show previous page.");
			AddString(PdfViewerStringId.CommandNextPageCaption, "Next");
			AddString(PdfViewerStringId.CommandNextPageDescription, "Show next page.");
			AddString(PdfViewerStringId.CommandZoomInCaption, "Zoom In");
			AddString(PdfViewerStringId.CommandZoomInDescription, "Zoom in to get a close-up view of the PDF document.");
			AddString(PdfViewerStringId.CommandZoomInShortcutCaption, " (Ctrl + Plus)");
			AddString(PdfViewerStringId.CommandZoomOutCaption, "Zoom Out");
			AddString(PdfViewerStringId.CommandZoomOutDescription, "Zoom out to see more of the page at a reduces size.");
			AddString(PdfViewerStringId.CommandZoomOutShortcutCaption, " (Ctrl + Minus)");
			AddString(PdfViewerStringId.CommandViewExactZoomListCaption, "Zoom");
			AddString(PdfViewerStringId.CommandViewExactZoomListDescription, "Change the zoom level of the PDF document.");
			AddString(PdfViewerStringId.CommandZoom10Caption, "10%");
			AddString(PdfViewerStringId.CommandZoom25Caption, "25%");
			AddString(PdfViewerStringId.CommandZoom50Caption, "50%");
			AddString(PdfViewerStringId.CommandZoom75Caption, "75%");
			AddString(PdfViewerStringId.CommandZoom100Caption, "100%");
			AddString(PdfViewerStringId.CommandZoom125Caption, "125%");
			AddString(PdfViewerStringId.CommandZoom150Caption, "150%");
			AddString(PdfViewerStringId.CommandZoom200Caption, "200%");
			AddString(PdfViewerStringId.CommandZoom400Caption, "400%");
			AddString(PdfViewerStringId.CommandZoom500Caption, "500%");
			AddString(PdfViewerStringId.CommandSetActualSizeZoomModeCaption, "Actual Size");
			AddString(PdfViewerStringId.CommandSetPageLevelZoomModeCaption, "Zoom to Page Level");
			AddString(PdfViewerStringId.CommandSetFitWidthZoomModeCaption, "Fit Width");
			AddString(PdfViewerStringId.CommandSetFitVisibleZoomModeCaption, "Fit Visible");
			AddString(PdfViewerStringId.CommandPrintFileCaption, "Print");
			AddString(PdfViewerStringId.CommandPrintFileDescription, "Print file.");
			AddString(PdfViewerStringId.CommandShowDocumentProperties, "Document properties...");
			AddString(PdfViewerStringId.CommandViewCursorModeListGroupCaption, "Cursor mode");
			AddString(PdfViewerStringId.CommandCursorModeHandToolCaption, "Hand tool");
			AddString(PdfViewerStringId.CommandCursorModeSelectToolCaption, "Select tool");
			AddString(PdfViewerStringId.CommandCursorModeMarqueeZoomCaption, "Marquee zoom");
			AddString(PdfViewerStringId.CommandClockwiseRotateCaption, "Clockwise rotate");
			AddString(PdfViewerStringId.CommandCounterclockwiseRotateCaption, "Counterclockwise rotate");
			AddString(PdfViewerStringId.CommandPreviousViewCaption, "Previous view");
			AddString(PdfViewerStringId.CommandNextViewCaption, "Next view");
			AddString(PdfViewerStringId.CommandSelectAllCaption, "Select All");
			AddString(PdfViewerStringId.CommandSaveAsCaption, "Save As");
			AddString(PdfViewerStringId.CommandCopyCaption, "Copy");
			AddString(PdfViewerStringId.CommandImportCaption, "Import");
			AddString(PdfViewerStringId.CommandImportDescription, "Import form data from the file.");
			AddString(PdfViewerStringId.CommandExportCaption, "Export");
			AddString(PdfViewerStringId.CommandExportDescription, "Export form data to the file.");
			AddString(PdfViewerStringId.RotateRibbonGroupCaption, "Rotate view");
			AddString(PdfViewerStringId.BarCaption, "PDF Viewer");
			AddString(PdfViewerStringId.BarFormDataCaption, "Form Data");
			AddString(PdfViewerStringId.FileRibbonGroupCaption, "File");
			AddString(PdfViewerStringId.NavigationRibbonGroupCaption, "Navigation");
			AddString(PdfViewerStringId.ZoomRibbonGroupCaption, "Zoom");
			AddString(PdfViewerStringId.FormDataRibbonGroupCaption, "Form Data");
			AddString(PdfViewerStringId.RecentDocumentsCaption, "Recent documents");
			AddString(PdfViewerStringId.OpenDocumentCaption, "Open...");
			AddString(PdfViewerStringId.SavingDocumentCaption, "Saving");
			AddString(PdfViewerStringId.SavingDocumentMessage, "Saving... Please wait.");
			AddString(PdfViewerStringId.LoadingDocumentCaption, "Loading... Please wait.");
			AddString(PdfViewerStringId.CancelButtonCaption, "Cancel");
			AddString(PdfViewerStringId.PdfFileExtension, ".pdf");
			AddString(PdfViewerStringId.PdfFileFilter, "PDF Files (.pdf)|*.pdf");
			AddString(PdfViewerStringId.FormDataFileFilter, "FDF Files (.fdf)|*.fdf|XML Files (.xml)|*.xml");
			AddString(PdfViewerStringId.MessageImportError, "Unable to import the specified data into the document form.\r\n{0}\r\nPlease ensure that the provided data meets the {1} specification.");
			AddString(PdfViewerStringId.MessageExportError, "An error occurred while exporting the document's form data.");
			AddString(PdfViewerStringId.MessageErrorCaption, "Error");
			AddString(PdfViewerStringId.MessageLoadingError, "Unable to load the PDF document because the following file is not available or it is not a valid PDF document.\r\n{0}\r\nPlease ensure that the application can access this file and that it is valid, or specify a different file.");
			AddString(PdfViewerStringId.MessageAddCommandConstructorError, "Cannot find a constructor with a PdfViewer type parameter in the {0} class");
			AddString(PdfViewerStringId.MessageErrorZoomFactorOutOfRange, "The zoom factor should be greater than or equal to {0} and less than or equal to {1}.");
			AddString(PdfViewerStringId.MessageErrorCurrentPageNumberOutOfRange, "The current page number value should be greater than 0.");
			AddString(PdfViewerStringId.MessageDocumentIsProtected, "{0} is protected");
			AddString(PdfViewerStringId.MessageSearchFinished, "Finished searching throughout the document. No more matches were found.");
			AddString(PdfViewerStringId.MessageSearchFinishedNoMatchesFound, "Finished searching throughout the document. No matches were found.");
			AddString(PdfViewerStringId.MessageSearchCaption, "Find");
			AddString(PdfViewerStringId.OpenDocumentFromWebCaption, "Open from web");
			AddString(PdfViewerStringId.MessageIncorrectUrl, "{0} is not valid url address.");
			AddString(PdfViewerStringId.PropertiesCaption, "Properties");
			AddString(PdfViewerStringId.FileSizeInBytes, "{0} Bytes");
			AddString(PdfViewerStringId.FileSize, "{0:0.00} {1} ({2} Bytes)");
			AddString(PdfViewerStringId.PageSize, "{0:0.00} x {1:0.00} in");
			AddString(PdfViewerStringId.UnitKiloBytes, "KB");
			AddString(PdfViewerStringId.UnitMegaBytes, "MB");
			AddString(PdfViewerStringId.UnitGigaBytes, "GB");
			AddString(PdfViewerStringId.UnitTeraBytes, "TB");
			AddString(PdfViewerStringId.UnitPetaBytes, "PB");
			AddString(PdfViewerStringId.UnitExaBytes, "EB");
			AddString(PdfViewerStringId.UnitZettaBytes, "ZB");
			#region Print
			AddString(PdfViewerStringId.PrintDialogTitle, "Print");
			AddString(PdfViewerStringId.PrintDialogPrinterName, "Printer name");
			AddString(PdfViewerStringId.PrintDialogPrinterPreferences, "Preferences");
			AddString(PdfViewerStringId.PrintDialogStatus, "Status");
			AddString(PdfViewerStringId.PrintDialogLocation, "Location");
			AddString(PdfViewerStringId.PrintDialogComment, "Comment");
			AddString(PdfViewerStringId.PrintDialogDocumentsInQueue, "Document(s) in queue");
			AddString(PdfViewerStringId.PrintDialogPrintingDpi, "Printing DPI");
			AddString(PdfViewerStringId.PrintDialogNumberOfCopies, "Number of copies");
			AddString(PdfViewerStringId.PrintDialogCollate, "Collate");
			AddString(PdfViewerStringId.PrintDialogPageRange, "Page range");
			AddString(PdfViewerStringId.PrintDialogPageRangeAll, "All");
			AddString(PdfViewerStringId.PrintDialogPageRangeCurrent, "Current page");
			AddString(PdfViewerStringId.PrintDialogPageRangePages, "Pages:");
			AddString(PdfViewerStringId.PrintDialogPageRangePagesExample, "For example, 5-12");
			AddString(PdfViewerStringId.PrintDialogPageSizing, "Page sizing");
			AddString(PdfViewerStringId.PrintDialogPageSizingFit, "Fit");
			AddString(PdfViewerStringId.PrintDialogPageSizingActualSize, "Actual size");
			AddString(PdfViewerStringId.PrintDialogPageSizingCustomScale, "Custom scale:");
			AddString(PdfViewerStringId.PrintDialogOrientation, "Orientation");
			AddString(PdfViewerStringId.PrintDialogPaperSource, "Paper source");
			AddString(PdfViewerStringId.PrintDialogFilePath, "File path");
			AddString(PdfViewerStringId.PrintDialogPrintToFile, "Print to file"); 
			AddString(PdfViewerStringId.PrintFileExtension, ".prn");
			AddString(PdfViewerStringId.PrintFileFilter, "Printable files (.prn)|*.prn");
			AddString(PdfViewerStringId.PrintDialogPaginationOf, " ({0})");
			AddString(PdfViewerStringId.PrintDialogPrintButtonCaption, "Print");
			AddString(PdfViewerStringId.PrintDialogPrintOrientationAuto, "Auto");
			AddString(PdfViewerStringId.PrintDialogPrintOrientationLandscape, "Landscape");
			AddString(PdfViewerStringId.PrintDialogPrintOrientationPortrait, "Portrait");
			#endregion
			#region Properties
			AddString(PdfViewerStringId.PropertiesDescriptionCaption, "Description");
			AddString(PdfViewerStringId.PropertiesRevisionCaption, "Revision");
			AddString(PdfViewerStringId.PropertiesAdvancedCaption, "Advanced");
			AddString(PdfViewerStringId.PropertiesFile, "File");
			AddString(PdfViewerStringId.PropertiesTitle, "Title");
			AddString(PdfViewerStringId.PropertiesAuthor, "Author");
			AddString(PdfViewerStringId.PropertiesSubject, "Subject");
			AddString(PdfViewerStringId.PropertiesKeywords, "Keywords");
			AddString(PdfViewerStringId.PropertiesCreated, "Created");
			AddString(PdfViewerStringId.PropertiesModified, "Modified");
			AddString(PdfViewerStringId.PropertiesApplication, "Application");
			AddString(PdfViewerStringId.PropertiesProducer, "Producer");
			AddString(PdfViewerStringId.PropertiesVersion, "Version");
			AddString(PdfViewerStringId.PropertiesLocation, "Location");
			AddString(PdfViewerStringId.PropertiesFileSize, "File Size");
			AddString(PdfViewerStringId.PropertiesNumberOfPages, "Number of Pages");
			AddString(PdfViewerStringId.PropertiesPageSize, "Page Size");
			#endregion
			#region OutlinesViewer
			AddString(PdfViewerStringId.OutlinesViewerExpandCurrentCaption, "Expand current bookmark");
			AddString(PdfViewerStringId.OutlinesViewerExpandTopLevelCaption, "Expand top level bookmark");
			AddString(PdfViewerStringId.OutlinesViewerCollapseTopLevelCaption, "Collapse top level bookmark");
			AddString(PdfViewerStringId.OutlinesViewerGoToCaption, "Go to bookmark");
			AddString(PdfViewerStringId.OutlinesViewerPrintCaption, "Print page(s)");
			AddString(PdfViewerStringId.OutlinesViewerPrintSectionCaption, "Print section(s)");
			AddString(PdfViewerStringId.OutlinesViewerWrapLongItemsCaption, "Wrap long bookmarks");
			AddString(PdfViewerStringId.OutlinesViewerHideAfterUseCaption, "Hide after use");
			AddString(PdfViewerStringId.OutlinesViewerPanelCaption, "Bookmarks");
			#endregion
			#region AttachmentViewer
			AddString(PdfViewerStringId.AttachmentsViewerOpenCaption, "Open Attachment"); 
			AddString(PdfViewerStringId.AttachmentsViewerOpenDescription, "Open file in its native application");
			AddString(PdfViewerStringId.AttachmentsViewerSaveCaption, "Save Attachment...");
			AddString(PdfViewerStringId.AttachmentsViewerSaveDescription, "Save Attachment");
			AddString(PdfViewerStringId.AttachmentsViewerPanelCaption, "Attachments");
			AddString(PdfViewerStringId.MessageFileAttachmentOpening, "The document is trying to access an embedded resource:\r\n'{0}'\r\nDo you want to open it?");
			#endregion
			AddString(PdfViewerStringId.SaveChangesCaption, "PDF Viewer");
			AddString(PdfViewerStringId.SaveChangesMessage, "Do you want to save the changes to the document before closing it?");
			AddString(PdfViewerStringId.MessageSecurityWarningCaption, "Security Warning");
			AddString(PdfViewerStringId.MessageSecurityWarningUriOpening, "The document is trying to access an external resource by using the following URI (uniform resource identifier): '{0}'\r\nDo you want to open it?");
			AddString(PdfViewerStringId.CommandPageLayoutCaption, "Page Display");
			AddString(PdfViewerStringId.CommandPageLayoutDescription, "");
			AddString(PdfViewerStringId.CommandSinglePageView, "Single Page View");
			AddString(PdfViewerStringId.CommandOneColumnView, "Enable Scrolling");
			AddString(PdfViewerStringId.CommandTwoPageView, "Two Page View");
			AddString(PdfViewerStringId.CommandTwoColumnView, "Two Page Scrolling");
			AddString(PdfViewerStringId.CommandShowCoverPage, "Show Cover Page");
			AddString(PdfViewerStringId.MessageSaveAsError, "Cannot save the PDF with the following name: {0}.");
		}
	}
	public class PdfViewerControlResXLocalizer : DXResXLocalizer<PdfViewerStringId> {
		ResourceManager resourceManager;
		public PdfViewerControlResXLocalizer()
			: base(new PdfViewerLocalizer()) {
			resourceManager = new ResourceManager("DevExpress.Xpf.PdfViewer.LocalizationRes", typeof(PdfViewerControlResXLocalizer).Assembly);
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return resourceManager ?? (resourceManager = new ResourceManager("DevExpress.Xpf.PdfViewer.LocalizationRes", typeof(PdfViewerControlResXLocalizer).Assembly));
		}
		public override string GetLocalizedString(PdfViewerStringId id) {
			return resourceManager.GetString("PdfViewerStringId." + id) ?? string.Empty;
		}
	}
	public class PdfViewerControlStringIdConverter : StringIdConverter<PdfViewerStringId> {
		protected override XtraLocalizer<PdfViewerStringId> Localizer { get { return PdfViewerLocalizer.Active; } }
	}
	public class PdfViewerControlLocalizedStringExtension : MarkupExtension {
		readonly PdfViewerStringId stringID;
		public PdfViewerControlLocalizedStringExtension(PdfViewerStringId stringID) {
			this.stringID = stringID;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return PdfViewerLocalizer.GetString(stringID);
		}
	}
}
