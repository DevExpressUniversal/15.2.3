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

using System.Globalization;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Validation.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public static class Text2ColorHelper {
		public static readonly Color DefaultColor = Colors.Black;
		public static object Convert(object value) {
			if (!(value is Color))
				return DefaultColor;
			Color val = (Color)value;
			return val.ToString(CultureInfo.InvariantCulture).ToUpperInvariant();
		}
		public static bool TryConvert(object value, out Color result) {
			result = DefaultColor;
			string val = value as string;
			if (string.IsNullOrWhiteSpace(val))
				return false;
			val = val.Replace("#", "");
			if (!IsNumber(val)) {
				try {
#if !SL
					result = (Color)System.Windows.Media.ColorConverter.ConvertFromString(val);
#else
					result = ColorConverter.ConvertFromString(val);
#endif
					return true;
				}
				catch {
				}
			}
			if (val.Length > 8)
				return false;
			if (val.Length == 3)
				val = string.Format("FF{0}{0}{1}{1}{2}{2}", val[0], val[1], val[2]);
			else if (val.Length == 4)
				val = string.Format("{0}{0}{1}{1}{2}{2}{3}{3}", val[0], val[1], val[2], val[3]);
			else if (val.Length == 6)
				val = "FF" + val;
			if (val.Length != 8)
				return false;
			try {
				byte a = System.Convert.ToByte(val.Substring(0, 2), 16);
				byte r = System.Convert.ToByte(val.Substring(2, 2), 16);
				byte g = System.Convert.ToByte(val.Substring(4, 2), 16);
				byte b = System.Convert.ToByte(val.Substring(6, 2), 16);
				result = Color.FromArgb(a, r, g, b);
				return true;
			}
			catch {
			}
			return false;
		}
		public static Color ConvertBack(object value) {
			Color result;
			TryConvert(value, out result);
			return result;
		}
		static bool IsNumber(string value) {
			uint temp;
			return uint.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out temp) && value.Length <= 8;
		}
	}
}
