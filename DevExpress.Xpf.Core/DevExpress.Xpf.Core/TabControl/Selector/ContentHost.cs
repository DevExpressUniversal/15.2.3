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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Native {
	public class FastRenderPanel : Panel {
		public static readonly DependencyProperty ContentCacheModeProperty = DependencyProperty.Register("ContentCacheMode", typeof(TabContentCacheMode), typeof(FastRenderPanel), new PropertyMetadata(TabContentCacheMode.None,
			(d, e) => ((FastRenderPanel)d).OnContentCacheModeChanged((TabContentCacheMode)e.OldValue, (TabContentCacheMode)e.NewValue)));
		public TabContentCacheMode ContentCacheMode { get { return (TabContentCacheMode)GetValue(ContentCacheModeProperty); } set { SetValue(ContentCacheModeProperty, value); } }
		ItemsControl owner;
		public ItemsControl Owner {
			get { return owner; }
			private set {
				if(owner == value) return;
				var oldValue = owner;
				owner = value;
				OnOwnerChanged(oldValue, value);
			}
		}
		public bool IsFastModeInitialized { get; private set; }
		protected internal ContentPresenter SelectedItem { get; private set; }
		protected internal Dictionary<object, ContentPresenter> Items { get; private set; }
		public FastRenderPanel() {
			Items = new Dictionary<object, ContentPresenter>();
			IsFastModeInitialized = false;
		}
		public void Initialize(ISelectorBase owner) {
			Initialize((ItemsControl)owner);
		}
		public void Initialize(ItemsControl owner) {
			if(!(owner is ISelectorBase)) throw new NotImplementedException();
			if(Owner == owner) return;
			if(Owner != null) Uninitialize();
			IsFastModeInitialized = true;
			Owner = owner;
			Loaded += OnLoaded;
			if(IsLoaded) OnLoaded(this, EventArgs.Empty);
		}
		public void Uninitialize() {
			if(Owner == null) return;
			Loaded -= OnLoaded;
			Clear();
			Owner = null;
			SelectedItem = null;
			IsFastModeInitialized = false;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(SelectedItem == null) return base.MeasureOverride(availableSize);
			foreach(UIElement child in InternalChildren)
				child.Measure(availableSize);
			return SelectedItem.DesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(SelectedItem == null) return base.ArrangeOverride(finalSize);
			SelectedItem.Arrange(new Rect(new Point(), finalSize));
			return new Size(SelectedItem.ActualWidth, SelectedItem.ActualHeight);
		}
		void SubsribeOwner(ItemsControl owner, EventHandler containerGeneratorStatusChangedHandler, NotifyCollectionChangedEventHandler itemsChangedHandler, EventHandler selectionChangedHandler) {
			ISelectorBase iOwner = (ISelectorBase)owner;
			owner.ItemContainerGenerator.StatusChanged += containerGeneratorStatusChangedHandler;
			iOwner.ItemsChanged += itemsChangedHandler;
			iOwner.SelectionChanged += selectionChangedHandler;
		}
		void UnsubsribeOwner(ItemsControl owner, EventHandler containerGeneratorStatusChangedHandler, NotifyCollectionChangedEventHandler itemsChangedHandler, EventHandler selectionChangedHandler) {
			ISelectorBase iOwner = (ISelectorBase)owner;
			owner.ItemContainerGenerator.StatusChanged -= containerGeneratorStatusChangedHandler;
			iOwner.ItemsChanged -= itemsChangedHandler;
			iOwner.SelectionChanged -= selectionChangedHandler;
		}
		void OnOwnerChanged(ItemsControl oldValue, ItemsControl newValue) {
			oldValue.Do(x => UnsubsribeOwner(x, OnContainerGeneratorStatusChanged, OnItemsChanged, OnSelectionChanged));
			newValue.Do(x => SubsribeOwner(x, OnContainerGeneratorStatusChanged, OnItemsChanged, OnSelectionChanged));
		}
		void OnLoaded(object sender, EventArgs e) {
			Synchronize();
		}
		void OnContentCacheModeChanged(TabContentCacheMode oldValue, TabContentCacheMode newValue) {
			if(Owner != null) Synchronize();
		}
		void OnContainerGeneratorStatusChanged(object sender, EventArgs e) {
			if(Owner.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) UpdateSelection();
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			if(Owner.Items.Count != Items.Count) Synchronize();
			else UpdateSelection();
			InvalidateMeasure();
		}
		void Synchronize() {
			switch(ContentCacheMode) {
				case TabContentCacheMode.None: return;
				case TabContentCacheMode.CacheAllTabs:
					SynchronizeAllTabs();
					break;
				case TabContentCacheMode.CacheTabsOnSelecting:
					SynchronizeSelectedTab();
					break;
			}
			UpdateSelection();
		}
		void SynchronizeSelectedTab() {
			ISelectorBase owner = (ISelectorBase)Owner;
			if(owner.SelectedItem != null && !Items.ContainsKey(owner.SelectedItem))
				AddItem(owner.SelectedItem);
		}
		void SynchronizeAllTabs() {
			foreach(object item in Owner.Items)
				if(!Items.ContainsKey(item)) AddItem(item);
			UpdateSelection();
		}
		void UpdateSelection() {
			if(Items.Count == 0) {
				SelectedItem = null;
				return;
			}
			ISelectorBase owner = (ISelectorBase)Owner;
			SelectedItem = owner.SelectedItem != null && Items.ContainsKey(owner.SelectedItem) ? Items[owner.SelectedItem] : null;
			if(SelectedItem != null)
				SelectedItem.ContentTemplate = GetActualContentTemplate(owner.GetContainer(owner.SelectedItem), owner.SelectedItem);
			foreach(UIElement item in Children)
				item.Visibility = item == SelectedItem ? Visibility.Visible : Visibility.Hidden;
		}
		void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					OnItemsAdd(e);
					break;
				case NotifyCollectionChangedAction.Remove:
					OnItemsRemove(e);
					break;
				case NotifyCollectionChangedAction.Move:
					OnItemsMove(e);
					break;
				case NotifyCollectionChangedAction.Replace:
					OnItemsReplace(e);
					break;
				case NotifyCollectionChangedAction.Reset:
					Clear();
					break;
			}
			Synchronize();
		}
		void OnItemsAdd(NotifyCollectionChangedEventArgs e) {
			if(e.NewItems == null || ContentCacheMode != TabContentCacheMode.CacheAllTabs) return;
			for(int i = 0; i < e.NewItems.Count; i++) {
				object item = e.NewItems[i];
				if(item == null || Items.ContainsKey(item)) continue;
				InsertItem(item, e.NewStartingIndex + i);
			}
		}
		void OnItemsRemove(NotifyCollectionChangedEventArgs e) {
			if(e.OldItems == null) return;
			foreach(object item in e.OldItems) RemoveItem(item);
		}
		void OnItemsMove(NotifyCollectionChangedEventArgs e) {
			object item = e.NewItems[0];
			ISelectorBase owner = (ISelectorBase)Owner;
			var container = owner.GetContainer(item);
			if(container == null || !Items.ContainsKey(item)) return;
			BindItem(Items[item], container);
		}
		void OnItemsReplace(NotifyCollectionChangedEventArgs e) {
			if(e.NewItems == null || e.OldItems == null) return;
			object oldTabItem = e.OldItems[0];
			object newTabItem = e.NewItems[0];
			if(oldTabItem == null || newTabItem == null || !Items.ContainsKey(oldTabItem)) return;
			RemoveItem(oldTabItem);
			InsertItem(newTabItem, e.NewStartingIndex);
		}
		void AddItem(object item) {
			InsertItem(item, Children.Count);
		}
		void InsertItem(object item, int index) {
			ISelectorBase owner = (ISelectorBase)Owner;
			var container = owner.GetContainer(item);
			if(container == null) return;
			var cp = new FastContentPresenter() { Visibility = Visibility.Collapsed };
			BindItem(cp, container);
			cp.ContentTemplate = GetActualContentTemplate(container, item);
			Children.Add(cp);
			Items.Add(item, cp);
		}
		void RemoveItem(object item) {
			if(item == null || !Items.ContainsKey(item)) return;
			Children.Remove(Items[item]);
			Items.Remove(item);
		}
		void Clear() {
			foreach(ContentPresenter cp in Children)
				ClearBind(cp);
			Items.Clear();
			Children.Clear();
			SelectedItem = null;
		}
		void BindItem(ContentPresenter cp, ContentControl container) {
			cp.SetBinding(ContentPresenter.ContentProperty, new Binding() { Source = container, Path = new PropertyPath(ContentControl.ContentProperty) });
			cp.SetBinding(MergingProperties.ElementMergingBehaviorProperty, new Binding() { Source = container, Path = new PropertyPath(MergingProperties.ElementMergingBehaviorProperty) });
		}
		void ClearBind(ContentPresenter cp) {
			BindingOperations.ClearAllBindings(cp);
			cp.Content = null;
			cp.ContentTemplate = null;
		}
		DataTemplate GetActualContentTemplate(ContentControl container, object content) {
			if(container == null) return null;
			if(container.ContentTemplate != null) return container.ContentTemplate;
			if(container.ContentTemplateSelector != null) return container.ContentTemplateSelector.SelectTemplate(container.Content, container);
			if(Owner.ItemTemplate != null) return Owner.ItemTemplate;
			if(Owner.ItemTemplateSelector != null) return Owner.ItemTemplateSelector.SelectTemplate(container.Content, container);
			return null;
		}
		class FastContentPresenter : ContentPresenter {
			public FastContentPresenter() {
				PreviewLostKeyboardFocus += OnPreviewLostKeyboardFocus;
			}
			static FastContentPresenter() {
				VisibilityProperty.OverrideMetadata(typeof(FastContentPresenter), new FrameworkPropertyMetadata(
					(d, e) => ((FastContentPresenter)d).OnVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue)));
			}
			WeakReference focusedElement = null;
			void OnPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
				focusedElement = new WeakReference(FocusHelper.GetFocusedElement());
			}
			void OnVisibilityChanged(Visibility oldValue, Visibility newValue) {
				if(newValue != Visibility.Visible || !IsVisible) return;
				var fe = focusedElement == null ? null : focusedElement.Target as FrameworkElement;
				if(fe == null) return;
				Dispatcher.BeginInvoke(new Action(() => fe.Focus()));
			}
			protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
				base.OnVisualChildrenChanged(visualAdded, visualRemoved);
				visualAdded.Do(x => BindingOperations.SetBinding(x, TextBlock.ForegroundProperty,
					new Binding() { Path = new PropertyPath(TextBlock.ForegroundProperty), Source = this }));
			}
		}
	}
	public class ContentHostPresenterBase : ContentPresenter {
		class ContentHostStorage {
			ContentHostBase host;
			public ContentHostBase Host {
				get { return host; }
				set {
					if(host == value) return;
					var oldValue = host;
					host = value;
					OnHostChanged(oldValue, value);
				}
			}
			public string Name { get; private set; }
			Func<FrameworkElement> GetHostChild;
			public ContentHostStorage(string hostName, Func<FrameworkElement> getHostChild) {
				Name = hostName;
				GetHostChild = getHostChild;
			}
			public void UpdateHostChild() {
				Host.Do(x => x.Child = GetHostChild());
			}
			void OnHostChanged(ContentHostBase oldValue, ContentHostBase newValue) {
				oldValue.Do(x => x.Child = null);
				newValue.Do(x => x.Child = GetHostChild());
			}
		}
		List<ContentHostStorage> ContentHostStorages = new List<ContentHostStorage>();
		static ContentHostPresenterBase() {
			ContentProperty.OverrideMetadata(typeof(ContentHostPresenterBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, 
				(d, e) => ((ContentHostPresenterBase)d).OnContentChanged(e.OldValue, e.NewValue)));
		}
		void OnContentChanged(object oldValue, object newValue) {
			(oldValue as ContentHostBase).Do(x => x.OwnerPresenter = null);
			if(IsInitialized) (newValue as ContentHostBase).Do(x => x.OwnerPresenter = this);
		}
		protected override void OnInitialized(EventArgs e) {
			(Content as ContentHostBase).Do(x => x.OwnerPresenter = this);
			base.OnInitialized(e);
		}
		protected void Init(string hostName, Func<FrameworkElement> getHostChild) {
			ContentHostStorages.Add(new ContentHostStorage(hostName, getHostChild));
		}
		protected void UpdateHostChild(string hostName) {
			ContentHostStorages.First(x => x.Name == hostName).UpdateHostChild();
		}
		protected ISelectorBase GetOwnerControl() {
			return TemplatedParent as ISelectorBase ?? LayoutTreeHelper.GetVisualParents(this).OfType<ISelectorBase>().FirstOrDefault();
		}
		internal void SetContentHost(string hostName, ContentHostBase value) {
			ContentHostStorages.First(x => x.Name == hostName).Host = value;
		}
	}
	public class ContentHostPresenter : ContentHostPresenterBase {
		public static readonly DependencyProperty ContentCacheModeProperty = DependencyProperty.Register("ContentCacheMode", typeof(TabContentCacheMode), typeof(ContentHostPresenter), new PropertyMetadata(TabContentCacheMode.None));
		public static readonly DependencyProperty RegularContentPresenterProperty = DependencyProperty.Register("RegularContentPresenter", typeof(object), typeof(ContentHostPresenter), new PropertyMetadata(null, (d, e) => ((ContentHostPresenter)d).UpdateContentHostChild()));
		public static readonly DependencyProperty FastContentPresenterProperty = DependencyProperty.Register("FastContentPresenter", typeof(object), typeof(ContentHostPresenter), new PropertyMetadata(null, (d, e) => ((ContentHostPresenter)d).UpdateContentHostChild()));
		public TabContentCacheMode ContentCacheMode { get { return (TabContentCacheMode)GetValue(ContentCacheModeProperty); } set { SetValue(ContentCacheModeProperty, value); } }
		public object RegularContentPresenter { get { return GetValue(RegularContentPresenterProperty); } set { SetValue(RegularContentPresenterProperty, value); } }
		public object FastContentPresenter { get { return GetValue(FastContentPresenterProperty); } set { SetValue(FastContentPresenterProperty, value); } }
		public FrameworkElement ContentHostChild { get; private set; }
		public ContentHostPresenter() {
			Init(ContentHost.ContentHostName, () => ContentHostChild);
		}
		void UpdateContentHostChild() {
			var oldContentHostChild = ContentHostChild;
			if(ContentCacheMode == TabContentCacheMode.None && RegularContentPresenter != null)
				ContentHostChild = (FrameworkElement)RegularContentPresenter;
			else if(FastContentPresenter != null) {
				FastRenderPanel fast = (FastRenderPanel)FastContentPresenter;
				if(!fast.IsFastModeInitialized) fast.Initialize(GetOwnerControl());
				ContentHostChild = fast;
			}
			if(oldContentHostChild != ContentHostChild) UpdateHostChild(ContentHost.ContentHostName);
		}
	}
	public class ContentAndFooterHostPresenter : ContentHostPresenter {
		public static readonly DependencyProperty FooterCacheModeProperty = DependencyProperty.Register("FooterCacheMode", typeof(TabContentCacheMode), typeof(ContentAndFooterHostPresenter), new PropertyMetadata(TabContentCacheMode.None));
		public static readonly DependencyProperty RegularFooterPresenterProperty = DependencyProperty.Register("RegularFooterPresenter", typeof(object), typeof(ContentAndFooterHostPresenter), new PropertyMetadata(null, (d, e) => ((ContentAndFooterHostPresenter)d).UpdateContentHostChild()));
		public static readonly DependencyProperty FastFooterPresenterProperty = DependencyProperty.Register("FastFooterPresenter", typeof(object), typeof(ContentAndFooterHostPresenter), new PropertyMetadata(null, (d, e) => ((ContentAndFooterHostPresenter)d).UpdateContentHostChild()));
		public TabContentCacheMode FooterCacheMode { get { return (TabContentCacheMode)GetValue(FooterCacheModeProperty); } set { SetValue(FooterCacheModeProperty, value); } }
		public object RegularFooterPresenter { get { return GetValue(RegularFooterPresenterProperty); } set { SetValue(RegularFooterPresenterProperty, value); } }
		public object FastFooterPresenter { get { return GetValue(FastFooterPresenterProperty); } set { SetValue(FastFooterPresenterProperty, value); } }
		public FrameworkElement FooterHostChild { get; private set; }
		public ContentAndFooterHostPresenter() {
			Init(FooterHost.FooterHostName, () => FooterHostChild);
		}
		void UpdateContentHostChild() {
			var oldContentHostChild = FooterHostChild;
			if(FooterCacheMode == TabContentCacheMode.None && RegularFooterPresenter != null) {
				FooterHostChild = (FrameworkElement)RegularFooterPresenter;
			} else if(FastFooterPresenter != null) {
				FastRenderPanel fast = (FastRenderPanel)FastFooterPresenter;
				if(!fast.IsFastModeInitialized) fast.Initialize(GetOwnerControl());
				FooterHostChild = fast;
			}
			if(oldContentHostChild != FooterHostChild) UpdateHostChild(FooterHost.FooterHostName);
		}
	}
	public abstract class ContentHostBase : Panel {
		ContentHostPresenterBase ownerPresenter = null;
		public ContentHostPresenterBase OwnerPresenter {
			get { return ownerPresenter; }
			internal set {
				if(ownerPresenter == value) return;
				var oldValue = ownerPresenter;
				ownerPresenter = value;
				OnOwnerPresenterChanged(oldValue, value);
			}
		}
		FrameworkElement child = null;
		public FrameworkElement Child {
			get { return child; }
			set {
				if(child == value) return;
				var oldValue = child;
				child = value;
				OnChildChanged(oldValue, value);
			}
		}
		public ContentHostBase() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		protected abstract string GetHostName();
		protected abstract ContentHostPresenterBase FindContentHostPresenter();
		void OnChildChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			oldValue.Do(x => Children.Remove(x));
			newValue.Do(x => Children.Add(x));
		}
		void OnOwnerPresenterChanged(ContentHostPresenterBase oldValue, ContentHostPresenterBase newValue) {
			if(oldValue == newValue) return;
			oldValue.Do(x => x.SetContentHost(GetHostName(), null));
			newValue.Do(x => x.SetContentHost(GetHostName(), this));
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			OwnerPresenter = FindContentHostPresenter();
			if(OwnerPresenter == null) LayoutUpdated += OnLayoutUpdated;
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			LayoutUpdated -= OnLayoutUpdated;
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			OwnerPresenter = FindContentHostPresenter();
			if(OwnerPresenter != null) LayoutUpdated -= OnLayoutUpdated;
		}
		protected override Size MeasureOverride(Size availableSize) {
			Child.Do(x => x.Measure(availableSize));
			return Child.Return(x => x.DesiredSize, () => new Size());
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Child.Do(x => x.Arrange(new Rect(new Point(), finalSize)));
			return finalSize;
		}
	}
}
namespace DevExpress.Xpf.Core {
	public class ContentHost : ContentHostBase {
		internal const string ContentHostName = "ContentHost";
		protected override string GetHostName() { return ContentHostName; }
		protected override ContentHostPresenterBase FindContentHostPresenter() {
			return LayoutTreeHelper.GetVisualParents(this).OfType<ContentHostPresenter>().FirstOrDefault();
		}
	}
	public class FooterHost : ContentHostBase {
		internal const string FooterHostName = "FooterHost";
		protected override string GetHostName() { return FooterHostName; }
		protected override ContentHostPresenterBase FindContentHostPresenter() {
			return LayoutTreeHelper.GetVisualParents(this).OfType<ContentAndFooterHostPresenter>().FirstOrDefault();
		}
	}
}
