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
using System.Text;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Export.Xl {
	#region XlColorType
	public enum XlColorType {
		Empty,
		Rgb,
		Theme,
		Auto
	}
	#endregion
	#region XlThemeColor
	public enum XlThemeColor {
		None = -1,
		Light1 = 0,
		Dark1 = 1,
		Light2 = 2,
		Dark2 = 3,
		Accent1 = 4,
		Accent2 = 5,
		Accent3 = 6,
		Accent4 = 7,
		Accent5 = 8,
		Accent6 = 9,
		Hyperlink = 10,
		FollowedHyperlink = 11
	}
	#endregion
	#region XlColor
	public class XlColor {
		#region Statics
		static readonly XlColor empty = new XlColor(XlColorType.Empty);
		static readonly XlColor auto = new XlColor(XlColorType.Auto);
		static Dictionary<XlThemeColor, Color> themeColorTable = CreateThemeColorTable();
		static Dictionary<XlThemeColor, Color> CreateThemeColorTable() {
			Dictionary<XlThemeColor, Color> result = new Dictionary<XlThemeColor, Color>();
			result.Add(XlThemeColor.Light1, DXColor.FromArgb(0xff, 0xff, 0xff, 0xff));
			result.Add(XlThemeColor.Dark1, DXColor.FromArgb(0xff, 0x00, 0x00, 0x00));
			result.Add(XlThemeColor.Light2, DXColor.FromArgb(0xff, 0xEE, 0xEC, 0xE1));
			result.Add(XlThemeColor.Dark2, DXColor.FromArgb(0xff, 0x1F, 0x49, 0x7D));
			result.Add(XlThemeColor.Accent1, DXColor.FromArgb(0xff, 0x4F, 0x81, 0xBD));
			result.Add(XlThemeColor.Accent2, DXColor.FromArgb(0xff, 0xC0, 0x50, 0x4D));
			result.Add(XlThemeColor.Accent3, DXColor.FromArgb(0xff, 0x9B, 0xBB, 0x59));
			result.Add(XlThemeColor.Accent4, DXColor.FromArgb(0xff, 0x80, 0x64, 0xA2));
			result.Add(XlThemeColor.Accent5, DXColor.FromArgb(0xff, 0x4B, 0xAC, 0xC6));
			result.Add(XlThemeColor.Accent6, DXColor.FromArgb(0xff, 0xF7, 0x96, 0x46));
			result.Add(XlThemeColor.Hyperlink, DXColor.FromArgb(0xff, 0x00, 0x00, 0xFF));
			result.Add(XlThemeColor.FollowedHyperlink, DXColor.FromArgb(0xff, 0x80, 0x00, 0x80));
			return result;
		}
		static Dictionary<XlThemeColor, Color> theme2013ColorTable = CreateTheme2013ColorTable();
		static Dictionary<XlThemeColor, Color> CreateTheme2013ColorTable() {
			Dictionary<XlThemeColor, Color> result = new Dictionary<XlThemeColor, Color>();
			result.Add(XlThemeColor.Light1, DXColor.FromArgb(0xff, 0xff, 0xff, 0xff));
			result.Add(XlThemeColor.Dark1, DXColor.FromArgb(0xff, 0x00, 0x00, 0x00));
			result.Add(XlThemeColor.Light2, DXColor.FromArgb(0xff, 0xE7, 0xE6, 0xE6));
			result.Add(XlThemeColor.Dark2, DXColor.FromArgb(0xff, 0x44, 0x54, 0x6A));
			result.Add(XlThemeColor.Accent1, DXColor.FromArgb(0xff, 0x5B, 0x9B, 0xD5));
			result.Add(XlThemeColor.Accent2, DXColor.FromArgb(0xff, 0xED, 0x7D, 0x31));
			result.Add(XlThemeColor.Accent3, DXColor.FromArgb(0xff, 0xA5, 0xA5, 0xA5));
			result.Add(XlThemeColor.Accent4, DXColor.FromArgb(0xff, 0xff, 0xC0, 0x00));
			result.Add(XlThemeColor.Accent5, DXColor.FromArgb(0xff, 0x44, 0x72, 0xC4));
			result.Add(XlThemeColor.Accent6, DXColor.FromArgb(0xff, 0x70, 0xAD, 0x47));
			result.Add(XlThemeColor.Hyperlink, DXColor.FromArgb(0xff, 0x05, 0x63, 0xC1));
			result.Add(XlThemeColor.FollowedHyperlink, DXColor.FromArgb(0xff, 0x95, 0x4F, 0x72));
			return result;
		}
		#endregion
		#region Fields
		XlColorType colorType;
		int argb;
		XlThemeColor themeColor;
		double tint;
		#endregion
		protected XlColor(XlColorType colorType) {
			this.colorType = colorType;
			this.argb = 0;
			this.themeColor = XlThemeColor.None;
			this.tint = 0.0;
		}
		protected XlColor(int argb) {
			this.colorType = XlColorType.Rgb;
			this.argb = argb;
			this.themeColor = XlThemeColor.None;
			this.tint = 0.0;
		}
		protected XlColor(XlThemeColor themeColor, double tint) {
			this.colorType = XlColorType.Theme;
			this.argb = 0;
			this.themeColor = themeColor;
			this.tint = tint;
		}
		#region Properties
		public static XlColor Empty { get { return empty; } }
		public static XlColor Auto { get { return auto; } }
		public XlColorType ColorType { get { return colorType; } }
		public Color Rgb {
			get {
				if(ColorType == XlColorType.Rgb)
					return DXColor.FromArgb(argb);
				if(ColorType == XlColorType.Theme)
					return ApplyTint(themeColorTable[themeColor]);
				return DXColor.Empty;
			} 
		}
		public XlThemeColor ThemeColor { get { return themeColor; } }
		public double Tint { get { return tint; } }
		public bool IsEmpty { get { return colorType == XlColorType.Empty; } }
		public bool IsAutoOrEmpty { get { return colorType == XlColorType.Auto || colorType == XlColorType.Empty; } }
		#endregion
		internal Color ConvertToRgb(XlDocumentTheme theme) {
			if(ColorType == XlColorType.Rgb)
				return DXColor.FromArgb(argb);
			if(ColorType == XlColorType.Theme)
				return ApplyTint(theme == XlDocumentTheme.Office2010 ? themeColorTable[themeColor] : theme2013ColorTable[themeColor]);
			return DXColor.Empty;
		}
		#region Implicit conversions
		public static implicit operator XlColor(Color value) {
			if(DXColor.IsTransparentOrEmpty(value))
				return XlColor.Empty;
			int argb = (int)((uint)value.ToArgb() | 0xff000000);
			return new XlColor(argb);
		}
		public static implicit operator Color(XlColor value) {
			return value.Rgb;
		}
		#endregion
		public static XlColor FromArgb(int argb) {
			argb = (int)((uint)argb | 0xff000000);
			return new XlColor(argb);
		}
		public static XlColor FromArgb(byte red, byte green, byte blue) {
			int argb = (int)((((uint)red << 16) | ((uint)green << 8) | (uint)blue) | 0xff000000);
			return new XlColor(argb);
		}
		public static XlColor FromTheme(XlThemeColor themeColor, double tint) {
			if(themeColor == XlThemeColor.None)
				throw new ArgumentException("themeColor");
			if(tint < -1.0 || tint > 1.0)
				throw new ArgumentOutOfRangeException("tint out of range -1...1");
			return new XlColor(themeColor, tint);
		}
		public override bool Equals(object obj) {
			XlColor other = obj as XlColor;
			if(other == null)
				return false;
			if(colorType != other.colorType)
				return false;
			if(colorType == XlColorType.Rgb)
				return argb == other.argb;
			if(colorType == XlColorType.Theme)
				return themeColor == other.themeColor && tint == other.tint;
			return true;
		}
		public override int GetHashCode() {
			int result = ColorType.GetHashCode();
			if(colorType == XlColorType.Rgb)
				result = result ^ argb;
			else if(colorType == XlColorType.Theme)
				result = result ^ themeColor.GetHashCode() ^ tint.GetHashCode();
			return result;
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append(colorType.ToString());
			if(colorType == XlColorType.Rgb) {
				sb.Append(": ");
				sb.Append(argb.ToString("X8"));
			}
			else if(colorType == XlColorType.Theme) {
				sb.Append(": ");
				sb.Append(themeColor.ToString());
				sb.Append(", ");
				sb.Append(tint.ToString());
			}
			return sb.ToString();
		}
		#region Utils
		Color ApplyTint(Color value) {
			if(tint == 0.0)
				return value;
			int max = Math.Max(Math.Max(value.R, value.G), value.B);
			int min = Math.Min(Math.Min(value.R, value.G), value.B);
			float hue = (max == min) ? 4f / 6f : value.GetHue() / 360f;
			float saturation = value.GetSaturation();
			float brightness = value.GetBrightness();
			if(tint < 0)
				brightness = brightness * (1 + (float)tint);
			if(tint > 0)
				brightness = brightness * (1 - (float)tint) + (float)tint;
			float value1 = (brightness < 0.5) ? brightness * (1 + saturation) : brightness + saturation - brightness * saturation;
			float value2 = 2 * brightness - value1;
			float[] colorRGB = new float[] { hue + 1f / 3f, hue, hue - 1f / 3f };
			for(int i = 0; i < 3; i++) {
				if(colorRGB[i] < 0)
					colorRGB[i] += 1;
				if(colorRGB[i] > 1)
					colorRGB[i] -= 1;
				if(6 * colorRGB[i] < 1)
					colorRGB[i] = value2 + ((value1 - value2) * 6 * colorRGB[i]);
				else if(6 * colorRGB[i] >= 1 && 6 * colorRGB[i] < 3)
					colorRGB[i] = value1;
				else if(6 * colorRGB[i] >= 3 && 6 * colorRGB[i] < 4)
					colorRGB[i] = value2 + ((value1 - value2) * (4 - 6 * colorRGB[i]));
				else
					colorRGB[i] = value2;
			}
			return DXColor.FromArgb(255, ToIntValue(colorRGB[0]), ToIntValue(colorRGB[1]), ToIntValue(colorRGB[2]));
		}
		int ToIntValue(float value) {
			return FixIntValue((int)Math.Round(255 * value, 0));
		}
		int FixIntValue(int value) {
			return value < 0 ? 0 : value > 255 ? 255 : value;
		}
		#endregion
	}
	#endregion
}
