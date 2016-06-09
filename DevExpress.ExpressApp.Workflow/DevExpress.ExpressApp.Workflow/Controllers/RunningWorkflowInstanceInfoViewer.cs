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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Actions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Workflow.Controllers {
	public class RunningWorkflowInstanceInfoViewer : ViewController<ObjectView> {
#if DebugTest
		public const string ActiveKey_HasAccessToRunningWorkflowInstanceInfoType = "HasAccessToRunningWorkflowInstanceInfoType";
		public const string ActiveKey_NonRunningWorkflowInstanceInfoType = "NonRunningWorkflowInstanceInfoType";
#else
		internal const string ActiveKey_HasAccessToRunningWorkflowInstanceInfoType = "HasAccessToRunningWorkflowInstanceInfoType";
		internal const string ActiveKey_NonRunningWorkflowInstanceInfoType = "NonRunningWorkflowInstanceInfoType";
#endif
		internal const string ListViewCriteriaName = "RunningWorkflowInstanceInfoViewer";
		private void ShowWorkflowInstances_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ShowWorkflowInstances(e);
		}
		protected internal virtual CriteriaOperator GetCriteria(object currentObject) {
			if(currentObject is IWorkflowDefinition) {
				return RunningWorkflowInstanceInfoHelper.CreateSelectInstancesCriteria(((IWorkflowDefinition)currentObject).GetUniqueId());
			}
			else {				
				return RunningWorkflowInstanceInfoHelper.CreateSelectInstancesCriteria(ObjectSpace.GetKeyValue(currentObject));
			}
		}
		protected virtual void ShowWorkflowInstances(SimpleActionExecuteEventArgs e) {
			ListView listView = Application.CreateListView(Application.CreateObjectSpace(), RunningWorkflowInstanceInfoType, true);
			listView.CollectionSource.Criteria[ListViewCriteriaName] = GetCriteria(e.CurrentObject);
			e.ShowViewParameters.CreatedView = listView;
			e.ShowViewParameters.TargetWindow = TargetWindow.NewWindow;
		}
		public RunningWorkflowInstanceInfoViewer() {
			ShowWorkflowInstancesAction = new SimpleAction(this, "ShowWorkflowInstances", WorkflowModule.WorkflowActionCategory);
			ShowWorkflowInstancesAction.ImageName = "Action_Workflow_ShowWorkflowInstances";
			ShowWorkflowInstancesAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			ShowWorkflowInstancesAction.Execute += new SimpleActionExecuteEventHandler(ShowWorkflowInstances_Execute);
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			if(RunningWorkflowInstanceInfoType != null) {
				Active[ActiveKey_HasAccessToRunningWorkflowInstanceInfoType] =
					SecuritySystem.IsGranted(new ObjectAccessPermission(RunningWorkflowInstanceInfoType, ObjectAccess.Read));
				Active[ActiveKey_NonRunningWorkflowInstanceInfoType] =
					view is ObjectView && view.ObjectTypeInfo != null && !typeof(IRunningWorkflowInstanceInfo).IsAssignableFrom(view.ObjectTypeInfo.Type);
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(Frame.Application != null) {
				WorkflowModule module = Frame.Application.Modules.FindModule<WorkflowModule>();
				if(module != null) {
					RunningWorkflowInstanceInfoType = module.RunningWorkflowInstanceInfoType;
				}
			}
			Active["RunningWorkflowInstanceInfoType"] = (RunningWorkflowInstanceInfoType != null);
		}
		public SimpleAction ShowWorkflowInstancesAction { get; private set; }
		public Type RunningWorkflowInstanceInfoType { get; set; }
#if DebugTest
		public CriteriaOperator DebugTest_GetCriteria(object currentObject) {
			return GetCriteria(currentObject);
		}
#endif
	}
}
