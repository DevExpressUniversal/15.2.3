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
	public class DataBarFormat : IndicatorFormatBase {
		[XtraSerializableProperty]
		public Brush ZeroLineBrush {
			get { return (Brush)GetValue(ZeroLineBrushProperty); }
			set { SetValue(ZeroLineBrushProperty, value); }
		}
		public static readonly DependencyProperty ZeroLineBrushProperty =
			DependencyProperty.Register("ZeroLineBrush", typeof(Brush), typeof(DataBarFormat), new PropertyMetadata(null));
		[XtraSerializableProperty]
		public double ZeroLineThickness {
			get { return (double)GetValue(ZeroLineThicknessProperty); }
			set { SetValue(ZeroLineThicknessProperty, value); }
		}
		public static readonly DependencyProperty ZeroLineThicknessProperty =
			DependencyProperty.Register("ZeroLineThickness", typeof(double), typeof(DataBarFormat), new PropertyMetadata(1d));
		[XtraSerializableProperty]
		public Thickness Margin {
			get { return (Thickness)GetValue(MarginProperty); }
			set { SetValue(MarginProperty, value); }
		}
		public static readonly DependencyProperty MarginProperty = DependencyProperty.Register("Margin", typeof(Thickness), typeof(DataBarFormat), new PropertyMetadata(new Thickness(1)));
		[XtraSerializableProperty]
		public Thickness BorderThickness {
			get { return (Thickness)GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}
		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(DataBarFormat), new PropertyMetadata(new Thickness(1)));
		[XtraSerializableProperty]
		public Brush BorderBrush {
			get { return (Brush)GetValue(BorderBrushProperty); }
			set { SetValue(BorderBrushProperty, value); }
		}
		public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(DataBarFormat), new PropertyMetadata(null));
		[XtraSerializableProperty]
		public Brush BorderBrushNegative {
			get { return (Brush)GetValue(BorderBrushNegativeProperty); }
			set { SetValue(BorderBrushNegativeProperty, value); }
		}
		public static readonly DependencyProperty BorderBrushNegativeProperty = DependencyProperty.Register("BorderBrushNegative", typeof(Brush), typeof(DataBarFormat), new PropertyMetadata(null));
		[XtraSerializableProperty]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(DataBarFormat), new PropertyMetadata(null));
		[XtraSerializableProperty]
		public Brush FillNegative {
			get { return (Brush)GetValue(FillNegativeProperty); }
			set { SetValue(FillNegativeProperty, value); }
		}
		public static readonly DependencyProperty FillNegativeProperty = DependencyProperty.Register("FillNegative", typeof(Brush), typeof(DataBarFormat), new PropertyMetadata(null));
		protected override Freezable CreateInstanceCore() {
			return new DataBarFormat();
		}
		public override DataBarFormatInfo CoerceDataBarFormatInfo(DataBarFormatInfo value, FormatValueProvider provider, decimal? minValue, decimal? maxValue) {
			return CalcDataBarFormatInfo(provider, value, minValue, maxValue);
		}
		public DataBarFormatInfo CalcDataBarFormatInfo(FormatValueProvider provider, DataBarFormatInfo value, decimal? minValue, decimal? maxValue) {
			decimal? cellValue = GetDecimalValue(provider.Value);
			if(cellValue == null)
				return value;
			decimal? nullableMin = GetSummaryValue(provider, ConditionalFormatSummaryType.Min, minValue);
			decimal? nullableMax = GetSummaryValue(provider, ConditionalFormatSummaryType.Max, maxValue);
			if(nullableMin == null || nullableMax == null)
				return value;
			decimal min = Math.Min(0m, nullableMin.Value);
			decimal max = Math.Max(0m, nullableMax.Value);
			decimal range = max - min;
			return DataBarFormatInfo.AddDataBarFormatInfo(value, this, (double)(range != 0 ? (-min / range) : 0.5m), (double)(range != 0 ? (GetNormalizedValue(cellValue.Value, nullableMin.Value, nullableMax.Value) - min) / range : 0.5m));
		}
	}
}
