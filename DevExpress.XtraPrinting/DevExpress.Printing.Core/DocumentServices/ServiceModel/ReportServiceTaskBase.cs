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
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.Utils;
namespace DevExpress.DocumentServices.ServiceModel {
	abstract class ReportServiceTaskBase<TResult> {
		protected static readonly TimeSpan DefaultUpdateStatusInterval = TimeSpan.FromMilliseconds(250);
		readonly IReportServiceClient client;
		protected object UserState { get; private set; }
		public IReportServiceClient Client { get { return client; } }
		public TResult Result { get; protected set; }
		public event AsyncCompletedEventHandler Completed;
		public ReportServiceTaskBase(IReportServiceClient client) {
			Guard.ArgumentNotNull(client, "client");
			this.client = client;
		}
		protected abstract void ProcessGeneratedReport(DocumentId documentId);
		protected void CreateDocumentAsync(InstanceIdentity reportIdentity, ReportParameter[] parameters, object asyncState) {
			Guard.ArgumentNotNull(reportIdentity, "reportIdentity");
			UserState = asyncState;
			CreateDocumentOperation operation =
				new CreateDocumentOperation(
					Client,
					reportIdentity,
					new ReportBuildArgs() { Parameters = parameters },
					false,
					DefaultUpdateStatusInterval);
			operation.Completed += createDocument_Completed;
			operation.Start();
		}
		private void createDocument_Completed(object sender, CreateDocumentCompletedEventArgs e) {
			CreateDocumentOperation createDocument = (CreateDocumentOperation)sender;
			createDocument.Completed -= createDocument_Completed;
			if(HasErrorOrCancelled(e))
				return;
			ProcessGeneratedReport(e.DocumentId);
		}
		protected bool HasErrorOrCancelled(AsyncCompletedEventArgs args) {
			if(args.Error != null || args.Cancelled) {
				RaiseCompleted(args.Error, args.Cancelled);
				return true;
			}
			return false;
		}
		protected void RaiseCompleted(Exception error, bool cancelled) {
			if(Completed != null) {
				AsyncCompletedEventArgs args = new AsyncCompletedEventArgs(error, cancelled, UserState);
				Completed(this, args);
			}
		}
	}
}
