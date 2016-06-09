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
using System.Windows.Controls.Primitives;
using System.Collections;
namespace DevExpress.Xpf.Ribbon {
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class DXTreeViewItem : DXTreeViewItemBase {
		#region static
		public static readonly DependencyProperty GlyphStyleProperty;
		public static readonly DependencyProperty GlyphContainerStyleProperty;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty IsGlyphVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsGlyphVisiblePropertyKey;
		public static readonly DependencyProperty ActualIsGlyphVisibleProperty;
		static DXTreeViewItem() {
			GlyphStyleProperty = DependencyPropertyManager.Register("GlyphStyle", typeof(Style), typeof(DXTreeViewItem), new FrameworkPropertyMetadata(null));
			GlyphContainerStyleProperty = DependencyPropertyManager.Register("GlyphContainerStyle", typeof(Style), typeof(DXTreeViewItem), new FrameworkPropertyMetadata(null));
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(DXTreeViewItem), new FrameworkPropertyMetadata(null, (d, e) => { ((DXTreeViewItem)d).OnGlyphChanged(e.OldValue as ImageSource); }));
			IsGlyphVisibleProperty = DependencyPropertyManager.Register("IsGlyphVisible", typeof(bool), typeof(DXTreeViewItem), new FrameworkPropertyMetadata(true, (d, e) => { ((DXTreeViewItem)d).OnIsGlyphVisibleChanged((bool)e.OldValue); }));
			ActualIsGlyphVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsGlyphVisible", typeof(bool), typeof(DXTreeViewItem), new FrameworkPropertyMetadata(false));
			ActualIsGlyphVisibleProperty = ActualIsGlyphVisiblePropertyKey.DependencyProperty;
		}
		#endregion        
		#region props
		public bool IsGlyphVisible {
			get { return (bool)GetValue(IsGlyphVisibleProperty); }
			set { SetValue(IsGlyphVisibleProperty, value); }
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public Style GlyphContainerStyle {
			get { return (Style)GetValue(GlyphContainerStyleProperty); }
			set { SetValue(GlyphContainerStyleProperty, value); }
		}
		public Style GlyphStyle {
			get { return (Style)GetValue(GlyphStyleProperty); }
			set { SetValue(GlyphStyleProperty, value); }
		}
		public bool ActualIsGlyphVisible {
			get { return (bool)GetValue(ActualIsGlyphVisibleProperty); }
			protected set { this.SetValue(ActualIsGlyphVisiblePropertyKey, value); }
		}
		#endregion
		public DXTreeViewItem()
			: base() {
				DefaultStyleKey = typeof(DXTreeViewItem);
		}
		protected virtual void OnIsGlyphVisibleChanged(bool oldValue) {
			UpdateActualIsGlyphVisible();
		}
		protected virtual void OnGlyphChanged(ImageSource oldValue) {
			UpdateActualIsGlyphVisible();
		}
		protected virtual void UpdateActualIsGlyphVisible() {
			ActualIsGlyphVisible = Glyph != null && IsGlyphVisible;
		}		
	}	
	public class DXTreeViewGroupItem : DXTreeViewItemBase {
		public DXTreeViewGroupItem() {
			DefaultStyleKey = typeof(DXTreeViewGroupItem);
			this.IsExpanded = true;
		}
		bool isFirstElementInTreeView = false;
		protected override void OnTreeViewChanged(DXTreeView oldValue) {
			base.OnTreeViewChanged(oldValue);
			if(TreeView == null) return;
			if(TreeView.Items.Contains(this) && TreeView.Items.IndexOf(this) == 0)
				this.isFirstElementInTreeView = true;
			UpdateVisualStates();
		}
		private void UpdateVisualStates() {
			if(isFirstElementInTreeView) {
				VisualStateManager.GoToState(this, "UpperElement", false);
			} else
				VisualStateManager.GoToState(this, "Normal", false);
		}
		protected override object OnCanExpandPropertyChangedCoerce(object v) {
			return false;
		}
		protected override object OnIsExpandedPropertyChangedCoerce(object v) {
			return true;
		}
		protected override DXTreeViewItemBase CreateClone() {
			return new DXTreeViewGroupItem();
	}
	}
	public abstract class DXTreeViewItemBase : ItemsControl, ICloneable {
		#region static
		public static readonly DependencyProperty AllowAnimationProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty BorderTemplateProperty;
		public static readonly DependencyProperty BorderStyleProperty;
		public static readonly DependencyProperty ExpanderStyleProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty ContentStyleProperty;
		public static readonly DependencyProperty ItemsPresenterStyleProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty CanExpandProperty;
		public static readonly DependencyProperty TreeViewProperty;
		static DXTreeViewItemBase() {
			AllowAnimationProperty = DependencyPropertyManager.Register("AllowAnimation", typeof(bool), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(true));
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));
			BorderStyleProperty = DependencyPropertyManager.Register("BorderStyle", typeof(Style), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(null));
			BorderTemplateProperty = DependencyPropertyManager.Register("BorderTemplate", typeof(ControlTemplate), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(null));
			ExpanderStyleProperty = DependencyPropertyManager.Register("ExpanderStyle", typeof(Style), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(null));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(null));
			ContentStyleProperty = DependencyPropertyManager.Register("ContentStyle", typeof(Style), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(null));
			ItemsPresenterStyleProperty = DependencyPropertyManager.Register("ItemsPresenterStyle", typeof(Style), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(null));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(String.Empty));
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => { (d as DXTreeViewItemBase).Collapse(); }), new CoerceValueCallback(OnIsExpandedPropertyChangedCoerce)));
			CanExpandProperty = DependencyPropertyManager.Register("CanExpand", typeof(bool), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(false, (d, e) => { ((DXTreeViewItemBase)d).OnCanExpandChanged(); }, new CoerceValueCallback(OnCanExpandPropertyChangedCoerce)));
			TreeViewProperty = DependencyPropertyManager.Register("TreeView", typeof(DXTreeView), typeof(DXTreeViewItemBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTreeViewPropertyChanged)));
		}
		static object OnCanExpandPropertyChangedCoerce(DependencyObject o, object v) { return ((DXTreeViewItemBase)o).OnCanExpandPropertyChangedCoerce(v); }
		static object OnIsExpandedPropertyChangedCoerce(DependencyObject o, object v) { return ((DXTreeViewItemBase)o).OnIsExpandedPropertyChangedCoerce(v); }
		protected static void OnTreeViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXTreeViewItemBase)d).OnTreeViewChanged((DXTreeView)e.OldValue);
		}
		protected static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXTreeViewItemBase)d).OnIsSelectedChanged((bool)e.OldValue);
		}
		#endregion
		#region prop
		public bool CanExpand {
			get { return (bool)GetValue(CanExpandProperty); }
			set { SetValue(CanExpandProperty, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public Style ItemsPresenterStyle {
			get { return (Style)GetValue(ItemsPresenterStyleProperty); }
			set { SetValue(ItemsPresenterStyleProperty, value); }
		}
		public Style ContentStyle {
			get { return (Style)GetValue(ContentStyleProperty); }
			set { SetValue(ContentStyleProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public Style ExpanderStyle {
			get { return (Style)GetValue(ExpanderStyleProperty); }
			set { SetValue(ExpanderStyleProperty, value); }
		}
		public Style BorderStyle {
			get { return (Style)GetValue(BorderStyleProperty); }
			set { SetValue(BorderStyleProperty, value); }
		}
		public ControlTemplate BorderTemplate {
			get { return (ControlTemplate)GetValue(BorderTemplateProperty); }
			set { SetValue(BorderTemplateProperty, value); }
		}
		public DXTreeView TreeView {
			get { return (DXTreeView)GetValue(TreeViewProperty); }
			set { SetValue(TreeViewProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public bool AllowAnimation {
			get { return (bool)GetValue(AllowAnimationProperty); }
			set { SetValue(AllowAnimationProperty, value); }
		}
		public DXTreeViewItemBase ParentTreeViewItem { get; set; }
		protected internal RibbonCheckedBorderControl ExpandButton { get; private set; }
		protected internal RibbonCheckedBorderControl CheckedBorder { get; private set; }
		#endregion
		public DXTreeViewItemBase() {
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
			Focusable = false;
		}		
		protected internal virtual void OnCanExpandChanged() { }
		protected internal virtual void Collapse() {
			foreach(object obj in Items) {
				if(obj is DXTreeViewItemBase) {
					(obj as DXTreeViewItemBase).IsExpanded = false;
				}
			}			
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
		protected internal virtual void Remove() {
			ItemsControl control = ParentTreeViewItem == null ? TreeView as ItemsControl : ParentTreeViewItem as ItemsControl;
			IList targetCollection = control.ItemsSource == null ? control.Items as IList : control.ItemsSource as IList;
			targetCollection.Remove(this);
		}
		protected internal virtual List<DXTreeViewItemBase> GetDXTreeViewItems(bool onlyIfIsExpanded) {
			List<DXTreeViewItemBase> retVal = new List<DXTreeViewItemBase>();
			retVal.Add(this);
			if(IsExpanded || !onlyIfIsExpanded) {
				foreach(object obj in Items) {
					DXTreeViewItemBase item = obj as DXTreeViewItemBase;
					if(item != null) {
						retVal.AddRange(item.GetDXTreeViewItems(onlyIfIsExpanded));
					}
				}
			}
			return retVal;
		}
		protected virtual object OnCanExpandPropertyChangedCoerce(object v) {
			return v;
		}
		protected virtual object OnIsExpandedPropertyChangedCoerce(object v) {
			return v;
		}
		protected virtual void OnTreeViewChanged(DXTreeView oldValue) {
			foreach(object obj in Items) {
				if(obj is DXTreeViewItem) {
					(obj as DXTreeViewItemBase).TreeView = TreeView;
				}
			}
		}
		protected virtual void OnIsSelectedChanged(bool oldValue) {			
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			UnSubscribeEventHandlers();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UnSubscribeEventHandlers();
			SubscribeEventHandlers();
		}
		void OnExpandButtonClick(object sender, EventArgs e) {
			IsExpanded = !IsExpanded;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnSubscribeEventHandlers();
			ExpandButton = GetTemplateChild("PART_Expander") as RibbonCheckedBorderControl;
			CheckedBorder = GetTemplateChild("PART_ContentAndGlyph") as RibbonCheckedBorderControl;			
			SubscribeEventHandlers();
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
		}		
		private WeakReference lastRemovedItem = new WeakReference(null);
		public DXTreeViewItemBase LastRemovedItem {
			get { return (DXTreeViewItemBase)lastRemovedItem.Target; }
			set { lastRemovedItem = new WeakReference(value); }
		}
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Remove && TreeView != null) {
				foreach(var oldItem in e.OldItems) {
					DXTreeViewItemBase itemBase = IsItemItsOwnContainerOverride(oldItem) ? (DXTreeViewItemBase)oldItem : (DXTreeViewItemBase)ItemContainerGenerator.ContainerFromItem(oldItem);
					if(itemBase == TreeView.SelectedItem) {
						LastRemovedItem = itemBase;						
						TreeView.SelectedItem = null;
						if (TreeView.SelectedItem == null)
							TreeView.SelectedItem = this;
					}
				}
			}
			if(e.NewItems == null && e.Action != NotifyCollectionChangedAction.Reset) return;
			System.Collections.IList items = e.Action == NotifyCollectionChangedAction.Reset ? Items : e.NewItems;
			foreach(object obj in items) {
				if(obj is DXTreeViewItem) {
					(obj as DXTreeViewItemBase).ParentTreeViewItem = this;
					(obj as DXTreeViewItemBase).TreeView = TreeView;
				}
				if (obj == LastRemovedItem) {
					TreeView.SelectedItem = (DXTreeViewItemBase)obj;
					LastRemovedItem = null;
				}
			}
			if(Items.Count != 0) {
				CanExpand = true;
				return;
			}
			CanExpand = false;
		}
		protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue) {
			OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
			(element as DXTreeViewItemBase).TreeView = TreeView;
			BindingOperations.SetBinding(element, DXTreeViewItemBase.AllowAnimationProperty, new Binding() { Source = this, Path = new PropertyPath("AllowAnimation") });
			TreeView.SubscribeToItemEvents(element as DXTreeViewItemBase);
			base.PrepareContainerForItemOverride(element, item);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			if(TreeView != null)
				TreeView.UnSubscribeFromItemEvents(element as DXTreeViewItemBase);
			(element as DXTreeViewItemBase).TreeView = null;
			(element as DXTreeViewItemBase).ParentTreeViewItem = null;			
			base.ClearContainerForItemOverride(element, item);
			if(Items.Count == 0) CanExpand = false;
		}
		void SubscribeEventHandlers() {
			if(ExpandButton != null)
				ExpandButton.Click += new EventHandler(OnExpandButtonClick);
		}
		void UnSubscribeEventHandlers() {
			if(ExpandButton != null)
				ExpandButton.Click -= OnExpandButtonClick;
		}
		object ICloneable.Clone() {
			return Clone();
		}
		protected virtual DXTreeViewItemBase CreateClone() {
			return new DXTreeViewItem();
		}
		public virtual DXTreeViewItemBase Clone() {
			var item = CreateClone();
			item.ItemTemplate = ItemTemplate;
			item.ItemTemplateSelector = ItemTemplateSelector;
			DXTreeView.CloneItems(this, item);
			return item;
		}
	}	
}
