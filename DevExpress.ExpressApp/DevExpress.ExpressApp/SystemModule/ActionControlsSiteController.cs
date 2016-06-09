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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Templates.ActionControls.Binding;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	public class ActionControlsSiteController : Controller, IModelExtender {
		private static readonly string UnspecifiedCategoryId = PredefinedCategory.Unspecified.ToString();
		private void containerManager_CustomizeContainerActions(object sender, CustomizeContainerActionsEventArgs e) {
			if(CustomizeContainerActions != null) {
				CustomizeContainerActions(this, e);
			}
		}
		private void containerManager_CustomAddActionControl(object sender, CustomAddActionControlEventArgs e) {
			if(CustomAddActionControlToContainer != null) {
				CustomAddActionControlToContainer(this, e);
			}
		}
		private void binder_CustomBind(object sender, CustomBindEventArgs e) {
			if(CustomBindActionControlToAction != null) {
				CustomBindActionControlToAction(this, e);
			}
		}
		private void Frame_TemplateChanged(object sender, EventArgs e) {
			IActionControlsSite site = Frame.Template as IActionControlsSite;
			if(site != null) {
				IEnumerable<ActionBase> actions = CollectActions();
				AddActionControlsToContainers(site, actions, GetActionToContainerMapping());
				IEnumerable<IActionControl> actionControls = GetActionControls(site);
				BindActionControlsToActions(actionControls, actions);
				foreach(IActionControl actionControl in actionControls) {
					if(CustomizeActionControl != null) {
						CustomizeActionControl(this, new ActionControlEventArgs(actionControl));
					}
				}
			}
		}
		private ActionCollection CollectActions() {
			ActionCollection actions = new ActionCollection();
			foreach(Controller controller in Frame.Controllers) {
				actions.AddRange(controller.Actions);
			}
			return actions;
		}
		private void AddActionControlsToContainers(IActionControlsSite site, IEnumerable<ActionBase> actions, IModelActionToContainerMapping actionToContainerMapping) {
			ActionControlContainerManager containerManager = new ActionControlContainerManager();
			if(CustomizeContainerActions != null) {
				containerManager.CustomizeContainerActions += new EventHandler<CustomizeContainerActionsEventArgs>(containerManager_CustomizeContainerActions);
			}
			if(CustomAddActionControlToContainer != null) {
				containerManager.CustomAddActionControl += new EventHandler<CustomAddActionControlEventArgs>(containerManager_CustomAddActionControl);
			}
			containerManager.SetActions(actions);
			containerManager.SetActionToContainerMapping(actionToContainerMapping);
			foreach(IActionControlContainer container in site.ActionContainers) {
				containerManager.AddActionControlsToContainer(container, container.ActionCategory);
			}
			if(site.DefaultContainer != null) {
				containerManager.AddActionControlsToContainer(site.DefaultContainer, UnspecifiedCategoryId);
			}
		}
		private void BindActionControlsToActions(IEnumerable<IActionControl> actionControls, IEnumerable<ActionBase> actions) {
			ActionToControlBinder binder = new ActionToControlBinder();
			if(CustomBindActionControlToAction != null) {
				binder.CustomBind += new EventHandler<CustomBindEventArgs>(binder_CustomBind);
			}
			binder.SetActions(actions);
			binder.RegisterActionControls(actionControls);
		}
		private IEnumerable<IActionControl> GetActionControls(IActionControlsSite site) {
			HashSet<IActionControl> actionControls = new HashSet<IActionControl>();
			actionControls.UnionWith(site.ActionControls);
			foreach(IActionControlContainer container in site.ActionContainers) {
				actionControls.UnionWith(container.GetActionControls());
			}
			if(site.DefaultContainer != null) {
				actionControls.UnionWith(site.DefaultContainer.GetActionControls());
			}
			return actionControls;
		}
		protected virtual IModelActionToContainerMapping GetActionToContainerMapping() {
			if(Application != null && Application.Model != null && Application.Model.ActionDesign is IModelActionDesignContainerMapping) {
				return ((IModelActionDesignContainerMapping)Application.Model.ActionDesign).ActionToContainerMapping;
			}
			return null;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
		}
		protected override void OnDeactivated() {
			Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
			base.OnDeactivated();
		}
		public event EventHandler<CustomizeContainerActionsEventArgs> CustomizeContainerActions;
		public event EventHandler<CustomAddActionControlEventArgs> CustomAddActionControlToContainer;
		public event EventHandler<CustomBindEventArgs> CustomBindActionControlToAction;
		public event EventHandler<ActionControlEventArgs> CustomizeActionControl;
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelActionDesign, IModelActionDesignContainerMapping>();
		}
	}
	public class CustomizeContainerActionsEventArgs : EventArgs {
		private object container;
		private string category;
		private ActionCollection allActions;
		private IModelActionToContainerMapping actionToContainerMapping;
		private ICollection<ActionBase> containerActions;
		public CustomizeContainerActionsEventArgs(object container, string category, ActionCollection allActions, IModelActionToContainerMapping actionToContainerMapping, ICollection<ActionBase> containerActions) {
			this.container = container;
			this.category = category;
			this.allActions = allActions;
			this.actionToContainerMapping = actionToContainerMapping;
			this.containerActions = containerActions;
		}
		public object Container {
			get { return container; }
		}
		public string Category {
			get { return category; }
		}
		public ActionCollection AllActions {
			get { return allActions; }
		}
		public IModelActionToContainerMapping ActionToContainerMapping {
			get { return actionToContainerMapping; }
		}
		public ICollection<ActionBase> ContainerActions {
			get { return containerActions; }
		}
	}
	public class ActionControlEventArgs : EventArgs {
		private IActionControl actionControl;
		public ActionControlEventArgs(IActionControl actionControl) {
			this.actionControl = actionControl;
		}
		public IActionControl ActionControl {
			get { return actionControl; }
		}
	}
}
