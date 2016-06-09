#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Actions {
	public enum EmptyItemsBehavior { None, Disable, Deactivate }
	public enum ChoiceActionItemFindType { Recursive, NonRecursive }
	public enum ChoiceActionItemFindTarget { Any, Leaf, Group }
	public enum DefaultItemMode { FirstActiveItem, LastExecutedItem }
	public enum ImageMode { UseActionImage, UseItemImage }
	public enum ChoiceActionBehaviorChangedType {
		ImageMode,
		DefaultItemMode,
		ShowItemsOnClick
	}
	internal interface IComplexChoiceAction {
		void ItemChangedCore(ChoiceActionItem item, ChoiceActionItemChangesType choiceActionItemType);
		void ItemsChangedCore(ChoiceActionItemChangesType choiceActionItemType);
	}
	public class ChoiceActionItemCollection : IList<ChoiceActionItem>, IList {
		private List<ChoiceActionItem> items = new List<ChoiceActionItem>();
		private IComplexChoiceAction owner;
		private bool raiseEvent = true;
		internal ChoiceActionItemCollection(IComplexChoiceAction owner) {
			this.owner = owner;
		}
		private void ItemChangedCore(ChoiceActionItem item, ChoiceActionItemChangesType choiceActionItemType) {
			if(owner != null && raiseEvent) {
				owner.ItemChangedCore(item, choiceActionItemType);
			}
		}
		private void ItemsChangedCore(ChoiceActionItemChangesType choiceActionItemType) {
			if(owner != null && raiseEvent) {
				owner.ItemsChangedCore(choiceActionItemType);
			}
		}
		public ChoiceActionItemCollection() : this(null) { }
		public void AddRange(IList<ChoiceActionItem> list) {
			try {
				raiseEvent = false;
				foreach(ChoiceActionItem item in list) {
					Add(item);
				}
			}
			finally {
				raiseEvent = true;
				ItemsChangedCore(ChoiceActionItemChangesType.ItemsAdd);
			}
		}
		public ChoiceActionItem Find(object data) {
			for(int i = 0; i < Count; i++) {
				if(ReflectionHelper.AreEqual(items[i].Data, data)) {
					return items[i];
				}
			}
			return null;
		}
		public ChoiceActionItem FindItemByID(string itemId) {
			return Find(itemId, ChoiceActionItemFindType.NonRecursive, ChoiceActionItemFindTarget.Any);
		}
		public ChoiceActionItem Find(string itemId, ChoiceActionItemFindType recursive, ChoiceActionItemFindTarget target) {
			if(string.IsNullOrEmpty(itemId)) {
				return null;
			}
			foreach(ChoiceActionItem item in this) {
				if(item.Id == itemId &&
					((target == ChoiceActionItemFindTarget.Any) ||
					(target == ChoiceActionItemFindTarget.Group && item.Items.Count > 0) ||
					(target == ChoiceActionItemFindTarget.Leaf && item.Items.Count == 0))) {
					return item;
				}
				if(recursive == ChoiceActionItemFindType.Recursive) {
					ChoiceActionItem result = item.Items.Find(itemId, recursive, target);
					if(result != null) {
						return result;
					}
				}
			}
			return null;
		}
		public void Add(ChoiceActionItem item) {
			Insert(Count, item);
		}
		public void Clear() {
			items.Clear();
			ItemsChangedCore(ChoiceActionItemChangesType.ItemsRemove);
		}
		public bool Contains(ChoiceActionItem item) {
			return items.Contains(item);
		}
		public void CopyTo(ChoiceActionItem[] array, int arrayIndex) {
			items.CopyTo(array, arrayIndex);
		}
		public bool Remove(ChoiceActionItem item) {
			bool result = items.Remove(item);
			ItemChangedCore(item, ChoiceActionItemChangesType.Remove);
			return result;
		}
		public IEnumerator<ChoiceActionItem> GetEnumerator() {
			return items.GetEnumerator();
		}
		public int IndexOf(ChoiceActionItem item) {
			return items.IndexOf(item);
		}
		public void Insert(int index, ChoiceActionItem item) {
			if(!items.Contains(item)) {
				item.Owner = owner;
				items.Insert(index, item);
				ItemChangedCore(item, ChoiceActionItemChangesType.Add);
			}
		}
		public void RemoveAt(int index) {
			Remove(items[index]);
		}
		public ChoiceActionItem this[int index] {
			get { return items[index]; }
			set { Insert(index, value); }
		}
		public int Count {
			get { return items.Count; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ChoiceActionItemCollectionFirstActiveItem")]
#endif
		public ChoiceActionItem FirstActiveItem {
			get {
				ChoiceActionItem fistActiveItem = null;
				foreach(ChoiceActionItem choiceActionItem in this) {
					if(choiceActionItem.Active && choiceActionItem.Enabled) {
						if(choiceActionItem.Items.Count == 0) {
							fistActiveItem = choiceActionItem;
						}
						else {
							fistActiveItem = choiceActionItem.Items.FirstActiveItem;
						}
					}
					if(fistActiveItem != null) {
						return fistActiveItem;
					}
				}
				return null;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#region IList Members
		public int Add(object value) {
			ChoiceActionItem item = value as ChoiceActionItem;
			Add(item);
			return items.IndexOf(item);
		}
		public bool Contains(object value) {
			return Contains(value as ChoiceActionItem);
		}
		public int IndexOf(object value) {
			return IndexOf(value as ChoiceActionItem);
		}
		public void Insert(int index, object value) {
			Insert(index, value as ChoiceActionItem);
		}
		public bool IsFixedSize {
			get { return false; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public void Remove(object value) {
			Remove(value as ChoiceActionItem);
		}
		object IList.this[int index] {
			get { return items[index]; }
			set { Insert(index, value); }
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			((IList)items).CopyTo(array, index);
		}
		public bool IsSynchronized {
			get { return ((ICollection)items).IsSynchronized; }
		}
		public object SyncRoot {
			get { return ((ICollection)items).SyncRoot; }
		}
		#endregion
	}
	public abstract class ChoiceActionBase : ActionBase, IComplexChoiceAction {
		public const string DefaultItemCaption = "Entry";
		private ChoiceActionItemCollection items;
		private Dictionary<Object, ChoiceActionItemChangesType> itemsChangedInfo = new Dictionary<Object, ChoiceActionItemChangesType>();
		private EmptyItemsBehavior emptyItemsBehavior;
		private void UpdateStateByItemsCount() {
			Active.BeginUpdate();
			Enabled.BeginUpdate();
			try {
				Active["EmptyItems"] = true;
				Enabled["EmptyItems"] = true;
				Enabled["AllItemsDisabled"] = true;
				bool hasVisible = false;
				foreach(ChoiceActionItem choiceActionItem in Items) {
					if(choiceActionItem.Active) {
						hasVisible = true;
						break;
					}
				}
				if(emptyItemsBehavior == EmptyItemsBehavior.Deactivate) {
					Active["EmptyItems"] = hasVisible;
				}
				if(emptyItemsBehavior == EmptyItemsBehavior.Disable) {
					Enabled["EmptyItems"] = hasVisible;
				}
				bool itemsPresented = Items.Count != 0;
				if(itemsPresented) {
					Enabled["AllItemsDisabled"] = Items.FirstActiveItem != null;
				}
			}
			finally {
				Active.EndUpdate();
				Enabled.EndUpdate();
			}
		}
		private void SetDefaultItemCaption(ChoiceActionItem item) {
			if(DesignMode) {
				if(string.IsNullOrEmpty(item.Caption)) {
					item.Caption = string.Format("{0} {1}", DefaultItemCaption, items.IndexOf(item) + 1);
				}
			}
		}
		protected ChoiceActionBase(IContainer container)
			: base(container) {
			items = new ChoiceActionItemCollection(this);
			EmptyItemsBehavior = EmptyItemsBehavior.Deactivate;
		}
		protected ChoiceActionBase(Controller owner, string id, string category)
			: base(owner, id, category) {
			items = new ChoiceActionItemCollection(this);
			EmptyItemsBehavior = EmptyItemsBehavior.Deactivate;
		}
		protected virtual void OnItemsChanged(ChoiceActionItem item, ChoiceActionItemChangesType changedType) {
			Object key = item != null ? item : (Object)this;
			if(LockCount > 0) {
				ChoiceActionItemChangesType value;
				if(itemsChangedInfo.TryGetValue(key, out value)) {
					itemsChangedInfo.Remove(key);
					itemsChangedInfo.Add(key, value | changedType);
				}
				else {
					itemsChangedInfo.Add(key, changedType);
				}
			}
			else {
				if(ItemsChanged != null) {
					ItemsChanged(this, new ItemsChangedEventArgs(key, changedType));
				}
			}
		}
		protected internal override void NotifyAboutCurrentAspectChanged() {
			base.NotifyAboutCurrentAspectChanged();
			BeginUpdate();
			foreach(ChoiceActionItem item in items) {
				item.NotifyAboutCurrentAspectChanged();
			}
			EndUpdate();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				BeginUpdate();
				items.Clear();
				EndUpdate();
			}
			ItemsChanged = null;
			base.Dispose(disposing);
		}
		protected override void ReleaseLockedEvents() {
			base.ReleaseLockedEvents();
			if(ItemsChanged != null && itemsChangedInfo.Count > 0) {
				ItemsChanged(this, new ItemsChangedEventArgs(itemsChangedInfo));
			}
			itemsChangedInfo.Clear();
		}
		public override void AssignInfo(IModelAction newInfo) {
			base.AssignInfo(newInfo);
			if(newInfo != null && newInfo.ChoiceActionItems != null) {
				foreach(ChoiceActionItem item in Items) {
					IModelChoiceActionItem itemInfo = newInfo.ChoiceActionItems[item.Id];
					if(itemInfo != null) {
						item.AssignInfo(itemInfo);
					}
				}
			}
		}
		protected virtual void OnBehaviorChanged(ChoiceActionBehaviorChangedEventArgs args) {
			if(BehaviorChanged != null) {
				BehaviorChanged(this, args);
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionBaseItems"),
#endif
 Category("Items"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ChoiceActionItemCollection Items {
			get { return items; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionBaseEmptyItemsBehavior"),
#endif
 Category("Items"), DefaultValue(EmptyItemsBehavior.Deactivate)]
		public EmptyItemsBehavior EmptyItemsBehavior {
			get { return emptyItemsBehavior; }
			set {
				emptyItemsBehavior = value;
				UpdateStateByItemsCount();
				OnItemsChanged(null, ChoiceActionItemChangesType.Items);
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionBaseImageMode"),
#endif
 Category("Behavior"), DefaultValue(ImageMode.UseActionImage)]
		public ImageMode ImageMode {
			get { return Model.ImageMode; }
			set {
				if(Model.ImageMode != value) {
					Model.ImageMode = value;
					OnBehaviorChanged(new ChoiceActionBehaviorChangedEventArgs(ChoiceActionBehaviorChangedType.ImageMode));
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionBaseDefaultItemMode"),
#endif
 Category("Behavior"), DefaultValue(DefaultItemMode.FirstActiveItem)]
		public DefaultItemMode DefaultItemMode {
			get { return Model.DefaultItemMode; }
			set {
				if(Model.DefaultItemMode != value) {
					Model.DefaultItemMode = value;
					OnBehaviorChanged(new ChoiceActionBehaviorChangedEventArgs(ChoiceActionBehaviorChangedType.DefaultItemMode));
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ChoiceActionBaseShowItemsOnClick"),
#endif
 Category("Behavior"), DefaultValue(false)]
		public bool ShowItemsOnClick {
			get { return Model.ShowItemsOnClick; }
			set {
				if(Model.ShowItemsOnClick != value) {
					Model.ShowItemsOnClick = value;
					OnBehaviorChanged(new ChoiceActionBehaviorChangedEventArgs(ChoiceActionBehaviorChangedType.ShowItemsOnClick));
				}
			}
		}
		[Browsable(false)]
		public event EventHandler<ChoiceActionBehaviorChangedEventArgs> BehaviorChanged;
		[Browsable(false)]
		public event EventHandler<ItemsChangedEventArgs> ItemsChanged;
		public bool IsHierarchical() {
			foreach (ChoiceActionItem item in Items) {
				if (item.Items.Count > 0) return true;
			}
			return false;
		}
		void IComplexChoiceAction.ItemChangedCore(ChoiceActionItem item, ChoiceActionItemChangesType changedType) {
			if(item != null && changedType == ChoiceActionItemChangesType.ItemsAdd || changedType == ChoiceActionItemChangesType.Add) {
				SetDefaultItemCaption(item);
			}
			UpdateStateByItemsCount();
			OnItemsChanged(item, changedType);
		}
		void IComplexChoiceAction.ItemsChangedCore(ChoiceActionItemChangesType changedType) {
			if(Items.Count > 0) {
				foreach(ChoiceActionItem item in items) {
					SetDefaultItemCaption(item);
				}
			}
			((IComplexChoiceAction)this).ItemChangedCore(null, changedType);
		}
	}
	public class ItemsChangedEventArgs : EventArgs {
		private Dictionary<Object, ChoiceActionItemChangesType> itemsChangedInfo = new Dictionary<Object, ChoiceActionItemChangesType>();
		private void AddItemsChangedInfo(Object key, ChoiceActionItemChangesType changedType) {
			if(key != null) {
				ChoiceActionItemChangesType value;
				if(itemsChangedInfo.TryGetValue(key, out value)) {
					itemsChangedInfo.Remove(key);
					itemsChangedInfo.Add(key, value | changedType);
				}
				else {
					itemsChangedInfo.Add(key, changedType);
				}
			}
		}
		public ItemsChangedEventArgs(Object key, ChoiceActionItemChangesType changedType) {
			AddItemsChangedInfo(key, changedType);
		}
		public ItemsChangedEventArgs(Dictionary<Object, ChoiceActionItemChangesType> itemsChangedInfo) {
			this.itemsChangedInfo = itemsChangedInfo;
		}
		public Dictionary<Object, ChoiceActionItemChangesType> ChangedItemsInfo {
			get { return itemsChangedInfo; }
		}
	}
	public class ChoiceActionBehaviorChangedEventArgs : EventArgs {
		private ChoiceActionBehaviorChangedType changedPropertyType;
		public ChoiceActionBehaviorChangedEventArgs(ChoiceActionBehaviorChangedType changedPropertyType) {
			this.changedPropertyType = changedPropertyType;
		}
		public ChoiceActionBehaviorChangedType ChangedPropertyType {
			get { return changedPropertyType; }
		}
	}
}
