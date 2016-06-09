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
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class ThreadFactoryService : IThreadFactoryService {
		const string DefaultThreadName = "DevExpress.XtraReports";
		protected class ThreadState {
			public Action Action { get; set; }
			public WindowsIdentity Identity { get; set; }
			public TaskCompletionSource<object> TaskCompletionSource { get; set; }
			public object State { get; set; }
			public ThreadState(Action action, WindowsIdentity identity, TaskCompletionSource<object> tcs, object state) {
				Guard.ArgumentNotNull(action, "action");
				Guard.ArgumentNotNull(tcs, "tcs");
				Action = action;
				Identity = identity;
				TaskCompletionSource = tcs;
				State = state;
			}
		}
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		readonly TimeSpan timeout = TimeSpan.FromMilliseconds(150);
		#region IThreadFactoryService
		public TimeSpan SleepTimeout {
			get { return timeout; }
		}
		public virtual Task Start(Action threadStart, ApartmentState apartmentState) {
			Thread thread = CreateThread();
			thread.SetApartmentState(apartmentState);
			ThreadState threadState = CreateThreadState(threadStart);
			thread.Start(threadState);
			return threadState.TaskCompletionSource.Task;
		}
		public virtual void Sleep() {
			Thread.Sleep(timeout);
		}
		#endregion
		protected virtual object CreateParentThreadStateObject() {
			return null;
		}
		protected virtual void ProcessChildThreadStateObject(object stateObject) {
		}
		protected virtual Thread CreateThread() {
			return new Thread(StartCore) {
				CurrentCulture = Thread.CurrentThread.CurrentCulture,
				CurrentUICulture = Thread.CurrentThread.CurrentUICulture,
				IsBackground = true,
				Name = DefaultThreadName
			};
		}
		ThreadState CreateThreadState(Action action) {
			return new ThreadState(
				action: action,
				identity: WindowsIdentity.GetCurrent(),
				tcs: new TaskCompletionSource<object>(),
				state: CreateParentThreadStateObject());
		}
		void StartCore(object state) {
			var threadState = (ThreadState)state;
			using(new TransientImpersonationContext(threadState.Identity)) {
				try {
					ProcessChildThreadStateObject(threadState.State);
					threadState.Action();
					threadState.TaskCompletionSource.SetResult(null);
				} catch(Exception e) {
					Logger.Info("Thread exception: " + e);
					threadState.TaskCompletionSource.SetException(e);
				}
			}
		}
	}
}
