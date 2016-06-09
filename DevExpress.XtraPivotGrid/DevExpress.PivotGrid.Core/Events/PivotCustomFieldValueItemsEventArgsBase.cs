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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Data.PivotGrid;
#if SL
namespace System.Collections.Generic {
	public static class ListExtensions {
		public static int FindIndex<T>(this List<T> list, Predicate<T> predicate) {
			int count = list.Count;
			for(int i = 0; i < count; i++)
				if(predicate(list[i]))
					return i;
			return -1;
		}
	}
}
#endif
namespace DevExpress.XtraPivotGrid.Data {
	public enum GrandTotalLocation { Near, Far }
	public class FieldValueCellBase {
		readonly PivotFieldValueItem item;
		public FieldValueCellBase(PivotFieldValueItem item) {
			if(item == null)
				throw new ArgumentNullException("FieldValueCellBase: item");
			this.item = item;
		}
		protected internal PivotFieldValueItem Item { get { return item; } }
		protected PivotFieldValueItem ParentItem { get { return item.Parent; } }
		protected PivotFieldItemBase Field { get { return item.Field; } }
		protected PivotFieldItemBase DataField { get { return IsDataFieldNull ? null : item.DataField; } }
		bool IsDataFieldNull {
			get {
				return Item.Data.GetIsDataFieldsVisible(!Item.IsColumn) ||
				   Item.IsDataFieldsVisible &&
					   (Item.IsTotal && Field == null || Item.DataLevel > Item.StartLevel);
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseStartLevel")]
#endif
		public int StartLevel { get { return item.StartLevel; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseEndLevel")]
#endif
		public int EndLevel { get { return item.EndLevel; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseValue")]
#endif
		public object Value { get { return item.Value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseValueType")]
#endif
		public PivotGridValueType ValueType { get { return item.ValueType; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseDisplayText")]
#endif
		public string DisplayText { get { return item.DisplayText; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseCanExpand")]
#endif
		public bool CanExpand { get { return item.ShowCollapsedButton; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseIsCollapsed")]
#endif
		public bool IsCollapsed { get { return item.IsCollapsed; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseIsColumn")]
#endif
		public bool IsColumn { get { return item.IsColumn; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseIsVisible")]
#endif
		public bool IsVisible { get { return item.IsVisible; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseMinIndex")]
#endif
		public int MinIndex { get { return item.MinLastLevelIndex; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueCellBaseMaxIndex")]
#endif
		public int MaxIndex { get { return item.MaxLastLevelIndex; } }
	}
	public class FieldValueSplitData {
		object value;
		int nestedCellCount;
		public FieldValueSplitData(object value, int nestedCellCount) {
			this.value = value;
			this.nestedCellCount = nestedCellCount;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueSplitDataValue")]
#endif
		public object Value { get { return this.value; } set { this.value = value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("FieldValueSplitDataNestedCellCount")]
#endif
		public int NestedCellCount { get { return nestedCellCount; } set { nestedCellCount = value; } }
	}
	public class PivotCustomFieldValueCellsEventArgsBase : EventArgs {
		static void Swap<T>(ref T a, ref T b) where T : struct {
			T temp = a;
			a = b;
			b = temp;
		}
		static PivotFieldValueItem CreateItem(PivotGridFieldValueItemsDataProviderBase dataProvider,
				 PivotFieldValueItemType type, object value, PivotFieldValueItem parentItem) {
			switch(type) {
				case PivotFieldValueItemType.Cell:
					return new PivotFieldCellValueItem(dataProvider, value, parentItem);
				case PivotFieldValueItemType.TotalCell:
					return new PivotFieldTotalCellValueItem(dataProvider, value, parentItem);
				case PivotFieldValueItemType.CustomTotalCell:
					return new PivotFieldCustomTotalCellValueItem(dataProvider, value, parentItem);
				case PivotFieldValueItemType.GrandTotalCell:
					return new PivotFieldGrandTotalCellValueItem(dataProvider, value, parentItem);
				case PivotFieldValueItemType.DataCell:
					return new PivotFieldDataCellValueItem(dataProvider, value, parentItem);
				case PivotFieldValueItemType.TopDataCell:
					return new PivotFieldTopDataCellValueItem(dataProvider, value, parentItem);
				default:
					throw new ArgumentOutOfRangeException("PivotCustomFieldValueCellsEventArgsBase.CreateItem: type");
			}
		}
		PivotVisualItemsBase visualItems;
		bool isUpdateRequired;
		public PivotCustomFieldValueCellsEventArgsBase(PivotVisualItemsBase items) {
			if(items == null || !items.IsReady)
				throw new ArgumentException("PivotCustomFieldValueCellsEventArgsBase");
			this.visualItems = items;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotCustomFieldValueCellsEventArgsBaseIsUpdateRequired")]
#endif
		public bool IsUpdateRequired { get { return isUpdateRequired; } }
		public int GetLevelCount(bool isColumn) {
			return GetItemsCreator(isColumn).LevelCount;
		}
		public GrandTotalLocation GetGrandTotalLocation(bool isColumn) {
			return GetItem(isColumn, 0).ValueType == PivotGridValueType.GrandTotal ? GrandTotalLocation.Near : GrandTotalLocation.Far;
		}
		public void SetGrandTotalLocation(bool isColumn, GrandTotalLocation location) {
			if(location == GetGrandTotalLocation(isColumn)) return;
			List<PivotFieldValueItem> grandTotals = new List<PivotFieldValueItem>();
			for(int i = GetCellCount(isColumn) - 1; i >= 0; i--) {
				PivotFieldValueItem item = GetItem(isColumn, i);
				if(item.ValueType == PivotGridValueType.GrandTotal) {
					grandTotals.Add(item);
					RemoveAt(isColumn, i);
				}
			}
			for(int i = 0; i < grandTotals.Count; i++) {
				if(location == GrandTotalLocation.Near) {
					Insert(isColumn, 0, grandTotals[i]);
				} else {
					Add(isColumn, grandTotals[grandTotals.Count - i - 1]);
				}
			}
		}
		public int GetCellCount(bool isColumn) {
			return GetItemsCreator(isColumn).Count;
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return visualItems.GetCellValue(columnIndex, rowIndex);
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotCustomFieldValueCellsEventArgsBaseColumnCount")]
#endif
		public int ColumnCount { get { return visualItems.ColumnCount; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotCustomFieldValueCellsEventArgsBaseRowCount")]
#endif
		public int RowCount { get { return visualItems.RowCount; } }
		public bool Remove(FieldValueCellBase cell) {
			if(Remove(false, cell.Item)) return true;
			return Remove(true, cell.Item);
		}
		protected bool DoCellIndexes(bool isColumn, Predicate<object[]> matchCells, Predicate<int> matchIndex) {
			List<int> lastLevelIndexes = GetColumnRowLastLevelIndexes(isColumn, true, matchCells);
			int count = GetCellCount(isColumn);
			for(int i = 0; i < count; i++) {
				PivotFieldValueItem item = GetItem(isColumn, i);
				bool condition = lastLevelIndexes.FindIndex(new Predicate<int>(delegate(int index) {
					return index == item.MinLastLevelIndex && index == item.MaxLastLevelIndex;
				})) != -1;
				if(condition) {
					if(matchIndex(i)) return true;
				}
			}
			return false;
		}
		List<int> GetColumnRowLastLevelIndexes(bool isColumn, bool firstCellOnly, Predicate<object[]> match) {
			List<int> indexes = new List<int>();
			int firstCount = visualItems.ColumnCount, secondCount = visualItems.RowCount;
			if(!isColumn) Swap<int>(ref firstCount, ref secondCount);			
			for(int i = 0; i < firstCount; i++) {
				object[] dataCellValues = new object[secondCount];
				for(int j = 0; j < secondCount; j++) {
					dataCellValues[j] = visualItems.GetCellValue(isColumn ? i : j, isColumn ? j : i);
				}
				if(match(dataCellValues)) {
					indexes.Add(i);
					if(firstCellOnly) return indexes;
				}
			}
			return indexes;
		}
		protected delegate bool IndexConditionDelegate(int index);
		protected void Split(bool isColumn, IndexConditionDelegate indexCondition, bool firstCellOnly, IList<FieldValueSplitData> cells) {
			for(int i = 0; i < GetCellCount(isColumn); ) {
				if(indexCondition(i)) {
					int index = SplitCore(isColumn, i, cells);
					if(index <= 0 || firstCellOnly) break;
					i = index;
					continue;
				}
				i++;
			}
			OnUpdate();
		}
		int SplitCore(bool isColumn, int index, IList<FieldValueSplitData> cells) {
			PivotFieldValueItem item = GetItem(isColumn, index);
			if(cells.Count == 0 || item == null || GetItem(isColumn, index + 1) == null || GetItem(isColumn, index + 1).StartLevel <= item.StartLevel)
				return -1;
			RemoveAt(isColumn, index);
			int insertIndex = index;
			foreach(FieldValueSplitData splitData in cells) {
				if(splitData.NestedCellCount <= 0) continue;
				PivotFieldValueItem newItem = CreateItem(GetDataProvider(isColumn), item.ItemType, splitData.Value, item);
				Insert(isColumn, insertIndex++, newItem);
				bool stopped;
				int count = GetNextLevelItemCount(isColumn, insertIndex, splitData.NestedCellCount, out stopped);
				if(stopped) break;
				insertIndex += count;
			}
			return insertIndex;
		}
		int GetNextLevelItemCount(bool isColumn, int index, int countLimit, out bool stopped) {
			stopped = true;
			int counter = countLimit - 1;
			PivotFieldValueItem item = GetItem(isColumn, index);
			for(int i = index + 1; i < GetCellCount(isColumn); i++) {
				if(item.StartLevel > GetItem(isColumn, i).StartLevel)
					return i - index;
				if(item.StartLevel == GetItem(isColumn, i).StartLevel) {
					if(counter == 0) {
						stopped = false;
						return i - index;
					}
					if(counter > 0) counter--;
				}
			}
			return GetCellCount(isColumn) - index;
		}
		protected PivotFieldValueItem GetItem(bool isColumn, int index) {
			return GetItemsCreator(isColumn).GetItem(index);
		}
		protected bool Remove(bool isColumn, PivotFieldValueItem item) {
			int index = IndexOf(isColumn, item);
			if(index < 0) return false;
			PivotGridOptionsViewBase options = item.Data.OptionsView;
			RemoveAt(isColumn, index);
			while(index < GetCellCount(isColumn)) {
				PivotFieldValueItem nextItem = GetItem(isColumn, index);
				if(nextItem.Level > item.Level || (options.RowTotalsLocation == PivotRowTotalsLocation.Tree && !nextItem.IsVisible && nextItem.Level == item.Level && nextItem.Value == item.Value))
					RemoveAt(isColumn, index);
				else
					break;
			}
			if(!options.IsTotalsFar(item.IsColumn, PivotGridValueType.Total) && !(item.Field != null && item.Field == item.DataField)) {
				while(--index >= 0) {
					PivotFieldValueItem upLevelItem = GetItem(isColumn, index);
					if(upLevelItem.ValueType == PivotGridValueType.Total && upLevelItem.VisibleIndex == item.VisibleIndex)
						upLevelItem.LockExpand = true;
					else
						break;
				}
			}
			EnsureIsParentRemoved(item);
			OnUpdate();
			return true;
		}
		void EnsureIsParentRemoved(PivotFieldValueItem item) {
			if(item.Parent != null && item.Parent.CellCount == 0)
				Remove(item.Parent.IsColumn, item.Parent);
		}
		PivotGridFieldValueItemsDataProviderBase GetDataProvider(bool isColumn) {
			return GetItemsCreator(isColumn).DataProvider;
		}
		PivotFieldValueItemsCreator GetItemsCreator(bool isColumn) {
			return visualItems.GetItemsCreator(isColumn);
		}
		void OnUpdate() {
			isUpdateRequired = true;
		}
		int IndexOf(bool isColumn, PivotFieldValueItem item) {
			return GetItemsCreator(isColumn).IndexOf(item);
		}
		void Insert(bool isColumn, int index, PivotFieldValueItem item) {
			GetItemsCreator(isColumn).Insert(index, item);
			OnUpdate();
		}
		void RemoveAt(bool isColumn, int index) {
			GetItemsCreator(isColumn).RemoveAt(index);
			OnUpdate();
		}
		void Add(bool isColumn, PivotFieldValueItem item) {
			GetItemsCreator(isColumn).Add(item);
			OnUpdate();
		}
	}
}
