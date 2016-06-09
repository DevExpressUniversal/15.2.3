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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.VisualElements {
	[DXToolboxBrowsable(false)]
	public class DockingSplitLayoutPanel : SplitLayoutPanel {
		public DockingSplitLayoutPanel() {
			SnapsToDevicePixels = true;
		}
	}
	public class RenderTransformPanel : Panel {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty DockProperty;
		static RenderTransformPanel() {
			var dProp = new DependencyPropertyRegistrator<RenderTransformPanel>();
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Vertical,
				(d, e) => ((RenderTransformPanel)d).OnOrientationPropertyChanged());
			dProp.Register("Dock", ref DockProperty, SWC.Dock.Top,
				(d, e) => ((RenderTransformPanel)d).OnDockChanged((SWC.Dock)e.OldValue, (SWC.Dock)e.NewValue));
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(Children.Count == 0)
				return Size.Empty;
			UIElement child = Children[0];
			child.Measure(GetCorrectSize(availableSize));
			return GetCorrectSize(child.DesiredSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Children.Count == 0)
				return Size.Empty;
			UIElement child = Children[0];
			Size arrangeSize = GetCorrectSize(finalSize);
			child.Arrange(new Rect(0, 0, arrangeSize.Width, arrangeSize.Height));
			RenderTransform = GetTransform(arrangeSize);
			return GetCorrectSize(arrangeSize);
		}
		Size GetCorrectSize(Size size) {
			return Orientation == Orientation.Horizontal ?
				size : new Size(size.Height, size.Width);
		}
		Transform GetTransform(Size size) {
			TransformGroup transform = new TransformGroup();
			switch(Dock) {
				case SWC.Dock.Left:
					transform.Children.Add(new RotateTransform() { Angle = -90 });
					transform.Children.Add(new TranslateTransform() { Y = size.Width });
					break;
				case SWC.Dock.Top:
					break;
				case SWC.Dock.Right:
					transform.Children.Add(new RotateTransform() { Angle = 90 });
					transform.Children.Add(new TranslateTransform() { X = size.Height });
					break;
				case SWC.Dock.Bottom:
					transform.Children.Add(new RotateTransform() { Angle = 180 });
					transform.Children.Add(new TranslateTransform() { X = size.Width });
					transform.Children.Add(new TranslateTransform() { Y = size.Height });
					break;
			}
			return transform;
		}
		void OnOrientationPropertyChanged() {
			InvalidateMeasure();
		}
		protected virtual void OnDockChanged(SWC.Dock oldValue, SWC.Dock newValue) {
			InvalidateMeasure();
		}
		public SWC.Dock Dock {
			get { return (SWC.Dock)GetValue(DockProperty); }
			set { SetValue(DockProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
	}
	[DXToolboxBrowsable(false)]
	public class TabHeaderTransformPanel : LayoutTransformPanel {
		#region static
		public static readonly DependencyProperty CaptionLocationProperty;
		public static readonly DependencyProperty CaptionOrientationProperty;
		static TabHeaderTransformPanel() {
			var dProp = new DependencyPropertyRegistrator<TabHeaderTransformPanel>();
			dProp.Register("CaptionLocation", ref CaptionLocationProperty, CaptionLocation.Default,
				(dObj, ea) => ((TabHeaderTransformPanel)dObj).OnCaptionLocationChanged(ea.NewValue));
			dProp.Register("CaptionOrientation", ref CaptionOrientationProperty, Orientation.Horizontal,
				(dObj, ea) => ((TabHeaderTransformPanel)dObj).OnCaptionOrientationChanged(ea.NewValue));
		}
		#endregion static
		public TabHeaderTransformPanel() {
			UseLayoutRounding = true;
		}
		protected virtual void OnCaptionLocationChanged(object newValue) {
			UpdateProperties();
		}
		protected virtual void OnCaptionOrientationChanged(object newValue) {
			UpdateProperties();
		}
		void UpdateProperties() {
			Orientation = CaptionOrientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
			if(CaptionOrientation == Orientation.Vertical) {
				Clockwise = (CaptionLocation == CaptionLocation.Right || CaptionLocation == CaptionLocation.Bottom);
			}
		}
		public Orientation CaptionOrientation {
			get { return (Orientation)GetValue(CaptionOrientationProperty); }
			set { SetValue(CaptionOrientationProperty, value); }
		}
		public CaptionLocation CaptionLocation {
			get { return (CaptionLocation)GetValue(CaptionLocationProperty); }
			set { SetValue(CaptionLocationProperty, value); }
		}
	}
	[DXToolboxBrowsable(false)]
	public class TabHeaderBackgroundPanel : RenderTransformPanel {
		#region static
		public static readonly DependencyProperty CaptionLocationProperty;
		public static readonly DependencyProperty CaptionOrientationProperty;
		static TabHeaderBackgroundPanel() {
			var dProp = new DependencyPropertyRegistrator<TabHeaderBackgroundPanel>();
			dProp.Register("CaptionLocation", ref CaptionLocationProperty, CaptionLocation.Default,
				(dObj, ea) => ((TabHeaderBackgroundPanel)dObj).OnCaptionLocationChanged(ea.NewValue));
			dProp.Register("CaptionOrientation", ref CaptionOrientationProperty, Orientation.Horizontal,
				(dObj, ea) => ((TabHeaderBackgroundPanel)dObj).OnCaptionOrientationChanged(ea.NewValue));
		}
		#endregion static
		public TabHeaderBackgroundPanel() {
			UseLayoutRounding = true;
			UpdateProperties();
		}
		protected virtual void OnCaptionLocationChanged(object newValue) {
			UpdateProperties();
		}
		protected virtual void OnCaptionOrientationChanged(object newValue) {
			UpdateProperties();
		}
		void UpdateProperties() {
			Orientation = CaptionOrientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
			Dock = Docking.Platform.CaptionLocationExtension.ToDock(CaptionLocation);
			if(CaptionOrientation == System.Windows.Controls.Orientation.Horizontal) {
				if(Dock == SWC.Dock.Top || Dock == SWC.Dock.Bottom)
					Orientation = System.Windows.Controls.Orientation.Horizontal;
			}
			else
				if(Dock == SWC.Dock.Left || Dock == SWC.Dock.Right)
					Orientation = System.Windows.Controls.Orientation.Vertical;
		}
		public Orientation CaptionOrientation {
			get { return (Orientation)GetValue(CaptionOrientationProperty); }
			set { SetValue(CaptionOrientationProperty, value); }
		}
		public CaptionLocation CaptionLocation {
			get { return (CaptionLocation)GetValue(CaptionLocationProperty); }
			set { SetValue(CaptionLocationProperty, value); }
		}
	}
	[DXToolboxBrowsable(false)]
	public class AutoHideTransformPanel : LayoutTransformPanel {
		public AutoHideTransformPanel() {
			Clockwise = true;
		}
	}
	[DXToolboxBrowsable(false)]
	public class DockDependentDecorator : Decorator {
		public static readonly DependencyProperty TopMarginProperty;
		public static readonly DependencyProperty BottomMarginProperty;
		public static readonly DependencyProperty LeftMarginProperty;
		public static readonly DependencyProperty RightMarginProperty;
		public static readonly DependencyProperty CaptionLocationProperty;
		static DockDependentDecorator() {
			var dProp = new DependencyPropertyRegistrator<DockDependentDecorator>();
			dProp.Register("TopMargin", ref TopMarginProperty, new Thickness(),
				(dObj, ea) => ((DockDependentDecorator)dObj).OnPropertyChanged(ea.NewValue));
			dProp.Register("BottomMargin", ref BottomMarginProperty, new Thickness(),
				(dObj, ea) => ((DockDependentDecorator)dObj).OnPropertyChanged(ea.NewValue));
			dProp.Register("LeftMargin", ref LeftMarginProperty, new Thickness(),
				(dObj, ea) => ((DockDependentDecorator)dObj).OnPropertyChanged(ea.NewValue));
			dProp.Register("RightMargin", ref RightMarginProperty, new Thickness(),
				(dObj, ea) => ((DockDependentDecorator)dObj).OnPropertyChanged(ea.NewValue));
			dProp.Register("CaptionLocation", ref CaptionLocationProperty, CaptionLocation.Default,
				(dObj, ea) => ((DockDependentDecorator)dObj).OnPropertyChanged(ea.NewValue));
		}
		public DockDependentDecorator() {
			UseLayoutRounding = true;
		}
		protected virtual void OnPropertyChanged(object newValue) {
			InvalidateMeasure();
		}
		protected virtual Thickness GetActualChildMargin() {
			switch(CaptionLocation) {
				case Docking.CaptionLocation.Left:
					return LeftMargin;
				case Docking.CaptionLocation.Top:
					return TopMargin;
				case Docking.CaptionLocation.Bottom:
					return BottomMargin;
				case Docking.CaptionLocation.Right:
					return RightMargin;
				default:
					return DefaultMargin;
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			if(Child == null) return base.MeasureOverride(constraint);
			Thickness actualChildMargin = GetActualChildMargin();
			double width = actualChildMargin.Left + actualChildMargin.Right;
			double height = actualChildMargin.Top + actualChildMargin.Bottom; 
			Size availableSize = constraint;
			if(!double.IsPositiveInfinity(availableSize.Width)) {
				availableSize.Width = Math.Max(0, availableSize.Width - width);
			}
			if(!double.IsPositiveInfinity(availableSize.Height)) {
				availableSize.Height = Math.Max(0, availableSize.Height - height);
			}
			Child.Measure(availableSize);
			return new Size(Math.Max(0, width + Child.DesiredSize.Width), Math.Max(0, height + Child.DesiredSize.Height));
		}
		Size CalcMaxDesiredSize(Thickness actualChildMargin) {
			double width = actualChildMargin.Left + actualChildMargin.Right;
			double height = actualChildMargin.Top + actualChildMargin.Bottom;
			return new Size(Math.Max(0, width + Child.DesiredSize.Width), Math.Max(0, height + Child.DesiredSize.Height));
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			if(Child == null) return base.ArrangeOverride(arrangeSize);
			Size finalSize = arrangeSize;
			Thickness childMargin = GetActualChildMargin();
			Size actualFinalSize = CalcMaxDesiredSize(childMargin);
			actualFinalSize.Height = Math.Max(actualFinalSize.Height, finalSize.Height);
			actualFinalSize.Width = Math.Max(actualFinalSize.Width, finalSize.Width);
			ArrangeCore(Child, childMargin, actualFinalSize);
			return finalSize;
		}
		protected virtual void ArrangeCore(UIElement child, Thickness element1Margin, Size finalSize) {
			Rect pos = new Rect(element1Margin.Left, element1Margin.Top,
				Math.Max(0, finalSize.Width - element1Margin.Left - element1Margin.Right),
				Math.Max(0, finalSize.Height - element1Margin.Top - element1Margin.Bottom));
			child.Arrange(pos);
		}
		public Thickness TopMargin {
			get { return (Thickness)GetValue(TopMarginProperty); }
			set { SetValue(TopMarginProperty, value); }
		}
		public Thickness BottomMargin {
			get { return (Thickness)GetValue(BottomMarginProperty); }
			set { SetValue(BottomMarginProperty, value); }
		}
		public Thickness LeftMargin {
			get { return (Thickness)GetValue(LeftMarginProperty); }
			set { SetValue(LeftMarginProperty, value); }
		}
		public Thickness RightMargin {
			get { return (Thickness)GetValue(RightMarginProperty); }
			set { SetValue(RightMarginProperty, value); }
		}
		protected virtual Thickness DefaultMargin { get { return TopMargin; } }
		public CaptionLocation CaptionLocation {
			get { return (CaptionLocation)GetValue(CaptionLocationProperty); }
			set { SetValue(CaptionLocationProperty, value); }
		}
	}
	[DXToolboxBrowsable(false)]
	public class TabHeaderContainer : DockDependentDecorator {
		#region static
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty TopSelectedMarginProperty;
		public static readonly DependencyProperty BottomSelectedMarginProperty;
		public static readonly DependencyProperty LeftSelectedMarginProperty;
		public static readonly DependencyProperty RightSelectedMarginProperty;
		static TabHeaderContainer() {
			var dProp = new DependencyPropertyRegistrator<TabHeaderContainer>();
			dProp.Register("IsSelected", ref IsSelectedProperty, false,
				(dObj, ea) => ((TabHeaderContainer)dObj).OnPropertyChanged(ea.NewValue));
			dProp.Register("TopSelectedMargin", ref TopSelectedMarginProperty, new Thickness(),
				(dObj, ea) => ((TabHeaderContainer)dObj).OnPropertyChanged(ea.NewValue));
			dProp.Register("BottomSelectedMargin", ref BottomSelectedMarginProperty, new Thickness(),
				(dObj, ea) => ((TabHeaderContainer)dObj).OnPropertyChanged(ea.NewValue));
			dProp.Register("LeftSelectedMargin", ref LeftSelectedMarginProperty, new Thickness(),
				(dObj, ea) => ((TabHeaderContainer)dObj).OnPropertyChanged(ea.NewValue));
			dProp.Register("RightSelectedMargin", ref RightSelectedMarginProperty, new Thickness(),
				(dObj, ea) => ((TabHeaderContainer)dObj).OnPropertyChanged(ea.NewValue));
		}
		#endregion static
		protected virtual Thickness GetSelectedChildMargin() {
			switch(CaptionLocation) {
				case Docking.CaptionLocation.Left:
					return LeftSelectedMargin;
				case Docking.CaptionLocation.Top:
					return TopSelectedMargin;
				case Docking.CaptionLocation.Bottom:
					return BottomSelectedMargin;
				case Docking.CaptionLocation.Right:
					return RightSelectedMargin;
				default:
					return DefaultSelectedMargin;
			}
		}
		protected override Thickness GetActualChildMargin() {
			return IsSelected ? GetSelectedChildMargin() : base.GetActualChildMargin();
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public Thickness TopSelectedMargin {
			get { return (Thickness)GetValue(TopSelectedMarginProperty); }
			set { SetValue(TopSelectedMarginProperty, value); }
		}
		public Thickness BottomSelectedMargin {
			get { return (Thickness)GetValue(BottomSelectedMarginProperty); }
			set { SetValue(BottomSelectedMarginProperty, value); }
		}
		public Thickness LeftSelectedMargin {
			get { return (Thickness)GetValue(LeftSelectedMarginProperty); }
			set { SetValue(LeftSelectedMarginProperty, value); }
		}
		public Thickness RightSelectedMargin {
			get { return (Thickness)GetValue(RightSelectedMarginProperty); }
			set { SetValue(RightSelectedMarginProperty, value); }
		}
		public Thickness DefaultSelectedMargin { get { return TopSelectedMargin; } }
	}
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	public class AppearanceControl : psvControl {
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ForegroundInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty BackgroundInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FontFamilyInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FontSizeInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FontStretchInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FontStyleInternalProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty FontWeightInternalProperty;
		static AppearanceControl() {
			var dProp = new DependencyPropertyRegistrator<AppearanceControl>();
			dProp.Register("ForegroundInternal", ref ForegroundInternalProperty, (Brush)null,
				(dObj, ea) => ((AppearanceControl)dObj).OnForegroundChanged(ea.NewValue));
			dProp.Register("BackgroundInternal", ref BackgroundInternalProperty, (Brush)null,
				(dObj, ea) => ((AppearanceControl)dObj).OnBackgroundChanged(ea.NewValue));
			dProp.Register("FontFamilyInternal", ref FontFamilyInternalProperty, (FontFamily)null,
				(dObj, ea) => ((AppearanceControl)dObj).OnFontFamilyChanged(ea.NewValue));
			dProp.Register("FontSizeInternal", ref FontSizeInternalProperty, double.NaN,
				(dObj, ea) => ((AppearanceControl)dObj).OnFontSizeChanged(ea.NewValue));
			dProp.Register("FontStretchInternal", ref FontStretchInternalProperty, FontStretches.Normal,
				(dObj, ea) => ((AppearanceControl)dObj).OnFontStretchChanged(ea.NewValue));
			dProp.Register("FontStyleInternal", ref FontStyleInternalProperty, FontStyles.Normal,
				(dObj, ea) => ((AppearanceControl)dObj).OnFontStyleChanged(ea.NewValue));
			dProp.Register("FontWeightInternal", ref FontWeightInternalProperty, FontWeights.Normal,
				(dObj, ea) => ((AppearanceControl)dObj).OnFontWeightChanged(ea.NewValue));
		}
		public AppearanceControl() {
			this.StartListen(ForegroundInternalProperty, "Foreground");
			this.StartListen(BackgroundInternalProperty, "Background");
			this.StartListen(FontFamilyInternalProperty, "FontFamily");
			this.StartListen(FontSizeInternalProperty, "FontSize");
			this.StartListen(FontStretchInternalProperty, "FontStretch");
			this.StartListen(FontStyleInternalProperty, "FontStyle");
			this.StartListen(FontWeightInternalProperty, "FontWeight");
		}
		protected virtual void OnForegroundChanged(object newValue) {
			OnVisualChanged();
		}
		protected virtual void OnBackgroundChanged(object newValue) {
			OnVisualChanged();
		}
		protected virtual void OnFontFamilyChanged(object newValue) {
			OnVisualChanged();
		}
		protected virtual void OnFontSizeChanged(object newValue) {
			OnVisualChanged();
		}
		protected virtual void OnFontStretchChanged(object newValue) {
			OnVisualChanged();
		}
		protected virtual void OnFontStyleChanged(object newValue) {
			OnVisualChanged();
		}
		protected virtual void OnFontWeightChanged(object newValue) {
			OnVisualChanged();
		}
		protected virtual void OnVisualChanged() { }
	}
	public class BaseGroupContentControl : psvContentControl {
		#region static
		static BaseGroupContentControl() {
			var dProp = new DependencyPropertyRegistrator<BaseGroupContentControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion
		public LayoutGroup LayoutGroup { get { return LayoutItem as LayoutGroup; } }
		public BaseGroupContentControl() {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(LayoutItem != null) UpdateVisual();
		}
		protected override void Subscribe(BaseLayoutItem item) {
			base.Subscribe(item);
			if(item != null)
				item.VisualChanged += new EventHandler(item_VisualChanged);
		}
		protected override void Unsubscribe(BaseLayoutItem item) {
			base.Unsubscribe(item);
			if(item != null)
				item.VisualChanged -= item_VisualChanged;
		}
		void item_VisualChanged(object sender, EventArgs e) {
			UpdateVisual();
		}
		void UpdateVisual() {
			UpdateVisualState();
			UpdateCaptionBackground();
		}
		protected virtual void UpdateVisualState() { }
		protected virtual void UpdateCaptionBackground() { }
	}
	public class GroupContentControl : BaseGroupContentControl {
		#region static
		static GroupContentControl() {
			var dProp = new DependencyPropertyRegistrator<GroupContentControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion
		public GroupContentControl() {
		}
		protected override void UpdateVisualState() {
			base.UpdateVisualState();
			VisualStateManager.GoToState(this, IsCaptionVisible ? "CaptionVisible" : "CaptionHidden", false);
		}
		protected virtual bool IsCaptionVisible { get { return LayoutItem != null && LayoutItem.IsCaptionVisible && (LayoutItem.HasCaption || LayoutItem.HasCaptionTemplate); } }
	}
	public class GroupBoxContentControl : GroupContentControl {
		#region static
		public static readonly DependencyProperty CaptionBackgroundProperty;
		static GroupBoxContentControl() {
			var dProp = new DependencyPropertyRegistrator<GroupBoxContentControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("CaptionBackground", ref CaptionBackgroundProperty, (Brush)null);
		}
		#endregion
		public GroupBoxContentControl() {
		}
		protected override void UpdateVisualState() {
			base.UpdateVisualState();
			if(LayoutGroup != null)
				VisualStateManager.GoToState(this, LayoutGroup.IsExpanded ? "Expanded" : "Collapsed", false);
		}
		protected override void UpdateCaptionBackground() {
			base.UpdateCaptionBackground();
			AppearanceObject appearance = LayoutItem.ActualAppearanceObject;
			if(appearance != null) CaptionBackground = appearance.Background;
		}
		protected override bool IsCaptionVisible { get { return LayoutItem != null && LayoutItem.IsCaptionVisible; } }
		public Brush CaptionBackground {
			get { return (Brush)GetValue(CaptionBackgroundProperty); }
			set { SetValue(CaptionBackgroundProperty, value); }
		}
	}
	public class OverlappingDockPanel : DockPanel {
		public static readonly DependencyProperty DisplayModeProperty =
			DependencyProperty.Register("DisplayMode", typeof(AutoHideMode), typeof(OverlappingDockPanel), new FrameworkPropertyMetadata(AutoHideMode.Default, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public AutoHideMode DisplayMode {
			get { return (AutoHideMode)GetValue(DisplayModeProperty); }
			set { SetValue(DisplayModeProperty, value); }
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			if(DisplayMode == AutoHideMode.Inline)
				return base.ArrangeOverride(arrangeSize);
			UIElementCollection elements = base.InternalChildren;
			int num = 0;
			int count = elements.Count;
			while(num < count) {
				UIElement element = elements[num];
				if(element != null) {
					element.Arrange(new Rect(arrangeSize));
				}
				num++;
			}
			return arrangeSize;
		}
		protected override Size MeasureOverride(Size constraint) {
			if(DisplayMode == AutoHideMode.Inline)
				return base.MeasureOverride(constraint);
			Size size = new Size();
			UIElementCollection internalChildren = base.InternalChildren;
			int num = 0;
			int count = internalChildren.Count;
			while(num < count) {
				UIElement element = internalChildren[num];
				if(element != null) {
					element.Measure(constraint);
					size.Width = Math.Max(size.Width, element.DesiredSize.Width);
					size.Height = Math.Max(size.Height, element.DesiredSize.Height);
				}
				num++;
			}
			return size;
		}
	}
	public class ElementSizer : Control {
		#region static
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ElementSizer), new PropertyMetadata(Orientation.Vertical));
		public static readonly DependencyProperty ThicknessProperty =
			DependencyProperty.Register("Thickness", typeof(double), typeof(ElementSizer), null);
		static ElementSizer() {
			var dProp = new DependencyPropertyRegistrator<ElementSizer>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public double Thickness {
			get { return (double)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}
		#region Template
		const string HorizontalRootElementName = "HorizontalRootElement";
		const string VerticalRootElementName = "VerticalRootElement";
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			HorizontalRootElement = GetTemplateChild(HorizontalRootElementName) as FrameworkElement;
			VerticalRootElement = GetTemplateChild(VerticalRootElementName) as FrameworkElement;
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if(HorizontalRootElement != null) {
				HorizontalRootElement.SetVisible(Orientation == Orientation.Horizontal);
				if(HorizontalRootElement.GetVisible())
					Cursor = HorizontalRootElement.Cursor;
			}
			if(VerticalRootElement != null) {
				VerticalRootElement.SetVisible(Orientation == Orientation.Vertical);
				if(VerticalRootElement.GetVisible())
					Cursor = VerticalRootElement.Cursor;
			}
			InvalidateMeasure();
		}
		protected FrameworkElement HorizontalRootElement { get; private set; }
		protected FrameworkElement VerticalRootElement { get; private set; }
		#endregion Template
	}
}
