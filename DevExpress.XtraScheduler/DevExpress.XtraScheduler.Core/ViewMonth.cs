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
	public interface IMonthViewProperties : IWeekViewProperties {
	}
	#region InnerMonthView
	public class InnerMonthView : InnerWeekView {
		#region Fields
		internal const int defaultWeekCount = 5;
		internal const bool defaultCompressWeekend = false;
		internal const bool defaultShowWeekend = true;
		int weekCount = defaultWeekCount;
		bool compressWeekend = defaultCompressWeekend;
		bool showWeekend = defaultShowWeekend;
		#endregion
		public InnerMonthView(IInnerSchedulerViewOwner owner, IMonthViewProperties properties)
			: base(owner, properties) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Month; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override Keys Shortcut { get { return Keys.Control | Keys.Alt | Keys.D4; } }
		[DefaultValue(defaultWeekCount), XtraSerializableProperty()]
		public int WeekCount {
			get { return weekCount; }
			set {
				value = Math.Max(1, value);
				int oldValue = weekCount;
				if (oldValue == value)
					return;
				weekCount = value;
				UpdateVisibleIntervals();
				RaiseChanged(SchedulerControlChangeType.WeekCountChanged);
				RaiseUIChanged("WeekCount", oldValue, value);
			}
		}
		[DefaultValue(defaultCompressWeekend), XtraSerializableProperty()]
		public bool CompressWeekend {
			get { return compressWeekend; }
			set {
				if (compressWeekend == value)
					return;
				compressWeekend = value;
				UpdateVisibleIntervals();
				RaisePropertyChanged("CompressWeekend");
				RaiseChanged(SchedulerControlChangeType.CompressWeekendChanged);
			}
		}
		void UpdateVisibleIntervals() {
			TimeIntervalCollection timeIntervals = GetVisibleIntervals();
			CreateVisibleIntervalsCore(timeIntervals.Start);
		}
		[DefaultValue(defaultShowWeekend), XtraSerializableProperty()]
		public bool ShowWeekend {
			get { return showWeekend; }
			set {
				if (showWeekend == value)
					return;
				showWeekend = value;
				RaiseChanged(SchedulerControlChangeType.ShowWeekendChanged);
			}
		}
		protected internal override int WeekCountCore { get { return WeekCount; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override bool CompressWeekendInternal { get { return compressWeekend; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override bool ShowWeekendInternal { get { return showWeekend; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToMonthView; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewDisplayName_Month); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultShortDisplayName { get { return SchedulerLocalizer.GetString(SchedulerStringId.ViewShortDisplayName_Month); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string DefaultMenuCaption { get { return SchedulerLocalizer.GetString(SchedulerStringId.MenuCmd_SwitchToMonthView); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override string Description { get { return SchedulerLocalizer.GetString(SchedulerStringId.Caption_MonthViewDescription); } }
		#endregion
		protected internal override ChangeActions ValidateSelectionInterval(SchedulerViewSelection selection) {
			ChangeActions result = base.ValidateSelectionInterval(selection);
			if (!ShowWeekend) {
				TimeInterval selectionInterval = selection.Interval;
				DayOfWeek startDayOfWeek = selectionInterval.Start.DayOfWeek;
				DayOfWeek endDayOfWeek = selectionInterval.End.AddTicks(-1).DayOfWeek;
				if (startDayOfWeek == DayOfWeek.Sunday || startDayOfWeek == DayOfWeek.Saturday ||
					endDayOfWeek == DayOfWeek.Sunday || endDayOfWeek == DayOfWeek.Saturday) {
					WeekIntervalCollection coll = (WeekIntervalCollection)InnerVisibleIntervals;
					DateTime start = coll.RoundToStart(selectionInterval);
					if (start.DayOfWeek == DayOfWeek.Sunday)
						start = start.AddDays(1);
					else if (start.DayOfWeek == DayOfWeek.Saturday)
						start = start.AddDays(2);
					TimeInterval newSelectionInterval = new TimeInterval(start, DateTimeHelper.DaySpan);
					result |= newSelectionInterval.Equals(selectionInterval) ? ChangeActions.None : ChangeActions.RaiseSelectionChanged;
					selection.Interval = newSelectionInterval;
					selection.FirstSelectedInterval = new TimeInterval(newSelectionInterval.Start, DateTimeHelper.DaySpan);
				}
			}
			return result;
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DayIntervalCollection days) {
			TimeIntervalCollection result = base.CreateValidIntervals(days);
			this.weekCount = result.Count;
			return result;
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DateTime visibleStart, DateTime visibleEnd) {
			TimeIntervalCollection result = base.CreateValidIntervals(visibleStart, visibleEnd);
			this.weekCount = result.Count;
			return result;
		}
		public override void Reset() {
			base.Reset();
			WeekCount = defaultWeekCount;
			CompressWeekend = defaultCompressWeekend;
			ShowWeekend = defaultShowWeekend;
		}
		protected internal override ChangeActions SetVisibleIntervalsCore(TimeIntervalCollection intervals) {
			int oldWeekCount = this.InnerVisibleIntervals.Count;
			BaseSetVisibleIntervalsCore(intervals);
			this.weekCount = this.InnerVisibleIntervals.Count;
			if (oldWeekCount != this.weekCount)
				return ChangeActionsCalculator.CalculateChangeActions(SchedulerControlChangeType.WeekCountChanged);
			return ChangeActions.None;
		}
		public override void ZoomIn() {
			WeekCount--;
		}
		protected internal override bool CanZoomIn() {
			return WeekCount > 1;
		}
		public override void ZoomOut() {
			WeekCount++;
		}
		protected internal override bool CanZoomOut() {
			return true;
		}
		public DayOfWeek ActualFirstDayOfWeek {
			get {
				if (CompressWeekend && FirstDayOfWeek == DayOfWeek.Sunday)
					return DayOfWeek.Monday;
				return FirstDayOfWeek;
			}
		}
	}
	#endregion
}
