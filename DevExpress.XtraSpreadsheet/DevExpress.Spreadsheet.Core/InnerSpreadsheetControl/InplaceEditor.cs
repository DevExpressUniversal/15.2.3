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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet {
	public enum CellEditorEnterValueMode {
		ActiveCell,
		Default = ActiveCell,
		SelectedCells,
		ArrayFormula,
		Cancel,
	}
}
namespace DevExpress.XtraSpreadsheet.Internal {
	public partial class InnerSpreadsheetControl {
		InnerCellInplaceEditor inplaceEditor;
		public InnerCellInplaceEditor InplaceEditor { get { return inplaceEditor; } }
		public bool IsInplaceEditorActive { get { return InplaceEditor != null ? InplaceEditor.IsActive : false; } }
		public bool IsAnyInplaceEditorActive { get { return IsInplaceEditorActive || IsCommentInplaceEditorActive; } }
		public bool CloseInplaceEditor(CellEditorEnterValueMode mode) {
			if (IsDisposed)
				return false;
			if (!IsInplaceEditorActive)
				return false;
			InplaceEditorSimpleCommand command = CreateInplaceEditorCommand(mode);
			if (command == null)
				return false;
			command.Execute();
			return !IsInplaceEditorActive;
		}
		InplaceEditorSimpleCommand CreateInplaceEditorCommand(CellEditorEnterValueMode mode) {
			switch (mode) {
				default:
				case CellEditorEnterValueMode.ActiveCell:
					return new InplaceEndEditCommand(Owner);
				case CellEditorEnterValueMode.SelectedCells:
					return new InplaceEndEditEnterToMultipleCellsCommand(Owner);
				case CellEditorEnterValueMode.ArrayFormula:
					return new InplaceEndEditEnterArrayFormulaCommand(Owner);
				case CellEditorEnterValueMode.Cancel:
					return new InplaceCancelEditCommand(Owner);
			}
		}
		public virtual ICellInplaceEditor CreateCellInplaceEditor(bool formulaBarShouldFocused) {
			return Owner.CreateCellInplaceEditor(formulaBarShouldFocused);
		}
		public void ActivateCellInplaceEditor(CellPosition cellPosition, string newText, CellEditorMode mode) {
			ActivateCellInplaceEditor(cellPosition, newText, -1, mode);
		}
		public void ActivateCellInplaceEditor(CellPosition cellPosition, string newText, int caretPosition, CellEditorMode mode) {
			bool needShow = true;
			if (!IsInplaceEditorActive)
				needShow = RaiseCellBeginEdit(this.Model.DocumentModel.ActiveSheet, cellPosition.Column, cellPosition.Row);
			if (needShow)
				InplaceEditor.ActivateAt(cellPosition, newText, caretPosition, mode);
		}
		public void OpenInplaceEditor(CellEditorMode mode) {
			InplaceBeginEditCommand command = new InplaceBeginEditCommand(Owner);
			command.Mode = mode;
			command.Execute();
		}
		public void OpenInplaceEditor(string text) {
			InplaceBeginEditCommand command = new InplaceBeginEditCommand(Owner);
			command.Text = text;
			command.Mode = CellEditorMode.Enter;
			command.Execute();
		}
	}
}
