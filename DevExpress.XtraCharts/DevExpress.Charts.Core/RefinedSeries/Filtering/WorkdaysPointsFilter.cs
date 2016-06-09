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
namespace DevExpress.Charts.Native {
	public class WorkdaysPointsFilter : PointsFilter {
		WeekdayCore workdays;
		List<DateTime> holidays = new List<DateTime>();
		List<DateTime> exactWorkdays = new List<DateTime>();
		IList<RefinedPoint> filteredPoints;
		IAxisData ArgumentAxisData {
			get {
				IXYSeriesView view = RefinedSeries.SeriesView as IXYSeriesView;
				return view != null ? view.AxisXData : null;
			}
		}
		public override bool NeedSortedByArgumentPoints { get { return true; } }
		public WorkdaysPointsFilter(RefinedSeries refinedSeries) : base(refinedSeries) {
		}
		bool IsEnabled() {
			if (RefinedSeries.ArgumentScaleType == ActualScaleType.DateTime) {
				IWorkdaysOptions options = GetOptions();
				return options != null && options.WorkdaysOnly;
			}
			return false;			
		}
		IWorkdaysOptions GetOptions() {
			IAxisData axisData = ArgumentAxisData;
			return axisData == null ? null :
				axisData.DateTimeScaleOptions.WorkdaysOptions;
		}		
		bool IsHoliday(DateTime dateTime) {
			bool isHoliday = false;
			DayOfWeek dayOfWeek = dateTime.DayOfWeek;
			switch (dayOfWeek) {
				case DayOfWeek.Sunday:
					if ((workdays & WeekdayCore.Sunday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Monday:
					if ((workdays & WeekdayCore.Monday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Tuesday:
					if ((workdays & WeekdayCore.Tuesday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Wednesday:
					if ((workdays & WeekdayCore.Wednesdey) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Thursday:
					if ((workdays & WeekdayCore.Thursday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Friday:
					if ((workdays & WeekdayCore.Friday) == 0)
						isHoliday = true;
					break;
				case DayOfWeek.Saturday:
					if ((workdays & WeekdayCore.Saturday) == 0)
						isHoliday = true;
					break;
			}
			foreach (DateTime holiday in holidays)
				if (dateTime.Date == holiday.Date) {
					isHoliday = true;
					break;
				}
			foreach (DateTime workday in exactWorkdays)
				if (dateTime.Date == workday.Date) {
					isHoliday = false;
					break;
				}
			return isHoliday;
		}
		IList<RefinedPoint> FilterPoints(IList<RefinedPoint> initialPoints) {
			if (initialPoints != null) {
				List<RefinedPoint> filteredPoints = new List<RefinedPoint>(initialPoints.Count);
				for (int i = 0; i < initialPoints.Count; i++) {
					RefinedPoint point = initialPoints[i];
					if (!IsHoliday(point.SeriesPoint.DateTimeArgument))
						filteredPoints.Add(point);
				}
				return filteredPoints;
			}
			return initialPoints;
		}
		protected override void Recalculate(IList<RefinedPoint> initialPoints) {
			if (initialPoints != null) 
				filteredPoints = FilterPoints(initialPoints);			
		}
		protected override IList<RefinedPoint> GetCachedPoints() {
			return filteredPoints;
		}
		public override bool Update() {
			bool invalid = false;
			if (Enable != IsEnabled()) {
				Enable = IsEnabled();
				invalid = true;
			} 
			if (Enable) {
				IWorkdaysOptions options = GetOptions();
				if (workdays != options.Workdays) {
					workdays = options.Workdays;
					invalid = true;
				}
				if (!options.Holidays.SequenceEqual(holidays)) {
					holidays.Clear();
					foreach (DateTime item in options.Holidays)
						holidays.Add(item);
					invalid = true;
				}
				if (!options.ExactWorkdays.SequenceEqual(exactWorkdays)) {
					exactWorkdays.Clear();
					foreach (DateTime item in options.ExactWorkdays)
						exactWorkdays.Add(item);
					invalid = true;
				}
			}
			if (invalid)
				ClearCache();			
			return invalid;
		}
		public override void ClearCache() {
			filteredPoints = null;
		}
	}
}
