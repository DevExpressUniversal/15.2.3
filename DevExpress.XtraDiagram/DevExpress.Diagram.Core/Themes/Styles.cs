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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
namespace DevExpress.Diagram.Core.Themes {
	public class DiagramItemStyle {
		readonly DiagramFontSettings fontSettings;
		readonly DiagramItemLineSettings lineSettings;
		readonly DiagramItemBrush shapeBrush;
		public DiagramFontSettings FontSettings { get { return fontSettings; } }
		public DiagramItemLineSettings LineSettings { get { return lineSettings; } }
		public DiagramItemBrush Brush { get { return shapeBrush; } }
		public DiagramItemStyle(DiagramItemBrush shapeBrush, DiagramFontSettings fontSettings, DiagramItemLineSettings lineSettings) {
			this.fontSettings = fontSettings;
			this.lineSettings = lineSettings;
			this.shapeBrush = shapeBrush;
		}
	}
	public static class DiagramItemStyleHelper {
		#region DEBUGTEST
#if DEBUGTEST
		public static DiagramItemBrush CorrectBrushForTests(DiagramItemBrush brush, Color light, bool allowLightForeground, bool allowLightStroke) {
			return CorrectBrush(brush, light, allowLightForeground, allowLightStroke);
		}
		public static Color CorrectLightForegroundForTests(Color light, Color foreground, Color background, Color stroke) {
			return CorrectLightForeground(light, foreground, background, stroke);
		}
		public static Color CorrectLightStrokeForTests(Color light, Color background, Color stroke) {
			return CorrectLightStroke(light, background, stroke);
		}
#endif
		#endregion
		public static DiagramItemBrush CorrectBrush(DiagramItemBrush brush, bool allowLightForeground, bool allowLightStroke) {
			return CorrectBrush(brush, Colors.White, allowLightForeground, allowLightStroke);
		}
		public static DiagramItemStyle ReplaceBrush(this DiagramItemStyle baseStyle, DiagramItemBrush brush) {
			return new DiagramItemStyle(brush, baseStyle.FontSettings, baseStyle.LineSettings);
		}
		public static DiagramItemStyle CreateDiagramItemStyle(DiagramTheme theme, DiagramItemStyleId styleId, bool allowLightForeground, bool allowLightStroke) {
			var baseStyle = theme.GetDiagramItemStyle(styleId);
			if(baseStyle == null)
				return null;
			DiagramItemBrush brush = CorrectBrush(baseStyle.Brush, allowLightForeground, allowLightStroke);
			return baseStyle.ReplaceBrush(brush);
		}
		public static DiagramItemBrush CorrectBrush(DiagramColorPalette palette, DiagramItemBrush baseBrush, DiagramThemeColorId? foregroundId, DiagramThemeColorId? backgroundId, DiagramThemeColorId? strokeId) {
			Color foreground = GetColorByColorId(palette, foregroundId, baseBrush.Foreground);
			Color background = GetColorByColorId(palette, backgroundId, baseBrush.Background);
			Color stroke = GetColorByColorId(palette, strokeId, baseBrush.Stroke);
			return new DiagramItemBrush(foreground, background, stroke);
		}
		static Color GetColorByColorId(DiagramColorPalette palette, DiagramThemeColorId? colorId, Color defaultColor) {
			if(colorId.HasValue)
				return palette.GetColorByColorId(colorId.Value);
			return defaultColor;
		}
		static DiagramItemBrush CorrectBrush(DiagramItemBrush brush, Color light, bool allowLightForeground, bool allowLightStroke) {
			Color foreground = !allowLightForeground ? CorrectLightForeground(light, brush.Foreground, brush.Background, brush.Stroke) : brush.Foreground;
			Color stroke = !allowLightStroke ? CorrectLightStroke(light, brush.Background, brush.Stroke) : brush.Stroke;
			return new DiagramItemBrush(foreground, brush.Background, stroke);
		}
		static Color CorrectLightForeground(Color light, Color foreground, Color background, Color stroke) {
			if(foreground != light) return foreground;
			if(background != light) return background;
			return stroke;
		}
		static Color CorrectLightStroke(Color light, Color background, Color stroke) {
			if(stroke != light) return stroke;
			return background;
		}
		public static Color? BrushToColor(Brush brush) {
			return (brush as SolidColorBrush).Return<SolidColorBrush, Color?> (x => x.Color, () => null);
		}
		public static Brush ColorToBrush(Color color) {
			return new SolidColorBrush(color).Do(x => x.Freeze());
		}
	}
}
