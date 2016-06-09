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
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Data.Filtering;
using System.Activities;
using System.Activities.Tracking;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Workflow.Server {
	public interface IRunningWorkflowInstanceInfoService {
		IRunningWorkflowInstanceInfo FindWorkflowInstanceInfo(string targetWorkflowUniqueId, object targetObjectKey);
		void CreateRunningWorkflowInstanceInfo(string targetWorkflowName, string targetWorkflowUniqueId, object targetObjectKey, Guid instanceHandle);
		void SetInstanceInfoState(Guid instanceHandle, string state);
	}
	public class RunningWorkflowInstanceInfoService : ServiceBase, IRunningWorkflowInstanceInfoService {
		public const string ActivityInstanceIdPropertyName = "ActivityInstanceId";
		private IObjectSpaceProvider objectSpaceProvider;
		private IObjectSpaceProvider ObjectSpaceProvider {
			get { return objectSpaceProvider ?? GetService<IObjectSpaceProvider>(); }
			set { objectSpaceProvider = value; }
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			ObjectSpaceProvider.TypesInfo.RegisterEntity(RunningWorkflowInstanceInfoType);
		}
		public RunningWorkflowInstanceInfoService(Type runningWorkflowInstanceInfoType) : this(runningWorkflowInstanceInfoType, null) { }
		public RunningWorkflowInstanceInfoService(Type runningWorkflowInstanceInfoType, IObjectSpaceProvider objectSpaceProvider) {
			Guard.ArgumentNotNull(runningWorkflowInstanceInfoType, "runningWorkflowInstanceInfoType");
			Guard.TypeArgumentIs(typeof(IRunningWorkflowInstanceInfo), runningWorkflowInstanceInfoType, "runningWorkflowInstanceInfoType");
			this.RunningWorkflowInstanceInfoType = runningWorkflowInstanceInfoType;
			this.objectSpaceProvider = objectSpaceProvider;
		}
		public IRunningWorkflowInstanceInfo FindWorkflowInstanceInfo(string targetWorkflowUniqueId, object targetObjectKey) {
			using(IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
				return (IRunningWorkflowInstanceInfo)objectSpace.FindObject(RunningWorkflowInstanceInfoType,
					RunningWorkflowInstanceInfoHelper.CreateSelectInstanceCriteria(targetObjectKey, targetWorkflowUniqueId));
			}
		}
		public void CreateRunningWorkflowInstanceInfo(string targetWorkflowName, string targetWorkflowUniqueId, object targetObjectKey, Guid instanceHandle) {
			using(IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
				IRunningWorkflowInstanceInfo info = (IRunningWorkflowInstanceInfo)objectSpace.CreateObject(RunningWorkflowInstanceInfoType);
				info.WorkflowName = targetWorkflowName;
				info.ActivityInstanceId = instanceHandle;
				info.TargetObjectHandle = RunningWorkflowInstanceInfoHelper.MakeTargetObjectHandle(targetObjectKey);
				info.WorkflowUniqueId = targetWorkflowUniqueId;
				info.State = WorkflowInstanceStates.Started;
				objectSpace.CommitChanges();
			}
		}
		public void SetInstanceInfoState(Guid instanceHandle, string state) {
			using(IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
				IRunningWorkflowInstanceInfo instanceInfo = (IRunningWorkflowInstanceInfo)objectSpace.FindObject(RunningWorkflowInstanceInfoType, 
					new BinaryOperator(ActivityInstanceIdPropertyName, instanceHandle));
				if(CustomSetInstanceInfoState != null) {
					CustomSetInstanceInfoState(this, new CustomSetInstanceInfoStateEventArgs(instanceHandle, state, objectSpace, instanceInfo));
				}
				if(instanceInfo != null) {
					instanceInfo.State = state;
					objectSpace.CommitChanges();
				}
			}
		}
		public Type RunningWorkflowInstanceInfoType { get; private set; }
		public event EventHandler<CustomSetInstanceInfoStateEventArgs> CustomSetInstanceInfoState;
	}
	public class RunningWorkflowInstanceInfoService<T> : RunningWorkflowInstanceInfoService where T : IRunningWorkflowInstanceInfo {
		public RunningWorkflowInstanceInfoService() : this(null) { }
		public RunningWorkflowInstanceInfoService(IObjectSpaceProvider objectSpaceProvider) : base(typeof(T), objectSpaceProvider) { }
	}
	public class CustomSetInstanceInfoStateEventArgs : EventArgs {
		public CustomSetInstanceInfoStateEventArgs(Guid instanceHandle, string state, IObjectSpace objectSpace, IRunningWorkflowInstanceInfo instanceInfo) {
			this.InstanceHandle = instanceHandle;
			this.State = state;
			this.ObjectSpace = objectSpace;
			this.FoundInstanceInfo = instanceInfo;
		}
		public Guid InstanceHandle { get; private set; }
		public string State { get; private set; }
		public IObjectSpace ObjectSpace { get; private set; }
		public IRunningWorkflowInstanceInfo FoundInstanceInfo { get; private set; }
	}
}
