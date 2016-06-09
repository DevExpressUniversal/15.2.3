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

using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Spreadsheet;
using System;
using DevExpress.Office.Utils;
using System.Drawing;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections;
using DevExpress.Export.Xl;
#if SL
	using System.Windows.Media;
#else
#endif
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Compatibility.System.Drawing;
	using DevExpress.Export.Xl;
	public abstract class InsideBordersPropertyAccessor<T> : RangeFormatsValuePropertyAccessor<T> {
		protected InsideBordersPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected delegate bool ActionWithModifier(FormatBasePropertyModifierBase modifier, Model.IFormatBaseBatchUpdateable obj);
		protected abstract Model.FormatBasePropertyModifierBase CreateFirstModifier(T value);
		protected abstract Model.FormatBasePropertyModifierBase CreateInnerModifier1(T value);
		protected abstract Model.FormatBasePropertyModifierBase CreateInnerModifier2(T value);
		protected abstract Model.FormatBasePropertyModifierBase CreateLastModifier(T value);
		protected internal override T CalculateValueCore() {
			T result = default(T);
			ActionWithModifier getValueFromRangeModifiers = (modifier, cell) => {
				result = (modifier as Model.FormatBasePropertyModifier<T>).GetFormatPropertyValue(cell);
				return false; 
			};
			T value = default(T);
			CellRange firstRange = Range.GetFirstInnerCellRange();
			bool changingBorders = false;
			SetValueCoreForSingleRangeExtracted(firstRange, value, getValueFromRangeModifiers, changingBorders);
			return result;
		}
		protected internal override bool SetValueCore(T value) {
			if (Range.CellCount == 0)
				return false;
			Model.Worksheet modelWorksheet = Range.Worksheet as Model.Worksheet;
			using (DevExpress.Office.History.HistoryTransaction transaction = new DevExpress.Office.History.HistoryTransaction(modelWorksheet.Workbook.History)) {
				return SetValueCoreForRangeBase(Range, value);
			}
		}
		bool SetValueCoreForSingleRange(Model.CellRange range, T value) {
			ActionWithModifier modifyRange = (modifier, cell) => {
				modifier.ModifyFormat(cell);
				return true;
			};
			bool changingBorders = true;
			return SetValueCoreForSingleRangeExtracted(range, value, modifyRange, changingBorders);
		}
		protected bool SetValueToRowColumnInterval(T value, Model.CellIntervalRange intervalRange, ActionWithModifier actionWithModifier, bool settingProperty) {
			if (intervalRange.IsColumnInterval)
				return SetValueCoreToColumnInterval(value, intervalRange, actionWithModifier, settingProperty);
			else
				return SetValueCoreToRowInterval(value, intervalRange, actionWithModifier, settingProperty);
		}
		protected abstract bool SetValueCoreToRowInterval(T value, CellIntervalRange intervalRange, ActionWithModifier actionWithModifier, bool settingProperty);
		protected abstract bool SetValueCoreToColumnInterval(T value, CellIntervalRange intervalRange, ActionWithModifier actionWithModifier, bool settingProperty);
		protected abstract bool SetValueCoreForSingleRangeExtracted(Model.CellRange range, T value, ActionWithModifier actionWithModifier, bool settingProperty);
		bool SetValueCoreForRangeBase(Model.CellRangeBase rangeBase, T value) {
			if (rangeBase.RangeType == CellRangeType.UnionRange) {
				Model.CellUnion union = (Model.CellUnion)rangeBase;
				bool result = false;
				foreach (Model.CellRangeBase innerRange in union.InnerCellRanges) {
					result |= SetValueCoreForRangeBase(innerRange, value);
				}
				return result;
			}
			else
				return SetValueCoreForSingleRange((Model.CellRange)rangeBase, value);
		}
		public IEnumerable<ICellBase> GetCells(Model.CellRange range, bool modifyOnlyExistingCells, bool settingProperty) {
			IEnumerable<ICellBase> enumerable;
			if (modifyOnlyExistingCells)
				enumerable = range.GetExistingCellsEnumerable();
			else {
				if (!settingProperty)
					enumerable = new Enumerable<ICellBase>(range.GetCellsForReadingEnumerator());
				else
					enumerable = range.GetAllCellsEnumerable(); 
			}
			return enumerable;
		}
		public IEnumerable<Model.Column> GetColumns(int fromColumn, int toColumn, bool forWrite) {
			IEnumerable<Model.Column> columns;
			if (forWrite)
				columns = ModelWorksheet.Columns.GetColumnRangesEnsureExist(fromColumn, toColumn);
			else
				columns = ModelWorksheet.Columns.GetColumnsForReading(fromColumn, fromColumn); 
			return columns;
		}
		public IEnumerable<Model.Row> GetRows(int from, int to, bool forWrite) {
			IEnumerable<Model.Row> rows;
			if (forWrite)
				rows = new Enumerable<Model.Row>(ModelWorksheet.Rows.GetAllRowsEnumerator(from, to, false));
			else
				rows = ModelWorksheet.Rows.GetRowsForReading(from, from); 
			return rows;
		}
		protected internal override IEnumerable<ICellBase> GetRangeExistingCellForModifyEnumerator(Model.CellRange range) {
			throw new InvalidOperationException();
		}
		protected override IList<Model.Row> GetAffectedRows(int first, int last) {
			throw new InvalidOperationException();
		}
		protected override IList<Model.Column> GetAffectedColumns(int first, int last) {
			throw new InvalidOperationException();
		}
	}
	public abstract class HorizontalInsideBordersPropertyAccessor<T> : InsideBordersPropertyAccessor<T> {
		protected HorizontalInsideBordersPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected override bool SetValueCoreForSingleRangeExtracted(Model.CellRange range, T value, ActionWithModifier actionWithModifier, bool settingProperty) {
			bool modifyOnlyExistingCells = false;
			const bool useFirstCalculatedValueAsResult = true;
			Model.CellIntervalRange intervalRange = range as Model.CellIntervalRange;
			if (intervalRange != null) {
				if (SetValueToRowColumnInterval(value, intervalRange, actionWithModifier, settingProperty))
					return useFirstCalculatedValueAsResult;
				modifyOnlyExistingCells = true;
			}
			Model.CellRange firstRowInRange = null;
			Model.CellRange lastRowInRange = null;
			Model.CellRange insideRowsInRange = null;
			if (range.Height >= 3) {
				firstRowInRange = range.GetSubRowRange(0, 0);
				lastRowInRange = range.GetSubRowRange(range.Height - 1, range.Height - 1);
				insideRowsInRange = range.GetSubRowRange(1, range.Height - 2);
			}
			else if (range.Height == 2) {
				firstRowInRange = range.GetSubRowRange(0, 0);
				lastRowInRange = range.GetSubRowRange(1, 1);
			}
			else
				return false;
			if (firstRowInRange != null) {
				Model.FormatBasePropertyModifierBase firstRowModifier = CreateFirstModifier(value);
				IEnumerable<ICellBase> enumerable = GetCells(firstRowInRange, modifyOnlyExistingCells, settingProperty);
				foreach (Model.ICellBase cellInfo in enumerable) {
					Model.ICell cell = cellInfo as ICell;
					if (cell != null) {
						if (!actionWithModifier(firstRowModifier, cell))
							return useFirstCalculatedValueAsResult;
					}
				}
			}
			if (lastRowInRange != null) {
				Model.FormatBasePropertyModifierBase lastRowModifier = CreateLastModifier(value);
				IEnumerable<ICellBase> enumerable = GetCells(lastRowInRange, modifyOnlyExistingCells, settingProperty);
				foreach (Model.ICellBase cellInfo in enumerable) {
					Model.ICell cell = cellInfo as ICell;
					if (cell != null)
						if (!actionWithModifier(lastRowModifier, cell))
							return useFirstCalculatedValueAsResult;
				}
			}
			if (insideRowsInRange != null) {
				Model.FormatBasePropertyModifierBase innerRowsModifier1 = CreateInnerModifier1(value);
				Model.FormatBasePropertyModifierBase innerRowsModifier2 = CreateInnerModifier2(value);
				IEnumerable<ICellBase> enumerable = GetCells(insideRowsInRange, modifyOnlyExistingCells, settingProperty);
				foreach (Model.ICellBase cellInfo in enumerable) {
					Model.ICell cell = cellInfo as Model.ICell;
					if (cell == null)
						continue;
					try {
						if (settingProperty)
							cell.BeginUpdate();
						if (innerRowsModifier1 != null)
							if (!actionWithModifier(innerRowsModifier1, cell))
								return useFirstCalculatedValueAsResult;
						if (innerRowsModifier2 != null)
							if (!actionWithModifier(innerRowsModifier2, cell))
								return useFirstCalculatedValueAsResult;
					}
					finally {
						if (settingProperty)
							cell.EndUpdate();
					}
				}
			}
			return true;
		}
		protected override bool SetValueCoreToColumnInterval(T value, Model.CellIntervalRange intervalRange, ActionWithModifier actionWithModifier, bool isModifyBorders) {
			int firstColumnIndex = intervalRange.TopLeft.Column;
			int lastColumnIndex = intervalRange.BottomRight.Column;
			Model.FormatBasePropertyModifierBase insideRowModifier1 = CreateInnerModifier1(value);
			Model.FormatBasePropertyModifierBase insideRowModifier2 = CreateInnerModifier2(value);
			IEnumerable<Model.Column> columns = GetColumns(firstColumnIndex, lastColumnIndex, isModifyBorders);
			foreach (Model.Column column in columns) {
				if (isModifyBorders)
					column.BeginUpdate();
				try {
					const bool useFirstCalculatedValueAsResult = true;
					if (!actionWithModifier(insideRowModifier1, column))
						return useFirstCalculatedValueAsResult;
					if (!actionWithModifier(insideRowModifier2, column))
						return useFirstCalculatedValueAsResult;
				}
				finally {
					if (isModifyBorders)
						column.EndUpdate();
				}
			}
			return false;
		}
		protected override bool SetValueCoreToRowInterval(T value, Model.CellIntervalRange intervalRange, ActionWithModifier actionWithModifier, bool ifSettingBorders) {
			int firstRowIndex = intervalRange.TopLeft.Row;
			int lastRowIndex = intervalRange.BottomRight.Row;
			bool rangeHasThreeOrMoreRows = lastRowIndex - firstRowIndex >= 2;
			bool rangeHasTwoRows = lastRowIndex - firstRowIndex == 1;
			bool rangeHasOneRow = lastRowIndex - firstRowIndex == 0;
			Model.FormatBasePropertyModifierBase topRowModifier = CreateFirstModifier(value);
			Model.FormatBasePropertyModifierBase bottomRowModifier = CreateLastModifier(value);
			Model.FormatBasePropertyModifierBase insideRowModifier1 = CreateInnerModifier1(value);
			Model.FormatBasePropertyModifierBase insideRowModifier2 = CreateInnerModifier2(value);
			const bool useFirstCalculatedValueAsResult = true;
			if (rangeHasOneRow) {
			}
			else if (rangeHasTwoRows || rangeHasThreeOrMoreRows) {
				Model.Row topOrNull = ifSettingBorders ? ModelWorksheet.Rows.GetRow(firstRowIndex) : null;
				Model.Row bottomOrNull = ifSettingBorders ? ModelWorksheet.Rows.GetRow(lastRowIndex) : null;
				if (topOrNull != null && !actionWithModifier(topRowModifier, topOrNull))
					return useFirstCalculatedValueAsResult;
				if (bottomOrNull != null && !actionWithModifier(bottomRowModifier, bottomOrNull))
					return useFirstCalculatedValueAsResult;
			}
			if (rangeHasThreeOrMoreRows) {
				int initialRowIndex = firstRowIndex + 1;
				int toRow = lastRowIndex - 1;
				IEnumerable<Model.Row> rows = GetRows(initialRowIndex, toRow, ifSettingBorders);
				foreach (Model.Row row in rows) {
					if (ifSettingBorders)
						row.BeginUpdate();
					try {
						if (!actionWithModifier(insideRowModifier1, row))
							return useFirstCalculatedValueAsResult;
						if (!actionWithModifier(insideRowModifier2, row))
							return useFirstCalculatedValueAsResult;
					}
					finally {
						if (ifSettingBorders)
							row.EndUpdate();
					}
				}
			}
			return false;
		}
	}
	public class HorizontalInsideBordersColorPropertyAccessor : HorizontalInsideBordersPropertyAccessor<Color> {
		public HorizontalInsideBordersColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			throw new InvalidOperationException();
		}
		protected override Model.FormatBasePropertyModifierBase CreateFirstModifier(Color value) {
			return new Model.BottomBorderColorPropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateInnerModifier1(Color value) {
			return new Model.TopBorderColorPropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateInnerModifier2(Color value) {
			return new Model.BottomBorderColorPropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateLastModifier(Color value) {
			return new Model.TopBorderColorPropertyModifier(value);
		}
	}
	public class HorizontalInsideBordersLineStylePropertyAccessor : HorizontalInsideBordersPropertyAccessor<XlBorderLineStyle> {
		public HorizontalInsideBordersLineStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlBorderLineStyle> CreateModifierCore(XlBorderLineStyle newValue) {
			throw new InvalidOperationException();
		}
		protected override Model.FormatBasePropertyModifierBase CreateFirstModifier(XlBorderLineStyle value) {
			return new Model.BottomBorderLineStylePropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateInnerModifier1(XlBorderLineStyle value) {
			return new Model.TopBorderLineStylePropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateInnerModifier2(XlBorderLineStyle value) {
			return new Model.BottomBorderLineStylePropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateLastModifier(XlBorderLineStyle value) {
			return new Model.TopBorderLineStylePropertyModifier(value);
		}
	}
}
