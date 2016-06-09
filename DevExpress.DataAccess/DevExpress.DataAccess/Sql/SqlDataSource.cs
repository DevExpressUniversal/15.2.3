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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Entity;
using DevExpress.Data.Filtering;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Services.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraReports;
#if !DXPORTABLE
using System.Drawing.Design;
using DevExpress.DataAccess.Wizard.Native;
#endif
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress.DataAccess.Sql {
#if !DXPORTABLE
	[Designer("DevExpress.DataAccess.Design.VSSqlDataSourceDesigner," + AssemblyInfo.SRAssemblyDataAccessDesignFull, typeof(IDesigner))]
	[XRDesigner("DevExpress.DataAccess.UI.Design.XRSqlDataSourceDesigner," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(IDesigner))]
	[ToolboxBitmap(typeof(ResFinder), "Bitmaps256.SqlDataSource.bmp")]
#endif
	[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DataAccess.Sql.SqlDataSource", "SqlDataSource")]
	[Description("A component for quick binding to SQL data sources.")]
	[ToolboxTabName(AssemblyInfo.DXTabData)]
	[DXToolboxItem(true)]
	public class SqlDataSource : DataComponentBase, IListSource, IDataConnectionParametersService, IListAdapterAsync {
		static SqlDataSource() {
			AllowCustomSqlQueries = true;
		}
		class ConnectionNameTypeConverter : TypeConverter {
			#region Overrides of TypeConverter
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				if(sourceType == typeof(string))
					return false;
				return base.CanConvertFrom(context, sourceType);
			}
			#endregion
		}
		class ConnectionParametersTypeConverter : ReadOnlyTypeConverter {
			public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
				return ((SqlDataSource)context.Instance).ConnectionParameters != null;
			}
		}
		const string defaultConnectionName = "Connection";
		readonly SqlQueryCollection queries;
		readonly MasterDetailInfoCollection relations;
		SqlDataConnection connection;
		DBSchema schema;
		readonly object schemaLock = new object();
		string connectionName;
		DataConnectionParametersBase connectionParameters;
		readonly ResultSet result;
		bool filled;
		public event CustomizeFilterExpressionEventHandler CustomizeFilterExpression;
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataSourceConnectionError")]
#endif
		public event ConnectionErrorEventHandler ConnectionError;
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataSourceConfigureDataConnection")]
#endif
		public event ConfigureDataConnectionEventHandler ConfigureDataConnection;
		public event ValidateCustomSqlQueryEventHandler ValidateCustomSqlQuery;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SqlDataSource(IContainer container) : this() {
			Guard.ArgumentNotNull(container, "container");
			container.Add(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SqlDataSource() {
			queries = new SqlQueryCollection(this);
			relations = new MasterDetailInfoCollection(this);
			this.result = new ResultSet(this);
			filled = false;
			AddServices();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SqlDataSource(string connectionName, DataConnectionParametersBase connectionParameters) : this() {
			Guard.ArgumentIsNotNullOrEmpty(connectionName, "connectionName");
			Guard.ArgumentNotNull(connectionParameters, "connectionParameters");
			this.connectionParameters = connectionParameters;
			this.connectionName = connectionName;
		}
		public SqlDataSource(DataConnectionParametersBase connectionParameters) : this(defaultConnectionName, connectionParameters) {
		}
		public SqlDataSource(string connectionName) : this() {
			Guard.ArgumentIsNotNullOrEmpty(connectionName, "connectionName");
			this.connectionName = connectionName;
		}
		void AddServices() {
			((IServiceContainer)this).AddService(typeof(IDataConnectionParametersService), this);
#if !DXPORTABLE
			((IServiceContainer)this).AddService(typeof(IConnectionStringsProvider), new RuntimeConnectionStringsProvider());
			((IServiceContainer)this).AddService(typeof(IConnectionProviderService), new ConnectionProviderService(this));
#endif
		}
		DBSchemaProviderAsyncHelper GetDBSchemaProviderAsyncHelper() {
			var dataConnectionParametersService = GetService(typeof(IDataConnectionParametersService)) as IDataConnectionParametersService;
			return new DBSchemaProviderAsyncHelper(dataConnectionParametersService);
		}
		internal Task LoadDBSchemaAsync(CancellationToken cancellationToken) {
			var dbSchemaProviderAsyncHelper = GetDBSchemaProviderAsyncHelper();
			return dbSchemaProviderAsyncHelper.GetSchemaAsync(Connection, cancellationToken)
					.ContinueWith(task => {
						if(task.IsFaulted)
							throw task.Exception;
						schema = task.Result;
					}, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default);
		}
		Task LoadDBSchemaAsync(string[] tableList, string[] proceduresList, CancellationToken cancellationToken) {
			var dbSchemaProviderAsyncHelper = GetDBSchemaProviderAsyncHelper();
			Action<Task<DBSchema>> merge = task => {
				if(task.IsFaulted && task.Exception != null)
					throw task.Exception;
				lock(schemaLock) { schema = DBSchema.MergeDbSchema(schema, task.Result); }
			};
			return Task.Factory.StartNew(() => {
				if(tableList.Length != 0)
					dbSchemaProviderAsyncHelper.GetSchemaAsync(Connection, tableList, cancellationToken)
						.ContinueWith(merge, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default)
						.Wait(cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();
				if(proceduresList.Length != 0)
					dbSchemaProviderAsyncHelper.GetStoredProceduresAsync(Connection, proceduresList, cancellationToken)
						.ContinueWith(merge, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default)
						.Wait(cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}
		public void Invalidate() {
			ClearResult();
		}
		protected internal void RaiseValidateCustomSqlQuery(ValidateCustomSqlQueryEventArgs e) {
			if (ValidateCustomSqlQuery != null)
				ValidateCustomSqlQuery(this, e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(connection != null)
					connection.Dispose();
			}
			base.Dispose(disposing);
		}
		#region IQueryOwner Members
		[Browsable(false)]
		public DBSchema DBSchema { 
			get { return schema; }
			internal set { schema = value; }
		}
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataSourceConnection")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public SqlDataConnection Connection {
			get {
				if(connection == null) {
					if(connectionParameters != null)
						connection = new SqlDataConnection(connectionName, connectionParameters);
					else if(connectionName != null)
						CreateConnection(connectionName);
					else
						return null;
				}
				return connection;
			}
		}
		internal void AssignConnection(SqlDataConnection sourceConnection) {
			SetConnectionInfo(sourceConnection);
			this.connection = sourceConnection;
		}
		[DefaultValue(null)]
		[TypeConverter(typeof(ConnectionNameTypeConverter))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.SqlDataSource.ConnectionName")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataSourceConnectionName")]
#endif
		[LocalizableCategory(DataAccessStringId.PropertyGridConnectionCategoryName)]
		public string ConnectionName {
			get { return connectionName; }
			set {
				if (connectionName != value) {
					DropConnection();
					connectionName = value;
				}
			}
		}
		[DefaultValue(null)]
		[TypeConverter(typeof(ConnectionParametersTypeConverter))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.SqlDataSource.ConnectionParameters")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataSourceConnectionParameters")]
#endif
		[LocalizableCategory(DataAccessStringId.PropertyGridConnectionCategoryName)]
		public DataConnectionParametersBase ConnectionParameters {
			get {
				return connectionParameters;
			}
			set {
				if (!DataConnectionParametersComparer.Equals(connectionParameters, value)) {
					DropConnection();
					connectionParameters = value;
				}
			}
		}
		internal void SetConnectionInfo(SqlDataConnection sourceConnection) {
			if(sourceConnection != null) {
				this.ConnectionName = sourceConnection.Name;
				if(sourceConnection.StoreConnectionNameOnly) {
					this.connectionParameters = null;
				}
				else
					this.ConnectionParameters = sourceConnection.ConnectionParameters ?? sourceConnection.CreateDataConnectionParameters();
			}
			else {
				this.connectionName = null;
				this.connectionParameters = null;
			}
		}
		void CreateConnection(string connectionName) {
			IConnectionProviderService connectionProviderService = (IConnectionProviderService)GetService(typeof(IConnectionProviderService));
			if(connectionProviderService == null)
				throw new InvalidOperationException("Cannot find the IConnectionProviderService.");
			connection = connectionProviderService.LoadConnection(connectionName);
		}
		void DropConnection() {
			DBSchema = null;
			if(connection != null) {
				if(connection.IsConnected)
					connection.Close();
				connection.Dispose();
				connection = null;
			}
		}
		#endregion
		#region IDataConnectionParametersService Members
		DataConnectionParametersBase IDataConnectionParametersService.RaiseConfigureDataConnection(string connectionName, DataConnectionParametersBase parameters) {
			if(ConfigureDataConnection == null)
				return parameters;
			ConfigureDataConnectionEventArgs eventArgs = new ConfigureDataConnectionEventArgs(connectionName, parameters);
			ConfigureDataConnection(this, eventArgs);
			return eventArgs.ConnectionParameters;
		}
		DataConnectionParametersBase IDataConnectionParametersService.RaiseHandleConnectionError(ConnectionErrorEventArgs eventArgs) {
			if(ConnectionError != null)
				ConnectionError(this, eventArgs);
			if(eventArgs.Handled && eventArgs.Cancel)
				return null;
			return eventArgs.ConnectionParameters;
		}
		#endregion
		protected CriteriaOperator RaiseCustomFilterExpression(CustomizeFilterExpressionEventArgs eventArgs) {
			if (CustomizeFilterExpression != null) {
				CustomizeFilterExpression(this, eventArgs);
				return eventArgs.FilterExpression;
			}
			return null;
		}
		protected bool ReadyToBuild {
			get {
				return Connection != null && Queries.Count > 0 &&
					(DBSchema != null || !Queries.Any(q => q is TableQuery));
			}
		}
		[Browsable(false)]
		public static bool DisableCustomQueryValidation { get; set; }
		[Browsable(false)]
		public static bool AllowCustomSqlQueries { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.SqlDataSource.Queries")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataSourceQueries")]
#endif
		[LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName)]
		public SqlQueryCollection Queries { get { return queries; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !DXPORTABLE
		[Editor("DevExpress.DataAccess.UI.Native.Sql.MasterDetailEditor," + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.SqlDataSource.Relations")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlDataSourceRelations")]
#endif
		[LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName)]
		public MasterDetailInfoCollection Relations { get { return relations; } }
		internal FieldListDescriptor ResultSchema {
			get {
				return FieldListDescriptor.ConvertFromResultSet(result);
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(null)]
		public string ResultSchemaSerializable {
			get {
				XElement xml = GetResultSchemaXml();
				if(xml == null)
					return null;
				return Base64Helper.Encode(xml);
			}
			set {
				if(value == null)
					return;
				XElement xml = Base64Helper.Decode(value);
				if(xml == null)
					return;
				SetResultSchemaXml(xml);
			}
		}		
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty]
		public string Base64 {
			get {
				XElement xml = SaveToXml();
				return Base64Helper.Encode(xml);
			}
			set {
				XElement xml = Base64Helper.Decode(value);
				LoadFromXml(xml);
			}
		}
		[Browsable(false)]
		public DataApi.IResultSet Result { get { return ResultSet; } }
		internal ResultSet ResultSet { get { return result; } }
		protected override IEnumerable<IParameter> AllParameters { get { return Queries.SelectMany(q => q.Parameters); } }
		public override XElement SaveToXml() {
			return SqlDataSourceSerializer.SaveToXml(this, ExtensionsProvider);
		}
		public override void LoadFromXml(XElement element) {
			SqlDataSourceSerializer.LoadFromXml(this, element, ExtensionsProvider);
		}
		internal void SetResultSchemaPart(string queryName, object dataSchema) {
			SetResultSchemaPart(this.result, queryName, dataSchema);
		}
		static void SetResultSchemaPart(ResultSet resultSet, string queryName, object dataSchema) {
			Guard.ArgumentIsNotNullOrEmpty(queryName, "queryName");
			List<ResultTable> tables = new List<ResultTable>(resultSet.Tables);
			ResultTable queryTable = tables.FirstOrDefault(table => table.TableName == queryName);
			if (queryTable == null) {
				queryTable = new ResultTable(queryName);
				tables.Add(queryTable);
			}
			else {
				queryTable.Columns.Clear();
			}
			var schema = dataSchema as ColumnInfoEx[];
			if(schema != null)
				for(int i = 0; i < schema.Length; i++)
					queryTable.AddColumn(ResultTable.CleanColumnName(schema[i].Name, i), schema[i].Type);
			else {
				DataContextBase dataContext = new DataContextBase();
				PropertyDescriptorCollection properties = dataContext[dataSchema].GetItemProperties();
				if (properties != null)
					foreach (PropertyDescriptor pd in properties) {
						FieldDescriptor fieldDescriptor = new FieldDescriptor() {
							Name = pd.Name,
							FieldType = pd.PropertyType
						};
						if (!string.IsNullOrEmpty(pd.DisplayName) && pd.DisplayName != pd.Name)
							fieldDescriptor.DisplayName = pd.DisplayName;
						queryTable.AddColumn(pd.Name, pd.PropertyType);
					}
			}
			resultSet.SetTables(tables);
		}
		internal void UpdateResultSet(IEnumerable<ResultTable> tables) {
			result.SetTables(tables);
		}
		internal void RenameResultSchemaPart(string oldQueryName, string newQueryName) {
			if (result.Tables.SingleOrDefault(t => t.TableName == newQueryName) != null)
				throw new InvalidNameException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageDuplicateItemName), newQueryName));
			ResultTable resultTable = result.Tables.SingleOrDefault(t => t.TableName == oldQueryName);
			if (resultTable != null) {
				resultTable.TableName = newQueryName;
				result.SetTablesCore(result.Tables);
			}
			UpdateResultSchemaRelations();
		}
		internal void RemoveResultSchemaPart(string queryName) {
			ResultTable table = result.Tables.SingleOrDefault(t => t.TableName == queryName);
			if(table == null)
				return;
			IEnumerable<ResultTable> newTables = result.Tables.Except(new[] { table });
			result.SetTables(newTables);
			UpdateResultSchemaRelations();
		}
		internal void UpdateResultSchemaRelations(ResultSet resultSet) {
			resultSet.ApplyRelations(Relations);
		}
		internal void UpdateResultSchemaRelations() {
			UpdateResultSchemaRelations(result);
		}
		public void RebuildResultSchema() { RebuildResultSchema((IEnumerable<IParameter>)null); }
		public void RebuildResultSchema(IEnumerable<IParameter> parameters) { RebuildResultSchema(queriesToExecute => true, parameters); }
		public void RebuildResultSchema(Predicate<IEnumerable<SqlQuery>> confirmExecution) { RebuildResultSchema(confirmExecution, null); }
		public void RebuildResultSchema(Predicate<IEnumerable<SqlQuery>> confirmExecution, IEnumerable<IParameter> parameters) { RebuildResultSchema(confirmExecution, parameters, Queries); }
		public void RebuildResultSchema(Predicate<IEnumerable<SqlQuery>> confirmExecution, IEnumerable<IParameter> parameters, IEnumerable<SqlQuery> queries) {
			CancellationToken cancellationToken = new CancellationToken();
			Task<ResultSet> task = RebuildResultSchemaAsync(confirmExecution, parameters, queries, cancellationToken);
			try {
				task.Wait(cancellationToken);
				ResultSet.SetTables(task.Result.Tables);
				ResultSet.ApplyRelations(Relations);
			}
			catch(Exception ex) {
				AggregateException aex = ex as AggregateException;
				Exception taskException = aex == null ? ex : ExceptionHelper.Unwrap(aex);
				throw taskException;
			}
		}
		internal Task<ResultSet> RebuildResultSchemaAsync(Predicate<IEnumerable<SqlQuery>> confirmExecution, IEnumerable<IParameter> parameters, CancellationToken cancellationToken) {
			return RebuildResultSchemaAsync(confirmExecution, parameters, Queries, cancellationToken);
		}
		internal Task<ResultSet> RebuildResultSchemaAsync(Predicate<IEnumerable<SqlQuery>> confirmExecution, IEnumerable<IParameter> parameters, 
			IEnumerable<SqlQuery> queries, CancellationToken cancellationToken) {
			return EnsureIsReadyToBuildAsync(cancellationToken).ContinueWith(task => {
				if(task.IsFaulted)
					throw task.Exception;
				List<SqlQuery> toExecute = new List<SqlQuery>();
				List<TableQuery> tableQueries = new List<TableQuery>();
				Dictionary<string, object> resultSchemaParts = new Dictionary<string, object>();
				IEnumerable<IParameter> sourceParameters = parameters != null ? parameters as IList<IParameter> ?? parameters.ToList() : new List<IParameter>();
				foreach(SqlQuery query in queries) {
					cancellationToken.ThrowIfCancellationRequested();
					if (query is TableQuery) 
						tableQueries.Add((TableQuery)query);
					StoredProcQuery procQuery = query as StoredProcQuery;
					if(procQuery != null) {
						object part = Connection.GetStoredProcSchema(procQuery.StoredProcName);
						if(part == null)
							toExecute.Add(procQuery);
						else
							resultSchemaParts.Add(procQuery.Name, part);
					}
					else {
						CustomSqlQuery customSqlQuery = query as CustomSqlQuery;
						if(customSqlQuery != null && AllowCustomSqlQueries) {
							try {
								ResultTable customSqlSchema = Connection.GetCustomSqlSchema(customSqlQuery, sourceParameters);
								resultSchemaParts.Add(customSqlQuery.Name, customSqlSchema);
							} catch {
								toExecute.Add(customSqlQuery);
							}
						}
					}
				}
				cancellationToken.ThrowIfCancellationRequested();
				if(toExecute.Count > 0 && !confirmExecution(toExecute))
					return null;
				cancellationToken.ThrowIfCancellationRequested();
				ResultSet resultSet = new ResultSet(this);
				foreach(KeyValuePair<string, object> part in resultSchemaParts)
					SetResultSchemaPart(resultSet, part.Key, part.Value);
				foreach (TableQuery query in tableQueries) {
					object querySchema = query.GetDataSchema(DBSchema);
					SetResultSchemaPart(resultSet, query.Name, querySchema);
				}
				foreach(var query in toExecute) {
					var data = ExecuteQuery(query, new QueryExecutor(Connection, DBSchema, null, sourceParameters), cancellationToken);
					SetResultSchemaPart(resultSet, query.Name, data);
				}
				return resultSet;
			}, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default);
		}
		protected Task EnsureIsReadyToBuildAsync(CancellationToken cancellationToken) {
			return Task.Factory.StartNew(() => {
				if(Connection == null)
					throw new DatabaseConnectionException("No connection has been specified.");
				IEnumerable<string> tables = queries.SelectMany(query => {
					TableQuery tableQuery = query as TableQuery;
					if(tableQuery == null)
						return new string[0];
					return tableQuery.Tables.Select(table => table.Name);
				}).OrderBy(name => name);
				if(DBSchema != null)
					tables = tables.Except(DBSchema.Tables.Union(DBSchema.Views).Select(table => table.Name));
				string[] tableList = tables.ToArray();
				string[] proceduresList = queries.OfType<StoredProcQuery>().Select(p => p.StoredProcName).ToArray();
				if(!Connection.IsConnected)
					Connection.CreateDataStore(DataConnectionParametersService, cancellationToken);
				if(!Connection.IsConnected)
					throw new DatabaseConnectionException("Connection has been cancelled.");
				LoadDBSchemaAsync(tableList, proceduresList, cancellationToken).Wait(cancellationToken);				
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}
		protected void ValidateQuery(SqlQuery query) {
			if(!(Connection.DataStore is ISupportStoredProc) && (query is StoredProcQuery))
				throw new NotSupportedException("The specified data provider does not support stored procedures.");
			query.Validate(DBSchema);
		}
		public void Fill() {
			Fill(null);
		}
		protected override void Fill(IEnumerable<IParameter> sourceParameters) {
			ServiceManager serviceManager = new ServiceManager();
			serviceManager.AddService(typeof(IParameterSupplierBase), new ParameterSupplier(sourceParameters));
			((IListAdapter)this).FillList(serviceManager);
		}
		internal Task<ResultSet> FillAsync(IEnumerable<IParameter> sourceParameters, CancellationToken cancellationToken) {
			return EnsureIsReadyToBuildAsync(cancellationToken).ContinueWith<ResultSet>(task => {
				if(task.IsFaulted)
					throw task.Exception;
				var t = FillCoreAsync(sourceParameters, cancellationToken);
				t.Wait(cancellationToken);
				return t.Result;
			}, cancellationToken, TaskContinuationOptions.None, TaskScheduler.Default);
		}
		#region IListAdapterAsync Members
		readonly Semaphore fillListSemaphore = new Semaphore(1, 1);
		IAsyncResult IListAdapterAsync.BeginFillList(IServiceProvider serviceProvider, CancellationToken token) {
			if(!fillListSemaphore.WaitOne(300))
				throw new InvalidOperationException("Data source is busy");
			IParameterSupplierBase parameterSupplier = serviceProvider != null ? serviceProvider.GetService<IParameterSupplierBase>() : null;
			IEnumerable<IParameter> parameters = parameterSupplier != null ? parameterSupplier.GetIParameters() : null;
			ReplaceServiceFromProvider(typeof(IConnectionProviderService), serviceProvider);
			return FillAsync(parameters, token);
		}
		void IListAdapterAsync.EndFillList(IAsyncResult result) {
			Task<ResultSet> task = (Task<ResultSet>)result;
			try { 
				task.Wait();
				this.result.SetTables(task.Result.Tables);
				this.result.ApplyRelations(Relations);
			}
			catch(AggregateException e) {
				if(task.IsFaulted)
					throw ExceptionHelper.Unwrap(e);
			}
			finally { fillListSemaphore.Release(); }
		}
		#endregion
		#region IListAdapter Members
		void IListAdapter.FillList(IServiceProvider serviceProvider) {
			IListAdapterAsync listAdapterAsync = this;
			IAsyncResult task = listAdapterAsync.BeginFillList(serviceProvider, CancellationToken.None);
			listAdapterAsync.EndFillList(task);
		}
		bool IListAdapter.IsFilled { get { return filled; } }
		#endregion
		protected virtual Task<ResultSet> FillCoreAsync(IEnumerable<IParameter> parameters, CancellationToken cancellationToken) {
			return Task.Factory.StartNew(() => {
				filled = false;
				if(!ReadyToBuild || queries.Any(q => !(q is TableQuery)) && !Connection.IsSqlDataStore)
					return new ResultSet();
				ResultSet resultSet = new ResultSet(this);
				cancellationToken.ThrowIfCancellationRequested();
				List<Task> populateResultSet = new List<Task>(queries.Count);
				QueryExecutor executor = new QueryExecutor(this, Connection, DBSchema, CustomizeFilterExpression, parameters);
				foreach(SqlQuery query in queries) {
					cancellationToken.ThrowIfCancellationRequested();
					if(query is CustomSqlQuery && !AllowCustomSqlQueries)
						continue;
					SqlQuery query1 = query; 
					populateResultSet.Add(Task.Factory.StartNew(() => {
						ResultTable result = ExecuteQuery(query1, executor, cancellationToken);
						if(result != null)
							resultSet.Add(result);
					}, cancellationToken));
				}
				Task.WaitAll(populateResultSet.ToArray(), cancellationToken);
				filled = true;
				return resultSet;
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}
		protected void ExecuteReader(string queryName, DBSchema dbSchema, IEnumerable<IParameter> sourceParameters, CancellationToken token, Action<IDataReader, string, CancellationToken> action) {
			ExecuteReader(queryName, dbSchema, sourceParameters, token, (reader, cancellationToken)=> { action(reader, queryName, cancellationToken); });
		}
		protected void ExecuteReader(string queryName, DBSchema dbSchema, IEnumerable<IParameter> sourceParameters, CancellationToken token, Action<IDataReader, CancellationToken> action) {
			SqlQuery query = Queries[queryName];
			IEnumerable<IParameter> parameters = ParametersEvaluator.EvaluateParameters(sourceParameters, query.Parameters);
			IList<IParameter> parametersList = parameters as IList<IParameter> ?? parameters.ToList();
			IDataStoreEx dataStoreEx = this.connection.DataStore as IDataStoreEx;
			TableQuery tableQuery = query as TableQuery;
			if(tableQuery != null) {
				if(dataStoreEx != null) {
					SelectStatement selectStatement = tableQuery.BuildSelectStatement(dbSchema, this.connection.SqlGeneratorFormatter, parametersList);
					QueryExecutor.ApplyCustomFilter(this, tableQuery, parametersList, selectStatement, CustomizeFilterExpression);
					Query xpoQuery = new SelectSqlGenerator(this.connection.SqlGeneratorFormatter).GenerateSql(selectStatement);
					dataStoreEx.ProcessQuery(token, xpoQuery, action);
				}
				return;
			}
			CustomSqlQuery sqlQuery = query as CustomSqlQuery;
			if(sqlQuery != null) {
				QueryParameterCollection queryParameters = new QueryParameterCollection(parametersList.Select(p => new OperandValue(p.Value)).ToArray());
				if(dataStoreEx != null) {
					Query xpoQuery = new Query(sqlQuery.Sql, queryParameters, parametersList.Select(p => p.Name).ToArray());
					dataStoreEx.ProcessQuery(token, xpoQuery, action);
				}
				return;
			}
			StoredProcQuery procQuery = query as StoredProcQuery;
			if(procQuery != null) {
				if(dataStoreEx != null)
					dataStoreEx.ProcessStoredProc(token, procQuery.StoredProcName, action, parametersList.Select(p => new OperandValue(p.Value)).ToArray());
				return;
			}
			throw new NotSupportedException(string.Format("Unknown query type: {0}", query.GetType()));
		}
		protected virtual ResultTable ExecuteQuery(SqlQuery query, QueryExecutor executor, CancellationToken cancellationToken) {
			ValidateQuery(query);
			SelectedDataEx data = executor.Execute(query, cancellationToken);
			return new ResultTable(query.Name, data);
		}
		IDataConnectionParametersService DataConnectionParametersService { get {
			return GetService(typeof(IDataConnectionParametersService)) as IDataConnectionParametersService;
		} }
		void ClearResult() {
			filled = false;
			result.SetTables(new ResultTable[0]);
		}
		internal XElement GetResultSchemaXml() {
			return ResultSchema.SaveToXml();
		}
		internal void SetResultSchemaXml(XElement xml) {
			FieldListDescriptor resultSchema = FieldListDescriptor.LoadFromXml(xml);
			resultSchema.FillResultSet(result);
		}
		#region IListSource Members
		bool IListSource.ContainsListCollection { get { return true; } }
		IList IListSource.GetList() {
			return result;
		}
		#endregion
		protected override string GetDataMember() {
			return Queries.Count > 0 ? Queries[0].Name : string.Empty;
		}
		internal void UpdateRelations(string oldQueryName, string newQueryName) {
			foreach(MasterDetailInfo rel in Relations) {
				if(rel.MasterQueryName == oldQueryName)
					rel.MasterQueryName = newQueryName;
				if(rel.DetailQueryName == oldQueryName)
					rel.DetailQueryName = newQueryName;
			}
		}
	}
}
