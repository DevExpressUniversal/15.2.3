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

using DevExpress.Utils.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Localization {
	public enum ASPxSpreadsheetStringId {
		InsertImage,
		InsertImage_FromWeb,
		InsertImage_FromLocal,
		InsertImage_EnterUrl,
		InsertImage_UploadInstructions,
		InsertImage_ImagePreview,
		InsertImage_ImageManager_Header,
		InsertLink,
		ChangeLink,
		InsertLink_URL,
		InsertLink_Email,
		InsertLink_EmailTo,
		InsertLink_Subject,
		InsertLink_DisplayProperties,
		InsertLink_Text,
		InsertLink_ToolTip,
		OpenFile,
		OpenFile_FromServer,
		OpenFile_FromComputer,
		OpenFile_ChooseInstruction,
		OpenFile_UploadInstruction,
		OpenFile_FileManager_Header,
		RenameSheet,
		RenameSheet_SheetName,
		SaveFile,
		SaveFile_FileAlreadyExists,
		SaveFile_SaveFileToServer,
		SaveFile_DownloadCopy,
		SaveFile_FolderPath,
		SaveFile_FileName,
		SaveFile_FileType,
		SaveFile_DownloadInstruction,
		SaveFile_FolderManager_Header,
		RowHeight,
		RowHeight_Caption,
		ColumnWidth,
		ColumnWidth_Caption,
		DefaultColumnWidth,
		DefaultColumnWidth_Caption,
		UnhideSheet,
		UnhideSheet_Caption,
		ChangeChartType,
		ChartSelectData,
		ChartSelectData_Caption,
		ModifyChartLayout,
		ChartChangeTitle,
		ChartChangeTitle_Caption,
		ChartChangeHorizontalAxisTitle,
		ChartChangeHorizontalAxisTitle_Caption,
		ChartChangeVerticalAxisTitle,
		ChartChangeVerticalAxisTitle_Caption,
		ModifyChartStyle,
		FindAndReplace,
		FindAndReplace_FindTab_Caption,
		FindAndReplace_FindWhat,
		FindAndReplace_MatchCase,
		FindAndReplace_Search,
		FindAndReplace_Search_ByRows,
		FindAndReplace_Search_ByColumns,
		FindAndReplace_LookIn,
		FindAndReplace_LookIn_Formulas,
		FindAndReplace_LookIn_Values,
		FindAndReplace_MatchCellContent,
		FindAndReplace_SearchResults,
		FindAndReplace_SearchResults_CellHeader,
		FindAndReplace_SearchResults_ValueHeader,
		ButtonInsert,
		ButtonChange,
		ButtonCancel,
		ButtonOK,
		ButtonOpen,
		ButtonSaveAs,
		ButtonSelect,
		ButtonDownload,
		ButtonFindAll,
		Invalid_FileName,
		Invalid_EMail,
		Invalid_URL,
		BorderLineStyle_Thin,
		BorderLineStyle_Dashed,
		BorderLineStyle_Dotted,
		BorderLineStyle_Double,
		BorderLineStyle_Medium,
		BorderLineStyle_MediumDashed,
		BorderLineStyle_MediumDotted,
		BorderLineStyle_Thick,
		ViewGroup_Title,
		FullScreenCommand_Text,
		FullScreenCommand_Description,
		RequiredFieldError,
		ExternalRibbonControl_NotFound,
		ConfirmOnLosingChanges,
		DataFilterSimple,
		TableSelectData,
		TableSelectData_Caption,
		TableSelectData_TableHasHeaders,
		MoveOrCopySheet,
		MoveOrCopySheet_MoveSheetsCaption,
		MoveOrCopySheet_CreateCopy,
		DataValidation,
		ModifyTableStyle,
		Caption_LightTableStyleCategory,
		Caption_MediumTableStyleCategory,
		Caption_DarkTableStyleCategory,
		Caption_PrefixTableStyleNamePart,
		Caption_LightTableStyleNamePart,
		Caption_MediumTableStyleNamePart,
		Caption_DarkTableStyleNamePart,
		PageSetup,
		PageSetup_PageTab_Caption,
		PageSetup_PageTab_OrientationGroup,
		PageSetup_PageTab_OrientationPortrait,
		PageSetup_PageTab_OrientationLandscape,
		PageSetup_PageTab_ScalingGroup,
		PageSetup_PageTab_AdjustTo,
		PageSetup_PageTab_Scale,
		PageSetup_PageTab_FitTo,
		PageSetup_PageTab_FitToWidth,
		PageSetup_PageTab_FitToHeight,
		PageSetup_PageTab_PaperSize,
		PageSetup_PageTab_PrintQuality,
		PageSetup_PageTab_FirstPageNumber,
		PageSetup_MarginsTab_Caption,
		PageSetup_MarginsTab_Left,
		PageSetup_MarginsTab_Top,
		PageSetup_MarginsTab_Header,
		PageSetup_MarginsTab_Right,
		PageSetup_MarginsTab_Footer,
		PageSetup_MarginsTab_Bottom,
		PageSetup_MarginsTab_CenterOnPageGroup,
		PageSetup_MarginsTab_CenterHorizontally,
		PageSetup_MarginsTab_CenterVertically,
		PageSetup_HeaderFooterTab_Caption,
		PageSetup_HeaderFooterTab_Header,
		PageSetup_HeaderFooterTab_Footer,
		PageSetup_HeaderFooterTab_DifferentOddEven,
		PageSetup_HeaderFooterTab_DifferentFirst,
		PageSetup_HeaderFooterTab_ScaleWithDoc,
		PageSetup_HeaderFooterTab_AlignWithMargins,
		PageSetup_SheetTab_Caption,
		PageSetup_SheetTab_PrintArea,
		PageSetup_SheetTab_PrintAreaErrorText,
		PageSetup_SheetTab_PrintGroup,
		PageSetup_SheetTab_Gridlines,
		PageSetup_SheetTab_Draft,
		PageSetup_SheetTab_PrintHeadings,
		PageSetup_SheetTab_Comments,
		PageSetup_SheetTab_CellErrorAs,
		PageSetup_SheetTab_PageOrderGroup,
		PageSetup_SheetTab_PageOrder_DownThenOver,
		PageSetup_SheetTab_PageOrder_OverThenDown,
		PageSetup_Print,
		FormulaBar_EnterButtonTooltip,
		FormulaBar_CancelButtonTooltip,
		FilterSimple_CheckAll,
		FilterSimple_UncheckAll,
		FilterTop10_Show,
		CustomFilter_ShowRows,
		CustomFilter_OperatorAnd,
		CustomFilter_OperatorOr,
		CustomFilter_QuestionSignDescription,
		CustomFilter_StarSignDescription
	}
	public class ASPxSpreadsheetResourcesLocalizer : ASPxResLocalizerBase<ASPxSpreadsheetStringId> {
		public ASPxSpreadsheetResourcesLocalizer()
			: base(new ASPxSpreadsheetLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName {
			get { return AssemblyInfo.SRAssemblyWebSpreadsheet; }
		}
		protected override string ResxName {
			get { return "DevExpress.Web.ASPxSpreadsheet.LocalizationRes"; }
		}
	}
	public class ASPxSpreadsheetLocalizer : XtraLocalizer<ASPxSpreadsheetStringId> {
		static ASPxSpreadsheetLocalizer() {
			SetActiveLocalizerProvider(new ASPxActiveLocalizerProvider<ASPxSpreadsheetStringId>(CreateResLocalizerInstance));
		}
		static XtraLocalizer<ASPxSpreadsheetStringId> CreateResLocalizerInstance() {
			return new ASPxSpreadsheetResourcesLocalizer();
		}
		public static string GetString(ASPxSpreadsheetStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxSpreadsheetStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxSpreadsheetStringId.InsertImage, StringResources.Spreadsheet_InsertImage);
			AddString(ASPxSpreadsheetStringId.InsertImage_FromWeb, StringResources.Spreadsheet_InsertImage_FromWeb);
			AddString(ASPxSpreadsheetStringId.InsertImage_FromLocal, StringResources.Spreadsheet_InsertImage_FromLocal);
			AddString(ASPxSpreadsheetStringId.InsertImage_EnterUrl, StringResources.Spreadsheet_InsertImage_EnterUrl);
			AddString(ASPxSpreadsheetStringId.InsertImage_UploadInstructions, StringResources.Spreadsheet_InsertImage_UploadInstructions);
			AddString(ASPxSpreadsheetStringId.InsertImage_ImagePreview, StringResources.Spreadsheet_InsertImage_ImagePreview);
			AddString(ASPxSpreadsheetStringId.InsertImage_ImageManager_Header, StringResources.Spreadsheet_InsertImage_ImageManager_Header);
			AddString(ASPxSpreadsheetStringId.InsertLink, StringResources.Spreadsheet_InsertLink);
			AddString(ASPxSpreadsheetStringId.ChangeLink, StringResources.Spreadsheet_ChangeLink);
			AddString(ASPxSpreadsheetStringId.InsertLink_DisplayProperties, StringResources.Spreadsheet_InsertLink_DisplayProperties);
			AddString(ASPxSpreadsheetStringId.InsertLink_Email, StringResources.Spreadsheet_InsertLink_Email);
			AddString(ASPxSpreadsheetStringId.InsertLink_EmailTo, StringResources.Spreadsheet_InsertLink_EmailTo);
			AddString(ASPxSpreadsheetStringId.InsertLink_Subject, StringResources.Spreadsheet_InsertLink_Subject);
			AddString(ASPxSpreadsheetStringId.InsertLink_Text, StringResources.Spreadsheet_InsertLink_Text);
			AddString(ASPxSpreadsheetStringId.InsertLink_ToolTip, StringResources.Spreadsheet_InsertLink_ToolTip);
			AddString(ASPxSpreadsheetStringId.InsertLink_URL, StringResources.Spreadsheet_InsertLink_URL);
			AddString(ASPxSpreadsheetStringId.OpenFile, StringResources.Spreadsheet_OpenFile);
			AddString(ASPxSpreadsheetStringId.OpenFile_ChooseInstruction, StringResources.Spreadsheet_OpenFile_ChooseInstruction);
			AddString(ASPxSpreadsheetStringId.OpenFile_FileManager_Header, StringResources.Spreadsheet_OpenFile_FileManager_Header);
			AddString(ASPxSpreadsheetStringId.OpenFile_FromComputer, StringResources.Spreadsheet_OpenFile_FromComputer);
			AddString(ASPxSpreadsheetStringId.OpenFile_FromServer, StringResources.Spreadsheet_OpenFile_FromServer);
			AddString(ASPxSpreadsheetStringId.OpenFile_UploadInstruction, StringResources.Spreadsheet_OpenFile_UploadInstruction);
			AddString(ASPxSpreadsheetStringId.RenameSheet, StringResources.Spreadsheet_RenameSheet);
			AddString(ASPxSpreadsheetStringId.RenameSheet_SheetName, StringResources.Spreadsheet_RenameSheet_SheetName);
			AddString(ASPxSpreadsheetStringId.SaveFile, StringResources.Spreadsheet_SaveFile);
			AddString(ASPxSpreadsheetStringId.SaveFile_DownloadCopy, StringResources.Spreadsheet_SaveFile_DownloadCopy);
			AddString(ASPxSpreadsheetStringId.SaveFile_DownloadInstruction, StringResources.Spreadsheet_SaveFile_DownloadInstruction);
			AddString(ASPxSpreadsheetStringId.SaveFile_FileAlreadyExists, StringResources.Spreadsheet_SaveFile_FileAlreadyExists);
			AddString(ASPxSpreadsheetStringId.SaveFile_FileName, StringResources.Spreadsheet_SaveFile_FileName);
			AddString(ASPxSpreadsheetStringId.SaveFile_FileType, StringResources.Spreadsheet_SaveFile_FileType);
			AddString(ASPxSpreadsheetStringId.SaveFile_FolderManager_Header, StringResources.Spreadsheet_SaveFile_FolderManager_Header);
			AddString(ASPxSpreadsheetStringId.SaveFile_FolderPath, StringResources.Spreadsheet_SaveFile_FolderPath);
			AddString(ASPxSpreadsheetStringId.SaveFile_SaveFileToServer, StringResources.Spreadsheet_SaveFile_SaveFileToServer);
			AddString(ASPxSpreadsheetStringId.RowHeight, StringResources.Spreadsheet_RowHeight);
			AddString(ASPxSpreadsheetStringId.RowHeight_Caption, StringResources.Spreadsheet_RowHeight_Caption);
			AddString(ASPxSpreadsheetStringId.ColumnWidth, StringResources.Spreadsheet_ColumnWidth);
			AddString(ASPxSpreadsheetStringId.ColumnWidth_Caption, StringResources.Spreadsheet_ColumnWidth_Caption);
			AddString(ASPxSpreadsheetStringId.DefaultColumnWidth, StringResources.Spreadsheet_DefaultColumnWidth);
			AddString(ASPxSpreadsheetStringId.DefaultColumnWidth_Caption, StringResources.Spreadsheet_DefaultColumnWidth_Caption);
			AddString(ASPxSpreadsheetStringId.UnhideSheet, StringResources.Spreadsheet_UnhideSheet);
			AddString(ASPxSpreadsheetStringId.UnhideSheet_Caption, StringResources.Spreadsheet_UnhideSheet_Caption);
			AddString(ASPxSpreadsheetStringId.ChangeChartType, StringResources.Spreadsheet_ChangeChartType);
			AddString(ASPxSpreadsheetStringId.ChartSelectData, StringResources.Spreadsheet_ChartSelectData);
			AddString(ASPxSpreadsheetStringId.ChartSelectData_Caption, StringResources.Spreadsheet_ChartSelectData_Caption);
			AddString(ASPxSpreadsheetStringId.ModifyChartLayout, StringResources.Spreadsheet_ModifyChartLayout);
			AddString(ASPxSpreadsheetStringId.FindAndReplace, StringResources.Spreadsheet_FindAndReplace);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_FindTab_Caption,		   StringResources.Spreadsheet_FindAndReplace_FindTab_Caption);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_FindWhat,				  StringResources.Spreadsheet_FindAndReplace_FindWhat);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_MatchCase,				 StringResources.Spreadsheet_FindAndReplace_MatchCase);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_Search,					StringResources.Spreadsheet_FindAndReplace_Search);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_Search_ByRows,			 StringResources.Spreadsheet_FindAndReplace_Search_ByRows);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_Search_ByColumns,		  StringResources.Spreadsheet_FindAndReplace_Search_ByColumns);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_LookIn,					StringResources.Spreadsheet_FindAndReplace_LookIn);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_LookIn_Formulas,		   StringResources.Spreadsheet_FindAndReplace_LookIn_Formulas);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_LookIn_Values,			 StringResources.Spreadsheet_FindAndReplace_LookIn_Values);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_MatchCellContent,		  StringResources.Spreadsheet_FindAndReplace_MatchCellContent);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_SearchResults,			 StringResources.Spreadsheet_FindAndReplace_SearchResults);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_SearchResults_CellHeader,  StringResources.Spreadsheet_FindAndReplace_SearchResults_CellHeader);
			AddString(ASPxSpreadsheetStringId.FindAndReplace_SearchResults_ValueHeader, StringResources.Spreadsheet_FindAndReplace_SearchResults_ValueHeader);
			AddString(ASPxSpreadsheetStringId.ChartChangeTitle, StringResources.Spreadsheet_ChartChangeTitle);
			AddString(ASPxSpreadsheetStringId.ChartChangeTitle_Caption, StringResources.Spreadsheet_ChartChangeTitle_Caption);
			AddString(ASPxSpreadsheetStringId.ChartChangeHorizontalAxisTitle, StringResources.Spreadsheet_ChartChangeHorizontalAxisTitle);
			AddString(ASPxSpreadsheetStringId.ChartChangeHorizontalAxisTitle_Caption, StringResources.Spreadsheet_ChartChangeHorizontalAxisTitle_Caption);
			AddString(ASPxSpreadsheetStringId.ChartChangeVerticalAxisTitle, StringResources.Spreadsheet_ChartChangeVerticalAxisTitle);
			AddString(ASPxSpreadsheetStringId.ChartChangeVerticalAxisTitle_Caption, StringResources.Spreadsheet_ChartChangeVerticalAxisTitle_Caption);
			AddString(ASPxSpreadsheetStringId.ModifyChartStyle, StringResources.Spreadsheet_ModifyChartStyle);
			AddString(ASPxSpreadsheetStringId.ButtonInsert, StringResources.Spreadsheet_ButtonInsert);
			AddString(ASPxSpreadsheetStringId.ButtonChange, StringResources.Spreadsheet_ButtonChange);
			AddString(ASPxSpreadsheetStringId.ButtonCancel, StringResources.Spreadsheet_ButtonCancel);
			AddString(ASPxSpreadsheetStringId.ButtonOK, StringResources.Spreadsheet_ButtonOK);
			AddString(ASPxSpreadsheetStringId.ButtonOpen, StringResources.Spreadsheet_ButtonOpen);
			AddString(ASPxSpreadsheetStringId.ButtonSaveAs, StringResources.Spreadsheet_ButtonSaveAs);
			AddString(ASPxSpreadsheetStringId.ButtonSelect, StringResources.Spreadsheet_ButtonSelect);
			AddString(ASPxSpreadsheetStringId.ButtonDownload, StringResources.Spreadsheet_ButtonDownload);
			AddString(ASPxSpreadsheetStringId.ButtonFindAll, StringResources.Spreadsheet_ButtonFindAll);
			AddString(ASPxSpreadsheetStringId.Invalid_FileName, StringResources.Spreadsheet_Invalid_FileName);
			AddString(ASPxSpreadsheetStringId.Invalid_EMail, StringResources.Spreadsheet_Invalid_EMail);
			AddString(ASPxSpreadsheetStringId.Invalid_URL, StringResources.Spreadsheet_Invalid_URL);
			AddString(ASPxSpreadsheetStringId.BorderLineStyle_Thin, StringResources.Spreadsheet_BorderLineStyle_Thin);
			AddString(ASPxSpreadsheetStringId.BorderLineStyle_Dashed, StringResources.Spreadsheet_BorderLineStyle_Dashed);
			AddString(ASPxSpreadsheetStringId.BorderLineStyle_Dotted, StringResources.Spreadsheet_BorderLineStyle_Dotted);
			AddString(ASPxSpreadsheetStringId.BorderLineStyle_Double, StringResources.Spreadsheet_BorderLineStyle_Double);
			AddString(ASPxSpreadsheetStringId.BorderLineStyle_Medium, StringResources.Spreadsheet_BorderLineStyle_Medium);
			AddString(ASPxSpreadsheetStringId.BorderLineStyle_MediumDashed, StringResources.Spreadsheet_BorderLineStyle_MediumDashed);
			AddString(ASPxSpreadsheetStringId.BorderLineStyle_MediumDotted, StringResources.Spreadsheet_BorderLineStyle_MediumDotted);
			AddString(ASPxSpreadsheetStringId.BorderLineStyle_Thick, StringResources.Spreadsheet_BorderLineStyle_Thick);
			AddString(ASPxSpreadsheetStringId.ViewGroup_Title, StringResources.Spreadsheet_ViewGroup_Title);
			AddString(ASPxSpreadsheetStringId.FullScreenCommand_Text, StringResources.Spreadsheet_FullScreenCommand_Text);
			AddString(ASPxSpreadsheetStringId.FullScreenCommand_Description,  StringResources.Spreadsheet_FullScreenCommand_Description);
			AddString(ASPxSpreadsheetStringId.RequiredFieldError, StringResources.Spreadsheet_RequiredFieldError);
			AddString(ASPxSpreadsheetStringId.ExternalRibbonControl_NotFound, StringResources.Spreadsheet_ExternalRibbonControl_NotFound);
			AddString(ASPxSpreadsheetStringId.ConfirmOnLosingChanges, StringResources.Spreadsheet_ConfirmOnLosingChanges);
			AddString(ASPxSpreadsheetStringId.DataFilterSimple, StringResources.Spreadsheet_DataFilterSimple);
			AddString(ASPxSpreadsheetStringId.TableSelectData, StringResources.Spreadsheet_TableSelectData);
			AddString(ASPxSpreadsheetStringId.TableSelectData_Caption, StringResources.Spreadsheet_TableSelectData_Caption);
			AddString(ASPxSpreadsheetStringId.TableSelectData_TableHasHeaders, StringResources.Spreadsheet_TableSelectData_TableHasHeaders);
			AddString(ASPxSpreadsheetStringId.MoveOrCopySheet, StringResources.Spreadsheet_MoveOrCopySheet);
			AddString(ASPxSpreadsheetStringId.MoveOrCopySheet_MoveSheetsCaption, StringResources.Spreadsheet_MoveOrCopySheet_MoveSheetsCaption);
			AddString(ASPxSpreadsheetStringId.MoveOrCopySheet_CreateCopy, StringResources.Spreadsheet_MoveOrCopySheet_CreateCopy);
			AddString(ASPxSpreadsheetStringId.DataValidation, StringResources.Spreadsheet_DataValidation);
			AddString(ASPxSpreadsheetStringId.ModifyTableStyle, StringResources.Spreadsheet_ModifyTableStyle);
			AddString(ASPxSpreadsheetStringId.Caption_PrefixTableStyleNamePart, StringResources.Spreadsheet_Caption_PrefixTableStyleNamePart);
			AddString(ASPxSpreadsheetStringId.Caption_LightTableStyleCategory, StringResources.Spreadsheet_Caption_LightTableStyleCategory);
			AddString(ASPxSpreadsheetStringId.Caption_MediumTableStyleCategory, StringResources.Spreadsheet_Caption_MediumTableStyleCategory);
			AddString(ASPxSpreadsheetStringId.Caption_DarkTableStyleCategory, StringResources.Spreadsheet_Caption_DarkTableStyleCategory);
			AddString(ASPxSpreadsheetStringId.Caption_LightTableStyleNamePart, StringResources.Spreadsheet_Caption_LightTableStyleNamePart);
			AddString(ASPxSpreadsheetStringId.Caption_MediumTableStyleNamePart, StringResources.Spreadsheet_Caption_MediumTableStyleNamePart);
			AddString(ASPxSpreadsheetStringId.Caption_DarkTableStyleNamePart, StringResources.Spreadsheet_Caption_DarkTableStyleNamePart);
			AddString(ASPxSpreadsheetStringId.PageSetup, StringResources.Spreadsheet_PageSetup);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_Caption, StringResources.Spreadsheet_PageSetup_PageTab_Caption);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_OrientationGroup, StringResources.Spreadsheet_PageSetup_PageTab_OrientationGroup);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_OrientationPortrait, StringResources.Spreadsheet_PageSetup_PageTab_OrientationPortrait);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_OrientationLandscape, StringResources.Spreadsheet_PageSetup_PageTab_OrientationLandscape);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_ScalingGroup, StringResources.Spreadsheet_PageSetup_PageTab_ScalingGroup);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_AdjustTo, StringResources.Spreadsheet_PageSetup_PageTab_AdjustTo);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_Scale, StringResources.Spreadsheet_PageSetup_PageTab_Scale);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_FitTo, StringResources.Spreadsheet_PageSetup_PageTab_FitTo);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_FitToWidth, StringResources.Spreadsheet_PageSetup_PageTab_FitToWidth);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_FitToHeight, StringResources.Spreadsheet_PageSetup_PageTab_FitToHeight);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_PaperSize, StringResources.Spreadsheet_PageSetup_PageTab_PaperSize);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_PrintQuality, StringResources.Spreadsheet_PageSetup_PageTab_PrintQuality);
			AddString(ASPxSpreadsheetStringId.PageSetup_PageTab_FirstPageNumber, StringResources.Spreadsheet_PageSetup_PageTab_FirstPageNumber);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_Caption, StringResources.Spreadsheet_PageSetup_MarginsTab_Caption);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_Left, StringResources.Spreadsheet_PageSetup_MarginsTab_Left);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_Top, StringResources.Spreadsheet_PageSetup_MarginsTab_Top);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_Header, StringResources.Spreadsheet_PageSetup_MarginsTab_Header);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_Right, StringResources.Spreadsheet_PageSetup_MarginsTab_Right);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_Footer, StringResources.Spreadsheet_PageSetup_MarginsTab_Footer);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_Bottom, StringResources.Spreadsheet_PageSetup_MarginsTab_Bottom);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_CenterOnPageGroup, StringResources.Spreadsheet_PageSetup_MarginsTab_CenterOnPageGroup);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_CenterHorizontally, StringResources.Spreadsheet_PageSetup_MarginsTab_CenterHorizontally);
			AddString(ASPxSpreadsheetStringId.PageSetup_MarginsTab_CenterVertically, StringResources.Spreadsheet_PageSetup_MarginsTab_CenterVertically);
			AddString(ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_Caption, StringResources.Spreadsheet_PageSetup_HeaderFooterTab_Caption);
			AddString(ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_Header, StringResources.Spreadsheet_PageSetup_HeaderFooterTab_Header);
			AddString(ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_Footer, StringResources.Spreadsheet_PageSetup_HeaderFooterTab_Footer);
			AddString(ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_DifferentOddEven, StringResources.Spreadsheet_PageSetup_HeaderFooterTab_DifferentOddEven);
			AddString(ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_DifferentFirst, StringResources.Spreadsheet_PageSetup_HeaderFooterTab_DifferentFirst);
			AddString(ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_ScaleWithDoc, StringResources.Spreadsheet_PageSetup_HeaderFooterTab_ScaleWithDoc);
			AddString(ASPxSpreadsheetStringId.PageSetup_HeaderFooterTab_AlignWithMargins, StringResources.Spreadsheet_PageSetup_HeaderFooterTab_AlignWithMargins);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_Caption, StringResources.Spreadsheet_PageSetup_SheetTab_Caption);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_PrintArea, StringResources.Spreadsheet_PageSetup_SheetTab_PrintArea);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_PrintAreaErrorText, StringResources.Spreadsheet_PageSetup_SheetTab_PrintAreaErrorText);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_PrintGroup, StringResources.Spreadsheet_PageSetup_SheetTab_PrintGroup);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_Gridlines, StringResources.Spreadsheet_PageSetup_SheetTab_Gridlines);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_Draft, StringResources.Spreadsheet_PageSetup_SheetTab_Draft);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_PrintHeadings, StringResources.Spreadsheet_PageSetup_SheetTab_PrintHeadings);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_Comments, StringResources.Spreadsheet_PageSetup_SheetTab_Comments);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_CellErrorAs, StringResources.Spreadsheet_PageSetup_SheetTab_CellErrorAs);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_PageOrderGroup, StringResources.Spreadsheet_PageSetup_SheetTab_PageOrderGroup);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_PageOrder_DownThenOver, StringResources.Spreadsheet_PageSetup_SheetTab_PageOrder_DownThenOver);
			AddString(ASPxSpreadsheetStringId.PageSetup_SheetTab_PageOrder_OverThenDown, StringResources.Spreadsheet_PageSetup_SheetTab_PageOrder_OverThenDown);
			AddString(ASPxSpreadsheetStringId.PageSetup_Print, StringResources.Spreadsheet_PageSetup_Print);
			AddString(ASPxSpreadsheetStringId.FormulaBar_EnterButtonTooltip, StringResources.Spreadsheet_FormulaBar_EnterButtonTooltip);
			AddString(ASPxSpreadsheetStringId.FormulaBar_CancelButtonTooltip, StringResources.Spreadsheet_FormulaBar_CancelButtonTooltip);
			AddString(ASPxSpreadsheetStringId.FilterSimple_CheckAll, StringResources.Spreadsheet_FilterSimple_CheckAll);
			AddString(ASPxSpreadsheetStringId.FilterSimple_UncheckAll, StringResources.Spreadsheet_FilterSimple_UncheckAll);
			AddString(ASPxSpreadsheetStringId.FilterTop10_Show, StringResources.Spreadsheet_FilterTop10_Show);
			AddString(ASPxSpreadsheetStringId.CustomFilter_ShowRows, StringResources.Spreadsheet_CustomFilter_ShowRows);
			AddString(ASPxSpreadsheetStringId.CustomFilter_OperatorAnd, StringResources.Spreadsheet_CustomFilter_OperatorAnd);
			AddString(ASPxSpreadsheetStringId.CustomFilter_OperatorOr, StringResources.Spreadsheet_CustomFilter_OperatorOr);
			AddString(ASPxSpreadsheetStringId.CustomFilter_QuestionSignDescription, StringResources.Spreadsheet_CustomFilter_QuestionSignDescription);
			AddString(ASPxSpreadsheetStringId.CustomFilter_StarSignDescription, StringResources.Spreadsheet_CustomFilter_StarSignDescription);
		}
	}
}
