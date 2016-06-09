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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System.Windows.Documents;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.NavBar {
	public enum DisplayMode {
		ImageAndText,
		Image,
		Text,
		Default
	}
	public class ImageSettings : INotifyPropertyChanged {
		public ImageSettings() {}
		public static ImageSettings GroupDefault {
			get {
				return new ImageSettings() {
					Height = 24,
					Width = 24,
					Stretch = Stretch.Uniform,
					StretchDirection = StretchDirection.Both
				};
			}
		}
		public static ImageSettings ItemDefault {
			get {
				return new ImageSettings() {
					Height = 16,
					Width = 16,
					Stretch = Stretch.Uniform,
					StretchDirection = StretchDirection.Both
				};
			}
		}
		private double width = 16d;
		private double height = 16d;
		private Stretch stretch = Stretch.Uniform;
		private StretchDirection stretchDirection = StretchDirection.Both;
		public StretchDirection StretchDirection {
			get { return stretchDirection; }
			set {
				if (value == stretchDirection) return;
				stretchDirection = value;
				RaisePropertyChange("StretchDirection");
			}
		}				
		public Stretch Stretch {
			get { return stretch; }
			set {
				if (value == stretch) return;
				stretch = value;
				RaisePropertyChange("Stretch");
			}
		}				
		public double Height {
			get { return height; }
			set {
				if (value == height) return;
				height = value;
				RaisePropertyChange("Height");
			}
		}				
		public double Width {
			get { return width; }
			set {
				if (value == width) return;
				width = value;
				RaisePropertyChange("Width");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaisePropertyChange(string propertyName) {
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public class LayoutSettings : INotifyPropertyChanged {
		public LayoutSettings() {}
		public static LayoutSettings Default {
			get {
				return new LayoutSettings() { 
					ImageHorizontalAlignment = HorizontalAlignment.Center, 
					TextVerticalAlignment = VerticalAlignment.Center, 
					TextHorizontalAlignment = HorizontalAlignment.Stretch, 
					ImageDocking = Dock.Left };
			}
		}				
		private HorizontalAlignment imageHorizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment imageVerticalAlignment = VerticalAlignment.Center;
		private HorizontalAlignment textHorizontalAlignment = HorizontalAlignment.Stretch;
		private VerticalAlignment textVerticalAlignment = VerticalAlignment.Center;
		private Dock imageDocking = Dock.Left;
		public Dock ImageDocking {
			get { return imageDocking; }
			set {
				if (value == imageDocking) return;
				imageDocking = value;
				RaisePropertyChange("ImageDocking");
			}
		}
		public VerticalAlignment TextVerticalAlignment {
			get { return textVerticalAlignment; }
			set {
				if (value == textVerticalAlignment) return;
				textVerticalAlignment = value;
				RaisePropertyChange("TextVerticalAlignment");
			}
		}
		public HorizontalAlignment TextHorizontalAlignment {
			get { return textHorizontalAlignment; }
			set {
				if (value == textHorizontalAlignment) return;
				textHorizontalAlignment = value;
				RaisePropertyChange("TextHorizontalAlignment");
			}
		}
		public VerticalAlignment ImageVerticalAlignment {
			get { return imageVerticalAlignment; }
			set {
				if (value == imageVerticalAlignment) return;
				imageVerticalAlignment = value;
				RaisePropertyChange("ImageVerticalAlignment");
			}
		}
		public HorizontalAlignment ImageHorizontalAlignment {
			get { return imageHorizontalAlignment; }
			set {
				if (value == imageHorizontalAlignment) return;
				imageHorizontalAlignment = value;
				RaisePropertyChange("ImageHorizontalAlignment");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaisePropertyChange(string propertyName) {
			if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public class FontSettings : INotifyPropertyChanged {
		public FontSettings() {}
		public static FontSettings Default {
			get {
				return new FontSettings();
			}
		}
		private FontWeight? fontWeight = null;
		private FontStyle? fontStyle = null;
		private FontStretch? fontStretch = null;
		private double fontSize = double.NaN;
		private FontFamily fontFamily = null;
		public FontFamily FontFamily {
			get { return fontFamily; }
			set {
				if (value == fontFamily) return;
				fontFamily = value;
				RaisePropertyChange("FontFamily");
			}
		}
		public double FontSize {
			get { return fontSize; }
			set {
				if (value == fontSize) return;
				fontSize = value;
				RaisePropertyChange("FontSize");
			}
		}
		public FontStretch? FontStretch {
			get { return fontStretch; }
			set {
				if (value == fontStretch) return;
				fontStretch = value;
				RaisePropertyChange("FontStretch");
			}
		}
		public FontStyle? FontStyle {
			get { return fontStyle; }
			set {
				if (value == fontStyle) return;
				fontStyle = value;
				RaisePropertyChange("FontStyle");
			}
		}
		public FontWeight? FontWeight {
			get { return fontWeight; }
			set {
				if (value == fontWeight) return;
				fontWeight = value;
				RaisePropertyChange("FontWeight");
			}
		}		
		DevExpress.Xpf.Bars.Native.WeakList<Control> ownerList = new Bars.Native.WeakList<Control>();
		public void AddOwner(Control control) {
			ownerList.Add(control);
			CheckAllProperties(control);
		}
		public void RemoveOwner(Control control) {
			ownerList.Remove(control);
		}
		DevExpress.Xpf.Bars.Native.WeakList<PropertyChangedEventHandler> propertyChangedEHList = new Bars.Native.WeakList<PropertyChangedEventHandler>();
		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyChangedEventHandler WeakPropertyChanged {
			add { propertyChangedEHList.Add(value); }
			remove {propertyChangedEHList.Remove(value);}
		}
		private void RaisePropertyChange(string propertyName) {
			var args = new PropertyChangedEventArgs(propertyName);
			if (PropertyChanged != null) PropertyChanged(this, args);
			foreach (var value in propertyChangedEHList) {
				value(this, args);
			}
			foreach (var control in ownerList)
				SetValueIfNotDefaultOrClearIfUnset(control, propertyName);
		}
		public void SetValueIfNotDefaultOrClearIfUnset(Control control, string propertyName) {
			switch(propertyName) {			   
				case "FontWeight":
					CheckFontWeight(control);
					return;
				case "FontStyle":
					CheckFontStyle(control);
					return;
				case "FontStretch":
					CheckFontStretch(control);
					return;
				case "FontSize":
					CheckFontSize(control);
					return;
				case "FontFamily":
					CheckFontFamily(control);
					return;
				default:
					return;
			}
		}
		public void CheckAllProperties(Control control) {
			CheckFontWeight(control);
			CheckFontStyle(control);
			CheckFontStretch(control);
			CheckFontSize(control);
			CheckFontFamily(control);
		}
		void SetCurrentValueCore(Control control, DependencyProperty property, object value) {
			control.SetCurrentValue(property, value);			
		}
		private void CheckFontFamily(Control control) {
			if (null == FontFamily) {
				control.ClearValue(Control.FontFamilyProperty);
			} else {
				SetCurrentValueCore(control, Control.FontFamilyProperty, (FontFamily)FontFamily);
			}
		}
		private void CheckFontSize(Control control) {
			if (Double.IsNaN(FontSize)) {
				control.ClearValue(Control.FontSizeProperty);
			} else {
				SetCurrentValueCore(control, Control.FontSizeProperty, (double)FontSize);
			}
		}
		private void CheckFontStretch(Control control) {
			if (null == FontStretch) {
				control.ClearValue(Control.FontStretchProperty);
			} else {
				SetCurrentValueCore(control, Control.FontStretchProperty, (FontStretch)FontStretch);
			}
		}
		private void CheckFontStyle(Control control) {
			if (null == FontStyle) {
				control.ClearValue(Control.FontStyleProperty);
			} else {
				SetCurrentValueCore(control, Control.FontStyleProperty, (FontStyle)FontStyle);
			}
		}
		private void CheckFontWeight(Control control) {
			if (null == FontWeight) {
				control.ClearValue(Control.FontWeightProperty);
			} else {
				SetCurrentValueCore(control, Control.FontWeightProperty, (FontWeight)FontWeight);
			}
		}
	}  
	public class NavBarImageSettingsExtension : MarkupExtension {
		public double Width { get; set; }
		public double Height { get; set; }
		public Stretch Stretch { get; set; }
		public StretchDirection StretchDirection { get; set; }
		public NavBarImageSettingsExtension() {
			Width = 16d;
			Height = 16d;
			Stretch = Stretch.Uniform;
			StretchDirection = StretchDirection.Both;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			ImageSettings imageSettings = new ImageSettings();
			imageSettings.Width = Width;
			imageSettings.Height = Height;
			imageSettings.Stretch = Stretch;
			imageSettings.StretchDirection = StretchDirection;
			return imageSettings;
		}
	}
	public class NavBarLayoutSettingsExtension : MarkupExtension {
		public VerticalAlignment ImageVerticalAlignment { get; set; }
		public HorizontalAlignment ImageHorizontalAlignment { get; set; }
		public VerticalAlignment TextVerticalAlignment { get; set; }
		public HorizontalAlignment TextHorizontalAlignment { get; set; }
		public Dock ImageDocking { get; set; }
		public NavBarLayoutSettingsExtension() {
			ImageVerticalAlignment = VerticalAlignment.Center;
			ImageHorizontalAlignment = HorizontalAlignment.Left;
			TextVerticalAlignment = VerticalAlignment.Center;
			TextHorizontalAlignment = HorizontalAlignment.Stretch;
			ImageDocking = Dock.Left;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			LayoutSettings alignmentSettings = new LayoutSettings();
			alignmentSettings.ImageVerticalAlignment = ImageVerticalAlignment;
			alignmentSettings.ImageHorizontalAlignment = ImageHorizontalAlignment;
			alignmentSettings.TextVerticalAlignment = TextVerticalAlignment;
			alignmentSettings.TextHorizontalAlignment = TextHorizontalAlignment;
			alignmentSettings.ImageDocking = ImageDocking;
			return alignmentSettings;
		}
	}
	public class NavBarFontSettingsExtension : MarkupExtension {
		public FontFamily FontFamily {get;set;}
		public double FontSize {get;set;}
		public FontStretch? FontStretch {get;set;}
		public FontStyle? FontStyle {get;set;}
		public FontWeight? FontWeight {get;set;}	  
		public NavBarFontSettingsExtension() {
			FontFamily = null;
			FontSize = Double.NaN;
			FontStretch = null;
			FontStyle = null;
			FontWeight = null;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new FontSettings {
				FontFamily = this.FontFamily,
				FontSize = this.FontSize,
				FontStretch = this.FontStretch,
				FontStyle = this.FontStyle,
				FontWeight = this.FontWeight
			};  
		}
	}
	internal class TextElementHelper<T> where T : FrameworkElement {
		public static void OverrideMetadata(Func<T, DependencyObject> getTargetFunc) {
			TextElement.FontSizeProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, (d, e) => OnFontSizeChanged((T)d, (double)e.NewValue, getTargetFunc)));
			TextElement.FontWeightProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight, (d, e) => OnFontWeightChanged((T)d, (FontWeight)e.NewValue, getTargetFunc)));
			TextElement.FontFamilyProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, (d, e) => OnFontFamilyChanged((T)d, (FontFamily)e.NewValue, getTargetFunc)));
			TextElement.FontStretchProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(FontStretches.Normal, (d, e) => OnFontStretchChanged((T)d, (FontStretch)e.NewValue, getTargetFunc)));
			TextElement.FontStyleProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle, (d, e) => OnFontStyleChanged((T)d, (FontStyle)e.NewValue, getTargetFunc)));
		}
		static bool CheckIsConnectedToVisualTree(T source) {
			return PresentationSource.FromVisual(source).With(x => x.RootVisual).ReturnSuccess();
		}
		static void OnFontStyleChanged(T source, System.Windows.FontStyle newValue, Func<T, DependencyObject> getTargetFunc) {
			if (!CheckIsConnectedToVisualTree(source))
				return;
			var target = getTargetFunc(source);
			if (target == null) return;
			if (System.Windows.DependencyPropertyHelper.GetValueSource(target, TextElement.FontSizeProperty).BaseValueSource == BaseValueSource.Inherited) {
				var fakeStyle = newValue == FontStyles.Italic ? FontStyles.Normal : FontStyles.Italic;
				TextElement.SetFontStyle(target, fakeStyle);
			}
			TextElement.SetFontStyle(target, newValue);
		}
		static void OnFontStretchChanged(T source, System.Windows.FontStretch newValue, Func<T, DependencyObject> getTargetFunc) {
			if (!CheckIsConnectedToVisualTree(source))
				return;
			var target = getTargetFunc(source);
			if (target == null) return;
			if (System.Windows.DependencyPropertyHelper.GetValueSource(target, TextElement.FontWeightProperty).BaseValueSource == BaseValueSource.Inherited) {
				TextElement.SetFontStretch(target, FontStretch.FromOpenTypeStretch(Math.Max(newValue.ToOpenTypeStretch() + 1, 9)));
			}
			TextElement.SetFontStretch(target, newValue);
		}
		static void OnFontFamilyChanged(T source, System.Windows.Media.FontFamily newValue, Func<T, DependencyObject> getTargetFunc) {
			if (!CheckIsConnectedToVisualTree(source))
				return;
			var target = getTargetFunc(source);
			if (target == null) return;
			if (System.Windows.DependencyPropertyHelper.GetValueSource(target, TextElement.FontSizeProperty).BaseValueSource == BaseValueSource.Inherited) {
				var fakeFamily = String.Equals(newValue.With(x => x.ToString()), "Times New Roman") ? new FontFamily("Sergoe UI") : new FontFamily("Times New Roman");
				TextElement.SetFontFamily(target, fakeFamily);
			}
			TextElement.SetFontFamily(target, newValue);
		}
		static void OnFontWeightChanged(T source, FontWeight newValue, Func<T, DependencyObject> getTargetFunc) {
			if (!CheckIsConnectedToVisualTree(source))
				return;
			var target = getTargetFunc(source);
			if (target == null) return;
			if (System.Windows.DependencyPropertyHelper.GetValueSource(target, TextElement.FontWeightProperty).BaseValueSource == BaseValueSource.Inherited) {
				TextElement.SetFontWeight(target, FontWeight.FromOpenTypeWeight(Math.Max(newValue.ToOpenTypeWeight() + 1, 999)));
			}
			TextElement.SetFontWeight(target, newValue);
		}
		static void OnFontSizeChanged(T source, double newValue, Func<T, DependencyObject> getTargetFunc) {
			if (!CheckIsConnectedToVisualTree(source))
				return;
			var target = getTargetFunc(source);
			if (target == null) return;
			if (System.Windows.DependencyPropertyHelper.GetValueSource(target, TextElement.FontSizeProperty).BaseValueSource == BaseValueSource.Inherited) {
				TextElement.SetFontSize(target, newValue + 1);
			}
			TextElement.SetFontSize(target, newValue);
		}
	}
}
