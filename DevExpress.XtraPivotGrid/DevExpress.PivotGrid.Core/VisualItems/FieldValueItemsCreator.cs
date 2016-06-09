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
using System.Reflection;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotFieldValueItemsCreator : PivotFieldValueItemBase, ICloneable {
		PivotGridData data;
		const int Format2Flag = 0x02000000;
		public static int GetNotNullCount(IEnumerable items) {
			int count = 0;
			foreach(object item in items) {
				if(item != null) count++;
			}
			return count;
		}		
		protected FieldValueItemCollection items;
		protected FieldValueItemCollection unpagedItems;
		bool isLoadedFromStream;
		int? lastLevelUnpagedItemCount, grandTotalItemCount;
		int? unpagedItemsCount;
		internal FieldValueItemCollection GetItems() {
			return items;
		}
		internal FieldValueItemCollection GetUnpagedItems() {
			return unpagedItems;
		}
		public PivotFieldValueItemsCreator(PivotGridData data, bool isColumn)
			: base(new PivotGridFieldValueItemsDataProvider(data, isColumn)) {
			this.items = new FieldValueItemCollection();
			this.unpagedItems = new FieldValueItemCollection();
			this.isLoadedFromStream = false;
			this.lastLevelUnpagedItemCount = null;
			this.grandTotalItemCount = null;
			this.unpagedItemsCount = null;
			this.data = data;
		}
		public override PivotGridData Data { get { return data; } }
		public int Count { get { return items.Count; } }
		public PivotFieldValueItem this[int index] { get { return items[index]; } }
		public PivotFieldValueItem GetUnpagedItem(int uniqueIndex) {
			ThrowIfLoadedFromStream();
			return unpagedItems[uniqueIndex];
		}
		public int UnpagedItemsCount {
			get {
				if(unpagedItemsCount.HasValue)
					return unpagedItemsCount.Value;
				return unpagedItems.Count;
			}
		}
		public int LastLevelItemCount { get { return items.LastLevelCount; } }
		public int LastLevelUnpagedItemCount {
			get {
				if(lastLevelUnpagedItemCount.HasValue)
					return lastLevelUnpagedItemCount.Value;
				return this.unpagedItems.LastLevelCount;
			}
		}
		public int GrandTotalItemCount {
			get {
				if(grandTotalItemCount.HasValue)
					return grandTotalItemCount.Value;
				return unpagedItems.GetGrandTotalItemCount(IsTotalsFar(PivotGridValueType.GrandTotal));
			}
		}
		public bool IsTotalsFar(PivotGridValueType valueType) {
			return Data.OptionsView.IsTotalsFar(IsColumn, valueType);
		}
		public PivotFieldValueItem GetLastLevelItem(int index) { 
			return items.GetLastLevelItem(index); 
		}
		public PivotFieldValueItem GetLastLevelUnpagedItem(int index) {
			ThrowIfLoadedFromStream();
			return unpagedItems.GetLastLevelItem(index);
		}
		public int LastLevel { get { return LevelCount - 1; } }
		public new int LevelCount {
			get {
				return items.LevelCount;
			}
		}
		public int UnpagedLevelCount {
			get {
				return unpagedItems.LevelCount;
			}
		}
		public void Clear() {
			ClearCore(true);
			ClearUnpagedCore();
		}
		void ClearCore(bool clearItems) {
			if(clearItems) this.items.Clear();
			this.grandTotalItemCount = null;
			this.isLoadedFromStream = false;
		}
		void ClearUnpagedCore() {
			if(this.items == this.unpagedItems)
				this.unpagedItems = new FieldValueItemCollection();
			else
				this.unpagedItems.Clear();
			this.unpagedItemsCount = null;
			this.lastLevelUnpagedItemCount = null;
		}
		internal PivotFieldValueItem GetItem(int index) {
			return (index >= 0 && index < Count) ? items[index] : null;
		}
		internal int IndexOf(PivotFieldValueItem item) {
			return items.IndexOf(item);
		}
		internal void Insert(int index, PivotFieldValueItem item) {
			items.Insert(index, item);
		}
		internal void RemoveAt(int index) {
			PivotFieldValueItem item = GetItem(index);
			if(item == null) return;
			if(item.Parent != null)
				item.Parent.RemoveCell(item);
			items.RemoveAt(index);
		}
		internal void Add(PivotFieldValueItem item) {
			items.Add(item);
		}
		public void CreateItems() {
			CreateItems(false);
		}
		internal void CreateItems(bool usePreviouslyItems) {
			CreateItems(usePreviouslyItems, VisualItemsPagingOptions.DefaultDisabled);
		}
		public void CreateItems(int startValueItem, int maxFieldValueItemCount) {
			CreateItems(false, new VisualItemsPagingOptions { Enabled = true, Start = startValueItem, Count = maxFieldValueItemCount});
		}
		internal void CreateItems(bool usePreviously, VisualItemsPagingOptions opts) {
			ClearCore(!usePreviously);
			if(!usePreviously) {
				FieldValueItemsGenerator generator = new FieldValueItemsGenerator(DataProvider, IsColumn);
				generator.Generate(items, new FieldValueGenerationParams(Data, IsColumn));
			}
			HideEmptyVariationItems();
			new ItemsCollectionActions.SetIndexesAction(true).Apply(items);
			new ItemsCollectionActions.SetLastLevelIndexesAction().Apply(items);
			ApplyPaging(usePreviously, opts);
		}
		internal void ApplyPaging(bool usePreviouslyItems, VisualItemsPagingOptions opts) {
			ClearUnpagedCore();
			bool allowPaging = opts.Enabled && opts.Count > 0;
			if(allowPaging) {
				unpagedItems.AddRange(items);
				unpagedItems.CreateLastLevelItems();
				List<PivotFieldValueItem> pageItems = FindPageItems(opts.Start, opts.Count, CalcLastLevelItemsCount(), opts.ShowGrandTotalsOnEachPage);
				items.Clear();
				items.AddRange(pageItems);
				new ItemsCollectionActions.SetIndexesAction(false).Apply(items);
				foreach(PivotFieldValueItem item in items)
					item.SaveCellsAsUnpagedCells();
			}
			else
				unpagedItems = items;
			items.CreateLastLevelItems();
			new ItemsCollectionActions.SetChildrenAction(usePreviouslyItems).Apply(items);
			if(LastLevelItemCount > items.Count)
				throw new Exception("LastLevelItemCount > items.Count");
		}
		void HideEmptyVariationItems() {
			if(!IsDataLocatedInThisArea) return;
			List<int> variationDataIndexes = GetVariationDataIndexes();
			if(variationDataIndexes.Count == 0)
				return;
			bool hasLastLevelItems = false;
			Dictionary<int, PivotFieldValueItem> removedItems = new Dictionary<int, PivotFieldValueItem>();
			for(int i = items.Count - 1; i >= 0; i--) {
				if(Data.GetPrevIndex(IsColumn, items[i].VisibleIndex) >= 0 || items[i].ValueType == PivotGridValueType.GrandTotal) {
					if(IsLastLevelItem(items[i])) hasLastLevelItems = true;
					continue;
				}
				if(items[i].EndLevel == LastLevel && variationDataIndexes.Contains(items[i].DataIndex) ||
					 items[i].EndLevel == LastLevel - 1 && (i == items.Count - 1 || items[i + 1].StartLevel < LastLevel)) {
					removedItems.Add(i, items[i]);
					items.Remove(items[i]);
				} else {
					if(IsLastLevelItem(items[i])) hasLastLevelItems = true;
				}
			}
			List<int> sortedIndexes = new List<int>();
			sortedIndexes.AddRange(removedItems.Keys);
			sortedIndexes.Sort();
			if(!hasLastLevelItems) {
				foreach(int index in sortedIndexes) {
					items.Insert(index, removedItems[index]);
				}
			}
		}
		List<int> GetVariationDataIndexes() {
			List<int> res = new List<int>();
			for(int i = 0; i < Data.DataFieldCount; i++) {
				if(Data.GetFieldByArea(PivotArea.DataArea, i).CanHideEmptyVariationItems)
					res.Add(i);
			}
			return res;
		}
		List<PivotFieldValueItem> FindPageItems(int startValueItem, int maxFieldValueItemCount, int lastLevelItemsCount, bool showGrandTotalOnEachPage) {
			bool isLastPage = (lastLevelItemsCount == 0) ||
					(startValueItem < lastLevelItemsCount && maxFieldValueItemCount >= lastLevelItemsCount - startValueItem),
				showGrandTotals = showGrandTotalOnEachPage || isLastPage;
			if(isLastPage)
				maxFieldValueItemCount = Math.Max(maxFieldValueItemCount, lastLevelItemsCount - startValueItem);
			bool leftCutted = startValueItem > 0,
				rightCutted = startValueItem + maxFieldValueItemCount < lastLevelItemsCount;
			List<PivotFieldValueItem> newItems = new List<PivotFieldValueItem>(maxFieldValueItemCount),
				grandTotals = new List<PivotFieldValueItem>(),
				parents = new List<PivotFieldValueItem>(),
				lastLevelItems = new List<PivotFieldValueItem>(lastLevelItemsCount);
			SplitItems(lastLevelItems, grandTotals, parents);
			if(startValueItem < lastLevelItems.Count) {
				lastLevelItems = lastLevelItems.GetRange(startValueItem,
					Math.Min(lastLevelItems.Count - startValueItem, maxFieldValueItemCount));
				AddWithParents(newItems, lastLevelItems, parents);
			}
			if(showGrandTotals) {
				if(this.Count > 0 && this[0].ValueType == PivotGridValueType.GrandTotal)
					newItems.InsertRange(0, grandTotals);
				else
					newItems.AddRange(grandTotals);
			}
			return newItems;
		}
		void SplitItems(List<PivotFieldValueItem> lastLevelItems, List<PivotFieldValueItem> grandTotals, List<PivotFieldValueItem> parents) {
			for(int i = 0; i < Count; i++) {
				switch(this[i].ValueType) {
					case PivotGridValueType.GrandTotal:
						grandTotals.Add(this[i]);
						break;
					case PivotGridValueType.Total:
					case PivotGridValueType.CustomTotal:
					case PivotGridValueType.Value:
						if(IsLastLevelItem(this[i]))
							lastLevelItems.Add(this[i]);
						else
							parents.Add(this[i]);
						break;
				}
			}
		}
		void AddWithParents(List<PivotFieldValueItem> newItems, List<PivotFieldValueItem> lastLevelItems, List<PivotFieldValueItem> parents) {
			for(int i = 0; i < lastLevelItems.Count; i++) {
				int index = newItems.Count;
				PivotFieldValueItem parent = GetParentItem(lastLevelItems[i]);
				while(parent != null && newItems.IndexOf(parent) == -1) {
					newItems.Insert(index, parent);
					parent = GetParentItem(parent);
				}
				newItems.Add(lastLevelItems[i]);
			}
		}
		int CalcLastLevelItemsCount() {
			int counter = 0;
			for(int i = 0; i < Count; i++) {
				counter += IsLastLevelItem(this[i]) && this[i].ValueType != PivotGridValueType.GrandTotal ? 1 : 0;
			}
			return counter;
		}
		public object[] GetItemValues(int uniqueIndex) {
			if(uniqueIndex < 0 || uniqueIndex >= UnpagedItemsCount) return null;
			return GetItemValues(GetUnpagedItem(uniqueIndex));
		}
		public object[] GetItemValues(PivotFieldValueItem item) {
			return unpagedItems.GetItemValues(item);
		}
		public object[] GetItemKey(PivotFieldValueItem item) {
			return items.GetItemKey(item);
		}
		public PivotFieldValueItem GetItemByValues(object[] values) {
			return unpagedItems.GetItemByValues(values);
		}
		public IList<PivotFieldValueItem> GetItemsByValues(object[] values, PivotFieldItemBase dataField) {
			return unpagedItems.GetItemsByValues(values, dataField);
		}
		public PivotFieldValueItem GetRootParentItem(PivotFieldValueItem item) {
			return items.GetParentItem(item, 0);
		}
		public PivotFieldValueItem GetParentItem(PivotFieldValueItem item) {
			return items.GetParentItem(item, item.StartLevel - 1);
		}
		public PivotFieldValueItem GetParentUnpagedItem(PivotFieldValueItem item) {
			ThrowIfLoadedFromStream();
			return unpagedItems.GetParentItem(item, item.StartLevel - 1, true);
		}
		public new void SaveToStream(TypedBinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write(Format2Flag);
			DataProvider.SaveToStream(writer);
			writer.Write(items.Count);
			for(int i = 0; i < items.Count; i++) {
				byte type = (byte)items[i].ItemType;
				writer.Write(type);
				items[i].SaveToStream(writer);
			}
			writer.Write(LastLevelUnpagedItemCount);
			writer.Write(UnpagedItemsCount);
			writer.Write(GrandTotalItemCount);
		}
		public void LoadFromStream(Stream stream) {
			Clear();
			TypedBinaryReader reader = TypedBinaryReader.CreateReader(stream, Data.OptionsData.CustomObjectConverter);
			base.LoadFromStream(reader);
			int format = reader.ReadInt32() & 0x0F000000;
			switch(format) {
				case Format2Flag:
					LoadFormat2FromStream(reader);
					break;
				default:
					throw new ArgumentException("Unsupported format!");
			}
			this.isLoadedFromStream = true;
		}
		protected void LoadFormat2FromStream(TypedBinaryReader reader) {
			DataProvider = new PivotGridFieldValueItemsStreamDataProvider(Data, IsColumn);
			((PivotGridFieldValueItemsStreamDataProvider)DataProvider).LoadFromStream(reader);
			int count = reader.ReadInt32();
			for(int i = 0; i < count; i++) {
				PivotFieldValueItemType type = (PivotFieldValueItemType)reader.ReadByte();
				PivotFieldValueItem item = PivotFieldValueItem.LoadItem(type, DataProvider, reader);
				items.Add(item);
			}
			lastLevelUnpagedItemCount = reader.ReadInt32();
			unpagedItemsCount = reader.ReadInt32();
			grandTotalItemCount = reader.ReadInt32();
			new ItemsCollectionActions.SetIndexesAction(false).Apply(items);
			new ItemsCollectionActions.SetChildrenAction(false).Apply(items);
			items.CreateLastLevelItems();
		}
		protected bool IsLastLevelItem(PivotFieldValueItem item) {
			return item.EndLevel == LastLevel;
		}		
		void ThrowIfLoadedFromStream() {
			if(isLoadedFromStream)
				throw new NotLoadedValuesException();
		}
		public PivotFieldValueItemsCreatorClone Clone() {
			PivotFieldValueItemsCreatorClone clone = new PivotFieldValueItemsCreatorClone(Data, IsColumn);
			clone.items = items.Clone();
			clone.unpagedItems = unpagedItems.Clone();
			clone.grandTotalItemCount = grandTotalItemCount;
			clone.isLoadedFromStream = isLoadedFromStream;
			clone.lastLevelUnpagedItemCount = lastLevelUnpagedItemCount;
			clone.unpagedItemsCount = unpagedItemsCount;
			return clone;
		}
		#region ICloneable Members
		object ICloneable.Clone() {
			return this.Clone();
		}
		#endregion
	}
	public class PivotFieldValueItemsCreatorClone : PivotFieldValueItemsCreator {
		public PivotFieldValueItemsCreatorClone(PivotGridData data, bool isColumn)
			: base(data, isColumn) { }
		internal FieldValueItemCollection Items { get { return items; } }
		internal FieldValueItemCollection UnpagedItems { get { return unpagedItems; } }
		public override PivotGridFieldValueItemsDataProviderBase DataProvider {
			get {
				return null;
			}
			protected set {
				throw new Exception("Incorrect clone items creator usage");
			}
		}
	}
}
