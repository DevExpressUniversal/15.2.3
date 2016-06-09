#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public class WebDocumentViewerModelGenerator : IWebDocumentViewerModelGenerator {
		readonly IReportManagementService reportManagementService;
		public WebDocumentViewerModelGenerator(IReportManagementService reportManagementService) {
			this.reportManagementService = reportManagementService;
		}
		public WebDocumentViewerModel Generate(IReportModelInfo reportModel, IEnumerable<ClientControlsMenuItemModel> menuItems) {
			menuItems = menuItems ?? Enumerable.Empty<ClientControlsMenuItemModel>();
			var reportInfo = new ReportToPreview();
			reportInfo.ReportId = reportManagementService.GetId(reportModel.ReportIdentity);
			reportInfo.ParametersInfo = reportManagementService.GetParameters(reportInfo.ReportId);
			var fixedExportOptions = ExportingStrategy.GetFixedExportOptions(reportModel.ExportOptions);
			reportInfo.ExportOptions = ReportLayoutJsonSerializer.GetExportOptionsJson(fixedExportOptions);
			var menuItemsContracts = ContractConverter.ConvertToContracts(menuItems);
			var menuItemsJSClickActions = ContractConverter.GetMenuItemsJSClickActions(menuItems);
			return new WebDocumentViewerModel {
				ReportInfo = reportInfo,
				MenuActions = menuItemsContracts,
				MenuItemJSClickActions = menuItemsJSClickActions,
			};
		}
	}
}
