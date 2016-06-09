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

using System.Windows;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Bars;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Themes;
using System.Collections.Generic;
using System;
using DevExpress.Xpf.Utils;
using System.Windows.Documents;
using DevExpress.Mvvm.Native;
using System.Windows.Data;
namespace DevExpress.Xpf.Ribbon {
	public class DXRibbonWindow : DXWindow, IRibbonWindow {
		#region static
		public static readonly DependencyProperty IsCaptionVisibleProperty;
		public static readonly DependencyProperty IsRibbonCaptionVisibleProperty;
		public static readonly DependencyProperty AeroContentMarginProperty;
		protected internal static readonly DependencyPropertyKey AeroContentMarginPropertyKey;
		public static readonly DependencyProperty ContentPresenterMarginProperty;
		protected internal static readonly DependencyPropertyKey ContentPresenterMarginPropertyKey;
		public static readonly DependencyProperty RibbonHeaderBorderHeightProperty;
		protected internal static readonly DependencyPropertyKey RibbonHeaderBorderHeightPropertyKey;
		public static readonly DependencyProperty RibbonAutoHideModeProperty;
		public static readonly DependencyProperty DisplayShowModeSelectorProperty;
		public static readonly DependencyProperty ActualRibbonProperty;
		protected static readonly DependencyPropertyKey ActualRibbonPropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty RibbonPulseProperty;				
		static DXRibbonWindow() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DXRibbonWindow), new FrameworkPropertyMetadata(typeof(DXRibbonWindow)));
			RibbonPulseProperty = DependencyPropertyManager.RegisterAttached("RibbonPulse", typeof(bool), typeof(DXRibbonWindow), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRibbonPulsePropertyChanged)));
			IsRibbonCaptionVisibleProperty = DependencyPropertyManager.Register("IsRibbonCaptionVisible", typeof(bool), typeof(DXRibbonWindow), new PropertyMetadata(false));
			IsCaptionVisibleProperty = DependencyPropertyManager.Register("IsCaptionVisible", typeof(bool), typeof(DXRibbonWindow), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsCaptionVisiblePropertyChanged)));
			ContentPresenterMarginPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ContentPresenterMargin", typeof(Thickness), typeof(DXRibbonWindow), new UIPropertyMetadata(new Thickness()));
			ContentPresenterMarginProperty = ContentPresenterMarginPropertyKey.DependencyProperty;
			AeroContentMarginPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("AeroContentMargin", typeof(Thickness), typeof(DXRibbonWindow), new PropertyMetadata(new Thickness(0, 0, 0, 0), new PropertyChangedCallback(OnAeroContentMarginPropertyChanged)));
			AeroContentMarginProperty = AeroContentMarginPropertyKey.DependencyProperty;
			RibbonHeaderBorderHeightPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("RibbonHeaderBorderHeight", typeof(double), typeof(DXRibbonWindow), new FrameworkPropertyMetadata(double.NaN));
			RibbonHeaderBorderHeightProperty = RibbonHeaderBorderHeightPropertyKey.DependencyProperty;
			RibbonAutoHideModeProperty = DependencyPropertyManager.Register("RibbonAutoHideMode", typeof(bool), typeof(DXRibbonWindow), new FrameworkPropertyMetadata(false, (d, e) => ((DXRibbonWindow)d).OnRibbonAutoHideModeChanged((bool)e.OldValue)));
			DisplayShowModeSelectorProperty = DependencyPropertyManager.Register("DisplayShowModeSelector", typeof(bool), typeof(DXRibbonWindow), new FrameworkPropertyMetadata(false));
			ActualRibbonPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualRibbon", typeof(RibbonControl), typeof(DXRibbonWindow), new FrameworkPropertyMetadata(null));
			ActualRibbonProperty = ActualRibbonPropertyKey.DependencyProperty;
		}
		public RibbonControl ActualRibbon {
			get { return (RibbonControl)GetValue(ActualRibbonProperty); }
		}
		public static Thickness GetAeroContentMargin(DependencyObject obj) {
			return (Thickness)obj.GetValue(AeroContentMarginProperty);
		}
		protected internal static void SetAeroContentMargin(DependencyObject obj, Thickness value) {
			obj.SetValue(AeroContentMarginPropertyKey, value);
		}
		public static Thickness GetContentPresenterMargin(DependencyObject obj) {
			return (Thickness)obj.GetValue(ContentPresenterMarginProperty);
		}
		protected internal static void SetContentPresenterMargin(DependencyObject obj, Thickness value) {
			obj.SetValue(ContentPresenterMarginPropertyKey, value);
		}
		public static double GetRibbonHeaderBorderHeight(DependencyObject obj) {
			return (double)obj.GetValue(RibbonHeaderBorderHeightProperty);
		}
		protected internal static void SetRibbonHeaderBorderHeight(DependencyObject obj, double value) {
			obj.SetValue(RibbonHeaderBorderHeightPropertyKey, value);
		}
		protected internal static void OnIsCaptionVisiblePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindow)sender).OnIsCaptionVisibleChanged((bool)e.OldValue);
		}
		protected internal static void OnAeroContentMarginPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			if(sender is DXRibbonWindow)
				((DXRibbonWindow)sender).OnAeroContentMarginChanged((Thickness)e.OldValue);
		}
		protected virtual void OnRibbonAutoHideModeChanged(bool oldValue) {
			if (RibbonAutoHideMode) {
				SetCurrentValue(WindowStateProperty, System.Windows.WindowState.Maximized);
			} else {
				ClearValue(WindowStateProperty);
			}
		}
		#endregion
		#region prop defs
		public bool RibbonAutoHideMode {
			get { return (bool)GetValue(RibbonAutoHideModeProperty); }
			set { SetValue(RibbonAutoHideModeProperty, value); }
		}
		public bool IsRibbonCaptionVisible {
			get { return (bool)GetValue(IsRibbonCaptionVisibleProperty); }
			set { SetValue(IsRibbonCaptionVisibleProperty, value); }
		}
		public bool IsCaptionVisible {
			get { return (bool)GetValue(IsCaptionVisibleProperty); }
			set { SetValue(IsCaptionVisibleProperty, value); }
		}
		public bool DisplayShowModeSelector {
			get { return (bool)GetValue(DisplayShowModeSelectorProperty); }
			set { SetValue(DisplayShowModeSelectorProperty, value); }
		}
		#endregion
		#region props
		bool isMinimizedHeaderVisibleCore;
		bool IsMinimizedHeaderVisible {
			get { return isMinimizedHeaderVisibleCore; }
			set {
				if(isMinimizedHeaderVisibleCore == value) return;
				bool oldValue = isMinimizedHeaderVisibleCore;
				isMinimizedHeaderVisibleCore = value;
				OnIsMinimizedHeaderVisibleChanged(oldValue);
			}
		}
		#endregion
		public DXRibbonWindow() {	
			LayoutUpdated += OnLayoutUpdated;
			Loaded += OnLoaded;
		}
		private void OnLoaded(object sender, RoutedEventArgs e) { }
		RibbonControl ribbon;
		internal RibbonControl Ribbon {
			get { return ribbon; }
			set {
				if (value == ribbon)
					return;
				RibbonControl oldValue = ribbon;
				ribbon = value;
				OnRibbonChanged(oldValue);
			}
		}
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			Ribbon.Do(ribbon => ribbon.UpdateIsInRibbonWindow());
			SetValue(ActualRibbonPropertyKey, Ribbon);
		}
		static void OnRibbonPulsePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			(d as RibbonControl).Do(x => x.UpdateWindow());
		}
		public void UpdateRibbon() {
			SetValue(RibbonPulseProperty, true);
			ClearValue(RibbonPulseProperty);
		}
		protected override void ShowSystemMenuOnRightClick(Point pos) {
			if (ActualRibbon != null && BarManager.GetDXContextMenu(ActualRibbon).ReturnSuccess())
				return;
			base.ShowSystemMenuOnRightClick(pos);
		}
		override protected bool CanResizeOrMaximize { get { return base.CanResizeOrMaximize && !RibbonAutoHideMode; } }
		protected override Thickness GetAeroBorderThickness() {
			Thickness result = new Thickness(-1);
			if(this.WindowState == System.Windows.WindowState.Maximized && Ribbon != null && Ribbon.HeaderBorder != null) {
				result.Top = Ribbon.HeaderBorder.RenderSize.Height;
			}
			return result;
		}
		internal EventHandler IsAeroModeChanged;
		protected override void OnIsAeroModeChanged(bool oldValue) {
			base.OnIsAeroModeChanged(oldValue);
			if(IsAeroModeChanged != null)
				IsAeroModeChanged(this, new EventArgs());
		}
		private void OnIsMinimizedHeaderVisibleChanged(bool oldValue) {
			FrameworkElement minimizedHeader = GetMinimizedHeaderContainer();
			if(minimizedHeader == null) return;
			minimizedHeader.Visibility = IsMinimizedHeaderVisible ? Visibility.Visible : Visibility.Collapsed;
		}
		protected internal virtual FrameworkElement GetMinimizedHeaderContainer() {
			return this.GetVisualByName("MinimizedHeader") as FrameworkElement;
		}
		protected internal virtual FrameworkElement GetControlBoxContainer() {
			return this.GetVisualByName("PART_ControlBoxContainer") as FrameworkElement;
		}
		protected internal virtual FrameworkElement GetTitleContainer() {
			return this.GetVisualByName("PART_RibbonCaptionContentPresenter") as FrameworkElement;
		}
		protected internal virtual UIElement GetContentContainer() {
			return LayoutHelper.FindElementByName(this, "PART_ContainerContent") as UIElement;
		}
		internal void DragOrMaximizeWindow(object sender, MouseButtonEventArgs e) {
			if(RibbonAutoHideMode)
				return;
			DragWindowOrMaximizeWindow(sender, e);
		}
		internal void ShowSystemMenu(MouseButtonEventArgs e) {
			Point offset = e.GetPosition(this);
			offset.X += Left;
			offset.Y += Top;
			ShowSystemMenu(offset);
		}
		internal void ShowSystemMenu(UIElement relativeObject, Point relativeOffset) {
			if(relativeObject == null)
				return;
			Point truePos = GetTruePos();
			Point offset = relativeObject.TransformToAncestor(relativeObject.GetRootVisual()).Transform(new Point(0, 0));
			offset.X += truePos.X + relativeOffset.X;
			offset.Y += truePos.Y + relativeOffset.Y;
			ShowSystemMenu(offset);
		}
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) { }
		protected internal new void ResizeWindow(DXWindowActiveResizeParts resizeMode) {
			base.ResizeWindow(resizeMode);
		}
		protected internal void SetAeroContentHorizontalOffset(double offset) {
			DXRibbonWindow.SetAeroContentMargin(this, new Thickness(0, offset, 0, 0));
			DXRibbonWindow.SetContentPresenterMargin(this, new Thickness(0, -offset, 0, 0));
		}
		protected internal void SetRibbonHeaderBorderHeight(double height) {
			DXRibbonWindow.SetRibbonHeaderBorderHeight(this, height);
			DXRibbonWindow.SetContentPresenterMargin(this, new Thickness(0, -height, 0, 0));
		}
		private void OnIsCaptionVisibleChanged(bool oldValue) {
			if(IsCaptionVisible) {
				SetRibbonHeaderBorderHeight(0);
			}
			if(IsAeroMode) {
				ProcessDWM();
				UpdateLayout();
			}
		}
		protected virtual void OnAeroContentMarginChanged(Thickness oldValue) {
			if(IsAeroMode) {
				ProcessDWM();
				UpdateLayout();
			}
		}
		protected override void OnStateChanged(EventArgs e) {
			base.OnStateChanged(e);
			IsMinimizedHeaderVisible = WindowState == WindowState.Minimized && !ShowInTaskbar;
			if(WindowState == WindowState.Normal)
				SetCurrentValue(RibbonAutoHideModeProperty, false);
		}
		internal void ForceClose() {
			OnClose();
		}
		internal void ForceMinimize() {
			OnMinimize();
		}
		internal void ForceRestore() {
			OnRestore();
		}
		internal void ForceMaximize() {
			OnMaximize();
		}
		protected override bool AllowOptimizeUpdateRootMargins() { return false; }
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (Ribbon == null || WindowState != WindowState.Maximized)
				return;
			Point ribbonOffset = e.GetPosition(Ribbon);
			if (ribbonOffset.X > 0 && ribbonOffset.X < Ribbon.ActualWidth && ribbonOffset.Y < 0)
				DragOrMaximizeWindow(this, e);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			RootContentPresenter = GetTemplateChild("PART_RootContentPresenter") as UIElement;
			DXWindow.SetIsMaximized(this, WindowState == WindowState.Maximized);
		}
		protected override Rect GetClientArea() {
			Rect clientArea = base.GetClientArea();
			if(!clientArea.Equals(Rect.Empty) && Ribbon != null && Ribbon.HeaderBorder != null) {
				double headerHeight = Ribbon.HeaderBorder.RenderSize.Height;
				clientArea = new Rect(new Point(clientArea.Left, clientArea.Top + headerHeight), clientArea.Size);
			}
			return clientArea;
		}
		UIElement RootContentPresenter { get; set; }
	}
	public class DXRibbonWindowContainer : ContentControl {
		#region static
		public static readonly DependencyProperty IsActiveProperty;
		public static readonly DependencyProperty IsStatusBarVisibleProperty;
		public static readonly DependencyProperty IsHeaderVisibleProperty;
		public static readonly DependencyProperty IconProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty IsMaximizeButtonVisibleProperty;
		public static readonly DependencyProperty IsMinimizeButtonVisibleProperty;
		public static readonly DependencyProperty IsRestoreButtonVisibleProperty;
		public static readonly DependencyProperty IsCloseButtonVisibleProperty;
		public bool IsAeroMode {
			get { return (bool)GetValue(IsAeroModeProperty); }
			set { SetValue(IsAeroModeProperty, value); }
		}
		public static readonly DependencyProperty IsAeroModeProperty =
			DependencyPropertyManager.Register("IsAeroMode", typeof(bool), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAeroModePropertyChanged)));
		protected static void OnIsAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIsAeroModeChanged((bool)e.OldValue);
		}
		protected virtual void OnIsAeroModeChanged(bool oldValue) {
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if(IsAeroMode)
				Template = AeroTemplate;
		}
		public ControlTemplate AeroTemplate {
			get { return (ControlTemplate)GetValue(AeroTemplateProperty); }
			set { SetValue(AeroTemplateProperty, value); }
		}
		public static readonly DependencyProperty AeroTemplateProperty =
			DependencyPropertyManager.Register("AeroTemplate", typeof(ControlTemplate), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAeroTemplatePropertyChanged)));
		protected static void OnAeroTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnAeroTemplateChanged((ControlTemplate)e.OldValue);
		}
		protected virtual void OnAeroTemplateChanged(ControlTemplate oldValue) {
			UpdateTemplate();
		}
		static DXRibbonWindowContainer() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(typeof(DXRibbonWindowContainer)));						
			IsActiveProperty = DependencyPropertyManager.Register("IsActive", typeof(bool), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsActivePropertyChanged)));
			IsStatusBarVisibleProperty = DependencyPropertyManager.Register("IsStatusBarVisible", typeof(bool), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsStatusBarVisiblePropertyChanged)));
			IsHeaderVisibleProperty = DependencyPropertyManager.Register("IsHeaderVisible", typeof(bool), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsHeaderVisiblePropertyChanged)));
			IconProperty = DependencyPropertyManager.Register("Icon", typeof(ImageSource), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIconPropertyChanged)));
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnCaptionPropertyChanged)));
			IsMaximizeButtonVisibleProperty = DependencyPropertyManager.Register("IsMaximizeButtonVisible", typeof(bool), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsMaximizeButtonVisiblePropertyChanged)));
			IsMinimizeButtonVisibleProperty = DependencyPropertyManager.Register("IsMinimizeButtonVisible", typeof(bool), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsMinimizeButtonVisiblePropertyChanged)));
			IsRestoreButtonVisibleProperty = DependencyPropertyManager.Register("IsRestoreButtonVisible", typeof(bool), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsRestoreButtonVisiblePropertyChanged)));
			IsCloseButtonVisibleProperty = DependencyPropertyManager.Register("IsCloseButtonVisible", typeof(bool), typeof(DXRibbonWindowContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsCloseButtonVisiblePropertyChanged)));
		}
		protected static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIsActiveChanged((bool)e.OldValue);
		}
		protected static void OnIsStatusBarVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIsStatusBarVisibleChanged((bool)e.OldValue);
		}
		protected static void OnIsHeaderVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIsHeaderVisibleChanged((bool)e.OldValue);
		}
		protected static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIconChanged((ImageSource)e.OldValue);
		}
		protected static void OnCaptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnCaptionChanged((string)e.OldValue);
		}
		protected static void OnIsMaximizeButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIsMaximizeButtonVisibleChanged((bool)e.OldValue);
		}
		protected static void OnIsMinimizeButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIsMinimizeButtonVisibleChanged((bool)e.OldValue);
		}
		protected static void OnIsRestoreButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIsRestoreButtonVisibleChanged((bool)e.OldValue);
		}
		protected static void OnIsCloseButtonVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowContainer)d).OnIsCloseButtonVisibleChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public bool IsActive {
			get { return (bool)GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}
		public bool IsStatusBarVisible {
			get { return (bool)GetValue(IsStatusBarVisibleProperty); }
			set { SetValue(IsStatusBarVisibleProperty, value); }
		}
		public bool IsHeaderVisible {
			get { return (bool)GetValue(IsHeaderVisibleProperty); }
			set { SetValue(IsHeaderVisibleProperty, value); }
		}
		public ImageSource Icon {
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public bool IsMaximizeButtonVisible {
			get { return (bool)GetValue(IsMaximizeButtonVisibleProperty); }
			set { SetValue(IsMaximizeButtonVisibleProperty, value); }
		}
		public bool IsMinimizeButtonVisible {
			get { return (bool)GetValue(IsMinimizeButtonVisibleProperty); }
			set { SetValue(IsMinimizeButtonVisibleProperty, value); }
		}
		public bool IsRestoreButtonVisible {
			get { return (bool)GetValue(IsRestoreButtonVisibleProperty); }
			set { SetValue(IsRestoreButtonVisibleProperty, value); }
		}
		public bool IsCloseButtonVisible {
			get { return (bool)GetValue(IsCloseButtonVisibleProperty); }
			set { SetValue(IsCloseButtonVisibleProperty, value); }
		}
		#endregion
		public DXRibbonWindowContainer() {
		}
		protected virtual void OnIsStatusBarVisibleChanged(bool oldValue) {
		}
		protected virtual void OnIsActiveChanged(bool oldValue) {
		}
		protected virtual void OnIsHeaderVisibleChanged(bool oldValue) {
		}
		protected virtual void OnIconChanged(ImageSource oldValue) {
		}
		protected virtual void OnCaptionChanged(string oldValue) {
		}
		protected virtual void OnIsMaximizeButtonVisibleChanged(bool oldValue) {
		}
		protected virtual void OnIsMinimizeButtonVisibleChanged(bool oldValue) {
		}
		protected virtual void OnIsRestoreButtonVisibleChanged(bool oldValue) {
		}
		protected virtual void OnIsCloseButtonVisibleChanged(bool oldValue) {
		}
		ContentControl CaptionContainer { get; set; }
		ContentControl ControlBoxContainer { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CaptionContainer = GetTemplateChild("PART_CaptionContainer") as ContentControl;
			ControlBoxContainer = GetTemplateChild("PART_ControlBoxContainer") as ContentControl;
		}
		protected internal virtual void SetAeroContentHorizontalOffset(double offset) {
			DXRibbonWindow.SetAeroContentMargin(this, new Thickness(0, offset, 0, 0));
			DXRibbonWindow.SetContentPresenterMargin(this, new Thickness(0, -offset, 0, 0));			
		}
		protected internal virtual void SetRibbonHeaderBorderHeight(double height) {
			DXRibbonWindow.SetRibbonHeaderBorderHeight(this, height);
			DXRibbonWindow.SetContentPresenterMargin(this, new Thickness(0, -height, 0, 0));		   
		}
		internal ContentControl GetCaptionContainer() {
			return CaptionContainer;
		}
		internal FrameworkElement GetControlBoxContainer() {
			return ControlBoxContainer;
		}
	}
}
