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
using System.Linq;
using System.Threading;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.Service.Native.DAL;
using DevExpress.XtraReports.Service.Native.Services.BinaryStore;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class CleanService : ICleanService, IDisposable {
		static readonly TimeSpan FirstTimeDelay = TimeSpan.FromSeconds(30);
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		readonly IDALService dalService;
		readonly IBinaryDataStorageServiceProvider binaryDataStorageServiceProvider;
		readonly object syncRoot = new object();
		readonly TimeSpan keepInterval;
		Timer timer;
		bool disposed;
		protected Timer Timer {
			get { return timer; }
			set { timer = value; }
		}
		public CleanService(
			IDALService dalService,
			IConfigurationService configurationService,
			IBinaryDataStorageServiceProvider binaryDataStorageServiceProvider) {
			Guard.ArgumentNotNull(dalService, "dalService");
			Guard.ArgumentNotNull(binaryDataStorageServiceProvider, "binaryDataStorageServiceProvider");
			this.dalService = dalService;
			this.binaryDataStorageServiceProvider = binaryDataStorageServiceProvider;
			keepInterval = CalculateKeepInterval(configurationService);
		}
		#region ICleanService Members
		public void SafeStart() {
			if(keepInterval.TotalMilliseconds == Timeout.Infinite) {
				return;
			}
			Helper.DoubleCheckInitialize(ref timer, syncRoot, InitializeTimer);
		}
		#endregion
		protected virtual void CleanCore(DateTime criteria, UnitOfWork session) {
			var listExternalKeys = new List<string>();
			var documents = StoredDocument.FindOutdated(criteria, session);
			CollectExternalKeys(listExternalKeys, documents, x => With(x.RelatedData, y => y.ExportOptionsExternalKey));
			CollectExternalKeys(listExternalKeys, documents, x => With(x.RelatedData, y => y.PrnxExternalKey));
			CollectExternalKeys(listExternalKeys, documents, x => With(x.RelatedData, y => y.WatermarkExternalKey));
			CollectExternalKeys(listExternalKeys, documents, x => With(x.RelatedData, y => y.DrillDownKeysExternalKey));
			session.Delete(documents);
			var requests = PagesRequestInformation.FindOutdated(criteria, session);
			CollectExternalKeys(listExternalKeys, requests, x => With(x.ResponseInformation, y => y.ExternalKey));
			session.Delete(requests);
			var exportedDocuments = ExportedDocument.FindOutdated(criteria, session);
			CollectExternalKeys(listExternalKeys, exportedDocuments, x => x.ExternalKey);
			session.Delete(exportedDocuments);
			IBinaryDataStorageService binaryDataStorageService = binaryDataStorageServiceProvider.GetService();
			const int maxSQLServerSupportParametersCountInQuery = 2000;
			while(listExternalKeys.Count > 0) {
				binaryDataStorageService.Clean(listExternalKeys.Take(maxSQLServerSupportParametersCountInQuery), session);
				listExternalKeys.RemoveRange(0, Math.Min(listExternalKeys.Count, maxSQLServerSupportParametersCountInQuery));
			}
		}
		void Clean() {
			var criteria = DateTime.UtcNow - keepInterval;
			try {
				using(var session = dalService.CreateUnitOfWork()) {
					CleanCore(criteria, session);
					session.CommitChanges();
				}
			} catch(Exception e) {
				Logger.Error("CleanService: Exception on Clean - " + e);
			}
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~CleanService() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposed) {
				return;
			}
			if(disposing) {
				using(timer) {
				}
			}
			disposed = true;
		}
		#endregion
		Timer InitializeTimer() {
			return new Timer(_ => Clean(), null, FirstTimeDelay, keepInterval);
		}
		protected static void CollectExternalKeys<T>(ICollection<string> collection, IEnumerable<T> items, Func<T, string> getKey) {
			foreach(T item in items) {
				string key = getKey(item);
				if(!string.IsNullOrEmpty(key)) {
					collection.Add(key);
				}
			}
		}
		static TimeSpan CalculateKeepInterval(IConfigurationService configurationService) {
			Guard.ArgumentNotNull(configurationService, "configurationService");
			IDocumentDataStorageProvider documentStore = configurationService.DocumentStoreConfiguration;
			return documentStore != null
				? documentStore.KeepInterval
				: DocumentDataStorageProviderExtensions.GetDefaultKeepInterval(null);
		}
		protected static V With<T, V>(T item, Func<T, V> get)
			where T : class {
			return item != null
				? get(item)
				: default(V);
		}
	}
}
