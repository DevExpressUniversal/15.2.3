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
using DevExpress.Xpo;
using System.Threading;
using DevExpress.ExpressApp.Utils;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Workflow.MiddleTier;
using DevExpress.ExpressApp.Workflow.CommonServices;
using System.Collections;
using DevExpress.Workflow;
namespace DevExpress.ExpressApp.Workflow.Server {
	public class StartWorkflowByRequestService : BaseTimerService {
		public StartWorkflowByRequestService(Type startWorkflowRequestType, TimeSpan delayPeriod)
			: this(startWorkflowRequestType, delayPeriod, null) {
		}
		public StartWorkflowByRequestService(Type startWorkflowRequestType, TimeSpan delayPeriod, IObjectSpaceProvider objectSpaceProvider) : base(delayPeriod) {
			Guard.TypeArgumentIs(typeof(IStartWorkflowRequest), startWorkflowRequestType, "startWorkflowRequestType");
			this.StartWorkflowRequestType = startWorkflowRequestType;
			this.ObjectSpaceProvider = objectSpaceProvider;
		}
		public override void OnTimer() {
			ProcessRequestsToStartWorkflows();
		}
		public virtual void ProcessRequestsToStartWorkflows() {
			using(IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
				IList startWorkflowRequests = new ArrayList(objectSpace.GetObjects(StartWorkflowRequestType));
				TracerHelper.TraceVariableValue("startWorkflowRequests.Count", startWorkflowRequests.Count);
				foreach(IStartWorkflowRequest request in startWorkflowRequests) {
					if(CancellationPending) {
						TracerHelper.TraceVariableValue("CancellationPending", CancellationPending);
						break;
					}
					try {
						TracerHelper.TraceVariableValue("request.TargetWorkflowUniqueId", request.TargetWorkflowUniqueId);
						IWorkflowDefinition definition = GetService<IWorkflowDefinitionProvider>().FindDefinition(request.TargetWorkflowUniqueId);
						TracerHelper.TraceVariableValue("definition", definition);
						if(definition != null) {
							TracerHelper.TraceVariableValue("definition.Name", definition.Name);
							TracerHelper.TraceVariableValue("definition.CanOpenHost", definition.CanOpenHost);
							if(definition.CanOpenHost) {
								TracerHelper.TraceVariableValue("request.TargetObjectKey", request.TargetObjectKey);
								if(GetService<IStartWorkflowService>().StartWorkflow(definition.Name, request.TargetWorkflowUniqueId, request.TargetObjectKey)) {
									OnRequestProcessed(objectSpace, request);
								}
							}
						}
					}
					catch(Exception e) {
						e.Data.Add("StartWorkflowByRequestService.ProcessRequestsToStartWorkflows.currentRequest",
							string.Format("Key={0}, TargetObjectKey={1}, TargetWorkflowUniqueId={2}", objectSpace.GetKeyValue(request), request.TargetObjectKey, request.TargetWorkflowUniqueId));
						throw;
					}
				}
			}
		}
		protected virtual void OnRequestProcessed(IObjectSpace objectSpace, IStartWorkflowRequest request) {
			objectSpace.Delete(request);
			objectSpace.CommitChanges();
		}
		[Obsolete("Use 'DelayPeriod' instead.")]
		public TimeSpan RequestsDetectionPeriod {
			get { return DelayPeriod; }
			set { DelayPeriod = value; }
		}
		public Type StartWorkflowRequestType { get; private set; }
	}
	public class StartWorkflowByRequestService<T> : StartWorkflowByRequestService where T : IStartWorkflowRequest {
		public StartWorkflowByRequestService(TimeSpan requestsDetectionPeriod) : base(typeof(T), requestsDetectionPeriod) { }
		public StartWorkflowByRequestService(TimeSpan requestsDetectionPeriod, IObjectSpaceProvider objectSpaceProvider) : base(typeof(T), requestsDetectionPeriod, objectSpaceProvider) { }
	}
}
