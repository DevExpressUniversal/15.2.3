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
using DevExpress.Utils;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Kpi {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[ToolboxBitmap(typeof(KpiModule), "Resources.Toolbox_Module_KPI.ico")]
	[Description("Provides Key Performance Indicators functionality in XAF applications.")]
	public sealed partial class KpiModule : ModuleBase {
		public const String KpiInstance_Chart_DetailViewID = "KpiInstance_Chart_DetailView";
		public static UsedExportedTypes UsedExportedTypes = UsedExportedTypes.XPObjects;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool EnableSelfRefresh = true;
		public const string KpiNavigationGroupName = "KPI";
		public static IEnumerable<Type> GetKpiBusinessClasses() {
			return new Type[]{
				typeof(BaseKpiObject),
				typeof(KpiDefinition),
				typeof(KpiInstance),
				typeof(KpiHistoryItem),
				typeof(KpiScorecard)
			};
		}
		public static IEnumerable<Type> GetKpiPersistentInterfaces() {
			return new Type[]{
				typeof(IDCKpiDefinition),
				typeof(IDCKpiInstance),
				typeof(IDCKpiHistoryItem),
				typeof(IDCKpiScorecard)
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList list = base.GetRequiredModuleTypesCore();
			list.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
			return list;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelKpiDashboardViewItem),
			typeof(ModelKpiDashboardViewItemLogic),
			typeof(DCKpiInstanceLogic),
			typeof(DCKpiHistoryItemLogic),
			typeof(KpiDefinitionLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(KpiDashboardOrganizationItem));
			if(UsedExportedTypes == UsedExportedTypes.XPObjects) {
				result.AddRange(GetKpiBusinessClasses());
			}
			else if(UsedExportedTypes == UsedExportedTypes.DCInterfaces) {
				result.AddRange(GetKpiPersistentInterfaces());
			}
			return result;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
#if DebugTest
				typeof(DevExpress.ExpressApp.Kpi.ShowKpiExChartController),
#endif
				typeof(DevExpress.ExpressApp.Kpi.AutoRefreshKpiController),
				typeof(DevExpress.ExpressApp.Kpi.DrilldownController),
				typeof(DevExpress.ExpressApp.Kpi.ForceRefreshKpiController),
				typeof(DevExpress.ExpressApp.Kpi.KpiDashboardOrganizationItemController),
				typeof(DevExpress.ExpressApp.Kpi.KpiDefinitionPreviewController),
				typeof(DevExpress.ExpressApp.Kpi.KpiInstanceDeactivateNewDeleteViewController),
				typeof(DevExpress.ExpressApp.Kpi.RefreshKpiController),
				typeof(DevExpress.ExpressApp.Kpi.ShowKpiAsChartController)
			};
		}
		public KpiModule() {
			InitializeComponent();
		}
		public override void Setup(ApplicationModulesManager moduleManager) {
			base.Setup(moduleManager);
			DevExpress.ExpressApp.Validation.ValidationRulesRegistrator.RegisterRule(moduleManager, typeof(IsValidKpiCriteriaCodeRule), typeof(DevExpress.Persistent.Validation.IRuleBaseProperties));
			foreach(Type exportedType in GetExportedTypes()) {
				if(typeof(IKpiDefinition).IsAssignableFrom(exportedType)) {
					ModelNodesGeneratorSettings.SetIdPrefix(exportedType, "KpiDefinition");
				}
				else if(typeof(IKpiInstance).IsAssignableFrom(exportedType)) {
					ModelNodesGeneratorSettings.SetIdPrefix(exportedType, "KpiInstance");
				}
				else if(typeof(IKpiHistoryItem).IsAssignableFrom(exportedType)) {
					ModelNodesGeneratorSettings.SetIdPrefix(exportedType, "KpiHistoryItem");
				}
				else if(exportedType.Name.Contains("KpiScorecard")) {
					ModelNodesGeneratorSettings.SetIdPrefix(exportedType, "KpiScorecard");
				}
			}
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			if(UsedExportedTypes == UsedExportedTypes.DCInterfaces) {
				XafTypesInfo.Instance.RegisterEntity("DCKpiDefinition", typeof(IDCKpiDefinition));
				XafTypesInfo.Instance.RegisterEntity("DCKpiInstance", typeof(IDCKpiInstance));
				XafTypesInfo.Instance.RegisterEntity("DCKpiHistoryItem", typeof(IDCKpiHistoryItem));
				XafTypesInfo.Instance.RegisterEntity("DCKpiScorecard", typeof(IDCKpiScorecard));
			}
		}
		public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
			base.CustomizeTypesInfo(typesInfo);
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(typeof(IModelKpiDashboardViewItem));
			BaseInfo criteriaMemberInfo = typeInfo.FindMember("Criteria") as BaseInfo;
			BaseInfo viewMemberInfo = typeInfo.FindMember("View") as BaseInfo;
			criteriaMemberInfo.AddAttribute(new ModelBrowsableAttribute(typeof(KpiDashboardItemPropertiesVisibilityCalculator)));
			viewMemberInfo.AddAttribute(new ModelBrowsableAttribute(typeof(KpiDashboardItemPropertiesVisibilityCalculator)));
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new DateRangeLocalizationModelNodesGeneratorUpdater());
			updaters.Add(new KpiViewsModelUpdater(this));
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void UpdateModel(IModelViews viewsModel) {
			ITypesInfo typesInfo = null;
			if(Application != null) {
				typesInfo = Application.TypesInfo;
			}
			else {
				typesInfo = XafTypesInfo.Instance;
			}
			foreach(ITypeInfo exportedTypeInfo in typesInfo.PersistentTypes) {
				Type exportedType = exportedTypeInfo.Type;
				if(typeof(IKpiInstance).IsAssignableFrom(exportedType)) {
					IModelDetailView detailViewModel = (IModelDetailView)viewsModel.GetNode(KpiModule.KpiInstance_Chart_DetailViewID);
					if(detailViewModel == null) {
						detailViewModel = viewsModel.AddNode<IModelDetailView>(KpiModule.KpiInstance_Chart_DetailViewID);
						detailViewModel.ModelClass = viewsModel.Application.BOModel.GetClass(exportedType);
					}
				}
			}
		}
	}
	public class DateRangeLocalizationModelNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelLocalizationNodesGenerator> {
		private const string DateRangeLocalizationGroupName = "DateRanges";
		public override void UpdateNode(ModelNode node) {
			IModelLocalization modelLocalization = (IModelLocalization)node;
			IModelLocalizationGroup dateRangesLocalizationGroup = modelLocalization[DateRangeLocalizationGroupName];
			if(dateRangesLocalizationGroup == null) {
				dateRangesLocalizationGroup = modelLocalization.AddNode<IModelLocalizationGroup>(DateRangeLocalizationGroupName);
			}
			foreach(IDateRange range in DateRangeRepository.GetRegisteredRanges()) {
				IModelLocalizationItem item = (IModelLocalizationItem)dateRangesLocalizationGroup[range.Name];
				if(item == null) {
					item = dateRangesLocalizationGroup.AddNode<IModelLocalizationItem>(range.Name);
					item.Value = range.Caption;
				}
			}
		}
	}
	public class KpiViewsModelUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
		private KpiModule kpiModule;
		public KpiViewsModelUpdater(KpiModule kpiModule) {
			this.kpiModule = kpiModule;
		}
		public override void UpdateNode(ModelNode node) {
			kpiModule.UpdateModel((IModelViews)node);
		}
	}
}
