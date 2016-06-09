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
using DevExpress.Workflow;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Workflow.CommonServices;
using System.Activities.Tracking;
using DevExpress.Workflow.Utils;
using System.ServiceModel.Description;
using System.ServiceModel.Activities.Description;
using DevExpress.Workflow.Store;
using System.ServiceModel.Activities;
namespace DevExpress.ExpressApp.Workflow.Server {
	public class CustomizeHostEventArgs : EventArgs {
		public CustomizeHostEventArgs(WorkflowServiceHost host) {
			this.Host = host;
		}
		public WorkflowInstanceStoreBehavior WorkflowInstanceStoreBehavior { get; set; }
		public WorkflowIdleBehavior WorkflowIdleBehavior { get; set; }
		public WorkflowServiceHost Host { get; private set; }
	}
	public class WorkflowServer : IDisposable {
		public static bool ExtendTraceLog = true;
		private List<object> sharedWorkflowExtensions = new List<object>();
		private void ServiceProvider_CustomHandleException(object sender, CustomHandleServiceExceptionEventArgs e) {
			if(CustomHandleException != null) {
				CustomHandleException(this, e);
			}
		}
		private void HostManager_CustomHandleException(object sender, DevExpress.ExpressApp.MiddleTier.CustomHandleExceptionEventArgs e) {
			CustomHandleServiceExceptionEventArgs args = new CustomHandleServiceExceptionEventArgs(HostManager, e.Exception);
			if(CustomHandleException != null) {
				CustomHandleException(this, args);
			}
			e.Handled = args.Handled;
		}
		private static void Tracing_NeedContextInformation(object sender, Persistent.Base.NeedContextInformationEventArgs e) {
			if(ExtendTraceLog) {
				e.ContextInformation = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
			}
		}
		private void OnCustomizeHost(CustomizeHostEventArgs args) {
			if(CustomizeHost != null) {
				CustomizeHost(this, args);
			}
		}
		protected virtual WorkflowHostManager CreateWorkflowHostManager(string baseUri) {
			return new WorkflowHostManager(baseUri);
		}
		static WorkflowServer() {
			DevExpress.Persistent.Base.Tracing.NeedContextInformation += new EventHandler<Persistent.Base.NeedContextInformationEventArgs>(Tracing_NeedContextInformation);
		}
		public WorkflowServer(string baseUri) {
			if(PersistentWorkflowDefinitionCore.ActivityXamlValidator == null) {
				PersistentWorkflowDefinitionCore.ActivityXamlValidator = new ActivityXamlValidator();
			}
			ServiceProvider = new ServiceProvider();
			ServiceProvider.CustomHandleException += new EventHandler<CustomHandleServiceExceptionEventArgs>(ServiceProvider_CustomHandleException);
			HostManager = CreateWorkflowHostManager(baseUri); 
			HostManager.CustomHandleException += new EventHandler<CustomHandleExceptionEventArgs>(HostManager_CustomHandleException);
			ServiceProvider.AddService(HostManager);
		}
		public WorkflowServer(string baseUri, IObjectSpaceProvider servicesObjectSpaceProvider, IObjectSpaceProvider activitiesObjectSpaceProvider)
			: this(baseUri) {
			ServicesObjectSpaceProvider = servicesObjectSpaceProvider;
			ActivitiesObjectSpaceProvider = activitiesObjectSpaceProvider;
			WorkflowInstanceType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IWorkflowInstance>();
			InstanceKeyType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IInstanceKey>();
			Type workflowDefinitionType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IWorkflowDefinition>();
			if(workflowDefinitionType != null) {
				WorkflowDefinitionProvider = new WorkflowDefinitionProvider(workflowDefinitionType);
			}
			HostManagerActivityProvider = new HostManagerActivityProvider();			
			StartWorkflowConditionsProvider = new StartWorkflowConditionsProvider();
			StartWorkflowListenerService = new StartWorkflowListenerService(TimeSpan.FromSeconds(15));
			Type startWorkflowRequestType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IStartWorkflowRequest>();
			if(startWorkflowRequestType != null) {
				StartWorkflowByRequestService = new StartWorkflowByRequestService(startWorkflowRequestType, TimeSpan.FromSeconds(15));
			}
			WorkflowServerStartWorkflowService = new WorkflowServerStartWorkflowService();
			Type runningWorkflowInstanceInfoType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IRunningWorkflowInstanceInfo>();
			if(runningWorkflowInstanceInfoType != null) {
				RunningWorkflowInstanceInfoService = new RunningWorkflowInstanceInfoService(runningWorkflowInstanceInfoType);
			}
			WorkflowRunningInstanceUpdater = new WorkflowRunningInstanceUpdater();
			Type trackingRecordType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<ITrackingRecord>();
			if(trackingRecordType != null) {
				TrackingParticipant = new XafTrackingParticipant(ServicesObjectSpaceProvider, trackingRecordType);
			}
			RefreshWorkflowDefinitionsService = new RefreshWorkflowDefinitionsService(TimeSpan.FromMinutes(15));
			Type workflowInstanceControlCommandRequestType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IWorkflowInstanceControlCommandRequest>();
			if(workflowInstanceControlCommandRequestType != null) {
				HostCommandRequestProcessor = new HostCommandRequestProcessor(workflowInstanceControlCommandRequestType, TimeSpan.FromSeconds(5));
			}
			HostManager.HostCreated += new EventHandler<WorkflowHostEventArgs>(HostManager_HostCreated);
		}
		private void HostManager_HostCreated(object sender, WorkflowHostEventArgs e) {
			WorkflowInstanceStoreBehavior instanceStoreBehavior = new WorkflowInstanceStoreBehavior(WorkflowInstanceType, InstanceKeyType, ServicesObjectSpaceProvider);
			CustomizeHostEventArgs args = new CustomizeHostEventArgs(e.WorkflowHost.Host);
			args.WorkflowIdleBehavior = new WorkflowIdleBehavior();
			args.WorkflowInstanceStoreBehavior = instanceStoreBehavior;
			OnCustomizeHost(args);
			if(args.WorkflowInstanceStoreBehavior != null) {
				e.WorkflowHost.Host.Description.Behaviors.Add(args.WorkflowInstanceStoreBehavior);
			}
			if(args.WorkflowIdleBehavior != null) {
				e.WorkflowHost.Host.Description.Behaviors.Add(args.WorkflowIdleBehavior);
			}		
		}
		internal void InitializeDefaults() {
			if(ServicesObjectSpaceProvider != null) {
				ServiceProvider.AddService(ServicesObjectSpaceProvider);
			}
			if(WorkflowDefinitionProvider != null) {
				ServiceProvider.AddService(WorkflowDefinitionProvider);
			}
			if(HostManagerActivityProvider != null) {
				ServiceProvider.AddService(HostManagerActivityProvider);
			}
			if(StartWorkflowConditionsProvider != null) {
				ServiceProvider.AddService(StartWorkflowConditionsProvider);
			}
			if(StartWorkflowListenerService != null) {
				ServiceProvider.AddService(StartWorkflowListenerService);
			}
			if(StartWorkflowByRequestService != null) {
				ServiceProvider.AddService(StartWorkflowByRequestService);
			}
			if(WorkflowServerStartWorkflowService != null) {
				ServiceProvider.AddService(WorkflowServerStartWorkflowService);
			}
			if(RunningWorkflowInstanceInfoService != null) {
				ServiceProvider.AddService(RunningWorkflowInstanceInfoService);
			}
			if(WorkflowRunningInstanceUpdater != null) {
				ServiceProvider.AddService(WorkflowRunningInstanceUpdater);
			}
			HostManager.HostCreated += delegate(object sender, WorkflowHostEventArgs e) {
				if(ActivitiesObjectSpaceProvider != null) {
					e.WorkflowHost.Host.WorkflowExtensions.Add(ActivitiesObjectSpaceProvider);
				}
				if(TrackingParticipant != null) {
					e.WorkflowHost.Host.WorkflowExtensions.Add(TrackingParticipant);
				}
			};
			if(RefreshWorkflowDefinitionsService != null) {
				ServiceProvider.AddService(RefreshWorkflowDefinitionsService);
			}
			if(HostCommandRequestProcessor != null) {
				ServiceProvider.AddService(HostCommandRequestProcessor);
			}
		}
		public void Start() {
			InitializeDefaults();
			HostManager.OpenHosts();
		}
		public void Stop() {
			if(HostManager != null) {
				HostManager.CloseHosts();
			}
		}
		public void Dispose() {
			Stop();
			if(HostManager != null) {
				HostManager.HostCreated -= new EventHandler<WorkflowHostEventArgs>(HostManager_HostCreated);
			}
			HostManager = null;
			if(ServiceProvider != null) {
				foreach(object service in ServiceProvider.Services) {
					if(service is IDisposable) {
						((IDisposable)service).Dispose();
					}
				}
			}
		}
		public WorkflowHostManager HostManager { get; private set; }
		public ServiceProvider ServiceProvider { get; protected set; }
		public IWorkflowDefinitionProvider WorkflowDefinitionProvider { get; set; }
		public object HostManagerActivityProvider { get; set; }
		public IStartWorkflowConditionProvider StartWorkflowConditionsProvider { get; set; }
		public StartWorkflowListenerService StartWorkflowListenerService { get; set; }
		public StartWorkflowByRequestService StartWorkflowByRequestService { get; set; }
		public IStartWorkflowService WorkflowServerStartWorkflowService { get; set; }
		public IRunningWorkflowInstanceInfoService RunningWorkflowInstanceInfoService { get; set; }
		public WorkflowRunningInstanceUpdater WorkflowRunningInstanceUpdater { get; set; }
		[Obsolete("Use the 'TrackingParticipant' property instead.")]
		public XpoTrackingParticipant XpoTrackingParticipant { get; set; }
		public TrackingParticipant TrackingParticipant { get; set; }
		public RefreshWorkflowDefinitionsService RefreshWorkflowDefinitionsService { get; set; }
		public HostCommandRequestProcessor HostCommandRequestProcessor { get; set; }
		public IObjectSpaceProvider ActivitiesObjectSpaceProvider { get; set; }
		public IObjectSpaceProvider ServicesObjectSpaceProvider { get; set; }
		public Type WorkflowInstanceType { get; set; }
		public Type InstanceKeyType { get; set; }
		public event EventHandler<CustomHandleServiceExceptionEventArgs> CustomHandleException;
		public event EventHandler<CustomizeHostEventArgs> CustomizeHost;
#if DebugTest
		public void DebugTest_InitializeDefaults() {
			InitializeDefaults();
		}
#endif
	}
	public abstract class WorkflowServerService : ServiceBase {
		public WorkflowHostManager HostManager {
			get { return GetService<WorkflowHostManager>(); }
		}
	}
}
