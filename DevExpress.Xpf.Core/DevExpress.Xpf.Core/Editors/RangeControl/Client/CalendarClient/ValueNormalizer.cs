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
namespace DevExpress.Xpf.Editors.RangeControl.Internal {
	public interface IValueNormalizer {
		double GetComparableValue(object realValue);
		object GetRealValue(double comparable);
	}
	public abstract class ValueNormalizerBase : IValueNormalizer {
		public abstract double GetComparableValue(object realValue);
		public abstract object GetRealValue(double comparable);
	}
	public class DateTimeValueNormalizer : ValueNormalizerBase {
		const long TicksPerMillisecond = 10000;
		const long TicksPerSecond = TicksPerMillisecond * 1000;
		const long TicksPerMinute = TicksPerSecond * 60;
		const long TicksPerHour = TicksPerMinute * 60;
		const long TicksPerDay = TicksPerHour * 24;
		const long DoubleDateOffset = DaysTo1899 * TicksPerDay;
		const int DaysPerYear = 365;
		const int DaysPer4Years = DaysPerYear * 4 + 1;	   
		const int DaysPer100Years = DaysPer4Years * 25 - 1;  
		const int DaysPer400Years = DaysPer100Years * 4 + 1; 
		const int DaysTo1899 = DaysPer400Years * 4 + DaysPer100Years * 3 - 367;
		const int DaysTo10000 = DaysPer400Years * 25 - 366;  
		const int MillisPerSecond = 1000;
		const int MillisPerMinute = MillisPerSecond * 60;
		const int MillisPerHour = MillisPerMinute * 60;
		const int MillisPerDay = MillisPerHour * 24;
		const long OADateMinAsTicks = (DaysPer100Years - DaysPerYear) * TicksPerDay;
		const double OADateMinAsDouble = -657435.0;
		const double OADateMaxAsDouble = 2958466.0;
		const long MaxMillis = (long)DaysTo10000 * MillisPerDay;
		public override double GetComparableValue(object realValue) {
			if(realValue == null || realValue.GetType() != typeof(DateTime)) 
				return 0;
			return ToOADate(Convert.ToDateTime(realValue));
		}
		public override object GetRealValue(double comparable) {
			return FromOADate(comparable);
		}
		public double GetComparableDuration(TimeSpan duration) {
			return TicksToOADate(duration.Ticks);
		}
		public TimeSpan GetRealDuration(double duration) {
			return FromOATimeSpan(duration);
		}
		static double TicksToOADate(long value) {
			if (value == 0)
				return 0.0;
			if (value < TicksPerDay)
				value += DoubleDateOffset;
			long millis = (value - DoubleDateOffset) / TicksPerMillisecond;
			if (millis < 0) {
				long frac = millis % MillisPerDay;
				if (frac != 0)
					millis -= (MillisPerDay + frac) * 2;
			}
			return (double)millis / MillisPerDay;
		}
		double ToOADate(DateTime dt) {
			return TicksToOADate(dt.Ticks);
		}
		static DateTime FromOADate(double d) {
			return new DateTime(DoubleDateToTicks(d), DateTimeKind.Unspecified);
		}
		static TimeSpan FromOATimeSpan(double d) {
			return new TimeSpan(DoubleDateToTicks(d));
		}
		internal static long DoubleDateToTicks(double value) {
			if (value >= OADateMaxAsDouble || value <= OADateMinAsDouble)
				throw new ArgumentException("Arg_OleAutDateInvalid");
			long millis = (long)(value * MillisPerDay + (value >= 0 ? 0.5 : -0.5));
			if (millis < 0) {
				millis -= (millis % MillisPerDay) * 2;
			}
			millis += DoubleDateOffset / TicksPerMillisecond;
			if (millis < 0 || millis >= MaxMillis)
				throw new ArgumentException("Arg_OleAutDateScale");
			return millis * TicksPerMillisecond;
		}
	}
	public class NumericValueNormalizer : ValueNormalizerBase {
		public override double GetComparableValue(object realValue) {
			return Convert.ToDouble(realValue);
		}
		public override object GetRealValue(double comparable) {
			return comparable;
		}
	}
}
