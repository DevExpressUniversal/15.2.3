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
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PivotTableFieldsFilterItemsCommandBase
	public abstract class PivotTableFieldsFilterItemsCommandBase : PivotTableCommandBase {
		protected PivotTableFieldsFilterItemsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		public abstract PivotTableAxis Axis { get; }
		protected internal override void ExecuteCore() {
			PivotTable table = ActiveSheet.PivotTables[ActiveSheet.PivotTableStaticInfo.TableIndex];
			if (table == null)
				return;
			if (InnerControl.AllowShowingForms)
				Control.ShowPivotTableFieldsFilterItemsForm(CreateViewModel(table));
		}
		public static string GetItemName(PivotTable table, int fieldIndex, int ItemIndex) {
			WorkbookDataContext dataContext = table.DocumentModel.DataContext;
			FormattedVariantValue formattedValue = PivotRefreshDataOnWorksheetCommand.GetItemHeaderCaption(table, fieldIndex, ItemIndex, dataContext);
			NumberFormat numberFormat = table.DocumentModel.Cache.NumberFormatCache[formattedValue.NumberFormatId];
			return numberFormat.Format(formattedValue.Value, table.DocumentModel.DataContext).Text;
		}
		protected static int GetVisibleItemCount(List<PivotItemFilterInfo> items) {
			int itemCount = 0;
			foreach (PivotItemFilterInfo itemInfo in items)
				if (itemInfo.IsVisible)
					itemCount++;
			return itemCount;
		}
		protected static int GetIndexOfOneVisibleItem(List<PivotItemFilterInfo> items) {
			foreach (PivotItemFilterInfo itemInfo in items)
				if (itemInfo.IsVisible)
					return itemInfo.Index;
			return -1;
		}
		protected abstract PivotTableFieldsFilterItemsViewModel CreateViewModel(PivotTable table);
		protected internal bool Validate(PivotTableFieldsFilterItemsViewModel viewModel) {
			foreach (PivotItemFilterInfo item in viewModel.Items)
				if (item.IsVisible)
					return true;
			return false;
		}
		protected override bool GetVisible(PivotTable table) {
			return !InnerControl.IsAnyInplaceEditorActive;
		}
		protected override bool GetEnabled(PivotTable table) {
			return GetVisible(table);
		}
	}
	#endregion
	#region PivotTableNonPageFieldsFilterItemsCommandBase
	public class PivotTableNonPageFieldsFilterItemsCommand : PivotTableFieldsFilterItemsCommandBase {
		public PivotTableNonPageFieldsFilterItemsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableItemFilter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableItemFilterDescription; } }
		public override PivotTableAxis Axis { get { return PivotTableAxis.None; } }
		#endregion
		protected override PivotTableFieldsFilterItemsViewModel CreateViewModel(PivotTable table) {
			PivotTableFieldsFilterItemsViewModel viewModel = new PivotTableFieldsFilterItemsViewModel(Control);
			PivotField field = table.Fields[ActiveSheet.PivotTableStaticInfo.FieldIndex];
			viewModel.PivotTable = table;
			viewModel.Field = field;
			viewModel.Items = GetPageFieldItems(table, field, ActiveSheet.PivotTableStaticInfo.FieldIndex);
			viewModel.SelectMultipleItems = false;
			viewModel.Axis = Axis;
			return viewModel;
		}
		public List<PivotItemFilterInfo> GetPageFieldItems(PivotTable table, PivotField field, int fieldIndex) {
			string itemName = string.Empty;
			List<PivotItemFilterInfo> result = new List<PivotItemFilterInfo>();
			for (int i = 0; i < field.Items.DataItemsCount; i++) {
				PivotItem item = field.Items[i];
				itemName = GetItemName(table, fieldIndex, i);
				result.Add(new PivotItemFilterInfo(itemName, i, !item.IsHidden));
			}
			return result;
		}
		void ShowPivotItems(PivotTableFieldsFilterItemsViewModel viewModel) {
			for (int i = 0; i < viewModel.Field.Items.DataItemsCount; i++) {
				PivotItem item = viewModel.Field.Items[i];
				item.IsHidden = true;
			}
			for (int i = 0; i < viewModel.Items.Count; i++)
				if (viewModel.Items[i].IsVisible) {
					int idx = viewModel.Items[i].Index;
					viewModel.Field.Items[idx].IsHidden = false;
				}
		}
		protected internal void ApplyChanges(PivotTableFieldsFilterItemsViewModel viewModel) {
			if (!viewModel.PivotTable.MultipleFieldFilters)
				viewModel.PivotTable.ClearFieldFilters(ActiveSheet.PivotTableStaticInfo.FieldIndex, PivotFilterClearType.AllExceptManual, viewModel.Control.InnerControl.ErrorHandler);
			int visibleItemCount = GetVisibleItemCount(viewModel.Items);
			IErrorHandler errorHandler = viewModel.Control.InnerControl.ErrorHandler;
			PivotField field = viewModel.Field;
			viewModel.PivotTable.BeginTransaction(errorHandler);
			try {
				if (visibleItemCount == 1)
					new ShowSinglePivotItemCommand(field, GetIndexOfOneVisibleItem(viewModel.Items), errorHandler).Execute();
				else if (visibleItemCount == viewModel.Items.Count)
					new ShowAllPivotItemsCommand(field, errorHandler).Execute();
				else {
					ShowPivotItems(viewModel);
				}
			}
			finally {
				viewModel.PivotTable.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTablePageFieldsFilterItemsCommand
	public class PivotTablePageFieldsFilterItemsCommand : PivotTableFieldsFilterItemsCommandBase {
		public PivotTablePageFieldsFilterItemsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTablePageFieldsFilterItems; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override PivotTableAxis Axis { get { return PivotTableAxis.Page; } }
		#endregion
		protected override PivotTableFieldsFilterItemsViewModel CreateViewModel(PivotTable table) {
			PivotTableFieldsFilterItemsViewModel viewModel = new PivotTableFieldsFilterItemsViewModel(Control);
			PivotField field = table.Fields[ActiveSheet.PivotTableStaticInfo.FieldIndex];
			viewModel.PivotTable = table;
			viewModel.Field = field;
			viewModel.Items = GetPageFieldItems(table, field, ActiveSheet.PivotTableStaticInfo.FieldIndex);
			viewModel.SelectMultipleItems = field.MultipleItemSelectionAllowed;
			viewModel.Axis = Axis;
			return viewModel;
		}
		public List<PivotItemFilterInfo> GetPageFieldItems(PivotTable table, PivotField field, int fieldIndex) {
			string itemName = string.Empty;
			List<PivotItemFilterInfo> result = new List<PivotItemFilterInfo>();
			for (int i = 0; i < field.Items.DataItemsCount; i++) {
				PivotItem item = field.Items[i];
				if (!field.ShowItemsWithNoData && item.HasMissingValue)
					continue;
				itemName = GetItemName(table, fieldIndex, i);
				result.Add(new PivotItemFilterInfo(itemName, i, !item.IsHidden));
			}
			return result;
		}
		void ShowPivotItems(PivotTableFieldsFilterItemsViewModel viewModel) {
			for (int i = 0; i < viewModel.Field.Items.DataItemsCount; i++) {
				PivotItem item = viewModel.Field.Items[i];
				if (!viewModel.Field.ShowItemsWithNoData && item.HasMissingValue)
					continue;
				item.IsHidden = true;
			}
			for (int i = 0; i < viewModel.Items.Count; i++)
				if (viewModel.Items[i].IsVisible) {
					int idx = viewModel.Items[i].Index;
					viewModel.Field.Items[idx].IsHidden = false;
				}
		}
		protected internal void ApplyChanges(PivotTableFieldsFilterItemsViewModel viewModel) {
			int visibleItemCount = GetVisibleItemCount(viewModel.Items);
			viewModel.PivotTable.BeginTransaction(ErrorHandler);
			try {
				viewModel.Field.MultipleItemSelectionAllowed = viewModel.SelectMultipleItems;
				if (!viewModel.SelectMultipleItems)
					new TurnOffShowSingleItemCommand(viewModel.Field, ErrorHandler).Execute();
				if (!viewModel.SelectMultipleItems && visibleItemCount == 1)
					new ShowSinglePivotItemCommand(viewModel.Field, GetIndexOfOneVisibleItem(viewModel.Items), ErrorHandler).Execute();
				else if (viewModel.SelectMultipleItems && visibleItemCount == 1) {
					ShowPivotItems(viewModel);
				}
				else if (visibleItemCount == viewModel.Items.Count)
					new ShowAllPivotItemsCommand(viewModel.Field, ErrorHandler).Execute();
				else {
					ShowPivotItems(viewModel);
				}
			}
			finally {
				viewModel.PivotTable.EndTransaction();
			}
		}
	}
	#endregion
	#region PivotTableRowFieldsFilterItemsCommand
	public class PivotTableRowFieldsFilterItemsCommand : PivotTableNonPageFieldsFilterItemsCommand {
		public PivotTableRowFieldsFilterItemsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableRowFieldsFilterItems; } }
		public override PivotTableAxis Axis { get { return PivotTableAxis.Row; } }
		#endregion
		protected override bool GetVisible(PivotTable table) {
			return !ActiveSheet.PivotTableStaticInfo.IsPageFilter && ActiveSheet.PivotTableStaticInfo.IsRowFieldsFilter;
		}
		protected override bool GetEnabled(PivotTable table) {
			return GetVisible(table);
		}
	}
	#endregion
	#region PivotTableColumnFieldsFilterItemsCommand
	public class PivotTableColumnFieldsFilterItemsCommand : PivotTableNonPageFieldsFilterItemsCommand {
		public PivotTableColumnFieldsFilterItemsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableColumnFieldsFilterItems; } }
		public override PivotTableAxis Axis { get { return PivotTableAxis.Column; } }
		#endregion
		protected override bool GetVisible(PivotTable table) {
			return !ActiveSheet.PivotTableStaticInfo.IsPageFilter && !ActiveSheet.PivotTableStaticInfo.IsRowFieldsFilter;
		}
		protected override bool GetEnabled(PivotTable table) {
			return GetVisible(table);
		}
	}
	#endregion
}
