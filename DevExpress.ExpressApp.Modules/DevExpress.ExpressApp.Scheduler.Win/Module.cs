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
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
namespace DevExpress.ExpressApp.Scheduler.Win {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Uses the XtraScheduler controls suite to display DevExpress.Persistent.Base.IEvent objects in Windows Forms XAF applications.")]
	[ToolboxBitmap(typeof(SchedulerWindowsFormsModule), "Resources.Toolbox_Module_Scheduler_Win.ico")]
	[ToolboxItemFilter("Xaf.Platform.Win")]
	public sealed class SchedulerWindowsFormsModule : SchedulerModuleBase {
		public override void Setup(XafApplication application)
		{
			base.Setup(application);
			application.LoggedOn += delegate(object sender, LogonEventArgs e)
			{
				if (NotificationsProvider != null) {
					NotificationsProvider.SetSchedulerStorage(new SchedulerStorage());
					RegisterNotificationsProvider();
				}
			};
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(SchedulerControlLocalizer));
			return result;
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList requiredModuleTypes = base.GetRequiredModuleTypesCore();
			requiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
			return requiredModuleTypes;
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelOptions, IModelOptionsSchedulerModule>();
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(EditorAliases.SchedulerListEditor, typeof(IEvent), typeof(SchedulerListEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.SchedulerLabelPropertyEditor, typeof(int), typeof(SchedulerLabelPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.SchedulerStatusPropertyEditor, typeof(int), typeof(SchedulerStatusPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.SchedulerRecurrenceInfoPropertyEditor, typeof(string), typeof(SchedulerRecurrenceInfoPropertyEditor), true)));
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelOptionsSchedulerModule),
			typeof(IModelOptionsScheduler)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(SchedulerDetailViewController),
				typeof(SchedulerListViewController),
				typeof(SchedulerRecurrenceInfoController),
				typeof(SchedulerResourceDeletingController)
			};
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new ListViewDataAccessModeGeneratorUpdater(typeof(SchedulerListEditor), new List<CollectionSourceDataAccessMode>() { CollectionSourceDataAccessMode.Client, CollectionSourceDataAccessMode.DataView }));
			updaters.Add(new LookupListViewDefaultEditorUpdater());
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
	}
	public interface IModelOptionsSchedulerModule {
#if !SL
	[DevExpressExpressAppSchedulerWinLocalizedDescription("IModelOptionsSchedulerModuleSchedulerModule")]
#endif
		IModelOptionsScheduler SchedulerModule { get; }
	}
#if !SL
	[DevExpressExpressAppSchedulerWinLocalizedDescription("WinIModelOptionsScheduler")]
#endif
	public interface IModelOptionsScheduler : IModelNode {
		[DefaultValue(RecurrentAppointmentAction.Ask)]
		[
#if !SL
	DevExpressExpressAppSchedulerWinLocalizedDescription("IModelOptionsSchedulerRecurrentAppointmentEditAction"),
#endif
 Category("Behavior")]
		RecurrentAppointmentAction RecurrentAppointmentEditAction { get; set; }
	}
}
