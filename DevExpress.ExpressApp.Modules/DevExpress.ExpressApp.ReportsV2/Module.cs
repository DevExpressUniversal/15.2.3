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
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Extensions;
namespace DevExpress.ExpressApp.ReportsV2 {
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Contains base classes and services that are used by the ReportsWindowsFormsModuleV2 and ReportsAspNetModuleV2.")]
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(ReportsModuleV2), "Resources.Toolbox_Module_ReportsV2.ico")]
	[DesignerCategory("Component")]
	public sealed class ReportsModuleV2 : ModuleBase, IModelNodesGeneratorUpdater {
		public const string XtraReportContextName = "XtraReport";
		public const string DefaultReportDataNavigationItemCaption = "Reports";
		public const string DefaultReportDataNavigationItemId = "ReportsV2";
		public const string DefaultReportDataCaption = "Report";
		public const string DefaultReportDataImageName = "BO_Report";
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string IsInplaceReportMemberName = "IsInplaceReport";  
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string PredefinedReportTypeMemberName = "PredefinedReportType";
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string ParametersObjectTypeNameMemberName = "ParametersObjectTypeName";
		private bool showAdditionalNavigation = false;
		private Type reportDataType;
		private bool isReportDataTypeInitialized;
		private bool enableInplaceReports = DefaultEnableInplaceReports;
		private ReportDataSourceHelper reportsDataSourceHelper;
		public ReportsModuleV2() { }
		public static ReportsModuleV2 FindReportsModule(ModuleList modules) {
			ReportsModuleV2 reportsModule = null;
			foreach(ModuleBase module in modules) {
				if(module is ReportsModuleV2) {
					reportsModule = (ReportsModuleV2)module;
				}
			}
			return reportsModule;
		}
		public static string GetEmptyDisplayNameErrorMessage() {
			return CaptionHelper.GetLocalizedText("Exceptions", "ReportsDisplayNameIsEmptyErrorMessage");
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string FindPredefinedReportTypeMemberName(Type reportDataType) {
			object[] result = reportDataType.GetCustomAttributes(typeof(PredefinedReportTypeMemberNameAttribute), true);
			if(result.Length > 0) {
				string memberName = ((PredefinedReportTypeMemberNameAttribute)result[0]).PredefinedReportTypeMemberName;
				if(!string.IsNullOrEmpty(memberName)) {
					return memberName;
				}
			}
			return PredefinedReportTypeMemberName;
		}
		private void application_LoggedOn(object sender, LogonEventArgs e) {
			Application.LoggedOn -= new EventHandler<LogonEventArgs>(application_LoggedOn);
			ApplicationReportObjectSpaceProvider.ContextApplication = Application;
			ReportDataProvider.ReportsStorage.ReportDataType = ReportDataType;
			FindPreviousReportStorage(Application.Modules);
			VisibleInReportsIndicator.SetInstance(new VisibleInReportsModelIndicator(Application.Model));
		}
		private void FindPreviousReportStorage(ModuleList modules) {
			foreach(ModuleBase module in modules) {
				IReportStorageExtensionContainer reportStorageContainer = module as IReportStorageExtensionContainer;
				if(reportStorageContainer == null) continue;
				if(reportStorageContainer.ReportStorage != null) {
					((IPreviousReportStorageExtensionContainer)ReportDataProvider.ReportsStorage).PreviousReportStorageExtension = reportStorageContainer.ReportStorage;
				}
				else {
					reportStorageContainer.ProhibitExtensionRegistration();
					reportStorageContainer.ReportStorageCreated += reportStorageContainer_ReportStorageCreated;
				}
			}
		}
		private void reportStorageContainer_ReportStorageCreated(object sender, ReportStorageCreatedEventArgs e) {
			IReportStorageExtensionContainer reportStorageContainer = sender as IReportStorageExtensionContainer;
			reportStorageContainer.ReportStorageCreated -= reportStorageContainer_ReportStorageCreated;
			((IPreviousReportStorageExtensionContainer)ReportDataProvider.ReportsStorage).PreviousReportStorageExtension = e.ReportStorage;
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
			return new Type[] { ReportDataType, typeof(NewReportWizardParameters) };
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(CustomizeNavigationItemsController),
				typeof(PrintSelectionBaseController),
				typeof(PreviewReportDialogController),
				typeof(CopyPredefinedReportsController),
				typeof(NewReportWizardController),
				typeof(DeleteReportController)
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
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
			}
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			if(reportsDataSourceHelper == null) {
				reportsDataSourceHelper = new ReportDataSourceHelper(application);
			}
			application.LoggedOn += new EventHandler<LogonEventArgs>(application_LoggedOn);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			reportsDataSourceHelper = null;
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		[TypeConverter(typeof(BusinessClassTypeConverter<IReportDataV2Writable>))]
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
					Guard.TypeArgumentIs(typeof(IReportDataV2Writable), value, "value");
				}
				isReportDataTypeInitialized = true;
				reportDataType = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReportDataSourceHelper ReportsDataSourceHelper {
			get {
				return reportsDataSourceHelper;
			}
			set {
				reportsDataSourceHelper = value;
			}
		}
		private Type FindDefaultReportDataType() {
			Type result;
			String assemblyName = "DevExpress.Persistent.BaseImpl" + XafAssemblyInfo.VersionSuffix;
			String typeFullName = "DevExpress.Persistent.BaseImpl.ReportDataV2";
			if(!AssemblyHelper.TryGetType(assemblyName, typeFullName, out result) && !DesignTimeTools.IsDesignMode) {
				throw new Exception(string.Format("Cannot find the '{0}' type or '{1}' assembly.", typeFullName, assemblyName));
			}
			return result;
		}
		public static bool ActivateReportController(Controller controller) {
			ReportsModuleV2 reportsModule = FindReportsModule(controller.Application.Modules);
			if(reportsModule == null) {
				controller.Active.SetItemValue("Application.Modules contains ReportsModule", false);
				return false;
			}
			if(reportsModule.ReportDataType == null) {
				controller.Active.SetItemValue("ReportsModule.ReportDataType is assigned", false);
				return false;
			}
			return true;
		}
		public bool EnableInplaceReports {
			get { return enableInplaceReports; }
			set { enableInplaceReports = value; }
		}
		[DefaultValue(false)]
		public bool ShowAdditionalNavigation {
			get { return showAdditionalNavigation; }
			set { showAdditionalNavigation = value; }
		}
		[DefaultValue(ReportStoreModes.DOM)]
		public ReportStoreModes ReportStoreMode {
			get { return ReportDataProvider.ReportsStorage.ReportStoreMode; }
			set { ReportDataProvider.ReportsStorage.ReportStoreMode = value; }
		}
		public static bool DefaultEnableInplaceReports = true;
		private InplaceReportsCacheHelper inplaceReportCacheHelper;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public InplaceReportsCacheHelper InplaceReportsCacheHelper {
			get {
				if(inplaceReportCacheHelper == null) {
					inplaceReportCacheHelper = new InplaceReportsCacheHelper();
				}
				return inplaceReportCacheHelper;
			}
			set {
				inplaceReportCacheHelper = value;
			}
		}
		#region IModelNodesGeneratorUpdater Members
		void IModelNodesGeneratorUpdater.UpdateNode(ModelNode node) {
			if(ReportDataType == null) return;
			AddStoredReportsNavigationItem(node);
		}
		private void AddStoredReportsNavigationItem(ModelNode node) {
			IModelApplication modelApplication = node.Application;
			IModelClass reportDataModelClass = modelApplication.BOModel.GetClass(ReportDataType);
			if(reportDataModelClass != null) {
				NavigationItemAttribute attr = reportDataModelClass.TypeInfo.FindAttribute<NavigationItemAttribute>();
				if(attr == null || (attr.IsNavigationItem == true && string.IsNullOrEmpty(attr.GroupName))) {
					IModelListView defaultListView = reportDataModelClass.DefaultListView;
					string defaultListViewId = defaultListView != null ? defaultListView.Id : string.Empty;
					ShowNavigationItemController.GenerateNavigationItem(modelApplication, new ViewShortcut(defaultListViewId, null), DefaultReportDataNavigationItemCaption, DefaultReportDataNavigationItemId, DefaultReportDataNavigationItemCaption);
				}
				IModelNavigationItemsForReports navigationItems = (IModelNavigationItemsForReports)node;
				navigationItems.GenerateRelatedReportsGroup = ShowAdditionalNavigation;
				navigationItems.RelatedReportsGroupCaption = DefaultReportDataNavigationItemCaption;
			}
		}
		Type IModelNodesGeneratorUpdater.GeneratorType { get { return typeof(NavigationItemNodeGenerator); } }
		#endregion
	}
	public interface IModelNavigationItemsForReports {
		[ Category("Behavior")]
		bool GenerateRelatedReportsGroup { get; set; }
		[ Localizable(true)]
		string RelatedReportsGroupCaption { get; set; }
	}
	public enum ReportStoreModes {
		DOM,
		XML
	}
}
