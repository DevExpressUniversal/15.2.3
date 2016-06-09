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
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.Reflection;
namespace DevExpress.XtraReports.Native {
	public enum ConnectionType { Sql, OleDB, ODBC, Oracle };
	public static class ConnectionStringHelper {
		public static DbConnection CreateDBConnection(string connectionString) {
			DbConnectionStringBuilder builder = new DbConnectionStringBuilder(false) { ConnectionString = connectionString };
			ConnectionType connectionType = GetConnectionType(builder);
			if(connectionType == ConnectionType.ODBC) {
				object dataSourceName;
				if(builder.TryGetValue("Data Source", out dataSourceName) || builder.TryGetValue("DSN", out dataSourceName))
					return new System.Data.Odbc.OdbcConnection("Dsn=" + dataSourceName);
				return new System.Data.Odbc.OdbcConnection(builder.ConnectionString);
			}
			if(connectionType == ConnectionType.Sql) {
				builder.Remove("provider");
				return new SqlConnection(builder.ConnectionString);
			}
			if(connectionType == ConnectionType.Oracle) {
				builder.Remove("provider");
#pragma warning disable 0618
				return new OracleConnection(builder.ConnectionString);
#pragma warning restore 0618
			}
			return new OleDbConnection(connectionString);
		}
		public static ConnectionType GetConnectionType(string connectionString) {
			return GetConnectionType(new DbConnectionStringBuilder(false) { ConnectionString = connectionString });
		}
		static ConnectionType GetConnectionType(DbConnectionStringBuilder builder) {
			object value;
			if(builder.TryGetValue("provider", out value)) {
				string provider = value as string;
				if(provider != null && provider.IndexOf("SQLOLEDB") >= 0)
					return ConnectionType.Sql;
				if(provider != null && provider.IndexOf("MSDASQL") >= 0)
					return ConnectionType.ODBC;
				if(provider != null && provider.IndexOf("MSDAORA") >= 0)
					return ConnectionType.Oracle;
			}
			return ConnectionType.OleDB;
		}
		public static string GetConnectionString() {
			try {
				Guid dataLinksGuid = new Guid("2206cdb2-19c1-11d1-89e0-00c04fd7a829");
				Type dataLinksType = Type.GetTypeFromCLSID(dataLinksGuid, true);
				object dataLinks = Activator.CreateInstance(dataLinksType);
				object connection = dataLinksType.InvokeMember("PromptNew", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, dataLinks, new object[] { }, null);
				if(connection == null)
					return string.Empty;
				Guid connetionGuid = new Guid("00000550-0000-0010-8000-00AA006D2EA4");
				Type connectionType = Type.GetTypeFromCLSID(connetionGuid, true);
				return connectionType.InvokeMember("ConnectionString", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, connection, new object[] { }, null) as string;
			} catch {
				return string.Empty;
			}
		}
		public static string GetConnectionName(string connectionString) {
			using(System.Data.Common.DbConnection connection = CreateDBConnection(connectionString)) {
				return String.IsNullOrEmpty(connection.Database) && String.IsNullOrEmpty(connection.DataSource) ? connectionString : String.IsNullOrEmpty(connection.Database) ? connection.DataSource : string.Format("{0}.{1}", connection.DataSource, connection.Database);
			}
		}
		public static string RemoveProviderFromConnectionString(string connectionString) {
			DbConnectionStringBuilder builder = new DbConnectionStringBuilder(false) { ConnectionString = connectionString };
			builder.Remove("provider");
			return builder.ConnectionString;
		}
	}
}
