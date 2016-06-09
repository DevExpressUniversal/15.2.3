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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Linq;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.Persistent.Base;
using DevExpress.Utils.Design;
using DevExpress.Workflow.Store;
namespace DevExpress.ExpressApp.Workflow {
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[ToolboxBitmap(typeof(WorkflowModule), "Resources.Toolbox_Module_Workflow.ico")]
	[Description("Integrates Windows Workflow Foundation in XAF applications.")]
	public sealed class WorkflowModule : ModuleBase {
		public const string ModuleName = "WorkflowModule";
		public const string WorkflowActionCategory = "Workflow";
		private bool isWorkflowDefinitionTypeInitialized = false;
		private bool isWorkflowControlCommandRequestTypeInitialized = false;
		private bool isRunningWorkflowInstanceInfoTypeInitialized = false;
		private bool isUserActivityVersionTypeInitialized = false;
		private bool isStartWorkflowRequestTypeInitialized = false;
		private bool isWorkflowInstanceTypeInitialized = false;
		private bool isWorkflowInstanceKeyTypeInitialized = false;
		private Type workflowDefinitionType;
		private Type workflowControlCommandRequestType;
		private Type runningWorkflowInstanceInfoType;
		private Type userActivityVersionType;
		private Type startWorkflowRequestType;
		private Type workflowInstanceType;
		private Type workflowInstanceKeyType;
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			ModuleTypeList requiredModules = base.GetRequiredModuleTypesCore();
			requiredModules.Add(typeof(ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
			return requiredModules;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			List<Type> result = new List<Type>();
			if(WorkflowDefinitionType != null) {
				result.Add(WorkflowDefinitionType);
			}
			if(WorkflowControlCommandRequestType != null) {
				result.Add(WorkflowControlCommandRequestType);
			}
			if(RunningWorkflowInstanceInfoType != null) {
				result.Add(RunningWorkflowInstanceInfoType);
			}
			if(UserActivityVersionType != null) {
				result.Add(UserActivityVersionType);
			}
			if(StartWorkflowRequestType != null) {
				result.Add(StartWorkflowRequestType);
			}
			if(WorkflowInstanceType != null) {
				result.Add(WorkflowInstanceType);
			}
			if(WorkflowInstanceKeyType != null) {
				result.Add(WorkflowInstanceKeyType);
			}
			return result;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(DevExpress.ExpressApp.Workflow.Controllers.ActivationController),
				typeof(DevExpress.ExpressApp.Workflow.Controllers.RunningWorkflowInstanceInfoViewer),
				typeof(DevExpress.ExpressApp.Workflow.Controllers.StartWorkflowController),
				typeof(DevExpress.ExpressApp.Workflow.Visualization.DisableRunningWorkflowInstanceInfoCreateByUserController),
				typeof(DevExpress.ExpressApp.Workflow.Visualization.RunningInstanceController),
				typeof(DevExpress.ExpressApp.Workflow.Visualization.TrackingVisualizationController)
			};
		}
		public WorkflowModule() {
			ActivateStartWorkflowController = true;
			if(PersistentWorkflowDefinitionCore.ActivityXamlValidator == null) {
				PersistentWorkflowDefinitionCore.ActivityXamlValidator = new ActivityXamlValidator();
			}
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			ModuleUpdater updater = new WorkflowDefinitionsUpdater(objectSpace, versionFromDB, WorkflowDefinitionType, UserActivityVersionType);
			return new ModuleUpdater[] { updater };
		}
		[DXDescription("DevExpress.ExpressApp.Workflow.WorkflowModule,WorkflowDefinitionType")]
		[TypeConverter(typeof(BusinessClassTypeConverter<IWorkflowDefinition>))]
		public Type WorkflowDefinitionType {
			get {
				if(!isWorkflowDefinitionTypeInitialized) {
					workflowDefinitionType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IWorkflowDefinition>(GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService);
					isWorkflowDefinitionTypeInitialized = true;
				}
				return workflowDefinitionType;
			}
			set {
				if(value != null) {
					Guard.TypeArgumentIs(typeof(IWorkflowDefinition), value, "value");
				}
				isWorkflowDefinitionTypeInitialized = true;
				workflowDefinitionType = value;
			}
		}
		[DXDescription("DevExpress.ExpressApp.Workflow.WorkflowModule,WorkflowControlCommandRequestType")]
		[TypeConverter(typeof(BusinessClassTypeConverter<IWorkflowInstanceControlCommandRequest>))]
		public Type WorkflowControlCommandRequestType {
			get {
				if(!isWorkflowControlCommandRequestTypeInitialized) {
					workflowControlCommandRequestType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IWorkflowInstanceControlCommandRequest>(GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService);
					isWorkflowControlCommandRequestTypeInitialized = true;
				}
				return workflowControlCommandRequestType;
			}
			set {
				if(value != null) {
					Guard.TypeArgumentIs(typeof(IWorkflowInstanceControlCommandRequest), value, "value");
				}
				isWorkflowControlCommandRequestTypeInitialized = true;
				workflowControlCommandRequestType = value;
			}
		}
		[DXDescription("DevExpress.ExpressApp.Workflow.WorkflowModule,RunningWorkflowInstanceInfoType")]
		[TypeConverter(typeof(BusinessClassTypeConverter<IRunningWorkflowInstanceInfo>))]
		public Type RunningWorkflowInstanceInfoType {
			get {
				if(!isRunningWorkflowInstanceInfoTypeInitialized) {
					runningWorkflowInstanceInfoType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IRunningWorkflowInstanceInfo>(GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService);
					isRunningWorkflowInstanceInfoTypeInitialized = true;
				}
				return runningWorkflowInstanceInfoType;
			}
			set {
				if(value != null) {
					Guard.TypeArgumentIs(typeof(IRunningWorkflowInstanceInfo), value, "value");
				}
				isRunningWorkflowInstanceInfoTypeInitialized = true;
				runningWorkflowInstanceInfoType = value;
			}
		}
		[DXDescription("DevExpress.ExpressApp.Workflow.WorkflowModule,UserActivityVersionType")]
		[TypeConverter(typeof(BusinessClassTypeConverter<IUserActivityVersionBase>))]
		public Type UserActivityVersionType {
			get {
				if(!isUserActivityVersionTypeInitialized) {
					userActivityVersionType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IUserActivityVersionBase>(GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService);
					isUserActivityVersionTypeInitialized = true;
				}
				return userActivityVersionType;
			}
			set {
				if(value != null) {
					Guard.TypeArgumentIs(typeof(IUserActivityVersionBase), value, "value");
				}
				isUserActivityVersionTypeInitialized = true;
				userActivityVersionType = value;
			}
		}
		[DXDescription("DevExpress.ExpressApp.Workflow.WorkflowModule,StartWorkflowRequestType")]
		[TypeConverter(typeof(BusinessClassTypeConverter<IStartWorkflowRequest>))]
		public Type StartWorkflowRequestType {
			get {
				if(!isStartWorkflowRequestTypeInitialized) {
					startWorkflowRequestType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IStartWorkflowRequest>(GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService);
					isStartWorkflowRequestTypeInitialized = true;
				}
				return startWorkflowRequestType;
			}
			set {
				if(value != null) {
					Guard.TypeArgumentIs(typeof(IStartWorkflowRequest), value, "value");
				}
				isStartWorkflowRequestTypeInitialized = true;
				startWorkflowRequestType = value;
			}
		}
		[DXDescription("DevExpress.ExpressApp.Workflow.WorkflowModule,WorkflowInstanceType")]
		[TypeConverter(typeof(BusinessClassTypeConverter<IWorkflowInstance>))]
		public Type WorkflowInstanceType {
			get {
				if(!isWorkflowInstanceTypeInitialized) {
					workflowInstanceType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IWorkflowInstance>(GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService);
					isWorkflowInstanceTypeInitialized = true;
				}
				return workflowInstanceType;
			}
			set {
				if(value != null) {
					Guard.TypeArgumentIs(typeof(IWorkflowInstance), value, "value");
				}
				isWorkflowInstanceTypeInitialized = true;
				workflowInstanceType = value;
			}
		}
		[DXDescription("DevExpress.ExpressApp.Workflow.WorkflowModule,WorkflowInstanceKeyType")]
		[TypeConverter(typeof(BusinessClassTypeConverter<IInstanceKey>))]
		public Type WorkflowInstanceKeyType {
			get {
				if(!isWorkflowInstanceKeyTypeInitialized) {
					workflowInstanceKeyType = WorkflowDataTypesHepler.FindDefaultWorkflowDataType<IInstanceKey>(GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService);
					isWorkflowInstanceKeyTypeInitialized = true;
				}
				return workflowInstanceKeyType;
			}
			set {
				if(value != null) {
					Guard.TypeArgumentIs(typeof(IInstanceKey), value, "value");
				}
				isWorkflowInstanceKeyTypeInitialized = true;
				workflowInstanceKeyType = value;
			}
		}
		[DefaultValue(true)]
		public bool ActivateStartWorkflowController { get; set; }
		public override string Name {
			get {
				return ModuleName;
			}
		}
	}
}
