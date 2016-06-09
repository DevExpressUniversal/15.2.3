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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[ToolboxBitmap(typeof(ConditionalAppearanceModule), "Resources.Toolbox_Module_ConditionalAppearance")]
	[Description("Provides the capability to customize the view's editors appearance against business rules in XAF applications.")]
	public sealed class ConditionalAppearanceModule : ModuleBase {
		private void Application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
			if(EnableControllersOnLogonWindow) {
				foreach(Type controllerType in GetDeclaredControllerTypes()) {
					e.Controllers.Add((Controller)DevExpress.Persistent.Base.ReflectionHelper.CreateObject(controllerType));
				}
			}
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelAppearanceRules),
			typeof(IModelConditionalAppearance),
			typeof(IModelAppearanceRule),
			typeof(ModelAppearanceRuleLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(DevExpress.ExpressApp.ConditionalAppearance.ActionAppearanceController),
				typeof(DevExpress.ExpressApp.ConditionalAppearance.AppearanceController),
				typeof(DevExpress.ExpressApp.ConditionalAppearance.DetailViewItemAppearanceController),
				typeof(DevExpress.ExpressApp.ConditionalAppearance.DetailViewLayoutItemAppearanceController),
				typeof(DevExpress.ExpressApp.ConditionalAppearance.ListViewItemAppearanceController),
				typeof(DevExpress.ExpressApp.ConditionalAppearance.RefreshAppearanceController),
				typeof(DevExpress.ExpressApp.ConditionalAppearance.AppearanceCustomizationListenerController),
				typeof(DevExpress.ExpressApp.ConditionalAppearance.LookupPropertyEditorRulesCustomizationController)
			};
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.CreateCustomLogonWindowControllers += new EventHandler<CreateCustomLogonWindowControllersEventArgs>(Application_CreateCustomLogonWindowControllers);
		}
		protected override void Dispose(bool disposing) {
			if(Application != null) {
				Application.CreateCustomLogonWindowControllers -= new EventHandler<CreateCustomLogonWindowControllersEventArgs>(Application_CreateCustomLogonWindowControllers);
			}
			base.Dispose(disposing);
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) { }
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelClass, IModelConditionalAppearance>();
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		static ConditionalAppearanceModule() {
			EnableControllersOnLogonWindow = true;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true)]
		public static bool EnableControllersOnLogonWindow { get; set; }
	}
}
