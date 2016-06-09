#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Threading;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class CommonWaiter {
		readonly EventWaitHandle firstQueue;
		readonly EventWaitHandle secondQueue;
		CommonWaiter(EventWaitHandle firstQueue, EventWaitHandle secondQueue) {
			this.firstQueue = firstQueue;
			this.secondQueue = secondQueue;
		}
		public void WaitAny() {
			EventWaitHandle.WaitAny(new WaitHandle[] { firstQueue, secondQueue });
		}
		public static CommonWaiter Create<T1, T2>(QueueWithWaiting<T1> firstQueue, QueueWithWaiting<T2> secondQueue)
			where T1 : class
			where T2 : class {
			return new CommonWaiter(firstQueue.QueueEvent, secondQueue.QueueEvent);
		}
	}
	public class QueueWithWaiting<T> where T : class {		
		CircularBuffer<T> queue;
		object locker;
		bool isAlive = true;
		bool isEmpty = true;
		public int Count { get { return queue.Count; } }
		public EventWaitHandle QueueEvent { get; private set; }
		public QueueWithWaiting(int count) {
			queue = new CircularBuffer<T>(count);
			QueueEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
			locker = new object();
		}
		public void Stop() {
			isAlive = false;
			lock (locker) {
				if (queue.Count == 0)
					QueueEvent.Set();
			}
		}
		public void Push(List<T> tasks) {
			lock (locker) {
				if (isEmpty) {
					this.queue.Push(tasks.ToArray(), 0, tasks.Count);
					isEmpty = false;
					QueueEvent.Set();
				}
				else {
					this.queue.Push(tasks.ToArray(), 0, tasks.Count);
				}
			}
		}
		public void Push(T task) {
			lock (locker) {
				if (isEmpty) {
					this.queue.PushOne(task);
					isEmpty = false;
					QueueEvent.Set();
				}
				else {
					this.queue.PushOne(task);
				}
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021")]
		public bool WaitAndPull(out T task) {
			if (isEmpty) {
				QueueEvent.WaitOne();
			}
			if (!isAlive) {
				task = null;
				return false;
			}
			lock (locker) {
				task = this.queue.PullOne();
				if (this.queue.Count < 1) {
					isEmpty = true;
					QueueEvent.Reset();
				}
			}
			return true;
		}
	}
}
