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
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.DurableInstancing;
namespace DevExpress.Workflow.Store {
	internal class WorkflowStoreCompletedAsyncResult : IAsyncResult {
		private AsyncCallback callback;
		private bool endCalled;
		private ManualResetEvent manualResetEvent;
		private object state;
		private object thisLock;
		protected delegate bool AsyncCompletion(IAsyncResult result);
		public WorkflowStoreCompletedAsyncResult(AsyncCallback callback, object state) : this(callback, state, null){
		}
		public WorkflowStoreCompletedAsyncResult(AsyncCallback callback, object state, InstancePersistenceCommand command) {
			this.Command = command;
			this.callback = callback;
			this.state = state;
			this.thisLock = new object();
			if(callback != null) {
				try {
					callback(this);
				}
				catch(Exception e) {
					throw new InvalidProgramException("Async callback threw an Exception", e);
				}
			}
		}
		public object AsyncState {
			get { return state; }
		}
		public WaitHandle AsyncWaitHandle {
			get {
				if(manualResetEvent != null) {
					return manualResetEvent;
				}
				lock(thisLock) {
					if(manualResetEvent == null) {
						manualResetEvent = new ManualResetEvent(true);
					}
				}
				return manualResetEvent;
			}
		}
		public bool CompletedSynchronously {
			get { return true; }
		}
		public bool HasCallback {
			get { return this.callback != null; }
		}
		public bool IsCompleted {
			get { return true; }
		}
		public InstancePersistenceCommand Command { get; private set; }
		public void End() {
			if(endCalled) {
				throw new InvalidOperationException("WorkflowStoreCompletedAsyncResult already ended.");
			}
			endCalled = true;
			if(manualResetEvent != null) {
				manualResetEvent.Close();
			}
		}
	}
}
