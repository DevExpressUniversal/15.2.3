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
using System.Threading.Tasks;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public static class AsyncHelper {
		public static void DoEvents(Dispatcher dispacher, DispatcherPriority? priority = null) {
			if(priority == null)
				dispacher.Invoke((Action)(() => { }));
			else
				dispacher.Invoke((Action)(() => { }), priority.Value);
		}
		public static ManualResetEvent DoWithDispatcher(Dispatcher dispacher, Action action, DispatcherPriority? priority = null) {
			return DoWithDispatcherCore(dispacher, action, priority, false);
		}
		public static ManualResetEvent DoWithDispatcherForce(Dispatcher dispacher, Action action, DispatcherPriority? priority = null) {
			return DoWithDispatcherCore(dispacher, action, priority, true);
		}
		public static Func<Thread> Create(Func<CancellationToken, IEnumerable<ManualResetEvent>> asyncAction) {
			AsyncHelperImpl<object, object> asyncHelper = new AsyncHelperImpl<object, object>(a => a, (s, a, c) => asyncAction(c));
			return () => asyncHelper.Do(null);
		}
		public static Func<TAsyncParam, Thread> Create<TAsyncParam>(Func<TAsyncParam, CancellationToken, IEnumerable<ManualResetEvent>> asyncAction) {
			AsyncHelperImpl<TAsyncParam, TAsyncParam> asyncHelper = new AsyncHelperImpl<TAsyncParam, TAsyncParam>(a => a, (s, a, c) => asyncAction(a, c));
			return s => asyncHelper.Do(s);
		}
		public static Func<TStartParam, Thread> Create<TStartParam, TAsyncParam>(Func<TStartParam, TAsyncParam> startAction, Func<TStartParam, TAsyncParam, CancellationToken, IEnumerable<ManualResetEvent>> asyncAction) {
			AsyncHelperImpl<TStartParam, TAsyncParam> asyncHelper = new AsyncHelperImpl<TStartParam, TAsyncParam>(startAction, asyncAction);
			return s => asyncHelper.Do(s);
		}
		static ManualResetEvent DoWithDispatcherCore(Dispatcher dispacher, Action action, DispatcherPriority? priority, bool force) {
			if(!force && (dispacher == null || dispacher.CheckAccess())) {
				action();
				return null;
			} else {
				ManualResetEvent done = new ManualResetEvent(false);
				if(priority == null)
					dispacher.BeginInvoke((Action<Action, ManualResetEvent>)DoAction, action, done);
				else
					dispacher.BeginInvoke((Action<Action, ManualResetEvent>)DoAction, priority.Value, action, done);
				return done;
			}
		}
		static void DoAction(Action action, ManualResetEvent done) {
			action();
			done.Set();
		}
		sealed class AsyncHelperImpl<TStartParam, TAsyncParam> {
			readonly Func<TStartParam, TAsyncParam> startAction;
			readonly Func<TStartParam, TAsyncParam, CancellationToken, IEnumerable<ManualResetEvent>> asyncAction;
			Thread currentAsyncTask = null;
			CancellationTokenSource currentAsyncTaskCancellationTokenSource = null;
			public AsyncHelperImpl(Func<TStartParam, TAsyncParam> startAction, Func<TStartParam, TAsyncParam, CancellationToken, IEnumerable<ManualResetEvent>> asyncAction) {
				this.startAction = startAction;
				this.asyncAction = asyncAction;
			}
			public Thread Do(TStartParam startParam) {
				if(currentAsyncTaskCancellationTokenSource != null)
					currentAsyncTaskCancellationTokenSource.Cancel();
				currentAsyncTaskCancellationTokenSource = new CancellationTokenSource();
				Thread oldAsyncTask = currentAsyncTask;
				TAsyncParam asyncParam = startAction(startParam);
				currentAsyncTask = new Thread(() => DoAsync(startParam, asyncParam, oldAsyncTask, currentAsyncTaskCancellationTokenSource.Token));
				currentAsyncTask.Start();
				return currentAsyncTask;
			}
			void DoAsync(TStartParam startParam, TAsyncParam asyncParam, Thread oldAsyncTask, CancellationToken cancellationToken) {
				if(oldAsyncTask != null)
					oldAsyncTask.Join();
				if(cancellationToken.IsCancellationRequested) return;
				foreach(ManualResetEvent deferredAction in asyncAction(startParam, asyncParam, cancellationToken) ?? new ManualResetEvent[] { }) {
					if(cancellationToken.IsCancellationRequested) return;
					if(deferredAction == null) continue;
					deferredAction.WaitOne(1000);
				}
			}
		}
	}
}
