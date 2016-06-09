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
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.ThemeKeys;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Data.Utils;
#if SL
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region DayView
	public class DayView : SchedulerViewBase, IInnerDayViewOwner, IDayViewProperties {
		internal const double DefaultDraggedAppointmentHeight = 100;
		WeakEventHandler<DayView, EventArgs, EventHandler> workTimeChangedHandler;
		static DayView() {
		}
		public DayView() {
			DefaultTimeRulers = new SchedulerTimeRulerCollection();
			DefaultTimeRulers.Add(new SchedulerTimeRuler());
			CreateTimeRulers();
			DefaultTimeSlots = new TimeSlotCollection();
			CreateTimeSlots();			
		}
		#region Properties
		#region AppointmentPadding
		public static readonly DependencyProperty AppointmentPaddingProperty = DependencyProperty.Register("AppointmentPadding", typeof(Thickness), typeof(DayView), new UIPropertyMetadata(new Thickness(1, 1, 10, 1), new PropertyChangedCallback(OnAppointmentPaddingChanged)));
		public Thickness AppointmentPadding {
			get { return (Thickness)GetValue(AppointmentPaddingProperty); }
			set { SetValue(AppointmentPaddingProperty, value); }
		}
		static void OnAppointmentPaddingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			DayView dayView = o as DayView;
			if (dayView != null)
				dayView.OnAppointmentPaddingChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
		}
		protected virtual void OnAppointmentPaddingChanged(Thickness oldValue, Thickness newValue) {
		}
		#endregion
		#region AllDayAreaScrollBarVisible
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewAllDayAreaScrollBarVisible")]
#endif
		public bool AllDayAreaScrollBarVisible {
			get { return (bool)GetValue(AllDayAreaScrollBarVisibleProperty); }
			set { SetValue(AllDayAreaScrollBarVisibleProperty, value); }
		}
		public static readonly DependencyProperty AllDayAreaScrollBarVisibleProperty = CreateAllDayAreaScrollBarVisibleProperty();
		static DependencyProperty CreateAllDayAreaScrollBarVisibleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, bool>("AllDayAreaScrollBarVisible", false, (d, e) => d.OnAllDayAreaScrollBarVisibleChanged(e.OldValue, e.NewValue));
		}
		protected virtual void OnAllDayAreaScrollBarVisibleChanged(bool oldValue, bool newValue) {
			base.OnContainerScrollBarVisibleChanged();
		}
		#endregion
		#region TimeSlotCollection
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewTimeSlots"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TimeSlotCollection TimeSlots {
			get { return (TimeSlotCollection)GetValue(TimeSlotsProperty); }
		}
		internal static readonly DependencyPropertyKey TimeSlotsPropertyKey = DependencyPropertyManager.RegisterReadOnly("TimeSlots",
			typeof(TimeSlotCollection), typeof(DayView), new PropertyMetadata(null));
		public static readonly DependencyProperty TimeSlotsProperty = TimeSlotsPropertyKey.DependencyProperty;
		#endregion
		#region DefaultTimeSlots
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSlotCollection DefaultTimeSlots {
			get { return (TimeSlotCollection)GetValue(DefaultTimeSlotsProperty); }
			set { SetValue(DefaultTimeSlotsProperty, value); }
		}
		public static readonly DependencyProperty DefaultTimeSlotsProperty = CreateDefaultTimeSlotsProperty();
		static DependencyProperty CreateDefaultTimeSlotsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, TimeSlotCollection>("DefaultTimeSlots", null);
		}
		#endregion
		#region SchedulerTimeRulerCollection
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewTimeRulers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerTimeRulerCollection TimeRulers {
			get { return (SchedulerTimeRulerCollection)GetValue(TimeRulersProperty); }
		}
		internal static readonly DependencyPropertyKey TimeRulersPropertyKey = DependencyPropertyManager.RegisterReadOnly("TimeRulers",
			typeof(SchedulerTimeRulerCollection), typeof(DayView), new PropertyMetadata(null));
		public static readonly DependencyProperty TimeRulersProperty = TimeRulersPropertyKey.DependencyProperty;
		#endregion
		#region DefaultTimeRulers
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerTimeRulerCollection DefaultTimeRulers {
			get { return (SchedulerTimeRulerCollection)GetValue(DefaultTimeRulersProperty); }
			set { SetValue(DefaultTimeRulersProperty, value); }
		}
		public static readonly DependencyProperty DefaultTimeRulersProperty = CreateDefaultTimeRulersProperty();
		static DependencyProperty CreateDefaultTimeRulersProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, SchedulerTimeRulerCollection>("DefaultTimeRulers", null);
		}
		#endregion
		#region DateHeaderStyle
		public static readonly DependencyProperty DateHeaderStyleProperty = CreateDateHeaderStyleProperty();
		static DependencyProperty CreateDateHeaderStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, Style>("DateHeaderStyle", null, (s, e) => s.OnDateHeaderStyleChanged(e.OldValue, e.NewValue));
		}
		void OnDateHeaderStyleChanged(System.Windows.Style oldValue, System.Windows.Style newValue) {
			if (newValue != null) {
				SealHelper.SealIfSealable(newValue);
			}
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewDateHeaderStyle")]
#endif
		public Style DateHeaderStyle {
			get { return (Style)GetValue(DateHeaderStyleProperty); }
			set { SetValue(DateHeaderStyleProperty, value); }
		}
		#endregion
		protected internal new InnerDayView InnerView { get { return (InnerDayView)base.InnerView; } }
		#region Type
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Day; } }
		#endregion
		#region ActualVisibleTime
		protected internal TimeOfDayInterval ActualVisibleTime {
			get { return InnerView.ShowWorkTimeOnly ? (TimeOfDayInterval)InnerView.WorkTime.Clone() : InnerView.VisibleTime; }
		}
		#endregion
		internal override double DraggedAppointmentHeightInternal { get { return DraggedAppointmentHeight; } }
		#region TopRowTime
		public TimeSpan TopRowTime {
			get { return (TimeSpan)GetValue(TopRowTimeProperty); }
			set { SetValue(TopRowTimeProperty, value); }
		}
		public static readonly DependencyProperty TopRowTimeProperty = CreateTopRowTimeProperty();
		static DependencyProperty CreateTopRowTimeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, TimeSpan>("TopRowTime", TimeSpan.MinValue, (d, e) => d.OnTopRowTimeChanged(e.OldValue, e.NewValue), null);
		}
		void OnTopRowTimeChanged(TimeSpan oldValue, TimeSpan newValue) {
			if (Control != null)
				Control.UpdateDateTimeScrollBarValue();
		}
		#endregion
		#region CellStyle
		public static readonly DependencyProperty CellStyleProperty = CreateCellStyleProperty();
		static DependencyProperty CreateCellStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, Style>("CellStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewCellStyle")]
#endif
		public Style CellStyle {
			get { return (Style)GetValue(CellStyleProperty); }
			set { SetValue(CellStyleProperty, value); }
		}
		#endregion
		#region ShowMoreButtonsOnEachColumn
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewShowMoreButtonsOnEachColumn"),
#endif
		DefaultValue(InnerDayView.DefaultShowMoreButtonsOnEachColumn), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowMoreButtonsOnEachColumn {
			get { return (bool)GetValue(ShowMoreButtonsOnEachColumnProperty); }
			set { SetValue(ShowMoreButtonsOnEachColumnProperty, value); }
		}
		public static readonly DependencyProperty ShowMoreButtonsOnEachColumnProperty = CreateShowMoreButtonsOnEachColumnProperty();
		static DependencyProperty CreateShowMoreButtonsOnEachColumnProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, bool>("ShowMoreButtonsOnEachColumn", InnerDayView.DefaultShowMoreButtonsOnEachColumn, (d, e) => d.OnShowMoreButtonsOnEachColumnChanged(e.OldValue, e.NewValue));
		}
		void OnShowMoreButtonsOnEachColumnChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowMoreButtonsOnEachColumnProperty, oldValue, newValue);
		}
		#endregion
		#region AppointmetDisplayOptions
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewAppointmentDisplayOptions")]
#endif
		public SchedulerDayViewAppointmentDisplayOptions AppointmentDisplayOptions {
			get { return (SchedulerDayViewAppointmentDisplayOptions)GetValue(AppointmentDisplayOptionsProperty); }
			set { SetValue(AppointmentDisplayOptionsProperty, value); }
		}
		public static readonly DependencyProperty AppointmentDisplayOptionsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, SchedulerDayViewAppointmentDisplayOptions>("AppointmentDisplayOptions", null, (d, e) => d.UpdateInnerObjectPropertyValue(AppointmentDisplayOptionsProperty, e.OldValue, e.NewValue), null);
		#endregion
		#region VerticalResourceHeaderStyle
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style VerticalResourceHeaderStyle { get { return base.VerticalResourceHeaderStyle; } set { base.VerticalResourceHeaderStyle = value; } }
		#endregion
		#region ShowWorkTimeOnly
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewShowWorkTimeOnly"),
#endif
DefaultValue(InnerDayView.DefaultShowWorkTimeOnly), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowWorkTimeOnly {
			get { return (bool)GetValue(ShowWorkTimeOnlyProperty); }
			set { SetValue(ShowWorkTimeOnlyProperty, value); }
		}
		public static readonly DependencyProperty ShowWorkTimeOnlyProperty = CreateShowWorkTimeOnlyProperty();
		static DependencyProperty CreateShowWorkTimeOnlyProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, bool>("ShowWorkTimeOnly", InnerDayView.DefaultShowWorkTimeOnly, (d, e) => d.OnShowWorkTimeOnlyChanged(e.OldValue, e.NewValue));
		}
		void OnShowWorkTimeOnlyChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowWorkTimeOnlyProperty, oldValue, newValue);
		}
		#endregion
		#region DraggedAppointmentHeight
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewDraggedAppointmentHeight"),
#endif
		DefaultValue(DayView.DefaultDraggedAppointmentHeight), XtraSerializableProperty(), NotifyParentProperty(true)]
		public double DraggedAppointmentHeight {
			get { return (double)GetValue(DraggedAppointmentHeightProperty); }
			set { SetValue(DraggedAppointmentHeightProperty, value); }
		}
		public static readonly DependencyProperty DraggedAppointmentHeightProperty = CreateDraggedAppointmentHeightProperty();
		static DependencyProperty CreateDraggedAppointmentHeightProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, double>("DraggedAppointmentHeight", DayView.DefaultDraggedAppointmentHeight);
		}
		#endregion
		#region WorkTime
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewWorkTime"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content), NotifyParentProperty(true), AutoFormatEnable]
		public TimeOfDayInterval WorkTime {
			get { return (TimeOfDayInterval)GetValue(WorkTimeProperty); }
			set { SetValue(WorkTimeProperty, value); }
		}
		public static readonly DependencyProperty WorkTimeProperty = CreateWorkTimeProperty();
		static DependencyProperty CreateWorkTimeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, TimeOfDayInterval>("WorkTime", WorkTimeInterval.WorkTime, (d, e) => d.OnWorkTimeChanged(e.OldValue, e.NewValue));
		}
		void OnWorkTimeChanged(TimeOfDayInterval oldValue, TimeOfDayInterval newValue) {
			UpdateInnerObjectPropertyValue(WorkTimeProperty, oldValue, newValue);
			UpdateWorkTimeChangedHandlers(oldValue, newValue);
		}
		void UpdateWorkTimeChangedHandlers(TimeOfDayInterval oldValue, TimeOfDayInterval newValue) {
			if (InnerView != null && newValue != null)
				newValue.Changed += GetWorkTimeChangedHandler().Handler;
		}
		void OnWorkTimeComponentsChanged(object sender, EventArgs e) {
			UpdateInnerObjectPropertyValue(WorkTimeProperty, InnerView.WorkTime, WorkTime);
		}
		#endregion
		#region VisibleTime
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewVisibleTime"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatEnable]
		public TimeOfDayInterval VisibleTime {
			get { return (TimeOfDayInterval)GetValue(VisibleTimeProperty); }
			set { SetValue(VisibleTimeProperty, value); }
		}
		public static readonly DependencyProperty VisibleTimeProperty = CreateVisibleTimeProperty();
		static DependencyProperty CreateVisibleTimeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, TimeOfDayInterval>("VisibleTime", TimeOfDayInterval.Day, (d, e) => d.OnVisibleTimePropertyChanged(e.OldValue, e.NewValue));
		}
		void OnVisibleTimePropertyChanged(TimeOfDayInterval oldValue, TimeOfDayInterval newValue) {
			UpdateInnerObjectPropertyValue(VisibleTimeProperty, oldValue, newValue);
		}
		#endregion
		#region TimeScale
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewTimeScale"),
#endif
		XtraSerializableProperty(), NotifyParentProperty(true)]
		public TimeSpan TimeScale {
			get { return (TimeSpan)GetValue(TimeScaleProperty); }
			set { SetValue(TimeScaleProperty, value); }
		}
		public static readonly DependencyProperty TimeScaleProperty = CreateTimeScaleProperty();
		static DependencyProperty CreateTimeScaleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, TimeSpan>("TimeScale", InnerDayView.defaultTimeScale, (d, e) => d.OnTimeScaleChanged(e.OldValue, e.NewValue));
		}
		void OnTimeScaleChanged(TimeSpan oldValue, TimeSpan newValue) {
			UpdateInnerObjectPropertyValue(TimeScaleProperty, oldValue, newValue);
		}
		#endregion
		#region ShowAllDayArea
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewShowAllDayArea"),
#endif
		DefaultValue(InnerDayView.DefaultShowAllDayArea), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowAllDayArea {
			get { return (bool)GetValue(ShowAllDayAreaProperty); }
			set { SetValue(ShowAllDayAreaProperty, value); }
		}
		public static readonly DependencyProperty ShowAllDayAreaProperty = CreateShowAllDayAreaProperty();
		static DependencyProperty CreateShowAllDayAreaProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, bool>("ShowAllDayArea", InnerDayView.DefaultShowAllDayArea, (d, e) => d.OnShowAllDayAreaChanged(e.OldValue, e.NewValue));
		}
		void OnShowAllDayAreaChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowAllDayAreaProperty, oldValue, newValue);
		}
		#endregion
		#region ShowAllAppointmentsAtTimeCells
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewShowAllAppointmentsAtTimeCells"),
#endif
		DefaultValue(InnerDayView.DefaultShowAllAppointmentsAtTimeCells), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowAllAppointmentsAtTimeCells {
			get { return (bool)GetValue(ShowAllAppointmentsAtTimeCellsProperty); }
			set { SetValue(ShowAllAppointmentsAtTimeCellsProperty, value); }
		}
		public static readonly DependencyProperty ShowAllAppointmentsAtTimeCellsProperty = CreateShowAllAppointmentsAtTimeCellsProperty();
		static DependencyProperty CreateShowAllAppointmentsAtTimeCellsProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, bool>("ShowAllAppointmentsAtTimeCells", InnerDayView.DefaultShowAllAppointmentsAtTimeCells, (d, e) => d.OnShowAllAppointmentsAtTimeCellsChanged(e.OldValue, e.NewValue));
		}
		void OnShowAllAppointmentsAtTimeCellsChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowAllAppointmentsAtTimeCellsProperty, oldValue, newValue);
		}
		#endregion
		#region ShowDayHeaders
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewShowDayHeaders"),
#endif
		DefaultValue(InnerDayView.DefaultShowDayHeaders), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowDayHeaders {
			get { return (bool)GetValue(ShowDayHeadersProperty); }
			set { SetValue(ShowDayHeadersProperty, value); }
		}
		public static readonly DependencyProperty ShowDayHeadersProperty = CreateShowDayHeadersProperty();
		static DependencyProperty CreateShowDayHeadersProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, bool>("ShowDayHeaders", InnerDayView.DefaultShowDayHeaders, (d, e) => d.OnShowDayHeadersChanged(e.OldValue, e.NewValue));
		}
		void OnShowDayHeadersChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowDayHeadersProperty, oldValue, newValue);
		}
		#endregion
		#region DayCount
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("DayViewDayCount"),
#endif
		DefaultValue(InnerDayView.DefaultDayCount), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int DayCount {
			get { return (int)GetValue(DayCountProperty); }
			set { SetValue(DayCountProperty, value); }
		}
		public static readonly DependencyProperty DayCountProperty = CreateDayCountProperty();
		static DependencyProperty CreateDayCountProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, int>("DayCount", InnerDayView.DefaultDayCount, (d, e) => d.OnDayCountChanged(e.OldValue, e.NewValue));
		}
		void OnDayCountChanged(int oldValue, int newValue) {
			UpdateInnerObjectPropertyValue(DayCountProperty, oldValue, newValue);
		}
		#endregion
		#region MoreButtonUpStyle
		public static readonly DependencyProperty MoreButtonUpStyleProperty = CreateMoreButtonUpStyleProperty();
		static DependencyProperty CreateMoreButtonUpStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, Style>("MoreButtonUpStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewMoreButtonUpStyle")]
#endif
		public Style MoreButtonUpStyle {
			get { return (Style)GetValue(MoreButtonUpStyleProperty); }
			set { SetValue(MoreButtonUpStyleProperty, value); }
		}
		#endregion
		#region MoreButtonDownTemplate
		public static readonly DependencyProperty MoreButtonDownStyleProperty = CreateMoreButtonDownStyleProperty();
		static DependencyProperty CreateMoreButtonDownStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, Style>("MoreButtonDownStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewMoreButtonDownStyle")]
#endif
		public Style MoreButtonDownStyle {
			get { return (Style)GetValue(MoreButtonDownStyleProperty); }
			set { SetValue(MoreButtonDownStyleProperty, value); }
		}
		#endregion
		#region AllDayAreaCellStyle
		public static readonly DependencyProperty AllDayAreaCellStyleProperty = CreateAllDayAreaCellStyleProperty();
		static DependencyProperty CreateAllDayAreaCellStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, Style>("AllDayAreaCellStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewAllDayAreaCellStyle")]
#endif
		public Style AllDayAreaCellStyle {
			get { return (Style)GetValue(AllDayAreaCellStyleProperty); }
			set { SetValue(AllDayAreaCellStyleProperty, value); }
		}
		#endregion
		#region CurrentTimeIndicatorStyle
		public static readonly DependencyProperty CurrentTimeIndicatorStyleProperty = CreateCurrentTimeIndicatorStyleProperty();
		static DependencyProperty CreateCurrentTimeIndicatorStyleProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, Style>("CurrentTimeIndicatorStyle", null);
		}
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewCurrentTimeIndicatorStyle")]
#endif
		public Style CurrentTimeIndicatorStyle {
			get { return (Style)GetValue(CurrentTimeIndicatorStyleProperty); }
			set { SetValue(CurrentTimeIndicatorStyleProperty, value); }
		}
		#endregion
		#region ActualShowAllAppointmentsAtTimeCells
		protected internal bool ActualShowAllAppointmentsAtTimeCells {
			get { return ((InnerDayView)InnerView).ActualShowAllAppointmentsAtTimeCells; }
		}
		#endregion
		#region VerticalAppointmentStyleSelector
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewVerticalAppointmentStyleSelector")]
#endif
		public StyleSelector VerticalAppointmentStyleSelector {
			get { return (StyleSelector)GetValue(VerticalAppointmentStyleSelectorProperty); }
			set { SetValue(VerticalAppointmentStyleSelectorProperty, value); }
		}
		public static readonly DependencyProperty VerticalAppointmentStyleSelectorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, StyleSelector>("VerticalAppointmentStyleSelector", null, (d, e) => d.OnVerticalAppointmentStyleSelectorChanged(e.OldValue, e.NewValue), null);
		void OnVerticalAppointmentStyleSelectorChanged(StyleSelector oldValue, StyleSelector newValue) {
		}
		#endregion
		#region VerticalAppointmentContentTemplate
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewVerticalAppointmentContentTemplate")]
#endif
		public DataTemplate VerticalAppointmentContentTemplate {
			get { return (DataTemplate)GetValue(VerticalAppointmentContentTemplateProperty); }
			set { SetValue(VerticalAppointmentContentTemplateProperty, value); }
		}
		public static readonly DependencyProperty VerticalAppointmentContentTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, DataTemplate>("VerticalAppointmentContentTemplate", null);
		#endregion
		#region VerticalAppointmentContentTemplateSelector
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("DayViewVerticalAppointmentContentTemplateSelector")]
#endif
		public DataTemplateSelector VerticalAppointmentContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(VerticalAppointmentContentTemplateSelectorProperty); }
			set { SetValue(VerticalAppointmentContentTemplateSelectorProperty, value); }
		}
		public static readonly DependencyProperty VerticalAppointmentContentTemplateSelectorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, DataTemplateSelector>("VerticalAppointmentContentTemplateSelector", null);
		#endregion
		#region DeferredScrolling
		ISchedulerDeferredScrollingOption ISchedulerViewPropertiesBase.DeferredScrolling { get { return null; } set { } }
		#endregion
		#region TimeIndicatorDisplayOptions
		public SchedulerTimeIndicatorDisplayOptions TimeIndicatorDisplayOptions {
			get { return (SchedulerTimeIndicatorDisplayOptions)GetValue(TimeIndicatorDisplayOptionsProperty); }
			set { SetValue(TimeIndicatorDisplayOptionsProperty, value); }
		}
		public static readonly DependencyProperty TimeIndicatorDisplayOptionsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, SchedulerTimeIndicatorDisplayOptions>("TimeIndicatorDisplayOptions", null, (o, e) => o.OnTimeIndicatorDisplayOptionsChanged(e.OldValue, e.NewValue));
		void OnTimeIndicatorDisplayOptionsChanged(SchedulerTimeIndicatorDisplayOptions oldValue, SchedulerTimeIndicatorDisplayOptions newValue) {
			IVisualDayView visualDayView = VisualViewInfo as IVisualDayView;
			if (visualDayView == null)
				return;
			visualDayView.TimeIndicatorVisibility = newValue.Visibility;
		}
		#endregion
		#region TimeMarkerVisibility
		[
		DefaultValue(InnerDayView.DefaultTimeMarkerVisibility), XtraSerializableProperty(), NotifyParentProperty(true)]
		public TimeMarkerVisibility TimeMarkerVisibility {
			get { return (TimeMarkerVisibility)GetValue(TimeMarkerVisibilityProperty); }
			set { SetValue(TimeMarkerVisibilityProperty, value); }
		}
		public static readonly DependencyProperty TimeMarkerVisibilityProperty = CreateTimeMarkerVisibilityProperty();
		static DependencyProperty CreateTimeMarkerVisibilityProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, TimeMarkerVisibility>("TimeMarkerVisibility", InnerDayView.DefaultTimeMarkerVisibility, (d, e) => d.OnTimeMarkerVisibilityChanged(e.OldValue, e.NewValue));
		}
		void OnTimeMarkerVisibilityChanged(TimeMarkerVisibility oldValue, TimeMarkerVisibility newValue) {
			UpdateInnerObjectPropertyValue(TimeMarkerVisibilityProperty, oldValue, newValue);
		}
		#endregion
		#region ShowCurrentTimeInAllColumns
		[Obsolete("Use DayView.TimeIndicatorDisplayOptions.Visibility property for DayView and its descendants - WorkWeekView, FullWeekView and TimelineView.", false)]
		public bool ShowCurrentTimeInAllColumns {
			get { return (bool)GetValue(ShowCurrentTimeInAllColumnsProperty); }
			set { SetValue(ShowCurrentTimeInAllColumnsProperty, value); }
		}
		public static readonly DependencyProperty ShowCurrentTimeInAllColumnsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DayView, bool>("ShowCurrentTimeInAllColumns", true, (o, e) => o.ShowCurrentTimeInAllColumnsChanged(e.OldValue, e.NewValue));
		void ShowCurrentTimeInAllColumnsChanged(bool oldValue, bool newValue) {
			if (Control == null || Control.InnerControl == null)
				return;
			Control.InnerControl.UpdateTimeMarkerVisibilityFromOptionBehavior();
		}
		#endregion
		#endregion
		protected override void InitializePropertiesWithDefaultValues() {
			if (AppointmentDisplayOptions == null)
				AppointmentDisplayOptions = new SchedulerDayViewAppointmentDisplayOptions();
			if (TimeIndicatorDisplayOptions == null)
				TimeIndicatorDisplayOptions = new SchedulerTimeIndicatorDisplayOptions();
		}
		protected virtual void CreateTimeRulers() {
			SchedulerTimeRulerCollection timeRulers = new SchedulerTimeRulerCollection();
			timeRulers.Clear();
			this.SetValue(TimeRulersPropertyKey, timeRulers);
			timeRulers.CollectionChanged += new CollectionChangedEventHandler<SchedulerTimeRuler>(OnTimeRulersCollectionChanged);
		}
		protected virtual void OnTimeRulersCollectionChanged(object sender, CollectionChangedEventArgs<SchedulerTimeRuler> e) {
			SynchronizeToInnerTimeRulers();
		}
		protected virtual void CreateTimeSlots() {
			TimeSlotCollection timeSlots = new TimeSlotCollection();
			timeSlots.Clear();
			this.SetValue(TimeSlotsPropertyKey, timeSlots);
			TimeSlots.CollectionChanged += new CollectionChangedEventHandler<TimeSlot>(OnTimeSlotsCollectionChanged);
		}
		protected virtual void OnTimeSlotsCollectionChanged(object sender, CollectionChangedEventArgs<TimeSlot> e) {
			SynchronizeToInnerTimeSlots();
		}
		protected override DependencyPropertySyncManager CreatePropertySyncManager() {
			return new DayViewPropertySyncManager(this);
		}
		protected override SchedulerAppointmentDisplayOptions GetAppointmentDisplayOptions() {
			return this.AppointmentDisplayOptions;
		}
		protected internal void SynchronizeToInnerTimeSlots() {
			if (InnerView == null)
				return;
			TimeSlotCollection actualTimeSlots = TimeSlots.Count > 0 ? TimeSlots : DefaultTimeSlots;
			TimeSlotCollection innerTimeSlots = InnerView.TimeSlots;
			innerTimeSlots.BeginUpdate();
			try {
				innerTimeSlots.Clear();
				innerTimeSlots.AddRange(actualTimeSlots);
			} finally {
				innerTimeSlots.EndUpdate();
			}
		}
		protected internal virtual void SynchronizeToInnerTimeRulers() {
			if (InnerView == null)
				return;
			SchedulerTimeRulerCollection actualTimeRulers = TimeRulers.Count > 0 ? TimeRulers : DefaultTimeRulers;
			TimeRulerCollection innerTimeRulers = ((InnerDayView)InnerView).TimeRulers;
			innerTimeRulers.BeginUpdate();
			try {
				innerTimeRulers.Clear();
				innerTimeRulers.AddRange(actualTimeRulers);
			} finally {
				innerTimeRulers.EndUpdate();
			}
		}
		protected internal override void SetupInnerViewBindings() {
			base.SetupInnerViewBindings();
			SynchronizeToInnerTimeRulers();
			SynchronizeToInnerTimeSlots();
			WorkTime.Changed += GetWorkTimeChangedHandler().Handler;
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerDayView(this,  this);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new DayViewAppointmentDisplayOptions();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new DayViewFactoryHelper(); 
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new DayViewDateTimeScrollController(this);
		}
		protected internal override AppointmentsLayoutCalculator CreateAppointmentsLayoutCalculator() {
			return new DayViewAppointmentsLayoutCalculator(this);
		}
		protected internal override SetSelectionCommand CreateSetSelectionCommand(InnerSchedulerControl control, TimeInterval interval, Resource resource) {
			return base.CreateSetSelectionCommand(control, interval, resource);
		}
		protected override bool ValidateAppointmentDisplayOptions(SchedulerAppointmentDisplayOptions appointmentDisplayOptions) {
			return appointmentDisplayOptions is SchedulerDayViewAppointmentDisplayOptions;
		}
		protected internal override void AttachExistingInnerView(SchedulerControl control, InnerSchedulerViewBase view) {
			base.AttachExistingInnerView(control, view);
		}
		#region IInnerDayViewOwner Members
		TimeInterval IInnerDayViewOwner.GetAvailableRowsTimeInterval() {
			return ActualVisibleTime.ToTimeInterval();
		}
		WeakEventHandler<DayView, EventArgs, EventHandler> GetWorkTimeChangedHandler() {
			if (workTimeChangedHandler == null)
				workTimeChangedHandler = new WeakEventHandler<DayView, EventArgs, EventHandler>(this, (view, s, e) => view.OnWorkTimeComponentsChanged(s, e), null, wh => new EventHandler(wh.OnEvent));
			return workTimeChangedHandler;
		}
		TimeOfDayInterval GetTimeOffset() {
			DayViewScrollBarAdapter adapter = (DayViewScrollBarAdapter)Control.DateTimeScrollBarObject.ScrollBarAdapter;
			return adapter.GetTimeOffset(ActualVisibleTime);
		}
		TimeInterval IInnerDayViewOwner.GetVisibleRowsTimeInterval() {
			TimeOfDayInterval timeOffset = GetTimeOffset();
			return new TimeInterval(DateTimeHelper.ToDateTime(timeOffset.Start), DateTimeHelper.ToDateTime(timeOffset.End));
		}
		TimeSpan IInnerDayViewOwner.GetTopRowTime() {
			TimeOfDayInterval timeOffset = GetTimeOffset();
			return timeOffset.Start;
		}
		void IInnerDayViewOwner.SetScrollStartTimeCore(TimeSpan value) {
			TopRowTime = value;
		}
		#endregion
		protected override void MakeAppointmentVisibleInScrollContainers(Appointment apt) {
			base.MakeAppointmentVisibleInScrollContainers(apt);
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region DayViewContentTemplateSelector
	public class DayViewContentStyleSelector : ViewContentStyleSelector {
		protected internal override Type GroupByNoneType { get { return typeof(VisualDayViewGroupByNone); } }
		protected internal override Type GroupByDateType { get { return typeof(VisualDayViewGroupByDate); } }
		protected internal override Type GroupByResourceType { get { return typeof(VisualDayViewGroupByResource); } }
	}
	#endregion
}
