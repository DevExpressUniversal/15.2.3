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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Web.SessionState;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Web;
using DevExpress.XtraReports.Web.Extensions;
namespace DevExpress.ExpressApp.ReportsV2.Web {
	[Description("Contains Controllers that allow end-user interaction with reports in ASP.NET Web XAF applications. Allows a user to filter, preview and download reports in different formats. This module is based on UI independent elements that are implemented in the ReportsModuleV2.")]
	[Designer("DevExpress.ExpressApp.Reports.Web.Design.ReportsWebDesigner, DevExpress.ExpressApp.Reports.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[DesignerCategory("Component")]
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[ToolboxBitmap(typeof(ReportsAspNetModuleV2), "Resources.Toolbox_Module_ReportsV2_Web.ico")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	public sealed class ReportsAspNetModuleV2 : ModuleBase, IModelNodesGeneratorUpdater {
		public const string ReportViewDetailViewWebName = "ReportViewer_DetailView_V2";
		public const string ReportDesignDetailViewWebName = "ReportDesigner_DetailView";
		private bool showFormatSpecificExportActions = false;
		private DesignAndPreviewDisplayModes designAndPreviewDisplayMode = DesignAndPreviewDisplayModes.Popup;
		private ClientLibrariesLocations clientLibrariesLocation = ClientLibrariesLocations.Embedded;
		private ReportViewerTypes reportViewerType = ReportViewerTypes.ASP;
		private ReportOutputType defaultPreviewFormat = ReportOutputType.ReportViewer;
		static ReportsAspNetModuleV2() {
			XafHttpHandler.RegisterHandler(new StreamExportHttpHandler());
		}
		[DefaultValue(false)]
		public bool ShowFormatSpecificExportActions {
			get { return showFormatSpecificExportActions; }
			set { showFormatSpecificExportActions = value; }
		}
		[DefaultValue(ReportOutputType.ReportViewer)]
		public ReportOutputType DefaultPreviewFormat {
			get { return defaultPreviewFormat; }
			set { defaultPreviewFormat = value; }
		}
		[DefaultValue(DesignAndPreviewDisplayModes.Popup)]
		public DesignAndPreviewDisplayModes DesignAndPreviewDisplayMode {
			get { return designAndPreviewDisplayMode; }
			set { designAndPreviewDisplayMode = value; }
		}
		[DefaultValue(ClientLibrariesLocations.Embedded)]
		public ClientLibrariesLocations ClientLibrariesLocation {
			get { return clientLibrariesLocation; }
			set { clientLibrariesLocation = value; }
		}
		[DefaultValue(ReportViewerTypes.ASP)]
		public ReportViewerTypes ReportViewerType {
			get { return reportViewerType; }
			set { reportViewerType = value; }
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.SetupComplete += new EventHandler<EventArgs>(application_SetupComplete);
			PrintSelectionBaseController.ShowInReportActionEnableModeDefault = PrintSelectionBaseController.ActionEnabledMode.ViewMode;
			application.LoggedOn += new EventHandler<LogonEventArgs>(application_LoggedOn);
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(this);
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList requiredModules = base.GetRequiredModuleTypesCore();
			requiredModules.Add(typeof(ExpressApp.ReportsV2.ReportsModuleV2));
			return requiredModules;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(IModelReportDesignerViewItem));
			return result;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(WebReportsController),
				typeof(WebReportServiceController),
				typeof(WebEditReportController),
				typeof(WebReportDesignerPopupController),
				typeof(WebReportViewerPopupController),
				typeof(WebReportWizardDialogController)
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelReportDesignerViewItem), typeof(ReportDesignerDetailItem), true)));
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(ASPxReportControlLocalizer));
			return result;
		}
		private void application_SetupComplete(object sender, EventArgs e) {
			XafApplication application = (XafApplication)sender;
			application.SetupComplete -= application_SetupComplete;
			RegisterDesignerTools();
			SetReportViewerModelView(application);
		}
		private void SetReportViewerModelView(XafApplication application) {
			ReportsModuleV2 reportsModuleV2 = Application.Modules.FindModule<ReportsModuleV2>();
			if(reportsModuleV2 != null) {
				IModelDetailView modelView = application.Model.Views[ReportViewDetailViewWebName] as IModelDetailView;
				if((modelView != null) && (modelView.ModelClass == null)) {
					modelView.ModelClass = application.Model.BOModel[reportsModuleV2.ReportDataType.FullName];
				}
				modelView = application.Model.Views[ReportDesignDetailViewWebName] as IModelDetailView;
				if((modelView != null) && (modelView.ModelClass == null)) {
					modelView.ModelClass = application.Model.BOModel[reportsModuleV2.ReportDataType.FullName];
				}
			}
		}
		private void application_LoggedOn(object sender, LogonEventArgs e) {
			Application.LoggedOn -= new EventHandler<LogonEventArgs>(application_LoggedOn);
			ReportDesignExtensionManager.CustomizeReportExtension += ReportDesignExtensionManager_CustomizeReportExtension;
			ReportDesignExtensionManager.Initialize(Application);
		}
		private void ReportDesignExtensionManager_CustomizeReportExtension(object sender, CustomizeReportExtensionEventArgs e) {
			ReportDesignExtensionManager.CustomizeReportExtension -= ReportDesignExtensionManager_CustomizeReportExtension;
			e.XafReportDataTypeProvider.CustomAddParameterTypes += XafReportDataTypeProvider_CustomAddParameterTypes;
		}
		private void XafReportDataTypeProvider_CustomAddParameterTypes(object sender, AddCustomParameterTypesEventArgs e) {
			((XtraReportDataTypeProvider)sender).CustomAddParameterTypes -= XafReportDataTypeProvider_CustomAddParameterTypes;
			ReportDesignerDetailItem.ParametersTypes = e.Dictionary;
		}
		private void RegisterDesignerTools() {
			DevExpress.XtraReports.Web.ReportDesigner.Native.ReportDesignerBootstrapper.SessionState = SessionStateBehavior.Required;
			ReportStorageWebService.RegisterTool(XafReportStorageWebTool.Instance);
			DevExpress.XtraReports.Web.ASPxReportDesigner.StaticInitialize();
			var container = (DevExpress.Utils.IoC.IntegrityContainer)DevExpress.XtraReports.Web.ReportDesigner.DefaultReportDesignerContainer.Current;
			container.RegisterType<DevExpress.XtraReports.Web.ReportDesigner.Native.Services.IPreviewReportLayoutService, PreviewInitializer>();
		}
		#region IModelNodesGeneratorUpdater Members
		void IModelNodesGeneratorUpdater.UpdateNode(ModelNode node) {
			AddReportViewerModelView(node);
			AddReportDesignerModelView(node);
		}
		private void AddReportViewerModelView(ModelNode node) {
			IModelDetailView modelView = FindReportModelView(node, ReportViewDetailViewWebName);
			if(modelView == null) {
				AddReportModelView(node.Application, ReportViewDetailViewWebName, "ReportViewer");
			}
		}
		private void AddReportDesignerModelView(ModelNode node) {
			IModelDetailView modelView = FindReportModelView(node, ReportDesignDetailViewWebName);
			if(modelView == null) {
				modelView = AddReportModelView(node.Application, ReportDesignDetailViewWebName, "ReportDesigner");
				AddReportDesignerViewItem(modelView);
			}
		}
		private void AddReportDesignerViewItem(IModelDetailView modelView) {
			if(modelView != null) {
				modelView.Items.AddNode<IModelReportDesignerViewItem>("ReportDesigner");
			}
		}
		private IModelDetailView FindReportModelView(ModelNode node, string modelId) {
			IModelApplication modelApplication = node.Application;
			return modelApplication.Views.GetNode(modelId) as IModelDetailView;
		}
		private IModelDetailView AddReportModelView(IModelApplication modelApplication, string modelId, string itemId) {
			IModelClass reportDataModelClass = GetReportDataModelClass(modelApplication);
			IModelDetailView modelView = null;
			if(reportDataModelClass != null) {
				modelView = modelApplication.Views.AddNode<IModelDetailView>(modelId);
				modelView.ModelClass = reportDataModelClass;
				IModelLayoutGroup modelLayoutGroup = (IModelLayoutGroup)modelView.Layout.GetNode("Main");
				if(modelLayoutGroup != null) {
					if(modelLayoutGroup is DevExpress.ExpressApp.Web.SystemModule.IModelViewLayoutElementWeb) {
						((DevExpress.ExpressApp.Web.SystemModule.IModelViewLayoutElementWeb)modelLayoutGroup).CustomCSSClassName = "ReportViewMainGroup";
					}
					modelLayoutGroup.ClearNodes();
					IModelLayoutViewItem modelLayoutItem = modelLayoutGroup.AddNode<IModelLayoutViewItem>(itemId);
					modelLayoutItem.Index = 0;
					modelLayoutItem.ViewItem = null;
				}
			}
			return modelView;
		}
		private IModelClass GetReportDataModelClass(IModelApplication modelApplication) {
			IModelClass reportDataModelClass = null;
			if(Application != null) {
				ReportsModuleV2 reportsModuleV2 = Application.Modules.FindModule<ReportsModuleV2>();
				if(reportsModuleV2 != null) {
					reportDataModelClass = modelApplication.BOModel.GetClass(reportsModuleV2.ReportDataType);
				}
			}
			return reportDataModelClass;
		}
		Type IModelNodesGeneratorUpdater.GeneratorType { get { return typeof(ModelViewsNodesGenerator); } }
		#endregion
	}
	public enum DesignAndPreviewDisplayModes {
		Popup,
		DetailView
	}
	public enum ClientLibrariesLocations {
		Embedded,
		Custom
	}
	public enum ReportViewerTypes {
		ASP,
		HTML5
	}
}
