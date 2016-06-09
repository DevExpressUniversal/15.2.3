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
using System.IO;
using System.ServiceModel;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.Xpf.Printing;
namespace DevExpress.ReportServer.ServiceModel.Client {
	[ServiceContract(Name = "IReportServerFacade")]
	[ServiceKnownType(ServiceKnownTypeProvider.GetKnownTypesMethodName, typeof(ServiceKnownTypeProvider))]
	public interface IReportServerFacadeAsync : IAsyncReportService {
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUploadLayout(Stream layout, AsyncCallback callback, object asyncState);
		TransientReportId EndUploadLayout(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetReports(AsyncCallback callback, object asyncState);
		IEnumerable<ReportCatalogItemDto> EndGetReports(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCreateReport(ReportDto message, AsyncCallback callback, object asyncState);
		CreateReportResult EndCreateReport(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginLoadReport(int id, AsyncCallback callback, object asyncState);
		ReportDto EndLoadReport(IAsyncResult ar);
		[OperationContract]
		void LockReport(int reportId);
		[OperationContract]
		void UnlockReport(int reportId);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginSaveReportById(int reportId, ReportDto reportDto, AsyncCallback callback, object asyncState);
		int EndSaveReportById(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUpdateReport(int reportId, ReportDto reportDto, AsyncCallback callback, object asyncState);
		int EndUpdateReport(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDeleteReport(int reportId, AsyncCallback callback, object asyncState);
		void EndDeleteReport(IAsyncResult ar);
		[OperationContract]
		string GetDataSourceSchema(int dataSourceId, object asyncState);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginLoadReportLayoutByRevisionId(int reportId, int revisionId, AsyncCallback callback, object asyncState);
		byte[] EndLoadReportLayoutByRevisionId(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginPing(AsyncCallback callback, object asyncState);
		void EndPing(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetReportRevisions(int reportId, AsyncCallback callback, object asyncState);
		IEnumerable<LayoutRevisionDto> EndGetReportRevisions(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginRollBackReportLayout(int reportId, int revisionId, AsyncCallback callback, object asyncState);
		void EndRollBackReportLayout(IAsyncResult ar);
		[OperationContract]
		ReportCatalogItemDto GetReportCatalogItemDto(int reportId);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCloneReport(int sourceReportId, ReportDto reportDto, AsyncCallback callback, object asyncState);
		ReportDto EndCloneReport(IAsyncResult ar);
		#region Category
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetCategories(AsyncCallback callback, object asyncState);
		IEnumerable<CategoryDto> EndGetCategories(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginCreateReportCategory(string categoryName, AsyncCallback callback, object asyncState);
		int EndCreateReportCategory(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUpdateReportCategory(int categoryId, string name, AsyncCallback callback, object asyncState);
		void EndUpdateReportCategory(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDeleteCategory(int categoryId, AsyncCallback callback, object asyncState);
		void EndDeleteCategory(IAsyncResult ar);
		#endregion
		#region DataModels
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetDataModels(AsyncCallback callback, object asyncState);
		IEnumerable<DataModelDto> EndGetDataModels(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetDataModel(int id, AsyncCallback callback, object asyncState);
		DataModelDto EndGetDataModel(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginUpdateDataModel(DataModelDto dataModel, AsyncCallback callback, object asyncState);
		void EndUpdateDataModel(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginDeleteDataModel(int id, AsyncCallback callback, object asyncState);
		void EndDeleteDataModel(IAsyncResult ar);
		#endregion
		#region Scheduler
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginExecuteJob(int scheduledJobId, int? scheduledJobResult, AsyncCallback callback, object asyncState);
		int? EndExecuteJob(IAsyncResult ar);
		#endregion
		#region Wizard
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetDataSources(AsyncCallback callback, object asyncState);
		IEnumerable<StoredDataSourceInfo> EndGetDataSources(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetDataMembers(string dataSourceName, AsyncCallback callback, object asyncState);
		IEnumerable<TableInfo> EndGetDataMembers(IAsyncResult ar);
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginGetColumns(string dataSourceName, TableInfo dataMemberName, AsyncCallback callback, object asyncState);
		IEnumerable<ColumnInfo> EndGetColumns(IAsyncResult ar);
		#endregion
	}
}
