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

using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Diagram.Core.Shapes;
using DevExpress.Diagram.Core.Shapes.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Diagram.Core.TypeConverters;
namespace DevExpress.Diagram.Core.Themes {
	[ContentProperty("ColorPalette")]
	public class Theme {
		public ColorPalette ColorPalette { get; set; }
		public double FontSize { get; set; }
		public FontFamily FontFamily { get; set; }
		public EffectCollection Effects { get; set; }
	}
	public class ColorPalette {
		public IEnumerable<Color> Accents { get { return new Color[] { Accent1, Accent2, Accent3, Accent4, Accent5, Accent6 }; } }
		public Color Accent1 { get; set; }
		public Color Accent2 { get; set; }
		public Color Accent3 { get; set; }
		public Color Accent4 { get; set; }
		public Color Accent5 { get; set; }
		public Color Accent6 { get; set; }
		public Color Dark { get; set; }
		public Color Light { get; set; }
	}
	public class EffectCollection {
		readonly List<Effect> themeEffects;
		readonly List<Effect> variantEffects;
		readonly List<Effect> connectorEffects;
		public List<Effect> ThemeEffects { get { return themeEffects; } }
		public List<Effect> VariantEffects { get { return variantEffects; } }
		public List<Effect> ConnectorEffects { get { return connectorEffects; } }
		public EffectCollection() {
			themeEffects = new List<Effect>();
			variantEffects = new List<Effect>();
			connectorEffects = new List<Effect>();
		}
	}
	public class Effect {
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Stroke { get; set; }
		public double StrokeThickness { get; set; }
		public DoubleCollection StrokeDashArray { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Background { get; set; }
		[TypeConverter(typeof(CriteriaOperatorConverter))]
		public CriteriaOperator Foreground { get; set; }
		public bool IsFontBold { get; set; }
		public bool IsFontItalic { get; set; }
		public bool IsFontUnderline { get; set; }
		public bool IsFontStrikethrough { get; set; }
		public Effect() {
			StrokeThickness = 1;
			StrokeDashArray = new DoubleCollection();
		}
		public DiagramItemBrush GetBrush(EffectContext context) {
			Color foreground = GetForeground(context);
			Color background = GetBackground(context);
			Color stroke = GetStroke(context);
			return new DiagramItemBrush(foreground, background, stroke);
		}
		internal Color GetBackground(EffectContext context) {
			return Evaluate(Background, context);
		}
		internal Color GetForeground(EffectContext context) {
			return Evaluate(Foreground, context);
		}
		internal Color GetStroke(EffectContext context) {
			return Evaluate(Stroke, context);
		}
		Color Evaluate(CriteriaOperator op, EffectContext context) {
			var properties = TypeDescriptor.GetProperties(context);
			ExpressionEvaluator evaluator = new ExpressionEvaluator(properties, op, new List<ICustomFunctionOperator> { new ChangeBrightnessFunction(), new GetColorFunction() });
			return (Color)evaluator.Evaluate(context);
		}
	}
	public class ThemeKey : DiagramResourceKey {
		public ThemeKey(string resourceKey)
			: base(resourceKey) {
		}
	}
	static class DiagramThemeFactory {
		public static DiagramTheme CreateTheme(string themeId, Func<string> getName, Theme theme) {
			DiagramColorPalette palette = CreateDiagramColorPalette(theme.ColorPalette);
			DiagramEffectCollection effectCollection = CreateDiagramEffectCollection(theme.Effects);
			DiagramFontSettings fontSettings = new DiagramFontSettings(theme.FontSize, theme.FontFamily, null);
			return new DiagramTheme(themeId, getName, palette, effectCollection, fontSettings);
		}
		public static DiagramColorPalette CreateDiagramColorPalette(ColorPalette palette) {
			return new DiagramColorPalette(palette.Accents.ToArray(), palette.Light, palette.Dark);
		}
		public static DiagramEffectCollection CreateDiagramEffectCollection(EffectCollection collection) {
			DiagramEffect[] variantEffects = collection.VariantEffects.Select(x => CreateDiagramEffect(x)).ToArray();
			DiagramEffect[] themeEffects = collection.ThemeEffects.Select(x => CreateDiagramEffect(x)).ToArray();
			DiagramEffect[] connectorEffects = collection.ConnectorEffects.Select(x => CreateDiagramEffect(x)).ToArray();
			return new DiagramEffectCollection(variantEffects, themeEffects, connectorEffects);
		}
		public static DiagramEffect CreateDiagramEffect(Effect effect) {
			DiagramItemLineSettings lineSettings = new DiagramItemLineSettings(effect.StrokeThickness, effect.StrokeDashArray.ToArray());
			var fontEffects = new DiagramFontEffects(effect.IsFontBold, effect.IsFontItalic, effect.IsFontUnderline, effect.IsFontStrikethrough);
			return new DiagramEffect(lineSettings, context => effect.GetBrush(context), fontEffects);
		}
	}
	class GetColorFunction : ICustomFunctionOperator {
		public string Name {
			get { return "Color"; }
		}
		public object Evaluate(params object[] operands) {
			if(operands.Length != 1)
				throw new ArgumentException(string.Format("Wrong arguments count ({0}). Function - '{1}'.", operands.Length, Name));
			ColorConverter converter = new ColorConverter();
			if(!converter.CanConvertFrom(operands[0].GetType()))
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(ICustomFunctionOperator).Name, Name, operands[0].GetType().FullName.ToString()));
			return converter.ConvertFrom(operands[0]);
		}
		public Type ResultType(params Type[] operands) {
			return typeof(Color);
		}
	}
	class ChangeBrightnessFunction : ICustomFunctionOperator {
		public string Name {
			get { return "ChangeBrightness"; }
		}
		public object Evaluate(params object[] operands) {
			if(operands.Length != 2)
				throw new ArgumentException(string.Format("Wrong arguments count ({0}). Function - '{1}'.", operands.Length, Name));
			if(!(operands[0] is Color))
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(ICustomFunctionOperator).Name, Name, operands[0].GetType().FullName.ToString()));
			if(!TypeConvertionValidator.CanConvert(operands[1], typeof(float)))
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorOperatorSubtypeNotSupportedForSpecificOperandType, typeof(ICustomFunctionOperator).Name, Name, operands[1].GetType().FullName.ToString()));
			Color color = (Color)operands[0];
			float brightness = Convert.ToSingle(operands[1]);
			return MathHelper.ChangeColorBrightness(color, brightness);
		}
		public Type ResultType(params Type[] operands) {
			return typeof(Color);
		}
	}
}
