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
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Globalization;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public class DemoBaseColorHelper {
		class ColorNameComparer : IEqualityComparer<string> {
			public bool Equals(string x, string y) {
				return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
			}
			public int GetHashCode(string obj) {
				return obj.ToLowerInvariant().GetHashCode();
			}
		}
		readonly Color BadColor = Color.FromArgb(255, 255, 13, 13);
		readonly Color Transparent = Color.FromArgb(0, 0, 0, 0);
		Random random = new Random();
		List<Color> autoColorsPool = new List<Color>();
		List<Color> groupColorsPool = new List<Color>();		
		Dictionary<object, int> autoIndexes = new Dictionary<object, int>();
		Dictionary<object, Color> groupColors = new Dictionary<object, Color>();
		object groupToken = new object();
		object mutex = new object();
		public DemoBaseColorHelper(IEnumerable<string> autoColors = null, IEnumerable<string> groupColors = null) {
			string[] defaultAutoColors = new[] { "#FFC170B5", "#FF31C4B8", "#FF99C431", "#FFFF8C58", "#FFDF889C", "#FFD18B68" };
			string[] defaultGroupColors = new[] { "#FF4BA49F", "#FF5B7AAC", "#FF8578B5", "#FF4BA49F", "#FF5B7AAC", "#FF8578B5", "#FFC170B5" };
			AddPredefinedColors(autoColors ?? defaultAutoColors, autoColorsPool);
			AddPredefinedColors(groupColors ?? defaultGroupColors, groupColorsPool);
		}
		Color GetProductGroupColor(string categoryName) {
			lock(mutex) {
				Color color;
				if(!groupColors.TryGetValue(categoryName, out color)) {
					color = GetNextColor(groupToken, groupColorsPool);
					groupColors[categoryName] = color;
				}
				return color;
			}
		}
		public Color GetColor(string colorString, object autoIndexMarker) {
			lock(mutex) {
				if(string.IsNullOrEmpty(colorString)) return Transparent;
				if(colorString[0] == '#') return GetColorFromString(colorString);
				if(string.Equals(colorString, "Random", StringComparison.InvariantCultureIgnoreCase)) return GetRandomColor();
				if(string.Equals(colorString, "Auto", StringComparison.InvariantCultureIgnoreCase)) return GetNextColor(autoIndexMarker, autoColorsPool);
				return GetProductGroupColor(colorString);
			}
		}
		public Color GetColorFromString(string color) {
			if(color == null || color.Length != 9 || color[0] != '#') return BadColor;
			string a = color.Substring(1, 2);
			string r = color.Substring(3, 2);
			string g = color.Substring(5, 2);
			string b = color.Substring(7, 2);
			int alpha;
			if(!int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out alpha)) return BadColor;
			int red;
			if(!int.TryParse(r, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out red)) return BadColor;
			int green;
			if(!int.TryParse(g, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out green)) return BadColor;
			int blue;
			if(!int.TryParse(b, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out blue)) return BadColor;
			return Color.FromArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);
		}
		public Color GetNextColor(object autoIndexMarker, List<Color> colorsPool) {
			lock(mutex) {
				int autoIndex;
				if(!autoIndexes.TryGetValue(autoIndexMarker, out autoIndex)) {
					autoIndex = 0;
					autoIndexes.Add(autoIndexMarker, autoIndex);
				}
				Color color = colorsPool[autoIndex % colorsPool.Count];
				autoIndexes[autoIndexMarker] = autoIndex + 1;
				return color;
			}
		}		
		public Color GetRandomColor() {
			return Color.FromArgb(255, (byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
		}
		void AddPredefinedColors(IEnumerable<string> colors, List<Color> pool) {
			foreach(string color in colors) {
				pool.Add(GetColorFromString(color));
			}
		}
	}
}
