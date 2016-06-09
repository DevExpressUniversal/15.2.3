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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.ComponentModel;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Core;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class ColorCollection : ObservableCollection<ColorDefinition> {
		public override string ToString() {
			return String.Format("Count={0}", Count);
		}
	}
	public class ColorDefinition {
		public string Name { get; set; }
		public Color Value { get; set; }
	}
	public class BrushCollection : ObservableCollection<BrushDefinition> {
		public override string ToString() {
			return String.Format("Count={0}", Count);
		}
	}
	public class BrushDefinition {
		public object Name { get; set; }
		public Brush Value { get; set; }
	}
	public class ColorResourceDictionary : Dictionary<string, Color> {
	}
	public class BrushResourceDictionary : Dictionary<string, Brush> {
	}
	#region GraphicResourceContainer
	public class GraphicResourceContainer : DependencyObject {
		ColorResourceDictionary colorResources;
		BrushResourceDictionary brushResources;
		public GraphicResourceContainer() {
			this.colorResources = new ColorResourceDictionary();
			this.brushResources = new BrushResourceDictionary();
		}
		public ColorResourceDictionary ColorResources { get { return colorResources; } }
		public BrushResourceDictionary BrushResources { get { return brushResources; } }
	}
	#endregion
	public class SchedulerColorConvertControl : Decorator {
		public SchedulerColorConvertControl() {
			this.baseColorsCollectionChangedHandler = new LeakSafeBaseColorsCollectionChangedHandler(this);
			this.baseBrushColorsCollectionChangedHandler = new LeakSafeBaseBrushColorsCollectionChangedHandler(this);
			Container = new GraphicResourceContainer();
		}
		#region Container
		public GraphicResourceContainer Container {
			get { return (GraphicResourceContainer)GetValue(ContainerProperty); }
			set { SetValue(ContainerProperty, value); }
		}
		public static readonly DependencyProperty ContainerProperty = CreateContainerProperty();
		static DependencyProperty CreateContainerProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerColorConvertControl, GraphicResourceContainer>("Container", null);
		}
		#endregion        
		#region ControlColor
		public Color ControlColor {
			get { return (Color)GetValue(ControlColorProperty); }
			set { SetValue(ControlColorProperty, value); }
		}
		public static readonly DependencyProperty ControlColorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerColorConvertControl, Color>("ControlColor", new Color(), (d, e) => d.OnControlColorChanged(e.OldValue, e.NewValue), null);
		void OnControlColorChanged(Color oldValue, Color newValue) {
			RecreateResources();
		}
		#endregion
		#region ControlBrush
		public Brush ControlBrush {
			get { return (Brush)GetValue(ControlBrushProperty); }
			set { SetValue(ControlBrushProperty, value); }
		}
		public static readonly DependencyProperty ControlBrushProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerColorConvertControl, Brush>("ControlBrush", null, (d, e) => d.OnControlBrushChanged(e.OldValue, e.NewValue), null);
		void OnControlBrushChanged(Brush oldValue, Brush newValue) {
			if(newValue == null) {
				return;
			}
			ControlColor = ((SolidColorBrush)newValue).Color;
			RecreateResources();
		}
		#endregion
		#region BaseColors
		public ColorCollection BaseColors {
			get { return (ColorCollection)GetValue(BaseColorsProperty); }
			set { SetValue(BaseColorsProperty, value); }
		}
		public static readonly DependencyProperty BaseColorsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerColorConvertControl, ColorCollection>("BaseColors", null, (d, e) => d.OnBaseColorsChanged(e.OldValue, e.NewValue), null);
		void OnBaseColorsChanged(ColorCollection oldValue, ColorCollection newValue) {
			if(oldValue != null)
				baseColorsCollectionChangedHandler.UnsubscribeCollectionChangedEvent(oldValue);
			if(newValue != null)
				baseColorsCollectionChangedHandler.SubscribeCollectionChangedEvent(newValue);
			RecreateResources();
		}
		LeakSafeBaseColorsCollectionChangedHandler baseColorsCollectionChangedHandler;
		class LeakSafeBaseColorsCollectionChangedHandler : IWeakEventListener {
			readonly WeakReference controlWeakReference;
			public LeakSafeBaseColorsCollectionChangedHandler(SchedulerColorConvertControl control) {
				this.controlWeakReference = new WeakReference(control);
			}
			public void SubscribeCollectionChangedEvent(ColorCollection collection) {
				CollectionChangedEventManager.AddListener(collection, this);
			}
			public void UnsubscribeCollectionChangedEvent(ColorCollection collection) {
				CollectionChangedEventManager.RemoveListener(collection, this);
			}
			void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
				SchedulerColorConvertControl control = controlWeakReference.Target as SchedulerColorConvertControl;
				if(control != null)
					control.BaseColorsCollectionChanged(sender, e);
			}
			bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
				NotifyCollectionChangedEventArgs args = e as NotifyCollectionChangedEventArgs;
				if (args != null)
					OnCollectionChanged(sender, args);
				return true;
			}
		}
		#endregion
		#region BaseBrushColors
		public ColorCollection BaseBrushColors {
			get { return (ColorCollection)GetValue(BaseBrushColorsProperty); }
			set { SetValue(BaseBrushColorsProperty, value); }
		}
		public static readonly DependencyProperty BaseBrushColorsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerColorConvertControl, ColorCollection>("BaseBrushColors", null, (d, e) => d.OnBaseBrushColorsChanged(e.OldValue, e.NewValue), null);
		void OnBaseBrushColorsChanged(ColorCollection oldValue, ColorCollection newValue) {
			if(oldValue != null)
				baseBrushColorsCollectionChangedHandler.UnsubscribeCollectionChangedEvent(oldValue);
			if(newValue != null)
				baseBrushColorsCollectionChangedHandler.SubscribeCollectionChangedEvent(newValue);
			RecreateResources();
		}
		LeakSafeBaseBrushColorsCollectionChangedHandler baseBrushColorsCollectionChangedHandler;
		class LeakSafeBaseBrushColorsCollectionChangedHandler {
			readonly WeakReference controlWeakReference;
			ColorCollection collection;
			public LeakSafeBaseBrushColorsCollectionChangedHandler(SchedulerColorConvertControl control) {
				this.controlWeakReference = new WeakReference(control);
			}
			public void SubscribeCollectionChangedEvent(ColorCollection collection) {
				if(this.collection != null)
					UnsubscribeCollectionChangedEvent(this.collection);
				collection.CollectionChanged += OnCollectionChanged;
				this.collection = collection;
			}
			public void UnsubscribeCollectionChangedEvent(ColorCollection collection) {
				collection.CollectionChanged -= OnCollectionChanged;
				this.collection = null;
			}
			void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
				SchedulerColorConvertControl control = controlWeakReference.Target as SchedulerColorConvertControl;
				if(control != null)
					control.BaseBrushColorsCollectionChanged(sender, e);
			}
		}
		#endregion
		#region DisableResourceColorProperty
		public static readonly DependencyProperty DisableResourceColorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerColorConvertControl, bool>("DisableResourceColor", false, (d, e) => d.OnDisableResourceColorChanged(e.OldValue, e.NewValue), null);
		public bool DisableResourceColor { get { return (bool)GetValue(DisableResourceColorProperty); } set { SetValue(DisableResourceColorProperty, value); } }
		#endregion
		void OnDisableResourceColorChanged(bool oldValue, bool newValue) {
			RecreateResources();
		}
		void BaseColorsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			RecreateResources();
		}
		void BaseBrushColorsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			RecreateResources();
		}
		void RecreateResources() {
			if (BaseColors == null && BaseBrushColors == null)
				return;
			Container = CreateContainer();
		}
		GraphicResourceContainer CreateContainer() {
			GraphicResourceContainer result = new GraphicResourceContainer();
			ColorConverter converter = CreateColorConverter();
			if(BaseColors != null) {
				bool skipConvert = DisableResourceColor || ControlColor.Equals(ColorExtension.Empty);
				foreach(ColorDefinition color in BaseColors) {
					result.ColorResources[color.Name] = skipConvert ? color.Value : (Color)converter.Convert(ControlColor, typeof(Color), color.Value, System.Globalization.CultureInfo.CurrentCulture);
				}
			}
			if(BaseBrushColors != null) {
				bool skipConvertBrushColor = DisableResourceColor || ControlColor.Equals(ColorExtension.Empty);
				foreach(ColorDefinition brushColor in BaseBrushColors) {
					Color color = (Color)(skipConvertBrushColor ? brushColor.Value : converter.Convert(ControlColor, typeof(Color), brushColor.Value, System.Globalization.CultureInfo.CurrentCulture));
					result.BrushResources[brushColor.Name] = new SolidColorBrush(color);
				}
			}
			return result;
		}
		protected virtual ColorConverter CreateColorConverter() {
			return new ColorConverter();
		}
	}
	public class LinearGradientBrushSetter : DependencyObject {
		#region Info attached property
		public static readonly DependencyProperty InfoProperty = CreatInfoProperty();
		static DependencyProperty CreatInfoProperty() {
			return DependencyProperty.RegisterAttached("Info", typeof(LinearGradientBrushInfo), typeof(LinearGradientBrushSetter), new PropertyMetadata(null, OnSetterChanged));
		}
		public static LinearGradientBrushInfo GetInfo(DependencyObject obj) {
			return (LinearGradientBrushInfo)obj.GetValue(InfoProperty);
		}
		public static void SetInfo(DependencyObject obj, LinearGradientBrushInfo value) {
			obj.SetValue(InfoProperty, value);
		}
		static void OnSetterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement targetElement = d as FrameworkElement;
			targetElement.Loaded += targetElement_Loaded;
		}
		static void targetElement_Loaded(object sender, RoutedEventArgs e) {
			FrameworkElement targetElement = sender as FrameworkElement;
			targetElement.Loaded -= targetElement_Loaded;
			targetElement.Dispatcher.BeginInvoke(new Action(() => {
				AppointmentColorConvertControl control = LayoutHelper.FindParentObject<AppointmentColorConvertControl>(targetElement);
				if (control == null)
					return;
				LinearGradientBrushInfo info = GetInfo(targetElement);
				if (info == null)
					return;
				FieldInfo fieldInfo = targetElement.GetType().GetField(info.TargetProperty + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				DependencyProperty targetDependencyProperty = fieldInfo.GetValue(null) as DependencyProperty;
				Binding binding = new Binding("Container");
				binding.Source = control;
				info.SetBinding(LinearGradientBrushInfo.ContainerProperty, binding);
				LinearGradientBrush brush = new LinearGradientBrush(info.GradientStops, 0);
				brush.StartPoint = info.StartPoint;
				brush.EndPoint = info.EndPoint;
				targetElement.SetValue(targetDependencyProperty, brush);
			}));
		}
		#endregion
	}
	[ContentProperty("GradientStops")]
	public class LinearGradientBrushInfo : FrameworkElement {
		public LinearGradientBrushInfo() {
			GradientStops = new GradientStopCollection();
		}
		public string TargetProperty { get; set; }
		public Point StartPoint { get; set; }
		public Point EndPoint { get; set; }
		public GradientStopCollection GradientStops { get; set; }
		#region Container
		public GraphicResourceContainer Container {
			get { return (GraphicResourceContainer)GetValue(ContainerProperty); }
			set { SetValue(ContainerProperty, value); }
		}
		public static readonly DependencyProperty ContainerProperty = CreateContainerProperty();
		static DependencyProperty CreateContainerProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<LinearGradientBrushInfo, GraphicResourceContainer>("Container", null, (d, e) => d.OnContainerChanged());
		}
		#endregion 
		#region GradientStopColorId
		public static readonly DependencyProperty GradientStopColorIdProperty = CreateGradientStopColorIdProperty();
		static DependencyProperty CreateGradientStopColorIdProperty() {
			return DependencyProperty.RegisterAttached("GradientStopColorId", typeof(string), typeof(LinearGradientBrushInfo), new PropertyMetadata(null, OnGradientStopColorIdChanged));
		}
		static void OnGradientStopColorIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		public static string GetGradientStopColorId(DependencyObject obj) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			return (string)obj.GetValue(GradientStopColorIdProperty);
		}
		public static void SetGradientStopColorId(DependencyObject obj, string value) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			obj.SetValue(GradientStopColorIdProperty, value);
		}
		#endregion
		protected virtual void OnContainerChanged() {
			foreach (GradientStop gradientStop in GradientStops)
				UpdateColorProperty(gradientStop);
		}
		protected virtual void UpdateColorProperty(GradientStop gradientStop) {
			ColorResourceDictionary dictionary = Container.ColorResources;
			string colorId = GetGradientStopColorId(gradientStop);
			Color color;
			if (!String.IsNullOrEmpty(colorId) && dictionary.TryGetValue(colorId, out color))
				gradientStop.Color = color;
		}
	}
	public class AppointmentColorConvertControl : SchedulerColorConvertControl {
		protected override ColorConverter CreateColorConverter() {
			return new AppointmentColorConverter();
		}
	}
	public class SchedulerOfficeColorConvertControl : SchedulerColorConvertControl {
		protected override ColorConverter CreateColorConverter() {
			return new OfficeColorConverter();
		}
	}
	public class HeaderControl : XPFContentControl {
		public HeaderControl() {
			DefaultStyleKey = typeof(HeaderControl);
		}
		#region Properties
		#region DisableResourceColorProperty
		public static readonly DependencyProperty DisableResourceColorProperty = DependencyPropertyHelper.RegisterProperty<HeaderControl, bool>("DisableResourceColor", false, (d, e) => d.OnDisableResourceColorChanged(e.OldValue, e.NewValue));
		private void OnDisableResourceColorChanged(bool oldVal, bool newVal) {
		}
		public bool DisableResourceColor { get { return (bool)GetValue(DisableResourceColorProperty); } set { SetValue(DisableResourceColorProperty, value); } }
		#endregion
		#region ResourceColor
		public static readonly DependencyProperty ResourceColorProperty =
			DependencyPropertyHelper.RegisterProperty<HeaderControl, Brush>("ResourceColor", new SolidColorBrush());
		public Brush ResourceColor { get { return (Brush)GetValue(ResourceColorProperty); } set { SetValue(ResourceColorProperty, value); } }
		#endregion
		#region GlareAlignment
		public static readonly DependencyProperty GlareAlignmentProperty =
			DependencyPropertyHelper.RegisterAttachedProperty< HeaderControl, HorizontalAlignment>("GlareAlignment", HorizontalAlignment.Left, new FrameworkPropertyMetadataOptions(), null);
		public static void SetGlareAlignment(DependencyObject element, HorizontalAlignment value) {
			element.SetValue(GlareAlignmentProperty, value);
		}
		public static HorizontalAlignment GetGlareAlignment(DependencyObject element) {
			return (HorizontalAlignment)element.GetValue(GlareAlignmentProperty);
		}
		#endregion
		#region IsAlternate
		public static readonly DependencyProperty IsAlternateProperty =
			DependencyPropertyHelper.RegisterProperty<HeaderControl, bool>("IsAlternate", false, (d, e) => d.OnIsAlternateChanged(e.OldValue, e.NewValue));
		public bool IsAlternate { get { return (bool)GetValue(IsAlternateProperty); } set { SetValue(IsAlternateProperty, value); } }
		private void OnIsAlternateChanged(bool oldVal, bool newVal) {
			SelectTemplate();
		}
		#endregion
		#region NormalTemplate
		public static readonly DependencyProperty NormalTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HeaderControl, ControlTemplate>("NormalTemplate", null, (d, e) => d.OnNormalTemplateChanged(e.OldValue, e.NewValue));
		public ControlTemplate NormalTemplate { get { return (ControlTemplate)GetValue(NormalTemplateProperty); } set { SetValue(NormalTemplateProperty, value); } }
		protected virtual void OnNormalTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue != null)
				SealHelper.SealIfSealable(newValue);
			SelectTemplate();
		}
		#endregion
		#region AlternateTemplate
		public static readonly DependencyProperty AlternateTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HeaderControl, ControlTemplate>("AlternateTemplate", null, (d, e) => d.OnAlternateTemplateChanged(e.OldValue, e.NewValue));
		public ControlTemplate AlternateTemplate { get { return (ControlTemplate)GetValue(AlternateTemplateProperty); } set { SetValue(AlternateTemplateProperty, value); } }
		protected virtual void OnAlternateTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue != null)
				SealHelper.SealIfSealable(newValue);
			SelectTemplate();
		}
		#endregion
		#endregion
		protected internal virtual void SelectTemplate() {
			ControlTemplate template = IsAlternate ? AlternateTemplate : NormalTemplate;
			if (!Object.ReferenceEquals(Template, template)) {
				Template = template;
				InvalidateArrange();
			}
		}
		#region Orientation
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public static readonly DependencyProperty OrientationProperty =
			DependencyPropertyHelper.RegisterProperty<HeaderControl, Orientation>("Orientation", Orientation.Horizontal, (d, e) => d.OnOrientationChanged(e.OldValue, e.NewValue), null);
		void OnOrientationChanged(Orientation oldValue, Orientation newValue) {
		}
		#endregion
	}
	public class VerticalHeaderControl : HeaderControl {
		public VerticalHeaderControl() {
			DefaultStyleKey = typeof(VerticalHeaderControl);
		}
	}
	#region SchedulerBorder
	[ContentProperty("Child")]
	public abstract class SchedulerBorder : Control
#if !SL
, IAddChild
#endif
 {
		const string BorderName = "PART_Border";
		UIElement child;
		protected SchedulerBorder() {
			DefaultStyleKey = typeof(SchedulerBorder);
		}
		[Bindable(true)]
		public virtual UIElement Child {
			get {
				return this.child;
			}
			set {
				if (this.child == value)
					return;
				this.child = value;
				Border b = GetTemplateChild(BorderName) as Border;
				if (b != null)
					b.Child = value;
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Border b = GetTemplateChild(BorderName) as Border;
			if (b != null) {
				FrameworkElement element = Child as FrameworkElement;
				if (element != null)
					element.SetParent(null);
				b.Child = Child;
			}
		}
		#region CornerRadius
		public CornerRadius CornerRadius {
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set {
				SetValue(CornerRadiusProperty, value);
				Border border = GetTemplateChild(BorderName) as Border;
				if (border != null)
					border.CornerRadius = value;
			}
		}
		public static readonly DependencyProperty CornerRadiusProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerBorder, CornerRadius>("CornerRadius", new CornerRadius(), (d, e) => d.OnCornerRadiusChanged(e.OldValue, e.NewValue), null);
		void OnCornerRadiusChanged(CornerRadius oldValue, CornerRadius newValue) {
			UpdateCornerRadius(newValue);
		}
		protected virtual void UpdateCornerRadius(CornerRadius newValue) {
		}
		#endregion
		public new Thickness Padding {
			get { return (Thickness)base.GetValue(PaddingProperty); }
			set {
				base.SetValue(PaddingProperty, value);
				Border border = GetTemplateChild(BorderName) as Border;
				if (border != null)
					border.Padding = value;
			}
		}
		public new Thickness Margin {
			get { return (Thickness)base.GetValue(MarginProperty); }
			set {
				base.SetValue(MarginProperty, value);
				Border border = GetTemplateChild(BorderName) as Border;
				if (border != null)
					border.Margin = value;
			}
		}
		public new Thickness BorderThickness {
			get { return (Thickness)base.GetValue(BorderThicknessProperty); }
			set {
				base.SetValue(BorderThicknessProperty, value);
				Border border = GetTemplateChild(BorderName) as Border;
				if (border != null)
					border.BorderThickness = value;
			}
		}
		#region IAddChild Members
#if !SL
		void System.Windows.Markup.IAddChild.AddChild(object value) {
			UIElement element = value as UIElement;
			if (element == null || this.Child != null) {
				throw new ArgumentException();
			}
			this.Child = element;
		}
		void System.Windows.Markup.IAddChild.AddText(string text) { }
#endif
		#endregion
	}
	#endregion
	public class SquareBorder : SchedulerBorder {
		const double MinSquareBorderWidth = 2;
		Size secondLayoutDesiredSize = Size.Empty;
		double MarginWidth { get { return (Margin.Left + Margin.Right); } }
		double MarginHeight { get { return (Margin.Top + Margin.Bottom); } }
		protected override Size MeasureOverride(Size availableSize) {
			if (Child != null)
				Child.Measure(secondLayoutDesiredSize);
			base.MeasureOverride(this.secondLayoutDesiredSize);
			return this.secondLayoutDesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if ((finalSize.Width - MarginWidth != finalSize.Height - MarginHeight) && this.secondLayoutDesiredSize == Size.Empty) {
				InvalidateMeasure();
				FrameworkElement parent = VisualTreeHelper.GetParent(this) as FrameworkElement;
				parent.InvalidateMeasure();
				double newWidth = finalSize.Height - MarginHeight + MarginWidth;
				finalSize.Width = Math.Max(MinSquareBorderWidth, newWidth);
				this.secondLayoutDesiredSize = new Size(finalSize.Width, finalSize.Height);
			}
			if (Child != null) {
				Child.Arrange(new Rect(new Point(0, 0), finalSize));
			}			
			return base.ArrangeOverride(finalSize);
		}
	}
#if SL
	public abstract class CalculatedBorderBase : SchedulerBorder {
#else
	public abstract class CalculatedBorderBase : Border {
#endif
		#region DefaultBorderThickness
		public Thickness DefaultBorderThickness {
			get { return (Thickness)GetValue(DefaultBorderThicknessProperty); }
			set { SetValue(DefaultBorderThicknessProperty, value); }
		}
		public static readonly DependencyProperty DefaultBorderThicknessProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CalculatedBorderBase, Thickness>("DefaultBorderThickness", new Thickness(0), (d, e) => d.OnDefaultBorderThicknessChanged(e.OldValue, e.NewValue), null);
		void OnDefaultBorderThicknessChanged(Thickness oldValue, Thickness newValue) {
			UpdateBorderThickness();
		}
		#endregion
		#region DefaultCornerRadius
		public CornerRadius DefaultCornerRadius {
			get { return (CornerRadius)GetValue(DefaultCornerRadiusProperty); }
			set { SetValue(DefaultCornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty DefaultCornerRadiusProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CalculatedBorderBase, CornerRadius>("DefaultCornerRadius", new CornerRadius(0), (d, e) => d.OnDefaultCornerRadiusChanged(e.OldValue, e.NewValue), null);
		void OnDefaultCornerRadiusChanged(CornerRadius oldValue, CornerRadius newValue) {
			UpdateCornerRadius();
		}
		#endregion
		#region DefaultMargin
		public Thickness DefaultMargin {
			get { return (Thickness)GetValue(DefaultMarginProperty); }
			set { SetValue(DefaultMarginProperty, value); }
		}
		public static readonly DependencyProperty DefaultMarginProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CalculatedBorderBase, Thickness>("DefaultMargin", new Thickness(0), (d, e) => d.OnDefaultMarginChanged(e.OldValue, e.NewValue), null);
		void OnDefaultMarginChanged(Thickness oldValue, Thickness newValue) {
			UpdateMargin();
		}
		#endregion
		#region DefaultPadding
		public Thickness DefaultPadding {
			get { return (Thickness)GetValue(DefaultPaddingProperty); }
			set { SetValue(DefaultPaddingProperty, value); }
		}
		public static readonly DependencyProperty DefaultPaddingProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CalculatedBorderBase, Thickness>("DefaultPadding", new Thickness(0), (d, e) => d.OnDefaultPaddingChanged(e.OldValue, e.NewValue), null);
		void OnDefaultPaddingChanged(Thickness oldValue, Thickness newValue) {
			UpdatePadding();
		}
		#endregion
		#region NoBorderMargin
		public Thickness NoBorderMargin {
			get { return (Thickness)GetValue(NoBorderMarginProperty); }
			set { SetValue(NoBorderMarginProperty, value); }
		}
		public static readonly DependencyProperty NoBorderMarginProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CalculatedBorderBase, Thickness>("NoBorderMargin", new Thickness(0), (d, e) => d.OnNoBorderMarginChanged(e.OldValue, e.NewValue), null);
		void OnNoBorderMarginChanged(Thickness oldValue, Thickness newValue) {
			UpdateMargin();
		}
		#endregion
		protected virtual void Update() {
			UpdateCornerRadius();
			UpdateMargin();
			UpdatePadding();
			UpdateBorderThickness();
		}
		protected abstract void UpdateCornerRadius();
		protected abstract void UpdateMargin();
		protected abstract void UpdatePadding();
		protected abstract void UpdateBorderThickness();
	}
	public class HeaderBorder : CalculatedBorderBase {
		#region HorizontalPosition
		public static readonly DependencyProperty ElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<HeaderBorder, ElementPosition>("ElementPosition", ElementPosition.Standalone, (d, e) => d.OnElementPositionChanged(e.OldValue, e.NewValue));
		public ElementPosition ElementPosition { get { return (ElementPosition)GetValue(ElementPositionProperty); } set { SetValue(ElementPositionProperty, value); } }
		#endregion
		protected virtual void OnElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			Update();
		}
		protected override void UpdateCornerRadius() {
			ElementPosition elementPosition = ElementPosition ?? ElementPosition.Standalone;
			bool isTop = elementPosition.IsTop;
			bool isBottom = elementPosition.IsBottom;
			bool isLeft = elementPosition.IsLeft;
			bool isRight = elementPosition.IsRight;
			bool hasTopCorner = (elementPosition.VerticalElementPosition.OuterSeparator & OuterSeparator.NoStartCorner) == 0;
			bool hasBottomCorner = (elementPosition.VerticalElementPosition.OuterSeparator & OuterSeparator.NoEndCorner) == 0;
			bool hasLeftCorner = (elementPosition.HorizontalElementPosition.OuterSeparator & OuterSeparator.NoStartCorner) == 0;
			bool hasRightCorner = (elementPosition.HorizontalElementPosition.OuterSeparator & OuterSeparator.NoEndCorner) == 0;
			bool hasTopLeftCorner = isTop & isLeft & hasTopCorner & hasLeftCorner;
			bool hasTopRightCorner = isTop & isRight & hasTopCorner & hasRightCorner;
			bool hasBottomLeftCorner = isBottom & isLeft & hasBottomCorner & hasLeftCorner;
			bool hasBottomRightCorner = isBottom & isRight & hasBottomCorner & hasRightCorner;
			UpdateCornerRadiusCore(hasTopLeftCorner, hasTopRightCorner, hasBottomLeftCorner, hasBottomRightCorner);
		}
		protected void UpdateCornerRadiusCore(bool hasTopLeftCorner, bool hasTopRightCorner, bool hasBottomLeftCorner, bool hasBottomRightCorner) {
			CornerRadius cornerRadius = DefaultCornerRadius;
			double topLeftCornerRadius = hasTopLeftCorner ? cornerRadius.TopLeft : 0;
			double topRightCornerRadius = hasTopRightCorner ? cornerRadius.TopRight : 0;
			double bottomLeftCornerRadius = hasBottomLeftCorner ? cornerRadius.BottomLeft : 0;
			double bottomRightCornerRadius = hasBottomRightCorner ? cornerRadius.BottomRight : 0;
			CornerRadius = new CornerRadius(topLeftCornerRadius, topRightCornerRadius, bottomRightCornerRadius, bottomLeftCornerRadius);
		}
		protected override void UpdateMargin() {
			Margin = DefaultMargin;
		}
		protected override void UpdatePadding() {
			ElementPosition elementPosition = ElementPosition ?? ElementPosition.Standalone;
			Thickness borderPadding = DefaultPadding;
			double leftPadding = elementPosition.HasLeftBorder ? borderPadding.Left : 0;
			double rightPadding = elementPosition.HasRightBorder ? borderPadding.Right : 0;
			double topPadding = elementPosition.HasTopBorder ? borderPadding.Top : 0;
			double bottomPadding = elementPosition.HasBottomBorder ? borderPadding.Bottom : 0;
			Padding = new Thickness(leftPadding, topPadding, rightPadding, bottomPadding);
		}
		protected override void UpdateBorderThickness() {
			ElementPosition elementPosition = ElementPosition ?? ElementPosition.Standalone;
			bool hasLeftBorder = elementPosition.HasLeftBorder;
			bool hasRightBorder = elementPosition.HasRightBorder;
			bool hasTopBorder = elementPosition.HasTopBorder;
			bool hasBottomBorder = elementPosition.HasBottomBorder;
			Thickness borderThickness = DefaultBorderThickness;
			double leftBorder = hasLeftBorder ? borderThickness.Left : 0;
			double rightBorder = hasRightBorder ? borderThickness.Right : 0;
			double topBorder = hasTopBorder ? borderThickness.Top : 0;
			double bottomBorder = hasBottomBorder ? borderThickness.Bottom : 0;
			BorderThickness = new Thickness(leftBorder, topBorder, rightBorder, bottomBorder);
		}
	}
	public class HeaderGlareBorder : HeaderBorder {
		protected override void UpdateCornerRadius() {
			ElementPosition elementPosition = ElementPosition ?? ElementPosition.Standalone;
			bool isTop = elementPosition.IsTop;
			bool isLeft = elementPosition.IsLeft;
			bool isRight = elementPosition.IsRight;
			bool hasTopCorner = (elementPosition.VerticalElementPosition.OuterSeparator & OuterSeparator.NoStartCorner) == 0;
			bool hasBottomCorner = (ElementPosition.VerticalElementPosition.OuterSeparator & OuterSeparator.NoEndCorner) == 0;
			bool hasLeftCorner = (ElementPosition.HorizontalElementPosition.OuterSeparator & OuterSeparator.NoStartCorner) == 0;
			bool hasRightCorner = (ElementPosition.HorizontalElementPosition.OuterSeparator & OuterSeparator.NoEndCorner) == 0;
			bool hasTopLeftCornerRadius = isTop & isLeft & hasTopCorner & hasLeftCorner;
			bool hasTopRightCornerRadius = isTop & isRight & hasTopCorner & hasRightCorner;
			bool hasBottomLeftCornerRadius = isTop & isRight & hasBottomCorner & hasLeftCorner;
			bool hasBottomRightCornerRadius = isTop & isRight & hasBottomCorner & hasRightCorner;
			UpdateCornerRadiusCore(hasTopLeftCornerRadius, hasTopRightCornerRadius, hasBottomLeftCornerRadius, hasBottomRightCornerRadius);
		}
		protected override void UpdatePadding() {
			ElementPosition elementPosition = ElementPosition ?? ElementPosition.Standalone;
			bool isTop = elementPosition.IsTop;
			bool isLeft = elementPosition.IsLeft;
			bool isRight = elementPosition.IsRight;
			if(isTop && (isLeft || isRight))
				Padding = DefaultPadding;
			else
				Padding = new Thickness();
		}
		protected override void UpdateBorderThickness() {
			BorderThickness = DefaultBorderThickness;
		}
	}
	public class VerticalHeaderGlareBorder : HeaderBorder {
		protected override void UpdateCornerRadius() {
			ElementPosition elementPosition = ElementPosition ?? ElementPosition.Standalone;
			bool isTop = elementPosition.IsTop;
			bool isLeft = elementPosition.IsLeft;
			bool isBottom = elementPosition.IsBottom;
			bool hasTopCorner = (elementPosition.VerticalElementPosition.OuterSeparator & OuterSeparator.NoStartCorner) == 0;
			bool hasBottomCorner = (ElementPosition.VerticalElementPosition.OuterSeparator & OuterSeparator.NoEndCorner) == 0;
			bool hasLeftCorner = (ElementPosition.HorizontalElementPosition.OuterSeparator & OuterSeparator.NoStartCorner) == 0;
			bool hasRightCorner = (ElementPosition.HorizontalElementPosition.OuterSeparator & OuterSeparator.NoEndCorner) == 0;
			bool hasTopLeftCornerRadius = isTop & isLeft & hasTopCorner & hasLeftCorner;
			bool hasTopRightCornerRadius = isTop & isLeft & hasTopCorner & hasRightCorner;
			bool hasBottomLeftCornerRadius = isBottom & isLeft & hasBottomCorner & hasLeftCorner;
			bool hasBottomRightCornerRadius = isBottom & isLeft & hasBottomCorner & hasRightCorner;
			UpdateCornerRadiusCore(hasTopLeftCornerRadius, hasTopRightCornerRadius, hasBottomLeftCornerRadius, hasBottomRightCornerRadius);
		}
	}
	public class CellBorder : HeaderBorder {
		protected override void UpdateCornerRadius() {
			CornerRadius = DefaultCornerRadius;
		}
		protected override void UpdateMargin() {
			ElementPosition elementPosition = ElementPosition ?? ElementPosition.Standalone;
			bool hasLeftMargin = (elementPosition.HorizontalPosition & ElementRelativePosition.Start) != 0;
			bool hasRightMargin = (elementPosition.HorizontalPosition & ElementRelativePosition.End) != 0;
			bool hasTopMargin = (elementPosition.VerticalPosition & ElementRelativePosition.Start) != 0;
			bool hasBottomMargin = (elementPosition.VerticalPosition & ElementRelativePosition.End) != 0;
			Thickness margin = DefaultMargin;
			double leftMargin = hasLeftMargin ? margin.Left : 0;
			double rightMargin = hasRightMargin ? margin.Right : 0;
			double topMargin = hasTopMargin ? margin.Top : 0;
			double bottomMargin = hasBottomMargin ? margin.Bottom : 0;
			Margin = new Thickness(leftMargin, rightMargin, topMargin, bottomMargin);
		}
		protected override void UpdatePadding() {
			Padding = DefaultPadding;
		}
	}
	public class CellContent : HeaderBorder {
		protected override void UpdateBorderThickness() {
			BorderThickness = DefaultBorderThickness;
		}
		protected override void UpdateCornerRadius() {
			CornerRadius = DefaultCornerRadius;
		}
		protected override void UpdateMargin() {
			ElementPosition elementPosition = ElementPosition ?? ElementPosition.Standalone;
			Thickness margin = DefaultMargin;
			bool hasLeftMargin = (elementPosition.HorizontalPosition & ElementRelativePosition.Start) != 0;
			bool hasRightMargin = (elementPosition.HorizontalPosition & ElementRelativePosition.End) != 0;
			bool hasTopMargin = (elementPosition.VerticalPosition & ElementRelativePosition.Start) != 0;
			bool hasBottomMargin = (elementPosition.VerticalPosition & ElementRelativePosition.End) != 0;
			double leftBorder = hasLeftMargin ? margin.Left : 0;
			double rightBorder = hasRightMargin ? margin.Right : 0;
			double topBorder = hasTopMargin ? margin.Top : 0;
			double bottomBorder = hasBottomMargin ? margin.Bottom : 0;
			Margin = new Thickness(leftBorder, topBorder, rightBorder, bottomBorder);
		}
		protected override void UpdatePadding() {
			Padding = DefaultPadding;
		}
	}
	public class MultiColorBorder : Control {
		static MultiColorBorder() {
		}
		public MultiColorBorder() {
			DefaultStyleKey = typeof(MultiColorBorder);
		}
		#region DefaultBorderThickness
		public Thickness DefaultBorderThickness {
			get { return (Thickness)GetValue(DefaultBorderThicknessProperty); }
			set { SetValue(DefaultBorderThicknessProperty, value); }
		}
		public static readonly DependencyProperty DefaultBorderThicknessProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<MultiColorBorder, Thickness>("DefaultBorderThickness", new Thickness(), (d, e) => d.OnDefaultBorderThicknessChanged(e.OldValue, e.NewValue), null);
		void OnDefaultBorderThicknessChanged(Thickness oldValue, Thickness newValue) {
			UpdateBorderThickness(newValue);
		}
		#endregion
		#region ElementPosition
		public ElementPosition ElementPosition {
			get { return (ElementPosition)GetValue(ElementPositionProperty); }
			set { SetValue(ElementPositionProperty, value); }
		}
		public static readonly DependencyProperty ElementPositionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<MultiColorBorder, ElementPosition>("ElementPosition", ElementPosition.Standalone, (d, e) => d.OnElementPositionChanged(e.OldValue, e.NewValue), null);
		void OnElementPositionChanged(ElementPosition oldValue, ElementPosition newValue) {
			UpdateBorderBrushes(InnerBorderBrush, OuterBorderBrush);
			UpdateBorderThickness(DefaultBorderThickness);
		}
		#endregion
		#region OuterBorderBrush
		public Brush OuterBorderBrush {
			get { return (Brush)GetValue(OuterBorderBrushProperty); }
			set { SetValue(OuterBorderBrushProperty, value); }
		}
		public static readonly DependencyProperty OuterBorderBrushProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<MultiColorBorder, Brush>("OuterBorderBrush", new SolidColorBrush(), (d, e) => d.OnOuterBorderBrushChanged(e.OldValue, e.NewValue), null);
		void OnOuterBorderBrushChanged(Brush oldValue, Brush newValue) {
			UpdateBorderBrushes(InnerBorderBrush, newValue);
		}
		#endregion
		#region InnerBorderBrush
		public Brush InnerBorderBrush {
			get { return (Brush)GetValue(InnerBorderBrushProperty); }
			set { SetValue(InnerBorderBrushProperty, value); }
		}
		public static readonly DependencyProperty InnerBorderBrushProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<MultiColorBorder, Brush>("InnerBorderBrush", new SolidColorBrush(), (d, e) => d.OnInnerBorderBrushChanged(e.OldValue, e.NewValue), null);
		void OnInnerBorderBrushChanged(Brush oldValue, Brush newValue) {
			UpdateBorderBrushes(newValue, OuterBorderBrush);
		}
		#endregion
		#region LeftBorderThickness
		public Thickness LeftBorderThickness {
			get { return (Thickness)GetValue(LeftBorderThicknessProperty); }
			private set { this.SetValue(LeftBorderThicknessPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey LeftBorderThicknessPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<MultiColorBorder, Thickness>("LeftBorderThickness", new Thickness(0));
		public static readonly DependencyProperty LeftBorderThicknessProperty = LeftBorderThicknessPropertyKey.DependencyProperty;
		#endregion
		#region RightBorderThickness
		public Thickness RightBorderThickness {
			get { return (Thickness)GetValue(RightBorderThicknessProperty); }
			private set { this.SetValue(RightBorderThicknessPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey RightBorderThicknessPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<MultiColorBorder, Thickness>("RightBorderThickness", new Thickness(0));
		public static readonly DependencyProperty RightBorderThicknessProperty = RightBorderThicknessPropertyKey.DependencyProperty;
		#endregion
		#region TopBorderThickness
		public Thickness TopBorderThickness {
			get { return (Thickness)GetValue(TopBorderThicknessProperty); }
			private set { this.SetValue(TopBorderThicknessPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey TopBorderThicknessPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<MultiColorBorder, Thickness>("TopBorderThickness", new Thickness(0));
		public static readonly DependencyProperty TopBorderThicknessProperty = TopBorderThicknessPropertyKey.DependencyProperty;
		#endregion
		#region BottomBorderThickness
		public Thickness BottomBorderThickness {
			get { return (Thickness)GetValue(BottomBorderThicknessProperty); }
			private set { this.SetValue(BottomBorderThicknessPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey BottomBorderThicknessPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<MultiColorBorder, Thickness>("BottomBorderThickness", new Thickness(0));
		public static readonly DependencyProperty BottomBorderThicknessProperty = BottomBorderThicknessPropertyKey.DependencyProperty;
		#endregion
		#region LeftBorderBrush
		public Brush LeftBorderBrush {
			get { return (Brush)GetValue(LeftBorderBrushProperty); }
			private set { this.SetValue(LeftBorderBrushPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey LeftBorderBrushPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<MultiColorBorder, Brush>("LeftBorderBrush", new SolidColorBrush());
		public static readonly DependencyProperty LeftBorderBrushProperty = LeftBorderBrushPropertyKey.DependencyProperty;
		#endregion
		#region RightBorderBrush
		public Brush RightBorderBrush {
			get { return (Brush)GetValue(RightBorderBrushProperty); }
			private set { this.SetValue(RightBorderBrushPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey RightBorderBrushPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<MultiColorBorder, Brush>("RightBorderBrush", new SolidColorBrush());
		public static readonly DependencyProperty RightBorderBrushProperty = RightBorderBrushPropertyKey.DependencyProperty;
		#endregion
		#region TopBorderBrush
		public Brush TopBorderBrush {
			get { return (Brush)GetValue(TopBorderBrushProperty); }
			private set { this.SetValue(TopBorderBrushPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey TopBorderBrushPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<MultiColorBorder, Brush>("TopBorderBrush", new SolidColorBrush());
		public static readonly DependencyProperty TopBorderBrushProperty = TopBorderBrushPropertyKey.DependencyProperty;
		#endregion
		#region BottomBorderBrush
		public Brush BottomBorderBrush {
			get { return (Brush)GetValue(BottomBorderBrushProperty); }
			private set { this.SetValue(BottomBorderBrushPropertyKey, value); }
		}
		internal static readonly DependencyPropertyKey BottomBorderBrushPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<MultiColorBorder, Brush>("BottomBorderBrush", new SolidColorBrush());
		public static readonly DependencyProperty BottomBorderBrushProperty = BottomBorderBrushPropertyKey.DependencyProperty;
		#endregion
		void UpdateBorderBrushes(Brush innerBrush, Brush outerBrush) {
			if(innerBrush == null || outerBrush == null)
				return;
			LeftBorderBrush = (ElementPosition.IsLeft) ? outerBrush : innerBrush;
			RightBorderBrush = (ElementPosition.IsRight) ? outerBrush : innerBrush;
			TopBorderBrush = (ElementPosition.IsTop) ? outerBrush : innerBrush;
			BottomBorderBrush = (ElementPosition.IsBottom) ? outerBrush : innerBrush;
		}
		void UpdateBorderThickness(Thickness newValue) {
			double left = (ElementPosition.HasLeftBorder) ? newValue.Left : 0;
			double right = (ElementPosition.HasRightBorder) ? newValue.Right : 0;
			double bottom = (ElementPosition.HasBottomBorder) ? newValue.Bottom : 0;
			double top = (ElementPosition.HasTopBorder) ? newValue.Top : 0;
			LeftBorderThickness = new Thickness(left, 0, 0, 0);
			RightBorderThickness = new Thickness(0, 0, right, 0);
			BottomBorderThickness = new Thickness(0, 0, 0, bottom);
			TopBorderThickness = new Thickness(0, top, 0, 0);
		}
	}	
}
