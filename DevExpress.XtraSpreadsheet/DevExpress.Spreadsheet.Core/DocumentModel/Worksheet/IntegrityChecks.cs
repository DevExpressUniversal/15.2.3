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
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	partial class SheetBase {
		#region Check Integrity
		public virtual void CheckIntegrity(CheckIntegrityFlags flags) {
			if (Workbook == null)
				IntegrityChecks.Fail("Sheetbase: Workbook should not be null");
		}
		#endregion
	}
	partial class Worksheet {
		#region Check Integrity
		public override void CheckIntegrity(CheckIntegrityFlags flags) {
			try {
				base.CheckIntegrity(flags);
				if (Workbook == null)
					IntegrityChecks.Fail("Worksheet: Workbook should not be null");
				if (!Workbook.Sheets.Contains(this))
					IntegrityChecks.Fail("Worksheet: worksheet is not contained in workbook");
				int rowIndex = -1;
				foreach (Row row in rows) {
					if (row.Index <= rowIndex) {
						IntegrityChecks.Fail("Worksheet: inconsistend row indices");
					}
					CheckRowIntegrity(row, flags);
					rowIndex = row.Index;
				}
				MergedCells.CheckIntegrity(flags);
				SharedFormulas.CheckIntegrity(flags);
				for (int i = 0; i < ArrayFormulaRanges.Count; i++) {
					CheckArrayFormulaIntegrity(ArrayFormulaRanges.GetArrayFormulaByRangeIndex(i));
				}
				tables.CheckIntegrity(flags);
				int count = DrawingObjects.Count;
				for (int i = 0; i < count; i++)
					CheckPictureIntegrity(DrawingObjects[i] as Picture);
				dataValidations.CheckIntegrity(flags);
			}
			finally {
			}
		}
		void CheckPictureIntegrity(Picture picture) {
			if (picture == null)
				return;
			if (picture.Image == null)
				IntegrityChecks.Fail("Picture: image should not be null");
		}
		public void CheckRowIntegrity(Row row, CheckIntegrityFlags flags) {
			if (row.Sheet == null)
				IntegrityChecks.Fail("Worksheet, row: row.Sheet should not be null");
			if (row.Sheet != this)
				IntegrityChecks.Fail("Worksheet, row: inconsistent row, row.Sheet != this");
			if ((flags & CheckIntegrityFlags.SkipTimeConsumingChecks) == 0) {
				Row thisRow = Rows.GetRow(row.Index);
				if (!Object.ReferenceEquals(thisRow, row))
					IntegrityChecks.Fail("Worksheet, row: row not found in Rows collection");
			}
			row.CheckIntegrity(flags);
		}
		public void CheckArrayFormulaIntegrity(ArrayFormula arrayFormula) {
			arrayFormula.CheckIntegrity();
		}
		#endregion
		public bool IsColumnVisible(int columnIndex) {
			IColumnRange columnRange = Columns.TryGetColumnRange(columnIndex);
			return columnRange == null || columnRange.IsVisible;
		}
		public bool IsRowVisible(int rowIndex) {
			Row row = Rows.TryGetRow(rowIndex);
			return row == null || row.IsVisible;
		}
	}
	partial class MergedCellCollection {
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			if (InnerTree != null) {
				if (InnerTree.Count != innerList.Count)
					IntegrityChecks.Fail("MergedCellCollection: innerList.Count and tree.Count doesn't match");
				foreach (CellRange mergedCell in InnerTree.InnerTree) {
					if (!innerList.Contains(mergedCell)) {
						IntegrityChecks.Fail(String.Format("MergedCellCollection: mergedCell {0} is in the tree, but no in the innerList", mergedCell.ToString()));
					}
				}
			}
			foreach (CellRange mergedCell in this.GetEVERYMergedRangeSLOWEnumerable()) {
				if (mergedCell.Worksheet != this.sheet)
					IntegrityChecks.Fail("MergedCellCollection: worksheet doesn't match");
				if (mergedCell.CellCount <= 0)
					IntegrityChecks.Fail(String.Format("MergedCellCollection: empty or single merged cell found: {0}", mergedCell.ToString()));
			}
			for (int i = 0; i < Count; i++) {
				CellRange firstMergedCell = this[i];
				for (int j = i + 1; j < Count; j++) {
					CellRange secondMergedCell = this[j];
					if (!Object.ReferenceEquals(firstMergedCell, secondMergedCell) && firstMergedCell.Intersects(secondMergedCell))
						IntegrityChecks.Fail(String.Format("MergedCellCollection: intersected merged cells found: '{0}' and '{1}'", firstMergedCell.ToString(), secondMergedCell.ToString()));
				}
			}
		}
	}
	partial class ModelHyperlinkCollection {
		public void CheckIntegrity(CheckIntegrityFlags flags) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				if (this[i].Worksheet != this.sheet)
					IntegrityChecks.Fail("HyperlinkCollection: worksheet doesn't match");
				if (this[i].Range.CellCount <= 0)
					IntegrityChecks.Fail(String.Format("HyperlinkCollection: empty or single merged cell found: {0}", this[i].Range.ToString()));
			}
		}
	}
}
