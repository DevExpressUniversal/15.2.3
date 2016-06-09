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
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class RemoveArrayFormulaAndValuesCommand : SpreadsheetModelCommand {
		CellRange rangeContainsAtLeastOneArrayFormulaRange;
		public RemoveArrayFormulaAndValuesCommand(Worksheet worksheet, CellRange rangeContainsAtLeastOneArrayFormulaRange)
			: base(worksheet) {
				this.rangeContainsAtLeastOneArrayFormulaRange = rangeContainsAtLeastOneArrayFormulaRange;
		}
		ArrayFormulaRangesCollection ArrayFormulaRanges { get { return Worksheet.ArrayFormulaRanges; } }
		protected internal override void ExecuteCore() {
			int existingFormulaIndex = ArrayFormulaRanges.FindArrayFormulaIndexInsideRange(rangeContainsAtLeastOneArrayFormulaRange, ArrayFormulaRanges.Count - 1);
			if (existingFormulaIndex < 0)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorAttemptToRemoveArrayFormula);
			do {
				CellRange exactArrayFormulaRange = ArrayFormulaRanges[existingFormulaIndex];
				Worksheet.RemoveArrayFormulaAt(existingFormulaIndex);
				ICell hostCell = null;
				foreach (ICellBase cellbase in exactArrayFormulaRange.GetAllCellsEnumerable()) {
					ICell cell = (ICell)cellbase;
					Debug.Assert(cell != null);
					if (cell.FormulaType == FormulaType.Array) {
						hostCell = cell;
						continue;
					}
					cell.ClearContent();
				}
				if (hostCell != null)
					hostCell.ClearContent();
				existingFormulaIndex = ArrayFormulaRanges.FindArrayFormulaIndexInsideRange(rangeContainsAtLeastOneArrayFormulaRange, existingFormulaIndex - 1);
			}
			while (existingFormulaIndex >= 0);
		}
	}
	public class InsertWorksheetArrayFormulaCommand : SpreadsheetModelCommand {
		readonly string formula;
		readonly CellRange range;
		public InsertWorksheetArrayFormulaCommand(Worksheet worksheet, CellRange range, string formula)
			: base(worksheet) {
			Guard.ArgumentIsNotNullOrEmpty(formula, "formula");
			Guard.ArgumentNotNull(range, "range");
			this.formula = formula;
			this.range = range;
		}
		protected internal override void ExecuteCore() {
			Worksheet.RemoveArrayFormulasFromCollectionInsideRange((ISheetPosition)range);
			ArrayFormula arrayFormula = new ArrayFormula(range, formula);
			ExecuteWithoutChecks(arrayFormula, range);
			Result = arrayFormula;
		}
		public static void ExecuteWithoutChecks(ArrayFormula arrayFormula, CellRange range) {
			Worksheet worksheet = range.Worksheet as Worksheet;
			DocumentModel workbook = worksheet.Workbook;
			LocateArrayFormulaToCells(worksheet, arrayFormula);
			worksheet.ArrayFormulaRanges.Add(range);
			workbook.InternalAPI.OnArrayFormulaAdd(worksheet, arrayFormula);
		}
		static void LocateArrayFormulaToCells(Worksheet sheet, ArrayFormula arrayFormula) {
			ICell topLeftCell = sheet[arrayFormula.Range.TopLeft];
			FormulaBase newFormula;
			foreach (ICellBase cellbase in arrayFormula.Range.GetAllCellsEnumerable()) {
				ICell cell = (ICell)cellbase;
				Debug.Assert(cell != null);
				if (Object.ReferenceEquals(cell, topLeftCell))
					newFormula = arrayFormula;
				else
					newFormula = new ArrayFormulaPart(cell, topLeftCell);
				cell.SetFormulaCore(newFormula);
				sheet.OnCellValueChanged(cell);
			}
		}
	}
	public class DeleteWorksheetArrayFormulaCommand : SpreadsheetModelCommand {
		int index;
		public DeleteWorksheetArrayFormulaCommand(Worksheet worksheet, int index)
			: base(worksheet) {
			Guard.ArgumentNonNegative(index, "index");
			this.index = index;
		}
		protected internal override void ExecuteCore() {
			DocumentModel.InternalAPI.OnArrayFormulaRemoveAt(Worksheet, index);
			Worksheet.ArrayFormulaRanges.RemoveAt(index);
		}
	}
	public class DeleteAllWorksheetArrayFormulaCommand : SpreadsheetModelCommand {
		public DeleteAllWorksheetArrayFormulaCommand(Worksheet worksheet)
			: base(worksheet) {
		}
		protected internal override void ExecuteCore() {
			DocumentModel.InternalAPI.OnArrayFormulaCollectionClear(Worksheet);
			Worksheet.ArrayFormulaRanges.Clear();
		}
	}
}
