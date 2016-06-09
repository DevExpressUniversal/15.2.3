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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatClearContentsCommand
	public class FormatClearContentsCommand : ClearSelectedCellsCommand {
		public FormatClearContentsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatClearContents; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatClearContents; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatClearContentsDescription; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (!CheckProtection())
				return;
			DocumentModel.BeginUpdateFromUI();
			try {
				if (!CheckError()) {
					IList<CellRange> selectedRanges = ActiveSheet.Selection.SelectedRanges;
					foreach (CellRange range in selectedRanges)
						ModifyRange(range);
				}
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected internal override void ModifyRange(CellRange range) {
			ActiveSheet.ClearContents(range, ErrorHandler);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
		}
		bool CheckProtection() {
			if (!Protection.SheetLocked)
				return true;
			if (ActiveSheet.Selection.IsSingleCell)
				return Control.InnerControl.TryEditActiveCellContent();
			IList<CellRange> selectedRanges = ActiveSheet.Selection.SelectedRanges;
			foreach (CellRange range in selectedRanges) {
				IModelErrorInfo error = CanModify(range);
				if (error != null) {
					if (ErrorHandler.HandleError(error) == ErrorHandlingResult.Abort)
						return false;
				}
			}
			return true;
		}
		IModelErrorInfo CanModify(CellRange cellRange) {
			foreach (ICell cell in cellRange.GetExistingCellsEnumerable()) {
				if (!ActiveSheet.CanEditCellContent(cell.Position, true))
					return new ModelErrorInfo(ModelErrorType.CellOrChartIsReadonly);
			}
			return null;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region FormatClearContentsContextMenuItemCommand
	public class FormatClearContentsContextMenuItemCommand : FormatClearContentsCommand {
		public FormatClearContentsContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatClearContentsContextMenuItem; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
