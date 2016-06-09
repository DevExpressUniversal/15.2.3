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
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public static class ColorHelper {
		struct ColorDescription {
			public string Name;
			public string Value;
			public ColorDescription(string name, string value) {
				Name = name;
				Value = value;
			}
			public static int Compare(ColorDescription value1, ColorDescription value2) {
				return value1.Name.CompareTo(value2.Name);
			}
		}
		static ColorDescription[] defaultColors = new ColorDescription[] {
			new ColorDescription("Transparent", "#00FFFFFF"),
			new ColorDescription("Snow", "#FFFFFAFA"),
			new ColorDescription("GhostWhite", "#FFF8F8FF"),
			new ColorDescription("WhiteSmoke", "#FFF5F5F5"),
			new ColorDescription("Gainsboro", "#FFDCDCDC"),
			new ColorDescription("FloralWhite", "#FFFFFAF0"),
			new ColorDescription("OldLace", "#FFFDF5E6"),
			new ColorDescription("Linen", "#FFFAF0E6"),
			new ColorDescription("AntiqueWhite", "#FFFAEBD7"),
			new ColorDescription("PapayaWhip", "#FFFFEFD5"),
			new ColorDescription("BlanchedAlmond", "#FFFFEBCD"),
			new ColorDescription("Bisque", "#FFFFE4C4"),
			new ColorDescription("PeachPuff", "#FFFFDAB9"),
			new ColorDescription("NavajoWhite", "#FFFFDEAD"),
			new ColorDescription("Moccasin", "#FFFFE4B5"),
			new ColorDescription("Cornsilk", "#FFFFF8DC"),
			new ColorDescription("Ivory", "#FFFFFFF0"),
			new ColorDescription("LemonChiffon", "#FFFFFACD"),
			new ColorDescription("Seashell", "#FFFFF5EE"),
			new ColorDescription("Honeydew", "#FFF0FFF0"),
			new ColorDescription("MintCream", "#FFF5FFFA"),
			new ColorDescription("DeepBlue", "#FFF0FFFF"),
			new ColorDescription("AliceBlue", "#FFF0F8FF"),
			new ColorDescription("Lavender", "#FFE6E6FA"),
			new ColorDescription("LavenderBlush", "#FFFFF0F5"),
			new ColorDescription("MistyRose", "#FFFFE4E1"),
			new ColorDescription("White", "#FFFFFFFF"),
			new ColorDescription("Black", "#FF000000"),
			new ColorDescription("DarkSlateGray", "#FF2F4F4F"),
			new ColorDescription("DimGray", "#FF696969"),
			new ColorDescription("SlateGray", "#FF708090"),
			new ColorDescription("LightSlateGray", "#FF778899"),
			new ColorDescription("DarkGray", "#FFA9A9A9"),
			new ColorDescription("Gray", "#FF808080"),
			new ColorDescription("Silver", "#FFC0C0C0"),
			new ColorDescription("LightGray", "#FFD3D3D3"),
			new ColorDescription("DarkSlateBlue", "#FF483D8B"),
			new ColorDescription("SlateBlue", "#FF6A5ACD"),
			new ColorDescription("MediumSlateBlue", "#FF7B68EE"),
			new ColorDescription("MidnightBlue", "#FF191970"),
			new ColorDescription("Navy", "#FF000080"),
			new ColorDescription("DarkBlue", "#FF00008B"),
			new ColorDescription("MediumBlue", "#FF0000CD"),
			new ColorDescription("Blue", "#FF0000FF"),
			new ColorDescription("RoyalBlue", "#FF4169E1"),
			new ColorDescription("CornflowerBlue", "#FF6495ED"),
			new ColorDescription("DodgerBlue", "#FF1E90FF"),
			new ColorDescription("DeepSkyBlue", "#FF00BFFF"),
			new ColorDescription("SkyBlue", "#FF87CEEB"),
			new ColorDescription("LightSkyBlue", "#FF87CEFA"),
			new ColorDescription("SteelBlue", "#FF4682B4"),
			new ColorDescription("LightSteelBlue", "#FFB0C4DE"),
			new ColorDescription("LightBlue", "#FFADD8E6"),
			new ColorDescription("PowderBlue", "#FFB0E0E6"),
			new ColorDescription("PaleTurquoise", "#FFAFEEEE"),
			new ColorDescription("DarkTurquoise", "#FF00CED1"),
			new ColorDescription("MediumTurquoise", "#FF48D1CC"),
			new ColorDescription("Turquoise", "#FF40E0D0"),
			new ColorDescription("Aqua", "#FF00FFFF"),
			new ColorDescription("DarkCyan", "#FF008B8B"),
			new ColorDescription("Cyan", "#FF00FFFF"),
			new ColorDescription("LightCyan", "#FFE0FFFF"),
			new ColorDescription("CadetBlue", "#FF5F9EA0"),
			new ColorDescription("MediumAquamarine", "#FF66CDAA"),
			new ColorDescription("Aquamarine", "#FF7FFFD4"),
			new ColorDescription("DarkGreen", "#FF006400"),
			new ColorDescription("DarkOliveGreen", "#FF556B2F"),
			new ColorDescription("DarkSeaGreen", "#FF8FBC8F"),
			new ColorDescription("Teal", "#FF008080"),
			new ColorDescription("SeaGreen", "#FF2E8B57"),
			new ColorDescription("MediumSeaGreen", "#FF3CB371"),
			new ColorDescription("LightSeaGreen", "#FF20B2AA"),
			new ColorDescription("PaleGreen", "#FF98FB98"),
			new ColorDescription("LightGreen", "#FF90EE90"),
			new ColorDescription("SpringGreen", "#FF00FF7F"),
			new ColorDescription("LawnGreen", "#FF7CFC00"),
			new ColorDescription("Green", "#FF008000"),
			new ColorDescription("Chartreuse", "#FF7FFF00"),
			new ColorDescription("MediumSpringGreen", "#FF00FA9A"),
			new ColorDescription("GreenYellow", "#FFADFF2F"),
			new ColorDescription("Lime", "#FF00FF00"),
			new ColorDescription("LimeGreen", "#FF32CD32"),
			new ColorDescription("YellowGreen", "#FF9ACD32"),
			new ColorDescription("ForestGreen", "#FF228B22"),
			new ColorDescription("Olive", "#FF808000"),
			new ColorDescription("OliveDrab", "#FF6B8E23"),
			new ColorDescription("DarkKhaki", "#FFBDB76B"),
			new ColorDescription("Khaki", "#FFF0E68C"),
			new ColorDescription("PaleGoldenrod", "#FFEEE8AA"),
			new ColorDescription("LightGoldenrodYellow", "#FFFAFAD2"),
			new ColorDescription("LightYellow", "#FFFFFFE0"),
			new ColorDescription("Yellow", "#FFFFFF00"),
			new ColorDescription("Gold", "#FFFFD700"),
			new ColorDescription("Goldenrod", "#FFDAA520"),
			new ColorDescription("DarkGoldenrod", "#FFB8860B"),
			new ColorDescription("RosyBrown", "#FFBC8F8F"),
			new ColorDescription("IndianRed", "#FFCD5C5C"),
			new ColorDescription("SaddleBrown", "#FF8B4513"),
			new ColorDescription("Sienna", "#FFA0522D"),
			new ColorDescription("Peru", "#FFCD853F"),
			new ColorDescription("Burlywood", "#FFDEB887"),
			new ColorDescription("Beige", "#FFF5F5DC"),
			new ColorDescription("Wheat", "#FFF5DEB3"),
			new ColorDescription("SandyBrown", "#FFF4A460"),
			new ColorDescription("Tan", "#FFD2B48C"),
			new ColorDescription("Chocolate", "#FFD2691E"),
			new ColorDescription("Firebrick", "#FFB22222"),
			new ColorDescription("Brown", "#FFA52A2A"),
			new ColorDescription("DarkSalmon", "#FFE9967A"),
			new ColorDescription("Salmon", "#FFFA8072"),
			new ColorDescription("LightSalmon", "#FFFFA07A"),
			new ColorDescription("Orange", "#FFFFA500"),
			new ColorDescription("DarkOrange", "#FFFF8C00"),
			new ColorDescription("Coral", "#FFFF7F50"),
			new ColorDescription("LightCoral", "#FFF08080"),
			new ColorDescription("Tomato", "#FFFF6347"),
			new ColorDescription("OrangeRed", "#FFFF4500"),
			new ColorDescription("Red", "#FFFF0000"),
			new ColorDescription("HotPink", "#FFFF69B4"),
			new ColorDescription("DeepPink", "#FFFF1493"),
			new ColorDescription("Pink", "#FFFFC0CB"),
			new ColorDescription("LightPink", "#FFFFB6C1"),
			new ColorDescription("PaleVioletRed", "#FFDB7093"),
			new ColorDescription("Maroon", "#FF800000"),
			new ColorDescription("MediumVioletRed", "#FFC71585"),
			new ColorDescription("Magenta", "#FFFF00FF"),
			new ColorDescription("Fuchsia", "#FFFF00FF"),
			new ColorDescription("Violet", "#FFEE82EE"),
			new ColorDescription("Plum", "#FFDDA0DD"),
			new ColorDescription("Orchid", "#FFDA70D6"),
			new ColorDescription("MediumOrchid", "#FFBA55D3"),
			new ColorDescription("DarkOrchid", "#FF9932CC"),
			new ColorDescription("DarkViolet", "#FF9400D3"),
			new ColorDescription("BlueViolet", "#FF8A2BE2"),
			new ColorDescription("Purple", "#FF800080"),
			new ColorDescription("MediumPurple", "#FF9370DB"),
			new ColorDescription("Thistle", "#FFD8BFD8"),
			new ColorDescription("DarkMagenta", "#FF8B008B"),
			new ColorDescription("DarkRed", "#FF8B0000"),
			new ColorDescription("Indigo", "#FF4B0082"),
			new ColorDescription("Crimson", "#FFDC143C")
		};
		static ColorHelper() {
			Array.Sort<ColorDescription>(defaultColors, ColorDescription.Compare);
		}
		static int FindColorDescriptionIndexByName(string name) {
			int leftIndex = 0;
			int rightIndex = defaultColors.GetLength(0) - 1;
			int centerIndex = 0;
			int cmpResult = 0;
			cmpResult = defaultColors[0].Name.CompareTo(name);
			if(cmpResult > 0) return -1;
			if(cmpResult == 0) return 0;
			cmpResult = defaultColors[rightIndex].Name.CompareTo(name);
			if(cmpResult < 0) return -1;
			if(cmpResult == 0) return 0;
			do {
				if((rightIndex - leftIndex) % 2 != 0) {
					centerIndex = (rightIndex + leftIndex - 1) / 2;
				} else {
					centerIndex = (rightIndex + leftIndex) / 2;
				}
				cmpResult = defaultColors[centerIndex].Name.CompareTo(name);
				if(cmpResult == 0)
					return centerIndex;
				if(cmpResult > 0) {
					if(rightIndex == centerIndex)
						return -1;
					rightIndex = centerIndex;
				} else {
					if(leftIndex == centerIndex)
						return -1;
					leftIndex = centerIndex;
				}
			} while(leftIndex != rightIndex || leftIndex != centerIndex);
			return -1;
		}
		static public Color CreateColorFromString(string stringValue) {
			Color? val = CreateColorFromStringCore(stringValue);
			return val == null ? new Color() : val.Value;
		}
		static Color? CreateColorFromStringCore(string stringValue) {
			int colorDescriptionIndex = FindColorDescriptionIndexByName(stringValue);
			string hexValue = colorDescriptionIndex != -1 ? defaultColors[colorDescriptionIndex].Value : stringValue;
			if(!hexValue.StartsWith("#")) return null;
			if(hexValue.Length != 7 && hexValue.Length != 9) return null;
			Int32 v;
			if(!Int32.TryParse(hexValue.Substring(1), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out v))
				return null;
			Color retValue = Colors.Transparent;
			retValue.B = (byte)(v & 255);
			retValue.G = (byte)((v >> 8) & 255);
			retValue.R = (byte)((v >> 16) & 255);
			retValue.A = (hexValue.Length == 7) ? retValue.A = 0xFF : retValue.A = (byte)((v >> 24) & 255);
			return retValue;
		}
		public static bool IsValidColorStringValue(string stringValue) {
			return CreateColorFromStringCore(stringValue) != null;
		}
		public static readonly Color DefaultColor = Color.FromArgb(255, 128, 128, 128);
		static byte OverlayComponents(byte component1, byte component2) {
			if(component1 < 128)
				return MultiplyComponents(component1, component2);
			else if(component1 > 128)
				return ScreenComponents(component1, component2);
			else
				return component2;
		}
		static byte ScreenComponents(int component1, int component2) {
			int component = 255 - 2 * (255 - component1) * (255 - component2) / 255;
			return component < 0 ? byte.MinValue : (byte)component;
		}
		static byte MultiplyComponents(int component1, int component2) {
			int component = 2 * component1 * component2 / 255;
			return component > 255 ? byte.MaxValue : (byte)component;
		}
		public static Color OverlayColor(Color color1, Color color2) {
			return Color.FromArgb(MixAlpha(color1.A, color2.A),
				OverlayComponents(color1.R, color2.R),
				OverlayComponents(color1.G, color2.G),
				OverlayComponents(color1.B, color2.B));
		}
		static void NormalizeGradientBrush(GradientBrush gradientBrush) {
			for(int i = 0; i < gradientBrush.GradientStops.Count - 1; i++) {
				int maxR = Math.Max(gradientBrush.GradientStops[i].Color.R, gradientBrush.GradientStops[i + 1].Color.R);
				int minR = Math.Min(gradientBrush.GradientStops[i].Color.R, gradientBrush.GradientStops[i + 1].Color.R);
				if(minR < 128 && maxR > 128) {
					int diffR = gradientBrush.GradientStops[i].Color.R - gradientBrush.GradientStops[i + 1].Color.R;
					double diffOfset = gradientBrush.GradientStops[i + 1].Offset - gradientBrush.GradientStops[i].Offset;
					double offset = (double)(gradientBrush.GradientStops[i].Color.R - ColorHelper.DefaultColor.R) / diffR * diffOfset
						+ gradientBrush.GradientStops[i].Offset;
					int diffA = gradientBrush.GradientStops[i].Color.A - gradientBrush.GradientStops[i + 1].Color.A;
					Color color = ColorHelper.DefaultColor;
					color.A = (byte)((int)((gradientBrush.GradientStops[i].Offset - offset) / diffOfset * diffA)
						+ gradientBrush.GradientStops[i + 1].Color.A);
					gradientBrush.GradientStops.Insert(i + 1, new GradientStop() { Color = color, Offset = offset });
				}
			}
		}
		public static byte MixAlpha(byte a1, byte a2) {
			return Convert.ToByte((a1 / 255.0) * (a2 / 255.0) * 255.0);
		}
		public static Brush MixColors(Brush brush, Color targetColor) {
			if(brush == null)
				return null;
			Brush brushClone = brush.CloneCurrentValue();
			SolidColorBrush solidBrush = brushClone as SolidColorBrush;
			if(solidBrush == null) {
				GradientBrush gradientBrush = brushClone as GradientBrush;
				if(gradientBrush != null)
					foreach(GradientStop gradientStop in gradientBrush.GradientStops)
						gradientStop.Color = OverlayColor(gradientStop.Color, targetColor);
			} else
				solidBrush.Color = OverlayColor(solidBrush.Color, targetColor);
			return brushClone;
		}
		public static Brush NormalizeGradients(Brush brush) {
			Brush brushClone = brush.CloneCurrentValue();
			GradientBrush gradientBrush = brushClone as GradientBrush;
			if(gradientBrush != null) {
				NormalizeGradientBrush(gradientBrush);
				return gradientBrush;
			}
			return brushClone;
		}
		public static Color Brightness(Color c, double brightness) {
			if(brightness.IsZero()) return c;
			return brightness < 0 ? Dark(c, -brightness) : Light(c, brightness);
		}
		static Color Light(Color color, double factor) {
			double min = 0.001; double max = 1.999;
			factor = Math.Min(Math.Max(factor, 0f), 1f);
			float perc = (float)(min + factor * (max - min));
			var c = System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
			c = System.Windows.Forms.ControlPaint.Light(c, perc);
			return Color.FromArgb(color.A, c.R, c.G, c.B);
		}
		static Color Dark(Color color, double factor) {
			double min = -0.5; double max = 1;
			factor = Math.Min(Math.Max(factor, min), max);
			float perc = (float)(min + factor * (max - min));
			var c = System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
			c = System.Windows.Forms.ControlPaint.Dark(c, perc);
			return Color.FromArgb(color.A, c.R, c.G, c.B);
		}
		public static bool IsTransparent(Color c) {
			return c.A == 0;
		}
	}
}
