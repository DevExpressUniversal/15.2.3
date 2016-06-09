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
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model.DomainLogics {
	[DomainLogic(typeof(IModelViewController))]
	public static class ModelViewControllerLogic {
		private static T GetViewControllerProperty<T>(IModelController node, ModelControllerLogic.GetProperty<T, ViewController> getProperty) {
			return ModelControllerLogic.GetControllerProperty<T, ViewController>(node, getProperty);
		}
		public static DevExpress.ExpressApp.ViewType Get_TargetViewType(IModelViewController node) {
			return GetViewControllerProperty<DevExpress.ExpressApp.ViewType>(node, delegate(ViewController c) { return c.TargetViewType; });
		}
		public static DevExpress.ExpressApp.Nesting Get_TargetViewNesting(IModelViewController node) {
			return GetViewControllerProperty<DevExpress.ExpressApp.Nesting>(node, delegate(ViewController c) { return c.TargetViewNesting; });
		}
		public static string Get_TargetObjectType(IModelViewController node) {
			return GetViewControllerProperty<string>(node, delegate(ViewController c) { return c.TargetObjectType == null ? string.Empty : c.TargetObjectType.FullName; });
		}
		public static string Get_TargetViewId(IModelViewController node) {
			return GetViewControllerProperty<string>(node, delegate(ViewController c) { return c.TargetViewId; });
		}
	}
	[DomainLogic(typeof(IModelWindowController))]
	public static class ModelWindowControllerLogic {
		public static WindowType Get_TargetWindowType(IModelController node) {
			return ModelControllerLogic.GetControllerProperty<WindowType, WindowController>(node, delegate(WindowController c) { return c.TargetWindowType; });
		}
	}
	[DomainLogic(typeof(IModelController))]
	public static class ModelControllerLogic {
		internal delegate T GetProperty<T, C>(C controller) where C : Controller;
		internal static T GetControllerProperty<T, C>(IModelController node, GetProperty<T, C> getProperty) where C : Controller {
			C controller = node.GetValue<Controller>(ModelControllersNodesGenerator.ControllerPropertyName) as C;
			if (controller != null) {
				return getProperty(controller);
			} else {
				return default(T);
			}
		}
		public static IModelController Get_BaseController(IModelController node) {
			string baseControllerName = GetControllerProperty<string, Controller>(node,
				delegate(Controller c) { return c.GetType().BaseType.FullName; });
			return node.Application.ActionDesign.Controllers[baseControllerName];
		}
	}
	[DomainLogic(typeof(IModelAction))]
	public static class ModelActionLogic {
		delegate T GetProperty<T, A>(A action) where A : ActionBase;
		private static T GetActionProperty<T, A>(IModelAction node, GetProperty<T, A> getProperty) where A : ActionBase {
			A action = GetAction(node) as A;
			if (action != null) {
				return getProperty(action);
			} else {
				return default(T);
			}
		}
		private static ActionBase GetAction(IModelAction modelAction) {
			Guard.ArgumentNotNull(modelAction, "modelAction");
			return modelAction.GetValue<ActionBase>(ModelActionsNodesGenerator.ActionPropertyName);
		}
		private static T GetActionBaseProperty<T>(IModelAction node, GetProperty<T, ActionBase> getProperty) {
			return GetActionProperty<T, ActionBase>(node, getProperty);
		}
		private static T GetChoiceActionProperty<T>(IModelAction node, GetProperty<T, ChoiceActionBase> getProperty) {
			return GetActionProperty<T, ChoiceActionBase>(node, getProperty);
		}
		public static bool Get_QuickAccess(IModelAction node) {
			return GetActionBaseProperty<bool>(node, delegate(ActionBase action) { return action.QuickAccess; });
		}
		public static string Get_ImageName(IModelAction node) {
			return GetActionBaseProperty<string>(node, delegate(ActionBase action) { return action.ImageName; });
		}
		public static DevExpress.ExpressApp.Actions.SelectionDependencyType Get_SelectionDependencyType(IModelAction node) {
			return GetActionBaseProperty<DevExpress.ExpressApp.Actions.SelectionDependencyType>(node, delegate(ActionBase action) { return action.SelectionDependencyType; });
		}
		public static string Get_Category(IModelAction node) {
			return GetActionBaseProperty<string>(node, delegate(ActionBase action) { return action.Category; });
		}
		public static string Get_ConfirmationMessage(IModelAction node) {
			return GetActionBaseProperty<string>(node, delegate(ActionBase action) { return action.ConfirmationMessage; });
		}
		public static string Get_Shortcut(IModelAction node) {
			return GetActionBaseProperty<string>(node, delegate(ActionBase action) { return action.Shortcut; });
		}
		public static string Get_ToolTip(IModelAction node) {
			string result = GetActionBaseProperty<string>(node, delegate(ActionBase action) { return action.ToolTip; });
			if(string.IsNullOrEmpty(result)) {
				result = node.Caption;
			}
			return result;
		}
		public static ViewType Get_TargetViewType(IModelAction node) {
			return GetActionBaseProperty<ViewType>(node, delegate(ActionBase action) { return action.TargetViewType; });
		}
		public static Nesting Get_TargetViewNesting(IModelAction node) {
			return GetActionBaseProperty<Nesting>(node, delegate(ActionBase action) { return action.TargetViewNesting; });
		}
		public static string Get_TargetObjectType(IModelAction node) {
			Type targetObjectType = GetActionBaseProperty<Type>(node, delegate(ActionBase action) { return action.TargetObjectType; });
			if (targetObjectType != null) {
				return targetObjectType.FullName;
			} else {
				return null;
			}
		}
		public static string Get_TargetViewId(IModelAction node) {
			return GetActionBaseProperty<string>(node, delegate(ActionBase action) { return action.TargetViewId; });
		}
		public static string Get_TargetObjectsCriteria(IModelAction node) {
			return GetActionBaseProperty<string>(node, delegate(ActionBase action) { return action.TargetObjectsCriteria; });
		}
		public static TargetObjectsCriteriaMode Get_TargetObjectsCriteriaMode(IModelAction node) {
			return GetActionBaseProperty<TargetObjectsCriteriaMode>(node, delegate(ActionBase action) { return action.TargetObjectsCriteriaMode; });
		}
		public static string Get_ShortCaption(IModelAction node) {
			return GetActionBaseProperty<string>(node, delegate(ActionBase action) {
				if (action is ParametrizedAction) {
					string shortCaption = ((ParametrizedAction)action).ShortCaption;
					if (!string.IsNullOrEmpty(shortCaption)) {
						return shortCaption;
					}
				}
				return node.Caption;
			});
		}
		public static string Get_NullValuePrompt(IModelAction node) {
			return GetActionBaseProperty<string>(node, delegate(ActionBase action) {
				if(action is ParametrizedAction) {
					return ((ParametrizedAction)action).NullValuePrompt;
				}
				return string.Empty;
			});
		}
		public static IModelController Get_Controller(IModelAction node) {
			Controller controller = GetActionBaseProperty<Controller>(node, delegate(ActionBase action) { return action.Controller; });
			if (controller != null) {
				return node.Application.ActionDesign.Controllers[controller.Name];
			}
			return null;
		}
		public static DevExpress.ExpressApp.Templates.ActionItemPaintStyle Get_PaintStyle(IModelAction node) {
			return GetActionBaseProperty<ActionItemPaintStyle>(node, delegate(ActionBase action) { return action.PaintStyle; });
		}
		public static bool Get_ShowItemsOnClick(IModelAction node) {
			return GetChoiceActionProperty<bool>(node, delegate(ChoiceActionBase choiceActionBase) { return choiceActionBase.ShowItemsOnClick; });
		}
		public static DefaultItemMode Get_DefaultItemMode(IModelAction node) {
			return GetActionProperty<DefaultItemMode, ChoiceActionBase>(node,
				delegate(ChoiceActionBase choiceActionBase) { return choiceActionBase.DefaultItemMode; });
		}
		public static ImageMode Get_ImageMode(IModelAction node) {
			return GetChoiceActionProperty<ImageMode>(node, delegate(ChoiceActionBase choiceActionBase) { return choiceActionBase.ImageMode; });
		}
	}
	[DomainLogic(typeof(IModelActions))]
	public static class ModelActionsLogic {
		public static void AfterConstruction(IModelActions actions) {
			((ModelNode)actions).NodeInfo.ChildNodesComparison = SortChildNodesHelper.DoSortNodesByIndex;
		}
	}
}
