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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageHeaderControl : SLBarItemLinkControlBase {
		#region static
		const int doubleClickWaitDelay = 200;
		public static readonly DependencyProperty PageProperty;
		public static readonly DependencyProperty PageHeaderControlProperty;
		public static readonly DependencyProperty SeparatorOpacityProperty;
		public static readonly DependencyProperty IsPageSelectedProperty;
		public static readonly DependencyProperty IsMinimizedRibbonCollapsedProperty;
		public static readonly DependencyProperty IsRibbonMinimizedProperty;
		public static readonly DependencyProperty ActualPageCaptionMinWidthProperty;
		protected static readonly DependencyPropertyKey ActualPageCaptionMinWidthPropertyKey;
		public static readonly DependencyProperty PageCaptionMinWidthProperty;
		public static readonly DependencyProperty CommonPageCaptionMinWidthProperty;
		public static readonly DependencyProperty IsRibbonBackStageViewOpenedProperty;
		public static readonly DependencyProperty AeroTemplateProperty;
		public static readonly DependencyProperty IsAeroModeProperty;
		public static readonly DependencyProperty BestDesiredSizeProperty;
		public static readonly DependencyProperty MinDesiredWidthProperty;
		public static readonly DependencyProperty IndexProperty;
		static RibbonPageHeaderControl() {
			Type ownerType = typeof(RibbonPageHeaderControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(RibbonPageHeaderControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));
		AeroTemplateProperty = DependencyPropertyManager.Register("AeroTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAeroTemplatePropertyChanged)));
			IsAeroModeProperty = DependencyPropertyManager.Register("IsAeroMode", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAeroModePropertyChanged)));
			PageProperty = DependencyPropertyManager.Register("Page", typeof(RibbonPage), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnPagePropertyChanged)));
			PageHeaderControlProperty = DependencyPropertyManager.RegisterAttached("PageHeaderControl", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnPageHeaderControlPropertyChanged)));
			SeparatorOpacityProperty = DependencyPropertyManager.Register("SeparatorOpacity", typeof(double), ownerType, new FrameworkPropertyMetadata(0d));
			IsPageSelectedProperty = DependencyPropertyManager.Register("IsPageSelected", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsPageSelectedPropertyChanged)));
			IsMinimizedRibbonCollapsedProperty = DependencyPropertyManager.Register("IsMinimizedRibbonCollapsed", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsMinimizedRibbonCollapsedChanged)));
			IsRibbonMinimizedProperty = DependencyPropertyManager.Register("IsRibbonMinimized", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsRibbonMinimizedChanged)));
			ActualPageCaptionMinWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualPageCaptionMinWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ActualPageCaptionMinWidthProperty = ActualPageCaptionMinWidthPropertyKey.DependencyProperty;
			PageCaptionMinWidthProperty = DependencyPropertyManager.Register("PageCaptionMinWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnPageCaptionMinWidthPropertyChanged)));
			CommonPageCaptionMinWidthProperty = DependencyPropertyManager.Register("CommonPageCaptionMinWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnCommonPageCaptionMinWidthPropertyChanged)));
			IsRibbonBackStageViewOpenedProperty = DependencyPropertyManager.Register("IsRibbonBackStageViewOpened", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsRibbonBackStageViewOpenedChanged)));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(ownerType, typeof(RibbonPageHeaderControlAutomationPeer), owner => new RibbonPageHeaderControlAutomationPeer((RibbonPageHeaderControl)owner));
			BestDesiredSizeProperty = DependencyPropertyManager.Register("BestDesiredSize", typeof(Size), ownerType, new FrameworkPropertyMetadata(default(Size), (d, e) => ((RibbonPageHeaderControl)d).OnBestDesiredSizeChanged((Size)e.OldValue)));
			MinDesiredWidthProperty = DependencyPropertyManager.Register("MinDesiredWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, (d, e) => ((RibbonPageHeaderControl)d).OnMinDesiredWidthChanged()));
		}
		protected static void OnPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).OnPageChanged(e.OldValue as RibbonPage);
		}
		protected static void OnPageHeaderControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RibbonPageHeaderContentPresenter ph = d as RibbonPageHeaderContentPresenter;
			if(ph != null) ph.OnPageHeaderChanged(e);
		}
		protected static void OnIsPageSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).OnIsPageSelectedChanged((bool)e.OldValue);
		}
		protected static void OnIsMinimizedRibbonCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).UpdateActualIsMinimizedRibbonCollapsed();
			((RibbonPageHeaderControl)d).UpdateCaptionControlProperties();
		}
		protected static void OnIsRibbonMinimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).UpdateCaptionControlProperties();
		}
		protected static void OnIsRibbonBackStageViewOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).UpdateCaptionControlProperties();
		}
		protected static void OnPageCaptionMinWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).UpdateActualCaptionMinWidth();
		}
		protected static void OnCommonPageCaptionMinWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).UpdateActualCaptionMinWidth();
		}
		public static RibbonPageHeaderControl GetPageHeaderControl(DependencyObject d) { return (RibbonPageHeaderControl)d.GetValue(PageHeaderControlProperty); }
		public static void SetPageHeaderControl(DependencyObject d, RibbonPageHeaderControl value) { d.SetValue(PageHeaderControlProperty, value); }
		protected static void OnAeroTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).OnAeroTemplateChanged((ControlTemplate)e.OldValue);
		}
		protected static void OnIsAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPageHeaderControl)d).OnIsAeroModeChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public RibbonPage Page {
			get { return (RibbonPage)GetValue(PageProperty); }
			set { SetValue(PageProperty, value); }
		}
		public double SeparatorOpacity {
			get { return (double)GetValue(SeparatorOpacityProperty); }
			set { SetValue(SeparatorOpacityProperty, value); }
		}
		public bool IsPageSelected {
			get { return (bool)GetValue(IsPageSelectedProperty); }
			set { SetValue(IsPageSelectedProperty, value); }
		}
		public bool IsMinimizedRibbonCollapsed {
			get { return (bool)GetValue(IsMinimizedRibbonCollapsedProperty); }
			set { SetValue(IsMinimizedRibbonCollapsedProperty, value); }
		}
		public bool IsRibbonMinimized {
			get { return (bool)GetValue(IsRibbonMinimizedProperty); }
			set { SetValue(IsRibbonMinimizedProperty, value); }
		}
		public double ActualPageCaptionMinWidth {
			get { return (double)GetValue(ActualPageCaptionMinWidthProperty); }
			protected set { this.SetValue(ActualPageCaptionMinWidthPropertyKey, value); }
		}
		public double PageCaptionMinWidth {
			get { return (double)GetValue(PageCaptionMinWidthProperty); }
			set { SetValue(PageCaptionMinWidthProperty, value); }
		}
		public double CommonPageCaptionMinWidth {
			get { return (double)GetValue(CommonPageCaptionMinWidthProperty); }
			set { SetValue(CommonPageCaptionMinWidthProperty, value); }
		}
		public bool IsRibbonBackStageViewOpened {
			get { return (bool)GetValue(IsRibbonBackStageViewOpenedProperty); }
			set { SetValue(IsRibbonBackStageViewOpenedProperty, value); }
		}
		public ControlTemplate AeroTemplate {
			get { return (ControlTemplate)GetValue(AeroTemplateProperty); }
			set { SetValue(AeroTemplateProperty, value); }
		}
		public bool IsAeroMode {
			get { return (bool)GetValue(IsAeroModeProperty); }
			set { SetValue(IsAeroModeProperty, value); }
		}
		public Size BestDesiredSize {
			get { return (Size)GetValue(BestDesiredSizeProperty); }
			set { SetValue(BestDesiredSizeProperty, value); }
		}
		public double MinDesiredWidth {
			get { return (double)GetValue(MinDesiredWidthProperty); }
			set { SetValue(MinDesiredWidthProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		#endregion
		#region props
		public RibbonControl Ribbon { get { return PageCategoryControl.With(category => category.Ribbon); } }
		public RibbonPageCategoryControl PageCategoryControl {
			get { return (RibbonPageCategoryControl)ItemsControl.ItemsControlFromItemContainer(this); }
		}
		protected Control SeparatorControl { get; private set; }
		internal protected RibbonPageCaptionControl CaptionControl { get; private set; }
		protected internal RibbonPageHeaderContentPresenter ContentPresenter { get; set; }
		public double BestDesiredWidth {
			get {
				if (!IsVisible)
					return 0d;
				return DesiredSize.Width - CaptionControl.DesiredSize.Width + CaptionControl.BestDesiredWidth;
			}
		}
		#endregion
		public RibbonPageHeaderControl() {
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		DispatcherTimer waitingDoubleClickTimer = null;
		bool ActualIsMinimizedRibbonCollapsed { get; set; }
		void StartDoubleClickTimer() {
			if(waitingDoubleClickTimer == null) {
				waitingDoubleClickTimer = new DispatcherTimer();
				waitingDoubleClickTimer.Interval = TimeSpan.FromMilliseconds(doubleClickWaitDelay);
				waitingDoubleClickTimer.Tick += new EventHandler(OnDoubleClickTimerTick);
			}
			waitingDoubleClickTimer.Start();
		}
		void OnDoubleClickTimerTick(object sender, EventArgs e) {
			StopDoubleClickTimer();
			OnMouseLeftButtonSingleClick();
			UpdateActualIsMinimizedRibbonCollapsed();
		}
		bool AllowDoubleClickTimer() {
			return false;
		}
		void StopDoubleClickTimer() {
			if(waitingDoubleClickTimer != null)
				waitingDoubleClickTimer.Stop();
		}
		bool IsWaitingDoubleClick() {
			return waitingDoubleClickTimer != null && waitingDoubleClickTimer.IsEnabled;
		}
		void OnLoaded(object sender, RoutedEventArgs e) { }
		void OnUnloaded(object sender, RoutedEventArgs e) { }
		protected virtual void UpdateActualCaptionMinWidth() {
			ActualPageCaptionMinWidth = PageCaptionMinWidth == 0 ? CommonPageCaptionMinWidth : PageCaptionMinWidth;
		}
		protected virtual void OnPageChanged(RibbonPage oldValue) {
			if(oldValue != null)
				oldValue.PageHeaderControls.Remove(this);
			if(Page != null) {
				Page.PageHeaderControls.Add(this);
				SetBinding(IndexProperty, new Binding() { Path = new PropertyPath(RibbonPage.IndexProperty), Source = Page });
			} else
				ClearValue(IndexProperty);			
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			if(Ribbon != null) Ribbon.popupIsAlwaysOpen = true;
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			if(Ribbon != null) Ribbon.popupIsAlwaysOpen = false;
		}
		protected virtual void OnMouseLeftButtonDoubleClick() {
			Ribbon.SetCurrentValue(RibbonControl.IsMinimizedProperty, !Ribbon.IsMinimized);
		}
		protected internal virtual void OnMouseLeftButtonSingleClick() {
			if(Ribbon == null)
				return;
			if(Ribbon.IsBackStageViewOpen) {
				Ribbon.CloseApplicationMenu();
				if(Ribbon.IsMinimized)
					return;
			}
			if(IsRibbonMinimized && Ribbon.SelectedPage == Page && !ActualIsMinimizedRibbonCollapsed) {
				Ribbon.CollapseMinimizedRibbon();
				return;
			}
			if(!Page.IsSelected) {
				Page.SetCurrentValue(RibbonPage.IsSelectedProperty, true);
			}
			if(Ribbon.IsMinimized)
				Ribbon.ExpandMinimizedRibbon();
		}
		protected override Size MeasureOverride(Size constraint) {
			var best = base.MeasureOverride(SizeHelper.Infinite);
			if(CaptionControl != null) {
				best.Width -= CaptionControl.DesiredSize.Width;
				MinDesiredWidth = best.Width + ActualPageCaptionMinWidth;
				best.Width += Math.Max(ActualPageCaptionMinWidth, CaptionControl.MaxDesiredSize.Width) + (Ribbon == null ? 0d : Ribbon.MaxPageCaptionTextIndent);
			}
			SeparatorOpacity = Double.IsPositiveInfinity(constraint.Width) ? 0d : 1 - (constraint.Width - MinDesiredWidth) / (best.Width - MinDesiredWidth);
			BestDesiredSize = best;
			return base.MeasureOverride(constraint);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(IsWaitingDoubleClick()) {
				StopDoubleClickTimer();
				OnMouseLeftButtonDoubleClick();
				return;
			}
			if(AllowDoubleClickTimer()) {
				StartDoubleClickTimer();
			} else {
				if(e.ClickCount == 1) {
					OnMouseLeftButtonSingleClick();
				} else {
					OnMouseLeftButtonDoubleClick();
				}
			}
		}
		protected virtual void OnAeroTemplateChanged(ControlTemplate oldValue) {
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if(IsAeroMode)
				Template = AeroTemplate;
		}
		protected virtual void OnIsAeroModeChanged(bool oldValue) {
			UpdateTemplate();
		}
		protected internal virtual void UpdateCaptionControlProperties() {
			if(CaptionControl == null) return;
			CaptionControl.IsSelected = ((IsPageSelected && !IsRibbonMinimized) || (IsPageSelected && !IsMinimizedRibbonCollapsed)) && !IsRibbonBackStageViewOpened;
			CaptionControl.PageHeaderControl = this;
		}
		protected virtual void OnIsPageSelectedChanged(bool oldValue) {
			UpdateCaptionControlProperties();
			RaiseWeakPageIsSelectedChanged(new ValueChangedEventArgs<bool>(oldValue, IsPageSelected));
		}
		DevExpress.Xpf.Bars.Native.WeakList<ValueChangedEventHandler<bool>> handlersWeakPageIsSelectedChanged = new Bars.Native.WeakList<ValueChangedEventHandler<bool>>();
		protected internal event ValueChangedEventHandler<bool> WeakPageIsSelectedChanged {
			add { handlersWeakPageIsSelectedChanged.Add(value); }
			remove { handlersWeakPageIsSelectedChanged.Remove(value); }
		}
		void RaiseWeakPageIsSelectedChanged(ValueChangedEventArgs<bool> args) {
			foreach(ValueChangedEventHandler<bool> e in handlersWeakPageIsSelectedChanged)
				e(this, args);
		}
		protected virtual void UpdateActualIsMinimizedRibbonCollapsed() {
			ActualIsMinimizedRibbonCollapsed = IsMinimizedRibbonCollapsed;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CaptionControl = GetTemplateChild("PART_CaptionControl") as RibbonPageCaptionControl;
			SeparatorControl = GetTemplateChild("PART_Separator") as Control;
			UpdateCaptionControlProperties();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(IsWaitingDoubleClick()) {
				StopDoubleClickTimer();
				OnMouseLeftButtonSingleClick();
			}
		}
		protected virtual void OnBestDesiredSizeChanged(Size oldValue) { }
		protected virtual void OnMinDesiredWidthChanged() { }
	}
}
