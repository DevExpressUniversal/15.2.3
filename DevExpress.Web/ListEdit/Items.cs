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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	abstract public class ListEditItemCollectionBase : Collection {
		bool dataBinding;
		public ListEditItemCollectionBase()
			: base() {
		}
		public ListEditItemCollectionBase(IWebControlObject owner)
			: base(owner) {
		}
		protected abstract bool ConvertEmptyStringToNull {
			get;
		}
		protected abstract Type GetValueType();
		protected abstract void OnCleared();
		protected abstract void OnItemDeleting(ListEditItemBase item);
		protected virtual internal Type ItemValueType {
			get {
				return GetValueType();
			}
		}
		protected internal bool DataBinding {
			get { return dataBinding; }
			set { dataBinding = value; }
		}
		protected virtual ListEditItemBase CreateItem() {
			return new ListEditItemBase();
		}
		protected void Add(ListEditItemBase item) {
			base.Add(item);
		}
		protected virtual ListEditItemBase Add() {
			ListEditItemBase item = CreateItem();
			Add(item);
			return item;
		}
		protected virtual ListEditItemBase Add(string text) {
			return Add(text, text);
		}
		protected virtual ListEditItemBase Add(string text, object value) {
			ListEditItemBase item = CreateItem();
			item.Text = text;
			item.Value = value;
			Add(item);
			return item;
		}
		public void AddRange(ICollection collection) {
			foreach(object objItem in collection) {
				ListEditItemBase item = objItem as ListEditItemBase;
				if(item != null)
					Add(CloneItem(item));
				else
					Add(objItem.ToString());
			}
		}
		public new void Clear() {
			base.Clear();
			OnCleared();
		}
		protected ListEditItemBase GetItemInternal(int index) {
			return this.GetItem(index) as ListEditItemBase;
		}
		protected int IndexOf(ListEditItemBase item) {
			return base.IndexOf(item);
		}
		public int IndexOfText(string text) {
			for(int i = 0; i < Count; i++) {
				if(string.Equals(GetItemInternal(i).Text, text))
					return i;
			}
			return -1;
		}
		public int IndexOfTextWithTrim(string text) {
			string trimmedText = text.Trim();
			string trimmedItemText = "";
			for(int i = 0; i < Count; i++) {
				trimmedItemText = GetItemInternal(i).Text.Trim();
				if(string.Equals(trimmedText, trimmedItemText))
					return i;
			}
			return -1;
		}
		public int IndexOfValue(object value) {
			for(int i = 0; i < Count; i++) {
				if(CommonUtils.AreEqual(GetItemInternal(i).Value, value, ConvertEmptyStringToNull))
					return i;
			}
			return -1;
		}
		protected ListEditItemBase FindByTextInternal(string text) {
			int index = IndexOfText(text);
			return index != -1 ? GetItemInternal(index) : null;
		}
		protected ListEditItemBase FindByTextWithTrimInternal(string text) {
			int index = IndexOfTextWithTrim(text);
			return index != -1 ? GetItemInternal(index) : null;
		}
		protected ListEditItemBase FindByValueInternal(object value) {
			int index = IndexOfValue(value);
			return index != -1 ? GetItemInternal(index) : null;
		}
		protected void Insert(int index, ListEditItemBase item) {
			base.Insert(index, item);
		}
		protected void Remove(ListEditItemBase item) {
			OnItemDeleting(item);
			base.Remove(item);
		}
		public new void RemoveAt(int index) {
			OnItemDeleting(GetItemInternal(index));
			base.RemoveAt(index);
		}
		internal Type FindFirstNonNullValueType() {
			for(int i = 0; i < Count; i++) {
				if(GetItemInternal(i).Value != null)
					return GetItemInternal(i).Value.GetType();
			}
			return null;
		}
		protected override Type GetKnownType() {
			return typeof(ListEditItemBase);
		}
		protected internal bool IsIndexValid(int index) {
			return 0 <= index && index < Count;
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			ListEditItemBase item = value as ListEditItemBase;
			if(item != null)
				item.OnAddedToCollection();
		}
		protected internal new void BeginUpdate() {
			base.BeginUpdate();
		}
		protected internal new void EndUpdate() {
			base.EndUpdate();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class ListEditItemCollection : ListEditItemCollectionBase {
		public ListEditItemCollection()
			: base() {
		}
		public ListEditItemCollection(IWebControlObject owner)
			: base(owner) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("ListEditItemCollectionItem")]
#endif
		public ListEditItem this[int index] {
			get { return (GetItem(index) as ListEditItem); }
		}
		public void Add(ListEditItem item) {
			base.Add(item);
		}
		protected override ListEditItemBase CreateItem() {
			return new ListEditItem();
		}
		public new ListEditItem Add() {
			return base.Add() as ListEditItem;
		}
		public new ListEditItem Add(string text) {
			return Add(text, text);
		}
		public new ListEditItem Add(string text, object value) {
			return base.Add(text, value) as ListEditItem;
		}
		public virtual ListEditItemBase Add(string text, object value, string imageUrl) {
			ListEditItem item = new ListEditItem(text, value, imageUrl);
			Add(item);
			return item;
		}
		public ListEditItem FindByText(string text) {
			return base.FindByTextInternal(text) as ListEditItem;
		}
		public ListEditItem FindByTextWithTrim(string text) {
			return base.FindByTextWithTrimInternal(text) as ListEditItem;
		}
		public ListEditItem FindByValue(object value) {
			return base.FindByValueInternal(value) as ListEditItem;
		}
		public int IndexOf(ListEditItem item) {
			return base.IndexOf(item);
		}
		public void Insert(int index, ListEditItem item) {
			base.Insert(index, item);
		}
		public void Remove(ListEditItem item) {
			base.Remove(item);
		}
		protected ListEditProperties ListEditProperties {
			get { return Owner as ListEditProperties; }
		}
		protected override bool ConvertEmptyStringToNull {
			get {
				return (ListEditProperties != null) ? ListEditProperties.ConvertEmptyStringToNull : false;
			}
		}
		protected internal virtual ListEditItemCollection CreateEmptyClone() {
			return new ListEditItemCollection();
		}
		protected internal bool? GetItemSelected(ListEditItemBase item) {
			return (ListEditProperties != null) ?
				ListEditProperties.GetItemSelected(item as ListEditItem) : null;
		}
		protected override Type GetKnownType() {
			return typeof(ListEditItem);
		}
		protected override Type GetValueType() {
			return ListEditProperties != null ? ListEditProperties.ValueType : null;
		}
		protected override void OnChanged() {
			if(ListEditProperties != null)
				ListEditProperties.OnItemsChanged();
		}
		protected override void OnCleared() {
			if(ListEditProperties != null)
				ListEditProperties.OnItemsCleared();
		}
		protected override void OnItemDeleting(ListEditItemBase item) {
			if(ListEditProperties != null)
				ListEditProperties.OnItemDeleting(item as ListEditItem);
		}
		protected internal void OnItemSelectionChanged(ListEditItemBase item, bool selected) {
			if(ListEditProperties != null)
				ListEditProperties.OnItemSelectionChanged(item as ListEditItem, selected);
		}
	}
	public class ListEditItemBase : CollectionItem {
		private bool synchronizeValue = true;
		private bool synchronizeText = true;
		private ITemplate textTemplate = null;
		private ListEditDataItemWrapper dataItemWrapper;
		int valueConvertationLockCount = 0;
		public ListEditItemBase()
			: base() {
		}
		public ListEditItemBase(string text)
			: this(text, text) {
		}
		public ListEditItemBase(string text, object value)
			: this() {
			Text = text;
			Value = value;
		}
		protected internal object DataItem {
			get { return dataItemWrapper != null ? dataItemWrapper.DataItem : null; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ListEditItemBaseText")]
#endif
		public virtual string Text {
			get { return IsDataItemAssigned ? DataItemWrapper.Text : GetStringProperty("Text", ""); }
			set {
				if(IsDataItemAssigned)
					throw new InvalidOperationException();
				else
					SetStringProperty("Text", "", value);
				synchronizeText = false;
				if(synchronizeValue && (Value == null || string.IsNullOrEmpty(Value as string)))
					SetConvertibleValue(value);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ListEditItemBaseValue")]
#endif
		public virtual object Value {
			get { 
				return IsDataItemAssigned ? 
					ConvertValueInternal(DataItemWrapper.Value) : 
					GetObjectProperty("Value", null); 
			} set {
				if(IsDataItemAssigned && !IsDataItemValuesVariable)
					return;
				if(IsDataItemValuesVariable)
					DataItemWrapper.Value = value;
				else
					SetValue(value);
				synchronizeValue = false;
				if(synchronizeText && string.IsNullOrEmpty(GetStringProperty("Text", "")))
					SetStringProperty("Text", "", value == null ? ToString() : value.ToString());
			}
		}
		[DefaultValue(""), NotifyParentProperty(true), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public string ValueString {
			get { return ""; }
			set { Value = value; }
		}
		protected override IDesignTimeCollectionItem GetDesignTimeItemParent() {
			return DesignTimeItemsHelper.FindParentBand(this);
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		TemplateContainer(typeof(ListEditItemTemplateContainer))]
		protected internal ITemplate TextTemplate {
			get { return textTemplate; }
			set {
				textTemplate = value;
				TemplatesChanged();
			}
		}
		protected ListEditDataItemWrapper DataItemWrapper {
			get { return dataItemWrapper; }
		}
		protected bool IsDataItemAssigned { get { return DataItemWrapper != null; } }
		private bool IsDataItemValuesVariable { get { return DataItemWrapper is ListEditUnboundDataItemWrapper; } }
		protected bool DataBinding {
			get {
				ListEditItemCollection itemCollection = Collection as ListEditItemCollection;
				return itemCollection != null ? itemCollection.DataBinding : false;
			}
		}
		protected internal bool ValueConvertationLocked {
			get { return valueConvertationLockCount > 0; }
		}
		protected internal void LockValueConvertation() {
			this.valueConvertationLockCount++;
		}
		protected internal void UnlockValueConvertation() {
			this.valueConvertationLockCount--;
		}
		public override void Assign(CollectionItem source) {
			ListEditItemBase src = source as ListEditItemBase;
			if(src != null) {
				Text = src.Text;
				Value = src.Value;
				TextTemplate = src.TextTemplate;
				SetDataItemWrapper(src.DataItemWrapper);
			}
			base.Assign(source);
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			synchronizeValue = false;
			synchronizeText = false;
		}
		public object GetValue(string fieldName) { 
			return this.dataItemWrapper != null ?
				this.dataItemWrapper.GetValue(fieldName) : null;
		}
		public void SetValue(string fieldName, object value) {
			if(this.dataItemWrapper != null)
				this.dataItemWrapper.SetValue(fieldName, value);
		}
		public override string ToString() {
			return (Text != "") ? Text : GetType().Name;
		}
		protected internal virtual ListEditItemBase Clone(){
			ListEditItemBase clone = new ListEditItemBase();
			clone.Assign(this);
			return clone;
		}
		protected internal object[] GetVisibleColumnValues() {
			return DataItemWrapper.GetVisibleColumnValues();
		}
		protected internal void SetDataItemWrapper(ListEditDataItemWrapper dataItemWrapper) {
			this.dataItemWrapper = dataItemWrapper;
		}
		protected internal void ConvertValue() {
			if(!ValueConvertationLocked) {
				object convertValue = ConvertValueInternal(Value);
				if(Value != convertValue)
					Value = convertValue;
			}
		}
		protected internal object ConvertValueInternal(object value) {
			ListEditItemCollection collection = Collection as ListEditItemCollection;
			if(collection == null)
				return value;
			Type type = collection.ItemValueType;
			return CommonUtils.GetConvertedArgumentValue(value, type, "Item[]");
		}
		protected virtual internal void OnAddedToCollection() {
			ConvertValue();
		}
		protected void SetValue(object value) {
			value = ConvertValueInternal(value);
			SetObjectProperty("Value", null, value);
		}
		protected void SetConvertibleValue(object value) {
			if(Collection != null) {
				try {
					SetValue(value);
				} catch(ArgumentException) {
					SetObjectProperty("Value", 0, null);
				}
			}
		}
	}
	[ControlBuilder(typeof(ListEditItemBuilder))]
	public class ListEditItem : ListEditItemBase {
		private bool cachedSelection = false;
		#region ctor
		public ListEditItem()
			: base() {
		}
		public ListEditItem(string text)
			: base(text) {
		}
		public ListEditItem(string text, object value)
			: base(text, value) {
		}
		public ListEditItem(string text, object value, string imageUrl)
			: base(text, value) {
			ImageUrl = imageUrl;
		}
		#endregion
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditItemImageUrl"),
#endif
		DefaultValue(""), Bindable(true), NotifyParentProperty(true), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty, AutoFormatUrlProperty]
		public string ImageUrl {
			get { return IsDataItemAssigned ? DataItemWrapper.ImageUrl : GetStringProperty("ImageUrl", ""); }
			set {
				if(IsDataItemAssigned)
					DataItemWrapper.ImageUrl = value;
				else
					SetStringProperty("ImageUrl", "", value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditItemSelected"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool Selected {
			get { return GetSelected(); }
			set { SetSelected(value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditItemText"),
#endif
		Localizable(true), Bindable(true), DefaultValue(""), RefreshProperties(RefreshProperties.All),
		NotifyParentProperty(true), AutoFormatDisable]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditItemValue"),
#endif
		DefaultValue(null), NotifyParentProperty(true), RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(StringToObjectTypeConverter))]
		public override object Value {
			get { return base.Value; }
			set { base.Value = value; }
		}
		public override void Assign(CollectionItem source) {
			ListEditItem src = source as ListEditItem;
			if(src != null)
				ImageUrl = src.ImageUrl;
			base.Assign(source);
		}
		protected override internal void OnAddedToCollection() {
			base.OnAddedToCollection();
			FlushSelectionCache();
		}
		protected internal bool CachedSelection {
			get { return this.cachedSelection; }
		}
		protected void CacheSelection(bool selected) {
			this.cachedSelection = selected;
		}
		protected internal new ListEditItem Clone() {
			ListEditItem clone = new ListEditItem();
			clone.Assign(this);
			return clone;
		}
		protected void FlushSelectionCache() {
			if(!DataBinding && !Selected)
				Selected = CachedSelection;
		}
		protected bool GetSelected() {
			ListEditItemCollection itemCollection = Collection as ListEditItemCollection;
			if(itemCollection != null) {
				bool? collectionItemSelected = itemCollection.GetItemSelected(this);
				if(collectionItemSelected != null)
					return collectionItemSelected.Value;
			}
			return CachedSelection;
		}
		protected virtual void OnSelectionChanged(bool selected) {
			ListEditItemCollection itemCollection = Collection as ListEditItemCollection;
			if(itemCollection != null)
				itemCollection.OnItemSelectionChanged(this, selected);
		}
		protected void SetSelected(bool selected) {
			if(GetSelected() != selected) {
				OnSelectionChanged(selected);
			}
			CacheSelection(selected);
		}
	}
	public class ListEditItemTemplateContainer : ItemTemplateContainerBase {
		private ListEditItem item = null;
#if !SL
	[DevExpressWebLocalizedDescription("ListEditItemTemplateContainerItem")]
#endif
		public ListEditItem Item {
			get { return item; }
		}
		public ListEditItemTemplateContainer(ListEditItem item)
			: base(item.Index, item) {
			this.item = item;
		}
	}
	public abstract class SelectedItemAndIndexCollectionBase : ICollection, IEnumerable {
		IMultiSelectListEdit listEdit;
		internal SelectedItemAndIndexCollectionBase(IMultiSelectListEdit listEdit) {
			this.listEdit = listEdit;
		}
		protected IMultiSelectListEdit ListEdit {
			get { return listEdit; }
		}
		protected SelectedValueCollection SelectedValues {
			get { return ListEdit.SelectedValues; }
		}
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			ArrayList indices = new ArrayList();
			for(int i = 0; i < SelectedValues.Count; i++)
				indices.Add(SelectedValues[i]);
			indices.CopyTo(array, index);
		}
#if !SL
	[DevExpressWebLocalizedDescription("SelectedItemAndIndexCollectionBaseCount")]
#endif
		public int Count {
			get { return SelectedValues.Count; }
		}
		bool ICollection.IsSynchronized {
			get { return (SelectedValues as ICollection).IsSynchronized; }
		}
		object ICollection.SyncRoot {
			get { return (SelectedValues as ICollection).SyncRoot; }
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return null;
		}
		#endregion
	}
	public class SelectedIndexCollection : SelectedItemAndIndexCollectionBase, ICollection, IEnumerable {
		internal SelectedIndexCollection(IMultiSelectListEdit listEdit)
			: base(listEdit) {
		}
		public bool Contains(int index) {
			ListEditItem item = ListEdit.Items[index];
			return SelectedValues.Contains(item.Value);
		}
		public int this[int index] {
			get {
				object itemValue = SelectedValues[index];
				ListEditItem item = ListEdit.Items.FindByValue(itemValue);
				return item.Index;
			}
		}
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			ArrayList indices = new ArrayList();
			ListEditItem selectedItem;
			for(int i = 0; i < SelectedValues.Count; i++) {
				selectedItem = ListEdit.Items.FindByValue(SelectedValues[i]);
				indices.Add(selectedItem.Index);
			}
			indices.CopyTo(array, index);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return new SelectedIndexCollectionIEnumerator(((IEnumerable)SelectedValues).GetEnumerator(), ListEdit);
		}
		#endregion
	}
	public class SelectedItemCollection : SelectedItemAndIndexCollectionBase, ICollection, IEnumerable {
		internal SelectedItemCollection(IMultiSelectListEdit listEdit)
			: base(listEdit) {
		}
		public virtual bool Contains(ListEditItem item) {
			return SelectedValues.Contains(item.Value);
		}
		public virtual ListEditItem this[int index] {
			get {
				object itemValue = SelectedValues[index];
				return ListEdit.Items.FindByValue(itemValue);
			}
		}
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			ArrayList selectedItems = new ArrayList(SelectedValues.Count);
			ListEditItem selectedItem;
			for(int i = 0; i < SelectedValues.Count; i++) {
				selectedItem = ListEdit.Items.FindByValue(SelectedValues[i]);
				selectedItems[i] = selectedItem;
			}
			selectedItems.CopyTo(array, index);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return new SelectedItemCollectionIEnumerator(((IEnumerable)SelectedValues).GetEnumerator(), ListEdit);
		}
		#endregion
	}
	public class SelectedValueCollectionBase : ICollection, IEnumerable, IStateManager {
		private ArrayList values;
		protected bool sortingRequired = false;
		protected int valueSortingLockCount = 0;
		protected internal delegate void SortMethod();
		protected SortMethod Sort;
		public SelectedValueCollectionBase() {
		}
		public object this[int index] {
			get {
				return values[index];
			}
		}
		protected ArrayList Values {
			get {
				if(values == null)
					values = new ArrayList();
				if(sortingRequired)
					SortValues();
				return values;
			}
		}
		public bool Contains(Object value) {
			LockValuesSorting();
			bool contains = Values.Contains(value);
			if(!contains && ConvertEmptyStringToNull) {
				bool isValueNullOrEmptyString = value == null || string.Empty.Equals(value);
				if(isValueNullOrEmptyString) {
					value = value == null ? "" : null;
					contains = Values.Contains(value);
				}
			}
			UnlockValuesSorting();
			return contains;
		}
		protected internal void AddInternal(object value) {
			LockValuesSorting();
			Values.Add(value);
			UnlockValuesSorting();
			SortingRequires();
		}
		protected internal void ClearSelection() {
			LockValuesSorting();
			Values.Clear();
			UnlockValuesSorting();
		}
		protected internal void RemoveInternal(object value) {
			LockValuesSorting();
			Values.Remove(value);
			UnlockValuesSorting();
		}
		protected void SortingRequires() {
			if(valueSortingLockCount == 0)
				this.sortingRequired = true;
		}
		protected void LockValuesSorting() {
			valueSortingLockCount++;
		}
		protected void UnlockValuesSorting() {
			valueSortingLockCount--;
		}
		protected void SortValues() {
			if(valueSortingLockCount != 0)
				return;
			LockValuesSorting();
			if(Sort != null && Values.Count > 1) {
				Sort();
				this.sortingRequired = false;
			}
			UnlockValuesSorting();
		}
		protected internal void SetSortMethod(SortMethod sortMethod) {
			this.Sort = sortMethod;
		}
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			Values.CopyTo(array, index);
		}
		public int Count {
			get { return Values.Count; }
		}
		bool ICollection.IsSynchronized {
			get { return Values.IsSynchronized; }
		}
		object ICollection.SyncRoot {
			get { return Values; }
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return Values.GetEnumerator();
		}
		#endregion
		#region IStateManager Members
		bool IStateManager.IsTrackingViewState {
			get { return Tracking; }
		}
		void IStateManager.LoadViewState(object state) {
			this.values = state as ArrayList;
		}
		object IStateManager.SaveViewState() {
			LockValuesSorting();
			ArrayList values = Values;
			UnlockValuesSorting();
			return values;
		}
		bool tracking;
		protected bool Tracking { set { tracking = value; } get { return tracking; } }
		void IStateManager.TrackViewState() {
			Tracking = true;
		}
		#endregion
		protected virtual bool ConvertEmptyStringToNull { get { return false; } }
	}
	public class SelectedValueCollection : SelectedValueCollectionBase {
		ASPxListEdit listEdit;
		public SelectedValueCollection(ASPxListEdit listEdit) {
			this.listEdit = listEdit;
		}
		protected ASPxListEdit ListEdit {
			get { return listEdit; }
		}
		protected ListEditItemCollection Items {
			get { return ListEdit.Items; }
		}
		protected override bool ConvertEmptyStringToNull {
			get { return ListEdit.Properties.ConvertEmptyStringToNull; }
		}
	}
}
namespace DevExpress.Web.Internal {
	public interface IListEditItemCollectionOwner {
		ListEditItemCollection Items { get; }
	}
	public abstract class SelectedIndicesAndItemsCollectionIEnumeratorBase : IEnumerator {
		IEnumerator valueEnumerator;
		IMultiSelectListEdit listEdit;
		public SelectedIndicesAndItemsCollectionIEnumeratorBase(IEnumerator valueEnumerator, IMultiSelectListEdit listEdit) {
			this.valueEnumerator = valueEnumerator;
			this.listEdit = listEdit;
		}
		protected ListEditItem CurrentItem {
			get { return listEdit.Items.FindByValue(valueEnumerator.Current); }
		}
		bool IEnumerator.MoveNext() { return valueEnumerator.MoveNext(); }
		void IEnumerator.Reset() { valueEnumerator.Reset(); }
		object IEnumerator.Current { get { return null; } }
	}
	public class SelectedIndexCollectionIEnumerator : SelectedIndicesAndItemsCollectionIEnumeratorBase, IEnumerator {
		public SelectedIndexCollectionIEnumerator(IEnumerator valueEnumerator, IMultiSelectListEdit listEdit)
			: base(valueEnumerator, listEdit) {
		}
		object IEnumerator.Current {
			get {
				ListEditItem curentItem = CurrentItem;
				return curentItem != null ? curentItem.Index : -1;
			}
		}
	}
	public class SelectedItemCollectionIEnumerator : SelectedIndicesAndItemsCollectionIEnumeratorBase, IEnumerator {
		public SelectedItemCollectionIEnumerator(IEnumerator valueEnumerator, IMultiSelectListEdit listEdit)
			: base(valueEnumerator, listEdit) {
		}
		object IEnumerator.Current { get { return CurrentItem; } }
	}
}
