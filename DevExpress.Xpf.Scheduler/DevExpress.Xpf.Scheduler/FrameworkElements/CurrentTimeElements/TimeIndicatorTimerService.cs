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
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Drawing.Native {
	public class TimeIndicatorTimerService {
		static TimeIndicatorTimerService instance;
		public static TimeIndicatorTimerService Instance {
			get {
				if (instance == null)
					instance = new TimeIndicatorTimerService();
				return instance;
			}
		}
		public static void ForceUpdate() {
			Instance.RaiseTimerTick();
		}
		DevExpress.Utils.Timer timer;
		protected TimeIndicatorTimerService() {
			Now = DateTime.Now;
		}
		public DateTime Now { get; protected set; }
		WeakEventHandler<TimeIndicatorTickEventArgs, EventHandler<TimeIndicatorTickEventArgs>> onTimerTick;
		public event EventHandler<TimeIndicatorTickEventArgs> TimerTick {
			add {
				onTimerTick += value;
				ResetTimer();
			}
			remove {
				onTimerTick -= value;
				ResetTimer();
			}
		}
		void ResetTimer() {
			if (onTimerTick == null)
				DeleteTimer();
			CreateTimer();
		}
		void CreateTimer() {
			if (this.timer != null)
				return;
			this.timer = new DevExpress.Utils.Timer();
			this.timer.Interval = new TimeSpan(0, 0, 30);
			this.timer.Tick += OnTimerTick;
			this.timer.Start();
		}
		void DeleteTimer() {
			if (this.timer == null)
				return;
			this.timer.Stop();
			this.timer.Dispose();
			this.timer = null;
		}
		void OnTimerTick(object sender, EventArgs e) {
			Now = DateTime.Now;
			RaiseTimerTick();
		}
		protected void RaiseTimerTick() {
			if (onTimerTick == null)
				return;
			TimeIndicatorTickEventArgs ea = new TimeIndicatorTickEventArgs(Now);
			onTimerTick.Raise(this, ea);
		}		
	}
	public class TimeIndicatorTickEventArgs : EventArgs {
		public TimeIndicatorTickEventArgs(DateTime now) {
			Now = now;
		}
		public DateTime Now { get; private set; }
	}   
}
