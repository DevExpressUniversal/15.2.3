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
using System.Configuration;
using System.Data.Common;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
using DataStoreBase = DevExpress.Xpo.DB.DataStoreBase;
namespace DevExpress.DataAccess.Native {
	public class AppConfigHelper {
		public static SqlDataConnection CreateSqlConnectionFromConnectionString(IConnectionStringsProvider connectionStringsProvider, string name) {
			IConnectionStringInfo connectionStringInfo = connectionStringsProvider.GetConnectionStringInfo(name);
			if(connectionStringInfo != null)
				return CreateSqlConnection(connectionStringInfo, true);
			return null;
		}
		public static Dictionary<string, SqlDataConnection> GetConnections() {
			return GetConnections(ConnectionStrings());
		}
		protected static Dictionary<string, SqlDataConnection> GetConnections(IConnectionStringInfo[] collection) {
			Dictionary<string, SqlDataConnection> result = new Dictionary<string, SqlDataConnection>(collection.Length);
			foreach(IConnectionStringInfo settings in collection) {
				SqlDataConnection sqlConnection = CreateSqlConnection(settings, true);
				if(sqlConnection != null)
					result.Add(settings.Name, sqlConnection);
			}
			return result;
		}
		static IConnectionStringInfo[] ConnectionStrings() {
			return RuntimeConnectionStringsProvider.GetConnectionStringInfos();
		}
		public static DataConnectionParametersBase LoadConnectionParameters(string name) {
			ConnectionStringSettings connStr = ConfigurationManager.ConnectionStrings[name];
			if(connStr == null)
				return null;
			return ConnectionStringParser.Parse(connStr.ConnectionString);
		}
		public static SqlDataConnection CreateSqlConnection(IConnectionStringInfo connectionStringInfo, bool useAppConfig) {
			string connectionString = connectionStringInfo.RunTimeConnectionString;
			DbConnectionStringBuilder builder = null;
			try {
				builder = new DbConnectionStringBuilder() { ConnectionString = connectionString };
			}
			catch(ArgumentException) {
				System.Diagnostics.Debug.WriteLine("Incorrect connectionString, Name {0}, Content '{1}'", connectionStringInfo.Name, connectionString);
				return null;
			}
			string provider = connectionStringInfo.ProviderName;
			if(!builder.ContainsKey(DataStoreBase.XpoProviderTypeParameterName.ToLowerInvariant())) {
				if(string.IsNullOrEmpty(provider) || provider.Equals("System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase))
					connectionString = AddXpoProviderToConnectionString(connectionString, MSSqlConnectionProvider.XpoProviderTypeString);
				else if(provider.StartsWith("Microsoft.SqlServerCe.Client.", StringComparison.InvariantCultureIgnoreCase))
					connectionString = AddXpoProviderToConnectionString(connectionString, MSSqlCEConnectionProvider.XpoProviderTypeString);
				else if(provider.Equals("Oracle.DataAccess.Client", StringComparison.InvariantCultureIgnoreCase))
					connectionString = AddXpoProviderToConnectionString(connectionString, OracleConnectionProvider.XpoProviderTypeString);
				else if(provider.Equals("System.Data.OleDb", StringComparison.InvariantCultureIgnoreCase)) {
					string oledbProvider = (string)builder[ConnectionStringParser.StrOledbProvider];
					if(oledbProvider.Equals(ConnectionStringParser.StrJetOledb4, StringComparison.InvariantCultureIgnoreCase) || oledbProvider.Equals(ConnectionStringParser.StrAceOledb12, StringComparison.InvariantCultureIgnoreCase))
						connectionString = AddXpoProviderToConnectionString(connectionString, AccessConnectionProvider.XpoProviderTypeString);
					else {
						System.Diagnostics.Debug.WriteLine("{0}.CreateSqlConnection: Unknown OLEDB provider: {1}", typeof(AppConfigHelper).Name, oledbProvider);
						return null;
					}
				}
				else {
					System.Diagnostics.Debug.WriteLine("{0}.CreateSqlConnection: Unknown provider: {1}", typeof(AppConfigHelper).Name, provider);
					return null;
				}
			}
			return new SqlDataConnection(connectionStringInfo.Name, ConnectionStringParser.Parse(connectionString)) { StoreConnectionNameOnly = useAppConfig };
		}
		static string AddXpoProviderToConnectionString(string connectionString, string providerName) {
			return string.Format("{0}={1};{2}", DataStoreBase.XpoProviderTypeParameterName, providerName, connectionString);
		}
	}
}
