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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Model {
	partial class Worksheet {
		public virtual ArrayFormula CreateArray(string formula, CellRange range) {
			Guard.ArgumentIsNotNullOrEmpty(formula, "formula");
			Guard.ArgumentNotNull(range, "range");
			CheckingChangingPartOfArray((ISheetPosition)range);
			CheckingChangingMergedCell(range, XtraSpreadsheetStringId.Msg_ErrorAttemptToCreateArrayFormulaInMergedCells);
			if (range.CellCount > 1 && Tables.ContainsItemsInRange(range, true))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorMuiltiCellArrayFormulaInTable);
			if (PivotTables.ContainsItemsInRange(range, true))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_PivotTableCanNotBeChanged);
			InsertWorksheetArrayFormulaCommand command = new InsertWorksheetArrayFormulaCommand(this, range, formula);
			command.Execute();
			return (command.Result as ArrayFormula);
		}
		protected internal void CreateArrayCore(string formula, CellRange range) {
			ArrayFormula arrayFormula = new ArrayFormula(range);
			arrayFormula.SetBodyTemporarily(formula, (ICell)range.GetCellRelative(0, 0));
			ICell topLeftCell = this[arrayFormula.Range.TopLeft];
			FormulaBase newFormula;
			foreach (ICellBase cellbase in arrayFormula.Range.GetAllCellsEnumerable()) {
				ICell cell = (ICell)cellbase;
				Debug.Assert(cell != null);
				if (Object.ReferenceEquals(cell, topLeftCell))
					newFormula = arrayFormula;
				else
					newFormula = new ArrayFormulaPart(cell, topLeftCell);
				cell.ApplyFormulaCore(newFormula);
			}
			ArrayFormulaRanges.Add(range);
		}
		public void RemoveArrayFormulaAt(int index) {
			DeleteWorksheetArrayFormulaCommand command = new DeleteWorksheetArrayFormulaCommand(this, index);
			command.Execute();
		}
		public void ClearArrayFormulas() {
			DeleteAllWorksheetArrayFormulaCommand command = new DeleteAllWorksheetArrayFormulaCommand(this);
			command.Execute();
		}
		public void RemoveArrayFormulasAndValuesFrom(CellRange rangeIncludeArrayRanges) {
			RemoveArrayFormulaAndValuesCommand command = new RemoveArrayFormulaAndValuesCommand(this, rangeIncludeArrayRanges);
			command.Execute();
		}
		public void RemoveArrayFormulasFromCollectionInsideRange(CellRangeBase rangeBase) {
			foreach (CellRange range in rangeBase.GetAreasEnumerable()) {
				int existingFormulaIndex = ArrayFormulaRanges.FindArrayFormulaIndexInsideRange(range, ArrayFormulaRanges.Count - 1);
				while (existingFormulaIndex >= 0) {
					RemoveArrayFormulaAt(existingFormulaIndex);
					existingFormulaIndex = ArrayFormulaRanges.FindArrayFormulaIndexInsideRange(range, existingFormulaIndex - 1);
				}
			}
		}
		public void RemoveArrayFormulasFromCollectionInsideRange(ISheetPosition sheetPosition) {
			int i = ArrayFormulaRanges.Count - 1;
			while (i >= 0) {
				CellRange item = ArrayFormulaRanges[i];
				if (sheetPosition.Includes(item))
					RemoveArrayFormulaAt(i);
				i--;
			}
		}
		protected internal void CheckingChangingPartOfArray(CellRangeBase range) {
			if (ArrayFormulaRanges.CheckChangePartOfArray(range))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorChangingPartOfAnArray);
		}
		protected internal void CheckingChangingPartOfArray(ISheetPosition sheetPosition) {
			if (ArrayFormulaRanges.CheckChangePartOfArray(sheetPosition))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorChangingPartOfAnArray);
		}
	}
}
