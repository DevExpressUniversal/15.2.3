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
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageCategoryControl : ItemsControl {
		#region static
		public static readonly DependencyProperty AllowCollapseProperty;
		public static readonly DependencyProperty PageCategoryProperty;
		public static readonly DependencyProperty RightCaptionPaddingProperty;
		private static readonly DependencyPropertyKey RightCaptionPaddingPropertyKey;
		public static readonly DependencyProperty IsRibbonMinimizedProperty;
		public static readonly DependencyProperty AeroTemplateProperty;
		public static readonly DependencyProperty IsAeroModeProperty;
		public static readonly DependencyProperty ShowHeaderProperty;
		public static readonly DependencyProperty CaptionWidthProperty;
		public static readonly DependencyProperty IsCollapsedProperty;
		static readonly DependencyPropertyKey IsCollapsedPropertyKey;
		public static readonly DependencyProperty CollapsedPagesProperty;
		static readonly DependencyPropertyKey CollapsedPagesPropertyKey;
		protected static readonly DependencyPropertyKey SizeInfoPropertyKey;
		public static readonly DependencyProperty SizeInfoProperty;
		static RibbonPageCategoryControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonPageCategoryControl), new FrameworkPropertyMetadata(typeof(RibbonPageCategoryControl)));
			AeroTemplateProperty = DependencyPropertyManager.Register("AeroTemplate", typeof(ControlTemplate), typeof(RibbonPageCategoryControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAeroTemplatePropertyChanged)));
			IsAeroModeProperty = DependencyPropertyManager.Register("IsAeroMode", typeof(bool), typeof(RibbonPageCategoryControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAeroModePropertyChanged)));
			AllowCollapseProperty = DependencyPropertyManager.Register("AllowCollapse", typeof(bool), typeof(RibbonPageCategoryControl), new PropertyMetadata(false));
			RightCaptionPaddingPropertyKey = DependencyPropertyManager.RegisterReadOnly("RightCaptionPadding", typeof(double), typeof(RibbonPageCategoryControl), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnRightCaptionPaddingPropertyChanged)));
			RightCaptionPaddingProperty = RightCaptionPaddingPropertyKey.DependencyProperty;
			PageCategoryProperty = DependencyPropertyManager.Register("PageCategory", typeof(RibbonPageCategoryBase), typeof(RibbonPageCategoryControl),
				new PropertyMetadata(null, OnPageCategoryPropertyChanged));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(RibbonPageCategoryControl), typeof(RibbonPageCategoryControlAutomationPeer), owner => new RibbonPageCategoryControlAutomationPeer((RibbonPageCategoryControl)owner));
			IsRibbonMinimizedProperty = DependencyPropertyManager.Register("IsRibbonMinimized", typeof(bool), typeof(RibbonPageCategoryControl), new FrameworkPropertyMetadata(false));
			ShowHeaderProperty = DependencyPropertyManager.Register("ShowHeader", typeof(bool), typeof(RibbonPageCategoryControl), new FrameworkPropertyMetadata(true, (d, e) => ((RibbonPageCategoryControl)d).OnShowHeaderChanged((bool)e.OldValue)));
			CaptionWidthProperty = DependencyPropertyManager.Register("CaptionWidth", typeof(double), typeof(RibbonPageCategoryControl), new FrameworkPropertyMetadata(0d, (d, e) => ((RibbonPageCategoryControl)d).OnCaptionWidthChanged()));
			SizeInfoPropertyKey = DependencyPropertyManager.RegisterReadOnly("SizeInfo", typeof(RibbonPageCategoryHeaderInfo), typeof(RibbonPageCategoryControl), new FrameworkPropertyMetadata(null, (d, e) => ((RibbonPageCategoryControl)d).OnSizeInfoChanged((RibbonPageCategoryHeaderInfo)e.OldValue)));
			SizeInfoProperty = SizeInfoPropertyKey.DependencyProperty;
			IsCollapsedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCollapsed", typeof(bool), typeof(RibbonPageCategoryControl), new PropertyMetadata(false, OnIsCollapsedPropertyChanged));
			IsCollapsedProperty = IsCollapsedPropertyKey.DependencyProperty;
			CollapsedPagesPropertyKey = DependencyPropertyManager.RegisterReadOnly("CollapsedPages", typeof(IEnumerable<RibbonPage>), typeof(RibbonPageCategoryControl), new PropertyMetadata(null, OnCollapsedPagesPropertyChanged));
			CollapsedPagesProperty = CollapsedPagesPropertyKey.DependencyProperty;
		}
		static void OnCollapsedPagesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryControl)d).OnCollapsedPagesChanged((IEnumerable<RibbonPage>)e.OldValue);
		}
		static void OnIsCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryControl)d).OnIsCollapsedChanged((bool)e.OldValue);
		}
		protected static void OnRightCaptionPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryControl)d).OnRightCaptionPaddingChanged(e);
		}
		protected static void OnPageCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryControl)d).OnPageCategoryChanged((RibbonPageCategoryBase)e.OldValue);
		}
		protected static void OnAeroTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryControl)d).OnAeroTemplateChanged((ControlTemplate)e.OldValue);
		}
		protected static void OnIsAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageCategoryControl)d).OnIsAeroModeChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public bool AllowCollapse {
			get { return (bool)GetValue(AllowCollapseProperty); }
			set { SetValue(AllowCollapseProperty, value); }
		}
		public RibbonPageCategoryBase PageCategory {
			get { return (RibbonPageCategoryBase)GetValue(PageCategoryProperty); }
			set { SetValue(PageCategoryProperty, value); }
		}
		public double CaptionWidth {
			get { return (double)GetValue(CaptionWidthProperty); }
			set { SetValue(CaptionWidthProperty, value); }
		}
		public IEnumerable<RibbonPage> CollapsedPages {
			get { return (IEnumerable<RibbonPage>)GetValue(CollapsedPagesProperty); }
			private set { SetValue(CollapsedPagesPropertyKey, value); }
		}
		public double RightCaptionPadding {
			get { return (double)GetValue(RightCaptionPaddingProperty); }
			protected internal set { this.SetValue(RightCaptionPaddingPropertyKey, value); }
		}
		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			private set { SetValue(IsCollapsedPropertyKey, value); }
		}
		public bool IsRibbonMinimized {
			get { return (bool)GetValue(IsRibbonMinimizedProperty); }
			set { SetValue(IsRibbonMinimizedProperty, value); }
		}
		public ControlTemplate AeroTemplate {
			get { return (ControlTemplate)GetValue(AeroTemplateProperty); }
			set { SetValue(AeroTemplateProperty, value); }
		}
		public bool IsAeroMode {
			get { return (bool)GetValue(IsAeroModeProperty); }
			set { SetValue(IsAeroModeProperty, value); }
		}
		public bool ShowHeader {
			get { return (bool)GetValue(ShowHeaderProperty); }
			set { SetValue(ShowHeaderProperty, value); }
		}
		public RibbonPageCategoryHeaderInfo SizeInfo {
			get { return (RibbonPageCategoryHeaderInfo)GetValue(SizeInfoProperty); }
			protected internal set { this.SetValue(SizeInfoPropertyKey, value); }
		}
		#endregion
		public RibbonControl Ribbon {
			get { return ribbonCore; }
			internal set {
				if(ribbonCore == value)
					return;
				RibbonControl oldValue = ribbonCore;
				ribbonCore = value;
				OnRibbonChanged(oldValue);
			}
		}
		public ToggleButton CollapseButton { get; set; }
		public RibbonPageCategoryHeaderControl PageCategoryHeaderControl {
			get { return pageCategoryHeaderControl; }
			set {
				if (pageCategoryHeaderControl == value)
					return;
				var oldValue = pageCategoryHeaderControl;
				pageCategoryHeaderControl = value;
				OnPageCategoryHeaderControlChanged(oldValue);
			}
		}
		protected RibbonCheckedBorderControl BackgroundHeader {
			get { return backgroundHeader; }
			private set {
				if(backgroundHeader != value) {
					var oldValue = backgroundHeader;
					backgroundHeader = value;
					OnBackgroundHeaderChanged(oldValue);
				}
			}
		}
		#if DEBUGTEST
		public RibbonCheckedBorderControl BackgroundHeaderForTests { get { return BackgroundHeader; } }
		#endif
		protected internal ItemsPresenter ItemsPresenter { get; set; }
		double MaxPageCaptionTextIndent { get { return Ribbon == null ? 0 : Ribbon.MaxPageCaptionTextIndent; } }
		RibbonPageHeaderControl activePage;
		public RibbonPageHeaderControl ActivePage {
			get {
				if (activePage == null || !activePage.IsVisible)
				   return (RibbonPageHeaderControl)Items.OfType<object>().Select(item => ItemContainerGenerator.ContainerFromItem(item)).FirstOrDefault();
				return activePage;
			}
			private set { activePage = value; }
		}
		public RibbonPageCategoryControl() {
			LayoutUpdated += OnLayoutUpdated;
			CollapsedPages = Enumerable.Empty<RibbonPage>();
			IsVisibleChanged += OnIsVisibleChanged;
		}
		void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateSizeInfo(0d);
		}
		public double GetHeaderWidth() {
			if (PageCategoryHeaderControl == null || !PageCategoryHeaderControl.IsVisible)
				return 0d;
			return ActualWidth;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PageCategoryHeaderControl = GetTemplateChild("PART_CategoryHeaderControl") as RibbonPageCategoryHeaderControl;
			BackgroundHeader = GetTemplateChild("PART_HighlightedHeadersBackground") as RibbonCheckedBorderControl;
			ItemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
			CollapseButton = GetTemplateChild("PART_CollapseButton") as ToggleButton;
		}
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			if (Ribbon != null && Ribbon.IsInRibbonWindow && Ribbon.WindowHelper.RibbonWindow != null) {
				var container = Ribbon.WindowHelper.GetControlBoxContainer();
				if (container == null) {
					var controlBoxRect = Ribbon.WindowHelper.RibbonWindow.GetControlBoxRect();
					var location = Ribbon.TranslatePoint(new Point(Ribbon.ActualWidth - controlBoxRect.Width, 0d), this);
					RightCaptionPadding = Math.Max(ActualWidth - location.X, 0d);
				} else
					RightCaptionPadding = Math.Max(ActualWidth - container.TranslatePoint(new Point(), this).X, 0d);
			}
		}
		protected virtual void OnBackgroundHeaderChanged(RibbonCheckedBorderControl oldValue) { }
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			BindingOperations.ClearBinding(this, IsAeroModeProperty);
			BindingOperations.ClearBinding(this, IsRibbonMinimizedProperty);
			if(Ribbon != null) {
				SetBinding(IsRibbonMinimizedProperty, new Binding("IsMinimized") { Source = Ribbon });
				SetBinding(IsAeroModeProperty, new Binding("IsAeroMode") { Source = Ribbon });
			}
		}
		protected virtual void OnRightCaptionPaddingChanged(DependencyPropertyChangedEventArgs e) {
			UpdateHeaderControlClipping();
		}
		protected virtual void UpdateHeaderControlClipping() {
			if(PageCategoryHeaderControl != null) {
				if(RightCaptionPadding > 0d)
					PageCategoryHeaderControl.Clip = new RectangleGeometry(new Rect(new Size(Math.Max(ActualWidth - RightCaptionPadding, 0d), ActualHeight)));
				else
					PageCategoryHeaderControl.ClearValue(FrameworkElement.ClipProperty);
			}
		}
		protected virtual void OnPageCategoryChanged(RibbonPageCategoryBase oldValue) {
			if(oldValue != null)
				oldValue.CategoryControls.Remove(this);
			if(PageCategory != null)
				PageCategory.CategoryControls.Add(this);
			UpdateItemsSource();
		}
		protected virtual void OnPageCategoryHeaderControlChanged(RibbonPageCategoryHeaderControl oldValue) { }
		protected virtual void OnAeroTemplateChanged(ControlTemplate oldValue) {
			UpdateTemplate();
		}
		protected virtual void OnIsAeroModeChanged(bool oldValue) {
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if(IsAeroMode)
				Template = AeroTemplate;
		}
		protected virtual void OnShowHeaderChanged(bool oldValue) { }
		protected virtual void UpdateItemsSource() {
			ItemsSource = PageCategory.With(cat => cat.ActualPagesCore);
		}
		protected virtual void OnIsCollapsedChanged(bool oldValue) { }
		protected virtual void OnCollapsedPagesChanged(IEnumerable<RibbonPage> oldValue) { }
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new RibbonPageHeaderControl();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is RibbonPageHeaderControl;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			RibbonPageHeaderControl control = (RibbonPageHeaderControl)element;
			RibbonPage page = (RibbonPage)item;
			page.IsSelectedChanged += OnPageIsSelectedChanged;
			control.Page = page;
			if (page.IsSelected)
				ActivePage = control;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			RibbonPageHeaderControl control = element as RibbonPageHeaderControl;
			if (control != null) {
				control.Page.IsSelectedChanged -= OnPageIsSelectedChanged;
				control.Page = null;
			}
			if (ActivePage == control)
				ActivePage = null;
			base.ClearContainerForItemOverride(element, item);
		}
		internal RibbonPageHeaderControl GetContainerFromIndex(int index) {
			return ItemContainerGenerator.ContainerFromIndex(index) as RibbonPageHeaderControl;
		}
		protected override Size MeasureOverride(Size constraint) {
			CollapseButton.Visibility = AllowCollapse.ToVisibility();
			Size result = base.MeasureOverride(SizeHelper.Infinite);
			if (PageCategoryHeaderControl != null) {
				CaptionWidth = PageCategoryHeaderControl.BestWidth;
			}
			UpdateSizeInfo(result.Width - CollapseButton.DesiredSize.Width);
			IsCollapsed = AllowCollapse && SizeInfo.MaxWidth > constraint.Width;
			CollapseButton.ClearValue(UIElement.VisibilityProperty);
			return base.MeasureOverride(constraint);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			CollapsedPages = GetCollapsedPages();
			return base.ArrangeOverride(arrangeBounds);
		}
		IEnumerable<RibbonPage> GetCollapsedPages() {
			if (!IsCollapsed || ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
				return Enumerable.Empty<RibbonPage>();
			else {
				return Items.OfType<RibbonPage>().Where(page => RibbonControlLayoutHelper.GetIsItemCollapsed(ItemContainerGenerator.ContainerFromItem(page)));
			}
		}
		protected virtual void UpdateSizeInfo(double diff) {
			if (Visibility != Visibility.Visible) {
				SizeInfo = new RibbonPageCategoryHeaderInfo(this);
				return;
			}
			var sizeInfo = AllowCollapse ? GetAllowCollapseSizeInfo() : GetSizeInfo();
			sizeInfo.MinWidth = Math.Max(CaptionWidth, sizeInfo.MinWidth);
			sizeInfo.MaxWidth = Math.Max(sizeInfo.MaxWidth, sizeInfo.MinWidth);
			sizeInfo.DesiredWidth = Math.Max(sizeInfo.DesiredWidth, sizeInfo.MinWidth);
			diff = Math.Max(0d, diff - sizeInfo.MaxWidth);
			sizeInfo.MaxWidth += diff;
			sizeInfo.DesiredWidth += diff;
			sizeInfo.MinWidth += diff;
			SizeInfo = sizeInfo;
		}
		protected virtual RibbonPageCategoryHeaderInfo GetAllowCollapseSizeInfo() {
			IEnumerable<RibbonPageHeaderControl> items = GetPageHeaders();
			RibbonPageCategoryHeaderInfo headerInfo = new RibbonPageCategoryHeaderInfo(this);
			headerInfo.MinWidth = ActivePage == null ? 0d : ActivePage.BestDesiredSize.Width;
			headerInfo.MaxWidth = items.Sum(header => header.BestDesiredSize.Width);
			headerInfo.DesiredWidth = headerInfo.MaxWidth;
			if (items.Count() > 1)
				headerInfo.MinWidth += CollapseButton.DesiredSize.Width;
			return headerInfo;
		}
		protected virtual RibbonPageCategoryHeaderInfo GetSizeInfo() {
			RibbonPageCategoryHeaderInfo headerInfo = new RibbonPageCategoryHeaderInfo(this);
			IEnumerable<RibbonPageHeaderControl> items = GetPageHeaders();
			foreach (RibbonPageHeaderControl headerControl in items) {
				headerInfo.MinWidth += headerControl.MinDesiredWidth;
				headerInfo.MaxWidth += headerControl.BestDesiredSize.Width;
				headerInfo.DesiredWidth += headerControl.BestDesiredSize.Width - MaxPageCaptionTextIndent;
			}
			return headerInfo;
		}
		IEnumerable<RibbonPageHeaderControl> GetPageHeaders() {
			if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
				return Enumerable.Empty<RibbonPageHeaderControl>();
			return Items.OfType<RibbonPage>().Where(page => page.ActualIsVisible).Select(item => (RibbonPageHeaderControl)ItemContainerGenerator.ContainerFromItem(item));
		}
		protected virtual void OnSizeInfoChanged(RibbonPageCategoryHeaderInfo oldValue) { }
		protected virtual void OnCaptionWidthChanged() { }
		protected virtual void OnPageIsSelectedChanged(object sender, EventArgs e) {
			var page = (RibbonPage)sender;
			if (page.IsSelected)
				ActivePage = (RibbonPageHeaderControl)ItemContainerGenerator.ContainerFromItem(page);
			InvalidateMeasure();
		}
		RibbonControl ribbonCore = null;
		RibbonCheckedBorderControl backgroundHeader;
		RibbonPageCategoryHeaderControl pageCategoryHeaderControl;
	}
}
