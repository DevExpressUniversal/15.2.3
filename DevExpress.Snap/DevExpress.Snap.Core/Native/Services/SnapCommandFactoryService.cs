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
using System.Reflection;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
using DevExpress.Snap.Core.Commands;
using DevExpress.Office.Utils;
namespace DevExpress.Snap.Core.Native.Services {	
	public class SnapCommandFactoryService : RichEditCommandFactoryServiceWrapper {
		#region Fields
		readonly IRichEditControl control;
		static readonly Type[] constructorParametersInterface = new Type[] { typeof(IRichEditControl) };
		readonly RichEditCommandConstructorTable commandConstructorTable;
		#endregion
		public SnapCommandFactoryService(IRichEditControl control, IRichEditCommandFactoryService baseService)
			: base(baseService) {
				Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.commandConstructorTable = CreateCommandConstructorTable();
		}
		public IRichEditControl Control { get { return control; } }
		 protected internal RichEditCommandConstructorTable CreateCommandConstructorTable() {
			RichEditCommandConstructorTable result = new RichEditCommandConstructorTable();
			PopulateConstructorTable(result);
			return result;
		}
		protected void AddCommandConstructor(RichEditCommandConstructorTable table, RichEditCommandId commandId, Type commandType) {
			ConstructorInfo ci = GetConstructorInfo(commandType);
			if (ci == null)
				Exceptions.ThrowArgumentException("commandType", commandType);
			table.Add(commandId, ci);
		}
		protected internal virtual ConstructorInfo GetConstructorInfo(Type commandType) {
			return commandType.GetConstructor(constructorParametersInterface);
		}
		protected internal virtual void PopulateConstructorTable(RichEditCommandConstructorTable table) {
			AddCommandConstructor(table, SnapCommandId.SortFieldAscending, typeof(SortFieldAscendingCommand));
			AddCommandConstructor(table, SnapCommandId.SortFieldDescending, typeof(SortFieldDescendingCommand));
			AddCommandConstructor(table, SnapCommandId.InsertGroupSeparator, typeof(InsertGroupSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertPageBreakGroupSeparator, typeof(InsertPageBreakGroupSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertNoneGroupSeparator, typeof(InsertNoneGroupSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertEmptyParagraphGroupSeparator, typeof(InsertEmptyParagraphGroupSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertEmptyRowGroupSeparator, typeof(InsertEmptyRowGroupSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertSectionBreakNextPageGroupSeparator, typeof(InsertSectionBreakNextPageGroupSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertSectionBreakEvenPageGroupSeparator, typeof(InsertSectionBreakEvenPageGroupSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertSectionBreakOddPageGroupSeparator, typeof(InsertSectionBreakOddPageGroupSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertDataRowSeparator, typeof(InsertDataRowSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertPageBreakDataRowSeparator, typeof(InsertPageBreakDataRowSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertNoneDataRowSeparator, typeof(InsertNoneDataRowSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertEmptyParagraphDataRowSeparator, typeof(InsertEmptyParagraphDataRowSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertEmptyRowDataRowSeparator, typeof(InsertEmptyRowDataRowSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertSectionBreakNextPageDataRowSeparator, typeof(InsertSectionBreakNextPageDataRowSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertSectionBreakEvenPageDataRowSeparator, typeof(InsertSectionBreakEvenPageDataRowSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.InsertSectionBreakOddPageDataRowSeparator, typeof(InsertSectionBreakOddPageDataRowSeparatorCommand));
			AddCommandConstructor(table, SnapCommandId.FilterList, typeof(FilterListCommand));
			AddCommandConstructor(table, SnapCommandId.ListHeader, typeof(ListHeaderCommand));
			AddCommandConstructor(table, SnapCommandId.ListFooter, typeof(ListFooterCommand));
			AddCommandConstructor(table, SnapCommandId.InsertListHeader, typeof(InsertListHeaderCommand));
			AddCommandConstructor(table, SnapCommandId.InsertListFooter, typeof(InsertListFooterCommand));
			AddCommandConstructor(table, SnapCommandId.RemoveListHeader, typeof(RemoveListHeaderCommand));
			AddCommandConstructor(table, SnapCommandId.RemoveListFooter, typeof(RemoveListFooterCommand));
			AddCommandConstructor(table, SnapCommandId.GroupHeader, typeof(GroupHeaderCommand));
			AddCommandConstructor(table, SnapCommandId.GroupFooter, typeof(GroupFooterCommand));
			AddCommandConstructor(table, SnapCommandId.InsertGroupHeader, typeof(InsertGroupHeaderCommand));
			AddCommandConstructor(table, SnapCommandId.InsertGroupFooter, typeof(InsertGroupFooterCommand));
			AddCommandConstructor(table, SnapCommandId.RemoveGroupHeader, typeof(RemoveGroupHeaderCommand));
			AddCommandConstructor(table, SnapCommandId.RemoveGroupFooter, typeof(RemoveGroupFooterCommand));
			AddCommandConstructor(table, SnapCommandId.GroupByField, typeof(GroupByFieldCommand));
			AddCommandConstructor(table, SnapCommandId.InsertCheckBox, typeof(InsertCheckBoxCommand));
			AddCommandConstructor(table, SnapCommandId.InsertBarCode, typeof(InsertBarCodeCommand));
			AddCommandConstructor(table, SnapCommandId.InsertSparkline, typeof(InsertSparklineCommand));
			AddCommandConstructor(table, SnapCommandId.InsertChart, typeof(InsertChartCommand));
			AddCommandConstructor(table, SnapCommandId.InsertIndex, typeof(InsertIndexCommand));
			AddCommandConstructor(table, SnapCommandId.ConvertToParagraphs, typeof(ConvertToParagraphsCommand));
			AddCommandConstructor(table, SnapCommandId.PasteSnxCommand, typeof(PasteSnxCommand));
			AddCommandConstructor(table, SnapCommandId.ChangeTheme, typeof(ChangeThemeCommand));
			AddCommandConstructor(table, SnapCommandId.ToggleFieldHighlighting, typeof(ToggleFieldHighlightingCommand));
			AddCommandConstructor(table, SnapCommandId.CreateFieldForTemplate, typeof(CreateFieldForTemplate));
			AddCommandConstructor(table, SnapCommandId.SummaryByField, typeof(SummaryCommand));
			AddCommandConstructor(table, SnapCommandId.SummaryCount, typeof(SummaryCountCommand));
			AddCommandConstructor(table, SnapCommandId.SummarySum, typeof(SummarySumCommand));
			AddCommandConstructor(table, SnapCommandId.SummaryAverage, typeof(SummaryAverageCommand));
			AddCommandConstructor(table, SnapCommandId.SummaryMin, typeof(SummaryMinCommand));
			AddCommandConstructor(table, SnapCommandId.SummaryMax, typeof(SummaryMaxCommand));
			AddCommandConstructor(table, SnapCommandId.ChangeEditorRowLimit, typeof(ChangeEditorRowLimitCommand));
			AddCommandConstructor(table, SnapCommandId.FilterField, typeof(FilterFieldCommand));
			AddCommandConstructor(table, SnapCommandId.FilterFieldPlaceHolder, typeof(FilterFieldPlaceHolderCommand));
			AddCommandConstructor(table, SnapCommandId.HighlightActiveElement, typeof(HighlightActiveElementCommand));
			AddCommandConstructor(table, SnapCommandId.GroupFieldsCollection, typeof(GroupFieldsCollectionCommand));
			AddCommandConstructor(table, SnapCommandId.DeleteList, typeof(DeleteListCommand));
			AddCommandConstructor(table, SnapCommandId.ShowReportStructureEditorForm, typeof(ShowReportStructureEditorFormCommand));
			AddCommandConstructor(table, SnapCommandId.NewDataSource, typeof(NewDataSourceCommand));
			AddCommandConstructor(table, SnapCommandId.ChangeTableCellStyle, typeof(ChangeTableCellStyleCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableRowAbove, typeof(SnapInsertTableRowAboveCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableRowBelow, typeof(SnapInsertTableRowBelowCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableColumnToTheLeft, typeof(SnapInsertTableColumnToTheLeftCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableColumnToTheRight, typeof(SnapInsertTableColumnToTheRightCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTableColumns, typeof(SnapDeleteTableColumnsCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTableColumnsMenuItem, typeof(SnapDeleteTableColumnsMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.MergeTableElement, typeof(SnapMergeTableElementMenuCommand));
			AddCommandConstructor(table, RichEditCommandId.MergeTableCells, typeof(SnapMergeTableCellsCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTableRows, typeof(SnapDeleteTableRowsCommand));
			AddCommandConstructor(table, RichEditCommandId.SelectUpperLevelObject, typeof(SnapSelectUpperLevelObjectCommand));
			AddCommandConstructor(table, RichEditCommandId.DeleteTable, typeof(SnapDeleteTableCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowDeleteTableCellsForm, typeof(SnapShowDeleteTableCellsFormCommand));
			AddCommandConstructor(table, RichEditCommandId.ShowDeleteTableCellsFormMenuItem, typeof(SnapShowDeleteTableCellsFormMenuCommand));
			AddCommandConstructor(table, SnapCommandId.MailMergeDataSource, typeof(ChangeMailMergeDataSourceCommand));
			AddCommandConstructor(table, SnapCommandId.MailMergeFilters, typeof(MailMergeFiltersCommand));
			AddCommandConstructor(table, SnapCommandId.MailMergeSorting, typeof(MailMergeSortingCommand));
			AddCommandConstructor(table, SnapCommandId.MailMergeCurrentRecord, typeof(ChangeMailMergeCurrentRecordCommand));
			AddCommandConstructor(table, SnapCommandId.SnapMailMergePrintPreview, typeof(SnapMailMergePrintPreviewCommand));
			AddCommandConstructor(table, SnapCommandId.SnapMailMergePrint, typeof(SnapMailMergePrintCommand));
			AddCommandConstructor(table, SnapCommandId.SnapMailMergeQuickPrint, typeof(SnapMailMergeQuickPrintCommand));
			AddCommandConstructor(table, SnapCommandId.ExportDocument, typeof(ExportDocumentCommand));
			AddCommandConstructor(table, SnapCommandId.SnapMailMergeExportDocument, typeof(SnapMailMergeExportDocumentCommand));
			AddCommandConstructor(table, SnapCommandId.FinishAndMerge, typeof(FinishAndMergeCommand));
			AddCommandConstructor(table, SnapCommandId.MailMergeFilterField, typeof(MailMergeFilterFieldCommand));
			AddCommandConstructor(table, SnapCommandId.SnapFilterField, typeof(SnapFilterFieldCommand));
			AddCommandConstructor(table, SnapCommandId.MailMergeSortFieldAscending, typeof(MailMergeSortFieldAscendingCommand));
			AddCommandConstructor(table, SnapCommandId.MailMergeSortFieldDescending, typeof(MailMergeSortFieldDescendingCommand));
			AddCommandConstructor(table, SnapCommandId.SnapSortFieldAscending, typeof(SnapSortFieldAscendingCommand));
			AddCommandConstructor(table, SnapCommandId.SnapSortFieldDescending, typeof(SnapSortFieldDescendingCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableOfContents, typeof(SnapInsertTableOfContentsCommand));
			AddCommandConstructor(table, RichEditCommandId.UpdateTableOfContents, typeof(SnapUpdateTableOfContentsCommand));
			AddCommandConstructor(table, RichEditCommandId.UpdateTableOfFigures, typeof(SnapUpdateTableOfFiguresCommand));
			AddCommandConstructor(table, RichEditCommandId.InsertTableOfFiguresPlaceholder, typeof(SnapInsertTableOfFiguresPlaceholderCommand));
		}
		public override RichEditCommand CreateCommand(RichEditCommandId commandId) {
			ConstructorInfo ci;
			if (commandConstructorTable.TryGetValue(commandId, out ci))
				return (RichEditCommand)ci.Invoke(new object[] { Control });
			return base.CreateCommand(commandId);
		}
	}
}
