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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.Printing.Core;
namespace DevExpress.XtraPrinting {
	public interface IDelayer {
		void Execute(Action action);
		void Abort();
	}
	public interface IDelayerFactory {
		IDelayer Create(TimeSpan interval);
	}
	[Obsolete("Use the SynchronizedDelayerFactory class instead.")]
	public class DispatcherTimerDelayerFactory : IDelayerFactory {
		public IDelayer Create(TimeSpan interval) {
			return new DispatcherTimerDelayer(interval);
		}
	}
	[Obsolete("Use the SynchronizedDelayerFactory class instead.")]
	public class ThreadingTimerDelayerFactory : IDelayerFactory {
		public IDelayer Create(TimeSpan interval) {
			return new ThreadingTimerDelayer(interval);
		}
	}
	public class SynchronizedDelayerFactory : IDelayerFactory {
		readonly SynchronizationContext syncContext;
		public SynchronizedDelayerFactory() : this(SynchronizationContext.Current) {
		}
		public SynchronizedDelayerFactory(SynchronizationContext syncContext) {
			this.syncContext = syncContext;
		}
		public IDelayer Create(TimeSpan interval) {
			return new SynchronizedDelayer(interval, syncContext);
		}
	}
	public class SynchronizedDelayer : IDelayer {
		static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1);
		readonly TimeSpan interval;
		readonly object syncLock = new object();
		readonly SynchronizationContext syncContext;
		Timer timer;
		public SynchronizedDelayer(TimeSpan interval, SynchronizationContext syncContext) {
			this.interval = interval;
			this.syncContext = syncContext;
		}
		public void Abort() {
			lock(syncLock) {
				if(timer != null) {
					RemoveTimer();
				}
			}
		}
		public void Execute(Action action) {
			if(action == null)
				throw new ArgumentNullException("action");
			lock(syncLock) {
				if(timer != null)
					throw new InvalidOperationException();
				timer = new Timer(TimerCallback, action, interval, InfiniteTimeSpan);
			}
		}
		private void TimerCallback(object state) {
			lock(syncLock) {
				if(timer == null)   
					return;
				RemoveTimer();
				ExecuteAction((Action)state);
			}
		}
		void RemoveTimer() {
			timer.Dispose();
			timer = null;
		}
		void ExecuteAction(Action action) {
			SendOrPostCallback callback = x => action();
			if(syncContext != null) {
				syncContext.Post(callback, null);
			} else {
				callback(null);
			}
		}
	}
}
