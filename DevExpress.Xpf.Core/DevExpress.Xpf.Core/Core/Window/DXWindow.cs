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
using System.IO;
using System.Windows;
using System.Security;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Text;
#if DXWINDOW
using DevExpress.Internal.DXWindow.Core.HandleDecorator;
using ActiveResizeParts = DevExpress.Internal.DXWindow.DXWindowActiveResizeParts;
using Decorator = DevExpress.Internal.DXWindow.Core.HandleDecorator.Decorator;
#else
using DevExpress.Xpf.Core.Native;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native;
using ActiveResizeParts = DevExpress.Xpf.Core.DXWindowActiveResizeParts;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Core.HandleDecorator;
using DevExpress.Xpf.Core.HandleDecorator.Helpers;
using Decorator = DevExpress.Xpf.Core.HandleDecorator.Decorator;
using System.Windows.Controls.Primitives;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public enum DXWindowActiveResizeParts {
		Bottom = 6,
		Top = 3,
		Right = 2,
		Left = 1,
		BottomRight = 8,
		BottomLeft = 7,
		TopRight = 5,
		TopLeft = 4,
		SizeGrip = 8
	}
	public enum BorderEffect {
		Default,
		None
	};
	public class DXWindow : Window, ILogicalOwner, IWindowResizeHelperClient {
		#region static
		public static readonly DependencyProperty IsDraggingOrResizingProperty;
		public static readonly DependencyProperty WindowTemplateProperty;
		public static readonly DependencyProperty AeroWindowTemplateProperty;
		protected static readonly DependencyPropertyKey ActualWindowTemplatePropertyKey;
		public static readonly DependencyProperty ActualWindowTemplateProperty;
		public static readonly DependencyProperty ShowIconProperty;
		public static readonly DependencyProperty ShowTitleProperty;
		public static readonly DependencyProperty SmallIconProperty;
		public static readonly DependencyProperty IsMaximizedProperty;
		public static readonly DependencyProperty IsActiveExProperty;
		public static readonly DependencyProperty AeroControlBoxWidthProperty;
		protected static readonly DependencyPropertyKey AeroControlBoxWidthPropertyKey;
		public static readonly DependencyProperty AeroControlBoxHeightProperty;
		protected static readonly DependencyPropertyKey AeroControlBoxHeightPropertyKey;
		public static readonly DependencyProperty AeroControlBoxMarginProperty;
		protected static readonly DependencyPropertyKey AeroControlBoxMarginPropertyKey;
		public static readonly DependencyProperty AeroBorderSizeProperty;
		public static readonly DependencyProperty IsAeroModeProperty;
		static DXWindow() {
			isAeroModeSupported = CheckSupportAeroMode();
			UpdateTransformationMatrixes();
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DXWindow), new FrameworkPropertyMetadata(typeof(DXWindow)));
			IsDraggingOrResizingProperty = DependencyProperty.Register("IsDraggingOrResizing", typeof(bool), typeof(DXWindow), new UIPropertyMetadata(false));
			WindowTemplateProperty = DependencyProperty.Register("WindowTemplate", typeof(DataTemplate), typeof(DXWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnWindowTemplatePropertyChanged)));
			AeroWindowTemplateProperty = DependencyProperty.Register("AeroWindowTemplate", typeof(DataTemplate), typeof(DXWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAeroWindowTemplatePropertyChanged)));
			ActualWindowTemplatePropertyKey = DependencyProperty.RegisterReadOnly("ActualWindowTemplate", typeof(DataTemplate), typeof(DXWindow), new FrameworkPropertyMetadata(null));
			ActualWindowTemplateProperty = ActualWindowTemplatePropertyKey.DependencyProperty;
			ShowIconProperty = DependencyProperty.RegisterAttached("ShowIcon", typeof(bool), typeof(DXWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, null));
			ShowTitleProperty = DependencyProperty.RegisterAttached("ShowTitle", typeof(bool), typeof(DXWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnShowTitlePropertyChanged)));
			SmallIconProperty = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(DXWindow), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSmallIconPropertyChanged)));
			IsMaximizedProperty = FloatingContainer.IsMaximizedProperty.AddOwner(typeof(DXWindow));
			IsActiveExProperty = FloatingContainer.IsActiveProperty.AddOwner(typeof(DXWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIsActivePropertyChanged)));
			AeroControlBoxWidthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AeroControlBoxWidth", typeof(double), typeof(DXWindow), new PropertyMetadata(double.NaN));
			AeroControlBoxWidthProperty = AeroControlBoxWidthPropertyKey.DependencyProperty;
			AeroControlBoxHeightPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AeroControlBoxHeight", typeof(double), typeof(DXWindow), new PropertyMetadata(double.NaN));
			AeroControlBoxHeightProperty = AeroControlBoxHeightPropertyKey.DependencyProperty;
			AeroControlBoxMarginPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AeroControlBoxMargin", typeof(Thickness), typeof(DXWindow), new PropertyMetadata(new Thickness()));
			AeroControlBoxMarginProperty = AeroControlBoxMarginPropertyKey.DependencyProperty;
			AeroBorderSizeProperty = DependencyProperty.Register("AeroBorderSize", typeof(double), typeof(DXWindow), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnAeroBorderSizePropertyChanged)));
			IsAeroModeProperty = DependencyProperty.Register("IsAeroMode", typeof(bool), typeof(DXWindow), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAeroModePropertyChanged), new CoerceValueCallback(OnIsAeroModePropertyCoerce)));
			Window.VisibilityProperty.AddOwner(typeof(DXWindow), new FrameworkPropertyMetadata(Visibility.Collapsed, DXWindow.OnVisibilityChanged));
			FrameworkElement.UseLayoutRoundingProperty.AddOwner(typeof(DXWindow), new FrameworkPropertyMetadata(false, DXWindow.OnUseLayoutRoundingPropertyChanged));
			Window.WindowStateProperty.AddOwner(typeof(DXWindow), new FrameworkPropertyMetadata(WindowState.Normal, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DXWindow.OnWindowStateChanged));
			Panel.BackgroundProperty.AddOwner(typeof(DXWindow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(DXWindow.OnWindowBackgroundChanged)));
			Window.WindowStyleProperty.AddOwner(typeof(DXWindow), new FrameworkPropertyMetadata(WindowStyle.SingleBorderWindow, DXWindow.OnWindowStyleChanged));
			Window.ResizeModeProperty.AddOwner(typeof(DXWindow), new FrameworkPropertyMetadata(ResizeMode.CanResize, FrameworkPropertyMetadataOptions.AffectsMeasure, DXWindow.OnResizeModeChanged));
			Window.IconProperty.AddOwner(typeof(DXWindow), new FrameworkPropertyMetadata(DXWindow.OnIconChanged));
			EventManager.RegisterClassHandler(typeof(UIElement), ContextMenuOpeningEvent, new ContextMenuEventHandler(OnContextMenuOpening));
		}
		[SecuritySafeCritical]
		private static void UpdateTransformationMatrixes() {
			IntPtr hdc = NativeMethods.GetDC(IntPtr.Zero);
			double ppiX = NativeMethods.GetDeviceCaps(hdc, NativeMethods.LOGPIXELSX);
			double ppiY = NativeMethods.GetDeviceCaps(hdc, NativeMethods.LOGPIXELSY);
			DeviceToDIPTransform = Matrix.Identity;
			DeviceToDIPTransform.Scale(96d / ppiX, 96d / ppiY);
			DIPToDeviceTransform = Matrix.Identity;
			DIPToDeviceTransform.Scale(ppiX / 96d, ppiY / 96d);
			NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
		}
		public static Thickness GetAeroControlBoxMargin(DependencyObject obj) {
			return (Thickness)obj.GetValue(AeroControlBoxMarginProperty);
		}
		protected static void SetAeroControlBoxMargin(DependencyObject obj, Thickness value) {
			obj.SetValue(AeroControlBoxMarginPropertyKey, value);
		}
		public static double GetAeroControlBoxWidth(DependencyObject obj) {
			return (double)obj.GetValue(AeroControlBoxWidthProperty);
		}
		protected static void SetAeroControlBoxWidth(DependencyObject obj, double value) {
			obj.SetValue(AeroControlBoxWidthPropertyKey, value);
		}
		public static double GetAeroControlBoxHeight(DependencyObject obj) {
			return (double)obj.GetValue(AeroControlBoxHeightProperty);
		}
		protected static void SetAeroControlBoxHeight(DependencyObject obj, double value) {
			obj.SetValue(AeroControlBoxHeightPropertyKey, value);
		}
		public static bool GetShowIcon(DependencyObject obj) {
			return (bool)obj.GetValue(ShowIconProperty);
		}
		public static void SetShowIcon(DependencyObject obj, bool value) {
			obj.SetValue(ShowIconProperty, value);
		}
		public static bool GetShowTitle(DependencyObject obj) {
			return (bool)obj.GetValue(ShowTitleProperty);
		}
		public static void SetShowTitle(DependencyObject obj, bool value) {
			obj.SetValue(ShowTitleProperty, value);
		}
		public static bool GetIsMaximized(DependencyObject obj) {
			return (bool)obj.GetValue(IsMaximizedProperty);
		}
		public static void SetIsMaximized(DependencyObject obj, bool value) {
			obj.SetValue(IsMaximizedProperty, value);
		}
		public static bool GetIsActiveEx(DependencyObject obj) {
			return (bool)obj.GetValue(IsActiveExProperty);
		}
		public static void SetIsActiveEx(DependencyObject obj, bool value) {
			obj.SetValue(IsActiveExProperty, value);
		}
		private static void OnWindowTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).UpdateActualWindowTemplate();
		}
		private static void OnSmallIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnSmallIconChanged((ImageSource)e.OldValue);
		}
		private static void OnShowTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnShowTitlePropertyChanged((bool)e.OldValue);
		}
		private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnIsActiveChanged((bool)e.NewValue);
		}
		private static void OnAeroWindowTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).UpdateActualWindowTemplate();
		}
		protected static void OnAeroBorderSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnAeroBorderSizeChanged((double)e.OldValue);
		}
		protected static void OnIsAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnIsAeroModeChanged((bool)e.OldValue);
		}
		protected static object OnIsAeroModePropertyCoerce(DependencyObject d, object baseValue) {
			if(d is DXWindow) return ((DXWindow)d).CoerceIsAeroModeValue((bool)baseValue); else return baseValue;
		}
		#endregion
		#region dep props
#if !DXWINDOW
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXWindowWindowTemplate")]
#endif
#endif
		public DataTemplate WindowTemplate {
			get { return (DataTemplate)GetValue(WindowTemplateProperty); }
			set { SetValue(WindowTemplateProperty, value); }
		}
#if !DXWINDOW
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXWindowAeroWindowTemplate")]
#endif
#endif
		public DataTemplate AeroWindowTemplate {
			get { return (DataTemplate)GetValue(AeroWindowTemplateProperty); }
			set { SetValue(AeroWindowTemplateProperty, value); }
		}
		public DataTemplate ActualWindowTemplate {
			get { return (DataTemplate)GetValue(ActualWindowTemplateProperty); }
			protected set { this.SetValue(ActualWindowTemplatePropertyKey, value); }
		}
#if !DXWINDOW
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXWindowShowIcon")]
#endif
#endif
		public bool ShowIcon {
			get { return (bool)GetValue(ShowIconProperty); }
			set { SetValue(ShowIconProperty, value); }
		}
#if !DXWINDOW
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXWindowShowTitle")]
#endif
#endif
		public bool ShowTitle {
			get { return (bool)GetValue(ShowTitleProperty); }
			set { SetValue(ShowTitleProperty, value); }
		}
#if !DXWINDOW
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXWindowSmallIcon")]
#endif
#endif
		public ImageSource SmallIcon {
			get { return (ImageSource)GetValue(SmallIconProperty); }
			set { SetValue(SmallIconProperty, value); }
		}
#if !DXWINDOW
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXWindowAeroBorderSize")]
#endif
#endif
		public double AeroBorderSize {
			get { return (double)GetValue(AeroBorderSizeProperty); }
			set { SetValue(AeroBorderSizeProperty, value); }
		}
#if !DXWINDOW
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXWindowIsAeroMode")]
#endif
#endif
		public bool IsAeroMode {
			get { return (bool)GetValue(IsAeroModeProperty); }
			set { SetValue(IsAeroModeProperty, value); }
		}
		#endregion
		public Thickness ResizeBorderThickness {
			get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
			set { SetValue(ResizeBorderThicknessProperty, value); }
		}
		public Thickness ResizeBorderThicknessInAeroMode {
			get { return (Thickness)GetValue(ResizeBorderThicknessInAeroModeProperty); }
			set { SetValue(ResizeBorderThicknessInAeroModeProperty, value); }
		}
		public Thickness BorderEffectLeftMargins {
			get { return (Thickness)GetValue(BorderEffectLeftMarginsProperty); }
			set { }
		}
		public Thickness BorderEffectRightMargins {
			get { return (Thickness)GetValue(BorderEffectRightMarginsProperty); }
			set { }
		}
		public Thickness BorderEffectTopMargins {
			get { return (Thickness)GetValue(BorderEffectTopMarginsProperty); }
			set { }
		}
		public Thickness BorderEffectBottomMargins {
			get { return (Thickness)GetValue(BorderEffectBottomMarginsProperty); }
			set { }
		}
		public TextBlock BorderEffectImagesUri {
			get { return (TextBlock)GetValue(BorderEffectImagesUriProperty); }
			set { }
		}
		public Thickness BorderEffectOffset {
			get { return (Thickness)GetValue(BorderEffectOffsetProperty); }
			set { }
		}
		public static readonly DependencyProperty BorderEffectLeftMarginsProperty =
			DependencyProperty.Register("BorderEffectLeftMargins", typeof(Thickness), typeof(DXWindow));
		public static readonly DependencyProperty BorderEffectRightMarginsProperty =
			DependencyProperty.Register("BorderEffectRightMargins", typeof(Thickness), typeof(DXWindow));
		public static readonly DependencyProperty BorderEffectTopMarginsProperty =
			DependencyProperty.Register("BorderEffectTopMargins", typeof(Thickness), typeof(DXWindow));
		public static readonly DependencyProperty BorderEffectBottomMarginsProperty =
			DependencyProperty.Register("BorderEffectBottomMargins", typeof(Thickness), typeof(DXWindow));
		public static readonly DependencyProperty BorderEffectImagesUriProperty =
			DependencyProperty.Register("BorderEffectImagesUri", typeof(TextBlock), typeof(DXWindow));
		public static readonly DependencyProperty BorderEffectOffsetProperty =
			DependencyProperty.Register("BorderEffectOffset", typeof(Thickness), typeof(DXWindow));
		public SolidColorBrush BorderEffectActiveColor {
			get { return (SolidColorBrush)GetValue(BorderEffectActiveColorProperty); }
			set { SetValue(BorderEffectActiveColorProperty, value); }
		}
		public static readonly DependencyProperty BorderEffectActiveColorProperty =
			DependencyProperty.Register("BorderEffectActiveColor", typeof(SolidColorBrush), typeof(DXWindow), new FrameworkPropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnBorderEffectActiveColorPropertyChanged)));
		protected static void OnBorderEffectActiveColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnBorderEffectActiveColorChanged((SolidColorBrush)e.OldValue);
		}
		public SolidColorBrush BorderEffectInactiveColor {
			get { return (SolidColorBrush)GetValue(BorderEffectInactiveColorProperty); }
			set { SetValue(BorderEffectInactiveColorProperty, value); }
		}
		public static readonly DependencyProperty BorderEffectInactiveColorProperty =
			DependencyProperty.Register("BorderEffectInactiveColor", typeof(SolidColorBrush), typeof(DXWindow), new FrameworkPropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnBorderEffectInactiveColorPropertyChanged)));
		protected static void OnBorderEffectInactiveColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnBorderEffectInactiveColorChanged((SolidColorBrush)e.OldValue);
		}
		protected virtual void OnBorderEffectActiveColorChanged(SolidColorBrush oldValue) {
			if(decorator != null) {
				decorator.ActiveColor = BorderEffectActiveColor;
				decorator.RenderDecorator();
				RedrawBorderWithEffectColor(this.ActiveState, false);
			}
		}
		protected virtual void OnBorderEffectInactiveColorChanged(SolidColorBrush oldValue) {
			if(decorator != null) {
				decorator.InactiveColor = BorderEffectInactiveColor;
				decorator.RenderDecorator();
				RedrawBorderWithEffectColor(this.ActiveState, false);
			}
		}
		void RedrawBorderWithEffectColor(bool activeState, bool needLayoutUpdate) {
			if(BorderEffect == BorderEffect.None)
				return;
			if(activeState)
				SetFloatingContainerBorderColor(BorderEffectActiveColor, needLayoutUpdate);
			else
				SetFloatingContainerBorderColor(BorderEffectInactiveColor, needLayoutUpdate);
		}
		void ResetFloatingContainerBorderEffectColor(SolidColorBrush color) {
			if(color == null) return;
			SetFloatingContainerBorderColor(color, false);
		}
		void SetFloatingContainerBorderColor(SolidColorBrush borderColorBrush, bool needLayoutUpdate) {
			if(needLayoutUpdate)
				UpdateLayout();
			Border border = GetVisualByName("FloatingContainerBorder") as Border;
			if(border != null) {
				border.BorderBrush = borderColorBrush;
			}
		}
		public static readonly DependencyProperty ResizeBorderThicknessProperty =
			DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(DXWindow), new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnResizeBorderThicknessPropertyChanged)));
		public static readonly DependencyProperty ResizeBorderThicknessInAeroModeProperty =
			DependencyProperty.Register("ResizeBorderThicknessInAeroMode", typeof(Thickness), typeof(DXWindow), new FrameworkPropertyMetadata(new Thickness(), new PropertyChangedCallback(OnResizeBorderThicknessInAeroModePropertyChanged)));
		protected static void OnResizeBorderThicknessInAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnResizeBorderThicknessInAeroModeChanged((Thickness)e.OldValue);
		}
		protected static void OnResizeBorderThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is DXWindow) ((DXWindow)d).OnResizeBorderThicknessChanged((Thickness)e.OldValue);
		}
		protected virtual void OnResizeBorderThicknessChanged(Thickness oldValue) {
			UpdateActualResizeBorderThickness();
		}
		protected virtual void OnResizeBorderThicknessInAeroModeChanged(Thickness oldValue) {
			UpdateActualResizeBorderThickness();
		}
		protected virtual void UpdateActualResizeBorderThickness() {
			ActualResizeBorderThickness = IsAeroMode ? ResizeBorderThicknessInAeroMode : ResizeBorderThickness;
		}
		protected virtual void OnActualResizeBorderThicknessChanged(Thickness oldValue) {
		}
		public Thickness ActualResizeBorderThickness {
			get { return actualResizeBorderThicknessCore; }
			private set {
				if(actualResizeBorderThicknessCore == value) return;
				Thickness oldValue = actualResizeBorderThicknessCore;
				actualResizeBorderThicknessCore = value;
				OnActualResizeBorderThicknessChanged(oldValue);
			}
		}
		Thickness actualResizeBorderThicknessCore = new Thickness();
		bool allowChangeRenderModeCore = true;
		public bool AllowChangeRenderMode { get { return allowChangeRenderModeCore; } set { allowChangeRenderModeCore = value; } }
		public DXWindow() {
			UpdateIsAeroModeEnabled();
			Loaded += DXWindow_Loaded;
		}
#if DEBUGTEST
		bool enabledAeroModeForWin8;
		public bool EnabledAeroModeForWin8 {
			get {
				return enabledAeroModeForWin8;
			}
			set {
				enabledAeroModeForWin8 = value;
				isAeroModeSupported = true;
				UpdateIsAeroModeEnabled();
			}
		}
#endif
		protected WindowInteropHelper interopHelperCore;
		protected Color colorizationColorCore = SystemColors.ControlColor;
		public enum ButtonParts {
			PART_CloseButton,
			PART_Minimize,
			PART_Restore,
			PART_Maximize
		}
		public enum OtherParts {
			PART_DragWidget,
			PART_StatusPanel,
			PART_Icon,
			PART_Text,
			PART_Header
		}
		public enum SysMenuItems {
			Restore = 0,
			Move = 1,
			Size = 2,
			Minimize = 3,
			Maximize = 4,
			Close = 5
		}
		bool enableTransparencyCore = false;
		WindowStyle originalWindowStyle;
		protected WindowStyle GetWindowStyleCore() {
			return EnableTransparency ? originalWindowStyle : WindowStyle;
		}
		public bool EnableTransparency {
			get { return enableTransparencyCore; }
			set {
				if(interopHelperCore != null) throw new ApplicationException("Can not change the EnableTransparency property after window handle created");
				enableTransparencyCore = value;
			}
		}
		public bool IsDraggingOrResizing {
			get { return (bool)GetValue(IsDraggingOrResizingProperty); }
			set { SetValue(IsDraggingOrResizingProperty, value); }
		}
		private static bool isAeroModeSupported = true;
		private bool isAeroModeEnabledCore = false;
		protected bool IsAeroModeEnabled {
			get { return isAeroModeEnabledCore; }
			set {
				if(isAeroModeEnabledCore == value || !isAeroModeSupported)
					return;
				bool oldValue = isAeroModeEnabledCore;
				isAeroModeEnabledCore = value;
				OnIsAeroModeEnabledChanged(oldValue);
			}
		}
		protected virtual void OnIsAeroModeEnabledChanged(bool oldValue) {
			CoerceValue(IsAeroModeProperty);
		}
		protected void UpdateIsAeroModeEnabled() {
			if(!isAeroModeSupported)
				return;
			IsAeroModeEnabled = DwmIsCompositionEnabled();
		}
		Button closeButton;
		static Matrix DeviceToDIPTransform = Matrix.Identity;
		static Matrix DIPToDeviceTransform = Matrix.Identity;
		static void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
			var element = sender as UIElement;
			if(element == null) return;
			var window = LayoutHelper.FindRoot(element) as DXWindow;
			if(window != null && !window.allowProcessContextMenu) e.Handled = true;
		}
		public static bool UpdateWindowRegionOnSourceInitialized = false;
		protected Grid PartFloatPaneBorders { get; private set; }
		protected FrameworkElement FloatingContainerHeader { get; private set; }
		protected Grid PartRootGrid { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartRootGrid = GetTemplateChild("Root_Grid") as Grid;
			PartFloatPaneBorders = GetTemplateChild("FloatPaneBorders") as Grid;
		}
		void ILogicalOwner.AddChild(object obj) {
			var dObj = obj as DependencyObject;
			if(dObj != null && LogicalTreeHelper.GetParent(dObj) == null)
				AddLogicalChild(obj);
		}
		void ILogicalOwner.RemoveChild(object obj) {
			RemoveLogicalChild(obj);
		}
		private static void OnResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var window = d as DXWindow;
			window.SetElementsVisilbility();
		}
		private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var window = d as DXWindow;
			window.PatchIcon(e.NewValue);
		}
		private static void OnWindowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var window = d as DXWindow;
			window.DoWindowStyleChangedUpdate((WindowStyle)e.NewValue);
		}
		protected void DoResetVisibility() {
			ResetElementVisibility(OtherParts.PART_Header.ToString());
			ResetElementVisibility(ButtonParts.PART_Maximize.ToString());
			ResetElementVisibility(ButtonParts.PART_Minimize.ToString());
			ResetElementVisibility(ButtonParts.PART_Restore.ToString());
		}
		protected void ResizeWindow(ActiveResizeParts resizeMode) {
			ResizeWindow(interopHelperCore.Handle, resizeMode);
		}
		static bool CheckSupportAeroMode() {
			if(Environment.OSVersion.Version.Major < 6) {
				return false;
			}
			try {
				DwmIsCompositionEnabled();
			} catch(DllNotFoundException) {
				return false;
			}
			return true;
		}
		[SecuritySafeCritical]
		static bool DwmIsCompositionEnabled() {
			return NativeMethods.DwmIsCompositionEnabled();
		}
		protected virtual void ResetFloatingContainerBodyMargin() {
			var element = GetVisualByName("FloatingContainerBody") as Border;
			if(element != null) element.SetValue(FrameworkElement.MarginProperty, DependencyProperty.UnsetValue);
			if(element != null) element.SetValue(Border.PaddingProperty, DependencyProperty.UnsetValue);
			element = GetVisualByName("FloatingContainerBorder") as Border;
			if(element != null) element.SetValue(Border.PaddingProperty, DependencyProperty.UnsetValue);
		}
		protected virtual void UpdateFloatingContainerBodyMargin(bool hideCompletly) {
			UpdateFloatingContainerBodyMargin(this, hideCompletly);
		}
		protected static void UpdateFloatingContainerBodyMargin(DependencyObject root, bool hideCompletly) {
			var element = GetVisualByName(root, "FloatingContainerBody") as Border;
			if(element != null) {
				element.Margin = new Thickness(hideCompletly ? 0 : element.Margin.Left);
				element.Padding = new Thickness(element.Padding.Left);
				Border innerborder = LayoutHelper.FindElementByName(element, "border") as Border;
				if(innerborder != null) {
					innerborder.Margin = new Thickness(innerborder.Margin.Left);
					innerborder.Padding = new Thickness(innerborder.Padding.Left);
				}
			}
			var elementCB = GetVisualByName(root, "FloatingContainerBodyBorder") as Border;
			if(elementCB != null) elementCB.Margin = new Thickness(elementCB.Margin.Left);
			if(hideCompletly) {
				if(element != null) element.Padding = new Thickness(0);
				element = GetVisualByName(root, "FloatingContainerBorder") as Border;
				if(element != null) element.Padding = new Thickness(0);
			}
		}
		protected void DoWindowStyleChangedUpdate(WindowStyle newStyle) {
			DoResetVisibility();
			SetElementsVisilbility();
			switch(newStyle) {
				case WindowStyle.None:
				SetElementVisibility(OtherParts.PART_Header.ToString(), Visibility.Collapsed);
				UpdateNoResizeAndWSNone();
				break;
				case WindowStyle.SingleBorderWindow:
				case WindowStyle.ThreeDBorderWindow:
				break;
				case WindowStyle.ToolWindow:
				SetElementVisibility(OtherParts.PART_Icon.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Maximize.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Minimize.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Restore.ToString(), Visibility.Collapsed);
				break;
			}
		}
		private static void OnWindowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
#if !DXWINDOW
			DXWindow window = d as DXWindow;
			if(window == null) return;
			FloatingContainerThemeKeyExtension fctke = new FloatingContainerThemeKeyExtension();
			fctke.ResourceKey = FloatingContainerThemeKey.FloatingContainerBackground;
			fctke.IsThemeIndependent = true;
			if(window.Resources.Contains(fctke))
				window.Resources.Remove(fctke);
			if(e.NewValue != null) {
				window.Resources.Add(fctke, e.NewValue);
			}
#endif
		}
		private static void OnUseLayoutRoundingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var window = d as DXWindow;
			if(window != null) {
				foreach(Window temp in window.OwnedWindows) {
					temp.SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, window.UseLayoutRounding);
				}
			}
		}
		private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var window = d as DXWindow;
			if(window != null && window.HwndSourceWindow == null) {
				if(window.EnableTransparency) {
					window.originalWindowStyle = window.WindowStyle;
					window.WindowStyle = System.Windows.WindowStyle.None;
					window.AllowsTransparency = true;
				}
				return;
			}
			if((Visibility)e.NewValue == Visibility.Visible) window.DXWindow_Loaded(d, null);
		}
		private static void OnWindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var window = d as DXWindow;
			if(!IsTabletMode()) {
				if(window != null && window.HwndSourceWindow == null) return;
				if(window.Visibility == Visibility.Hidden) return;
				bool isMaximized = (WindowState)e.NewValue == WindowState.Maximized;
				if(window.ResizeMode == ResizeMode.CanResize || window.ResizeMode == ResizeMode.CanResizeWithGrip) DXWindow.SetIsMaximized(d, isMaximized);
				if(window != null) {
					window.UpdateRootMargins(isMaximized);
					window.UpdateFloatPaneBorders(isMaximized);
					if(window.interopHelperCore != null)
						window.previousWindowState = (WindowState)e.NewValue;
				}
			}
			window.ProcessDWM();
		}
		protected void UpdateFloatPaneBorders(bool maximized) {
			if(PartFloatPaneBorders == null) return;
			PartFloatPaneBorders.Visibility = maximized ? Visibility.Hidden : Visibility.Visible;
		}
		protected void UpdateRootMargins(bool maximized) {
			if(PartRootGrid == null) return;
			if(maximized)
				PartRootGrid.SetValue(Grid.MarginProperty, CalcRootMargins());
			else
				PartRootGrid.ClearValue(Grid.MarginProperty);
		}
		protected Thickness CalcRootMargins() {
			Thickness result = GetScreenMargins(WindowRect);
			if(WindowStyle == System.Windows.WindowStyle.None) result = SetTicknessWithoutTaskBar(result);
			if(ResizeMode == System.Windows.ResizeMode.NoResize) result = new Thickness(0);
			var maxWidthDefaultValue = (double)MaxWidthProperty.GetMetadata(typeof(FrameworkElement)).DefaultValue;
			var maxHeightDefaultValue = (double)MaxHeightProperty.GetMetadata(typeof(FrameworkElement)).DefaultValue;
			if(MaxWidth != maxWidthDefaultValue) result.Right = 0;
			if(MaxHeight != maxHeightDefaultValue) result.Bottom = 0;
			return result;
		}
		private Thickness SetTicknessWithoutTaskBar(Thickness tickness) {
			if(tickness.Bottom > tickness.Top) tickness.Bottom = tickness.Top;
			if(tickness.Right > tickness.Left) tickness.Right = tickness.Left;
			if(tickness.Top > tickness.Bottom) tickness.Top = tickness.Bottom;
			if(tickness.Left > tickness.Right) tickness.Left = tickness.Right;
			return tickness;
		}
		protected virtual void OnSmallIconChanged(ImageSource oldValue) {
		}
		protected virtual void OnShowTitlePropertyChanged(bool oldValue) {
			AttachToText();
		}
		protected virtual void OnAeroBorderSizeChanged(double oldValue) {
		}
		protected static bool IsUncPath(string s) {
			if(String.IsNullOrEmpty(s)) return true;
			return s.StartsWith(@"\\");
		}
		bool ActiveState {
			get { return (bool)this.GetValue(IsActiveExProperty); }
			set {
				if(decorator != null) {
					decorator.ActiveStateChanged(value);
					RedrawBorderWithEffectColor(value, false);
				}
			}
		}
		protected virtual void OnIsActiveChanged(bool newValue) {
			ActiveState = newValue;
		}
		[ThreadStatic]
		static ImageSource imageSource = null;
		[SecuritySafeCritical]
		private void CheckSystemIcon() {
			if(!SetAppIcon(interopHelperCore.Handle)) SetDefaultIcon();
		}
		[SecuritySafeCritical]
		protected int GetAppIconCore(IntPtr hwnd, int iconType, int iconHandle) {
			if(iconHandle == 0) return Win32.SendMessage(hwnd, NativeMethods.WM_GETICON, iconType, IntPtr.Zero);
			else return iconHandle;
		}
		[SecuritySafeCritical]
		protected bool SetAppIcon(IntPtr hwnd) {
			if(SmallIcon != null) return true;
			int iconHandle = 0;
			iconHandle = GetAppIconCore(hwnd, NativeMethods.ICON_SMALL2, iconHandle);
			iconHandle = GetAppIconCore(hwnd, NativeMethods.ICON_SMALL, iconHandle);
			iconHandle = GetAppIconCore(hwnd, NativeMethods.ICON_BIG, iconHandle);
			if(iconHandle == 0) return false;
			var icn = System.Drawing.Icon.FromHandle(new IntPtr(iconHandle));
			if(icn == null) return false;
			var imageSource = Imaging.CreateBitmapSourceFromHIcon(
									  icn.Handle,
									  Int32Rect.Empty,
									  BitmapSizeOptions.FromEmptyOptions());
			PatchIcon(imageSource);
			return true;
		}
		[SecuritySafeCritical]
		private void SetDefaultIcon() {
			if(Icon == null) {
				IntPtr handle = IntPtr.Zero;
				if(imageSource == null) {
					try {
						IntPtr moduleHandle = NativeMethods.GetModuleHandle("user32.dll");
						handle = NativeMethods.LoadImage(moduleHandle, new IntPtr(100), 1, 16, 16, 0x8000);
						imageSource = Imaging.CreateBitmapSourceFromHIcon(
										handle,
										Int32Rect.Empty,
										BitmapSizeOptions.FromEmptyOptions());
						PatchIcon(imageSource);
					} catch { } finally {
						if(handle != IntPtr.Zero) NativeMethods.DeleteObject(handle);
					}
				}
				if(imageSource != null) PatchIcon(imageSource);
			}
		}
		void DXWindow_Loaded(object sender, RoutedEventArgs e) {
			if(NeedReAttachToVisualTree) AttachToVisualTree();
			RedrawBorderWithEffectColor(this.ActiveState, false);
			UpdateRootMargins(WindowState == System.Windows.WindowState.Maximized);
			this.DelayedExecute(() => {
				if(NeedReAttachToVisualTree) AttachToVisualTree();
				if(WindowState != WindowState.Normal) {
					OnWindowStateChanged(this, new DependencyPropertyChangedEventArgs(Window.WindowStateProperty, WindowState.Normal, WindowState));
					if(NeedReAttachToVisualTree) AttachToVisualTree();
				}
				if(ResizeMode == System.Windows.ResizeMode.NoResize) {
					ResizeMode = System.Windows.ResizeMode.CanResize;
					UpdateLayout();
					ResizeMode = System.Windows.ResizeMode.NoResize;
				}
			});
		}
		protected object GetField(string fieldName) {
			var type = typeof(Window);
			return GetField(fieldName, type);
		}
		protected object GetField(string fieldName, Type type) {
			var fi = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(fi != null) return fi.GetValue(this);
			return null;
		}
		protected void PatchField(object newVal, string fieldName) {
			var type = typeof(Window);
			PatchField(newVal, fieldName, type, this);
		}
		protected void PatchField(object newVal, string fieldName, Type type, object instance) {
			var fi = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(fi != null) fi.SetValue(instance, newVal);
			else {
#if DEBUG
				throw new NullReferenceException("field patch failed");
#endif
			}
		}
		protected void Set_updateHwndSize(bool newVal) {
			PatchField(newVal, "_updateHwndSize");
		}
		protected void Set_updateHwndLocation(bool newVal) {
			PatchField(newVal, "_updateHwndLocation");
		}
		protected HwndSource HwndSourceWindow {
			get {
				var type = typeof(Window);
				var fi = type.GetProperty("HwndSourceWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if(fi != null)
					return (HwndSource)fi.GetValue(this, new object[] { });
				else return null;
			}
		}
		protected virtual void UpdateActualWindowTemplate() {
			ActualWindowTemplate = IsAeroMode ? AeroWindowTemplate : WindowTemplate;
		}
		WindowState previousWindowState;
		bool partFloatingContainerHeadersWasSet = false;
		[SecuritySafeCritical]
		private bool WmSizeChanged(IntPtr wParam) {
			WindowStateProcessing(wParam);
			var rect = new NativeMethods.RECT(0, 0, 0, 0);
			NativeMethods.IntGetClientRect(new HandleRef(this, interopHelperCore.Handle), ref rect);
			if(WindowState == System.Windows.WindowState.Minimized && Owner != null && FloatingContainerHeader != null) {
				FloatingContainerHeader.Width = rect.right;
				FloatingContainerHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				partFloatingContainerHeadersWasSet = true;
				SetElementVisibility(ButtonParts.PART_Minimize.ToString(), System.Windows.Visibility.Hidden);
				SetElementVisibility(ButtonParts.PART_Restore.ToString(), System.Windows.Visibility.Visible);
			} else {
				if(partFloatingContainerHeadersWasSet && FloatingContainerHeader != null) {
					FloatingContainerHeader.ClearValue(FrameworkElement.WidthProperty);
					FloatingContainerHeader.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
					ResetElementVisibility(ButtonParts.PART_Minimize.ToString());
					ResetElementVisibility(ButtonParts.PART_Restore.ToString());
					partFloatingContainerHeadersWasSet = false;
				}
			}
			PatchWindowRegion(rect.right, rect.bottom);
			return false;
		}
		private void WindowStateProcessing(IntPtr wParam) {
			switch(((int)wParam)) {
				case 0:
				if(previousWindowState != WindowState.Normal) {
					if(WindowState != WindowState.Normal) {
						WindowState = WindowState.Normal;
					}
					previousWindowState = WindowState.Normal;
#if NET35
						SetElementsVisilbility();
#endif
				}
				break;
				case 1:
				if(previousWindowState != WindowState.Minimized) {
					if(WindowState != WindowState.Minimized) {
						WindowState = WindowState.Minimized;
					}
					previousWindowState = WindowState.Minimized;
				}
				break;
				case 2:
				if(previousWindowState != WindowState.Maximized) {
					if(WindowState != WindowState.Maximized) {
						WindowState = WindowState.Maximized;
					}
					previousWindowState = WindowState.Maximized;
#if NET35
						SetElementVisibility_NET35("FloatPaneBorders", Visibility.Collapsed);
#endif
				}
				break;
			}
		}
		int trueLeft, trueTop;
		[SecurityCritical]
		private bool WmMoveChanged(IntPtr hwnd, IntPtr lParam) {
			WmMoveChangedImpl(hwnd, lParam);
			InvalidateVisual();
			return false;
		}
		[SecuritySafeCritical]
		private void WmMoveChangedImpl(IntPtr hwnd, IntPtr lParam) {
			var rect = new NativeMethods.RECT(0, 0, 0, 0);
			NativeMethods.IntGetClientRect(new HandleRef(this, hwnd), ref rect);
			trueLeft = NativeMethods.SignedLOWORD(lParam);
			trueTop = NativeMethods.SignedHIWORD(lParam);
			var logicalTopLeft = ScreenToLogicalPixels(new Point(trueLeft, trueTop));
			PatchField(logicalTopLeft.X, "_actualLeft");
			PatchField(logicalTopLeft.Y, "_actualTop");
			PatchWindowRegion(rect.right, rect.bottom);
			int right = trueLeft + rect.right;
			int bottom = trueTop + rect.bottom;
			rect = new NativeMethods.RECT(trueLeft, trueTop, right, bottom);
			var t = typeof(Window);
			if(WindowState == System.Windows.WindowState.Minimized && CanPatchLeftTop()) PatchLeftTop();
			var mi = t.GetMethod("WmMoveChangedHelper", BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi != null) {
				mi.Invoke(this, new object[] { });
			}
		}
		int magicNumber = -32000;
		private bool CanPatchLeftTop() {
			return trueLeft != magicNumber && trueTop != magicNumber;
		}
		protected Point ScreenToLogicalPixels(Point point) {
			var target = GetHwndTarget();
			if(target != null) {
				var matrix = target.TransformFromDevice;
				return matrix.Transform(point);
			}
			return point;
		}
		protected Point LogicalPixelsToScreen(Point point) {
			var target = GetHwndTarget();
			if(target != null) {
				var matrix = target.TransformToDevice;
				return matrix.Transform(point);
			}
			return point;
		}
		void PatchLeftTop() {
			try {
				var logicalTopLeft = ScreenToLogicalPixels(new Point(trueLeft, trueTop));
				Set_updateHwndLocation(false);
				SetValue(LeftProperty, logicalTopLeft.X);
				SetValue(TopProperty, logicalTopLeft.Y);
			} finally {
				Set_updateHwndLocation(true);
			}
		}
		private Size headerSizeCore = new Size();
		public Size HeaderSize {
			get { return headerSizeCore; }
			set {
				if(headerSizeCore == value)
					return;
				Size oldValue = headerSizeCore;
				headerSizeCore = value;
				OnHeaderSizeChanged(oldValue);
			}
		}
		protected virtual void OnHeaderSizeChanged(Size oldValue) {
		}
		protected virtual Thickness GetAeroBorderThickness() {
			double headerHeight = 0;
			double sideSize = -1;
			if(HeaderSize.Height == 0)
				headerHeight = AeroBorderSize;
			else
				headerHeight = HeaderSize.Height;
			return new Thickness(sideSize, headerHeight, sideSize, sideSize);
		}
		[SecuritySafeCritical]
		protected HwndTarget GetHwndTarget() {
			PresentationSource source = PresentationSource.FromVisual(this);
			if(source != null) return source.CompositionTarget as HwndTarget;
			return null;
		}
		[SecuritySafeCritical]
		private static IntPtr CallDefWindowProcWithoutVisibleStyle(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			bool flag = UtilityMethods.ModifyStyle(hWnd, 0x10000000, 0);
			var ptr = NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
			if(flag) {
				UtilityMethods.ModifyStyle(hWnd, 0, 0x10000000);
			}
			handled = true;
			return ptr;
		}
		bool isWindowSizing = false;
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			if(isWindowSizing) e.Handled = true;
			base.OnPreviewMouseMove(e);
		}
		private static NativeMethods.MONITORINFOEX MonitorInfoFromWindow(IntPtr hWnd) {
			IntPtr hMonitor = NativeMethods.MonitorFromWindow(hWnd, 2);
			NativeMethods.MONITORINFOEX result = new NativeMethods.MONITORINFOEX();
			result.cbSize = (int)Marshal.SizeOf(typeof(NativeMethods.MONITORINFOEX));
			NativeMethods.GetMonitorInfo(new HandleRef(null, hMonitor), result);
			return result;
		}
		private NativeMethods.WINDOWINFO GetWindowInfo(IntPtr hWnd) {
			NativeMethods.WINDOWINFO wINDOWINFO = new NativeMethods.WINDOWINFO();
			wINDOWINFO.cbSize = Marshal.SizeOf(wINDOWINFO);
			NativeMethods.GetWindowInfo(hWnd, ref wINDOWINFO);
			return wINDOWINFO;
		}
		private static NativeMethods.RECT GetClientRectRelativeToWindowRect(IntPtr hWnd) {
			NativeMethods.RECT rECT;
			NativeMethods.GetWindowRect(hWnd, out rECT);
			NativeMethods.RECT result;
			NativeMethods.GetClientRect(hWnd, out result);
			NativeMethods.POINT pOINT = new NativeMethods.POINT { x = 0, y = 0 };
			NativeMethods.ClientToScreen(hWnd, ref pOINT);
			result.Offset(pOINT.x - rECT.left, pOINT.y - rECT.top);
			return result;
		}
		protected void ChangeRenderMode(RenderMode newMode) {
			if(AllowChangeRenderMode) {
				HwndTarget target = GetHwndTarget();
				if(target != null) target.RenderMode = newMode;
			}
		}
		bool isProcessingDefWndProc;
		[SecuritySafeCritical]
		protected virtual IntPtr HwndSourceHookHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if(msg == NativeMethods.WM_NCPAINT) {
				if(IsAeroMode) {
					Rect controlBoxSize = GetControlBoxRect();
					Rect wndRect = GetWindowRect();
					SetAeroControlBoxHeight(this, controlBoxSize.Height);
					SetAeroControlBoxWidth(this, controlBoxSize.Width);
					SetAeroControlBoxMargin(this, new Thickness(0, 0, wndRect.Right - controlBoxSize.Right, 0));
					return IntPtr.Zero;
				}
				handled = true;
				return IntPtr.Zero;
			}
			if(msg == NativeMethods.WM_NCUAHDRAWCAPTION) {
				handled = true;
				return IntPtr.Zero;
			}
			if(msg == NativeMethods.WM_NCCALCSIZE) {
				if(IsAeroMode) {
					var rcClientArea = (NativeMethods.RECT)Marshal.PtrToStructure(lParam, typeof(NativeMethods.RECT));
					rcClientArea.bottom += 1;
					Marshal.StructureToPtr(rcClientArea, lParam, false);
					handled = true;
					return new IntPtr((int)0x0100 | 0x0200);
				}
				Rect windowRect = WindowRect;
				bool isOnPrimaryScreen = ScreenHelper.IsOnPrimaryScreen(new Point(windowRect.CenterX(), windowRect.CenterY()));
				if(wParam == new IntPtr(1) && WindowState == System.Windows.WindowState.Maximized && isOnPrimaryScreen) {
					NativeMethods.NCCALCSIZE_PARAMS csp = (NativeMethods.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));
					int edge;
					if(TaskBarHelper.IsAutoHide(out edge, interopHelperCore.Handle)) {
						if(edge == (int)TaskBarHelper.ABEdge.ABE_BOTTOM) csp.rgrc0.bottom -= 10;
						if(edge == (int)TaskBarHelper.ABEdge.ABE_RIGHT) csp.rgrc0.right -= 10;
						if(edge == (int)TaskBarHelper.ABEdge.ABE_TOP) csp.rgrc0.bottom -= 10;
						if(edge == (int)TaskBarHelper.ABEdge.ABE_LEFT) csp.rgrc0.right -= 10;
					}
					Marshal.StructureToPtr(csp, lParam, false);
				}
				handled = true;
				return IntPtr.Zero;
			}
			if(msg == NativeMethods.WM_NCACTIVATE) {
				if(IsAeroMode) {
					IntPtr lRet = NativeMethods.DefWindowProc(hwnd, NativeMethods.WM_NCACTIVATE, wParam, new IntPtr(-1));
					handled = true;
					return lRet;
				} else
					SetIsActiveEx(this, (int)wParam > 0);
			}
			if(msg == NativeMethods.WM_NCHITTEST) {
				if(IsAeroMode)
					return DoNonClientHitTest(hwnd, msg, wParam, lParam, ref handled);
				else {
					handled = true;
					return DoNCHitTest(lParam);
				}
			}
			if(interopHelperCore == null)
				return IntPtr.Zero;
			if(hwnd != interopHelperCore.Handle)
				return IntPtr.Zero;
			if(msg == NativeMethods.WM_ACTIVATE) {
				InvalidateWindowCaption();
				return IntPtr.Zero;
			}
			if(msg == NativeMethods.WM_NCACTIVATE || msg == NativeMethods.WM_SETTEXT || msg == NativeMethods.WM_SETICON) {
				if(isProcessingDefWndProc)
					return IntPtr.Zero;
				InvalidateWindowCaption();
				isProcessingDefWndProc = true;
				var result = IntPtr.Zero;
				result = CallDefWindowProcWithoutVisibleStyle(hwnd, msg, wParam, lParam, ref handled);
				isProcessingDefWndProc = false;
				return result;
			}
			if(msg == NativeMethods.WM_MOVE) {
				WmMoveChanged(interopHelperCore.Handle, lParam);
				handled = true;
				return IntPtr.Zero;
			}
			if(msg == NativeMethods.WM_SIZE) {
				WmSizeChanged(wParam);
			}
			if(msg == NativeMethods.WM_NCRBUTTONDOWN || msg == NativeMethods.WM_NCRBUTTONUP || msg == NativeMethods.WM_NCRBUTTONDBLCLK) {
				handled = true;
				return IntPtr.Zero;
			}
			HwndTarget target = GetHwndTarget();
			if(msg == NativeMethods.WM_ENTERSIZEMOVE) {
				IsDraggingOrResizing = true;
				if(target != null) {
					if(!isDragMove) { ChangeRenderMode(RenderMode.SoftwareOnly); isWindowSizing = true; } else PatchField(true, "_windowPosChanging", typeof(HwndTarget), target);
				}
			}
			if(msg == NativeMethods.WM_EXITSIZEMOVE) {
				if(target != null) {
					if(!isDragMove) { ChangeRenderMode(RenderMode.Default); isWindowSizing = false; } else PatchField(false, "_windowPosChanging", typeof(HwndTarget), target);
				}
				InvalidateVisual();
				IsDraggingOrResizing = false; isDragMove = false;
			}
			if(msg == NativeMethods.WM_CAPTURECHANGED && isWindowSizing) {
				if(target != null) {
					if(!isDragMove) {
						ChangeRenderMode(RenderMode.Default); isWindowSizing = false; InvalidateVisual();
					}
				}
				IsDraggingOrResizing = false; isDragMove = false;
			}
			if(msg == NativeMethods.WM_RBUTTONUP) {
				if(!allowProcessContextMenu) {
					handled = true;
					return IntPtr.Zero;
				}
			}
			if(msg == NativeMethods.WM_DWMCOMPOSITIONCHANGED) {
				UpdateIsAeroModeEnabled();
				if(IsAeroMode) {
					ProcessDWM();
					UpdateLayout();
					handled = true;
					return IntPtr.Zero;
				}
			}
			return IntPtr.Zero;
		}
		[SecuritySafeCritical]
		protected void ProcessDWM() {
			if(IsAeroMode) {
				NativeMethods.SetWindowRgn(interopHelperCore.Handle, IntPtr.Zero, false);
				if(HwndSourceWindow == null)
					return;
				HwndSourceWindow.CompositionTarget.BackgroundColor = Colors.Transparent;
				Thickness aeroBorderThickness = GetAeroBorderThickness();
				if(WindowState == System.Windows.WindowState.Maximized) {
					Point topOffset = DIPToDeviceTransform.Transform(new Point(0, GetWindowRect().Top));
					aeroBorderThickness.Top -= topOffset.Y;
				}
				Point topLeft = DIPToDeviceTransform.Transform(new Point(aeroBorderThickness.Left, aeroBorderThickness.Top));
				Point bottomRight = DIPToDeviceTransform.Transform(new Point(aeroBorderThickness.Right, aeroBorderThickness.Bottom));
				NativeMethods.MARGIN aeroMargin = new NativeMethods.MARGIN() { left = (int)topLeft.X, right = (int)bottomRight.X, top = (int)topLeft.Y, bottom = (int)bottomRight.Y };
				NativeMethods.DwmExtendFrameIntoClientArea(interopHelperCore.Handle, ref aeroMargin);
				NativeMethods.SetWindowPosOptions setWindowPosOptions = NativeMethods.SetWindowPosOptions.FRAMECHANGED | NativeMethods.SetWindowPosOptions.NOSIZE | NativeMethods.SetWindowPosOptions.NOMOVE | NativeMethods.SetWindowPosOptions.NOZORDER | NativeMethods.SetWindowPosOptions.NOOWNERZORDER | NativeMethods.SetWindowPosOptions.NOACTIVATE;
				NativeMethods.SetWindowPos(interopHelperCore.Handle, IntPtr.Zero, 0, 0, 0, 0, setWindowPosOptions);
			}
		}
		[SecuritySafeCritical]
		public static bool IsTabletMode() {
			return NativeMethods.GetSystemMetrics(0x56) != 0 && NativeMethods.GetSystemMetrics(0x2003) == 0;
		}
		[SecuritySafeCritical]
		IntPtr DoNonClientHitTest(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			IntPtr res = new IntPtr(0);
			handled = NativeMethods.DwmDefWindowProc(hwnd, msg, wParam, lParam, ref res);
			if(res == IntPtr.Zero) {
				handled = true;
				return DoNCHitTest(lParam);
			}
			return res;
		}
		IntPtr DoNCHitTest(IntPtr lParam) {
			int result = NativeMethods.HTCLIENT;
			if(WindowState == WindowState.Maximized)
				return new IntPtr((int)NativeMethods.HTCLIENT);
			Point position = new Point(NativeMethods.SignedLOWORD(lParam), NativeMethods.SignedHIWORD(lParam));
			Point topLeft = DIPToDeviceTransform.Transform(new Point(Left, Top));
			Point transformedSize = DIPToDeviceTransform.Transform(new Point(ActualWidth, ActualHeight));
			Point rigthBottom = new Point(topLeft.X + transformedSize.X, topLeft.Y + transformedSize.Y);
			Rect windowBounds = new Rect(topLeft.X, topLeft.Y, rigthBottom.X - topLeft.X, rigthBottom.Y - topLeft.Y);
			Point leftTopThickness = DIPToDeviceTransform.Transform(new Point(ActualResizeBorderThickness.Left, ActualResizeBorderThickness.Top));
			Point rightBottomThickness = DIPToDeviceTransform.Transform(new Point(ActualResizeBorderThickness.Right, ActualResizeBorderThickness.Bottom));
			Thickness scaledResizeBorderThickness = new Thickness(leftTopThickness.X, leftTopThickness.Y, rightBottomThickness.X, rightBottomThickness.Y);
			if(position.X >= windowBounds.Left && position.X <= windowBounds.Left + scaledResizeBorderThickness.Left) {
				if(position.Y >= windowBounds.Top && position.Y < windowBounds.Top + scaledResizeBorderThickness.Top)
					result = NativeMethods.HTTOPLEFT;
				else if(position.Y < windowBounds.Top + windowBounds.Height && position.Y >= windowBounds.Top + windowBounds.Height - scaledResizeBorderThickness.Bottom)
					result = NativeMethods.HTBOTTOMLEFT;
				else result = NativeMethods.HTLEFT;
			} else if(position.X <= windowBounds.Left + windowBounds.Width && position.X >= windowBounds.Left + windowBounds.Width - scaledResizeBorderThickness.Right) {
				if(position.Y >= windowBounds.Top && position.Y < windowBounds.Top + scaledResizeBorderThickness.Top)
					result = NativeMethods.HTTOPRIGHT;
				else if(position.Y < windowBounds.Top + windowBounds.Height && position.Y >= windowBounds.Top + windowBounds.Height - scaledResizeBorderThickness.Bottom)
					result = NativeMethods.HTBOTTOMRIGHT;
				else result = NativeMethods.HTRIGHT;
			} else {
				if(position.Y >= windowBounds.Top && position.Y < windowBounds.Top + scaledResizeBorderThickness.Top)
					result = NativeMethods.HTTOP;
				else if(position.Y < windowBounds.Top + windowBounds.Height && position.Y >= windowBounds.Top + windowBounds.Height - scaledResizeBorderThickness.Bottom)
					result = NativeMethods.HTBOTTOM;
			}
			if(result == NativeMethods.HTTOP || result == NativeMethods.HTTOPLEFT) {
				Rect buttonsRect = Rect.Empty;
				foreach(string buttonName in Enum.GetNames(typeof(ButtonParts))) {
					FrameworkElement button = GetVisualByName(buttonName) as FrameworkElement;
					if(button != null && button.Visibility == System.Windows.Visibility.Visible) {
						buttonsRect = Rect.Union(buttonsRect, new Rect(button.TranslatePoint(new Point(topLeft.X, topLeft.Y), null), new Size(button.ActualWidth, button.ActualHeight)));
					}
				}
				if(buttonsRect.Contains(position)) result = NativeMethods.HTCLIENT;
			}
			return new IntPtr(result);
		}
		[SecuritySafeCritical]
		private void InvalidateWindowCaption() {
			InvalidateVisual();
			NativeMethods.RedrawWindow(
				interopHelperCore.Handle,
				IntPtr.Zero,
				IntPtr.Zero,
				(int)NativeMethods.DDW_INVALIDATE | (int)NativeMethods.DDW_FRAME | (int)NativeMethods.DDW_UPDATENOW);
		}
		bool stylePatched = true;
		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);
			if(interopHelperCore == null) interopHelperCore = new WindowInteropHelper(this);
			CheckSystemIcon();
			HwndSource.FromHwnd(interopHelperCore.Handle).AddHook(HwndSourceHookHandler);
			if(!stylePatched) {
				stylePatched = true;
				int style = NativeMethods.GetWindowLong(interopHelperCore.Handle, NativeMethods.GWL_STYLE);
				NativeMethods.SetWindowLong(
					interopHelperCore.Handle,
					NativeMethods.GWL_STYLE,
					style & ~NativeMethods.WS_CAPTION);
				stylePatched = true;
			}
			if(WindowState == System.Windows.WindowState.Normal || UpdateWindowRegionOnSourceInitialized)
				PatchWindowRegion((int)Width, (int)Height);
			InvalidateVisual();
			InvalidateMeasure();
			ProcessDWM();
#if !DXWINDOW
			ThemeManager.AddThemeChangedHandler(this, OnThemeChanged);
#endif
		}
		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			SetIsActiveEx(this, true);
		}
		protected override void OnDeactivated(EventArgs e) {
			InvalidateVisual();
			base.OnDeactivated(e);
			SetIsActiveEx(this, false);
		}
		[StructLayout(LayoutKind.Sequential)]
		protected struct WindowMinMax {
			internal double minWidth;
			internal double maxWidth;
			internal double minHeight;
			internal double maxHeight;
			internal WindowMinMax(double minSize, double maxSize) {
				minWidth = minSize;
				maxWidth = maxSize;
				minHeight = minSize;
				maxHeight = maxSize;
			}
		}
		protected WindowMinMax GetWindowMinMax() {
			var max = new WindowMinMax();
			var point = new Point((double)GetField("_trackMaxWidthDeviceUnits"), (double)GetField("_trackMaxHeightDeviceUnits"));
			var point2 = new Point((double)GetField("_trackMinWidthDeviceUnits"), (double)GetField("_trackMinHeightDeviceUnits"));
			max.minWidth = Math.Max(base.MinWidth, point2.X);
			if(base.MinWidth > base.MaxWidth) {
				max.maxWidth = Math.Min(base.MinWidth, point.X);
			} else if(!double.IsPositiveInfinity(base.MaxWidth)) {
				max.maxWidth = Math.Min(base.MaxWidth, point.X);
			} else {
				max.maxWidth = point.X;
			}
			max.minHeight = Math.Max(base.MinHeight, point2.Y);
			if(base.MinHeight > base.MaxHeight) {
				max.maxHeight = Math.Min(base.MinHeight, point.Y);
				return max;
			}
			if(!double.IsPositiveInfinity(base.MaxHeight)) {
				max.maxHeight = Math.Min(base.MaxHeight, point.Y);
				return max;
			}
			max.maxHeight = point.Y;
			Point resultMin = new Point(max.minWidth, max.minHeight);
			Point resultMax = new Point(max.maxWidth, max.maxHeight);
			resultMin = RoundPointToScreenPixels(resultMin);
			resultMax = RoundPointToScreenPixels(resultMax);
			max.maxHeight = resultMax.Y;
			max.maxWidth = resultMax.X;
			max.minHeight = resultMin.Y;
			max.minWidth = resultMin.X;
			return max;
		}
		double Round(double source) { return Math.Round(source + 0.5); }
		Point RoundPoint(Point source) { return new Point(Round(source.X), Round(source.Y)); }
		Point RoundPointToScreenPixels(Point source) {
			Point screenRes = LogicalPixelsToScreen(source);
			screenRes = RoundPoint(screenRes);
			Point logicalRes = ScreenToLogicalPixels(screenRes);
			return logicalRes;
		}
		protected virtual bool AllowOptimizeUpdateRootMargins() { return true; }
		private Size MeasureOverrideHelper(Size constraint) {
			if(interopHelperCore == null) interopHelperCore = new WindowInteropHelper(this);
			if(VisualChildrenCount > 0) {
				var visualChild = GetVisualChild(0) as UIElement;
				if(visualChild != null) {
					var hwndNonClientAreaSizeInMeasureUnits = new Size(0, 0);
					if(AllowOptimizeUpdateRootMargins() && !IsTabletMode()) UpdateRootMargins(WindowState == System.Windows.WindowState.Maximized);
					var availableSize = new Size { Width = (constraint.Width == double.PositiveInfinity) ? double.PositiveInfinity : Math.Max((double)0.0, (double)(constraint.Width - hwndNonClientAreaSizeInMeasureUnits.Width)), Height = (constraint.Height == double.PositiveInfinity) ? double.PositiveInfinity : Math.Max((double)0.0, (double)(constraint.Height - hwndNonClientAreaSizeInMeasureUnits.Height)) };
					visualChild.Measure(availableSize);
					var desiredSize = visualChild.DesiredSize;
					Point result = new Point(desiredSize.Width + hwndNonClientAreaSizeInMeasureUnits.Width, desiredSize.Height + hwndNonClientAreaSizeInMeasureUnits.Height);
					Point logicalRes = RoundPointToScreenPixels(result);
					return new Size(logicalRes.X, logicalRes.Y);
				}
			}
			return new Size(0, 0);
		}
		protected override Size MeasureOverride(Size availableSize) {
			var windowMinMax = GetWindowMinMax();
			Point windowMinInLogicalPoints = ScreenToLogicalPixels(new Point(windowMinMax.minWidth, windowMinMax.minHeight));
			Point windowMaxInLogicalPoints = ScreenToLogicalPixels(new Point(windowMinMax.maxWidth, windowMinMax.maxHeight));
			Size constraint = new Size();
			constraint.Width = Math.Max(windowMinInLogicalPoints.X, Math.Min(availableSize.Width, windowMaxInLogicalPoints.X));
			constraint.Height = Math.Max(windowMinInLogicalPoints.Y, Math.Min(availableSize.Height, windowMaxInLogicalPoints.Y));
			var size2 = MeasureOverrideHelper(constraint);
			var result = new Size(Math.Max(size2.Width, windowMinMax.minWidth), Math.Max(size2.Height, windowMinMax.minHeight));
			return result;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if(VisualChildrenCount > 0) {
				var fe = GetVisualChild(0) as FrameworkElement;
				if(null == fe) { return base.ArrangeOverride(arrangeBounds); }
				fe.Arrange(new Rect(arrangeBounds));
				if(base.FlowDirection == FlowDirection.RightToLeft)
					fe.LayoutTransform = new MatrixTransform(-1.0, 0.0, 0.0, 1.0, arrangeBounds.Width, 0.0);
				else
					fe.LayoutTransform = null;
			}
			return arrangeBounds;
		}
		protected double GetIconWidth() {
			return SystemParameters.SmallIconWidth;
		}
		protected double GetIconHeight() {
			return SystemParameters.SmallIconHeight;
		}
		protected void PatchIcon(object newVal) {
			var bf = newVal as BitmapFrame;
			if(bf == null) {
				var bmi = newVal as BitmapImage;
				if(bmi != null) {
					try {
						bf = BitmapFrame.Create(bmi.UriSource);
					} catch {
						var tbitmap = new TransformedBitmap(bmi, new ScaleTransform(GetIconWidth() / bmi.Width, GetIconHeight() / bmi.Height));
						SmallIcon = tbitmap;
						return;
					}
				}
			}
			var bfresult = (BitmapFrame)null;
			if(bf != null) {
				int bestMatch = Int32.MaxValue;
				if(bf != null) {
					foreach(var temp in bf.Decoder.Frames) {
						int currentMatch = Math.Abs((int)(GetIconWidth() - temp.Width)) +
								   Math.Abs((int)(GetIconWidth() - temp.Height));
						if(currentMatch < bestMatch) {
							bfresult = temp;
							bestMatch = currentMatch;
						}
					}
				}
				if(bfresult.Width > GetIconWidth()) {
					var tbitmap = new TransformedBitmap(bfresult, new ScaleTransform(GetIconWidth() / bfresult.Width, GetIconHeight() / bfresult.Height));
					SmallIcon = tbitmap;
					return;
				}
			}
			if(bfresult == null) SmallIcon = newVal as ImageSource;
			else SmallIcon = bfresult;
		}
		[SecuritySafeCritical]
		private static Rect GetScreenBounds(Rect boundingBox) {
			var rc = new NativeMethods.RECT(0, 0, 0, 0);
			var temp = new NativeMethods.RECT(boundingBox);
			var handle = NativeMethods.MonitorFromRect(ref temp, 2);
			if(handle != IntPtr.Zero) {
				var info = new NativeMethods.MONITORINFOEX();
				NativeMethods.GetMonitorInfo(new HandleRef(null, handle), info);
				rc = info.rcMonitor;
			}
			return new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
		}
		[SecuritySafeCritical]
		Thickness GetScreenMargins(Rect bounds) {
			var rect = new NativeMethods.RECT(bounds);
			var handle = NativeMethods.MonitorFromRect(ref rect, 2);
			if(handle != IntPtr.Zero) {
				var info = new NativeMethods.MONITORINFOEX();
				NativeMethods.GetMonitorInfo(new HandleRef(null, handle), info);
				rect = info.rcWork;
			}
			return PatchScreenMarginsByDpi(bounds, rect);
		}
		double GetDpiFactor() {
			if(interopHelperCore != null && interopHelperCore.Handle != IntPtr.Zero) {
				var source = PresentationSource.FromVisual(this);
				if(source != null) return source.CompositionTarget.TransformToDevice.M11;
			}
			using(var source = new HwndSource(new HwndSourceParameters())) {
				if(source != null) return source.CompositionTarget.TransformToDevice.M11;
			}
			return 1;
		}
		[SecuritySafeCritical]
		Thickness PatchScreenMarginsByDpi(Rect bounds, NativeMethods.RECT rect) {
			double dpiFactor = GetDpiFactor();
			int pacthedLeft = (int)Math.Ceiling((rect.left - bounds.Left) / dpiFactor);
			int pacthedTop = (int)Math.Ceiling((rect.top - bounds.Top) / dpiFactor);
			int pacthedRight = (int)Math.Ceiling((bounds.Right - rect.right) / dpiFactor);
			int pacthedBottom = (int)Math.Ceiling((bounds.Bottom - rect.bottom) / dpiFactor);
			return new Thickness(pacthedLeft, pacthedTop, pacthedRight, pacthedBottom);
		}
		IntPtr region = IntPtr.Zero;
		[SecuritySafeCritical]
		protected virtual void PatchWindowRegion(int width, int height) {
			if(IsAeroMode) return;
			if(interopHelperCore == null) return;
			if(WindowState == WindowState.Maximized) {
				if(IsWinSeven) {
					if(IsClassicTheme())
						PatchWindowRegionCore(width, height, 4);
					else
						if(!IsAeroModeEnabled) PatchWindowRegionCore(width, height, 8);
						else NativeMethods.SetWindowRgn(interopHelperCore.Handle, IntPtr.Zero, true);
				} else
					NativeMethods.SetWindowRgn(interopHelperCore.Handle, IntPtr.Zero, true);
			} else {
				PatchWindowRegionCore(width, height, 0);
			}
		}
		[SecuritySafeCritical]
		private void PatchWindowRegionCore(int width, int height, int happyNumber) {
			if(region != IntPtr.Zero) NativeMethods.DeleteObject(region);
			region = NativeMethods.CreateRectRgn(happyNumber, 0, width - happyNumber, height);
			NativeMethods.SetWindowRgn(interopHelperCore.Handle, region, true);
		}
		[SecuritySafeCritical]
		private void AdjustSystemMenu(IntPtr hmenu) {
			if(ResizeMode == ResizeMode.NoResize) {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf020, 1);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf030, 1);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf060, 0);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf120, 1);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf000, 1);
				InvalidateWindowCaption();
				return;
			}
			if(ResizeMode == ResizeMode.CanMinimize) {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf020, 0);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf030, 1);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf060, 0);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf120, 1);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf000, 1);
				InvalidateWindowCaption();
				return;
			}
			bool flag = (GetWindowStyleCore() == WindowStyle.ToolWindow) || (GetWindowStyleCore() != WindowStyle.None);
			bool flag2 = WindowState != WindowState.Minimized;
			bool flag3 = WindowState != WindowState.Maximized;
			bool flag5 = WindowState != WindowState.Normal;
			bool flag6 = (flag && (WindowState != WindowState.Minimized)) && (WindowState != WindowState.Maximized);
			if(!flag2) {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf020, 1);
			} else {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf020, 0);
			}
			if(!flag3) {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf030, 1);
			} else {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf030, 0);
			}
			NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf060, 0);
			if(!flag5) {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf120, 1);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xF010, 0);
			} else {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf120, 0);
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xF010, 1);
			}
			if(!flag6) {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf000, 1);
			} else {
				NativeMethods.EnableMenuItem(new HandleRef(this, hmenu), 0xf000, 0);
			}
			InvalidateWindowCaption();
		}
		[SecuritySafeCritical]
		private IntPtr GetMenuHandle() {
			var menuHandle = NativeMethods.GetSystemMenu(interopHelperCore.Handle, false);
			AdjustSystemMenu(menuHandle);
			return menuHandle;
		}
		protected internal object GetVisualByName(string name) {
			return DXWindow.GetVisualByName(this, name);
		}
		static internal object GetVisualByName(DependencyObject root, string name) {
			using(var enumerator = new VisualTreeEnumeratorWithConditionalStop(root, DXWindowTreeStop)) {
				while(enumerator.MoveNext()) {
					var dObj = enumerator.Current;
					if((string)dObj.GetValue(FrameworkElement.NameProperty) == name)
						return dObj;
				}
			}
			return null;
		}
#if !DXWINDOW
		public Rect ClientArea {
			get {
				return GetClientArea();
			}
		}
		protected virtual Rect GetClientArea() {
			FrameworkElement pcc = LayoutHelper.FindElementByName(this, "PART_ContainerContent") as FrameworkElement;
			if(pcc != null) {
				var rect = pcc.TransformToVisual(this).TransformBounds(LayoutInformation.GetLayoutSlot(pcc));
				if(WindowState == WindowState.Maximized) {
					double left = rect.Left - ActualResizeBorderThickness.Left;
					double top = rect.Top - ActualResizeBorderThickness.Top;
					rect = new Rect(new Point(left, top), rect.Size);
				}
				return rect;
			} else
				return Rect.Empty;
		}
#endif
		static bool DXWindowTreeStop(DependencyObject child) {
			return (string)child.GetValue(FrameworkElement.NameProperty) == "PART_ContainerContent";
		}
		protected virtual bool NeedReAttachToVisualTree {
			get {
				return closeButton != GetVisualByName(ButtonParts.PART_CloseButton.ToString());
			}
		}
		protected void AttachToVisualTree() {
			WindowResizeHelper.Subscribe(this);
			FloatingContainerHeader = GetVisualByName(OtherParts.PART_Header.ToString()) as FrameworkElement;
			var element = GetVisualByName(OtherParts.PART_DragWidget.ToString()) as FrameworkElement;
			if(element != null) element.PreviewMouseDown += DragWindowOrMaximizeWindow;
			foreach(string buttonName in Enum.GetNames(typeof(ButtonParts))) {
				var button = GetVisualByName(buttonName) as Button;
				if(ButtonParts.PART_CloseButton == (ButtonParts)Enum.Parse(typeof(ButtonParts), buttonName)) {
					if(closeButton != null) closeButton.Click -= ButtonPartClick;
					closeButton = button;
					if(closeButton == null) return;
					closeButton.IsVisibleChanged += closeButton_IsVisibleChanged;
				}
				if(button != null) {
					button.Click += ButtonPartClick;
				}
			}
			DoResetVisibility();
			AttachToIcon();
			AttachToText();
			DoWindowStyleChangedUpdate(GetWindowStyleCore());
			UpdateRootMargins(GetIsMaximized(this));
			UpdateFloatPaneBorders(GetIsMaximized(this));
			if(Owner != null) SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, Owner.UseLayoutRounding);
		}
		void closeButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			this.DelayedExecuteEnqueue(() => {
				if(NeedReAttachToVisualTree) AttachToVisualTree();
			});
		}
		protected virtual void OnIsAeroModeChanged(bool oldValue) {
			UpdateActualWindowTemplate();
			UpdateActualResizeBorderThickness();
			UpdateLayout();
		}
		protected virtual bool CoerceIsAeroModeValue(bool baseValue) {
			return IsAeroModeEnabled && baseValue;
		}
		private void SetElementsVisilbility() {
			switch(ResizeMode) {
				case ResizeMode.CanMinimize:
				SetElementVisibility(OtherParts.PART_StatusPanel.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Maximize.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Restore.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Minimize.ToString(), Visibility.Visible);
				SetElementVisibility(ButtonParts.PART_CloseButton.ToString(), Visibility.Visible);
#if NET35
					SetElementVisibility_NET35("FloatPaneBorders", Visibility.Collapsed);
#else
				SetElementVisibility("FloatPaneBorders", Visibility.Collapsed);
#endif
				break;
				case ResizeMode.CanResize:
				case ResizeMode.CanResizeWithGrip:
				SetElementVisibility(OtherParts.PART_StatusPanel.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Minimize.ToString(), Visibility.Visible);
				SetElementVisibility(ButtonParts.PART_CloseButton.ToString(), Visibility.Visible);
				if(WindowState == System.Windows.WindowState.Normal) {
					SetElementVisibility(ButtonParts.PART_Maximize.ToString(), Visibility.Visible);
					SetElementVisibility(ButtonParts.PART_Restore.ToString(), Visibility.Collapsed);
				}
				if(WindowState == System.Windows.WindowState.Maximized) {
					SetElementVisibility(ButtonParts.PART_Restore.ToString(), Visibility.Visible);
					SetElementVisibility(ButtonParts.PART_Maximize.ToString(), Visibility.Collapsed);
				}
#if NET35
					SetElementVisibility_NET35("FloatPaneBorders", Visibility.Visible);
#else
				SetElementVisibility("FloatPaneBorders", Visibility.Visible);
#endif
				if(ResizeMode == System.Windows.ResizeMode.CanResize) break;
				SetElementVisibility(OtherParts.PART_StatusPanel.ToString(), Visibility.Visible);
				SetElementVisibility(ButtonParts.PART_Minimize.ToString(), Visibility.Visible);
				SetElementVisibility(ButtonParts.PART_CloseButton.ToString(), Visibility.Visible);
#if NET35
					SetElementVisibility_NET35("FloatPaneBorders", Visibility.Visible);
#else
				SetElementVisibility("FloatPaneBorders", Visibility.Visible);
#endif
				break;
				case ResizeMode.NoResize:
				SetElementVisibility(OtherParts.PART_StatusPanel.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Maximize.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Restore.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_Minimize.ToString(), Visibility.Collapsed);
				SetElementVisibility(ButtonParts.PART_CloseButton.ToString(), Visibility.Visible);
#if NET35
					SetElementVisibility_NET35("FloatPaneBorders", Visibility.Collapsed);
#else
				SetElementVisibility("FloatPaneBorders", Visibility.Collapsed);
				UpdateNoResizeAndWSNone();
#endif
				break;
			}
		}
		protected virtual void UpdateNoResizeAndWSNone() {
			ResetFloatingContainerBodyMargin();
			if(WindowStyle == System.Windows.WindowStyle.None) {
				UpdateFloatingContainerBodyMargin(ResizeMode == System.Windows.ResizeMode.NoResize);
			}
		}
		protected void SetElementVisibility(string elementName, Visibility visibility) {
			var element = GetVisualByName(elementName) as FrameworkElement;
			if(element != null) element.SetCurrentValue(FrameworkElement.VisibilityProperty, visibility);
		}
#if NET35
		protected void SetElementVisibility_NET35(string elementName, Visibility visibility) {
			var element = GetVisualByName(elementName) as FrameworkElement;
			if(element != null) element.SetValue(FrameworkElement.VisibilityProperty, visibility);
		}
#endif
		protected void ResetElementVisibility(string elementName) {
			var element = GetVisualByName(elementName) as FrameworkElement;
			if(element != null) element.SetValue(FrameworkElement.VisibilityProperty, DependencyProperty.UnsetValue);
		}
		void AttachToText() {
			var caption = GetVisualByName(OtherParts.PART_Text.ToString()) as TextBlock;
			if(caption != null) {
				var binding = new Binding() { Path = new PropertyPath(TitleProperty), Source = this };
				caption.SetBinding(TextBlock.TextProperty, binding);
				caption.Visibility = GetShowTitle(this) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
			}
		}
		[SecuritySafeCritical]
		void AttachToIcon() {
			var iconImage = GetVisualByName(OtherParts.PART_Icon.ToString()) as Image;
			if(iconImage != null) {
				var imgBinding = new Binding() { Path = new PropertyPath(SmallIconProperty), Source = this };
				var imgVisibleBinding = new Binding() { Path = new PropertyPath(ShowIconProperty), Source = this, Converter = new DevExpress.Mvvm.UI.BooleanToVisibilityConverter() { Inverse = false } , Mode = BindingMode.TwoWay};
				iconImage.SetBinding(UIElement.VisibilityProperty, imgVisibleBinding);
				iconImage.SetBinding(Image.SourceProperty, imgBinding);
				double dF = GetDpiFactor();
				iconImage.Width = GetIconWidth() * dF;
				iconImage.Height = GetIconHeight() * dF;
				iconImage.SnapsToDevicePixels = true;
				iconImage.Stretch = Stretch.None;
				iconImage.IsHitTestVisible = true;
				iconImage.PreviewMouseDown += IconMouseProcessing;
			}
		}
		void IconMouseProcessing(object sender, MouseButtonEventArgs e) {
			if(e.ClickCount == 2 && e.ChangedButton == MouseButton.Left) Close();
			else if(e.ClickCount == 1 &&
				((e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed) ||
				(e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Pressed))
				) {
				var pos = Point.Add(e.GetPosition(this), new Vector(trueLeft + 5, trueTop + 5));
				ShowSystemMenuOnIconClick(pos);
			}
		}		
		protected Point GetTruePos() {
			return new Point(trueLeft, trueTop);
		}
		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			if(!AllowsTransparency && !IsAeroMode)
				drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1), new Rect(0, 0, Width, Height));
		}
		[SecuritySafeCritical]
		protected override void OnClosing(CancelEventArgs e) {
			if(region != IntPtr.Zero) NativeMethods.DeleteObject(region);
#if !DXWINDOW
			ThemeManager.RemoveThemeChangedHandler(this, OnThemeChanged);
#endif
			base.OnClosing(e);
			if(!e.Cancel && BorderEffect != BorderEffect.None) HideBorderEffect();
		}
		protected virtual void OnClose() {
			Close();
		}
		protected virtual void OnMinimize() {
			WindowState = WindowState.Minimized;
		}
		protected virtual void OnRestore() {
			DXWindow.SetIsMaximized(this, false);
			WindowState = WindowState.Normal;
		}
		protected virtual void OnMaximize() {
			SizeToContent = System.Windows.SizeToContent.Manual;
			DXWindow.SetIsMaximized(this, true);
			WindowState = WindowState.Maximized;
		}
		void ButtonPartClick(object sender, RoutedEventArgs e) {
			var button = sender as Button;
			if(button != null) {
				switch((ButtonParts)Enum.Parse(typeof(ButtonParts), button.Name)) {
					case ButtonParts.PART_CloseButton: OnClose(); break;
					case ButtonParts.PART_Minimize: OnMinimize(); break;
					case ButtonParts.PART_Restore: OnRestore(); break;
					case ButtonParts.PART_Maximize: OnMaximize(); break;
				}
			}
		}
		protected virtual bool CanResizeOrMaximize { get { return ResizeMode != System.Windows.ResizeMode.NoResize && ResizeMode != System.Windows.ResizeMode.CanMinimize; } }
		protected internal void DragWindowOrMaximizeWindow(object sender, MouseButtonEventArgs e) {
			if(e.Handled) return;
			if(!(CanResizeOrMaximize && e.ClickCount == 2) && e.LeftButton == MouseButtonState.Pressed) {
				DragMoveInternal(interopHelperCore.Handle);
			}
			if(dragWindowOrMaximizeWindowAsyncInvoked) return;
			if(e.RightButton == MouseButtonState.Pressed) e.Handled = true;
			Dispatcher.BeginInvoke(
					new Action<MouseButtonEventArgs>(DragWindowOrMaximizeWindowAsync), new object[] { e }
				);
			dragWindowOrMaximizeWindowAsyncInvoked = true;
		}
		bool isDragMove = false;
		[SecuritySafeCritical]
		void DragMoveInternal(IntPtr handle) {
			if(isWindowSizing) return;
			if(WindowState == WindowState.Normal || (WindowState == WindowState.Maximized && IsWinSeven)) {
				isDragMove = true;
				Win32.SendMessage(handle, 0x112, 0xf012, IntPtr.Zero);
				Win32.SendMessage(handle, 0x202, 0, IntPtr.Zero);
			}
		}
		static bool IsWinSeven {
			get {
				var version = Environment.OSVersion.Version;
				return (version.Major >= 6) && (version.Minor >= 1);
			}
		}
		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode)]
		static extern int GetCurrentThemeName([MarshalAs(UnmanagedType.LPWStr)]StringBuilder pszThemeFileName, Int32 dwMaxNameChars, IntPtr pszColorBuff, Int32 cchMaxColorChars, IntPtr pszSizeBuff, Int32 cchMaxSizeChars);
		const int MAX_PATH = 260;
		[SecuritySafeCritical]
		bool IsClassicTheme() {
			StringBuilder name = new StringBuilder(MAX_PATH, MAX_PATH);
			return GetCurrentThemeName(name, MAX_PATH, IntPtr.Zero, 0, IntPtr.Zero, 0) < 0;
		}
		bool dragWindowOrMaximizeWindowAsyncInvoked;
		protected void DragWindowOrMaximizeWindowAsync(MouseButtonEventArgs e) {
			try {
				if(e.RightButton == MouseButtonState.Pressed) {
					var pos = Point.Add(e.GetPosition(this), new Vector(trueLeft, trueTop));
					ShowSystemMenuOnRightClick(pos);
					return;
				}
				if (CanResizeOrMaximize && e.ClickCount == 2) {
					SizeToContent = System.Windows.SizeToContent.Manual;
					WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
				}
			} finally {
				dragWindowOrMaximizeWindowAsyncInvoked = false;
			}
		}		
		void ActivePartMouseDown(object sender, MouseButtonEventArgs e) {
			if(ResizeMode != ResizeMode.NoResize && ResizeMode != ResizeMode.CanMinimize && WindowState == System.Windows.WindowState.Normal) {
				var fe = sender as FrameworkElement;
				if(fe != null) {
					string name = fe.Name.Substring(fe.Name.LastIndexOf("_") + 1);
					var activePart = (ActiveResizeParts)Enum.Parse(typeof(ActiveResizeParts), name);
					activePart = WindowResizeHelper.CorrectResizePart(FlowDirection, activePart);
					ResizeWindow(interopHelperCore.Handle, activePart);
				}
			}
		}
		protected internal Rect WindowRect {
			get { return GetWindowRect(); }
		}
		protected internal Rect WindowRectInLogicalUnits {
			get {
				var rect = WindowRect;
				var logicalLocation = ScreenToLogicalPixels(rect.Location);
				var logicalSize = ScreenToLogicalPixels(new Point(rect.Width, rect.Height));
				return new Rect(logicalLocation, new Size(logicalSize.X, logicalSize.Y));
			}
		}
		[SecuritySafeCritical]
		private Rect GetWindowRect() {
			var rect = new NativeMethods.RECT();
			if(interopHelperCore != null)
				NativeMethods.GetWindowRect(new HandleRef(this, interopHelperCore.Handle), ref rect);
			return new Rect(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
		}
		[SecuritySafeCritical]
		private static void ResizeWindow(IntPtr handle, ActiveResizeParts sizingAction) {
			if(Mouse.LeftButton == MouseButtonState.Pressed) {
				Win32.SendMessage(handle, Win32.WM_SYSCOMMAND, (int)(Win32.SC_SIZE + sizingAction), IntPtr.Zero);
				Win32.SendMessage(handle, 514, 0, IntPtr.Zero);
			}
		}
		[SecuritySafeCritical]
		private static Rect GetControlBoxRect(IntPtr handle) {
			NativeMethods.TITLEBARINFOEX titleBarInfoEx = new NativeMethods.TITLEBARINFOEX();
			titleBarInfoEx.cbSize = Marshal.SizeOf(typeof(NativeMethods.TITLEBARINFOEX));
			NativeMethods.SendMessage(handle, NativeMethods.WM_GETTITLEBARINFOEX, IntPtr.Zero, ref titleBarInfoEx);
			Rect res = new Rect();
			for(int i = (int)NativeMethods.TitleBarElements.MinimizeButton; i < NativeMethods.TITLEBARINFOEX.CCHILDREN_TITLEBAR + 1; i++) {
				if(titleBarInfoEx.rgrect[i].right - titleBarInfoEx.rgrect[i].left != 0) {
					res.X = res.X == 0 ? titleBarInfoEx.rgrect[i].left : Math.Min(titleBarInfoEx.rgrect[i].left, res.X);
					res.Y = res.Y == 0 ? titleBarInfoEx.rgrect[i].top : Math.Min(titleBarInfoEx.rgrect[i].top, res.Y);
					res.Width = Math.Max(titleBarInfoEx.rgrect[i].right - res.X, res.Width);
					res.Height = Math.Max(titleBarInfoEx.rgrect[i].bottom - res.Y, res.Height);
				}
			}
			return res;
		}
		public Rect GetControlBoxRect() {
			if(interopHelperCore != null) {
				return GetControlBoxRect(interopHelperCore.Handle);
			}
			return new Rect();
		}
		bool allowProcessContextMenu = true;
		protected bool ShowSystemMenu(Point point) {
			return ShowSystemMenu(interopHelperCore.Handle, point);
		}
		public static readonly DependencyProperty AllowSystemMenuProperty = DependencyProperty.Register("AllowSystemMenu", typeof(bool), typeof(DXWindow), new PropertyMetadata(true));
		public bool AllowSystemMenu {
			get { return (bool)GetValue(AllowSystemMenuProperty); }
			set { SetValue(AllowSystemMenuProperty, value); }
		}
		DateTime lastSystemMenuShownTime = DateTime.Now;
		protected virtual void ShowSystemMenuOnRightClick(Point pos) {
			ShowSystemMenu(interopHelperCore.Handle, pos);
		}
		protected virtual void ShowSystemMenuOnIconClick(Point pos) {
			ShowSystemMenu(interopHelperCore.Handle, pos);
		}
		[SecuritySafeCritical]
		protected bool ShowSystemMenu(IntPtr hwnd, Point pt) {
			if(!AllowSystemMenu) return false;
			const int TPM_LEFTALIGN = 0x0000, TPM_TOPALIGN = 0x0000, TPM_RETURNCMD = 0x0100, WM_SYSCOMMAND = 0x0112;
			if(hwnd == IntPtr.Zero)
				return false;
			if((DateTime.Now - lastSystemMenuShownTime).TotalMilliseconds < 100) return false;
			IntPtr command;
			try {
				allowProcessContextMenu = false;
				command = NativeMethods.TrackPopupMenu(GetMenuHandle(), TPM_RETURNCMD | TPM_TOPALIGN | TPM_LEFTALIGN,
					   (int)pt.X, (int)pt.Y, 0, hwnd, IntPtr.Zero);
			} finally {
				lastSystemMenuShownTime = DateTime.Now;
				allowProcessContextMenu = true;
			}
			Win32.SendMessage(hwnd, WM_SYSCOMMAND, command.ToInt32(), IntPtr.Zero);
			return true;
		}
		private class ColorTranslator {
			public static Color FromArgb(int argb) {
				return Color.FromArgb(GetA(argb), GetR(argb), GetG(argb), GetB(argb));
			}
			static byte GetA(int argb) {
				return (byte)((argb >> 0x18) & 0xffL);
			}
			static byte GetR(int argb) {
				return (byte)((argb >> 0x10) & 0xffL);
			}
			static byte GetG(int argb) {
				return (byte)((argb >> 8) & 0xffL);
			}
			static byte GetB(int argb) {
				return (byte)(argb & 0xffL);
			}
		}
		#region IWindowResizeHelperClient Members
		FrameworkElement IWindowResizeHelperClient.GetVisualByName(string name) {
			return GetVisualByName(name) as FrameworkElement;
		}
		void IWindowResizeHelperClient.ActivePartMouseDown(object sender, MouseButtonEventArgs e) {
			ActivePartMouseDown(sender, e);
		}
		#endregion
		EventHandlerList events;
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		internal void OnHeaderSizeChanged(DXWindowHeader windowHeader, Size newSize, Size oldSize) {
			HeaderSize = newSize;
			if(newSize.Height != oldSize.Height) {
				ProcessDWM();
				UpdateLayout();
			}
		}
		Decorator decorator;
		BorderEffect borderEffect = BorderEffect.None;
		[DefaultValue(BorderEffect.None)]
		public BorderEffect BorderEffect {
			get { return borderEffect; }
			set {
				if(BorderEffect == value)
					return;
				borderEffect = value;
				OnAllowBorderEffectChanged();
			}
		}
		void HideBorderEffect() {
			if(decorator != null) {
				decorator.Hide();
				ReleaseBorderEffect();
			}
		}
		public void BorderEffectReset() {
			ClearValue(BorderEffectActiveColorProperty);
			ClearValue(BorderEffectInactiveColorProperty);
			RedrawBorderWithEffectColor(this.ActiveState, false);
		}
		protected virtual void OnAllowBorderEffectChanged() {
			if(BorderEffect != BorderEffect.None)
				InitBorderEffect();
			else
				ReleaseBorderEffect();
		}
		void InitBorderEffect() {
			if(decorator == null) {
				decorator = CreateFormHandleDecorator();
				decorator.Control = this;
				RedrawBorderWithEffectColor(this.ActiveState, true);
			}
		}
		void ReleaseBorderEffect() {
			if(decorator == null)
				return;
			decorator.Dispose();
			decorator = null;
		}
		Decorator CreateFormHandleDecorator() {
			StructDecoratorMargins structBorderEffectMargins = new StructDecoratorMargins() {
				LeftMargins = BorderEffectLeftMargins,
				RightMargins = BorderEffectRightMargins,
				TopMargins = BorderEffectTopMargins,
				BottomMargins = BorderEffectBottomMargins
			};
			return new FormHandleDecorator(BorderEffectActiveColor, BorderEffectInactiveColor, BorderEffectOffset, structBorderEffectMargins, this.ActiveState);
		}
#if !DXWINDOW
		void OnThemeChanged(object sender, EventArgs e) {
			this.DelayedExecute(() => { if(NeedReAttachToVisualTree)  AttachToVisualTree(); });
			if(BorderEffect == BorderEffect.None) return;
			BorderEffect = BorderEffect.None;
			BorderEffect = BorderEffect.Default;
		}
#endif
	}
	public class NativeMethods {
		public const int WM_MOUSEHWHEEL = 0x020E;
		public const int WM_NCCALCSIZE = 0x83;
		public const int WM_NCPAINT = 0x85;
		public const int WM_NCUAHDRAWCAPTION = 0xAE;
		public const int WM_NCUAHDRAWFRAME = 0xAF;
		public const int WM_NCACTIVATE = 0x86;
		public const int WM_NCHITTEST = 0x84;
		public const int WM_ACTIVATE = 6;
		public const int WM_PAINT = 0x000F;
		public const int WM_RBUTTONUP = 0x205;
		public const int WM_SETICON = 0x0080;
		public const int WM_SETTEXT = 12;
		public const int GWL_STYLE = -16;
		public const int WM_SIZE = 5;
		public const int WM_ENTERSIZEMOVE = 0x231;
		public const int WM_EXITSIZEMOVE = 0x232;
		public const int WM_CAPTURECHANGED = 0x215;
		public const int WM_MOVE = 3;
		public const int WM_GETICON = 0x7f;
		public const int WM_NCRBUTTONDOWN = 0x00A4;
		public const int WM_NCRBUTTONUP = 0x00A5;
		public const int WM_NCRBUTTONDBLCLK = 0x00A6;
		public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
		public const int WM_WINDOWPOSCHANGING = 0x0046;
		public const int WM_WINDOWPOSCHANGED = 0x0047;
		public const int WS_CAPTION = 0xc00000;
		public const int DDW_FRAME = 0x400;
		public const int DDW_INVALIDATE = 1;
		public const int DDW_UPDATENOW = 0x100;
		public const int ICON_BIG = 1;
		public const int ICON_SMALL = 0;
		public const int ICON_SMALL2 = 2;
		public const int LOGPIXELSX = 88;
		public const int LOGPIXELSY = 90;
		public const int WM_GETTITLEBARINFOEX = 0x033F;
		public const int HTCLIENT = 1;
		public const int HTLEFT = 10;
		public const int HTRIGHT = 11;
		public const int HTTOP = 12;
		public const int HTTOPLEFT = 13;
		public const int HTTOPRIGHT = 14;
		public const int HTBOTTOM = 15;
		public const int HTBOTTOMLEFT = 16;
		public const int HTBOTTOMRIGHT = 17;
		public static int SignedLOWORD(IntPtr intPtr) {
			return SignedLOWORD(IntPtrToInt32(intPtr));
		}
		public static int SignedHIWORD(IntPtr intPtr) {
			return SignedHIWORD(IntPtrToInt32(intPtr));
		}
		public static int SignedLOWORD(int n) {
			return (short)(n & 0xffff);
		}
		public static int SignedHIWORD(int n) {
			return (short)((n >> 0x10) & 0xffff);
		}
		public static int IntPtrToInt32(IntPtr intPtr) {
			return (int)intPtr.ToInt64();
		}
		[DllImport("user32.dll")]
		public static extern int GetMessageTime();
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);
		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr hObject);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FillRect(IntPtr hDC, ref RECT rect, IntPtr hbrush);
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateSolidBrush(int colorref);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetSystemMetrics(int nIndex);
		[DllImport("User32.dll")]
		internal static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int dwFlags);
		[DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosOptions uFlags);
		[DllImport("dwmapi.dll")]
		public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGIN pMarInset);
		[DllImport("dwmapi.dll", PreserveSig = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DwmIsCompositionEnabled();
		[DllImport("dwmapi.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);
		[DllImport("user32.dll", EntryPoint = "SetWindowPlacement", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		public static extern bool SetWindowPlacement(HandleRef hWnd, [In] ref NativeMethods.WINDOWPLACEMENT placement);
		[DllImport("user32.dll", EntryPoint = "GetWindowPlacement", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		public static extern bool GetWindowPlacement(HandleRef hWnd, ref NativeMethods.WINDOWPLACEMENT placement);
		[DllImport("user32.dll", EntryPoint = "GetClientRect", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		public static extern bool IntGetClientRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool RedrawWindow(HandleRef hwnd, RECT rcUpdate, HandleRef hrgnUpdate, int flags);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool RedrawWindow(HandleRef hwnd, IntPtr rcUpdate, HandleRef hrgnUpdate, int flags);
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool RedrawWindow(IntPtr hWnd, IntPtr rectUpdate, IntPtr hRgnUpdate, int uFlags);
		[DllImport("user32.dll")]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
		[DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
		public static extern int GetMenuItemCount(IntPtr hmenu);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool EnableMenuItem(HandleRef hMenu, int UIDEnabledItem, int uEnable);
		[DllImport("user32.dll", EntryPoint = "RemoveMenu")]
		public static extern int RemoveMenu(IntPtr hmenu, int npos, int wflags);
		[DllImport("user32.dll", EntryPoint = "DrawMenuBar")]
		public static extern int DrawMenuBar(IntPtr hwnd);
		[DllImport("user32.dll")]
		public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
		[DllImport("gdi32")]
		public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
		[DllImport("user32.dll", EntryPoint = "GetWindowRect", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		public static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect);
		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hwnd);
		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, ref TITLEBARINFOEX lParam);
		[DllImport("user32.dll")]
		public static extern IntPtr TrackPopupMenu(IntPtr menuHandle, int uFlags, int x, int y, int nReserved, IntPtr hwnd, IntPtr par);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);
		public static bool AdjustWindowRectEx(ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle) {
			bool flag = IntAdjustWindowRectEx(ref lpRect, dwStyle, bMenu, dwExStyle);
			if(!flag) {
				throw new Win32Exception();
			}
			return flag;
		}
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", EntryPoint = "AdjustWindowRectEx", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		public static extern bool IntAdjustWindowRectEx(ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);
		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern IntPtr MonitorFromRect(ref NativeMethods.RECT rect, int flags);
		[DllImport("user32.dll", EntryPoint = "GetMonitorInfo", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] NativeMethods.MONITORINFOEX info);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetActiveWindow();
		[StructLayout(LayoutKind.Sequential)]
		public struct NCCALCSIZE_PARAMS {
			public RECT rgrc0, rgrc1, rgrc2;
			public IntPtr lppos;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct TITLEBARINFOEX {
			public const int CCHILDREN_TITLEBAR = 5;
			public int cbSize;
			public RECT rcTitleBar;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = CCHILDREN_TITLEBAR + 1)]
			public int[] rgstate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = CCHILDREN_TITLEBAR + 1)]
			public RECT[] rgrect;
		};
		public enum TitleBarElements {
			TitleBar = 0,
			Reserved = 1,
			MinimizeButton = 2,
			MaximizeButton = 3,
			HelpButton = 4,
			CloseButton = 5
		};
		[Flags]
		public enum TitleBarElementStates {
			Focusable = 0x00100000,
			Invisible = 0x00008000,
			Offscreen = 0x00010000,
			Unavailable = 0x00000001,
			Pressed = 0x00000008
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct MARGIN {
			public int left;
			public int right;
			public int top;
			public int bottom;
		};
		[Flags]
		public enum SetWindowPosOptions {
			ASYNCWINDOWPOS = 0x4000,
			DEFERERASE = 0x2000,
			DRAWFRAME = 0x0020,
			FRAMECHANGED = 0x0020,
			HIDEWINDOW = 0x0080,
			NOACTIVATE = 0x0010,
			NOCOPYBITS = 0x0100,
			NOMOVE = 0x0002,
			NOOWNERZORDER = 0x0200,
			NOREDRAW = 0x0008,
			NOREPOSITION = 0x0200,
			NOSENDCHANGING = 0x0400,
			NOSIZE = 0x0001,
			NOZORDER = 0x0004,
			SHOWWINDOW = 0x0040,
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public POINT(int x, int y) {
				this.x = x;
				this.y = y;
			}
			public int x;
			public int y;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWINFO {
			public int cbSize;
			public RECT rcWindow;
			public RECT rcClient;
			public int dwStyle;
			public int dwExStyle;
			public int dwWindowStatus;
			public int cxWindowBorders;
			public int cyWindowBorders;
			public short atomWindowType;
			public short wCreatorVersion;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int left;
			public int top;
			public int right;
			public int bottom;
			public RECT(int left, int top, int right, int bottom) {
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}
			public RECT(Rect rect) {
				left = (int)rect.Left;
				top = (int)rect.Top;
				right = (int)rect.Right;
				bottom = (int)rect.Bottom;
			}
			public void Offset(int dx, int dy) {
				left += dx;
				right += dx;
				top += dy;
				bottom += dy;
			}
			public int Height {
				get {
					return this.bottom - this.top;
				}
				set {
					this.bottom = this.top + value;
				}
			}
			public int Width {
				get {
					return this.right - this.left;
				}
				set {
					this.right = this.left + value;
				}
			}
		}
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[SecurityCritical]
		public static extern IntPtr LoadImage(IntPtr hinst, IntPtr lpszName, int uType, int cxDesired, int cyDesired, int fuLoad);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		[SecurityCritical]
		public static extern IntPtr GetModuleHandle(string lpModuleName);
		public enum GWL {
			EXSTYLE = -20,
			STYLE = -16
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPLACEMENT {
			public int length;
			public int flags;
			public int showCmd;
			public int ptMinPosition_x;
			public int ptMinPosition_y;
			public int ptMaxPosition_x;
			public int ptMaxPosition_y;
			public int rcNormalPosition_left;
			public int rcNormalPosition_top;
			public int rcNormalPosition_right;
			public int rcNormalPosition_bottom;
		}
		[DllImport("user32.dll")]
		internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
		public class MONITORINFOEX {
			internal int cbSize = Marshal.SizeOf(typeof(NativeMethods.MONITORINFOEX));
			public NativeMethods.RECT rcMonitor = new NativeMethods.RECT();
			public NativeMethods.RECT rcWork = new NativeMethods.RECT();
			internal int dwFlags;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
			internal char[] szDevice = new char[0x20];
		}
	}
	public interface IWindowResizeHelperClient {
		FrameworkElement GetVisualByName(string name);
		void ActivePartMouseDown(object sender, MouseButtonEventArgs e);
	}
	public class WindowResizeHelper {
		public static void Subscribe(IWindowResizeHelperClient client) {
#if DXWINDOW
			foreach(string elementName in Enum.GetNames(typeof(DXWindowActiveResizeParts))) {
#else
			foreach(string elementName in Enum.GetNames(typeof(DevExpress.Xpf.Core.DXWindowActiveResizeParts))) {
#endif
				string prefix = "Part_";
				if(elementName == "SizeGrip") prefix = "PART_";
				var fe = client.GetVisualByName(prefix + elementName) as FrameworkElement;
				if(fe != null) fe.PreviewMouseDown += client.ActivePartMouseDown;
			}
		}
		public static ActiveResizeParts CorrectResizePart(FlowDirection fd, ActiveResizeParts activePart) {
			if(fd == FlowDirection.RightToLeft) {
				var tempActivePart = activePart;
				switch(tempActivePart) {
					case ActiveResizeParts.Left: activePart = ActiveResizeParts.Right; break;
					case ActiveResizeParts.Right: activePart = ActiveResizeParts.Left; break;
					case ActiveResizeParts.TopLeft: activePart = ActiveResizeParts.TopRight; break;
					case ActiveResizeParts.BottomLeft: activePart = ActiveResizeParts.BottomRight; break;
					case ActiveResizeParts.BottomRight: activePart = ActiveResizeParts.BottomLeft; break;
					case ActiveResizeParts.TopRight: activePart = ActiveResizeParts.TopLeft; break;
				}
			}
			return activePart;
		}
	}
	internal static class UtilityMethods {
		[SecurityCritical]
		internal static bool ModifyStyle(IntPtr hWnd, int styleToRemove, int styleToAdd) {
			int windowLong = NativeMethods.GetWindowLong(hWnd, (int)NativeMethods.GWL.STYLE);
			int dwNewLong = (windowLong & ~styleToRemove) | styleToAdd;
			if(dwNewLong == windowLong) {
				return false;
			}
			NativeMethods.SetWindowLong(hWnd, (int)NativeMethods.GWL.STYLE, dwNewLong);
			return true;
		}
	}
	internal class TaskBarHelper {
		[SecurityCritical]
		public static bool IsAutoHide(out int edge, IntPtr handle) {
			edge = 0;
			APPBARDATA data = new APPBARDATA();
			data.cbSize = Marshal.SizeOf(data);
			data.hWnd = handle;
			int res = SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref data);
			if((res & ABS_AUTOHIDE) == 0) return false;
			for(int n = 0; n < 4; n++) {
				edge = n;
				data.uEdge = (uint)n;
				if(SHAppBarMessage((int)ABMsg.ABM_GETAUTOHIDEBAR, ref data) != 0) return true;
			}
			edge = (int)ABEdge.ABE_BOTTOM;
			return true;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct APPBARDATA {
			public int cbSize;
			public IntPtr hWnd;
			public uint uCallbackMessage;
			public uint uEdge;
			public NativeMethods.RECT rc;
			public IntPtr lParam;
		}
		internal enum ABMsg : int {
			ABM_NEW = 0,
			ABM_REMOVE,
			ABM_QUERYPOS,
			ABM_SETPOS,
			ABM_GETSTATE,
			ABM_GETTASKBARPOS,
			ABM_ACTIVATE,
			ABM_GETAUTOHIDEBAR,
			ABM_SETAUTOHIDEBAR,
			ABM_WINDOWPOSCHANGED,
			ABM_SETSTATE
		}
		internal enum ABEdge : int {
			ABE_LEFT = 0,
			ABE_TOP,
			ABE_RIGHT,
			ABE_BOTTOM
		}
		const int ABS_AUTOHIDE = 1;
		[DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
		static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);
	}
	public class DXWindowSmartCenteringPanel : Grid {
		DXWindow windowCore = null;
		DXWindow Window {
			get {
				if(windowCore == null)
					windowCore = LayoutHelper.FindParentObject<DXWindow>(this);
				return windowCore;
			}
		}
		public double LeftIndent {
			get {
				if(Window != null) return TranslatePoint(new Point(0, 0), Window).X;
				return 0;
			}
		}
		public double RightIndent {
			get {
				if(Window != null) return Window.ActualWidth - TranslatePoint(new Point(ActualWidth, 0), Window).X;
				return 0;
			}
		}
		protected UIElement CenteringChild { get { return Children.Count > 0 ? Children[0] : null; } }
		Size prevArrangeSize = Size.Empty;
		int watchdog = 100;
		protected override Size ArrangeOverride(Size arrangeSize) {
			try {
				if(CenteringChild != null && Window != null) {
					double desiredChildWidth = CenteringChild.DesiredSize.Width;
					double desiredChildHeight = CenteringChild.DesiredSize.Height;
					double idealHalf = (Window.ActualWidth - desiredChildWidth) / 2;
					double idealHeight = (arrangeSize.Height - desiredChildHeight) / 2;
					if(idealHalf <= 0 || (idealHalf - LeftIndent) <= 0 || (idealHalf - RightIndent) <= 0) {
						return base.ArrangeOverride(arrangeSize);
					} else {
						CenteringChild.Arrange(new Rect(idealHalf - LeftIndent, idealHeight < 0 ? 0 : idealHeight, desiredChildWidth, CenteringChild.DesiredSize.Height));
						return arrangeSize;
					}
				}
				return base.ArrangeOverride(arrangeSize);
			} finally {
				if(prevArrangeSize != arrangeSize || (Window == null || LeftIndent == 0 || RightIndent == 0 || CenteringChild == null) && watchdog-- > 0) InvalidateMeasure();
				prevArrangeSize = arrangeSize;
			}
		}
	}
	public class DXWindowHeader : Border {
		public static readonly DependencyProperty IsAeroModeEnabledProperty =
			DependencyProperty.Register("IsAeroModeEnabled", typeof(bool), typeof(DXWindowHeader), new FrameworkPropertyMetadata(true));
		public bool IsAeroModeEnabled {
			get { return (bool)GetValue(IsAeroModeEnabledProperty); }
			set { SetValue(IsAeroModeEnabledProperty, value); }
		}
		DXWindow windowCore = null;
		DXWindow Window {
			get {
				if(windowCore == null)
					windowCore = LayoutHelper.FindParentObject<DXWindow>(this);
				return windowCore;
			}
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			if(Window != null && IsAeroModeEnabled)
				Window.OnHeaderSizeChanged(this, sizeInfo.NewSize, sizeInfo.PreviousSize);
		}
	}
}
