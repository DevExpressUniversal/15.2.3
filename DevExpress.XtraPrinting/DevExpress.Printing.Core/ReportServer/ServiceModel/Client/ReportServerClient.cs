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
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.ReportServer.ServiceModel.DataContracts;
namespace DevExpress.ReportServer.ServiceModel.Client {
	public class ReportServerClient : ReportServiceClient, IReportServerClient {
		new IReportServerFacadeAsync Channel {
			get { return (IReportServerFacadeAsync)base.Channel; }
		}
		public IContextChannel ContextChannel {
			get { return (IContextChannel)Channel; }
		}
		[Obsolete("Use the ReportServerClient(IReportServerFacadeAsync channel) constructor instead.")]
		public ReportServerClient(IReportServerFacadeAsync channel, string restEndpointAddress)
			: base((IAsyncReportService)channel) {
		}
		public ReportServerClient(IReportServerFacadeAsync channel)
			: base((IAsyncReportService)channel) {
		}
		public void UploadLayout(Stream layout, object asyncState, Action<ScalarOperationCompletedEventArgs<TransientReportId>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndUploadLayout, () => (s, args) => onCompleted(args));
			Channel.BeginUploadLayout(layout, callback, asyncState);
		}
		public void GetReports(object asyncState, Action<ScalarOperationCompletedEventArgs<IEnumerable<ReportCatalogItemDto>>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation<IEnumerable<ReportCatalogItemDto>>(ar, Channel.EndGetReports, () => (s, args) => onCompleted(args));
			Channel.BeginGetReports(callback, asyncState);
		}
		public void CreateReport(ReportDto message, object asyncState, Action<ScalarOperationCompletedEventArgs<CreateReportResult>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndCreateReport, () => (s, args) => onCompleted(args));
			Channel.BeginCreateReport(message, callback, asyncState);
		}
		public void LoadReport(int reportId, object asyncState, Action<ScalarOperationCompletedEventArgs<ReportDto>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndLoadReport, () => (s, args) => onCompleted(args));
			Channel.BeginLoadReport(reportId, callback, asyncState);
		}
		public void UpdateReport(int reportId, ReportDto reportDto, object asyncState, Action<ScalarOperationCompletedEventArgs<int>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndUpdateReport, () => (s, args) => onCompleted(args));
			Channel.BeginUpdateReport(reportId, reportDto, callback, asyncState);
		}
		public void DeleteReport(int reportId, object asyncState, Action<AsyncCompletedEventArgs> onCompleted) {
			AsyncCallback callback = ar => EndVoidOperation(ar, Channel.EndDeleteReport, () => (s, args) => onCompleted(args));
			Channel.BeginDeleteReport(reportId, callback, asyncState);
		}
		public void LockReport(int reportId) {
			Channel.LockReport(reportId);
		}
		public void UnlockReport(int reportId) {
			Channel.UnlockReport(reportId);
		}
		public void SaveReportById(int reportId, ReportDto reportDto, object asyncState, Action<ScalarOperationCompletedEventArgs<int>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndSaveReportById, () => (s, args) => onCompleted(args));
			Channel.BeginSaveReportById(reportId, reportDto, callback, asyncState);
		}
		public void GetCategories(object asyncState, Action<ScalarOperationCompletedEventArgs<IEnumerable<CategoryDto>>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetCategories, () => (s, args) => onCompleted(args));
			Channel.BeginGetCategories(callback, asyncState);
		}
		public void CreateReportCategory(string categoryName, object asyncState, Action<ScalarOperationCompletedEventArgs<int>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndCreateReportCategory, () => (s, e) => onCompleted(e));
			Channel.BeginCreateReportCategory(categoryName, callback, asyncState);
		}
		public void UpdateReportCategory(int categoryId, string name, object asyncState, Action<AsyncCompletedEventArgs> onCompleted) {
			AsyncCallback callback = ar => EndVoidOperation(ar, Channel.EndUpdateReportCategory, () => (s, args) => onCompleted(args));
			Channel.BeginUpdateReportCategory(categoryId, name, callback, asyncState);
		}
		public void DeleteReportCategory(int categoryId, object asyncState, Action<AsyncCompletedEventArgs> onCompleted) {
			AsyncCallback callback = ar => EndVoidOperation(ar, Channel.EndDeleteCategory, () => (s, args) => onCompleted(args));
			Channel.BeginDeleteCategory(categoryId, callback, asyncState);
		}
		public string GetDataSourceSchema(int dataSourceId, object asyncState) {
			return Channel.GetDataSourceSchema(dataSourceId, asyncState);
		}
		public void LoadReportLayoutByRevisionId(int reportId, int revisionId, object asyncState, Action<ScalarOperationCompletedEventArgs<byte[]>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndLoadReportLayoutByRevisionId, () => (s, args) => onCompleted(args));
			Channel.BeginLoadReportLayoutByRevisionId(reportId, revisionId, callback, asyncState);
		}
		public void Ping(Action<AsyncCompletedEventArgs> onCompleted, object asyncState) {
			AsyncCallback callback = ar => EndVoidOperation(ar, Channel.EndPing, () => (s, args) => onCompleted(args));
			Channel.BeginPing(callback, asyncState);
		}
		public void GetReportRevisions(int reportId, object asyncState, Action<ScalarOperationCompletedEventArgs<IEnumerable<LayoutRevisionDto>>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetReportRevisions, () => (s, args) => onCompleted(args));
			Channel.BeginGetReportRevisions(reportId, callback, asyncState);
		}
		public void RollbackReportLayout(int reportId, int revisionId, object asyncState, Action<AsyncCompletedEventArgs> onCompleted) {
			AsyncCallback callback = ar => EndVoidOperation(ar, Channel.EndRollBackReportLayout, () => (s, args) => onCompleted(args));
			Channel.BeginRollBackReportLayout(reportId, revisionId, callback, asyncState);
		}
		public ReportCatalogItemDto GetReportCatalogItemDto(int reportId) {
			return Channel.GetReportCatalogItemDto(reportId);
		}
		#region IReportWizardServiceClient
		public void GetColumnsAsync(string dataSourceName, TableInfo dataMemberName, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetColumns, () => GetColumnsCompleted);
			Channel.BeginGetColumns(dataSourceName, dataMemberName, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<IEnumerable<ColumnInfo>>> GetColumnsCompleted;
		public void GetDataSourcesAsync(object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetDataSources, () => GetDataSourcesCompleted);
			Channel.BeginGetDataSources(callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<IEnumerable<DataSourceInfo>>> GetDataSourcesCompleted;
		public void GetDataMembersAsync(string dataSourceName, object asyncState) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndGetDataMembers, () => GetDataMembersCompleted);
			Channel.BeginGetDataMembers(dataSourceName, callback, asyncState);
		}
		public event EventHandler<ScalarOperationCompletedEventArgs<IEnumerable<TableInfo>>> GetDataMembersCompleted;
		public void CloneReport(int sourceReportId, ReportDto reportDto, object asyncState, Action<ScalarOperationCompletedEventArgs<ReportDto>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndCloneReport, () => (s, args) => onCompleted(args));
			Channel.BeginCloneReport(sourceReportId, reportDto, callback, asyncState);
		}
		#endregion
		#region Task-based operations
		#region Report
		public Task<IEnumerable<ReportCatalogItemDto>> GetReportsAsync(object asyncState) {
			return Task.Factory.FromAsync<IEnumerable<ReportCatalogItemDto>>(Channel.BeginGetReports, Channel.EndGetReports, asyncState);
		}
		public Task<ReportDto> LoadReportAsync(int id, object asyncState) {
			return Task.Factory.FromAsync<int, ReportDto>(Channel.BeginLoadReport, Channel.EndLoadReport, id, asyncState);
		}
		#endregion
		#region Category
		public Task<IEnumerable<CategoryDto>> GetCategoriesAsync(object asyncState) {
			return Task.Factory.FromAsync<IEnumerable<CategoryDto>>(Channel.BeginGetCategories, Channel.EndGetCategories, asyncState);
		}
		public Task<int> CreateCategoryAsync(string name, object asyncState) {
			return Task.Factory.FromAsync<string, int>(Channel.BeginCreateReportCategory, Channel.EndCreateReportCategory, name, asyncState);
		}
		public Task UpdateCategoryAsync(int id, string name, object asyncState) {
			return Task.Factory.FromAsync<int, string>(Channel.BeginUpdateReportCategory, Channel.EndUpdateReportCategory, id, name, asyncState);
		}
		public Task DeleteCategoryAsync(int id, object asyncState) {
			return Task.Factory.FromAsync<int>(Channel.BeginDeleteCategory, Channel.EndDeleteCategory, id, asyncState);
		}
		#endregion
		#region DataModel
		public Task<IEnumerable<DataModelDto>> GetDataModelsAsync(object asyncState) {
			return Task.Factory.FromAsync<IEnumerable<DataModelDto>>(Channel.BeginGetDataModels, Channel.EndGetDataModels, asyncState);
		}
		public Task<DataModelDto> GetDataModelAsync(int id, object asyncState) {
			return Task.Factory.FromAsync<int, DataModelDto>(Channel.BeginGetDataModel, Channel.EndGetDataModel, id, asyncState);
		}
		public Task UpdateDataModelAsync(DataModelDto dataModel, object asyncState) {
			return Task.Factory.FromAsync<DataModelDto>(Channel.BeginUpdateDataModel, Channel.EndUpdateDataModel, dataModel, asyncState);
		}
		public Task DeleteDataModelAsync(int id, object asyncState) {
			return Task.Factory.FromAsync<int>(Channel.BeginDeleteDataModel, Channel.EndDeleteDataModel, id, asyncState);
		}
		#endregion
		#region Scheduler
		public Task<int?> ExecuteJobAsync(int scheduledJobId, int? scheduledJobResult, object asyncState) {
			return Task.Factory.FromAsync<int, int?, int?>(Channel.BeginExecuteJob, Channel.EndExecuteJob, scheduledJobId, scheduledJobResult, asyncState);
		}
		#endregion
		#endregion
	}
}
