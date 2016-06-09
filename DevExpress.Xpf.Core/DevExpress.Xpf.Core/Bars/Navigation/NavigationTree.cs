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
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Native;
using System.Windows;
using System.Windows.Threading;
namespace DevExpress.Xpf.Bars {
	[Flags]
	public enum NavigationKeys {
		None = 0x0,
		Arrows = 0x1,
		Tab = 0x2,
		HomeEnd = 0x4,
		CtrlTab = 0x8,
		All = Arrows | Tab | HomeEnd | CtrlTab
	}
	public class NavigationTree : DispatcherObject {
		static NavigationTree() {
			FocusObserver.Initialize();
			EventManager.RegisterClassHandler(typeof(FrameworkElement), Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp));
			FocusObserver.FocusChanged += OnFocusedElementChanged;
		}
		[ThreadStatic]
		static NavigationTree instance;
		[ThreadStatic]
		static bool isMenuMode;
		public static bool IsMenuMode {
			get { return isMenuMode; } private set { isMenuMode = value; }
		}
		protected static NavigationTree Instance { get { return instance; } }
		static NavigationTree UnlockedInstance { get { return Instance.If(x => !globalLocker.IsLocked); } }
		public static IBarsNavigationSupport CurrentElement {
			get { return Instance.With(x => x.Current).With(x => x.Element); }
		}
		public NavigationTreeNode Root { get; protected internal set; }
		NavigationTreeNode current;
		PostponedAction processOnFocusedElementChangedPostponedAction = new PostponedAction(() => true);
		readonly Locker selectLocker = new Locker();
		readonly Locker globalSelectLocker = new Locker();
		readonly Locker disableMouseEventsProcessingLocker = new Locker();
		static readonly Locker globalLocker = new Locker();
		public NavigationTreeNode Current {
			get { return current; }
			protected set {
				if (value == current) return;
				NavigationTreeNode oldValue = current;
				current = value;
				OnCurrentChanged(oldValue);
			}
		}
		public bool EnableKeyboardCues { get; private set; }
		protected void OnCurrentChanged(NavigationTreeNode oldValue) {
			globalSelectLocker.DoIfNotLocked(() => OnCurrentChangedLocked(oldValue));
		}
		protected virtual void OnCurrentChangedLocked(NavigationTreeNode oldValue) {
			List<NavigationTreeNode> rootPath = new List<NavigationTreeNode>();
			var curr = Current;
			if (oldValue == null) FocusObserver.SaveFocus(!CheckFocusedElementInsideNavigationElement());
			while (curr != null) {
				rootPath.Add(curr);
				curr = curr.Parent;
			}
			ForEach(x => {
				if (rootPath.Contains(x)) return;
				x.Deselect();
			}, true);
			selectLocker.DoIfNotLocked(() => {
				Current.Do(x => x.Select());
			});
			if (Current == null)
				ExitMenuMode(false);
		}
		static bool CheckFocusedElementInsideNavigationElement() {
			return (Keyboard.FocusedElement as FrameworkElement).With(x => LayoutHelper.FindLayoutOrVisualParentObject(x, fe => fe is INavigationElement)).ReturnSuccess();
		}
		static void OnMouseUp(object sender, MouseButtonEventArgs e) {
			if (UnlockedInstance == null)
				return;
			UnlockedInstance.ProcessOnMouseUp(sender, e);
		}
		static void OnFocusedElementChanged(object sender, ValueChangedEventArgs<IInputElement> e) {
			if (UnlockedInstance == null)
				return;
			UnlockedInstance.ProcessOnFocusedElementChanged(sender, e);
		}
		public static bool ToggleMenuMode(IBarsNavigationSupport element, bool enableKeyboardCues, bool checkCanEnterMenuMode) {
			if (IsMenuMode || PopupMenuManager.IsAnyPopupOpened) {
				ExitMenuMode(false);
				PopupMenuManager.CloseAllPopups();
				return true;
			} else {
				return EnterMenuMode(element, enableKeyboardCues, checkCanEnterMenuMode);
			}
		}
		public static bool EnterMenuMode(IBarsNavigationSupport element, bool enableKeyboardCues, bool checkCanEnterMenuMode) {
			if (globalLocker.IsLocked)
				return false;
			if (BarManagerCustomizationHelper.IsInCustomizationMode(element))
				return false;
			if (checkCanEnterMenuMode && (!(element is INavigationOwner) || !(((INavigationOwner)element).CanEnterMenuMode))) {
				return false;
			}
			EnsureTree(element);
			Instance.EnableKeyboardCues = enableKeyboardCues;
			SelectElement(element);
			IsMenuMode = true;
			CancelExitingMenuMode();
			return true;
		}		
		public static void StartNavigation(IBarsNavigationSupport element) {			
			if (globalLocker.IsLocked)
				return;
			try {
				if (BarManagerCustomizationHelper.IsInCustomizationMode(element))
					return;
				if (CurrentElement == element)
					return;
				if (EnsureTree(element))
					return;
				SelectElement(element);
			} finally {
				CancelExitingMenuMode();
			}			
		}
		public static void ExitMenuMode() {
			ExitMenuMode(true);
		}
		public static void ExitMenuMode(bool restoreFocusAsync) {
			ExitMenuMode(true, restoreFocusAsync);
		}
		public static void ExitMenuMode(bool restoreFocus, bool asyncRestore) {
			if (globalLocker.IsLocked)
				return;
			Instance.Do(x => x.Destroy());
			instance = null;
			ScopeNavigationOwner.Destroy();
			IsMenuMode = false;
			if (restoreFocus) {
				FocusObserver.RestoreFocus(asyncRestore, () => (Keyboard.FocusedElement as DependencyObject).With(x => TreeHelper.GetParent<IBarsNavigationSupport>(x, ns => true)) != null);
			}			
		}
		public static bool SelectElement(IBarsNavigationSupport element) { return UnlockedInstance.Return(x => x.ProcessSelectElement(element), () => false); }
		public static bool SelectParent(Func<IBarsNavigationSupport, bool> predicate) { return UnlockedInstance.Return(x => x.ProcessSelectParent(predicate), () => false); }
		public static bool SelectChild(Func<IBarsNavigationSupport, bool> predicate) { return UnlockedInstance.Return(x => x.ProcessSelectChild(predicate), () => false); }
		public static bool Pop(bool selectableOnly) { return UnlockedInstance.Return(x => x.ProcessPop(selectableOnly), () => false); }
		public static bool SelectNext() { return UnlockedInstance.Return(x => x.ProcessSelectNext(), () => false); }
		public static bool SelectPrevious() { return UnlockedInstance.Return(x => x.ProcessSelectPrevious(), () => false); }
		public static bool SelectNextContainer() { return UnlockedInstance.Return(x => x.ProcessSelectNextContainer(), () => false); }
		public static bool SelectPreviousContainer() { return UnlockedInstance.Return(x => x.ProcessSelectPreviousContainer(), () => false); }
		public static bool SelectLast() { return UnlockedInstance.Return(x => x.ProcessSelectLast(), () => false); }
		public static bool SelectFirst() { return UnlockedInstance.Return(x => x.ProcessSelectFirst(), () => false); }
		public static IDisposable DisableMouseEventsProcessing() { return UnlockedInstance.With(x => x.disableMouseEventsProcessingLocker.Lock()); }
		public static IDisposable LockElementNotifications() { return UnlockedInstance.With(x => x.globalSelectLocker.Lock()); ; }
		public static IDisposable Lock() { return globalLocker.Lock(); }
		public static void EnableMouseEventsProcessing() { UnlockedInstance.Do(x => x.disableMouseEventsProcessingLocker.Unlock()); }
		public static void UnlockElementNotifications() { UnlockedInstance.Do(x => x.globalSelectLocker.Unlock()); ; }
		public static void Unlock() { globalLocker.Unlock(); }
		static bool EnsureTree(IBarsNavigationSupport element) {
			try {
				IBarsNavigationSupport root = element;
				while (true) {
					var parent = root.Parent;
					if (parent == null)
						break;
					root = parent;
				}
				if (Instance == null || Instance.Root.Element != root) {
					Instance.Do(x => x.Destroy());
					instance = new NavigationTree(root);
					return false;
				}
				return true;
			} finally {
				Instance.FindNode(element);
			}
		}
		NavigationTree(IBarsNavigationSupport root) {
			Root = new NavigationTreeNode(this, null, root);
		}
		public bool ProcessSelectElement(IBarsNavigationSupport element) {
			var value = FindNode(element);
			if (value == null && element != null)
				return false;
			if (Current == value)
				OnCurrentChanged(Current);
			else
				Current = value;
			return true;
		}
		public void Select(NavigationTreeNode element) {
			Current = element;
		}
		NavigationTreeNode FindNode(IBarsNavigationSupport element) {
			if (Current.With(x => x.Element) == element)
				return Current;
			if (Root.With(x => x.Element) == element)
				return Root;
			NavigationTreeNode node;
			if (Current != null && Current.TryFindNode(x => x == element, out node, false)) {
				return node;
			}
			Root.TryFindNode(x => x == element, out node, true);
			return node;
		}
		void Destroy() {
			using (selectLocker.Lock()) {
				Root.Destroy();
				ForEach(x => x.Destroy(), true);
			}
			Root = null;
		}
		bool ProcessPop(bool selectableOnly) {
			var current = Current;
			var oldValue = current;
			current = current.With(x => x.Parent);
			if (selectableOnly) {
				while (current != null && !current.Element.IsSelectable) {
					current = current.Parent;
				}
			}
			bool shouldlock = current != null && !current.Element.IsSelectable;
			if (shouldlock) {
				selectLocker.Lock();
			}
			Current = current;
			if (shouldlock) {
				selectLocker.Unlock();
			}
			return Current != oldValue;
		}
		bool ProcessSelectNext() { return Current.Return(x => x.SelectNext(), () => false); }
		bool ProcessSelectPrevious() { return Current.Return(x => x.SelectPrevious(), () => false); }
		bool ProcessSelectNextContainer() { return Current.With(x => x.Parent).Return(x => x.SelectNext(), () => false); }
		bool ProcessSelectPreviousContainer() { return Current.With(x => x.Parent).Return(x => x.SelectPrevious(), () => false); }
		bool ProcessSelectLast() { return Current.Return(x => x.SelectLast(), () => false); }
		bool ProcessSelectFirst() { return Current.Return(x => x.SelectFirst(), () => false); }
		bool ProcessSelectParent(Func<IBarsNavigationSupport, bool> predicate) {
			var current = Current;
			while (current != null) {
				if (predicate(current.Element)) {
					Select(current);
					return true;
				}
				current = current.Parent;
			}
			return false;
		}
		bool ProcessSelectChild(Func<IBarsNavigationSupport, bool> predicate) {
			var current = Current ?? Root;
			if (current == null)
				return false;
			NavigationTreeNode node;
			if (current.TryFindNode(predicate, out node, true)) {
				Select(node);
				return true;
			}
			return false;
		}
		void ForEach(Action<NavigationTreeNode> action, bool expandedNodesOnly) {
			if (Root == null)
				return;
			action(Root);
			Root.ForEach(action, expandedNodesOnly);
		}
		void ProcessOnMouseUp(object sender, MouseButtonEventArgs e) {
			if (disableMouseEventsProcessingLocker.IsLocked)
				return;
			var feSource = e.OriginalSource as DependencyObject;
			var element = TreeHelper.GetParent<IBarsNavigationSupport>(feSource, fe => !fe.ExitNavigationOnMouseUp, logicalTreeFirst: true)
				?? TreeHelper.GetParent<IBarsNavigationSupport>(feSource, fe => !fe.ExitNavigationOnMouseUp, logicalTreeFirst: false);
			if (element == null && feSource.With(PresentationSource.FromDependencyObject) != null)
				ExitMenuMode();
		}
		void ProcessOnFocusedElementChanged(object sender, ValueChangedEventArgs<IInputElement> e) {
			processOnFocusedElementChangedPostponedAction.PerformPostpone(() => {
				var args = e;
				var newElement = TreeHelper.GetParent<IBarsNavigationSupport>(Keyboard.FocusedElement as DependencyObject, x => !x.ExitNavigationOnFocusChangedWithin, true, true)
				?? TreeHelper.GetParent<IBarsNavigationSupport>(Keyboard.FocusedElement as DependencyObject, x => !x.ExitNavigationOnFocusChangedWithin, true, false);
				if (newElement == null) {
					FocusObserver.Reset();
					ExitMenuMode();
				}
			});
			Dispatcher.BeginInvoke(new Action(() => { processOnFocusedElementChangedPostponedAction.PerformForce(); }));
		}
		static void CancelExitingMenuMode() {
			Instance.Do(x => { x.processOnFocusedElementChangedPostponedAction.PerformPostpone(null); FocusObserver.CancelRestore(); });
		}
	}
	public class NavigationTreeNode {
		readonly NavigationTree tree;
		readonly NavigationTreeNode parent;
		readonly IBarsNavigationSupport element;
		readonly NavigationTreeNodeStrategy strategy;
		public NavigationTree Tree { get { return tree; } }
		public NavigationTreeNode Parent { get { return parent; } }
		public IBarsNavigationSupport Element { get { return element; } }
		IList<NavigationTreeNode> Children { get { return strategy.Children; } }
		public NavigationTreeNode(NavigationTree tree, NavigationTreeNode parent, IBarsNavigationSupport element) {
			this.tree = tree;
			this.parent = parent;
			this.element = element;
			this.strategy = CreateStrategy(element);
		}
		NavigationTreeNodeStrategy CreateStrategy(IBarsNavigationSupport element) {
			if (element is INavigationOwner && element is INavigationElement)
				return new NavigationTreeNodeMergedStrategy(element, this);
			if (element is INavigationOwner)
				return new NavigationTreeNodeOwnerStrategy((INavigationOwner)element, this);
			if (element is INavigationElement)
				return new NavigationTreeNodeElementStrategy((INavigationElement)element, this);
			throw new ArgumentException("element");
		}
		public void Destroy() {
			Deselect(true);
			strategy.Destroy();
		}
		public bool TryFindNode(Func<IBarsNavigationSupport, bool> predicate, out NavigationTreeNode node, bool enterChildren) {
			return strategy.TryFindNode(predicate, out node, enterChildren);
		}
		public void Deselect() { Deselect(false); }
		protected void Deselect(bool destroying) { strategy.Deselect(destroying); }
		public void Select() { strategy.Select(); }
		public bool SelectNext() { return strategy.SelectNext(); }
		public bool SelectPrevious() { return strategy.SelectPrevious(); }
		public bool SelectLast() { return strategy.SelectLast(); }
		public bool SelectFirst() { return strategy.SelectFirst(); }
		public void ForEach(Action<NavigationTreeNode> action, bool expandedNodesOnly) {
			strategy.ForEach(action, expandedNodesOnly);
		}
		public void ProcessKeyDown(object sender, KeyEventArgs e) {
			strategy.ProcessKeyDown(sender, e);
		}
		public void ResetChildren() {
			strategy.ResetChildren();
		}
	}
	public abstract class NavigationTreeNodeStrategy {
		static List<int> nodeSearchIDs = new List<int>();
		readonly IBarsNavigationSupport element;
		readonly NavigationTreeNode node;
		protected IBarsNavigationSupport Element { get { return element; } }
		protected NavigationTreeNode Node { get { return node; } }
		protected internal bool ShouldProcessKeyDown { get; set; }
		public NavigationTreeNodeStrategy(IBarsNavigationSupport element, NavigationTreeNode node) {
			this.element = element;
			this.node = node;
			this.ShouldProcessKeyDown = true;
			Element.KeyDown += ProcessKeyDown;
			Element.KeyUp += ProcessKeyUp;
		}
		public abstract void ProcessKeyDown(object sender, KeyEventArgs e);
		public abstract void ProcessKeyUp(object sender, KeyEventArgs e);
		public abstract IList<NavigationTreeNode> Children { get; }
		public virtual bool TryFindNode(Func<IBarsNavigationSupport, bool> predicate, out NavigationTreeNode node, bool enterChildren) {
			if (nodeSearchIDs.Contains(Element.ID)) {
				node = null;
				return false;
			}
			try {
				nodeSearchIDs.Add(Element.ID);
				foreach (var child in Children) {
					if (predicate(child.Element)) {
						node = child;
						return true;
					}
					if (enterChildren && child.TryFindNode(predicate, out node, true)) {
						return true;
					}
				}
				node = null;
				return false;
			} finally {
				nodeSearchIDs.Remove(Element.ID);
			}
		}
		public abstract void ForEach(Action<NavigationTreeNode> action, bool expandedNodesOnly);
		public abstract void Deselect(bool destroying);
		public abstract void Select();
		public virtual void Destroy() {
			Element.KeyDown -= ProcessKeyDown;
			Element.KeyDown -= ProcessKeyUp;
		}
		public abstract bool SelectNext();
		public abstract bool SelectPrevious();
		public abstract bool SelectLast();
		public abstract bool SelectFirst();
		public abstract void ResetChildren();
	}
	public class NavigationTreeNodeOwnerStrategy : NavigationTreeNodeStrategy {
		IList<NavigationTreeNode> children;
		new INavigationOwner Element { get { return base.Element as INavigationOwner; } }
		public override IList<NavigationTreeNode> Children { get { return children ?? (children = InitializeChildren()); } }
		public NavigationTreeNodeOwnerStrategy(INavigationOwner element, NavigationTreeNode node) : base(element, node) {
			if (element is IMutableNavigationOwner) {
				((IMutableNavigationOwner)element).Changed += OnChanged;
			}
		}
		protected virtual IList<NavigationTreeNode> InitializeChildren() {
			List<NavigationTreeNode> result = new List<NavigationTreeNode>();
			foreach (var child in Element.Elements) {
				result.Add(new NavigationTreeNode(Node.Tree, Node, child));
			}
			return result.AsReadOnly();
		}
		public override void Destroy() {
			if (Element is IMutableNavigationOwner) {
				((IMutableNavigationOwner)Element).Changed -= OnChanged;
			}
			base.Destroy();
		}
		protected void OnChanged(object sender, EventArgs e) {
			ResetChildren();
		}
		bool isSelected = false;
		public override void ResetChildren() {
			var currentElement = NavigationTree.CurrentElement;
			using (NavigationTree.Lock()) {
				ForEach(x => x.Destroy(), true);
				children = null;
			}
			NavigationTree.SelectElement(currentElement);
		}
		public override void Deselect(bool destroying) {
			if (!isSelected)
				return;
			isSelected = false;
			MenuModeHelper.EnableKeyboardCues((DependencyObject)Element, false);
			Element.OnRemovedFromSelection(destroying);
		}
		public override void Select() {
			if (isSelected)
				return;
			isSelected = true;
			if (Node.Tree.EnableKeyboardCues)
				MenuModeHelper.EnableKeyboardCues((DependencyObject)Element, true);
			Element.OnAddedToSelection();
			SelectByIndex(0);
		}
		public override void ProcessKeyDown(object sender, KeyEventArgs e) {
			if (Element.NavigationManager == null)
				return;
			Element.NavigationManager.ProcessKeyDown(e);
		}
		public override void ProcessKeyUp(object sender, KeyEventArgs e) {
			if (Element.NavigationManager == null)
				return;
			Element.NavigationManager.ProcessKeyUp(e);
		}
		bool GetCurrentIndexInChildrenCollection(out int index) {
			index = -1;
			var current = Node.Tree.Current;
			if (current.Parent != Node)
				return false;
			index = Children.IndexOf(current);
			if (index == -1)
				return false;
			return true;
		}
		public override bool SelectNext() {
			int index;
			if (!GetCurrentIndexInChildrenCollection(out index))
				return false;
			return SelectByIndex(index + 1);
		}
		public override bool SelectPrevious() {
			int index;
			if (!GetCurrentIndexInChildrenCollection(out index))
				return false;
			return SelectByIndex(index - 1);
		}
		public override bool SelectLast() {
			if (Children.Count == 0)
				return false;
			return SelectByIndex(Children.Count - 1);
		}
		public override bool SelectFirst() {
			if (Children.Count == 0)
				return false;
			return SelectByIndex(0);
		}
		bool SelectByIndex(int index) {
			if (Element.NavigationManager != null && Element.NavigationManager.IsNavigationLocked)
				return false;
			if (Children.IsValidIndex(index)) {
				Node.Tree.Select(Children[index]);
				return true;
			}
			if (index == 0)
				return false;
			switch (Element.NavigationMode) {
				case KeyboardNavigationMode.Continue:
					return NavigationTree.Pop(false) && (index < 0 ? NavigationTree.SelectPrevious() : NavigationTree.SelectNext());
				case KeyboardNavigationMode.Cycle:
					return index < 0 ? SelectLast() : SelectFirst();
			}
			return false;
		}
		public override void ForEach(Action<NavigationTreeNode> action, bool expandedNodesOnly) {
			if (expandedNodesOnly && children == null) return;
			foreach (var child in Children) {
				action(child);
				child.ForEach(action, expandedNodesOnly);
			}
		}
	}
	public class NavigationTreeNodeElementStrategy : NavigationTreeNodeStrategy {
		IList<NavigationTreeNode> children;
		new INavigationElement Element { get { return base.Element as INavigationElement; } }
		public override IList<NavigationTreeNode> Children { get { return children ?? (children = GetChildren()); } }
		private IList<NavigationTreeNode> GetChildren() {
			List<NavigationTreeNode> children = new List<NavigationTreeNode>();
			if (Element.BoundOwner != null)
				children.Add(new NavigationTreeNode(Node.Tree, Node, Element.BoundOwner));
			return children.AsReadOnly();
		}
		public NavigationTreeNodeElementStrategy(INavigationElement element, NavigationTreeNode node) : base(element, node) {
			if (element is IMutableNavigationElement) {
				((IMutableNavigationElement)element).Changed += OnElementChanged;
			}
		}
		public override void Destroy() {
			if (Element is IMutableNavigationElement) {
				((IMutableNavigationElement)Element).Changed -= OnElementChanged;
			}
			base.Destroy();
		}
		public override void ResetChildren() {
			var currentElement = NavigationTree.CurrentElement;
			using (NavigationTree.Lock()) {
				ForEach(x => x.Destroy(), true);
				children = null;
			}
			NavigationTree.SelectElement(currentElement);
		}
		public override void Deselect(bool destroying) { Element.IsSelected = false; }
		public override void Select() { Element.IsSelected = true; }
		public override void ProcessKeyDown(object sender, KeyEventArgs e) {
			if (Node.Tree.Current == Node)
				e.Handled = Element.ProcessKeyDown(e);
		}
		public override void ProcessKeyUp(object sender, KeyEventArgs e) { }
		protected virtual void OnElementChanged(object sender, EventArgs e) {
			Node.With(x => x.Parent).Do(x => x.ResetChildren());
		}
		public override bool SelectNext() { return Node.Parent.Return(x => x.SelectNext(), () => false); }
		public override bool SelectPrevious() { return Node.Parent.Return(x => x.SelectPrevious(), () => false); }
		public override bool SelectLast() { return Node.Parent.Return(x => x.SelectLast(), () => false); }
		public override bool SelectFirst() { return Node.Parent.Return(x => x.SelectFirst(), () => false); }
		public override void ForEach(Action<NavigationTreeNode> action, bool expandedNodesOnly) {
			if (children == null && expandedNodesOnly || Children.Count == 0)
				return;
			action(Children[0]);
			Children[0].ForEach(action, expandedNodesOnly);
		}
	}
	public class NavigationTreeNodeMergedStrategy : NavigationTreeNodeStrategy {
		readonly NavigationTreeNodeOwnerStrategy ownerStrategy;
		readonly NavigationTreeNodeElementStrategy elementStrategy;
		INavigationElement ElementValue { get { return Element as INavigationElement; } }
		INavigationOwner OwnerValue { get { return ElementValue as INavigationOwner; } }
		public NavigationTreeNodeMergedStrategy(IBarsNavigationSupport element, NavigationTreeNode node) : base(element, node) {
			this.elementStrategy = new NavigationTreeNodeElementStrategy((INavigationElement)element, node);
			this.ownerStrategy = new NavigationTreeNodeOwnerStrategy((INavigationOwner)element, node);
		}
		public override IList<NavigationTreeNode> Children { get { return ownerStrategy.Children; } }
		public override void Deselect(bool destroying) {
			if (!ElementValue.IsSelectable)
				ownerStrategy.Deselect(destroying);
			else
				elementStrategy.Deselect(destroying);
		}
		public override void Select() {
			if (!ElementValue.IsSelectable)
				ownerStrategy.Select();
			else
				elementStrategy.Select();
		}
		public override void ResetChildren() {
			var currentElement = NavigationTree.CurrentElement;
			using (NavigationTree.Lock()) {
				ownerStrategy.Destroy();
				elementStrategy.Destroy();
			}
			NavigationTree.SelectElement(currentElement);
		}
		public override bool TryFindNode(Func<IBarsNavigationSupport, bool> predicate, out NavigationTreeNode node, bool enterChildren) { return elementStrategy.TryFindNode(predicate, out node, enterChildren) || ownerStrategy.TryFindNode(predicate, out node, enterChildren); }
		public override void ProcessKeyDown(object sender, KeyEventArgs e) {
			if (e.Handled)
				return;
			elementStrategy.ProcessKeyDown(sender, e);
			if (e.Handled)
				return;
			ownerStrategy.ProcessKeyDown(sender, e);
		}
		public override void ProcessKeyUp(object sender, KeyEventArgs e) {
			if (e.Handled)
				return;
			ownerStrategy.ProcessKeyUp(sender, e);
		}
		public override bool SelectNext() { return ownerStrategy.SelectNext() || elementStrategy.SelectNext(); }
		public override bool SelectPrevious() { return ownerStrategy.SelectPrevious() || elementStrategy.SelectPrevious(); }
		public override bool SelectLast() { return ownerStrategy.SelectLast() || elementStrategy.SelectLast(); }
		public override bool SelectFirst() { return ownerStrategy.SelectFirst() || elementStrategy.SelectFirst(); }
		public override void Destroy() {
			base.Destroy();
			elementStrategy.Destroy();
			ownerStrategy.Destroy();
		}
		public override void ForEach(Action<NavigationTreeNode> action, bool expandedNodesOnly) {
			ownerStrategy.ForEach(action, expandedNodesOnly);
		}
	}
}
