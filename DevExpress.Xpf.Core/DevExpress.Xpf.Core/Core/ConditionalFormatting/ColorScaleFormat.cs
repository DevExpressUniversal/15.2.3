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
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Mvvm.POCO;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Utils.Serializing;
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public class ColorScaleFormat : IndicatorFormatBase {
		[XtraSerializableProperty]
		public Color ColorMin {
			get { return (Color)GetValue(ColorMinProperty); }
			set { SetValue(ColorMinProperty, value); }
		}
		public static readonly DependencyProperty ColorMinProperty = DependencyProperty.Register("ColorMin", typeof(Color), typeof(ColorScaleFormat), new PropertyMetadata(Colors.Transparent));
		[XtraSerializableProperty]
		public Color? ColorMiddle {
			get { return (Color?)GetValue(ColorMiddleProperty); }
			set { SetValue(ColorMiddleProperty, value); }
		}
		public static readonly DependencyProperty ColorMiddleProperty = DependencyProperty.Register("ColorMiddle", typeof(Color?), typeof(ColorScaleFormat), new PropertyMetadata(null));
		[XtraSerializableProperty]
		public Color ColorMax {
			get { return (Color)GetValue(ColorMaxProperty); }
			set { SetValue(ColorMaxProperty, value); }
		}
		public static readonly DependencyProperty ColorMaxProperty = DependencyProperty.Register("ColorMax", typeof(Color), typeof(ColorScaleFormat), new PropertyMetadata(Colors.Transparent));
		protected override Freezable CreateInstanceCore() {
			return new ColorScaleFormat();
		}
		public override Brush CoerceBackground(Brush value, FormatValueProvider provider, decimal? minValue, decimal? maxValue) {
			Color? color = CalcColor(provider, minValue, maxValue);
			return color != null ? new SolidColorBrush(color.Value) : value;
		}
		public Color? CalcColor(FormatValueProvider provider, decimal? minValue, decimal? maxValue) {
			decimal? cellValue = GetDecimalValue(provider.Value);
			if(cellValue == null)
				return null;
			decimal? min = GetSummaryValue(provider, ConditionalFormatSummaryType.Min, minValue);
			decimal? max = GetSummaryValue(provider, ConditionalFormatSummaryType.Max, maxValue);
			if(min == null || max == null)
				return null;
			Color colorLow = ColorMin;
			Color colorHigh = ColorMax;
			if(ColorMiddle != null) {
				decimal average = (min.Value + max.Value) / 2;
				if(cellValue < average) {
					colorHigh = ColorMiddle.Value;
					max = average;
				} else {
					colorLow = ColorMiddle.Value;
					min = average;
				}
			}
			return CalcColorCore(colorLow, colorHigh, min.Value, max.Value, GetNormalizedValue(cellValue.Value, min.Value, max.Value));
		}
		static Color CalcColorCore(Color colorLow, Color colorHigh, decimal min, decimal max, decimal cellValue) {
			decimal ratio = GetRatio(min, max, cellValue);
			var color = Color.FromArgb(
				GetScaleValue(ratio, colorLow.A, colorHigh.A),
				GetScaleValue(ratio, colorLow.R, colorHigh.R),
				GetScaleValue(ratio, colorLow.G, colorHigh.G),
				GetScaleValue(ratio, colorLow.B, colorHigh.B)
			);
			return color;
		}
		internal static decimal GetRatio(decimal min, decimal max, decimal cellValue) {
			decimal ratio;
			if(min == max)
				ratio = 1;
			else
				ratio = (cellValue - min) / (max - min);
			return ratio;
		}
		static byte GetScaleValue(decimal ratio, decimal low, decimal high) {
			return (byte)Math.Round((low + (high - low) * ratio));
		}
	}
}
