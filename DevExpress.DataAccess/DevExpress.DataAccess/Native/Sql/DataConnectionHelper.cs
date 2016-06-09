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
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.Native.Sql {
	public static class DataConnectionHelper {
		public const string ServerParam = "server";
		public const string DatabaseParam = "database";
		public const string PasswordParam = "password";
		public const string UserIdParam = "userid";
		public const string XmlDataConnection = "DataConnection";
		public const string XmlProviderKey = "ProviderKey";
		public const string XmlParameters = "Parameters";
		public const string XmlParameter = "Parameter";
		public const string XmlValue = "Value";
		public const string XmlRequirePassword = "RequirePassword";
		public const string XmlName = "Name";
		public const string XmlConnectionString = "ConnectionString";
		public const string XmlDataConnectionBase = "DataConnectionBase";
		public const string XmlUseAppConfig = "FromAppConfig";
		static DataConnectionHelper() {
			InMemoryDataStore.Register();
			DataAccessMSSqlConnectionProvider.ProviderRegister();
			DataAccessSQLiteConnectionProvider.ProviderRegister();
#if !DXPORTABLE
			DataAccessAdvantageConnectionProvider.ProviderRegister();
			DataAccessAsaConnectionProvider.ProviderRegister();
			DataAccessAseConnectionProvider.ProviderRegister();
			DataAccessAmazonRedshiftConnectionProvider.ProviderRegister();
			DataAccessDB2ConnectionProvider.ProviderRegister();
			DataAccessFirebirdConnectionProvider.ProviderRegister();
			DataAccessBigQueryConnectionProvider.ProviderRegister();
			DataAccessMSAccessConnectionProvider.ProviderRegister();
			DataAccessMSSqlCEConnectionProvider.ProviderRegister();
			DataAccessMySqlConnectionProvider.ProviderRegister();
			DataAccessOracleConnectionProvider.ProviderRegister();
			DataAccessODPConnectionProvider.ProviderRegister();
			DataAccessODPManagedConnectionProvider.ProviderRegister();
			DataAccessPervasiveSqlConnectionProvider.ProviderRegister();
			DataAccessPostgreSqlConnectionProvider.ProviderRegister();
			DataAccessTeradataConnectionProvider.ProviderRegister();
			DataAccessVistaDBConnectionProvider.ProviderRegister();
			DataAccessVistaDB5ConnectionProvider.ProviderRegister();
# endif
		}
		public static void RegisterProviders() {
		}
		public static ProviderFactory[] GetProviderFactories() {
			return DataStoreBase.Factories;
		}
		public static ProviderFactory GetProviderFactory(string providerKey) {
			Guard.ArgumentIsNotNullOrEmpty(providerKey, "providerKey");
			ProviderFactory[] factories = GetProviderFactories();
			foreach(ProviderFactory factory in factories)
				if(string.CompareOrdinal(factory.ProviderKey, providerKey) == 0)
					return factory;
			return null;
		}
		public static DataConnectionParametersBase CreateDataConnectionParameters(ProviderFactory factory, Dictionary<string, string> parameters) {
			DataConnectionParametersBase dataConnectionParameters = DataConnectionParametersRepository.Instance.CreateItemByKey(factory.ProviderKey);
			dataConnectionParameters.Factory = factory;
#if !DXPORTABLE
			if(factory.ProviderKey == "Firebird") {
				if(!parameters.ContainsKey(ProviderFactory.ServerParamID) || string.IsNullOrEmpty(parameters[ProviderFactory.ServerParamID]))
					((FireBirdConnectionParameters)dataConnectionParameters).ConnectionType = FireBirdConnectionType.Embedded;
			}
			if(factory.ProviderKey == AsaConnectionProvider.XpoProviderTypeString) {
				if(!parameters.ContainsKey(ProviderFactory.ServerParamID) || string.IsNullOrEmpty(parameters[ProviderFactory.ServerParamID]))
					((AsaConnectionParameters)dataConnectionParameters).ConnectionType = AsaConnectionType.File;
			}
#endif
			DataAccessConnectionParameter.SetParamsDict(factory.ProviderKey, parameters, dataConnectionParameters);
			return dataConnectionParameters;
		}
		public static void BlackoutCredentials(DataConnectionParametersBase parameters) {
			IConnectionPage connPage = parameters;
			connPage.UserName = null;
			connPage.Password = null;
#if !DXPORTABLE
			IConnectionPageBigQuery pageBigQuery = parameters as IConnectionPageBigQuery;
			if(pageBigQuery != null) {
				pageBigQuery.PrivateKeyFileName = null;
				pageBigQuery.ServiceAccountEmail = null;
				pageBigQuery.OAuthClientID = null;
				pageBigQuery.OAuthClientSecret = null;
				pageBigQuery.OAuthRefreshToken = null;
			}
#endif
			if(parameters is CustomStringConnectionParameters)
				throw new NotSupportedException();
		}
	}
}
