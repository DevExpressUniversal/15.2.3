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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PivotTableExpandCollapseFieldCommandBase (abstract class)
	public abstract class PivotTableExpandCollapseFieldCommandBase : PivotTableCommandBase {
		protected PivotTableExpandCollapseFieldCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			PivotTable table = TryGetPivotTable();
			if (table == null)
				return;
			PivotFieldZoneInfo zoneInfo = table.CalculationInfo.GetActiveFieldInfo(ActiveSheet.Selection.ActiveCell);
			if (zoneInfo.FieldIndex == -1)
				return;
			table.BeginTransaction(ErrorHandler);
			try {
				if (zoneInfo.Axis == PivotTableAxis.Row) 
					ModifyPivotItemHideDetails(table, zoneInfo.FieldIndex, table.RowFields);
				else
					ModifyPivotItemHideDetails(table, zoneInfo.FieldIndex, table.ColumnFields);
			}
			finally {
				table.EndTransaction();
			}
		}
		protected override bool GetEnabled(PivotTable table) {
			if (table == null)
				return false;
			return GetState(table);
		}
		public static bool GetState(PivotTable table) {
			Worksheet activeSheet = table.DocumentModel.ActiveSheet;
			SheetViewSelection selection = activeSheet.Selection;
			table = activeSheet.TryGetPivotTable(selection.ActiveCell);
			PivotFieldZoneInfo zoneInfo = table.CalculationInfo.GetActiveFieldInfo(activeSheet.Selection.ActiveCell);
			if (zoneInfo.FieldIndex != -1 && (zoneInfo.Axis == PivotTableAxis.Row || zoneInfo.Axis == PivotTableAxis.Column))
				return true;
			return false;
		}
		protected abstract void ModifyPivotItemHideDetails(PivotTable table, int fieldIndex, PivotTableColumnRowFieldIndices fields);
	}
	#endregion
	#region PivotTableExpandFieldCommand
	public class PivotTableExpandFieldCommand : PivotTableExpandCollapseFieldCommandBase {
		public PivotTableExpandFieldCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableExpandField; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableExpandField; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableExpandFieldDescription; } }
		public override string ImageName { get { return "ExpandFieldPivotTable"; } }
		#endregion
		protected override void ModifyPivotItemHideDetails(PivotTable table, int fieldIndex, PivotTableColumnRowFieldIndices fields) {
			int index = fields.GetIndexElementByFieldIndex(fieldIndex);
			for (; index < table.RowFields.Count; index++) {
				int currentFieldIndex = fields[index].FieldIndex;
				if (currentFieldIndex < 0)
					continue;
				PivotField field = table.Fields[currentFieldIndex];
				if (field.Items.AllDataItemsAreExpanded)
					continue;
				else {
					PivotItemCollection pivotItems = table.Fields[currentFieldIndex].Items;
					foreach (PivotItem item in pivotItems)
						item.HideDetails = false;
					return;
				}
			}
		}
	}
	#endregion
	#region PivotTableCollapseFieldCommand
	public class PivotTableCollapseFieldCommand : PivotTableExpandCollapseFieldCommandBase {
		public PivotTableCollapseFieldCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableCollapseField; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableCollapseField; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableCollapseFieldDescription; } }
		public override string ImageName { get { return "CollapseFieldPivotTable"; } }
		#endregion
		protected override void ModifyPivotItemHideDetails(PivotTable table, int fieldIndex, PivotTableColumnRowFieldIndices fields) {
			int index = fields.GetIndexElementByFieldIndex(fieldIndex);
			if (index == fields.Count - 1)
				index--;
			for (; index >= 0; index--) {
				int currentFieldIndex = fields[index].FieldIndex;
				if (currentFieldIndex < 0)
					continue;
				PivotField field = table.Fields[currentFieldIndex];
				if (field.Items.AllDataItemsAreCollapsed)
					continue;
				else {
					PivotItemCollection pivotItems = table.Fields[currentFieldIndex].Items;
					foreach (PivotItem item in pivotItems)
						item.HideDetails = true;
					return;
				}
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableExpandCollapseFieldCommandGroup
	public class PivotTableExpandCollapseFieldCommandGroup : SpreadsheetCommandGroup {
		public PivotTableExpandCollapseFieldCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableExpandCollapseFieldCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableExpandCollapseCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableExpandCollapseCommandGroupDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !InnerControl.IsAnyInplaceEditorActive && !ActiveSheet.ReadOnly);
			ApplyActiveSheetProtection(state, !Protection.PivotTablesLocked);
			state.Visible = GetVisible();
		}
		bool GetVisible() {
			SheetViewSelection selection = ActiveSheet.Selection;
			PivotTable table = ActiveSheet.TryGetPivotTable(selection.ActiveCell);
			if (table == null)
				return false;
			return PivotTableExpandCollapseFieldCommandBase.GetState(table);
		}
	}
	#endregion
	#region PivotTableExpandFieldContextMenuItemCommand
	public class PivotTableExpandFieldContextMenuItemCommand : PivotTableExpandFieldCommand {
		public PivotTableExpandFieldContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableExpandFieldContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableExpandFieldContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
	}
	#endregion
	#region PivotTableCollapseFieldContextMenuItemCommand
	public class PivotTableCollapseFieldContextMenuItemCommand : PivotTableCollapseFieldCommand {
		public PivotTableCollapseFieldContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableCollapseFieldContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableCollapseFieldContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
	}
	#endregion
}
