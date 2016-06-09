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
using System;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FieldSettingsPivotTableCommand
	public class FieldSettingsPivotTableCommand : SelectFieldTypePivotTableCommand {
		public FieldSettingsPivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FieldSettingsPivotTable; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			int fieldIndex = GetPivotFieldIndex(table);
			if (IsPivotFieldValid(table, fieldIndex))
				if (InnerControl.AllowShowingForms)
					Control.ShowFieldSettingsPivotTableForm(CreateViewModel(table, fieldIndex));
		}
		FieldSettingsPivotTableViewModel CreateViewModel(PivotTable table, int index) {
			IPivotCacheField cacheField = table.Cache.CacheFields[index];
			PivotField field = table.Fields[index];
			FieldSettingsPivotTableViewModel viewModel = new FieldSettingsPivotTableViewModel(Control);
			viewModel.PivotTable = table;
			viewModel.PivotField = field;
			if (!String.IsNullOrEmpty(cacheField.Caption))
				viewModel.SourceName = cacheField.Caption;
			else
				viewModel.SourceName = cacheField.Name;
			viewModel.CustomName = table.GetFieldCaption(index);
			if (field.Subtotal == PivotFieldItemType.DefaultValue) {
				viewModel.IsAutomaticSubtotal = true;
				viewModel.IsNoneSubtotal = false;
				viewModel.IsCustomSubtotal = false;
			}
			else if (field.Subtotal == PivotFieldItemType.Blank) {
				viewModel.IsAutomaticSubtotal = false;
				viewModel.IsNoneSubtotal = true;
				viewModel.IsCustomSubtotal = false;
			}
			else {
				viewModel.IsAutomaticSubtotal = false;
				viewModel.IsNoneSubtotal = false;
				viewModel.IsCustomSubtotal = true;
			}
			viewModel.Subtotal = field.Subtotal;
			viewModel.IncludeNewItemsInFilter = field.IncludeNewItemsInFilter;
			viewModel.Outline = field.Outline;
			viewModel.Compact = field.Compact;
			viewModel.SubtotalTop = field.SubtotalTop;
			viewModel.Tabular = !field.Outline;
			viewModel.FillDownLabels = field.FillDownLabels;
			viewModel.InsertBlankRow = field.InsertBlankRow;
			viewModel.ShowItemsWithNoData = field.ShowItemsWithNoData;
			viewModel.InsertPageBreak = field.InsertPageBreak;
			return viewModel;
		}
		bool IsPivotFieldValid(PivotTable table, int index) {
			if (table == null || index == -1)
				return false;
			return IsPivotFieldZone(table);
		}
		protected bool IsPivotFieldZone(PivotTable table) {
			PivotFieldZoneInfo zoneInfo = table.CalculationInfo.GetActiveFieldInfo(ActiveSheet.Selection.ActiveCell);
			if (zoneInfo.FieldIndex == -1)
				return false;
			return (zoneInfo.Axis != PivotTableAxis.None && zoneInfo.Axis != PivotTableAxis.Value) ? true : false;
		}
		protected internal bool Validate(FieldSettingsPivotTableViewModel viewModel) {
			return CreatePivotFieldRenameCommand(viewModel).Validate();
		}
		protected internal void ApplyChanges(FieldSettingsPivotTableViewModel viewModel) {
			viewModel.PivotTable.BeginTransaction(ErrorHandler);
			try {
				PivotFieldRenameCommand fieldRenameCommand = CreatePivotFieldRenameCommand(viewModel);
				if (fieldRenameCommand.Validate())
					fieldRenameCommand.ExecuteCore();
				viewModel.PivotField.Subtotal = viewModel.Subtotal;
				viewModel.PivotField.IncludeNewItemsInFilter = viewModel.IncludeNewItemsInFilter;
				viewModel.PivotField.Outline = viewModel.Outline;
				viewModel.PivotField.Compact = viewModel.Compact;
				viewModel.PivotField.SubtotalTop = viewModel.SubtotalTop;
				viewModel.PivotField.FillDownLabels = viewModel.FillDownLabels;
				viewModel.PivotField.InsertBlankRow = viewModel.InsertBlankRow;
				viewModel.PivotField.ShowItemsWithNoData = viewModel.ShowItemsWithNoData;
				viewModel.PivotField.InsertPageBreak = viewModel.InsertPageBreak;
			}
			finally {
				viewModel.PivotTable.EndTransaction();
			}
		}
		PivotFieldRenameCommand CreatePivotFieldRenameCommand(FieldSettingsPivotTableViewModel viewModel) {
			return new PivotFieldRenameCommand(viewModel.PivotField, viewModel.CustomName, ErrorHandler);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region FieldSettingsPivotTableContextMenuItemCommand
	public class FieldSettingsPivotTableContextMenuItemCommand : FieldSettingsPivotTableCommand {
		public FieldSettingsPivotTableContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FieldSettingsPivotTableContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FieldSettingsPivotTableContextMenuItem; } }
		#endregion
		protected override bool GetVisible(PivotTable table) {
			if (table == null)
				return false;
			int index = GetPivotFieldIndex(table);
			if (index == -1)
				return false;
			return IsPivotFieldZone(table);
		}
		protected override bool GetEnabled(PivotTable table) {
			if (table == null)
				return false;
			int index = GetPivotFieldIndex(table);
			if (index == -1 || InnerControl.IsAnyInplaceEditorActive || !table.EnableFieldProperties)
				return false;
			return IsPivotFieldZone(table);
		}
	}
	#endregion
}
