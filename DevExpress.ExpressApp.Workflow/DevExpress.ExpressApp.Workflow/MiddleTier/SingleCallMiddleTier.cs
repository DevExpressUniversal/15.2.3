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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Workflow.ServiceModel;
namespace DevExpress.ExpressApp.Workflow.MiddleTier {
	[Obsolete("Use the 'DevExpress.ExpressApp.MiddleTier.DataServiceOperationStartedEventArgs' instead.")]
	public class ObjectSpaceManipulationEventArgs : EventArgs {
		public ObjectSpaceManipulationEventArgs(IObjectSpace objectSpace) : base() {
			this.ObjectSpace = objectSpace;
		}
		public IObjectSpace ObjectSpace { get; private set; }
	}
	[Obsolete("Use the 'DevExpress.ExpressApp.MiddleTier.ApplicationDataService' instead.")]
	public interface ISingleCallMiddleTier {
		event EventHandler<ObjectSpaceManipulationEventArgs> Committing;
		event EventHandler<ObjectSpaceManipulationEventArgs> Committed;
	}
	[Obsolete("Use the 'DevExpress.ExpressApp.MiddleTier.ApplicationDataService' instead.")]
	public interface ISingleCallMiddleTierCore : ISingleCallMiddleTier {
		void RaiseCommitting(IObjectSpace objectSpace);
		void RaiseCommitted(IObjectSpace objectSpace);
	}
	[Obsolete("Use the 'DevExpress.ExpressApp.MiddleTier.ApplicationDataService' instead.")]
	public class SingleCallMiddleTier : ISingleCallMiddleTierCore {
		public SingleCallMiddleTier() {
			this.ServiceProvider = new ServiceProvider();
			ServiceProvider.AddService(this);
		}
		public void CreateDefaultWorkflowServices(Type workflowDefinitionType, Type startWorkflowRequestType) {
			ServiceProvider.AddService(new MiddleTierAutoStartWorkflowService());
			ServiceProvider.AddService(new MiddleTierWorkflowStarterService(startWorkflowRequestType));
			ServiceProvider.AddService(new DevExpress.ExpressApp.Workflow.CommonServices.StartWorkflowConditionsProvider());
			ServiceProvider.AddService(new DevExpress.ExpressApp.Workflow.CommonServices.WorkflowDefinitionProvider(workflowDefinitionType));
		}
		public ServiceProvider ServiceProvider { get; private set; }
		public void RaiseCommitting(IObjectSpace objectSpace) {
			if(Committing != null) {
				Committing(this, new ObjectSpaceManipulationEventArgs(objectSpace));
			}
		}
		public void RaiseCommitted(IObjectSpace objectSpace) {
			if(Committed != null) {
				Committed(this, new ObjectSpaceManipulationEventArgs(objectSpace));
			}
		}
		public event EventHandler<ObjectSpaceManipulationEventArgs> Committing;
		public event EventHandler<ObjectSpaceManipulationEventArgs> Committed;
	}
	public static class WorkflowServices {
		public static void AddDefaultWorkflowServices(DevExpress.ExpressApp.MiddleTier.ServiceProvider serviceProvider, Type workflowDefinitionType, Type startWorkflowRequestType) {
			serviceProvider.AddService(new MiddleTierAutoStartWorkflowService());
			serviceProvider.AddService(new MiddleTierWorkflowStarterService(startWorkflowRequestType));
			serviceProvider.AddService(new DevExpress.ExpressApp.Workflow.CommonServices.StartWorkflowConditionsProvider());
			serviceProvider.AddService(new DevExpress.ExpressApp.Workflow.CommonServices.WorkflowDefinitionProvider(workflowDefinitionType));
		}
	}
}
