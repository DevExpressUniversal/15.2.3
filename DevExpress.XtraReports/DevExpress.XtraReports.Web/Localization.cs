#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.Collections.Generic;
using DevExpress.Utils.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.XtraReports.Web.Localization {
	public enum ASPxReportsStringId {
		SearchDialog_Header,
		SearchDialog_EnterText,
		SearchDialog_FindWhat,
		SearchDialog_FindWhat_AccessKey,
		SearchDialog_FindNext,
		SearchDialog_Cancel,
		SearchDialog_WholeWord,
		SearchDialog_WholeWord_AccessKey,
		SearchDialog_Case,
		SearchDialog_Case_AccessKey,
		SearchDialog_Up,
		SearchDialog_Up_AccessKey,
		SearchDialog_Down,
		SearchDialog_Down_AccessKey,
		SearchDialog_Finished,
		ToolBarItemText_Search,
		ToolBarItemText_PrintReport,
		ToolBarItemText_PrintPage,
		ToolBarItemText_FirstPage,
		ToolBarItemText_PreviousPage,
		ToolBarItemText_NextPage,
		ToolBarItemText_LastPage,
		ToolBarItemText_SaveToWindow,
		ToolBarItemText_SaveToDisk,
		ToolBarItemText_PageLabel,
		ToolBarItemText_OfLabel,
		ParametersPanel_Reset,
		ParametersPanel_Submit,
		ParametersPanel_True,
		ParametersPanel_False,
		ParametersPanel_GuidValidationError,
		ParametersPanel_DateTimeValueValidationError,
		ParametersPanel_GenericRegexValidationError,
		ExportName_pdf,
		ExportName_xls,
		ExportName_xlsx,
		ExportName_rtf,
		ExportName_mht,
		ExportName_html,
		ExportName_txt,
		ExportName_csv,
		ExportName_png,
		DocumentViewer_RemotePageByPage_Error,
		DocumentViewer_RemoteRequestCredentials_Error,
		DocumentViewer_RemoteSourceSettings_Error,
		DocumentViewer_RemoteSourceSettings_CustomTokenStorage_Error,
		DocumentViewer_RemoteSourceSettingsAndConfiguration_Error,
		DocumentViewer_LocalAndRemoteSource_Error,
		DocumentViewer_RemoteAuthenticatorCredential_Error,
		DocumentViewer_RemoteAuthenticatorCredentialHandled_Error,
		DocumentViewer_RemoteAuthenticatorLogin_Error,
		DocumentViewer_NoRemoteDocumentInformation_Error,
		DocumentViewer_RemoteSourceConnection_Error,
		DocumentViewer_ExternalRibbonNotFound_Error,
		DocumentViewer_RibbonHomeTabText,
		DocumentViewer_RibbonPrintGroupText,
		DocumentViewer_RibbonNavigationGroupText,
		DocumentViewer_RibbonExportGroupText,
		DocumentViewer_RibbonReportGroupText,
		DocumentViewer_RibbonCommandText_SaveToWindow,
		DocumentViewer_RibbonCommandText_SaveToFile,
		DocumentViewer_RibbonCommandText_FindText,
		DocumentViewer_RibbonCommandText_FirstPage,
		DocumentViewer_RibbonCommandText_PreviousPage,
		DocumentViewer_RibbonCommandText_NextPage,
		DocumentViewer_RibbonCommandText_LastPage,
		DocumentViewer_RibbonCommandText_PrintReport,
		DocumentViewer_RibbonCommandText_PrintPage,
		DocumentViewer_RibbonCommandText_DocumentMap,
		DocumentViewer_RibbonCommandText_ParametersPanel,
		DocumentViewer_RibbonCommandToolTip_SaveToWindow,
		DocumentViewer_RibbonCommandToolTip_SaveToFile,
		DocumentViewer_RibbonCommandToolTip_FindText,
		DocumentViewer_RibbonCommandToolTip_FirstPage,
		DocumentViewer_RibbonCommandToolTip_PreviousPage,
		DocumentViewer_RibbonCommandToolTip_NextPage,
		DocumentViewer_RibbonCommandToolTip_LastPage,
		DocumentViewer_RibbonCommandToolTip_PrintReport,
		DocumentViewer_RibbonCommandToolTip_PrintPage,
		DocumentViewer_RibbonCommandToolTip_DocumentMap,
		DocumentViewer_RibbonCommandToolTip_ParametersPanel,
		DocumentViewer_RibbonPageCountText,
		DocumentViewer_RibbonCurrentPageText,
		DocumentViewer_RibbonCurrentPageToolTip,
		WebDocumentViewer_PlatformNotSupported_Error,
		WebDocumentViewer_OpenReport_Error,
		WebDocumentViewer_InitializationError,
		WebDocumentViewer_DocumentCreationError,
		WebDocumentViewer_GetBuildStatusError,
		WebDocumentViewer_SearchError,
		WebDocumentViewer_GetDocumentDataError,
		WebDocumentViewer_GetLookUpValuesError,
		WebDocumentViewer_0Pages,
		WebDocumentViewer_NoParameters,
		WebDocumentViewer_SearchResultText,
		WebDocumentViewer_DocumentBuilding,
		WebDocumentViewer_ExportToText,
		WebDocumentViewer_ToggleMultipageMode,
		ReportDesigner_PlatformNotSupported_Error,
		ReportDesigner_Groups,
		ReportDesigner_Tables,
		ReportDesigner_ReportActions_InsertBottomMarginBand,
		ReportDesigner_ReportActions_InsertDetailBand,
		ReportDesigner_ReportActions_InsertDetailReportBand,
		ReportDesigner_ReportActions_InsertGroupFooterBand,
		ReportDesigner_ReportActions_InsertGroupHeaderBand,
		ReportDesigner_ReportActions_InsertPageFooterBand,
		ReportDesigner_ReportActions_InsertPageHeaderBand,
		ReportDesigner_ReportActions_InsertReportFooterBand,
		ReportDesigner_ReportActions_InsertReportHeaderBand,
		ReportDesigner_ReportActions_InsertSubBand,
		ReportDesigner_ReportActions_InsertTopMarginBand,
		ReportDesigner_TableActions_DeleteCell,
		ReportDesigner_TableActions_DeleteColumn,
		ReportDesigner_TableActions_DeleteRow,
		ReportDesigner_TableActions_InsertCell,
		ReportDesigner_TableActions_InsertRowAbove,
		ReportDesigner_TableActions_InsertRowBelow,
		ReportDesigner_TableActions_InsertColumnToLeft,
		ReportDesigner_TableActions_InsertColumnToRight,
		ReportDesigner_GroupFields_Empty,
		ReportDesigner_Preview_ParametersTitle,
		ReportDesigner_FieldList_Parameters,
		ReportDesigner_StylesEditor_CreateNew,
		ReportDesigner_Parameters_CreateParameters,
		ReportDesigner_Pivot_AddFilterFields,
		ReportDesigner_Pivot_AddRowFields,
		ReportDesigner_Pivot_AddColumnFields,
		ReportDesigner_Pivot_AddDataItems,
		ReportDesigner_PivotActions_InsertFieldInTheFilterArea,
		ReportDesigner_PivotActions_InsertFieldInTheDataArea,
		ReportDesigner_PivotActions_InsertFieldInTheColumnArea,
		ReportDesigner_PivotActions_InsertFieldInTheRowArea,
		ReportDesigner_TooltipButtons_Preview,
		ReportDesigner_MenuButtons_RunWizard,
		ReportDesigner_MenuButtons_Save,
		ReportDesigner_ElementsAction_SizeToControlWidth,
		ReportDesigner_ElementsAction_SizeToControlHeight,
		ReportDesigner_ElementsAction_SizeToControl,
		ReportDesigner_FieldListActions_AddParameter,
		ReportDesigner_FieldListActions_AddCalculatedField,
		ReportDesigner_FieldListActions_RemoveParameter,
		ReportDesigner_FieldListActions_RemoveCalculatedField,
		ReportDesigner_Accordion_Collapsed,
		ReportDesigner_Wizard_ChooseDataSource_Title,
		ReportDesigner_Wizard_ChooseDataSource_Description,
		ReportDesigner_Wizard_ChooseDataMember_Title,
		ReportDesigner_Wizard_ChooseDataMember_Description,
		ReportDesigner_Wizard_ChooseColumns_Title,
		ReportDesigner_Wizard_ChooseColumns_Description,
		ReportDesigner_Wizard_AvailableFields,
		ReportDesigner_Wizard_SelectedFields,
		ReportDesigner_Wizard_CreateGroups_Title,
		ReportDesigner_Wizard_CreateGroups_Description,
		ReportDesigner_Wizard_SummaryOptions_Title,
		ReportDesigner_Wizard_SummaryOptions_Description,
		ReportDesigner_Wizard_SummaryOptions_Average,
		ReportDesigner_Wizard_SummaryOptions_Count,
		ReportDesigner_Wizard_SummaryOptions_Max,
		ReportDesigner_Wizard_SummaryOptions_Min,
		ReportDesigner_Wizard_SummaryOptions_Sum,
		ReportDesigner_Wizard_SummaryOptions_IgnoreNullValues,
		ReportDesigner_Wizard_ReportLayout_Title,
		ReportDesigner_Wizard_ReportLayout_Description,
		ReportDesigner_Wizard_ReportLayout_Portrait,
		ReportDesigner_Wizard_ReportLayout_Landscape,
		ReportDesigner_Wizard_ReportLayout_Columnar,
		ReportDesigner_Wizard_ReportLayout_Tabular,
		ReportDesigner_Wizard_ReportLayout_Justified,
		ReportDesigner_Wizard_ReportLayout_Stepped,
		ReportDesigner_Wizard_ReportLayout_Outline1,
		ReportDesigner_Wizard_ReportLayout_Outline2,
		ReportDesigner_Wizard_ReportLayout_AlignLeft1,
		ReportDesigner_Wizard_ReportLayout_AlignLeft2,
		ReportDesigner_Wizard_ReportLayout_AdjustFieldWidth,
		ReportDesigner_Wizard_Report_Style,
		ReportDesigner_Wizard_ReportStyle_Description,
		ReportDesigner_Wizard_ReportStyle_Bold,
		ReportDesigner_Wizard_ReportStyle_Casual,
		ReportDesigner_Wizard_ReportStyle_Corporate,
		ReportDesigner_Wizard_ReportStyle_Compact,
		ReportDesigner_Wizard_ReportStyle_Formal,
		ReportDesigner_Wizard_ReportStyle_Title,
		ReportDesigner_Wizard_ReportStyle_Caption,
		ReportDesigner_Wizard_ReportStyle_Data,
		ReportDesigner_Wizard_ReportComplete_Title,
		ReportDesigner_Wizard_ReportComplete_Description,
		ReportDesigner_Wizard_ReportComplete_SpecifyTitle,
		ReportDesigner_Wizard_Next,
		ReportDesigner_Wizard_Previous,
		ReportDesigner_Wizard_Finish,
		ReportDesigner_Wizard_Header,
		ReportDesigner_Wizard_DataSourceHeader,
	}
	public static class XRWebStringResources {
		public const string DocumentViewer_RibbonHomeTabText = "Home";
		public const string DocumentViewer_RibbonPrintGroupText = "Print";
		public const string DocumentViewer_RibbonNavigationGroupText = "Navigation";
		public const string DocumentViewer_RibbonExportGroupText = "Export";
		public const string DocumentViewer_RibbonReportGroupText = "Report";
	}
	public partial class ASPxReportsLocalizer {
		void AddStrings() {
			AddString(ASPxReportsStringId.SearchDialog_Header, "Search");
			AddString(ASPxReportsStringId.SearchDialog_EnterText, "Enter the text to find in the document.");
			AddString(ASPxReportsStringId.SearchDialog_FindWhat, "Find&nbsp;what");
			AddString(ASPxReportsStringId.SearchDialog_FindWhat_AccessKey, "");
			AddString(ASPxReportsStringId.SearchDialog_FindNext, "Find Next");
			AddString(ASPxReportsStringId.SearchDialog_Cancel, "Cancel");
			AddString(ASPxReportsStringId.SearchDialog_WholeWord, "Match whole word only");
			AddString(ASPxReportsStringId.SearchDialog_WholeWord_AccessKey, "");
			AddString(ASPxReportsStringId.SearchDialog_Case, "Match case");
			AddString(ASPxReportsStringId.SearchDialog_Case_AccessKey, "");
			AddString(ASPxReportsStringId.SearchDialog_Up, "Up");
			AddString(ASPxReportsStringId.SearchDialog_Up_AccessKey, "");
			AddString(ASPxReportsStringId.SearchDialog_Down, "Down");
			AddString(ASPxReportsStringId.SearchDialog_Down_AccessKey, "");
			AddString(ASPxReportsStringId.SearchDialog_Finished, "Finished searching the document.");
			AddString(ASPxReportsStringId.ToolBarItemText_Search, "Display the search window");
			AddString(ASPxReportsStringId.ToolBarItemText_PrintReport, "Print the report");
			AddString(ASPxReportsStringId.ToolBarItemText_PrintPage, "Print the current page");
			AddString(ASPxReportsStringId.ToolBarItemText_FirstPage, "First Page");
			AddString(ASPxReportsStringId.ToolBarItemText_PreviousPage, "Previous Page");
			AddString(ASPxReportsStringId.ToolBarItemText_NextPage, "Next Page");
			AddString(ASPxReportsStringId.ToolBarItemText_LastPage, "Last Page");
			AddString(ASPxReportsStringId.ToolBarItemText_SaveToWindow, "Export a report and show it in a new window");
			AddString(ASPxReportsStringId.ToolBarItemText_SaveToDisk, "Export a report and save it to the disk");
			AddString(ASPxReportsStringId.ToolBarItemText_PageLabel, "Page");
			AddString(ASPxReportsStringId.ToolBarItemText_OfLabel, "of");
			AddString(ASPxReportsStringId.ParametersPanel_Reset, "Reset");
			AddString(ASPxReportsStringId.ParametersPanel_Submit, "Submit");
			AddString(ASPxReportsStringId.ParametersPanel_True, "Yes");
			AddString(ASPxReportsStringId.ParametersPanel_False, "No");
			AddString(ASPxReportsStringId.ParametersPanel_GuidValidationError, "Guid should contain 32 digits delimited with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).");
			AddString(ASPxReportsStringId.ParametersPanel_DateTimeValueValidationError, "The value cannot be empty.");
			AddString(ASPxReportsStringId.ParametersPanel_GenericRegexValidationError, "The value is not valid.");
			AddString(ASPxReportsStringId.ExportName_pdf, "Pdf");
			AddString(ASPxReportsStringId.ExportName_xls, "Xls");
			AddString(ASPxReportsStringId.ExportName_xlsx, "Xlsx");
			AddString(ASPxReportsStringId.ExportName_rtf, "Rtf");
			AddString(ASPxReportsStringId.ExportName_mht, "Mht");
			AddString(ASPxReportsStringId.ExportName_html, "Html");
			AddString(ASPxReportsStringId.ExportName_txt, "Text");
			AddString(ASPxReportsStringId.ExportName_csv, "Csv");
			AddString(ASPxReportsStringId.ExportName_png, "Image");
			AddString(ASPxReportsStringId.DocumentViewer_RemotePageByPage_Error, "To view a remote report, enable the PageByPage property of the SettingsReportViewer.");
			AddString(ASPxReportsStringId.DocumentViewer_RemoteRequestCredentials_Error, "The RequestCredentials event has not been subscribed to.");
			AddString(ASPxReportsStringId.DocumentViewer_RemoteSourceSettings_Error, "To view the remote report, specify the ServerUri or EndpointConfigurationName property of the ASPxDocumentViewer.SettingsRemoteSource.");
			AddString(ASPxReportsStringId.DocumentViewer_RemoteSourceSettings_CustomTokenStorage_Error, "The DocumentViewerRemoteSourceSettings.CustomTokenStorage property is not assigned.");
			AddString(ASPxReportsStringId.DocumentViewer_RemoteSourceSettingsAndConfiguration_Error, "It is only possible to assign either the SettingsRemoteSource or ConfigurationRemoteSource property of ASPxDocumentViewer at a time.");
			AddString(ASPxReportsStringId.DocumentViewer_LocalAndRemoteSource_Error, "It is only possible to assign either the Local Report or Remote Source of ASPxDocumentViewer at a time.");
			AddString(ASPxReportsStringId.DocumentViewer_RemoteAuthenticatorCredential_Error, "The user credentials cannot be empty.");
			AddString(ASPxReportsStringId.DocumentViewer_RemoteAuthenticatorCredentialHandled_Error, "To log in to the Report Server, handle the RequestCredentials event.");
			AddString(ASPxReportsStringId.DocumentViewer_RemoteAuthenticatorLogin_Error, "Failed to log in with the specified user credentials.");
			AddString(ASPxReportsStringId.DocumentViewer_NoRemoteDocumentInformation_Error, "This command cannot be executed because a document has not yet been generated.");
			AddString(ASPxReportsStringId.DocumentViewer_RemoteSourceConnection_Error, "The specified Report Service has not been found.");
			AddString(ASPxReportsStringId.DocumentViewer_ExternalRibbonNotFound_Error, "Cannot find a toolbar control with the specified name: '{0}'.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonHomeTabText, "Home");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonPrintGroupText, "Print");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonNavigationGroupText, "Navigation");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonExportGroupText, "Export");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonReportGroupText, "Report");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_SaveToWindow, "Save To Window");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_SaveToFile, "Save To File");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_FindText, "Find Text");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_FirstPage, "First Page");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_PreviousPage, "Previous Page");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_NextPage, "Next Page");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_LastPage, "Last Page");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_PrintReport, "Print Report");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_PrintPage, "Print Page");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_DocumentMap, "Document Map");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandText_ParametersPanel, "Parameters Panel");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_SaveToWindow, "Save the document in a specified format and display the result in a new window.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_SaveToFile, "Save the document to a file in a specified format.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_FindText, "Find text in the document.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_FirstPage, "Display the first document page.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_PreviousPage, "Display the previous document page.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_NextPage, "Display the next document page.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_LastPage, "Display the last document page.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_PrintReport, "Specify the print settings and print the document.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_PrintPage, "Specify the print settings and print the current page.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_DocumentMap, "Navigate through the report's hierarchy of bookmarks.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCommandToolTip_ParametersPanel, "Access and modify the report parameter values.");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonPageCountText, "Page Count:");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCurrentPageText, "Current Page");
			AddString(ASPxReportsStringId.DocumentViewer_RibbonCurrentPageToolTip, "Display the specified page.");
			AddString(ASPxReportsStringId.WebDocumentViewer_PlatformNotSupported_Error, "To be able to run the Document Viewer, the client web browser must support HTML5.");
			AddString(ASPxReportsStringId.WebDocumentViewer_OpenReport_Error, "To display a report, only one of the following actions can be performed at a time:\n- assigning the ASPxWebDocumentViewer.ReportSourceId property;\n- calling the ASPxWebDocumentViewer.OpenReport method;\n- calling the ASPxWebDocumentViewer.OpenReportXmlLayout method.");
			AddString(ASPxReportsStringId.WebDocumentViewer_InitializationError, "The report preview initialization has failed");
			AddString(ASPxReportsStringId.WebDocumentViewer_DocumentCreationError, "Cannot create a document for the current report");
			AddString(ASPxReportsStringId.WebDocumentViewer_GetBuildStatusError, "Error obtaining a build status");
			AddString(ASPxReportsStringId.WebDocumentViewer_SearchError, "An error occurred during search");
			AddString(ASPxReportsStringId.WebDocumentViewer_GetDocumentDataError, "Cannot obtain additional document data for the current document");
			AddString(ASPxReportsStringId.WebDocumentViewer_GetLookUpValuesError, "Cannot supply filtered lookup values to a report parameter editor");
			AddString(ASPxReportsStringId.WebDocumentViewer_0Pages, "0 pages");
			AddString(ASPxReportsStringId.WebDocumentViewer_NoParameters, "The report does not contain any parameters.");
			AddString(ASPxReportsStringId.WebDocumentViewer_SearchResultText, "Search result");
			AddString(ASPxReportsStringId.WebDocumentViewer_DocumentBuilding, "Document is building...");
			AddString(ASPxReportsStringId.WebDocumentViewer_ExportToText, "Export To");
			AddString(ASPxReportsStringId.WebDocumentViewer_ToggleMultipageMode, "Toggle Multipage Mode");
			AddString(ASPxReportsStringId.ReportDesigner_PlatformNotSupported_Error, "To be able to run the Report Designer, the client web browser must support HTML5.");
			AddString(ASPxReportsStringId.ReportDesigner_Groups, "Groups");
			AddString(ASPxReportsStringId.ReportDesigner_Tables, "Tables");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertBottomMarginBand, "Insert Bottom Margin Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertDetailBand, "Insert Detail Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertDetailReportBand, "Insert Detail Report Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertGroupFooterBand, "Insert Group Footer Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertGroupHeaderBand, "Insert Group Header Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertPageFooterBand, "Insert Page Footer Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertPageHeaderBand, "Insert Page Header Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertReportFooterBand, "Insert Report Footer Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertReportHeaderBand, "Insert Report Header Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertSubBand, "Insert Sub-Band");
			AddString(ASPxReportsStringId.ReportDesigner_ReportActions_InsertTopMarginBand, "Insert Top Margin Band");
			AddString(ASPxReportsStringId.ReportDesigner_TableActions_DeleteCell, "Delete Cell");
			AddString(ASPxReportsStringId.ReportDesigner_TableActions_DeleteColumn, "Delete Column");
			AddString(ASPxReportsStringId.ReportDesigner_TableActions_DeleteRow, "Delete Row");
			AddString(ASPxReportsStringId.ReportDesigner_TableActions_InsertCell, "Insert Cell");
			AddString(ASPxReportsStringId.ReportDesigner_TableActions_InsertRowAbove, "Insert Row Above");
			AddString(ASPxReportsStringId.ReportDesigner_TableActions_InsertRowBelow, "Insert Row Below");
			AddString(ASPxReportsStringId.ReportDesigner_TableActions_InsertColumnToLeft, "Insert Column To the Left");
			AddString(ASPxReportsStringId.ReportDesigner_TableActions_InsertColumnToRight, "Insert Column To the Right");
			AddString(ASPxReportsStringId.ReportDesigner_GroupFields_Empty, "To create a new item, click Add.");
			AddString(ASPxReportsStringId.ReportDesigner_Preview_ParametersTitle, "Preview Parameters");
			AddString(ASPxReportsStringId.ReportDesigner_FieldList_Parameters, "There are no parameters available yet.");
			AddString(ASPxReportsStringId.ReportDesigner_StylesEditor_CreateNew, "Create a New Style");
			AddString(ASPxReportsStringId.ReportDesigner_Parameters_CreateParameters, "The report does not have any parameters yet. To create a new parameter, click Add Parameter.");
			AddString(ASPxReportsStringId.ReportDesigner_Pivot_AddFilterFields, "Add Filter Fields Here");
			AddString(ASPxReportsStringId.ReportDesigner_Pivot_AddRowFields, "Add Row Fields Here");
			AddString(ASPxReportsStringId.ReportDesigner_Pivot_AddColumnFields, "Add Column Fields Here");
			AddString(ASPxReportsStringId.ReportDesigner_Pivot_AddDataItems, "Add Data Items Here");
			AddString(ASPxReportsStringId.ReportDesigner_PivotActions_InsertFieldInTheFilterArea, "Insert Field in the Filter Area");
			AddString(ASPxReportsStringId.ReportDesigner_PivotActions_InsertFieldInTheDataArea, "Insert Field in the Data Area");
			AddString(ASPxReportsStringId.ReportDesigner_PivotActions_InsertFieldInTheColumnArea, "Insert Field in the Column Area");
			AddString(ASPxReportsStringId.ReportDesigner_PivotActions_InsertFieldInTheRowArea, "Insert Field in the Row Area");
			AddString(ASPxReportsStringId.ReportDesigner_TooltipButtons_Preview, "Preview");
			AddString(ASPxReportsStringId.ReportDesigner_MenuButtons_RunWizard, "Run Wizard");
			AddString(ASPxReportsStringId.ReportDesigner_MenuButtons_Save, "Save");
			AddString(ASPxReportsStringId.ReportDesigner_ElementsAction_SizeToControlWidth, "Size to Control Width");
			AddString(ASPxReportsStringId.ReportDesigner_ElementsAction_SizeToControlHeight, "Size to Control Height");
			AddString(ASPxReportsStringId.ReportDesigner_ElementsAction_SizeToControl, "Size to Control");
			AddString(ASPxReportsStringId.ReportDesigner_FieldListActions_AddParameter, "Add parameter");
			AddString(ASPxReportsStringId.ReportDesigner_FieldListActions_AddCalculatedField, "Add calculated field");
			AddString(ASPxReportsStringId.ReportDesigner_FieldListActions_RemoveParameter, "Remove parameter");
			AddString(ASPxReportsStringId.ReportDesigner_FieldListActions_RemoveCalculatedField, "Remove calculated field");
			AddString(ASPxReportsStringId.ReportDesigner_Accordion_Collapsed, "Collapsed");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ChooseDataSource_Title, "Choose a Data Source");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ChooseDataSource_Description, "Choose a Data Source to use in your report.");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ChooseDataMember_Title, "Choose a Table or View");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ChooseDataMember_Description, "The table or view you choose determines wich columns will be available in your report.");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ChooseColumns_Title, "Choose Columns to Display in Your Report");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ChooseColumns_Description, "Select the columns you want to display within your report.");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_AvailableFields, "Available fields");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SelectedFields, "Selected fields");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_CreateGroups_Title, "Create Groups");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_CreateGroups_Description, "Create multiple groups, each with a single field value, or define several fields in the same group.");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Title, "Choose summary options");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Description, "What summary function would you like to calculate?");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Average, "Average");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Count, "Count");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Max, "Max");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Min, "Min");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_Sum, "Sum");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_SummaryOptions_IgnoreNullValues, "Ignore null values");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Title, "Choose a Report Layout");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Description, "The report layout specifies the manner in which selected data fields are arranged on individual pages.");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Portrait, "Portrait");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Landscape, "Landscape");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Columnar, "Columnar");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Tabular, "Tabular");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Justified, "Justified");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Stepped, "Stepped");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Outline1, "Outline 1");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_Outline2, "Outline 2");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_AlignLeft1, "Align Left 1");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_AlignLeft2, "Align Left 2");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportLayout_AdjustFieldWidth, "Adjust the field width so all fields fit onto a page");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_Report_Style, "Choose a Report Style");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Description, "The report style specifies the appearance of your report.");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Bold, "Bold");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Casual, "Casual");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Corporate, "Corporate");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Compact, "Compact");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Formal, "Formal");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Title, "Title");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Caption, "Caption");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportStyle_Data, "Data");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportComplete_Title, "The Report is Complete");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportComplete_Description, "We have all the information needed to process the report.");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_ReportComplete_SpecifyTitle, "Specify the report's title");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_Next, "Next");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_Previous, "Previous");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_Finish, "Finish");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_Header, "Report Wizard");
			AddString(ASPxReportsStringId.ReportDesigner_Wizard_DataSourceHeader, "Data Source Wizard");
		}
	}
}
