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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
namespace DevExpress.Utils.Svg {
	public class SvgPaletteDictionary : Dictionary<ObjectState, SvgPalette> {
		public void Clone(SvgPaletteDictionary source) {
			foreach(var item in source) {
				this.Add(item.Key, item.Value.Clone());
			}
		}
	}
	public class SvgColor {
		public SvgColor(string name, Color value) {
			Name = name;
			Value = value;
		}
		public string Name { get; set; }
		public Color Value { get; set; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SvgPalette : ISvgPaletteProvider {
		public static Dictionary<string, string> DefaultColors;
		static SvgPalette() {
			DefaultColors = new Dictionary<string, string>();
			DefaultColors.Add("#d04d2f", "Red");
			DefaultColors.Add("#4dae89", "Green");
			DefaultColors.Add("#377ab5", "Blue");
			DefaultColors.Add("#eeb764", "Yellow");
			DefaultColors.Add("#000000", "Black");
			DefaultColors.Add("#ffffff", "White");
		}
		public SvgPalette() {
			Colors = new List<SvgColor>();
		}
		public SvgPalette Merge(SvgPalette palette) {
			var result = Clone();
			var comparer = new SvgColorNameComparer();
			foreach(var item in palette.Colors) {
				if(!result.Colors.Contains(item, comparer))
					result.Colors.Add(item);
			}
			return result;
		}
		public override bool Equals(object obj) {
			var paletteEntry = obj as SvgPalette;
			if(paletteEntry == null) return false;
			return EqualsCore(this, paletteEntry);
		}
		public override int GetHashCode() {
			int result = int.MaxValue;
			foreach(var item in Colors) {
				result ^= item.Name.GetHashCode() ^ item.Value.GetHashCode();
			}
			return base.GetHashCode();
		}
		public static bool operator ==(SvgPalette palette1, SvgPalette palette2) {
			return EqualsCore(palette1, palette2);
		}
		static bool EqualsCore(SvgPalette palette1, SvgPalette palette2) {
			if(System.Object.ReferenceEquals(palette1, palette2))
				return true;
			if(((object)palette1 == null) || ((object)palette2 == null))
				return false;
			if(palette1.Colors.Count != palette2.Colors.Count) return false;
			return palette1.Colors.SequenceEqual(palette2.Colors, new PaletteComparer());
		}
		public static bool operator !=(SvgPalette palette1, SvgPalette palette2) {
			return !EqualsCore(palette1, palette2);
		}
		public List<SvgColor> Colors { get; set; }
		static ColorConverter converter = new ColorConverter();
		public Color GetColor(string defaultColor) {
			string colorName;
			Color emptyColor = (Color)converter.ConvertFromString(defaultColor);
			if(DefaultColors.TryGetValue(defaultColor.ToLower(), out colorName)) {
				SvgColor result = Colors.FirstOrDefault(x => x.Name == colorName);
				return result != null ? result.Value : emptyColor;
			}
			return emptyColor;
		}
		public Color GetColor(Color defaultColor) {
			string defaultColorName = ConvertToHex(defaultColor).ToLower();
			return GetColor(defaultColorName);
		}
		private static string ConvertToHex(Color color) {
			return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
		}
		ISvgPaletteProvider ISvgPaletteProvider.Clone() {
			return Clone();
		}
		bool ISvgPaletteProvider.Equals(ISvgPaletteProvider provider) {
			return EqualsCore(this as SvgPalette, provider as SvgPalette);
		}
		public SvgPalette Clone() {
			SvgPalette result = new SvgPalette();
			foreach(var item in Colors) {
				result.Colors.Add(new SvgColor(item.Name, item.Value));
			}
			return result;
		}
	}
	public class SvgColorNameComparer : IEqualityComparer<SvgColor> {
		#region IEqualityComparer<SvgColor> Members
		public bool Equals(SvgColor x, SvgColor y) {
			if(Object.ReferenceEquals(x, y)) return true;
			return string.Equals(x.Name, y.Name);
		}
		public int GetHashCode(SvgColor obj) {
			return obj.Name == null ? 0 : obj.Name.GetHashCode();
		}
		#endregion
	}
	public class PaletteComparer : IEqualityComparer<SvgColor> {
		#region IEqualityComparer<SvgColor> Members
		public bool Equals(SvgColor x, SvgColor y) {
			if(Object.ReferenceEquals(x, y)) return true;
			return string.Equals(x.Name, y.Name) && Color.Equals(x.Value, y.Value);
		}
		public int GetHashCode(SvgColor obj) {
			int hashProductName = obj.Name == null ? 0 : obj.Name.GetHashCode();
			int hashProductCode = obj.Value.GetHashCode();
			return hashProductName ^ hashProductCode;
		}
		#endregion
	}
}
