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
using System.Activities.DurableInstancing;
using System.ServiceModel.Activities;
using System.ServiceModel.Description;
using DevExpress.ExpressApp;
using DevExpress.Utils;
namespace DevExpress.Workflow.Store {
	public class WorkflowInstanceStoreBehavior : IServiceBehavior {
		private WorkflowInstanceStore workflowInstanceStore;
		[Obsolete("Use the 'WorkflowInstanceStoreBehavior(Type workflowInstanceType, Type workflowInstanceKeyType, IObjectSpaceProvider objectSpaceProvider)' constructor instead.", true)]
		public WorkflowInstanceStoreBehavior(string connectionString) { }
		[Obsolete("Use the 'WorkflowInstanceStoreBehavior(Type workflowInstanceType, Type workflowInstanceKeyType, IObjectSpaceProvider objectSpaceProvider)' constructor instead.", true)]
		public WorkflowInstanceStoreBehavior(Type workflowInstanceType, Type workflowInstanceKeyType, string connectionString) { }		
		[Obsolete("Use the 'WorkflowInstanceStoreBehavior(Type workflowInstanceType, Type workflowInstanceKeyType, IObjectSpaceProvider objectSpaceProvider)' constructor instead.", true)]
		public WorkflowInstanceStoreBehavior(IObjectSpaceProvider objectSpaceProvider) { }		
		public WorkflowInstanceStoreBehavior(Type workflowInstanceType, Type workflowInstanceKeyType, IObjectSpaceProvider objectSpaceProvider) {
			workflowInstanceStore = new WorkflowInstanceStore(workflowInstanceType, workflowInstanceKeyType, objectSpaceProvider);
		}
		public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }
		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase) {
			Guard.ArgumentNotNull(serviceHostBase, "serviceHostBase");
			WorkflowServiceHost host = serviceHostBase as WorkflowServiceHost;
			if(host != null) {
				host.DurableInstancingOptions.InstanceStore = this.WorkflowInstanceStore;
			}
		}
		public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase) { }
		public WorkflowInstanceStore WorkflowInstanceStore {
			get { return workflowInstanceStore; }
		}
		[Obsolete("Use 'WorkflowInstanceStore.InstanceCompletionAction' instead.")]
		public InstanceCompletionAction InstanceCompletionAction {
			get { return WorkflowInstanceStore.InstanceCompletionAction; }
			set { WorkflowInstanceStore.InstanceCompletionAction = value; }
		}
		[Obsolete("Use 'WorkflowInstanceStore.RunnableInstancesDetectionPeriod' instead.")]
		public TimeSpan RunnableInstancesDetectionPeriod {
			get { return WorkflowInstanceStore.RunnableInstancesDetectionPeriod; }
			set {
				WorkflowInstanceStore.RunnableInstancesDetectionPeriod = value;
			}
		}
	}
}
