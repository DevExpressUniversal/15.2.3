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
using DevExpress.XtraBars.InternalItems;
namespace DevExpress.XtraBars {
	[ListBindable(false)]
	public class BarItems : CollectionBase {
		BarManager manager;
		Hashtable nameHash;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event CollectionChangeEventHandler CollectionChanged;
		internal BarItems(BarManager manager) {
			this.manager = manager;
			this.nameHash = new Hashtable();
		}
		internal void RibbonClearItems() {
			InnerList.Clear();
			NameHash.Clear();
		}
		protected BarManager Manager { get { return manager; } }
		protected virtual Hashtable NameHash { get { return nameHash; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemsItem")]
#endif
		public BarItem this[int index] {
			get {
				return (BarItem)List[index];
			}
		}
		public BarItem FindById(int itemId) {
			if(itemId == -1) return null;
			foreach(BarItem item in this) {
				if(item.Id == itemId) return item;
			}
			return null;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemsItem")]
#endif
		public BarItem this[string name] {
			get {
				if(NameHash.ContainsKey(name)) return NameHash[name] as BarItem;
				return null;
			}
		}
		public virtual void AddRange(BarItem[] items) {
			foreach(BarItem item in items) {
				Add(item);
			}
		}
		public virtual int Add(BarItem item) {
			if(item.IsPrivateItem) {
				item.Manager = Manager;
				return -1;
			}
			int index = IndexOf(item);
			if(item.Manager == Manager && index != -1) return index;
			return List.Add(item);
		}
		protected override void OnClear() {
			NameHash.Clear();
			for(int n = Count - 1; n >= 0; n--) {
				if(ShouldSkipItem(this[n])) continue;
				RemoveAt(n);
			}
			RaiseChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected virtual bool ShouldSkipItem(BarItem item) {
			if(item is RibbonExpandCollapseItem) return true;
			if(item is AutoHiddenPagesMenuItem) return true;
			return false;
		}
		protected virtual void RaiseChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		protected internal virtual void OnItemChanged(BarItem item) {
			if(item == null || CollectionChanged == null || item.IsPrivateItem) return;
			RaiseChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			BarItem bItem = item as BarItem;
			if(NameHash.ContainsKey(bItem.Name)) NameHash.Remove(bItem.Name);
			if(Manager == null || Manager.AllowDisposeItems) {
				DisposeBarItem(bItem);
			}
			base.OnRemoveComplete(index, item);
			RaiseChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected void DisposeBarItem(BarItem bItem) {
			if(bItem.Manager != null) {
				bItem.Manager.OnItemRemoved(bItem);
				return;
			}
			bItem.Manager = null;
			bItem.Dispose();
		}
		protected override void OnInsertComplete(int index, object item) {
			BarItem barItem = (BarItem)item;
			if(barItem.Name != null && barItem.Name.Length > 0) NameHash[barItem.Name] = barItem;
			if(barItem.Manager != Manager) {
				barItem.Manager = Manager;
			}
			base.OnInsertComplete(index, item);
			RaiseChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		internal void UpdateItemHash(BarItem item, string oldName, string newName) {
			if(NameHash.ContainsKey(oldName)) NameHash.Remove(oldName);
			if(newName != null && newName.Length > 0) NameHash[newName] = item;
		}
		public virtual bool Contains(BarItem item) {
			return List.Contains(item);
		}
		public virtual int IndexOf(BarItem item) {
			return List.IndexOf(item);
		}
		public virtual void Insert(int index, object item) {
			BarItem barItem = (BarItem)item;
			if(barItem.Manager == Manager) return;
			List.Insert(index, item);
		}
		public virtual void Remove(BarItem item) {
			if(List.Contains(item))	List.Remove(item);
		}
		public new virtual void RemoveAt(int index) {
			List.RemoveAt(index);
		}
		internal void InternalAdd(object item) {
			if(InnerList.Contains(item)) return;
			InnerList.Add(item);
		}
		internal void InternalInsert(int index, object item) {
			InnerList.Insert(index, item);
		}
		internal void InternalRemove(object item) {
			InnerList.Remove(item);
		}
		public BarButtonItem CreateSplitButton(string caption, PopupMenu menu) {
			menu.Manager = Manager;
			BarButtonItem item = new BarButtonItem(Manager, caption);
			item.ButtonStyle = BarButtonStyle.DropDown;
			item.DropDownControl = menu;
			return item;
		}
		public BarButtonItem CreateButton(string caption) {
			BarButtonItem item = new BarButtonItem(Manager, caption);
			return item;
		}
		public BarCheckItem CreateCheckItem(string caption, bool check) {
			BarCheckItem item = new BarCheckItem(null, check);
			item.Caption = caption;
			item.Manager = Manager;
			return item;
		}
		public PopupMenu CreatePopupMenu(params BarItem[] items) {
			PopupMenu menu = new PopupMenu(Manager);
			menu.ItemLinks.AddRange(items);
			return menu;
		}
		public BarSubItem CreateMenu(string caption, params BarItem[] items) {
			BarSubItem item = new BarSubItem(Manager, caption, items);
			return item;
		}
	}
	[ListBindable(false)]
	public class ReadOnlyListBase : ReadOnlyCollectionBase, IList {
		public ReadOnlyListBase() {
		}
		int IList.Add(object value) { throw new NotImplementedException();  }
		bool IList.Contains(object item) { return InnerList.Contains(item); }
		int IList.IndexOf(object item) { return InnerList.IndexOf(item); }
		void IList.Clear() { throw new NotImplementedException(); }
		void IList.Insert(int index, object val) { throw new NotImplementedException(); }
		bool IList.IsFixedSize { get { return false; } }
		bool IList.IsReadOnly { get { return true; } }
		object IList.this[int index] { get { return InnerList[index]; } set { throw new NotImplementedException(); } }
		void IList.RemoveAt(int index) { throw new NotImplementedException(); }
		void IList.Remove( object val) { throw new NotImplementedException(); }
		protected virtual bool OnInsert(int index, object item) {
			return true;
		}
		protected internal virtual int AddItem(object item) {
			if(!OnInsert(Count, item)) return -1;
			int index = InnerList.Add(item);
			OnInsertComplete(index, item);
			return index;
		}
		protected internal void InsertItem(int index, object item) {
			if(!OnInsert(index, item)) return;
			InnerList.Insert(index, item);
			OnInsertComplete(index, item);
		}
		protected internal void RemoveItem(object item) {
			int index = InnerList.IndexOf(item);
			if(index == -1) return;
			RemoveItemAt(index);
		}
		protected internal void RemoveItemAt(int index) {
			object item = InnerList[index];
			InnerList.RemoveAt(index);
			OnRemoveComplete(index, item);
		}
		protected internal void ClearItems() {
			OnClear();
			OnClearComplete();
		}
		protected virtual void OnClear() { InnerList.Clear(); }
		protected virtual void OnClearComplete() { }
		protected virtual void OnInsertComplete(int index, object item) { }
		protected virtual void OnRemoveComplete(int index, object item) { }
	}
	public class BarItemLinkReadOnlyCollection : ReadOnlyListBase, IList {
		ArrayList recentList;
		int lockUpdate = 0;
		public BarItemLinkReadOnlyCollection() {
			this.recentList = new ArrayList();
		}
		protected virtual void BeginUpdate() {
			this.lockUpdate ++;
		}
		protected virtual bool EndUpdate() {
			if(--this.lockUpdate == 0) return true;
			return false;
		}
		protected internal void AddLinkRange(ICollection items) {
			BeginUpdate();
			try {
				foreach(BarItemLink link in items) {
					AddItem(link);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal int IsLockUpdate { get { return lockUpdate; } }
		protected override void OnClear() {
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) RemoveItemAt(n);
			} finally {
				EndUpdate();
			}
			RecentList.Clear();
		}
		protected virtual ArrayList RecentList { get { return recentList; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemLinkReadOnlyCollectionItem")]
#endif
		public BarItemLink this[int index] { get { return InnerList[index] as BarItemLink; } }
		public int IndexOf(BarItemLink link) { return InnerList.IndexOf(link); }
		public bool Contains(BarItemLink link) { return InnerList.Contains(link); }
		protected override void OnRemoveComplete(int position, object item) {
			if(RecentList != null && RecentList.Contains(item)) RecentList.Remove(item);
		}
		protected override void OnInsertComplete(int position, object item) { 
			RecentList.Add(item);
		}
		class RecentComparer : IComparer {
			BarItemLinkReadOnlyCollection links;
			public RecentComparer(BarItemLinkReadOnlyCollection links) {
				this.links = links;
			}
			int IComparer.Compare(object o1, object o2) {
				BarItemLink link1 = o1 as BarItemLink, link2 = o2 as BarItemLink;
				if(link1 == link2) return 0;
				if(IsSystemLink(link1)) return 1;
				if(IsSystemLink(link2)) return -1;
				if(link1.MostRecentlyUsed == link2.MostRecentlyUsed) {
					int res = link2.ClickCount.CompareTo(link1.ClickCount);
					if(res == 0) return this.links.IndexOf(link1).CompareTo(this.links.IndexOf(link2));
					return res;
				}
				if(link1.MostRecentlyUsed) return -1;
				return 1;
			}
			bool IsSystemLink(BarItemLink link) {
				return ((link.Item is BarQBarCustomizationItem) || (link.Item is BarDesignTimeItem));
			}
		}
		internal void SortRecentList() {
			RecentList.Sort(new RecentComparer(this));
			UpdateRecentIndexes();
		}
		internal void UpdateMostRecentlyUsed(BarItemLink link, bool touchOther) {
			if(link.MostRecentlyUsed) {
				if(link.ClickCount < link.Manager.GetController().PropertiesBar.MostRecentlyUsedClickCount)
					link.ClickCount = link.Manager.GetController().PropertiesBar.MostRecentlyUsedClickCount;
				UpdateRecentIndex(link, 0, touchOther, touchOther);
			} else {
				link.ClickCount = 0;
				UpdateRecentIndex(link, Count, touchOther, touchOther);
				link.recentIndex = -1;
			}
		}
		internal void UpdateRecentIndex(BarItemLink link, int newRecentIndex) {
			UpdateRecentIndex(link, newRecentIndex, true, true);
		}
		internal void UpdateRecentIndex(BarItemLink link, int newRecentIndex, bool updateOther, bool replaceRecentIndex) {
			if(newRecentIndex < 0) newRecentIndex = -1;
			if(replaceRecentIndex) link.recentIndex = newRecentIndex;
			int prevIndex = recentList.IndexOf(link);
			if(prevIndex == newRecentIndex) return;
			recentList.Remove(link);
			if(newRecentIndex == -1) {
				if(updateOther) UpdateRecentIndexes();
				return;
			}
			if(prevIndex != -1) {
				if(prevIndex < newRecentIndex) newRecentIndex --;
			}
			if(newRecentIndex >= recentList.Count) 
				recentList.Add(link);
			else
				recentList.Insert(newRecentIndex, link);
			if(updateOther) UpdateRecentIndexes();
		}
		internal void ResetRecentIndexes() {
			RecentList.Clear();
			RecentList.AddRange(this);
			ClearRecentIndexes();
			UpdateRecentIndexes();
		}
		internal void ClearRecentIndexes() {
			foreach(BarItemLink link in this) 
				link.recentIndex = -1;
		}
		internal void UpdateRecentIndexes() {
			for(int n = 0; n < recentList.Count; n++) {
				GetRecentLink(n).recentIndex = n;
			}
		}
		internal void CopyTo(Array array) {
			InnerList.CopyTo(array);
		}
		public BarItemLink GetRecentLink(int index) { return RecentList[index] as BarItemLink; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemLinkReadOnlyCollectionRecentLinkCount")]
#endif
		public int RecentLinkCount { get { return RecentList.Count; } }
		protected internal virtual bool IsMergedState { get { return false; } }
		#region IList Members
		int IList.Add(object value) {
			BarItemLink link = value as BarItemLink;
			if(link == null) throw new Exception("Invalid parameter.");
			return AddItem(link);
		}
		void IList.Clear() { ClearItems(); }
		bool IList.Contains(object value) { return Contains(value as BarItemLink); }
		int IList.IndexOf(object value) { return IndexOf(value as BarItemLink); }
		void IList.Insert(int index, object value) {
			BarItemLink link = value as BarItemLink;
			if(link == null) throw new Exception("Invalid parameter.");
			InsertItem(index, link);
		}
		void IList.Remove(object value) {
			RemoveItem(value);
		}
		void IList.RemoveAt(int index) {
			RemoveItemAt(index);
		}
		object IList.this[int index] { get { return this[index]; } set { } }
		#endregion
	}
	public class BarItemLinkCollection : BarItemLinkReadOnlyCollection, BarLinksHolder, IList, IVisualEffectsHolder, IEnumerable<BarItemLink> {
		object owner;
		BarItemLinkReadOnlyCollection mergeSource = null;
		bool mergedState = false;
		int lockPersistInfo = 0;
		LinksInfo linksPersistInfo;
		public event CollectionChangeEventHandler CollectionChanged;
		public BarItemLinkCollection() : this(null) {
		}
		public BarItemLinkCollection(object owner) {
			this.owner = owner;
			this.linksPersistInfo = new LinksInfo();
		}
		internal BarItemLinkCollection(bool allowPersist) {
			this.owner = null;
			this.linksPersistInfo = null;
		}
		protected override void OnClear() {
			base.OnClear();
			OnChanged();
		}
		protected internal override int AddItem(object item) {
			if(InnerList.Contains(item)) return IndexOf(item as BarItemLink);
			return base.AddItem(item);
		}
		protected internal virtual object Owner { get { return owner; } }
		protected internal BarItemLink Add(BarItemLink itemLink) {
			AddItem(itemLink);
			return itemLink;
		}
		protected internal BarItemLink Insert(int position, BarItemLink itemLink) {
			InsertItem(position, itemLink);
			return itemLink;
		}
		public void Assign(BarItemLinkCollection links) {
			BeginUpdate();
			Clear();
			try {
				foreach(BarItemLink link in links) {
					BarItemLink link2 = Add(link.Item);
					link2.BeginGroup = link.BeginGroup;
				}
			}
			finally {
				EndUpdate();
				OnChanged();
			}
		}
		protected virtual BarManager Manager {
			get {
				BarLinksHolder holder = Owner as BarLinksHolder;
				return holder == null ? null : holder.Manager;
			}
		}
		public virtual BarItemLink Add(BarItem item, bool beginGroup) {
			BarItemLink link = Add(item);
			if(link != null) link.BeginGroup = beginGroup;
			return link;
		}
		public virtual BarItemLink Add(BarItem item, bool beginGroup, string keyTip) {
			BarItemLink link = Add(item, beginGroup);
			if(link != null) link.KeyTip = keyTip;
			return link;
		}
		public virtual BarItemLink Add(BarItem item, bool beginGroup, string keyTip, string dropDownKeyTip) {
			BarItemLink link = Add(item, beginGroup, keyTip);
			BarButtonItemLink buttonLink = link as BarButtonItemLink;
			if(buttonLink != null) buttonLink.DropDownKeyTip = dropDownKeyTip;
			return link;
		}
		public virtual BarItemLink Add(BarItem item, bool beginGroup, string keyTip, string dropDownKeyTip, bool buttonGroup) {
			BarItemLink link = Add(item, beginGroup, keyTip, dropDownKeyTip);
			if(link != null) link.ActAsButtonGroup = buttonGroup;
			return link;
		}
		public virtual BarItemLink Add(BarItem item, string keyTip, string dropDownKeyTip) {
			BarItemLink link = Add(item, keyTip);
			BarButtonItemLink buttonLink = link as BarButtonItemLink;
			if(buttonLink != null) buttonLink.DropDownKeyTip = dropDownKeyTip;
			return link;
		}
		public virtual BarItemLink Add(BarItem item, string keyTip) {
			BarItemLink link = Add(item);
			if(link != null) link.KeyTip = keyTip;
			return link;
		}
		int IList.Add(object value) {
			BarItem item = value as BarItem;
			if(item == null) throw new Exception("Invalid parameter.");
			return IndexOf(Add(item));
		}
		void IList.Insert(int index, object value) {
			BarItem item = value as BarItem;
			if(item == null) throw new Exception("Invalid parameter.");
			Insert(index, item);
		}
		public virtual BarItemLink Add(BarItem item) { return Add(item, (LinkPersistInfo)null); }
		protected internal BarItemLink Add(BarItem item, LinkPersistInfo info) {
			BarItemLink itemLink = CreateLink(item);
			if(info != null) itemLink.Assign(info);
			AddItem(itemLink);
			return itemLink;
		}
		protected BarItemLink AddMergeLink(BarItemLink sourceLink) {
			return Add(sourceLink.Item, new LinkPersistInfo(sourceLink));
		}
		public virtual BarItemLink Insert(int position, BarItem item) {
			BarItemLink itemLink = CreateLink(item);
			InsertItem(position, itemLink);
			return itemLink;
		}
		public virtual BarItemLink Insert(BarItemLink beforeLink, BarItem item) {
			BarItemLink itemLink = CreateLink(item);
			int index = IndexOf(beforeLink);
			if(index == -1) {
				AddItem(itemLink);
			} else {
				InsertItem(index, itemLink);
			}
			return itemLink;
		}
		public virtual void Clear() {
			ClearItems();
		}
		public virtual void RemoveAt(int index) { RemoveItemAt(index); }
		public virtual void Remove(BarItemLink link) {
			RemoveItem(link);
		}
		public void AddRange(params BarItem[] items) {
			AddRange((IEnumerable<BarItem>)items);
		}
		public void AddRange(IEnumerable<BarItem> items) {
			BeginUpdate();
			try {
				foreach(BarItem item in items) Add(item);
			} finally {
				EndUpdate();
				OnChanged();
			}
		}
		bool BarLinksHolder.Enabled { get { return true; } }
		protected override void OnRemoveComplete(int position, object item) {
			BarItemLink link = item as BarItemLink;
			if(LinksPersistInfo != null && !IsLockPersistInfo) LinksPersistInfo.RemoveLink(link);
			if(link.Item != null) {
				link.Item.Links.RemoveItem(link.LinkedObject);
				link.Item.Links.RemoveItem(link);
			}
			base.OnRemoveComplete(position, link);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, link));
		}
		protected override void OnInsertComplete(int position, object item) {
			base.OnInsertComplete(position, item);
			BarItemLink link = item as BarItemLink;
			if(!IsLockPersistInfo) {
				if(LinksPersistInfo != null) {
					LinkPersistInfo pi = new LinkPersistInfo(link);
					if(position == -1 || position >= LinksPersistInfo.Count)
						LinksPersistInfo.Add(pi);
					else 
						LinksPersistInfo.Insert(position, pi);
				}
			}
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, link));
		}
		protected virtual void ReCreatePersistInfo() {
			if(LinksPersistInfo == null) return;
			LinksPersistInfo.Clear();
			foreach(BarItemLink link in this) {
				LinksPersistInfo.Add(new LinkPersistInfo(link));
			}
		}
		protected virtual void LockPersistInfo() {
			this.lockPersistInfo ++;
		}
		protected virtual void UnLockPersistInfo() {
			this.lockPersistInfo --;
		}
		protected virtual bool IsLockPersistInfo { get { return lockPersistInfo != 0; } }
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		protected virtual BarItemLink CreateLink(BarItem item) {
			if(item.Manager == null) item.Manager = Manager;
			BarItemLink link = item.CreateLink(this, Owner); 
			item.Links.AddItem(link);
			return link;
		}
		protected internal virtual LinksInfo LinksPersistInfo { 
			get { return linksPersistInfo; } 
			set { linksPersistInfo = value; }
		}
		protected internal virtual void ReMerge() {
			if(!IsMergedState) return;
			MergeCore(MergeSource);
		}
		protected internal virtual void Merge(BarItemLinkReadOnlyCollection links) {
			Merge(links, false);
		}
		protected internal virtual void Merge(BarItemLinkReadOnlyCollection links, bool force) {
			if(links == null) return;
			if(!force && IsMergedState) return;
			this.mergedState = true;
			this.mergeSource = links;
			MergeCore(links);
		}
		protected internal virtual bool CanVisibleMerged(BarItemLink link) {
			if(link.Item == null) return false;
			BarMenuMerge mergeType = link.Item.MergeType;
			if(mergeType == BarMenuMerge.Remove) return false;
			if(mergeType != BarMenuMerge.Replace) return true;
			if(link.Item.Manager != GetManager()) return true;
			foreach(BarItemLink mLink in this) {
				if(link != mLink && link.Item.Caption == mLink.Item.Caption && 
					link.Item.Manager != mLink.Item.Manager && mLink.Item.MergeType == mergeType) return false;
			}
			return true;
		}
		protected virtual void MergeCore(BarItemLinkReadOnlyCollection links) {
			if(links == null) return;
			LockPersistInfo();
			BeginUpdate();
			try {
				ArrayList list = new ArrayList(), removeLinks = new ArrayList();
				CopyToList(list, this, false);
				CopyToList(list, links, true);
				list.Sort(new MergeLinksComparer());
				for(int n = 0; n < list.Count; n++) {
					MergeLinkInfo info = list[n] as MergeLinkInfo;
					if(info.Links == this && info.Link.Item.MergeType == BarMenuMerge.MergeItems) {
						MergeLinkInfo mergeInfo = info.FindMergeInfo(list, links);
						if(mergeInfo == null) continue;
						BarLinksHolder h1 = info.Link.Item as BarLinksHolder;
						BarLinksHolder h2 = mergeInfo.Link.Item as BarLinksHolder;
						if(h1 != null && h2 != null) {
							removeLinks.Add(mergeInfo);
							if(ForceMerge) 
								h1.ItemLinks.Merge(h2.ItemLinks, true);
							else
								h1.ItemLinks.Merge(h2.ItemLinks);
						}
					}
				}
				InnerList.Clear();
				foreach(MergeLinkInfo info in list) {
					if(removeLinks.Contains(info)) continue;
					if(info.Links == this) 
						InnerList.Add(info.Link);
					else 
						AddMergeLink(info.Link);
				}
			}
			finally {
				EndUpdate();
				UnLockPersistInfo();
			}
			OnChanged();
		}
		internal static bool ForceMerge = false;
		BarManager GetManager() {
			BarLinksHolder holder = Owner as BarLinksHolder;
			if(holder != null) return holder.Manager;
			BarItemLink link = Owner as BarItemLink;
			Bar bar = Owner as Bar;
			if(bar != null) return bar.Manager;
			if(link != null) return link.Manager;
			return null;
		}
		protected internal BarItemLinkReadOnlyCollection MergeSource { get { return mergeSource; } }
		internal void SetMergeSource(BarItemLinkReadOnlyCollection source) { this.mergeSource = source; }
		protected internal virtual void UnMerge() {
			if(!IsMergedState) return;
			this.mergedState = false;
			this.mergeSource = null;
			BarManager manager = GetManager();
			LockPersistInfo();
			BeginUpdate();
			try {
				ArrayList list = new ArrayList();
				list.AddRange(this);
				InnerList.Clear();
				foreach(BarItemLink link in list) {
					if(link.Item.Manager == manager) {
						BarLinksHolder holder = link.Item as BarLinksHolder;
						if(holder != null) holder.ItemLinks.UnMerge();
						InnerList.Add(link);
					} else
						link.Dispose();
				}
			}
			finally {
				RemoveMergedLinksFromRecentList();
				EndUpdate();
				UnLockPersistInfo();
				ReCreatePersistInfo();
			}
			OnChanged();
		}
		protected virtual void RemoveMergedLinksFromRecentList() {
			BarManager manager = GetManager();
			for(int i = RecentList.Count - 1; i >= 0; i--) {
				BarItemLink link = (BarItemLink)RecentList[i];
				if(link.Item == null || link.Item.Manager != manager) {
					RecentList.Remove(link);
				}
			}
		}
		protected void CopyToList(ArrayList list, BarItemLinkReadOnlyCollection links, bool canRemove) {
			foreach(BarItemLink link in links) {
				if(canRemove && link.Item.MergeType == BarMenuMerge.Remove) continue;
				list.Add(new MergeLinkInfo(link, list.Count, links));
			}
		}
		protected class MergeLinkInfo {
			public MergeLinkInfo(BarItemLink link, int index, BarItemLinkReadOnlyCollection links) { 
				this.Link = link;
				this.Index = index;
				this.Links = links;
			}
			public BarItemLinkReadOnlyCollection Links;
			public BarItemLink Link;
			public int Index;
			public MergeLinkInfo FindMergeInfo(ArrayList list, BarItemLinkReadOnlyCollection links) {
				foreach(MergeLinkInfo info in list) {
					if(info.Links != links) continue;
					if(info.Link.Item.Caption == this.Link.Item.Caption && info.Link.GetType().Equals(this.Link.GetType()) 
						&& info.Link.Item.MergeType == BarMenuMerge.MergeItems) {
						return info;
					}
				}
				return null;
			}
		}
		protected class MergeLinksComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				MergeLinkInfo link1 = a as MergeLinkInfo, link2 = b as MergeLinkInfo;
				if(link1 == link2) return 0;
				int cmp = link1.Link.Item.MergeOrder.CompareTo(link2.Link.Item.MergeOrder);
				if(cmp != 0) return cmp;
				return link1.Index.CompareTo(link2.Index);
			}
		}
		protected void OnChanged() {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected internal override bool IsMergedState { get { return mergedState; } }
		internal void SetIsMergedState(bool value) { this.mergedState = value; }
		protected internal void Assign(LinksInfo linksInfo) {
			if(linksInfo == null) return;
			try {
				BeginUpdate();
				Clear();
				foreach(LinkPersistInfo info in linksInfo) {
					BarItemLink link = Add(info.Item);
					link.Assign(info);
				}
			} finally {
				EndUpdate();
				OnChanged();
			}
		}
		#region BarLinksHolder Members
		BarManager BarLinksHolder.Manager { get { return Manager; } }
		BarItemLink BarLinksHolder.InsertItem(BarItemLink beforeLink, BarItem item) { return Insert(beforeLink, item); }
		BarItemLink BarLinksHolder.AddItem(BarItem item) { return  Add(item); }
		BarItemLink BarLinksHolder.AddItem(BarItem item, LinkPersistInfo info) { return Add(item, info);  }
		MenuDrawMode BarLinksHolder.MenuDrawMode { get { return MenuDrawMode.Default; } }
		void BarLinksHolder.RemoveLink(BarItemLink itemLink) { Remove(itemLink); }
		BarItemLinkCollection BarLinksHolder.ItemLinks { get { return this; } }
		void BarLinksHolder.BeginUpdate() { BeginUpdate();  }
		void BarLinksHolder.EndUpdate() { EndUpdate(); }
		void BarLinksHolder.ClearLinks() { Clear(); }
		#endregion
		protected internal bool HasVisibleItems {
			get {
				bool hasVisibleItems = false;
				foreach(BarItemLink link in this) {
					if(link.Visible) {
						if(link.Ribbon != null && link.Ribbon.AllowInplaceLinks) {
							BarLinkContainerExItem item = link.Item as BarLinkContainerExItem;
							if(item != null && !item.ItemLinks.HasVisibleItems)
								continue;
						}
						if(link.Item != null && link.Item.Visibility != BarItemVisibility.Never && link.Item.Visibility != BarItemVisibility.OnlyInCustomizing) {
							hasVisibleItems = true;
							break;
						}
					}
				}
				return hasVisibleItems;
			}
		}
		bool IVisualEffectsHolder.VisualEffectsVisible {
			get { return GetVisulEffectsVisible(); }
		}
		protected virtual bool GetVisulEffectsVisible() {
			IVisualEffectsHolder holder = owner as IVisualEffectsHolder;
			return holder != null ? holder.VisualEffectsVisible : false;
		}
		DevExpress.Utils.VisualEffects.ISupportAdornerUIManager IVisualEffectsHolder.VisualEffectsOwner {
			get { return GetVisualEffectsOwner(); }
		}
		protected virtual DevExpress.Utils.VisualEffects.ISupportAdornerUIManager GetVisualEffectsOwner() {
			IVisualEffectsHolder holder = owner as IVisualEffectsHolder;
			return holder != null ? holder.VisualEffectsOwner : null;
		}
		#region IEnumerable<BarItemLink>
		IEnumerator<BarItemLink> IEnumerable<BarItemLink>.GetEnumerator() {
			foreach(BarItemLink link in InnerList)
				yield return link;
		}
		#endregion
	}
}
