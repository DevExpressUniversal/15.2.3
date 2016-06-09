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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Allows you to create several variants of a List View. Includes an Action that allows end-users to switch between different List View variants.")]
	[ToolboxBitmap(typeof(ViewVariantsModule), "Resources.Toolbox_Module_ViewVariants.ico")]
	public sealed class ViewVariantsModule : ModuleBase {
		private const bool ShowAdditionalNavigationDefault = false;
		private bool showAdditionalNavigation = ShowAdditionalNavigationDefault;
		private bool isVariantsProviderDefaultValue = true;
		private bool isFrameVariantsEngineDefaultValue = true;
		private IVariantsProvider variantsProvider;
		private IFrameVariantsEngine frameVariantsEngine;
		private void UpdateVariantsProviderDefaultValue(IModelApplication model) {
			if(model != null) {
				SetVariantsProvider(new ModelVariantsProvider(model));
			}
			else {
				SetVariantsProvider(null);
			}
		}
		private void Application_ModelChanged(object sender, EventArgs e) {
			if(isVariantsProviderDefaultValue) {
				UpdateVariantsProviderDefaultValue(((XafApplication)sender).Model);
			}
		}
		private void SetVariantsProvider(IVariantsProvider value) {
			if(isFrameVariantsEngineDefaultValue) {
				SetFrameVariantsEngine(null);
			}
			variantsProvider = value;
			if(isFrameVariantsEngineDefaultValue && (variantsProvider != null)) {
				SetFrameVariantsEngine(new FrameVariantsEngine(variantsProvider, new XafApplicationViewsFactory(Application)));
			}
			if(VariantsProviderChanged != null) {
				VariantsProviderChanged(this, EventArgs.Empty);
			}
		}
		private void SetFrameVariantsEngine(IFrameVariantsEngine value) {
			if(frameVariantsEngine == value) {
				return;
			}
			if((frameVariantsEngine != null) && (frameVariantsEngine is IDisposable)) {
				((IDisposable)frameVariantsEngine).Dispose();
			}
			frameVariantsEngine = value;
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.ModelChanged += Application_ModelChanged;
			UpdateVariantsProviderDefaultValue(application.Model);
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelNavigationItemsVariantSettings),
			typeof(IModelViewVariants),
			typeof(IModelVariants),
			typeof(IModelVariant),
			typeof(ModelVariantLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(ChangeVariantController),
				typeof(CustomizeNavigationItemsController)
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelView, IModelViewVariants>();
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			if(ShowAdditionalNavigation) {
				updaters.Add(new UpdateNavigationItemNodeGenerator());
			}
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		[
#if !SL
	DevExpressExpressAppViewVariantsModuleLocalizedDescription("ViewVariantsModuleShowAdditionalNavigation"),
#endif
 DefaultValue(ShowAdditionalNavigationDefault)]
		public bool ShowAdditionalNavigation {
			get { return showAdditionalNavigation; }
			set { showAdditionalNavigation = value; }
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public IVariantsProvider VariantsProvider {
			get {
				return variantsProvider;
			}
			set {
				isVariantsProviderDefaultValue = false;
				if(Application != null) {
					Application.ModelChanged -= Application_ModelChanged; 
				}
				SetVariantsProvider(value);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public IFrameVariantsEngine FrameVariantsEngine {
			get {
				return frameVariantsEngine;
			}
			set {
				isFrameVariantsEngineDefaultValue = false;
				SetFrameVariantsEngine(value);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<EventArgs> VariantsProviderChanged;
		#region Obsoleted since 15.1
		[Obsolete(ModelVariantsProvider.DefaultVariantObsoleteText), EditorBrowsable(EditorBrowsableState.Never)] 
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public bool GenerateVariantsNode {
			get { return false; }
			set { }
		}
		#endregion
	}
	class UpdateNavigationItemNodeGenerator : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator> {
		#region IModelNodesGeneratorUpdater Members
		public override void UpdateNode(ModelNode node) {
			IModelNavigationItemsVariantSettings navigationItems = (IModelNavigationItemsVariantSettings)node;
			if(navigationItems != null) {
				navigationItems.GenerateRelatedViewVariantsGroup = true;
			}
		}
		#endregion
	}
}
