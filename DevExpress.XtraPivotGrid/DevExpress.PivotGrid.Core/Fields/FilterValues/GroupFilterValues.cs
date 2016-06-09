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
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	public interface IFilterValues {
		PivotFilterType FilterType { get; set; }
		CriteriaOperator GetActualCriteria();
	}
	public interface IPivotGroupFilterValueParent {
		void OnChildChanged();
		int Level { get; }
		IPivotGroupFilterValueParent Parent { get; }
		PivotGridGroup Group { get; }
		PivotGroupFilterValuesCollection GetLevelValues(int level);
	}
	public class PivotGroupFilterValues : IFilterValues, IPivotGroupFilterValueParent, IXtraSerializableLayoutEx, IGroupFilter {
		PivotGridGroup group;
		PivotFilterType filterType;
		PivotGroupFilterValuesCollection values;
		int lockUpdateCount;
		public PivotGroupFilterValues(PivotGridGroup group)
			: this(group, PivotFilterType.Excluded) {
		}
		public PivotGroupFilterValues(PivotGridGroup group, PivotFilterType filterType) {
			this.group = group;
			this.filterType = filterType;
			this.values = new PivotGroupFilterValuesCollection(this);
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValuesGroup")]
#endif
		public PivotGridGroup Group {
			get {
				if(group != null && !string.IsNullOrEmpty(group.Hierarchy) && group.Count == 0
						&& group.Collection != null && group.Index < 0) {
					group = group.Collection.Find(delegate(PivotGridGroup obj) {
						return obj.Hierarchy == group.Hierarchy;
					});
				}
				return group;
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValuesFilterType")]
#endif
		[XtraSerializableProperty(1)]
		public PivotFilterType FilterType {
			get { return filterType; }
			set {
				if(FilterType == value) return;
				filterType = value;
				OnChanged();
			}
		}
		internal void InvertFilterValues(PivotFilterType newFilterType) {
			BeginUpdate();
			PivotGroupFilterValues newFilterValues = new PivotGroupFilterValues(Group, newFilterType);
			newFilterValues.BeginUpdate();
			PivotGroupFilterValuesCollection newFilterValuesCollection = newFilterValues.Values ?? new PivotGroupFilterValuesCollection(this);
			List<object> allValues = Group.GetUniqueValues(null);
			PivotGroupFilterValue childValue;
			if(group.Count > 1) {
				for(int i = 0; i < allValues.Count; i++) {
					if(Values.ContainsValue(allValues[i]) && Values[allValues[i]].ChildValues.Count == 0)
						continue;
					else {
						childValue = InvertBranch(allValues[i], null);
						if(childValue != null)
							newFilterValuesCollection.Add(childValue);
					}
				}
			} else {
				if(group.Count == 1) {
					foreach(PivotGroupFilterValue value in Values)
						allValues.Remove(value.Value);
					foreach(object value in allValues)
						newFilterValuesCollection.Add(value);
				}
			}
			values = newFilterValuesCollection;
			newFilterValues.CancelUpdate();
			CancelUpdate();
			FilterType = newFilterType;
		}
		PivotGroupFilterValue InvertBranch(object newValue, object[] parentValues) {
			PivotGroupFilterValue newFilterValue = new PivotGroupFilterValue(newValue);
			parentValues = BuildParentValues(parentValues, newValue);
			PivotGroupFilterValuesCollection levelFilterValues;
			PivotGroupFilterValue childValue;
			levelFilterValues = GetValuesFromBranch(parentValues);
			if(levelFilterValues == null)
				return newFilterValue;
			List<object> allValues = Group.GetUniqueValues(parentValues);
			for(int i = 0; i < allValues.Count; i++) {
				if(levelFilterValues != null && 
					levelFilterValues.ContainsValue(allValues[i]) && 
					(levelFilterValues[allValues[i]].ChildValues == null || 
					levelFilterValues[allValues[i]].ChildValues.Count == 0))
					continue;
				else {
					childValue = InvertBranch(allValues[i], parentValues);
					if(childValue != null)
						newFilterValue.ChildValues.Add(childValue);
				}
			}
			if(parentValues.Length != LevelCount && newFilterValue.ChildValues.Count == 0) return null;
			return newFilterValue;
		}
		PivotGroupFilterValuesCollection GetValuesFromBranch(object[] branch) {
			if(branch == null) return Values;
			PivotGroupFilterValuesCollection res = Values;
			for(int i = 0; i < branch.Length; i++) {
				res = res.ContainsValue(branch[i]) ? res[branch[i]].ChildValues : null;
				if(res == null) return null;
			}
			return res;
		}
		object[] BuildParentValues(object[] oldValues, object newValue) {
			int newArrayLenght = oldValues != null ? oldValues.Length + 1 : 1;
			object[] res = new object[newArrayLenght];
			if(newArrayLenght == 1) {
				res[0] = newValue;
				return res;
			}
			for(int i = 0; i < oldValues.Length; i++) {
				res[i] = oldValues[i];
			}
			res[newArrayLenght - 1] = newValue;
			return res;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValuesCount")]
#endif
		public int Count { get { return !IsGroupEmpty ? Values.Count : 0; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValuesIsEmpty")]
#endif
		public bool IsEmpty {
			get {
				if(Group == null || !Group.IsFilterAllowed) return true;
				if(FilterType == PivotFilterType.Excluded)
					return IsGroupEmpty || Count == 0;
				return Group != null ? Group.IsFilterEmpty : true;
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValuesHasFilter")]
#endif
		public bool HasFilter { get { return !IsEmpty; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValuesValues"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Content, 0)]
		public PivotGroupFilterValuesCollection Values {
			get { return !IsGroupEmpty ? values : null; }
		}
		protected PivotGridData Data { get { return Group != null ? Group.Data : null; } }
		protected PivotGridDataAsync DataAsync { get { return Data as PivotGridDataAsync; } }
		protected bool IsGroupEmpty { get { return Group != null ? Group.Count == 0 : false; } }
		public void Reset() {
			values.Clear();
			filterType = PivotFilterType.Excluded;
			OnChanged();
		}
		public bool IsEquals(PivotGroupFilterValues b) {
			if(b.FilterType != this.FilterType)
				return false;
			return PivotGroupFilterValuesCollection.IsEquals(Values, b.Values);
		}
		public PivotGroupFilterValues Clone() {
			PivotGroupFilterValues clone = new PivotGroupFilterValues(Group);
			clone.Assign(this);
			return clone;
		}
		public void Assign(PivotGroupFilterValues src) {
			BeginUpdate();
			Values.Clear();
			FilterType = src.FilterType;
			PivotGroupFilterValuesCollection.Clone(src.Values, Values);
			CancelUpdate();
		}
		public bool SetValues(PivotGroupFilterValuesCollection values, PivotFilterType filterType, bool allowOnChanged) {
			return SetValues(false, values, filterType, allowOnChanged, null);
		}
		public bool SetValuesAsync(PivotGroupFilterValuesCollection values, PivotFilterType filterType, AsyncCompletedHandler handler) {
			return SetValues(true, values, filterType, true, handler);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SetValuesAsync(PivotGroupFilterValuesCollection values, PivotFilterType filterType, bool allowOnChanged) {
			return SetValues(true, values, filterType, allowOnChanged, null);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SetValuesAsync(PivotGroupFilterValuesCollection values, bool allowOnChanged) {
			return SetValuesAsync(values, FilterType, allowOnChanged);
		}
		bool SetValues(bool isAsync, PivotGroupFilterValuesCollection values, PivotFilterType filterType, bool allowOnChanged, AsyncCompletedHandler handler) {
			bool changed = OnBeforeSetValues(values, filterType, allowOnChanged);
			if(changed && allowOnChanged)
				OnChanged(isAsync, handler);
			if(!changed && allowOnChanged && handler != null)
				handler(AsyncOperationResult.Create(true, null));
			return changed;
		}
		bool OnBeforeSetValues(PivotGroupFilterValuesCollection values, PivotFilterType filterType, bool allowOnChanged) {
			bool notEquals = !PivotGroupFilterValuesCollection.IsEquals(this.values, values),
				changed = notEquals || this.filterType != filterType;
			if(notEquals)
				this.values = values;
			this.filterType = filterType;
			return changed;
		}
		public bool? Contains(object value, object[] parentValues) {
			return Contains(value, parentValues, true);
		}
		public bool? Contains(object value, object[] parentValues, bool invertExcluded) {
			bool? res = Group.IsFilterValueChecked(parentValues, value);
			if(res.HasValue && FilterType == PivotFilterType.Excluded && invertExcluded)
				res = !res.Value;
			return res;
		}
		public PivotGroupFilterValue FindFilterValue(params object[] values) {
			if(this.values.IsEmpty || values.Length == 0) return null;
			PivotGroupFilterValue filterValue = this.values[values[0]];
			for(int i = 1; i < values.Length; i++) {
				if(filterValue == null)
					return null;
				filterValue = filterValue.ChildValues[values[i]];
			}
			return filterValue;
		}
		void OnChanged() {
			OnChanged(false, null);
		}
		protected virtual void OnChanged(bool isAsync, AsyncCompletedHandler handler) {
			if(IsLockUpdate) return;
			if(handler == null)
				handler = delegate(AsyncOperationResult result) { };
			if(Data != null) {
				if(isAsync && DataAsync != null)
					DataAsync.OnGroupFilteringChangedAsync(group, false, handler);
				else
					Data.OnGroupFilteringChanged(group);
			}
		}
		protected virtual void OnChildChanged() {
			OnChanged();
		}
		int Level { get { return -1; } }
		int LevelCount { get { return IsGroupEmpty ? 0 : Group.Count; } }
		protected internal void SaveToStream(TypedBinaryWriter writer) {
			writer.Write(LevelCount);
			if(LevelCount == 0) return;
			Type[] levelTypes = Group.GetFieldFilterValuesTypes();
			foreach(Type fieldType in levelTypes) {
				writer.WriteType(fieldType);
			}
			writer.Write((byte)FilterType);
			Values.SaveToStream(writer, levelTypes);
		}
		protected internal void LoadFromStream(TypedBinaryReader reader) {
			Reset();
			int levelCount = reader.ReadInt32();
			if(levelCount == 0) return;
			BeginUpdate();
			Type[] levelTypes = new Type[levelCount];
			for(int i = 0; i < levelCount; i++) {
				levelTypes[i] = reader.ReadType();
			}
			FilterType = (PivotFilterType)reader.ReadByte();
			values.LoadFromStream(reader, levelTypes);
			EndUpdate();
		}
		public virtual void BeginUpdate() {
			this.lockUpdateCount++;
		}
		public void CancelUpdate() {
			this.lockUpdateCount--;
		}
		public virtual void EndUpdate() {
			if(--this.lockUpdateCount == 0) {
				OnChanged();
			}
		}
		protected bool IsLockUpdate { get { return this.lockUpdateCount > 0; } }
		#region IPivotGridGroupFilterValueParent Members
		void IPivotGroupFilterValueParent.OnChildChanged() {
			OnChildChanged();
		}
		int IPivotGroupFilterValueParent.Level {
			get { return this.Level; }
		}
		IPivotGroupFilterValueParent IPivotGroupFilterValueParent.Parent {
			get { return null; }
		}
		PivotGroupFilterValuesCollection IPivotGroupFilterValueParent.GetLevelValues(int level) {
			return this.Level == level ? Values : null;
		}
		#endregion
		#region IFilterValues Members
		PivotFilterType IFilterValues.FilterType { get { return this.FilterType; } set { this.FilterType = value; } }
		#endregion
		#region IXtraSerializableLayoutEx Members
		public bool AllowProperty(DevExpress.Utils.OptionsLayoutBase options, string propertyName, int id) {
#if !SL
			return (options as PivotGridOptionsLayout != null) ? ((PivotGridOptionsLayout)options).StoreDataSettings : true;
#else
			return true;
#endif
		}
		public void ResetProperties(DevExpress.Utils.OptionsLayoutBase options) {
#if !SL
			PivotGridOptionsLayout optionsLayout = options as PivotGridOptionsLayout;
			if(optionsLayout == null) return;
			if(!optionsLayout.StoreDataSettings && (optionsLayout.ResetOptions & PivotGridResetOptions.OptionsData) != 0) 
#endif
			{
				Values.Clear();
				FilterType = PivotFilterType.Excluded;
			}
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public string DeferFilterString {
			get {
				return deferFilter != null ? deferFilter.PersistentString : string.Empty;
			}
			set {
				if(string.IsNullOrEmpty(value))
					return;
				PivotGroupFilterItems items = new PivotGroupFilterItems(Data, Group[0], false, true);
				items.PersistentString = value;
				deferFilter = items;
			}
		}
		IGroupFilter deferFilter;
		internal IGroupFilter GetDefereFilter() {
			return deferFilter;
		}
		internal void SetDefereFilter(IGroupFilter filter) {
			deferFilter = filter;
		}
		internal IGroupFilter GetActual(bool deferUpdates) {
			return deferUpdates && deferFilter != null ? deferFilter : this;
		}
		#region IFilterValues 
		CriteriaOperator IFilterValues.GetActualCriteria() {
			return CriteriaFilterHelper.CreateCriteria(FilterType, Values, Group.Data.IsForceNullInCriteria());
		}
		#endregion
		#region IGroupFilter
		int IGroupFilter.LevelCount { get { return Group.Count; } }
		int IGroupFilter.GetOLAPLevel(int i) {
			return Group[i].Level;
		}
		string IGroupFilter.GetFieldName(int i) { return Group[i].FieldName; }
		string IGroupFilter.PersistentString { get { return string.Empty; } }
		#endregion
	}
	public class PivotGroupFilterValuesCollection : IEnumerable<PivotGroupFilterValue>, IXtraSupportDeserializeCollectionItem, IXtraSupportDeserializeCollection {
		NullableDictionary<object, PivotGroupFilterValue> items;
		IPivotGroupFilterValueParent owner;
		public PivotGroupFilterValuesCollection(IPivotGroupFilterValueParent owner) {
			if(owner == null)
				throw new ArgumentNullException("owner");
			this.owner = owner;
			this.items = new NullableDictionary<object, PivotGroupFilterValue>();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false)]
		public List<PivotGroupFilterValue> Items {
			get {
				List<PivotGroupFilterValue> list = new List<PivotGroupFilterValue>();
				list.AddRange(items.Values);
				return list;
			}
		}
		protected IPivotGroupFilterValueParent Owner { get { return owner; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValuesCollectionCount")]
#endif
		public int Count { get { return items.Count; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValuesCollectionIsEmpty")]
#endif
		public bool IsEmpty { get { return Count == 0; } }
		public PivotGroupFilterValue this[object key] {
			get {
				PivotGroupFilterValue res = null;
				return items.TryGetValue(key, out res) ? res : null;
			}
			set {
				PivotGroupFilterValue exist;
				if(items.TryGetValue(key, out exist))
					exist.Parent = null;
				value.Parent = Owner;
				items[key] = value;
				OnChanged();
			}
		}
		protected void AddCore(PivotGroupFilterValue value) {
			if(items.Contains(value.Value))
				throw new ArgumentException("Collection already contains this value");
			items.Add(value.Value, value);
		}
		protected void RemoveCore(PivotGroupFilterValue value) {
			items.Remove(value.Value);
		}
		public void Add(PivotGroupFilterValue value) {
			value.Parent = Owner;
			AddCore(value);
			OnChanged();
		}
		public PivotGroupFilterValue Add(object value) {
			PivotGroupFilterValue item = new PivotGroupFilterValue(value);
			Add(item);
			return item;
		}
		public void Remove(PivotGroupFilterValue value) {
			RemoveCore(value, true);
		}
		internal void RemoveCore(PivotGroupFilterValue value, bool resetParent) {
			if(resetParent)
				value.Parent = null;
			RemoveCore(value);
			OnChanged();
		}
		public void Remove(object value) {
			PivotGroupFilterValue filterValue = this[value];
			if(filterValue != null)
				Remove(filterValue);
		}
		public void Clear() {
			if(items.Count == 0) return;
			foreach(KeyValuePair<object, PivotGroupFilterValue> pair in items) {
				if(Owner != pair.Value.Parent)
					pair.Value.Parent = null;
			}
			items.Clear();
			OnChanged();
		}
		public bool ContainsValue(object value) {
			return items.Contains(value);
		}
		protected virtual void OnChanged() {
			if(Owner != null)
				Owner.OnChildChanged();
		}
		public override string ToString() {
			return "{" + items.Count + " values}";
		}
		public List<object> ToList() {
			return GetValues(this);
		}
		public static bool IsEquals(PivotGroupFilterValuesCollection a, PivotGroupFilterValuesCollection b) {
			if((a == null && b == null) || (a == null && b.Count == 0) || (b == null && a.Count == 0))
				return true;
			if(a == null && b != null || a != null && b == null)
				return false;
			if(a.items.Count != b.items.Count)
				return false;
			foreach(KeyValuePair<object, PivotGroupFilterValue> aItem in a.items) {
				PivotGroupFilterValue bValue = b[aItem.Key];
				if(bValue == null) return false;
				if(!IsEquals(aItem.Value.ChildValues, bValue.ChildValues))
					return false;
			}
			return true;
		}
		public static void Clone(PivotGroupFilterValuesCollection src, PivotGroupFilterValuesCollection dest) {
			if(src == null || dest == null) return;
			foreach(PivotGroupFilterValue value in src.items.Values) {
				PivotGroupFilterValue clone = dest.Add(value.Value);
				Clone(value.ChildValues, clone.ChildValues);
			}
		}
		public List<PivotGroupFilterValue> FindAll(Predicate<PivotGroupFilterValue> predicate) {
			List<PivotGroupFilterValue> res = new List<PivotGroupFilterValue>();
			foreach(PivotGroupFilterValue value in this) {
				if(predicate(value))
					res.Add(value);
			}
			return res;
		}
		internal List<object> GetLastLevelValues() {
			List<PivotGroupFilterValue> lastLevelValues = FindAll(
				   delegate(PivotGroupFilterValue value) {
					   return value.ChildValues == null || value.ChildValues.Count == 0;
				   });
			return GetValues(lastLevelValues);
		}
		List<object> GetValues(IEnumerable<PivotGroupFilterValue> values) {
			List<object> res = new List<object>();
			foreach(PivotGroupFilterValue value in values) {
				res.Add(value.Value);
			}
			return res;
		}
		protected internal void SaveToStream(TypedBinaryWriter writer, Type[] levelTypes) {
			writer.Write(Count);
			foreach(PivotGroupFilterValue value in this) {
				value.SaveToStream(writer, levelTypes);
			}
		}
		protected internal void LoadFromStream(TypedBinaryReader reader, Type[] levelTypes) {
			Clear();
			int count = reader.ReadInt32();
			for(int i = 0; i < count; i++) {
				PivotGroupFilterValue value = new PivotGroupFilterValue(this.Owner);
				if(value.LoadFromStream(reader, levelTypes))
					this.Add(value);
			}
		}
		#region IEnumerable<PivotGroupFilterValue> Members
		IEnumerator<PivotGroupFilterValue> IEnumerable<PivotGroupFilterValue>.GetEnumerator() {
			return items.Values.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return items.Values.GetEnumerator();
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		public object CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName != "Items")
				throw new NotImplementedException();
			XtraPropertyInfo propInfo = e.Item.ChildProperties["Value"];
			if(propInfo.Value == null || propInfo.PropertyType == null) {
				return Add(propInfo.Value);
			} else {
				string str = propInfo.Value as string;
				if(str != null && Owner != null && Owner.Group != null && Owner.Group.Data != null && Owner.Group.Data.OptionsData.CustomObjectConverter != null && Owner.Group.Data.OptionsData.CustomObjectConverter.CanConvert(propInfo.PropertyType))
					return Add(Owner.Group.Data.OptionsData.CustomObjectConverter.FromString(propInfo.PropertyType, str));
				else
				return Add(propInfo.ValueToObject(propInfo.PropertyType));
			}
		}
		public void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
		}
		#endregion
		#region IXtraSupportDeserializeCollection Members
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			bool fullLayout = e.Options == OptionsLayoutBase.FullLayout;
			PivotGridOptionsLayout optionsLayout = e.Options as PivotGridOptionsLayout;
			if(optionsLayout == null && !fullLayout) return;
#if !SL
			if(fullLayout || !optionsLayout.StoreDataSettings && (optionsLayout.ResetOptions & PivotGridResetOptions.OptionsData) != 0)
#endif
			Clear();
		}
		public bool ClearCollection(string propertyName, XtraItemEventArgs e) {
			return false;
		}
		#endregion
	}
	public class PivotGroupFilterValue : PivotFilterValue, IPivotGroupFilterValueParent {
		IPivotGroupFilterValueParent parent;
		PivotGroupFilterValuesCollection childValues;
		int level;
		public PivotGroupFilterValue(object value) : this(null, value) {
		}
		internal PivotGroupFilterValue(IPivotGroupFilterValueParent parent) : this(parent, null) {
		}
		internal PivotGroupFilterValue(IPivotGroupFilterValueParent parent, object value) : base(value) {
			this.Parent = parent;
		}
		protected internal IPivotGroupFilterValueParent Parent {
			get { return parent; }
			set {
				if(value == parent) return;
				if(Parent != null && value != null)
					throw new ArgumentException("PivotGridFilterValue can be added into a single collection only.");
				parent = value;
				this.level = parent != null ? parent.Level + 1 : -1;
			}
		}
		protected PivotGridGroup Group {
			get {
				if(Parent != null) return Parent.Group;
				return null;
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValueLevel"),
#endif
		XtraSerializableProperty(1)
		]
		public int Level { get { return level; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValueField")]
#endif
		public PivotGridFieldBase Field { get { return Group[Level]; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValueIsLastLevel")]
#endif
		public bool IsLastLevel { get { return Group != null ? Level == Group.Count - 1 : false; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGroupFilterValueChildValues"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Content, 2)
		]
		public PivotGroupFilterValuesCollection ChildValues {
			get {
				if(!IsLastLevel && childValues == null)
					childValues = new PivotGroupFilterValuesCollection(this);
				return IsLastLevel ? null : childValues;
			}
		}
		public void Clear() {
			if(childValues != null) childValues.Clear();
		}
		internal PivotGroupFilterValuesCollection GetLevelValues(int level) {
			if(level == this.Level) {
				return ChildValues;
			} else if(level < this.Level) {
				return (Parent != null) ? Parent.GetLevelValues(level) : null;
			} else
				throw new ArgumentException("The GetLevelValues 'level' parameter is incorrect.");
		}
		protected virtual void OnChanged() {
			if(Parent != null)
				Parent.OnChildChanged();
		}
		protected virtual void OnChildChanged() {
			OnChanged();
		}
		protected internal void SaveToStream(TypedBinaryWriter writer, Type[] levelTypes) {
			if(Level < 0) return;
			Type levelType = levelTypes[Level];
			if(levelType == null) return;
			levelTypes[Level] = PivotGridFieldFilterValues.CheckValueType(this.Value, levelType);
			writer.WriteObject(this.Value);
			if(this.ChildValues != null)
				this.ChildValues.SaveToStream(writer, levelTypes);
		}
		protected internal bool LoadFromStream(TypedBinaryReader reader, Type[] levelTypes) {
			if(Level < 0) return false;
			Type levelType = levelTypes[Level];
			if(levelType == null) return false;
			this.Value = reader.ReadObject(levelType);
			if(ChildValues != null)
				ChildValues.LoadFromStream(reader, levelTypes);
			return true;
		}
		#region IPivotGridGroupFilterValueParent Members
		void IPivotGroupFilterValueParent.OnChildChanged() {
			OnChildChanged();
		}
		int IPivotGroupFilterValueParent.Level {
			get { return this.Level; }
		}
		PivotGridGroup IPivotGroupFilterValueParent.Group {
			get { return Group; }
		}
		IPivotGroupFilterValueParent IPivotGroupFilterValueParent.Parent {
			get { return Parent; }
		}
		PivotGroupFilterValuesCollection IPivotGroupFilterValueParent.GetLevelValues(int level) {
			return this.GetLevelValues(level);
		}
		#endregion
	}
}
