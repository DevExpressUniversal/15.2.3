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
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Native;
#if !SL
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Windows.Shapes;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Mvvm;
using System.Linq.Expressions;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using DevExpress.Xpf.Utils.Themes;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using IInputElement = System.Windows.UIElement;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Mvvm;
using System.Linq.Expressions;
#endif
namespace DevExpress.Xpf.Editors.Internal {
	public class HColorToZColorConverter : IValueConverter {
		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new HSBColor((int)value, 100, 100).Color;
		}
		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ColorViewModelValueChangedEventArgs : EventArgs {
		public Color Color { get; private set; }
		public ColorViewModelValueChangedEventArgs(Color color) {
			Color = color;
		}
	}
	public abstract class ColorBase : BindableBase {
		int a = 255;
		Color color;
		EditMode editMode;
		protected readonly Locker updateColorLocker = new Locker();
		public int A { get { return a; } set { SetProperty(ref a, value, () => A, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public Color Color { get { return color; } set { SetProperty(ref color, value, () => Color, OnColorChanged); } }
		public EditMode EditMode { get { return editMode; } set { SetProperty(ref editMode, value, () => EditMode); } }
		public static IEnumerable<LookUpEditEnumItem<ColorPickerColorMode>> ComboBoxItemsSource { get; private set; }
		static ColorBase() {
			ComboBoxItemsSource = new List<LookUpEditEnumItem<ColorPickerColorMode>> {
				new LookUpEditEnumItem<ColorPickerColorMode> { Text = EditorLocalizer.GetString(EditorStringId.RGB), Value = ColorPickerColorMode.RGB },
				new LookUpEditEnumItem<ColorPickerColorMode> { Text = EditorLocalizer.GetString(EditorStringId.CMYK), Value = ColorPickerColorMode.CMYK },
				new LookUpEditEnumItem<ColorPickerColorMode> { Text = EditorLocalizer.GetString(EditorStringId.HLS), Value = ColorPickerColorMode.HLS },
				new LookUpEditEnumItem<ColorPickerColorMode> { Text = EditorLocalizer.GetString(EditorStringId.HSB), Value = ColorPickerColorMode.HSB }
			};
		}
		protected virtual void OnColorChanged() {
			updateColorLocker.DoLockedActionIfNotLocked(UpdateValue);
			if (ColorChanged != null)
				ColorChanged(this, new ColorViewModelValueChangedEventArgs(Color));
		}
		public ColorPickerColorMode ColorMode { get; protected set; }
		public event EventHandler<ColorViewModelValueChangedEventArgs> ColorChanged;
		protected abstract void UpdateValue();
		protected abstract void UpdateColor();
	}
	public class HSBColor : ColorBase {
		int h;
		int s;
		int b;
		public int H { get { return h; } set { SetProperty(ref h, value, () => H, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int S { get { return s; } set { SetProperty(ref s, value, () => S, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int B { get { return b; } set { SetProperty(ref b, value, () => B, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public HSBColor(Color color) {
			ColorMode = ColorPickerColorMode.HSB;
			Color = color;
		}
		public HSBColor(int h, int s, int b) : this(h, s, b, 255) { }
		public HSBColor(int h, int s, int b, int a) {
			ColorMode = ColorPickerColorMode.HSB;
			this.h = h;
			this.s = s;
			this.b = b;
			this.A = a;
			updateColorLocker.DoLockedActionIfNotLocked(UpdateColor);
		}
		protected override void UpdateColor() {
			Color result = new Color();
			double r = 0d, g = 0d, b = 0d;
			int hi = Convert.ToInt32(Math.Floor(H / 60d));
			if (hi > 5)
				hi = 0;
			double bMin = (100d - S) * B / 100d;
			double a = (B - bMin) * (H % 60d) / 60d;
			double bInc = bMin + a;
			double bDec = B - a;
			switch (hi) {
				case 0:
					r = B; g = bInc; b = bMin;
					break;
				case 1:
					r = bDec; g = B; b = bMin;
					break;
				case 2:
					r = bMin; g = B; b = bInc;
					break;
				case 3:
					r = bMin; g = bDec; b = B;
					break;
				case 4:
					r = bInc; g = bMin; b = B;
					break;
				case 5:
					r = B; g = bMin; b = bDec;
					break;
			}
			result.R = Convert.ToByte(r * 2.55d);
			result.G = Convert.ToByte(g * 2.55d);
			result.B = Convert.ToByte(b * 2.55d);
			result.A = Convert.ToByte(A);
			Color = result;
		}
		protected override void UpdateValue() {
			double r = Color.R / 255d;
			double g = Color.G / 255d;
			double b = Color.B / 255d;
			double max = Math.Max(r, Math.Max(g, b));
			double min = Math.Min(r, Math.Min(g, b));
			if (max.AreClose(min))
				H = 0;
			else if (max.AreClose(r) && g.GreaterThanOrClose(b))
				H = Convert.ToInt32(60d * (g - b) / (max - min));
			else if (max.AreClose(r) && g.LessThan(b))
				H = Convert.ToInt32(60d * (g - b) / (max - min) + 360d);
			else if (max.AreClose(g))
				H = Convert.ToInt32(60d * (b - r) / (max - min) + 120d);
			else if (max.AreClose(b))
				H = Convert.ToInt32(60d * (r - g) / (max - min) + 240d);
			S = max.IsZero() ? 0 : Convert.ToInt32((1d - (min / max)) * 100d);
			B = Convert.ToByte(max * 100d);
			A = Color.A;
		}
	}
	public class RGBColor : ColorBase {
		int r;
		int g;
		int b;
		public int R { get { return r; } set { SetProperty(ref r, value, () => R, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int G { get { return g; } set { SetProperty(ref g, value, () => G, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int B { get { return b; } set { SetProperty(ref b, value, () => B, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public RGBColor(Color color) {
			ColorMode = ColorPickerColorMode.RGB;
			this.Color = color;
		}
		public RGBColor(int r, int g, int b) : this(r, g, b, 255) { }
		public RGBColor(int r, int g, int b, int a) {
			ColorMode = ColorPickerColorMode.RGB;
			this.r = r;
			this.g = g;
			this.b = b;
			this.A = a;
			updateColorLocker.DoLockedActionIfNotLocked(UpdateColor);
		}
		protected override void UpdateColor() {
			Color = new Color() { R = Convert.ToByte(R), G = Convert.ToByte(G), B = Convert.ToByte(B), A = Convert.ToByte(A) };
		}
		protected override void UpdateValue() {
			R = Color.R;
			G = Color.G;
			B = Color.B;
			A = Color.A;
		}
	}
	public class CMYKColor : ColorBase {
		int c;
		int m;
		int y;
		int k;
		public int C { get { return c; } set { SetProperty(ref c, value, () => C, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int M { get { return m; } set { SetProperty(ref m, value, () => M, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int Y { get { return y; } set { SetProperty(ref y, value, () => Y, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int K { get { return k; } set { SetProperty(ref k, value, () => K, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public CMYKColor(Color color) {
			ColorMode = ColorPickerColorMode.CMYK;
			Color = color;
		}
		public CMYKColor(int c, int m, int y, int k) : this(c, m, y, k, 255) { }
		public CMYKColor(int c, int m, int y, int k, int a) {
			ColorMode = ColorPickerColorMode.CMYK;
			this.c = c;
			this.m = m;
			this.y = y;
			this.k = k;
			this.A = a;
			updateColorLocker.DoLockedActionIfNotLocked(UpdateColor);
		}
		protected override void UpdateColor() {
			Color result = new Color();
			result.R = Convert.ToByte((1d - C / 100d) * (1d - K / 100d) * 255d);
			result.G = Convert.ToByte((1d - M / 100d) * (1d - K / 100d) * 255d);
			result.B = Convert.ToByte((1d - Y / 100d) * (1d - K / 100d) * 255d);
			result.A = Convert.ToByte(A);
			Color = result;
		}
		protected override void UpdateValue() {
			double r = Color.R / 255d;
			double g = Color.G / 255d;
			double b = Color.B / 255d;
			double k = 1d - Math.Max(r, Math.Max(g, b));
			double c = k.AreClose(1d) ? 0d : (1d - r - k) / (1d - k);
			double m = k.AreClose(1d) ? 0d : (1d - g - k) / (1d - k);
			double y = k.AreClose(1d) ? 0d : (1d - b - k) / (1d - k);
			K = Convert.ToInt32(100d * k);
			C = Convert.ToInt32(100d * c);
			M = Convert.ToInt32(100d * m);
			Y = Convert.ToInt32(100d * y);
			A = Color.A;
		}
	}
	public class HLSColor : ColorBase {
		int h;
		int l;
		int s;
		public int H { get { return h; } set { SetProperty(ref h, value, () => H, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int L { get { return l; } set { SetProperty(ref l, value, () => L, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public int S { get { return s; } set { SetProperty(ref s, value, () => S, () => updateColorLocker.DoLockedActionIfNotLocked(UpdateColor)); } }
		public HLSColor(Color color) {
			ColorMode = ColorPickerColorMode.HLS;
			Color = color;
		}
		public HLSColor(int h, int l, int s) : this(h, l, s, 255) { }
		public HLSColor(int h, int l, int s, int a) {
			ColorMode = ColorPickerColorMode.HLS;
			this.h = h;
			this.l = l;
			this.s = s;
			this.A = a;
			updateColorLocker.DoLockedActionIfNotLocked(UpdateColor);
		}
		protected override void UpdateColor() {
			Color result = new Color();
			double s = S / 100d;
			double l = L / 100d;
			double q = l.LessThan(0.5d) ? l * (1d + s) : (l + s) - l * s;
			double p = 2d * l - q;
			double hk = H / 360d;
			result.R = Convert.ToByte(HLSColorCalc(hk + 1.0d / 3d, p, q) * 255d);
			result.G = Convert.ToByte(HLSColorCalc(hk, p, q) * 255d);
			result.B = Convert.ToByte(HLSColorCalc(hk - 1d / 3d, p, q) * 255d);
			result.A = Convert.ToByte(A);
			Color = result;
		}
		protected override void UpdateValue() {
			double r = Color.R / 255d;
			double g = Color.G / 255d;
			double b = Color.B / 255d;
			double max = Math.Max(r, Math.Max(g, b));
			double min = Math.Min(r, Math.Min(g, b));
			if (max.AreClose(min))
				H = 0;
			else if (max.AreClose(r) && g.GreaterThanOrClose(b))
				H = Convert.ToInt32(60d * (g - b) / (max - min));
			else if (max.AreClose(r) && g.LessThan(b))
				H = Convert.ToInt32(60d * (g - b) / (max - min) + 360d);
			else if (max.AreClose(g))
				H = Convert.ToInt32(60d * (b - r) / (max - min) + 120d);
			else if (max.AreClose(b))
				H = Convert.ToInt32(60d * (r - g) / (max - min) + 240d);
			L = Convert.ToByte((max + min) * 50d);
			S = max.AreClose(min) ? (byte)0 : Convert.ToByte(((max - min) / (1 - Math.Abs(1 - (max + min)))) * 100d);
			A = Color.A;
		}
		double HLSColorCalc(double c, double p, double q) {
			if (c.LessThan(0d))
				c += 1d;
			if (c.GreaterThan(1d))
				c -= 1d;
			if (c.LessThan(1d / 6d))
				return p + ((q - p) * 6d * c);
			if (c.LessThan(0.5) && c.GreaterThan(1d / 6d))
				return q;
			if (c.GreaterThan(0.5) && c.LessThan(2d / 3d))
				return p + ((q - p) * (2d / 3d - c) * 6d);
			return p;
		}
	}
}
