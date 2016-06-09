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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Localization;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
#endif
namespace DevExpress.XtraScheduler.Native {
	public interface IWorkWeekViewProperties : IDayViewProperties {
	}
	public abstract class InnerWeekViewBase : InnerDayView {
		protected InnerWeekViewBase(IInnerSchedulerViewOwner owner, IDayViewProperties properties)
			: base(owner, properties) {
		}   
		protected abstract bool IsFullWeek {get;}
		protected internal override TimeInterval RoundLimitInterval(TimeInterval interval) {
			DayIntervalCollection startIntervals = CreateValidIntervalsCore(interval.Start);
			DayIntervalCollection endIntervals = CreateValidIntervalsCore(interval.End.AddTicks(-1));
			DateTime start = startIntervals.Start;
			DateTime end = endIntervals.End;
			if (startIntervals.End <= interval.Start)
				start += DateTimeHelper.WeekSpan;
			if (endIntervals.Start >= interval.End)
				end -= DateTimeHelper.WeekSpan;
			if (end < start)
				end = start;
			return new TimeInterval(start, end);
		}
		protected internal override TimeIntervalCollection CreateTimeIntervalCollection() {
			return new DayIntervalCollection();
		}
		protected internal override TimeInterval CreateValidSelectionInterval(SchedulerViewSelection selection) {
			TimeInterval result = null;
			int count = InnerVisibleIntervals.Count;
			for (int i = 0; i < count; i++) {
				TimeInterval interval = InnerVisibleIntervals[i];
				TimeInterval dayInterval = new TimeInterval(interval.Start.Date + ActualVisibleTime.Start, interval.Start.Date + ActualVisibleTime.End);
				TimeInterval intersectionInterval = TimeInterval.Intersect(dayInterval, selection.Interval);
				if (intersectionInterval.Duration.Ticks > 0)
					result = TimeInterval.Union(result, intersectionInterval);
			}
			if (result == null)
				result = TimeInterval.Empty;
			return result;
		}
		protected internal override void CreateVisibleIntervalsCore(DateTime date) {
			InnerVisibleIntervals.BeginUpdate();
			try {
				InnerVisibleIntervals.Clear();
				InnerVisibleIntervals.AddRange(CreateValidIntervalsCore(date));
			} finally {
				InnerVisibleIntervals.EndUpdate();
			}
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DayIntervalCollection days) {
			return CreateValidIntervalsCore(days.Start);
		}
		protected internal virtual DayIntervalCollection CreateValidIntervalsCore(DateTime date) {
			WorkDaysCollection workDays = Owner.WorkDays;
			DayIntervalCollection result = new DayIntervalCollection();
			DateTime startOfWeek = DateTimeHelper.GetStartOfWeekUI(date, Owner.FirstDayOfWeek);
			if (DateTime.MaxValue - startOfWeek < DateTimeHelper.WeekSpan)
				startOfWeek -= DateTimeHelper.WeekSpan;
			if (IsFullWeek) {
				result.Add(new TimeInterval(startOfWeek, DateTimeHelper.WeekSpan));
				return result;
			}
			DateTime currentDate = startOfWeek;
			for (int i = 0; i < 7; i++) {
				if (workDays.IsWeekDaysWorkDay(currentDate))  
					result.Add(new TimeInterval(currentDate, DateTimeHelper.DaySpan));
				currentDate = currentDate.AddDays(1);
			}
			if (result.Count <= 0)
				result.Add(new TimeInterval(startOfWeek, DateTimeHelper.WeekSpan));
			return result;
		}
		protected internal override ChangeActions SetVisibleIntervalsCore(TimeIntervalCollection intervals) {
			InnerVisibleIntervals.Clear();
			CreateVisibleIntervalsCore(intervals.Start);
			SetDayCountCore(InnerVisibleIntervals.Count);
			return ChangeActions.None;
		}
		protected internal override DateTime CalculateNewStartDateBackward() {
			return InnerVisibleIntervals.Start.AddDays(-7);
		}
		protected internal override DateTime CalculateNewStartDateForward() {
			return InnerVisibleIntervals.Start.AddDays(7);
		}		
	}
	#region InnerWorkWeekView
	public class InnerWorkWeekView : InnerWeekViewBase {
		internal const bool DefaultShowFullWeek = false;
		bool showFullWeek = DefaultShowFullWeek;
		public InnerWorkWeekView(IInnerSchedulerViewOwner owner, IWorkWeekViewProperties properties)
			: base(owner, properties) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.WorkWeek; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override Keys Shortcut { get { return Keys.Control | Keys.Alt | Keys.D2; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToWorkWeekView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewDisplayName_WorkDays); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultShortDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewShortDisplayName_WorkDays); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_SwitchToWorkWeekView); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string Description { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WorkWeekViewDescription); } }
		[DefaultValue(DefaultShowFullWeek), XtraSerializableProperty()]
		public bool ShowFullWeek {
			get { return showFullWeek; }
			set {
				if (showFullWeek == value)
					return;
				showFullWeek = value;
				RaiseChanged(SchedulerControlChangeType.ShowFullWeekChanged);
			}
		}
		protected override bool IsFullWeek {
			get { return showFullWeek; }
		}
		#endregion
		public override void Initialize(SchedulerViewSelection selection) {
			base.Initialize(selection);
			this.ShowFullWeek = DefaultShowFullWeek;
		}
		public override void Reset() {
			base.Reset();
			this.ShowFullWeek = DefaultShowFullWeek;
		}
	}
	#endregion
}
