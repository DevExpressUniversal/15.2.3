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
using System.ComponentModel;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IFormulaBarControl {
		ICellInplaceEditor InplaceEditor { get; }
		bool Expanded { get; set; }
	}
	public interface IFormulaBarControllerOwner : IFormulaBarControl {
		bool EditMode { get; }
		event KeyEventHandler KeyDown;
		event EventHandler Enter;
		event EventHandler CancelButtonClick;
		event EventHandler OkButtonClick;
		event EventHandler InsertFunctionButtonClick;
		event EventHandler Rollback;
		FormulaBarController Controller { get; }
	}
	public partial class FormulaBarController : IDisposable {
		ISpreadsheetControl spreadsheetControl;
		readonly IFormulaBarControllerOwner owner;
		string ownersText;
		CellPosition lastActiveCell;
		public FormulaBarController(IFormulaBarControllerOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.ownersText = String.Empty;
			InvalidateActiveCell();
		}
		#region Properties
		public ISpreadsheetControl SpreadsheetControl {
			get {
				return spreadsheetControl;
			}
			set {
				if (spreadsheetControl == value)
					return;
				UnsubscribeControlEvents();
				ResetFields();
				spreadsheetControl = value;
				if (value != null) {
					SubscribeControlEvents();
					SelectionChangedCore();
				}
			}
		}
		protected internal DocumentModel Workbook { get { return SpreadsheetControl != null && SpreadsheetControl.InnerControl != null ? SpreadsheetControl.InnerControl.DocumentModel : null; } }
		public string OwnersText { get { return ownersText; } }
		protected CellPosition ActiveCell { get { return Workbook.ActiveSheet.Selection.ActiveCell; } }
		public EditFunctionArgumentsCommand EditFunctionArgumentsCommand { get; set; }
		#endregion
		void InvalidateActiveCell() {
			lastActiveCell = CellPosition.InvalidValue;
		}
		void SubscribeControlEvents() {
			if (Workbook != null) {
				Workbook.InnerActiveSheetChanged += OnActiveSheetChanged;
				Workbook.InnerSelectionChanged += OnDocumentSelectionChanged;
				Workbook.ContentSetted += OnContentSetted;
				Workbook.InternalAPI.CellValueChanged += OnCellValueChanged;
			}
			owner.KeyDown += OnOwnerKeyDown;
			owner.Enter += OnOwnerEnter;
			owner.CancelButtonClick += OnOwnerCancelClick;
			owner.OkButtonClick += OnOwnerOkClick;
			owner.InsertFunctionButtonClick += OnOwnerInsertFunctionClick;
			owner.Rollback += OnOwnerRollback;
			if (SpreadsheetControl != null)
				this.SpreadsheetControl.ContentChanged += OnContentChanged;
		}
		void UnsubscribeControlEvents() {
			if (Workbook != null) {
				Workbook.InnerActiveSheetChanged -= OnActiveSheetChanged;
				Workbook.InnerSelectionChanged -= OnDocumentSelectionChanged;
				Workbook.ContentSetted -= OnContentSetted;
				Workbook.InternalAPI.CellValueChanged -= OnCellValueChanged;
			}
			owner.KeyDown -= OnOwnerKeyDown;
			owner.Enter -= OnOwnerEnter;
			owner.CancelButtonClick -= OnOwnerCancelClick;
			owner.OkButtonClick -= OnOwnerOkClick;
			owner.InsertFunctionButtonClick -= OnOwnerInsertFunctionClick;
			owner.Rollback -= OnOwnerRollback;
			if (SpreadsheetControl != null)
				this.SpreadsheetControl.ContentChanged -= OnContentChanged;
		}
		void OnOwnerRollback(object sender, EventArgs e) {
			SelectionChangedCore();
		}
		void ResetFields() {
			ownersText = String.Empty;
			InvalidateActiveCell();
			spreadsheetControl = null;
		}
		void OnCellValueChanged(object sender, SpreadsheetCellEventArgs e) {
			if (ActiveCellValueChanged(e))
				SelectionChangedCore();
		}
		bool ActiveCellValueChanged(SpreadsheetCellEventArgs e) {
			CellPosition senderCell = new CellPosition(e.ColumnIndex, e.RowIndex);
			return ActiveCell.EqualsPosition(senderCell);
		}
		void OnOwnerInsertFunctionClick(object sender, EventArgs e) {
			InsertFunctionCommand command = new InsertFunctionCommand(SpreadsheetControl);
			command.Execute();
		}
		void OnOwnerOkClick(object sender, EventArgs e) {
			InplaceEndEditCommand command = new InplaceEndEditCommand(SpreadsheetControl);
			command.Execute();
		}
		void OnOwnerCancelClick(object sender, EventArgs e) {
			InplaceCancelEditCommand command = new InplaceCancelEditCommand(SpreadsheetControl);
			command.Execute();
		}
		void OnOwnerEnter(object sender, EventArgs e) {
			if (owner.EditMode)
				return;
			owner.Enter -= OnOwnerEnter;
			try {
				InplaceBeginEditCommand command = new InplaceBeginEditCommand(SpreadsheetControl);
				command.Mode = CellEditorMode.EditInFormulaBar;
				command.Execute();
			}
			finally {
				owner.Enter += OnOwnerEnter;
			}
		}
		void OnOwnerKeyDown(object sender, KeyEventArgs e) {
			owner.KeyDown -= OnOwnerKeyDown;
			try {
				SpreadsheetControl.InnerControl.InplaceEditor.OnKeyDown(e);
#if !SL
				e.SuppressKeyPress = (e.KeyCode == Keys.Return);
#endif
			}
			finally {
				owner.KeyDown += OnOwnerKeyDown;
			}
		}
		void OnDocumentSelectionChanged(object sender, EventArgs e) {
			if (lastActiveCell.EqualsPosition(ActiveCell))
				return;
			SelectionChangedCore();
			lastActiveCell = ActiveCell;
		}
		void SelectionChangedCore() {
			if (Workbook.ActiveSheet.Selection.IsDrawingSelected)
				ownersText = String.Empty;
			else {
				ICell activeCell = ObtainCell();
				ownersText = CellInplaceEditorHelper.CalculateEditorText(null, activeCell);
				if (activeCell.HasFormula) {
					FormulaBase formula = activeCell.GetFormula();
					if (formula is ArrayFormula || formula is ArrayFormulaPart)
						ownersText = "{" + ownersText + "}";
				}
			}
			RaiseSelectionChanged();
		}
		void OnActiveSheetChanged(object sender, ActiveSheetChangedEventArgs e) {
			InvalidateActiveCell();
			SelectionChangedCore();
		}
		void OnContentSetted(object sender, DocumentContentSettedEventArgs e) {
			DocumentModelChangeType changeType = e.ChangeType;
			if (changeType == DocumentModelChangeType.CreateEmptyDocument || changeType == DocumentModelChangeType.LoadNewDocument) {
				InvalidateActiveCell();
				SelectionChangedCore();
			}
		}
		void OnContentChanged(object sender, EventArgs e) {
			if (!SpreadsheetControl.InnerControl.IsInplaceEditorActive) 
				SelectionChangedCore();
		}
		ICell ObtainCell() {
			Worksheet sheet = Workbook.ActiveSheet;
			CellPosition position = sheet.MergedCells.CorrectCellPositionToTopLeftInsideMergedRange(ActiveCell);
			return sheet.GetCellForFormatting(position);
		}
		public void OnOwnerSelectionChanged(object s, EventArgs eventArgs) {
			if (EditFunctionArgumentsCommand != null) {
				EditFunctionArgumentsCommand.UpdateViewModel();
			}
		}
		public void Dispose() {
			UnsubscribeControlEvents();
		}
	}
}
