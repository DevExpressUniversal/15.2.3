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
#if DXPORTABLE
using DevExpress.Compatibility.System.Drawing;
#else
using System.Drawing;
#endif
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using System.Diagnostics;
namespace DevExpress.Office.Model {
	#region ColorType
	public enum ColorType {
		Rgb = 0,
		Theme = 1,
		Index = 2,
		Auto = 3
	}
	#endregion
	#region ThemeColorIndex
	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct ThemeColorIndex : IConvertToInt<ThemeColorIndex>, IComparable<ThemeColorIndex> {
		readonly int m_value;
		public static readonly ThemeColorIndex None = new ThemeColorIndex(-1);
		public static readonly ThemeColorIndex Light1 = new ThemeColorIndex(0);
		public static readonly ThemeColorIndex Dark1 = new ThemeColorIndex(1);
		public static readonly ThemeColorIndex Light2 = new ThemeColorIndex(2);
		public static readonly ThemeColorIndex Dark2 = new ThemeColorIndex(3);
		public static readonly ThemeColorIndex Accent1 = new ThemeColorIndex(4);
		public static readonly ThemeColorIndex Accent2 = new ThemeColorIndex(5);
		public static readonly ThemeColorIndex Accent3 = new ThemeColorIndex(6);
		public static readonly ThemeColorIndex Accent4 = new ThemeColorIndex(7);
		public static readonly ThemeColorIndex Accent5 = new ThemeColorIndex(8);
		public static readonly ThemeColorIndex Accent6 = new ThemeColorIndex(9);
		public static readonly ThemeColorIndex Hyperlink = new ThemeColorIndex(10);
		public static readonly ThemeColorIndex FollowedHyperlink = new ThemeColorIndex(11);
		public static readonly ThemeColorIndex Background1 = new ThemeColorIndex(12);
		public static readonly ThemeColorIndex Background2 = new ThemeColorIndex(13);
		public static readonly ThemeColorIndex Text1 = new ThemeColorIndex(14);
		public static readonly ThemeColorIndex Text2 = new ThemeColorIndex(15);
		[System.Diagnostics.DebuggerStepThrough]
		public ThemeColorIndex(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is ThemeColorIndex) && (this.m_value == ((ThemeColorIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static ThemeColorIndex operator +(ThemeColorIndex index, int value) {
			return new ThemeColorIndex(index.m_value + value);
		}
		public static int operator -(ThemeColorIndex index1, ThemeColorIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static ThemeColorIndex operator -(ThemeColorIndex index, int value) {
			return new ThemeColorIndex(index.m_value - value);
		}
		public static ThemeColorIndex operator ++(ThemeColorIndex index) {
			return new ThemeColorIndex(index.m_value + 1);
		}
		public static ThemeColorIndex operator --(ThemeColorIndex index) {
			return new ThemeColorIndex(index.m_value - 1);
		}
		public static bool operator <(ThemeColorIndex index1, ThemeColorIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(ThemeColorIndex index1, ThemeColorIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(ThemeColorIndex index1, ThemeColorIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(ThemeColorIndex index1, ThemeColorIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(ThemeColorIndex index1, ThemeColorIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(ThemeColorIndex index1, ThemeColorIndex index2) {
			return index1.m_value != index2.m_value;
		}
		public int ToInt() {
			return m_value;
		}
		#region IConvertToInt<ThemeColorSchemeIndex> Members
		[System.Diagnostics.DebuggerStepThrough]
		int IConvertToInt<ThemeColorIndex>.ToInt() {
			return m_value;
		}
		[System.Diagnostics.DebuggerStepThrough]
		ThemeColorIndex IConvertToInt<ThemeColorIndex>.FromInt(int value) {
			return new ThemeColorIndex(value);
		}
		#endregion
		#region IComparable<ThemeColorSchemeIndex> Members
		public int CompareTo(ThemeColorIndex other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region ColorModelInfo
	public class ColorModelInfo : ICloneable<ColorModelInfo>, ISupportsCopyFrom<ColorModelInfo>, ISupportsSizeOf {
		#region Static Members
		public static ColorModelInfo Create(ThemeColorIndex index) {
			return Create(index, 0);
		}
		public static ColorModelInfo Create(ThemeColorIndex index, double tint) {
			ColorModelInfo result = new ColorModelInfo();
			result.Theme = index;
			result.Tint = tint;
			return result;
		}
		public static ColorModelInfo Create(Color rgb) {
			ColorModelInfo result = new ColorModelInfo();
			result.Rgb = rgb;
			return result;
		}
		public static ColorModelInfo Create(int colorIndex) {
			ColorModelInfo result = new ColorModelInfo();
			result.ColorIndex = colorIndex;
			return result;
		}
		public static ColorModelInfo CreateAutomatic() {
			ColorModelInfo result = new ColorModelInfo();
			result.Auto = true;
			return result;
		}
		public static int ConvertColorIndex(ColorModelInfoCache sourceCache, int sourceColorIndex, ColorModelInfoCache targetCache) {
			ColorModelInfo info = sourceCache[sourceColorIndex];
			return GetColorIndex(targetCache, info);
		}
		public static int GetColorIndex(ColorModelInfoCache cache, Color color) {
			ColorModelInfo info = Create(color);
			return GetColorIndex(cache, info);
		}
		public static int GetColorIndex(ColorModelInfoCache cache, ColorModelInfo info) {
			return cache.GetItemIndex(info);
		}
		#endregion
		#region Fields
		public const int DefaultColorIndex = -1;
		ColorType colorType;
		Color rgb;
		ThemeColorIndex theme = ThemeColorIndex.None;
		int colorIndex = DefaultColorIndex;
		bool auto;
		double tint;
		#endregion
		#region Properties
		public ColorType ColorType { get { return colorType; } }
		public Color Rgb {
			get { return rgb; }
			set {
				if (ColorType != ColorType.Rgb)
					SetColorType(ColorType.Rgb);
				if (rgb != value)
					rgb = value;
			}
		}
		public ThemeColorIndex Theme {
			get { return theme; }
			set {
				if (ColorType != ColorType.Theme)
					SetColorType(ColorType.Theme);
				if (theme != value)
					theme = value;
			}
		}
		public int ColorIndex {
			get { return colorIndex; }
			set {
				if (ColorType != ColorType.Index)
					SetColorType(ColorType.Index);
				if (colorIndex != value)
					colorIndex = value;
			}
		}
		public bool Auto {
			get { return auto; }
			set {
				if (ColorType != ColorType.Auto)
					SetColorType(ColorType.Auto);
				if (auto != value)
					auto = value;
			}
		}
		public double Tint {
			get { return tint; }
			set {
				if (Math.Abs(value) > 1)
					Exceptions.ThrowInternalException();
				tint = value;
			}
		}
		public bool IsEmpty { get { return DXColor.IsTransparentOrEmpty(Rgb) && ColorType == ColorType.Rgb; } }
		#endregion
		void SetColorType(ColorType colorType) {
			RestoreDefaultValues();
			this.colorType = colorType;
		}
		void RestoreDefaultValues() {
			this.theme = ThemeColorIndex.None;
			this.colorIndex = DefaultColorIndex;
			this.auto = false;
			this.rgb = DXColor.Empty;
		}
		public Color ToRgb(Palette palette, ThemeDrawingColorCollection colors) {
			Color color = DXColor.Empty;
			switch (colorType) {
				case ColorType.Index:
					if (ColorIndex != DefaultColorIndex) {
						color = palette[ColorIndex];
						if (color.A == 0)
							color = DXColor.FromArgb(255, color.R, color.G, color.B);
					}
					break;
				case ColorType.Theme:
					if (Theme != ThemeColorIndex.None)
						color = colors.GetColor(Theme);
					break;
				case ColorType.Rgb:
					color = Rgb;
					break;
			}
			return ColorHSL.CalculateColorRGB(color, tint);
		}
		public ColorModelInfo Clone() {
			ColorModelInfo result = new ColorModelInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ColorModelInfo value) {
			this.colorType = value.colorType;
			this.rgb = value.rgb;
			this.theme = value.theme;
			this.colorIndex = value.colorIndex;
			this.auto = value.auto;
			this.tint = value.Tint;
		}
		public override bool Equals(object obj) {
			ColorModelInfo info = obj as ColorModelInfo;
			if (info == null)
				return false;
			return
				 this.colorType == info.colorType &&
				 this.rgb == info.Rgb &&
				 this.theme == info.Theme &&
				 this.colorIndex == info.ColorIndex &&
				 this.auto == info.auto &&
				 this.tint == info.Tint;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(
				(int)colorType, rgb.GetHashCode(), theme.GetHashCode(), colorIndex, auto.GetHashCode(), tint.GetHashCode());
		}
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
#if DEBUGTEST
		public static bool CheckDefaults2(ColorModelInfo info) {
			bool result = true;
			result &= ColorType.Rgb == info.ColorType;
			result &= DXColor.Empty == info.Rgb;
			result &= ThemeColorIndex.None == info.Theme;
			result &= ColorModelInfo.DefaultColorIndex == info.ColorIndex;
			result &= !info.Auto;
			result &= 0 == info.Tint;
			return result;
		}
#endif
	}
	#endregion
	#region ColorModelInfoCache
	public class ColorModelInfoCache : UniqueItemsCache<ColorModelInfo> {
		public const int DefaultItemIndex = 0;
		public ColorModelInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
			UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		protected override ColorModelInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new ColorModelInfo();
		}
#if DEBUGTEST
		public static bool CheckDefaults(ColorModelInfoCache collection) {
			bool result = true;
			result &= collection != null;
			result &= collection.Count > 0;
			result &= ColorModelInfo.CheckDefaults2(collection.DefaultItem);
			return result;
		}
#endif
	}
	#endregion
	#region ColorHSL
	public struct ColorHSL {
		#region Static Members
		readonly static ColorHSL defaultValue = new ColorHSL(0, 0, 0);
		public static ColorHSL DefaultValue { get { return defaultValue; } }
		public static ColorHSL FromColorRGB(Color color) {
			int max = Math.Max(Math.Max(color.R, color.G), color.B);
			int min = Math.Min(Math.Min(color.R, color.G), color.B);
			float hue = (max == min) ? 4f / 6f : color.GetHue() / 360f;
			return new ColorHSL(hue, color.GetSaturation(), color.GetBrightness());
		}
		public static Color CalculateColorRGB(Color color, double tint) {
			if (color == DXColor.Empty || tint == 0)
				return color;
			return FromColorRGB(color).ApplyTint(tint).ToRgb();
		}
		#endregion
		#region Fields
		const float MaxAngle = DrawingValueConstants.MaxPositiveFixedAngle;
		const float MaxThousandthOfPercentage = DrawingValueConstants.ThousandthOfPercentage;
		float hue;
		float saturation;
		float luminance;
		#endregion
		public ColorHSL(float hue, float saturation, float luminance) {
			System.Diagnostics.Debug.Assert(hue <= 1 && hue >= 0);
			System.Diagnostics.Debug.Assert(saturation <= 1 && saturation >= 0);
			System.Diagnostics.Debug.Assert(luminance <= 1 && luminance >= 0);
			this.hue = hue;
			this.saturation = saturation;
			this.luminance = luminance;
		}
		public ColorHSL(int hue, int saturation, int luminance)
			: this(hue / MaxAngle, saturation / MaxThousandthOfPercentage, luminance / MaxThousandthOfPercentage) {
		}
		#region Properties
		public int Hue { get { return GetIntValue(hue, MaxAngle); } set { hue = GetFloatValue(GetValidValue(value), MaxAngle); } }
		public int Saturation { get { return GetIntValue(saturation, MaxThousandthOfPercentage); } set { saturation = GetFloatValue(GetValidValue(value), MaxThousandthOfPercentage); } }
		public int Luminance { get { return GetIntValue(luminance, MaxThousandthOfPercentage); } set { luminance = GetFloatValue(GetValidValue(value), MaxThousandthOfPercentage); } }
		public float FloatHue { get { return hue; } set { hue = GetValidValue(value); } }
		public float FloatSaturation { get { return saturation; } set { saturation = GetValidValue(value); } }
		public float FloatLuminance { get { return luminance; } set { luminance = GetValidValue(value); } }
		#endregion
		float GetValidValue(float value) {
			return (value > 1) ? 1 : (value < 0) ? 0 : value;
		}
		int GetIntValue(float value, float maxValue) {
			return (int)Math.Round(value * maxValue);
		}
		float GetFloatValue(float value, float maxValue) {
			return value / maxValue;
		}
		public Color ToRgb() {
			float value1 = (luminance < 0.5) ? luminance * (1 + saturation) : luminance + saturation - luminance * saturation;
			float value2 = 2 * luminance - value1;
			float[] colorRGB = new float[] { hue + 1f / 3f, hue, hue - 1f / 3f };
			for (int i = 0; i < 3; i++) {
				if (colorRGB[i] < 0)
					colorRGB[i] += 1;
				if (colorRGB[i] > 1)
					colorRGB[i] -= 1;
				if (6 * colorRGB[i] < 1)
					colorRGB[i] = value2 + ((value1 - value2) * 6 * colorRGB[i]);
				else if (6 * colorRGB[i] >= 1 && 6 * colorRGB[i] < 3)
					colorRGB[i] = value1;
				else if (6 * colorRGB[i] >= 3 && 6 * colorRGB[i] < 4)
					colorRGB[i] = value2 + ((value1 - value2) * (4 - 6 * colorRGB[i]));
				else
					colorRGB[i] = value2;
			}
			return DXColor.FromArgb(ToIntValue(colorRGB[0]), ToIntValue(colorRGB[1]), ToIntValue(colorRGB[2]));
		}
		int ToIntValue(float value) {
			return FixIntValue((int)Math.Round(255 * value, 0));
		}
		int FixIntValue(int value) {
			return value < 0 ? 0 : value > 255 ? 255 : value;
		}
		ColorHSL ApplyTint(double tint) {
			if (tint < 0)
				luminance = luminance * (1 + (float)tint);
			if (tint > 0)
				luminance = luminance * (1 - (float)tint) + (float)tint;
			return this;
		}
		public ColorHSL GetComplementColor() {
			hue += hue > 0.5 ? -0.5f : 0.5f;
			return this;
		}
		public ColorHSL ApplyHue(int value) {
			FloatHue = value / MaxAngle;
			return this;
		}
		public ColorHSL ApplyHueMod(int value) {
			hue = hue * value / MaxThousandthOfPercentage;
			FixHue();
			return this;
		}
		public ColorHSL ApplyHueOffset(int value) {
			hue = hue + value / MaxAngle;
			FixHue();
			return this;
		}
		void FixHue() {
			if (hue > 1)
				hue = hue - (int)hue;
		}
		public ColorHSL ApplySaturation(int value) {
			FloatSaturation = value / MaxThousandthOfPercentage;
			return this;
		}
		public ColorHSL ApplySaturationMod(int value) {
			saturation = saturation * value / MaxThousandthOfPercentage;
			return this;
		}
		public ColorHSL ApplySaturationOffset(int value) {
			saturation = saturation + value / MaxThousandthOfPercentage;
			return this;
		}
		public ColorHSL ApplyLuminance(int value) {
			FloatLuminance = value / MaxThousandthOfPercentage;
			return this;
		}
		public ColorHSL ApplyLuminanceMod(int value) {
			luminance = luminance * value / MaxThousandthOfPercentage;
			return this;
		}
		public ColorHSL ApplyLuminanceOffset(int value) {
			luminance = luminance + value / MaxThousandthOfPercentage;
			return this;
		}
	}
	#endregion
	#region SystemColorValues
	public enum SystemColorValues {
		Sc3dDkShadow = 0x14,
		Sc3dLight = 0x13,
		ScActiveBorder = 0x15,
		ScActiveCaption = 0x06,
		ScAppWorkspace = 0x18,
		ScBackground = 0x17,
		ScBtnFace = 0x00,
		ScBtnHighlight = 0x07,
		ScBtnShadow = 0x08,
		ScBtnText = 0x09,
		ScCaptionText = 0x05,
		ScGradientActiveCaption = -2,
		ScGradientInactiveCaption = -3,
		ScGrayText = 0x0a,
		ScHighlight = 0x03,
		ScHighlightText = 0x04,
		ScHotLight = -4,
		ScInactiveBorder = 0x16,
		ScInactiveCaption = 0x0b,
		ScInactiveCaptionText = 0x0c,
		ScInfoBk = 0x0d,
		ScInfoText = 0x0e,
		ScMenu = 0x02,
		ScMenuBar = -5,
		ScMenuHighlight = -6,
		ScMenuText = 0x0f,
		ScScrollBar = 0x10,
		ScWindow = 0x11,
		ScWindowFrame = 0x12,
		ScWindowText = 0x01,
		Empty = -1
	}
	#endregion
	#region SchemeColorValues
	public enum SchemeColorValues {
		Accent1,
		Accent2,
		Accent3,
		Accent4,
		Accent5,
		Accent6,
		Background1,
		Background2,
		Dark1,
		Dark2,
		FollowedHyperlink,
		Hyperlink,
		Light1,
		Light2,
		Style, 
		Text1,
		Text2,
		Empty
	}
	#endregion
	#region ScRGBColor
	public struct ScRGBColor {
		#region Static members
		readonly static ScRGBColor defaultValue = new ScRGBColor(0, 0, 0);
		public static ScRGBColor DefaultValue { get { return defaultValue; } }
		public static Color ToRgb(int scR, int scG, int scB) {
			ScRGBColor scRgb = new ScRGBColor(scR, scG, scB);
			return scRgb.ToRgb();
		}
		#endregion
		#region Fields
		int scR;
		int scG;
		int scB;
		#endregion
		public ScRGBColor(int scR, int scG, int scB) {
			this.scR = scR;
			this.scG = scG;
			this.scB = scB;
		}
		#region Properties
		public int ScR { get { return scR; } set { scR = GetValidValue(value); } }
		public int ScG { get { return scG; } set { scG = GetValidValue(value); } }
		public int ScB { get { return scB; } set { scB = GetValidValue(value); } }
		#endregion
		int GetValidValue(int value) {
			return (value < 0) ? 0 : value;
		}
		public Color ToRgb() {
			double r = scR * 1f / DrawingValueConstants.ThousandthOfPercentage;
			double g = scG * 1f / DrawingValueConstants.ThousandthOfPercentage;
			double b = scB * 1f / DrawingValueConstants.ThousandthOfPercentage;
			const double a = 0.055;
			r = (r <= 0.0031308) ? 12.92 * r : (1 + a) * Math.Pow(r, 1 / 2.4) - a;
			g = (g <= 0.0031308) ? 12.92 * g : (1 + a) * Math.Pow(g, 1 / 2.4) - a;
			b = (b <= 0.0031308) ? 12.92 * b : (1 + a) * Math.Pow(b, 1 / 2.4) - a;
			r *= 255;
			g *= 255;
			b *= 255;
			return DXColor.FromArgb(Convert.ToInt32(r), Convert.ToInt32(g), Convert.ToInt32(b));
		}
	}
	#endregion
}
