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
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridFilterItem : IFilterItem {
		object value;
		public PivotGridFilterItem(object value, string text, bool? isChecked)
			: this(value, text, isChecked, true) {
		}
		public PivotGridFilterItem(object value, string text, bool? isChecked, bool isVisible) {
			this.value = value;
			Text = text;
			IsChecked = isChecked;
			IsVisible = isVisible;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFilterItemValue")]
#endif
		public object Value {
			get { return value; }
			set {
				this.value = value;
				Text = value.ToString();
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFilterItemIsChecked")]
#endif
		public bool? IsChecked { get; set; }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFilterItemIsBlank")]
#endif
		public bool IsBlank { get { return Value == null; } }
		public bool IsVisible { get; set; }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFilterItemLevel")]
#endif
		public virtual int Level { get { return 0; } }
		public string Text { get; set; }
		public override string ToString() { return Text; }
		internal void EnsureValueType(Type type) {
			try {
				if(Value != null && Value != DBNull.Value && !type.IsInstanceOfType(Value))
					this.value = Convert.ChangeType(this.value, type, CultureInfo.CurrentCulture);
			} catch {
				throw new IncorrectFilterItemValueTypeException();
			}
		}
	}
	public interface IFieldFilter {
		PivotFilterType FilterType { get; }
		bool ShowBlanks { get; }
		object[] Values { get; }
		NullableHashtable HashTable { get; }
		bool HasFilter { get; }
		string PersistentString { get; }
	}
	public class PivotGridFilterItems : PivotFilterItemsBase, IFieldFilter, IFilterValues {
		Collection<PivotGridFilterItem> allItems;
		PivotGridFilterItem blankItem;
		public PivotGridFilterItems(PivotGridData data, PivotGridFieldBase field)
			: this(data, field, false, false, false) {
		}
		public PivotGridFilterItems(PivotGridData data, PivotGridFieldBase field, bool radioMode)
			: this(data, field, radioMode, false, false) {
		}
		public PivotGridFilterItems(PivotGridData data, PivotGridFieldBase field, bool radioMode, bool showOnlyAvailableItems)
			: this(data, field, radioMode, showOnlyAvailableItems, false) {
		}
		public PivotGridFilterItems(PivotGridData data, PivotGridFieldBase field, bool radioMode, bool showOnlyAvailableItems, bool deferUpdates)
			: base(data, field, radioMode, showOnlyAvailableItems, deferUpdates) {
			this.allItems = new Collection<PivotGridFilterItem>();
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFilterItemsFilterValues")]
#endif
		public override IFilterValues FilterValues { get { return FieldFilterValues; } }
		PivotGridFieldFilterValues FieldFilterValues { get { return Field.FilterValues; } }
		protected override bool ApplyFilterCore(bool allowOnChanged) {
			return
				Data.CanDoRefresh ?
					FieldFilterValues.SetValuesAsync(GetFilteredValues(), FilterType, FilterShowBlank, allowOnChanged) :
						FieldFilterValues.SetValues(GetFilteredValues(), FilterType, FilterShowBlank, allowOnChanged);
		}
		PivotFilterType FilterType {
			get {
				return ShowNewValues ? PivotFilterType.Excluded : PivotFilterType.Included;
			}
		}
		object[] IFieldFilter.Values { get { return GetFilteredValues().ToArray(); } }
		PivotFilterType IFieldFilter.FilterType { get { return FilterType; } }
		bool IFieldFilter.ShowBlanks { get { return FilterShowBlank; } }
		NullableHashtable IFieldFilter.HashTable {
			get {
				NullableHashtable table = new NullableHashtable();
				foreach(object value in GetFilteredValues())
					table.Add(value, true);
				return table;
			}
		}
		bool IFieldFilter.HasFilter { get { return true; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFilterItemsPersistentString")]
#endif
		public override string PersistentString {
			get {
				return PivotGridSerializeHelper.ToBase64String(writer => {
					writer.WriteType(ColumnType);
					writer.Write(Count);
					foreach(PivotGridFilterItem item in this) {
						if(ColumnType == typeof(object))
							writer.WriteTypedObject(item.Value);
						else
							writer.WriteObject(item.Value);
						writer.Write(item.IsChecked == true);
						writer.Write(item.IsVisible);
					}
				}, Data.OptionsData.CustomObjectConverter);
			}
			set {
				if(value == null || value == string.Empty)
					return;
				ClearItems();
				deserializing = true;
				MemoryStream stream = new MemoryStream(Convert.FromBase64String(value));
				TypedBinaryReader reader = TypedBinaryReader.CreateReader(stream, Data.OptionsData.CustomObjectConverter);
				Type columnType = reader.ReadType();
				int count = reader.ReadInt32();
				for(int i = 0; i < count; i++) {
					object val = columnType == typeof(object) ? reader.ReadTypedObject() : reader.ReadObject(columnType);
					Add(val, GetDisplayText(val), reader.ReadBoolean(), reader.ReadBoolean());
				}
				UpdateBlankItem();
				reader.Dispose();
				stream.Dispose();
				deserializing = false;
			}
		}
		bool deserializing;
		protected override void ClearItems() {
			base.ClearItems();
			this.allItems.Clear();
			this.blankItem = null;
		}
		protected override PivotGridFilterItem CreatePivotFilterItem(IFilterItem parent, int level, object value, string text, bool? isChecked, bool isVisible) {
			return new PivotGridFilterItem(value, text, isChecked, isVisible);
		}
		protected override bool ActualShowNewValues { get { return Field.ShowNewValues; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFilterItemsGroup")]
#endif
		public override PivotGridGroup Group { get { return null; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public bool HasBlankItem { get { return blankItem != null && blankItem.IsBlank; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public PivotGridFilterItem BlankItem {
			get { return HasBlankItem ? blankItem : null; }
		}
		protected bool CanShowBlankItem {
			get {
				return !Data.IsOLAP;
			}
		}
		protected bool FilterShowBlank {
			get {
				if(!HasBlankItem)
					return ShowBlanks;
				return BlankItem.IsChecked == true;
			}
		}
		protected string ShowBlanksItemCaption { get { return GetDisplayTextCore(null, 0, true); } }
		protected virtual List<object> GetFilteredValues() {
			List<object> values = new List<object>();
			bool includeState = !ShowNewValues;
			foreach(PivotGridFilterItem item in allItems) {
				if(item != BlankItem && item.IsChecked == includeState)
					values.Add(item.Value);
			}
			return values;
		}
		protected override void InsertItem(int index, PivotGridFilterItem item) {
			EnsureItemValueType(item);
			base.InsertItem(index, item);
			allItems.Add(item);
		}
		protected override void SetItem(int index, PivotGridFilterItem item) {
			EnsureItemValueType(item);
			int i = (this[index] == allItems[index]) ? index : allItems.IndexOf(this[index]);
			base.SetItem(index, item);
			allItems[i] = item;
		}
		void EnsureItemValueType(PivotGridFilterItem item) {
			if(!deserializing && Data.IsFieldTypeCheckRequired(Field))
				item.EnsureValueType(ColumnType);
		}
		protected override Collection<PivotGridFilterItem> AllItems { get { return allItems; } }
		protected override void Add(object value, string text, bool? isChecked, bool isVisible) {
			Add(CreatePivotFilterItem(null, 0, value, text, isChecked, isVisible));
		}
		protected override void AddValues() {
			if(ShowOnlyAvailableItems) {
				AddValuesCore(Data.GetSortedUniqueValues(Field), false);
				ModifyValuesVisibility(Data.GetAvailableFieldValues(Field, DeferUpdates));
			} else
				AddValuesCore(Data.GetSortedUniqueValues(Field), true);
			Data.OnCustomFilterPopupItems(this);
		}
		protected override void AddValuesAsync(AsyncCompletedHandler completed) {
			if(ShowOnlyAvailableItems) {
				DataAsync.GetSortedUniqueValuesAsync(Field, false, sortedValues => {
					AddValuesCore((IList)sortedValues.Value, false);
					DataAsync.GetAvailableFieldValuesAsync(Field, false, availableValues => {
						ModifyValuesVisibility((IEnumerable<object>)availableValues.Value);
						Data.OnCustomFilterPopupItems(this);
						if(completed != null)
							completed.Invoke(availableValues);
					}, DeferUpdates);
				});
			} else {
				DataAsync.GetSortedUniqueValuesAsync(Field, false, result => {
					AddValuesCore((IList)result.Value, true);
					Data.OnCustomFilterPopupItems(this);
					if(completed != null)
						completed.Invoke(result);
				});
			}
		}
		public override void EnsureAvailableItems() {
			if(ShowOnlyAvailableItems) {
				for(int i = 0; i < Count; i++)
					this[i].IsVisible = false;
				ModifyValuesVisibility(Data.GetAvailableFieldValues(Field, DeferUpdates));
			}
		}
		public override void EnsureAvailableItemsAsync(AsyncCompletedHandler completed) {
			if(ShowOnlyAvailableItems) {
				DataAsync.GetAvailableFieldValuesAsync(Field, false, availableValues => {
					for(int i = 0; i < Count; i++)
						this[i].IsVisible = false;
					ModifyValuesVisibility((IEnumerable<object>)availableValues.Value);
					if(completed != null)
						completed.Invoke(availableValues);
				}, DeferUpdates);
			} else {
				base.EnsureAvailableItemsAsync(completed);
			}
		}
		void AddValuesCore(IList values, bool isVisible) {
			AddBlankValue();
			if(values == null)
				return;
			bool removeNull = CanShowBlankItem;
			foreach(object value in values) {
				if(removeNull && object.ReferenceEquals(null, value))
					continue;
				Add(value, GetDisplayText(value), IsItemChecked(value), isVisible);
			}
			UpdateBlankItem();
		}
		void ModifyValuesVisibility(IEnumerable<object> valuesToModify) {
			List<object> commonList = new List<object>();
			commonList.AddRange(valuesToModify);
			for(int i = 0; i < Count; i++)
				commonList.Add(this[i]);
			commonList.Sort((a, b) => {
				object val1 = a, val2 = b;
				PivotGridFilterItem item1 = val1 as PivotGridFilterItem;
				if(item1 != null)
					val1 = item1.Value;
				PivotGridFilterItem item2 = val2 as PivotGridFilterItem;
				if(item2 != null)
					val2 = item2.Value;
				if(val1 == null) {
					if(val2 == null)
						return 0;
					else
						return -1;
				}
				if(val2 == null)
					return 1;
				try {
					return Comparer.Default.Compare(val1, val2);
				} catch {
					return Comparer.Default.Compare(val1.ToString(), val2.ToString());
				}
			});
			for(int i = 0; i < commonList.Count - 1; i++) {
				object val1 = commonList[i];
				object val2 = commonList[i + 1];
				PivotGridFilterItem item1 = val1 as PivotGridFilterItem;
				PivotGridFilterItem item2 = val2 as PivotGridFilterItem;
				if(item1 != null && item2 == null && object.Equals(item1.Value, val2))
					item1.IsVisible = true;
				if(item2 != null && item1 == null && object.Equals(item2.Value, val1))
					item2.IsVisible = true;
			}
		}
		void UpdateBlankItem() {
			this.blankItem = (CanShowBlankItem && Count > 0 && this[0].IsBlank) ? this[0] : null;
		}
		void AddBlankValue() {
			if(!CanShowBlankItem || !HasNullValues)
				return;
			Add(null, ShowBlanksItemCaption, IsItemChecked(null), true);
		}
		bool ShowBlanks { get { return FieldFilterValues.ShowBlanks; } }
		public override List<object> LoadValues(object[] parentValues) { throw new InvalidOperationException("Cannot load non-group values"); }
		public override void LoadValuesAsync(object[] parentValues, AsyncCompletedHandler asyncCompleted) { throw new InvalidOperationException("Cannot load non-group values"); }
		protected virtual bool? IsItemChecked(object value) {
			if(value == null && CanShowBlankItem)
				return ShowBlanks;
			return FieldFilterValues.Contains(value);
		}
		protected override void UpdateBlankItemCaptions() {
			if(HasBlankItem)
				BlankItem.Text = ShowBlanksItemCaption;
		}
		#region IFilterValues
		PivotFilterType IFilterValues.FilterType {
			get { return FilterType; }
			set { throw new ArgumentException(); }
		}
		CriteriaOperator IFilterValues.GetActualCriteria() {
			if(!CanAccept)
				return null;
			List<object> included = new List<object>();
			List<object> excluded = new List<object>();
			foreach(PivotGridFilterItem item in AllItems) {
				if(item.IsChecked == true)
					included.Add(item.Value);
				else
					excluded.Add(item.Value);
			}
			if(included.Count == Count || excluded.Count == Count || included.Count == 0 || excluded.Count == 0)
				return null;
			PivotFilterType filterType;
			List<object> values;
			if(included.Count > excluded.Count) {
				filterType = PivotFilterType.Excluded;
				values = excluded;
			} else {
				filterType = PivotFilterType.Included;
				values = included;
			}
			return CriteriaFilterHelper.CreateCriteria(filterType, values, Field.Data == null ? Field.Name : Data.GetFilterCriteriaFieldName(Field), Field.Data.IsForceNullInCriteria());
		}
		#endregion
	}
}
