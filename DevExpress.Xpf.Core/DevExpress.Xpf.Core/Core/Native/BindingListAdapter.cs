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
using DevExpress.Xpf.Core;
using DevExpress.Data.Helpers;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using System.Windows.Threading;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using System.Windows.Forms;
using DevExpress.Xpf.ChunkList;
namespace DevExpress.Xpf.Core.Native {
	public interface ILoaded {
		bool IsItemLoaded(int index);
	}
	public class AllowSubscribeDependecyPropertyNotificationEventArgs : EventArgs {
		public AllowSubscribeDependecyPropertyNotificationEventArgs(object item, DependencyPropertyDescriptor property, bool allow) {
			this.Item = item;
			this.Property = property;
			this.Allow = allow;
		}
		public object Item { get; private set; } 
		public DependencyPropertyDescriptor Property { get; private set; }
		public bool Allow { get; set; }
	}
	public delegate void AllowSubscribeDependecyPropertyNotificationEventHandler(object sender, AllowSubscribeDependecyPropertyNotificationEventArgs e);
	[Flags]
	public enum ItemPropertyNotificationMode { 
		None = 0,
		PropertyChanged = 1,
		DependencyProperties = 2,
		PropertyChanging = 4,
		All = PropertyChanged | DependencyProperties | PropertyChanging,
	}
	public class NotificationsBufferizationSettings {
		readonly TimeSpan interval;
		public TimeSpan Interval { get { return interval; } }
		public NotificationsBufferizationSettings(TimeSpan interval) {
			this.interval = interval;
		}
	}
	public class PropertyChangingWeakEventHandler<TOwner> : WeakEventHandler<TOwner, PropertyChangingEventArgs, PropertyChangingEventHandler> where TOwner : class {
		static Action<WeakEventHandler<TOwner, PropertyChangingEventArgs, PropertyChangingEventHandler>, object> action = (h, o) => ((INotifyPropertyChanging)o).PropertyChanging -= h.Handler;
		static Func<WeakEventHandler<TOwner, PropertyChangingEventArgs, PropertyChangingEventHandler>, PropertyChangingEventHandler> create = h => new PropertyChangingEventHandler(h.OnEvent);
		public PropertyChangingWeakEventHandler(TOwner owner, Action<TOwner, object, PropertyChangingEventArgs> onEventAction)
			: base(owner, onEventAction, action, create) {
		}
	}
	public class BindingListAdapter : BindingListAdapterBase, ICancelAddNew, IListWrapper, IRefreshable, IListChanging {
		static readonly PropertyChangedEventArgs EmptyEventArgs = new PropertyChangedEventArgs(null);
		PropertyChangingWeakEventHandler<BindingListAdapter> propertyChangingHandler;
		PropertyChangingWeakEventHandler<BindingListAdapter> PropertyChangingHandler {
			get {
				if(propertyChangingHandler == null) {
					propertyChangingHandler = new PropertyChangingWeakEventHandler<BindingListAdapter>(this, (owner, o, e) => owner.OnObjectPropertyChanging(o, e));
				}
				return propertyChangingHandler;
			}
		}
		protected virtual void OnObjectPropertyChanging(object sender, PropertyChangingEventArgs e) {
			RaiseChangedIfNeeded(sender, e.PropertyName,
				(index, propertyDescriptor) => {
					RaiseListChanging(new ListChangingEventArgs(index, propertyDescriptor));
				}, () => UnsubscribeItemPropertyChangedEvent(sender));
		}
		public static BindingListAdapter CreateFromList(IList list, ItemPropertyNotificationMode itemPropertyNotificationMode = ItemPropertyNotificationMode.PropertyChanged) {
			return list is ITypedList ? new TypedListBindingListAdapter(list, itemPropertyNotificationMode) : new BindingListAdapter(list, itemPropertyNotificationMode);
		}
		bool isNewItemRowEditing;
		public event AllowSubscribeDependecyPropertyNotificationEventHandler AllowSubscribeDependecyPropertyNotification;
		readonly ItemPropertyNotificationMode itemPropertyNotificationMode;
		bool ShouldSubscribePropertyChanged { get { return (itemPropertyNotificationMode & ItemPropertyNotificationMode.PropertyChanged) != ItemPropertyNotificationMode.None; } }
		bool ShouldSubscribePropertyChanging { get { return (itemPropertyNotificationMode & ItemPropertyNotificationMode.PropertyChanging) != ItemPropertyNotificationMode.None; } }
		protected override bool ShouldSubscribePropertiesChanged { get { return itemPropertyNotificationMode != ItemPropertyNotificationMode.None; } }
		bool ShouldSubscribeDependencyProperties { get { return (itemPropertyNotificationMode & ItemPropertyNotificationMode.DependencyProperties) != ItemPropertyNotificationMode.None; } }
		internal static DevExpress.Data.Helpers.ItemPropertyNotificationMode ConvertMode(ItemPropertyNotificationMode mode) {
			switch(mode){
				case ItemPropertyNotificationMode.PropertyChanged:
				case ItemPropertyNotificationMode.All:
					return Data.Helpers.ItemPropertyNotificationMode.PropertyChanged;
				default:
					return Data.Helpers.ItemPropertyNotificationMode.None;
			}
		}
		internal BindingListAdapter(IList source, ItemPropertyNotificationMode itemPropertyNotificationMode = ItemPropertyNotificationMode.PropertyChanged)
			: base(source, ConvertMode(itemPropertyNotificationMode), false) {
			this.itemPropertyNotificationMode = itemPropertyNotificationMode;
			SubscribeAll(source);
		}
		protected override void NotifyOnItemRemove(int oldIndex, int newIndex, object item) {
			NotifyChanged(new ChunkListChangedEventArgs(ListChangedType.ItemDeleted, oldIndex, item));
		}
		protected override void NotifyOnItemReplace(int startingIndex, object oldItem) {
			NotifyChanged(new ChunkListChangedEventArgs(ListChangedType.ItemChanged, startingIndex, oldItem));
		}
		protected override void SubscribeItemPropertyChangedEvent(object item) {
			base.SubscribeItemPropertyChangedEvent(item);
			INotifyPropertyChanging propertyChanging = item as INotifyPropertyChanging;
			if(ShouldSubscribePropertyChanging && propertyChanging != null)
				propertyChanging.PropertyChanging += PropertyChangingHandler.Handler;
			EnumerateDependencyPropertyDescriptors(item, dProperty => {
				dProperty.RemoveValueChanged(item, OnObjectDependencyPropertyChanged);
				dProperty.AddValueChanged(item, OnObjectDependencyPropertyChanged);
			});
		}
		protected override void UnsubscribeItemPropertyChangedEvent(object item) {
			base.UnsubscribeItemPropertyChangedEvent(item);
			INotifyPropertyChanging propertyChanging = item as INotifyPropertyChanging;
			if(ShouldSubscribePropertyChanged && propertyChanging != null)
				propertyChanging.PropertyChanging -= PropertyChangingHandler.Handler;
			EnumerateDependencyPropertyDescriptors(item, dProperty => dProperty.RemoveValueChanged(item, OnObjectDependencyPropertyChanged));
		}
		void EnumerateDependencyPropertyDescriptors(object item, Action<DependencyPropertyDescriptor> descriptorAction) {
			if(ShouldSubscribeDependencyProperties && item is DependencyObject) {
				foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(item)) {
					DependencyPropertyDescriptor dProperty = DependencyPropertyDescriptor.FromProperty(property);
					if(dProperty != null) {
						bool allow = !dProperty.IsAttached;
						if(AllowSubscribeDependecyPropertyNotification != null) {
							AllowSubscribeDependecyPropertyNotificationEventArgs eventArgs = new AllowSubscribeDependecyPropertyNotificationEventArgs(item, dProperty, allow);
							AllowSubscribeDependecyPropertyNotification(this, eventArgs);
							allow = eventArgs.Allow;
						}
						if(allow)
							descriptorAction(dProperty);
					}
				}
			}
		}
		void OnObjectDependencyPropertyChanged(object sender, EventArgs e) {
			OnObjectPropertyChanged(sender, EmptyEventArgs);
		}
		public event ListChangingEventHandler ListChanging;
		void RaiseListChanging(ListChangingEventArgs args) {
			if(ListChanging != null)
				ListChanging(this, args);
		}
		protected override void AddNewInternal() {
			isNewItemRowEditing = true;
		}
		#region ICancelAddNew Members
		public void CancelNew(int itemIndex) {
			if (OriginalDataSource is IEditableCollectionView) {
				((IEditableCollectionView)OriginalDataSource).CancelNew();
			} else {
				if (isNewItemRowEditing) {
					RemoveAt(itemIndex);
					isNewItemRowEditing = false;
				}
			}
		}
		public void EndNew(int itemIndex) {
			if (OriginalDataSource is IEditableCollectionView) {
				((IEditableCollectionView)OriginalDataSource).CommitNew();
			} else {
				isNewItemRowEditing = false;
			}
		}
		#endregion
		#region IListIndexerPropertyTypeSupport Members
		Type IListWrapper.WrappedListType { get { return source.GetType(); } }
		#endregion
		protected override bool IsItemLoaded(int index) {
			if (source is ILoaded) {
				return (source as ILoaded).IsItemLoaded(index);
			}
			return base.IsItemLoaded(index);
		}
		void IRefreshable.Refresh() {
			(source as IRefreshable).Do(x => x.Refresh());
		}
	}
	public class TypedListBindingListAdapter : BindingListAdapter, ITypedList {
		ITypedList TypedList { get { return (ITypedList)source; } }
		public TypedListBindingListAdapter(IList source, ItemPropertyNotificationMode itemPropertyNotificationMode = ItemPropertyNotificationMode.PropertyChanged)
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
	public class DictionaryListAdapter : BindingListAdapter, ITypedList {
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
		public DictionaryListAdapter(IList source, IDictionary<string, Type> types = null)
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
