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
using System.IO;
using System.Linq;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.Utils;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Data.IO;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotVisualItemsBase : ICloneable, IDisposable, IThreadSafeAccessible, ICalculationContext<PivotFieldValueItem, int>, ICalculationSource<PivotFieldValueItem, int> {
		[Flags]
		protected enum PivotVisualItemsState : byte {
			Normal = 0,
			Ready = 1,
			ReadyFieldItems = 2,
			ReadOnly = 4,
			Disposed = 8,
			Filled = Ready | ReadyFieldItems
		}
		readonly PivotGridData data;
		PivotFieldValueItemsCreator columnItemsCreator;
		PivotFieldValueItemsCreator rowItemsCreator;
		PivotGridCellDataProviderBase cellDataProvider;
		PivotGridCellDataProviderBase emptyCellsDataProvider;
		PivotGridCellStreamDataProvider streamDataProvider;
		WeakEventHandler<EventArgs, ItemsEmptyEventHandler> cleared;
		WeakEventHandler<EventArgs, EventHandler> beforeCalculating;
		WeakEventHandler<EventArgs, EventHandler> afterCalculating;
		PivotVisualItemsState state;
		int rowTreeWidth;
		int rowMaxExpandedLevel;
		PivotGridFieldBase rowTreeField;
		PivotFieldItemCollection fieldItems;
		public PivotVisualItemsBase(PivotGridData data) {
			this.data = data;
			this.fieldItems = new PivotFieldItemCollection();
			this.state = PivotVisualItemsState.Normal;
		}
		~PivotVisualItemsBase() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			cleared = null;
			Clear();
			IsDisposed = true;
		}
		protected internal PivotFieldItemCollection FieldItems {
			get { return fieldItems; }
		}
		protected internal virtual PivotFieldItemBase CreateFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, PivotGridFieldBase field) {
			return new PivotFieldItemBase(data, groupItems, field);
		}
		public PivotGridData Data {
			get { return data; }
		}
		protected PivotFieldValueItemsCreator ColumnItemsCreator {
			get {
				if(columnItemsCreator == null)
					columnItemsCreator = CreateColumnItemsCreator();
				return columnItemsCreator;
			}
		}
		protected virtual PivotFieldValueItemsCreator RowItemsCreator {
			get {
				if(rowItemsCreator == null)
					rowItemsCreator = CreateRowItemsCreator();
				return rowItemsCreator;
			}
		}
		public PivotGridCellDataProviderBase CellDataProvider {
			get {
				if(IsReadOnly)
					return EmptyCellsDataProvider;
				return GetCellDataProvider();
			}
		}
		PivotGridCellDataProviderBase GetCellDataProvider() {
			if(StreamDataProvider != null)
				return StreamDataProvider;
			if(cellDataProvider == null)
				cellDataProvider = CreateCellDataProvider();
			return cellDataProvider;
		}
		protected PivotGridCellDataProviderBase EmptyCellsDataProvider {
			get {
				if(emptyCellsDataProvider == null)
					emptyCellsDataProvider = new PivotGridEmptyCellsDataProvider(Data, GetCellDataProvider());
				return emptyCellsDataProvider;
			}
		}
		protected PivotGridCellStreamDataProvider StreamDataProvider {
			get { return streamDataProvider; }
			set { streamDataProvider = value; }
		}
		protected PivotVisualItemsState State {
			get { return state; }
		}
		void SetState(PivotVisualItemsState state, bool included) {
			if(included) {
				this.state |= state;
			} else {
				this.state &= ~state;
			}
		}
		protected bool IsDisposed {
			get { return (state & PivotVisualItemsState.Disposed) != 0; }
			private set { SetState(PivotVisualItemsState.Disposed, value); }
		}
		public bool IsReadOnly {
			get { return (state & PivotVisualItemsState.ReadOnly) != 0; }
			protected internal set { SetState(PivotVisualItemsState.ReadOnly, value); }
		}
		public bool IsReady {
			get { return (state & PivotVisualItemsState.Ready) != 0; }
			private set { SetState(PivotVisualItemsState.Ready, value); }
		}
		public bool IsReadyFieldItems {
			get { return (state & PivotVisualItemsState.ReadyFieldItems) != 0; }
			private set { SetState(PivotVisualItemsState.ReadyFieldItems, value); }
		}
		protected void EnsureIsReady() {
			IsReady = ColumnCount != 0 && RowCount != 0;
		}
		internal void Invalidate() {
			IsReadyFieldItems = false;
			IsReady = false;
		}
		public int ColumnCount { get { return GetLastLevelItemCount(true); } }
		public int RowCount { get { return GetLastLevelItemCount(false); } }
		public virtual int RowTreeLevelOffset {
			get { return Data.OptionsView.RowTreeOffset; }
		}
		public virtual int RowTreeLevelWidth {
			get { return Data.OptionsView.RowTreeWidth; }
			set { Data.OptionsView.RowTreeWidth = value; }
		}
		public int RowTreeWidth {
			get {
				if(rowTreeWidth < 0)
					rowTreeWidth = GetRowTreeWidth();
				return rowTreeWidth;
			}
		}
		public int GetRowTreeWidthItemDiff(PivotFieldValueItem item) {
			return (item.StartLevel - RowMaxExpandedLevel) * RowTreeLevelOffset;
		}
		protected internal int RowMaxExpandedLevel {
			get {
				if(rowMaxExpandedLevel < 0)
					rowMaxExpandedLevel = GetMaxExpandedLevel(false);
				return rowMaxExpandedLevel;
			}
		}
		public PivotFieldItemBase GetSizingField(PivotFieldItemBase field) {
			if(field.Area == PivotArea.RowArea && Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree)
				return Data.GetFieldItem(RowTreeFieldItem);
			return Data.GetFieldItem(field);
		}
		public PivotGridFieldBase RowTreeField {
			get {
				if(rowTreeField == null) {
					rowTreeField = CreateRowTreeField();
					RowTreeField.IsRowTreeField = true;
					PrepareRowTreeFieldAfterCreating();
				}
				return rowTreeField;
			}
		}
		protected virtual void PrepareRowTreeFieldAfterCreating() { }
		public PivotFieldItemBase RowTreeFieldItem {
			get { return FieldItems.RowTreeFieldItem; }
		}		
		protected virtual int GetRowTreeWidth() {
			return RowTreeLevelOffset * RowMaxExpandedLevel + RowTreeLevelWidth;
		}
		public int GetMaxExpandedLevel(bool isColumn) {
			int count = GetItemCount(isColumn);
			int maxLevel = -1;
			for(int i = 0; i < count; i++) {
				PivotFieldValueItem item = GetItem(isColumn, i);
				if(item.IsDataLocatedInThisArea && item.Level == item.DataLevel) 
					continue;
				if(item.StartLevel > maxLevel)
					maxLevel = item.StartLevel;
			}
			return maxLevel;
		}
		public event ItemsEmptyEventHandler Cleared {
			add { cleared += value; }
			remove { cleared -= value; }
		}
		public event EventHandler BeforeCalculating {
			add { beforeCalculating += value; }
			remove { beforeCalculating -= value; }
		}
		public event EventHandler AfterCalculating {
			add { afterCalculating += value; }
			remove { afterCalculating -= value; }
		}
		protected virtual PivotFieldValueItemsCreator CreateColumnItemsCreator() {
			return new PivotFieldValueItemsCreator(Data, true);
		}
		protected virtual PivotFieldValueItemsCreator CreateRowItemsCreator() {
			return new PivotFieldValueItemsCreator(Data, false);
		}
		protected virtual PivotGridCellDataProviderBase CreateCellDataProvider() {
			return new PivotGridCellDataProvider(Data);
		}
		protected virtual PivotGridFieldBase CreateRowTreeField() {
			return new PivotTreeRowFieldBase(this);
		}
		public void EnsureIsCalculated() {
			if(IsDisposed) return;
			EnsureFieldItems();
			EnsureVisualItems();
		}
		protected internal void EnsureFieldItems() {
			if(!IsReadOnly && !IsReadyFieldItems && CanRecalculate) {
				FieldItems.RePopulate(Data, this);
				IsReadyFieldItems = true;
			}
		}
		void EnsureVisualItems() {
			if(!IsReadOnly && !IsReady && IsReadyFieldItems && CanRecalculate)
				Calculate();
			rowMaxExpandedLevel = -1;
		}
		bool CanRecalculate { get { return !Data.IsLockUpdate && !Data.Disposing; } }
		protected virtual void Calculate() {
			if(IsReadOnly || IsDisposed) return;
			CreateItems(VisualItemsPagingOptions.DefaultDisabled, VisualItemsPagingOptions.DefaultDisabled);
			new AggregationLevelsCalculator<PivotFieldValueItem, int>(this).Calculate(Data.GetAggregations(false));	
		}
		void OnBeforeCalculating() {
			beforeCalculating.SafeRaise(this, EventArgs.Empty);
		}
		void OnAfterCalculating() {
			afterCalculating.SafeRaise(this, EventArgs.Empty);
		}
		protected virtual void CreateItems(VisualItemsPagingOptions columnOpts, VisualItemsPagingOptions rowOpts) {
			if(IsReadOnly) return;
			OnBeforeCalculating();
			ColumnItemsCreator.CreateItems(false, VisualItemsPagingOptions.DefaultDisabled);
			RowItemsCreator.CreateItems(false, VisualItemsPagingOptions.DefaultDisabled);
			EnsureIsReady();
			if(OnCustomFieldValueCells()) {
				ColumnItemsCreator.CreateItems(true, columnOpts);
				RowItemsCreator.CreateItems(true, rowOpts);
			} else {
				if(columnOpts.Enabled)
					ColumnItemsCreator.ApplyPaging(true, columnOpts);
				if(rowOpts.Enabled)
					RowItemsCreator.ApplyPaging(true, rowOpts);
			}
			OnAfterCalculating();
		}
		protected virtual bool RaiseCustomFieldValueCells {
			get { return true; }
		}
		bool OnCustomFieldValueCells() {
			if(RaiseCustomFieldValueCells)
				return Data.OnCustomFieldValueCells(this);
			return false;
		}
		public virtual void Clear() {
			if(IsReadOnly) return;
			if(columnItemsCreator != null)
				ClearItemsCreator(columnItemsCreator);
			if(rowItemsCreator != null)
				ClearItemsCreator(rowItemsCreator);
			StreamDataProvider = null;
			RaiseCleared();
			rowTreeWidth = -1;
			rowMaxExpandedLevel = -1;
			IsReady = false;
			IsReadyFieldItems = false;
		}
		protected void ClearItemsCreator(PivotFieldValueItemsCreator itemsCreator) {
			itemsCreator.Clear();
			itemsCreator.ResetDataProvider();
		}
		protected internal PivotFieldValueItemsCreator GetItemsCreator(bool isColumn) {
			return isColumn ? ColumnItemsCreator : RowItemsCreator;
		}
		public PivotFieldValueItem GetColumnItem(int index) {
			return GetLastLevelItem(true, index);
		}
		public PivotFieldValueItem GetRowItem(int index) {
			return GetLastLevelItem(false, index);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return PivotCellValue.GetValue(GetCellValueEx(columnIndex, rowIndex));
		}
		public PivotCellValue GetCellValueEx(int columnIndex, int rowIndex) {
			return CellDataProvider.GetCellValueEx(GetColumnItem(columnIndex), GetRowItem(rowIndex));
		}
		public string GetCellDisplayText(int columnIndex, int rowIndex) {
			if(columnIndex < 0 || columnIndex >= ColumnCount)
				throw new ArgumentOutOfRangeException("columnIndex");
			if(rowIndex < 0 || rowIndex >= RowCount)
				throw new ArgumentOutOfRangeException("rowIndex");
			PivotGridCellItem cellItem = new PivotGridCellItem(CellDataProvider, GetColumnItem(columnIndex), GetRowItem(rowIndex), columnIndex, rowIndex);
			return CellDataProvider.GetDisplayText(cellItem, GetCellValueEx(columnIndex, rowIndex));
		}
		public object GetFieldValue(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			return item != null ? item.Value : null;
		}
		public string GetFieldValueDisplayText(PivotFieldItemBase field, int lastLevelIndex) {
			if(field == null)
				throw new ArgumentNullException("field");
			if(lastLevelIndex < 0)
				throw new ArgumentOutOfRangeException("lastLevelIndex");
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			string displayText = (item != null ? item.DisplayText : String.Empty);
			return displayText == null ? String.Empty : displayText;
		}
		public object GetFieldValue(bool isColumn, int lastLevelIndex, int level) {
			PivotFieldValueItem item = GetItem(isColumn, lastLevelIndex, level);
			return item != null ? item.Value : null;
		}
		public PivotGridValueType GetFieldValueType(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			if(item == null) throw new ArgumentException("no field value found");
			return item.ValueType;
		}
		public IOLAPMember GetOLAPMember(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			return GetOLAPMemberCore(field, item);
		}
		protected IOLAPMember GetOLAPMemberCore(PivotGridFieldBase field, PivotFieldValueItem item) {
			return item != null ? Data.GetOLAPMember(field, item.VisibleIndex) : null;
		}
		public bool IsObjectCollapsed(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetItem(field, lastLevelIndex);
			return item != null ? Data.IsObjectCollapsed(field.IsColumn, item.VisibleIndex) : false;
		}
		public bool IsObjectCollapsed(bool isColumn, int lastLevelIndex, int level) {
			PivotFieldValueItem item = GetItem(isColumn, lastLevelIndex, level);
			return item != null ? Data.IsObjectCollapsed(isColumn, item.VisibleIndex) : false;
		}
		public int GetLevelCount(bool isColumn) {
			return GetItemsCreator(isColumn).LevelCount;
		}
		public int GetLastLevelItemCount(bool isColumn) {
			return GetItemsCreator(isColumn).LastLevelItemCount;
		}
		public int GetItemCount(bool isColumn) {
			return GetItemsCreator(isColumn).Count;
		}
		public PivotFieldValueItem GetLastLevelItem(bool isColumn, int lastLevelIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return GetLastLevelItemCore(lastLevelIndex, itemsCreator);
		}
		public PivotFieldValueItem GetLastLevelUnpagedItem(bool isColumn, int lastLevelIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return GetLastLevelUnpagedItemCore(lastLevelIndex, itemsCreator);
		}
		protected PivotFieldValueItem GetLastLevelItemCore(int lastLevelIndex, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelItemCount) return null;
			return itemsCreator.GetLastLevelItem(lastLevelIndex);
		}
		protected PivotFieldValueItem GetLastLevelUnpagedItemCore(int lastLevelIndex, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelUnpagedItemCount) return null;
			return itemsCreator.GetLastLevelUnpagedItem(lastLevelIndex);
		}
		public PivotFieldValueItem GetItem(bool isColumn, int index) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			if(index < 0 || index >= itemsCreator.Count) return null;
			return itemsCreator[index];
		}
		public PivotFieldValueItem GetItem(bool isColumn, int lastLevelIndex, int level) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return GetItemCore(lastLevelIndex, level, itemsCreator);
		}
		public virtual PivotFieldValueItem GetUnpagedItem(bool isColumn, int lastLevelIndex, int level) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelUnpagedItemCount)
				return null;
			PivotFieldValueItem item = itemsCreator.GetLastLevelUnpagedItem(lastLevelIndex);
			while(item != null && !item.ContainsLevel(level))
				item = itemsCreator.GetParentUnpagedItem(item);
			return item;
		}
		public PivotFieldValueItem GetItem(bool isColumn, object[] values) {
			return GetItemsCreator(isColumn).GetItemByValues(values);
		}
		public IList<PivotFieldValueItem> GetItems(bool isColumn, object[] values, PivotFieldItemBase dataField) {
			return GetItemsCreator(isColumn).GetItemsByValues(values, dataField);
		}
		public int GetLastLevelIndex(bool isColumn, object[] values) {
			PivotFieldValueItem valueItem = GetItem(isColumn, values);
			return valueItem != null ? valueItem.MinLastLevelIndex : -1;
		}
		public int GetLastLevelIndex(bool isColumn, object[] values, PivotFieldItemBase dataField) {
			IList<PivotFieldValueItem> valueItems = GetItems(isColumn, values, dataField);
			if(valueItems.Count == 0)
				return -1;
			PivotFieldValueItem fieldValueItem = valueItems.Count > 1 ? 
				valueItems.First<PivotFieldValueItem>((item) => { return item.IsTotal; }) :
				valueItems[0];
			return fieldValueItem.MinLastLevelIndex;
		}
		protected PivotFieldValueItem GetItemCore(int lastLevelIndex, int level, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelItemCount) return null;
			PivotFieldValueItem item = itemsCreator.GetLastLevelItem(lastLevelIndex);
			while(item != null && !item.ContainsLevel(level))
				item = itemsCreator.GetParentItem(item);
			return item;
		}
		protected PivotFieldValueItem GetUnpagedItemCore(int lastLevelIndex, int level, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelUnpagedItemCount) return null;
			PivotFieldValueItem item = itemsCreator.GetLastLevelUnpagedItem(lastLevelIndex);
			while(item != null && !item.ContainsLevel(level))
				item = itemsCreator.GetParentUnpagedItem(item);
			return item;
		}
		public PivotFieldValueItem GetItem(PivotGridFieldBase field, int lastLevelIndex) {
			return GetItem(Data.GetFieldItem(field), lastLevelIndex);
		}
		public PivotFieldValueItem GetItem(PivotFieldItemBase field, int lastLevelIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(field.IsColumn);
			return GetItemCore(field, lastLevelIndex, itemsCreator);
		}
		protected PivotFieldValueItem GetItemCore(PivotFieldItemBase field, int lastLevelIndex, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelItemCount || !field.IsColumnOrRow) return null;
			PivotFieldValueItem item = itemsCreator.GetLastLevelItem(lastLevelIndex);
			while(item != null && item.Field != field)
				item = itemsCreator.GetParentItem(item);
			return item;
		}
		protected PivotFieldValueItem GetUnpagedItemCore(PivotGridFieldBase field, int lastLevelIndex, PivotFieldValueItemsCreator itemsCreator) {
			if(lastLevelIndex < 0 || lastLevelIndex >= itemsCreator.LastLevelUnpagedItemCount || !field.IsColumnOrRow) return null;
			PivotFieldValueItem item = itemsCreator.GetLastLevelUnpagedItem(lastLevelIndex);
			while(item != null && Data.GetField(item.Field) != field)
				item = itemsCreator.GetParentUnpagedItem(item);
			return item;
		}
		public PivotFieldValueItem GetParentItem(bool isColumn, PivotFieldValueItem item) {
			return GetItemsCreator(isColumn).GetParentItem(item);
		}
		protected PivotFieldValueItem GetParentUnpagedItem(bool isColumn, PivotFieldValueItem item) {
			return GetItemsCreator(isColumn).GetParentUnpagedItem(item);
		}
		public List<PivotGridFieldSortCondition> GetFieldSortConditions(bool isColumn, int index) {
			PivotFieldValueItem item = GetItem(isColumn, index);
			return Data.GetFieldSortConditions(item.IsColumn, item.VisibleIndex);
		}
		public List<PivotGridFieldPair> GetSortedBySummaryFields(bool isColumn, int itemIndex) {
			PivotFieldValueItem item = GetItem(isColumn, itemIndex);
			if(item == null || !item.IsLastFieldLevel) return null;
			if(item.SavedSortedBySummaryFields != null) return item.SavedSortedBySummaryFields;
			List<PivotFieldItemBase> dataFields;
			item.SavedSortedBySummaryFields = null;
			if(item.Area != FieldItems.DataFieldItem.Area) {
				dataFields = FieldItems.GetFieldItemsByArea(PivotArea.DataArea, false);
				if(dataFields.Count == 0)
					dataFields.Add(item.DataField);
			} else {
				dataFields = new List<PivotFieldItemBase>();
				dataFields.Add(item.DataField);
			}
			List<PivotGridFieldSortCondition> itemConditions = GetFieldSortConditions(isColumn, itemIndex);
			foreach(PivotFieldItemBase datafield in dataFields) {
				int fieldCount = FieldItems.Count;
				for(int i = 0; i < fieldCount; i++) {
					PivotFieldItemBase field = FieldItems[i];
					if(field.Area == item.CrossArea && field.Visible && IsFieldSortedBySummaryCore(field, datafield, item.SummaryType, itemConditions)) {
						if(item.SavedSortedBySummaryFields == null)
							item.SavedSortedBySummaryFields = new List<PivotGridFieldPair>();
						item.SavedSortedBySummaryFields.Add(new PivotGridFieldPair(Data, field, datafield != null ? datafield : Data.GetFieldItem(field.SortBySummaryInfo.Field)));
					}
				}
			}
			return item.SavedSortedBySummaryFields;
		}
		bool IsFieldSortedBySummaryCore(PivotFieldItemBase field, PivotFieldItemBase dataField, PivotSummaryType itemSummaryType,
									List<PivotGridFieldSortCondition> itemConditions) {
			PivotSummaryType? customTotalSummaryType = field.SortBySummaryInfo.CustomTotalSummaryType;
			bool hasCustomSummary = !customTotalSummaryType.HasValue ||
				(customTotalSummaryType.HasValue && itemSummaryType == customTotalSummaryType.Value);
			return hasCustomSummary && Data.IsFieldSortedBySummary(Data.GetField(field), Data.GetField(dataField), itemConditions);
		}
		public bool IsFieldSortedBySummary(bool isColumn, PivotFieldItemBase field, PivotFieldItemBase dataField, int index) {
			return IsFieldSortedBySummaryCore(field, dataField, GetItem(isColumn, index).SummaryType, 
				GetFieldSortConditions(isColumn, index));
		}
		public bool GetIsAnyFieldSortedBySummary(bool isColumn, int index) {
			return GetSortedBySummaryFields(isColumn, index) != null;
		}
		public PivotGridCellItem CreateUnpagedCellItem(int columnIndex, int rowIndex) {
			PivotFieldValueItem columnItem = GetItemsCreator(true).GetLastLevelUnpagedItem(columnIndex);
			PivotFieldValueItem rowItem = GetItemsCreator(false).GetLastLevelUnpagedItem(rowIndex);
			if(columnItem == null || rowItem == null)
				return null;
			return new PivotGridCellItem(CellDataProvider, columnItem, rowItem, columnIndex, rowIndex);
		}
		public PivotGridCellItem CreateCellItem(int columnIndex, int rowIndex) {
			PivotFieldValueItem columnItem = GetColumnItem(columnIndex);
			PivotFieldValueItem rowItem = GetRowItem(rowIndex);
			if(columnItem == null || rowItem == null)
				return null;
			return CreateCellItem(columnItem, rowItem, columnIndex, rowIndex);
		}
		public virtual PivotGridCellItem CreateCellItem(PivotFieldValueItem columnFieldValueItem, PivotFieldValueItem rowFieldValueItem, int columnIndex, int rowIndex) {
			return new PivotGridCellItem(CellDataProvider, columnFieldValueItem, rowFieldValueItem, columnIndex, rowIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			return CreateDrillDownDataSource(columnIndex, rowIndex, -1);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex);
			if(cellItem == null)
				return null;
			return Data.GetDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex, maxRowCount);
		}
		public PivotDrillDownDataSource CreateQueryModeDrillDownDataSource(int columnIndex, int rowIndex,
			int maxRowCount, List<string> customColumns) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex);
			if(cellItem == null)
				return null;
			return Data.GetQueryModeDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex,
				maxRowCount, customColumns);
		}
		[Obsolete("This method is now obsolete. Use the CreateQueryModeDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
				return CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		public string SavedFieldValueItemsState() {
			return PivotGridSerializeHelper.ToBase64String(SavedFieldValueItemsStateCore, Data.OptionsData.CustomObjectConverter);
		}
		protected virtual void SavedFieldValueItemsStateCore(TypedBinaryWriter writer) {
			ColumnItemsCreator.SaveToStream(writer);
			RowItemsCreator.SaveToStream(writer);
		}
		public string SavedDataCellsState() {
			return PivotGridSerializeHelper.ToBase64String(SavedDataCellsStateCore, Data.OptionsData.CustomObjectConverter);
		}
		protected virtual void SavedDataCellsStateCore(TypedBinaryWriter writer) {
			CellDataProvider.SaveToStream(writer, ColumnItemsCreator, RowItemsCreator);
		}
		internal void LoadFieldValueItemsState(string state) {
			if(string.IsNullOrEmpty(state)) return;
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(state));
			LoadFieldValueItemsStateCore(stream);
		}
		protected virtual void LoadFieldValueItemsStateCore(MemoryStream stream) {
			ColumnItemsCreator.LoadFromStream(stream);
			RowItemsCreator.LoadFromStream(stream);
			EnsureIsReady();
		}
		internal void LoadDataCellsState(string state) {
			if(string.IsNullOrEmpty(state)) return;
			StreamDataProvider = new PivotGridCellStreamDataProvider(Data);
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(state));
			LoadDataCellsStateCore(stream);
		}
		protected virtual void LoadDataCellsStateCore(MemoryStream stream) {
			StreamDataProvider.LoadFromStream(stream, ColumnItemsCreator, RowItemsCreator);
		}
		protected virtual void RaiseCleared() {
			if(cleared != null)
				cleared.Raise(this, EventArgs.Empty);
		}
		protected internal PivotVisualItemsBase Clone(bool readOnly) {
			PivotVisualItemsBase clone = Data.CreateVisualItems();
			clone.cellDataProvider = new PivotGridEmptyCellsDataProvider(Data, GetCellDataProvider());
			clone.columnItemsCreator = ColumnItemsCreator.Clone();
			clone.rowItemsCreator = RowItemsCreator.Clone();
			clone.rowMaxExpandedLevel = RowMaxExpandedLevel;
			clone.rowTreeField = RowTreeField;
			clone.rowTreeWidth = rowTreeWidth;
			clone.streamDataProvider = StreamDataProvider;
			clone.state = state;
			clone.EnsureIsReady();
			clone.IsReadOnly = readOnly;
			return clone;
		}
		#region ICloneable Members
		object ICloneable.Clone() {
			return this.Clone(false);
		}
		#endregion
		#region IThreadSafeFieldOwner Members
		IThreadSafeFieldCollection IThreadSafeAccessible.Fields {
			get { return FieldItems as IThreadSafeFieldCollection; }
		}
		IThreadSafeGroupCollection IThreadSafeAccessible.Groups {
			get { return FieldItems.GroupItems as IThreadSafeGroupCollection; }
		}
		IThreadSafeField IThreadSafeAccessible.GetFieldByArea(PivotArea area, int index) {
			return (IThreadSafeField)FieldItems.GetFieldItemByArea(area, index);
		}
		IThreadSafeField IThreadSafeAccessible.GetFieldByLevel(bool isColumn, int level) {
			return (IThreadSafeField)FieldItems.GetFieldItemByLevel(isColumn, level);
		}
		List<IThreadSafeField> IThreadSafeAccessible.GetFieldsByArea(PivotArea area) {
			List<PivotFieldItemBase> items = FieldItems.GetFieldItemsByArea(area, false);
			List<IThreadSafeField> fields = new List<IThreadSafeField>();
			items.ForEach(delegate(PivotFieldItemBase item) {
				fields.Add(item as IThreadSafeField);
			});
			return fields;
		}
		int IThreadSafeAccessible.GetFieldCountByArea(PivotArea area) {
			return FieldItems.GetFieldCountByArea(area);
		}
		string IThreadSafeAccessible.GetFieldValueDisplayText(IThreadSafeField field, int lastLevelIndex) {
			return GetFieldValueDisplayText((PivotFieldItemBase)field, lastLevelIndex);
		}
		string IThreadSafeAccessible.GetCellDisplayText(int columnIndex, int rowIndex) {
			return GetCellDisplayText(columnIndex, rowIndex);
		}
		bool IThreadSafeAccessible.IsAsyncInProgress { get { return Data.IsLocked; } }
		#endregion
		#region ICalculationContext
		ICalculationSource<PivotFieldValueItem, int> ICalculationContext<PivotFieldValueItem, int>.GetValueProvider() {
			return this;
		}
		int ICalculationContext<PivotFieldValueItem, int>.GetData(int index) {
			return index;
		}
		IEnumerable<PivotFieldValueItem> ICalculationContext<PivotFieldValueItem, int>.EnumerateFullLevel(bool isColumn, int level) {
			foreach(PivotFieldValueItem item in GetItemsCreator(isColumn).GetItems())
				if(item.FieldLevel == level)
					yield return item;
		}
		#endregion
		#region ICalculationSource
		object ICalculationSource<PivotFieldValueItem, int>.GetValue(PivotFieldValueItem column, PivotFieldValueItem row, int data) {
			if(Data.GetFieldCountByArea(PivotArea.DataArea) > 1 && (Data.DataField.Area == PivotArea.ColumnArea ? column : row).DataIndex != data)
				return null;
			PivotCellValue cellValue = GetCellValueEx(column.MinLastLevelIndex, row.MinLastLevelIndex);
			return cellValue != null ? cellValue.Value : null;
		}
		object ICalculationContext<PivotFieldValueItem, int>.GetValue(PivotFieldValueItem levelValue) {
			return levelValue.Value;
		}
		object ICalculationContext<PivotFieldValueItem, int>.GetDisplayValue(PivotFieldValueItem levelValue) {
			return levelValue.Value;
		}
		#endregion
	}
	public class VisualItemsPagingOptions {
		public static VisualItemsPagingOptions DefaultDisabled {
			get {
				VisualItemsPagingOptions opts = new VisualItemsPagingOptions();
				PrepareDisabledOptions(opts);
				return opts;
			}
		}
		static void PrepareDisabledOptions(VisualItemsPagingOptions opts) {
			opts.Enabled = false;
			opts.ShowGrandTotalsOnEachPage = true;
			opts.Start = 0;
			opts.Count = 0;
		}
		public bool Enabled { get; set; }
		public bool ShowGrandTotalsOnEachPage { get; set; }
		public int Start { get; set; }
		public int Count { get; set; }
		public VisualItemsPagingOptions() {
			PrepareDisabledOptions(this);
		}
	}
}
