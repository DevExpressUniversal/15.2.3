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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
namespace DevExpress.Xpf.NavBar.Internal {
	public class SelectionStrategy {
		#region properties
		readonly NavBarControl owner;
		readonly Locker publicLocker;
		readonly Locker multipleSelectionLocker;
		readonly Locker commonLocker;
		readonly Locker indexSelectionLocker;
		readonly Locker singleSelectionLocker;
		readonly Locker changingEventLocker;
		readonly Locker groupChangingEventLocker;
		private bool allowSelectMultiple;
		private bool allowSelectDisabled;
		private bool allowSelectItems;
		protected NavBarControl Owner { get { return owner; } }
		protected NavBarViewBase View { get { return Owner.View; } }		
		protected bool IsLocked { get { return publicLocker.IsLocked || commonLocker.IsLocked; } }
		public bool AllowGroupChangingEvent {
			get { return !groupChangingEventLocker.IsLocked; }
		}
		public bool AllowSelectItems {
			get { return allowSelectItems; }
			set {
				if (value == allowSelectItems) return;
				allowSelectItems = value;
				OnAllowSelectItemsChanged();
			}
		}
		public bool AllowSelectDisabled {
			get { return allowSelectDisabled; }
			set {
				if (value == allowSelectDisabled) return;
				allowSelectDisabled = value;
				OnAllowSelectDisabledChanged();
			}
		}
		public bool AllowSelectMultiple {
			get { return allowSelectMultiple; }
			set {
				if (value == allowSelectMultiple) return;
				allowSelectMultiple = value;
				OnAllowSelectMultipleChanged();
			}
		}
		#endregion
		private NavBarGroup selectedGroup;
		private NavBarItem selectedItem;
		private List<NavBarItem> selectedItems;
		private List<NavBarItem> pendingSelectedItems;
		public List<NavBarItem> SelectedItems {
			get { return multipleSelectionLocker.IsLocked ? pendingSelectedItems : selectedItems; }
			set {
				if (SelectedItems == value)
					return;
				var oldSelectedItems = SelectedItems;
				if (multipleSelectionLocker.IsLocked)
					pendingSelectedItems = value;
				else {
					selectedItems = value;
					UpdateOwnerSelectedItems(oldSelectedItems);
				}
			}
		}		
		public NavBarItem SelectedItem {
			get { return selectedItem; }
			set { 
				if(selectedItem==value)
					return;
				var oldSelectedItem = selectedItem;
				selectedItem = value;
				UpdateOwnerSelectedItem(oldSelectedItem);
			}
		}		
		public NavBarGroup SelectedGroup {
			get { return selectedGroup; }
			set {
				if (selectedGroup == value)
					return;
				var oldValue = selectedGroup;
				selectedGroup = value;
				UpdateOwnerSelectedGroup(oldValue);
			}
		}
		protected internal NavBarItem LastSelectedItem { get; set; }
		void UpdateOwnerSelectedGroup(NavBarGroup oldValue) {
			oldValue.Do(x => x.IsActive = false);
			Owner.SelectedGroup = SelectedGroup.With(x => GetGroupSourceObject(x)) ?? SelectedGroup;
			Owner.GetBindingExpression(NavBarControl.SelectedGroupProperty).Do(x => x.UpdateSource());
			Owner.ActiveGroup = SelectedGroup;
			if (SelectedGroup != null) {
				SelectedGroup.IsActive = true;
				SelectedGroup.UpdateScrollMode();
				SelectedGroup.RaiseActivateEvent();
			}
			if (View != null) View.RaiseEvent(new NavBarActiveGroupChangedEventArgs(SelectedGroup) { RoutedEvent = NavBarViewBase.ActiveGroupChangedEvent });
			if (SelectedGroup != null) {
				if (SelectedGroup.SelectedItem != null && (AllowSelectMultiple ? !SelectedItems.Contains(SelectedGroup.SelectedItem) : SelectedGroup.SelectedItem != SelectedItem)) {
					SelectItem(SelectedGroup.SelectedItem, SelectedGroup);
				}
				if (SelectedGroup.SelectedItem == null && SelectedGroup.SelectedItemIndex != -1) {
					SelectItem(SelectedGroup, SelectedGroup.SelectedItemIndex);
				}
			}
		}
		void UpdateOwnerSelectedItem(NavBarItem oldSelectedItem) {
			Owner.SelectedItem = SelectedItem.With(x=>x.SourceObject) ?? SelectedItem;
			if (AllowSelectMultiple)
				return;
			SelectedItems = new List<NavBarItem>();
			ClearItemSelection(oldSelectedItem);
			SelectedItem.Do(x => x.IsSelected = true);
			SelectedItem.With(x => x.Group).Do(x => x.SelectedItem = SelectedItem);
			View.RaiseEvent(new NavBarItemSelectedEventArgs(SelectedItem.With(x=>x.Group) ?? SelectedGroup, SelectedItem) { RoutedEvent = NavBarViewBase.ItemSelectedEvent });
		}
		private void ClearItemSelection(NavBarItem oldSelectedItem) {
			this.changingEventLocker.Lock();
			oldSelectedItem.Do(x => x.IsSelected = false);
			oldSelectedItem.With(x => x.Group).Do(x => x.SelectedItem = null);
			this.changingEventLocker.Unlock();
		}
		void UpdateOwnerSelectedItems(List<NavBarItem> oldSelectedItems) {
			commonLocker.Lock();
			if (AllowSelectMultiple) {
				SelectedItem = null;
			}
			var removedItems = oldSelectedItems.Except(SelectedItems);
			var addedItems = SelectedItems.Except(oldSelectedItems);
			List<NavBarItem> replacedItems = new List<NavBarItem>();
			foreach (var removedItem in removedItems) {
				var newItem = addedItems.FirstOrDefault(x => x.Group == removedItem.Group);
				if (newItem == null)
					continue;
				if (AllowSelectMultiple || SelectedItem != newItem) {
					replacedItems.Add(removedItem);
					removedItem.IsSelected = false;
					newItem.IsSelected = true;
					newItem.Group.Do(x => x.SelectedItem = newItem);
				}
			}
			removedItems = removedItems.Except(replacedItems);
			addedItems = addedItems.Except(replacedItems);
			foreach (var removedItem in removedItems) {
				if (AllowSelectMultiple || SelectedItem != removedItem) {
					removedItem.IsSelected = false;
					removedItem.Group.Do(x => x.SelectedItem = null);
				}				
			}
			foreach (var addedItem in addedItems) {
				addedItem.IsSelected = true;
				addedItem.Group.Do(x => x.SelectedItem = addedItem);
			}
			Owner.SelectedItems = SelectedItems.Select(x => GetSourceIfExists(x)).AsEnumerable();
			pendingSelectedItems = new List<NavBarItem>();
			commonLocker.Unlock();
		}
		public SelectionStrategy(NavBarControl owner) {
			this.owner = owner;
			this.selectedItems = new List<NavBarItem>();
			this.pendingSelectedItems = new List<NavBarItem>();
			this.publicLocker = new Locker();
			this.multipleSelectionLocker = new Locker();
			this.commonLocker = new Locker();
			this.indexSelectionLocker = new Locker();
			this.singleSelectionLocker = new Locker();
			this.changingEventLocker = new Locker();
			this.groupChangingEventLocker = new Locker();
			this.multipleSelectionLocker.Unlocked += (o, e) => SelectedItems = pendingSelectedItems.Where(x=>x!=null).ToList();			
		}
		public void Lock() {
			publicLocker.Lock();
		}
		public void Unlock() {
			publicLocker.Unlock();
			publicLocker.DoIfNotLocked(() => {
				SelectFirstVisibleGroup();
				UpdateSelectedItem();
			});
		}
		public void UpdateSelectedItem() {
			NavBarItem item = null;
			if (SelectedGroup != null) {
				item = SelectedGroup.SynchronizedItems.FirstOrDefault(x => x.SourceObject != null && x.SourceObject == SelectedGroup.SelectedItem.With(i => i.SourceObject)) ?? SelectedGroup.SelectedItem ?? SelectedGroup.SynchronizedItems.FirstOrDefault(x => x.IsSelected);
			}
			item = item ?? GetItem(Owner.SelectedItem);
			if (item != null)
				SelectItem(item, SelectedGroup);
		}
		public void SelectFirstVisibleGroup(bool ifExists = false) {
			if (IsLocked)
				return;
			groupChangingEventLocker.Lock();			
			SelectedGroup = (ifExists ? null : (GetGroup(Owner.SelectedGroup) ?? GetItem(Owner.SelectedItem).With(x => x.Group) ?? Owner.SelectedItems.OfType<object>().Select(x=>GetItem(x)).FirstOrDefault().With(x => x.Group))) ?? Owner.With(x => x.Groups).FirstOrDefault();
			changingEventLocker.DoLockedAction(() => {
				if (Owner.EachGroupHasSelectedItem)
					return;
				if (SelectedGroup != null) {
					foreach (var group in Owner.Groups) {
						if (group == SelectedGroup) continue;
						group.SelectedItemIndex = -1;
					}
				}			
			});			
			groupChangingEventLocker.Unlock();
		}
		public void SelectGroup(object element) {
			if (IsLocked)
				return;
			var newGroup = GetGroup(element);						
			SelectedGroup = newGroup;
		}
		public void SelectItem(object element, NavBarGroup group = null) {
			if (IsLocked)
				return;					   
			if (AllowSelectMultiple) {
				multipleSelectionLocker.DoLockedActionIfNotLocked(() => { InitializePendingSelection(); AddToSelection(element, group); });
			} else {
				singleSelectionLocker.DoLockedActionIfNotLocked(() => ReplaceSelection(element, group));
			}
			group = group ?? GetItem(element).With(x => x.Group);
			if (group != null && element!=null)
				SelectGroup(group);
		}
		private void ReplaceSelection(object element, NavBarGroup group) {
			if (AllowSelectMultiple)
				return;
			var oldItem = SelectedItem;
			var oldGroup = SelectedItem.With(x => x.Group);
			var newItem = GetItem(element, group);
			if (newItem == SelectedItem)
				return;
			group = group ?? newItem.With(x => x.Group);
			if (group == null && newItem == null) {
				SelectedItem = null;
				return;
			}
			if (newItem == null && group != SelectedGroup) {
				if (group.With(x => x.SelectedItem) != null)
					ClearItemSelection(group.SelectedItem);
				return;
			}
			SelectedItem = newItem;
			if (oldGroup != group && newItem!=null)
				SelectedGroup = group;
			if (SelectedItem != null)
				SelectedItem.RaiseSelectEvent();
		}
		void AddToSelection(object element, NavBarGroup group = null) {
			if (!AllowSelectMultiple)
				return;
			if (group != null) {
				RemoveFromSelection(SelectedItems.FirstOrDefault(x=>GetItem(x).Group==group), group);
			}
			SelectedItems.Add(GetItem(element, group) ?? GetItem(element));
		}
		void RemoveFromSelection(object element, NavBarGroup group = null) {
			if (group == null && element == null) return;
			var item = element==null ? group.With(x=>x.SelectedItem) : (GetItem(element, group) ?? GetItem(element));
			SelectedItems.Remove(item);			
		}
		public void SelectItems(IEnumerable elements) {
			if (IsLocked || !AllowSelectMultiple)
				return;
			multipleSelectionLocker.DoLockedActionIfNotLocked(()=>SelectItemsImpl(elements));			
		}
		void SelectItemsImpl(IEnumerable elements) {
			InitializePendingSelection();
			foreach (var element in SelectedItems.ToArray())
				RemoveFromSelection(element);
			foreach (var element in elements)
				AddToSelection(element);
		}
		private void InitializePendingSelection() {
			pendingSelectedItems.AddRange(selectedItems);
		}
		object GetSourceIfExists(object element) {
			if (element is NavBarItem) {
				var item = (NavBarItem)element;
				return item.SourceObject ?? item;
			}
			if (element is NavBarGroup) {
				var group = (NavBarGroup)element;
				return group.SourceObject ?? group;
			}
			return element;
		}
		NavBarGroup GetGroup(object element) {
			if (element is NavBarGroup || element==null)
				return element as NavBarGroup;
			return Owner.Groups.FirstOrDefault(x => Equals(element, GetGroupSourceObject(x)));
		}
		object GetGroupSourceObject(object element) {
			var sourceObject = (element as NavBarGroup).With(x => x.SourceObject) ?? element;
			return (sourceObject as CollectionViewGroup).If(x => !String.IsNullOrEmpty(Owner.GroupDescription)).With(x => x.Name) ?? sourceObject;
		}
		NavBarItem GetItem(object element, NavBarGroup group = null) {
			if (element is NavBarItem || element==null) return element as NavBarItem;
			if (group != null)
				return group.SynchronizedItems.Where(x => x.SourceObject == element).FirstOrDefault();
			return Owner.Groups.SelectMany(x => x.SynchronizedItems).Where(x => x.SourceObject == element).FirstOrDefault();
		}
		public void UpdateSelectionSettings() {
			changingEventLocker.DoLockedAction(() => {
				AllowSelectItems = Owner.AllowSelectItem;
				AllowSelectDisabled = Owner.AllowSelectDisabledItem;
				AllowSelectMultiple = Owner.EachGroupHasSelectedItem;
			});
		}
		protected virtual void OnAllowSelectItemsChanged() {
			if (!AllowSelectItems) {
				if (AllowSelectMultiple)
					SelectItems(DevExpress.Data.Filtering.Helpers.EmptyEnumerable<object>.Instance);
				else
					SelectItem(null, SelectedGroup);
			}
		}
		protected virtual void OnAllowSelectDisabledChanged() {
			if (AllowSelectDisabled)
				return;
			if (AllowSelectMultiple) {
				SelectItems(SelectedItems.Where(x => GetItem(x).If(i => i.IsEnabled).ReturnSuccess()).ToArray());
			} else {
				if (GetItem(SelectedItem, SelectedGroup).If(x => !x.IsEnabled).ReturnSuccess())
					SelectItem(null, SelectedGroup);
			}
		}
		protected virtual void OnAllowSelectMultipleChanged() {
			if (AllowSelectMultiple)
				SelectItems(SelectedItem == null ? new object[] { } : new object[] { SelectedItem });
			else
				SelectItem(SelectedItems.LastOrDefault());
		}
		internal void SelectItem(NavBarGroup group, int newIndex) {
			if (IsValidIndex(group, newIndex) && !indexSelectionLocker.IsLocked)
				SelectItem(newIndex == ConstantHelper.InvalidIndex ? null : group.SynchronizedItems[newIndex], group);
		}
		bool IsValidIndex(NavBarGroup group, int newIndex) {
			return newIndex >= 0 && group.SynchronizedItems.Count > newIndex || newIndex == ConstantHelper.InvalidIndex;
		}
		internal object CoerceSelection(object item) {
			var navBarItem = GetItem(item);
			var group = navBarItem.With(x => x.Group);
			if (group != null && navBarItem != null) {
				var coerced = CoerceSelection(group, navBarItem);
				return (coerced as NavBarItem).With(x => x.SourceObject) ?? coerced;
			}				
			return item;
		}
		internal object CoerceSelection(NavBarGroup navBarGroup, NavBarItem navBarItem) {
			var coercedItem = navBarItem;
			if (!AllowSelectItems)
				coercedItem =  null;
			if (!AllowSelectDisabled && navBarItem.If(x => !x.IsEnabled).ReturnSuccess())
				coercedItem =  navBarGroup.SelectedItem;
			if (!navBarGroup.SynchronizedItems.Contains(navBarItem) && navBarItem!=null)
				coercedItem =  navBarGroup.SelectedItem;
			if (!changingEventLocker.IsLocked && (navBarItem != (SelectedItem ?? SelectedItems.Where(x => x.Group == navBarGroup).FirstOrDefault())) && View !=null) {
				NavBarItemSelectingEventArgs args = new NavBarItemSelectingEventArgs(SelectedItem.With(x=>x.Group) ?? SelectedGroup, SelectedItem ?? SelectedGroup.With(x => x.SelectedItem), navBarGroup, navBarItem) { RoutedEvent = NavBarViewBase.ItemSelectingEvent };
				View.RaiseEvent(args);
				if (args.Cancel)
					coercedItem = SelectedGroup.With(x => x.SelectedItem);
			}
			return coercedItem;
		}
		internal object CoerceSelection(NavBarGroup navBarGroup, int newIndex) {
			if (!AllowSelectItems)
				return -1;
			if(!IsValidIndex(navBarGroup, newIndex))
				return navBarGroup.SelectedItemIndex;
			var item = newIndex == ConstantHelper.InvalidIndex ? null : navBarGroup.SynchronizedItems[newIndex];
			var coercedItem = CoerceSelection(navBarGroup, item);
			if (item == coercedItem)
				return newIndex;
			return navBarGroup.SelectedItemIndex;
		}
		public void UpdateSelectedItemIndexByItem(NavBarGroup group, object item) {
			if (group == null)
				return;			
			var navBarItem = GetItem(item, group);
			indexSelectionLocker.Lock();
				group.SelectedItemIndex = group.SynchronizedItems.IndexOf(navBarItem);
			indexSelectionLocker.Unlock();
		}
		public void OnGroupRemoved(NavBarGroup group) {
			if (AllowSelectMultiple) {
				SelectItems(SelectedItems.Where(x => x.Group != group).ToList());
			} else {
				if (SelectedItem.With(x => x.Group) == group)
					SelectItem(null);
			}
			if (SelectedGroup == group)
				SelectFirstVisibleGroup(true);
		}
		public void OnItemRemoved(NavBarGroup group, NavBarItem item) {
			if (AllowSelectMultiple) {
				SelectItems(SelectedItems.Where(x => x != item));
			} else {
				if (SelectedItem == item) {
					LastSelectedItem = SelectedItem;
					SelectItem(null, group);
				}
			}
		}
	}
	public class CollapsedNavPaneItemsSelectionStrategy {
		public CollapsedNavPaneItemsSelectionStrategy(NavBarControl owner) {
			Owner = owner;
		}
		NavBarControl Owner { get; set; }
		NavigationPaneView View { get { return Owner.View as NavigationPaneView; } }
		public void UnselectItem(NavBarGroup group) {
			group.CollapsedNavPaneItems.FirstOrDefault(x => x.IsSelected).Do(x => x.IsSelected = false);
			group.CollapsedNavPaneSelectedItem = null;
		}
		public void SelectItem(object obj) {
			if (obj == null)
				return;
			NavBarItem item;
			NavBarGroup group;
			if (obj is NavBarItem) {
				item = (NavBarItem)obj;
				group = Owner.Groups.FirstOrDefault(x => x.CollapsedNavPaneItems.Contains(item));
			} else {
				group = Owner.Groups.FirstOrDefault(x => x.CollapsedNavPaneItems.Any(i => i.SourceObject == obj));
				item = group == null ? null : group.CollapsedNavPaneItems.FirstOrDefault(x => x.SourceObject == obj);
			}
			if (group == null)
				return;
			if (!item.IsSelected) {
				UnselectItem(group);
				item.IsSelected = true;
			}
			if (!View.EachCollapsedGroupHasSelectedItem) {
				foreach (var g in Owner.Groups.Where(x => x != group)) {
					UnselectItem(g);
				}
			}
			group.CollapsedNavPaneSelectedItem = item.With(x => x.SourceObject) ?? item;
		}
		public void UpdateSelectedItem(NavBarGroup group) {
			var item = group.CollapsedNavPaneItems.FirstOrDefault(x => x.IsSelected);
			group.CollapsedNavPaneSelectedItem = item == null ? null : item.SourceObject ?? item;
		}
		public bool IsValidItem(object item) {
			if (item == null)
				return false;
			if (item is NavBarItem)
				return Owner.Groups.Any(x => x.CollapsedNavPaneItems.Contains(item));
			else
				return Owner.Groups.Any(x => x.CollapsedNavPaneItems.Any(i => i.SourceObject == item));
		}
	}
}
