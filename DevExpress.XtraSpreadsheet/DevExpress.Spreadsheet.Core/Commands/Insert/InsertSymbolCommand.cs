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
using System.ComponentModel;
using DevExpress.Office.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Internal;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Import;
using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Forms;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region InsertSymbolCommand
	public class InsertSymbolCommand : SpreadsheetCommand {
		public InsertSymbolCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.InsertSymbol; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_InsertSymbol; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_InsertSymbolDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "Symbol"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				SpreadsheetInsertSymbolViewModel viewModel = new SpreadsheetInsertSymbolViewModel(Control);
				CellPosition position = ActiveSheet.Selection.ActiveCell;
				ICell cell = ActiveSheet.Selection.Sheet.GetCellForFormatting(position);
				viewModel.FontName = cell.ActualFont.Name;
				Control.ShowInsertSymbolForm(viewModel);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		public void ApplyChanges(InsertSymbolViewModel viewModel) {
			string value = new String(viewModel.UnicodeChar, 1);
			CellPosition position = ActiveSheet.Selection.ActiveCell;
			InnerControl.ActivateCellInplaceEditor(position, value, value.Length, CellEditorMode.Edit);
			ICell cell = ActiveSheet.Selection.Sheet.GetCellForFormatting(position);
			if (String.Compare(cell.Font.Name, viewModel.FontName, StringComparison.CurrentCultureIgnoreCase) != 0) {
				ActiveSheet[position].Font.Name = viewModel.FontName;
				InnerControl.InplaceEditor.UpdateBoundsAndFont();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = !ActiveSheet.Selection.IsDrawingSelected && IsContentEditable && !InnerControl.IsCommentInplaceEditorActive;
			state.Visible = true;
			state.Checked = false;
			ApplyActiveSheetProtection(state);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	public class SpreadsheetInsertSymbolViewModel : InsertSymbolViewModel {
		readonly ISpreadsheetControl control;
		public SpreadsheetInsertSymbolViewModel(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public ISpreadsheetControl Control { get { return control; } }
		public override void ApplyChanges() {
			InsertSymbolCommand command = new InsertSymbolCommand(Control);
			command.ApplyChanges(this);
		}
	}
}
