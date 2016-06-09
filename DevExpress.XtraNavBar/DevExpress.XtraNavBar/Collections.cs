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
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
namespace DevExpress.XtraNavBar {
	public class CollectionItemEventArgs : EventArgs {
		object item;
		public CollectionItemEventArgs(object item) {
			this.item = item;
		}
		public object Item { get { return item; } }
	}
	public delegate void CollectionItemEventHandler(object sender, CollectionItemEventArgs e);
	public class Collection : CollectionBase {
		int lockUpdate = 0;
		public event CollectionChangeEventHandler CollectionChanged;
		public event CollectionItemEventHandler CollectionItemChanged;
		protected virtual ICollectionItem CreateItem() {
			return null;
		}
		protected virtual ICollectionItem Add() {
			ICollectionItem item = CreateItem();
			return item;
		}
		protected ICollectionItem this[string name] {
			get { 
				for(int n = Count - 1; n >= 0; n--) {
					ICollectionItem item = List[n] as ICollectionItem;
					if(item.ItemName == name) return item;
				}
				return null;
			}
		}
		public virtual void Add(ICollectionItem item) {
			AddItem(item);
		}
		public virtual object Insert(int index, ICollectionItem item) {
			List.Insert(index, item);
			return item;
		}
		public virtual int IndexOf(ICollectionItem item) {
			return List.IndexOf(item);
		}
		protected virtual bool CheckIndex(int index) {
			if(index < 0 || index >= Count) return false;
			return true;
		}
		public virtual void Move(int fromIndex, int toIndex) {
			if(fromIndex == toIndex || !CheckIndex(fromIndex) || !CheckIndex(toIndex)) return;
			object oldItem = InnerList[fromIndex];
			InnerList.RemoveAt(fromIndex);
			if(toIndex >= InnerList.Count) 
				InnerList.Add(oldItem);
			else
				InnerList.Insert(toIndex, oldItem);
			RaiseOnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		internal ICollectionItem AddItem(ICollectionItem item) {
			if(List.Contains(item) && item.Collection == this) return item;
			List.Add(item);
			return item;
		}
		internal void RemoveItem(ICollectionItem item) {
			List.Remove(item);
		}
		public virtual void Remove(ICollectionItem item) {
			RemoveItem(item);
		}
		protected virtual void BeginUpdate() {
			lockUpdate ++;
		}
		protected virtual void EndUpdate() {
			if(-- lockUpdate == 0)
				RaiseOnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected override void OnInsert(int index, object item) {
			base.OnInsert(index, item);
			ICollectionItem colItem = item as ICollectionItem;
			if(colItem == null) throw new ArgumentException("invalid collectionItem");
			if(colItem.Collection != null) {
				if(colItem.Collection == this && !List.Contains(item)) return;
				throw new ArgumentException("already in collection");
			}
		}
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			ICollectionItem colItem = item as ICollectionItem;
			colItem.SetCollection(this);
			colItem.ItemChanged += new EventHandler(Collection_CollectionItemChanged);
			RaiseOnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			ICollectionItem colItem = item as ICollectionItem;
			colItem.SetCollection(null);
			colItem.ItemChanged -= new EventHandler(Collection_CollectionItemChanged);
			RaiseOnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnClear() {
			int count = Count;
			if(count == 0) return;
			try {
				for(int n = count - 1; n >= 0; n--) {
					RemoveAt(n);
				}
			}
			finally {
			}
		}
		void Collection_CollectionItemChanged(object sender, EventArgs e) {
			OnCollectionItemChanged(sender);
		}
		protected virtual void OnCollectionItemChanged(object item) {
			if(lockUpdate != 0) return;
			if(CollectionItemChanged != null)
				CollectionItemChanged(this, new CollectionItemEventArgs(item));
		}
		protected virtual void RaiseOnCollectionChanged(CollectionChangeEventArgs e) {
			if(lockUpdate != 0) return;
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}
	}
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))
#if DXWhidbey
   , DesignerSerializer("DevExpress.Utils.Design.DXCollectionCodeDomSerializer, " + AssemblyInfo.SRAssemblyDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")
#endif
	]
	public class NavGroupCollection : Collection, DevExpress.Utils.Design.ICollectionEditorSupport, IEnumerable<NavBarGroup> {
		NavBarControl navBar;
		public NavGroupCollection(NavBarControl navBar) {
			this.navBar = navBar;
		}
		void DevExpress.Utils.Design.ICollectionEditorSupport.ReplaceItems(object[] items) {
			InnerList.Clear();
			if(items == null) return;
			NavBar.BeginUpdate();
			try {
				foreach(object item in items) {
					Add(item as ICollectionItem);
				}
			} finally {
				NavBar.EndUpdate();
			}
		}
		protected NavBarControl NavBar { get { return navBar; } }
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavGroupCollectionItem")]
#endif
		public virtual NavBarGroup this[int index] {
			get { return List[index] as NavBarGroup; }
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavGroupCollectionItem")]
#endif
		public new virtual NavBarGroup this[string name] {
			get { return base[name] as NavBarGroup;	}
		}
		public NavBarGroup Add(NavBarGroup group) {
			if(group.NavBar == null) group.SetNavBarCore(NavBar);
			return AddItem(group) as NavBarGroup;
		}
		public new virtual NavBarGroup Add() {
			return AddItem(CreateItem()) as NavBarGroup;
		}
		public virtual void AddRange(NavBarGroup[] groups) {
			foreach(NavBarGroup group in groups) AddItem(group);
		}
		protected override ICollectionItem CreateItem() {
			return new NavBarGroup();
		}
		internal void RestoreGroupOrderCore(ArrayList order) {
			ArrayList list = InnerList.Clone() as ArrayList;
			InnerList.Clear();
			foreach(string groupName in order) {
				NavBarGroup group = FindGroup(list, groupName);
				if(group != null) InnerList.Add(group);
			}
			for(int n = list.Count - 1; n >= 0; n--) {
				NavBarGroup group = list[n] as NavBarGroup;
				if(InnerList.Contains(group)) continue;
				group.Visible = false;
				InnerList.Add(group);
			}
		}
		NavBarGroup FindGroup(ArrayList list, string name) {
			foreach(NavBarGroup group in list) {
				if(group.Name == name) return group;
			}
			return null;
		}
		public int GetVisibleGroupCount() {
			int count = 0;
			foreach(NavBarGroup group in this) {
				if(group.IsVisible) count ++;
			}
			return count;
		}
		#region IEnumerable<NavBarGroup> Members
		IEnumerator<NavBarGroup> IEnumerable<NavBarGroup>.GetEnumerator() {
			foreach(NavBarGroup group in InnerList)
				yield return group;
		}
		#endregion
	}	
	public class NavReadOnlyLinkCollection : ReadOnlyCollectionBase, System.Collections.Generic.IEnumerable<NavBarItemLink>, Utils.ISupportSearchDataAdapter {
		DevExpress.Utils.SearchDataAdapter adapter;
		public NavReadOnlyLinkCollection() : base() {
			adapter = new DevExpress.Utils.SearchDataAdapter<NavBarItemLink>();
			adapter.SetDataSource(InnerList);
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavReadOnlyLinkCollectionItem")]
#endif
		public virtual NavBarItemLink this[int index] {
			get { 
				return adapter.GetValueAtIndex(string.Empty, index) as NavBarItemLink; 
			}
		}
		internal void AddLink(NavBarItemLink link) {
			InnerList.Add(link);
		}
		internal void ClearLinks() {
			InnerList.Clear();
		}
		internal void RemoveLink(NavBarItemLink link) {
			InnerList.Remove(link);
		}
		public virtual int IndexOf(NavBarItemLink link) {
			int index = InnerList.IndexOf(link);
			return adapter.GetControllerRow(index);
		}
		System.Collections.Generic.IEnumerator<NavBarItemLink> System.Collections.Generic.IEnumerable<NavBarItemLink>.GetEnumerator() {
			foreach(NavBarItemLink link in InnerList)
				yield return link;
		}
		public override int Count { get { return adapter.VisibleCount; } }
		#region ISupportSearchDataAdapter Members
		bool DevExpress.Utils.ISupportSearchDataAdapter.AdapterEnabled {
			get { return true; }
			set { }
		}
		Data.Filtering.CriteriaOperator Utils.ISupportSearchDataAdapter.FilterCriteria {
			get { return adapter.FilterCriteria; }
			set { adapter.FilterCriteria = value; }
		}
		int Utils.ISupportSearchDataAdapter.GetSourceIndex(int filteredIndex) { return filteredIndex; }
		int Utils.ISupportSearchDataAdapter.GetVisibleIndex(int index) { return index; }
		int Utils.ISupportSearchDataAdapter.VisibleCount { get { return adapter.VisibleCount; } }
		#endregion
	}
	[ListBindable(false)
#if DXWhidbey
, DesignerSerializer("DevExpress.Utils.Design.DXCollectionCodeDomSerializer, " + AssemblyInfo.SRAssemblyDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design"),
#endif
	]
	public class NavLinkCollection : Collection, IEnumerable<NavBarItemLink> {
		NavBarGroup group;
		public NavLinkCollection(NavBarGroup group) {
			this.group = group;
		}
		[Browsable(false)]
		public NavBarGroup Group { get { return group; } }
		protected NavBarControl NavBar { get { return Group == null ? null : Group.NavBar; } }
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavLinkCollectionItem")]
#endif
		public virtual NavBarItemLink this[int index] {
			get { return List[index] as NavBarItemLink; }
		}
		class CaptionSorter : IComparer {
			public int Compare(object a, object b) {
				NavBarItemLink link1 = a as NavBarItemLink, link2 = b as NavBarItemLink;
				int res = Comparer.Default.Compare(link1.Caption, link2.Caption);
				if(res == 0) return Comparer.Default.Compare(link1.GetHashCode(), link2.GetHashCode());
				return res;
			}
		}
		public virtual void SortByCaption() {
			Sort(new CaptionSorter());
		}
		public bool Contains(NavBarItemLink link) { return List.Contains(link); }
		public virtual void Sort(IComparer comparer) {
			if(comparer == null) return;
			InnerList.Sort(comparer);
			RaiseOnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected override ICollectionItem CreateItem() {
			return null;
		}
		public virtual void AddRange(NavBarItemLink[] links) {
			foreach(NavBarItemLink link in links) AddItem(link);
		}
		public virtual NavBarItemLink Add(NavBarItem item) {
			if(item.NavBar == null) {
				item.SetNavBarCore(NavBar);
			}
			return AddItem(new NavBarItemLink(item)) as NavBarItemLink;
		}
		public override object Insert(int index, ICollectionItem item) {
			NavBarItemLink link = new NavBarItemLink(item as NavBarItem);
			List.Insert(index, link);
			return link;
		}
		public virtual void Remove(NavBarItem item) {
			for(int n = Count - 1; n >= 0; n--) {
				if(this[n].Item == item) RemoveAt(n);
			}
		}
		#region IEnumerable<NavBarItemLink> Members
		IEnumerator<NavBarItemLink> IEnumerable<NavBarItemLink>.GetEnumerator() {
			foreach(NavBarItemLink link in InnerList)
				yield return link;
		}
		#endregion
	}
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))
#if DXWhidbey
, DesignerSerializer("DevExpress.Utils.Design.DXCollectionCodeDomSerializer, " + AssemblyInfo.SRAssemblyDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")
#endif
	]
	public class NavItemCollection : Collection, DevExpress.Utils.Design.ICollectionEditorSupport, IEnumerable<NavBarItem> {
		NavBarControl navBar;
		public NavItemCollection(NavBarControl navBar) {
			this.navBar = navBar;
		}
		void DevExpress.Utils.Design.ICollectionEditorSupport.ReplaceItems(object[] items) {
			InnerList.Clear();
			if(items == null) return;
			NavBar.BeginUpdate();
			try {
				foreach(object item in items) {
					Add(item as ICollectionItem);
				}
			} finally {
				NavBar.EndUpdate();
			}
		}
		protected NavBarControl NavBar { get { return navBar; } }
		public NavBarItem Add(NavBarItem item) {
			if(item.NavBar == null) item.SetNavBarCore(NavBar);
			return AddItem(item) as NavBarItem;
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavItemCollectionItem")]
#endif
		public virtual NavBarItem this[int index] {
			get { return List[index] as NavBarItem; }
		}
#if !SL
	[DevExpressXtraNavBarLocalizedDescription("NavItemCollectionItem")]
#endif
		public new virtual NavBarItem this[string name] {
			get { return base[name] as NavBarItem; 	}
		}
		public new virtual NavBarItem Add() {
			return AddItem(CreateItem()) as NavBarItem;
		}
		public virtual NavBarItem Add(bool isSeparator) {
			if(!isSeparator) return Add();
			return AddItem(new NavBarSeparatorItem()) as NavBarItem;
		}
		public virtual void AddRange(NavBarItem[] items) {
			foreach(NavBarItem item in items) AddItem(item);
		}
		protected override ICollectionItem CreateItem() {
			return new NavBarItem();
		}
		#region IEnumerable<NavBarItem> Members
		IEnumerator<NavBarItem> IEnumerable<NavBarItem>.GetEnumerator() {
			foreach(NavBarItem item in InnerList)
				yield return item;
		}
		#endregion
	}
}
