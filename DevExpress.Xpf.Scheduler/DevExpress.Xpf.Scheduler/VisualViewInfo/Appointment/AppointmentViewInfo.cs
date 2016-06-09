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

using System.Windows;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;
using System;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.XtraScheduler.Drawing.Internal;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region VisualAppointmentViewInfo
	public class VisualAppointmentViewInfo : DependencyObject, ISelectableIntervalViewInfo, IAppointmentView, ISupportCopyFrom<AppointmentControl>, IBatchUpdateable, IBatchUpdateHandler {
		bool suspendEvents;
		int changedPropertiesCounter;
		BatchUpdateHelper batchUpdateHelper;
		double version;
		public VisualAppointmentViewInfo() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
		}
		#region Properties
		#region HasTopBorder
		bool lastHasTopBorder = false;
		public bool HasTopBorder {
			get { return (bool)GetValue(HasTopBorderProperty); }
			set { if (this.lastHasTopBorder != value) SetValue(HasTopBorderProperty, value); }
		}
		public static readonly DependencyProperty HasTopBorderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("HasTopBorder", false, (d, e) => d.OnHasTopBorderChanged(e.OldValue, e.NewValue), null);
		void OnHasTopBorderChanged(bool oldValue, bool newValue) {
			this.lastHasTopBorder = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region HasBottomBorder
		bool lastHasBottomBorder = false;
		public bool HasBottomBorder {
			get { return (bool)GetValue(HasBottomBorderProperty); }
			set { if (this.lastHasBottomBorder != value) SetValue(HasBottomBorderProperty, value); }
		}
		public static readonly DependencyProperty HasBottomBorderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("HasBottomBorder", false, (d, e) => d.OnHasBottomBorderChanged(e.OldValue, e.NewValue), null);
		void OnHasBottomBorderChanged(bool oldValue, bool newValue) {
			this.lastHasBottomBorder = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region HasLeftBorder
		bool lastHasLeftBorder = false;
		public bool HasLeftBorder {
			get { return (bool)GetValue(HasLeftBorderProperty); }
			set { if (this.lastHasLeftBorder != value) SetValue(HasLeftBorderProperty, value); }
		}
		public static readonly DependencyProperty HasLeftBorderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("HasLeftBorder", false, (d, e) => d.OnHasLeftBorderChanged(e.OldValue, e.NewValue), null);
		void OnHasLeftBorderChanged(bool oldValue, bool newValue) {
			this.lastHasLeftBorder = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region HasRightBorder
		bool lastHasRightBorder = false;
		public bool HasRightBorder {
			get { return (bool)GetValue(HasRightBorderProperty); }
			set { if (this.lastHasRightBorder != value) SetValue(HasRightBorderProperty, value); }
		}
		public static readonly DependencyProperty HasRightBorderProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("HasRightBorder", false, (d, e) => d.OnHasRightBorderChanged(e.OldValue, e.NewValue), null);
		void OnHasRightBorderChanged(bool oldValue, bool newValue) {
			this.lastHasRightBorder = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region IsStartVisible
		bool lastIsStartVisible = false;
		public bool IsStartVisible {
			get { return (bool)GetValue(IsStartVisibleProperty); }
			set { if (this.lastIsStartVisible != value) SetValue(IsStartVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsStartVisibleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("IsStartVisible", false, (d, e) => d.OnIsStartVisibleChanged(e.OldValue, e.NewValue), null);
		void OnIsStartVisibleChanged(bool oldValue, bool newValue) {
			this.lastIsStartVisible = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region IsEndVisible
		bool lastIsEndVisible = false;
		public bool IsEndVisible {
			get { return (bool)GetValue(IsEndVisibleProperty); }
			set { if (this.lastIsEndVisible != value) SetValue(IsEndVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsEndVisibleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("IsEndVisible", false, (d, e) => d.OnIsEndVisibleChanged(e.OldValue, e.NewValue), null);
		void OnIsEndVisibleChanged(bool oldValue, bool newValue) {
			this.lastIsEndVisible = newValue;
			OnPropertiesChanged();
		}
		#endregion
		public bool Visible { get; set; }
		#region Selected
		bool lastSelected = false;
		public bool Selected {
			get { return (bool)GetValue(SelectedProperty); }
			set { if (this.lastSelected != value) SetValue(SelectedProperty, value); }
		}
		public static readonly DependencyProperty SelectedProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("Selected", false, (d, e) => d.OnSelectedChanged(e.OldValue, e.NewValue), null);
		void OnSelectedChanged(bool oldValue, bool newValue) {
			this.lastSelected = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region Label
		IAppointmentLabel lastLabel = null;
		public IAppointmentLabel Label {
			get { return (IAppointmentLabel)GetValue(LabelProperty); }
			set { if (this.lastLabel != value) SetValue(LabelProperty, value); }
		}
		public static readonly DependencyProperty LabelProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, IAppointmentLabel>("Label", null, (d, e) => d.OnLabelChanged(e.OldValue, e.NewValue), null);
		void OnLabelChanged(IAppointmentLabel oldValue, IAppointmentLabel newValue) {
			this.lastLabel = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region LabelColor
		Color lastLabelColor = Colors.White;
		public Color LabelColor {
			get { return (Color)GetValue(LabelColorProperty); }
			set { if (this.lastLabelColor != value) SetValue(LabelColorProperty, value); }
		}
		public static readonly DependencyProperty LabelColorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, Color>("LabelColor", Colors.White, (d, e) => d.OnLabelColorChanged(e.OldValue, e.NewValue), null);
		void OnLabelColorChanged(Color oldValue, Color newValue) {
			this.lastLabelColor = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region LabelBrush
		Brush lastLabelBrush = null;
		public Brush LabelBrush {
			get { return (Brush)GetValue(LabelBrushProperty); }
			set { if (this.lastLabelBrush != value) SetValue(LabelBrushProperty, value); }
		}
		public static readonly DependencyProperty LabelBrushProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, Brush>("LabelBrush", null, (d, e) => d.OnLabelBrushChanged(e.OldValue, e.NewValue), null);
		void OnLabelBrushChanged(Brush oldValue, Brush newValue) {
			this.lastLabelBrush = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region StatusBrush
		Brush lastStatusBrush = null;
		public Brush StatusBrush {
			get { return (Brush)GetValue(StatusBrushProperty); }
			set { if (this.lastStatusBrush != value) SetValue(StatusBrushProperty, value); }
		}
		public static readonly DependencyProperty StatusBrushProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, Brush>("StatusBrush", null, (d, e) => d.OnStatusBrushChanged(e.OldValue, e.NewValue), null);
		void OnStatusBrushChanged(Brush oldValue, Brush newValue) {
			this.lastStatusBrush = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region Status
		IAppointmentStatus lastStatus = null;
		public IAppointmentStatus Status {
			get { return (IAppointmentStatus)GetValue(StatusProperty); }
			set { if (this.lastStatus != value) SetValue(StatusProperty, value); }
		}
		public static readonly DependencyProperty StatusProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, IAppointmentStatus>("Status", null, (d, e) => d.OnStatusChanged(e.OldValue, e.NewValue), null);
		void OnStatusChanged(IAppointmentStatus oldValue, IAppointmentStatus newValue) {
			this.lastStatus = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region AppointmentStart
		DateTime lastAppointmentStart = DateTime.MinValue;
		public DateTime AppointmentStart {
			get { return (DateTime)GetValue(AppointmentStartProperty); }
			set { if (this.lastAppointmentStart != value) SetValue(AppointmentStartProperty, value); }
		}
		public static readonly DependencyProperty AppointmentStartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, DateTime>("AppointmentStart", DateTime.MinValue, (d, e) => d.OnAppointmentStartChanged(e.OldValue, e.NewValue), null);
		void OnAppointmentStartChanged(DateTime oldValue, DateTime newValue) {
			this.lastAppointmentStart = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region AppointmentEnd
		DateTime lastAppointmentEnd = DateTime.MinValue;
		public DateTime AppointmentEnd {
			get { return (DateTime)GetValue(AppointmentEndProperty); }
			set { if (this.lastAppointmentEnd != value) SetValue(AppointmentEndProperty, value); }
		}
		public static readonly DependencyProperty AppointmentEndProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, DateTime>("AppointmentEnd", DateTime.MinValue, (d, e) => d.OnAppointmentEndChanged(e.OldValue, e.NewValue), null);
		void OnAppointmentEndChanged(DateTime oldValue, DateTime newValue) {
			this.lastAppointmentEnd = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region AppointmentStatusId
		object lastAppointmentStatusId;
		public object AppointmentStatusId {
			get { return GetValue(AppointmentStatusIdProperty); }
			set {
				if (!Object.Equals(lastAppointmentStatusId, value))
					SetValue(AppointmentStatusIdProperty, value);
			}
		}
		public static readonly DependencyProperty AppointmentStatusIdProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, object>("AppointmentStatusId", 0, (d, e) => d.OnAppointmentStatusIdChanged(e.OldValue, e.NewValue), null);
		void OnAppointmentStatusIdChanged(object oldValue, object newValue) {
			this.lastAppointmentStatusId = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region AppointmentLabelId
		object lastAppointmentLabelId;
		public object AppointmentLabelId {
			get { return GetValue(AppointmentLabelIdProperty); }
			set {
				if (!Object.Equals(this.lastAppointmentLabelId, value))
					SetValue(AppointmentLabelIdProperty, value);
			}
		}
		public static readonly DependencyProperty AppointmentLabelIdProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, object>("AppointmentLabelId", 0, (d, e) => d.OnAppointmentLabelIdChanged(e.OldValue, e.NewValue), null);
		void OnAppointmentLabelIdChanged(object oldValue, object newValue) {
			this.lastAppointmentLabelId = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region IntervalStart
		DateTime lastIntervalStart = DateTime.MinValue;
		public DateTime IntervalStart {
			get { return (DateTime)GetValue(IntervalStartProperty); }
			set { if (this.lastIntervalStart != value) SetValue(IntervalStartProperty, value); }
		}
		public static readonly DependencyProperty IntervalStartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, DateTime>("IntervalStart", DateTime.MinValue, (d, e) => d.OnIntervalStartChanged(e.OldValue, e.NewValue), null);
		void OnIntervalStartChanged(DateTime oldValue, DateTime newValue) {
			this.lastIntervalStart = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region IntervalEnd
		DateTime lastIntervalEnd = DateTime.MinValue;
		public DateTime IntervalEnd {
			get { return (DateTime)GetValue(IntervalEndProperty); }
			set { if (this.lastIntervalEnd != value) SetValue(IntervalEndProperty, value); }
		}
		public static readonly DependencyProperty IntervalEndProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, DateTime>("IntervalEnd", DateTime.MinValue, (d, e) => d.OnIntervalEndChanged(e.OldValue, e.NewValue), null);
		void OnIntervalEndChanged(DateTime oldValue, DateTime newValue) {
			this.lastIntervalEnd = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region AppointmentDuration
		TimeSpan lastAppointmentDuration = TimeSpan.Zero;
		public TimeSpan AppointmentDuration {
			get { return (TimeSpan)GetValue(AppointmentDurationProperty); }
			set { if (this.lastAppointmentDuration != value) SetValue(AppointmentDurationProperty, value); }
		}
		public static readonly DependencyProperty AppointmentDurationProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, TimeSpan>("AppointmentDuration", TimeSpan.Zero, (d, e) => d.OnAppointmentDurationChanged(e.OldValue, e.NewValue), null);
		void OnAppointmentDurationChanged(TimeSpan oldValue, TimeSpan newValue) {
			this.lastAppointmentDuration = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region ResourceId
		object lastResourceId = ResourceBase.Empty.Id;
		public object ResourceId {
			get { return (object)GetValue(ResourceIdProperty); }
			set { if (this.lastResourceId != value) SetValue(ResourceIdProperty, value); }
		}
		public static readonly DependencyProperty ResourceIdProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, object>("ResourceId", ResourceBase.Empty.Id, (d, e) => d.OnResourceIdChanged(e.OldValue, e.NewValue), null);
		void OnResourceIdChanged(object oldValue, object newValue) {
			this.lastResourceId = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region Options
		AppointmentViewInfoOptions lastOptions = null;
		public AppointmentViewInfoOptions Options {
			get { return (AppointmentViewInfoOptions)GetValue(OptionsProperty); }
			set { if (this.lastOptions != value) SetValue(OptionsProperty, value); }
		}
		public static readonly DependencyProperty OptionsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, AppointmentViewInfoOptions>("Options", null, (d, e) => d.OnOptionsChanged(e.OldValue, e.NewValue), null);
		void OnOptionsChanged(AppointmentViewInfoOptions oldValue, AppointmentViewInfoOptions newValue) {
			this.lastOptions = newValue;
			if (oldValue != null && oldValue.Compare(newValue))
				return;
			OnPropertiesChanged();
		}
		#endregion
		#region View
		SchedulerViewBase lastView = null;
		public SchedulerViewBase View {
			get { return (SchedulerViewBase)GetValue(ViewProperty); }
			set { if (this.lastView != value) SetValue(ViewProperty, value); }
		}
		public static readonly DependencyProperty ViewProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, SchedulerViewBase>("View", null, (d, e) => d.OnViewChanged(e.OldValue, e.NewValue), null);
		void OnViewChanged(SchedulerViewBase oldValue, SchedulerViewBase newValue) {
			this.lastView = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region Subject
		string lastSubject = String.Empty;
		public string Subject {
			get { return (string)GetValue(SubjectProperty); }
			set { if (this.lastSubject != value) SetValue(SubjectProperty, value); }
		}
		public static readonly DependencyProperty SubjectProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, string>("Subject", String.Empty, (d, e) => d.OnSubjectChanged(e.OldValue, e.NewValue), null);
		void OnSubjectChanged(string oldValue, string newValue) {
			this.lastSubject = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region Location
		string lastLocation = String.Empty;
		public string Location {
			get { return (string)GetValue(LocationProperty); }
			set { if (this.lastLocation != value) SetValue(LocationProperty, value); }
		}
		public static readonly DependencyProperty LocationProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, string>("Location", String.Empty, (d, e) => d.OnLocationChanged(e.OldValue, e.NewValue), null);
		void OnLocationChanged(string oldValue, string newValue) {
			this.lastLocation = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region Description
		string lastDescription = String.Empty;
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { if (this.lastDescription != value) SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, string>("Description", String.Empty, (d, e) => d.OnDescriptionChanged(e.OldValue, e.NewValue), null);
		void OnDescriptionChanged(string oldValue, string newValue) {
			this.lastDescription = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region StartTimeText
		string lastStartTimeText = String.Empty;
		public string StartTimeText {
			get { return (string)GetValue(StartTimeTextProperty); }
			set { if (this.lastStartTimeText != value) SetValue(StartTimeTextProperty, value); }
		}
		public static readonly DependencyProperty StartTimeTextProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, string>("StartTimeText", String.Empty, (d, e) => d.OnStartTimeTextChanged(e.OldValue, e.NewValue), null);
		void OnStartTimeTextChanged(string oldValue, string newValue) {
			this.lastStartTimeText = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region EndTimeText
		string lastEndTimeText = String.Empty;
		public string EndTimeText {
			get { return (string)GetValue(EndTimeTextProperty); }
			set { if (this.lastEndTimeText != value) SetValue(EndTimeTextProperty, value); }
		}
		public static readonly DependencyProperty EndTimeTextProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, string>("EndTimeText", String.Empty, (d, e) => d.OnEndTimeTextChanged(e.OldValue, e.NewValue), null);
		void OnEndTimeTextChanged(string oldValue, string newValue) {
			this.lastEndTimeText = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region SameDay
		bool lastSameDay = false;
		public bool SameDay {
			get { return (bool)GetValue(SameDayProperty); }
			set { if (this.lastSameDay != value) SetValue(SameDayProperty, value); }
		}
		public static readonly DependencyProperty SameDayProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("SameDay", false, (d, e) => d.OnSameDayChanged(e.OldValue, e.NewValue), null);
		void OnSameDayChanged(bool oldValue, bool newValue) {
			this.lastSameDay = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region LongerThanADay
		bool lastLongerThanADay = false;
		public bool LongerThanADay {
			get { return (bool)GetValue(LongerThanADayProperty); }
			set { if (this.lastLongerThanADay != value) SetValue(LongerThanADayProperty, value); }
		}
		public static readonly DependencyProperty LongerThanADayProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("LongerThanADay", false, (d, e) => d.OnLongerThanADayChanged(e.OldValue, e.NewValue), null);
		void OnLongerThanADayChanged(bool oldValue, bool newValue) {
			this.lastLongerThanADay = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region ShowTimeAsClock
		bool lastShowTimeAsClock = false;
		public bool ShowTimeAsClock {
			get { return (bool)GetValue(ShowTimeAsClockProperty); }
			set { if (this.lastShowTimeAsClock != value) SetValue(ShowTimeAsClockProperty, value); }
		}
		public static readonly DependencyProperty ShowTimeAsClockProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("ShowTimeAsClock", false, (d, e) => d.OnShowTimeAsClockChanged(e.OldValue, e.NewValue), null);
		void OnShowTimeAsClockChanged(bool oldValue, bool newValue) {
			this.lastShowTimeAsClock = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region ShowStartContinueItem
		bool lastShowStartContinueItem = false;
		public bool ShowStartContinueItem {
			get { return (bool)GetValue(ShowStartContinueItemProperty); }
			set { if (this.lastShowStartContinueItem != value) SetValue(ShowStartContinueItemProperty, value); }
		}
		public static readonly DependencyProperty ShowStartContinueItemProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("ShowStartContinueItem", false, (d, e) => d.OnShowStartContinueItemChanged(e.OldValue, e.NewValue), null);
		void OnShowStartContinueItemChanged(bool oldValue, bool newValue) {
			this.lastShowStartContinueItem = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region CanResizeStart
		bool lastCanResizeStart = false;
		public bool CanResizeStart {
			get { return (bool)GetValue(CanResizeStartProperty); }
			set { if (this.lastCanResizeStart != value) SetValue(CanResizeStartProperty, value); }
		}
		public static readonly DependencyProperty CanResizeStartProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("CanResizeStart", false, (d, e) => d.OnCanResizeStartChanged(e.OldValue, e.NewValue), null);
		void OnCanResizeStartChanged(bool oldValue, bool newValue) {
			this.lastCanResizeStart = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region CanResizeEnd
		bool lastCanResizeEnd = false;
		public bool CanResizeEnd {
			get { return (bool)GetValue(CanResizeEndProperty); }
			set { if (this.lastCanResizeEnd != value) SetValue(CanResizeEndProperty, value); }
		}
		public static readonly DependencyProperty CanResizeEndProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("CanResizeEnd", false, (d, e) => d.OnCanResizeEndChanged(e.OldValue, e.NewValue), null);
		void OnCanResizeEndChanged(bool oldValue, bool newValue) {
			this.lastCanResizeEnd = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region ShowEndContinueItem
		bool lastShowEndContinueItem = false;
		public bool ShowEndContinueItem {
			get { return (bool)GetValue(ShowEndContinueItemProperty); }
			set { if (this.lastShowEndContinueItem != value) SetValue(ShowEndContinueItemProperty, value); }
		}
		public static readonly DependencyProperty ShowEndContinueItemProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, bool>("ShowEndContinueItem", false, (d, e) => d.OnShowEndContinueItemChanged(e.OldValue, e.NewValue), null);
		void OnShowEndContinueItemChanged(bool oldValue, bool newValue) {
			this.lastShowEndContinueItem = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region StartContinueItemText
		string lastStartContinueItemText = String.Empty;
		public string StartContinueItemText {
			get { return (string)GetValue(StartContinueItemTextProperty); }
			set { if (this.lastStartContinueItemText != value) SetValue(StartContinueItemTextProperty, value); }
		}
		public static readonly DependencyProperty StartContinueItemTextProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, string>("StartContinueItemText", String.Empty, (d, e) => d.OnStartContinueItemTextChanged(e.OldValue, e.NewValue), null);
		void OnStartContinueItemTextChanged(string oldValue, string newValue) {
			this.lastStartContinueItemText = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region EndContinueItemText
		string lastEndContinueItemText = String.Empty;
		public string EndContinueItemText {
			get { return (string)GetValue(EndContinueItemTextProperty); }
			set { if (this.lastEndContinueItemText != value) SetValue(EndContinueItemTextProperty, value); }
		}
		public static readonly DependencyProperty EndContinueItemTextProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, string>("EndContinueItemText", String.Empty, (d, e) => d.OnEndContinueItemTextChanged(e.OldValue, e.NewValue), null);
		void OnEndContinueItemTextChanged(string oldValue, string newValue) {
			this.lastEndContinueItemText = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region CustomViewInfo
		object lastCustomViewInfo = null;
		public object CustomViewInfo {
			get { return (object)GetValue(CustomViewInfoProperty); }
			set { if (!Object.Equals(this.lastCustomViewInfo, value)) SetValue(CustomViewInfoProperty, value); }
		}
		public static readonly DependencyProperty CustomViewInfoProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualAppointmentViewInfo, object>("CustomViewInfo", null, (d, e) => d.OnCustomViewInfoChanged(e.OldValue, e.NewValue), null);
		void OnCustomViewInfoChanged(object oldValue, object newValue) {
			this.lastCustomViewInfo = newValue;
			OnPropertiesChanged();
		}
		#endregion
		#region ToolTipText
		public string ToolTipText {
			get { return (string)GetValue(ToolTipTextProperty); }
			protected set { this.SetValue(ToolTipTextPropertyKey, value); }
		}
		static readonly DependencyPropertyKey ToolTipTextPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualAppointmentViewInfo, string>("ToolTipText", String.Empty);
		public static readonly DependencyProperty ToolTipTextProperty = ToolTipTextPropertyKey.DependencyProperty;
		#endregion
		internal Appointment Appointment { get { return appointment; } }
		internal double Version { get { return version; } }
		internal VisualAppointmentControl Control { get; set; }
		internal double Height { get; set; }
		#endregion
		#region Events
		EventHandler onPropertiesChanged;
		public event EventHandler PropertiesChanged {
			add { onPropertiesChanged += value; }
			remove { onPropertiesChanged -= value; }
		}
		void OnPropertiesChanged() {
			if (this.suspendEvents) {
				this.changedPropertiesCounter++;
				return;
			}
			IncreaseVersion();
			RaiseOnPropertiesChanged();
		}
		protected void RaiseOnPropertiesChanged() {
			if (onPropertiesChanged != null)
				onPropertiesChanged(this, EventArgs.Empty);
		}
		#endregion
		Appointment appointment;
		protected internal TimeInterval GetInterval() {
			return new TimeInterval(IntervalStart, IntervalEnd);
		}
		protected internal bool IsLongTime() {
			TimeInterval interval = new TimeInterval(AppointmentStart, AppointmentEnd);
			return interval.LongerThanADay || !interval.SameDay;
		}
		#region ISelectableIntervalViewInfo Members
		SchedulerHitTest ISelectableIntervalViewInfo.HitTestType {
			get { return SchedulerHitTest.AppointmentContent; }
		}
		TimeInterval ISelectableIntervalViewInfo.Interval {
			get { return GetInterval(); }
		}
		Resource ISelectableIntervalViewInfo.Resource {
			get { return View.Control.Storage.ResourceStorage.GetResourceById(ResourceId); }
		}
		bool ISelectableIntervalViewInfo.Selected {
			get { return Selected; }
		}
		#endregion
		protected internal virtual void SetAppointment(Appointment appointment) {
			this.appointment = appointment;
		}
		#region IAppointmentView Members
		Appointment IAppointmentView.Appointment {
			get { return appointment; }
		}
		#endregion
		AppointmentContentCalculatorHelper contentCalculatorHelper;
		protected internal AppointmentContentCalculatorHelper GetContentCalculatorHelper() {
			return this.contentCalculatorHelper;
		}
		public void SetContentCalculatorHelper(AppointmentContentCalculatorHelper contentCalculatorHelper) {
			this.contentCalculatorHelper = contentCalculatorHelper;
		}
		void IncreaseVersion() {
			this.version++;
		}
		#region ISupportCopyFrom<AppointmentControl> Members
		public void CopyFrom(AppointmentControl source) {
			BeginUpdate();
			SetAppointment(source.Appointment);
			AppointmentDuration = source.Appointment.Duration;
			AppointmentEnd = source.Appointment.End;
			AppointmentStart = source.Appointment.Start;
			AppointmentStatusId = source.Appointment.StatusKey;
			AppointmentLabelId = source.Appointment.LabelKey;
			HasBottomBorder = source.ViewInfo.HasBottomBorder;
			HasLeftBorder = source.ViewInfo.HasLeftBorder;
			HasRightBorder = source.ViewInfo.HasRightBorder;
			HasTopBorder = source.ViewInfo.HasTopBorder;
			IntervalEnd = source.ViewInfo.Interval.End;
			IntervalStart = source.ViewInfo.Interval.Start;
			IsEndVisible = source.ViewInfo.IsEndVisible;
			IsStartVisible = source.ViewInfo.IsStartVisible;
			Label = source.ViewInfo.Label;
			LabelColor = source.ViewInfo.LabelColor;
			LabelBrush = source.ViewInfo.LabelBrush;
			ResourceId = source.ViewInfo.Resource.Id;
			Selected = source.ViewInfo.Selected;
			Status = source.ViewInfo.Status;
			StatusBrush = source.ViewInfo.StatusBrush;
			Visible = source.ViewInfo.Visible;
			Options = source.ViewInfo.Options;
			Subject = source.ViewInfo.Subject;
			Location = source.ViewInfo.Location;
			Description = source.ViewInfo.Description;
			SameDay = source.ViewInfo.AppointmentInterval.SameDay;
			LongerThanADay = source.ViewInfo.AppointmentInterval.LongerThanADay;
			CustomViewInfo = source.ViewInfo.CustomViewInfo;
			View = source.View;
			StartTimeText = source.ViewInfo.ContentCalculatorHelper.GetStartTimeText(source.ViewInfo);
			EndTimeText = source.ViewInfo.ContentCalculatorHelper.GetEndTimeText(source.ViewInfo);
			ShowTimeAsClock = source.ViewInfo.ContentCalculatorHelper.ShouldShowTimeAsClock(source.ViewInfo);
			HorizontalAppointmentContentCalculatorHelper horizontalContentCalculatorHelper = source.ViewInfo.ContentCalculatorHelper as HorizontalAppointmentContentCalculatorHelper;
			if (horizontalContentCalculatorHelper != null) {
				ShowStartContinueItem = !IsStartVisible && horizontalContentCalculatorHelper.ShouldShowStartContinueItems(source.ViewInfo);
				ShowEndContinueItem = !IsEndVisible && horizontalContentCalculatorHelper.ShouldShowEndContinueItems(source.ViewInfo);
				StartContinueItemText = horizontalContentCalculatorHelper.GetStartContinueItemText(source.ViewInfo);
				EndContinueItemText = horizontalContentCalculatorHelper.GetEndContinueItemText(source.ViewInfo);
			}
			else {
				ShowStartContinueItem = false;
				ShowStartContinueItem = false;
				StartContinueItemText = String.Empty;
				EndContinueItemText = String.Empty;
			}
			string locationText = !string.IsNullOrEmpty(Location) ? string.Format(" {0}", Location) : string.Empty;
			ToolTipText = string.Format("{0}-{1} {2}{3}", StartTimeText, EndTimeText, Subject, locationText);
			bool allowAppointmentResize = View.Control.MouseHandler.AppointmentOperationHelper.CanResizeAppointment(source.Appointment);
			CanResizeStart = (HasLeftBorder || HasTopBorder) && allowAppointmentResize;
			CanResizeEnd = (HasRightBorder || HasBottomBorder) && allowAppointmentResize;
			SetAppointment(source.Appointment);
			EndUpdate();
			SchedulerLogger.Trace(XpfLoggerTraceLevel.AppoinmentPanel, "->VisualAppointmentViewInfo.CopyFrom");
		}
		#endregion
		public virtual void CopyFrom(VisualAppointmentViewInfo source) {
			BeginUpdate();
			SetAppointment(source.Appointment);
			AppointmentDuration = source.AppointmentDuration;
			AppointmentEnd = source.AppointmentEnd;
			AppointmentStart = source.AppointmentStart;
			AppointmentStatusId = source.AppointmentStatusId;
			AppointmentLabelId = source.AppointmentLabelId;
			HasBottomBorder = source.HasBottomBorder;
			HasLeftBorder = source.HasLeftBorder;
			HasRightBorder = source.HasRightBorder;
			HasTopBorder = source.HasTopBorder;
			IntervalEnd = source.IntervalEnd;
			IntervalStart = source.IntervalStart;
			IsEndVisible = source.IsEndVisible;
			IsStartVisible = source.IsStartVisible;
			LabelColor = source.LabelColor;
			LabelBrush = source.LabelBrush;
			ResourceId = source.ResourceId;
			Selected = source.Selected;
			Status = source.Status;
			StatusBrush = source.StatusBrush;
			Visible = source.Visible;
			Options = source.Options;
			Subject = source.Subject;
			Location = source.Location;
			Description = source.Description;
			SameDay = source.SameDay;
			LongerThanADay = source.LongerThanADay;
			CustomViewInfo = source.CustomViewInfo;
			View = source.View;
			StartTimeText = source.StartTimeText;
			EndTimeText = source.EndTimeText;
			ShowTimeAsClock = source.ShowTimeAsClock;
			ShowStartContinueItem = source.ShowStartContinueItem;
			ShowStartContinueItem = source.ShowStartContinueItem;
			StartContinueItemText = source.StartContinueItemText;
			EndContinueItemText = source.EndContinueItemText;
			ToolTipText = source.ToolTipText;
			CanResizeStart = source.CanResizeStart;
			CanResizeEnd = source.CanResizeEnd;
			SetAppointment(source.Appointment);
			EndUpdate();
		}
		public virtual VisualAppointmentViewInfo Clone() {
			VisualAppointmentViewInfo clone = new VisualAppointmentViewInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#region IBatchUpdateable
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		public void BeginUpdate() {
			this.batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			this.batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			this.batchUpdateHelper.CancelUpdate();
		}
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
			OnBeginUpdate();
		}
		void IBatchUpdateHandler.OnEndUpdate() {
			OnEndUpdate();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdate();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
			OnCancelUpdate();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdate();
		}
		protected virtual void OnFirstBeginUpdate() {
			this.changedPropertiesCounter = 0;
			this.suspendEvents = true;
		}
		protected virtual void OnBeginUpdate() {
		}
		protected virtual void OnEndUpdate() {
		}
		protected virtual void OnLastEndUpdate() {
			this.suspendEvents = false;
			if (this.changedPropertiesCounter > 0) {
				IncreaseVersion();
				RaiseOnPropertiesChanged();
			}
			this.changedPropertiesCounter = 0;
		}
		protected virtual void OnCancelUpdate() {
		}
		protected virtual void OnLastCancelUpdate() {
			this.suspendEvents = false;
			if (this.changedPropertiesCounter > 0)
				IncreaseVersion();
			this.changedPropertiesCounter = 0;
		}
		#endregion
	}
	#endregion
}
