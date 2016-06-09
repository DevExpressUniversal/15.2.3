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

using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class HorizontalTimeIndicatorLayoutCalculator : TimeIndicatorLayoutCalculator {
		protected new DayViewInfo ViewInfo {
			get {
				return (DayViewInfo)base.ViewInfo;
			}
		}
		public HorizontalTimeIndicatorLayoutCalculator(DayViewInfo viewInfo, TimeIndicatorVisibility visibility)
			: base(viewInfo, visibility) {
		}
		protected override ViewInfoItemCollection CreateTimeIndicatorCore(DateTime now) {
			if (ViewInfo.View.TimeScale == null)
				return new ViewInfoItemCollection();
			TimeSpan time = CalcTimeOfDay(now);
			int currentTimeRowIndex = ViewInfo.VisibleRows.BinarySearchStartDate(DateTimeHelper.Floor(time, ViewInfo.View.TimeScale));
			if (currentTimeRowIndex < 0)
				return new ViewInfoItemCollection();
			DayViewRow row = ViewInfo.VisibleRows[currentTimeRowIndex];
			Rectangle bounds = row.Bounds;
			int position = RectUtils.CalcDateY(row.Interval, row.Bounds, time);
			return CreateCurrentTimeItems(bounds, position);
		}
		protected override TimeSpan CalcTimeOfDay(DateTime date) {
			DayView dayView = ViewInfo.View as DayView;
			if (dayView == null)
				return base.CalcTimeOfDay(date);
			TimeSpan result = date.TimeOfDay;
			if (result < dayView.VisibleTime.Start)
				result += DateTimeHelper.DaySpan;
			return result;
		}
		ViewInfoItemCollection CreateCurrentTimeItems(Rectangle bounds, int y) {
			ViewInfoItemCollection result = new ViewInfoItemCollection();
			ResourceBaseCollection resources = ViewInfo.VisibleResources;
			TimeIntervalCollection intervals = ViewInfo.VisibleIntervals;
			int resourceCount = 1;
			int intervalCount = 0;
			if (resources != null && resources.Count > 1 && ViewInfo.View.GroupType == SchedulerGroupType.Resource)
				resourceCount = resources.Count;
			if (intervals != null && intervals.Count > 0) {
				IndexPair intervalsIndx = FindNowTimeIntervalsIndx(intervals);
				intervalCount = intervals.Count;
				if (CanShowOnAllIntervals(intervalsIndx != null, Visibility)) {
					intervalsIndx = new IndexPair(0, intervalCount - 1);
					result.Add(CreateCurrentTimeItem(bounds, y, intervalsIndx, intervalCount));
					return result;
				}
				Rectangle[] resourceBounds = RectUtils.SplitHorizontally(bounds, resourceCount);
				if (intervalsIndx != null)
					for (int i = 0; i < resourceCount; i++)
						result.Add(CreateCurrentTimeItem(resourceBounds[i], y, intervalsIndx, intervalCount));
			}
			return result;
		}
		HorisontalTimeIndicatorItem CreateCurrentTimeItem(Rectangle bounds, int position, IndexPair intervalsIndx, int intervalCount) {
			Rectangle[] intervalBounds = RectUtils.SplitHorizontally(bounds, intervalCount);
			Rectangle intervalStartBounds = intervalBounds[intervalsIndx.First];
			Rectangle intervalEndBounds = intervalBounds[intervalsIndx.Last];
			HorisontalTimeIndicatorItem item = new HorisontalTimeIndicatorItem();
			bounds = Rectangle.Union(intervalStartBounds, intervalEndBounds);
			Size size = item.CalcContentSize(null, bounds);
			Point location = new Point(bounds.X, position - size.Height / 2);
			item.Bounds = new Rectangle(location, size);
			return item;
		}
	}
}
