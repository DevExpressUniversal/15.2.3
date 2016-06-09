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
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using DevExpress.Data.Utils;
using System.Runtime.CompilerServices;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	public interface IUserInterfaceObject {
		object Id { get; }
		string DisplayName { get; set; }
		string MenuCaption { get; set; }
	}
	public interface IViewAsyncAccessor {
		IViewAsyncSupport View { get; }
	}
}
namespace DevExpress.XtraScheduler {
	#region UserInterfaceObject
#if !SL
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverter))]
#endif
	public abstract class UserInterfaceObject : IUserInterfaceObject, IXtraSupportShouldSerialize, ISupportObjectChanged, INotifyPropertyChanged, IIdProvider {
		object id;
		string displayName;
		string menuCaption;
		string defaultDisplayName;
		string defaultMenuCaption;
		XtraSupportShouldSerializeHelper shouldSerializeHelper = new XtraSupportShouldSerializeHelper();
		EventHandler onChanged;
		PropertyChangedEventHandler propertyChanged;
		protected UserInterfaceObject(object id, string displayName)
			: this(id, displayName, displayName) {
		}
		protected UserInterfaceObject(object id, string displayName, string menuCaption) {
			this.id = id ?? CreateDefaultId();
			this.DisplayName = displayName;
			this.MenuCaption = menuCaption;
			this.defaultDisplayName = this.DisplayName;
			this.defaultMenuCaption = this.MenuCaption;
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("DisplayName", XtraShouldSerializeDisplayName);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("MenuCaption", XtraShouldSerializeMenuCaption);
		}
		protected XtraSupportShouldSerializeHelper ShouldSerializeHelper {
			get { return shouldSerializeHelper; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual string DefaultDisplayName {
			get { return defaultDisplayName; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual string DefaultMenuCaption {
			get { return defaultMenuCaption; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Id {
			get { return id; }
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("UserInterfaceObjectDisplayName"),
#endif
 RefreshProperties(RefreshProperties.All), NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true), AutoFormatDisable()]
		public virtual string DisplayName {
			get { return displayName; }
			set {
				if (value == null)
					value = String.Empty;
				if (displayName != value) {
					string oldValue = displayName;
					displayName = value;
					OnChanged("DisplayName", oldValue, value);
				}
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("UserInterfaceObjectMenuCaption"),
#endif
 NotifyParentProperty(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Localizable(true), AutoFormatDisable()]
		public virtual string MenuCaption {
			get { return menuCaption; }
			set {
				if (value == null)
					value = String.Empty;
				if (menuCaption != value) {
					string oldValue = menuCaption;
					menuCaption = value;
					OnChanged("MenuCaption", oldValue, value);
				}
			}
		}
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return shouldSerializeHelper.ShouldSerialize(propertyName);
		}
		protected virtual object CreateDefaultId() {
			return null;
		}
		protected internal virtual void ResetDisplayName() {
			DisplayName = DefaultDisplayName;
		}
		protected internal virtual bool ShouldSerializeDisplayName() {
			return true;
		}
		protected internal virtual bool XtraShouldSerializeDisplayName() {
			return ShouldSerializeDisplayName();
		}
		protected internal virtual bool ShouldSerializeMenuCaption() {
			return MenuCaption != DisplayName;
		}
		protected internal virtual bool XtraShouldSerializeMenuCaption() {
			return ShouldSerializeMenuCaption();
		}
		protected internal virtual void ResetMenuCaption() {
			MenuCaption = DefaultMenuCaption;
		}
		protected internal virtual void Assign(UserInterfaceObject source) {
			if (source == null)
				return;
			DisplayName = source.DisplayName;
			MenuCaption = source.MenuCaption;
		}
		protected internal virtual void OnChanged(string propertyName, object oldValue, object newValue) {
			RaiseChanged();
			RaisePropertyChanged(propertyName);
		}
		protected internal virtual void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public override bool Equals(object obj) {
			UserInterfaceObject info = obj as UserInterfaceObject;
			if (info == null)
				return false;
			return Object.Equals(Id, info.Id) && DisplayName == info.DisplayName && MenuCaption == info.MenuCaption;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return DisplayName.Length == 0 ? base.ToString() : DisplayName;
		}
		event EventHandler ISupportObjectChanged.Changed {
			add { onChanged += value; }
			remove { onChanged -= value; }
		}
		public event PropertyChangedEventHandler PropertyChanged {
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}
		#region IIdProvider Members
		void IIdProvider.SetId(object id) {
			this.id = id;
		}
		#endregion
	}
	#endregion
	#region UserInterfaceObjectCollection
#if !SL
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
#endif
	public abstract class UserInterfaceObjectCollection<T> : SchedulerCollectionBase<T>, IStorageCollection<T> where T : IUserInterfaceObject {
		Dictionary<object, T> cache = new Dictionary<object, T>();
		PropertyChangedWeakEventHandler<UserInterfaceObjectCollection<T>> propertyChangedHandler;
		protected UserInterfaceObjectCollection() {
		}
		protected UserInterfaceObjectCollection(DXCollectionUniquenessProviderType uniquenessProviderType)
			: base(uniquenessProviderType) {
		}
		protected override bool IsIndexCacheEnabled {
			get { return false; }
		}
		PropertyChangedWeakEventHandler<UserInterfaceObjectCollection<T>> PropertyChangedHandler {
			get {
				if (propertyChangedHandler == null) {
					propertyChangedHandler = new PropertyChangedWeakEventHandler<UserInterfaceObjectCollection<T>>(this, (owner, o, e) => owner.OnObjectPropertyChanged(o, e));
				}
				return propertyChangedHandler;
			}
		}
		protected internal abstract object ProvideDefaultId();
		protected internal abstract T CreateItemInstance(object id, string displayName, string menuCaption);
		protected internal abstract UserInterfaceObjectCollection<T> CreateDefaultContent();
		protected internal abstract T CloneItem(T item);
		protected override T GetItem(int index) {
			return (index >= 0 && index < Count) ? InnerList[index] : CreateItemInstance(null, String.Empty, String.Empty);
		}
		protected T CreateItem(object id, string displayName, string menuCaption) {
			T item = CreateItemInstance(id != null ? id : ProvideDefaultId(), displayName, menuCaption);
			return item;
		}
		protected internal virtual void Assign(UserInterfaceObjectCollection<T> source) {
			if (source == null)
				return;
			BeginUpdate();
			try {
				Clear();
				for (int i = 0; i < source.InnerList.Count; i++)
					Add(CloneItem(source.InnerList[i]));
			} finally {
				EndUpdate();
			}
		}
		public bool HasDefaultContent() {
			UserInterfaceObjectCollection<T> defaultContent = CreateDefaultContent();
			try {
				if (defaultContent == null)
					return false;
				if (Count != defaultContent.Count)
					return false;
				int count = Count;
				for (int i = 0; i < count; i++) {
					if (!GetItem(i).Equals(defaultContent.GetItem(i)))
						return false;
				}
				return true;
			} finally {
				IDisposable disposableContent = defaultContent as IDisposable;
				if (disposableContent != null)
					disposableContent.Dispose();
			}
		}
		public int Add(string displayName) {
			return Add(displayName, displayName);
		}
		public int Add(string displayName, string menuCaption) {
			T item = CreateItemInstance(null, displayName, menuCaption);
			return Add(item);
		}
		public override int Add(T value) {
			if (value.Id == null) {
				IIdProvider idProvider = value as IIdProvider;
				if (idProvider != null)
					idProvider.SetId(ProvideDefaultId());
			}
			return base.Add(value);
		}
		public virtual void LoadDefaults() {
			BeginUpdate();
			try {
				Clear();
				UserInterfaceObjectCollection<T> defaultContent = CreateDefaultContent();
				AddRange(defaultContent);
			} finally {
				EndUpdate();
			}
		}
		public T GetById(object id) {
			if (id == null)
				return CreateItemInstance(null, string.Empty, string.Empty);
			T result;
			lock (this.cache) {
				if (this.cache.TryGetValue(id, out result))
					return result;
			}
			result = this.FirstOrDefault(o => SchedulerUtils.SpecialEquals(o.Id, id));
			if (result == null)
				result = CreateItemInstance(null, string.Empty, string.Empty);
			else
				AddToCache(id, result);
			return result;
		}
		public T GetByIndex(int index) {
			return GetItem(index);
		}
		protected override void OnClearComplete() {
			this.cache.Clear();
			base.OnClearComplete();
		}
		protected override void OnRemoveComplete(int index, T value) {
			DeleteFromCache(value.Id);
			base.OnRemoveComplete(index, value);
		}
		private void AddToCache(object id, T value) {
			lock (this.cache) {
				if (!this.cache.ContainsKey(id))
					this.cache.Add(id, value);
			}
		}
		private void DeleteFromCache(object id) {
			lock (this.cache) {
				if (id != null && this.cache.ContainsKey(id))
					this.cache.Remove(id);
			}
		}
		protected override void OnInsertComplete(int index, T value) {
			base.OnInsertComplete(index, value);
			SubscribeItemPropertyChangedEvent(value);
		}
		private void OnObjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "Id")
				this.cache.Clear();
		}
		void AddListener(INotifyPropertyChanged notifyPropertyChanged) {
			notifyPropertyChanged.PropertyChanged += PropertyChangedHandler.Handler;
		}
		void RemoveListener(INotifyPropertyChanged notifyPropertyChanged) {
			notifyPropertyChanged.PropertyChanged -= PropertyChangedHandler.Handler;
		}
		private void SubscribeItemPropertyChangedEvent(object item) {
			INotifyPropertyChanged notifyPropertyChanged = item as INotifyPropertyChanged;
			if (notifyPropertyChanged != null) {
				RemoveListener(notifyPropertyChanged);
				AddListener(notifyPropertyChanged);
			}
		}
	}
	#endregion
}
