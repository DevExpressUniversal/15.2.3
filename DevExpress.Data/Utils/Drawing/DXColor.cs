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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
using System.Globalization;
namespace DevExpress.Utils {
	#region DXColor
	public static class DXColor {
		public static readonly Color Empty = Color.Empty;
		public static readonly Color Transparent = Color.Transparent;
		public static readonly Color Black = Color.Black;
		public static readonly Color Gray = Color.Gray;
		public static readonly Color Azure = Color.Azure;
		public static readonly Color Wheat = Color.Wheat;
		public static readonly Color Bisque = Color.Bisque;
		public static readonly Color BlanchedAlmond = Color.BlanchedAlmond;
		public static readonly Color Beige = Color.Beige;
		public static readonly Color Cyan = Color.Cyan;
		public static readonly Color Red = Color.Red;
		public static readonly Color Blue = Color.Blue;
		public static readonly Color Green = Color.Green;
		public static readonly Color Yellow = Color.Yellow;
		public static readonly Color White = Color.White;
		public static readonly Color RosyBrown = Color.RosyBrown;
		public static readonly Color LightGreen = Color.LightGreen;
		public static readonly Color YellowGreen = Color.YellowGreen;
		public static readonly Color AliceBlue = Color.AliceBlue;
		public static readonly Color DimGray = Color.DimGray;
		public static readonly Color Teal = Color.Teal;
		public static readonly Color Sienna = Color.Sienna;
		public static readonly Color SaddleBrown = Color.SaddleBrown;
		public static readonly Color SeaGreen = Color.SeaGreen;
		public static readonly Color Snow = Color.Snow;
		public static readonly Color Maroon = Color.Maroon;
		public static readonly Color Aqua = Color.Aqua;
		public static readonly Color Aquamarine = Color.Aquamarine;
		public static readonly Color Silver = Color.Silver;
		public static readonly Color Magenta = Color.Magenta;
		public static readonly Color DarkBlue = Color.DarkBlue;
		public static readonly Color DarkCyan = Color.DarkCyan;
		public static readonly Color DarkGreen = Color.DarkGreen;
		public static readonly Color DarkMagenta = Color.DarkMagenta;
		public static readonly Color DarkRed = Color.DarkRed;
		public static readonly Color LightGray = Color.LightGray;
		public static readonly Color Brown = Color.Brown;
		public static readonly Color SkyBlue = Color.SkyBlue;
		public static readonly Color SteelBlue = Color.SteelBlue;
		public static readonly Color Coral = Color.Coral;
		public static Color FromArgb(int red, int green, int blue) {
			return Color.FromArgb(red, green, blue);
		}
		public static Color FromArgb(int alpha, int red, int green, int blue) {
			return Color.FromArgb(alpha, red, green, blue);
		}
		public static Color FromArgb(int argb) {
			return Color.FromArgb(argb);
		}
		public static int ToArgb(Color color) {
			return color.ToArgb();
		}
		public static Color FromOle(int oleColor) {
#if DXPORTABLE
			return FromArgb(oleColor);
#else
			return ColorTranslator.FromOle(oleColor);
#endif
		}
		public static Color FromName(string name) {
			return Color.FromName(name);
		}
		public static int ToOle(Color color) {
#if DXPORTABLE
			return ToArgb(color);
#else
			return ColorTranslator.ToOle(color);
#endif
		}
		public static string ToHtml(Color c) {
#if DXPORTABLE
			return String.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
#else
			return ColorTranslator.ToHtml(c);
#endif
		}
		public static Color FromHtml(string htmlColor) {
#if DXPORTABLE
			Color c = Color.Empty;
			if (htmlColor == null || htmlColor.Length == 0)
				return c;
			if ((htmlColor[0] == '#') && ((htmlColor.Length == 7) || (htmlColor.Length == 4))) {
				if (htmlColor.Length == 7) {
					c = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16),
									   Convert.ToInt32(htmlColor.Substring(3, 2), 16),
									   Convert.ToInt32(htmlColor.Substring(5, 2), 16));
				}
				else {
					string r = Char.ToString(htmlColor[1]);
					string g = Char.ToString(htmlColor[2]);
					string b = Char.ToString(htmlColor[3]);
					c = Color.FromArgb(Convert.ToInt32(r + r, 16),
									   Convert.ToInt32(g + g, 16),
									   Convert.ToInt32(b + b, 16));
				}
			}
			if (c.IsEmpty && String.Equals(htmlColor, "LightGrey", StringComparison.OrdinalIgnoreCase)) {
				c = Color.LightGray;
			}
			if (c.IsEmpty) {
				if (htmlSysColorTable == null) {
					InitializeHtmlSysColorTable();
				}
				htmlSysColorTable.TryGetValue(htmlColor.ToLower(CultureInfo.InvariantCulture), out c);
			}
			if (c.IsEmpty) {
				ColorConverter converter = new ColorConverter();
				c = (Color)converter.ConvertFromString(null, CultureInfo.CurrentCulture, htmlColor);
			}
			return c;
#else
			return ColorTranslator.FromHtml(htmlColor);
#endif
		}
#if DXPORTABLE
		static Dictionary<string, Color> htmlSysColorTable;
		static void InitializeHtmlSysColorTable() {
			htmlSysColorTable = new Dictionary<string, Color>(26);
			htmlSysColorTable["activeborder"] = Color.FromKnownColor(KnownColor.ActiveBorder);
			htmlSysColorTable["activecaption"] = Color.FromKnownColor(KnownColor.ActiveCaption);
			htmlSysColorTable["appworkspace"] = Color.FromKnownColor(KnownColor.AppWorkspace);
			htmlSysColorTable["background"] = Color.FromKnownColor(KnownColor.Desktop);
			htmlSysColorTable["buttonface"] = Color.FromKnownColor(KnownColor.Control);
			htmlSysColorTable["buttonhighlight"] = Color.FromKnownColor(KnownColor.ControlLightLight);
			htmlSysColorTable["buttonshadow"] = Color.FromKnownColor(KnownColor.ControlDark);
			htmlSysColorTable["buttontext"] = Color.FromKnownColor(KnownColor.ControlText);
			htmlSysColorTable["captiontext"] = Color.FromKnownColor(KnownColor.ActiveCaptionText);
			htmlSysColorTable["graytext"] = Color.FromKnownColor(KnownColor.GrayText);
			htmlSysColorTable["highlight"] = Color.FromKnownColor(KnownColor.Highlight);
			htmlSysColorTable["highlighttext"] = Color.FromKnownColor(KnownColor.HighlightText);
			htmlSysColorTable["inactiveborder"] = Color.FromKnownColor(KnownColor.InactiveBorder);
			htmlSysColorTable["inactivecaption"] = Color.FromKnownColor(KnownColor.InactiveCaption);
			htmlSysColorTable["inactivecaptiontext"] = Color.FromKnownColor(KnownColor.InactiveCaptionText);
			htmlSysColorTable["infobackground"] = Color.FromKnownColor(KnownColor.Info);
			htmlSysColorTable["infotext"] = Color.FromKnownColor(KnownColor.InfoText);
			htmlSysColorTable["menu"] = Color.FromKnownColor(KnownColor.Menu);
			htmlSysColorTable["menutext"] = Color.FromKnownColor(KnownColor.MenuText);
			htmlSysColorTable["scrollbar"] = Color.FromKnownColor(KnownColor.ScrollBar);
			htmlSysColorTable["threeddarkshadow"] = Color.FromKnownColor(KnownColor.ControlDarkDark);
			htmlSysColorTable["threedface"] = Color.FromKnownColor(KnownColor.Control);
			htmlSysColorTable["threedhighlight"] = Color.FromKnownColor(KnownColor.ControlLight);
			htmlSysColorTable["threedlightshadow"] = Color.FromKnownColor(KnownColor.ControlLightLight);
			htmlSysColorTable["window"] = Color.FromKnownColor(KnownColor.Window);
			htmlSysColorTable["windowframe"] = Color.FromKnownColor(KnownColor.WindowFrame);
			htmlSysColorTable["windowtext"] = Color.FromKnownColor(KnownColor.WindowText);
		}
#endif
		public static bool IsEmpty(Color c) {
			return c.IsEmpty;
		}
		#region PredefinedColors
		public static readonly Dictionary<string, Color> PredefinedColors = CreatePredefinedColorsTable();
		static Dictionary<string, Color> CreatePredefinedColorsTable() {
			Dictionary<string, Color> result = new Dictionary<string, Color>();
			result.Add("AliceBlue", DXColor.AliceBlue); 
			result.Add("AntiqueWhite", DXColor.FromArgb(0xFF, 0xFA, 0xEB, 0xD7));
			result.Add("Aqua", DXColor.Aqua); 
			result.Add("Aquamarine", DXColor.Aquamarine); 
			result.Add("Azure", DXColor.Azure); 
			result.Add("Beige", DXColor.Beige); 
			result.Add("Bisque", DXColor.Bisque); 
			result.Add("Black", DXColor.Black); 
			result.Add("BlanchedAlmond", DXColor.BlanchedAlmond); 
			result.Add("Blue", DXColor.Blue); 
			result.Add("BlueViolet", DXColor.FromArgb(0xFF, 0x8A, 0x2B, 0xE2));
			result.Add("Brown", DXColor.FromArgb(0xFF, 0xA5, 0x2A, 0x2A));
			result.Add("BurlyWood", DXColor.FromArgb(0xFF, 0xDE, 0xB8, 0x87));
			result.Add("CadetBlue", DXColor.FromArgb(0xFF, 0x5F, 0x9E, 0xA0));
			result.Add("Chartreuse", DXColor.FromArgb(0xFF, 0x7F, 0xFF, 0x00));
			result.Add("Chocolate", DXColor.FromArgb(0xFF, 0xD2, 0x69, 0x1E));
			result.Add("Coral", DXColor.FromArgb(0xFF, 0xFF, 0x7F, 0x50));
			result.Add("CornflowerBlue", DXColor.FromArgb(0xFF, 0x64, 0x95, 0xED));
			result.Add("Cornsilk", DXColor.FromArgb(0xFF, 0xFF, 0xF8, 0xDC));
			result.Add("Crimson", DXColor.FromArgb(0xFF, 0xDC, 0x14, 0x3C));
			result.Add("Cyan", DXColor.Cyan);
			result.Add("DarkBlue", DXColor.DarkBlue); 
			result.Add("DarkCyan", DXColor.DarkCyan); 
			result.Add("DarkGoldenrod", DXColor.FromArgb(0xFF, 0xB8, 0x86, 0x0B));
			result.Add("DarkGray", DXColor.FromArgb(0xFF, 0xA9, 0xA9, 0xA9));
			result.Add("DarkGreen", DXColor.DarkGreen); 
			result.Add("DarkKhaki", DXColor.FromArgb(0xFF, 0xBD, 0xB7, 0x6B));
			result.Add("DarkMagenta", DXColor.DarkMagenta); 
			result.Add("DarkOliveGreen", DXColor.FromArgb(0xFF, 0x55, 0x6B, 0x2F));
			result.Add("DarkOrange", DXColor.FromArgb(0xFF, 0xFF, 0x8C, 0x00));
			result.Add("DarkOrchid", DXColor.FromArgb(0xFF, 0x99, 0x32, 0xCC));
			result.Add("DarkRed", DXColor.DarkRed); 
			result.Add("DarkSalmon", DXColor.FromArgb(0xFF, 0xE9, 0x96, 0x7A));
			result.Add("DarkSeaGreen", DXColor.FromArgb(0xFF, 0x8F, 0xBC, 0x8F));
			result.Add("DarkSlateBlue", DXColor.FromArgb(0xFF, 0x48, 0x3D, 0x8B));
			result.Add("DarkSlateGray", DXColor.FromArgb(0xFF, 0x2F, 0x4F, 0x4F));
			result.Add("DarkTurquoise", DXColor.FromArgb(0xFF, 0x00, 0xCE, 0xD1));
			result.Add("DarkViolet", DXColor.FromArgb(0xFF, 0x94, 0x00, 0xD3));
			result.Add("DeepPink", DXColor.FromArgb(0xFF, 0xFF, 0x14, 0x93));
			result.Add("DeepSkyBlue", DXColor.FromArgb(0xFF, 0x00, 0xBF, 0xFF));
			result.Add("DimGray", DXColor.DimGray); 
			result.Add("DodgerBlue", DXColor.FromArgb(0xFF, 0x1E, 0x90, 0xFF));
			result.Add("FireBrick", DXColor.FromArgb(0xFF, 0xB2, 0x22, 0x22));
			result.Add("FloralWhite", DXColor.FromArgb(0xFF, 0xFF, 0xFA, 0xF0));
			result.Add("ForestGreen", DXColor.FromArgb(0xFF, 0x22, 0x8B, 0x22));
			result.Add("Fuchsia", DXColor.FromArgb(0xFF, 0xFF, 0x00, 0xFF));
			result.Add("Gainsboro", DXColor.FromArgb(0xFF, 0xDC, 0xDC, 0xDC));
			result.Add("GhostWhite", DXColor.FromArgb(0xFF, 0xF8, 0xF8, 0xFF));
			result.Add("Gold", DXColor.FromArgb(0xFF, 0xFF, 0xD7, 0x00));
			result.Add("Goldenrod", DXColor.FromArgb(0xFF, 0xDA, 0xA5, 0x20));
			result.Add("Gray", DXColor.Gray); 
			result.Add("Green", DXColor.Green); 
			result.Add("GreenYellow", DXColor.FromArgb(0xFF, 0xAD, 0xFF, 0x2F));
			result.Add("Honeydew", DXColor.FromArgb(0xFF, 0xF0, 0xFF, 0xF0));
			result.Add("HotPink", DXColor.FromArgb(0xFF, 0xFF, 0x69, 0xB4));
			result.Add("IndianRed", DXColor.FromArgb(0xFF, 0xCD, 0x5C, 0x5C));
			result.Add("Indigo", DXColor.FromArgb(0xFF, 0x4B, 0x00, 0x82));
			result.Add("Ivory", DXColor.FromArgb(0xFF, 0xFF, 0xFF, 0xF0));
			result.Add("Khaki", DXColor.FromArgb(0xFF, 0xF0, 0xE6, 0x8C));
			result.Add("Lavender", DXColor.FromArgb(0xFF, 0xE6, 0xE6, 0xFA));
			result.Add("LavenderBlush", DXColor.FromArgb(0xFF, 0xFF, 0xF0, 0xF5));
			result.Add("LawnGreen", DXColor.FromArgb(0xFF, 0x7C, 0xFC, 0x00));
			result.Add("LemonChiffon", DXColor.FromArgb(0xFF, 0xFF, 0xFA, 0xCD));
			result.Add("LightBlue", DXColor.FromArgb(0xFF, 0xAD, 0xD8, 0xE6));
			result.Add("LightCoral", DXColor.FromArgb(0xFF, 0xF0, 0x80, 0x80));
			result.Add("LightCyan", DXColor.FromArgb(0xFF, 0xE0, 0xFF, 0xFF));
			result.Add("LightGoldenrodYellow", DXColor.FromArgb(0xFF, 0xFA, 0xFA, 0xD2));
			result.Add("LightGray", DXColor.LightGray); 
			result.Add("LightGreen", DXColor.LightGreen); 
			result.Add("LightPink", DXColor.FromArgb(0xFF, 0xFF, 0xB6, 0xC1));
			result.Add("LightSalmon", DXColor.FromArgb(0xFF, 0xFF, 0xA0, 0x7A));
			result.Add("LightSeaGreen", DXColor.FromArgb(0xFF, 0x20, 0xB2, 0xAA));
			result.Add("LightSkyBlue", DXColor.FromArgb(0xFF, 0x87, 0xCE, 0xFA));
			result.Add("LightSlateGray", DXColor.FromArgb(0xFF, 0x77, 0x88, 0x99));
			result.Add("LightSteelBlue", DXColor.FromArgb(0xFF, 0xB0, 0xC4, 0xDE));
			result.Add("LightYellow", DXColor.FromArgb(0xFF, 0xFF, 0xFF, 0xE0));
			result.Add("Lime", DXColor.FromArgb(0xFF, 0x00, 0xFF, 0x00));
			result.Add("LimeGreen", DXColor.FromArgb(0xFF, 0x32, 0xCD, 0x32));
			result.Add("Linen", DXColor.FromArgb(0xFF, 0xFA, 0xF0, 0xE6));
			result.Add("Magenta", DXColor.Magenta);
			result.Add("Maroon", DXColor.Maroon); 
			result.Add("MediumAquaMarine", DXColor.FromArgb(0xFF, 0x66, 0xCD, 0xAA));
			result.Add("MediumBlue", DXColor.FromArgb(0xFF, 0x00, 0x00, 0xCD));
			result.Add("MediumOrchid", DXColor.FromArgb(0xFF, 0xBA, 0x55, 0xD3));
			result.Add("MediumPurple", DXColor.FromArgb(0xFF, 0x93, 0x70, 0xD8));
			result.Add("MediumSeaGreen", DXColor.FromArgb(0xFF, 0x3C, 0xB3, 0x71));
			result.Add("MediumSlateBlue", DXColor.FromArgb(0xFF, 0x7B, 0x68, 0xEE));
			result.Add("MediumSpringGreen", DXColor.FromArgb(0xFF, 0x00, 0xFA, 0x9A));
			result.Add("MediumTurquoise", DXColor.FromArgb(0xFF, 0x48, 0xD1, 0xCC));
			result.Add("MediumVioletRed", DXColor.FromArgb(0xFF, 0xC7, 0x15, 0x85));
			result.Add("MidnightBlue", DXColor.FromArgb(0xFF, 0x19, 0x19, 0x70));
			result.Add("MintCream", DXColor.FromArgb(0xFF, 0xF5, 0xFF, 0xFA));
			result.Add("MistyRose", DXColor.FromArgb(0xFF, 0xFF, 0xE4, 0xE1));
			result.Add("Moccasin", DXColor.FromArgb(0xFF, 0xFF, 0xE4, 0xB5));
			result.Add("NavajoWhite", DXColor.FromArgb(0xFF, 0xFF, 0xDE, 0xAD));
			result.Add("Navy", DXColor.FromArgb(0xFF, 0x00, 0x00, 0x80));
			result.Add("OldLace", DXColor.FromArgb(0xFF, 0xFD, 0xF5, 0xE6));
			result.Add("Olive", DXColor.FromArgb(0xFF, 0x80, 0x80, 0x00));
			result.Add("OliveDrab", DXColor.FromArgb(0xFF, 0x6B, 0x8E, 0x23));
			result.Add("Orange", DXColor.FromArgb(0xFF, 0xFF, 0xA5, 0x00));
			result.Add("OrangeRed", DXColor.FromArgb(0xFF, 0xFF, 0x45, 0x00));
			result.Add("Orchid", DXColor.FromArgb(0xFF, 0xDA, 0x70, 0xD6));
			result.Add("PaleGoldenrod", DXColor.FromArgb(0xFF, 0xEE, 0xE8, 0xAA));
			result.Add("PaleGreen", DXColor.FromArgb(0xFF, 0x98, 0xFB, 0x98));
			result.Add("PaleTurquoise", DXColor.FromArgb(0xFF, 0xAF, 0xEE, 0xEE));
			result.Add("PaleVioletRed", DXColor.FromArgb(0xFF, 0xD8, 0x70, 0x93));
			result.Add("PapayaWhip", DXColor.FromArgb(0xFF, 0xFF, 0xEF, 0xD5));
			result.Add("PeachPuff", DXColor.FromArgb(0xFF, 0xFF, 0xDA, 0xB9));
			result.Add("Peru", DXColor.FromArgb(0xFF, 0xCD, 0x85, 0x3F));
			result.Add("Pink", DXColor.FromArgb(0xFF, 0xFF, 0xC0, 0xCB));
			result.Add("Plum", DXColor.FromArgb(0xFF, 0xDD, 0xA0, 0xDD));
			result.Add("PowderBlue", DXColor.FromArgb(0xFF, 0xB0, 0xE0, 0xE6));
			result.Add("Purple", DXColor.FromArgb(0xFF, 0x80, 0x00, 0x80));
			result.Add("Red", DXColor.Red); 
			result.Add("RosyBrown", DXColor.RosyBrown); 
			result.Add("RoyalBlue", DXColor.FromArgb(0xFF, 0x41, 0x69, 0xE1));
			result.Add("SaddleBrown", DXColor.SaddleBrown); 
			result.Add("Salmon", DXColor.FromArgb(0xFF, 0xFA, 0x80, 0x72));
			result.Add("SandyBrown", DXColor.FromArgb(0xFF, 0xF4, 0xA4, 0x60));
			result.Add("SeaGreen", DXColor.SeaGreen); 
			result.Add("SeaShell", DXColor.FromArgb(0xFF, 0xFF, 0xF5, 0xEE));
			result.Add("Sienna", DXColor.Sienna); 
			result.Add("Silver", DXColor.Silver); 
			result.Add("SkyBlue", DXColor.FromArgb(0xFF, 0x87, 0xCE, 0xEB));
			result.Add("SlateBlue", DXColor.FromArgb(0xFF, 0x6A, 0x5A, 0xCD));
			result.Add("SlateGray", DXColor.FromArgb(0xFF, 0x70, 0x80, 0x90));
			result.Add("Snow", DXColor.Snow); 
			result.Add("SpringGreen", DXColor.FromArgb(0xFF, 0x00, 0xFF, 0x7F));
			result.Add("SteelBlue", DXColor.FromArgb(0xFF, 0x46, 0x82, 0xB4));
			result.Add("Tan", DXColor.FromArgb(0xFF, 0xD2, 0xB4, 0x8C));
			result.Add("Teal", DXColor.Teal); 
			result.Add("Thistle", DXColor.FromArgb(0xFF, 0xD8, 0xBF, 0xD8));
			result.Add("Tomato", DXColor.FromArgb(0xFF, 0xFF, 0x63, 0x47));
			result.Add("Turquoise", DXColor.FromArgb(0xFF, 0x40, 0xE0, 0xD0));
			result.Add("Transparent", DXColor.Transparent); 
			result.Add("Violet", DXColor.FromArgb(0xFF, 0xEE, 0x82, 0xEE));
			result.Add("Wheat", DXColor.Wheat); 
			result.Add("White", DXColor.White); 
			result.Add("WhiteSmoke", DXColor.FromArgb(0xFF, 0xF5, 0xF5, 0xF5));
			result.Add("Yellow", DXColor.Yellow); 
			result.Add("YellowGreen", DXColor.YellowGreen); 
			return result;
		}
		#endregion
		public static Color Blend(Color color, Color backgroundColor) {
			if (color.A >= 255)
				return DXColor.FromArgb(color.R, color.G, color.B);
			float alpha = color.A / 255.0f;
			float one_alpha = 1.0f - alpha;
			return DXColor.FromArgb(
				(int)(color.R * alpha + backgroundColor.R * one_alpha),
				(int)(color.G * alpha + backgroundColor.G * one_alpha),
				(int)(color.B * alpha + backgroundColor.B * one_alpha));
		}
		public static bool IsTransparentOrEmpty(Color color) {
			return color == DXColor.Empty || color == DXColor.Transparent;
		}
		public static Color CalculateNearestColor(ICollection<Color> colorsToChooseFrom, Color value) {
			float valueHue = value.GetHue();
			float valueBrightness = value.GetBrightness();
			float valueSaturation = value.GetSaturation();
			Color result = DXColor.Empty;
			float minDistance = float.MaxValue;
			foreach (Color color in colorsToChooseFrom) {
				float hue = Math.Abs(color.GetHue() - valueHue);
				if (hue > 180.0f)
					hue = 360.0f - hue;
				float saturation = color.GetSaturation() - valueSaturation;
				float brightness = color.GetBrightness() - valueBrightness;
				float distance = hue * hue + saturation * saturation + brightness * brightness;
				if (distance < minDistance) {
					minDistance = distance;
					result = color;
				}
			}
			return result;
		}
	}
	#endregion
	public static class DXSystemColors {
		public static Color Control { get { return SystemColors.Control; } }
		public static Color ControlDark { get { return SystemColors.ControlDark; } }
		public static Color Window { get { return SystemColors.Window; } }
		public static Color ActiveBorder { get { return SystemColors.ActiveBorder; } }
		public static Color ActiveCaption { get { return SystemColors.ActiveCaption; } }
		public static Color ActiveCaptionText { get { return SystemColors.ActiveCaptionText; } }
		public static Color AppWorkspace { get { return SystemColors.AppWorkspace; } }
		public static Color ControlDarkDark { get { return SystemColors.ControlDarkDark; } }
		public static Color ControlLight { get { return SystemColors.ControlLight; } }
		public static Color ControlLightLight { get { return SystemColors.ControlLightLight; } }
		public static Color ControlText { get { return SystemColors.ControlText; } }
		public static Color Desktop { get { return SystemColors.Desktop; } }
		public static Color GrayText { get { return SystemColors.GrayText; } }
		public static Color Highlight { get { return SystemColors.Highlight; } }
		public static Color HighlightText { get { return SystemColors.HighlightText; } }
		public static Color HotTrack { get { return SystemColors.HotTrack; } }
		public static Color InactiveBorder { get { return SystemColors.InactiveBorder; } }
		public static Color InactiveCaption { get { return SystemColors.InactiveCaption; } }
		public static Color InactiveCaptionText { get { return SystemColors.InactiveCaptionText; } }
		public static Color Info { get { return SystemColors.Info; } }
		public static Color InfoText { get { return SystemColors.InfoText; } }
		public static Color Menu { get { return SystemColors.Menu; } }
		public static Color MenuText { get { return SystemColors.MenuText; } }
		public static Color ScrollBar { get { return SystemColors.ScrollBar; } }
		public static Color WindowFrame { get { return SystemColors.WindowFrame; } }
		public static Color WindowText { get { return SystemColors.WindowText; } }
		public static Color GradientActiveCaption { get { return SystemColors.GradientActiveCaption; } }
		public static Color GradientInactiveCaption { get { return SystemColors.GradientInactiveCaption; } }
		public static Color MenuBar { get { return SystemColors.MenuBar; } }
		public static Color MenuHighlight { get { return SystemColors.MenuHighlight; } }
	}
}
