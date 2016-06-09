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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
namespace DevExpress.Diagram.Core.Themes {
	public class DiagramColorPalette {
		#region Static
		const int AccentCount = 6;
		const double LightColorLuminance = 200;
		static readonly ReadOnlyCollection<float> darkColorBrightnessFactors = new ReadOnlyCollection<float>(new float[] { 0, 0.8f, 0.6f, 0.4f, -0.25f, -0.5f });
		static readonly ReadOnlyCollection<float> lightColorBrightnessFactors = new ReadOnlyCollection<float>(new float[] { -0, -0.05f, -0.15f, -0.25f, -0.35f, -0.5f });
		static List<Color> GetThemeColors(Color light, Color dark, Color[] accents) {
			var colors = GetBaseThemeColors(light, dark, accents);
			var getBrightness = Enumerable.Range(0, darkColorBrightnessFactors.Count).Select(i => new Func<Color, float>(color => IsLightColor(color) ? lightColorBrightnessFactors[i] : darkColorBrightnessFactors[i]));
			return getBrightness.SelectMany(func => colors.Select(color => MathHelper.ChangeColorBrightness(color, func(color)))).ToList();
		}
		static IList<Color> GetBaseThemeColors(Color light, Color dark, Color[] accents) {
			List<Color> colors = new List<Color>();
			colors.Add(Colors.White);
			colors.Add(Colors.Black);
			colors.Add(light);
			colors.Add(dark);
			colors.AddRange(accents);
			return colors;
		}
		static double GetLuminance(Color color) {
			return 0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B;
		}
		static bool IsLightColor(Color color) {
			return GetLuminance(color) > LightColorLuminance;
		}
		#endregion
		readonly ReadOnlyCollection<Color> colors;
		readonly ReadOnlyCollection<Color> accents;
		readonly Color light;
		readonly Color dark;
		public ReadOnlyCollection<Color> Accents { get { return accents; } }
		public Color Light { get { return light; } }
		public Color Dark { get { return dark; } }
		public ReadOnlyCollection<Color> ThemeColors { get { return colors; } }
		public DiagramColorPalette(Color[] accents, Color light, Color dark) {
			if(accents == null)
				throw new ArgumentNullException();
			if(accents.Length != AccentCount)
				throw new ArgumentOutOfRangeException();
			this.accents = new ReadOnlyCollection<Color>(accents);
			this.light = light;
			this.dark = dark;
			this.colors = new ReadOnlyCollection<Color>(GetThemeColors(light, dark, accents));
		}
		public Color GetColorByColorId(DiagramThemeColorId colorId) {
			return colors[(int)colorId];
		}
	}
	public class DiagramItemBrush {
		readonly Color foregroundCore;
		readonly Color backgroundCore;
		readonly Color strokeCore;
		public Color Foreground { get { return foregroundCore; } }
		public Color Background { get { return backgroundCore; } }
		public Color Stroke { get { return strokeCore; } }
		public DiagramItemBrush(Color foreground, Color background, Color stroke) {
			this.foregroundCore = foreground;
			this.backgroundCore = background;
			this.strokeCore = stroke;
		}
	}
}
