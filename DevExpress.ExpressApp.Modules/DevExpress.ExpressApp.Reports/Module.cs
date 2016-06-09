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
using System.Drawing;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Extensions;
namespace DevExpress.ExpressApp.Reports {
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Contains base classes and services that are used by the ReportsWindowsFormsModule and ReportsAspNetModule.")]
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(ReportsModule), "Resources.Toolbox_Module_Reports.ico")]
	public sealed class ReportsModule : ModuleBase, IModelNodesGeneratorUpdater, IReportStorageExtensionContainer {
		public const string DefaultReportDataNavigationItemCaption = "Reports";
		public const string DefaultReportDataCaption = "Report";
		public const string DefaultReportDataImageName = "BO_Report";
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string EmptyReportNameErrorMessage = @"The ""ReportName"" field must have a value.";   
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string IsInplaceReportMemberName = "IsInplaceReport";  
		private bool showAdditionalNavigation = false;
		private Type reportDataType;
		private bool isReportDataTypeInitialized;
		private bool enableInplaceReports = DefaultEnableInplaceReports;
		private static XafReportStorageExtension reportStorageExtension;
		private static bool extensionRegistraionIsAllowed = true;
		private void application_LoggedOn(object sender, LogonEventArgs e) {
			Application.LoggedOn -= new EventHandler<LogonEventArgs>(application_LoggedOn);
			XafReportStorageExtension = new XafReportStorageExtension(Application);
			OnReportStorageCreated();
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelNavigationItemsForReports)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			if(ReportDataType == null) {
				return Type.EmptyTypes;
			}
			return new Type[] { ReportDataType };
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(CustomizeNavigationItemsController),
				typeof(InplaceReportCacheController),
				typeof(PrintSelectionBaseController),
				typeof(ReportDataSelectionDialogController),
				typeof(InitializeReportsController)
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		public ReportsModule() {
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelClass, IModelClassReportsVisibility>();
			extenders.Add<IModelRootNavigationItems, IModelNavigationItemsForReports>();
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(this);
		}
		public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
			base.CustomizeTypesInfo(typesInfo);
			if(ReportDataType != null) {
				ITypeInfo reportDataTypeInfo = typesInfo.FindTypeInfo(ReportDataType);
				if(reportDataTypeInfo.FindAttribute<DisplayNameAttribute>() == null) {
					reportDataTypeInfo.AddAttribute(new DisplayNameAttribute(DefaultReportDataCaption));
				}
				if(reportDataTypeInfo.FindAttribute<ImageNameAttribute>() == null) {
					reportDataTypeInfo.AddAttribute(new ImageNameAttribute(DefaultReportDataImageName));
				}
				if((reportDataTypeInfo.KeyMember != null) && (reportDataTypeInfo.KeyMember.FindAttribute<BrowsableAttribute>() == null)) {
					reportDataTypeInfo.KeyMember.AddAttribute(new BrowsableAttribute(false));
				}
			}
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.LoggedOn += new EventHandler<LogonEventArgs>(application_LoggedOn);
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
#if !SL
	[DevExpressExpressAppReportsLocalizedDescription("ReportsModuleReportDataType")]
#endif
		[TypeConverter(typeof(BusinessClassTypeConverter<IReportData>))]
		public Type ReportDataType {
			get {
				if(!isReportDataTypeInitialized) {
					isReportDataTypeInitialized = true;
					reportDataType = FindDefaultReportDataType();
				}
				return reportDataType; 
			}
			set {
				if(value != null) {
					Guard.TypeArgumentIs(typeof(IReportData), value, "value");
				}
				isReportDataTypeInitialized = true;
				reportDataType = value;
			}
		}
		private Type FindDefaultReportDataType() {
			Type result;
			String assemblyName = "DevExpress.Persistent.BaseImpl" + XafAssemblyInfo.VersionSuffix;
			String typeFullName = "DevExpress.Persistent.BaseImpl.ReportData";
			if(!AssemblyHelper.TryGetType(assemblyName, typeFullName, out result) && !DesignTimeTools.IsDesignMode) {
				throw new Exception(string.Format("Cannot find the '{0}' type or '{1}' assembly.", typeFullName, assemblyName));
			}
			return result;
		}
		public static bool GetIsVisibleInReports(ITypeInfo classInfo, out bool isVisible) {
			return VisibleInReportsIndicator.Instance.GetIsVisibleInReports(classInfo, out isVisible);
		}
		public static ReportsModule FindReportsModule(ModuleList modules) {
			ReportsModule reportsModule = null;
			foreach(ModuleBase module in modules) {
				if(module is ReportsModule) {
					if(reportsModule != null) {
						throw new InvalidOperationException("invalid operation: " + reportsModule.GetType().FullName + ", " + module.GetType().FullName);
					}
					reportsModule = (ReportsModule)module;
				}
			}
			return reportsModule;
		}
		public static Type GetCurrentReportDataType(ModuleList modules) {
			ReportsModule reportsModule = FindReportsModule(modules);
			return reportsModule == null ? null : reportsModule.ReportDataType;
		}
		public static bool ActivateReportController(ViewController controller) {
			Type reportDataType;
			return ReportsModule.ActivateReportController(controller, out reportDataType);
		}
		public static bool ActivateReportController(ViewController controller, out Type reportDataType) {
			reportDataType = null;
			ReportsModule reportsModule = FindReportsModule(controller.Application.Modules);
			if(reportsModule == null) {
				controller.Active.SetItemValue("Application.Modules contains ReportsModule", false);
				return false;
			}
			reportDataType = reportsModule.ReportDataType;
			if(reportDataType == null) {
				controller.Active.SetItemValue("ReportsModule.ReportDataType is assigned", false);
				return false;
			}
			return true;
		}
#if !SL
	[DevExpressExpressAppReportsLocalizedDescription("ReportsModuleEnableInplaceReports")]
#endif
		public bool EnableInplaceReports {
			get { return enableInplaceReports; }
			set { enableInplaceReports = value; }
		}
#if !SL
	[DevExpressExpressAppReportsLocalizedDescription("ReportsModuleShowAdditionalNavigation")]
#endif
			[DefaultValue(false)]
		public bool ShowAdditionalNavigation {
			get { return showAdditionalNavigation; }
			set { showAdditionalNavigation = value; }
		}
		public static bool DefaultEnableInplaceReports = true;
		public static XafReportStorageExtension XafReportStorageExtension {
			get { return reportStorageExtension; }
			set {
				reportStorageExtension = value;
				if(extensionRegistraionIsAllowed) {
					ReportStorageExtension.RegisterExtensionGlobal(reportStorageExtension);
				}
			}
		}
		#region IModelNodesGeneratorUpdater Members
		void IModelNodesGeneratorUpdater.UpdateNode(ModelNode node) {
			if(ReportDataType == null) return;
			IModelApplication modelApplication = node.Application;
			IModelClass modelClass = modelApplication.BOModel.GetClass(ReportDataType);
			if(modelClass != null) {
				NavigationItemAttribute attr = modelClass.TypeInfo.FindAttribute<NavigationItemAttribute>();
				if(attr == null || (attr.IsNavigationItem == true && string.IsNullOrEmpty(attr.GroupName))) {
					IModelListView defaultListView = modelClass.DefaultListView;
					string defaultListViewId = defaultListView != null ? defaultListView.Id : string.Empty;
					ShowNavigationItemController.GenerateNavigationItem(modelApplication, new ViewShortcut(defaultListViewId, null), DefaultReportDataNavigationItemCaption, DefaultReportDataNavigationItemCaption, DefaultReportDataNavigationItemCaption);
				}
				IModelNavigationItemsForReports navigationItems = (IModelNavigationItemsForReports)node;
				navigationItems.GenerateRelatedReportsGroup = ShowAdditionalNavigation;
				navigationItems.RelatedReportsGroupCaption = DefaultReportDataNavigationItemCaption;
				IModelDetailView modelView = modelApplication.Views.GetNode("ReportViewer_DetailView") as IModelDetailView;
				if(modelView == null) {
					modelView = modelApplication.Views.AddNode<IModelDetailView>("ReportViewer_DetailView");
					modelView.ModelClass = modelClass;
					IModelLayoutGroup modelLayoutGroup = (IModelLayoutGroup)modelView.Layout.GetNode("Main");
					modelLayoutGroup.ClearNodes();
					IModelLayoutViewItem modelLayoutItem = modelLayoutGroup.AddNode<IModelLayoutViewItem>("ReportViewer");
					modelLayoutItem.Index = 0;
					modelLayoutItem.ViewItem = null;
				}
			}
		}
		Type IModelNodesGeneratorUpdater.GeneratorType { get { return typeof(NavigationItemNodeGenerator); } }
		#endregion
		#region IReportStorageExtensionContainer Members
		ReportStorageExtension IReportStorageExtensionContainer.ReportStorage {
			get { return XafReportStorageExtension; }
		}
		void IReportStorageExtensionContainer.ProhibitExtensionRegistration() {
			extensionRegistraionIsAllowed = false;
		}
		private void OnReportStorageCreated() {
			if(ReportStorageCreated != null) {
				ReportStorageCreated(this, new ReportStorageCreatedEventArgs(XafReportStorageExtension));
			}
		}
		public event EventHandler<ReportStorageCreatedEventArgs> ReportStorageCreated;
		#endregion
	}
	public interface IModelNavigationItemsForReports {
		[
#if !SL
	DevExpressExpressAppReportsLocalizedDescription("IModelNavigationItemsForReportsGenerateRelatedReportsGroup"),
#endif
 Category("Behavior")]
		bool GenerateRelatedReportsGroup { get; set; }
		[
#if !SL
	DevExpressExpressAppReportsLocalizedDescription("IModelNavigationItemsForReportsRelatedReportsGroupCaption"),
#endif
 Localizable(true)]
		string RelatedReportsGroupCaption { get; set; }
	}
}
