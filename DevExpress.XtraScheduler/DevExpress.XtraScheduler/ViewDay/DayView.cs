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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Implementation;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler {
	#region DayView
	public class DayView : SchedulerViewBase, IInnerDayViewOwner {
		#region Fields
		const int DefaultStatusLineWidth = 0;
		const int DefaultRowHeight = 0;
		TimeSpan scrollStartTime;
		VisibleRowsInfo prevVisibleRowsInfo = new VisibleRowsInfo();
		int statusLineWidth = DefaultStatusLineWidth;
		int rowHeight = DefaultRowHeight;
		TimeIndicatorDisplayOptions timeIndicatorDisplayOptions;
		#endregion
		public DayView(SchedulerControl control)
			: base(control) {
			this.timeIndicatorDisplayOptions = CreateTimeIndicatorDisplayOptions();
		}
		#region Properties
		#region ShowWorkTimeOnly
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewShowWorkTimeOnly"),
#endif
DefaultValue(InnerDayView.DefaultShowWorkTimeOnly), XtraSerializableProperty()]
		public bool ShowWorkTimeOnly {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowWorkTimeOnly;
				} else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowWorkTimeOnly = value;
				}
			}
		}
		#endregion
		#region ShowDayHeaders
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewShowDayHeaders"),
#endif
DefaultValue(InnerDayView.DefaultShowDayHeaders), XtraSerializableProperty()]
		public bool ShowDayHeaders {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowDayHeaders;
				} else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowDayHeaders = value;
				}
			}
		}
		#endregion
		#region ShowMoreButtonsOnEachColumn
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewShowMoreButtonsOnEachColumn"),
#endif
DefaultValue(InnerDayView.DefaultShowMoreButtonsOnEachColumn), XtraSerializableProperty()]
		public bool ShowMoreButtonsOnEachColumn {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowMoreButtonsOnEachColumn;
				} else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowMoreButtonsOnEachColumn = value;
				}
			}
		}
		#endregion
		#region AppointmentShadows
		[Obsolete("You should use the 'AppointmentDisplayOptions.ShowShadows' instead", false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool AppointmentShadows {
			get { return AppointmentDisplayOptions.ShowShadows; }
			set {
				AppointmentDisplayOptions.ShowShadows = value;
			}
		}
		#endregion
		#region ShowAllDayArea
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewShowAllDayArea"),
#endif
DefaultValue(InnerDayView.DefaultShowAllDayArea), XtraSerializableProperty()]
		public bool ShowAllDayArea {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowAllDayArea;
				} else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowAllDayArea = value;
				}
			}
		}
		#endregion
		#region ShowAllAppointmentsAtTimeCells
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewShowAllAppointmentsAtTimeCells"),
#endif
DefaultValue(InnerDayView.DefaultShowAllAppointmentsAtTimeCells), XtraSerializableProperty()]
		public bool ShowAllAppointmentsAtTimeCells {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.ShowAllAppointmentsAtTimeCells;
				} else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.ShowAllAppointmentsAtTimeCells = value;
				}
			}
		}
		#endregion
		#region VisibleTime
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewVisibleTime"),
#endif
 XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TimeOfDayInterval VisibleTime {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.VisibleTime;
				} else
					return null;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.VisibleTime = value;
				}
			}
		}
		internal bool ShouldSerializeVisibleTime() {
			return !VisibleTime.IsEqual(InnerDayView.defaultVisibleTime);
		}
		internal void ResetVisibleTime() {
			VisibleTime = InnerDayView.defaultVisibleTime;
		}
		internal bool XtraShouldSerializeVisibleTime() {
			return ShouldSerializeVisibleTime();
		}
		#endregion
		#region VisibleTimeSnapMode
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewVisibleTimeSnapMode"),
#endif
DefaultValue(InnerDayView.DefaultVisibleTimeSnapMode), XtraSerializableProperty()]
		public bool VisibleTimeSnapMode {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.VisibleTimeSnapMode;
				} else
					return false;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.VisibleTimeSnapMode = value;
				}
			}
		}
		#endregion
		#region WorkTime
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewWorkTime"),
#endif
 XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public TimeOfDayInterval WorkTime {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.WorkTime;
				} else
					return null;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.WorkTime = value;
				}
			}
		}
		internal bool ShouldSerializeWorkTime() {
			return !WorkTime.IsEqual(InnerDayView.defaultWorkTime);
		}
		internal void ResetWorkTime() {
			WorkTime = InnerDayView.defaultWorkTime;
		}
		internal bool XtraShouldSerializeWorkTime() {
			return ShouldSerializeWorkTime();
		}
		#endregion
		#region TimeScale
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewTimeScale"),
#endif
XtraSerializableProperty()]
		public TimeSpan TimeScale {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.TimeScale;
				} else
					return TimeSpan.Zero;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.TimeScale = value;
				}
			}
		}
		internal bool ShouldSerializeTimeScale() {
			return TimeScale != InnerDayView.defaultTimeScale;
		}
		internal void ResetTimeScale() {
			TimeScale = InnerDayView.defaultTimeScale;
		}
		#endregion
		#region TimeSlots
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewTimeSlots"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public TimeSlotCollection TimeSlots {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.TimeSlots;
				} else
					return null;
			}
		}
		internal bool ShouldSerializeTimeSlots() {
			return !TimeSlots.HasDefaultContent();
		}
		internal void ResetTimeSlots() {
			TimeSlots.LoadDefaults();
		}
		internal bool XtraShouldSerializeTimeSlots() {
			return ShouldSerializeTimeSlots();
		}
		internal object XtraCreateTimeSlotsItem(XtraItemEventArgs e) {
			InnerDayView innerView = (InnerDayView)InnerView;
			if (innerView != null)
				return innerView.XtraCreateTimeSlotsItem(e);
			else
				return null;
		}
		internal void XtraSetIndexTimeSlotsItem(XtraSetItemIndexEventArgs e) {
			InnerDayView innerView = (InnerDayView)InnerView;
			if (innerView != null)
				innerView.XtraSetIndexTimeSlotsItem(e);
		}
		#endregion
		#region TimeRulers
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewTimeRulers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true)]
		public TimeRulerCollection TimeRulers {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.TimeRulers;
				} else
					return null;
			}
		}
		internal object XtraCreateTimeRulersItem(XtraItemEventArgs e) {
			InnerDayView innerView = (InnerDayView)InnerView;
			if (innerView != null)
				return innerView.XtraCreateTimeRulersItem(e);
			else
				return null;
		}
		internal void XtraSetIndexTimeRulersItem(XtraSetItemIndexEventArgs e) {
			InnerDayView innerView = (InnerDayView)InnerView;
			if (innerView != null)
				innerView.XtraSetIndexTimeRulersItem(e);
		}
		#endregion
		#region TimeMarkerVisibility
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewTimeMarkerVisibility"),
#endif
		DefaultValue(InnerDayView.DefaultTimeMarkerVisibility), XtraSerializableProperty()]
		public TimeMarkerVisibility TimeMarkerVisibility {
			get {
				InnerDayView innerView = (InnerDayView)InnerView;
				if (innerView != null)
					return innerView.TimeMarkerVisibility;
				else
					return InnerDayView.DefaultTimeMarkerVisibility;
			}
			set {
				InnerDayView innerView = (InnerDayView)InnerView;
				if (innerView != null)
					innerView.TimeMarkerVisibility = value;
			}
		}
		#endregion
		protected internal TimeSpan ScrollStartTime { get { return scrollStartTime; } }
		#region TopRowTime
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan TopRowTime {
			get {
				if (ViewInfo != null)
					return ViewInfo.TopRowTime;
				else
					return ScrollStartTime;
			}
			set {
				if (value.Ticks < 0)
					Exceptions.ThrowArgumentException("TopRowTime", value);
				if (ScrollStartTime == value)
					return;
				SetScrollStartTimeCore(value);
				RaiseChanged(SchedulerControlChangeType.ScrollStartTimeChanged);
			}
		}
		#endregion
		#region StatusLineWidth
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewStatusLineWidth"),
#endif
DefaultValue(DefaultStatusLineWidth), Category(SRCategoryNames.Appearance), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int StatusLineWidth {
			get { return statusLineWidth; }
			set {
				value = Math.Max(0, value);
				if (value == statusLineWidth)
					return;
				this.statusLineWidth = value;
				RaiseChanged(SchedulerControlChangeType.StatusLineWidthChanged);
			}
		}
		#endregion
		#region RowHeight
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewRowHeight"),
#endif
DefaultValue(DefaultRowHeight), Category(SRCategoryNames.Appearance), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int RowHeight {
			get { return rowHeight; }
			set {
				value = Math.Max(0, value);
				if (value == rowHeight)
					return;
				this.rowHeight = value;
				RaiseChanged(SchedulerControlChangeType.RowHeightChanged);
			}
		}
		#endregion
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAllDayAreaScrollBarVisible"),
#endif
DefaultValue(SchedulerViewBase.defaultContainerScrollbarVisible)]
		public bool AllDayAreaScrollBarVisible {
			get { return base.ContainerScrollBarVisibility == SchedulerScrollBarVisibility.Always; }
			set { base.ContainerScrollBarVisibility = value ? SchedulerScrollBarVisibility.Always : SchedulerScrollBarVisibility.Never; }
		}
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppearance"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DayViewAppearance Appearance { get { return (DayViewAppearance)base.Appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.Day; } }
		protected internal TimeOfDayInterval ActualVisibleTime {
			get { return ((InnerDayView)InnerView).ActualVisibleTime; }
		}
		protected internal bool ActualShowAllAppointmentsAtTimeCells {
			get { return ((InnerDayView)InnerView).ActualShowAllAppointmentsAtTimeCells; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DayViewInfo ViewInfo { get { return (DayViewInfo)base.ViewInfo; } }
		#region DayCount
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewDayCount"),
#endif
DefaultValue(InnerDayView.DefaultDayCount), XtraSerializableProperty()]
		public virtual int DayCount {
			get {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					return innerView.DayCount;
				} else
					return 0;
			}
			set {
				if (InnerView != null) {
					InnerDayView innerView = (InnerDayView)InnerView;
					innerView.DayCount = value;
				}
			}
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerMenuItemId MenuItemId { get { return SchedulerMenuItemId.SwitchToDayView; } }
		[
#if !SL
	DevExpressXtraSchedulerLocalizedDescription("DayViewAppointmentDisplayOptions"),
#endif
 Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public new DayViewAppointmentDisplayOptions AppointmentDisplayOptions { get { return (DayViewAppointmentDisplayOptions)base.AppointmentDisplayOptions; } }
		#region TimeIndicatorDisplayOptions
		[
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TimeIndicatorDisplayOptions TimeIndicatorDisplayOptions {
			get {
				return timeIndicatorDisplayOptions;
			}
		}
		#endregion
		protected internal virtual bool ShowExtendedCells { get { return false; } }
		protected internal virtual int MinimumExtendedCellsInColumn { get { return 3; } }
		#endregion
		public override void RecreateViewInfo() {
			base.RecreateViewInfo();
			if (ViewInfo.PreliminaryLayoutResult != null)
				ViewInfo.PreliminaryLayoutResult.SetViewInfo(ViewInfo);
		}
		#region ShouldSerialize... and Reset... methods
		internal bool ShouldSerializeAppearance() {
			return true;
		}
		internal void ResetAppearance() {
		}
		#endregion
		protected internal override void Initialize() {
			base.Initialize();
			SubscribeTimeIndicatorDisplayOptionsEvents();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (timeIndicatorDisplayOptions != null) {
				UnsubscribeTimeIndicatorDisplayOptionsEvents();
				timeIndicatorDisplayOptions = null;
			}
		}
		#region Events
		#region TopRowTimeChanged
		ChangeEventHandler onTopRowTimeChanged;
		public event ChangeEventHandler TopRowTimeChanged {
			add { onTopRowTimeChanged += value; }
			remove { onTopRowTimeChanged -= value; }
		}
		protected internal virtual void RaiseTopRowTimeChanged(ChangeEventArgs e) {
			if (onTopRowTimeChanged != null)
				onTopRowTimeChanged(this, e);
		}
		#endregion
		#region VisibleRowCountChanged
		ChangeEventHandler onVisibleRowCountChanged;
		public event ChangeEventHandler VisibleRowCountChanged {
			add { onVisibleRowCountChanged += value; }
			remove { onVisibleRowCountChanged -= value; }
		}
		protected internal virtual void RaiseVisibleRowCountChanged(ChangeEventArgs e) {
			if (onVisibleRowCountChanged != null)
				onVisibleRowCountChanged(this, e);
		}
		#endregion
		#endregion
		#region SubscribeTimeIndicatorDisplayOptionsEvents
		protected internal virtual void SubscribeTimeIndicatorDisplayOptionsEvents() {
			timeIndicatorDisplayOptions.Changed += new BaseOptionChangedEventHandler(OnTimeIndicatorDisplayOptionsChanged);
		}
		#endregion
		#region UnsubscribeTimeIndicatorDisplayOptionsEvents
		protected internal virtual void UnsubscribeTimeIndicatorDisplayOptionsEvents() {
			timeIndicatorDisplayOptions.Changed -= new BaseOptionChangedEventHandler(OnTimeIndicatorDisplayOptionsChanged);
		}
		#endregion
		protected internal virtual void OnTimeIndicatorDisplayOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			RaiseChanged(SchedulerControlChangeType.TimeIndicatorDisplayOptionsChanged);
		}
		#endregion
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerDayView(this, new DayViewProperties());
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new DayViewDateTimeScrollController(this);
		}
		protected internal override BaseViewAppearance CreateAppearance() {
			return new DayViewAppearance();
		}
		protected internal override ViewPainterBase CreatePainter(SchedulerPaintStyle paintStyle) {
			if (paintStyle == null)
				Exceptions.ThrowArgumentException("paintStyle", paintStyle);
			return paintStyle.CreateDayViewPainter();
		}
		protected internal override SchedulerViewInfoBase CreateViewInfoCore() {
			return new DayViewInfo(this);
		}
		protected internal override void InitializeViewInfo(SchedulerViewInfoBase viewInfo) {
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new DayViewAppointmentDisplayOptions();
		}
		protected internal override ViewFactoryHelper CreateFactoryHelper() {
			return new DayViewFactoryHelper();
		}
		protected internal override bool ChangeResourceScrollBarOrientationIfNeeded(ResourceNavigator navigator) {
			bool oldVertical = navigator.Vertical;
			navigator.Vertical = false;
			return oldVertical != false;
		}
		protected internal override bool ChangeDateTimeScrollBarOrientationIfNeeded(DateTimeScrollBar scrollBar) {
			ScrollBarType oldScrollBarType = scrollBar.ScrollBarType;
			scrollBar.ScrollBarType = ScrollBarType.Vertical;
			return oldScrollBarType != ScrollBarType.Vertical;
		}
		protected internal virtual void SetScrollStartTimeCore(TimeSpan time) {
			scrollStartTime = time;
		}
		protected internal override SetSelectionCommand CreateSetSelectionCommand(InnerSchedulerControl control, TimeInterval interval, Resource resource) {
			return new DayViewSetSelectionWinCommand(control, interval, resource);
		}
		protected internal virtual VisibleRowsInfo CalculateVisibleRowsInfo() {
			VisibleRowsInfo result = new VisibleRowsInfo();
			if (ViewInfo != null) {
				result.VisibleRowCount = ViewInfo.VisibleRows.Count;
				if (result.VisibleRowCount > 0)
					result.TopRowTime = ViewInfo.VisibleRows[0].Interval.Start;
			}
			return result;
		}
		protected internal override void RecalcFinalLayout() {
			base.RecalcFinalLayout();
			RaiseTopRowTimeChangedAndVisibleRowCountChangedIfNeeded();
		}
		protected internal virtual void RaiseTopRowTimeChangedAndVisibleRowCountChangedIfNeeded() {
			VisibleRowsInfo currentVisibleRowsInfo = CalculateVisibleRowsInfo();
			if (currentVisibleRowsInfo.TopRowTime != prevVisibleRowsInfo.TopRowTime)
				RaiseTopRowTimeChanged(new ChangeEventArgs("TopRowTime", prevVisibleRowsInfo.TopRowTime, currentVisibleRowsInfo.TopRowTime));
			if (currentVisibleRowsInfo.VisibleRowCount != prevVisibleRowsInfo.VisibleRowCount)
				RaiseVisibleRowCountChanged(new ChangeEventArgs("VisibleRowCount", prevVisibleRowsInfo.VisibleRowCount, currentVisibleRowsInfo.VisibleRowCount));
			this.prevVisibleRowsInfo = currentVisibleRowsInfo;
		}
		protected override void MakeAppointmentVisibleInScrollContainers(Appointment apt) {
			ViewInfo.MakeAppointmentVisibleInScrollContainers(apt);
		}
		TimeInterval IInnerDayViewOwner.GetVisibleRowsTimeInterval() {
			return GetRowsTimeInterval(ViewInfo.VisibleRows);
		}
		TimeInterval IInnerDayViewOwner.GetAvailableRowsTimeInterval() {
			return GetRowsTimeInterval(ViewInfo.Rows);
		}
		TimeSpan IInnerDayViewOwner.GetTopRowTime() {
			return TopRowTime;
		}
		void IInnerDayViewOwner.SetScrollStartTimeCore(TimeSpan value) {
			SetScrollStartTimeCore(value);
		}
		protected internal virtual TimeInterval GetRowsTimeInterval(DayViewRowCollection rows) {
			int count = rows.Count;
			if (count <= 0)
				return TimeInterval.Empty;
			return new TimeInterval(new DateTime(rows[0].Interval.Start.Ticks), new DateTime(rows[count - 1].Interval.End.Ticks));
		}
	}
}
namespace DevExpress.XtraScheduler.Drawing {
	#region VisibleRowsInfo
	public class VisibleRowsInfo {
		#region Fields
		TimeSpan topRowTime = TimeSpan.MaxValue;
		int visibleRowCount = int.MinValue;
		#endregion
		#region Properties
		public TimeSpan TopRowTime { get { return topRowTime; } set { topRowTime = value; } }
		public int VisibleRowCount { get { return visibleRowCount; } set { visibleRowCount = value; } }
		#endregion
	}
	#endregion
	#region DayViewRowStartDateComparer
	public class DayViewRowStartDateComparer : IComparer<DayViewRow>, IComparer {
		int IComparer.Compare(object x, object y) {
			DayViewRow rowX = (DayViewRow)x;
			DayViewRow rowY = (DayViewRow)y;
			return CompareCore(rowX, rowY);
		}
		public int Compare(DayViewRow x, DayViewRow y) {
			return CompareCore(x, y);
		}
		static int CompareCore(DayViewRow rowX, DayViewRow rowY) {
			return Comparer.Default.Compare(rowX.Interval.Start, rowY.Interval.Start);
		}
	}
	#endregion
	#region DayViewRowEndDateComparer
	public class DayViewRowEndDateComparer : IComparer<DayViewRow>, IComparer {
		int IComparer.Compare(object x, object y) {
			DayViewRow rowX = (DayViewRow)x;
			DayViewRow rowY = (DayViewRow)y;
			return CompareCore(rowX, rowY);
		}
		public int Compare(DayViewRow x, DayViewRow y) {
			return CompareCore(x, y);
		}
		static int CompareCore(DayViewRow rowX, DayViewRow rowY) {
			return Comparer.Default.Compare(rowX.Interval.End, rowY.Interval.End);
		}
	}
	#endregion
	#region DayViewRowCollection
	public class DayViewRowCollection : List<DayViewRow> {
		internal static DayViewRowStartDateComparer startComparer = new DayViewRowStartDateComparer();
		internal static DayViewRowEndDateComparer endComparer = new DayViewRowEndDateComparer();
		public int BinarySearchStartDate(TimeSpan time) {
			return BinarySearchStartDate(0, Count, time);
		}
		public int BinarySearchEndDate(TimeSpan time) {
			return BinarySearchEndDate(0, Count, time);
		}
		public int BinarySearchStartDate(int index, int count, TimeSpan time) {
			DayViewRow row = new DayViewRow(new TimeOfDayInterval(time, time));
			return BinarySearch(index, count, row, startComparer);
		}
		public int BinarySearchEndDate(int index, int count, TimeSpan time) {
			DayViewRow row = new DayViewRow(new TimeOfDayInterval(time, time));
			return BinarySearch(index, count, row, endComparer);
		}
	}
	#endregion
	public interface ISupportTimeRuler {
		TimeZoneHelper TimeZoneHelper { get; }
		DayViewAppearance PaintAppearance { get; }
		ITimeRulerFormatStringService GetFormatStringProvider();
		TimeSpan TimeScale { get; }
	}
	public class AllDayAreaScrollContainer : SchedulerViewCellContainer {
		static readonly AllDayAreaScrollContainer empty = new AllDayAreaScrollContainer();
		public static new AllDayAreaScrollContainer Empty { get { return empty; } }
		public AllDayAreaScrollContainer()
			: base(null) {
		}
		protected internal override SchedulerViewCellBase CreateCellInstance() {
			return null;
		}
	}
	public class ExtendedCell : TimeCell {
		string text;
		public ExtendedCell()
			: this(null, String.Empty, TimeInterval.Empty) {
		}
		public ExtendedCell(Appointment apt, string text, TimeInterval interval) {
			Appointment = apt;
			this.text = text;
			this.Interval = interval;
		}
#if !SL
	[DevExpressXtraSchedulerLocalizedDescription("ExtendedCellText")]
#endif
		public string Text {
			get { return text; }
			set { text = value; }
		}
		public Appointment Appointment { get; private set; }
	}
	public class ExtendedCellsComparer : IComparer<ITimeCell> {
		#region IComparer<ITimeCell> Members
		public int Compare(ITimeCell x, ITimeCell y) {
			int compareStart = Comparer.Default.Compare(x.Interval.Start, y.Interval.Start);
			if (compareStart != 0)
				return compareStart;
			else
				return Comparer.Default.Compare(x.Interval.End, y.Interval.End);
		}
		#endregion
	}
}
