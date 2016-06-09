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
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Data.IO;
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.Data {
	public class FieldValueItemsGenerator {
		readonly PivotGridFieldValueItemsDataProviderBase dataProvider;
		public FieldValueItemsGenerator(PivotGridData data, bool isColumn) {
			this.dataProvider = new PivotGridFieldValueItemsDataProvider(data, isColumn);
		}
		public FieldValueItemsGenerator(PivotGridFieldValueItemsDataProviderBase dataProvider, bool isColumn) {
			this.dataProvider = dataProvider;
		}
		protected PivotGridFieldValueItemsDataProviderBase DataProvider {
			get { return dataProvider; }
		}
		public void Generate(FieldValueItemCollection collection, FieldValueGenerationParams _params) {
			collection.Clear();
			DataProvider.Reset();
			GrandTotalChunk chunk = new GrandTotalChunk(DataProvider, _params, new ValuesGeneratorState(DataProvider));
			collection.AddRange(chunk.GenerateItems());
		}
	}
	public class ValueItemChunk {
		readonly PivotGridFieldValueItemsDataProviderBase dataProvider;
		readonly ValuesGeneratorState state;
		readonly int itemIndex, itemLevel, dataIndex;
		readonly FieldValueGenerationParams _params;
		PivotGridFieldBase field;
		public ValueItemChunk(PivotGridFieldValueItemsDataProviderBase dataProvider, int itemIndex, int dataIndex,
				FieldValueGenerationParams _params, ValuesGeneratorState state) :
			this(dataProvider, itemIndex, dataProvider.GetObjectLevel(itemIndex), dataIndex, _params, state) {
		}
		protected ValueItemChunk(PivotGridFieldValueItemsDataProviderBase dataProvider, int itemIndex, int itemLevel, 
				int dataIndex,
				FieldValueGenerationParams _params, ValuesGeneratorState state) {
			this._params = _params;
			this.dataProvider = dataProvider;
			this.state = state;
			this.itemIndex = itemIndex;
			this.dataIndex = dataIndex;
			this.itemLevel = itemLevel;
			if(this.itemLevel >= 0 && this.itemLevel == DataProvider.LevelCount - 1)
				throw new ArgumentException("Can't create a ValueItemChunk for last level items");
		}
		protected PivotGridFieldValueItemsDataProviderBase DataProvider { get { return dataProvider; } }
		protected ValuesGeneratorState State { get { return state; } }
		protected bool IsColumn { get { return DataProvider.IsColumn; } }
		protected int ItemIndex { get { return itemIndex; } }
		protected virtual int DataIndex { get { return dataIndex; } }
		protected int ItemLevel { get { return itemLevel;} }
		protected PivotGridFieldBase Field {
			get {
				if(field == null)
					field = DataProvider.Data.GetFieldByArea(IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea, ItemLevel);
				return field;
			}
		}
		protected FieldValueGenerationParams Params { get { return _params; } }		
		public virtual List<PivotFieldValueItem> GenerateItems(out int maxVisibleIndex) {
			maxVisibleIndex = ItemIndex;
			List<PivotFieldValueItem> res = new List<PivotFieldValueItem>();
			bool isCollapsed = DataProvider.IsObjectCollapsed(ItemIndex);
			if(isCollapsed) {
				CreateChildItemCore(res, ItemIndex, DataProvider.GetObjectLevel(ItemIndex), isCollapsed);
			} else {
				int childItemsCount = 0;
				List<PivotFieldValueItem> childItems = GenerateChildItems(ItemIndex + 1, true, out childItemsCount, out maxVisibleIndex);
				if(childItems.Count > 0) {
					res.Add(new PivotFieldCellValueItem(DataProvider, ItemIndex, DataProvider.GetObjectLevel(ItemIndex), isCollapsed, DataIndex));
					res.AddRange(childItems);
				}
				AddTotals(res, childItemsCount);
			}			
			return res;
		}
		protected List<PivotFieldValueItem> GenerateChildItems(int startIndex, bool generateDataItems, 
				out int childItemsCount, out int maxVisibleIndex) {
			List<PivotFieldValueItem> items = new List<PivotFieldValueItem>();
			int nextLevel = ItemLevel + 1;
			bool createChunks = nextLevel < DataProvider.LevelCount - 1;
			int cellCount = DataProvider.CellCount;
			childItemsCount = 0;
			maxVisibleIndex = 0;
			for(int i = startIndex; i < cellCount; i++) {
				int itemLevel = DataProvider.GetObjectLevel(i);
				if(itemLevel <= ItemLevel) break;
				maxVisibleIndex = i;
				if(State.IsDataLocatedInThisArea && generateDataItems && itemLevel == State.DataLevel) {
					int dataChildItemsCount = CreateTopDataItem(items, i, ref maxVisibleIndex);
					childItemsCount += dataChildItemsCount;					
					i = maxVisibleIndex;
					continue;
				}
				if(createChunks) {
					if(itemLevel > nextLevel) continue;
					if(itemLevel < nextLevel)
						throw new Exception("broken hierarchy");
					CreateChildChunk(items, i, ref maxVisibleIndex);
					i = maxVisibleIndex;
				} else {
					CreateChildItemCore(items, i, itemLevel, DataProvider.IsObjectCollapsed(i));
				}
				childItemsCount++;
			}
			return items;
		}
		void CreateChildChunk(List<PivotFieldValueItem> items, int visibleIndex, ref int maxVisibleIndex) {
			ValueItemChunk child = new ValueItemChunk(DataProvider, visibleIndex, DataIndex, Params, State);
			items.AddRange(child.GenerateItems(out maxVisibleIndex));
			if(maxVisibleIndex < visibleIndex)
				throw new Exception("child generated no items");
		}
		int CreateTopDataItem(List<PivotFieldValueItem> items, int visibleIndex, ref int maxVisibleIndex) {
			TopDataItemsChunk child = new TopDataItemsChunk(DataProvider, ItemIndex, visibleIndex, ItemLevel, Params, State);
			int dataChildItemsCount;
			items.AddRange(child.GenerateItems(out dataChildItemsCount, out maxVisibleIndex));
			if(maxVisibleIndex < visibleIndex)
				throw new Exception("child generated no items");
			return dataChildItemsCount;
		}
		void CreateChildItemCore(List<PivotFieldValueItem> items, int visibleIndex, int level, bool isCollapsed) {
			if(HideItems(PivotGridValueType.Value))
				return;
			PivotFieldCellValueItem item = new PivotFieldCellValueItem(DataProvider, visibleIndex, level, isCollapsed, DataIndex);
			if(item.DataField != null && !item.DataField.Options.ShowValues && !IsBeforeDataItems(item) && State.IsDataLocatedInThisArea && State.DataFieldCount > 1)
				return;
			items.Add(item);
			if(State.IsDataItemsVisible && item.EndLevel == DataProvider.LevelCount - 1) {
				if(State.GetDataItemsCount(PivotGridValueType.Value) > 1)
					items.AddRange(CreateLastLevelDataItems(DataProvider.LevelCount, visibleIndex, PivotGridValueType.Value));
				else
					item.EndLevel++;
			}
			if(IsBeforeDataItems(item))
				item.DataIndex = GetDataIndex(PivotGridValueType.Value);
		}
		#region totals
		protected virtual void AddTotals(List<PivotFieldValueItem> items, int childItemsCount) {
			if(!AddTotals(childItemsCount)) return;
			if(IsColumn) {
				if(Params.ColumnTotalsLocation == PivotTotalsLocation.Far)
					items.AddRange(CreateTotals());
				if(Params.ColumnTotalsLocation == PivotTotalsLocation.Near)
					items.InsertRange(0, CreateTotals());
			} else {
				if(Params.RowTotalsLocation == PivotRowTotalsLocation.Far)
					items.AddRange(CreateTotals());
				if(Params.RowTotalsLocation == PivotRowTotalsLocation.Near)
					items.InsertRange(0, CreateTotals());
				if(Params.RowTotalsLocation == PivotRowTotalsLocation.Tree) {
					items.InsertRange(0, CreateAutoTotals());
					if(Field.TotalsVisibility == PivotTotalsVisibility.CustomTotals)
						items.AddRange(CreateCustomTotals());
				}
			}
		}
		bool AddTotals(int childItemCount) {
			if(childItemCount == 0) return false;
			if(!IsColumn && Params.RowTotalsLocation == PivotRowTotalsLocation.Tree)
				return true;
			if(Field.TotalsVisibility == PivotTotalsVisibility.CustomTotals && !Field.Options.ShowCustomTotals)
				return false;
			if(Field.TotalsVisibility == PivotTotalsVisibility.AutomaticTotals) {
				if(!Field.Options.ShowTotals || !Params.ShowAutoTotals)
					return false;
			}
			if(childItemCount == 1) {				
				if(Field.TotalsVisibility == PivotTotalsVisibility.AutomaticTotals &&
					!Params.ShowAutoTotalsForSingleValues) return false;
				if(Field.TotalsVisibility == PivotTotalsVisibility.CustomTotals &&
					!Params.ShowCustomTotalsForSingleValues) return false;
			}
			return Field.IsAggregatable;
		}
		List<PivotFieldValueItem> CreateTotals() {
			switch(Field.TotalsVisibility) {
				case PivotTotalsVisibility.AutomaticTotals:
					return CreateAutoTotals();
				case PivotTotalsVisibility.CustomTotals:
					return CreateCustomTotals();
			}
			return new List<PivotFieldValueItem>();
		}
		List<PivotFieldValueItem> CreateAutoTotals() {
			AutoTotalChunk totalsChunk = new AutoTotalChunk(DataProvider, ItemIndex, DataIndex, 
				Params, State);
			return totalsChunk.GenerateItems();
		}
		List<PivotFieldValueItem> CreateCustomTotals() {
			CustomTotalsChunk totalsChunk = new CustomTotalsChunk(DataProvider, ItemIndex, DataIndex, 
				Params, State);
			return totalsChunk.GenerateItems();
		}
		#endregion
		protected List<PivotFieldValueItem> CreateLastLevelDataItems(int level, int visibleIndex, PivotGridValueType valueType) {
			List<PivotFieldValueItem> dataItems = new List<PivotFieldValueItem>();
			AddLastLevelDataItemsCore(dataItems, level, visibleIndex, valueType, false);
			if(dataItems.Count == 0)
				AddLastLevelDataItemsCore(dataItems, level, visibleIndex, valueType, true);
			return dataItems;
		}
		protected int GetDataItemsCount(PivotGridValueType valueType) {
			int count = 0;
			for(int i = 0; i < State.DataFieldCount; i++) {
				PivotGridFieldBase dataField = DataProvider.Data.GetFieldByArea(PivotArea.DataArea, i);
				if(dataField.CanShowValueType(valueType))
					count++;
			}
			return count;
		}
		protected int GetDataIndex(PivotGridValueType valueType) {
			for(int i = 0; i < State.DataFieldCount; i++) {
				PivotGridFieldBase dataField = DataProvider.Data.GetFieldByArea(PivotArea.DataArea, i);
				if(dataField.CanShowValueType(valueType))
					return i;
			}
			return 0;
		}
		protected bool HideItems(PivotGridValueType valueType) {
			return State.IsDataLocatedInThisArea && State.DataFieldCount > 0 && GetDataItemsCount(valueType) == 0;
		}
		protected bool IsBeforeDataItems(PivotFieldValueItem item) {
			return item.StartLevel < State.DataLevel || State.DataLevel < 0;
		}
		void AddLastLevelDataItemsCore(List<PivotFieldValueItem> items, int level, int visibleIndex, PivotGridValueType valueType, bool forced) {
			for(int i = 0; i < State.DataFieldCount; i++) {
				PivotGridFieldBase dataField = DataProvider.Data.GetFieldByArea(PivotArea.DataArea, i);
				if(!forced && !dataField.CanShowValueType(valueType))
					continue;
				PivotFieldValueItem dataItem = CreateDataItem(i, level, visibleIndex, valueType);
				items.Add(dataItem);
			}
		}
		protected virtual PivotFieldValueItem CreateDataItem(int dataIndex, int level, int visibleIndex, PivotGridValueType valueType) {
			PivotFieldValueItem dataItem = new PivotFieldDataCellValueItem(DataProvider,
				visibleIndex, dataIndex, valueType, level);
			return dataItem;
		}
	}
	public class AutoTotalChunk : ValueItemChunk {
		public AutoTotalChunk(PivotGridFieldValueItemsDataProviderBase dataProvider, int itemIndex, int dataIndex,
				FieldValueGenerationParams _params, ValuesGeneratorState state) :
			base(dataProvider, itemIndex, dataIndex, _params, state) {
		}
		public List<PivotFieldValueItem> GenerateItems() {
			int maxVisibleIndex;
			return GenerateItems(out maxVisibleIndex);
		}
		public override List<PivotFieldValueItem> GenerateItems(out int maxVisibleIndex) {
			maxVisibleIndex = ItemIndex;
			List<PivotFieldValueItem> res = new List<PivotFieldValueItem>();
			if(HideItems(PivotGridValueType.Total))
				return res;
			PivotFieldTotalCellValueItem totalCell = new PivotFieldTotalCellValueItem(DataProvider,
				ItemIndex, DataIndex);
			if(totalCell.DataField != null && State.IsDataLocatedInThisArea && !totalCell.DataField.Options.ShowTotals && !IsBeforeDataItems(totalCell))
				return res;
			res.Add(totalCell);
			if(State.IsDataItemsVisible && totalCell.StartLevel < State.DataLevel)
				AddTotalDataCells(res, totalCell, PivotGridValueType.Total);
			if(IsBeforeDataItems(totalCell))
				totalCell.DataIndex = GetDataIndex(PivotGridValueType.Total);
			return res;
		}
		protected void AddTotalDataCells(List<PivotFieldValueItem> res, PivotFieldValueItem cell, PivotGridValueType valueType) {
			List<PivotFieldValueItem> dataItems = CreateLastLevelDataItems(cell.EndLevel + 1, ItemIndex, valueType);
			if(dataItems.Count == 0)
				throw new Exception("no data items generated");
			if(dataItems.Count == 1)
				cell.EndLevel++;				
			else
				res.AddRange(dataItems);
			cell.DataIndex = dataItems[0].DataIndex;
		}
	}
	public class CustomTotalsChunk : AutoTotalChunk {
		PivotGridCustomTotalBase currentCustomTotal;
		public CustomTotalsChunk(PivotGridFieldValueItemsDataProviderBase dataProvider, int itemIndex, int dataIndex,
				FieldValueGenerationParams _params, ValuesGeneratorState state) :
			base(dataProvider, itemIndex, dataIndex, _params, state) {
		}
		public override List<PivotFieldValueItem> GenerateItems(out int maxVisibleIndex) {
			maxVisibleIndex = ItemIndex;
			List<PivotFieldValueItem> res = new List<PivotFieldValueItem>();
			if(HideItems(PivotGridValueType.CustomTotal))
				return res;
			for(int i = 0; i < Field.CustomTotals.Count; i++) {
				this.currentCustomTotal = Field.CustomTotals[i];
				PivotFieldCustomTotalCellValueItem totalCell =
					new PivotFieldCustomTotalCellValueItem(DataProvider, ItemIndex, this.currentCustomTotal, DataIndex, i == 0);
				res.Add(totalCell);
				if(State.IsDataItemsVisible && totalCell.StartLevel < State.DataLevel) 
					AddTotalDataCells(res, totalCell, PivotGridValueType.CustomTotal);
				if(IsBeforeDataItems(totalCell))
					totalCell.DataIndex = GetDataIndex(PivotGridValueType.CustomTotal);				
			}
			return res;
		}
		protected override PivotFieldValueItem CreateDataItem(int dataIndex, int level, int visibleIndex, PivotGridValueType valueType) {
			return new PivotFieldCustomTotalDataCellValueItem(DataProvider, visibleIndex, this.currentCustomTotal, dataIndex);
		}
	}
	public class GrandTotalChunk : AutoTotalChunk {
		public GrandTotalChunk(PivotGridFieldValueItemsDataProviderBase dataProvider,
				FieldValueGenerationParams _params, ValuesGeneratorState state)
			: base(dataProvider, -1, 0, _params, state) {
		}
		public override List<PivotFieldValueItem> GenerateItems(out int maxVisibleIndex) {
			maxVisibleIndex = -1;
			List<PivotFieldValueItem> res = new List<PivotFieldValueItem>();
			int childItemsCount = 0;
			if(DataProvider.LevelCount > 0)
				res.AddRange(GenerateChildItems(0, true, out childItemsCount, out maxVisibleIndex));			
			AddTotals(res, childItemsCount);
			return res;
		}
		protected override void AddTotals(List<PivotFieldValueItem> items, int childItemsCount) {
			if(items.Count > 0) {
				if(GetDataItemsCount(PivotGridValueType.GrandTotal) == 0 || Params.GrandTotalsLocation == null) 
					return;
				if(childItemsCount == 1 && !Params.ShowGrandTotalsForSingleValues) 
					return;
				PivotArea area = IsColumn ? PivotArea.ColumnArea : PivotArea.RowArea;
				if(DataProvider.Data.GetFieldCountByArea(area) != 0 && !DataProvider.Data.GetFieldByArea(area, 0).IsAggregatable)
					return;
			}			
			if(Params.GrandTotalsLocation == PivotTotalsLocation.Near)
				items.InsertRange(0, CreateGrandTotals());
			else
				items.AddRange(CreateGrandTotals());
		}		
		List<PivotFieldValueItem> CreateGrandTotals() {
			List<PivotFieldValueItem> res = new List<PivotFieldValueItem>();
			if(!Params.HideGrandTotalHeader) {				
				PivotFieldGrandTotalCellValueItem grandTotalItem = new PivotFieldGrandTotalCellValueItem(DataProvider, 0);
				grandTotalItem.DataIndex = GetDataIndex(PivotGridValueType.GrandTotal);
				res.Add(grandTotalItem);
				if(State.IsDataItemsVisible)
					AddTotalDataCells(res, grandTotalItem, PivotGridValueType.GrandTotal);
			} else {
				if(State.IsDataItemsVisible) {
					List<PivotFieldValueItem> dataItems = CreateLastLevelDataItems(0, ItemIndex, PivotGridValueType.GrandTotal);
					foreach(PivotFieldValueItem dataItem in dataItems) {
						dataItem.EndLevel = DataProvider.LevelCount;
					}
					res.AddRange(dataItems);
				}
			}
			return res;
		}
	}
	public class TopDataItemsChunk : ValueItemChunk {
		int actualDataIndex;
		readonly int firstChildIndex;
		public TopDataItemsChunk(PivotGridFieldValueItemsDataProviderBase dataProvider, int itemIndex, int firstChildIndex,
					int parentItemLevel, FieldValueGenerationParams _params, ValuesGeneratorState state)
			: base(dataProvider, itemIndex, parentItemLevel, -1, _params, state) {
				this.firstChildIndex = firstChildIndex;
		}
		protected override int DataIndex { get { return actualDataIndex; } }
		public List<PivotFieldValueItem> GenerateItems(out int childItemsCount, out int maxVisibleIndex) {
			childItemsCount = 0;
			maxVisibleIndex = 0;
			List<PivotFieldValueItem> items = new List<PivotFieldValueItem>();
			for(int i = 0; i < State.DataFieldCount; i++) {
				this.actualDataIndex = i;
				int visibleIndex = ItemIndex >= 0 ? ItemIndex : 0;  
				PivotFieldTopDataCellValueItem dataItem = new PivotFieldTopDataCellValueItem(DataProvider,
					visibleIndex, i);
				items.Add(dataItem);
				items.AddRange(GenerateChildItems(this.firstChildIndex, false, out childItemsCount, out maxVisibleIndex));
			}
			return items;
		}
		public override List<PivotFieldValueItem> GenerateItems(out int maxVisibleIndex) {
			int childItemsCount;
			return GenerateItems(out childItemsCount, out maxVisibleIndex);
		}
	}
	public class FieldValueGenerationParams {
		PivotRowTotalsLocation rowTotalsLocation;
		PivotTotalsLocation columnTotalsLocation;
		int pageStartIndex, pageItemCount;
		bool showAutoTotalsForSingleValues, showCustomTotalsForSingleValues;
		bool showAutoTotals;
		PivotTotalsLocation? grandTotalsLocation;		
		bool showGrandTotalsForSingleValues;
		bool hideGrandTotalHeader;
		public FieldValueGenerationParams() {
		}
		public FieldValueGenerationParams(PivotRowTotalsLocation? rowTotalsLocation, 
			PivotTotalsLocation? columnTotalsLocation, int pageStartIndex, int pageItemCount) {
				if(rowTotalsLocation.HasValue) {
					this.rowTotalsLocation = rowTotalsLocation.Value;
				} else {
					this.rowTotalsLocation = PivotRowTotalsLocation.Far;
				}
				if(columnTotalsLocation.HasValue) {
					this.columnTotalsLocation = columnTotalsLocation.Value;
				} else {
					this.columnTotalsLocation = PivotTotalsLocation.Far;
				}
				this.showAutoTotals = true;
		}
		public FieldValueGenerationParams(PivotRowTotalsLocation? rowTotalsLocation,
			PivotTotalsLocation? columnTotalsLocation)
			: this(rowTotalsLocation, columnTotalsLocation, 0, 0) {
		}
		public FieldValueGenerationParams(PivotGridData data, bool isColumn) {
			PivotGridOptionsViewBase options = data.OptionsView;
			ColumnTotalsLocation = options.ColumnTotalsLocation;
			RowTotalsLocation = options.RowTotalsLocation;
			ShowAutoTotals = (isColumn && options.ShowColumnTotals) || (!isColumn && options.ShowRowTotals);
			ShowAutoTotalsForSingleValues = options.ShowTotalsForSingleValues;
			ShowCustomTotalsForSingleValues = options.ShowCustomTotalsForSingleValues;
			if(isColumn && options.ShowColumnGrandTotals)
				GrandTotalsLocation = options.ColumnTotalsLocation;
			if(!isColumn && options.ShowRowGrandTotals)
				GrandTotalsLocation = TotalsLocationHelper.ToTotalsLocation(options.RowTotalsLocation, PivotTotalsLocation.Far);
			ShowGrandTotalsForSingleValues = options.ShowGrandTotalsForSingleValues;
			HideGrandTotalHeader = data.ShouldRemoveGrandTotalHeader(isColumn);
		}
		public PivotRowTotalsLocation RowTotalsLocation { 
			get { return rowTotalsLocation; }
			set { rowTotalsLocation = value; }
		}
		public PivotTotalsLocation ColumnTotalsLocation {
			get { return columnTotalsLocation; }
			set { columnTotalsLocation = value; }
		}
		public int PageStartIndex {
			get { return pageStartIndex; }
			set { pageStartIndex = value; }
		}
		public int PageItemCount {
			get { return pageItemCount; }
			set { pageItemCount = value; }
		}
		public bool ShowAutoTotals {
			get { return showAutoTotals; }
			set { showAutoTotals = value; }
		}
		public bool ShowAutoTotalsForSingleValues {
			get { return showAutoTotalsForSingleValues; }
			set { showAutoTotalsForSingleValues = value; }
		}
		public bool ShowCustomTotalsForSingleValues {
			get { return showCustomTotalsForSingleValues; }
			set { showCustomTotalsForSingleValues = value; }
		}
		public PivotTotalsLocation? GrandTotalsLocation {
			get { return grandTotalsLocation; }
			set { grandTotalsLocation = value; }
		}
		public bool ShowGrandTotalsForSingleValues {
			get { return showGrandTotalsForSingleValues; }
			set { showGrandTotalsForSingleValues = value; }
		}
		public bool HideGrandTotalHeader {
			get { return hideGrandTotalHeader; }
			set { hideGrandTotalHeader = value; }
		}
	}
	public class ValuesGeneratorState {
		readonly PivotGridFieldValueItemsDataProviderBase dataProvider;
		readonly bool isColumn, isDataItemsVisible, isDataLocatedInThisArea;
		readonly int dataLevel, dataFieldCount;
		public ValuesGeneratorState(PivotGridFieldValueItemsDataProviderBase dataProvider) {
			this.dataProvider = dataProvider;
			this.isColumn = dataProvider.IsColumn;
			this.isDataItemsVisible = Data.GetIsDataFieldsVisible(IsColumn);
			this.isDataLocatedInThisArea = Data.GetIsDataLocatedInThisArea(IsColumn);
			this.dataLevel = GetDataLevel();
			this.dataFieldCount = Data.DataFieldCount;
		}
		protected PivotGridFieldValueItemsDataProviderBase DataProvider { get { return dataProvider; } }
		protected PivotGridData Data { get { return DataProvider.Data; } }
		public bool IsColumn { get { return isColumn; } }
		public bool IsDataItemsVisible {
			get { return isDataItemsVisible; }
		}
		public bool IsDataLocatedInThisArea {
			get { return isDataLocatedInThisArea; }
		}
		public int DataLevel {
			get { return dataLevel; }
		}
		public int DataFieldCount {
			get { return dataFieldCount; }
		}
		int GetDataLevel() {
			int index = Data.OptionsDataField.DataFieldAreaIndex;
			if(index == 0 && DataProvider.LevelCount == 0 && !Data.ShouldRemoveGrandTotalHeader(IsColumn))
				index++;
			return index;
		}		
		public int GetDataItemsCount(PivotGridValueType valueType) {
			int count = 0;
			for(int i = 0; i < DataFieldCount; i++) {
				if(Data.GetFieldByArea(PivotArea.DataArea, i).CanShowValueType(valueType))
					count++;
			}
			return count;
		}
	}
}
