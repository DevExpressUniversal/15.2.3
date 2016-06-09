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

using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Keyboard;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region InnerDataValidationInplaceEditor
	public class InnerDataValidationInplaceEditor {
		#region Fields
		readonly InnerSpreadsheetControl innerControl;
		IDataValidationInplaceEditor editor;
		DataValidationKeyboardHandler keyboardHandler;
		bool isActive;
		bool isTextValue;
		#endregion
		public InnerDataValidationInplaceEditor(InnerSpreadsheetControl innerControl) {
			Guard.ArgumentNotNull(innerControl, "innerControl");
			this.innerControl = innerControl;
			InitializeKeyboardHandler();
		}
		#region Properties
		public InnerSpreadsheetControl InnerControl { get { return innerControl; } }
		public bool IsActive { get { return isActive; } }
		#endregion
		#region KeyboardHandler
		void InitializeKeyboardHandler() {
			this.keyboardHandler = new DataValidationKeyboardHandler();
			AppendKeyboardShortcuts(keyboardHandler, new SpreadsheetKeyHashProvider());
		}
		void AppendKeyboardShortcuts(DataValidationKeyboardHandler keyboardHandler, IKeyHashProvider provider) {
			keyboardHandler.RegisterKeyHandler(provider, Keys.Escape, Keys.None, SpreadsheetCommandId.DataValidationCloseEditor);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Space, Keys.None, SpreadsheetCommandId.DataValidationCloseEditor);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Space, Keys.Control, SpreadsheetCommandId.DataValidationCloseEditor);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Space, Keys.Shift, SpreadsheetCommandId.DataValidationCloseEditor);
			keyboardHandler.RegisterKeyHandler(provider, Keys.Enter, Keys.None, SpreadsheetCommandId.DataValidationInplaceEndEdit);
		}
		#endregion
		IDataValidationInplaceEditor CreateEditor() {
			return InnerControl.CreateDataValidationInplaceEditor();
		}
		public void Activate(Rectangle bounds, DataValidationInplaceValueStorage allowedValuesStorage) {
			InnerControl.SetNewKeyboardHandler(keyboardHandler);
			this.editor = CreateEditor();
			this.editor.SetBounds(bounds);
			SetAllowedValuesToEditor(allowedValuesStorage);
			this.editor.Activate();
			this.editor.SetFocus();
			this.isActive = true;
		}
		void SetAllowedValuesToEditor(DataValidationInplaceValueStorage allowedValuesStorage) {
			this.isTextValue = allowedValuesStorage.IsTextValue;
			if (isTextValue)
				this.editor.SetAllowedValues(allowedValuesStorage.TextAllowedValues, allowedValuesStorage.TextActiveValue);
			else
				this.editor.SetAllowedValues(allowedValuesStorage.DataValidationInplaceAllowedValues, allowedValuesStorage.DataValidationInplaceActiveValue);
		}
		public void Deactivate() {
			InnerControl.RestoreKeyboardHandler();
			this.editor.Deactivate();
			this.isActive = false;
		}
		public void Commit() {
			if (isTextValue) {
				string value = (string)editor.Value;
				Debug.Assert(!String.IsNullOrEmpty(value));
				ICell cell = GetCellOrCreate();
				CellContentSnapshotWithFormat snapshot = new CellContentSnapshotWithFormat(cell);
				cell.SetTextSmart(value);
				RaiseCellValueChanged(snapshot, cell);
			}
			else {
				if (editor.Value == null)
					return;
				VariantValue value = ((DataValidationInplaceValue)editor.Value).Value;
				ICell cell = ObtainCell(value);
				CellContentSnapshotWithFormat snapshot = new CellContentSnapshotWithFormat(cell);
				cell.Value = value;
				RaiseCellValueChanged(snapshot, cell);
			}
		}
		ICell GetCellOrCreate() {
			Worksheet sheet = innerControl.DocumentModel.ActiveSheet;
			CellPosition activeCell = GetActiveCellPosition(sheet);
			return sheet.GetCellOrCreate(activeCell.Column, activeCell.Row);
		}
		ICell ObtainCell(VariantValue value) {
			Worksheet sheet = innerControl.DocumentModel.ActiveSheet;
			CellPosition activeCell = GetActiveCellPosition(sheet);
			if (value.IsEmpty)
				return sheet.GetCellForFormatting(activeCell.Column, activeCell.Row);
			return sheet.GetCellOrCreate(activeCell.Column, activeCell.Row);
		}
		static CellPosition GetActiveCellPosition(Worksheet sheet) {
			CellPosition activeCell = sheet.Selection.ActiveCell;
			return sheet.Selection.GetActualCellRange(activeCell).TopLeft;
		}
		void RaiseCellValueChanged(CellContentSnapshotWithFormat snapshot, ICell cell) {
			if (snapshot.Value.IsEqual(cell.Value, StringComparison.Ordinal, cell.Context.StringTable) && snapshot.Format.Equals(cell.ActualFormat))
				return;
			SpreadsheetCellEventArgs args = new SpreadsheetCellEventArgs(snapshot);
			innerControl.RaiseCellValueChanged(args);
		}
	}
	#endregion
}
