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

using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region OptionsPivotTableCommand
	public class OptionsPivotTableCommand : PivotTableCommandBase {
		public OptionsPivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.OptionsPivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_OptionsPivotTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_OptionsPivotTableDescription; } }
		public override string ImageName { get { return "OptionsPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			if (InnerControl.AllowShowingForms) 
				Control.ShowOptionsPivotTableForm(CreateViewModel(table));
		}
		OptionsPivotTableViewModel CreateViewModel(PivotTable table) {
			OptionsPivotTableViewModel viewModel = new OptionsPivotTableViewModel(Control);
			viewModel.PivotTable = table;
			viewModel.Name = table.Name;
			CreateLayoutAndFormatPartViewModel(viewModel, table);
			CreateTotalsAndFiltersPartViewModel(viewModel, table);
			CreateDisplayPartViewModel(viewModel, table);
			CreatePrintingPartViewMidel(viewModel, table);
			CreateDataPartViewModel(viewModel, table);
			CreateAltTextPartViewModel(viewModel, table);
			return viewModel;
		}
		void CreateLayoutAndFormatPartViewModel(OptionsPivotTableViewModel viewModel, PivotTable table) {
			viewModel.MergeItem = table.MergeItem;
			viewModel.Indent = table.Indent;
			if (table.PageOverThenDown)
				viewModel.FieldsReportFilterAreaMode = viewModel.GetFilterAreaModeStringByEnum(FieldsReportFilterArea.OverThenDown);
			else
				viewModel.FieldsReportFilterAreaMode = viewModel.GetFilterAreaModeStringByEnum(FieldsReportFilterArea.DownThenOver);
			viewModel.PageOverThenDown = table.PageOverThenDown;
			viewModel.PageWrap = table.PageWrap;
			viewModel.ShowError = table.ShowError;
			viewModel.ErrorCaption = table.ErrorCaption;
			viewModel.ShowMissing = table.ShowMissing;
			viewModel.MissingCaption = table.MissingCaption;
			viewModel.UseAutoFormatting = table.UseAutoFormatting;
			viewModel.PreserveFormatting = table.PreserveFormatting;
		}
		void CreateTotalsAndFiltersPartViewModel(OptionsPivotTableViewModel viewModel, PivotTable table) {
			viewModel.RowGrandTotals = table.RowGrandTotals;
			viewModel.ColumnGrandTotals = table.ColumnGrandTotals;
			viewModel.SubtotalHiddenItems = table.SubtotalHiddenItems;
			viewModel.MultipleFieldFilters = table.MultipleFieldFilters;
			viewModel.CustomListSort = table.CustomListSort;
		}
		void CreateDisplayPartViewModel(OptionsPivotTableViewModel viewModel, PivotTable table) {
			viewModel.ShowDrill = table.ShowDrill;
			viewModel.ShowDataTips = table.ShowDataTips;
			viewModel.ShowMemberPropertyTips = table.ShowMemberPropertyTips;
			viewModel.ShowHeaders = table.ShowHeaders;
			viewModel.ShowValuesRow = table.ShowValuesRow;
			viewModel.ShowEmptyRow = table.ShowEmptyRow;
			viewModel.ShowEmptyColumn = table.ShowEmptyColumn;
			viewModel.ShowItems = table.ShowItems;
			viewModel.FieldListSortAscending = table.FieldListSortAscending;
		}
		void CreatePrintingPartViewMidel(OptionsPivotTableViewModel viewModel, PivotTable table) {
			viewModel.PrintDrill = table.PrintDrill;
			viewModel.ItemPrintTitles = table.ItemPrintTitles;
			viewModel.FieldPrintTitles = table.FieldPrintTitles;
		}
		void CreateDataPartViewModel(OptionsPivotTableViewModel viewModel, PivotTable table) {
			viewModel.SaveData = table.Cache.SaveData;
			viewModel.EnableDrill = table.EnableDrill;
			viewModel.RefreshOnLoad = table.Cache.RefreshOnLoad;
			if (table.Cache.MissingItemsLimit == PivotCache.DefaultMissingItemsLimit)
				viewModel.MissingItemsLimitMode = viewModel.GetMissingItemsLimitModeStringByEnum(MissingItemsLimit.Automatic);
			else if (table.Cache.MissingItemsLimit == PivotCache.NoneMissingItemsLimit)
				viewModel.MissingItemsLimitMode = viewModel.GetMissingItemsLimitModeStringByEnum(MissingItemsLimit.None);
			else
				viewModel.MissingItemsLimitMode = viewModel.GetMissingItemsLimitModeStringByEnum(MissingItemsLimit.Max);
			viewModel.MissingItemsLimitValue = table.Cache.MissingItemsLimit;
			viewModel.EditData = table.EditData;
		}
		void CreateAltTextPartViewModel(OptionsPivotTableViewModel viewModel, PivotTable table) {
			viewModel.AltText = table.AltText;
			viewModel.AltTextSummary = table.AltTextSummary;
		}
		protected internal bool Validate(OptionsPivotTableViewModel viewModel) {
			if (viewModel.Indent < 0 || viewModel.Indent > 127) {
				IModelErrorInfo error = new ModelErrorInfoWithArgs(ModelErrorType.IncorrectNumberRange, new string[2] { "0", "127" });
				Control.InnerControl.ErrorHandler.HandleError(error);
				return false;
			}
			if (viewModel.PageWrap < 0 || viewModel.PageWrap > 255) {
				IModelErrorInfo error = new ModelErrorInfoWithArgs(ModelErrorType.IncorrectNumberRange, new string[2] { "0", "255" });
				Control.InnerControl.ErrorHandler.HandleError(error);
				return false;
			}
			if (!CreatePivotTableRenameCommand(viewModel).Validate())
				return false;
			return true;
		}
		protected internal void ApplyChanges(OptionsPivotTableViewModel viewModel) {
			viewModel.PivotTable.BeginTransaction(ErrorHandler);
			try {
				CreatePivotTableRenameCommand(viewModel).ExecuteCore();
				ApplyChangesLayoutAndFormatPartViewModel(viewModel);
				ApplyChangesTotalsAndFiltersPartViewModel(viewModel);
				ApplyChangesDisplayPartViewModel(viewModel);
				ApplyChangesPrintingPartViewMidel(viewModel);
				ApplyChangesDataPartViewModel(viewModel);
				ApplyChangesAltTextPartViewModel(viewModel);
			}
			finally {
				viewModel.PivotTable.EndTransaction();
			}
		}
		void ApplyChangesLayoutAndFormatPartViewModel(OptionsPivotTableViewModel viewModel) {
			viewModel.PivotTable.MergeItem = viewModel.MergeItem;
			viewModel.PivotTable.Indent = viewModel.Indent;
			viewModel.PivotTable.PageOverThenDown = viewModel.PageOverThenDown;
			viewModel.PivotTable.PageWrap = viewModel.PageWrap;
			viewModel.PivotTable.ShowError = viewModel.ShowError;
			viewModel.PivotTable.ErrorCaption = viewModel.ErrorCaption;
			viewModel.PivotTable.ShowMissing = viewModel.ShowMissing;
			viewModel.PivotTable.MissingCaption = viewModel.MissingCaption;
			viewModel.PivotTable.UseAutoFormatting = viewModel.UseAutoFormatting;
			viewModel.PivotTable.PreserveFormatting = viewModel.PreserveFormatting;
		}
		void ApplyChangesTotalsAndFiltersPartViewModel(OptionsPivotTableViewModel viewModel) {
			viewModel.PivotTable.RowGrandTotals = viewModel.RowGrandTotals;
			viewModel.PivotTable.ColumnGrandTotals = viewModel.ColumnGrandTotals;
			viewModel.PivotTable.SubtotalHiddenItems = viewModel.SubtotalHiddenItems;
			viewModel.PivotTable.MultipleFieldFilters = viewModel.MultipleFieldFilters;
			viewModel.PivotTable.CustomListSort = viewModel.CustomListSort;
		}
		void ApplyChangesDisplayPartViewModel(OptionsPivotTableViewModel viewModel) {
			viewModel.PivotTable.ShowDrill = viewModel.ShowDrill;
			viewModel.PivotTable.ShowDataTips = viewModel.ShowDataTips;
			viewModel.PivotTable.ShowMemberPropertyTips = viewModel.ShowMemberPropertyTips;
			viewModel.PivotTable.ShowHeaders = viewModel.ShowHeaders;
			viewModel.PivotTable.ShowValuesRow = viewModel.ShowValuesRow;
			viewModel.PivotTable.ShowEmptyRow = viewModel.ShowEmptyRow;
			viewModel.PivotTable.ShowEmptyColumn = viewModel.ShowEmptyColumn;
			viewModel.PivotTable.ShowItems = viewModel.ShowItems;
			viewModel.PivotTable.FieldListSortAscending = viewModel.FieldListSortAscending;
		}
		void ApplyChangesPrintingPartViewMidel(OptionsPivotTableViewModel viewModel) {
			viewModel.PivotTable.PrintDrill = viewModel.PrintDrill;
			viewModel.PivotTable.ItemPrintTitles = viewModel.ItemPrintTitles;
			viewModel.PivotTable.FieldPrintTitles = viewModel.FieldPrintTitles;
		}
		void ApplyChangesDataPartViewModel(OptionsPivotTableViewModel viewModel) {
			viewModel.PivotTable.Cache.SaveData = viewModel.SaveData;
			viewModel.PivotTable.EnableDrill = viewModel.EnableDrill;
			viewModel.PivotTable.Cache.RefreshOnLoad = viewModel.RefreshOnLoad;
			viewModel.PivotTable.Cache.MissingItemsLimit = viewModel.MissingItemsLimitValue;
			viewModel.PivotTable.EditData = viewModel.EditData;
		}
		void ApplyChangesAltTextPartViewModel(OptionsPivotTableViewModel viewModel) {
			viewModel.PivotTable.AltText = viewModel.AltText;
			viewModel.PivotTable.AltTextSummary = viewModel.AltTextSummary;
		}
		PivotTableRenameCommand CreatePivotTableRenameCommand(OptionsPivotTableViewModel viewModel) {
			return new PivotTableRenameCommand(viewModel.PivotTable, viewModel.Name, Control.InnerControl.ErrorHandler);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region OptionsPivotTableContextMenuItemCommand
	public class OptionsPivotTableContextMenuItemCommand : OptionsPivotTableCommand {
		public OptionsPivotTableContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.OptionsPivotTableContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_OptionsPivotTableContextMenuItem; } }
		#endregion
	}
	#endregion
}
