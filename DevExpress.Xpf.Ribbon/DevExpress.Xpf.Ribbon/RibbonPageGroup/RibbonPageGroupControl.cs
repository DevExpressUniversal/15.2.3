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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Ribbon.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageGroupControl : RibbonLinksControl {
		#region static
		public static readonly DependencyProperty IsCollapsedProperty;
		public static readonly DependencyProperty IsSeparatorVisibleProperty;
		public static readonly DependencyProperty IsPopupOpenedProperty;
		protected static readonly DependencyPropertyKey IsPopupOpenedPropertyKey;
		public static readonly DependencyProperty PageGroupProperty;
		public static readonly DependencyProperty BorderTemplateProperty;
		public static readonly DependencyProperty ActualBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualBorderTemplatePropertyKey;
		public static readonly DependencyProperty BorderTemplateInPopupProperty;
		public static readonly DependencyProperty HighlightedBorderTemplateProperty;
		public static readonly DependencyProperty HighlightedBorderTemplateInPopupProperty;
		public static readonly DependencyProperty ActualPopupButtonContentBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualPopupButtonContentBorderTemplatePropertyKey;
		public static readonly DependencyProperty ActualPopupButtonBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualPopupButtonBorderTemplatePropertyKey;
		public static readonly DependencyProperty ActualPopupButtonGlyphBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualPopupButtonGlyphBorderTemplatePropertyKey;
		public static readonly DependencyProperty PopupButtonBorderTemplateProperty;
		public static readonly DependencyProperty PopupButtonContentBorderTemplateProperty;
		public static readonly DependencyProperty PopupButtonGlyphBorderTemplateProperty;
		public static readonly DependencyProperty HighlightedPopupButtonBorderTemplateProperty;
		public static readonly DependencyProperty HighlightedPopupButtonContentBorderTemplateProperty;
		public static readonly DependencyProperty HighlightedPopupButtonGlyphBorderTemplateProperty;
		public static readonly DependencyProperty ActualCaptionBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualCaptionBorderTemplatePropertyKey;
		public static readonly DependencyProperty CaptionBorderTemplateProperty;
		public static readonly DependencyProperty HighlightedCaptionBorderTemplateProperty;
		public static readonly DependencyProperty PopupHorizontalOffsetProperty;
		public static readonly DependencyProperty PopupVerticalOffsetProperty;
		public static readonly DependencyProperty IsPageGroupVisibleProperty;
		public static readonly DependencyProperty AllowCollapseProperty;
		public static readonly DependencyProperty RibbonStyleProperty;
		public static readonly DependencyProperty IsDesignTimeCaptionVisibleProperty;
		public static readonly DependencyProperty IsTabletModeProperty;
		static readonly DependencyPropertyKey IsTabletModePropertyKey;
		public static readonly DependencyProperty IndexProperty;
		public static readonly DependencyProperty IsMergedProperty;
		static RibbonPageGroupControl() {
			Type ownerType = typeof(RibbonPageGroupControl);
			get_CollectionView = ReflectionHelper.CreateFieldGetter<ItemCollection, CollectionView>(typeof(ItemCollection), "_collectionView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			IsMergedProperty = DependencyProperty.Register("IsMerged", typeof(bool), typeof(RibbonPageGroupControl), new FrameworkPropertyMetadata(false, (d,e)=>((RibbonPageGroupControl)d).OnIsMergedChanged()));
			IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(RibbonPageGroupControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentArrange));
			IsCollapsedProperty = DependencyPropertyManager.Register("IsCollapsed", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsCollapsedPropertyChanged)));
			IsSeparatorVisibleProperty = DependencyPropertyManager.Register("IsSeparatorVisible", typeof(bool), ownerType, new PropertyMetadata(true));
			IsPopupOpenedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsPopupOpened", typeof(bool), ownerType, new PropertyMetadata(false, OnIsPopupOpenedPropertyChanged));
			IsPopupOpenedProperty = IsPopupOpenedPropertyKey.DependencyProperty;
			PageGroupProperty = DependencyPropertyManager.Register("PageGroup", typeof(RibbonPageGroup), ownerType, new PropertyMetadata(null, OnPageGroupPropertyChanged));
			BorderTemplateProperty = DependencyPropertyManager.Register("BorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnBorderTemplatePropertyChanged)));
			BorderTemplateInPopupProperty = DependencyPropertyManager.Register("BorderTemplateInPopup", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnBorderTemplateInPopupPropertyChanged)));
			HighlightedBorderTemplateProperty = DependencyPropertyManager.Register("HighlightedBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedBorderTemplatePropertyChanged)));
			HighlightedBorderTemplateInPopupProperty = DependencyPropertyManager.Register("HighlightedBorderTemplateInPopup", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedBorderTemplateInPopupPropertyChanged)));
			ActualBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorderTemplate", typeof(ControlTemplate), ownerType, new UIPropertyMetadata(null));
			ActualBorderTemplateProperty = ActualBorderTemplatePropertyKey.DependencyProperty;
			ActualPopupButtonContentBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPopupButtonContentBorderTemplate", typeof(ControlTemplate), ownerType, new UIPropertyMetadata(null));
			ActualPopupButtonContentBorderTemplateProperty = ActualPopupButtonContentBorderTemplatePropertyKey.DependencyProperty;
			ActualPopupButtonBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPopupButtonBorderTemplate", typeof(ControlTemplate), ownerType, new UIPropertyMetadata(null));
			ActualPopupButtonBorderTemplateProperty = ActualPopupButtonBorderTemplatePropertyKey.DependencyProperty;
			ActualPopupButtonGlyphBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPopupButtonGlyphBorderTemplate", typeof(ControlTemplate), ownerType, new UIPropertyMetadata(null));
			ActualPopupButtonGlyphBorderTemplateProperty = ActualPopupButtonGlyphBorderTemplatePropertyKey.DependencyProperty;
			PopupButtonBorderTemplateProperty = DependencyPropertyManager.Register("PopupButtonBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnPopupButtonBorderTemplatePropertyChanged)));
			PopupButtonContentBorderTemplateProperty = DependencyPropertyManager.Register("PopupButtonContentBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnPopupButtonContentBorderTemplatePropertyChanged)));
			PopupButtonGlyphBorderTemplateProperty = DependencyPropertyManager.Register("PopupButtonGlyphBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnPopupButtonGlyphBorderTemplatePropertyChanged)));
			RibbonStyleProperty = RibbonControl.RibbonStyleProperty.AddOwner(ownerType, new PropertyMetadata(OnRibbonStylePropertyChanged));
			HighlightedPopupButtonBorderTemplateProperty = DependencyPropertyManager.Register("HighlightedPopupButtonBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedPopupButtonBorderTemplatePropertyChanged)));
			HighlightedPopupButtonContentBorderTemplateProperty = DependencyPropertyManager.Register("HighlightedPopupButtonContentBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedPopupButtonContentBorderTemplatePropertyChanged)));
			HighlightedPopupButtonGlyphBorderTemplateProperty = DependencyPropertyManager.Register("HighlightedPopupButtonGlyphBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedPopupButtonGlyphBorderTemplatePropertyChanged)));
			ActualCaptionBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCaptionBorderTemplate", typeof(ControlTemplate), ownerType, new UIPropertyMetadata(null));
			ActualCaptionBorderTemplateProperty = ActualCaptionBorderTemplatePropertyKey.DependencyProperty;
			CaptionBorderTemplateProperty = DependencyPropertyManager.Register("CaptionBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnCaptionBorderTemplatePropertyChanged)));
			HighlightedCaptionBorderTemplateProperty = DependencyPropertyManager.Register("HighlightedCaptionBorderTemplate", typeof(ControlTemplate), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedCaptionBorderTemplatePropertyChanged)));
			PopupHorizontalOffsetProperty = DependencyPropertyManager.Register("PopupHorizontalOffset", typeof(double), ownerType, new UIPropertyMetadata(0d));
			PopupVerticalOffsetProperty = DependencyPropertyManager.Register("PopupVerticalOffset", typeof(double), ownerType, new UIPropertyMetadata(0d));
			IsPageGroupVisibleProperty = DependencyPropertyManager.Register("IsPageGroupVisible", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, OnIsPageGroupVisiblePropertyChanged, CoerceIsPageGroupVisibleProperty));
			AllowCollapseProperty = DependencyPropertyManager.Register("AllowCollapse", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, OnAllowCollapsePropertyChanged));
			IsDesignTimeCaptionVisibleProperty = DependencyPropertyManager.Register("IsDesignTimeCaptionVisible", typeof(bool), ownerType, new PropertyMetadata(false));
			IsTabletModePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsTabletMode", typeof(bool), ownerType, new PropertyMetadata(false));
			IsTabletModeProperty = IsTabletModePropertyKey.DependencyProperty;
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(ownerType, typeof(RibbonPageGroupControlAutomationPeer), owner => new RibbonPageGroupControlAutomationPeer((RibbonPageGroupControl)owner));
		}		
		protected static object CoerceIsPageGroupVisibleProperty(DependencyObject d, object baseValue) {
			return ((RibbonPageGroupControl)d).CoerceIsPageGroupVisibleProperty(baseValue);
		}
		protected object CoerceIsPageGroupVisibleProperty(object value) {
			if (PageGroup == null) return value;
			return (bool)value & (!PageGroup.ActualHideWhenEmpty || HaveVisibleInfos);
		}
		protected static void OnIsCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).OnIsCollapsedChanged((bool)e.OldValue);
		}
		protected static void OnIsPopupOpenedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).OnIsPopupOpenedChanged((bool)e.OldValue);
		}
		protected static void OnPageGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).OnPageGroupChanged((RibbonPageGroup)e.OldValue);
		}
		protected static void OnBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnHighlightedBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnBorderTemplateInPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnHighlightedBorderTemplateInPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnPopupButtonBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnPopupButtonContentBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnPopupButtonGlyphBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnHighlightedPopupButtonBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnHighlightedPopupButtonContentBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnHighlightedPopupButtonGlyphBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnCaptionBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnHighlightedCaptionBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).UpdateActualBorders();
		}
		protected static void OnIsPageGroupVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).OnIsPageGroupVisibleChanged((bool)e.OldValue);
		}
		protected static void OnAllowCollapsePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).OnAllowCollapseChanged((bool)e.OldValue);
		}
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageGroupControl)d).OnRibbonStyleChanged((RibbonStyle)e.OldValue);
		}
		#endregion
		#region dep props
		public ControlTemplate BorderTemplate {
			get { return (ControlTemplate)GetValue(BorderTemplateProperty); }
			set { SetValue(BorderTemplateProperty, value); }
		}
		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			set { SetValue(IsCollapsedProperty, value); }
		}
		public bool IsPopupOpened {
			get { return (bool)GetValue(IsPopupOpenedProperty); }
			protected set { this.SetValue(IsPopupOpenedPropertyKey, value); }
		}				
		public RibbonPageGroup PageGroup {
			get { return (RibbonPageGroup)GetValue(PageGroupProperty); }
			set { SetValue(PageGroupProperty, value); }
		}
		public ControlTemplate ActualBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualBorderTemplateProperty); }
			protected set { this.SetValue(ActualBorderTemplatePropertyKey, value); }
		}
		public ControlTemplate BorderTemplateInPopup {
			get { return (ControlTemplate)GetValue(BorderTemplateInPopupProperty); }
			set { SetValue(BorderTemplateInPopupProperty, value); }
		}
		public ControlTemplate HighlightedBorderTemplate {
			get { return (ControlTemplate)GetValue(HighlightedBorderTemplateProperty); }
			set { SetValue(HighlightedBorderTemplateProperty, value); }
		}
		public ControlTemplate HighlightedBorderTemplateInPopup {
			get { return (ControlTemplate)GetValue(HighlightedBorderTemplateInPopupProperty); }
			set { SetValue(HighlightedBorderTemplateInPopupProperty, value); }
		}
		public ControlTemplate ActualPopupButtonContentBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualPopupButtonContentBorderTemplateProperty); }
			protected set { this.SetValue(ActualPopupButtonContentBorderTemplatePropertyKey, value); }
		}
		public ControlTemplate ActualPopupButtonBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualPopupButtonBorderTemplateProperty); }
			protected set { this.SetValue(ActualPopupButtonBorderTemplatePropertyKey, value); }
		}
		public ControlTemplate ActualPopupButtonGlyphBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualPopupButtonGlyphBorderTemplateProperty); }
			protected set { this.SetValue(ActualPopupButtonGlyphBorderTemplatePropertyKey, value); }
		}
		public ControlTemplate PopupButtonBorderTemplate {
			get { return (ControlTemplate)GetValue(PopupButtonBorderTemplateProperty); }
			set { SetValue(PopupButtonBorderTemplateProperty, value); }
		}
		public ControlTemplate PopupButtonContentBorderTemplate {
			get { return (ControlTemplate)GetValue(PopupButtonContentBorderTemplateProperty); }
			set { SetValue(PopupButtonContentBorderTemplateProperty, value); }
		}
		public ControlTemplate PopupButtonGlyphBorderTemplate {
			get { return (ControlTemplate)GetValue(PopupButtonGlyphBorderTemplateProperty); }
			set { SetValue(PopupButtonGlyphBorderTemplateProperty, value); }
		}
		public ControlTemplate HighlightedPopupButtonBorderTemplate {
			get { return (ControlTemplate)GetValue(HighlightedPopupButtonBorderTemplateProperty); }
			set { SetValue(HighlightedPopupButtonBorderTemplateProperty, value); }
		}
		public ControlTemplate HighlightedPopupButtonContentBorderTemplate {
			get { return (ControlTemplate)GetValue(HighlightedPopupButtonContentBorderTemplateProperty); }
			set { SetValue(HighlightedPopupButtonContentBorderTemplateProperty, value); }
		}
		public ControlTemplate HighlightedPopupButtonGlyphBorderTemplate {
			get { return (ControlTemplate)GetValue(HighlightedPopupButtonGlyphBorderTemplateProperty); }
			set { SetValue(HighlightedPopupButtonGlyphBorderTemplateProperty, value); }
		}
		public ControlTemplate CaptionBorderTemplate {
			get { return (ControlTemplate)GetValue(CaptionBorderTemplateProperty); }
			set { SetValue(CaptionBorderTemplateProperty, value); }
		}
		public ControlTemplate HighlightedCaptionBorderTemplate {
			get { return (ControlTemplate)GetValue(HighlightedCaptionBorderTemplateProperty); }
			set { SetValue(HighlightedCaptionBorderTemplateProperty, value); }
		}
		public bool IsMerged {
			get { return (bool)GetValue(IsMergedProperty); }
			set { SetValue(IsMergedProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public ControlTemplate ActualCaptionBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualCaptionBorderTemplateProperty); }
			protected set { this.SetValue(ActualCaptionBorderTemplatePropertyKey, value); }
		}
		public double PopupHorizontalOffset {
			get { return (double)GetValue(PopupHorizontalOffsetProperty); }
			set { SetValue(PopupHorizontalOffsetProperty, value); }
		}
		public double PopupVerticalOffset {
			get { return (double)GetValue(PopupVerticalOffsetProperty); }
			set { SetValue(PopupVerticalOffsetProperty, value); }
		}
		public bool IsPageGroupVisible {
			get { return (bool)GetValue(IsPageGroupVisibleProperty); }
			set { SetValue(IsPageGroupVisibleProperty, value); }
		}
		public bool AllowCollapse {
			get { return (bool)GetValue(AllowCollapseProperty); }
			set { SetValue(AllowCollapseProperty, value); }
		}
		public bool IsDesignTimeCaptionVisible {
			get { return (bool)GetValue(IsDesignTimeCaptionVisibleProperty); }
			set { SetValue(IsDesignTimeCaptionVisibleProperty, value); }
		}
		public bool IsSeparatorVisible {
			get { return (bool)GetValue(IsSeparatorVisibleProperty); }
			set { SetValue(IsSeparatorVisibleProperty, value); }
		}
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public bool IsTabletMode {
			get { return (bool)GetValue(IsTabletModeProperty); }
			protected set { SetValue(IsTabletModePropertyKey, value); }
		}
		#endregion
		#region props
		private RibbonControl ribbonCore = null;
		private RibbonPageGroupControl ownerCore = null;
		public RibbonControl Ribbon {
			get { return ribbonCore; }
			protected internal set {
				if(ribbonCore == value) return;
				RibbonControl oldValue = ribbonCore;
				ribbonCore = value;
				OnRibbonChanged(oldValue);
			}
		}
		public RibbonPageGroupControl Owner {
			get { return ownerCore; }
			set {
				if(ownerCore == value) return;
				RibbonPageGroupControl oldValue = ownerCore;
				ownerCore = value;
				OnOwnerChanged(oldValue);
			}
		}		
		public RibbonPageGroupsControl PageGroupsControl {
			get { return ItemsControl.ItemsControlFromItemContainer(this) as RibbonPageGroupsControl; }
		}
		PageGroupCollapseButtonInfo collapseButtonInfo;
		protected internal PageGroupCollapseButtonInfo CollapseButtonInfo {
			get {
				if(collapseButtonInfo == null && !IsOrigin)
					collapseButtonInfo = new PageGroupCollapseButtonInfo(this);
				return collapseButtonInfo;
			}
		}
		public IEnumerable<BarItemLinkBase> CollapsedItemLinks {
			get { return GetCollapsedItemLinks(); }
		}
		PageGroupControlCollaspeItemsStrategyBase collapseItemsStrategy;
		public PageGroupControlCollaspeItemsStrategyBase CollapseItemsStrategy {
			get { return collapseItemsStrategy; }
			private set {
				if (collapseItemsStrategy == value)
					return;
				var oldValue = collapseItemsStrategy;
				collapseItemsStrategy = value;
				OnCollapseItemsStrategyChanged(oldValue);
			}
		}
		#endregion
		public RibbonPageGroupControl() {
			ContainerType = LinkContainerType.RibbonPageGroup;
			IsTabletMode = IsRibbonTablet(RibbonStyle);
			UpdateCollapseItemsStrategy();
			IsVisibleChanged += OnIsVisibleChanged;
			refreshAction = new PostponedAction(() => true);
		}		
		protected RibbonSelectedPageControl SelectedPageControl {
			get {
				if(PageGroupsControl == null || PageGroupsControl.PagesControl == null)
					return null;
				return PageGroupsControl.PagesControl.SelectedPageControl;
			}
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override bool OpenPopupsAsMenu { get { return false; } }
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateActualBorders();
			UnsubscribeTemplateEvents();
			SubscribeTemplateEvents();
			UpdateSuperTip();
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			UnsubscribeTemplateEvents();
		}
		protected internal bool IsOrigin { get; set; }
		RibbonPageGroupPopup popupGroup;
		protected internal RibbonPageGroupPopup PopupGroup {
			get { return popupGroup; }
			private set {
				if (value == popupGroup) return;
				RibbonPageGroupPopup oldValue = popupGroup;
				popupGroup = value;
				OnPopupGroupChanged(oldValue);
			}
		}
		protected virtual void OnPopupGroupChanged(RibbonPageGroupPopup oldValue) {
			if (oldValue != null) {
				oldValue.Closed -= OnPopupClosed;
				RemoveVisualChild(oldValue);
				RemoveLogicalChild(oldValue);
			}
			if (PopupGroup != null) {
				PopupGroup.PageGroup = PageGroup;
				PopupGroup.OwnerPageGroupControl = this;
				PopupGroup.Closed += new EventHandler(OnPopupClosed);
				AddLogicalChild(PopupGroup);
				AddVisualChild(PopupGroup);
			}
		}
		protected override IEnumerator LogicalChildren {
			get {
				ArrayList children = new ArrayList();
				if(CollapseButtonInfo != null) {
					children.Add(CollapseButtonInfo.Item);
					children.Add(CollapseButtonInfo.Link);
				}
				return new MergedEnumerator(base.LogicalChildren, new SingleLogicalChildEnumerator(PopupGroup), children.GetEnumerator());
			}
		}
		protected override int VisualChildrenCount { get { return base.VisualChildrenCount + PopupGroup.Return(x => 1, () => 0); } }
		protected override Visual GetVisualChild(int index) { return index == base.VisualChildrenCount ? PopupGroup : base.GetVisualChild(index); }
		protected internal bool HaveVisibleInfos { get; set; }
		protected internal virtual void OpenPopup() {
			CheckInitializePopupGroup();
			PopupGroup.VerticalOffset = PopupVerticalOffset;
			PopupGroup.HorizontalOffset = PopupHorizontalOffset;			
			PopupGroup.ShowPopup(this);
			IsPopupOpened = true;
		}
		protected virtual void CheckInitializePopupGroup() {
			if (PopupGroup != null)
				return;
			PopupGroup = CreatePopupGroup();			
		}
		protected internal virtual void OpenPopup(bool focusPopup) {
			OpenPopup();
			if (focusPopup)
				PopupGroup.Focus();
		}
		protected internal virtual void ClosePopup() {
			if(!IsPopupOpened)
				return;
			ClosePopupCore();
			IsPopupOpened = false;
		}
		protected virtual void ClosePopupCore() {
			if(PopupGroup == null)
				return;			
			PopupGroup.IsOpen = false;
		}
		protected virtual RibbonPageGroupPopup CreatePopupGroup() {
			return new RibbonPageGroupPopup();
		}
		protected BarButtonItemLinkControl OriginBarButtonItemLinkControl { get; set; }
		protected BarButtonItemLinkControl OriginLargeBarButtonItemLinkControl { get; set; }
		protected internal ItemBorderControl CollapsedStateBorder { get; protected set; }
		protected internal RibbonCheckedBorderControl CaptionButton { get; protected set; }
		protected FrameworkElement PopupButton { get; private set; }
		protected internal ContentControl CaptionControl { get; set; }
		protected virtual void OnPageGroupChanged(RibbonPageGroup oldValue) {			
			if (oldValue != null) {
				oldValue.AfterMerge -= OnMergePageGroup;
				oldValue.AfterUnMerge -= OnUnMergePageGroup;
				oldValue.ItemLinks.CollectionChanged -= OnPageGroupItemLinksCollectionChanged;
				oldValue.PageGroupControls.Remove(this);
			}
			RecreateItemsSource();
			UpdateActualBorders();
			if (PageGroup == null)
				return;
			BindingOperations.SetBinding(this, IndexProperty, new Binding() { Path = new PropertyPath(RibbonPageGroup.IndexProperty), Source = PageGroup });
			PageGroup.AfterMerge += new EventHandler(OnMergePageGroup);
			PageGroup.AfterUnMerge += new EventHandler(OnUnMergePageGroup);
			PageGroup.ItemLinks.CollectionChanged += OnPageGroupItemLinksCollectionChanged;
			PageGroup.PageGroupControls.Add(this);
		}
		protected virtual void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) { }
		void OnPageGroupItemLinksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (IsTabletMode)
				RecreateItemsSource();
		}
		void OnMergePageGroup(object sender, EventArgs e) {
			RecreateItemsSource();
		}
		void OnUnMergePageGroup(object sender, EventArgs e) {
			RecreateItemsSource();
		}
		private void OnOwnerChanged(RibbonPageGroupControl ribbonPageGroupControl) {
			UpdateActualBorders();
		}
		protected virtual void OnIsPopupOpenedChanged(bool oldValue) {
			UpdateCollapsedBorderState();
		}
		protected virtual IEnumerable<BarItemLinkBase> GetCollapsedItemLinks() {
			return CollapseItemsStrategy.GetCollapsedLinks(this);
		}
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			base.OnApplyTemplate();
			var itemsPresenter = GetTemplateChild("PART_ItemsPresenter") as Control;
			if(itemsPresenter != null && IsOrigin)
				itemsPresenter.Visibility = Visibility.Collapsed;
			PopupButton = (FrameworkElement)GetTemplateChild("PART_PopupButton");
			CollapsedStateBorder = (ItemBorderControl)GetTemplateChild("PART_PopupButtonBorder");
			CaptionButton = (RibbonCheckedBorderControl)GetTemplateChild("PART_CaptionButton");
			CaptionControl = (ContentControl)GetTemplateChild("PART_Caption");
			OriginBarButtonItemLinkControl = GetTemplateChild("PART_OriginButtonItemLinkControl") as BarButtonItemLinkControl;
			if(OriginBarButtonItemLinkControl != null)
				OriginBarButtonItemLinkControl.Glyph = BarManagerHelper.GetDefaultBarItemGlyph(false);
			OriginLargeBarButtonItemLinkControl = GetTemplateChild("PART_OriginLargeButtonItemLinkControl") as BarButtonItemLinkControl;
			if(OriginLargeBarButtonItemLinkControl != null)
				OriginLargeBarButtonItemLinkControl.Glyph = BarManagerHelper.GetDefaultBarItemGlyph(true);
			if(CollapsedStateBorder != null)
				CollapsedStateBorder.IsActive = true;
			SubscribeTemplateEvents();
			UpdateCollapsedBorderState();
		}
		protected virtual void SubscribeTemplateEvents() {
			if(CaptionButton != null)
				CaptionButton.MouseLeftButtonUp += new MouseButtonEventHandler(OnCaptionButtonMouseLeftButtonUp);
		}
		protected virtual void UnsubscribeTemplateEvents() {
			if(CaptionButton != null)
				CaptionButton.MouseLeftButtonUp -= OnCaptionButtonMouseLeftButtonUp;
		}
		void OnCaptionButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			OnCaptionButtonClick();
		}
		protected internal virtual void OnCaptionButtonClick() {
			if(PageGroup == null) return;
			PageGroup.OnCaptionButtonClick(CaptionButton);
		}
		void OnPopupClosed(object sender, EventArgs e) {
			ClosePopupCore();
		}
		protected virtual void RecreateItemsSource() {
			BarItemLinkInfoCollection oldValue = ItemsSource as BarItemLinkInfoCollection;
			BarItemLinkInfoCollection newValue = CollapseItemsStrategy.GetActualItemSource(this);
			ItemsSource = newValue;			
			if (oldValue != null) {
				oldValue.Source = null;
				oldValue.HaveVisibleInfosChanged -= OnInfoVisibilityChanged;
				oldValue.RequestReset -= OnRequestReset;
			}
			if (newValue != null) {
				newValue.HaveVisibleInfosChanged += OnInfoVisibilityChanged;
				newValue.RequestReset += OnRequestReset;
				OnInfoVisibilityChanged(newValue, new EventArgs());
			}
		}
		readonly PostponedAction refreshAction;
		static readonly Func<ItemCollection, CollectionView> get_CollectionView;
		protected virtual void OnRequestReset(object sender, EventArgs e) {
			refreshAction.PerformPostpone(() => get_CollectionView(Items).Do(x=>x.Refresh()));
		}
		protected override void OnLayoutUpdated(object sender, EventArgs e) {
			base.OnLayoutUpdated(sender, e);
			refreshAction.PerformForce();
		}
		protected virtual void OnIsMergedChanged() {
			UpdateItemsCollectionProperties();			
		}
		protected virtual void UpdateItemsCollectionProperties() {
			if (!IsMerged) {
				if (!Items.SortDescriptions.Contains(new SortDescription("Index", ListSortDirection.Ascending)))
					Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
			} else {
				Items.SortDescriptions.Clear();
			}
			Items.Filter = new Predicate<object>(x => !((BarItemLinkInfo)x).IsRemoved);
		}		
		protected virtual void OnRibbonStyleChanged(RibbonStyle oldValue) {
			IsTabletMode = IsRibbonTablet(RibbonStyle);
			IsDesignTimeCaptionVisible = IsTabletMode && DesignerProperties.GetIsInDesignMode(this);
			UpdateCollapseItemsStrategy();
			if (IsTabletMode) {
				ClosePopup();
				ClearValue(IsCollapsedProperty);
			}
		}
		void UpdateCollapseItemsStrategy() {
			CollapseItemsStrategy = PageGroupControlCollaspeItemsStrategyBase.GetActualStrategy(RibbonStyle);
		}
		protected internal void OnInfoVisibilityChanged(object sender, EventArgs e) {
			var info = (ItemsSource as BarItemLinkInfoCollection);
			if (info != null)
				HaveVisibleInfos = info.HaveVisibleInfos;
			this.CoerceValue(IsPageGroupVisibleProperty);
		}
		public override BarItemLinkCollection ItemLinks {
			get { return PageGroup == null ? null : PageGroup.ItemLinks; }
		}
		protected override NavigationManager CreateNavigationManager() {
			return null;
		}
		protected virtual void OnIsCollapsedChanged(bool oldValue) {
			LayoutHelper.ForEachElement(this, (e) => { e.InvalidateMeasure(); });			
			if(!IsCollapsed) {
				RecreateEditors();
			}
		}
		protected virtual void OnCollapseItemsStrategyChanged(PageGroupControlCollaspeItemsStrategyBase oldValue) {
			RecreateItemsSource();
		}
		protected virtual void SetItemsRibbonStyle(ItemsRange res, RibbonItemStyles style) {
			for(int i = 0; i < res.Count; i++) {
				((BarItemLinkControl)GetChild(res.StartIndex + i)).CurrentRibbonStyle = style;
			}
		}
		bool IsLeftButtonPressed { get { return Mouse.LeftButton == MouseButtonState.Pressed; } }
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if (e.LeftButton == MouseButtonState.Pressed && IsCollapsed && PopupButton.Return(x => x.IsMouseOver, () => true)) {
				if (!IsPopupOpened)
					OpenPopup();
				else ClosePopup();
				e.Handled = true;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			UpdateCollapsedBorderState();
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateCollapsedBorderState();
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateCollapsedBorderState();
		}		
		protected virtual void UpdateCollapsedBorderState() {
			if(CollapsedStateBorder == null)
				return;
			if(IsMouseOver) {
				if(IsPopupOpened || IsLeftButtonPressed)
					CollapsedStateBorder.State = BorderState.HoverChecked;
				else
					CollapsedStateBorder.State = BorderState.Hover;
			}
			else {
				if(IsPopupOpened)
					CollapsedStateBorder.State = BorderState.Checked;
				else
					CollapsedStateBorder.State = BorderState.Normal;
			}
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			UpdateCollapsedBorderState();
		}
		protected virtual bool CanReduceGallery() {
			return FindLargestReducableGallery(GetGalleryList()) != null;
		}
		private bool ReduceGallery() {
			RibbonGalleryBarItemLinkControl gallery = FindLargestReducableGallery(GetGalleryList());
			if(gallery == null)
				return false;
			return gallery.Reduce();
		}
		protected virtual bool CanReduceOfLargeButtons() {
			return FindGroupOfLargeButtons().Count != 0;
		}
		protected virtual bool ReduceGroupOfLargeButtons() {
			ItemsRange res = FindGroupOfLargeButtons();
			if(res.Count == 0)
				return false;
			SetItemsRibbonStyle(res, RibbonItemStyles.SmallWithText);
			return true;
		}
		protected virtual bool CanReduceGroupOfSmallButtonsWithText() {
			return FindGroupOfSmallButtonsWithText().Count != 0;
		}
		protected virtual bool ReduceGroupOfSmallButtonsWithText() {
			ItemsRange res = FindGroupOfSmallButtonsWithText();
			if(res.Count == 0)
				return false;
			SetItemsRibbonStyle(res, RibbonItemStyles.SmallWithoutText);
			return true;
		}
		protected internal virtual List<RibbonGalleryBarItemLinkControl> GetGalleryList() {
			if (ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
				return new List<RibbonGalleryBarItemLinkControl>();
			return Items.OfType<UIElement>().Select(elem => ((BarItemLinkInfo)ItemContainerGenerator.ContainerFromItem(elem)).LinkControl).OfType<RibbonGalleryBarItemLinkControl>().ToList();
		}
		protected internal virtual void Reset() {
			for(int i = 0; i < GetSnapshotItemsCount(); i++) {
				BarItemLinkControlBase linkBase = GetChild(i);
				BarItemLinkControl lc = linkBase as BarItemLinkControl;
				if(lc != null)
					lc.InitializeRibbonStyle();
				BarButtonGroupLinkControl blc = lc as BarButtonGroupLinkControl;
				if(blc != null && blc.ItemsControl != null && blc.ItemsControl.ItemsPresenterCore != null) {
					blc.Height = double.NaN;
					ButtonGroupSetLayoutCalculator.SetRowCount(blc, 2);
					if(blc.LinkInfo != null) {
						ButtonGroupSetLayoutCalculator.SetRowCount(blc.LinkInfo, 2);
					}
				}
			}
		}
		static internal RibbonGalleryBarItemLinkControl FindLargestReducableGallery(List<RibbonGalleryBarItemLinkControl> list) {
			RibbonGalleryBarItemLinkControl largestReducableGallery = null;
			double maxWidth = 0;
			foreach(RibbonGalleryBarItemLinkControl gallery in list) {
				if(gallery.CanReduce() && maxWidth < gallery.ActualWidth) {
					largestReducableGallery = gallery;
					maxWidth = gallery.ActualWidth;
				}
			}
			return largestReducableGallery;
		}
		protected virtual ItemsRange FindGroupOfLargeButtons() {
			for(int i = GetSnapshotItemsCount() - 1; i >= 1; i--) {
				if(IsModifiableLargeButton(GetChild(i) as BarItemLinkControl)) {
					if(IsModifiableLargeButton(GetChild(i - 1) as BarItemLinkControl)) {
						if(i < 2)
							return new ItemsRange(0, 2);
						if(IsModifiableLargeButton(GetChild(i - 2) as BarItemLinkControl))
							return new ItemsRange(i - 2, 3);
						else
							return new ItemsRange(i - 1, 2);
					}
				}
			}
			return new ItemsRange();
		}
		protected virtual ItemsRange FindGroupOfSmallButtonsWithText() {
			for(int i = GetSnapshotItemsCount() - 1; i >= 1; i--) {
				if(IsModifiableSmallButtonWithText(GetChild(i) as BarItemLinkControl)) {
					if(IsModifiableSmallButtonWithText(GetChild(i - 1) as BarItemLinkControl)) {
						if(i < 2)
							return new ItemsRange(0, 2);
						if(IsModifiableSmallButtonWithText(GetChild(i - 2) as BarItemLinkControl))
							return new ItemsRange(i - 2, 3);
						else
							return new ItemsRange(i - 1, 2);
					}
				}
			}
			return new ItemsRange();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			if(SelectedPageControl != null) {
				SelectedPageControl.IsFirstLayoutUpdated = true;
				SelectedPageControl.InvalidateMeasure();
			}
		}
		protected internal virtual bool CanReduce(bool allowCollapse) {
			if(CanReduceGallery())
				return true;
			if(CanReduceButtonGroups())
				return true;
			if(CanReduceOfLargeButtons())
				return true;
			if(CanReduceGroupOfSmallButtonsWithText())
				return true;
			if(allowCollapse && !IsCollapsed)
				return PageGroup.AllowCollapse;
			return false;
		}
		protected internal virtual void Reduce() {
			if(ReduceGallery())
				return;
			if(ReduceButtonGroups())
				return;
			if(ReduceGroupOfLargeButtons())
				return;
			if(ReduceGroupOfSmallButtonsWithText())
				return;
			if(PageGroup.AllowCollapse)
				this.SetCurrentValue(IsCollapsedProperty, true);
		}
		protected virtual internal bool CanReduceButtonGroups() {
			return FindButtonGroupsIn2Rows().Count != 0;
		}
		protected internal virtual bool ReduceButtonGroups() {
			ItemsRange res = FindButtonGroupsIn2Rows();
			if(res.Count == 0)
				return false;
			for(int i = res.StartIndex; i <= res.EndIndex; i++) {
				BarItemLinkControlBase linkControl = GetChild(i);
				ButtonGroupSetLayoutCalculator.SetRowCount(linkControl, 3);
				if(linkControl.LinkInfo != null) {
					ButtonGroupSetLayoutCalculator.SetRowCount(linkControl.LinkInfo, 3);
				}
			}
			return true;
		}
		protected virtual ItemsRange FindButtonGroupsIn2Rows() {
			int startPos = 0;
			ItemsRange res;
			for(; ; ) {
				res = FindButtonGroupsIn2Rows(startPos);
				if(res.Count == 0)
					return res;
				if(IsButtonGroupLayoutCanGiveEffect(res))
					return res;
				startPos = res.EndIndex + 1;
			}
		}
		protected virtual bool IsButtonGroupLayoutCanGiveEffect(ItemsRange range) {
			if(range.Count < 3)
				return false;
			if(ButtonGroupSetLayoutCalculator.GetRowCount(GetChild(range.StartIndex)) == 3)
				return false;
			return true;
		}
		protected virtual ItemsRange FindButtonGroupsIn2Rows(int startIndex) {
			for(int i = startIndex; i < GetSnapshotItemsCount(); i++) {
				if(!IsButtonGroup(i))
					continue;
				return new ItemsRange(i, GetButtonGroupCount(i));
			}
			return new ItemsRange(0, 0);
		}
		protected virtual int GetButtonGroupCount(int startPos) {
			int index;
			for(index = startPos; index < GetSnapshotItemsCount(); index++) {
				if(!IsButtonGroup(index))
					break;
			}
			return index - startPos;
		}
		public bool IsButtonGroup(int index) { return GetChild(index) is BarButtonGroupLinkControl; }
		protected virtual bool IsModifiableLargeButton(BarItemLinkControl linkControl) {
			if(linkControl == null || linkControl.CurrentRibbonStyle != RibbonItemStyles.Large)
				return false;
			return linkControl.SupportSmallWithTextRibbonStyle();
		}
		protected virtual bool IsModifiableSmallButtonWithText(BarItemLinkControl linkControl) {
			if(linkControl == null || linkControl.CurrentRibbonStyle != RibbonItemStyles.SmallWithText)
				return false;
			return linkControl.SupportSmallWithoutTextRibbonStyle();
		}
		protected internal BarItemLinkControlBase GetChild(int index) {
			BarItemLinkInfo container = GetSnapshotItem(index) as BarItemLinkInfo;
			return container == null ? null : container.LinkControl;
		}
		protected internal double GetBestHeightForPageGroupPanel() {
			if(OriginBarButtonItemLinkControl == null || Ribbon == null)
				return 0;
			if(Ribbon != null && IsRibbonTablet(Ribbon.RibbonStyle)) {
				return OriginBarButtonItemLinkControl == null ? 0 : OriginBarButtonItemLinkControl.DesiredSize.Height;
			}
			double largeButtonHeight = OriginLargeBarButtonItemLinkControl == null ? 0 : OriginLargeBarButtonItemLinkControl.DesiredSize.Height;
			return Math.Max(OriginBarButtonItemLinkControl.DesiredSize.Height * 3 + Ribbon.RowIndent * 2, largeButtonHeight);
		}
		protected internal virtual void UpdateSuperTip() {
			if(PageGroup == null)
				return;
			if(PageGroup.SuperTip == null) {
				if(CaptionButton != null)
					CaptionButton.SetToolTip(null);
				if(CollapsedStateBorder != null)
					CollapsedStateBorder.SetToolTip(null);
			} else {
				if(CaptionButton != null)
					CaptionButton.SetToolTip(new SuperTipControl(PageGroup.SuperTip));
				if(CollapsedStateBorder != null)
					CollapsedStateBorder.SetToolTip(new SuperTipControl(PageGroup.SuperTip));
			}
		}
		protected virtual void UpdateDefaultActualBorders() {
			ActualPopupButtonBorderTemplate = PopupButtonBorderTemplate;
			ActualPopupButtonContentBorderTemplate = PopupButtonContentBorderTemplate;
			ActualPopupButtonGlyphBorderTemplate = PopupButtonGlyphBorderTemplate;
			ActualCaptionBorderTemplate = CaptionBorderTemplate;
		}
		protected virtual void UpdateActualBorders() {
			if(IsOrigin) {
				ActualBorderTemplate = BorderTemplate;
				UpdateDefaultActualBorders();
			}
			if(PageGroup == null || PageGroup.Page == null || PageGroup.Page.PageCategory == null)
				return;
			if(Owner == null) {
				ActualBorderTemplate = PageGroup.Page.PageCategory.IsDefault ? BorderTemplate : HighlightedBorderTemplate;
			}
			else {
				ActualBorderTemplate = PageGroup.Page.PageCategory.IsDefault ? BorderTemplateInPopup : HighlightedBorderTemplateInPopup;
			}
			if(PageGroup.Page.PageCategory.IsDefault) {
				UpdateDefaultActualBorders();
			}
			else {
				ActualPopupButtonBorderTemplate = HighlightedPopupButtonBorderTemplate;
				ActualPopupButtonContentBorderTemplate = HighlightedPopupButtonContentBorderTemplate;
				ActualPopupButtonGlyphBorderTemplate = HighlightedPopupButtonGlyphBorderTemplate;
				ActualCaptionBorderTemplate = HighlightedCaptionBorderTemplate;
			}
		}
		WeakReference ribbonWR = new WeakReference(null);
		RibbonControl RibbonWR {
			get { return (RibbonControl)ribbonWR.Target; }
			set { ribbonWR = new WeakReference(value); }
		}
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			RibbonWR = Ribbon ?? oldValue;
			SetBinding(IsMergedProperty, new Binding() { Path = new PropertyPath(RibbonControl.IsMergedProperty), Source = Ribbon });
			UpdateItemsCollectionProperties();			
		}
		internal double GetKeyTipRowOffset(RibbonKeyTipVerticalPlacement row, UIElement keyTipTarget) {
			Point offset = keyTipTarget.TransformToVisual(this).Transform(new Point(0, 0));
			double itemsPresenterOffset = keyTipTarget.TransformToVisual(ItemsPresenter).Transform(new Point(0, 0)).Y;
			Thickness margin = new Thickness();
			if(keyTipTarget is FrameworkElement)
				margin = ((FrameworkElement)keyTipTarget).Margin;
			switch(row) {
				case RibbonKeyTipVerticalPlacement.TopRow:
					return -itemsPresenterOffset;
				case RibbonKeyTipVerticalPlacement.CenterRow:
					return ItemsPresenter.ActualHeight / 2 - itemsPresenterOffset;
				case RibbonKeyTipVerticalPlacement.BottomRow:
					return ItemsPresenter.ActualHeight - itemsPresenterOffset - margin.Top - margin.Bottom;
				case RibbonKeyTipVerticalPlacement.CaptionGroupRow:
					return ActualHeight - offset.Y - margin.Top - margin.Bottom;
			}
			return 0;
		}
		protected override void OnItemClick(BarItemLinkControl linkControl) {
			base.OnItemClick(linkControl);
			RibbonControl ribbon = Ribbon ?? RibbonWR;
			if(ribbon != null) {
				if(linkControl is BarSplitButtonItemLinkControl && !((BarSplitButtonItemLinkControl)linkControl).IsArrowPressed && !((BarSplitButtonItemLinkControl)linkControl).ActualActAsDropDown) {
					ribbon.CollapseMinimizedRibbon();
					return;
				}
				if(linkControl.GetType() == typeof(BarButtonItemLinkControl) || linkControl.GetType() == typeof(BarCheckItemLinkControl)) {
					ribbon.CollapseMinimizedRibbon();
				}
			}
		}
		protected internal virtual void OnContainerItemClick(FrameworkElement container, BarItemLinkControl linkControl) {
			OnItemClick(linkControl);
		}
		protected virtual void OnIsPageGroupVisibleChanged(bool oldValue) {
			Visibility = IsPageGroupVisible ? Visibility.Visible : Visibility.Collapsed;
			if(SelectedPageControl != null)
				SelectedPageControl.ResetAllGroups();			
		}
		protected virtual void OnAllowCollapseChanged(bool oldValue) {
			if(IsCollapsed == false && AllowCollapse == true)
				IsCollapsed = true;
			if(SelectedPageControl != null)
				SelectedPageControl.ResetAllGroups();
		}
		private BaseEditSettings GetEditSettings(BarEditItemLinkControl control) {
			if(control == null)
				return null;
			if(control.EditItem == null)
				return null;
			return control.EditItem.EditSettings;
		}
		protected internal virtual void RecreateEditors() {
			for(int i = 0; i < GetSnapshotItemsCount(); i++) {
				BarItemLinkInfo li = Items[i] as BarItemLinkInfo;
				if(li != null && li.LinkControl is BarEditItemLinkControl) {
					BarEditItemLinkControl linkControl = li.LinkControl as BarEditItemLinkControl;
					ListBoxEditSettings listSettings = GetEditSettings(linkControl) as ListBoxEditSettings;
					ComboBoxEditSettings comboSettings = GetEditSettings(linkControl) as ComboBoxEditSettings;
					if(listSettings != null || comboSettings != null) {
						linkControl.RecreateEdit();
					}
				}
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			UpdateSeparatorVisibility();
			return base.MeasureOverride(constraint);
		}
		protected virtual void UpdateSeparatorVisibility() {
			if(IsOrigin)
				return;
			ItemsControl itemsControl = ItemsControlFromItemContainer(this);
			IsSeparatorVisible = itemsControl != null && itemsControl.Items.IndexOf(PageGroup) != itemsControl.Items.Count - 1;
			IsSeparatorVisible |= !IsTabletMode;
		}
		protected internal class PageGroupCollapseButtonInfo {
			public RibbonPageGroupControl PageGroupControl { get { return pageGroupControl; } }
			public BarSubItem Item {
				get {
					if(subItem == null)
						subItem = CreateItem();
					return subItem;
				}
			}
			public BarSubItemLink Link {
				get {
					if(link == null)
						link = CreateLink();
					return link;
				}
			}
			public PageGroupCollapseButtonInfo(RibbonPageGroupControl pageGroupControl) {
				this.pageGroupControl = pageGroupControl;
			}
			protected virtual BarSubItem CreateItem() {
				var item = new BarSubItem() { IsPrivate = true };
				item.GetItemData += OnSubItemGetItemData;
				item.CloseUp += OnSubItemCloseUp;
				item.SetResourceReference(BarSubItem.StyleProperty, new RibbonPageGroupThemeKeyExtension() {
					ResourceKey = RibbonPageGroupThemeKeys.CollapsedItemStyle, IsThemeIndependent = true
				});
				BindingOperations.SetBinding(item, BarItem.ContentProperty, CreateBinding("PageGroup.Caption"));
				RibbonControl.SetAllowAddingToToolbar(item, false);
				return item;
			}
			protected virtual BarSubItemLink CreateLink() {
				return Item.CreateLink(true) as BarSubItemLink;
			}
			protected virtual void UpdateItemLinks() {
				if(PageGroupControl.CollapsedItemLinks != null) {
					Item.ItemLinks.BeginUpdate();
					foreach(var link in PageGroupControl.CollapsedItemLinks) {
						Item.ItemLinks.Add(link);
					}
					Item.ItemLinks.EndUpdate();
				}
			}
			protected virtual void OnSubItemGetItemData(object sender, EventArgs e) {
				UpdateItemLinks();
			}
			protected virtual void OnSubItemCloseUp(object sender, EventArgs e) {
				Item.ItemLinks.Clear();
			}
			Binding CreateBinding(string path) {
				Binding binding = new Binding(path);
				binding.Source = PageGroupControl;
				binding.Mode = BindingMode.OneWay;
				return binding;
			}
			readonly RibbonPageGroupControl pageGroupControl;
			BarSubItemLink link;
			BarSubItem subItem;
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public class ControlClipper : Behavior<FrameworkElement> {
		#region staic
		public static readonly DependencyProperty TargetProperty;
		static ControlClipper() {
			TargetProperty = DependencyProperty.Register("Target", typeof(UIElement), typeof(ControlClipper), new PropertyMetadata(null, OnTargetPropertyChanged));
		}
		static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ControlClipper)d).OnTargetChanged((UIElement)e.OldValue);
		}
		#endregion
		public UIElement Target {
			get { return (UIElement)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.SizeChanged += OnSizeChanged;
			UpdateClipping();
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.SizeChanged -= OnSizeChanged;
			AssociatedObject.ClearValue(UIElement.ClipProperty);
		}
		void OnTargetChanged(UIElement uIElement) {
			UpdateClipping();
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateClipping();
		}
		void UpdateClipping() {
			if(AssociatedObject == null)
				return;
			if(Target == null) {
				AssociatedObject.ClearValue(UIElement.ClipProperty);
				return;
			}
			Point leftTopOffset = Target.TranslatePoint(new Point(), AssociatedObject);
			double right = Math.Min(leftTopOffset.X + Target.RenderSize.Width, AssociatedObject.RenderSize.Width);
			double bottom = Math.Min(leftTopOffset.Y + Target.RenderSize.Height, AssociatedObject.RenderSize.Height);
			Rect clipRect = new Rect(leftTopOffset, new Point(right, bottom));
			Rect renderSizeRect = new Rect(AssociatedObject.RenderSize);
			AssociatedObject.Clip = Geometry.Combine(new RectangleGeometry(renderSizeRect), new RectangleGeometry(clipRect), GeometryCombineMode.Exclude, null);
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public abstract class PageGroupControlCollaspeItemsStrategyBase {
		public static PageGroupControlCollaspeItemsStrategyBase GetActualStrategy(RibbonStyle ribbonStyle) {
			if (ribbonStyle == RibbonStyle.OfficeSlim)
				return OfficeSlimStrategy;
			if (ribbonStyle == RibbonStyle.TabletOffice)
				return TabletOfficeStrategy;
			return ClassicStrategy;
		}
		public static PageGroupControlCollaspeItemsStrategyBase ClassicStrategy {
			get { return classicStrategy ?? (classicStrategy = new ClassicOfficePageGroupControlCollaspeItemsStrategy()); }
		}
		public static PageGroupControlCollaspeItemsStrategyBase OfficeSlimStrategy {
			get { return officeSlimStrategy ?? (officeSlimStrategy = new OfficeSlimPageGroupControlCollaspeItemsStrategy()); }
		}
		public static PageGroupControlCollaspeItemsStrategyBase TabletOfficeStrategy {
			get { return tabletOfficeStrategy ?? (tabletOfficeStrategy = new TabletOfficePageGroupControlCollaspeItemsStrategy()); }
		}
		static PageGroupControlCollaspeItemsStrategyBase classicStrategy, officeSlimStrategy, tabletOfficeStrategy;
		public BarItemLinkInfoCollection GetActualItemSource(RibbonPageGroupControl pageGroupControl) {
			if (pageGroupControl.PageGroup == null)
				return null;
			return GetActualItemSourceOverride(pageGroupControl);
		}
		public IEnumerable<BarItemLinkBase> GetCollapsedLinks(RibbonPageGroupControl pageGroupControl) {
			return GetCollapsedLinksOverride(pageGroupControl);
		}
		protected abstract IEnumerable<BarItemLinkBase> GetCollapsedLinksOverride(RibbonPageGroupControl pageGroupControl);
		protected abstract BarItemLinkInfoCollection GetActualItemSourceOverride(RibbonPageGroupControl pageGroupControl);
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public class ClassicOfficePageGroupControlCollaspeItemsStrategy : PageGroupControlCollaspeItemsStrategyBase {
		protected override BarItemLinkInfoCollection GetActualItemSourceOverride(RibbonPageGroupControl pageGroupControl) {
			var pageGroup = pageGroupControl.PageGroup;
			return new BarItemLinkInfoCollection(((ILinksHolder)pageGroup).ActualLinks);
		}
		protected override IEnumerable<BarItemLinkBase> GetCollapsedLinksOverride(RibbonPageGroupControl pageGroupControl) {
			throw new NotSupportedException();
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public class TabletOfficePageGroupControlCollaspeItemsStrategy : PageGroupControlCollaspeItemsStrategyBase {
		protected virtual IEnumerable<BarItemLinkBase> GetActualItemLinks(IEnumerable<BarItemLinkBase> source) {
			return source.SelectMany(link => {
				if (link is BarItemLink && ((BarItemLink)link).Item is BarButtonGroup)
					return ((ILinksHolder)((BarItemLink)link).Item).ActualLinks.ToArray();
				return new[] { link };
			});
		}
		protected override BarItemLinkInfoCollection GetActualItemSourceOverride(RibbonPageGroupControl pageGroupControl) {
			var pageGroup = pageGroupControl.PageGroup;
			var actualLinks = ((ILinksHolder)pageGroup).ActualLinks.ToList();
			actualLinks = GetActualItemLinks(actualLinks).ToList();
			if (pageGroupControl.CollapseButtonInfo != null)
				actualLinks.Add(pageGroupControl.CollapseButtonInfo.Link);
			return new BarItemLinkInfoCollection(actualLinks);
		}
		protected override IEnumerable<BarItemLinkBase> GetCollapsedLinksOverride(RibbonPageGroupControl pageGroupControl) {
			var infos = pageGroupControl.Items.OfType<BarItemLinkInfo>();
			return infos.Where(link => RibbonControlLayoutHelper.GetIsItemCollapsed(link)).Select(link => GetCollapsedLink(link));
		}
		protected virtual BarItemLinkBase GetCollapsedLink(BarItemLinkInfo link) {
			return ((BarItemLinkBase)((ICloneable)link.Link).Clone());
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	public class OfficeSlimPageGroupControlCollaspeItemsStrategy : TabletOfficePageGroupControlCollaspeItemsStrategy {
		protected override IEnumerable<BarItemLinkBase> GetActualItemLinks(IEnumerable<BarItemLinkBase> source) {
			return source.ToList();
		}
		protected override BarItemLinkBase GetCollapsedLink(BarItemLinkInfo link) {
			if (link.Item is BarButtonGroup) {
				return GetButtonGroupLink((BarButtonGroup)link.Item);
			}
			return base.GetCollapsedLink(link);
		}
		BarItemLinkBase GetButtonGroupLink(BarButtonGroup buttonGroup) {
			var headerItem = new BarItemMenuHeader() { IsPrivate = true, Content = buttonGroup.Content };
			var links = ((ILinksHolder)buttonGroup).ActualLinks;
			headerItem.ItemsOrientation = HeaderOrientation.Horizontal;
			foreach (var barItemLink in links) {
				if (IsEditItem(barItemLink))
					headerItem.ItemsOrientation = HeaderOrientation.Vertical;
				headerItem.Items.Add(barItemLink);
			}
			return headerItem.CreateLink();
		}
		bool IsEditItem(BarItemLinkBase barItemLink) {
			var link = barItemLink as BarItemLink;
			return link.Return(l => l.Item is BarEditItem, () => false);
		}
	}
}
