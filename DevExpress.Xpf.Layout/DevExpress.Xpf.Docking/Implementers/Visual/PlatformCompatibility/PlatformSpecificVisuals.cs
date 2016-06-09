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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class psvContentPresenter : ContentPresenter, IDisposable {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ContentInternalProperty;
		static psvContentPresenter() {
			var dProp = new DependencyPropertyRegistrator<psvContentPresenter>();
			dProp.Register("ContentInternal", ref ContentInternalProperty, (object)null,
				(dObj, e) => ((psvContentPresenter)dObj).OnContentChanged(e.NewValue, e.OldValue));
		}
		#endregion static
		public psvContentPresenter() {
			Focusable = false;
			this.StartListen(ContentInternalProperty, "Content");
			Loaded += psvContentPresenter_Loaded;
		}
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				Loaded -= psvContentPresenter_Loaded;
				ClearValue(ContentInternalProperty);
				ClearValue(ContentProperty);
				OnDispose();
				DockLayoutManager.Release(this);
			}
			GC.SuppressFinalize(this);
		}
		void psvContentPresenter_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DockLayoutManager.Ensure(this);
		}
		protected virtual void OnDispose() { }
		protected virtual void OnLoaded() { }
		protected virtual void OnContentChanged(object content, object oldContent) { }
	}
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	public class psvItemsControl : ItemsControl, IDisposable {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty HasItemsInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ActualSizeProperty;
		static psvItemsControl() {
			var dProp = new DependencyPropertyRegistrator<psvItemsControl>();
			dProp.Register("HasItemsInternal", ref HasItemsInternalProperty, (bool)false,
				(dObj, e) => ((psvItemsControl)dObj).OnHasItemsChanged((bool)e.NewValue));
			dProp.Register("ActualSize", ref ActualSizeProperty, Size.Empty,
				(dObj, e) => ((psvItemsControl)dObj).OnActualSizeChanged((Size)e.NewValue));
		}
		#endregion static
		public psvItemsControl() {
			Focusable = false;
			IsTabStop = false;
			this.StartListen(HasItemsInternalProperty, "HasItems");
			Loaded += psvItemsControl_Loaded;
			Unloaded += psvItemsControl_Unloaded;
		}
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				Loaded -= psvItemsControl_Loaded;
				Unloaded -= psvItemsControl_Unloaded;
				UnsubscribeUpdateLayout();
				ClearValue(HasItemsInternalProperty);
				OnDispose();
				ClearItemsSource();
				ClearValue(ActualSizeProperty);
				DockLayoutManager.Release(this);
				if(PartItemsPanel != null)
					ReleaseItemsPanelCore(PartItemsPanel);
				PartItemsPanel = null;
				PartItemsPresenter = null;
				Container = null;
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void ClearItemsSource() {
			ClearValue(ItemsSourceProperty);
		}
		public static void Clear(psvItemsControl itemsControl) {
			if(itemsControl != null)
				itemsControl.ClearItemsSource();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			PrepareContainer(element, item);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			ClearContainer(element);
			base.ClearContainerForItemOverride(element, item);
		}
		protected virtual void PrepareContainer(DependencyObject element, object item) {
		}
		protected virtual void ClearContainer(DependencyObject element) { }
		void psvItemsControl_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		void psvItemsControl_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
		}
		protected sealed override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			SetValue(ActualSizeProperty, sizeInfo.NewSize);
			base.OnRenderSizeChanged(sizeInfo);
		}
		protected sealed override void OnInitialized(System.EventArgs e) {
			base.OnInitialized(e);
			OnInitialized();
		}
		protected ItemsPresenter PartItemsPresenter { get; private set; }
		protected Panel PartItemsPanel { get; private set; }
		protected DockLayoutManager Container { get; private set; }
		void SubscribeUpdateLayout() {
			if(PartItemsPresenter != null) {
				PartItemsPresenter.LayoutUpdated += OnLayoutUpdated;
				PartItemsPresenter.SizeChanged += OnLayoutUpdated;
			}
		}
		void UnsubscribeUpdateLayout() {
			if(PartItemsPresenter != null) {
				PartItemsPresenter.LayoutUpdated -= OnLayoutUpdated;
				PartItemsPresenter.SizeChanged -= OnLayoutUpdated;
			}
		}
		public override void OnApplyTemplate() {
			ClearTemplateChildren();
			base.OnApplyTemplate();
			Container = DockLayoutManager.Ensure(this);
			PartItemsPresenter = LayoutItemsHelper.GetTemplateChild<ItemsPresenter>(this);
			SubscribeUpdateLayout();
		}
		protected virtual void ClearTemplateChildren() {
			UnsubscribeUpdateLayout();
			if(PartItemsPanel != null && !LayoutItemsHelper.IsTemplateChild(PartItemsPanel, this)) {
				ReleaseItemsPanelCore(PartItemsPanel);
				PartItemsPanel = null;
			}
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			UnsubscribeUpdateLayout();
			if(PartItemsPresenter != null) {
				PartItemsPanel = LayoutItemsHelper.GetTemplateChild<Panel>(PartItemsPresenter);
				if(PartItemsPanel != null) {
					EnsureItemsPanelCore(PartItemsPanel);
				}
			}
		}
		protected virtual void ReleaseItemsPanelCore(Panel itemsPanel) { }
		protected virtual void EnsureItemsPanelCore(Panel itemsPanel) { }
		protected virtual void OnDispose() { }
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		protected virtual void OnHasItemsChanged(bool hasItems) { }
		protected virtual void OnActualSizeChanged(Size value) { }
		protected virtual void OnInitialized() { }
	}
	public class psvControl : Control, IDisposable {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ActualSizeProperty;
		static psvControl() {
			var dProp = new DependencyPropertyRegistrator<psvControl>();
			dProp.Register("ActualSize", ref ActualSizeProperty, Size.Empty,
				(dObj, e) => ((psvControl)dObj).OnActualSizeChanged((Size)e.NewValue));
		}
		#endregion static
		public psvControl() {
			Focusable = false;
			IsTabStop = false;
			Loaded += psvControl_Loaded;
			Unloaded += psvControl_Unloaded;
		}
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				Loaded -= psvControl_Loaded;
				Unloaded -= psvControl_Unloaded;
				ClearValue(ActualSizeProperty);
				OnDispose();
				DockLayoutManager.Release(this);
				Container = null;
			}
			GC.SuppressFinalize(this);
		}
		void psvControl_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		void psvControl_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
		}
		protected sealed override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			SetValue(ActualSizeProperty, sizeInfo.NewSize);
			base.OnRenderSizeChanged(sizeInfo);
		}
		protected sealed override void OnInitialized(System.EventArgs e) {
			base.OnInitialized(e);
			OnInitialized();
		}
		int preparingContainerForItem = 0;
		protected bool IsPreparingContainerForItem {
			get { return preparingContainerForItem > 0; }
		}
		public void BeginPrepareContainer() {
			++preparingContainerForItem;
		}
		public void EndPrepareContainer() {
			if(--preparingContainerForItem == 0) {
				OnPrepareContainerForItemComplete();
			}
		}
		protected DockLayoutManager Container { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Container = DockLayoutManager.Ensure(this);
		}
		protected virtual void OnDispose() { }
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		protected virtual void OnPrepareContainerForItemComplete() { }
		protected virtual void OnInitialized() { }
		protected virtual void OnActualSizeChanged(Size value) { }
	}
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	public class psvContentControl : ContentControl, IDisposable {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ActualSizeProperty;
		static readonly DependencyPropertyKey LayoutItemPropertyKey;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty LayoutItemProperty;
		static psvContentControl() {
			var dProp = new DependencyPropertyRegistrator<psvContentControl>();
			dProp.Register("ActualSize", ref ActualSizeProperty, Size.Empty,
				(dObj, e) => ((psvContentControl)dObj).OnActualSizeChanged((Size)e.NewValue));
			dProp.RegisterReadonly("LayoutItem", ref LayoutItemPropertyKey, ref LayoutItemProperty, (BaseLayoutItem)null,
				(dObj, e) => ((psvContentControl)dObj).OnLayoutItemChanged((BaseLayoutItem)e.NewValue, (BaseLayoutItem)e.OldValue));
		}
		#endregion static
		public psvContentControl() {
			Focusable = false;
			Loaded += psvContentControl_Loaded;
			Unloaded += psvContentControl_Unloaded;
		}
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				Loaded -= psvContentControl_Loaded;
				Unloaded -= psvContentControl_Unloaded;
				ClearValue(ActualSizeProperty);
				OnDispose();
				ClearValue(LayoutItemPropertyKey);
				ClearValue(ContentProperty);
				DockLayoutManager.Release(this);
				Container = null;
			}
			GC.SuppressFinalize(this);
		}
		void psvContentControl_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		void psvContentControl_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
		}
		protected sealed override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			SetValue(ActualSizeProperty, sizeInfo.NewSize);
			base.OnRenderSizeChanged(sizeInfo);
		}
		public static void EnsureContentElement<T>(DependencyObject element, ContentPresenter presenter) where T : psvContentControl {
			if(element == null) return;
			T owner = LayoutItemsHelper.GetTemplateParent<T>(presenter);
			if(owner != null)
				owner.EnsureContentElementCore(element);
		}
		protected virtual void EnsureContentElementCore(DependencyObject element) { }
		protected override void OnContentChanged(object oldContent, object newContent) {
			LayoutItem = LayoutItemData.ConvertToBaseLayoutItem(newContent);
			base.OnContentChanged(oldContent, newContent);
		}
		public BaseLayoutItem LayoutItem {
			get { return (BaseLayoutItem)GetValue(LayoutItemProperty); }
			internal set { SetValue(LayoutItemPropertyKey, value); }
		}
		protected virtual void Subscribe(BaseLayoutItem item) { }
		protected virtual void Unsubscribe(BaseLayoutItem item) { }
		protected virtual void OnLayoutItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			Unsubscribe(oldItem);
			if(item == null) {
				ClearValue(DockLayoutManager.LayoutItemProperty);
			}
			else SetValue(DockLayoutManager.LayoutItemProperty, item);
			EnsureUIElements(item, oldItem);
			Subscribe(item);
		}
		void EnsureUIElements(BaseLayoutItem item, BaseLayoutItem oldItem) {
			Layout.Core.IUIElement uiElement = this as Layout.Core.IUIElement;
			if(uiElement != null) {
				if(oldItem != null) oldItem.UIElements.Remove(uiElement);
				if(item != null) item.UIElements.Add(uiElement);
			}
		}
		protected DockLayoutManager Container { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Container = DockLayoutManager.Ensure(this);
		}
		protected virtual void OnDispose() {
			Unsubscribe(LayoutItem);
		}
		protected virtual void OnLoaded() {
		}
		protected virtual void OnUnloaded() { }
		protected virtual void OnActualSizeChanged(Size value) { }
	}
	public class psvHeaderedContentControl : psvContentControl {
		#region static
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		static psvHeaderedContentControl() {
			var dProp = new DependencyPropertyRegistrator<psvHeaderedContentControl>();
			dProp.Register("Header", ref HeaderProperty, (object)null,
				(d, e) => ((psvHeaderedContentControl)d).OnHeaderChanged(e.OldValue, e.NewValue));
			dProp.Register("HeaderTemplate", ref HeaderTemplateProperty, (DataTemplate)null,
				(d, e) => ((psvHeaderedContentControl)d).OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue));
		}
		#endregion
		protected override void OnDispose() {
			ClearValue(HeaderProperty);
			ClearValue(HeaderTemplateProperty);
			base.OnDispose();
		}
		public object Header {
			get { return GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		protected virtual void OnHeaderChanged(object oldHeader, object newHeader) { }
		protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate) { }
	}
	public class psvPanel : Panel, IDisposable {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ActualSizeProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty DataContextInternalProperty;
		static psvPanel() {
			var dProp = new DependencyPropertyRegistrator<psvPanel>();
			dProp.Register("ActualSize", ref ActualSizeProperty, Size.Empty,
				(dObj, e) => ((psvPanel)dObj).OnActualSizeChanged((Size)e.NewValue));
			dProp.Register("DataContextInternal", ref DataContextInternalProperty, (object)null,
				(dObj, e) => ((psvPanel)dObj).OnDataContextChanged(e.NewValue, e.OldValue));
		}
		#endregion static
		public psvPanel() {
			Focusable = false;
			SetBinding(DataContextInternalProperty, new System.Windows.Data.Binding());
			Loaded += psvPanel_Loaded;
		}
		void psvPanel_Loaded(object sender, RoutedEventArgs e) {
			OnLoaded();
		}
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				Loaded -= psvPanel_Loaded;
				ClearValue(ActualSizeProperty);
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		protected sealed override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			SetValue(ActualSizeProperty, sizeInfo.NewSize);
			base.OnRenderSizeChanged(sizeInfo);
		}
		protected virtual void OnLoaded() { }
		protected virtual void OnDispose() { }
		protected virtual void OnActualSizeChanged(Size value) { }
		protected virtual void OnDataContextChanged(object newValue, object oldValue) { }
	}
	public class psvStackPanel : StackPanel {
		public psvStackPanel() {
			Focusable = false;
		}
	}
	public class psvGrid : Grid, IDisposable {
		public psvGrid() {
			Focusable = false;
			Loaded += psvGrid_Loaded;
			Unloaded += psvGrid_Unloaded;
		}
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				Loaded -= psvGrid_Loaded;
				Unloaded -= psvGrid_Unloaded;
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		void psvGrid_Loaded(object sender, RoutedEventArgs e) {
			IsLoadedComplete = true;
			OnLoaded();
		}
		void psvGrid_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
			IsLoadedComplete = false;
		}
		protected internal bool IsLoadedComplete { get; set; }
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		protected virtual void OnDispose() { }
	}
	public class psvCaption : ContentControl {
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty TextTrimmingProperty;
		public static readonly DependencyProperty TextWrappingProperty;
		public static readonly DependencyProperty RecognizeAccessKeyProperty;
		static psvCaption() {
			var dProp = new DependencyPropertyRegistrator<psvCaption>();
			dProp.Register("Text", ref TextProperty, (string)null);
			dProp.Register("TextTrimming", ref TextTrimmingProperty, TextTrimming.None);
			dProp.Register("TextWrapping", ref TextWrappingProperty, TextWrapping.NoWrap);
			dProp.Register("RecognizeAccessKey", ref RecognizeAccessKeyProperty, false,
				(dObj, e) => ((psvCaption)dObj).OnRecognizeAccessKeyChanged((bool)e.OldValue, (bool)e.NewValue));
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public TextTrimming TextTrimming {
			get { return (TextTrimming)GetValue(TextTrimmingProperty); }
			set { SetValue(TextTrimmingProperty, value); }
		}
		public TextWrapping TextWrapping {
			get { return (TextWrapping)GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}
		protected TextBlock PartTextBlock;
		public psvCaption() {
			PartTextBlock = new TextBlock();
			this.Forward(PartTextBlock, TextBlock.TextProperty, "Text");
			this.Forward(PartTextBlock, TextBlock.TextTrimmingProperty, "TextTrimming");
			this.Forward(PartTextBlock, TextBlock.TextWrappingProperty, "TextWrapping");
			PartAccessText = new AccessText();
			this.Forward(PartAccessText, AccessText.TextProperty, "Text");
			this.Forward(PartAccessText, AccessText.TextTrimmingProperty, "TextTrimming");
			this.Forward(PartAccessText, AccessText.TextWrappingProperty, "TextWrapping");
			Content = ActualContent;
			IsTabStop = false;
		}
		FrameworkElement ActualContent { get { return RecognizeAccessKey ? (FrameworkElement)PartAccessText : PartTextBlock; } }
		protected AccessText PartAccessText;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			BaseLayoutItem item = DataContext as BaseLayoutItem;
			RecognizeAccessKey = LayoutItemsHelper.CanRecognizeAccessKey(item);
		}
		public bool RecognizeAccessKey {
			get { return (bool)GetValue(RecognizeAccessKeyProperty); }
			set { SetValue(RecognizeAccessKeyProperty, value); }
		}
		protected virtual void OnRecognizeAccessKeyChanged(bool oldValue, bool newValue) {
			Content = ActualContent;
		}
	}
	public class psvTabCaption : TextBlock {
	}
	public class psvSelector<T> : psvItemsControl where T : class {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ItemsSourceInternalProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty SelectedIndexProperty;
		static psvSelector() {
			var dProp = new DependencyPropertyRegistrator<psvSelector<T>>();
			dProp.Register("SelectedItem", ref SelectedItemProperty, (T)null,
				(dObj, e) => ((psvSelector<T>)dObj).OnSelectedItemChanged((T)e.NewValue, (T)e.OldValue),
				(dObj, value) => ((psvSelector<T>)dObj).CoerceSelectedItem((T)value));
			dProp.Register("SelectedIndex", ref SelectedIndexProperty, -1,
				(dObj, e) => ((psvSelector<T>)dObj).OnSelectedIndexChanged((int)e.NewValue, (int)e.OldValue),
				(dObj, value) => ((psvSelector<T>)dObj).CoerceSelectedIndex((int)value));
			dProp.Register("ItemsSourceInternal", ref ItemsSourceInternalProperty, (IEnumerable)null,
				(dObj, e) => ((psvSelector<T>)dObj).OnItemsSourceInternalChanged());
		}
		#endregion
		public psvSelector() {
			this.StartListen(ItemsSourceInternalProperty, "ItemsSource");
		}
		protected override void OnDispose() {
			deferredSelectedIndex = -1;
			base.OnDispose();
		}
		int syncWithCurrent = 0;
		void OnItemsSourceInternalChanged() {
			if(syncWithCurrent > 0) return;
			bool deferredSelection = IsValidIndex(deferredSelectedIndex);
			if(!deferredSelection) syncWithCurrent++;
			CoerceValue(SelectedIndexProperty);
			CoerceValue(SelectedItemProperty);
			deferredSelectedIndex = -1;
			if(!deferredSelection) syncWithCurrent--;
		}
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			CheckSelectedItemRemoved(e);
		}
		void CheckSelectedItemRemoved(NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Remove) {
				if(e.OldItems.Contains(SelectedItem)) {
					CoerceValue(SelectedItemProperty);
				}
			}
		}
		protected virtual void OnSelectedItemChanged(T item, T oldItem) {
			int index;
			if(IsValidItem(item, out index)) {
				ChangeSelection(oldItem, false);
				SetValue(SelectedIndexProperty, index);
				ChangeSelection(item, true);
			}
			else CoerceValue(SelectedIndexProperty);
		}
		protected virtual void OnSelectedIndexChanged(int index, int oldIndex) {
			if(IsValidIndex(index))
				SetValue(SelectedItemProperty, Items[index]);
			else ClearValue(SelectedItemProperty);
		}
		protected virtual T CoerceSelectedItem(T item) {
			if(syncWithCurrent > 0)
				return GetCurrentItem();
			if(!Items.Contains(item)) {
				if(Items.Contains(SelectedItem)) {
					return SelectedItem;
				}
				else {
					return IsValidIndex(SelectedIndex) ? (T)Items[SelectedIndex] : null;
				}
			}
			return item;
		}
		protected virtual object CoerceSelectedIndex(int index) {
			if(TrySetDeferredSelectedIndex(ref index))
				return index;
			if(syncWithCurrent > 0)
				return GetCurrentPosition();
			if(!IsValidIndex(index)) {
				if(IsValidIndex(index) && Items.Count > 0 && SelectedIndex != index) {
					return SelectedIndex;
				}
				else {
					return Items.IndexOf(SelectedItem);
				}
			}
			return index;
		}
		int GetCurrentPosition() {
			return Items.CurrentPosition;
		}
		T GetCurrentItem() {
			return (T)Items.CurrentItem;
		}
		int deferredSelectedIndex = -1;
		bool TrySetDeferredSelectedIndex(ref int index) {
			if(IsDisposing) return false;
			if(deferredSelectedIndex == -1) {
				if(ItemsSource == null) {
					deferredSelectedIndex = index;
					index = -1;
					return true;
				}
			}
			else {
				if(ItemsSource != null) {
					index = deferredSelectedIndex;
					deferredSelectedIndex = -1;
					return true;
				}
			}
			return false;
		}
		bool IsValidItem(T item, out int index) {
			index = Items.IndexOf(item);
			return index != -1;
		}
		bool IsValidIndex(int index) {
			return index >= 0 && index < Items.Count;
		}
		public T SelectedItem {
			get { return (T)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public int SelectedIndex {
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
		void ChangeSelection(T item, bool selected) {
			var selectorItem = ItemContainerGenerator.ContainerFromItem(item) as psvSelectorItem;
			if(selectorItem != null)
				selectorItem.IsSelected = selected;
		}
		protected sealed override bool IsItemItsOwnContainerOverride(object item) {
			return item is psvSelectorItem;
		}
		protected sealed override DependencyObject GetContainerForItemOverride() {
			return CreateSelectorItem();
		}
		protected sealed override void PrepareContainer(DependencyObject element, object item) {
			base.PrepareContainer(element, item);
			psvSelectorItem selectorItem = element as psvSelectorItem;
			if(selectorItem != null) {
				selectorItem.Content = item;
				selectorItem.IsSelected = object.Equals(SelectedItem, item);
				PrepareSelectorItem(selectorItem, (T)item);
			}
		}
		protected sealed override void ClearContainer(DependencyObject element) {
			psvSelectorItem selectorItem = element as psvSelectorItem;
			if(selectorItem != null) {
				selectorItem.IsSelected = false;
				ClearSelectorItem(selectorItem);
				selectorItem.Dispose();
			}
		}
		protected virtual psvSelectorItem CreateSelectorItem() {
			return new psvSelectorItem();
		}
		protected virtual void ClearSelectorItem(psvSelectorItem selectorItem) { }
		protected virtual void PrepareSelectorItem(psvSelectorItem selectorItem, T item) { }
	}
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	public class psvContentSelectorControl<T> : psvSelector<T> where T : class {
		#region static
		static readonly DependencyPropertyKey SelectedContentPropertyKey;
		public static readonly DependencyProperty SelectedContentProperty;
		public static readonly DependencyProperty SelectedContentTemplateProperty;
		static psvContentSelectorControl() {
			var dProp = new DependencyPropertyRegistrator<psvContentSelectorControl<T>>();
			dProp.RegisterReadonly("SelectedContent", ref SelectedContentPropertyKey, ref SelectedContentProperty, (T)null,
				(dObj, e) => ((psvContentSelectorControl<T>)dObj).OnSelectedContentChanged((T)e.NewValue, (T)e.OldValue),
				(dObj, value) => ((psvContentSelectorControl<T>)dObj).CoerceSelectedContent((T)value));
			dProp.Register("SelectedContentTemplate", ref SelectedContentTemplateProperty, (DataTemplate)null,
				(dObj, e) => ((psvContentSelectorControl<T>)dObj).OnSelectedContentTemplateChanged());
		}
		#endregion static
		protected override void OnDispose() {
			if(PartSelectedContentPresenter != null) {
				PartSelectedContentPresenter.Dispose();
				PartSelectedContentPresenter = null;
			}
			base.OnDispose();
		}
		protected override void OnSelectedItemChanged(T item, T oldItem) {
			base.OnSelectedItemChanged(item, oldItem);
			CoerceValue(SelectedContentProperty);
		}
		protected virtual T CoerceSelectedContent(T value) {
			return SelectedItem;
		}
		public T SelectedContent {
			get { return (T)GetValue(SelectedContentProperty); }
		}
		public DataTemplate SelectedContentTemplate {
			get { return (DataTemplate)GetValue(SelectedContentTemplateProperty); }
			set { SetValue(SelectedContentTemplateProperty, value); }
		}
		protected psvContentPresenter PartSelectedContentPresenter { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartSelectedContentPresenter = LayoutItemsHelper.GetTemplateChild<psvContentPresenter>(this);
			if(PartSelectedContentPresenter != null) {
				this.Forward(PartSelectedContentPresenter, ContentPresenter.ContentTemplateProperty, "SelectedContentTemplate");
			}
		}
		protected virtual void OnSelectedContentChanged(T newValue, T oldValue) { }
		protected virtual void OnSelectedContentTemplateChanged() { }
	}
	public class psvSelectorItem : psvContentControl {
		#region static
		static readonly DependencyPropertyKey IsSelectedPropertyKey;
		public static readonly DependencyProperty IsSelectedProperty;
		static psvSelectorItem() {
			var dProp = new DependencyPropertyRegistrator<psvSelectorItem>();
			dProp.RegisterReadonly("IsSelected", ref IsSelectedPropertyKey, ref IsSelectedProperty, false,
				(dObj, e) => ((psvSelectorItem)dObj).OnIsSelectedChanged((bool)e.NewValue));
		}
		#endregion static
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			internal set { SetValue(IsSelectedPropertyKey, value); }
		}
		protected virtual void OnIsSelectedChanged(bool selected) { }
	}
	public class psvDecorator : psvPanel {
		public static readonly DependencyProperty ChildProperty;
		static psvDecorator() {
			var dProp = new DependencyPropertyRegistrator<psvDecorator>();
			dProp.Register("Child", ref ChildProperty, (UIElement)null,
				(dObj, e) => ((psvDecorator)dObj).OnChildChanged((UIElement)e.NewValue, (UIElement)e.OldValue));
		}
		public UIElement Child {
			get { return (UIElement)GetValue(ChildProperty); }
			set { SetValue(ChildProperty, value); }
		}
		protected virtual void OnChildChanged(UIElement child, UIElement oldChild) {
			if(oldChild != null)
				Children.Remove(oldChild);
			if(child != null)
				Children.Add(child);
		}
		protected override Size MeasureOverride(Size constraint) {
			UIElement child = this.Child;
			if(child != null) {
				child.Measure(constraint);
				return child.DesiredSize;
			}
			return new Size();
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			Rect finalRect = new Rect(new Point(), arrangeSize);
			UIElement child = this.Child;
			if(child != null) {
				child.Arrange(finalRect);
			}
			return arrangeSize;
		}
	}
	public class psvDockPanel : DockPanel {
	}
	public class psvListBox : ListBox {
		protected override DependencyObject GetContainerForItemOverride() {
			return new psvListBoxItem();
		}
	}
	public class psvListBoxItem : ListBoxItem {
	}
	public class ScrollablePanel : psvPanel, IScrollInfo {
		static double ComputeScrollOffset(double topView, double bottomView, double topChild, double bottomChild) {
			bool fBottom = topChild < topView && bottomChild < bottomView;
			bool fTop = bottomChild > bottomView && topChild > topView;
			bool fSize = (bottomChild - topChild) > (bottomView - topView);
			if(!fBottom && !fTop) 
				return topView;
			if((fBottom && !fSize) || (fTop && fSize))
				return topChild;
			return (bottomChild - (bottomView - topView));
		}
		const double LineSize = 16d;
		const double WheelSize = 48d;
		ScrollData scrollData;
		protected Size Viewport { get { return scrollData._viewport; } }
		public ScrollablePanel() {
			scrollData = new ScrollData();
		}
		#region IScrollInfo Members
		public bool CanHorizontallyScroll { get; set; }
		public bool CanVerticallyScroll { get; set; }
		public double ExtentHeight { get { return scrollData._extent.Height; } }
		public double ExtentWidth { get { return scrollData._extent.Width; } }
		public double HorizontalOffset { get { return scrollData._offset.X; } }
		public void LineDown() {
			SetVerticalOffset(VerticalOffset + LineSize);
		}
		public void LineLeft() {
			SetHorizontalOffset(HorizontalOffset - LineSize);
		}
		public void LineRight() {
			SetHorizontalOffset(HorizontalOffset + LineSize);
		}
		public void LineUp() {
			SetVerticalOffset(VerticalOffset - LineSize);
		}
		public Rect MakeVisible(System.Windows.Media.Visual visual, Rect rectangle) {
			if(rectangle.IsEmpty || visual == null || !IsAncestorOf(visual)) return Rect.Empty;
			rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);
			return MakeVisibleCore(rectangle);
		}
		Rect MakeVisibleCore(Rect rectangle) {
			Rect viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
			rectangle.X += viewRect.X;
			rectangle.Y += viewRect.Y;
			viewRect.X = ComputeScrollOffset(viewRect.Left, viewRect.Right, rectangle.Left, rectangle.Right);
			viewRect.Y = ComputeScrollOffset(viewRect.Top, viewRect.Bottom, rectangle.Top, rectangle.Bottom);
			SetHorizontalOffset(viewRect.X);
			SetVerticalOffset(viewRect.Y);
			rectangle.Intersect(viewRect);
			rectangle.X -= viewRect.X;
			rectangle.Y -= viewRect.Y;
			return rectangle;
		}
		protected void VerifyScrollData(Size viewport, Size extent) {
			if(double.IsInfinity(viewport.Width))
				viewport.Width = extent.Width;
			if(double.IsInfinity(viewport.Height))
				viewport.Height = extent.Height;
			scrollData._extent = extent;
			scrollData._viewport = viewport;
			scrollData._offset.X = Math.Max(0, Math.Min(scrollData._offset.X, ExtentWidth - ViewportWidth));
			scrollData._offset.Y = Math.Max(0, Math.Min(scrollData._offset.Y, ExtentHeight - ViewportHeight));
			if(ScrollOwner != null) 
				ScrollOwner.InvalidateScrollInfo();
		}
		public void MouseWheelDown() {
			SetVerticalOffset(VerticalOffset + WheelSize);
		}
		public void MouseWheelLeft() {
			SetHorizontalOffset(HorizontalOffset - WheelSize);
		}
		public void MouseWheelRight() {
			SetHorizontalOffset(HorizontalOffset + WheelSize);
		}
		public void MouseWheelUp() {
			SetVerticalOffset(VerticalOffset - WheelSize);
		}
		public void PageDown() {
			SetVerticalOffset(VerticalOffset + ViewportHeight);
		}
		public void PageLeft() {
			SetHorizontalOffset(HorizontalOffset - ViewportWidth);
		}
		public void PageRight() {
			SetHorizontalOffset(HorizontalOffset + ViewportWidth);
		}
		public void PageUp() {
			SetVerticalOffset(VerticalOffset - ViewportHeight);
		}
		public ScrollViewer ScrollOwner { get; set; }
		public void SetHorizontalOffset(double offset) {
			offset = Math.Max(0, Math.Min(offset, ExtentWidth - ViewportWidth));
			if(offset != scrollData._offset.X) {
				scrollData._offset.X = offset;
				InvalidateArrange();
			}
		}
		public void SetVerticalOffset(double offset) {
			offset = Math.Max(0, Math.Min(offset, ExtentHeight - ViewportHeight));
			if(offset != scrollData._offset.Y) {
				scrollData._offset.Y = offset;
				InvalidateArrange();
			}
		}
		public double VerticalOffset { get { return scrollData._offset.Y; } }
		public double ViewportHeight { get { return scrollData._viewport.Height; } }
		public double ViewportWidth { get { return scrollData._viewport.Width; } }
		#endregion
		class ScrollData {
			internal Point _offset;
			internal Size _viewport;
			internal Size _extent;
		}
	}
}
