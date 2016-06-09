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
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.EntityClient;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.EF.Utils;
namespace DevExpress.ExpressApp.EF {
	public delegate ObjectContext CreateObjectContextHandler(IList<IDisposable> disposableObjects);
	public delegate void UpdateSchemaHandler();
	public class EFObjectSpaceProvider : IObjectSpaceProvider, IDatabaseSchemaChecker, IDisposable {
		private Type contextType;
		private DbConnection connection;
		private String connectionString;
		private String metadataLocations;
		private String providerName;
		protected ITypesInfo typesInfo;
		protected EFTypeInfoSource efTypeInfoSource;
		private MetadataWorkspace metadataWorkspace;
		private Type moduleInfoType;
		private SchemaUpdateMode schemaUpdateMode = SchemaUpdateMode.DatabaseAndSchema;
		private CheckCompatibilityType? checkCompatibilityType = null;
		private DatabaseSchemaState? schemaState = null;
		protected CreateObjectContextHandler createObjectContextDelegate;
		protected UpdateSchemaHandler updateSchemaDelegate;
		private String RemovePassword(String connectionString) {
			String result = connectionString;
			if(!String.IsNullOrEmpty(connectionString)) {
				ConnectionStringParser parser = new ConnectionStringParser(connectionString);
				parser.RemovePartByName("Password");
				result = parser.GetConnectionString();
			}
			return result;
		}
		private void Init(Type contextType, ITypesInfo typesInfo, EFTypeInfoSource efTypeInfoSource, DbConnection connection, String connectionString, String metadataLocations, String providerName) {
			if(contextType == null) {
				throw new ArgumentException(@"The ""contextType"" parameter must not be null.");
			}
			if(!typeof(DbContext).IsAssignableFrom(contextType) && !typeof(ObjectContext).IsAssignableFrom(contextType)) {
				String messageText = String.Format(@"The ""{0}"" must be inherited from the ""{1}"" or the ""{2}"" class.",
					contextType.Name, typeof(DbContext).FullName, typeof(ObjectContext).FullName);
				throw new Exception(messageText);
			}
			this.contextType = contextType;
			this.typesInfo = typesInfo;
			this.efTypeInfoSource = efTypeInfoSource;
			this.connection = connection;
			this.connectionString = connectionString;
			this.metadataLocations = metadataLocations;
			this.providerName = providerName;
			createObjectContextDelegate = (IList<IDisposable> disposableObjects) => { return CreateObjectContext(disposableObjects); };
			updateSchemaDelegate = () => { UpdateSchema(); };
			lock(DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
				if(this.efTypeInfoSource == null) {
					this.efTypeInfoSource = FindTypeInfoSource(contextType);
				}
				if(this.efTypeInfoSource == null) {
					IList<IDisposable> disposableObjects = new List<IDisposable>();
					ObjectContext objectContext = CreateObjectContext(disposableObjects);
					MetadataWorkspace metadataWorkspace = objectContext.MetadataWorkspace;
					objectContext.Dispose();
					foreach(IDisposable disposable in disposableObjects) {
						disposable.Dispose();
					}
					this.efTypeInfoSource = new EFTypeInfoSource(typesInfo, contextType, metadataWorkspace);
					((TypesInfo)typesInfo).AddEntityStore(this.efTypeInfoSource);
				}
			}
			ParseCriteriaScope.Init(typesInfo);
		}
		private EFTypeInfoSource FindTypeInfoSource(Type contextType) {
			EFTypeInfoSource result = null;
			foreach(IEntityStore entityStore in ((TypesInfo)typesInfo).EntityStores) {
				if((entityStore is EFTypeInfoSource) && (((EFTypeInfoSource)entityStore).ContextType == contextType)) {
					result = (EFTypeInfoSource)entityStore;
					break;
				}
			}
			return result;
		}
		protected virtual Type FindModuleInfoType() {
			ITypeInfo objectContextTypeInfo = typesInfo.FindTypeInfo(contextType);
			foreach(IMemberInfo objectContextMemberInfo in objectContextTypeInfo.Members) {
				if(objectContextMemberInfo.MemberType.IsGenericType
						&&
						(
							(objectContextMemberInfo.MemberType.GetGenericTypeDefinition() == typeof(DbSet<>))
							||
							(objectContextMemberInfo.MemberType.GetGenericTypeDefinition() == typeof(ObjectSet<>))
						)
						&& typeof(IModuleInfo).IsAssignableFrom(objectContextMemberInfo.MemberType.GetGenericArguments()[0])) {
					return objectContextMemberInfo.MemberType.GetGenericArguments()[0];
				}
			}
			return null;
		}
		protected virtual ObjectContext CreateObjectContext(IList<IDisposable> disposableObjects) {
			Object context = null;
			if(connection != null) {
				if(String.IsNullOrWhiteSpace(metadataLocations)) {
					context = Activator.CreateInstance(contextType, connection);
				}
				else {
					if(metadataWorkspace == null) {
						metadataWorkspace = new MetadataWorkspace(metadataLocations.Split('|'), new Assembly[] { contextType.Assembly });
					}
					EntityConnection entityConnection = new EntityConnection(metadataWorkspace, connection);
					context = Activator.CreateInstance(contextType, entityConnection);
					disposableObjects.Add(entityConnection);
				}
			}
			else if(!String.IsNullOrEmpty(connectionString)) {
				if(String.IsNullOrWhiteSpace(metadataLocations)) {
					context = Activator.CreateInstance(contextType, connectionString);
				}
				else {
					EntityConnectionStringBuilder efConnectionStringBuilder = new EntityConnectionStringBuilder();
					efConnectionStringBuilder.ProviderConnectionString = connectionString;
					efConnectionStringBuilder.Metadata = metadataLocations;
					efConnectionStringBuilder.Provider = providerName;
					context = Activator.CreateInstance(contextType, efConnectionStringBuilder.ConnectionString);
				}
			}
			if(!schemaState.HasValue && context != null) {
				schemaState = CheckDatabaseSchemaCompatibilityCore(context);
			}
			ObjectContext result = null;
			if(context is ObjectContext) {
				result = (ObjectContext)context;
			}
			else if(context is DbContext) {
				result = ((IObjectContextAdapter)context).ObjectContext;
				disposableObjects.Add((DbContext)context);
			}
			return result;
		}
		protected virtual void OnSchemaUpdating(HandledEventArgs args) {
			if(SchemaUpdating != null) {
				SchemaUpdating(this, args);
			}
		}
		public EFObjectSpaceProvider(Type contextType, ITypesInfo typesInfo, EFTypeInfoSource efTypeInfoSource, DbConnection connection, String metadataLocations, String providerName) {
			if(connection == null) {
				throw new ArgumentException(@"The ""connection"" parameter must not be null.");
			}
			Init(contextType, typesInfo, efTypeInfoSource, connection, "", metadataLocations, providerName);
		}
		public EFObjectSpaceProvider(Type contextType, ITypesInfo typesInfo, EFTypeInfoSource efTypeInfoSource, String connectionString, String metadataLocations, String providerName) {
			if(String.IsNullOrEmpty(connectionString)) {
				throw new ArgumentException(@"The ""connectionString"" parameter must not be null.");
			}
			Init(contextType, typesInfo, efTypeInfoSource, null, connectionString, metadataLocations, providerName);
		}
		public EFObjectSpaceProvider(Type contextType, ITypesInfo typesInfo, EFTypeInfoSource efTypeInfoSource, DbConnection connection)
			: this(contextType, typesInfo, efTypeInfoSource, connection, "", "") {
		}
		public EFObjectSpaceProvider(Type contextType, ITypesInfo typesInfo, EFTypeInfoSource efTypeInfoSource, String connectionString)
			: this(contextType, typesInfo, efTypeInfoSource, connectionString, "", "") {
		}
		public EFObjectSpaceProvider(Type contextType, DbConnection connection)
			: this(contextType, XafTypesInfo.Instance, null, connection) {
		}
		public EFObjectSpaceProvider(Type contextType, String connectionString)
			: this(contextType, XafTypesInfo.Instance, null, connectionString) {
		}
		public virtual void Dispose() {
			connection = null;
			efTypeInfoSource = null;
			SchemaUpdating = null;
		}
		public virtual IObjectSpace CreateObjectSpace() {
			return new EFObjectSpace(typesInfo, efTypeInfoSource, createObjectContextDelegate);
		}
		public virtual IObjectSpace CreateUpdatingObjectSpace(Boolean allowUpdateSchema) {
			return new EFObjectSpace(typesInfo, efTypeInfoSource, createObjectContextDelegate, allowUpdateSchema ? updateSchemaDelegate : null);
		}
		public void UpdateSchema() {
			HandledEventArgs args = new HandledEventArgs(false);
			OnSchemaUpdating(args);
			if(!args.Handled) {
				List<IDisposable> disposableObjects = new List<IDisposable>();
				using(ObjectContext objectContext = CreateObjectContext(disposableObjects)) {
					if(!objectContext.DatabaseExists()) {
						objectContext.CreateDatabase();
					}
					schemaState = CheckDatabaseSchemaCompatibilityCore(objectContext);
					if(disposableObjects != null) {
						foreach(IDisposable disposableObject in disposableObjects) {
							disposableObject.Dispose();
						}
					}
				}
			}
		}
		public void DeleteDatabase() {
			List<IDisposable> disposableObjects = new List<IDisposable>();
			using(ObjectContext objectContext = CreateObjectContext(disposableObjects)) {
				if(objectContext.DatabaseExists()) {
					objectContext.DeleteDatabase();
				}
				schemaState = CheckDatabaseSchemaCompatibilityCore(objectContext);
				if(disposableObjects != null) {
					foreach(IDisposable disposableObject in disposableObjects) {
						disposableObject.Dispose();
					}
				}
			}
		}
		public String ConnectionString {
			get {
				String result = "";
				if(connectionString.Contains("res://*/")) {
					EntityConnectionStringBuilder efConnectionStringBuilder = new EntityConnectionStringBuilder(connectionString);
					efConnectionStringBuilder.ProviderConnectionString = RemovePassword(efConnectionStringBuilder.ProviderConnectionString);
					result = efConnectionStringBuilder.ConnectionString;
				}
				else if(connection != null) {
					result = RemovePassword(connection.ConnectionString);
				}
				else {
					result = RemovePassword(connectionString);
				}
				return result;
			}
			set { connectionString = value; }
		}
		public DbConnection Connection {
			get { return connection; }
		}
		public ITypesInfo TypesInfo {
			get { return typesInfo; }
		}
		public IEntityStore EntityStore {
			get { return efTypeInfoSource; }
		}
		public Type ModuleInfoType {
			get {
				if(moduleInfoType == null) {
					moduleInfoType = FindModuleInfoType();
				}
				return moduleInfoType;
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
		public event HandledEventHandler SchemaUpdating;
		#region IDatabaseSchemaChecker
		protected virtual DatabaseSchemaState CheckDatabaseSchemaCompatibilityCore(Object context) {
			DatabaseSchemaState result = DatabaseSchemaState.SchemaExists;
			if(context is DbContext) {
				Database database = ((DbContext)context).Database;
				if(!database.Exists()) {
					result = DatabaseSchemaState.DatabaseMissing;
				}
				else {
					if(!database.CompatibleWithModel(false)) { 
						result = DatabaseSchemaState.SchemaRequiresUpdate;
					}
				}
			}
			else if(context is ObjectContext) {
				if(!((ObjectContext)context).DatabaseExists()) {
					result = DatabaseSchemaState.DatabaseMissing;
				}
			}
			return result;
		}
		public DatabaseSchemaState CheckDatabaseSchemaCompatibility(out Exception exception) {
			exception = null;
			DatabaseSchemaState result = DatabaseSchemaState.SchemaExists;
			List<IDisposable> disposableObjects = new List<IDisposable>();
			if(schemaState.HasValue) {
				result = schemaState.Value;
			}
			else {
				using(ObjectContext objectContext = CreateObjectContext(disposableObjects)) {
					if(schemaState.HasValue) {
						result = schemaState.Value;
					}
					if(disposableObjects != null) {
						foreach(IDisposable disposableObject in disposableObjects) {
							disposableObject.Dispose();
						}
					}
				}
			}
			return result;
		}
		#endregion
	}
}
