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
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.PivotGrid;
namespace DevExpress.XtraPivotGrid.Data {
	public interface IPivotGridFieldsProvider {
		PivotGridFieldCollectionBase Fields { get; }
		PivotGridGroupCollection Groups { get; }
		PivotGridFieldBase GetFieldByArea(PivotArea area, int index);
		List<PivotGridFieldBase> GetFieldsByArea(PivotArea area, bool includeDataField);
		PivotGridFieldBase GetFieldByLevel(bool isColumn, int level);
		PivotDataArea DataFieldArea { get; }
	}
	public class PivotCellValuesProvider : ICellValuesProvider {
		readonly IPivotGridFieldsProvider fieldsProvider;
		readonly IPivotGridDataSourceOwner dataSourceOwner;
		readonly PivotDataSourceObjectLevelHelper dataSourceObjectLevelHelper;
		readonly ICellValuesProvider dataSourceCellValuesProvider;
		readonly Dictionary<PivotGridFieldBase, ICellValuesProvider> cellValuesProviders;
		readonly Dictionary<PivotGridFieldBase, List<PivotCalculationBase>> fieldCalculations;
		readonly Dictionary<PivotDataCoord, PivotCellValue> cachedValues = new Dictionary<PivotDataCoord,PivotCellValue>();
		readonly CalculatorInfo info;
		bool shouldCacheValues = false;
		bool isReady = false;
		bool hasDataFields = false;
		public PivotCellValuesProvider(IPivotGridFieldsProvider fieldsProvider, IPivotGridDataSourceOwner dataSourceOwner,
																PivotDataSourceObjectLevelHelper dataSourceObjectLevelHelper) {
			this.fieldsProvider = fieldsProvider;
			this.dataSourceOwner = dataSourceOwner;
			this.dataSourceObjectLevelHelper = dataSourceObjectLevelHelper;
			this.dataSourceCellValuesProvider = new DataSourceCellValuesProvider(dataSourceOwner);
			info = new CalculatorInfo(fieldsProvider, dataSourceObjectLevelHelper, dataSourceOwner);
			cellValuesProviders = new Dictionary<PivotGridFieldBase, ICellValuesProvider>();
			fieldCalculations = new Dictionary<PivotGridFieldBase, List<PivotCalculationBase>>();
		}
		public void Invalidate() {
			cellValuesProviders.Clear();
			fieldCalculations.Clear();
			cachedValues.Clear();
			isReady = false;
		}
		public void EnsureIsValid() {
			if(!isReady) {
				isReady = true;
				IList<PivotGridFieldBase> dataFields = fieldsProvider.GetFieldsByArea(PivotArea.DataArea, false);
				PivotRunningTotalCalculationBase forcedRunningTotalsCalculation = GetForcedRunningTotalCalculation();
				shouldCacheValues = false;
				if(dataFields.Count == 0) {
					hasDataFields = false;
					return;
				}
				hasDataFields = true;
				foreach(PivotGridFieldBase field in dataFields) {
					List<PivotCalculationBase> calculations = new List<PivotCalculationBase>();
					if(field.SummaryDisplayType != PivotSummaryDisplayType.Default || forcedRunningTotalsCalculation != null) {
						if(forcedRunningTotalsCalculation != null)
							calculations.Add(forcedRunningTotalsCalculation);
						if(field.SummaryDisplayType != PivotSummaryDisplayType.Default)
							calculations.Add(PivotCalculationBase.CreateCalculationBySummaryType(field));
					}
					else {
						calculations.AddRange(field.Calculations);
					}
					cellValuesProviders[field] = dataSourceCellValuesProvider;
					foreach(PivotCalculationBase calc in calculations) {
						CalculatorBase calculator = CreateCalculator(calc, cellValuesProviders[field]);
						RunningTotalsCalculator runningTotalCalculator = calculator as RunningTotalsCalculator;
						if(runningTotalCalculator != null) {
							shouldCacheValues = true;
							if(calc == forcedRunningTotalsCalculation)
								runningTotalCalculator.IsForced = true;
						}
						cellValuesProviders[field] = calculator;
					}
					fieldCalculations[field] = calculations;
				}
			}
		}
		public PivotCalculationBase GetLastCalculation(PivotGridFieldBase field) {
			if(fieldCalculations.ContainsKey(field) && fieldCalculations[field].Count > 0)
				return fieldCalculations[field][fieldCalculations[field].Count - 1];
			return null;
		}
		public IList<PivotCalculationBase> GetCalculations(PivotGridFieldBase field) {
			if(fieldCalculations.ContainsKey(field))
				return fieldCalculations[field];
			return null;
		}
		PivotRunningTotalCalculationBase GetForcedRunningTotalCalculation() {
			IList<PivotGridFieldBase> columnFields = GetRunningTotalFields(fieldsProvider.GetFieldsByArea(PivotArea.ColumnArea, false));
			IList<PivotGridFieldBase> rowFields = GetRunningTotalFields(fieldsProvider.GetFieldsByArea(PivotArea.RowArea, false));
			if(columnFields.Count == 0 && rowFields.Count == 0)
				return null;
			PivotRunningTotalCalculationBase calc = new PivotRunningTotalCalculationBase();
			if(columnFields.Count > 0 && rowFields.Count > 0) {
				calc.Direction = PivotRunningTotalCalculationDirection.RightThenDown;
			} else {
				if(columnFields.Count > 0)
					calc.Direction = PivotRunningTotalCalculationDirection.LeftToRight;
				else
					calc.Direction = PivotRunningTotalCalculationDirection.TopToBottom;
			}
			calc.ApplyToTargetCellsOnly = true;
			foreach(PivotGridFieldBase columnField in columnFields) {
				calc.TargetCellsInternal.Add(new PivotCalculationTargetCellInfoBase(columnField, null, true));
				foreach(PivotGridFieldBase rowField in rowFields) {
					calc.TargetCellsInternal.Add(new PivotCalculationTargetCellInfoBase(columnField, rowField));
				}
			}
			foreach(PivotGridFieldBase rowField in rowFields) {
				calc.TargetCellsInternal.Add(new PivotCalculationTargetCellInfoBase(rowField, null, false));
				foreach(PivotGridFieldBase columnField in columnFields) {
					calc.TargetCellsInternal.Add(new PivotCalculationTargetCellInfoBase(rowField, columnField));
				}
			}
			return calc;
		}
		IList<PivotGridFieldBase> GetRunningTotalFields(IList<PivotGridFieldBase> fields) {
			List<PivotGridFieldBase> res = new List<PivotGridFieldBase>();
			foreach(PivotGridFieldBase field in fields) {
				if(field.Visible && field.RunningTotal)
					res.Add(field);
			}
			return res;
		}
		CalculatorBase CreateCalculator(PivotCalculationBase calc, ICellValuesProvider cellValuesProvider) {
			PivotVariationCalculationBase variationCalculation = calc as PivotVariationCalculationBase;
			if(variationCalculation != null)
				return new VariationCalculator(variationCalculation, info, cellValuesProvider);
			PivotPercentageCalculationBase percentageCalculation = calc as PivotPercentageCalculationBase;
			if(percentageCalculation != null)
				return new PercentageCalculator(percentageCalculation, info, cellValuesProvider);
			PivotRankCalculationBase rankCalculation = calc as PivotRankCalculationBase;
			if(rankCalculation != null)
				return new RankCalculator(rankCalculation, info, cellValuesProvider);
			PivotRunningTotalCalculationBase runningTotalCalculation = calc as PivotRunningTotalCalculationBase;
			if(runningTotalCalculation != null)
				return new RunningTotalsCalculator(runningTotalCalculation, info, cellValuesProvider);
			PivotIndexCalculationBase indexCalculation = calc as PivotIndexCalculationBase;
			if(indexCalculation != null)
				return new IndexCalculator(info, cellValuesProvider);
			return null;
		}
		PivotCellValue ICellValuesProvider.GetCellValue(PivotDataCoord coord) {
			if(shouldCacheValues) {
				if(cachedValues.ContainsKey(coord))
					return cachedValues[coord];
			}
			PivotGridFieldBase dataField = fieldsProvider.GetFieldByArea(PivotArea.DataArea, coord.Data);
			ICellValuesProvider provider = hasDataFields && dataField != null ? cellValuesProviders[dataField] : dataSourceCellValuesProvider;
			PivotCellValue cellValue = provider.GetCellValue(coord);
			if(shouldCacheValues)
				cachedValues[coord] = cellValue;
			return cellValue;
		}
		internal Dictionary<PivotDataCoord, PivotCellValue> CachedValues { get { return cachedValues; } }
	}
	public abstract class CalculatorBase : ICellValuesProvider {
		readonly CalculatorInfo info;
		readonly ICellValuesProvider cellValuesProvider;
		protected CalculatorBase(CalculatorInfo info, ICellValuesProvider cellValuesProvider) {
			this.info = info;
			this.cellValuesProvider = cellValuesProvider;
		}
		protected CalculatorInfo Info { get { return info; } }
		protected ICellValuesProvider CellValuesProvider { get { return cellValuesProvider; } }
		PivotCellValue ICellValuesProvider.GetCellValue(PivotDataCoord coord) {
			PivotGridFieldBase dataField = info.FieldsProvider.GetFieldByArea(PivotArea.DataArea, coord.Data);
			if(dataField != null && ShouldCalculate(coord, dataField))
				return CalculateValue(coord, dataField);
			else
				return cellValuesProvider.GetCellValue(coord);
		}
		protected virtual bool ShouldCalculate(PivotDataCoord coord, PivotGridFieldBase dataField) {
			return true;
		}
		protected abstract PivotCellValue CalculateValue(PivotDataCoord coord, PivotGridFieldBase dataField);
	}
	public abstract class SpecificCellsCalculatorBase : CalculatorBase {
		readonly bool useTargetCells;
		readonly IList<PivotCalculationTargetCellInfoBase> targetCells;
		protected SpecificCellsCalculatorBase(PivotSpecificCellsCalculationBase calculation, CalculatorInfo info, ICellValuesProvider cellValuesProvider)
			: base(info, cellValuesProvider) {
			this.useTargetCells = calculation.ApplyToTargetCellsOnly;
			this.targetCells = CloneTargetCells(calculation.TargetCellsInternal);
		}
		IList<PivotCalculationTargetCellInfoBase> CloneTargetCells(IList<PivotCalculationTargetCellInfoBase> targetCells) {
			List<PivotCalculationTargetCellInfoBase> clone = new List<PivotCalculationTargetCellInfoBase>();
			foreach(PivotCalculationTargetCellInfoBase cellInfo in targetCells)
				clone.Add(new PivotCalculationTargetCellInfoBase(cellInfo.ColumnField, cellInfo.RowField, cellInfo.IsColumnGrandTotal));
			return clone;
		}
		protected override bool ShouldCalculate(PivotDataCoord coord, PivotGridFieldBase dataField) {
			if(!useTargetCells)
				return true;
			PivotGridFieldBase columnField = Info.FieldsProvider.GetFieldByLevel(true, Info.DataSource.GetObjectLevel(true, coord.Col)),
								rowField = Info.FieldsProvider.GetFieldByLevel(false, Info.DataSource.GetObjectLevel(false, coord.Row));
			foreach(PivotCalculationTargetCellInfoBase cellInfo in targetCells) {
				bool isColumnFieldMatch = (cellInfo.ColumnField == columnField);
				bool isRowFieldMatch = (cellInfo.ColumnField == rowField);
				if(cellInfo.ColumnField == null) {
					if(cellInfo.IsColumnGrandTotal) {
						isColumnFieldMatch = (columnField == null);
						isRowFieldMatch = false;
					} else {
						isRowFieldMatch = (rowField == null);
						isColumnFieldMatch = false;
					}
				}
				if(isColumnFieldMatch) {
					if(cellInfo.RowField == null || cellInfo.RowField == rowField)
						return true;
				}
				if(isRowFieldMatch) {
					if(cellInfo.RowField == null || cellInfo.RowField == columnField)
						return true;
				}
			}
			return false;
		}
	}
	public class VariationCalculator : SpecificCellsCalculatorBase {
		readonly DefaultBoolean calculateAcrossGroups;
		readonly PivotVariationCalculationDirection direction;
		readonly PivotVariationCalculationType variationType;
		public VariationCalculator(PivotVariationCalculationBase calculation, CalculatorInfo info, ICellValuesProvider cellValuesProvider)
		: base(calculation, info, cellValuesProvider){
			calculateAcrossGroups = calculation.CalculateAcrossGroups;
			direction = calculation.Direction;
			variationType = calculation.VariationType;
		}
		protected override PivotCellValue CalculateValue(PivotDataCoord coord, PivotGridFieldBase dataField) {
			bool isByColumn = direction == PivotVariationCalculationDirection.LeftToRight;
			int currentIndex = isByColumn ? coord.Col : coord.Row;
			int prevIndex = 0;
			if(calculateAcrossGroups != DefaultBoolean.Default)
				prevIndex = Info.LevelHelper.GetPrevIndex(isByColumn, currentIndex, calculateAcrossGroups.ToBoolean(true));
			else
				prevIndex = Info.LevelHelper.GetPrevIndex(isByColumn, currentIndex);
			if(prevIndex == -1) return null;
			PivotCellValue value = CellValuesProvider.GetCellValue(coord);
			if(value != null && value.Value == PivotSummaryValue.ErrorValue)
				return value;
			PivotDataCoord prevCoord = new PivotDataCoord(isByColumn ? prevIndex : coord.Col, isByColumn ? coord.Row : prevIndex, coord.Data, coord.Summary);
			PivotCellValue prevValue = CellValuesProvider.GetCellValue(prevCoord);
			if(value == null && prevValue == null)
				return null;
			if(prevValue != null && prevValue.Value == PivotSummaryValue.ErrorValue)
				return prevValue;
			decimal curVal = PivotCellValue.GetDecimalValue(value),
				prevVal = PivotCellValue.GetDecimalValue(prevValue);
			return new PivotCellValue(GetVariation(variationType, curVal, prevVal));
		}
		decimal GetVariation(PivotVariationCalculationType variationType, decimal curVal, decimal prevVal) {
			if(variationType == PivotVariationCalculationType.Absolute)
				return (curVal - prevVal);
			if(variationType == PivotVariationCalculationType.Percent) {
				if(prevVal != 0m)
					return (curVal - prevVal) / prevVal;
				if(curVal != 0m)
					return 1m;
				return 0m;
			}
			throw new Exception("Incorrect Variation Type");
		}
	}
	public class PercentageCalculator : SpecificCellsCalculatorBase {
		readonly PivotPercentageCalculationBaseLevel baseLevel;
		readonly PivotPercentageCalculationBaseType baseType;
		readonly object[] columnValues;
		readonly object rowValues;
		public PercentageCalculator(PivotPercentageCalculationBase calculation, CalculatorInfo info, ICellValuesProvider cellValuesProvider)
			: base(calculation, info, cellValuesProvider) {
			baseLevel = calculation.BaseLevel;
			baseType = calculation.BaseType;
			columnValues = calculation.ColumnValues != null ? new List<object>(calculation.ColumnValues).ToArray() : null;
			rowValues = calculation.RowValues != null ? new List<object>(calculation.RowValues).ToArray() : null;
		}
		protected override PivotCellValue CalculateValue(PivotDataCoord coord, PivotGridFieldBase dataField) {
			bool byColumn = baseType == PivotPercentageCalculationBaseType.Column;
			bool byRow = baseType == PivotPercentageCalculationBaseType.Row;
			if(baseType == PivotPercentageCalculationBaseType.Cell) {
				byColumn = true;
				byRow = true;
			}
			PivotCellValue baseValue = CellValuesProvider.GetCellValue(coord);
			PivotDataCoord totalCoord = GetTotalCoord(coord, byColumn, byRow, baseLevel);
			PivotCellValue totalCellValue = CellValuesProvider.GetCellValue(totalCoord);
			object value = PivotCellValue.GetValue(baseValue);
			object totalValue = PivotCellValue.GetValue(totalCellValue);
			if(value == PivotSummaryValue.ErrorValue || totalValue == PivotSummaryValue.ErrorValue)
				return PivotCellValue.ErrorValue;
			try {
				decimal decValue = PivotCellValue.GetDecimalValue(baseValue),
							decTotalValue = PivotCellValue.GetDecimalValue(totalCellValue);
				if(decTotalValue == 0)
					return PivotCellValue.Zero;
				return new PivotCellValue(decValue / decTotalValue);
			} catch {
				return PivotCellValue.ErrorValue;
			}
		}
		PivotDataCoord GetTotalCoord(PivotDataCoord coord, bool byColumn, bool byRow, PivotPercentageCalculationBaseLevel percType) {
			int columnIndex = coord.Col;
			int rowIndex = coord.Row;
			switch(percType) {
				case PivotPercentageCalculationBaseLevel.Custom:
					columnIndex = -1;
					rowIndex = -1;
					break;
				case PivotPercentageCalculationBaseLevel.Total:
					if(byColumn)
						rowIndex = GetTotalIndex(false, rowIndex);
					if(byRow)
						columnIndex = GetTotalIndex(true, columnIndex);
					break;
				case PivotPercentageCalculationBaseLevel.GrandTotal:
					if(byColumn)
						rowIndex = -1;
					if(byRow)
						columnIndex = -1;
					break;
			}
			return new PivotDataCoord(columnIndex, rowIndex, coord.Data, coord.Summary);
		}
		int GetTotalIndex(bool byColumn, int currentIndex) {
			if(currentIndex == -1)
				return -1;
			int totalIndex = currentIndex,
				curLevel = Info.DataSource.GetObjectLevel(byColumn, currentIndex);
			if(curLevel == 0)
				totalIndex = -1;
			else
				while(Info.DataSource.GetObjectLevel(byColumn, totalIndex) >= curLevel) totalIndex--;
			return totalIndex;
		}
	}
	public class RankCalculator : SpecificCellsCalculatorBase {
		readonly Dictionary<PivotGridFieldBase, Dictionary<int, Dictionary<int, Dictionary<int, PivotCellValue>>>> ranks;
		readonly PivotRankCalculationOrder order;
		readonly PivotRankCalculationScope scope;
		public RankCalculator(PivotRankCalculationBase calculation, CalculatorInfo info, ICellValuesProvider cellValuesProvider)
			: base(calculation, info, cellValuesProvider) {
			order = calculation.Order;
			scope = calculation.Scope;
			ranks = new Dictionary<PivotGridFieldBase, Dictionary<int, Dictionary<int, Dictionary<int, PivotCellValue>>>>();
		}
		protected override PivotCellValue CalculateValue(PivotDataCoord coord, PivotGridFieldBase dataField) {
			bool isRankColumn = scope == PivotRankCalculationScope.Column;
			if(GetRankVariableIndex(coord, isRankColumn) < 0) 
				return PivotCellValue.Null;
			bool isReverseOrder = order == PivotRankCalculationOrder.LargestToSmallest;
			PivotGridFieldBase baseField = GetBaseField(coord, isRankColumn);
			if(baseField == null)
				return PivotCellValue.Null;
			Dictionary<int, Dictionary<int, Dictionary<int, PivotCellValue>>> pair1;
			if(!ranks.TryGetValue(baseField, out pair1)) {
				pair1 = new Dictionary<int, Dictionary<int, Dictionary<int, PivotCellValue>>>();
				ranks[baseField] = pair1;
			}
			Dictionary<int, Dictionary<int, PivotCellValue>> pair2;
			if(!pair1.TryGetValue(GetRankBaseIndex(coord, isRankColumn), out pair2)) {
				pair2 = new Dictionary<int, Dictionary<int, PivotCellValue>>();
				pair1[GetRankBaseIndex(coord, isRankColumn)] = pair2;
			}
			Dictionary<int, PivotCellValue> pair3;
			int groupStartIndex = Info.LevelHelper.GetGroupStartIndex(!isRankColumn, GetRankVariableIndex(coord, isRankColumn));
			if(!pair2.TryGetValue(groupStartIndex, out pair3)) {
				pair3 = CreateRank(coord, groupStartIndex, isRankColumn, isReverseOrder);
				pair2[groupStartIndex] = pair3;
			}
			PivotCellValue cell;
			if(pair3.TryGetValue(GetRankVariableIndex(coord, isRankColumn), out cell))
				return cell;
			return PivotCellValue.Null;
		}
		int GetRankBaseIndex(PivotDataCoord coord, bool isRankColumn) {
			return isRankColumn ? coord.Col : coord.Row;
		}
		PivotGridFieldBase GetBaseField(PivotDataCoord coord, bool isRankColumn) {
			int level = Info.DataSource.GetObjectLevel(!isRankColumn, isRankColumn ? coord.Row : coord.Col);
			return Info.FieldsProvider.GetFieldByLevel(!isRankColumn, level);
		}
		int GetRankVariableIndex(PivotDataCoord coord, bool isRankColumn) {
			return isRankColumn ? coord.Row : coord.Col;
		}
		Dictionary<int, PivotCellValue> CreateRank(PivotDataCoord coord, int groupIndex, bool isRankColumn, bool isReverseOrder) {
			int baseIndex = GetRankBaseIndex(coord, isRankColumn);
			Dictionary<int, PivotCellValue> valuesByIndex = new Dictionary<int, PivotCellValue>();
			do {
				PivotDataCoord curCoord = new PivotDataCoord(isRankColumn ? baseIndex : groupIndex, isRankColumn ? groupIndex : baseIndex, coord.Data, coord.Summary);
				PivotCellValue value = CellValuesProvider.GetCellValue(curCoord);
				if(value == null)
					continue;
				valuesByIndex.Add(groupIndex, value);
			} while((groupIndex = Info.LevelHelper.GetNextIndex(!isRankColumn, groupIndex)) > 0);
			Dictionary<object, object> uniqueValues = new Dictionary<object, object>();
			foreach(int index in valuesByIndex.Keys) {
				PivotCellValue value = valuesByIndex[index];
				if(value == null || value.Value == null)
					continue;
				uniqueValues[PivotCellValue.GetValue(value)] = null;
			}
			List<object> sortedValues = new List<object>(uniqueValues.Keys);
			if(sortedValues.Contains(PivotSummaryValue.ErrorValue))
				sortedValues.Remove(PivotSummaryValue.ErrorValue);
			sortedValues.Sort(new DevExpress.PivotGrid.QueryMode.UniqueValuesComparer());
			if(isReverseOrder)
				sortedValues.Reverse();
			Dictionary<int, PivotCellValue> result = new Dictionary<int, PivotCellValue>();
			foreach(int index in valuesByIndex.Keys) {
				PivotCellValue value = valuesByIndex[index];
				if(value == null || value.Value == null) {
					result[index] = PivotCellValue.Null;
				} else {
					object rankValue = PivotCellValue.GetValue(value);
					if(sortedValues.Contains(rankValue))
						rankValue = sortedValues.IndexOf(rankValue) + 1;
					result[index] = new PivotCellValue(rankValue);
				}
			}
			return result;
		}
	}
	public class RunningTotalsCalculator : SpecificCellsCalculatorBase {
		readonly PivotRunningTotalCalculationDirection direction;
		readonly DefaultBoolean calculateAcrossGroups;
		bool isForced = false;
		public bool IsForced {
			get { return isForced; }
			set { isForced = value; }
		}
		public RunningTotalsCalculator(PivotRunningTotalCalculationBase calculation, CalculatorInfo info, ICellValuesProvider cellValuesProvider)
			: base(calculation, info, cellValuesProvider) {
			direction = calculation.Direction;
			calculateAcrossGroups = calculation.CalculateAcrossGroups;
		}
		protected override PivotCellValue CalculateValue(PivotDataCoord coord, PivotGridFieldBase dataField) {
			if((isForced && Info.DataSource.ShouldCalculateRunningSummary) || dataField.DataType == typeof(DateTime) || dataField.IsKPIMeasure)
				return CellValuesProvider.GetCellValue(coord);
			PivotGridFieldBase columnField = Info.FieldsProvider.GetFieldByArea(PivotArea.ColumnArea, Info.DataSource.GetObjectLevel(true, coord.Col));
			PivotGridFieldBase rowField = Info.FieldsProvider.GetFieldByArea(PivotArea.RowArea, Info.DataSource.GetObjectLevel(false, coord.Row));
			if(columnField == null && rowField == null)
				return CellValuesProvider.GetCellValue(coord);
			bool isRunningColumn = IsRunning(dataField, columnField),
				 isRunningRow = IsRunning(dataField, rowField);
			return GetRunningCellValue(coord, isRunningColumn, isRunningRow);
		}
		PivotCellValue GetRunningCellValue(PivotDataCoord coord, bool runColumn, bool runRow) {
			PivotCellValue value = CellValuesProvider.GetCellValue(coord);
			try {
				if (runColumn)
					value = PivotCellValue.Sum(value, GetRunningCellValueCore(coord, true));
				if (runRow)
					value = PivotCellValue.Sum(value, GetRunningCellValueCore(coord, false));
				return value;
			} catch {
				return PivotCellValue.ErrorValue;
			}
		}
		PivotCellValue GetRunningCellValueCore(PivotDataCoord coord, bool isColumn) {
			PivotDataCoord.Iterator iterator = new PivotDataCoord.Iterator(coord, isColumn);
			int startLevel = Info.DataSource.GetObjectLevel(isColumn, iterator.Coord);
			bool hasChildren = true;
			PivotCellValue res = PivotCellValue.Null;
			while(iterator.MovePrevious(Info.LevelHelper, calculateAcrossGroups)) {
				int curLevel = Info.DataSource.GetObjectLevel(isColumn, iterator.Coord);
				if(curLevel == startLevel) {
					hasChildren = true;
					res = PivotCellValue.Sum(res, CellValuesProvider.GetCellValue(iterator.Child));
				} else {
					if(curLevel == startLevel - 1) {
						if(!hasChildren)
							res = PivotCellValue.Sum(res, CellValuesProvider.GetCellValue(iterator.Child));
						hasChildren = false;
					}
				}
			}
			return res;
		}
		bool IsRunning(PivotGridFieldBase dataField, PivotGridFieldBase field) {
			if(dataField == null || field == null)
				return false;
			bool isRunning = false;
			switch(direction) {
				case PivotRunningTotalCalculationDirection.LeftToRight:
				isRunning = (field.Area == PivotArea.ColumnArea);
				break;
				case PivotRunningTotalCalculationDirection.TopToBottom:
				isRunning = (field.Area == PivotArea.RowArea);
				break;
				case PivotRunningTotalCalculationDirection.RightThenDown:
				isRunning = true;
				break;
			}
			return isRunning;
		}
	}
	public class IndexCalculator : CalculatorBase {
		public IndexCalculator(CalculatorInfo info, ICellValuesProvider cellValuesProvider)
			: base(info, cellValuesProvider) {
		}
		protected override PivotCellValue CalculateValue(PivotDataCoord coord, PivotGridFieldBase dataField) {
			decimal value = PivotCellValue.GetDecimalValue(CellValuesProvider.GetCellValue(coord));
			decimal grandTotalValue = GetGrandTotalValue(coord);
			decimal columnGrandTotalValue = GetColumnGrandTotalValue(coord);
			decimal rowGrandTotalValue = GetRowGrandTotalValue(coord);
			decimal denominator = columnGrandTotalValue * rowGrandTotalValue;
			if(denominator == 0m)
				return PivotCellValue.One;
			return new PivotCellValue(value * grandTotalValue / denominator);
		}
		decimal GetGrandTotalValue(PivotDataCoord coord) {
			PivotDataCoord grandTotal = new PivotDataCoord(-1, -1, coord.Data, coord.Summary);
			return PivotCellValue.GetDecimalValue(CellValuesProvider.GetCellValue(grandTotal));
		}
		decimal GetColumnGrandTotalValue(PivotDataCoord coord) {
			PivotDataCoord columnGrandTotal = new PivotDataCoord(-1, coord.Row, coord.Data, coord.Summary);
			return PivotCellValue.GetDecimalValue(CellValuesProvider.GetCellValue(columnGrandTotal));
		}
		decimal GetRowGrandTotalValue(PivotDataCoord coord) {
			PivotDataCoord rowGrandTotal = new PivotDataCoord(coord.Col, -1, coord.Data, coord.Summary);
			return PivotCellValue.GetDecimalValue(CellValuesProvider.GetCellValue(rowGrandTotal));
		}
	}
	public class DataSourceCellValuesProvider : ICellValuesProvider {
		readonly IPivotGridDataSourceOwner dataSourceOwner;
		public DataSourceCellValuesProvider(IPivotGridDataSourceOwner dataSourceOwner) {
			this.dataSourceOwner = dataSourceOwner;
		}
		PivotCellValue ICellValuesProvider.GetCellValue(PivotDataCoord coord) {
			return dataSourceOwner.DataSource.GetCellValue(coord.Col, coord.Row, coord.Data, coord.Summary);
		}
	}
	public interface ICellValuesProvider {
		PivotCellValue GetCellValue(PivotDataCoord coord);
	}
	public class CalculatorInfo {
		readonly IPivotGridFieldsProvider fieldsProvider;
		readonly PivotDataSourceObjectLevelHelper dataSourceObjectLevelHelper;
		readonly IPivotGridDataSourceOwner dataSourceOwner;
		public CalculatorInfo(IPivotGridFieldsProvider fieldsProvider, PivotDataSourceObjectLevelHelper dataSourceObjectLevelHelper, IPivotGridDataSourceOwner dataSourceOwner) {
			this.fieldsProvider = fieldsProvider;
			this.dataSourceObjectLevelHelper = dataSourceObjectLevelHelper;
			this.dataSourceOwner = dataSourceOwner;
		}
		public IPivotGridFieldsProvider FieldsProvider { get { return fieldsProvider; } }
		public PivotDataSourceObjectLevelHelper LevelHelper { get { return dataSourceObjectLevelHelper; } }
		public IPivotGridDataSource DataSource { get { return dataSourceOwner.DataSource; } }
	}
}
