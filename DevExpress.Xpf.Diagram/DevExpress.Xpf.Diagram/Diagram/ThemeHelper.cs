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

using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Themes;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Diagram {
	internal static class DiagramThemeHelper {
		#region DEBUGTEST
#if DEBUGTEST
		public static Style CreateStyleForTests(DevExpress.Diagram.Core.Themes.DiagramItemStyle itemStyle) {
			return CreateShapeStyle(itemStyle);
		}
		public static IEnumerable<DependencyProperty> GetStylePropertiesForTests(Style style) {
			return GetStyleProperties(style);
		}
#endif
		#endregion
		#region ThemeStyle
		public static Style CreateMergedStyle(Style baseStyle, IEnumerable<Style> styles) {
			var setters = styles
				.Where(x => x != null)
				.SelectMany(x => x.Setters);
			var baseProperties = GetStyleProperties(baseStyle);
			Style mergedStyle = baseStyle != null ? new Style(baseStyle.TargetType, baseStyle) : new Style();
			foreach(Setter setter in setters.Where(x => x is Setter)) {
				if(!baseProperties.Contains(setter.Property))
					mergedStyle.Setters.Add(setter);
			}
			mergedStyle.Seal();
			return mergedStyle.Setters.Any() ? mergedStyle : baseStyle;
		}
		public static Style CreateShapeStyle(this DiagramTheme theme, DiagramItemStyleId styleId) {
			var diagramStyle = theme.GetDiagramItemStyle(styleId);
			return CreateShapeStyle(diagramStyle);
		}
		public static Style CreateShapeStyle(DiagramItemStyle itemStyle) {
			if(itemStyle == null)
				return null;
			var style = new Style(typeof(Control));
			foreach(var setter in GetSetters(itemStyle))
				style.Setters.Add(setter);
			style.Seal();
			return style;
		}
		static IEnumerable<DependencyProperty> GetStyleProperties(Style style) {
			var setters = Enumerable.Empty<SetterBase>();
			Style currentStyle = style;
			while(currentStyle != null) {
				setters = setters.Concat(currentStyle.Setters);
				currentStyle = currentStyle.BasedOn;
			}
			return setters.Where(x => x is Setter).Select(x => ((Setter)x).Property);
		}
		static IEnumerable<Setter> GetSetters(DiagramItemStyle itemStyle) {
			#region Font
			var fontEffects = itemStyle.FontSettings.FontEffects;
			if(fontEffects != null) {
				yield return new Setter(Control.FontWeightProperty, FontHelper.IsBoldToFontWeight(fontEffects.IsFontBold));
				yield return new Setter(Control.FontStyleProperty, FontHelper.IsItalicToFontStyle(fontEffects.IsFontItalic));
				yield return new Setter(DiagramItem.TextDecorationsProperty, FontHelper.FlagsToTextDecorations(fontEffects.IsFontUnderline, fontEffects.IsFontStrikethrough));
			}
			yield return new Setter(TextBlock.FontFamilyProperty, itemStyle.FontSettings.FontFamily);
			yield return new Setter(TextBlock.FontSizeProperty, itemStyle.FontSettings.FontSize);
			#endregion
			yield return new Setter(Control.ForegroundProperty, CreateSolidColorBrush(itemStyle.Brush.Foreground));
			yield return new Setter(Control.BackgroundProperty, CreateSolidColorBrush(itemStyle.Brush.Background));
			yield return new Setter(Control.BorderBrushProperty, CreateSolidColorBrush(itemStyle.Brush.Stroke));
			yield return new Setter(Control.BorderThicknessProperty, new Thickness(itemStyle.LineSettings.StrokeThickness));
			yield return new Setter(DiagramItem.StrokeProperty, CreateSolidColorBrush(itemStyle.Brush.Stroke));
			yield return new Setter(DiagramItem.StrokeThicknessProperty, itemStyle.LineSettings.StrokeThickness);
			yield return new Setter(DiagramItem.StrokeDashArrayProperty, new DoubleCollection(itemStyle.LineSettings.StrokeDashArray));
		}
		static Brush CreateSolidColorBrush(Color color) {
			return new SolidColorBrush(color);
		}
		#endregion
		public static Pen GetPen(Pen basePen, Brush stroke, double thickness, IEnumerable<double> dashes, PenLineCap? penLineCap = null) {
			if(IsValidPen(basePen, stroke, thickness, dashes))
				return basePen;
			return new Pen(stroke, thickness) { DashStyle = new DashStyle(dashes, 0) }.Do(x => {
				if(penLineCap.HasValue)
					x.StartLineCap = x.EndLineCap = penLineCap.Value;
				x.Freeze();
			});
		}
		static bool IsValidPen(Pen pen, Brush stroke, double thickness, IEnumerable<double> dashes) {
			return pen != null && pen.Brush == stroke && pen.Thickness == thickness && Enumerable.SequenceEqual(dashes ?? Enumerable.Empty<double>(), pen.DashStyle.Dashes);
		}
	}
	public class DiagramImageExtension : MarkupExtension {
		public string Source { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return GetDiagramImage(Source);
		}
		public static BitmapImage GetDiagramImage(string source) {
			BitmapImage result = null;
			using(var stream = typeof(IDiagramControl).Assembly.GetManifestResourceStream(source)) {
				result = ImageHelper.CreateImageFromStream(stream);
			}
			return result;
		}
	}
}
