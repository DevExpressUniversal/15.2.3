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
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class VerticalTimeIndicatorLayoutCalculator : TimeIndicatorLayoutCalculator {
		protected new TimelineViewInfo ViewInfo {
			get {
				return (TimelineViewInfo)base.ViewInfo;
			}
		}
		public VerticalTimeIndicatorLayoutCalculator(TimelineViewInfo viewInfo, TimeIndicatorVisibility visibility)
			: base(viewInfo, visibility) {
		}
		protected override ViewInfoItemCollection CreateTimeIndicatorCore(DateTime now) {
			TimeIntervalCollection intervals = ViewInfo.VisibleIntervals;
			int currentTimeColumnIndex = BinarySearchStartDate(intervals, now);
			if (currentTimeColumnIndex < 0 || ViewInfo.Timelines.Count < 1)
				return new ViewInfoItemCollection();
			SchedulerViewCellContainer timeline = ViewInfo.Timelines[0];
			SchedulerViewCellBase cell = timeline.Cells[currentTimeColumnIndex];
			int position = CalculateLocationX(intervals[currentTimeColumnIndex], cell.Bounds, now);
			Rectangle bounds = timeline.Bounds;
			bounds.Height = ViewInfo.CalculateCellsTotalHeight();
			return CreateCurrentTimeItems(bounds, position);
		}
		ViewInfoItemCollection CreateCurrentTimeItems(Rectangle bounds, int position) {
			ViewInfoItemCollection result = new ViewInfoItemCollection();
			result.Add(CreateCurrentTimeItem(bounds, position));
			return result;
		}
		VerticalTimeIndicatorItem CreateCurrentTimeItem(Rectangle bounds, int position) {
			VerticalTimeIndicatorItem item = new VerticalTimeIndicatorItem();
			Size size = item.CalcContentSize(null, bounds);
			Point location = new Point(position - size.Width / 2, bounds.Y);
			item.Bounds = new Rectangle(location, size);
			return item;
		}
		int BinarySearchStartDate(TimeIntervalCollection intervals, DateTime now) {
			int intervalCount = intervals.Count;
			for (int i = 0; i < intervalCount; i++) {
				if (intervals[i].Start <= now && intervals[i].End >= now)
					return i;
			}
			return -1;
		}
		int CalculateLocationX(TimeInterval what, Rectangle whatRect, DateTime time) {
			if (!what.Contains(time))
				return -1;
			float offsetRatio = (time - what.Start).Ticks / (float)what.Duration.Ticks;
			int offset = (int)(whatRect.Width * offsetRatio);
			return whatRect.X + offset;
		}
	}
}
