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
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.ThemeKeys;
using System.Windows.Data;
using DevExpress.Xpf.Scheduler.Native;
#if SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Scheduler {
	#region WorkWeekView
	public class WorkWeekView : DayView, IWorkWeekViewProperties {
		static WorkWeekView() {
		}
		public WorkWeekView() {
			DefaultStyleKey = typeof(WorkWeekView);
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.WorkWeek; } }
		#region ShowFullWeek
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("WorkWeekViewShowFullWeek")]
#endif
		public bool ShowFullWeek {
			get { return (bool)GetValue(ShowFullWeekProperty); }
			set { SetValue(ShowFullWeekProperty, value); }
		}
		public static readonly DependencyProperty ShowFullWeekProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WorkWeekView, bool>("ShowFullWeek", false, (d, e) => d.OnShowFullWeekChanged(e.OldValue, e.NewValue));
		void OnShowFullWeekChanged(bool oldValue, bool newValue) {
			UpdateInnerObjectPropertyValue(ShowFullWeekProperty, oldValue, newValue);
		}
		#endregion
		#endregion
		protected override DependencyPropertySyncManager CreatePropertySyncManager() {
			return new WorkWeekViewPropertySyncManager(this);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new DayViewAppointmentDisplayOptions();
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerWorkWeekView(this,  this);
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new DayViewDateTimeScrollController(this);
		}
		protected override bool ValidateAppointmentDisplayOptions(SchedulerAppointmentDisplayOptions appointmentDisplayOptions) {
			return appointmentDisplayOptions is SchedulerDayViewAppointmentDisplayOptions;
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region WorkWeekViewContentStyleSelector
	public class WorkWeekViewContentStyleSelector : ViewContentStyleSelector {
		protected internal override Type GroupByNoneType { get { return typeof(VisualDayViewGroupByNone); } }
		protected internal override Type GroupByDateType { get { return typeof(VisualDayViewGroupByDate); } }
		protected internal override Type GroupByResourceType { get { return typeof(VisualDayViewGroupByResource); } }
	}
	#endregion
}
