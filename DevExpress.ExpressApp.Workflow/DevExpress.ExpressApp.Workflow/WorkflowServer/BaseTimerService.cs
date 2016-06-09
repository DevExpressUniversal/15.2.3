#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Workflow;
namespace DevExpress.ExpressApp.Workflow.Server {
	public abstract class BaseTimerService : WorkflowServerService, IDisposable {
#if DebugTest
		public class SimpleDelayManager {
#else 
		protected internal class SimpleDelayManager {
#endif
			public virtual void Sleep(TimeSpan delayPeriod) {
				Thread.Sleep(delayPeriod);
			}
		}
		protected internal Thread workingThread;
		private IObjectSpaceProvider objectSpaceProvider;
		private object locker = new object();
		protected virtual bool CanCloseThread() {
			return true;
		}
		private void CloseThread() {
			TracerHelper.TraceMethodEnter(this, "CloseThread");
			if(workingThread != null) {
				CancellationPending = true;
				lock(locker) {
					workingThread.Abort();
					workingThread = null;
				}
			}
			TracerHelper.TraceMethodExit(this, "CloseThread");
		}
		private void HostManager_HostsClosing(object sender, EventArgs e) {
			TracerHelper.TraceMethodEnter(this, "HostManager_HostsClosing");
			if(CanCloseThread()) {
				CloseThread();
			}
			TracerHelper.TraceMethodExit(this, "HostManager_HostsClosing");
		}
		private void HostManager_HostsOpened(object sender, EventArgs e) {
			if(workingThread == null) {
				TracerHelper.TraceMethodEnter(this, "StartWorkingThread");
				CancellationPending = false;
				locker = new object();
				workingThread = new Thread(ThreadMethod);
				workingThread.Start();
				TracerHelper.TraceMethodExit(this, "StartWorkingThread");
			}
		}
		private void ThreadMethod() {
			try {
				while(true) {
					TracerHelper.TraceMethodEnter(this, "ThreadMethod_RoundX");
					TracerHelper.TraceVariableValue("DelayThenExecute", DelayThenExecute.ToString());
					TracerHelper.TraceVariableValue("DelayPeriod", DelayPeriod.ToString());
					if(DelayThenExecute) {
						DelayManager.Sleep(DelayPeriod);
					}
					lock(locker) {
						TracerHelper.TraceMethodEnter(this, "OnTimer");
						OnTimer();
						TracerHelper.TraceMethodExit(this, "OnTimer");
					}
					if(!DelayThenExecute) {
						DelayManager.Sleep(DelayPeriod);
					}
					TracerHelper.TraceMethodExit(this, "ThreadMethod_RoundX");
				}
			}
			catch(ThreadAbortException) { }
			catch(Exception e) {
				if(!OnCustomHandleException(e)) {
					throw;
				}
			}
		}
		protected internal SimpleDelayManager DelayManager { get; set; }
		protected internal bool DelayThenExecute { get; set; }
		protected override void OnInitialized() {
			base.OnInitialized();
			HostManager.HostsOpened += new EventHandler(HostManager_HostsOpened);
			HostManager.HostsClosing += new EventHandler(HostManager_HostsClosing);
		}
		public BaseTimerService(TimeSpan delayPeriod) : this(delayPeriod, null) { }
		public BaseTimerService(TimeSpan delayPeriod, IObjectSpaceProvider objectSpaceProvider) {
			this.DelayManager = new SimpleDelayManager();
			this.objectSpaceProvider = objectSpaceProvider;
			this.DelayPeriod = delayPeriod;
		}
		public void Dispose() {
			CloseThread();
		}
		public abstract void OnTimer();
		public TimeSpan DelayPeriod { get; set; }
		public bool CancellationPending { get; set; }
		public IObjectSpaceProvider ObjectSpaceProvider {
			get { return objectSpaceProvider ?? GetService<IObjectSpaceProvider>(); }
			set { objectSpaceProvider = value; }
		}
#if DebugTest
		public SimpleDelayManager DebugTest_DelayManager { 
			get { return DelayManager;}
			set { DelayManager = value; }
		}
		public Thread DebugTest_WorkingThread {
			get { return workingThread; }
		}
#endif
	}
}
