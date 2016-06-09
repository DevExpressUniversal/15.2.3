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
	#region DataFieldSettingsPivotTableCommand
	public class DataFieldSettingsPivotTableCommand : SelectFieldTypePivotTableCommand {
		public DataFieldSettingsPivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFieldSettingsPivotTable; } }
		public virtual DataFieldSettingsInitialTabPage InitialTabPage { get { return DataFieldSettingsInitialTabPage.SummarizeValuesBy; } }
		#endregion
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			int fieldIndex = GetPivotFieldIndex(table);
			if (IsDataFieldValid(table, fieldIndex))
				if (InnerControl.AllowShowingForms)
					Control.ShowDataFieldSettingsPivotTableForm(CreateViewModel(table, fieldIndex));
		}
		DataFieldSettingsPivotTableViewModel CreateViewModel(PivotTable table, int index) {
			PivotDataField dataField = GetDataField(table, index);
			IPivotCacheField cacheField = table.Cache.CacheFields[index];
			DataFieldSettingsPivotTableViewModel viewModel = new DataFieldSettingsPivotTableViewModel(Control, InitialTabPage);
			viewModel.PivotTable = table;
			viewModel.ErrorHandler = ErrorHandler;
			if (!String.IsNullOrEmpty(cacheField.Caption))
				viewModel.SourceName = cacheField.Caption;
			else
				viewModel.SourceName = cacheField.Name;
			if (dataField != null) {
				viewModel.PivotDataField = dataField;
				viewModel.Subtotal = viewModel.GetSubtotalStringByEnum(dataField.Subtotal);
				viewModel.CustomName = dataField.Name;
				viewModel.ShowDataAs = viewModel.GetShowDataAsStringByEnum(dataField.ShowDataAs);
				viewModel.NumberFormatIndex = dataField.NumberFormatIndex;
				viewModel.BaseField = dataField.BaseField;
				viewModel.BaseFieldTable = viewModel.PopulateBaseFieldTable();
				viewModel.BaseFieldName = viewModel.GetBaseFieldNameStringByInt(viewModel.BaseField);
				viewModel.BaseItemTable = viewModel.PopulateBaseItemTable(viewModel.BaseField, false);
				if (dataField.BaseItem > PivotTableLayoutCalculator.NextItem)
					viewModel.BaseItem = 0;
				else
					viewModel.BaseItem = dataField.BaseItem;
				viewModel.BaseItemName = viewModel.GetBaseItemNameStringByInt(viewModel.BaseItem);
			}
			return viewModel;
		}
		PivotDataField GetDataField(PivotTable table, int index) {
			foreach (PivotDataField field in table.DataFields)
				if (field.FieldIndex == index)
					return field;
			return null;
		}
		bool IsDataFieldValid(PivotTable table, int index) {
			if (table == null || index == -1)
				return false;
			return IsDataFieldZone(table);
		}
		protected bool IsDataFieldZone(PivotTable table) {
			PivotFieldZoneInfo zoneInfo = table.CalculationInfo.GetActiveFieldInfo(ActiveSheet.Selection.ActiveCell);
			if (zoneInfo.FieldIndex == -1)
				return false;
			return (zoneInfo.Axis != PivotTableAxis.None && zoneInfo.Axis == PivotTableAxis.Value) ? true : false;
		}
		protected internal bool Validate(DataFieldSettingsPivotTableViewModel viewModel) {
			return CreatePivotDataFieldRenameCommand(viewModel).Validate();
		}
		protected internal void ApplyChanges(DataFieldSettingsPivotTableViewModel viewModel) {
			viewModel.PivotTable.BeginTransaction(ErrorHandler);
			try {
				PivotDataFieldRenameCommand dataFieldRenameCommand = CreatePivotDataFieldRenameCommand(viewModel);
				if (dataFieldRenameCommand.Validate())
					dataFieldRenameCommand.ExecuteCore();
				viewModel.PivotDataField.Subtotal = viewModel.GetSubtotalByString(viewModel.Subtotal);
				viewModel.PivotDataField.ShowDataAs = viewModel.GetShowDataAsByString(viewModel.ShowDataAs);
				viewModel.PivotDataField.NumberFormatIndex = viewModel.NumberFormatIndex;
				viewModel.PivotDataField.BaseField = viewModel.BaseField;
				viewModel.PivotDataField.BaseItem = viewModel.BaseItem;
			}
			finally {
				viewModel.PivotTable.EndTransaction();
			}
		}
		PivotDataFieldRenameCommand CreatePivotDataFieldRenameCommand(DataFieldSettingsPivotTableViewModel viewModel) {
			return new PivotDataFieldRenameCommand(viewModel.PivotDataField, viewModel.CustomName, ErrorHandler);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableSummarizeValuesByMoreOptionsCommand
	public class PivotTableSummarizeValuesByMoreOptionsCommand : DataFieldSettingsPivotTableCommand {
		public PivotTableSummarizeValuesByMoreOptionsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableSummarizeValuesByMoreOptions; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableMoreOptions; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override DataFieldSettingsInitialTabPage InitialTabPage { get { return base.InitialTabPage; } }
		public override string ImageName { get { return String.Empty; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsMoreOptionsPivotTableCommand
	public class PivotTableShowValuesAsMoreOptionsCommand : DataFieldSettingsPivotTableCommand {
		public PivotTableShowValuesAsMoreOptionsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsMoreOptions; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableMoreOptions; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override DataFieldSettingsInitialTabPage InitialTabPage { get { return DataFieldSettingsInitialTabPage.ShowValuesAs; } }
		public override string ImageName { get { return String.Empty; } }
		#endregion
	}
	#endregion
	#region DataFieldSettingsPivotTableContextMenuItemCommand
	public class DataFieldSettingsPivotTableContextMenuItemCommand : DataFieldSettingsPivotTableCommand {
		public DataFieldSettingsPivotTableContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.DataFieldSettingsPivotTableContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFieldSettingsPivotTableContextMenuItem; } }
		#endregion
		protected override bool GetVisible(PivotTable table) {
			if (table == null)
				return false;
			int index = GetPivotFieldIndex(table);
			if (index == -1)
				return false;
			return IsDataFieldZone(table);
		}
		protected override bool GetEnabled(PivotTable table) {
			if (table == null)
				return false;
			int index = GetPivotFieldIndex(table);
			if (index == -1 || InnerControl.IsAnyInplaceEditorActive || !table.EnableFieldProperties)
				return false;
			return IsDataFieldZone(table);
		}
	}
	#endregion
}
