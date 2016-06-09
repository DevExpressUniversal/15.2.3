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
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel.Design;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Web.ASPxScheduler;
namespace DevExpress.ExpressApp.Scheduler.Web {
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Uses the ASPxScheduler controls suite to display DevExpress.Persistent.Base.IEvent objects in Web XAF applications.")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[ToolboxBitmap(typeof(SchedulerAspNetModule), "Resources.Toolbox_Module_Scheduler_Web.ico")]
	[Designer("DevExpress.ExpressApp.Scheduler.Web.Design.SchedulerWebDesigner, DevExpress.ExpressApp.Scheduler.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[DesignerCategory("Component")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	public sealed class SchedulerAspNetModule : SchedulerModuleBase {
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList requiredModuleTypes = base.GetRequiredModuleTypesCore();
			requiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
			return requiredModuleTypes;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelListViewSchedulerWeb)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(SchedulerActionsController),
				typeof(SchedulerDetailViewActionsController),
				typeof(SchedulerListViewController),
				typeof(SchedulerRecurrenceInfoController),
				typeof(DateNavigatorController)
			};
		}
		public override void Setup(XafApplication application)
		{
			base.Setup(application);
			application.LoggedOn += delegate(object sender, LogonEventArgs e)
			{
				if (NotificationsProvider != null) {
					NotificationsProvider.SetSchedulerStorage(new XafASPxSchedulerStorage());
					RegisterNotificationsProvider();
				}
			};
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(ASPxSchedulerControlLocalizer));
			result.Add(typeof(SchedulerControlCoreLocalizer));
			result.Add(typeof(SchedulerAspNetModuleLocalizer));
			return result;
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelListView, IModelListViewSchedulerWeb>();
		}
		protected override void RegisterEditorDescriptors(List<DevExpress.ExpressApp.Editors.EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(EditorAliases.SchedulerListEditor, typeof(IEvent), typeof(ASPxSchedulerListEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.SchedulerLabelPropertyEditor, typeof(int), typeof(ASPxSchedulerLabelPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.SchedulerStatusPropertyEditor, typeof(int), typeof(ASPxSchedulerStatusPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.SchedulerRecurrenceInfoPropertyEditor, typeof(string), typeof(ASPxSchedulerRecurrenceInfoPropertyEditor), true)));
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new ListViewDataAccessModeGeneratorUpdater(typeof(ASPxSchedulerListEditor), new List<CollectionSourceDataAccessMode>() { CollectionSourceDataAccessMode.Client, CollectionSourceDataAccessMode.DataView}));
		}
	}
	public interface IModelListViewSchedulerWeb {
		[DefaultValue(2)]
		[ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
#if !SL
	[DevExpressExpressAppSchedulerWebLocalizedDescription("IModelListViewSchedulerWebDateNavigatorRowCount")]
#endif
		int DateNavigatorRowCount { get; set; }
		[ModelBrowsable(typeof(ModelListViewSchedulerVisibilityCalculator))]
		[
#if !SL
	DevExpressExpressAppSchedulerWebLocalizedDescription("IModelListViewSchedulerWebScrollAreaHeight"),
#endif
 DefaultValue(-1)]
		int ScrollAreaHeight { get; set; }
	}
}
