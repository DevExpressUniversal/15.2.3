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
using DevExpress.Workflow.Utils;
using System.Activities.Tracking;
using System.Activities;
using DevExpress.Workflow;
using System.ServiceModel;
using System.Threading;
namespace DevExpress.ExpressApp.Workflow.Server {
	public interface IStartWorkflowService {
		bool StartWorkflow(string targetWorkflowName, string targetWorkflowUniqueId, object targetObjectKey);		
	}
	public class WorkflowServerStartWorkflowService : WorkflowServerService, IStartWorkflowService {
		public virtual bool StartWorkflow(string targetWorkflowName, string targetWorkflowUniqueId, object targetObjectKey) {			
			TracerHelper.TraceMethodEnter(this, "StartWorkflow");
			TracerHelper.TraceVariableValue("targetWorkflowName", targetWorkflowName);
			TracerHelper.TraceVariableValue("targetWorkflowUniqueId", targetWorkflowUniqueId);
			TracerHelper.TraceVariableValue("targetObjectKey", targetObjectKey);
			if(!HostManager.Hosts.ContainsKey(targetWorkflowUniqueId) || HostManager.Hosts[targetWorkflowUniqueId].State != CommunicationState.Opened) {
				TracerHelper.TraceVariableValue("result", false);
				TracerHelper.TraceMethodExit(this, "StartWorkflow");
				return false;
			}
			else {
				Guid instanceHandle = HostManager.Hosts[targetWorkflowUniqueId].StartWorkflowSuspended(new Dictionary<string, object>() { { "targetObjectId", targetObjectKey } });
				GetService<IRunningWorkflowInstanceInfoService>().CreateRunningWorkflowInstanceInfo(targetWorkflowName, HostManager.Hosts[targetWorkflowUniqueId].ActivityUniqueId, targetObjectKey, instanceHandle);
				DevExpress.Persistent.Base.Tracing.Tracer.LogText("WorkflowServerStartWorkflowService.Unsuspend");
				HostManager.Hosts[targetWorkflowUniqueId].Unsuspend(instanceHandle);
				TracerHelper.TraceVariableValue("result", true);
				TracerHelper.TraceMethodExit(this, "StartWorkflow");
				return true;
			}
		}
	}
	public class WorkflowRunningInstanceUpdater : WorkflowServerService {
		private void trackingParticipant_TrackReceived(object sender, TrackingEventArgs e) {
			if(e.Record is WorkflowInstanceRecord) {
				string state = ((WorkflowInstanceRecord)e.Record).State;
				if(state != WorkflowInstanceStates.Deleted) {
					GetService<IRunningWorkflowInstanceInfoService>().SetInstanceInfoState(e.Record.InstanceId, state);
				}
			}
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			HostManager.HostCreated += delegate(object sender, WorkflowHostEventArgs e) {
				TrackingParticipantBase trackingParticipant = new TrackingParticipantBase();
				trackingParticipant.TrackReceived += new EventHandler<TrackingEventArgs>(trackingParticipant_TrackReceived);
				e.WorkflowHost.Host.WorkflowExtensions.Add(trackingParticipant);
			};
		}
	}
}
