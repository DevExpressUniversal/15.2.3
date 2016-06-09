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
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Workflow.Win {
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[ToolboxBitmap(typeof(WorkflowWindowsFormsModule), "Resources.Toolbox_Module_Workflow_Win.ico")]
	[Description("Integrates Windows Workflow Foundation in XAF Win applications.")]
	[ToolboxItemFilter("Xaf.Platform.Win")]
	public sealed class WorkflowWindowsFormsModule : ModuleBase {
		public override void Setup(XafApplication application) {
			base.Setup(application);
			AvailableActivitiesProvider = new AvailableActivitiesProvider(application, this);
		}
		public WorkflowWindowsFormsModule() {
			this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
			this.RequiredModuleTypes.Add(typeof(WorkflowModule));
		}
		protected override void RegisterEditorDescriptors(System.Collections.Generic.List<DevExpress.ExpressApp.Editors.EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration("WorkflowPropertyEditor", typeof(string), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration("WorkflowPropertyEditor", typeof(string), typeof(WorkflowPropertyEditor), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration("TrackingRecordVisualizationInfoPropertyEditor", typeof(ITrackingRecordVisualizationInfo), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration("TrackingRecordVisualizationInfoPropertyEditor", typeof(ITrackingRecordVisualizationInfo), typeof(WorkflowVisualizationPropertyEditor), true)));
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return new Type[0];
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] { 
				typeof(DevExpress.ExpressApp.Workflow.Win.DebugWorkflowController), 
				typeof(WinTrackingVisualizationController), 
				typeof(AvailableActivitiesController), 
				typeof(WorkflowVisualizationController), 
				typeof(DisableAutoCommitListViewController) };
		}
		public List<ActivityInformation> GetAvailableActivities(List<ActivityInformation> activitiesInformation) {
			ActivitiesInformationEventArgs activitiesInformationEventArgs = new ActivitiesInformationEventArgs(activitiesInformation);
			if(QueryAvailableActivities != null) {
				QueryAvailableActivities(this, activitiesInformationEventArgs);
			}
			return activitiesInformationEventArgs.ActivitiesInformation;
		}
		[Browsable(false)]
		public AvailableActivitiesProvider AvailableActivitiesProvider { get; set; }
		public override void AddGeneratorUpdaters(Model.Core.ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new NavigationItemsWorkflowsUpdater(ModuleManager));
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		[DXDescription("DevExpress.ExpressApp.Workflow.Win.WorkflowWindowsFormsModule,QueryAvailableActivities")]
		public event EventHandler<ActivitiesInformationEventArgs> QueryAvailableActivities;
	}
}
