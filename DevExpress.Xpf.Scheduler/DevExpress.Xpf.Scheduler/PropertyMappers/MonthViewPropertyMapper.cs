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
	public class WeekViewPropertySyncManager : SchedulerViewPropertySyncManager<WeekView> {
		public WeekViewPropertySyncManager(WeekView view)
			: base(view) {
		}
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(WeekView.AppointmentDisplayOptionsProperty, new WeekViewAppointmentDisplayOptionsPropertyMapper(WeekView.AppointmentDisplayOptionsProperty, View));
		}
	}
	public class MonthViewPropertySyncManager : SchedulerViewPropertySyncManager<MonthView> {
		public MonthViewPropertySyncManager(MonthView view)
			: base(view) {
		}
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(MonthView.ShowWeekendProperty, new MonthViewShowWeekendPropertyMapper(MonthView.ShowWeekendProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(MonthView.CompressWeekendProperty, new MonthViewCompressWeekendPropertyMapper(MonthView.CompressWeekendProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(MonthView.WeekCountProperty, new MonthViewWeekCountPropertyMapper(MonthView.WeekCountProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(MonthView.AppointmentDisplayOptionsProperty, new MonthViewAppointmentDisplayOptionsPropertyMapper(MonthView.AppointmentDisplayOptionsProperty, View));
		}
	}
	#region MonthView mappers
	public abstract class MonthViewPropertyMapperBase : SchedulerViewPropertyMapperBase<MonthView, InnerMonthView> {
		protected MonthViewPropertyMapperBase(DependencyProperty property, MonthView view)
			: base(property, view) {
		}
	}
	public class MonthViewCompressWeekendPropertyMapper : MonthViewPropertyMapperBase {
		public MonthViewCompressWeekendPropertyMapper(DependencyProperty property, MonthView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.CompressWeekendChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.CompressWeekend;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.CompressWeekend = (bool)newValue;
		}
	}
	public class MonthViewShowWeekendPropertyMapper : MonthViewPropertyMapperBase {
		public MonthViewShowWeekendPropertyMapper(DependencyProperty property, MonthView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.ShowWeekendChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.ShowWeekend;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.ShowWeekend = (bool)newValue;
		}
	}
	public class MonthViewWeekCountPropertyMapper : MonthViewPropertyMapperBase {
		public MonthViewWeekCountPropertyMapper(DependencyProperty property, MonthView view)
			: base(property, view) {
		}
		protected override bool CanUpdateOwnerProperty(SchedulerControlChangeType changeType) {
			return changeType == SchedulerControlChangeType.WeekCountChanged;
		}
		public override object GetInnerPropertyValue() {
			return InnerView.WeekCount;
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerView.WeekCount = (int)newValue;
		}
	}
	public class WeekViewAppointmentDisplayOptionsPropertyMapper : AppointmentDisplayOptionsPropertyMapperBase<WeekView, AppointmentDisplayOptions> {
		public WeekViewAppointmentDisplayOptionsPropertyMapper(DependencyProperty property, WeekView owner)
			: base(property, owner) {
		}
	}
	public class MonthViewAppointmentDisplayOptionsPropertyMapper : AppointmentDisplayOptionsPropertyMapperBase<MonthView, AppointmentDisplayOptions> {
		public MonthViewAppointmentDisplayOptionsPropertyMapper(DependencyProperty property, MonthView owner)
			: base(property, owner) {
		}
	}
	#endregion    
}
