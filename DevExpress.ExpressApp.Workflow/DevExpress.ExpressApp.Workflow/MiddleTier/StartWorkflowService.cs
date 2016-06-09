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
using DevExpress.ExpressApp.Workflow.StartWorkflowConditions;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp.Security.ClientServer;
namespace DevExpress.ExpressApp.Workflow.MiddleTier {
	public interface IMiddleTierConditionComparer {
		IList<object> GetTargetObjects(IObjectSpace objectSpace, IStartWorkflowCondition startWorkFlowCondition);
	}
	public class MiddleTierObjectCreatedConditionComparer : IMiddleTierConditionComparer {
		public IList<object> GetTargetObjects(IObjectSpace objectSpace, IStartWorkflowCondition startWorkFlowCondition) {
			Guard.ArgumentNotNull(objectSpace, "objectSpace");
			IList<object> targetObjects = new List<object>();
			ObjectCreatedStartWorkflowCondition creationCondition = startWorkFlowCondition as ObjectCreatedStartWorkflowCondition;
			if(creationCondition != null) {
				foreach(object obj in objectSpace.GetObjectsToSave(false)) {
					if(creationCondition.TargetObjectType.IsAssignableFrom(obj.GetType()) && objectSpace.IsNewObject(obj)) {
						targetObjects.Add(obj);
					}
				}
			}
			return targetObjects;
		}
	}
	public class MiddleTierAutoStartWorkflowService : DevExpress.ExpressApp.MiddleTier.ServiceBase {
		private void dataService_Committing(object sender, DataServiceOperationEventArgs e) {
			IStartWorkflowConditionProvider startWorkflowConditionProvider = ServiceProvider.GetService<IStartWorkflowConditionProvider>();
			Guard.ArgumentNotNull(startWorkflowConditionProvider, "ServiceProvider.GetService<IStartWorkflowConditionProvider>()");
			List<IStartWorkflowCondition> conditions = new List<IStartWorkflowCondition>(startWorkflowConditionProvider.GetConditions());
			if(conditions == null)
				return;
			Dictionary<string, List<object>>  targetWorkflows = new Dictionary<string, List<object>>();
			foreach(IStartWorkflowCondition condition in conditions) {
				IMiddleTierConditionComparer comparer = FindComparer(condition);
				if(comparer != null) {
					IList<object> targetObjects = comparer.GetTargetObjects(e.ObjectSpace, condition);
					if(targetObjects != null) {
						foreach(object obj in targetObjects) {
							List<object> workflowObjects;
							if(!targetWorkflows.TryGetValue(condition.TargetWorkflowUniqueId, out workflowObjects)) {
								workflowObjects = new List<object>();
								targetWorkflows[condition.TargetWorkflowUniqueId] = workflowObjects;
							}
							workflowObjects.Add(obj);
						}
					}
				}
				else {
				}
			}
			if(targetWorkflows.Count > 0) {
				e.Completed += delegate(object senderCompleted, EventArgs argsCompleted) {
					IWorkflowStarter workflowStarter = ServiceProvider.GetService<IWorkflowStarter>();
					Guard.ArgumentNotNull(workflowStarter, "ServiceProvider.GetService<IWorkflowStarter>()");
					workflowStarter.StartWorkflows(targetWorkflows);
				};
			}
		}
		protected virtual IMiddleTierConditionComparer FindComparer(IStartWorkflowCondition condition) {
			IMiddleTierConditionComparer result = null;
			if(RegisteredConditionComparers.TryGetValue(condition.GetType(), out result)) {
				return result;
			}
			return null;
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			IApplicationDataService dataService = ServiceProvider.GetService<IApplicationDataService>();
			Guard.ArgumentNotNull(dataService, "ServiceProvider.GetService<ApplicationDataService>()");
			dataService.Committing += new EventHandler<DataServiceOperationEventArgs>(dataService_Committing);
		}
		static MiddleTierAutoStartWorkflowService() {
			RegisteredConditionComparers.Add(typeof(ObjectCreatedStartWorkflowCondition), new MiddleTierObjectCreatedConditionComparer());
		}
		public static Dictionary<Type, IMiddleTierConditionComparer> RegisteredConditionComparers = new Dictionary<Type, IMiddleTierConditionComparer>();
	}
}
