#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Compatibility.System;
#if DXRESTRICTED
using IDataReader = System.Data.Common.DbDataReader;
#endif
namespace DevExpress.DashboardCommon {
	public enum DataProcessingMode { Client, Server }
	[
	DXToolboxItem(false),
	]
	public class  DashboardSqlDataSource : SqlDataSource, IDashboardDataSource, IDashboardDataSourceInternal {
		const DataProcessingMode DefaultDataProcessingMode = DataProcessingMode.Server;
		const string xmlDataProcessingMode = "DataProcessingMode";
		readonly Locker loadingLocker = new Locker();
		readonly CalculatedFieldsController calculatedFieldsController;
		readonly Dictionary<string, IList> lists = new Dictionary<string, IList>();
		readonly Dictionary<string, PickManager> pickManagers = new Dictionary<string, PickManager>();
		readonly Dictionary<string, DevExpress.DashboardCommon.DataProcessing.IStorage> storages = new Dictionary<string, DevExpress.DashboardCommon.DataProcessing.IStorage>();
#if !DXPORTABLE
		readonly ConcurrentDictionary<string, ServerModeQueryExecutor> serverModeSources = new ConcurrentDictionary<string, ServerModeQueryExecutor>();
#endif
		readonly DataSourceProperties properties;
		readonly DataSourceComponentNameController componentNameController;
		readonly List<SqlQuery> invalidForServerModeCustomSQL = new List<SqlQuery>();
		readonly List<DataLoaderError> errors = new List<DataLoaderError>();
		DataProcessingMode dataProcessingMode = DefaultDataProcessingMode;
		IEnumerable<IParameter> parameters;
		event EventHandler<DataChangedEventArgs> DataSourceDataChanged;
		event EventHandler<NameChangingEventArgs> NameChanging;
		event EventHandler<NameChangedEventArgs> NameChanged;
		event EventHandler CaptionChanged;
		event EventHandler<DataProcessingModeChangedEventArgs> DataProcessingModeChanged;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardSqlDataSourceCalculatedFields"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public CalculatedFieldCollection CalculatedFields { get { return calculatedFieldsController.CalculatedFields; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardSqlDataSourceDataProcessingMode"),
#endif
		Category(CategoryNames.Data),
		TypeConverter(TypeNames.DataProcessingModeConverter),
		Editor(TypeNames.DataProcessingModeEditor, typeof(UITypeEditor)),
		DefaultValue(DefaultDataProcessingMode)
		]
		public DataProcessingMode DataProcessingMode {
			get { return dataProcessingMode; }
			set {
				if(value != dataProcessingMode) {
					dataProcessingMode = value;
					calculatedFieldsController.ClearCache();
					RaiseDataProcessingModeChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardSqlDataSourceName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		Browsable(true)
		]
		public override string Name { get { return componentNameController.Name; } set { componentNameController.Name = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardSqlDataSourceComponentName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		]
		public string ComponentName { get { return componentNameController.ComponentName; } set { componentNameController.ComponentName = value; } }
		[
		Browsable(false)
		]
		public bool IsConnected {
			get {
				SqlDataConnection connection = TryGetConnection();
				return connection != null && connection.IsConnected;
			}
		}
		bool IsServerModeSupported {
			get {
#if !DXPORTABLE
				SqlDataConnection connection = TryGetConnection();
				return connection != null && connection.IsSqlDataStore;
#else
				return false;
#endif
			}
		}
		bool ConnectionWasCreated { get; set; }
		public DashboardSqlDataSource()
			: this((string)null) {
		}
		public DashboardSqlDataSource(string name, string connectionName)
			: this(name) {
			ConnectionName = connectionName;
		}
		public DashboardSqlDataSource(DataConnectionParametersBase connectionParameters)
			: this(null, connectionParameters) {
		}
		public DashboardSqlDataSource(string name, DataConnectionParametersBase connectionParameters)
			: this(name) {
			ConnectionParameters = connectionParameters;
		}
		public DashboardSqlDataSource(string name)
			: base("connection") {
			properties = new DataSourceProperties(this);
			calculatedFieldsController = new CalculatedFieldsController(this);
			calculatedFieldsController.ConstructTree += OnConstructTree;
			componentNameController = new DataSourceComponentNameController(name, loadingLocker, () => Site);
			componentNameController.NameChanged += componentNameController_NameChanged;
			componentNameController.NameChanging += componentNameController_NameChanging;
			componentNameController.CaptionChanged += componentNameController_CaptionChanged;
			IBindingList bindingList = (IBindingList)((IListSource)this).GetList();
			bindingList.ListChanged += bindingList_ListChanged;
			RefreshServices();
		}
		public DataProcessingMode GetDataProcessingMode(string queryName) {
			if(Queries.ContainsName(queryName)) {
				SqlQuery query = Queries[queryName];
				if(IsSqlServerModeSupported(query))
					return DataProcessingMode;
			}
			return DataProcessingMode.Client;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override XElement SaveToXml() {
			XElement element = base.SaveToXml();
			componentNameController.SaveComponentNameToXml(element);
			calculatedFieldsController.SaveToXml(element);
			if(dataProcessingMode != DefaultDataProcessingMode)
				element.Add(new XAttribute(xmlDataProcessingMode, DataProcessingMode));
			return element;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			componentNameController.LoadComponentNameFromXml(element);
			calculatedFieldsController.LoadFromXml(element);
			string dataProcessingModeString = XmlHelper.GetAttributeValue(element, xmlDataProcessingMode);
			if(dataProcessingModeString != null)
				DataProcessingMode = XmlHelper.EnumFromString<DataProcessingMode>(dataProcessingModeString);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void BeginInit() {
			loadingLocker.Lock();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void EndInit() {
			loadingLocker.Unlock();
		}
		void PerformFill(Action action) {
			ConnectionWasCreated = true;
			try {
				action();
			} catch(InvalidOperationException) {
				ConnectionWasCreated = false;
				throw;
			}
		}
		internal void CreateFileStorage(string queryName, string fileName, IEnumerable<IParameter> parameters) {
#if !DXPORTABLE
			if(Connection.DataStore == null)
				Connection.Open();
			ExecuteReader(queryName, Connection.GetDBSchema(true), parameters, CancellationToken.None, (dataReader, cancellationToken) => {
				new DataStorageProcessor(new TypedDataReaderIDataReaderWrapper(dataReader, GetAliases(queryName))).ProcessTables(fileName);
			});
#endif
		}
		internal void FillSync(IEnumerable<IParameter> parameters, IPlatformDependenciesService platformDependenciesService, CancellationToken cancellationToken) {
			this.parameters = parameters;
#if !DXPORTABLE
			PerformFill(() => {
				Task task = FillAsync(parameters, cancellationToken);
				while(!task.IsCompleted) {
					if(platformDependenciesService != null)
						platformDependenciesService.DoEvents();
					Thread.Sleep(10);
				}
				Task<ResultSet> resultTask = (Task<ResultSet>)task;
				try {
					ResultSet.SetTables(resultTask.Result.Tables);
					ResultSet.ApplyRelations(Relations);
					if(errors.Count>0){
						StringBuilder sb = new StringBuilder();
						foreach(DataLoaderError error in errors)
							sb.AppendLine(string.Format("{0}: {1}", error.DataSourceName, error.Message));
						throw new Exception(sb.ToString());
					}
				} catch(AggregateException e) {
					if(resultTask.IsFaulted)
						throw ExceptionHelper.Unwrap(e);
					if(e.GetBaseException() is TaskCanceledException)
						throw e.GetBaseException();
				}
			});
#else
			Fill(parameters);
#endif
		}
		protected override Task<ResultSet> FillCoreAsync(IEnumerable<IParameter> parameters, CancellationToken cancellationToken) {
#if !DXPORTABLE
			serverModeSources.Clear();
#endif
			storages.Clear();
			errors.Clear();			
			return base.FillCoreAsync(parameters, cancellationToken);
		}
		protected override void Fill(IEnumerable<IParameter> sourceParameters) {
			this.parameters = sourceParameters;
			PerformFill(() => {
				base.Fill(sourceParameters);
			});
		}
		protected override ResultTable ExecuteQuery(SqlQuery query, QueryExecutor executor, System.Threading.CancellationToken cancellationToken) {
			try {
#if !DXPORTABLE
				if (GetDataProcessingMode(query.Name) == DataProcessingMode.Server) {
					ResultTable resultTable = null;
					CriteriaOperator customFilter = null;
					ValidateQuery(query);
					TableQuery tableQuery = query.GetTableQueryForServerMode();
					CustomSqlQuery customSqlQuery = query.GetCustomSqlQueryForServerMode(DBSchema, Connection.SqlGeneratorFormatter);
					if (tableQuery != null) {
						resultTable = (ResultTable)tableQuery.GetDataSchema(DBSchema);
						customFilter = this.RaiseCustomFilterExpression(new CustomizeFilterExpressionEventArgs(tableQuery.Name, null));
					}
					else if (customSqlQuery != null) {
						SelectStatement schemaRetrievingSelectStatement = ServerModeQueryExecutor.GetSubSelect(Connection.DataStore, customSqlQuery.Sql);
						DevExpress.Xpo.DB.Helpers.Query schemaRetrievingXpoQuery = new DevExpress.Xpo.DB.Helpers.SelectSqlGenerator(Connection.SqlGeneratorFormatter).GenerateSql(schemaRetrievingSelectStatement);
						try {
							CustomSqlQuery customSqlSchemaRetrievingQuery = new CustomSqlQuery(customSqlQuery) { Sql = schemaRetrievingXpoQuery.Sql };
							resultTable = new ResultTable(query.Name, executor.Execute(customSqlSchemaRetrievingQuery, cancellationToken));
						}
						catch {
							invalidForServerModeCustomSQL.Add(query);
						}
					}
					if (resultTable != null) {
						ServerModeQueryExecutor serverModeSource = null;
						serverModeSource = new ServerModeQueryExecutor(Connection, DBSchema);
						serverModeSources.AddOrUpdate(query.Name, serverModeSource, (name, q) => serverModeSource);
						serverModeSource.Query = query;
						serverModeSource.ResultTable = resultTable;
						serverModeSource.Parameters = ParametersEvaluator.EvaluateParameters(executor.SourceParameters, query.Parameters);
						serverModeSource.CustomFilter = customFilter;
						return resultTable;
					}
				}
#endif
				if (DataSession.CalculateNewClientModeEngine) {
					if (!storages.ContainsKey(query.Name)) {
						ValidateQuery(query);
						if (Connection.IsSqlDataStore)
							ExecuteReader(query.Name, DBSchema, parameters, cancellationToken, CreateSqlStorage);
						else {
							ResultTable xmlTable = base.ExecuteQuery(query, executor, cancellationToken);
							string[] columnNames = xmlTable.Columns.Select(c => c.Name).ToArray();
							using (DataReaderExEnumerableWrapper dataReaderEx = new DataReaderExEnumerableWrapper(((ITypedList)xmlTable).GetItemProperties(null).Cast<PropertyDescriptor>().ToList(), columnNames, xmlTable)) {
								CreateStorage(query.Name, dataReaderEx, cancellationToken);
							}
						}
					}
					ResultTable table = new ResultTable(query.Name);
					IStorage storage;
					if (storages.TryGetValue(query.Name, out storage))
						table.Columns.AddRange(storage.Columns.Select(name => new ResultColumn(name, storage.GetColumnType(name), new object[] { })));
					return table;
				}
				return base.ExecuteQuery(query, executor, cancellationToken);
			}
			catch (ValidationException validationException) {
				errors.Add(new DataLoaderError(DataLoaderErrorType.Query, query.Name, validationException.Message));
			}
			catch (InvalidOperationException invalidOperationException) {
				errors.Add(new DataLoaderError(DataLoaderErrorType.Query, query.Name, invalidOperationException.Message));
			}
			catch (SqlException sqlException) {
				errors.Add(new DataLoaderError(DataLoaderErrorType.Query, query.Name, sqlException.Message));
			}
			return null;
		}
		string[] GetAliases(string queryName) {
			TableQuery query = Queries[queryName] as TableQuery;
			return query != null ? query.Tables.SelectMany(table => table.SelectedColumns).Select(c => c.ActualName).ToArray() : null;
		}
		void CreateSqlStorage(IDataReader dataReader, string queryName, CancellationToken token) {			
			CreateStorage(queryName, new TypedDataReaderIDataReaderWrapper(dataReader, GetAliases(queryName)), token);
		}
		void CreateStorage(string dataMember, ITypedDataReader dataReaderEx, CancellationToken token) {
			if(storages.ContainsKey(dataMember))
				return;
			if(GetDataProcessingMode(dataMember) != DataProcessingMode.Server) {
				IStorage result;
				DataStorageProcessor processor = new DataStorageProcessor(dataReaderEx);
				result = processor.ProcessTables(token);
				if(result != null)
					storages.Add(dataMember, result);
			}
		}
		void RefreshServices() {
#if !DXPORTABLE
			((IServiceContainer)this).RemoveService(typeof(IConnectionProviderService));
			((IServiceContainer)this).AddService(typeof(IConnectionProviderService), new DashboardConnectionProviderService());
#endif
		}
		void OnConstructTree(object sender, EventArgs e) {
			foreach(KeyValuePair<string, PickManager> pickManagerInfo in pickManagers) {
				pickManagerInfo.Value.ConstructTree(pickManagerInfo.Key);
			}
		}
		void componentNameController_NameChanged(object sender, NameChangedEventArgs e) {
			if(NameChanged != null)
				NameChanged(this, e);
		}
		void componentNameController_CaptionChanged(object sender, EventArgs e) {
			if(CaptionChanged != null)
				CaptionChanged(this, e);
		}
		void componentNameController_NameChanging(object sender, NameChangingEventArgs e) {
			if(NameChanging != null)
				NameChanging(this, e);
		}
		void bindingList_ListChanged(object sender, ListChangedEventArgs e) {
			Refresh();
			RaiseDataChaged();
		}
		void Refresh() {
			IListSource listSource = (IListSource)this;
			IList li = listSource.GetList();
			PropertyDescriptorCollection pds = ((ITypedList)li[0]).GetItemProperties(null);
			pickManagers.Clear();
			lists.Clear();
			SqlDataConnection connection = TryGetConnection();
			bool xml = connection != null && !connection.IsSqlDataStore;
			foreach(PropertyDescriptor pd in pds) {
				IList list = (IList)pd.GetValue(li);
				lists.Add(pd.Name, list);
				PickManagerWithCalcFields pickManager = new PickManagerWithCalcFields(pd.Name, this, pd.Name, calculatedFieldsController);
				pickManager.ConstructTree(pd.Name);
				pickManagers.Add(pd.Name, pickManager);
				if(xml && !storages.ContainsKey(pd.Name)) {
				}
			}
			calculatedFieldsController.ClearCache();
		}
		void RaiseDataChaged() {
			if(DataSourceDataChanged != null)
				DataSourceDataChanged(this, new DataChangedEventArgs(true));
		}
		void RaiseDataProcessingModeChanged() {
			if(DataProcessingModeChanged != null)
				DataProcessingModeChanged(this, new DataProcessingModeChangedEventArgs(this));
		}
		bool IsSqlServerModeSupported(SqlQuery query) {
			SqlDataConnection connection = TryGetConnection();
			return !(query is StoredProcQuery) && !invalidForServerModeCustomSQL.Contains(query) && IsServerModeSupported;
		}
		bool ParametersContainsDisplayMemberServerMode(IDataSourceSchema dataSourceSchema, string valueMember, string displayMember) {
			return !string.IsNullOrEmpty(displayMember) && valueMember != displayMember && dataSourceSchema.GetField(displayMember) != null;
		}
		List<ParameterValueViewModel> CreateModelValuesFromDynamicLookupSettingsServerMode(string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			List<ParameterValueViewModel> parameterValues = new List<ParameterValueViewModel>();
			if(((IDashboardDataSource)this).GetPivotDataSource(queryName) != null) {
				if(!string.IsNullOrEmpty(valueMember)) {
					PivotGridData data = new PivotGridData();
					data.PivotDataSource = (IPivotGridDataSource)((ICloneable)((IDashboardDataSource)this).GetPivotDataSource(queryName)).Clone();
					data.RetrieveFields(XtraPivotGrid.PivotArea.FilterArea, false);
					PivotGridFieldBase fieldValue = data.Fields[valueMember] ?? CreateCalculatedField(valueMember, queryName, data, provider);
					PivotGridFieldBase fieldDisplayText = null;
					if(!string.IsNullOrEmpty(displayMember))
						fieldDisplayText = data.Fields[displayMember] ?? CreateCalculatedField(displayMember, queryName, data, provider);
					if(fieldValue != null) {
						if(fieldDisplayText == null || fieldDisplayText == fieldValue) {
							object[] sortedUniqueValues = new object[0];
							try {
								sortedUniqueValues = data.GetSortedUniqueValues(fieldValue);
							}
							catch { }
							foreach(object val in sortedUniqueValues)
								parameterValues.Add(new ParameterValueViewModel() {
									Value = val,
									DisplayText = object.Equals(null, val) ? string.Empty : val.ToString()
								});
						} else {
							data.BeginUpdate();
							fieldValue.Area = XtraPivotGrid.PivotArea.RowArea;
							fieldValue.Visible = true;
							fieldDisplayText.Area = XtraPivotGrid.PivotArea.RowArea;
							fieldDisplayText.Visible = true;
							data.EndUpdate();
							for(int i = 0; i < data.PivotDataSource.GetCellCount(false); i++)
								if(data.PivotDataSource.GetObjectLevel(false, i) == 1)
									parameterValues.Add(new ParameterValueViewModel() {
										Value = data.PivotDataSource.GetFieldValue(false, i, 0),
										DisplayText = (data.PivotDataSource.GetFieldValue(false, i, 1) ?? string.Empty).ToString()
									});
						}
					}
				}
			}
			return parameterValues;
		}
		PivotGridFieldBase CreateCalculatedField(string displayText, string queryName, PivotGridData data, IActualParametersProvider parameters) {
			if(string.IsNullOrEmpty(displayText))
				return null;
			PivotGridFieldBase field = new XtraPivotGrid.PivotGridFieldBase();
			if(CalculatedFieldsController.PrepareCalculatedField(this, queryName, displayText, field, parameters)) {
				data.Fields.Add(field);
				return field;
			} else
				return null;
		}
		IList GetListByQuery(string queryName) {
			if(queryName == null || GetDataProcessingMode(queryName) == DataProcessingMode.Server)
				return null;
			IList list = null;
			lists.TryGetValue(queryName, out list);
			return list;
		}
		SqlDataConnection TryGetConnection() {
			return ConnectionWasCreated ? Connection : null;
		}
#region IDashboardDataSource
#if !DXPORTABLE
		IDataProvider IDashboardDataSource.DataProvider { get { return null; } set { throw new NotSupportedException(); } }
#pragma warning disable 612, 618
		OlapDataProvider IDashboardDataSource.OlapDataProvider { get { return null; } }
		SqlDataProvider IDashboardDataSource.SqlDataProvider { get { return null; } }
#pragma warning restore 612, 618
#endif
		bool IDashboardDataSource.HasDataProvider { get { return false; } }
		bool IDashboardDataSource.IsServerModeSupported { get { return IsServerModeSupported; } }
		IEnumerable<IParameter> IDashboardDataSource.Parameters { get { return parameters; } }
		string IDashboardDataSource.Filter { get { return null; } set {  } }
		object IDashboardDataSource.Data { get { return this; } set { throw new NotSupportedException(); } }
		IDashboardDataSourceInternal IDashboardDataSource.GetDataSourceInternal() {
			return this;
		}
		IDataSourceSchema IDashboardDataSource.GetDataSourceSchema(string dataMember) {
			if(dataMember == null)
				return null;
			PickManager dataSchemaProvider;
			if(pickManagers.TryGetValue(dataMember, out dataSchemaProvider))
				return dataSchemaProvider;
			return null;
		}
		ICalculatedFieldsController IDashboardDataSource.GetCalculatedFieldsController() {
			return calculatedFieldsController;
		}
#endregion
#region IDashboardDataSourceInternal
		Dashboard IDashboardDataSourceInternal.Dashboard { get; set; }
		DataSourceProperties IDashboardDataSourceInternal.Properties { get { return properties; } }
		event EventHandler<DataChangedEventArgs> IDashboardDataSourceInternal.DataSourceDataChanged {
			add { DataSourceDataChanged += value; }
			remove { DataSourceDataChanged -= value; }
		}
		event EventHandler<NameChangedEventArgs> IDashboardDataSourceInternal.NameChanged {
			add { NameChanged += value; }
			remove { NameChanged -= value; }
		}
		event EventHandler IDashboardDataSourceInternal.CaptionChanged {
			add { CaptionChanged += value; }
			remove { CaptionChanged -= value; }
		}
		event EventHandler<DataProcessingModeChangedEventArgs> IDashboardDataSourceInternal.DataProcessingModeChanged {
			add { DataProcessingModeChanged += value; }
			remove { DataProcessingModeChanged -= value; }
		}
		bool IDashboardDataSourceInternal.GetIsSqlServerMode(DataProcessingMode dataProcessingMode, string queryName) {
			if(Queries.ContainsName(queryName)) {
				SqlQuery query = Queries[queryName];
				if(IsSqlServerModeSupported(query))
					return dataProcessingMode == DataProcessingMode.Server;
			}
			return false;
		}
		bool IDashboardDataSourceInternal.IsSqlServerMode(string queryName) {
			return GetDataProcessingMode(queryName) == DataProcessingMode.Server;
		}
		Type IDashboardDataSourceInternal.ServerGetUnboundExpressionType(string expression, string queryName) {
#if !DXPORTABLE
			if(GetDataProcessingMode(queryName) == DataProcessingMode.Server) {
				ServerModeQueryExecutor serverModeSource = null;
				if(serverModeSources.TryGetValue(queryName, out serverModeSource))
					return serverModeSource.PivotServerModeDataSource.GetUnboundExpressionType(expression, true);
			}
#endif
			return typeof(object);
		}
		CalculatedFieldDataColumnInfo IDashboardDataSourceInternal.CreateCalculatedFieldColumnInfo(CalculatedField field, IEnumerable<IParameter> parameters) {
			return new CalculatedFieldDataColumnInfo(field, GetRootNode(field.DataMember), calculatedFieldsController.CalculatedFields, parameters);
		}
		DataNode GetRootNode(string dataMember) {
			IDataSourceSchema dataSourceSchema = ((IDashboardDataSource)this).GetDataSourceSchema(dataMember);
			return dataSourceSchema == null ? null : dataSourceSchema.RootNode;
		}
		bool IDashboardDataSourceInternal.ContainsParametersDisplayMember(string valueMember, string displayMember, string queryName) {
			IDataSourceSchema dataSourceSchema = ((IDashboardDataSource)this).GetDataSourceSchema(queryName);
			if(GetDataProcessingMode(queryName) == DataProcessingMode.Server) {
				return ParametersContainsDisplayMemberServerMode(dataSourceSchema, valueMember, displayMember);
			} else {
				return DataSourceHelper.ContainsDisplayMember(dataSourceSchema, displayMember, valueMember);
			}
		}
		List<ParameterValueViewModel> IDashboardDataSourceInternal.GetParameterValues(string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			if(GetDataProcessingMode(queryName) == DataProcessingMode.Server)
				return CreateModelValuesFromDynamicLookupSettingsServerMode(valueMember, displayMember, queryName, provider);
			else
				return DataSourceHelper.GetDynamicLookupValues(this, queryName, displayMember, valueMember);
		}
		void IDashboardDataSourceInternal.SetParameters(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
		}
		IPivotGridDataSource IDashboardDataSourceInternal.GetPivotDataSource(string queryName) {
			if(queryName == null || GetDataProcessingMode(queryName) == DataProcessingMode.Client)
				return null;
#if !DXPORTABLE
			ServerModeQueryExecutor serverModeSource = null;
			if(serverModeSources.TryGetValue(queryName, out serverModeSource))
				return serverModeSource.PivotServerModeDataSource;
#endif
			return null;
		}
		IList IDashboardDataSourceInternal.GetListSource(string dataMember) {
			return GetListByQuery(dataMember);
		}
		DevExpress.DashboardCommon.DataProcessing.IStorage IDashboardDataSourceInternal.GetStorage(string dataMember) {
			if(dataMember == null || GetDataProcessingMode(dataMember) == DataProcessingMode.Server)
				return null;
			DevExpress.DashboardCommon.DataProcessing.IStorage storage = null;
			return storages.TryGetValue(dataMember, out storage) ? storage : null;
		}
		string IDashboardDataSourceInternal.GetName_13_1() {
			return componentNameController.Name_13_1;
		}
		IEnumerable<string> IDashboardDataSourceInternal.GetDataSets() {
			return Queries.Select(q => q.Name);
		}
		object IDashboardDataSourceInternal.GetDataSchema(string queryName) {
			if(GetDataProcessingMode(queryName) == DataProcessingMode.Server) {
#if !DXPORTABLE
				ServerModeQueryExecutor serverModeSource = null;
				if(serverModeSources.TryGetValue(queryName, out serverModeSource))
					return serverModeSource.ResultTable;
#endif
			} else {
				return GetListByQuery(queryName);
			}
			return null;
		}
#endregion
#region IDashboardComponent
		event EventHandler<NameChangingEventArgs> IDashboardComponent.NameChanging {
			add { NameChanging += value; }
			remove { NameChanging -= value; }
		}
#endregion
#region ISupportPrefix
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultSqlDataSourceName); } }
#endregion
	}
	static class QueryExtensions {
		static bool ThreatAsCustomSqlForServerMode(this TableQuery query) {
			return query.Grouping != null && query.Grouping.Count > 0;
		}
		public static TableQuery GetTableQueryForServerMode(this SqlQuery query) {
			TableQuery tableQuery = query as TableQuery;
			if(tableQuery == null || tableQuery.ThreatAsCustomSqlForServerMode())
				return null;
			return tableQuery;
		}
		public static CustomSqlQuery GetCustomSqlQueryForServerMode(this SqlQuery query, DBSchema dbSchema, ISqlGeneratorFormatter sqlFormatter) {
			CustomSqlQuery customSqlQuery = query as CustomSqlQuery;
			if(customSqlQuery != null)
				return customSqlQuery;
			if(dbSchema == null || sqlFormatter == null)
				return null;
			TableQuery tableQuery = query as TableQuery;
			if(tableQuery == null || !tableQuery.ThreatAsCustomSqlForServerMode())
				return null;
			customSqlQuery = new CustomSqlQuery(query.Name, tableQuery.BuildSql(dbSchema, sqlFormatter));
			foreach(QueryParameter parameter in query.Parameters)
				customSqlQuery.Parameters.Add(new QueryParameter(parameter.Name, parameter.Type, parameter.Value));
			return customSqlQuery;
		}
	}
}
namespace DevExpress.DashboardCommon.Native {
	public class TypedDataReaderIDataReaderWrapper : ITypedDataReader {
		#region CastHelper
		static class CastHelper {
			static Dictionary<Type, Delegate> casters = new Dictionary<Type, Delegate>();
			static Func<TIn, TOut> GetWorker<TIn, TOut>() {
				DXContract.Requires(typeof(TIn) == typeof(TOut));
				Type type = typeof(TIn);
				Delegate result;
				if(!casters.TryGetValue(type, out result)) {
					ParameterExpression value = Expression.Parameter(typeof(TIn));
					Expression cast = Expression.Convert(value, typeof(TOut));
					result = Expression.Lambda(cast, value).Compile();
					casters.Add(type, result);
				}
				return (Func<TIn, TOut>)result;
			}
			public static TOut Cast<TIn, TOut>(TIn value) {
				Func<TIn, TOut> worker = GetWorker<TIn, TOut>();
				return worker(value);
			}
		}
		#endregion
		IDataReader dataReader;
		string[] aliases;
		public TypedDataReaderIDataReaderWrapper(IDataReader dataReader, string[] aliases) {
			this.dataReader = dataReader;
			this.aliases = aliases;
		}
		TimeSpan GetTimeSpan(int i) {
			object value = dataReader.GetValue(i);
			double seconds = Convert.ToDouble(value);
			if(seconds > TimeSpan.MaxValue.TotalSeconds - 0.0005 && seconds < TimeSpan.MaxValue.TotalSeconds + 0.0005)
				return TimeSpan.MaxValue;
			if(seconds < TimeSpan.MinValue.TotalSeconds + 0.0005 && seconds > TimeSpan.MinValue.TotalSeconds - 0.0005)
				return TimeSpan.MinValue;
			return TimeSpan.FromSeconds(seconds);
		}
		#region ITypedDataReader Members
		public int FieldCount { get { return dataReader.FieldCount; } }
		public Type GetFieldType(int i) { return dataReader.GetFieldType(i); }
		public string GetFieldName(int index) {
			string name = aliases == null ? dataReader.GetName(index) : aliases[index];
			if(string.IsNullOrEmpty(name)) {
				string[] names = new string[dataReader.FieldCount];
				for(int i=0;i<names.Length;i++)
					names[i] = dataReader.GetName(i);
				int j = 0;
				do {
					name = string.Format("Column{0}", j + 1);
					j++;
				} while(names.Contains(name));
			}
			return name;
		}
		public bool Read() {
			return dataReader.Read();
		}
		public bool IsNull(int i) {
			return dataReader.IsDBNull(i);
		}
		public T GetValue<T>(int i) {
			if(typeof(T) == typeof(Int32)) return CastHelper.Cast<Int32, T>(dataReader.GetInt32(i));
			if(typeof(T) == typeof(String)) return CastHelper.Cast<String, T>(dataReader.GetString(i));
			if(typeof(T) == typeof(Decimal)) return CastHelper.Cast<Decimal, T>(dataReader.GetDecimal(i));
			if(typeof(T) == typeof(Double)) return CastHelper.Cast<Double, T>(dataReader.GetDouble(i));
			if(typeof(T) == typeof(float)) return CastHelper.Cast<float, T>(dataReader.GetFloat(i));
			if(typeof(T) == typeof(bool)) return CastHelper.Cast<bool, T>(dataReader.GetBoolean(i));
			if(typeof(T) == typeof(Byte)) return CastHelper.Cast<Byte, T>(dataReader.GetByte(i));
			if(typeof(T) == typeof(SByte)) return CastHelper.Cast<SByte, T>(Convert.ToSByte(dataReader.GetByte(i)));
			if(typeof(T) == typeof(Int16)) return CastHelper.Cast<Int16, T>(dataReader.GetInt16(i));
			if(typeof(T) == typeof(Int64)) return CastHelper.Cast<Int64, T>(dataReader.GetInt64(i));
			if(typeof(T) == typeof(UInt16)) return CastHelper.Cast<UInt16, T>(Convert.ToUInt16(dataReader.GetInt16(i)));
			if(typeof(T) == typeof(UInt32)) return CastHelper.Cast<UInt32, T>(Convert.ToUInt32(dataReader.GetInt32(i)));
			if(typeof(T) == typeof(UInt64)) return CastHelper.Cast<UInt64, T>(Convert.ToUInt64(dataReader.GetInt64(i)));
			if(typeof(T) == typeof(Char)) return CastHelper.Cast<Char, T>(dataReader.GetChar(i));
			if(typeof(T) == typeof(Byte[])) return CastHelper.Cast<Byte[], T>(dataReader.GetValue(i) as byte[]);
			if(typeof(T) == typeof(DateTime)) return CastHelper.Cast<DateTime, T>(dataReader.GetDateTime(i));
			if(typeof(T) == typeof(TimeSpan)) return CastHelper.Cast<TimeSpan, T>(GetTimeSpan(i));
			if(typeof(T) == typeof(Guid)) return CastHelper.Cast<Guid, T>(dataReader.GetGuid(i));
			return (T)dataReader.GetValue(i); 
		}
		#endregion
	}
}
