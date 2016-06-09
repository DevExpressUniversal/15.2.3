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

using System.ComponentModel;
using System;
namespace DevExpress.XtraPrinting.Localization {
	#region enum PreviewStringId
	public enum PreviewStringId {
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You shouldn't use the 'EmptyString' member")]
		EmptyString,
		Button_Cancel,
		Button_Ok,
		Button_Help,
		Button_Apply,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PreviewForm_Caption' member")]
		PPF_Preview_Caption,
		PreviewForm_Caption,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Customize' member")]
		TB_CustomizeBtn_ToolTip,
		TB_TTip_Customize,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Print' member")]
		TB_PrintBtn_ToolTip,
		TB_TTip_Print,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_PrintDirect' member")]
		TB_PrintDirectBtn_ToolTip,
		TB_TTip_PrintDirect,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_PageSetup' member")]
		TB_PageSetupBtn_ToolTip,
		TB_TTip_PageSetup,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Magnifier' member")]
		TB_MagnifierBtn_ToolTip,
		TB_TTip_Magnifier,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_ZoomIn' member")]
		TB_ZoomInBtn_ToolTip,
		TB_TTip_ZoomIn,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_ZoomOut' member")]
		TB_ZoomOutBtn_ToolTip,
		TB_TTip_ZoomOut,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Zoom' member")]
		TB_ZoomBtn_ToolTip,
		TB_TTip_Zoom,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Search' member")]
		TB_SearchBtn_ToolTip,
		TB_TTip_Search,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_FirstPage' member")]
		TB_FirstPageBtn_ToolTip,
		TB_TTip_FirstPage,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_PreviousPage' member")]
		TB_PreviousPageBtn_ToolTip,
		TB_TTip_PreviousPage,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_NextPage' member")]
		TB_NextPageBtn_ToolTip,
		TB_TTip_NextPage,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_LastPage' member")]
		TB_LastPageBtn_ToolTip,
		TB_TTip_LastPage,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_MultiplePages' member")]
		TB_MultiplePagesBtn_ToolTip,
		TB_TTip_MultiplePages,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Backgr' member")]
		TB_BackGroundBtn_ToolTip,
		TB_TTip_Backgr,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Close' member")]
		TB_ClosePreviewBtn_ToolTip,
		TB_TTip_Close,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_EditPageHF' member")]
		TB_EditPageHFBtn_ToolTip,
		TB_TTip_EditPageHF,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_HandTool' member")]
		TB_HandToolBtn_ToolTip,
		TB_TTip_HandTool,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Export' member")]
		TB_ExportBtn_ToolTip,
		TB_TTip_Export,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Send' member")]
		TB_SendBtn_ToolTip,
		TB_TTip_Send,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Map' member")]
		TB_DocMap_ToolTip,
		TB_TTip_Map,
		TB_TTip_Thumbnails,
		TB_TTip_Parameters,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'TB_TTip_Watermark' member")]
		TB_Watermark_ToolTip,
		TB_TTip_Watermark,
		TB_TTip_Scale,
		TB_TTip_Open,
		TB_TTip_Save,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'MenuItem_PdfDocument' member")]
		barExport_PDF_Document,
		MenuItem_PdfDocument,
		MenuItem_PageLayout,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'MenuItem_TxtDocument' member")]
		barExport_Text_Document,
		MenuItem_TxtDocument,
		MenuItem_GraphicDocument,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'MenuItem_CsvDocument' member")]
		barExport_CSV_Document,
		MenuItem_CsvDocument,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'MenuItem_MhtDocument' member")]
		barExport_MHT_Document,
		MenuItem_MhtDocument,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'MenuItem_XlsDocument' member")]
		barExport_Excel_Document,
		MenuItem_XlsDocument,
		MenuItem_XlsxDocument,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'MenuItem_RtfDocument' member")]
		barExport_RichTextDocument,
		MenuItem_RtfDocument,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'MenuItem_HtmDocument' member")]
		barExport_HTMLDocument,
		MenuItem_HtmDocument,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SaveDlg_FilterBmp' member")]
		barExport_BMP,
		SaveDlg_FilterBmp,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SaveDlg_FilterGif' member")]
		barExport_GIF,
		SaveDlg_FilterGif,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SaveDlg_FilterJpeg' member")]
		barExport_JPEG,
		SaveDlg_FilterJpeg,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SaveDlg_FilterPng' member")]
		barExport_PNG,
		SaveDlg_FilterPng,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SaveDlg_FilterTiff' member")]
		barExport_TIFF,
		SaveDlg_FilterTiff,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SaveDlg_FilterEmf' member")]
		barExport_EMF,
		SaveDlg_FilterEmf,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SaveDlg_FilterWmf' member")]
		barExport_WMF,
		SaveDlg_FilterWmf,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SB_PageOfPages' member")]
		SB_TotalPageNo,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SB_PageOfPages' member")]
		SB_CurrentPageNo,
		SB_PageOfPages,
		SB_ZoomFactor,
		SB_PageNone,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SB_PageInfo' member")]
		SB_PageInfo_OF,
		SB_PageInfo,
		SB_PageOfPagesHint,
		SB_TTip_Stop,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_PageContentEditor_Caption,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_PageNumber_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_PageOfPages_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_DatePrinted_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_TimePrinted_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_UserName_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_Image_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_FontButton,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_FooterLabel,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_HeaderLabel,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_AlignTops_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_AlignMiddles_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization you should localize the HeaderFooterForm.resx file")]
		PCE_AlignBottoms_ToolTip,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'MPForm_Lbl_Pages' member")]
		MPE_PagesLabel,
		MPForm_Lbl_Pages,
		WaitForm_Caption,
		Msg_ErrorTitle,
		Msg_EmptyDocument,
		Msg_CreatingDocument,
		Msg_ExportingDocument,
		Msg_UnavailableNetPrinter,
		Msg_NeedPrinter,
		Msg_WrongPrinter,
		Msg_WrongPrinting,
		Msg_WrongPageSettings,
		Msg_CustomDrawWarning,
		Msg_PageMarginsWarning,
		Msg_IncorrectPageRange,
		Msg_FontInvalidNumber,
		Msg_NotSupportedFont,
		Msg_IncorrectZoomFactor,
		Msg_InvalidMeasurement,
		Msg_CannotAccessFile,
		Msg_FileReadOnly,
		Msg_OpenFileQuestion,
		Msg_OpenFileQuestionCaption,
		Msg_CantFitBarcodeToControlBounds,
		Msg_InvalidBarcodeText,
		Msg_InvalidBarcodeTextFormat,
		Msg_InvalidBarcodeData,
		Msg_InvPropName,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete()]
		Msg_SearchDialogFinishedSearching,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete()]
		Msg_SearchDialogTotalFound,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete()]
		Msg_SearchDialogReady,
		Msg_NoDifferentFilesInStream,
		Msg_BigFileToCreate,
		Msg_BigFileToCreateJPEG,
		Msg_BigBitmapToCreate,
		Msg_XlsMoreThanMaxRows,
		Msg_XlsMoreThanMaxColumns,
		Msg_XlsxMoreThanMaxRows,
		Msg_XlsxMoreThanMaxColumns,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'Msg_FileDoesNotHavePrnxExtention' member")]
		Msg_FileDosntHavePrnxExtention,
		Msg_FileDoesNotHavePrnxExtention,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'Msg_FileDoesNotContainValidXml' member")]
		Msg_FileDosntContainValidXml,
		Msg_FileDoesNotContainValidXml,
		Msg_GoToNonExistentPage,
		Msg_Caption,
		Msg_PathTooLong,
		Msg_CannotLoadDocument,
		Msg_NoParameters,
		Msg_SeparatorCannotBeEmptyString,
		Msg_InvalidatePath,
		Msg_FileAlreadyExists,
		Margin_Inch,
		Margin_Millimeter,
		Margin_TopMargin,
		Margin_BottomMargin,
		Margin_LeftMargin,
		Margin_RightMargin,
		Shapes_Rectangle,
		Shapes_Ellipse,
		Shapes_Arrow,
		Shapes_TopArrow,
		Shapes_BottomArrow,
		Shapes_LeftArrow,
		Shapes_RightArrow,
		Shapes_Polygon,
		Shapes_Triangle,
		Shapes_Square,
		Shapes_Pentagon,
		Shapes_Hexagon,
		Shapes_Octagon,
		Shapes_Star,
		Shapes_ThreePointStar,
		Shapes_FourPointStar,
		Shapes_FivePointStar,
		Shapes_SixPointStar,
		Shapes_EightPointStar,
		Shapes_Line,
		Shapes_SlantLine,
		Shapes_BackslantLine,
		Shapes_HorizontalLine,
		Shapes_VerticalLine,
		Shapes_Cross,
		Shapes_Brace,
		Shapes_Bracket,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'ScrollingInfo_Page' member")]
		Page_Scroll,
		ScrollingInfo_Page,
		WMForm_PictureDlg_Title,
		WMForm_ImageStretch,
		WMForm_ImageClip,
		WMForm_ImageZoom,
		WMForm_Watermark_Asap,
		WMForm_Watermark_Confidential,
		WMForm_Watermark_Copy,
		WMForm_Watermark_DoNotCopy,
		WMForm_Watermark_Draft,
		WMForm_Watermark_Evaluation,
		WMForm_Watermark_Original,
		WMForm_Watermark_Personal,
		WMForm_Watermark_Sample,
		WMForm_Watermark_TopSecret,
		WMForm_Watermark_Urgent,
		WMForm_Direction_Horizontal,
		WMForm_Direction_Vertical,
		WMForm_Direction_BackwardDiagonal,
		WMForm_Direction_ForwardDiagonal,
		WMForm_VertAlign_Bottom,
		WMForm_VertAlign_Middle,
		WMForm_VertAlign_Top,
		WMForm_HorzAlign_Left,
		WMForm_HorzAlign_Center,
		WMForm_HorzAlign_Right,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization use the DevExpress .NET Localization Service.")]
		WMForm_ZOrderRgrItem_InFront,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization use the DevExpress .NET Localization Service.")]
		WMForm_ZOrderRgrItem_Behind,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization use the DevExpress .NET Localization Service.")]
		WMForm_PageRangeRgrItem_All,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization use the DevExpress .NET Localization Service.")]
		WMForm_PageRangeRgrItem_Pages,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'SaveDlg_Title' member")]
		dlgSaveAs,
		SaveDlg_Title,
		SaveDlg_FilterPdf,
		SaveDlg_FilterHtm,
		SaveDlg_FilterMht,
		SaveDlg_FilterRtf,
		SaveDlg_FilterXls,
		SaveDlg_FilterXlsx,
		SaveDlg_FilterCsv,
		SaveDlg_FilterTxt,
		SaveDlg_FilterNativeFormat,
		SaveDlg_FilterXps,
		MenuItem_File,
		MenuItem_View,
		MenuItem_Background,
		MenuItem_PageSetup,
		MenuItem_Print,
		MenuItem_PrintDirect,
		MenuItem_Export,
		MenuItem_Send,
		MenuItem_Exit,
		MenuItem_ViewToolbar,
		MenuItem_ViewStatusbar,
		MenuItem_ViewContinuous,
		MenuItem_ViewFacing,
		MenuItem_BackgrColor,
		MenuItem_Watermark,
		MenuItem_ZoomPageWidth,
		MenuItem_ZoomTextWidth,
		MenuItem_ZoomWholePage,
		MenuItem_ZoomTwoPages,
		MenuItem_Copy,
		MenuItem_PrintSelection,
		PageInfo_PageNumber,
		PageInfo_PageNumberOfTotal,
		PageInfo_PageTotal,
		PageInfo_PageDate,
		PageInfo_PageTime,
		PageInfo_PageUserName,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'EMail_From' member")]
		dlgSendFrom,
		EMail_From,
		BarText_Toolbar,
		BarText_MainMenu,
		BarText_StatusBar,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete()]
		ScalePopup_GroupText,
		ScalePopup_AdjustTo,
		ScalePopup_NormalSize,
		ScalePopup_FitTo,
		ScalePopup_PagesWide,
		ExportOption_PdfPageRange,
		ExportOption_PdfConvertImagesToJpeg,
		ExportOption_PdfCompressed,
		ExportOption_PdfACompatibility,
		ExportOption_PdfShowPrintDialogOnOpen,
		ExportOption_PdfNeverEmbeddedFonts,
		ExportOption_PdfPasswordSecurityOptions,
		ExportOption_PdfSignatureOptions,
		ExportOption_PdfImageQuality,
		ExportOption_PdfImageQuality_Lowest,
		ExportOption_PdfImageQuality_Low,
		ExportOption_PdfImageQuality_Medium,
		ExportOption_PdfImageQuality_High,
		ExportOption_PdfImageQuality_Highest,
		ExportOption_PdfDocumentAuthor,
		ExportOption_PdfDocumentApplication,
		ExportOption_PdfDocumentTitle,
		ExportOption_PdfDocumentSubject,
		ExportOption_PdfDocumentKeywords,
		ExportOption_PdfPrintingPermissions_None,
		ExportOption_PdfPrintingPermissions_LowResolution,
		ExportOption_PdfPrintingPermissions_HighResolution,
		ExportOption_PdfChangingPermissions_None,
		ExportOption_PdfChangingPermissions_InsertingDeletingRotating,
		ExportOption_PdfChangingPermissions_FillingSigning,
		ExportOption_PdfChangingPermissions_CommentingFillingSigning,
		ExportOption_PdfChangingPermissions_AnyExceptExtractingPages,
		ExportOption_ConfirmOpenPasswordForm_Caption,
		ExportOption_ConfirmOpenPasswordForm_Note,
		ExportOption_ConfirmOpenPasswordForm_Name,
		ExportOption_ConfirmPermissionsPasswordForm_Caption,
		ExportOption_ConfirmPermissionsPasswordForm_Note,
		ExportOption_ConfirmPermissionsPasswordForm_Name,
		ExportOption_ConfirmationDoesNotMatchForm_Msg,
		ExportOption_HtmlExportMode,
		ExportOption_HtmlExportMode_SingleFile,
		ExportOption_HtmlExportMode_SingleFilePageByPage,
		ExportOption_HtmlExportMode_DifferentFiles,
		ExportOption_HtmlCharacterSet,
		ExportOption_HtmlTitle,
		ExportOption_HtmlRemoveSecondarySymbols,
		ExportOption_HtmlEmbedImagesInHTML,
		ExportOption_HtmlPageRange,
		ExportOption_HtmlPageBorderWidth,
		ExportOption_HtmlPageBorderColor,
		ExportOption_HtmlTableLayout,
		ExportOption_HtmlExportWatermarks,
		ExportOption_RtfExportMode,
		ExportOption_RtfExportMode_SingleFile,
		ExportOption_RtfExportMode_SingleFilePageByPage,
		ExportOption_RtfPageRange,
		ExportOption_RtfExportWatermarks,
		ExportOption_TextSeparator,
		ExportOption_TextSeparator_TabAlias,
		ExportOption_TextEncoding,
		ExportOption_TextQuoteStringsWithSeparators,
		ExportOption_TextExportMode,
		ExportOption_TextExportMode_Value,
		ExportOption_TextExportMode_Text,
		ExportOption_XlsRawDataMode,
		ExportOption_XlsShowGridLines,
		ExportOption_XlsUseNativeFormat,
		ExportOption_XlsExportHyperlinks,
		ExportOption_XlsSheetName,
		ExportOption_XlsExportMode,
		ExportOption_XlsExportMode_SingleFile,
		ExportOption_XlsExportMode_DifferentFiles,
		ExportOption_XlsPageRange,
		ExportOption_XlsxExportMode,
		ExportOption_XlsxExportMode_SingleFile,
		ExportOption_XlsxExportMode_SingleFilePageByPage,
		ExportOption_XlsxExportMode_DifferentFiles,
		ExportOption_XlsxPageRange,
		ExportOption_ImageExportMode,
		ExportOption_ImageExportMode_SingleFile,
		ExportOption_ImageExportMode_SingleFilePageByPage,
		ExportOption_ImageExportMode_DifferentFiles,
		ExportOption_ImagePageRange,
		ExportOption_ImagePageBorderWidth,
		ExportOption_ImagePageBorderColor,
		ExportOption_ImageFormat,
		ExportOption_ImageResolution,
		ExportOption_NativeFormatCompressed,
		ExportOption_XpsPageRange,
		ExportOption_XpsCompression,
		ExportOption_XpsCompression_NotCompressed,
		ExportOption_XpsCompression_Normal,
		ExportOption_XpsCompression_Maximum,
		ExportOption_XpsCompression_Fast,
		ExportOption_XpsCompression_SuperFast,
		ExportOption_XpsDocumentCreator,
		ExportOption_XpsDocumentCategory,
		ExportOption_XpsDocumentTitle,
		ExportOption_XpsDocumentSubject,
		ExportOption_XpsDocumentKeywords,
		ExportOption_XpsDocumentVersion,
		ExportOption_XpsDocumentDescription,
		FolderBrowseDlg_ExportDirectory,
		ExportOptionsForm_CaptionPdf,
		ExportOptionsForm_CaptionXls,
		ExportOptionsForm_CaptionXlsx,
		ExportOptionsForm_CaptionTxt,
		ExportOptionsForm_CaptionCsv,
		ExportOptionsForm_CaptionImage,
		ExportOptionsForm_CaptionHtml,
		ExportOptionsForm_CaptionMht,
		ExportOptionsForm_CaptionRtf,
		ExportOptionsForm_CaptionNativeOptions,
		ExportOptionsForm_CaptionXps,
		RibbonPreview_PageText,
		RibbonPreview_PageGroup_Print,
		RibbonPreview_PageGroup_PageSetup,
		RibbonPreview_PageGroup_Navigation,
		RibbonPreview_PageGroup_Zoom,
		RibbonPreview_PageGroup_Background,
		RibbonPreview_PageGroup_Export,
		RibbonPreview_PageGroup_Document,
		RibbonPreview_PageGroup_Close,
		RibbonPreview_DocumentMap_Caption,
		RibbonPreview_Parameters_Caption,
		RibbonPreview_Find_Caption,
		RibbonPreview_Pointer_Caption,
		RibbonPreview_HandTool_Caption,
		RibbonPreview_Customize_Caption,
		RibbonPreview_Print_Caption,
		RibbonPreview_PrintDirect_Caption,
		RibbonPreview_PageSetup_Caption,
		RibbonPreview_EditPageHF_Caption,
		RibbonPreview_Magnifier_Caption,
		RibbonPreview_ZoomOut_Caption,
		RibbonPreview_ZoomExact_Caption,
		RibbonPreview_ZoomIn_Caption,
		RibbonPreview_ShowFirstPage_Caption,
		RibbonPreview_ShowPrevPage_Caption,
		RibbonPreview_ShowNextPage_Caption,
		RibbonPreview_ShowLastPage_Caption,
		RibbonPreview_MultiplePages_Caption,
		RibbonPreview_FillBackground_Caption,
		RibbonPreview_Watermark_Caption,
		RibbonPreview_ExportFile_Caption,
		RibbonPreview_SendFile_Caption,
		RibbonPreview_ClosePreview_Caption,
		RibbonPreview_Scale_Caption,
		RibbonPreview_PageOrientation_Caption,
		RibbonPreview_PaperSize_Caption,
		RibbonPreview_PageMargins_Caption,
		RibbonPreview_Zoom_Caption,
		RibbonPreview_Save_Caption,
		RibbonPreview_Open_Caption,
		RibbonPreview_Thumbnails_Caption,
		RibbonPreview_DocumentMap_STipTitle,
		RibbonPreview_Parameters_STipTitle,
		RibbonPreview_Find_STipTitle,
		RibbonPreview_Pointer_STipTitle,
		RibbonPreview_HandTool_STipTitle,
		RibbonPreview_Customize_STipTitle,
		RibbonPreview_Print_STipTitle,
		RibbonPreview_PrintDirect_STipTitle,
		RibbonPreview_PageSetup_STipTitle,
		RibbonPreview_EditPageHF_STipTitle,
		RibbonPreview_Magnifier_STipTitle,
		RibbonPreview_ZoomOut_STipTitle,
		RibbonPreview_ZoomIn_STipTitle,
		RibbonPreview_ShowFirstPage_STipTitle,
		RibbonPreview_ShowPrevPage_STipTitle,
		RibbonPreview_ShowNextPage_STipTitle,
		RibbonPreview_ShowLastPage_STipTitle,
		RibbonPreview_MultiplePages_STipTitle,
		RibbonPreview_FillBackground_STipTitle,
		RibbonPreview_Watermark_STipTitle,
		RibbonPreview_ExportFile_STipTitle,
		RibbonPreview_SendFile_STipTitle,
		RibbonPreview_ClosePreview_STipTitle,
		RibbonPreview_Scale_STipTitle,
		RibbonPreview_PageOrientation_STipTitle,
		RibbonPreview_PaperSize_STipTitle,
		RibbonPreview_PageMargins_STipTitle,
		RibbonPreview_Zoom_STipTitle,
		RibbonPreview_PageGroup_PageSetup_STipTitle,
		RibbonPreview_Save_STipTitle,
		RibbonPreview_Open_STipTitle,
		RibbonPreview_Thumbnails_STipTitle,
		RibbonPreview_DocumentMap_STipContent,
		RibbonPreview_Parameters_STipContent,
		RibbonPreview_Find_STipContent,
		RibbonPreview_Pointer_STipContent,
		RibbonPreview_HandTool_STipContent,
		RibbonPreview_Customize_STipContent,
		RibbonPreview_Print_STipContent,
		RibbonPreview_PrintDirect_STipContent,
		RibbonPreview_PageSetup_STipContent,
		RibbonPreview_EditPageHF_STipContent,
		RibbonPreview_Magnifier_STipContent,
		RibbonPreview_ZoomOut_STipContent,
		RibbonPreview_ZoomIn_STipContent,
		RibbonPreview_ShowFirstPage_STipContent,
		RibbonPreview_ShowPrevPage_STipContent,
		RibbonPreview_ShowNextPage_STipContent,
		RibbonPreview_ShowLastPage_STipContent,
		RibbonPreview_MultiplePages_STipContent,
		RibbonPreview_FillBackground_STipContent,
		RibbonPreview_Watermark_STipContent,
		RibbonPreview_ExportFile_STipContent,
		RibbonPreview_SendFile_STipContent,
		RibbonPreview_ClosePreview_STipContent,
		RibbonPreview_Scale_STipContent,
		RibbonPreview_PageOrientation_STipContent,
		RibbonPreview_PaperSize_STipContent,
		RibbonPreview_PageMargins_STipContent,
		RibbonPreview_Zoom_STipContent,
		RibbonPreview_PageGroup_PageSetup_STipContent,
		RibbonPreview_Save_STipContent,
		RibbonPreview_Open_STipContent,
		RibbonPreview_Thumbnails_STipContent,
		RibbonPreview_ExportPdf_Caption,
		RibbonPreview_ExportHtm_Caption,
		RibbonPreview_ExportMht_Caption,
		RibbonPreview_ExportRtf_Caption,
		RibbonPreview_ExportXls_Caption,
		RibbonPreview_ExportXlsx_Caption,
		RibbonPreview_ExportCsv_Caption,
		RibbonPreview_ExportTxt_Caption,
		RibbonPreview_ExportGraphic_Caption,
		RibbonPreview_ExportXps_Caption,
		RibbonPreview_SendPdf_Caption,
		RibbonPreview_SendMht_Caption,
		RibbonPreview_SendRtf_Caption,
		RibbonPreview_SendXls_Caption,
		RibbonPreview_SendXlsx_Caption,
		RibbonPreview_SendCsv_Caption,
		RibbonPreview_SendTxt_Caption,
		RibbonPreview_SendGraphic_Caption,
		RibbonPreview_SendXps_Caption,
		RibbonPreview_ExportPdf_Description,
		RibbonPreview_ExportHtm_Description,
		RibbonPreview_ExportTxt_Description,
		RibbonPreview_ExportCsv_Description,
		RibbonPreview_ExportMht_Description,
		RibbonPreview_ExportXls_Description,
		RibbonPreview_ExportXlsx_Description,
		RibbonPreview_ExportRtf_Description,
		RibbonPreview_ExportGraphic_Description,
		RibbonPreview_ExportXps_Description,
		RibbonPreview_SendPdf_Description,
		RibbonPreview_SendTxt_Description,
		RibbonPreview_SendCsv_Description,
		RibbonPreview_SendMht_Description,
		RibbonPreview_SendXls_Description,
		RibbonPreview_SendXlsx_Description,
		RibbonPreview_SendRtf_Description,
		RibbonPreview_SendGraphic_Description,
		RibbonPreview_SendXps_Description,
		RibbonPreview_ExportPdf_STipTitle,
		RibbonPreview_ExportHtm_STipTitle,
		RibbonPreview_ExportTxt_STipTitle,
		RibbonPreview_ExportCsv_STipTitle,
		RibbonPreview_ExportMht_STipTitle,
		RibbonPreview_ExportXls_STipTitle,
		RibbonPreview_ExportXlsx_STipTitle,
		RibbonPreview_ExportRtf_STipTitle,
		RibbonPreview_ExportGraphic_STipTitle,
		RibbonPreview_SendPdf_STipTitle,
		RibbonPreview_SendTxt_STipTitle,
		RibbonPreview_SendCsv_STipTitle,
		RibbonPreview_SendMht_STipTitle,
		RibbonPreview_SendXls_STipTitle,
		RibbonPreview_SendXlsx_STipTitle,
		RibbonPreview_SendRtf_STipTitle,
		RibbonPreview_SendGraphic_STipTitle,
		RibbonPreview_ExportPdf_STipContent,
		RibbonPreview_ExportHtm_STipContent,
		RibbonPreview_ExportTxt_STipContent,
		RibbonPreview_ExportCsv_STipContent,
		RibbonPreview_ExportMht_STipContent,
		RibbonPreview_ExportXls_STipContent,
		RibbonPreview_ExportXlsx_STipContent,
		RibbonPreview_ExportRtf_STipContent,
		RibbonPreview_ExportGraphic_STipContent,
		RibbonPreview_SendPdf_STipContent,
		RibbonPreview_SendTxt_STipContent,
		RibbonPreview_SendCsv_STipContent,
		RibbonPreview_SendMht_STipContent,
		RibbonPreview_SendXls_STipContent,
		RibbonPreview_SendXlsx_STipContent,
		RibbonPreview_SendRtf_STipContent,
		RibbonPreview_SendGraphic_STipContent,
		RibbonPreview_GalleryItem_PageOrientationPortrait_Caption,
		RibbonPreview_GalleryItem_PageOrientationLandscape_Caption,
		RibbonPreview_GalleryItem_PageMarginsNormal_Caption,
		RibbonPreview_GalleryItem_PageMarginsNarrow_Caption,
		RibbonPreview_GalleryItem_PageMarginsModerate_Caption,
		RibbonPreview_GalleryItem_PageMarginsWide_Caption,
		RibbonPreview_GalleryItem_PageOrientationPortrait_Description,
		RibbonPreview_GalleryItem_PageOrientationLandscape_Description,
		RibbonPreview_GalleryItem_PageMarginsNormal_Description,
		RibbonPreview_GalleryItem_PageMarginsNarrow_Description,
		RibbonPreview_GalleryItem_PageMarginsModerate_Description,
		RibbonPreview_GalleryItem_PageMarginsWide_Description,
		RibbonPreview_GalleryItem_PageMargins_Description,
		RibbonPreview_GalleryItem_PaperSize_Description,
		OpenFileDialog_Filter,
		OpenFileDialog_Title,
		ExportOption_PdfPasswordSecurityOptions_DocumentOpenPassword,
		ExportOption_PdfPasswordSecurityOptions_Permissions,
		ExportOption_PdfPasswordSecurityOptions_None,
		ParametersRequest_Submit,
		ParametersRequest_Reset,
		ParametersRequest_Caption,
		NoneString,
		WatermarkTypePicture,
		WatermarkTypeText,
		ParameterLookUpSettingsNoLookUp,
		ExportOption_PdfSignatureOptions_Certificate,
		ExportOption_PdfSignatureOptions_Reason,
		ExportOption_PdfSignatureOptions_Location,
		ExportOption_PdfSignatureOptions_ContactInfo,
		ExportOption_PdfSignatureOptions_None,
		ExportOption_PdfSignature_EmptyCertificate,
		ExportOption_PdfSignature_Issuer,
		ExportOption_PdfSignature_ValidRange,
		NetworkPrinterFormat,
		PrinterStatus_Busy,
		PrinterStatus_DoorOpen,
		PrinterStatus_DriverUpdateNeeded,
		PrinterStatus_Error,
		PrinterStatus_Initializing,
		PrinterStatus_IOActive,
		PrinterStatus_ManualFeed,
		PrinterStatus_NotAvailable,
		PrinterStatus_NoToner,
		PrinterStatus_Offline,
		PrinterStatus_OutOfMemory,
		PrinterStatus_OutputBinFull,
		PrinterStatus_PagePunt,
		PrinterStatus_PaperJam,
		PrinterStatus_PaperOut,
		PrinterStatus_PaperProblem,
		PrinterStatus_Paused,
		PrinterStatus_PendingDeletion,
		PrinterStatus_PowerSave,
		PrinterStatus_Printing,
		PrinterStatus_Processing,
		PrinterStatus_Ready,
		PrinterStatus_ServerOffline,
		PrinterStatus_ServerUnknown,
		PrinterStatus_TonerLow,
		PrinterStatus_UserIntervention,
		PrinterStatus_Waiting,
		PrinterStatus_WarmingUp,
	}
	#endregion
	#region PreviewLocalizer.AddStrings 
	public partial class PreviewLocalizer {
		void AddStrings() {
			AddString(PreviewStringId.Button_Cancel, "Cancel");
			AddString(PreviewStringId.Button_Ok, "OK");
			AddString(PreviewStringId.Button_Help, "Help");
			AddString(PreviewStringId.Button_Apply, "Apply");
			AddString(PreviewStringId.PreviewForm_Caption, "Preview");
			AddString(PreviewStringId.TB_TTip_Customize, "Customize");
			AddString(PreviewStringId.TB_TTip_Print, "Print");
			AddString(PreviewStringId.TB_TTip_PrintDirect, "Quick Print");
			AddString(PreviewStringId.TB_TTip_PageSetup, "Page Setup");
			AddString(PreviewStringId.TB_TTip_Magnifier, "Magnifier");
			AddString(PreviewStringId.TB_TTip_ZoomIn, "Zoom In");
			AddString(PreviewStringId.TB_TTip_ZoomOut, "Zoom Out");
			AddString(PreviewStringId.TB_TTip_Zoom, "Zoom");
			AddString(PreviewStringId.TB_TTip_Search, "Search");
			AddString(PreviewStringId.TB_TTip_FirstPage, "First Page");
			AddString(PreviewStringId.TB_TTip_PreviousPage, "Previous Page");
			AddString(PreviewStringId.TB_TTip_NextPage, "Next Page");
			AddString(PreviewStringId.TB_TTip_LastPage, "Last Page");
			AddString(PreviewStringId.TB_TTip_MultiplePages, "Multiple Pages");
			AddString(PreviewStringId.TB_TTip_Backgr, "Background");
			AddString(PreviewStringId.TB_TTip_Close, "Close Preview");
			AddString(PreviewStringId.TB_TTip_EditPageHF, "Header And Footer");
			AddString(PreviewStringId.TB_TTip_HandTool, "Hand Tool");
			AddString(PreviewStringId.TB_TTip_Export, "Export Document...");
			AddString(PreviewStringId.TB_TTip_Send, "Send via E-Mail...");
			AddString(PreviewStringId.TB_TTip_Map, "Document Map");
			AddString(PreviewStringId.TB_TTip_Thumbnails, "Thumbnails");
			AddString(PreviewStringId.TB_TTip_Parameters, "Parameters");
			AddString(PreviewStringId.TB_TTip_Watermark, "Watermark");
			AddString(PreviewStringId.TB_TTip_Scale, "Scale");
			AddString(PreviewStringId.TB_TTip_Open, "Open a document");
			AddString(PreviewStringId.TB_TTip_Save, "Save the document");
			AddString(PreviewStringId.MenuItem_PdfDocument, "PDF File");
			AddString(PreviewStringId.MenuItem_PageLayout, "&Page Layout");
			AddString(PreviewStringId.MenuItem_TxtDocument, "Text File");
			AddString(PreviewStringId.MenuItem_GraphicDocument, "Image File");
			AddString(PreviewStringId.MenuItem_CsvDocument, "CSV File");
			AddString(PreviewStringId.MenuItem_MhtDocument, "MHT File");
			AddString(PreviewStringId.MenuItem_XlsDocument, "XLS File");
			AddString(PreviewStringId.MenuItem_XlsxDocument, "XLSX File");
			AddString(PreviewStringId.MenuItem_RtfDocument, "RTF File");
			AddString(PreviewStringId.MenuItem_HtmDocument, "HTML File");
			AddString(PreviewStringId.SaveDlg_FilterBmp, "BMP Bitmap Format");
			AddString(PreviewStringId.SaveDlg_FilterGif, "GIF Graphics Interchange Format");
			AddString(PreviewStringId.SaveDlg_FilterJpeg, "JPEG File Interchange Format");
			AddString(PreviewStringId.SaveDlg_FilterPng, "PNG Portable Network Graphics Format");
			AddString(PreviewStringId.SaveDlg_FilterTiff, "TIFF Tag Image File Format");
			AddString(PreviewStringId.SaveDlg_FilterEmf, "EMF Enhanced Windows Metafile");
			AddString(PreviewStringId.SaveDlg_FilterWmf, "WMF Windows Metafile");
			AddString(PreviewStringId.SB_PageOfPages, "Page {0} of {1}");
			AddString(PreviewStringId.SB_ZoomFactor, "Zoom Factor: ");
			AddString(PreviewStringId.SB_PageNone, "Nothing");
			AddString(PreviewStringId.SB_PageInfo, "{0} of {1}");
			AddString(PreviewStringId.SB_PageOfPagesHint, "Page number in document. Click to open the Go To Page dialog.");
			AddString(PreviewStringId.SB_TTip_Stop, "Stop");
			AddString(PreviewStringId.MPForm_Lbl_Pages, "Pages");
			AddString(PreviewStringId.WaitForm_Caption, "Please Wait");
			AddString(PreviewStringId.Msg_ErrorTitle, "Error");
			AddString(PreviewStringId.Msg_EmptyDocument, "The document does not contain any pages.");
			AddString(PreviewStringId.Msg_CreatingDocument, "Creating the document...");
			AddString(PreviewStringId.Msg_ExportingDocument, "Exporting the document...");
			AddString(PreviewStringId.Msg_UnavailableNetPrinter, "The net printer is unavailable.");
			AddString(PreviewStringId.Msg_NeedPrinter, "No printers installed.");
			AddString(PreviewStringId.Msg_WrongPrinter, "The printer name is invalid. Please check the printer settings.");
			AddString(PreviewStringId.Msg_WrongPrinting, "An error occurred during printing a document.");
			AddString(PreviewStringId.Msg_WrongPageSettings, "The current printer doesn't support the selected paper size.\r\nProceed with printing anyway?");
			AddString(PreviewStringId.Msg_CustomDrawWarning, "Warning!");
			AddString(PreviewStringId.Msg_PageMarginsWarning, "One or more margins are set outside the printable area of the page. Continue?");
			AddString(PreviewStringId.Msg_IncorrectPageRange, "This is not a valid page range");
			AddString(PreviewStringId.Msg_FontInvalidNumber, "The font size cannot be set to zero or a negative number");
			AddString(PreviewStringId.Msg_NotSupportedFont, "This font is not yet supported");
			AddString(PreviewStringId.Msg_IncorrectZoomFactor, "The number must be between {0} and {1}.");
			AddString(PreviewStringId.Msg_InvalidMeasurement, "This is not a valid measurement.");
			AddString(PreviewStringId.Msg_CannotAccessFile, "The process cannot access the file \"{0}\" because it is being used by another process.");
			AddString(PreviewStringId.Msg_FileReadOnly, "File \"{0}\" is set to read-only, try again with a different file name.");
			AddString(PreviewStringId.Msg_OpenFileQuestion, "Do you want to open this file?");
			AddString(PreviewStringId.Msg_OpenFileQuestionCaption, "Export");
			AddString(PreviewStringId.Msg_CantFitBarcodeToControlBounds, "Control's boundaries are too small for the barcode");
			AddString(PreviewStringId.Msg_InvalidBarcodeText, "There are invalid characters in the text");
			AddString(PreviewStringId.Msg_InvalidBarcodeTextFormat, "Invalid text format");
			AddString(PreviewStringId.Msg_InvalidBarcodeData, "Binary data can't be longer than 1033 bytes.");
			AddString(PreviewStringId.Msg_InvPropName, "Invalid property name");
			AddString(PreviewStringId.Msg_NoDifferentFilesInStream, "A document can't be exported to a stream in the DifferentFiles mode. Use the SingleFile or SingleFilePageByPage mode instead.");
			AddString(PreviewStringId.Msg_BigFileToCreate, "The output file is too big. Try to reduce the number of its pages, or split it into several documents.");
			AddString(PreviewStringId.Msg_BigFileToCreateJPEG, "The output file is too big to create a JPEG file. Please choose another image format or another export mode.");
			AddString(PreviewStringId.Msg_BigBitmapToCreate, "The output file is too big. Please try to reduce the image resolution,\r\nor choose another export mode.");
			AddString(PreviewStringId.Msg_XlsMoreThanMaxRows, "The created XLS file is too big for the XLS format, because it contains more than 65,536 rows.\r\nPlease try to use the XLSX format, instead.");
			AddString(PreviewStringId.Msg_XlsMoreThanMaxColumns, "The created XLS file is too big for the XLS format, because it contains more than 256 columns.\r\nPlease try to use the XLSX format, instead.");
			AddString(PreviewStringId.Msg_XlsxMoreThanMaxRows, "The created XLSX file is too big for the XLSX format, because it contains more than 1,048,576 rows.\r\nPlease try to reduce the amount of rows in your report and export the report to XLSX again.");
			AddString(PreviewStringId.Msg_XlsxMoreThanMaxColumns, "The created XLSX file is too big for the XLSX format, because it contains more than 16,384 columns.\r\nPlease try to reduce the amount of columns in your report and export the report to XLSX again.");
			AddString(PreviewStringId.Msg_FileDoesNotHavePrnxExtention, "The specified file doesn't have a PRNX extension. Proceed anyway?");
			AddString(PreviewStringId.Msg_FileDoesNotContainValidXml, "The specified file doesn't contain valid XML data in the PRNX format. Loading is stopped.");
			AddString(PreviewStringId.Msg_GoToNonExistentPage, "There is no page numbered {0} in this document.");
			AddString(PreviewStringId.Msg_Caption, "Printing");
			AddString(PreviewStringId.Msg_PathTooLong, "The path is too long.\r\nTry a shorter name.");
			AddString(PreviewStringId.Msg_CannotLoadDocument, "The specified file cannot be loaded, because it either does not contain valid XML data or exceeds the allowed size.");
			AddString(PreviewStringId.Msg_NoParameters, "The specified parameters do not exist: {0}.");
			AddString(PreviewStringId.Msg_SeparatorCannotBeEmptyString, "The separator cannot be an empty string.");
			AddString(PreviewStringId.Msg_InvalidatePath, "Cannot find the specified path.");
			AddString(PreviewStringId.Msg_FileAlreadyExists, "The output file already exists. Click OK to overwrite.");
			AddString(PreviewStringId.Margin_Inch, "Inch");
			AddString(PreviewStringId.Margin_Millimeter, "mm");
			AddString(PreviewStringId.Margin_TopMargin, "Top Margin");
			AddString(PreviewStringId.Margin_BottomMargin, "Bottom Margin");
			AddString(PreviewStringId.Margin_LeftMargin, "Left Margin");
			AddString(PreviewStringId.Margin_RightMargin, "Right Margin");
			AddString(PreviewStringId.Shapes_Rectangle, "Rectangle");
			AddString(PreviewStringId.Shapes_Ellipse, "Ellipse");
			AddString(PreviewStringId.Shapes_Arrow, "Arrow");
			AddString(PreviewStringId.Shapes_TopArrow, "Top Arrow");
			AddString(PreviewStringId.Shapes_BottomArrow, "Bottom Arrow");
			AddString(PreviewStringId.Shapes_LeftArrow, "Left Arrow");
			AddString(PreviewStringId.Shapes_RightArrow, "Right Arrow");
			AddString(PreviewStringId.Shapes_Polygon, "Polygon");
			AddString(PreviewStringId.Shapes_Triangle, "Triangle");
			AddString(PreviewStringId.Shapes_Square, "Square");
			AddString(PreviewStringId.Shapes_Pentagon, "Pentagon");
			AddString(PreviewStringId.Shapes_Hexagon, "Hexagon");
			AddString(PreviewStringId.Shapes_Octagon, "Octagon");
			AddString(PreviewStringId.Shapes_Star, "Star");
			AddString(PreviewStringId.Shapes_ThreePointStar, "3-Point Star");
			AddString(PreviewStringId.Shapes_FourPointStar, "4-Point Star");
			AddString(PreviewStringId.Shapes_FivePointStar, "5-Point Star");
			AddString(PreviewStringId.Shapes_SixPointStar, "6-Point Star");
			AddString(PreviewStringId.Shapes_EightPointStar, "8-Point Star");
			AddString(PreviewStringId.Shapes_Line, "Line");
			AddString(PreviewStringId.Shapes_SlantLine, "Slant Line");
			AddString(PreviewStringId.Shapes_BackslantLine, "Backslant Line");
			AddString(PreviewStringId.Shapes_HorizontalLine, "Horizontal Line");
			AddString(PreviewStringId.Shapes_VerticalLine, "Vertical Line");
			AddString(PreviewStringId.Shapes_Cross, "Cross");
			AddString(PreviewStringId.Shapes_Brace, "Brace");
			AddString(PreviewStringId.Shapes_Bracket, "Bracket");
			AddString(PreviewStringId.ScrollingInfo_Page, "Page");
			AddString(PreviewStringId.WMForm_PictureDlg_Title, "Select Picture");
			AddString(PreviewStringId.WMForm_ImageStretch, "Stretch");
			AddString(PreviewStringId.WMForm_ImageClip, "Clip");
			AddString(PreviewStringId.WMForm_ImageZoom, "Zoom");
			AddString(PreviewStringId.WMForm_Watermark_Asap, "ASAP");
			AddString(PreviewStringId.WMForm_Watermark_Confidential, "CONFIDENTIAL");
			AddString(PreviewStringId.WMForm_Watermark_Copy, "COPY");
			AddString(PreviewStringId.WMForm_Watermark_DoNotCopy, "DO NOT COPY");
			AddString(PreviewStringId.WMForm_Watermark_Draft, "DRAFT");
			AddString(PreviewStringId.WMForm_Watermark_Evaluation, "EVALUATION");
			AddString(PreviewStringId.WMForm_Watermark_Original, "ORIGINAL");
			AddString(PreviewStringId.WMForm_Watermark_Personal, "PERSONAL");
			AddString(PreviewStringId.WMForm_Watermark_Sample, "SAMPLE");
			AddString(PreviewStringId.WMForm_Watermark_TopSecret, "TOP SECRET");
			AddString(PreviewStringId.WMForm_Watermark_Urgent, "URGENT");
			AddString(PreviewStringId.WMForm_Direction_Horizontal, "Horizontal");
			AddString(PreviewStringId.WMForm_Direction_Vertical, "Vertical");
			AddString(PreviewStringId.WMForm_Direction_BackwardDiagonal, "Backward Diagonal");
			AddString(PreviewStringId.WMForm_Direction_ForwardDiagonal, "Forward Diagonal");
			AddString(PreviewStringId.WMForm_VertAlign_Bottom, "Bottom");
			AddString(PreviewStringId.WMForm_VertAlign_Middle, "Middle");
			AddString(PreviewStringId.WMForm_VertAlign_Top, "Top");
			AddString(PreviewStringId.WMForm_HorzAlign_Left, "Left");
			AddString(PreviewStringId.WMForm_HorzAlign_Center, "Center");
			AddString(PreviewStringId.WMForm_HorzAlign_Right, "Right");
			AddString(PreviewStringId.SaveDlg_Title, "Save As");
			AddString(PreviewStringId.SaveDlg_FilterPdf, "PDF Document");
			AddString(PreviewStringId.SaveDlg_FilterHtm, "HTML Document");
			AddString(PreviewStringId.SaveDlg_FilterMht, "MHT Document");
			AddString(PreviewStringId.SaveDlg_FilterRtf, "Rich Text Document");
			AddString(PreviewStringId.SaveDlg_FilterXls, "XLS Document");
			AddString(PreviewStringId.SaveDlg_FilterXlsx, "XLSX Document");
			AddString(PreviewStringId.SaveDlg_FilterCsv, "CSV Document");
			AddString(PreviewStringId.SaveDlg_FilterTxt, "Text Document");
			AddString(PreviewStringId.SaveDlg_FilterNativeFormat, "Native Format");
			AddString(PreviewStringId.SaveDlg_FilterXps, "XPS Document");
			AddString(PreviewStringId.MenuItem_File, "&File");
			AddString(PreviewStringId.MenuItem_View, "&View");
			AddString(PreviewStringId.MenuItem_Background, "&Background");
			AddString(PreviewStringId.MenuItem_PageSetup, "Page Set&up...");
			AddString(PreviewStringId.MenuItem_Print, "&Print...");
			AddString(PreviewStringId.MenuItem_PrintDirect, "P&rint");
			AddString(PreviewStringId.MenuItem_Export, "&Export To");
			AddString(PreviewStringId.MenuItem_Send, "Sen&d As");
			AddString(PreviewStringId.MenuItem_Exit, "E&xit");
			AddString(PreviewStringId.MenuItem_ViewToolbar, "&Toolbar");
			AddString(PreviewStringId.MenuItem_ViewStatusbar, "&Statusbar");
			AddString(PreviewStringId.MenuItem_ViewContinuous, "&Continuous");
			AddString(PreviewStringId.MenuItem_ViewFacing, "&Facing");
			AddString(PreviewStringId.MenuItem_BackgrColor, "&Color...");
			AddString(PreviewStringId.MenuItem_Watermark, "&Watermark...");
			AddString(PreviewStringId.MenuItem_ZoomPageWidth, "Page Width");
			AddString(PreviewStringId.MenuItem_ZoomTextWidth, "Text Width");
			AddString(PreviewStringId.MenuItem_ZoomWholePage, "Whole Page");
			AddString(PreviewStringId.MenuItem_ZoomTwoPages, "Two Pages");
			AddString(PreviewStringId.MenuItem_Copy, "Copy");
			AddString(PreviewStringId.MenuItem_PrintSelection, "Print...");
			AddString(PreviewStringId.PageInfo_PageNumber, "[Page #]");
			AddString(PreviewStringId.PageInfo_PageNumberOfTotal, "[Page # of Pages #]");
			AddString(PreviewStringId.PageInfo_PageTotal, "[Pages #]");
			AddString(PreviewStringId.PageInfo_PageDate, "[Date Printed]");
			AddString(PreviewStringId.PageInfo_PageTime, "[Time Printed]");
			AddString(PreviewStringId.PageInfo_PageUserName, "[User Name]");
			AddString(PreviewStringId.EMail_From, "From");
			AddString(PreviewStringId.BarText_Toolbar, "Toolbar");
			AddString(PreviewStringId.BarText_MainMenu, "Main Menu");
			AddString(PreviewStringId.BarText_StatusBar, "Status Bar");
			AddString(PreviewStringId.ScalePopup_AdjustTo, "Adjust to");
			AddString(PreviewStringId.ScalePopup_NormalSize, "% of normal size");
			AddString(PreviewStringId.ScalePopup_FitTo, "Fit to");
			AddString(PreviewStringId.ScalePopup_PagesWide, "pages wide");
			AddString(PreviewStringId.ExportOption_PdfPageRange, "Page range:");
			AddString(PreviewStringId.ExportOption_PdfConvertImagesToJpeg, "Convert Images to Jpeg");
			AddString(PreviewStringId.ExportOption_PdfCompressed, "Compressed");
			AddString(PreviewStringId.ExportOption_PdfACompatibility, "PDF/A-2b");
			AddString(PreviewStringId.ExportOption_PdfShowPrintDialogOnOpen, "Show print dialog on open");
			AddString(PreviewStringId.ExportOption_PdfNeverEmbeddedFonts, "Don't embed these fonts:");
			AddString(PreviewStringId.ExportOption_PdfPasswordSecurityOptions, "Password Security:");
			AddString(PreviewStringId.ExportOption_PdfSignatureOptions, "Digital Signature:");
			AddString(PreviewStringId.ExportOption_PdfImageQuality, "Images quality:");
			AddString(PreviewStringId.ExportOption_PdfImageQuality_Lowest, "Lowest");
			AddString(PreviewStringId.ExportOption_PdfImageQuality_Low, "Low");
			AddString(PreviewStringId.ExportOption_PdfImageQuality_Medium, "Medium");
			AddString(PreviewStringId.ExportOption_PdfImageQuality_High, "High");
			AddString(PreviewStringId.ExportOption_PdfImageQuality_Highest, "Highest");
			AddString(PreviewStringId.ExportOption_PdfDocumentAuthor, "Author:");
			AddString(PreviewStringId.ExportOption_PdfDocumentApplication, "Application:");
			AddString(PreviewStringId.ExportOption_PdfDocumentTitle, "Title:");
			AddString(PreviewStringId.ExportOption_PdfDocumentSubject, "Subject:");
			AddString(PreviewStringId.ExportOption_PdfDocumentKeywords, "Keywords:");
			AddString(PreviewStringId.ExportOption_PdfPrintingPermissions_None, "None");
			AddString(PreviewStringId.ExportOption_PdfPrintingPermissions_LowResolution, "Low Resolution (150 dpi)");
			AddString(PreviewStringId.ExportOption_PdfPrintingPermissions_HighResolution, "High Resolution");
			AddString(PreviewStringId.ExportOption_PdfChangingPermissions_None, "None");
			AddString(PreviewStringId.ExportOption_PdfChangingPermissions_InsertingDeletingRotating, "Inserting, deleting and rotating pages");
			AddString(PreviewStringId.ExportOption_PdfChangingPermissions_FillingSigning, "Filling in form fields and signing existing signature fields");
			AddString(PreviewStringId.ExportOption_PdfChangingPermissions_CommentingFillingSigning, "Commenting, filling in form fields, and signing existing signature fields");
			AddString(PreviewStringId.ExportOption_PdfChangingPermissions_AnyExceptExtractingPages, "Any except extracting pages");
			AddString(PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Caption, "Confirm Document Open Password");
			AddString(PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Note, "Please confirm the Document Open Password. Be sure to make a note of the password. It will be required to open the document.");
			AddString(PreviewStringId.ExportOption_ConfirmOpenPasswordForm_Name, "Document Open Pa&ssword:");
			AddString(PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Caption, "Confirm Permissions Password");
			AddString(PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Note, "Please confirm the Permissions Password. Be sure to make a note of the password. You will need it to change these settings in the future.");
			AddString(PreviewStringId.ExportOption_ConfirmPermissionsPasswordForm_Name, "&Permissions Password:");
			AddString(PreviewStringId.ExportOption_ConfirmationDoesNotMatchForm_Msg, "Confirmation password does not match. Please start over and enter the password again.");
			AddString(PreviewStringId.ExportOption_HtmlExportMode, "Export mode:");
			AddString(PreviewStringId.ExportOption_HtmlExportMode_SingleFile, "Single file");
			AddString(PreviewStringId.ExportOption_HtmlExportMode_SingleFilePageByPage, "Single file page-by-page");
			AddString(PreviewStringId.ExportOption_HtmlExportMode_DifferentFiles, "Different files");
			AddString(PreviewStringId.ExportOption_HtmlCharacterSet, "Character set:");
			AddString(PreviewStringId.ExportOption_HtmlTitle, "Title:");
			AddString(PreviewStringId.ExportOption_HtmlRemoveSecondarySymbols, "Remove carriage returns");
			AddString(PreviewStringId.ExportOption_HtmlEmbedImagesInHTML, "Embed images in HTML");
			AddString(PreviewStringId.ExportOption_HtmlPageRange, "Page range:");
			AddString(PreviewStringId.ExportOption_HtmlPageBorderWidth, "Page border width:");
			AddString(PreviewStringId.ExportOption_HtmlPageBorderColor, "Page border color:");
			AddString(PreviewStringId.ExportOption_HtmlTableLayout, "Table layout");
			AddString(PreviewStringId.ExportOption_HtmlExportWatermarks, "Export watermarks");
			AddString(PreviewStringId.ExportOption_RtfExportMode, "Export mode:");
			AddString(PreviewStringId.ExportOption_RtfExportMode_SingleFile, "Single file");
			AddString(PreviewStringId.ExportOption_RtfExportMode_SingleFilePageByPage, "Single file page-by-page");
			AddString(PreviewStringId.ExportOption_RtfPageRange, "Page range:");
			AddString(PreviewStringId.ExportOption_RtfExportWatermarks, "Export watermarks");
			AddString(PreviewStringId.ExportOption_TextSeparator, "Text separator:");
			AddString(PreviewStringId.ExportOption_TextSeparator_TabAlias, "TAB");
			AddString(PreviewStringId.ExportOption_TextEncoding, "Encoding:");
			AddString(PreviewStringId.ExportOption_TextQuoteStringsWithSeparators, "Quote strings with separators");
			AddString(PreviewStringId.ExportOption_TextExportMode, "Text export mode:");
			AddString(PreviewStringId.ExportOption_TextExportMode_Value, "Value");
			AddString(PreviewStringId.ExportOption_TextExportMode_Text, "Text");
			AddString(PreviewStringId.ExportOption_XlsRawDataMode, "Raw data mode");
			AddString(PreviewStringId.ExportOption_XlsShowGridLines, "Show grid lines");
			AddString(PreviewStringId.ExportOption_XlsUseNativeFormat, "Export values using their format");
			AddString(PreviewStringId.ExportOption_XlsExportHyperlinks, "Export hyperlinks");
			AddString(PreviewStringId.ExportOption_XlsSheetName, "Sheet name:");
			AddString(PreviewStringId.ExportOption_XlsExportMode, "Export mode:");
			AddString(PreviewStringId.ExportOption_XlsExportMode_SingleFile, "Single file");
			AddString(PreviewStringId.ExportOption_XlsExportMode_DifferentFiles, "Different files");
			AddString(PreviewStringId.ExportOption_XlsPageRange, "Page range:");
			AddString(PreviewStringId.ExportOption_XlsxExportMode, "Export mode:");
			AddString(PreviewStringId.ExportOption_XlsxExportMode_SingleFile, "Single file");
			AddString(PreviewStringId.ExportOption_XlsxExportMode_SingleFilePageByPage, "Single file page-by-page");
			AddString(PreviewStringId.ExportOption_XlsxExportMode_DifferentFiles, "Different files");
			AddString(PreviewStringId.ExportOption_XlsxPageRange, "Page range:");
			AddString(PreviewStringId.ExportOption_ImageExportMode, "Export mode:");
			AddString(PreviewStringId.ExportOption_ImageExportMode_SingleFile, "Single file");
			AddString(PreviewStringId.ExportOption_ImageExportMode_SingleFilePageByPage, "Single file page-by-page");
			AddString(PreviewStringId.ExportOption_ImageExportMode_DifferentFiles, "Different files");
			AddString(PreviewStringId.ExportOption_ImagePageRange, "Page range:");
			AddString(PreviewStringId.ExportOption_ImagePageBorderWidth, "Page border width:");
			AddString(PreviewStringId.ExportOption_ImagePageBorderColor, "Page border color:");
			AddString(PreviewStringId.ExportOption_ImageFormat, "Image format:");
			AddString(PreviewStringId.ExportOption_ImageResolution, "Resolution (dpi):");
			AddString(PreviewStringId.ExportOption_NativeFormatCompressed, "Compressed");
			AddString(PreviewStringId.ExportOption_XpsPageRange, "Page range:");
			AddString(PreviewStringId.ExportOption_XpsCompression, "Compression:");
			AddString(PreviewStringId.ExportOption_XpsCompression_NotCompressed, "Not compressed");
			AddString(PreviewStringId.ExportOption_XpsCompression_Normal, "Normal");
			AddString(PreviewStringId.ExportOption_XpsCompression_Maximum, "Maximum");
			AddString(PreviewStringId.ExportOption_XpsCompression_Fast, "Fast");
			AddString(PreviewStringId.ExportOption_XpsCompression_SuperFast, "Super fast");
			AddString(PreviewStringId.ExportOption_XpsDocumentCreator, "Creator:");
			AddString(PreviewStringId.ExportOption_XpsDocumentCategory, "Category:");
			AddString(PreviewStringId.ExportOption_XpsDocumentTitle, "Title:");
			AddString(PreviewStringId.ExportOption_XpsDocumentSubject, "Subject:");
			AddString(PreviewStringId.ExportOption_XpsDocumentKeywords, "Keywords:");
			AddString(PreviewStringId.ExportOption_XpsDocumentVersion, "Version:");
			AddString(PreviewStringId.ExportOption_XpsDocumentDescription, "Description:");
			AddString(PreviewStringId.FolderBrowseDlg_ExportDirectory, "Select a folder to save the exported document to:");
			AddString(PreviewStringId.ExportOptionsForm_CaptionPdf, "PDF Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionXls, "XLS Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionXlsx, "XLSX Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionTxt, "Text Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionCsv, "CSV Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionImage, "Image Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionHtml, "HTML Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionMht, "MHT Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionRtf, "RTF Export Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionNativeOptions, "Native Format Options");
			AddString(PreviewStringId.ExportOptionsForm_CaptionXps, "XPS Export Options");
			AddString(PreviewStringId.RibbonPreview_PageText, "Print Preview");
			AddString(PreviewStringId.RibbonPreview_PageGroup_Print, "Print");
			AddString(PreviewStringId.RibbonPreview_PageGroup_PageSetup, "Page Setup");
			AddString(PreviewStringId.RibbonPreview_PageGroup_Navigation, "Navigation");
			AddString(PreviewStringId.RibbonPreview_PageGroup_Zoom, "Zoom");
			AddString(PreviewStringId.RibbonPreview_PageGroup_Background, "Page Background");
			AddString(PreviewStringId.RibbonPreview_PageGroup_Export, "Export");
			AddString(PreviewStringId.RibbonPreview_PageGroup_Document, "Document");
			AddString(PreviewStringId.RibbonPreview_PageGroup_Close, "Close");
			AddString(PreviewStringId.RibbonPreview_DocumentMap_Caption, "Bookmarks");
			AddString(PreviewStringId.RibbonPreview_Parameters_Caption, "Parameters");
			AddString(PreviewStringId.RibbonPreview_Find_Caption, "Find");
			AddString(PreviewStringId.RibbonPreview_Pointer_Caption, "Pointer");
			AddString(PreviewStringId.RibbonPreview_HandTool_Caption, "Hand Tool");
			AddString(PreviewStringId.RibbonPreview_Customize_Caption, "Options");
			AddString(PreviewStringId.RibbonPreview_Print_Caption, "Print");
			AddString(PreviewStringId.RibbonPreview_PrintDirect_Caption, "Quick Print");
			AddString(PreviewStringId.RibbonPreview_PageSetup_Caption, "Custom Margins...");
			AddString(PreviewStringId.RibbonPreview_EditPageHF_Caption, "Header/Footer");
			AddString(PreviewStringId.RibbonPreview_Magnifier_Caption, "Magnifier");
			AddString(PreviewStringId.RibbonPreview_ZoomOut_Caption, "Zoom Out");
			AddString(PreviewStringId.RibbonPreview_ZoomExact_Caption, "Exact:");
			AddString(PreviewStringId.RibbonPreview_ZoomIn_Caption, "Zoom In");
			AddString(PreviewStringId.RibbonPreview_ShowFirstPage_Caption, "First Page");
			AddString(PreviewStringId.RibbonPreview_ShowPrevPage_Caption, "Previous Page");
			AddString(PreviewStringId.RibbonPreview_ShowNextPage_Caption, "Next  Page ");
			AddString(PreviewStringId.RibbonPreview_ShowLastPage_Caption, "Last  Page ");
			AddString(PreviewStringId.RibbonPreview_MultiplePages_Caption, "Many Pages");
			AddString(PreviewStringId.RibbonPreview_FillBackground_Caption, "Page Color");
			AddString(PreviewStringId.RibbonPreview_Watermark_Caption, "Watermark");
			AddString(PreviewStringId.RibbonPreview_ExportFile_Caption, "Export To");
			AddString(PreviewStringId.RibbonPreview_SendFile_Caption, "E-Mail As");
			AddString(PreviewStringId.RibbonPreview_ClosePreview_Caption, "Close");
			AddString(PreviewStringId.RibbonPreview_Scale_Caption, "Scale");
			AddString(PreviewStringId.RibbonPreview_PageOrientation_Caption, "Orientation");
			AddString(PreviewStringId.RibbonPreview_PaperSize_Caption, "Size");
			AddString(PreviewStringId.RibbonPreview_PageMargins_Caption, "Margins");
			AddString(PreviewStringId.RibbonPreview_Zoom_Caption, "Zoom");
			AddString(PreviewStringId.RibbonPreview_Save_Caption, "Save");
			AddString(PreviewStringId.RibbonPreview_Open_Caption, "Open");
			AddString(PreviewStringId.RibbonPreview_Thumbnails_Caption, "Thumbnails");
			AddString(PreviewStringId.RibbonPreview_DocumentMap_STipTitle, "Document Map");
			AddString(PreviewStringId.RibbonPreview_Parameters_STipTitle, "Parameters");
			AddString(PreviewStringId.RibbonPreview_Find_STipTitle, "Find");
			AddString(PreviewStringId.RibbonPreview_Pointer_STipTitle, "Mouse Pointer");
			AddString(PreviewStringId.RibbonPreview_HandTool_STipTitle, "Hand Tool");
			AddString(PreviewStringId.RibbonPreview_Customize_STipTitle, "Options");
			AddString(PreviewStringId.RibbonPreview_Print_STipTitle, "Print (Ctrl+P)");
			AddString(PreviewStringId.RibbonPreview_PrintDirect_STipTitle, "Quick Print");
			AddString(PreviewStringId.RibbonPreview_PageSetup_STipTitle, "Page Setup");
			AddString(PreviewStringId.RibbonPreview_EditPageHF_STipTitle, "Header and Footer");
			AddString(PreviewStringId.RibbonPreview_Magnifier_STipTitle, "Magnifier");
			AddString(PreviewStringId.RibbonPreview_ZoomOut_STipTitle, "Zoom Out");
			AddString(PreviewStringId.RibbonPreview_ZoomIn_STipTitle, "Zoom In");
			AddString(PreviewStringId.RibbonPreview_ShowFirstPage_STipTitle, "First Page (Ctrl+Home)");
			AddString(PreviewStringId.RibbonPreview_ShowPrevPage_STipTitle, "Previous Page (PageUp)");
			AddString(PreviewStringId.RibbonPreview_ShowNextPage_STipTitle, "Next Page (PageDown)");
			AddString(PreviewStringId.RibbonPreview_ShowLastPage_STipTitle, "Last Page (Ctrl+End)");
			AddString(PreviewStringId.RibbonPreview_MultiplePages_STipTitle, "View Many Pages");
			AddString(PreviewStringId.RibbonPreview_FillBackground_STipTitle, "Background Color");
			AddString(PreviewStringId.RibbonPreview_Watermark_STipTitle, "Watermark");
			AddString(PreviewStringId.RibbonPreview_ExportFile_STipTitle, "Export To...");
			AddString(PreviewStringId.RibbonPreview_SendFile_STipTitle, "E-Mail As...");
			AddString(PreviewStringId.RibbonPreview_ClosePreview_STipTitle, "Close Print Preview");
			AddString(PreviewStringId.RibbonPreview_Scale_STipTitle, "Scale");
			AddString(PreviewStringId.RibbonPreview_PageOrientation_STipTitle, "Page Orientation");
			AddString(PreviewStringId.RibbonPreview_PaperSize_STipTitle, "Page Size");
			AddString(PreviewStringId.RibbonPreview_PageMargins_STipTitle, "Page Margins");
			AddString(PreviewStringId.RibbonPreview_Zoom_STipTitle, "Zoom");
			AddString(PreviewStringId.RibbonPreview_PageGroup_PageSetup_STipTitle, "Page Setup");
			AddString(PreviewStringId.RibbonPreview_Save_STipTitle, "Save (Ctrl + S)");
			AddString(PreviewStringId.RibbonPreview_Open_STipTitle, "Open (Ctrl + O)");
			AddString(PreviewStringId.RibbonPreview_Thumbnails_STipTitle, "Thumbnails");
			AddString(PreviewStringId.RibbonPreview_DocumentMap_STipContent, "Open the Document Map, which allows you to navigate through a structural view of the document.");
			AddString(PreviewStringId.RibbonPreview_Parameters_STipContent, "Open the Parameters pane, which allows you to enter values for report parameters.");
			AddString(PreviewStringId.RibbonPreview_Find_STipContent, "Show the Find dialog to find text in the document.");
			AddString(PreviewStringId.RibbonPreview_Pointer_STipContent, "Show the mouse pointer.");
			AddString(PreviewStringId.RibbonPreview_HandTool_STipContent, "Invoke the Hand tool to manually scroll through pages.");
			AddString(PreviewStringId.RibbonPreview_Customize_STipContent, "Open the Print Options dialog, in which you can change printing options.");
			AddString(PreviewStringId.RibbonPreview_Print_STipContent, "Select a printer, number of copies and other printing options before printing.");
			AddString(PreviewStringId.RibbonPreview_PrintDirect_STipContent, "Send the document directly to the default printer without making changes.");
			AddString(PreviewStringId.RibbonPreview_PageSetup_STipContent, "Show the Page Setup dialog.");
			AddString(PreviewStringId.RibbonPreview_EditPageHF_STipContent, "Edit the header and footer of the document.");
			AddString(PreviewStringId.RibbonPreview_Magnifier_STipContent, "Invoke the Magnifier tool.\r\n\r\nClicking once on a document zooms it so that a single page becomes entirely visible, while clicking another time zooms it to 100% of the normal size.");
			AddString(PreviewStringId.RibbonPreview_ZoomOut_STipContent, "Zoom out to see more of the page at a reduced size.");
			AddString(PreviewStringId.RibbonPreview_ZoomIn_STipContent, "Zoom in to get a close-up view of the document.");
			AddString(PreviewStringId.RibbonPreview_ShowFirstPage_STipContent, "Navigate to the first page of the document.");
			AddString(PreviewStringId.RibbonPreview_ShowPrevPage_STipContent, "Navigate to the previous page of the document.");
			AddString(PreviewStringId.RibbonPreview_ShowNextPage_STipContent, "Navigate to the next page of the document.");
			AddString(PreviewStringId.RibbonPreview_ShowLastPage_STipContent, "Navigate to the last page of the document.");
			AddString(PreviewStringId.RibbonPreview_MultiplePages_STipContent, "Choose the page layout to arrange the document pages in preview.");
			AddString(PreviewStringId.RibbonPreview_FillBackground_STipContent, "Choose a color for the background of the document pages.");
			AddString(PreviewStringId.RibbonPreview_Watermark_STipContent, "Insert ghosted text or image behind the content of a page.\r\n\r\nThis is often used to indicate that a document is to be treated specially.");
			AddString(PreviewStringId.RibbonPreview_ExportFile_STipContent, "Export the current document in one of the available formats, and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_SendFile_STipContent, "Export the current document in one of the available formats, and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_ClosePreview_STipContent, "Close Print Preview of the document.");
			AddString(PreviewStringId.RibbonPreview_Scale_STipContent, "Stretch or shrink the printed output to a percentage of its actual size.");
			AddString(PreviewStringId.RibbonPreview_PageOrientation_STipContent, "Switch the pages between portrait and landscape layouts.");
			AddString(PreviewStringId.RibbonPreview_PaperSize_STipContent, "Choose the paper size of the document.");
			AddString(PreviewStringId.RibbonPreview_PageMargins_STipContent, "Select the margin sizes for the entire document.\r\n\r\nTo apply specific margin sizes to the document, click Custom Margins.");
			AddString(PreviewStringId.RibbonPreview_Zoom_STipContent, "Change the zoom level of the document preview.");
			AddString(PreviewStringId.RibbonPreview_PageGroup_PageSetup_STipContent, "Show the Page Setup dialog.");
			AddString(PreviewStringId.RibbonPreview_Save_STipContent, "Save the document.");
			AddString(PreviewStringId.RibbonPreview_Open_STipContent, "Open a document.");
			AddString(PreviewStringId.RibbonPreview_Thumbnails_STipContent, "Open the Thumbnails, which allows you to navigate through the document.");
			AddString(PreviewStringId.RibbonPreview_ExportPdf_Caption, "PDF File");
			AddString(PreviewStringId.RibbonPreview_ExportHtm_Caption, "HTML File");
			AddString(PreviewStringId.RibbonPreview_ExportMht_Caption, "MHT File");
			AddString(PreviewStringId.RibbonPreview_ExportRtf_Caption, "RTF File");
			AddString(PreviewStringId.RibbonPreview_ExportXls_Caption, "XLS File");
			AddString(PreviewStringId.RibbonPreview_ExportXlsx_Caption, "XLSX File");
			AddString(PreviewStringId.RibbonPreview_ExportCsv_Caption, "CSV File");
			AddString(PreviewStringId.RibbonPreview_ExportTxt_Caption, "Text File");
			AddString(PreviewStringId.RibbonPreview_ExportGraphic_Caption, "Image File");
			AddString(PreviewStringId.RibbonPreview_ExportXps_Caption, "XPS File");
			AddString(PreviewStringId.RibbonPreview_SendPdf_Caption, "PDF File");
			AddString(PreviewStringId.RibbonPreview_SendMht_Caption, "MHT File");
			AddString(PreviewStringId.RibbonPreview_SendRtf_Caption, "RTF File");
			AddString(PreviewStringId.RibbonPreview_SendXls_Caption, "XLS File");
			AddString(PreviewStringId.RibbonPreview_SendXlsx_Caption, "XLSX File");
			AddString(PreviewStringId.RibbonPreview_SendCsv_Caption, "CSV File");
			AddString(PreviewStringId.RibbonPreview_SendTxt_Caption, "Text File");
			AddString(PreviewStringId.RibbonPreview_SendGraphic_Caption, "Image File");
			AddString(PreviewStringId.RibbonPreview_SendXps_Caption, "XPS File");
			AddString(PreviewStringId.RibbonPreview_ExportPdf_Description, "Adobe Portable Document Format");
			AddString(PreviewStringId.RibbonPreview_ExportHtm_Description, "Web Page");
			AddString(PreviewStringId.RibbonPreview_ExportTxt_Description, "Plain Text");
			AddString(PreviewStringId.RibbonPreview_ExportCsv_Description, "Comma-Separated Values Text");
			AddString(PreviewStringId.RibbonPreview_ExportMht_Description, "Single File Web Page");
			AddString(PreviewStringId.RibbonPreview_ExportXls_Description, "Microsoft Excel 2000-2003 Workbook");
			AddString(PreviewStringId.RibbonPreview_ExportXlsx_Description, "Microsoft Excel 2007 Workbook");
			AddString(PreviewStringId.RibbonPreview_ExportRtf_Description, "Rich Text Format");
			AddString(PreviewStringId.RibbonPreview_ExportGraphic_Description, "BMP, GIF, JPEG, PNG, TIFF, EMF, WMF");
			AddString(PreviewStringId.RibbonPreview_ExportXps_Description, "XPS");
			AddString(PreviewStringId.RibbonPreview_SendPdf_Description, "Adobe Portable Document Format");
			AddString(PreviewStringId.RibbonPreview_SendTxt_Description, "Plain Text");
			AddString(PreviewStringId.RibbonPreview_SendCsv_Description, "Comma-Separated Values Text");
			AddString(PreviewStringId.RibbonPreview_SendMht_Description, "Single File Web Page");
			AddString(PreviewStringId.RibbonPreview_SendXls_Description, "Microsoft Excel 2000-2003 Workbook");
			AddString(PreviewStringId.RibbonPreview_SendXlsx_Description, "Microsoft Excel 2007 Workbook");
			AddString(PreviewStringId.RibbonPreview_SendRtf_Description, "Rich Text Format");
			AddString(PreviewStringId.RibbonPreview_SendGraphic_Description, "BMP, GIF, JPEG, PNG, TIFF, EMF, WMF");
			AddString(PreviewStringId.RibbonPreview_SendXps_Description, "XPS");
			AddString(PreviewStringId.RibbonPreview_ExportPdf_STipTitle, "Export to PDF");
			AddString(PreviewStringId.RibbonPreview_ExportHtm_STipTitle, "Export to HTML");
			AddString(PreviewStringId.RibbonPreview_ExportTxt_STipTitle, "Export to Text");
			AddString(PreviewStringId.RibbonPreview_ExportCsv_STipTitle, "Export to CSV");
			AddString(PreviewStringId.RibbonPreview_ExportMht_STipTitle, "Export to MHT");
			AddString(PreviewStringId.RibbonPreview_ExportXls_STipTitle, "Export to XLS");
			AddString(PreviewStringId.RibbonPreview_ExportXlsx_STipTitle, "Export to XLSX");
			AddString(PreviewStringId.RibbonPreview_ExportRtf_STipTitle, "Export to RTF");
			AddString(PreviewStringId.RibbonPreview_ExportGraphic_STipTitle, "Export to Image");
			AddString(PreviewStringId.RibbonPreview_SendPdf_STipTitle, "E-Mail As PDF");
			AddString(PreviewStringId.RibbonPreview_SendTxt_STipTitle, "E-Mail As Text");
			AddString(PreviewStringId.RibbonPreview_SendCsv_STipTitle, "E-Mail As CSV");
			AddString(PreviewStringId.RibbonPreview_SendMht_STipTitle, "E-Mail As MHT");
			AddString(PreviewStringId.RibbonPreview_SendXls_STipTitle, "E-Mail As XLS");
			AddString(PreviewStringId.RibbonPreview_SendXlsx_STipTitle, "E-Mail As XLSX");
			AddString(PreviewStringId.RibbonPreview_SendRtf_STipTitle, "E-Mail As RTF");
			AddString(PreviewStringId.RibbonPreview_SendGraphic_STipTitle, "E-Mail As Image");
			AddString(PreviewStringId.RibbonPreview_ExportPdf_STipContent, "Export the document to PDF and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_ExportHtm_STipContent, "Export the document to HTML and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_ExportTxt_STipContent, "Export the document to Text and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_ExportCsv_STipContent, "Export the document to CSV and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_ExportMht_STipContent, "Export the document to MHT and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_ExportXls_STipContent, "Export the document to XLS and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_ExportXlsx_STipContent, "Export the document to XLSX and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_ExportRtf_STipContent, "Export the document to RTF and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_ExportGraphic_STipContent, "Export the document to Image and save it to the file on a disk.");
			AddString(PreviewStringId.RibbonPreview_SendPdf_STipContent, "Export the document to PDF and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_SendTxt_STipContent, "Export the document to Text and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_SendCsv_STipContent, "Export the document to CSV and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_SendMht_STipContent, "Export the document to MHT and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_SendXls_STipContent, "Export the document to XLS and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_SendXlsx_STipContent, "Export the document to XLSX and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_SendRtf_STipContent, "Export the document to RTF and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_SendGraphic_STipContent, "Export the document to Image and attach it to the e-mail.");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageOrientationPortrait_Caption, "Portrait");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageOrientationLandscape_Caption, "Landscape");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMarginsNormal_Caption, "Normal");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMarginsNarrow_Caption, "Narrow");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMarginsModerate_Caption, "Moderate");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMarginsWide_Caption, "Wide");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageOrientationPortrait_Description, "");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageOrientationLandscape_Description, "");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMarginsNormal_Description, "Normal");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMarginsNarrow_Description, "Narrow");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMarginsModerate_Description, "Moderate");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMarginsWide_Description, "Wide");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PageMargins_Description, "Top:\t\t{0}\t\tBottom:\t\t{1}\r\nLeft:\t\t {2}\t\tRight:\t\t   {3}");
			AddString(PreviewStringId.RibbonPreview_GalleryItem_PaperSize_Description, "{0} x {1}");
			AddString(PreviewStringId.OpenFileDialog_Filter, "Preview Document Files (*{0})|*{0}|All Files (*.*)|*.*");
			AddString(PreviewStringId.OpenFileDialog_Title, "Open");
			AddString(PreviewStringId.ExportOption_PdfPasswordSecurityOptions_DocumentOpenPassword, "Document Open Password");
			AddString(PreviewStringId.ExportOption_PdfPasswordSecurityOptions_Permissions, "Permissions");
			AddString(PreviewStringId.ExportOption_PdfPasswordSecurityOptions_None, "(none)");
			AddString(PreviewStringId.ParametersRequest_Submit, "Submit");
			AddString(PreviewStringId.ParametersRequest_Reset, "Reset");
			AddString(PreviewStringId.ParametersRequest_Caption, "Parameters");
			AddString(PreviewStringId.NoneString, "(none)");
			AddString(PreviewStringId.WatermarkTypePicture, "(Picture)");
			AddString(PreviewStringId.WatermarkTypeText, "(Text)");
			AddString(PreviewStringId.ParameterLookUpSettingsNoLookUp, "No Look-Up");
			AddString(PreviewStringId.ExportOption_PdfSignatureOptions_Certificate, "Certificate");
			AddString(PreviewStringId.ExportOption_PdfSignatureOptions_Reason, "Reason");
			AddString(PreviewStringId.ExportOption_PdfSignatureOptions_Location, "Location");
			AddString(PreviewStringId.ExportOption_PdfSignatureOptions_ContactInfo, "Contact Info");
			AddString(PreviewStringId.ExportOption_PdfSignatureOptions_None, "(none)");
			AddString(PreviewStringId.ExportOption_PdfSignature_EmptyCertificate, "None");
			AddString(PreviewStringId.ExportOption_PdfSignature_Issuer, "Issuer: ");
			AddString(PreviewStringId.ExportOption_PdfSignature_ValidRange, "Valid From: {0:d} to {1:d}");
			AddString(PreviewStringId.NetworkPrinterFormat, "{0} on {1}");
			AddString(PreviewStringId.PrinterStatus_Busy, "The printer is busy.");
			AddString(PreviewStringId.PrinterStatus_DoorOpen, "The printer door is open.");
			AddString(PreviewStringId.PrinterStatus_DriverUpdateNeeded, "The printer driver needs to be updated.");
			AddString(PreviewStringId.PrinterStatus_Error, "Error");
			AddString(PreviewStringId.PrinterStatus_Initializing, "Initializing the Preview...");
			AddString(PreviewStringId.PrinterStatus_IOActive, "The printer's input/output is active.");
			AddString(PreviewStringId.PrinterStatus_ManualFeed, "The manual feed is enabled.");
			AddString(PreviewStringId.PrinterStatus_NotAvailable, "The printer is not available.");
			AddString(PreviewStringId.PrinterStatus_NoToner, "The printer has no toner.");
			AddString(PreviewStringId.PrinterStatus_Offline, "The printer is offline.");
			AddString(PreviewStringId.PrinterStatus_OutOfMemory, "The printer is out of memory.");
			AddString(PreviewStringId.PrinterStatus_OutputBinFull, "The output bin is full.");
			AddString(PreviewStringId.PrinterStatus_PagePunt, "A page punt has occurred.");
			AddString(PreviewStringId.PrinterStatus_PaperJam, "The paper has jammed.");
			AddString(PreviewStringId.PrinterStatus_PaperOut, "The printer is out of paper.");
			AddString(PreviewStringId.PrinterStatus_PaperProblem, "A paper-related problem has occurred.");
			AddString(PreviewStringId.PrinterStatus_Paused, "The printer is paused.");
			AddString(PreviewStringId.PrinterStatus_PendingDeletion, "Print task deletion is pending.");
			AddString(PreviewStringId.PrinterStatus_PowerSave, "The power save mode is on.");
			AddString(PreviewStringId.PrinterStatus_Printing, "Printing...");
			AddString(PreviewStringId.PrinterStatus_Processing, "Processing...");
			AddString(PreviewStringId.PrinterStatus_Ready, "The printer is ready.");
			AddString(PreviewStringId.PrinterStatus_ServerOffline, "The server is offline.");
			AddString(PreviewStringId.PrinterStatus_ServerUnknown, "The server is unknown.");
			AddString(PreviewStringId.PrinterStatus_TonerLow, "The toner is low.");
			AddString(PreviewStringId.PrinterStatus_UserIntervention, "A user intervention has occurred.");
			AddString(PreviewStringId.PrinterStatus_Waiting, "Waiting...");
			AddString(PreviewStringId.PrinterStatus_WarmingUp, "The printer is warming up.");
		}
	}
	 #endregion
}
