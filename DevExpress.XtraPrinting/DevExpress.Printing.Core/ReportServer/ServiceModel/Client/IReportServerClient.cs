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
using DevExpress.Data.XtraReports.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.ReportServer.ServiceModel.DataContracts;
namespace DevExpress.ReportServer.ServiceModel.Client {
	public interface IReportServerClient : IReportServiceClient, IReportWizardServiceClient {
		IContextChannel ContextChannel { get; }
		#region Report
		void UploadLayout(Stream layout, object asyncState, Action<ScalarOperationCompletedEventArgs<TransientReportId>> onCompleted);
		void GetReports(object asyncState, Action<ScalarOperationCompletedEventArgs<IEnumerable<ReportCatalogItemDto>>> onCompleted);
		void CreateReport(ReportDto message, object asyncState, Action<ScalarOperationCompletedEventArgs<CreateReportResult>> onCompleted);
		void LoadReport(int id, object asyncState, Action<ScalarOperationCompletedEventArgs<ReportDto>> onCompleted);
		void LockReport(int id);
		void UnlockReport(int id);
		void SaveReportById(int reportId, ReportDto reportDto, object asyncState, Action<ScalarOperationCompletedEventArgs<int>> onCompleted);
		void UpdateReport(int reportId, ReportDto reportDto, object asyncState, Action<ScalarOperationCompletedEventArgs<int>> onCompleted);
		void DeleteReport(int reportId, object asyncState, Action<AsyncCompletedEventArgs> onCompleted);
		void LoadReportLayoutByRevisionId(int reportId, int revisionId, object asyncState, Action<ScalarOperationCompletedEventArgs<byte[]>> onCompleted);
		void GetReportRevisions(int reportId, object asyncState, Action<ScalarOperationCompletedEventArgs<IEnumerable<LayoutRevisionDto>>> onCompleted);
		void RollbackReportLayout(int reportId, int revisionId, object asyncState, Action<AsyncCompletedEventArgs> onCompleted);
		ReportCatalogItemDto GetReportCatalogItemDto(int reportId);
		void CloneReport(int sourceReportId, ReportDto reportDto, object asyncState, Action<ScalarOperationCompletedEventArgs<ReportDto>> onCompleted);
		#endregion
		#region Category
		void GetCategories(object asyncState, Action<ScalarOperationCompletedEventArgs<IEnumerable<CategoryDto>>> onCompleted);
		void CreateReportCategory(string categoryName, object asyncState, Action<ScalarOperationCompletedEventArgs<int>> onCompleted);
		void UpdateReportCategory(int categoryId, string categoryName, object asyncState, Action<AsyncCompletedEventArgs> onCompleted);
		void DeleteReportCategory(int categoryId, object asyncState, Action<AsyncCompletedEventArgs> onCompleted);
		#endregion
		#region Data Model
		string GetDataSourceSchema(int dataSourceId, object asyncState);
		#endregion
		void Ping(Action<AsyncCompletedEventArgs> onCompleted, object asyncState);
		#region Task-based operations
		#region Report
		Task<IEnumerable<ReportCatalogItemDto>> GetReportsAsync(object asyncState);
		Task<ReportDto> LoadReportAsync(int id, object asyncState);
		#endregion
		#region Category
		Task<IEnumerable<CategoryDto>> GetCategoriesAsync(object asyncState);
		Task<int> CreateCategoryAsync(string name, object asyncState);
		Task UpdateCategoryAsync(int id, string name, object asyncState);
		Task DeleteCategoryAsync(int id, object asyncState);
		#endregion
		#region Data Model
		Task<IEnumerable<DataModelDto>> GetDataModelsAsync(object asyncState);
		Task<DataModelDto> GetDataModelAsync(int id, object asyncState);
		Task UpdateDataModelAsync(DataModelDto dataModel, object asyncState);
		Task DeleteDataModelAsync(int id, object asyncState);
		#endregion
		#region Scheduler
		Task<int?> ExecuteJobAsync(int scheduledJobId, int? scheduledJobResult, object asyncState);
		#endregion
		#endregion
	}
}
