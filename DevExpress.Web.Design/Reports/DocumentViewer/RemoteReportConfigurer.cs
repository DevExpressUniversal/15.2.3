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

using System.ComponentModel;
using System.ServiceModel;
using System.Windows.Forms;
using DevExpress.Data.Utils;
using DevExpress.ReportServer.Printing;
using DevExpress.ReportServer.ServiceModel.Client;
using DevExpress.XtraPrinting.Design;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign;
using DevExpress.XtraReports.Web;
using EnvDTE;
namespace DevExpress.Web.Design.Reports.DocumentViewer {
	static class RemoteReportConfigurer {
		public static bool Configure(ASPxDocumentViewer component, string remoteSourceCachedUserName, out RemoteDocumentSourceModel model) {
			var configHelper = new AppConfigHelper(component.Site);
			var launcher = new DocumentViewerWizardLauncherService(CreateWizardModel(component, remoteSourceCachedUserName));
			var site = component.Site;
			if(launcher.Start(site, configHelper) == DialogResult.OK) {
				model = launcher.WizardModel;
				OnSuccessConfigure(site, model, configHelper);
				return true;
			}
			model = null;
			return false;
		}
		static void OnSuccessConfigure(ISite site, RemoteDocumentSourceModel model, AppConfigHelper configHelper) {
			var projectItem = site.GetService<ProjectItem>();
			ReferenceHelper.AddReferences(projectItem, typeof(EndpointAddress).Assembly, typeof(ReportServerClientFactory).Assembly);
			if(model.GenerateEndpoints) {
				configHelper.ConfigureEndpoints(model.Endpoint, false);
			}
		}
		static RemoteDocumentSourceModel CreateWizardModel(ASPxDocumentViewer component, string remoteSourceCachedUserName) {
			var settings = component.SettingsRemoteSource;
			return new RemoteDocumentSourceModel {
				DocumentSourceType = settings.AuthenticationType == AuthenticationType.None
					? RemoteDocumentSourceType.ReportService
					: RemoteDocumentSourceType.ReportServer,
				AuthenticationType = settings.AuthenticationType,
				Endpoint = settings.EndpointConfigurationName,
				GenerateEndpoints = !string.IsNullOrEmpty(settings.EndpointConfigurationName),
				ServiceUri = settings.ServerUri,
				ReportId = settings.ReportId,
				ReportName = settings.ReportTypeName,
				UserName = remoteSourceCachedUserName
			};
		}
	}
}
