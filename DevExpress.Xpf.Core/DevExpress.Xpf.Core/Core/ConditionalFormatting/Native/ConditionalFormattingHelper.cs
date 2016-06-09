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

using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.GridData;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Mvvm.POCO;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Native {
	public class ConditionalFormattingHelper<T> where T : FrameworkElement, IConditionalFormattingClient<T> {
		static bool CoerceCallbacksRegistered = false;
		static void EnsureCoerceCallbacksRegistered(DependencyProperty backgroundProperty) {
			if(!CoerceCallbacksRegistered) {
				CoerceCallbacksRegistered = true;
				TextBlock.FontSizeProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(TextBlock.FontSizeProperty.DefaultMetadata.DefaultValue, null, CoerceFontSize));
				TextBlock.ForegroundProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(TextBlock.ForegroundProperty.DefaultMetadata.DefaultValue, null, CoerceForeground));
				TextBlock.FontFamilyProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(TextBlock.FontFamilyProperty.DefaultMetadata.DefaultValue, null, CoerceFontFamily));
				TextBlock.FontStyleProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(TextBlock.FontStyleProperty.DefaultMetadata.DefaultValue, null, CoerceFontStyle));
				TextBlock.FontStretchProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(TextBlock.FontStretchProperty.DefaultMetadata.DefaultValue, null, CoerceFontStretch));
				TextBlock.FontWeightProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(TextBlock.FontWeightProperty.DefaultMetadata.DefaultValue, null, CoerceFontWeight));
				InplaceBaseEditHelper.TextDecorationsProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(InplaceBaseEditHelper.TextDecorationsProperty.DefaultMetadata.DefaultValue, null, CoerceTextDecorations));
				if(backgroundProperty != null)
					backgroundProperty.OverrideMetadata(typeof(T), new FrameworkPropertyMetadata(backgroundProperty.DefaultMetadata.DefaultValue, null, CoerceBackground));
			}
		}
		static object CoerceForeground(DependencyObject d, object baseValue) {
			return ((IConditionalFormattingClient<T>)d).FormattingHelper.CoerceForeground(baseValue as Brush);
		}
		static object CoerceFontSize(DependencyObject d, object baseValue) {
			return ((IConditionalFormattingClient<T>)d).FormattingHelper.CoerceFontSize((double)baseValue);
		}
		static object CoerceFontStyle(DependencyObject d, object baseValue) {
			return ((IConditionalFormattingClient<T>)d).FormattingHelper.CoerceFontStyle((FontStyle)baseValue);
		}
		static object CoerceFontFamily(DependencyObject d, object baseValue) {
			return ((IConditionalFormattingClient<T>)d).FormattingHelper.CoerceFontFamily((FontFamily)baseValue);
		}
		static object CoerceFontStretch(DependencyObject d, object baseValue) {
			return ((IConditionalFormattingClient<T>)d).FormattingHelper.CoerceFontStretch((FontStretch)baseValue);
		}
		static object CoerceFontWeight(DependencyObject d, object baseValue) {
			return ((IConditionalFormattingClient<T>)d).FormattingHelper.CoerceFontWeight((FontWeight)baseValue);
		}
		static object CoerceBackground(DependencyObject d, object baseValue) {
			return ((IConditionalFormattingClient<T>)d).FormattingHelper.CoerceBackground((Brush)baseValue);
		}
		static object CoerceTextDecorations(DependencyObject d, object baseValue) {
			return ((IConditionalFormattingClient<T>)d).FormattingHelper.CoerceTextDecorations((TextDecorationCollection)baseValue);
		}
		readonly T owner;
		[IgnoreDependencyPropertiesConsistencyChecker]
		readonly DependencyProperty backgroundProperty;
		public ConditionalFormattingHelper(T owner, DependencyProperty backgroundProperty = null) {
			this.owner = owner;
			this.backgroundProperty = backgroundProperty;
		}
		Brush CoerceForeground(Brush brush) {
			return CoerceConditionalFormatValue(brush, x => x.CoerceForeground, onlyInNoneSelectionState: true);
		}
		double CoerceFontSize(double fontSize) {
			return CoerceConditionalFormatValue(fontSize, x => x.CoerceFontSize, onlyInNoneSelectionState: false);
		}
		FontStyle CoerceFontStyle(FontStyle fontStyle) {
			return CoerceConditionalFormatValue(fontStyle, x => x.CoerceFontStyle, onlyInNoneSelectionState: false);
		}
		FontFamily CoerceFontFamily(FontFamily fontFamily) {
			return CoerceConditionalFormatValue(fontFamily, x => x.CoerceFontFamily, onlyInNoneSelectionState: false);
		}
		FontStretch CoerceFontStretch(FontStretch fontStretch) {
			return CoerceConditionalFormatValue(fontStretch, x => x.CoerceFontStretch, onlyInNoneSelectionState: false);
		}
		FontWeight CoerceFontWeight(FontWeight fontWeight) {
			return CoerceConditionalFormatValue(fontWeight, x => x.CoerceFontWeight, onlyInNoneSelectionState: false);
		}
		TValue CoerceConditionalFormatValue<TValue>(TValue value, Func<FormatConditionBaseInfo, Func<TValue, FormatValueProvider, TValue>> coerceActionAccessor, bool onlyInNoneSelectionState) {
			if(!owner.IsReady)
				return value;
			owner.Locker.DoLockedAction(() => {
				var conditions = owner.GetRelatedConditions();
				if(conditions != null && (!onlyInNoneSelectionState || !owner.IsSelected)) {
					foreach(FormatConditionBaseInfo formatCondition in conditions) {
						var valueProvider = owner.GetValueProvider(formatCondition.ActualFieldName);
						if(valueProvider != null)
							value = coerceActionAccessor(formatCondition)(value, valueProvider.Value);
					}
				}
			});
			return value;
		}
		public Brush CoerceBackground(Brush background) {
			return CoerceConditionalFormatValue(background, x => x.CoerceBackground, onlyInNoneSelectionState: true);
		}
		internal TextDecorationCollection CoerceTextDecorations(TextDecorationCollection textDecorations) {
			return CoerceConditionalFormatValue(textDecorations, x => x.CoerceTextDecorations, onlyInNoneSelectionState: false);
		}
		public void UpdateConditionalAppearance() {
			var conditions = owner.GetRelatedConditions();
			var newMask = ConditionalFormattingMaskHelper.GetConditionsMask(conditions);
			if(newMask != ConditionalFormatMask.None || currentMask != ConditionalFormatMask.None) {
				UpdateConditionalAppearanceCore(newMask);
			}
			currentMask = newMask;
		}
		ConditionalFormatMask currentMask = ConditionalFormatMask.None;
		void UpdateConditionalAppearanceCore(ConditionalFormatMask newMask) {
			EnsureCoerceCallbacksRegistered(backgroundProperty);
			CoerceIfNeeded(newMask, ConditionalFormatMask.DataBarOrIcon, () => owner.UpdateDataBarFormatInfo(CoerceConditionalFormatValue((DataBarFormatInfo)null, x => x.CoerceDataBarFormatInfo, onlyInNoneSelectionState: false)));
			CoerceIfNeeded(newMask, ConditionalFormatMask.Background, () => {
				owner.UpdateBackground();
				if(backgroundProperty != null)
					owner.CoerceValue(backgroundProperty);
			});
			CoerceIfNeeded(newMask, ConditionalFormatMask.Foreground, () => owner.CoerceValue(TextBlock.ForegroundProperty));
			CoerceIfNeeded(newMask, ConditionalFormatMask.FontSize, () => owner.CoerceValue(TextBlock.FontSizeProperty));
			CoerceIfNeeded(newMask, ConditionalFormatMask.FontStyle, () => owner.CoerceValue(TextBlock.FontStyleProperty));
			CoerceIfNeeded(newMask, ConditionalFormatMask.FontFamily, () => owner.CoerceValue(TextBlock.FontFamilyProperty));
			CoerceIfNeeded(newMask, ConditionalFormatMask.FontStretch, () => owner.CoerceValue(TextBlock.FontStretchProperty));
			CoerceIfNeeded(newMask, ConditionalFormatMask.FontWeight, () => owner.CoerceValue(TextBlock.FontWeightProperty));
			CoerceIfNeeded(newMask, ConditionalFormatMask.TextDecorations, () => owner.CoerceValue(InplaceBaseEditHelper.TextDecorationsProperty));
		}
		void CoerceIfNeeded(ConditionalFormatMask newMask, ConditionalFormatMask flag, Action coerceAction) {
			if((newMask & flag) > 0 || (currentMask & flag) > 0)
				coerceAction();
		}
	}
}
