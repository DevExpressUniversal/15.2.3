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
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Native {
	public enum TrimmingMode { None, Horizontal, Vertical }
	public class TrimContentPresenter : ContentPresenter {
		public static readonly DependencyProperty TrimmingModeProperty = DependencyProperty.Register("TrimmingMode", typeof(TrimmingMode), typeof(TrimContentPresenter),
			new FrameworkPropertyMetadata(TrimmingMode.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
		public static readonly DependencyProperty TrimmingSizeProperty = DependencyProperty.Register("TrimmingSize", typeof(double), typeof(TrimContentPresenter),
			new FrameworkPropertyMetadata(15d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
		static readonly DependencyPropertyKey IsTrimmedPropertyKey = DependencyProperty.RegisterReadOnly("IsTrimmed", typeof(bool), typeof(TrimContentPresenter),
			new PropertyMetadata(false, (d, e) => ((TrimContentPresenter)d).OnIsTrimmedChanged((bool)e.OldValue, (bool)e.NewValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsTrimmedProperty = IsTrimmedPropertyKey.DependencyProperty;
		public static readonly DependencyProperty DisableToolTipWhenFullyVisibleProperty = DependencyProperty.Register("DisableToolTipWhenFullyVisible", typeof(bool), typeof(TrimContentPresenter),
			new PropertyMetadata(false, (d, e) => ((TrimContentPresenter)d).OnDisableToolTipWhenFullyVisibleChanged((bool)e.OldValue, (bool)e.NewValue)));
		public static readonly DependencyProperty ToolTipOwnerProperty = DependencyProperty.Register("ToolTipOwner", typeof(FrameworkElement), typeof(TrimContentPresenter),
			new PropertyMetadata(null, (d, e) => ((TrimContentPresenter)d).OnToolTipOwnerChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue)));
		public TrimmingMode TrimmingMode { get { return (TrimmingMode)GetValue(TrimmingModeProperty); } set { SetValue(TrimmingModeProperty, value); } }
		public double TrimmingSize { get { return (double)GetValue(TrimmingSizeProperty); } set { SetValue(TrimmingSizeProperty, value); } }
		public bool IsTrimmed { get { return (bool)GetValue(IsTrimmedProperty); } private set { SetValue(IsTrimmedPropertyKey, value); } }
		public bool DisableToolTipWhenFullyVisible { get { return (bool)GetValue(DisableToolTipWhenFullyVisibleProperty); } set { SetValue(DisableToolTipWhenFullyVisibleProperty, value); } }
		public FrameworkElement ToolTipOwner { get { return (FrameworkElement)GetValue(ToolTipOwnerProperty); } set { SetValue(ToolTipOwnerProperty, value); } }
		protected SizeHelperBase OrientationHelper { get { return SizeHelperBase.GetDefineSizeHelper(TrimmingMode == TrimmingMode.Vertical ? Orientation.Vertical : Orientation.Horizontal); } }
		protected override Size MeasureOverride(Size avSize) {
			if(TrimmingMode == TrimmingMode.None) return base.MeasureOverride(avSize);
			UIElement child = VisualTreeHelper.GetChildrenCount(this) > 0 ? VisualTreeHelper.GetChild(this, 0) as UIElement : null;
			if(child == null) return new Size();
			Size sz = avSize; OrientationHelper.SetDefineSize(ref sz, double.PositiveInfinity);
			child.Measure(sz);
			Size res = child.DesiredSize;
			OrientationHelper.SetDefineSize(ref res, Math.Min(OrientationHelper.GetDefineSize(child.DesiredSize), OrientationHelper.GetDefineSize(avSize)));
			return res;
		}
		protected override Size ArrangeOverride(Size avSize) {
			UIElement child = VisualTreeHelper.GetChildrenCount(this) > 0 ? VisualTreeHelper.GetChild(this, 0) as UIElement : null;
			var res = base.ArrangeOverride(avSize);
			UpdateOpacityMask(child as FrameworkElement, res);
			return res;
		}
		protected virtual void UpdateOpacityMask(FrameworkElement child, Size actualSize) {
			if(child == null) return;
			LinearGradientBrush opacityBrush = null;
			try {
				if(TrimmingMode == TrimmingMode.None) return;
				double childWidth = OrientationHelper.GetDefineSize(child.DesiredSize);
				double actualWidth = OrientationHelper.GetDefineSize(actualSize);
				if(childWidth <= actualWidth) return;
				if(TrimmingMode == TrimmingMode.Horizontal) {
					if(child.HorizontalAlignment == HorizontalAlignment.Left || child.HorizontalAlignment == HorizontalAlignment.Stretch)
						opacityBrush = new LinearGradientBrush() { StartPoint = new Point(0, 0.5), EndPoint = new Point(1, 0.5) };
					else opacityBrush = new LinearGradientBrush() { StartPoint = new Point(1, 0.5), EndPoint = new Point(0, 0.5) };
				} else {
					if(child.VerticalAlignment == VerticalAlignment.Top || child.VerticalAlignment == VerticalAlignment.Stretch)
						opacityBrush = new LinearGradientBrush() { StartPoint = new Point(0.5, 0), EndPoint = new Point(0.5, 1) };
					else opacityBrush = new LinearGradientBrush() { StartPoint = new Point(0.5, 1), EndPoint = new Point(0.5, 0) };
				}
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, 0));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Black, (actualWidth - TrimmingSize) / childWidth));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, (actualWidth) / childWidth));
				opacityBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
			} finally {
				child.OpacityMask = opacityBrush;
				IsTrimmed = opacityBrush != null;
			}
		}
		void OnIsTrimmedChanged(bool oldValue, bool newValue) {
			if(ToolTipOwner != null && DisableToolTipWhenFullyVisible) ToolTipService.SetIsEnabled(ToolTipOwner, newValue);
		}
		void OnDisableToolTipWhenFullyVisibleChanged(bool oldValue, bool newValue) {
			OnIsTrimmedChanged(IsTrimmed, IsTrimmed);
			if(!newValue && ToolTipOwner != null) ToolTipService.SetIsEnabled(ToolTipOwner, true);
		}
		void OnToolTipOwnerChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			OnIsTrimmedChanged(IsTrimmed, IsTrimmed);
		}
	}
	public class TabViewInfo {
		public HeaderLocation HeaderLocation { get; private set; }
		HeaderOrientation headerOrientationCore;
		Orientation? headerOrientation;
		public Orientation HeaderOrientation {
			get {
				if(headerOrientation == null) {
					if(headerOrientationCore != Xpf.Core.HeaderOrientation.Default)
						headerOrientation = headerOrientationCore == Xpf.Core.HeaderOrientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
					else headerOrientation = Orientation;
				}
				return headerOrientation.Value;
			}
		}
		Orientation? orientation;
		public Orientation Orientation { get { return orientation == null ? (orientation = RotatableHelper.Convert(HeaderLocation)).Value : orientation.Value; } }
		public SizeHelperBase OrientationHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		public bool IsHeaderOrientationDifferent { get { return HeaderOrientation != Orientation; } }
		public bool IsHoldMode { get; private set; }
		public bool IsMultiLineMode { get; private set; }
		public Color Background { get; private set; }
		public Color Border { get; private set; }
		public TabViewInfo(DXTabControl tabControl) {
			orientation = null;
			headerOrientation = null;
			headerOrientationCore = tabControl.With(x => x.View as TabControlScrollView).Return(x => x.HeaderOrientation, () => Xpf.Core.HeaderOrientation.Default);
			HeaderLocation = tabControl.With(x => x.View).Return(x => x.HeaderLocation, () => HeaderLocation.Top);
			IsMultiLineMode = tabControl.With(x => x.View).With(x => x.MultiLineView) != null;
			IsHoldMode = tabControl.With(x => x.View).With(x => x.MultiLineView).Return(x => x.FixedHeaders, () => false);
			Background = tabControl.With(x => x.SelectedContainer).Return(x => x.BackgroundColor, () => Colors.Transparent);
			Border = tabControl.With(x => x.SelectedContainer).Return(x => x.BorderColor, () => Colors.Transparent);
		}
		public TabViewInfo(DXTabItem tabItem) : this(tabItem.Owner) {
			Background = tabItem.Return(x => x.BackgroundColor, () => Colors.Transparent);
			Border = tabItem.Return(x => x.BorderColor, () => Colors.Transparent);
		}
		public bool Equals(TabViewInfo value) {
			if(value == null) return false;
			return HeaderLocation == value.HeaderLocation && headerOrientationCore == value.headerOrientationCore 
				&& IsHoldMode == value.IsHoldMode && IsMultiLineMode == value.IsMultiLineMode 
				&& Background == value.Background && Border == value.Border;
		}
	}
	public enum GlyphControlMode { Left, Right, Up, Down, Plus, Close }
	public class GlyphControl : Control {
		public static readonly DependencyProperty GlyphControlModeProperty = DependencyProperty.RegisterAttached("GlyphControlMode", typeof(GlyphControlMode?), typeof(GlyphControl),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnGlyphControlModeChanged));
		public static GlyphControlMode? GetGlyphControlMode(DependencyObject obj) { return (GlyphControlMode?)obj.GetValue(GlyphControlModeProperty); }
		public static void SetGlyphControlMode(DependencyObject obj, GlyphControlMode? value) { obj.SetValue(GlyphControlModeProperty, value); }
		static void OnGlyphControlModeChanged(object sender, DependencyPropertyChangedEventArgs e) {
			(sender as GlyphControl).Do(x => x.UpdateTemplate());
		}
		public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(GlyphControl), new PropertyMetadata(0, (d, e) => ((GlyphControl)d).ApplySize()));
		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(GlyphControlMode), typeof(GlyphControl), new PropertyMetadata(GlyphControlMode.Close, (d, e) => ((GlyphControl)d).UpdateTemplate()));
		public static readonly DependencyProperty GlyphLeftTemplateProperty = DependencyProperty.Register("GlyphLeftTemplate", typeof(ControlTemplate), typeof(GlyphControl), new PropertyMetadata(null, (d, e) => ((GlyphControl)d).UpdateTemplate()));
		public static readonly DependencyProperty GlyphRightTemplateProperty = DependencyProperty.Register("GlyphRightTemplate", typeof(ControlTemplate), typeof(GlyphControl), new PropertyMetadata(null, (d, e) => ((GlyphControl)d).UpdateTemplate()));
		public static readonly DependencyProperty GlyphUpTemplateProperty = DependencyProperty.Register("GlyphUpTemplate", typeof(ControlTemplate), typeof(GlyphControl), new PropertyMetadata(null, (d, e) => ((GlyphControl)d).UpdateTemplate()));
		public static readonly DependencyProperty GlyphDownTemplateProperty = DependencyProperty.Register("GlyphDownTemplate", typeof(ControlTemplate), typeof(GlyphControl), new PropertyMetadata(null, (d, e) => ((GlyphControl)d).UpdateTemplate()));
		public static readonly DependencyProperty GlyphPlusTemplateProperty = DependencyProperty.Register("GlyphPlusTemplate", typeof(ControlTemplate), typeof(GlyphControl), new PropertyMetadata(null, (d, e) => ((GlyphControl)d).UpdateTemplate()));
		public static readonly DependencyProperty GlyphCloseTemplateProperty = DependencyProperty.Register("GlyphCloseTemplate", typeof(ControlTemplate), typeof(GlyphControl), new PropertyMetadata(null, (d, e) => ((GlyphControl)d).UpdateTemplate()));
		public static readonly DependencyProperty GlyphLeftPaddingProperty = DependencyProperty.Register("GlyphLeftPadding", typeof(Thickness), typeof(GlyphControl), new PropertyMetadata(new Thickness(), (d, e) => ((GlyphControl)d).UpdatePadding()));
		public static readonly DependencyProperty GlyphRightPaddingProperty = DependencyProperty.Register("GlyphRightPadding", typeof(Thickness), typeof(GlyphControl), new PropertyMetadata(new Thickness(), (d, e) => ((GlyphControl)d).UpdatePadding()));
		public static readonly DependencyProperty GlyphUpPaddingProperty = DependencyProperty.Register("GlyphUpPadding", typeof(Thickness), typeof(GlyphControl), new PropertyMetadata(new Thickness(), (d, e) => ((GlyphControl)d).UpdatePadding()));
		public static readonly DependencyProperty GlyphDownPaddingProperty = DependencyProperty.Register("GlyphDownPadding", typeof(Thickness), typeof(GlyphControl), new PropertyMetadata(new Thickness(), (d, e) => ((GlyphControl)d).UpdatePadding()));
		public static readonly DependencyProperty GlyphPlusPaddingProperty = DependencyProperty.Register("GlyphPlusPadding", typeof(Thickness), typeof(GlyphControl), new PropertyMetadata(new Thickness(), (d, e) => ((GlyphControl)d).UpdatePadding()));
		public static readonly DependencyProperty GlyphClosePaddingProperty = DependencyProperty.Register("GlyphClosePadding", typeof(Thickness), typeof(GlyphControl), new PropertyMetadata(new Thickness(), (d, e) => ((GlyphControl)d).UpdatePadding()));
		public static readonly DependencyProperty GlyphViewInfoProperty = DependencyProperty.Register("GlyphViewInfo", typeof(GlyphControlViewInfo), typeof(GlyphControl), new PropertyMetadata(null, (d, e) => ((GlyphControl)d).OnGlyphViewInfoChanged()));
		public int Size { get { return (int)GetValue(SizeProperty); } set { SetValue(SizeProperty, value); } }
		public GlyphControlMode Mode { get { return (GlyphControlMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }
		public ControlTemplate GlyphLeftTemplate { get { return (ControlTemplate)GetValue(GlyphLeftTemplateProperty); } set { SetValue(GlyphLeftTemplateProperty, value); } }
		public ControlTemplate GlyphRightTemplate { get { return (ControlTemplate)GetValue(GlyphRightTemplateProperty); } set { SetValue(GlyphRightTemplateProperty, value); } }
		public ControlTemplate GlyphUpTemplate { get { return (ControlTemplate)GetValue(GlyphUpTemplateProperty); } set { SetValue(GlyphUpTemplateProperty, value); } }
		public ControlTemplate GlyphDownTemplate { get { return (ControlTemplate)GetValue(GlyphDownTemplateProperty); } set { SetValue(GlyphDownTemplateProperty, value); } }
		public ControlTemplate GlyphPlusTemplate { get { return (ControlTemplate)GetValue(GlyphPlusTemplateProperty); } set { SetValue(GlyphPlusTemplateProperty, value); } }
		public ControlTemplate GlyphCloseTemplate { get { return (ControlTemplate)GetValue(GlyphCloseTemplateProperty); } set { SetValue(GlyphCloseTemplateProperty, value); } }
		public Thickness GlyphLeftPadding { get { return (Thickness)GetValue(GlyphLeftPaddingProperty); } set { SetValue(GlyphLeftPaddingProperty, value); } }
		public Thickness GlyphRightPadding { get { return (Thickness)GetValue(GlyphRightPaddingProperty); } set { SetValue(GlyphRightPaddingProperty, value); } }
		public Thickness GlyphUpPadding { get { return (Thickness)GetValue(GlyphUpPaddingProperty); } set { SetValue(GlyphUpPaddingProperty, value); } }
		public Thickness GlyphDownPadding { get { return (Thickness)GetValue(GlyphDownPaddingProperty); } set { SetValue(GlyphDownPaddingProperty, value); } }
		public Thickness GlyphPlusPadding { get { return (Thickness)GetValue(GlyphPlusPaddingProperty); } set { SetValue(GlyphPlusPaddingProperty, value); } }
		public Thickness GlyphClosePadding { get { return (Thickness)GetValue(GlyphClosePaddingProperty); } set { SetValue(GlyphClosePaddingProperty, value); } }
		public GlyphControlViewInfo GlyphViewInfo { get { return (GlyphControlViewInfo)GetValue(GlyphViewInfoProperty); } set { SetValue(GlyphViewInfoProperty, value); } }
		static GlyphControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GlyphControl), new FrameworkPropertyMetadata(typeof(GlyphControl)));
			FocusableProperty.OverrideMetadata(typeof(GlyphControl), new FrameworkPropertyMetadata(false));
		}
		public Path Path { get { return (Path)GetTemplateChild("PART_Path"); } }
		protected override void OnStyleChanged(Style oldStyle, Style newStyle) {
			base.OnStyleChanged(oldStyle, newStyle);
			UpdateTemplate();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			InitOriginalGlyphSize();
			ApplySize();
		}
		void UpdateTemplate() {
			ControlTemplate res = null;
			var inheritedMode = GetGlyphControlMode(this);
			var mode = inheritedMode.HasValue ? inheritedMode.Value : Mode;
			switch(mode) {
				case GlyphControlMode.Left: res = GlyphLeftTemplate; break;
				case GlyphControlMode.Right: res = GlyphRightTemplate; break;
				case GlyphControlMode.Up: res = GlyphUpTemplate; break;
				case GlyphControlMode.Down: res = GlyphDownTemplate; break;
				case GlyphControlMode.Plus: res = GlyphPlusTemplate; break;
				case GlyphControlMode.Close: res = GlyphCloseTemplate; break;
			}
			Template = res;
			UpdatePadding();
		}
		void UpdatePadding() {
			Thickness res = new Thickness();
			var inheritedMode = GetGlyphControlMode(this);
			var mode = inheritedMode.HasValue ? inheritedMode.Value : Mode;
			switch(mode) {
				case GlyphControlMode.Left: res = GlyphLeftPadding; break;
				case GlyphControlMode.Right: res = GlyphRightPadding; break;
				case GlyphControlMode.Up: res = GlyphUpPadding; break;
				case GlyphControlMode.Down: res = GlyphDownPadding; break;
				case GlyphControlMode.Plus: res = GlyphPlusPadding; break;
				case GlyphControlMode.Close: res = GlyphClosePadding; break;
			}
			Padding = res;
		}   
		void OnGlyphViewInfoChanged() {
			if(GlyphViewInfo == null) return;
			GlyphLeftPadding = GlyphViewInfo.GlyphLeftPadding;
			GlyphRightPadding = GlyphViewInfo.GlyphRightPadding;
			GlyphUpPadding = GlyphViewInfo.GlyphUpPadding;
			GlyphDownPadding = GlyphViewInfo.GlyphDownPadding;
			GlyphPlusPadding = GlyphViewInfo.GlyphPlusPadding;
			GlyphClosePadding = GlyphViewInfo.GlyphClosePadding;
			Size = GlyphViewInfo.Size;
		}
		void ApplySize() {
			if(Path == null) return;
			Path.Width = originalGlyphWidth + Size;
			Path.Height = originalGlyphHeight + Size;
		}
		void InitOriginalGlyphSize() {
			originalGlyphWidth = 0;
			originalGlyphHeight = 0;
			if(Path == null) return;
			originalGlyphWidth = Path.Width;
			originalGlyphHeight = Path.Height;
		}
		double originalGlyphWidth = 0;
		double originalGlyphHeight = 0;
	}
	public class GlyphControlViewInfo : MarkupExtension {
		public int Size { get; set; }
		[Obsolete]
		public int Padding { get; set; }
		public Thickness GlyphLeftPadding { get; set; }
		public Thickness GlyphRightPadding { get; set; }
		public Thickness GlyphUpPadding { get; set; }
		public Thickness GlyphDownPadding { get; set; }
		public Thickness GlyphPlusPadding { get; set; }
		public Thickness GlyphClosePadding { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) { return this; }
	}
	[ContentProperty("ActualChild")]
	public class RotatableBorder : Decorator {
		#region Dependency Properties
		public static readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(Dock), typeof(RotatableBorder), new PropertyMetadata(Dock.Top, (d, e) => ((RotatableBorder)d).OnLocationChanged()));
		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(RotatableBorder), new PropertyMetadata((d, e) => ((RotatableBorder)d).Update()));
		public static readonly DependencyProperty BorderCornerRadiusProperty = DependencyProperty.Register("BorderCornerRadius", typeof(CornerRadius), typeof(RotatableBorder), new PropertyMetadata((d, e) => ((RotatableBorder)d).Update()));
		public static readonly DependencyProperty BorderMarginProperty = DependencyProperty.Register("BorderMargin", typeof(Thickness), typeof(RotatableBorder), new PropertyMetadata((d, e) => ((RotatableBorder)d).Update()));
		public static readonly DependencyProperty BorderPaddingProperty = DependencyProperty.Register("BorderPadding", typeof(Thickness), typeof(RotatableBorder), new PropertyMetadata((d, e) => ((RotatableBorder)d).Update()));
		public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(RotatableBorder), new PropertyMetadata((d, e) => ((RotatableBorder)d).Update()));
		public static readonly DependencyProperty BorderBackgroundProperty = DependencyProperty.Register("BorderBackground", typeof(Brush), typeof(RotatableBorder), new PropertyMetadata((d, e) => ((RotatableBorder)d).Update()));
		public static readonly DependencyProperty BorderOpacityMaskProperty = DependencyProperty.Register("BorderOpacityMask", typeof(Brush), typeof(RotatableBorder), new PropertyMetadata((d, e) => ((RotatableBorder)d).Update()));
		public Dock Location { get { return (Dock)GetValue(LocationProperty); } set { SetValue(LocationProperty, value); } }
		public CornerRadius BorderCornerRadius { get { return (CornerRadius)GetValue(BorderCornerRadiusProperty); } set { SetValue(BorderCornerRadiusProperty, value); } }
		public Thickness BorderThickness { get { return (Thickness)GetValue(BorderThicknessProperty); } set { SetValue(BorderThicknessProperty, value); } }
		public Thickness BorderMargin { get { return (Thickness)GetValue(BorderMarginProperty); } set { SetValue(BorderMarginProperty, value); } }
		public Thickness BorderPadding { get { return (Thickness)GetValue(BorderPaddingProperty); } set { SetValue(BorderPaddingProperty, value); } }
		public Brush BorderBrush { get { return (Brush)GetValue(BorderBrushProperty); } set { SetValue(BorderBrushProperty, value); } }
		public Brush BorderBackground { get { return (Brush)GetValue(BorderBackgroundProperty); } set { SetValue(BorderBackgroundProperty, value); } }
		public Brush BorderOpacityMask { get { return (Brush)GetValue(BorderOpacityMaskProperty); } set { SetValue(BorderOpacityMaskProperty, value); } }
		#endregion Dependency Properties
		UIElement actualChild;
		public UIElement ActualChild {
			get { return actualChild; }
			set {
				if(actualChild == value) return;
				var oldValue = actualChild;
				actualChild = value;
				OnActualChildChanged(oldValue, value);
			}
		}
		DXBorder Border;
		public RotatableBorder() {
			InitLayout();
			Loaded += OnLoaded;
		}
		protected virtual void InitLayout() {
			Border = new DXBorder();
			Child = Border;
		}
		protected virtual void OnActualChildChanged(UIElement oldValue, UIElement newValue) {
			Border.Child = newValue;
			OnLocationChanged();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) { }
		protected virtual void Update() {
			Border.BorderThickness = RotatableHelper.CorrectThickness(BorderThickness, Location);
			Border.CornerRadius = RotatableHelper.CorrectCornerRadius(BorderCornerRadius, Location);
			var margin = RotatableHelper.CorrectThickness(BorderMargin, Location);
			Margin = margin.Multiply(ScreenHelper.DpiThicknessCorrection);
			Border.Padding = RotatableHelper.CorrectThickness(BorderPadding, Location);
			Border.BorderBrush = RotatableHelper.CorrectBrush(BorderBrush, Location);
			Border.Background = RotatableHelper.CorrectBrush(BorderBackground, Location);
			OpacityMask = RotatableHelper.CorrectBrush(BorderOpacityMask, Location);
		}
		protected virtual void OnLocationChanged() {
			Update();
			(ActualChild as RotatableBorder).Do(x => x.Location = Location);
		}
	}
	public enum TabBorderMode { None, BorderBrush, BorderBackground  }
	public enum TabBackgroundMode { None, BorderBrush, BorderBackground }
	public class TabBorder : RotatableBorder {
		#region Dependency Properties
		public static readonly DependencyProperty ViewInfoProperty = DependencyProperty.Register("ViewInfo", typeof(TabViewInfo), typeof(TabBorder), new PropertyMetadata((d, e) => ((TabBorder)d).OnViewInfoChanged()));
		public static readonly DependencyProperty HoldBorderPaddingProperty = DependencyProperty.Register("HoldBorderPadding", typeof(Thickness?), typeof(TabBorder));
		public static readonly DependencyProperty HoldBorderThicknessProperty = DependencyProperty.Register("HoldBorderThickness", typeof(Thickness?), typeof(TabBorder), new PropertyMetadata((d, e) => ((TabBorder)d).Update()));
		public static readonly DependencyProperty HoldBorderCornerRadiusProperty = DependencyProperty.Register("HoldBorderCornerRadius", typeof(CornerRadius?), typeof(TabBorder), new PropertyMetadata((d, e) => ((TabBorder)d).Update()));
		public static readonly DependencyProperty HoldBorderMarginProperty = DependencyProperty.Register("HoldBorderMargin", typeof(Thickness?), typeof(TabBorder), new PropertyMetadata((d, e) => ((TabBorder)d).Update()));
		public static readonly DependencyProperty BorderModeProperty = DependencyProperty.Register("BorderMode", typeof(TabBorderMode), typeof(TabBorder), new PropertyMetadata(TabBorderMode.None, (d, e) => ((TabBorder)d).Update()));
		public static readonly DependencyProperty BackgroundModeProperty = DependencyProperty.Register("BackgroundMode", typeof(TabBackgroundMode), typeof(TabBorder), new PropertyMetadata(TabBackgroundMode.None, (d, e) => ((TabBorder)d).Update()));
		public static readonly DependencyProperty LeaveOriginBorderColorProperty = DependencyProperty.Register("LeaveOriginBorderColor", typeof(bool), typeof(TabBorder), new PropertyMetadata(true, (d, e) => ((TabBorder)d).Update()));
		public static readonly DependencyProperty LeaveOriginBackgroundColorProperty = DependencyProperty.Register("LeaveOriginBackgroundColor", typeof(bool), typeof(TabBorder), new PropertyMetadata(true, (d, e) => ((TabBorder)d).Update()));
		public static readonly DependencyProperty CustomBackgroundBrightnessProperty = DependencyProperty.Register("CustomBackgroundBrightness", typeof(double), typeof(TabBorder), new PropertyMetadata(0d));
		public static readonly DependencyProperty CustomBorderBrightnessProperty = DependencyProperty.Register("CustomBorderBrightness", typeof(double), typeof(TabBorder), new PropertyMetadata(0d));
		public TabViewInfo ViewInfo { get { return (TabViewInfo)GetValue(ViewInfoProperty); } set { SetValue(ViewInfoProperty, value); } }
		public Thickness? HoldBorderPadding { get { return (Thickness?)GetValue(HoldBorderPaddingProperty); } set { SetValue(HoldBorderPaddingProperty, value); } }
		public Thickness? HoldBorderThickness { get { return (Thickness?)GetValue(HoldBorderThicknessProperty); } set { SetValue(HoldBorderThicknessProperty, value); } }
		public CornerRadius? HoldBorderCornerRadius { get { return (CornerRadius?)GetValue(HoldBorderCornerRadiusProperty); } set { SetValue(HoldBorderCornerRadiusProperty, value); } }
		public Thickness? HoldBorderMargin { get { return (Thickness?)GetValue(HoldBorderMarginProperty); } set { SetValue(HoldBorderMarginProperty, value); } }
		public TabBorderMode BorderMode { get { return (TabBorderMode)GetValue(BorderModeProperty); } set { SetValue(BorderModeProperty, value); } }
		public TabBackgroundMode BackgroundMode { get { return (TabBackgroundMode)GetValue(BackgroundModeProperty); } set { SetValue(BackgroundModeProperty, value); } }
		public bool LeaveOriginBorderColor { get { return (bool)GetValue(LeaveOriginBorderColorProperty); } set { SetValue(LeaveOriginBorderColorProperty, value); } }
		public bool LeaveOriginBackgroundColor { get { return (bool)GetValue(LeaveOriginBackgroundColorProperty); } set { SetValue(LeaveOriginBackgroundColorProperty, value); } }
		public double CustomBackgroundBrightness { get { return (double)GetValue(CustomBackgroundBrightnessProperty); } set { SetValue(CustomBackgroundBrightnessProperty, value); } }
		public double CustomBorderBrightness { get { return (double)GetValue(CustomBorderBrightnessProperty); } set { SetValue(CustomBorderBrightnessProperty, value); } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Dock Location { get { return base.Location; } private set { base.Location = value; } }
		#endregion Dependency Properties
		Grid Layout;
		DXBorder DecorBorder;
		DXBorder ColorBorder;
		DXBorder ChildBorder;
		protected override void InitLayout() {
			Layout = new Grid();
			DecorBorder = new DXBorder() { IsHitTestVisible = false };
			ColorBorder = new DXBorder() { IsHitTestVisible = false };
			ChildBorder = new DXBorder() { Background = Brushes.Transparent };
			Layout.Children.Add(DecorBorder);
			Layout.Children.Add(ColorBorder);
			Layout.Children.Add(ChildBorder);
			Child = Layout;
		}
		protected override void OnActualChildChanged(UIElement oldValue, UIElement newValue) {
			ChildBorder.Child = newValue;
		}
		void OnViewInfoChanged() {
			Location = RotatableHelper.Convert(ViewInfo);
			(ActualChild as TabBorder).Do(x => x.ViewInfo = ViewInfo);
			Update();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			OnViewInfoChanged();
		}
		protected override void OnLocationChanged() { }
		protected override void Update() {
			OpacityMask = RotatableHelper.CorrectBrush(BorderOpacityMask, Location);
			bool isHoldMode = ViewInfo.Return(x => x.IsHoldMode, () => false);
			var borderThickness = RotatableHelper.CorrectThickness(isHoldMode && HoldBorderThickness.HasValue ? HoldBorderThickness.Value : BorderThickness, Location);
			var cornerRadius = RotatableHelper.CorrectCornerRadius(isHoldMode && HoldBorderCornerRadius.HasValue ? HoldBorderCornerRadius.Value : BorderCornerRadius, Location);
			var margin = RotatableHelper.CorrectThickness(isHoldMode && HoldBorderMargin.HasValue ? HoldBorderMargin.Value : BorderMargin, Location);
			Margin = margin.Multiply(ScreenHelper.DpiThicknessCorrection);
			var padding = RotatableHelper.CorrectThickness(isHoldMode && HoldBorderPadding.HasValue ? HoldBorderPadding.Value : BorderPadding, Location);
			var borderBrush = RotatableHelper.CorrectBrush(BorderBrush, Location);
			var background = RotatableHelper.CorrectBrush(BorderBackground, Location);
			DecorBorder.BorderThickness = ColorBorder.BorderThickness = ChildBorder.BorderThickness = borderThickness;
			DecorBorder.CornerRadius = ColorBorder.CornerRadius = ChildBorder.CornerRadius = cornerRadius;
			ChildBorder.Padding = padding;
			DecorBorder.BorderBrush = borderBrush;
			DecorBorder.Background = background;
			ColorBorder.BorderBrush = Brushes.Transparent;
			ColorBorder.Background = Brushes.Transparent;
			Brush borderColor = ViewInfo.Return(x => new SolidColorBrush(ColorHelper.Brightness(x.Border, CustomBorderBrightness)), () => Brushes.Transparent);
			Brush backgroundColor = ViewInfo.Return(x => new SolidColorBrush(ColorHelper.Brightness(x.Background, CustomBackgroundBrightness)), () => Brushes.Transparent);
			borderColor = RotatableHelper.CorrectBrush(borderColor, Location); backgroundColor = RotatableHelper.CorrectBrush(backgroundColor, Location);
			ApplyBorderColor(ColorBorder, borderColor); ApplyBackground(ColorBorder, backgroundColor);
			if(!IsTransparent(borderColor) && !LeaveOriginBorderColor) {
				ApplyBorderColor(DecorBorder, borderColor);
				ApplyBorderColor(ColorBorder, Brushes.Transparent);
			}
			if(!IsTransparent(backgroundColor) && !LeaveOriginBackgroundColor) {
				ApplyBackground(DecorBorder, backgroundColor);
				ApplyBackground(ColorBorder, Brushes.Transparent);
			}
			ColorBorder.Visibility = !IsTransparent(ColorBorder.Background) || !IsTransparent(ColorBorder.BorderBrush) ? Visibility.Visible : Visibility.Collapsed;
		}
		void ApplyBorderColor(Border border, Brush borderColor) {
			if(BorderMode == TabBorderMode.BorderBrush) border.BorderBrush = borderColor;
			else if(BorderMode == TabBorderMode.BorderBackground) border.Background = borderColor;
		}
		void ApplyBackground(Border border, Brush backgroundColor) {
			if(BackgroundMode == TabBackgroundMode.BorderBackground) border.Background = backgroundColor;
			else if(BackgroundMode == TabBackgroundMode.BorderBrush) border.BorderBrush = backgroundColor;
		}
		bool IsTransparent(Brush b) {
			return (b as SolidColorBrush).Return(x => ColorHelper.IsTransparent(x.Color), () => false);
		}
	}
	public class RotatableLayoutPanel : Grid {
		#region Properties
		public static readonly DependencyProperty LocationProperty =
			DependencyProperty.Register("Location", typeof(Dock), typeof(RotatableLayoutPanel),
			new PropertyMetadata(Dock.Top, (d, e) => ((RotatableLayoutPanel)d).Update()));
		public Dock Location {
			get { return (Dock)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public static readonly DependencyProperty LayoutPositionProperty =
			DependencyProperty.RegisterAttached("LayoutPosition", typeof(int), typeof(RotatableLayoutPanel), new PropertyMetadata(0));
		public static int GetLayoutPosition(DependencyObject obj) {
			return (int)obj.GetValue(LayoutPositionProperty);
		}
		public static void SetLayoutPosition(DependencyObject obj, int value) {
			obj.SetValue(LayoutPositionProperty, value);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new ColumnDefinitionCollection ColumnDefinitions { get { return base.ColumnDefinitions; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new RowDefinitionCollection RowDefinitions { get { return base.RowDefinitions; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ObservableCollection<RowDefinition> LayoutDefinitions { get { return layoutDefinitions; } }
		ObservableCollection<RowDefinition> layoutDefinitions = new ObservableCollection<RowDefinition>();
		#endregion Properties
		public RotatableLayoutPanel() {
			LayoutDefinitions.CollectionChanged += (d, e) => Update();
			Loaded += OnLoaded;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) { }
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			Update();
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			Update();
		}
		protected virtual void Update() {
			if(!IsInitialized) return;
			UpdateLayoutDefinitions();
			UpdateChildrenPosition();
		}
		void UpdateLayoutDefinitions() {
			RowDefinitions.Clear();
			ColumnDefinitions.Clear();
			if(Location == Dock.Top) LayoutDefinitions.ToList().ForEach(RowDefinitions.Add);
			else if(Location == Dock.Bottom) LayoutDefinitions.Reverse().ToList().ForEach(RowDefinitions.Add);
			else if(Location == Dock.Left) TransformRowDefinitions(LayoutDefinitions).ToList().ForEach(ColumnDefinitions.Add);
			else if(Location == Dock.Right) TransformRowDefinitions(LayoutDefinitions).Reverse().ToList().ForEach(ColumnDefinitions.Add);
		}
		void UpdateChildrenPosition() {
			Func<UIElement, int> GetRowIndex = x => {
				int res = GetLayoutPosition(x);
				return res > 0 ? res : 0;
			};
			Func<UIElement, int> GetInvertRowIndex = x => {
				int res = LayoutDefinitions.Count - 1 - GetLayoutPosition(x);
				return res > 0 ? res : 0;
			};
			foreach(UIElement child in Children) {
				if(Location == Dock.Top) SetRow(child, GetRowIndex(child));
				else if(Location == Dock.Bottom) SetRow(child, GetInvertRowIndex(child));
				else if(Location == Dock.Left) SetColumn(child, GetRowIndex(child));
				else if(Location == Dock.Right) SetColumn(child, GetInvertRowIndex(child));
			}
		}
		static IList<ColumnDefinition> TransformRowDefinitions(IList<RowDefinition> rows) {
			List<ColumnDefinition> res = new List<ColumnDefinition>();
			rows.ToList().ForEach(x => res.Add(new ColumnDefinition() { Width = x.Height, MaxWidth = x.MaxHeight }));
			return res;
		}
	}
	public class TabLayoutPanel : RotatableLayoutPanel {
		public static readonly DependencyProperty ViewInfoProperty =
			DependencyProperty.Register("ViewInfo", typeof(TabViewInfo), typeof(TabLayoutPanel),
			new PropertyMetadata((d, e) => ((TabLayoutPanel)d).OnViewInfoChanged()));
		public TabViewInfo ViewInfo {
			get { return (TabViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Dock Location {
			get { return base.Location; }
			private set { base.Location = value; }
		}
		void OnViewInfoChanged() {
			Location = RotatableHelper.Convert(ViewInfo);
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			OnViewInfoChanged();
		}
	}
	public class OrientableLayout {
		ObservableCollection<RowDefinition> rowDefinitions = new ObservableCollection<RowDefinition>();
		ObservableCollection<ColumnDefinition> columnDefinitions = new ObservableCollection<ColumnDefinition>();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ObservableCollection<RowDefinition> RowDefinitions { get { return rowDefinitions; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ObservableCollection<ColumnDefinition> ColumnDefinitions { get { return columnDefinitions; } }
	}
	public class OrientableLayoutPanel : Grid {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new ColumnDefinitionCollection ColumnDefinitions { get { return base.ColumnDefinitions; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new RowDefinitionCollection RowDefinitions { get { return base.RowDefinitions; } }
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(OrientableLayoutPanel),
			new PropertyMetadata(Orientation.Horizontal, (d, e) => ((OrientableLayoutPanel)d).Update()));
		public static readonly DependencyProperty HorizontalLayoutProperty =
			DependencyProperty.Register("HorizontalLayout", typeof(OrientableLayout), typeof(OrientableLayoutPanel),
			new PropertyMetadata(null, (d, e) => ((OrientableLayoutPanel)d).Update()));
		public static readonly DependencyProperty VerticalLayoutProperty =
			DependencyProperty.Register("VerticalLayout", typeof(OrientableLayout), typeof(OrientableLayoutPanel),
			new PropertyMetadata(null, (d, e) => ((OrientableLayoutPanel)d).Update()));
		public static readonly DependencyProperty HRowProperty =
			DependencyProperty.RegisterAttached("HRow", typeof(int), typeof(OrientableLayoutPanel), new PropertyMetadata(0));
		public static readonly DependencyProperty HColumnProperty =
		   DependencyProperty.RegisterAttached("HColumn", typeof(int), typeof(OrientableLayoutPanel), new PropertyMetadata(0));
		public static readonly DependencyProperty VRowProperty =
			DependencyProperty.RegisterAttached("VRow", typeof(int), typeof(OrientableLayoutPanel), new PropertyMetadata(0));
		public static readonly DependencyProperty VColumnProperty =
			DependencyProperty.RegisterAttached("VColumn", typeof(int), typeof(OrientableLayoutPanel), new PropertyMetadata(0));
		public static int GetHRow(DependencyObject obj) {
			return (int)obj.GetValue(HRowProperty);
		}
		public static void SetHRow(DependencyObject obj, int value) {
			obj.SetValue(HRowProperty, value);
		}
		public static int GetHColumn(DependencyObject obj) {
			return (int)obj.GetValue(HColumnProperty);
		}
		public static void SetHColumn(DependencyObject obj, int value) {
			obj.SetValue(HColumnProperty, value);
		}
		public static int GetVRow(DependencyObject obj) {
			return (int)obj.GetValue(VRowProperty);
		}
		public static void SetVRow(DependencyObject obj, int value) {
			obj.SetValue(VRowProperty, value);
		}
		public static int GetVColumn(DependencyObject obj) {
			return (int)obj.GetValue(VColumnProperty);
		}
		public static void SetVColumn(DependencyObject obj, int value) {
			obj.SetValue(VColumnProperty, value);
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public OrientableLayout HorizontalLayout {
			get { return (OrientableLayout)GetValue(HorizontalLayoutProperty); }
			set { SetValue(HorizontalLayoutProperty, value); }
		}
		public OrientableLayout VerticalLayout {
			get { return (OrientableLayout)GetValue(VerticalLayoutProperty); }
			set { SetValue(VerticalLayoutProperty, value); }
		}
		public OrientableLayoutPanel() {
			Loaded += OnLoaded;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) { }
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			Update();
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			Update();
		}
		protected virtual void Update() {
			if(!IsInitialized) return;
			UpdateLayoutDefinitions();
			UpdateChildrenPosition();
		}
		void UpdateLayoutDefinitions() {
			RowDefinitions.Clear();
			ColumnDefinitions.Clear();
			if(Orientation == Orientation.Horizontal && HorizontalLayout != null) {
				foreach(var row in HorizontalLayout.RowDefinitions)
					RowDefinitions.Add(row);
				foreach(var column in HorizontalLayout.ColumnDefinitions)
					ColumnDefinitions.Add(column);
			} else if(Orientation == Orientation.Vertical && VerticalLayout != null) {
				foreach(var row in VerticalLayout.RowDefinitions)
					RowDefinitions.Add(row);
				foreach(var column in VerticalLayout.ColumnDefinitions)
					ColumnDefinitions.Add(column);
			}
		}
		void UpdateChildrenPosition() {
			Func<UIElement, int> GetActualRow = x => Orientation == Orientation.Horizontal ? GetHRow(x) : GetVRow(x);
			Func<UIElement, int> GetActualColumn = x => Orientation == Orientation.Horizontal ? GetHColumn(x) : GetVColumn(x);
			foreach(UIElement child in Children) {
				SetRow(child, GetActualRow(child));
				SetColumn(child, GetActualColumn(child));
			}
		}
	}
	public class TabOrientablePanel : OrientableLayoutPanel {
		public static readonly DependencyProperty ChildMarginProperty =
			DependencyProperty.RegisterAttached("ChildMargin", typeof(Thickness), typeof(TabOrientablePanel),
			new FrameworkPropertyMetadata(new Thickness(), (d,e) => OnChildMarginChanged(d, (Thickness)e.NewValue)));
		public static Thickness GetChildMargin(DependencyObject obj) { return (Thickness)obj.GetValue(ChildMarginProperty); }
		public static void SetChildMargin(DependencyObject obj, Thickness value) { obj.SetValue(ChildMarginProperty, value); }
		static void OnChildMarginChanged(DependencyObject obj, Thickness value) {
			if(value == new Thickness()) return;
			FrameworkElement child = (FrameworkElement)obj;
			var owner = LayoutTreeHelper.GetVisualParents(child).OfType<TabOrientablePanel>().FirstOrDefault();
			if(owner == null) return;
			var margin = RotatableHelper.CorrectThicknessBasedOnOrientation(value, owner.ViewInfo);
			child.Margin = margin.Multiply(ScreenHelper.DpiThicknessCorrection);
		}
		public static readonly DependencyProperty IndentProperty = DependencyProperty.Register("Indent", typeof(Thickness), typeof(TabOrientablePanel),
			new FrameworkPropertyMetadata(new Thickness(), (d, e) => ((TabOrientablePanel)d).OnIndentChanged()));
		public Thickness Indent { get { return (Thickness)GetValue(IndentProperty); } set { SetValue(IndentProperty, value); } }
		public static readonly DependencyProperty ViewInfoProperty =
			DependencyProperty.Register("ViewInfo", typeof(TabViewInfo), typeof(TabOrientablePanel),
			new FrameworkPropertyMetadata(null, (d, e) => ((TabOrientablePanel)d).OnViewInfoChanged()));
		public TabViewInfo ViewInfo {
			get { return (TabViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Orientation Orientation {
			get { return base.Orientation; }
			private set { base.Orientation = value; }
		}
		protected virtual void OnViewInfoChanged() {
			Orientation = ViewInfo.Return(x => x.Orientation, () => Orientation.Horizontal);
			LayoutTreeHelper.GetVisualChildren(this).OfType<FrameworkElement>().ToList().ForEach(x => OnChildMarginChanged(x, GetChildMargin(x)));
			OnIndentChanged();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			OnViewInfoChanged();
		}
		void OnIndentChanged() {
			ViewInfo.If(x => x.HeaderLocation == HeaderLocation.Top).Do(x => {
				Margin = Indent;
			});
		}
	}
	public class RotateTransformExtension : MarkupExtension {
		public double Angle { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new RotateTransform(Angle);
		}
	}
	public static class RotatableHelper {
		public static Orientation Convert(HeaderLocation value) {
			return value == HeaderLocation.Left || value == HeaderLocation.Right ?
				Orientation.Vertical : Orientation.Horizontal;
		}
		public static Dock Convert(TabViewInfo value) {
			HeaderLocation headerLocation = value.Return(x => x.HeaderLocation, () => HeaderLocation.Top);
			switch(headerLocation) {
				case HeaderLocation.Left: return Dock.Left;
				case HeaderLocation.Right: return Dock.Right;
				case HeaderLocation.Bottom: return Dock.Bottom;
				default: return Dock.Top;
			}
		}
		public static Thickness CorrectThickness(Thickness value, TabViewInfo viewInfo) {
			return CorrectThickness(value, Convert(viewInfo));
		}
		public static Thickness CorrectThickness(Thickness value, Dock location) {
			switch(location) {
				case Dock.Left: return RotateLeft(value);
				case Dock.Right: return RotateRight(value);
				case Dock.Bottom: return RotateTwice(value);
				default: return value;
			}
		}
		public static Thickness CorrectThicknessBasedOnOrientation(Thickness value, TabViewInfo viewInfo) {
			Dock location = Convert(viewInfo);
			switch(location) {
				case Dock.Left: return new Thickness(value.Top, value.Right, value.Bottom, value.Left);
				case Dock.Right: return new Thickness(value.Bottom, value.Right, value.Top, value.Left);
				case Dock.Bottom: return new Thickness(value.Left, value.Bottom, value.Right, value.Top);
				default: return value;
			}
		}
		public static CornerRadius CorrectCornerRadius(CornerRadius value, TabViewInfo viewInfo) {
			return CorrectCornerRadius(value, Convert(viewInfo));
		}
		public static CornerRadius CorrectCornerRadius(CornerRadius value, Dock location) {
			switch(location) {
				case Dock.Left: return RotateLeft(value);
				case Dock.Right: return RotateRight(value);
				case Dock.Bottom: return RotateTwice(value);
				default: return value;
			}
		}
		public static Brush CorrectBrush(Brush value, TabViewInfo viewInfo) {
			return CorrectBrush(value, Convert(viewInfo));
		}
		public static Brush CorrectBrush(Brush value, Dock location) {
			if(value == null || value == Brushes.Transparent) return value;
			Brush res = value.Clone();
			if(location == Dock.Top) res.RelativeTransform = null;
			else if(location == Dock.Left) res.RelativeTransform = GetRotateLeftTransform();
			else if(location == Dock.Right) res.RelativeTransform = GetRotateRightTransform();
			else if(location == Dock.Bottom) res.RelativeTransform = GetRotateTwiceTransform();
			return res;
		}
		static Thickness RotateLeft(Thickness value) {
			return new Thickness(value.Top, value.Right, value.Bottom, value.Left);
		}
		static Thickness RotateRight(Thickness value) {
			return new Thickness(value.Bottom, value.Left, value.Top, value.Right);
		}
		static Thickness RotateTwice(Thickness value) {
			return new Thickness(value.Right, value.Bottom, value.Left, value.Top);
		}
		static CornerRadius RotateLeft(CornerRadius value) {
			return new CornerRadius(value.TopRight, value.BottomRight, value.BottomLeft, value.TopLeft);
		}
		static CornerRadius RotateRight(CornerRadius value) {
			return new CornerRadius(value.BottomLeft, value.TopLeft, value.TopRight, value.BottomRight);
		}
		static CornerRadius RotateTwice(CornerRadius value) {
			return new CornerRadius(value.BottomRight, value.BottomLeft, value.TopLeft, value.TopRight);
		}
		static Transform GetRotateLeftTransform() {
			return new RotateTransform(-90, 0.5, 0.5);
		}
		static Transform GetRotateRightTransform() {
			return new RotateTransform(90, 0.5, 0.5);
		}
		static Transform GetRotateTwiceTransform() {
			return new RotateTransform(180, 0.5, 0.5);
		}
	}
	[ContentProperty("ActualChild")]
	public class HeaderContainerPresenter : Decorator {
		public static readonly DependencyProperty ViewInfoProperty =
			DependencyProperty.Register("ViewInfo", typeof(TabViewInfo), typeof(HeaderContainerPresenter),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty ChildMinHeightProperty =
			DependencyProperty.Register("ChildMinHeight", typeof(double), typeof(HeaderContainerPresenter),
			new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsArrange));
		public TabViewInfo ViewInfo {
			get { return (TabViewInfo)GetValue(ViewInfoProperty); }
			set { SetValue(ViewInfoProperty, value); }
		}
		public double ChildMinHeight {
			get { return (double)GetValue(ChildMinHeightProperty); }
			set { SetValue(ChildMinHeightProperty, value); }
		}
		public FrameworkElement ActualChild {
			get { return (FrameworkElement)Child.Child; }
			set { Child.Child = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new Border Child {
			get { return (Border)base.Child; }
			private set { base.Child = value; }
		}
		public HeaderContainerPresenter() {
			IsVisibleChanged += OnIsVisibleChanged;
			Child = new Border();
		}
		void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			InvalidateMeasure(); 
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(Child == null) return new Size();
			Child.Measure(availableSize);
			return Child.DesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Child == null || ViewInfo == null)
				return base.ArrangeOverride(finalSize);
			Size childSize = finalSize;
			if(ViewInfo.Orientation == Orientation.Horizontal)
				childSize.Height = !double.IsNaN(ChildMinHeight) ? ChildMinHeight : finalSize.Height;
			else childSize.Width = !double.IsNaN(ChildMinHeight) ? ChildMinHeight : finalSize.Width;
			Point pos = new Point();
			if(ViewInfo.IsMultiLineMode && !ViewInfo.IsHoldMode) {
				if(ViewInfo.HeaderLocation == HeaderLocation.Bottom)
					pos.Y = finalSize.Height - childSize.Height;
				if(ViewInfo.HeaderLocation == HeaderLocation.Right)
					pos.X = finalSize.Width - childSize.Width;
			}
			Child.Arrange(new Rect(pos, childSize));
			if(ViewInfo.HeaderOrientation == Orientation.Horizontal)
				Child.LayoutTransform = null;
			else {
				double angle = ViewInfo.HeaderLocation == HeaderLocation.Top || ViewInfo.HeaderLocation == HeaderLocation.Left ? -90 : 90;
				Child.LayoutTransform = new RotateTransform() { Angle = angle };
			}
			return finalSize;
		}
	}
	public class HeaderSmartPresenter : Panel {
		public static double CollapseCoef = 6d;
		public static readonly DependencyProperty HideControlProperty = DependencyProperty.Register("HideControl", typeof(bool?), typeof(HeaderSmartPresenter),
			new FrameworkPropertyMetadata(null, (d, e) => ((HeaderSmartPresenter)d).OnHideControlChanged((bool?)e.OldValue, (bool?)e.NewValue)));
		public bool? HideControl { get { return (bool?)GetValue(HideControlProperty); } set { SetValue(HideControlProperty, value); } }
		public FrameworkElement IconBox { get { return icon.With(x => (FrameworkElement)x.Child); } set { icon.Child = value; } }
		public FrameworkElement ContentBox { get { return header.With(x => (FrameworkElement)x.Child); } set { header.Child = value; } }
		public FrameworkElement ControlBox { get { return control.With(x => (FrameworkElement)x.Child); } set { control.Child = value; } }
		Border icon, header, control;
		public HeaderSmartPresenter() {
			Children.Add(header = new Border());
			Children.Add(icon = new Border());
			Children.Add(control = new Border());
		}
		protected virtual void OnHideControlChanged(bool? oldValue, bool? newValue) {
			InvalidateMeasure();
			InvalidateArrange();
		}
		protected virtual double GetWidthForHideControl(double fullWidth) {
			return fullWidth - (header.DesiredSize.Width / 2);
		}
		protected override Size MeasureOverride(Size availableSize) {
			icon.Measure(new Size(double.PositiveInfinity, availableSize.Height));
			header.Measure(new Size(double.PositiveInfinity, availableSize.Height));
			control.Measure(new Size(double.PositiveInfinity, availableSize.Height));
			double avWidth = availableSize.Width;
			double avHeight = availableSize.Height;
			double height = Math.Min(avHeight, Math.Max(Math.Max(icon.DesiredSize.Height, header.DesiredSize.Height), control.DesiredSize.Height));
			double width = icon.DesiredSize.Width + header.DesiredSize.Width + control.DesiredSize.Width;
			bool isControlHidden = control.Opacity == 0;
			double collapseMaxSize = GetWidthForHideControl(width) + CollapseCoef;
			double collapseMinSize = GetWidthForHideControl(width) - CollapseCoef;
			bool hideControl = avWidth < (isControlHidden ? collapseMaxSize : collapseMinSize);
			hideControl = HideControl.HasValue ? HideControl.Value : hideControl;
			MeasuerChild(control, avWidth, height, hideControl);
			MeasuerChild(icon, avWidth - control.DesiredSize.Width, height);
			MeasuerChild(header, avWidth - icon.DesiredSize.Width - control.DesiredSize.Width, height);
			width = icon.DesiredSize.Width + header.DesiredSize.Width + control.DesiredSize.Width;
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double avWidth = finalSize.Width;
			double height = finalSize.Height;
			ArrangeChild(control, avWidth - control.DesiredSize.Width, control.DesiredSize.Width, height);
			ArrangeChild(icon, 0, icon.DesiredSize.Width, height);
			ArrangeChild(header, icon.DesiredSize.Width, avWidth - icon.DesiredSize.Width - control.DesiredSize.Width, height);
			return finalSize;
		}
		void MeasuerChild(FrameworkElement child, double width, double height, bool hideChild = false) {
			if(hideChild) {
				child.Opacity = 0;
				child.Measure(Size.Empty);
				return;
			}
			child.Opacity = 1;
			child.Measure(new Size(Math.Max(0, width), Math.Max(0, height)));
		}
		void ArrangeChild(FrameworkElement child, double x, double width, double height) {
			if(child.Opacity == 0) {
				child.Arrange(new Rect());
				return;
			}
			Point point = new Point(x, 0);
			Size size = new Size(Math.Max(0, width), Math.Max(0, height));
			child.Arrange(new Rect(point, size));
		}
	}
	public class TabHeaderSmartPresenter : HeaderSmartPresenter {
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty SmartPresentersProperty = DependencyProperty.RegisterAttached("SmartPresenters", typeof(List<TabHeaderSmartPresenter>), typeof(TabHeaderSmartPresenter), new FrameworkPropertyMetadata(null));
		static List<TabHeaderSmartPresenter> GetSmartPresenters(FrameworkElement obj) { return (List<TabHeaderSmartPresenter>)obj.GetValue(SmartPresentersProperty); }
		static void SetSmartPresenters(FrameworkElement obj, List<TabHeaderSmartPresenter> value) { obj.SetValue(SmartPresentersProperty, value); }
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabHeaderSmartPresenter),
			new FrameworkPropertyMetadata(false, (d, e) => ((TabHeaderSmartPresenter)d).OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue)));
		public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
		public TabHeaderSmartPresenter() {
			Loaded += (d, e) => ClearOwnerPanel();
			Unloaded += (d, e) => ClearOwnerPanel();
		}
		TabPanelBase ownerPanel;
		TabPanelBase OwnerPanel { get { if(ownerPanel == null) InitOwnerPanel(); return ownerPanel; } }
		void InitOwnerPanel() {
			ownerPanel = LayoutTreeHelper.GetVisualParents(this).OfType<TabPanelBase>().FirstOrDefault();
			OnOwnerPanelChanged(null, ownerPanel);
		}
		void ClearOwnerPanel() {
			OnOwnerPanelChanged(ownerPanel, null);
			ownerPanel = null;
		}
		void OnOwnerPanelChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			if(oldValue != null) {
				GetSmartPresenters(oldValue).Do(x => x.Remove(this));
				GetSmartPresenters(oldValue).If(x => x.Count == 0).Do(x => SetSmartPresenters(oldValue, null));
			}
			if(newValue != null) {
				if(GetSmartPresenters(newValue) == null) SetSmartPresenters(newValue, new List<TabHeaderSmartPresenter>());
				GetSmartPresenters(newValue).Add(this);
			}
		}
		protected override void OnHideControlChanged(bool? oldValue, bool? newValue) {
			var presenters = OwnerPanel.With(x => GetSmartPresenters(OwnerPanel));
			if(presenters == null) return;
			foreach(var presenter in presenters) {
				presenter.InvalidateMeasure();
				presenter.InvalidateArrange();
			}
		}
		protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue) {
			if(newValue) HideControl = false;
			else HideControl = null;
		}
		double widthForHideControl;
		protected override double GetWidthForHideControl(double fullWidth) {
			widthForHideControl = base.GetWidthForHideControl(fullWidth);
			var presenters = OwnerPanel.With(x => GetSmartPresenters(OwnerPanel));
			if(presenters == null) return widthForHideControl;
			double res = 0d;
			foreach(var presenter in presenters)
				res = Math.Max(presenter.widthForHideControl, res);
			return res;
		}
	}
	public class TabControlContentPresenter : ContentPresenter {
		public static readonly DependencyProperty ActualHorizontalAlignmentProperty =
			DependencyProperty.Register("ActualHorizontalAlignment", typeof(HorizontalAlignment), typeof(TabControlContentPresenter), 
			new PropertyMetadata(HorizontalAlignment.Stretch, (d,e) => ((TabControlContentPresenter)d).UpdateAlignment()));
		public static readonly DependencyProperty ActualVerticalAlignmentProperty =
			DependencyProperty.Register("ActualVerticalAlignment", typeof(VerticalAlignment), typeof(TabControlContentPresenter),
			new PropertyMetadata(VerticalAlignment.Stretch, (d, e) => ((TabControlContentPresenter)d).UpdateAlignment()));
		public HorizontalAlignment ActualHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(ActualHorizontalAlignmentProperty); }
			set { SetValue(ActualHorizontalAlignmentProperty, value); }
		}
		public VerticalAlignment ActualVerticalAlignment {
			get { return (VerticalAlignment)GetValue(ActualVerticalAlignmentProperty); }
			set { SetValue(ActualVerticalAlignmentProperty, value); }
		}
		void UpdateAlignment() {
			HorizontalAlignment = ActualHorizontalAlignment;
			VerticalAlignment = ActualVerticalAlignment;
		}
		static TabControlContentPresenter() {
			HorizontalAlignmentProperty.OverrideMetadata(typeof(TabControlContentPresenter), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch,
				(d, e) => { }, (d, e) => d.GetValue(ActualHorizontalAlignmentProperty)));
			VerticalAlignmentProperty.OverrideMetadata(typeof(TabControlContentPresenter), new FrameworkPropertyMetadata(VerticalAlignment.Stretch,
				(d, e) => { }, (d, e) => d.GetValue(ActualVerticalAlignmentProperty)));
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			if(visualAdded == null) return;
			var foregroundValueSource = System.Windows.DependencyPropertyHelper.GetValueSource(visualAdded, TextBlock.ForegroundProperty);
			if(foregroundValueSource.BaseValueSource == BaseValueSource.Default || foregroundValueSource.BaseValueSource == BaseValueSource.Inherited) {
				BindingOperations.SetBinding(visualAdded, TextBlock.ForegroundProperty,
					new Binding() { Path = new PropertyPath(TextBlock.ForegroundProperty), Source = this });
			}
		}
	}
	public class DXTabControlLastItemBehavior : Behavior<DXTabControl> {
		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DXTabControlLastItemBehavior), new PropertyMetadata(false, OnIsEnabledChanged));
		public static readonly DependencyProperty IsLastItemProperty = DependencyProperty.RegisterAttached("IsLastItem", typeof(bool), typeof(DXTabControlLastItemBehavior), new PropertyMetadata(false));
		public static bool GetIsLastItem(DXTabItem obj) { return (bool)obj.GetValue(IsLastItemProperty); }
		public static void SetIsLastItem(DXTabItem obj, bool value) { obj.SetValue(IsLastItemProperty, value); }
		public static bool GetIsEnabled(DXTabControl obj) { return (bool)obj.GetValue(IsEnabledProperty); }
		public static void SetIsEnabled(DXTabControl obj, bool value) { obj.SetValue(IsEnabledProperty, value); }
		static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			bool oldValue = (bool)e.OldValue;
			bool newValue = (bool)e.NewValue;
			DXTabControl tabControl = (DXTabControl)d;
			BehaviorCollection behaviors = Interaction.GetBehaviors(tabControl);
			if(!oldValue && newValue)
				behaviors.Add(new DXTabControlLastItemBehavior());
			else if(oldValue && !newValue)
				behaviors.Remove(behaviors.OfType<DXTabControlLastItemBehavior>().First());
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.ItemContainerGenerator.ItemsChanged += OnTabItemsChanged;
			AssociatedObject.TabHidden += OnTabHidden;
			AssociatedObject.TabShown += OnTabShown;
			AssociatedObject.Loaded += OnTabControlLoaded;
			OnTabItemsChangedCore();
		}
		protected override void OnDetaching() {
			AssociatedObject.ItemContainerGenerator.ItemsChanged -= OnTabItemsChanged;
			AssociatedObject.TabHidden -= OnTabHidden;
			AssociatedObject.TabShown -= OnTabShown;
			AssociatedObject.Loaded -= OnTabControlLoaded;
			base.OnDetaching();
		}
		void OnTabControlLoaded(object sender, RoutedEventArgs e) {
			OnTabItemsChanged(null, null);
		}
		void OnTabShown(object sender, TabControlTabShownEventArgs e) {
			OnTabItemsChanged(null, null);
		}
		void OnTabHidden(object sender, TabControlTabHiddenEventArgs e) {
			OnTabItemsChanged(null, null);
		}
		void OnTabItemsChanged(object sender, ItemsChangedEventArgs e) {
			AssociatedObject.Dispatcher.BeginInvoke(new Action(OnTabItemsChangedCore), DispatcherPriority.Render);
		}
		void OnTabItemsChangedCore() {
			bool isLastItemSet = false;
			for(int i = AssociatedObject.Items.Count - 1; i >= 0; i--) {
				var item = AssociatedObject.GetTabItem(i);
				if(item == null || item.Visibility != Visibility.Visible) continue;
				if(!isLastItemSet) {
					SetIsLastItem(item, true);
					isLastItemSet = true;
				} else SetIsLastItem(item, false);
			}
		}
	}
}
