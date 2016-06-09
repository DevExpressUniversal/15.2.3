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

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Workflow.Win {
	public class NavigationItemsWorkflowsUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator> {
		public const string WorkflowGroupName = "Workflow";
		public const string WorkflowDefinitionNavigationItemId = "Workflow Definition";
		private ApplicationModulesManager modulesManager;
		public static IModelNavigationItem GenerateWorkflowNavigationItem(IModelApplication modelApplication, WorkflowModule workflowModule) {
			if(modelApplication == null || workflowModule == null) {
				return null;
			}
			IModelClass modelClass = modelApplication.BOModel.GetClass(workflowModule.WorkflowDefinitionType);
			if(modelClass == null) {
				return null;
			}
			string defaultListViewId = modelClass.DefaultListView != null ? modelClass.DefaultListView.Id : string.Empty;
			return ShowNavigationItemController.GenerateNavigationItem(modelApplication, WorkflowGroupName, WorkflowDefinitionNavigationItemId, "", defaultListViewId, null);
		}
		public override void UpdateNode(ModelNode node) {
			IModelApplication modelApplication = node.Application;
			if(modelApplication == null || modulesManager == null) {
				return;
			}
			WorkflowModule workflowModule = modulesManager.Modules.FindModule<WorkflowModule>();
			if(workflowModule == null) {
				return;
			}
			GenerateWorkflowNavigationItem(modelApplication, workflowModule);
		}
		public NavigationItemsWorkflowsUpdater(ApplicationModulesManager modulesManager) {
			this.modulesManager = modulesManager;
		}
	}
}
