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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Core.TabControlAutomation;
using DevExpress.Xpf.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core {
	#region Types
	public enum TabContentCacheMode { None, CacheAllTabs, CacheTabsOnSelecting }
	public class TabControlSelectionChangedEventArgs : EventArgs {
		public int OldSelectedIndex { get; private set; }
		public int NewSelectedIndex { get; private set; }
		public object OldSelectedItem { get; private set; }
		public object NewSelectedItem { get; private set; }
		public TabControlSelectionChangedEventArgs(int oldSelectedIndex, int newSelectedIndex, object oldSelectedItem, object newSeleсtedItem) {
			OldSelectedIndex = oldSelectedIndex;
			NewSelectedIndex = newSelectedIndex;
			OldSelectedItem = oldSelectedItem;
			NewSelectedItem = newSeleсtedItem;
		}
	}
	public class TabControlSelectionChangingEventArgs : CancelEventArgs {
		public int OldSelectedIndex { get; private set; }
		public int NewSelectedIndex { get; private set; }
		public object OldSelectedItem { get; private set; }
		public object NewSelectedItem { get; private set; }
		public TabControlSelectionChangingEventArgs(int oldSelectedIndex, int newSelectedIndex, object oldSelectedItem, object newSeleсtedItem) {
			OldSelectedIndex = oldSelectedIndex;
			NewSelectedIndex = newSelectedIndex;
			OldSelectedItem = oldSelectedItem;
			NewSelectedItem = newSeleсtedItem;
		}
	}
	public abstract class TabControlCommonEventArgsBase : EventArgs {
		public int TabIndex { get; private set; }
		public object Item { get; private set; }
		public TabControlCommonEventArgsBase(int index, object item) {
			TabIndex = index;
			Item = item;
		}
	}
	public abstract class TabControlCommonCancelEventArgsBase : CancelEventArgs {
		public int TabIndex { get; private set; }
		public object Item { get; private set; }
		public TabControlCommonCancelEventArgsBase(int index, object item) {
			TabIndex = index;
			Item = item;
		}
	}
	public class TabControlTabShownEventArgs : TabControlCommonEventArgsBase {
		public TabControlTabShownEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabShowingEventArgs : TabControlCommonCancelEventArgsBase {
		public TabControlTabShowingEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabHiddenEventArgs : TabControlCommonEventArgsBase {
		public TabControlTabHiddenEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabHidingEventArgs : TabControlCommonCancelEventArgsBase {
		public TabControlTabHidingEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabAddedEventArgs : TabControlCommonEventArgsBase {
		public TabControlTabAddedEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabAddingEventArgs : CancelEventArgs {
		public object Item { get; set; }
		public TabControlTabAddingEventArgs() { }
	}
	public class TabControlTabInsertedEventArgs : TabControlCommonEventArgsBase {
		public TabControlTabInsertedEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabInsertingEventArgs : TabControlCommonCancelEventArgsBase {
		public TabControlTabInsertingEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabRemovedEventArgs : TabControlCommonEventArgsBase {
		public TabControlTabRemovedEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabRemovingEventArgs : TabControlCommonCancelEventArgsBase {
		public TabControlTabRemovingEventArgs(int index, object item) : base(index, item) { }
	}
	public class TabControlTabMovingEventArgs : CancelEventArgs {
		public object Item { get; private set; }
		public int OldTabIndex { get; private set; }
		public int NewTabIndex { get; private set; }
		public TabControlTabMovingEventArgs(object item, int oldIndex, int newIndex) {
			Item = item;
			OldTabIndex = oldIndex;
			NewTabIndex = newIndex;
		}
	}
	public class TabControlTabMovedEventArgs : EventArgs {
		public object Item { get; private set; }
		public int OldTabIndex { get; private set; }
		public int NewTabIndex { get; private set; }
		public TabControlTabMovedEventArgs(object item, int oldIndex, int newIndex) {
			Item = item;
			OldTabIndex = oldIndex;
			NewTabIndex = newIndex;
		}
	}
	public delegate void TabControlSelectionChangingEventHandler(object sender, TabControlSelectionChangingEventArgs e);
	public delegate void TabControlSelectionChangedEventHandler(object sender, TabControlSelectionChangedEventArgs e);
	public delegate void TabControlTabShowingEventHandler(object sender, TabControlTabShowingEventArgs e);
	public delegate void TabControlTabShownEventHandler(object sender, TabControlTabShownEventArgs e);
	public delegate void TabControlTabHidingEventHandler(object sender, TabControlTabHidingEventArgs e);
	public delegate void TabControlTabHiddenEventHandler(object sender, TabControlTabHiddenEventArgs e);
	public delegate void TabControlTabAddingEventHandler(object sender, TabControlTabAddingEventArgs e);
	public delegate void TabControlTabAddedEventHandler(object sender, TabControlTabAddedEventArgs e);
	public delegate void TabControlTabInsertingEventHandler(object sender, TabControlTabInsertingEventArgs e);
	public delegate void TabControlTabInsertedEventHandler(object sender, TabControlTabInsertedEventArgs e);
	public delegate void TabControlTabRemovingEventHandler(object sender, TabControlTabRemovingEventArgs e);
	public delegate void TabControlTabRemovedEventHandler(object sender, TabControlTabRemovedEventArgs e);
	public delegate void TabControlTabMovingEventHandler(object sender, TabControlTabMovingEventArgs e);
	public delegate void TabControlTabMovedEventHandler(object sender, TabControlTabMovedEventArgs e);
	public delegate void PrepareTabItemDelegate(DXTabItem tab, object item);
	public class TabControlNewTabbedWindowEventArgs : EventArgs {
		public Window SourceWindow { get; private set; }
		public DXTabControl SourceTabControl { get; private set; }
		public Window NewWindow { get; set; }
		public DXTabControl NewTabControl { get; set; }
		public TabControlNewTabbedWindowEventArgs(Window sourceWindow, DXTabControl sourceTabControl, Window newWindow, DXTabControl newTabControl) {
			SourceWindow = sourceWindow;
			SourceTabControl = sourceTabControl;
			NewWindow = newWindow;
			NewTabControl = newTabControl;
		}
	}
	public delegate void TabControlNewTabbedWindowEventHandler(object sender, TabControlNewTabbedWindowEventArgs e);
	#endregion Types
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	[TemplatePart(Name = TabPanelName, Type = typeof(TabPanelContainer))]
	[DefaultEvent("SelectionChanged"), DefaultProperty("SelectedIndex")]
	public class DXTabControl : HeaderedSelectorBase<DXTabControl, DXTabItem>, ICloneable {
		#region Properties
		public const string FastRenderPanelName = "PART_FastRenderPanel";
		public const string ContentPresenterName = "PART_SelectedContentHost";
		public const string TabPanelName = "PART_TabPanelContainer";
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty PanelIndentProperty = DependencyProperty.Register("PanelIndent", typeof(Thickness), typeof(DXTabControl));
		public static readonly DependencyProperty BackgroundTemplateProperty = DependencyProperty.Register("BackgroundTemplate", typeof(DataTemplate), typeof(DXTabControl));
		public static readonly DependencyProperty ControlBoxLeftTemplateProperty = DependencyProperty.Register("ControlBoxLeftTemplate", typeof(DataTemplate), typeof(DXTabControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((DXTabControl)d).OnLogicalElementTemplateChanged(ControlBoxLeftProperty, ControlBoxLeftPropertyKey, (DataTemplate)e.NewValue)));
		public static readonly DependencyProperty ControlBoxRightTemplateProperty = DependencyProperty.Register("ControlBoxRightTemplate", typeof(DataTemplate), typeof(DXTabControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((DXTabControl)d).OnLogicalElementTemplateChanged(ControlBoxRightProperty, ControlBoxRightPropertyKey, (DataTemplate)e.NewValue)));
		public static readonly DependencyProperty ControlBoxPanelTemplateProperty = DependencyProperty.Register("ControlBoxPanelTemplate", typeof(DataTemplate), typeof(DXTabControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((DXTabControl)d).OnLogicalElementTemplateChanged(ControlBoxPanelProperty, ControlBoxPanelPropertyKey, (DataTemplate)e.NewValue)));
		public static readonly DependencyProperty ContentHeaderTemplateProperty = DependencyProperty.Register("ContentHeaderTemplate", typeof(DataTemplate), typeof(DXTabControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((DXTabControl)d).OnLogicalElementTemplateChanged(ContentHeaderProperty, ContentHeaderPropertyKey, (DataTemplate)e.NewValue)));
		public static readonly DependencyProperty ContentFooterTemplateProperty = DependencyProperty.Register("ContentFooterTemplate", typeof(DataTemplate), typeof(DXTabControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((DXTabControl)d).OnLogicalElementTemplateChanged(ContentFooterProperty, ContentFooterPropertyKey, (DataTemplate)e.NewValue)));
		public static readonly DependencyProperty ContentHostTemplateProperty = DependencyProperty.Register("ContentHostTemplate", typeof(DataTemplate), typeof(DXTabControl),
			new FrameworkPropertyMetadata(null, (d, e) => ((DXTabControl)d).OnLogicalElementTemplateChanged(ContentHostProperty, ContentHostPropertyKey, (DataTemplate)e.NewValue)));
		static readonly DependencyPropertyKey ControlBoxLeftPropertyKey = DependencyProperty.RegisterReadOnly("ControlBoxLeft", typeof(object), typeof(DXTabControl), null);
		static readonly DependencyPropertyKey ControlBoxRightPropertyKey = DependencyProperty.RegisterReadOnly("ControlBoxRight", typeof(object), typeof(DXTabControl), null);
		static readonly DependencyPropertyKey ControlBoxPanelPropertyKey = DependencyProperty.RegisterReadOnly("ControlBoxPanel", typeof(object), typeof(DXTabControl), null);
		static readonly DependencyPropertyKey ContentHeaderPropertyKey = DependencyProperty.RegisterReadOnly("ContentHeader", typeof(object), typeof(DXTabControl), null);
		static readonly DependencyPropertyKey ContentFooterPropertyKey = DependencyProperty.RegisterReadOnly("ContentFooter", typeof(object), typeof(DXTabControl), null);
		static readonly DependencyPropertyKey ContentHostPropertyKey = DependencyProperty.RegisterReadOnly("ContentHost", typeof(object), typeof(DXTabControl), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ControlBoxLeftProperty = ControlBoxLeftPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ControlBoxRightProperty = ControlBoxRightPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ControlBoxPanelProperty = ControlBoxPanelPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ContentHeaderProperty = ContentHeaderPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ContentFooterProperty = ContentFooterPropertyKey.DependencyProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ContentHostProperty = ContentHostPropertyKey.DependencyProperty;
		public static readonly DependencyProperty SelectedTabItemProperty = DependencyProperty.Register("SelectedTabItem", typeof(DXTabItem), typeof(DXTabControl),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((DXTabControl)d).OnSelectedTabItemChanged()));
		public static readonly DependencyProperty DestroyContentOnTabSwitchingProperty = DependencyProperty.Register("DestroyContentOnTabSwitching", typeof(bool), typeof(DXTabControl), new PropertyMetadata(true));
		public static readonly DependencyProperty TabContentCacheModeProperty = DependencyProperty.Register("TabContentCacheMode", typeof(TabContentCacheMode), typeof(DXTabControl), new PropertyMetadata(TabContentCacheMode.None));
		public static readonly DependencyProperty AllowMergingProperty = DependencyPropertyManager.Register("AllowMerging", typeof(bool?), typeof(DXTabControl), new FrameworkPropertyMetadata((bool?)null, FrameworkPropertyMetadataOptions.Inherits, (d, e) => ((DXTabControl)d).OnAllowMergingChanged()));
		static readonly DependencyPropertyKey ViewInfoPropertyKey = DependencyProperty.RegisterReadOnly("ViewInfo", typeof(TabViewInfo), typeof(DXTabControl), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ViewInfoProperty = ViewInfoPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey MenuInfoPropertyKey = DependencyProperty.RegisterReadOnly("MenuInfo", typeof(DXTabControlHeaderMenuInfo), typeof(DXTabControl), null);
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty MenuInfoProperty = MenuInfoPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View", typeof(TabControlViewBase), typeof(DXTabControl), new PropertyMetadata(null,
			(d, e) => ((DXTabControl)d).OnViewPropertyChanged((TabControlViewBase)e.OldValue, (TabControlViewBase)e.NewValue),
			(d, e) => ((DXTabControl)d).CoerceViewProperty(e as TabControlViewBase)));
		public DataTemplate BackgroundTemplate { get { return (DataTemplate)GetValue(BackgroundTemplateProperty); } set { SetValue(BackgroundTemplateProperty, value); } }
		public DataTemplate ControlBoxLeftTemplate { get { return (DataTemplate)GetValue(ControlBoxLeftTemplateProperty); } set { SetValue(ControlBoxLeftTemplateProperty, value); } }
		public DataTemplate ControlBoxRightTemplate { get { return (DataTemplate)GetValue(ControlBoxRightTemplateProperty); } set { SetValue(ControlBoxRightTemplateProperty, value); } }
		public DataTemplate ControlBoxPanelTemplate { get { return (DataTemplate)GetValue(ControlBoxPanelTemplateProperty); } set { SetValue(ControlBoxPanelTemplateProperty, value); } }
		public DataTemplate ContentHeaderTemplate { get { return (DataTemplate)GetValue(ContentHeaderTemplateProperty); } set { SetValue(ContentHeaderTemplateProperty, value); } }
		public DataTemplate ContentFooterTemplate { get { return (DataTemplate)GetValue(ContentFooterTemplateProperty); } set { SetValue(ContentFooterTemplateProperty, value); } }
		public DataTemplate ContentHostTemplate { get { return (DataTemplate)GetValue(ContentHostTemplateProperty); } set { SetValue(ContentHostTemplateProperty, value); } }
		[Obsolete("Use the SelectedContainer property.")]
		public DXTabItem SelectedTabItem { get { return (DXTabItem)GetValue(SelectedTabItemProperty); } set { SetValue(SelectedTabItemProperty, value); } }
		[Obsolete("Use the TabContentCacheMode property.")]
		public bool DestroyContentOnTabSwitching { get { return (bool)GetValue(DestroyContentOnTabSwitchingProperty); } set { SetValue(DestroyContentOnTabSwitchingProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabControlTabContentCacheMode")]
#endif
		public TabContentCacheMode TabContentCacheMode { get { return (TabContentCacheMode)GetValue(TabContentCacheModeProperty); } set { SetValue(TabContentCacheModeProperty, value); } }
		public bool? AllowMerging { get { return (bool?)GetValue(AllowMergingProperty); } set { SetValue(AllowMergingProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabControlView")]
#endif
		public TabControlViewBase View { get { return (TabControlViewBase)GetValue(ViewProperty); } set { SetValue(ViewProperty, value); } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabControlPrevButton")]
#endif
		public RepeatButton PrevButton { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabControlNextButton")]
#endif
		public RepeatButton NextButton { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabControlHeaderMenu")]
#endif
		public ToggleButton HeaderMenu { get; private set; }
		public Button CloseButton { get; private set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXTabControlTabPanel")]
#endif
		public TabPanelContainer TabPanel { get; private set; }
		ContentHostPresenter ContentHostPresenter { get; set; }
		protected internal FastRenderPanel FastRenderPanel { get { return ContentHostPresenter.With(x => x.ContentHostChild as FastRenderPanel); } }
		protected internal ContentPresenter ContentPresenter { get { return ContentHostPresenter.With(x => x.ContentHostChild as ContentPresenter); } }
		[EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public DXTabControlSerializationInfo SerializationInfo { get; private set; }
		#endregion Properties
		protected virtual DXTabControl CloneCore(DXTabControl tab) {
			DXTabControl res = new DXTabControl();
			res.Name = tab.Name;
			DXSerializer.SetEnabled(res, DXSerializer.GetEnabled(tab));
			res.View = tab.View;
			res.ControlBoxLeftTemplate = tab.ControlBoxLeftTemplate;
			res.ControlBoxRightTemplate = tab.ControlBoxRightTemplate;
			res.ContentHeaderTemplate = tab.ContentHeaderTemplate;
			res.ContentFooterTemplate = tab.ContentFooterTemplate;
			res.ContentHostTemplate = tab.ContentHostTemplate;
			res.TabContentCacheMode = tab.TabContentCacheMode;
			res.AllowMerging = tab.AllowMerging;
			res.Padding = tab.Padding;
			res.Margin = tab.Margin;
			res.SnapsToDevicePixels = tab.SnapsToDevicePixels;
			res.UseLayoutRounding = tab.UseLayoutRounding;
			res.ItemTemplate = tab.ItemTemplate;
			res.ItemTemplateSelector = tab.ItemTemplateSelector;
			res.ItemStringFormat = tab.ItemStringFormat;
			res.ItemHeaderTemplate = tab.ItemHeaderTemplate;
			res.ItemHeaderTemplateSelector = tab.ItemHeaderTemplateSelector;
			res.ItemHeaderStringFormat = tab.ItemHeaderStringFormat;
			res.ItemContainerStyle = tab.ItemContainerStyle;
			res.ItemContainerStyleSelector = tab.ItemContainerStyleSelector;
			res.PrepareTabItemDelegate = tab.PrepareTabItemDelegate;
			return res;
		}
		object ICloneable.Clone() {
			return CloneCore(this);
		}
		static DXTabControl() {
			DXSerializer.EnabledProperty.OverrideMetadata(typeof(DXTabControl), new UIPropertyMetadata(false));
			DXSerializer.SerializationIDDefaultProperty.OverrideMetadata(typeof(DXTabControl), new UIPropertyMetadata(typeof(DXTabControl).Name));
			DXSerializer.SerializationProviderProperty.OverrideMetadata(typeof(DXTabControl), new UIPropertyMetadata(new DXTabControlSerializationProvider()));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DXTabControl), new FrameworkPropertyMetadata(typeof(DXTabControl)));
			IsTabStopProperty.OverrideMetadata(typeof(DXTabControl), new FrameworkPropertyMetadata(false));
			NavigationAutomationPeersCreator.Default.RegisterObject(typeof(DXTabControl), typeof(DXTabControlAutomationPeer), owner => new DXTabControlAutomationPeer((DXTabControl)owner));
		}
		public DXTabControl() {
			SerializationInfo = new DXTabControlSerializationInfo();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			SetValue(MenuInfoPropertyKey, DevExpress.Mvvm.POCO.ViewModelSource.Create(() => new DXTabControlHeaderMenuInfo(this)));
			OnAllowMergingChanged();
		}
		public FrameworkElement GetLayoutChild(string childName) {
			return (FrameworkElement)GetTemplateChild(childName);
		}
		public void SaveLayoutToStream(Stream stream) {
			DXSerializer.SerializeSingleObject(this, stream, GetType().Name);
		}
		public void SaveLayoutToXml(string path) {
			DXSerializer.SerializeSingleObject(this, path, GetType().Name);
		}
		public void RestoreLayoutFromStream(Stream stream) {
			DXSerializer.DeserializeSingleObject(this, stream, GetType().Name);
		}
		public void RestoreLayoutFromXml(string path) {
			DXSerializer.DeserializeSingleObject(this, path, GetType().Name);
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		public DXTabItem GetTabItem(object item) {
			return GetContainer(item);
		}
		public DXTabItem GetTabItem(int index) {
			return GetContainer(index);
		}
		public void ForEachTabItem(Action<DXTabItem> action) {
			var items = Items.OfType<object>().ToList();
			foreach(var item in items)
				if(Items.Contains(item))
					GetTabItem(item).Do(action);
		}
		public bool HasFocusedTabItem() {
			bool res = false;
			ForEachTabItem(x => res |= x.IsFocused);
			return res;
		}
		protected internal int VisibleItemsCount {
			get {
				int res = 0;
				for(int i = 0; i < Items.Count; i++)
					GetTabItem(i).If(x => x.Visibility == Visibility.Visible).Do(x => res++);
				return res;
			}
		}
		List<object> newItems = new List<object>();
		void CheckNewItems() {
			List<object> itemsToRemove = new List<object>();
			foreach(var newItem in newItems)
				if(!Items.Contains(newItem))
					itemsToRemove.Add(newItem);
			foreach(var newItem in itemsToRemove)
				newItems.Remove(newItem);
		}
		public virtual void AddNewTabItem() {
			var item = AddNewItem();
			var tabItem = GetTabItem(item);
			if(tabItem != null)
				tabItem.SetCurrentValue(DXTabItem.IsNewProperty, true);
			else newItems.Add(item);
		}
		public virtual void InsertTabItem(object item, int index) {
			InsertItem(item, index);
		}
		public void RemoveTabItem(object item) {
			RemoveTabItem(IndexOf(item));
		}
		public virtual void RemoveTabItem(int index) {
			RemoveItem(index);
		}
		public void MoveTabItem(object item, int index) {
			MoveTabItem(IndexOf(item), index);
		}
		public virtual void MoveTabItem(int oldIndex, int newIndex) {
			MoveItem(oldIndex, newIndex);
		}
		public void ShowTabItem(object item, bool raiseEvents = true) {
			ShowTabItem(IndexOf(item), raiseEvents);
		}
		public virtual void ShowTabItem(int index, bool raiseEvents = true) {
			ShowItem(index, raiseEvents);
		}
		public void HideTabItem(object item, bool raiseEvents = true) {
			HideTabItem(IndexOf(item), raiseEvents);
		}
		public virtual void HideTabItem(int index, bool raiseEvents = true) {
			HideItem(index, raiseEvents);
		}
		public void SelectTabItem(object item) {
			SelectTabItem(IndexOf(item));
		}
		public virtual void SelectTabItem(int index) {
			SelectItem(GetCoercedSelectedIndex(index, null));
		}
		protected override int GetCoercedSelectedIndex(int index, NotifyCollectionChangedAction? originativeAction) {
			index = Math.Max(0, index);
			index = Math.Min(Items.Count - 1, index);
			return View.Return(x => x.CoerceSelection(index, originativeAction), () => base.GetCoercedSelectedIndex(index, originativeAction));
		}
		public virtual void SelectNext() {
			base.SelectNextItem();
		}
		public virtual void SelectPrev() {
			base.SelectPrevItem();
		}
		public virtual void SelectFirst() {
			base.SelectFirstItem();
		}
		public virtual void SelectLast() {
			base.SelectLastItem();
		}
		public virtual bool CanSelectNext() {
			return base.CanSelectNextItem();
		}
		public virtual bool CanSelectPrev() {
			return base.CanSelectPrevItem();
		}
		protected override bool GetIsNavigationInversed(FlowDirection flowDirection) {
			if(flowDirection == FlowDirection.LeftToRight || View == null) return false;
			if(View.HeaderLocation == HeaderLocation.Right || View.HeaderLocation == HeaderLocation.Left) return false;
			return true;
		}
		void OnViewPropertyChanged(TabControlViewBase oldValue, TabControlViewBase newValue) {
			if(oldValue == newValue) return;
			oldValue.Do(x => x.Assign(null));
			newValue.Do(x => x.Assign(this));
			UpdateViewProperties();
		}
		object CoerceViewProperty(TabControlViewBase baseValue) {
			if(baseValue == null || !baseValue.CheckAccess())
				return new TabControlMultiLineView();
			if(baseValue.Owner != null && baseValue.Owner != this)
				return (TabControlViewBase)baseValue.Clone();
			return baseValue;
		}
		bool allowUpdateSelectedTabItem = true;
		protected override void OnSelectedContainerChanged(DXTabItem oldValue, DXTabItem newValue) {
			base.OnSelectedContainerChanged(oldValue, newValue);
			if(allowUpdateSelectedTabItem)
				SetCurrentValue(SelectedTabItemProperty, SelectedContainer);
		}
		void OnSelectedTabItemChanged() {
#pragma warning disable
			allowUpdateSelectedTabItem = false;
			SetCurrentValue(SelectedContainerProperty, SelectedTabItem);
			allowUpdateSelectedTabItem = true;
#pragma warning restore
		}
		protected virtual void OnAllowMergingChanged() {
			var behavior = ElementMergingBehavior.InternalWithInternal;
			if(AllowMerging.HasValue)
				behavior = AllowMerging.Value ? ElementMergingBehavior.All : ElementMergingBehavior.Undefined;
			MergingProperties.SetElementMergingBehavior(this, behavior);
			UpdateViewProperties();
		}
		public void UpdateViewProperties() {
			TabViewInfo newViewInfo = new TabViewInfo(this);
			TabViewInfo oldViewInfo = (TabViewInfo)GetValue(ViewInfoProperty);
			if(!newViewInfo.Equals(oldViewInfo))
				SetValue(ViewInfoPropertyKey, newViewInfo);
			ForEachTabItem(tabItem => tabItem.UpdateViewPropertiesСore());
			View.Do(x => x.UpdateViewPropertiesCore());
			(GetValue(MenuInfoProperty) as DXTabControlHeaderMenuInfo).Do(x => x.UpdateHasItems());
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PrevButton = (RepeatButton)GetTemplateChild("PART_PrevButton");
			NextButton = (RepeatButton)GetTemplateChild("PART_NextButton");
			HeaderMenu = (ToggleButton)GetTemplateChild("PART_HeaderMenu");
			CloseButton = (Button)GetTemplateChild("PART_CloseButton");
			TabPanel = (TabPanelContainer)GetTemplateChild("PART_TabPanelContainer");
			ContentHostPresenter = (ContentHostPresenter)GetTemplateChild("PART_ContentHostPresenter");
			UpdateViewProperties();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, true);
			UpdateViewProperties();
			Dispatcher.BeginInvoke(new Action(UpdateViewProperties), DispatcherPriority.Render);
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, false);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if(View == null) View = new TabControlMultiLineView();
		}
		protected override void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue)
				UpdateViewProperties();
		}
		protected override DXTabItem CreateContainer() {
			return new DXTabItem();
		}
		protected override DXTabItem CreateContainerForNewItem() {
			var res = CreateContainer();
			res.SetCurrentValue(DXTabItem.IsNewProperty, true);
			res.Header = "New DXTabItem";
			return res;
		}
		public PrepareTabItemDelegate PrepareTabItemDelegate;
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			DXTabItem tabItem = (DXTabItem)element;
			if(newItems.Contains(item)) {
				tabItem.SetCurrentValue(DXTabItem.IsNewProperty, true);
				newItems.Remove(item);
			}
			base.PrepareContainerForItemOverride(element, item);
			if(!tabItem.IsPropertySet(Control.TabIndexProperty)) tabItem.TabIndex = IndexOf(tabItem);
			PrepareTabItemDelegate.Do(x => x(tabItem, item));
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			CheckNewItems();
			UpdateViewProperties();
		}
		protected override void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e) {
			base.OnItemContainerGeneratorStatusChanged(sender, e);
			UpdateViewProperties();
		}
		public event TabControlSelectionChangedEventHandler SelectionChanged;
		public event TabControlSelectionChangingEventHandler SelectionChanging;
		public event TabControlTabHiddenEventHandler TabHidden;
		public event TabControlTabHidingEventHandler TabHiding;
		public event TabControlTabShownEventHandler TabShown;
		public event TabControlTabShowingEventHandler TabShowing;
		public event TabControlTabRemovedEventHandler TabRemoved;
		public event TabControlTabRemovingEventHandler TabRemoving;
		public event TabControlTabInsertedEventHandler TabInserted;
		public event TabControlTabInsertingEventHandler TabInserting;
		public event TabControlTabMovedEventHandler TabMoved;
		public event TabControlTabMovingEventHandler TabMoving;
		public event TabControlTabAddedEventHandler TabAdded;
		public event TabControlTabAddingEventHandler TabAdding;
		protected override void RaiseSelectionChanging(int oldIndex, int newIndex, object oldItem, object newItem, CancelEventArgs e) {
			base.RaiseSelectionChanging(oldIndex, newIndex, oldItem, newItem, e);
			var args = new TabControlSelectionChangingEventArgs(oldIndex, newIndex, oldItem, newItem) { Cancel = e.Cancel };
			SelectionChanging.Do(x => x(this, args));
			e.Cancel = args.Cancel;
		}
		protected override void RaiseSelectionChanged(int oldIndex, int newIndex, object oldItem, object newItem) {
			base.RaiseSelectionChanged(oldIndex, newIndex, oldItem, newItem);
			SelectionChanged.Do(x => x(this, new TabControlSelectionChangedEventArgs(oldIndex, newIndex, oldItem, newItem)));
			UpdateViewProperties();
		}
		protected override void RaiseItemRemoving(int index, object item, CancelEventArgs e) {
			base.RaiseItemRemoving(index, item, e);
			var args = new TabControlTabRemovingEventArgs(index, item) { Cancel = e.Cancel };
			TabRemoving.Do(x => x(this, args));
			e.Cancel = args.Cancel;
		}
		protected override void RaiseItemRemoved(int index, object item) {
			base.RaiseItemRemoved(index, item);
			TabRemoved.Do(x => x(this, new TabControlTabRemovedEventArgs(index, item)));
		}
		protected override void RaiseItemHiding(int index, object item, CancelEventArgs e) {
			base.RaiseItemHiding(index, item, e);
			var args = new TabControlTabHidingEventArgs(index, item) { Cancel = e.Cancel };
			TabHiding.Do(x => x(this, args));
			e.Cancel = args.Cancel;
		}
		protected override void RaiseItemHidden(int index, object item) {
			base.RaiseItemHidden(index, item);
			TabHidden.Do(x => x(this, new TabControlTabHiddenEventArgs(index, item)));
			UpdateViewProperties();
		}
		protected override void RaiseItemShowing(int index, object item, CancelEventArgs e) {
			base.RaiseItemShowing(index, item, e);
			var args = new TabControlTabShowingEventArgs(index, item) { Cancel = e.Cancel };
			TabShowing.Do(x => x(this, args));
			e.Cancel = args.Cancel;
		}
		protected override void RaiseItemShown(int index, object item) {
			base.RaiseItemShown(index, item);
			TabShown.Do(x => x(this, new TabControlTabShownEventArgs(index, item)));
			UpdateViewProperties();
		}
		protected override void RaiseItemAdding(out object item, CancelEventArgs e) {
			base.RaiseItemAdding(out item, e);
			TabControlTabAddingEventArgs args = new TabControlTabAddingEventArgs() { Cancel = e.Cancel };
			TabAdding.Do(x => x(this, args));
			item = args.Item;
			e.Cancel = args.Cancel;
		}
		protected override void RaiseItemAdded(int index, object item) {
			base.RaiseItemAdded(index, item);
			TabAdded.Do(x => x(this, new TabControlTabAddedEventArgs(index, item)));
		}
		protected override void RaiseItemInserting(int index, object item, CancelEventArgs e) {
			base.RaiseItemInserting(index, item, e);
			TabControlTabInsertingEventArgs args = new TabControlTabInsertingEventArgs(index, item) { Cancel = e.Cancel };
			TabInserting.Do(x => x(this, args));
			e.Cancel = args.Cancel;
		}
		protected override void RaiseItemInserted(int index, object item) {
			base.RaiseItemInserted(index, item);
			TabInserted.Do(x => x(this, new TabControlTabInsertedEventArgs(index, item)));
		}
		protected override void RaiseItemMoving(int oldIndex, int newIndex, object item, CancelEventArgs e) {
			base.RaiseItemMoving(oldIndex, newIndex, item, e);
			TabControlTabMovingEventArgs args = new TabControlTabMovingEventArgs(item, oldIndex, newIndex) { Cancel = e.Cancel };
			TabMoving.Do(x => x(this, args));
			e.Cancel = args.Cancel;
		}
		protected override void RaiseItemMoved(int oldIndex, int newIndex, object item) {
			base.RaiseItemMoved(oldIndex, newIndex, item);
			TabMoved.Do(x => x(this, new TabControlTabMovedEventArgs(item, oldIndex, newIndex)));
		}
		public event TabControlNewTabbedWindowEventHandler NewTabbedWindow;
		protected virtual internal TabControlNewTabbedWindowEventArgs RaiseNewTabbedWindow(Window newWindow, DXTabControl newTabControl) {
			TabControlNewTabbedWindowEventArgs args = new TabControlNewTabbedWindowEventArgs(Window.GetWindow(this), this, newWindow, newTabControl);
			NewTabbedWindow.Do(x => x(this, args));
			return args;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			TabControlKeyboardController.OnTabControlKeyDown(this, e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			TabControlKeyboardController.OnTabControlKeyUp(this, e);
		}
	}
}
