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
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraScheduler.Drawing;
#if SL
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.XtraScheduler {
	#region RemindersFormDefaultAction
	public enum RemindersFormDefaultAction {
		DismissAll = 0,
		SnoozeAll = 1,
		Custom = 2
	}
	#endregion
	#region AppointmentStatusDisplayType
	public enum AppointmentStatusDisplayType {
		Never = 0,
		Time = 1,
		Bounds = 2,
	}
	#endregion
	#region AppointmentContinueArrowDisplayType
	public enum AppointmentContinueArrowDisplayType {
		Never = 0,
		Arrow = 1,
		ArrowWithText = 2,
		Auto = 3,
	}
	#endregion
	#region AppointmentSnapToCellMode
	public enum AppointmentSnapToCellsMode {
		Auto = 0,
		Always = 1,
		Never = 2,
		Disabled = 3
	}
	#endregion
	#region UpdateSelectionDurationAction
	public enum UpdateSelectionDurationAction {
		Adjust,
		Reset
	}
	#endregion
	#region PercentCompleteDisplayType
	public enum PercentCompleteDisplayType {
		BarProgress = 0,
		Number = 1,
		Both = 2,
		None = 3
	}
	#endregion
	#region SchedulerNotificationOptions (abstract class)
	public abstract class SchedulerNotificationOptions : BaseOptions {
		protected SchedulerNotificationOptions() {
			Reset();
		}
		#region Events
		protected internal event BaseOptionChangedEventHandler Changed { add { ChangedCore += value; } remove { ChangedCore -= value; } }
		#endregion
		public override void Reset() {
			BeginUpdate();
			try {
				ResetCore();
			} finally {
				EndUpdate();
			}
		}
		protected internal abstract void ResetCore();
	}
	#endregion
	#region AppointmentDisplayOptions
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class AppointmentDisplayOptions : SchedulerNotificationOptions {
		#region Fields
		const AppointmentSnapToCellsMode defaultSnapToCellsMode = AppointmentSnapToCellsMode.Always;
		const int defaultAppointmentHeight = 0;
		const bool defaultAppointmentAutoHeight = false;
		const ViewInfoItemAlignment defaultStatusAlignment = ViewInfoItemAlignment.Top;
		internal const int defaultAppointmentInterspacing = 2;
		AppointmentTimeDisplayType timeDisplayType;
		AppointmentTimeVisibility startTimeVisibility;
		AppointmentTimeVisibility endTimeVisibility;
		bool showRecurrence;
		bool showReminder;
		int appointmentHeight;
		bool appointmentAutoHeight;
		AppointmentSnapToCellsMode snapToCellsMode;
		AppointmentStatusDisplayType statusDisplayType;
		ViewInfoItemAlignment statusAlignment;
		AppointmentContinueArrowDisplayType continueArrowDisplayType;
		int innerAppointmentVerticalInterspacing = defaultAppointmentInterspacing;
		#endregion
		#region Properties
		#region TimeDisplayType
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsTimeDisplayType"),
#endif
 XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public AppointmentTimeDisplayType TimeDisplayType {
			get { return timeDisplayType; }
			set {
				AppointmentTimeDisplayType oldVal = timeDisplayType;
				if (timeDisplayType == value)
					return;
				timeDisplayType = value;
				OnChanged(new BaseOptionChangedEventArgs("TimeDisplayType", oldVal, timeDisplayType));
			}
		}
		#endregion
		#region StartTimeVisibility
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsStartTimeVisibility"),
#endif
DefaultValue(AppointmentTimeVisibility.Auto), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public AppointmentTimeVisibility StartTimeVisibility {
			get { return startTimeVisibility; }
			set {
				AppointmentTimeVisibility oldVal = startTimeVisibility;
				if (startTimeVisibility == value)
					return;
				startTimeVisibility = value;
				OnChanged(new BaseOptionChangedEventArgs("StartTimeVisibility", oldVal, startTimeVisibility));
			}
		}
		#endregion
		#region EndTimeVisibility
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsEndTimeVisibility"),
#endif
DefaultValue(AppointmentTimeVisibility.Auto), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public AppointmentTimeVisibility EndTimeVisibility {
			get { return endTimeVisibility; }
			set {
				AppointmentTimeVisibility oldVal = endTimeVisibility;
				if (endTimeVisibility == value)
					return;
				endTimeVisibility = value;
				OnChanged(new BaseOptionChangedEventArgs("EndTimeVisibility", oldVal, endTimeVisibility));
			}
		}
		#endregion
		#region ShowRecurrence
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsShowRecurrence"),
#endif
XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowRecurrence {
			get { return showRecurrence; }
			set {
				if (showRecurrence == value)
					return;
				showRecurrence = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowRecurrence", !showRecurrence, showRecurrence));
			}
		}
		internal virtual bool DefaultShowRecurrence { get { return true; } }
		protected internal virtual AppointmentTimeDisplayType DefaultTimeDisplayType { get { return AppointmentTimeDisplayType.Auto; } }
		protected internal virtual bool ShouldSerializeShowRecurrence() {
			return showRecurrence != DefaultShowRecurrence;
		}
		protected internal virtual void ResetShowRecurrence() {
			ShowRecurrence = DefaultShowRecurrence;
		}
		#endregion
		protected internal virtual bool ShouldSerializeTimeDisplayType() {
			return timeDisplayType != DefaultTimeDisplayType;
		}
		protected internal virtual void ResetTimeDisplayType() {
			TimeDisplayType = DefaultTimeDisplayType;
		}
		#region ShowReminder
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsShowReminder"),
#endif
XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowReminder {
			get { return showReminder; }
			set {
				if (showReminder == value)
					return;
				showReminder = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowReminder", !showReminder, showReminder));
			}
		}
		internal virtual bool DefaultShowReminder { get { return true; } }
		protected internal virtual bool ShouldSerializeShowReminder() {
			return showReminder != DefaultShowReminder;
		}
		protected internal virtual void ResetShowReminder() {
			ShowReminder = DefaultShowReminder;
		}
		#endregion
		#region AppointmentHeight
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsAppointmentHeight"),
#endif
DefaultValue(defaultAppointmentHeight), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public int AppointmentHeight {
			get { return appointmentHeight; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("AppointmentHeight", value);
				int oldValue = appointmentHeight;
				if (appointmentHeight == value)
					return;
				appointmentHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("AppointmentHeight", oldValue, appointmentHeight));
			}
		}
		#endregion
		#region AppointmentAutoHeight
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsAppointmentAutoHeight"),
#endif
DefaultValue(defaultAppointmentAutoHeight), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public bool AppointmentAutoHeight {
			get { return appointmentAutoHeight; }
			set {
				if (appointmentAutoHeight == value)
					return;
				appointmentAutoHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("AppointmentAutoHeight", !appointmentAutoHeight, appointmentAutoHeight));
			}
		}
		#endregion
		#region SnapToCellsMode
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsSnapToCellsMode"),
#endif
XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual AppointmentSnapToCellsMode SnapToCellsMode {
			get { return snapToCellsMode; }
			set {
				if (snapToCellsMode == value)
					return;
				AppointmentSnapToCellsMode oldValue = snapToCellsMode;
				snapToCellsMode = value;
				OnChanged(new BaseOptionChangedEventArgs("SnapToCellsMode", oldValue, snapToCellsMode));
			}
		}
		protected internal virtual bool ShouldSerializeSnapToCellsMode() {
			return snapToCellsMode != defaultSnapToCellsMode;
		}
		protected internal virtual void ResetSnapToCellsMode() {
			SnapToCellsMode = defaultSnapToCellsMode;
		}
		#endregion
		#region SnapToCells
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsSnapToCells"),
#endif
 XtraSerializableProperty(XtraSerializationFlags.None), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable(),
		Obsolete("Please, use SnapToCellsMode property instead.", false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool SnapToCells {
			get { return SnapToCellsMode == AppointmentSnapToCellsMode.Always; }
			set {
				if (value == true)
					SnapToCellsMode = AppointmentSnapToCellsMode.Always;
				else
					SnapToCellsMode = AppointmentSnapToCellsMode.Auto;
			}
		}
		protected internal virtual bool ShouldSerializeSnapToCells() {
			return false;
		}
		#endregion
		#region StatusAlignment
		[DefaultValue(defaultStatusAlignment), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		protected internal virtual ViewInfoItemAlignment StatusAlignment {
			get { return statusAlignment; }
			set {
				ViewInfoItemAlignment oldVal = statusAlignment;
				if (statusAlignment == value)
					return;
				statusAlignment = value;
				OnChanged(new BaseOptionChangedEventArgs("StatusAlignment", oldVal, statusAlignment));
			}
		}
		#endregion
		#region StatusDisplayType
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsStatusDisplayType"),
#endif
XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual AppointmentStatusDisplayType StatusDisplayType {
			get { return statusDisplayType; }
			set {
				AppointmentStatusDisplayType oldVal = statusDisplayType;
				if (statusDisplayType == value)
					return;
				statusDisplayType = value;
				OnChanged(new BaseOptionChangedEventArgs("StatusDisplayType", oldVal, statusDisplayType));
			}
		}
		protected internal virtual bool ShouldSerializeStatusDisplayType() {
			return statusDisplayType != DefaultStatusDisplayType;
		}
		protected internal virtual void ResetStatusDisplayType() {
			StatusDisplayType = DefaultStatusDisplayType;
		}
		internal virtual AppointmentStatusDisplayType DefaultStatusDisplayType { get { return AppointmentStatusDisplayType.Never; } }
		#endregion
		#region ContinueArrowDisplayType
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDisplayOptionsContinueArrowDisplayType"),
#endif
XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual AppointmentContinueArrowDisplayType ContinueArrowDisplayType {
			get { return continueArrowDisplayType; }
			set {
				AppointmentContinueArrowDisplayType oldVal = continueArrowDisplayType;
				if (continueArrowDisplayType == value)
					return;
				continueArrowDisplayType = value;
				OnChanged(new BaseOptionChangedEventArgs("ContinueArrowDisplayType", oldVal, continueArrowDisplayType));
			}
		}
		internal virtual AppointmentContinueArrowDisplayType DefaultContinueArrowDisplayType { get { return AppointmentContinueArrowDisplayType.Auto; } }
		protected internal virtual bool ShouldSerializeContinueArrowDisplayType() {
			return continueArrowDisplayType != DefaultContinueArrowDisplayType;
		}
		protected internal virtual void ResetContinueArrowDisplayType() {
			ContinueArrowDisplayType = DefaultContinueArrowDisplayType;
		}
		#endregion
		#region InnerAppointmentVerticalInterspacing
		internal virtual int InnerAppointmentVerticalInterspacing {
			get { return innerAppointmentVerticalInterspacing; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("AppointmentInterspacing", value);
				int oldValue = innerAppointmentVerticalInterspacing;
				if (oldValue == value)
					return;
				innerAppointmentVerticalInterspacing = value;
				OnChanged(new BaseOptionChangedEventArgs("InnerAppointmentVerticalInterspacing", oldValue, value));
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			StartTimeVisibility = AppointmentTimeVisibility.Auto;
			EndTimeVisibility = AppointmentTimeVisibility.Auto;
			AppointmentHeight = defaultAppointmentHeight;
			AppointmentAutoHeight = defaultAppointmentAutoHeight;
			StatusAlignment = defaultStatusAlignment;
			ResetTimeDisplayType();
			ResetSnapToCellsMode();
			ResetShowRecurrence();
			ResetShowReminder();
			ResetStatusDisplayType();
			ResetContinueArrowDisplayType();
			InnerAppointmentVerticalInterspacing = defaultAppointmentInterspacing;
		}
		public override string ToString() {
			return String.Empty;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				AppointmentDisplayOptions viewOptions = options as AppointmentDisplayOptions;
				if (viewOptions != null) {
					TimeDisplayType = viewOptions.TimeDisplayType;
					StartTimeVisibility = viewOptions.StartTimeVisibility;
					EndTimeVisibility = viewOptions.EndTimeVisibility;
					ShowRecurrence = viewOptions.ShowRecurrence;
					ShowReminder = viewOptions.ShowReminder;
					AppointmentHeight = viewOptions.AppointmentHeight;
					AppointmentAutoHeight = viewOptions.AppointmentAutoHeight;
					SnapToCellsMode = viewOptions.SnapToCellsMode;
					StatusDisplayType = viewOptions.StatusDisplayType;
					StatusAlignment = viewOptions.StatusAlignment;
					InnerAppointmentVerticalInterspacing = viewOptions.InnerAppointmentVerticalInterspacing;
					ContinueArrowDisplayType = viewOptions.ContinueArrowDisplayType;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	#endregion
	#region CellAutoHeightOptions
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class CellsAutoHeightOptions : SchedulerNotificationOptions {
		bool enabled;
		int minHeight;
		#region Enabled
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("CellsAutoHeightOptionsEnabled"),
#endif
 DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public bool Enabled {
			get {
				return enabled;
			}
			set {
				if (enabled == value)
					return;
				bool oldValue = enabled;
				enabled = value;
				OnChanged(new BaseOptionChangedEventArgs("Enabled", oldValue, enabled));
			}
		}
		#endregion
		#region MinHeight
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("CellsAutoHeightOptionsMinHeight"),
#endif
 DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatEnable()]
		public int MinHeight {
			get { return minHeight; }
			set {
				int oldValue = MinHeight;
				if (oldValue == value)
					return;
				minHeight = value;
				OnChanged(new BaseOptionChangedEventArgs("MinHeight", oldValue, value));
			}
		}
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			CellsAutoHeightOptions optionsCellAutoHeight = options as CellsAutoHeightOptions;
			if (optionsCellAutoHeight != null) {
				BeginUpdate();
				try {
					Enabled = optionsCellAutoHeight.Enabled;
					MinHeight = optionsCellAutoHeight.MinHeight;
				} finally {
					EndUpdate();
				}
			}
		}
		protected internal override void ResetCore() {
			Enabled = false;
			MinHeight = 0;
		}
	}
	#endregion
	#region TimeIndicatorDisplayOptionsBase
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class TimeIndicatorDisplayOptionsBase : SchedulerNotificationOptions {
		TimeIndicatorVisibility visibility;
		#region Visibility
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimeIndicatorDisplayOptionsBaseVisibility"),
#endif
 DefaultValue(TimeIndicatorVisibility.TodayView), XtraSerializableProperty()]
		public TimeIndicatorVisibility Visibility {
			get { return visibility; }
			set {
				TimeIndicatorVisibility oldValue = visibility;
				if (oldValue == value)
					return;
				visibility = value;
				OnChanged(new BaseOptionChangedEventArgs("Visibility", oldValue, visibility));
			}
		}
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			TimeIndicatorDisplayOptionsBase optionsTimeIndicator = options as TimeIndicatorDisplayOptionsBase;
			if (optionsTimeIndicator != null) {
				BeginUpdate();
				try {
					Visibility = optionsTimeIndicator.Visibility;
				} finally {
					EndUpdate();
				}
			}
		}
		protected internal override void ResetCore() {
			Visibility = TimeIndicatorVisibility.TodayView;
		}
	}
	#endregion
	#region DayViewAppointmentDisplayOptions
	public class DayViewAppointmentDisplayOptions : AppointmentDisplayOptions {
		#region Fields
		const bool defaultShowShadows = true;
		const AppointmentStatusDisplayType defaultAllDayAppointmentsStatusDisplayType = AppointmentStatusDisplayType.Never;
		bool showShadows;
		AppointmentStatusDisplayType allDayAppointmentsStatusDisplayType;
		SchedulerColumnPadding columnPadding = new SchedulerColumnPadding();
		#endregion
		public DayViewAppointmentDisplayOptions() {
			this.columnPadding.Changed += new BaseOptionChangedEventHandler(OnColumnPaddingChanged);
		}
		#region Properties
		#region DefaultStatusDisplayType
		internal override AppointmentStatusDisplayType DefaultStatusDisplayType { get { return AppointmentStatusDisplayType.Time; } }
		#endregion
		#region ShowShadows
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("DayViewAppointmentDisplayOptionsShowShadows"),
#endif
DefaultValue(defaultShowShadows), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public bool ShowShadows {
			get { return showShadows; }
			set {
				if (showShadows == value)
					return;
				showShadows = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowShadows", !showShadows, showShadows));
			}
		}
		#endregion
		#region AllDayAppointmentsStatusDisplayType
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("DayViewAppointmentDisplayOptionsAllDayAppointmentsStatusDisplayType"),
#endif
		DefaultValue(defaultAllDayAppointmentsStatusDisplayType), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual AppointmentStatusDisplayType AllDayAppointmentsStatusDisplayType {
			get { return allDayAppointmentsStatusDisplayType; }
			set {
				AppointmentStatusDisplayType oldVal = allDayAppointmentsStatusDisplayType;
				if (allDayAppointmentsStatusDisplayType == value)
					return;
				allDayAppointmentsStatusDisplayType = value;
				OnChanged(new BaseOptionChangedEventArgs("AllDayAppointmentsStatusDisplayType", oldVal, allDayAppointmentsStatusDisplayType));
			}
		}
		#endregion
		#region ColumnPadding
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("DayViewAppointmentDisplayOptionsColumnPadding"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true)]
		public SchedulerColumnPadding ColumnPadding { get { return columnPadding; } }
		#endregion
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			AllDayAppointmentsStatusDisplayType = defaultAllDayAppointmentsStatusDisplayType;
			ShowShadows = defaultShowShadows;
			ColumnPadding.Reset();
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				DayViewAppointmentDisplayOptions viewOptions = options as DayViewAppointmentDisplayOptions;
				if (viewOptions != null) {
					ShowShadows = viewOptions.ShowShadows;
					AllDayAppointmentsStatusDisplayType = viewOptions.AllDayAppointmentsStatusDisplayType;
					ColumnPadding.Assign(viewOptions.ColumnPadding);
					ContinueArrowDisplayType = viewOptions.ContinueArrowDisplayType;
				}
			} finally {
				EndUpdate();
			}
		}
		void OnColumnPaddingChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
	}
	#endregion
	#region WeekViewAppointmentDisplayOptions
	public class WeekViewAppointmentDisplayOptions : AppointmentDisplayOptions {
		#region Fields
		const bool defaultShowBordersForSameDayAppointments = false;
		bool showBordersForSameDayAppointments = defaultShowBordersForSameDayAppointments;
		#endregion
		#region Properties
		#region SnapToCellsMode
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Category(SRCategoryNames.Appearance), AutoFormatEnable()]
		public override AppointmentSnapToCellsMode SnapToCellsMode {
			get { return AppointmentSnapToCellsMode.Always; }
			set { }
		}
		protected internal override bool ShouldSerializeSnapToCellsMode() {
			return false;
		}
		protected internal override void ResetSnapToCellsMode() {
			base.SnapToCellsMode = AppointmentSnapToCellsMode.Always;
		}
		protected internal AppointmentSnapToCellsMode BaseSnapToCellsMode { get { return base.SnapToCellsMode; } set { base.SnapToCellsMode = value; } }
		#endregion
		#region ShowBordersForSameDayAppointments
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("WeekViewAppointmentDisplayOptionsShowBordersForSameDayAppointments"),
#endif
DefaultValue(defaultShowBordersForSameDayAppointments), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual bool ShowBordersForSameDayAppointments {
			get { return showBordersForSameDayAppointments; }
			set {
				if (showBordersForSameDayAppointments == value)
					return;
				showBordersForSameDayAppointments = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowBordersForSameDayAppointments", !showBordersForSameDayAppointments, showBordersForSameDayAppointments));
			}
		}
		#endregion
		#region AppointmentInterspacing
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("WeekViewAppointmentDisplayOptionsAppointmentInterspacing"),
#endif
DefaultValue(AppointmentDisplayOptions.defaultAppointmentInterspacing), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual int AppointmentInterspacing {
			get { return InnerAppointmentVerticalInterspacing; }
			set { InnerAppointmentVerticalInterspacing = value; }
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			ShowBordersForSameDayAppointments = defaultShowBordersForSameDayAppointments;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				WeekViewAppointmentDisplayOptions viewOptions = options as WeekViewAppointmentDisplayOptions;
				if (viewOptions == null)
					return;
				this.ShowBordersForSameDayAppointments = viewOptions.ShowBordersForSameDayAppointments;
			} finally {
				EndUpdate();
			}
		}
	}
	#endregion
	#region MonthViewAppointmentDisplayOptions
	public class MonthViewAppointmentDisplayOptions : WeekViewAppointmentDisplayOptions {
		internal override bool DefaultShowRecurrence { get { return false; } }
		internal override bool DefaultShowReminder { get { return false; } }
	}
	#endregion
	#region TimelineAppointmentDisplayOptions
	public class TimelineViewAppointmentDisplayOptions : AppointmentDisplayOptions {
		#region Properties
		#region AppointmentInterspacing
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("TimelineViewAppointmentDisplayOptionsAppointmentInterspacing"),
#endif
DefaultValue(AppointmentDisplayOptions.defaultAppointmentInterspacing), XtraSerializableProperty(), Category(SRCategoryNames.Appearance), NotifyParentProperty(true), AutoFormatEnable()]
		public virtual int AppointmentInterspacing {
			get { return InnerAppointmentVerticalInterspacing; }
			set { InnerAppointmentVerticalInterspacing = value; }
		}
		#endregion
		#endregion
	}
	#endregion
	#region GanttViewAppointmentDisplayOptions
	public class GanttViewAppointmentDisplayOptions : TimelineViewAppointmentDisplayOptions {
		#region Fields
		const PercentCompleteDisplayType defaultPercentCompleteDisplayType = PercentCompleteDisplayType.Both;
		PercentCompleteDisplayType percentCompleteDisplayType;
		#endregion
		#region Properties
		#region PercentCompleteDisplayType
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("GanttViewAppointmentDisplayOptionsPercentCompleteDisplayType"),
#endif
DefaultValue(defaultPercentCompleteDisplayType), XtraSerializableProperty(), NotifyParentProperty(true)]
		public PercentCompleteDisplayType PercentCompleteDisplayType {
			get { return percentCompleteDisplayType; }
			set {
				PercentCompleteDisplayType oldValue = percentCompleteDisplayType;
				if (oldValue == value)
					return;
				percentCompleteDisplayType = value;
				OnChanged(new BaseOptionChangedEventArgs("PercentCompleteDisplayType", oldValue, value));
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			PercentCompleteDisplayType = defaultPercentCompleteDisplayType;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				GanttViewAppointmentDisplayOptions viewOptions = options as GanttViewAppointmentDisplayOptions;
				if (viewOptions == null)
					return;
				PercentCompleteDisplayType = viewOptions.PercentCompleteDisplayType;
			} finally {
				EndUpdate();
			}
		}
	}
	#endregion
	#region UsedAppointmentType
	public enum UsedAppointmentType {
		None = 0,
		Recurring = 1,
		NonRecurring = 2,
		All = 4,
		Custom = 5
	}
	#endregion
	#region AllowDisplayAppointmentForm
	public enum AllowDisplayAppointmentForm {
		Auto,
		Never,
		Always
	}
	#endregion
	#region AppointmentConflictsMode
	public enum AppointmentConflictsMode {
		Allowed = 0,
		Forbidden = 1,
		Custom = 2,
	}
	#endregion
	#region AllowDisplayAppointmentDependencyForm
	public enum AllowDisplayAppointmentDependencyForm {
		Auto,
		Never,
		Always
	}
	#endregion
	#region SchedulerOptionsCustomization
	public class SchedulerOptionsCustomization : SchedulerNotificationOptions {
		#region Fields
		UsedAppointmentType allowAppointmentDrag;
		UsedAppointmentType allowAppointmentResize;
		UsedAppointmentType allowAppointmentDelete;
		UsedAppointmentType allowAppointmentCopy;
		UsedAppointmentType allowAppointmentCreate;
		UsedAppointmentType allowAppointmentEdit;
		UsedAppointmentType allowInplaceEditor;
		UsedAppointmentType allowAppointmentDragBetweenResources;
		bool allowAppointmentMultiSelect;
		AppointmentConflictsMode allowAppointmentConflicts;
		AllowDisplayAppointmentForm allowDisplayAppointmentForm;
		AllowDisplayAppointmentDependencyForm allowDisplayAppointmentDependencyForm;
		#endregion
		#region Properties
		#region AllowAppointmentCopy
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentCopy"),
#endif
DefaultValue(UsedAppointmentType.All), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public UsedAppointmentType AllowAppointmentCopy {
			get { return allowAppointmentCopy; }
			set {
				UsedAppointmentType oldValue = allowAppointmentCopy;
				if (oldValue == value)
					return;
				allowAppointmentCopy = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentCopy", oldValue, value));
			}
		}
		#endregion
		#region AllowAppointmentCreate
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentCreate"),
#endif
DefaultValue(UsedAppointmentType.All), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public UsedAppointmentType AllowAppointmentCreate {
			get { return allowAppointmentCreate; }
			set {
				UsedAppointmentType oldValue = allowAppointmentCreate;
				if (oldValue == value)
					return;
				allowAppointmentCreate = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentCreate", oldValue, value));
			}
		}
		#endregion
		#region AllowAppointmentDelete
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentDelete"),
#endif
DefaultValue(UsedAppointmentType.All), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public UsedAppointmentType AllowAppointmentDelete {
			get { return allowAppointmentDelete; }
			set {
				UsedAppointmentType oldValue = allowAppointmentDelete;
				if (oldValue == value)
					return;
				allowAppointmentDelete = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentDelete", oldValue, value));
			}
		}
		#endregion
		#region AllowAppointmentDrag
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentDrag"),
#endif
DefaultValue(UsedAppointmentType.All), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public UsedAppointmentType AllowAppointmentDrag {
			get { return allowAppointmentDrag; }
			set {
				UsedAppointmentType oldValue = allowAppointmentDrag;
				if (oldValue == value)
					return;
				allowAppointmentDrag = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentDrag", oldValue, value));
			}
		}
		#endregion
		#region AllowAppointmentEdit
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentEdit"),
#endif
DefaultValue(UsedAppointmentType.All), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public UsedAppointmentType AllowAppointmentEdit {
			get { return allowAppointmentEdit; }
			set {
				UsedAppointmentType oldValue = allowAppointmentEdit;
				if (oldValue == value)
					return;
				allowAppointmentEdit = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentEdit", oldValue, value));
			}
		}
		#endregion
		#region AllowAppointmentResize
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentResize"),
#endif
DefaultValue(UsedAppointmentType.All), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public UsedAppointmentType AllowAppointmentResize {
			get { return allowAppointmentResize; }
			set {
				UsedAppointmentType oldValue = allowAppointmentResize;
				if (oldValue == value)
					return;
				allowAppointmentResize = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentResize", oldValue, value));
			}
		}
		#endregion
		#region AllowAppointmentDragBetweenResources
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentDragBetweenResources"),
#endif
DefaultValue(UsedAppointmentType.All), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public UsedAppointmentType AllowAppointmentDragBetweenResources {
			get { return allowAppointmentDragBetweenResources; }
			set {
				UsedAppointmentType oldValue = allowAppointmentDragBetweenResources;
				if (oldValue == value)
					return;
				allowAppointmentDragBetweenResources = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentDragBetweenResources", oldValue, value));
			}
		}
		#endregion
		#region AllowInplaceEditor
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowInplaceEditor"),
#endif
DefaultValue(UsedAppointmentType.All), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public UsedAppointmentType AllowInplaceEditor {
			get { return allowInplaceEditor; }
			set {
				UsedAppointmentType oldValue = allowInplaceEditor;
				if (oldValue == value)
					return;
				allowInplaceEditor = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowInplaceEditor", oldValue, value));
			}
		}
		#endregion
		#region AllowAppointmentMultiSelect
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentMultiSelect"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public bool AllowAppointmentMultiSelect {
			get { return allowAppointmentMultiSelect; }
			set {
				if (allowAppointmentMultiSelect == value)
					return;
				allowAppointmentMultiSelect = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentMultiSelect", !value, value));
			}
		}
		#endregion
		#region AllowAppointmentConflicts
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowAppointmentConflicts"),
#endif
DefaultValue(AppointmentConflictsMode.Allowed), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public AppointmentConflictsMode AllowAppointmentConflicts {
			get { return allowAppointmentConflicts; }
			set {
				AppointmentConflictsMode oldValue = allowAppointmentConflicts;
				if (oldValue == value)
					return;
				allowAppointmentConflicts = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowAppointmentConflicts", oldValue, value));
			}
		}
		#endregion
		#region AllowDisplayAppointmentForm
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowDisplayAppointmentForm"),
#endif
DefaultValue(AllowDisplayAppointmentForm.Auto), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public AllowDisplayAppointmentForm AllowDisplayAppointmentForm {
			get { return allowDisplayAppointmentForm; }
			set {
				AllowDisplayAppointmentForm oldValue = allowDisplayAppointmentForm;
				if (oldValue == value)
					return;
				allowDisplayAppointmentForm = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDisplayAppointmentForm", oldValue, value));
			}
		}
		#endregion
		#region AllowDisplayAppointmentDependencyForm (hidden)
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsCustomizationAllowDisplayAppointmentDependencyForm"),
#endif
DefaultValue(AllowDisplayAppointmentDependencyForm.Auto), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public AllowDisplayAppointmentDependencyForm AllowDisplayAppointmentDependencyForm {
			get { return allowDisplayAppointmentDependencyForm; }
			set {
				AllowDisplayAppointmentDependencyForm oldValue = allowDisplayAppointmentDependencyForm;
				if (oldValue == value)
					return;
				allowDisplayAppointmentDependencyForm = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDisplayAppointmentDependencyForm", oldValue, value));
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			AllowAppointmentCopy = UsedAppointmentType.All;
			AllowAppointmentCreate = UsedAppointmentType.All;
			AllowAppointmentDelete = UsedAppointmentType.All;
			AllowAppointmentDrag = UsedAppointmentType.All;
			AllowAppointmentEdit = UsedAppointmentType.All;
			AllowAppointmentResize = UsedAppointmentType.All;
			AllowInplaceEditor = UsedAppointmentType.All;
			AllowAppointmentDragBetweenResources = UsedAppointmentType.All;
			AllowAppointmentMultiSelect = true;
			AllowAppointmentConflicts = AppointmentConflictsMode.Allowed;
			AllowDisplayAppointmentForm = AllowDisplayAppointmentForm.Auto;
			AllowDisplayAppointmentDependencyForm = AllowDisplayAppointmentDependencyForm.Auto;
		}
		public override string ToString() {
			return String.Empty;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			SchedulerOptionsCustomization optionsCustomization = options as SchedulerOptionsCustomization;
			if (optionsCustomization != null) {
				BeginUpdate();
				try {
					AllowAppointmentCopy = optionsCustomization.AllowAppointmentCopy;
					AllowAppointmentCreate = optionsCustomization.AllowAppointmentCreate;
					AllowAppointmentDelete = optionsCustomization.AllowAppointmentDelete;
					AllowAppointmentDrag = optionsCustomization.AllowAppointmentDrag;
					AllowAppointmentEdit = optionsCustomization.AllowAppointmentEdit;
					AllowAppointmentResize = optionsCustomization.AllowAppointmentResize;
					AllowAppointmentDragBetweenResources = optionsCustomization.AllowAppointmentDragBetweenResources;
					AllowInplaceEditor = optionsCustomization.AllowInplaceEditor;
					AllowAppointmentMultiSelect = optionsCustomization.AllowAppointmentMultiSelect;
					AllowAppointmentConflicts = optionsCustomization.AllowAppointmentConflicts;
					AllowDisplayAppointmentForm = optionsCustomization.AllowDisplayAppointmentForm;
					AllowDisplayAppointmentDependencyForm = optionsCustomization.AllowDisplayAppointmentDependencyForm;
				} finally {
					EndUpdate();
				}
			}
		}
	}
	#endregion
	#region SchedulerOptionsBehaviorBase (abstract class)
	public abstract class SchedulerOptionsBehaviorBase : SchedulerNotificationOptions {
		internal const MouseWheelScrollAction DefaultMouseWheelScrollAction = MouseWheelScrollAction.Time;
		internal const bool DefaultTouchAllowed = true;
		#region Fields
		const RecurrentAppointmentAction defaultRecurrentAppointmentEditAction = RecurrentAppointmentAction.Occurrence;
		const RecurrentAppointmentAction defaultRecurrentAppointmentDeleteAction = RecurrentAppointmentAction.Ask;
		const RemindersFormDefaultAction defaultRemindersFormDefaultAction = RemindersFormDefaultAction.DismissAll;
		RecurrentAppointmentAction recurrentAppointmentEditAction;
		RecurrentAppointmentAction recurrentAppointmentDeleteAction;
		string clientTimeZoneId;
		bool showRemindersForm;
		RemindersFormDefaultAction remindersFormDefaultAction;
		bool selectOnRightClick;
		TimeZoneHelper timeZoneEngine;
		MouseWheelScrollAction innerMouseWheelScrollAction = DefaultMouseWheelScrollAction;
		bool innerTouchAllowed = DefaultTouchAllowed;
		bool smartFetch = true;
#pragma warning disable 618
		CurrentTimeVisibility showCurrentTime = CurrentTimeVisibility.Auto;
#pragma warning restore 618
		#endregion
		#region Properties
		internal TimeZoneHelper TimeZoneHelper {
			get { return timeZoneEngine; }
			set {
				if (timeZoneEngine == value)
					return;
				string oldTimeZoneId = null;
				if (!ShouldSerializeClientTimeZoneId()) {
					oldTimeZoneId = ClientTimeZoneId;
				}
				timeZoneEngine = value;
				if (!ShouldSerializeClientTimeZoneId()) {
					if (oldTimeZoneId != ClientTimeZoneId)
						OnChanged(new BaseOptionChangedEventArgs("ClientTimeZoneId", oldTimeZoneId, ClientTimeZoneId));
				}
			}
		}
		#region RecurrentAppointmentEditAction
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsBehaviorBaseRecurrentAppointmentEditAction"),
#endif
DefaultValue(defaultRecurrentAppointmentEditAction), XtraSerializableProperty(), NotifyParentProperty(true)]
		public RecurrentAppointmentAction RecurrentAppointmentEditAction {
			get { return recurrentAppointmentEditAction; }
			set {
				RecurrentAppointmentAction oldValue = recurrentAppointmentEditAction;
				if (oldValue == value)
					return;
				recurrentAppointmentEditAction = value;
				OnChanged(new BaseOptionChangedEventArgs("RecurrentAppointmentEditAction", oldValue, value));
			}
		}
		#endregion
		#region RecurrentAppointmentDeleteAction
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsBehaviorBaseRecurrentAppointmentDeleteAction"),
#endif
DefaultValue(defaultRecurrentAppointmentDeleteAction), XtraSerializableProperty(), NotifyParentProperty(true)]
		public RecurrentAppointmentAction RecurrentAppointmentDeleteAction {
			get { return recurrentAppointmentDeleteAction; }
			set {
				RecurrentAppointmentAction oldValue = recurrentAppointmentDeleteAction;
				if (oldValue == value)
					return;
				recurrentAppointmentDeleteAction = value;
				OnChanged(new BaseOptionChangedEventArgs("RecurrentAppointmentDeleteAction", oldValue, value));
			}
		}
		#endregion
		#region ClientTimeZoneId
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsBehaviorBaseClientTimeZoneId"),
#endif
		TypeConverter(typeof(DevExpress.XtraScheduler.Design.TimeZoneIdStringTypeConverter)),
		XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public string ClientTimeZoneId {
			get {
				if (String.IsNullOrEmpty(clientTimeZoneId))
					return ObtainServerTimeZoneId();
				return clientTimeZoneId;
			}
			set {
				string oldValue = clientTimeZoneId;
				if (oldValue == value)
					return;
				clientTimeZoneId = value;
				OnChanged(new BaseOptionChangedEventArgs("ClientTimeZoneId", oldValue, ClientTimeZoneId));
			}
		}
		protected internal virtual bool ShouldSerializeClientTimeZoneId() {
			return !String.IsNullOrEmpty(this.clientTimeZoneId);
		}
		protected internal virtual void ResetClientTimeZoneId() {
			ClientTimeZoneId = null;
		}
		#endregion
		#region ShowRemindersForm
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsBehaviorBaseShowRemindersForm"),
#endif
DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool ShowRemindersForm {
			get { return showRemindersForm; }
			set {
				if (showRemindersForm == value)
					return;
				showRemindersForm = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowRemindersForm", !showRemindersForm, showRemindersForm));
			}
		}
		#endregion
		[DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool EnableSmartFetch {
			get { return this.smartFetch; }
			set {
				if (this.smartFetch == value)
					return;
				this.smartFetch = value;
				OnChanged("SmartFetch", !this.smartFetch, this.smartFetch);
			}
		}
		#region RemindersFormDefaultAction
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsBehaviorBaseRemindersFormDefaultAction"),
#endif
DefaultValue(defaultRemindersFormDefaultAction), XtraSerializableProperty(), NotifyParentProperty(true)]
		public RemindersFormDefaultAction RemindersFormDefaultAction {
			get { return remindersFormDefaultAction; }
			set {
				RemindersFormDefaultAction oldValue = remindersFormDefaultAction;
				if (oldValue == value)
					return;
				remindersFormDefaultAction = value;
				OnChanged(new BaseOptionChangedEventArgs("RemindersFormDefaultAction", oldValue, value));
			}
		}
		#endregion
		#region SelectOnRightClick
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsBehaviorBaseSelectOnRightClick"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool SelectOnRightClick {
			get { return selectOnRightClick; }
			set {
				if (selectOnRightClick == value)
					return;
				selectOnRightClick = value;
				OnChanged(new BaseOptionChangedEventArgs("SelectOnRightClick", !selectOnRightClick, selectOnRightClick));
			}
		}
		#endregion
		#region MouseWheelScrollAction
#if !SL
		internal MouseWheelScrollAction InnerMouseWheelScrollAction {
			get {
				return innerMouseWheelScrollAction;
			}
			set {
				MouseWheelScrollAction oldValue = innerMouseWheelScrollAction;
				if (oldValue == value)
					return;
				innerMouseWheelScrollAction = value;
				OnChanged(new BaseOptionChangedEventArgs("MouseWheelScrollAction", oldValue, value));
			}
		}
#endif
		#endregion
		#region TouchAllowed
#if !SL
		internal bool InnerTouchAllowed {
			get { return innerTouchAllowed; }
			set {
				if (innerTouchAllowed == value)
					return;
				innerTouchAllowed = value;
				OnChanged(new BaseOptionChangedEventArgs("TouchAllowed", !value, value));
			}
		}
#endif
		#endregion
#pragma warning disable 618 
		[DefaultValue(CurrentTimeVisibility.Auto), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		[Obsolete("Use DayView.TimeMarkerVisibility property for DayView and its descendants - WorkWeekView and FullWeekView.", false)]
		public virtual CurrentTimeVisibility ShowCurrentTime {
			get { return showCurrentTime; }
			set {
				CurrentTimeVisibility oldValue = showCurrentTime;
				if (oldValue == value)
					return;
				showCurrentTime = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCurrentTime", oldValue, value));
			}
		}
#pragma warning restore 618
		#endregion
		#region ResetCore
		protected internal override void ResetCore() {
			RecurrentAppointmentEditAction = defaultRecurrentAppointmentEditAction;
			RecurrentAppointmentDeleteAction = defaultRecurrentAppointmentDeleteAction;
			ClientTimeZoneId = null;
			ShowRemindersForm = true;
			EnableSmartFetch = true;
			RemindersFormDefaultAction = defaultRemindersFormDefaultAction;
			SelectOnRightClick = false;
#if !SL
			InnerMouseWheelScrollAction = DefaultMouseWheelScrollAction;
			InnerTouchAllowed = DefaultTouchAllowed;
#endif
		}
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			SchedulerOptionsBehaviorBase optionsBehavior = options as SchedulerOptionsBehaviorBase;
			if (optionsBehavior != null) {
				BeginUpdate();
				try {
					TimeZoneHelper = optionsBehavior.TimeZoneHelper;
					RecurrentAppointmentEditAction = optionsBehavior.RecurrentAppointmentEditAction;
					RecurrentAppointmentDeleteAction = optionsBehavior.RecurrentAppointmentDeleteAction;
					if (optionsBehavior.ShouldSerializeClientTimeZoneId())
						ClientTimeZoneId = optionsBehavior.ClientTimeZoneId;
					else
						ResetClientTimeZoneId();
					ShowRemindersForm = optionsBehavior.ShowRemindersForm;
					EnableSmartFetch = optionsBehavior.EnableSmartFetch;
					RemindersFormDefaultAction = optionsBehavior.RemindersFormDefaultAction;
					SelectOnRightClick = optionsBehavior.SelectOnRightClick;
#if !SL
					InnerMouseWheelScrollAction = optionsBehavior.InnerMouseWheelScrollAction;
					InnerTouchAllowed = optionsBehavior.InnerTouchAllowed;
#endif
#pragma warning disable 618
					ShowCurrentTime = optionsBehavior.ShowCurrentTime;
#pragma warning restore 618
				} finally {
					EndUpdate();
				}
			}
		}
		protected virtual string ObtainServerTimeZoneId() {
			return TimeZoneEngine.Local.Id;
		}
	}
	#endregion
	#region SchedulerNavigationButtonOptions
	public class SchedulerNavigationButtonOptions : SchedulerNotificationOptions, IXtraSupportShouldSerialize {
		#region Fields
		internal static readonly TimeSpan defaultAppointmentSearchInterval = TimeSpan.FromDays(2 * 365);
		string prevCaption;
		string nextCaption;
		NavigationButtonVisibility visibility;
		TimeSpan appointmentSearchInterval;
		XtraSupportShouldSerializeHelper shouldSerializeHelper = new XtraSupportShouldSerializeHelper();
		#endregion
		public SchedulerNavigationButtonOptions() {
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("PrevCaption", XtraShouldSerializePrevCaption);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("NextCaption", XtraShouldSerializeNextCaption);
			shouldSerializeHelper.RegisterXtraShouldSerializeMethod("AppointmentSearchInterval", XtraShouldSerializeAppointmentSearchInterval);
		}
		#region Properties
		#region PrevCaption
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerNavigationButtonOptionsPrevCaption"),
#endif
XtraSerializableProperty(XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatDisable(), Localizable(true)]
		public string PrevCaption {
			get { return prevCaption; }
			set {
				string oldPrevCaption = prevCaption;
				if (oldPrevCaption == value)
					return;
				prevCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("PrevCaption", oldPrevCaption, prevCaption));
			}
		}
		protected internal virtual bool ShouldSerializePrevCaption() {
			return PrevCaption != SchedulerLocalizer.GetString(SchedulerStringId.Caption_PrevAppointment);
		}
		protected internal virtual bool XtraShouldSerializePrevCaption() {
			return ShouldSerializePrevCaption();
		}
		protected internal virtual void ResetPrevCaption() {
			this.PrevCaption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_PrevAppointment);
		}
		#endregion
		#region NextCaption
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerNavigationButtonOptionsNextCaption"),
#endif
XtraSerializableProperty(XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatDisable(), Localizable(true)]
		public string NextCaption {
			get { return nextCaption; }
			set {
				string oldNextCaption = nextCaption;
				if (oldNextCaption == value)
					return;
				nextCaption = value;
				OnChanged(new BaseOptionChangedEventArgs("NextCaption", oldNextCaption, nextCaption));
			}
		}
		protected internal virtual bool ShouldSerializeNextCaption() {
			return NextCaption != SchedulerLocalizer.GetString(SchedulerStringId.Caption_NextAppointment);
		}
		protected internal virtual bool XtraShouldSerializeNextCaption() {
			return ShouldSerializeNextCaption();
		}
		protected internal virtual void ResetNextCaption() {
			this.NextCaption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_NextAppointment);
		}
		#endregion
		#region Visibility
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerNavigationButtonOptionsVisibility"),
#endif
DefaultValue(NavigationButtonVisibility.Auto), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public NavigationButtonVisibility Visibility {
			get { return visibility; }
			set {
				NavigationButtonVisibility oldVisibility = visibility;
				if (visibility == value)
					return;
				visibility = value;
				OnChanged(new BaseOptionChangedEventArgs("Visibility", oldVisibility, visibility));
			}
		}
		#endregion
		#region AppointmentSearchInterval
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerNavigationButtonOptionsAppointmentSearchInterval"),
#endif
NotifyParentProperty(true), AutoFormatDisable(), Localizable(false)]
		public TimeSpan AppointmentSearchInterval {
			get { return appointmentSearchInterval; }
			set {
				if (value.Ticks <= 0)
					Exceptions.ThrowArgumentException("AppointmentSearchInterval", value);
				if (appointmentSearchInterval == value)
					return;
				TimeSpan oldAppointmentSearchInterval = AppointmentSearchInterval;
				appointmentSearchInterval = value;
				OnChanged(new BaseOptionChangedEventArgs("AppointmentSearchInterval", oldAppointmentSearchInterval, appointmentSearchInterval));
			}
		}
		protected internal virtual bool ShouldSerializeAppointmentSearchInterval() {
			return AppointmentSearchInterval != defaultAppointmentSearchInterval;
		}
		protected internal virtual void ResetAppointmentSearchInterval() {
			AppointmentSearchInterval = defaultAppointmentSearchInterval;
		}
		protected internal virtual bool XtraShouldSerializeAppointmentSearchInterval() {
			return ShouldSerializeAppointmentSearchInterval();
		}
		#endregion
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			SchedulerNavigationButtonOptions navigationButtonOptions = options as SchedulerNavigationButtonOptions;
			if (navigationButtonOptions != null) {
				BeginUpdate();
				try {
					PrevCaption = navigationButtonOptions.PrevCaption;
					NextCaption = navigationButtonOptions.NextCaption;
					Visibility = navigationButtonOptions.Visibility;
					AppointmentSearchInterval = navigationButtonOptions.AppointmentSearchInterval;
				} finally {
					EndUpdate();
				}
			}
		}
		protected internal override void ResetCore() {
			this.PrevCaption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_PrevAppointment);
			this.NextCaption = SchedulerLocalizer.GetString(SchedulerStringId.Caption_NextAppointment);
			this.Visibility = NavigationButtonVisibility.Auto;
			this.AppointmentSearchInterval = defaultAppointmentSearchInterval;
		}
		#region IXtraSupportShouldSerialize Members
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return shouldSerializeHelper.ShouldSerialize(propertyName);
		}
		#endregion
	}
	#endregion
	#region SchedulerOptionsViewBase
	public abstract class SchedulerOptionsViewBase : SchedulerNotificationOptions {
		#region Fields
		bool showOnlyResourceAppointments;
		FirstDayOfWeek firstDayOfWeek;
		SchedulerNavigationButtonOptions navigationButtonOptions = new SchedulerNavigationButtonOptions();
		#endregion
		protected SchedulerOptionsViewBase() {
			SubscribeNavigationButtonOptionsEvents();
		}
		#region Properties
		#region FirstDayOfWeek
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsViewBaseFirstDayOfWeek"),
#endif
DefaultValue(FirstDayOfWeek.System), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public FirstDayOfWeek FirstDayOfWeek {
			get { return firstDayOfWeek; }
			set {
				FirstDayOfWeek oldDayOfWeek = firstDayOfWeek;
				if (oldDayOfWeek == value)
					return;
				firstDayOfWeek = value;
				OnChanged(new BaseOptionChangedEventArgs("FirstDayOfWeek", oldDayOfWeek, firstDayOfWeek));
			}
		}
		#endregion
		#region ShowOnlyResourceAppointments
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsViewBaseShowOnlyResourceAppointments"),
#endif
DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable()]
		public bool ShowOnlyResourceAppointments {
			get { return showOnlyResourceAppointments; }
			set {
				if (showOnlyResourceAppointments == value)
					return;
				showOnlyResourceAppointments = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowOnlyResourceAppointments", !showOnlyResourceAppointments, showOnlyResourceAppointments));
			}
		}
		#endregion
		#region NavigationButtons
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerOptionsViewBaseNavigationButtons"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), NotifyParentProperty(true)]
		public SchedulerNavigationButtonOptions NavigationButtons { get { return navigationButtonOptions; } }
		#endregion
		#endregion
		protected internal virtual void SubscribeNavigationButtonOptionsEvents() {
			navigationButtonOptions.Changed += OnNavigationButtonOptionsChanged;
		}
		protected internal virtual void UnsubscribeNavigationButtonOptionsEvents() {
			navigationButtonOptions.Changed -= OnNavigationButtonOptionsChanged;
		}
		protected internal virtual void OnNavigationButtonOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
		protected internal override void ResetCore() {
			FirstDayOfWeek = FirstDayOfWeek.System;
			ShowOnlyResourceAppointments = false;
			NavigationButtons.Reset();
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			SchedulerOptionsViewBase optionsView = options as SchedulerOptionsViewBase;
			if (optionsView != null) {
				BeginUpdate();
				try {
					FirstDayOfWeek = optionsView.FirstDayOfWeek;
					ShowOnlyResourceAppointments = optionsView.ShowOnlyResourceAppointments;
					NavigationButtons.Assign(optionsView.NavigationButtons);
				} finally {
					EndUpdate();
				}
			}
		}
	}
	#endregion
	#region OptionsSelectionBehavior
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class OptionsSelectionBehavior : BaseOptions {
		UpdateSelectionDurationAction updateSelectionDurationAction;
		bool keepSelectedAppointments;
		public OptionsSelectionBehavior() {
			this.updateSelectionDurationAction = UpdateSelectionDurationAction.Reset;
			this.keepSelectedAppointments = false;
		}
		#region Properties
		#region UpdateSelectionDurationAction
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("OptionsSelectionBehaviorUpdateSelectionDurationAction"),
#endif
		DefaultValue(UpdateSelectionDurationAction.Reset), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatDisable()]
		public UpdateSelectionDurationAction UpdateSelectionDurationAction {
			get { return updateSelectionDurationAction; }
			set {
				UpdateSelectionDurationAction oldValue = updateSelectionDurationAction;
				if (oldValue == value)
					return;
				updateSelectionDurationAction = value;
				OnChanged("UpdateSelectionDurationAction", oldValue, value);
			}
		}
		#endregion
		#region KeepSelectedAppointments
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), NotifyParentProperty(true), AutoFormatDisable()]
		public bool KeepSelectedAppointments {
			get { return keepSelectedAppointments; }
			set {
				bool oldValue = keepSelectedAppointments;
				if (oldValue == value)
					return;
				keepSelectedAppointments = value;
				OnChanged("KeepSelectedAppointments", oldValue, value);
			}
		}
		#endregion
		#endregion
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			OptionsSelectionBehavior optionsSelectionBehavior = options as OptionsSelectionBehavior;
			if (optionsSelectionBehavior != null) {
				BeginUpdate();
				try {
					UpdateSelectionDurationAction = optionsSelectionBehavior.UpdateSelectionDurationAction;
					KeepSelectedAppointments = optionsSelectionBehavior.KeepSelectedAppointments;
				} finally {
					EndUpdate();
				}
			}
		}
	}
	#endregion
	#region SchedulerColumnPadding
	public class SchedulerColumnPadding : SchedulerNotificationOptions {
		const int DefaultLeft = 0;
		const int DefaultRight = 2;
		int left;
		int right;
		public SchedulerColumnPadding() {
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerColumnPaddingLeft"),
#endif
DefaultValue(DefaultLeft), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int Left {
			get { return left; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Left", value);
				int oldValue = left;
				if (oldValue == value)
					return;
				left = value;
				OnChanged(new BaseOptionChangedEventArgs("Left", oldValue, value));
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SchedulerColumnPaddingRight"),
#endif
DefaultValue(DefaultRight), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int Right {
			get { return right; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Right", value);
				int oldValue = right;
				if (oldValue == value)
					return;
				right = value;
				OnChanged(new BaseOptionChangedEventArgs("Right", oldValue, value));
			}
		}
		protected internal override void ResetCore() {
			Left = DefaultLeft;
			Right = DefaultRight;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				SchedulerColumnPadding otherOptions = options as SchedulerColumnPadding;
				if (otherOptions == null)
					return;
				Left = otherOptions.Left;
				Right = otherOptions.Right;
			} finally {
				EndUpdate();
			}
		}
	}
	#endregion
	#region SelectionBarOptions
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SelectionBarOptions : SchedulerNotificationOptions {
		const int defaultHeight = 0;
		#region Fields
		int height;
		bool visible;
		#endregion
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SelectionBarOptionsVisible"),
#endif
Category(SRCategoryNames.Appearance), DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)]
		public bool Visible {
			get { return visible; }
			set {
				if (visible == value)
					return;
				visible = value;
				OnChanged(new BaseOptionChangedEventArgs("Visible", !visible, visible));
			}
		}
		[
#if !SL
	DevExpressXtraSchedulerCoreLocalizedDescription("SelectionBarOptionsHeight"),
#endif
Category(SRCategoryNames.Appearance), DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int Height {
			get { return height; }
			set {
				int val = Math.Max(0, value);
				int oldVal = height;
				if (oldVal == val)
					return;
				height = val;
				OnChanged(new BaseOptionChangedEventArgs("Height", oldVal, height));
			}
		}
		#endregion
		public override string ToString() {
			return String.Empty;
		}
		protected internal override void ResetCore() {
			Visible = true;
			Height = defaultHeight;
		}
	}
	#endregion
	public enum TimeIndicatorVisibility { Never, Always, TodayView, CurrentDate };
	public enum TimeMarkerVisibility { Never, Always, TodayView };
	[Obsolete("Use TimeIndicatorVisibility or TimeMarkerVisibility instead.", false)]
	public enum CurrentTimeVisibility {
		Always, Never, Auto
	};
}
namespace DevExpress.XtraScheduler.Drawing {
	public enum ViewInfoItemAlignment {
		Left = 0,
		Top = 1,
		Right = 2,
		Bottom = 3,
	}
}
namespace DevExpress.XtraScheduler.Native {
	public interface ISchedulerDeferredScrollingOption {
		bool Allow { get; set; }
		event BaseOptionChangedEventHandler Changed;
	}
}
