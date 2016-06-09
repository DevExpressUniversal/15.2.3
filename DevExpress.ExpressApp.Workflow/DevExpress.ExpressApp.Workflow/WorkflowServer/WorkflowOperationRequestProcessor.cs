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
using System.ServiceModel;
using System.ServiceModel.Activities;
using DevExpress.Workflow;
using System.Collections;
namespace DevExpress.ExpressApp.Workflow.Server {
	public class HostCommandRequestProcessor : BaseTimerService {
		public const string ResultPropertyName = "Result";
		private WorkflowHost FindHost(string workflowUniqueId) {
			foreach(WorkflowHost host in HostManager.Hosts.Values) {
				if(host.Host.Activity.DisplayName == workflowUniqueId) {
					return host;
				}
			}
			return null;
		}
		public HostCommandRequestProcessor(Type workflowInstanceControlCommandRequestType, TimeSpan requestsDetectionPeriod) : base(requestsDetectionPeriod) {
			this.WorkflowInstanceControlCommandRequestType = workflowInstanceControlCommandRequestType;
		}
		public HostCommandRequestProcessor(Type workflowInstanceControlCommandRequestType, TimeSpan requestsDetectionPeriod, IObjectSpaceProvider objectSpaceProvider) 
			: base(requestsDetectionPeriod, objectSpaceProvider) {
			this.WorkflowInstanceControlCommandRequestType = workflowInstanceControlCommandRequestType;
		}
		public override void OnTimer() {
			using(IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
				IList objects = objectSpace.GetObjects(WorkflowInstanceControlCommandRequestType, 
					new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, new OperandProperty(ResultPropertyName)));
				List<IWorkflowInstanceControlCommandRequest> workflowOperationRequests = 
					new List<IWorkflowInstanceControlCommandRequest>(objects.Cast<IWorkflowInstanceControlCommandRequest>());
				workflowOperationRequests.Sort(delegate(IWorkflowInstanceControlCommandRequest x, IWorkflowInstanceControlCommandRequest y) { return x.CreatedOn.CompareTo(y.CreatedOn); });
				foreach(IWorkflowInstanceControlCommandRequest request in workflowOperationRequests) {
					if(CancellationPending) {
						break;
					}
					ProcessRequest(objectSpace, request);
					OnRequestProcessed(objectSpace, request);
				}
			}
		}
		protected virtual void ProcessRequest(IObjectSpace objectSpace, IWorkflowInstanceControlCommandRequest request) {
			WorkflowHost host = FindHost(request.TargetWorkflowUniqueId);
			if(host != null && host.State == CommunicationState.Opened) {
				try {
					WorkflowControlClient workflowControlClient = new WorkflowControlClient(host.ControlEndpoint);
					switch(request.Command) {
						case WorkflowControlCommand.Cancel:
							workflowControlClient.Cancel(request.TargetActivityInstanceId);
							break;
						case WorkflowControlCommand.Suspend:
							workflowControlClient.Suspend(request.TargetActivityInstanceId);
							break;
						case WorkflowControlCommand.Resume:
							workflowControlClient.Unsuspend(request.TargetActivityInstanceId);
							break;
						case WorkflowControlCommand.Terminate:
							workflowControlClient.Terminate(request.TargetActivityInstanceId);
							break;
						default:
							throw new ArgumentException();
					}
				}
				catch(Exception e) {
					request.Result = e.Message;
				}
			}
		}
		protected virtual void OnRequestProcessed(IObjectSpace objectSpace, IWorkflowInstanceControlCommandRequest request) {
			if(string.IsNullOrEmpty(request.Result)) {
				objectSpace.Delete(request);
			}
			objectSpace.CommitChanges();
		}
		public Type WorkflowInstanceControlCommandRequestType { get; private set; }
	}
	public class HostCommandRequestProcessor<T> : HostCommandRequestProcessor where T : IWorkflowInstanceControlCommandRequest {
		public HostCommandRequestProcessor(TimeSpan requestsDetectionPeriod)
			: base(typeof(T), requestsDetectionPeriod) {
		}
		public HostCommandRequestProcessor(TimeSpan requestsDetectionPeriod, IObjectSpaceProvider objectSpaceProvider)
			: base(typeof(T), requestsDetectionPeriod, objectSpaceProvider) {
		}
	}
}
