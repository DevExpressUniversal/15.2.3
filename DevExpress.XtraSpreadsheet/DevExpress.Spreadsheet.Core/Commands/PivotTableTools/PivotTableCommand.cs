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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PivotTableCommand (abstract class)
	public abstract class PivotTableCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected PivotTableCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected PivotTable TryGetPivotTable() {
			return ActiveSheet.TryGetPivotTable(ActiveSheet.Selection.ActiveCell);
		}
		public static string CreateReferenceString(Worksheet sheet, CellRange range) {
			WorkbookDataContext context = sheet.Workbook.DataContext;
			range = DefineNameCommand.ConvertToAbsoluteRange(range);
			return CellRangeToString.GetReferenceCommon(range, context.UseR1C1ReferenceStyle, range.TopLeft, PositionType.Absolute, PositionType.Absolute, true);
		}
		protected bool HandleError(IModelErrorInfo error) {
			if (error != null)
				if (ErrorHandler.HandleError(error) == ErrorHandlingResult.Abort)
					return false;
			return true;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default);
			ApplyActiveSheetProtection(state, !Protection.PivotTablesLocked);
			PivotTable table = TryGetPivotTable();
			state.Visible = GetVisible(table);
			state.Enabled &= GetEnabled(table);
		}
		protected virtual bool GetVisible(PivotTable table) {
			return table != null ? true : false;
		}
		protected virtual bool GetEnabled(PivotTable table) {
			return GetEnabledCore(Control.InnerControl, table);
		}
		public static bool GetEnabledCore(InnerSpreadsheetControl innerControl, PivotTable table) {
			if (table == null || innerControl.IsAnyInplaceEditorActive)
				return false;
			SheetViewSelection selection = table.DocumentModel.ActiveSheet.Selection;
			foreach (CellRange range in selection.SelectedRanges) 
				if (!table.WholeRange.Includes(range))
					return false;
			return true;
		}
	}
	#endregion
	#region LayoutPivotTableCommandBase
	public abstract class LayoutPivotTableCommandBase : PivotTableCommandBase {
		protected LayoutPivotTableCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool GetEnabled(PivotTable table) {
			CellPosition activeCell = ActiveSheet.Selection.ActiveCell;
			if (table == null || InnerControl.IsAnyInplaceEditorActive)
				return false;
			return true;
		}
	}
	#endregion
}
