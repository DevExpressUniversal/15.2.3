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
namespace DevExpress.XtraReports.Localization {
	#region enum ReportStringId
	public enum ReportStringId {
		Dlg_SaveFile_Title,
		Msg_InvalidDrillDownControl,
		Msg_WarningFontNameCantBeEmpty,
		Msg_FileNotFound,
		Msg_WrongReportClassName,
		Msg_CreateReportInstance,
		Msg_FileCorrupted,
		Msg_FileContentCorrupted,
		Msg_IncorrectArgument,
		Msg_InvalidMethodCall,
		Msg_ScriptError,
		Msg_ScriptExecutionError,
		Msg_ScriptCodeIsNotCorrect,
		Msg_InvalidReportSource,
		Msg_IncorrectBandType,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'PreviewLocalizer.Msg_InvPropName' member")]
		Msg_InvPropName,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.Msg_CantFitBarcodeToControlBounds' instead")]
		Msg_CantFitBarcodeToControlBounds,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.Msg_InvalidBarcodeText' instead")]
		Msg_InvalidBarcodeText,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.Msg_InvalidBarcodeTextFormat' instead")]
		Msg_InvalidBarcodeTextFormat,
		Msg_CreateSomeInstance,
		Msg_DontSupportMulticolumn,
		Msg_FillDataError,
		Msg_PlacingXrTocIntoIncorrectContainer,
		Msg_NoBookmarksWereFoundInReportForXrToc,
		Msg_InvalidLeaderSymbolForXrTocLevel,
		Msg_InvalidXrTocInstance,
		Msg_InvalidXrTocInstanceInBand,
		Msg_ApplyChangesQuestion,
		[Obsolete("You should use the 'Msg_CyclicBookmarks' member instead")]
		Msg_CyclicBoormarks,
		Msg_CyclicBookmarks,
		Msg_LargeText,
		Msg_ScriptingPermissionErrorMessage,
		Msg_ReportImporting,
		Msg_IncorrectPadding,
		Msg_WarningControlsAreOverlapped,
		Msg_WarningControlsAreOutOfMargin,
		Msg_WarningUnsavedReports,
		Msg_ShapeRotationToolTip,
		Msg_WarningRemoveCalculatedFields,
		Msg_WarningRemoveParameters,
		Msg_WarningRemoveStyles,
		Msg_WarningRemoveFormattingRules,
		[Obsolete("You should use the 'ErrorTitle' instead")]
		Msg_ScriptingErrorTitle,
		Msg_ErrorTitle,
		Msg_SerializationErrorTitle,
		Msg_InvalidExpression,
		Msg_InvalidExpressionEx,
		Msg_NoCustomFunction,
		Msg_InvalidCondition,
		Msg_GroupSortWarning,
		Msg_GroupSortNoDataSource,
		Msg_NotEnoughMemoryToPaint,
		Msg_Caption,
		Msg_ParameterBindingValueTypeMismatch,
		Cmd_Commands,
		Cmd_InsertDetailReport,
		Cmd_InsertUnboundDetailReport,
		Cmd_ViewCode,
		Cmd_BringToFront,
		Cmd_SendToBack,
		Cmd_AlignToGrid,
		Cmd_TopMargin,
		Cmd_BottomMargin,
		Cmd_ReportHeader,
		Cmd_ReportFooter,
		Cmd_PageHeader,
		Cmd_PageFooter,
		Cmd_GroupHeader,
		Cmd_GroupFooter,
		Cmd_Detail,
		Cmd_DetailReport,
		Cmd_RtfClear,
		Cmd_RtfLoad,
		Cmd_TableInsert,
		Cmd_TableInsertRowAbove,
		Cmd_TableInsertRowBelow,
		Cmd_TableInsertColumnToLeft,
		Cmd_TableInsertColumnToRight,
		Cmd_TableInsertCell,
		Cmd_TableDelete,
		Cmd_TableDeleteRow,
		Cmd_TableDeleteColumn,
		Cmd_TableDeleteCell,
		Cmd_TableConvertToLabels,
		Cmd_Cut,
		Cmd_Copy,
		Cmd_Paste,
		Cmd_Delete,
		Cmd_Properties,
		Cmd_InsertBand,
		Cmd_AddSubBand,
		Cmd_BandMoveUp,
		Cmd_BandMoveDown,
		Cmd_AddStyle,
		Cmd_EditStyles,
		Cmd_DeleteStyle,
		Cmd_PurgeStyles,
		Cmd_ClearStyles,
		Cmd_SelectControlsWithStyle,
		Cmd_AssignStyleToXRControl,
		Cmd_AssignOddStyleToXRControl,
		Cmd_AssignEvenStyleToXRControl,
		Cmd_AddFormattingRule,
		Cmd_EditFormattingRules,
		Cmd_DeleteFormattingRule,
		Cmd_PurgeFormattingRules,
		Cmd_ClearFormattingRules,
		Cmd_SelectControlsWithFormattingRule,
		CatLayout,
		CatAppearance,
		CatData,
		CatBehavior,
		CatNavigation,
		CatPageSettings,
		CatUserDesigner,
		CatDesign,
		CatParameters,
		CatStructure,
		CatPrinting,
		CatElements,
		CatOptions,
		BandDsg_QuantityPerPage,
		BandDsg_QuantityPerReport,
		UD_ReportDesigner,
		UD_Msg_ReportChanged,
		UD_Msg_MdiReportChanged,
		UD_TTip_FileOpen,
		UD_TTip_FileSave,
		UD_Msg_ReportSourceUrlNotFound,
		UD_TTip_EditCut,
		UD_TTip_EditCopy,
		UD_TTip_EditPaste,
		UD_TTip_Undo,
		UD_TTip_Redo,
		UD_TTip_AlignToGrid,
		UD_TTip_AlignLeft,
		UD_TTip_AlignVerticalCenters,
		UD_TTip_AlignRight,
		UD_TTip_AlignTop,
		UD_TTip_AlignHorizontalCenters,
		UD_TTip_AlignBottom,
		UD_TTip_SizeToControlWidth,
		UD_TTip_SizeToGrid,
		UD_TTip_SizeToControlHeight,
		UD_TTip_SizeToControl,
		UD_TTip_HorizSpaceMakeEqual,
		UD_TTip_HorizSpaceIncrease,
		UD_TTip_HorizSpaceDecrease,
		UD_TTip_HorizSpaceConcatenate,
		UD_TTip_VertSpaceMakeEqual,
		UD_TTip_VertSpaceIncrease,
		UD_TTip_VertSpaceDecrease,
		UD_TTip_VertSpaceConcatenate,
		UD_TTip_CenterHorizontally,
		UD_TTip_CenterVertically,
		UD_TTip_BringToFront,
		UD_TTip_SendToBack,
		UD_TTip_FormatBold,
		UD_TTip_FormatItalic,
		UD_TTip_FormatUnderline,
		UD_TTip_FormatAlignLeft,
		UD_TTip_FormatCenter,
		UD_TTip_FormatAlignRight,
		UD_TTip_FormatFontName,
		UD_TTip_FormatFontSize,
		UD_TTip_FormatForeColor,
		UD_TTip_FormatBackColor,
		UD_TTip_FormatJustify,
		UD_TTip_ItemDescription,
		UD_TTip_TableDescription,
		UD_TTip_DataMemberDescription,
		UD_FormCaption,
		UD_XtraReportsToolboxCategoryName,
		UD_XtraReportsPointerItemCaption,
		Verb_EditBands,
		Verb_EditGroupFields,
		Verb_Import,
		Verb_Export,
		Verb_Save,
		Verb_About,
		Verb_RTFClear,
		Verb_RTFLoad,
		Verb_FormatString,
		Verb_SummaryWizard,
		Verb_ReportWizard,
		Verb_Insert,
		Verb_Delete,
		Verb_Bind,
		Verb_EditText,
		Verb_AddFieldToArea,
		Verb_RunDesigner,
		Verb_EditBindings,
		Verb_LoadReportTemplate,
		BCForm_Lbl_Property,
		BCForm_Lbl_Binding,
		FRSForm_Caption,
		FRSForm_Msg_NoRuleSelected,
		FRSForm_Msg_MoreThanOneRule,
		FRSForm_TTip_AddRule,
		FRSForm_TTip_RemoveRule,
		FRSForm_TTip_ClearRules,
		FRSForm_TTip_PurgeRules,
		SSForm_Caption,
		SSForm_Btn_Close,
		SSForm_Msg_NoStyleSelected,
		SSForm_Msg_MoreThanOneStyle,
		SSForm_Msg_SelectedStylesText,
		SSForm_Msg_StyleSheetError,
		SSForm_Msg_InvalidFileFormat,
		SSForm_Msg_StyleNamePreviewPostfix,
		SSForm_Msg_FileFilter,
		SSForm_TTip_AddStyle,
		SSForm_TTip_RemoveStyle,
		SSForm_TTip_ClearStyles,
		SSForm_TTip_PurgeStyles,
		SSForm_TTip_SaveStyles,
		SSForm_TTip_LoadStyles,
		SR_Side_Margins,
		SR_Top_Margin,
		SR_Vertical_Pitch,
		SR_Horizontal_Pitch,
		SR_Width,
		SR_Height,
		SR_Number_Down,
		SR_Number_Across,
		ScriptEditor_ErrorDescription,
		ScriptEditor_ErrorLine,
		ScriptEditor_ErrorColumn,
		ScriptEditor_Validate,
		ScriptEditor_ScriptsAreValid,
		ScriptEditor_ScriptHasBeenChanged,
		ScriptEditor_ClickValidate,
		ScriptEditor_NewString,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.Msg_SearchDialogFinishedSearching' instead")]
		FindForm_Msg_FinishedSearching,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.Msg_SearchDialogTotalFound' instead")]
		FindForm_Msg_TotalFound,
		RepTabCtl_HtmlView,
		RepTabCtl_Preview,
		RepTabCtl_Designer,
		RepTabCtl_Scripts,
		RepTabCtl_ReportStatus,
		PanelDesignMsg,
		MultiColumnDesignMsg1,
		MultiColumnDesignMsg2,
		UD_Group_File,
		UD_Group_Edit,
		UD_Group_View,
		UD_Group_Format,
		UD_Group_Window,
		UD_Group_Font,
		UD_Group_Justify,
		UD_Group_Align,
		UD_Group_MakeSameSize,
		UD_Group_HorizontalSpacing,
		UD_Group_VerticalSpacing,
		UD_Group_CenterInForm,
		UD_Group_Order,
		UD_Group_ToolbarsList,
		UD_Group_DockPanelsList,
		UD_Group_TabButtonsList,
		UD_Capt_MainMenuName,
		UD_Capt_ToolbarName,
		UD_Capt_LayoutToolbarName,
		UD_Capt_FormattingToolbarName,
		UD_Capt_StatusBarName,
		UD_Capt_ZoomToolbarName,
		UD_Capt_NewReport,
		UD_Capt_NewWizardReport,
		UD_Capt_OpenFile,
		UD_Capt_SaveFile,
		UD_Capt_SaveFileAs,
		UD_Capt_SaveAll,
		UD_Capt_Exit,
		UD_Capt_TabbedInterface,
		UD_Capt_MdiCascade,
		UD_Capt_MdiTileHorizontal,
		UD_Capt_MdiTileVertical,
		UD_Capt_Close,
		UD_Capt_Cut,
		UD_Capt_Copy,
		UD_Capt_Paste,
		UD_Capt_Delete,
		UD_Capt_SelectAll,
		UD_Capt_Undo,
		UD_Capt_Redo,
		UD_Capt_ForegroundColor,
		UD_Capt_BackGroundColor,
		UD_Capt_FontBold,
		UD_Capt_FontItalic,
		UD_Capt_FontUnderline,
		UD_Capt_JustifyLeft,
		UD_Capt_JustifyCenter,
		UD_Capt_JustifyRight,
		UD_Capt_JustifyJustify,
		UD_Capt_AlignLefts,
		UD_Capt_AlignCenters,
		UD_Capt_AlignRights,
		UD_Capt_AlignTops,
		UD_Capt_AlignMiddles,
		UD_Capt_AlignBottoms,
		UD_Capt_AlignToGrid,
		UD_Capt_MakeSameSizeWidth,
		UD_Capt_MakeSameSizeSizeToGrid,
		UD_Capt_MakeSameSizeHeight,
		UD_Capt_MakeSameSizeBoth,
		UD_Capt_SpacingMakeEqual,
		UD_Capt_SpacingIncrease,
		UD_Capt_SpacingDecrease,
		UD_Capt_SpacingRemove,
		UD_Capt_CenterInFormHorizontally,
		UD_Capt_CenterInFormVertically,
		UD_Capt_OrderBringToFront,
		UD_Capt_OrderSendToBack,
		UD_Capt_Zoom,
		UD_Capt_ZoomIn,
		UD_Capt_ZoomOut,
		UD_Capt_ZoomFactor,
		UD_Hint_NewReport,
		UD_Hint_NewWizardReport,
		UD_Hint_OpenFile,
		UD_Hint_SaveFile,
		UD_Hint_SaveFileAs,
		UD_Hint_SaveAll,
		UD_Hint_Exit,
		UD_Hint_Close,
		UD_Hint_TabbedInterface,
		UD_Hint_MdiCascade,
		UD_Hint_MdiTileHorizontal,
		UD_Hint_MdiTileVertical,
		UD_Hint_Cut,
		UD_Hint_Copy,
		UD_Hint_Paste,
		UD_Hint_Delete,
		UD_Hint_SelectAll,
		UD_Hint_Undo,
		UD_Hint_Redo,
		UD_Hint_ForegroundColor,
		UD_Hint_BackGroundColor,
		UD_Hint_FontBold,
		UD_Hint_FontItalic,
		UD_Hint_FontUnderline,
		UD_Hint_JustifyLeft,
		UD_Hint_JustifyCenter,
		UD_Hint_JustifyRight,
		UD_Hint_JustifyJustify,
		UD_Hint_AlignLefts,
		UD_Hint_AlignCenters,
		UD_Hint_AlignRights,
		UD_Hint_AlignTops,
		UD_Hint_AlignMiddles,
		UD_Hint_AlignBottoms,
		UD_Hint_AlignToGrid,
		UD_Hint_MakeSameSizeWidth,
		UD_Hint_MakeSameSizeSizeToGrid,
		UD_Hint_MakeSameSizeHeight,
		UD_Hint_MakeSameSizeBoth,
		UD_Hint_SpacingMakeEqual,
		UD_Hint_SpacingIncrease,
		UD_Hint_SpacingDecrease,
		UD_Hint_SpacingRemove,
		UD_Hint_CenterInFormHorizontally,
		UD_Hint_CenterInFormVertically,
		UD_Hint_OrderBringToFront,
		UD_Hint_OrderSendToBack,
		UD_Hint_Zoom,
		UD_Hint_ZoomIn,
		UD_Hint_ZoomOut,
		UD_Hint_ViewBars,
		UD_Hint_ViewDockPanels,
		UD_Hint_ViewTabs,
		UD_PropertyGrid_NotSetText,
		UD_SaveFileDialog_DialogFilter,
		UD_Title_FieldList,
		UD_Title_FieldList_NonePickerNodeText,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.NoneString' instead")]
		UD_Title_FieldList_NoneNodeText,
		UD_Title_FieldList_ProjectObjectsText,
		UD_Title_FieldList_AddNewDataSourceText,
		UD_Title_GroupAndSort,
		UD_Title_ErrorList,
		UD_Title_ReportExplorer,
		UD_Title_ReportExplorer_Components,
		UD_Title_ReportExplorer_FormattingRules,
		UD_Title_ReportExplorer_Styles,
		UD_Title_ReportExplorer_NullControl,
		UD_Title_PropertyGrid,
		UD_Title_ToolBox,
		STag_Name_DataBinding,
		STag_Name_FormatString,
		STag_Name_Checked,
		STag_Name_PreviewRowCount,
		STag_Name_Bands,
		STag_Name_Height,
		STag_Name_ColumnMode,
		STag_Name_ColumnCount,
		STag_Name_ColumnWidth,
		STag_Name_ColumnSpacing,
		STag_Name_ColumnLayout,
		STag_Name_FieldArea,
		STag_Capt_Tasks,
		STag_Capt_Format,
		RibbonXRDesign_PageText,
		RibbonXRDesign_ToolboxControlsPage,
		RibbonXRDesign_HtmlPageText,
		RibbonXRDesign_StatusBar_HtmlProcessing,
		RibbonXRDesign_StatusBar_HtmlDone,
		RibbonXRDesign_PageGroup_Report,
		RibbonXRDesign_PageGroup_Edit,
		RibbonXRDesign_PageGroup_Font,
		RibbonXRDesign_PageGroup_Alignment,
		RibbonXRDesign_PageGroup_SizeAndLayout,
		RibbonXRDesign_PageGroup_Zoom,
		RibbonXRDesign_PageGroup_View,
		RibbonXRDesign_PageGroup_Scripts,
		RibbonXRDesign_PageGroup_HtmlNavigation,
		RibbonXRDesign_AlignToGrid_Caption,
		RibbonXRDesign_AlignLeft_Caption,
		RibbonXRDesign_AlignVerticalCenters_Caption,
		RibbonXRDesign_AlignRight_Caption,
		RibbonXRDesign_AlignTop_Caption,
		RibbonXRDesign_AlignHorizontalCenters_Caption,
		RibbonXRDesign_AlignBottom_Caption,
		RibbonXRDesign_SizeToControlWidth_Caption,
		RibbonXRDesign_SizeToGrid_Caption,
		RibbonXRDesign_SizeToControlHeight_Caption,
		RibbonXRDesign_SizeToControl_Caption,
		RibbonXRDesign_HorizSpaceMakeEqual_Caption,
		RibbonXRDesign_HorizSpaceIncrease_Caption,
		RibbonXRDesign_HorizSpaceDecrease_Caption,
		RibbonXRDesign_HorizSpaceConcatenate_Caption,
		RibbonXRDesign_VertSpaceMakeEqual_Caption,
		RibbonXRDesign_VertSpaceIncrease_Caption,
		RibbonXRDesign_VertSpaceDecrease_Caption,
		RibbonXRDesign_VertSpaceConcatenate_Caption,
		RibbonXRDesign_CenterHorizontally_Caption,
		RibbonXRDesign_CenterVertically_Caption,
		RibbonXRDesign_BringToFront_Caption,
		RibbonXRDesign_SendToBack_Caption,
		RibbonXRDesign_FontBold_Caption,
		RibbonXRDesign_FontItalic_Caption,
		RibbonXRDesign_FontUnderline_Caption,
		RibbonXRDesign_ForeColor_Caption,
		RibbonXRDesign_BackColor_Caption,
		RibbonXRDesign_JustifyLeft_Caption,
		RibbonXRDesign_JustifyCenter_Caption,
		RibbonXRDesign_JustifyRight_Caption,
		RibbonXRDesign_JustifyJustify_Caption,
		RibbonXRDesign_NewReport_Caption,
		RibbonXRDesign_NewReportWizard_Caption,
		RibbonXRDesign_OpenFile_Caption,
		RibbonXRDesign_SaveFile_Caption,
		RibbonXRDesign_SaveFileAs_Caption,
		RibbonXRDesign_SaveAll_Caption,
		RibbonXRDesign_Cut_Caption,
		RibbonXRDesign_Copy_Caption,
		RibbonXRDesign_Paste_Caption,
		RibbonXRDesign_Undo_Caption,
		RibbonXRDesign_Redo_Caption,
		RibbonXRDesign_Exit_Caption,
		RibbonXRDesign_Close_Caption,
		RibbonXRDesign_Zoom_Caption,
		RibbonXRDesign_ZoomIn_Caption,
		RibbonXRDesign_ZoomOut_Caption,
		RibbonXRDesign_ZoomExact_Caption,
		RibbonXRDesign_Windows_Caption,
		RibbonXRDesign_Scripts_Caption,
		RibbonXRDesign_HtmlHome_Caption,
		RibbonXRDesign_HtmlBackward_Caption,
		RibbonXRDesign_HtmlForward_Caption,
		RibbonXRDesign_HtmlRefresh_Caption,
		RibbonXRDesign_HtmlFind_Caption,
		RibbonXRDesign_AlignToGrid_STipTitle,
		RibbonXRDesign_AlignLeft_STipTitle,
		RibbonXRDesign_AlignVerticalCenters_STipTitle,
		RibbonXRDesign_AlignRight_STipTitle,
		RibbonXRDesign_AlignTop_STipTitle,
		RibbonXRDesign_AlignHorizontalCenters_STipTitle,
		RibbonXRDesign_AlignBottom_STipTitle,
		RibbonXRDesign_SizeToControlWidth_STipTitle,
		RibbonXRDesign_SizeToGrid_STipTitle,
		RibbonXRDesign_SizeToControlHeight_STipTitle,
		RibbonXRDesign_SizeToControl_STipTitle,
		RibbonXRDesign_HorizSpaceMakeEqual_STipTitle,
		RibbonXRDesign_HorizSpaceIncrease_STipTitle,
		RibbonXRDesign_HorizSpaceDecrease_STipTitle,
		RibbonXRDesign_HorizSpaceConcatenate_STipTitle,
		RibbonXRDesign_VertSpaceMakeEqual_STipTitle,
		RibbonXRDesign_VertSpaceIncrease_STipTitle,
		RibbonXRDesign_VertSpaceDecrease_STipTitle,
		RibbonXRDesign_VertSpaceConcatenate_STipTitle,
		RibbonXRDesign_CenterHorizontally_STipTitle,
		RibbonXRDesign_CenterVertically_STipTitle,
		RibbonXRDesign_BringToFront_STipTitle,
		RibbonXRDesign_SendToBack_STipTitle,
		RibbonXRDesign_FontBold_STipTitle,
		RibbonXRDesign_FontItalic_STipTitle,
		RibbonXRDesign_FontUnderline_STipTitle,
		RibbonXRDesign_ForeColor_STipTitle,
		RibbonXRDesign_BackColor_STipTitle,
		RibbonXRDesign_JustifyLeft_STipTitle,
		RibbonXRDesign_JustifyCenter_STipTitle,
		RibbonXRDesign_JustifyRight_STipTitle,
		RibbonXRDesign_JustifyJustify_STipTitle,
		RibbonXRDesign_NewReport_STipTitle,
		RibbonXRDesign_NewReportWizard_STipTitle,
		RibbonXRDesign_OpenFile_STipTitle,
		RibbonXRDesign_SaveFile_STipTitle,
		RibbonXRDesign_SaveFileAs_STipTitle,
		RibbonXRDesign_SaveAll_STipTitle,
		RibbonXRDesign_Cut_STipTitle,
		RibbonXRDesign_Copy_STipTitle,
		RibbonXRDesign_Paste_STipTitle,
		RibbonXRDesign_Undo_STipTitle,
		RibbonXRDesign_Redo_STipTitle,
		RibbonXRDesign_Exit_STipTitle,
		RibbonXRDesign_Close_STipTitle,
		RibbonXRDesign_Zoom_STipTitle,
		RibbonXRDesign_ZoomIn_STipTitle,
		RibbonXRDesign_ZoomOut_STipTitle,
		RibbonXRDesign_FontName_STipTitle,
		RibbonXRDesign_FontSize_STipTitle,
		RibbonXRDesign_Windows_STipTitle,
		RibbonXRDesign_Scripts_STipTitle,
		RibbonXRDesign_HtmlHome_STipTitle,
		RibbonXRDesign_HtmlBackward_STipTitle,
		RibbonXRDesign_HtmlForward_STipTitle,
		RibbonXRDesign_HtmlRefresh_STipTitle,
		RibbonXRDesign_HtmlFind_STipTitle,
		RibbonXRDesign_AlignToGrid_STipContent,
		RibbonXRDesign_AlignLeft_STipContent,
		RibbonXRDesign_AlignVerticalCenters_STipContent,
		RibbonXRDesign_AlignRight_STipContent,
		RibbonXRDesign_AlignTop_STipContent,
		RibbonXRDesign_AlignHorizontalCenters_STipContent,
		RibbonXRDesign_AlignBottom_STipContent,
		RibbonXRDesign_SizeToControlWidth_STipContent,
		RibbonXRDesign_SizeToGrid_STipContent,
		RibbonXRDesign_SizeToControlHeight_STipContent,
		RibbonXRDesign_SizeToControl_STipContent,
		RibbonXRDesign_HorizSpaceMakeEqual_STipContent,
		RibbonXRDesign_HorizSpaceIncrease_STipContent,
		RibbonXRDesign_HorizSpaceDecrease_STipContent,
		RibbonXRDesign_HorizSpaceConcatenate_STipContent,
		RibbonXRDesign_VertSpaceMakeEqual_STipContent,
		RibbonXRDesign_VertSpaceIncrease_STipContent,
		RibbonXRDesign_VertSpaceDecrease_STipContent,
		RibbonXRDesign_VertSpaceConcatenate_STipContent,
		RibbonXRDesign_CenterHorizontally_STipContent,
		RibbonXRDesign_CenterVertically_STipContent,
		RibbonXRDesign_BringToFront_STipContent,
		RibbonXRDesign_SendToBack_STipContent,
		RibbonXRDesign_FontBold_STipContent,
		RibbonXRDesign_FontItalic_STipContent,
		RibbonXRDesign_FontUnderline_STipContent,
		RibbonXRDesign_ForeColor_STipContent,
		RibbonXRDesign_BackColor_STipContent,
		RibbonXRDesign_JustifyLeft_STipContent,
		RibbonXRDesign_JustifyCenter_STipContent,
		RibbonXRDesign_JustifyRight_STipContent,
		RibbonXRDesign_JustifyJustify_STipContent,
		RibbonXRDesign_Close_STipContent,
		RibbonXRDesign_Exit_STipContent,
		RibbonXRDesign_NewReport_STipContent,
		RibbonXRDesign_NewReportWizard_STipContent,
		RibbonXRDesign_OpenFile_STipContent,
		RibbonXRDesign_SaveFile_STipContent,
		RibbonXRDesign_SaveFileAs_STipContent,
		RibbonXRDesign_SaveAll_STipContent,
		RibbonXRDesign_Cut_STipContent,
		RibbonXRDesign_Copy_STipContent,
		RibbonXRDesign_Paste_STipContent,
		RibbonXRDesign_Undo_STipContent,
		RibbonXRDesign_Redo_STipContent,
		RibbonXRDesign_Zoom_STipContent,
		RibbonXRDesign_ZoomIn_STipContent,
		RibbonXRDesign_ZoomOut_STipContent,
		RibbonXRDesign_FontName_STipContent,
		RibbonXRDesign_FontSize_STipContent,
		RibbonXRDesign_Windows_STipContent,
		RibbonXRDesign_Scripts_STipContent,
		RibbonXRDesign_HtmlHome_STipContent,
		RibbonXRDesign_HtmlBackward_STipContent,
		RibbonXRDesign_HtmlForward_STipContent,
		RibbonXRDesign_HtmlRefresh_STipContent,
		RibbonXRDesign_HtmlFind_STipContent,
		RibbonXRDesign_NewReport_Description,
		RibbonXRDesign_NewReportWizard_Description,
		RibbonXRDesign_SaveFile_Description,
		RibbonXRDesign_SaveFileAs_Description,
		XRSubreport_NameInfo,
		XRSubreport_NullReportSourceInfo,
		XRSubreport_ReportSourceInfo,
		XRSubreport_ReportSourceUrlInfo,
		PivotGridForm_GroupMain_Caption,
		PivotGridForm_GroupMain_Description,
		PivotGridForm_ItemFields_Caption,
		PivotGridForm_ItemFields_Description,
		PivotGridForm_ItemLayout_Caption,
		PivotGridForm_ItemLayout_Description,
		PivotGridForm_GroupPrinting_Caption,
		PivotGridForm_GroupPrinting_Description,
		PivotGridForm_ItemAppearances_Caption,
		PivotGridForm_ItemAppearances_Description,
		PivotGridForm_ItemSettings_Caption,
		PivotGridForm_ItemSettings_Description,
		PivotGridFrame_Fields_ColumnsText,
		PivotGridFrame_Fields_DescriptionText1,
		PivotGridFrame_Fields_DescriptionText2,
		PivotGridFrame_Layouts_DescriptionText,
		PivotGridFrame_Layouts_SelectorCaption1,
		PivotGridFrame_Layouts_SelectorCaption2,
		PivotGridFrame_Appearances_DescriptionText,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.ParametersRequest_Submit' instead")]
		ParametersRequest_Submit,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.ParametersRequest_Reset' instead")]
		ParametersRequest_Reset,
		[Obsolete("You should use the 'DevExpress.XtraPrinting.Localization.PreviewStringId.ParametersRequest_Caption' instead")]
		ParametersRequest_Caption,
		DesignerStatus_Location,
		DesignerStatus_Size,
		DesignerStatus_Height,
		Wizard_PageChooseFields_Msg,
		GroupSort_AddGroup,
		GroupSort_AddSort,
		GroupSort_MoveUp,
		GroupSort_MoveDown,
		GroupSort_Delete,
		NewParameterEditorForm_DataSource,
		NewParameterEditorForm_DataAdapter,
		NewParameterEditorForm_DataMember,
		NewParameterEditorForm_ValueMember,
		NewParameterEditorForm_DisplayMember,
		BindingMapperForm_InvalidBindingWarning,
		BindingMapperForm_ShowOnlyInvalidBindings,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("For localization use the DevExpress .NET Localization Service.")]
		XRTableOfContents_LevelCollectionEditor_Title,
		SubreportDesigner_EditParameterBindings,
	}
	#endregion
	#region ReportLocalizer.AddStrings 
	public partial class ReportLocalizer {
		void AddStrings() {
			AddString(ReportStringId.Dlg_SaveFile_Title, "Save '{0}'");
			AddString(ReportStringId.Msg_InvalidDrillDownControl, "The DrillDownControl property of the '{0}' band is not valid.");
			AddString(ReportStringId.Msg_WarningFontNameCantBeEmpty, "The Font name can't be empty.");
			AddString(ReportStringId.Msg_FileNotFound, "File not found.");
			AddString(ReportStringId.Msg_WrongReportClassName, "An error occurred during deserialization - possible wrong report class name");
			AddString(ReportStringId.Msg_CreateReportInstance, "The report currently being edited is of a different type than the one you are trying to open.\r\nDo you want to open the selected report anyway?");
			AddString(ReportStringId.Msg_FileCorrupted, "Can't load the report. The file is possibly corrupted or report's assembly is missing.");
			AddString(ReportStringId.Msg_FileContentCorrupted, "Can't load the report's layout. The file is possibly corrupted or contains incorrect information.");
			AddString(ReportStringId.Msg_IncorrectArgument, "Incorrect argument's value");
			AddString(ReportStringId.Msg_InvalidMethodCall, "This method call is invalid for the object's current state");
			AddString(ReportStringId.Msg_ScriptError, "There are following errors in script(s):\r\n{0}");
			AddString(ReportStringId.Msg_ScriptExecutionError, "The following error occurred when the script in procedure {0} was executed:\r\n {1}");
			AddString(ReportStringId.Msg_ScriptCodeIsNotCorrect, "Entered code is not correct");
			AddString(ReportStringId.Msg_InvalidReportSource, "The 'ReportSource' property of a subreport control cannot be set to a descendant of the current report");
			AddString(ReportStringId.Msg_IncorrectBandType, "Incorrect band type");
			AddString(ReportStringId.Msg_CreateSomeInstance, "Can't create two instances of a class on a form");
			AddString(ReportStringId.Msg_DontSupportMulticolumn, "Detail reports don't support multicolumn.");
			AddString(ReportStringId.Msg_FillDataError, "Error when trying to populate the datasource. The following exception was thrown:");
			AddString(ReportStringId.Msg_PlacingXrTocIntoIncorrectContainer, "The XRTableOfContents control can be placed only into Report Header and Report Footer bands.");
			AddString(ReportStringId.Msg_NoBookmarksWereFoundInReportForXrToc, "No bookmarks were found in the report. To create a table of contents, specify a bookmark for at least one report element.");
			AddString(ReportStringId.Msg_InvalidLeaderSymbolForXrTocLevel, "Invalid leader symbol.");
			AddString(ReportStringId.Msg_InvalidXrTocInstance, "No more instances of XRTableOfContents can be added to the report.");
			AddString(ReportStringId.Msg_InvalidXrTocInstanceInBand, "No more instances of XRTableOfContents can be added to the band.");
			AddString(ReportStringId.Msg_ApplyChangesQuestion, "Are you sure you want to apply these changes?");
			AddString(ReportStringId.Msg_CyclicBookmarks, "There are cyclic bookmarks in the report.");
			AddString(ReportStringId.Msg_LargeText, "Text is too large.");
			AddString(ReportStringId.Msg_ScriptingPermissionErrorMessage, "You don't have sufficient permission to execute the scripts in this report.\r\n\r\nDetails:\r\n\r\n{0}");
			AddString(ReportStringId.Msg_ReportImporting, "Importing a report layout. Please, wait...");
			AddString(ReportStringId.Msg_IncorrectPadding, "The padding should be greater than or equal to 0.");
			AddString(ReportStringId.Msg_WarningControlsAreOverlapped, "Export warning: The following controls are overlapped and may be exported to HTML, RTF, XLS, XLSX, CSV and Text incorrectly - {0}.");
			AddString(ReportStringId.Msg_WarningControlsAreOutOfMargin, "Printing warning: The following controls are outside the right page margin, and this will cause extra pages to be printed - {0}.");
			AddString(ReportStringId.Msg_WarningUnsavedReports, "Printing warning: Save the following reports to preview subreports with recent changes applied - {0}.");
			AddString(ReportStringId.Msg_ShapeRotationToolTip, "Use Ctrl with the left mouse button to rotate the shape");
			AddString(ReportStringId.Msg_WarningRemoveCalculatedFields, "This operation will remove all calculated fields from all data tables. Do you wish to proceed?");
			AddString(ReportStringId.Msg_WarningRemoveParameters, "This operation will remove all parameters. Do you wish to proceed?");
			AddString(ReportStringId.Msg_WarningRemoveStyles, "This operation will remove all styles. Do you wish to proceed?");
			AddString(ReportStringId.Msg_WarningRemoveFormattingRules, "This operation will remove all formatting rules. Do you wish to proceed?");
			AddString(ReportStringId.Msg_ErrorTitle, "Error");
			AddString(ReportStringId.Msg_SerializationErrorTitle, "Serialization Error");
			AddString(ReportStringId.Msg_InvalidExpression, "The specified expression contains invalid symbols (line {0}, character {1}).");
			AddString(ReportStringId.Msg_InvalidExpressionEx, "The specified expression is invalid.");
			AddString(ReportStringId.Msg_NoCustomFunction, "Custom function '{0}' not found.");
			AddString(ReportStringId.Msg_InvalidCondition, "The condition must be Boolean!");
			AddString(ReportStringId.Msg_GroupSortWarning, "The group header or footer you want to delete is not empty. Do you want to delete this band along with its controls?");
			AddString(ReportStringId.Msg_GroupSortNoDataSource, "To add a new grouping or sorting level, first provide a data source for the report.");
			AddString(ReportStringId.Msg_NotEnoughMemoryToPaint, "Not enough memory to paint. Zoom level will be reset.");
			AddString(ReportStringId.Msg_Caption, "XtraReports");
			AddString(ReportStringId.Msg_ParameterBindingValueTypeMismatch, "A parameter binding assigns a value of an incompatible type to the subreport parameter \"{0}\".");
			AddString(ReportStringId.Cmd_Commands, "Commands");
			AddString(ReportStringId.Cmd_InsertDetailReport, "Insert Detail Report");
			AddString(ReportStringId.Cmd_InsertUnboundDetailReport, "Unbound");
			AddString(ReportStringId.Cmd_ViewCode, "View &Code");
			AddString(ReportStringId.Cmd_BringToFront, "&Bring To Front");
			AddString(ReportStringId.Cmd_SendToBack, "&Send To Back");
			AddString(ReportStringId.Cmd_AlignToGrid, "Align To &Grid");
			AddString(ReportStringId.Cmd_TopMargin, "TopMargin");
			AddString(ReportStringId.Cmd_BottomMargin, "BottomMargin");
			AddString(ReportStringId.Cmd_ReportHeader, "ReportHeader");
			AddString(ReportStringId.Cmd_ReportFooter, "ReportFooter");
			AddString(ReportStringId.Cmd_PageHeader, "PageHeader");
			AddString(ReportStringId.Cmd_PageFooter, "PageFooter");
			AddString(ReportStringId.Cmd_GroupHeader, "GroupHeader");
			AddString(ReportStringId.Cmd_GroupFooter, "GroupFooter");
			AddString(ReportStringId.Cmd_Detail, "Detail");
			AddString(ReportStringId.Cmd_DetailReport, "DetailReport");
			AddString(ReportStringId.Cmd_RtfClear, "Clear");
			AddString(ReportStringId.Cmd_RtfLoad, "Load File...");
			AddString(ReportStringId.Cmd_TableInsert, "&Insert");
			AddString(ReportStringId.Cmd_TableInsertRowAbove, "Row &Above");
			AddString(ReportStringId.Cmd_TableInsertRowBelow, "Row &Below");
			AddString(ReportStringId.Cmd_TableInsertColumnToLeft, "Column To &Left");
			AddString(ReportStringId.Cmd_TableInsertColumnToRight, "Column To &Right");
			AddString(ReportStringId.Cmd_TableInsertCell, "&Cell");
			AddString(ReportStringId.Cmd_TableDelete, "De&lete");
			AddString(ReportStringId.Cmd_TableDeleteRow, "&Row");
			AddString(ReportStringId.Cmd_TableDeleteColumn, "&Column");
			AddString(ReportStringId.Cmd_TableDeleteCell, "Ce&ll");
			AddString(ReportStringId.Cmd_TableConvertToLabels, "&Convert To Labels");
			AddString(ReportStringId.Cmd_Cut, "Cu&t");
			AddString(ReportStringId.Cmd_Copy, "Cop&y");
			AddString(ReportStringId.Cmd_Paste, "&Paste");
			AddString(ReportStringId.Cmd_Delete, "&Delete");
			AddString(ReportStringId.Cmd_Properties, "P&roperties");
			AddString(ReportStringId.Cmd_InsertBand, "Insert &Band");
			AddString(ReportStringId.Cmd_AddSubBand, "Add &Sub-Band");
			AddString(ReportStringId.Cmd_BandMoveUp, "Move Up");
			AddString(ReportStringId.Cmd_BandMoveDown, "Move Down");
			AddString(ReportStringId.Cmd_AddStyle, "Add Style");
			AddString(ReportStringId.Cmd_EditStyles, "Edit Styles...");
			AddString(ReportStringId.Cmd_DeleteStyle, "Delete");
			AddString(ReportStringId.Cmd_PurgeStyles, "Delete Unused Styles");
			AddString(ReportStringId.Cmd_ClearStyles, "Remove All Styles");
			AddString(ReportStringId.Cmd_SelectControlsWithStyle, "Select Controls With Style");
			AddString(ReportStringId.Cmd_AssignStyleToXRControl, "Assign Style To The XRControl");
			AddString(ReportStringId.Cmd_AssignOddStyleToXRControl, "Assign Odd Style To The XRControl");
			AddString(ReportStringId.Cmd_AssignEvenStyleToXRControl, "Assign Even Style To The XRControl");
			AddString(ReportStringId.Cmd_AddFormattingRule, "Add Formatting Rule");
			AddString(ReportStringId.Cmd_EditFormattingRules, "Edit Formatting Rules...");
			AddString(ReportStringId.Cmd_DeleteFormattingRule, "Delete");
			AddString(ReportStringId.Cmd_PurgeFormattingRules, "Delete Unused Formatting Rules");
			AddString(ReportStringId.Cmd_ClearFormattingRules, "Remove All Formatting Rules");
			AddString(ReportStringId.Cmd_SelectControlsWithFormattingRule, "Select Controls With Formatting Rule");
			AddString(ReportStringId.CatLayout, "Layout");
			AddString(ReportStringId.CatAppearance, "Appearance");
			AddString(ReportStringId.CatData, "Data");
			AddString(ReportStringId.CatBehavior, "Behavior");
			AddString(ReportStringId.CatNavigation, "Navigation");
			AddString(ReportStringId.CatPageSettings, "Page Settings");
			AddString(ReportStringId.CatUserDesigner, "User Designer");
			AddString(ReportStringId.CatDesign, "Design");
			AddString(ReportStringId.CatParameters, "Parameters");
			AddString(ReportStringId.CatStructure, "Structure");
			AddString(ReportStringId.CatPrinting, "Printing");
			AddString(ReportStringId.CatElements, "Elements");
			AddString(ReportStringId.CatOptions, "Options");
			AddString(ReportStringId.BandDsg_QuantityPerPage, "one band per page");
			AddString(ReportStringId.BandDsg_QuantityPerReport, "one band per report");
			AddString(ReportStringId.UD_ReportDesigner, "Report Designer");
			AddString(ReportStringId.UD_Msg_ReportChanged, "Report has been changed. Do you want to save changes ?");
			AddString(ReportStringId.UD_Msg_MdiReportChanged, "\"{0}\" has been changed. Do you want to save changes ?");
			AddString(ReportStringId.UD_TTip_FileOpen, "Open File");
			AddString(ReportStringId.UD_TTip_FileSave, "Save File");
			AddString(ReportStringId.UD_Msg_ReportSourceUrlNotFound, "No report with the specified URL has been found. Do you want to create a new report?");
			AddString(ReportStringId.UD_TTip_EditCut, "Cut");
			AddString(ReportStringId.UD_TTip_EditCopy, "Copy");
			AddString(ReportStringId.UD_TTip_EditPaste, "Paste");
			AddString(ReportStringId.UD_TTip_Undo, "Undo");
			AddString(ReportStringId.UD_TTip_Redo, "Redo");
			AddString(ReportStringId.UD_TTip_AlignToGrid, "Align to Grid");
			AddString(ReportStringId.UD_TTip_AlignLeft, "Align Lefts");
			AddString(ReportStringId.UD_TTip_AlignVerticalCenters, "Align Centers");
			AddString(ReportStringId.UD_TTip_AlignRight, "Align Rights");
			AddString(ReportStringId.UD_TTip_AlignTop, "Align Tops");
			AddString(ReportStringId.UD_TTip_AlignHorizontalCenters, "Align Middles");
			AddString(ReportStringId.UD_TTip_AlignBottom, "Align Bottoms");
			AddString(ReportStringId.UD_TTip_SizeToControlWidth, "Make Same Width");
			AddString(ReportStringId.UD_TTip_SizeToGrid, "Size to Grid");
			AddString(ReportStringId.UD_TTip_SizeToControlHeight, "Make Same Height");
			AddString(ReportStringId.UD_TTip_SizeToControl, "Make Same size");
			AddString(ReportStringId.UD_TTip_HorizSpaceMakeEqual, "Make Horizontal Spacing Equal");
			AddString(ReportStringId.UD_TTip_HorizSpaceIncrease, "Increase Horizontal Spacing");
			AddString(ReportStringId.UD_TTip_HorizSpaceDecrease, "Decrease Horizontal Spacing");
			AddString(ReportStringId.UD_TTip_HorizSpaceConcatenate, "Remove Horizontal Spacing");
			AddString(ReportStringId.UD_TTip_VertSpaceMakeEqual, "Make Vertical Spacing Equal");
			AddString(ReportStringId.UD_TTip_VertSpaceIncrease, "Increase Vertical Spacing");
			AddString(ReportStringId.UD_TTip_VertSpaceDecrease, "Decrease Vertical Spacing");
			AddString(ReportStringId.UD_TTip_VertSpaceConcatenate, "Remove Vertical Spacing");
			AddString(ReportStringId.UD_TTip_CenterHorizontally, "Center Horizontally");
			AddString(ReportStringId.UD_TTip_CenterVertically, "CenterVertically");
			AddString(ReportStringId.UD_TTip_BringToFront, "Bring to Front");
			AddString(ReportStringId.UD_TTip_SendToBack, "Send to Back");
			AddString(ReportStringId.UD_TTip_FormatBold, "Bold");
			AddString(ReportStringId.UD_TTip_FormatItalic, "Italic");
			AddString(ReportStringId.UD_TTip_FormatUnderline, "Underline");
			AddString(ReportStringId.UD_TTip_FormatAlignLeft, "Align Left");
			AddString(ReportStringId.UD_TTip_FormatCenter, "Center");
			AddString(ReportStringId.UD_TTip_FormatAlignRight, "Align Right");
			AddString(ReportStringId.UD_TTip_FormatFontName, "Font Name");
			AddString(ReportStringId.UD_TTip_FormatFontSize, "Font Size");
			AddString(ReportStringId.UD_TTip_FormatForeColor, "Foreground Color");
			AddString(ReportStringId.UD_TTip_FormatBackColor, "Background Color");
			AddString(ReportStringId.UD_TTip_FormatJustify, "Justify");
			AddString(ReportStringId.UD_TTip_ItemDescription, "Drag-and-drop this item to create a control bound to it;\r\n- or -\r\nDrag this item with the right mouse button or SHIFT\r\nto select a bound control from the popup menu;\r\n- or -\r\nUse the context menu to add a calculated field or parameter.");
			AddString(ReportStringId.UD_TTip_TableDescription, "Drag-and-drop this item to create a table with its items;\r\n- or -\r\nDrag this item with the right mouse button or SHIFT\r\nto create a 'header' table with field names;\r\n- or -\r\nUse the context menu to add a calculated field or parameter.");
			AddString(ReportStringId.UD_TTip_DataMemberDescription, "\r\n\r\nDataMember: {0}");
			AddString(ReportStringId.UD_FormCaption, "Report Designer");
			AddString(ReportStringId.UD_XtraReportsToolboxCategoryName, "Standard Controls");
			AddString(ReportStringId.UD_XtraReportsPointerItemCaption, "Pointer");
			AddString(ReportStringId.Verb_EditBands, "Edit and Reorder Bands...");
			AddString(ReportStringId.Verb_EditGroupFields, "Edit GroupFields...");
			AddString(ReportStringId.Verb_Import, "Open/Import...");
			AddString(ReportStringId.Verb_Export, "Save/Export...");
			AddString(ReportStringId.Verb_Save, "Save...");
			AddString(ReportStringId.Verb_About, "About");
			AddString(ReportStringId.Verb_RTFClear, "Clear");
			AddString(ReportStringId.Verb_RTFLoad, "Load File...");
			AddString(ReportStringId.Verb_FormatString, "Format String...");
			AddString(ReportStringId.Verb_SummaryWizard, "Summary...");
			AddString(ReportStringId.Verb_ReportWizard, "Design in Report Wizard...");
			AddString(ReportStringId.Verb_Insert, "Insert...");
			AddString(ReportStringId.Verb_Delete, "Delete...");
			AddString(ReportStringId.Verb_Bind, "Bind");
			AddString(ReportStringId.Verb_EditText, "Edit Text");
			AddString(ReportStringId.Verb_AddFieldToArea, "Add Field to Area");
			AddString(ReportStringId.Verb_RunDesigner, "Run Designer...");
			AddString(ReportStringId.Verb_EditBindings, "Edit Bindings...");
			AddString(ReportStringId.Verb_LoadReportTemplate, "Load Report Template...");
			AddString(ReportStringId.BCForm_Lbl_Property, "Property");
			AddString(ReportStringId.BCForm_Lbl_Binding, "Binding");
			AddString(ReportStringId.FRSForm_Caption, "Formatting Rule Sheet Editor");
			AddString(ReportStringId.FRSForm_Msg_NoRuleSelected, "No formatting rules are selected");
			AddString(ReportStringId.FRSForm_Msg_MoreThanOneRule, "You selected more than one formatting rule");
			AddString(ReportStringId.FRSForm_TTip_AddRule, "Add a formatting rule");
			AddString(ReportStringId.FRSForm_TTip_RemoveRule, "Remove a formatting rule");
			AddString(ReportStringId.FRSForm_TTip_ClearRules, "Clear formatting rules");
			AddString(ReportStringId.FRSForm_TTip_PurgeRules, "Delete unused formatting rules");
			AddString(ReportStringId.SSForm_Caption, "Styles Editor");
			AddString(ReportStringId.SSForm_Btn_Close, "Close");
			AddString(ReportStringId.SSForm_Msg_NoStyleSelected, "No styles are selected");
			AddString(ReportStringId.SSForm_Msg_MoreThanOneStyle, "You selected more than one style");
			AddString(ReportStringId.SSForm_Msg_SelectedStylesText, " selected styles...");
			AddString(ReportStringId.SSForm_Msg_StyleSheetError, "StyleSheet error");
			AddString(ReportStringId.SSForm_Msg_InvalidFileFormat, "Invalid file format");
			AddString(ReportStringId.SSForm_Msg_StyleNamePreviewPostfix, " style");
			AddString(ReportStringId.SSForm_Msg_FileFilter, "Report StyleSheet files (*.repss)|*.repss|All files (*.*)|*.*");
			AddString(ReportStringId.SSForm_TTip_AddStyle, "Add a style");
			AddString(ReportStringId.SSForm_TTip_RemoveStyle, "Remove a style");
			AddString(ReportStringId.SSForm_TTip_ClearStyles, "Clear styles");
			AddString(ReportStringId.SSForm_TTip_PurgeStyles, "Delete unused styles");
			AddString(ReportStringId.SSForm_TTip_SaveStyles, "Save styles to a file");
			AddString(ReportStringId.SSForm_TTip_LoadStyles, "Load styles from a file");
			AddString(ReportStringId.SR_Side_Margins, "Side margins");
			AddString(ReportStringId.SR_Top_Margin, "Top\r\nmargin");
			AddString(ReportStringId.SR_Vertical_Pitch, "Vertical\r\npitch");
			AddString(ReportStringId.SR_Horizontal_Pitch, "Horizontal pitch");
			AddString(ReportStringId.SR_Width, "Width");
			AddString(ReportStringId.SR_Height, "Height");
			AddString(ReportStringId.SR_Number_Down, "Number\r\nDown");
			AddString(ReportStringId.SR_Number_Across, "Number Across");
			AddString(ReportStringId.ScriptEditor_ErrorDescription, "Description");
			AddString(ReportStringId.ScriptEditor_ErrorLine, "Line");
			AddString(ReportStringId.ScriptEditor_ErrorColumn, "Column");
			AddString(ReportStringId.ScriptEditor_Validate, "Validate");
			AddString(ReportStringId.ScriptEditor_ScriptsAreValid, "All scripts are valid.");
			AddString(ReportStringId.ScriptEditor_ScriptHasBeenChanged, "The error log is unrelated to the actual script, because the script has been changed after its last validation.\r\nTo see the actual script errors, click the Validate button again.");
			AddString(ReportStringId.ScriptEditor_ClickValidate, "Click \"Validate\" to check scripts.");
			AddString(ReportStringId.ScriptEditor_NewString, "(New)");
			AddString(ReportStringId.RepTabCtl_HtmlView, "HTML View");
			AddString(ReportStringId.RepTabCtl_Preview, "Preview");
			AddString(ReportStringId.RepTabCtl_Designer, "Designer");
			AddString(ReportStringId.RepTabCtl_Scripts, "Scripts");
			AddString(ReportStringId.RepTabCtl_ReportStatus, "{0} {{ PaperKind: {1} }}");
			AddString(ReportStringId.PanelDesignMsg, "Place controls here to keep them together");
			AddString(ReportStringId.MultiColumnDesignMsg1, "Space for repeating columns.");
			AddString(ReportStringId.MultiColumnDesignMsg2, "Controls placed here will be printed incorrectly.");
			AddString(ReportStringId.UD_Group_File, "&File");
			AddString(ReportStringId.UD_Group_Edit, "&Edit");
			AddString(ReportStringId.UD_Group_View, "&View");
			AddString(ReportStringId.UD_Group_Format, "Fo&rmat");
			AddString(ReportStringId.UD_Group_Window, "&Window");
			AddString(ReportStringId.UD_Group_Font, "&Font");
			AddString(ReportStringId.UD_Group_Justify, "&Justify");
			AddString(ReportStringId.UD_Group_Align, "&Align");
			AddString(ReportStringId.UD_Group_MakeSameSize, "&Make Same Size");
			AddString(ReportStringId.UD_Group_HorizontalSpacing, "&Horizontal Spacing");
			AddString(ReportStringId.UD_Group_VerticalSpacing, "&Vertical Spacing");
			AddString(ReportStringId.UD_Group_CenterInForm, "&Center in Form");
			AddString(ReportStringId.UD_Group_Order, "&Order");
			AddString(ReportStringId.UD_Group_ToolbarsList, "&Toolbars");
			AddString(ReportStringId.UD_Group_DockPanelsList, "&Windows");
			AddString(ReportStringId.UD_Group_TabButtonsList, "Tab Buttons");
			AddString(ReportStringId.UD_Capt_MainMenuName, "Main Menu");
			AddString(ReportStringId.UD_Capt_ToolbarName, "Toolbar");
			AddString(ReportStringId.UD_Capt_LayoutToolbarName, "Layout Toolbar");
			AddString(ReportStringId.UD_Capt_FormattingToolbarName, "Formatting Toolbar");
			AddString(ReportStringId.UD_Capt_StatusBarName, "Status Bar");
			AddString(ReportStringId.UD_Capt_ZoomToolbarName, "Zoom Toolbar");
			AddString(ReportStringId.UD_Capt_NewReport, "&New");
			AddString(ReportStringId.UD_Capt_NewWizardReport, "New via &Wizard...");
			AddString(ReportStringId.UD_Capt_OpenFile, "&Open...");
			AddString(ReportStringId.UD_Capt_SaveFile, "&Save");
			AddString(ReportStringId.UD_Capt_SaveFileAs, "Save &As...");
			AddString(ReportStringId.UD_Capt_SaveAll, "Save A&ll");
			AddString(ReportStringId.UD_Capt_Exit, "E&xit");
			AddString(ReportStringId.UD_Capt_TabbedInterface, "&Tabbed Interface");
			AddString(ReportStringId.UD_Capt_MdiCascade, "&Cascade");
			AddString(ReportStringId.UD_Capt_MdiTileHorizontal, "Tile &Horizontal");
			AddString(ReportStringId.UD_Capt_MdiTileVertical, "Tile &Vertical");
			AddString(ReportStringId.UD_Capt_Close, "&Close");
			AddString(ReportStringId.UD_Capt_Cut, "Cu&t");
			AddString(ReportStringId.UD_Capt_Copy, "&Copy");
			AddString(ReportStringId.UD_Capt_Paste, "&Paste");
			AddString(ReportStringId.UD_Capt_Delete, "&Delete");
			AddString(ReportStringId.UD_Capt_SelectAll, "Select &All");
			AddString(ReportStringId.UD_Capt_Undo, "&Undo");
			AddString(ReportStringId.UD_Capt_Redo, "&Redo");
			AddString(ReportStringId.UD_Capt_ForegroundColor, "For&eground Color");
			AddString(ReportStringId.UD_Capt_BackGroundColor, "Bac&kground Color");
			AddString(ReportStringId.UD_Capt_FontBold, "&Bold");
			AddString(ReportStringId.UD_Capt_FontItalic, "&Italic");
			AddString(ReportStringId.UD_Capt_FontUnderline, "&Underline");
			AddString(ReportStringId.UD_Capt_JustifyLeft, "&Left");
			AddString(ReportStringId.UD_Capt_JustifyCenter, "&Center");
			AddString(ReportStringId.UD_Capt_JustifyRight, "&Rights");
			AddString(ReportStringId.UD_Capt_JustifyJustify, "&Justify");
			AddString(ReportStringId.UD_Capt_AlignLefts, "&Lefts");
			AddString(ReportStringId.UD_Capt_AlignCenters, "&Centers");
			AddString(ReportStringId.UD_Capt_AlignRights, "&Rights");
			AddString(ReportStringId.UD_Capt_AlignTops, "&Tops");
			AddString(ReportStringId.UD_Capt_AlignMiddles, "&Middles");
			AddString(ReportStringId.UD_Capt_AlignBottoms, "&Bottoms");
			AddString(ReportStringId.UD_Capt_AlignToGrid, "to &Grid");
			AddString(ReportStringId.UD_Capt_MakeSameSizeWidth, "&Width");
			AddString(ReportStringId.UD_Capt_MakeSameSizeSizeToGrid, "Size to Gri&d");
			AddString(ReportStringId.UD_Capt_MakeSameSizeHeight, "&Height");
			AddString(ReportStringId.UD_Capt_MakeSameSizeBoth, "&Both");
			AddString(ReportStringId.UD_Capt_SpacingMakeEqual, "Make &Equal");
			AddString(ReportStringId.UD_Capt_SpacingIncrease, "&Increase");
			AddString(ReportStringId.UD_Capt_SpacingDecrease, "&Decrease");
			AddString(ReportStringId.UD_Capt_SpacingRemove, "&Remove");
			AddString(ReportStringId.UD_Capt_CenterInFormHorizontally, "&Horizontally");
			AddString(ReportStringId.UD_Capt_CenterInFormVertically, "&Vertically");
			AddString(ReportStringId.UD_Capt_OrderBringToFront, "&Bring to Front");
			AddString(ReportStringId.UD_Capt_OrderSendToBack, "&Send to Back");
			AddString(ReportStringId.UD_Capt_Zoom, "Zoom");
			AddString(ReportStringId.UD_Capt_ZoomIn, "Zoom In");
			AddString(ReportStringId.UD_Capt_ZoomOut, "Zoom Out");
			AddString(ReportStringId.UD_Capt_ZoomFactor, "Zoom Factor: {0}%");
			AddString(ReportStringId.UD_Hint_NewReport, "Create a new blank report");
			AddString(ReportStringId.UD_Hint_NewWizardReport, "Create a new report using the Wizard");
			AddString(ReportStringId.UD_Hint_OpenFile, "Open a report");
			AddString(ReportStringId.UD_Hint_SaveFile, "Save the report");
			AddString(ReportStringId.UD_Hint_SaveFileAs, "Save the report with a new name");
			AddString(ReportStringId.UD_Hint_SaveAll, "Save all reports");
			AddString(ReportStringId.UD_Hint_Exit, "Close the designer");
			AddString(ReportStringId.UD_Hint_Close, "Close the report");
			AddString(ReportStringId.UD_Hint_TabbedInterface, "Switch between tabbed and window MDI layout modes");
			AddString(ReportStringId.UD_Hint_MdiCascade, "Arrange all open documents cascaded, so that they overlap each other");
			AddString(ReportStringId.UD_Hint_MdiTileHorizontal, "Arrange all open documents from top to bottom");
			AddString(ReportStringId.UD_Hint_MdiTileVertical, "Arrange all open documents from left to right");
			AddString(ReportStringId.UD_Hint_Cut, "Delete the control and copy it to the clipboard");
			AddString(ReportStringId.UD_Hint_Copy, "Copy the control to the clipboard");
			AddString(ReportStringId.UD_Hint_Paste, "Add the control from the clipboard");
			AddString(ReportStringId.UD_Hint_Delete, "Delete the control");
			AddString(ReportStringId.UD_Hint_SelectAll, "Select all the controls in the document");
			AddString(ReportStringId.UD_Hint_Undo, "Undo the last operation");
			AddString(ReportStringId.UD_Hint_Redo, "Redo the last operation");
			AddString(ReportStringId.UD_Hint_ForegroundColor, "Set the foreground color of the control");
			AddString(ReportStringId.UD_Hint_BackGroundColor, "Set the background color of the control");
			AddString(ReportStringId.UD_Hint_FontBold, "Make the font bold");
			AddString(ReportStringId.UD_Hint_FontItalic, "Make the font italic");
			AddString(ReportStringId.UD_Hint_FontUnderline, "Underline the font");
			AddString(ReportStringId.UD_Hint_JustifyLeft, "Align the control's text to the left");
			AddString(ReportStringId.UD_Hint_JustifyCenter, "Align the control's text to the center");
			AddString(ReportStringId.UD_Hint_JustifyRight, "Align the control's text to the right");
			AddString(ReportStringId.UD_Hint_JustifyJustify, "Justify the control's text");
			AddString(ReportStringId.UD_Hint_AlignLefts, "Left align the selected controls");
			AddString(ReportStringId.UD_Hint_AlignCenters, "Align the centers of the selected controls vertically");
			AddString(ReportStringId.UD_Hint_AlignRights, "Right align the selected controls");
			AddString(ReportStringId.UD_Hint_AlignTops, "Align the tops of the selected controls");
			AddString(ReportStringId.UD_Hint_AlignMiddles, "Align the centers of the selected controls horizontally");
			AddString(ReportStringId.UD_Hint_AlignBottoms, "Align the bottoms of the selected controls");
			AddString(ReportStringId.UD_Hint_AlignToGrid, "Align the positions of the selected controls to the grid");
			AddString(ReportStringId.UD_Hint_MakeSameSizeWidth, "Make the selected controls have the same width");
			AddString(ReportStringId.UD_Hint_MakeSameSizeSizeToGrid, "Size the selected controls to the grid");
			AddString(ReportStringId.UD_Hint_MakeSameSizeHeight, "Make the selected controls have the same height");
			AddString(ReportStringId.UD_Hint_MakeSameSizeBoth, "Make the selected controls the same size");
			AddString(ReportStringId.UD_Hint_SpacingMakeEqual, "Make the spacing between the selected controls equal");
			AddString(ReportStringId.UD_Hint_SpacingIncrease, "Increase the spacing between the selected controls");
			AddString(ReportStringId.UD_Hint_SpacingDecrease, "Decrease the spacing between the selected controls");
			AddString(ReportStringId.UD_Hint_SpacingRemove, "Remove the spacing between the selected controls");
			AddString(ReportStringId.UD_Hint_CenterInFormHorizontally, "Horizontally center the selected controls within a band");
			AddString(ReportStringId.UD_Hint_CenterInFormVertically, "Vertically center the selected controls within a band");
			AddString(ReportStringId.UD_Hint_OrderBringToFront, "Bring the selected controls to the front");
			AddString(ReportStringId.UD_Hint_OrderSendToBack, "Move the selected controls to the back");
			AddString(ReportStringId.UD_Hint_Zoom, "Select or input the zoom factor");
			AddString(ReportStringId.UD_Hint_ZoomIn, "Zoom in the design surface");
			AddString(ReportStringId.UD_Hint_ZoomOut, "Zoom out the design surface");
			AddString(ReportStringId.UD_Hint_ViewBars, "Hide or show the {0}");
			AddString(ReportStringId.UD_Hint_ViewDockPanels, "Hide or show the {0} window");
			AddString(ReportStringId.UD_Hint_ViewTabs, "Switch to the {0} tab");
			AddString(ReportStringId.UD_PropertyGrid_NotSetText, "(Not set)");
			AddString(ReportStringId.UD_SaveFileDialog_DialogFilter, "Report Files (*{0})|*{1}|All Files (*.*)|*.*");
			AddString(ReportStringId.UD_Title_FieldList, "Field List");
			AddString(ReportStringId.UD_Title_FieldList_NonePickerNodeText, "None");
			AddString(ReportStringId.UD_Title_FieldList_ProjectObjectsText, "Project Objects");
			AddString(ReportStringId.UD_Title_FieldList_AddNewDataSourceText, "Add New DataSource");
			AddString(ReportStringId.UD_Title_GroupAndSort, "Group and Sort");
			AddString(ReportStringId.UD_Title_ErrorList, "Scripts Errors");
			AddString(ReportStringId.UD_Title_ReportExplorer, "Report Explorer");
			AddString(ReportStringId.UD_Title_ReportExplorer_Components, "Components");
			AddString(ReportStringId.UD_Title_ReportExplorer_FormattingRules, "Formatting Rules");
			AddString(ReportStringId.UD_Title_ReportExplorer_Styles, "Styles");
			AddString(ReportStringId.UD_Title_ReportExplorer_NullControl, "None");
			AddString(ReportStringId.UD_Title_PropertyGrid, "Property Grid");
			AddString(ReportStringId.UD_Title_ToolBox, "Tool Box");
			AddString(ReportStringId.STag_Name_DataBinding, "Data Binding");
			AddString(ReportStringId.STag_Name_FormatString, "Format String");
			AddString(ReportStringId.STag_Name_Checked, "Checked");
			AddString(ReportStringId.STag_Name_PreviewRowCount, "Preview Row Count");
			AddString(ReportStringId.STag_Name_Bands, "Bands");
			AddString(ReportStringId.STag_Name_Height, "Height");
			AddString(ReportStringId.STag_Name_ColumnMode, "Multi-Column Mode");
			AddString(ReportStringId.STag_Name_ColumnCount, "Column Count");
			AddString(ReportStringId.STag_Name_ColumnWidth, "Column Width");
			AddString(ReportStringId.STag_Name_ColumnSpacing, "Column Spacing");
			AddString(ReportStringId.STag_Name_ColumnLayout, "Multi-Column Layout");
			AddString(ReportStringId.STag_Name_FieldArea, "Field Area for a New Field");
			AddString(ReportStringId.STag_Capt_Tasks, "Tasks");
			AddString(ReportStringId.STag_Capt_Format, "{0} {1}");
			AddString(ReportStringId.RibbonXRDesign_PageText, "Report Designer");
			AddString(ReportStringId.RibbonXRDesign_ToolboxControlsPage, "Toolbox");
			AddString(ReportStringId.RibbonXRDesign_HtmlPageText, "HTML View");
			AddString(ReportStringId.RibbonXRDesign_StatusBar_HtmlProcessing, "Processing...");
			AddString(ReportStringId.RibbonXRDesign_StatusBar_HtmlDone, "Done");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_Report, "Report");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_Edit, "Edit");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_Font, "Font");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_Alignment, "Alignment");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_SizeAndLayout, "Layout");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_Zoom, "Zoom");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_View, "View");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_Scripts, "Scripts");
			AddString(ReportStringId.RibbonXRDesign_PageGroup_HtmlNavigation, "Navigation");
			AddString(ReportStringId.RibbonXRDesign_AlignToGrid_Caption, "Align to Grid");
			AddString(ReportStringId.RibbonXRDesign_AlignLeft_Caption, "Align Lefts");
			AddString(ReportStringId.RibbonXRDesign_AlignVerticalCenters_Caption, "Align Centers");
			AddString(ReportStringId.RibbonXRDesign_AlignRight_Caption, "Align Rights");
			AddString(ReportStringId.RibbonXRDesign_AlignTop_Caption, "Align Tops");
			AddString(ReportStringId.RibbonXRDesign_AlignHorizontalCenters_Caption, "Align Middles");
			AddString(ReportStringId.RibbonXRDesign_AlignBottom_Caption, "Align Bottoms");
			AddString(ReportStringId.RibbonXRDesign_SizeToControlWidth_Caption, "Make Same Width");
			AddString(ReportStringId.RibbonXRDesign_SizeToGrid_Caption, "Size to Grid");
			AddString(ReportStringId.RibbonXRDesign_SizeToControlHeight_Caption, "Make Same Height");
			AddString(ReportStringId.RibbonXRDesign_SizeToControl_Caption, "Make Same Size");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceMakeEqual_Caption, "Make Horizontal Spacing Equal");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceIncrease_Caption, "Increase Horizontal Spacing");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceDecrease_Caption, "Decrease Horizontal Spacing");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceConcatenate_Caption, "Remove Horizontal Spacing");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceMakeEqual_Caption, "Make Vertical Spacing Equal");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceIncrease_Caption, "Increase Vertical Spacing");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceDecrease_Caption, "Decrease Vertical Spacing");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceConcatenate_Caption, "Remove Vertical Spacing");
			AddString(ReportStringId.RibbonXRDesign_CenterHorizontally_Caption, "Center Horizontally");
			AddString(ReportStringId.RibbonXRDesign_CenterVertically_Caption, "Center Vertically");
			AddString(ReportStringId.RibbonXRDesign_BringToFront_Caption, "Bring to Front");
			AddString(ReportStringId.RibbonXRDesign_SendToBack_Caption, "Send to Back");
			AddString(ReportStringId.RibbonXRDesign_FontBold_Caption, "Bold");
			AddString(ReportStringId.RibbonXRDesign_FontItalic_Caption, "Italic");
			AddString(ReportStringId.RibbonXRDesign_FontUnderline_Caption, "Underline");
			AddString(ReportStringId.RibbonXRDesign_ForeColor_Caption, "Foreground Color");
			AddString(ReportStringId.RibbonXRDesign_BackColor_Caption, "Background Color");
			AddString(ReportStringId.RibbonXRDesign_JustifyLeft_Caption, "Align Text Left");
			AddString(ReportStringId.RibbonXRDesign_JustifyCenter_Caption, "Center Text");
			AddString(ReportStringId.RibbonXRDesign_JustifyRight_Caption, "Align Text Right");
			AddString(ReportStringId.RibbonXRDesign_JustifyJustify_Caption, "Justify");
			AddString(ReportStringId.RibbonXRDesign_NewReport_Caption, "New Report");
			AddString(ReportStringId.RibbonXRDesign_NewReportWizard_Caption, "New Report via Wizard...");
			AddString(ReportStringId.RibbonXRDesign_OpenFile_Caption, "Open...");
			AddString(ReportStringId.RibbonXRDesign_SaveFile_Caption, "Save");
			AddString(ReportStringId.RibbonXRDesign_SaveFileAs_Caption, "Save As...");
			AddString(ReportStringId.RibbonXRDesign_SaveAll_Caption, "Save All");
			AddString(ReportStringId.RibbonXRDesign_Cut_Caption, "Cut");
			AddString(ReportStringId.RibbonXRDesign_Copy_Caption, "Copy");
			AddString(ReportStringId.RibbonXRDesign_Paste_Caption, "Paste");
			AddString(ReportStringId.RibbonXRDesign_Undo_Caption, "Undo");
			AddString(ReportStringId.RibbonXRDesign_Redo_Caption, "Redo");
			AddString(ReportStringId.RibbonXRDesign_Exit_Caption, "Exit");
			AddString(ReportStringId.RibbonXRDesign_Close_Caption, "Close");
			AddString(ReportStringId.RibbonXRDesign_Zoom_Caption, "Zoom");
			AddString(ReportStringId.RibbonXRDesign_ZoomIn_Caption, "Zoom In");
			AddString(ReportStringId.RibbonXRDesign_ZoomOut_Caption, "Zoom Out");
			AddString(ReportStringId.RibbonXRDesign_ZoomExact_Caption, "Exact:");
			AddString(ReportStringId.RibbonXRDesign_Windows_Caption, "Windows");
			AddString(ReportStringId.RibbonXRDesign_Scripts_Caption, "Scripts");
			AddString(ReportStringId.RibbonXRDesign_HtmlHome_Caption, "Home");
			AddString(ReportStringId.RibbonXRDesign_HtmlBackward_Caption, "Back");
			AddString(ReportStringId.RibbonXRDesign_HtmlForward_Caption, "Forward");
			AddString(ReportStringId.RibbonXRDesign_HtmlRefresh_Caption, "Refresh");
			AddString(ReportStringId.RibbonXRDesign_HtmlFind_Caption, "Find");
			AddString(ReportStringId.RibbonXRDesign_AlignToGrid_STipTitle, "Align to Grid");
			AddString(ReportStringId.RibbonXRDesign_AlignLeft_STipTitle, "Align Lefts");
			AddString(ReportStringId.RibbonXRDesign_AlignVerticalCenters_STipTitle, "Align Centers");
			AddString(ReportStringId.RibbonXRDesign_AlignRight_STipTitle, "Align Rights");
			AddString(ReportStringId.RibbonXRDesign_AlignTop_STipTitle, "Align Tops");
			AddString(ReportStringId.RibbonXRDesign_AlignHorizontalCenters_STipTitle, "Align Middles");
			AddString(ReportStringId.RibbonXRDesign_AlignBottom_STipTitle, "Align Bottoms");
			AddString(ReportStringId.RibbonXRDesign_SizeToControlWidth_STipTitle, "Make Same Width");
			AddString(ReportStringId.RibbonXRDesign_SizeToGrid_STipTitle, "Size to Grid");
			AddString(ReportStringId.RibbonXRDesign_SizeToControlHeight_STipTitle, "Make Same Height");
			AddString(ReportStringId.RibbonXRDesign_SizeToControl_STipTitle, "Make Same Size");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceMakeEqual_STipTitle, "Make Horizontal Spacing Equal");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceIncrease_STipTitle, "Increase Horizontal Spacing");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceDecrease_STipTitle, "Decrease Horizontal Spacing");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceConcatenate_STipTitle, "Remove Horizontal Spacing");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceMakeEqual_STipTitle, "Make Vertical Spacing Equal");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceIncrease_STipTitle, "Increase Vertical Spacing");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceDecrease_STipTitle, "Decrease Vertical Spacing");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceConcatenate_STipTitle, "Remove Vertical Spacing");
			AddString(ReportStringId.RibbonXRDesign_CenterHorizontally_STipTitle, "Center Horizontally");
			AddString(ReportStringId.RibbonXRDesign_CenterVertically_STipTitle, "Center Vertically");
			AddString(ReportStringId.RibbonXRDesign_BringToFront_STipTitle, "Bring to Front");
			AddString(ReportStringId.RibbonXRDesign_SendToBack_STipTitle, "Send to Back");
			AddString(ReportStringId.RibbonXRDesign_FontBold_STipTitle, "Bold");
			AddString(ReportStringId.RibbonXRDesign_FontItalic_STipTitle, "Italic");
			AddString(ReportStringId.RibbonXRDesign_FontUnderline_STipTitle, "Underline");
			AddString(ReportStringId.RibbonXRDesign_ForeColor_STipTitle, "Foreground Color");
			AddString(ReportStringId.RibbonXRDesign_BackColor_STipTitle, "Background Color");
			AddString(ReportStringId.RibbonXRDesign_JustifyLeft_STipTitle, "Align Text Left");
			AddString(ReportStringId.RibbonXRDesign_JustifyCenter_STipTitle, "Center Text");
			AddString(ReportStringId.RibbonXRDesign_JustifyRight_STipTitle, "Align Text Right");
			AddString(ReportStringId.RibbonXRDesign_JustifyJustify_STipTitle, "Justify");
			AddString(ReportStringId.RibbonXRDesign_NewReport_STipTitle, "New Blank Report");
			AddString(ReportStringId.RibbonXRDesign_NewReportWizard_STipTitle, "New Report via Wizard");
			AddString(ReportStringId.RibbonXRDesign_OpenFile_STipTitle, "Open Report");
			AddString(ReportStringId.RibbonXRDesign_SaveFile_STipTitle, "Save Report");
			AddString(ReportStringId.RibbonXRDesign_SaveFileAs_STipTitle, "Save Report As");
			AddString(ReportStringId.RibbonXRDesign_SaveAll_STipTitle, "Save All Reports");
			AddString(ReportStringId.RibbonXRDesign_Cut_STipTitle, "Cut");
			AddString(ReportStringId.RibbonXRDesign_Copy_STipTitle, "Copy");
			AddString(ReportStringId.RibbonXRDesign_Paste_STipTitle, "Paste");
			AddString(ReportStringId.RibbonXRDesign_Undo_STipTitle, "Undo");
			AddString(ReportStringId.RibbonXRDesign_Redo_STipTitle, "Redo");
			AddString(ReportStringId.RibbonXRDesign_Exit_STipTitle, "Exit");
			AddString(ReportStringId.RibbonXRDesign_Close_STipTitle, "Close");
			AddString(ReportStringId.RibbonXRDesign_Zoom_STipTitle, "Zoom");
			AddString(ReportStringId.RibbonXRDesign_ZoomIn_STipTitle, "Zoom In");
			AddString(ReportStringId.RibbonXRDesign_ZoomOut_STipTitle, "Zoom Out");
			AddString(ReportStringId.RibbonXRDesign_FontName_STipTitle, "Font");
			AddString(ReportStringId.RibbonXRDesign_FontSize_STipTitle, "Font Size");
			AddString(ReportStringId.RibbonXRDesign_Windows_STipTitle, "Show/Hide Windows");
			AddString(ReportStringId.RibbonXRDesign_Scripts_STipTitle, "Show/Hide Scripts");
			AddString(ReportStringId.RibbonXRDesign_HtmlHome_STipTitle, "Home");
			AddString(ReportStringId.RibbonXRDesign_HtmlBackward_STipTitle, "Back");
			AddString(ReportStringId.RibbonXRDesign_HtmlForward_STipTitle, "Forward");
			AddString(ReportStringId.RibbonXRDesign_HtmlRefresh_STipTitle, "Refresh");
			AddString(ReportStringId.RibbonXRDesign_HtmlFind_STipTitle, "Find");
			AddString(ReportStringId.RibbonXRDesign_AlignToGrid_STipContent, "Align the positions of the selected controls to the grid.");
			AddString(ReportStringId.RibbonXRDesign_AlignLeft_STipContent, "Left align the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_AlignVerticalCenters_STipContent, "Align the centers of the selected controls vertically.");
			AddString(ReportStringId.RibbonXRDesign_AlignRight_STipContent, "Right align the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_AlignTop_STipContent, "Align the tops of the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_AlignHorizontalCenters_STipContent, "Align the centers of the selected controls horizontally.");
			AddString(ReportStringId.RibbonXRDesign_AlignBottom_STipContent, "Align the bottoms of the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_SizeToControlWidth_STipContent, "Make the selected controls have the same width.");
			AddString(ReportStringId.RibbonXRDesign_SizeToGrid_STipContent, "Size the selected controls to the grid.");
			AddString(ReportStringId.RibbonXRDesign_SizeToControlHeight_STipContent, "Make the selected controls have the same height.");
			AddString(ReportStringId.RibbonXRDesign_SizeToControl_STipContent, "Make the selected controls have the same size.");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceMakeEqual_STipContent, "Make the horizontal spacing between the selected controls equal.");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceIncrease_STipContent, "Increase the horizontal spacing between the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceDecrease_STipContent, "Decrease the horizontal spacing between the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_HorizSpaceConcatenate_STipContent, "Remove the horizontal spacing between the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceMakeEqual_STipContent, "Make the vertical spacing between the selected controls equal.");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceIncrease_STipContent, "Increase the vertical spacing between the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceDecrease_STipContent, "Decrease the vertical spacing between the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_VertSpaceConcatenate_STipContent, "Remove the vertical spacing between the selected controls.");
			AddString(ReportStringId.RibbonXRDesign_CenterHorizontally_STipContent, "Horizontally center the selected controls within a band.");
			AddString(ReportStringId.RibbonXRDesign_CenterVertically_STipContent, "Vertically center the selected controls within a band.");
			AddString(ReportStringId.RibbonXRDesign_BringToFront_STipContent, "Bring the selected controls to the front.");
			AddString(ReportStringId.RibbonXRDesign_SendToBack_STipContent, "Move the selected controls to the back.");
			AddString(ReportStringId.RibbonXRDesign_FontBold_STipContent, "Make the selected text bold.");
			AddString(ReportStringId.RibbonXRDesign_FontItalic_STipContent, "Italicize the text.");
			AddString(ReportStringId.RibbonXRDesign_FontUnderline_STipContent, "Underline the selected text.");
			AddString(ReportStringId.RibbonXRDesign_ForeColor_STipContent, "Change the text foreground color.");
			AddString(ReportStringId.RibbonXRDesign_BackColor_STipContent, "Change the text background color.");
			AddString(ReportStringId.RibbonXRDesign_JustifyLeft_STipContent, "Align text to the left.");
			AddString(ReportStringId.RibbonXRDesign_JustifyCenter_STipContent, "Center text.");
			AddString(ReportStringId.RibbonXRDesign_JustifyRight_STipContent, "Align text to the right.");
			AddString(ReportStringId.RibbonXRDesign_JustifyJustify_STipContent, "Align text to both the left and right sides, adding extra space between words as necessary.");
			AddString(ReportStringId.RibbonXRDesign_Close_STipContent, "Close the current report.");
			AddString(ReportStringId.RibbonXRDesign_Exit_STipContent, "Close the report designer.");
			AddString(ReportStringId.RibbonXRDesign_NewReport_STipContent, "Create a new blank report.");
			AddString(ReportStringId.RibbonXRDesign_NewReportWizard_STipContent, "Launch the report wizard to create a new report.");
			AddString(ReportStringId.RibbonXRDesign_OpenFile_STipContent, "Open a report.");
			AddString(ReportStringId.RibbonXRDesign_SaveFile_STipContent, "Save the current report.");
			AddString(ReportStringId.RibbonXRDesign_SaveFileAs_STipContent, "Save the current report with a new name.");
			AddString(ReportStringId.RibbonXRDesign_SaveAll_STipContent, "Save all modified reports.");
			AddString(ReportStringId.RibbonXRDesign_Cut_STipContent, "Cut the selected controls from the report and put them on the Clipboard.");
			AddString(ReportStringId.RibbonXRDesign_Copy_STipContent, "Copy the selected controls and put them on the Clipboard.");
			AddString(ReportStringId.RibbonXRDesign_Paste_STipContent, "Paste the contents of the Clipboard.");
			AddString(ReportStringId.RibbonXRDesign_Undo_STipContent, "Undo the last operation.");
			AddString(ReportStringId.RibbonXRDesign_Redo_STipContent, "Redo the last operation.");
			AddString(ReportStringId.RibbonXRDesign_Zoom_STipContent, "Change the zoom level of the document designer.");
			AddString(ReportStringId.RibbonXRDesign_ZoomIn_STipContent, "Zoom in to get a close-up view of the report.");
			AddString(ReportStringId.RibbonXRDesign_ZoomOut_STipContent, "Zoom out to see more of the report at a reduced size.");
			AddString(ReportStringId.RibbonXRDesign_FontName_STipContent, "Change the font face.");
			AddString(ReportStringId.RibbonXRDesign_FontSize_STipContent, "Change the font size.");
			AddString(ReportStringId.RibbonXRDesign_Windows_STipContent, "Show or hide the Tool Box, Report Explorer, Field List and Property Grid windows.");
			AddString(ReportStringId.RibbonXRDesign_Scripts_STipContent, "Show or hide the Scripts Editor.");
			AddString(ReportStringId.RibbonXRDesign_HtmlHome_STipContent, "Display the home page.");
			AddString(ReportStringId.RibbonXRDesign_HtmlBackward_STipContent, "Move back to the previous page.");
			AddString(ReportStringId.RibbonXRDesign_HtmlForward_STipContent, "Move forward to the next page.");
			AddString(ReportStringId.RibbonXRDesign_HtmlRefresh_STipContent, "Refresh this page.");
			AddString(ReportStringId.RibbonXRDesign_HtmlFind_STipContent, "Find the text on this page.");
			AddString(ReportStringId.RibbonXRDesign_NewReport_Description, "Create a new blank report.");
			AddString(ReportStringId.RibbonXRDesign_NewReportWizard_Description, "Launch the report wizard to create a new report.");
			AddString(ReportStringId.RibbonXRDesign_SaveFile_Description, "Save the current report.");
			AddString(ReportStringId.RibbonXRDesign_SaveFileAs_Description, "Save the current report with a new name.");
			AddString(ReportStringId.XRSubreport_NameInfo, "Name: {0}\r\n");
			AddString(ReportStringId.XRSubreport_NullReportSourceInfo, "Null");
			AddString(ReportStringId.XRSubreport_ReportSourceInfo, "Report Source: {0}\r\n");
			AddString(ReportStringId.XRSubreport_ReportSourceUrlInfo, "Report Source Url: {0}\r\n");
			AddString(ReportStringId.PivotGridForm_GroupMain_Caption, "Main");
			AddString(ReportStringId.PivotGridForm_GroupMain_Description, "Main settings(Fields, Layout).");
			AddString(ReportStringId.PivotGridForm_ItemFields_Caption, "Fields");
			AddString(ReportStringId.PivotGridForm_ItemFields_Description, "Manage fields.");
			AddString(ReportStringId.PivotGridForm_ItemLayout_Caption, "Layout");
			AddString(ReportStringId.PivotGridForm_ItemLayout_Description, "Customize the current XRPivotGrid's layout and preview its data.");
			AddString(ReportStringId.PivotGridForm_GroupPrinting_Caption, "Printing");
			AddString(ReportStringId.PivotGridForm_GroupPrinting_Description, "Printing option management for the current XRPivotGrid.");
			AddString(ReportStringId.PivotGridForm_ItemAppearances_Caption, "Appearances");
			AddString(ReportStringId.PivotGridForm_ItemAppearances_Description, "Adjust the print appearances of the current XRPivotGrid.");
			AddString(ReportStringId.PivotGridForm_ItemSettings_Caption, "Printing Settings");
			AddString(ReportStringId.PivotGridForm_ItemSettings_Description, "Adjust the printing settings for the current XRPivotGrid.");
			AddString(ReportStringId.PivotGridFrame_Fields_ColumnsText, "XRPivotGrid Fields");
			AddString(ReportStringId.PivotGridFrame_Fields_DescriptionText1, "You can add and delete XRPivotGrid fields and modify their settings.");
			AddString(ReportStringId.PivotGridFrame_Fields_DescriptionText2, "Select and drag field to the PivotGrid fields panel to create PivotGrid field.");
			AddString(ReportStringId.PivotGridFrame_Layouts_DescriptionText, "Modify the XRPivotGrid's layout (sorting settings, field arrangement) and click the Apply button to apply the modifications to the current XRPivotGrid. You can also save the layout to an XML file (this can be loaded and applied to other views at design time and runtime).");
			AddString(ReportStringId.PivotGridFrame_Layouts_SelectorCaption1, "Hide fields &selector");
			AddString(ReportStringId.PivotGridFrame_Layouts_SelectorCaption2, "Show fields &selector");
			AddString(ReportStringId.PivotGridFrame_Appearances_DescriptionText, "Select one or more of the Appearance objects to customize the printing appearances of the corresponding visual elements.");
			AddString(ReportStringId.DesignerStatus_Location, "Loc");
			AddString(ReportStringId.DesignerStatus_Size, "Size");
			AddString(ReportStringId.DesignerStatus_Height, "Height");
			AddString(ReportStringId.Wizard_PageChooseFields_Msg, "You must select fields for the report before you continue");
			AddString(ReportStringId.GroupSort_AddGroup, "Add a Group");
			AddString(ReportStringId.GroupSort_AddSort, "Add a Sort");
			AddString(ReportStringId.GroupSort_MoveUp, "Move Up");
			AddString(ReportStringId.GroupSort_MoveDown, "Move Down");
			AddString(ReportStringId.GroupSort_Delete, "Delete");
			AddString(ReportStringId.NewParameterEditorForm_DataSource, "Data source:");
			AddString(ReportStringId.NewParameterEditorForm_DataAdapter, "Data adapter:");
			AddString(ReportStringId.NewParameterEditorForm_DataMember, "Data member:");
			AddString(ReportStringId.NewParameterEditorForm_ValueMember, "Value member:");
			AddString(ReportStringId.NewParameterEditorForm_DisplayMember, "Display member:");
			AddString(ReportStringId.BindingMapperForm_InvalidBindingWarning, "Invalid binding");
			AddString(ReportStringId.BindingMapperForm_ShowOnlyInvalidBindings, "Show only invalid bindings");
			AddString(ReportStringId.SubreportDesigner_EditParameterBindings, "Edit Parameter Bindings...");
		}
	}
	 #endregion
}
