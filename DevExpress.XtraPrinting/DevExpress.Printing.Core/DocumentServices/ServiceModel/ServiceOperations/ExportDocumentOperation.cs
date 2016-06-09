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
using System.Diagnostics;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DocumentServices.ServiceModel.ServiceOperations {
	public class ExportDocumentOperation : ReportServiceOperation {
		protected ExportId exportId;
		readonly protected DocumentExportArgs exportArgs;
		public event EventHandler<ExportDocumentStartedEventArgs> Started;
		public event EventHandler<ExportDocumentProgressEventArgs> Progress;
		public event EventHandler<ExportDocumentCompletedEventArgs> Completed;
		public override bool CanStop {
			get { return false; }
		}
		public ExportDocumentOperation(IReportServiceClient client, DocumentId documentId, ExportFormat format, ExportOptions exportOptions, TimeSpan statusUpdateInterval)
			: this(client, documentId, format, exportOptions, statusUpdateInterval, null) {
		}
		public ExportDocumentOperation(IReportServiceClient client, DocumentId documentId, ExportFormat format, ExportOptions exportOptions, TimeSpan statusUpdateInterval, object customArgs)
			: base(client, statusUpdateInterval, documentId) {
			Guard.ArgumentNotNull(exportOptions, "exportOptions");
			exportArgs = Helper.CreateDocumentExportArgs(format, exportOptions, customArgs);
		}
		public ExportDocumentOperation(IReportServiceClient client, DocumentId documentId, DocumentExportArgs exportArgs, TimeSpan statusUpdateInterval)
			: base(client, statusUpdateInterval, documentId) {
			Guard.ArgumentNotNull(exportArgs, "exportArgs");
			this.exportArgs = exportArgs;
		}
		public override void Start() {
			Client.StartExportCompleted += client_StartExportCompleted;
			Client.GetExportStatusCompleted += client_GetTaskStatusCompleted;
			Client.GetExportedDocumentCompleted += client_GetExportedDocumentCompleted;
			Client.StartExportAsync(documentId, exportArgs, instanceId);
		}
		public override void Stop() {
			throw new NotSupportedException("Export document operation can't be stopped.");
		}
		protected override void UnsubscribeClientEvents() {
			Client.StartExportCompleted -= client_StartExportCompleted;
			Client.GetExportStatusCompleted -= client_GetTaskStatusCompleted;
			Client.GetExportedDocumentCompleted -= client_GetExportedDocumentCompleted;
		}
		protected void OnCompleted(ExportDocumentCompletedEventArgs args) {
			UnsubscribeClientEvents();
			if(Completed != null)
				Completed(this, args);
		}
		void RaiseProgress(ExportDocumentProgressEventArgs args) {
			if(Progress != null)
				Progress(this, args);
		}
		void client_StartExportCompleted(object sender, ScalarOperationCompletedEventArgs<ExportId> e) {
			AssertAlive();
			AssertInstanceId(e.UserState);
			if(e.Cancelled || e.Error != null) {
				OnCompleted(new ExportDocumentCompletedEventArgs(null, null, e.Error, e.Cancelled, e.UserState));
				return;
			}
			exportId = e.Result;
			RaiseStarted(exportId);
			QueryTaskStatus();
		}
		void RaiseStarted(ExportId exportId) {
			if(Started != null)
				Started(this, new ExportDocumentStartedEventArgs(exportId));
		}
		void client_GetTaskStatusCompleted(object sender, ScalarOperationCompletedEventArgs<ExportStatus> e) {
			AssertAlive();
			AssertInstanceId(e.UserState);
			if(e.Cancelled || e.Error != null || e.Result.Status == TaskStatus.Fault) {
				var error = e.Error ?? e.Result.Fault.ToException();
				OnCompleted(new ExportDocumentCompletedEventArgs(null, exportId, error, e.Cancelled, e.UserState));
				return;
			}
			RaiseProgress(new ExportDocumentProgressEventArgs(e.Result.ProgressPosition));
			if(e.Result.Status == TaskStatus.InProgress) {
				QueryTaskStatus();
				return;
			}
			Debug.Assert(e.Result.Status == TaskStatus.Complete);
			OnAfterGetTaskStatusCompleted(exportId, instanceId, e);
		}
		void QueryTaskStatus() {
			Delayer.Execute(() => Client.GetExportStatusAsync(exportId, instanceId));
		}
		void OnAfterGetTaskStatusCompleted(ExportId exportId, Guid instanceId, ScalarOperationCompletedEventArgs<ExportStatus> e) {
			Client.GetExportedDocumentAsync(exportId, instanceId);
		}
		void client_GetExportedDocumentCompleted(object sender, ScalarOperationCompletedEventArgs<byte[]> e) {
			AssertAlive();
			AssertInstanceId(e.UserState);
			OnCompleted(new ExportDocumentCompletedEventArgs(e.Error == null ? e.Result : null, exportId, e.Error, e.Cancelled, e.UserState));
		}
	}
}
