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
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Xpf.Scheduler.Native {
	public class DayViewPropertySyncManager : SchedulerViewPropertySyncManager<DayView> {
		public DayViewPropertySyncManager(DayView view)
			: base(view) {
		}
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(DayView.DayCountProperty, new DayViewDayCountPropertyMapper(DayView.DayCountProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.ShowAllDayAreaProperty, new DayViewShowAllDayAreaPropertyMapper(DayView.ShowAllDayAreaProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.ShowAllAppointmentsAtTimeCellsProperty, new ShowAllAppointmentsAtTimeCellsPropertyMapper(DayView.ShowAllAppointmentsAtTimeCellsProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.ShowDayHeadersProperty, new DayViewShowDayHeadersPropertyMapper(DayView.ShowDayHeadersProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.ShowWorkTimeOnlyProperty, new DayViewShowWorkTimeOnlyPropertyMapper(DayView.ShowWorkTimeOnlyProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.ShowMoreButtonsOnEachColumnProperty, new DayViewShowMoreButtonsOnEachColumnPropertyMapper(DayView.ShowMoreButtonsOnEachColumnProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.TimeScaleProperty, new DayViewTimeScalePropertyMapper(DayView.TimeScaleProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.VisibleTimeProperty, new DayVIewVisibleTimePropertyMapper(DayView.VisibleTimeProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.WorkTimeProperty, new DayViewWorkTimePropertyMapper(DayView.WorkTimeProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.AppointmentDisplayOptionsProperty, new DayViewAppointmentDisplayOptionsPropertyMapper(DayView.AppointmentDisplayOptionsProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(DayView.TimeMarkerVisibilityProperty, new DayViewTimeMarkerVisibilityPropertyMapper(DayView.TimeMarkerVisibilityProperty, View));
		}
	}
	#region DayView mappers
	public abstract class DayViewPropertyMapperBase : SchedulerViewPropertyMapperBase<DayView, InnerDayView> {
		protected DayViewPropertyMapperBase(DependencyProperty property, DayView view)
			: base(property, view) {
		}
	}
	public class DayViewDayCountPropertyMapper : DayViewPropertyMapperBase {
		public DayViewDayCountPropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.DayCountChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.DayCount;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.DayCount = (int)newValue;
		}
	}
	public class DayViewShowAllDayAreaPropertyMapper : DayViewPropertyMapperBase {
		public DayViewShowAllDayAreaPropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ShowAllDayAreaChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ShowAllDayArea;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ShowAllDayArea = (bool)newValue;
		}
	}
	public class ShowAllAppointmentsAtTimeCellsPropertyMapper : DayViewPropertyMapperBase {
		public ShowAllAppointmentsAtTimeCellsPropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ShowAllAppointmentsAtTimeCellsChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ShowAllAppointmentsAtTimeCells;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ShowAllAppointmentsAtTimeCells = (bool)newValue;
		}
	}
	public class DayViewShowDayHeadersPropertyMapper : DayViewPropertyMapperBase {
		public DayViewShowDayHeadersPropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ShowDayViewDayHeadersChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ShowDayHeaders;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ShowDayHeaders = (bool)newValue;
		}
	}
	public class DayViewShowWorkTimeOnlyPropertyMapper : DayViewPropertyMapperBase {
		public DayViewShowWorkTimeOnlyPropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ShowWorkTimeOnlyChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ShowWorkTimeOnly;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ShowWorkTimeOnly = (bool)newValue;
		}
	}
	public class DayViewShowMoreButtonsOnEachColumnPropertyMapper : DayViewPropertyMapperBase {
		public DayViewShowMoreButtonsOnEachColumnPropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ShowMoreButtonsOnEachColumnChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ShowMoreButtonsOnEachColumn;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ShowMoreButtonsOnEachColumn = (bool)newValue;
		}
	}
	public class DayViewTimeScalePropertyMapper : DayViewPropertyMapperBase {
		public DayViewTimeScalePropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.TimeScaleChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.TimeScale;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.TimeScale = (TimeSpan)newValue;
			DevExpress.Xpf.Scheduler.Commands.XpfSwitchTimeScaleCommand.SyncTopRowTime(View.Control.InnerControl);
		}
	}
	public class DayVIewVisibleTimePropertyMapper : DayViewPropertyMapperBase {
		public DayVIewVisibleTimePropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.VisibleTimeChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.VisibleTime;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.VisibleTime = (TimeOfDayInterval)newValue;
		}
	}
	public class DayViewWorkTimePropertyMapper : DayViewPropertyMapperBase {
		public DayViewWorkTimePropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.WorkTimeChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.WorkTime;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.WorkTime = (TimeOfDayInterval)newValue;
		}
	}
	public class DayViewTimeMarkerVisibilityPropertyMapper : DayViewPropertyMapperBase {
		public DayViewTimeMarkerVisibilityPropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.TimeMarkerVisibilityChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.TimeMarkerVisibility;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.TimeMarkerVisibility = (TimeMarkerVisibility)newValue;
		}
	}
	public class DayViewAppointmentDisplayOptionsPropertyMapper : AppointmentDisplayOptionsPropertyMapperBase<DayView, AppointmentDisplayOptions> {
		public DayViewAppointmentDisplayOptionsPropertyMapper(DependencyProperty property, DayView view)
			: base(property, view) {
		}
	}
	#endregion
}
