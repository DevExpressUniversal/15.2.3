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
using DevExpress.Data.Filtering;
using System.Xaml;
using System.Xml;
using System.Activities.XamlIntegration;
using System.Activities;
using System.IO;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Workflow.Visualization {
	public abstract class RunningWorkflowInstanceInfoControllerBase<T> : ViewController<T> where T : ObjectView {
		protected static string AllowEditByUserKey = "AllowEditByUser";
		internal static string PatchVersionedXaml(string versionedXaml) {
			string result = versionedXaml;
			int startPos = result.IndexOf(":UserActivity_", 0);
			while(startPos >= 0) {
				int endPos = result.IndexOf(" ", startPos);
				string versionedActivityTypeName = result.Substring(startPos + 1, endPos - startPos).Trim();
				if(versionedActivityTypeName.Length > 50) {
					string activityTypeName = versionedActivityTypeName.Substring(0, 49);
					result = result.Replace(versionedActivityTypeName, activityTypeName);
					endPos = endPos - (versionedActivityTypeName.Length - 50);
				}
				startPos = endPos < result.Length ? result.IndexOf(":UserActivity_", endPos) : -1;
			}
			return result;
		}		
		protected IUserActivityVersionBase GetUserActivityVersion(IRunningWorkflowInstanceInfo info) {
			string workflowUniqueId = info.WorkflowUniqueId.Substring(0, 49);
			int version = int.Parse(info.WorkflowUniqueId.Remove(0, 50));
			return (IUserActivityVersionBase)View.ObjectSpace.FindObject(UserActivityVersionType, new GroupOperator(new BinaryOperator("WorkflowUniqueId", workflowUniqueId), new BinaryOperator("Version", version)));
		}
		protected IWorkflowDefinition GetWorkflowDefinition(IRunningWorkflowInstanceInfo info) {
			return (IWorkflowDefinition)View.ObjectSpace.FindObject(WorkflowDefinitionType, new BinaryOperator("Oid", PersistentWorkflowDefinitionCore.GetWorkflowDefinitionKeyByUniqueId(info.WorkflowUniqueId)));
		}
		protected string GetWorkflowXaml(IRunningWorkflowInstanceInfo info) {
			if(info.WorkflowUniqueId.Length == 49) {
				IWorkflowDefinition definition = GetWorkflowDefinition(info);
				if(definition != null) {
					return definition.Xaml;
				}
			}
			IUserActivityVersionBase userActivityVersion = GetUserActivityVersion(info);
			if(userActivityVersion != null) {
				return PatchVersionedXaml(userActivityVersion.Xaml);
			}
			return null;
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(Frame.Application != null) {
				WorkflowModule module = Frame.Application.Modules.FindModule<WorkflowModule>();
				if(module != null) {
					RunningWorkflowInstanceInfoType = module.RunningWorkflowInstanceInfoType;
					WorkflowDefinitionType = module.WorkflowDefinitionType;
					UserActivityVersionType = module.UserActivityVersionType;
				}
			}
			Active["RunningWorkflowInstanceInfoType"] = (RunningWorkflowInstanceInfoType != null);
			Active["WorkflowDefinitionType"] = (WorkflowDefinitionType != null);			
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.AllowEdit[AllowEditByUserKey] = false;
		}
		public RunningWorkflowInstanceInfoControllerBase() {
			TargetObjectType = typeof(IRunningWorkflowInstanceInfo);
		}
		public Type RunningWorkflowInstanceInfoType { get; set; }
		public Type WorkflowDefinitionType { get; set; }
		public Type UserActivityVersionType { get; set; }
#if DebugTest
		public static string DebugTest_PatchVersionedXaml(string versionedXaml) {
			return PatchVersionedXaml(versionedXaml);
		}
#endif
	}
	public class DisableRunningWorkflowInstanceInfoCreateByUserController : WindowController {
		private static void FilterTypes(CollectTypesEventArgs e) {
			List<Type> typesToRemove = new List<Type>();
			foreach(Type type in e.Types) {
				if(typeof(IRunningWorkflowInstanceInfo).IsAssignableFrom(type)) {
					typesToRemove.Add(type);
				}
			}
			foreach(Type type in typesToRemove) {
				e.Types.Remove(type);
			}
		}
		private void controller_CollectCreatableItemTypes(object sender, CollectTypesEventArgs e) {
			FilterTypes(e);
		}
		private void controller_CollectDescendantTypes(object sender, CollectTypesEventArgs e) {
			FilterTypes(e);
		}
		private void UnsubscribeCollectCreatableItemTypes() {
			if(Window != null) {
				NewObjectViewController controller = Window.GetController<NewObjectViewController>();
				if(controller != null) {
					controller.CollectCreatableItemTypes -= new EventHandler<CollectTypesEventArgs>(controller_CollectCreatableItemTypes);
					controller.CollectDescendantTypes -= new EventHandler<CollectTypesEventArgs>(controller_CollectDescendantTypes);
				}
			}
		}
		protected override void OnWindowChanging(Window window) {
			base.OnWindowChanging(window);
			UnsubscribeCollectCreatableItemTypes();
			NewObjectViewController controller = window.GetController<NewObjectViewController>();
			if(controller != null) {
				controller.CollectCreatableItemTypes += new EventHandler<CollectTypesEventArgs>(controller_CollectCreatableItemTypes);
				controller.CollectDescendantTypes += new EventHandler<CollectTypesEventArgs>(controller_CollectDescendantTypes);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
		}
	}
}
