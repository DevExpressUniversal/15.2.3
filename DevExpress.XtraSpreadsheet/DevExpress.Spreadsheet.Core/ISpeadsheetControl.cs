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
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office;
using DevExpress.Office.Forms;
using DevExpress.Office.Internal;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.API.Native;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet {
	public interface ISpreadsheetControl : ISpreadsheetComponent, IWin32Window, ICommandAwareControl<SpreadsheetCommandId> {
		InnerSpreadsheetControl InnerControl { get; }
		InnerSpreadsheetDocumentServer InnerDocumentServer { get; }
		IWorkbook Document { get; }
		DevExpress.Spreadsheet.Worksheet ActiveWorksheet { get; }
		DevExpress.Spreadsheet.Cell ActiveCell { get; }
		DevExpress.Spreadsheet.Range Selection { get; set; }
		DevExpress.Spreadsheet.Range SelectedCell { get; set; }
		IList<DevExpress.Spreadsheet.Range> GetSelectedRanges();
		bool SetSelectedRanges(IList<DevExpress.Spreadsheet.Range> ranges);
		bool SetSelectedRanges(IList<DevExpress.Spreadsheet.Range> ranges, bool expandToMergedCellsSize);
		DevExpress.Spreadsheet.Shape SelectedShape { get; set; }
		DevExpress.Spreadsheet.Picture SelectedPicture { get; set; }
		IList<DevExpress.Spreadsheet.Shape> GetSelectedShapes();
		bool SetSelectedShapes(IList<DevExpress.Spreadsheet.Shape> Shapes);
		Rectangle ViewBounds { get; }
		Rectangle LayoutViewBounds { get; }
		DocumentUnit UIUnit { get; set; }
		DocumentLayoutUnit LayoutUnit { get; set; }
		bool IsPrintingAvailable { get; }
		bool IsPrintPreviewAvailable { get; }
		Cursor Cursor { get; set; }
		bool UseGdiPlus { get; }
		DialogResult ShowWarningMessage(string message);
		DialogResult ShowMessage(string message, string title, MessageBoxIcon icon);
		DialogResult ShowDataValidationDialog(string message, string title, Model.DataValidationErrorStyle errorStyle);
		DialogResult ShowYesNoCancelMessage(string message);
		bool ShowOkCancelMessage(string message);
		bool ShowYesNoMessage(string message);
		bool CaptureMouse();
		bool ReleaseMouse();
		event CancelEventHandler DocumentClosing;
		bool IsHyperlinkActive();
		void UpdateUIFromBackgroundThread(Action method);
		void Print();
		void ShowPrintDialog();
		void ShowPrintPreview();
		void ShowRibbonPrintPreview();
		void ShowFormatCellsForm(FormatCellsFormProperties properties, FormatCellsFormInitialTabPage initialTabPage, ShowFormatCellsFormCallback callback);
		void ShowHyperlinkForm(IHyperlinkViewInfo hyperlink, ShowHyperlinkFormCallback callback);
		void ShowConditionalFormattingTop10RuleForm(ConditionalFormattingTopBottomRuleViewModel viewModel);
		void ShowConditionalFormattingAverageRuleForm(ConditionalFormattingAverageRuleViewModel viewModel);
		void ShowConditionalFormattingExpressionRuleForm(ConditionalFormattingHighlightCellsRuleViewModel viewModel);
		void ShowConditionalFormattingTextRuleForm(ConditionalFormattingTextRuleViewModel viewModel);
		void ShowConditionalFormattingDuplicateValuesRuleForm(ConditionalFormattingDuplicateValuesRuleViewModel viewModel);
		void ShowConditionalFormattingDateOccurringRuleForm(ConditionalFormattingDateOccurringRuleViewModel viewModel);
		void ShowConditionalFormattingBetweenRuleForm(ConditionalFormattingBetweenRuleViewModel viewModel);
		void ShowRenameSheetForm(RenameSheetViewModel viewModel);
#if !DXPORTABLE
		void ShowDataMemberEditorForm(DataMemberEditorViewModel viewModel);
		void ShowSelectDataMemberForm(SelectDataMemberViewModel viewModel);
		void ShowSelectDataSourceForm(SelectDataSourceViewModel viewModel);
		void ShowAddDataSourceForm(Action<object> callback);
		void ShowFilterEditorForm(FilterEditorViewModel viewModel);
		void ShowGroupEditorForm(GroupEditorViewModel viewModel);
		void ShowGroupRangeEditorForm(GroupRangeEditorViewModel viewModel);
		void ShowManageQueriesForm();
		void ShowManageRelationsForm();
		void ShowManageDataSourcesForm(ManageDataSourcesViewModel viewModel);
#endif
		void ShowOutlineSettingsForm(OutlineSettingsViewModel viewModel);
		void ShowOutlineSubtotalForm(OutlineSubtotalViewModel viewModel);
		void ShowGroupUngroupForm(GroupViewModel viewModel);
		void ShowMailMergePreviewForm();
		void ShowUnhideSheetForm(UnhideSheetViewModel viewModel);
		void ShowPasteSpecialForm(PasteSpecialInfo properties, ShowPasteSpecialFormCallback callback, object callbackData);
		void ShowPasteSpecialLocalForm(ModelPasteSpecialOptions properties, ShowPasteSpecialFormLocalCallback callback, object callbackData);
		void ShowMoveOrCopySheetForm(MoveOrCopySheetViewModel viewModel);
		void ShowTableInsertForm(InsertTableViewModel viewModel);
		void ShowChangeChartTypeForm(ChangeChartTypeViewModel viewModel);
		void ShowChangeChartTitleForm(ChangeChartTitleViewModel viewModel);
		void ShowChangeChartHorizontalAxisTitleForm(ChangeChartHorizontalAxisTitleViewModel viewModel);
		void ShowChangeChartVerticalAxisTitleForm(ChangeChartVerticalAxisTitleViewModel viewModel);
		void ShowChartSelectDataForm(ChartSelectDataViewModel viewModel);
		void ShowProtectSheetForm(ProtectSheetViewModel viewModel);
		void ShowProtectWorkbookForm(ProtectWorkbookViewModel viewModel);
		void ShowUnprotectSheetForm(UnprotectSheetViewModel viewModel);
		void ShowUnprotectWorkbookForm(UnprotectWorkbookViewModel viewModel);
		void ShowUnprotectRangeForm(UnprotectRangeViewModel viewModel);
		void ShowInsertFunctionForm(InsertFunctionViewModel viewModel);
		void ShowInsertSymbolForm(InsertSymbolViewModel viewModel);
		void ShowFindReplaceForm(FindReplaceViewModel viewModel);
		void ShowFunctionArgumentsForm(FunctionArgumentsViewModel viewModel);
		void ShowDefineNameForm(DefineNameViewModel viewModel);
		void ShowNameManagerForm(NameManagerViewModel viewModel);
		void ShowCreateDefinedNamesFromSelectionForm(CreateDefinedNamesFromSelectionViewModel viewModel);
		void ShowProtectedRangeForm(ProtectedRangeViewModel viewModel);
		void ShowProtectedRangePermissionsForm(ProtectedRangePermissionsViewModel viewModel);
		void ShowProtectedRangeManagerForm(ProtectedRangeManagerViewModel viewModel);
		void ShowRowHeightForm(RowHeightViewModel viewModel);
		void ShowColumnWidthForm(ColumnWidthViewModel viewModel);
		void ShowDefaultColumnWidthForm(DefaultColumnWidthViewModel viewModel);
		void ShowDocumentPropertiesForm(DocumentPropertiesViewModel viewModel);
		void ShowAutoFilterForm(AutoFilterViewModel viewModel);
		void ShowGenericFilterForm(GenericFilterViewModel viewModel);
		void ShowTop10FilterForm(Top10FilterViewModel viewModel);
		void ShowSimpleFilterForm(SimpleFilterViewModel viewModel);
		void ShowPageSetupForm(PageSetupViewModel viewModel, PageSetupFormInitialTabPage initialTabPage);
		void ShowHeaderFooterForm(HeaderFooterViewModel viewModel);
		void ShowDataValidationForm(DataValidationViewModel viewModel);
		void ShowInsertPivotTableForm(InsertPivotTableViewModel viewModel);
		void ShowOptionsPivotTableForm(OptionsPivotTableViewModel viewModel);
		void ShowMovePivotTableForm(MovePivotTableViewModel viewModel);
		void ShowChangeDataSourcePivotTableForm(ChangeDataSourcePivotTableViewModel viewModel);
		void ShowFieldSettingsPivotTableForm(FieldSettingsPivotTableViewModel viewModel);
		void ShowDataFieldSettingsPivotTableForm(DataFieldSettingsPivotTableViewModel viewModel);
		void ShowPivotTableFieldsFilterItemsForm(PivotTableFieldsFilterItemsViewModel viewModel);
		void ShowPivotTableAutoFilterForm();
		void ShowPivotTableValueFilterForm(PivotTableValueFiltersViewModel viewModel);
		void ShowPivotTableTop10FilterForm(PivotTableTop10FiltersViewModel viewModel);
		void ShowPivotTableDateFilterForm(PivotTableDateFiltersViewModel viewModel);
		void ShowPivotTableLabelFilterForm(PivotTableLabelFiltersViewModel viewModel);
		void ShowPivotTableShowValuesAsForm(PivotTableShowValuesAsViewModel viewModel);
		IPivotTableFieldsPanel CreatePivotTableFieldsPanel();
		SpreadsheetMouseHandlerStrategyFactory CreateMouseHandlerStrategyFactory();
		IPlatformSpecificScrollBarAdapter CreatePlatformSpecificScrollBarAdapter();
		SpreadsheetViewVerticalScrollController CreateSpreadsheetViewVerticalScrollController(SpreadsheetView view);
		SpreadsheetViewHorizontalScrollController CreateSpreadsheetViewHorizontalScrollController(SpreadsheetView view);
	}
	public delegate void ShowFormatCellsFormCallback(FormatCellsFormProperties properties);
	public delegate void ShowHyperlinkFormCallback(IHyperlinkViewInfo hyperlink);
	public delegate void ShowPasteSpecialFormCallback(PasteSpecialInfo properties, object data);
	public delegate void ShowPasteSpecialFormLocalCallback(ModelPasteSpecialOptions properties);
}
