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
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPivotGrid {
	public enum PivotFilterType { Excluded, Included };
	public class PivotFilterValue {
		object value;
		[
		XtraSerializableProperty(0)
		]
		public object Value { 
			get { return value; }
			protected set { this.value = value; }
		}
		public PivotFilterValue(object value) {
			this.value = value;
		}
		public override string ToString() {
			return Value != null ? Value.ToString() : string.Empty;
		}
	}
	[Serializable]
	public class PivotGridFieldFilterValues : IFilterValues, IXtraSerializableLayoutEx, IFieldFilter, IXtraSerializable, IXtraSupportDeserializeCollectionItem, IXtraSupportDeserializeCollection {
		protected internal const string AllItemsShouldHaveSameType = "All filter items should have the same type as the owner field.",
			ValueTypeFieldHasNullValue = "Fields bound to non-nullable columns can't have null filter values.";
		protected internal static Type CheckValueType(object value, Type fieldType) {
			if(value == null) {
				if(fieldType.IsValueType())
					throw new ArgumentException(ValueTypeFieldHasNullValue);
				return fieldType;
			}
			if(fieldType == typeof(object))
				return value.GetType();
			else if(fieldType != value.GetType())
				throw new ArgumentException(AllItemsShouldHaveSameType);
			return fieldType;
		}
		readonly NullableHashtable values;
		readonly PivotGridFieldBase field;
		PivotFilterType filterType;
		bool showBlanks;
		bool? savedIsEmpty;
		bool lastCaseSensitive = true;
		PivotGridFieldFilterValues(PivotGridFieldFilterValues source, bool assignField) {
			this.lastCaseSensitive = source.lastCaseSensitive;
			this.values = Init();
			Assign(source);
			if(assignField)
				field = source.Field;
		}
		protected internal PivotGridFieldFilterValues(bool lastCaseSensitive) {
			this.lastCaseSensitive = lastCaseSensitive;
			this.values = Init();
		}
		public PivotGridFieldFilterValues(PivotGridFieldBase field) {
			Guard.ArgumentNotNull(field, "field");
			this.field = field;
			lastCaseSensitive = field.Data == null || field.Data.PivotDataSource == null ? true : field.Data.CaseSensitive;
			this.values = Init();
		}
		NullableHashtable Init() {
			this.filterType = PivotFilterType.Excluded;
			this.showBlanks = true;
			this.savedIsEmpty = null;
			return new NullableHashtable(0, FilterValuesEqualityComparer.Create(lastCaseSensitive));
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesField")]
#endif
		public PivotGridFieldBase Field { get { return field; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesCount")]
#endif
		public int Count { get { return values.Count; } }
		internal bool? SavedIsEmpty { get { return savedIsEmpty; } set { savedIsEmpty = value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesIsEmpty")]
#endif
		public bool IsEmpty {
			get {
				if(SavedIsEmpty.HasValue)
					return SavedIsEmpty.Value;
				CalculateSavedIsEmpty(GetUniqueValues);
				return SavedIsEmpty.Value;
			}
		}
		protected PivotGridData Data { get { return Field != null ? Field.Data : null; } }
		protected PivotGridDataAsync DataAsync { get { return Data as PivotGridDataAsync; } }
		protected internal delegate object[] GetUniqueFieldValues(PivotGridFieldBase field);
		protected internal void CalculateSavedIsEmpty(GetUniqueFieldValues getUniqueFieldValues) {
			SavedIsEmpty = GetIsEmpty(getUniqueFieldValues);
		}
		bool GetIsEmpty(GetUniqueFieldValues getUniqueFieldValues) {
			if(FilterType == PivotFilterType.Excluded)
				return Count == 0;
			object[] uniqueValues = getUniqueFieldValues(Field);
			if(uniqueValues.Length == 0) 
				return values.Count == 0;
			foreach(object value in uniqueValues) {
				if(!Contains(value))
					return false;
			}
			return true;
		}
		object[] GetUniqueValues(PivotGridFieldBase field) {
			return field.GetUniqueValues();
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesHasFilter")]
#endif
		public bool HasFilter { get { return !IsEmpty || !ShowBlanks; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesValues"),
#endif
		XtraSerializableProperty(0), XtraSerializablePropertyId(100)]
		public object[] Values {
			get {
				object[] fValues = new object[Count];
				values.CopyKeysTo(fValues, 0);
				return fValues;
			}
			set {
				SetValuesCore(value, filterType);
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false)
		]
		public ICollection<PivotFilterValue> ValuesCore {
			get {
				return new ReadOnlyCollection<PivotFilterValue>(Values.Select((obj) => new PivotFilterValue(obj)).ToList());
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesValuesIncluded")]
#endif
		public object[] ValuesIncluded {
			get { return GetValues(PivotFilterType.Included); }
			set { SetValuesCore(value, PivotFilterType.Included); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesValuesExcluded")]
#endif
		public object[] ValuesExcluded {
			get { return GetValues(PivotFilterType.Excluded); }
			set { SetValuesCore(value, PivotFilterType.Excluded); }
		}
		object[] GetValues(PivotFilterType fType) {
			if(fType == FilterType)
				return Values;
			if(Field == null)
				return new object[0];
			List<object> list = new List<object>();
			if(Field.Data != null && Field.Data.IsOLAP && Field.ActualOLAPFilterByUniqueName) {
				IOLAPMember[] mems = Field.GetOLAPMembers();
				for(int i = 0; i < mems.Length; i++)
					if(!mems[i].IsTotal)
						list.Add(mems[i].UniqueName);
			} else {
				list.AddRange(Field.GetUniqueValues());
			}
			object[] values = Values;
			for(int i = 0; i < values.Length; i++)
				list.Remove(values[i]);
			if(!ShowBlanks)
				list.Remove(null);
			return list.ToArray();
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesShowBlanks")]
#endif
		[XtraSerializableProperty(1)]
		public bool ShowBlanks {
			get { return showBlanks; }
			set {
				if(value == ShowBlanks) return;
				showBlanks = value;
				OnChanged();
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldFilterValuesFilterType")]
#endif
		[XtraSerializableProperty(2)]
		public PivotFilterType FilterType {
			get { return filterType; }
			set {
				if(FilterType == value) return;
				filterType = value;
				OnChanged();
			}
		}
		public void SetFilterType(PivotFilterType filterType) {
			SetValuesCore(filterType == PivotFilterType.Excluded ? ValuesExcluded : ValuesIncluded, filterType);
		}
		public void InvertFilterType() {
			SetFilterType(FilterType == PivotFilterType.Excluded ? PivotFilterType.Included : PivotFilterType.Excluded);
		}
		public void Add(object value) {
			if(values.ContainsKey(value)) return;
			values.Add(value, null);
			OnChanged();
		}
		public void Remove(object value) {
			if(!values.ContainsKey(value)) return;
			values.Remove(value);
			OnChanged();
		}
		public void Clear() {
			if(!HasFilter && values.Count == 0) return;
			values.Clear();
			this.showBlanks = true;
			SavedIsEmpty = null;
			OnChanged();
		}
		public bool Contains(object value) {
			return Contains(value, true);
		}
		public bool Contains(object value, bool invertExcluded) {
			bool res = values.ContainsKey(value);
			if(FilterType == PivotFilterType.Excluded && invertExcluded)
				res = !res;
			return res;
		}
		protected internal bool ContainsKey(object value) {
			return values.ContainsKey(value);
		}
		public void Assign(PivotGridFieldFilterValues filteredValues) {
			if(filteredValues == null)
				return;
			SetValues((IList<object>)filteredValues.Values, filteredValues.FilterType, filteredValues.ShowBlanks);
		}		
		public bool IsEquals(IList<object> values) {
			if(this.values.Count != values.Count) return false;
			for(int i = 0; i < values.Count; i++) {
				if(!this.values.ContainsKey(values[i]))
					return false;
			}
			return true;
		}
		public bool IsEquals(PivotGridFieldFilterValues b) {
			if(b.FilterType != this.FilterType || b.ShowBlanks != this.ShowBlanks)
				return false;
			return IsEquals(b.Values);
		}
		internal PivotGridFieldFilterValues Clone() {
			return Clone(false);
		}
		internal PivotGridFieldFilterValues Clone(bool assignField) {
			return new PivotGridFieldFilterValues(this, assignField);
		}	 
		public void SetValues(IList<object> values, PivotFilterType filterType, bool showBlanks) {
			SetValues(values, filterType, showBlanks, true);
		}
		public bool SetValues(IList<object> values, PivotFilterType filterType, bool showBlanks, bool allowOnChanged) {
			return SetValues(false, values, filterType, showBlanks, allowOnChanged, null);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SetValuesAsync(IList<object> values, PivotFilterType filterType, bool showBlanks, bool allowOnChanged) {
			return SetValues(true, values, filterType, showBlanks, allowOnChanged, null);
		}
		public bool SetValuesAsync(IList<object> values, PivotFilterType filterType, bool showBlanks, AsyncCompletedHandler handler) {
			bool changed = SetValues(true, values, filterType, showBlanks, true, handler);
			if(!changed)
				handler(AsyncOperationResult.Create(true, null));
			return changed;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SetValuesAsync(IList<object> values, PivotFilterType filterType, bool showBlanks, bool allowOnChanged, AsyncCompletedHandler handler) {
			return SetValues(true, values, filterType, showBlanks, allowOnChanged, handler);
		}
		internal void EnsureCaseSensitive(bool value) {
			if(lastCaseSensitive == value)
				return;
			lastCaseSensitive = value;
			values.SwichComparer(FilterValuesEqualityComparer.Create(lastCaseSensitive));
		}
		protected bool SetValues(bool isAsync, IList<object> values, PivotFilterType filterType, bool showBlanks, bool allowOnChanged, AsyncCompletedHandler handler) {
			bool changed = OnBeforeSetValues(values, filterType, showBlanks, allowOnChanged);
			if(changed)
				OnChanged(isAsync, allowOnChanged, handler);
			return changed;
		}
		bool OnBeforeSetValues(IList<object> values, PivotFilterType filterType, bool showBlanks, bool allowOnChanged) {
			if(OnChanging(filterType, showBlanks, values)) return false;
			bool notEquals = !IsEquals(values),
				changed = notEquals || this.filterType != filterType || this.showBlanks != showBlanks;
			if(notEquals) {
				this.values.Clear();
				for(int i = 0; i < values.Count; i++)
					this.values[values[i]] = null;
			}
			this.filterType = filterType;
			this.showBlanks = showBlanks;
			return changed;
		}
		void SetValuesCore(object[] values, PivotFilterType filterType) {
			if(values == null) values = new object[0];
			List<object> list = new List<object>(values);
			if(OnChanging(filterType, ShowBlanks, list)) return;
			this.filterType = filterType;
			if(list.Count == 0) {
				Clear();
				return;
			}
			this.values.Clear();
			for(int i = 0; i < list.Count; i++)
				this.values.Add(list[i], null);
			OnChanged();
		}
		protected internal NullableHashtable GetHashtable() { return values; }
		NullableHashtable IFieldFilter.HashTable { get { return GetHashtable(); } }
		string IFieldFilter.PersistentString { get { return string.Empty; } }
		protected virtual bool OnChanging(PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return Field != null ? Field.OnFilteredValueChanging(filterType, showBlanks, values) : false;
		}
		protected virtual void OnChanged() {
			OnChanged(true);
		}
		protected void OnChanged(bool notifyField) {
			OnChanged(false, notifyField, null);
		}
		protected void OnChanged(bool notifyField, AsyncCompletedHandler handler) {
			OnChanged(false, notifyField, handler);
		}
		protected void OnChanged(bool isAsync, bool notifyField, AsyncCompletedHandler handler) {
			if(handler == null)
				handler = delegate(AsyncOperationResult result) { };
			SavedIsEmpty = null;
			if(notifyField && Field != null && Data != null) {
				if(isAsync && DataAsync != null)
					DataAsync.OnFieldFilteringChangedAsync(field, false, handler);
				else
					Data.OnFieldFilteringChanged(field);
			}
		}
		protected internal void SaveToStream(TypedBinaryWriter writer, Type fieldType) {
			if(fieldType == null) {
				writer.WriteType(fieldType);
				return;
			}
			object[] values = Values;
			if(Count > 0) {
				foreach(object val in values) {
					fieldType = PivotGridFieldFilterValues.CheckValueType(val, fieldType);
				}
			}
			writer.WriteType(fieldType);
			writer.Write((byte)FilterType);
			writer.Write(ShowBlanks);
			writer.Write(Count);
			for(int i = 0; i < Count; i++) {
				writer.WriteObject(values[i]);
			}
		}
		protected internal void LoadFromStream(TypedBinaryReader reader) {
			Type fieldType = reader.ReadType();
			if(fieldType == null) return;
			PivotFilterType filterType = (PivotFilterType)reader.ReadByte();
			bool showBlanks = reader.ReadBoolean();
			int count = reader.ReadInt32();
			object[] values = new object[count];
			for(int i = 0; i < count; i++) {
				values[i] = reader.ReadObject(fieldType);
			}
			SetValues(values, filterType, showBlanks);
		}
		#region IFilterValues Members
		PivotFilterType IFilterValues.FilterType { get { return this.FilterType; } set { this.FilterType = value; } }
		#endregion
		#region IXtraSerializableLayoutEx Members
		public bool AllowProperty(DevExpress.Utils.OptionsLayoutBase options, string propertyName, int id) {
			if(isSerializing && propertyName == "Values" && Count > 0 && !(Values[0] is Enum))
				return false;
			if(isSerializing && propertyName == "ValuesCore" && Count > 0 && Values[0] is Enum)
				return false;
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
			Clear();
		}
		#endregion
		#region IXtraSerializable        
		bool isSerializing;
		List<object> deserializedFilter;
		void IXtraSerializable.OnStartSerializing() {
			isSerializing = true;
		}
		void IXtraSerializable.OnEndSerializing() {
			isSerializing = false;
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
		}
		#endregion 
		#region IXtraSupportDeserializeCollectionItem
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName != "ValuesCore")
				throw new NotImplementedException();
			XtraPropertyInfo propInfo = e.Item.ChildProperties["Value"];
			bool isStringValue = propInfo.Value is string;
			if(propInfo.Value == null || isStringValue && propInfo.PropertyType == null) {
				deserializedFilter.Add(propInfo.Value);
				return new PivotFilterValue(null);
			} else {
				object value;
				if(isStringValue && Data != null && Data.OptionsData.CustomObjectConverter != null && Data.OptionsData.CustomObjectConverter.CanConvert(propInfo.PropertyType))
					value = Data.OptionsData.CustomObjectConverter.FromString(propInfo.PropertyType, (string)propInfo.Value);
				else
					value = propInfo.ValueToObject(propInfo.PropertyType);
				deserializedFilter.Add(value);
				return new PivotFilterValue(value);
			}
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
		}
		#endregion
		#region IXtraSupportDeserializeCollection
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			if(propertyName != "ValuesCore")
				throw new NotImplementedException();
			Values = deserializedFilter.ToArray();
		}
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			if(propertyName != "ValuesCore")
				throw new NotImplementedException();
			deserializedFilter = new List<object>();
		}
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) {
			return false;
		}
		#endregion
		#region IFilterValues 
		CriteriaOperator IFilterValues.GetActualCriteria() {
			if(Data != null && !Data.IsOLAP && Data.IsServerMode && Count > 2000) {
				object[] fvalues = GetUniqueValues(Field);
				if(fvalues != null && fvalues.Length > 2000 && fvalues.Length - Count < 2000)
					return GetActualCriteriaCore(filterType == PivotFilterType.Included ? PivotFilterType.Excluded : PivotFilterType.Included, fvalues.Except(values.EnumerateKeys()));
			}
			return GetActualCriteriaCore(filterType, values.EnumerateKeys());
		}
		private CriteriaOperator GetActualCriteriaCore(PivotFilterType filterType, IEnumerable<object> values) {
			return this.values.Count > 0 ? CriteriaFilterHelper.CreateCriteria(filterType, values, Data == null ? field.Name : Data.GetFilterCriteriaFieldName(field), field.Data.IsForceNullInCriteria()) : null;
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
				PivotGridFilterItems items = new PivotGridFilterItems(Data, Field, false, Data.OptionsFilter.ShowOnlyAvailableItems, true);
				items.PersistentString = value;
				deferFilter = items; 
			}
		}
		IFieldFilter deferFilter;
		protected internal void SetDefereFilter(IFieldFilter filter){
			deferFilter = filter;
		}
		protected internal IFieldFilter GetDefereFilter() {
			return deferFilter;
		}
		protected internal IFieldFilter GetActual(bool deferUpdates) {
			return deferUpdates && deferFilter != null ? deferFilter : this;
		}
	}
	internal class FilterValuesEqualityComparer : IEqualityComparer {
		readonly ValueComparer valueComparer = new ValueComparer();
		readonly bool caseSensitive;
		static FilterValuesEqualityComparer sensitive;
		static FilterValuesEqualityComparer insensitive;
		public static FilterValuesEqualityComparer Create(bool caseSensitive) {
			if(caseSensitive) {
				if(sensitive == null)
					sensitive = new FilterValuesEqualityComparer(true);
				return sensitive;
			} else {
				if(insensitive == null)
					insensitive = new FilterValuesEqualityComparer(false);
				return insensitive;
			}
		}
		FilterValuesEqualityComparer(bool caseSensitive) {
			this.caseSensitive = caseSensitive;
		}
		protected bool CaseSensitive {
			get { return caseSensitive; }
		}
		protected StringComparison StringComparision {
			get {
				return CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
			}
		}
		protected StringComparer StringComparer {
			get {
				return CaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase;
			}
		}
		bool IEqualityComparer.Equals(object x, object y) {
			string val1 = x as string,
				val2 = y as string;
			if(val1 != null && val2 != null)
				return String.Equals(val1, val2, StringComparision);
			else
				return valueComparer.Compare(x, y) == 0;
		}
		int IEqualityComparer.GetHashCode(object obj) {
#if DXPORTABLE
			string text = obj as string;
			if (text != null)
				return StringComparer.GetHashCode(text);
			if (obj != null)
				return obj.GetHashCode();
			else
				return StringComparer.GetHashCode(text); 
#else
			return StringComparer.GetHashCode(obj);
#endif
		}
	}
}
