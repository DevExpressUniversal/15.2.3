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

using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Xpf.Scheduler.Commands {
	public class NavigationButtonUICommand : SchedulerUICommand {
		XtraScheduler.Resource resource;
		TimeInterval interval;
		public NavigationButtonUICommand(TimeInterval interval, XtraScheduler.Resource resource)
			: base(SchedulerCommandId.None) {
			this.interval = interval;
			this.resource = resource;
		}
		public TimeInterval Interval { get { return interval; } }
		public Resource Resource { get { return resource; } }
		protected override bool CanExecuteCore(SchedulerControl control) {
			return Interval != null && Resource != null;
		}
		protected override void ExecuteCore(SchedulerControl control) {
			control.BeginUpdate();
			try {
				control.ActiveView.GotoTimeInterval(interval);
			}
			finally {
				control.EndUpdate();
			}
			Appointment apt = resource.Id == ResourceBase.Empty.Id ? control.ActiveView.FilteredAppointments.FindAppointmentExact(interval) : control.ActiveView.FilteredAppointments.FindAppointmentExact(interval, resource.Id);
			if(apt != null)
				control.ActiveView.SelectAppointment(apt, resource);
		}
	}
}
