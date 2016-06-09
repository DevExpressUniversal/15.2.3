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
using System.Windows;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Scheduler;
using DevExpress.Scheduler.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Utils.Serializing;
#if SL
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region TimelineView
	public class TimelineView : SchedulerViewBase, IWpfTimelineViewProperties {
		const int defaultIntervalCount = 10;
		TimeScaleCollection scales;
		static TimelineView() {
		}
		public TimelineView() {
			this.scales = new TimeScaleCollection();
		}
		#region Properties
		#region TimelineScrollBarVisible
		[Obsolete("Use the TimelineScrollBarVisibility property instead.")]
		public bool TimelineScrollBarVisible {
			get { return (bool)GetValue(TimelineScrollBarVisibleProperty); }
			set { SetValue(TimelineScrollBarVisibleProperty, value); }
		}
		public static readonly DependencyProperty TimelineScrollBarVisibleProperty = CreateTimelineScrollBarVisibleProperty();
		static DependencyProperty CreateTimelineScrollBarVisibleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, bool>("TimelineScrollBarVisible", false, (d, e) => d.OnTimelineScrollBarVisibleChanged(e.OldValue, e.NewValue));
		}
		void OnTimelineScrollBarVisibleChanged(bool oldValue, bool newValue) {
			SetCurrentValue(TimelineScrollBarVisibilityProperty, (newValue) ? SchedulerScrollBarVisibility.Always : SchedulerScrollBarVisibility.Never);
		}
		#endregion
		#region TimelineScrollBarVisibility
		public SchedulerScrollBarVisibility TimelineScrollBarVisibility {
			get { return (SchedulerScrollBarVisibility)GetValue(TimelineScrollBarVisibilityProperty); }
			set { SetValue(TimelineScrollBarVisibilityProperty, value); }
		}
		public static readonly DependencyProperty TimelineScrollBarVisibilityProperty = CreateTimelineScrollBarVisibilityProperty();
		static DependencyProperty CreateTimelineScrollBarVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, SchedulerScrollBarVisibility>("TimelineScrollBarVisibility", SchedulerScrollBarVisibility.Never, (d, e) => d.OnTimelineScrollBarVisibilityChanged(e.OldValue, e.NewValue));
		}
		void OnTimelineScrollBarVisibilityChanged(SchedulerScrollBarVisibility oldValue, SchedulerScrollBarVisibility newValue) {
			base.OnContainerScrollBarVisibleChanged();
		}
		#endregion
		protected internal TimeScaleCollection InnerScales { get { return InnerView != null ? ((InnerTimelineView)InnerView).Scales : new TimeScaleCollection(); } }
		#region Type
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Timeline; } }
		#endregion
		#region AppointmetDisplayOptions
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimelineViewAppointmentDisplayOptions")]
#endif
		public SchedulerTimelineViewAppointmentDisplayOptions AppointmentDisplayOptions {
			get { return (SchedulerTimelineViewAppointmentDisplayOptions)GetValue(AppointmentDisplayOptionsProperty); }
			set { SetValue(AppointmentDisplayOptionsProperty, value); }
		}
		public static readonly DependencyProperty AppointmentDisplayOptionsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, SchedulerTimelineViewAppointmentDisplayOptions>("AppointmentDisplayOptions", null, (d, e) => d.UpdateInnerObjectPropertyValue(AppointmentDisplayOptionsProperty, e.OldValue, e.NewValue), null);
		#endregion
		#region SelectionBar
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimelineViewSelectionBar")]
#endif
		public SchedulerSelectionBarOptions SelectionBar {
			get { return (SchedulerSelectionBarOptions)GetValue(SelectionBarProperty); }
			set { SetValue(SelectionBarProperty, value); }
		}
		public static readonly DependencyProperty SelectionBarProperty = CreateSelectionBarProperty();
		static DependencyProperty CreateSelectionBarProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, SchedulerSelectionBarOptions>("SelectionBar", null, (d, e) => d.UpdateInnerObjectPropertyValue(SelectionBarProperty, e.OldValue, e.NewValue));
		}
		#endregion
		#region CellStyle
		public static readonly DependencyProperty CellStyleProperty = CreateCellStyleProperty();
		static DependencyProperty CreateCellStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, Style>("CellStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimelineViewCellStyle")]
#endif
		public Style CellStyle {
			get { return (Style)GetValue(CellStyleProperty); }
			set { SetValue(CellStyleProperty, value); }
		}
		#endregion
		#region SelectionBarCellStyle
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimelineViewSelectionBarCellStyle")]
#endif
		public Style SelectionBarCellStyle {
			get { return (Style)GetValue(SelectionBarCellStyleProperty); }
			set { SetValue(SelectionBarCellStyleProperty, value); }
		}
		internal const string SelectionBarCellStylePropertyName = "SelectionBarCellStyle";
		public static readonly DependencyProperty SelectionBarCellStyleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, Style>(SelectionBarCellStylePropertyName, null);
		#endregion
		#region DateHeaderStyle
		internal const string DateHeaderStylePropertyName = "DateHeaderStyle";
		public static readonly DependencyProperty DateHeaderStyleProperty = CreateDateHeaderStyleProperty();
		static DependencyProperty CreateDateHeaderStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, Style>(DateHeaderStylePropertyName, null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimelineViewDateHeaderStyle")]
#endif
		public Style DateHeaderStyle {
			get { return (Style)GetValue(DateHeaderStyleProperty); }
			set { SetValue(DateHeaderStyleProperty, value); }
		}
		#endregion
		#region Scales
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimelineViewScales")]
#endif
		public TimeScaleCollection Scales { get { return scales; } set { scales = value; } }
		#endregion
		#region IntervalCount
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimelineViewIntervalCount")]
#endif
		public int IntervalCount {
			get { return (int)GetValue(IntervalCountProperty); }
			set { SetValue(IntervalCountProperty, value); }
		}
		public static readonly DependencyProperty IntervalCountProperty = CreateIntervalCountProperty();
		static DependencyProperty CreateIntervalCountProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, int>("IntervalCount", defaultIntervalCount, (d, e) => d.OnIntervalCountPropertyChanged(e.OldValue, e.NewValue));
		}
		private void OnIntervalCountPropertyChanged(int oldValue, int newValue) {
			ApplyIntervalCountToInnerView();
		}
		#endregion
		#region WorkTime
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("TimelineViewWorkTime"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public TimeOfDayInterval WorkTime {
			get { return (TimeOfDayInterval)GetValue(WorkTimeProperty); }
			set { SetValue(WorkTimeProperty, value); }
		}
		public static readonly DependencyProperty WorkTimeProperty = CreateWorkTimeProperty();
		static DependencyProperty CreateWorkTimeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, TimeOfDayInterval>("WorkTime", WorkTimeInterval.WorkTime, (d, e) => d.OnWorkTimeChanged(e.OldValue, e.NewValue));
		}
		void OnWorkTimeChanged(TimeOfDayInterval oldValue, TimeOfDayInterval newValue) {
			UpdateInnerObjectPropertyValue(WorkTimeProperty, oldValue, newValue);
		}
		#endregion
		#region MoreButtonTemplate
		public static readonly DependencyProperty MoreButtonStyleProperty = CreateMoreButtonStyleProperty();
		static DependencyProperty CreateMoreButtonStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, Style>("MoreButtonStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("TimelineViewMoreButtonStyle")]
#endif
		public Style MoreButtonStyle {
			get { return (Style)GetValue(MoreButtonStyleProperty); }
			set { SetValue(MoreButtonStyleProperty, value); }
		}
		#endregion
		#region DeferredScrolling
		ISchedulerDeferredScrollingOption ISchedulerViewPropertiesBase.DeferredScrolling { get { return DeferredScrolling; } set { DeferredScrolling = (SchedulerDeferredScrollingOption)value; } }
		public SchedulerDeferredScrollingOption DeferredScrolling {
			get { return (SchedulerDeferredScrollingOption)GetValue(DeferredScrollingProperty); }
			set { SetValue(DeferredScrollingProperty, value); }
		}
		public static readonly DependencyProperty DeferredScrollingProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, SchedulerDeferredScrollingOption>("DeferredScrolling", null, (d, e) => d.OnDeferredScrollingChanged(e.OldValue, e.NewValue), null);
		void OnDeferredScrollingChanged(SchedulerDeferredScrollingOption oldValue, SchedulerDeferredScrollingOption newValue) {
		}
		#endregion
		#region DateTimeScrollbarVisible
		public bool DateTimeScrollbarVisible {
			get { return (bool)GetValue(DateTimeScrollbarVisibleProperty); }
			set { SetValue(DateTimeScrollbarVisibleProperty, value); }
		}
		public static readonly DependencyProperty DateTimeScrollbarVisibleProperty = CreateDateTimeScrollbarVisibleProperty();
		static DependencyProperty CreateDateTimeScrollbarVisibleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, bool>("DateTimeScrollbarVisible", true);
		}
		#endregion
		#region TimeIndicatorDisplayOptions
		public SchedulerTimeIndicatorDisplayOptions TimeIndicatorDisplayOptions {
			get { return (SchedulerTimeIndicatorDisplayOptions)GetValue(TimeIndicatorDisplayOptionsProperty); }
			set { SetValue(TimeIndicatorDisplayOptionsProperty, value); }
		}
		public static readonly DependencyProperty TimeIndicatorDisplayOptionsProperty = CreateTimeIndicatorDisplayOptionsProperty();
		static DependencyProperty CreateTimeIndicatorDisplayOptionsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, SchedulerTimeIndicatorDisplayOptions>("TimeIndicatorDisplayOptions", null);
		}
		#endregion
		#region OptionsSelectionBehavior
		public OptionsSelectionBehavior OptionsSelectionBehavior {
			get { return (OptionsSelectionBehavior)GetValue(OptionsSelectionBehaviorProperty); }
			set { SetValue(OptionsSelectionBehaviorProperty, value); }
		}
		public static readonly DependencyProperty OptionsSelectionBehaviorProperty = CreateOptionsSelectionBehaviorProperty();
		static DependencyProperty CreateOptionsSelectionBehaviorProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<TimelineView, OptionsSelectionBehavior>("OptionsSelectionBehavior", null);
		}
		#endregion
		#endregion
		protected override void InitializePropertiesWithDefaultValues() {
			if (OptionsSelectionBehavior == null)
				OptionsSelectionBehavior = new OptionsSelectionBehavior();
			if (AppointmentDisplayOptions == null)
				AppointmentDisplayOptions = new SchedulerTimelineViewAppointmentDisplayOptions();
			if (SelectionBar== null)
				SelectionBar = new SchedulerSelectionBarOptions();
			if (DeferredScrolling == null)
				DeferredScrolling = new SchedulerDeferredScrollingOption();
			if (TimeIndicatorDisplayOptions == null)
				TimeIndicatorDisplayOptions = new SchedulerTimeIndicatorDisplayOptions();
		}
		protected override DependencyPropertySyncManager CreatePropertySyncManager() {
			return new TimelineViewPropertySyncManager(this);
		}
		public TimeScale GetBaseTimeScale() {
			if (InnerView != null) {
				InnerTimelineView innerView = (InnerTimelineView)InnerView;
				return innerView.GetBaseTimeScale();
			}
			return null;
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new TimelineViewAppointmentDisplayOptions();
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new WpfInnerTimelineView(this,  this);
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new TimelineViewFactoryHelper();
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new TimelineViewDateTimeScrollController(this);
		}
		protected internal override void SetupInnerViewBindings() {
			base.SetupInnerViewBindings();
			SynchronizeTimeScales();
		}
		protected internal override void SynchronizeToInnerView(InnerSchedulerViewBase view) {
			base.SynchronizeToInnerView(view);
		}
		private void SynchronizeSelectionBarOptions() {
			OnOptionsAssigned(null, SelectionBar, ((InnerTimelineView)InnerView).SelectionBar);
		}
		protected internal virtual void SynchronizeTimeScales() {
			if (InnerView == null)
				return;
			if (Scales.Count == 0) {
				Scales.LoadDefaults();
			}
			((InnerTimelineView)InnerView).InnerScales = Scales;
		}
		protected internal virtual void ApplyIntervalCountToInnerView() {
			if (InnerView != null && IntervalCount != InnerVisibleIntervals.Count)
				InnerView.RaiseChanged(SchedulerControlChangeType.TimelineIntervalCountChanged);
		}
		protected override bool ValidateAppointmentDisplayOptions(SchedulerAppointmentDisplayOptions appointmentDisplayOptions) {
			return appointmentDisplayOptions is SchedulerTimelineViewAppointmentDisplayOptions;
		}
		protected override SchedulerAppointmentDisplayOptions GetAppointmentDisplayOptions() {
			return this.AppointmentDisplayOptions;
		}
		protected override void MakeAppointmentVisibleInScrollContainers(Appointment apt) {
			base.MakeAppointmentVisibleInScrollContainers(apt);
		}
	}
	#endregion
}
namespace DevExpress.Scheduler.Native {
	public class WpfInnerTimelineView : InnerTimelineView {
		public WpfInnerTimelineView(IInnerSchedulerViewOwner owner, ITimelineViewProperties properties)
			: base(owner, properties) {
		}
		protected new IWpfTimelineViewProperties Properties { get { return (IWpfTimelineViewProperties)base.Properties; } }
		int IntervalCount { get { return Properties.IntervalCount; } }
		protected internal override void PopulateVisibleIntervalsCore(DateTime date) {
			TimelineView view = (TimelineView)Owner;
			TimeScaleIntervalCollection collection = (TimeScaleIntervalCollection)InnerVisibleIntervals;
			TimeScale scale = collection.Scale;
			DateTime currentDate = scale.Floor(date);
			int count = view.IntervalCount;
			for (int i = 0; i < count; i++) {
				InnerVisibleIntervals.Add(new TimeInterval(currentDate, TimeSpan.Zero));
				currentDate = scale.GetNextDate(currentDate);
			}
		}
		protected internal override ChangeActions SetVisibleDays(DayIntervalCollection days, SchedulerViewSelection selection) {
			ChangeActions actions = base.SetVisibleDays(days, selection);
			Properties.IntervalCount = GetVisibleIntervals().Count;
			return actions;
		}
		protected internal override ChangeActions SetVisibleInterval(DateTime start, DateTime end, SchedulerViewSelection selection) {
			ChangeActions actions = base.SetVisibleInterval(start, end, selection);
			Properties.IntervalCount = GetVisibleIntervals().Count;
			return actions;
		}
		protected internal override TimeIntervalCollection CreateValidIntervals(DayIntervalCollection days) {
			if (days.Count < 1)
				return days;
			TimeScale baseScale = GetBaseTimeScale();
			DateTime start = days[0].Start;
			TimeIntervalCollection intervals = new TimeIntervalCollection();
			for (int i = 0; i < IntervalCount; i++) {
				DateTime end = baseScale.GetNextDate(start);
				intervals.Add(new TimeInterval(start, end));
				start = end;
			}
			DayIntervalCollection result = new DayIntervalCollection();
			result.AddRange(intervals);
			return result;
		}
	}
	public interface IWpfTimelineViewProperties : ITimelineViewProperties {
		int IntervalCount { get; set; }
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region TimelineViewContentStyleSelector
	public class TimelineViewContentStyleSelector : ViewContentStyleSelector {
		public TimelineViewContentStyleSelector() {
		}
		protected internal override Type GroupByNoneType { get { return typeof(VisualTimelineViewGroupByNone); } }
		protected internal override Type GroupByDateType { get { return typeof(VisualTimelineViewGroupByDate); } }
		protected internal override Type GroupByResourceType { get { return typeof(VisualTimelineViewGroupByDate); } }
	}
	#endregion
}
