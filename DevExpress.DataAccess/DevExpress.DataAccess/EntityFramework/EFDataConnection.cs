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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.Entity.Model;
using DevExpress.Entity.ProjectModel;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
namespace DevExpress.DataAccess.EntityFramework {
	[TypeConverter("DevExpress.DataAccess.UI.Native.EntityFramework.EFDataConnectionTypeConverter," + AssemblyInfo.SRAssemblyDataAccessUI)]
	[Editor("DevExpress.DataAccess.UI.Native.EntityFramework.EFDataConnectionEditor, " + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
	public class EFDataConnection : IDataConnection {
		#region static
		static DBTable CreateDbTable(IEntitySetInfo es) {
			DBTable dbTable = new DBTable(es.Name);
			dbTable.Columns.AddRange(es.ElementType.AllProperties.Select(a => new DBColumn() {Name = a.DisplayName, DBTypeName = "string", ColumnType = DBColumn.GetColumnType(a.PropertyType, true)}));
			return dbTable;
		}
		static DBStoredProcedure CreateDbStoredProcedure(IEntityFunctionInfo ef) {
			DBStoredProcedure procedure = new DBStoredProcedure {Name = ef.Name};
			procedure.Arguments.AddRange(ef.Parameters.Select(p => new DBStoredProcedureArgument() {Name = p.Name, Type = DBColumn.GetColumnType(p.ClrType, true)}));
			if(ef.ResultTypeProperties != null)
				procedure.ResultSets.Add(new DBStoredProcedureResultSet(ef.ResultTypeProperties.Select(p => new DBNameTypePair(p.Name, DBColumn.GetColumnType(p.ClrType, true))).ToList()));
			return procedure;
		}
		static void ProcessException(Exception exception) {
			UnableToOpenDatabaseException unableToOpenException = exception as UnableToOpenDatabaseException;
			Exception innerException = exception;
			string message = exception.Message;
			if(unableToOpenException != null && unableToOpenException.InnerException != null) {
				innerException = unableToOpenException.InnerException;
				message = innerException.Message;
			}
			TargetInvocationException taex = exception as TargetInvocationException;
			if(taex != null && taex.InnerException != null) {
				innerException = taex.InnerException;
				message = taex.Message;
			}
			throw new EFConnectionException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.DatabaseConnectionExceptionStringId), message), innerException);
		}
		#endregion
		readonly EFConnectionParameters connectionParameters;
		DataAccessEntityFrameworkModel model;
		IConnectionStringsProvider connectionStringsProvider;
		DbContainerInfo containerInfo;
		bool connected;
		bool storeConnectionNameOnly;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool StoreConnectionNameOnly
		{
			get { return storeConnectionNameOnly; }
			set
			{
				storeConnectionNameOnly = value;
				if(ConnectionParameters != null)
					ConnectionParameters.ConnectionString = null;
			}
		}
		[Browsable(false)]
		public string Name { get; set; }
		public EFConnectionParameters ConnectionParameters { get { return connectionParameters; } }
		public string ConnectionString
		{
			get { return ConnectionParameters != null ? ConnectionParameters.ConnectionString : null; }
			set
			{
				if(ConnectionParameters != null)
					ConnectionParameters.ConnectionString = value;
			}
		}
		public bool IsConnected { get { return connected; } }
		public ISolutionTypesProvider SolutionTypesProvider { get; set; }
		public IConnectionStringsProvider ConnectionStringsProvider { get { return connectionStringsProvider ?? new DevExpress.DataAccess.Native.RuntimeConnectionStringsProvider(); } set { connectionStringsProvider = value; } }
		internal DbContainerInfo ContainerInfo { get { return containerInfo; } }
		internal DataAccessEntityFrameworkModel Model { get { return model; } }
		internal MethodInfo[] SourceMethods
		{
			get
			{
				if(ConnectionParameters == null || ConnectionParameters.Source == null)
					return null;
				MethodInfo[] methods = ConnectionParameters.Source.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
				return methods.Where(m => !m.IsSpecialName && m.ReturnType.GetInterfaces().Any(i => i == typeof(IEnumerable))).ToArray();
			}
		}
		public EFDataConnection(string name, EFConnectionParameters connectionParameters) {
			this.connectionParameters = connectionParameters;
			Name = name;
		}
		public void Dispose() {
			if(model != null) {
				model.Dispose();
				model = null;
			}
		}
		public void Open() {
			if(!IsConnected)
				CreateDataStoreCore();
		}
		public void Close() {
			model = null;
			connected = false;
		}
		public DBSchema GetDBSchema() {
			DbContainerInfo dbContainerInfo = ContainerInfo;
			if(dbContainerInfo == null)
				return new DBSchema();
			DBTable[] tables = dbContainerInfo.EntitySets.Where(es => es.IsView == false).Select(CreateDbTable).ToArray();
			DBTable[] views = dbContainerInfo.EntitySets.Where(es => es.IsView).Select(CreateDbTable).ToArray();
			DBStoredProcedure[] procs = dbContainerInfo.EntityFunctions.Where(f => f.ResultTypeProperties != null).Select(CreateDbStoredProcedure).ToArray();
			return new DBSchema(tables, views, procs);
		}
		public DBSchema GetDBSchema(string[] tableList) {
			DbContainerInfo dbContainerInfo = ContainerInfo;
			if(dbContainerInfo == null)
				return new DBSchema();
			DBTable[] tables = dbContainerInfo.EntitySets.Where(es => tableList.Contains(es.Name) && es.IsView == false).Select(CreateDbTable).ToArray();
			DBTable[] views = dbContainerInfo.EntitySets.Where(es => tableList.Contains(es.Name) && es.IsView).Select(CreateDbTable).ToArray();
			return new DBSchema(tables, views);
		}
		public EFConnectionParameters CreateDataConnectionParameters() {
			return new EFConnectionParameters(ConnectionParameters.Source, ConnectionParameters.ConnectionStringName, ConnectionParameters.ConnectionString);
		}
		public string CreateConnectionString() {
			return ConnectionString;
		}
		string IDataConnection.CreateConnectionString(bool blackoutCredentials) {
			if(blackoutCredentials)
				throw new NotSupportedException();
			return CreateConnectionString();
		}
		void IDataConnection.BlackoutCredentials() { throw new NotSupportedException(); }
		internal void CreateDataStore() {
			if(IsConnected)
				return;
			Exception exception = null;
			try {
				CreateDataStoreCore();
			} catch(Exception ex) {
				exception = ex;
			}
			if(IsConnected)
				return;
			if(exception != null)
				ProcessException(exception);
		}
		void CreateDataStoreCore() {
			if(model == null)
				model = new DataAccessEntityFrameworkModel(SolutionTypesProvider);
			else
				model.Exceptions.Clear();
			string connectionString;
			if(string.IsNullOrEmpty(ConnectionParameters.ConnectionString) && !string.IsNullOrEmpty(ConnectionParameters.ConnectionStringName)) {
				connectionString = ConnectionStringsProvider.GetConnectionString(ConnectionParameters.ConnectionStringName);
				if(string.IsNullOrEmpty(connectionString))
					throw new InvalidOperationException(string.Format("The application config file does not contain the specified connection: \"{0}\".", ConnectionParameters.ConnectionStringName));
			} else
				connectionString = ConnectionParameters.ConnectionString;
			model.ConnectionString = connectionString;
			DbContainerInfo dbContainerInfo = (DbContainerInfo)model.GetContainer(ConnectionParameters.Source.FullName, false);
			if(model.Exceptions.Count > 0) {
				throw new AggregateException(model.Exceptions);
			}
			containerInfo = dbContainerInfo;
			connected = containerInfo != null;
		}
	}
}
