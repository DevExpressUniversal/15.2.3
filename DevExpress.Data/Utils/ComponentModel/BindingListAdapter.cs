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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using System.Windows;
using DevExpress.Data.Helpers;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using System.Windows.Threading;
using DevExpress.Data.Utils;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.Data.Helpers {
	[Flags]
	public enum ItemPropertyNotificationMode { 
		None = 0,
		PropertyChanged = 1
	}
	public class BindingListAdapterBase : IBindingList, IDisposable {
		static readonly PropertyChangedEventArgs EmptyEventArgs = new PropertyChangedEventArgs(null);
		public static BindingListAdapterBase CreateFromList(IList list){
			return CreateFromList(list, ItemPropertyNotificationMode.PropertyChanged);
		}
		public static BindingListAdapterBase CreateFromList(IList list, ItemPropertyNotificationMode itemPropertyNotificationMode) {
			return list is ITypedList ? new TypedListBindingListAdapterBase(list, itemPropertyNotificationMode) : new BindingListAdapterBase(list, itemPropertyNotificationMode);
		}
		readonly ItemPropertyNotificationMode itemPropertyNotificationMode;
		bool ShouldSubscribePropertyChanged { get { return (itemPropertyNotificationMode & ItemPropertyNotificationMode.PropertyChanged) != ItemPropertyNotificationMode.None; } }
		protected virtual bool ShouldSubscribePropertiesChanged { get { return itemPropertyNotificationMode != ItemPropertyNotificationMode.None; } }
		protected readonly IList source;
		INotifyCollectionChanged NotifyCollectionChanged { get { return source as INotifyCollectionChanged; } }
		PropertyChangedWeakEventHandler<BindingListAdapterBase> propertyChangedHandler;
		PropertyChangedWeakEventHandler<BindingListAdapterBase> PropertyChangedHandler {
			get {
				if(propertyChangedHandler == null) {
					propertyChangedHandler = new PropertyChangedWeakEventHandler<BindingListAdapterBase>(this, (owner, o, e) => owner.OnObjectPropertyChanged(o, e));
				}
				return propertyChangedHandler;
			}
		}
		CollectionChangedWeakEventHandler<BindingListAdapterBase> collectionChangedHandler;
		CollectionChangedWeakEventHandler<BindingListAdapterBase> CollectionChangedHandler {
			get {
				if(collectionChangedHandler == null) {
					collectionChangedHandler = new CollectionChangedWeakEventHandler<BindingListAdapterBase>(this, (owner, o, e) => owner.OnSourceCollectionChanged(o, e));
				}
				return collectionChangedHandler;
			}
		}
		public bool RaisesItemChangedEvents { get; set; }
		public object OriginalDataSource { get; set; }
		public BindingListAdapterBase(IList source)
			: this(source, ItemPropertyNotificationMode.PropertyChanged) {
		}
		public BindingListAdapterBase(IList source, ItemPropertyNotificationMode itemPropertyNotificationMode)
			: this(source, itemPropertyNotificationMode, true) {
		}
		protected BindingListAdapterBase(IList source, ItemPropertyNotificationMode itemPropertyNotificationMode, bool doSubsribe) {
			this.source = source;
			this.itemPropertyNotificationMode = itemPropertyNotificationMode;
			if(doSubsribe)
				SubscribeAll(source);
			RaisesItemChangedEvents = true;
		}
		protected void SubscribeAll(IList source) {
			if(NotifyCollectionChanged != null)
				NotifyCollectionChanged.CollectionChanged += CollectionChangedHandler.Handler;
			SubscribeItemsPropertyChangedEvent(source.Count);
		}
		#region IBindingList Members
		public bool SupportsSearching { get { return false; } }
		public int Find(PropertyDescriptor property, object key) {
			throw new NotSupportedException();
		}
		public bool AllowNew {
			get {
				Type rowType = GetRowType();
				return rowType != null && rowType.GetConstructor(Type.EmptyTypes) != null;
			}
		}
		public object AddNew() {
			if(OriginalDataSource is IEditableCollectionView) {
				return ((IEditableCollectionView)OriginalDataSource).AddNew();
			} else {
				object obj = Activator.CreateInstance(GetRowType());
				Add(obj);
				AddNewInternal();				
				return obj;
			}
		}
		Type GetRowType() {
			bool ignore = false;
			Type rowType = ListDataControllerHelper.GetRowType(((IListWrapper)this).WrappedListType, out ignore);
			if(rowType == null && Count > 0 && IsItemLoaded(0))
				return this[0].GetType();
#if !SL
			else if (rowType == null && OriginalDataSource != null) {
				System.Windows.Data.CollectionView collectionView = OriginalDataSource as System.Windows.Data.CollectionView;
				if (collectionView.SourceCollection != null)
					rowType = ListDataControllerHelper.GetRowType(collectionView.SourceCollection.GetType(), out ignore);
			}
#endif
			return rowType;
		}
		protected virtual void AddNewInternal() { }
		public bool SupportsSorting { get { return false; } }
		public bool IsSorted { get { throw new NotSupportedException(); } }
		public void RemoveSort() { throw new NotSupportedException(); }
		public ListSortDirection SortDirection { get { throw new NotSupportedException(); } }
		public PropertyDescriptor SortProperty { get { throw new NotSupportedException(); } }
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			throw new NotSupportedException();
		}
		public void AddIndex(PropertyDescriptor property) { }
		public void RemoveIndex(PropertyDescriptor property) { }
		public bool AllowRemove { get { return true; } }
		public bool AllowEdit { get { return true; } }
		public bool SupportsChangeNotification { get { return true; } }
		public event ListChangedEventHandler ListChanged;
		#endregion
		#region IList Members
		public int Add(object value) {
			return source.Add(value);
		}
		public void Clear() {
			source.Clear();
		}
		public bool Contains(object value) {
			return source.Contains(value);
		}
#if DEBUGTEST
		public int IndexOfCallCount { get; private set; }
#endif
		public int IndexOf(object value) {
#if DEBUGTEST
			IndexOfCallCount++;
#endif
			return source.IndexOf(value);
		}
		public void Insert(int index, object value) {
			source.Insert(index, value);
		}
		public bool IsFixedSize { get { return source.IsFixedSize; } }
		public bool IsReadOnly { get { return source.IsReadOnly; } }
		public void Remove(object value) {
			if(OriginalDataSource is IEditableCollectionView)
				((IEditableCollectionView)OriginalDataSource).Remove(value);
			else
				source.Remove(value);
		}
		public void RemoveAt(int index) {
			if(OriginalDataSource is IEditableCollectionView)
				((IEditableCollectionView)OriginalDataSource).RemoveAt(index);
			else
				source.RemoveAt(index);
		}
		public object this[int index] { get { return source[index]; } set { source[index] = value; } }
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			source.CopyTo(array, index);
		}
		public int Count { get { return source.Count; } }
		public bool IsSynchronized { get { return source.IsSynchronized; } }
		public object SyncRoot { get { return source.SyncRoot; } }
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			return source.GetEnumerator();
		}
		#endregion
		void SubscribeItemsPropertyChangedEvent(int count) {
			SubscribeItemsPropertyChangedEvent(count, 0);
		}
		void SubscribeItemsPropertyChangedEvent(int count, int startIndex) {
			if(!ShouldSubscribePropertiesChanged)
				return;
			for(int i = startIndex; i < count + startIndex; i++) {
				if(IsItemLoaded(i)) {
					SubscribeItemPropertyChangedEvent(source[i]);
				}
			}
		}
		protected virtual bool IsItemLoaded(int index) {
			return true;
		}
		protected virtual void SubscribeItemPropertyChangedEvent(object item) {
			INotifyPropertyChanged notifyPropertyChanged = item as INotifyPropertyChanged;
			if(ShouldSubscribePropertyChanged && notifyPropertyChanged != null) {
				RemoveListener(notifyPropertyChanged);
				AddListener(notifyPropertyChanged);
			}			
		}
		void AddListener(INotifyPropertyChanged notifyPropertyChanged) {
			notifyPropertyChanged.PropertyChanged += PropertyChangedHandler.Handler;
		}
		void RemoveListener(INotifyPropertyChanged notifyPropertyChanged) {
			notifyPropertyChanged.PropertyChanged -= PropertyChangedHandler.Handler;
		}
		void UnsubscribeItemsPropertyChangedEvent(IList oldItems, bool needCheckItemLoading) {
			if(!ShouldSubscribePropertiesChanged)
				return;
			if(oldItems != null) {
				for(int i = 0; i < oldItems.Count; i++) {
					if(!needCheckItemLoading || IsItemLoaded(i)) {
						UnsubscribeItemPropertyChangedEvent(oldItems[i]);
					}
				}
			}
		}
		protected virtual void UnsubscribeItemPropertyChangedEvent(object item) {
			INotifyPropertyChanged notifyPropertyChanged = item as INotifyPropertyChanged;
			if(ShouldSubscribePropertyChanged && notifyPropertyChanged != null)
				RemoveListener(notifyPropertyChanged);
		}
		int lastChangedItemIndex = -1;
		PropertyDescriptorCollection itemProperties;
#if DEBUGTEST
		public int ObjectPropertyChangedFireCount { get; protected set; }
#endif
		protected virtual void OnObjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(RaisesItemChangedEvents)
				OnObjectPropertyChangedCore(sender, e);
		}
		protected virtual void OnObjectPropertyChangedCore(object sender, PropertyChangedEventArgs e) {
#if DEBUGTEST
			ObjectPropertyChangedFireCount++;
#endif
			RaiseChangedIfNeeded(sender, e.PropertyName, 
				(index, propertyDescriptor) => {
					NotifyChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index, propertyDescriptor));
			}, () => UnsubscribeItemPropertyChangedEvent(sender));
		}
		protected void RaiseChangedIfNeeded(object sender, string propertyName, Action<int, PropertyDescriptor> raiseEvent, Action unsubscribe) {
			int itemIndex;
			if(lastChangedItemIndex >= 0 && lastChangedItemIndex < Count && object.Equals(this[lastChangedItemIndex], sender)) {
				itemIndex = lastChangedItemIndex;
			} else {
				itemIndex = IndexOf(sender);
				lastChangedItemIndex = itemIndex;
			}
			if(itemIndex >= 0) {
				if(itemProperties == null)
					itemProperties = ListBindingHelper.GetListItemProperties(source);
				raiseEvent(itemIndex, !string.IsNullOrEmpty(propertyName) ? itemProperties[propertyName] : null);
			} else {
				unsubscribe();
			}
		}
		protected void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			int startingIndex;
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					startingIndex = e.NewStartingIndex;
					for(int i = 0; i < e.NewItems.Count; i++) {
						if(startingIndex < 0)
							startingIndex = source.IndexOf(e.NewItems[0]);
						if(startingIndex < 0)
							throw new InvalidOperationException("A collection Add event refers to item that does not belong to collection.");
						NotifyChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, startingIndex + i));
					}
					SubscribeItemsPropertyChangedEvent(e.NewItems.Count, startingIndex);
					break;
#if !SL
				case NotifyCollectionChangedAction.Move:
					if(e.NewItems.Count == 1) {
						if(e.OldStartingIndex < 0)
							throw new InvalidOperationException("Cannot find moved item index.");
						NotifyChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, e.NewStartingIndex, e.OldStartingIndex));
					} else {
						NotifyChanged(new ListChangedEventArgs(ListChangedType.Reset, e.NewStartingIndex));
					}
					break;
#endif
				case NotifyCollectionChangedAction.Remove:
					if(e.OldStartingIndex < 0)
						throw new InvalidOperationException("Cannot find removed item.");
					for(int i = 0; i < e.OldItems.Count; i++) {
						NotifyOnItemRemove(e.OldStartingIndex, e.NewStartingIndex, e.OldItems[i]);
					}
					UnsubscribeItemsPropertyChangedEvent(e.OldItems, false);
					break;
				case NotifyCollectionChangedAction.Replace:
					startingIndex = e.NewStartingIndex;
					for(int i = 0; i < e.NewItems.Count; i++) {
						if(startingIndex < 0)
							startingIndex = source.IndexOf(e.NewItems[0]);
						if(startingIndex < 0)
							throw new InvalidOperationException("A collection Replace event refers to item that does not belong to collection.");
						NotifyOnItemReplace(startingIndex + i, e.OldItems.Count > i ? e.OldItems[i] : null);
					}
					UnsubscribeItemsPropertyChangedEvent(e.OldItems, false);
					SubscribeItemsPropertyChangedEvent(e.NewItems.Count, startingIndex);
					break;
				case NotifyCollectionChangedAction.Reset:
					NotifyChanged(new ListChangedEventArgs(ListChangedType.Reset, e.NewStartingIndex));
					UnsubscribeItemsPropertyChangedEvent(source, true);
					SubscribeItemsPropertyChangedEvent(source.Count);
					break;
			}
		}
		protected virtual void NotifyOnItemRemove(int oldIndex, int newIndex, object item) {
			NotifyChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, oldIndex, newIndex));
		}
		protected virtual void NotifyOnItemReplace(int startingIndex, object oldItem) {
			NotifyChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, startingIndex));
		}
		protected void NotifyChanged(ListChangedEventArgs e) {
			if(ListChanged != null)
				ListChanged(this, e);
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
			UnsubscribeItemsPropertyChangedEvent(this.source, false);
		}
		#endregion
	}
	public class TypedListBindingListAdapterBase : BindingListAdapterBase, ITypedList {
		ITypedList TypedList { get { return (ITypedList)source; } }
		public TypedListBindingListAdapterBase(IList source):this(source, ItemPropertyNotificationMode.PropertyChanged){
		}
		public TypedListBindingListAdapterBase(IList source, ItemPropertyNotificationMode itemPropertyNotificationMode)
			: base(source, itemPropertyNotificationMode) {
			if(!(source is ITypedList))
				throw new ArgumentException("source");
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return TypedList.GetItemProperties(listAccessors);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return TypedList.GetListName(listAccessors);
		}
	}
	public class DictionaryListAdapterBase : BindingListAdapterBase, ITypedList {
		class DictionaryPropertyDescriptor : PropertyDescriptor {
			readonly Type propertyType;
			readonly string name;
			readonly object key;
			public DictionaryPropertyDescriptor(string name, Type propertyType, object key)
				: base("[" + name + "]", null) {
				this.name = name;
				this.propertyType = propertyType;
				this.key = key;
			}
			public override bool CanResetValue(object component) { return false; }
			public override Type ComponentType { get { return typeof(IDictionary); } }
			public override object GetValue(object component) {
				return ((IDictionary)component)[key];
			}
			public override bool IsReadOnly { get { return false; } }
			public override Type PropertyType { get { return propertyType; } }
			public override void ResetValue(object component) { throw new NotImplementedException(); }
			public override void SetValue(object component, object value) {
				((IDictionary)component)[key] = value;
			}
			public override bool ShouldSerializeValue(object obj) {
				return false;
			}
			public override string DisplayName { get { return name; } }
		}
		PropertyDescriptorCollection properties = new PropertyDescriptorCollection(null);
		public DictionaryListAdapterBase(IList source):this(source, null){
		}
		public DictionaryListAdapterBase(IList source, IDictionary<string, Type> types)
			: base(source) {
			if(types != null) {
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				foreach(KeyValuePair<string, Type> dictionaryEntry in types) {
					list.Add(new DictionaryPropertyDescriptor(dictionaryEntry.Key, dictionaryEntry.Value, dictionaryEntry.Key));
				}
				properties = new PropertyDescriptorCollection(list.ToArray(), true);
			}
		}
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(Count > 0 && properties.Count == 0) {
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				foreach(DictionaryEntry dictionaryEntry in this[0] as IDictionary) {
					Type type = dictionaryEntry.Value != null ? dictionaryEntry.Value.GetType() : typeof(object);
					if(type.IsValueType)
						type = typeof(Nullable<>).MakeGenericType(type);
					list.Add(new DictionaryPropertyDescriptor(Convert.ToString(dictionaryEntry.Key), type, dictionaryEntry.Key));
				}
				properties = new PropertyDescriptorCollection(list.ToArray(), true);
			}
			return properties;
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return null;
		}
		#endregion
	}
}
