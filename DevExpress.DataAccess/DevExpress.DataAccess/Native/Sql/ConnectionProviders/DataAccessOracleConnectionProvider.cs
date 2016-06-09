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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
namespace DevExpress.DataAccess.Native.Sql.ConnectionProviders {
	public class DataAccessOracleConnectionProvider : OracleConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportStoredProc, IDataStoreEx {
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessOracleConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessOracleConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("System.Data.OracleClient.OracleConnection", DataAccessCreateProviderFromConnection);
		}
		public static void ProviderRegister() {
		}
		public DataAccessOracleConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			string name = OracleConnectionProviderHelper.GetParameterName(parameter, index, ref createParameter);
			return name ?? base.GetParameterName(new ConstantValue(parameter.Value), index, ref createParameter);
		}
		protected override string GetSafeNameRoot(string originalName) {
			return originalName;
		}
		public override string ComposeSafeColumnName(string columnName) {
			return columnName;
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return base.FormatColumn(columnName, ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd));
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			string encloseAlias = ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd);
			return string.IsNullOrEmpty(encloseAlias) ? base.FormatTable(schema, tableName) : base.FormatTable(schema, tableName, encloseAlias);
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			string result = OracleConnectionProviderHelper.FormatFunction(processParameter, operatorType, operands);
			return result ?? base.FormatFunction(processParameter, operatorType, operands);
		}
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			using(IDbCommand command = CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = sprocName;
				CommandBuilderDeriveParameters(command);
				IDataParameter returnParameter;
				List<IDataParameter> outParameters;
				PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
				return ExecuteSprocInternal(command, returnParameter, outParameters);
			}
		}
		#region IAliasFormatter
		string IAliasFormatter.AliasLead { get { return aliasLead; } }
		string IAliasFormatter.AliasEnd { get { return aliasEnd; } }
		bool IAliasFormatter.SingleQuotedString { get { return singleQuotedString; } }
		int IAliasFormatter.MaxTableAliasLength { get { return GetSafeNameTableMaxLength(); } }
		int IAliasFormatter.MaxColumnAliasLength { get { return GetSafeNameColumnMaxLength(); } }
		#endregion
		#region IDataStoreSchemaExplorerEx Members
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			return OracleConnectionProviderHelper.GetStorageDataObjectsNames(this, this.SysUsersAvailable, this.ObjectsOwner, types);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageTables(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(bool includeColumns, params string[] tablesList) {
			return OracleConnectionProviderHelper.GetStorageDataObjects(this, this.SysUsersAvailable, this.ObjectsOwner, includeColumns, false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageViews(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(bool includeColumns, params string[] tablesList) {
			return OracleConnectionProviderHelper.GetStorageDataObjects(this, this.SysUsersAvailable, this.ObjectsOwner, includeColumns, true, tablesList);
		}
		void IDataStoreSchemaExplorerEx.GetColumns(params DBTable[] tables) {
			OracleConnectionProviderHelper.GetStorageTablesColumns(this, this.SysUsersAvailable, this.ObjectsOwner, tables, true);
		}
		#endregion
		#region ISupportStoredProc Members
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			return OracleConnectionProviderHelper.GetStoredProcedures(this, this.SysUsersAvailable, procedureNames);
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			return OracleConnectionProviderHelper.GetStoredProcedureTableSchema(this, CommandBuilderDeriveParameters, procedureName);
		}
		#endregion
		#region IDataStoreEx
		void IDataStoreEx.ProcessQuery(CancellationToken cancellationToken, Query query, Action<IDataReader, CancellationToken> action) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		void IDataStoreEx.ProcessStoredProc(CancellationToken cancellationToken, string sprocName, Action<IDataReader, CancellationToken> action, params OperandValue[] parameters) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		SelectedDataEx IDataStoreEx.SelectData(CancellationToken cancellationToken, Query query, string[] columns) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetData(command, cancellationToken, columns, NativeSkipTakeSupported, query.SkipSelectedRecords);
				}
			}
		}
		ColumnInfoEx[] IDataStoreEx.SelectSchema(Query query) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetSchema(command);
				}
			}
		}
		SelectedDataEx IDataStoreEx.ExecuteStoredProcedure(CancellationToken cancellationToken, string sprocName, params OperandValue[] parameters) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					return DataStoreExHelper.GetData(command, cancellationToken);
				}
			}
		}
		void PrepareCommandForStoredProc(IDbCommand command, string sprocName, OperandValue[] parameters) {
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = sprocName;
			CommandBuilderDeriveParameters(command);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
		}
		#endregion
	}
	public class DataAccessODPConnectionProvider : ODPConnectionProvider, IAliasFormatter, IDataStoreSchemaExplorerEx, ISupportStoredProc, IDataStoreEx {
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		bool? isODP10;
		public bool IsODP10 {
			get {
				if(this.isODP10 == null) {
					Type odpConnectionType = Connection.GetType();
					AssemblyName name = new AssemblyName(odpConnectionType.Assembly.FullName);
					this.isODP10 = name.Version.Major > 9 || ((name.Version.Major == 2 || name.Version.Major == 4) && name.Version.Minor >= 100);
				}
				return this.isODP10 ?? false;
			}
		}
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessODPConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessODPConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("Oracle.DataAccess.Client.OracleConnection", DataAccessCreateProviderFromConnection);
			RegisterFactory(new DataAccessODPProviderFactory());
		}
		public static void ProviderRegister() {
		}
		public DataAccessODPConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			string name = OracleConnectionProviderHelper.GetParameterName(parameter, index, ref createParameter);
			return name ?? base.GetParameterName(new ConstantValue(parameter.Value), index, ref createParameter);
		}
		protected override string GetSafeNameRoot(string originalName) {
			return originalName;
		}
		public override string ComposeSafeColumnName(string columnName) {
			return columnName;
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return base.FormatColumn(columnName, ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd));
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			string encloseAlias = ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd);
			return string.IsNullOrEmpty(encloseAlias) ? base.FormatTable(schema, tableName) : base.FormatTable(schema, tableName, encloseAlias);
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			string result = OracleConnectionProviderHelper.FormatFunction(processParameter, operatorType, operands);
			return result ?? base.FormatFunction(processParameter, operatorType, operands);
		}
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			if(!IsODP10)
				return base.ExecuteSproc(sprocName, parameters);
			using(IDbCommand command = CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = sprocName;
				CommandBuilderDeriveParameters(command);
				IDataParameter returnParameter;
				List<IDataParameter> outParameters;
				PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
				return ExecuteSprocInternal(command, returnParameter, outParameters);
			}
		}
		#region IAliasFormatter
		string IAliasFormatter.AliasLead { get { return aliasLead; } }
		string IAliasFormatter.AliasEnd { get { return aliasEnd; } }
		bool IAliasFormatter.SingleQuotedString { get { return singleQuotedString; } }
		int IAliasFormatter.MaxTableAliasLength { get { return GetSafeNameTableMaxLength(); } }
		int IAliasFormatter.MaxColumnAliasLength { get { return GetSafeNameColumnMaxLength(); } }
		#endregion
		#region IDataStoreSchemaExplorerEx Members
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			return OracleConnectionProviderHelper.GetStorageDataObjectsNames(this, this.SysUsersAvailable, this.ObjectsOwner, types);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageTables(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(bool includeColumns, params string[] tablesList) {
			return OracleConnectionProviderHelper.GetStorageDataObjects(this, this.SysUsersAvailable, this.ObjectsOwner, includeColumns, false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageViews(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(bool includeColumns, params string[] tablesList) {
			return OracleConnectionProviderHelper.GetStorageDataObjects(this, this.SysUsersAvailable, this.ObjectsOwner, includeColumns, true, tablesList);
		}
		void IDataStoreSchemaExplorerEx.GetColumns(params DBTable[] tables) {
			OracleConnectionProviderHelper.GetStorageTablesColumns(this, this.SysUsersAvailable, this.ObjectsOwner, tables, true);
		}
		#endregion
		#region ISupportStoredProc Members
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			return OracleConnectionProviderHelper.GetStoredProcedures(this, this.SysUsersAvailable, procedureNames);
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			return OracleConnectionProviderHelper.GetStoredProcedureTableSchema(this, CommandBuilderDeriveParameters, procedureName);
		}
		#endregion
		#region IDataStoreEx
		void IDataStoreEx.ProcessQuery(CancellationToken cancellationToken, Query query, Action<IDataReader, CancellationToken> action) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		void IDataStoreEx.ProcessStoredProc(CancellationToken cancellationToken, string sprocName, Action<IDataReader, CancellationToken> action, params OperandValue[] parameters) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		SelectedDataEx IDataStoreEx.SelectData(CancellationToken cancellationToken, Query query, string[] columns) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetData(command, cancellationToken, columns, NativeSkipTakeSupported, query.SkipSelectedRecords);
				}
			}
		}
		ColumnInfoEx[] IDataStoreEx.SelectSchema(Query query) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetSchema(command);
				}
			}
		}
		SelectedDataEx IDataStoreEx.ExecuteStoredProcedure(CancellationToken cancellationToken, string sprocName, params OperandValue[] parameters) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					return DataStoreExHelper.GetData(command, cancellationToken);
				}
			}
		}
		void PrepareCommandForStoredProc(IDbCommand command, string sprocName, OperandValue[] parameters) {
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = sprocName;
			CommandBuilderDeriveParameters(command);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
		}
		#endregion
	}
	public class DataAccessODPManagedConnectionProvider : ODPManagedConnectionProvider, IAliasFormatter, ISupportStoredProc, IDataStoreSchemaExplorerEx, IDataStoreEx {
		const string aliasLead = "\"";
		const string aliasEnd = "\"";
		const bool singleQuotedString = true;
		bool? isODP10;
		public bool IsODP10 {
			get {
				if(this.isODP10 == null) {
					Type odpConnectionType = Connection.GetType();
					AssemblyName name = new AssemblyName(odpConnectionType.Assembly.FullName);
					this.isODP10 = name.Version.Major > 9 || ((name.Version.Major == 2 || name.Version.Major == 4) && name.Version.Minor >= 100);
				}
				return this.isODP10 ?? false;
			}
		}
		[SuppressMessage("Microsoft.Design", "CA1021")]
		public static IDataStore DataAccessCreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] {connection};
			return DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore DataAccessCreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new DataAccessODPManagedConnectionProvider(connection, autoCreateOption);
		}
		static DataAccessODPManagedConnectionProvider() {
			Register();
			RegisterDataStoreProvider(XpoProviderTypeString, DataAccessCreateProviderFromString);
			RegisterDataStoreProvider("Oracle.ManagedDataAccess.Client.OracleConnection", DataAccessCreateProviderFromConnection);
		}
		public static void ProviderRegister() {
		}
		public DataAccessODPManagedConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			string name = OracleConnectionProviderHelper.GetParameterName(parameter, index, ref createParameter);
			return name ?? base.GetParameterName(new ConstantValue(parameter.Value), index, ref createParameter);
		}
		protected override string GetSafeNameRoot(string originalName) {
			return originalName;
		}
		public override string ComposeSafeColumnName(string columnName) {
			return columnName;
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return base.FormatColumn(columnName, ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd));
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			if(tableName == SubSelectStatement.CustomSqlString)
				return tableAlias;
			string encloseAlias = ConnectionProviderHelper.EncloseAlias(tableAlias, aliasLead, aliasEnd);
			return string.IsNullOrEmpty(encloseAlias) ? base.FormatTable(schema, tableName) : base.FormatTable(schema, tableName, encloseAlias);
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			string result = OracleConnectionProviderHelper.FormatFunction(processParameter, operatorType, operands);
			return result ?? base.FormatFunction(processParameter, operatorType, operands);
		}
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			if(!IsODP10)
				return base.ExecuteSproc(sprocName, parameters);
			using(IDbCommand command = CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = sprocName;
				CommandBuilderDeriveParameters(command);
				IDataParameter returnParameter;
				List<IDataParameter> outParameters;
				PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
				return ExecuteSprocInternal(command, returnParameter, outParameters);
			}
		}
		#region IAliasFormatter
		string IAliasFormatter.AliasLead { get { return aliasLead; } }
		string IAliasFormatter.AliasEnd { get { return aliasEnd; } }
		bool IAliasFormatter.SingleQuotedString { get { return singleQuotedString; } }
		int IAliasFormatter.MaxTableAliasLength { get { return GetSafeNameTableMaxLength(); } }
		int IAliasFormatter.MaxColumnAliasLength { get { return GetSafeNameColumnMaxLength(); } }
		#endregion
		#region IDataStoreSchemaExplorerEx Members
		string[] IDataStoreSchemaExplorerEx.GetStorageDataObjectsNames(DataObjectTypes types) {
			return OracleConnectionProviderHelper.GetStorageDataObjectsNames(this, this.SysUsersAvailable, this.ObjectsOwner, types);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageTables(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageTables(bool includeColumns, params string[] tablesList) {
			return OracleConnectionProviderHelper.GetStorageDataObjects(this, this.SysUsersAvailable, this.ObjectsOwner, includeColumns, false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(params string[] tablesList) {
			return ((IDataStoreSchemaExplorerEx)this).GetStorageViews(false, tablesList);
		}
		DBTable[] IDataStoreSchemaExplorerEx.GetStorageViews(bool includeColumns, params string[] tablesList) {
			return OracleConnectionProviderHelper.GetStorageDataObjects(this, this.SysUsersAvailable, this.ObjectsOwner, includeColumns, true, tablesList);
		}
		void IDataStoreSchemaExplorerEx.GetColumns(params DBTable[] tables) {
			OracleConnectionProviderHelper.GetStorageTablesColumns(this, this.SysUsersAvailable, this.ObjectsOwner, tables, true);
		}
		#endregion
		#region ISupportStoredProc Members
		DBStoredProcedure[] ISupportStoredProc.GetStoredProcedures(params string[] procedureNames) {
			return OracleConnectionProviderHelper.GetStoredProcedures(this, this.SysUsersAvailable, procedureNames);
		}
		DBStoredProcedureResultSet ISupportStoredProc.GetStoredProcedureTableSchema(string procedureName) {
			return OracleConnectionProviderHelper.GetStoredProcedureTableSchema(this, CommandBuilderDeriveParameters, procedureName);
		}
		#endregion
		#region IDataStoreEx
		void IDataStoreEx.ProcessQuery(CancellationToken cancellationToken, Query query, Action<IDataReader, CancellationToken> action) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		void IDataStoreEx.ProcessStoredProc(CancellationToken cancellationToken, string sprocName, Action<IDataReader, CancellationToken> action, params OperandValue[] parameters) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					using(IDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess)) {
						action(reader, cancellationToken);
					}
				}
			}
		}
		SelectedDataEx IDataStoreEx.SelectData(CancellationToken cancellationToken, Query query, string[] columns) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetData(command, cancellationToken, columns, NativeSkipTakeSupported, query.SkipSelectedRecords);
				}
			}
		}
		ColumnInfoEx[] IDataStoreEx.SelectSchema(Query query) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand(query)) {
					return DataStoreExHelper.GetSchema(command);
				}
			}
		}
		SelectedDataEx IDataStoreEx.ExecuteStoredProcedure(CancellationToken cancellationToken, string sprocName, params OperandValue[] parameters) {
			lock(SyncRoot) {
				using(IDbCommand command = CreateCommand()) {
					PrepareCommandForStoredProc(command, sprocName, parameters);
					return DataStoreExHelper.GetData(command, cancellationToken);
				}
			}
		}
		void PrepareCommandForStoredProc(IDbCommand command, string sprocName, OperandValue[] parameters) {
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = sprocName;
			CommandBuilderDeriveParameters(command);
			IDataParameter returnParameter;
			List<IDataParameter> outParameters;
			PrepareParametersForExecuteSproc(parameters, command, out outParameters, out returnParameter);
		}
		#endregion
	}
	public class DataAccessODPProviderFactory : OracleProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return DataAccessODPConnectionProvider.DataAccessCreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return DataAccessODPConnectionProvider.DataAccessCreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
	}
	static class OracleConnectionProviderHelper {
		public static string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			switch(operatorType) {
				case FunctionOperatorType.StartsWith:
					if(operands[1] is OperandParameter)
						return string.Format(CultureInfo.InvariantCulture, "(Left({0}, Len({1})) = ({1}))",
							processParameter(operands[0]), processParameter(operands[1]));
					break;
			}
			return null;
		}
		public static string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			bool shouldCreateParameter = createParameter;
			OperandParameter param = parameter as OperandParameter;
			if(!ReferenceEquals(param, null)) {
				createParameter = true;
				return string.Format(":{0}", param.ParameterName);
			}
			object value = parameter.Value;
			if(value == null)
				return null;
			createParameter = false;
			switch(Type.GetTypeCode(value.GetType())) {
				case TypeCode.Decimal:
					return ((decimal)value).ToString(CultureInfo.InvariantCulture);
				case TypeCode.DateTime:
					return string.Format("to_date('{0}', 'dd.mm.yyyy hh24:mi:ss')", ((DateTime)value).ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture));
			}
			createParameter = shouldCreateParameter;
			return null;
		}
		public static string[] GetStorageDataObjectsNames(IDataStoreEx provider, bool sysUsersAvailable, string objectsOwner, DataObjectTypes types) {
			string queryTextTablesSys = String.Format(@"select CASE WHEN o.OWNER IS NOT NULL AND o.OWNER <> '{0}' then o.OWNER || '.' ELSE '' END || o.TABLE_NAME as full_table_name
                                                        from SYS.All_TABLES o
                                                          inner join SYS.USER$ u on o.""OWNER"" = u.""NAME""
                                                        where u.""NAME"" <> 'SYS' and u.""NAME"" <> 'SYSTEM' and u.""TYPE#"" = 1 and u.ASTATUS = 0", objectsOwner);
			string queryTextViewsSys = String.Format(@"select CASE WHEN o.OWNER IS NOT NULL AND o.OWNER <> '{0}' then o.OWNER || '.' ELSE '' END || o.VIEW_NAME as full_view_name
                                                       from SYS.All_VIEWS o
                                                         inner join SYS.USER$ u on o.""OWNER"" = u.""NAME""
                                                       where u.""NAME"" <> 'SYS' and u.""NAME"" <> 'SYSTEM' and u.""TYPE#"" = 1 and u.ASTATUS = 0", objectsOwner);
			const string queryTextTables = @"select TABLE_NAME as full_table_name from USER_TABLES";
			const string queryTextViews = @"select VIEW_NAME as full_view_name from USER_VIEWS";
			StringBuilder queryText = new StringBuilder();
			if((types & DataObjectTypes.Tables) != 0)
				queryText.Append(sysUsersAvailable ? queryTextTablesSys : queryTextTables);
			if((types & DataObjectTypes.Tables) != 0 && (types & DataObjectTypes.Views) != 0)
				queryText.Append(" union ");
			if((types & DataObjectTypes.Views) != 0)
				queryText.Append(sysUsersAvailable ? queryTextViewsSys : queryTextViews);
			Query getTablesDetails = new Query(queryText.ToString());
			List<string> result = new List<string>();
			provider.ProcessQuery(CancellationToken.None, getTablesDetails, (reader, cancellationToken)=> {
				while(reader.Read()) {
					result.Add(reader.GetString(0));
				}
			});
			return result.ToArray();
		}
		public static DBTable[] GetStorageDataObjects(IDataStoreEx provider, bool sysUsersAvailable, string objectsOwner, bool includeColumns, bool isView, params string[] tablesList) {
			string[] tableNames = GetStorageDataObjectsNames(provider, sysUsersAvailable, objectsOwner, isView ? DataObjectTypes.Views : DataObjectTypes.Tables);
			IEnumerable<string> filteredTables = tablesList.Length == 0 ? tableNames : tableNames.Where(n => tablesList.Any(f => f == n));
			DBTable[] tables = filteredTables.Select(p => ConnectionProviderHelper.CreateTable(p, isView)).ToArray();
			if(includeColumns)
				GetStorageTablesColumns(provider, sysUsersAvailable, objectsOwner, tables, tablesList.Length != 0);
			GetStorageTablesForeignKeys(provider, sysUsersAvailable, objectsOwner, tables, tablesList.Length != 0);
			return tables;
		}
		public static void GetStorageTablesColumns(IDataStoreEx provider, bool sysUsersAvailable, string objectsOwner, ICollection<DBTable> tables, bool useTablesFilter) {
			if(useTablesFilter && tables.Count == 0)
				return;
			string getTablesColumnsQuery;
			if(sysUsersAvailable)
				getTablesColumnsQuery = string.Format(@"with table_columns as (
                                                         SELECT   CASE WHEN c.OWNER IS NOT NULL AND c.OWNER <> '{0}' then c.OWNER || '.' ELSE '' END || c.TABLE_NAME as full_table_name,
                                                                  COLUMN_NAME, DATA_TYPE, CHAR_COL_DECL_LENGTH, DATA_PRECISION, DATA_SCALE
                                                         FROM     ALL_TAB_COLUMNS c
                                                             join SYS.USER$ u on c.""OWNER"" = u.""NAME""
                                                         where u.""NAME"" <> 'SYS' and u.""NAME"" <> 'SYSTEM' and u.""TYPE#"" = 1 and u.ASTATUS = 0
                                                         ORDER BY TABLE_NAME, COLUMN_ID)
                                                        select * from table_columns {1}", objectsOwner, GetTablesFilter(tables, "where LOWER(full_table_name)", useTablesFilter));
			else
				getTablesColumnsQuery = string.Format(@"SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHAR_COL_DECL_LENGTH, DATA_PRECISION, DATA_SCALE
                                                        FROM     USER_TAB_COLUMNS {0}
                                                        ORDER BY TABLE_NAME, COLUMN_ID", GetTablesFilter(tables, "where LOWER(TABLE_NAME)", useTablesFilter));
			provider.ProcessQuery(CancellationToken.None, new Query(getTablesColumnsQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string tableName = reader.GetString(0);
					if(table == null || !string.Equals(table.Name, tableName))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, tableName));
					if(table == null)
						continue;
					string columnName = reader.GetString(1);
					string typeName = reader.GetString(2);
					int size = !reader.IsDBNull(3) ? ((IConvertible)reader.GetValue(3)).ToInt32(CultureInfo.InvariantCulture) : 0;
					int precision = !reader.IsDBNull(4) ? ((IConvertible)reader.GetValue(4)).ToInt32(CultureInfo.InvariantCulture) : 0;
					int scale = !reader.IsDBNull(5) ? ((IConvertible)reader.GetValue(5)).ToInt32(CultureInfo.InvariantCulture) : 0;
					DBColumnType type = GetColumnType(typeName, size, precision, scale);
					DBColumn column = ConnectionProviderHelper.CreateColumn(columnName, false, false, type == DBColumnType.String ? size : 0, type);
					table.AddColumn(column);
				}
			});
		}
		public static void GetStorageTablesForeignKeys(IDataStoreEx provider, bool sysUsersAvailable, string objectsOwner, ICollection<DBTable> tables, bool useTablesFilter) {
			if(useTablesFilter && tables.Count == 0)
				return;
			string getTablesForeignKeysQuery;
			if(sysUsersAvailable)
				getTablesForeignKeysQuery = string.Format(@"with table_primary_keys as (
                                                             select CASE WHEN c.OWNER IS NOT NULL AND c.OWNER <> '{0}' then c.OWNER || '.' ELSE '' END || c.TABLE_NAME as full_table_name,
                                                                    CASE WHEN fc.OWNER IS NOT NULL AND fc.OWNER <> '{0}' then fc.OWNER || '.' ELSE '' END || fc.TABLE_NAME as primary_table_name,
                                                                    tc.COLUMN_NAME, fc.COLUMN_NAME as PRIMARY_COLUMN_NAME, c.CONSTRAINT_NAME
                                                             from ALL_CONSTRAINTS c
                                                              join ALL_CONS_COLUMNS tc on c.CONSTRAINT_NAME = tc.CONSTRAINT_NAME and c.owner = tc.owner
                                                              join ALL_CONS_COLUMNS fc on c.R_CONSTRAINT_NAME = fc.CONSTRAINT_NAME and c.owner = fc.owner and tc.POSITION = fc.POSITION 
                                                              join SYS.USER$ u on c.""OWNER"" = u.""NAME""
                                                             where u.""NAME"" <> 'SYS' and u.""NAME"" <> 'SYSTEM' and u.""TYPE#"" = 1 and u.ASTATUS = 0
                                                             order by c.CONSTRAINT_NAME, tc.POSITION)
                                                            select * from table_primary_keys {1}", objectsOwner, GetTablesFilter(tables, "where LOWER(full_table_name)", useTablesFilter));
			else
				getTablesForeignKeysQuery = string.Format(@"select c.TABLE_NAME, fc.TABLE_NAME as PRIMARY_TABLE_NAME, tc.COLUMN_NAME, fc.COLUMN_NAME as PRIMARY_COLUMN_NAME, c.CONSTRAINT_NAME
                                                            from USER_CONSTRAINTS c
                                                             join USER_CONS_COLUMNS tc on tc.CONSTRAINT_NAME = c.CONSTRAINT_NAME 
                                                             join USER_CONS_COLUMNS fc on c.R_CONSTRAINT_NAME = fc.CONSTRAINT_NAME and tc.POSITION = fc.POSITION 
                                                            {0}
                                                            order by c.CONSTRAINT_NAME, tc.POSITION", GetTablesFilter(tables, "where LOWER(c.TABLE_NAME)", useTablesFilter));
			provider.ProcessQuery(CancellationToken.None, new Query(getTablesForeignKeysQuery), (reader, cancellationToken)=> {
				DBTable table = null;
				while(reader.Read()) {
					string foreignTableName = reader.GetString(0);
					string primaryTableName = reader.GetString(1);
					if(table == null || !string.Equals(table.Name, foreignTableName, StringComparison.InvariantCultureIgnoreCase))
						table = tables.FirstOrDefault(t => string.Equals(t.Name, foreignTableName, StringComparison.InvariantCultureIgnoreCase));
					if(table == null)
						continue;
					string foreignColumnName = reader.GetString(2);
					string primaryColumnName = reader.GetString(3);
					string keyName = reader.GetString(4);
					DBForeignKey fk = table.ForeignKeys.FirstOrDefault(k => string.Equals(k.Name, keyName, StringComparison.InvariantCultureIgnoreCase));
					if(fk == null) {
						fk = new DBForeignKey {
							Name = keyName, PrimaryKeyTable = primaryTableName
						};
						table.ForeignKeys.Add(fk);
					}
					fk.Columns.Add(foreignColumnName);
					fk.PrimaryKeyTableKeyColumns.Add(primaryColumnName);
				}
			});
		}
		public static DBStoredProcedure[] GetStoredProcedures(IDataStoreEx provider, bool sysUsersAvailable, params string[] procedureNames) {
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			if(procedureNames.Length == 0) {
				IEnumerable<string> allProcedureNames = GetProcedureNames(provider, sysUsersAvailable);
				result.AddRange(allProcedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));
			} else
				result.AddRange(procedureNames.Select(name => ConnectionProviderHelper.CreateDBStoredProcedure(name)));
			List<DBStoredProcedure> forSkip = new List<DBStoredProcedure>();
			foreach(DBStoredProcedure procedure in result) {
				string[] procedureSchema = procedure.Name.Split('.');
				string procedureName = procedureSchema.Last();
				string packageName;
				StringBuilder queryText = new StringBuilder("SELECT argument_name, data_type, data_length, data_precision, data_scale, in_out FROM ");
				if(sysUsersAvailable) {
					packageName = procedureSchema.Length == 2 ? String.Empty : procedureSchema[1];
					queryText.Append("all_arguments");
				} else {
					packageName = procedureSchema.Length == 1 ? String.Empty : procedureSchema[0];
					queryText.Append("user_arguments");
				}
				queryText.Append(String.Format(" where object_name = '{0}'", procedureName));
				if(!String.IsNullOrEmpty(packageName))
					queryText.Append(String.Format(" and package_name = '{0}'", packageName));
				queryText.Append(" ORDER BY Position");
				bool skipProc = false;
				DBStoredProcedure storedProcedure = procedure;
				provider.ProcessQuery(CancellationToken.None, new Query(queryText.ToString()), (reader, cancellationToken)=> {
					while(reader.Read()) {
						DBStoredProcedureArgument argument = new DBStoredProcedureArgument();
						int size = reader["data_length"] == DBNull.Value ? 0 : Convert.ToInt32(reader["data_length"]);
						int precision = reader["data_precision"] == DBNull.Value ? 0 : Convert.ToInt32(reader["data_precision"]);
						int scale = reader["data_scale"] == DBNull.Value ? 0 : Convert.ToInt32(reader["data_scale"]);
						if(reader["data_type"] == DBNull.Value) {
							skipProc = true;
							break;
						}
						argument.Name = reader["argument_name"] == DBNull.Value ? String.Empty : (string)reader["argument_name"];
						argument.Type = GetColumnType((string)reader["data_type"], size, precision, scale);
						string direction = ((string)reader["in_out"]).ToUpper();
						switch(direction) {
							case "IN":
								argument.Direction = DBStoredProcedureArgumentDirection.In;
								break;
							case "OUT":
								argument.Direction = DBStoredProcedureArgumentDirection.Out;
								break;
							default:
								argument.Direction = DBStoredProcedureArgumentDirection.InOut;
								break;
						}
						storedProcedure.Arguments.Add(argument);
					}
					if(skipProc)
						forSkip.Add(storedProcedure);
				});
			}
			forSkip.ForEach(x => result.Remove(x));
			return result.ToArray();
		}
		public static DBStoredProcedureResultSet GetStoredProcedureTableSchema(ConnectionProviderSql provider, Action<IDbCommand> commandBuilderDeriveParameters, string procedureName) {
			DBStoredProcedureResultSet result = new DBStoredProcedureResultSet();
			using(IDbCommand command = provider.CreateCommand()) {
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = procedureName;
				commandBuilderDeriveParameters(command);
				using(IDataReader reader = command.ExecuteReader()) {
					DataTable table = reader.GetSchemaTable();
					if(table != null)
						foreach(DataRow row in table.Rows) {
							DBColumnType type = DBColumn.GetColumnType(row["DataType"] as Type);
							result.Columns.Add(new DBNameTypePair(row["ColumnName"].ToString(), type));
						}
				}
			}
			return result;
		}
		public static DBColumnType GetColumnType(string typeName, int size, int precision, int scale) {
			switch(typeName.ToLower()) {
				case "int":
					return DBColumnType.Int32;
				case "blob":
				case "raw":
					return DBColumnType.ByteArray;
				case "number":
					if(precision == 0 || scale != 0)
						return DBColumnType.Decimal;
					if(precision < 3)
						return DBColumnType.Byte;
					if(precision < 5)
						return DBColumnType.Int16;
					if(precision < 10)
						return DBColumnType.Int32;
					if(precision < 20)
						return DBColumnType.Int64;
					return DBColumnType.Decimal;
				case "nchar":
				case "char":
					if(size > 1)
						return DBColumnType.String;
					return DBColumnType.Char;
				case "money":
					return DBColumnType.Decimal;
				case "float":
					return DBColumnType.Double;
				case "nvarchar":
				case "varchar":
				case "varchar2":
				case "nvarchar2":
					return DBColumnType.String;
				case "date":
				case "timestamp":
					return DBColumnType.DateTime;
				case "clob":
				case "nclob":
					return DBColumnType.String;
			}
			if(typeName.ToLowerInvariant().StartsWith("timestamp"))
				return DBColumnType.DateTime;
			return DBColumnType.Unknown;
		}
		static string GetTablesFilter(ICollection<DBTable> tables, string prefix, bool useTablesFilter) {
			return (useTablesFilter && tables.Count != 0)
				? string.Format("{0} in ({1})", prefix, string.Join(", ", tables.Select(n => string.Format("'{0}'", n.Name.ToLowerInvariant()))))
				: string.Empty;
		}
		static IEnumerable<string> GetProcedureNames(IDataStoreEx provider, bool sysUsersAvailable) {
			List<string> procedureNames = new List<string>();
			string queryText = sysUsersAvailable
				? @"SELECT concat(o.OWNER, nvl2(o.procedure_name, concat('.', concat(o.object_name, concat('.', o.procedure_name))),concat('.', o.object_name))) as ProcedureName
                    from SYS.all_procedures o inner join SYS.USER$ u on o.OWNER = u.NAME where o.owner not in ('SYS', 'SYSTEM') and u.type# = 1 and u.ASTATUS = 0"
				: @"SELECT object_name as ProcedureName
                    FROM User_Procedures
                    where procedure_name is null
                    union
                    SELECT concat(concat(object_name, '.'), procedure_name) as ProcedureName
                    FROM User_Procedures where procedure_name <> '(null)'";
			provider.ProcessQuery(CancellationToken.None, new Query(queryText), (reader, cancellationToken)=> {
				while(reader.Read()) {
					if(!reader.IsDBNull(0))
						procedureNames.Add(reader.GetString(0));
				}
			});
			return procedureNames;
		}
	}
}
