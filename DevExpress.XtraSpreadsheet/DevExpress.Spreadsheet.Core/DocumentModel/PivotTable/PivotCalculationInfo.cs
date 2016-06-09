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
	#region PivotTableOutOfDateState
	public enum PivotTableOutOfDateState {
		None = 0,				   
		CalculatedCache = 7,		
		CalculatedCacheMask = 4,	
		Layout = 3,				 
		LayoutMask = 2,			 
		WorksheetData = 1,		  
		WorksheetDataMask = 1,	  
	}
	#endregion
	#region PivotCalculationInfo
	public class PivotCalculationInfo : IPivotStyleOwner {
		#region Fields
		readonly PivotTable pivotTable;
		readonly PivotLayoutItems columnItems;
		readonly PivotLayoutItems rowItems;
		readonly PivotStyleFormatCache styleFormatCache;
		readonly IPivotLayoutItemAccessor rowLayoutItemAccessor;
		readonly IPivotLayoutItemAccessor columnLayoutItemAccessor;
		int transactionCounter = 0;
		PivotCalculatedCache calculatedCache;
		PivotTableOutOfDateState state;
		IPivotTableTransaction transaction;
		PivotZoneFormattingCache pivotZoneCache;
		bool manualUpdate;
		#endregion
		public PivotCalculationInfo(PivotTable pivotTable) {
			this.pivotTable = pivotTable;
			this.columnItems = new PivotLayoutItems();
			this.rowItems = new PivotLayoutItems();
			this.styleFormatCache = new PivotStyleFormatCache(this);
			this.State = PivotTableOutOfDateState.None;
			this.rowLayoutItemAccessor = new PivotLayoutItemRowAccessor(pivotTable);
			this.columnLayoutItemAccessor = new PivotLayoutItemColumnAccessor(pivotTable);
			pivotTable.WholeRangeChanged += OnPivotTableWholeRangeChanged;
		}
		#region Properties
		public DocumentModel DocumentModel { get { return pivotTable.DocumentModel; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		public PivotLayoutItems ColumnItems { get { return columnItems; } }
		public PivotLayoutItems RowItems { get { return rowItems; } }
		public bool ManualUpdate { get { return manualUpdate; } set { manualUpdate = value; } }
		#region CalculatedCache
		public PivotCalculatedCache CalculatedCache { get { return calculatedCache; } }
		internal void SetCalculatedCacheCore(PivotCalculatedCache value) {
			this.calculatedCache = value;
		}
		#endregion
		public IPivotLayoutItemAccessor RowLayoutItemAccessor { get { return rowLayoutItemAccessor; } }
		public IPivotLayoutItemAccessor ColumnLayoutItemAccessor { get { return columnLayoutItemAccessor; } }
		public PivotTableOutOfDateState State { get { return state; } set { state = value; } }
		#region StyleFormatCache
		public PivotStyleFormatCache StyleFormatCache {
			get {
				PivotTableStyleInfo styleInfo = pivotTable.StyleInfo;
				if (styleInfo.ApplyTableStyle && !styleFormatCache.IsValid) {
					TableStyleElementInfoCache styleCache = styleInfo.Style.Cache;
					if (!styleCache.IsEmpty)
						styleFormatCache.Prepare(styleCache);
				}
				return styleFormatCache;
			}
		}
		#endregion
		public IPivotTableTransaction Transaction { get { return transaction; } }
		public bool HasActiveTransaction { get { return transactionCounter > 0; } }
		#endregion
		#region IPivotStyleOwner members
		IPivotStyleOptions IPivotStyleOwner.Options { get { return pivotTable.StyleInfo; } }
		PivotZoneFormattingCache IPivotStyleOwner.PivotZoneCache { get { return pivotZoneCache; } set { pivotZoneCache = value; } }
		IPivotTableLocation IPivotStyleOwner.Location { get { return pivotTable.Location; } }
		int IPivotStyleOwner.ColumnFieldsCount { get { return pivotTable.ColumnFields.Count; } }
		int IPivotStyleOwner.RowFieldsCount { get { return pivotTable.RowFields.Count; } }
		int IPivotStyleOwner.DataFieldsCount { get { return pivotTable.DataFields.Count; } }
		int IPivotStyleOwner.PageFieldsCount { get { return pivotTable.PageFields.Count; } }
		void IPivotStyleOwner.CacheColumnItemStripeInfo(int columnItemIndex, TableStyleStripeInfo info) {
			columnItems[columnItemIndex].CachedStripeInfo = info;
		}
		int IPivotStyleOwner.GetDataColumnItemIndex(int absoluteDataIndex) {
			return pivotTable.Location.GetDataColumnItemIndex(absoluteDataIndex);
		}
		int IPivotStyleOwner.GetDataRowItemIndex(int absoluteDataIndex) {
			return pivotTable.Location.GetDataRowItemIndex(absoluteDataIndex);
		}
		TableStyleStripeInfo IPivotStyleOwner.GetColumnItemStripeInfo(int columnItemIndex) {
			return columnItems[columnItemIndex].CachedStripeInfo;
		}
		void IPivotStyleOwner.CacheRowItemStripeInfo(int rowItemIndex, TableStyleStripeInfo info) {
			rowItems[rowItemIndex].CachedStripeInfo = info;
		}
		TableStyleStripeInfo IPivotStyleOwner.GetRowItemStripeInfo(int rowItemIndex) {
			return rowItems[rowItemIndex].CachedStripeInfo;
		}
		IPivotZone IPivotStyleOwner.GetPivotZoneByCellPosition(CellPosition cellPosition) {
			return GetPivotZoneByCellPosition(cellPosition);
		}
		#endregion
		public void InvalidateStyleFormatCache() {
			styleFormatCache.SetInvalid();
		}
		public void InvalidateLayout() {
			OnPivotTableChanged(PivotTableOutOfDateState.Layout);
		}
		public void InvalidateCalculatedCache() {
			OnPivotTableChanged(PivotTableOutOfDateState.CalculatedCache);
		}
		public void InvalidateWorksheetData() {
			OnPivotTableChanged(PivotTableOutOfDateState.WorksheetData);
		}
		#region OnPivotCacheRefreshed
		internal bool OnPivotCacheRefreshed(PivotCache cache, IErrorHandler errorHandler, PivotTableCacheRefreshInfo refreshInfo) {
			if (!object.ReferenceEquals(cache, pivotTable.Cache))
				return true;
			pivotTable.BeginTransaction(errorHandler);
			UpdateFieldsOnCacheChanges(refreshInfo.NewToOldFieldIndices);
			UpdateFieldItemsOnCacheChanges(refreshInfo);
			OnPivotTableChanged(PivotTableOutOfDateState.CalculatedCache);
			return pivotTable.EndTransaction();
		}
		void UpdateFieldItemsOnCacheChanges(PivotTableCacheRefreshInfo refreshInfo) {
			for (int i = 0; i < pivotTable.Fields.Count; ++i) {
				PivotField field = pivotTable.Fields[i];
				if (field.Items.Count <= 0)				 
					continue;
				IPivotCacheField cacheField = pivotTable.Cache.CacheFields[i];
				int[] sharedItemIndicesConversionTable = refreshInfo.SharedItemIndicesConversionTables[i];
				bool[] alreadyExistingItemTable = refreshInfo.AlreadyExistingItemTables[i];
				int lastDataItemIndex = -1;
				for (int j = 0; j < field.Items.Count; j++) {
					PivotItem item = field.Items[j];
					if (item.IsDataItem)
						UpdatePivotItem(item, cacheField, sharedItemIndicesConversionTable[item.ItemIndex]);
					else {
						lastDataItemIndex = j - 1;
						break;
					}
				}
				bool hideNewItems = !field.IncludeNewItemsInFilter && PivotTable.FieldHasHiddenItems(i);
				for (int j = 0; j < alreadyExistingItemTable.Length; j++)
					if (!alreadyExistingItemTable[j]) {
						field.Items.Insert(lastDataItemIndex + 1, CreatePivotItem(i, j, cacheField, hideNewItems));
						lastDataItemIndex++;
					}
			}
		}
		public PivotItem CreatePivotItem(int fieldIndex, int sharedItemIndex, IPivotCacheField cacheField, bool hideNewItems) {
			IPivotCacheRecordValue sharedItemValue = cacheField.SharedItems[sharedItemIndex];
			PivotItem pivotItem = new PivotItem(PivotTable);
			pivotItem.SetItemIndexCore(sharedItemIndex);
			pivotItem.SetHasMissingValueCore(sharedItemValue.IsUnusedItem);
			pivotItem.SetCalculatedMemberCore(sharedItemValue.IsCalculatedItem);
			pivotItem.SetIsHiddenCore(hideNewItems);
			if (sharedItemValue.HasCaption)
				pivotItem.SetItemUserCaptionCore(sharedItemValue.Caption);
			return pivotItem;
		}
		void UpdatePivotItem(PivotItem item, IPivotCacheField cacheField, int newSharedItemIndex) {
			item.ItemIndex = newSharedItemIndex;
			IPivotCacheRecordValue sharedItemValue = cacheField.SharedItems[newSharedItemIndex];
			item.HasMissingValue = sharedItemValue.IsUnusedItem;
			item.CalculatedMember = sharedItemValue.IsCalculatedItem;
			if (sharedItemValue.HasCaption)
				item.SetItemUserCaptionCore(sharedItemValue.Caption);
		}
		void UpdateFieldsOnCacheChanges(Dictionary<int, int> newToOldFieldIndices) {
			Dictionary<int, PivotField> oldFields = new Dictionary<int, PivotField>();
			for (int k = 0; k < pivotTable.Fields.Count; ++k)
				oldFields.Add(k, pivotTable.Fields[k]);
			pivotTable.Fields.Clear();
			Dictionary<int, int> oldToNewFieldIndices = new Dictionary<int, int>();
			for (int newFieldIndex = 0; newFieldIndex < newToOldFieldIndices.Count; ++newFieldIndex) {
				PivotField field;
				int oldFieldIndex = newToOldFieldIndices[newFieldIndex];
				if (oldFieldIndex == -1)
					field = pivotTable.CreateField();
				else {
					oldToNewFieldIndices.Add(oldFieldIndex, newFieldIndex);
					field = oldFields[oldFieldIndex];
				}
				pivotTable.Fields.Add(field);
			}
			ChangeReferenceIndices(pivotTable.PageFields, oldToNewFieldIndices);
			ChangeReferenceIndices(pivotTable.DataFields, oldToNewFieldIndices);
			if (pivotTable.DataFields.Count >= 2)
				oldToNewFieldIndices.Add(PivotTable.ValuesFieldFakeIndex, PivotTable.ValuesFieldFakeIndex);
			ChangeReferenceIndices(pivotTable.RowFields, oldToNewFieldIndices);
			ChangeReferenceIndices(pivotTable.ColumnFields, oldToNewFieldIndices);
		}
		void ChangeReferenceIndices<T>(PivotFieldReferenceCollection<T> collection, Dictionary<int, int> oldToNewIndices) where T : PivotFieldReference {
			for (int i = 0; i < collection.Count; ) {
				PivotFieldReference reference = collection[i];
				int oldFieldIndex = reference.FieldIndex;
				int newFieldIndex;
				if (!oldToNewIndices.TryGetValue(oldFieldIndex, out newFieldIndex))
					collection.RemoveAt(i);
				else {
					if (oldFieldIndex != newFieldIndex)
						reference.OnFieldIndexChanged(DocumentModel, oldFieldIndex, newFieldIndex);
					++i;
				}
			}
		}
		#endregion
		void OnPivotTableChanged(PivotTableOutOfDateState state) {
			this.State |= state;
		}
		void OnPivotTableWholeRangeChanged(object sender, CellRangeBaseChangedEventArgs e) {
			InvalidateStyleFormatCache();
		}
		internal bool RefreshPivotTable() {
			if (ManualUpdate)
				return true;
			return RefreshPivotTableCore(State, Transaction);
		}
		public bool RefreshPivotTableCore(PivotTableOutOfDateState state, IPivotTableTransaction transaction) {
			pivotTable.RaiseFieldsChanged(); 
			PivotTable.CalculationInfo.InvalidateStyleFormatCache();
			PivotTable.LayoutCellCache.Invalidate(); 
			if (state == PivotTableOutOfDateState.None)
				return true;
			if (PivotTable.Cache.RefreshNeeded)
				if (transaction.ErrorHandler.HandleError(new ModelErrorInfo(ModelErrorType.PivotTableCanNotBeBuiltFromEmptyCache)) == ErrorHandlingResult.Abort)
					return false;
			System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
			if ((state & PivotTableOutOfDateState.CalculatedCacheMask) > 0) {
				PivotAggregateCommand command = new PivotAggregateCommand(transaction);
				if (!command.Execute())
					return false;
			}
			if ((state & PivotTableOutOfDateState.LayoutMask) > 0) {
				PivotCalculateLayoutCommand command = new PivotCalculateLayoutCommand(transaction);
				if (!command.Execute())
					return false;
			}
			if ((state & PivotTableOutOfDateState.WorksheetDataMask) > 0) {
				PivotRefreshDataOnWorksheetCommand command = new PivotRefreshDataOnWorksheetCommand(transaction);
				if (!command.Execute())
					return false;
			}
			pivotTable.CalculationInfo.State = PivotTableOutOfDateState.None;
			System.Diagnostics.Debug.WriteLine("PivotTable updated in " + stopWatch.ElapsedMilliseconds + " ms.");
			return true;
		}
		bool ExecuteCommand(PivotTableErrorHandledCommand command, PivotTableOutOfDateState state, IErrorHandler errorHandler) {
			pivotTable.BeginTransaction(errorHandler);
			try {
				if (command.Execute()) {
					OnPivotTableChanged(state);
					return true;
				}
				return false;
			}
			finally {
				pivotTable.EndTransaction();
			}
		}
		public bool AddFieldToKeyFields(int fieldIndex, PivotTableAxis axis, IErrorHandler errorHandler) {
			return ExecuteCommand(new PivotInsertFieldToKeyFieldsCommand(pivotTable, fieldIndex, axis, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public bool InsertFieldToKeyFields(int fieldIndex, PivotTableAxis axis, int insertIndex, IErrorHandler errorHandler) {
			return ExecuteCommand(new PivotInsertFieldToKeyFieldsCommand(pivotTable, fieldIndex, axis, insertIndex, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public bool AddDataField(int fieldIndex, string caption, IErrorHandler errorHandler) {
			return ExecuteCommand(new PivotInsertDataFieldCommand(pivotTable, fieldIndex, caption, pivotTable.DataFields.Count, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public bool AddDataField(int fieldIndex, string caption, PivotDataConsolidateFunction function, IErrorHandler errorHandler) {
			return ExecuteCommand(new PivotInsertDataFieldCommand(pivotTable, fieldIndex, caption, function, pivotTable.DataFields.Count, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public bool InsertDataField(int fieldIndex, string caption, int insertIndex, IErrorHandler errorHandler) {
			return ExecuteCommand(new PivotInsertDataFieldCommand(pivotTable, fieldIndex, caption, insertIndex, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public bool InsertDataField(int fieldIndex, string caption, PivotDataConsolidateFunction function, int insertIndex, IErrorHandler errorHandler) {
			return ExecuteCommand(new PivotInsertDataFieldCommand(pivotTable, fieldIndex, caption, function, insertIndex, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public void RemoveKeyField(int fieldReferenceIndex, PivotTableAxis axis, IErrorHandler errorHandler) {
			ExecuteCommand(new PivotRemoveKeyFieldCommand(pivotTable, fieldReferenceIndex, axis, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public void RemoveFieldFromKeyFields(int fieldIndex, IErrorHandler errorHandler) {
			ExecuteCommand(new PivotRemoveFieldFromKeyFieldsCommand(pivotTable, fieldIndex, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public void ClearKeyFields(PivotTableAxis axisType, IErrorHandler errorHandler) {
			ExecuteCommand(new PivotClearKeyFieldsCommand(pivotTable, axisType, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public void ClearAllKeyFields(IErrorHandler errorHandler) {
			ExecuteCommand(new PivotClearKeyFieldsCommand(pivotTable, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		public void MoveKeyField(PivotTableAxis sourceAxis, int sourceIndex, PivotTableAxis targetAxis, int targetIndex, IErrorHandler errorHandler) {
			ExecuteCommand(new PivotMoveFieldReferenceToTargetIndexCommand(pivotTable, sourceAxis, sourceIndex, targetAxis, targetIndex, errorHandler), PivotTableOutOfDateState.CalculatedCache, errorHandler);
		}
		#region Transaction
		internal void BeginUpdate(IErrorHandler errorHandler) {
			transactionCounter++;
			if (transactionCounter == 1) {
				this.transaction = new PivotTableTransaction(pivotTable, errorHandler);
				this.transaction.Init();
			}
		}
		internal bool EndUpdate() {
			transactionCounter--;
			if (transactionCounter <= 0) {
				System.Diagnostics.Debug.Assert(transactionCounter == 0);
				return EndUpdateCore();
			}
			return true;
		}
		bool EndUpdateCore() {
			PivotTableOutOfDateState startState = State;
			bool success = RefreshPivotTable();
			if (success)
				Transaction.Commit(startState);
			else
				Transaction.Rollback(startState);
			transaction = null;
			return success;
		}
		#endregion
		#region PivotZone
		public PivotZone GetPivotZoneByCellPosition(CellPosition cellPosition) {
			PivotZoneAxisInfo columnAxisInfo = CalculateAxisInfo(cellPosition, columnLayoutItemAccessor);
			PivotZoneAxisInfo rowAxisInfo = CalculateAxisInfo(cellPosition, rowLayoutItemAccessor);
			return new PivotZone(pivotTable, columnAxisInfo, rowAxisInfo);
		}
		PivotZoneAxisInfo CalculateAxisInfo(CellPosition cellPosition, IPivotLayoutItemAccessor layoutAccessor) {
			int itemIndex = layoutAccessor.GetActiveLayoutItemIndex(cellPosition);
			IPivotLayoutItem item = layoutAccessor.GetItemByIndex(itemIndex);
			if (item == null)
				return null;
			int index = layoutAccessor.GetOtherItemIndexByPositionIncludingHeaders(cellPosition);
			if (index < 0)
				return null;
			switch (item.Type) {
				case PivotFieldItemType.Grand:
					return GetGrandFormatting(layoutAccessor, index, item);
				case PivotFieldItemType.Blank:
					return GetBlankFormatting(layoutAccessor, index, item, itemIndex);
				case PivotFieldItemType.Data:
					return GetDataFormatting(layoutAccessor, index, item, itemIndex);
				default:
					return GetSubtotalFormatting(layoutAccessor, index, item, itemIndex);
			}
		}
		PivotZoneAxisInfo GetGrandFormatting(IPivotLayoutItemAccessor layoutAccessor, int index, IPivotLayoutItem item) {
			int otherRangeSize = layoutAccessor.OtherRangeSize;
			PivotZoneAxisInfo result = new PivotZoneAxisInfo(item);
			result.Formatting = PivotFormattingType.Grand;
			result.HasFirstCell = index == 0;
			result.HasLastCell = index == otherRangeSize - 1;
			result.IsVertical = layoutAccessor is PivotLayoutItemColumnAccessor;
			return result;
		}
		PivotZoneAxisInfo GetBlankFormatting(IPivotLayoutItemAccessor layoutAccessor, int index, IPivotLayoutItem item, int itemIndex) {
			return GetDataFormattingCore(layoutAccessor, index, item, itemIndex, true);
		}
		PivotZoneAxisInfo GetDataFormatting(IPivotLayoutItemAccessor layoutAccessor, int index, IPivotLayoutItem item, int itemIndex) {
			return GetDataFormattingCore(layoutAccessor, index, item, itemIndex, false);
		}
		PivotZoneAxisInfo GetSubtotalFormatting(IPivotLayoutItemAccessor layoutAccessor, int index, IPivotLayoutItem item, int itemIndex) {
			int startSubtotalIndex = System.Math.Min(item.RepeatedItemsCount, layoutAccessor.FirstDataItemIndex - 1);
			int lastFieldIndex = item.PivotFieldItemIndices.Length + item.RepeatedItemsCount - 1;
			int fieldCount = layoutAccessor.KeyFieldCount;
			PivotZoneAxisInfo result;
			if (index >= startSubtotalIndex) {
				result = new PivotZoneAxisInfo(item);
				result.SetSubTotalFormatting(fieldCount, lastFieldIndex + 1);
				result.FieldIndex = lastFieldIndex; 
				result.HasFirstCell = index <= startSubtotalIndex;
				result.HasLastCell = index == layoutAccessor.OtherRangeSize - 1;
				result.IsVertical = layoutAccessor is PivotLayoutItemColumnAccessor;
			}
			else
				result = GetSubheadingFormatting(layoutAccessor, item, index, itemIndex, false);
			return result;
		}
		PivotZoneAxisInfo GetDataFormattingCore(IPivotLayoutItemAccessor layoutAccessor, int index, IPivotLayoutItem item, int itemIndex, bool createBlank) {
			return GetSubheadingFormatting(layoutAccessor, item, index, itemIndex, createBlank);
		}
		PivotZoneAxisInfo GetSubheadingFormatting(IPivotLayoutItemAccessor layoutAccessor, IPivotLayoutItem item, int index, int itemIndex, bool createBlank) {
			PivotZoneAxisInfo result = new PivotZoneAxisInfo(item);
			PivotZoneCalculationInfo calcInfo = new PivotZoneCalculationInfo();
			calcInfo.FieldCount = layoutAccessor.KeyFieldCount;
			calcInfo.LastFieldIndex = item.PivotFieldItemIndices.Length + item.RepeatedItemsCount - 1;
			result.Formatting = PivotFormattingType.White;
			if (calcInfo.LastFieldIndex >= 0) {
				CalulateCurrentField(layoutAccessor, item, index, calcInfo);
				result.FieldIndex = calcInfo.FieldIndex;
			}
			result.IsVertical = GetIsVertical(layoutAccessor, calcInfo.FieldIsTabular);
			if (!calcInfo.FieldIsTabular && createBlank)
				PrepareNonTabularBlankFormatting(result, layoutAccessor, index);
			else {
				bool isOrdinalDataArea = CheckIsDataArea(calcInfo, createBlank, index, layoutAccessor.FirstDataItemIndex);
				if (isOrdinalDataArea)
					PrepareDataAreaFormatting(result, layoutAccessor, index, calcInfo, createBlank);
				else {
					PrepareHeaderFormatting(result, calcInfo, index);
					if (calcInfo.FieldIsTabular)
						PrepareBordersForTabularHeader(result, layoutAccessor, itemIndex, item, calcInfo.FieldIndex);
				}
			}
			return result;
		}
		private int CalulateCurrentField(IPivotLayoutItemAccessor layoutAccessor, IPivotLayoutItem item, int index, PivotZoneCalculationInfo calcInfo) {
			int visibleFieldsCount = calcInfo.FieldCount;
			PivotField field;
			int shift = 0;
			int currentPositionIndex = 0;
			int currentFieldIndex = -1;
			do {
				currentPositionIndex += shift;
				currentFieldIndex++;
				field = layoutAccessor.GetFieldByAxisKeyFieldIndex(currentFieldIndex, item.DataFieldIndex);
				shift = layoutAccessor.FieldIsOutline(field) ? 0 : 1;
				calcInfo.FieldIsTabular = layoutAccessor.FieldIsTabular(field);
				if (calcInfo.FieldIsTabular && currentFieldIndex >= item.RepeatedItemsCount) {
					int fieldItemIndex = item.PivotFieldItemIndices[currentFieldIndex - item.RepeatedItemsCount];
					if (field.Items.Count > fieldItemIndex) 
					calcInfo.FieldIsCollapsed = calcInfo.FieldIsTabular && field.Items[fieldItemIndex].HideDetails;
					visibleFieldsCount = currentFieldIndex + 1;
				}
			}
			while ((currentPositionIndex < index || shift == 0) && currentFieldIndex < calcInfo.LastFieldIndex && !calcInfo.FieldIsCollapsed);
			if (calcInfo.FieldIsCollapsed)
				calcInfo.FieldCount = visibleFieldsCount;
			calcInfo.PositionIndex = currentPositionIndex;
			calcInfo.FieldIndex = currentFieldIndex;
			return visibleFieldsCount;
		}
		bool CheckIsDataArea(PivotZoneCalculationInfo calcInfo, bool createBlank, int index, int firstDataItem) {
			bool result;
			if (calcInfo.FieldIsTabular) {
				if (calcInfo.FieldIsCollapsed && !createBlank)
					result = index > calcInfo.PositionIndex;
				else
					result = index >= calcInfo.LastFieldIndex;
			}
			else
				result = index >= firstDataItem - 1;
			return result;
		}
		void PrepareHeaderFormatting(PivotZoneAxisInfo info, PivotZoneCalculationInfo calcInfo, int index) {
			if (calcInfo.PositionIndex < index && calcInfo.FieldIndex < calcInfo.FieldCount && calcInfo.FieldIndex >= calcInfo.LastFieldIndex) {
				if (!calcInfo.FieldIsTabular)
					info.SetSubHeadingFormatting(calcInfo.FieldCount, calcInfo.LastFieldIndex + 1);
			}
			else {
				if (calcInfo.FieldIndex >= calcInfo.LastFieldIndex || calcInfo.FieldIsTabular)
					if (calcInfo.FieldIsCollapsed)
						info.SetSubHeadingFormatting(calcInfo.FieldCount + 1, calcInfo.PositionIndex + 1);
					else
						info.SetSubHeadingFormatting(calcInfo.FieldCount, calcInfo.PositionIndex + 1);
			}
			info.HasFirstCell = calcInfo.PositionIndex >= index;
			info.HasLastCell = calcInfo.FieldIndex < calcInfo.LastFieldIndex;
		}
		void PrepareBordersForTabularHeader(PivotZoneAxisInfo info, IPivotLayoutItemAccessor layoutAccessor, int itemIndex, IPivotLayoutItem item, int currentFieldIndex) {
			IPivotLayoutItem previousItem = layoutAccessor.GetItemByIndex(itemIndex - 1);
			IPivotLayoutItem nextItem = layoutAccessor.GetItemByIndex(itemIndex + 1);
			if (previousItem != null) {
				if (currentFieldIndex < item.RepeatedItemsCount)
					info.HasFirstCell = currentFieldIndex > previousItem.RepeatedItemsCount;
			}
			if (nextItem != null)
				info.HasLastCell = currentFieldIndex >= nextItem.RepeatedItemsCount;
		}
		void PrepareDataAreaFormatting(PivotZoneAxisInfo info, IPivotLayoutItemAccessor layoutAccessor, int index, PivotZoneCalculationInfo calcInfo, bool createBlank) {
			info.FieldIndex = calcInfo.LastFieldIndex;
			info.HasFirstCell = index == calcInfo.PositionIndex - 1 || index == 0;
			info.HasLastCell = index == layoutAccessor.OtherRangeSize - 1;
			if (createBlank)
				info.Formatting = PivotFormattingType.Blank;
			else
				info.SetSubHeadingFormatting(calcInfo.FieldCount, calcInfo.LastFieldIndex + 1);
		}
		void PrepareNonTabularBlankFormatting(PivotZoneAxisInfo info, IPivotLayoutItemAccessor layoutAccessor, int index) {
			info.Formatting = PivotFormattingType.Blank;
			info.HasFirstCell = index == 0;
			info.HasLastCell = index == layoutAccessor.OtherRangeSize - 1;
		}
		bool GetIsVertical(IPivotLayoutItemAccessor layoutAccessor, bool fieldIsTabular) {
			if (layoutAccessor is PivotLayoutItemColumnAccessor)
				return false;
			else
				return fieldIsTabular;
		}
		#endregion
		public int GetActiveFieldIndex(CellPosition cellPosition) {
			if (cellPosition.Row < pivotTable.Location.Range.TopRowIndex)
				return GetActivePageFieldInfo(cellPosition).FieldIndex;
			PivotZone zone = GetPivotZoneByCellPosition(cellPosition);
			return zone.GetActiveFieldIndex();
		}
		public PivotFieldZoneInfo GetActiveFieldInfo(CellPosition cellPosition) {
			if (cellPosition.Row < pivotTable.Location.Range.TopRowIndex)
				return GetActivePageFieldInfo(cellPosition);
			PivotZone zone = GetPivotZoneByCellPosition(cellPosition);
			return zone.GetActiveFieldInfo();
		}
		PivotFieldZoneInfo GetActivePageFieldInfo(CellPosition cellPosition) {
			int pageColumn = (cellPosition.Column - pivotTable.Location.Range.LeftColumnIndex) / 3;
			int pageRow = pivotTable.Location.RowPageCount - (pivotTable.Location.Range.TopRowIndex - cellPosition.Row - 1);
			int fieldReference;
			if (pivotTable.PageOverThenDown)
				fieldReference = pivotTable.Location.ColumnPageCount * pageRow + pageColumn;
			else
				fieldReference = pivotTable.Location.RowPageCount * pageColumn + pageRow;
			return new PivotFieldZoneInfo(PivotTableAxis.Page, pivotTable.PageFields[fieldReference], fieldReference);
		}
		public void CopyFromNoHistory(PivotCalculationInfo source) {
			if (transaction != null)
				Exceptions.ThrowInvalidOperationException("Can't copy from pivot table with active transaction.");
			columnItems.InnerList.AddRange(source.columnItems.InnerList);
			rowItems.InnerList.AddRange(source.rowItems.InnerList);
			InvalidateStyleFormatCache();
			InvalidateCalculatedCache();
		}
	}
	#endregion
	public class PivotZoneCalculationInfo {
		public bool FieldIsCollapsed { get; set; }
		public bool FieldIsTabular { get; set; }
		public int Index { get; set; }
		public int PositionIndex { get; set; }
		public int LastFieldIndex { get; set; }
		public int FieldCount { get; set; }
		public int FieldIndex { get; set; }
	}
	#region PivotLayoutCellCache
	public class PivotLayoutCellCache {
		readonly PivotTable pivotTable;
		Dictionary<CellPosition, PivotLayoutCellInfo> cache;
		public PivotLayoutCellCache(PivotTable pivotTable) {
			this.pivotTable = pivotTable;
			pivotTable.WholeRangeChanged += OnPivotTableWholeRangeChanged;
		}
		void OnPivotTableWholeRangeChanged(object sender, CellRangeBaseChangedEventArgs e) {
			Invalidate();
		}
		public Dictionary<CellPosition, PivotLayoutCellInfo>.Enumerator GetCellInfoEnumerator() {
			if (cache == null)
				Refresh();
			return cache.GetEnumerator();
		}
		public bool TryGetButtonInfo(CellPosition position, out PivotLayoutCellInfo info) {
			if (cache == null)
				Refresh();
			return cache.TryGetValue(position, out info);
		}
		public void Invalidate() {
			cache = null;
		}
		void Refresh() {
			cache = new Dictionary<CellPosition, PivotLayoutCellInfo>();
			PivotReportHeadersBuilder rowInfo = new PivotReportRowHeadersBuilder(pivotTable);
			PivotReportHeadersBuilder columnInfo = new PivotReportColumnHeadersBuilder(pivotTable);
			rowInfo.PrepareCache();
			columnInfo.PrepareCache();
			if (pivotTable.ShowDrill) {
				AddCollapseButtons(rowInfo);
				AddCollapseButtons(columnInfo);
			}
			FillLabels(rowInfo);
			FillLabels(columnInfo);
			PivotRefreshDataOnWorksheetCommand.FillPageHeaders(pivotTable, AddPageFilterInfo);
		}
		void AddCollapseButtons(PivotReportHeadersBuilder info) {
			for (int i = 0; i < info.ItemsCount; ++i) {
				IPivotLayoutItem layoutItem = info.GetItem(i);
				if (layoutItem.Type != PivotFieldItemType.Data)
					continue;
				int columnIndex = 0;
				bool prevIsCompact = false;
				for (int k = 0; k < layoutItem.RepeatedItemsCount; ++k)
					if (!info.CompactFormList[k]) {
						if (info.Fields[k] != PivotTable.ValuesFieldFakeIndex)
							AddIndentInfo(info, columnIndex, i);
						++columnIndex;
						prevIsCompact = false;
					}
					else
						prevIsCompact = true;
				int lastFieldWithButton = info.Fields.LastKeyFieldIndex - 1;
				for (int j = layoutItem.RepeatedItemsCount; j < layoutItem.RepeatedItemsCount + layoutItem.PivotFieldItemIndices.Length; ++j) {
					if (j > lastFieldWithButton) {
						if (prevIsCompact)
							AddIndentInfo(info, columnIndex, i);
					}
					else {
						int fieldIndex = info.Fields[j];
						if (fieldIndex == PivotTable.ValuesFieldFakeIndex) {
							if (prevIsCompact)
								AddIndentInfo(info, columnIndex, i);
						}
						else {
							int itemIndex = layoutItem.PivotFieldItemIndices[j - layoutItem.RepeatedItemsCount];
							if (itemIndex == PivotTableLayoutCalculator.LastFieldSubtotalsItemIndex)
								AddIndentInfo(info, columnIndex, i);
							else
								AddCollapseInfo(info, columnIndex, i, fieldIndex, itemIndex);
						}
					}
					if (!info.CompactFormList[j]) {
						++columnIndex;
						prevIsCompact = false;
					}
					else
						prevIsCompact = true;
				}
			}
		}
		void FillLabels(PivotReportHeadersBuilder info) {
			if (info.LabelFieldIndices.Count == 0)
				return;
			if (info.LabelFieldIndices[0] == PivotReportHeadersBuilder.DefaultLabelCaptionIndex) {
				int fieldIndex = -1;
				if (info.Fields.KeyIndicesCount == 1)
					fieldIndex = info.Fields[info.Fields.LastKeyFieldIndex];
				AddLabelFilterInfo(info, 0, fieldIndex);
				return;
			}
			for (int i = 0; i < info.LabelFieldIndices.Count; ++i) {
				int fieldIndex = info.LabelFieldIndices[i];
				if (fieldIndex == PivotTable.ValuesFieldFakeIndex)
					continue;
				AddLabelFilterInfo(info, i, fieldIndex);
			}
		}
		void AddCollapseInfo(PivotReportHeadersBuilder info, int column, int row, int fieldIndex, int itemIndex) {
			CellPosition position = info.GetHeaderPosition(column, row);
			cache.Add(position, PivotLayoutCellInfo.CreateCollapseButtonInfo(fieldIndex, itemIndex));
		}
		void AddIndentInfo(PivotReportHeadersBuilder info, int column, int row) {
			CellPosition position = info.GetHeaderPosition(column, row);
			cache.Add(position, PivotLayoutCellInfo.CreateIndentInfo());
		}
		void AddLabelFilterInfo(PivotReportHeadersBuilder info, int labelIndex, int fieldIndex) {
			CellPosition position = new CellPosition(info.GetLabelColumnIndex(labelIndex), info.GetLabelRowIndex());
			cache.Add(position, PivotLayoutCellInfo.CreateLabelsFilterButtonInfo(fieldIndex, !info.IsColumn));
		}
		void AddPageFilterInfo(int sheetColumn, int sheetRow, int pageIndex) {
			int fieldIndex = pivotTable.PageFields[pageIndex].FieldIndex;
			CellPosition position = new CellPosition(sheetColumn + 1, sheetRow);
			cache.Add(position, PivotLayoutCellInfo.CreatePageFilterButtonInfo(fieldIndex));
		}
		public void CopyFromNoHistory(CellPositionOffset offset, PivotLayoutCellCache source) {
			Invalidate();
			if (source.cache == null)
				return;
			cache = new Dictionary<CellPosition, PivotLayoutCellInfo>(source.cache.Count);
			foreach (KeyValuePair<CellPosition, PivotLayoutCellInfo> pair in source.cache)
				cache.Add(pair.Key.GetShiftedAny(offset, pivotTable.Worksheet), pair.Value);
		}
	}
	public class PivotLayoutCellInfo {
		public static PivotLayoutCellInfo CreateIndentInfo() {
			return new PivotLayoutCellInfo(PivotLayouCellInfoType.Indent, -1, -1);
		}
		public static PivotLayoutCellInfo CreateCollapseButtonInfo(int fieldIndex, int itemIndex) {
			return new PivotLayoutCellInfo(PivotLayouCellInfoType.CollapseButton | PivotLayouCellInfoType.Indent, fieldIndex, itemIndex);
		}
		public static PivotLayoutCellInfo CreateLabelsFilterButtonInfo(int fieldIndex, bool rowFieldsFilter) {
			PivotLayouCellInfoType type = PivotLayouCellInfoType.LabelFilterButton;
			if (rowFieldsFilter)
				type |= PivotLayouCellInfoType.RowFieldsFilter;
			return new PivotLayoutCellInfo(type, fieldIndex, -1);
		}
		public static PivotLayoutCellInfo CreatePageFilterButtonInfo(int fieldIndex) {
			return new PivotLayoutCellInfo(PivotLayouCellInfoType.PageFilterButton, fieldIndex, -1);
		}
		int fieldIndex;
		int itemIndex;
		PivotLayouCellInfoType type;
		PivotLayoutCellInfo(PivotLayouCellInfoType type, int fieldIndex, int itemIndex) {
			this.type = type;
			this.fieldIndex = fieldIndex;
			this.itemIndex = itemIndex;
		}
		public bool Indent { get { return (type & PivotLayouCellInfoType.Indent) > 0; } }
		public bool FilterButton { get { return (type & (PivotLayouCellInfoType.LabelFilterButton | PivotLayouCellInfoType.PageFilterButton)) > 0; } }
		public bool CollapseButton { get { return (type & PivotLayouCellInfoType.CollapseButton) > 0; } }
		public bool LabelFilterButton { get { return (type & PivotLayouCellInfoType.LabelFilterButton) > 0; } }
		public bool PageFilterButton { get { return (type & PivotLayouCellInfoType.PageFilterButton) > 0; } }
		public bool RowFieldsFilter { get { return (type & PivotLayouCellInfoType.RowFieldsFilter) > 0; } }
		public int FieldIndex { get { return fieldIndex; } } 
		public int ItemIndex { get { return itemIndex; } }
	}
	public enum PivotLayouCellInfoType {
		Indent = 0x1,
		CollapseButton = 0x2,
		LabelFilterButton = 0x4,
		PageFilterButton = 0x8,
		RowFieldsFilter = 0x10
	}
	#endregion
}
