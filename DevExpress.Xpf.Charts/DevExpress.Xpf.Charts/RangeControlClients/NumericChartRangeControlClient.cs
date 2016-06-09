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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using DevExpress.Utils;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Internal;
namespace DevExpress.Xpf.Charts.RangeControlClient {
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfCommon)]
	public class NumericChartRangeControlClient : ChartRangeControlClient {
		public static readonly DependencyProperty SnapAlignmentProperty =
			DependencyProperty.Register("SnapAlignment", typeof(double), typeof(NumericChartRangeControlClient),
			new PropertyMetadata(0.0, OnSnapAlignmentPropertyChanged));
		public static readonly DependencyProperty GridAlignmentProperty =
			DependencyProperty.Register("GridAlignment", typeof(double), typeof(NumericChartRangeControlClient),
			new PropertyMetadata(defaultGridAligment, OnGridAlignmentPropertyChanged));
		static void OnSnapAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NumericChartRangeControlClient client = d as NumericChartRangeControlClient;
			if (client != null && e.NewValue != null)
				((NumericGridCalculator)client.ChartGridCalculator).UpdateSnapAligment((double)e.NewValue);
		}
		static void OnGridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NumericChartRangeControlClient client = d as NumericChartRangeControlClient;
			if (client != null && e.NewValue != null) {
				((NumericGridCalculator)client.ChartGridCalculator).UpdateGridAligment((double)e.NewValue);
				client.UpdateItems();
			}
		}
		const double defaultGridAligment = 0;
		NumericGridCalculator gridCalculator; 
		protected  override GridCalculator ChartGridCalculator { get { return gridCalculator; } }
		[Category(Categories.Layout)]
		public double SnapAlignment {
			get { return (double)GetValue(SnapAlignmentProperty); }
			set { SetValue(SnapAlignmentProperty, value); }
		}
		[Category(Categories.Layout)]
		public double GridAlignment {
			get { return (double)GetValue(GridAlignmentProperty); }
			set { SetValue(GridAlignmentProperty, value); }
		}
		double MinVisualRange { get { return GridAlignment == defaultGridAligment ? 1 : GridAlignment; } }
		public NumericChartRangeControlClient()
			: base() {
			DefaultStyleKey = typeof(NumericChartRangeControlClient);
			this.gridCalculator = new NumericGridCalculator(GridAlignment);
		}
		protected override object ConvertToNative(double value) {
			return SparklineMathUtils.ConvertToNative(value, SparklineScaleType.Numeric);
		}
		protected override void CorrectVisibleRange(object oldStart, object oldEnd, object newStart, object newEnd, out object correctStart, out object correctEnd) {
			double delta = ((double)newEnd - (double)newStart);
			if (delta < MinVisualRange) {
				correctStart = oldStart;
				correctEnd = oldEnd;
			}
			else {
				correctStart = newStart;
				correctEnd = newEnd;
			}
		}
		protected override bool CheckValueType(object value) {
			if (value == null)
				return false;
			Type valueType = value.GetType();
			return (valueType == typeof(double) || valueType == typeof(float) || valueType == typeof(int) ||
				valueType == typeof(uint) || valueType == typeof(long) || valueType == typeof(ulong) ||
				valueType == typeof(decimal) || valueType == typeof(short) || valueType == typeof(ushort) ||
				valueType == typeof(byte) || valueType == typeof(sbyte) || valueType == typeof(char));
		}
		protected override object TryParseValue(object value) {
			if (value == null)
				return null;
			Type valueType = value.GetType();
			if (valueType == typeof(string)) {
				double numeric;
				if (double.TryParse((string)value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out numeric))
					return numeric;
			}
			else if (valueType == typeof(double) || valueType == typeof(float) || valueType == typeof(int) ||
				valueType == typeof(uint) || valueType == typeof(long) || valueType == typeof(ulong) ||
				valueType == typeof(decimal) || valueType == typeof(short) || valueType == typeof(ushort) ||
				valueType == typeof(byte) || valueType == typeof(sbyte) || valueType == typeof(char))
				return Convert.ToDouble(value);
			return null;
		}
	}
}
