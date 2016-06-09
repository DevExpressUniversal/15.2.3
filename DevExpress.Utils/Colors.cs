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
using System.Drawing;
namespace DevExpress.Utils.Colors {
	#region HSL Support
	public class HueSatLight {
		public HueSatLight() {
			hueCore = 0;
			saturationCore = 0;
			lightnessCore = 0;
		}
		public HueSatLight(ColorWrapper aColor) {
			Hue = aColor.GetHue() / 360.0;
			Lightness = aColor.GetBrightness();
			Saturation = aColor.GetSaturation();
		}
		public ColorWrapper AsRGB {
			get {
				double lRed = 0, lGreen = 0, lBlue = 0;
				if(Lightness == 0) {
					lRed = lGreen = lBlue = 0;
				}
				else {
					if(Saturation == 0) {
						lRed = lGreen = lBlue = Lightness;
					}
					else {
						double temp2 = ((Lightness <= 0.5) ? Lightness * (1.0 + Saturation) : Lightness + Saturation - (Lightness * Saturation));
						double temp1 = 2.0 * Lightness - temp2;
						double[] lHueShift = new double[] { Hue + 1.0 / 3.0, Hue, Hue - 1.0 / 3.0 };
						double[] lColorArray = new double[] { 0, 0, 0 };
						for(int i = 0; i < 3; i++) {
							if(lHueShift[i] < 0)
								lHueShift[i] += 1.0;
							if(lHueShift[i] > 1)
								lHueShift[i] -= 1.0;
							if(6.0 * lHueShift[i] < 1.0)
								lColorArray[i] = temp1 + (temp2 - temp1) * lHueShift[i] * 6.0;
							else if(2.0 * lHueShift[i] < 1.0)
								lColorArray[i] = temp2;
							else if(3.0 * lHueShift[i] < 2.0)
								lColorArray[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - lHueShift[i]) * 6.0);
							else
								lColorArray[i] = temp1;
						}
						lRed = lColorArray[0];
						lGreen = lColorArray[1];
						lBlue = lColorArray[2];
					}
				}
				return ColorWrapper.FromArgb((byte)(255 * lRed), (byte)(255 * lGreen), (byte)(255 * lBlue));
			}
		}
		double hueCore;
		public double Hue {
			get { return hueCore; }
			set {
				if(value > 1)
					hueCore = 1;
				else if(value < 0)
					hueCore = 0;
				else
					hueCore = value;
			}
		}
		double saturationCore;
		public double Saturation {
			get { return saturationCore; }
			set {
				if(value > 1)
					saturationCore = 1;
				else if(value < 0)
					saturationCore = 0;
				else
					saturationCore = value;
			}
		}
		double lightnessCore;
		public double Lightness {
			get { return lightnessCore; }
			set {
				if(value > 1)
					lightnessCore = 1;
				else if(value < 0)
					lightnessCore = 0;
				else
					lightnessCore = value;
			}
		}
	}
	#endregion
	#region HSB Support
	public struct HueSatBright {
		double _Hue, _Saturation, _Brightness, _Transparency;
		public HueSatBright(double hue, double saturation, double brightness) {
			_Transparency = 0;
			_Hue = InBounds(hue, 0, 1.0);
			_Saturation = InBounds(saturation, 0, 1.0);
			_Brightness = InBounds(brightness, 0, 1.0);
		}
		static double InBounds(double value, double low, double high) {
			return Math.Min(Math.Max(value, low), high);
		}
		static int InBoundsInt(double value, double low, double high) {
			return (int)Math.Round(Math.Min(Math.Max(value, low), high));
		}
		public HueSatBright(int alpha, double hue, double saturation, double brightness) {
			_Transparency = 1.0 - alpha;
			_Hue = InBounds(hue, 0, 1.0);
			_Saturation = InBounds(saturation, 0, 1.0);
			_Brightness = InBounds(brightness, 0, 1.0);
		}
		public HueSatBright(Color color) {
			HueSatBright temp = FromColor(color);
			_Transparency = temp._Transparency;
			_Hue = temp._Hue;
			_Saturation = temp._Saturation;
			_Brightness = temp._Brightness;
		}
		public double Hue {
			get { return _Hue; }
			set { _Hue = value; }
		}
		public double Saturation {
			get { return _Saturation; }
			set { _Saturation = value; }
		}
		public double Brightness {
			get { return _Brightness; }
			set { _Brightness = value; }
		}
		public double Alpha {
			get { return 1.0 - _Transparency; }
			set { _Transparency = 1.0 - value; }
		}
		public Color AsRGB { get { return FromHSB(this); } }
		static Color FromHSB(HueSatBright hsbColor) {
			double brightness = hsbColor._Brightness * 255.0;
			double red = brightness;
			double green = brightness;
			double blue = brightness;
			double saturation = hsbColor._Saturation;
			if(saturation != 0) {
				double max = brightness;
				double dif = brightness * saturation;
				double min = brightness - dif;
				double hue = hsbColor._Hue * 360f;
				if(hue < 60f) {
					red = max;
					green = hue * dif / 60f + min;
					blue = min;
				}
				else if(hue < 120f) {
					red = -(hue - 120f) * dif / 60f + min;
					green = max;
					blue = min;
				}
				else if(hue < 180f) {
					red = min;
					green = max;
					blue = (hue - 120f) * dif / 60f + min;
				}
				else if(hue < 240f) {
					red = min;
					green = -(hue - 240f) * dif / 60f + min;
					blue = max;
				}
				else if(hue < 300f) {
					red = (hue - 240f) * dif / 60f + min;
					green = min;
					blue = max;
				}
				else if(hue <= 360f) {
					red = max;
					green = min;
					blue = -(hue - 360f) * dif / 60 + min;
				}
				else {
					red = 0;
					green = 0;
					blue = 0;
				}
			}
			return Color.FromArgb(InBoundsInt(255.0 * hsbColor.Alpha, 0, 255), InBoundsInt(red, 0, 255), InBoundsInt(green, 0, 255), InBoundsInt(blue, 0, 255));
		}
		public static HueSatBright FromColor(Color color) {
			HueSatBright ret = new HueSatBright(0f, 0f, 0f);
			ret._Transparency = 1.0 - color.A / 255.0;
			double r = color.R;
			double g = color.G;
			double b = color.B;
			double max = Math.Max(r, Math.Max(g, b));
			if(max <= 0) {
				return ret;
			}
			double min = Math.Min(r, Math.Min(g, b));
			double dif = max - min;
			if(max > min) {
				if(g == max) {
					ret._Hue = (b - r) / dif * 60f + 120f;
				}
				else if(b == max) {
					ret._Hue = (r - g) / dif * 60f + 240f;
				}
				else if(b > g) {
					ret._Hue = (g - b) / dif * 60f + 360f;
				}
				else {
					ret._Hue = (g - b) / dif * 60f;
				}
				if(ret._Hue < 0) {
					ret._Hue = ret._Hue + 360f;
				}
			}
			else {
				ret._Hue = 0;
			}
			ret._Hue /= 360f;
			ret._Saturation = (dif / max);
			ret._Brightness = max / 255.0;
			return ret;
		}
	}
	#endregion
	#region Color Converter
	public class ColorConverter {
		public static Color FromHueSatLight(HueSatLight hsl) {
			return FromHueSatBrightCore(hsl.Hue, hsl.Saturation, hsl.Lightness);
		}
		public static HueSatLight FromColor(Color rgb) {
			double hue, sat, light;
			FromColorCore(rgb, out hue, out sat, out light);
			return new HueSatLight() { Hue = hue, Saturation = sat, Lightness = light };
		}
		static Color FromHueSatBrightCore(double hue, double saturation, double lightness) {
			double red = lightness, green = lightness, blue = lightness, temp;
			temp = (lightness <= 0.5) ? (lightness * (1.0 + saturation)) : (lightness + saturation - lightness * saturation);
			if(temp > 0) {
				int sextant;
				double m, sv, fract, vsf, mid1, mid2;
				m = lightness + lightness - temp;
				sv = (temp - m) / temp;
				hue *= 6.0;
				sextant = (int)hue;
				fract = hue - sextant;
				vsf = temp * sv * fract;
				mid1 = m + vsf;
				mid2 = temp - vsf;
				switch(sextant) {
					case 0:
						red = temp;
						green = mid1;
						blue = m;
						break;
					case 1:
						red = mid2;
						green = temp;
						blue = m;
						break;
					case 2:
						red = m;
						green = temp;
						blue = mid1;
						break;
					case 3:
						red = m;
						green = mid2;
						blue = temp;
						break;
					case 4:
						red = mid1;
						green = m;
						blue = temp;
						break;
					case 5:
						red = temp;
						green = m;
						blue = mid2;
						break;
				}
			}
			return Color.FromArgb((byte)(red * 255.0f), (byte)(green * 255.0f), (byte)(blue * 255.0f));
		}
		static void FromColorCore(Color rgb, out double hue, out double sat, out double light) {
			double red = rgb.R / 255.0, green = rgb.G / 255.0, blue = rgb.B / 255.0, v, m, vm, r2, g2, b2;
			hue = sat = light = 0;
			v = Math.Max(red, green);
			v = Math.Max(v, blue);
			m = Math.Min(red, green);
			m = Math.Min(m, blue);
			light = (m + v) / 2.0;
			if(light <= 0.0)
				return;
			vm = v - m;
			sat = vm;
			if(sat > 0.0)
				sat /= (light <= 0.5) ? (v + m) : (2.0 - v - m);
			else return;
			r2 = (v - red) / vm;
			g2 = (v - green) / vm;
			b2 = (v - blue) / vm;
			if(red == v) hue = (green == m ? 5.0 + b2 : 1.0 - g2);
			else if(green == v) hue = (blue == m ? 1.0 + r2 : 3.0 - b2);
			else hue = (red == m ? 3.0 + g2 : 5.0 - r2);
			hue /= 6.0;
		}
		public static bool GetColorValueHex(string hexValue, out Color newColor) {
			newColor = Color.Empty;
			if(!hexValue.StartsWith("#"))
				return false;
			try {
				string hexString = hexValue.Substring(1);
				if(hexString.Length == 8) {
					string alpha = hexString.Substring(0, 2);
					string colorValue = hexString.Substring(2);
					return GetColorValueHex(alpha, colorValue, out newColor);
				}
				else if(hexString.Length == 6)
					return GetColorValueHex("FF", hexString, out newColor);
				else if(hexString.Length == 3) {
					string expandedHexValue = new string(hexString[0], 2) + new string(hexString[1], 2) + new string(hexString[2], 2);
					return GetColorValueHex("FF", expandedHexValue, out newColor);
				}
			}
			catch {
				newColor = Color.Empty;
			}
			return false;
		}
		public static bool GetColorValueHex(string alpha, string hexValue, out Color newColor) {
			newColor = Color.Empty;
			if(string.IsNullOrEmpty(alpha) || string.IsNullOrEmpty(hexValue))
				return false;
			string R = hexValue.Substring(0, 2);
			string G = hexValue.Substring(2, 2);
			string B = hexValue.Substring(4, 2);
			try {
				int alphaValue = Convert.ToInt32(alpha, 16);
				int RValue = Convert.ToInt32(R, 16);
				int GValue = Convert.ToInt32(G, 16);
				int BValue = Convert.ToInt32(B, 16);
				newColor = Color.FromArgb(alphaValue, RValue, GValue, BValue);
				return true;
			}
			catch { }
			return false;
		}
	}
	#endregion
	interface IPropertyConverter {
		object Convert(object src);
		object ConvertBack(object dst);
	}
	class ColorWrapperConverter : IPropertyConverter {
		public object Convert(object src) {
			Color color = (Color)src;
			return new ColorWrapper(color.A, color.R, color.G, color.B);
		}
		public object ConvertBack(object dst) {
			ColorWrapper wrapper = (ColorWrapper)dst;
			return Color.FromArgb(wrapper.A, wrapper.R, wrapper.G, wrapper.B);
		}
	}
	class PropertyConverterRegistry {
		Dictionary<Type, object> _Converters;
		public PropertyConverterRegistry() {
			_Converters = new Dictionary<Type, object>();
		}
		TSource TryCastToSource<TSource>(object dst) {
			try {
				return (TSource)dst;
			}
			catch(InvalidCastException) {
				return default(TSource);
			}
		}
		public void Register<TSource>(IPropertyConverter converter, bool replaceExisting) {
			Type key = typeof(TSource);
			if(_Converters.ContainsKey(key))
				if(replaceExisting)
					_Converters.Remove(key);
				else
					return;
			_Converters.Add(key, converter);
		}
		public IPropertyConverter GetConverter<TSource>() {
			return GetConverter(typeof(TSource));
		}
		public IPropertyConverter GetConverter(Type type) {
			if(type == null)
				return null;
			object value;
			if(_Converters.TryGetValue(type, out value))
				return (IPropertyConverter)value;
			Type baseType = type.BaseType;
			while(baseType != null) {
				if(_Converters.TryGetValue(baseType, out value))
					return (IPropertyConverter)value;
				baseType = baseType.BaseType;
			}
			return null;
		}
		public object Convert(object src) {
			if(src == null)
				return null;
			IPropertyConverter converter = GetConverter(src.GetType());
			if(converter == null)
				return src;
			return converter.Convert(src);
		}
		public TSource ConvertBack<TSource>(object dst) {
			IPropertyConverter converter = GetConverter<TSource>();
			if(converter == null)
				return TryCastToSource<TSource>(dst);
			return (TSource)converter.ConvertBack(dst);
		}
	}
	class PlatformEngine {
		static PlatformEngine() {
			Converters = new PropertyConverterRegistry();
			Converters.Register<Color>(new ColorWrapperConverter(), true);
		}
		public static PropertyConverterRegistry Converters { get; private set; }
		public static object Convert(object src) {
			return Converters.Convert(src);
		}
		public static TSource ConvertBack<TSource>(object dst) {
			return Converters.ConvertBack<TSource>(dst);
		}
	}
	public struct ColorWrapper {
		byte _A, _R, _G, _B;
		public ColorWrapper(byte r, byte g, byte b) : this(255, r, g, b) { }
		public ColorWrapper(byte a, byte r, byte g, byte b) {
			_A = a;
			_R = r;
			_G = g;
			_B = b;
		}
		public static T ConvertTo<T>(ColorWrapper color) {
			return PlatformEngine.ConvertBack<T>(color);
		}
		public static ColorWrapper FromArgb(byte r, byte g, byte b) {
			return FromArgb(0xff, r, g, b);
		}
		public static ColorWrapper FromArgb(byte a, byte r, byte g, byte b) {
			return new ColorWrapper(a, r, g, b);
		}
		public double GetHue() {
			if(R == G && G == B)
				return 0f;
			float rKoef = ((float)this.R) / 255f;
			float gKoef = ((float)this.G) / 255f;
			float bKoef = ((float)this.B) / 255f;
			float result = 0f;
			float maxRGB = rKoef;
			maxRGB = Math.Max(gKoef, maxRGB);
			maxRGB = Math.Max(bKoef, maxRGB);
			float minRGB = rKoef;
			minRGB = Math.Min(gKoef, minRGB);
			minRGB = Math.Min(bKoef, minRGB);
			float diff = maxRGB - minRGB;
			if(rKoef == maxRGB)
				result = (gKoef - bKoef) / diff;
			else if(gKoef == maxRGB)
				result = 2f + ((bKoef - rKoef) / diff);
			else if(bKoef == maxRGB)
				result = 4f + ((rKoef - gKoef) / diff);
			result *= 60f;
			if(result < 0f)
				result += 360f;
			return result;
		}
		public float GetSaturation() {
			float rKoef = ((float)this.R) / 255f;
			float gKoef = ((float)this.G) / 255f;
			float bKoef = ((float)this.B) / 255f;
			float result = 0f;
			float maxRGB = rKoef;
			maxRGB = Math.Max(gKoef, maxRGB);
			maxRGB = Math.Max(bKoef, maxRGB);
			float minRGB = rKoef;
			minRGB = Math.Min(gKoef, minRGB);
			minRGB = Math.Min(bKoef, minRGB);
			if(maxRGB == minRGB)
				return result;
			float brightness = (maxRGB + minRGB) / 2f;
			if(brightness <= 0.5)
				return (maxRGB - minRGB) / (maxRGB + minRGB);
			return (maxRGB - minRGB) / ((2f - maxRGB) - minRGB);
		}
		public float GetBrightness() {
			float rKoef = ((float)this.R) / 255f;
			float gKoef = ((float)this.G) / 255f;
			float bKoef = ((float)this.B) / 255f;
			float maxRGB = rKoef;
			maxRGB = Math.Max(gKoef, maxRGB);
			maxRGB = Math.Max(bKoef, maxRGB);
			float minRGB = rKoef;
			minRGB = Math.Min(gKoef, minRGB);
			minRGB = Math.Min(bKoef, minRGB);
			return (maxRGB + minRGB) / 2f;
		}
		public override bool Equals(object obj) {
			if(obj is ColorWrapper) {
				ColorWrapper color = (ColorWrapper)obj;
				return _A == color._A && _R == color._R && _G == color._G && _B == color._B;
			}
			return false;
		}
		public override int GetHashCode() {
			return _A.GetHashCode() ^ _R.GetHashCode() ^ _G.GetHashCode() ^ _B.GetHashCode();
		}
		public static bool operator ==(ColorWrapper left, ColorWrapper right) {
			return left.Equals(right);
		}
		public static bool operator !=(ColorWrapper left, ColorWrapper right) {
			return !left.Equals(right);
		}
		public override string ToString() {
			return string.Format("R={0}, G={1}, B={2}", R, G, B);
		}
		public byte A { get { return _A; } }
		public byte R { get { return _R; } }
		public byte G { get { return _G; } }
		public byte B { get { return _B; } }
	}
}
