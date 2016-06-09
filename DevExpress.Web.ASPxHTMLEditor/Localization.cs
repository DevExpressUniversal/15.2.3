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
using DevExpress.Utils.Localization;
using DevExpress.Web.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.Internal;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxHtmlEditor.Localization {
	public enum ASPxHtmlEditorStringId {
		Alt_ConstrainProportions,
		Alt_ConstrainProportionsOff,
		AutomaticColorCaption,
		TableElementInconsistentColor,
		DesignViewTab,
		HtmlViewTab,
		PreviewTab,
		CommandCut,
		CommandCopy,
		CommandPaste,
		CommandPasteRtf,
		CommandPasteHtmlSourceFormatting,
		CommandPasteHtmlMergeFormatting,
		CommandPasteHtmlPlainText,
		CommandUndo,
		CommandRedo,
		CommandPrint,
		CommandRemoveFormat,
		CommandSubscript,
		CommandSuperscript,
		CommandOrderedList,
		CommandBulletList,
		CommandIndent,
		CommandOutdent,
		CommandUnlink,
		CommandCheckSpelling,
		CommandBold,
		CommandItalic,
		CommandUnderline,
		CommandStrikethrough,
		CommandLeft,
		CommandCenter,
		CommandRight,
		CommandJustify,
		CommandBackColor,
		CommandForeColor,
		CommandApplyCss,
		CommandFullscreen,
		CommandComment,
		CommandUncomment,
		CommandFormatDocument,
		CommandCollapseTag,
		CommandExpandTag,
		InsertImage,
		ChangeImage,
		ContextMenu_ChangeImage,
		SelectImage,
		InsertLink,
		ChangeLink,
		ContextMenu_ChangeLink,
		ContextMenu_ChangeElementProperties,
		SelectDocument,
		InsertAudio,
		ChangeAudio,
		ContextMenu_ChangeAudio,
		SelectAudio,
		InsertVideo,
		ChangeVideo,
		ContextMenu_ChangeVideo,
		SelectVideo,
		InsertFlash,
		ChangeFlash,
		ChangeElementProperties,
		DeleteElement,
		ContextMenu_ChangeFlash,
		SelectFlash,
		InsertYouTubeVideo,
		ChangeYouTubeVideo,
		ContextMenu_ChangeYouTubeVideo,
		InsertPlaceholder,
		ChangePlaceholder,
		ContextMenu_ChangePlaceholder,
		FontSize,
		FontName,
		Paragraph,
		PasteRtf_Instructions,
		PasteRtf_StripFont,
		ButtonOk,
		ButtonCancel,
		ButtonChange,
		ButtonInsert,
		ButtonSelect,
		ButtonUpload,
		InsertLink_Url,
		InsertLink_Email,
		InsertLink_DisplayProperties,
		InsertLink_Text,
		InsertLink_ToolTip,
		InsertLink_OpenInNewWindow,
		InsertLink_EmailTo,
		InsertLink_EmailErrorText,
		InsertLink_Subject,
		InsertLink_SelectDocument,
		InsertPlaceholder_Placeholders,
		DefaultErrorText,
		RequiredHtmlContentError,
		RequiredFieldError,
		InsertImage_FromWeb,
		InsertImage_FromLocal,
		InsertImage_EnterUrl,
		InsertImage_SaveToServer,
		InsertImage_Preview,
		InsertImage_UploadInstructions,
		InsertImage_MoreOptions,
		InsertImage_UseFloat,
		InsertImage_SelectImage,
		InsertImage_PreviewText,
		InsertImage_GalleryTabText,
		InsertImage_CommonSettingsTabName,
		InsertImage_StyleSettingsTabName,
		ImageProps_Size,
		ImageProps_OriginalSize,
		ImageProps_CustomSize,
		ImageProps_Width,
		ImageProps_Pixels,
		ImageProps_Height,
		ImageProps_CreateThumbnail,
		ImageProps_CreateThumbnailTooltip,
		ImageProps_NewImageName,
		ImageProps_Position,
		ImageProps_PositionLeft,
		ImageProps_PositionCenter,
		ImageProps_PositionRight,
		ImageProps_Description,
		ImageProps_Margins,
		ImageProps_MarginTop,
		ImageProps_MarginBottom,
		ImageProps_MarginLeft,
		ImageProps_MarginRight,
		ImageProps_Border,
		ImageProps_BorderWidth,
		ImageProps_BorderColor,
		ImageProps_BorderStyle,
		ImageProps_CssClass,
		InvalidUrlErrorText,
		InsertTable,
		ContextMenu_InsertTable,
		TableProperties,
		ContextMenu_TableProperties,
		TableCellProperties,
		ContextMenu_TableCellProperties,
		TableColumnProperties,
		ContextMenu_TableColumnProperties,
		TableRowProperties,
		ContextMenu_TableRowProperties,
		ChangeTableColumn_Size,
		InsertTableRowAbove,
		InsertTableRowBelow,
		InsertTableColumnToLeft,
		InsertTableColumnToRight,
		DeleteTable,
		DeleteTableRow,
		DeleteTableColumn,
		MergeTableCellDown,
		MergeTableCellHorizontal,
		SplitTableCellVertical,
		SplitTableCellHorizontal,
		InsertTable_Size,
		InsertTable_Columns,
		InsertTable_Rows,
		InsertTable_Width,
		InsertTable_Height,
		InsertTable_FullWidth,
		InsertTable_AutoFitToContent,
		InsertTable_Custom,
		InsertTable_EqualColumnWidths,
		InsertTable_Layout,
		InsertTable_CellPaddings,
		InsertTable_Alignment,
		InsertTable_VertAlignment,
		InsertTable_HorzAlignment,
		InsertTable_None,
		InsertTable_Alignment_Left,
		InsertTable_Alignment_Center,
		InsertTable_Alignment_Right,
		InsertTable_VAlignment_Top,
		InsertTable_VAlignment_Bottom,
		InsertTable_VAlignment_Middle,
		InsertTable_CellSpacing,
		InsertTable_Appearance,
		InsertTable_BorderColor,
		InsertTable_BorderSize,
		InsertTable_BgColor,
		InsertTable_Accessibility,
		InsertTable_Headers,
		InsertTable_FirstRow,
		InsertTable_FirstColumn,
		InsertTable_Both,
		InsertTable_Caption,
		InsertTable_Summary,
		InsertTable_ApplyToAllCell,
		SelectAll,
		FindAndReplace,
		SaveAsRtf,
		SaveAsDocx,
		SaveAsMht,
		SaveAsOdt,
		SaveAsPdf,
		SaveAsTxt,
		SaveAsRtf_ToolTip,
		SaveAsDocx_ToolTip,
		SaveAsMht_ToolTip,
		SaveAsOdt_ToolTip,
		SaveAsPdf_ToolTip,
		SaveAsTxt_ToolTip,
		RibbonTab_Home,
		RibbonTab_Insert,
		RibbonTab_View,
		RibbonTab_Table,
		RibbonTab_Review,
		RibbonTabCategory_Layout,
		RibbonGroup_Undo,
		RibbonGroup_Clipboard,
		RibbonGroup_Font,
		RibbonGroup_Paragraph,
		RibbonGroup_Images,
		RibbonGroup_Links,
		RibbonGroup_Views,
		RibbonGroup_Tables,
		RibbonGroup_DeleteTable,
		RibbonGroup_InsertTable,
		RibbonGroup_MergeTable,
		RibbonGroup_TableProperties,
		RibbonGroup_Spelling,
		RibbonGroup_Media,
		RibbonGroup_Editing,
		RibbonItem_InsertImage,
		RibbonItem_InsertLink,
		RibbonItem_Table,
		RibbonItem_InsertTable,
		RibbonItem_TableProperties,
		RibbonItem_CellProperties,
		RibbonItem_ColumnProperties,
		RibbonItem_RowProperties,
		RibbonItem_InsertTableRowAbove,
		RibbonItem_InsertTableRowBelow,
		RibbonItem_InsertTableColumnToLeft,
		RibbonItem_InsertTableColumnToRight,
		RibbonItem_InsertAudio,
		RibbonItem_InsertVideo,
		RibbonItem_InsertFlash,
		RibbonItem_InsertYouTubeVideo,
		RibbonItem_InsertPlaceholder,
		InsertVideo_CommonSettingsTabName,
		InsertVideo_StyleSettingsTabName,
		InsertVideo_FromWeb,
		InsertVideo_FromLocal,
		InsertVideo_EnterUrl,
		InsertVideo_SaveToServer,
		InsertVideo_Preview,
		InsertVideo_UploadInstructions,
		InsertVideo_MoreOptions,
		InsertVideo_Preload,
		InsertVideo_PreloadNone,
		InsertVideo_PreloadMetadata,
		InsertVideo_PreloadAuto,
		InsertVideo_Poster,
		InsertVideo_AutoPlay,
		InsertVideo_Loop,
		InsertVideo_ShowControls,
		InsertVideo_Width,
		InsertVideo_Pixels,
		InsertVideo_Height,
		InsertVideo_Position,
		InsertVideo_PositionLeft,
		InsertVideo_PositionCenter,
		InsertVideo_PositionRight,
		InsertVideo_Margins,
		InsertVideo_MarginTop,
		InsertVideo_MarginBottom,
		InsertVideo_MarginLeft,
		InsertVideo_MarginRight,
		InsertVideo_Border,
		InsertVideo_BorderWidth,
		InsertVideo_BorderColor,
		InsertVideo_BorderStyle,
		InsertVideo_CssClass,
		InsertVideo_PosterHelpText,
		InsertVideo_PreloadHelpText,
		InsertVideo_PreviewText,
		InsertVideo_GalleryTabText,
		ChangeElementProperties_CommonSettingsTabName,
		ChangeElementProperties_StyleSettingsTabName,
		ChangeElementProperties_Id,
		ChangeElementProperties_Value,
		ChangeElementProperties_Title,
		ChangeElementProperties_TabIndex,
		ChangeElementProperties_Accept,
		ChangeElementProperties_Alt,
		ChangeElementProperties_Src,
		ChangeElementProperties_Direction,
		ChangeElementProperties_InputType,
		ChangeElementProperties_Action,
		ChangeElementProperties_Method,
		ChangeElementProperties_Name,
		ChangeElementProperties_For,
		ChangeElementProperties_Disabled,
		ChangeElementProperties_Checked,
		ChangeElementProperties_MaxLength,
		ChangeElementProperties_Size,
		ChangeElementProperties_Readonly,
		InsertAudio_CommonSettingsTabName,
		InsertAudio_StyleSettingsTabName,
		InsertAudio_FromWeb,
		InsertAudio_FromLocal,
		InsertAudio_EnterUrl,
		InsertAudio_SaveToServer,
		InsertAudio_Preview,
		InsertAudio_UploadInstructions,
		InsertAudio_MoreOptions,
		InsertAudio_Preload,
		InsertAudio_PreloadNone,
		InsertAudio_PreloadMetadata,
		InsertAudio_PreloadAuto,
		InsertAudio_AutoPlay,
		InsertAudio_Loop,
		InsertAudio_ShowControls,
		InsertAudio_Width,
		InsertAudio_Pixels,
		InsertAudio_Height,
		InsertAudio_Position,
		InsertAudio_PositionLeft,
		InsertAudio_PositionCenter,
		InsertAudio_PositionRight,
		InsertAudio_Margins,
		InsertAudio_MarginTop,
		InsertAudio_MarginBottom,
		InsertAudio_MarginLeft,
		InsertAudio_MarginRight,
		InsertAudio_Border,
		InsertAudio_BorderWidth,
		InsertAudio_BorderColor,
		InsertAudio_BorderStyle,
		InsertAudio_CssClass,
		InsertAudio_PreloadHelpText,
		InsertAudio_PreviewText,
		InsertAudio_GalleryTabText,
		InsertFlash_CommonSettingsTabName,
		InsertFlash_StyleSettingsTabName,
		InsertFlash_FromWeb,
		InsertFlash_FromLocal,
		InsertFlash_EnterUrl,
		InsertFlash_SaveToServer,
		InsertFlash_Preview,
		InsertFlash_UploadInstructions,
		InsertFlash_MoreOptions,
		InsertFlash_PreviewText,
		InsertFlash_GalleryTabText,
		InsertFlash_EnableFlashMenu,
		InsertFlash_AutoPlay,
		InsertFlash_Loop,
		InsertFlash_AllowFullscreen,
		InsertFlash_Quality,
		InsertFlash_QualityBest,
		InsertFlash_QualityHigh,
		InsertFlash_QualityAutoHigh,
		InsertFlash_QualityMedium,
		InsertFlash_QualityLow,
		InsertFlash_QualityAutoLow,
		InsertFlash_Width,
		InsertFlash_Pixels,
		InsertFlash_Height,
		InsertFlash_Position,
		InsertFlash_PositionLeft,
		InsertFlash_PositionCenter,
		InsertFlash_PositionRight,
		InsertFlash_Margins,
		InsertFlash_MarginTop,
		InsertFlash_MarginBottom,
		InsertFlash_MarginLeft,
		InsertFlash_MarginRight,
		InsertFlash_Border,
		InsertFlash_BorderWidth,
		InsertFlash_BorderColor,
		InsertFlash_BorderStyle,
		InsertFlash_CssClass,
		InsertYouTubeVideo_CommonSettingsTabName,
		InsertYouTubeVideo_StyleSettingsTabName,
		InsertYouTubeVideo_EnterUrl,
		InsertYouTubeVideo_Preview,
		InsertYouTubeVideo_MoreOptions,
		InsertYouTubeVideo_ShowControls,
		InsertYouTubeVideo_ShowSameVideos,
		InsertYouTubeVideo_SecureMode,
		InsertYouTubeVideo_ShowVideoName,
		InsertImage_AllowedFileExtensionsText,
		InsertImage_MaximumUploadFileSizeText,
		InsertFlash_AllowedFileExtensionsText,
		InsertFlash_MaximumUploadFileSizeText,
		InsertVideo_AllowedFileExtensionsText,
		InsertVideo_MaximumUploadFileSizeText,
		InsertAudio_AllowedFileExtensionsText,
		InsertAudio_MaximumUploadFileSizeText,
		InsertYouTubeVideo_Width,
		InsertYouTubeVideo_Pixels,
		InsertYouTubeVideo_Height,
		InsertYouTubeVideo_Position,
		InsertYouTubeVideo_PositionLeft,
		InsertYouTubeVideo_PositionCenter,
		InsertYouTubeVideo_PositionRight,
		InsertYouTubeVideo_Margins,
		InsertYouTubeVideo_MarginTop,
		InsertYouTubeVideo_MarginBottom,
		InsertYouTubeVideo_MarginLeft,
		InsertYouTubeVideo_MarginRight,
		InsertYouTubeVideo_Border,
		InsertYouTubeVideo_BorderWidth,
		InsertYouTubeVideo_BorderColor,
		InsertYouTubeVideo_BorderStyle,
		InsertYouTubeVideo_CssClass,
		InsertYouTubeVideo_SecureModeHelpText,
		InsertYouTubeVideo_ValidationErrorText,
		AdvancedSearch_Next,
		AdvancedSearch_Previous,
		AdvancedSearch_Replace,
		AdvancedSearch_ReplaceAll,
		AdvancedSearch_Results,
		AdvancedSearch_ReplaceWith,
		AdvancedSearch_Find,
		AdvancedSearch_MatchCase,
		AdvancedSearch_Header,
		AdvancedSearch_Of,
		AdvancedSearch_ReplaceAllNotify,
		AdvancedSearch_ReplaceProcessNotify,
		AdvancedSearch_SearchLimit,
		QuickSearch_SearchFieldNullText
	}
	public class ASPxHtmlEditorResLocalizer : ASPxResLocalizerBase<ASPxHtmlEditorStringId> {
		public ASPxHtmlEditorResLocalizer() 
			: base(new ASPxHtmlEditorLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName { 
			get { return AssemblyInfo.SRAssemblyHtmlEditorWeb; } 
		}
		protected override string ResxName {
			get { return "DevExpress.Web.ASPxHtmlEditor.LocalizationRes"; }
		}
	}
	public class ASPxHtmlEditorLocalizer : XtraLocalizer<ASPxHtmlEditorStringId> {
		static ASPxHtmlEditorLocalizer() {
			SetActiveLocalizerProvider(new ASPxActiveLocalizerProvider<ASPxHtmlEditorStringId>(CreateResLocalizerInstance));
		}
		static XtraLocalizer<ASPxHtmlEditorStringId> CreateResLocalizerInstance() {
			return new ASPxHtmlEditorResLocalizer();
		}
		public static string GetString(ASPxHtmlEditorStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxHtmlEditorStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxHtmlEditorStringId.TableElementInconsistentColor, StringResources.HtmlEditorText_TableElementInconsistentColor);
			AddString(ASPxHtmlEditorStringId.AutomaticColorCaption, StringResources.HtmlEditorText_AutomaticColorCaption);
			AddString(ASPxHtmlEditorStringId.Alt_ConstrainProportions, StringResources.HtmlEditor_Alt_ConstrainProportions);
			AddString(ASPxHtmlEditorStringId.Alt_ConstrainProportionsOff, StringResources.HtmlEditor_Alt_ConstrainProportionsOff);
			AddString(ASPxHtmlEditorStringId.DesignViewTab, StringResources.HtmlEditorText_Design);
			AddString(ASPxHtmlEditorStringId.HtmlViewTab, StringResources.HtmlEditorText_HTML);
			AddString(ASPxHtmlEditorStringId.PreviewTab, StringResources.HtmlEditorText_Preview);
			AddString(ASPxHtmlEditorStringId.CommandCut, StringResources.HtmlEditorText_Cut);
			AddString(ASPxHtmlEditorStringId.CommandCopy, StringResources.HtmlEditorText_Copy);
			AddString(ASPxHtmlEditorStringId.CommandPaste, StringResources.HtmlEditorText_Paste);
			AddString(ASPxHtmlEditorStringId.CommandPasteRtf, StringResources.HtmlEditorText_PasteRtf);
			AddString(ASPxHtmlEditorStringId.CommandPasteHtmlSourceFormatting, StringResources.HtmlEditorText_PasteHtmlSourceFormatting);
			AddString(ASPxHtmlEditorStringId.CommandPasteHtmlMergeFormatting, StringResources.HtmlEditorText_PasteHtmlMergeFormatting);
			AddString(ASPxHtmlEditorStringId.CommandPasteHtmlPlainText, StringResources.HtmlEditorText_PasteHtmlPlainText);
			AddString(ASPxHtmlEditorStringId.CommandUndo, StringResources.HtmlEditorText_Undo);
			AddString(ASPxHtmlEditorStringId.CommandRedo, StringResources.HtmlEditorText_Redo);
			AddString(ASPxHtmlEditorStringId.CommandPrint, StringResources.HtmlEditorText_Print);
			AddString(ASPxHtmlEditorStringId.CommandRemoveFormat, StringResources.HtmlEditorText_RemoveFormat);
			AddString(ASPxHtmlEditorStringId.CommandSubscript, StringResources.HtmlEditorText_Subscript);
			AddString(ASPxHtmlEditorStringId.CommandSuperscript, StringResources.HtmlEditorText_Superscript);
			AddString(ASPxHtmlEditorStringId.CommandOrderedList, StringResources.HtmlEditorText_OrderedList);
			AddString(ASPxHtmlEditorStringId.CommandBulletList, StringResources.HtmlEditorText_BulletList);
			AddString(ASPxHtmlEditorStringId.CommandIndent, StringResources.HtmlEditorText_Indent);
			AddString(ASPxHtmlEditorStringId.CommandOutdent, StringResources.HtmlEditorText_Outdent);
			AddString(ASPxHtmlEditorStringId.CommandFormatDocument, StringResources.HtmlEditorText_FormatDocument);
			AddString(ASPxHtmlEditorStringId.CommandUnlink, StringResources.HtmlEditorText_Unlink);			
			AddString(ASPxHtmlEditorStringId.CommandCheckSpelling, StringResources.HtmlEditorText_CheckSpelling);
			AddString(ASPxHtmlEditorStringId.CommandBold, StringResources.HtmlEditorText_Bold);
			AddString(ASPxHtmlEditorStringId.CommandItalic, StringResources.HtmlEditorText_Italic);
			AddString(ASPxHtmlEditorStringId.CommandUnderline, StringResources.HtmlEditorText_Underline);
			AddString(ASPxHtmlEditorStringId.CommandStrikethrough, StringResources.HtmlEditorText_Strikethrough);
			AddString(ASPxHtmlEditorStringId.CommandLeft, StringResources.HtmlEditorText_AlignLeft);
			AddString(ASPxHtmlEditorStringId.CommandCenter, StringResources.HtmlEditorText_AlignCenter);
			AddString(ASPxHtmlEditorStringId.CommandRight, StringResources.HtmlEditorText_AlignRight);
			AddString(ASPxHtmlEditorStringId.CommandJustify, StringResources.HtmlEditorText_Justify);
			AddString(ASPxHtmlEditorStringId.CommandBackColor, StringResources.HtmlEditorText_BackColor);
			AddString(ASPxHtmlEditorStringId.CommandForeColor, StringResources.HtmlEditorText_ForeColor);
			AddString(ASPxHtmlEditorStringId.CommandApplyCss, StringResources.HtmlEditorText_ApplyCss);
			AddString(ASPxHtmlEditorStringId.CommandFullscreen, StringResources.HtmlEditorText_Fullscreen);
			AddString(ASPxHtmlEditorStringId.InsertImage, StringResources.HtmlEditorText_InsertImage);
			AddString(ASPxHtmlEditorStringId.ChangeImage, StringResources.HtmlEditorText_ChangeImage);
			AddString(ASPxHtmlEditorStringId.ContextMenu_ChangeImage, StringResources.HtmlEditorText_ContextMenu_ChangeImage);
			AddString(ASPxHtmlEditorStringId.SelectImage, StringResources.HtmlEditorText_SelectImage);
			AddString(ASPxHtmlEditorStringId.CommandComment, StringResources.HtmlEditorText_Comment);
			AddString(ASPxHtmlEditorStringId.CommandUncomment, StringResources.HtmlEditorText_Uncomment);
			AddString(ASPxHtmlEditorStringId.CommandCollapseTag, StringResources.HtmlEditorText_CollapseTag);
			AddString(ASPxHtmlEditorStringId.CommandExpandTag, StringResources.HtmlEditorText_ExpandTag);
			AddString(ASPxHtmlEditorStringId.InsertAudio, StringResources.HtmlEditorText_InsertAudio);
			AddString(ASPxHtmlEditorStringId.ChangeAudio, StringResources.HtmlEditorText_ChangeAudio);
			AddString(ASPxHtmlEditorStringId.ContextMenu_ChangeAudio, StringResources.HtmlEditorText_ContextMenu_ChangeAudio);
			AddString(ASPxHtmlEditorStringId.SelectAudio, StringResources.HtmlEditorText_SelectAudio);
			AddString(ASPxHtmlEditorStringId.InsertVideo, StringResources.HtmlEditorText_InsertVideo);
			AddString(ASPxHtmlEditorStringId.ChangeVideo, StringResources.HtmlEditorText_ChangeVideo);
			AddString(ASPxHtmlEditorStringId.ContextMenu_ChangeVideo, StringResources.HtmlEditorText_ContextMenu_ChangeVideo);
			AddString(ASPxHtmlEditorStringId.SelectVideo, StringResources.HtmlEditorText_SelectVideo);
			AddString(ASPxHtmlEditorStringId.DeleteElement, StringResources.HtmlEditorText_DeleteElement);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties, StringResources.HtmlEditorText_ChangeElementProperties);
			AddString(ASPxHtmlEditorStringId.InsertFlash, StringResources.HtmlEditorText_InsertFlash);
			AddString(ASPxHtmlEditorStringId.ChangeFlash, StringResources.HtmlEditorText_ChangeFlash);
			AddString(ASPxHtmlEditorStringId.ContextMenu_ChangeFlash, StringResources.HtmlEditorText_ContextMenu_ChangeFlash);
			AddString(ASPxHtmlEditorStringId.SelectFlash, StringResources.HtmlEditorText_SelectFlash);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo, StringResources.HtmlEditorText_InsertYouTubeVideo);
			AddString(ASPxHtmlEditorStringId.ChangeYouTubeVideo, StringResources.HtmlEditorText_ChangeYouTubeVideo);
			AddString(ASPxHtmlEditorStringId.ContextMenu_ChangeYouTubeVideo, StringResources.HtmlEditorText_ContextMenu_ChangeYouTubeVideo);
			AddString(ASPxHtmlEditorStringId.InsertPlaceholder, StringResources.HtmlEditorText_InsertPlaceholder);
			AddString(ASPxHtmlEditorStringId.ChangePlaceholder, StringResources.HtmlEditorText_ChangePlaceholder);
			AddString(ASPxHtmlEditorStringId.ContextMenu_ChangePlaceholder, StringResources.HtmlEditorText_ContextMenu_ChangePlaceholder);
			AddString(ASPxHtmlEditorStringId.SelectDocument, StringResources.HtmlEditorText_SelectDocument);
			AddString(ASPxHtmlEditorStringId.InsertLink, StringResources.HtmlEditorText_InsertLink);
			AddString(ASPxHtmlEditorStringId.ContextMenu_ChangeElementProperties, StringResources.HtmlEditorText_ContextMenu_ChangeElementProperties);
			AddString(ASPxHtmlEditorStringId.ContextMenu_ChangeLink, StringResources.HtmlEditorText_ContextMenu_ChangeLink);
			AddString(ASPxHtmlEditorStringId.ChangeLink, StringResources.HtmlEditorText_ChangeLink);
			AddString(ASPxHtmlEditorStringId.FontName, StringResources.HtmlEditorText_FontName);
			AddString(ASPxHtmlEditorStringId.FontSize, StringResources.HtmlEditorText_FontSize);
			AddString(ASPxHtmlEditorStringId.Paragraph, StringResources.HtmlEditorText_Paragraph);
			AddString(ASPxHtmlEditorStringId.PasteRtf_Instructions, StringResources.HtmlEditorText_PasteRtf_Instructions);
			AddString(ASPxHtmlEditorStringId.PasteRtf_StripFont, StringResources.HtmlEditorText_PasteRtf_StripFont);
			AddString(ASPxHtmlEditorStringId.ButtonOk, StringResources.HtmlEditorText_Ok);
			AddString(ASPxHtmlEditorStringId.ButtonCancel, StringResources.HtmlEditorText_Cancel);
			AddString(ASPxHtmlEditorStringId.ButtonChange, StringResources.HtmlEditorText_Change);
			AddString(ASPxHtmlEditorStringId.ButtonInsert, StringResources.HtmlEditorText_Insert);
			AddString(ASPxHtmlEditorStringId.ButtonSelect, StringResources.HtmlEditorText_Select);
			AddString(ASPxHtmlEditorStringId.ButtonUpload, StringResources.HtmlEditorText_Upload);
			AddString(ASPxHtmlEditorStringId.InsertLink_Url, StringResources.HtmlEditorText_InsertLink_Url);
			AddString(ASPxHtmlEditorStringId.InsertLink_Email, StringResources.HtmlEditorText_InsertLink_Email);
			AddString(ASPxHtmlEditorStringId.InsertLink_DisplayProperties, StringResources.HtmlEditorText_InsertLink_DisplayProperties);
			AddString(ASPxHtmlEditorStringId.InsertLink_Text, StringResources.HtmlEditorText_InsertLink_Text);
			AddString(ASPxHtmlEditorStringId.InsertLink_ToolTip, StringResources.HtmlEditorText_InsertLink_ToolTip);
			AddString(ASPxHtmlEditorStringId.InsertLink_OpenInNewWindow, StringResources.HtmlEditorText_InsertLink_OpenInNewWindow);
			AddString(ASPxHtmlEditorStringId.InsertLink_EmailTo, StringResources.HtmlEditorText_InsertLink_EmailTo);
			AddString(ASPxHtmlEditorStringId.InsertLink_EmailErrorText, StringResources.HtmlEditorText_InsertLink_EmailErrorText);
			AddString(ASPxHtmlEditorStringId.InsertLink_Subject, StringResources.HtmlEditorText_InsertLink_Subject);
			AddString(ASPxHtmlEditorStringId.InsertLink_SelectDocument, StringResources.HtmlEditorText_InsertLink_SelectDocument);
			AddString(ASPxHtmlEditorStringId.InsertPlaceholder_Placeholders, StringResources.HtmlEditorText_InsertPlaceholder_Placeholders);
			AddString(ASPxHtmlEditorStringId.DefaultErrorText, StringResources.HtmlEditorText_DefaultErrorText);
			AddString(ASPxHtmlEditorStringId.RequiredHtmlContentError, StringResources.HtmlEditorText_RequiredHtmlContentError);
			AddString(ASPxHtmlEditorStringId.RequiredFieldError, StringResources.HtmlEditorText_RequiredFieldError);
			AddString(ASPxHtmlEditorStringId.InsertImage_FromWeb, StringResources.HtmlEditorText_InsertImage_FromWeb);
			AddString(ASPxHtmlEditorStringId.InsertImage_FromLocal, StringResources.HtmlEditorText_InsertImage_FromLocal); 
			AddString(ASPxHtmlEditorStringId.InsertImage_EnterUrl, StringResources.HtmlEditorText_InsertImage_EnterUrl); 
			AddString(ASPxHtmlEditorStringId.InsertImage_SaveToServer, StringResources.HtmlEditorText_InsertImage_SaveToServer);
			AddString(ASPxHtmlEditorStringId.InsertImage_Preview, StringResources.HtmlEditorText_InsertImage_Preview); 
			AddString(ASPxHtmlEditorStringId.InsertImage_UploadInstructions, StringResources.HtmlEditorText_InsertImage_UploadInstructions);
			AddString(ASPxHtmlEditorStringId.InsertImage_AllowedFileExtensionsText, StringResources.HtmlEditorText_InsertImage_AllowedFileExtensionsText);
			AddString(ASPxHtmlEditorStringId.InsertImage_MaximumUploadFileSizeText, StringResources.HtmlEditorText_InsertImage_MaximumUploadFileSizeText);
			AddString(ASPxHtmlEditorStringId.InsertFlash_AllowedFileExtensionsText, StringResources.HtmlEditorText_InsertFlash_AllowedFileExtensionsText);
			AddString(ASPxHtmlEditorStringId.InsertFlash_MaximumUploadFileSizeText, StringResources.HtmlEditorText_InsertFlash_MaximumUploadFileSizeText);
			AddString(ASPxHtmlEditorStringId.InsertVideo_AllowedFileExtensionsText, StringResources.HtmlEditorText_InsertVideo_AllowedFileExtensionsText);
			AddString(ASPxHtmlEditorStringId.InsertVideo_MaximumUploadFileSizeText, StringResources.HtmlEditorText_InsertVideo_MaximumUploadFileSizeText); 
			AddString(ASPxHtmlEditorStringId.InsertAudio_AllowedFileExtensionsText, StringResources.HtmlEditorText_InsertAudio_AllowedFileExtensionsText);
			AddString(ASPxHtmlEditorStringId.InsertAudio_MaximumUploadFileSizeText, StringResources.HtmlEditorText_InsertAudio_MaximumUploadFileSizeText); 
			AddString(ASPxHtmlEditorStringId.InsertImage_PreviewText, StringResources.HtmlEditorText_InsertImage_PreviewText);
			AddString(ASPxHtmlEditorStringId.InsertImage_GalleryTabText, StringResources.HtmlEditorText_InsertImage_GalleryTabText);
			AddString(ASPxHtmlEditorStringId.InsertImage_MoreOptions, StringResources.HtmlEditorText_InsertImage_MoreOptions);
			AddString(ASPxHtmlEditorStringId.InsertImage_UseFloat, StringResources.HtmlEditorText_InsertImage_UseFloat);
			AddString(ASPxHtmlEditorStringId.InsertImage_SelectImage, StringResources.HtmlEditorText_InsertImage_SelectImage);
			AddString(ASPxHtmlEditorStringId.ImageProps_Size, StringResources.HtmlEditorText_ImageProps_Size);
			AddString(ASPxHtmlEditorStringId.ImageProps_OriginalSize, StringResources.HtmlEditorText_ImageProps_OriginalSize); 
			AddString(ASPxHtmlEditorStringId.ImageProps_CustomSize, StringResources.HtmlEditorText_ImageProps_CustomSize);
			AddString(ASPxHtmlEditorStringId.ImageProps_Width, StringResources.HtmlEditorText_ImageProps_Width); 
			AddString(ASPxHtmlEditorStringId.ImageProps_Pixels, StringResources.HtmlEditorText_ImageProps_Pixels); 
			AddString(ASPxHtmlEditorStringId.ImageProps_Height, StringResources.HtmlEditorText_ImageProps_Height); 
			AddString(ASPxHtmlEditorStringId.ImageProps_CreateThumbnail, StringResources.HtmlEditorText_ImageProps_CreateThumbnail);
			AddString(ASPxHtmlEditorStringId.ImageProps_CreateThumbnailTooltip, StringResources.HtmlEditorText_ImageProps_CreateThumbnailTooltip); 
			AddString(ASPxHtmlEditorStringId.ImageProps_NewImageName, StringResources.HtmlEditorText_ImageProps_NewImageName);
			AddString(ASPxHtmlEditorStringId.ImageProps_Position, StringResources.HtmlEditorText_ImageProps_Position); 
			AddString(ASPxHtmlEditorStringId.ImageProps_PositionLeft, StringResources.HtmlEditorText_ImageProps_PositionLeft); 
			AddString(ASPxHtmlEditorStringId.ImageProps_PositionCenter, StringResources.HtmlEditorText_ImageProps_PositionCenter); 
			AddString(ASPxHtmlEditorStringId.ImageProps_PositionRight, StringResources.HtmlEditorText_ImageProps_PositionRight);
			AddString(ASPxHtmlEditorStringId.ImageProps_Description, StringResources.HtmlEditorText_ImageProps_Description);
			AddString(ASPxHtmlEditorStringId.ImageProps_Margins, StringResources.HtmlEditorText_ImageProps_Margins);
			AddString(ASPxHtmlEditorStringId.ImageProps_MarginTop, StringResources.HtmlEditorText_ImageProps_MarginTop);
			AddString(ASPxHtmlEditorStringId.ImageProps_MarginBottom, StringResources.HtmlEditorText_ImageProps_MarginBottom);
			AddString(ASPxHtmlEditorStringId.ImageProps_MarginLeft, StringResources.HtmlEditorText_ImageProps_MarginLeft);
			AddString(ASPxHtmlEditorStringId.ImageProps_MarginRight, StringResources.HtmlEditorText_ImageProps_MarginRight);
			AddString(ASPxHtmlEditorStringId.ImageProps_Border, StringResources.HtmlEditorText_ImageProps_Border);
			AddString(ASPxHtmlEditorStringId.ImageProps_BorderWidth, StringResources.HtmlEditorText_ImageProps_BorderWidth);
			AddString(ASPxHtmlEditorStringId.ImageProps_BorderColor, StringResources.HtmlEditorText_ImageProps_BorderColor);
			AddString(ASPxHtmlEditorStringId.ImageProps_BorderStyle, StringResources.HtmlEditorText_ImageProps_BorderStyle);
			AddString(ASPxHtmlEditorStringId.ImageProps_CssClass, StringResources.HtmlEditorText_ImageProps_CssClass);
			AddString(ASPxHtmlEditorStringId.InvalidUrlErrorText, StringResources.HtmlEditorText_InvalidUrl);
			AddString(ASPxHtmlEditorStringId.InsertTable, StringResources.HtmlEditorText_InsertTable);
			AddString(ASPxHtmlEditorStringId.ContextMenu_InsertTable, StringResources.HtmlEditorText_ContextMenu_InsertTable);
			AddString(ASPxHtmlEditorStringId.TableProperties, StringResources.HtmlEditorText_TableProperties);
			AddString(ASPxHtmlEditorStringId.ContextMenu_TableProperties, StringResources.HtmlEditorText_ContextMenu_TableProperties);
			AddString(ASPxHtmlEditorStringId.TableCellProperties, StringResources.HtmlEditorText_TableCellProperties);
			AddString(ASPxHtmlEditorStringId.ContextMenu_TableCellProperties, StringResources.HtmlEditorText_ContextMenu_TableCellProperties);
			AddString(ASPxHtmlEditorStringId.ChangeTableColumn_Size, StringResources.HtmlEditorText_ChangeTableColumn_Size);			
			AddString(ASPxHtmlEditorStringId.TableColumnProperties, StringResources.HtmlEditorText_TableColumnProperties);
			AddString(ASPxHtmlEditorStringId.TableRowProperties, StringResources.HtmlEditorText_TableRowProperties);
			AddString(ASPxHtmlEditorStringId.ContextMenu_TableColumnProperties, StringResources.HtmlEditorText_ContextMenu_TableColumnProperties);
			AddString(ASPxHtmlEditorStringId.ContextMenu_TableRowProperties, StringResources.HtmlEditorText_ContextMenu_TableRowProperties);
			AddString(ASPxHtmlEditorStringId.DeleteTable, StringResources.HtmlEditorText_DeleteTable);
			AddString(ASPxHtmlEditorStringId.DeleteTableRow, StringResources.HtmlEditorText_DeleteTableRow);
			AddString(ASPxHtmlEditorStringId.DeleteTableColumn, StringResources.HtmlEditorText_DeleteTableColumn);
			AddString(ASPxHtmlEditorStringId.InsertTableRowAbove, StringResources.HtmlEditorText_InsertTableRowAbove);
			AddString(ASPxHtmlEditorStringId.InsertTableRowBelow, StringResources.HtmlEditorText_InsertTableRowBelow);
			AddString(ASPxHtmlEditorStringId.InsertTableColumnToLeft, StringResources.HtmlEditorText_InsertTableColumnToLeft);
			AddString(ASPxHtmlEditorStringId.InsertTableColumnToRight, StringResources.HtmlEditorText_InsertTableColumnToRight);
			AddString(ASPxHtmlEditorStringId.MergeTableCellDown, StringResources.HtmlEditorText_MergeTableCellDown);
			AddString(ASPxHtmlEditorStringId.SplitTableCellVertical, StringResources.HtmlEditorText_SplitTableCellVertical);
			AddString(ASPxHtmlEditorStringId.MergeTableCellHorizontal, StringResources.HtmlEditorText_MergeTableCellRight);
			AddString(ASPxHtmlEditorStringId.SplitTableCellHorizontal, StringResources.HtmlEditorText_SplitTableCellHorizontal);
			AddString(ASPxHtmlEditorStringId.InsertTable_Size, StringResources.HtmlEditorText_InsertTable_Size);
			AddString(ASPxHtmlEditorStringId.InsertTable_Columns, StringResources.HtmlEditorText_InsertTable_Columns);
			AddString(ASPxHtmlEditorStringId.InsertTable_Rows, StringResources.HtmlEditorText_InsertTable_Rows);
			AddString(ASPxHtmlEditorStringId.InsertTable_Width, StringResources.HtmlEditorText_InsertTable_Width);
			AddString(ASPxHtmlEditorStringId.InsertTable_Height, StringResources.HtmlEditorText_InsertTable_Height);
			AddString(ASPxHtmlEditorStringId.InsertTable_FullWidth, StringResources.HtmlEditorText_InsertTable_FullWidth);
			AddString(ASPxHtmlEditorStringId.InsertTable_AutoFitToContent, StringResources.HtmlEditorText_InsertTable_AutoFitToContent);
			AddString(ASPxHtmlEditorStringId.InsertTable_Custom, StringResources.HtmlEditorText_InsertTable_Custom);
			AddString(ASPxHtmlEditorStringId.InsertTable_EqualColumnWidths, StringResources.HtmlEditorText_InsertTable_EqualColumnWidths);
			AddString(ASPxHtmlEditorStringId.InsertTable_Layout, StringResources.HtmlEditorText_InsertTable_Layout);
			AddString(ASPxHtmlEditorStringId.InsertTable_CellPaddings, StringResources.HtmlEditorText_InsertTable_CellPaddings);
			AddString(ASPxHtmlEditorStringId.InsertTable_Alignment, StringResources.HtmlEditorText_InsertTable_Alignment);
			AddString(ASPxHtmlEditorStringId.InsertTable_HorzAlignment, StringResources.HtmlEditorText_InsertTable_HorzAlignment);
			AddString(ASPxHtmlEditorStringId.InsertTable_VertAlignment, StringResources.HtmlEditorText_InsertTable_VertAlignment);
			AddString(ASPxHtmlEditorStringId.InsertTable_None, StringResources.HtmlEditorText_InsertTable_None);
			AddString(ASPxHtmlEditorStringId.InsertTable_Alignment_Left, StringResources.HtmlEditorText_InsertTable_Alignment_Left);
			AddString(ASPxHtmlEditorStringId.InsertTable_Alignment_Right, StringResources.HtmlEditorText_InsertTable_Alignment_Right);
			AddString(ASPxHtmlEditorStringId.InsertTable_Alignment_Center, StringResources.HtmlEditorText_InsertTable_Alignment_Center);
			AddString(ASPxHtmlEditorStringId.InsertTable_VAlignment_Top, StringResources.HtmlEditorText_InsertTable_VAlignment_Top);
			AddString(ASPxHtmlEditorStringId.InsertTable_VAlignment_Bottom, StringResources.HtmlEditorText_InsertTable_VAlignment_Bottom);
			AddString(ASPxHtmlEditorStringId.InsertTable_VAlignment_Middle, StringResources.HtmlEditorText_InsertTable_VAlignment_Middle);
			AddString(ASPxHtmlEditorStringId.InsertTable_CellSpacing, StringResources.HtmlEditorText_InsertTable_CellSpacing);
			AddString(ASPxHtmlEditorStringId.InsertTable_Appearance, StringResources.HtmlEditorText_InsertTable_Appearance);
			AddString(ASPxHtmlEditorStringId.InsertTable_BorderColor, StringResources.HtmlEditorText_InsertTable_BorderColor);
			AddString(ASPxHtmlEditorStringId.InsertTable_BorderSize, StringResources.HtmlEditorText_InsertTable_BorderSize);
			AddString(ASPxHtmlEditorStringId.InsertTable_BgColor, StringResources.HtmlEditorText_InsertTable_BgColor);
			AddString(ASPxHtmlEditorStringId.InsertTable_ApplyToAllCell, StringResources.HtmlEditorText_InsertTable_ApplyToAllCell);			
			AddString(ASPxHtmlEditorStringId.InsertTable_Accessibility, StringResources.HtmlEditorText_InsertTable_Accessibility);
			AddString(ASPxHtmlEditorStringId.InsertTable_Headers, StringResources.HtmlEditorText_InsertTable_Headers);
			AddString(ASPxHtmlEditorStringId.InsertTable_FirstRow, StringResources.HtmlEditorText_InsertTable_FirstRow);
			AddString(ASPxHtmlEditorStringId.InsertTable_FirstColumn, StringResources.HtmlEditorText_InsertTable_FirstColumn);
			AddString(ASPxHtmlEditorStringId.InsertTable_Both, StringResources.HtmlEditorText_InsertTable_Both);
			AddString(ASPxHtmlEditorStringId.InsertTable_Caption, StringResources.HtmlEditorText_InsertTable_Caption);
			AddString(ASPxHtmlEditorStringId.InsertTable_Summary, StringResources.HtmlEditorText_InsertTable_Summary);
			AddString(ASPxHtmlEditorStringId.SelectAll, StringResources.HtmlEditorText_SelectAll);
			AddString(ASPxHtmlEditorStringId.FindAndReplace, StringResources.HtmlEditorText_FindAndReplace);
			AddString(ASPxHtmlEditorStringId.SaveAsRtf, StringResources.HtmlEditorText_SaveAsRtf);
			AddString(ASPxHtmlEditorStringId.SaveAsDocx, StringResources.HtmlEditorText_SaveAsDocx);
			AddString(ASPxHtmlEditorStringId.SaveAsMht, StringResources.HtmlEditorText_SaveAsMht);
			AddString(ASPxHtmlEditorStringId.SaveAsOdt, StringResources.HtmlEditorText_SaveAsOdt);
			AddString(ASPxHtmlEditorStringId.SaveAsPdf, StringResources.HtmlEditorText_SaveAsPdf);
			AddString(ASPxHtmlEditorStringId.SaveAsTxt, StringResources.HtmlEditorText_SaveAsTxt);
			AddString(ASPxHtmlEditorStringId.SaveAsRtf_ToolTip, StringResources.HtmlEditorText_SaveAsRtf_ToolTip);
			AddString(ASPxHtmlEditorStringId.SaveAsDocx_ToolTip, StringResources.HtmlEditorText_SaveAsDocx_ToolTip);
			AddString(ASPxHtmlEditorStringId.SaveAsMht_ToolTip, StringResources.HtmlEditorText_SaveAsMht_ToolTip);
			AddString(ASPxHtmlEditorStringId.SaveAsOdt_ToolTip, StringResources.HtmlEditorText_SaveAsOdt_ToolTip);
			AddString(ASPxHtmlEditorStringId.SaveAsPdf_ToolTip, StringResources.HtmlEditorText_SaveAsPdf_ToolTip);
			AddString(ASPxHtmlEditorStringId.SaveAsTxt_ToolTip, StringResources.HtmlEditorText_SaveAsTxt_ToolTip);
			AddString(ASPxHtmlEditorStringId.RibbonTab_Home, StringResources.HtmlEditorText_RibbonTab_Home);
			AddString(ASPxHtmlEditorStringId.RibbonTab_Insert, StringResources.HtmlEditorText_RibbonTab_Insert);
			AddString(ASPxHtmlEditorStringId.RibbonTab_View, StringResources.HtmlEditorText_RibbonTab_View);
			AddString(ASPxHtmlEditorStringId.RibbonTab_Table, StringResources.HtmlEditorText_RibbonTab_Table);
			AddString(ASPxHtmlEditorStringId.RibbonTab_Review, StringResources.HtmlEditorText_RibbonTab_Review);
			AddString(ASPxHtmlEditorStringId.RibbonTabCategory_Layout, StringResources.HtmlEditorText_RibbonTabCategory_Layout);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Undo, StringResources.HtmlEditorText_RibbonGroup_Undo);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Clipboard, StringResources.HtmlEditorText_RibbonGroup_Clipboard);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Font, StringResources.HtmlEditorText_RibbonGroup_Font);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Paragraph, StringResources.HtmlEditorText_RibbonGroup_Paragraph);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Images, StringResources.HtmlEditorText_RibbonGroup_Images);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Links, StringResources.HtmlEditorText_RibbonGroup_Links);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Views, StringResources.HtmlEditorText_RibbonGroup_Views);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Tables, StringResources.HtmlEditorText_RibbonGroup_Tables);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_DeleteTable, StringResources.HtmlEditorText_RibbonGroup_DeleteTable);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_InsertTable, StringResources.HtmlEditorText_RibbonGroup_InsertTable);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_MergeTable, StringResources.HtmlEditorText_RibbonGroup_MergeTable);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_TableProperties, StringResources.HtmlEditorText_RibbonGroup_TableProperties);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Spelling, StringResources.HtmlEditorText_RibbonGroup_Spelling);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Media, StringResources.HtmlEditorText_RibbonGroup_Media);
			AddString(ASPxHtmlEditorStringId.RibbonGroup_Editing, StringResources.HtmlEditorText_RibbonGroup_Editing);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertImage, StringResources.HtmlEditorText_RibbonItem_InsertImage);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertLink, StringResources.HtmlEditorText_RibbonItem_InsertLink);
			AddString(ASPxHtmlEditorStringId.RibbonItem_Table, StringResources.HtmlEditorText_RibbonItem_Table);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertTable, StringResources.HtmlEditorText_RibbonItem_InsertTable);
			AddString(ASPxHtmlEditorStringId.RibbonItem_TableProperties, StringResources.HtmlEditorText_RibbonItem_TableProperties);
			AddString(ASPxHtmlEditorStringId.RibbonItem_CellProperties, StringResources.HtmlEditorText_RibbonItem_CellProperties);
			AddString(ASPxHtmlEditorStringId.RibbonItem_ColumnProperties, StringResources.HtmlEditorText_RibbonItem_ColumnProperties);
			AddString(ASPxHtmlEditorStringId.RibbonItem_RowProperties, StringResources.HtmlEditorText_RibbonItem_RowProperties);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertTableRowAbove, StringResources.HtmlEditorText_RibbonItem_InsertTableRowAbove);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertTableRowBelow, StringResources.HtmlEditorText_RibbonItem_InsertTableRowBelow);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertTableColumnToLeft, StringResources.HtmlEditorText_RibbonItem_InsertTableColumnToLeft);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertTableColumnToRight, StringResources.HtmlEditorText_RibbonItem_InsertTableColumnToRight);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertAudio, StringResources.HtmlEditorText_RibbonItem_InsertAudio);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertVideo, StringResources.HtmlEditorText_RibbonItem_InsertVideo);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertFlash, StringResources.HtmlEditorText_RibbonItem_InsertFlash);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertYouTubeVideo, StringResources.HtmlEditorText_RibbonItem_InsertYouTubeVideo);
			AddString(ASPxHtmlEditorStringId.RibbonItem_InsertPlaceholder, StringResources.HtmlEditorText_RibbonItem_InsertPlaceHolder);
			AddString(ASPxHtmlEditorStringId.InsertFlash_PreviewText, StringResources.HtmlEditorText_InsertFlash_PreviewText);
			AddString(ASPxHtmlEditorStringId.InsertFlash_GalleryTabText, StringResources.HtmlEditorText_InsertFlash_GalleryTabText);
			AddString(ASPxHtmlEditorStringId.InsertFlash_EnterUrl, StringResources.HtmlEditorText_InsertFlash_EnterUrl);
			AddString(ASPxHtmlEditorStringId.InsertFlash_FromLocal, StringResources.HtmlEditorText_InsertFlash_FromLocal);
			AddString(ASPxHtmlEditorStringId.InsertFlash_FromWeb, StringResources.HtmlEditorText_InsertFlash_FromWeb);
			AddString(ASPxHtmlEditorStringId.InsertFlash_MoreOptions, StringResources.HtmlEditorText_InsertFlash_MoreOptions);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Preview, StringResources.HtmlEditorText_InsertFlash_Preview);
			AddString(ASPxHtmlEditorStringId.InsertFlash_SaveToServer, StringResources.HtmlEditorText_InsertFlash_SaveToServer);
			AddString(ASPxHtmlEditorStringId.InsertFlash_UploadInstructions, StringResources.HtmlEditorText_InsertFlash_UploadInstructions);
			AddString(ASPxHtmlEditorStringId.InsertFlash_EnableFlashMenu, StringResources.HtmlEditorText_InsertFlash_EnableFlashMenu);
			AddString(ASPxHtmlEditorStringId.InsertFlash_AutoPlay, StringResources.HtmlEditorText_InsertFlash_AutoPlay);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Loop, StringResources.HtmlEditorText_InsertFlash_Loop);
			AddString(ASPxHtmlEditorStringId.InsertFlash_AllowFullscreen, StringResources.HtmlEditorText_InsertFlash_AllowFullscreen);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Quality, StringResources.HtmlEditorText_InsertFlash_Quality);
			AddString(ASPxHtmlEditorStringId.InsertFlash_QualityBest, StringResources.HtmlEditorText_InsertFlash_QualityBest);
			AddString(ASPxHtmlEditorStringId.InsertFlash_QualityHigh, StringResources.HtmlEditorText_InsertFlash_QualityHigh);
			AddString(ASPxHtmlEditorStringId.InsertFlash_QualityAutoHigh, StringResources.HtmlEditorText_InsertFlash_QualityAutoHigh);
			AddString(ASPxHtmlEditorStringId.InsertFlash_QualityMedium, StringResources.HtmlEditorText_InsertFlash_QualityMedium);
			AddString(ASPxHtmlEditorStringId.InsertFlash_QualityLow, StringResources.HtmlEditorText_InsertFlash_QualityLow);
			AddString(ASPxHtmlEditorStringId.InsertFlash_QualityAutoLow, StringResources.HtmlEditorText_InsertFlash_QualityAutoLow);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Width, StringResources.HtmlEditorText_InsertFlash_Width);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Pixels, StringResources.HtmlEditorText_InsertFlash_Pixels);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Height, StringResources.HtmlEditorText_InsertFlash_Height);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Position, StringResources.HtmlEditorText_InsertFlash_Position);
			AddString(ASPxHtmlEditorStringId.InsertFlash_PositionLeft, StringResources.HtmlEditorText_InsertFlash_PositionLeft);
			AddString(ASPxHtmlEditorStringId.InsertFlash_PositionCenter, StringResources.HtmlEditorText_InsertFlash_PositionCenter);
			AddString(ASPxHtmlEditorStringId.InsertFlash_PositionRight, StringResources.HtmlEditorText_InsertFlash_PositionRight);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Margins, StringResources.HtmlEditorText_InsertFlash_Margins);
			AddString(ASPxHtmlEditorStringId.InsertFlash_MarginTop, StringResources.HtmlEditorText_InsertFlash_MarginTop);
			AddString(ASPxHtmlEditorStringId.InsertFlash_MarginBottom, StringResources.HtmlEditorText_InsertFlash_MarginBottom);
			AddString(ASPxHtmlEditorStringId.InsertFlash_MarginLeft, StringResources.HtmlEditorText_InsertFlash_MarginLeft);
			AddString(ASPxHtmlEditorStringId.InsertFlash_MarginRight, StringResources.HtmlEditorText_InsertFlash_MarginRight);
			AddString(ASPxHtmlEditorStringId.InsertFlash_Border, StringResources.HtmlEditorText_InsertFlash_Border);
			AddString(ASPxHtmlEditorStringId.InsertFlash_BorderWidth, StringResources.HtmlEditorText_InsertFlash_BorderWidth);
			AddString(ASPxHtmlEditorStringId.InsertFlash_BorderColor, StringResources.HtmlEditorText_InsertFlash_BorderColor);
			AddString(ASPxHtmlEditorStringId.InsertFlash_BorderStyle, StringResources.HtmlEditorText_InsertFlash_BorderStyle);
			AddString(ASPxHtmlEditorStringId.InsertFlash_CssClass, StringResources.HtmlEditorText_InsertFlash_CssClass);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_EnterUrl, StringResources.HtmlEditorText_InsertYouTubeVideo_EnterUrl);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_Preview, StringResources.HtmlEditorText_InsertYouTubeVideo_Preview);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_MoreOptions, StringResources.HtmlEditorText_InsertYouTubeVideo_MoreOptions);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_ShowControls, StringResources.HtmlEditorText_InsertYouTubeVideo_ShowControls);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_ShowSameVideos, StringResources.HtmlEditorText_InsertYouTubeVideo_ShowSameVideos);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_SecureMode, StringResources.HtmlEditorText_InsertYouTubeVideo_SecureMode);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_ShowVideoName, StringResources.HtmlEditorText_InsertYouTubeVideo_ShowVideoName);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_Width, StringResources.HtmlEditorText_InsertYouTubeVideo_Width);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_Pixels, StringResources.HtmlEditorText_InsertYouTubeVideo_Pixels);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_Height, StringResources.HtmlEditorText_InsertYouTubeVideo_Height);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_Position, StringResources.HtmlEditorText_InsertYouTubeVideo_Position);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_PositionLeft, StringResources.HtmlEditorText_InsertYouTubeVideo_PositionLeft);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_PositionCenter, StringResources.HtmlEditorText_InsertYouTubeVideo_PositionCenter);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_PositionRight, StringResources.HtmlEditorText_InsertYouTubeVideo_PositionRight);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_Margins, StringResources.HtmlEditorText_InsertYouTubeVideo_Margins);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_MarginTop, StringResources.HtmlEditorText_InsertYouTubeVideo_MarginTop);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_MarginBottom, StringResources.HtmlEditorText_InsertYouTubeVideo_MarginBottom);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_MarginLeft, StringResources.HtmlEditorText_InsertYouTubeVideo_MarginLeft);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_MarginRight, StringResources.HtmlEditorText_InsertYouTubeVideo_MarginRight);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_Border, StringResources.HtmlEditorText_InsertYouTubeVideo_Border);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_BorderWidth, StringResources.HtmlEditorText_InsertYouTubeVideo_BorderWidth);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_BorderColor, StringResources.HtmlEditorText_InsertYouTubeVideo_BorderColor);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_BorderStyle, StringResources.HtmlEditorText_InsertFlash_BorderStyle);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_CssClass, StringResources.HtmlEditorText_InsertYouTubeVideo_CssClass);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PreviewText, StringResources.HtmlEditorText_InsertVideo_PreviewText);
			AddString(ASPxHtmlEditorStringId.InsertVideo_GalleryTabText, StringResources.HtmlEditorText_InsertVideo_GalleryTabText);
			AddString(ASPxHtmlEditorStringId.InsertVideo_FromWeb, StringResources.HtmlEditorText_InsertVideo_FromWeb);
			AddString(ASPxHtmlEditorStringId.InsertVideo_FromLocal, StringResources.HtmlEditorText_InsertVideo_FromLocal);
			AddString(ASPxHtmlEditorStringId.InsertVideo_EnterUrl, StringResources.HtmlEditorText_InsertVideo_EnterUrl);
			AddString(ASPxHtmlEditorStringId.InsertVideo_SaveToServer, StringResources.HtmlEditorText_InsertVideo_SaveToServer);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Preview, StringResources.HtmlEditorText_InsertVideo_Preview);
			AddString(ASPxHtmlEditorStringId.InsertVideo_UploadInstructions, StringResources.HtmlEditorText_InsertVideo_UploadInstructions);
			AddString(ASPxHtmlEditorStringId.InsertVideo_MoreOptions, StringResources.HtmlEditorText_InsertVideo_MoreOptions);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Preload, StringResources.HtmlEditorText_InsertVideo_Preload);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PreloadNone, StringResources.HtmlEditorText_InsertVideo_PreloadNone);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PreloadMetadata, StringResources.HtmlEditorText_InsertVideo_PreloadMetadata);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PreloadAuto, StringResources.HtmlEditorText_InsertVideo_PreloadAuto);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Poster, StringResources.HtmlEditorText_InsertVideo_Poster);
			AddString(ASPxHtmlEditorStringId.InsertVideo_AutoPlay, StringResources.HtmlEditorText_InsertVideo_AutoPlay);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Loop, StringResources.HtmlEditorText_InsertVideo_Loop);
			AddString(ASPxHtmlEditorStringId.InsertVideo_ShowControls, StringResources.HtmlEditorText_InsertVideo_ShowControls);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Width, StringResources.HtmlEditorText_InsertVideo_Width);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Pixels, StringResources.HtmlEditorText_InsertVideo_Pixels);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Height, StringResources.HtmlEditorText_InsertVideo_Height);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Position, StringResources.HtmlEditorText_InsertVideo_Position);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PositionLeft, StringResources.HtmlEditorText_InsertVideo_PositionLeft);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PositionCenter, StringResources.HtmlEditorText_InsertVideo_PositionCenter);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PositionRight, StringResources.HtmlEditorText_InsertVideo_PositionRight);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Margins, StringResources.HtmlEditorText_InsertVideo_Margins);
			AddString(ASPxHtmlEditorStringId.InsertVideo_MarginTop, StringResources.HtmlEditorText_InsertVideo_MarginTop);
			AddString(ASPxHtmlEditorStringId.InsertVideo_MarginBottom, StringResources.HtmlEditorText_InsertVideo_MarginBottom);
			AddString(ASPxHtmlEditorStringId.InsertVideo_MarginLeft, StringResources.HtmlEditorText_InsertVideo_MarginLeft);
			AddString(ASPxHtmlEditorStringId.InsertVideo_MarginRight, StringResources.HtmlEditorText_InsertVideo_MarginRight);
			AddString(ASPxHtmlEditorStringId.InsertVideo_Border, StringResources.HtmlEditorText_InsertVideo_Border);
			AddString(ASPxHtmlEditorStringId.InsertVideo_BorderWidth, StringResources.HtmlEditorText_InsertVideo_BorderWidth);
			AddString(ASPxHtmlEditorStringId.InsertVideo_BorderColor, StringResources.HtmlEditorText_InsertVideo_BorderColor);
			AddString(ASPxHtmlEditorStringId.InsertVideo_BorderStyle, StringResources.HtmlEditorText_InsertVideo_BorderStyle);
			AddString(ASPxHtmlEditorStringId.InsertVideo_CssClass, StringResources.HtmlEditorText_InsertVideo_CssClass);
			AddString(ASPxHtmlEditorStringId.InsertAudio_PreviewText, StringResources.HtmlEditorText_InsertAudio_PreviewText);
			AddString(ASPxHtmlEditorStringId.InsertAudio_GalleryTabText, StringResources.HtmlEditorText_InsertAudio_GalleryTabText);
			AddString(ASPxHtmlEditorStringId.InsertAudio_FromWeb, StringResources.HtmlEditorText_InsertAudio_FromWeb);
			AddString(ASPxHtmlEditorStringId.InsertAudio_FromLocal, StringResources.HtmlEditorText_InsertAudio_FromLocal);
			AddString(ASPxHtmlEditorStringId.InsertAudio_EnterUrl, StringResources.HtmlEditorText_InsertAudio_EnterUrl);
			AddString(ASPxHtmlEditorStringId.InsertAudio_SaveToServer, StringResources.HtmlEditorText_InsertAudio_SaveToServer);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Preview, StringResources.HtmlEditorText_InsertAudio_Preview);
			AddString(ASPxHtmlEditorStringId.InsertAudio_UploadInstructions, StringResources.HtmlEditorText_InsertAudio_UploadInstructions);
			AddString(ASPxHtmlEditorStringId.InsertAudio_MoreOptions, StringResources.HtmlEditorText_InsertAudio_MoreOptions);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Preload, StringResources.HtmlEditorText_InsertAudio_Preload);
			AddString(ASPxHtmlEditorStringId.InsertAudio_PreloadNone, StringResources.HtmlEditorText_InsertAudio_PreloadNone);
			AddString(ASPxHtmlEditorStringId.InsertAudio_PreloadMetadata, StringResources.HtmlEditorText_InsertAudio_PreloadMetadata);
			AddString(ASPxHtmlEditorStringId.InsertAudio_PreloadAuto, StringResources.HtmlEditorText_InsertAudio_PreloadAuto);
			AddString(ASPxHtmlEditorStringId.InsertAudio_AutoPlay, StringResources.HtmlEditorText_InsertAudio_AutoPlay);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Loop, StringResources.HtmlEditorText_InsertAudio_Loop);
			AddString(ASPxHtmlEditorStringId.InsertAudio_ShowControls, StringResources.HtmlEditorText_InsertAudio_ShowControls);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Width, StringResources.HtmlEditorText_InsertAudio_Width);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Pixels, StringResources.HtmlEditorText_InsertAudio_Pixels);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Height, StringResources.HtmlEditorText_InsertAudio_Height);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Position, StringResources.HtmlEditorText_InsertAudio_Position);
			AddString(ASPxHtmlEditorStringId.InsertAudio_PositionLeft, StringResources.HtmlEditorText_InsertAudio_PositionLeft);
			AddString(ASPxHtmlEditorStringId.InsertAudio_PositionCenter, StringResources.HtmlEditorText_InsertAudio_PositionCenter);
			AddString(ASPxHtmlEditorStringId.InsertAudio_PositionRight, StringResources.HtmlEditorText_InsertAudio_PositionRight);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Margins, StringResources.HtmlEditorText_InsertAudio_Margins);
			AddString(ASPxHtmlEditorStringId.InsertAudio_MarginTop, StringResources.HtmlEditorText_InsertAudio_MarginTop);
			AddString(ASPxHtmlEditorStringId.InsertAudio_MarginBottom, StringResources.HtmlEditorText_InsertAudio_MarginBottom);
			AddString(ASPxHtmlEditorStringId.InsertAudio_MarginLeft, StringResources.HtmlEditorText_InsertAudio_MarginLeft);
			AddString(ASPxHtmlEditorStringId.InsertAudio_MarginRight, StringResources.HtmlEditorText_InsertAudio_MarginRight);
			AddString(ASPxHtmlEditorStringId.InsertAudio_Border, StringResources.HtmlEditorText_InsertAudio_Border);
			AddString(ASPxHtmlEditorStringId.InsertAudio_BorderWidth, StringResources.HtmlEditorText_InsertAudio_BorderWidth);
			AddString(ASPxHtmlEditorStringId.InsertAudio_BorderColor, StringResources.HtmlEditorText_InsertAudio_BorderColor);
			AddString(ASPxHtmlEditorStringId.InsertAudio_BorderStyle, StringResources.HtmlEditorText_InsertAudio_BorderStyle);
			AddString(ASPxHtmlEditorStringId.InsertAudio_CssClass, StringResources.HtmlEditorText_InsertAudio_CssClass);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Id, StringResources.HtmlEditorText_ChangeElementProperties_Id);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Value, StringResources.HtmlEditorText_ChangeElementProperties_Value);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Title, StringResources.HtmlEditorText_ChangeElementProperties_Title);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_TabIndex, StringResources.HtmlEditorText_ChangeElementProperties_TabIndex);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Accept, StringResources.HtmlEditorText_ChangeElementProperties_Accept);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Alt, StringResources.HtmlEditorText_ChangeElementProperties_Alt);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Src, StringResources.HtmlEditorText_ChangeElementProperties_Src);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Direction, StringResources.HtmlEditorText_ChangeElementProperties_Direction);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_InputType, StringResources.HtmlEditorText_ChangeElementProperties_InputType);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Action, StringResources.HtmlEditorText_ChangeElementProperties_Action);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Method, StringResources.HtmlEditorText_ChangeElementProperties_Method);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Name, StringResources.HtmlEditorText_ChangeElementProperties_Name);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_For, StringResources.HtmlEditorText_ChangeElementProperties_For);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Disabled, StringResources.HtmlEditorText_ChangeElementProperties_Disabled);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Checked, StringResources.HtmlEditorText_ChangeElementProperties_Checked);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_MaxLength, StringResources.HtmlEditorText_ChangeElementProperties_MaxLength);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Size, StringResources.HtmlEditorText_ChangeElementProperties_Size);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_Readonly, StringResources.HtmlEditorText_ChangeElementProperties_Readonly);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_CommonSettingsTabName, StringResources.HtmlEditorText_ChangeElementProperties_CommonSettingsTabName);
			AddString(ASPxHtmlEditorStringId.ChangeElementProperties_StyleSettingsTabName, StringResources.HtmlEditorText_ChangeElementProperties_StyleSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertAudio_CommonSettingsTabName, StringResources.HtmlEditorText_InsertAudio_CommonSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertAudio_StyleSettingsTabName, StringResources.HtmlEditorText_InsertAudio_StyleSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertFlash_CommonSettingsTabName, StringResources.HtmlEditorText_InsertFlash_CommonSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertFlash_StyleSettingsTabName, StringResources.HtmlEditorText_InsertFlash_StyleSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertVideo_CommonSettingsTabName, StringResources.HtmlEditorText_InsertVideo_CommonSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertVideo_StyleSettingsTabName, StringResources.HtmlEditorText_InsertVideo_StyleSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertImage_CommonSettingsTabName, StringResources.HtmlEditorText_InsertImage_CommonSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertImage_StyleSettingsTabName, StringResources.HtmlEditorText_InsertImage_StyleSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_CommonSettingsTabName, StringResources.HtmlEditorText_InsertYouTubeVideo_CommonSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_StyleSettingsTabName, StringResources.HtmlEditorText_InsertYouTubeVideo_StyleSettingsTabName);
			AddString(ASPxHtmlEditorStringId.InsertAudio_PreloadHelpText, StringResources.HtmlEditorText_InsertAudio_PreloadHelpText);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PosterHelpText, StringResources.HtmlEditorText_InsertVideo_PosterHelpText);
			AddString(ASPxHtmlEditorStringId.InsertVideo_PreloadHelpText, StringResources.HtmlEditorText_InsertVideo_PreloadHelpText);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_SecureModeHelpText, StringResources.HtmlEditorText_InsertYouTubeVideo_SecureModeHelpText);
			AddString(ASPxHtmlEditorStringId.InsertYouTubeVideo_ValidationErrorText, StringResources.HtmlEditorText_InsertYouTubeVideo_ValidationErrorText);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_Next, StringResources.HtmlEditorText_AdvancedSearch_Next);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_Previous, StringResources.HtmlEditorText_AdvancedSearch_Previous);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_Replace, StringResources.HtmlEditorText_AdvancedSearch_Replace);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_ReplaceAll, StringResources.HtmlEditorText_AdvancedSearch_ReplaceAll);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_Results, StringResources.HtmlEditorText_AdvancedSearch_Results);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_ReplaceWith, StringResources.HtmlEditorText_AdvancedSearch_ReplaceWith);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_Find, StringResources.HtmlEditorText_AdvancedSearch_Find);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_MatchCase, StringResources.HtmlEditorText_AdvancedSearch_MatchCase);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_Header, StringResources.HtmlEditorText_AdvancedSearch_Header);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_Of, StringResources.HtmlEditorText_AdvancedSearch_Of);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_ReplaceAllNotify, StringResources.HtmlEditorText_AdvancedSearch_ReplaceAllNotify);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_ReplaceProcessNotify, StringResources.HtmlEditorText_AdvancedSearch_ReplaceProcessNotify);
			AddString(ASPxHtmlEditorStringId.AdvancedSearch_SearchLimit, StringResources.HtmlEditorText_AdvancedSearch_SearchLimit);
			AddString(ASPxHtmlEditorStringId.QuickSearch_SearchFieldNullText, StringResources.HtmlEditorText_QuickSearch_SearchFieldNullText);
		}
	}
}
