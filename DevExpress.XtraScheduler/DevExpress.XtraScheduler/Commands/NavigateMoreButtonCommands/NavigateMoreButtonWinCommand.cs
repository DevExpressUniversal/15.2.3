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

using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Commands {
	public class NavigateMoreButtonWinCommand : NavigateMoreButtonCommand {
		SchedulerControl control;
		public NavigateMoreButtonWinCommand(SchedulerControl control, TimeInterval interval, Resource resource, DateTime viewStart )
			: base(control.InnerControl, interval, resource, viewStart) {
			this.control = control;
		}
		public SchedulerControl SchedulerControl { get { return control; } }
		protected internal override void SetDayViewProperties(DateTime targetViewStart) {
			base.SetDayViewProperties(targetViewStart);
			InnerDayView innerDayView = (InnerDayView)InnerControl.ActiveView;
			TimeSpan actualTime = CalcActualTime(innerDayView, targetViewStart);
			DayView dayView = (DayView)SchedulerControl.ActiveView;
			dayView.TopRowTime = actualTime;
		}
		protected override void SetSelection() {
			SchedulerControl.ActiveView.SetSelection(Interval, Resource);
		}
		protected internal virtual TimeSpan CalcActualTime(InnerDayView dayView, DateTime targetViewStart) {
			TimeSpan buttonTimeOfDay = targetViewStart.TimeOfDay;
			if (ShouldSetPreviousDay(dayView.VisibleTime, buttonTimeOfDay))
				buttonTimeOfDay += TimeSpan.FromDays(1);
			return buttonTimeOfDay;
		}
	}
}
