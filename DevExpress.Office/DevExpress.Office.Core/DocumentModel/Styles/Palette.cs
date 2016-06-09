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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Office.Model {
	public class Palette : ICloneable<Palette>, ISupportsCopyFrom<Palette> {
		#region Fields
		public const int BuiltInColorsCount = 8;
		public const int DefaultForegroundColorIndex = 64;
		public const int DefaultBackgroundColorIndex = 65;
		public const int SystemWindowFrameColorIndex = 66;
		public const int System3DFaceColorIndex = 67;
		public const int System3DTextColorIndex = 68;
		public const int System3DHighlightColorIndex = 69;
		public const int System3DShadowColorIndex = 70;
		public const int SystemHighlightColorIndex = 71;
		public const int SystemControlTextColorIndex = 72;
		public const int SystemControlScrollColorIndex = 73;
		public const int SystemControlInverseColorIndex = 74;
		public const int SystemControlBodyColorIndex = 75;
		public const int SystemControlFrameColorIndex = 76;
		public const int DefaultChartForegroundColorIndex = 77;
		public const int DefaultChartBackgroundColorIndex = 78;
		public const int ChartNeutralColorIndex = 79;
		public const int ToolTipFillColorIndex = 80;
		public const int ToolTipTextColorIndex = 81;
		public const int FontAutomaticColorIndex = 32767;
		Dictionary<int, Color> colorTable;
		bool isCustomIndexedColorTable;
		#endregion
		public Palette() {
			Reset();
		}
		#region Properties
		public Color this[int index] {
			get { return GetColor(index); }
			set { SetColor(index, value); }
		}
		public Color DefaultForegroundColor {
			get { return colorTable[DefaultForegroundColorIndex]; }
			set { colorTable[DefaultForegroundColorIndex] = value; }
		}
		public Color DefaultBackgroundColor {
			get { return colorTable[DefaultBackgroundColorIndex]; }
			set { colorTable[DefaultBackgroundColorIndex] = value; }
		}
		public Color DefaultChartForegroundColor {
			get { return colorTable[DefaultChartForegroundColorIndex]; }
			set { colorTable[DefaultChartForegroundColorIndex] = value; }
		}
		public Color DefaultChartBackgroundColor {
			get { return colorTable[DefaultChartBackgroundColorIndex]; }
			set { colorTable[DefaultChartBackgroundColorIndex] = value; }
		}
		public Color ChartNeutralColor { get { return colorTable[ChartNeutralColorIndex]; } }
		public Color ToolTipTextColor {
			get { return colorTable[ToolTipTextColorIndex]; }
			set { colorTable[ToolTipTextColorIndex] = value; }
		}
		public Color FontAutomaticColor {
			get { return colorTable[FontAutomaticColorIndex]; }
			set { colorTable[FontAutomaticColorIndex] = value; }
		}
		public bool IsCustomIndexedColorTable { get { return isCustomIndexedColorTable; } private set { isCustomIndexedColorTable = value; } }
		#endregion
		public void Reset() {
			colorTable = CreateDefaultColorTable();
		}
		Dictionary<int, Color> CreateDefaultColorTable() {
			Dictionary<int, Color> result = new Dictionary<int, Color>();
			result.Add(0, DXColor.FromArgb(0, 0, 0, 0));
			result.Add(1, DXColor.FromArgb(0, 255, 255, 255));
			result.Add(2, DXColor.FromArgb(0, 255, 0, 0));
			result.Add(3, DXColor.FromArgb(0, 0, 255, 0));
			result.Add(4, DXColor.FromArgb(0, 0, 0, 255));
			result.Add(5, DXColor.FromArgb(0, 255, 255, 0));
			result.Add(6, DXColor.FromArgb(0, 255, 0, 255));
			result.Add(7, DXColor.FromArgb(0, 0, 255, 255));
			result.Add(8, DXColor.FromArgb(0, 0, 0, 0));
			result.Add(9, DXColor.FromArgb(0, 255, 255, 255));
			result.Add(10, DXColor.FromArgb(0, 255, 0, 0));
			result.Add(11, DXColor.FromArgb(0, 0, 255, 0));
			result.Add(12, DXColor.FromArgb(0, 0, 0, 255));
			result.Add(13, DXColor.FromArgb(0, 255, 255, 0));
			result.Add(14, DXColor.FromArgb(0, 255, 0, 255));
			result.Add(15, DXColor.FromArgb(0, 0, 255, 255));
			result.Add(16, DXColor.FromArgb(0, 128, 0, 0));
			result.Add(17, DXColor.FromArgb(0, 0, 128, 0));
			result.Add(18, DXColor.FromArgb(0, 0, 0, 128));
			result.Add(19, DXColor.FromArgb(0, 128, 128, 0));
			result.Add(20, DXColor.FromArgb(0, 128, 0, 128));
			result.Add(21, DXColor.FromArgb(0, 0, 128, 128));
			result.Add(22, DXColor.FromArgb(0, 192, 192, 192));
			result.Add(23, DXColor.FromArgb(0, 128, 128, 128));
			result.Add(24, DXColor.FromArgb(0, 153, 153, 255));
			result.Add(25, DXColor.FromArgb(0, 153, 51, 102));
			result.Add(26, DXColor.FromArgb(0, 255, 255, 204));
			result.Add(27, DXColor.FromArgb(0, 204, 255, 255));
			result.Add(28, DXColor.FromArgb(0, 102, 0, 102));
			result.Add(29, DXColor.FromArgb(0, 255, 128, 128));
			result.Add(30, DXColor.FromArgb(0, 0, 102, 204));
			result.Add(31, DXColor.FromArgb(0, 204, 204, 255));
			result.Add(32, DXColor.FromArgb(0, 0, 0, 128));
			result.Add(33, DXColor.FromArgb(0, 255, 0, 255));
			result.Add(34, DXColor.FromArgb(0, 255, 255, 0));
			result.Add(35, DXColor.FromArgb(0, 0, 255, 255));
			result.Add(36, DXColor.FromArgb(0, 128, 0, 128));
			result.Add(37, DXColor.FromArgb(0, 128, 0, 0));
			result.Add(38, DXColor.FromArgb(0, 0, 128, 128));
			result.Add(39, DXColor.FromArgb(0, 0, 0, 255));
			result.Add(40, DXColor.FromArgb(0, 0, 204, 255));
			result.Add(41, DXColor.FromArgb(0, 204, 255, 255));
			result.Add(42, DXColor.FromArgb(0, 204, 255, 204));
			result.Add(43, DXColor.FromArgb(0, 255, 255, 153));
			result.Add(44, DXColor.FromArgb(0, 153, 204, 255));
			result.Add(45, DXColor.FromArgb(0, 255, 153, 204));
			result.Add(46, DXColor.FromArgb(0, 204, 153, 255));
			result.Add(47, DXColor.FromArgb(0, 255, 204, 153));
			result.Add(48, DXColor.FromArgb(0, 51, 102, 255));
			result.Add(49, DXColor.FromArgb(0, 51, 204, 204));
			result.Add(50, DXColor.FromArgb(0, 153, 204, 0));
			result.Add(51, DXColor.FromArgb(0, 255, 204, 0));
			result.Add(52, DXColor.FromArgb(0, 255, 153, 0));
			result.Add(53, DXColor.FromArgb(0, 255, 102, 0));
			result.Add(54, DXColor.FromArgb(0, 102, 102, 153));
			result.Add(55, DXColor.FromArgb(0, 150, 150, 150));
			result.Add(56, DXColor.FromArgb(0, 0, 51, 102));
			result.Add(57, DXColor.FromArgb(0, 51, 153, 102));
			result.Add(58, DXColor.FromArgb(0, 0, 51, 0));
			result.Add(59, DXColor.FromArgb(0, 51, 51, 0));
			result.Add(60, DXColor.FromArgb(0, 153, 51, 0));
			result.Add(61, DXColor.FromArgb(0, 153, 51, 102));
			result.Add(62, DXColor.FromArgb(0, 51, 51, 153));
			result.Add(63, DXColor.FromArgb(0, 51, 51, 51));
			result.Add(SystemWindowFrameColorIndex, DXSystemColors.WindowFrame);
			result.Add(System3DFaceColorIndex, DXSystemColors.Control);
			result.Add(System3DTextColorIndex, DXSystemColors.ControlText);
			result.Add(System3DHighlightColorIndex, DXSystemColors.ControlLight);
			result.Add(System3DShadowColorIndex, DXSystemColors.ControlDark);
			result.Add(SystemHighlightColorIndex, DXSystemColors.Highlight);
			result.Add(SystemControlTextColorIndex, DXSystemColors.ControlText);
			Color sbc = DXSystemColors.ScrollBar;
			result.Add(SystemControlScrollColorIndex, sbc);
			result.Add(SystemControlInverseColorIndex, DXColor.FromArgb(0, (byte)~sbc.R, (byte)~sbc.G, (byte)~sbc.B));
			result.Add(SystemControlBodyColorIndex, DXSystemColors.Window);
			result.Add(SystemControlFrameColorIndex, DXSystemColors.WindowFrame);
			result.Add(DefaultForegroundColorIndex, DXSystemColors.WindowText);
			result.Add(DefaultBackgroundColorIndex, DXSystemColors.Window);
			result.Add(DefaultChartForegroundColorIndex, DXColor.FromArgb(0, 0, 0, 0));
			result.Add(DefaultChartBackgroundColorIndex, DXColor.FromArgb(0, 255, 255, 255));
			result.Add(ChartNeutralColorIndex, DXColor.FromArgb(0, 0, 0, 0));
			result.Add(ToolTipFillColorIndex, DXSystemColors.Info);
			result.Add(ToolTipTextColorIndex, DXSystemColors.InfoText);
			result.Add(FontAutomaticColorIndex, DXColor.Empty);
			return result;
		}
		Color GetColor(int index) {
			Color color;
			if (!colorTable.TryGetValue(index, out color))
				Exceptions.ThrowInternalException();
			return color;
		}
		void SetColor(int index, Color value) {
			if (colorTable.ContainsKey(index)) {
				if (colorTable[index] != value) {
					IsCustomIndexedColorTable = true;
					colorTable[index] = value;
				}
			}
			else
				Exceptions.ThrowInternalException();
		}
		public bool IsValidColorIndex(int index) {
			return colorTable.ContainsKey(index);
		}
		public int GetColorIndex(ThemeDrawingColorCollection colors, ColorModelInfoCache cache, int index, bool foreground) {
			if (!cache.IsIndexValid(index))
				return foreground ? DefaultForegroundColorIndex : DefaultBackgroundColorIndex;
			ColorModelInfo colorInfo = cache[index];
			ColorModelInfo defaultItem = cache.DefaultItem;
			if (Object.ReferenceEquals(defaultItem, colorInfo))
				return foreground ? DefaultForegroundColorIndex : DefaultBackgroundColorIndex;
			if (colorInfo.ColorType == ColorType.Auto)
				return foreground ? DefaultForegroundColorIndex : DefaultBackgroundColorIndex;
			if (colorInfo.ColorType == ColorType.Index) {
				if (!IsValidColorIndex(colorInfo.ColorIndex))
					return foreground ? DefaultForegroundColorIndex : DefaultBackgroundColorIndex;
				if (colorInfo.ColorIndex <= DefaultBackgroundColorIndex)
					return colorInfo.ColorIndex;
			}
			Color color = colorInfo.ToRgb(this, colors);
			return GetPaletteNearestColorIndex(color);
		}
		public int GetColorIndex( ThemeDrawingColorCollection colors, ColorModelInfo colorInfo, bool foreground) {
			if (colorInfo.ColorType == ColorType.Auto)
				return foreground ? DefaultForegroundColorIndex : DefaultBackgroundColorIndex;
			if (colorInfo.ColorType == ColorType.Index) {
				if (!IsValidColorIndex(colorInfo.ColorIndex))
					return foreground ? DefaultForegroundColorIndex : DefaultBackgroundColorIndex;
				if (colorInfo.ColorIndex >= Palette.BuiltInColorsCount)
					return colorInfo.ColorIndex;
			}
			Color color = colorInfo.ToRgb(this, colors);
			return GetPaletteNearestColorIndex(color);
		}
		public int GetFontColorIndex( ThemeDrawingColorCollection colors, ColorModelInfo colorInfo) {
			if (colorInfo.ColorType == ColorType.Auto)
				return FontAutomaticColorIndex;
			if (colorInfo.ColorType == ColorType.Index) {
				if (!IsValidColorIndex(colorInfo.ColorIndex))
					return FontAutomaticColorIndex;
				return colorInfo.ColorIndex;
			}
			Color color = colorInfo.ToRgb(this, colors);
			return GetPaletteNearestColorIndex(color);
		}
		public int GetColorIndex(Color color) {
			int index = GetExactColorIndex(color, 0, FontAutomaticColorIndex);
			if (index != -1)
				return index;
			if (color.A == 0xFF) {
				color = DXColor.FromArgb(0, color.R, color.G, color.B);
				index = GetExactColorIndex(color, 0, FontAutomaticColorIndex);
				if (index != -1)
					return index;
			}
			return DefaultForegroundColorIndex;
		}
		int GetExactColorIndex(Color color, int minIndex, int maxIndex) {
			foreach (KeyValuePair<int, Color> pair in colorTable) {
				if (pair.Key < minIndex) continue;
				if (pair.Key > maxIndex) continue;
				if (pair.Value == color)
					return pair.Key;
			}
			return -1;
		}
		#region ColorComparison (real epic fail)
		public int GetNearestColorIndex(Color color) {
			int nearest = GetExactColorIndex(color, 0, 63);
			if (nearest != -1) return nearest;
			return GetNearestColorIndexCore(color, 0, 63);
		}
		public int GetPaletteNearestColorIndex(Color color) {
			int nearest = GetExactColorIndex(color, 8, 63);
			if (nearest != -1) return nearest;
			return GetNearestColorIndexCore(color, 8, 63);
		}
		class ColorDistanceInfo : IComparable<ColorDistanceInfo> {
			public double Distance { get; set; }
			public int ColorIndex { get; set; }
			#region IComparable<ColorDistanceInfo> Members
			public int CompareTo(ColorDistanceInfo other) {
				if (Distance > other.Distance) return 1;
				if (Distance < other.Distance) return -1;
				return 0;
			}
			#endregion
		}
		int GetNearestColorIndexCore(Color color, int minIndex, int maxIndex) {
			List<ColorDistanceInfo> items = new List<ColorDistanceInfo>();
			foreach (KeyValuePair<int, Color> pair in colorTable) {
				if (pair.Key < minIndex) continue;
				if (pair.Key > maxIndex) continue;
				if (IsCompatibleColors(pair.Value, color)) {
					ColorDistanceInfo item = new ColorDistanceInfo();
					item.Distance = GetColorDistance(color, pair.Value, 3.0);
					item.ColorIndex = pair.Key;
					items.Add(item);
				}
			}
			items.Sort();
			const int limit = 5;
			if (items.Count > limit)
				items.RemoveRange(limit, items.Count - limit);
			int nearest = -1;
			double distance = double.MaxValue;
			foreach (ColorDistanceInfo item in items) {
				if (nearest == -1) {
					nearest = item.ColorIndex;
					distance = GetColorDistance(color, colorTable[item.ColorIndex], 1.5);
				}
				else {
					double d = GetColorDistance(color, colorTable[item.ColorIndex], 1.5);
					if (d < distance) {
						nearest = item.ColorIndex;
						distance = d;
					}
				}
			}
			return nearest;
		}
		double GetColorDistance(Color x, Color y, double rgbWeight) {
			double hsbD = ColorDifference.HSB(x, y);
			double rgbD = ColorDifference.RGB(x, y) * rgbWeight;
			return hsbD + rgbD;
		}
		bool IsCompatibleColors(Color x, Color y) {
			bool xIsGray = x.R == x.G && x.R == x.B;
			bool yIsGray = y.R == y.G && y.R == y.B;
			return xIsGray == yIsGray;
		}
		#endregion
		#region ICloneable<Palette> Members
		public Palette Clone() {
			Palette result = new Palette();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<Palette> Members
		public void CopyFrom(Palette value) {
			this.colorTable = new Dictionary<int, Color>();
			foreach (KeyValuePair<int, Color> pair in value.colorTable)
				this.colorTable.Add(pair.Key, pair.Value);
		}
		#endregion
	}
	#region Color converters and difference calc
	public static class ColorDifference {
		public static double RGB(Color x, Color y) {
			double deltaR = ((double)x.R - (double)y.R) / 255;
			double deltaG = ((double)x.G - (double)y.G) / 255;
			double deltaB = ((double)x.B - (double)y.B) / 255;
			return Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB);
		}
		public static double HSB(Color x, Color y) {
			double deltaH = Math.Abs(x.GetHue() - y.GetHue());
			if (deltaH > 180.0)
				deltaH = 360.0 - deltaH;
			deltaH /= 57.3;
			double deltaB = Math.Abs(x.GetBrightness() - y.GetBrightness()) * 3.0;
			double deltaS = Math.Abs(x.GetSaturation() - y.GetSaturation()) * 1.5;
			return deltaB + deltaH + deltaS;
		}
	}
	public static class CieXYZConverter {
		static double CLinear(double c) {
			if (c <= 0.04045) return c / 12.92;
			return Math.Pow((c + 0.055) / 1.055, 2.4);
		}
		public static double GetX(Color color) {
			double r = CLinear((double)color.R / 255.0);
			double g = CLinear((double)color.G / 255.0);
			double b = CLinear((double)color.B / 255.0);
			return 0.4124 * r + 0.3876 * g + 0.1805 * b;
		}
		public static double GetY(Color color) {
			double r = CLinear((double)color.R / 255.0);
			double g = CLinear((double)color.G / 255.0);
			double b = CLinear((double)color.B / 255.0);
			return 0.2126 * r + 0.7152 * g + 0.0722 * b;
		}
		public static double GetZ(Color color) {
			double r = CLinear((double)color.R / 255.0);
			double g = CLinear((double)color.G / 255.0);
			double b = CLinear((double)color.B / 255.0);
			return 0.0193 * r + 0.1192 * g + 0.9505 * b;
		}
	}
	public static class CieLABConverter {
		static readonly double threshold = Math.Pow(6.0 / 29.0, 3.0);
		static double Func(double t) {
			if (t > threshold)
				return Math.Pow(t, 1.0 / 3.0);
			return Math.Pow(29.0 / 6.0, 2.0) * t / 3.0 + 4.0 / 29.0;
		}
		public static double GetL(Color color) {
			double y = CieXYZConverter.GetY(color);
			return 116.0 * Func(y / 3.0) - 16.0;
		}
		public static double GetA(Color color) {
			double x = CieXYZConverter.GetX(color);
			double y = CieXYZConverter.GetY(color);
			return 500.0 * (Func(x / 3.0) - Func(y / 3.0));
		}
		public static double GetB(Color color) {
			double y = CieXYZConverter.GetY(color);
			double z = CieXYZConverter.GetZ(color);
			return 200.0 * (Func(y / 3.0) - Func(z / 3.0));
		}
	}
	public static class CieColorDifference {
		public static double Cie76(Color x, Color y) {
			double dL = CieLABConverter.GetL(x) - CieLABConverter.GetL(y);
			double dA = CieLABConverter.GetA(x) - CieLABConverter.GetA(y);
			double dB = CieLABConverter.GetB(x) - CieLABConverter.GetB(y);
			return Math.Sqrt(dL * dL + dA * dA + dB * dB);
		}
		public static double Cie94(Color x, Color y) {
			double xA = CieLABConverter.GetA(x);
			double xB = CieLABConverter.GetB(x);
			double yA = CieLABConverter.GetA(y);
			double yB = CieLABConverter.GetB(y);
			double dL = CieLABConverter.GetL(x) - CieLABConverter.GetL(y);
			double dA = xA - yA;
			double dB = xB - yB;
			double C1 = Math.Sqrt(xA * xA + xB * xB);
			double C2 = Math.Sqrt(yA * yA + yB * yB);
			double dC = C1 - C2;
			double dH = Math.Sqrt(dA * dA + dB * dB - dC * dC);
			return Math.Sqrt(dL * dL + Math.Pow(dC / (1 + 0.045 * C1), 2.0) + Math.Pow(dH / (1 + 0.015 * C1), 2.0));
		}
	}
	#endregion
}
