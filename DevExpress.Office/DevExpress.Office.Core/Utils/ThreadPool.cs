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
using System.Threading;
namespace DevExpress.Office.Utils {
	#region ThreadPoolItem
	public class ThreadPoolItem : IDisposable {
		#region Fields
		const int CanExecuteEvent = 0;
		const int AbortExecutionEvent = 1;
		EventWaitHandle[] handles;
		AutoResetEvent complete;
		WaitCallback currentJob;
		#endregion
		public ThreadPoolItem() {
			this.handles = new EventWaitHandle[2];
			this.handles[CanExecuteEvent] = new AutoResetEvent(false);
			this.handles[AbortExecutionEvent] = new ManualResetEvent(false);
			complete = new AutoResetEvent(false);
			CreateThread(); 
		}
		#region Properties
		public WaitCallback CurrentJob { get { return currentJob; } set { currentJob = value; } }
		public EventWaitHandle CanExecute { get { return handles[CanExecuteEvent]; } }
		public EventWaitHandle AbortExecution { get { return handles[AbortExecutionEvent]; } }
		public EventWaitHandle Complete { get { return complete; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (handles != null) {
					AbortExecution.Set();
					CanExecute.Dispose();
					Complete.Dispose();
					handles = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		void CreateThread() {
			Thread thread = new Thread(Worker);
			thread.IsBackground = true;
#if !SL && !DXRESTRICTED
			thread.Priority = ThreadPriority.Lowest;
#endif
			thread.Start();
		}
		protected internal virtual void Worker() {
			try {
				for (; ; ) {
					int index = WaitHandle.WaitAny(handles);
					if (index != 0)
						return;
					try {
						CurrentJob(null);
					}
					catch {
					}
					Complete.Set();
				}
			}
			catch {
			}
		}
	}
	#endregion
	#region OfficeThreadPool
	public class OfficeThreadPool : IDisposable {
		#region Fields
		int maxThreadsCount;
		Thread poolManager;
		List<ThreadPoolItem> threads;
		Queue<ThreadPoolItem> freeThreads;
		Queue<WaitCallback> jobQueue;
		EventWaitHandle[] completionHandles;
		#endregion
		public OfficeThreadPool() {
			this.maxThreadsCount = 3;
			this.jobQueue = new Queue<WaitCallback>();
			Start();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				Shutdown();
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		public void QueueJob(WaitCallback job) {
			ThreadPoolItem poolItem = GetAvailablePoolItem();
			if (poolItem != null)
				ExecuteJob(poolItem, job);
			else {
				lock (jobQueue) {
					jobQueue.Enqueue(job);
				}
			}
		}
		protected internal virtual void Start() {
			CreatePoolThreads();
			CreateCompletionHandles();
			StartPoolManager();
		}
		public void Reset() {
			Shutdown();
			Start();
		}
		protected internal virtual void Shutdown() {
			StopPoolManager();
			jobQueue.Clear();
			DisposePoolThreads();
			DisposeCompletionHandles();
		}
		protected internal virtual void CreatePoolThreads() {
			this.threads = new List<ThreadPoolItem>(maxThreadsCount);
			this.freeThreads = new Queue<ThreadPoolItem>(maxThreadsCount);
			for (int i = 0; i < maxThreadsCount; i++) {
				ThreadPoolItem poolItem = new ThreadPoolItem();
				threads.Add(poolItem);
				freeThreads.Enqueue(poolItem);
			}
		}
		protected internal virtual void DisposePoolThreads() {
			int count = threads.Count;
			for (int i = 0; i < count; i++)
				threads[i].Dispose();
			threads.Clear();
			freeThreads.Clear();
		}
		protected internal virtual void CreateCompletionHandles() {
			int count = threads.Count;
			this.completionHandles = new EventWaitHandle[threads.Count + 1];
			for (int i = 0; i < count; i++)
				completionHandles[i] = threads[i].Complete;
			completionHandles[count] = new ManualResetEvent(false); 
		}
		protected internal virtual void DisposeCompletionHandles() {
			completionHandles[completionHandles.Length - 1].Dispose();
		}
		WaitCallback GetNextJobFromQueue() {
			lock (jobQueue) {
				if (jobQueue.Count > 0)
					return jobQueue.Dequeue();
				else
					return null;
			}
		}
		protected internal virtual void EnqueueNextJob(int poolItemIndex) {
			WaitCallback nextJob = GetNextJobFromQueue();
			ThreadPoolItem poolItem = threads[poolItemIndex];
			poolItem.CurrentJob = nextJob;
			if (nextJob != null)
				ExecuteJob(poolItem, nextJob);
			else
				MakeAvailable(poolItem);
		}
		protected internal virtual void ExecuteJob(ThreadPoolItem poolItem, WaitCallback job) {
			poolItem.CurrentJob = job;
			poolItem.CanExecute.Set();
		}
		protected internal virtual ThreadPoolItem GetAvailablePoolItem() {
			lock (freeThreads) {
				if (freeThreads.Count > 0)
					return freeThreads.Dequeue();
				else
					return null;
			}
		}
		protected internal virtual void MakeAvailable(ThreadPoolItem poolItem) {
			lock (freeThreads) {
				freeThreads.Enqueue(poolItem);
			}
		}
		#region Pool Manager
		protected internal virtual void StartPoolManager() {
			this.poolManager = new Thread(PoolManagerWorker);
			this.poolManager.IsBackground = true;
			this.poolManager.Start();
		}
		protected internal virtual void StopPoolManager() {
			completionHandles[completionHandles.Length - 1].Set();
			poolManager.Join();
		}
		protected internal virtual void PoolManagerWorker() {
			for (; ; ) {
				try {
					int index = WaitHandle.WaitAny(completionHandles);
					if (index == threads.Count) 
						break;
					if (index >= 0 && index < completionHandles.Length)
						EnqueueNextJob(index);
				}
				catch {
				}
			}
		}
		#endregion
	}
	#endregion
}
