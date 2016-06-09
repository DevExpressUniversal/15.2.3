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
using System.Windows;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Native {
	#region SchedulerOptionsBasePropertySyncManager
	public abstract class SchedulerOptionsBasePropertySyncManager<T> : DependencyPropertySyncManager where T : BaseOptions {
		readonly SchedulerOptionsBase<T> options;
		protected SchedulerOptionsBasePropertySyncManager(SchedulerOptionsBase<T> options) {
			Guard.ArgumentNotNull(options, "options");
			this.options = options;
		}
		protected SchedulerOptionsBase<T> OptionsBase { get { return options; } }
	}
	#endregion
	#region SchedulerAppointmentDisplayOptionsPropertySyncManager
	public class SchedulerAppointmentDisplayOptionsPropertySyncManager : SchedulerOptionsBasePropertySyncManager<AppointmentDisplayOptions> {
		public SchedulerAppointmentDisplayOptionsPropertySyncManager(SchedulerAppointmentDisplayOptions options)
			: base(options) {
		}
		public SchedulerAppointmentDisplayOptions Options { get { return (SchedulerAppointmentDisplayOptions)OptionsBase; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(SchedulerAppointmentDisplayOptions.StatusDisplayTypeProperty, new StatusDisplayTypePropertyMapper(SchedulerAppointmentDisplayOptions.StatusDisplayTypeProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerAppointmentDisplayOptions.StartTimeVisibilityProperty, new StartTimeVisibilityPropertyMapper(SchedulerAppointmentDisplayOptions.StartTimeVisibilityProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerAppointmentDisplayOptions.EndTimeVisibilityProperty, new EndTimeVisibilityPropertyMapper(SchedulerAppointmentDisplayOptions.EndTimeVisibilityProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerAppointmentDisplayOptions.ShowReminderProperty, new ShowReminderPropertyMapper(SchedulerAppointmentDisplayOptions.ShowReminderProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerAppointmentDisplayOptions.ShowRecurrenceProperty, new ShowRecurrencePropertyMapper(SchedulerAppointmentDisplayOptions.ShowRecurrenceProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerAppointmentDisplayOptions.TimeDisplayTypeProperty, new TimeDisplayTypePropertyMapper(SchedulerAppointmentDisplayOptions.TimeDisplayTypeProperty, Options));
		}
	}
	#endregion
	#region SchedulerViewAppointmentDisplayOptionsPropertySyncManager
	public class SchedulerViewAppointmentDisplayOptionsPropertySyncManager : SchedulerAppointmentDisplayOptionsPropertySyncManager {
		public SchedulerViewAppointmentDisplayOptionsPropertySyncManager(SchedulerViewAppointmentDisplayOptions options)
			: base(options) {
		}
		public new SchedulerViewAppointmentDisplayOptions Options { get { return (SchedulerViewAppointmentDisplayOptions)OptionsBase; } }
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(SchedulerViewAppointmentDisplayOptions.SnapToCellsModeProperty, new SnapToCellsModePropertyMapper(SchedulerViewAppointmentDisplayOptions.SnapToCellsModeProperty, Options));
		}
	}
	#endregion
	#region SchedulerDayViewAppointmentDisplayOptionsPropertySyncManager
	public class SchedulerDayViewAppointmentDisplayOptionsPropertySyncManager : SchedulerViewAppointmentDisplayOptionsPropertySyncManager {
		public SchedulerDayViewAppointmentDisplayOptionsPropertySyncManager(SchedulerDayViewAppointmentDisplayOptions options)
			: base(options) {
		}
		public new SchedulerDayViewAppointmentDisplayOptions Options { get { return (SchedulerDayViewAppointmentDisplayOptions)OptionsBase; } }
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(SchedulerDayViewAppointmentDisplayOptions.AllDayAppointmentsStatusDisplayTypeProperty, new AllDayAppointmentsStatusDisplayTypePropertyMapper(SchedulerDayViewAppointmentDisplayOptions.AllDayAppointmentsStatusDisplayTypeProperty, Options));
		}
	}
	#endregion
	#region SchedulerTimelineViewAppointmentDisplayOptionsPropertySyncManager
	public class SchedulerTimelineViewAppointmentDisplayOptionsPropertySyncManager : SchedulerViewAppointmentDisplayOptionsPropertySyncManager {
		public SchedulerTimelineViewAppointmentDisplayOptionsPropertySyncManager(SchedulerTimelineViewAppointmentDisplayOptions options)
			: base(options) {
		}
		public new SchedulerTimelineViewAppointmentDisplayOptions Options { get { return (SchedulerTimelineViewAppointmentDisplayOptions)OptionsBase; } }
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(SchedulerTimelineViewAppointmentDisplayOptions.AppointmentAutoHeightProperty, new AppointmentAutoHeightPropertyMapper(SchedulerTimelineViewAppointmentDisplayOptions.AppointmentAutoHeightProperty, Options));
		}
	}
	public class AppointmentAutoHeightPropertyMapper : SchedulerAppointmentDisplayOptionsPropertyMapper {
		public AppointmentAutoHeightPropertyMapper(DependencyProperty property, SchedulerAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AppointmentAutoHeight";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AppointmentAutoHeight;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AppointmentAutoHeight = (bool)newValue;
		}
	}
	#endregion
	public class SchedulerMonthViewAppointmentDisplayOptionsPropertySyncManager : SchedulerAppointmentDisplayOptionsPropertySyncManager {
		public SchedulerMonthViewAppointmentDisplayOptionsPropertySyncManager(SchedulerMonthViewAppointmentDisplayOptions options)
			: base(options) {
		}
		public new SchedulerMonthViewAppointmentDisplayOptions Options { get { return (SchedulerMonthViewAppointmentDisplayOptions)OptionsBase; } }
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(SchedulerMonthViewAppointmentDisplayOptions.AppointmentAutoHeightProperty, new AppointmentAutoHeightPropertyMapper(SchedulerMonthViewAppointmentDisplayOptions.AppointmentAutoHeightProperty, Options));
		}
	}
	public class SchedulerWeekViewAppointmentDisplayOptionsPropertySyncManager : SchedulerAppointmentDisplayOptionsPropertySyncManager {
		public SchedulerWeekViewAppointmentDisplayOptionsPropertySyncManager(SchedulerWeekViewAppointmentDisplayOptions options)
			: base(options) {
		}
		public new SchedulerWeekViewAppointmentDisplayOptions Options { get { return (SchedulerWeekViewAppointmentDisplayOptions)OptionsBase; } }
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(SchedulerWeekViewAppointmentDisplayOptions.AppointmentAutoHeightProperty, new AppointmentAutoHeightPropertyMapper(SchedulerWeekViewAppointmentDisplayOptions.AppointmentAutoHeightProperty, Options));
		}
	}
	#region OptionsBehaviorPropertySyncManager
	public class OptionsBehaviorPropertySyncManager : SchedulerOptionsBasePropertySyncManager<SchedulerOptionsBehavior> {
		public OptionsBehaviorPropertySyncManager(OptionsBehavior options)
			: base(options) {
		}
		public OptionsBehavior Options { get { return (OptionsBehavior)OptionsBase; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.RecurrentAppointmentEditActionProperty, new RecurrentAppointmentEditActionPropertyMapper(OptionsBehavior.RecurrentAppointmentEditActionProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.RecurrentAppointmentDeleteActionProperty, new RecurrentAppointmentDeleteActionPropertyMapper(OptionsBehavior.RecurrentAppointmentDeleteActionProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.ClientTimeZoneIdProperty, new ClientTimeZoneIdPropertyMapper(OptionsBehavior.ClientTimeZoneIdProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.ShowRemindersFormProperty, new ShowRemindersFormPropertyMapper(OptionsBehavior.ShowRemindersFormProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.RemindersFormDefaultActionProperty, new RemindersFormDefaultActionPropertyMapper(OptionsBehavior.RemindersFormDefaultActionProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.SelectOnRightClickProperty, new SelectOnRightClickPropertyMapper(OptionsBehavior.SelectOnRightClickProperty, Options));
#if !SL
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.MouseWheelScrollActionProperty, new MouseWheelScrollActionPropertyMapper(OptionsBehavior.MouseWheelScrollActionProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.TouchAllowedProperty, new TouchAllowedPropertyMapper(OptionsBehavior.TouchAllowedProperty, Options));
#endif
			PropertyMapperTable.RegisterPropertyMapper(OptionsBehavior.ShowCurrentTimeProperty, new ShowCurrentTimePropertyMapper(OptionsBehavior.ShowCurrentTimeProperty, Options));
		}
	}
	#endregion
	#region OptionsCustomizationPropertySyncManager
	public class OptionsCustomizationPropertySyncManager : SchedulerOptionsBasePropertySyncManager<SchedulerOptionsCustomization> {
		public OptionsCustomizationPropertySyncManager(OptionsCustomization options)
			: base(options) {
		}
		public OptionsCustomization Options { get { return (OptionsCustomization)OptionsBase; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentCopyProperty, new AllowAppointmentCopyPropertyMapper(OptionsCustomization.AllowAppointmentCopyProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentDragProperty, new AllowAppointmentDragPropertyMapper(OptionsCustomization.AllowAppointmentDragProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentResizeProperty, new AllowAppointmentResizePropertyMapper(OptionsCustomization.AllowAppointmentResizeProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentDeleteProperty, new AllowAppointmentDeletePropertyMapper(OptionsCustomization.AllowAppointmentDeleteProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentCreateProperty, new AllowAppointmentCreatePropertyMapper(OptionsCustomization.AllowAppointmentCreateProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentEditProperty, new AllowAppointmentEditPropertyMapper(OptionsCustomization.AllowAppointmentEditProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentDragBetweenResourcesProperty, new AllowAppointmentDragBetweenResourcesPropertyMapper(OptionsCustomization.AllowAppointmentDragBetweenResourcesProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowInplaceEditorProperty, new AllowInplaceEditorPropertyMapper(OptionsCustomization.AllowInplaceEditorProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentMultiSelectProperty, new AllowAppointmentMultiSelectPropertyMapper(OptionsCustomization.AllowAppointmentMultiSelectProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowAppointmentConflictsProperty, new AllowAppointmentConflictsPropertyMapper(OptionsCustomization.AllowAppointmentConflictsProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsCustomization.AllowDisplayAppointmentFormProperty, new AllowDisplayAppointmentFormPropertyMapper(OptionsCustomization.AllowDisplayAppointmentFormProperty, Options));
		}
	}
	#endregion
	#region OptionsViewPropertySyncManager
	public class OptionsViewPropertySyncManager : SchedulerOptionsBasePropertySyncManager<SchedulerOptionsViewBase> {
		public OptionsViewPropertySyncManager(OptionsView options)
			: base(options) {
		}
		public OptionsView Options { get { return (OptionsView)OptionsBase; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(OptionsView.FirstDayOfWeekProperty, new FirstDayOfWeekPropertyMapper(OptionsView.FirstDayOfWeekProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(OptionsView.ShowOnlyResourceAppointmentsProperty, new ShowOnlyResourceAppointmentsPropertyMapper(OptionsView.ShowOnlyResourceAppointmentsProperty, Options));
		}
	}
	#endregion
	#region NavigationButtonOptionsPropertySyncManager
	public class NavigationButtonOptionsPropertySyncManager : SchedulerOptionsBasePropertySyncManager<SchedulerNavigationButtonOptions> {
		public NavigationButtonOptionsPropertySyncManager(NavigationButtonOptions options)
			: base(options) {
		}
		public NavigationButtonOptions Options { get { return (NavigationButtonOptions)OptionsBase; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(NavigationButtonOptions.VisibilityProperty, new VisibilityPropertyMapper(NavigationButtonOptions.VisibilityProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(NavigationButtonOptions.AppointmentSearchIntervalProperty, new AppointmentSearchIntervalPropertyMapper(NavigationButtonOptions.AppointmentSearchIntervalProperty, Options));
		}
	}
	#endregion
	#region SchedulerSelectionBarOptionsPropertySyncManager
	public class SchedulerSelectionBarOptionsPropertySyncManager : SchedulerOptionsBasePropertySyncManager<SelectionBarOptions> {
		public SchedulerSelectionBarOptionsPropertySyncManager(SchedulerSelectionBarOptions options)
			: base(options) {
		}
		public SchedulerSelectionBarOptions Options { get { return (SchedulerSelectionBarOptions)OptionsBase; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(SchedulerSelectionBarOptions.VisibleProperty, new VisiblePropertyMapper(SchedulerSelectionBarOptions.VisibleProperty, Options));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerSelectionBarOptions.HeightProperty, new HeightPropertyMapper(SchedulerSelectionBarOptions.HeightProperty, Options));
		}
	}
	#endregion
	public abstract class SchedulerOptionsBasePropertyMapper<T> : DependencyPropertyMapperBase where T : BaseOptions {
		protected SchedulerOptionsBasePropertyMapper(DependencyProperty property, SchedulerOptionsBase<T> options)
			: base(property, options) {
		}
		public SchedulerOptionsBase<T> Options { get { return (SchedulerOptionsBase<T>)Owner; } }
		public T InnerOptions { get { return Options.InnerObject; } }
		protected abstract string GetInnerPropertyName();
		protected abstract void SetInnerPropertyValueCore(object newValue);
		protected abstract object GetInnerPropertyValueCore();
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			SchedulerNotificationOptions opt = InnerOptions as SchedulerNotificationOptions;
			if (opt != null)
				opt.Changed += OnInnerOptionsChanged;
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			SchedulerNotificationOptions opt = InnerOptions as SchedulerNotificationOptions;
			if (opt != null)
				opt.Changed -= OnInnerOptionsChanged;
		}
		protected void OnInnerOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == GetInnerPropertyName())
				UpdateOwnerPropertyValue();
		}
		public override object GetInnerPropertyValue() {
			XtraSchedulerDebug.CheckNotNull("InnerOptions", InnerOptions);
			return GetInnerPropertyValueCore();
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			XtraSchedulerDebug.CheckNotNull("InnerOptions", InnerOptions);
			SetInnerPropertyValueCore(newValue);
		}
	}
	#region SchedulerAppointmentDisplayOptions
	public abstract class SchedulerAppointmentDisplayOptionsPropertyMapper : SchedulerOptionsBasePropertyMapper<AppointmentDisplayOptions> {
		protected SchedulerAppointmentDisplayOptionsPropertyMapper(DependencyProperty property, SchedulerAppointmentDisplayOptions options)
			: base(property, options) {
		}
	}
	public class StatusDisplayTypePropertyMapper : SchedulerAppointmentDisplayOptionsPropertyMapper {
		public StatusDisplayTypePropertyMapper(DependencyProperty property, SchedulerAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.StatusDisplayType;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.StatusDisplayType = (AppointmentStatusDisplayType)newValue;
		}
		protected override string GetInnerPropertyName() {
			return "StatusDisplayType";
		}
	}
	public class StartTimeVisibilityPropertyMapper : SchedulerAppointmentDisplayOptionsPropertyMapper {
		public StartTimeVisibilityPropertyMapper(DependencyProperty property, SchedulerAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "StartTimeVisibility";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.StartTimeVisibility;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.StartTimeVisibility = (AppointmentTimeVisibility)newValue;
		}
	}
	public class EndTimeVisibilityPropertyMapper : SchedulerAppointmentDisplayOptionsPropertyMapper {
		public EndTimeVisibilityPropertyMapper(DependencyProperty property, SchedulerAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "EndTimeVisibility";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.EndTimeVisibility;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.EndTimeVisibility = (AppointmentTimeVisibility)newValue;
		}
	}
	public class TimeDisplayTypePropertyMapper : SchedulerAppointmentDisplayOptionsPropertyMapper {
		public TimeDisplayTypePropertyMapper(DependencyProperty property, SchedulerAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "TimeDisplayType";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.TimeDisplayType;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.TimeDisplayType = (AppointmentTimeDisplayType)newValue;
		}
	}
	public class ShowReminderPropertyMapper : SchedulerAppointmentDisplayOptionsPropertyMapper {
		public ShowReminderPropertyMapper(DependencyProperty property, SchedulerAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "ShowReminder";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.ShowReminder;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.ShowReminder = (bool)newValue;
		}
	}
	public class ShowRecurrencePropertyMapper : SchedulerAppointmentDisplayOptionsPropertyMapper {
		public ShowRecurrencePropertyMapper(DependencyProperty property, SchedulerAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return " ShowRecurrence";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.ShowRecurrence;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.ShowRecurrence = (bool)newValue;
		}
	}
	#endregion
	#region SchedulerViewAppointmentDisplayOptions
	public abstract class SchedulerViewAppointmentDisplayOptionsPropertyMapper : SchedulerOptionsBasePropertyMapper<AppointmentDisplayOptions> {
		protected SchedulerViewAppointmentDisplayOptionsPropertyMapper(DependencyProperty property, SchedulerViewAppointmentDisplayOptions options)
			: base(property, options) {
		}
	}
	public class SnapToCellsModePropertyMapper : SchedulerViewAppointmentDisplayOptionsPropertyMapper {
		public SnapToCellsModePropertyMapper(DependencyProperty property, SchedulerViewAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "SnapToCellsMode";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.SnapToCellsMode;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.SnapToCellsMode = (AppointmentSnapToCellsMode)newValue;
		}
	}
	#endregion
	#region SchedulerDayViewAppointmentDisplayOptions
	public abstract class SchedulerDayViewAppointmentDisplayOptionsPropertyMapper : SchedulerViewAppointmentDisplayOptionsPropertyMapper {
		protected SchedulerDayViewAppointmentDisplayOptionsPropertyMapper(DependencyProperty property, SchedulerDayViewAppointmentDisplayOptions options)
			: base(property, options) {
		}
		public DayViewAppointmentDisplayOptions DayViewInnerOptions { get { return (DayViewAppointmentDisplayOptions)InnerOptions; } }
	}
	public class AllDayAppointmentsStatusDisplayTypePropertyMapper : SchedulerDayViewAppointmentDisplayOptionsPropertyMapper {
		public AllDayAppointmentsStatusDisplayTypePropertyMapper(DependencyProperty property, SchedulerDayViewAppointmentDisplayOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllDayAppointmentsStatusDisplayType";
		}
		protected override object GetInnerPropertyValueCore() {
			return DayViewInnerOptions.AllDayAppointmentsStatusDisplayType;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			DayViewInnerOptions.AllDayAppointmentsStatusDisplayType = (AppointmentStatusDisplayType)newValue;
		}
	}
	#endregion
	#region OptionsBehavior
	public abstract class OptionsBehaviorPropertyMapperBase : SchedulerOptionsBasePropertyMapper<SchedulerOptionsBehavior> {
		protected OptionsBehaviorPropertyMapperBase(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
	}
	public class RecurrentAppointmentEditActionPropertyMapper : OptionsBehaviorPropertyMapperBase {
		public RecurrentAppointmentEditActionPropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "RecurrentAppointmentEditAction";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.RecurrentAppointmentEditAction;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.RecurrentAppointmentEditAction = (RecurrentAppointmentAction)newValue;
		}
	}
	public class RecurrentAppointmentDeleteActionPropertyMapper : OptionsBehaviorPropertyMapperBase {
		public RecurrentAppointmentDeleteActionPropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "RecurrentAppointmentDeleteAction";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.RecurrentAppointmentDeleteAction;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.RecurrentAppointmentDeleteAction = (RecurrentAppointmentAction)newValue;
		}
	}
	public class ClientTimeZoneIdPropertyMapper : OptionsBehaviorPropertyMapperBase {
		bool lockInnerPropertyUpdate;
		public ClientTimeZoneIdPropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {				
		}
		protected override string GetInnerPropertyName() {
			return "ClientTimeZoneId";
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			if (InnerOptions == null || InnerOptions.TimeZoneHelper == null)
				return;
			InnerOptions.TimeZoneHelper.ClientTimeZoneChanged += ClientTimeZoneChanged;
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			if (InnerOptions == null || InnerOptions.TimeZoneHelper == null)
				return;
			InnerOptions.TimeZoneHelper.ClientTimeZoneChanged -= ClientTimeZoneChanged;
		}
		void ClientTimeZoneChanged(object sender, EventArgs e) {
			UpdateOwnerPropertyValue();
		}		
		protected override void UpdateOwnerPropertyValue() {
			this.lockInnerPropertyUpdate = true;
			try {
				base.UpdateOwnerPropertyValue();
			} finally {
				this.lockInnerPropertyUpdate = false;
			}
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.ClientTimeZoneId;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			if (this.lockInnerPropertyUpdate)
				return;
			InnerOptions.ClientTimeZoneId = (string)newValue;
			DevExpress.Xpf.Scheduler.Drawing.Native.TimeIndicatorTimerService.ForceUpdate();
		}
	}
	public class ShowRemindersFormPropertyMapper : OptionsBehaviorPropertyMapperBase {
		public ShowRemindersFormPropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "ShowRemindersForm";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.ShowRemindersForm;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.ShowRemindersForm = (bool)newValue;
		}
	}
	public class RemindersFormDefaultActionPropertyMapper : OptionsBehaviorPropertyMapperBase {
		public RemindersFormDefaultActionPropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "RemindersFormDefaultAction";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.RemindersFormDefaultAction;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.RemindersFormDefaultAction = (RemindersFormDefaultAction)newValue;
		}
	}
	public class SelectOnRightClickPropertyMapper : OptionsBehaviorPropertyMapperBase {
		public SelectOnRightClickPropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "SelectOnRightClick";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.SelectOnRightClick;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.SelectOnRightClick = (bool)newValue;
		}
	}
#pragma warning disable 618
	public class ShowCurrentTimePropertyMapper : OptionsBehaviorPropertyMapperBase {
		public ShowCurrentTimePropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "ShowCurrentTime";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.ShowCurrentTime;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.ShowCurrentTime = (CurrentTimeVisibility)newValue;
		}
	}
#pragma warning restore 618
#if !SL
	public class TouchAllowedPropertyMapper : OptionsBehaviorPropertyMapperBase {
		public TouchAllowedPropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "TouchAllowed";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.InnerTouchAllowed;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.InnerTouchAllowed = (bool)newValue;
		}
	}
	public class MouseWheelScrollActionPropertyMapper : OptionsBehaviorPropertyMapperBase {
		public MouseWheelScrollActionPropertyMapper(DependencyProperty property, OptionsBehavior options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "MouseWheelScrollAction";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.MouseWheelScrollAction;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.MouseWheelScrollAction = (MouseWheelScrollAction)newValue;
		}
	}
#endif
	#endregion
	#region OptionsCustomization
	public abstract class OptionsCustomizationPropertyMapper : SchedulerOptionsBasePropertyMapper<SchedulerOptionsCustomization> {
		protected OptionsCustomizationPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
	}
	public class AllowAppointmentCopyPropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentCopyPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentCopy";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentCopy;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentCopy = (UsedAppointmentType)newValue;
		}
	}
	public class AllowAppointmentDragPropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentDragPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentDrag";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentDrag;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentDrag = (UsedAppointmentType)newValue;
		}
	}
	public class AllowAppointmentResizePropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentResizePropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentResize";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentResize;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentResize = (UsedAppointmentType)newValue;
		}
	}
	public class AllowAppointmentDeletePropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentDeletePropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentDelete";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentDelete;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentDelete = (UsedAppointmentType)newValue;
		}
	}
	public class AllowAppointmentCreatePropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentCreatePropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentCreate";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentCreate;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentCreate = (UsedAppointmentType)newValue;
		}
	}
	public class AllowAppointmentEditPropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentEditPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentEdit";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentEdit;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentEdit = (UsedAppointmentType)newValue;
		}
	}
	public class AllowAppointmentDragBetweenResourcesPropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentDragBetweenResourcesPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentDragBetweenResources";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentDragBetweenResources;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentDragBetweenResources = (UsedAppointmentType)newValue;
		}
	}
	public class AllowInplaceEditorPropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowInplaceEditorPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowInplaceEditor";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowInplaceEditor;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowInplaceEditor = (UsedAppointmentType)newValue;
		}
	}
	public class AllowAppointmentMultiSelectPropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentMultiSelectPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentMultiSelect";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentMultiSelect;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentMultiSelect = (bool)newValue;
		}
	}
	public class AllowAppointmentConflictsPropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowAppointmentConflictsPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowAppointmentConflicts";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowAppointmentConflicts;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowAppointmentConflicts = (AppointmentConflictsMode)newValue;
		}
	}
	public class AllowDisplayAppointmentFormPropertyMapper : OptionsCustomizationPropertyMapper {
		public AllowDisplayAppointmentFormPropertyMapper(DependencyProperty property, OptionsCustomization options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AllowDisplayAppointmentForm";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AllowDisplayAppointmentForm;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AllowDisplayAppointmentForm = (AllowDisplayAppointmentForm)newValue;
		}
	}
	#endregion
	#region OptionsView mappers
	public abstract class OptionsViewPropertyMapper : SchedulerOptionsBasePropertyMapper<SchedulerOptionsViewBase> {
		protected OptionsViewPropertyMapper(DependencyProperty property, OptionsView options)
			: base(property, options) {
		}
	}
	public class FirstDayOfWeekPropertyMapper : OptionsViewPropertyMapper {
		public FirstDayOfWeekPropertyMapper(DependencyProperty property, OptionsView options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "FirstDayOfWeek";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.FirstDayOfWeek;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.FirstDayOfWeek = (FirstDayOfWeek)newValue;
		}
	}
	public class ShowOnlyResourceAppointmentsPropertyMapper : OptionsViewPropertyMapper {
		public ShowOnlyResourceAppointmentsPropertyMapper(DependencyProperty property, OptionsView options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "ShowOnlyResourceAppointments";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.ShowOnlyResourceAppointments;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.ShowOnlyResourceAppointments = (bool)newValue;
		}
	}
	#endregion
	#region NavigationButtonOptions
	public abstract class NavigationButtonOptionsPropertyMapper : SchedulerOptionsBasePropertyMapper<SchedulerNavigationButtonOptions> {
		protected NavigationButtonOptionsPropertyMapper(DependencyProperty property, NavigationButtonOptions options)
			: base(property, options) {
		}
	}
	public class VisibilityPropertyMapper : NavigationButtonOptionsPropertyMapper {
		public VisibilityPropertyMapper(DependencyProperty property, NavigationButtonOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "Visibility";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.Visibility;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.Visibility = (NavigationButtonVisibility)newValue;
		}
	}
	public class AppointmentSearchIntervalPropertyMapper : NavigationButtonOptionsPropertyMapper {
		public AppointmentSearchIntervalPropertyMapper(DependencyProperty property, NavigationButtonOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "AppointmentSearchInterval";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.AppointmentSearchInterval;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.AppointmentSearchInterval = (TimeSpan)newValue;
		}
	}
	#endregion
	#region SchedulerSelectionBarOptions
	public abstract class SchedulerSelectionBarOptionsPropertyMapperBase : SchedulerOptionsBasePropertyMapper<SelectionBarOptions> {
		protected SchedulerSelectionBarOptionsPropertyMapperBase(DependencyProperty property, SchedulerSelectionBarOptions options)
			: base(property, options) {
		}
	}
	public class VisiblePropertyMapper : SchedulerSelectionBarOptionsPropertyMapperBase {
		public VisiblePropertyMapper(DependencyProperty property, SchedulerSelectionBarOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "Visible";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.Visible;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.Visible = (bool)newValue;
		}
	}
	public class HeightPropertyMapper : SchedulerSelectionBarOptionsPropertyMapperBase {
		public HeightPropertyMapper(DependencyProperty property, SchedulerSelectionBarOptions options)
			: base(property, options) {
		}
		protected override string GetInnerPropertyName() {
			return "Height";
		}
		protected override object GetInnerPropertyValueCore() {
			return InnerOptions.Height;
		}
		protected override void SetInnerPropertyValueCore(object newValue) {
			InnerOptions.Height = (int)newValue;
		}
	}
	#endregion
	#region TimeIndicatorDisplayOptionsPropertySyncManager
	public class TimeIndicatorDisplayOptionsPropertySyncManager : SchedulerOptionsBasePropertySyncManager<TimeIndicatorDisplayOptionsBase> {
		public TimeIndicatorDisplayOptionsPropertySyncManager(SchedulerTimeIndicatorDisplayOptions options)
			: base(options) {
		}
		public SchedulerTimeIndicatorDisplayOptions Options { get { return (SchedulerTimeIndicatorDisplayOptions)OptionsBase; } }
		public override void Register() {
		}
	}
	#endregion
}
