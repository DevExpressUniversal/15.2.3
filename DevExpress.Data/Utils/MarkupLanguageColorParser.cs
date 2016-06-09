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
using System.Text;
using System.Globalization;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Data.Utils {
	public static class MarkupLanguageColorParser {
		public static Color ParseColor(string value) {
			StringBuilder colorName = new StringBuilder();
			if (value[0] == '#') {
				colorName.Append(value.Remove(0, 1));
				if (colorName.Length == 8)
					return GetColorByArgb(colorName.ToString());
				if (colorName.Length == 4) {
					colorName.Insert(0, colorName[0].ToString(), 1);
					colorName.Insert(2, colorName[2].ToString(), 1);
					colorName.Insert(4, colorName[4].ToString(), 1);
					colorName.Insert(6, colorName[6].ToString(), 1);
					return GetColorByArgb(colorName.ToString());
				}
				if (colorName.Length == 6)
					return GetColorByRgb(colorName.ToString());
				if (colorName.Length == 3) {
					colorName.Insert(0, colorName[0].ToString(), 1);
					colorName.Insert(2, colorName[2].ToString(), 1);
					colorName.Insert(4, colorName[4].ToString(), 1);
					return GetColorByRgb(colorName.ToString());
				}
				return GetColorByName(value);
			}
			if (value.Length == 6) {
				Color color = GetColorByRgb(value);
				if (color != DXColor.Empty)
					return color;
			}
			if (value.StartsWith("rgb(", StringComparison.CurrentCultureIgnoreCase)) {
				Color result = ParseRGB(value.Substring(4));
				if (result != DXColor.Empty)
					return result;
			}
			return GetColorByName(value);
		}
		static Color GetColorByName(string value) {
			Color color = DXColor.FromName(value);
#if !SL
			if (color.IsKnownColor)
				return color;
			return DXColor.Empty;
#else
			return color;
#endif
		}
		static int GetColor(string colorName, int startIndex) {
			int color;
			string sr = colorName.Substring(startIndex, 2);
			Int32.TryParse(sr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color);
			if (color == 0 && sr != "00")
				 color = -1;
			return color;
		}
		static Color GetColorByRgb(string colorName) {
			int r = GetColor(colorName, 0);
			int g = GetColor(colorName, 2);
			int b = GetColor(colorName, 4);
			if (r != -1 && g != -1 && b != -1)
				return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
			return DXColor.Empty;
		}
		static Color GetColorByArgb(string colorName) {
			int a = GetColor(colorName, 0);
			int r = GetColor(colorName, 2);
			int g = GetColor(colorName, 4);
			int b = GetColor(colorName, 6);
			if (a != -1 && r != -1 && g != -1 && b != -1)
				return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
			return DXColor.Empty;
		}
		static Color ParseRGB(string value) {
			string rgb = String.Empty;
			int color;
			List<int> colors = new List<int>();
			for (int i = 0; i < value.Length; i++) {
				if (value[i] != ',' && value[i] != ')') {
					if (!Char.IsWhiteSpace(value[i]))
						rgb += value[i];
				}
				else {
					bool isDigit = Int32.TryParse(rgb, out color);
					if (isDigit)
						colors.Add(color);
					rgb = String.Empty;
				}
			}
			if (colors.Count == 3) {
				return Color.FromArgb(255, (byte)colors[0], (byte)colors[1], (byte)colors[2]);
			}
			return DXColor.Empty;
		}
	}
}
