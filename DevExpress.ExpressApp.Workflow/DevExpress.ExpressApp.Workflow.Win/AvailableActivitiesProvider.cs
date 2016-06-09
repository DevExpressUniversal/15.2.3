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
using System.Activities.Statements;
using System.ServiceModel.Activities;
using DevExpress.Workflow.Activities;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Workflow.Win;
using System.Activities.Core.Presentation.Factories;
namespace DevExpress.ExpressApp.Workflow {
	public class ActivitiesInformationEventArgs : EventArgs {
		List<ActivityInformation> activitiesInformation = null;
		public ActivitiesInformationEventArgs(List<ActivityInformation> activitiesInformation) {
			this.activitiesInformation = activitiesInformation;
		}
		public List<ActivityInformation> ActivitiesInformation {
			get { return activitiesInformation; }
		}
	}
	public class AvailableActivitiesProvider {
		private XafApplication application;
		private WorkflowWindowsFormsModule workflowWindowsFormsModule;
		List<ActivityInformation> registeredActivities = new List<ActivityInformation>();
		IDictionary<string, Type> createdTypes = new Dictionary<string, Type>();
		private Type WorkflowDefinitionType {
			get {
				if(workflowWindowsFormsModule != null) {
					WorkflowModule workflowModule = workflowWindowsFormsModule.ModuleManager.Modules.FindModule<WorkflowModule>();
					if(workflowModule != null) {
						return workflowModule.WorkflowDefinitionType;
					}
				}
				return null;
			}
		}
		private void FillDefaultRegisteredActivities() {
			const string controlFlowCategoryName = "Control Flow";
			registeredActivities.Add(new ActivityInformation(typeof(DoWhile), controlFlowCategoryName, ImageLoader.Instance.GetImageInfo("DoWhile").Image));
			registeredActivities.Add(new ActivityInformation(typeof(ForEachWithBodyFactory<>), controlFlowCategoryName, "ForEach<T>", ImageLoader.Instance.GetImageInfo("ForEach").Image));
			registeredActivities.Add(new ActivityInformation(typeof(If), controlFlowCategoryName, ImageLoader.Instance.GetImageInfo("If").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Parallel), controlFlowCategoryName, ImageLoader.Instance.GetImageInfo("Parallel").Image));
			registeredActivities.Add(new ActivityInformation(typeof(ParallelForEachWithBodyFactory<>), controlFlowCategoryName, "ParallelForEach<T>", ImageLoader.Instance.GetImageInfo("ParallelForEach").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Pick), controlFlowCategoryName, ImageLoader.Instance.GetImageInfo("Pick").Image));
			registeredActivities.Add(new ActivityInformation(typeof(PickBranch), controlFlowCategoryName, ImageLoader.Instance.GetImageInfo("PickBranch").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Sequence), controlFlowCategoryName, ImageLoader.Instance.GetImageInfo("Sequence").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Switch<>), controlFlowCategoryName, "Switch<T>", ImageLoader.Instance.GetImageInfo("Switch").Image));
			registeredActivities.Add(new ActivityInformation(typeof(While), controlFlowCategoryName, ImageLoader.Instance.GetImageInfo("While").Image));
			string flowchartCategoryName = "Flowchart";
			registeredActivities.Add(new ActivityInformation(typeof(Flowchart), flowchartCategoryName, ImageLoader.Instance.GetImageInfo("Flowchart").Image));
			registeredActivities.Add(new ActivityInformation(typeof(FlowDecision), flowchartCategoryName, ImageLoader.Instance.GetImageInfo("FlowDecision").Image));
			registeredActivities.Add(new ActivityInformation(typeof(FlowSwitch<>), flowchartCategoryName, "FlowSwitch<T>", ImageLoader.Instance.GetImageInfo("FlowSwitch").Image));
			string messagingCategoryName = "Messaging";
			registeredActivities.Add(new ActivityInformation(typeof(Send), messagingCategoryName, ImageLoader.Instance.GetImageInfo("Send").Image));
			registeredActivities.Add(new ActivityInformation(typeof(SendReply), messagingCategoryName, ImageLoader.Instance.GetImageInfo("SendReply").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Receive), messagingCategoryName, ImageLoader.Instance.GetImageInfo("Receive").Image));
			registeredActivities.Add(new ActivityInformation(typeof(ReceiveReply), messagingCategoryName, ImageLoader.Instance.GetImageInfo("ReceiveReply").Image));
			registeredActivities.Add(new ActivityInformation(typeof(InitializeCorrelation), messagingCategoryName, ImageLoader.Instance.GetImageInfo("InitializeCorrelation").Image));
			registeredActivities.Add(new ActivityInformation(typeof(CorrelationScope), messagingCategoryName, ImageLoader.Instance.GetImageInfo("CorrelationScope").Image));
			registeredActivities.Add(new ActivityInformation(typeof(TransactedReceiveScope), messagingCategoryName, ImageLoader.Instance.GetImageInfo("TransactedReceiveScope").Image));
			string runtimeCategoryName = "Runtime";
			registeredActivities.Add(new ActivityInformation(typeof(Persist), runtimeCategoryName, ImageLoader.Instance.GetImageInfo("Persist").Image));
			registeredActivities.Add(new ActivityInformation(typeof(TerminateWorkflow), runtimeCategoryName, ImageLoader.Instance.GetImageInfo("TerminateWorkflow").Image));
			string primitivesCategoryName = "Primitives";
			registeredActivities.Add(new ActivityInformation(typeof(Assign), primitivesCategoryName, ImageLoader.Instance.GetImageInfo("Assign").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Delay), primitivesCategoryName, ImageLoader.Instance.GetImageInfo("Delay").Image));
			registeredActivities.Add(new ActivityInformation(typeof(InvokeMethod), primitivesCategoryName, ImageLoader.Instance.GetImageInfo("InvokeMethod").Image));
			registeredActivities.Add(new ActivityInformation(typeof(WriteLine), primitivesCategoryName, ImageLoader.Instance.GetImageInfo("WriteLine").Image));
			string transactionCategoryName = "Transaction";
			registeredActivities.Add(new ActivityInformation(typeof(CancellationScope), transactionCategoryName, ImageLoader.Instance.GetImageInfo("CancellationScope").Image));
			registeredActivities.Add(new ActivityInformation(typeof(CompensableActivity), transactionCategoryName, ImageLoader.Instance.GetImageInfo("CompensableActivity").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Compensate), transactionCategoryName, ImageLoader.Instance.GetImageInfo("Compensate").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Confirm), transactionCategoryName, ImageLoader.Instance.GetImageInfo("Confirm").Image));
			registeredActivities.Add(new ActivityInformation(typeof(TransactionScope), transactionCategoryName, ImageLoader.Instance.GetImageInfo("TransactionScope").Image));
			string collectionCategoryName = "Collection";
			registeredActivities.Add(new ActivityInformation(typeof(AddToCollection<>), collectionCategoryName, "AddToCollection<T>", ImageLoader.Instance.GetImageInfo("AddToCollection").Image));
			registeredActivities.Add(new ActivityInformation(typeof(ClearCollection<>), collectionCategoryName, "ClearCollection<T>", ImageLoader.Instance.GetImageInfo("ClearCollection").Image));
			registeredActivities.Add(new ActivityInformation(typeof(ExistsInCollection<>), collectionCategoryName, "ExistsInCollection<T>", ImageLoader.Instance.GetImageInfo("ExistsInCollection").Image));
			registeredActivities.Add(new ActivityInformation(typeof(RemoveFromCollection<>), collectionCategoryName, "RemoveFromCollection<T>", ImageLoader.Instance.GetImageInfo("RemoveFromCollection").Image));
			string errorHandlingCategoryName = "Error Handling";
			registeredActivities.Add(new ActivityInformation(typeof(Rethrow), errorHandlingCategoryName, ImageLoader.Instance.GetImageInfo("Rethrow").Image));
			registeredActivities.Add(new ActivityInformation(typeof(Throw), errorHandlingCategoryName, ImageLoader.Instance.GetImageInfo("Throw").Image));
			registeredActivities.Add(new ActivityInformation(typeof(TryCatch), errorHandlingCategoryName, ImageLoader.Instance.GetImageInfo("TryCatch").Image));
			registeredActivities.Add(new ActivityInformation(typeof(NoPersistScope), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(CreateObjectSpace), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(CommitChanges), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(Rollback), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(DeleteObject<>), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(GetObjectByKey<>), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(FindObjectByCriteria<>), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(GetObjectsByCriteria<>), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(CreateObject<>), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(GetObjectKey), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(ObjectSpaceTransactionScope), ActivitiesAssemblyInfo.DXActivitiesTabName));
			registeredActivities.Add(new ActivityInformation(typeof(TransactionalGetObjectSpace), ActivitiesAssemblyInfo.DXActivitiesTabName));
		}
		public AvailableActivitiesProvider(XafApplication application, WorkflowWindowsFormsModule workflowWindowsFormsModule) {
			this.application = application;
			this.workflowWindowsFormsModule = workflowWindowsFormsModule;
			FillDefaultRegisteredActivities();
		}
		public void RefreshCompiledActivities() {
			createdTypes.Clear();
			if(application.ObjectSpaceProvider != null && WorkflowDefinitionType != null) {
				List<UserActivityDescription> activityDefinitions = new List<UserActivityDescription>();
				using(IObjectSpace os = application.ObjectSpaceProvider.CreateObjectSpace()) {
					foreach(IWorkflowDefinition workflowDefinition in os.GetObjects(WorkflowDefinitionType)) {
						if(workflowDefinition.CanCompileForDesigner) {
							activityDefinitions.Add(new UserActivityDescription(workflowDefinition.GetUniqueId(), workflowDefinition.Xaml, workflowDefinition.Name));
						}
					}
				}
				createdTypes = UserActivityBuilderHelper.CreateUserActivityTypes(activityDefinitions);
			}
		}
		public virtual List<ActivityInformation> GetAvailableActivities() {
			string userActivitiesCategoryName = "User Activities";
			List<ActivityInformation> availableActivities = new List<ActivityInformation>(registeredActivities);
			foreach(KeyValuePair<string, Type> activityTypeDescription in createdTypes) {
				availableActivities.Add(new ActivityInformation(activityTypeDescription.Value, userActivitiesCategoryName, activityTypeDescription.Key));
			}
			if(workflowWindowsFormsModule != null) {
				return workflowWindowsFormsModule.GetAvailableActivities(availableActivities);
			}
			return availableActivities;
		}
	}
}
