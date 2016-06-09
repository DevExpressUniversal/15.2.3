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
using DevExpress.ExpressApp.Workflow.MiddleTier;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security.ClientServer;
namespace DevExpress.ExpressApp.Workflow.Controllers {
#if DebugTest
	public class FakeApplicationDataService : IApplicationDataService {
#else
	internal class FakeApplicationDataService : IApplicationDataService {
#endif
		public void RaiseCommitting(DataServiceOperationEventArgs args) {
			if(Committing != null) {
				Committing(this, args);
			}
		}
		public event EventHandler<DataServiceOperationEventArgs> Committing;
	}
	public class DataServiceCreatedEventArgs : EventArgs {
		public DataServiceCreatedEventArgs(DevExpress.ExpressApp.MiddleTier.ServiceProvider serviceProvider, IApplicationDataService dataService) {
			this.DataService = dataService;
			this.ServiceProvider = serviceProvider;
		}
		public IApplicationDataService DataService { get; private set; }
		public DevExpress.ExpressApp.MiddleTier.IServiceProviderEx ServiceProvider { get; private set; }
	}
	public class StartWorkflowController : WindowController {
		public StartWorkflowController() {
			TargetWindowType = WindowType.Main;
		}
		public Type StartWorkflowRequestType { get; set; }
		public Type WorkflowDefinitionType { get; set; }
		protected override void OnActivated() {
			base.OnActivated();
			WorkflowModule module = Application.Modules.FindModule<WorkflowModule>();
			if(module != null) {
				StartWorkflowRequestType = module.StartWorkflowRequestType;
				WorkflowDefinitionType = module.WorkflowDefinitionType;
				Active["WorkflowModule.ActivateStartWorkflowController"] = module.ActivateStartWorkflowController;
			}
			if(StartWorkflowRequestType != null && WorkflowDefinitionType != null && Active) { 
				Application.ObjectSpaceCreated += new EventHandler<ObjectSpaceCreatedEventArgs>(Application_ObjectSpaceCreated);
			}
		}
		protected override void OnDeactivated() {
			Application.ObjectSpaceCreated -= new EventHandler<ObjectSpaceCreatedEventArgs>(Application_ObjectSpaceCreated);
			base.OnDeactivated();
		}
		private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e) {
			FakeApplicationDataService dataService = new FakeApplicationDataService();
			DevExpress.ExpressApp.MiddleTier.ServiceProvider serviceProvider = new DevExpress.ExpressApp.MiddleTier.ServiceProvider();
			serviceProvider.AddService(dataService);
			serviceProvider.AddService(Application.ObjectSpaceProvider);
			WorkflowServices.AddDefaultWorkflowServices(serviceProvider, WorkflowDefinitionType, StartWorkflowRequestType);
			if(DataServiceCreated != null) {
				DataServiceCreated(this, new DataServiceCreatedEventArgs(serviceProvider, dataService));
			}
			new CommitOperationHelper(dataService).Attach(e.ObjectSpace);
		}
#pragma warning disable 0067
		[Obsolete("Use 'DataServiceCreated' instead.")]
		public event EventHandler<MiddleTierCreatedEventArgs> MiddleTierCreated;
#pragma warning restore 0067
		public event EventHandler<DataServiceCreatedEventArgs> DataServiceCreated;
	}
#if DebugTest
	public class ObjectSpaceDataServiceOperationEventArgs : DataServiceOperationEventArgs {
#else
	internal class ObjectSpaceDataServiceOperationEventArgs : DataServiceOperationEventArgs {
#endif
		public ObjectSpaceDataServiceOperationEventArgs(IObjectSpace objectSpace) {
			this.ObjectSpace = objectSpace;
		}
		public void RaiseCompleted() {
			OnCompleted();
		}
	}
#if DebugTest
	public class CommitOperationHelper {
#else
	internal class CommitOperationHelper {
#endif
		private FakeApplicationDataService dataService;
		private ObjectSpaceDataServiceOperationEventArgs dataServiceOperation;
		private void objectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e) {
			dataServiceOperation = new ObjectSpaceDataServiceOperationEventArgs((IObjectSpace)sender);
			dataService.RaiseCommitting(DataServiceOperation);
		}
		private void objectSpace_Committed(object sender, EventArgs e) {
			if(dataServiceOperation == null) {
				throw new InvalidOperationException();
			}
			dataServiceOperation.RaiseCompleted();
			dataServiceOperation = null;
		}
		public CommitOperationHelper(FakeApplicationDataService dataService) {
			Guard.ArgumentNotNull(dataService, "dataService");
			this.dataService = dataService;
		}
		public void Attach(IObjectSpace objectSpace) {
			objectSpace.Committing += new EventHandler<System.ComponentModel.CancelEventArgs>(objectSpace_Committing);
			objectSpace.Committed += new EventHandler(objectSpace_Committed);
		}
		public DataServiceOperationEventArgs DataServiceOperation { get { return dataServiceOperation; } }
	}
}
