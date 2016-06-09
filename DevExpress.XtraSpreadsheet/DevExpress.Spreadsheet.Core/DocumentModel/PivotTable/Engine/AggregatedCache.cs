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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
namespace DevExpress.XtraSpreadsheet.Model {
	#region AggregatedCacheCalculator
	public class AggregatedCacheCalculator {
		#region AggregatorThreadArguments
		class AggregatorThreadArguments {
			public PivotCalculatedCache CalculatedCache { get; set; }
			public int StartIndex { get; set; }
			public int EndIndex { get; set; }
		}
		#endregion
		const int maxThreadCount = 8;
		const int minRecordsPerThread = 4096;
		readonly IPivotTableTransaction transaction;
		readonly List<int> keyFieldIndices;
		public AggregatedCacheCalculator(IPivotTableTransaction transaction) {
			this.transaction = transaction;
			this.keyFieldIndices = transaction.GetKeyFieldIndices();
		}
		#region Properties
		IPivotTableTransaction Transaction { get { return transaction; } }
		PivotCache PivotCache { get { return Transaction.PivotTable.Cache; } }
		PivotTable PivotTable { get { return Transaction.PivotTable; } }
		#endregion
		public PivotCalculatedCache Calculate() {
			PivotCalculatedCache result;
			if (keyFieldIndices.Count <= 0) {
				if (PivotTable.DataFields.Count < 0)
					result = CalculateWithoutKeyFields();
				else
					result = CalculateWithDataFieldsOnly();
			}
			else
				result = CalculateCore();
			return result;
		}
		PivotCalculatedCache CalculateWithoutKeyFields() {
			return new PivotCalculatedCache(PivotTable, keyFieldIndices); 
		}
		PivotCalculatedCache CalculateWithDataFieldsOnly() {
			PivotCalculatedCache result = new PivotCalculatedCache(PivotTable, keyFieldIndices);
			PivotCellCalculationInfo grandTotal = new PivotCellCalculationInfo(PivotTable.DataFields.Count);
			for (int i = 0; i < PivotCache.Records.Count; i++) {
				IPivotCacheRecord record = PivotCache.Records[i];
				if (CheckPageFilters(record))
					grandTotal.AddValue(record, PivotTable);
			}
			result.AddDataFieldsSubtotal(grandTotal);
			return result;
		}
		PivotCalculatedCache CalculateCore() {
			PivotCalculatedCache result;
			System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
			int recordCount = PivotCache.Records.Count;
			int threadCount = recordCount / minRecordsPerThread;
			if (threadCount > 0)
				result = IterateThroughDataMultithreaded();
			else
				result = IterateThroughDataSingleThread();
			System.Diagnostics.Debug.WriteLine("The pivot table aggregation takes " + stopWatch.Elapsed);
			result.CalculateSubtotals(transaction);
			System.Diagnostics.Debug.WriteLine("The pivot table aggregation + subtotal calculation takes " + stopWatch.Elapsed);
			return result;
		}
		PivotCalculatedCache IterateThroughDataSingleThread() {
			int recordsCount = PivotCache.Records.Count;
			PivotCalculatedCache calculatedCache = new PivotCalculatedCache(PivotTable, keyFieldIndices);
			AggregatorThreadArguments arguments = new AggregatorThreadArguments() { CalculatedCache = calculatedCache, StartIndex = 0, EndIndex = recordsCount };
			IterateThroughDataThreadTask(arguments);
			return calculatedCache;
		}
		#region IterateThroughDataMultithreaded
		PivotCalculatedCache IterateThroughDataMultithreaded() {
			int recordsCount = PivotCache.Records.Count;
			int threadCount = Math.Min(maxThreadCount, recordsCount / minRecordsPerThread);
			int recordsPerThread = recordsCount / threadCount;
			List<PivotCalculatedCache> calculatedCachesPerThread = new List<PivotCalculatedCache>();
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < threadCount; i++) {
				PivotCalculatedCache calculatedCache = new PivotCalculatedCache(PivotTable, keyFieldIndices);
				int startIndex = i * recordsPerThread;
				int endIndex = (i + 1) * recordsPerThread;
				if (recordsCount - endIndex < recordsPerThread)
					endIndex = recordsCount;
				AggregatorThreadArguments threadArguments = new AggregatorThreadArguments() { CalculatedCache = calculatedCache, StartIndex = startIndex, EndIndex = endIndex };
				Thread thread = new Thread(IterateThroughDataThreadTask);
				thread.Start(threadArguments);
				calculatedCachesPerThread.Add(calculatedCache);
				threads.Add(thread);
			}
			foreach (Thread thread in threads)
				thread.Join();
			return MergeCaches(calculatedCachesPerThread);
		}
		PivotCalculatedCache MergeCaches(List<PivotCalculatedCache> calculatedCachesPerThread) {
			PivotCalculatedCache result = calculatedCachesPerThread[0];
			for (int i = 1; i < calculatedCachesPerThread.Count; i++)
				result.MergeWith(calculatedCachesPerThread[i]);
			return result;
		}
		#endregion
		void IterateThroughDataThreadTask(object argumentsObj) {
			AggregatorThreadArguments arguments = argumentsObj as AggregatorThreadArguments;
			if (arguments == null)
				throw new ArgumentException("IterateThroughDataThreadTask called with invalid arguments");
			for (int i = arguments.StartIndex; i < arguments.EndIndex; i++)
				ProcessRecord(i, arguments.CalculatedCache);
		}
		void ProcessRecord(int i, PivotCalculatedCache calculatedCache) {
			IPivotCacheRecord record = PivotCache.Records[i];
			if (CheckPageFilters(record)) {
				PivotDataKey recordDataKey = GenerateKey(record, i);
				calculatedCache.Add(recordDataKey, record);
			}
		}
		bool CheckPageFilters(IPivotCacheRecord record) {
			return Transaction.FieldItemsVisibility.FilterRecordByPageFields(record);
		}
		PivotDataKey GenerateKey(IPivotCacheRecord record, int index) {
			int count = keyFieldIndices.Count;
			int[] keyValueSharedItemIndices = new int[count];
			for (int i = 0; i < count; i++)
				keyValueSharedItemIndices[i] = PivotTable.Cache.CacheFields.GetSharedItemIndex(record, keyFieldIndices[i]);
			return new PivotDataKey(keyValueSharedItemIndices);
		}
	}
	#endregion
	#region PivotCalculatedCache
	public class PivotCalculatedCache {
		readonly PivotTable pivotTable;
		readonly Dictionary<PivotDataKey, PivotCellCalculationInfo> cellValues;
		readonly Dictionary<PivotDataKey, PivotCellCalculationInfo> subtotals;
		readonly int dataFieldsCount = 0;
		AxisGrandTotals rowGrandTotals;
		AxisGrandTotals columnGrandTotals;
		private List<int> keyFieldIndices;
		public PivotCalculatedCache(PivotTable pivotTable, List<int> keyFieldIndices) {
			this.pivotTable = pivotTable;
			this.keyFieldIndices = keyFieldIndices;
			this.cellValues = new Dictionary<PivotDataKey, PivotCellCalculationInfo>();
			this.subtotals = new Dictionary<PivotDataKey, PivotCellCalculationInfo>();
			dataFieldsCount = pivotTable.DataFields.Count;
		}
		#region GetInfo
		internal PivotCellCalculationInfo GetCellInfo(PivotDataKey key) {
			PivotCellCalculationInfo result = null;
			cellValues.TryGetValue(key, out result);
			return result;
		}
		internal PivotCellCalculationInfo GetSubtotalInfo(PivotDataKey key) {
			PivotCellCalculationInfo result = null;
			subtotals.TryGetValue(key, out result);
			return result;
		}
		public PivotCacheHasKeyResponse HasSubtotalInfo(PivotDataKey key) {
			PivotCellCalculationInfo info = null;
			if (subtotals.TryGetValue(key, out info))
				return info.PassedFilters ? PivotCacheHasKeyResponse.Yes : PivotCacheHasKeyResponse.Filtered;
			return PivotCacheHasKeyResponse.No;
		}
		#endregion
		public void Add(PivotDataKey recordDataKey, IPivotCacheRecord record) {
			PivotCellCalculationInfo info = null;
			if (!cellValues.TryGetValue(recordDataKey, out info)) {
						info = new PivotCellCalculationInfo(dataFieldsCount);
						cellValues.Add(recordDataKey, info);
			}
			info.AddValue(record, pivotTable);
		}
		internal PivotCacheHasKeyResponse HasRowKeySequence(List<int> key) {
			int[] sharedItemsIndices = new int[keyFieldIndices.Count];
			for (int i = 0; i < key.Count; i++)
				sharedItemsIndices[i] = key[i];
			for (int i = key.Count; i < keyFieldIndices.Count; i++)
				sharedItemsIndices[i] = -1;
			PivotDataKey dataKey = new PivotDataKey(sharedItemsIndices);
			PivotCellCalculationInfo info = null;
			if (subtotals.TryGetValue(dataKey, out info))
				return info.PassedFilters ? PivotCacheHasKeyResponse.Yes : PivotCacheHasKeyResponse.Filtered;
			return PivotCacheHasKeyResponse.No;
		}
		internal PivotCacheHasKeyResponse HasColumnKeySequence(List<int> key) {
			int[] sharedItemsIndices = new int[keyFieldIndices.Count];
			int rowFieldsCount = pivotTable.RowFields.KeyIndicesCount;
			for (int i = 0; i < rowFieldsCount; i++)
				sharedItemsIndices[i] = -1;
			for (int i = 0; i < key.Count; i++)
				sharedItemsIndices[i + rowFieldsCount] = key[i];
			for (int i = key.Count + rowFieldsCount; i < keyFieldIndices.Count; i++)
				sharedItemsIndices[i] = -1;
			PivotDataKey dataKey = new PivotDataKey(sharedItemsIndices);
			PivotCellCalculationInfo info = null;
			if (subtotals.TryGetValue(dataKey, out info))
				return info.PassedFilters ? PivotCacheHasKeyResponse.Yes : PivotCacheHasKeyResponse.Filtered;
			return PivotCacheHasKeyResponse.No;
		}
		internal bool HasTableGrandTotal() {
			int[] key = PrepareInitialKey();
			return subtotals.ContainsKey(key);
		}
		public int[] PrepareInitialKey() {
			int keyFieldIndicesCount = keyFieldIndices.Count;
			int[] key = new int[keyFieldIndicesCount];
			for (int i = 0; i < keyFieldIndicesCount; i++)
				key[i] = -1; 
			return key;
		}
		public void CalculateSubtotals(IPivotTableTransaction transaction) {
			CalculateFilters(transaction);
			int rowFieldsCount = pivotTable.RowFields.KeyIndicesCount;
			int columnFieldsCount = keyFieldIndices.Count - rowFieldsCount;
			bool hasAxisWithNoFields = rowFieldsCount == 0 || columnFieldsCount == 0;
			subtotals.Clear();
			int keyFieldIndicesCount = keyFieldIndices.Count;
			int[] key = PrepareInitialKey();
			PivotDataKey grandTotalKey = new PivotDataKey((int[])key.Clone());
			PivotCellCalculationInfo grandTotal = new PivotCellCalculationInfo(pivotTable.DataFields.Count);
			bool hasValues = false;
			bool grandTotalsAlreadyCalculated = rowGrandTotals != null && columnGrandTotals != null;
			foreach (KeyValuePair<PivotDataKey, PivotCellCalculationInfo> pair in cellValues) {
				PivotDataKey dataKey = pair.Key;
				PivotCellCalculationInfo cellValue = pair.Value;
				bool enabled;
				if (grandTotalsAlreadyCalculated)
					enabled = CheckDataKeyGrandTotals(dataKey);
				else
					enabled = transaction.FieldItemsVisibility.FilterDataKey(dataKey);
				AddSubtotalsRecursive(key, 0, dataKey, keyFieldIndicesCount, cellValue, enabled, true);
				if (hasAxisWithNoFields)
					AddSubtotalValue(dataKey.Items, cellValue, enabled); 
				if (enabled) {
					grandTotal.AddValueGroup(cellValue, true);
					hasValues = true;
				}
			}
			if (hasValues)
				subtotals.Add(grandTotalKey, grandTotal);
			ClearGrandTotals();
		}
		bool CheckDataKeyGrandTotals(PivotDataKey dataKey) {
			int rowFieldsCount = pivotTable.RowFields.KeyIndicesCount;
			bool rowResult = CheckAxisGrandTotals(0, rowFieldsCount, rowGrandTotals, dataKey);
			bool columnResult = CheckAxisGrandTotals(rowFieldsCount, keyFieldIndices.Count - rowFieldsCount, columnGrandTotals, dataKey);
			return rowResult && columnResult;
		}
		void ClearGrandTotals() {
			this.rowGrandTotals = null;
			this.columnGrandTotals = null;
		}
		#region Filters
		void CalculateFilters(IPivotTableTransaction transaction) {
			CalculateLabelFitlers(transaction);
			ApplyMeasureFilters(transaction);
		}
		#region CalculateLabelFitlers
		void CalculateLabelFitlers(IPivotTableTransaction transaction) {
			transaction.FieldItemsVisibility.ApplyHiddenItemsAndLabelFilters();
		}
		#endregion
		#region MeasureFilters
		void ApplyMeasureFilters(IPivotTableTransaction transaction) {
			bool hasFilters = false;
			foreach (PivotFilter filter in pivotTable.Filters) {
				if (filter.IsMeasureFilter) {
					if (hasFilters)
						CalculateGrandTotalsOnExisting(transaction);
					else
						CalculateGrandTotalsFirstTime(transaction);
					hasFilters = true;
					ApplyMeasureFilter(filter);
				}
			}
		}
		bool ApplyMeasureFilter(PivotFilter filter) {
			PivotField field = pivotTable.Fields[filter.FieldIndex];
			AxisGrandTotals grandTotals;
			int index;
			if (field.Axis == PivotTableAxis.Row) {
				grandTotals = rowGrandTotals;
				index = pivotTable.RowFields.GetIndexElementByFieldIndex(filter.FieldIndex);
			}
			else {
				grandTotals = columnGrandTotals;
				index = pivotTable.ColumnFields.GetIndexElementByFieldIndex(filter.FieldIndex);
			}
			if (index < 0)
				return false;
			AxisGrandTotalsLevel grandTotalsLevel = grandTotals[index];
			filter.ApplyFilter(pivotTable, grandTotalsLevel);
			return true;
		}
		#endregion
		#endregion
		bool CheckAxisGrandTotals(int startIndex, int fieldsCount, AxisGrandTotals grandTotals, PivotDataKey dataKey) {
			int[] keyIndices = new int[fieldsCount];
			Array.Copy(dataKey.Items, startIndex, keyIndices, 0, fieldsCount);
			for (int i = fieldsCount - 1; i >= 0; i--) {
				PivotCellCalculationInfo existingInfo = null;
				PivotDataKey key = new PivotDataKey(keyIndices);
				AxisGrandTotalsLevel currentLevel = grandTotals[i];
				if (currentLevel.TryGetValue(keyIndices, out existingInfo)) {
					if (!existingInfo.PassedFilters)
						return false;
				}
				else
					throw new InvalidOperationException("Internal exception");
				keyIndices[i] = -1;
			}
			return true;
		}
		#region GenerateGrandTotals
		void CalculateGrandTotalsFirstTime(IPivotTableTransaction transaction) {
			int rowFieldsCount = pivotTable.RowFields.KeyIndicesCount;
			int columnFieldsCount = pivotTable.ColumnFields.KeyIndicesCount;
			rowGrandTotals = new AxisGrandTotals(rowFieldsCount);
			columnGrandTotals = new AxisGrandTotals(columnFieldsCount);
			foreach (KeyValuePair<PivotDataKey, PivotCellCalculationInfo> pair in cellValues) {
				PivotCellCalculationInfo cellValue = pair.Value;
				bool passedFilters = transaction.FieldItemsVisibility.FilterDataKey(pair.Key);
				GenerateAxisGrandTotals(0, rowFieldsCount, rowGrandTotals, pair, passedFilters);
				GenerateAxisGrandTotals(rowFieldsCount, keyFieldIndices.Count - rowFieldsCount, columnGrandTotals, pair, passedFilters);
			}
		}
		void GenerateAxisGrandTotals(int startIndex, int fieldsCount, AxisGrandTotals grandTotals, KeyValuePair<PivotDataKey, PivotCellCalculationInfo> pair, bool passedFilters) {
			PivotDataKey dataKey = pair.Key;
			PivotCellCalculationInfo cellValue = pair.Value;
			int[] keyIndices = new int[fieldsCount];
			Array.Copy(dataKey.Items, startIndex, keyIndices, 0, fieldsCount);
			for (int i = fieldsCount - 1; i >= 0; i--) {
				PivotCellCalculationInfo existingInfo = null;
				PivotDataKey key = new PivotDataKey((int[])keyIndices.Clone());
				AxisGrandTotalsLevel currentLevel = grandTotals[i];
				if (!currentLevel.TryGetValue(key, out existingInfo)) {
					existingInfo = new PivotCellCalculationInfo(pivotTable.DataFields.Count);
					currentLevel.Add(key, existingInfo);
				}
				existingInfo.AddValueGroup(cellValue, passedFilters);
				keyIndices[i] = -1;
			}
		}
		void CalculateGrandTotalsOnExisting(IPivotTableTransaction transaction) {
			int rowFieldsCount = pivotTable.RowFields.KeyIndicesCount;
			int columnFieldsCount = pivotTable.ColumnFields.KeyIndicesCount;
			AxisGrandTotals newRowGrandTotals = new AxisGrandTotals(rowFieldsCount);
			AxisGrandTotals newColumnGrandTotals = new AxisGrandTotals(columnFieldsCount);
			foreach (KeyValuePair<PivotDataKey, PivotCellCalculationInfo> pair in cellValues) {
				PivotCellCalculationInfo cellValue = pair.Value;
				bool passedFilters = CheckDataKeyGrandTotals(pair.Key);
				GenerateAxisGrandTotals(0, rowFieldsCount, newRowGrandTotals, pair, passedFilters);
				GenerateAxisGrandTotals(rowFieldsCount, keyFieldIndices.Count - rowFieldsCount, newColumnGrandTotals, pair, passedFilters);
			}
			this.rowGrandTotals = newRowGrandTotals;
			this.columnGrandTotals = newColumnGrandTotals;
		}
		#endregion
		void AddSubtotalsRecursive(int[] key, int position, PivotDataKey dataKey, int keyFieldIndicesCount, PivotCellCalculationInfo cellValue, bool enabled, bool allKeyEntered) {
			if (position >= keyFieldIndicesCount)
				return;
			key[position] = dataKey[position];
			if (position < key.Length - 1 || !allKeyEntered)
				AddSubtotalValue(key, cellValue, enabled);
			AddSubtotalsRecursive(key, position + 1, dataKey, keyFieldIndicesCount, cellValue, enabled, allKeyEntered);
			key[position] = -1;
			AddSubtotalsRecursive(key, position + 1, dataKey, keyFieldIndicesCount, cellValue, enabled, false);
		}
		void AddSubtotalValue(int[] key, PivotCellCalculationInfo cellValue, bool enabled) {
			PivotDataKey subtotalKey = (int[])key.Clone();
			PivotCellCalculationInfo existingInfo = null;
			if (!subtotals.TryGetValue(subtotalKey, out existingInfo)) {
				existingInfo = new PivotCellCalculationInfo(pivotTable.DataFields.Count);
				subtotals.Add(subtotalKey, existingInfo);
			}
			existingInfo.AddValueGroup(cellValue, enabled);
		}
		public void AddDataFieldsSubtotal(PivotCellCalculationInfo grandTotal) {
			PivotDataKey grandTotalKey = new PivotDataKey(new int[0]);
			subtotals.Add(grandTotalKey, grandTotal);
		}
		internal void MergeWith(PivotCalculatedCache pivotCalculatedCache) {
			foreach (KeyValuePair<PivotDataKey, PivotCellCalculationInfo> cellValue in pivotCalculatedCache.cellValues) {
				PivotCellCalculationInfo info = null;
				if (!cellValues.TryGetValue(cellValue.Key, out info))
					cellValues.Add(cellValue.Key, cellValue.Value);
				else
					info.AddValueGroup(cellValue.Value);
			}
		}
	}
	#endregion
	#region PivotCacheHasKeyResponse
	public enum PivotCacheHasKeyResponse {
		No,
		Yes,
		Filtered
	}
	#endregion
	#region PivotCellCalculationInfo
	public class PivotCellCalculationInfo {
		bool passedFilters = false;
		PivotCellDataFieldCalculationInfo[] dataFieldCalcInfos;
		public PivotCellCalculationInfo(int dataFieldsCount) {
			dataFieldCalcInfos = new PivotCellDataFieldCalculationInfo[dataFieldsCount];
			for (int i = 0; i < dataFieldsCount; i++) {
				dataFieldCalcInfos[i] = new PivotCellDataFieldCalculationInfo();
			}
		}
		public PivotCellDataFieldCalculationInfo[] DataFieldInfos { get { return dataFieldCalcInfos; } }
		public bool PassedFilters { get { return passedFilters; } set { passedFilters = value; } }
		internal void AddValue(IPivotCacheRecord record, PivotTable pivotTable) {
			for (int i = 0; i < pivotTable.DataFields.Count; i++) {
				IPivotCacheRecordValue value = pivotTable.Cache.CacheFields.GetValue(record, pivotTable.DataFields[i].FieldIndex);
				dataFieldCalcInfos[i].AddValue(value);
			}
		}
		internal void AddValueGroup(PivotCellCalculationInfo cellValue, bool passedFilters) {
			if (!passedFilters)
				return;
			Debug.Assert(cellValue.dataFieldCalcInfos.Length == dataFieldCalcInfos.Length);
			for (int i = 0; i < dataFieldCalcInfos.Length; i++)
				dataFieldCalcInfos[i].AddValueGroup(cellValue.dataFieldCalcInfos[i]);
			this.passedFilters |= passedFilters;
		}
		internal void AddValueGroup(PivotCellCalculationInfo cellValue) {
			Debug.Assert(cellValue.dataFieldCalcInfos.Length == dataFieldCalcInfos.Length);
			for (int i = 0; i < dataFieldCalcInfos.Length; i++)
				dataFieldCalcInfos[i].AddValueGroup(cellValue.dataFieldCalcInfos[i]);
		}
		internal VariantValue GetValue(int dataFieldIndex, PivotDataConsolidateFunction function) {
			return this.dataFieldCalcInfos[dataFieldIndex].GetValue(function);
		}
	}
	#endregion
	#region PivotCellDataFieldCalculationInfo
	public class PivotCellDataFieldCalculationInfo : IPivotCacheRecordValueVisitor {
		volatile ICellError errorValue = null;
		volatile int count = 0;
		volatile int countNumbers = 0;
		double sum = 0;
		double min = double.MaxValue;
		double max = double.MinValue;
		double product = 1;
		double variance = 0;
		public void AddValueGroup(PivotCellDataFieldCalculationInfo dataFieldCalcInfo) {
			if (this.errorValue == null) {
				this.errorValue = dataFieldCalcInfo.errorValue;
				if (dataFieldCalcInfo.countNumbers > 0) {
					if (this.countNumbers == 0)
						this.variance = dataFieldCalcInfo.variance;
					else {
						int countSum = (this.countNumbers + dataFieldCalcInfo.countNumbers);
						double totalSum = (this.sum + dataFieldCalcInfo.sum);
						double groupAverage = totalSum / countSum;
						double x = Math.Pow(((this.sum / this.countNumbers) - groupAverage), 2) * this.countNumbers;
						double y = Math.Pow(((dataFieldCalcInfo.sum / dataFieldCalcInfo.countNumbers) - groupAverage), 2) * dataFieldCalcInfo.countNumbers;
						this.variance = (((x + y) / countSum) + ((this.variance + dataFieldCalcInfo.variance) / countSum)) * countSum;
					}
					if (this.min > dataFieldCalcInfo.min)
						this.min = dataFieldCalcInfo.min;
					if (this.max < dataFieldCalcInfo.max)
						this.max = dataFieldCalcInfo.max;
					this.sum += dataFieldCalcInfo.sum;
					this.product *= dataFieldCalcInfo.product;
				}
			}
			this.count += dataFieldCalcInfo.count;
			this.countNumbers += dataFieldCalcInfo.countNumbers;
		}
		public void AddValue(IPivotCacheRecordValue dataFieldValue) {
			dataFieldValue.Visit(this);
		}
		public void Visit(PivotCacheRecordSharedItemsIndexValue indexValue) {
			throw new InvalidOperationException("");
		}
		public void Visit(PivotCacheRecordEmptyValue value) {
		}
		public void Visit(PivotCacheRecordBooleanValue value) {
			count++;
		}
		public void Visit(PivotCacheRecordCharacterValue value) {
			count++;
		}
		public void Visit(PivotCacheRecordErrorValue value) {
			count++;
			errorValue = value.Value;
		}
		public void Visit(PivotCacheRecordDateTimeValue value) {
			VisitNumeric(WorkbookDataContext.ToDateTimeSerialDouble(value.Value, DateSystem.Date1900));
		}
		public void Visit(PivotCacheRecordNumericValue value) {
			VisitNumeric(value.Value);
		}
		public void Visit(PivotCacheRecordOrdinalEmptyValue value) {
		}
		public void Visit(PivotCacheRecordOrdinalBooleanValue value) {
			count++;
		}
		public void Visit(PivotCacheRecordOrdinalCharacterValue value) {
			count++;
		}
		public void Visit(PivotCacheRecordOrdinalErrorValue value) {
			count++;
			errorValue = value.Value;
		}
		public void Visit(PivotCacheRecordOrdinalDateTimeValue value) {
			VisitNumeric(WorkbookDataContext.ToDateTimeSerialDouble(value.Value, DateSystem.Date1900));
		}
		public void Visit(PivotCacheRecordOrdinalNumericValue value) {
			VisitNumeric(value.Value);
		}
		void VisitNumeric(double value) {
			++count;
			++countNumbers;
			if (errorValue == null) {
				double oldMean = 0;
				if (countNumbers > 1)
					oldMean = sum / (countNumbers - 1);
				if (value > max)
					max = value;
				if (value < min)
					min = value;
				sum += value;
				product *= value;
				variance += (value - oldMean) * (value - (sum / countNumbers));
			}
		}
		public VariantValue GetValue(PivotDataConsolidateFunction function) {
			if (count == 0)
				return VariantValue.Missing;
			if (function == PivotDataConsolidateFunction.Count)
				return count;
			if (function == PivotDataConsolidateFunction.CountNums)
				return countNumbers;
			if (errorValue != null)
				return errorValue.Value;
			if (countNumbers == 0)
				switch (function) {
					case PivotDataConsolidateFunction.Sum:
					case PivotDataConsolidateFunction.Max:
					case PivotDataConsolidateFunction.Min:
					case PivotDataConsolidateFunction.Product:
						return 0;
					default:
						return VariantValue.ErrorDivisionByZero;
				}
			switch (function) {
				case PivotDataConsolidateFunction.Sum:
					return sum;
				case PivotDataConsolidateFunction.Max:
					return max;
				case PivotDataConsolidateFunction.Min:
					return min;
				case PivotDataConsolidateFunction.Product:
					return product;
				case PivotDataConsolidateFunction.Average:
					return sum / countNumbers;
				case PivotDataConsolidateFunction.Varp:
					return variance / countNumbers;
				case PivotDataConsolidateFunction.StdDevp:
					return Math.Sqrt(variance / countNumbers);
				case PivotDataConsolidateFunction.Var:
					return countNumbers <= 1 ? VariantValue.ErrorDivisionByZero : variance / (countNumbers - 1);
				case PivotDataConsolidateFunction.StdDev:
					return countNumbers <= 1 ? VariantValue.ErrorDivisionByZero : Math.Sqrt(variance / (countNumbers - 1));
				default:
					throw new ArgumentException("Not implemeted");
			}
		}
	}
	#endregion
	public class AxisGrandTotals {
		readonly AxisGrandTotalsLevel[] levels;
		public AxisGrandTotals(int levelCount) {
			this.levels = new AxisGrandTotalsLevel[levelCount];
			for (int i = 0; i < levelCount; i++) {
				levels[i] = new AxisGrandTotalsLevel();
			}
		}
		public AxisGrandTotalsLevel this[int index] { get { return levels[index]; } }
	}
	public class AxisGrandTotalsLevel : Dictionary<PivotDataKey, PivotCellCalculationInfo> {
	}
}
