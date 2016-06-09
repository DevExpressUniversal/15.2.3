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
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.WindowsUI.Base {
	[TemplatePart(Name = "PART_ItemsPresenter", Type = typeof(ItemsPresenter))]
	public abstract class veSelector : veSelectorBase, ISelector {
		protected virtual bool RequiresSelectedItem { get { return true; } }
		protected override void OnItemContainerGeneratorStatusChanged() {
			if(RequiresSelectedItem)
				EnsureSelectedItem();
		}
		protected override void OnHasItemsChanged(bool hasItems) {
			base.OnHasItemsChanged(hasItems);
			if(RequiresSelectedItem)
				EnsureSelectedItem();
		}
		int selectionLock;
		protected bool IsSelectionLocked { get { return selectionLock > 0; } }
		void EnsureSelectedItem() {
			if(base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated) {
				if(base.HasItems) {
					if(base.SelectedItem == null) {
						selectionLock++;
						try {
							base.SelectedIndex = 0;
						}
						finally {
							selectionLock--;
						}
					}
				}
			}
		}
		protected override void OnSelectionChangedCore(IList removedItems, IList addedItems) {
			if(removedItems != null) {
				foreach(var item in removedItems) {
					var container = ItemContainerGenerator.ContainerFromItem(item);
					if(container is ISelectorItem && !addedItems.Return(x => x.Contains(item), () => false))
						((ISelectorItem)container).IsSelected = false;
				}
			}
			if(addedItems != null && addedItems.Count > 0) {
				var item = addedItems[addedItems.Count - 1];
				var container = ItemContainerGenerator.ContainerFromItem(item);
				if(container is ISelectorItem) ((ISelectorItem)container).IsSelected = true;
			}
		}
		protected virtual bool CanChangeSelection() { return true; }
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			if(!CanChangeSelection()) return;
			if((e.Action == NotifyCollectionChangedAction.Remove) && (base.SelectedIndex == -1)) {
				int startIndex = e.OldStartingIndex + 1;
				if(startIndex > base.Items.Count) {
					startIndex = 0;
				}
				ISelectorItem item = this.FindNextContainer(startIndex, -1);
				if(item != null) {
					item.IsSelected = true;
				}
			}
		}
		private ISelectorItem FindNextContainer(int startIndex, int direction) {
			if(direction != 0) {
				int index = startIndex;
				for(int i = 0; i < base.Items.Count; i++) {
					index += direction;
					if(index >= base.Items.Count) {
						index = 0;
					}
					else if(index < 0) {
						index = base.Items.Count - 1;
					}
					ISelectorItem item2 = base.ItemContainerGenerator.ContainerFromIndex(index) as ISelectorItem;
					if(CheckItem(item2)) return item2;
				}
			}
			return null;
		}
		protected virtual bool CheckItem(ISelectorItem item) {
			return item != null;
		}
		protected sealed override DependencyObject GetContainerForItemOverride() {
			return CreateSelectorItem() as DependencyObject;
		}
		protected abstract ISelectorItem CreateSelectorItem();
		protected sealed override void PrepareContainer(DependencyObject element, object item) {
			base.PrepareContainer(element, item);
			PrepareSelectorItem(element as ISelectorItem, item);
		}
		protected sealed override void ClearContainer(DependencyObject element, object item) {
			ClearSelectorItem(element as ISelectorItem, item);
			base.ClearContainer(element, item);
		}
		protected virtual void ClearSelectorItem(ISelectorItem selectorItem, object item) {
			if(selectorItem != null) {
				selectorItem.Owner = null;
			}
		}
		protected virtual void PrepareSelectorItem(ISelectorItem selectorItem, object item) {
			if(selectorItem != null) {
				selectorItem.IsSelected = object.Equals(SelectedItem, item);
				selectorItem.Owner = this;
			}
		}
		protected virtual void UpdateSelectionCore() { }
		#region IContentSelector Members
		protected virtual void SelectCore(int index) {
			SelectedIndex = index;
		}
		public void Select(ISelectorItem item) {
			DependencyObject container = ItemContainerGenerator.ContainerFromItem(item) ?? item as DependencyObject;
			if(container != null) {
				int index = ItemContainerGenerator.IndexFromContainer(container);
				SelectCore(index);
			}
		}
		#endregion
		#region ISelector Members
		void ISelector.Select(ISelectorItem item) {
			Select(item);
		}
		void ISelector.UpdateSelection() {
			UpdateSelectionCore();
		}
		#endregion
	}
	public abstract class veSelectorBase : Selector, IDisposable {
		#region static
		public static readonly RoutedEvent SelectionChangingEvent;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty HasItemsInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty SelectedItemInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty SelectedIndexInternalProperty;
		static readonly Action<veSelectorBase, bool> setCanSelectMultiple =
			ReflectionHelper.CreateInstanceMethodHandler<Action<veSelectorBase, bool>>(null, "set_CanSelectMultiple",
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, typeof(veSelectorBase));
		static veSelectorBase() {
			var dProp = new DependencyPropertyRegistrator<veSelectorBase>();
			dProp.Register("HasItemsInternal", ref HasItemsInternalProperty, false,
				(dObj, e) => ((veSelectorBase)dObj).OnHasItemsChanged((bool)e.NewValue));
			dProp.Register("SelectedItemInternal", ref SelectedItemInternalProperty, (object)null,
				(dObj, e) => ((veSelectorBase)dObj).OnSelectedItemChanged(e.OldValue, e.NewValue));
			dProp.Register("SelectedIndexInternal", ref SelectedIndexInternalProperty, -1,
				(dObj, e) => ((veSelectorBase)dObj).OnSelectedIndexChanged((int)e.OldValue, (int)e.NewValue));
			SelectedIndexProperty.OverrideMetadata(typeof(veSelectorBase), new FrameworkPropertyMetadata((PropertyChangedCallback)null, new CoerceValueCallback((d, value) => ((veSelectorBase)d).OnCoerceSelectedIndex((int)value)))); 
			SelectionChangingEvent = EventManager.RegisterRoutedEvent("SelectionChanging", RoutingStrategy.Bubble, typeof(SelectionChangingEventHandler), typeof(veSelectorBase));
		}
		#endregion static
		public veSelectorBase() {
			setCanSelectMultiple(this, false);
			IsTabStop = false;
			SubscribeEvents();
			SetBinding(HasItemsInternalProperty, new Binding() { Path = new PropertyPath("HasItems"), Source = this });
			SetBinding(SelectedItemInternalProperty, new Binding() { Path = new PropertyPath("SelectedItem"), Source = this });
			SetBinding(SelectedIndexInternalProperty, new Binding() { Path = new PropertyPath("SelectedIndex"), Source = this });
		}
		public bool IsDisposing { get; private set; }
		void IDisposable.Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				UnSubscribeEvents();
				OnDispose();
				ClearValue(ItemsSourceProperty);
				if(PartItemsPanel != null)
					ReleaseItemsPanelCore(PartItemsPanel);
				PartItemsPanel = null;
				PartItemsPresenter = null;
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void SubscribeEvents() {
			Loaded += veSelectorBase_Loaded;
			Unloaded += veSelectorBase_Unloaded;
			SizeChanged += veSelectorBase_SizeChanged;
			IsEnabledChanged += veSelectorBase_IsEnabledChanged;
		}
		protected virtual void UnSubscribeEvents() {
			IsEnabledChanged -= veSelectorBase_IsEnabledChanged;
			SizeChanged -= veSelectorBase_SizeChanged;
			Loaded -= veSelectorBase_Loaded;
			Unloaded -= veSelectorBase_Unloaded;
		}
		void veSelectorBase_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		void veSelectorBase_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
		}
		void ItemContainerGenerator_StatusChanged(object sender, EventArgs e) {
			OnItemContainerGeneratorStatusChanged();
		}
		protected virtual void OnItemContainerGeneratorStatusChanged() { }
		protected virtual void OnSelectionChangedCore(IList removedItems, IList addedItems) { }
		protected virtual object OnCoerceSelectedIndex(int value) {
			var coerceValue = SelectedIndexProperty.GetMetadata(typeof(Selector)).CoerceValueCallback(this as Selector, value);
			if(!(coerceValue is int))
				return DependencyProperty.UnsetValue;
			value = (int)coerceValue;
			var oldIndex = SelectedIndex;
			var newIndex = (int)value;
			if(oldIndex != newIndex && !RaiseSelectionChanging(oldIndex, newIndex)) {
				return oldIndex;
			}
			return value;
		}
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			base.OnSelectionChanged(e);
			OnSelectionChangedCore(e.RemovedItems, e.AddedItems);
			ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
			if(view != null && this.IsSynchronizedWithCurrentItem.GetValueOrDefault()) view.MoveCurrentToPosition(SelectedIndex);
		}
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
		}
		void veSelectorBase_SizeChanged(object sender, SizeChangedEventArgs e) {
			OnSizeChanged(e.NewSize);
		}
		void veSelectorBase_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			OnIsEnabledChanged();
		}
		protected sealed override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			PrepareContainer(element, item);
		}
		protected virtual void PrepareContainer(DependencyObject element, object item) {
			if(element is IItemContainer) {
				((IItemContainer)element).PrepareContainer(item, ItemTemplate, ItemTemplateSelector);
			}
		}
		protected sealed override void ClearContainerForItemOverride(DependencyObject element, object item) {
			ClearContainer(element, item);
			base.ClearContainerForItemOverride(element, item);
		}
		protected virtual void ClearContainer(DependencyObject element, object item) {
			if(element is IItemContainer && element != item)
				((IItemContainer)element).ClearContainer();
		} 
		ScrollViewer PartScrollViewer;
		protected ItemsPresenter PartItemsPresenter;
		protected internal Panel PartItemsPanel;
		public sealed override void OnApplyTemplate() {
			ClearTemplateChildren();
			BeforeApplyTemplate();
			base.OnApplyTemplate();
			GetTemplateChildren();
			OnApplyTemplateComplete();
		}
		protected virtual void ClearTemplateChildren() {
			if(PartItemsPanel != null && !LayoutTreeHelper.IsTemplateChild(PartItemsPanel, this)) {
				ReleaseItemsPanelCore(PartItemsPanel);
				PartItemsPanel = null;
			}
			if(PartItemsPresenter != null && !LayoutTreeHelper.IsTemplateChild(PartItemsPresenter, this))
				PartItemsPresenter.SizeChanged -= PartItemsPresenter_SizeChanged;
			if(PartScrollViewer != null && !LayoutTreeHelper.IsTemplateChild(PartScrollViewer, this))
				PartScrollViewer.SizeChanged -= PartItemsPresenter_SizeChanged;
		}
		protected virtual void GetTemplateChildren() {
			PartItemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter ??
				LayoutTreeHelper.GetTemplateChild<ItemsPresenter, veItemsControl>(this);
			if(PartItemsPresenter != null)
				PartItemsPresenter.SizeChanged += PartItemsPresenter_SizeChanged;
			else {
				PartScrollViewer = LayoutTreeHelper.GetTemplateChild<ScrollViewer, veItemsControl>(this);
				if(PartScrollViewer != null) {
					PartScrollViewer.SizeChanged += PartItemsPresenter_SizeChanged;
				}
			}
		}
		protected virtual void OnApplyTemplateComplete() { }
		protected virtual void BeforeApplyTemplate() { }
		void PartItemsPresenter_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(PartItemsPresenter != null)
				PartItemsPresenter.SizeChanged -= PartItemsPresenter_SizeChanged;
			if(PartScrollViewer != null) {
				PartScrollViewer.SizeChanged -= PartItemsPresenter_SizeChanged;
				PartItemsPresenter = LayoutTreeHelper.GetTemplateChild<ItemsPresenter, veItemsControl>(this);
			}
			EnsureItemsPanel(PartItemsPresenter);
		}
		protected internal void EnsureItemsPanel(ItemsPresenter itemsPresenter, Panel panel = null) {
			if(PartItemsPanel == null) {
				PartItemsPanel = panel ?? LayoutTreeHelper.GetTemplateChild<Panel, ItemsPresenter>(itemsPresenter);
				if(PartItemsPanel != null)
					EnsureItemsPanelCore(PartItemsPanel);
			}
			else {
				if(panel != PartItemsPanel)
					PartItemsPanel.InvalidateMeasure();
			}
		}
		protected virtual void ReleaseItemsPanelCore(Panel itemsPanel) {
			IItemsPanel IPanel = itemsPanel as IItemsPanel;
			if(IPanel != null) IPanel.ItemsControl = null;
		}
		protected virtual void EnsureItemsPanelCore(Panel itemsPanel) {
			IItemsPanel IPanel = itemsPanel as IItemsPanel;
			if(IPanel != null) IPanel.ItemsControl = this;
		}
		protected bool RaiseSelectionChanging(int oldIndex, int newIndex) {
			var eventArgs = new SelectionChangingEventArgs(oldIndex, newIndex);
			OnSelectionChanging(eventArgs);
			return !eventArgs.Cancel;
		}
		protected virtual void OnSelectionChanging(SelectionChangingEventArgs e) {
			RaiseEvent(e);
		}
		internal int IndexOf(object item) {
			if(item == null) return -1;
			if(Items.Contains(item))
				return Items.IndexOf(item);
			if(IsItemItsOwnContainerOverride(item))
				return ItemContainerGenerator.IndexFromContainer((DependencyObject)item);
			return -1;
		}
		public event SelectionChangingEventHandler SelectionChanging {
			add { AddHandler(SelectionChangingEvent, value); }
			remove { RemoveHandler(SelectionChangingEvent, value); }
		}
		protected virtual void OnDispose() { }
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		protected virtual void OnIsEnabledChanged() { }
		protected virtual void OnSizeChanged(Size size) { }
		protected virtual void OnHasItemsChanged(bool hasItems) { }
		protected virtual void OnSelectedItemChanged(object oldValue, object newValue) { }
		protected virtual void OnSelectedIndexChanged(int oldValue, int newValue) { }
	}
	public delegate void SelectionChangingEventHandler(object sender, SelectionChangingEventArgs e);
	public class SelectionChangingEventArgs : RoutedEventArgs {
		public bool Cancel { get; set; }
		public int OldIndex { get; private set; }
		public int NewIndex { get; private set; }
		public object OldValue {
			get {
				if(Source == null || OldIndex == -1)
					return null;
				var selector = Source as veSelectorBase;
				return selector.Items[OldIndex];
			}
		}
		public object NewValue {
			get {
				if(Source == null || NewIndex == -1)
					return null;
				var selector = Source as veSelectorBase;
				return selector.Items[NewIndex];
			}
		}
		public SelectionChangingEventArgs(int oldValue, int newValue) : base(veSelectorBase.SelectionChangingEvent) {
			OldIndex = oldValue;
			NewIndex = newValue;
		}
	}
}
