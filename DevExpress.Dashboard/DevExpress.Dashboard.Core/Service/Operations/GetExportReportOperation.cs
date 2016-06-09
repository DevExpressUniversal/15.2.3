#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
using System;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Service {
	public class GetExportReportOperation : SessionOperation<ExportArgs> {
		public GetExportReportOperation(IDashboardServer server, IDashboardServiceAdminHandlers admin, ExportArgs args)
			: base(server, admin, args) {
		}
		protected override IList<AffectedItemInfo> ExecuteInternal(bool dataBindingApplied, DashboardServiceResult result, DashboardServerResult serverResult, DashboardSessionState sessionState) {
			DashboardExportData data = DashboardServiceOperationHelper.CreateExportData(serverResult.Session, Args.ExportInfo);
			if (Args.Exporter != null)
				((GetExportReportResult)result).ReportHolder = Args.Exporter.CreateReportHolder(Args.ExportInfo.Mode, data, Args.ExportInfo.ExportOptions);
			else
				throw new Exception("Export does not supported for this platform"); 
			return null;
		}
	}
}
