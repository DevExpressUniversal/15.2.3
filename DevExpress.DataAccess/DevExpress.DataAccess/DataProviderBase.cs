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
using System.Threading;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Sql;
using DevExpress.Services.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.DB.Exceptions;
using System.ComponentModel.Design;
namespace DevExpress.DataAccess {
	[Obsolete("The DataProviderBase class is obsolete now. Use the DashboardSqlDataSource class instead")]
	public class DataProviderBase : DisposableObject, IDataProvider, IServiceContainer, IParametersOwner {
		#region Inner classes
		public class SelectSqlGeneratorWithAlias : SelectSqlGenerator {
			readonly DataSelection dataSelection;
			public SelectSqlGeneratorWithAlias(DataSelection dataSelection, ISqlGeneratorFormatter formatter)
				: base(formatter) {
				this.dataSelection = dataSelection;
			}
			public override string GetNextParameterName(OperandValue parameter) {
				OperandParameter operandParameter = parameter as OperandParameter;
				return base.GetNextParameterName(!ReferenceEquals(operandParameter, null) ? new OperandParameter(operandParameter.ParameterName, new object()) : parameter);
			}
			protected override string PatchProperty(CriteriaOperator propertyOperator, string propertyString) {
				QueryOperand operand = propertyOperator as QueryOperand;
				if(ReferenceEquals(operand, null))
					return propertyString;
				DataColumn column = dataSelection.Operands[operand];
				string alias = column.Alias;
				if(String.IsNullOrEmpty(alias))
					return propertyString;
				alias = this.formatter.FormatColumn(alias);
				return string.Format("{0} as {1}", propertyString, alias);
			}
		}
		#endregion
		internal const int TopSelectedRecordsForPreview = 1000;
		internal const string XmlDataConnection = "DataConnection";
		const string xmlSql = "Sql";
		protected static DataView CreateSqlView(SelectedData selectedData) {
			DataView view = new DataView(selectedData.ResultSet[1].Rows);
			foreach(SelectStatementResultRow schemaRow in selectedData.ResultSet[0].Rows) {
				string name = (string)schemaRow.Values[0];
				Type type = DBColumn.GetType((DBColumnType)Enum.Parse(typeof(DBColumnType), (string)schemaRow.Values[2]));
				if(String.IsNullOrEmpty(name)) {
					int index = 1;
					do {
						name = string.Format(DataAccessLocalizer.GetString(DataAccessStringId.EmptyColumnAliasPattern), index);
						index++;
					}
					while(view.ContainsColumnName(name));
				}
				view.AddColumn(name, type);
			}
			return view;
		}
		readonly Locker locker = new Locker();
		readonly ServiceManager serviceManger = new ServiceManager();
		SqlDataConnection dataConnection;
		Thread executeThread;
		DBSchema dbSchema = new DBSchema();
		DataSelection dataSelection;
		string storedProcedureName;
		object data;
		object dataPreview;
		bool isCustomSql;
		string sql;
		bool locked;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SqlDataConnection DataConnection { get { return dataConnection; } }
		[Browsable(false)]
		public string SqlQuery {
			get { return sql; }
			set {
				SetSqlQuery(value, true);				
			}
		}
		internal DBTable[] Tables { get { return dbSchema.Tables; } }
		internal DBTable[] Views { get { return dbSchema.Views; } }
		internal DBStoredProcedure[] StoredProcedures { get { return dbSchema.StoredProcedures; } }
		internal DBSchema DBSchema { get { return dbSchema; } set { SetDBSchemaInternal(value, null); } }
		internal virtual object Data { get { return data; } set { data = value; } }
		internal virtual object DataSchema { get { return FDataSchema; } set { FDataSchema = value; } }
		internal bool IsSqlDataProvider { get { return dataConnection == null || dataConnection.DataStore == null || dataConnection.IsSqlDataStore; } }
		internal DataSelection DataSelection { get { return dataSelection; } set { SetSelection(value, true); } }
		internal string StoredProcedureName {
			get { return storedProcedureName; }
			set {
				if(storedProcedureName != value) {
					storedProcedureName = value;
					OnChanged();
				}
			}
		}
		internal bool ContainsSelectStatement { get { return DataSelection.CreateSelectStatement() != null; } }		
		internal bool IsCustomSql {
			get { return isCustomSql; }
			set {
				isCustomSql = value;
				ResetData();
				if(!isCustomSql && !dataSelection.IsEmpty) {
					OnChanged();
					if(IsSqlDataProvider)
						sql = CreateAutoSql();
				}
			}
		}
		internal bool IsEmpty {
			get { return IsCustomSql ? string.IsNullOrEmpty(SqlQuery) : DataSelection.IsEmpty && string.IsNullOrEmpty(StoredProcedureName); }
		}
		internal List<IParameter> Parameters { get; private set; }
		protected object FDataSchema { get; set; }
		protected internal virtual bool HasSelectedColumns { get { return DataSelection.SelectedColumns.Count > 0; } }
		protected internal virtual bool ShouldAddTableWithoutReferences { get { return false; } }
		DataConnectionBase IDataProvider.Connection { get { return dataConnection; } set { SetDataConnection((SqlDataConnection)value); } }
		internal event EventHandler<DataSchemaChangedEventArgs> DataSchemaChanged;
		internal event EventHandler DataSelectionChanged;
		internal event EventHandler<TablesRemovingEventArgs> TablesRemoving;
		internal event EventHandler Changed;
		internal DataProviderBase(SqlDataConnection dataConnection)
			: this() {
			SetDataConnection(dataConnection);
		}
		protected internal DataProviderBase() {
			Parameters = new List<IParameter>();
			dataSelection = new DataSelection(this);
		}
#if DEBUGTEST
		internal DataProviderBase(object data, object dataPreview) {
			this.data = FDataSchema = data;
			this.dataPreview = dataPreview;
		}
#endif   
		void SetDataConnection(SqlDataConnection value) {
			if (value != null && value != dataConnection) {
				IReferenceCountObject refCountObject;
				if (dataConnection != null) {
					refCountObject = dataConnection as IReferenceCountObject;
					if (refCountObject != null)
						refCountObject.RemoveReference();
				}
				dataConnection = value;
				refCountObject = dataConnection as IReferenceCountObject;
				if (refCountObject != null)
					refCountObject.AddReference();
				OnDataConnectionSet();
			}
		}
		bool RaiseTablesRemoving(IEnumerable<DataTable> tablesToRemove) {
			if(TablesRemoving != null) {
				TablesRemovingEventArgs eventArgs = new TablesRemovingEventArgs(tablesToRemove);
				TablesRemoving(this, eventArgs);
				return !eventArgs.Cancel;
			}
			return true;
		}
		void ApplyCustomFilter(string tableName, SelectStatement selectStatement) {
			CriteriaOperator filterOperator = RaiseCustomFilter(tableName);
			if(ReferenceEquals(filterOperator, null))
				return;
			selectStatement.Condition = filterOperator;
		}
		protected virtual CriteriaOperator RaiseCustomFilter(string tableName) {
			IDataLoaderHelper helper = ((IServiceProvider)this).GetService(typeof(IDataLoaderHelper)) as IDataLoaderHelper;
			if(helper == null)
				return null;
			return helper.RaiseCustomFilter(tableName);
		}
	   string CreateAutoSql() {
			SelectStatement selectStatement = GetSelectStatementForAutoSql();
			return CreateAutoSql(selectStatement);
		}
		SelectedData GetData() {
			if(DataConnection == null || DataConnection.CommandChannel == null)
				return null;
			if(string.IsNullOrEmpty(storedProcedureName))
				return ExecuteSelect(SqlQuery);
			SelectStatementResult schema = new SelectStatementResult();
			ISupportStoredProc dataStore = DataConnection.DataStore as ISupportStoredProc;
			OperandValue[] arguments = Parameters.Select(p => new OperandValue(p.Value)).ToArray();
			SelectedData selectedData = CommandChannelHelper.ExecuteSproc(DataConnection.CommandChannel, StoredProcedureName, arguments);
			if(dataStore == null)
				throw new NotSupportedException("The specified data provider does not support stored procedures.");
			DBStoredProcedureResultSet result = dataStore.GetStoredProcedureTableSchema(StoredProcedureName);
			schema.Rows = result.Columns.Select(c => new SelectStatementResultRow(new object[] {c.Name, string.Empty, c.Type.ToString()})).ToArray();
			return new SelectedData(schema, selectedData.ResultSet[0]);
		}
		void SetDBSchemaInternal(DBSchema value, List<DataLoaderError> errors) { 
			dbSchema = value;
			locker.Lock();
			try {
				LoadDataObjects(errors);
			} finally {
				locker.Unlock();
			}
			OnChanged();
		}
		protected List<KeyValuePair<string, OperandValue>> GetCustomSqlCommandParameters() {
			List<KeyValuePair<string, OperandValue>> result = new List<KeyValuePair<string, OperandValue>>();
			ICommandParameterNameProvider commandParameterNameProvider = DataConnection.DataStore as ICommandParameterNameProvider;
			foreach(IParameter parameter in Parameters) {
				if(commandParameterNameProvider != null)
					foreach(string name in commandParameterNameProvider.GetCommandParameterNames(parameter.Name)) {
						result.Add(new KeyValuePair<string, OperandValue>(name, new OperandValue(parameter.Value)));
					}
				else {
					result.Add(new KeyValuePair<string, OperandValue>(parameter.Name, new OperandValue(parameter.Value)));
				}
			}
			return result;
		}
		protected SelectedData ExecuteSelect(string query) {
			List<KeyValuePair<string, OperandValue>> commandParameters = GetCustomSqlCommandParameters();
			if(commandParameters.Count == 0)
				return CommandChannelHelper.ExecuteQueryWithMetadata(DataConnection.CommandChannel, query);
			return CommandChannelHelper.ExecuteQueryWithMetadataWithParams(DataConnection.CommandChannel, query, new QueryParameterCollection(commandParameters.Select(p => p.Value).ToArray()), commandParameters.Select(p => p.Key).ToArray());
		}
		void OnChanged() {
			if(!locked) {
				if(Changed != null)
					Changed(this, EventArgs.Empty);
				SchemaChanged();
			}
		}
		internal void ResetData() {
			dataPreview = null;
		}
		internal string CreateAutoSql(SelectStatement selectStatement) {
			string autoSql = "";
			if(selectStatement != null && DataConnection.SqlGeneratorFormatter != null) {
				SelectSqlGeneratorWithAlias sqlGenerator = new SelectSqlGeneratorWithAlias(DataSelection, DataConnection.SqlGeneratorFormatter);
				Query query = sqlGenerator.GenerateSql(selectStatement);
				autoSql = query.Sql;
			}
			return autoSql;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(serviceManger != null) {
					serviceManger.Dispose();
				}
				if (dataConnection != null) {
					IReferenceCountObject refCountObject = dataConnection as IReferenceCountObject;					
					if(refCountObject!=null)
						refCountObject.RemoveReference();
				}
			}
			base.Dispose(disposing);
		}
		protected SelectedData SelectData(string tableName, SelectStatement selectStatement) {
			if(DataConnection.DataStore == null)
				return null;
			ApplyCustomFilter(tableName, selectStatement);
			return DataConnection.DataStore.SelectData(selectStatement);
		}
		protected virtual SelectStatement GetSelectStatementForAutoSql() {
			return DataSelection.CreateSelectStatement();
		}
		protected virtual void SchemaChanged() {
		}
		protected bool Contains(DBTable item) {
			foreach(DBTable table in Tables)
				if(table.Equals(item))
					return true;
			foreach(DBTable view in Views)
				if(view.Equals(item))
					return true;
			return false;
		}
		protected void ExecuteCore() {
			if(IsSqlDataProvider && (IsCustomSql || !string.IsNullOrEmpty(storedProcedureName))) {
				SelectedData selectedData = GetData();
				if(selectedData != null)
					DataSchema = Data = CreateSqlView(selectedData);
			} else
				ExecuteSelection();
		}
		protected virtual void OnDataConnectionSet() {
		} 
		protected virtual void ExecuteSelection() {
		}
		protected void ExecutePreviewCore() {
			if(IsSqlDataProvider && (IsCustomSql || !string.IsNullOrEmpty(storedProcedureName))) {
				SelectedData selectedData = GetData();
				DataPreview = CreateSqlView(selectedData);
			} else
				ExecutePreviewSelection();
		}
		protected virtual void ExecutePreviewSelection() {
		}
		protected virtual void OnDataSelectionChangedInternal() {
			ResetData();
			OnChanged();
			if(IsSqlDataProvider)
				sql = CreateAutoSql();
		}
		protected internal virtual DataTable AddTableToSelection(DBTable table, DataReferences dataReferences) {
			Guard.ArgumentNotNull(table, "table");
			Guard.ArgumentNotNull(dataReferences, "dataReferences");
			if(!Contains(table))
				throw new DataSourceWizardException(string.Format("The data provider does not contain the '{0}' table", table.Name));
			if(dataReferences.Count == 0)
				throw new DataSourceWizardException("The reference list is empty");
			DataTable dataTable = dataSelection.AddTable(table, dataReferences);
			OnDataSelectionChanged();
			return dataTable;
		}
		protected internal virtual IEnumerable<DataTable> GetTablesForRelation(string currentTable) {
			return this.DataSelection.TakeWhile(table => table.Alias != currentTable);
		}
		protected internal virtual bool AllowEditSQL {
			get { return true; }
		}
		protected internal virtual bool AllowPreviewResults {
			get { return true; }
		}
		protected internal virtual Type ActionType {
			get { return typeof(JoinType); }
		}
		internal void BeginUpdate() {
			locked = true;
		}
		internal void EndUpdate() {
			locked = false;
			OnChanged();
		}
		internal SqlQuery GetQuery() {
			string queryName = !DataSelection.IsEmpty ? DataSelection[0].Alias ?? DataSelection[0].TableName : null;
			return GetQuery(DataSelection, queryName);
		}
		internal SqlQuery GetQuery(IEnumerable<DataTable> tables, string queryName) {
			List<QueryParameter> parameters = Parameters.Select(p => new QueryParameter(p.Name, p.Type, p.Value)).ToList();
			if(!string.IsNullOrEmpty(StoredProcedureName)) {
				return new StoredProcQuery(StoredProcedureName, StoredProcedureName, parameters);
			}
			if(IsCustomSql) {
				CustomSqlQuery customSqlQuery = new CustomSqlQuery("CustomSqlQuery", SqlQuery);
				customSqlQuery.Parameters.AddRange(parameters);
				return customSqlQuery;
			}
			TableQuery query = new TableQuery();
			foreach(DataTable table in tables) {
				TableInfo queryTable = query.AddTable(table.TableName);
				if(!string.IsNullOrEmpty(table.Alias) && !string.Equals(table.Alias, queryTable.Name))
					queryTable.Alias = table.Alias;
				foreach(DataColumn column in table.SelectedColumns) {
					ColumnInfo queryColumn = queryTable.SelectColumn(column.ColumnName);
					if(!string.IsNullOrEmpty(column.Alias) && !string.Equals(column.Alias, queryColumn.Name))
						queryColumn.Alias = column.Alias;
				}
				IEnumerable<DataReference> joinReferences = table.References;
				if (table.References.ActionType != Native.Sql.ActionType.MasterDetailRelation) {
					JoinType joinType = table.References.ActionType == Native.Sql.ActionType.InnerJoin
						? JoinType.Inner
						: JoinType.LeftOuter;
					Dictionary<DataTable, RelationInfo> parents = new Dictionary<DataTable, RelationInfo>();
					foreach (DataReference reference in joinReferences) {
						DataTable parentTable = reference.ParentDataTable;
						RelationColumnInfo.ConditionType conditionType =
							(RelationColumnInfo.ConditionType) reference.OperatorType;
						if (!parents.ContainsKey(parentTable)) {
							string parentTableName = !string.IsNullOrEmpty(parentTable.Alias)
								? parentTable.Alias
								: parentTable.TableName;
							RelationInfo relation = query.AddRelation(parentTableName, queryTable.ActualName,
								reference.ParentKeyColumn, reference.KeyColumn);
							relation.JoinType = joinType;
							relation.KeyColumns[0].ConditionOperator = conditionType;
							parents.Add(parentTable, relation);
						}
						else
							parents[parentTable].KeyColumns.Add(new RelationColumnInfo(reference.ParentKeyColumn,
								reference.KeyColumn, conditionType));
					}
				}
			}
			var filter = DataSelection.Filters[queryName];
			if(filter != null)
				query.FilterString = filter.FilterString;
			if(!string.IsNullOrEmpty(queryName))
				query.Name = queryName;
			query.Parameters.AddRange(parameters);
			return query;
		}
		internal void SetSqlQuery(string newSql, bool switchToCustomSql) {
			if(newSql != sql) {
				sql = newSql;
				if(switchToCustomSql)
					IsCustomSql = true;
				else
					ResetData();
				OnChanged();
			}
		}
		internal void SetSqlQuery(string newSql) {
			SetSqlQuery(newSql, false);
		}
		internal void SetParameters(IEnumerable<IParameter> parameters) {
			if(object.ReferenceEquals(Parameters, parameters))
				return;
			Parameters.Clear();
			Parameters.AddRange(parameters);
			if(!isCustomSql)
				OnDataSelectionChanged();
		}
		internal IList<DataTable> RemoveDataTableFromSelection(DataTable dataTable) {
			Guard.ArgumentNotNull(dataTable, "dataTable");
			IList<DataTable> dataTablesToRemove = dataSelection.GetDataTablesToRemove(dataTable);
			if(RaiseTablesRemoving(dataTablesToRemove)) {
				dataSelection.RemoveDataTables(dataTablesToRemove);
				OnDataSelectionChanged();
				return dataTablesToRemove;
			}
			return null;
		}
		internal void SetSelection(DataSelection newSelection, bool refreshData) {
			if(dataSelection != newSelection) {
				dataSelection = newSelection;
				if(refreshData)
					OnDataSelectionChanged();
			}
		}
		internal DBTable FindTable(Predicate<DBTable> predicate) {
			foreach(DBTable table in Tables)
				if(predicate(table))
					return table;
			foreach(DBTable view in Views)
				if(predicate(view))
					return view;
			return null;
		}
		internal DBTable FindTable(string name) {
			return FindTable(table => table.Name.ToLowerInvariant() == name.ToLowerInvariant());
		}
		internal DBColumn FindColumn(string tableName, Predicate<DBColumn> predicate) {
			DBTable table = FindTable(tableName);
			if(table == null)
				return null;
			foreach(DBColumn column in table.Columns)
				if(predicate(column))
					return column;
			return null;
		}
		internal DBColumn FindColumn(string tableName, string columnName) {
			return FindColumn(tableName, column => column.Name.ToLowerInvariant() == columnName.ToLowerInvariant());
		}
		internal DataTable AddTableToSelectionByReference(DataColumn dataColumn) {
			Guard.ArgumentNotNull(dataColumn, "table");
			DataTable dataTable = dataSelection.AddTableByReference(dataColumn);
			OnDataSelectionChanged();
			return dataTable;
		}
		internal DataTable AddTableToSelection(DBTable table) {
			Guard.ArgumentNotNull(table, "table");
			if(!Contains(table))
				throw new DataSourceWizardException(string.Format("The data provider does not contain the '{0}' table", table.Name));
			DataTable dataTable = dataSelection.AddTable(table);
			OnDataSelectionChanged();
			return dataTable;
		}
		internal string GetFilterString(string filterTableName) {
			if(dataSelection.Filters.Count == 0)
				return null;
			if(!dataSelection.Filters.Contains(filterTableName))
				return null;
			return dataSelection.Filters[filterTableName].FilterString;
		}
		internal void SetFilterString(string tableName, string filterExpression) {
			SetFilterString(tableName, filterExpression, true);
		}
		internal void SetFilterString(string tableName, string filterExpression, bool raiseChanged) {
			if(dataSelection.Filters[tableName] != null)
				dataSelection.Filters[tableName].FilterString = filterExpression;
			else
				dataSelection.Filters.Add(tableName, filterExpression);
			if(!IsCustomSql && raiseChanged && ((IDataProvider)this).IsConnected)
				OnDataSelectionChanged();
		}
		internal void OnDataSelectionChanged() {
			if(!locker.IsLocked) {
				OnDataSelectionChangedInternal();
				if(DataSelectionChanged != null)
					DataSelectionChanged(this, EventArgs.Empty);
			}
		}
		internal void OnColumnOrTableAliasChanged() {
			if(!locker.IsLocked) {
				OnChanged();
				if(IsSqlDataProvider)
					sql = CreateAutoSql();
			}
		}
		internal void RaiseTableOrColumnAliasChanged(IList<AliasChangeInfo> aliasChanges) {
			if(DataSchemaChanged != null)
				DataSchemaChanged(this, new DataSchemaChangedEventArgs(aliasChanges));
		}
		internal object DataPreview {
			get { return dataPreview; }
			set { dataPreview = value; }
		}
		#region IDataProvider
		object IDataProvider.Data { get { return Data; } }
		object IDataProvider.DataSchema { get { return DataSchema; } }
		bool IDataProvider.IsConnected { get { return dataConnection != null && dataConnection.IsConnected; } }
		event EventHandler<DataSchemaChangedEventArgs> IDataProvider.DataSchemaChanged {
			add { DataSchemaChanged = (EventHandler<DataSchemaChangedEventArgs>)Delegate.Combine(DataSchemaChanged, value); }
			remove { DataSchemaChanged = (EventHandler<DataSchemaChangedEventArgs>)Delegate.Remove(DataSchemaChanged, value); }
		}
		void IDataProvider.SaveToXml(XElement element) {
			if(dataConnection != null)
				element.Add(new XAttribute(XmlDataConnection, dataConnection.Name));
			if(dataSelection.Count > 0)
				element.Add(dataSelection.SaveToXml());
			if(IsCustomSql && !string.IsNullOrEmpty(sql))
				element.Add(new XAttribute(xmlSql, sql));
		}
		void IDataProvider.LoadFromXml(XElement element) {
			XElement selectionElement = element.Element(DataSelection.XmlSelection);
			if(selectionElement != null)
				dataSelection = new DataSelection(this, selectionElement);
			sql = XmlHelperBase.GetAttributeValue(element, xmlSql);
			isCustomSql = !String.IsNullOrEmpty(sql);
		}
		protected void LoadDataObjectsCore() {
			dataSelection.LoadDataObjects();
		}
		protected virtual void LoadDataObjects(List<DataLoaderError> errors) {
			LoadDataObjectsCore();
		}
		void IDataProvider.EndLoading(IDBSchemaProvider dbSchemaProvider) {
			((IDataProvider)this).EndLoading(dbSchemaProvider, null);
		} 
		void IDataProvider.EndLoading(IDBSchemaProvider dbSchemaProvider, List<DataLoaderError> errors) {
			if(DataConnection == null)
				return;
			SetDBSchemaInternal(DataConnection.GetDBSchema(dataSelection.Select(dataTable => dataTable.TableName).ToArray()), errors);
			if(IsSqlDataProvider && string.IsNullOrEmpty(sql) && DataConnection.IsConnected)
				sql = CreateAutoSql();
		}
		bool IDataProvider.CancelExecute() {
			if(executeThread == null)
				return false;
			executeThread.Abort();
			executeThread = null;
			return true;
		}
		#endregion
		#region IParametersOwner Members
		object IParametersOwner.DataSchema {
			get { return DataSchema; }
		}
		IEnumerable<IParameter> IParameterSupplierBase.GetIParameters() {
			return Parameters;
		}
		#endregion
		#region IServiceContainer Members
		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote) {
			serviceManger.AddService(serviceType, callback, promote);
		}
		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback) {
			serviceManger.AddService(serviceType, callback);
		}
		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
			serviceManger.AddService(serviceType, serviceInstance, promote);
		}
		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, object serviceInstance) {
			serviceManger.AddService(serviceType, serviceInstance);
		}
		void System.ComponentModel.Design.IServiceContainer.RemoveService(Type serviceType, bool promote) {
			serviceManger.RemoveService(serviceType, promote);
		}
		void System.ComponentModel.Design.IServiceContainer.RemoveService(Type serviceType) {
			serviceManger.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return serviceManger.GetService(serviceType);			
		}
		#endregion
	}
}
