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
using System.Threading.Tasks;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.DocumentServices.ServiceModel {
	public static class ReportServiceTaskFactoryExtensions {
		public static Task<byte[]> ExportReportAsync(this TaskFactory taskFactory, IReportServiceClient client, string reportName, ExportOptionsBase exportOptions, ReportParameter[] parameters, object asyncState) {
			Guard.ArgumentIsNotNullOrEmpty(reportName, "reportName");
			return ExportReportAsync(taskFactory, client, new ReportNameIdentity(reportName), exportOptions, parameters, asyncState);
		}
		public static Task<byte[]> ExportReportAsync(this TaskFactory taskFactory, IReportServiceClient client, InstanceIdentity reportIdentity, ExportOptionsBase exportOptions, ReportParameter[] parameters, object asyncState) {
			Guard.ArgumentNotNull(client, "client");
			Guard.ArgumentNotNull(reportIdentity, "reportIdentity");
			Guard.ArgumentNotNull(exportOptions, "exportOptions");
			TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>(asyncState);
			ExportReportTask aepTask = new ExportReportTask(client);
			aepTask.Completed += ReportServiceTaskCompleted<byte[]>;
			aepTask.ExecuteAsync(reportIdentity, exportOptions, parameters, tcs);
			return tcs.Task;
		}
		static void ReportServiceTaskCompleted<TResult>(object sender, AsyncCompletedEventArgs e) {
			ReportServiceTaskBase<TResult> aepTask = (ReportServiceTaskBase<TResult>)sender;
			aepTask.Completed -= ReportServiceTaskCompleted<TResult>;
			TaskCompletionSource<TResult> tcs = (TaskCompletionSource<TResult>)e.UserState;
			if(e.Error != null) {
				tcs.SetException(e.Error);
			} else if(e.Cancelled) {
				tcs.SetCanceled();
			} else {
				tcs.SetResult(aepTask.Result);
			}
		}
	}
}
