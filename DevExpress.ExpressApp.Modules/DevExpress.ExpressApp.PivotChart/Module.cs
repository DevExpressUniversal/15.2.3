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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.PivotChart {
	[Description("Provides end-users with an extremely intuitive experience in slicing and presenting data.")]
	[ToolboxBitmap(typeof(PivotChartModuleBase), "Resources.Toolbox_Module_PivotChart.ico")]
	public sealed partial class PivotChartModuleBase : ModuleBase, IModelNodesGeneratorUpdater
	{
		public const string DefaultAnalysisDataNavigationItemCaption = "Analysis";
		private bool showAdditionalNavigation;
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelPivotChartNavigation),
			typeof(IModelMemberPivotChartVisibility),
			typeof(ModelMemberPivotChartVisibilityLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(DevExpress.ExpressApp.PivotChart.AnalysisDataBindController),
				typeof(DevExpress.ExpressApp.PivotChart.AnalysisReadOnlyController),
				typeof(DevExpress.ExpressApp.PivotChart.CustomizeNavigationItemsController),
				typeof(DevExpress.ExpressApp.PivotChart.InitializePivotChartConroller)
			};
		}
		public PivotChartModuleBase() {
			InitializeComponent();
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(this);
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.AnalysisInfoPropertyEditor, typeof(IAnalysisInfo), true)));
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelClass, IModelClassReportsVisibility>();
			extenders.Add<IModelMember, IModelMemberPivotChartVisibility>();
			extenders.Add<IModelRootNavigationItems, IModelPivotChartNavigation>();
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			return new Type[] {
				typeof(PivotGridLocalizer),
				typeof(ChartLocalizer),
				typeof(AnalysisControlLocalizer)
			};
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.SetupComplete += new EventHandler<EventArgs>(application_SetupComplete);
		}
		void application_SetupComplete(object sender, EventArgs e) {
		}
#if !SL
	[DevExpressExpressAppPivotChartLocalizedDescription("PivotChartModuleBaseShowAdditionalNavigation")]
#endif
		public bool ShowAdditionalNavigation {
			get { return showAdditionalNavigation; }
			set { showAdditionalNavigation = value; }
		}
		#region IModelNodesGeneratorUpdater Members
		void IModelNodesGeneratorUpdater.UpdateNode(ModelNode node) {
			IModelPivotChartNavigation navigationItems = (IModelPivotChartNavigation)node;
			Utils.Guard.ArgumentNotNull(navigationItems, "navigationItems");
			navigationItems.GenerateRelatedAnalysisGroup = ShowAdditionalNavigation;
			navigationItems.RelatedAnalysisGroupCaption = DefaultAnalysisDataNavigationItemCaption;
			foreach(IModelClass modelClass in node.Application.BOModel) {
				if(typeof(IAnalysisInfo).IsAssignableFrom(modelClass.TypeInfo.Type)) {
					IModelClassNavigation modelClassNavigation = modelClass as IModelClassNavigation;
					if(modelClassNavigation != null && !modelClassNavigation.IsNavigationItem) {
						ShowNavigationItemController.GenerateNavigationItem(node.Application, new ViewShortcut(modelClass.DefaultListView.Id, null), "Reports");
					}
				}
			}
		}
		Type IModelNodesGeneratorUpdater.GeneratorType { get { return typeof(NavigationItemNodeGenerator); } }
		#endregion
	}
	public interface IModelMemberPivotChartVisibility {
		[
#if !SL
	DevExpressExpressAppPivotChartLocalizedDescription("IModelMemberPivotChartVisibilityIsVisibleInAnalysis"),
#endif
 Category("Behavior")]
		bool IsVisibleInAnalysis { get; set; }
	}
	public interface IModelPivotChartNavigation {
		[
#if !SL
	DevExpressExpressAppPivotChartLocalizedDescription("IModelPivotChartNavigationGenerateRelatedAnalysisGroup"),
#endif
 Category("Behavior")]
		bool GenerateRelatedAnalysisGroup { get; set; }
		[
#if !SL
	DevExpressExpressAppPivotChartLocalizedDescription("IModelPivotChartNavigationRelatedAnalysisGroupCaption"),
#endif
 Localizable(true)]
		string RelatedAnalysisGroupCaption { get; set; }
	}
	[DomainLogic(typeof(IModelMemberPivotChartVisibility))]
	public static class ModelMemberPivotChartVisibilityLogic {
		public static bool Get_IsVisibleInAnalysis(IModelMember modelMember) {
			return DimensionPropertyExtractor.IsVisibleInAnalysis(modelMember.MemberInfo);
		}
	}
}
