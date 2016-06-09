#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.IO;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Azure.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using Microsoft.WindowsAzure.Storage.Table;
namespace DevExpress.XtraReports.Web.Azure.WebDocumentViewer {
	public class AzureEntityStorageManager : IAzureEntityStorageManager {
		static ILoggingService Logger {
			get { return DefaultLoggingService.Instance; }
		}
		CloudTable reportTable;
		CloudTable documentTable;
		#region Properties
		internal CloudTable ReportTable {
			get {
				if(reportTable == null) {
					reportTable = serviceTableProvider.GetReportTable();
				}
				return reportTable;
			}
		}
		internal CloudTable DocumentTable {
			get {
				if(documentTable == null) {
					documentTable = serviceTableProvider.GetDocumentTable();
				}
				return documentTable;
			}
		}
		#endregion
		readonly IAzureServiceTableProvider serviceTableProvider;
		readonly IAzureFileStorageManager fileStorage;
		public AzureEntityStorageManager(IAzureServiceTableProvider serviceTableProvider, IAzureFileStorageManager fileStorage) {
			this.serviceTableProvider = serviceTableProvider;
			this.fileStorage = fileStorage;
		}
		#region IAzureStorageManager
		public bool IsBlankDocumentExists(string id) {
			var retrieveOperation = TableOperation.Retrieve(id, ServiceConstatns.DocumentEntityHotRowKey);
			TableResult retrieveOperationResult = null;
			try {
				retrieveOperationResult = DocumentTable.Execute(retrieveOperation);
			} catch { }
			ITableEntity tableEntity = (ITableEntity)retrieveOperationResult.Result;
			return tableEntity == null ? false : true;
		}
		public void Save(GeneratedDocumentInfo documentInfo) {
			if(string.IsNullOrEmpty(documentInfo.Id))
				throw new ArgumentNullException("id");
			string filePath = null;
			TableOperation insertOperation;
			if(documentInfo.Document != null) {
				using(var documentStream = new MemoryStream()) {
					PrintingSystemAccessor.SaveIndependentPages(documentInfo.Document.PrintingSystem, documentStream);
					filePath = fileStorage.SaveDocument(documentInfo.Id, documentStream);
				}
			} else if(string.IsNullOrEmpty(documentInfo.FaultMessage)) {
				var hotDocumentEntity = new DocumentTableEntity(documentInfo.Id, ServiceConstatns.DocumentEntityHotRowKey);
				insertOperation = TableOperation.Insert(hotDocumentEntity);
				TableResult result = DocumentTable.Execute(insertOperation);
				return;
			}
			string drillDownString = null;
			if(documentInfo.DrillDownKeys != null)
				drillDownString = JsonSerializer.Stringify<Dictionary<string, bool>>(documentInfo.DrillDownKeys, null);
			var documentEntity = new DocumentTableEntity(documentInfo.Id, ServiceConstatns.DocumentEntityColdRowKey) {
				DrillDownKeys = drillDownString,
				FaultMessage = documentInfo.FaultMessage,
				FilePath = filePath,
				Hash = documentInfo.Hash
			};
			try {
				insertOperation = TableOperation.Insert(documentEntity);
				DocumentTable.Execute(insertOperation);
			} finally {
				var retrieveOperation = TableOperation.Retrieve<DocumentTableEntity>(documentInfo.Id, ServiceConstatns.DocumentEntityHotRowKey);
				var retrieveOperationResult = DocumentTable.Execute(retrieveOperation);
				if(retrieveOperationResult.Result != null) {
					var deleteOperation = TableOperation.Delete((ITableEntity)retrieveOperationResult.Result);
					DocumentTable.Execute(deleteOperation);
				}
			}
		}
		public void Save(string id, XtraReport report) {
			string filePath = null;
			if(report != null)
				using(var reportStream = new MemoryStream()) {
					report.SaveLayout(reportStream, false);
					filePath = fileStorage.SaveReport(id, reportStream);
				}
			var reportEntity = new ReportTableEntity(id) { FilePath = filePath };
			TableOperation insertOperation = TableOperation.Insert(reportEntity);
			ReportTable.Execute(insertOperation);
		}
		public bool TryRemove(string id) {
			if(string.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			TableOperation retrieveCold = TableOperation.Retrieve(id, ServiceConstatns.DocumentEntityColdRowKey);
			TableResult coldResult = DocumentTable.Execute(retrieveCold);
			if(coldResult.Result != null) {
				try {
					TableOperation deleteCold = TableOperation.Delete((ITableEntity)coldResult.Result);
					DocumentTable.Execute(deleteCold);
				} catch {
					return false;
				}
			}
			return true;
		}
		public XtraReport LoadReport(string id) {
			var retriveOperation = TableOperation.Retrieve<ReportTableEntity>(id, ReportTableEntity.ReportTableEntityRowKey);
			TableResult result = ReportTable.Execute(retriveOperation);
			if(result.Result == null)
				throw new ArgumentException("There is no report with id: '" + id + "'");
			var reportTableEntity = (ReportTableEntity)result.Result;
			using(var stream = fileStorage.GetReport(reportTableEntity.FilePath)) {
				stream.Position = 0;
				return XtraReport.FromStream(stream, true);
			}
		}
		public GeneratedDocumentInfo LoadDocument(string id) {
			var retriveOperation = TableOperation.Retrieve<DocumentTableEntity>(id, ServiceConstatns.DocumentEntityColdRowKey);
			TableResult result = DocumentTable.Execute(retriveOperation);
			var documentEntity = (DocumentTableEntity)result.Result;
			Dictionary<string, bool> drillDownKeys = null;
			var ps = XtraReport.CreatePrintingSystem();
			Document document = null;
			using(var stream = fileStorage.GetDocument(documentEntity.FilePath)) {
				ps.LoadDocument(stream);
				document = ps.Document;
				if(!string.IsNullOrEmpty(documentEntity.DrillDownKeys) && documentEntity.DrillDownKeys != "[]")
					drillDownKeys = ActionHelper.Read<Dictionary<string, bool>>(documentEntity.DrillDownKeys);
			}
			return new GeneratedDocumentInfo() {
				Document = document,
				DrillDownKeys = drillDownKeys,
				FaultMessage = documentEntity.FaultMessage,
				Hash = documentEntity.Hash,
				Id = id
			};
		}
		public void Clean(TimeSpan timeToLife) {
			TableQuery<DynamicTableEntity> rangeQuery = new TableQuery<DynamicTableEntity>()
				.Where(TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThan, DateTimeOffset.Now - TimeSpan.FromMinutes(30)));
			TableOperation deleteOperation;
			TableResult result;
			foreach(DynamicTableEntity entity in ReportTable.ExecuteQuery(rangeQuery)) {
				try {
					deleteOperation = TableOperation.Delete(entity);
					result = ReportTable.Execute(deleteOperation);
				} catch(Exception ex) {
					ProcessCleanTableException("Reports", entity.PartitionKey, entity.RowKey, ex);
				}
			}
			foreach(DynamicTableEntity entity in DocumentTable.ExecuteQuery(rangeQuery)) {
				try {
					deleteOperation = TableOperation.Delete(entity);
					result = DocumentTable.Execute(deleteOperation);
				} catch(Exception ex) {
					ProcessCleanTableException("Documents", entity.PartitionKey, entity.RowKey, ex);
				}
			}
		}
		#endregion
		void ProcessCleanTableException(string tableName, string entityPartitionKey, string entityRowKey, Exception e) {
			Logger.Error(string.Format("Cleaner - Cannot clean the {0} container: {1}", tableName, e));
		}
	}
}
