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

using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
#if SILVERLIGHT
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using Viewbox = System.Windows.Controls.Viewbox;
#endif
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class ShapeContentPresenter : ContentPresenter {
		#region static
#if SILVERLIGHT
	   internal static readonly DependencyProperty ContentListenerProperty = DevExpress.Xpf.Core.WPFCompatibility.DependencyPropertyManager.RegisterAttached(
					"ContentListener", typeof(object), typeof(AppBar), new PropertyMetadata(OnContentChanged));
#endif
		public static readonly DependencyProperty ShapeStyleProperty;
		public static readonly DependencyProperty ForegroundProperty;
		public static readonly DependencyProperty TextWrappingProperty;
		public static readonly DependencyProperty TextAlignmentProperty;
		public static readonly DependencyProperty FontFamilyProperty;
		public static readonly DependencyProperty FontSizeProperty;
		public static readonly DependencyProperty AccentColorProperty;
		public static readonly DependencyProperty AllowAccentProperty;
		static ShapeContentPresenter() {
			var dProp = new DependencyPropertyRegistrator<ShapeContentPresenter>();
			dProp.Register("ShapeStyle", ref ShapeStyleProperty, (Style)null, new FrameworkPropertyMetadataOptions());
			dProp.Register("Foreground", ref ForegroundProperty, (Brush)null, new FrameworkPropertyMetadataOptions());
			dProp.Register("TextWrapping", ref TextWrappingProperty, TextWrapping.Wrap, new FrameworkPropertyMetadataOptions());
			dProp.Register("TextAlignment", ref TextAlignmentProperty, TextAlignment.Center, new FrameworkPropertyMetadataOptions());
			dProp.Register("FontFamily", ref FontFamilyProperty, "Segoe UI", new FrameworkPropertyMetadataOptions());
			dProp.Register("FontSize", ref FontSizeProperty, 11, new FrameworkPropertyMetadataOptions());
			dProp.Register("AccentColor", ref AccentColorProperty, Colors.Transparent,
				(d, e) => ((ShapeContentPresenter)d).OnAccentColorChanged((Color)e.OldValue, (Color)e.NewValue));
			dProp.Register("AllowAccent", ref AllowAccentProperty, false,
				(d, e) => ((ShapeContentPresenter)d).OnAllowAccentChanged((bool)e.OldValue, (bool)e.NewValue));
#if !SILVERLIGHT
			ContentProperty.OverrideMetadata(typeof(ShapeContentPresenter), new FrameworkPropertyMetadata(null, OnContentChanged));
#endif
		}
		static void UpdateImageColor(UIElement image, Color color, bool enabled) {
			ColorizerEffect effect = image.Effect as ColorizerEffect;
			if(!enabled || color == Colors.Transparent) {
				image.Effect = null;
				return;
			}
			if(image.Effect == null) {
				image.Effect = new ColorizerEffect() { Color = color };
				return;
			}
			effect.Color = color;
		}
		static void OnContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
			((ShapeContentPresenter)sender).OnContentChanged(args.OldValue as DependencyObject, args.NewValue as DependencyObject);
		}
		protected static IEnumerable<FrameworkElement> GetShapes(DependencyObject content) {
			Shape contentShape = content as Shape;
			if(contentShape != null) {
				yield return contentShape;
			}
			Panel shapePanel = content as Panel;
			if(shapePanel != null) {
				foreach(UIElement child in shapePanel.Children) {
					var childShapes = GetShapes(child);
					foreach(FrameworkElement childShape in childShapes)
						yield return childShape;
				}
			}
			TextBlock text = content as TextBlock;
			if(text != null) {
				yield return text;
			}
			Viewbox viewBox = content as Viewbox;
			if(viewBox != null) {
				var childShapes = GetShapes(viewBox.Child);
				foreach(FrameworkElement childShape in childShapes)
					yield return childShape;
			}
			Image image = content as Image;
			if(image != null) {
				yield return image;
			}
		}
		#endregion
#if SILVERLIGHT
		public bool IsLoaded { get; private set; }
#endif
		public ShapeContentPresenter() {
#if SILVERLIGHT
				this.StartListen(ContentListenerProperty, "Content");
#endif
			Loaded += ShapeContentPresenter_Loaded;
			Unloaded += ShapeContentPresenter_Unloaded;
		}
		void ShapeContentPresenter_Unloaded(object sender, RoutedEventArgs e) {
#if SILVERLIGHT
			IsLoaded = false;
#endif
			UpdateImageColor(this, AccentColor, AllowAccent);
		}
		void ShapeContentPresenter_Loaded(object sender, RoutedEventArgs e) {
			UpdateImageColor(this, AccentColor, AllowAccent);
#if SILVERLIGHT
			IsLoaded = true;
#endif
		}
		void OnContentChanged(DependencyObject oldValue, DependencyObject newValue) {
			foreach(FrameworkElement oldShape in GetShapes(oldValue)) {
				UpdateShapeStyle(oldShape, null);
				UnbindShape(oldShape);
			}
			foreach(FrameworkElement shape in GetShapes(newValue)) {
				UpdateShapeStyle(shape, ShapeStyle);
				BindShape(shape);
			}
		}
		protected virtual void OnAccentColorChanged(Color oldValue, Color newValue) {
			if(IsLoaded) UpdateImageColor(this, AccentColor, AllowAccent);
		}
		public void OnAllowAccentChanged(bool oldValue, bool newValue) {
			if(IsLoaded) UpdateImageColor(this, AccentColor, AllowAccent);
		}
		protected virtual void OAllowAccentChanged(bool oldValue, bool newValue) {
			if(IsLoaded) UpdateImageColor(this, AccentColor, AllowAccent);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			foreach(FrameworkElement element in GetShapes(VisualTreeHelper.GetChild(this, 0))) {
				BindShape(element);
			}
		}
		public bool AllowAccent {
			get { return (bool)GetValue(AllowAccentProperty); }
			set { SetValue(AllowAccentProperty, value); }
		}
		public Color AccentColor {
			get { return (Color)GetValue(AccentColorProperty); }
			set { SetValue(AccentColorProperty, value); }
		}
		public Style ShapeStyle {
			get { return (Style)GetValue(ShapeStyleProperty); }
			set { SetValue(ShapeStyleProperty, value); }
		}
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public TextWrapping TextWrapping {
			get { return (TextWrapping)GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}
		public TextAlignment TextAlignment {
			get { return (TextAlignment)GetValue(TextAlignmentProperty); }
			set { SetValue(TextAlignmentProperty, value); }
		}
		public string FontFamily {
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}
		public int FontSize {
			get { return (int)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}
		protected virtual void BindShape(FrameworkElement shape) {
			if(shape is Shape) {
				this.Forward(shape, Shape.StrokeProperty, "Foreground");
				this.Forward(shape, Shape.FillProperty, "Foreground");
			}
			else
				if(shape is TextBlock) {
					this.Forward(shape, TextBlock.ForegroundProperty, "Foreground");
					this.Forward(shape, TextBlock.TextWrappingProperty, "TextWrapping");
					this.Forward(shape, TextBlock.TextAlignmentProperty, "TextAlignment");
					this.Forward(shape, TextBlock.FontFamilyProperty, "FontFamily");
					this.Forward(shape, TextBlock.FontSizeProperty, "FontSize");
				}
		}
		protected virtual void UnbindShape(FrameworkElement shape) {
			if(shape is Shape) {
				shape.ClearValue(Shape.StrokeProperty);
				shape.ClearValue(Shape.FillProperty);
			}
			else if(shape is TextBlock) {
				shape.ClearValue(TextBlock.ForegroundProperty);
				shape.ClearValue(TextBlock.TextWrappingProperty);
				shape.ClearValue(TextBlock.TextAlignmentProperty);
				shape.ClearValue(TextBlock.FontFamilyProperty);
				shape.ClearValue(TextBlock.FontSizeProperty);
			}
			else if(shape is Image) {
				ImageColorizer.SetIsEnabled(shape, false);
			}
		}
		protected virtual void OnShapeStyleChanged(Style style) {
			UpdateShapeStyle(style);
		}
		protected void UpdateShapeStyle(Style style) {
			foreach(Shape shape in GetShapes(Content as DependencyObject))
				UpdateShapeStyle(shape, style);
		}
		protected void UpdateShapeStyle(FrameworkElement shape, Style style) {
			if(shape == null)
				return;
			if(style != null)
				shape.Style = style;
			else
				shape.ClearValue(Shape.StyleProperty);
		}
	}
#if SILVERLIGHT
	public class DXImage : Control {
		const string DefaultTemplateXAML =
			@"<ControlTemplate TargetType='local:DXImage' " +
				"xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
				"xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
				"xmlns:local='clr-namespace:DevExpress.Xpf.WindowsUI.Internal;assembly=DevExpress.Xpf.Controls" + AssemblyInfo.VSuffix + "'>" +
				"<Image x:Name='PART_Image' Source='{TemplateBinding Source}' Stretch='{TemplateBinding Stretch}'/>" +
			"</ControlTemplate>";
		static ControlTemplate _DefaultTemplate;
		static ControlTemplate DefaultTemplate {
			get {
				if(_DefaultTemplate == null)
					_DefaultTemplate = (ControlTemplate)System.Windows.Markup.XamlReader.Load(DefaultTemplateXAML);
				return _DefaultTemplate;
			}
		}
	#region Dependency Properties
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(ImageSource), typeof(DXImage), null);
		public static readonly DependencyProperty StretchProperty =
			DependencyProperty.Register("Stretch", typeof(Stretch), typeof(DXImage), null);
		#endregion Dependency Properties
		public DXImage() {
			IsTabStop = false;
			Template = DefaultTemplate;
		}
		protected Image PartImage { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartImage = GetTemplateChild("PART_Image") as Image;
		}
		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
	}
#endif
	public class ImagePresenter : DXImage {
		#region
		public static readonly DependencyProperty AccentColorProperty;
		public static readonly DependencyProperty AllowAccentProperty;
		static ImagePresenter() {
			var dProp = new DependencyPropertyRegistrator<ImagePresenter>();
			dProp.Register("AccentColor", ref AccentColorProperty, Colors.Transparent, OnAccentColorChanged);
			dProp.Register("AllowAccent", ref AllowAccentProperty, false, OnAllowAccentChanged);
		}
		private static void OnAccentColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ImagePresenter imagePresenter = o as ImagePresenter;
			if(imagePresenter != null)
				imagePresenter.OnAccentColorChanged((Color)e.OldValue, (Color)e.NewValue);
		}
		private static void OnAllowAccentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			ImagePresenter imagePresenter = o as ImagePresenter;
			if(imagePresenter != null)
				imagePresenter.OnAllowAccentChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion
		protected virtual void OnAccentColorChanged(Color oldValue, Color newValue) {
#if SILVERLIGHT
			if(PartImage != null) ImageColorizer.SetColor(PartImage, newValue);
#else
			ImageColorizer.SetColor(this, newValue);
#endif
		}
		protected virtual void OnAllowAccentChanged(bool oldValue, bool newValue) {
#if SILVERLIGHT
			if(PartImage != null) ImageColorizer.SetIsEnabled(PartImage, newValue);
#else
			ImageColorizer.SetIsEnabled(this, newValue);
#endif
		}
#if SILVERLIGHT
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(PartImage != null) {
				ImageColorizer.SetColor(PartImage, AccentColor);
				ImageColorizer.SetIsEnabled(PartImage, AllowAccent);
			}
		}
#endif
		public Color AccentColor {
			get { return (Color)GetValue(AccentColorProperty); }
			set { SetValue(AccentColorProperty, value); }
		}
		public bool AllowAccent {
			get { return (bool)GetValue(AllowAccentProperty); }
			set { SetValue(AllowAccentProperty, value); }
		}
	}
}
