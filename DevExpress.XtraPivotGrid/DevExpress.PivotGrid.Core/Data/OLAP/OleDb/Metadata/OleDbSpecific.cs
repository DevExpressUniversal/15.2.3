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
using System.Data.OleDb;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
	class OleConnection : IOLAPConnection {
		OleDbConnection connection;
		internal OleConnection(string connectionString) {
			this.connection = new OleDbConnection(connectionString);
		}
		internal OleDbConnection Connection { get { return this.connection; } }
		internal DataTable GetOleDbSchemaTable(Guid schema, object[] restrictions) {
			return connection.GetOleDbSchemaTable(schema, restrictions);
		}
		internal string ServerVersion { get { return connection.ServerVersion; } }
		internal string ConnectionString {
			get { return connection.ConnectionString; }
		}
		#region IOLAPConnection Members
		string IOLAPConnection.Database {
			get { return connection.Database; }
		}
		string IOLAPConnection.ServerVersion { 
			get { return this.ServerVersion; } 
		}
		IOLAPCommand IOLAPConnection.CreateCommand(string mdx) {
			return new OleCommand(this, mdx);
		}
		void IOLAPConnection.Open() {
			connection.Open();
		}
		void IOLAPConnection.Close(bool endSession) {
			connection.Close();
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			connection.Dispose();
		}
		#endregion
	}
	class OleCommand : IOLAPCommand {
		OleDbCommand command;
		OleConnection connection;
		internal OleCommand(OleConnection oleConnection, string mdx) {
			this.command = new OleDbCommand(mdx, oleConnection.Connection);
			this.connection = oleConnection;
		}
		internal OleDbDataReader ExecuteReader() {
			return command.ExecuteReader();
		}
		#region IOLAPCommand Members
		string IOLAPCommand.CommandText {
			get { return command.CommandText; }
			set { command.CommandText = value; }
		}
		int IOLAPCommand.CommandTimeout {
			get { return command.CommandTimeout; }
			set { command.CommandTimeout = value; }
		}
		IOLAPConnection IOLAPCommand.Connection {
			get { return this.connection; }
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			command.Dispose();
		}
		#endregion
	}
}
