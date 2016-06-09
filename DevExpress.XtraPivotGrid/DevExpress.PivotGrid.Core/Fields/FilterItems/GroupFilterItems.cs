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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils.Controls;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGroupFilterItem : PivotGridFilterItem {
		int level;
		IFilterItem parent;
		public PivotGroupFilterItem(IFilterItem parent, int level, object value, string text, bool? isChecked)
			: this(parent, level, value, text, isChecked, true) {
		}
		public PivotGroupFilterItem(IFilterItem parent, int level, object value, string text, bool? isChecked, bool isVisible)
			: base(value, text, isChecked, isVisible) {
			this.parent = parent;
			this.level = level;
		}
		public override int Level { get { return level; } }
		protected internal object[] ParentValues {
			get {
				if(Level == 0)
					return null;
				object[] parentValues = new object[Level];
				Parent.AddParentValue(parentValues);
				return parentValues;
			}
		}
		public PivotGroupFilterItem Parent { get { return (PivotGroupFilterItem)parent; } set { if(Level == 0) parent = value; } }
		public object[] GetValuesBranch() {
			List<object> branch = new List<object>();
			branch.Add(this.Value);
			PivotGroupFilterItem ansector = Parent;
			while(ansector != null) {
				branch.Insert(0, ansector.Value);
				ansector = ansector.Parent;
			}
			return branch.ToArray();
		}
		public PivotGroupFilterItem[] GetBranch() {
			List<PivotGroupFilterItem> branch = new List<PivotGroupFilterItem>();
			branch.Add(this);
			PivotGroupFilterItem ansector = Parent;
			while(ansector != null) {
				branch.Insert(0, ansector);
				ansector = ansector.Parent;
			}
			return branch.ToArray();
		}
		void AddParentValue(object[] parentValues) {
			parentValues[Level] = Value;
			if(Parent != null && Level != 0)
				Parent.AddParentValue(parentValues);
		}
	}
	public interface IGroupFilter {
		bool HasFilter { get; }
		PivotGroupFilterValuesCollection Values { get; }
		PivotFilterType FilterType { get; }
		int GetOLAPLevel(int i);
		string GetFieldName(int i);
		int LevelCount { get; }
		string PersistentString { get; }
	}
	public class PivotGroupFilterItems : PivotFilterItemsBase, IGroupFilter, IFilterValues {
		public PivotGroupFilterItems(PivotGridData data, PivotGridFieldBase field)
			: this(data, field, false) {
		}
		public PivotGroupFilterItems(PivotGridData data, PivotGridFieldBase field, bool radioMode)
			: base(data, field, radioMode) {
		}
		public PivotGroupFilterItems(PivotGridData data, PivotGridFieldBase field, bool radioMode, bool deferUpdates)
			: base(data, field, radioMode, false, deferUpdates) {
		}
		public new PivotGroupFilterItem this[int index] {
			get { return (PivotGroupFilterItem)base[index]; }
			set { base[index] = value; }
		}
		public override IEnumerable<PivotGridFilterItem> VisibleItems {
			get {
				foreach(PivotGroupFilterItem item in this)
					if(item.IsVisible && item.Level == 0)
						yield return item;
			}
		}
		protected override bool ActualShowNewValues { get { return Group.ShowNewValues; } }
		public override IFilterValues FilterValues { get { return GroupFilterValues; } }
		PivotGroupFilterValues GroupFilterValues { get { return Group.FilterValues; } }
		public override string PersistentString {
			get {
				return DevExpress.Data.PivotGrid.PivotGridSerializeHelper.ToBase64String(writer => {
					Type[] levelTypes = Group.GetFieldFilterValuesTypes();
					foreach(Type fieldType in levelTypes)
						writer.WriteType(fieldType);
					writer.Write(Count);
					foreach(PivotGroupFilterItem item in this) {
						int level = item.Level;
						writer.Write(level);
						if(levelTypes[level] == typeof(object))
							writer.WriteTypedObject(item.Value);
						else
							writer.WriteObject(item.Value);
						writer.Write(GetCheckedChar(item.IsChecked));
						writer.Write(item.IsVisible);
					}
				}
				, Data.OptionsData.CustomObjectConverter);
			}
			set {
				if(string.IsNullOrEmpty(value))
					return;
				MemoryStream stream = new MemoryStream(Convert.FromBase64String(value));
				TypedBinaryReader reader = TypedBinaryReader.CreateReader(stream, Data.OptionsData.CustomObjectConverter);
				ClearItems();
				PivotGroupFilterItem[] parentItems = new PivotGroupFilterItem[LevelCount + 1];
				Type[] levelTypes = new Type[LevelCount];
				for(int i = 0; i < levelTypes.Length; i++)
					levelTypes[i] = reader.ReadType();
				int previousLevel = 0;
				int count = reader.ReadInt32();
				for(int i = 0; i < count; i++) {
					int level = reader.ReadInt32();
					Type columnType = levelTypes[level];
					object val = columnType == typeof(object) ? reader.ReadTypedObject() : reader.ReadObject(columnType);
					Add(parentItems[level], level, val, GetDisplayTextCore(val, level, false), GetIsChecked(reader.ReadChar()), reader.ReadBoolean());
					if(level < previousLevel)
						ZeroArray(parentItems, level + 1);
					parentItems[level + 1] = this[Count - 1];
					previousLevel = level;
				}
				reader.Dispose();
				stream.Dispose();
				AddChecksRange();
			}
		}
		void ZeroArray(object[] array, int startIndex) {
			for(int i = startIndex; i < array.Length; i++)
				array[i] = null;
		}
		protected override bool ApplyFilterCore(bool allowOnChanged) {
			return GroupFilterValues.SetValuesAsync(GetFilteredValues(), GetFilterType(), allowOnChanged);
		}
		PivotFilterType GetFilterType() {
			return ShowNewValues ? PivotFilterType.Excluded : PivotFilterType.Included;
		}
		protected override void InvertCheckStateCore(bool visibleItemsOnly) {
			for(int i = 0; i < Count; i++) {
				if(visibleItemsOnly && !this[i].IsVisible)
					continue;
				if(this[i].IsChecked == null)
					LoadValues(this[i].GetBranch());
				else
					this[i].IsChecked = !this[i].IsChecked.Value;
			}
		}
		Collection<PivotGridFilterItem> GetSourceGroupFilterItems() {
			if(ShowNewValues == ActualShowNewValues)
				return AllItems;
			PivotGroupFilterItems sourceItems = new PivotGroupFilterItems(Data, Field);
			foreach(PivotGroupFilterItem item in AllItems)
				sourceItems.AllItems.Add(item);
			sourceItems.AddChecksRange();
			for(int i = 0; i < sourceItems.Count; i++) {
				if(!sourceItems[i].IsChecked.HasValue && !HasChildren(sourceItems.AllItems, i))
					sourceItems.LoadValues(sourceItems[i].GetBranch());
			}
			return sourceItems.AllItems;
		}
		protected PivotGroupFilterValuesCollection GetFilteredValues() {
			return GetFilteredValues(!ShowNewValues);
		}
		protected PivotGroupFilterValuesCollection GetFilteredValues(bool includeState) {
			return GetFilteredValuesCore(GetSourceGroupFilterItems(), GroupFilterValues.Clone(), includeState);
		}
		static PivotGroupFilterValuesCollection GetFilteredValuesCore(Collection<PivotGridFilterItem> items, PivotGroupFilterValues filterValues, bool includeState) {
			PivotGroupFilterValues newFilter = new PivotGroupFilterValues(filterValues.Group, filterValues.FilterType);
			newFilter.BeginUpdate();
			filterValues.BeginUpdate();
			try {
				PivotGroupFilterValuesCollection levelValues = newFilter.Values;
				PivotGroupFilterValue filterValue = null;
				int previousLevel = 0;
				for(int index = 0; index < items.Count; index++) {
					PivotGroupFilterItem item = (PivotGroupFilterItem)items[index];
					if(index != 0 && item.Level > previousLevel && items[index - 1].IsChecked.HasValue)
						continue;
					if(index != 0 && item.Level != previousLevel)
						levelValues = filterValue.GetLevelValues(item.Level - 1);
					if(!item.IsChecked.HasValue && !HasChildren(items, index)) {
						PivotGroupFilterValuesCollection values = item.Level == 0 ? filterValues.Values : filterValues.FindFilterValue(item.ParentValues).ChildValues;
						filterValue = values[item.Value];
						values.Remove(filterValue);
						levelValues.Add(filterValue);
					} else if(!item.IsChecked.HasValue || item.IsChecked == includeState)
						filterValue = levelValues.Add(item.Value);
					previousLevel = item.Level;
				}
			} finally {
				newFilter.CancelUpdate();
				filterValues.CancelUpdate();
			}
			return newFilter.Values;
		}
		static bool HasChildren(Collection<PivotGridFilterItem> items, int itemIndex) {
			int nextIndex = itemIndex + 1;
			return nextIndex < items.Count && items[itemIndex].Level < items[nextIndex].Level;
		}
		protected override PivotGridFilterItem CreatePivotFilterItem(IFilterItem parent, int level, object value, string text, bool? isChecked, bool isVisible) {
			return new PivotGroupFilterItem(parent, level, value, text, isChecked, isVisible);
		}
		protected override string GetShowBlanksItemCaptionCore() {
			return PivotGridLocalizer.GetString(PivotGridStringId.FilterBlank);
		}
		protected override void Add(object value, string text, bool? isChecked, bool isVisible) {
			Add(null, 0, value, text, isChecked, isVisible);
		}
		protected override void AddValues() {
			List<object> values = Data.GetSortedUniqueGroupValues(Group, null);
			AddValuesCore(values);
		}
		protected override void AddValuesAsync(AsyncCompletedHandler asyncCompleted) {
			DataAsync.GetSortedUniqueGroupValuesAsync(Group, null, false, delegate(AsyncOperationResult result) {
				AddValuesCore(result.Value as IList);
				if(asyncCompleted != null)
					asyncCompleted.Invoke(result);
			});
		}
		protected override void UpdateBlankItemCaptions() {
			foreach(PivotGridFilterItem item in AllItems) {
				if(item.IsBlank)
					item.Text = GetDisplayTextCore(null, item.Level, true);
			}
		}
		public List<object> GetChildValues(object[] branchItems) {
			if(branchItems == null || branchItems.Length == 0)
				return null;
			PivotGridFilterItem parentItem = (PivotGridFilterItem)branchItems[branchItems.Length - 1];
			int index = IndexOf(parentItem);
			if(index >= Count || !HasChildren(this, index))
				return null;
			List<object> childValues = new List<object>();
			while(++index < Count && this[index].Level != parentItem.Level) {
				PivotGroupFilterItem item = this[index];
				if(item.Level - 1 == parentItem.Level)
					childValues.Add(item);
			}
			return childValues;
		}
		public override List<object> LoadValues(object[] branchItems) {
			if(branchItems == null || branchItems.Length == 0)
				return null;
			PivotGroupFilterItem parentItem = (PivotGroupFilterItem)branchItems[branchItems.Length - 1];
			return InsertValues(parentItem);
		}
		public override void LoadValuesAsync(object[] branchItems, AsyncCompletedHandler asyncCompleted) {
			if(branchItems == null || branchItems.Length == 0) {
				asyncCompleted(AsyncOperationResult.Create(null, null));
				return;
			}
			DataAsync.SetLoadingPanelType(PivotLoadingPanelType.FilterPopupLoadingPanel);
			PivotGroupFilterItem parentItem = (PivotGroupFilterItem)branchItems[branchItems.Length - 1];
			InsertValuesAsync(parentItem, asyncCompleted);
		}
		List<object> InsertValues(PivotGroupFilterItem parent) {
			int startIndex = IndexOf(parent);
			if(startIndex < 0 || HasChildren(AllItems, startIndex))
				return null;
			startIndex++;
			List<object> values = Data.GetSortedUniqueGroupValues(Group, parent.GetValuesBranch());
			return InsertValuesCore(values, parent, startIndex);
		}
		void InsertValuesAsync(PivotGroupFilterItem parent, AsyncCompletedHandler asyncCompleted) {
			List<object> values;
			DataAsync.GetSortedUniqueGroupValuesAsync(Group, parent.GetValuesBranch(), false, result => {
				int startIndex = IndexOf(parent);
				if(startIndex < 0 || HasChildren(AllItems, startIndex))
					return;
				startIndex++;
				values = (List<object>)result.Value;
				List<object> inserted = InsertValuesCore(values, parent, startIndex);
				AsyncOperationResult insertionResult = AsyncOperationResult.Create(inserted, result.Exception);
				DataAsync.SetLoadingPanelType(PivotLoadingPanelType.MainLoadingPanel);
				asyncCompleted(insertionResult);
			});
		}
		List<object> InsertValuesCore(List<object> values, PivotGroupFilterItem parent, int startIndex) {
			if(values == null)
				return new List<object>();
			object[] parentValues = parent.GetValuesBranch();
			int level = (parentValues != null) ? parentValues.Length : 0,
				index = startIndex, count = values.Count;
			List<object> items = new List<object>(count);
			foreach(object value in values) {
				PivotGroupFilterItem item = (PivotGroupFilterItem)CreatePivotFilterItem(parent, level, value, GetDisplayTextCore(value, level, true), IsItemChecked(parent, parentValues, value), true);
				this.Insert(index++, item);
				items.Add(item);
			}
			InsertChecksRange(startIndex, count);
			return items;
		}
		void AddValuesCore(IList values) {
			if(values == null)
				return;
			foreach(object value in values) {
				Add(value, GetDisplayTextCore(value, 0, true), IsItemChecked(null, null, value), true);
			}
		}
		void Add(IFilterItem parent, int level, object value, string text, bool? isChecked, bool isVisible) {
			PivotGroupFilterItem item = (PivotGroupFilterItem)CreatePivotFilterItem(parent, level, value, text, isChecked, isVisible);
			this.Add(item);
		}
		protected virtual bool? IsItemChecked(IFilterItem parent, object[] parentValues, object value) {
			if(parent != null && parent.IsChecked.HasValue)
				return parent.IsChecked.Value;
			return GroupFilterValues.Contains(value, parentValues);
		}
		#region IFilterValues
		PivotFilterType IFilterValues.FilterType {
			get { return ShowNewValues ? PivotFilterType.Excluded : PivotFilterType.Included; }
			set { throw new ArgumentException(); }
		}
		CriteriaOperator IFilterValues.GetActualCriteria() {
			if(!CanAccept)
				return null;
			ProcessNullValues();
			PivotGroupFilterValuesCollection includedValues = GetFilteredValues(true);
			PivotGroupFilterValuesCollection excludedValues = GetFilteredValues(false);
			if(includedValues.IsEmpty || excludedValues.IsEmpty)
				return null;
			PivotFilterType filterType;
			PivotGroupFilterValuesCollection collection;
			if(CountValues(includedValues) > CountValues(excludedValues)) {
				filterType = PivotFilterType.Excluded;
				collection = excludedValues;
			} else {
				filterType = PivotFilterType.Included;
				collection = includedValues;
			}
			return CriteriaFilterHelper.CreateCriteria(filterType, collection, Group.Data.IsForceNullInCriteria());
		}
		void ProcessNullValues() {
			int i = 0;
			while(i < Count) {
				PivotGroupFilterItem current = this[i];
				if(!current.IsChecked.HasValue && (i == Count - 1 || current.Level >= this[i + 1].Level))
					LoadValues(current.GetBranch());
				i++;
			}
		}
		int CountValues(PivotGroupFilterValuesCollection values) {
			if(values == null)
				return 0;
			int count = values.Count;
			foreach(PivotGroupFilterValue value in values)
				count += CountValues(value.ChildValues);
			return count;
		}
		#endregion
		#region IGroupFilter
		bool IGroupFilter.HasFilter { get { return true; } }
		PivotGroupFilterValuesCollection IGroupFilter.Values { get { return GetFilteredValues(); } }
		PivotFilterType IGroupFilter.FilterType { get { return GetFilterType(); } }
		int IGroupFilter.LevelCount { get { return Group.Count; } }
		int IGroupFilter.GetOLAPLevel(int i) { return Group[i].Level; }
		string IGroupFilter.GetFieldName(int i) { return Group[i].FieldName; }
		#endregion
	}
}
