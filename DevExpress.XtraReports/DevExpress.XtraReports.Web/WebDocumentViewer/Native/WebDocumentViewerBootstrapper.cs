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

using System;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
using System.Web.SessionState;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native {
	public static class WebDocumentViewerBootstrapper {
		readonly static List<Action<IServiceContainerRegistrator>> appendServices = new List<Action<IServiceContainerRegistrator>>(0);
		public static SessionStateBehavior SessionState = SessionStateBehavior.Disabled;
		internal static ServiceIntegrityContainer CreateInitializedContainer() {
			var container = new ServiceIntegrityContainer();
			var registrator = new IntegrityContainerRegistrator(container);
			RegisterStandardServices(registrator);
			foreach(var action in appendServices) {
				action(registrator);
			}
			return container;
		}
		public static void AppendRegistration(Action<IServiceContainerRegistrator> action) {
			Guard.ArgumentNotNull(action, "action");
			appendServices.Add(action);
		}
		public static void RegisterStandardServices(IServiceContainerRegistrator registrator) {
			registrator.RegisterSingleton<IDocumentManagementService, DocumentManagementService>();
			registrator.RegisterSingleton<IDocumentStorage, InMemoryDocumentStorage>();
			registrator.RegisterSingleton<IReportManagementService, ReportManagementService>();
			registrator.RegisterSingleton<IStoragesCleaner, ThreadingTimerStoragesCleaner>();
			registrator.RegisterInstance(StoragesCleanerSettings.CreateDefault());
			registrator.RegisterSingleton<IReportHashCodeGenerator, EmptyReportHashCodeGenerator>();
			registrator.RegisterSingleton<IDocumentBuilder, DocumentBuilder>();
			registrator.RegisterSingleton<IWebDocumentViewerRequestManager, WebDocumentViewerRequestManager>();
			registrator.RegisterTransient<IWebDocumentViewerRequestController, WebDocumentViewerRequestController>();
			registrator.RegisterSingleton<ILocalizationInfoProvider, WebDocumentViewerLocalizationInfoProvider>();
			registrator.RegisterSingleton<IJSContentGenerator<WebDocumentViewerModel>, WebDocumentViewerJSContentGenerator>();
			registrator.RegisterSingleton<IWebDocumentViewerHtmlContentGenerator, WebDocumentViewerHtmlContentGenerator>();
			registrator.RegisterTransient<IWebDocumentViewerModelGenerator, WebDocumentViewerModelGenerator>();
			registrator.RegisterSingleton<IWebDocumentViewerReportResolver, WebDocumentViewerReportResolver>();
			registrator.RegisterSingleton<UrlResolver, WebDocumentViewerUrlResolver>();
			registrator.RegisterSingleton<IPollingOperationService<PrintingSystemBase, ExportedDocument>, ExportPollingOperationService>();
			registrator.RegisterSingleton<IExportedDocumentStorage, ExportedDocumentStorage>();
		}
	}
}
