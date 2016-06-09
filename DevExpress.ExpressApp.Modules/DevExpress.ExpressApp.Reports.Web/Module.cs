#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Web;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
namespace DevExpress.ExpressApp.Reports.Web {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Contains Controllers that allow end-user interaction with reports in ASP.NET Web XAF applications. Allows a user to filter, preview and download reports in different formats. This module is based on UI independent elements that are implemented in the ReportsModule.")]
	[ToolboxBitmap(typeof(ReportsAspNetModule), "Resources.Toolbox_Module_Reports_Web.ico")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	[Designer("DevExpress.ExpressApp.Reports.Web.Design.ReportsWebDesigner, DevExpress.ExpressApp.Reports.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[DesignerCategory("Component")]
	public sealed class ReportsAspNetModule : ModuleBase {
		private bool showFormatSpecificPreviewActions = false;
		private ReportOutputType defaultPreviewFormat = ReportOutputType.ReportViewer;
		private void application_SetupComplete(object sender, EventArgs e) {
			XafApplication application = (XafApplication)sender;
			application.SetupComplete -= application_SetupComplete;
			ReportsModule reportsModule = Application.Modules.FindModule<ReportsModule>();
			if(reportsModule != null) {
				IModelDetailView modelView = application.Model.Views["ReportViewer_DetailView"] as IModelDetailView;
				if((modelView != null) && (modelView.ModelClass == null)) {
					modelView.ModelClass = application.Model.BOModel[reportsModule.ReportDataType.FullName];
				}
			}
		}
		static ReportsAspNetModule() {
			XafHttpHandler.RegisterHandler(new ReportExportHttpHandler());
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList requiredModules = base.GetRequiredModuleTypesCore();
			requiredModules.Add(typeof(ExpressApp.Reports.ReportsModule));
			return requiredModules;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return null;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(ReportDialogController),
				typeof(ReportsController),
				typeof(ReportViewerDialogController),
				typeof(WebReportServiceController),
				typeof(InitializeReportDesignExtensionController)
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.SetupComplete += new EventHandler<EventArgs>(application_SetupComplete);
			PrintSelectionBaseController.ShowInReportActionEnableModeDefault = PrintSelectionBaseController.ActionEnabledMode.ViewMode;
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
#if !SL
	[DevExpressExpressAppReportsWebLocalizedDescription("ReportsAspNetModuleShowFormatSpecificPreviewActions")]
#endif
		[DefaultValue(false)]
		public bool ShowFormatSpecificPreviewActions {
			get { return showFormatSpecificPreviewActions; }
			set { showFormatSpecificPreviewActions = value; }
		}
#if !SL
	[DevExpressExpressAppReportsWebLocalizedDescription("ReportsAspNetModuleDefaultPreviewFormat")]
#endif
		[DefaultValue(ReportOutputType.ReportViewer)]
		public ReportOutputType DefaultPreviewFormat {
			get { return defaultPreviewFormat; }
			set { defaultPreviewFormat = value; }
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(ASPxReportControlLocalizer));
			return result;
		}
	}
}
