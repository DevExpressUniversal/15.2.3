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
using System.Data;
using System.Reflection;
using System.Threading;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System.Collections.Generic;
using DevExpress.Xpo.Exceptions;
using System.ComponentModel;
namespace DevExpress.Xpo.Helpers {
	public interface IDataLayerForTests : IDataLayer {
		void ClearDatabase();
	}
	public interface IDataLayerProvider : DevExpress.Xpo.Metadata.Helpers.IXPDictionaryProvider {
		IDataLayer DataLayer { get;}
	}
	public abstract class BaseDataLayer : IDataLayer, ICommandChannel {
		protected readonly Dictionary<XPClassInfo, XPClassInfo> EnsuredTypes = new Dictionary<XPClassInfo, XPClassInfo>();
		readonly XPDictionary dictionary;
		readonly IDataStore provider;
		readonly ICommandChannel nestedCommandChannel;
		AutoCreateOption? autoCreateOption;
		protected BaseDataLayer(XPDictionary dictionary, IDataStore provider, params Assembly[] persistentObjectsAssemblies) {
			if(dictionary == null)
				dictionary = XpoDefault.GetDictionary();
			this.dictionary = dictionary;
			if(persistentObjectsAssemblies != null && persistentObjectsAssemblies.Length > 0) {
				this.dictionary.CollectClassInfos(persistentObjectsAssemblies);
			}
			this.provider = provider;
			this.nestedCommandChannel = provider as ICommandChannel;
			BeforeClassInfoSubscribe();
			this.dictionary.ClassInfoChanged += new ClassInfoEventHandler(OnClassInfoChanged);
		}
		protected virtual void BeforeClassInfoSubscribe() { }
		public void Dispose() {
			this.Dispose(true);
		}
		~BaseDataLayer() {
			this.Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				Dictionary.ClassInfoChanged -= new ClassInfoEventHandler(OnClassInfoChanged);
			}
		}
		public IDataStore ConnectionProvider { get { return provider; } }
		public XPDictionary Dictionary { get { return dictionary; } }
		protected abstract void OnClassInfoChanged(object sender, ClassInfoEventArgs e);
		protected void RegisterEnsuredTypes(ICollection<XPClassInfo> justEnsuredTypes) {
			foreach(XPClassInfo type in justEnsuredTypes) {
				EnsuredTypes.Add(type, type);
#if !SL
				if(SchemaInit != null) {
					using(IDbCommand command = CreateCommand()) {
						SchemaInit(this, new SchemaInitEventArgs(type, command));
					}
				}
#endif
			}
		}
		public abstract UpdateSchemaResult UpdateSchema(bool dontCreate, params XPClassInfo[] types);
		public abstract SelectedData SelectData(params SelectStatement[] selects);
		public abstract ModificationResult ModifyData(params ModificationStatement[] dmlStatements);
		public virtual AutoCreateOption AutoCreateOption {
			get {
				if(!autoCreateOption.HasValue)
					autoCreateOption = provider.AutoCreateOption;
				return autoCreateOption.Value;
			}
		}
#if !SL
		public event SchemaInitEventHandler SchemaInit;
		public abstract IDbConnection Connection { get; }
		public abstract IDbCommand CreateCommand();
#endif
		volatile static int seq = 0;
		int seqNum = ++seq;
		public override string ToString() {
			return base.ToString() + '(' + seqNum.ToString() + ')';
		}
		IDataLayer IDataLayerProvider.DataLayer {
			get { return this; }
		}
		readonly Dictionary<object, object> staticData = new Dictionary<object, object>();
		protected void ClearStaticData() {
			staticData.Clear();
		}
		public void SetDataLayerWideData(object key, object data) {
			staticData[key] = data;
		}
		public object GetDataLayerWideData(object key) {
			object res;
			return staticData.TryGetValue(key, out res) ? res : null;
		}
		readonly static object loadedTypesKey = new object();
		public static void SetDataLayerWideObjectTypes(IDataLayer layer, Dictionary<XPClassInfo, XPObjectType> loadedTypes) {
			layer.SetDataLayerWideData(loadedTypesKey, loadedTypes);
		}
		public static Dictionary<XPClassInfo, XPObjectType> GetDataLayerWideObjectTypes(IDataLayer layer) {
			return (Dictionary<XPClassInfo, XPObjectType>)layer.GetDataLayerWideData(loadedTypesKey);
		}
		readonly static object staticTypesKey = new object();
		static Dictionary<XPClassInfo, XPClassInfo> GetStaticTypesDictionary(IDataLayer layer) {
			Dictionary<XPClassInfo, XPClassInfo> rv = (Dictionary<XPClassInfo, XPClassInfo>)layer.GetDataLayerWideData(staticTypesKey);
			if(rv == null) {
				rv = new Dictionary<XPClassInfo, XPClassInfo>();
				layer.SetDataLayerWideData(staticTypesKey, rv);
			}
			return rv;
		}
		public static void RegisterStaticTypes(IDataLayer layer, params XPClassInfo[] types) {
			Dictionary<XPClassInfo, XPClassInfo> staticTypes = GetStaticTypesDictionary(layer);
			foreach(XPClassInfo ci in types) {
				staticTypes[ci] = ci;
			}
		}
		public static bool IsStaticType(IDataLayer layer, XPClassInfo type) {
			return GetStaticTypesDictionary(layer).ContainsKey(type);
		}
		readonly static object staticCacheKey = new object();
		public static IObjectMap GetStaticCache(IDataLayer layer, XPClassInfo info) {
			Dictionary<XPClassInfo, IObjectMap> staticCache = (Dictionary<XPClassInfo, IObjectMap>)layer.GetDataLayerWideData(staticCacheKey);
			if(staticCache == null) {
				staticCache = new Dictionary<XPClassInfo, IObjectMap>();
				layer.SetDataLayerWideData(staticCacheKey, staticCache);
			}
			IObjectMap cache;
			if(!staticCache.TryGetValue(info, out cache)) {
				cache = new StrongObjectMap();
				staticCache.Add(info, cache);
			}
			return cache;
		}
		sealed class StrongObjectMap : Dictionary<object, object>, IObjectMap {
			void IObjectMap.Add(object theObject, object id) {
				if(!ContainsKey(id)) {
					base.Add(id, theObject);
				} else {
					throw new ObjectCacheException(id, theObject, this[id]);
				}
			}
			void IObjectMap.Remove(object id) {
				Remove(id);
			}
			public object Get(object id) {
				object res;
				return TryGetValue(id, out res) ? res : null;
			}
			public int CompactCache() {
				return 0;
			}
			public void ClearCache() {
			}
		}
		object ICommandChannel.Do(string command, object args) {
			return Do(command, args);
		}
		protected virtual object Do(string command, object args) {
			if(nestedCommandChannel == null) {
				if(provider == null) {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				} else {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, provider.GetType().FullName));
				}
			}
			return nestedCommandChannel.Do(command, args);
		}
	}
}
namespace DevExpress.Xpo {
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Xpo.Helpers;
	public interface IDataLayer : IDisposable, IDataLayerProvider {
		UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params XPClassInfo[] types);
		SelectedData SelectData(params SelectStatement[] selects);
		ModificationResult ModifyData(params ModificationStatement[] dmlStatements);
#if !SL
		event SchemaInitEventHandler SchemaInit;
		IDbConnection Connection { get;}
		IDbCommand CreateCommand();
#endif
		AutoCreateOption AutoCreateOption { get; }
		void SetDataLayerWideData(object key, object data);
		object GetDataLayerWideData(object key);
	}
	public class SimpleDataLayer : BaseDataLayer
#if !SL
		, IDataLayerForTests
#else
		, IDataLayer
#endif
	{
		public SimpleDataLayer(IDataStore provider) : this(null, provider) { }
		public SimpleDataLayer(XPDictionary dictionary, IDataStore provider) : base(dictionary, provider) { }
		protected override void OnClassInfoChanged(object sender, ClassInfoEventArgs e) {
			ThreadSafetyRoadBlockEnter();
			try {
				EnsuredTypes.Remove(e.ClassInfo);
			} finally {
				ThreadSafetyRoadBlockLeave();
			}
		}
		public override UpdateSchemaResult UpdateSchema(bool dontCreate, params XPClassInfo[] types) {
			if(AutoCreateOption == AutoCreateOption.SchemaAlreadyExists) {
				foreach(XPClassInfo t in types)
					t.CheckAbstractReference();
				return UpdateSchemaResult.SchemaExists;
			}
			ICollection<XPClassInfo> typesNeedEnsure = Dictionary.ExpandTypesToEnsure(types, EnsuredTypes);
			if(typesNeedEnsure.Count == 0)
				return UpdateSchemaResult.SchemaExists;
			DBTable[] tables = XPDictionary.CollectTables(typesNeedEnsure);
			ThreadSafetyRoadBlockEnter();
			try {
				if(ConnectionProvider.UpdateSchema(dontCreate, tables) == UpdateSchemaResult.FirstTableNotExists)
					return UpdateSchemaResult.FirstTableNotExists;
				RegisterEnsuredTypes(typesNeedEnsure);
				return UpdateSchemaResult.SchemaExists;
			} finally {
				ThreadSafetyRoadBlockLeave();
			}
		}
		public override SelectedData SelectData(params SelectStatement[] selects) {
			ThreadSafetyRoadBlockEnter();
			try {
				return ConnectionProvider.SelectData(selects);
			} finally {
				ThreadSafetyRoadBlockLeave();
			}
		}
		public override ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			ThreadSafetyRoadBlockEnter();
			try {
				return ConnectionProvider.ModifyData(dmlStatements);
			} finally {
				ThreadSafetyRoadBlockLeave();
			}
		}
#if !SL
		public void ClearDatabase() {
			ThreadSafetyRoadBlockEnter();
			try {
				EnsuredTypes.Clear();
				ClearStaticData();
				((IDataStoreForTests)ConnectionProvider).ClearDatabase();
			} finally {
				ThreadSafetyRoadBlockLeave();
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SimpleDataLayerConnection")]
#endif
public override IDbConnection Connection {
			get {
				ISqlDataStore connSource = ConnectionProvider as ISqlDataStore;
				if(connSource != null)
					try {
						return connSource.Connection;
					} catch { }
				return null;
			}
		}
		public override IDbCommand CreateCommand() {
			ISqlDataStore commandSource = ConnectionProvider as ISqlDataStore;
			if(commandSource != null)
				try {
					return commandSource.CreateCommand();
				} catch { }
			return null;
		}
#endif
		void ThreadSafetyRoadBlockEnter() {
		}
		void ThreadSafetyRoadBlockLeave() {
		}
	}
#if !CF && !SL
	public class ThreadSafeDataLayer : BaseDataLayer, ICommandChannel {
		readonly ReaderWriterLock ensuredTypesRwl = new ReaderWriterLock();
		public ThreadSafeDataLayer(XPDictionary dictionary, IDataStore provider, params Assembly[] persistentObjectsAssemblies) : base(dictionary, provider, persistentObjectsAssemblies) { }
		protected override void OnClassInfoChanged(object sender, ClassInfoEventArgs e) {
			throw new InvalidOperationException(Res.GetString(Res.ThreadSafeDataLayer_DictionaryModified, e.ClassInfo.FullName));
		}
		public override UpdateSchemaResult UpdateSchema(bool dontCreate, params XPClassInfo[] types) {
			if(AutoCreateOption == AutoCreateOption.SchemaAlreadyExists) {
				foreach(XPClassInfo t in types)
					t.CheckAbstractReference();
				return UpdateSchemaResult.SchemaExists;
			}
			ensuredTypesRwl.AcquireReaderLock(Timeout.Infinite);
			try {
				int ensuredTypesCount = EnsuredTypes.Count;
				ICollection<XPClassInfo> typesNeedEnsure = Dictionary.ExpandTypesToEnsure(types, EnsuredTypes);
				if(typesNeedEnsure.Count == 0)
					return UpdateSchemaResult.SchemaExists;
				ensuredTypesRwl.UpgradeToWriterLock(Timeout.Infinite);
				if(EnsuredTypes.Count != ensuredTypesCount) {
					typesNeedEnsure = Dictionary.ExpandTypesToEnsure(types, EnsuredTypes);
					if(typesNeedEnsure.Count == 0)
						return UpdateSchemaResult.SchemaExists;
				}
				DBTable[] tables = XPDictionary.CollectTables(typesNeedEnsure);
				if(ConnectionProvider.UpdateSchema(dontCreate, tables) == UpdateSchemaResult.FirstTableNotExists)
					return UpdateSchemaResult.FirstTableNotExists;
				RegisterEnsuredTypes(typesNeedEnsure);
				return UpdateSchemaResult.SchemaExists;
			} finally {
				ensuredTypesRwl.ReleaseReaderLock();
			}
		}
		public override SelectedData SelectData(params SelectStatement[] selects) {
			ensuredTypesRwl.AcquireReaderLock(Timeout.Infinite);
			try {
				return ConnectionProvider.SelectData(selects);
			} finally {
				ensuredTypesRwl.ReleaseReaderLock();
			}
		}
		public override ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			ensuredTypesRwl.AcquireReaderLock(Timeout.Infinite);
			try {
				return ConnectionProvider.ModifyData(dmlStatements);
			} finally {
				ensuredTypesRwl.ReleaseReaderLock();
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("ThreadSafeDataLayerConnection")]
#endif
public override IDbConnection Connection { get { return null; } }
		public override IDbCommand CreateCommand() { return null; }
		protected override void BeforeClassInfoSubscribe() {
			base.BeforeClassInfoSubscribe();
			using(Session touchAllTypesSession = new StrongSession(SimpleObjectLayer.FromDataLayer(this))) {
				touchAllTypesSession.TypesManager.EnsureIsTypedObjectValid();
				Dictionary<XPClassInfo, XPClassInfo> touchedHolder = new Dictionary<XPClassInfo, XPClassInfo>();
				foreach(XPClassInfo ci in new ArrayList(Dictionary.Classes))
					ci.TouchRecursive(touchedHolder);
			}
		}
		protected override object Do(string command, object args) {
			switch(command) {
				case CommandChannelHelper.Command_ExplicitBeginTransaction:
				case CommandChannelHelper.Command_ExplicitCommitTransaction:
				case CommandChannelHelper.Command_ExplicitRollbackTransaction:
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, "ThreadSafeDataLayer"));
				default:
					return base.Do(command, args);
			}
		}
	}
#endif
}
