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
using System.ServiceModel.Activities;
using DevExpress.Workflow;
using DevExpress.Persistent.Base;
using System.Configuration;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using System.Activities.Tracking;
namespace DevExpress.ExpressApp.Workflow.Visualization {
	public class RunningInstanceController : ViewController<ObjectView> {
		private Type workflowControlCommandRequestType;
		private const string EnabledByWorkflowState = "EnabledByWorkflowState";
		internal const string ActiveKey_HasWriteAccessToWorkflowControlCommandRequestType = "HasWriteAccessToWorkflowControlCommandRequestType";
		public const string ActiveKey_XafPersistentStore = "XafPersistentStore";
		private void unsuspend_Execute(object sender, SimpleActionExecuteEventArgs e) {
			CreateOperationRequest(WorkflowControlCommand.Resume, e, ((ActionBase)sender).TargetObjectsCriteria);
		}
		private void terminate_Execute(object sender, SimpleActionExecuteEventArgs e) {
			CreateOperationRequest(WorkflowControlCommand.Terminate, e, ((ActionBase)sender).TargetObjectsCriteria);
		}
		private void cancel_Execute(object sender, SimpleActionExecuteEventArgs e) {
			CreateOperationRequest(WorkflowControlCommand.Cancel, e, ((ActionBase)sender).TargetObjectsCriteria);
		}
		private void suspend_Execute(object sender, SimpleActionExecuteEventArgs e) {
			CreateOperationRequest(WorkflowControlCommand.Suspend, e, ((ActionBase)sender).TargetObjectsCriteria);
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(WorkflowControlCommandRequestType == null && Frame.Application != null) {
				WorkflowModule module = Frame.Application.Modules.FindModule<WorkflowModule>();
				if(module != null) {
					WorkflowControlCommandRequestType = module.WorkflowControlCommandRequestType;
				}
			}
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			if(WorkflowControlCommandRequestType != null) {
				Active.RemoveItem(ActiveKey_HasWriteAccessToWorkflowControlCommandRequestType);
				if(view.ObjectTypeInfo != null && TargetObjectType.IsAssignableFrom(view.ObjectTypeInfo.Type)) {
					Active[ActiveKey_HasWriteAccessToWorkflowControlCommandRequestType] = SecuritySystem.IsGranted(new ObjectAccessPermission(WorkflowControlCommandRequestType, ObjectAccess.Create));
				}
			}
		}
		protected virtual void CreateOperationRequest(WorkflowControlCommand operation, SimpleActionExecuteEventArgs e, string targetObjectsCriteria) {
			using(IObjectSpace os = Application.CreateObjectSpace()) {
				foreach(IRunningWorkflowInstanceInfo instance in e.SelectedObjects) {
					bool? fit = os.IsObjectFitForCriteria(instance, CriteriaOperator.Parse(targetObjectsCriteria));
					if(fit.HasValue && fit.Value) {
						IWorkflowInstanceControlCommandRequest request = (IWorkflowInstanceControlCommandRequest)os.CreateObject(WorkflowControlCommandRequestType);
						request.TargetActivityInstanceId = instance.ActivityInstanceId;
						request.TargetWorkflowUniqueId = instance.WorkflowUniqueId;
						request.Command = operation;
						os.CommitChanges();
					}
				}
			}
		}
		public RunningInstanceController() {
			TargetObjectType = typeof(IRunningWorkflowInstanceInfo);
			BinaryOperator executingCriteria = new BinaryOperator("State", System.Activities.ActivityInstanceState.Executing);
			CancelAction = new SimpleAction(this, "CancelWorkflowInstance", PredefinedCategory.RecordEdit);
			CancelAction.Caption = "Cancel";
			CancelAction.ConfirmationMessage = "You are about to cancel the selected item(s). Do you want to proceed?";
			CancelAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			CancelAction.Execute += new SimpleActionExecuteEventHandler(cancel_Execute);
			TerminateAction = new SimpleAction(this, "TerminateWorkflowInstance", PredefinedCategory.RecordEdit);
			TerminateAction.Caption = "Terminate";
			TerminateAction.ConfirmationMessage = "You are about to terminate the selected item(s). Do you want to proceed?";
			TerminateAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			TerminateAction.Execute += new SimpleActionExecuteEventHandler(terminate_Execute);
			SuspendAction = new SimpleAction(this, "SuspendWorkflowInstance", PredefinedCategory.RecordEdit);
			SuspendAction.Caption = "Suspend";
			SuspendAction.ConfirmationMessage = "You are about to suspend the selected item(s). Do you want to proceed?";
			SuspendAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			SuspendAction.Execute += new SimpleActionExecuteEventHandler(suspend_Execute);
			SuspendAction.Active[ActiveKey_XafPersistentStore] = false; 
			ResumeAction = new SimpleAction(this, "ResumeWorkflowInstance", PredefinedCategory.RecordEdit);
			ResumeAction.Caption = "Resume";
			ResumeAction.ConfirmationMessage = "You are about to resume the selected item(s). Do you want to proceed?";
			ResumeAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			ResumeAction.Execute += new SimpleActionExecuteEventHandler(unsuspend_Execute);
			ResumeAction.Active[ActiveKey_XafPersistentStore] = false; 
		}
		public Type WorkflowControlCommandRequestType {
			get { return workflowControlCommandRequestType; }
			set {
				workflowControlCommandRequestType = value;
				Active["WorkflowControlCommandRequestType"] = (WorkflowControlCommandRequestType != null);
			}
		}
		public SimpleAction ResumeAction { get; private set; }
		public SimpleAction SuspendAction { get; private set; }
		public SimpleAction CancelAction { get; private set; }
		public SimpleAction TerminateAction { get; private set; }
	}
}
