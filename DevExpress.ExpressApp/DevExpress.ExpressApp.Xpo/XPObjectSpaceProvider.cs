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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo.Updating;
using DevExpress.ExpressApp.Xpo.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
namespace DevExpress.ExpressApp.Xpo {
	public interface IXpoDataStoreProvider {
		IDataStore CreateWorkingStore(out IDisposable[] disposableObjects);
		IDataStore CreateUpdatingStore(Boolean allowUpdateSchema, out IDisposable[] disposableObjects);
		IDataStore CreateSchemaCheckingStore(out IDisposable[] disposableObjects);
		string ConnectionString { get; }
	}
	public class ConnectionDataStoreProvider : IXpoDataStoreProvider {
		private IDbConnection connection;
		private IDataStore workingStore;
		public ConnectionDataStoreProvider(IDbConnection connection) {
			Guard.ArgumentNotNull(connection, "connection");
			this.connection = connection;
		}
		public IDataStore CreateWorkingStore(out IDisposable[] disposableObjects) {
			disposableObjects = null;
			if(workingStore == null) {
				workingStore = XpoDefault.GetConnectionProvider(connection, AutoCreateOption.SchemaAlreadyExists);
			}
			return workingStore;
		}
		public IDataStore CreateUpdatingStore(Boolean allowUpdateSchema, out IDisposable[] disposableObjects) {
			disposableObjects = null;
			AutoCreateOption createOption = allowUpdateSchema ? AutoCreateOption.DatabaseAndSchema : AutoCreateOption.SchemaAlreadyExists;
			return XpoDefault.GetConnectionProvider(connection, createOption);
		}
		public IDataStore CreateSchemaCheckingStore(out IDisposable[] disposableObjects) {
			disposableObjects = null;
			return XpoDefault.GetConnectionProvider(connection, AutoCreateOption.None);
		}
		public string ConnectionString {
			get { return connection.ConnectionString; }
		}
	}
	public class ConnectionStringDataStoreProvider : IXpoDataStoreProvider, IDisposable {
		private static Dictionary<String, IDataStore> xmlFileStoreMap = new Dictionary<String, IDataStore>();
		private IDisposable[] dataStoreDisposableObjects = null;
		private String connectionString;
		private IDataStore xmlFileStore;
		private IDataStore workingStore;
		private Boolean useCachedDataStore = false;
		public ConnectionStringDataStoreProvider(string connectionString, Boolean useCachedDataStore)
			: this(connectionString) {
				this.useCachedDataStore = useCachedDataStore;
		}
		public ConnectionStringDataStoreProvider(string connectionString) {
			Guard.ArgumentNotNullOrEmpty(connectionString, "connectionString");
			this.connectionString = connectionString;
			ConnectionStringParser parser = new ConnectionStringParser(connectionString);
			Boolean isXmlFileBased = parser.GetPartByName(DataStoreBase.XpoProviderTypeParameterName) == DataSetDataStore.XpoProviderTypeString;
			if(isXmlFileBased) {
				String xmlFileName = parser.GetPartByName("Data Source");
				if(xmlFileStoreMap.ContainsKey(xmlFileName)) {
					xmlFileStore = xmlFileStoreMap[xmlFileName];
				}
				else {
					xmlFileStore = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
					xmlFileStoreMap[xmlFileName] = xmlFileStore;
				}
			}
		}
		public IDataStore CreateWorkingStore(out IDisposable[] disposableObjects) {
			disposableObjects = null;
			if(xmlFileStore != null) {
				return xmlFileStore;
			}
			if(!useCachedDataStore) {
				return XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists, out disposableObjects);
			}
			if(workingStore == null) {
				workingStore = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists, out dataStoreDisposableObjects);
			}
			return workingStore;
		}
		public IDataStore CreateUpdatingStore(Boolean allowUpdateSchema, out IDisposable[] disposableObjects) {
			disposableObjects = null;
			if(xmlFileStore != null) {
				return xmlFileStore;
			}
			else {
				AutoCreateOption createOption = allowUpdateSchema ? AutoCreateOption.DatabaseAndSchema : AutoCreateOption.SchemaAlreadyExists;
				return XpoDefault.GetConnectionProvider(connectionString, createOption, out disposableObjects);
			}
		}
		public IDataStore CreateSchemaCheckingStore(out IDisposable[] disposableObjects) {
			disposableObjects = null;
			if(xmlFileStore != null) {
				return xmlFileStore;
			}
			return XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.None, out disposableObjects);
		}
		public void Dispose() {
			foreach(IDisposable disposableObject in dataStoreDisposableObjects) {
				disposableObject.Dispose();
			}
			dataStoreDisposableObjects = null;
		}
		public string ConnectionString {
			get { return connectionString; }
		}
	}
	public class MemoryDataStoreProvider : IXpoDataStoreProvider {
		private DataSet dataSet;
		public MemoryDataStoreProvider() : this(new DataSet()) { }
		public MemoryDataStoreProvider(DataSet dataSet) {
			this.dataSet = dataSet;
		}
		public IDataStore CreateWorkingStore(out IDisposable[] disposableObjects) {
			disposableObjects = null;
			return new DataSetDataStore(dataSet, AutoCreateOption.DatabaseAndSchema);
		}
		public IDataStore CreateUpdatingStore(Boolean allowUpdateSchema, out IDisposable[] disposableObjects) {
			disposableObjects = null;
			return new DataSetDataStore(dataSet, AutoCreateOption.DatabaseAndSchema);
		}
		public IDataStore CreateSchemaCheckingStore(out IDisposable[] disposableObjects) {
			disposableObjects = null;
			return new DataSetDataStore(dataSet, AutoCreateOption.None);
		}
		public string ConnectionString {
			get { return null; }
		}
	}
	public class DummyDataStoreProvider : IXpoDataStoreProvider {
		private IDataStore workingStore;
		private IDataStore updatingStore;
		public DummyDataStoreProvider(IDataStore dataStore, IDataStore updatingDataStore, IDataStore updatingReadOnlyDataStore) {
			Guard.ArgumentNotNull(dataStore, "dataStore");
			this.workingStore = dataStore;
			this.updatingStore = updatingDataStore;
		}
		public IDataStore CreateWorkingStore(out IDisposable[] disposableObjects) {
			disposableObjects = null;
			return workingStore;
		}
		public IDataStore CreateUpdatingStore(Boolean allowUpdateSchema, out IDisposable[] disposableObjects) {
			disposableObjects = null;
			return allowUpdateSchema ? updatingStore : workingStore;
		}
		public IDataStore CreateSchemaCheckingStore(out IDisposable[] disposableObjects) {
			disposableObjects = null;
			return updatingStore;
		}
		public string ConnectionString {
			get { return null; }
		}
	}
	public class InMemoryDataStoreProvider {
		public const string XpoProviderTypeString = "InMemoryDataStoreProvider";
		public const string XpoConnectionString = "XpoProvider=InMemoryDataStoreProvider";
		public static string ConnectionString { get { return XpoConnectionString; } }
		private static IValueManager<InMemoryDataStore> valueManager;
		private static bool isInitialized;
		private static object locker = new object();
		static InMemoryDataStoreProvider() {
			valueManager = ValueManager.GetValueManager<InMemoryDataStore>("InMemoryDataStore_InMemoryDataStore");
			Register();
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {			
			objectsToDisposeOnDisconnect = new IDisposable[0];
			InMemoryDataStore current = valueManager.Value;
			if(current == null) {
				current = new InMemoryDataStore(AutoCreateOption.DatabaseAndSchema);
				valueManager.Value = current;
				Match match = Regex.Match(connectionString, "Data Source=(?<fileName>.*);", RegexOptions.IgnoreCase);
				if(match.Success) {
					current.ReadXml(match.Groups["fileName"].Value);
				}
			}
			return current;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Register() {
			lock(locker) {
				if(!isInitialized) {
					DataStoreBase.RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
					isInitialized = true;
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Register(IValueManager<InMemoryDataStore> valueManager) {
			Register();
			lock(locker) {
				InMemoryDataStoreProvider.valueManager = valueManager;
			}
		}
	}
	public class XPObjectSpaceProvider : IObjectSpaceProvider, ITableNameCustomizer, IDisposableExt, IDatabaseSchemaChecker {
#if DebugTest
		private static Object lockObject = new Object();
#endif
		private ITypesInfo typesInfo;
		private XpoTypeInfoSource xpoTypeInfoSource;
		private IXpoDataStoreProvider dataStoreProvider;
		private IDisposable[] dataStoreDisposableObjects;
		private IDataLayer dataLayer;
		private IDataStore asyncServerModeDataStoreProvider;
		private IDisposable[] asyncServerModeDataStoreDisposableObjects;
		private Session defaultSession;
		private Boolean isDisposed;
		private XPObjectSpace lastUpdatingObjectSpaceForUpdateSchema;
		private Type moduleInfoType;
		private SchemaUpdateMode schemaUpdateMode = SchemaUpdateMode.DatabaseAndSchema;
		private CheckCompatibilityType? checkCompatibilityType = null;
		protected Boolean threadSafe;
		private void lastUpdatingObjectSpaceForUpdateSchema_Disposed(object sender, EventArgs e) {
			ReleaseLastUpdatingObjectSpaceForUpdateSchema();
		}
		private void ReleaseLastUpdatingObjectSpaceForUpdateSchema() {
			if(lastUpdatingObjectSpaceForUpdateSchema != null) {
				lastUpdatingObjectSpaceForUpdateSchema.Disposed -= new EventHandler(lastUpdatingObjectSpaceForUpdateSchema_Disposed);
				lastUpdatingObjectSpaceForUpdateSchema = null;
			}
		}
		private IXpoDataStoreProvider GetDataStoreProvider(String connectionString, IDbConnection connection) {
			IXpoDataStoreProvider dataStoreProvider;
			if(!String.IsNullOrEmpty(connectionString)) {
				dataStoreProvider = new ConnectionStringDataStoreProvider(connectionString);
			}
			else if(connection != null) {
				dataStoreProvider = new ConnectionDataStoreProvider(connection);
			}
			else {
				throw new ArgumentNullException("connectionString & connection");
			}
			return dataStoreProvider;
		}
		private void Initialize(IXpoDataStoreProvider dataStoreProvider, ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			Guard.ArgumentNotNull(xpoTypeInfoSource, "xpoTypeInfoSource");
			XpoDefault.DefaultCaseSensitive = false;
			XpoDefault.IdentityMapBehavior = IdentityMapBehavior.Strong;
			this.typesInfo = typesInfo;
			this.xpoTypeInfoSource = xpoTypeInfoSource;
			this.dataStoreProvider = dataStoreProvider;
			if(Session.DefaultSession != null) {
				defaultSession = Session.DefaultSession;
				defaultSession.BeforeConnect += new SessionManipulationEventHandler(DefaultSession_BeforeConnect);
			}
#if DebugTest
			lock(lockObject) {
				DevExpress.ExpressApp.Tests.BaseXafTest.DisposableObjects.Add(this);  
			}
#endif
		}
		private void DefaultSession_BeforeConnect(Object sender, SessionManipulationEventArgs e) {
			throw new Exception(
				"This error occurs because you tried to use the Session.DefaultSession " +
				"(please refer to XPO documentation to learn more) in XAF. " +
				"It is not allowed by default. Instead, use the XafApplication.ConnectionString property " +
				"to configure the connection to the database according to your needs. " +
				"Most likely, this problem is caused by the fact that you instantiated your persistent object, " +
				"XPCollection, etc. without the Session parameter and the default session was used by default. " +
				"Use a non-default session object to instantiate your objects and collections.");
		}
		private String RemovePassword(String connectionString) {
			if(String.IsNullOrEmpty(connectionString)) {
				return connectionString;
			}
			else {
				ConnectionStringParser parser = new ConnectionStringParser(connectionString);
				parser.RemovePartByName("Password");
				parser.RemovePartByName("Jet OLEDB:Database Password");
				return parser.GetConnectionString();
			}
		}
		private UnitOfWork CreateUnitOfWork(IDataStore dataStore, IDisposable[] disposableObjects) {
			List<IDisposable> disposableObjectsList = new List<IDisposable>();
			if(disposableObjects != null) {
				disposableObjectsList.AddRange(disposableObjects);
			}
			SimpleDataLayer dataLayer = new SimpleDataLayer(XPDictionary, dataStore);
			disposableObjectsList.Add(dataLayer);
			return new UnitOfWork(dataLayer, disposableObjectsList.ToArray());
		}
		private void ReleaseResources() {
			if(dataStoreDisposableObjects != null) {
				foreach(IDisposable disposableObject in dataStoreDisposableObjects) {
					disposableObject.Dispose();
				}
				dataStoreDisposableObjects = null;
			}
			if(dataLayer != null) {
				dataLayer.Dispose();
				dataLayer = null;
			}
		}
		private void AsyncServerModeSourceResolveSession(ResolveSessionEventArgs args) {
			if(asyncServerModeDataStoreProvider == null) {
				asyncServerModeDataStoreProvider = dataStoreProvider.CreateWorkingStore(out asyncServerModeDataStoreDisposableObjects);
			}
			args.Session = CreateUnitOfWork(asyncServerModeDataStoreProvider, null);
		}
		private void AsyncServerModeSourceDismissSession(ResolveSessionEventArgs args) {
			IDisposable toDispose = args.Session as IDisposable;
			if(toDispose != null) {
				toDispose.Dispose();
			}
		}
		private XPClassInfo[] GetPersistentClasses() {
			ICollection xpoClasses = XPDictionary.Classes;
			List<XPClassInfo> persistentClasses = new List<XPClassInfo>(xpoClasses.Count);
			foreach (XPClassInfo classInfo in xpoClasses) {
				if (classInfo.IsPersistent) {
					persistentClasses.Add(classInfo);
				}
			}
			return persistentClasses.ToArray();
		}
		protected virtual Type FindModuleInfoType() {
			Type result = null;
			if((xpoTypeInfoSource != null) && (xpoTypeInfoSource.EntityTypes != null)) {
				foreach(Type type in xpoTypeInfoSource.EntityTypes) {
					if(typeof(IModuleInfo).IsAssignableFrom(type)) {
						result = type;
						break;
					}
				}
			}
			return result;
		}
		protected virtual UnitOfWork CreateUpdatingUnitOfWork(Boolean allowUpdateSchema) {
			IDisposable[] disposableObjects;
			IDataStore dataStore = dataStoreProvider.CreateUpdatingStore(allowUpdateSchema, out disposableObjects);
			return CreateUnitOfWork(dataStore, disposableObjects);
		}
		protected virtual XPObjectSpace CreateUpdatingObjectSpaceCore(Boolean allowUpdateSchema) {
			return new XPObjectSpace(typesInfo, xpoTypeInfoSource, () => { return CreateUpdatingUnitOfWork(allowUpdateSchema); });
		}
		protected virtual IDataLayer CreateDataLayer(IDataStore dataStore) {
			if(threadSafe) {
				return new ThreadSafeDataLayer(XPDictionary, dataStore);
			}
			else {
				return new SimpleDataLayer(XPDictionary, dataStore);
			}
		}
		protected virtual UnitOfWork CreateUnitOfWork(IDataLayer dataLayer) {
			return new UnitOfWork(dataLayer);
		}
		protected virtual IObjectSpace CreateObjectSpaceCore() {
			return new XPObjectSpace(typesInfo, xpoTypeInfoSource, () => { return CreateUnitOfWork(dataLayer); });
		}
		public XPObjectSpaceProvider(IXpoDataStoreProvider dataStoreProvider, Boolean threadSafe) {
			this.threadSafe = threadSafe;
			ITypesInfo typesInfo = XpoTypesInfoHelper.GetTypesInfo();
			XpoTypeInfoSource xpoTypeInfoSource = XpoTypesInfoHelper.GetXpoTypeInfoSource();
			Initialize(dataStoreProvider, typesInfo, xpoTypeInfoSource);
		}
		public XPObjectSpaceProvider(String connectionString, IDbConnection connection, Boolean threadSafe) {
			this.threadSafe = threadSafe;
			ITypesInfo typesInfo = XpoTypesInfoHelper.GetTypesInfo();
			XpoTypeInfoSource xpoTypeInfoSource = XpoTypesInfoHelper.GetXpoTypeInfoSource();
			IXpoDataStoreProvider dataStoreProvider = GetDataStoreProvider(connectionString, connection);
			Initialize(dataStoreProvider, typesInfo, xpoTypeInfoSource);
		}
		public XPObjectSpaceProvider(IXpoDataStoreProvider dataStoreProvider, ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource, Boolean threadSafe) {
			this.threadSafe = threadSafe;
			Initialize(dataStoreProvider, typesInfo, xpoTypeInfoSource);
		}
		public XPObjectSpaceProvider(IXpoDataStoreProvider dataStoreProvider)
			: this(dataStoreProvider, false) {
		}
		public XPObjectSpaceProvider(String connectionString, IDbConnection connection)
			: this(connectionString, connection, false) {
		}
		public XPObjectSpaceProvider(IDbConnection connection)
			: this(null, connection) {
		}
		public XPObjectSpaceProvider(String connectionString)
			: this(connectionString, null) {
		}
		public XPObjectSpaceProvider(IXpoDataStoreProvider dataStoreProvider, ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource)
			: this(dataStoreProvider, typesInfo, xpoTypeInfoSource, false) {
		}
		static XPObjectSpaceProvider() {
			IsNewObjectCriteriaOperator.Register();
		}
		public virtual void Dispose() { 
#if DebugTest
			DevExpress.ExpressApp.Tests.BaseXafTest.DisposableObjects.Remove(this);
#endif
			if(defaultSession != null) {
				defaultSession.BeforeConnect -= new SessionManipulationEventHandler(DefaultSession_BeforeConnect);
				defaultSession = null;
			}
			if(asyncServerModeDataStoreDisposableObjects != null) {
				foreach(IDisposable disposableObject in asyncServerModeDataStoreDisposableObjects) {
					disposableObject.Dispose();
				}
				asyncServerModeDataStoreDisposableObjects = null;
			}
			asyncServerModeDataStoreProvider = null;
			ReleaseResources();
			xpoTypeInfoSource = null;
			typesInfo = null;
			ReleaseLastUpdatingObjectSpaceForUpdateSchema();
			isDisposed = true;
		}
		public IObjectSpace CreateObjectSpace() {
			Guard.NotDisposed(this);
			if(dataLayer == null) {
				IDataStore dataStore = dataStoreProvider.CreateWorkingStore(out dataStoreDisposableObjects);
				dataLayer = CreateDataLayer(dataStore);
			}
			IObjectSpace objectSpace = CreateObjectSpaceCore();
			if(objectSpace is XPObjectSpace) {
				((XPObjectSpace)objectSpace).AsyncServerModeSourceResolveSession = AsyncServerModeSourceResolveSession;
				((XPObjectSpace)objectSpace).AsyncServerModeSourceDismissSession = AsyncServerModeSourceDismissSession;
			}
			return objectSpace;
		}
		public IObjectSpace CreateUpdatingObjectSpace(Boolean allowUpdateSchema) {
			Guard.NotDisposed(this);
			XPObjectSpace result = CreateUpdatingObjectSpaceCore(allowUpdateSchema);
			if(allowUpdateSchema) {
				lastUpdatingObjectSpaceForUpdateSchema = result;
				lastUpdatingObjectSpaceForUpdateSchema.Disposed += new EventHandler(lastUpdatingObjectSpaceForUpdateSchema_Disposed);
			}
			return result;
		}
		public void SetDataStoreProvider(IXpoDataStoreProvider provider) {
			ReleaseResources();
			dataStoreProvider = provider;
		}
		protected IXpoDataStoreProvider DataStoreProvider {
			get { return dataStoreProvider; }
		}
		public String ConnectionString {
			get {
				if(dataLayer != null && dataLayer.Connection != null) {
					return dataLayer.Connection.ConnectionString;
				}
				return RemovePassword(dataStoreProvider.ConnectionString);
			}
			set {
				SetDataStoreProvider(new ConnectionStringDataStoreProvider(value));
			}
		}
		public ITypesInfo TypesInfo {
			get { return typesInfo; }
		}
		public XpoTypeInfoSource XpoTypeInfoSource {
			get { return xpoTypeInfoSource; }
		}
		public XPDictionary XPDictionary {
			get { return xpoTypeInfoSource.XPDictionary; }
		}
		public IDataLayer DataLayer {
			get { return dataLayer; }
		}
		public Boolean IsDisposed {
			get { return isDisposed; }
		}
		public Boolean ThreadSafe {
			get { return threadSafe; }
		}
		IObjectSpace IObjectSpaceProvider.CreateObjectSpace() {
			return CreateObjectSpace();
		}
		IObjectSpace IObjectSpaceProvider.CreateUpdatingObjectSpace(Boolean allowUpdateSchema) {
			return CreateUpdatingObjectSpace(allowUpdateSchema);
		}
		void IObjectSpaceProvider.UpdateSchema() {
			XPClassInfo[] persistentClasses = GetPersistentClasses();
			if(lastUpdatingObjectSpaceForUpdateSchema != null) {
				lastUpdatingObjectSpaceForUpdateSchema.Session.UpdateSchema(persistentClasses);
				lastUpdatingObjectSpaceForUpdateSchema.Session.UpdateSchema(typeof(XPObjectType));
			}
			else {
				using(UnitOfWork unitOfWork = CreateUpdatingUnitOfWork(true)) {
					unitOfWork.UpdateSchema(persistentClasses);
					unitOfWork.UpdateSchema(typeof(XPObjectType));
				}
			}
		}
		ITypesInfo IObjectSpaceProvider.TypesInfo {
			get { return TypesInfo; }
		}
		IEntityStore IObjectSpaceProvider.EntityStore {
			get { return xpoTypeInfoSource; }
		}
		String IObjectSpaceProvider.ConnectionString {
			get { return ConnectionString; }
			set { ConnectionString = value; }
		}
		public Type ModuleInfoType {
			get {
				if(moduleInfoType == null) {
					moduleInfoType = FindModuleInfoType();
				}
				if(moduleInfoType != null) {
					return moduleInfoType;
				}
				else {
					return typeof(ModuleInfo);
				}
			}
		}
		public SchemaUpdateMode SchemaUpdateMode {
			get { return schemaUpdateMode; }
			set { schemaUpdateMode = value; }
		}
		public CheckCompatibilityType? CheckCompatibilityType {
			get { return checkCompatibilityType; }
			set { checkCompatibilityType = value; }
		}
		void ITableNameCustomizer.Customize(String tablePrefixes) {
			TableNameCustomizer customizer = new TableNameCustomizer(tablePrefixes);
			customizer.CustomizeTableName += CustomizeTableName;
			customizer.Customize(xpoTypeInfoSource.XPDictionary);
		}
		public event EventHandler<CustomizeTableNameEventArgs> CustomizeTableName;
		DatabaseSchemaState IDatabaseSchemaChecker.CheckDatabaseSchemaCompatibility(out Exception exception) {
			DatabaseSchemaState result = DatabaseSchemaState.SchemaRequiresUpdate;
			exception = null;
			if(dataStoreProvider != null) {
				try {
					IDisposable[] disposableObjects;
					IDataStore dataStore = dataStoreProvider.CreateSchemaCheckingStore(out disposableObjects);
					if(dataStore != null) {
						using(UnitOfWork unitOfWork = CreateUnitOfWork(dataStore, disposableObjects)) {
							if(UpdateSchemaResult.SchemaExists == unitOfWork.UpdateSchema(true, GetPersistentClasses())) {
								result = DatabaseSchemaState.SchemaExists;
							}
						}
					}
				}
				catch(UnableToOpenDatabaseException e) {
					exception = e;
					result = DatabaseSchemaState.DatabaseMissing;
				}
				catch(SchemaCorrectionNeededException e) {
					exception = e;
					result = DatabaseSchemaState.SchemaRequiresUpdate;
				}
				catch(Exception e) {
					exception = e;
				}
			}
			return result;
		}
	}
}
