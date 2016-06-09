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
namespace DevExpress.Charts.Native {
	public class DateTimeUnitSelector : UnitSelector<DateTimeMeasureUnitNative, DateTimeGridAlignmentNative> {
		const double sizeMillisecond = 1.0;
		const double sizeSecond = sizeMillisecond * 1000.0;
		const double sizeMinute = sizeSecond * 60.0;
		const double sizeHour = sizeMinute * 60.0;
		const double sizeDay = sizeHour * 24.0;
		const double sizeWeek = sizeDay * 7.0;
		const double sizeMonth = sizeDay * 30.4375;
		const double sizeQuarter = sizeMonth * 3.0;
		const double sizeYear = sizeDay * 365.25;
		public const double DefaultGridSpacing = 1.0;
		public static DateTimeMeasureUnitNative DefaultMeasureUnit { get { return DateTimeMeasureUnitNative.Day; } }
		public static DateTimeGridAlignmentNative DefaultGridAlignment { get { return DateTimeGridAlignmentNative.Day; } }
		public static double GetMeasureUnitValue(DateTimeMeasureUnitNative measureUnit) {
			switch (measureUnit) {
				case DateTimeMeasureUnitNative.Millisecond:
					return sizeMillisecond;
				case DateTimeMeasureUnitNative.Second:
					return sizeSecond;
				case DateTimeMeasureUnitNative.Minute:
					return sizeMinute;
				case DateTimeMeasureUnitNative.Hour:
					return sizeHour;
				case DateTimeMeasureUnitNative.Day:
					return sizeDay;
				case DateTimeMeasureUnitNative.Week:
					return sizeWeek;
				case DateTimeMeasureUnitNative.Month:
					return sizeMonth;
				case DateTimeMeasureUnitNative.Quarter:
					return sizeQuarter;
				case DateTimeMeasureUnitNative.Year:
					return sizeYear;
				default:
					throw new ArgumentException();
			}
		}
		readonly UnitContainer commonUnitContainer;
		List<UnitContainer.AlignmentItem> alignmentSteps;
		public DateTimeUnitSelector() {
			List<UnitContainer.MeasureItem> measureSteps = new List<UnitContainer.MeasureItem>();
			measureSteps.Add(new UnitContainer.MeasureItem(sizeMillisecond, DateTimeMeasureUnitNative.Millisecond));
			measureSteps.Add(new UnitContainer.MeasureItem(sizeSecond, DateTimeMeasureUnitNative.Second));
			measureSteps.Add(new UnitContainer.MeasureItem(sizeMinute, DateTimeMeasureUnitNative.Minute));
			measureSteps.Add(new UnitContainer.MeasureItem(sizeHour, DateTimeMeasureUnitNative.Hour));
			measureSteps.Add(new UnitContainer.MeasureItem(sizeDay, DateTimeMeasureUnitNative.Day));
			measureSteps.Add(new UnitContainer.MeasureItem(sizeWeek, DateTimeMeasureUnitNative.Week));
			measureSteps.Add(new UnitContainer.MeasureItem(sizeMonth, DateTimeMeasureUnitNative.Month));
			measureSteps.Add(new UnitContainer.MeasureItem(sizeQuarter, DateTimeMeasureUnitNative.Quarter));
			measureSteps.Add(new UnitContainer.MeasureItem(sizeYear, DateTimeMeasureUnitNative.Year));
			alignmentSteps = new List<UnitContainer.AlignmentItem>();
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeMillisecond, DateTimeGridAlignmentNative.Millisecond));
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeSecond, DateTimeGridAlignmentNative.Second));
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeMinute, DateTimeGridAlignmentNative.Minute));
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeHour, DateTimeGridAlignmentNative.Hour));
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeDay, DateTimeGridAlignmentNative.Day));
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeWeek, DateTimeGridAlignmentNative.Week));
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeMonth, DateTimeGridAlignmentNative.Month));
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeQuarter, DateTimeGridAlignmentNative.Quarter));
			alignmentSteps.Add(new UnitContainer.AlignmentItem(sizeYear, DateTimeGridAlignmentNative.Year));
			this.commonUnitContainer = new UnitContainer();
			this.commonUnitContainer.UpdateActiveUnits(measureSteps, alignmentSteps);
			ActiveUnitContainer.UpdateActiveUnits(measureSteps, alignmentSteps);
		}
		IList<UnitContainer.MeasureItem> FilterActiveMeasureUnits(double min, double max) {
			var minUnit = commonUnitContainer.SelectMeasureUnit(min, min);
			var maxUnit = commonUnitContainer.SelectMeasureUnit(max, max);
			var activeUnits = new List<UnitContainer.MeasureItem>();
			for (int i = 0; i < commonUnitContainer.MeasureUnitCount; i++) {
				var measureItem = commonUnitContainer.GetMeasureUnitAt(i);
				if ((measureItem.Threshold >= minUnit.Threshold) && (measureItem.Threshold <= maxUnit.Threshold))
					activeUnits.Add(measureItem);
			}
			if (activeUnits.Count == 0) {
				if (maxUnit.Threshold <= commonUnitContainer.GetMeasureUnitAt(0).Threshold)
					activeUnits.Add(commonUnitContainer.GetMeasureUnitAt(0));
				else
					activeUnits.Add(commonUnitContainer.GetMeasureUnitAt(commonUnitContainer.MeasureUnitCount - 1));
			}
			return activeUnits;
		}
		IList<UnitContainer.AlignmentItem> FilterActiveAlignmentUnits(double min, double max) {
			var minUnit = commonUnitContainer.SelectAlignmentUnit(min, min);
			var maxUnit = commonUnitContainer.SelectAlignmentUnit(max, max);
			var activeUnits = new List<UnitContainer.AlignmentItem>();
			for (int i = 0; i < commonUnitContainer.AlignmentUnitCount; i++) {
				var alignmentItem = commonUnitContainer.GetAlignmentUnitAt(i);
				if ((alignmentItem.Threshold >= minUnit.Threshold) && (alignmentItem.Threshold <= maxUnit.Threshold))
					activeUnits.Add(alignmentItem);
			}
			if (activeUnits.Count == 0) {
				if (maxUnit.Threshold <= commonUnitContainer.GetMeasureUnitAt(0).Threshold)
					activeUnits.Add(commonUnitContainer.GetAlignmentUnitAt(0));
				else
					activeUnits.Add(commonUnitContainer.GetAlignmentUnitAt(commonUnitContainer.MeasureUnitCount - 1));
			}
			return activeUnits;
		}
		public override void UpdateActiveUnits(double min, double max) {
			ActiveUnitContainer.UpdateActiveUnits(FilterActiveMeasureUnits(min, max), FilterActiveAlignmentUnits(min, max));
		}
	}
	public class DateTimeMeasureUnitsCalculatorCore : MeasureUnitsCalculatorBase<DateTimeMeasureUnitNative, DateTimeGridAlignmentNative> {
		readonly double labelLength;
		double previousThreshold;
		IDateTimeScaleOptions DateTimeScaleOptions { get { return Axis.DateTimeScaleOptions; } }
		protected override bool UseMinMeasureUnit { get { return false; } }
		protected override IScaleOptionsBase<DateTimeMeasureUnitNative> ScaleOptions { get { return DateTimeScaleOptions; } } 
		public DateTimeMeasureUnitsCalculatorCore(IAxisData axis, double labelLength) : base(axis) {
			this.labelLength = labelLength;
		}
		DateTimeUnitSelector.GridAlignment CalculateGridAlignment(double pixelsPerUnit) {
			double threshold = CurrentMeasureUnit / pixelsPerUnit * labelLength;
			DateTimeUnitSelector.GridAlignment selectedAlignment = UnitSelector.SelectAlignment(threshold, previousThreshold);
			previousThreshold = DateTimeUnitSelector.GetMeasureUnitValue((DateTimeMeasureUnitNative)selectedAlignment.Unit);
			return selectedAlignment;
		}
		protected override UnitSelector<DateTimeMeasureUnitNative, DateTimeGridAlignmentNative> CreateUnitSelector() {
			return new DateTimeUnitSelector();
		}
		protected override void UpdateAutomaticGrid(DateTimeMeasureUnitNative measureUnit, double pixelsPerUnit) {
			DateTimeUnitSelector.GridAlignment selectedAlignment = CalculateGridAlignment(pixelsPerUnit);
			DateTimeScaleOptions.UpdateAutomaticGrid(selectedAlignment.Unit, selectedAlignment.Spacing);
		}
		protected override bool UpdateAutomaticUnits(DateTimeMeasureUnitNative measureUnit, int pixelsPerUnit) {
			DateTimeUnitSelector.GridAlignment selectedAlignment = CalculateGridAlignment(pixelsPerUnit);
			return DateTimeScaleOptions.UpdateAutomaticUnits(measureUnit, selectedAlignment.Unit, selectedAlignment.Spacing);
		}
	}
}
