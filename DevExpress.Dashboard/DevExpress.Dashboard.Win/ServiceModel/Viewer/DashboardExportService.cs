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

using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardExport;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils;
using System.Collections;
using System.IO;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDashboardExportService {
		void PerformExport(string filePath, ExportInfo exportInfo, Hashtable clientState);
		void PerformExport(Stream stream, ExportInfo exportInfo, Hashtable clientState);
		IReportHolder GetExportReport(ExportInfo exportInfo, Hashtable clientState);
	}
	public class DashboardExportService : IDashboardExportService {
		readonly DashboardViewer dashboardViewer;
		DashboardServiceClient ServiceClient { get { return dashboardViewer.ServiceClient; } }
		public DashboardExportService(DashboardViewer dashboardViewer) {
			Guard.ArgumentNotNull(dashboardViewer, "dashboardViewer");
			this.dashboardViewer = dashboardViewer;
		}
		public void PerformExport(string filePath, ExportInfo exportInfo, Hashtable clientState) {
			Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite); 
			try {
				PerformExport(stream, exportInfo, clientState);
				stream.Close();
			} catch {
				stream.Close();
				File.Delete(filePath);
				throw;
			}
		}
		public void PerformExport(Stream stream, ExportInfo exportInfo, Hashtable clientState) {
			ServiceClient.Export(exportInfo, stream, clientState);
		}
		public IReportHolder GetExportReport(ExportInfo exportInfo, Hashtable clientState) {
			return ServiceClient.GetExportReport(exportInfo, clientState);
		}
	}
}
