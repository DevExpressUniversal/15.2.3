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
using System.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP.AdoWrappers {
	public class AdomdConnection : ByVersionWrapper, IDbConnection, IOLAPConnection {
		public AdomdConnection(AdomdVersion version)
			: base(version, "Microsoft.AnalysisServices.AdomdClient.AdomdConnection") {
		}
		public AdomdConnection(AdomdVersion version, string connectionString) : this(version) {
			this.ConnectionString = connectionString;
		}
		public string ConnectionString {
			get { return (string)GetPropertyValue("ConnectionString"); }
			set { SetPropertyValue("ConnectionString", value); }
		}
		public string ServerVersion {
			get { return (string)GetPropertyValue("ServerVersion"); }
		}
		public string Database {
			get { return (string)GetPropertyValue("Database"); }
		}
		public int ConnectionTimeout {
			get { return (int)GetPropertyValue("ConnectionTimeout"); }
		}
		public string SessionID {
			get { return (string)GetPropertyValue("SessionID"); }
			set { SetPropertyValue("SessionID", value); }
		}
		public void Open() {
			InvokeMethod("Open");
		}
		public void Close(bool closeConnection) {
			InvokeMethod("Close", new object[] { closeConnection } );
		}
		public ConnectionState State {
			get { return (ConnectionState)GetPropertyValue("State"); }
		}
		public bool IsOpen { get { return State == ConnectionState.Open; } }
		public AdomdCommand CreateCommand(string mdx) {
			AdomdCommand command = new AdomdCommand(Version);
			command.CommandText = mdx;
			command.Connection = this;
			return command;
		}
		public DataSet GetSchemaDataSet(string schemaName, AdomdRestrictionCollection restrictions) {
			return (DataSet)InvokeMethod("GetSchemaDataSet", schemaName, restrictions.Instance);
		}
		public DataSet GetSchemaDataSet(Guid schema, object[] restrictions) {
			return (DataSet)InvokeMethod("GetSchemaDataSet", schema, restrictions);
		}
		public void ChangeDatabase(string database) {
			InvokeMethod("ChangeDatabase", database);
		}
		#region IDbConnection Members
		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il) {
			throw new NotImplementedException();
		}
		IDbTransaction IDbConnection.BeginTransaction() {
			throw new NotImplementedException();
		}
		void IDbConnection.ChangeDatabase(string databaseName) {
			throw new NotImplementedException();
		}
		void IDbConnection.Close() {
			this.Close(false);
		}
		string IDbConnection.ConnectionString {
			get { return this.ConnectionString; }
			set {
				if(this.ConnectionString == value) return;
				this.ConnectionString = value;
			}
		}
		int IDbConnection.ConnectionTimeout {
			get { throw new NotImplementedException(); }
		}
		IDbCommand IDbConnection.CreateCommand() {
			return this.CreateCommand(string.Empty);
		}
		string IDbConnection.Database {
			get { return this.Database; }
		}
		void IDbConnection.Open() {
			this.Open();
		}
		ConnectionState IDbConnection.State {
			get { throw new NotImplementedException(); }
		}
		#endregion
		#region IOLAPConnection Members
		IOLAPCommand IOLAPConnection.CreateCommand(string mdx) {
			return this.CreateCommand(mdx);
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			this.Close(false);
			base.Dispose();
		}
		#endregion
	}
	public class AdomdCommand : ByVersionWrapper, IDbCommand, IOLAPCommand {
		AdomdConnection connection;
		public AdomdCommand(AdomdVersion version)
			: base(version, "Microsoft.AnalysisServices.AdomdClient.AdomdCommand") {
		}
		public string CommandText {
			get { return (string)GetPropertyValue("CommandText"); }
			set { SetPropertyValue("CommandText", value); }
		}
		public int CommandTimeout {
			get { return (int)GetPropertyValue("CommandTimeout"); }
			set { SetPropertyValue("CommandTimeout", value); }
		}
		public AdomdConnection Connection {
			get {
				return connection; 
			}
			set {
				connection = value;
				SetPropertyValue("Connection", connection.Instance);
			}
		}
		public IDataReader ExecuteReader() {
			return (IDataReader)InvokeMethod("ExecuteReader");
		}
		#region IDbCommand Members
		void IDbCommand.Cancel() {
			throw new System.NotImplementedException();
		}
		CommandType IDbCommand.CommandType {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		IDbConnection IDbCommand.Connection {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		IDbDataParameter IDbCommand.CreateParameter() {
			throw new System.NotImplementedException();
		}
		int IDbCommand.ExecuteNonQuery() {
			return (int)InvokeMethod("ExecuteNonQuery");
		}
		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior) {
			throw new System.NotImplementedException();
		}
		IDataReader IDbCommand.ExecuteReader() {
			throw new System.NotImplementedException();
		}
		object IDbCommand.ExecuteScalar() {
			throw new System.NotImplementedException();
		}
		IDataParameterCollection IDbCommand.Parameters {
			get { throw new System.NotImplementedException(); }
		}
		void IDbCommand.Prepare() {
			throw new System.NotImplementedException();
		}
		IDbTransaction IDbCommand.Transaction {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		UpdateRowSource IDbCommand.UpdatedRowSource {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		#endregion
		#region IOLAPCommand Members
		IOLAPConnection IOLAPCommand.Connection {
			get { return connection; }
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			base.Dispose();
		}
		#endregion
		internal System.Xml.XmlReader ExecuteXmlReader() {
			return (System.Xml.XmlReader)InvokeMethod("ExecuteXmlReader");
		}
	}
}
