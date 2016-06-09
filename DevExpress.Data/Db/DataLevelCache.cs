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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using DevExpress.Xpo.Helpers;
namespace DevExpress.Xpo.DB.Helpers {
	using DevExpress.Xpo.DB;
	using System.Diagnostics;
	using DevExpress.Xpo.Logger;
	using DevExpress.Utils;
	using Compatibility.System;
	[Serializable]
	public class DataCacheCookie {
		public Guid Guid;
		public long Age;
		public DataCacheCookie() {}
		public DataCacheCookie(Guid guid, long age) {
			this.Guid = guid;
			this.Age = age;
		}
		public static readonly DataCacheCookie Empty = new DataCacheCookie(Guid.Empty, 0);
	}
	[Serializable]
	public class TableAge {
		public string Name;
		public long Age;
		public TableAge() {}
		public TableAge(string name, long age) {
			this.Name = name;
			this.Age = age;
		}
	}
	[Serializable]
	public class DataCacheConfiguration {
		static readonly DataCacheConfiguration empty = new DataCacheConfiguration(DataCacheConfigurationCaching.All, (string[])null);
		public static DataCacheConfiguration Empty { get { return DataCacheConfiguration.empty; } }
		string[] tables;
		DataCacheConfigurationCaching caching;
		public string[] Tables {
			get { return tables; }
			set {
				tables = value;
				tableDictionary = null;
			}
		}
		public DataCacheConfigurationCaching Caching {
			get { return caching; }
			set { caching = value; }
		}
		[NonSerialized]
		Dictionary<string, bool> tableDictionary;
		[System.Xml.Serialization.XmlIgnore]
		public Dictionary<string, bool> TableDictionary {
			get {
				if(tableDictionary == null && Tables != null) {
					tableDictionary = CreateTableDictionary(Tables);
				}
				return tableDictionary;
			}
		}
		public DataCacheConfiguration() { }
		public DataCacheConfiguration(DataCacheConfigurationCaching caching, params string[] tables) {
			this.tables = tables;
			this.caching = caching;
		}
		public static Dictionary<string, bool> CreateTableDictionary(string[] tableList) {
			if(tableList == null) return null;
			Dictionary<string, bool> result = new Dictionary<string, bool>();
			foreach(string table in tableList) {
				result[table] = true;
			}
			return result;
		}
	}
	public enum DataCacheConfigurationCaching {
		All,
		InList,
		NotInList
	}
	[Serializable]
	public class DataCacheResult {
		public TableAge[] UpdatedTableAges;
		public DataCacheConfiguration CacheConfig;
		public DataCacheCookie Cookie;
	}
	[Serializable]
	public class DataCacheUpdateSchemaResult : DataCacheResult {
		public UpdateSchemaResult UpdateSchemaResult;
	}
	[Serializable]
	public class DataCacheSelectDataResult : DataCacheResult {
		public DataCacheCookie SelectingCookie;
		public SelectedData SelectedData;
	}
	[Serializable]
	public class DataCacheModificationResult : DataCacheResult {
		public ModificationResult ModificationResult;
	}
#if SL || DXPORTABLE
	class DataCacheLockForAwfulFrameworks: IDisposable {
		public readonly object Lock;
		public DataCacheLockForAwfulFrameworks(object obj) {
			Lock = obj;
			Monitor.Enter(Lock);
		}
		public void Dispose() {
			Monitor.Exit(Lock);
		}
	}
#else
	class DataCacheWriterLock: IDisposable {
		public readonly ReaderWriterLock RWL;
		public DataCacheWriterLock(ReaderWriterLock rwl) {
			RWL = rwl;
			RWL.AcquireWriterLock(Timeout.Infinite);
		}
		public void Dispose() {
			RWL.ReleaseWriterLock();
		}
	}
	class DataCacheReaderLock: IDisposable {
		public readonly ReaderWriterLock RWL;
		public DataCacheReaderLock(ReaderWriterLock rwl) {
			RWL = rwl;
			RWL.AcquireReaderLock(Timeout.Infinite);
		}
		public void Dispose() {
			RWL.ReleaseReaderLock();
		}
	}
#endif
	public abstract class DataCacheBase:
#if !SL && !DXPORTABLE
		MarshalByRefObject, 
#endif
		ICachedDataStore, IDataStoreSchemaExplorer, ICommandChannel {
		protected DataCacheConfiguration cacheConfiguration = new DataCacheConfiguration();
		protected readonly ICommandChannel nestedCommandChannel;
		public const string LogCategory = "DataCache";
		protected long Age = 0;
		protected Guid MyGuid = Guid.NewGuid();
		protected DataCacheCookie GetCurrentCookie() {
			ValidateLockedRead();
			return new DataCacheCookie(this.MyGuid, this.Age);
		}
		protected DataCacheCookie GetCurrentCookieSafe() {
			using(LockForRead()) {
				return GetCurrentCookie();
			}
		}
#if SL || DXPORTABLE
		protected IDisposable LockForRead() {
			return LockForChange();
		}
		protected IDisposable LockForChange() {
			return new DataCacheLockForAwfulFrameworks(TablesAges);
		}
		[System.Diagnostics.Conditional("DEBUG")]
		protected void ValidateLockedRead() {
		}
		[System.Diagnostics.Conditional("DEBUG")]
		protected void ValidateLockedWrite() {
		}
#else
		ReaderWriterLock _RWL = new ReaderWriterLock();
		protected IDisposable LockForRead() {
			return new DataCacheReaderLock(_RWL);
		}
		protected IDisposable LockForChange() {
			if(_RWL.IsReaderLockHeld)
				throw new InvalidOperationException("internal error: reader lock already held!!!");
			return new DataCacheWriterLock(_RWL);
		}
		[System.Diagnostics.Conditional("DEBUG")]
		protected void ValidateLockedRead() {
			if(!_RWL.IsReaderLockHeld && !_RWL.IsWriterLockHeld)
				throw new InvalidOperationException("internal error: reader lock expected!!!");
		}
		[System.Diagnostics.Conditional("DEBUG")]
		protected void ValidateLockedWrite() {
			if(!_RWL.IsWriterLockHeld)
				throw new InvalidOperationException("internal error: writer lock expected!!!");
		}
#endif
		protected readonly Dictionary<string, long> TablesAges = new Dictionary<string, long>();
		protected DataCacheBase(ICommandChannel nestedCommandChannel) {
			this.nestedCommandChannel = nestedCommandChannel;
		}
		public DataCacheUpdateSchemaResult UpdateSchema(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist) {
			return LogManager.Log<DataCacheUpdateSchemaResult>(LogCategory, () => {
				return UpdateSchemaCore(cookie, tables, dontCreateIfFirstTableNotExist);
			}, (d) => {
				return LogMessage.CreateMessage(this, cookie, string.Concat("UpdateSchema: ", 
					LogMessage.CollectionToString<DBTable>(tables, (table) => { return table.Name; })), d);
			});
		}
		public DataCacheSelectDataResult SelectData(DataCacheCookie cookie, SelectStatement[] selects) {
			return LogManager.LogMany<DataCacheSelectDataResult>(LogCategory, () => {
				return SelectDataCore(cookie, selects);
			}, (d) => {
				if(selects == null && selects.Length == 0) return null;
				LogMessage[] messages = new LogMessage[selects.Length];
				for(int i = 0; i < selects.Length; i++) {
					messages[i] = LogMessage.CreateMessage(this, cookie, selects[i].ToString(), d);
				}
				return messages;
			});
		}
		public DataCacheModificationResult ModifyData(DataCacheCookie cookie, ModificationStatement[] statements) {
			return LogManager.LogMany<DataCacheModificationResult>(LogCategory, () => {
				return ModifyDataCore(cookie, statements);
			}, (d) => {
				if(statements == null && statements.Length == 0) return null;
				LogMessage[] messages = new LogMessage[statements.Length];
				for(int i = 0; i < statements.Length; i++) {
					messages[i] = LogMessage.CreateMessage(this, cookie, statements[i].ToString(), d);
				}
				return messages;
			});
		}
		public DataCacheResult ProcessCookie(DataCacheCookie cookie) {
			return LogManager.Log<DataCacheResult>(LogCategory, () => {
				return ProcessCookieCore(cookie);
			}, (d) => {
				return LogMessage.CreateMessage(this, cookie, "ProcessCookie", d);
			});
		}
		public DataCacheResult NotifyDirtyTables(DataCacheCookie cookie, params string[] dirtyTablesNames) {
			return LogManager.Log<DataCacheResult>(LogCategory, () => {
				return NotifyDirtyTablesCore(cookie, dirtyTablesNames);
			}, (d) => {
				return LogMessage.CreateMessage(this, cookie, string.Concat("NotifyDirtyTables: ", string.Join(";", dirtyTablesNames)), d);
			});
		}
		protected abstract DataCacheUpdateSchemaResult UpdateSchemaCore(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist);
		protected abstract DataCacheSelectDataResult SelectDataCore(DataCacheCookie cookie, SelectStatement[] selects);
		protected abstract DataCacheModificationResult ModifyDataCore(DataCacheCookie cookie, ModificationStatement[] statements);
		protected abstract DataCacheResult ProcessCookieCore(DataCacheCookie cookie);
		protected abstract DataCacheResult NotifyDirtyTablesCore(DataCacheCookie cookie, params string[] dirtyTablesNames);
		public DataCacheResult NotifyDirtyTables(params string[] dirtyTablesNames) {
			return NotifyDirtyTables(DataCacheCookie.Empty, dirtyTablesNames);
		}
		protected void ProcessChildResultSinceCookie(DataCacheResult result, DataCacheCookie cookie) {
			using(LockForRead()) {
				result.Cookie = GetCurrentCookie();
				if(this.MyGuid != cookie.Guid) {	
					result.CacheConfig = cacheConfiguration;
					result.UpdatedTableAges = null;
				} else if(this.Age == cookie.Age) {	
					result.CacheConfig = null;
					result.UpdatedTableAges = new TableAge[0];
				} else {
					System.Diagnostics.Debug.Assert(this.Age > cookie.Age);
					result.CacheConfig = null;
					List<TableAge> updatedAges = new List<TableAge>();
					foreach(KeyValuePair<string, long> ta in this.TablesAges) {
						if(ta.Value > cookie.Age) {
							updatedAges.Add(new TableAge(ta.Key, ta.Value));
						}
					}
					result.UpdatedTableAges = updatedAges.ToArray();
				}
			}
		}
		public virtual UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			return UpdateSchema(DataCacheCookie.Empty, tables, dontCreateIfFirstTableNotExist).UpdateSchemaResult;
		}
		public virtual SelectedData SelectData(params SelectStatement[] selects) {
			return SelectData(DataCacheCookie.Empty, selects).SelectedData;
		}
		public virtual ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			return ModifyData(DataCacheCookie.Empty, dmlStatements).ModificationResult;
		}
		public abstract AutoCreateOption AutoCreateOption { get; }
		public abstract string[] GetStorageTablesList(bool includeViews);
		public abstract DBTable[] GetStorageTables(params string[] tables);
		protected virtual void ResetCore() {
			ValidateLockedWrite();
			this.MyGuid = Guid.NewGuid();
			this.Age = 0;
			this.TablesAges.Clear();
		}
		public void Reset() {
			using(LockForChange()) {
				ResetCore();
			}
		}
		public abstract void Configure(DataCacheConfiguration configuration);
		public static bool IsBadForCache(DataCacheConfiguration config, JoinNode node) {
			if(node.Table is DBProjection)
				return false;
			return IsBadForCache(config, node.Table.Name);
		}
		public static bool IsBadForCache(DataCacheConfiguration config, string tableName) {
			if(config != null && config.TableDictionary != null) {
				switch(config.Caching) {
					case DataCacheConfigurationCaching.NotInList:
						if(config.TableDictionary.ContainsKey(tableName)) return true;
						break;
					case DataCacheConfigurationCaching.InList:
						if(!config.TableDictionary.ContainsKey(tableName)) return true;
						break;
				}
			}
			return false;
		}
		object ICommandChannel.Do(string command, object args) {
			return DoInternal(command, args);
		}
		protected virtual object DoInternal(string command, object args) {
			if(nestedCommandChannel == null) {
				throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
			}
			return nestedCommandChannel.Do(command, args);
		}
	}
	public class CacheRecord {
		string[] tablesInStatement;
		SelectStatement statement;
		public CacheRecord Prev, Next;
		public string[] TablesInStatement {
			get {
				if(tablesInStatement == null) {
					tablesInStatement = statement.GetTablesNames();
					statement = null;
				}
				return tablesInStatement;
			}
		}
		public readonly string HashString;
		public string TableName {
			get { return statement == null || statement.Table is DBProjection ? tablesInStatement[0] : statement.Table.Name; }
		}
		SelectStatementResult queryResult;
		public SelectStatementResult QueryResult {
			get {
				return queryResult.Clone();
			}
			set {
				queryResult = value.Clone();
			}
		}
		public CacheRecord(SelectStatement statement) {
			this.statement = statement;
			this.HashString = QueryStatementToStringFormatter.GetString(statement);
		}
		public override bool Equals(object obj) {
			CacheRecord anotherNode = obj as CacheRecord;
			if(anotherNode == null)
				return false;
			return this.HashString == anotherNode.HashString;
		}
		public override int GetHashCode() {
			return this.HashString.GetHashCode();
		}
		public override string ToString() { return HashString; }
	}
}
namespace DevExpress.Xpo.DB {
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Data.Filtering;
	using System.ComponentModel;
	using Compatibility.System.ComponentModel;
	public interface ICacheToCacheCommunicationCore {
		DataCacheUpdateSchemaResult UpdateSchema(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist);
		DataCacheSelectDataResult SelectData(DataCacheCookie cookie, SelectStatement[] selects);
		DataCacheModificationResult ModifyData(DataCacheCookie cookie, ModificationStatement[] dmlStatements);
		DataCacheResult ProcessCookie(DataCacheCookie cookie);
		DataCacheResult NotifyDirtyTables(DataCacheCookie cookie, params string[] dirtyTablesNames);
	}
	public interface ICachedDataStore : ICacheToCacheCommunicationCore, IDataStore { }
	[Obsolete("Please use ICachedDataStore interface instead", true)]
	public interface ICacheToCacheCommuticationInterface : ICachedDataStore { }
	public class DataCacheRoot : DataCacheBase
#if DEBUG
, IDataStoreForTests
#endif
 {
#if DEBUG
		public void ClearDatabase() {
			using(LockForChange()) {
				((IDataStoreForTests)Nested).ClearDatabase();
				ResetCore();
			}
		}
#endif
		IDataStore _nested;
		protected IDataStore Nested { get { return _nested; } }
		public DataCacheRoot(IDataStore subjectForCache)
			: base(subjectForCache as ICommandChannel) {
			this._nested = subjectForCache;
#if !SL && !DXPORTABLE
			PerformanceCounters.DataCacheRootCount.Increment();
			PerformanceCounters.DataCacheRootCreated.Increment();
#endif
		}
#if !SL && !DXPORTABLE
		~DataCacheRoot() {
			PerformanceCounters.DataCacheRootCount.Decrement();
			PerformanceCounters.DataCacheRootFinalized.Increment();
		}
#endif
		protected override DataCacheUpdateSchemaResult UpdateSchemaCore(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist) {
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheRootTotalRequests, PerformanceCounters.DataCacheRootTotalQueue, PerformanceCounters.DataCacheRootSchemaUpdateRequests, PerformanceCounters.DataCacheRootSchemaUpdateQueue))
#endif
			{
				DataCacheUpdateSchemaResult result = new DataCacheUpdateSchemaResult();
				result.UpdateSchemaResult = Nested.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
				ProcessChildResultSinceCookie(result, cookie);
				return result;
			}
		}
		protected override DataCacheSelectDataResult SelectDataCore(DataCacheCookie cookie, SelectStatement[] selects) {
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheRootTotalRequests, PerformanceCounters.DataCacheRootTotalQueue, PerformanceCounters.DataCacheRootSelectRequests, PerformanceCounters.DataCacheRootSelectQueue))
			{
				PerformanceCounters.DataCacheRootSelectQueries.Increment(selects.Length);
#else
			{
#endif
				DataCacheSelectDataResult result = new DataCacheSelectDataResult();
				result.SelectingCookie = GetCurrentCookieSafe();
				result.SelectedData = Nested.SelectData(selects);
				ProcessChildResultSinceCookie(result, cookie);
				return result;
			}
		}
		protected override DataCacheModificationResult ModifyDataCore(DataCacheCookie cookie, ModificationStatement[] dmlStatements) {
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheRootTotalRequests, PerformanceCounters.DataCacheRootTotalQueue, PerformanceCounters.DataCacheRootModifyRequests, PerformanceCounters.DataCacheRootModifyQueue))
			{
				PerformanceCounters.DataCacheRootModifyStatements.Increment(dmlStatements.Length);
#else
			{
#endif
				DataCacheModificationResult result = new DataCacheModificationResult();
				result.ModificationResult = Nested.ModifyData(dmlStatements);
				string[] dirtyTables = BaseStatement.GetTablesNames(dmlStatements);
				NotifyDirtyTablesCore(dirtyTables);
				ProcessChildResultSinceCookie(result, cookie);
				return result;
			}
		}
		void NotifyDirtyTablesCore(string[] dirtyTablesNames) {
			if(dirtyTablesNames == null || dirtyTablesNames.Length == 0)
				return;
			using(LockForChange()) {
				if(Age == long.MaxValue) {
					ResetCore();
				} else {
					++Age;
					foreach(string tableName in dirtyTablesNames) {
						this.TablesAges[tableName] = Age;
					}
				}
			}
		}
		protected override DataCacheResult NotifyDirtyTablesCore(DataCacheCookie cookie, params string[] dirtyTablesNames) {
			if(dirtyTablesNames == null)
				dirtyTablesNames = new string[0];
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheRootTotalRequests, PerformanceCounters.DataCacheRootTotalQueue, PerformanceCounters.DataCacheRootNotifyDirtyTablesRequests, PerformanceCounters.DataCacheRootNotifyDirtyTablesQueue))
			{
				PerformanceCounters.DataCacheRootNotifyDirtyTablesTables.Increment(dirtyTablesNames.Length);
#else
			{
#endif
				DataCacheResult result = new DataCacheResult();
				NotifyDirtyTablesCore(dirtyTablesNames);
				ProcessChildResultSinceCookie(result, cookie);
				return result;
			}
		}
		protected override DataCacheResult ProcessCookieCore(DataCacheCookie cookie) {
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheRootTotalRequests, PerformanceCounters.DataCacheRootTotalQueue, PerformanceCounters.DataCacheRootProcessCookieRequests, PerformanceCounters.DataCacheRootProcessCookieQueue))
#endif
			{
				DataCacheResult result = new DataCacheResult();
				ProcessChildResultSinceCookie(result, cookie);
				return result;
			}
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			IDataStoreSchemaExplorer nestedSource = Nested as IDataStoreSchemaExplorer;
			if(nestedSource == null)
				return null;
			return nestedSource.GetStorageTablesList(includeViews);
		}
		public override DBTable[] GetStorageTables(params string[] tables) {
			IDataStoreSchemaExplorer nestedSource = Nested as IDataStoreSchemaExplorer;
			if(nestedSource == null)
				return null;
			return nestedSource.GetStorageTables(tables);
		}
		public override AutoCreateOption AutoCreateOption { get { return Nested.AutoCreateOption; } }
		public override void Configure(DataCacheConfiguration configuration) {
			using(LockForChange()) {
				cacheConfiguration = configuration;
				ResetCore();
			}
		}
	}
	public class DataCacheNode : DataCacheBase
#if DEBUG
, IDataStoreForTests
#endif
 {
#if DEBUG
		public void ClearDatabase() {
			using(LockForChange()) {
				((IDataStoreForTests)Nested).ClearDatabase();
				ResetCore();
			}
		}
#endif
		ICacheToCacheCommunicationCore _nested;
#if !SL && !DXPORTABLE
		int objectsInCacheForPerfCounters = 0;
#endif
		protected readonly Dictionary<string, Dictionary<string, CacheRecord>> RecordsByTables = new Dictionary<string, Dictionary<string, CacheRecord>>();
		protected CacheRecord First, Last;
		protected DateTime LastUpdateTime = DateTime.MinValue;
		public TimeSpan MaxCacheLatency = new TimeSpan(0, 0, 30);
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use GlobalTotalMemoryPurgeThreshold field instead")]
		public static long GlobalTotalMemoryPurgeTreshhold {
			get { return GlobalTotalMemoryPurgeThreshold; }
			set { GlobalTotalMemoryPurgeThreshold = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use TotalMemoryPurgeThreshold field instead")]
		public long TotalMemoryPurgeTreshhold {
			get { return TotalMemoryPurgeThreshold; }
			set { TotalMemoryPurgeThreshold = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use TotalMemoryNotPurgeThreshold field instead")]
		public long TotalMemoryNotPurgeTreshhold {
			get { return TotalMemoryNotPurgeThreshold; }
			set { TotalMemoryNotPurgeThreshold = value; }
		}
#if CF
		public static long GlobalTotalMemoryPurgeThreshold = 32L*1024L*1024L;
		public long TotalMemoryPurgeThreshold = 32L*1024L*1024L;
		public long TotalMemoryNotPurgeThreshold = 4L * 1024L * 1024L;
#else
		public static long GlobalTotalMemoryPurgeThreshold = long.MaxValue;
		public long TotalMemoryPurgeThreshold = long.MaxValue;
		public long TotalMemoryNotPurgeThreshold = 64L * 1024L * 1024L;
#endif
		public int MinCachedRequestsAfterPurge = 16;
		protected ICacheToCacheCommunicationCore Nested { get { return _nested; } }
		bool isAutoCreateOptionCached = false;
		AutoCreateOption _AutoCreateOption = AutoCreateOption.None;
		object _AutoCreateOptionLock = new object();
		public override AutoCreateOption AutoCreateOption {
			get {
				if(!isAutoCreateOptionCached) {
					lock(_AutoCreateOptionLock) {
						if(!isAutoCreateOptionCached) {
							isAutoCreateOptionCached = true;
							try {
								_AutoCreateOption = ((IDataStore)Nested).AutoCreateOption;
							} catch { }
						}
					}
				}
				return _AutoCreateOption;
			}
		}
		public DataCacheNode(ICacheToCacheCommunicationCore parentCache)
			: base(parentCache as ICommandChannel) {
			this._nested = parentCache;
#if !SL && !DXPORTABLE
			PerformanceCounters.DataCacheNodeCount.Increment();
			PerformanceCounters.DataCacheNodeCreated.Increment();
#endif
		}
#if !SL && !DXPORTABLE
		~DataCacheNode() {
			PerformanceCounters.DataCacheNodeCachedCount.Decrement(objectsInCacheForPerfCounters);
			PerformanceCounters.DataCacheNodeCachedRemoved.Increment(objectsInCacheForPerfCounters);
			objectsInCacheForPerfCounters = 0;
			PerformanceCounters.DataCacheNodeCount.Decrement();
			PerformanceCounters.DataCacheNodeFinalized.Increment();
		}
#endif
		protected override DataCacheUpdateSchemaResult UpdateSchemaCore(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist) {
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheNodeTotalRequests, PerformanceCounters.DataCacheNodeTotalQueue, PerformanceCounters.DataCacheNodeSchemaUpdateRequests, PerformanceCounters.DataCacheNodeSchemaUpdateQueue)) {
#endif
			DataCacheUpdateSchemaResult result =
					Nested.UpdateSchema(GetCurrentCookieSafe(), tables, dontCreateIfFirstTableNotExist);
				ProcessParentResult(result);
				ProcessChildResultSinceCookie(result, cookie);
				return result;
#if !SL && !DXPORTABLE
			}
#endif
		}
		protected override DataCacheSelectDataResult SelectDataCore(DataCacheCookie cookie, SelectStatement[] selects) {
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheNodeTotalRequests, PerformanceCounters.DataCacheNodeTotalQueue, PerformanceCounters.DataCacheNodeSelectRequests, PerformanceCounters.DataCacheNodeSelectQueue)) {
				PerformanceCounters.DataCacheNodeSelectQueries.Increment(selects.Length);
#endif
			CacheRecord[] newRecords = new CacheRecord[selects.Length];
				for(int i = 0; i < selects.Length; ++i) {
					SelectStatement stmt = selects[i];
					if(IsGoodForCache(stmt))
						newRecords[i] = new CacheRecord(stmt);
				}
				if(newRecords.Length > 0 && newRecords[0] != null) {
					bool isFirstRecordAvailable = false;
					using(LockForRead()) {
						isFirstRecordAvailable = GetCachedRecord(newRecords[0]) != null;
					}
					if(isFirstRecordAvailable)
						ProcessCurrentCookieIfNeeded();
				}
				DataCacheSelectDataResult result = new DataCacheSelectDataResult();
				SelectStatementResult[] results = new SelectStatementResult[selects.Length];
				for(int i = 0; i < selects.Length; ) {
					if(newRecords[i] != null) {
						using(LockForChange()) {
							CacheRecord rec = GetCachedRecord(newRecords[i]);
							if(rec != null) {
								if(i == 0)
									result.SelectingCookie = GetCurrentCookie();
								results[i] = rec.QueryResult;
								PromoteRecordToMRU(rec);
								++i;
#if !SL && !DXPORTABLE
								PerformanceCounters.DataCacheNodeCacheHit.Increment();
#endif
							continue;
							}
						}
					}
					int statementsToSendToParent = 1;
					using(LockForRead()) {
						for(; ; ) {
							int candidate = i + statementsToSendToParent;
							if(candidate >= selects.Length)
								break;
							if(newRecords[candidate] != null && GetCachedRecord(newRecords[candidate]) != null)
								break;
							statementsToSendToParent++;
							if(statementsToSendToParent >= 128 + 64) {
								statementsToSendToParent = 128;
								break;
							}
						}
					}
					SelectStatement[] nestedCallSelects = new SelectStatement[statementsToSendToParent];
					for(int j = 0; j < statementsToSendToParent; ++j) {
						nestedCallSelects[j] = selects[i + j];
					}
					DataCacheSelectDataResult rootResult = Nested.SelectData(GetCurrentCookieSafe(), nestedCallSelects);
					ProcessParentResult(rootResult);
					if(i == 0)
						result.SelectingCookie = rootResult.SelectingCookie;
					for(int j = 0; j < statementsToSendToParent; ++j) {
						SelectStatementResult nr = rootResult.SelectedData.ResultSet[j];
						results[i] = nr;
						CacheRecord newRecord = newRecords[i];
						bool passthrough;
						if(newRecord == null) {
							passthrough = true;
						} else {
							using(LockForChange()) {
								if(GetCachedRecord(newRecord) == null && IsGoodForCache(selects[i], nr) && IsNotInvalidated(newRecord, rootResult.SelectingCookie)) {
									newRecord.QueryResult = nr;
									RegisterNewRecord(newRecord);
									passthrough = false;
								} else {
									passthrough = true;
								}
							}
						}
#if !SL && !DXPORTABLE
						if(passthrough) {
							PerformanceCounters.DataCacheNodeCachePassthrough.Increment();
						} else {
							PerformanceCounters.DataCacheNodeCacheMiss.Increment();
						}
#else
					if (passthrough) { }
#endif
						++i;
					}
				}
				result.SelectedData = new SelectedData(results);
				ProcessChildResultSinceCookie(result, cookie);
				return result;
#if !SL && !DXPORTABLE
			}
#endif
		}
		bool IsNotInvalidated(CacheRecord newRecord, DataCacheCookie actualCookie) {
			ValidateLockedRead();
			if(actualCookie.Guid != this.MyGuid)
				return false;
			if(actualCookie.Age == this.Age)
				return true;
			System.Diagnostics.Debug.Assert(actualCookie.Age < this.Age);
			foreach(string tblName in newRecord.TablesInStatement) {
				long tblAge;
				if(TablesAges.TryGetValue(tblName, out tblAge)) {
					if(tblAge > actualCookie.Age)
						return false;
				}
			}
			return true;
		}
		protected virtual bool IsGoodForCache(SelectStatement stmt) {
			if(IsBadForCache(cacheConfiguration, stmt)) return false;
			List<JoinNode> listToCollectNodes = new List<JoinNode>();
			IndeterminateStatmentFinder indeterminateStatementFinder = new IndeterminateStatmentFinder(listToCollectNodes);
			foreach (CriteriaOperator criteria in stmt.Operands) {
				if (indeterminateStatementFinder.Process(criteria)) return false;
			}
			if (indeterminateStatementFinder.Process(stmt.Condition)) return false;
			foreach (CriteriaOperator criteria in stmt.GroupProperties) {
				if (indeterminateStatementFinder.Process(criteria)) return false;
			}
			if (indeterminateStatementFinder.Process(stmt.GroupCondition)) return false;
			foreach (SortingColumn column in stmt.SortProperties) {
				if (indeterminateStatementFinder.Process(column.Property)) return false;
			}
			foreach (JoinNode node in stmt.SubNodes) {
				if(!IsGoodForCacheJoinNode(indeterminateStatementFinder, node)) return false;
			}
			for(int i = 0; i < listToCollectNodes.Count; i++) {
				if(!IsGoodForCacheJoinNode(indeterminateStatementFinder, listToCollectNodes[i])) return false;
			}
			return true;
		}
		bool IsGoodForCacheJoinNode(IndeterminateStatmentFinder indeterminateStatementFinder, JoinNode node) {
			if(IsBadForCache(cacheConfiguration, node)) return false;
			if(indeterminateStatementFinder.Process(node.Condition)) return false;
			foreach (JoinNode subNode in node.SubNodes) {
				if(!IsGoodForCacheJoinNode(indeterminateStatementFinder, subNode)) return false;
			}
			return true;
		}
		protected virtual bool IsGoodForCache(SelectStatement stmt, SelectStatementResult stmtResult) {
			if(IsBadForCache(cacheConfiguration, stmt)) return false;
			return true;
		}
		protected override DataCacheModificationResult ModifyDataCore(DataCacheCookie cookie, ModificationStatement[] dmlStatements) {
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheNodeTotalRequests, PerformanceCounters.DataCacheNodeTotalQueue, PerformanceCounters.DataCacheNodeModifyRequests, PerformanceCounters.DataCacheNodeModifyQueue)) {
				PerformanceCounters.DataCacheNodeModifyStatements.Increment(dmlStatements.Length);
#endif
				DataCacheModificationResult result =
					Nested.ModifyData(GetCurrentCookieSafe(), dmlStatements);
				using(LockForChange()) {
					ProcessParentResult(result);
					ProcessChildResultSinceCookie(result, cookie);
				}
				return result;
#if !SL && !DXPORTABLE
			}
#endif
		}
		protected override DataCacheResult NotifyDirtyTablesCore(DataCacheCookie cookie, params string[] dirtyTablesNames) {
			if(dirtyTablesNames == null)
				dirtyTablesNames = new string[0];
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheNodeTotalRequests, PerformanceCounters.DataCacheNodeTotalQueue, PerformanceCounters.DataCacheNodeNotifyDirtyTablesRequests, PerformanceCounters.DataCacheNodeNotifyDirtyTablesQueue)) {
				PerformanceCounters.DataCacheNodeNotifyDirtyTablesTables.Increment(dirtyTablesNames.Length);
#endif
			DataCacheResult result =
					Nested.NotifyDirtyTables(GetCurrentCookieSafe(), dirtyTablesNames);
				using(LockForChange()) {
					ProcessParentResult(result);
					ProcessChildResultSinceCookie(result, cookie);
				}
				return result;
#if !SL && !DXPORTABLE
			}
#endif
		}
		public void CatchUp() {
			NotifyDirtyTables();
		}
		protected override DataCacheResult ProcessCookieCore(DataCacheCookie cookie) {
#if !SL && !DXPORTABLE
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.DataCacheNodeTotalRequests, PerformanceCounters.DataCacheNodeTotalQueue, PerformanceCounters.DataCacheNodeProcessCookieRequests, PerformanceCounters.DataCacheNodeProcessCookieQueue)) {
#endif
			ProcessCurrentCookieIfNeeded();
				DataCacheResult result = new DataCacheResult();
				ProcessChildResultSinceCookie(result, cookie);
				return result;
#if !SL && !DXPORTABLE
			}
#endif
		}
		protected bool IsCacheFresh {
			get {
				if(MaxCacheLatency == TimeSpan.Zero)
					return false;
				DateTime now = DateTime.UtcNow;
				DateTime lastUpdated = LastUpdateTime;
				if(now - lastUpdated >= MaxCacheLatency)
					return false;
				if(lastUpdated - now >= MaxCacheLatency)
					return false;
				return true;
			}
		}
		protected void ProcessCurrentCookieIfNeeded() {
			using(LockForRead()) {
				if(IsCacheFresh)
					return;
			}
			DataCacheResult result = Nested.ProcessCookie(GetCurrentCookieSafe());
			ProcessParentResult(result);
		}
		protected void ProcessParentResult(DataCacheResult result) {
			using(LockForChange()) {
				if(result.CacheConfig != null)
					cacheConfiguration = result.CacheConfig;
				if(this.MyGuid != result.Cookie.Guid) {
					ResetCore();
					this.MyGuid = result.Cookie.Guid;
					this.Age = result.Cookie.Age;
				} else {
					if(result.Cookie.Age > this.Age) {
						this.Age = result.Cookie.Age;
						foreach(TableAge ta in result.UpdatedTableAges) {
							long currentAge;
							if(!this.TablesAges.TryGetValue(ta.Name, out currentAge) || currentAge < ta.Age) {
								this.TablesAges[ta.Name] = ta.Age;
								UnregisterRecordsForTable(ta.Name);
							}
						}
					}
				}
				LastUpdateTime = DateTime.UtcNow;
			}
		}
		protected override void ResetCore() {
			base.ResetCore();
#if !SL && !DXPORTABLE
			PerformanceCounters.DataCacheNodeCachedCount.Decrement(objectsInCacheForPerfCounters);
			PerformanceCounters.DataCacheNodeCachedRemoved.Increment(objectsInCacheForPerfCounters);
			objectsInCacheForPerfCounters = 0;
#endif
			RecordsByTables.Clear();
			First = null;
			Last = null;
		}
		protected CacheRecord GetCachedRecord(CacheRecord sample) {
			ValidateLockedRead();
			return GetCachedRecord(sample.TableName, sample.HashString);
		}
		protected CacheRecord GetCachedRecord(string rootTableName, string statementUniqueString) {
			Dictionary<string, CacheRecord> nodes;
			if(!RecordsByTables.TryGetValue(rootTableName, out nodes))
				return null;
			CacheRecord oldNode;
			nodes.TryGetValue(statementUniqueString, out oldNode);
			return oldNode;
		}
		protected void PromoteRecordToMRU(CacheRecord record) {
			if(record.Prev == null)
				return;
			record.Prev.Next = record.Next;
			if(record.Next != null)
				record.Next.Prev = record.Prev;
			else
				this.Last = record.Prev;
			record.Next = this.First;
			record.Prev = null;
			this.First.Prev = record;
			this.First = record;
		}
		protected void RegisterNewRecord(CacheRecord newRecord) {
			foreach(string tableName in newRecord.TablesInStatement) {
				Dictionary<string, CacheRecord> tableRecords;
				if(!RecordsByTables.TryGetValue(tableName, out tableRecords)) {
					tableRecords = new Dictionary<string, CacheRecord>();
					RecordsByTables.Add(tableName, tableRecords);
				}
				tableRecords.Add(newRecord.HashString, newRecord);
			}
			if(this.First != null)
				this.First.Prev = newRecord;
			newRecord.Next = this.First;
			this.First = newRecord;
			if(this.Last == null)
				this.Last = newRecord;
#if !SL && !DXPORTABLE
			objectsInCacheForPerfCounters++;
			DevExpress.Xpo.Helpers.PerformanceCounters.DataCacheNodeCachedCount.Increment();
			DevExpress.Xpo.Helpers.PerformanceCounters.DataCacheNodeCachedAdded.Increment();
#endif
			PurgeIfNeeded();
		}
		protected void UnregisterRecordsForTable(string table) {
			Dictionary<string, CacheRecord> tableRecords;
			if(!RecordsByTables.TryGetValue(table, out tableRecords))
				return;
			foreach(CacheRecord record in new List<CacheRecord>(tableRecords.Values)) {
				RemoveFromCache(record);
			}
			System.Diagnostics.Debug.Assert(tableRecords.Count == 0);
		}
		protected void RemoveFromCache(CacheRecord record) {
			foreach(string table in record.TablesInStatement) {
				Dictionary<string, CacheRecord> records = RecordsByTables[table];
				System.Diagnostics.Debug.Assert(records.ContainsKey(record.HashString));
				records.Remove(record.HashString);
			}
			if(record.Prev == null) {
				this.First = record.Next;
			} else {
				record.Prev.Next = record.Next;
			}
			if(record.Next == null) {
				this.Last = record.Prev;
			} else {
				record.Next.Prev = record.Prev;
			}
#if !SL && !DXPORTABLE
			objectsInCacheForPerfCounters--;
			DevExpress.Xpo.Helpers.PerformanceCounters.DataCacheNodeCachedCount.Decrement();
			DevExpress.Xpo.Helpers.PerformanceCounters.DataCacheNodeCachedRemoved.Increment();
#endif
		}
		protected bool IsWorkingSetOverlap(long totalMemory) {
#if CF || SL || DXPORTABLE
			return false;
#else
			return totalMemory > Environment.WorkingSet;
#endif
		}
		protected virtual void PurgeIfNeeded() {
			System.Diagnostics.Debug.Assert(Last != null);
			if(!purgingEnabled)
				return;
			long totalMemory = GC.GetTotalMemory(false);
			if(totalMemory <= TotalMemoryNotPurgeThreshold)
				return;
			if(totalMemory >= TotalMemoryPurgeThreshold || totalMemory >= GlobalTotalMemoryPurgeThreshold || IsWorkingSetOverlap(totalMemory)) {
				DoPurge();
			}
		}
		bool purgingEnabled = true;
		class GCFlagger {
			public readonly DataCacheNode Node;
			public GCFlagger(DataCacheNode owner) {
				this.Node = owner;
				System.Diagnostics.Debug.Assert(this.Node.purgingEnabled);
				this.Node.purgingEnabled = false;
			}
			~GCFlagger() {
				try {
					this.Node.purgingEnabled = true;
				} catch { }
			}
		}
		protected virtual void DoPurge() {
			System.Diagnostics.Debug.Assert(Last != null);
			System.Diagnostics.Debug.Assert(purgingEnabled == true);
			new Thread(new ThreadStart(Purge)).Start();
		}
		protected virtual void Purge() {
			using(LockForChange()) {
				if(Last == null)
					return;
				if(!purgingEnabled)
					return;
				new GCFlagger(this);
				CacheRecord currentRecord = this.First;
				int recordsCount = 1;
				for(; ; ) {
					System.Diagnostics.Debug.Assert(currentRecord != null);
					if(currentRecord.Next == null) {
						System.Diagnostics.Debug.Assert(ReferenceEquals(currentRecord, this.Last));
						break;
					}
					++recordsCount;
					currentRecord = currentRecord.Next;
				}
				if(recordsCount <= MinCachedRequestsAfterPurge)
					return;
				int recordsToLeft = Math.Max(recordsCount / 3 * 2, MinCachedRequestsAfterPurge);
				while(recordsCount > recordsToLeft) {
					RemoveFromCache(this.Last);
					--recordsCount;
				}
			}
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			IDataStoreSchemaExplorer nestedSource = Nested as IDataStoreSchemaExplorer;
			if(nestedSource == null)
				return null;
			return nestedSource.GetStorageTablesList(includeViews);
		}
		public override DBTable[] GetStorageTables(params string[] tables) {
			IDataStoreSchemaExplorer nestedSource = Nested as IDataStoreSchemaExplorer;
			if(nestedSource == null)
				return null;
			return nestedSource.GetStorageTables(tables);
		}
		public override void Configure(DataCacheConfiguration configuration) {
			throw new NotImplementedException("The method or operation is not implemented.");
		}
	}
	public class DataCacheNodeLocal : DataCacheNode {
		public DataCacheNodeLocal(ICacheToCacheCommunicationCore parentCache) : base(parentCache) { }
		protected override bool IsGoodForCache(SelectStatement stmt) {
			return base.IsGoodForCache(stmt) && IsProbablyGroupByStatement(stmt);
		}
		public static bool IsProbablyGroupByStatement(SelectStatement stmt) {
			if(stmt.GroupProperties.Count > 0)
				return true;
			if(stmt.Operands.Count > 0 && stmt.Operands[0] is QuerySubQueryContainer)
				return true;
			return false;
		}
	}
	public class IndeterminateStatmentFinder : IQueryCriteriaVisitor<bool>{
		List<JoinNode> listToNodeCollection;
		public IndeterminateStatmentFinder(List<JoinNode> listToNodeCollection) {
			this.listToNodeCollection = listToNodeCollection;
		}
		public bool Visit(BetweenOperator theOperator) {
			if(Process(theOperator.BeginExpression)) return true;
			return Process(theOperator.EndExpression);
		}
		public bool Visit(BinaryOperator theOperator) {
			if(Process(theOperator.LeftOperand)) return true;
			return Process(theOperator.RightOperand);
		}
		public bool Visit(UnaryOperator theOperator) {
			return Process(theOperator.Operand);
		}
		public bool Visit(InOperator theOperator) {
			if(Process(theOperator.LeftOperand)) return true;
			foreach (CriteriaOperator op in theOperator.Operands) {
				if(Process(op)) return true;
			}
			return false;
		}
		public bool Visit(GroupOperator theOperator) {
			foreach (CriteriaOperator op in theOperator.Operands) {
				if(Process(op)) return true;
			}
			return false;
		}
		public bool Visit(OperandValue theOperand) {
			return false;
		}
		public bool Visit(FunctionOperator theOperator) {
			switch (theOperator.OperatorType) {
				case FunctionOperatorType.UtcNow:
				case FunctionOperatorType.Now:
				case FunctionOperatorType.Rnd:
				case FunctionOperatorType.Today:
				case FunctionOperatorType.CustomNonDeterministic:
					return true;
			}
			foreach (CriteriaOperator op in theOperator.Operands) {
				if(Process(op)) return true;
			}
			return false;
		}
		public bool Visit(QueryOperand theOperand) {
			return false;
		}
		public bool Visit(QuerySubQueryContainer theOperand) {
			if(listToNodeCollection != null && theOperand.Node != null) listToNodeCollection.Add(theOperand.Node);
			return Process(theOperand.AggregateProperty);
		}
		public bool Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return false;
			return operand.Accept(this);
		}
	}
}
