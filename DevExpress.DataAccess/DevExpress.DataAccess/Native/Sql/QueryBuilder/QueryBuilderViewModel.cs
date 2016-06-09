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
using System.Threading.Tasks;
using DevExpress.Data;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public class QueryBuilderViewModel : IQueryBuilderViewModel, INotifyPropertyChanged {
		const int maxLineLength = 50;
		readonly QueryBuilderModel model;
		readonly QueryGridItemData.List queryGridData;
		readonly SelectionItemData.List selectionData;
		readonly AvailableItemData.List availableData;
		string sqlText;
		bool allowEditSql;
		bool customSqlModified;
		IDBSchemaProvider schemaProvider;
		DBSchema dbSchema;
		SqlDataConnection connection;
		IAliasFormatter aliasFormatter;
		QueryExecutor queryExecutor;
		ICustomQueryValidator customQueryValidator;
		int updateCnt;
		Dictionary<string, HashSet<string>> columnsWithFKs;
		Dictionary<string, DBTable> dbTablesAndViews;
		public QueryBuilderViewModel(QueryBuilderModel model) {
			this.model = model;
			this.queryGridData = new QueryGridItemData.List(this);
			this.selectionData = new SelectionItemData.List(this);
			this.availableData = new AvailableItemData.List(this);
		}
		public event EventHandler BeforeUpdate;
		public event EventHandler AfterUpdate;
		public event EventHandler ShowWaitForm;
		public event EventHandler HideWaitForm;
		public event EventHandler<ErrorEventArgs> Error;
		public event PropertyChangedEventHandler SqlTextChanged;
		public event PropertyChangedEventHandler AllowEditSqlChanged;
		public event PropertyChangedEventHandler IsSqlSupportedChanged;
		public event PropertyChangedEventHandler AliasFormatterChanged;
		public event PropertyChangedEventHandler PropertyChanged;
		public QueryGridItemData.List QueryGrid { get { return this.queryGridData; } }
		public SelectionItemData.List Selection { get { return this.selectionData; } }
		public AvailableItemData.List Available { get { return this.availableData; } }
		public string SqlText { get { return this.sqlText; } set { SetSqlText(value, true); } }
		public bool AllowEditSql { get { return this.allowEditSql; } set { SetAllowEditSql(value, true); } }
		public bool IsSqlSupported { get { return this.connection != null && this.connection.DataStore is ISqlDataStore; } }
		public IAliasFormatter AliasFormatter {
			get {
				return this.aliasFormatter;
			}
			private set {
				if(this.aliasFormatter == value)
					return;
				this.aliasFormatter = value;
				RaiseAliasFormatterChanged();
			}
		}
		public bool IsUpdating {
			get { return this.updateCnt != 0; }
		}
		public bool CustomSqlModified { get { return this.customSqlModified; } }
		internal TableQuery ModelQuery { get { return this.model.TableQuery; } }
		internal Dictionary<string, HashSet<string>> ColumnsWithFKs { get { return this.columnsWithFKs; } }
		int MaxColumnAliasLength { get { return AliasFormatter == null ? 0 : AliasFormatter.MaxColumnAliasLength; } }
		int MaxTableAliasLength { get { return AliasFormatter == null ? 0 : AliasFormatter.MaxTableAliasLength; } }
		public void Initialize(IDBSchemaProvider schemaProvider, SqlDataConnection connection, IEnumerable<IParameter> sourceParameters, ICustomQueryValidator customQueryValidator) {
			this.connection = connection;
			this.schemaProvider = schemaProvider;
			this.customQueryValidator = customQueryValidator;
			this.dbSchema = schemaProvider.GetSchema(connection);
			this.dbTablesAndViews = this.dbSchema.Tables.Union(this.dbSchema.Views).ToDictionary(table => table.Name, StringComparer.Ordinal);
			this.queryExecutor = new QueryExecutor(connection, this.dbSchema, null, sourceParameters);
			AliasFormatter = connection.DataStore as IAliasFormatter;
			this.columnsWithFKs = this.dbTablesAndViews.Keys.ToDictionary(s => s, _ => new HashSet<string>());
			foreach(DBTable dbTable in this.dbSchema.Tables)
				foreach(DBForeignKey dbForeignKey in dbTable.ForeignKeys.Where(key => key.Columns.Count == 1))
					ColumnsWithFKs[dbTable.Name].Add(dbForeignKey.Columns[0]);
			BeginUpdate(true);
			try {
				RaiseIsSqlSupportedChanged();
				Selection.Initialize(ModelQuery.Tables, ModelQuery.Sorting, ModelQuery.Relations, ModelQuery.Grouping);
				QueryGrid.Initialize(ModelQuery.Tables, ModelQuery.Sorting, ModelQuery.Grouping);
				Available.Initialize(GetTables(), GetViews());
				if(connection.IsSqlDataStore)
					SetSqlText(
						this.model.SqlEditing
							? this.model.CustomSqlQuery.Sql
							: this.model.TableQuery.BuildSql(this.dbSchema, connection.SqlGeneratorFormatter), false);
				else if(this.model.SqlEditing)
					throw new InvalidOperationException();
				SetAllowEditSql(this.model.SqlEditing, false);
				UpdateConditions();
			}
			finally { EndUpdate(true); }
		}
		public void OnAvailableExpand(string table) {
			BeginUpdate(true);
			Exception error = null;
			try {
				AvailableItemData tableNode = Available[table];
				if(tableNode == null || tableNode.Children.Count != 1)
					return;
				AvailableItemData.ColumnData fakeNode = tableNode.Children[0];
				if(fakeNode.Name != null)
					return;
				foreach(DBColumn column in GetColumns(table))
					tableNode.Children.Add(new AvailableItemData.ColumnData(column.Name,
						column.Size <= 0
							? column.ColumnType.ToString()
							: string.Format("{0}({1})", column.ColumnType, column.Size)));
				tableNode.Children.Remove(fakeNode);
			}
			catch(Exception ex) { error = ex; }
			finally { EndUpdate(true); }
			if(error != null)
				RaiseError(error);
		}
		public void OnTableAddToSelection(string table, Func<IJoinEditorView> createViewFunc) {
			TableAddCore(table, alias => {
				string actualName = alias ?? table;
				DBTable dbTable = GetDbTable(table);
				var presets = GetRelationInfosFromFKs(table, dbTable, actualName);
				IEnumerable<RelationInfo> relationInfos = presets as RelationInfo[] ?? presets.ToArray();
				if(relationInfos.Count() != 1) {
					RelationInfoList jeModel;
					if(!LaunchJoinEditor(createViewFunc, table, alias, true, out jeModel, relationInfos))
						return null;
					relationInfos = jeModel;
				}
				return relationInfos;
			});
		}
		public void OnTableRemoveFromSelection(string tableName, Predicate<IEnumerable<string>> confirmMultipleRemovePredicate) {
			if(this.allowEditSql)
				return;
			BeginUpdate(false);
			try {
				RelationInfo[] downstream = GetRelationsHierarchy(tableName).ToArray();
				List<string> toRemove = downstream.Select(r => r.NestedTable).Distinct().ToList();
				toRemove.Insert(0, tableName);
				if(toRemove.Count > 1 && !confirmMultipleRemovePredicate(toRemove))
					return;
				foreach(RelationInfo relationInfo in downstream)
					this.model.TableQuery.Relations.Remove(relationInfo);
				for(int i = this.model.TableQuery.Relations.Count - 1; i >= 0; i--) {
					RelationInfo r = this.model.TableQuery.Relations[i];
					if(!string.Equals(r.NestedTable, tableName, StringComparison.Ordinal))
						continue;
					if(r.KeyColumns.Count == 1) {
						SelectionItemData selectionItemData = Selection.FindNode(r.ParentTable, r.KeyColumns[0].ParentKeyColumn);
						if(selectionItemData.ForeignKey == SelectionItemData.FKState.AlreadyJoined && AllRelationsRemoving(r.ParentTable, r.KeyColumns[0].ParentKeyColumn, toRemove))
							Selection.SetForeignKey(selectionItemData, SelectionItemData.FKState.CanBeJoined);
					}
					this.model.TableQuery.Relations.RemoveAt(i);
				}
				QueryGrid.RemoveAll(data => toRemove.Contains(data.Table, StringComparer.Ordinal));
				this.model.TableQuery.Tables.RemoveAll(t => toRemove.Contains(t.ActualName, StringComparer.Ordinal));
				foreach(string table in toRemove) {
					SelectionItemData tableNode = Selection.FindTableNode(table);
					int tableId = tableNode.Id;
					Selection.RemoveAll(data => data.Parent == tableId);
					Selection.Remove(tableNode);
				}
				UpdateShadowed();
				UpdateConditions();
			}
			finally { EndUpdate(false); }
		}
		public void OnEditFilter(Func<IFiltersView> createViewFunc) {
			if(this.allowEditSql)
				return;
			BeginUpdate(false);
			try {
				FiltersModel feModel;
				if(LaunchFilterEditor(createViewFunc, out feModel)) {
					ModelQuery.FilterString = feModel.FilterString;
					ModelQuery.Top = feModel.TopRecords;
					ModelQuery.Skip = feModel.SkipRecords;
					ModelQuery.GroupFilterString = feModel.GroupFilterString;
					ModelQuery.Parameters.Clear();
					ModelQuery.Parameters.AddRange(feModel.Parameters);
				}
			}
			finally { EndUpdate(false); }
		}
		public void OnJoinWithForeignKey(string table, string column) {
			TableInfo tableInfo = GetTableInfo(table);
			if(tableInfo == null)
				return;
			DBTable dbTable = GetDbTable(tableInfo.Name);
			if(dbTable == null)
				return;
			DBForeignKey foreignKey = dbTable.ForeignKeys.FirstOrDefault(fk => fk.Columns.Count == 1 && string.Equals(fk.Columns[0], column, StringComparison.Ordinal));
			if(foreignKey == null)
				return;
			TableAddCore(foreignKey.PrimaryKeyTable,
				alias =>
					new[] {
						new RelationInfo(table, alias ?? foreignKey.PrimaryKeyTable, JoinType.Inner,
							new RelationColumnInfo(column, foreignKey.PrimaryKeyTableKeyColumns[0]))
					});
		}
		public Task<SelectedDataEx> GetPreviewDataAsync(CancellationToken cancellationToken) {
			return Task.Factory.StartNew(() => {
				model.Query.Validate(customQueryValidator, connection.ConnectionParameters, dbSchema);
				return this.queryExecutor.Execute(this.model.Query, true, cancellationToken);
			}, cancellationToken);
		}
		public bool IsTopThousandApplicable() {  return this.model.Query.IsTopThousandApplicable(); }
		public void OnEditJoin(string table, Func<IJoinEditorView> createViewFunc) {
			if(this.allowEditSql)
				return;
			BeginUpdate(false);
			try {
				TableInfo tableInfo = GetTableInfo(table);
				RelationInfoList jeModel;
				if(!LaunchJoinEditor(createViewFunc, tableInfo.Name, tableInfo.Alias, false, out jeModel))
					return;
				var removingRelInfos = ModelQuery.Relations.Where(r => string.Equals(r.NestedTable, table, StringComparison.Ordinal)).ToArray();
				string[] removingTables = { table };
				foreach(RelationInfo rel in removingRelInfos) {
					if(rel.KeyColumns.Count == 1) {
						string parentTable = rel.ParentTable;
						string parentColumn = rel.KeyColumns[0].ParentKeyColumn;
						SelectionItemData node = Selection.FindNode(parentTable, parentColumn);
						if(node.ForeignKey == SelectionItemData.FKState.AlreadyJoined && AllRelationsRemoving(parentTable, parentColumn, removingTables))
							Selection.SetForeignKey(node, SelectionItemData.FKState.CanBeJoined);
					}
					ModelQuery.Relations.Remove(rel);
				}
				this.model.TableQuery.Relations.RemoveAll(
					info => string.Equals(info.NestedTable, table, StringComparison.Ordinal));
				this.model.TableQuery.Relations.AddRange(jeModel);
				foreach(RelationInfo relationInfo in jeModel.Where(info => info.KeyColumns.Count == 1)) {
					SelectionItemData selectionItemData = Selection.FindNode(relationInfo.ParentTable, relationInfo.KeyColumns[0].ParentKeyColumn);
					if(selectionItemData.ForeignKey == SelectionItemData.FKState.CanBeJoined && IsJoinedWithFK(table, relationInfo))
						Selection.SetForeignKey(selectionItemData, SelectionItemData.FKState.AlreadyJoined);
				}
				UpdateConditions();
			}
			finally { EndUpdate(false); }
		}
		public Dictionary<string, HashSet<string>> GetColumnsWithFKs() {
			return ColumnsWithFKs;
		}
		internal void BeginUpdate(bool waitForm) {
			if(this.updateCnt == 0)
				RaiseBeforeUpdate(waitForm);
			this.updateCnt++;
		}
		internal void EndUpdate(bool waitForm) {
			if(this.updateCnt == 1)
				UpdateSqlText();
			this.updateCnt--;
			if(this.updateCnt == 0)
				RaiseAfterUpdate(waitForm);
			else if(this.updateCnt < 0)
				throw new InvalidOperationException();
		}
		internal TableInfo GetTableInfo(string name) {
			return
				this.model.TableQuery.Tables.Single(
					tableInfo => string.Equals(tableInfo.ActualName, name, StringComparison.Ordinal));
		}
		internal ColumnInfo GetColumnInfo(string column) {
			return
				this.model.TableQuery.Tables.Select(
					ti =>
						ti.SelectedColumns.SingleOrDefault(
							ci => string.Equals(ci.ActualName, column, StringComparison.Ordinal)))
					.SingleOrDefault(ci => ci != null);
		}
		internal ColumnInfo GetColumnInfo(string table, string column) {
			return
				GetTableInfo(table)
					.SelectedColumns.SingleOrDefault(ci => string.Equals(ci.ActualName, column, StringComparison.Ordinal));
		}
		internal bool GetShadowed(string table) {
			if(this.allowEditSql)
				return true;
			if(this.model.TableQuery == null || this.model.TableQuery.Tables.Count == 0)
				return true;
			if(TableSelected(table))
				return true;
			if(GetDbTable(table).ForeignKeys.Any(fk => TableSelected(fk.PrimaryKeyTable)))
				return false;
			if(this.model.TableQuery.Tables.Select(t => t.Name).Distinct()
					.Any(selected => GetDbTable(selected).ForeignKeys
						.Any(fk => string.Equals(fk.PrimaryKeyTable, table, StringComparison.Ordinal))))
				return false;
			return true;
		}
		internal ConditionStringInfo GetConditionString(TableInfo table) {
			var relations = this.model.TableQuery.Relations.Where(r => string.Equals(r.NestedTable, table.ActualName, StringComparison.Ordinal)).ToArray();
			if(!relations.Any())
				return null;
			IEnumerable<ConditionStringInfo.ColumnInfo> columns =
				relations.SelectMany(
					rel =>
						rel.KeyColumns.Select(
							kc => new ConditionStringInfo.ColumnInfo(rel.ParentTable, kc.ParentKeyColumn)));
			string jointype = new ActionTypeData((ActionType)relations.First().JoinType).ToString();
			return new ConditionStringInfo(DataAccessStringId.QueryDesignerJoinExpressionPattern, jointype, columns);
		}
		internal IEnumerable<DBColumn> GetColumns(string table) {
			DBTable dbTable = GetDbTable(table);
			if(dbTable == null)
				return Enumerable.Empty<DBColumn>();
			if(dbTable.Columns.Count == 0)
				this.schemaProvider.LoadColumns(this.connection, dbTable);
			return dbTable.Columns;
		}
		internal string CreateColumnAlias(string nameBase) {
			return CreateAliasCore(nameBase, MaxColumnAliasLength,
				name =>
					!ModelQuery.Tables.Any(
						table =>
							table.SelectedColumns.Any(
								column => string.Equals(column.ActualName, name, StringComparison.Ordinal))));
		}
		internal void UpdateConditions() {
			foreach(SelectionItemData column in Selection) {
				bool isParent = column.Parent == -1;
				SelectionItemData table = isParent ? column : Selection.First(data => data.Id == column.Parent);
				var columnConditionString = isParent ? GetConditionString(GetTableInfo(table.Name)) : GetColumnConditionString(table.Name, column.Name, column.ForeignKey);
				Selection.SetCondition(column, columnConditionString);
			}
		}
		internal bool IsJoinedWithFK(RelationInfo relationInfo) { return IsJoinedWithFK(relationInfo.NestedTable, relationInfo); }
		ConditionStringInfo GetColumnConditionString(string table, string column, SelectionItemData.FKState state) {
			switch(state) {
				case SelectionItemData.FKState.CanBeJoined:
					TableInfo tInfo = GetTableInfo(table);
					DBTable dbTable = GetDbTable(tInfo.Name);
					DBForeignKey foreignKey =
						dbTable.ForeignKeys.FirstOrDefault(
							fk =>
								fk.Columns.Count == 1 && string.Equals(fk.Columns[0], column, StringComparison.Ordinal));
					if(foreignKey == null)
						return null;
					return new ConditionStringInfo(DataAccessStringId.QueryBuilderCanJoin,
						new ConditionStringInfo.ColumnInfo(foreignKey.PrimaryKeyTable, foreignKey.PrimaryKeyTableKeyColumns[0]));
				case SelectionItemData.FKState.AlreadyJoined:
					return new ConditionStringInfo(DataAccessStringId.QueryBuilderJoinedOn,
						ModelQuery.Relations.Where(
							info =>
								info.KeyColumns.Count == 1 &&
								string.Equals(info.ParentTable, table, StringComparison.Ordinal) &&
								string.Equals(info.KeyColumns[0].ParentKeyColumn, column, StringComparison.Ordinal))
							.Select(
								info =>
									new ConditionStringInfo.ColumnInfo(info.NestedTable,
										info.KeyColumns[0].NestedKeyColumn)));
				default:
					return null;
			}
		}
		void TableAddCore(string table, Func<string, IEnumerable<RelationInfo>> getRelationsFunc) {
			if(this.allowEditSql)
				return;
			BeginUpdate(false);
			try {
				string alias = ModelQuery.Tables.Any(t => string.Equals(t.Name, table, StringComparison.Ordinal))
					? CreateTableAlias(table)
					: null;
				string actualName = alias ?? table;
				if(this.model.TableQuery.Tables.Count > 0) {
					EnsureDBColumnsLoaded(table);
					IEnumerable<RelationInfo> relationInfos = getRelationsFunc(alias);
					if(relationInfos == null)
						return;
					RelationInfo[] relations = relationInfos as RelationInfo[] ?? relationInfos.ToArray();
					this.model.TableQuery.Relations.AddRange(relations);
					foreach(RelationInfo relationInfo in relations.Where(info => info.KeyColumns.Count == 1)) {
						SelectionItemData selectionItemData = Selection.FindNode(relationInfo.ParentTable, relationInfo.KeyColumns[0].ParentKeyColumn);
						if(selectionItemData.ForeignKey == SelectionItemData.FKState.CanBeJoined && IsJoinedWithFK(table, relationInfo))
							Selection.SetForeignKey(selectionItemData, SelectionItemData.FKState.AlreadyJoined);
					}
				}
				TableInfo tableInfo = new TableInfo(table) { Alias = alias };
				this.model.TableQuery.Tables.Add(tableInfo);
				ConditionStringInfo condition = GetConditionString(tableInfo);
				int tableId = Selection.AddNew(actualName, condition, false).Id;
				foreach(string column in GetColumns(table).Select(column => column.Name)) {
					SelectionItemData.FKState foreignKey = ColumnsWithFKs[table].Contains(column)
						? SelectionItemData.FKState.CanBeJoined
						: SelectionItemData.FKState.NoFK;
					Selection.AddNew(tableId, column, null, false, false, false, false, false, foreignKey);
				}
				UpdateShadowed();
				UpdateConditions();
			}
			finally { EndUpdate(false); }
		}
		void EnsureDBColumnsLoaded(string table) {
			RaiseShowWaitForm();
			try { GetColumns(table); }
			finally { RaiseHideWaitForm(); }
		}
		bool IsJoinedWithFK(string table, RelationInfo relationInfo) {
			DBTable dbTable = GetDbTable(GetTableInfo(relationInfo.ParentTable).Name);
			return dbTable.ForeignKeys.Any(fk => IsJoinedWithFK(table, relationInfo, fk));
		}
		static bool IsJoinedWithFK(string table, RelationInfo relationInfo, DBForeignKey fk) {
			return 
				fk.Columns.Count == 1 &&
				string.Equals(fk.Columns[0], relationInfo.KeyColumns[0].ParentKeyColumn, StringComparison.Ordinal) &&
				string.Equals(fk.PrimaryKeyTable, table, StringComparison.Ordinal) &&
				string.Equals(fk.PrimaryKeyTableKeyColumns[0], relationInfo.KeyColumns[0].NestedKeyColumn, StringComparison.Ordinal);
		}
		IEnumerable<RelationInfo> GetRelationInfosFromFKs(string table, DBTable dbTable, string actualName) {
			IEnumerable<DBForeignKey> foreignKeysFrom =
				dbTable.ForeignKeys.Where(
					fk => this.model.TableQuery.Tables.Any(t => string.Equals(t.Name, fk.PrimaryKeyTable, StringComparison.Ordinal)));
			foreach(DBForeignKey fk in foreignKeysFrom) {
				DBForeignKey foreignKey = fk; 
				yield return
					new RelationInfo(fk.PrimaryKeyTable, actualName, JoinType.Inner,
						fk.Columns.Cast<string>()
							.Select((nestedCol, i) => new RelationColumnInfo(foreignKey.PrimaryKeyTableKeyColumns[i], nestedCol)))
					;
			}
			HashSet<string> toSkip = new HashSet<string>(
				NotUnique(ModelQuery.Tables.Select(t => t.Name)),
				StringComparer.Ordinal) { table };
			foreach(TableInfo ti in this.model.TableQuery.Tables) {
				if(toSkip.Contains(ti.Name))
					continue;
				foreach(DBForeignKey fk in GetDbTable(ti.Name).ForeignKeys) {
					if(!string.Equals(fk.PrimaryKeyTable, table, StringComparison.Ordinal))
						continue;
					DBForeignKey foreignKey = fk; 
					yield return
						new RelationInfo(ti.ActualName, actualName, JoinType.Inner,
							fk.Columns.Cast<string>()
								.Select((col, i) => new RelationColumnInfo(col, foreignKey.PrimaryKeyTableKeyColumns[i])))
						;
				}
			}
		}
		IEnumerable<string> NotUnique(IEnumerable<string> source) {
			HashSet<string> set = new HashSet<string>(StringComparer.Ordinal);
			return source.Where(item => !set.Add(item));
		}
		void UpdateSqlText() {
			string sql;
			if(!IsSqlSupported)
				sql = string.Empty;
			else if(this.model.SqlEditing)
				sql = this.model.CustomSqlQuery.Sql;
			else {
				sql = this.model.TableQuery.BuildSql(this.dbSchema, this.connection.SqlGeneratorFormatter);
				sql = AutoSqlWrapHelper.AutoSqlTextWrap(sql, maxLineLength);
			}
			SetSqlText(sql, false);
			RaiseSqlTextChanged();
		}
		void SetSqlText(string value, bool update) {
			if(string.Equals(this.sqlText, value, StringComparison.Ordinal))
				return;
			BeginUpdate(false);
			try {
				if(update) {
					if(!this.model.SqlEditing)
						return;
					this.model.CustomSqlQuery.Sql = value.Trim();
					this.customSqlModified = true;
				}
				this.sqlText = value;
			}
			finally {
				EndUpdate(false);
				RaisePropertyChanged("SqlText");
				RaisePropertyChanged("CustomSqlModified");
			}
		}
		void SetAllowEditSql(bool value, bool update) {
			if(this.allowEditSql == value)
				return;
			if(value && !IsSqlSupported) {
				RaiseAllowEditSqlChanged();
				return;
			}
			BeginUpdate(false);
			try {
				this.allowEditSql = value;
				this.customSqlModified = !update;
				if(update) {
					this.model.SqlEditing = value;
					if(value) {
						string sql = this.model.TableQuery.BuildSql(this.dbSchema, this.connection.SqlGeneratorFormatter);
						sql = AutoSqlWrapHelper.AutoSqlTextWrap(sql, maxLineLength);
						if(this.model.CustomSqlQuery == null)
							this.model.CustomSqlQuery = new CustomSqlQuery(this.model.TableQuery.Name, sql);
						else
							this.model.CustomSqlQuery.Sql = sql;
					}
					else {
						if(this.model.TableQuery == null)
							this.model.TableQuery = new TableQuery(this.model.CustomSqlQuery.Name);
					}
					UpdateShadowed();
				}
				RaiseAllowEditSqlChanged();
			}
			finally {
				EndUpdate(false);
				RaisePropertyChanged("AllowEditSql");
				RaisePropertyChanged("CustomSqlModified");
			}
		}
		string CreateTableAlias(string nameBase) {
			return CreateAliasCore(nameBase, MaxTableAliasLength,
				name => !ModelQuery.Tables.Any(table => string.Equals(table.ActualName, name, StringComparison.Ordinal)));
		}
		string CreateAliasCore(string nameBase, int maxLen, Predicate<string> isOk) {
			if(isOk(nameBase))
				return nameBase;
			int index = 0;
			for(;;) {
				string result = index != 0 ? string.Format("{0}_{1}", nameBase, index) : nameBase;
				index++;
				if(maxLen > 0 && result.Length > maxLen) {
					nameBase = nameBase.Substring(0, nameBase.Length - 1);
					index = 1;
					continue;
				}
				if(isOk(result))
					return result;
			}
		}
		void RaiseBeforeUpdate(bool waitForm) {
			if(BeforeUpdate != null)
				BeforeUpdate(this, EventArgs.Empty);
			if(waitForm)
				RaiseShowWaitForm();
		}
		void RaiseShowWaitForm() {
			if(ShowWaitForm != null)
				ShowWaitForm(this, EventArgs.Empty);
		}
		void RaiseAfterUpdate(bool waitForm) {
			if(waitForm)
				RaiseHideWaitForm();
			if(AfterUpdate != null)
				AfterUpdate(this, EventArgs.Empty);
		}
		void RaiseHideWaitForm() {
			if(HideWaitForm != null)
				HideWaitForm(this, EventArgs.Empty);
		}
		void RaiseError(Exception ex) {
			if(Error != null)
				Error(this, new ErrorEventArgs(ex));
		}
		void RaiseSqlTextChanged() {
			if(SqlTextChanged != null)
				SqlTextChanged(this, new PropertyChangedEventArgs("SqlText"));
		}
		void RaiseAllowEditSqlChanged() {
			if(AllowEditSqlChanged != null)
				AllowEditSqlChanged(this, new PropertyChangedEventArgs("AllowEditSql"));
		}
		void RaiseIsSqlSupportedChanged() {
			if(IsSqlSupportedChanged != null)
				IsSqlSupportedChanged(this, new PropertyChangedEventArgs("IsSqlPropertyChanged"));
		}
		void RaiseAliasFormatterChanged() {
			if(AliasFormatterChanged != null) {
				AliasFormatterChanged(this, new PropertyChangedEventArgs("AliasFormatter"));
			}
			RaisePropertyChanged("AliasFormatter");
		}
		void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		IEnumerable<string> GetTables() { return this.dbSchema.Tables.Select(table => table.Name).OrderBy(s => s); }
		IEnumerable<string> GetViews() { return this.dbSchema.Views.Select(view => view.Name).OrderBy(s => s); }
		DBTable GetDbTable(string tableName) {
			return this.dbTablesAndViews[tableName];
		}
		bool TableSelected(string table) {
			return this.model.TableQuery.Tables.Any(t => string.Equals(t.Name, table, StringComparison.Ordinal));
		}
		void UpdateShadowed() {
			foreach(AvailableItemData table in Available) {
				bool value = GetShadowed(table.Name);
				if(table.Shadowed != value)
					Available.SetShadowed(table, value);
			}
		}
		bool LaunchJoinEditor(Func<IJoinEditorView> createViewFunc, string tableName, string tableAlias, bool newTable, out RelationInfoList jeModel) {
			return LaunchJoinEditor(createViewFunc, tableName, tableAlias, newTable, out jeModel, Enumerable.Empty<RelationInfo>());
		}
		bool LaunchJoinEditor(Func<IJoinEditorView> createViewFunc, string tableName, string tableAlias, bool newTable, out RelationInfoList jeModel, IEnumerable<RelationInfo> presets) {
			EnsureDBColumnsLoaded(tableName);
			string tableActualName = tableAlias ?? tableName;
			IJoinEditorView jeView = createViewFunc();
			jeModel = new RelationInfoList(this.model.TableQuery.Relations.Where(info => string.Equals(info.NestedTable, tableActualName, StringComparison.Ordinal)).Union(presets));
			IEnumerable<TableInfo> tableInfos = this.model.TableQuery.Tables;
			if(newTable)
				tableInfos = tableInfos.Union(new[] { new TableInfo(tableName) { Alias = tableAlias } });
			JoinEditorPresenter jePresenter = new JoinEditorPresenter(jeModel, jeView, this.dbSchema,
				tableInfos.ToDictionary(table => table.ActualName, table => table.Name),
				tableActualName);
			jePresenter.InitView();
			return jePresenter.Do();
		}
		bool LaunchFilterEditor(Func<IFiltersView> createViewFunc, out FiltersModel feModel) {
			IDictionary<string, DBTable> filterDbTables = ModelQuery.Tables.ToDictionary(table => table.ActualName, table => GetDbTable(table.Name));
			Dictionary<string, DBTable> groupFilterDbTables = GroupFilterPatcher.GetVirtualSchema(ModelQuery, dbSchema).ToDictionary(t => t.Name);
			string groupFilterString = GroupFilterPatcher.PrependTableNames(ModelQuery.GroupFilterString, ModelQuery);
			if (!LaunchFilterEditorCore(createViewFunc, filterDbTables, groupFilterDbTables, ModelQuery.FilterString, groupFilterString, out feModel))
				return false;
			return true;
		}
		bool LaunchFilterEditorCore(Func<IFiltersView> createViewFunc, IDictionary<string, DBTable> dbTables, Dictionary<string, DBTable> groupDbTables, string filterString, string groupFilterString, out FiltersModel feModel) {
			feModel = new FiltersModel(filterString, groupFilterString, ModelQuery.Top, ModelQuery.Skip, ModelQuery.Parameters);
			IFiltersView feView = createViewFunc();
			FiltersPresenter presenter = new FiltersPresenter(feModel, feView, ModelQuery, dbTables, groupDbTables);
			presenter.InitView();
			return presenter.Do();
		}
		IEnumerable<RelationInfo> GetRelationsHierarchy(string table) {
			foreach(RelationInfo curve in this.model.TableQuery.Relations.Where(r => string.Equals(r.ParentTable, table, StringComparison.Ordinal))) {
				yield return curve;
				foreach(RelationInfo info in GetRelationsHierarchy(curve.NestedTable))
					yield return info;
			}
		}
		bool AllRelationsRemoving(string table, string column, ICollection<string> removingTables) {
			return ModelQuery.Relations.Where(
				rel => rel.KeyColumns.Count == 1 &&
					   string.Equals(rel.ParentTable, table, StringComparison.Ordinal) &&
					   string.Equals(rel.KeyColumns[0].ParentKeyColumn, column))
				.All(rel => removingTables.Contains(rel.NestedTable));
		}
	}
}
