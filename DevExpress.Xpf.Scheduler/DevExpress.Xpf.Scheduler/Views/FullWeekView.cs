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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Scheduler{
	public class FullWeekView : DayView, IFullWeekViewProperties {
		static FullWeekView() {
			EnabledProperty.OverrideMetadata(typeof(FullWeekView), new FrameworkPropertyMetadata(false));
		}
		public FullWeekView() {
			DefaultStyleKey = typeof(FullWeekView);
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override SchedulerViewType Type { get { return SchedulerViewType.FullWeek; } }
		#endregion
		protected override DependencyPropertySyncManager CreatePropertySyncManager() {
			return new DayViewPropertySyncManager(this);
		}
		protected internal override AppointmentDisplayOptions CreateAppointmentDisplayOptionsCore() {
			return new DayViewAppointmentDisplayOptions();
		}
		protected internal override InnerSchedulerViewBase CreateInnerView() {
			return new InnerFullWeekView(this, this);
		}
		protected internal override ViewDateTimeScrollController CreateDateTimeScrollController() {
			return new DayViewDateTimeScrollController(this);
		}
		protected override bool ValidateAppointmentDisplayOptions(SchedulerAppointmentDisplayOptions appointmentDisplayOptions) {
			return appointmentDisplayOptions is SchedulerDayViewAppointmentDisplayOptions;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Drawing {
	#region FullWeekViewContentStyleSelector
	public class FullWeekViewContentStyleSelector : ViewContentStyleSelector {
		protected internal override Type GroupByNoneType { get { return typeof(VisualDayViewGroupByNone); } }
		protected internal override Type GroupByDateType { get { return typeof(VisualDayViewGroupByDate); } }
		protected internal override Type GroupByResourceType { get { return typeof(VisualDayViewGroupByResource); } }
	}
	#endregion
}
