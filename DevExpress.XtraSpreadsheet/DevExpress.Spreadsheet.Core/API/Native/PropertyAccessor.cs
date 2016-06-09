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
using DevExpress.Utils;
using System.Diagnostics;
using DevExpress.Spreadsheet;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.API.Internal;
using ModelCell = DevExpress.XtraSpreadsheet.Model.ICell;
using ModelCellKey = DevExpress.XtraSpreadsheet.Model.CellKey;
using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
using ModelDefinedName = DevExpress.XtraSpreadsheet.Model.DefinedName;
using ModelDefinedNameDase = DevExpress.XtraSpreadsheet.Model.DefinedNameBase;
using ModelHyperlink = DevExpress.XtraSpreadsheet.Model.ModelHyperlink;
using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region PropertyAccessor<T> (abstract class)
	public abstract class PropertyAccessor<T> {
		public abstract T GetValue();
		public abstract bool SetValue(T value);
	}
	#endregion
	#region CachedPropertyAccessor<T>
	public class CachedPropertyAccessor<T> : PropertyAccessor<T> {
		T value;
		public T Value { get { return value; } set { this.value = value; } }
		public override T GetValue() {
			return value;
		}
		public override bool SetValue(T value) {
			if (Object.Equals(this.Value, value))
				return false;
			this.value = value;
			return true;
		}
	}
	#endregion
	#region CalculatedPropertyAccessor<T> (abstract class)
	public abstract class CalculatedPropertyAccessor<T> : PropertyAccessor<T> {
		public override T GetValue() {
			return CalculateValue();
		}
		protected internal abstract T CalculateValue();
	}
	#endregion
	#region SmartPropertyAccessor<T> (abstract class)
	public abstract class SmartPropertyAccessor<T> : CalculatedPropertyAccessor<T> {
		PropertyAccessor<T> currentAccessor;
		protected SmartPropertyAccessor() {
			this.currentAccessor = this;
		}
		public override T GetValue() {
			if (currentAccessor == this)
				return CalculateValue();
			else
				return currentAccessor.GetValue();
		}
		public override bool SetValue(T value) {
			bool result = SetValueCore(value);
			if (currentAccessor != this)
				result = currentAccessor.SetValue(value) || result;
			return result;
		}
		protected internal override T CalculateValue() {
			T result = CalculateValueCore();
			CachedPropertyAccessor<T> newAccessor = new CachedPropertyAccessor<T>();
			newAccessor.Value = result;
			currentAccessor = newAccessor;
			return result;
		}
		protected internal abstract T CalculateValueCore();
		protected internal abstract bool SetValueCore(T value);
	}
	#endregion
	#region RangeFormatsPropertyAccessor<T> (abstract class)
	public abstract class RangeFormatsPropertyAccessor<T> : SmartPropertyAccessor<T> {
		Model.CellRangeBase range;
		protected RangeFormatsPropertyAccessor(Model.CellRangeBase range) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
		}
		public Model.CellRangeBase Range { get { return range; } }
		public Model.Worksheet ModelWorksheet { get { return range.Worksheet as Model.Worksheet; } }
		protected internal override T CalculateValueCore() {
			if (range.CellCount == 0)
				return GetDefaultValue(); 
			DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier = CreateModifier(GetDefaultPropertyValue());
			Model.CellIntervalRange intervalRange = this.range as Model.CellIntervalRange;
			if (intervalRange != null) {
				if (intervalRange.IsColumnInterval)
					return GetValueFromColumnInterval(modifier, intervalRange);
				else
					return GetValueFromRowInterval(modifier, intervalRange);
			}
			else {
				ModelCellPosition position = range.GetFirstInnerCellRange().TopLeft;
				ModelCell modelCell = (ModelCell)ModelWorksheet.GetCellForFormatting(position);
				return CalculateValueCore(modelCell, modifier);
			}
		}
		protected internal virtual T GetDefaultPropertyValue() {
			return default(T);
		}
		protected internal virtual bool Compare(T value, T result) {
			return value.Equals(result);
		}
		protected internal override bool SetValueCore(T value) {
			if (range.CellCount == 0)
				return false;
			DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier = CreateModifier(value);
			DevExpress.XtraSpreadsheet.Model.Worksheet modelWorksheet = range.Worksheet as Model.Worksheet;
			modelWorksheet.Workbook.BeginUpdate();
			try {
				SetValueCoreForSingleRange(range, modifier, modelWorksheet);
			}
			finally{
				modelWorksheet.Workbook.EndUpdate();
			}
			return true;
		}
		void SetValueCoreForSingleRange(Model.CellRangeBase cellRangeBase, Model.FormatBasePropertyModifierBase modifier, Model.Worksheet modelWorksheet) {
			switch (cellRangeBase.RangeType) {
				case Model.CellRangeType.IntervalRange:
					Model.CellIntervalRange intervalRange = (Model.CellIntervalRange)cellRangeBase;
					if (intervalRange.IsColumnInterval)
						SetValueForColumnInterval(modifier, intervalRange);
					else
						SetValueForRowInterval(modifier, intervalRange);
					break;
				case Model.CellRangeType.UnionRange:
					Model.CellUnion union = (Model.CellUnion)cellRangeBase;
					foreach (Model.CellRangeBase innerCellRange in union.InnerCellRanges){
						ModelWorksheet innerRangeWorksheet = innerCellRange.Worksheet as ModelWorksheet;
						if (innerRangeWorksheet == null)
							innerRangeWorksheet = modelWorksheet;
						SetValueCoreForSingleRange(innerCellRange, modifier, innerRangeWorksheet);
					}
					break;
				default:
					Model.CellRange range = (Model.CellRange)cellRangeBase;
					foreach (Model.ICellBase cell in GetRangeCellForModifyEnumerator(range)) {
							modifier.ModifyFormat((Model.ICell)cell);
						}
					break;
			}
		}
		protected virtual IList<Model.Column> GetAffectedColumns(int firstColumnIndex, int lastColumnIndex) {
			return ModelWorksheet.Columns.GetColumnRangesEnsureExist(firstColumnIndex, lastColumnIndex);
		}
		protected virtual IList<Model.Row> GetAffectedRows(int first, int last) {
			Model.IRowCollection rows = ModelWorksheet.Rows;
			IList<Model.Row> result = new List<Model.Row>();
			for (int i = first; i <= last; i++) {
				result.Add(rows[i]);
			}
			return result;
		}
		void SetValueForColumnInterval(DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier, Model.CellIntervalRange intervalRange) {
			int firstColumn = intervalRange.TopLeft.Column;
			int lastColumn = intervalRange.BottomRight.Column;
			IList<Model.Column> affectedColumns = GetAffectedColumns(firstColumn, lastColumn);
			foreach (Model.Column column in affectedColumns) {
				modifier.ModifyFormat(column);
			}
			foreach (Model.ICellBase existingCell in GetRangeExistingCellForModifyEnumerator(intervalRange)) {
				modifier.ModifyFormat((Model.ICell)existingCell);
			}
		}
		void SetValueForRowInterval(DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier, Model.CellIntervalRange intervalRange) {
			int firstRow = intervalRange.TopLeft.Row;
			int lastRow = intervalRange.BottomRight.Row;
			IList<Model.Row> affectedRows = GetAffectedRows(firstRow, lastRow);
			foreach (Model.Row row in affectedRows) {
				modifier.ModifyFormat(row);
			}
			foreach (Model.ICellBase existingCell in GetRangeExistingCellForModifyEnumerator(intervalRange)) {
				modifier.ModifyFormat((Model.ICell)existingCell);
			}
		}
		T GetValueFromColumnInterval(DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier, Model.CellIntervalRange intervalRange) {
			int startColumnIndex = intervalRange.TopLeft.Column;
			Model.Column firstColumn = this.ModelWorksheet.Columns.GetColumnRangeForReading(startColumnIndex);
			T firstColumnValue = CalculateValueCore(firstColumn.FormatInfo, modifier);
			return firstColumnValue;
		}
		T GetValueFromRowInterval(DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier, Model.CellIntervalRange intervalRange) {
			int startRowIndex = intervalRange.TopLeft.Row;
			Model.Row firstRow = this.ModelWorksheet.Rows.TryGetRow(startRowIndex);
			if (firstRow == null)
				return default(T);
			T firstRowValue = CalculateValueCore(firstRow.FormatInfo, modifier);
			return firstRowValue;
		}
		protected internal virtual IEnumerable<Model.ICellBase> GetRangeExistingCellForModifyEnumerator(Model.CellRange range) {
			return range.GetExistingCellsEnumerable();
		}
		protected internal virtual System.Collections.Generic.IEnumerable<Model.ICellBase> GetRangeCellForModifyEnumerator(Model.CellRange range) {
			return range.GetAllCellsEnumerable();
		}
		protected Model.FormatBase GetDefaultFormat() {
			Model.DocumentModel workbook = range.Worksheet.Workbook as Model.DocumentModel;
			return workbook.StyleSheet.CellStyles.Normal.FormatInfo;
		}
		protected abstract T GetDefaultValue();
		protected internal abstract T CalculateValueCore(ModelCell cell, DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier);
		protected internal abstract T CalculateValueCore(Model.FormatBase format, DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier);
		protected internal abstract DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase CreateModifier(T newValue);
	}
	#endregion
	#region RangeCellsPropertyAccessor<T> (abstract class)   ONLY For Cell !!!!
	public abstract class RangeCellsPropertyAccessor<T> : SmartPropertyAccessor<T> {
		Model.CellRangeBase range;
		protected RangeCellsPropertyAccessor(Model.CellRangeBase range) {
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
		}
		protected virtual Model.CellRangeBase GetTargetRange(Model.CellRangeBase range) {
			return range;
		}
		protected internal override T CalculateValueCore() {
			Model.CellRangeBase targetRange = GetTargetRange(range);
			if (targetRange == null || targetRange.CellCount == 0)
				return GetDefaultValue(); 
			DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase modifier = CreateModifier(default(T));
			T result;
			Model.ICell firstCell = targetRange.GetFirstCellUnsafe() as Model.ICell;
			if (firstCell != null)
				result = CalculateValueCore(firstCell, modifier);
			else
				result = GetDefaultValue();
			int nonEmptyCellsCount = 0;
			foreach (Model.ICellBase cellInfo in targetRange.GetExistingCellsEnumerable()) {
				nonEmptyCellsCount++;
				if (nonEmptyCellsCount == 1)
					continue;
				T value = CalculateValueCore((Model.ICell)cellInfo, modifier);
				if (!Compare(value, result))
					return GetValueForDifferentValues();
			}
			if (nonEmptyCellsCount == 0)
				return result;
			if (nonEmptyCellsCount != targetRange.CellCount) {
				return GetValueForDifferentValues();
			}
			return result;
		}
		protected internal virtual bool Compare(T value, T result) {
			return value.Equals(result);
		}
		protected internal override bool SetValueCore(T value) {
			Model.CellRangeBase targetRange = GetTargetRange(range);
			if (targetRange == null || targetRange.CellCount == 0)
				return false;
			DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase modifier = CreateModifier(value);
			DevExpress.XtraSpreadsheet.Model.Worksheet modelWorksheet = targetRange.Worksheet as Model.Worksheet;
			ModelWorkbook workbook = modelWorksheet.Workbook;
			try {
				workbook.BeginUpdate();
				using (DevExpress.Office.History.HistoryTransaction transaction = new DevExpress.Office.History.HistoryTransaction(workbook.History)) {
					foreach (Model.ICell cell in targetRange.GetAllCellsEnumerable())
						modifier.ModifyFormat(cell);
				}
			}
			finally {
				workbook.EndUpdate();
			}
			return true;
		}
		Model.FormatBase GetDefaultFormat() {
			Model.DocumentModel workbook = range.Worksheet.Workbook as Model.DocumentModel;
			return workbook.Cache.CellFormatCache.DefaultCellStyleFormatItem;
		}
		protected abstract T GetDefaultValue();
		protected abstract T GetValueForDifferentValues();
		protected internal abstract T CalculateValueCore(Model.ICell cell, DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase modifier);
		protected internal abstract T CalculateValueCore(Model.FormatBase format, DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase modifier);
		protected internal abstract DevExpress.XtraSpreadsheet.Model.CellFormatPropertyModifierBase CreateModifier(T newValue);
	}
	#endregion
	#region RangeFormatsValuePropertyAccessor<T> (abstract)
	public abstract class RangeFormatsValuePropertyAccessor<T> : RangeFormatsPropertyAccessor<T> {
		protected RangeFormatsValuePropertyAccessor(Model.CellRangeBase range)
			: base(range) {
		}
		protected internal override T CalculateValueCore(ModelCell cell, DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier) {
			DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<T> typedModifier = (DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<T>)modifier;
			return typedModifier.GetFormatPropertyValue(cell);
		}
		protected internal override T CalculateValueCore(Model.FormatBase format, DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase modifier) {
			DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<T> typedModifier = (DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<T>)modifier;
			return typedModifier.GetFormatPropertyValue(format);
		}
		protected internal override DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifierBase CreateModifier(T newValue) {
			return CreateModifierCore(newValue);
		}
		protected override T GetDefaultValue() {
			return CalculateValueCore(GetDefaultFormat(), CreateModifier(default(T)));
		}
		protected internal abstract DevExpress.XtraSpreadsheet.Model.FormatBasePropertyModifier<T> CreateModifierCore(T newValue);
	}
	#endregion
	#region RangeMergedCellsPropertyAccessor<T> (abstract class)
	public abstract class RangeMergedCellsPropertyAccessor<T> : RangeCellsPropertyAccessor<T> {
		protected RangeMergedCellsPropertyAccessor(Model.CellRangeBase range) 
			: base(range) {
		}
		protected override Model.CellRangeBase GetTargetRange(Model.CellRangeBase range) {
			DevExpress.XtraSpreadsheet.Model.Worksheet modelWorksheet = range.Worksheet as Model.Worksheet;
			List<ModelCellRange> mergedRanges = modelWorksheet.MergedCells.GetMergedCellRangesIntersectsRange(range);
			if(mergedRanges.Count == 0)
				return range;
			List<Model.CellRangeBase> cellsToExclude = new List<Model.CellRangeBase>();
			foreach (ModelCellRange mergedRange in mergedRanges) {
				if (mergedRange.Width > 1)
					cellsToExclude.Add(new ModelCellRange(modelWorksheet, 
						new ModelCellPosition(mergedRange.LeftColumnIndex + 1, mergedRange.TopRowIndex), 
						new ModelCellPosition(mergedRange.RightColumnIndex, mergedRange.BottomRowIndex)));
				if (mergedRange.Height > 1)
					cellsToExclude.Add(new ModelCellRange(modelWorksheet, 
						new ModelCellPosition(mergedRange.LeftColumnIndex, mergedRange.TopRowIndex + 1), 
						new ModelCellPosition(mergedRange.LeftColumnIndex, mergedRange.BottomRowIndex)));
			}
			if (cellsToExclude.Count > 1) {
				Model.CellUnion rangeToExclude = new Model.CellUnion(modelWorksheet, cellsToExclude);
				return range.ExcludeRange(rangeToExclude);
			}
			return range.ExcludeRange(cellsToExclude[0]);
		}
	}
	#endregion
}
