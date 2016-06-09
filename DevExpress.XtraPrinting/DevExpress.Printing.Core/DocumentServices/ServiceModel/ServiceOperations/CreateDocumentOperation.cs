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
using System.Linq;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
namespace DevExpress.DocumentServices.ServiceModel.ServiceOperations {
	public class CreateDocumentOperation : ReportServiceOperation {
		readonly InstanceIdentity instanceIdentity;
		readonly ReportBuildArgs buildArgs;
		readonly bool shouldRequestReportInfo;
		public event EventHandler<CreateDocumentStartedEventArgs> Started;
		public event EventHandler<CreateDocumentProgressEventArgs> Progress;
		public event EventHandler<CreateDocumentCompletedEventArgs> Completed;
		public event EventHandler<CreateDocumentReportParametersEventArgs> GetReportParameters;
		public override bool CanStop {
			get { return documentId != null; }
		}
		public CreateDocumentOperation(IReportServiceClient client, InstanceIdentity identity, ReportBuildArgs buildArgs, bool shouldRequestReportInfo, TimeSpan statusUpdateInterval)
			: base(client, statusUpdateInterval, null) {
			Guard.ArgumentNotNull(identity, "identity");
			Guard.ArgumentNotNull(buildArgs, "buildArgs");
			instanceIdentity = identity;
			this.buildArgs = buildArgs;
			this.shouldRequestReportInfo = shouldRequestReportInfo;
		}
		public override void Start() {
			Client.GetReportParametersCompleted += client_GetReportParametersCompleted;
			Client.StartBuildCompleted += client_StartBuildCompleted;
			Client.GetBuildStatusCompleted += ClientGetBuildStatusCompleted;
			if(documentId == null) {
				if(shouldRequestReportInfo) {
					Client.GetReportParametersAsync(instanceIdentity, instanceId);
				} else {
					Client.StartBuildAsync(instanceIdentity, buildArgs, instanceId);
				}
			} else {
				StartBuildCompleted();
			}
		}
		public override void Stop() {
			Client.StopBuildAsync(documentId, null);
		}
		public void SetParameters(IParameterContainer parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			buildArgs.Parameters = parameters.ToParameterStubs();
		}
		protected override void UnsubscribeClientEvents() {
			Client.GetReportParametersCompleted -= client_GetReportParametersCompleted;
			Client.StartBuildCompleted -= client_StartBuildCompleted;
			Client.GetBuildStatusCompleted -= ClientGetBuildStatusCompleted;
		}
		protected void OnGetReportParameters(ReportParameterContainer reportParameters) {
			if(GetReportParameters != null) {
				GetReportParameters(this, new CreateDocumentReportParametersEventArgs(reportParameters));
			}
		}
		protected void OnCompleted(DocumentId documentId, Exception error, bool cancelled, object userState) {
			UnsubscribeClientEvents();
			if(!Aborted && Completed != null)
				Completed(this, new CreateDocumentCompletedEventArgs(documentId, error, cancelled, userState));
		}
		void client_GetReportParametersCompleted(object sender, ScalarOperationCompletedEventArgs<ReportParameterContainer> e) {
			if(!IsSameInstanceId(e.UserState))
				return;
			if(TryProcessError(e)) {
				OnCompleted(documentId, e.Error, e.Cancelled, e.UserState);
				return;
			}
			var parameterContainer = e.Result;
			OnGetReportParameters(parameterContainer);
			parameterContainer.ShouldRequestParameters = parameterContainer.ShouldRequestParameters && parameterContainer.Parameters.Any(x => x.Visible);
			if(parameterContainer.ShouldRequestParameters) {
				OnCompleted(documentId, e.Error, e.Cancelled, e.UserState);
			} else {
				Client.StartBuildAsync(instanceIdentity, buildArgs, instanceId);
			}
		}
		void client_StartBuildCompleted(object sender, ScalarOperationCompletedEventArgs<DocumentId> e) {
			if(!IsSameInstanceId(e.UserState))
				return;
			if(TryProcessError(e)) {
				OnCompleted(null, e.Error, e.Cancelled, e.UserState);
				return;
			}
			documentId = e.Result;
			StartBuildCompleted();
		}
		void StartBuildCompleted() {
			RaiseStarted(documentId);
			RaiseCanStopChanged();
			QueryBuildStatus();
		}
		protected void ClientGetBuildStatusCompleted(object sender, ScalarOperationCompletedEventArgs<BuildStatus> e) {
			if(!IsSameInstanceId(e.UserState))
				return;
			if(TryProcessError(e)) {
				OnCompleted(documentId, e.Error, e.Cancelled, e.UserState);
				return;
			}
			RaiseProgress(e.Result.ProgressPosition, e.Result.PageCount);
			if(e.Result.Status != TaskStatus.InProgress) {
				var error = e.Result.Status == TaskStatus.Fault ? e.Result.Fault.ToException() : null;
				OnCompleted(documentId, error, e.Cancelled, e.UserState);
				return;
			}
			QueryBuildStatus();
		}
		void QueryBuildStatus() {
			Delayer.Execute(() => Client.GetBuildStatusAsync(documentId, instanceId));
		}
		protected void RaiseProgress(int progressPosition, int pageCount) {
			if(Progress != null)
				Progress(this, new CreateDocumentProgressEventArgs(progressPosition, pageCount));
		}
		protected void RaiseStarted(DocumentId documentId) {
			if(Started != null)
				Started(this, new CreateDocumentStartedEventArgs(documentId));
		}
	}
}
