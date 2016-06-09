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
	public abstract class InsideVerticalFormatsValuePropertyAccessor<T> : InsideBordersPropertyAccessor<T> {
		protected InsideVerticalFormatsValuePropertyAccessor(Model.CellRangeBase range)
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
			Model.CellRange firstColumn = null;
			Model.CellRange lastColumn = null;
			Model.CellRange insideColumns = null;
			if (range.Width >= 3) {
				firstColumn = range.GetSubColumnRange(0, 0);
				lastColumn = range.GetSubColumnRange(range.Width - 1, range.Width - 1);
				insideColumns = range.GetSubColumnRange(1, range.Width - 2);
			}
			else if (range.Width == 2) {
				firstColumn = range.GetSubColumnRange(0, 0);
				lastColumn = range.GetSubColumnRange(1, 1);
			}
			else
				return false;
			if (firstColumn != null) {
				Model.FormatBasePropertyModifierBase firstColumnModifier = CreateFirstModifier(value);
				IEnumerable<ICellBase> enumerable = GetCells(firstColumn, modifyOnlyExistingCells, settingProperty);
				foreach (Model.ICellBase cellInfo in enumerable) {
					ICell cell = cellInfo as ICell;
					if (cell != null)
						if (!actionWithModifier(firstColumnModifier, cell))
							return useFirstCalculatedValueAsResult;
				}
			}
			if (lastColumn != null) {
				Model.FormatBasePropertyModifierBase lastColumnModifier = CreateLastModifier(value);
				IEnumerable<ICellBase> enumerable = GetCells(lastColumn, modifyOnlyExistingCells, settingProperty);
				foreach (Model.ICellBase cellInfo in enumerable) {
					ICell cell = cellInfo as ICell;
					if (cell != null)
						if (!actionWithModifier(lastColumnModifier, cell))
							return useFirstCalculatedValueAsResult;
				}
			}
			if (insideColumns != null) {
				Model.FormatBasePropertyModifierBase innerColumnsModifier1 = CreateInnerModifier1(value);
				Model.FormatBasePropertyModifierBase innerColumnsModifier2 = CreateInnerModifier2(value);
				IEnumerable<ICellBase> enumerable = GetCells(insideColumns, modifyOnlyExistingCells, settingProperty);
				foreach (Model.ICellBase cellInfo in enumerable) {
					Model.ICell cell = cellInfo as Model.ICell;
					if (cell == null)
						continue;
					try {
						if (settingProperty)
							cell.BeginUpdate();
						if (innerColumnsModifier1 != null)
							if (!actionWithModifier(innerColumnsModifier1, cell))
								return useFirstCalculatedValueAsResult;
						if (innerColumnsModifier2 != null)
							if (!actionWithModifier(innerColumnsModifier2, cell))
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
			bool rangeHasThreeOrMoreColumns = lastColumnIndex - firstColumnIndex >= 2;
			bool rangeHasTwoColumns = lastColumnIndex - firstColumnIndex == 1;
			bool rangeHasOneColumn = lastColumnIndex - firstColumnIndex == 0;
			Model.FormatBasePropertyModifierBase leftColumnModifier = CreateFirstModifier(value);
			Model.FormatBasePropertyModifierBase rightColumnModifier = CreateLastModifier(value);
			Model.FormatBasePropertyModifierBase insideColumnModifier1 = CreateInnerModifier1(value);
			Model.FormatBasePropertyModifierBase insideColumnModifier2 = CreateInnerModifier2(value);
			const bool useFirstCalculatedValueAsResult = true;
			if (rangeHasOneColumn) {
			}
			else if (rangeHasTwoColumns || rangeHasThreeOrMoreColumns) {
				Model.Column leftColumn = isModifyBorders ? ModelWorksheet.Columns.GetIsolatedColumn(firstColumnIndex)
					: ModelWorksheet.Columns.GetReadonlyColumnRange(firstColumnIndex) as Model.Column;
				Model.Column rightColum = isModifyBorders ? ModelWorksheet.Columns.GetIsolatedColumn(lastColumnIndex)
					: ModelWorksheet.Columns.GetReadonlyColumnRange(lastColumnIndex) as Model.Column;
				if (!actionWithModifier(leftColumnModifier, leftColumn))
					return useFirstCalculatedValueAsResult;
				if (!actionWithModifier(rightColumnModifier, rightColum))
					return useFirstCalculatedValueAsResult;
			}
			if (rangeHasThreeOrMoreColumns) {
				int fromColumn = firstColumnIndex + 1;
				int toColumn = lastColumnIndex - 1;
				IEnumerable<Model.Column> insideColumns = GetColumns(fromColumn, toColumn, isModifyBorders);
				foreach (Model.Column insideColumn in insideColumns) {
					if (isModifyBorders)
						insideColumn.BeginUpdate();
					try {
						if (!actionWithModifier(insideColumnModifier1, insideColumn))
							return useFirstCalculatedValueAsResult;
						if (!actionWithModifier(insideColumnModifier2, insideColumn))
							return useFirstCalculatedValueAsResult;
					}
					finally {
						if (isModifyBorders)
							insideColumn.EndUpdate();
					}
				}
			}
			return false;
		}
		protected override bool SetValueCoreToRowInterval(T value, Model.CellIntervalRange intervalRange, ActionWithModifier actionWithModifier, bool ifSetting) {
			int firstRowIndex = intervalRange.TopLeft.Row;
			int lastRowIndex = intervalRange.BottomRight.Row;
			Model.FormatBasePropertyModifierBase modifier1 = CreateInnerModifier1(value);
			Model.FormatBasePropertyModifierBase modifier2 = CreateInnerModifier2(value);
			IEnumerable<Model.Row> rows = GetRows(firstRowIndex, lastRowIndex, ifSetting);
		   foreach(Model.Row row in rows) {
			   if (ifSetting)
					row.BeginUpdate();
				try {
					if (!actionWithModifier(modifier1, row))
						return true; 
					if (!actionWithModifier(modifier2, row))
						return true; 
				}
				finally {
					if (ifSetting)
						row.EndUpdate();
				}
			}
		   return false;
		}
	}
	public class InsideVerticalBorderColorPropertyAccessor : InsideVerticalFormatsValuePropertyAccessor<Color> {
		public InsideVerticalBorderColorPropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<Color> CreateModifierCore(Color newValue) {
			throw new InvalidOperationException();
		}
		protected override Model.FormatBasePropertyModifierBase CreateFirstModifier(Color value) {
			return new Model.RightBorderColorPropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateInnerModifier1(Color value) {
			return new Model.LeftBorderColorPropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateInnerModifier2(Color value) {
			return new Model.RightBorderColorPropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateLastModifier(Color value) {
			return new Model.LeftBorderColorPropertyModifier(value);
		}
	}
	public class InsideVerticalBorderLineStylePropertyAccessor : InsideVerticalFormatsValuePropertyAccessor<XlBorderLineStyle> {
		public InsideVerticalBorderLineStylePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<XlBorderLineStyle> CreateModifierCore(XlBorderLineStyle newValue) {
			throw new InvalidOperationException();
		}
		protected override Model.FormatBasePropertyModifierBase CreateFirstModifier(XlBorderLineStyle value) {
			return new Model.RightBorderLineStylePropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateInnerModifier1(XlBorderLineStyle value) {
			return new Model.LeftBorderLineStylePropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateInnerModifier2(XlBorderLineStyle value) {
			return new Model.RightBorderLineStylePropertyModifier(value);
		}
		protected override Model.FormatBasePropertyModifierBase CreateLastModifier(XlBorderLineStyle value) {
			return new Model.LeftBorderLineStylePropertyModifier(value);
		}
	}
}
