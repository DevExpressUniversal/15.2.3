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
namespace DevExpress.XtraEditors.Native {
	public class ChartRangeControlClientPaletteEntry {
		const string emptyColorName = "Empty";
		static Regex regex = new Regex(@"^(\d+),\s*(\d+),\s*(\d+),\s*(\d+)$", RegexOptions.Compiled);
		static Color String2Color(string colorString) {
			if (colorString == emptyColorName)
				return Color.Empty;
			Match m = regex.Match(colorString);
			if (m.Success) {
				int a = int.Parse(m.Groups[1].Value);
				int r = int.Parse(m.Groups[2].Value);
				int g = int.Parse(m.Groups[3].Value);
				int b = int.Parse(m.Groups[4].Value);
				return Color.FromArgb(a, r, g, b);
			} else
				return Color.FromName(colorString);
		}
		readonly Color color;
		readonly ChartRangeControlClientPalette palette;
		public Color Color {
			get { return color; }
		}
		internal ChartRangeControlClientPaletteEntry(ChartRangeControlClientPalette palette, string color) {
			this.palette = palette;
			this.color = String2Color(color);
		}
	}
}
