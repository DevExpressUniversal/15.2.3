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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class MonthViewGroupCellsLayoutCalculator : SchedulerWeekBasedViewCellsLayoutCalculator {
		protected MonthViewGroupCellsLayoutCalculator(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ViewInfoPainterBase painter)
			: base(cache, viewInfo, painter) {
		}
		protected internal new MonthViewInfo ViewInfo { get { return (MonthViewInfo)base.ViewInfo; } }
		protected virtual ResourceBaseCollection VisibleResources { get { return ViewInfo.VisibleResources; } }
		public override void CreateWeeks() {
			CreateWeeksForResources();
		}
		public override void CalcWeeksLayout(Rectangle bounds) {
			for (int i = 0; i < VisibleResources.Count; i++) {
				CalcSeveralWeeksLayoutForResource(i, CalcWeekBounds(bounds, i), GetHeadersByWeekNumber(i));
			}
		}
		protected internal override void CalculatePreliminaryLayout() {
			CreateWeeks();
			CalcWeeksPreliminaryLayout();
			ViewInfo.PreliminaryLayoutResult.CellContainers.Clear();
			ViewInfo.PreliminaryLayoutResult.CellContainers.AddRange(Weeks);
		}
		#region CreateWeeksForResources
		protected internal virtual void CreateWeeksForResources() {
			for (int i = 0; i < VisibleResources.Count; i++)
				CreateWeeksForResource(VisibleResources[i], i);
		}
		#endregion
		#region CreateWeeksForResource
		protected internal virtual void CreateWeeksForResource(Resource resource, int visibleResourceIndex) {
			XtraSchedulerDebug.Assert(ViewInfo.WeekCount == View.InnerVisibleIntervals.Count);
			int count = ViewInfo.VisibleIntervals.Count;
			for (int i = 0; i < count; i++) {
				SchedulerColorSchema colorSchema = GetColorSchema(resource, visibleResourceIndex);
				MonthSingleWeekViewInfo week = (MonthSingleWeekViewInfo)GetCellContainerByResourceAndInterval(resource, new TimeInterval(View.InnerVisibleIntervals[i].Start, TimeSpan.FromDays(0)));
				if (week == null)
					week = CreateMonthResourceWeek(resource, View.InnerVisibleIntervals[i].Start, colorSchema);
				else
					week.SetViewInfo(ViewInfo);
				bool hasBottomBorder = i < count - 1;
				InitializeContainerBorders(week, hasBottomBorder);
				week.FirstVisible = (i == 0);
				Weeks.Add(week);
			}
		}
		#endregion
		protected internal virtual void InitializeContainerBorders(MonthSingleWeekViewInfo container, bool hasBottomBorder) {
			ResetContainerBorders(container);
			container.HasBottomBorder = hasBottomBorder;
		}
		protected internal MonthSingleWeekViewInfo CreateMonthResourceWeek(Resource resource, DateTime start, SchedulerColorSchema colorSchema) {
			return new MonthSingleWeekViewInfo(ViewInfo, resource, start, colorSchema);
		}
		protected internal virtual void CalcSeveralWeeksLayoutForResource(int resourceIndex, Rectangle bounds, SchedulerHeaderCollection weekDaysHeaders) {
			Rectangle[] rects = RectUtils.SplitVertically(bounds, ViewInfo.WeekCount);
			int weeksPerResource = ViewInfo.WeekCount;
			int count = ViewInfo.WeekCount;
			Rectangle[] anchorBounds = GetAnchorBounds(weekDaysHeaders);
			for (int i = 0; i < count; i++) {
				SingleWeekViewInfo week = (SingleWeekViewInfo)ViewInfo.PreliminaryLayoutResult.CellContainers[resourceIndex * weeksPerResource + i];
				week.Bounds = rects[i];
				DateTime[] weekDays = ViewInfo.GetWeekDates(week.Interval.Start);
				week.CalcLayout(weekDays, anchorBounds, Cache, (SchedulerHeaderPainter)Painter);
			}
		}
		protected void CalcWeeksPreliminaryLayout() {
			for (int i = 0; i < Weeks.Count; i++)
				Weeks[i].CalcPreliminaryLayout(ViewInfo.GetWeekDates(Weeks[i].Interval.Start), Cache, (SchedulerHeaderPainter)Painter, i > 0, i < Weeks.Count - 1);
		}
		protected abstract Rectangle CalcWeekBounds(Rectangle bounds, int weekNumber);
		protected abstract SchedulerHeaderCollection GetHeadersByWeekNumber(int weekNumber);
	}
}
