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
using System.Globalization;
using DevExpress.ChartRangeControlClient.Core;
using DevExpress.Sparkline.Core;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using System.Text.RegularExpressions;
namespace DevExpress.XtraEditors {
	public enum RangeControlDateTimeGridAlignment {
		Millisecond = 0,
		Second = 1,
		Minute = 2,
		Hour = 3,
		Day = 4,
		Week = 5,
		Month = 6,
		Year = 8
	}
	public sealed class DateTimeChartRangeControlClientGridOptions : ChartRangeControlClientGridOptions {
		#region Nested Classes : DateTimeLabelFormatter, UnitSelector, AlignmentUnit, DateTimeGridMapping
		sealed class DateTimeLabelFormatter : IFormatProvider, ICustomFormatter {
			readonly DateTimeChartRangeControlClientGridOptions gridOptions;
			public DateTimeLabelFormatter(DateTimeChartRangeControlClientGridOptions gridOptions) {
				this.gridOptions = gridOptions;
			}
			#region IFormatProvider
			object IFormatProvider.GetFormat(Type formatType) {
				if (formatType == typeof(ICustomFormatter))
					return this;
				return null;
			}
			#endregion
			#region ICustomFormatter
			string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider) {
				if (!Equals(formatProvider))
					return null;
				if (string.IsNullOrWhiteSpace(format)) {
					DateTime value = Convert.ToDateTime(arg);
					DateTimeFormatInfo dateTimeInfo = DateTimeFormatInfo.CurrentInfo;
					string dateSeparator = dateTimeInfo.DateSeparator;
					string timeSeparator = dateTimeInfo.TimeSeparator;
					switch (gridOptions.gridMapping.CurrentAlignment) {
						default:
						case DateTimeMeasureUnit.Millisecond:
							string timePattern = DateTimeFormatInfo.CurrentInfo.LongTimePattern;
							timePattern = Regex.Replace(timePattern, "(:ss|:s)", "$1.fff");
							return String.Format("{0:d} {0:" + timePattern + "}", value);
						case DateTimeMeasureUnit.Second:
							return String.Format("{0:G}", value);
						case DateTimeMeasureUnit.Minute:
							return String.Format("{0:g}", value);
						case DateTimeMeasureUnit.Hour:
							return String.Format("{0:g}", value);
						case DateTimeMeasureUnit.Day:
						case DateTimeMeasureUnit.Week:
							return String.Format("{0:d}", value);
						case DateTimeMeasureUnit.Quarter:
						case DateTimeMeasureUnit.Month:
							string datePattern = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
							datePattern = Regex.Replace(datePattern, "(.d|.dd|.ddd)", "");
							datePattern = Regex.Replace(datePattern, "(^-)", "");
							datePattern = Regex.Replace(datePattern, "(M)", "MMMM");
							return String.Format("{0:" + datePattern + "}", value);
						case DateTimeMeasureUnit.Year:
							return String.Format("{0:yyyy}", value);
					}
				}
				try {
					return String.Format("{0:" + format + "}", arg);
				} catch {
					return String.Format("{0}", arg);
				}
			}
			#endregion
		}
		sealed class AlignmentUnit : GridUnit {
			readonly DateTimeMeasureUnit measureUnit;
			public DateTimeMeasureUnit MeasureUnit {
				get { return measureUnit; }
			}
			public AlignmentUnit(DateTimeMeasureUnit measureUnit, double spacing) : base(SparklineDateTimeUtils.SizeOfMeasureUnit(measureUnit), spacing) {
				this.measureUnit = measureUnit;
			}
		}
		sealed class UnitSelector {
			readonly List<AlignmentUnit> units;
			public UnitSelector() {
				this.units = new List<AlignmentUnit>();
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Millisecond, 10));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Millisecond, 20));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Millisecond, 40));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Millisecond, 80));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Millisecond, 160));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Millisecond, 320));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Millisecond, 640));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Second, 1));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Second, 2));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Second, 5));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Second, 10));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Second, 20));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Second, 40));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Minute, 1));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Minute, 2));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Minute, 5));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Minute, 10));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Minute, 20));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Minute, 40));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Hour, 1));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Hour, 2));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Hour, 3));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Hour, 6));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Hour, 12));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Day, 1));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Day, 2));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Week, 1));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Week, 2));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Month, 1));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Month, 3));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Month, 6));
				units.Add(new AlignmentUnit(DateTimeMeasureUnit.Year, 1));
			}
			public int SelectUnit(double unit) {
				double currentUnit = unit;
				for (int i = 0; i < units.Count; i++) {
					if (units[i].Step > currentUnit)
						return (i == 0) ? 0 : i - 1;
				}
				return units.Count - 1;
			}
			public AlignmentUnit GetUnit(int index) {
				if (index >= units.Count)
					index = units.Count - 1;
				return units[index];
			}
		}
		sealed class DateTimeGridMapping : IChartCoreClientGridMapping {
			readonly DateTimeChartRangeControlClientGridOptions gridOptions;
			AlignmentUnit currentUnit;
			public DateTimeMeasureUnit CurrentAlignment {
				get { return ((currentUnit == null) || !gridOptions.Auto) ? (DateTimeMeasureUnit)gridOptions.GridAlignment : currentUnit.MeasureUnit; }
			}
			public AlignmentUnit CurrentUnit {
				get { return currentUnit; }
			}
			public DateTimeGridMapping(DateTimeChartRangeControlClientGridOptions gridOptions) {
				this.gridOptions = gridOptions;
			}
			#region IChartCoreClientGridMapping
			double IChartCoreClientGridMapping.CeilValue(GridUnit unit, double value) {
				return RoundValue(unit, value, true);
			}
			double IChartCoreClientGridMapping.FloorValue(GridUnit unit, double value) {
				return RoundValue(unit, value, false);
			}
			double IChartCoreClientGridMapping.GetGridValue(GridUnit unit, double index) {
				AlignmentUnit alignmentUnit = (AlignmentUnit)unit;
				DateTime dateTimeValue = SparklineDateTimeUtils.Add(DateTime.MinValue, alignmentUnit.MeasureUnit, index * unit.Spacing);
				return SparklineDateTimeUtils.Difference(DateTime.MinValue, dateTimeValue, DateTimeMeasureUnit.Millisecond);
			}
			GridUnit IChartCoreClientGridMapping.SelectGridUnit(double unit) {
				if (unit < 1)
					unit = 1;
				UnitSelector unitSelector = gridOptions.unitSelector;
				int unitIndex = unitSelector.SelectUnit(unit);
				AlignmentUnit selectedUnit = unitSelector.GetUnit(unitIndex);
				AlignmentUnit nextUnit = unitSelector.GetUnit(unitIndex + 1);
				double difference = nextUnit.Step - selectedUnit.Step;
				if (difference != 0) {
					double threshold = (unit - selectedUnit.Step) / difference;
					if (threshold >= 0.5)
						selectedUnit = nextUnit;
				} else {
					selectedUnit = new AlignmentUnit(selectedUnit.MeasureUnit, Math.Ceiling(unit / selectedUnit.Unit));
				}
				currentUnit = selectedUnit;
				return selectedUnit;
			}
			#endregion
			double RoundValue(GridUnit unit, double value, bool ceil) {
				AlignmentUnit alignmentUnit = (AlignmentUnit)unit;
				DateTimeMeasureUnit measureUnit = alignmentUnit.MeasureUnit;
				double spacing = unit.Spacing;
				DateTime dateTimeValue = DateTime.MinValue.AddMilliseconds(value);
				double difference = SparklineDateTimeUtils.Difference(DateTime.MinValue, dateTimeValue, measureUnit);
				DateTime floorDate = SparklineDateTimeUtils.Add(DateTime.MinValue, measureUnit, difference);
				ceil = ceil & (floorDate < dateTimeValue);
				double aligned = Math.Max(0.0, Math.Floor(difference / spacing) * spacing);
				DateTime alignedDate = SparklineDateTimeUtils.Add(DateTime.MinValue, measureUnit, aligned + (ceil ? spacing : 0));
				return Math.Round(SparklineDateTimeUtils.Difference(DateTime.MinValue, alignedDate, measureUnit) / spacing);
			}
			public void CleanOnDisableAuto() {
				currentUnit = null;
			}
		}
		#endregion
		const RangeControlDateTimeGridAlignment DefaultGridAlignment = RangeControlDateTimeGridAlignment.Day;
		const RangeControlDateTimeGridAlignment DefaultSnapAlignment = RangeControlDateTimeGridAlignment.Day;
		RangeControlDateTimeGridAlignment gridAlignment = DefaultGridAlignment;
		RangeControlDateTimeGridAlignment snapAlignment = DefaultSnapAlignment;
		DateTimeLabelFormatter labelFormatter;
		UnitSelector unitSelector;
		DateTimeGridMapping gridMapping;
		protected override GridUnit CoreGridUnit {
			get { return new AlignmentUnit((DateTimeMeasureUnit)GridAlignment, GridSpacing); }
		}
		protected override GridUnit CoreSnapUnit {
			get {
				if (Auto && (gridMapping.CurrentUnit != null))
					return gridMapping.CurrentUnit;
				return new AlignmentUnit((DateTimeMeasureUnit)SnapAlignment, SnapSpacing); 
			}
		}
		protected override double PixelPerUnit {
			get { return 250; }
		}
		protected override IChartCoreClientGridMapping GridMapping {
			get { return gridMapping; }
		}
		protected override IFormatProvider LabelFormatProviderInternal {
			get { return labelFormatter; }
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.DateTimeChartRangeControlClientGridOptions.GridAlignment"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DateTimeChartRangeControlClientGridOptionsGridAlignment"),
#endif
		XtraSerializableProperty
		]
		public RangeControlDateTimeGridAlignment GridAlignment {
			get { return gridAlignment; }
			set {
				if (Auto) {
					DisableAuto();
					gridAlignment = value;
					RaiseChanged();
				} else if (gridAlignment != value) {
					gridAlignment = value;
					RaiseChanged();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.DateTimeChartRangeControlClientGridOptions.SnapAlignment"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DateTimeChartRangeControlClientGridOptionsSnapAlignment"),
#endif
		XtraSerializableProperty
		]
		public RangeControlDateTimeGridAlignment SnapAlignment {
			get { return snapAlignment; }
			set {
				if (Auto) {
					DisableAuto();
					snapAlignment = value;
					RaiseChanged();
				} else if (snapAlignment != value) {
					snapAlignment = value;
					RaiseChanged();
				}
			}
		}
		public DateTimeChartRangeControlClientGridOptions() {
			this.labelFormatter = new DateTimeLabelFormatter(this);
			this.gridMapping = new DateTimeGridMapping(this);
			this.unitSelector = new UnitSelector();
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeGridAlignment() {
			return ShouldSerializeAuto() && gridAlignment != DefaultGridAlignment;
		}
		bool ShouldSerializeSnapAlignment() {
			return ShouldSerializeAuto() && snapAlignment != DefaultSnapAlignment;
		}
		void ResetGridAlignment() {
			GridAlignment = DefaultGridAlignment;
		}
		void ResetSnapAlignment() {
			SnapAlignment = DefaultSnapAlignment;
		}
		#endregion
		protected override void PushAutoUnitToProperties() {
			AlignmentUnit selectedUnit = gridMapping.CurrentUnit;
			if (selectedUnit != null) {
				GridAlignment = (RangeControlDateTimeGridAlignment)selectedUnit.MeasureUnit;
				SnapAlignment = GridAlignment;
				GridSpacing = selectedUnit.Spacing;
				SnapSpacing = GridSpacing;
			} else {
				GridAlignment = DefaultGridAlignment;
				SnapAlignment = DefaultSnapAlignment;
				GridSpacing = DefaultGridSpacing;
				SnapSpacing = DefaultSnapSpacing;
			}
			gridMapping.CleanOnDisableAuto();
		}
		protected override bool ValidateLabelFormat(string format) {
			if (!string.IsNullOrEmpty(format)) {
				DateTime anyDateTimeValue = DateTime.Now;
				try {
					anyDateTimeValue.ToString(format);
				} catch {
					return false;
				}
			}
			return true;
		}
		protected internal override object GetNativeGridValue(double value) {
			return SparklineDateTimeUtils.Add(DateTime.MinValue, DateTimeMeasureUnit.Millisecond, value);
		}
	}
}
