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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Reflection;
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Collections;
#endif
#if !DEMOCENTER
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars;
using System.Windows.Input;
#endif
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if DEMOCENTER
namespace DevExpress.DemoCenter.Xpf.Helpers {
#else
namespace DevExpress.Xpf.Core {
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
	public abstract class ControlBase : Control, IControl {
		#region Dependency Properties
		protected static DependencyProperty RegisterPropertyListener(string propertyListenerName) {
			return DependencyProperty.Register(propertyListenerName + "Listener", typeof(object), typeof(ControlBase),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (ControlBase)o;
						if (!control._IsInitializingPropertyListener)
							control.OnPropertyChanged(e.Property, e.OldValue, e.NewValue);
					}));
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		private static readonly DependencyProperty ClipListener = RegisterPropertyListener("Clip");
#if SILVERLIGHT
		static readonly DependencyPropertyKey IsMouseOverPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMouseOver", typeof(bool), typeof(ControlBase), new SLPropertyMetadata(false));
		public static readonly DependencyProperty IsMouseOverProperty = IsMouseOverPropertyKey.DependencyProperty;
#endif
		#endregion Dependency Properties
		private bool _IsInitializingPropertyListener;
#if SILVERLIGHT
		private bool _IsLoaded;
#endif
		private bool _IsUpdatingClip;
		public ControlBase() {
			Controller = CreateController();
			AttachPropertyListener("Clip", ClipListener);
			LayoutUpdated += (sender, e) => OnLayoutUpdated();
			Loaded += (sender, e) => OnLoaded();
			SizeChanged += (sender, e) => OnSizeChanged(e);
			Unloaded += (sender, e) => OnUnloaded();
		}
#if SILVERLIGHT
		public bool IsMouseOver { get { return (bool)GetValue(IsMouseOverProperty); } private set { this.SetValue(IsMouseOverPropertyKey, value); } }
#endif
		public Rect AbsoluteBounds { get { return this.GetBounds(null); } }
		public Point AbsolutePosition { get { return this.GetPosition(null); } }
		public Rect Bounds { get { return this.GetBounds(); } }
		public ControlControllerBase Controller { get; private set; }
#if SILVERLIGHT
		public bool IsLoaded {
			get { return _IsLoaded && Parent != null; }
		}
#endif
		public event EventHandler StartDrag {
			add { Controller.StartDrag += value; }
			remove { Controller.StartDrag -= value; }
		}
		public event EventHandler EndDrag {
			add { Controller.EndDrag += value; }
			remove { Controller.EndDrag -= value; }
		}
		#region Template
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateState(false);
		}
		#endregion Template
		#region Layout
		protected override Size MeasureOverride(Size availableSize) {
			Size result;
			Controller.ResetScrollBarsVisibility();
			do {
				result = OnMeasure(availableSize);
				OriginalDesiredSize = result;
				result.Width = Math.Min(result.Width, availableSize.Width);
				result.Height = Math.Min(result.Height, availableSize.Height);
				Controller.UpdateScrollBarsVisibility();
				if(result.Width == 0 || result.Height == 0)
					break;
			}
			while(result.Width > availableSize.Width || result.Height > availableSize.Height);
			return result;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			var result = OnArrange(RectHelper.New(finalSize));
			Controller.UpdateScrolling();
			return result;
		}
		protected virtual Size OnMeasure(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		protected virtual Size OnArrange(Rect bounds) {
			return base.ArrangeOverride(bounds.Size());
		}
		protected Size OriginalDesiredSize { get; private set; }
		#endregion Layout
		protected virtual ControlControllerBase CreateController() {
			return new ControlControllerBase(this);
		}
		protected void AttachPropertyListener(string propertyName, DependencyProperty propertyListener, object source = null) {
			_IsInitializingPropertyListener = true;
			try {
				SetBinding(propertyListener, new Binding(propertyName) { Source = source ?? this });
			}
			finally {
				_IsInitializingPropertyListener = false;
			}
		}
		protected void DetachPropertyListener(DependencyProperty propertyListener) {
			_IsInitializingPropertyListener = true;
			try {
				ClearValue(propertyListener);
			}
			finally {
				_IsInitializingPropertyListener = false;
			}
		}
		protected virtual Geometry GetGeometry() {
			return new RectangleGeometry { Rect = this.GetSize().ToRect() };
		}
		protected void GoToState(string stateName, bool useTransitions) {
			VisualStateManager.GoToState(this, stateName, useTransitions);
		}
		protected virtual void OnLayoutUpdated() {
		}
		protected virtual void OnLoaded() {
#if SILVERLIGHT
			_IsLoaded = true;
#endif
		}
		protected virtual void OnPropertyChanged(DependencyProperty propertyListener, object oldValue, object newValue) {
			if (propertyListener == ClipListener)
				UpdateClip();
		}
		protected virtual void OnSizeChanged(SizeChangedEventArgs e) {
			UpdateClip();
		}
		protected virtual void OnUnloaded() {
#if SILVERLIGHT
			_IsLoaded = false;
#endif
		}
		protected void UpdateClip() {
			if (!IsClipped || _IsUpdatingClip)
				return;
			_IsUpdatingClip = true;
			try {
				var geometry = GetGeometry();
				if (Clip == null || Clip.GetType() == geometry.GetType())
					Clip = geometry;
			}
			finally {
				_IsUpdatingClip = false;
			}
		}
		protected virtual void UpdateState(bool useTransitions) {
			Controller.UpdateState(false);
		}
		protected void Changed() {
			InvalidateMeasure();
		}
		protected virtual bool IsClipped { get { return Controller.IsScrollable(); } }
#if SILVERLIGHT
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			IsMouseOver = true;
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsMouseOver = false;
		}
#endif
		#region IControl
		FrameworkElement IControl.Control { get { return this; } }
		Controller IControl.Controller { get { return Controller; } }
		#endregion IControl
	}
	public class ControlControllerBase : Controller {
		public ControlControllerBase(IControl control) : base(control) { }
		public virtual void UpdateState(bool useTransitions) {
			string stateName;
			if(Control.IsEnabled)
				if(IsMouseLeftButtonDown || Control.AreAnyTouchesOver)
					stateName = "Pressed";
				else
					if(IsMouseEntered)
						stateName = "MouseOver";
					else
						stateName = "Normal";
			else
				stateName = "Disabled";
			VisualStateManager.GoToState(Control, stateName, useTransitions);
		}
		public new Control Control { get { return (Control)base.Control; } }
		protected override void AttachToEvents() {
			base.AttachToEvents();
			Control.IsEnabledChanged += (sender, e) => OnIsEnabledChanged();
		}
		protected virtual void OnIsEnabledChanged() {
			UpdateState(false);
		}
		#region Keyboard and Mouse Handling
		protected override void OnIsMouseEnteredChanged() {
			base.OnIsMouseEnteredChanged();
			UpdateState(true);
		}
		protected override void OnIsMouseLeftButtonDownChanged() {
			base.OnIsMouseLeftButtonDownChanged();
			UpdateState(true);
		}
		protected override void OnTouchDown(TouchEventArgs e) {
			base.OnTouchDown(e);
			UpdateState(true);
		}
		protected override void OnTouchUp(TouchEventArgs e) {
			base.OnTouchUp(e);
			UpdateState(true);
		}
		protected override void OnTouchLeave(TouchEventArgs e) {
			base.OnTouchLeave(e);
			UpdateState(true);
		}
		#endregion Keyboard and Mouse Handling
	}
	[ContentProperty("Content"), DXToolboxBrowsable(false)]
	public class ContentControlBase : ControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(object), typeof(ContentControlBase),
				new PropertyMetadata((o, e) => ((ContentControlBase)o).OnContentChanged(e.OldValue, e.NewValue)));
		public static readonly DependencyProperty ContentTemplateProperty =
			DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ContentControlBase), null);
		#endregion Dependency Properties
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		protected virtual void OnContentChanged(object oldValue, object newValue) {
#if !SILVERLIGHT
			if (IsContentInLogicalTree) {
				RemoveLogicalChild(oldValue);
				AddLogicalChild(newValue);
			}
#endif
		}
#if !SILVERLIGHT
		protected virtual bool IsContentInLogicalTree { get { return true; } }
		protected override IEnumerator LogicalChildren {
			get {
				if (!IsContentInLogicalTree || Content == null)
					return base.LogicalChildren;
				else
					return new object[] { Content }.GetEnumerator();
			}
		}
#endif
	}
	public class HeaderedContentControlBase : ContentControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(object), typeof(HeaderedContentControlBase),
				new PropertyMetadata((o, e) => ((HeaderedContentControlBase)o).OnHeaderChanged(e.OldValue, e.NewValue)));
		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedContentControlBase), null);
		#endregion Dependency Properties
		public object Header {
			get { return (object)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		protected virtual void OnHeaderChanged(object oldValue, object newValue) {
#if !SILVERLIGHT
			if (IsHeaderInLogicalTree) {
				RemoveLogicalChild(oldValue);
				AddLogicalChild(newValue);
			}
#endif
		}
#if !SILVERLIGHT
		protected virtual bool IsHeaderInLogicalTree { get { return true; } }
		protected override IEnumerator LogicalChildren {
			get {
				if (!IsHeaderInLogicalTree || Header == null)
					return base.LogicalChildren;
				else {
					var logicalChildren = new List<object>();
					IEnumerator baseLogicalChildren = base.LogicalChildren;
					if (baseLogicalChildren != null)
						while (baseLogicalChildren.MoveNext())
							logicalChildren.Add(baseLogicalChildren.Current);
					logicalChildren.Add(Header);
					return logicalChildren.GetEnumerator();
				}
			}
		}
#endif
	}
#endif
#if !DEMOCENTER
	[DXToolboxBrowsable(false)]
#endif
	[ContentProperty("Content")]
	public class DXContentPresenter : Control {
		const string DefaultTemplateXAML =
			@"<ControlTemplate TargetType='local:DXContentPresenter' " +
				"xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
				"xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
#if DEMOCENTER
				"xmlns:local='clr-namespace:DevExpress.DemoCenter.Xpf.Helpers;assembly=DevExpress.DemoCenter.Xpf'>" +
#else
				"xmlns:local='clr-namespace:DevExpress.Xpf.Core;assembly=DevExpress.Xpf.Core" + AssemblyInfo.VSuffix + "'>" +
#endif
				"<ContentPresenter Content='{TemplateBinding Content}' ContentTemplate='{TemplateBinding ContentTemplate}'/>" +
			"</ControlTemplate>";
		static ControlTemplate _DefaultTemplate;
		static ControlTemplate DefaultTemplate {
			get {
				if (_DefaultTemplate == null)
#if SILVERLIGHT
					_DefaultTemplate = (ControlTemplate)XamlReader.Load(DefaultTemplateXAML);
#else
					_DefaultTemplate = (ControlTemplate)XamlReader.Parse(DefaultTemplateXAML);
#endif
				return _DefaultTemplate;
			}
		}
		#region Dependency Properties
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(object), typeof(DXContentPresenter),
				new PropertyMetadata((o, e) => ((DXContentPresenter)o).OnContentChanged(e.OldValue, e.NewValue)));
		public static readonly DependencyProperty ContentTemplateProperty =
			DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(DXContentPresenter), null);
		#endregion Dependency Properties
		public DXContentPresenter() {
#if !SILVERLIGHT
			Focusable = false;
#endif
			IsTabStop = false;
			Template = DefaultTemplate;
		}
		public object Content {
			get { return GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		#region Template
		#endregion Template
		protected virtual void OnContentChanged(object oldValue, object newValue) {
			var element = newValue as UIElement;
			if (element != null) {
				var elementParent = VisualTreeHelper.GetParent(element) as ContentPresenter;
				if (elementParent != null)
					elementParent.Content = null;
			}
		}
	}
#if !DEMOCENTER
	public interface IFrameworkElement {
		Size Measure(Size availableSize);
		void InvalidateMeasureEx();
	}
	public class CornerBox : ControlBase {
		public CornerBox() {
			DefaultStyleKey = typeof(CornerBox);
		}
	}
	[ContentProperty("Content")]
	public class ContentContainer : DXContentPresenter {
		public static readonly DependencyProperty ForegroundSolidColor1Property;
		public static readonly DependencyProperty ForegroundSolidColor2Property;
		public static readonly DependencyProperty ForegroundSolidColor3Property;
		public static readonly DependencyProperty ForegroundSolidColor4Property;
		public static readonly DependencyProperty ForegroundSolidColor5Property;
		public static readonly DependencyProperty ForegroundSolidColor6Property;
		static List<DependencyProperty> foregroundSolidColorProperties;
		static ContentContainer() {
			ForegroundSolidColor1Property = DependencyProperty.Register("ForegroundSolidColor1", typeof(Color), typeof(ContentContainer),
				new PropertyMetadata((d, e) => ((ContentContainer)d).UpdateForeground()));
			ForegroundSolidColor2Property = DependencyProperty.Register("ForegroundSolidColor2", typeof(Color), typeof(ContentContainer),
				new PropertyMetadata((d, e) => ((ContentContainer)d).UpdateForeground()));
			ForegroundSolidColor3Property = DependencyProperty.Register("ForegroundSolidColor3", typeof(Color), typeof(ContentContainer),
				new PropertyMetadata((d, e) => ((ContentContainer)d).UpdateForeground()));
			ForegroundSolidColor4Property = DependencyProperty.Register("ForegroundSolidColor4", typeof(Color), typeof(ContentContainer),
				new PropertyMetadata((d, e) => ((ContentContainer)d).UpdateForeground()));
			ForegroundSolidColor5Property = DependencyProperty.Register("ForegroundSolidColor5", typeof(Color), typeof(ContentContainer),
				new PropertyMetadata((d, e) => ((ContentContainer)d).UpdateForeground()));
			ForegroundSolidColor6Property = DependencyProperty.Register("ForegroundSolidColor6", typeof(Color), typeof(ContentContainer),
				new PropertyMetadata((d, e) => ((ContentContainer)d).UpdateForeground()));
			foregroundSolidColorProperties = new List<DependencyProperty>() { ForegroundSolidColor1Property, ForegroundSolidColor2Property, ForegroundSolidColor3Property, ForegroundSolidColor4Property, ForegroundSolidColor5Property, ForegroundSolidColor6Property };
		}
		public ContentContainer() {
			FocusHelper2.SetFocusable(this, false);
		}
		public Color ForegroundSolidColor1 {
			get { return (Color)GetValue(ForegroundSolidColor1Property); }
			set { SetValue(ForegroundSolidColor1Property, value); }
		}
		public Color ForegroundSolidColor2 {
			get { return (Color)GetValue(ForegroundSolidColor2Property); }
			set { SetValue(ForegroundSolidColor2Property, value); }
		}
		public Color ForegroundSolidColor3 {
			get { return (Color)GetValue(ForegroundSolidColor3Property); }
			set { SetValue(ForegroundSolidColor3Property, value); }
		}
		public Color ForegroundSolidColor4 {
			get { return (Color)GetValue(ForegroundSolidColor4Property); }
			set { SetValue(ForegroundSolidColor4Property, value); }
		}
		public Color ForegroundSolidColor5 {
			get { return (Color)GetValue(ForegroundSolidColor5Property); }
			set { SetValue(ForegroundSolidColor5Property, value); }
		}
		public Color ForegroundSolidColor6 {
			get { return (Color)GetValue(ForegroundSolidColor6Property); }
			set { SetValue(ForegroundSolidColor6Property, value); }
		}
		void SetSolidForeground(Color color) {
			if (color == Colors.Transparent)
				ClearValue(ForegroundProperty);
			else
				Foreground = new SolidColorBrush(color);
		}
		void UpdateForeground() {
			foreach (DependencyProperty property in foregroundSolidColorProperties) {
				ValueSource valueSource = System.Windows.DependencyPropertyHelper.GetValueSource(this, property);
#if !SL
				if (valueSource.BaseValueSource != BaseValueSource.Default || valueSource.IsAnimated && ((Color)GetValue(property)) != default(Color)) {
#else
				if (valueSource.BaseValueSource != BaseValueSource.Default || ((Color)GetValue(property)) != default(Color)) {
#endif
					SetSolidForeground((Color)GetValue(property));
					return;
				}
			}
			ClearValue(ForegroundProperty);
		}
	}
#endif
	public interface INavigationItem : System.ComponentModel.INotifyPropertyChanged, ICommandSource {
		string Header { get; }
		object DataContext { get; }
		DataTemplate PeekFormTemplate { get; }
		DataTemplateSelector PeekFormTemplateSelector { get; }
	}
	public interface INavigatorClient : System.ComponentModel.INotifyPropertyChanged {
		IEnumerable<INavigationItem> Items { get; }
		IList<IBarManagerControllerAction> MenuActions { get; }
		INavigationItem SelectedItem { get; set; }
		bool AcceptsCompactNavigation { get; }
		bool Compact { get; set; }
		int PeekFormShowDelay { get; }
		int PeekFormHideDelay { get; }
		bool IsAttached { get; set; }
	}
	public interface IExpandableChild {
		bool IsExpanded { get; set; }
		double CollapseWidth { get; }
		double ExpandedWidth { get; set; }
		event ValueChangedEventHandler<bool> IsExpandedChanged;
	}
}
