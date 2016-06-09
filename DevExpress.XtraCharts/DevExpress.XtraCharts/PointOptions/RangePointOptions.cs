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
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	public abstract class RangePointOptions : PointOptions {
		const bool defaultValueAsDuration = false;
		const TimeSpanFormat defaultValueDurationFormat = TimeSpanFormat.Standard;
		#region fields and properties
		bool valueAsDuration = defaultValueAsDuration;
		TimeSpanFormat valueDurationFormat = defaultValueDurationFormat;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangePointOptionsValueAsDuration"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangePointOptions.ValueAsDuration"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool ValueAsDuration {
			get { return valueAsDuration; }
			set {
				if (value != valueAsDuration) {
					SendNotification(new ElementWillChangeNotification(this));
					valueAsDuration = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RangePointOptionsValueDurationFormat"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RangePointOptions.ValueDurationFormat"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public TimeSpanFormat ValueDurationFormat {
			get { return valueDurationFormat; }
			set {
				if (value != valueDurationFormat) {
					SendNotification(new ElementWillChangeNotification(this));
					valueDurationFormat = value;
					RaiseControlChanged();
				}
			}
		}
		#endregion
		public RangePointOptions() {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if(propertyName == "ValueAsDuration")
				return ShouldSerializeValueAsDuration();
			if(propertyName == "ValueDurationFormat")
				return ShouldSerializeValueDurationFormat();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeValueAsDuration() {
			return false;
		}
		bool ShouldSerializeValueDurationFormat() {
			return false;
		}
		#endregion
		internal RangeValueToStringConveter CreateRangeValueToStringConverter(RangeValueToStringConveter.Mode mode) {
			if(valueAsDuration)
				mode = RangeValueToStringConveter.Mode.Duration;
			return CreateConverter(mode);
		}
		protected internal virtual RangeValueToStringConveter CreateConverter(RangeValueToStringConveter.Mode mode) {
			return new RangeValueToStringConveter(ValueNumericOptions, ValueDateTimeOptions, mode, valueDurationFormat);
		}
		protected internal override ValueToStringConverter CreateValueToStringConverter() {
			return CreateRangeValueToStringConverter(RangeValueToStringConveter.Mode.All);
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj))
				return false;
			RangePointOptions options = obj as RangePointOptions;
			return
				options != null &&
				valueDurationFormat == options.ValueDurationFormat &&
				valueAsDuration == options.ValueAsDuration;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RangePointOptions options = obj as RangePointOptions;
			if(options == null)
				return;
			valueDurationFormat = options.valueDurationFormat;
			valueAsDuration = options.valueAsDuration;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[
	TypeConverter(typeof(RangeBarPointOptionsTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RangeBarPointOptions : RangePointOptions {
		protected override ChartElement CreateObjectForClone() {
			return new RangeBarPointOptions();
		}
	}
	[
	TypeConverter(typeof(RangeAreaPointOptionsTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RangeAreaPointOptions : RangePointOptions {
		protected internal override RangeValueToStringConveter CreateConverter(RangeValueToStringConveter.Mode mode) {
			return new RangeAreaValueToStringConveter(ValueNumericOptions, ValueDateTimeOptions, mode, ValueDurationFormat);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RangeAreaPointOptions();
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class RangeValueToStringConveter : ValueToStringConverter {
		#region inner classes
		public enum Mode {
			All,
			Max,
			Min,
			Duration
		}
		#endregion
		Mode mode;
		TimeSpanFormat durationFormat;
		protected Mode ConverterMode { get { return mode; } }
		protected virtual bool ShouldSortValues { get { return true; } }
		public RangeValueToStringConveter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, Mode mode, TimeSpanFormat durationFormat) : base(numericOptions, dateTimeOptions) {
			this.mode = mode;
			this.durationFormat = durationFormat;
		}
		string GetDurationString(double minValue, double maxValue) {
			return NumericOptionsHelper.GetValueText(Math.Abs(maxValue - minValue), NumericOptions);
		}
		string GetDurationString(DateTime minValue, DateTime maxValue) {
			TimeSpan span = maxValue - minValue;
			switch (durationFormat) {
				case TimeSpanFormat.Standard:
					return span.ToString();
				case TimeSpanFormat.TotalDays:
					return NumericOptionsHelper.GetValueText(span.TotalDays, NumericOptions);
				case TimeSpanFormat.TotalHours:
					return NumericOptionsHelper.GetValueText(span.TotalHours, NumericOptions);
				case TimeSpanFormat.TotalMinutes:
					return NumericOptionsHelper.GetValueText(span.TotalMinutes, NumericOptions);
				case TimeSpanFormat.TotalSeconds:
					return NumericOptionsHelper.GetValueText(span.TotalSeconds, NumericOptions);
				case TimeSpanFormat.TotalMilliseconds:
					return NumericOptionsHelper.GetValueText(span.TotalMilliseconds, NumericOptions);
				default:
					throw new DefaultSwitchException();
			}
		}
		string GetDurationString(object[] values) {
			if(values[0] is double && values[1] is double)
				return GetDurationString((double)values[0], (double)values[1]);
			else if(values[0] is DateTime && values[1] is DateTime)
				return GetDurationString((DateTime)values[0], (DateTime)values[1]);
			else
				throw new ArgumentException();
		}		
		object[] SortValues(object[] values) {
			object[] sortedValues = new object[2];
			Array.Copy(values, 0, sortedValues, 0, 2);
			Array.Sort(sortedValues);
			return sortedValues;
		}
		object[] FilterValues(object[] values) {
			if (ShouldSortValues)
				values = SortValues(values);
			switch (this.mode) {
				case Mode.All:
				case Mode.Duration:
					return values;
				case Mode.Max:
					return new object[] { values[1] };
				case Mode.Min:
					return new object[] { values[0] };
				default:
					throw new DefaultSwitchException();
			}
		}
		public override string ConvertTo(object[] values) {
			values = FilterValues(values);
			if (mode == Mode.Duration)
				return GetDurationString(values);
			string result = GetValueText(values[0]);
			for (int i = 1; i < values.Length; i++)
				result += ", " + GetValueText(values[i]);
			return result;
		}		
	}
	public class RangeAreaValueToStringConveter : RangeValueToStringConveter {
		public RangeAreaValueToStringConveter(INumericOptions numericOptions, IDateTimeOptions dateTimeOptions, Mode mode, TimeSpanFormat durationFormat)
			: base(numericOptions, dateTimeOptions, mode, durationFormat) { }
		protected override bool ShouldSortValues {
			get {
				if (ConverterMode == Mode.All)
					return false;
				return base.ShouldSortValues;
			}
		}
	}
}
