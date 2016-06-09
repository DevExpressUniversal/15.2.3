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

using DevExpress.Office.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ShowValuesAsCache {
		readonly Dictionary<PivotCellKey, ShowValueAsRankCache> rankCache = new Dictionary<PivotCellKey, ShowValueAsRankCache>(); 
		public void CacheRank(PivotCellKey key, PivotDataConsolidateFunction function, VariantValue rank, PivotDataField dataField) {
			ShowValueAsRankCache cachedValues;
			if (!rankCache.TryGetValue(key, out cachedValues)) {
				cachedValues = new ShowValueAsRankCache();
				rankCache.Add(key, cachedValues);
			}
			ShowValueAsRankCacheKey cacheKey = new ShowValueAsRankCacheKey(dataField.FieldIndex, function);
			if (!cachedValues.ContainsKey(cacheKey))
				cachedValues.Add(cacheKey, rank);
		}
		public bool TryGetCachedRank(PivotCellKey key, PivotDataConsolidateFunction function, PivotTable pivotTable, PivotDataField dataField, out VariantValue value) {
			ShowValueAsRankCache cachedValues;
			if (rankCache.TryGetValue(key, out cachedValues)) {
				ShowValueAsRankCacheKey cacheKey = new ShowValueAsRankCacheKey(dataField.FieldIndex, function);
				return cachedValues.TryGetValue(cacheKey, out value);
			}
			else {
				value = VariantValue.Empty;
				return false;
			}
		}
	}
	public class ShowValueAsRankCache : Dictionary<ShowValueAsRankCacheKey, VariantValue> {
	}
	public class ShowValueAsRankCacheKey {
		int fieldIndex;
		PivotDataConsolidateFunction function;
		public ShowValueAsRankCacheKey(int fieldIndex, PivotDataConsolidateFunction function) {
			this.fieldIndex = fieldIndex;
			this.function = function;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(fieldIndex, (int)function);
		}
		public override bool Equals(object obj) {
			ShowValueAsRankCacheKey other = obj as ShowValueAsRankCacheKey;
			if (other == null)
				return false;
			if (fieldIndex != other.fieldIndex || function != other.function)
				return false;
			return true;
		}
	}
	public interface IShowValuesAsCalculator {
		VariantValue Calculate(PivotCellKey key, PivotFieldItemType rowType, PivotFieldItemType columType, PivotDataField dataField, ShowValuesAsCache valuesCache, PivotTable pivotTable, int dataFieldIndex);
	}
	public static class ShowValuesAsCalculatorFactory {
		static Dictionary<PivotShowDataAs, IShowValuesAsCalculator> calculators = new Dictionary<PivotShowDataAs, IShowValuesAsCalculator>();
		static readonly object syncRoot = new object();
		public static IShowValuesAsCalculator GetCalculator(PivotShowDataAs showValuesAs) {
			lock (syncRoot) {
				IShowValuesAsCalculator result;
				if (!calculators.TryGetValue(showValuesAs, out result)) {
					result = CreateCalculator(showValuesAs);
					calculators.Add(showValuesAs, result);
				}
				return result;
			}
		}
		static IShowValuesAsCalculator CreateCalculator(PivotShowDataAs showValuesAs) {
			switch (showValuesAs) {
				case PivotShowDataAs.Normal:
					return new ShowValuesAsNormalCalculator();
				case PivotShowDataAs.PercentOfTotal:
					return new ShowValuesAsPercentOfTotalCalculator();
				case PivotShowDataAs.PercentOfColumn:
					return new ShowValuesAsPercentOfColumnCalculator();
				case PivotShowDataAs.PercentOfRow:
					return new ShowValuesAsPercentOfRowCalculator();
				case PivotShowDataAs.Percent:
					return new ShowValuesAsPercentCalculator();
				case PivotShowDataAs.Difference:
					return new ShowValuesAsDifferenceCalculator();
				case PivotShowDataAs.PercentDifference:
					return new ShowValuesAsPercentDifferenceCalculator();
				case PivotShowDataAs.PercentOfParentRow:
					return new ShowValuesAsPercentOfParentRowCalculator();
				case PivotShowDataAs.PercentOfParentColumn:
					return new ShowValuesAsPercentOfParentColumnCalculator();
				case PivotShowDataAs.PercentOfParent:
					return new ShowValuesAsPercentOfParentCalculator();
				case PivotShowDataAs.RunningTotal:
					return new ShowValuesAsRunningTotalCalculator();
				case PivotShowDataAs.PercentOfRunningTotal:
					return new ShowValuesAsPercentRunningTotalCalculator();
				case PivotShowDataAs.RankAscending:
					return new ShowValuesAsRankAscendingCalculator();
				case PivotShowDataAs.RankDescending:
					return new ShowValuesAsRankDescendingCalculator();
				case PivotShowDataAs.Index:
					return new ShowValuesAsIndexCalculator();
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
	}
	public abstract class ShowValuesAsCalculatorBase : IShowValuesAsCalculator {
		PivotCellKey key;
		PivotDataField dataField;
		int dataFieldIndex;
		ShowValuesAsCache valuesCache;
		PivotTable pivotTable;
		PivotFieldItemType resultType;
		PivotFieldItemType rowType;
		PivotFieldItemType columnType;
		PivotDataConsolidateFunction function;
		PivotDataConsolidateFunction rowFunction;
		PivotDataConsolidateFunction columnFunction;
		protected PivotCellKey Key { get { return key; } }
		protected PivotDataField DataField { get { return dataField; } }
		protected int DataFieldIndex { get { return dataFieldIndex; } }
		protected ShowValuesAsCache ValuesCache { get { return valuesCache; } }
		protected PivotTable PivotTable { get { return pivotTable; } }
		protected PivotFieldItemType ResultType { get { return resultType; } }
		protected PivotFieldItemType RowType { get { return rowType; } }
		protected PivotFieldItemType ColumnType { get { return columnType; } }
		protected PivotDataConsolidateFunction Function { get { return function; } }
		protected PivotDataConsolidateFunction RowFunction { get { return rowFunction; } }
		protected PivotDataConsolidateFunction ColumnFunction { get { return columnFunction; } }
		public VariantValue Calculate(PivotCellKey key, PivotFieldItemType rowType, PivotFieldItemType columnType, PivotDataField dataField, ShowValuesAsCache valuesCache, PivotTable pivotTable, int dataFieldIndex) {
			this.key = key;
			this.dataField = dataField;
			this.dataFieldIndex = dataFieldIndex;
			this.valuesCache = valuesCache;
			this.pivotTable = pivotTable;
			this.rowType = rowType;
			this.columnType = columnType;
			this.resultType = GetResultFieldItemType(rowType, columnType);
			if (resultType == PivotFieldItemType.Blank)
				return VariantValue.Empty;
			this.function = GetFunction(resultType);
			this.rowFunction = GetFunction(rowType);
			this.columnFunction = GetFunction(columnType);
			return Calculate();
		}
		PivotDataConsolidateFunction GetFunction(PivotFieldItemType itemType) {
			bool isCustomSubtotal = (itemType & (PivotFieldItemType.Grand | PivotFieldItemType.Data | PivotFieldItemType.DefaultValue)) == 0;
			return isCustomSubtotal ? (PivotDataConsolidateFunction)itemType : dataField.Subtotal;
		}
		PivotFieldItemType GetResultFieldItemType(PivotFieldItemType rowType, PivotFieldItemType columnType) {
			if (rowType == PivotFieldItemType.Data)
				return columnType;
			if (columnType == PivotFieldItemType.Data)
				return rowType;
			if (rowType == PivotFieldItemType.Grand)
				return columnType;
			if (columnType == PivotFieldItemType.Grand)
				return rowType;
			if (columnType == rowType)
				return columnType;
			return PivotFieldItemType.Blank;
		}
		protected PivotCellCalculationInfo GetCalcInfo(PivotCellKey key, PivotTable pivotTable) {
			PivotCalculatedCache cache = pivotTable.CalculationInfo.CalculatedCache;
			if (key.IsSubtotalKey(pivotTable))
				return cache.GetSubtotalInfo(key.DataKey);
			else
				return cache.GetCellInfo(key.DataKey);
		}
		protected VariantValue GetCalculatedValue(PivotCellKey key, PivotDataConsolidateFunction function, PivotTable pivotTable, int dataFieldIndex) {
			PivotCellCalculationInfo calcInfo = GetCalcInfo(key, pivotTable);
			if (calcInfo == null)
				return VariantValue.Missing;
			return calcInfo.GetValue(dataFieldIndex, function);
		}
		protected abstract VariantValue Calculate();
		protected VariantValue CalculateCellValue() {
			return GetCalculatedValue(key, function, pivotTable, dataFieldIndex);
		}
		protected VariantValue CalculateTotalValue(PivotCellKey key) {
			return GetCalculatedValue(key, dataField.Subtotal, pivotTable, dataFieldIndex);
		}
		protected VariantValue CalculateTotalValue(PivotCellKey key, PivotDataConsolidateFunction function) {
			return GetCalculatedValue(key, function, pivotTable, dataFieldIndex);
		}
		protected VariantValue CalculateGrandTotal() {
			return CalculateTotalValue(new PivotCellKey(key.DataKey.Length));
		}
		protected VariantValue CalculateColumnTotal() {
			return GetCalculatedValue(new PivotCellKey(new List<int>(key.ColumnKey), new List<int>(), key.ToColumnDataKey(pivotTable)), columnFunction, pivotTable, dataFieldIndex);
		}
		protected VariantValue CalculateRowTotal() {
			return GetCalculatedValue(new PivotCellKey(new List<int>(), new List<int>(key.RowKey), key.ToRowDataKey(pivotTable)), rowFunction, pivotTable, dataFieldIndex);
		}
		protected VariantValue CalculateDifferenceCore(VariantValue cellValue, VariantValue total) {
			if (total.IsError)
				return total;
			return cellValue.NumericValue - total.NumericValue;
		}
		protected VariantValue CalculatePercentCore(VariantValue cellValue, VariantValue total) {
			if (total.IsError)
				return total;
			if (total.NumericValue == 0)
				return VariantValue.ErrorDivisionByZero;
			return cellValue.NumericValue / total.NumericValue;
		}
		protected VariantValue CalculatePercentOfParentCore(VariantValue cellValue, VariantValue total) {
			if (total.IsMissing)
				return VariantValue.Empty;
			if (total.IsError)
				return VariantValue.Empty;
			if (total.NumericValue == 0)
				return VariantValue.ErrorDivisionByZero;
			return cellValue.NumericValue / total.NumericValue;
		}
	}
	public class ShowValuesAsNormalCalculator : ShowValuesAsCalculatorBase {
		protected override VariantValue Calculate() {
			return CalculateCellValue();
		}
	}
	public class ShowValuesAsIndexCalculator : ShowValuesAsCalculatorBase {
		protected override VariantValue Calculate() {
			VariantValue cellValue = CalculateCellValue();
			VariantValue grandTotal = CalculateGrandTotal();
			VariantValue rowTotal = CalculateRowTotal();
			VariantValue columnTotal = CalculateColumnTotal();
			return CalculatePercentCore(cellValue.NumericValue * grandTotal.NumericValue, rowTotal.NumericValue * columnTotal.NumericValue);
		}
	}
	public class ShowValuesAsPercentOfTotalCalculator : ShowValuesAsCalculatorBase {
		protected override VariantValue Calculate() {
			VariantValue cellValue = CalculateCellValue();
			VariantValue total = CalculateTotal();
			return CalculatePercentCore(cellValue, total);
		}
		protected virtual VariantValue CalculateTotal() {
			return CalculateGrandTotal();
		}
	}
	public class ShowValuesAsPercentOfColumnCalculator : ShowValuesAsPercentOfTotalCalculator {
		protected override VariantValue CalculateTotal() {
			return CalculateColumnTotal();
		}
	}
	public class ShowValuesAsPercentOfRowCalculator : ShowValuesAsPercentOfTotalCalculator {
		protected override VariantValue CalculateTotal() {
			return CalculateRowTotal();
		}
	}
	public abstract class ShowValuesAsCalculatorModifierKeyBase : ShowValuesAsCalculatorBase {
		PivotField field;
		PivotCellKeyModifier keyModifier;
		protected PivotField Field { get { return field; } }
		protected PivotCellKeyModifier KeyModifier { get { return keyModifier; } }
		protected virtual bool ShouldCheckIsItemHided { get { return false; } }
		protected abstract VariantValue CalculateCore();
		protected VariantValue CalculateCellValueKeyModifierBased() {
			return GetCalculatedValue(KeyModifier.Key, Function, PivotTable, DataFieldIndex);
		}
		protected override VariantValue Calculate() {
			field = PivotTable.Fields[DataField.BaseField];
			PivotTableAxis axis = field.Axis;
			if (axis == PivotTableAxis.Column)
				keyModifier = new PivotCellKeyColumnModifier(Key, PivotTable, DataField.BaseField, ColumnType, RowType);
			else if (axis == PivotTableAxis.Row)
				keyModifier = new PivotCellKeyRowModifier(Key, PivotTable, DataField.BaseField, ColumnType, RowType);
			else
				return VariantValue.ErrorValueNotAvailable;
			if (keyModifier.CurrentKeyCount <= keyModifier.Position) {
				if (ShouldCheckIsItemHided)
					if (keyModifier.CurrentKeyCount > 0) {
						int lastFieldReferenceIndex = keyModifier.CurrentKeyCount - 1;
						int fieldIndex = keyModifier.GetFields(PivotTable)[lastFieldReferenceIndex];
						if (fieldIndex != PivotTable.ValuesFieldFakeIndex) {
							PivotField lastField = PivotTable.Fields[fieldIndex];
							int itemIndex = keyModifier.GetItemIndex(lastFieldReferenceIndex);
							PivotItem item = lastField.Items[itemIndex];
							if (item.HideDetails)
								return VariantValue.ErrorValueNotAvailable;
						}
					}
				return VariantValue.Empty;
			}
			return CalculateCore();
		}
	}
	public class ShowValuesAsPercentCalculator : ShowValuesAsCalculatorModifierKeyBase {
		protected override bool ShouldCheckIsItemHided { get { return true; } }
		protected virtual bool ShouldProcessBaseItem { get { return true; } } 
		protected virtual bool IsPercent { get { return true; } }
		protected virtual VariantValue GetResult(VariantValue cellValue, VariantValue total) {
			return CalculatePercentCore(cellValue, total);
		}
		protected override VariantValue CalculateCore() {
			if (KeyModifier.GetItemIndex(0) == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex)
				return VariantValue.Empty;
			VariantValue cellValue = CalculateCellValue();
			bool isBaseItem;
			int totalItemIndex = DataField.BaseItem;
			if (totalItemIndex == PivotTableLayoutCalculator.PreviousItem)
				isBaseItem = SetPreviousIndex();
			else if (totalItemIndex == PivotTableLayoutCalculator.NextItem)
				isBaseItem = SetNextIndex();
			else {
				int itemIndex = KeyModifier.GetItemIndex();
				bool hidden = Field.Items[itemIndex].HideDetails;
				KeyModifier.SetItemIndex(totalItemIndex, PivotTable);
				if (Field.Items[totalItemIndex].HideDetails != hidden)
					return VariantValue.ErrorValueNotAvailable;
				isBaseItem = itemIndex == totalItemIndex;
			}
			if (!ShouldProcessBaseItem && isBaseItem)
				return VariantValue.Empty;
			if (!PivotReportContainsCell())
				return VariantValue.ErrorValueNotAvailable;
			VariantValue total = CalculateCellValueKeyModifierBased();
			if (IsPercent) {
				if (!isBaseItem) {
					if (cellValue.IsMissing)
						return VariantValue.ErrorNullIntersection;
					if (cellValue.IsError)
						return cellValue;
					if (total.IsError)
						return total;
				}
				else {
					if (cellValue.IsMissing)
						return VariantValue.Empty;
					if (cellValue.IsError)
						return VariantValue.Empty;
					if (total.IsError)
						return VariantValue.Empty;
				}
				if (total.IsMissing)
					return VariantValue.Empty;
			}
			else {
				if (cellValue.IsError)
					return cellValue;
				if (total.IsError)
					return total;
			}
			return GetResult(cellValue, total);
		}
		bool SetNextIndex() {
			int itemIndex = KeyModifier.GetItemIndex();
			int totalItemIndex = itemIndex;
			bool hidden = Field.Items[itemIndex].HideDetails;
			do {
				++totalItemIndex;
				if (totalItemIndex >= Field.Items.Count || Field.Items[totalItemIndex].ItemType != PivotFieldItemType.Data) {
					KeyModifier.SetItemIndex(itemIndex, PivotTable);
					return true;
				}
				KeyModifier.SetItemIndex(totalItemIndex, PivotTable);
			}
			while (Field.Items[totalItemIndex].HideDetails != hidden || !PivotReportContainsCell());
			return itemIndex == totalItemIndex;
		}
		bool SetPreviousIndex() {
			int itemIndex = KeyModifier.GetItemIndex();
			int totalItemIndex = itemIndex;
			bool hidden = Field.Items[itemIndex].HideDetails;
			do {
				--totalItemIndex;
				if (totalItemIndex < 0) {
					KeyModifier.SetItemIndex(itemIndex, PivotTable);
					return true;
				}
				KeyModifier.SetItemIndex(totalItemIndex, PivotTable);
			}
			while (Field.Items[totalItemIndex].HideDetails != hidden || !PivotReportContainsCell());
			return itemIndex == totalItemIndex;
		}
		bool PivotReportContainsCell() {
			PivotTableColumnRowFieldIndices indices = KeyModifier.GetFields(PivotTable);
			for (int i = 0; i < KeyModifier.CurrentKeyCount; ++i) {
				int fieldIndex = indices[i].FieldIndex;
				if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
					continue;
				PivotField field = PivotTable.Fields[fieldIndex];
				if (field.ShowItemsWithNoData || KeyModifier.CacheHasCurrentKeySequence(PivotTable, i + 1))
					continue;
				return false;
			}
			return true;
		}
	}
	public class ShowValuesAsDifferenceCalculator : ShowValuesAsPercentCalculator {
		protected override bool ShouldProcessBaseItem { get { return false; } }
		protected override bool IsPercent { get { return false; } }
		protected override VariantValue GetResult(VariantValue cellValue, VariantValue total) {
			return CalculateDifferenceCore(cellValue, total);
		}
	}
	public class ShowValuesAsPercentDifferenceCalculator : ShowValuesAsPercentCalculator {
		protected override bool ShouldProcessBaseItem { get { return false; } }
		protected override bool IsPercent { get { return true; } }
		protected override VariantValue GetResult(VariantValue cellValue, VariantValue total) {
			return CalculatePercentCore(CalculateDifferenceCore(cellValue, total), total);
		}
	}
	public class ShowValuesAsPercentOfParentCalculator : ShowValuesAsCalculatorModifierKeyBase {
		protected override VariantValue CalculateCore() {
			if (KeyModifier.GetItemIndex(0) == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex)
				return VariantValue.Empty;
			bool IsSubtotal = (KeyModifier.CurrentType & (PivotFieldItemType.SubtotalMask & ~PivotFieldItemType.DefaultValue)) > 0;
			if (IsSubtotal)
				return VariantValue.Empty;
			VariantValue cellValue = CalculateCellValue();
			int position = KeyModifier.Position + 1;
			KeyModifier.RemoveRange(position, KeyModifier.CurrentKeyCount - position, PivotTable);
			PivotTableColumnRowFieldIndices indices = KeyModifier.GetFields(PivotTable);
			if (indices.Count > position && indices[position].FieldIndex == PivotTable.ValuesFieldFakeIndex)
				return 1;
			VariantValue total = CalculateCellValueKeyModifierBased();
			return CalculatePercentOfParentCore(cellValue, total);
		}
	}
	public class ShowValuesAsPercentOfParentRowCalculator : ShowValuesAsCalculatorBase {
		protected override VariantValue Calculate() {
			VariantValue cellValue = CalculateCellValue();
			PivotCellKeyModifier key = GetKeyModifier(this.Key);
			if (key.CurrentKeyCount > 0) {
				int lastItemIndex = key.CurrentKeyCount - 1;
				PivotTableColumnRowFieldIndices indices = key.GetFields(PivotTable);
				if (key.GetItemIndex(0) == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex) {
					PivotField field = PivotTable.Fields[indices[lastItemIndex].FieldIndex];
					int lastItem = key.GetItemIndex(lastItemIndex);
					int nextFieldItem = lastItem + 1;
					if (field.Items[nextFieldItem].ItemType == PivotFieldItemType.Data)
						key.SetItemIndex(lastItemIndex, nextFieldItem, PivotTable);
					else
						key.RemoveAt(lastItemIndex, PivotTable);
				}
				else {
					if (indices.ValuesFieldIndex == key.CurrentKeyCount - 1) {
						if (lastItemIndex < 1)
							return VariantValue.Empty;
						key.RemoveAt(lastItemIndex, PivotTable);
						--lastItemIndex;
					}
					key.RemoveAt(lastItemIndex, PivotTable);
				}
			}
			VariantValue value = CalculateTotalValue(key.Key, GetFunction());
			return CalculatePercentOfParentCore(cellValue, value);
		}
		protected virtual PivotCellKeyModifier GetKeyModifier(PivotCellKey key) {
			return new PivotCellKeyRowModifier(Key, PivotTable, DataField.BaseField, ColumnType, RowType);
		}
		protected virtual PivotDataConsolidateFunction GetFunction() {
			return ColumnFunction;
		}
	}
	public class ShowValuesAsPercentOfParentColumnCalculator : ShowValuesAsPercentOfParentRowCalculator {
		protected override PivotCellKeyModifier GetKeyModifier(PivotCellKey key) {
			return new PivotCellKeyColumnModifier(Key, PivotTable, DataField.BaseField, ColumnType, RowType);
		}
		protected override PivotDataConsolidateFunction GetFunction() {
			return RowFunction;
		}
	}
	public class ShowValuesAsRunningTotalCalculator : ShowValuesAsCalculatorModifierKeyBase {
		protected override bool ShouldCheckIsItemHided { get { return true; } }
		protected override VariantValue CalculateCore() {
			if (KeyModifier.GetItemIndex(0) == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex)
				return VariantValue.Empty;
			int itemIndex = KeyModifier.GetItemIndex();
			PivotCellCalculationInfo resultInfo = GetItems(itemIndex, itemIndex);
			VariantValue cellValue = resultInfo.GetValue(DataFieldIndex, Function);
			return CalculateCore(cellValue);
		}
		protected virtual VariantValue CalculateCore(VariantValue cellValue) {
			if (cellValue.IsError)
				return cellValue;
			if (cellValue.IsMissing)
				return 0;
			return cellValue;
		}
		protected PivotCellCalculationInfo GetItems(int lastItemIndex, int itemIndex) {
			PivotCellCalculationInfo resultInfo = new PivotCellCalculationInfo(PivotTable.DataFields.Count);
			bool itemIsHided = Field.Items[itemIndex].HideDetails;
			if (lastItemIndex >= Field.Items.DataItemsCount)
				lastItemIndex = Field.Items.DataItemsCount - 1;
			for (int i = 0; i <= lastItemIndex; ++i) {
				PivotItem item = Field.Items[i];
				if (item.HideDetails == itemIsHided) {
					KeyModifier.SetItemIndex(i, PivotTable);
					PivotCellCalculationInfo currentInfo = GetCalcInfo(KeyModifier.Key, PivotTable);
					if (currentInfo != null) {
						resultInfo.AddValueGroup(currentInfo, true);
					}
				}
			}
			return resultInfo;
		}
		protected PivotCellCalculationInfo GetItemsNoChecks(int lastItemIndex) {
			PivotCellCalculationInfo resultInfo = new PivotCellCalculationInfo(PivotTable.DataFields.Count);
			if (lastItemIndex >= Field.Items.DataItemsCount)
				lastItemIndex = Field.Items.DataItemsCount - 1;
			for (int i = 0; i <= lastItemIndex; ++i) {
				PivotItem item = Field.Items[i];
				KeyModifier.SetItemIndex(i, PivotTable);
				PivotCellCalculationInfo currentInfo = GetCalcInfo(KeyModifier.Key, PivotTable);
				if (currentInfo != null) {
					resultInfo.AddValueGroup(currentInfo, true);
				}
			}
			return resultInfo;
		}
	}
	public class ShowValuesAsPercentRunningTotalCalculator : ShowValuesAsRunningTotalCalculator {
		protected override VariantValue CalculateCore(VariantValue cellValue) {
			bool bad = false;
			PivotDataConsolidateFunction function;
			PivotCellCalculationInfo resultInfo;
			if (KeyModifier.CurrentKeyCount < KeyModifier.GetFields(PivotTable).Count) {
				function = GetDefaultTotalFunction();
				resultInfo = GetItemsNoChecks(Field.Items.Count - 1);
			}
			else {
				function = GetTotalFunction(out bad);
				resultInfo = GetItems(Field.Items.Count - 1, KeyModifier.GetItemIndex());
			}
			VariantValue total;
			if (bad)
				if (cellValue.IsMissing)
					total = VariantValue.Missing;
				else
					total = VariantValue.Empty;
			else
				total = resultInfo.GetValue(DataFieldIndex, function);
			if (total.IsMissing)
				return VariantValue.Empty;
			return CalculatePercentCore(cellValue, total);
		}
		PivotDataConsolidateFunction GetTotalFunction(out bool bad) {
			bad = false;
			PivotDataConsolidateFunction subtotal;
			if (KeyModifier.Position > 0) {
				subtotal = FirstSubtotal();
				if ((KeyModifier.OtherType & PivotFieldItemType.SubtotalMask) > 0 && subtotal != (PivotDataConsolidateFunction)KeyModifier.OtherType)
					bad = true;
				return subtotal;
			}
			subtotal = GetDefaultTotalFunction();
			PivotTableColumnRowFieldIndices indices = KeyModifier.GetFields(PivotTable);
			int fieldReferenceIndex = indices.Count - 1;
			int fieldIndex;
			PivotField field;
			for (int i = 0; i < fieldReferenceIndex; ++i) {
				fieldIndex = indices[i].FieldIndex;
				if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
					continue;
				field = PivotTable.Fields[fieldIndex];
				for (int itemIndex = 0; itemIndex < field.Items.Count; ++itemIndex)
					if (field.Items[itemIndex].HideDetails)
						return subtotal;
			}
			fieldIndex = indices[fieldReferenceIndex].FieldIndex;
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return subtotal;
			field = PivotTable.Fields[fieldIndex];
			int lastDataItemIndex = field.Items.DataItemsCount - 1;
			if (lastDataItemIndex == field.Items.Count - 1)
				return subtotal;
			for (int i = lastDataItemIndex + 1; i < field.Items.Count; ++i) {
				PivotItem item = field.Items[i];
				if ((item.ItemType & (PivotFieldItemType.SubtotalMask & ~PivotFieldItemType.DefaultValue)) == 0)
					return subtotal;
				subtotal = (PivotDataConsolidateFunction)item.ItemType;
			}
			if ((KeyModifier.OtherType & PivotFieldItemType.SubtotalMask) > 0 && subtotal != (PivotDataConsolidateFunction)KeyModifier.OtherType)
				bad = true;
			return subtotal;
		}
		PivotDataConsolidateFunction FirstSubtotal() {
			int lastDataItemIndex = Field.Items.DataItemsCount - 1;
			++lastDataItemIndex;
			if (lastDataItemIndex < Field.Items.Count) {
				PivotItem item = Field.Items[lastDataItemIndex];
				if ((item.ItemType & (PivotFieldItemType.DefaultValue | PivotFieldItemType.Grand)) == 0)
					return (PivotDataConsolidateFunction)item.ItemType;
			}
			return DataField.Subtotal;
		}
		PivotDataConsolidateFunction GetDefaultTotalFunction() {
			if (KeyModifier is PivotCellKeyColumnModifier)
				return RowFunction;
			return ColumnFunction;
		}
	}
	public class ShowValuesAsRankAscendingCalculator : ShowValuesAsCalculatorModifierKeyBase {
		protected override VariantValue Calculate() {
			VariantValue cellValue;
			if (ValuesCache.TryGetCachedRank(Key, Function, PivotTable, DataField, out cellValue))
				return cellValue;
			return base.Calculate();
		}
		protected override VariantValue CalculateCore() {
			SortedDictionary<VariantValue, RankKeys> values = new SortedDictionary<VariantValue, RankKeys>(GetComparer(PivotTable.DocumentModel.SharedStringTable));
			PivotTableColumnRowFieldIndices indices = KeyModifier.GetFields(PivotTable);
			if (KeyModifier.CurrentKeyCount < indices.Count)
				PrepareSubtotalValues(values);
			else {
				if (indices.Count <= 1)
					PrepareDataValues(values);
				else {
					int fieldReferenceIndex = indices.Count - 1;
					if (KeyModifier.Position >= fieldReferenceIndex)
						if (KeyModifier.GetItemIndex(0) == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex)
							PrepareLastFieldSubtotalValues(values, 0, Field.Items.DataItemsCount - 1, fieldReferenceIndex);
						else
							PrepareDataValues(values);
					else {
						int lastItemIndex = KeyModifier.GetItemIndex(fieldReferenceIndex);
						PrepareDataValues(values);
						PrepareLastFieldSubtotalValues(values, lastItemIndex, lastItemIndex, fieldReferenceIndex);
					}
				}
			}
			int rank = 0;
			foreach (KeyValuePair<VariantValue, RankKeys> pair in values) {
				++rank;
				for (int i = 0; i < pair.Value.Count; ++i) {
					RankKey rankKey = pair.Value[i];
					ValuesCache.CacheRank(rankKey.Key, rankKey.Subtotal, rank, DataField);
				}
			}
			VariantValue result;
			ValuesCache.TryGetCachedRank(Key, Function, PivotTable, DataField, out result);
			return result;
		}
		protected virtual IComparer<VariantValue> GetComparer(SharedStringTable table) {
			return new SortVariantValueComparer(table);
		}
		void PrepareDataValues(SortedDictionary<VariantValue, RankKeys> values) {
			for (int i = 0; i < Field.Items.DataItemsCount; ++i) {
				PivotItem item = Field.Items[i];
				if (item.HideDetails)
					continue;
				KeyModifier.SetItemIndex(i, PivotTable);
				AddValue(values, Function);
			}
		}
		void PrepareLastFieldSubtotalValues(SortedDictionary<VariantValue, RankKeys> values, int firstItemIndex, int lastItemIndex, int fieldReferenceIndex) {
			PivotTableColumnRowFieldIndices indices = KeyModifier.GetFields(PivotTable);
			int fieldIndex;
			PivotField field;
			for (int i = 0; i < fieldReferenceIndex; ++i) {
				fieldIndex = indices[i].FieldIndex;
				if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
					continue;
				field = PivotTable.Fields[fieldIndex];
				for (int itemIndex = 0; itemIndex < field.Items.Count; ++itemIndex)
					if (field.Items[itemIndex].HideDetails)
						return;
			}
			fieldIndex = indices[fieldReferenceIndex].FieldIndex;
			if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
				return;
			field = PivotTable.Fields[fieldIndex];
			int lastDataItemIndex = field.Items.DataItemsCount - 1;
			if (lastDataItemIndex == field.Items.Count - 1)
				return;
			for (int i = 0; i < fieldReferenceIndex; ++i)
				KeyModifier.SetItemIndex(i, PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex, PivotTable);
			for (int i = firstItemIndex; i <= lastItemIndex; ++i) {
				KeyModifier.SetItemIndex(fieldReferenceIndex, i, PivotTable);
				for (int j = lastDataItemIndex + 1; j < field.Items.Count; ++j) {
					PivotItem item = field.Items[j];
					if ((item.ItemType & (PivotFieldItemType.SubtotalMask & ~PivotFieldItemType.DefaultValue)) == 0)
						break;
					if ((KeyModifier.OtherType & PivotFieldItemType.SubtotalMask) > 0 && item.ItemType != KeyModifier.OtherType)
						continue;
					AddValue(values, (PivotDataConsolidateFunction)item.ItemType);
				}
			}
		}
		void PrepareSubtotalValues(SortedDictionary<VariantValue, RankKeys> values) {
			for (int i = 0; i < Field.Items.DataItemsCount; ++i) {
				PivotItem item = Field.Items[i];
				KeyModifier.SetItemIndex(i, PivotTable);
				bool isSubtotal = KeyModifier.CurrentKeyCount - 1 == KeyModifier.Position;
				if (item.HideDetails) {
					if (isSubtotal)
						AddValue(values, DataField.Subtotal);
					continue;
				}
				PivotField subtotalField;
				if (isSubtotal)
					subtotalField = Field;
				else {
					int subtotalFieldReferenceIndex = KeyModifier.CurrentKeyCount - 1;
					int subtotalFieldIndex = KeyModifier.GetFields(PivotTable)[subtotalFieldReferenceIndex].FieldIndex;
					subtotalField = PivotTable.Fields[subtotalFieldIndex];
					int subtotalFieldItemIndex = KeyModifier.GetItemIndex(subtotalFieldReferenceIndex);
					PivotItem subtotalFieldItem = subtotalField.Items[subtotalFieldItemIndex];
					if (subtotalFieldItem.HideDetails) {
						AddValue(values, DataField.Subtotal);
						continue;
					}
				}
				for (int j = subtotalField.Items.Count - 1; j >= 0; --j) {
					PivotFieldItemType itemType = subtotalField.Items[j].ItemType;
					if ((itemType & PivotFieldItemType.SubtotalMask) == 0)
						break;
					PivotDataConsolidateFunction subtotal;
					if ((itemType & PivotFieldItemType.DefaultValue) > 0)
						subtotal = DataField.Subtotal;
					else {
						if ((KeyModifier.OtherType & PivotFieldItemType.SubtotalMask) > 0 && itemType != KeyModifier.OtherType)
							continue;
						subtotal = (PivotDataConsolidateFunction)itemType;
					}
					AddValue(values, subtotal);
				}
			}
		}
		void AddValue(SortedDictionary<VariantValue, RankKeys> values, PivotDataConsolidateFunction subtotal) {
			PivotCellKey key = KeyModifier.Key;
			VariantValue value = GetCalculatedValue(key, subtotal, PivotTable, DataFieldIndex);
			if (value.IsNumeric) {
				RankKey rankKey = new RankKey(subtotal, key.Clone()); 
				if (values.ContainsKey(value))
					values[value].Add(rankKey);
				else {
					RankKeys rankKeys = new RankKeys() { rankKey };
					values.Add(value, rankKeys);
				}
			}
			else
				if (value.IsMissing)
					ValuesCache.CacheRank(key, subtotal, VariantValue.Empty, DataField);
		}
	}
	public class ShowValuesAsRankDescendingCalculator : ShowValuesAsRankAscendingCalculator {
		protected override IComparer<VariantValue> GetComparer(SharedStringTable table) {
			return new DescendingSortVariantValueComparer(table);
		}
	}
	public class RankKeys : List<RankKey> {
	}
	public class RankKey {
		public RankKey(PivotDataConsolidateFunction subtotal, PivotCellKey key) {
			this.Subtotal = subtotal;
			this.Key = key;
		}
		public PivotDataConsolidateFunction Subtotal { get; set; }
		public PivotCellKey Key { get; set; }
	}
	public abstract class PivotCellKeyModifier {
		int position;
		PivotCellKey key;
		PivotFieldItemType columnType;
		PivotFieldItemType rowType;
		protected PivotCellKeyModifier(PivotCellKey key, PivotTable table, int baseField, PivotFieldItemType columnType, PivotFieldItemType rowType) {
			this.key = key.Clone();
			this.position = GetFields(table).IndexOf(baseField);
			this.columnType = columnType;
			this.rowType = rowType;
		}
		public int Position { get { return position; } }
		public PivotCellKey Key { get { return key; } }
		protected PivotFieldItemType ColumnType { get { return columnType; } }
		protected PivotFieldItemType RowType { get { return rowType; } }
		public int CurrentKeyCount { get { return CurrentKey.Count; } }
		public abstract PivotFieldItemType CurrentType { get; }
		public abstract PivotFieldItemType OtherType { get; }
		protected abstract List<int> CurrentKey { get; }
		public abstract PivotTableColumnRowFieldIndices GetFields(PivotTable table);
		public abstract bool CacheHasCurrentKeySequence(PivotTable table, int count);
		public abstract void RemoveAt(int index, PivotTable table);
		public abstract void RemoveRange(int index, int count, PivotTable table);
		public abstract void SetItemIndex(int position, int index, PivotTable table);
		public int GetItemIndex() {
			return GetItemIndex(position);
		}
		public int GetItemIndex(int position) {
			return CurrentKey[position];
		}
		public void SetItemIndex(int index, PivotTable table) {
			SetItemIndex(position, index, table);
		}
	}
	public class PivotCellKeyRowModifier : PivotCellKeyModifier {
		public PivotCellKeyRowModifier(PivotCellKey key, PivotTable table, int baseField, PivotFieldItemType columnType, PivotFieldItemType rowType)
			: base(key, table, baseField, columnType, rowType) {
		}
		public override PivotFieldItemType CurrentType { get { return RowType; } }
		public override PivotFieldItemType OtherType { get { return ColumnType; } }
		protected override List<int> CurrentKey { get { return Key.RowKey; } }
		public override PivotTableColumnRowFieldIndices GetFields(PivotTable table) {
			return table.RowFields;
		}
		public override bool CacheHasCurrentKeySequence(PivotTable pivotTable, int count) {
			return pivotTable.CalculatedCache.HasRowKeySequence(Key.ToRowDataKey(pivotTable, count)) == PivotCacheHasKeyResponse.Yes;
		}
		public override void RemoveAt(int index, PivotTable table) {
			Key.RemoveAtRow(index, table);
		}
		public override void RemoveRange(int index, int count, PivotTable table) {
			Key.RemoveRowRange(index, count, table);
		}
		public override void SetItemIndex(int position, int index, PivotTable table) {
			Key.SetRowIndex(position, index, table);
		}
	}
	public class PivotCellKeyColumnModifier : PivotCellKeyModifier {
		public PivotCellKeyColumnModifier(PivotCellKey key, PivotTable table, int baseField, PivotFieldItemType columnType, PivotFieldItemType rowType)
			: base(key, table, baseField, columnType, rowType) {
		}
		public override PivotFieldItemType CurrentType { get { return ColumnType; } }
		public override PivotFieldItemType OtherType { get { return RowType; } }
		protected override List<int> CurrentKey { get { return Key.ColumnKey; } }
		public override PivotTableColumnRowFieldIndices GetFields(PivotTable table) {
			return table.ColumnFields;
		}
		public override bool CacheHasCurrentKeySequence(PivotTable pivotTable, int count) {
			return pivotTable.CalculatedCache.HasColumnKeySequence(Key.ToColumnDataKey(pivotTable, count)) == PivotCacheHasKeyResponse.Yes;
		}
		public override void RemoveAt(int index, PivotTable table) {
			Key.RemoveAtColumn(index, table);
		}
		public override void RemoveRange(int index, int count, PivotTable table) {
			Key.RemoveColumnRange(index, count, table);
		}
		public override void SetItemIndex(int position, int index, PivotTable table) {
			Key.SetColumnIndex(position, index, table);
		}
	}
}
