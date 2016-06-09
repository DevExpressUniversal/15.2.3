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
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using System.Windows;
using System.Linq;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevExpress.Xpf.Bars;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Xpf.Ribbon.Automation;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Ribbon {
	public class DXTreeView : ItemsControl, ICloneable {
		#region events
		private static readonly object selectedItemChanged = new object();
		private static readonly object selectedItemRightMouseButtonClick = new object();
		private static readonly object selectedItemLeftMouseButtonClick = new object();
		private static readonly object selectedItemRemoved = new object();
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		protected internal event EventHandler SelectedItemChanged {
			add { Events.AddHandler(selectedItemChanged, value); }
			remove { Events.RemoveHandler(selectedItemChanged, value); }
		}
		protected internal event EventHandler SelectedItemRightMouseButtonClick {
			add { Events.AddHandler(selectedItemRightMouseButtonClick, value); }
			remove { Events.RemoveHandler(selectedItemRightMouseButtonClick, value); }
		}
		protected internal event EventHandler SelectedItemLeftMouseButtonClick {
			add { Events.AddHandler(selectedItemLeftMouseButtonClick, value); }
			remove { Events.RemoveHandler(selectedItemLeftMouseButtonClick, value); }
		}
		protected internal event EventHandler SelectedItemRemoved {
			add { Events.AddHandler(selectedItemRemoved, value); }
			remove { Events.RemoveHandler(selectedItemRemoved, value); }
		}
		protected void RaiseSelectedItemChanged() {
			EventHandler handler = Events[selectedItemChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void RaiseSelectedItemRemoved() {
			EventHandler handler = Events[selectedItemRemoved] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void RaiseSelectedItemRightMouseButtonClick() {
			EventHandler handler = Events[selectedItemRightMouseButtonClick] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void RaiseSelectedItemLeftMouseButtonClick() {
			EventHandler handler = Events[selectedItemLeftMouseButtonClick] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#region static
		public static readonly DependencyProperty AllowAnimationProperty;
		public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty;
		public static readonly DependencyProperty VerticalScrollBarVisibilityProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty BackgroundStyleProperty;
		public static readonly DependencyProperty BackgroundTemplateProperty;
		public Style BackgroundStyle {
			get { return (Style)GetValue(BackgroundStyleProperty); }
			set { SetValue(BackgroundStyleProperty, value); }
		}
		public ControlTemplate BackgroundTemplate {
			get { return (ControlTemplate)GetValue(BackgroundTemplateProperty); }
			set { SetValue(BackgroundTemplateProperty, value); }
		}
		static DXTreeView() {
			AllowAnimationProperty = DependencyPropertyManager.Register("AllowAnimation", typeof(bool), typeof(DXTreeView), new FrameworkPropertyMetadata(true));
			HorizontalScrollBarVisibilityProperty = DependencyPropertyManager.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DXTreeView), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));
			VerticalScrollBarVisibilityProperty = DependencyPropertyManager.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DXTreeView), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));
			SelectedItemProperty = DependencyPropertyManager.Register("SelectedItem", typeof(DXTreeViewItemBase), typeof(DXTreeView), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemPropertyChanged)));
			BackgroundStyleProperty = DependencyPropertyManager.Register("BackgroundStyle", typeof(Style), typeof(DXTreeView), new FrameworkPropertyMetadata(null));
			BackgroundTemplateProperty = DependencyPropertyManager.Register("BackgroundTemplate", typeof(ControlTemplate), typeof(DXTreeView), new FrameworkPropertyMetadata(null));
		}
		protected static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXTreeView)d).OnSelectedItemChanged((DXTreeViewItemBase)e.OldValue);
		}		
		#endregion  
		public ScrollBarVisibility HorizontalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
			set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
		}
		public ScrollBarVisibility VerticalScrollBarVisibility {
			get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
			set { SetValue(VerticalScrollBarVisibilityProperty, value); }
		}
		public DXTreeViewItemBase SelectedItem {
			get { return (DXTreeViewItemBase)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public bool AllowAnimation {
			get { return (bool)GetValue(AllowAnimationProperty); }
			set { SetValue(AllowAnimationProperty, value); }
		}
		public DXTreeView() {
			DefaultStyleKey = typeof(DXTreeView);
			this.Loaded += new RoutedEventHandler(OnLoaded);
			this.Unloaded += new RoutedEventHandler(OnUnLoaded);
		}
		void OnUnLoaded(object sender, RoutedEventArgs e) {
			UnSubscribeFromTopElementsEvents();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			SubscribeToTopElementsEvents();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			scrViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;			
		}
		internal ScrollViewer scrViewer;		
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Remove) {
				foreach(var oldItem in e.OldItems) {
					DXTreeViewItemBase itemBase = IsItemItsOwnContainerOverride(oldItem) ? (DXTreeViewItemBase)oldItem : (DXTreeViewItemBase)ItemContainerGenerator.ContainerFromItem(oldItem);
					if(itemBase == SelectedItem) {
						SelectedItem = null;
					}
				}
			}
			if(e.NewItems == null && e.Action!= NotifyCollectionChangedAction.Reset) return;
			if(e.Action == NotifyCollectionChangedAction.Reset) SelectedItem = null;			
			System.Collections.IList items = e.Action == NotifyCollectionChangedAction.Reset ? Items : e.NewItems;
			if(Items.Count == 0) return;
			foreach(object obj in items) {
				if(obj is DXTreeViewItemBase) {
					(obj as DXTreeViewItemBase).TreeView = this;
					SubscribeToItemEvents(obj as DXTreeViewItemBase);					
				}
			}
		}	
		protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue) {
			indexOfSelectedItem = -1;
			SelectedItem = null;			
			OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));			
			base.OnItemsSourceChanged(oldValue, newValue);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new DXTreeViewItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is DXTreeViewItemBase;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			if(!IsItemItsOwnContainerOverride(item)) {
				(element as DXTreeViewItemBase).Content = item;
			}
			(element as DXTreeViewItemBase).TreeView = this;
			BindingOperations.SetBinding(element, DXTreeViewItemBase.AllowAnimationProperty, new Binding() { Source = this, Path = new PropertyPath("AllowAnimation") });
			SubscribeToItemEvents(element as DXTreeViewItemBase);
			base.PrepareContainerForItemOverride(element, item);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			(element as DXTreeViewItemBase).TreeView = null;
			(element as DXTreeViewItemBase).ParentTreeViewItem = null;			
			UnSubscribeFromItemEvents(element as DXTreeViewItemBase);
			base.ClearContainerForItemOverride(element, item);
		}
		internal bool skipMoveSelection = false;
		protected internal virtual void OnSelectedItemChanged(DXTreeViewItemBase oldValue) {
			if(oldValue != null)
				oldValue.IsSelected = false;
			else indexOfSelectedItem = -1;
			if(SelectedItem != null) {
				SelectedItem.IsSelected = true;
				indexOfSelectedItem = ItemsToArray(true).IndexOf(SelectedItem);
			} else
				if(!skipMoveSelection)
					MoveSelectionDown(false);
			if(scrViewer != null)
				scrViewer.Focus();			
			RaiseSelectedItemChanged();
			if(SelectedItem != null) {
				FrameworkElement fe = null;
				if(SelectedItem.ExpandButton != null)
					fe = SelectedItem.ExpandButton;
				else
					if(SelectedItem.CheckedBorder != null)
						fe = SelectedItem.CheckedBorder;
					else fe = SelectedItem;
				Rect feRect = fe.GetBounds();
				Rect resultRect = new Rect(feRect.Left, feRect.Top - 7, feRect.Width, feRect.Height + 9);
				fe.BringIntoView(resultRect);
			}
		}
		protected internal virtual void OnItemMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {			
			if(sender is DXTreeViewItemBase) {
				SelectedItem = sender as DXTreeViewItemBase;				
				e.Handled = true;
			}
		}
		EventHandlerList events;
		protected internal virtual void SubscribeToItemEvents(DXTreeViewItemBase dXTreeViewItemBase) {
			UnSubscribeFromItemEvents(dXTreeViewItemBase);
			dXTreeViewItemBase.MouseLeftButtonDown += new MouseButtonEventHandler(OnItemMouseLeftButtonUp);
			dXTreeViewItemBase.MouseRightButtonDown += new MouseButtonEventHandler(dXTreeViewItemBase_MouseRightButtonUp);
			dXTreeViewItemBase.MouseDoubleClick += new MouseButtonEventHandler(OnPreviewItemMouseDoubleClick);			
		}
		protected internal virtual void dXTreeViewItemBase_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			SelectedItem = (sender as DXTreeViewItemBase);
			RaiseSelectedItemRightMouseButtonClick();
			e.Handled = true;
		}
		protected internal virtual void OnPreviewItemMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if(SelectedItem == null || !SelectedItem.CanExpand || ((DXTreeViewItemBase)sender) != SelectedItem) return;
			SelectedItem.IsExpanded = !SelectedItem.IsExpanded;
			e.Handled = true;
		}
		protected internal virtual void UnSubscribeFromItemEvents(DXTreeViewItemBase dXTreeViewItemBase) {
			dXTreeViewItemBase.MouseLeftButtonDown -= new MouseButtonEventHandler(OnItemMouseLeftButtonUp);
			dXTreeViewItemBase.MouseRightButtonDown -= new MouseButtonEventHandler(dXTreeViewItemBase_MouseRightButtonUp);
			dXTreeViewItemBase.MouseDoubleClick -= new MouseButtonEventHandler(OnPreviewItemMouseDoubleClick);
		}
		FrameworkElement TopElement = null;
		protected internal virtual void SubscribeToTopElementsEvents() {
			if(TopElement == null)
				TopElement = DevExpress.Xpf.Core.Native.LayoutHelper.GetTopLevelVisual(this);
			else
				TopElement.RemoveHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewOwnerMouseDown));
			TopElement.AddHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewOwnerMouseDown));
		}
		protected internal virtual void OnPreviewOwnerMouseDown(object sender, MouseButtonEventArgs e) {
		}
		protected internal virtual void UnSubscribeFromTopElementsEvents() {
			if(TopElement != null)
				TopElement.RemoveHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewOwnerMouseDown));
			TopElement = null;
		}
		protected internal int indexOfSelectedItem;
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			if(SelectedItem != null) {
				if(e.Key == Key.Down) {
					MoveSelectionDown(true);
					e.Handled = true;
				}
				if(e.Key == Key.Up) {
					MoveSelectionUp();
					e.Handled = true;
				}
				if(e.Key == Key.Left) {
					CollapseSelectedItem();
					e.Handled = true;
				}
				if(e.Key == Key.Right) {
					ExpandSelectedItem();
					e.Handled = true;
				}
				if(e.Key == Key.Space) {					
					if(SelectedItem.Content is DependencyObject)
					SetFocusToFirstFocusable(SelectedItem.Content as DependencyObject);
				}
			} else {
				SelectedItem = FindFirstSelectableItem(Items);
			}
			base.OnPreviewKeyDown(e);		
		}
		protected internal virtual void SetFocusToFirstFocusable(DependencyObject obj) {
			int childrenCount = VisualTreeHelper.GetChildrenCount(obj);			
			for(int i = 0; i < childrenCount; i++) {				
				IInputElement element = VisualTreeHelper.GetChild(obj, i) as IInputElement;
				if(element.Focusable == true) {
					element.Focus();
					return;
				}
			}
			for(int i = 0; i < childrenCount; i++) {
				SetFocusToFirstFocusable(VisualTreeHelper.GetChild(obj, i));
			}
		}
		protected internal virtual void ExpandSelectedItem() {
			if(SelectedItem.CanExpand == true && !SelectedItem.IsExpanded)
				SelectedItem.IsExpanded = true;
			MoveSelectionDown(true);
		}
		protected internal virtual void CollapseSelectedItem() {
			if(SelectedItem.IsExpanded == true) {
				SelectedItem.IsExpanded = false;
			} else {
				if(SelectedItem.ParentTreeViewItem != null && !(SelectedItem.ParentTreeViewItem is DXTreeViewGroupItem)) {
					SelectedItem = SelectedItem.ParentTreeViewItem;
					SelectedItem.IsExpanded = false;
				} else {
					MoveSelectionUp();
				}
			}
		}
		protected internal virtual List<DXTreeViewItemBase> ItemsToArray(bool onlySelectable, bool invisibleToo = false) {
			List<DXTreeViewItemBase> retVal = new List<DXTreeViewItemBase>();
			foreach(object obj in Items) {				
				if(obj is DXTreeViewItemBase) {
					DXTreeViewItemBase item = obj as DXTreeViewItemBase;
					retVal.AddRange(item.GetDXTreeViewItems(onlySelectable));
				} else {
					retVal.Add(ItemContainerGenerator.ContainerFromItem(obj) as DXTreeViewItemBase);
				}
			}
			if (!invisibleToo) {
			retVal.RemoveAll(i => i.IsVisible() == false);
			}
			if(onlySelectable)
				retVal.RemoveAll(i => i is DXTreeViewGroupItem);
			return retVal;
		}		
		protected internal virtual void MoveSelectionUp() {
			if(indexOfSelectedItem == -1) {
				SelectedItem = FindFirstSelectableItem(Items);
				return;
			}
			if(Items.Count == 0) return;
			List<DXTreeViewItemBase> items = ItemsToArray(true);
			SelectedItem = indexOfSelectedItem == 0 ? items[items.Count - 1] : items[indexOfSelectedItem - 1];
		}
		protected internal virtual void MoveSelectionDown(bool round) {
			if(indexOfSelectedItem == -1) {
				SelectedItem = FindFirstSelectableItem();
				return;
			}
			List<DXTreeViewItemBase> items = ItemsToArray(true);
			if(items.Count == 0 || indexOfSelectedItem+1>items.Count) return;
			SelectedItem = indexOfSelectedItem == (items.Count - 1) ? (round ? items[0] : items[indexOfSelectedItem]) : items[indexOfSelectedItem + 1];
		}		
		protected internal virtual DXTreeViewItemBase FindFirstSelectableItem(ItemCollection collection) {
			foreach(DXTreeViewItemBase item in collection) {
				if(item is DXTreeViewItem) return item;
			}
			return null;
		}
		protected internal virtual DXTreeViewItemBase FindFirstSelectableItem() {
			List<DXTreeViewItemBase> items = ItemsToArray(true);
			if(items.Count(i => i is DXTreeViewItem) != 0)
				return items.First(i => i is DXTreeViewItem);
			return null;
		}
		protected internal virtual DXTreeViewItemBase FindLastSelectableItem(ItemCollection collection) {			
			for(int i = collection.Count-1; i>=0; i--){
				object obj = collection[i];
				if(obj is DXTreeViewItem) {
					return obj as DXTreeViewItemBase;
				}
			}
			return null;
		}
		protected internal virtual DXTreeViewItemBase FindItem(Predicate<DXTreeViewItemBase> predicate) {
			foreach(DXTreeViewItemBase item in Items) {
				if(predicate(item)) return item;
			}
			DXTreeViewItemBase result = null;
			foreach(DXTreeViewItemBase item in Items) {
				result = item.FindItem(predicate);
				if(result != null) return result;
			}
			return null;
		}
		object ICloneable.Clone() {
			return Clone();
		}
		public virtual DXTreeView Clone() {
			DXTreeView view = new DXTreeView();
			view.ItemTemplate = ItemTemplate;
			view.ItemTemplateSelector = ItemTemplateSelector;
			CloneItems(this, view);
			return view;
		}
		internal static void CloneItems(ItemsControl source, ItemsControl target) {
			if (source.ItemsSource != null) {
				if (source.ItemsSource.OfType<DXTreeViewItemBase>().Count() != source.ItemsSource.OfType<object>().Count())
					target.ItemsSource = source.ItemsSource;
				else {
					var collection = new System.Collections.ObjectModel.ObservableCollection<DXTreeViewItemBase>();
					foreach (DXTreeViewItemBase item in source.ItemsSource) {
						collection.Add(item.Clone());
					}
					target.ItemsSource = collection;
				}
			} else {
				if (source.Items.OfType<DXTreeViewItemBase>().Count() != source.Items.OfType<object>().Count()) {
					foreach (var item in source.Items) {
						if (item is ICloneable) {
							target.Items.Add(((ICloneable)item).Clone());
							continue;
						}
						target.Items.Add(item);
					}
				} else {
					foreach (DXTreeViewItemBase item in source.Items) {
						target.Items.Add(item.Clone());
					}
				}
			}
		}
	}		  
}
