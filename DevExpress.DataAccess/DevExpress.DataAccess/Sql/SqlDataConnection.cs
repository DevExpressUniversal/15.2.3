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
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.Sql {
	public class SqlDataConnection : DataConnectionBase, INamedItem, IDisposable, IDataConnection {
		protected static void GetTablesAndViewsDBSchema(string[] tableList, IDataStoreSchemaExplorer schemaExplorer, List<DBTable> tables, List<DBTable> views) {
			IDataStoreSchemaExplorerEx schemaExplorerEx = schemaExplorer as IDataStoreSchemaExplorerEx;
			if(schemaExplorerEx != null) {
				tables.AddRange(schemaExplorerEx.GetStorageTables(true, tableList));
				views.AddRange(schemaExplorerEx.GetStorageViews(true, tableList));
			} else {
				DBTable[] allDataObjects = schemaExplorer.GetStorageTables(tableList);
				foreach(DBTable table in allDataObjects)
					if(!table.IsView)
						tables.Add(table);
					else
						views.Add(table);
			}
		}
		protected static DBStoredProcedure[] GetStoredProceduresSchema(IDataStoreSchemaExplorer schemaExplorer) {
			ISupportStoredProc schemaExplorerSp = schemaExplorer as ISupportStoredProc;
			DBStoredProcedure[] procedures = schemaExplorerSp != null ? schemaExplorerSp.GetStoredProcedures() : new DBStoredProcedure[0];
			return procedures;
		}
		protected static DBTable[] GetDBSchemaTables(IDataStoreSchemaExplorer schemaExplorer) {
			return GetDBSchemaTables(schemaExplorer, true);
		}
		protected static DBTable[] GetDBSchemaTables(IDataStoreSchemaExplorer schemaExplorer, bool loadColumns) {
			IDataStoreSchemaExplorerEx schemaExplorerEx = schemaExplorer as IDataStoreSchemaExplorerEx;
			if(schemaExplorerEx != null)
				return schemaExplorerEx.GetStorageTables(loadColumns);
			List<string> tableNames = new List<string>(schemaExplorer.GetStorageTablesList(false));
			return schemaExplorer.GetStorageTables(tableNames.ToArray());
		}
		protected static DBTable[] GetDBSchemaViews(IDataStoreSchemaExplorer schemaExplorer) {
			return GetDBSchemaViews(schemaExplorer, true);
		}
		protected static DBTable[] GetDBSchemaViews(IDataStoreSchemaExplorer schemaExplorer, bool loadColumns) {
			IDataStoreSchemaExplorerEx schemaExplorerEx = schemaExplorer as IDataStoreSchemaExplorerEx;
			if(schemaExplorerEx != null)
				return schemaExplorerEx.GetStorageViews(loadColumns);
			List<string> tableNames = new List<string>(schemaExplorer.GetStorageTablesList(false));
			List<string> viewNames = new List<string>(schemaExplorer.GetStorageTablesList(true));
			foreach(string table in tableNames)
				viewNames.Remove(table);
			return schemaExplorer.GetStorageTables(viewNames.ToArray());
		}
		ProviderFactory factory;
		DataConnectionParametersBase connectionParameters;
		readonly Dictionary<string, string> parameters = new Dictionary<string, string>();
		IDataStore dataStore;
		IDisposable[] objectsToDispose;
		string userId;
		string password;
		protected DBSchema dbSchema;
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataConnectionConnectionString")]
#endif
		public override string ConnectionString { get; set; }
		[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never)
		]
		public string ProviderKey { get { return this.factory.ProviderKey; } set { this.factory = DataConnectionHelper.GetProviderFactory(value); } }
		[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DefaultValue(null)
		]
		public string ParametersSerializable
		{
			get { return Base64XmlSerializer.GetBase64String(SaveParametersToXml()); }
			set
			{
				StoreConnectionNameOnly = false;
				XElement element = Base64XmlSerializer.GetXElement(value);
				if(element != null)
					LoadParametersFromXml(element);
			}
		}
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataConnectionConnectionParameters")]
#endif
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public DataConnectionParametersBase ConnectionParameters {
			get { return this.connectionParameters; }
			private set {
				this.connectionParameters = value;
				ApplyParameters(this.connectionParameters);
			}
		}
#if DXPORTABLE 
		public
#else
		internal
#endif
		IDataStoreSchemaExplorer SchemaExplorer { get { return (IDataStoreSchemaExplorer)DataStore; } }
		internal Dictionary<string, string> Parameters { get { return this.parameters; } }
		internal ISqlGeneratorFormatter SqlGeneratorFormatter { get { return DataStore as ISqlGeneratorFormatter; } }
		internal ICommandChannel CommandChannel { get { return (ICommandChannel)DataStore; } }
		internal IDisposable[] ObjectsToDispose { get { return this.objectsToDispose; } }
		internal ProviderFactory Factory { get { return this.factory; } }
		internal IDataStore DataStore { get { return this.dataStore; } }
#if DXPORTABLE 
		public
#else
		internal
#endif
		bool IsSqlDataStore { get { return this.dataStore is ISqlDataStore; } }
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataConnectionIsConnected")]
#endif
		public override bool IsConnected { get { return this.dataStore != null; } }
		Dictionary<string, string> ActualParameters
		{
			get
			{
				Dictionary<string, string> actualParameters = new Dictionary<string, string>(this.parameters);
				if(!actualParameters.ContainsKey(DataConnectionHelper.UserIdParam))
					actualParameters.Add(DataConnectionHelper.UserIdParam, this.userId ?? string.Empty);
				if(!actualParameters.ContainsKey(DataConnectionHelper.PasswordParam))
					actualParameters.Add(DataConnectionHelper.PasswordParam, this.password ?? string.Empty);
				return actualParameters;
			}
		}
		public SqlDataConnection() {
		}
		public SqlDataConnection(string name, DataConnectionParametersBase connectionParameters)
			: this(name) {
			Guard.ArgumentNotNull(connectionParameters, "connectionParameters");
			ConnectionParameters = connectionParameters;
		}
		SqlDataConnection(string name) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			Name = name;
		}
#if DEBUGTEST
		public SqlDataConnection(IDataStore dataStore) {
			Guard.ArgumentNotNull(dataStore, "dataStore");
			this.dataStore = dataStore;
		}
#endif
#if DXPORTABLE
		public
#else 
		protected internal 
#endif
		override void SaveToXml(XElement element) {
			if(StoreConnectionNameOnly) {
				element.Add(new XAttribute(DataConnectionHelper.XmlName, Name));
				element.Add(new XAttribute(DataConnectionHelper.XmlUseAppConfig, StoreConnectionNameOnly));
				return;
			}
			base.SaveToXml(element);
			if(ShouldSerializeProviderKey())
				element.Add(new XAttribute(DataConnectionHelper.XmlProviderKey, ProviderKey));
			XElement parametersElement = SaveParametersToXml();
			if(parametersElement != null)
				element.Add(parametersElement);
		}
		XElement SaveParametersToXml() {
			if(ShouldSerializeParameters()) {
				XElement parametersElement = new XElement(DataConnectionHelper.XmlParameters);
				foreach(KeyValuePair<string, string> pair in this.parameters) {
					XElement parameterElement = new XElement(DataConnectionHelper.XmlParameter);
					parameterElement.Add(new XAttribute(DataConnectionHelper.XmlName, pair.Key));
					parameterElement.Add(new XAttribute(DataConnectionHelper.XmlValue, pair.Value));
					parametersElement.Add(parameterElement);
				}
				return parametersElement;
			}
			return null;
		}
		public override DBSchema GetDBSchema() {
			return GetDBSchema(true);
		}
		public override DBSchema GetDBSchema(string[] tableList) {
			IDataStoreSchemaExplorer schemaExplorer = CheckSchemaExplorer();
			List<DBTable> tables = new List<DBTable>();
			List<DBTable> views = new List<DBTable>();
			GetTablesAndViewsDBSchema(tableList, schemaExplorer, tables, views);
			return new DBSchema(tables.ToArray(), views.ToArray());
		}
		public DBSchema GetDBSchema(bool loadColumns) {
			if(this.dbSchema != null)
				return this.dbSchema;
			IDataStoreSchemaExplorer schemaExplorer = SchemaExplorer;
			if(schemaExplorer == null)
				throw new InvalidOperationException("The connection is not open.");
			DBTable[] tables = GetDBSchemaTables(schemaExplorer, loadColumns);
			DBTable[] views = GetDBSchemaViews(schemaExplorer, loadColumns);
			DBStoredProcedure[] procedures = GetStoredProceduresSchema(schemaExplorer);
			this.dbSchema = new DBSchema(tables, views, procedures);
			return this.dbSchema;
		}
		internal void DropDBSchemaCache() { dbSchema = null; }
		public void LoadDBColumns(params DBTable[] tables) {
			IDataStoreSchemaExplorer schemaExplorer = CheckSchemaExplorer();
			IDataStoreSchemaExplorerEx schemaExplorerEx = schemaExplorer as IDataStoreSchemaExplorerEx;
			if(schemaExplorerEx != null)
				schemaExplorerEx.GetColumns(tables);
		}
		IDataStoreSchemaExplorer CheckSchemaExplorer() {
			IDataStoreSchemaExplorer schemaExplorer = SchemaExplorer;
			if (schemaExplorer == null)
				throw new InvalidOperationException("The connection is not open.");
			return schemaExplorer;
		}
#if DXPORTABLE
		public
#else 
		protected internal 
#endif
		override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string useAppConfig = XmlHelperBase.GetAttributeValue(element, DataConnectionHelper.XmlUseAppConfig);
			if (!string.IsNullOrEmpty(useAppConfig) && StringExtensions.CompareInvariantCultureIgnoreCase(useAppConfig, bool.TrueString) == 0) {
				StoreConnectionNameOnly = true;
			} else {
				string providerKeyAttr = XmlHelperBase.GetAttributeValue(element, DataConnectionHelper.XmlProviderKey);
				if(!string.IsNullOrEmpty(providerKeyAttr))
					ProviderKey = providerKeyAttr;
				XElement parametersElement = element.Element(DataConnectionHelper.XmlParameters);
				if(parametersElement != null)
					LoadParametersFromXml(parametersElement);
			}
		}
		void LoadParametersFromXml(XElement parametersElement) {
			ConnectionString = null; 
			this.parameters.Clear();
			foreach(XElement parameterElement in parametersElement.Elements(DataConnectionHelper.XmlParameter)) {
				string parameterName = XmlHelperBase.GetAttributeValue(parameterElement, DataConnectionHelper.XmlName);
				if(String.IsNullOrEmpty(parameterName))
					throw new XmlException();
				string parameterValue = XmlHelperBase.GetAttributeValue(parameterElement, DataConnectionHelper.XmlValue);
				if(parameterValue == null)
					throw new XmlException();
				this.parameters.Add(parameterName, parameterValue);
			}
		}
		protected override void ApplyParameters(DataConnectionParametersBase dataConnectionParameters) {
			CustomStringConnectionParameters customStringParameters = dataConnectionParameters as CustomStringConnectionParameters;
			if(customStringParameters == null) {
				dataConnectionParameters.Factory = DataConnectionHelper.GetProviderFactory(DataConnectionParametersRepository.Instance.GetKeyByItem(dataConnectionParameters));
				this.parameters.Clear();
				Dictionary<string, string> paramsDict = DataAccessConnectionParameter.GetParamsDict(dataConnectionParameters);
				foreach(KeyValuePair<string, string> pair in paramsDict) {
					if(pair.Key == DataConnectionHelper.PasswordParam)
						this.password = pair.Value;
					else if(pair.Key == DataConnectionHelper.UserIdParam)
						this.userId = pair.Value;
					else
						this.parameters.Add(pair.Key, pair.Value ?? string.Empty);
				}
#if DEBUGTEST
				Tests.TestConnectionParameters testParams = dataConnectionParameters as Tests.TestConnectionParameters;
				if(testParams != null)
					if(testParams.SupportSql)
						this.parameters.Add("suppportSql", "true");
#endif
				this.factory = dataConnectionParameters.Factory;
				ConnectionString = string.Empty;
			}
			base.ApplyParameters(dataConnectionParameters);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Close();
				if(this.objectsToDispose != null) {
					foreach(IDisposable obj in this.objectsToDispose.Where(obj => obj != null))
						obj.Dispose();
					this.objectsToDispose = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override void CreateDataStoreCore() {
#if !DXPORTABLE
			if(HasConnectionString) {
				DataConnectionHelper.RegisterProviders();
				this.dataStore = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.SchemaAlreadyExists, out this.objectsToDispose);
			} else
#endif
				this.dataStore = this.factory.CreateProvider(ActualParameters, AutoCreateOption.SchemaAlreadyExists, out this.objectsToDispose);
		}
		public override void Close() {
			if(!IsConnected)
				return;
			ISqlDataStore sqlDataStore = this.dataStore as ISqlDataStore;
			if(sqlDataStore != null && sqlDataStore.Connection != null)
				sqlDataStore.Connection.Close();
			IDisposable toDispose = this.dataStore as IDisposable;
			if(toDispose != null)
				toDispose.Dispose();
			this.dataStore = null;
		}
		public override DataConnectionParametersBase CreateDataConnectionParameters() {
			DataConnectionParametersBase dataConnectionParameters = HasConnectionString ? new CustomStringConnectionParameters(ConnectionString) : DataConnectionHelper.CreateDataConnectionParameters(this.factory, ActualParameters);
#if DEBUGTEST && !DXPORTABLE
			Tests.TestConnectionParameters testParams = dataConnectionParameters as Tests.TestConnectionParameters;
			if(testParams != null) {
				string supportSql;
				if(ActualParameters.TryGetValue("suppportSql", out supportSql))
					if(supportSql=="true")
					testParams.SupportSql = true;					
			}
#endif
			return dataConnectionParameters;
		}
		public override string CreateConnectionString() { return CreateConnectionString(false); }
		public string CreateConnectionString(bool blackoutCredentials) {
			if(HasConnectionString)
				return ConnectionString;
			if(Factory == null)
				return string.Empty;
			DataConnectionParametersBase connParameters = DataConnectionHelper.CreateDataConnectionParameters(this.factory, ActualParameters);
			if(blackoutCredentials)
				DataConnectionHelper.BlackoutCredentials(connParameters);
			Dictionary<string, string> paramsDict = DataAccessConnectionParameter.GetParamsDict(connParameters);
			return Factory.GetConnectionString(paramsDict);
		}
		public void BlackoutCredentials() {
			DataConnectionHelper.BlackoutCredentials(ConnectionParameters);
			ApplyParameters(ConnectionParameters);
		}
		bool ShouldSerializeProviderKey() {
			return !HasConnectionString;
		}
		bool ShouldSerializeParameters() {
			return !HasConnectionString;
		}
		internal object GetStoredProcSchema(string storedProcName) {
			if(!IsConnected)
				throw new InvalidOperationException("The connection is not open.");
			ISupportStoredProc schemaProvider = DataStore as ISupportStoredProc;
			if(schemaProvider == null)
				return null;
			DBStoredProcedureResultSet resultSet = schemaProvider.GetStoredProcedureTableSchema(storedProcName);
			ResultTable resultTable = new ResultTable(storedProcName);
			foreach(DBNameTypePair column in resultSet.Columns)
				resultTable.AddColumn(column.Name, DBColumn.GetType(column.Type));
			return resultTable;
		}
		internal ResultTable GetCustomSqlSchema(CustomSqlQuery customSqlQuery, IEnumerable<IParameter> parameters) {
			IDataStoreEx dataStoreEx = (IDataStoreEx)CommandChannel;
			IList<IParameter> parametersList = parameters.Select(QueryParameter.FromIParameter).Cast<IParameter>().ToList();
			QueryParameterCollection parameterCollection = new QueryParameterCollection(parametersList.Select(p => new OperandValue(p.Value)).ToArray());
			Query query = new Query(customSqlQuery.Sql, parameterCollection, parametersList.Select(p => p.Name).ToArray());
			ResultTable customSqlSchema = new ResultTable(customSqlQuery.Name);
			ColumnInfoEx[] resultColumns = dataStoreEx.SelectSchema(query);
			for(int i = 0; i < resultColumns.Length; i++) {
				ColumnInfoEx column = resultColumns[i];
				string cleanColumnName = ResultTable.CleanColumnName(column.Name, i);
				customSqlSchema.AddColumn(cleanColumnName, column.Type);
			}
			return customSqlSchema;
		}
		internal bool IsStoredProceduresSupported() {
			if(!IsConnected)
				throw new InvalidOperationException("The connection is not open.");
			return DataStore is ISupportStoredProc;
		}
		string INamedItem.Name { get { return Name + (StoreConnectionNameOnly ? string.Empty : DataAccessLocalizer.GetString(DataAccessStringId.ConnectionStringPostfixServerExplorer)); } set { Name = value; } }
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
