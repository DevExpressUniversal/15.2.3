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
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Utils;
#if !CF && !SL
using DevExpress.Xpo.Logger;
#endif
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress.Xpo.DB.Helpers {
	class ColumnTypeResolver: CriteriaTypeResolverBase, IQueryCriteriaVisitor<CriteriaTypeResolverResult> {
		static readonly ColumnTypeResolver Instance = new ColumnTypeResolver();
		ColumnTypeResolver() { }
		CriteriaTypeResolverResult IQueryCriteriaVisitor<CriteriaTypeResolverResult>.Visit(QueryOperand theOperand) {
			return new CriteriaTypeResolverResult(DBColumn.GetType(theOperand.ColumnType));
		}
		CriteriaTypeResolverResult IQueryCriteriaVisitor<CriteriaTypeResolverResult>.Visit(QuerySubQueryContainer subQueryContainer) {
			switch(subQueryContainer.AggregateType) {
				case Aggregate.Exists:
					return new CriteriaTypeResolverResult(typeof(bool));
				case Aggregate.Count:
					return new CriteriaTypeResolverResult(typeof(int));
				case Aggregate.Avg: {
						var result = Process(subQueryContainer.AggregateProperty);
						if(result.Type == typeof(decimal)) {
							return result;
						}
						return new CriteriaTypeResolverResult(typeof(double));
					}
				default:
					return Process(subQueryContainer.AggregateProperty);
			}
		}
		ConnectionProviderSql provider;
		Type ResolveTypeInternal(CriteriaOperator criteria, ConnectionProviderSql provider) {
			this.provider = provider;
			return Process(criteria).Type;
		}
		static public Type ResolveType(CriteriaOperator criteria, ConnectionProviderSql provider) {
			return Instance.ResolveTypeInternal(criteria, provider);
		}
		protected override Type GetCustomFunctionType(string functionName, params Type[] operands) {
			ICustomFunctionOperator customFunction = null;
			if(provider != null) {
				customFunction = provider.GetCustomFunctionOperator(functionName);
			}
			if(customFunction == null) {
				return base.GetCustomFunctionType(functionName, operands);
			}
			return customFunction.ResultType(operands);
		}
	}
}
namespace DevExpress.Xpo.DB {
	using DevExpress.Xpo.DB.Helpers;
	using System.Text;
	using System.Collections.Generic;
	using System.Data.Common;
	using DevExpress.Data.Db;
	using DevExpress.Xpo.Logger;
	using System.Linq;
	using Compatibility.System.Collections.Specialized;
	public abstract class ProviderFactory {
		public const string UseIntegratedSecurityParamID = "useIntegratedSecurity",
			ServerParamID = "server", DatabaseParamID = "database", UserIDParamID = "userid",
			PasswordParamID = "password", ReadOnlyParamID = "read only";
		public abstract IDataStore CreateProvider(Dictionary<string, string> parameters, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect);
		public abstract IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect);
		public abstract IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption);
		public abstract bool HasUserName { get; }
		public abstract bool HasPassword { get; }
		public abstract bool HasIntegratedSecurity { get; }
		public abstract bool IsServerbased { get; }
		public abstract bool IsFilebased { get; }
		public abstract bool HasMultipleDatabases { get; }
		public abstract string ProviderKey { get; }
		public abstract string[] GetDatabases(string server, string userid, string password);
		public abstract string FileFilter { get; }
		public abstract bool MeanSchemaGeneration { get; }
		public virtual bool SupportStoredProcedures { get { return false; } }
		public abstract string GetConnectionString(Dictionary<string, string> parameters);
	}
	public abstract class ConnectionProviderSql : DataStoreBase, ISqlGeneratorFormatterEx, ISqlGeneratorFormatterSupportSkipTake, ISqlGeneratorFormatterSupportOuterApply, ISqlDataStore, IDataStoreSchemaExplorer, IDataStoreSchemaExplorerSp, ICommandChannel {
		protected ConnectionProviderSql(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(autoCreateOption) {
#if !DXPORTABLE
			PerformanceCounters.SqlDataStoreCount.Increment();
			PerformanceCounters.SqlDataStoreCreated.Increment();
#endif
			this.connection = connection;
			connectString = connection.ConnectionString;
			if(Connection.State != ConnectionState.Open)
				CreateDataBase();
			if(connection.State != ConnectionState.Open)
				connection.Open();
		}
		~ConnectionProviderSql() {
#if !DXPORTABLE
			PerformanceCounters.SqlDataStoreCount.Decrement();
			PerformanceCounters.SqlDataStoreFinalized.Increment();
#endif
		}
#if !DXPORTABLE
		public override ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.SqlDataStoreTotalRequests, PerformanceCounters.SqlDataStoreTotalQueue, PerformanceCounters.SqlDataStoreModifyRequests, PerformanceCounters.SqlDataStoreModifyQueue)) {
				PerformanceCounters.SqlDataStoreModifyStatements.Increment(dmlStatements.Length);
				for(int tryCount = 1; ; tryCount++) {
					try {
						return base.ModifyData(dmlStatements);
					} catch(Exception e) {
						if(!explicitTransaction && tryCount <= MaxDeadLockTryCount) {
							bool isDeadLock = false;
							Exception exception = e;
							while(exception != null && !isDeadLock) {
								isDeadLock = IsDeadLock(exception);
								exception = exception.InnerException;
							}
							if(isDeadLock) {
								if(tryCount > 1) {
									System.Threading.Thread.Sleep(Randomizer.Next(MaxDeadLockRetryDelayMilliseconds));
								}
								continue;
							}
						}
						throw;
					}
				}
			}
		}
		public override SelectedData SelectData(params SelectStatement[] selects) {
			using(IDisposable c = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.SqlDataStoreTotalRequests, PerformanceCounters.SqlDataStoreTotalQueue, PerformanceCounters.SqlDataStoreSelectRequests, PerformanceCounters.SqlDataStoreSelectQueue)) {
				PerformanceCounters.SqlDataStoreSelectQueries.Increment(selects.Length);
				return base.SelectData(selects);
			}
		}
		public override UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			using(IDisposable c1 = new PerformanceCounters.QueueLengthCounter(PerformanceCounters.SqlDataStoreTotalRequests, PerformanceCounters.SqlDataStoreTotalQueue, PerformanceCounters.SqlDataStoreSchemaUpdateRequests, PerformanceCounters.SqlDataStoreSchemaUpdateQueue)) {
				return base.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
			}
		}
#endif
		Random randomizer;
		Random Randomizer {
			get {
				if(randomizer == null) {
					randomizer = new Random(DateTime.UtcNow.GetHashCode());
				}
				return randomizer;
			}
		}
		bool explicitTransaction = false;
		IDbConnection connection;
		string connectString;
		public bool CanCreateDatabase { get { return AutoCreateOption == AutoCreateOption.DatabaseAndSchema; } }
		public bool CanCreateSchema { get { return AutoCreateOption == AutoCreateOption.SchemaOnly || CanCreateDatabase; } }
		public IDbConnection Connection {
			get {
				return connection;
			}
		}
		public string ConnectionString {
			get { return connectString; }
		}
		protected abstract void CreateDataBase();
		protected abstract IDbConnection CreateConnection();
		public override object SyncRoot { get { return Connection; } }
		Dictionary<string, ICustomFunctionOperatorFormattable> customFunctionsByName = new Dictionary<string, ICustomFunctionOperatorFormattable>(StringExtensions.ComparerInvariantCultureIgnoreCase);
		public void RegisterCustomFunctionOperators(ICollection<ICustomFunctionOperatorFormattable> customFunctions) {
			if(customFunctions == null)
				return;
			foreach(ICustomFunctionOperatorFormattable customFunction in customFunctions) {
				RegisterCustomFunctionOperator(customFunction);
			}
		}
		public void RegisterCustomFunctionOperator(ICustomFunctionOperatorFormattable customFunction) {
			if(customFunction == null)
				return;
			customFunctionsByName[customFunction.Name] = customFunction;
		}
		public bool UnregisterCustomFunctionOperator(ICustomFunctionOperatorFormattable customFunction) {
			if(customFunction == null)
				return false;
			return customFunctionsByName.Remove(customFunction.Name);
		}
		public bool UnregisterCustomFunctionOperator(string functionName) {
			if(functionName == null)
				return false;
			return customFunctionsByName.Remove(functionName);
		}
		public ICustomFunctionOperatorFormattable GetCustomFunctionOperator(string functionName) {
			ICustomFunctionOperatorFormattable customFunction;
			if(customFunctionsByName.TryGetValue(functionName, out customFunction)) {
				return customFunction;
			}
			return CriteriaOperator.CustomFunctionCount > 0 ? CriteriaOperator.GetCustomFunction(functionName) as ICustomFunctionOperatorFormattable : null;
		}
		public ICollection<ICustomFunctionOperatorFormattable> CustomFunctionOperators { get { return customFunctionsByName.Values; } }
		protected static bool IsSingleColumnPKColumn(DBTable table, DBColumn column) {
			if(!column.IsKey)
				return false;
			if(table == null)
				return true;
			if(table.PrimaryKey != null) {
				System.Diagnostics.Debug.Assert(table.PrimaryKey.Columns.Contains(column.Name));
				return table.PrimaryKey.Columns.Count == 1;
			} else {
				foreach(DBColumn c in table.Columns) {
					if(c.IsKey && c.Name != column.Name) {
						return false;
					}
				}
				return true;
			}
		}
		public static DBColumnType GetColumnType(DbType type, bool supressExceptionOnUnknown) {
			switch(type) {
				case DbType.Boolean:
					return DBColumnType.Boolean;
				case DbType.Byte:
					return DBColumnType.Byte;
				case DbType.SByte:
					return DBColumnType.SByte;
				case DbType.String:
				case DbType.StringFixedLength:
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					return DBColumnType.String;
				case DbType.Currency:
				case DbType.Decimal:
				case DbType.VarNumeric:
					return DBColumnType.Decimal;
				case DbType.Double:
					return DBColumnType.Double;
				case DbType.Single:
					return DBColumnType.Single;
				case DbType.Int32:
					return DBColumnType.Int32;
				case DbType.UInt32:
					return DBColumnType.UInt32;
				case DbType.Int16:
					return DBColumnType.Int16;
				case DbType.UInt16:
					return DBColumnType.UInt16;
				case DbType.Int64:
					return DBColumnType.Int64;
				case DbType.UInt64:
					return DBColumnType.UInt64;
				case DbType.DateTime:
#if !CF
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
#endif
				case DbType.Time:
				case DbType.Date:
					return DBColumnType.DateTime;
				case DbType.Binary:
					return DBColumnType.ByteArray;
				case DbType.Guid:
					return DBColumnType.Guid;
				case DbType.Object:
					if(!supressExceptionOnUnknown) {
						throw new InvalidOperationException(type.ToString());
					}
					return DBColumnType.Unknown;
				default:
					throw new InvalidOperationException(type.ToString());
			}
		}
		public static DbType GetDbType(DBColumnType type) {
			switch(type) {
				case DBColumnType.Boolean:
					return DbType.Boolean;
				case DBColumnType.Byte:
					return DbType.Byte;
				case DBColumnType.SByte:
					return DbType.SByte;
				case DBColumnType.String:
					return DbType.String;
				case DBColumnType.Decimal:
					return DbType.Decimal;
				case DBColumnType.Double:
					return DbType.Double;
				case DBColumnType.Single:
					return DbType.Single;
				case DBColumnType.Int32:
					return DbType.Int32;
				case DBColumnType.UInt32:
					return DbType.UInt32;
				case DBColumnType.Int16:
					return DbType.Int16;
				case DBColumnType.UInt16:
					return DbType.UInt16;
				case DBColumnType.Int64:
					return DbType.Int64;
				case DBColumnType.UInt64:
					return DbType.UInt64;
				case DBColumnType.DateTime:
					return DbType.DateTime;
				case DBColumnType.ByteArray:
					return DbType.Binary;
				case DBColumnType.Guid:
					return DbType.Guid;
				case DBColumnType.Unknown:
					return DbType.Object;
				default:
					throw new InvalidOperationException(type.ToString());
			}
		}
		public const string IdentityColumnMagicName = "XpoIdentityColumn";
		public virtual string GenerateStoredProceduresInfoOnce() { return string.Empty; }
		public virtual string GenerateStoredProcedures(DBTable table, out string dropLines) {
			throw new NotSupportedException();
		}
		public virtual DBStoredProcedure[] GetStoredProcedures() {
			throw new NotImplementedException();
		}
		protected bool IsKey(DBTable table, string column) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(table.PrimaryKey.Columns[i] == column) { return true; }
			}
			return false;
		}
		protected DBColumn GetDbColumnByName(DBTable table, string name) {
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].Name == name) { return table.Columns[i]; }
			}
			throw new InvalidOperationException("Couldn't find primary key column.");
		}
		protected void StringBuilderAppendLine(StringBuilder sb) {
			StringBuilderAppendLine(sb, string.Empty);
		}
		protected void StringBuilderAppendLine(StringBuilder sb, string str) {
#if CF
			sb.Append(str + '\n');
#else
			sb.AppendLine(str);
#endif
		}
		protected bool ColumnIsIdentity(DBTable table, string column) {
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].Name != column) { continue; }
				return table.Columns[i].IsIdentity;
			}
			throw new InvalidOperationException("Column not found");
		}
		protected abstract string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column);
		protected abstract string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column);
		protected virtual string GetSqlCreateColumnTypeForTimeSpan(DBTable table, DBColumn column) {
			return GetSqlCreateColumnTypeForDouble(table, column);
		}
		protected abstract string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column);
		protected string GetSqlCreateColumnType(DBTable table, DBColumn column) {
			if(column.DBTypeName != null && column.DBTypeName.Length > 0)
				return column.DBTypeName;
			switch(column.ColumnType) {
				case DBColumnType.Boolean:
					return GetSqlCreateColumnTypeForBoolean(table, column);
				case DBColumnType.Byte:
					return GetSqlCreateColumnTypeForByte(table, column);
				case DBColumnType.SByte:
					return GetSqlCreateColumnTypeForSByte(table, column);
				case DBColumnType.Char:
					return GetSqlCreateColumnTypeForChar(table, column);
				case DBColumnType.Decimal:
					return GetSqlCreateColumnTypeForDecimal(table, column);
				case DBColumnType.Double:
					return GetSqlCreateColumnTypeForDouble(table, column);
				case DBColumnType.Single:
					return GetSqlCreateColumnTypeForSingle(table, column);
				case DBColumnType.Int32:
					return GetSqlCreateColumnTypeForInt32(table, column);
				case DBColumnType.UInt32:
					return GetSqlCreateColumnTypeForUInt32(table, column);
				case DBColumnType.Int16:
					return GetSqlCreateColumnTypeForInt16(table, column);
				case DBColumnType.UInt16:
					return GetSqlCreateColumnTypeForUInt16(table, column);
				case DBColumnType.Int64:
					return GetSqlCreateColumnTypeForInt64(table, column);
				case DBColumnType.UInt64:
					return GetSqlCreateColumnTypeForUInt64(table, column);
				case DBColumnType.String:
					return GetSqlCreateColumnTypeForString(table, column);
				case DBColumnType.DateTime:
					return GetSqlCreateColumnTypeForDateTime(table, column);
				case DBColumnType.Guid:
					return GetSqlCreateColumnTypeForGuid(table, column);
				case DBColumnType.TimeSpan:
					return GetSqlCreateColumnTypeForTimeSpan(table, column);
				case DBColumnType.ByteArray:
					return GetSqlCreateColumnTypeForByteArray(table, column);
			}
			throw new ArgumentException();
		}
		public abstract string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column);
		protected void ExecuteSqlSchemaUpdate(string objectTypeName, string objectName, string parentObjectName, string textSql) {
			if(!CanCreateSchema)
				throw new SchemaCorrectionNeededException(textSql);
			try {
				ExecSql(new Query(textSql));
			} catch(Exception ex) {
				throw new UnableToCreateDBObjectException(objectTypeName, objectName, parentObjectName, ex);
			}
		}
		public abstract ICollection CollectTablesToCreate(ICollection tables);
		public abstract void GetTableSchema(DBTable table, bool checkIndexes, bool checkForeignKeys);
		public abstract string[] GetStorageTablesList(bool includeViews);
		public virtual DBTable[] GetStorageTables(params string[] tables) {
			if(tables == null) {
				tables = GetStorageTablesList(false);
				if(tables == null) {
					return new DBTable[0];
				}
			}
			List<DBTable> result = new List<DBTable>(tables.Length);
			foreach(string tableName in tables) {
				DBTable newTable = new DBTable(tableName);
				GetTableSchema(newTable, true, true);
				result.Add(newTable);
			}
			return result.ToArray();
		}
		bool IsColumnExists(DBTable table, DBColumn column) {
			for(int i = 0; i < table.Columns.Count; i++)
				if(String.Compare(((DBColumn)table.Columns[i]).Name, ComposeSafeColumnName(column.Name), true) == 0)
					return true;
			return false;
		}
		bool IsColumnsEqual(StringCollection first, StringCollection second) {
			if(first.Count != second.Count)
				return false;
			for(int i = 0; i < first.Count; i++)
				if(String.Compare(ComposeSafeColumnName(first[i]), ComposeSafeColumnName(second[i]), true) != 0)
					return false;
			return true;
		}
		bool IsIndexExists(DBTable table, DBIndex index) {
			foreach(DBIndex i in table.Indexes) {
				if(IsColumnsEqual(i.Columns, index.Columns))
					return true;
			}
			return false;
		}
		bool IsForeignKeyExists(DBTable table, DBForeignKey foreignKey) {
			foreach(DBForeignKey fk in table.ForeignKeys) {
				if(String.Compare(ComposeSafeTableName(foreignKey.PrimaryKeyTable), ComposeSafeTableName(fk.PrimaryKeyTable), true) == 0 && IsColumnsEqual(fk.Columns, foreignKey.Columns) && IsColumnsEqual(fk.PrimaryKeyTableKeyColumns, foreignKey.PrimaryKeyTableKeyColumns))
					return true;
			}
			return false;
		}
		protected virtual bool NeedsIndexForForeignKey { get { return true; } }
		protected override UpdateSchemaResult ProcessUpdateSchema(bool skipIfFirstTableNotExists, params DBTable[] tables) {
			ICollection collectedTables = CollectTablesToCreate(tables);
			if(skipIfFirstTableNotExists && collectedTables.Count > 0) {
				IEnumerator te = tables.GetEnumerator();
				IEnumerator ce = collectedTables.GetEnumerator();
				te.MoveNext();
				ce.MoveNext();
				if(object.ReferenceEquals(te.Current, ce.Current))
					return UpdateSchemaResult.FirstTableNotExists;
			}
			if(CanCreateSchema)
				BeginTransaction();
			try {
				if(!CanCreateSchema && collectedTables.Count > 0) {
					IEnumerator ce = collectedTables.GetEnumerator();
					ce.MoveNext();
					throw new SchemaCorrectionNeededException("Table '" + ComposeSafeTableName(((DBTable)ce.Current).Name) + "' not found");
				} else {
					foreach(DBTable table in collectedTables)
						CreateTable(table);
				}
				Dictionary<DBTable, DBTable> newTables = new Dictionary<DBTable, DBTable>();
				foreach(DBTable table in tables) {
					if(table.IsView)
						continue;
					DBTable newtable = new DBTable(table.Name);
					bool collectIndexes = false;
					bool collectFKs = false;
					if(CanCreateSchema) {
						collectIndexes = table.Indexes.Count > 0;
						collectFKs = table.ForeignKeys.Count > 0;
						if(NeedsIndexForForeignKey)
							collectIndexes = collectIndexes || collectFKs;
					}
					GetTableSchema(newtable, collectIndexes, collectFKs);
					newTables[table] = newtable;
					foreach(DBColumn column in table.Columns) {
						if(!IsColumnExists(newtable, column)) {
							if(CanCreateSchema)
								CreateColumn(table, column);
							else
								throw new SchemaCorrectionNeededException("Column '" + ComposeSafeColumnName(column.Name) + "' not found in table '" + ComposeSafeTableName(table.Name) + "'");
						}
					}
					if(CanCreateSchema && newtable.PrimaryKey == null && table.PrimaryKey != null) {
						CreatePrimaryKey(table);
					}
				}
				if(CanCreateSchema) {
					foreach(DBTable table in tables) {
						DBTable newtable;
						newTables.TryGetValue(table, out newtable);
						if(newtable == null)
							continue;
						foreach(DBIndex index in table.Indexes) {
							if(!IsIndexExists(newtable, index)) {
								CreateIndex(table, index);
								newtable.AddIndex(index);
							}
						}
						if(NeedsIndexForForeignKey) {
							foreach(DBForeignKey fk in table.ForeignKeys) {
								DBIndex index = new DBIndex(fk.Columns, false);
								if(!IsIndexExists(newtable, index) &&
									(table.PrimaryKey == null || (table.PrimaryKey != null && !IsColumnsEqual(table.PrimaryKey.Columns, index.Columns)))) {
									CreateIndex(table, index);
									newtable.AddIndex(index);
								}
							}
						}
						foreach(DBForeignKey fk in table.ForeignKeys) {
							if(!IsForeignKeyExists(newtable, fk))
								CreateForeignKey(table, fk);
						}
					}
					CommitTransaction();
				}
			} catch {
				if(CanCreateSchema)
					RollbackTransaction();
				throw;
			}
			return UpdateSchemaResult.SchemaExists;
		}
		public virtual void CreateTable(DBTable table) {
			string columns = "";
			foreach(DBColumn col in table.Columns) {
				if(columns.Length > 0)
					columns += ", ";
				columns += (FormatColumnSafe(col.Name) + ' ' + GetSqlCreateColumnFullAttributes(table, col));
			}
			ExecuteSqlSchemaUpdate("Table", table.Name, string.Empty, String.Format(CultureInfo.InvariantCulture,
				"create table {0} ({1})",
				FormatTableSafe(table), columns));
		}
		public virtual void CreatePrimaryKey(DBTable table) {
			StringCollection formattedColumns = new StringCollection();
			for(Int32 i = 0; i < table.PrimaryKey.Columns.Count; ++i)
				formattedColumns.Add(FormatColumnSafe(table.PrimaryKey.Columns[i]));
			ExecuteSqlSchemaUpdate("PrimaryKey", GetPrimaryKeyName(table.PrimaryKey, table), table.Name, String.Format(CultureInfo.InvariantCulture,
				"alter table {0} add constraint {1} primary key ({2})",
				FormatTableSafe(table), FormatConstraintSafe(GetPrimaryKeyName(table.PrimaryKey, table)), StringListHelper.DelimitedText(formattedColumns, ",")));
		}
		public virtual void CreateColumn(DBTable table, DBColumn column) {
			ExecuteSqlSchemaUpdate("Column", column.Name, table.Name, String.Format(CultureInfo.InvariantCulture,
				"alter table {0} add {1} {2}",
				FormatTableSafe(table), FormatColumnSafe(column.Name), GetSqlCreateColumnFullAttributes(table, column)));
		}
		public virtual void CreateIndex(DBTable table, DBIndex index) {
			StringCollection formattedColumns = new StringCollection();
			for(Int32 i = 0; i < index.Columns.Count; ++i)
				formattedColumns.Add(FormatColumnSafe(index.Columns[i]));
			ExecuteSqlSchemaUpdate("Index", GetIndexName(index, table), table.Name, String.Format(CultureInfo.InvariantCulture,
				CreateIndexTemplate,
				index.IsUnique ? "unique" : string.Empty, FormatConstraintSafe(GetIndexName(index, table)), FormatTableSafe(table), StringListHelper.DelimitedText(formattedColumns, ",")));
		}
		protected virtual string CreateIndexTemplate { get { return "create {0} index {1} on {2}({3})"; } }
		public virtual void CreateForeignKey(DBTable table, DBForeignKey fk) {
			StringCollection formattedColumns = new StringCollection();
			for(Int32 i = 0; i < fk.Columns.Count; ++i)
				formattedColumns.Add(FormatColumnSafe(fk.Columns[i]));
			StringCollection formattedRefColumns = new StringCollection();
			for(Int32 i = 0; i < fk.PrimaryKeyTableKeyColumns.Count; ++i)
				formattedRefColumns.Add(FormatColumnSafe(fk.PrimaryKeyTableKeyColumns[i]));
			ExecuteSqlSchemaUpdate("ForeignKey", GetForeignKeyName(fk, table), table.Name, String.Format(CultureInfo.InvariantCulture,
				CreateForeignKeyTemplate,
				FormatTableSafe(table),
				FormatConstraintSafe(GetForeignKeyName(fk, table)),
				StringListHelper.DelimitedText(formattedColumns, ","),
				FormatTable(ComposeSafeSchemaName(fk.PrimaryKeyTable), ComposeSafeTableName(fk.PrimaryKeyTable)),
				StringListHelper.DelimitedText(formattedRefColumns, ",")));
		}
		protected virtual string CreateForeignKeyTemplate { get { return "alter table {0} add constraint {1} foreign key ({2}) references {3}({4})"; } }
		IDbTransaction transaction;
		protected virtual Exception WrapException(Exception e, IDbCommand query) {
			return new SqlExecutionErrorException(query.CommandText, GetParametersString(query), e);
		}
		static protected string GetParametersString(IDbCommand query) {
			StringBuilder parameters = new StringBuilder(query.Parameters.Count * 10);
			int count = query.Parameters.Count;
			for(int i = 0; i < count; i++) {
				IDataParameter param = (IDataParameter)query.Parameters[i];
				string parameter = param.Value == null ? "Null" : param.Value.ToString();
				if(parameter.Length > 64)
					parameter = parameter.Substring(0, 64) + "...";
				parameters.AppendFormat(CultureInfo.InvariantCulture, i == 0 ? "{{{0}}}" : ",{{{0}}}", parameter);
			}
			return parameters.ToString();
		}
		int InternalExecSql(IDbCommand command) {
			return LogManager.Log<int>(LogManager.LogCategorySQL, () => {
				return command.ExecuteNonQuery();
			}, (d) => {
				return LogMessage.CreateMessage(this, command, d);
			});
		}
		protected class ReformatReadValueArgs {
			public Type DbType;
			public TypeCode DbTypeCode;
			public readonly Type TargetType;
			public readonly TypeCode TargetTypeCode;
			public ReformatReadValueArgs(Type targetType) {
				this.DbType = null;
				this.DbTypeCode = TypeCode.Empty;
				this.TargetType = targetType;
				this.TargetTypeCode = DXTypeExtensions.GetTypeCode(targetType);
			}
			public void AttachValueReadFromDb(object dbData) {
				if(this.DbType != null)
					return;
				this.DbType = dbData.GetType();
				this.DbTypeCode = DXTypeExtensions.GetTypeCode(this.DbType);
			}
		}
		ReformatReadValueArgs[] FillReformatters(CriteriaOperatorCollection targets) {
			ReformatReadValueArgs[] result = new ReformatReadValueArgs[targets.Count];
			for(int i = 0; i < targets.Count; ++i) {
				Type targetType = ColumnTypeResolver.ResolveType(targets[i], this);
				result[i] = new ReformatReadValueArgs(Nullable.GetUnderlyingType(targetType) ?? targetType);
			}
			return result;
		}
		void ReformatReadValues(object[] values, ReformatReadValueArgs[] converters) {
			for(int i = 0; i < values.Length; ++i) {
				object value = values[i];
				if(value == null) {
					continue;
				}
				if(value == DBNull.Value) {
					values[i] = null;
					continue;
				}
				ReformatReadValueArgs args = converters[i];
				args.AttachValueReadFromDb(value);
				values[i] = ReformatReadValue(value, args);
			}
		}
		protected virtual object ReformatReadValue(object value, ReformatReadValueArgs args) {
			if(args.DbType == args.TargetType)
				return value;
			switch(args.TargetTypeCode) {
				case TypeCode.Object:
					if(args.TargetType == typeof(Guid)) {
						return value is byte[] ? new Guid((byte[])value) : new Guid(value.ToString());
					} else if(args.TargetType == typeof(TimeSpan)) {
						double seconds = Convert.ToDouble(value);
						if(seconds > TimeSpan.MaxValue.TotalSeconds - 0.0005 && seconds < TimeSpan.MaxValue.TotalSeconds + 0.0005)
							return TimeSpan.MaxValue;
						if(seconds < TimeSpan.MinValue.TotalSeconds + 0.0005 && seconds > TimeSpan.MinValue.TotalSeconds - 0.0005)
							return TimeSpan.MinValue;
						return TimeSpan.FromSeconds(seconds);
					} else if(args.TargetType == typeof(object)) {
						return value;
					}
					break;
				case TypeCode.Char:
					if(args.DbTypeCode == TypeCode.String) {
						string charStr = (string)value;
						if(charStr.Length == 0)
							return ' ';
						else
							return Convert.ToChar(charStr);
					}
					break;
			}
			return Convert.ChangeType(value, args.TargetType, CultureInfo.InvariantCulture);
		}
		protected virtual bool IsFieldTypesNeeded { get { return false; } }
		protected virtual void GetValues(IDataReader reader, Type[] fieldTypes, object[] values) {
			reader.GetValues(values);
		}
		protected SelectStatementResult[] InternalGetData(IDbCommand command, CriteriaOperatorCollection targets, int skipClause, int topClause, bool includeMetadata) {
			return LogManager.Log<SelectStatementResult[]>(LogManager.LogCategorySQL, () => {
				using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
					List<SelectStatementResult> selectResults = new List<SelectStatementResult>();
					do {
						if(!NativeSkipTakeSupported) {
							bool doContinue = false;
							for(int i = 0; i < skipClause; ++i) {
								if(!reader.Read()) {
									if(includeMetadata)
										selectResults.Add(new SelectStatementResult(FillMetaData(reader, null)));
									selectResults.Add(new SelectStatementResult());
									doContinue = true;
									break;
								}
							}
							if(doContinue)
								continue;
						}
						List<object[]> resultSet = new List<object[]>();
						ReformatReadValueArgs[] converters = null;
						object[][] metaDataSet = null;
						Type[] fieldTypes = null;
						for(; ; ) {
							if(topClause > 0 && resultSet.Count >= topClause)
								break;
							if(!reader.Read())
								break;
							if(IsFieldTypesNeeded && fieldTypes == null) {
								fieldTypes = FillTypes(reader);
							}
							if(includeMetadata && metaDataSet == null) {
								metaDataSet = FillMetaData(reader, fieldTypes);
							}
							object[] values = new object[reader.FieldCount];
							GetValues(reader, fieldTypes, values);
							if(targets != null) {
								if(converters == null) {
									converters = FillReformatters(targets);
								}
								ReformatReadValues(values, converters);
							}
							resultSet.Add(values);
						}
						if(includeMetadata) {
							if(metaDataSet == null) {
								metaDataSet = FillMetaData(reader, fieldTypes);
							}
							selectResults.Add(new SelectStatementResult(metaDataSet));
						}
						selectResults.Add(new SelectStatementResult(resultSet));
					} while(topClause == 0 && skipClause == 0 && reader.NextResult());
					return selectResults.ToArray();
				}
			}, (d) => {
				return LogMessage.CreateMessage(this, command, d);
			});
		}
		static Type[] FillTypes(IDataReader reader) {
			Type[] fieldTypes = new Type[reader.FieldCount];
			for(int i = reader.FieldCount - 1; i >= 0; i--) {
				fieldTypes[i] = reader.GetFieldType(i);
			}
			return fieldTypes;
		}
		static object[][] FillMetaData(IDataReader reader, Type[] fieldTypes) {
			object[][] metaDataSet = new object[reader.FieldCount][];
			bool hasFieldTypes = fieldTypes != null;
			for(int i = reader.FieldCount - 1; i >= 0; i--) {
				Type columnType = hasFieldTypes ? fieldTypes[i] : reader.GetFieldType(i);
				metaDataSet[i] = new object[] { reader.GetName(i), reader.GetDataTypeName(i), DBColumn.GetColumnType(columnType, true).ToString() };
			}
			return metaDataSet;
		}
		protected virtual object InternalGetScalar(IDbCommand command) {
			return LogManager.Log<object>(LogManager.LogCategorySQL, () => {
				return command.ExecuteScalar();
			}, (d) => {
				return LogMessage.CreateMessage(this, command, d);
			});
		}
		protected virtual bool IsDeadLock(Exception e) {
			return false;
		}
		protected virtual bool IsConnectionBroken(Exception e) {
			return Connection.State != ConnectionState.Open;
		}
		protected virtual void OpenConnectionInternal() {
			Connection.Open();
		}
		void DoReconnect() {
			OpenConnectionInternal();
			OnReconnected();
		}
		protected virtual void OnReconnected() {
			if(reconnected != null)
				reconnected(this, EventArgs.Empty);
		}
		protected void OpenConnection() {
			ConnectionState state = Connection.State;
			System.Diagnostics.Debug.Assert(state != ConnectionState.Executing);
			System.Diagnostics.Debug.Assert(state != ConnectionState.Fetching);
			if(state == ConnectionState.Broken) {
				Connection.Close();
				state = ConnectionState.Closed;
			}
			if(state == ConnectionState.Closed) {
				System.Diagnostics.Debug.Assert(Connection.ConnectionString.Length != 0);
				OpenConnectionInternal();
			}
		}
		IDbTransaction ConnectionBeginTransaction(object il) {
			if(il == null)
				return Connection.BeginTransaction();
			else
				return Connection.BeginTransaction((IsolationLevel)il);
		}
		protected virtual void BeginTransactionCore(object il) {
			try {
				OpenConnection();
#if !CF && !DXPORTABLE
				if(System.Transactions.Transaction.Current == null)
#endif
					transaction = ConnectionBeginTransaction(il);
				CreateCommandPool();
			} catch(Exception e) {
				if(IsConnectionBroken(e)) {
					DoReconnect();
					transaction = ConnectionBeginTransaction(il);
					CreateCommandPool();
				} else
					throw;
			}
		}
		protected virtual void CommitTransactionCore() {
			ReleaseCommandPool();
			if(transaction != null)
				transaction.Commit();
			transaction = null;
		}
		protected virtual void RollbackTransactionCore() {
			ReleaseCommandPool();
			if(transaction != null) {
				try {
					transaction.Rollback();
				} catch {
				}
				transaction = null;
			}
		}
		protected void BeginTransaction() {
			if(!explicitTransaction)
				BeginTransactionCore(null);
		}
		protected void CommitTransaction() {
			if(!explicitTransaction)
				CommitTransactionCore();
		}
		protected void RollbackTransaction() {
			if(!explicitTransaction)
				RollbackTransactionCore();
		}
		public void ExplicitBeginTransaction() {
			BeginTransactionCore(null);
			explicitTransaction = true;
		}
		public void ExplicitBeginTransaction(IsolationLevel il) {
			BeginTransactionCore(il);
			explicitTransaction = true;
		}
		public void ExplicitCommitTransaction() {
			CommitTransactionCore();
			explicitTransaction = false;
		}
		public void ExplicitRollbackTransaction() {
			RollbackTransactionCore();
			explicitTransaction = false;
		}
		public virtual IDbTransaction Transaction { get { return transaction; } }
		public virtual IDbCommand CreateCommand() {
			OpenConnection();
			IDbCommand command = Connection.CreateCommand();
			command.Connection = Connection;
			command.Transaction = Transaction;
			command.CommandTimeout = 300;
			return command;
		}
		protected virtual object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			switch(clientValueTypeCode) {
				case TypeCode.Char:
					return new string((char)clientValue, 1);
				case TypeCode.Object:
					if(clientValue is TimeSpan) {
						return ((TimeSpan)clientValue).TotalSeconds;
					}
					break;
			}
			return clientValue;
		}
		protected object ConvertParameter(object parameter) {
			if(parameter == null)
				return System.DBNull.Value;
			TypeCode clientTypeCode = DXTypeExtensions.GetTypeCode(parameter.GetType());
			object result = ConvertToDbParameter(parameter, clientTypeCode);
			return result;
		}
		protected virtual IDataParameter CreateParameter(IDbCommand command) {
			return command.CreateParameter();
		}
		protected static TraceSwitch xpoSwitch = new TraceSwitch("XPO", "");
		protected class DbCommandTracer {
			readonly IDbCommand command;
			public DbCommandTracer(IDbCommand command) {
				this.command = command;
			}
			public override string ToString() {
				string parameters = GetParametersString(command);
				return string.Format(!String.IsNullOrEmpty(parameters) ?
					"{0:dd.MM.yy HH:mm:ss.fff} Executing sql '{1}' with parameters {2}" :
					"{0:dd.MM.yy HH:mm:ss.fff} Executing sql '{1}'",
					DateTime.Now, command.CommandText.Replace('\n', ' '), parameters);
			}
		}
		protected class SelectStatementResultTracer {
			SelectStatementResult[] results;
			CriteriaOperatorCollection targets;
			public SelectStatementResultTracer(CriteriaOperatorCollection targets, SelectStatementResult[] results) {
				this.results = results;
				this.targets = targets;
			}
			int GetValueSize(object value) {
				if(value == null)
					return 4;
				switch(DXTypeExtensions.GetTypeCode(value.GetType())) {
					case TypeCode.Boolean:
						return 1;
					case TypeCode.UInt16:
					case TypeCode.Int16:
						return 2;
					case TypeCode.UInt32:
					case TypeCode.Single:
					case TypeCode.Int32:
						return 4;
					case TypeCode.String:
						return ((string)value).Length * 2 + 4;
					case TypeCode.DateTime:
						return 8;
					case TypeCode.Decimal:
						return 10;
					case TypeCode.Double:
						return 8;
					case TypeCode.Object: {
							if(value is byte[])
								return ((byte[])value).Length + 4;
							if(value is Guid)
								return 16;
							return 4;
						}
					case TypeCode.Byte:
						return 1;
					case TypeCode.Char:
						return 2;
					case TypeCode.Int64:
					case TypeCode.UInt64:
						return 8;
				}
				return 0;
			}
			public override string ToString() {
				SelectStatementResult result = results.Length == 2 ? results[1] : results[0];
				int count = result.Rows.Length;
				if(count > 0) {
					if(targets != null) {
						int paramCount = result.Rows[0].Values.Length;
						int[] sizes = new int[paramCount];
						for(int i = 0; i < count; i++) {
							SelectStatementResultRow row = result.Rows[i];
							for(int j = 0; j < row.Values.Length; j++) {
								sizes[j] += GetValueSize(row.Values[j]);
							}
						}
						int size = 0;
						StringBuilder parameters = new StringBuilder(paramCount * 6);
						for(int i = 0; i < paramCount; i++) {
							parameters.AppendFormat(CultureInfo.InvariantCulture, i == 0 ? "{0} = {1}" : ", {0} = {1}", targets[i], sizes[i]);
							size += sizes[i];
						}
						return String.Format("{3}Result: rowcount = {0}, total = {1}, {2}", count, size, parameters, DateTime.Now.ToString("dd.MM.yy HH:mm:ss.fff "));
					} else {
						int size = 0;
						for(int i = 0; i < count; i++) {
							SelectStatementResultRow row = result.Rows[i];
							for(int j = 0; j < row.Values.Length; j++) {
								size += GetValueSize(row.Values[j]);
							}
						}
						return String.Format("{2}Result: rowcount = {0}, size = {1}", count, size, DateTime.Now.ToString("dd.MM.yy HH:mm:ss.fff "));
					}
				} else {
					return DateTime.Now.ToString("dd.MM.yy HH:mm:ss.fff ") + "Result: Empty";
				}
			}
		}
		protected virtual IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			IDataParameter param = CreateParameter(command);
			param.Value = value;
			param.ParameterName = name;
			if(param.DbType == DbType.AnsiString)
				param.DbType = DbType.String;
			return param;
		}
		protected void PrepareParameters(IDbCommand command, Query query) {
			for(int i = 0; i < query.Parameters.Count; ++i) {
				command.Parameters.Add(CreateParameter(command, ConvertParameter(query.Parameters[i].Value), (string)query.ParametersNames[i]));
			}
		}
		void PreparePoolParameters(IDbCommand command, Query query) {
			for(int i = 0; i < query.Parameters.Count; ++i)
				((IDbDataParameter)command.Parameters[i]).Value = ConvertParameter(query.Parameters[i].Value);
		}
		void CreateCommandPool() {
			if(SupportCommandPrepare)
				commandPool = new List<IDbCommand>();
		}
		void ReleaseCommandPool() {
			if(commandPool != null) {
				for(int i = 0; i < commandPool.Count; i++)
					commandPool[i].Dispose();
				commandPool = null;
			}
		}
		protected virtual bool SupportCommandPrepare { get { return false; } }
		List<IDbCommand> commandPool;
		IDbCommand GetCommandFromPool(Query query) {
			if(commandPool == null)
				return CreateCommand(query);
			int count = commandPool.Count;
			IDbCommand command = null;
			for(int i = 0; i < count; i++) {
				if(commandPool[i].CommandText == query.Sql) {
					command = commandPool[i];
					commandPool.RemoveAt(i);
					break;
				}
			}
			if(command == null)
				return CreateCommand(query);
			PreparePoolParameters(command, query);
#if !CF
			Trace.WriteLineIf(xpoSwitch.TraceInfo, new DbCommandTracer(command));
#endif
			return command;
		}
		void ReleasePooledCommand(IDbCommand command) {
			if(commandPool != null) {
				commandPool.Insert(0, command);
				if(commandPool.Count <= 10)
					return;
				else {
					command = commandPool[10];
					commandPool.RemoveAt(10);
				}
			}
			command.Dispose();
		}
		protected virtual IDbCommand CreateCommand(Query query) {
			IDbCommand command = CreateCommand();
			PrepareParameters(command, query);
			command.CommandText = query.Sql;
#if !CF
			Trace.WriteLineIf(xpoSwitch.TraceInfo, new DbCommandTracer(command));
#endif
			return command;
		}
		protected SelectStatementResult SelectData(Query query) {
			return SelectData(query, false);
		}
		protected SelectStatementResult SelectData(Query query, bool includeMetadata) {
			return SelectData(query, null, includeMetadata);
		}
		public static int MaxDeadLockTryCount = 3;
		public static int MaxDeadLockRetryDelayMilliseconds = 500;
		SelectStatementResult SelectData(Query query, CriteriaOperatorCollection targets, bool includeMetadata) {
			if(query.ConstantValues != null && query.OperandIndexes != null && query.ConstantValues.Count > 0) {
				CriteriaOperatorCollection customTargets = new CriteriaOperatorCollection();
				if(query.OperandIndexes.Count == 0) {
					customTargets.Add(new OperandValue(1));
				} else {
					CriteriaOperator[] trgts = new CriteriaOperator[query.OperandIndexes.Count];
					for(int i = 0; i < targets.Count; i++) {
						if(query.OperandIndexes.ContainsKey(i)) {
							trgts[query.OperandIndexes[i]] = targets[i];
						}
					}
					customTargets.AddRange(trgts);
				}
				SelectStatementResult queryResult = SelectDataSimple(query, customTargets, includeMetadata)[0];
				SelectStatementResultRow[] rows = new SelectStatementResultRow[queryResult.Rows.Length];
				for(int ri = 0; ri < rows.Length; ri++) {
					object[] values = new object[targets.Count];
					for(int i = 0; i < targets.Count; i++) {
						if(query.OperandIndexes.ContainsKey(i)) {
							values[i] = queryResult.Rows[ri].Values[query.OperandIndexes[i]];
						} else {
							values[i] = query.ConstantValues[i].Value;
						}
					}
					rows[ri] = new SelectStatementResultRow(values);
				}
				return new SelectStatementResult(rows);
			}
			return SelectDataSimple(query, targets, includeMetadata)[0];
		}
		SelectStatementResult[] SelectDataSimple(Query query, CriteriaOperatorCollection targets, bool includeMetadata) {
			IDbCommand command = GetCommandFromPool(query);
			try {
				SelectStatementResult[] result;
				for(int tryCount = 1; ; ++tryCount) {
					try {
						result = InternalGetData(command, targets, query.SkipSelectedRecords, query.TopSelectedRecords, includeMetadata);
						break;
					} catch(Exception e) {
						if(Transaction == null && IsConnectionBroken(e) && tryCount <= 1) {
							try {
								DoReconnect();
								continue;
							} catch {
							}
						} else if(Transaction != null && !explicitTransaction && IsDeadLock(e) && tryCount <= MaxDeadLockTryCount) {
							if(tryCount > 1) {
								System.Threading.Thread.Sleep(Randomizer.Next(MaxDeadLockRetryDelayMilliseconds));
							}
							continue;
						}
						throw WrapException(e, command);
					}
				}
#if !CF
				Trace.WriteLineIf(xpoSwitch.TraceInfo, new SelectStatementResultTracer(targets, result));
#endif
				return result;
			} finally {
				ReleasePooledCommand(command);
			}
		}
		protected override SelectStatementResult ProcessSelectData(SelectStatement selects) {
			Query query = new SelectSqlGenerator(this).GenerateSql(selects);
			return SelectData(query, selects.Operands, false);
		}
		protected virtual Int64 GetIdentity(Query sql) {
			return -1;
		}
		protected virtual Int64 GetIdentity(InsertStatement root, TaggedParametersHolder identities) {
			return GetIdentity(new InsertSqlGenerator(this, identities, new Dictionary<OperandValue, string>()).GenerateSql(root));
		}
		protected object GetScalar(Query query) {
			IDbCommand command = GetCommandFromPool(query);
			try {
				try {
					try {
						return InternalGetScalar(command);
					} catch(Exception e) {
						if(Transaction == null && IsConnectionBroken(e)) {
							DoReconnect();
							return InternalGetScalar(command);
						} else
							throw;
					}
				} catch(Exception e) {
					throw WrapException(e, command);
				}
			} finally {
				ReleasePooledCommand(command);
			}
		}
		public int ExecSql(Query query) {
			lock(SyncRoot) {	
				IDbCommand command = GetCommandFromPool(query);
				try {
					try {
						try {
							return InternalExecSql(command);
						} catch(Exception e) {
							if(Transaction == null && IsConnectionBroken(e)) {
								DoReconnect();
								return InternalExecSql(command);
							} else
								throw;
						}
					} catch(Exception e) {
						throw WrapException(e, command);
					}
				} finally {
					ReleasePooledCommand(command);
				}
			}
		}
		protected void ExecSql(QueryCollection queries) {
			foreach(Query query in queries)
				ExecSql(query);
		}
		ParameterValue DoInsertRecord(InsertStatement root, TaggedParametersHolder identities) {
			if(ReferenceEquals(root.IdentityParameter, null)) {
				ExecSql(new InsertSqlGenerator(this, identities, new Dictionary<OperandValue, string>()).GenerateSql(root));
				return null;
			} else {
				identities.ConsolidateIdentity(root.IdentityParameter);
				switch(root.IdentityColumnType) {
					case DBColumnType.Int32:
						root.IdentityParameter.Value = (int)GetIdentity(root, identities);
						break;
					case DBColumnType.Int64:
						root.IdentityParameter.Value = GetIdentity(root, identities);
						break;
					default:
						throw new NotSupportedException(string.Format("The AutoIncremented key with '{0}' type is not supported for '{1}'.", root.IdentityColumnType, this.GetType()));
				}
				return root.IdentityParameter;
			}
		}
		ParameterValue DoUpdateRecord(UpdateStatement root, TaggedParametersHolder identities) {
			Query query = new UpdateSqlGenerator(this, identities, new Dictionary<OperandValue, string>()).GenerateSql(root);
			if(query.Sql != null) {
				int count = ExecSql(query);
				if(root.RecordsAffected != 0 && root.RecordsAffected != count) {
					RollbackTransaction();
					throw new LockingException();
				}
			}
			return null;
		}
		ParameterValue DoDeleteRecord(DeleteStatement root, TaggedParametersHolder identities) {
			int count = ExecSql(new DeleteSqlGenerator(this, identities, new Dictionary<OperandValue, string>()).GenerateSql(root));
			if(root.RecordsAffected != 0 && root.RecordsAffected != count) {
				RollbackTransaction();
				throw new LockingException();
			}
			return null;
		}
		protected ParameterValue UpdateRecord(ModificationStatement root, TaggedParametersHolder identities) {
			if(root is InsertStatement) {
				return DoInsertRecord((InsertStatement)root, identities);
			} else if(root is UpdateStatement) {
				return DoUpdateRecord((UpdateStatement)root, identities);
			} else if(root is DeleteStatement) {
				return DoDeleteRecord((DeleteStatement)root, identities);
			} else {
				throw new InvalidOperationException();
			}
		}
		protected override ModificationResult ProcessModifyData(params ModificationStatement[] dmlStatements) {
			BeginTransaction();
			try {
				List<ParameterValue> result = new List<ParameterValue>();
				TaggedParametersHolder identities = new TaggedParametersHolder();
				foreach(ModificationStatement root in dmlStatements) {
					ParameterValue res = UpdateRecord(root, identities);
					if(!object.ReferenceEquals(null, res)) {
						result.Add(res);
					}
				}
				CommitTransaction();
				return new ModificationResult(result);
			} catch {
				RollbackTransaction();
				throw;
			}
		}
		EventHandler reconnected;
		public event EventHandler Reconnected {
			add { reconnected += value; }
			remove { reconnected -= value; }
		}
#region ISqlGeneratorFormatter implementation
		public abstract string FormatTable(string schema, string tableName);
		public abstract string FormatTable(string schema, string tableName, string tableAlias);
		public abstract string FormatColumn(string columnName);
		public abstract string FormatColumn(string columnName, string tableAlias);
		public virtual string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords) {
			return FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, 0, topSelectedRecords);
		}
		public abstract string FormatInsertDefaultValues(string tableName);
		public abstract string FormatInsert(string tableName, string fields, string values);
		public abstract string FormatUpdate(string tableName, string sets, string whereClause);
		public abstract string FormatDelete(string tableName, string whereClause);
		public virtual bool NativeSkipTakeSupported { get { return false; } }
		public virtual string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords) {
			if(!NativeSkipTakeSupported) { throw new NotSupportedException(); }
			if(skipSelectedRecords != 0 && orderBySql == null) {
				throw new InvalidOperationException("Can not skip records without ORDER BY clause.");
			}
			return null;
		}
		public virtual string FormatUnary(UnaryOperatorType operatorType, string operand) {
			return BaseFormatterHelper.DefaultFormatUnary(operatorType, operand);
		}
		public virtual string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			return BaseFormatterHelper.DefaultFormatBinary(operatorType, leftOperand, rightOperand);
		}
		string FormatCustomFunction(params string[] operands) {
			string functionName = operands[0];
			string[] newOperands;
			if(operands.Length > 1) {
				newOperands = new string[operands.Length - 1];
				Array.Copy(operands, 1, newOperands, 0, operands.Length - 1);
			} else {
				newOperands = new string[0];
			}
			ICustomFunctionOperatorFormattable customFunction = GetCustomFunctionOperator(functionName);
			if(customFunction == null)
				throw new NotSupportedException(string.Format("DefaultFormatFunction for custom({0})", functionName)); 
			return customFunction.Format(this.GetType(), newOperands);
		}
		public virtual string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			if(operatorType == FunctionOperatorType.Custom || operatorType == FunctionOperatorType.CustomNonDeterministic) {
				return FormatCustomFunction(operands);
			}
			return BaseFormatterHelper.DefaultFormatFunction(operatorType, operands);
		}
		public virtual string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			string[] strings = new string[operands.Length];
			for(int i = 0; i < operands.Length; ++i) {
				if(i == 0 && (operatorType == FunctionOperatorType.Custom || operatorType == FunctionOperatorType.CustomNonDeterministic))
					strings[i] = (string)operands[i];
				else
					strings[i] = processParameter(operands[i]);
			}
			return FormatFunction(operatorType, strings);
		}
		public virtual string FormatOrder(string sortProperty, SortingDirection direction) {
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", sortProperty, direction == SortingDirection.Ascending ? "asc" : "desc");
		}
		public abstract string GetParameterName(OperandValue parameter, int index, ref bool createPrameter);
		public virtual string ComposeSafeTableName(string tableName) {
			int dot = tableName.IndexOf('.');
			if(dot > 0)
				tableName = tableName.Remove(0, dot + 1);
			return GetSafeObjectName(tableName, GetSafeNameRoot(tableName), GetSafeNameTableMaxLength());
		}
		public virtual string ComposeSafeSchemaName(string tableName) {
			int dot = tableName.IndexOf('.');
			if(dot > 0) {
				tableName = tableName.Substring(0, dot);
				return GetSafeObjectName(tableName, GetSafeNameRoot(tableName), GetSafeNameTableMaxLength());
			} else
				return String.Empty;
		}
		public virtual string ComposeSafeColumnName(string columnName) {
			return GetSafeObjectName(columnName, GetSafeNameRoot(columnName), GetSafeNameColumnMaxLength());
		}
		public virtual bool BraceJoin { get { return true; } }
		public virtual bool SupportNamedParameters { get { return true; } }
#endregion
		public abstract string FormatConstraint(string constraintName);
		protected static string GetSafeNameAccess(string originalName) {
			char[] result = originalName.ToCharArray();
			int len = result.Length;
			if(len == 0)
				return originalName;
			bool spacesToUnderscore = result[0] == ' ' || result[len - 1] == ' ';
			for(int i = 0; i < len; i++) {
				switch(result[i]) {
					case '\u0000':
					case '\u0001':
					case '\u0002':
					case '\u0003':
					case '\u0004':
					case '\u0005':
					case '\u0006':
					case '\u0007':
					case '\u0008':
					case '\u0009':
					case '\u000A':
					case '\u000B':
					case '\u000C':
					case '\u000D':
					case '\u000E':
					case '\u000F':
					case '\u0010':
					case '\u0011':
					case '\u0012':
					case '\u0013':
					case '\u0014':
					case '\u0015':
					case '\u0016':
					case '\u0017':
					case '\u0018':
					case '\u0019':
					case '\u001A':
					case '\u001B':
					case '\u001C':
					case '\u001D':
					case '\u001E':
					case '\u001F':
					case '[':
					case ']':
					case '!':
					case '.':
					case '`':
						result[i] = '_';
						break;
					case ' ':
						if(spacesToUnderscore)
							result[i] = '_';
						break;
				}
			}
			return new string(result);
		}
		protected static string GetSafeNameMsSql(string originalName) {
			char[] result = originalName.ToCharArray();
			int len = result.Length;
			for(int i = 0; i < len; i++) {
				switch(result[i]) {
					case '\0':
					case '\uFFFF':
					case '.':
					case '[':
					case ']':
					case '"':
						result[i] = '_';
						break;
					default:
						if(result[i].GetUnicodeCategory() == UnicodeCategory.Surrogate) {
							result[i] = '_';
						}
						break;
				}
			}
			return new string(result);
		}
		protected static string GetSafeNameDefault(string originalName) {
			char[] result = originalName.ToCharArray();
			int len = result.Length;
			if(len == 0)
				return originalName;
			bool spacesToUnderscore = result[0] == ' ' || result[len - 1] == ' ';
			for(int i = 0; i < len; i++) {
				switch(result[i].GetUnicodeCategory()) {
					case UnicodeCategory.Control:
					case UnicodeCategory.LineSeparator:
					case UnicodeCategory.ParagraphSeparator:
					case UnicodeCategory.ConnectorPunctuation:
					case UnicodeCategory.DashPunctuation:
					case UnicodeCategory.OpenPunctuation:
					case UnicodeCategory.ClosePunctuation:
					case UnicodeCategory.InitialQuotePunctuation:
					case UnicodeCategory.FinalQuotePunctuation:
					case UnicodeCategory.OtherPunctuation:
						result[i] = '_';
						break;
					case UnicodeCategory.SpaceSeparator:
						if(spacesToUnderscore)
							result[i] = '_';
						break;
				}
			}
			return new string(result);
		}
		protected virtual string GetSafeNameRoot(string originalName) {
			return GetSafeNameDefault(originalName);
		}
		protected abstract int GetSafeNameTableMaxLength();
		protected virtual int GetSafeNameColumnMaxLength() {
			return GetSafeNameTableMaxLength();
		}
		protected virtual int GetSafeNameConstraintMaxLength() {
			return GetSafeNameTableMaxLength();
		}
		public virtual string ComposeSafeConstraintName(string constraintName) {
			return GetSafeObjectName(constraintName, GetSafeNameRoot(constraintName), GetSafeNameConstraintMaxLength());
		}
		protected string GetSafeObjectName(string originalName, string patchedName, int maxLength) {
			if(originalName.Length <= maxLength && originalName == patchedName)
				return originalName;
			patchedName = patchedName.TrimEnd('_');
			if(patchedName.IndexOf("__") >= 0) {
				System.Text.StringBuilder builder = new System.Text.StringBuilder(patchedName.Length - 1);
				char prevChar = '\0';
				foreach(char ch in patchedName) {
					if(prevChar == '_' && ch == '_')
						continue;
					prevChar = ch;
					builder.Append(ch);
				}
				patchedName = builder.ToString();
			}
			string suffix = '_' + GetDbNameHashString(originalName);
			string prefix = patchedName;
			if(prefix.Length + suffix.Length > maxLength) {
				prefix = prefix.Substring(0, maxLength - suffix.Length);
			}
			return prefix + suffix;
		}
		protected string GetDbNameHashString(string dbName) {
			const int hashLength = 32;
			const int hashShift = 7;
			UInt32 hash = 0U;
			foreach(char ch in dbName) {
				UInt32 hashOverflow = hash >> (hashLength - hashShift);
				hash <<= hashShift;
				hash ^= hashOverflow;
				hash ^= (UInt32)ch;
			}
			return hash.ToString("X8", CultureInfo.InvariantCulture);
		}
		protected virtual string GetPrimaryKeyName(DBPrimaryKey cons, DBTable table) {
			if(cons.Name != null)
				return cons.Name;
			return "PK_" + table.Name;
		}
		protected virtual string GetForeignKeyName(DBForeignKey cons, DBTable table) {
			if(cons.Name != null)
				return cons.Name;
			string result = "FK_" + table.Name + '_';
			foreach(string col in cons.Columns) {
				result += col;
			}
			return result;
		}
		protected virtual string GetIndexName(DBIndex cons, DBTable table) {
			if(cons.Name != null)
				return cons.Name;
			string result = "i";
			foreach(string col in cons.Columns) {
				result += col;
			}
			result += '_' + table.Name;
			return result;
		}
		public string FormatColumnSafe(string columnName) {
			return FormatColumn(ComposeSafeColumnName(columnName));
		}
		public string FormatTableSafe(DBTable table) {
			return FormatTable(ComposeSafeSchemaName(table.Name), ComposeSafeTableName(table.Name));
		}
		public string FormatConstraintSafe(string constraintName) {
			return FormatConstraint(ComposeSafeConstraintName(constraintName));
		}
		object ICommandChannel.Do(string command, object args) {
			lock(SyncRoot) {
				return DoInternal(command, args);
			}
		}
		Query ProcessSqlQueryCommandArgument(object args) {			
			CommandChannelHelper.SqlQuery sqlQuery = args as DevExpress.Xpo.Helpers.CommandChannelHelper.SqlQuery;
			if(sqlQuery == null)
				throw new ArgumentException(string.Format(CommandChannelHelper.Message_CommandWrongParameterSet, CommandChannelHelper.Command_ExecuteNonQuerySQLWithParams));
			List<string> paramNames = new List<string>(sqlQuery.ParametersNames);
			for(int i = sqlQuery.ParametersNames.Length; i < sqlQuery.Parameters.Count; i++) {
				bool create = true;
				paramNames.Add(GetParameterName(sqlQuery.Parameters[i], i, ref create));
			}
			return new Query(sqlQuery.SqlCommand, sqlQuery.Parameters, paramNames);
		}
		protected virtual object DoInternal(string command, object args) {
			switch(command) {
				case CommandChannelHelper.Command_ExplicitBeginTransaction:
#if !SL
					if(args != null && args is IsolationLevel)
						ExplicitBeginTransaction((IsolationLevel)args);
					else
#endif
						ExplicitBeginTransaction();
					return null;
				case CommandChannelHelper.Command_ExplicitCommitTransaction:
					ExplicitCommitTransaction();
					return null;
				case CommandChannelHelper.Command_ExplicitRollbackTransaction:
					ExplicitRollbackTransaction();
					return null;
				case CommandChannelHelper.Command_ExecuteStoredProcedure: {
						DevExpress.Xpo.Helpers.CommandChannelHelper.SprocQuery query = args as DevExpress.Xpo.Helpers.CommandChannelHelper.SprocQuery;
						if(query == null)
							throw new ArgumentException(string.Format(CommandChannelHelper.Message_CommandWrongParameterSet, CommandChannelHelper.Command_ExecuteStoredProcedure));
						return FixDBNull(ExecuteSproc(query.SprocName, query.Parameters));
					}
				case CommandChannelHelper.Command_ExecuteStoredProcedureParametrized: {
						DevExpress.Xpo.Helpers.CommandChannelHelper.SprocQuery query = args as DevExpress.Xpo.Helpers.CommandChannelHelper.SprocQuery;
						if(query == null)
							throw new ArgumentException(string.Format(CommandChannelHelper.Message_CommandWrongParameterSet, CommandChannelHelper.Command_ExecuteStoredProcedureParametrized));
						return FixDBNull(ExecuteSprocParametrized(query.SprocName, query.Parameters));
					}
				case CommandChannelHelper.Command_ExecuteNonQuerySQL:
					return ExecSql(new Query((string)args));
				case CommandChannelHelper.Command_ExecuteNonQuerySQLWithParams: {						
						return ExecSql(ProcessSqlQueryCommandArgument(args));
					}
				case CommandChannelHelper.Command_ExecuteScalarSQL:
					return FixDBNullScalar(GetScalar(new Query((string)args)));
				case CommandChannelHelper.Command_ExecuteScalarSQLWithParams: {
						return FixDBNullScalar(GetScalar(ProcessSqlQueryCommandArgument(args)));
					}
				case CommandChannelHelper.Command_ExecuteQuerySQL:
					return FixDBNull(new SelectedData(SelectDataSimple(new Query((string)args), null, false)));
				case CommandChannelHelper.Command_ExecuteQuerySQLWithParams: {
						return FixDBNull(new SelectedData(SelectDataSimple(ProcessSqlQueryCommandArgument(args), null, false)));
					}
				case CommandChannelHelper.Command_ExecuteQuerySQLWithMetadata:
					return FixDBNull(new SelectedData(SelectDataSimple(new Query((string)args), null, true)));
				case CommandChannelHelper.Command_ExecuteQuerySQLWithMetadataWithParams: {
						return FixDBNull(new SelectedData(SelectDataSimple(ProcessSqlQueryCommandArgument(args), null, true)));
					}
				default:
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
			}
		}
		object FixDBNullScalar(object data) {
			return data is DBNull ? null : data;
		}
		SelectedData FixDBNull(SelectedData data) {
			if(data == null || data.ResultSet == null || data.ResultSet.Length == 0)
				return data;
			foreach(SelectStatementResult ssr in data.ResultSet) {
				if(ssr == null || ssr.Rows == null || ssr.Rows.Length == 0)
					continue;
				foreach(SelectStatementResultRow row in ssr.Rows) {
					if(row == null || row.Values == null || row.Values.Length == 0)
						continue;
					for(int i = 0; i < row.Values.Length; i++) {
						if(row.Values[i] is DBNull)
							row.Values[i] = null;
					}
				}
			}
			return data;
		}
#if DEBUGTEST
		public virtual SelectedData ExecuteSprocForTests(string sprocName, params OperandValue[] parameters) { return ExecuteSproc(sprocName, parameters); }
		public virtual SelectedData ExecuteSprocParametrizedForTests(string sprocName, params OperandValue[] parameters) { return ExecuteSprocParametrized(sprocName, parameters); }
#endif
		protected abstract void CommandBuilderDeriveParameters(IDbCommand command);
		protected virtual SelectedData ExecuteSprocInternal(IDbCommand command, IDataParameter returnParameter, List<IDataParameter> outParameters) {
			List<SelectStatementResult> selectStatmentResults = GetSelectedStatmentResults(command);
			if(outParameters.Count > 0) {
				selectStatmentResults.Add(new SelectStatementResult(
					outParameters.Select(op => new SelectStatementResultRow(new object[] { op.ParameterName, op.Value })).ToArray()));
			}
			if(returnParameter != null) {
				selectStatmentResults.Add(new SelectStatementResult(new SelectStatementResultRow[] {
					new SelectStatementResultRow(new object[] { returnParameter.Value })
				}));
			}
			return new SelectedData(selectStatmentResults.ToArray());
		}
		protected virtual SelectedData ExecuteSprocParametrized(string sprocName, params OperandValue[] parameters) {
			using(IDbCommand command = CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = FormatTable(ComposeSafeSchemaName(sprocName), ComposeSafeTableName(sprocName));
				IDataParameter returnParameter = null;
				List<IDataParameter> outParameters = new List<IDataParameter>();
				foreach(SprocParameter parameter in parameters) {
					IDbDataParameter dbParameter = (IDbDataParameter)CreateParameter(command, ConvertParameter(parameter.Value), parameter.ParameterName);
					switch(parameter.Direction) {
						case SprocParameterDirection.InputOutput:
						case SprocParameterDirection.Output:
							outParameters.Add(dbParameter);
							break;
						case SprocParameterDirection.ReturnValue:
							if(returnParameter != null)
								throw new ArgumentException("Duplicate return parameter.");
							returnParameter = dbParameter;
							break;
					}
					dbParameter.Direction = SprocParameter.GetDataParameterDirection(parameter.Direction);
					if(parameter.DbType.HasValue)
						dbParameter.DbType = GetDbType(parameter.DbType.Value);
					if(parameter.Size.HasValue)
						dbParameter.Size = parameter.Size.Value;
					if(parameter.Precision.HasValue)
						dbParameter.Precision = parameter.Precision.Value;
					if(parameter.Scale.HasValue)
						dbParameter.Scale = parameter.Scale.Value;
					command.Parameters.Add(dbParameter);
				}
				return ExecuteSprocInternal(command, returnParameter, outParameters);
			}
		}
		protected void PrepareParametersForExecuteSproc(OperandValue[] parameters, IDbCommand command, out List<IDataParameter> outParameters, out IDataParameter returnParameter) {
			int counter = 0;
			returnParameter = null;
			outParameters = new List<IDataParameter>();
			foreach(IDataParameter param in command.Parameters) {
				switch(param.Direction) {
					case ParameterDirection.ReturnValue:
						returnParameter = param;
						continue;
					case ParameterDirection.Output:
						outParameters.Add(param);
						param.Value = DBNull.Value;
						continue;
					case ParameterDirection.InputOutput:
						outParameters.Add(param);
						param.Value = DBNull.Value;
						break;
				}
				if(counter < parameters.Length) {
					param.Value = ConvertParameter(parameters[counter++].Value);
				}
			}
		}
		protected virtual SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			using(IDbCommand command = CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = FormatTable(ComposeSafeSchemaName(sprocName), ComposeSafeTableName(sprocName));
				CommandBuilderDeriveParameters(command);
				IDataParameter returnParameter;
				List<IDataParameter> outParameters;
				PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
				return ExecuteSprocInternal(command, returnParameter, outParameters);
			}
		}
		protected List<SelectStatementResult> GetSelectedStatmentResults(IDbCommand command) {
			using(IDataReader reader = command.ExecuteReader()) {
				List<SelectStatementResult> selectStatmentResults = new List<SelectStatementResult>();
				if(reader == null) {
					selectStatmentResults.Add(new SelectStatementResult(new SelectStatementResultRow[0]));
					return selectStatmentResults;
				}
				do {
					List<SelectStatementResultRow> rows = new List<SelectStatementResultRow>();
					while(reader.Read()) {
						Type[] fieldTypes = null;
						object[] values = new object[reader.FieldCount];
						if(IsFieldTypesNeeded && fieldTypes == null) {
							fieldTypes = new Type[reader.FieldCount];
							for(int i = reader.FieldCount - 1; i >= 0; i--) {
								fieldTypes[i] = reader.GetFieldType(i);
							}
						}
						GetValues(reader, fieldTypes, values);
						rows.Add(new SelectStatementResultRow(values));
					}
					selectStatmentResults.Add(new SelectStatementResult(rows.ToArray()));
				}
				while(reader.NextResult());
				return selectStatmentResults;
			}
		}
		public virtual bool NativeOuterApplySupported { get { return false; } }
		public virtual string FormatOuterApply(string sql, string alias) {
			return string.Format(CultureInfo.InvariantCulture, "outer apply ({0}) {1}", sql, alias);
		}
	}
}
