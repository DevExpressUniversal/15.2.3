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
using System.Diagnostics;
using System.Reflection;
namespace DevExpress.Xpo.Helpers {
	public class PerformanceCounters {
		public class Counter {
			public readonly static Counter Empty = new Counter();
#if !CF
			PerformanceCounter counter;
			public Counter(string categoryName, string instanceName, string counterName, PerformanceCounterType counterType) {
				counter = new PerformanceCounter();
				counter.CategoryName = categoryName;
				counter.CounterName = counterName;
				counter.InstanceName = instanceName;
				counter.ReadOnly = false;
				counter.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
				counter.RawValue = 0;
			}
			public void Dispose() {
				if(counter != null)
					counter.RemoveInstance();
			}
			public void Increment() {
				if(counter != null)
					counter.Increment();
			}
			public void Increment(int count) {
				if(counter != null)
					counter.IncrementBy(count);
			}
#else
			public void Increment() { }
			public void Increment(int count) { }
#endif
			public Counter() { }
			public void Decrement() {
				Increment(-1);
			}
			public void Decrement(int count) {
				Increment(-count);
			}
		}
		public class QueueLengthCounter : IDisposable {
			readonly Counter CounterTotal;
			readonly Counter CounterExact;
			readonly Counter CounterTotalQueue;
			readonly Counter CounterExactQueue;
			public QueueLengthCounter(Counter cntTotal, Counter cntTotalQueue, Counter cntExact, Counter cntExactQueue) {
				this.CounterTotal = cntTotal;
				this.CounterTotal.Increment();
				this.CounterExact = cntExact;
				this.CounterExact.Increment();
				this.CounterTotalQueue = cntTotalQueue;
				this.CounterTotalQueue.Increment();
				this.CounterExactQueue = cntExactQueue;
				this.CounterExactQueue.Increment();
			}
			public void Dispose() {
				this.CounterTotalQueue.Decrement();
				this.CounterExactQueue.Decrement();
			}
		}
		static public Counter ObjectsInCache {
			get { return GetCounter(CounterIndexes.ObjectsInCache); }
		}
		static public Counter ObjectsInCacheAdded {
			get { return GetCounter(CounterIndexes.ObjectsInCacheAdded); }
		}
		static public Counter ObjectsInCacheRemoved {
			get { return GetCounter(CounterIndexes.ObjectsInCacheRemoved); }
		}
		static public Counter SessionCount {
			get { return GetCounter(CounterIndexes.SessionCount); }
		}
		static public Counter SessionConnected {
			get { return GetCounter(CounterIndexes.SessionConnected); }
		}
		static public Counter SessionDisconnected {
			get { return GetCounter(CounterIndexes.SessionDisconnected); }
		}
		static public Counter SqlDataStoreCount {
			get { return GetCounter(CounterIndexes.SqlDataStoreCount); }
		}
		static public Counter SqlDataStoreCreated {
			get { return GetCounter(CounterIndexes.SqlDataStoreCreated); }
		}
		static public Counter SqlDataStoreFinalized {
			get { return GetCounter(CounterIndexes.SqlDataStoreFinalized); }
		}
		static public Counter SqlDataStoreTotalRequests {
			get { return GetCounter(CounterIndexes.SqlDataStoreTotalRequests); }
		}
		static public Counter SqlDataStoreSchemaUpdateRequests {
			get { return GetCounter(CounterIndexes.SqlDataStoreSchemaUpdateRequests); }
		}
		static public Counter SqlDataStoreSelectRequests {
			get { return GetCounter(CounterIndexes.SqlDataStoreSelectRequests); }
		}
		static public Counter SqlDataStoreModifyRequests {
			get { return GetCounter(CounterIndexes.SqlDataStoreModifyRequests); }
		}
		static public Counter SqlDataStoreSelectQueries {
			get { return GetCounter(CounterIndexes.SqlDataStoreSelectQueries); }
		}
		static public Counter SqlDataStoreModifyStatements {
			get { return GetCounter(CounterIndexes.SqlDataStoreModifyStatements); }
		}
		static public Counter SqlDataStoreTotalQueue {
			get { return GetCounter(CounterIndexes.SqlDataStoreTotalQueue); }
		}
		static public Counter SqlDataStoreSelectQueue {
			get { return GetCounter(CounterIndexes.SqlDataStoreSelectQueue); }
		}
		static public Counter SqlDataStoreModifyQueue {
			get { return GetCounter(CounterIndexes.SqlDataStoreModifyQueue); }
		}
		static public Counter SqlDataStoreSchemaUpdateQueue {
			get { return GetCounter(CounterIndexes.SqlDataStoreSchemaUpdateQueue); }
		}
		static public Counter DataCacheNodeCacheHit {
			get { return GetCounter(CounterIndexes.DataCacheNodeCacheHit); }
		}
		static public Counter DataCacheNodeCacheMiss {
			get { return GetCounter(CounterIndexes.DataCacheNodeCacheMiss); }
		}
		static public Counter DataCacheNodeCachePassthrough {
			get { return GetCounter(CounterIndexes.DataCacheNodeCachePassthrough); }
		}
		static public Counter DataCacheNodeCachedCount {
			get { return GetCounter(CounterIndexes.DataCacheNodeCachedCount); }
		}
		static public Counter DataCacheNodeCachedAdded {
			get { return GetCounter(CounterIndexes.DataCacheNodeCachedAdded); }
		}
		static public Counter DataCacheNodeCachedRemoved {
			get { return GetCounter(CounterIndexes.DataCacheNodeCachedRemoved); }
		}
		static public Counter DataCacheNodeCount {
			get { return GetCounter(CounterIndexes.DataCacheNodeCount); }
		}
		static public Counter DataCacheNodeCreated {
			get { return GetCounter(CounterIndexes.DataCacheNodeCreated); }
		}
		static public Counter DataCacheNodeFinalized {
			get { return GetCounter(CounterIndexes.DataCacheNodeFinalized); }
		}
		static public Counter DataCacheRootCount {
			get { return GetCounter(CounterIndexes.DataCacheRootCount); }
		}
		static public Counter DataCacheRootCreated {
			get { return GetCounter(CounterIndexes.DataCacheRootCreated); }
		}
		static public Counter DataCacheRootFinalized {
			get { return GetCounter(CounterIndexes.DataCacheRootFinalized); }
		}
		static public Counter MSSql2005CacheRootDependencyEstablished {
			get { return GetCounter(CounterIndexes.MSSql2005CacheRootDependencyEstablished); }
		}
		static public Counter MSSql2005CacheRootDependencyTriggered {
			get { return GetCounter(CounterIndexes.MSSql2005CacheRootDependencyTriggered); }
		}
		static public Counter DataCacheRootTotalRequests {
			get { return GetCounter(CounterIndexes.DataCacheRootTotalRequests); }
		}
		static public Counter DataCacheRootSchemaUpdateRequests {
			get { return GetCounter(CounterIndexes.DataCacheRootSchemaUpdateRequests); }
		}
		static public Counter DataCacheRootSelectRequests {
			get { return GetCounter(CounterIndexes.DataCacheRootSelectRequests); }
		}
		static public Counter DataCacheRootModifyRequests {
			get { return GetCounter(CounterIndexes.DataCacheRootModifyRequests); }
		}
		static public Counter DataCacheRootSelectQueries {
			get { return GetCounter(CounterIndexes.DataCacheRootSelectQueries); }
		}
		static public Counter DataCacheRootModifyStatements {
			get { return GetCounter(CounterIndexes.DataCacheRootModifyStatements); }
		}
		static public Counter DataCacheRootTotalQueue {
			get { return GetCounter(CounterIndexes.DataCacheRootTotalQueue); }
		}
		static public Counter DataCacheRootSelectQueue {
			get { return GetCounter(CounterIndexes.DataCacheRootSelectQueue); }
		}
		static public Counter DataCacheRootModifyQueue {
			get { return GetCounter(CounterIndexes.DataCacheRootModifyQueue); }
		}
		static public Counter DataCacheRootSchemaUpdateQueue {
			get { return GetCounter(CounterIndexes.DataCacheRootSchemaUpdateQueue); }
		}
		static public Counter DataCacheRootProcessCookieQueue {
			get { return GetCounter(CounterIndexes.DataCacheRootProcessCookieQueue); }
		}
		static public Counter DataCacheRootNotifyDirtyTablesQueue {
			get { return GetCounter(CounterIndexes.DataCacheRootNotifyDirtyTablesQueue); }
		}
		static public Counter DataCacheRootProcessCookieRequests {
			get { return GetCounter(CounterIndexes.DataCacheRootProcessCookieRequests); }
		}
		static public Counter DataCacheRootNotifyDirtyTablesRequests {
			get { return GetCounter(CounterIndexes.DataCacheRootNotifyDirtyTablesRequests); }
		}
		static public Counter DataCacheRootNotifyDirtyTablesTables {
			get { return GetCounter(CounterIndexes.DataCacheRootNotifyDirtyTablesTables); }
		}
		static public Counter DataCacheNodeTotalRequests {
			get { return GetCounter(CounterIndexes.DataCacheNodeTotalRequests); }
		}
		static public Counter DataCacheNodeSchemaUpdateRequests {
			get { return GetCounter(CounterIndexes.DataCacheNodeSchemaUpdateRequests); }
		}
		static public Counter DataCacheNodeSelectRequests {
			get { return GetCounter(CounterIndexes.DataCacheNodeSelectRequests); }
		}
		static public Counter DataCacheNodeModifyRequests {
			get { return GetCounter(CounterIndexes.DataCacheNodeModifyRequests); }
		}
		static public Counter DataCacheNodeSelectQueries {
			get { return GetCounter(CounterIndexes.DataCacheNodeSelectQueries); }
		}
		static public Counter DataCacheNodeModifyStatements {
			get { return GetCounter(CounterIndexes.DataCacheNodeModifyStatements); }
		}
		static public Counter DataCacheNodeTotalQueue {
			get { return GetCounter(CounterIndexes.DataCacheNodeTotalQueue); }
		}
		static public Counter DataCacheNodeSelectQueue {
			get { return GetCounter(CounterIndexes.DataCacheNodeSelectQueue); }
		}
		static public Counter DataCacheNodeModifyQueue {
			get { return GetCounter(CounterIndexes.DataCacheNodeModifyQueue); }
		}
		static public Counter DataCacheNodeSchemaUpdateQueue {
			get { return GetCounter(CounterIndexes.DataCacheNodeSchemaUpdateQueue); }
		}
		static public Counter DataCacheNodeProcessCookieQueue {
			get { return GetCounter(CounterIndexes.DataCacheNodeProcessCookieQueue); }
		}
		static public Counter DataCacheNodeNotifyDirtyTablesQueue {
			get { return GetCounter(CounterIndexes.DataCacheNodeNotifyDirtyTablesQueue); }
		}
		static public Counter DataCacheNodeProcessCookieRequests {
			get { return GetCounter(CounterIndexes.DataCacheNodeProcessCookieRequests); }
		}
		static public Counter DataCacheNodeNotifyDirtyTablesRequests {
			get { return GetCounter(CounterIndexes.DataCacheNodeNotifyDirtyTablesRequests); }
		}
		static public Counter DataCacheNodeNotifyDirtyTablesTables {
			get { return GetCounter(CounterIndexes.DataCacheNodeNotifyDirtyTablesTables); }
		}
		public enum CounterIndexes {
			ObjectsInCache, ObjectsInCacheAdded, ObjectsInCacheRemoved,
			SessionCount, SessionConnected, SessionDisconnected,
			SqlDataStoreCount, SqlDataStoreCreated, SqlDataStoreFinalized,
			SqlDataStoreTotalRequests, SqlDataStoreSchemaUpdateRequests, SqlDataStoreSelectRequests, SqlDataStoreModifyRequests,
			SqlDataStoreSelectQueries, SqlDataStoreModifyStatements,
			SqlDataStoreTotalQueue, SqlDataStoreSelectQueue, SqlDataStoreModifyQueue, SqlDataStoreSchemaUpdateQueue,
			DataCacheNodeCacheHit, DataCacheNodeCacheMiss, DataCacheNodeCachePassthrough,
			DataCacheNodeCachedCount, DataCacheNodeCachedAdded, DataCacheNodeCachedRemoved,
			DataCacheNodeCount, DataCacheNodeCreated, DataCacheNodeFinalized,
			DataCacheRootCount, DataCacheRootCreated, DataCacheRootFinalized,
			MSSql2005CacheRootDependencyEstablished, MSSql2005CacheRootDependencyTriggered,
			DataCacheRootTotalRequests, DataCacheRootSchemaUpdateRequests, DataCacheRootSelectRequests, DataCacheRootModifyRequests,
			DataCacheRootSelectQueries, DataCacheRootModifyStatements,
			DataCacheRootTotalQueue, DataCacheRootSelectQueue, DataCacheRootModifyQueue, DataCacheRootSchemaUpdateQueue,
			DataCacheRootProcessCookieQueue, DataCacheRootNotifyDirtyTablesQueue,
			DataCacheRootProcessCookieRequests, DataCacheRootNotifyDirtyTablesRequests, DataCacheRootNotifyDirtyTablesTables,
			DataCacheNodeTotalRequests, DataCacheNodeSchemaUpdateRequests, DataCacheNodeSelectRequests, DataCacheNodeModifyRequests,
			DataCacheNodeSelectQueries, DataCacheNodeModifyStatements,
			DataCacheNodeTotalQueue, DataCacheNodeSelectQueue, DataCacheNodeModifyQueue, DataCacheNodeSchemaUpdateQueue,
			DataCacheNodeProcessCookieQueue, DataCacheNodeNotifyDirtyTablesQueue,
			DataCacheNodeProcessCookieRequests, DataCacheNodeNotifyDirtyTablesRequests, DataCacheNodeNotifyDirtyTablesTables,
		}
#if !CF
		static PerformanceCounters() {
			CounterCreationData[] data = new CounterCreationData[Enum.GetNames(typeof(CounterIndexes)).Length];
			data[(int)CounterIndexes.ObjectsInCache] = new CounterCreationData("Objects in identity map", "Objects in identity map", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.ObjectsInCacheAdded] = new CounterCreationData("Objects added to identity map", "Objects added to identity map", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.ObjectsInCacheRemoved] = new CounterCreationData("Objects removed from identity map", "Objects removed from identity map", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SessionCount] = new CounterCreationData("Session connected count", "Connected Session count", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.SessionConnected] = new CounterCreationData("Sessions connected", "Sessions connected", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SessionDisconnected] = new CounterCreationData("Sessions disconnected", "Sessions disconnected", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreCount] = new CounterCreationData("SqlDataStore instances", "SqlDataStore instances", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.SqlDataStoreCreated] = new CounterCreationData("SqlDataStore created", "SqlDataStore created", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreFinalized] = new CounterCreationData("SqlDataStore finalized", "SqlDataStore finalized", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreTotalRequests] = new CounterCreationData("SqlDataStore requests handled", "SqlDataStore requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreSchemaUpdateRequests] = new CounterCreationData("SqlDataStore schema update requests handled", "SqlDataStore schema update requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreSelectRequests] = new CounterCreationData("SqlDataStore select data requests handled", "SqlDataStore select data requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreModifyRequests] = new CounterCreationData("SqlDataStore modify data requests handled", "SqlDataStore modify data requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreSelectQueries] = new CounterCreationData("SqlDataStore select data queries handled", "SqlDataStore select data queries handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreModifyStatements] = new CounterCreationData("SqlDataStore modify data statements handled", "SqlDataStore modify data statements handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.SqlDataStoreTotalQueue] = new CounterCreationData("SqlDataStore queue length", "SqlDataStore queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.SqlDataStoreSelectQueue] = new CounterCreationData("SqlDataStore select data queue length", "SqlDataStore select data queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.SqlDataStoreModifyQueue] = new CounterCreationData("SqlDataStore modify data queue length", "SqlDataStore modify data queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.SqlDataStoreSchemaUpdateQueue] = new CounterCreationData("SqlDataStore schema update queue length", "SqlDataStore schema update queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeCacheHit] = new CounterCreationData("DataCacheNode cache hit", "DataCacheNode cache hit", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeCacheMiss] = new CounterCreationData("DataCacheNode cache miss", "DataCacheNode cache miss", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeCachePassthrough] = new CounterCreationData("DataCacheNode cache passthrough", "DataCacheNode cache passthrough", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeCachedCount] = new CounterCreationData("DataCacheNode cached records count", "DataCacheNode cached records count", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeCachedAdded] = new CounterCreationData("DataCacheNode cached records added", "DataCacheNode cached records added", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeCachedRemoved] = new CounterCreationData("DataCacheNode cached records removed", "DataCacheNode cached records removed", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.MSSql2005CacheRootDependencyEstablished] = new CounterCreationData("MSSql2005CacheRoot dependency established", "MSSql2005CacheRoot dependency established", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.MSSql2005CacheRootDependencyTriggered] = new CounterCreationData("MSSql2005CacheRoot dependency triggered", "MSSql2005CacheRoot dependency triggered", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootCount] = new CounterCreationData("DataCacheRoot count", "DataCacheRoot count", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheRootCreated] = new CounterCreationData("DataCacheRoot created", "DataCacheRoot created", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootFinalized] = new CounterCreationData("DataCacheRoot finalized", "DataCacheRoot finalized", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootTotalRequests] = new CounterCreationData("DataCacheRoot requests handled", "DataCacheRoot requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootSchemaUpdateRequests] = new CounterCreationData("DataCacheRoot schema update requests handled", "DataCacheRoot schema update requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootSelectRequests] = new CounterCreationData("DataCacheRoot select data requests handled", "DataCacheRoot select data requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootModifyRequests] = new CounterCreationData("DataCacheRoot modify data requests handled", "DataCacheRoot modify data requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootProcessCookieRequests] = new CounterCreationData("DataCacheRoot process cookie requests handled", "DataCacheRoot process cookie requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootNotifyDirtyTablesRequests] = new CounterCreationData("DataCacheRoot notify dirty tables requests handled", "DataCacheRoot notify dirty tables requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootSelectQueries] = new CounterCreationData("DataCacheRoot select data queries handled", "DataCacheRoot select data queries handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootModifyStatements] = new CounterCreationData("DataCacheRoot modify data statements handled", "DataCacheRoot modify data statements handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootNotifyDirtyTablesTables] = new CounterCreationData("DataCacheRoot notify dirty tables table names handled", "DataCacheRoot notify dirty tables table names handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheRootTotalQueue] = new CounterCreationData("DataCacheRoot queue length", "DataCacheRoot queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheRootSelectQueue] = new CounterCreationData("DataCacheRoot select data queue length", "DataCacheRoot select data queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheRootModifyQueue] = new CounterCreationData("DataCacheRoot modify data queue length", "DataCacheRoot modify data queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheRootSchemaUpdateQueue] = new CounterCreationData("DataCacheRoot schema update queue length", "DataCacheRoot schema update queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheRootProcessCookieQueue] = new CounterCreationData("DataCacheRoot process cookie queue length", "DataCacheRoot process cookie queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheRootNotifyDirtyTablesQueue] = new CounterCreationData("DataCacheRoot notify dirty tables queue length", "DataCacheRoot notify dirty tables queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeCount] = new CounterCreationData("DataCacheNode count", "DataCacheNode count", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeCreated] = new CounterCreationData("DataCacheNode created", "DataCacheNode created", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeFinalized] = new CounterCreationData("DataCacheNode finalized", "DataCacheNode finalized", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeTotalRequests] = new CounterCreationData("DataCacheNode requests handled", "DataCacheNode requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeSchemaUpdateRequests] = new CounterCreationData("DataCacheNode schema update requests handled", "DataCacheNode schema update requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeSelectRequests] = new CounterCreationData("DataCacheNode select data requests handled", "DataCacheNode select data requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeModifyRequests] = new CounterCreationData("DataCacheNode modify data requests handled", "DataCacheNode modify data requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeProcessCookieRequests] = new CounterCreationData("DataCacheNode process cookie requests handled", "DataCacheNode process cookie requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeNotifyDirtyTablesRequests] = new CounterCreationData("DataCacheNode notify dirty tables requests handled", "DataCacheNode notify dirty tables requests handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeSelectQueries] = new CounterCreationData("DataCacheNode select data queries handled", "DataCacheNode select data queries handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeModifyStatements] = new CounterCreationData("DataCacheNode modify data statements handled", "DataCacheNode modify data statements handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeNotifyDirtyTablesTables] = new CounterCreationData("DataCacheNode notify dirty tables table names handled", "DataCacheNode notify dirty tables table names handled", PerformanceCounterType.RateOfCountsPerSecond32);
			data[(int)CounterIndexes.DataCacheNodeTotalQueue] = new CounterCreationData("DataCacheNode queue length", "DataCacheNode queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeSelectQueue] = new CounterCreationData("DataCacheNode select data queue length", "DataCacheNode select data queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeModifyQueue] = new CounterCreationData("DataCacheNode modify data queue length", "DataCacheNode modify data queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeSchemaUpdateQueue] = new CounterCreationData("DataCacheNode schema update queue length", "DataCacheNode schema update queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeProcessCookieQueue] = new CounterCreationData("DataCacheNode process cookie queue length", "DataCacheNode process cookie queue length", PerformanceCounterType.NumberOfItems32);
			data[(int)CounterIndexes.DataCacheNodeNotifyDirtyTablesQueue] = new CounterCreationData("DataCacheNode notify dirty tables queue length", "DataCacheNode notify dirty tables queue length", PerformanceCounterType.NumberOfItems32);
			CountersData = new CounterCreationDataCollection(data);
			counters = new Counter[CountersData.Count];
			for(int i = 0; i < counters.Length; i++)
				counters[i] = Counter.Empty;
			InitIfNeeded();
		}
		static CounterCreationDataCollection CountersData;
		static Counter[] counters;
		static Counter GetCounter(CounterIndexes index) {
			return counters[(int)index];
		}
		static void CreateCounters(string category, string instanceName) {
			Counter[] list = new Counter[CountersData.Count];
			for(int i = 0; i < list.Length; i++)
				list[i] = CreateCounter(CountersData[i], category, instanceName);
			counters = list;
		}
		static void Clear() {
			for(int i = 0; i < counters.Length; i++)
				counters[i].Dispose();
		}
		static Counter CreateCounter(CounterCreationData counter, string category, string instanceName) {
			if(PlatformID.Win32NT == Environment.OSVersion.Platform &&
				Environment.OSVersion.Version.Major >= 5) {
				try {
					return new Counter(category, instanceName, counter.CounterName, counter.CounterType);
				} catch(InvalidOperationException) {
				}
			}
			return Counter.Empty;
		}
		static string GetInstanceName() {
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			string name = null;
			if(entryAssembly != null) {
				AssemblyName assemblyName = entryAssembly.GetName();
				if(assemblyName != null)
					name = assemblyName.Name;
			}
			if(string.IsNullOrEmpty(name)) {
				AppDomain currentDomain = AppDomain.CurrentDomain;
				if(currentDomain != null)
					name = currentDomain.FriendlyName;
			}
			if(name == null)
				return null;
			char[] nameArray = name.ToCharArray();
			for(int i = 0; i < nameArray.Length; i++)
				switch(nameArray[i]) {
					case '(':
						nameArray[i] = '[';
						break;
					case ')':
						nameArray[i] = ']';
						break;
					case '#':
					case '/':
					case '\\':
						nameArray[i] = '_';
						break;
				}
			return new string(nameArray) + "[" + Process.GetCurrentProcess().Id.ToString() + "]";
		}
		static void DomainUnloadEventHandler(object sender, EventArgs e) {
			Clear();
		}
		static void EnusreCategory(string category) {
			if(PerformanceCounterCategory.Exists(category)) {
				bool res = true;
				foreach(CounterCreationData counter in CountersData) {
					if(!PerformanceCounterCategory.CounterExists(counter.CounterName, category)) {
						res = false;
						break;
					}
				}
				if(res)
					return;
				PerformanceCounterCategory.Delete(category);
			}
			PerformanceCounterCategory.Create(category, "Xpo", PerformanceCounterCategoryType.MultiInstance, CountersData);
		}
		static object syncRoot = new object();
		static bool inited = false;
		public static void Init() {
			lock(syncRoot) {
				if(inited)
					return;
				inited = true;
			}
			const string Category = "DevExpress Xpo";
			EnusreCategory(Category);
			CreateCounters(Category, GetInstanceName());
			AppDomain.CurrentDomain.DomainUnload += new EventHandler(DomainUnloadEventHandler);
		}
		static void InitIfNeeded() {
			BooleanSwitch perfCountersSwitch = new BooleanSwitch("XpoPerfomanceCounters", "");
			if(perfCountersSwitch.Enabled)
				Init();
		}
#else
		static Counter GetCounter(CounterIndexes index) {
			return Counter.Empty;
		}
#endif
	}
}
