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
using System.Data.Common;
using System.Linq;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.Service.Native.DAL;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class DALService : IDALService {
		static readonly string[] SimpleXpoProviders = {
			AccessConnectionProvider.XpoProviderTypeString,
			AccessConnectionProviderMultiUserThreadSafe.XpoProviderTypeString,
			InMemoryDataStore.XpoProviderTypeString,
			DataSetDataStore.XpoProviderTypeString
		};
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		static string InMemoryDataStoreConnectionString {
			get { return InMemoryDataStore.GetConnectionStringInMemory(true); }
		}
		readonly object initializingSyncRoot = new object();
		readonly object dataLayerSyncRoot = new object();
		readonly IConfigurationService configurationService;
		IDataLayer dataLayer;
		volatile bool isDbSchemaUpdated;
		Assembly[] dataStoreSchemaAssemblies;
		IDataLayer DataLayer {
			get {
				Helper.DoubleCheckInitialize(ref dataLayer, dataLayerSyncRoot, CreateDataLayer);
				return dataLayer;
			}
		}
		Assembly[] DataStoreSchemaAssemblies {
			get {
				if(dataStoreSchemaAssemblies == null) {
					dataStoreSchemaAssemblies = GetDataStoreSchemaAssemblies().ToArray();
				}
				return dataStoreSchemaAssemblies;
			}
		}
#if DEBUGTEST
		public bool IsDbSchemaUpdated_TEST {
			get { return isDbSchemaUpdated; }
			set { isDbSchemaUpdated = value; }
		}
#endif
		string connectionString;
		string ConnectionString {
			get {
				if(connectionString == null) {
					IDocumentDataStorageProvider documentStoreConfiguration = configurationService.DocumentStoreConfiguration;
					string currentConnectionString = documentStoreConfiguration != null
						? documentStoreConfiguration.ConnectionString
						: null;
					if(string.IsNullOrEmpty(currentConnectionString)) {
						currentConnectionString = InMemoryDataStoreConnectionString;
					}
					var builder = new DbConnectionStringBuilder { ConnectionString = currentConnectionString };
					string provider;
					connectionString = builder.TryGetValue(DataStoreBase.XpoProviderTypeParameterName, out provider) && SimpleXpoProviders.Contains(provider)
						? currentConnectionString
						: XpoDefault.GetConnectionPoolString(currentConnectionString);
				}
				return connectionString;
			}
		}
		protected virtual ICollection<Assembly> GetDataStoreSchemaAssemblies() {
			return new List<Assembly>(1) { typeof(StoredDocument).Assembly };
		}
		public DALService(IConfigurationService configurationService) {
			Guard.ArgumentNotNull(configurationService, "configurationService");
			this.configurationService = configurationService;
		}
		#region IDALService Members
		public void SafeInitialize() {
			if(isDbSchemaUpdated) {
				return;
			}
			lock (initializingSyncRoot) {
				if(isDbSchemaUpdated) {
					return;
				}
				try {
					using(var session = CreateUnitOfWork()) {
						session.UpdateSchema(DataStoreSchemaAssemblies);
						session.CreateObjectTypeRecords(DataStoreSchemaAssemblies);
						isDbSchemaUpdated = true;
					}
				} catch(Exception e) {
					Logger.Error("Unable to update schema: " + e);
					throw;
				}
			}
		}
		public UnitOfWork CreateUnitOfWork() {
			return new SmartUnitOfWork(DataLayer);
		}
		#endregion
#if DEBUGTEST
		internal string PrepareConnectionStringTEST() {
			return ConnectionString;
		}
#endif
		IDataLayer CreateDataLayer() {
			var xpDictionary = new ReflectionDictionary();
			xpDictionary.GetDataStoreSchema(DataStoreSchemaAssemblies);
			var dataStore = XpoDefault.GetConnectionProvider(ConnectionString, AutoCreateOption.DatabaseAndSchema);
			return new ThreadSafeDataLayer(xpDictionary, dataStore);
		}
		#region IDisposable
		bool disposed;
		public void Dispose() {
			Dispose(true);
		}
		~DALService() {
			Dispose(false);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(!disposing || disposed) {
				return;
			}
			disposed = true;
			using(dataLayer) {
			}
		}
		#endregion
	}
}
