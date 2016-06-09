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
using System.Text.RegularExpressions;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Metadata;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
namespace DevExpress.Xpo.Helpers {
	using DevExpress.Xpo;
	public class DataLayerWrapperS18452 : IDataLayerForTests, ICommandChannel {
		public readonly IDataLayer Nested;
		readonly ICommandChannel nestedCommandChannel;
		protected IDisposable[] ToDispose;
		public DataLayerWrapperS18452(IDataLayer nested, IDisposable[] toDispose) {
			this.Nested = nested;
			this.nestedCommandChannel = Nested as ICommandChannel;
			this.ToDispose = toDispose;
		}
		public void ClearDatabase() {
			((IDataLayerForTests)Nested).ClearDatabase();
		}
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params XPClassInfo[] types) {
			return Nested.UpdateSchema(dontCreateIfFirstTableNotExist, types);
		}
		public SelectedData SelectData(params SelectStatement[] selects) {
			return Nested.SelectData(selects);
		}
		public ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			return Nested.ModifyData(dmlStatements);
		}
#if !SL
		public event SchemaInitEventHandler SchemaInit {
			add { Nested.SchemaInit += value; }
			remove { Nested.SchemaInit -= value; }
		}
		public IDbConnection Connection {
			get { return Nested.Connection; }
		}
		public IDbCommand CreateCommand() {
			return Nested.CreateCommand();
		}
#endif
		public AutoCreateOption AutoCreateOption {
			get { return Nested.AutoCreateOption; }
		}
		public void Dispose() {
			if(ToDispose != null) {
				foreach(IDisposable d in ToDispose) {
					if(d != null)
						d.Dispose();
				}
				ToDispose = null;
			}
		}
		public IDataLayer DataLayer {
			get { return this; }
		}
		public XPDictionary Dictionary {
			get { return Nested.Dictionary; }
		}
		public void SetDataLayerWideData(object key, object data) {
			Nested.SetDataLayerWideData(key, data);
		}
		public object GetDataLayerWideData(object key) {
			return Nested.GetDataLayerWideData(key);
		}
		object ICommandChannel.Do(string command, object args) {
			if(nestedCommandChannel == null) {
				if(Nested == null) {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				} else {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, Nested.GetType().FullName));
				}
			}
			return nestedCommandChannel.Do(command, args);
		}
	}
}
namespace DevExpress.Xpo {
	using DevExpress.Xpo.Helpers;
#if !SL
	using DevExpress.Xpo.DB;
	using System.Security;   
#endif
#if !CF
	using System.ServiceModel;
#endif
	public enum OptimisticLockingReadBehavior { Default, Ignore, ReloadObject, Mixed, ThrowException, MergeCollisionIgnore, MergeCollisionThrowException, MergeCollisionReload }
	public enum OptimisticLockingReadMergeBehavior { Default, Ignore, ThrowException, Reload }
	[Obsolete("Use IdentityMapBehavior instead")]
	public enum CacheBehavior { Default, 
#if !SL
		Weak, 
#endif
		Strong 
	}
	public enum IdentityMapBehavior { Default, 
#if !SL
		Weak, 
#endif
		Strong
	}
	public enum GuidGenerationMode { FrameworkDefault, UuidCreateSequential }
	public static class XpoDefault {
		static bool trackPropertiesModifications;
		internal static readonly object SyncRoot = new object();
		static Session session = new Session();
		static IDataLayer dataLayer;
		static IObjectLayer objectLayer;
		static XPDictionary dictionary;
		static string connectionString;
		static OptimisticLockingReadBehavior _OptimisticLockingReadBehavior = DefaultOptimisticLockingReadBehavior;
		public const OptimisticLockingReadBehavior DefaultOptimisticLockingReadBehavior = DevExpress.Xpo.OptimisticLockingReadBehavior.Mixed;
		static IdentityMapBehavior _IdentityMapBehavior = DefaultIdentityMapBehavior;
		public const IdentityMapBehavior DefaultIdentityMapBehavior =
#if SL
			DevExpress.Xpo.IdentityMapBehavior.Strong;
#else
			DevExpress.Xpo.IdentityMapBehavior.Weak;
#endif
		static bool useFastAccessors = true;
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultSession")]
#endif
public static Session Session {
			get {
				return session;
			}
			set {
				if(ReferenceEquals(session, value))
					return;
				lock(SyncRoot) {
					if(session != null) {
						session.Dispose();
					}
					session = value;
				}
			}
		}
		internal static Session GetSession() {
			Session rv = Session;
			if(rv == null)
				throw new ArgumentNullException("XpoDefault.Session");
			return rv;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultDataLayer")]
#endif
public static IDataLayer DataLayer {
			get { return dataLayer; }
			set {
				if(ReferenceEquals(DataLayer, value))
					return;
				lock(SyncRoot) {
					if(ConnectionString != null)
						throw new InvalidOperationException(Res.GetString(Res.XpoDefault_CannotAssignPropertyWhileAnotherIsNotNull, "DataLayer", "ConnectionString"));
					if(Dictionary != null && value != null && !ReferenceEquals(Dictionary, value.Dictionary))
						throw new InvalidOperationException(Res.GetString(Res.XpoDefault_CannotAssignPropertyWhileAnotherIsNotNull, "DataLayer", "Dictionary"));
					dataLayer = value;
				}
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultObjectLayer")]
#endif
public static IObjectLayer ObjectLayer {
			get { return objectLayer; }
			set {
				if(ReferenceEquals(ObjectLayer, value))
					return;
				lock(SyncRoot) {
					if(ConnectionString != null)
						throw new InvalidOperationException(Res.GetString(Res.XpoDefault_CannotAssignPropertyWhileAnotherIsNotNull, "DataLayer", "ConnectionString"));
					if(Dictionary != null && value != null && !ReferenceEquals(Dictionary, value.Dictionary))
						throw new InvalidOperationException(Res.GetString(Res.XpoDefault_CannotAssignPropertyWhileAnotherIsNotNull, "DataLayer", "Dictionary"));
					objectLayer = value;
				}
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultDictionary")]
#endif
public static XPDictionary Dictionary {
			get { return dictionary; }
			set {
				if(ReferenceEquals(Dictionary, value))
					return;
				lock(SyncRoot) {
					if(DataLayer != null && value != null && !ReferenceEquals(DataLayer.Dictionary, value))
						throw new InvalidOperationException(Res.GetString(Res.XpoDefault_CannotAssignPropertyWhileAnotherIsNotNull, "Dictionary", "DataLayer"));
					dictionary = value;
				}
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultConnectionString")]
#endif
public static string ConnectionString {
			get { return connectionString; }
			set {
				if(ConnectionString == value)
					return;
				lock(SyncRoot) {
					if(DataLayer != null)
						throw new InvalidOperationException(Res.GetString(Res.XpoDefault_CannotAssignPropertyWhileAnotherIsNotNull, "ConnectionString", "DataLayer"));
					connectionString = value;
				}
			}
		}
		static bool defaultCaseSensitive = false;
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultDefaultCaseSensitive")]
#endif
public static bool DefaultCaseSensitive {
			get { return defaultCaseSensitive; }
			set { defaultCaseSensitive = value; }
		}
		static bool isObjectModifiedOnNonPersistentPropertyChange = true;
		public static bool IsObjectModifiedOnNonPersistentPropertyChange {
			get { return isObjectModifiedOnNonPersistentPropertyChange; }
			set { isObjectModifiedOnNonPersistentPropertyChange = value; }
		}
		public static bool TrackPropertiesModifications {
			get { return trackPropertiesModifications; }
			set { trackPropertiesModifications = value; }
		}
		static int defaultStringMappingFieldSize = SizeAttribute.DefaultStringMappingFieldSize;
		public static int DefaultStringMappingFieldSize {
			get { return defaultStringMappingFieldSize; }
			set { defaultStringMappingFieldSize = value; }
		}
		public static XPDictionary GetDictionary() {
			lock(SyncRoot) {
				if(Dictionary != null)
					return Dictionary;
				if(DataLayer != null)
					return DataLayer.Dictionary;
			}
			return new ReflectionDictionary();
		}
#if !SL
		public const string AppSettingsConnectionStringKey = "XpoDefaultConnectionString";
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultActiveConnectionString")]
#endif
public static string ActiveConnectionString {
			get {
				lock(SyncRoot) {
					if(ConnectionString != null)
						return ConnectionString;
				}
				string appsettingsConnectionString = System.Configuration.ConfigurationManager.AppSettings[AppSettingsConnectionStringKey];
				if(appsettingsConnectionString != null && appsettingsConnectionString.Length > 0)
					return appsettingsConnectionString;
				string mdbFile = System.IO.Path.ChangeExtension(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
					AppDomain.CurrentDomain.FriendlyName), "mdb");
				if(System.IO.File.Exists(mdbFile)) {
					return AccessConnectionProvider.GetConnectionString(mdbFile);
				}
				return DroneDataStore.GetConnectionString();
			}
		}
		[System.Runtime.InteropServices.DllImport("rpcrt4.dll", SetLastError = true)]
		static extern int UuidCreateSequential(out Guid guid);
		public static GuidGenerationMode GuidGenerationMode = GuidGenerationMode.FrameworkDefault;
		static bool uuidbroken = false;
		[SecuritySafeCritical]
		static Guid NewGuidSequential() {
			try {
				Guid newGuid;
				if(UuidCreateSequential(out newGuid) == 0) {
					return newGuid;
				}
			} catch { }
			uuidbroken = true;
			return Guid.NewGuid();
		}
#endif
		public static Guid NewGuid() {
#if !SL
			if(GuidGenerationMode != GuidGenerationMode.FrameworkDefault && !uuidbroken)
				return NewGuidSequential();
#endif
			return Guid.NewGuid();
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultOptimisticLockingReadBehavior")]
#endif
public static OptimisticLockingReadBehavior OptimisticLockingReadBehavior {
			get { return _OptimisticLockingReadBehavior; }
			set {
				if(value == OptimisticLockingReadBehavior.Default)
					value = DefaultOptimisticLockingReadBehavior;
				_OptimisticLockingReadBehavior = value;
			}
		}
		public static void ForcePerformanceCountersCreation() {
#if !CF && !SL
			PerformanceCounters.Init();
#endif
		}
		[Obsolete("Use IdentityMapBehavior instead")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static CacheBehavior CacheBehavior {
			get { return (CacheBehavior)(int)IdentityMapBehavior; }
			set {
				IdentityMapBehavior = (IdentityMapBehavior)(int)value;
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultIdentityMapBehavior")]
#endif
public static IdentityMapBehavior IdentityMapBehavior {
			get { return _IdentityMapBehavior; }
			set {
				if (value == IdentityMapBehavior.Default)
					value = DefaultIdentityMapBehavior;
				_IdentityMapBehavior = value;
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XpoDefaultUseFastAccessors")]
#endif
public static bool UseFastAccessors {
			get { return useFastAccessors; }
			set { useFastAccessors = value; }
		}
		public static IDataStore GetConnectionProvider(AutoCreateOption defaultAutoCreateOption) {
			return GetConnectionProvider((string)null, defaultAutoCreateOption);
		}
		public static IDataStore GetConnectionProvider(string connectionString, AutoCreateOption defaultAutoCreateOption) {
			IDisposable[] disposeOnDisconnectObjects;
			return GetConnectionProvider(connectionString, defaultAutoCreateOption, out disposeOnDisconnectObjects);
		}
#if !SL
		static void GetPoolParameters(ConnectionStringParser helper, ref string plainConnectionString, out bool? pool, out int? poolSize, out int? poolMaxConnections) {
			pool = null;
			poolSize = null;
			poolMaxConnections = null;
			string poolString = helper.GetPartByName(DataStorePool.XpoPoolParameterName);
			string poolSizeString = helper.GetPartByName(DataStorePool.XpoPoolSizeParameterName);
			string poolMaxConnectionsString = helper.GetPartByName(DataStorePool.XpoPoolMaxConnectionsParameterName);
			if(!string.IsNullOrEmpty(poolString)) {
				pool = (string.Equals(poolString, "false", StringComparison.InvariantCultureIgnoreCase)
					|| string.Equals(poolString, "disable", StringComparison.InvariantCultureIgnoreCase)
					|| string.Equals(poolString, "disabled", StringComparison.InvariantCultureIgnoreCase) || poolString == "0") ? false : true;
			}
			if(!string.IsNullOrEmpty(poolSizeString)) {
#if !CF
				int poolSizeInt;
				if(int.TryParse(poolSizeString, out poolSizeInt)) {
					poolSize = poolSizeInt;
				}
#else
				try {
					poolSize = int.Parse(poolSizeString);
				} catch { }
#endif
			}
			if(!string.IsNullOrEmpty(poolMaxConnectionsString)) {
#if !CF
				int poolMaxConnectionsInt;
				if(int.TryParse(poolMaxConnectionsString, out poolMaxConnectionsInt)) {
					poolMaxConnections = poolMaxConnectionsInt;
				}
#else
				try {
					poolMaxConnections = int.Parse(poolMaxConnectionsString);
				} catch { }
#endif
			}
			if(helper.PartExists(DataStorePool.XpoPoolParameterName) 
				|| helper.PartExists(DataStorePool.XpoPoolSizeParameterName) 
				|| helper.PartExists(DataStorePool.XpoPoolMaxConnectionsParameterName)) {
				helper.RemovePartByName(DataStorePool.XpoPoolParameterName);
				helper.RemovePartByName(DataStorePool.XpoPoolSizeParameterName);
				helper.RemovePartByName(DataStorePool.XpoPoolMaxConnectionsParameterName);
				plainConnectionString = helper.GetConnectionString();
			}
		}
#endif
		public static IDataStore GetConnectionProvider(string connectionString, AutoCreateOption defaultAutoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
#if SL
			if (connectionString.IndexOf("://") >= 1 && connectionString.IndexOf("://") <= 10 && connectionString.EndsWith(".svc")) {
					objectsToDisposeOnDisconnect = new IDisposable[0];
					return CreateWCFWebServiceStore(connectionString);
			}				
			throw new CannotFindAppropriateConnectionProviderException(connectionString);
		}
#else
			if(connectionString == null || connectionString.Length == 0)
				connectionString = ActiveConnectionString;
			ConnectionStringParser helper = new ConnectionStringParser(connectionString);
			string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
			if(providerType != null && providerType.Length == 0)
				providerType = null;
			string plainConnectionString = string.Empty;
			bool? pool;
			int? poolSize;
			int? poolMaxConnections;
			GetPoolParameters(helper, ref plainConnectionString, out pool, out poolSize, out poolMaxConnections);
			if((pool.HasValue && pool.Value) || (!pool.HasValue && (poolSize.HasValue || poolMaxConnections.HasValue))) {
				DataStorePool dataStorePool = new DataStorePool(defaultAutoCreateOption, plainConnectionString, poolSize, poolMaxConnections);
				objectsToDisposeOnDisconnect = new IDisposable[] { dataStorePool };
				return dataStorePool;
			}
			if(string.IsNullOrEmpty(plainConnectionString))
				plainConnectionString = connectionString;
			if(providerType != null) {
				helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
				plainConnectionString = helper.GetConnectionString();
			} else {
				if(0 != helper.GetPartByName("Initial Catalog").Length && 0 == helper.GetPartByName("Provider").Length) {
					providerType = MSSqlConnectionProvider.XpoProviderTypeString;
				} else {
#if !CF
					string provider = helper.GetPartByName("Provider").ToLower(CultureInfo.InvariantCulture);
					if(provider.IndexOf("microsoft.ace.oledb") >= 0 || provider.IndexOf("microsoft.jet.oledb") >= 0) {
						providerType = AccessConnectionProvider.XpoProviderTypeString;
					} else if(connectionString.IndexOf("://") >= 1 && connectionString.IndexOf("://") <= 10) {
						if(connectionString.EndsWith(".asmx")) {
							objectsToDisposeOnDisconnect = new IDisposable[0];
							return CreateWebServiceStore(connectionString);
						} else if(connectionString.StartsWith("net.tcp")) {
							objectsToDisposeOnDisconnect = new IDisposable[0];
							return CreateWCFTcpServiceStore(connectionString);
						} else if(connectionString.EndsWith(".svc")) {
							objectsToDisposeOnDisconnect = new IDisposable[0];
							return CreateWCFWebServiceStore(connectionString);
						} else {
							objectsToDisposeOnDisconnect = new IDisposable[0];
							return ActivateRemoteDataStore(connectionString);
						}
					} else
#else
					if(0 != helper.GetPartByName("Data Source").Length) {
						providerType = MSSqlCEConnectionProvider.XpoProviderTypeString;
					} else
#endif
						throw new CannotFindAppropriateConnectionProviderException(ConnectionStringRemovePassword(helper));
				}
			}
			IDataStore result = DataStoreBase.QueryDataStore(providerType, plainConnectionString, defaultAutoCreateOption, out objectsToDisposeOnDisconnect);
			if(result == null)
				throw new CannotFindAppropriateConnectionProviderException(ConnectionStringRemovePassword(helper));
			return result;
		}
		internal static string ConnectionStringRemovePassword(string connectionString) {
			return ConnectionStringRemovePassword(new ConnectionStringParser(connectionString));
		}
		internal static string ConnectionStringRemovePassword(ConnectionStringParser helper) {
			string removedString = "***REMOVED***";
			helper.UpdatePartByName("password", removedString);
			helper.UpdatePartByName("pwd", removedString);
			return helper.GetConnectionString();
		}
		static IDataStore CreateWebServiceStore(string connectionString) {
			return new WebServiceDataStore(connectionString);
		}
#endif
#if !CF
		static IDataStore CreateWCFWebServiceStore(string connectionString) {
			EndpointAddress address = new EndpointAddress(connectionString);
			BasicHttpBinding binding = new BasicHttpBinding();
			binding.MaxReceivedMessageSize = Int32.MaxValue;
#if !SL
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
#endif
			return CreateWcfServiceStore(connectionString, address, binding);
		}
#if !SL
		static IDataStore CreateWCFTcpServiceStore(string connectionString) {
			EndpointAddress address = new EndpointAddress(connectionString);			
			NetTcpBinding binding = new NetTcpBinding();
			binding.MaxReceivedMessageSize = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			return CreateWcfServiceStore(connectionString, address, binding);
		}
#endif
		static IDataStore CreateWcfServiceStore(string connectionString, EndpointAddress address, System.ServiceModel.Channels.Binding binding) {
			try {
				try {
					DataCacheNode node = new DataCacheNode(new CachedDataStoreClient(binding, address));
					node.ProcessCookie(DataCacheCookie.Empty);
					return node;
				} catch { }
				IDataStore store = new DataStoreClient(binding, address);
				store.AutoCreateOption.ToString();
				return store;
			} catch(Exception e) {
				throw new DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException(connectionString, e);
			}
		}
#endif
#if !CF && !SL
		[SecuritySafeCritical]
		static IDataStore ActivateRemoteDataStore(string url) {
			try {
				try {
					ICacheToCacheCommunicationCore parentCache = (ICacheToCacheCommunicationCore)Activator.GetObject(typeof(ICacheToCacheCommunicationCore), url);
					if(parentCache != null && typeof(ICacheToCacheCommunicationCore).IsAssignableFrom(parentCache.GetType())) {
						DataCacheNode node = new DataCacheNode(parentCache);
						node.ProcessCookie(DataCacheCookie.Empty);
						return node;
					}
				} catch {
				}
				IDataStore store = (IDataStore)Activator.GetObject(typeof(IDataStore), url);
				store.AutoCreateOption.ToString();	
				return store;
			} catch(Exception e) {
				throw new DevExpress.Xpo.DB.Exceptions.UnableToOpenDatabaseException(url, e);
			}
		}
#endif
#if !SL
		private static string ClearPoolConnectionString(string connectionString) {
			ConnectionStringParser helper = new ConnectionStringParser(connectionString);
			if(helper.PartExists(DataStorePool.XpoPoolParameterName)
				|| helper.PartExists(DataStorePool.XpoPoolSizeParameterName)
				|| helper.PartExists(DataStorePool.XpoPoolMaxConnectionsParameterName)) {
				helper.RemovePartByName(DataStorePool.XpoPoolParameterName);
				helper.RemovePartByName(DataStorePool.XpoPoolSizeParameterName);
				helper.RemovePartByName(DataStorePool.XpoPoolMaxConnectionsParameterName);
				return helper.GetConnectionString();
			}
			return connectionString;
		}
		public static string GetConnectionPoolString(string connectionString) {
			return string.Format("{0};{1}=True;", ClearPoolConnectionString(connectionString), DataStorePool.XpoPoolParameterName);
		}
		public static string GetConnectionPoolString(string connectionString, int poolSize) {
			return string.Format("{0};{1}={2};", ClearPoolConnectionString(connectionString), DataStorePool.XpoPoolSizeParameterName, poolSize);
		}
		public static string GetConnectionPoolString(string connectionString, int poolSize, int maxConnections) {
			return string.Format("{0};{1}={2};{3}={4};", ClearPoolConnectionString(connectionString), DataStorePool.XpoPoolSizeParameterName, poolSize, DataStorePool.XpoPoolMaxConnectionsParameterName, maxConnections);
		}
		public static IDataStore GetConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption) {
			if(connection == null)
				throw new ArgumentNullException("connection");
			IDataStore result = DataStoreBase.QueryDataStore(connection, autoCreateOption);
			if(result == null)
				throw new CannotFindAppropriateConnectionProviderException(connection.GetType().Name + "(" + connection.ConnectionString + ")");
			return result;
		}
#endif
		public static IDataLayer GetDataLayer(AutoCreateOption defaultAutoCreateOption) {
			return GetDataLayer((string)null, null, defaultAutoCreateOption);
		}
		public static IDataLayer GetDataLayer(XPDictionary dictionary, AutoCreateOption defaultAutoCreateOption) {
			return GetDataLayer((string)null, dictionary, defaultAutoCreateOption);
		}
		public static IDataLayer GetDataLayer(string connectionString, AutoCreateOption defaultAutoCreateOption) {
			return GetDataLayer(connectionString, null, defaultAutoCreateOption);
		}
		public static IDataLayer GetDataLayer(string connectionString, XPDictionary dictionary, AutoCreateOption defaultAutoCreateOption) {
			IDisposable[] disposeOnDisconnectObjects;
			IDataLayer layer = GetDataLayer(connectionString, dictionary, defaultAutoCreateOption, out disposeOnDisconnectObjects);
			return new DataLayerWrapperS18452(layer, disposeOnDisconnectObjects);
		}
		public static IDataLayer GetDataLayer(string connectionString, XPDictionary dictionary, AutoCreateOption defaultAutoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDisposable[] providersDisposable;
			IDataStore provider = GetConnectionProvider(connectionString, defaultAutoCreateOption, out providersDisposable);
			SimpleDataLayer rv = new SimpleDataLayer(dictionary, provider);
			objectsToDisposeOnDisconnect = new IDisposable[providersDisposable.Length + 1];
			providersDisposable.CopyTo(objectsToDisposeOnDisconnect, 0);
			objectsToDisposeOnDisconnect[providersDisposable.Length] = rv;
			return rv;
		}
#if !SL
		public static IDataLayer GetDataLayer(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return GetDataLayer(connection, null, autoCreateOption);
		}
		public static IDataLayer GetDataLayer(IDbConnection connection, XPDictionary dictionary, AutoCreateOption autoCreateOption) {
			IDisposable[] disposeOnDisconnectObjects;
			IDataLayer layer = GetDataLayer(connection, dictionary, autoCreateOption, out disposeOnDisconnectObjects);
			return new DataLayerWrapperS18452((IDataLayerForTests)layer, disposeOnDisconnectObjects);
		}
		public static IDataLayer GetDataLayer(IDbConnection connection, XPDictionary dictionary, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDataStore provider = GetConnectionProvider(connection, autoCreateOption);
			SimpleDataLayer rv = new SimpleDataLayer(dictionary, provider);
			objectsToDisposeOnDisconnect = new IDisposable[] { rv };
			return rv;
		}
		static XpoDefault() {
			RegisterDefaultProviders();
		}
		static void RegisterDefaultProviders() {
			MSSqlConnectionProvider.Register();
			InMemoryDataStore.Register();
			DataSetDataStore.Register();
			DroneDataStore.Register();
#if !CF
			AccessConnectionProvider.Register();
			AccessConnectionProviderMultiUserThreadSafe.Register();
			MSSql2005SqlDependencyCacheRoot.Register();
#else
			MSSqlCEConnectionProvider.Register();
#endif
			BonusProvidersRegistrator.Register();
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public static void RegisterBonusProviders() { }
#endif
		static int maxInSize = -1;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int MaxInSize {
			get {
				if(maxInSize < 0) maxInSize = GetTerminalInSize(Int32.MaxValue);
				return maxInSize;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int GetTerminalInSize(int size, int parametersPerObject) {
			if(parametersPerObject <= 1)
				return GetTerminalInSize(size);
			else
				return GetTerminalInSize(Math.Min(Math.Max(GetTerminalInSize(int.MaxValue) / parametersPerObject, 1), size));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static int GetTerminalInSize(int size) {
			if(size <= 16)
				return size;
			int num = terminalInSizes.Length - 1;
			while(size < terminalInSizes[num])
				num--;
			return terminalInSizes[num];
		}
		static readonly int[] terminalInSizes =
 { 16, 21, 34, 55
#if !CF
	 , 89, 144
	 , 233
#endif
 }
;
	}
}
