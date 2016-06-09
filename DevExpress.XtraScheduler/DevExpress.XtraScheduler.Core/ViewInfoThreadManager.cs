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
using System.CodeDom.Compiler;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
namespace DevExpress.XtraScheduler.Internal {
	public class ViewInfoThreadManager {
		int runningThreadCount;
		ManualResetEventSlim signal;
		PendingAction pendingAction;
		public ViewInfoThreadManager() {
			this.runningThreadCount = 0;
			this.signal = new ManualResetEventSlim(true);
			this.pendingAction = null;
		}
		public void Run(Action action) {
			DoBeforeTask();
			Tuple<Action, SynchronizationContext> argsTuple = Tuple.Create<Action, SynchronizationContext>(action, SynchronizationContext.Current);
			ThreadPool.QueueUserWorkItem(a => {
				Tuple<Action, SynchronizationContext> tuple = (Tuple<Action, SynchronizationContext>)argsTuple;
				try {
					tuple.Item1();
				} catch (Exception e) {
					RethrowException(tuple.Item2, e);
				} finally {
					DoAfterTask(tuple.Item2);
				}
			}, argsTuple);
		}
		public void Run<T>(Action<T> action, T arg) {
			DoBeforeTask();
			Tuple<T, Action<T>, SynchronizationContext> argsTuple = Tuple.Create<T, Action<T>, SynchronizationContext>(arg, action, SynchronizationContext.Current);
			ThreadPool.QueueUserWorkItem(args => {
				Tuple<T, Action<T>, SynchronizationContext> tuple = (Tuple<T, Action<T>, SynchronizationContext>)args;
				try {
					tuple.Item2(tuple.Item1);
				} catch (Exception e) {
					RethrowException(tuple.Item3, e);
				} finally {
					DoAfterTask(tuple.Item3);
				}
			}, argsTuple);
		}
		public void Run<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2) {
			DoBeforeTask();
			Tuple<T1, T2, Action<T1, T2>, SynchronizationContext> argsTuple = Tuple.Create<T1, T2, Action<T1, T2>, SynchronizationContext>(arg1, arg2, action, SynchronizationContext.Current);
			ThreadPool.QueueUserWorkItem(args => {
				Tuple<T1, T2, Action<T1, T2>, SynchronizationContext> tuple = (Tuple<T1, T2, Action<T1, T2>, SynchronizationContext>)args;
				try {
					tuple.Item3(tuple.Item1, tuple.Item2);
				} catch (Exception e) {
					RethrowException(tuple.Item4, e);
				} finally {
					DoAfterTask(tuple.Item4);
				}
			}, argsTuple);
		}
		public void Run<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3) {
			DoBeforeTask();
			Tuple<T1, T2, T3, Action<T1, T2, T3>, SynchronizationContext> argsTuple = Tuple.Create<T1, T2, T3, Action<T1, T2, T3>, SynchronizationContext>(arg1, arg2, arg3, action, SynchronizationContext.Current);
			ThreadPool.QueueUserWorkItem(args => {
				Tuple<T1, T2, T3, Action<T1, T2, T3>, SynchronizationContext> tuple = (Tuple<T1, T2, T3, Action<T1, T2, T3>, SynchronizationContext>)args;
				try {
					tuple.Item4(tuple.Item1, tuple.Item2, tuple.Item3);
				} catch (Exception e) {
					RethrowException(tuple.Item5, e);
				} finally {
					DoAfterTask(tuple.Item5);
				}
			}, argsTuple);
		}
		public void Run<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
			DoBeforeTask();
			Tuple<T1, T2, T3, T4, Action<T1, T2, T3, T4>, SynchronizationContext> argsTuple = Tuple.Create<T1, T2, T3, T4, Action<T1, T2, T3, T4>, SynchronizationContext>(arg1, arg2, arg3, arg4, action, SynchronizationContext.Current);
			ThreadPool.QueueUserWorkItem(args => {
				Tuple<T1, T2, T3, T4, Action<T1, T2, T3, T4>, SynchronizationContext> tuple = (Tuple<T1, T2, T3, T4, Action<T1, T2, T3, T4>, SynchronizationContext>)args;
				try {
					tuple.Item5(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
				} catch (Exception e) {
					RethrowException(tuple.Item6, e);
				} finally {
					DoAfterTask(tuple.Item6);
				}
			}, argsTuple);
		}
		public void Run(Delegate action, params object[] args) {
			DoBeforeTask();
			Tuple<object[], Delegate, SynchronizationContext> argsTuple = Tuple.Create<object[], Delegate, SynchronizationContext>(args, action, SynchronizationContext.Current);
			ThreadPool.QueueUserWorkItem(localArgs => {
				Tuple<object[], Delegate, SynchronizationContext> tuple = (Tuple<object[], Delegate, SynchronizationContext>)localArgs;
				try {
					tuple.Item2.DynamicInvoke(tuple.Item1);
				} catch (Exception e) {
					RethrowException(tuple.Item3, e);
				} finally {
					DoAfterTask(tuple.Item3);
				}
			}, argsTuple);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool CheckRunningThreads() {
			return this.runningThreadCount == 0;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void WhenAllRunningThreadsFinish(Delegate threadsFinishedAction, params object[] args) {
			if (threadsFinishedAction == null)
				return;
			if (CheckRunningThreads())
				threadsFinishedAction.DynamicInvoke(args);
			else
				pendingAction = new PendingAction(threadsFinishedAction, args);
		}
		public void WaitForAllThreads() {
			ThreadUtils.WaitFor(this.signal.WaitHandle);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		void DoBeforeTask() {
			IncThreadCount();
			ResetSignal();
		}
		void IncThreadCount() {
			this.runningThreadCount++;
		}
		void ResetSignal() {
			this.signal.Reset();
		}
		void SetSignal() {
			this.signal.Set();
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		void DoAfterTask(SynchronizationContext syncContext) {
			DecThreadCount(syncContext);
			if (this.runningThreadCount == 0)
				DoAfterAllThreads(syncContext);
		}
		void DecThreadCount(SynchronizationContext syncContext) {
			this.runningThreadCount--;
		}
		void DoAfterAllThreads(SynchronizationContext syncContext) {
			if (pendingAction != null) {
				syncContext.Post(arg => {
					PendingAction pa = (PendingAction)arg;
					pa.Action.DynamicInvoke(pa.Args);
				}, pendingAction);
				pendingAction = null;
			}
			SetSignal();
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		void RethrowException(SynchronizationContext syncContext, Exception e) {
			syncContext.Post(ex => {
				Exception exception = (Exception)ex;
				throw new Exception(exception.Message, exception);
			}, e);
		}
	}
	public class PendingAction {
		public PendingAction(Delegate action, object[] args) {
			Action = action;
			Args = args;
		}
		public object[] Args { get; private set; }
		public Delegate Action { get; private set; }
	}
	public static class ThreadUtils {
		[DllImport("kernel32.dll", SetLastError = true)]
		[SuppressUnmanagedCodeSecurity]
		[GeneratedCode("Suppress FxCop check", "")]
		static extern Int32 WaitForSingleObject(IntPtr Handle, Int32 Wait);
		[SecuritySafeCritical]
		public static void WaitFor(WaitHandle handle) {
			WaitForSingleObject(handle.SafeWaitHandle.DangerousGetHandle(), -1);
		}
	}
}
