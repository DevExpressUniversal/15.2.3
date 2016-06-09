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
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Scheduler.Commands {
	public class XpfSwitchTimeScaleCommand : SwitchTimeScaleCommand {
		public XpfSwitchTimeScaleCommand(ISchedulerCommandTarget target, InnerDayView view, TimeSlot slot) : base(target, view, slot){
		}
		protected internal override void ExecuteCore() {
			SyncTopRowTime(InnerControl);
			base.ExecuteCore();
		}
		public static void SyncTopRowTime(InnerSchedulerControl innerControl) {
			SchedulerControl xpfScheduler = innerControl.Owner as SchedulerControl;
			if (xpfScheduler == null)
				return;
			DayView xpfView = xpfScheduler.ActiveView as DayView;
			if (xpfView == null)
				return;
			TimeSpan topRowTime = xpfView.TopRowTime;
			xpfScheduler.Dispatcher.BeginInvoke(new Action(() => {
				xpfView.TopRowTime = topRowTime;
				xpfScheduler.UpdateDateTimeScrollBarValue();
			})
#if !SL
			, System.Windows.Threading.DispatcherPriority.Background
#endif
			);
		}
	}
}
