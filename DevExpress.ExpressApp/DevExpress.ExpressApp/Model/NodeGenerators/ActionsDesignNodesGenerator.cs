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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	public class ModelControllersNodesGenerator : ModelNodesGeneratorBase {
		public const string ControllerPropertyName = "Controller";
		protected override void GenerateNodesCore(ModelNode node) {
			IModelController result;
			foreach (Controller controller in ((IModelSources)node.Application).Controllers) {
				if (controller is ViewController) {
					result = node.AddNode<IModelViewController>(controller.Name);
				} else if (controller is WindowController) {
					result = node.AddNode<IModelWindowController>(controller.Name);
				} else {
					result = node.AddNode<IModelController>(controller.Name);
				}
				result.SetValue<Controller>(ControllerPropertyName, controller);
			}
		}
	}
	public class ModelActionsNodesGenerator : ModelNodesGeneratorBase {
		public const string ActionPropertyName = "Action";
		protected override void GenerateNodesCore(ModelNode node) {
			Dictionary<Type, Controller> controllerByType = new Dictionary<Type, Controller>();
			foreach (Controller controller in ((IModelSources)node.Application).Controllers) {
				controllerByType.Add(controller.GetType(), controller);
			}
			foreach (Controller controller in ((IModelSources)node.Application).Controllers) {
				IModelController modelController = node.Application.ActionDesign.Controllers[controller.Name];
				IList<ActionBase> ownedActions = GetOwnActions(controller, controllerByType);
				foreach(ActionBase action in ownedActions) {
					IModelAction modelAction = node.AddNode<IModelAction>(action.Id);
					if (action is ChoiceActionBase) {
						modelAction.AddNode<IModelChoiceActionItems>();
					}
					SetAction(modelAction, action);
				}
				modelController = node.Application.ActionDesign.Controllers[controller.Name];
			}
			IModelAction newActionInfo = node.Application.ActionDesign.Actions["New"];
			if(newActionInfo != null) {
				IModelDisableReasons disableReasonsInfo = newActionInfo.DisableReasons;
				if(disableReasonsInfo == null) {
					disableReasonsInfo = newActionInfo.AddNode<IModelDisableReasons>("DisableReasons");
				}
				if(disableReasonsInfo["MasterObjectIsNew"] == null) {
					IModelReason reason = disableReasonsInfo.AddNode<IModelReason>("MasterObjectIsNew");
					reason.Caption = "The edited object is new.";
				}
			}
		}
		private IList<ActionBase> GetOwnActions(Controller controller, Dictionary<Type, Controller> controllerByType) {
			IList<ActionBase> result = new List<ActionBase>(controller.Actions);
			Type baseControllerType = controller.GetType().BaseType;
			while(baseControllerType != null && typeof(Controller).IsAssignableFrom(baseControllerType)) {
				Controller baseController;
				if(controllerByType.TryGetValue(baseControllerType, out baseController)) {
					for(int i = result.Count - 1; i >= 0; i--) {
						ActionBase action = result[i];
						if(baseController.Actions[action.Id] != null) {
							result.Remove(action);
						}
					}
				}
				baseControllerType = baseControllerType.BaseType;
			}
			return result;
		}
		public static void SetAction(IModelAction modelAction, ActionBase action) {
			modelAction.SetValue<ActionBase>(ActionPropertyName, action);
			modelAction.Caption = action.Caption;
		}
	}
	public class ModelChoiceActionItemsNodesGenerator : ModelNodesGeneratorBase {
		public const string ChoiceActionItemPropertyName = "ChoiceActionItem";
		protected override void GenerateNodesCore(ModelNode node) {
			ChoiceActionItemCollection items = null;
			if(node.Parent is IModelAction) {
				items = (node.Parent.GetValue<ActionBase>(ModelActionsNodesGenerator.ActionPropertyName) as ChoiceActionBase).Items;
			}
			if(node.Parent is IModelChoiceActionItem) {
				items = node.Parent.GetValue<ChoiceActionItem>(ChoiceActionItemPropertyName).Items;
			}
			foreach(ChoiceActionItem item in items) {
				IModelChoiceActionItem modelItem = node.AddNode<IModelChoiceActionItem>(item.Id);
				modelItem.Caption = item.Caption;
				modelItem.ImageName = item.ImageName;
				modelItem.Shortcut = item.Shortcut;
				modelItem.SetValue<ChoiceActionItem>(ChoiceActionItemPropertyName, item);
				if(item.Items != null) {
					modelItem.AddNode<IModelChoiceActionItems>();
				}
			}
		}
	}
	public class ModelControllerActionsNodesGenerator : ModelNodesGeneratorBase {
		Dictionary<string, List<string>> controllerActions = new Dictionary<string, List<string>>();
		int allActionsCount = 0;
		int allControllersCount = 0;
		protected override void GenerateNodesCore(ModelNode node) {
			CreateControllerActionsCache(node);
			List<string> actionsId;
			IModelController modelController = (IModelController)node.Parent;
			if(controllerActions.TryGetValue(modelController.Name, out actionsId)) {
				foreach(string actionId in actionsId) {
					node.AddNode<IModelActionLink>(actionId);
				}
			}
		}
		private void CreateControllerActionsCache(ModelNode node) {
			int actionsCount = node.Application.ActionDesign.Actions.Count;
			int controllersCount = node.Application.ActionDesign.Controllers.Count;
			if(allActionsCount != actionsCount || allControllersCount != controllersCount) {
				controllerActions.Clear();
				allActionsCount = actionsCount;
				allControllersCount = controllersCount;
				List<string> actionsId;
				foreach(IModelAction modelAction in node.Application.ActionDesign.Actions) {
					if(!controllerActions.TryGetValue(modelAction.Controller.Name, out actionsId)) {
						actionsId = new List<string>();
						controllerActions[modelAction.Controller.Name] = actionsId;
					}
					actionsId.Add(modelAction.Id);
				}
			}
		}
	}
}
