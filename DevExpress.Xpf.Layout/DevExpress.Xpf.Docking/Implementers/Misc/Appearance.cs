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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Docking {
	public class AppearanceObject : Freezable {
		#region static
		public static readonly DependencyProperty BackgroundProperty;
		public static readonly DependencyProperty ForegroundProperty;
		public static readonly DependencyProperty FontFamilyProperty;
		public static readonly DependencyProperty FontSizeProperty;
		public static readonly DependencyProperty FontStretchProperty;
		public static readonly DependencyProperty FontStyleProperty;
		public static readonly DependencyProperty FontWeightProperty;
		public static readonly DependencyProperty TabBackgroundColorProperty;
		static List<DependencyProperty> _MergedProperties;
		internal static List<DependencyProperty> MergedProperties {
			get {
				if(_MergedProperties == null) _MergedProperties = CreateMergedPropertiesList();
				return _MergedProperties; }
		}
		static List<DependencyProperty> CreateMergedPropertiesList() {
			return new List<DependencyProperty> {
				BackgroundProperty, ForegroundProperty,
				FontFamilyProperty, FontSizeProperty, FontStretchProperty, FontStyleProperty, FontWeightProperty,
				TabBackgroundColorProperty
			};
		}
		static AppearanceObject() {
			var dProp = new DependencyPropertyRegistrator<AppearanceObject>();
			dProp.Register("Background", ref BackgroundProperty, (Brush)null,
				(dObj, e) => ((AppearanceObject)dObj).OnAppearanceObjectPropertyChanged(e));
			dProp.Register("Foreground", ref ForegroundProperty, (Brush)null,
				(dObj, e) => ((AppearanceObject)dObj).OnAppearanceObjectPropertyChanged(e));
			dProp.Register("FontFamily", ref FontFamilyProperty, (FontFamily)null,
				(dObj, e) => ((AppearanceObject)dObj).OnAppearanceObjectPropertyChanged(e));
			dProp.Register("FontSize", ref FontSizeProperty, double.NaN,
				(dObj, e) => ((AppearanceObject)dObj).OnAppearanceObjectPropertyChanged(e));
			dProp.Register("FontStretch", ref FontStretchProperty, (FontStretch?)null,
				(dObj, e) => ((AppearanceObject)dObj).OnAppearanceObjectPropertyChanged(e));
			dProp.Register("FontStyle", ref FontStyleProperty, (FontStyle?)null,
				(dObj, e) => ((AppearanceObject)dObj).OnAppearanceObjectPropertyChanged(e));
			dProp.Register("FontWeight", ref FontWeightProperty, (FontWeight?)null,
				(dObj, e) => ((AppearanceObject)dObj).OnAppearanceObjectPropertyChanged(e));
			dProp.Register("TabBackgroundColor", ref TabBackgroundColorProperty, Colors.Transparent,
				(dObj, e) => ((AppearanceObject)dObj).OnAppearanceObjectPropertyChanged(e));
		}
		protected override Freezable CreateInstanceCore() {
			return new AppearanceObject();
		}
		#endregion
		protected internal void OnAppearanceObjectPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if(Appearance != null) Appearance.OnAppearanceObjectPropertyChanged(e);
		}
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public FontFamily FontFamily {
			get { return (FontFamily)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}
		public double FontSize {
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}
		public FontStretch? FontStretch {
			get { return (FontStretch?)GetValue(FontStretchProperty); }
			set { SetValue(FontStretchProperty, value); }
		}
		public FontStyle? FontStyle {
			get { return (FontStyle?)GetValue(FontStyleProperty); }
			set { SetValue(FontStyleProperty, value); }
		}
		public FontWeight? FontWeight {
			get { return (FontWeight?)GetValue(FontWeightProperty); }
			set { SetValue(FontWeightProperty, value); }
		}
		public Color TabBackgroundColor {
			get { return (Color)GetValue(TabBackgroundColorProperty); }
			set { SetValue(TabBackgroundColorProperty, value); }
		}
		protected internal Appearance Appearance { get; set; }
	}
	public class Appearance : Freezable {
		#region static
		public static readonly DependencyProperty NormalProperty;
		public static readonly DependencyProperty ActiveProperty;
		static Appearance() {
			var dProp = new DependencyPropertyRegistrator<Appearance>();
			dProp.Register("Normal", ref NormalProperty, (AppearanceObject)null, (d, e) => ((Appearance)d).OnAppearanceObjectChanged((AppearanceObject)e.NewValue));
			dProp.Register("Active", ref ActiveProperty, (AppearanceObject)null, (d, e) => ((Appearance)d).OnAppearanceObjectChanged((AppearanceObject)e.NewValue));
		}
		#endregion static
		public Appearance() {
			Normal = new AppearanceObject();
			Active = new AppearanceObject();
		}
		protected override Freezable CreateInstanceCore() {
			return new Appearance();
		}
		public AppearanceObject Normal {
			get { return (AppearanceObject)GetValue(NormalProperty); }
			set { SetValue(NormalProperty, value); }
		}
		public AppearanceObject Active {
			get { return (AppearanceObject)GetValue(ActiveProperty); }
			set { SetValue(ActiveProperty, value); }
		}
		protected virtual void OnAppearanceObjectChanged(AppearanceObject appearanceObject) {
			if(appearanceObject != null) appearanceObject.Appearance = this;
		}
		protected internal virtual void OnAppearanceObjectPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if(Owner != null) Owner.OnAppearanceObjectPropertyChanged(e);
		}
		protected internal BaseLayoutItem Owner { get; set; }
	}
	public class AppearanceHelper {
		internal static Appearance UpdateAppearance(Appearance result, Appearance parentAppearance, Appearance appearance) {
			if(parentAppearance == null) parentAppearance = new Appearance();
			if(appearance == null) appearance = new Appearance();
			Update(result.Normal, parentAppearance.Normal, appearance.Normal);
			Update(result.Active, parentAppearance.Active, appearance.Active);
			return result;
		}
		internal static AppearanceObject Update(AppearanceObject result, AppearanceObject parentAppearance, AppearanceObject appearance) {
			var properties = AppearanceObject.MergedProperties;
			foreach(DependencyProperty property in properties) {
				object parentValue = parentAppearance.GetValue(property);
				object value = appearance.GetValue(property);
				result.SetValue(property, Merge(parentValue, value));
			}
			return result;
		}
		public static Appearance GetActualAppearance(Appearance parentAppearance, Appearance appearance) {
			if(parentAppearance == null) parentAppearance = new Appearance();
			if(appearance == null) appearance = new Appearance();
			Appearance result = new Appearance();
			result.Normal = Merge(parentAppearance.Normal, appearance.Normal);
			result.Active = Merge(parentAppearance.Active, appearance.Active);
			return result;
		}
		public static AppearanceObject Merge(AppearanceObject parentAppearance, AppearanceObject appearance) {
			AppearanceObject result = new AppearanceObject();
			var properties = AppearanceObject.MergedProperties;
			foreach(DependencyProperty property in properties) {
				object parentValue = parentAppearance.GetValue(property);
				object value = appearance.GetValue(property);
				result.SetValue(property, Merge(parentValue, value));
			}
			return result;
		}
		internal static object Merge(object parentValue, object value) {
			if(parentValue is double)
				return Merge((double)parentValue, (double)value);
			if(parentValue is Color)
				return Merge((Color)parentValue, (Color)value);
			return value ?? parentValue;
		}
		internal static object Merge(double parentValue, double value) {
			return double.IsNaN(value) ? parentValue : value;
		}
		internal static object Merge(Color parentValue, Color value) {
			return Colors.Transparent == value ? parentValue : value;
		}
	}
}
