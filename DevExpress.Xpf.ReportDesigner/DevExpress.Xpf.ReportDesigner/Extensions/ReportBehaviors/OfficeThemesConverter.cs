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

using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Reports.UserDesigner.Extensions {
	class ReportStyles {
		public XRControlStyle Header;
		public XRControlStyle GroupHeader;
		public XRControlStyle DetailEven;
		public XRControlStyle DetailOdd;
		public XRControlStyle GroupFooter;
		public XRControlStyle Footer;
	}
	static class OfficeThemesConverter {
		static System.Drawing.Color ToDrawingColor(System.Windows.Media.Color color) {
			return System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
		}
		static Color Brightness(Color color, float coeff) {
			if(coeff == 1F)
				return color;
			var hsl = ColorConverter.ToHSL(color);
			hsl = ColorConverter.AdjustLightness(hsl, coeff);
			return ColorConverter.ToRGB(hsl);
		}
		public static void ConvertTheme(string name, OfficeThemeColors colors, OfficeThemeFonts fonts, ReportStyles styles) {
#if DEBUG
#endif
			var shades = MakeShades(colors);
			styles.Header.BackColor = ToDrawingColor(shades.DarkColor2[1]);
			styles.Header.ForeColor = ToDrawingColor(shades.DarkColor2[0]);
			styles.Header.Font = new System.Drawing.Font(fonts.MajorFontSet.LatinTypeface, 12);
			styles.DetailEven.BackColor = ToDrawingColor(shades.LightColor1[0]);
			styles.DetailEven.ForeColor = ToDrawingColor(shades.DarkColor1[3]);
			styles.DetailEven.Font = new System.Drawing.Font(fonts.MajorFontSet.LatinTypeface, 9);
			styles.DetailOdd.BackColor = ToDrawingColor(shades.LightColor1[1]);
			styles.DetailOdd.ForeColor = ToDrawingColor(shades.DarkColor1[3]);
			styles.DetailOdd.Font = new System.Drawing.Font(fonts.MajorFontSet.LatinTypeface, 9);
			styles.GroupHeader.BackColor = ToDrawingColor(shades.DarkColor2[1]);
			styles.GroupHeader.ForeColor = ToDrawingColor(shades.DarkColor2[0]);
			styles.GroupHeader.Font = new System.Drawing.Font(fonts.MajorFontSet.LatinTypeface, 9);
			styles.Footer.BackColor = ToDrawingColor(shades.DarkColor2[1]);
			styles.Footer.ForeColor = ToDrawingColor(shades.DarkColor2[0]);
		}
		internal class Shades {
			public Color[] LightColor1 { get; set; }
			public Color[] DarkColor1 { get; set; }
			public Color[] LightColor2 { get; set; }
			public Color[] DarkColor2 { get; set; }
			public Color[][] Accents { get; set; }
		}
		public static Shades MakeShades(OfficeThemeColors colors) {
			var accentCoeffs = new[] { 1F, 1.8F, 1.6F, 1.4F, 0.75F, 0.5F };
			var background1Coeffs = new[] { 1F, 0.95F, 0.85F, 0.75F, 0.65F, 0.5F };
			var text1Coeffs = new[] { 1F, 1.5F, 1.35F, 1.25F, 1.15F, 1.05F };
			var background2Coeffs = new[] { 1F, 0.9F, 0.75F, 0.5F, 0.25F, 0.1F };
			var text2Coeffs = new[] { 1F, 1.8F, 1.6F, 1.4F, 0.75F, 0.5F };
			Func<Color, float[], Color[]> shades = (c, cfs) =>
				Enumerable.Range(0, cfs.Length)
				.Select(_ => c)
				.Zip(cfs, Brightness)
				.ToArray();
			return new Shades {
				LightColor1 = shades(colors.LightColors[0], background1Coeffs),
				DarkColor1 = shades(colors.DarkColors[0], text1Coeffs),
				LightColor2 = shades(colors.LightColors[1], background2Coeffs),
				DarkColor2 = shades(colors.DarkColors[1], text2Coeffs),
				Accents = colors.Accents.Select(x => shades(x, accentCoeffs)).ToArray()
			};
		}
		static void Visualize(OfficeThemeColors colors) {
			var window = new Window { SizeToContent = SizeToContent.WidthAndHeight };
			var content = new ThemeVisualizer();
			var shades = MakeShades(colors);
			content.DataContext = new {
				LightColor1 = shades.LightColor1.Select(x => new SolidColorBrush(x)).ToArray(),
				DarkColor1 = shades.DarkColor1.Select(x => new SolidColorBrush(x)).ToArray(),
				LightColor2 = shades.LightColor2.Select(x => new SolidColorBrush(x)).ToArray(),
				DarkColor2 = shades.DarkColor2.Select(x => new SolidColorBrush(x)).ToArray(),
				Accents = shades.Accents.Select(x => x.Select(y => new SolidColorBrush(y)).ToArray()).ToArray(),
			};
			window.Content = content;
			window.Show();
		}
	}
}
