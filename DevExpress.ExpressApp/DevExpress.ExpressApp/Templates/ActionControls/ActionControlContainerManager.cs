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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Templates.ActionControls {
	public class ActionControlContainerManager {
		private IModelActionToContainerMapping actionToContainerMapping;
		private bool isActionToContainerMappingInitialized;
		private ActionCollection actions;
		private bool isActionsInitialized;
		private ICollection<ActionBase> GetActions(ActionCollection allActions, IModelActionContainer containerModel) {
			List<ActionBase> actions = new List<ActionBase>();
			foreach(IModelActionLink actionLink in containerModel) {
				ActionBase action = allActions.Find(actionLink.ActionId);
				if(action == null) {
					string message = string.Format("Cannot add a control for the '{0}' Action to the '{1}' Action Control Container because the Action is not found.", actionLink.ActionId, containerModel.Id);
					throw new InvalidOperationException(message);
				}
				actions.Add(action);
			}
			return actions;
		}
		private ICollection<ActionBase> GetActions(ActionCollection allActions, string containerId) {
			List<ActionBase> actions = new List<ActionBase>();
			foreach(ActionBase action in allActions) {
				if(action.Category == containerId) {
					actions.Add(action);
				}
			}
			return actions;
		}
		protected virtual ICollection<ActionBase> GetActions(string containerId) {
			IModelActionContainer containerModel = actionToContainerMapping[containerId];
			if(containerModel != null) {
				return GetActions(actions, containerModel);
			}
			return GetActions(actions, containerId);
		}
		protected virtual void AddActionControl(ActionBase action, IActionControlContainer container) {
			if(CustomAddActionControl != null) {
				CustomAddActionControlEventArgs args = new CustomAddActionControlEventArgs(action, container);
				CustomAddActionControl(this, args);
				if(args.Handled) {
					return;
				}
			}
			if(action is SimpleAction || action is PopupWindowShowAction) {
				container.AddSimpleActionControl(action.Id);
				return;
			}
			if(action is SingleChoiceAction) {
				container.AddSingleChoiceActionControl(action.Id, ((SingleChoiceAction)action).IsHierarchical(), ((SingleChoiceAction)action).ItemType);
				return;
			}
			if(action is ParametrizedAction) {
				container.AddParametrizedActionControl(action.Id, ((ParametrizedAction)action).ValueType);
				return;
			}
			string message = string.Format("Cannot add a control for the '{0}' Action to the '{1}' Action Control Container because the '{2}' Action type is not supported.", action, container.ActionCategory, action.GetType());
			throw new InvalidOperationException(message);
		}
		public ActionControlContainerManager() {
			this.actions = new ActionCollection();
		}
		public void SetActionToContainerMapping(IModelActionToContainerMapping actionToContainerMapping) {
			Guard.ArgumentNotNull(actionToContainerMapping, "actionToContainerMapping");
			if(isActionToContainerMappingInitialized) {
				throw new InvalidOperationException("Action-to-container mapping is already initialized.");
			}
			this.actionToContainerMapping = actionToContainerMapping;
			isActionToContainerMappingInitialized = true;
		}
		public void SetActions(IEnumerable<ActionBase> actions) {
			Guard.ArgumentNotNull(actions, "actions");
			if(isActionsInitialized) {
				throw new InvalidOperationException("Actions are already initialized.");
			}
			this.actions.AddRange(actions);
			this.isActionsInitialized = true;
		}
		public void AddActionControlsToContainer(IActionControlContainer container, string actionCategory) {
			Guard.ArgumentNotNull(container, "container");
			Guard.ArgumentNotNull(actionCategory, "actionCategory");
			if(!isActionToContainerMappingInitialized) {
				throw new InvalidOperationException("Action-to-container mapping is not initialized.");
			}
			if(!isActionsInitialized) {
				throw new InvalidOperationException("Actions are not initialized.");
			}
			ICollection<ActionBase> containerActions = GetActions(actionCategory);
			if(CustomizeContainerActions != null) {
				CustomizeContainerActionsEventArgs args = new CustomizeContainerActionsEventArgs(container, actionCategory, actions, actionToContainerMapping, containerActions);
				CustomizeContainerActions(this, args);
			}
			foreach(ActionBase action in containerActions) {
				AddActionControl(action, container);
			}
		}
		public void Clear() {
			this.actionToContainerMapping = null;
			this.actions.Clear();
			this.isActionToContainerMappingInitialized = false;
			this.isActionsInitialized = false;
		}
		public event EventHandler<CustomAddActionControlEventArgs> CustomAddActionControl;
		public event EventHandler<CustomizeContainerActionsEventArgs> CustomizeContainerActions;
	}
	public class CustomAddActionControlEventArgs : HandledEventArgs {
		private ActionBase action;
		private IActionControlContainer container;
		public CustomAddActionControlEventArgs(ActionBase action, IActionControlContainer container) {
			this.action = action;
			this.container = container;
		}
		public ActionBase Action {
			get { return action; }
		}
		public IActionControlContainer Container {
			get { return container; }
		}
	}
}
