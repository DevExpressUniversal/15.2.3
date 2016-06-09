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
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPivotGrid.Data {
	public class FieldValueItemCollection : BaseCollection<PivotFieldValueItem>, ICloneable {
		readonly List<PivotFieldValueItem> lastLevelItems;
		int? levelCount;
		PivotFieldValueItem grandTotalItem;
		public FieldValueItemCollection() {
			this.lastLevelItems = new List<PivotFieldValueItem>();
		}
		public int LastLevelCount { get { return lastLevelItems.Count; } }
		public int LevelCount {
			get {
				if(levelCount == null)
					levelCount = GetLevelCountCore();
				return levelCount.Value;
			}
		}
		public PivotFieldValueItem GrandTotalItem {
			get {
				if(grandTotalItem == null)
					grandTotalItem = GetGrandTotalItemCore();
				return grandTotalItem;
			}
		}
		int GetLevelCountCore() {
			int maxLevel = 0;
			for(int i = 0; i < Count; i++) {
				int endLevel = this[i].EndLevel;
				if(endLevel > maxLevel)
					maxLevel = endLevel;
			}
			return maxLevel + 1;
		}
		PivotFieldValueItem GetGrandTotalItemCore() {
			if(Count == 0) return null;
			for(int i = Count - 1; i >= 0; i--) {
				PivotFieldValueItem valueItem = this[i];
				if(valueItem.ValueType == PivotGridValueType.GrandTotal && valueItem.StartLevel == 0)
					return valueItem;
			}
			return null;
		}
		protected override void OnInsertComplete(int index, PivotFieldValueItem value) {
			base.OnInsertComplete(index, value);
			ResetCaches();
		}
		protected override void OnInsertComplete(int index, IEnumerable<PivotFieldValueItem> collection) {
			base.OnInsertComplete(index, collection);
			ResetCaches();
		}
		protected override void OnRemoveComplete(int index, PivotFieldValueItem value) {
			base.OnRemoveComplete(index, value);
			ResetCaches();
		}
		protected override void OnReplaceComplete(int index, PivotFieldValueItem oldItem, PivotFieldValueItem newItem) {
			base.OnReplaceComplete(index, oldItem, newItem);
			ResetCaches();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			ResetCaches();
		}
		protected void ResetCaches() {
			this.levelCount = null;
			this.grandTotalItem = null;
		}
		public new void Clear() {
			base.Clear();
			this.lastLevelItems.Clear();
		}
		public void CreateLastLevelItems() {
			int lastLevel = LevelCount - 1;
			lastLevelItems.Clear();
			for(int i = 0; i < Count; i++) {
				PivotFieldValueItem item = this[i];
				item.IsLastFieldLevel = (item.EndLevel == lastLevel);
				if(item.IsLastFieldLevel) {
					lastLevelItems.Add(item);
				}
			}
			if(Count > 0 && lastLevelItems.Count == 0)
				throw new ArgumentException("Invalid lastLevel");
		}
		public PivotFieldValueItem GetLastLevelItem(int index) {
			return lastLevelItems[index];
		}
		public int GetGrandTotalItemCount(bool isTotalsFar) {
			int count = 0;
			if(isTotalsFar) {
				for(int i = LastLevelCount - 1; i >= 0; i--) {
					if(GetLastLevelItem(i).ValueType == PivotGridValueType.GrandTotal) count++;
					else break;
				}
			} else {
				for(int i = 0; i < LastLevelCount; i++) {
					if(GetLastLevelItem(i).ValueType == PivotGridValueType.GrandTotal) count++;
					else break;
				}
			}
			return count;
		}
		public PivotFieldValueItem GetParentItem(PivotFieldValueItem item, int prevLevel) {
			return GetParentItem(item, prevLevel, false);
		}
		public PivotFieldValueItem GetParentItem(PivotFieldValueItem item, int prevLevel, bool useUniqueIndex) {
			if(item == null || item.StartLevel == 0) 
				return null;
			if(item.Parent != null && item.Parent.ContainsLevel(prevLevel))
				return item.Parent;
			for(int i = useUniqueIndex ? item.UniqueIndex : item.Index; i >= 0; i--) {
				if(this[i].ContainsLevel(prevLevel)) 
					return this[i];
			}
			return null;
		}
		public object[] GetItemValues(PivotFieldValueItem item) {
			if(item == null) return null;
			int levelCount = LevelCount;
			PivotFieldValueItem[] items = new PivotFieldValueItem[levelCount];
			while(item != null) {
				if(!(item is PivotFieldTopDataCellValueItem) && !(item is PivotFieldDataCellValueItem))
					items[item.StartLevel] = item;
				item = GetParentItem(item, item.StartLevel - 1);
			}
			object[] values = new object[PivotFieldValueItemsCreator.GetNotNullCount(items)];
			int index = 0;
			for(int i = 0; i < items.Length; i++) {
				if(items[i] == null) continue;
				values[index++] = items[i].Value;
			}
			return values;
		}
		public object[] GetItemKey(PivotFieldValueItem item) {
			if(item == null) return null;
			PivotFieldValueItem[] items = new PivotFieldValueItem[LevelCount];
			while(item != null) {
				items[item.StartLevel] = item;
				item = GetParentItem(item, item.StartLevel - 1);
			}
			object[] values = new object[PivotFieldValueItemsCreator.GetNotNullCount(items)];
			int index = 0;
			for(int i = 0; i < items.Length; i++) {
				PivotFieldValueItem itm = items[i];
				if(itm == null) continue;
				object value = itm.Value;
				if(value == null)
					value = itm.DisplayText;
				values[index++] = value;
			}
			return values;
		}
		public PivotFieldValueItem GetItemByValues(object[] values) {
			if(Count == 0) return null;
			if(values == null || values.Length == 0)
				return GrandTotalItem;
			int currentLevel = 0;
			PivotFieldValueItem founded = null;
			bool isNotLastLevelValue = values.Length < LevelCount;
			for(int i = 0; i < Count; i++) {
				PivotFieldValueItem valueItem = this[i];
				if(valueItem.StartLevel < currentLevel)
					currentLevel = 0;
				if(valueItem.StartLevel != currentLevel || !object.Equals(valueItem.Value, values[currentLevel]))
					continue;
				if(currentLevel == values.Length - 1) {
					if((valueItem.IsCollapsed || valueItem.IsTotal) == isNotLastLevelValue || valueItem.IsLastFieldLevel)
						return valueItem;
					else {
						founded = valueItem;
						continue;
				}
				}
				else {
					if(object.Equals(valueItem.Value, values[currentLevel]))
						currentLevel++;
					if(currentLevel > values.Length - 1)
						return null;
				}
			}
			return founded;
		}
		public IList<PivotFieldValueItem> GetItemsByValues(object[] values, PivotFieldItemBase dataField) {
			List<PivotFieldValueItem> result = new List<PivotFieldValueItem>();
			for(int i = 0; i < Count; i++) {
				PivotFieldValueItem item = this[i];
				if(dataField != null && !IsItemBelongToThisDataField(item, dataField))
					continue;
				object[] currentValues = GetItemValues(item);
				if(AreValuesEqual(currentValues, values))
					result.Add(this[i]);
			}
			return result;
		}
		bool IsItemBelongToThisDataField(PivotFieldValueItem item, PivotFieldItemBase dataField) {
			Guard.ArgumentNotNull(dataField, "dataField");
			Guard.ArgumentNotNull(item, "item");
			PivotFieldValueItem currentItem = item;
			while(item != null) {
				if(item is PivotFieldTopDataCellValueItem || item is PivotFieldDataCellValueItem) {
					if(object.ReferenceEquals(item.DataField, dataField))
						return true;
				}
				item = GetParentItem(item, item.StartLevel - 1);
			}
			return false;
		}
		bool AreValuesEqual(object[] values1, object[] values2) {
			if(values1 == null && values2 == null)
				return false;
			if(values1.Length != values2.Length)
				return false;
			for(int i = 0; i < values1.Length; i++) {
				if(!object.Equals(values1[i], values2[i]))
					return false;
			}
			return true;
		}
		public FieldValueItemCollection Clone() {
			FieldValueItemCollection clone = new FieldValueItemCollection();
			for(int i = 0; i < Count; i++) {
				PivotFieldValueItem item = this[i];
				item.SetCloneText();
				clone.Add(item);
			}
			for(int i = 0; i < lastLevelItems.Count; i++) {
				PivotFieldValueItem item = lastLevelItems[i];
				item.SetCloneText();
				clone.lastLevelItems.Add(item);
			}
			clone.grandTotalItem = grandTotalItem;
			clone.levelCount = levelCount;
			return clone;
		}
		#region ICloneable Members
		object ICloneable.Clone() {
			return this.Clone();
		}
		#endregion
	}
	namespace ItemsCollectionActions {
		public abstract class ItemsCollectionAction {
			public abstract void Apply(FieldValueItemCollection collection);
		}
		public abstract class SimpleItemsCollectionAction : ItemsCollectionAction {
			FieldValueItemCollection collection;
			protected FieldValueItemCollection Collection { get { return collection; } }
			public override void Apply(FieldValueItemCollection collection) {
				this.collection = collection;
				try {
					OnBeforeApply();
					collection.ForEach(DoAction);
				} finally {
					this.collection = null;
				}
			}
			protected virtual void OnBeforeApply() { }
			protected abstract void DoAction(PivotFieldValueItem item);
		}
		public class SetChildrenAction : SimpleItemsCollectionAction {
			readonly bool clearChildren;
			public SetChildrenAction(bool clearChildren) {
				this.clearChildren = clearChildren;
			}
			protected override void OnBeforeApply() {
				base.OnBeforeApply();
				if(clearChildren)
					foreach(PivotFieldValueItem cell in Collection)
						cell.ClearCells();
				SetParents();
			}
			void SetParents() {
				PivotFieldValueItem[] parents = new PivotFieldValueItem[Collection.LevelCount];
				for(int i = 0; i < Collection.Count; i++) {
					PivotFieldValueItem cell = Collection[i];
					int parentLevel = cell.StartLevel - 1;
					if(parentLevel >= 0 && parentLevel < parents.Length)
						cell.Parent = parents[parentLevel];
					for(int j = cell.StartLevel; j <= cell.EndLevel; j++) {
						parents[j] = cell;
					}
				}
			}
			protected override void DoAction(PivotFieldValueItem item) {
				PivotFieldValueItem parent = item.Parent != null ? item.Parent : Collection.GetParentItem(item, item.StartLevel - 1);
				if(parent != null) {
					parent.AddCell(item);
				}
			}
		}
		public class SetIndexesAction : ItemsCollectionAction {
			readonly bool setUniqueIndex;
			public SetIndexesAction(bool setUniqueIndex) {
				this.setUniqueIndex = setUniqueIndex;
			}
			public override void Apply(FieldValueItemCollection collection) {
				for(int i = 0; i < collection.Count; i++) {
					collection[i].SetIndex(i);
					if(this.setUniqueIndex)
						collection[i].UniqueIndex = i;
				}
			}
		}
		public class SetLastLevelIndexesAction : ItemsCollectionAction {
			public override void Apply(FieldValueItemCollection collection) {
				int lastLevelIndex;
				int lastLevel = collection.LevelCount - 1;
				for(int curLevel = lastLevel; curLevel >= 0; curLevel--) {
					PivotFieldValueItem lastItem = null;
					lastLevelIndex = -1;
					collection.ForEach(delegate(PivotFieldValueItem item) {
						if(item.ContainsLevel(curLevel) && curLevel != lastLevel) {
							if(lastItem != null)
								lastItem.MaxLastLevelIndex = lastLevelIndex;
							if(item.EndLevel == lastLevel)
								lastItem = null;
							else {
								item.MinLastLevelIndex = lastLevelIndex + 1;
								lastItem = item;
							}
						}
						if(item.EndLevel == lastLevel) {
							lastLevelIndex++;
							if(curLevel == lastLevel)
								item.MaxLastLevelIndex = item.MinLastLevelIndex = lastLevelIndex;
						}
					});
					if(lastItem != null)
						lastItem.MaxLastLevelIndex = lastLevelIndex;
				}
			}
		}
	}
}
