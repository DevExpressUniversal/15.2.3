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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotTableLayoutCalculator
	public class PivotTableLayoutCalculator {
		#region Fields
		readonly IPivotTableTransaction transaction;
		public const int LastFieldSubtotalsItemIndex = 1048832;
		public const int PreviousItem = 1048828;
		public const int NextItem = 1048829;
		#endregion
		public PivotTableLayoutCalculator(IPivotTableTransaction transaction) {
			this.transaction = transaction;
		}
		#region Properties
		public PivotTable PivotTable { get { return Transaction.PivotTable; } }
		public IPivotTableTransaction Transaction { get { return transaction; } }
		int DataFieldCount { get { return PivotTable.DataFields.Count; } }
		#endregion
		public void Calculate() {
			bool tableHasGrandTotals = PivotTable.CalculatedCache.HasTableGrandTotal();
			PivotTableAggregationProcessData processData = new PivotTableAggregationProcessData(PivotTable, PivotTable.CalculationInfo.RowLayoutItemAccessor, tableHasGrandTotals, 0);
			PrepareAxis(processData);
			processData = new PivotTableAggregationProcessData(PivotTable, PivotTable.CalculationInfo.ColumnLayoutItemAccessor, tableHasGrandTotals, transaction.PivotTable.RowFields.KeyIndicesCount);
			PrepareAxis(processData);
		}
		void PrepareAxis(PivotTableAggregationProcessData processData) {
			if (processData.AxisBuilder.HasFields) {
				PrepareAxisRecursive(processData);
				processData.LayoutBuilder.Flush();
				AddLastFieldSubtotals(processData);
			}
			else
				CreateAxisWithoutFields(processData);
				AddGrandTotals(processData);
		}
		void PrepareAxisRecursive(PivotTableAggregationProcessData processData) {
			try {
				processData.CurrentFieldReferenceIndex++;
				if (processData.CurrentFieldReferenceIndex >= processData.AxisBuilder.KeyFieldCount)
					return;
				processData.CurrentFieldIndex = processData.AxisBuilder.GetFieldIndex(processData.CurrentFieldReferenceIndex);
				if (processData.CurrentFieldIndex == PivotTable.ValuesFieldFakeIndex)
					AddDataItems(processData);
				else {
					processData.CurrentKeyFieldIndex++;
					PrepareAxisRecursiveCore(processData);
					processData.CurrentKeyFieldIndex--;
				}
			}
			finally {
				processData.CurrentFieldReferenceIndex--;
			}
		}
		void AddDataItems(PivotTableAggregationProcessData processData) {
			int startDataFieldIndex = processData.DataFieldIndex;
			bool previousSimpleSubtotal = processData.SimpleSubtotal;
			processData.SimpleSubtotal = true;
			try {
				for (int dataFieldIndex = 0; dataFieldIndex < DataFieldCount; dataFieldIndex++) {
					processData.LayoutBuilder.AddItem(PivotTable.OutlineData, PivotFieldItemType.Data, dataFieldIndex, processData.CurrentFieldReferenceIndex, dataFieldIndex);
					processData.DataFieldIndex = dataFieldIndex;
					PrepareAxisRecursive(processData);
				}
			}
			finally {
				processData.SimpleSubtotal = previousSimpleSubtotal;
				processData.DataFieldIndex = startDataFieldIndex;
			}
		}
		void PrepareAxisRecursiveCore(PivotTableAggregationProcessData processData) {
			PivotField field = PivotTable.Fields[processData.CurrentFieldIndex];
			int lastDataItemIndex = field.Items.DataItemsCount - 1;
			IPivotLayoutItemAccessor axisBuilder = processData.AxisBuilder;
			int[] currentKey = processData.CurrentKey;
			int currentFieldReferenceIndex = processData.CurrentFieldReferenceIndex;
			bool isNotLastFieldReference = currentFieldReferenceIndex < processData.LastKeyFieldIndex;
			for (int itemIndex = 0; itemIndex <= lastDataItemIndex; itemIndex++) {
				PivotItem currentItem = field.Items[itemIndex];
				currentKey[processData.CurrentKeyFieldIndex] = currentItem.ItemIndex;
				if (ShouldSkipItem(currentKey, currentItem, field))
					continue;
				processData.LayoutBuilder.AddItem(field.Outline, currentItem.ItemType, itemIndex, currentFieldReferenceIndex, processData.DataFieldIndex);
				bool hideDetails = currentItem.HideDetails;
				if (hideDetails) {
					if (currentFieldReferenceIndex < axisBuilder.KeyFieldCount - 1)
						processData.HasCollapsedItems = true;
				}
				else {
					PrepareAxisRecursive(processData);
					if (isNotLastFieldReference)
						AddSubtotals(processData, currentFieldReferenceIndex, field, itemIndex);
				}
				if (isNotLastFieldReference)
					AddBlankLine(processData, currentFieldReferenceIndex, field, itemIndex);
			}
			currentKey[processData.CurrentKeyFieldIndex] = -1;
		}
		bool ShouldSkipItem(PivotDataKey currentKey, PivotItem currentItem, PivotField field) {
			if (currentItem.IsHidden)
				return true;
			PivotCacheHasKeyResponse cacheHasDataResponse = PivotTable.CalculatedCache.HasSubtotalInfo(currentKey);
			if (cacheHasDataResponse == PivotCacheHasKeyResponse.Filtered || (cacheHasDataResponse == PivotCacheHasKeyResponse.No && !field.ShowItemsWithNoData))
				return true;
			return false;
		}
		void AddSubtotals(PivotTableAggregationProcessData processData, int currentFieldReferenceIndex, PivotField field, int fieldItemIndex) {
			int fieldItemsCount = field.Items.Count;
			int lastDataItemIndex = field.Items.DataItemsCount - 1;
			int subTotalCount = fieldItemsCount - lastDataItemIndex - 1;
			IPivotReportLayoutFormBuilder layoutBuilder = processData.LayoutBuilder;
			if (processData.SimpleSubtotal) {
				if (layoutBuilder.AllowsSubtotalAtTheTopOfEachGroup(field) && field.SubtotalTop && subTotalCount < 2)
					return; 
				for (int fieldItemSubtotalIndex = lastDataItemIndex + 1; fieldItemSubtotalIndex < fieldItemsCount; fieldItemSubtotalIndex++) {
					PivotItem currentItem = field.Items[fieldItemSubtotalIndex];
					layoutBuilder.AddItem(field.Outline, currentItem.ItemType, fieldItemIndex, currentFieldReferenceIndex, processData.DataFieldIndex);
				}
			}
			else {
				for (int fieldItemSubtotalIndex = lastDataItemIndex + 1; fieldItemSubtotalIndex < fieldItemsCount; fieldItemSubtotalIndex++) {
					PivotItem currentItem = field.Items[fieldItemSubtotalIndex];
					for (int dataFieldIndex = 0; dataFieldIndex < DataFieldCount; dataFieldIndex++)
						layoutBuilder.AddItem(field.Outline, currentItem.ItemType, fieldItemIndex, currentFieldReferenceIndex, dataFieldIndex);
				}
			}
		}
		void AddLastFieldSubtotals(PivotTableAggregationProcessData processData) {
			IPivotLayoutItemAccessor axisBuilder = processData.AxisBuilder;
			if (processData.HasCollapsedItems)
				return;
			if (axisBuilder.KeyFieldCount < 2)
				return;
			int fieldReferenceIndex = axisBuilder.FieldIndices[axisBuilder.FieldIndices.Count - 1];
			if (fieldReferenceIndex == PivotTable.ValuesFieldFakeIndex)
				return;
			PivotField field = PivotTable.Fields[fieldReferenceIndex];
			int lastDataItemIndex = field.Items.DataItemsCount - 1;
			if (lastDataItemIndex == field.Items.Count - 1)
				return;
			int dataFieldsCount = processData.SimpleSubtotal ? 1 : DataFieldCount;
			int[] key = new int[axisBuilder.FieldIndices.Count];
			for (int i = 0; i < key.Length - 1; ++i)
				key[i] = LastFieldSubtotalsItemIndex;
			PivotItem firstSubtotalItem = field.Items[lastDataItemIndex + 1];
			if ((firstSubtotalItem.ItemType & (PivotFieldItemType.SubtotalMask & ~PivotFieldItemType.DefaultValue)) == 0)
				return;
			axisBuilder.AddLayoutItem(key, 0, 0, firstSubtotalItem.ItemType);
			for (int i = 0; i <= lastDataItemIndex; ++i)
				for (int j = lastDataItemIndex + 1; j < field.Items.Count; ++j) {
					PivotItem subtotalItem = field.Items[j];
					if ((subtotalItem.ItemType & (PivotFieldItemType.SubtotalMask & ~PivotFieldItemType.DefaultValue)) == 0)
						break;
					for (int dataFieldIndex = 0; dataFieldIndex < dataFieldsCount; ++dataFieldIndex) {
						if (i == 0 && j == lastDataItemIndex + 1 && dataFieldIndex == 0)
							continue;
						axisBuilder.AddLayoutItem(new int[] { i }, axisBuilder.FieldIndices.Count - 1, dataFieldIndex, subtotalItem.ItemType);
					}
				}
		}
		void AddGrandTotals(PivotTableAggregationProcessData processData) {
			IPivotLayoutItemAccessor axisBuilder = processData.AxisBuilder;
			if (!axisBuilder.AxisHasGrandTotals || !axisBuilder.FieldIndices.HasKeyField)
				return;
			int grandTotalCount = axisBuilder.FieldIndices.HasValuesField ? DataFieldCount : 1;
			for (int dataFieldIndex = 0; dataFieldIndex < grandTotalCount; dataFieldIndex++)
				axisBuilder.AddLayoutItem(new int[] { 0 }, 0, dataFieldIndex, PivotFieldItemType.Grand);
		}
		void AddBlankLine(PivotTableAggregationProcessData processData, int currentFieldReferenceIndex, PivotField field, int fieldItemIndex) {
			if (field.InsertBlankRow && processData.AxisBuilder.SupportsBlankRow)
				processData.LayoutBuilder.AddItem(field.Outline, PivotFieldItemType.Blank, fieldItemIndex, currentFieldReferenceIndex, processData.DataFieldIndex);
		}
		void CreateAxisWithoutFields(PivotTableAggregationProcessData processData) {
			if (processData.AxisBuilder.OtherAxisHasFields || DataFieldCount > 0)
				processData.AxisBuilder.AddLayoutItem(new int[] { }, 0, 0, PivotFieldItemType.Data);
		}
	}
	#endregion
	#region IPivotReportLayoutFormBuilder
	public interface IPivotReportLayoutFormBuilder {
		void AddItem(bool isOutline, PivotFieldItemType itemType, int pivotFieldItemIndex, int currentLevel, int dataFieldIndex);
		bool AllowsSubtotalAtTheTopOfEachGroup(PivotField field);
		void Flush();
	}
	#endregion
	#region PivotReportLayoutTabularFormBuilder
	public class PivotReportLayoutTabularFormBuilder : IPivotReportLayoutFormBuilder {
		#region Fields
		readonly IPivotLayoutItemAccessor axisBuilder;
		IPivotLayoutItem pendingItem;
		int previousLevel = -1;
		PivotFieldItemType lastItemType = PivotFieldItemType.Data;
		#endregion
		public PivotReportLayoutTabularFormBuilder(IPivotLayoutItemAccessor axisBuilder) {
			this.axisBuilder = axisBuilder;
		}
		public virtual bool AllowsSubtotalAtTheTopOfEachGroup(PivotField field) {
			return false;
		}
		public virtual void AddItem(bool isOutline, PivotFieldItemType itemType, int pivotFieldItemIndex, int currentLevel, int dataFieldIndex) {
			int repeatedItemsCount = currentLevel;
			if (pendingItem != null) {
				if (itemType == PivotFieldItemType.Data && dataFieldIndex == pendingItem.DataFieldIndex && currentLevel >= previousLevel) {
					if (currentLevel > previousLevel) {
						int pendingItemItemIndicesLength = pendingItem.PivotFieldItemIndices.Length;
						int[] resizedArray = pendingItem.PivotFieldItemIndices;
						Array.Resize<int>(ref resizedArray, pendingItemItemIndicesLength + 1);
						resizedArray[pendingItemItemIndicesLength] = pivotFieldItemIndex;
						pendingItem = PivotLayoutItemFactory.CreateInstance(resizedArray, pendingItem.RepeatedItemsCount, pendingItem.DataFieldIndex, pendingItem.Type);
					}
					else {
						AddItem(pendingItem);
						pendingItem = PivotLayoutItemFactory.CreateInstance(new int[] { pivotFieldItemIndex }, repeatedItemsCount, pendingItem.DataFieldIndex, pendingItem.Type);
					}
					previousLevel = currentLevel;
					return;
				}
				else
					Flush();
			}
			IPivotLayoutItem column = PivotLayoutItemFactory.CreateInstance(new int[] { pivotFieldItemIndex }, repeatedItemsCount, dataFieldIndex, itemType);
			if (itemType == PivotFieldItemType.Data)
				pendingItem = column;
			else
				AddItem(column);
			previousLevel = currentLevel;
		}
		void AddItem(IPivotLayoutItem item) {
			if (item.Type != PivotFieldItemType.Blank || lastItemType != PivotFieldItemType.Blank)
				axisBuilder.AddLayoutItem(item.PivotFieldItemIndices, item.RepeatedItemsCount, item.DataFieldIndex, item.Type);
			lastItemType = item.Type;
		}
		public void Flush() {
			if (pendingItem != null) {
				AddItem(pendingItem);
				pendingItem = null;
			}
		}
	}
	#endregion
	#region PivotReportLayoutAutoFormBuilder
	public class PivotReportLayoutAutoFormBuilder : PivotReportLayoutTabularFormBuilder {
		public PivotReportLayoutAutoFormBuilder(IPivotLayoutItemAccessor axisBuilder)
			: base(axisBuilder) {
		}
		public override bool AllowsSubtotalAtTheTopOfEachGroup(PivotField field) {
			return field.Outline;
		}
		public override void AddItem(bool isOutline, PivotFieldItemType itemType, int pivotFieldItemIndex, int currentLevel, int dataFieldIndex) {
			base.AddItem(isOutline, itemType, pivotFieldItemIndex, currentLevel, dataFieldIndex);
			if (isOutline)
				Flush();
		}
	}
	#endregion
	#region PivotTableFieldItemsVisibilityData
	public class PivotTableFieldItemsVisibilityData : IEnumerable<PivotItemVisibility> {
		readonly PivotItemCollection items;
		readonly List<bool> fieldItemsVisibilityCache;
		readonly List<bool> sharedItemsVisibilityCache;
		public PivotTableFieldItemsVisibilityData(PivotItemCollection items) {
			this.items = items;
			this.sharedItemsVisibilityCache = new List<bool>();
			this.fieldItemsVisibilityCache = new List<bool>(items.Count);
			Init(items);
		}
		public bool this[int itemIndex] { get { return fieldItemsVisibilityCache[itemIndex]; } }
		public int Count { get { return items.Count; } }
		void Init(PivotItemCollection items) {
			for (int i = 0; i < items.Count; i++)
				sharedItemsVisibilityCache.Add(true);
			foreach (PivotItem item in items) {
				bool visible = !item.IsHidden || !item.IsDataItem;
				fieldItemsVisibilityCache.Add(visible);
				if (item.IsDataItem)
					sharedItemsVisibilityCache[item.ItemIndex] = visible;
			}
		}
		public bool CheckItemBySharedItemIndex(int sharedItemIndex) {
			return sharedItemsVisibilityCache[sharedItemIndex];
		}
		internal void HideAllItems() {
			for (int i = 0; i < fieldItemsVisibilityCache.Count; i++) {
				fieldItemsVisibilityCache[i] = false;
				sharedItemsVisibilityCache[i] = false;
			}
		}
		internal void Clear() {
			for (int i = 0; i < fieldItemsVisibilityCache.Count; i++) {
				fieldItemsVisibilityCache[i] = true;
				sharedItemsVisibilityCache[i] = true;
			}
		}
		internal void SetItemVisibility(int itemIndex, int sharedItemIndex, bool value) {
			fieldItemsVisibilityCache[itemIndex] = value;
			sharedItemsVisibilityCache[sharedItemIndex] = value;
		}
		#region IEnumerable<PivotItemVisibility> Members
		public IEnumerator<PivotItemVisibility> GetEnumerator() {
			for (int i = 0; i < fieldItemsVisibilityCache.Count; i++) {
				yield return new PivotItemVisibility(items[i], fieldItemsVisibilityCache[i]);
			}
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((PivotTableFieldItemsVisibilityData)this).GetEnumerator();
		}
		#endregion
		internal PivotItemVisibility GetItemVisibility(int itemIndex) {
			return new PivotItemVisibility(items[itemIndex], fieldItemsVisibilityCache[itemIndex]);
		}
	}
	#endregion
	#region PivotTableFieldItemsVisibility
	public class PivotTableFieldItemsVisibility {
		readonly PivotTable pivotTable;
		readonly List<PivotTableFieldItemsVisibilityData> fieldItemsVisibility;
		readonly List<PivotTableFieldItemsVisibilityData> pageFieldItemsVisibility;
		List<int> keyFieldIndices;
		List<int> pageFieldIndices;
		public PivotTableFieldItemsVisibility(PivotTable pivotTable) {
			this.pivotTable = pivotTable;
			fieldItemsVisibility = new List<PivotTableFieldItemsVisibilityData>();
			pageFieldItemsVisibility = new List<PivotTableFieldItemsVisibilityData>();
		}
		internal List<PivotTableFieldItemsVisibilityData> FieldItemsVisibility { get { return fieldItemsVisibility; } }
		public void ApplyPageFields() {
			if (pivotTable.SubtotalHiddenItems)
				return;
			pageFieldIndices = new List<int>(pivotTable.PageFields.Count);
			foreach (PivotPageField pageField in pivotTable.PageFields) {
				int pageFieldFieldIndex = pageField.FieldIndex;
				pageFieldIndices.Add(pageFieldFieldIndex);
				PivotTableFieldItemsVisibilityData visibilityData = new PivotTableFieldItemsVisibilityData(pivotTable.Fields[pageFieldFieldIndex].Items);
				pageFieldItemsVisibility.Add(visibilityData);
				if (pageField.ItemIndex >= 0) {
					int pageFieldItemIndex = pageField.ItemIndex;
					PivotField field = pivotTable.Fields[pageFieldFieldIndex];
					visibilityData.HideAllItems();
					visibilityData.SetItemVisibility(pageFieldItemIndex, field.Items[pageFieldItemIndex].ItemIndex, true);
				}
			}
		}
		public void ApplyHiddenItemsAndLabelFilters() {
			keyFieldIndices = pivotTable.GetKeyFieldIndices();
			for (int i = 0; i < keyFieldIndices.Count; i++) {
				PivotField field = pivotTable.Fields[keyFieldIndices[i]];
				PivotTableFieldItemsVisibilityData visibilityData = new PivotTableFieldItemsVisibilityData(field.Items);
				fieldItemsVisibility.Add(visibilityData);
			}
			ApplyLabelFilters();
		}
		void ApplyLabelFilters() {
			Dictionary<int, int> fieldIndexToKeyFieldIndexTranslationTable = new Dictionary<int, int>();
			for (int i = 0; i < keyFieldIndices.Count; i++) {
				fieldIndexToKeyFieldIndexTranslationTable.Add(keyFieldIndices[i], i);
			}
			foreach (PivotFilter filter in pivotTable.Filters) {
				if (filter.IsLabelFilter) {
					int keyFieldIndex = -1;
					if (fieldIndexToKeyFieldIndexTranslationTable.TryGetValue(filter.FieldIndex, out keyFieldIndex)) {
						PivotTableFieldItemsVisibilityData visibilityData = FieldItemsVisibility[keyFieldIndex];
						filter.ApplyFilter(pivotTable, visibilityData);
					}
				}
			}
		}
		public bool FilterRecordByPageFields(IPivotCacheRecord record) {
			if (pageFieldIndices == null)
				return true;
			for (int i = 0; i < pageFieldIndices.Count; i++) {
				int sharedItemIndex = pivotTable.Cache.CacheFields.GetSharedItemIndex(record, pageFieldIndices[i]);
				if (!pageFieldItemsVisibility[i].CheckItemBySharedItemIndex(sharedItemIndex))
					return false;
			}
			return true;
		}
		internal bool FilterDataKey(PivotDataKey dataKey) {
			for (int i = 0; i < keyFieldIndices.Count; i++) {
				int sharedItemIndex = dataKey[i];
				if (sharedItemIndex == -1)
					continue;
				if (!fieldItemsVisibility[i].CheckItemBySharedItemIndex(sharedItemIndex))
					return false;
			}
			return true;
		}
	}
	#endregion
	#region PivotItemVisibility
	public struct PivotItemVisibility : IEquatable<PivotItemVisibility> {
		#region Field
		readonly PivotItem item;
		readonly bool value;
		#endregion
		public PivotItemVisibility(PivotItem item, bool value) {
			this.item = item;
			this.value = value;
		}
		#region Properties
		public PivotItem Item { get { return item; } }
		public bool Visible { get { return value; } }
		#endregion
		public override bool Equals(object obj) {
			if (!(obj is PivotItemVisibility))
				return false;
			PivotItemVisibility other = (PivotItemVisibility)obj;
			return value == other.value && item.Equals(other.item);
		}
		public bool Equals(PivotItemVisibility other) {
			return value == other.value && item.Equals(other.item);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(value.GetHashCode(), item.GetHashCode());
		}
		public override string ToString() {
			return base.ToString();
		}
	}
	#endregion
	#region PivotTableTransaction
	public interface IPivotTableTransaction {
		IErrorHandler ErrorHandler { get; }
		PivotTableFieldItemsVisibility FieldItemsVisibility { get; }
		PivotTable PivotTable { get; }
		bool SuppressRefreshDataOnWorksheetValidation { get; set; }
		void InitPageFieldsVisibilityData();
		void InitFiltersVisibilityData();
		void Init();
		void Commit(PivotTableOutOfDateState startState);
		void Rollback(PivotTableOutOfDateState startState);
		List<int> GetKeyFieldIndices();
	}
	public class PivotTableTransaction : IPivotTableTransaction {
		#region Fields
		readonly IErrorHandler errorHandler;
		readonly PivotTable pivotTable;
		readonly PivotTableFieldItemsVisibility fieldItemsVisibility;
		#endregion
		public PivotTableTransaction(PivotTable pivotTable, IErrorHandler errorHandler) {
			Guard.ArgumentNotNull(pivotTable, "pivotTable");
			Guard.ArgumentNotNull(errorHandler, "errorHandler");
			this.pivotTable = pivotTable;
			this.errorHandler = errorHandler;
			this.fieldItemsVisibility = new PivotTableFieldItemsVisibility(pivotTable);
		}
		#region Properties
		public IErrorHandler ErrorHandler { get { return errorHandler; } }
		public PivotTableFieldItemsVisibility FieldItemsVisibility { get { return fieldItemsVisibility; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		public bool SuppressRefreshDataOnWorksheetValidation { get; set; }
		#endregion
		public void InitPageFieldsVisibilityData() {
			FieldItemsVisibility.ApplyPageFields();
		}
		public void InitFiltersVisibilityData() {
			FieldItemsVisibility.ApplyHiddenItemsAndLabelFilters();
		}
		public void Init() {
		}
		public void Commit(PivotTableOutOfDateState startState) {
			if (startState != PivotTableOutOfDateState.None) {
				PivotTableStateHistoryItem historyItem = new PivotTableStateHistoryItem(PivotTable, startState);
				pivotTable.DocumentModel.History.Add(historyItem);
			}
		}
		public void Rollback(PivotTableOutOfDateState startState) {
			DevExpress.Office.History.DocumentHistory history = pivotTable.DocumentModel.History;
			if (history.Transaction != null)
				history.Transaction.Rollback();
			bool refreshResult = pivotTable.CalculationInfo.RefreshPivotTableCore(startState & ~PivotTableOutOfDateState.WorksheetDataMask, new PivotTableHistoryTransaction(pivotTable));
			System.Diagnostics.Debug.Assert(refreshResult);
		}
		public List<int> GetKeyFieldIndices() {
			return pivotTable.GetKeyFieldIndices();
		}
	}
	public class PivotTableHistoryTransaction : IPivotTableTransaction {
		readonly IErrorHandler errorHandler;
		readonly PivotTable pivotTable;
		readonly PivotTableFieldItemsVisibility fieldItemsVisibility;
		public PivotTableHistoryTransaction(PivotTable pivotTable) {
			Guard.ArgumentNotNull(pivotTable, "pivotTable");
			this.pivotTable = pivotTable;
			this.errorHandler = new AsserttingErrorHandler();
			this.fieldItemsVisibility = new PivotTableFieldItemsVisibility(pivotTable);
		}
		public IErrorHandler ErrorHandler { get { return errorHandler; } }
		public PivotTableFieldItemsVisibility FieldItemsVisibility { get { return fieldItemsVisibility; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		public bool SuppressRefreshDataOnWorksheetValidation { get { return false; } set { } }
		public void InitPageFieldsVisibilityData() {
			FieldItemsVisibility.ApplyPageFields();
		}
		public void InitFiltersVisibilityData() {
			FieldItemsVisibility.ApplyHiddenItemsAndLabelFilters();
		}
		public void Init() {
		}
		public void Commit(PivotTableOutOfDateState startState) {
		}
		public void Rollback(PivotTableOutOfDateState startState) {
		}
		public List<int> GetKeyFieldIndices() {
			return pivotTable.GetKeyFieldIndices();
		}
	}
	#endregion
	#region PivotTableAggregationProcessData
	public class PivotTableAggregationProcessData {
		readonly IPivotLayoutItemAccessor axisBuilder;
		readonly IPivotReportLayoutFormBuilder layoutBuilder;
		readonly bool tableHasGrandTotals;
		readonly int lastKeyFieldIndex;
		bool simpleSubtotal;
		int[] currentKey;
		public PivotTableAggregationProcessData(PivotTable pivotTable, IPivotLayoutItemAccessor axisBuilder, bool tableHasGrandTotals, int startKeyIndex) {
			this.axisBuilder = axisBuilder;
			this.tableHasGrandTotals = tableHasGrandTotals;
			currentKey = pivotTable.CalculatedCache.PrepareInitialKey();
			layoutBuilder = axisBuilder.GetInitialLayoutBuilder();
			this.lastKeyFieldIndex = axisBuilder.FieldIndices.LastKeyFieldIndex;
			this.simpleSubtotal = !axisBuilder.FieldIndices.HasValuesField;
			CurrentFieldReferenceIndex = -1;
			CurrentKeyFieldIndex = startKeyIndex - 1;
		}
		public IPivotLayoutItemAccessor AxisBuilder { get { return axisBuilder; } }
		public IPivotReportLayoutFormBuilder LayoutBuilder { get { return layoutBuilder; } }
		public bool SimpleSubtotal { get { return simpleSubtotal; } set { simpleSubtotal = value; } }
		public bool TableHasGrandTotals { get { return tableHasGrandTotals; } }
		public int LastKeyFieldIndex { get { return lastKeyFieldIndex; } }
		public int[] CurrentKey { get { return currentKey; } }
		public int CurrentFieldIndex { get; set; }
		public int DataFieldIndex { get; set; }
		public bool HasCollapsedItems { get; set; }
		public int CurrentFieldReferenceIndex { get; set; }
		public int CurrentKeyFieldIndex { get; set; } 
	}
	#endregion
}
