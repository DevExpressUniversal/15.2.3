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
namespace DevExpress.Web.Office.Internal {
	public interface IWorksessionsForeachProxy {
		void ForEachWorkSession(Action<IWorkSession> action);
	}
	public class WorksessionsForeachProxy : IWorksessionsForeachProxy {
		public virtual void ForEachWorkSession(Action<IWorkSession> action) {
			WorkSessions.ForEachWorkSession(workSessionBase => action(workSessionBase));
		}
	}
	public interface IWorkSessionAutoProcessingProvider {
		void Process(IWorkSession workSession);
		bool ShouldBeProcessed(IWorkSession workSession, DateTime now);
	}
	public sealed class WorkSessionProcessing {
		static object syncRoot = new Object();
		private WorkSessionProcessing() { }
		static WorkSessionProcessing() {
			WorkSessionProcessingSchedulerInitializator.RegisterProcessProviders();
			HibernateAllDocumentsOnApplicationEnd = true;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}
		static void CurrentDomain_DomainUnload(object sender, EventArgs e) {
			if(EnableHibernation && HibernateAllDocumentsOnApplicationEnd) {
				WorkSessions.ForEachWorkSession(workSessionBase => 
					((IWorkSessionHibernateProvider)WorkSessionProcessing.WorkSessionHibernateProviderSingleton).Hibernate(workSessionBase));
			}
		}
		public static bool EnableHibernation { 
			get { return HibernationSettings.Enabled; } 
			set { 
				if(value == HibernationSettings.Enabled) return;
				HibernationSettings.Enabled = value; 
				((IWorkSessionHibernateProvider)WorkSessionHibernateProviderSingleton).OnDisposingSettingChanged();
				if(value)
					Start();
			} 
		}
		public static TimeSpan HibernateTimeout { get { return HibernationSettings.HibernateTimeout; } set { HibernationSettings.HibernateTimeout = value; } }
		public static TimeSpan HibernatedDocumentsDisposeTimeout {
			get { return HibernationSettings.HibernatedDocumentsDisposeTimeout; } 
			set { 
				if(value == HibernationSettings.HibernatedDocumentsDisposeTimeout) return;
				HibernationSettings.HibernatedDocumentsDisposeTimeout = value; 
				((IWorkSessionHibernateProvider)WorkSessionHibernateProviderSingleton).OnDisposingSettingChanged();
			} 
		}
		public static string HibernationStoragePath { 
			get { return HibernationSettings.StoragePath; } 
			set { 
				if(value == HibernationSettings.StoragePath) return;
				HibernationSettings.StoragePath = value; 
				((IWorkSessionHibernateProvider)WorkSessionHibernateProviderSingleton).OnDisposingSettingChanged();
			} 
		}
		public static bool HibernateAllDocumentsOnApplicationEnd {
			get { return HibernationSettings.HibernateAllDocumentsOnApplicationEnd; } 
			set { HibernationSettings.HibernateAllDocumentsOnApplicationEnd = value; } 
		}
		private static volatile WorkSessionProcessingScheduler WorkSessionProcessingSchedulerInstance;
		static WorkSessionProcessingScheduler WorkSessionProcessingSchedulerSingleton {
			get {
				if(WorkSessionProcessingSchedulerInstance == null) {
					lock(syncRoot) {
						if(WorkSessionProcessingSchedulerInstance == null) {
							var schedulingStrategy = new WorkSessionsHalfQueueSchedulingStrategy();
							WorkSessionProcessingSchedulerInstance = new WorkSessionProcessingScheduler(schedulingStrategy);
							WorkSessionProcessingSchedulerInitializator.RegisterProcessProviders();
						}
					}
				}
				return WorkSessionProcessingSchedulerInstance;
			}
		}
		private static volatile IWorkSessionAutoProcessingProvider WorkSessionAutoSaveProviderInstance;
		public static IWorkSessionAutoProcessingProvider WorkSessionAutoSaveProviderSingleton {
			get {
				if(WorkSessionAutoSaveProviderInstance == null) {
					lock(syncRoot) {
						if(WorkSessionAutoSaveProviderInstance == null) {
							WorkSessionAutoSaveProviderInstance = new WorkSessionAutoSaveProvider();
						}
					}
				}
				return WorkSessionAutoSaveProviderInstance;
			}
		}
		private static volatile IWorkSessionAutoProcessingProvider WorkSessionHibernateProviderInstance;
		public static IWorkSessionAutoProcessingProvider WorkSessionHibernateProviderSingleton {
			get {
				if(WorkSessionHibernateProviderInstance == null) {
					lock(syncRoot) {
						if(WorkSessionHibernateProviderInstance == null) {
							WorkSessionHibernateProviderInstance = new LocalFSWorkSessionHibernationProvider();
						}
					}
				}
				return WorkSessionHibernateProviderInstance;
			}
		}
		public static void SetWorkSessionHibernateProvider(IWorkSessionHibernateProvider workSessionHibernateProvider) {
			lock(syncRoot) {
				WorkSessionProcessingScheduler.UnRegisterProcessProviders(WorkSessionHibernateProviderSingleton);
				WorkSessionHibernateProviderInstance = workSessionHibernateProvider;
				WorkSessionProcessingScheduler.RegisterProcessProviders(WorkSessionHibernateProviderSingleton);
			}
		}
		public static void SetWorkSessionsProcessingSchedulingStrategy(IWorkSessionsProcSchedulingStrategy workSessionsProcSchedulingStrategy) {
			lock(syncRoot) {
				WorkSessionProcessingScheduler.UnRegisterProcessProviders(WorkSessionHibernateProviderSingleton);
				WorkSessionProcessingSchedulerInstance = new WorkSessionProcessingScheduler(workSessionsProcSchedulingStrategy);
				WorkSessionProcessingScheduler.RegisterProcessProviders(WorkSessionHibernateProviderSingleton);
			}
		}
		public static void Start() {
			lock(syncRoot) {
				WorkSessionProcessingSchedulerSingleton.Start();
			}
		}
		public static void Stop() {
			lock(syncRoot) {
				WorkSessionProcessingSchedulerSingleton.Stop();
			}
		}
	}
	public class WorkSessionTask {
		IWorkSession workSession;
		Action<IWorkSession> action;
		public WorkSessionTask(IWorkSession workSession, Action<IWorkSession> action) {
			this.workSession = workSession;
			this.action = action;
		}
		public IWorkSession WorkSession { get { return workSession; } }
		public bool Act() {
			bool success = true;
			try {
				action(workSession);
			} catch(Exception exc) {
				DevExpress.Web.Internal.CommonUtils.RaiseCallbackErrorInternal(this, exc);
				success = false;
			}
			return success;
		}
	}
	public static  class WorkSessionProcessingSchedulerInitializator {
		static object syncRoot = new Object();
		public static void RegisterProcessProviders() {
			lock(syncRoot) {
				WorkSessionProcessingScheduler.RegisterProcessProviders(WorkSessionProcessing.WorkSessionAutoSaveProviderSingleton);
				WorkSessionProcessingScheduler.RegisterProcessProviders(WorkSessionProcessing.WorkSessionHibernateProviderSingleton);
			}
		}
	}
	class WorkSessionProcessingScheduler {
		const string timerCacheKey = "DevexpressASPNETDocumentsProcessingTimer";
		public TimeSpan ServiceTimeout { get; set; }
		public TimeSpan CurrentServiceTimeout { get; set; }
		IWorksessionsForeachProxy worksessions;
		IWorkSessionsProcSchedulingStrategy strategy;
		Queue<WorkSessionTask> queue = new Queue<WorkSessionTask>();
		bool active = false;
		public WorkSessionProcessingScheduler(IWorkSessionsProcSchedulingStrategy strategy) : this(new WorksessionsForeachProxy(), strategy) { }
		public WorkSessionProcessingScheduler(IWorksessionsForeachProxy worksessions, IWorkSessionsProcSchedulingStrategy strategy) {
			this.worksessions = worksessions;
			this.strategy = strategy;
			ServiceTimeout = strategy.ServiceTimeout;
		}
		static List<IWorkSessionAutoProcessingProvider> processProviders = new List<IWorkSessionAutoProcessingProvider>();
		internal static void RegisterProcessProviders(IWorkSessionAutoProcessingProvider processProvider) {
			if(!processProviders.Contains(processProvider))
				processProviders.Add(processProvider);
		}
		internal static void UnRegisterProcessProviders(IWorkSessionAutoProcessingProvider processProvider) {
			processProviders.Remove(processProvider);
		}
		public void Start() {
			if(this.active) return;
			this.active = true;
			CurrentServiceTimeout = ServiceTimeout;
			OnServiceTimeout();
		}
		public void Stop() {
			this.active = false;
		}
		void SetServiceTimer(int serviceTimeoutInSecont) {
			SetServiceTimer(TimeSpan.FromSeconds(serviceTimeoutInSecont));
		}
		public virtual void SetServiceTimer(TimeSpan timeout) {
			if(!this.active) return;
			if(System.Web.HttpRuntime.Cache[timerCacheKey] != null)
				return;
			System.Web.HttpRuntime.Cache.Insert(
				timerCacheKey,
				string.Empty,
				null,
				System.Web.Caching.Cache.NoAbsoluteExpiration,
				timeout,
				System.Web.Caching.CacheItemPriority.NotRemovable,
				(key, value, reason) => OnServiceTimeout());
		}
		public void OnServiceTimeout() {
			if(!this.active) return;
			int previousQueueSize = queue.Count;
			FillQueue(worksessions, queue);
			int currentQueueSize = queue.Count;
			int delta = currentQueueSize - previousQueueSize;
			ProcessQueue(queue, delta);
			int nextServiceTimeout = strategy.GetNextServiceTimeout(queue, delta, CurrentServiceTimeout.Seconds);
			SetServiceTimer(nextServiceTimeout);
		}
		static void FillQueue(IWorksessionsForeachProxy worksessions, Queue<WorkSessionTask> taskQueue) {
			DateTime now = DateTime.Now;
			worksessions.ForEachWorkSession(workSession => {
				foreach(var processProvider in processProviders) {
					if(processProvider.ShouldBeProcessed(workSession, now))
						EnqueueTask(taskQueue, workSession, ws => processProvider.Process(ws));
				}
			});
		}
		static void EnqueueTask(Queue<WorkSessionTask> taskQueue, IWorkSession workSession, Action<IWorkSession> action) {
			if(!workSessionIsAlreadyInQueue(taskQueue, workSession)) {
				var task = new WorkSessionTask(workSession, action);
				taskQueue.Enqueue(task);
			}
		}
		private static bool workSessionIsAlreadyInQueue(Queue<WorkSessionTask> taskQueue, IWorkSession workSession) {
			foreach(var i in taskQueue) {
				if(i.WorkSession == workSession)
					return true;
			}
			return false;
		}
		void ProcessQueue(Queue<WorkSessionTask> taskQueue, int delta) {
			int docToProcessCount = strategy.GetCountToProcess(taskQueue, delta);
			while(true) {
				if(taskQueue.Count == 0)
					break;
				if(docToProcessCount == 0)
					break;
				var task = taskQueue.Dequeue();
				bool success = task.Act();
				if(success)
					docToProcessCount--;
			}
		}
	}
	public interface IWorkSessionsProcSchedulingStrategy {
		TimeSpan ServiceTimeout { get; }
		int GetCountToProcess(Queue<WorkSessionTask> taskQueue, int delta);
		int GetNextServiceTimeout(Queue<WorkSessionTask> taskQueue, int delta, int previousTimeout);
	}
	public abstract class WorkSessionsProcSchedulingStrategyBase : IWorkSessionsProcSchedulingStrategy {
		public static readonly TimeSpan DefaultServiceTimeout = TimeSpan.FromSeconds(20);
		public WorkSessionsProcSchedulingStrategyBase() { }
		public virtual TimeSpan ServiceTimeout { get { return WorkSessionsProcSchedulingStrategyBase.DefaultServiceTimeout; } }
		public virtual int GetCountToProcess(Queue<WorkSessionTask> taskQueue, int delta) {
			return taskQueue.Count;
		}
		public virtual int GetNextServiceTimeout(Queue<WorkSessionTask> taskQueue, int delta, int previousTimeout) {
			return previousTimeout;
		}
	}
	public class WorkSessionsHalfQueueSchedulingStrategy : WorkSessionsProcSchedulingStrategyBase {
		const int DefaultMinDocToProcessCount = 10;
		int minDocToProcessCount;
		public WorkSessionsHalfQueueSchedulingStrategy() : this(DefaultMinDocToProcessCount) { }
		public WorkSessionsHalfQueueSchedulingStrategy(int minDocToProcessCount) {
			this.minDocToProcessCount = minDocToProcessCount;
		}
		public override int GetCountToProcess(Queue<WorkSessionTask> taskQueue, int delta) {
			int half = (int)Math.Ceiling((double)taskQueue.Count / 2);
			return half > minDocToProcessCount ? half : minDocToProcessCount;
		}
	}
	public class WorkSessionsConstDocCountSchedulingStrategy : WorkSessionsProcSchedulingStrategyBase {
		const int DefaultDocToProcessCount = 10;
		int docToProcessCount;
		public WorkSessionsConstDocCountSchedulingStrategy() : this(DefaultDocToProcessCount) { }
		public WorkSessionsConstDocCountSchedulingStrategy(int docToProcessCount) {
			this.docToProcessCount = docToProcessCount;
		}
		public override int GetCountToProcess(Queue<WorkSessionTask> taskQueue, int delta) {
			return docToProcessCount;
		}
	}
}
