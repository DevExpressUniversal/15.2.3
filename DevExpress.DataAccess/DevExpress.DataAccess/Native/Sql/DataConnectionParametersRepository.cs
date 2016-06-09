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

using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql {
	public class DataConnectionParametersRepository : ItemsRepository<DataConnectionParametersBase> {
		public const string OlapProviderKey = "Microsoft SQL Server Analysis Services";
		public const string XmlFileProviderKey = "InMemorySetFull";
		public const string CustomConStrTag = "<Custom connection string>";
		static DataConnectionParametersRepository parametersRepository;
		static readonly object repositoryLock = new object();
		public static DataConnectionParametersRepository Instance
		{
			get
			{
				lock(repositoryLock) {
					if(parametersRepository == null)
						CreateParametersRepository();
					return parametersRepository;
				}
			}
		}
		static void CreateParametersRepository() {
			parametersRepository = new DataConnectionParametersRepository();
			parametersRepository.RegisterItemType(XmlFileProviderKey, typeof(XmlFileConnectionParameters));
#if !DXPORTABLE
			parametersRepository.RegisterItemType("Access97", typeof(Access97ConnectionParameters));
			parametersRepository.RegisterItemType("Access2007", typeof(Access2007ConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessOracleConnectionProvider.XpoProviderTypeString, typeof(OracleConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessAdvantageConnectionProvider.XpoProviderTypeString, typeof(AdvantageConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessAseConnectionProvider.XpoProviderTypeString, typeof(AseConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessAsaConnectionProvider.XpoProviderTypeString, typeof(AsaConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessAmazonRedshiftConnectionProvider.XpoProviderTypeString, typeof(AmazonRedshiftConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessDB2ConnectionProvider.XpoProviderTypeString, typeof(DB2ConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessFirebirdConnectionProvider.XpoProviderTypeString, typeof(FireBirdConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessBigQueryConnectionProvider.XpoProviderTypeString, typeof(BigQueryConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessMySqlConnectionProvider.XpoProviderTypeString, typeof(MySqlConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessPervasiveSqlConnectionProvider.XpoProviderTypeString, typeof(PervasiveSqlConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessPostgreSqlConnectionProvider.XpoProviderTypeString, typeof(PostgreSqlConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessVistaDBConnectionProvider.XpoProviderTypeString, typeof(VistaDBConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessVistaDB5ConnectionProvider.XpoProviderTypeString, typeof(VistaDB5ConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessMSSqlCEConnectionProvider.XpoProviderTypeString, typeof(MsSqlCEConnectionParameters));
#endif
			parametersRepository.RegisterItemType(DataAccessMSSqlConnectionProvider.XpoProviderTypeString, typeof(MsSqlConnectionParameters));
			parametersRepository.RegisterItemType(DataAccessSQLiteConnectionProvider.XpoProviderTypeString, typeof(SQLiteConnectionParameters));
#if !DXPORTABLE
			parametersRepository.RegisterItemType(DataAccessTeradataConnectionProvider.XpoProviderTypeString, typeof(TeradataConnectionParameters));
			parametersRepository.RegisterItemType(OlapProviderKey, typeof(OlapConnectionParameters));
#endif
#if DEBUGTEST
			parametersRepository.RegisterItemType("TestFactory", typeof(DevExpress.DataAccess.Tests.TestConnectionParameters));
#endif
			parametersRepository.RegisterItemType(CustomConStrTag, typeof(CustomStringConnectionParameters));
		}
		DataConnectionParametersRepository() {
		}
		public DataConnectionParametersBase CreateParametersByFactory(ProviderFactory factory) {
			DataConnectionParametersBase parameters = CreateItemByKey(factory.ProviderKey);
			parameters.Factory = factory;
			return parameters;
		}
	}
}
