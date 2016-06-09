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

using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.RangeControl;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
namespace DevExpress.Xpf.Charts.RangeControlClient {
	public enum DateTimeGridAlignment {
		Millisecond = DateTimeGridAlignmentNative.Millisecond,
		Second = DateTimeGridAlignmentNative.Second,
		Minute = DateTimeGridAlignmentNative.Minute,
		Hour = DateTimeGridAlignmentNative.Hour,
		Day = DateTimeGridAlignmentNative.Day,
		Week = DateTimeGridAlignmentNative.Week,
		Month = DateTimeGridAlignmentNative.Month,
		Quarter = DateTimeGridAlignmentNative.Quarter,
		Year = DateTimeGridAlignmentNative.Year,
		Auto
	}
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfCommon)]
	public class DateTimeChartRangeControlClient : ChartRangeControlClient {
		public static readonly DependencyProperty SnapAlignmentProperty =
			DependencyProperty.Register("SnapAlignment", typeof(DateTimeMeasurementUnit), typeof(DateTimeChartRangeControlClient),
			new PropertyMetadata(DateTimeMeasurementUnit.Hour, OnSnapAlignmentPropertyChanged));
		public static readonly DependencyProperty GridAlignmentProperty =
			DependencyProperty.Register("GridAlignment", typeof(DateTimeGridAlignment), typeof(DateTimeChartRangeControlClient),
			new PropertyMetadata(DateTimeGridAlignment.Auto, OnGridAlignmentPropertyChanged));
		static void OnSnapAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { 
			DateTimeChartRangeControlClient client = d as DateTimeChartRangeControlClient;
			if (client != null && e.NewValue != null)
				((DateTimeGridCalculator)client.ChartGridCalculator).UpdateSnapAligment((DateTimeMeasurementUnit)e.NewValue);
		}
		static void OnGridAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DateTimeChartRangeControlClient client = d as DateTimeChartRangeControlClient;
			if (client != null && e.NewValue != null) {
				((DateTimeGridCalculator)client.ChartGridCalculator).UpdateGridAligment((DateTimeGridAlignment)e.NewValue);
				client.UpdateItems();
			}
		}
		DateTimeGridCalculator gridCalculator;
		protected  override GridCalculator ChartGridCalculator { get { return gridCalculator; } }
		[Category(Categories.Layout)]
		public DateTimeMeasurementUnit SnapAlignment {
			get { return (DateTimeMeasurementUnit)GetValue(SnapAlignmentProperty); }
			set { SetValue(SnapAlignmentProperty, value); }
		}
		[Category(Categories.Layout)]
		public DateTimeGridAlignment GridAlignment {
			get { return (DateTimeGridAlignment)GetValue(GridAlignmentProperty); }
			set { SetValue(GridAlignmentProperty, value); }
		}
		TimeSpan MinVisualRange {
			get {
				if (GridAlignment == DateTimeGridAlignment.Auto)
					return TimeSpan.FromDays(1);
				switch (GridAlignment) {
					case DateTimeGridAlignment.Millisecond:
						return TimeSpan.FromMilliseconds(1);
					case DateTimeGridAlignment.Second:
						return TimeSpan.FromSeconds(1);
					case DateTimeGridAlignment.Minute:
						return TimeSpan.FromMinutes(1);
					case DateTimeGridAlignment.Hour:
						return TimeSpan.FromHours(1);
					case DateTimeGridAlignment.Day:
						return TimeSpan.FromDays(1);
					case DateTimeGridAlignment.Week:
					case DateTimeGridAlignment.Month:
					case DateTimeGridAlignment.Quarter:
					case DateTimeGridAlignment.Year:
					default:
						return TimeSpan.FromDays(7);
				}
			}
		}
		public DateTimeChartRangeControlClient()
			: base() {
			DefaultStyleKey = typeof(DateTimeChartRangeControlClient);
			this.gridCalculator = new DateTimeGridCalculator(GridAlignment);
		}
		protected override void CorrectVisibleRange(object oldStart, object oldEnd, object newStart, object newEnd, out object correctStart, out object correctEnd) {
			TimeSpan delta = ((DateTime)newEnd - (DateTime)newStart);
			if (delta < MinVisualRange) {
				correctStart = oldStart;
				correctEnd = oldEnd;
			}
			else {
				correctStart = newStart;
				correctEnd = newEnd;
			}
		}
		protected override object ConvertToNative(double value) {
			return SparklineMathUtils.ConvertToNative(value, SparklineScaleType.DateTime);
		}
		protected override bool CheckValueType(object value) {
			return value is DateTime;
		}
		protected override object TryParseValue(object value) {
			if (value == null)
				return null;
			Type valueType = value.GetType();
			if (valueType == typeof(string)) {
				DateTime dateTime;
				if (DateTime.TryParse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
					return dateTime;
			}
			else if (valueType == typeof(DateTime))
				return value;
			return null;
		}
	}
}
