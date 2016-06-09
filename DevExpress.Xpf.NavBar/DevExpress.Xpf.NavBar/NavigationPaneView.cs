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
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.NavBar.Automation;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Data.Native;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors.Flyout;
namespace DevExpress.Xpf.NavBar {
	[TemplateVisualState(Name = "Vertical", GroupName = "OrientationStates"), TemplateVisualState(Name = "Horizontal", GroupName = "OrientationStates")]
	[TemplateVisualState(Name = "Dragging", GroupName = "DraggingStates"), TemplateVisualState(Name = "NotDragging", GroupName = "DraggingStates")]
	public class NavPaneSplitter : Thumb {
		#region DependencyProperties
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ShowBorderProperty;				
		#endregion
		static NavPaneSplitter() {
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(NavPaneSplitter), new PropertyMetadata(Orientation.Vertical, (d, e) => ((NavPaneSplitter)d).OnOrientationPropertyChanged()));
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(NavPaneSplitter), new FrameworkPropertyMetadata(true, (d, e) => ((NavPaneSplitter)d).OnShowBorderChanged((bool)e.OldValue)));
			IsManipulationEnabledProperty.OverrideMetadata(typeof(NavPaneSplitter), new FrameworkPropertyMetadata(true));
		}
		public NavPaneSplitter() {
			this.SetDefaultStyleKey(typeof(NavPaneSplitter));
		}
		#region Properties
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}		
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateAnimations();
			SetBindings();
			UpdateDraggingStates();
			UpdateOrientationStates();			
		}
		protected internal virtual void OnOrientationPropertyChanged() {
			UpdateOrientationStates();
		}
		protected virtual void OnShowBorderChanged(bool oldValue) {
			UpdateOrientationStates();
		}
		void SetBindings() {
			SetBinding(OrientationProperty, new Binding("View.Orientation"));
			SetBinding(ShowBorderProperty, new Binding("View.ShowBorder"));
		}
		void UpdateAnimations() {
			NavBarVisualStateHelper.UpdateStates(this, "OrientationStates");
		}
		void UpdateDraggingStates() {
			VisualStateManager.GoToState(this, IsDragging ? "Dragging" : "NotDragging", false);
		}
		void UpdateOrientationStates() {
			VisualStateManager.GoToState(this, Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical", false);
			string state = Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical";
			state += (ShowBorder ? "WithBorder" : "WithoutBorder");
			VisualStateManager.GoToState(this, state, false);
		}		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == IsDraggingProperty)
				UpdateDraggingStates();
		}
	}
	public class NavigationPaneViewContent : DependencyObject {
		#region static
		public static readonly DependencyProperty ViewProperty;
		public static readonly DependencyProperty NavBarProperty;
		static NavigationPaneViewContent() {
			ViewProperty = DependencyPropertyManager.Register("View", typeof(NavigationPaneView), typeof(NavigationPaneViewContent), new FrameworkPropertyMetadata(null));
			NavBarProperty = DependencyPropertyManager.Register("NavBar", typeof(NavBarControl), typeof(NavigationPaneViewContent), new FrameworkPropertyMetadata(null));
		}
		#endregion
		public NavigationPaneView View {
			get { return (NavigationPaneView)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public NavBarControl NavBar {
			get { return (NavBarControl)GetValue(NavBarProperty); }
			set { SetValue(NavBarProperty, value); }
		}
	}
	public class NavPaneExpandedChangedEventArgs : RoutedEventArgs {
		public NavPaneExpandedChangedEventArgs(bool isExpanded) {
			IsExpanded = isExpanded;
		}		
		public bool IsExpanded { get; internal set; }
	}
	public class NavPaneExpandedChangingEventArgs : NavPaneExpandedChangedEventArgs {
		public NavPaneExpandedChangingEventArgs(bool isExpanded)
			: base(isExpanded) {
		}
		public bool Cancel { get; set; }
	}
	public delegate void NavPaneExpandedChangedEventHandler(object sender, NavPaneExpandedChangedEventArgs e);
	public delegate void NavPaneExpandedChangingEventHandler(object sender, NavPaneExpandedChangingEventArgs e);
	public enum ExpandButtonMode {
		Normal,
		Inverted
	}
	public enum Element {
		ActiveGroup,
		GroupButtonPanel,
		OverflowPanel,
		CollapsedActiveGroup
	}
	[Flags]
	public enum PeekFormShowMode {
		None = 0x0,
		Collapsed = 0x1,
		OverflowPanel = 0x2,
		Both = Collapsed | OverflowPanel
	}
	public partial class NavigationPaneView : NavBarViewBase {
		bool isSplitterVisibleInternal = true;
		bool isOverflowPanelVisibleInternal = true;
		static int defaultItemsControlGroupCount = 3;
		ObservableCollection<IBarManagerControllerAction> menuCustomizations;
		public static readonly DependencyProperty MaxVisibleGroupCountProperty;
		public static readonly DependencyProperty ActiveGroupMinHeightProperty;
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty GroupButtonControlTemplateProperty;
		public static readonly DependencyProperty GroupButtonTemplateProperty;
		public static readonly DependencyProperty GroupButtonTemplateSelectorProperty;
		public static readonly DependencyProperty OverflowGroupControlTemplateProperty;
		public static readonly DependencyProperty OverflowGroupTemplateProperty;
		public static readonly DependencyProperty OverflowGroupTemplateSelectorProperty;
		public static readonly DependencyProperty IsExpandButtonVisibleProperty;
		public static readonly DependencyProperty IsOverflowPanelVisibleProperty;
		public static readonly DependencyProperty IsSplitterVisibleProperty;
		public static readonly DependencyProperty ExpandButtonModeProperty;
		public static readonly DependencyProperty ExpandedTemplateProperty;
		public static readonly DependencyProperty CollapsedTemplateProperty;
		public static readonly DependencyProperty CollapsedActiveGroupControlTemplateProperty;
		public static readonly DependencyProperty ItemsControlGroupsProperty;
		public static readonly DependencyProperty OverflowPanelGroupsProperty;
		public static readonly DependencyProperty ItemsControlGroupCountProperty;
		public static readonly DependencyProperty ElementProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ExpandedWidthProperty;
		public static RoutedEvent NavPaneExpandedChangingEvent;
		public static RoutedEvent NavPaneExpandedChangedEvent;
		public static readonly DependencyProperty ItemVisualStyleInPopupProperty;
		public static readonly DependencyProperty MaxPopupWidthProperty;
		protected internal static readonly DependencyProperty ActualIsOverflowPanelVisibleProperty;
		protected internal static readonly DependencyProperty ActualIsSplitterVisibleProperty;
		public static readonly DependencyProperty IsPopupOpenProperty;
		static readonly DependencyPropertyKey ActualMaxVisibleGroupCountPropertyKey;
		public static readonly DependencyProperty ActualMaxVisibleGroupCountProperty;
		protected static readonly DependencyPropertyKey IsCompactPropertyKey;
		protected internal static readonly DependencyProperty IsCompactProperty;
		public static readonly DependencyProperty EachCollapsedGroupHasSelectedItemProperty;
		public static readonly DependencyProperty ActiveGroupCollapsedNavPaneSelectedItemProperty;
		public static readonly DependencyProperty PeekFormShowModeProperty;
		public static readonly DependencyProperty PeekFormShowDelayProperty;
		public static readonly DependencyProperty PeekFormHideDelayProperty;
		static NavigationPaneView() {
			MaxVisibleGroupCountProperty = DependencyPropertyManager.Register("MaxVisibleGroupCount", typeof(int), typeof(NavigationPaneView),
				new FrameworkPropertyMetadata(defaultItemsControlGroupCount, FrameworkPropertyMetadataOptions.AffectsMeasure, OnMaxVisibleGroupCountPropertyChanged), ValidateMaxVisibleGroupCount);
			ActiveGroupMinHeightProperty = DependencyPropertyManager.Register("ActiveGroupMinHeight", typeof(double), typeof(NavigationPaneView), new PropertyMetadata(150d, OnActiveGroupMinHeightChanged), ValidateActiveGroupMinHeight);
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(NavigationPaneView), new PropertyMetadata(true, OnIsExpandedChanged, CoerceIsExpanded));
			GroupButtonTemplateProperty = DependencyPropertyManager.Register("GroupButtonTemplate", typeof(DataTemplate), typeof(NavigationPaneView), new PropertyMetadata(null, OnGroupButtonTemplateChanged));
			GroupButtonTemplateSelectorProperty = DependencyPropertyManager.Register("GroupButtonTemplateSelector", typeof(DataTemplateSelector), typeof(NavigationPaneView), new PropertyMetadata(null, OnGroupButtonTemplateSelectorChanged));
			GroupButtonControlTemplateProperty = DependencyPropertyManager.Register("GroupButtonControlTemplate", typeof(ControlTemplate), typeof(NavigationPaneView), new PropertyMetadata(null));
			OverflowGroupTemplateProperty = DependencyPropertyManager.Register("OverflowGroupTemplate", typeof(DataTemplate), typeof(NavigationPaneView), new PropertyMetadata(null, OnOverflowGroupTemplateChanged));
			OverflowGroupTemplateSelectorProperty = DependencyPropertyManager.Register("OverflowGroupTemplateSelector", typeof(DataTemplateSelector), typeof(NavigationPaneView), new PropertyMetadata(null, OnOverflowGroupTemplateSelectorChanged));
			OverflowGroupControlTemplateProperty = DependencyPropertyManager.Register("OverflowGroupControlTemplate", typeof(ControlTemplate), typeof(NavigationPaneView), new PropertyMetadata(null));
			IsExpandButtonVisibleProperty = DependencyPropertyManager.Register("IsExpandButtonVisible", typeof(bool), typeof(NavigationPaneView), new PropertyMetadata(true));
			IsOverflowPanelVisibleProperty = DependencyPropertyManager.Register("IsOverflowPanelVisible", typeof(bool), typeof(NavigationPaneView), new PropertyMetadata(true, OnIsOverFlowPanelVisibleChanged));
			IsSplitterVisibleProperty = DependencyPropertyManager.Register("IsSplitterVisible", typeof(bool), typeof(NavigationPaneView), new PropertyMetadata(true, OnIsSplitterVisibleChanged));
			ExpandButtonModeProperty = DependencyPropertyManager.Register("ExpandButtonMode", typeof(ExpandButtonMode), typeof(NavigationPaneView), new PropertyMetadata(ExpandButtonMode.Normal));
			ExpandedTemplateProperty = DependencyPropertyManager.Register("ExpandedTemplate", typeof(DataTemplate), typeof(NavigationPaneView), new PropertyMetadata(null));
			CollapsedTemplateProperty = DependencyPropertyManager.Register("CollapsedTemplate", typeof(DataTemplate), typeof(NavigationPaneView), new PropertyMetadata(null));
			CollapsedActiveGroupControlTemplateProperty = DependencyPropertyManager.Register("CollapsedActiveGroupControlTemplate", typeof(ControlTemplate), typeof(NavigationPaneView), new PropertyMetadata(null));
			ExpandedWidthProperty = DependencyPropertyManager.Register("ExpandedWidth", typeof(double), typeof(NavigationPaneView), new PropertyMetadata(double.NaN));
			ItemsControlGroupsProperty = DependencyPropertyManager.Register("ItemsControlGroups", typeof(ReadOnlyObservableCollection<NavBarGroup>), typeof(NavigationPaneView));
			OverflowPanelGroupsProperty = DependencyPropertyManager.Register("OverflowPanelGroups", typeof(ReadOnlyObservableCollection<NavBarGroup>), typeof(NavigationPaneView));
			ItemsControlGroupCountProperty = DependencyPropertyManager.Register("ItemsControlGroupCount", typeof(int), typeof(NavigationPaneView), new FrameworkPropertyMetadata(defaultItemsControlGroupCount, ItemsControlGroupCountPropertyChanged));
			ElementProperty = DependencyPropertyManager.RegisterAttached("Element", typeof(Element), typeof(NavigationPaneView), new FrameworkPropertyMetadata(Element.ActiveGroup));
			ContentProperty = DependencyPropertyManager.RegisterAttached("Content", typeof(NavigationPaneViewContent), typeof(NavigationPaneView), new FrameworkPropertyMetadata(null));
			NavPaneExpandedChangingEvent = EventManager.RegisterRoutedEvent("NavPaneExpandedChangingEvent", RoutingStrategy.Direct, typeof(NavPaneExpandedChangingEventHandler), typeof(NavigationPaneView));
			NavPaneExpandedChangedEvent = EventManager.RegisterRoutedEvent("NavPaneExpandedChangedEvent", RoutingStrategy.Direct, typeof(NavPaneExpandedChangedEventHandler), typeof(NavigationPaneView));
			ItemVisualStyleInPopupProperty = DependencyPropertyManager.Register("ItemVisualStyleInPopup", typeof(Style), typeof(NavigationPaneView), new PropertyMetadata(null, OnItemVisualStyleInPopupPropertyChanged));
			MaxPopupWidthProperty = DependencyPropertyManager.Register("MaxPopupWidth", typeof(double), typeof(NavigationPaneView), new FrameworkPropertyMetadata(double.PositiveInfinity));
			ActualIsOverflowPanelVisibleProperty = DependencyPropertyManager.Register("ActualIsOverflowPanelVisible", typeof(bool), typeof(NavigationPaneView), new FrameworkPropertyMetadata(true));
			ActualIsSplitterVisibleProperty = DependencyPropertyManager.Register("ActualIsSplitterVisible", typeof(bool), typeof(NavigationPaneView), new FrameworkPropertyMetadata(true));
			IsPopupOpenProperty = DependencyPropertyManager.Register("IsPopupOpen", typeof(bool), typeof(NavigationPaneView), new FrameworkPropertyMetadata(false, (d, e) => ((NavigationPaneView)d).OnIsPopupOpenChanged((bool)e.OldValue), CoerceOnPopupOpen));
			CommandManager.RegisterClassCommandBinding(typeof(NavigationPaneView), new CommandBinding(NavigationPaneCommands.ChangeNavPaneExpanded, new ExecutedRoutedEventHandler(OnChangeNavPaneExpanded)));
			CommandManager.RegisterClassCommandBinding(typeof(NavigationPaneView), new CommandBinding(NavigationPaneCommands.ShowMoreGroups, new ExecutedRoutedEventHandler(OnShowMoreGroups), (d, e) => ((NavigationPaneView)d).CanShowMoreGroups(e)));
			CommandManager.RegisterClassCommandBinding(typeof(NavigationPaneView), new CommandBinding(NavigationPaneCommands.ShowFewerGroups, new ExecutedRoutedEventHandler(OnShowFewerGroups), (d, e) => ((NavigationPaneView)d).CanShowFewerGroups(e)));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(NavigationPaneView), typeof(NavigationPaneViewAutomationPeer), owner => new NavigationPaneViewAutomationPeer((NavigationPaneView)owner));
			ActualMaxVisibleGroupCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualMaxVisibleGroupCount", typeof(int), typeof(NavigationPaneView), new FrameworkPropertyMetadata(defaultItemsControlGroupCount, OnActualMaxVisibleGroupCountChanged));
			ActualMaxVisibleGroupCountProperty = ActualMaxVisibleGroupCountPropertyKey.DependencyProperty;
			IsCompactPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCompact", typeof(bool), typeof(NavigationPaneView), new FrameworkPropertyMetadata(false,OnIsCompactChanged));
			IsCompactProperty = IsCompactPropertyKey.DependencyProperty;
			EachCollapsedGroupHasSelectedItemProperty = DependencyPropertyManager.Register("EachCollapsedGroupHasSelectedItem", typeof(bool), typeof(NavigationPaneView), new FrameworkPropertyMetadata(true));
			ActiveGroupCollapsedNavPaneSelectedItemProperty = DependencyPropertyManager.Register("ActiveGroupCollapsedNavPaneSelectedItem", typeof(object), typeof(NavigationPaneView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((NavigationPaneView)d).OnActiveGroupCollapsedNavPaneSelectedItemChanged(e)));
			PeekFormShowModeProperty = DependencyPropertyManager.Register("PeekFormShowMode", typeof(PeekFormShowMode), typeof(NavigationPaneView), new FrameworkPropertyMetadata(PeekFormShowMode.Both));
			PeekFormShowDelayProperty = DependencyPropertyManager.Register("PeekFormShowDelay", typeof(int), typeof(NavigationPaneView), new FrameworkPropertyMetadata(700));
			PeekFormHideDelayProperty = DependencyPropertyManager.Register("PeekFormHideDelay", typeof(int), typeof(NavigationPaneView), new FrameworkPropertyMetadata(400));
		}
		static object CoerceOnPopupOpen(DependencyObject d, object baseValue) {
			var navbar = ((NavigationPaneView)d).NavBar;
			if (navbar.ActiveGroup != null && navbar.ActiveGroup.NavPaneShowMode != ShowMode.Items)
				return !((NavigationPaneView)d).IsExpanded && (bool)baseValue;
			else
				return false;
		}
		static void OnIsSplitterVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).UpdateActualIsSplitterVisible();
		}
		static void OnIsOverFlowPanelVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).UpdateActualIsOverflowPanelVisible();
		}
		private static void OnIsCompactChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue)
				((NavigationPaneView)d).ActualMaxVisibleGroupCount = 0;
			else
				((NavigationPaneView)d).ActualMaxVisibleGroupCount = ((NavigationPaneView)d).MaxVisibleGroupCount;
		}
		void OnActiveGroupCollapsedNavPaneSelectedItemChanged(DependencyPropertyChangedEventArgs e) {
			if (CollapsedNavPaneSelectionStrategy.IsValidItem(ActiveGroupCollapsedNavPaneSelectedItem))
				CollapsedNavPaneSelectionStrategy.SelectItem(ActiveGroupCollapsedNavPaneSelectedItem);
			else
				CollapsedNavPaneSelectionStrategy.UnselectItem(NavBar.ActiveGroup);
			UpdateActiveGroupCollapsedNavPaneSelectedItem();
		}
		static void ItemsControlGroupCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var view = (NavigationPaneView)d;
			view.UpdateView();
		}
		static void InvalidateMeasureVisalTree(FrameworkElement elem, FrameworkElement root) {
			while(elem != root && elem != null) {
				elem.InvalidateMeasure();
				elem = VisualTreeHelper.GetParent(elem) as FrameworkElement;
			}
			root.InvalidateMeasure();
		}
		static void OnMaxVisibleGroupCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(((NavigationPaneView)d).IsInternalSetMaxVisibleGroupCount) {
			}
			if (!((NavigationPaneView)d).IsCompact)
				((NavigationPaneView)d).ActualMaxVisibleGroupCount = (int)e.NewValue;
		}
		private static void OnActualMaxVisibleGroupCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).ItemsControlGroupCount = (int)e.NewValue;
			((NavigationPaneView)d).UpdateView();
		}
		static bool ValidateMaxVisibleGroupCount(object d) {
			return (int)d >= 0;
		}
		static void OnActiveGroupMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).UpdateView();
		}
		static bool ValidateActiveGroupMinHeight(object d) {
			return (double)d >= 0;
		}
		static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).OnIsExpandedChanged();
		}
		static object CoerceIsExpanded(DependencyObject d, object value) {
			return ((NavigationPaneView)d).CoerceIsExpanded((bool)value);
		}
		static void OnGroupButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).UpdateActualGroupButtonTemplateSelector();
		}
		static void OnChangeNavPaneExpanded(object sender, ExecutedRoutedEventArgs e) {
			((NavigationPaneView)sender).ChangeNavPaneExpanded();
		}
		static void OnGroupButtonTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).UpdateActualGroupButtonTemplateSelector();
		}
		static void OnOverflowGroupTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).UpdateActualOverflowGroupTemplateSelector();
		}
		static void OnOverflowGroupTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).UpdateActualOverflowGroupTemplateSelector();
		}
		static void OnItemVisualStyleInPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneView)d).OnItemVisualStyleInPopupChanged();
		}
		public static void OnShowMoreGroups(object sender, ExecutedRoutedEventArgs e) {
			((NavigationPaneView)sender).ShowMoreGroups(e.Parameter);
		}
		public static void OnShowFewerGroups(object sender, ExecutedRoutedEventArgs e) {
			((NavigationPaneView)sender).ShowFewerGroups(e.Parameter);
		}
		public static void SetElement(DependencyObject d, Element element) {
			d.SetValue(ElementProperty, element);
		}
		public static Element GetElement(DependencyObject d) {
			return (Element)d.GetValue(ElementProperty);
		}
		bool isPopupOpenedCore = false;
		internal bool IsPopupOpened {
			get { return isPopupOpenedCore; }
			set {
				if(isPopupOpenedCore == value)
					return;
				isPopupOpenedCore = value;
				OnIsPopupOpenedChanged();
			}
		}
		protected internal NavigationPanePanel Panel { get; set; }
		protected virtual void OnIsPopupOpenedChanged() {
			UpdateActualItemVisualStyle();
		}
		void CreateBindings() {
			Binding b = new Binding("IsExpanded") { Source = this, Converter = new IsActiveToAnimationProgressConverter() };
			BindingOperations.SetBinding(this, NavBarAnimationOptions.AnimationProgressProperty, b);
			if (Window.GetWindow(this) != null)
				BindingOperations.SetBinding(this, NavigationPaneView.MaxPopupWidthProperty, new Binding("ActualWidth") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1), Converter = new DoubleDivisionConverter(), ConverterParameter = 2d });
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewExpandedWidth"),
#endif
 Category(Categories.OptionsView)]
		public double ExpandedWidth {
			get { return (double)GetValue(ExpandedWidthProperty); }
			set { SetValue(ExpandedWidthProperty, value); }
		}
		int actualVisibleGroupCount = 0;
		protected internal int ActualVisibleGroupCount {
			get { return actualVisibleGroupCount; }
			set {
				if(ActualVisibleGroupCount == value)
					return;
				actualVisibleGroupCount = value;
				OnActualVisibleGroupCountChanged();
			}
		}
		protected virtual void OnActualVisibleGroupCountChanged() {
			UpdateOverflowControlGroupsCollection();
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewMaxVisibleGroupCount"),
#endif
 Category(Categories.OptionsView)]
		public int MaxVisibleGroupCount {
			get { return (int)GetValue(MaxVisibleGroupCountProperty); }
			set { SetValue(MaxVisibleGroupCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewActiveGroupMinHeight"),
#endif
		Category(Categories.OptionsView)]
		public double ActiveGroupMinHeight {
			get { return (double)GetValue(ActiveGroupMinHeightProperty); }
			set { SetValue(ActiveGroupMinHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewIsExpanded"),
#endif
		Category(Categories.OptionsView)]
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewGroupButtonControlTemplate"),
#endif
		Category(Categories.Templates)]
		public ControlTemplate GroupButtonControlTemplate {
			get { return (ControlTemplate)GetValue(GroupButtonControlTemplateProperty); }
			set { SetValue(GroupButtonControlTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewGroupButtonTemplate"),
#endif
		Category(Categories.Templates)]
		public DataTemplate GroupButtonTemplate {
			get { return (DataTemplate)GetValue(GroupButtonTemplateProperty); }
			set { SetValue(GroupButtonTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewGroupButtonTemplateSelector"),
#endif
		Category(Categories.Templates)]
		public DataTemplateSelector GroupButtonTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupButtonTemplateSelectorProperty); }
			set { SetValue(GroupButtonTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewOverflowGroupControlTemplate"),
#endif
		Category(Categories.Templates)]
		public ControlTemplate OverflowGroupControlTemplate {
			get { return (ControlTemplate)GetValue(OverflowGroupControlTemplateProperty); }
			set { SetValue(OverflowGroupControlTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewOverflowGroupTemplate"),
#endif
		Category(Categories.Templates)]
		public DataTemplate OverflowGroupTemplate {
			get { return (DataTemplate)GetValue(OverflowGroupTemplateProperty); }
			set { SetValue(OverflowGroupTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewOverflowGroupTemplateSelector"),
#endif
				Category(Categories.Templates)]
		public DataTemplateSelector OverflowGroupTemplateSelector {
			get { return (DataTemplateSelector)GetValue(OverflowGroupTemplateSelectorProperty); }
			set { SetValue(OverflowGroupTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewIsExpandButtonVisible"),
#endif
		Category(Categories.OptionsView)]
		public bool IsExpandButtonVisible {
			get { return (bool)GetValue(IsExpandButtonVisibleProperty); }
			set { SetValue(IsExpandButtonVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewIsOverflowPanelVisible"),
#endif
		Category(Categories.OptionsView)]
		public bool IsOverflowPanelVisible {
			get { return (bool)GetValue(IsOverflowPanelVisibleProperty); }
			set { SetValue(IsOverflowPanelVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewIsSplitterVisible"),
#endif
		Category(Categories.OptionsView)]
		public bool IsSplitterVisible {
			get { return (bool)GetValue(IsSplitterVisibleProperty); }
			set { SetValue(IsSplitterVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewExpandButtonMode"),
#endif
		Category(Categories.OptionsView)]
		public ExpandButtonMode ExpandButtonMode {
			get { return (ExpandButtonMode)GetValue(ExpandButtonModeProperty); }
			set { SetValue(ExpandButtonModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewExpandedTemplate"),
#endif
		Category(Categories.Templates)]
		public DataTemplate ExpandedTemplate {
			get { return (DataTemplate)GetValue(ExpandedTemplateProperty); }
			set { SetValue(ExpandedTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewCollapsedTemplate"),
#endif
		Category(Categories.Templates)]
		public DataTemplate CollapsedTemplate {
			get { return (DataTemplate)GetValue(CollapsedTemplateProperty); }
			set { SetValue(CollapsedTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavigationPaneViewCollapsedActiveGroupControlTemplate"),
#endif
		Category(Categories.Templates)]
		public ControlTemplate CollapsedActiveGroupControlTemplate {
			get { return (ControlTemplate)GetValue(CollapsedActiveGroupControlTemplateProperty); }
			set { SetValue(CollapsedActiveGroupControlTemplateProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ItemsControlGroupCount {
			get { return (int)GetValue(ItemsControlGroupCountProperty); }
			set { SetValue(ItemsControlGroupCountProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyObservableCollection<NavBarGroup> ItemsControlGroups {
			get { return (ReadOnlyObservableCollection<NavBarGroup>)GetValue(ItemsControlGroupsProperty); }
			set { SetValue(ItemsControlGroupsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyObservableCollection<NavBarGroup> OverflowPanelGroups {
			get { return (ReadOnlyObservableCollection<NavBarGroup>)GetValue(OverflowPanelGroupsProperty); }
			set { SetValue(OverflowPanelGroupsProperty, value); }
		}
		public NavigationPaneViewContent Content {
			get { return (NavigationPaneViewContent)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public Style ItemVisualStyleInPopup {
			get { return (Style)GetValue(ItemVisualStyleInPopupProperty); }
			set { SetValue(ItemVisualStyleInPopupProperty, value); }
		}
		public double MaxPopupWidth {
			get { return (double)GetValue(MaxPopupWidthProperty); }
			set { SetValue(MaxPopupWidthProperty, value); }
		}
		double expandedWidthCore = Double.NaN;
		[DefaultValue(Double.NaN)]
		protected internal double ExpandedWidthCore {
			get { return GetExpandedWidthCore(); }
			set { expandedWidthCore = value; }
		}
		protected internal bool ActualIsOverflowPanelVisible {
			get { return (bool)GetValue(ActualIsOverflowPanelVisibleProperty); }
			set { SetValue(ActualIsOverflowPanelVisibleProperty, value); }
		}
		protected internal bool ActualIsSplitterVisible {
			get { return (bool)GetValue(ActualIsSplitterVisibleProperty); }
			set { SetValue(ActualIsSplitterVisibleProperty, value); }
		}
		protected internal bool IsOverflowPanelVisibleInternal {
			get { return isOverflowPanelVisibleInternal; }
			set {
				isOverflowPanelVisibleInternal = value;
				UpdateActualIsOverflowPanelVisible();
			}
		}
		protected internal bool IsSplitterVisibleInternal {
			get { return isSplitterVisibleInternal; }
			set { isSplitterVisibleInternal = value;
			UpdateActualIsSplitterVisible();
			}
		}
		public bool IsPopupOpen {
			get { return (bool)GetValue(IsPopupOpenProperty); }
			set { SetValue(IsPopupOpenProperty, value); }
		}
		void UpdateActualIsSplitterVisible() {
			ActualIsSplitterVisible = IsSplitterVisible && IsSplitterVisibleInternal;
		}
		void UpdateActualIsOverflowPanelVisible() {
			ActualIsOverflowPanelVisible = IsOverflowPanelVisibleInternal && IsOverflowPanelVisible;
		}
		public int ActualMaxVisibleGroupCount {
			get { return (int)GetValue(ActualMaxVisibleGroupCountProperty); }
			protected internal set { this.SetValue(ActualMaxVisibleGroupCountPropertyKey, value); }
		}
		protected internal bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { this.SetValue(IsCompactPropertyKey, value); }
		}
		public bool EachCollapsedGroupHasSelectedItem {
			get { return (bool)GetValue(EachCollapsedGroupHasSelectedItemProperty); }
			set { SetValue(EachCollapsedGroupHasSelectedItemProperty, value); }
		}
		public object ActiveGroupCollapsedNavPaneSelectedItem {
			get { return (object)GetValue(ActiveGroupCollapsedNavPaneSelectedItemProperty); }
			set { SetValue(ActiveGroupCollapsedNavPaneSelectedItemProperty, value); }
		}
		public PeekFormShowMode PeekFormShowMode {
			get { return (PeekFormShowMode)GetValue(PeekFormShowModeProperty); }
			set { SetValue(PeekFormShowModeProperty, value); }
		}
		public int PeekFormShowDelay {
			get { return (int)GetValue(PeekFormShowDelayProperty); }
			set { SetValue(PeekFormShowDelayProperty, value); }
		}
		public int PeekFormHideDelay {
			get { return (int)GetValue(PeekFormHideDelayProperty); }
			set { SetValue(PeekFormHideDelayProperty, value); }
		}
#if DEBUGTEST
		internal NavBarPopupMenu NavPaneDropDownMenu { get; set; }
#endif
		private double GetExpandedWidthCore() {
			if(!Double.IsNaN(ExpandedWidth)) return ExpandedWidth;
			if(Double.IsNaN(expandedWidthCore)) return Double.NaN;			
			if(Panel==null || ExpandInfoProvider.IsExpanding || ExpandInfoProvider.IsJustExpanded) return expandedWidthCore;
			NavPaneActiveGroupControl activeGroupControl = Panel.ActiveGroup as NavPaneActiveGroupControl;
			if(activeGroupControl==null) return expandedWidthCore;
			return Math.Max(GetSizeHelper().GetSecondarySize(activeGroupControl.desiredInfiniteSize), expandedWidthCore);
		}
		protected internal NavigationPaneExpandInfoProvider ExpandInfoProvider { get; set; }
		public NavigationPaneView() {
			this.SetDefaultStyleKey(typeof(NavigationPaneView));
			UpdateActualGroupButtonTemplateSelector();
			UpdateActualOverflowGroupTemplateSelector();
			Actions = new ActionsList();
			Actions.Add(new UpdateItemsControlGroupsAction(this));
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			ActiveGroupChanged += OnActiveGroupChanged;
			NavBarViewKind = NavBarViewKind.NavigationPane;
			ExpandInfoProvider = new NavigationPaneExpandInfoProvider(this);
			onMenuManagerBeforeCheckCloseAllPopups = new MouseEventHandler(OnMenuManagerBeforeCheckCloseAllPopups);
			CreateBindings();
			UpdateActualIsSplitterVisible();
			UpdateActualIsOverflowPanelVisible();
		}
		void OnActiveGroupChanged(object sender, NavBarActiveGroupChangedEventArgs e) {
			UpdateActiveGroupCollapsedNavPaneSelectedItem();
		}
		void UpdateActiveGroupCollapsedNavPaneSelectedItem() {
			if (NavBar != null && NavBar.ActiveGroup != null)
				ActiveGroupCollapsedNavPaneSelectedItem = NavBar.ActiveGroup.CollapsedNavPaneSelectedItem;
		}
		protected internal virtual SizeHelperBase GetSizeHelper() {
			return SizeHelperBase.GetDefineSizeHelper(Orientation);
		}
		Size lastMeasureSize = Size.Empty;
		bool measureByInfinityChanged = false;
		protected override Size MeasureOverride(Size availableSize) {
			if(IsExpanded
				&& !ExpandInfoProvider.IsJustExpanded
				&& Expander.AnimationProgress == 1
				&& !IsInfiniteWidth(availableSize)) {
				ExpandedWidthCore = Double.IsNaN(ExpandedWidth) ? (Orientation == System.Windows.Controls.Orientation.Vertical ? availableSize.Width : availableSize.Height) : ExpandedWidth;
			}
			measureByInfinityChanged = IsExpanded && !(ExpandInfoProvider.IsExpanding || ExpandInfoProvider.IsCollapsing || ExpandInfoProvider.IsJustCollapsed || ExpandInfoProvider.IsJustExpanded) && double.IsNaN(ExpandedWidth);
			if(measureByInfinityChanged != false) {
				measureByInfinityChanged &= lastMeasureSize.IsEmpty || (Orientation == System.Windows.Controls.Orientation.Vertical ?
					double.IsInfinity(availableSize.Width) && !double.IsInfinity(lastMeasureSize.Width) :
					double.IsInfinity(availableSize.Height) && !double.IsInfinity(lastMeasureSize.Height));
				Dispatcher.BeginInvoke(new Action(() => { measureByInfinityChanged = false; }));
			}
			bool shouldUseExpandedWidth = !Double.IsNaN(ExpandedWidthCore) && !measureByInfinityChanged;
			double measureWidth = availableSize.Width;
			double measureHeight = availableSize.Height;
			if(Orientation == System.Windows.Controls.Orientation.Vertical) {
				measureWidth = shouldUseExpandedWidth ? Math.Min(ExpandedWidthCore,availableSize.Width) : availableSize.Width;
			} else {
				measureHeight = shouldUseExpandedWidth ? Math.Min(ExpandedWidthCore, availableSize.Height) : availableSize.Height;
			}
			return base.MeasureOverride(new Size( measureWidth, measureHeight));
		}
		private bool IsInfiniteWidth(Size availableSize) {
			return (Orientation == System.Windows.Controls.Orientation.Vertical? double.IsInfinity(availableSize.Width): double.IsInfinity(availableSize.Height));
		}
		protected override void OnNavBarChanged(DependencyPropertyChangedEventArgs e) {
			base.OnNavBarChanged(e);
			Content = new NavigationPaneViewContent() { View = this, NavBar = NavBar };
		}
		void CanShowMoreGroups(CanExecuteRoutedEventArgs e) {
			e.CanExecute = ActualVisibleGroupCount < ItemsControlGroups.Count;
		}
		void CanShowFewerGroups(CanExecuteRoutedEventArgs e) {
			e.CanExecute = ActualVisibleGroupCount > 0;
		}
		void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			if (NavBar == null)
				return;
			LayoutUpdated += NavigationPaneView_LayoutUpdated;
			UpdateState();
		}
		void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			LayoutUpdated -= NavigationPaneView_LayoutUpdated;
			ActiveGroupChanged -= OnActiveGroupChanged;			
		}
		void NavigationPaneView_LayoutUpdated(object sender, EventArgs e) {
			Actions.Execute(this);
		}
		internal override void SetNavBar(NavBarControl navBar) {
			base.SetNavBar(navBar);
			UpdateActualGroupButtonTemplateSelector();
			UpdateActualOverflowGroupTemplateSelector();
		}
		public ObservableCollection<IBarManagerControllerAction> MenuCustomizations {
			get { return menuCustomizations ?? (menuCustomizations = new ObservableCollection<IBarManagerControllerAction>()); }
		}
		Internal.CollapsedNavPaneItemsSelectionStrategy collapsedNavPaneSelectionStrategy;
		internal Internal.CollapsedNavPaneItemsSelectionStrategy CollapsedNavPaneSelectionStrategy {
			get { return collapsedNavPaneSelectionStrategy?? (collapsedNavPaneSelectionStrategy = new Internal.CollapsedNavPaneItemsSelectionStrategy(NavBar)); }
		}
		public event NavPaneExpandedChangingEventHandler NavPaneExpandedChanging {
			add { this.AddHandler(NavPaneExpandedChangingEvent, value); }
			remove { RemoveHandler(NavPaneExpandedChangingEvent, value); }
		}
		public event NavPaneExpandedChangedEventHandler NavPaneExpandedChanged {
			add { AddHandler(NavPaneExpandedChangedEvent, value); }
			remove { RemoveHandler(NavPaneExpandedChangedEvent, value); }
		}
		public void ChangeNavPaneExpanded() {
			IsExpanded = !IsExpanded;
		}
		public void ShowMoreGroups(object parameter) {
			int value = parameter == null ? 1 : (int)parameter;
			InternalSetMaxVisibleGroupCount(Math.Min(ItemsControlGroups.Count, MaxVisibleGroupCount + value));
		}
		public void ShowFewerGroups(object parameter) {
			int count = parameter == null ? 1 : (int)parameter;
			InternalSetMaxVisibleGroupCount(Math.Max(0, MaxVisibleGroupCount - count));
		}
		internal void InternalSetMaxVisibleGroupCount(int value) {
			IsInternalSetMaxVisibleGroupCount = true;
			MaxVisibleGroupCount = value;
			IsInternalSetMaxVisibleGroupCount = false;
			UpdateView();
		}
		internal bool IsInternalSetMaxVisibleGroupCount = false;
		bool ContainsItems(IList list) {
			return list != null && list.Count > 0;
		}
		#region UpdateLayout
		ActionsList Actions { get; set; }
		void UpdateItemsControlGroupsCollection() {
			Actions.Add(new UpdateItemsControlGroupsAction(this));
		}
		void UpdateOverflowControlGroupsCollection() {
			Actions.Add(new UpdateOverflowGroupsAction(this));
		}
		#endregion
		void OnIsExpandedChanged() {
			RaiseEvent(new NavPaneExpandedChangedEventArgs(IsExpanded) { RoutedEvent = NavigationPaneView.NavPaneExpandedChangedEvent });
			if(NavBar != null)
				NavBar.RaiseIsExpandedChanged(!IsExpanded, IsExpanded);
			UpdateExpandedWidthCore();
			UpdateAnimationMinHeightWidth();
			ResetWidthProperty();
			UpdatePresenterTemplateByEnabledState();
			UpdateVisualStateByExpandState();
		}
		internal void UpdateGroupsDisplayMode(DisplayMode mode) {
			if (NavBar != null && NavBar.Groups != null)
				foreach (var group in NavBar.Groups) {
					if (group.ImageSource != null)
						if (mode == DisplayMode.Text)
							NavigationPaneExpandInfoProvider.SetIsCompleteCollapsed(group, false);
						else
							NavigationPaneExpandInfoProvider.SetIsCompleteCollapsed(group, true);
				}
		}
		private void UpdateAnimationMinHeightWidth() {
			if(IsExpanded) return;
			double imageWidth = 24d;
			if(Panel != null && Panel.GroupsControl != null) {
				foreach(var group_ in Panel.GroupsControl.Items) {
					NavBarGroup group = group_ as NavBarGroup ?? (new FrameworkElementInfoSLCompatibilityConverterExtension() { ConvertToInfo = false }.Convert(group_) as NavBarGroup);
					if (group == null)
						continue;
					imageWidth = Math.Max(Orientation == Orientation.Horizontal ? group.ActualImageSettings.Height : group.ActualImageSettings.Width, imageWidth);
				}
			}
			NavBarAnimationOptions.SetMinHeight(this, imageWidth + 12d);
			NavBarAnimationOptions.SetMinWidth(this, imageWidth + 12d);
		}
		private void ResetWidthProperty() {
			if(NavBar == null)
				return;
			if(Orientation == System.Windows.Controls.Orientation.Vertical) {
				NavBar.Width = double.NaN;
			}
			else {
				NavBar.Height = double.NaN;
			}
			if(IsExpanded && storedMinWidth != null) {
				if(Orientation == System.Windows.Controls.Orientation.Vertical) {
					NavBar.RestorePropertyValue(FrameworkElement.MinWidthProperty, storedMinWidth);
					NavBar.RestorePropertyValue(FrameworkElement.MaxWidthProperty, storedMaxWidth);
				}
				else {
					NavBar.RestorePropertyValue(FrameworkElement.MinHeightProperty, storedMinWidth);
					NavBar.RestorePropertyValue(FrameworkElement.MaxHeightProperty, storedMaxWidth);
				}
				this.storedMinWidth = null;
				this.storedMaxWidth = null;
			}
		}
		bool CoerceIsExpanded(bool newValue) {
			return CoerceIsExpandedCore(newValue, IsExpanded);
		}
		protected ContentPresenter NavPaneContentPresenter { get; private set; }
		DXExpander expander;
		public DXExpander Expander {
			get { return expander; }
			protected set {
				if(Expander == value)
					return;
				UnSubscribeExpanderEvents();
				expander = value;
				SubscribeExpanderEvents();
			}
		}
		private void SubscribeExpanderEvents() {
			ExpandInfoProvider.Expander = Expander;
			if(Expander != null) {
				Expander.GetExpandCollapseInfo += new ExpandCollapseInfoEventHandler(OnGetExpandCollapseInfo);
			}
		}
		object storedMinWidth, storedMaxWidth;
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if(IsExpanded) {
				if(Orientation == System.Windows.Controls.Orientation.Vertical) {
					arrangeBounds = new Size(Double.IsNaN(ExpandedWidth) ? arrangeBounds.Width : ExpandedWidth, arrangeBounds.Height);
				} else {
					arrangeBounds = new Size(arrangeBounds.Width, Double.IsNaN(ExpandedWidth) ? arrangeBounds.Height : ExpandedWidth);
				}
			}
			Size res = base.ArrangeOverride(arrangeBounds);
			if(Expander == null) return res;
			if(IsExpanded && Expander.AnimationProgress == 1) {
				ExpandedWidthCore = Double.IsNaN(ExpandedWidth) ? (Orientation == System.Windows.Controls.Orientation.Vertical ? arrangeBounds.Width : arrangeBounds.Height) : ExpandedWidth;
			}
			if(!IsExpanded && Expander.AnimationProgress == 0 && this.storedMinWidth == null) {
				if(Orientation == System.Windows.Controls.Orientation.Vertical) {
					storedMinWidth = NavBar.StorePropertyValue(FrameworkElement.MinWidthProperty);
					storedMaxWidth = NavBar.StorePropertyValue(FrameworkElement.MaxWidthProperty);
					NavBar.MaxWidth = arrangeBounds.Width+Margin.Left+Margin.Right;
					NavBar.MinWidth = arrangeBounds.Width + Margin.Left + Margin.Right;
				}
				else {
					storedMinWidth = NavBar.StorePropertyValue(FrameworkElement.MinHeightProperty);
					storedMaxWidth = NavBar.StorePropertyValue(FrameworkElement.MaxHeightProperty);
					NavBar.MaxHeight = arrangeBounds.Height + Margin.Top + Margin.Bottom;
					NavBar.MinHeight = arrangeBounds.Height + Margin.Top + Margin.Bottom;
				}
			}
			return res;
		}		
		void OnGetExpandCollapseInfo(object sender, ExpandCollapseInfoEventArgs e) {
			if(double.IsNaN(ExpandedWidthCore) || measureByInfinityChanged)
				return;
			if(Orientation == System.Windows.Controls.Orientation.Vertical) {
				if(double.IsInfinity(e.Size.Width))
					e.Size = new Size(ExpandedWidthCore, e.Size.Height);
			}
			else {
				if(double.IsInfinity(e.Size.Height))
					e.Size = new Size(e.Size.Width, ExpandedWidthCore);
			}
		}
		private void UnSubscribeExpanderEvents() {
			if(Expander != null) {
				Expander.GetExpandCollapseInfo -= new ExpandCollapseInfoEventHandler(OnGetExpandCollapseInfo);
			}
		}
		void UpdateExpandedWidthCore() {
			if(IsExpanded || ExpandInfoProvider.IsExpanding)
				return;
			double expandedWidthCandidate = Orientation == System.Windows.Controls.Orientation.Vertical ? ActualWidth : ActualHeight;
			ExpandedWidthCore = Double.IsNaN(ExpandedWidth) ? (expandedWidthCandidate == 0d ? Double.NaN : expandedWidthCandidate) : ExpandedWidth;
		}
		NavPanePopup navPanePopup;
		CollapsedActiveGroup collapsedActiveGroup;
		CollapsedActiveGroupDefaultElement collapsedActiveGroupDefaultElement;
		public override void OnApplyTemplate() {
			Expander = GetTemplateChild("PART_DXExpander") as DXExpander;
			base.OnApplyTemplate();
			NavPaneContentPresenter = GetTemplateChild("PART_NavPaneContentPresenter") as ContentPresenter;
			UpdatePresenterTemplateByEnabledState();			
			bool menuManagerFound = false;
			bool popupFound = false;
			bool collapsedGoupFound = false;
			Dispatcher.BeginInvoke(new Action(() => {
				DevExpress.Xpf.Core.Native.LayoutHelper.FindElement(this, fe => {
					if (fe is NavPanePopup)
						navPanePopup = fe as NavPanePopup;
					if (fe is CollapsedActiveGroup)
						collapsedActiveGroup = fe as CollapsedActiveGroup;
					if (fe is CollapsedActiveGroupDefaultElement)
						collapsedActiveGroupDefaultElement = fe as CollapsedActiveGroupDefaultElement;
					return menuManagerFound && popupFound && collapsedGoupFound;
				});
			}));			
		}		
		MouseEventHandler onMenuManagerBeforeCheckCloseAllPopups;
		protected internal virtual void OnMenuManagerBeforeCheckCloseAllPopups(object sender, MouseEventArgs e) {
			if (collapsedActiveGroup != null && navPanePopup != null) {
				if (!navPanePopup.IsOpen) return;
				bool mouseOverPopup = false;
				mouseOverPopup = navPanePopup.IsMouseOver;
				if (collapsedActiveGroup.IsMouseOver 
					|| (collapsedActiveGroupDefaultElement!=null && collapsedActiveGroupDefaultElement.IsMouseOver) 
					|| mouseOverPopup) return;
				navPanePopup.IsOpen = false;
			}
		}
		protected virtual void UpdatePresenterTemplateByEnabledState() {
			if(NavPaneContentPresenter == null)
				return;			
			Binding b = new Binding(IsExpanded ? "ExpandedTemplate" : "CollapsedTemplate");
			b.Source = this;
			BindingOperations.SetBinding(NavPaneContentPresenter, ContentPresenter.ContentTemplateProperty, b);
		}
		protected virtual void UpdateVisualStateByExpandState() {
			VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", false);
		}
		bool CoerceIsExpandedCore(bool newValue, bool oldValue) {
			if (oldValue != newValue) {
				NavPaneExpandedChangingEventArgs e = new NavPaneExpandedChangingEventArgs(newValue);
				e.RoutedEvent = NavigationPaneView.NavPaneExpandedChangingEvent;
				RaiseEvent(e);
				if (e.Cancel)
					return oldValue;
			}
			return newValue;
		}
		void UpdateActualGroupButtonTemplateSelector() {
			if(NavBar != null)
				NavBar.UpdateGroups((w) => w.UpdateActualNavPaneGroupButtonTemplateSelector());
		}
		void UpdateActualOverflowGroupTemplateSelector() {
			if(NavBar != null)
				NavBar.UpdateGroups((w) => w.UpdateActualNavPaneOverflowGroupTemplateSelector());
		}
		protected virtual void OnItemVisualStyleInPopupChanged() {
		}
		protected virtual void OnIsPopupOpenChanged(bool oldValue) {
		}
		void UpdateState() {
			if (NavBar == null)
				return;
			UpdateViewForce();
			UpdatePresenterTemplateByEnabledState();
			UpdateVisualStateByExpandState();
		}
		protected internal override void UpdateView() {
			base.UpdateView();
			UpdateItemsControlGroupsCollection();
			UpdateOverflowControlGroupsCollection();
		}
		protected internal override void UpdateViewForce() {
			base.UpdateViewForce();
			UpdateView();
			Actions.Execute(this);
		}
	}
	interface IAction {
		bool IsSimilar(IAction action);
		void Execute(FrameworkElement panel);
	}
	class ActionsList : IAction {
		List<IAction> actions = new List<IAction>();
		public int Count { get { return actions.Count; } }
		public ActionsList() {
		}
		public void Execute(FrameworkElement panel) {
			foreach(IAction action in actions)
				action.Execute(panel);
			actions.Clear();
		}
		public void Add(IAction action) {
			for(int i = actions.Count - 1; i >= 0; i--) {
				IAction current = actions[i];
				if(current.IsSimilar(action))
					actions.RemoveAt(i);
			}
			actions.Add(action);
		}
		public bool IsSimilar(IAction action) {
			return false;
		}
	}
	abstract class ChangeCollectionAction : IAction {
		protected NavigationPaneView View { get; set; }
		public ChangeCollectionAction(NavigationPaneView view) {
			View = view;
		}
		#region IAction Members
		public abstract void Execute(FrameworkElement panel);
		public bool IsSimilar(IAction action) {
			return action.GetType().Equals(GetType());
		}
		#endregion
	}
	class UpdateItemsControlGroupsAction : ChangeCollectionAction {
		public UpdateItemsControlGroupsAction(NavigationPaneView view)
			: base(view) {
		}
		public override void Execute(FrameworkElement panel) {
			var itemsControlGroups = new ObservableCollection<NavBarGroup>();
			for(int i = 0; i < View.NavBar.Groups.Count; i++) {
				var group = View.NavBar.Groups[i];
				if(group.IsNavPaneGroup)
					itemsControlGroups.Add(group);
			}
			View.ItemsControlGroups = new ReadOnlyObservableCollection<NavBarGroup>(itemsControlGroups);
		}
	}
	class UpdateOverflowGroupsAction : ChangeCollectionAction {
		public UpdateOverflowGroupsAction(NavigationPaneView view)
			: base(view) {
		}
		public override void Execute(FrameworkElement panel) {
			var navPaneGroups = new List<NavBarGroup>(View.NavBar.Groups.Where(group => group.IsNavPaneGroup));
			var overflowGroups = new ObservableCollection<NavBarGroup>();
			for(int i = View.ActualVisibleGroupCount; i < navPaneGroups.Count; i++) {
				var group = navPaneGroups[i];
				if(!group.IsNavPaneGroup)
					continue;
				overflowGroups.Add(group);
			}
			View.OverflowPanelGroups = new ReadOnlyObservableCollection<NavBarGroup>(overflowGroups);
			foreach (var group in View.NavBar.Groups) { 
				group.IsOverflowGroup = View.OverflowPanelGroups.Contains(group);
				group.UpdateMenuGroupItem();
			}
		}
	}
	public class NavigationPaneExpandInfoProvider : DependencyObject {
		#region props
		public DXExpander Expander {
			get { return (DXExpander)GetValue(ExpanderProperty); }
			set { SetValue(ExpanderProperty, value); }
		}
		public bool IsExpanding {
			get { return (bool)GetValue(IsExpandingProperty); }
			set { SetValue(IsExpandingProperty, value); }
		}
		public bool IsCollapsing {
			get { return (bool)GetValue(IsCollapsingProperty); }
			set { SetValue(IsCollapsingProperty, value); }
		}
		public bool IsJustCollapsed {
			get { return (bool)GetValue(IsJustCollapsedProperty); }
			set { SetValue(IsJustCollapsedProperty, value); }
		}
		public bool IsJustExpanded {
			get { return (bool)GetValue(IsJustExpandedProperty); }
			set { SetValue(IsJustExpandedProperty, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public NavigationPaneView View { get; set; }
		#endregion
		#region static
		public static bool GetIsCompleteCollapsed(DependencyObject obj) {
			return (bool)obj.GetValue(IsCompleteCollapsedProperty);
		}
		public static void SetIsCompleteCollapsed(DependencyObject obj, bool value) {
			obj.SetValue(IsCompleteCollapsedProperty, value);
		}
		public static readonly DependencyProperty IsCompleteCollapsedProperty =
			DependencyProperty.RegisterAttached("IsCompleteCollapsed", typeof(bool), typeof(NavigationPaneExpandInfoProvider), new PropertyMetadata(false));
		public static readonly DependencyProperty IsJustCollapsedProperty =
			DependencyPropertyManager.Register("IsJustCollapsed", typeof(bool), typeof(NavigationPaneExpandInfoProvider), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsCollapsingProperty =
			DependencyPropertyManager.Register("IsCollapsing", typeof(bool), typeof(NavigationPaneExpandInfoProvider), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCollapsingPropertyChanged)));
		public static readonly DependencyProperty IsJustExpandedProperty =
			DependencyPropertyManager.Register("IsJustExpanded", typeof(bool), typeof(NavigationPaneExpandInfoProvider), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(NavigationPaneExpandInfoProvider), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty IsExpandingProperty =
			DependencyPropertyManager.Register("IsExpanding", typeof(bool), typeof(NavigationPaneExpandInfoProvider), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsExpandingPropertyChanged)));
		public static readonly DependencyProperty ExpanderProperty =
			DependencyPropertyManager.Register("Expander", typeof(DXExpander), typeof(NavigationPaneExpandInfoProvider), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnExpanderPropertyChanged)));
		public NavigationPaneExpandInfoProvider(NavigationPaneView view) {
			View = view;
		}
		protected static void OnIsCollapsingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneExpandInfoProvider)d).OnIsCollapsingChanged((bool)e.OldValue);
		}
		protected static void OnIsExpandingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneExpandInfoProvider)d).OnIsExpandingChanged((bool)e.OldValue);
		}
		protected static void OnExpanderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavigationPaneExpandInfoProvider)d).OnExpanderChanged((DXExpander)e.OldValue);
		}
		#endregion
		protected virtual void OnExpanderChanged(DXExpander oldValue) {
			ClearValue(NavigationPaneExpandInfoProvider.IsExpandedProperty);
			ClearValue(NavigationPaneExpandInfoProvider.IsExpandingProperty);
			ClearValue(NavigationPaneExpandInfoProvider.IsCollapsingProperty);
			if(Expander != null) {
				BindingOperations.SetBinding(this, NavigationPaneExpandInfoProvider.IsExpandedProperty, new Binding("IsExpanded") { Source = Expander });
				BindingOperations.SetBinding(this, NavigationPaneExpandInfoProvider.IsExpandingProperty, new Binding("Expanding") { Source = Expander });
				BindingOperations.SetBinding(this, NavigationPaneExpandInfoProvider.IsCollapsingProperty, new Binding("Collapsing") { Source = Expander });
			}
		}
		protected virtual void OnIsExpandingChanged(bool oldValue) {
			if(!IsExpanding && oldValue) {
				IsJustExpanded = true;
				Dispatcher.BeginInvoke(new Action(() => { IsJustExpanded = false; }));
			}
			if (IsExpanding) {
				View.UpdateGroupsDisplayMode(DisplayMode.Text);
			}
		}
		protected virtual void OnIsCollapsingChanged(bool oldValue) {
			if(!IsCollapsing && oldValue) {
				IsJustCollapsed = true;
				Dispatcher.BeginInvoke(new Action(() => { IsJustCollapsed = false; }));
				View.UpdateGroupsDisplayMode(DisplayMode.ImageAndText);
			}
		}
	}
	public class NavPanePopup : Popup {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ActualParentProperty;
		bool? menuDropAlignment = null;
		static NavPanePopup() {
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(NavPanePopup), new PropertyMetadata(Orientation.Vertical, (d, e) => ((NavPanePopup)d).OnOrientationPropertyChanged()));
			ActualParentProperty = DependencyPropertyManager.Register("ActualParent", typeof(FrameworkElement), typeof(NavPanePopup), new PropertyMetadata(null, (d, e) => ((NavPanePopup)d).OnActualParentPropertyChanged()));
		}
		public NavPanePopup() {
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}		
		public void ClosePopup() {			
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public FrameworkElement ActualParent {
			get { return (FrameworkElement)GetValue(ActualParentProperty); }
			set { SetValue(ActualParentProperty, value); }
		}
		void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			this.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
		}
		void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			CollapsedActiveGroup.AllowCollapsed = false;
		}
		void OnOrientationPropertyChanged() {
			UpdateHeightProperty();
		}
		void OnActualParentPropertyChanged() {
			UpdateHeightProperty();
		}
		void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			this.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
		}
		protected override void OnOpened(EventArgs e) {
			base.OnOpened(e);
			UpdateHorizontalOffset();
		}
		internal void UpdateHorizontalOffset() {
			if (!menuDropAlignment.HasValue || (menuDropAlignment.HasValue && menuDropAlignment.Value != SystemParameters.MenuDropAlignment)) {
				if (SystemParameters.MenuDropAlignment)
					HorizontalOffset = (PlacementTarget as FrameworkElement).ActualWidth + (Child as FrameworkElement).RenderSize.Width;
				else
					HorizontalOffset = 1d;
				menuDropAlignment = SystemParameters.MenuDropAlignment;
			}
		}
		void UpdateHeightProperty() {
			if(ActualParent == null)
				return;
			string path = Orientation == Orientation.Vertical ? "ActualHeight" : "ActualWidth";
			SetBinding(HeightProperty, new Binding(path) { Source = ActualParent });
		}
	}
	[TemplateVisualState(Name = "Vertical", GroupName = "OrientationStates"), TemplateVisualState(Name = "Horizontal", GroupName = "OrientationStates")]
	public class CollapsedActiveGroup : ToggleButton {
		public static bool AllowCollapsed = true;
		internal static readonly DependencyProperty ElementProperty;
		internal static readonly DependencyProperty OrientationProperty;
		public CollapsedActiveGroup() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		static CollapsedActiveGroup() {
			ElementProperty = DependencyPropertyManager.Register("Element", typeof(Element), typeof(CollapsedActiveGroup), new PropertyMetadata(Element.ActiveGroup, (d, e) => ((CollapsedActiveGroup)d).OnElementPropertyChanged()));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(CollapsedActiveGroup), new PropertyMetadata(Orientation.Vertical, (d,e)=> ((CollapsedActiveGroup)d).OnOrientationPropertyChanged()));
		}
		internal Element Element {
			get { return (Element)GetValue(ElementProperty); }
			set { SetValue(ElementProperty, value); }
		}
		internal Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetBindings();
			UpdateForegroundStates();
			UpdateOrientationStates();
		}
		UIElement Root { get; set; }
		UIElement GetRootElement() {
			return DevExpress.Xpf.Core.Native.LayoutHelper.GetRoot(this);
		}
		void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			Subscribe();
		}
		void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			AllowCollapsed = false;
		}
		void OnMouseLeftButtonDownOnRootVisual(object sender, MouseButtonEventArgs e) {
			if(AllowCollapsed && IsChecked.HasValue && IsChecked.Value)
				IsChecked = false;
		}
		void OnMouseLeftButtonUpOnRootVisual(object sender, MouseButtonEventArgs e) {
			AllowCollapsed = true;
		}
		void ClosePopupOnWindowAction(object sender, EventArgs e) {
			IsChecked = false;
			AllowCollapsed = true;
		}
		void OnElementPropertyChanged() {
			UpdateForegroundStates();
		}
		void OnOrientationPropertyChanged() {
			UpdateOrientationStates();
		}
		void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			Unsubscribe();
		}
		void Subscribe() {			
			Unsubscribe();
			UIElement root = GetRootElement();
			Root = root;
			if(root == null)
				return;
			Window window = root as Window;
			if(window != null) {
				window.Activated += ClosePopupOnWindowAction;
				window.Deactivated += ClosePopupOnWindowAction;
				window.StateChanged += ClosePopupOnWindowAction;
				window.SizeChanged += ClosePopupOnWindowAction;
				window.LocationChanged += ClosePopupOnWindowAction;
			}
			root.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDownOnRootVisual), true);
			root.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUpOnRootVisual), true);
			this.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
		}
		void Unsubscribe() {
			UIElement root = Root;
			Root = null;
			if(root == null)
				return;
			Window window = root as Window;
			if(window != null) {
				window.Activated -= ClosePopupOnWindowAction;
				window.Deactivated -= ClosePopupOnWindowAction;
				window.StateChanged -= ClosePopupOnWindowAction;
				window.SizeChanged -= ClosePopupOnWindowAction;
				window.LocationChanged -= ClosePopupOnWindowAction;
			}
			root.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDownOnRootVisual));
			root.RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUpOnRootVisual));
			this.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
		}
		void SetBindings() {			
			SetBinding(ElementProperty, new Binding("(0)") { Path = new PropertyPath(NavigationPaneView.ElementProperty), RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
			SetBinding(OrientationProperty, new Binding("NavBar.View.Orientation"));
			SetBinding(OpacityProperty, new Binding("NavBar.View.Expander.AnimationProgress") { Converter = new DoubleInvertConverter() { MaxValue = 1 } });
		}
		protected internal void UpdateForegroundStates() {
			VisualStateManager.GoToState(this, Element == Element.CollapsedActiveGroup ? "CustomForeground" : "NormalForeground", false);
		}
		void UpdateOrientationStates() {
			VisualStateManager.GoToState(this, Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical", false);
		}
	}
	[TemplateVisualState(Name = "NormalMode", GroupName = "ExpandModeStates")]
	[TemplateVisualState(Name = "Invert", GroupName = "ExpandModeStates")]
	public class NavPaneExpandButton : ExpandButtonBase {
		internal static readonly DependencyProperty ExpandButtonModeProperty;
		public NavPaneExpandButton() {
			this.SetDefaultStyleKey(typeof(NavPaneExpandButton));
		}
		static NavPaneExpandButton() {
			ExpandButtonModeProperty = DependencyPropertyManager.Register("ExpandButtonMode", typeof(ExpandButtonMode), typeof(NavPaneExpandButton), new PropertyMetadata(ExpandButtonMode.Normal, (d, e) => ((NavPaneExpandButton)d).OnExpandButtonModePropertyChanged()));
			IsManipulationEnabledProperty.OverrideMetadata(typeof(NavPaneExpandButton), new FrameworkPropertyMetadata(true));
		}
		internal ExpandButtonMode ExpandButtonMode {
			get { return (ExpandButtonMode)GetValue(ExpandButtonModeProperty); }
			set { SetValue(ExpandButtonModeProperty, value); }
		}
		public override void OnApplyTemplate() {
			NavBarControl navBar = NavBarViewBase.FindAncestorCore<NavBarControl>(this);
			if(navBar != null)
				SetBinding(ExpandButtonModeProperty, new Binding("View.ExpandButtonMode") { Source = navBar });
			UpdateExpandModeStates();
			base.OnApplyTemplate();
		}
		protected internal virtual void OnExpandButtonModePropertyChanged() {
			UpdateExpandModeStates();
		}
		void UpdateExpandModeStates() {
			VisualStateManager.GoToState(this, ExpandButtonMode == ExpandButtonMode.Normal ? "NormalMode" : "Invert", false);
		}
		protected internal override void SetBindings() {
			SetBinding(IsExpandedProperty, new Binding("NavBar.View.IsExpanded"));
		}
		#region Touch
		bool emulateClick = false;
		TouchPoint point = null;
		protected override void OnTouchDown(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchDown(e);
			point = e.GetTouchPoint(this);
			emulateClick = true;
		}
		protected override void OnTouchMove(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchMove(e);
			if(point == null || !(object.Equals(e.GetTouchPoint(this).Position, point.Position)))
				emulateClick = false;
		}
		protected override void OnTouchUp(System.Windows.Input.TouchEventArgs e) {
			base.OnTouchUp(e);
			if(emulateClick) {
				point = null;
				emulateClick = false;
				if(!e.Handled) {
					OnClick();
					e.Handled = true;
				}
			}
		}
		#endregion
	}
	public class NavPaneContentPresenter : DXContentPresenter {
		const string DefaultTemplateXAML =
	@"<ControlTemplate TargetType='local:DXContentPresenter' " +
		"xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
		"xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
		"xmlns:local='clr-namespace:DevExpress.Xpf.Core;assembly=DevExpress.Xpf.Core" + AssemblyInfo.VSuffix + "'>" +
		"<ContentPresenter Content='{TemplateBinding Content}' ContentTemplate='{TemplateBinding ContentTemplate}' DataContext='{x:Null}' FlowDirection='{TemplateBinding FlowDirection}'/>" +
	"</ControlTemplate>";
		static ControlTemplate _DefaultTemplate;
		static ControlTemplate DefaultTemplate
		{
			get
			{
				if (_DefaultTemplate == null)
					_DefaultTemplate = (ControlTemplate)XamlReader.Parse(DefaultTemplateXAML);
				return _DefaultTemplate;
			}
		}
		public object ActualContent {
			get { return (object)GetValue(ActualContentProperty); }
			set { SetValue(ActualContentProperty, value); }
		}
		static NavPaneContentPresenter() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(NavPaneContentPresenter), new FrameworkPropertyMetadata(typeof(NavPaneContentPresenter)));
			FocusableProperty.OverrideMetadata(typeof(NavPaneContentPresenter), new FrameworkPropertyMetadata(false));
		}
		public NavPaneContentPresenter() {
			Template = DefaultTemplate;
			Loaded += new RoutedEventHandler(NavPaneContentPresenter_Loaded);
			Unloaded += new RoutedEventHandler(NavPaneContentPresenter_Unloaded);
		}
		bool isLoaded = false;
		void NavPaneContentPresenter_Unloaded(object sender, System.Windows.RoutedEventArgs e) {
			isLoaded = false;
			ClearContent();
		}
		void NavPaneContentPresenter_Loaded(object sender, System.Windows.RoutedEventArgs e) {
			isLoaded = true;
			SetContent();
		}
		public static readonly DependencyProperty ActualContentProperty =
			DependencyPropertyManager.Register("ActualContent", typeof(object), typeof(NavPaneContentPresenter), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActualContentPropertyChanged)));
		protected static void OnActualContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavPaneContentPresenter)d).OnActualContentChanged((object)e.OldValue);
		}
		protected virtual void OnActualContentChanged(object oldValue) {
			if(isLoaded)
				SetContent();
		}
		void ClearContent() {
			UIElement elem = Content as UIElement;
			if (elem != null) {
				elem.IsHitTestVisible = false;
			}
			Content = null;
		}
		void SetContent() {
			ClearContent();
			Content = ActualContent;
			UIElement elem = Content as UIElement;
			if (elem != null) {
				elem.IsHitTestVisible = true;
				var valueSource = System.Windows.DependencyPropertyHelper.GetValueSource(elem, ForegroundProperty).BaseValueSource;
				if (valueSource == BaseValueSource.Default || valueSource == BaseValueSource.Inherited) {
					var group = DataContext as NavBarGroup;
					if (group != null)
						elem.SetCurrentValue(ForegroundProperty, System.Windows.Documents.TextElement.GetForeground(group));
				}
			}
		}
	}
	public class NavPaneFlyoutControl : FlyoutControl {
		public NavPaneFlyoutControl() {
			StaysOpen = false;			
		}
		protected internal new Popup Popup { get { return base.Popup; } }		
	}
}
