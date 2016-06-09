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
using System.Linq;
using System.Text;
using System.Windows.Media;
using DevExpress.Internal;
using System.Linq.Expressions;
using System.Windows;
namespace DevExpress.Diagram.Core {
	public interface IFontTraits {
		bool IsFontBold { get; set; }
		bool IsFontItalic { get; set; }
		bool IsFontUnderline { get; set; }
		bool IsFontStrikethrough { get; set; }
		double FontSize { get; set; }
		bool AllowEdit { get; }
	}
	public static class DiagramFontActions {
		readonly static double?[] FontSizeSteps = { 6, 8, 9, 10, 12, 14, 18, 24, 30, 36, 48, 60 };
		public static void IncreaseSelectionFontSize(this IDiagramControl diagram) {
			diagram.ChangeSelectionFontSize(fontSize => FontSizeSteps.FirstOrDefault(x => x.Value > fontSize));
		}
		public static void DecreaseSelectionFontSize(this IDiagramControl diagram) {
			diagram.ChangeSelectionFontSize(fontSize => FontSizeSteps.Reverse().FirstOrDefault(x => x.Value < fontSize));
		}
		static void ChangeSelectionFontSize(this IDiagramControl diagram, Func<double, double?> getNextSize) {
			diagram.ChangeSelectionFontProperty<double>(
				getNewValue: value => getNextSize(value),
				finalProperty: ExpressionHelper.GetProperty((IFontTraits x) => x.FontSize)
			);
		}
		public static void ToggleFontTraitsProperty(this IDiagramControl diagram, Expression<Func<IFontTraits, bool>> propertyExpression) {
			diagram.ChangeSelectionFontProperty<bool>(
				getNewValue: value => !value,
				finalProperty: ExpressionHelper.GetProperty(propertyExpression)
			);
		}
		static void ChangeSelectionFontProperty<T>(this IDiagramControl diagram, Func<T, T?> getNewValue, PropertyDescriptor finalProperty) where T : struct {
			var property = ProxyPropertyDescriptor.Create(finalProperty, (IDiagramItem item) => item.Controller.GetFontTraits());
			var pairs = diagram.SelectedItems()
				.Where(x => x.Controller.GetFontTraits() != null)
				.Select(x => new { Item = x, NewValue = getNewValue((T)property.GetValue(x)) })
				.Where(x => x.NewValue != null)
				.Select(x => Tuple.Create(x.Item, x.NewValue.Value))
				.ToArray();
			if(!pairs.Any())
				return;
			ApplyValues(diagram, property, pairs);
		}
		static void ApplyValues<T>(IDiagramControl diagram, PropertyDescriptor property, Tuple<IDiagramItem, T>[] pairs) where T : struct {
			diagram.ExecuteWithSelectionRestore(transaction => pairs.ForEach(x => {
				transaction.SetItemProperty(x.Item1, x.Item2, property);
			}), null, allowMerge: false);
		}
	}
	public class FontTraits : IFontTraits {
		public static Color BrushToColor(Brush brush) {
			return (brush as SolidColorBrush).Return(x => x.Color, () => Colors.Transparent);
		}
		public static Brush ColorToBrush(Color color) {
			return new SolidColorBrush(color).Do(x => x.Freeze());
		}
		public readonly IDiagramItem TextItem;
		public FontTraits(IDiagramItem item) {
			this.TextItem = item;
		}
		[Browsable(false)]
		public bool IsFontBold {
			get { return FontHelper.FontWeightToIsBold(TextItem.FontWeight); }
			set { TextItem.FontWeight = FontHelper.IsBoldToFontWeight(value); }
		}
		[Browsable(false)]
		public bool IsFontItalic {
			get { return FontHelper.FontStyleToIsItalic(TextItem.FontStyle); }
			set { TextItem.FontStyle = FontHelper.IsItalicToFontStyle(value); }
		}
		[Browsable(false)]
		public bool IsFontUnderline {
			get { return FontHelper.TextDecorationsToIsUnderline(TextItem.TextDecorations); }
			set { TextItem.TextDecorations = FontHelper.IsUnderlineToTextDecorations(value, TextItem.TextDecorations); }
		}
		[Browsable(false)]
		public bool IsFontStrikethrough {
			get { return FontHelper.TextDecorationsToIsStrikethrough(TextItem.TextDecorations); }
			set { TextItem.TextDecorations = FontHelper.IsStrikethroughToTextDecorations(value, TextItem.TextDecorations); }
		}
		[Browsable(false)]
		public double FontSize {
			get { return TextItem.FontSize; }
			set {
				if(value > 0.0d)
					TextItem.FontSize = value;
			}
		}
		[Browsable(false)]
		public bool AllowEdit { get { return TextItem.Controller.GetTextProperty() != null; } }
	}
	public static class FontHelper {
		static readonly FontWeight[] BoldWeights = new[] {
				FontWeights.Black,
				FontWeights.Bold,
				FontWeights.DemiBold,
				FontWeights.ExtraBlack,
				FontWeights.ExtraBold,
				FontWeights.Heavy,
				FontWeights.SemiBold,
				FontWeights.UltraBlack,
				FontWeights.UltraBold,
			};
		public static bool FontWeightToIsBold(FontWeight weight) {
			return BoldWeights.Contains(weight);
		}
		public static FontWeight IsBoldToFontWeight(bool isBold) {
			return isBold ? FontWeights.Bold : FontWeights.Normal;
		}
		public static bool FontStyleToIsItalic(FontStyle style) {
			return style != FontStyles.Normal;
		}
		public static FontStyle IsItalicToFontStyle(bool isItalic) {
			return isItalic ? FontStyles.Italic : FontStyles.Normal;
		}
		public static TextDecorationCollection FlagsToTextDecorations(bool isUnderline, bool isStrikethrough) {
			if(isUnderline && isStrikethrough)
				return GetDecorations(TextDecorations.Underline.Concat(TextDecorations.Strikethrough).ToArray());
			if(isUnderline)
				return TextDecorations.Underline;
			if(isStrikethrough)
				return TextDecorations.Strikethrough;
			return null;
		}
		public static bool TextDecorationsToIsUnderline(TextDecorationCollection decorations) {
			return ContainsDecorations(TextDecorations.Underline, decorations);
		}
		public static TextDecorationCollection IsUnderlineToTextDecorations(bool isUnderline, TextDecorationCollection baseDecorations) {
			return ToggleDecorations(TextDecorations.Underline, isUnderline, baseDecorations);
		}
		public static bool TextDecorationsToIsStrikethrough(TextDecorationCollection decorations) {
			return ContainsDecorations(TextDecorations.Strikethrough, decorations);
		}
		public static TextDecorationCollection IsStrikethroughToTextDecorations(bool isStrikethrough, TextDecorationCollection baseDecorations) {
			return ToggleDecorations(TextDecorations.Strikethrough, isStrikethrough, baseDecorations);
		}
		static bool ContainsDecorations(TextDecorationCollection flag, TextDecorationCollection decorations) {
			return decorations != null && !flag.Except(decorations).Any();
		}
		static TextDecorationCollection ToggleDecorations(TextDecorationCollection flag, bool flagState, TextDecorationCollection decorations) {
			if(flagState)
				return GetDecorations(decorations != null ? decorations.Union(flag).ToArray() : flag.ToArray());
			if(ContainsDecorations(flag, decorations))
				return GetDecorations(decorations.Except(flag).ToArray());
			return decorations;
		}
		static TextDecorationCollection GetDecorations(TextDecoration[] newDecorations) {
			if(!newDecorations.Any()) return null;
			var collection = new TextDecorationCollection(newDecorations);
			collection.Freeze();
			return collection;
		}
	}
}
