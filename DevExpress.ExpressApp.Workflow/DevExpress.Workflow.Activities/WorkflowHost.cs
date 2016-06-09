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
using System.Activities;
using System.Collections.Generic;
using System.ServiceModel.Activities;
using System.ServiceModel;
using System.Activities.Validation;
using DevExpress.Workflow.Utils;
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
namespace DevExpress.Workflow {
	public class WorkflowHost {
		private const string ControlSuffix = "Control";
		private void Host_Faulted(object sender, EventArgs e) {
			if(Faulted != null) {
				Faulted(this, e);
			}
		}
		private void InitializeHost(string workflowHostUri) {
			Host.AddDefaultEndpoints();
			Host.Faulted += new EventHandler(Host_Faulted);
			CreationEndpoint = CreationEndpoint.Create(workflowHostUri);
			Host.AddServiceEndpoint(CreationEndpoint);
			ControlEndpoint = new WorkflowControlEndpoint(new BasicHttpBinding(), new EndpointAddress(new Uri(workflowHostUri + "/" + ControlSuffix)));
			Host.AddServiceEndpoint(ControlEndpoint);
		}
		private void ValidateActivity(Activity activity) {
			ValidationResults validationResults = ConstraintHelper.ValidateActivity(activity);
			if(validationResults.Errors.Count > 0) {
				ValidationException ex = new ValidationException(validationResults.Errors[0].Message);
				ex.Data.Add("WorkflowId", activity.DisplayName);
				throw ex;
			}
		}
		internal protected WorkflowHost() { }
		public WorkflowHost(Activity activity, string configurationName, string workflowHostUri) {
			ValidateActivity(activity);
			WorkflowService service = new WorkflowService {
				Body = activity,
				ConfigurationName = configurationName
			};
			Host = new WorkflowServiceHost(service, new Uri(workflowHostUri));
			InitializeHost(workflowHostUri);
		}
		public WorkflowHost(Activity activity, string workflowHostUri) {
			ValidateActivity(activity);
			Host = new WorkflowServiceHost(activity, new Uri(workflowHostUri));
			InitializeHost(workflowHostUri);
		}
		public virtual Guid StartWorkflow(IDictionary<string, object> inputs) {
			IWorkflowCreation creationClient = new ChannelFactory<IWorkflowCreation>(CreationEndpoint.Binding, CreationEndpoint.Address).CreateChannel();
			return creationClient.Create(inputs);
		}
		public static Guid StartWorkflow(string workflowHostUri, IDictionary<string, object> inputs) {
			return CreationEndpoint.GetCreationClient(workflowHostUri).Create(inputs);
		}
		public virtual Guid StartWorkflowSuspended(IDictionary<string, object> inputs) {
			IWorkflowCreation creationClient = new ChannelFactory<IWorkflowCreation>(CreationEndpoint.Binding, CreationEndpoint.Address).CreateChannel();
			return creationClient.CreateSuspended(inputs);
		}
		public static Guid StartWorkflowSuspended(string workflowHostUri, IDictionary<string, object> inputs) {
			return CreationEndpoint.GetCreationClient(workflowHostUri).CreateSuspended(inputs);
		}
		public virtual void Unsuspend(Guid instanceId) {
			new WorkflowControlClient(ControlEndpoint).Unsuspend(instanceId);
		}
		public static void Unsuspend(string workflowHostUri, Guid instanceId) {
			new WorkflowControlClient(new BasicHttpBinding(), new EndpointAddress(new Uri(workflowHostUri + "/" + ControlSuffix))).Unsuspend(instanceId);
		}
		public static WorkflowControlClient CreateWorkflowControlClient(string workflowHostUri) {
			WorkflowControlEndpoint controlEndpoint = new WorkflowControlEndpoint(new BasicHttpBinding(), new EndpointAddress(new Uri(workflowHostUri + "/" + ControlSuffix)));
			return new WorkflowControlClient(controlEndpoint);
		}
		public virtual CommunicationState State { get { return Host.State; } }
		public WorkflowServiceHost Host { get; private set; }
		public CreationEndpoint CreationEndpoint { get; private set; }
		public WorkflowControlEndpoint ControlEndpoint { get; private set; }
		[Obsolete("Use the 'ActivityUniqueId' property instead")]
		public virtual string ActivityUnigueId { get { return Host.Activity.DisplayName; } }
		public virtual string ActivityUniqueId { get { return Host.Activity.DisplayName; } }
		public event EventHandler Faulted;
	}
}
