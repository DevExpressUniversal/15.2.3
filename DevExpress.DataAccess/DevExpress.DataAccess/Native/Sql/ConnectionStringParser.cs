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

using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql {
	static class ConnectionStringParser {
		public const string StrOledbProvider = "Provider";
		public const string StrJetOledb4 = "Microsoft.Jet.OLEDB.4.0";
		public const string StrAceOledb12 = "Microsoft.ACE.OLEDB.12.0";
		class ConnectionStringModel {
			readonly DbConnectionStringBuilder builder;
			readonly HashSet<string> unparsedKeys;
			public ConnectionStringModel(string connectionString) {
				this.builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
				this.unparsedKeys = new HashSet<string>();
				foreach(object key in this.builder.Keys)
					this.unparsedKeys.Add((string)key);
			}
			bool HasUnparsedKeys { get { return this.unparsedKeys.Count != 0; } }
#if !DXPORTABLE
			public Access97ConnectionParameters ParseAccess97ConnectionString() {
				if(!CheckFixedParameter("Mode", "Share Deny None"))
					return null;
				string filename = GetParameter("data source");
				string username = GetParameter("user id");
				string password = GetParameter("Jet OLEDB:Database password");
				if(HasUnparsedKeys)
					return null;
				return new Access97ConnectionParameters(filename, username, password);
			}
			public Access2007ConnectionParameters ParseAccess2007ConnectionString() {
				if(!CheckFixedParameter("Mode", "Share Deny None"))
					return null;
				string filename = GetParameter("data source");
				string password = GetParameter("Jet OLEDB:Database password");
				if(HasUnparsedKeys)
					return null;
				return new Access2007ConnectionParameters(filename, password);
			}
			public AdvantageConnectionParameters ParseAdvantageConnectionString() {
				if(!CheckFixedParameter("TrimTrailingSpaces", "true"))
					return null;
				string filename = GetParameter("Data source");
				string username = GetParameter("user id");
				string password = GetParameter("Password");
				string servertype = GetParameter("servertype");
				if(HasUnparsedKeys)
					return null;
				if(string.IsNullOrWhiteSpace(servertype))
					return new AdvantageConnectionParameters(filename, username, password);
				switch(servertype.ToLowerInvariant()) {
					case "local":
						return new AdvantageConnectionParameters(filename, username, password, AdvantageServerType.Local);
					case "remote":
						return new AdvantageConnectionParameters(filename, username, password, AdvantageServerType.Remote);
					case "internet":
						return new AdvantageConnectionParameters(filename, username, password, AdvantageServerType.Internet);
					default: return null;
				}
			}
			public BigQueryConnectionParameters ParseBigQueryTypeString() {
				string project = GetParameter("ProjectID");
				string dataSet = GetParameter("DataSetID");
				string keyFileName = GetParameter("PrivateKeyFileName");
				if(!string.IsNullOrEmpty(keyFileName)) {
					string email = GetParameter("ServiceAccountEmail");
					return new BigQueryConnectionParameters(project, dataSet, email, keyFileName);
				}
				string cilentID = GetParameter("OAuthClientID");
				string clientSecret = GetParameter("OAuthClientSecret");
				string refreshToken = GetParameter("OAuthRefreshToken");
				return new BigQueryConnectionParameters(project, dataSet, cilentID, clientSecret, refreshToken);
			}
			public OracleConnectionParameters ParseOracleConnectionString() {
				string server = GetParameter("Data Source");
				string username = GetParameter("user id");
				string password = GetParameter("password");
				if(HasUnparsedKeys)
					return null;
				return new OracleConnectionParameters(server, username, password);
			}
			public AseConnectionParameters ParseAseConnectionString() {
				if(!(CheckFixedParameter("Port", "5000") && CheckFixedParameter("persist security info", "true")))
					return null;
				string server = GetParameter("Data Source");
				string database = GetParameter("Initial Catalog");
				string username = GetParameter("User ID");
				string password = GetParameter("Password");
				if(HasUnparsedKeys)
					return null;
				return new AseConnectionParameters(server, database, username, password);
			}
			public AsaConnectionParameters ParseAsaConnectionString() {
				if(!CheckFixedParameter("Persist Security Info", "true"))
					return null;
				string server = GetParameter("eng");
				string username = GetParameter("uid");
				string password = GetParameter("pwd");
				string database = GetParameter("dbn");
				string file = GetParameter("dbf");
				string links = GetParameter("links");
				string hostname = null;
				if(!string.IsNullOrWhiteSpace(links)) {
					Regex tcpip = new Regex(@"\Atcpip\{host=(?<name>.+)\}\z", RegexOptions.IgnoreCase);
					Match match = tcpip.Match(links);
					if(!match.Success)
						return null;
					hostname = match.Groups["name"].Value;
				}
				if(HasUnparsedKeys)
					return null;
				return server != null
					? hostname != null ? new AsaConnectionParameters(server, hostname, database, username, password) : new AsaConnectionParameters(server, database, username, password) 
					: new AsaConnectionParameters(file, username, password);
			}
			public DB2ConnectionParameters ParseDB2ConnectionString() {
				if(!CheckFixedParameter("persist security info", "true"))
					return null;
				string server = GetParameter("server");
				string database = GetParameter("database");
				string username = GetParameter("user id");
				string password = GetParameter("password");
				if(HasUnparsedKeys)
					return null;
				return new DB2ConnectionParameters(server, database, username, password);
			}
			public FireBirdConnectionParameters ParseFirebirdConnectionString() {
				if(!CheckFixedParameter("Charset", "NONE"))
					return null;
				string server = GetParameter("DataSource");
				string database = GetParameter("Database");
				string username = GetParameter("User");
				string password = GetParameter("Password");
				string serverType = GetParameter("ServerType");
				if(HasUnparsedKeys || serverType == null)
					return null;
				if(serverType == ((int)FirebirdServerType.Server).ToString()) {
					return new FireBirdConnectionParameters(server, database, username, password);
				}
				if(serverType == ((int)FirebirdServerType.Embedded).ToString() && server == "localhost")
					return new FireBirdConnectionParameters(database, username, password);
				return null;
			}
			public MySqlConnectionParameters ParseMySqlConnectionString() {
				if(!(CheckFixedParameter("persist security info", "true") && CheckFixedParameter("CharSet", "utf8")))
					return null;
				string server = GetParameter("server");
				string port = GetParameter("port");
				string database = GetParameter("database");
				string username = GetParameter("user id");
				string password = GetParameter("password");
				if(HasUnparsedKeys)
					return null;
				return new MySqlConnectionParameters(server, database, username, password, port);
			}
			public PervasiveSqlConnectionParameters ParsePervasiveConnectionString() {
				string server = GetParameter("Server");
				string database = GetParameter("ServerDSN");
				string username = GetParameter("UID");
				string password = GetParameter("PWD");
				if(HasUnparsedKeys)
					return null;
				return new PervasiveSqlConnectionParameters(server, database, username, password);
			}
			public PostgreSqlConnectionParameters ParsePostgresConnectionString() {
				if(!CheckFixedParameter("Encoding", "UNICODE"))
					return null;
				string server = GetParameter("Server");
				string port = GetParameter("Port");
				string database = GetParameter("Database");
				string username = GetParameter("User Id");
				string password = GetParameter("Password");
				if(HasUnparsedKeys)
					return null;
				return port != null 
					? new PostgreSqlConnectionParameters(server, port, database, username, password) 
					: new PostgreSqlConnectionParameters(server, database, username, password);
			}
			public VistaDBConnectionParameters ParseVistaDBConnectionString() {
				string filename = GetParameter("Data Source");
				string password = GetParameter("Password");
				if(HasUnparsedKeys)
					return null;
				return new VistaDBConnectionParameters(filename, password);
			}
			public VistaDBConnectionParameters ParseVistaDB5ConnectionString() {
				string filename = GetParameter("Data Source");
				string password = GetParameter("Password");
				if(HasUnparsedKeys)
					return null;
				return new VistaDB5ConnectionParameters(filename, password);
			}
			public MsSqlCEConnectionParameters ParseMSSqlServerCEConnectionString() {
				string filename = GetParameter("data source");
				string password = GetParameter("password");
				if(HasUnparsedKeys)
					return null;
				return new MsSqlCEConnectionParameters(filename, password);
			}
#endif
			public MsSqlConnectionParameters ParseMSSqlServerConnectionString() {
				string server = GetParameter("data source");
				string database = GetParameter("initial catalog");
				if(server == null || database == null)
					return null;
				string security = GetParameter("integrated security");
				switch(security) {
					case "SSPI":
					case "True":
						if(HasUnparsedKeys)
							return null;
						return new MsSqlConnectionParameters(server, database, null, null, MsSqlAuthorizationType.Windows);
					case null:
						if(!CheckFixedParameter("Persist Security Info", "true"))
							return null;
						string username = GetParameter("user id");
						string password = GetParameter("password");
						if(HasUnparsedKeys)
							return null;
						return new MsSqlConnectionParameters(server, database, username, password, MsSqlAuthorizationType.SqlServer);
					default:
						return null;
				}
			}
			public SQLiteConnectionParameters ParseSQLiteConnectionString() {
				string fileName = GetParameter("Data Source");
				string password = GetParameter("Password");
				if(HasUnparsedKeys)
					return null;
				return new SQLiteConnectionParameters(fileName, password);
			}
#if !DXPORTABLE
			public TeradataConnectionParameters ParseTeradataConnectionString() {
				if(!CheckFixedParameter("Session Mode", "ANSI"))
					return null;
				string server = GetParameter("Data Source");
				string database = GetParameter("Database");
				string username = GetParameter("User Id");
				string password = GetParameter("Password");
				string port = GetParameter("Port Number");
				if(HasUnparsedKeys)
					return null;
				return new TeradataConnectionParameters(server, port, database, username, password);
			}
#endif
			public bool CheckFixedParameter(string key, string value) {
				key = key.ToLowerInvariant();
				if(this.unparsedKeys.Contains(key))
					this.unparsedKeys.Remove(key);
				else
					return false;
				return ((string)this.builder[key] == value);
			}
			public string GetParameter(string key) {
				key = key.ToLowerInvariant();
				if(this.unparsedKeys.Contains(key))
					this.unparsedKeys.Remove(key);
				else
					return null;
				return (string)this.builder[key];
			}
		}
		public static DataConnectionParametersBase Parse(string connectionString) {
			ConnectionStringModel model = new ConnectionStringModel(connectionString);
			DataConnectionParametersBase result;
			switch(model.GetParameter(DataStoreBase.XpoProviderTypeParameterName)) {
#if !DXPORTABLE
				case AccessConnectionProvider.XpoProviderTypeString: 
					string provider = model.GetParameter(StrOledbProvider);
					switch(provider) {
						case StrJetOledb4:
							result = model.ParseAccess97ConnectionString();
							break;
						case StrAceOledb12:
							result = model.ParseAccess2007ConnectionString();
							break;
						default:
							result = null;
							break;
					}
					break;
				case OracleConnectionProvider.XpoProviderTypeString: 
					result = model.ParseOracleConnectionString();
					break;
				case AdvantageConnectionProvider.XpoProviderTypeString: 
					result = model.ParseAdvantageConnectionString();
					break;
				case AseConnectionProvider.XpoProviderTypeString: 
					result = model.ParseAseConnectionString();
					break;
				case AsaConnectionProvider.XpoProviderTypeString: 
					result = model.ParseAsaConnectionString();
					break;
				case DataAccessBigQueryConnectionProvider.XpoProviderTypeString: 
					result = model.ParseBigQueryTypeString();
					break;
				case DB2ConnectionProvider.XpoProviderTypeString: 
					result = model.ParseDB2ConnectionString();
					break;
				case FirebirdConnectionProvider.XpoProviderTypeString: 
					result = model.ParseFirebirdConnectionString();
					break;
				case MySqlConnectionProvider.XpoProviderTypeString: 
					result = model.ParseMySqlConnectionString();
					break;
				case PervasiveSqlConnectionProvider.XpoProviderTypeString: 
					result = model.ParsePervasiveConnectionString();
					break;
				case PostgreSqlConnectionProvider.XpoProviderTypeString: 
					result = model.ParsePostgresConnectionString();
					break;
				case VistaDBConnectionProvider.XpoProviderTypeString: 
					result = model.ParseVistaDBConnectionString();
					break;
				case VistaDB5ConnectionProvider.XpoProviderTypeString: 
					result = model.ParseVistaDB5ConnectionString();
					break;
				case MSSqlCEConnectionProvider.XpoProviderTypeString: 
					result = model.ParseMSSqlServerCEConnectionString();
					break;
#endif
				case MSSqlConnectionProvider.XpoProviderTypeString: 
					result = model.ParseMSSqlServerConnectionString();
					break;
				case SQLiteConnectionProvider.XpoProviderTypeString: 
					result = model.ParseSQLiteConnectionString();
					break;
#if !DXPORTABLE
				case DataAccessTeradataConnectionProvider.XpoProviderTypeString: 
					result = model.ParseTeradataConnectionString();
					break;
#endif
				default: 
					result = null;
					break;
			}
			return result ?? new CustomStringConnectionParameters(connectionString);
		}
	}
}
