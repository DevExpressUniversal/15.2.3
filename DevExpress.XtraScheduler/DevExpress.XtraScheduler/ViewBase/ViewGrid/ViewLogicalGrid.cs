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
namespace DevExpress.XtraScheduler.Native {
	public abstract class ViewLogicalGrid {
		internal static ViewLogicalGrid Create(SchedulerViewBase view) {
			DayView dayView = view as DayView;
			if (dayView != null) {
				switch (dayView.GroupType) {
					case SchedulerGroupType.None:
						return new DayViewGrid(dayView);
					case SchedulerGroupType.Resource:
						return new DayViewGroupByResourceGrid(dayView);
					case SchedulerGroupType.Date:
						return new DayViewGroupByDateGrid(dayView);
				}
			}
			WeekView weekView = view as WeekView;
			if (weekView != null) {
				switch (weekView.GroupType) {
					case SchedulerGroupType.None:
						return new WeekViewGrid(weekView);
					case SchedulerGroupType.Resource:
						return new WeekViewGroupByResourceGrid(weekView);
					case SchedulerGroupType.Date:
						return new WeekViewGroupByDateGrid(weekView);
				}
			}
			TimelineView timelineView = view as TimelineView;
			if (timelineView != null) {
				switch (timelineView.GroupType) {
					case SchedulerGroupType.None:
						return new TimelineViewGrid(timelineView);
					case SchedulerGroupType.Date:
						return new TimelineViewGroupByDateGrid(timelineView);
					case SchedulerGroupType.Resource:
						return new TimelineViewGroupByDateGrid(timelineView);
				}
			}
			return null;
		}
		public abstract ScrollDeltaInfo CalculateVerticalDelta(int physicalOffset);
		public abstract ScrollDeltaInfo CalculateHorizontalDelta(int physicalOffset);
		public virtual void HorizontalOverScroll(int delta) {
		}
	}
}
