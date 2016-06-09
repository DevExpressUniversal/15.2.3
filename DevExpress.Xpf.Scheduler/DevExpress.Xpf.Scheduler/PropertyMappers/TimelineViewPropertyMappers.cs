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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Native {
	public class TimelineViewPropertySyncManager : SchedulerViewPropertySyncManager<TimelineView> {
		public TimelineViewPropertySyncManager(TimelineView view)
			: base(view) {
		}
		public override void Register() {
			base.Register();
			PropertyMapperTable.RegisterPropertyMapper(TimelineView.AppointmentDisplayOptionsProperty, new TimelineViewAppointmentDisplayOptionsPropertyMapper(TimelineView.AppointmentDisplayOptionsProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(TimelineView.SelectionBarProperty, new SchedulerSelectionBarPropertyMapper(TimelineView.SelectionBarProperty, View));
			PropertyMapperTable.RegisterPropertyMapper(TimelineView.WorkTimeProperty, new TimelineViewWorkTimePropertyMapper(TimelineView.WorkTimeProperty, View));
		}
	}
	#region TimelineView mappers
	public abstract class TimelineViewPropertyMapperBase : SchedulerViewPropertyMapperBase<TimelineView, InnerTimelineView> {
		protected TimelineViewPropertyMapperBase(DependencyProperty property, TimelineView view)
			: base(property, view) {
		}
	}
	public class TimelineViewAppointmentDisplayOptionsPropertyMapper : AppointmentDisplayOptionsPropertyMapperBase<TimelineView, AppointmentDisplayOptions> {
		public TimelineViewAppointmentDisplayOptionsPropertyMapper(DependencyProperty property, TimelineView owner)
			: base(property, owner) {
		}
	}
	public class SchedulerSelectionBarPropertyMapper : ViewBaseInnerObjectContainerPropertyMapper<TimelineView, SelectionBarOptions> {
		public SchedulerSelectionBarPropertyMapper(DependencyProperty property, TimelineView owner)
			: base(property, owner) {
		}
		public override object GetInnerPropertyValue() {
			InnerTimelineView innerView = (InnerTimelineView)PropertyOwner.InnerView;
			return innerView.SelectionBar;
		}
	}
	public class TimelineViewWorkTimePropertyMapper : TimelineViewPropertyMapperBase {
		public TimelineViewWorkTimePropertyMapper(DependencyProperty property, TimelineView view)
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
	#endregion
}
