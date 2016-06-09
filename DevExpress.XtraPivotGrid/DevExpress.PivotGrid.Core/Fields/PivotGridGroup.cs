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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPivotGrid {
	public class PivotGroupFields : BaseCollection<PivotGridFieldBase> {
		readonly PivotGridGroup group;
		public PivotGroupFields(PivotGridGroup group) {
			this.group = group;
		}
		protected override bool CanAddItem(PivotGridFieldBase item) {
			return !item.HasGroup;
		}
		protected override void OnClear() {
			for(int i = 0; i < Count; i++) {
				this[i].Group = null;
			}
			base.OnClear();
		}
		protected override void OnInsertComplete(int index, IEnumerable<PivotGridFieldBase> collection) {
			base.OnInsertComplete(index, collection);
			foreach(PivotGridFieldBase item in collection) {
				item.Group = group;
			}
		}
		protected override void OnInsertComplete(int index, PivotGridFieldBase item) {
			base.OnInsertComplete(index, item);
			item.Group = group;
		}
		protected override void OnRemoveComplete(int index, PivotGridFieldBase item) {
			base.OnRemoveComplete(index, item);
			item.Group = null;
		}
		protected override void OnReplaceComplete(int index, PivotGridFieldBase oldItem, PivotGridFieldBase newItem) {
			base.OnReplaceComplete(index, oldItem, newItem);
			oldItem.Group = null;
			newItem.Group = group;
		}
	}
#if SL
	public class UniversalTypeConverterEx : DevExpress.Xpf.ComponentModel.ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(DevExpress.Xpf.ComponentModel.Design.Serialization.InstanceDescriptor)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(DevExpress.Xpf.ComponentModel.Design.Serialization.InstanceDescriptor) && value != null)
				return new DevExpress.Xpf.ComponentModel.Design.Serialization.InstanceDescriptor(value.GetType().GetConstructor(Type.EmptyTypes), null, false);
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
#endif
#if !DXPORTABLE
	[TypeConverter(typeof(UniversalTypeConverterEx))]
#endif
	public class PivotGridGroup : IEnumerable {
		PivotGroupFields innerList;
		string caption;
		PivotGridGroupCollection collection;
		string hierarchy;
		PivotGroupFilterValues filterValues;
		public PivotGridGroup() : this(string.Empty) { }
		public PivotGridGroup(string caption) {
			this.collection = null;
			this.caption = caption;
			this.innerList = new PivotGroupFields(this);
			this.filterValues = CreateFilterValues();
		}
		protected internal PivotGridGroupCollection Collection {
			get { return collection; }
			set { collection = value; }
		}
		protected internal PivotGridData Data { get { return Collection != null ? Collection.Data : null; } }
		protected bool IsLoading { get { return Data != null ? Data.IsLoading : false; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridGroupItem")]
#endif
		public PivotGridFieldBase this[int index] { get { return innerList[index] as PivotGridFieldBase; } }
		[Browsable(false)]
		public int Index { get { return Collection != null ? Collection.IndexOf(this) : -1; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridGroupCount"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridGroup.Count")
		]
		public int Count { get { return innerList.Count; } }
		public void RemoveAt(int index) {
			if(index < 0 && index >= Count) return;
			RemoveCore(index);
		}
		protected void RemoveCore(int index) {
			if(IsOLAP && IsFieldValid(this[index]) && !this[index].IsDisposed && (Data == null || Data.Fields == null || Data.Fields.Contains(this[index])))
				throw new Exception("Cannot remove the field from the hierarchy.");
			object obj = innerList[index];
			innerList.RemoveAt(index);
			OnRemoveComplete(index, obj);
		}
		public void Remove(PivotGridFieldBase field) {
			RemoveAt(IndexOf(field));
		}
		public void Clear() {
			innerList.Clear();
			filterValues.Reset();
			OnChanged();
		}
		protected void Sort(IComparer<PivotGridFieldBase> comparer) {
			innerList.Sort(comparer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.NameCollection, true, false, true, 0)]
		public IList Fields { get { return innerList; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateFieldsItem(XtraItemEventArgs e) {
			string name = e.Item.Value.ToString();
			if(name != string.Empty && Data != null) {
				PivotGridFieldBase field = Data.Fields.GetFieldByName(name);
				if(field != null) {
					return field;
				}
			}
			return null;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridGroupCaption"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridGroup.Caption"),
		DefaultValue(""), XtraSerializableProperty()
		]
		public string Caption {
			get { return GetCaptionCore(); }
			set {
				if(value == null)
					value = string.Empty;
				if(Caption == value) return;
				caption = value;
				FireChanged(null);
			}
		}
		string GetCaptionCore() {
			if(Data != null && Data.IsOLAP && string.IsNullOrEmpty(caption))
				return Data.GetHierarchyCaption(Hierarchy);
			return caption;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotArea Area { get { return Count > 0 ? this[0].AreaCore : PivotArea.FilterArea; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AreaIndex { get { return Count > 0 ? this[0].AreaIndex : -1; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Visible { get { return Count > 0 ? this[0].VisibleCore : false; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridGroupVisibleCount"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridGroup.VisibleCount")
		]
		public int VisibleCount {
			get {
				for(int i = 0; i < Count; i++) {
					if(!this[i].ExpandedInFieldsGroup)
						return i + 1;
				}
				return Count;
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridGroupFilterValues"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
		DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridGroup.FilterValues"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Content, 1)
		]
		public PivotGroupFilterValues FilterValues {
			get { return filterValues; }
			protected internal set {
				if(filterValues == value) return;
				filterValues = value;
				OnFilterChanged();
			}
		}
		public bool ShowNewValues {
			get { return FilterValues.FilterType == PivotFilterType.Excluded; }
			set {
				PivotFilterType newFilterType = value ? PivotFilterType.Excluded : PivotFilterType.Included;
				if(FilterValues.FilterType == newFilterType) return;
				PivotGridDataAsync data = Data as PivotGridDataAsync;
				if(data != null)
					data.InvertGroupFilterAsync(FilterValues, newFilterType, false, (d) => { });
				else
					FilterValues.InvertFilterValues(newFilterType);
			}
		}
		public bool IsFieldVisible(PivotGridFieldBase field) {
			if(field.Group != this) return field.Visible;
			int fieldIndex = IndexOf(field);
			for(int i = 0; i < fieldIndex; i++) {
				if(!this[i].ExpandedInFieldsGroup)
					return false;
			}
			return Visible;
		}
		protected internal Type[] GetFieldFilterValuesTypes() {
			Type[] res = new Type[Count];
			for(int i = 0; i < Count; i++) {
				if(this[i].ActualOLAPFilterByUniqueName)
					res[i] = typeof(string);
				else
					res[i] = Data.GetFieldType(this[i]);
			}
			return res;
		}
		public List<PivotGridFieldBase> GetVisibleFields() {
			List<PivotGridFieldBase> res = new List<PivotGridFieldBase>();
			for(int i = 0; i < Count; i++) {
				if(!this[i].ExpandedInFieldsGroup || !this[i].Visible)
					break;
				res.Add(this[i]);
			}
			return res;
		}
		internal bool? IsFilterValueChecked(object[] parentValues, object value) {
			if(Data == null || (parentValues != null && parentValues.Length > Fields.Count)) return false;
			return Data.IsGroupFilterValueChecked(this, parentValues, value);
		}
		internal bool IsFilterEmpty {
			get {
				if(Data == null || FilterValues.Count == 0 || Count == 0) return true;
				return Data.GetIsEmptyGroupFilter(this);
			}
		}
		public bool IsFilterAllowed {
			get {
				return Count > 0 && this[0].GroupFilterMode == PivotGroupFilterMode.Tree;
			}
		}
		public List<object> GetUniqueValues(object[] parentValues) {
			if(Data == null || (parentValues != null && parentValues.Length > Fields.Count)) return null;
			return Data.GetSortedUniqueGroupValues(this, parentValues);
		}		
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanExpandField(PivotGridFieldBase field) {
			if(field.Group != this) return false;
			return IndexOf(field) < Count - 1;
		}
		public bool CanAdd(PivotGridFieldBase field) {
			return Collection == null || !Collection.Contains(field);
		}
		public void Add(PivotGridFieldBase field) {
			if(IsOLAP && !IsFieldValid(field) && !IsLoading)
				throw new Exception("Cannot move the field to another hierarchy.");
			if(field.HasGroup) return;
			innerList.Add(field);
			OnInsertComplete(Count, field);
		}
		public void AddRange(PivotGridFieldBase[] fields) {
			foreach(PivotGridFieldBase field in fields) {
				Add(field);
			}
		}
		public int IndexOf(PivotGridFieldBase field) {
			return innerList.IndexOf(field);
		}
		public bool Contains(PivotGridFieldBase field) {
			return innerList.Contains(field);
		}
		public bool CanChangeArea(PivotGridFieldBase field) {
			return field.Group == this && IndexOf(field) < 1;
		}
		public bool CanChangeAreaTo(PivotArea newArea, int newAreaIndex) {
			if(Count < 2 || Area != newArea) return true;
			return newAreaIndex <= AreaIndex || newAreaIndex >= AreaIndex + VisibleCount;
		}
		public void ChangeFieldIndex(PivotGridFieldBase field, int newIndex) {
			if(newIndex < 0 || newIndex >= Count || field.Group != this || IndexOf(field) == newIndex) return;
			innerList.Remove(field);
			innerList.Insert(newIndex, field);
			OnChanged();
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption)) return Caption;
			if(Count > 0) {
				string st = this[0].ToString();
				for(int i = 1; i < Count; i++) {
					st += " - " + this[i].ToString();
				}
				return st;
			}
			if(Collection != null) {
				return "Group " + Collection.IndexOf(this).ToString();
			}
			return string.Empty;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public string Hierarchy { get { return hierarchy; } set { hierarchy = value; } }
		protected bool CollectionContainsField(PivotGridFieldBase field) {
			return Collection != null ? Collection.Contains(field) : Contains(field);
		}
		protected virtual void OnInsertComplete(int index, object obj) {
			if(IsOLAP && Count > 1)
				Sort(new ByLevelComarer());
			FireChanged(obj);
			OnChanged();
		}
		protected virtual void OnRemoveComplete(int index, object obj) {
			FireChanged(obj);
			OnChanged();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void OnChanged() {
			UpdateAreaIndexes();
			if(Collection != null)
				Collection.GroupsChanged();
		}
		protected internal void UpdateAreaIndexes() {
			UpdateAreaIndexes(null);
		}
		protected internal virtual void UpdateAreaIndexes(PivotGridFieldBase field) {
			if(Data == null) return;
			Data.Fields.UpdateAreaIndexes(field);
			Data.DoRefresh();
		}
		protected void FireChanged(object obj) {
			if(Data != null)
				Data.FireChanged(obj);
		}
		System.Collections.IEnumerator IEnumerable.GetEnumerator() {
			return (innerList as IEnumerable).GetEnumerator();
		}
		internal void RemoveInvalidFields() {
			if(!IsOLAP) return;
			for(int i = Count - 1; i >= 0; i--)
				if(!IsFieldValid(this[i]))
					RemoveCore(i);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsOLAP { get { return !string.IsNullOrEmpty(Hierarchy); } }
		protected internal bool IsFieldValid(PivotGridFieldBase field) {
			return field.Hierarchy == Hierarchy;
		}
		class ByLevelComarer : IComparer<PivotGridFieldBase> {
			bool connectException = false;
			public int Compare(PivotGridFieldBase x, PivotGridFieldBase y) {
				if(connectException) return 0;
				try {
					return Comparer<int>.Default.Compare(x.Level, y.Level);
				} catch(OLAPConnectionException) {
					connectException = true;
					return 0;
				}
			}
		}
		protected internal virtual void OnFilterChanged() {
			if(Data == null) return;
			Data.OnGroupFilteringChanged(this);
		}
		protected virtual PivotGroupFilterValues CreateFilterValues() {
			return new PivotGroupFilterValues(this);
		}
		internal int[] GetColumnHandles() {
			int[] res = new int[Fields.Count];
			for(int i = 0; i < Count; i++) {
				res[i] = this[i].ColumnHandle;
			}
			return res;
		}
	}
	[ListBindable(false)]
	public class PivotGridGroupCollection : CollectionBase {
		PivotGridData data;
		public PivotGridGroupCollection(PivotGridData data) {
			this.data = data;
		}
		protected internal PivotGridData Data { get { return data; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridGroupCollectionItem")]
#endif
		public PivotGridGroup this[int index] { get { return InnerList[index] as PivotGridGroup; } }
		public PivotGridGroup Add() {
			PivotGridGroup fieldGroup = CreateGroup();
			Add(fieldGroup);
			return fieldGroup;
		}
		public void Add(PivotGridGroup fieldGroup) {
			List.Add(fieldGroup);
		}
		public void Add(string caption) {
			Add(new PivotGridFieldBase[0] { }, caption);
		}
		public PivotGridGroup Add(params PivotGridFieldBase[] fields) {
			return Add(fields, string.Empty);
		}
		public PivotGridGroup Add(PivotGridFieldBase[] fields, string caption) {
			PivotGridGroup group = CreateGroup();
			group.AddRange(fields);
			group.Caption = caption;
			Add(group);
			return group;
		}
		public void Insert(int index, PivotGridGroup fieldGroup) {
			List.Insert(index, fieldGroup);
		}
		public void Remove(PivotGridGroup fieldGroup) {
			List.Remove(fieldGroup);
		}
		public int IndexOf(PivotGridGroup fieldGroup) {
			return InnerList.IndexOf(fieldGroup);
		}
		public PivotGridGroup Find(Predicate<PivotGridGroup> predicate) {
			foreach(PivotGridGroup group in InnerList) {
				if(predicate(group))
					return group;
			}
			return null;
		}
		public bool Contains(PivotGridGroup fieldGroup) {
			return InnerList.Contains(fieldGroup);
		}
		public bool Contains(PivotGridFieldBase field) {
			return GetGroupByField(field) != null;
		}
		public void AddRange(PivotGridGroup[] groups) {
			foreach(PivotGridGroup fieldGroup in groups) {
				Add(fieldGroup);
			}
		}
		public PivotGridGroup GetGroupByField(PivotGridFieldBase field) {
			for(int i = 0; i < Count; i++) {
				if(this[i].Contains(field))
					return this[i];
			}
			return null;
		}
		public bool CanChangeAreaTo(PivotArea newArea, int newAreaIndex) {
			for(int i = 0; i < Count; i++) {
				if(!this[i].CanChangeAreaTo(newArea, newAreaIndex))
					return false;
			}
			return true;
		}
		protected internal void GroupsChanged() {
			FireChanged();
			if(Data != null)
				Data.OnGroupsChanged();
		}
		protected override void OnInsert(int index, object obj) {
			base.OnInsert(index, obj);
			PivotGridGroup fieldGroup = obj as PivotGridGroup;
			fieldGroup.Collection = this;
			if(fieldGroup.Count > 0) {
				for(int i = fieldGroup.Count - 1; i >= 0; i--) {
					if(fieldGroup[i].Group != fieldGroup)
						fieldGroup.RemoveAt(i);
				}
			}
		}
		protected override void OnInsertComplete(int index, object obj) {
			base.OnInsertComplete(index, obj);
			PivotGridGroup fieldGroup = obj as PivotGridGroup;
			if(fieldGroup.Count > 1) {
				fieldGroup.UpdateAreaIndexes();
			}
			fieldGroup.OnChanged();
			for(int i = index + 1; i < Count; i++) {
				this[i].OnChanged();
			}
			GroupsChanged();
			FieldCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			GroupsChanged();
			((PivotGridGroup)obj).Fields.Clear();
			FieldCollectionChanged();
		}
		protected override void OnClear() {
			for(int i = 0; i < Count; i++) {
				this[i].Fields.Clear();
			}
			base.OnClear();
		}
		protected void FireChanged() {
			if(Data != null)
				Data.FireChanged();
		}
		protected virtual PivotGridGroup CreateGroup() {
			return new PivotGridGroup();
		}
		internal PivotGridGroup CreateGroup(string hierarhy) {
			PivotGridGroup group = CreateGroup();
			group.Hierarchy = hierarhy;
			return group;
		}
		void FieldCollectionChanged() {
			if(Data != null)
				Data.FieldCollectionChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if(Data != null)
				Data.OnGroupsCleared();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			FieldCollectionChanged();
		}
	}
}
