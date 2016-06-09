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

using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Xpf.Core;
using DevExpress.XtraScheduler.Design;
using DevExpress.XtraScheduler;
using System.ComponentModel;
using System.Windows;
using System;
using DevExpress.Utils.Controls;
namespace DevExpress.Xpf.Scheduler {
	#region OptionsBehavior
	public class OptionsBehavior : SchedulerOptionsBase<SchedulerOptionsBehavior> {
		static OptionsBehavior() {
		}
		#region Properties
		#region RecurrentAppointmentEditAction
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsBehaviorRecurrentAppointmentEditAction")]
#endif
		public RecurrentAppointmentAction RecurrentAppointmentEditAction {
			get { return (RecurrentAppointmentAction)GetValue(RecurrentAppointmentEditActionProperty); }
			set { SetValue(RecurrentAppointmentEditActionProperty, value); }
		}
		public static readonly DependencyProperty RecurrentAppointmentEditActionProperty = CreateRecurrentAppointmentEditActionProperty();
		static DependencyProperty CreateRecurrentAppointmentEditActionProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, RecurrentAppointmentAction>("RecurrentAppointmentEditAction", RecurrentAppointmentAction.Occurrence, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnRecurrentAppointmentEditActionChanged(e.OldValue, e.NewValue));
		}
		private void OnRecurrentAppointmentEditActionChanged(RecurrentAppointmentAction oldValue, RecurrentAppointmentAction newValue) {
			UpdateInnerObjectPropertyValue(RecurrentAppointmentEditActionProperty, oldValue, newValue);
		}
		#endregion
		#region RecurrentAppointmentDeleteAction
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsBehaviorRecurrentAppointmentDeleteAction")]
#endif
		public RecurrentAppointmentAction RecurrentAppointmentDeleteAction {
			get { return (RecurrentAppointmentAction)GetValue(RecurrentAppointmentDeleteActionProperty); }
			set { SetValue(RecurrentAppointmentDeleteActionProperty, value); }
		}
		public static readonly DependencyProperty RecurrentAppointmentDeleteActionProperty = CreateRecurrentAppointmentDeleteActionProperty();
		static DependencyProperty CreateRecurrentAppointmentDeleteActionProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, RecurrentAppointmentAction>("RecurrentAppointmentDeleteAction", RecurrentAppointmentAction.Ask, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnRecurrentAppointmentDeleteActionChanged(e.OldValue, e.NewValue));
		}
		private void OnRecurrentAppointmentDeleteActionChanged(RecurrentAppointmentAction oldValue, RecurrentAppointmentAction newValue) {
			UpdateInnerObjectPropertyValue(RecurrentAppointmentDeleteActionProperty, oldValue, newValue);
		}
		#endregion
		#region ClientTimeZoneId
		[
#if !SL
	DevExpressXpfSchedulerLocalizedDescription("OptionsBehaviorClientTimeZoneId"),
#endif
 TypeConverter(typeof(TimeZoneIdTypeConverter))]
		public string ClientTimeZoneId {
			get { return (string)GetValue(ClientTimeZoneIdProperty); }
			set { SetValue(ClientTimeZoneIdProperty, value); }
		}
		public static readonly DependencyProperty ClientTimeZoneIdProperty = CreateClientTimeZoneIdProperty();
		static DependencyProperty CreateClientTimeZoneIdProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, string>("ClientTimeZoneId", String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnClientTimeZoneIdChanged(e.OldValue, e.NewValue));
		}
		void OnClientTimeZoneIdChanged(string oldValue, string newValue) {
			UpdateInnerObjectPropertyValue(ClientTimeZoneIdProperty, oldValue, newValue);
		}
		#endregion
		#region ShowRemindersForm
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsBehaviorShowRemindersForm")]
#endif
		public bool ShowRemindersForm {
			get { return (bool)GetValue(ShowRemindersFormProperty); }
			set { SetValue(ShowRemindersFormProperty, value); }
		}
		public static readonly DependencyProperty ShowRemindersFormProperty = CreateShowRemindersFormProperty();
		static DependencyProperty CreateShowRemindersFormProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, bool>("ShowRemindersForm", true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnShowRemindersFormChanged(e.OldValue, e.NewValue));
		}
		private void OnShowRemindersFormChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowRemindersFormProperty, oldValue, newValue);
		}
		#endregion
		#region RemindersFormDefaultAction
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsBehaviorRemindersFormDefaultAction")]
#endif
		public RemindersFormDefaultAction RemindersFormDefaultAction {
			get { return (RemindersFormDefaultAction)GetValue(RemindersFormDefaultActionProperty); }
			set { SetValue(RemindersFormDefaultActionProperty, value); }
		}
		public static readonly DependencyProperty RemindersFormDefaultActionProperty = CreateRemindersFormDefaultActionProperty();
		static DependencyProperty CreateRemindersFormDefaultActionProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, RemindersFormDefaultAction>("RemindersFormDefaultAction", RemindersFormDefaultAction.DismissAll, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnRemindersFormDefaultActionChanged(e.OldValue, e.NewValue));
		}
		private void OnRemindersFormDefaultActionChanged(RemindersFormDefaultAction oldValue, RemindersFormDefaultAction newValue) {
			UpdateInnerObjectPropertyValue(RemindersFormDefaultActionProperty, oldValue, newValue);
		}
		#endregion
		#region SelectOnRightClick
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("OptionsBehaviorSelectOnRightClick")]
#endif
		public bool SelectOnRightClick {
			get { return (bool)GetValue(SelectOnRightClickProperty); }
			set { SetValue(SelectOnRightClickProperty, value); }
		}
		public static readonly DependencyProperty SelectOnRightClickProperty = CreateSelectOnRightClickProperty();
		static DependencyProperty CreateSelectOnRightClickProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, bool>("SelectOnRightClick", false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => d.OnSelectOnRightClickChanged(e.OldValue, e.NewValue));
		}
		private void OnSelectOnRightClickChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(SelectOnRightClickProperty, oldValue, newValue);
		}
		#endregion
		#region MouseWheelScrollAction
#if !SL
		public MouseWheelScrollAction MouseWheelScrollAction {
			get { return (MouseWheelScrollAction)GetValue(MouseWheelScrollActionProperty); }
			set { SetValue(MouseWheelScrollActionProperty, value); }
		}
		public static readonly DependencyProperty MouseWheelScrollActionProperty = CreateMouseWheelScrollActionProperty();
		static DependencyProperty CreateMouseWheelScrollActionProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, MouseWheelScrollAction>("MouseWheelScrollAction", MouseWheelScrollAction.Time, (d, e) => d.OnMouseWheelScrollActionChanged(e.OldValue, e.NewValue), null);
		}
		void OnMouseWheelScrollActionChanged(MouseWheelScrollAction oldValue, MouseWheelScrollAction newValue) {
			UpdateInnerObjectPropertyValue(MouseWheelScrollActionProperty, oldValue, newValue);
		}
#endif
		#endregion
		#region TouchAllowed
#if !SL
		public bool TouchAllowed {
			get { return (bool)GetValue(TouchAllowedProperty); }
			set { SetValue(TouchAllowedProperty, value); }
		}
		public static readonly DependencyProperty TouchAllowedProperty = CreateTouchAllowedProperty();
		static DependencyProperty CreateTouchAllowedProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, bool>("TouchAllowed", SchedulerOptionsBehavior.DefaultTouchAllowed, (d, e) => d.OnTouchAllowedChanged(e.OldValue, e.NewValue), null);
		}
		void OnTouchAllowedChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(TouchAllowedProperty, oldValue, newValue);
		}
#endif
		#endregion
		#region AllowLeaveFocusOnTab
#if !SL
		public static readonly DependencyProperty AllowLeaveFocusOnTabProperty = DependencyProperty.Register("AllowLeaveFocusOnTab", typeof(bool), typeof(OptionsBehavior), new UIPropertyMetadata(false));
		public bool AllowLeaveFocusOnTab {
			get { return (bool)GetValue(AllowLeaveFocusOnTabProperty); }
			set { SetValue(AllowLeaveFocusOnTabProperty, value); }
		}
#endif
		#endregion
#pragma warning disable 618
		#region ShowCurrentTime
		[Obsolete("Use DayView.TimeIndicatorDisplayOptions.Visibility property for DayView and its descendants - WorkWeekView, FullWeekView and TimelineView.", false)]
		public CurrentTimeVisibility ShowCurrentTime {
			get { return (CurrentTimeVisibility)GetValue(ShowCurrentTimeProperty); }
			set { SetValue(ShowCurrentTimeProperty, value); }
		}
		public static readonly DependencyProperty ShowCurrentTimeProperty = CreateShowCurrentTimeProperty();
		static DependencyProperty CreateShowCurrentTimeProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<OptionsBehavior, CurrentTimeVisibility>("ShowCurrentTime", CurrentTimeVisibility.Auto, (d, e) => d.OnShowCurrentTimeChanged(e.OldValue, e.NewValue), null);
		}
		void OnShowCurrentTimeChanged(CurrentTimeVisibility oldValue, CurrentTimeVisibility newValue) {
			UpdateInnerObjectPropertyValue(ShowCurrentTimeProperty, oldValue, newValue);
		}
		#endregion
#pragma warning restore 618
		#endregion
		protected override DependencyPropertySyncManager CreateDependencyPropertySyncManager() {
			return new OptionsBehaviorPropertySyncManager(this);
		}
#if !SL
		protected override Freezable CreateInstanceCore() {
			return new OptionsBehavior();
		}
#endif
	}
	#endregion
	#region SchedulerOptionsBehavior
	public class SchedulerOptionsBehavior : SchedulerOptionsBehaviorBase {
#if !SL
		#region MouseWheelScrollAction
		public MouseWheelScrollAction MouseWheelScrollAction {
			get { return InnerMouseWheelScrollAction; }
			set {
				InnerMouseWheelScrollAction = value;
			}
		}
		#endregion
#endif
	}
	#endregion
}
