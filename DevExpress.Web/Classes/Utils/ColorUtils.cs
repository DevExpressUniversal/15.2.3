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

using System.Drawing;
using System.Text.RegularExpressions;
namespace DevExpress.Web.Internal {	
	public static class ColorUtils {
		public static string ToHexColor(Color color) {
			string hexColor = "";
			if(color != null && !color.IsEmpty)
				hexColor = string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
			return hexColor;
		}
		public static Color ValueToColor(object value) {
			Color color = Color.Empty;
			if(value != null && !TryParseColor(value.ToString(), out color))
				color = Color.Empty;
			return color;
		}
		public static bool TryParseColor(string text, out Color color) {
			color = ParseColor(text);
			return color != Color.Empty;
		}
		public static Color ParseColor(string text) {
			Color color = Color.FromName(text);
			if(!color.IsKnownColor)
				color = ColorFromHexColor(text);
			if(color == Color.Empty)
				color = ColorFromRGBColor(text);
			return color;
		}
		public static Color ColorFromHexColor(string hex) {
			Regex regExp = new Regex("^#?([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$");
			if(regExp.IsMatch(hex)) {
				hex = hex.Replace("#", "");
				if(hex.Length == 3) {
					string newHex = "";
					for(int i = 0; i < 3; i++)
						newHex += hex.Substring(i, 1) + hex.Substring(i, 1);
					hex = newHex;
				}
				byte r = 0, g = 0, b = 0;
				int start = 0;
				r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
				g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
				b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);
				return Color.FromArgb(0xFF, r, g, b);
			}
			return Color.Empty;
		}
		public static Color ColorFromRGBColor(string text) {
			Color color = Color.Empty;
			Regex regExp = new Regex(@"\s*([0-9]+)\s*,\s*([0-9]+)\s*,\s*([0-9]+)\s*");
			Match match = regExp.Match(text);
			if(match.Success)
				color = Color.FromArgb(0xFF, int.Parse(match.Groups[1].Value), 
					int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
			return color;
		}
	}
}
