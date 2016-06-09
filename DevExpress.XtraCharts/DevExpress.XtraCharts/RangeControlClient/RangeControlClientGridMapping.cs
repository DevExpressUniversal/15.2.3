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
using System.Linq;
using System.Text;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class RangeControlClientGridMapping {
		public RangeControlClientGridMapping() {
		}
		protected internal abstract double CeilValue(RangeControlGridUnit unit, double value, double offset);
		protected internal abstract double FloorValue(RangeControlGridUnit unit, double value, double offset);
		protected internal abstract double GetGridValue(RangeControlGridUnit unit, double index);
	}
	public abstract class NumericRangeControlClientGridMappingBase : RangeControlClientGridMapping {
		public NumericRangeControlClientGridMappingBase() {
		}
		protected internal override double CeilValue(RangeControlGridUnit unit, double value, double offset) {
			return Math.Ceiling((value + unit.ReversedOffset) / unit.Step) + offset;
		}
		protected internal override double FloorValue(RangeControlGridUnit unit, double value, double offset) {
			return Math.Floor((value + unit.ReversedOffset) / unit.Step) + offset;
		}
		protected internal override double GetGridValue(RangeControlGridUnit unit, double index) {
			return index * unit.Step - unit.ReversedOffset;
		}
	}
	public class NumericRangeControlClientGridMapping : NumericRangeControlClientGridMappingBase {
		public NumericRangeControlClientGridMapping() : base() {
		}
	}
	public class QualitativeRangeControlClientGridMapping : NumericRangeControlClientGridMappingBase {
		public QualitativeRangeControlClientGridMapping() : base() {
		}
	}
	public class DateTimeRangeControlClientGridMapping : RangeControlClientGridMapping {
		readonly WorkdaysOptions workdaysOptions;
		public DateTimeRangeControlClientGridMapping(WorkdaysOptions workdaysOptions) {
			this.workdaysOptions = workdaysOptions;
		}
		double CorrectGridUnits(double gridUnits, RangeControlGridUnit unit, double offset, bool ceil) {
			double unitsCount = (gridUnits + unit.ReversedOffset) / unit.Spacing;
			double floorUnitsCount = Math.Floor(unitsCount);
			double precision = unitsCount - floorUnitsCount;
			double totalOffset = offset;
			if (ceil)
				totalOffset++;
			return (floorUnitsCount + totalOffset) * unit.Spacing - unit.ReversedOffset;
		}
		double RoundValue(RangeControlGridUnit unit, double value, double offset, bool ceil) {
			AlignedRangeControlGridUnit alignUnit = (AlignedRangeControlGridUnit)unit;
			DateTimeMeasureUnitNative snapAlignment = alignUnit.Alignment;
			DayOfWeek fstDayOfWeek = DateTimeUtilsExt.GetFirstDayOfWeek(workdaysOptions);
			DateTime dateTimeValue = DateTime.MinValue.AddMilliseconds(value);
			double gridUnits = DateTimeUtilsExt.Difference(DateTime.MinValue, dateTimeValue, snapAlignment, workdaysOptions);
			DateTime floorDateTimeValue = DateTimeUtilsExt.Add(DateTime.MinValue, snapAlignment, gridUnits, fstDayOfWeek);
			double floorValue = CorrectGridUnits(gridUnits, alignUnit, offset, ceil && (floorDateTimeValue < dateTimeValue));
			DateTime alignedValue = DateTimeUtilsExt.Add(DateTime.MinValue, snapAlignment, floorValue, fstDayOfWeek);
			return DateTimeUtilsExt.Difference(DateTime.MinValue, alignedValue, DateTimeMeasureUnitNative.Millisecond, workdaysOptions);
		}
		protected internal override double CeilValue(RangeControlGridUnit unit, double value, double offset) {
			return RoundValue(unit, value, offset, true);
		}
		protected internal override double FloorValue(RangeControlGridUnit unit, double value, double offset) {
			return RoundValue(unit, value, offset, false);
		}
		protected internal override double GetGridValue(RangeControlGridUnit unit, double index) {
			return index;
		}
	}
}
