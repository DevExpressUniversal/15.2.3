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
using System.ComponentModel;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DocumentServices.ServiceModel.ServiceOperations {
	public class PrintDocumentOperation : ReportServiceOperation {
		PrintId printId;
		PrintingStatus printingStatus = PrintingStatus.None;
		bool stopped;
		readonly string documentName;
		readonly PageCompatibility compatibility;
		public event EventHandler<PrintDocumentStartedEventArgs> Started;
		public event EventHandler<PrintDocumentProgressEventArgs> Progress;
		public event EventHandler<PrintDocumentCompletedEventArgs> Completed;
		readonly protected bool fIsDirectPrinting;
		public bool IsDirectPrinting {
			get { return fIsDirectPrinting; }
		}
		public PrintDocumentOperation(IReportServiceClient client, DocumentId documentId, PageCompatibility compatibility, string documentName, TimeSpan statusUpdateInterval, bool isDirectPrinting)
			: base(client, statusUpdateInterval, documentId) {
			Guard.ArgumentNotNull(documentId, "documentId");
			Guard.ArgumentNotNull(documentName, "documentName");
			this.documentName = documentName;
			this.fIsDirectPrinting = isDirectPrinting;
			this.compatibility = compatibility;
		}
		public override void Start() {
			Client.StartPrintCompleted += client_StartPrintCompleted;
			Client.StopPrintCompleted += client_StopPrintCompleted;
			Client.GetPrintStatusCompleted += client_GetPrintStatusCompleted;
			Client.GetPrintDocumentCompleted += client_GetPrintDocumentCompleted;
			Client.StartPrintAsync(documentId, compatibility, instanceId);
			stopped = false;
		}
		public override bool CanStop {
			get { return !stopped && printId != null && printingStatus == PrintingStatus.Generating; }
		}
		public override void Stop() {
			Client.StopPrintAsync(printId, instanceId);
			stopped = true;
		}
		protected override void UnsubscribeClientEvents() {
			Client.StartPrintCompleted -= client_StartPrintCompleted;
			Client.StopPrintCompleted -= client_StopPrintCompleted;
			Client.GetPrintStatusCompleted -= client_GetPrintStatusCompleted;
			Client.GetPrintDocumentCompleted -= client_GetPrintDocumentCompleted;
		}
		void QueryPrintStatus() {
			Delayer.Execute(() => Client.GetPrintStatusAsync(printId, instanceId));
		}
		void ProcessGetPrintDocument() {
			Client.GetPrintDocumentAsync(printId, instanceId);
		}
		void OnStarted() {
			if(Started != null)
				Started(this, new PrintDocumentStartedEventArgs(printId));
		}
		void OnCompleted(string[] pages, Exception fault, bool cancelled, object userState) {
			UnsubscribeClientEvents();
			Delayer.Abort();
			printId = null;
			printingStatus = PrintingStatus.None;
			if(Completed != null)
				Completed(this, new PrintDocumentCompletedEventArgs(pages, stopped, printId, fault, cancelled, userState));
		}
		void OnCompleted(Exception fault, bool cancelled, object userState) {
			OnCompleted(null, fault, cancelled, userState);
		}
		void OnProgress(int progressPosition) {
			if(Progress != null)
				Progress(this, new PrintDocumentProgressEventArgs(progressPosition));
		}
		void client_StartPrintCompleted(object sender, ScalarOperationCompletedEventArgs<PrintId> e) {
			AssertAlive();
			AssertInstanceId(e.UserState);
			if(e.Cancelled || e.Error != null) {
				OnCompleted(e.Error, e.Cancelled, e.UserState);
				return;
			}
			printId = e.Result;
			printingStatus = PrintingStatus.Generating;
			OnStarted();
			RaiseCanStopChanged();
			QueryPrintStatus();
		}
		void client_StopPrintCompleted(object sender, AsyncCompletedEventArgs e) {
			AssertAlive();
			AssertInstanceId(e.UserState);
			OnCompleted(e.Error, e.Cancelled, e.UserState);
		}
		void client_GetPrintStatusCompleted(object sender, ScalarOperationCompletedEventArgs<PrintStatus> e) {
			AssertAlive();
			AssertInstanceId(e.UserState);
			if(e.Cancelled || e.Error != null || e.Result.Status == TaskStatus.Fault) {
				var error = e.Error ?? (e.Result.Status == TaskStatus.Fault ? e.Result.Fault.ToException() : null);
				OnCompleted(error, e.Cancelled, e.UserState);
				return;
			}
			OnProgress(e.Result.ProgressPosition);
			if(e.Result.Status == TaskStatus.Complete) {
				ProcessGetPrintDocument();
				return;
			}
			QueryPrintStatus();
		}
		void client_GetPrintDocumentCompleted(object sender, ScalarOperationCompletedEventArgs<byte[]> e) {
			AssertAlive();
			AssertInstanceId(e.UserState);
			if(e.Cancelled || e.Error != null) {
				OnCompleted(e.Error, e.Cancelled, e.UserState);
				return;
			}
			OnCompleted(Helper.DeserializePages(e.Result), e.Error, e.Cancelled, e.UserState);
		}
	}
}
