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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Internal {
	#region InplaceEditorCommitStrategy (abstract class)
	public abstract class InplaceEditorCommitStrategy {
		#region Fields
		readonly Worksheet sheet;
		readonly CellPosition cellPosition;
		readonly CellEditorCommitMode commitMode;
		readonly List<CellContentSnapshot> affectedCells = new List<CellContentSnapshot>();
		readonly ICellInplaceEditorCommitControllerOwner owner;
		#endregion
		protected InplaceEditorCommitStrategy(ICellInplaceEditorCommitControllerOwner owner, Worksheet sheet, CellPosition position) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.owner = owner;
			this.sheet = sheet;
			this.cellPosition = position;
			this.commitMode = sheet.Workbook.BehaviorOptions.CellEditor.CommitMode;
		}
		#region Properties
		protected Worksheet Sheet { get { return sheet; } }
		protected DocumentModel DocumentModel { get { return sheet.Workbook; } }
		protected WorkbookDataContext Context { get { return DocumentModel.DataContext; } }
		protected CellPosition CellPosition { get { return cellPosition; } }
		public IList<CellContentSnapshot> AffectedCells { get { return affectedCells; } }
		public bool CircularReferenceFound { get; set; }
		public CellEditorCommitMode CommitMode { get { return commitMode; } }
		public bool AttemptToChangeLockedCells { get; set; }
		protected virtual bool IsFormula { get { return false; } }
		#endregion
		#region Commit
		public bool Commit(FormattedInplaceEditorValue value) {
			return Commit(value.Text);
		}
		public bool Commit(string text) {
			DocumentModel.BeginUpdate();
			try {
				DialogResult checkResult = CheckDataValidation(text);
				if (checkResult != DialogResult.OK && checkResult != DialogResult.Yes)
					return checkResult == DialogResult.Cancel;
				if (IgnoreEmptyValue(text))
					return true;
				return CommitCore(text);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region CheckDataValidation
		DialogResult CheckDataValidation(string text) {
			DataValidation dataValidation = FindDataValidation();
			if (dataValidation == null || !dataValidation.ShowErrorMessage)
				return DialogResult.OK;
			if (!Validate(dataValidation, text, IsFormula))
				return ShowDataValidationDialog(text, dataValidation);
			else {
				RemoveInvalidDataCircle();
				return DialogResult.OK;
			}
		}
		DialogResult ShowDataValidationDialog(string text, DataValidation dataValidation) {
			string errorMessage = dataValidation.Error;
			if (String.IsNullOrEmpty(errorMessage))
				errorMessage = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_DataValidationFailed);
			string errorTitle = dataValidation.ErrorTitle;
			if (String.IsNullOrEmpty(errorTitle)) {
#if DXPORTABLE
				errorTitle = "Spreadsheet";
#else
				errorTitle = System.Windows.Forms.Application.ProductName;
#endif
			}
			return owner.ShowDataValidationDialog(text, errorMessage, errorTitle, dataValidation.ErrorStyle);
		}
		void RemoveInvalidDataCircle() {
			CellKey cellKey = new CellKey(Sheet.SheetId, CellPosition.Column, CellPosition.Row);
			if (Sheet.ShowInvalidDataCircles) {
				Sheet.InvalidDataCircles.Remove(cellKey);
				DocumentModel.ApplyChanges(DocumentModelChangeActions.Redraw);
			}
		}
		DataValidation FindDataValidation() {
			CellRange activeCellRange = new CellRange(Sheet, cellPosition, cellPosition);
			foreach (DataValidation dataValidation in Sheet.DataValidations)
				if (dataValidation.CellRange.Intersects(activeCellRange))
					return dataValidation;
			return null;
		}
		bool Validate(DataValidation dataValidation, string text, bool isFormula) {
			ICell cell = Sheet[cellPosition];
			VariantValue value;
			if (isFormula) {
				ParsedExpression expression = GetParsedExpression(text);
				if (expression == null)
					return false;
				value = expression.Evaluate(Context);
			}
			else
				value = GetVariantValue(cell, text);
			VariantValue oldValue = cell.Value;
			cell.AssignValueCore(value);
			bool result = dataValidation.ValidateValue(value, cellPosition);
			cell.AssignValueCore(oldValue);
			return result;
		}
		ParsedExpression GetParsedExpression(string text) {
			ParsedExpression expression;
			Context.PushCurrentCell(cellPosition);
			try {
				expression = Context.ParseExpression(text, OperandDataType.Value, true);
			}
			finally {
				Context.PopCurrentCell();
			}
			return expression;
		}
		VariantValue GetVariantValue(ICell cell, string text) {
			if (!String.IsNullOrEmpty(text)) {
				if (cell == null)
					return Context.ConvertTextToVariantValueWithCaching(text);
				NumberFormat actualNumberFormat = cell.ActualFormat;
				if (actualNumberFormat.IsText)
					return text;
				FormattedVariantValue formattedValue = CellValueFormatter.GetValue(cell.GetValue(), text, Context, true, actualNumberFormat);
				return formattedValue.Value;
			}
			return VariantValue.Empty;
		}
		#endregion
		protected virtual bool IgnoreEmptyValue(string text) {
			return false;
		}
		protected virtual bool ShouldCommitCellValue(string text, ICell cell) {
			if (CommitMode == CellEditorCommitMode.ChangedOnly) {
				VariantValue newValue = GetVariantValue(cell, text);
				return !cell.Value.IsEqual(newValue, StringComparison.Ordinal, cell.Context.StringTable);
			}
			return true;
		}
		protected bool ShouldCommitCellFormula(string text, ICell cell) {
			if (CommitMode == CellEditorCommitMode.ChangedOnly)
				return StringExtensions.CompareInvariantCultureIgnoreCase(text, cell.FormulaBody) != 0;
			else
				return true;
		}
		protected abstract bool CommitCore(string text);
	}
	#endregion
	#region SingleCellInplaceEditorCommitStrategy (abstract class)
	public abstract class SingleCellInplaceEditorCommitStrategy : InplaceEditorCommitStrategy {
		protected SingleCellInplaceEditorCommitStrategy(ICellInplaceEditorCommitControllerOwner owner, Worksheet sheet, CellPosition position)
			: base(owner, sheet, position) {
		}
		protected override bool IgnoreEmptyValue(string text) {
			if (CommitMode != CellEditorCommitMode.Always) {
				if (String.IsNullOrEmpty(text) && Sheet.TryGetCell(CellPosition.Column, CellPosition.Row) == null)
					return true;
			}
			return false;
		}
		protected override bool CommitCore(string text) {
			ICell activeCell = Sheet[CellPosition];
			AffectedCells.Add(new CellContentSnapshot(activeCell));
			AssignCellValue(activeCell, text);
			if (Sheet.Workbook.Properties.CalculationOptions.CalculationMode != ModelCalculationMode.Manual)
				DocumentModel.CalculationChain.CalculateWorkbookIfHasMarkedCells();
			CircularReferenceFound = activeCell.HasCircularReferences() || DocumentModel.HasCircularReferences;
			if (!CircularReferenceFound && DocumentModel.BehaviorOptions.Column.ResizeAllowed && DocumentModel.BehaviorOptions.Column.AutoFitAllowed)
				Sheet.TryBestFitColumn(activeCell, ColumnBestFitMode.InplaceEditorMode);
			return true;
		}
		protected abstract void AssignCellValue(ICell cell, string text);
	}
	#endregion
	#region SingleCellValueInplaceEditorCommitStrategy
	public class SingleCellValueInplaceEditorCommitStrategy : SingleCellInplaceEditorCommitStrategy {
		public SingleCellValueInplaceEditorCommitStrategy(ICellInplaceEditorCommitControllerOwner owner, Worksheet sheet, CellPosition position)
			: base(owner, sheet, position) {
		}
		protected override bool CommitCore(string text) {
			if (!ShouldCommitCellValue(text, Sheet[CellPosition]))
				return true; 
			return base.CommitCore(text);
		}
		protected override void AssignCellValue(ICell cell, string text) {
			if (String.IsNullOrEmpty(text))
				cell.Value = VariantValue.Empty;
			else
				cell.SetTextSmart(text);
		}
	}
	#endregion
	#region SingleCellFormulaInplaceEditorCommitStrategy
	public class SingleCellFormulaInplaceEditorCommitStrategy : SingleCellInplaceEditorCommitStrategy {
		public SingleCellFormulaInplaceEditorCommitStrategy(ICellInplaceEditorCommitControllerOwner owner, Worksheet sheet, CellPosition position)
			: base(owner, sheet, position) {
		}
		protected override bool IsFormula { get { return true; } }
		protected override bool CommitCore(string text) {
			if (!ShouldCommitCellFormula(text, Sheet[CellPosition]))
				return true; 
			return base.CommitCore(text);
		}
		protected override void AssignCellValue(ICell cell, string text) {
			cell.SetTextSmart(text);
			if (cell.HasFormula && cell.Worksheet.Workbook.Properties.CalculationOptions.CalculationMode == ModelCalculationMode.Manual) {
				DocumentModel.CalculationChain.CalculateCell(cell);
			}
		}
	}
	#endregion
	#region MultipleCellsValueInplaceEditorCommitStrategy
	public class MultipleCellsValueInplaceEditorCommitStrategy : InplaceEditorCommitStrategy {
		public MultipleCellsValueInplaceEditorCommitStrategy(ICellInplaceEditorCommitControllerOwner owner, Worksheet sheet, CellPosition position)
			: base(owner, sheet, position) {
		}
		protected override bool CommitCore(string text) {
			CellRangeBase selection = Sheet.Selection.AsRange();
			foreach (CellBase cellBase in selection.GetAllCellsEnumerable()) {
				ICell cell = (ICell)cellBase;
				if (!ShouldCommitCellValue(text, cell))
					continue;
				if (!Sheet.CanEditCellContent(cell.Position, false)) {
					AttemptToChangeLockedCells = true;
				}
				else {
					AffectedCells.Add(new CellContentSnapshot(cell));
					cell.SetTextSmart(text);
				}
			}
			return true;
		}
		protected override bool ShouldCommitCellValue(string text, ICell cell) {
			CellRange mergedRange = Sheet.MergedCells.GetMergedCellRange(cell);
			if (mergedRange != null && !Sheet.MergedCells.IsHostMergedCell(cell, mergedRange))
				return false;
			return base.ShouldCommitCellValue(text, cell);
		}
	}
	#endregion
	#region MultipleCellsFormulaInplaceEditorCommitStrategy
	public class MultipleCellsFormulaInplaceEditorCommitStrategy : InplaceEditorCommitStrategy {
		CellRangeBase affectedRange;
		public MultipleCellsFormulaInplaceEditorCommitStrategy(ICellInplaceEditorCommitControllerOwner owner, Worksheet sheet, CellPosition position)
			: base(owner, sheet, position) {
		}
		protected override bool IsFormula { get { return true; } }
		protected override bool CommitCore(string text) {
			CellRangeBase selection = Sheet.Selection.AsRange();
			List<CellRangeBase> lockedCells = new List<CellRangeBase>();
			foreach (CellKey cellkey in selection.GetAllCellKeysEnumerable()) {
				CellPosition position = cellkey.GetPosition();
				if (!Sheet.CanEditCellContent(position, false))
					lockedCells.Add(new CellRange(Sheet, position, position));
			}
			if (lockedCells.Count > 0) {
				CellUnion rangeToExclude = new CellUnion(Sheet, lockedCells);
				affectedRange = selection.ExcludeRange(rangeToExclude);
				AttemptToChangeLockedCells = true;
			}
			else
				affectedRange = selection;
			foreach (CellBase cellBase in affectedRange.GetAllCellsEnumerable()) {
				ICell cell = (ICell)cellBase;
				if (cell != null)
					AffectedCells.Add(new CellContentSnapshot(cell));
			}
			AssignFormula(text);
			CalculateAffectedCells();
			foreach (CellBase cellBase in affectedRange.GetExistingCellsEnumerable()) {
				ICell cell = (ICell)cellBase;
				if (cell != null)
					CircularReferenceFound |= cell.HasCircularReferences();
			}
			return true;
		}
		void CalculateAffectedCells() {
			if (DocumentModel.Properties.CalculationOptions.CalculationMode != ModelCalculationMode.Manual)
				return;
			CalculationChain calculationChain = DocumentModel.CalculationChain;
			foreach (CellBase cellBase in affectedRange.GetExistingCellsEnumerable()) {
				ICell cell = (ICell)cellBase;
				if (cell != null && cell.HasFormula) {
					calculationChain.CalculateCell(cell);
				}
			}
		}
		protected virtual void AssignFormula(string text) {
			Sheet.LocateFormulaToMultipleCells(affectedRange, CellPosition, text, true);
		}
	}
	#endregion
	#region MultipleCellsArrayFormulaInplaceEditorCommitStrategy
	public class MultipleCellsArrayFormulaInplaceEditorCommitStrategy : MultipleCellsFormulaInplaceEditorCommitStrategy {
		public MultipleCellsArrayFormulaInplaceEditorCommitStrategy(ICellInplaceEditorCommitControllerOwner owner, Worksheet sheet, CellPosition position)
			: base(owner, sheet, position) {
		}
		protected override bool IsFormula { get { return true; } }
		protected override void AssignFormula(string text) {
			CellRange range = Sheet.Selection.ActiveRange;
			CheckForLockedCells(range);
			Sheet.CreateArray(text, range);
		}
		void CheckForLockedCells(CellRange range) {
			CellPositionEnumerator allPositions = range.GetAllPositionsEnumerator();
			while (allPositions.MoveNext()) {
				if (!Sheet.CanEditCellContent(allPositions.Current, false))
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_CellOrChartIsReadonly);
			}
		}
	}
	#endregion
	public class FormattedInplaceEditorValue {
		public string Text { get; set; }
		public string Formula { get; set; }
		public bool IsFormula { get; set; }
		public VariantValue Value { get; set; }
		public bool UseValue { get; set; }
		public string NumberFormat { get; set; }
		public bool UseNumberFormat { get; set; }
	}
}
