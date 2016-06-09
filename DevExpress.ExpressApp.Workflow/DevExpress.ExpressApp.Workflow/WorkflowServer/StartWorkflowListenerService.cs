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
using System.Threading;
using DevExpress.Data.Filtering;
using System.Collections;
using DevExpress.Xpo;
using System.Activities;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Workflow.StartWorkflowConditions;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.Workflow;
using System.Activities.Tracking;
namespace DevExpress.ExpressApp.Workflow.Server {
	public interface IWorkflowServerConditionComparer {
		IList<object> GetTargetObjectKeys(IObjectSpace objectSpace, IStartWorkflowCondition condition);
	}
	public class CriteriaStartWorkflowConditionComparer : ServiceBase, IWorkflowServerConditionComparer {
		private bool IsSuitableTargetObject(IObjectSpace objectSpace, string targetWorkflowUniqueId, object targetObjectKey) {
			bool result = true;
			IRunningWorkflowInstanceInfo instanceInfo = GetService<IRunningWorkflowInstanceInfoService>().FindWorkflowInstanceInfo(targetWorkflowUniqueId, targetObjectKey);
			if(instanceInfo != null) {
				IWorkflowDefinitionSettings definition = GetService<IWorkflowDefinitionProvider>().FindDefinition(targetWorkflowUniqueId) as IWorkflowDefinitionSettings;
				bool isInstanceExecutionFinished = instanceInfo.State == WorkflowInstanceStates.Aborted || instanceInfo.State == WorkflowInstanceStates.Canceled ||
					instanceInfo.State == WorkflowInstanceStates.Completed || instanceInfo.State == WorkflowInstanceStates.Deleted ||
					instanceInfo.State == WorkflowInstanceStates.Terminated || instanceInfo.State == WorkflowInstanceStates.UnhandledException;
				result = definition != null && definition.AllowMultipleRuns && isInstanceExecutionFinished;
			}
			return result;
		}
		public IList<object> GetTargetObjectKeys(IObjectSpace objectSpace, IStartWorkflowCondition condition) {
			using(objectSpace.CreateParseCriteriaScope()) {
				List<object> result = new List<object>();
				CriteriaStartWorkflowCondition criteriaCondition = condition as CriteriaStartWorkflowCondition;
				if(criteriaCondition != null) {
					LocalizedCriteriaWrapper criteriaWrapper = new LocalizedCriteriaWrapper(criteriaCondition.TargetObjectType, criteriaCondition.Criteria);
					new FilterWithObjectsProcessor(objectSpace).Process(criteriaWrapper.CriteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
					string keyPropertyName = objectSpace.GetKeyPropertyName(criteriaCondition.TargetObjectType);
					IList objectKeys = objectSpace.CreateDataView(criteriaCondition.TargetObjectType, keyPropertyName, criteriaWrapper.CriteriaOperator, null);
					int topReturnedObjectsCount = StartWorkflowListenerService.MaxSimultaneouslyOperatedTargetObjectsCount;
					int returnedObjectsCount = 0;
					foreach(object keyRecord in objectKeys) {
						object key = ((XafDataViewRecord)keyRecord)[keyPropertyName];
						if(IsSuitableTargetObject(objectSpace, condition.TargetWorkflowUniqueId, key)) {
							result.Add(key);
							returnedObjectsCount++;
						}
						if(returnedObjectsCount == topReturnedObjectsCount && topReturnedObjectsCount > 0) {
							break;
						}
					}
				}
				return result;
			}
		}
	}
	public class StartWorkflowConditionHandlerManager : ServiceBase {
		public Dictionary<Type, Type> RegisteredConditionComparers = new Dictionary<Type, Type>();
		protected virtual IWorkflowServerConditionComparer FindComparer(IStartWorkflowCondition condition) {
			Type comparerType;
			if(RegisteredConditionComparers.TryGetValue(condition.GetType(), out comparerType)) {
				IWorkflowServerConditionComparer result = (IWorkflowServerConditionComparer)Activator.CreateInstance(comparerType);
				if(result is IService) {
					((IService)result).Initialize(ServiceProvider);
				}
				return result;
			}
			return null;
		}
		public StartWorkflowConditionHandlerManager() {
			RegisteredConditionComparers.Add(typeof(CriteriaStartWorkflowCondition), typeof(CriteriaStartWorkflowConditionComparer));
		}
		public virtual IList<object> GetTargetObjectKeys(IObjectSpace objectSpace, IStartWorkflowCondition condition) {
			IWorkflowServerConditionComparer comparer = FindComparer(condition);
			if(comparer != null) {
				return comparer.GetTargetObjectKeys(objectSpace, condition);
			}
			return new object[0];
		}
	}
	public class StartWorkflowListenerService : BaseTimerService {
		public static int MaxSimultaneouslyOperatedTargetObjectsCount = 0;
		protected StartWorkflowConditionHandlerManager StartWorkflowConditionHandlerManager {
			get {
				StartWorkflowConditionHandlerManager result = FindService<StartWorkflowConditionHandlerManager>();
				if(result != null) {
					return result;
				}
				result = new StartWorkflowConditionHandlerManager();
				result.Initialize(ServiceProvider);
				return result;
			}
		}
		public StartWorkflowListenerService(TimeSpan requestsDetectionPeriod)
			: base(requestsDetectionPeriod) {
		}
		public StartWorkflowListenerService(TimeSpan requestsDetectionPeriod, IObjectSpaceProvider objectSpaceProvider)
			: base(requestsDetectionPeriod, objectSpaceProvider) {
		}
		public override void OnTimer() {
			IList<IStartWorkflowCondition> startWorkflowConditions = GetService<IStartWorkflowConditionProvider>().GetConditions();
			TracerHelper.TraceVariableValue("startWorkflowConditions", startWorkflowConditions);
			TracerHelper.TraceVariableValue("startWorkflowConditions.Count", startWorkflowConditions.Count);
			if(startWorkflowConditions == null || startWorkflowConditions.Count == 0) {
				return;
			}
			foreach(IStartWorkflowCondition condition in startWorkflowConditions) {
				IWorkflowDefinition definition = GetService<IWorkflowDefinitionProvider>().FindDefinition(condition.TargetWorkflowUniqueId);
				TracerHelper.TraceVariableValue("definition", definition);
				if(definition != null) {
					TracerHelper.TraceVariableValue("definition.Name", definition.Name);
					TracerHelper.TraceVariableValue("definition.CanOpenHost", definition.CanOpenHost);
					if(definition.CanOpenHost) {
						using(IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
							IList<object> targetObjectsKeys = StartWorkflowConditionHandlerManager.GetTargetObjectKeys(objectSpace, condition);
							TracerHelper.TraceVariableValue("targetObjects", targetObjectsKeys);
							if(targetObjectsKeys != null && targetObjectsKeys.Count > 0) {
								TracerHelper.TraceVariableValue("targetObjects.Count", targetObjectsKeys.Count);
								foreach(object targetObjectKey in targetObjectsKeys) {
									if(CancellationPending) {
										TracerHelper.TraceVariableValue("CancellationPending", CancellationPending);
										break;
									}
									try {
										GetService<IStartWorkflowService>().StartWorkflow(definition.Name, condition.TargetWorkflowUniqueId, targetObjectKey);
									}
									catch(Exception e) {
										e.Data.Add("StartWorkflowListenerService.StartWorkflows",
											string.Format("TargetObjectKey={0}, TargetWorkflowUniqueId={1}, TargetWorkflowName={2}", targetObjectKey, definition.GetUniqueId(), definition.Name));
										throw;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
