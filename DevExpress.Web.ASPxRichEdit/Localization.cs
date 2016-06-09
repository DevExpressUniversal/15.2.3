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
using DevExpress.Utils.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using System.ComponentModel;
using System.Drawing.Printing;
namespace DevExpress.Web.ASPxRichEdit.Localization {
	public static class RichEditLocalization {
		public static string GetPaperKindDisplayName(PaperKind paperKind) {
			Type resourceFinder = typeof(DevExpress.Data.ResFinder);
			string resourceFileName = DXDisplayNameAttribute.DefaultResourceFile;
			var resourceManager = new System.Resources.ResourceManager(string.Concat(resourceFinder.Namespace, ".", resourceFileName), resourceFinder.Assembly);
			return GetPaperKindDisplayName(resourceManager, paperKind);
		}
		public static string GetPaperKindDisplayName(System.Resources.ResourceManager resourceManager, PaperKind paperKind) {
			return resourceManager.GetString(GetResourceName(paperKind));
		}
		static string GetResourceName(object enumValue) {
			return string.Concat(enumValue.GetType().FullName, ".", enumValue);
		}
	}
	public enum ASPxRichEditStringId {
		Font,
		FontStyle,
		FontSize,
		FontColor,
		UnderlineStyle,
		UnderlineColor,
		Effects,
		AllCaps,
		Hidden,
		Normal,
		Bold,
		Italic,
		BoldItalic,
		Dotted,
		Strikeout,
		DoubleStrikeout,
		Superscript,
		Subscript,
		UnderlineWordsOnly,
		UnderlineType_None,
		UnderlineType_Single,
		UnderlineType_Dotted,
		UnderlineType_Dashed,
		UnderlineType_DashDotted,
		UnderlineType_DashDotDotted,
		UnderlineType_Double,
		UnderlineType_HeavyWave,
		UnderlineType_LongDashed,
		UnderlineType_ThickSingle,
		UnderlineType_ThickDotted,
		UnderlineType_ThickDashed,
		UnderlineType_ThickDashDotted,
		UnderlineType_ThickDashDotDotted,
		UnderlineType_ThickLongDashed,
		UnderlineType_DoubleWave,
		UnderlineType_Wave,
		UnderlineType_DashSmallGap,
		StrikeoutType_None,
		StrikeoutType_Single,
		StrikeoutType_Double,
		CharacterFormattingScript_Normal,
		CharacterFormattingScript_Subscript,
		CharacterFormattingScript_Superscript,
		ParagraphTab0,
		ParagraphTab1,
		General,
		Alignment,
		Left,
		Right,
		Centered,
		Justified,
		OutlineLevel,
		BodyText,
		Level,
		Indentation,
		Special,
		By,
		none,
		FirstLine,
		Hanging,
		Spacing,
		Before,
		LineSpacing,
		At,
		After,
		Single,
		spacing_1_5_lines,
		Double,
		Multiple,
		Exactly,
		AtLeast,
		NoSpace,
		Pagination,
		KLT,
		PBB,
		Options,
		Preview,
		Style,
		Color,
		SaveToServer,
		DownloadCopy,
		FolderPath,
		FileType,
		FileName,
		ChooseType,
		SaveFileAs,
		Invalid_FileName,
		Portrait,
		Landscape,
		Continuous,
		NewPage,
		EvenPage,
		OddPage,
		WholeDocument,
		CurrentSection,
		SelectedSections,
		ThisPointForward,
		Margins,
		Paper,
		Layout,
		Top,
		Bottom,
		Orientation,
		PaperSize,
		Width,
		Height,
		Section,
		SectionStart,
		HeadersAndFooters,
		DifferentOddAndEven,
		DifferentFirstPage,
		ApplyTo,
		CancelButton,
		OkButton,
		OpenButton,
		SaveAsButton,
		DownloadButton,
		SelectButton,
		InsertButton,
		ChangeButton,
		CloseButton,
		InsertTable_TableSize,
		InsertTable_NumberOfColumns,
		InsertTable_NumberOfRows,
		SplitCells_MergeCells,
		InsertCells_ShiftCellsRight,
		InsertCells_ShiftCellsDown,
		InsertCells_InsertEntireRow,
		InsertCells_InsertEntireColumn,
		DeleteCells_ShiftCellsLeft,
		DeleteCells_ShiftCellsUp,
		DeleteCells_DeleteEntireRow,
		DeleteCells_DeleteEntireColumn,
		InsertImage_FromWeb,
		InsertImage_FromLocal,
		InsertImage_EnterUrl,
		InsertImage_UploadInstructions,
		InsertImage_ImagePreview,
		TableProperties_Size,
		TableProperties_Alignment,
		TableProperties_VerticalAlignment,
		TableProperties_MeasureIn,
		TableProperties_PreferredWidth,
		TableProperties_IndentLeft,
		TableProperties_SpecifyHeight,
		TableProperties_RowHeight,
		TableProperties_Table,
		TableProperties_Row,
		TableProperties_Column,
		TableProperties_Cell,
		TableProperties_Options,
		TableProperties_BordersAndShading,
		TableProperties_PreviousRow,
		TableProperties_NextRow,
		TableProperties_PreviousColumn,
		TableProperties_NextColumn,
		TableOptions_DefaultCellMargins,
		TableOptions_DefaultCellSpacing,
		TableOptions_AllowSpacing,
		TableOptions_AutoResize,
		BorderShading_Borders,
		BorderShading_Shading,
		BorderShading_Fill,
		BorderShading_BordersDescription,
		BorderShading_BordersNone,
		BorderShading_BordersBox,
		BorderShading_BordersAll,
		BorderShading_BordersGrid,
		BorderShading_BordersCustom,
		CellOptions_CellMargins,
		CellOptions_SameAsTable,
		CellOptions_WrapText,
		Columns_Presets,
		Columns_NumberOfColumns,
		Columns_WidthAndSpacing,
		Columns_EqualWidth,
		Columns_Col,
		Columns_Width,
		Columns_Spacing,
		PageFile,
		PageHome,
		PageInsert,
		PagePageLayout,
		PageReferences,
		PageMailings,
		PageReview,
		PageView,
		PageHeaderAndFooter,
		PageTableDesign,
		PageTableLayout,
		PagePicture,
		GroupUndo,
		GroupFont,
		GroupParagraph,
		GroupClipboard,
		GroupEditing,
		GroupCommon,
		GroupStyles,
		GroupZoom,
		GroupShow,
		GroupIllustrations,
		GroupText,
		GroupTables,
		GroupSymbols,
		GroupLinks,
		GroupPages,
		GroupHeaderFooter,
		GroupHeaderFooterToolsDesignClose,
		GroupHeaderFooterToolsDesignNavigation,
		GroupMailMerge,
		GroupDocumentViews,
		GroupHeaderFooterToolsDesignOptions,
		GroupPageSetup,
		GroupDocumentProtection,
		GroupDocumentProofing,
		GroupDocumentComment,
		GroupDocumentTracking,
		GroupTableOfContents,
		GroupFloatingPictureToolsArrange,
		GroupFloatingPictureToolsShapeStyles,
		GroupCaptions,
		GroupPageBackground,
		GroupDocumentLanguage,
		GroupView,
		GroupTableToolsDesignTableStyleOptions,
		GroupTableToolsDesignTableStyles,
		GroupTableToolsDesignBordersAndShadings,
		GroupTableToolsLayoutTable,
		GroupTableToolsLayoutRowsAndColumns,
		GroupTableToolsLayoutMerge,
		GroupTableToolsLayoutCellSize,
		GroupTableToolsLayoutAlignment,
		RequiredFieldError,
		MenuCmd_ToggleFullScreen,
		MenuCmd_ToggleFullScreenDescription,
		SaveAsFileTitle,
		OpenFileTitle,
		FontTitle,
		ParagraphTitle,
		PageSetupTitle,
		ColumnsTitle,
		InsertTableTitle,
		InsertTableCellsTitle,
		DeleteTableCellsTitle,
		SplitTableCellsTitle,
		TablePropertiesTitle,
		BorderShadingTitle,
		InsertMergeFieldTitle,
		ExportRangeTitle,
		BookmarkTitle,
		InsertImageTitle,
		ErrorTitle,
		RulerMarginLeftTitle,
		RulerMarginRightTitle,
		RulerFirstLineIdentTitle,
		RulerHangingIdentTitle,
		RulerRightIdentTitle,
		RulerLeftIdentTitle,
		RulerLeftTabTitle,
		RulerRightTabTitle,
		RulerCenterTabTitle,
		RulerDecimalTabTitle,
		MarginsNarrow,
		MarginsNormal,
		MarginsModerate,
		MarginsWide,
		ModelIsChangedError,
		SessionHasExpiredError,
		OpeningAndOverstoreImpossibleError,
		ClipboardAccessDeniedError,
		ClipboardAccessDeniedErrorTouch,
		InnerExceptionsError,
		AuthExceptionsError,
		ConfirmOnLosingChanges,
		RestartNumbering,
		ContinueNumbering,
		UpdateField,
		ToggleFieldCodes,
		OpenHyperlink,
		EditHyperlink,
		RemoveHyperlink,
		CantOpenDocumentError,
		CantSaveDocumentError,
		DocVariableExceptionError,
		MenuCmd_CreateField,
		MenuCmd_CreateFieldDescription,
		MenuCmd_UpdateAllFields,
		MenuCmd_UpdateAllFieldsDescription,
		ParagraphStyles,
		CharacterStyles,
		BulletedAndNumberingTitle,
		CustomizeNumberedListTitle,
		CustomizeBulletedListTitle,
		CustomizeOutlineNumberedTitle,
		HyperlinkTitle,
		TabsTitle,
		SymbolsTitle,
		OtherLabels_None,
		OtherLabels_All,
		Numbering_Character,
		Numbering_Font,
		Numbering_BulletCharacter,
		Numbering_BulletPosition,
		Numbering_TextPosition,
		Numbering_AlignedAt,
		Numbering_IndentAt,
		Numbering_StartAt,
		Numbering_NumberFormat,
		Numbering_NumberStyle,
		Numbering_NumberPosition,
		Numbering_Left,
		Numbering_Center,
		Numbering_Right,
		Numbering_FollowNumberWith,
		Numbering_Level,
		Numbering_TabCharacter,
		Numbering_Space,
		Numbering_Nothing,
		Numbering_Customize,
		Numbering_Bulleted,
		Numbering_Numbered,
		Numbering_OutlineNumbered,
		Hyperlink_EmailAddress,
		Hyperlink_Url,
		Hyperlink_Bookmark,
		Hyperlink_WebPage,
		Hyperlink_PlaceInThisDocument,
		Hyperlink_EmailTo,
		Hyperlink_Subject,
		Hyperlink_DisplayProperties,
		Hyperlink_Text,
		Hyperlink_ToolTip,
		Hyperlink_InvalidEmail,
		Tabs_Set,
		Tabs_Clear,
		Tabs_ClearAll,
		Tabs_TabStopPosition,
		Tabs_DefaultTabStops,
		Tabs_TabStopsToBeCleared,
		Tabs_Alignment,
		Tabs_Leader,
		Tabs_Left,
		Tabs_Center,
		Tabs_Right,
		Tabs_Decimal,
		Tabs_None,
		Tabs_Hyphens,
		Tabs_EqualSign,
		Tabs_Dots,
		Tabs_Underline,
		Tabs_MiddleDots,
		Tabs_ThickLine,
		FinishMerge_From,
		FinishMerge_Count,
		FinishMerge_MergeMode,
		FinishMerge_AllRecord,
		FinishMerge_CurrentRecord,
		FinishMerge_Range,
		FinishMerge_NewParagraph,
		FinishMerge_NewSection,
		FinishMerge_JoinTables,
		InsertMergeField_Fields,
		Bookmarks_BookmarkName,
		Bookmarks_SortBy,
		Bookmarks_Name,
		Bookmarks_Location,
		Bookmarks_Add,
		Bookmarks_Delete,
		Bookmarks_GoTo,
		MenuCmd_MakeTextSentenceCase,
		MenuCmd_MakeTextSentenceCaseDescription,
		MenuCmd_ShowCellOptionsForm,
		MenuCmd_ShowCellOptionsFormDescription
	}
	public class ASPxRichEditResourcesLocalizer : ASPxResLocalizerBase<ASPxRichEditStringId> {
		public ASPxRichEditResourcesLocalizer()
			: base(new ASPxRichEditLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName {
			get {
				return AssemblyInfo.SRAssemblyWebRichEdit;
			}
		}
		protected override string ResxName {
			get {
				return "DevExpress.Web.ASPxRichEdit.LocalizationRes";
			}
		}
	}
	public class ASPxRichEditLocalizer : XtraLocalizer<ASPxRichEditStringId> {
		static ASPxRichEditLocalizer() {
			SetActiveLocalizerProvider(new ASPxActiveLocalizerProvider<ASPxRichEditStringId>(CreateResLocalizerInstance));
		}
		static XtraLocalizer<ASPxRichEditStringId> CreateResLocalizerInstance() {
			return new ASPxRichEditResourcesLocalizer();
		}
		public static string GetString(ASPxRichEditStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxRichEditStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxRichEditStringId.Font, StringResources.RichEdit_Font);
			AddString(ASPxRichEditStringId.FontStyle, StringResources.RichEdit_FontStyle);
			AddString(ASPxRichEditStringId.Normal, StringResources.RichEdit_Normal);
			AddString(ASPxRichEditStringId.Bold, StringResources.RichEdit_Bold);
			AddString(ASPxRichEditStringId.Italic, StringResources.RichEdit_Italic);
			AddString(ASPxRichEditStringId.BoldItalic, StringResources.RichEdit_BoldItalic);
			AddString(ASPxRichEditStringId.FontSize, StringResources.RichEdit_FontSize);
			AddString(ASPxRichEditStringId.FontColor, StringResources.RichEdit_FontColor);
			AddString(ASPxRichEditStringId.UnderlineStyle, StringResources.RichEdit_UnderlineStyle);
			AddString(ASPxRichEditStringId.UnderlineColor, StringResources.RichEdit_UnderlineColor);
			AddString(ASPxRichEditStringId.Effects, StringResources.RichEdit_Effects);
			AddString(ASPxRichEditStringId.AllCaps, StringResources.RichEdit_AllCaps);
			AddString(ASPxRichEditStringId.Hidden, StringResources.RichEdit_Hidden);
			AddString(ASPxRichEditStringId.UnderlineWordsOnly, StringResources.RichEdit_UnderlineWordsOnly);
			AddString(ASPxRichEditStringId.UnderlineType_None, StringResources.RichEdit_UnderlineType_None);
			AddString(ASPxRichEditStringId.UnderlineType_Single, StringResources.RichEdit_UnderlineType_Single);
			AddString(ASPxRichEditStringId.UnderlineType_Dotted, StringResources.RichEdit_UnderlineType_Dotted);
			AddString(ASPxRichEditStringId.UnderlineType_Dashed, StringResources.RichEdit_UnderlineType_Dashed);
			AddString(ASPxRichEditStringId.UnderlineType_DashDotted, StringResources.RichEdit_UnderlineType_DashDotted);
			AddString(ASPxRichEditStringId.UnderlineType_DashDotDotted, StringResources.RichEdit_UnderlineType_DashDotDotted);
			AddString(ASPxRichEditStringId.UnderlineType_Double, StringResources.RichEdit_UnderlineType_Double);
			AddString(ASPxRichEditStringId.UnderlineType_HeavyWave, StringResources.RichEdit_UnderlineType_HeavyWave);
			AddString(ASPxRichEditStringId.UnderlineType_LongDashed, StringResources.RichEdit_UnderlineType_LongDashed);
			AddString(ASPxRichEditStringId.UnderlineType_ThickSingle, StringResources.RichEdit_UnderlineType_ThickSingle);
			AddString(ASPxRichEditStringId.UnderlineType_ThickDotted, StringResources.RichEdit_UnderlineType_ThickDotted);
			AddString(ASPxRichEditStringId.UnderlineType_ThickDashed, StringResources.RichEdit_UnderlineType_ThickDashed);
			AddString(ASPxRichEditStringId.UnderlineType_ThickDashDotted, StringResources.RichEdit_UnderlineType_ThickDashDotted);
			AddString(ASPxRichEditStringId.UnderlineType_ThickDashDotDotted, StringResources.RichEdit_UnderlineType_ThickDashDotDotted);
			AddString(ASPxRichEditStringId.UnderlineType_ThickLongDashed, StringResources.RichEdit_UnderlineType_ThickLongDashed);
			AddString(ASPxRichEditStringId.UnderlineType_DoubleWave, StringResources.RichEdit_UnderlineType_DoubleWave);
			AddString(ASPxRichEditStringId.UnderlineType_Wave, StringResources.RichEdit_UnderlineType_Wave);
			AddString(ASPxRichEditStringId.UnderlineType_DashSmallGap, StringResources.RichEdit_UnderlineType_DashSmallGap);
			AddString(ASPxRichEditStringId.StrikeoutType_None, StringResources.RichEdit_StrikeoutType_None);
			AddString(ASPxRichEditStringId.StrikeoutType_Single, StringResources.RichEdit_StrikeoutType_Single);
			AddString(ASPxRichEditStringId.StrikeoutType_Double, StringResources.RichEdit_StrikeoutType_Double);
			AddString(ASPxRichEditStringId.CharacterFormattingScript_Normal, StringResources.RichEdit_CharacterFormattingScript_Normal);
			AddString(ASPxRichEditStringId.CharacterFormattingScript_Subscript, StringResources.RichEdit_CharacterFormattingScript_Subscript);
			AddString(ASPxRichEditStringId.CharacterFormattingScript_Superscript, StringResources.RichEdit_CharacterFormattingScript_Superscript);
			AddString(ASPxRichEditStringId.ParagraphTab0, StringResources.RichEdit_ParagraphTab0);
			AddString(ASPxRichEditStringId.ParagraphTab1, StringResources.RichEdit_ParagraphTab1);
			AddString(ASPxRichEditStringId.General, StringResources.RichEdit_General);
			AddString(ASPxRichEditStringId.Alignment, StringResources.RichEdit_Alignment);
			AddString(ASPxRichEditStringId.Left, StringResources.RichEdit_Left);
			AddString(ASPxRichEditStringId.Right, StringResources.RichEdit_Right);
			AddString(ASPxRichEditStringId.Centered, StringResources.RichEdit_Centered);
			AddString(ASPxRichEditStringId.Justified, StringResources.RichEdit_Justified);
			AddString(ASPxRichEditStringId.OutlineLevel, StringResources.RichEdit_OutlineLevel);
			AddString(ASPxRichEditStringId.BodyText, StringResources.RichEdit_BodyText);
			AddString(ASPxRichEditStringId.Level, StringResources.RichEdit_Level);
			AddString(ASPxRichEditStringId.Indentation, StringResources.RichEdit_Indentation);
			AddString(ASPxRichEditStringId.Special, StringResources.RichEdit_Special);
			AddString(ASPxRichEditStringId.By, StringResources.RichEdit_By);
			AddString(ASPxRichEditStringId.none, StringResources.RichEdit_none);
			AddString(ASPxRichEditStringId.FirstLine, StringResources.RichEdit_FirstLine);
			AddString(ASPxRichEditStringId.Hanging, StringResources.RichEdit_Hanging);
			AddString(ASPxRichEditStringId.Spacing, StringResources.RichEdit_Spacing);
			AddString(ASPxRichEditStringId.Before, StringResources.RichEdit_Before);
			AddString(ASPxRichEditStringId.LineSpacing, StringResources.RichEdit_LineSpacing);
			AddString(ASPxRichEditStringId.At, StringResources.RichEdit_At);
			AddString(ASPxRichEditStringId.After, StringResources.RichEdit_After);
			AddString(ASPxRichEditStringId.Single, StringResources.RichEdit_Single);
			AddString(ASPxRichEditStringId.spacing_1_5_lines, StringResources.RichEdit_15lines);
			AddString(ASPxRichEditStringId.Double, StringResources.RichEdit_Double);
			AddString(ASPxRichEditStringId.Multiple, StringResources.RichEdit_Multiple);
			AddString(ASPxRichEditStringId.Exactly, StringResources.RichEdit_Exactly);
			AddString(ASPxRichEditStringId.AtLeast, StringResources.RichEdit_AtLeast);
			AddString(ASPxRichEditStringId.NoSpace, StringResources.RichEdit_NoSpace);
			AddString(ASPxRichEditStringId.Pagination, StringResources.RichEdit_Pagination);
			AddString(ASPxRichEditStringId.KLT, StringResources.RichEdit_KLT);
			AddString(ASPxRichEditStringId.PBB, StringResources.RichEdit_PBB);
			AddString(ASPxRichEditStringId.Options, StringResources.RichEdit_Options);
			AddString(ASPxRichEditStringId.Preview, StringResources.RichEdit_Preview);
			AddString(ASPxRichEditStringId.Style, StringResources.RichEdit_Style);
			AddString(ASPxRichEditStringId.Color, StringResources.RichEdit_Color);
			AddString(ASPxRichEditStringId.SaveToServer, StringResources.RichEdit_SaveToServer);
			AddString(ASPxRichEditStringId.DownloadCopy, StringResources.RichEdit_DownloadCopy);
			AddString(ASPxRichEditStringId.FolderPath, StringResources.RichEdit_FolderPath);
			AddString(ASPxRichEditStringId.FileType, StringResources.RichEdit_FileType);
			AddString(ASPxRichEditStringId.FileName, StringResources.RichEdit_FileName);
			AddString(ASPxRichEditStringId.ChooseType, StringResources.RichEdit_ChooseType);
			AddString(ASPxRichEditStringId.SaveFileAs, StringResources.RichEdit_SaveFileAs);
			AddString(ASPxRichEditStringId.Invalid_FileName, StringResources.RichEdit_Invalid_FileName);
			AddString(ASPxRichEditStringId.Portrait, StringResources.RichEdit_Portrait);
			AddString(ASPxRichEditStringId.Landscape, StringResources.RichEdit_Landscape);
			AddString(ASPxRichEditStringId.Continuous, StringResources.RichEdit_Continuous);
			AddString(ASPxRichEditStringId.NewPage, StringResources.RichEdit_NewPage);
			AddString(ASPxRichEditStringId.EvenPage, StringResources.RichEdit_EvenPage);
			AddString(ASPxRichEditStringId.OddPage, StringResources.RichEdit_OddPage);
			AddString(ASPxRichEditStringId.WholeDocument, StringResources.RichEdit_WholeDocument);
			AddString(ASPxRichEditStringId.CurrentSection, StringResources.RichEdit_CurrentSection);
			AddString(ASPxRichEditStringId.SelectedSections, StringResources.RichEdit_SelectedSections);
			AddString(ASPxRichEditStringId.ThisPointForward, StringResources.RichEdit_ThisPointForward);
			AddString(ASPxRichEditStringId.Margins, StringResources.RichEdit_Margins);
			AddString(ASPxRichEditStringId.Paper, StringResources.RichEdit_Paper);
			AddString(ASPxRichEditStringId.Layout, StringResources.RichEdit_Layout);
			AddString(ASPxRichEditStringId.Top, StringResources.RichEdit_Top);
			AddString(ASPxRichEditStringId.Bottom, StringResources.RichEdit_Bottom);
			AddString(ASPxRichEditStringId.Orientation, StringResources.RichEdit_Orientation);
			AddString(ASPxRichEditStringId.PaperSize, StringResources.RichEdit_PaperSize);
			AddString(ASPxRichEditStringId.Width, StringResources.RichEdit_Width);
			AddString(ASPxRichEditStringId.Height, StringResources.RichEdit_Height);
			AddString(ASPxRichEditStringId.Section, StringResources.RichEdit_Section);
			AddString(ASPxRichEditStringId.SectionStart, StringResources.RichEdit_SectionStart);
			AddString(ASPxRichEditStringId.HeadersAndFooters, StringResources.RichEdit_HeadersAndFooters);
			AddString(ASPxRichEditStringId.DifferentOddAndEven, StringResources.RichEdit_DifferentOddAndEven);
			AddString(ASPxRichEditStringId.DifferentFirstPage, StringResources.RichEdit_DifferentFirstPage);
			AddString(ASPxRichEditStringId.ApplyTo, StringResources.RichEdit_ApplyTo);
			AddString(ASPxRichEditStringId.CancelButton, StringResources.RichEdit_CancelButton);
			AddString(ASPxRichEditStringId.OkButton, StringResources.RichEdit_OkButton);
			AddString(ASPxRichEditStringId.OpenButton, StringResources.RichEdit_OpenButton);
			AddString(ASPxRichEditStringId.SaveAsButton, StringResources.RichEdit_SaveAsButton);
			AddString(ASPxRichEditStringId.DownloadButton, StringResources.RichEdit_DownloadButton);
			AddString(ASPxRichEditStringId.SelectButton, StringResources.RichEdit_SelectButton);
			AddString(ASPxRichEditStringId.InsertButton, StringResources.RichEdit_InsertButton);
			AddString(ASPxRichEditStringId.ChangeButton, StringResources.RichEdit_ChangeButton);
			AddString(ASPxRichEditStringId.CloseButton, StringResources.RichEdit_CloseButton);
			AddString(ASPxRichEditStringId.InsertTable_TableSize, StringResources.RichEdit_InsertTable_TableSize);
			AddString(ASPxRichEditStringId.InsertTable_NumberOfColumns, StringResources.RichEdit_InsertTable_NumberOfColumns);
			AddString(ASPxRichEditStringId.InsertTable_NumberOfRows, StringResources.RichEdit_InsertTable_NumberOfRows);
			AddString(ASPxRichEditStringId.SplitCells_MergeCells, StringResources.RichEdit_SplitCells_MergeCells);
			AddString(ASPxRichEditStringId.InsertCells_ShiftCellsRight, StringResources.RichEdit_InsertCells_ShiftCellsRight);
			AddString(ASPxRichEditStringId.InsertCells_ShiftCellsDown, StringResources.RichEdit_InsertCells_ShiftCellsDown);
			AddString(ASPxRichEditStringId.InsertCells_InsertEntireRow, StringResources.RichEdit_InsertCells_InsertEntireRow);
			AddString(ASPxRichEditStringId.InsertCells_InsertEntireColumn, StringResources.RichEdit_InsertCells_InsertEntireColumn);
			AddString(ASPxRichEditStringId.DeleteCells_ShiftCellsLeft, StringResources.RichEdit_DeleteCells_ShiftCellsLeft);
			AddString(ASPxRichEditStringId.DeleteCells_ShiftCellsUp, StringResources.RichEdit_DeleteCells_ShiftCellsUp);
			AddString(ASPxRichEditStringId.DeleteCells_DeleteEntireRow, StringResources.RichEdit_DeleteCells_DeleteEntireRow);
			AddString(ASPxRichEditStringId.DeleteCells_DeleteEntireColumn, StringResources.RichEdit_DeleteCells_DeleteEntireColumn);
			AddString(ASPxRichEditStringId.InsertImage_FromWeb, StringResources.RichEdit_InsertImage_FromWeb);
			AddString(ASPxRichEditStringId.InsertImage_FromLocal, StringResources.RichEdit_InsertImage_FromLocal);
			AddString(ASPxRichEditStringId.InsertImage_EnterUrl, StringResources.RichEdit_InsertImage_EnterUrl);
			AddString(ASPxRichEditStringId.InsertImage_UploadInstructions, StringResources.RichEdit_InsertImage_UploadInstructions);
			AddString(ASPxRichEditStringId.InsertImage_ImagePreview, StringResources.RichEdit_InsertImage_ImagePreview);
			AddString(ASPxRichEditStringId.TableProperties_Size, StringResources.RichEdit_TableProperties_Size);
			AddString(ASPxRichEditStringId.TableProperties_Alignment, StringResources.RichEdit_TableProperties_Alignment);
			AddString(ASPxRichEditStringId.TableProperties_VerticalAlignment, StringResources.RichEdit_TableProperties_VerticalAlignment);
			AddString(ASPxRichEditStringId.TableProperties_MeasureIn, StringResources.RichEdit_TableProperties_MeasureIn);
			AddString(ASPxRichEditStringId.TableProperties_PreferredWidth, StringResources.RichEdit_TableProperties_PreferredWidth);
			AddString(ASPxRichEditStringId.TableProperties_IndentLeft, StringResources.RichEdit_TableProperties_IndentLeft);
			AddString(ASPxRichEditStringId.TableProperties_SpecifyHeight, StringResources.RichEdit_TableProperties_SpecifyHeight);
			AddString(ASPxRichEditStringId.TableProperties_RowHeight, StringResources.RichEdit_TableProperties_RowHeight);
			AddString(ASPxRichEditStringId.TableProperties_Table, StringResources.RichEdit_TableProperties_Table);
			AddString(ASPxRichEditStringId.TableProperties_Row, StringResources.RichEdit_TableProperties_Row);
			AddString(ASPxRichEditStringId.TableProperties_Column, StringResources.RichEdit_TableProperties_Column);
			AddString(ASPxRichEditStringId.TableProperties_Cell, StringResources.RichEdit_TableProperties_Cell);
			AddString(ASPxRichEditStringId.TableProperties_Options, StringResources.RichEdit_TableProperties_Options);
			AddString(ASPxRichEditStringId.TableProperties_BordersAndShading, StringResources.RichEdit_TableProperties_BordersAndShading);
			AddString(ASPxRichEditStringId.TableProperties_PreviousRow, StringResources.RichEdit_TableProperties_PreviousRow);
			AddString(ASPxRichEditStringId.TableProperties_NextRow, StringResources.RichEdit_TableProperties_NextRow);
			AddString(ASPxRichEditStringId.TableProperties_PreviousColumn, StringResources.RichEdit_TableProperties_PreviousColumn);
			AddString(ASPxRichEditStringId.TableProperties_NextColumn, StringResources.RichEdit_TableProperties_NextColumn);
			AddString(ASPxRichEditStringId.TableOptions_DefaultCellMargins, StringResources.RichEdit_TableOptions_DefaultCellMargins);
			AddString(ASPxRichEditStringId.TableOptions_DefaultCellSpacing, StringResources.RichEdit_TableOptions_DefaultCellSpacing);
			AddString(ASPxRichEditStringId.TableOptions_AllowSpacing, StringResources.RichEdit_TableOptions_AllowSpacing);
			AddString(ASPxRichEditStringId.TableOptions_AutoResize, StringResources.RichEdit_TableOptions_AutoResize);
			AddString(ASPxRichEditStringId.BorderShading_Borders, StringResources.RichEdit_BorderShading_Borders);
			AddString(ASPxRichEditStringId.BorderShading_Shading, StringResources.RichEdit_BorderShading_Shading);
			AddString(ASPxRichEditStringId.BorderShading_Fill, StringResources.RichEdit_BorderShading_Fill);
			AddString(ASPxRichEditStringId.BorderShading_BordersDescription, StringResources.RichEdit_BorderShading_BordersDescription);
			AddString(ASPxRichEditStringId.BorderShading_BordersNone, StringResources.RichEdit_BorderShading_BordersNone);
			AddString(ASPxRichEditStringId.BorderShading_BordersBox, StringResources.RichEdit_BorderShading_BordersBox);
			AddString(ASPxRichEditStringId.BorderShading_BordersAll, StringResources.RichEdit_BorderShading_BordersAll);
			AddString(ASPxRichEditStringId.BorderShading_BordersGrid, StringResources.RichEdit_BorderShading_BordersGrid);
			AddString(ASPxRichEditStringId.BorderShading_BordersCustom, StringResources.RichEdit_BorderShading_BordersCustom);
			AddString(ASPxRichEditStringId.CellOptions_CellMargins, StringResources.RichEdit_CellOptions_CellMargins);
			AddString(ASPxRichEditStringId.CellOptions_SameAsTable, StringResources.RichEdit_CellOptions_SameAsTable);
			AddString(ASPxRichEditStringId.CellOptions_WrapText, StringResources.RichEdit_CellOptions_WrapText);
			AddString(ASPxRichEditStringId.Columns_Presets, StringResources.RichEdit_Columns_Presets);
			AddString(ASPxRichEditStringId.Columns_NumberOfColumns, StringResources.RichEdit_Columns_NumberOfColumns);
			AddString(ASPxRichEditStringId.Columns_WidthAndSpacing, StringResources.RichEdit_WidthAndSpacing);
			AddString(ASPxRichEditStringId.Columns_EqualWidth, StringResources.RichEdit_Columns_EqualWidth);
			AddString(ASPxRichEditStringId.Columns_Col, StringResources.RichEdit_Columns_Col);
			AddString(ASPxRichEditStringId.Columns_Width, StringResources.RichEdit_Columns_Width);
			AddString(ASPxRichEditStringId.Columns_Spacing, StringResources.RichEdit_Columns_Spacing);
			AddString(ASPxRichEditStringId.PageFile, StringResources.RichEdit_PageFile);
			AddString(ASPxRichEditStringId.PageHome, StringResources.RichEdit_PageHome);
			AddString(ASPxRichEditStringId.PageInsert, StringResources.RichEdit_PageInsert);
			AddString(ASPxRichEditStringId.PagePageLayout, StringResources.RichEdit_PagePageLayout);
			AddString(ASPxRichEditStringId.PageReferences, StringResources.RichEdit_PageReferences);
			AddString(ASPxRichEditStringId.PageMailings, StringResources.RichEdit_PageMailings);
			AddString(ASPxRichEditStringId.PageReview, StringResources.RichEdit_PageReview);
			AddString(ASPxRichEditStringId.PageView, StringResources.RichEdit_PageView);
			AddString(ASPxRichEditStringId.PageHeaderAndFooter, StringResources.RichEdit_PageHeaderAndFooter);
			AddString(ASPxRichEditStringId.PageTableDesign, StringResources.RichEdit_PageTableDesign);
			AddString(ASPxRichEditStringId.PageTableLayout, StringResources.RichEdit_PageTableLayout);
			AddString(ASPxRichEditStringId.PagePicture, StringResources.RichEdit_PagePicture);
			AddString(ASPxRichEditStringId.GroupUndo, StringResources.RichEdit_GroupUndo);
			AddString(ASPxRichEditStringId.GroupFont, StringResources.RichEdit_GroupFont);
			AddString(ASPxRichEditStringId.GroupParagraph, StringResources.RichEdit_GroupParagraph);
			AddString(ASPxRichEditStringId.GroupClipboard, StringResources.RichEdit_GroupClipboard);
			AddString(ASPxRichEditStringId.GroupEditing, StringResources.RichEdit_GroupEditing);
			AddString(ASPxRichEditStringId.GroupCommon, StringResources.RichEdit_GroupCommon);
			AddString(ASPxRichEditStringId.GroupStyles, StringResources.RichEdit_GroupStyles);
			AddString(ASPxRichEditStringId.GroupZoom, StringResources.RichEdit_GroupZoom);
			AddString(ASPxRichEditStringId.GroupShow, StringResources.RichEdit_GroupShow);
			AddString(ASPxRichEditStringId.GroupIllustrations, StringResources.RichEdit_GroupIllustrations);
			AddString(ASPxRichEditStringId.GroupText, StringResources.RichEdit_GroupText);
			AddString(ASPxRichEditStringId.GroupTables, StringResources.RichEdit_GroupTables);
			AddString(ASPxRichEditStringId.GroupSymbols, StringResources.RichEdit_GroupSymbols);
			AddString(ASPxRichEditStringId.GroupLinks, StringResources.RichEdit_GroupLinks);
			AddString(ASPxRichEditStringId.GroupPages, StringResources.RichEdit_GroupPages);
			AddString(ASPxRichEditStringId.GroupHeaderFooter, StringResources.RichEdit_GroupHeaderFooter);
			AddString(ASPxRichEditStringId.GroupHeaderFooterToolsDesignClose, StringResources.RichEdit_GroupHeaderFooterToolsDesignClose);
			AddString(ASPxRichEditStringId.GroupHeaderFooterToolsDesignNavigation, StringResources.RichEdit_GroupHeaderFooterToolsDesignNavigation);
			AddString(ASPxRichEditStringId.GroupMailMerge, StringResources.RichEdit_GroupMailMerge);
			AddString(ASPxRichEditStringId.GroupDocumentViews, StringResources.RichEdit_GroupDocumentViews);
			AddString(ASPxRichEditStringId.GroupHeaderFooterToolsDesignOptions, StringResources.RichEdit_GroupHeaderFooterToolsDesignOptions);
			AddString(ASPxRichEditStringId.GroupPageSetup, StringResources.RichEdit_GroupPageSetup);
			AddString(ASPxRichEditStringId.GroupDocumentProtection, StringResources.RichEdit_GroupDocumentProtection);
			AddString(ASPxRichEditStringId.GroupDocumentProofing, StringResources.RichEdit_GroupDocumentProofing);
			AddString(ASPxRichEditStringId.GroupDocumentComment, StringResources.RichEdit_GroupDocumentComment);
			AddString(ASPxRichEditStringId.GroupDocumentTracking, StringResources.RichEdit_GroupDocumentTracking);
			AddString(ASPxRichEditStringId.GroupTableOfContents, StringResources.RichEdit_GroupTableOfContents);
			AddString(ASPxRichEditStringId.GroupFloatingPictureToolsArrange, StringResources.RichEdit_GroupFloatingPictureToolsArrange);
			AddString(ASPxRichEditStringId.GroupFloatingPictureToolsShapeStyles, StringResources.RichEdit_GroupFloatingPictureToolsShapeStyles);
			AddString(ASPxRichEditStringId.GroupCaptions, StringResources.RichEdit_GroupCaptions);
			AddString(ASPxRichEditStringId.GroupPageBackground, StringResources.RichEdit_GroupPageBackground);
			AddString(ASPxRichEditStringId.GroupDocumentLanguage, StringResources.RichEdit_GroupDocumentLanguage);
			AddString(ASPxRichEditStringId.GroupView, StringResources.RichEdit_GroupView);
			AddString(ASPxRichEditStringId.GroupTableToolsDesignTableStyleOptions, StringResources.RichEdit_GroupTableToolsDesignTableStyleOptions);
			AddString(ASPxRichEditStringId.GroupTableToolsDesignTableStyles, StringResources.RichEdit_GroupTableToolsDesignTableStyles);
			AddString(ASPxRichEditStringId.GroupTableToolsDesignBordersAndShadings, StringResources.RichEdit_GroupTableToolsDesignBordersAndShadings);
			AddString(ASPxRichEditStringId.GroupTableToolsLayoutTable, StringResources.RichEdit_GroupTableToolsLayoutTable);
			AddString(ASPxRichEditStringId.GroupTableToolsLayoutRowsAndColumns, StringResources.RichEdit_GroupTableToolsLayoutRowsAndColumns);
			AddString(ASPxRichEditStringId.GroupTableToolsLayoutMerge, StringResources.RichEdit_GroupTableToolsLayoutMerge);
			AddString(ASPxRichEditStringId.GroupTableToolsLayoutCellSize, StringResources.RichEdit_GroupTableToolsLayoutCellSize);
			AddString(ASPxRichEditStringId.GroupTableToolsLayoutAlignment, StringResources.RichEdit_GroupTableToolsLayoutAlignment);
			AddString(ASPxRichEditStringId.RequiredFieldError, StringResources.RichEdit_RequiredFieldError);
			AddString(ASPxRichEditStringId.MenuCmd_ToggleFullScreen, StringResources.RichEdit_MenuCmd_ToggleFullScreen);
			AddString(ASPxRichEditStringId.MenuCmd_ToggleFullScreenDescription, StringResources.RichEdit_MenuCmd_ToggleFullScreenDescription);
			AddString(ASPxRichEditStringId.SaveAsFileTitle, StringResources.RichEdit_DialogTitle_SaveAsFile);
			AddString(ASPxRichEditStringId.OpenFileTitle, StringResources.RichEdit_DialogTitle_OpenFile);
			AddString(ASPxRichEditStringId.FontTitle, StringResources.RichEdit_DialogTitle_Font);
			AddString(ASPxRichEditStringId.ParagraphTitle, StringResources.RichEdit_DialogTitle_Paragraph);
			AddString(ASPxRichEditStringId.PageSetupTitle, StringResources.RichEdit_DialogTitle_PageSetup);
			AddString(ASPxRichEditStringId.ColumnsTitle, StringResources.RichEdit_DialogTitle_Columns);
			AddString(ASPxRichEditStringId.InsertTableTitle, StringResources.RichEdit_DialogTitle_InsertTable);
			AddString(ASPxRichEditStringId.InsertTableCellsTitle, StringResources.RichEdit_DialogTitle_InsertTableCells);
			AddString(ASPxRichEditStringId.DeleteTableCellsTitle, StringResources.RichEdit_DialogTitle_DeleteTableCells);
			AddString(ASPxRichEditStringId.SplitTableCellsTitle, StringResources.RichEdit_DialogTitle_SplitTableCells);
			AddString(ASPxRichEditStringId.TablePropertiesTitle, StringResources.RichEdit_DialogTitle_TableProperties);
			AddString(ASPxRichEditStringId.BorderShadingTitle, StringResources.RichEdit_DialogTitle_BorderShading);
			AddString(ASPxRichEditStringId.InsertImageTitle, StringResources.RichEdit_DialogTitle_InsertImage);
			AddString(ASPxRichEditStringId.ErrorTitle, StringResources.RichEdit_DialogTitle_Error);
			AddString(ASPxRichEditStringId.InsertMergeFieldTitle, StringResources.RichEdit_DialogTitle_InsertMergeField);
			AddString(ASPxRichEditStringId.ExportRangeTitle, StringResources.RichEdit_DialogTitle_ExportRange);
			AddString(ASPxRichEditStringId.BookmarkTitle, StringResources.RichEdit_DialogTitle_BookmarkTitle);
			AddString(ASPxRichEditStringId.RulerMarginLeftTitle, StringResources.RichEdit_RulerMarginLeftTitle);
			AddString(ASPxRichEditStringId.RulerMarginRightTitle, StringResources.RichEdit_RulerMarginRightTitle);
			AddString(ASPxRichEditStringId.RulerFirstLineIdentTitle, StringResources.RichEdit_RulerFirstLineIdentTitle);
			AddString(ASPxRichEditStringId.RulerHangingIdentTitle, StringResources.RichEdit_RulerHangingIdentTitle);
			AddString(ASPxRichEditStringId.RulerLeftIdentTitle, StringResources.RichEdit_RulerLeftIdentTitle);
			AddString(ASPxRichEditStringId.RulerRightIdentTitle, StringResources.RichEdit_RulerRightIdentTitle);
			AddString(ASPxRichEditStringId.RulerLeftTabTitle, StringResources.RichEdit_RulerLeftTabTitle);
			AddString(ASPxRichEditStringId.RulerRightTabTitle, StringResources.RichEdit_RulerRightTabTitle);
			AddString(ASPxRichEditStringId.RulerCenterTabTitle, StringResources.RichEdit_RulerCenterTabTitle);
			AddString(ASPxRichEditStringId.RulerDecimalTabTitle, StringResources.RichEdit_RulerDecimalTabTitle);
			AddString(ASPxRichEditStringId.MarginsNormal, StringResources.RichEdit_MarginsNormal);
			AddString(ASPxRichEditStringId.MarginsNarrow, StringResources.RichEdit_MarginsNarrow);
			AddString(ASPxRichEditStringId.MarginsModerate, StringResources.RichEdit_MarginsModerate);
			AddString(ASPxRichEditStringId.MarginsWide, StringResources.RichEdit_MarginsWide);
			AddString(ASPxRichEditStringId.ModelIsChangedError, StringResources.RichEdit_ModelIsChangedError);
			AddString(ASPxRichEditStringId.SessionHasExpiredError, StringResources.RichEdit_SessionHasExpiredError);
			AddString(ASPxRichEditStringId.OpeningAndOverstoreImpossibleError, StringResources.RichEdit_OpeningAndOverstoreImpossibleError);
			AddString(ASPxRichEditStringId.ClipboardAccessDeniedError, StringResources.RichEdit_ClipboardAccessDeniedError);
			AddString(ASPxRichEditStringId.ClipboardAccessDeniedErrorTouch, StringResources.RichEdit_ClipboardAccessDeniedErrorTouch);
			AddString(ASPxRichEditStringId.InnerExceptionsError, StringResources.RichEdit_InnerExceptionsError);
			AddString(ASPxRichEditStringId.AuthExceptionsError, StringResources.RichEdit_AuthExceptionsError);
			AddString(ASPxRichEditStringId.ConfirmOnLosingChanges, StringResources.RichEdit_ConfirmOnLosingChanges);
			AddString(ASPxRichEditStringId.RestartNumbering, StringResources.RichEdit_RestartNumbering);
			AddString(ASPxRichEditStringId.ContinueNumbering, StringResources.RichEdit_ContinueNumbering);
			AddString(ASPxRichEditStringId.UpdateField, StringResources.RichEdit_UpdateField);
			AddString(ASPxRichEditStringId.ToggleFieldCodes, StringResources.RichEdit_ToggleFieldCodes);
			AddString(ASPxRichEditStringId.OpenHyperlink, StringResources.RichEdit_OpenHyperlink);
			AddString(ASPxRichEditStringId.EditHyperlink, StringResources.RichEdit_EditHyperlink);
			AddString(ASPxRichEditStringId.RemoveHyperlink, StringResources.RichEdit_RemoveHyperlink);
			AddString(ASPxRichEditStringId.CantOpenDocumentError, StringResources.RichEdit_CantOpenDocumentError);
			AddString(ASPxRichEditStringId.CantSaveDocumentError, StringResources.RichEdit_CantSaveDocumentError);
			AddString(ASPxRichEditStringId.DocVariableExceptionError, StringResources.RichEdit_DocVariableExceptionError);
			AddString(ASPxRichEditStringId.MenuCmd_CreateField, StringResources.RichEdit_CreateField);
			AddString(ASPxRichEditStringId.MenuCmd_CreateFieldDescription, StringResources.RichEdit_CreateFieldDescription);
			AddString(ASPxRichEditStringId.MenuCmd_UpdateAllFields, StringResources.RichEdit_UpdateAllFields);
			AddString(ASPxRichEditStringId.MenuCmd_UpdateAllFieldsDescription, StringResources.RichEdit_UpdateAllFieldsDescription);
			AddString(ASPxRichEditStringId.ParagraphStyles, StringResources.RichEdit_ParagraphStyles);
			AddString(ASPxRichEditStringId.CharacterStyles, StringResources.RichEdit_CharacterStyles);
			AddString(ASPxRichEditStringId.BulletedAndNumberingTitle, StringResources.RichEdit_BulletedAndNumberingTitle);
			AddString(ASPxRichEditStringId.CustomizeNumberedListTitle, StringResources.RichEdit_CustomizeNumberedListTitle);
			AddString(ASPxRichEditStringId.CustomizeBulletedListTitle, StringResources.RichEdit_CustomizeBulletedListTitle);
			AddString(ASPxRichEditStringId.CustomizeOutlineNumberedTitle, StringResources.RichEdit_CustomizeOutlineNumberedTitle);
			AddString(ASPxRichEditStringId.HyperlinkTitle, StringResources.RichEdit_HyperlinkTitle);
			AddString(ASPxRichEditStringId.TabsTitle, StringResources.RichEdit_TabsTitle);
			AddString(ASPxRichEditStringId.SymbolsTitle, StringResources.RichEdit_SymbolsTitle);
			AddString(ASPxRichEditStringId.OtherLabels_None, StringResources.RichEdit_OtherLabels_None);
			AddString(ASPxRichEditStringId.OtherLabels_All, StringResources.RichEdit_OtherLabels_All);
			AddString(ASPxRichEditStringId.Numbering_Character, StringResources.RichEdit_Numbering_Character);
			AddString(ASPxRichEditStringId.Numbering_Font, StringResources.RichEdit_Numbering_Font);
			AddString(ASPxRichEditStringId.Numbering_BulletCharacter, StringResources.RichEdit_Numbering_BulletCharacter);
			AddString(ASPxRichEditStringId.Numbering_BulletPosition, StringResources.RichEdit_Numbering_BulletPosition);
			AddString(ASPxRichEditStringId.Numbering_TextPosition, StringResources.RichEdit_Numbering_TextPosition);
			AddString(ASPxRichEditStringId.Numbering_AlignedAt, StringResources.RichEdit_Numbering_AlignedAt);
			AddString(ASPxRichEditStringId.Numbering_IndentAt, StringResources.RichEdit_Numbering_IndentAt);
			AddString(ASPxRichEditStringId.Numbering_StartAt, StringResources.RichEdit_Numbering_StartAt);
			AddString(ASPxRichEditStringId.Numbering_NumberFormat, StringResources.RichEdit_Numbering_NumberFormat);
			AddString(ASPxRichEditStringId.Numbering_NumberStyle, StringResources.RichEdit_Numbering_NumberStyle);
			AddString(ASPxRichEditStringId.Numbering_NumberPosition, StringResources.RichEdit_Numbering_NumberPosition);
			AddString(ASPxRichEditStringId.Numbering_Left, StringResources.RichEdit_Numbering_Left);
			AddString(ASPxRichEditStringId.Numbering_Center, StringResources.RichEdit_Numbering_Center);
			AddString(ASPxRichEditStringId.Numbering_Right, StringResources.RichEdit_Numbering_Right);
			AddString(ASPxRichEditStringId.Numbering_FollowNumberWith, StringResources.RichEdit_Numbering_FollowNumberWith);
			AddString(ASPxRichEditStringId.Numbering_Level, StringResources.RichEdit_Numbering_Level);
			AddString(ASPxRichEditStringId.Numbering_TabCharacter, StringResources.RichEdit_Numbering_TabCharacter);
			AddString(ASPxRichEditStringId.Numbering_Space, StringResources.RichEdit_Numbering_Space);
			AddString(ASPxRichEditStringId.Numbering_Nothing, StringResources.RichEdit_Numbering_Nothing);
			AddString(ASPxRichEditStringId.Numbering_Customize, StringResources.RichEdit_Numbering_Customize);
			AddString(ASPxRichEditStringId.Numbering_Bulleted, StringResources.RichEdit_Numbering_Bulleted);
			AddString(ASPxRichEditStringId.Numbering_Numbered, StringResources.RichEdit_Numbering_Numbered);
			AddString(ASPxRichEditStringId.Numbering_OutlineNumbered, StringResources.RichEdit_Numbering_OutlineNumbered);
			AddString(ASPxRichEditStringId.Hyperlink_EmailAddress, StringResources.RichEdit_Hyperlink_EmailAddress);
			AddString(ASPxRichEditStringId.Hyperlink_Url, StringResources.RichEdit_Hyperlink_Url);
			AddString(ASPxRichEditStringId.Hyperlink_Bookmark, StringResources.RichEdit_Hyperlink_Bookmark);
			AddString(ASPxRichEditStringId.Hyperlink_WebPage, StringResources.RichEdit_Hyperlink_WebPage);
			AddString(ASPxRichEditStringId.Hyperlink_PlaceInThisDocument, StringResources.RichEdit_Hyperlink_PlaceInThisDocument);
			AddString(ASPxRichEditStringId.Hyperlink_EmailTo, StringResources.RichEdit_Hyperlink_EmailTo);
			AddString(ASPxRichEditStringId.Hyperlink_Subject, StringResources.RichEdit_Hyperlink_Subject);
			AddString(ASPxRichEditStringId.Hyperlink_DisplayProperties, StringResources.RichEdit_Hyperlink_DisplayProperties);
			AddString(ASPxRichEditStringId.Hyperlink_Text, StringResources.RichEdit_Hyperlink_Text);
			AddString(ASPxRichEditStringId.Hyperlink_ToolTip, StringResources.RichEdit_Hyperlink_ToolTip);
			AddString(ASPxRichEditStringId.Hyperlink_InvalidEmail, StringResources.RichEdit_Hyperlink_InvalidEmail);
			AddString(ASPxRichEditStringId.Tabs_Set, StringResources.RichEdit_Tabs_Set);
			AddString(ASPxRichEditStringId.Tabs_Clear, StringResources.RichEdit_Tabs_Clear);
			AddString(ASPxRichEditStringId.Tabs_ClearAll, StringResources.RichEdit_Tabs_ClearAll);
			AddString(ASPxRichEditStringId.Tabs_TabStopPosition, StringResources.RichEdit_Tabs_TabStopPosition);
			AddString(ASPxRichEditStringId.Tabs_DefaultTabStops, StringResources.RichEdit_Tabs_DefaultTabStops);
			AddString(ASPxRichEditStringId.Tabs_TabStopsToBeCleared, StringResources.RichEdit_Tabs_TabStopsToBeCleared);
			AddString(ASPxRichEditStringId.Tabs_Alignment, StringResources.RichEdit_Tabs_Alignment);
			AddString(ASPxRichEditStringId.Tabs_Leader, StringResources.RichEdit_Tabs_Leader);
			AddString(ASPxRichEditStringId.Tabs_Left, StringResources.RichEdit_Tabs_Left);
			AddString(ASPxRichEditStringId.Tabs_Center, StringResources.RichEdit_Tabs_Center);
			AddString(ASPxRichEditStringId.Tabs_Right, StringResources.RichEdit_Tabs_Right);
			AddString(ASPxRichEditStringId.Tabs_Decimal, StringResources.RichEdit_Tabs_Decimal);
			AddString(ASPxRichEditStringId.Tabs_None, StringResources.RichEdit_Tabs_None);
			AddString(ASPxRichEditStringId.Tabs_Hyphens, StringResources.RichEdit_Tabs_Hyphens);
			AddString(ASPxRichEditStringId.Tabs_EqualSign, StringResources.RichEdit_Tabs_EqualSign);
			AddString(ASPxRichEditStringId.Tabs_Dots, StringResources.RichEdit_Tabs_Dots);
			AddString(ASPxRichEditStringId.Tabs_Underline, StringResources.RichEdit_Tabs_Underline);
			AddString(ASPxRichEditStringId.Tabs_MiddleDots, StringResources.RichEdit_Tabs_MiddleDots);
			AddString(ASPxRichEditStringId.Tabs_ThickLine, StringResources.RichEdit_Tabs_ThickLine);
			AddString(ASPxRichEditStringId.FinishMerge_From, StringResources.RichEdit_FinishMerge_From);
			AddString(ASPxRichEditStringId.FinishMerge_Count, StringResources.RichEdit_FinishMerge_Count);
			AddString(ASPxRichEditStringId.FinishMerge_MergeMode, StringResources.RichEdit_FinishMerge_MergeMode);
			AddString(ASPxRichEditStringId.FinishMerge_AllRecord, StringResources.RichEdit_FinishMerge_AllRecord);
			AddString(ASPxRichEditStringId.FinishMerge_CurrentRecord, StringResources.RichEdit_FinishMerge_CurrentRecord);
			AddString(ASPxRichEditStringId.FinishMerge_Range, StringResources.RichEdit_FinishMerge_Range);
			AddString(ASPxRichEditStringId.FinishMerge_NewParagraph, StringResources.RichEdit_FinishMerge_NewParagraph);
			AddString(ASPxRichEditStringId.FinishMerge_NewSection, StringResources.RichEdit_FinishMerge_NewSection);
			AddString(ASPxRichEditStringId.FinishMerge_JoinTables, StringResources.RichEdit_FinishMerge_JoinTables);
			AddString(ASPxRichEditStringId.InsertMergeField_Fields, StringResources.RichEdit_InsertMergeField_Fields);
			AddString(ASPxRichEditStringId.Bookmarks_BookmarkName, StringResources.RichEdit_Bookmarks_BookmarkName);
			AddString(ASPxRichEditStringId.Bookmarks_SortBy, StringResources.RichEdit_Bookmarks_SortBy);
			AddString(ASPxRichEditStringId.Bookmarks_Name, StringResources.RichEdit_Bookmarks_Name);
			AddString(ASPxRichEditStringId.Bookmarks_Location, StringResources.RichEdit_Bookmarks_Location);
			AddString(ASPxRichEditStringId.Bookmarks_Add, StringResources.RichEdit_Bookmarks_Add);
			AddString(ASPxRichEditStringId.Bookmarks_Delete, StringResources.RichEdit_Bookmarks_Delete);
			AddString(ASPxRichEditStringId.Bookmarks_GoTo, StringResources.RichEdit_Bookmarks_GoTo);
			AddString(ASPxRichEditStringId.MenuCmd_MakeTextSentenceCase, StringResources.RichEdit_MenuCmd_MakeTextSentenceCase);
			AddString(ASPxRichEditStringId.MenuCmd_MakeTextSentenceCaseDescription, StringResources.RichEdit_MenuCmd_MakeTextSentenceCaseDescription);
			AddString(ASPxRichEditStringId.MenuCmd_ShowCellOptionsForm, StringResources.RichEdit_MenuCmd_ShowCellOptionsForm);
			AddString(ASPxRichEditStringId.MenuCmd_ShowCellOptionsFormDescription, StringResources.RichEdit_MenuCmd_ShowCellOptionsFormDescription);
		}
	}
}
