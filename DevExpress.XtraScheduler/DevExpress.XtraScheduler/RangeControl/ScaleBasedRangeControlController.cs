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
using System.Text;
using DevExpress.XtraScheduler.Drawing;
using System.Drawing;
using DevExpress.Utils;
using System.Collections.Specialized;
using DevExpress.XtraEditors;
using System.ComponentModel;
namespace DevExpress.XtraScheduler.Native {
	public class ScaleBasedRangeControlController : ScaleBasedRangeControlControllerBase {
		public TimeInterval RoundInterval(DateTime start, DateTime end, RangeControlValidationType type) {
			bool isSelected = IsValidatedRangeSelected(start, end, type);
			DateTime roundedStart = isSelected ? BaseScale.Floor(start) : BaseScale.Round(start);
			DateTime roundedEnd = BaseScale.Round(end);
			if (roundedEnd == roundedStart)
				roundedEnd = BaseScale.GetNextDate(roundedStart);
			return new TimeInterval(roundedStart, roundedEnd);
		}
		protected virtual bool IsValidatedRangeSelected(DateTime start, DateTime end, RangeControlValidationType type) { 
			return start == end || type == RangeControlValidationType.Selection;
		}
		protected internal string FindOptimalDateTimeFormat(TimeScale scale, DateTime[] dates, Graphics gr, Font font, int width) {
			ITimeScaleDateTimeFormatter formatter = TimeScaleDateTimeFormatterFactory.Default.CreateFormatter(scale);
			return DateTimeFormatHelper.FindOptimalDateTimeFormat(formatter, dates, width, gr, font);
		}
		public string FormatRulerOptimalFormatCaption(int rulerIndex, string optimalFormat, DateTime start, DateTime end) {
			ISchedulerRangeControlRuler ruler = Rulers[rulerIndex];
			TimeScale scale = ruler.Scale;
			if (string.IsNullOrEmpty(optimalFormat))
				return FormatRulerFixedFormatCaption(rulerIndex, start, end);
			ITimeScaleDateTimeFormatter formatter = TimeScaleDateTimeFormatterFactory.Default.CreateFormatter(scale);
			return formatter.Format(optimalFormat, start, scale.GetNextDate(start));
		}
		public virtual NormalizedRangeInfo ValidateRange(DateTime start, DateTime end, RangeControlValidationType type) {
			TimeInterval roundedInterval = RoundInterval(start, end, type);
			DateTime maxEnd = roundedInterval.End;
			if (MaxSelectedIntervalCount > 0) {
				maxEnd = CalculateMaximumSelectedEnd(roundedInterval.Start, MaxSelectedIntervalCount);
			}
			roundedInterval.End = DateTimeHelper.Min(roundedInterval.End, maxEnd);
			return CreateNormalizedRangeInfo(roundedInterval.Start, roundedInterval.End);
		}
		protected NormalizedRangeInfo CreateNormalizedRangeInfo(DateTime min, DateTime max) {
			NormalizedRangeInfo result = new NormalizedRangeInfo();
			result.Range.Minimum = GetNormalizedValue(min);
			result.Range.Maximum = GetNormalizedValue(max);
			return result;
		}
		public virtual RangeControlRange PrepareRangeForUpdate() {
			DateTime start = DataProvider.SelectedRangeStart;
			SchedulerControlClientDataProvider controlDataProvider = DataProvider as SchedulerControlClientDataProvider;
			if (controlDataProvider != null && controlDataProvider.InnerControl.ActiveViewType == SchedulerViewType.Month) {
				DateTime newStart = BaseScale.Floor(start);
				if (newStart - start > TimeSpan.FromDays(6))
					start = newStart;
			} else
				start = BaseScale.Floor(start);
			DateTime end = DataProvider.SelectedRangeEnd;
			if (start < Minimum)
				start = Minimum;
			if (end < Minimum)
				end = Minimum;
			NormalizedRangeInfo validatedInfo = ValidateRange(start, end, RangeControlValidationType.Range);
			return new RangeControlRange(GetValue(validatedInfo.Range.Minimum), GetValue(validatedInfo.Range.Maximum));
		}
		#region DateTime to value conversion
		public override DateTime GetValue(double normalizedValue) {
			double maxPosition = GetComparableRangeInterval();
			double position = normalizedValue * maxPosition;
			return Minimum + TimeSpan.FromTicks((long)Math.Round(position));
		}
		public override double GetNormalizedValue(object value) {
			double maxPosition = GetComparableRangeInterval();
			DateTime dt = Convert.ToDateTime(value);
			double pos = (dt - Minimum).Ticks;
			return pos / maxPosition;
		}
		public override double GetComparableValue(DateTime value) {
			return value.Ticks;
		}
		public override DateTime GetRealValue(double comparableValue) {
			return new DateTime((long)comparableValue);
		}
		#endregion
	}
}
