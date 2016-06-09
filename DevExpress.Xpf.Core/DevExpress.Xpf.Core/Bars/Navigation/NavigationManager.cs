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
using System.Windows.Input;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System.Windows;
namespace DevExpress.Xpf.Bars {	
	public class NavigationManager {
		Func<object, EventArgs, bool> onEnterMenuMode;
		readonly Locker navigationLocker = new Locker();
		public bool IsNavigationLocked { get { return navigationLocker.IsLocked; } }
		protected INavigationOwner Owner { get; private set; }
		static NavigationManager() {
			FocusObserver.Initialize();
		}
		public NavigationManager(INavigationOwner owner) {
			this.Owner = owner;
			ValidateOwnerNavigationMode();
			MenuModeHelper.EnterMenuMode += (onEnterMenuMode = new Func<object, EventArgs, bool>(OnEnterMenuMode));
		}
		protected virtual bool OnEnterMenuMode(object sender, EventArgs args) {
			if (IsNavigationLocked)
				return false;
			return NavigationTree.ToggleMenuMode(Owner, true, true);
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			ProcessCore(e, true);
		}   
		public virtual void ProcessKeyUp(KeyEventArgs e) {
			ProcessCore(e, false);
		}
		void ProcessCore(KeyEventArgs e, bool isKeyDown) {
			bool result;
			var previousElement = NavigationTree.CurrentElement;
			using (NavigationTree.LockElementNotifications()) {
				var children = Owner.Elements.ToList();
				while (NavigationTree.CurrentElement != null && !children.Contains(NavigationTree.CurrentElement))
					NavigationTree.Pop(false);
				if (isKeyDown)
					result = ProcessKeyDownOverride(GetModifiers(e), GetKey(e));
				else
					result = ProcessKeyUpOverride(GetModifiers(e), GetKey(e));
			}
			if (result) {
				NavigationTree.SelectElement(NavigationTree.CurrentElement);
				e.Handled = true;
				return;
			}
			using (NavigationTree.LockElementNotifications())
				NavigationTree.SelectElement(previousElement);
		}
		protected virtual bool ProcessKeyUpOverride(ModifierKeys modifiers, Key key) {
			return false;
		}
		protected virtual bool ProcessKeyDownOverride(ModifierKeys modifiers, Key key) {			
			if (ShouldSelectContainer(key, modifiers)) {
				return SelectContainer();
			}
			if (ShouldSelectFirst(key, modifiers)) {
				return SelectFirst();
			}
			if (ShouldSelectLast(key, modifiers)) {
				return SelectLast();
			}
			if (ShouldSelectPrevious(key, modifiers)) {
				return SelectPrevious();
			}
			if (ShouldSelectNext(key, modifiers)) {
				return SelectNext();
			}
			if (ShouldSelectPreviousContainer(key, modifiers)) {
				return SelectPreviousContainer();
			}
			if (ShouldSelectNextContainer(key, modifiers)) {
				return SelectNextContainer();
			}
			return false;
		}		
		protected virtual Key GetKey(KeyEventArgs e) {
			if (GetFlowDirection() == FlowDirection.LeftToRight)
				return e.Key;
			if (e.Key == Key.Left)
				return Key.Right;
			if (e.Key == Key.Right)
				return Key.Left;
			return e.Key;
		}
		protected virtual FlowDirection GetFlowDirection() {
			var feOwner = Owner as FrameworkElement;
			if (feOwner == null)
				return FlowDirection.LeftToRight;
			return feOwner.FlowDirection;
		}
		protected virtual bool ShouldSelectContainer(Key key, ModifierKeys modifiers) {
			return key == Key.Escape;
		}		
		protected virtual bool ShouldSelectNextContainer(Key key, ModifierKeys modifiers) {
			return Owner.NavigationKeys.HasFlag(NavigationKeys.CtrlTab) && modifiers.HasFlag(ModifierKeys.Control) && !modifiers.HasFlag(ModifierKeys.Shift) && key == Key.Tab;
		}
		protected virtual bool ShouldSelectPreviousContainer(Key key, ModifierKeys modifiers) {
			return Owner.NavigationKeys.HasFlag(NavigationKeys.CtrlTab) && modifiers.HasFlag(ModifierKeys.Control) && modifiers.HasFlag(ModifierKeys.Shift) && key == Key.Tab;
		}
		protected virtual bool ShouldSelectLast(Key key, ModifierKeys modifiers) {
			return Owner.NavigationKeys.HasFlag(NavigationKeys.HomeEnd) && !modifiers.HasFlag(ModifierKeys.Control) && key == Key.End || ShouldSelectPrevious(key, modifiers) && NavigationTree.CurrentElement == Owner;
		}
		protected virtual bool ShouldSelectFirst(Key key, ModifierKeys modifiers) {
			return Owner.NavigationKeys.HasFlag(NavigationKeys.HomeEnd) && !modifiers.HasFlag(ModifierKeys.Control) && key == Key.Home || ShouldSelectNext(key, modifiers) && NavigationTree.CurrentElement == Owner;
		}
		protected virtual bool ShouldSelectNext(Key key, ModifierKeys modifiers) {
			var nextKey = (Owner.Orientation == Orientation.Horizontal) ? Key.Right : Key.Down;
			return Owner.NavigationKeys.HasFlag(NavigationKeys.Tab) && !modifiers.HasFlag(ModifierKeys.Control) && !modifiers.HasFlag(ModifierKeys.Shift) && key == Key.Tab || Owner.NavigationKeys.HasFlag(NavigationKeys.Arrows) && key == nextKey;
		}
		protected virtual bool ShouldSelectPrevious(Key key, ModifierKeys modifiers) {
			var prevKey = (Owner.Orientation == Orientation.Horizontal) ? Key.Left : Key.Up;
			return Owner.NavigationKeys.HasFlag(NavigationKeys.Tab) && !modifiers.HasFlag(ModifierKeys.Control) && modifiers.HasFlag(ModifierKeys.Shift) && key == Key.Tab || Owner.NavigationKeys.HasFlag(NavigationKeys.Arrows) && key == prevKey;
		}
		public bool SelectContainer() {
			if (IsNavigationLocked) return false;
			return SelectContainer(true);
		}
		protected virtual bool SelectContainer(bool selectableOnly) {
			return NavigationTree.SelectElement(Owner) && NavigationTree.Pop(selectableOnly);
		}
		public virtual bool SelectNextContainer() {
			if (IsNavigationLocked) return false;
			return NavigationTree.SelectElement(Owner) && NavigationTree.SelectNextContainer();
		}
		public virtual bool SelectPreviousContainer() {
			if (IsNavigationLocked) return false;
			return NavigationTree.SelectElement(Owner) && NavigationTree.SelectPreviousContainer();
		}
		public virtual bool SelectNext() {
			if (IsNavigationLocked) return false;
			return NavigationTree.SelectNext();
		}
		public virtual bool SelectPrevious() {
			if (IsNavigationLocked) return false;
			return NavigationTree.SelectPrevious();
		}
		public virtual bool SelectLast() {
			if (IsNavigationLocked) return false;
			return NavigationTree.SelectElement(Owner) && NavigationTree.SelectLast();
		}
		public virtual bool SelectFirst() {
			if (IsNavigationLocked) return false;
			return NavigationTree.SelectElement(Owner) && NavigationTree.SelectFirst();
		}		
		protected virtual ModifierKeys GetModifiers(KeyEventArgs e) {
			ModifierKeys forced = ModifierKeys.None;
#if DEBUGTEST
			forced = ModifierKeysHelper.ForcedModifiers ?? ModifierKeys.None;
#endif
			return ModifierKeysHelper.GetKeyboardModifiers(e) | forced;
		}
		protected virtual void ValidateOwnerNavigationMode() {
			bool isValid = false;
			if (Owner != null) {
				isValid |= Owner.NavigationMode == KeyboardNavigationMode.Continue
					|| Owner.NavigationMode == KeyboardNavigationMode.Cycle
					|| Owner.NavigationMode == KeyboardNavigationMode.None;
			}
			if (!isValid) {
				throw new ArgumentException();
			}
		}
		public IDisposable Lock() {
			return navigationLocker.Lock();
		}
		public void Unlock() {
			navigationLocker.Unlock();
		}
	}
	public class PopupMenuNavigationManager : NavigationManager {
		SubMenuBarControl BarControl { get { return Owner as SubMenuBarControl; } }
		PopupMenuBase Popup { get { return BarControl.Popup; } }
		bool IsOpen { get { return Popup.Return(x => x.IsOpen, () => false); } }
		bool IsNestedPopup { get { return Popup.OwnerLinkControl.With(x => x.LinksControl as SubMenuBarControl) != null; } }
		public PopupMenuNavigationManager(SubMenuBarControl bc) : base(bc) { }
		protected override bool ShouldSelectContainer(Key key, ModifierKeys modifiers) {
			if (IsNavigationLocked) return false;
			return IsOpen && base.ShouldSelectContainer(key, modifiers) || key == Key.Left && IsNestedPopup;
		}
		protected override bool ShouldSelectFirst(Key key, ModifierKeys modifiers) {
			return ShouldSelectNext(key, modifiers) && SelectFirstOrLast() || base.ShouldSelectFirst(key, modifiers);
		}
		protected override bool ShouldSelectLast(Key key, ModifierKeys modifiers) {
			return ShouldSelectPrevious(key, modifiers) && SelectFirstOrLast() || base.ShouldSelectLast(key, modifiers);
		}
		bool SelectFirstOrLast() {
			return PopupMenuManager.TopPopup == Popup && NavigationTree.CurrentElement == null;
		}
		protected override bool ProcessKeyDownOverride(ModifierKeys modifiers, Key key) {
			if (base.ProcessKeyDownOverride(modifiers, key))
				return true;
			if (ShouldSelectNextMenu(key, modifiers)) {
				return SelectNextMenu();
			}
			if (ShouldSelectPreviousMenu(key, modifiers)) {
				return SelectPreviousMenu();
			}
			return false;
		}
		protected virtual bool ShouldSelectPreviousMenu(Key key, ModifierKeys modifiers) {
			return key == Key.Left && !IsNestedPopup && IsOpen;
		}
		protected virtual bool ShouldSelectNextMenu(Key key, ModifierKeys modifiers) {
			return key == Key.Right && IsOpen;
		}
		protected virtual bool SelectPreviousMenu() {
			if (IsNavigationLocked) return false;
			return SelectMenu(true);
		}
		protected virtual bool SelectNextMenu() {
			if (IsNavigationLocked) return false;
			return SelectMenu(false);
		}
		bool SelectMenu(bool previous) {
			if (NavigationTree.CurrentElement == null) {
				NavigationTree.SelectElement(Popup.ContentControl);
			}
			if (!NavigationTree.SelectParent(x => x.Parent is BarControl))
				return false;
			if (previous)
				NavigationTree.SelectPrevious();
			else
				NavigationTree.SelectNext();
			IPopupOwner pOwner = GetPopupOwner(NavigationTree.CurrentElement);
			if (pOwner == null)
				return true;
			pOwner.ShowPopup();
			var bound = (NavigationTree.CurrentElement as INavigationElement).With(x => x.BoundOwner);
			bound.Do(x => NavigationTree.SelectElement(x));
			return true;
		}
		protected virtual IPopupOwner GetPopupOwner(IBarsNavigationSupport bcElement) {
			var li = bcElement as BarItemLinkInfo;
			if (li == null)
				return null;
			return li.LinkControl as IPopupOwner;
		}
		protected override bool ShouldSelectNextContainer(Key key, ModifierKeys modifiers) {
			return IsOpen && base.ShouldSelectNextContainer(key, modifiers);
		}
		protected override bool ShouldSelectPreviousContainer(Key key, ModifierKeys modifiers) {
			return IsOpen && base.ShouldSelectPreviousContainer(key, modifiers);
		}
	}
	public class GalleryControlNavigationManager : NavigationManager {
		public GalleryControlNavigationManager(INavigationOwner owner) : base(owner) {
		}
		protected override bool ProcessKeyDownOverride(ModifierKeys modifiers, Key key) {
			if (ShouldSelectRight(key, modifiers)) {
				return SelectRight();
			}
			if (ShouldSelectLeft(key, modifiers)) {
				return SelectLeft();
			}
			if (ShouldSelectUp(key, modifiers)) {
				return SelectUp();
			}
			if (ShouldSelectDown(key, modifiers)) {
				return SelectDown();
			}
			if (ShouldSelectNextGroup(key, modifiers)) {
				return SelectNextGroup();
			}
			if (ShouldSelectPreviousGroup(key, modifiers)) {
				return SelectPreviousGroup();
			}
			return base.ProcessKeyDownOverride(modifiers, key);
		}
		protected virtual bool ShouldSelectPreviousGroup(Key key, ModifierKeys modifiers) { return base.ShouldSelectPreviousContainer(key, modifiers); }
		protected virtual bool ShouldSelectNextGroup(Key key, ModifierKeys modifiers) { return base.ShouldSelectNextContainer(key, modifiers); }
		protected virtual bool ShouldSelectDown(Key key, ModifierKeys modifiers) { return key == Key.Down; }
		protected virtual bool ShouldSelectUp(Key key, ModifierKeys modifiers) { return key == Key.Up; }
		protected virtual bool ShouldSelectLeft(Key key, ModifierKeys modifiers) { return key == Key.Left; }
		protected virtual bool ShouldSelectRight(Key key, ModifierKeys modifiers) { return key == Key.Right; }
		protected virtual bool SelectPreviousGroup() {
			if (SelectFirstIfNothingSelected())
				return true;
			return SelectInGroups(-1);
		}
		protected virtual bool SelectNextGroup() {
			if (SelectFirstIfNothingSelected())
				return true;
			return SelectInGroups(1);
		}
		protected virtual bool SelectDown() {
			if (SelectFirstIfNothingSelected())
				return true;
			return SelectInColumn(1);
		}
		protected virtual bool SelectUp() {
			if (SelectFirstIfNothingSelected())
				return true;
			return SelectInColumn(-1);
		}
		protected virtual bool SelectLeft() {
			if (SelectFirstIfNothingSelected())
				return true;
			return SelectInRow(-1);
		}
		protected virtual bool SelectRight() {
			if (SelectFirstIfNothingSelected())
				return true;
			return SelectInRow(1);
		}
		bool SelectInGroups(int offset) {
			var items = Items.GroupBy(x => x.GroupControl).OrderBy(x => x.Key.Group.Gallery.Groups.IndexOf(x.Key.Group)).ToList();
			var currentIndex = items.IndexOf(items.First(x => x.Key == CurrentItem.GroupControl));
			var count = items.Count();
			int newIndex = GetNewIndex(offset, currentIndex, count);
			if (!items.IsValidIndex(newIndex))
				return false;
			return NavigationTree.SelectElement(items[newIndex].FirstOrDefault());
		}
		int GetNewIndex(int offset, int currentIndex, int count) {
			if (((GalleryControl)Owner).AllowCyclicNavigation)
				return (currentIndex + offset + count) % count;
			return currentIndex + offset;
		}
		bool SelectInRow(int offset) {
			var items = Items.Where(x => x.GroupControl == CurrentItem.GroupControl).Where(x => x.DesiredRowIndex == CurrentItem.DesiredRowIndex).OrderBy(x => x.DesiredColIndex).ToList();
			var currentIndex = items.IndexOf(CurrentItem);
			var count = items.Count();
			var newIndex = GetNewIndex(offset, currentIndex, count);
			if (!items.IsValidIndex(newIndex))
				return false;
			return NavigationTree.SelectElement(items[newIndex]);
		}
		bool SelectInColumn(int offset) {
			var items = Items.Where(x => x.DesiredColIndex == CurrentItem.DesiredColIndex).OrderBy(x => x.Gallery.Groups.IndexOf(x.GroupControl.Group)).ThenBy(x => x.DesiredRowIndex).ToList();
			var currentIndex = items.IndexOf(CurrentItem);
			var count = items.Count();
			var newIndex = GetNewIndex(offset, currentIndex, count);
			if (!items.IsValidIndex(newIndex))
				return false;
			return NavigationTree.SelectElement(items[newIndex]);
		}
		bool SelectFirstIfNothingSelected() {
			if (CurrentItem == null)
				return SelectFirst();
			return false;
		}
		GalleryItemControl CurrentItem { get { return NavigationTree.CurrentElement as GalleryItemControl; } }
		IEnumerable<GalleryItemControl> Items { get { return Owner.Elements.OfType<GalleryItemControl>(); } }
		protected override bool ShouldSelectFirst(Key key, ModifierKeys modifiers) { return base.ShouldSelectFirst(key, modifiers); }
		protected override bool ShouldSelectLast(Key key, ModifierKeys modifiers) { return base.ShouldSelectLast(key, modifiers); }
		protected override bool ShouldSelectNext(Key key, ModifierKeys modifiers) { return base.ShouldSelectNext(key, modifiers); }
		protected override bool ShouldSelectPrevious(Key key, ModifierKeys modifiers) { return base.ShouldSelectPrevious(key, modifiers); }
		protected override bool ShouldSelectNextContainer(Key key, ModifierKeys modifiers) { return false; }
		protected override bool ShouldSelectPreviousContainer(Key key, ModifierKeys modifiers) { return false; }
		protected override bool ShouldSelectContainer(Key key, ModifierKeys modifiers) { return false; }
	}
	public class ScopeNavigationOwner : INavigationOwner {
		static Dictionary<DependencyObject, ScopeNavigationOwner> owners = new Dictionary<DependencyObject, ScopeNavigationOwner>();
		FrameworkElement Target { get; set; }
		BarNameScope Scope { get; set; }
		NavigationManager NavigationManager { get; set; }
		ScopeNavigationOwner(DependencyObject node) {
			Target = (Scope = node.With(BarNameScope.GetScope)).With(x => x.Target) as FrameworkElement;
			NavigationManager = new NavigationManager(this);
		}
		static ScopeNavigationOwner From(DependencyObject element) {
			var st = element.With(BarNameScope.FindScopeTarget);
			if (st != null) {
				ScopeNavigationOwner result;
				if (!owners.TryGetValue(st, out result)) {
					result = new ScopeNavigationOwner(st);
					owners.Add(st, result);
				}
				return result;
			}
			return new ScopeNavigationOwner(null);
		}
		public static INavigationOwner GetOwner(BarControl bc) {
			return From(bc);
		}
		public static void Destroy() {
			owners.Clear();
		}
		void AddTargetHandler(RoutedEvent rEvent, Delegate value) {
			if (Target == null)
				return;
			Target.AddHandler(rEvent, value);			
		}
		void RemoveTargetHandler(RoutedEvent rEvent, Delegate value) {
			if (Target == null)
				return;
			Target.RemoveHandler(rEvent, value);
		}
		IList<INavigationElement> GetElements() {
			if (Target == null)
				return new List<INavigationElement>();
			var elements = Scope.GetService<IElementRegistratorService>().GetElements<IFrameworkInputElement>();
			var unorderedElements = elements.OfType<Bar>().Select(x => x.DockInfo.BarControl).Where(x => x != null).Select(x => new GlobalNavigationElementWrapper(x, Target)).ToList();
			GlobalNavigationElementWrapperGroup.Group(unorderedElements);
			var orderedElements = unorderedElements.OrderBy(x => x.GroupBounds.Top).ThenBy(x => x.GroupBounds.Left).ThenBy(x => x.Bounds.Top).ThenBy(x => x.Bounds.Left);
			return orderedElements.Select(x => x.NavigationElement).OfType<INavigationElement>().ToList();
		}
		class GlobalNavigationElementWrapperGroup {
			List<GlobalNavigationElementWrapper> Elements { get; set; }
			public Rect Bounds {
				get {
					Rect result = Rect.Empty;
					foreach (var elem in Elements) {
						result.Union(elem.Bounds);
					}
					return result;
				}
			}
			public GlobalNavigationElementWrapperGroup() {
				Elements = new List<GlobalNavigationElementWrapper>();
			}
			bool CanJoin(GlobalNavigationElementWrapper element) {
				if (Elements.Count == 0)
					return true;
				var orientation = Elements[0].Orientation;
				if (orientation != element.Orientation)
					return false;
				foreach (var inner in Elements) {
					Rect extBounds = ExtendBounds(inner.Bounds, orientation);
					if (extBounds.IntersectsWith(element.Bounds))
						return true;
				}
				return false;
			}
			Rect ExtendBounds(Rect bounds, Orientation orientation) {
				var xCoef = orientation == Orientation.Horizontal ? 0 : 10;
				var yCoef = 10;
				return new Rect(bounds.X - xCoef, bounds.Y - yCoef, bounds.Width + xCoef * 2, bounds.Height + yCoef * 2);
			}
			public static IEnumerable<GlobalNavigationElementWrapperGroup> Group(IEnumerable<GlobalNavigationElementWrapper> wrappers) {
				List<GlobalNavigationElementWrapperGroup> elems = new List<GlobalNavigationElementWrapperGroup>();
				foreach (var wrapper in wrappers) {
					var elem = (elems.FirstOrDefault(x => x.CanJoin(wrapper)) ?? new GlobalNavigationElementWrapperGroup()).Do(x => x.Join(wrapper));
					if (elems.Contains(elem))
						continue;
					elems.Add(elem);
				}
				return elems;
			}
			protected virtual void Join(GlobalNavigationElementWrapper wrapper) {
				Elements.Add(wrapper);
				wrapper.Group = this;
			}
		}
		class GlobalNavigationElementWrapper {
			public BarControl NavigationElement { get; set; }
			public Rect Bounds { get; set; }
			public Rect GroupBounds { get { return Group.Bounds; } }
			public Orientation Orientation { get; set; }
			public GlobalNavigationElementWrapperGroup Group { get; set; }
			public GlobalNavigationElementWrapper(BarControl x, FrameworkElement target) {
				NavigationElement = x;
				Bounds = x.GetBounds(target);
				Orientation = x.ContainerOrientation;
			}
		}
		NavigationManager GetNavigationManager() {
			return NavigationManager;
		}
		int GetID() {
			if (Target == null)
				return GetHashCode();
			return Target.GetHashCode();
		}
		INavigationElement INavigationOwner.BoundElement { get { return null; } }
		bool INavigationOwner.CanEnterMenuMode { get { return false; } }
		IList<INavigationElement> INavigationOwner.Elements { get { return GetElements(); } }
		bool IBarsNavigationSupport.ExitNavigationOnFocusChangedWithin { get { return true; } }
		bool IBarsNavigationSupport.ExitNavigationOnMouseUp { get { return true; } }
		int IBarsNavigationSupport.ID { get { return GetID(); } }
		bool IBarsNavigationSupport.IsSelectable { get { return false; } }
		NavigationKeys INavigationOwner.NavigationKeys { get { return NavigationKeys.CtrlTab; } }
		NavigationManager INavigationOwner.NavigationManager { get { return GetNavigationManager(); } }
		KeyboardNavigationMode INavigationOwner.NavigationMode { get { return KeyboardNavigationMode.Cycle; } }
		Orientation INavigationOwner.Orientation { get { return Orientation.Horizontal; } }
		IBarsNavigationSupport IBarsNavigationSupport.Parent { get { return null; } }
		event KeyEventHandler IBarsNavigationSupport.KeyDown { add { AddTargetHandler(Keyboard.KeyDownEvent, value); } remove { RemoveTargetHandler(Keyboard.KeyDownEvent, value); } }
		event KeyEventHandler IBarsNavigationSupport.KeyUp { add { AddTargetHandler(Keyboard.KeyUpEvent, value); } remove { RemoveTargetHandler(Keyboard.KeyUpEvent, value); } }
		void INavigationOwner.OnAddedToSelection() { }
		void INavigationOwner.OnRemovedFromSelection(bool destroying) { }
	}
}
