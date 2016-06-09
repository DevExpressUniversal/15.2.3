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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using DevExpress.Utils.Extensions.Helpers;
namespace DevExpress.XtraPivotGrid.Data {
	public interface IActionsQueue {
		event EventHandler QueueStarting;
		event EventHandler QueueCompleted;
		event EventHandler ActionStarting;
		event EventHandler ActionCompleted;
		bool IsQueueRunning { get; }
		void SetQueueContext(IActionContext context);
		void SetIsQueueAsync(bool isAsync);
		void EnqueueAction(Action action);
		void EnqueueDelayed(Action action);
		void CompleteAction();
		void RunQueue();
	}
	public interface IActionContext {
		void Invoke(Action action, bool isAsync);
	}
	public class ActionsQueue : IActionsQueue {
		#region Weak Events
		WeakEventHandler<EventArgs, EventHandler> queueStartingHandler;
		WeakEventHandler<EventArgs, EventHandler> queueCompletedHandler;
		WeakEventHandler<EventArgs, EventHandler> actionStartingHandler;
		WeakEventHandler<EventArgs, EventHandler> actionCompletedHandler;
		public event EventHandler QueueStarting { add { queueStartingHandler += value; } remove { queueStartingHandler -= value; } }
		public event EventHandler QueueCompleted { add { queueCompletedHandler += value; } remove { queueCompletedHandler -= value; } }
		public event EventHandler ActionStarting { add { actionStartingHandler += value; } remove { actionStartingHandler -= value; } }
		public event EventHandler ActionCompleted { add { actionCompletedHandler += value; } remove { actionCompletedHandler -= value; } }
		#endregion
		readonly Queue<Action> actions = new Queue<Action>();
		public ActionsQueue()
			: this(null) {
		}
		public ActionsQueue(IActionContext context)
			: this(context, true) {
		}
		public ActionsQueue(IActionContext context, bool isAsync) {
			IsActionRunning = false;
			IsQueueRunning = false;
			Context = context;
			IsAsync = isAsync;
		}
		protected IActionContext Context { get; set; }
		protected bool IsAsync { get; set; }
		protected bool IsActionRunning { get; set; }
		public bool IsQueueRunning { get; private set; }
		public void SetQueueContext(IActionContext context) {
			Context = context;
		}
		public void SetIsQueueAsync(bool isAsync) {
			IsAsync = isAsync;
		}
		public void EnqueueAction(Action action) {
			actions.Enqueue(action);
			RunQueue();
		}
		public void EnqueueDelayed(Action action) {
			actions.Enqueue(action);
		}
		public void CompleteAction() {
			IsActionRunning = false;
			this.actionCompletedHandler.SafeRaise(this, EventArgs.Empty);
			if(IsAsync || actions.Count == 0)
				RunQueue();
		}
		public void RunQueue() {
			SetQueueState();
			if(!IsActionRunning && IsQueueRunning) {
				if(IsAsync) {
					RunQueueCore();
				} else {
					while(actions.Count != 0 && !IsActionRunning && IsQueueRunning)
						RunQueueCore();
				}
			}
		}
		void SetQueueState() {
			if(actions.Count == 0) {
				if(IsQueueRunning) {
					IsQueueRunning = false;
					this.queueCompletedHandler.SafeRaise(this, EventArgs.Empty);
				}
			} else {
				if(!IsQueueRunning) {
					this.queueStartingHandler.SafeRaise(this, EventArgs.Empty);
					IsQueueRunning = true;
				}
			}
		}
		void RunQueueCore() {
			IsActionRunning = true;
			this.actionStartingHandler.SafeRaise(this, EventArgs.Empty);
			Context.Invoke(actions.Dequeue(), IsAsync);
		}
	}
}
