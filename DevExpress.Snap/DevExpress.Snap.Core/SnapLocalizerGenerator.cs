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
namespace DevExpress.Snap.Localization {
	#region enum SnapStringId
	public enum SnapStringId {
		SortFieldAscendingCommand_MenuCaption,
		SortFieldAscendingCommand_Description,
		SortFieldDescendingCommand_MenuCaption,
		SortFieldDescendingCommand_Description,
		GroupByFieldCommand_MenuCaption,
		GroupByFieldCommand_Description,
		FilterFieldCommand_MenuCaption,
		FilterFieldCommand_Description,
		PropertiesCommand_MenuCaption,
		PropertiesCommand_Description,
		SummaryCommand_MenuCaption,
		SummaryCommand_Description,
		SummarySumCommand_MenuCaption,
		SummarySumCommand_Description,
		SummaryCountCommand_MenuCaption,
		SummaryCountCommand_Description,
		SummaryAverageCommand_MenuCaption,
		SummaryAverageCommand_Description,
		SummaryMinCommand_MenuCaption,
		SummaryMinCommand_Description,
		SummaryMaxCommand_MenuCaption,
		SummaryMaxCommand_Description,
		ListHeaderCommand_MenuCaption,
		ListHeaderCommand_Description,
		ListFooterCommand_MenuCaption,
		ListFooterCommand_Description,
		InsertListHeaderCommand_MenuCaption,
		InsertListHeaderCommand_Description,
		InsertListFooterCommand_MenuCaption,
		InsertListFooterCommand_Description,
		RemoveListHeaderCommand_MenuCaption,
		RemoveListHeaderCommand_Description,
		RemoveListFooterCommand_MenuCaption,
		RemoveListFooterCommand_Description,
		FilterListCommand_MenuCaption,
		FilterListCommand_Description,
		ShowCustomSortForm_MenuCaption,
		ShowCustomSortForm_Description,
		ConvertToParagraphsCommand_MenuCaption,
		ConvertToParagraphsCommand_Description,
		ChangeEditorRowLimitCommand_MenuCaption,
		ChangeEditorRowLimitCommand_Description,
		DeleteListCommand_MenuCaption,
		DeleteListCommand_Description,
		InsertGroupHeaderCommand_MenuCaption,
		InsertGroupHeaderCommand_Description,
		InsertGroupFooterCommand_MenuCaption,
		InsertGroupFooterCommand_Description,
		RemoveGroupHeaderCommand_MenuCaption,
		RemoveGroupHeaderCommand_Description,
		RemoveGroupFooterCommand_MenuCaption,
		RemoveGroupFooterCommand_Description,
		GroupHeaderCommand_MenuCaption,
		GroupHeaderCommand_Description,
		GroupFooterCommand_MenuCaption,
		GroupFooterCommand_Description,
		GroupFieldsCollectionCommand_MenuCaption,
		GroupFieldsCollectionCommand_Description,
		InsertGroupSeparatorCommand_MenuCaption,
		InsertGroupSeparatorCommand_Description,
		InsertPageBreakGroupSeparatorCommand_MenuCaption,
		InsertPageBreakGroupSeparatorCommand_Description,
		InsertNoneGroupSeparatorCommand_MenuCaption,
		InsertNoneGroupSeparatorCommand_Description,
		InsertEmptyParagraphGroupSeparatorCommand_MenuCaption,
		InsertEmptyParagraphGroupSeparatorCommand_Description,
		InsertEmptyRowGroupSeparatorCommand_MenuCaption,
		InsertEmptyRowGroupSeparatorCommand_Description,
		InsertSectionBreakNextPageGroupSeparatorCommand_MenuCaption,
		InsertSectionBreakNextPageGroupSeparatorCommand_Description,
		InsertSectionBreakEvenPageGroupSeparatorCommand_MenuCaption,
		InsertSectionBreakEvenPageGroupSeparatorCommand_Description,
		InsertSectionBreakOddPageGroupSeparatorCommand_MenuCaption,
		InsertSectionBreakOddPageGroupSeparatorCommand_Description,
		InsertDataRowSeparatorCommand_MenuCaption,
		InsertDataRowSeparatorCommand_Description,
		InsertPageBreakDataRowSeparatorCommand_MenuCaption,
		InsertPageBreakDataRowSeparatorCommand_Description,
		InsertNoneDataRowSeparatorCommand_MenuCaption,
		InsertNoneDataRowSeparatorCommand_Description,
		InsertEmptyParagraphDataRowSeparatorCommand_MenuCaption,
		InsertEmptyParagraphDataRowSeparatorCommand_Description,
		InsertEmptyRowDataRowSeparatorCommand_MenuCaption,
		InsertEmptyRowDataRowSeparatorCommand_Description,
		InsertSectionBreakNextPageDataRowSeparatorCommand_MenuCaption,
		InsertSectionBreakNextPageDataRowSeparatorCommand_Description,
		InsertSectionBreakEvenPageDataRowSeparatorCommand_MenuCaption,
		InsertSectionBreakEvenPageDataRowSeparatorCommand_Description,
		InsertSectionBreakOddPageDataRowSeparatorCommand_MenuCaption,
		InsertSectionBreakOddPageDataRowSeparatorCommand_Description,
		Sorting_MenuCaption,
		ThemeName_Casual,
		ThemeName_ContrastCyan,
		ThemeName_ContrastOrange,
		ThemeName_ContrastRed,
		ThemeName_ContrastSalmon,
		ThemeName_ContrastYellow,
		ThemeName_DodgerBlue,
		ThemeName_FormalBlue,
		ThemeName_MildBlue,
		ThemeName_MildBrown,
		ThemeName_MildCyan,
		ThemeName_MildViolet,
		ThemeName_SoftLilac,
		ChangeThemeCommand_MenuCaption,
		ChangeThemeCommand_Description,
		ExportDocumentCommand_MenuCaption,
		ExportDocumentCommand_Description,
		MoveUp_MenuCaption,
		MoveUp_Description,
		MoveDown_MenuCaption,
		MoveDown_Description,
		ShowReportStructureEditorForm_MenuCaption,
		ShowReportStructureEditorForm_Description,
		MenuCmd_NewTableCellStyle,
		MenuCmd_ModifyTableCellStyle,
		MenuCmd_DeleteTableCellStyle,
		NewDataSourceCommand_MenuCaption,
		NewDataSourceCommand_Description,
		RemoveDataSourceCommand_MenuCaption,
		RemoveDataSourceCommand_Description,
		InsertBarCodeCommand_MenuCaption,
		InsertBarCodeCommand_Description,
		InsertSparklineCommand_MenuCaption,
		InsertSparklineCommand_Description,
		InsertCheckBoxCommand_MenuCaption,
		InsertCheckBoxCommand_Description,
		InsertChartCommand_MenuCaption,
		InsertChartCommand_Description,
		InsertIndexCommand_MenuCaption,
		InsertIndexCommand_Description,
		ToggleFieldHighlighting_MenuCaption,
		ToggleFieldHighlighting_Description,
		RunChartDesignerCommand_Description,
		RunChartDesignerCommand_MenuCaption,
		Msg_UnsupportedDocumentVersion,
		Msg_DataSourceNameExists,
		Msg_InvalidEditorRowLimit,
		Msg_FieldDefinedAsSortingCriterionMoreThanOnce,
		Msg_FieldAlreadyDefinedAsGroupingCriterion,
		Msg_CollectionAlreadyContainsTheme,
		Msg_CannotDeleteDefaultTheme,
		Msg_ThemeIsNotLoaded,
		Msg_CannotPerformAsynchronousOperation,
		Msg_CannotChangeDataSourceName,
		ShowGroupSortingsCheckBox_Text,
		FileFilterDescription_SnapFiles,
		FileFilterDescription_SnapThemeFiles,
		HighlightActiveElementCommand_MenuCaption,
		HighlightActiveElementCommand_Description,
		SummaryTooltip,
		EditorRowLimitShowAll,
		ReorderReportStructureForm_Text,
		CustomSortForm_Text,
		ProgressIndicationForm_Text,
		Msg_StopMailMerge,
		SnapListLockException,
		SnapListSecondBeginUpdateException,
		SnapListPropertyOutOfDataException,
		SnapEntityAddLock,
		SnapEntityRemoveLock,
		ArraysLengthsMismatchException,
		SnapPrintPreviewCommand_MenuCaption,
		SnapPrintPreviewCommand_Description,
		SnapPrintCommand_MenuCaption,
		SnapPrintCommand_Description,
		SnapQuickPrintCommand_MenuCaption,
		SnapQuickPrintCommand_Description,
		MailMergeDataSource_MenuCaption,
		MailMergeDataSource_Description,
		MailMergeFilters_MenuCaption,
		MailMergeFilters_Description,
		MailMergeSorting_MenuCaption,
		MailMergeSorting_Description,
		MailMergeCurrentRecord_MenuCaption,
		MailMergeCurrentRecord_Description,
		Msg_InvalidMailMergeCurrentRecord,
		RecordSeparator_None,
		RecordSeparator_PageBreak,
		RecordSeparator_SectionNextPage,
		RecordSeparator_SectionEvenPage,
		RecordSeparator_SectionOddPage,
		RecordSeparator_Paragraph,
		FinishAndMergeCommand_MenuCaption,
		FinishAndMergeCommand_Description,
		TemplateDecoratorType_DataRow,
		TemplateDecoratorType_GroupFooter,
		TemplateDecoratorType_GroupHeader,
		TemplateDecoratorType_GroupSeparator,
		TemplateDecoratorType_ListFooter,
		TemplateDecoratorType_ListHeader,
		TemplateDecoratorType_Separator,
		TemplateDecoratorType_WholeGroup,
		TemplateDecoratorType_WholeList,
		HotZonePainter_DropValues,
		HotZonePainter_DropArguments,
		HotZonePainter_SecondLine,
		ReportExplorer_ListNode,
		ReportExplorer_GroupNode,
		CalculatedField_DataMember,
		CalculatedField_Expression,
		CalculatedField_FieldType,
		CalculatedField_Name,
		CalculatedField_DataSourceName,
		GroupField_FieldName,
		GroupField_SortOrder,
		ParametersErrorInvalidCharacters,
		ParametersErrorNoName,
		ParameterService_AddParameter,
		ParameterService_CreateParameter,
		SortingForm_SortByColumnCaption,
		SortingForm_OrderColumnCaption,
		WizardPageDataSourceName,
	}
	#endregion
	#region SnapLocalizer.AddStrings 
	public partial class SnapLocalizer {
		void AddStrings() {
			AddString(SnapStringId.SortFieldAscendingCommand_MenuCaption, "Sort Ascending");
			AddString(SnapStringId.SortFieldAscendingCommand_Description, "Click to sort (in ascending order) by the field.");
			AddString(SnapStringId.SortFieldDescendingCommand_MenuCaption, "Sort Descending");
			AddString(SnapStringId.SortFieldDescendingCommand_Description, "Click to sort (in descending order) by the field.");
			AddString(SnapStringId.GroupByFieldCommand_MenuCaption, "Group By Field");
			AddString(SnapStringId.GroupByFieldCommand_Description, "Click to enable/disable grouping by field.\r\n\r\nThis will break the list into groups with the selected field being used as a grouping criterion.");
			AddString(SnapStringId.FilterFieldCommand_MenuCaption, "Quick Filter");
			AddString(SnapStringId.FilterFieldCommand_Description, "Select which of the field’s data records to exclude from the document.\r\n\r\nFor advanced filtering, use the Filter option in the List tab.");
			AddString(SnapStringId.PropertiesCommand_MenuCaption, "Properties");
			AddString(SnapStringId.PropertiesCommand_Description, "Access the properties of an element that are assigned to the field. To assign a different element to the field, use the Content Type property.\r\n\r\nTo access the data options of the field, use the Binding property.");
			AddString(SnapStringId.SummaryCommand_MenuCaption, "Summary");
			AddString(SnapStringId.SummaryCommand_Description, "Choose the summary function to calculate for the field.\r\n\r\nThe summary function result will be shown at the list footer which is made visible if it previously was not.");
			AddString(SnapStringId.SummarySumCommand_MenuCaption, "Sum");
			AddString(SnapStringId.SummarySumCommand_Description, "Sum");
			AddString(SnapStringId.SummaryCountCommand_MenuCaption, "Count");
			AddString(SnapStringId.SummaryCountCommand_Description, "Count");
			AddString(SnapStringId.SummaryAverageCommand_MenuCaption, "Average");
			AddString(SnapStringId.SummaryAverageCommand_Description, "Average");
			AddString(SnapStringId.SummaryMinCommand_MenuCaption, "Min");
			AddString(SnapStringId.SummaryMinCommand_Description, "Min");
			AddString(SnapStringId.SummaryMaxCommand_MenuCaption, "Max");
			AddString(SnapStringId.SummaryMaxCommand_Description, "Max");
			AddString(SnapStringId.ListHeaderCommand_MenuCaption, "Header");
			AddString(SnapStringId.ListHeaderCommand_Description, "Click to add or remove a list header.\r\n\r\nIt contains columns captions by default.");
			AddString(SnapStringId.ListFooterCommand_MenuCaption, "Footer");
			AddString(SnapStringId.ListFooterCommand_Description, "Click to add or remove a list footer.\r\n\r\nIt is blank by default. You can use it to present list summaries and other information.");
			AddString(SnapStringId.InsertListHeaderCommand_MenuCaption, "Add Header");
			AddString(SnapStringId.InsertListHeaderCommand_Description, "Add Header");
			AddString(SnapStringId.InsertListFooterCommand_MenuCaption, "Add Footer");
			AddString(SnapStringId.InsertListFooterCommand_Description, "Add Footer");
			AddString(SnapStringId.RemoveListHeaderCommand_MenuCaption, "Remove Header");
			AddString(SnapStringId.RemoveListHeaderCommand_Description, "Remove Header");
			AddString(SnapStringId.RemoveListFooterCommand_MenuCaption, "Remove Footer");
			AddString(SnapStringId.RemoveListFooterCommand_Description, "Remove Footer");
			AddString(SnapStringId.FilterListCommand_MenuCaption, "Filter");
			AddString(SnapStringId.FilterListCommand_Description, "Filter the list data by applying whatever complex criteria that may be required. The filtering condition can also accommodate calculated fields and parameters that are created using Data Explorer.\r\n\r\nTo single out which data records to omit in the document, use the Quick Filter option in the Field tab.");
			AddString(SnapStringId.ShowCustomSortForm_MenuCaption, "Sort");
			AddString(SnapStringId.ShowCustomSortForm_Description, "Sort");
			AddString(SnapStringId.ConvertToParagraphsCommand_MenuCaption, "Convert to Paragraphs");
			AddString(SnapStringId.ConvertToParagraphsCommand_Description, "Click to convert the tables to paragraphs.\r\n\r\nEvery field will then be presented in a separate paragraph, and you can highlight fields by using the Highlight option in the View tab of this toolbar.");
			AddString(SnapStringId.ChangeEditorRowLimitCommand_MenuCaption, "Editor Row Limit");
			AddString(SnapStringId.ChangeEditorRowLimitCommand_Description, "Define the maximum number of rows shown in the document lists and groups during your editing session.\r\n\r\nThis option allows you to save time when working with large data sources. It does not affect the document's data in the print preview.");
			AddString(SnapStringId.DeleteListCommand_MenuCaption, "Delete List");
			AddString(SnapStringId.DeleteListCommand_Description, "Remove the list along with its data from the document.");
			AddString(SnapStringId.InsertGroupHeaderCommand_MenuCaption, "Add Header");
			AddString(SnapStringId.InsertGroupHeaderCommand_Description, "Add Header");
			AddString(SnapStringId.InsertGroupFooterCommand_MenuCaption, "Add Footer");
			AddString(SnapStringId.InsertGroupFooterCommand_Description, "Add Footer");
			AddString(SnapStringId.RemoveGroupHeaderCommand_MenuCaption, "Remove Header");
			AddString(SnapStringId.RemoveGroupHeaderCommand_Description, "Remove Header");
			AddString(SnapStringId.RemoveGroupFooterCommand_MenuCaption, "Remove Footer");
			AddString(SnapStringId.RemoveGroupFooterCommand_Description, "Remove Footer");
			AddString(SnapStringId.GroupHeaderCommand_MenuCaption, "Header");
			AddString(SnapStringId.GroupHeaderCommand_Description, "Click to add or remove a group header.\r\n\r\nThe group header is created after a grouping has been applied by a field. The header displays the grouping criterion field.\r\n\r\nHiding both the header and footer of a group will disable grouping altogether.");
			AddString(SnapStringId.GroupFooterCommand_MenuCaption, "Footer");
			AddString(SnapStringId.GroupFooterCommand_Description, "Click to add or remove a group footer.\r\n\r\nThe footer displays the group summary function result. By default, the Count summary is being calculated, which you can change using the Summary option in the Field tab.\r\n\r\nHiding both the header and footer of a group will disable grouping altogether.");
			AddString(SnapStringId.GroupFieldsCollectionCommand_MenuCaption, "Group Fields");
			AddString(SnapStringId.GroupFieldsCollectionCommand_Description, "Click to manage the group's criteria.\r\n\r\nEvery group can have multiple criteria. In the document, a separate group section is created for every grouping criterion, with its own header and footer.");
			AddString(SnapStringId.InsertGroupSeparatorCommand_MenuCaption, "Separator");
			AddString(SnapStringId.InsertGroupSeparatorCommand_Description, "Choose a separator to delimit groups in the document.");
			AddString(SnapStringId.InsertPageBreakGroupSeparatorCommand_MenuCaption, "Page Break");
			AddString(SnapStringId.InsertPageBreakGroupSeparatorCommand_Description, "Page Break");
			AddString(SnapStringId.InsertNoneGroupSeparatorCommand_MenuCaption, "None");
			AddString(SnapStringId.InsertNoneGroupSeparatorCommand_Description, "None");
			AddString(SnapStringId.InsertEmptyParagraphGroupSeparatorCommand_MenuCaption, "Empty Paragraph");
			AddString(SnapStringId.InsertEmptyParagraphGroupSeparatorCommand_Description, "Empty Paragraph");
			AddString(SnapStringId.InsertEmptyRowGroupSeparatorCommand_MenuCaption, "Empty Row");
			AddString(SnapStringId.InsertEmptyRowGroupSeparatorCommand_Description, "Empty Row");
			AddString(SnapStringId.InsertSectionBreakNextPageGroupSeparatorCommand_MenuCaption, "Section (Next Page)");
			AddString(SnapStringId.InsertSectionBreakNextPageGroupSeparatorCommand_Description, "Section (Next Page)");
			AddString(SnapStringId.InsertSectionBreakEvenPageGroupSeparatorCommand_MenuCaption, "Section (Even Page)");
			AddString(SnapStringId.InsertSectionBreakEvenPageGroupSeparatorCommand_Description, "Section (Even Page)");
			AddString(SnapStringId.InsertSectionBreakOddPageGroupSeparatorCommand_MenuCaption, "Section (Odd Page)");
			AddString(SnapStringId.InsertSectionBreakOddPageGroupSeparatorCommand_Description, "Section (Odd Page)");
			AddString(SnapStringId.InsertDataRowSeparatorCommand_MenuCaption, "Separator");
			AddString(SnapStringId.InsertDataRowSeparatorCommand_Description, "Split the document into sections by inserting an appropriate separator in the current carriage position.");
			AddString(SnapStringId.InsertPageBreakDataRowSeparatorCommand_MenuCaption, "PageBreak");
			AddString(SnapStringId.InsertPageBreakDataRowSeparatorCommand_Description, "PageBreak");
			AddString(SnapStringId.InsertNoneDataRowSeparatorCommand_MenuCaption, "None");
			AddString(SnapStringId.InsertNoneDataRowSeparatorCommand_Description, "None");
			AddString(SnapStringId.InsertEmptyParagraphDataRowSeparatorCommand_MenuCaption, "Empty Paragraph");
			AddString(SnapStringId.InsertEmptyParagraphDataRowSeparatorCommand_Description, "Empty Paragraph");
			AddString(SnapStringId.InsertEmptyRowDataRowSeparatorCommand_MenuCaption, "Empty Row");
			AddString(SnapStringId.InsertEmptyRowDataRowSeparatorCommand_Description, "Empty Row");
			AddString(SnapStringId.InsertSectionBreakNextPageDataRowSeparatorCommand_MenuCaption, "Section (Next Page)");
			AddString(SnapStringId.InsertSectionBreakNextPageDataRowSeparatorCommand_Description, "Section (Next Page)");
			AddString(SnapStringId.InsertSectionBreakEvenPageDataRowSeparatorCommand_MenuCaption, "Section (Even Page)");
			AddString(SnapStringId.InsertSectionBreakEvenPageDataRowSeparatorCommand_Description, "Section (Even Page)");
			AddString(SnapStringId.InsertSectionBreakOddPageDataRowSeparatorCommand_MenuCaption, "Section (Odd Page)");
			AddString(SnapStringId.InsertSectionBreakOddPageDataRowSeparatorCommand_Description, "Section (Odd Page)");
			AddString(SnapStringId.Sorting_MenuCaption, "Sorting");
			AddString(SnapStringId.ThemeName_Casual, "Casual");
			AddString(SnapStringId.ThemeName_ContrastCyan, "Contrast Cyan");
			AddString(SnapStringId.ThemeName_ContrastOrange, "Contrast Orange");
			AddString(SnapStringId.ThemeName_ContrastRed, "Contrast Red");
			AddString(SnapStringId.ThemeName_ContrastSalmon, "Contrast Salmon");
			AddString(SnapStringId.ThemeName_ContrastYellow, "Contrast Yellow");
			AddString(SnapStringId.ThemeName_DodgerBlue, "Dodger Blue");
			AddString(SnapStringId.ThemeName_FormalBlue, "Formal Blue");
			AddString(SnapStringId.ThemeName_MildBlue, "Mild Blue");
			AddString(SnapStringId.ThemeName_MildBrown, "Mild Brown");
			AddString(SnapStringId.ThemeName_MildCyan, "Mild Cyan");
			AddString(SnapStringId.ThemeName_MildViolet, "Mild Violet");
			AddString(SnapStringId.ThemeName_SoftLilac, "Soft Lilac");
			AddString(SnapStringId.ChangeThemeCommand_MenuCaption, "Change Theme");
			AddString(SnapStringId.ChangeThemeCommand_Description, "Change Theme");
			AddString(SnapStringId.ExportDocumentCommand_MenuCaption, "Export...");
			AddString(SnapStringId.ExportDocumentCommand_Description, "Render a copy of the document in a third-party format.");
			AddString(SnapStringId.MoveUp_MenuCaption, "Move Up");
			AddString(SnapStringId.MoveUp_Description, "Move Up");
			AddString(SnapStringId.MoveDown_MenuCaption, "Move Down");
			AddString(SnapStringId.MoveDown_Description, "Move Down");
			AddString(SnapStringId.ShowReportStructureEditorForm_MenuCaption, "Arrange Groups");
			AddString(SnapStringId.ShowReportStructureEditorForm_Description, "Specify the order for applying the available groupings.");
			AddString(SnapStringId.MenuCmd_NewTableCellStyle, "New Cell Style...");
			AddString(SnapStringId.MenuCmd_ModifyTableCellStyle, "Modify Cell Style...");
			AddString(SnapStringId.MenuCmd_DeleteTableCellStyle, "Delete Cell Style...");
			AddString(SnapStringId.NewDataSourceCommand_MenuCaption, "Add New Data Source");
			AddString(SnapStringId.NewDataSourceCommand_Description, "Connect the document to a data source.");
			AddString(SnapStringId.RemoveDataSourceCommand_MenuCaption, "Remove Data Source");
			AddString(SnapStringId.RemoveDataSourceCommand_Description, "Remove the specified data source.");
			AddString(SnapStringId.InsertBarCodeCommand_MenuCaption, "Bar Code");
			AddString(SnapStringId.InsertBarCodeCommand_Description, "Inserts a bar code that can encode a variety of popular symbologies.\r\n\r\nTo customize the currently selected bar code, click the Properties button in the Field tab.");
			AddString(SnapStringId.InsertSparklineCommand_MenuCaption, "Sparkline");
			AddString(SnapStringId.InsertSparklineCommand_Description, "Inserts a miniature inline chart that is best suited to illustrate the general variation of a value along a time frame.\r\n\r\nTo customize the currently selected sparkline, click the Properties button in the Field tab.");
			AddString(SnapStringId.InsertCheckBoxCommand_MenuCaption, "Check Box");
			AddString(SnapStringId.InsertCheckBoxCommand_Description, "Inserts a check box to display Boolean (True/False/Indeterminate) values.\r\n\r\nTo customize the currently selected check box, click the Properties button in the Field tab.");
			AddString(SnapStringId.InsertChartCommand_MenuCaption, "Chart");
			AddString(SnapStringId.InsertChartCommand_Description, "Inserts a full-featured graphical chart that can plot dozens of different series types.\r\n\r\nTo customize the currently selected chart, use the Design tab options of the Chart Tools menu category.");
			AddString(SnapStringId.InsertIndexCommand_MenuCaption, "Row Index");
			AddString(SnapStringId.InsertIndexCommand_Description, "Enumerates records of a data source column within the document.\r\n\r\nTo specify the format string of the currently selected index and define its behavior when within groups, click the Properties button in the Field tab.");
			AddString(SnapStringId.ToggleFieldHighlighting_MenuCaption, "Highlight Fields");
			AddString(SnapStringId.ToggleFieldHighlighting_Description, "Identify dynamic elements within the document.");
			AddString(SnapStringId.RunChartDesignerCommand_Description, "Run designer...");
			AddString(SnapStringId.RunChartDesignerCommand_MenuCaption, "Run designer...");
			AddString(SnapStringId.Msg_UnsupportedDocumentVersion, "Unsupported Snap document version");
			AddString(SnapStringId.Msg_DataSourceNameExists, "A data source with the same name already exists");
			AddString(SnapStringId.Msg_InvalidEditorRowLimit, "Invalid value");
			AddString(SnapStringId.Msg_FieldDefinedAsSortingCriterionMoreThanOnce, "The {0} field can be defined as a sorting criterion only once.");
			AddString(SnapStringId.Msg_FieldAlreadyDefinedAsGroupingCriterion, "The {0} field has already been defined as a grouping criterion.");
			AddString(SnapStringId.Msg_CollectionAlreadyContainsTheme, "The collection already contains a theme named {0}.");
			AddString(SnapStringId.Msg_CannotDeleteDefaultTheme, "Cannot delete default theme.");
			AddString(SnapStringId.Msg_ThemeIsNotLoaded, "The theme is not loaded. Cannot use the {0} theme.");
			AddString(SnapStringId.Msg_CannotPerformAsynchronousOperation, "Cannot perform the same asynchronous operation in multiple simultaneous threads.");
			AddString(SnapStringId.Msg_CannotChangeDataSourceName, "Cannot change the data source name.");
			AddString(SnapStringId.ShowGroupSortingsCheckBox_Text, "Show Group Sortings");
			AddString(SnapStringId.FileFilterDescription_SnapFiles, "Snap Document");
			AddString(SnapStringId.FileFilterDescription_SnapThemeFiles, "Snap Theme");
			AddString(SnapStringId.HighlightActiveElementCommand_MenuCaption, "Highlight");
			AddString(SnapStringId.HighlightActiveElementCommand_Description, "Click to show the information about the current element’s type and bounds.");
			AddString(SnapStringId.SummaryTooltip, "Calculates the {0} summary function by the {1} field for the {2}");
			AddString(SnapStringId.EditorRowLimitShowAll, "(Show All)");
			AddString(SnapStringId.ReorderReportStructureForm_Text, "Groups Order Editor");
			AddString(SnapStringId.CustomSortForm_Text, "Sort");
			AddString(SnapStringId.ProgressIndicationForm_Text, "Performing mail merge...");
			AddString(SnapStringId.Msg_StopMailMerge, "Interrupt the report execution?");
			AddString(SnapStringId.SnapListLockException, "Modify attempt before BeginUpdate() call");
			AddString(SnapStringId.SnapListSecondBeginUpdateException, "Previous update should be finished before start new one");
			AddString(SnapStringId.SnapListPropertyOutOfDataException, "Value is out of data, list property must be reread after EndUpdate() call");
			AddString(SnapStringId.SnapEntityAddLock, "Unable to add new {0} while updating another field");
			AddString(SnapStringId.SnapEntityRemoveLock, "Unable to remove field while updating another one");
			AddString(SnapStringId.ArraysLengthsMismatchException, "Arrays must have same length");
			AddString(SnapStringId.SnapPrintPreviewCommand_MenuCaption, "Print Pre&view...");
			AddString(SnapStringId.SnapPrintPreviewCommand_Description, "Preview pages before printing.");
			AddString(SnapStringId.SnapPrintCommand_MenuCaption, "&Print...");
			AddString(SnapStringId.SnapPrintCommand_Description, "Select a printer, number of copies, and other printing options before printing.");
			AddString(SnapStringId.SnapQuickPrintCommand_MenuCaption, "&Quick Print");
			AddString(SnapStringId.SnapQuickPrintCommand_Description, "Send the document directly to the default printer without making changes.");
			AddString(SnapStringId.MailMergeDataSource_MenuCaption, "Data Source");
			AddString(SnapStringId.MailMergeDataSource_Description, "Select the data source to use with mail merge.\r\n\r\nBe sure to save the current document version prior to making any changes with this option. Once mail merge is assigned to a data source, the current document is recreated, resulting in a possible loss of the previous layout.");
			AddString(SnapStringId.MailMergeFilters_MenuCaption, "Filter");
			AddString(SnapStringId.MailMergeFilters_Description, "Filter the data supplied by a mail merge data source.");
			AddString(SnapStringId.MailMergeSorting_MenuCaption, "Sort");
			AddString(SnapStringId.MailMergeSorting_Description, "Sort the data supplied by a mail merge data source and arrange the succession in which sorting levels are applied.");
			AddString(SnapStringId.MailMergeCurrentRecord_MenuCaption, "Current Record");
			AddString(SnapStringId.MailMergeCurrentRecord_Description, "Mail Merge Data Source record visible.");
			AddString(SnapStringId.Msg_InvalidMailMergeCurrentRecord, "Invalid value");
			AddString(SnapStringId.RecordSeparator_None, "None");
			AddString(SnapStringId.RecordSeparator_PageBreak, "Page Break");
			AddString(SnapStringId.RecordSeparator_SectionNextPage, "Section (Next Page)");
			AddString(SnapStringId.RecordSeparator_SectionEvenPage, "Section (Even Page)");
			AddString(SnapStringId.RecordSeparator_SectionOddPage, "Section (Odd Page)");
			AddString(SnapStringId.RecordSeparator_Paragraph, "Paragraph");
			AddString(SnapStringId.FinishAndMergeCommand_MenuCaption, "Finish && Merge");
			AddString(SnapStringId.FinishAndMergeCommand_Description, "Export, preview or print the document.\r\n\r\nThe document can include all available data records, or only part of them (i.e., the specified data row range, or only the single row that is currently displayed). Data records can be split off by using a separator of a selected type.");
			AddString(SnapStringId.TemplateDecoratorType_DataRow, "DataRow");
			AddString(SnapStringId.TemplateDecoratorType_GroupFooter, "GroupFooter");
			AddString(SnapStringId.TemplateDecoratorType_GroupHeader, "GroupHeader");
			AddString(SnapStringId.TemplateDecoratorType_GroupSeparator, "GroupSeparator");
			AddString(SnapStringId.TemplateDecoratorType_ListFooter, "ListFooter");
			AddString(SnapStringId.TemplateDecoratorType_ListHeader, "ListHeader");
			AddString(SnapStringId.TemplateDecoratorType_Separator, "Separator");
			AddString(SnapStringId.TemplateDecoratorType_WholeGroup, "WholeGroup");
			AddString(SnapStringId.TemplateDecoratorType_WholeList, "WholeList");
			AddString(SnapStringId.HotZonePainter_DropValues, "Drop values");
			AddString(SnapStringId.HotZonePainter_DropArguments, "Drop arguments");
			AddString(SnapStringId.HotZonePainter_SecondLine, "here");
			AddString(SnapStringId.ReportExplorer_ListNode, "List");
			AddString(SnapStringId.ReportExplorer_GroupNode, "Group");
			AddString(SnapStringId.CalculatedField_DataMember, "Data Member");
			AddString(SnapStringId.CalculatedField_Expression, "Expression");
			AddString(SnapStringId.CalculatedField_FieldType, "Field Type");
			AddString(SnapStringId.CalculatedField_Name, "Name");
			AddString(SnapStringId.CalculatedField_DataSourceName, "Data Source Name");
			AddString(SnapStringId.GroupField_FieldName, "Field Name");
			AddString(SnapStringId.GroupField_SortOrder, "Sort Order");
			AddString(SnapStringId.ParametersErrorInvalidCharacters, "Cannot create a parameter with invalid name: ");
			AddString(SnapStringId.ParametersErrorNoName, "Cannot create a parameter without specifying its name.");
			AddString(SnapStringId.ParameterService_AddParameter, "New Parameter");
			AddString(SnapStringId.ParameterService_CreateParameter, "Query Parameter");
			AddString(SnapStringId.SortingForm_SortByColumnCaption, "Sort by");
			AddString(SnapStringId.SortingForm_OrderColumnCaption, "Order");
			AddString(SnapStringId.WizardPageDataSourceName, "Enter the data source name");
		}
	}
	 #endregion
}
