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
using DevExpress.XtraScheduler.Localization;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data;
#endif
namespace DevExpress.XtraScheduler.Native {
	public interface IWeekViewProperties : ISchedulerViewPropertiesBase {
	}   
	#region InnerWeekView
	public class InnerWeekView : InnerSchedulerViewBase {
		public InnerWeekView(IInnerSchedulerViewOwner owner, IWeekViewProperties properties)
			: base(owner, properties) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Week; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override Keys Shortcut { get { return Keys.Control | Keys.Alt | Keys.D3; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual bool CompressWeekendInternal { get { return true; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual bool ShowWeekendInternal { get { return true; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal virtual DayOfWeek FirstDayOfWeek { get { return Owner.FirstDayOfWeek; } }
		protected internal virtual int WeekCountCore { get { return 1; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToWeekView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewDisplayName_Week); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultShortDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewShortDisplayName_Week); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_SwitchToWeekView); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string Description { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_WeekViewDescription); } }
		protected internal virtual bool DrawMoreButtonsOverAppointments { get { return true; } }
		#endregion
		protected internal override TimeIntervalCollection CreateTimeIntervalCollection() {
			return new WeekIntervalCollection();
		}
		protected internal override void CreateVisibleIntervalsCore(DateTime date) {
			WeekIntervalCollection coll = (WeekIntervalCollection)InnerVisibleIntervals;
			coll.FirstDayOfWeek = FirstDayOfWeek;
			coll.CompressWeekend = CompressWeekendInternal;
			coll.Clear();
			DateTime start = coll.RoundToStart(new TimeInterval(date, TimeSpan.Zero));
			DateTime end = start.AddDays(7 * WeekCountCore);
			coll.Add(new TimeInterval(start, end));
		}
		protected internal override TimeInterval RoundLimitInterval(TimeInterval interval) {
			WeekIntervalCollection coll = (WeekIntervalCollection)InnerVisibleIntervals;
			DateTime start = coll.RoundToStart(interval);
			DateTime end = coll.RoundToEnd(interval);
			return new TimeInterval(start, end);
		}
		protected internal override TimeInterval RoundSelectionInterval(TimeInterval interval) {
			DateTime start = DateTimeHelper.Floor(interval.Start, DateTimeHelper.DaySpan);
			DateTime end = DateTimeHelper.Ceil(interval.End, DateTimeHelper.DaySpan);
			return new TimeInterval(start, end);
		}
		protected internal override TimeInterval CreateDefaultSelectionInterval(DateTime date) {
			return new TimeInterval(date, DateTimeHelper.DaySpan);
		}
		protected internal override ChangeActions SynchronizeSelectionInterval(SchedulerViewSelection selection, bool activeViewChanged) {
			DateTime start = DateTimeHelper.Floor(selection.Interval.Start, DateTimeHelper.DaySpan);
			DateTime end = DateTimeHelper.Ceil(selection.Interval.End, DateTimeHelper.DaySpan);
			if (start == end)
				end = DateTimeHelper.Ceil(start.AddDays(1), DateTimeHelper.DaySpan);
			TimeInterval newSelectionInterval = new TimeInterval(start, end);
			ChangeActions result = newSelectionInterval.Equals(selection.Interval) ? ChangeActions.None : ChangeActions.RaiseSelectionChanged;
			selection.Interval = newSelectionInterval;
			result |= ValidateSelectionInterval(selection);
			return result;
		}
		protected internal override ChangeActions ValidateSelectionInterval(SchedulerViewSelection selection) {
			TimeInterval validSelection = TimeInterval.Intersect(InnerVisibleIntervals.Interval, selection.Interval);
			if (validSelection.Duration == TimeSpan.Zero)
				return InitializeSelection(selection);
			else {
				ChangeActions result = validSelection.Equals(selection.Interval) ? ChangeActions.None : ChangeActions.RaiseSelectionChanged;
				selection.Interval = validSelection;
				selection.FirstSelectedInterval = new TimeInterval(validSelection.Start, DateTimeHelper.DaySpan);
				return result;
			}
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DayIntervalCollection days) {
			WeekIntervalCollection result = new WeekIntervalCollection();
			result.FirstDayOfWeek = FirstDayOfWeek;
			result.CompressWeekend = CompressWeekendInternal;
			result.AddRange(days);
			return result;
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DateTime visibleStart, DateTime visibleEnd) {
			WeekIntervalCollection result = new WeekIntervalCollection();
			result.FirstDayOfWeek = FirstDayOfWeek;
			result.CompressWeekend = CompressWeekendInternal;
			DayIntervalCollection days = new DayIntervalCollection();
			days.Add(new TimeInterval(visibleStart, visibleEnd));
			result.AddRange(days);
			return result;
		}
		protected void BaseSetVisibleIntervalsCore(TimeIntervalCollection intervals) {
			base.SetVisibleIntervalsCore(intervals);
		}
		protected internal override ChangeActions SetVisibleIntervalsCore(TimeIntervalCollection intervals) {
			CreateVisibleIntervalsCore(intervals.Start);
			return ChangeActions.None;
		}
		protected internal override TimeOfDayIntervalCollection CalcResourceWorkTimeInterval(TimeInterval interval, Resource resource) {
			return new TimeOfDayIntervalCollection();
		}
		public override void ZoomIn() {
		}
		protected internal override bool CanZoomIn() {
			return false;
		}
		public override void ZoomOut() {
		}
		protected internal override bool CanZoomOut() {
			return false;
		}
	}
	#endregion
}
