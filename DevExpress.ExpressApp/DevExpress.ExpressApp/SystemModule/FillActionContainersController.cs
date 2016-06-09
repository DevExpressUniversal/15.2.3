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
using System.Linq;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	public interface IModelActionDesignContainerMapping {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionDesignContainerMappingActionToContainerMapping")]
#endif
		IModelActionToContainerMapping ActionToContainerMapping { get; }
	}
	[ModelNodesGenerator(typeof(ModelActionContainersGenerator))]
	[ImageName("ModelEditor_Actions_ActionToContainerMapping")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelActionToContainerMapping")]
#endif
	public interface IModelActionToContainerMapping : IModelNode, IModelList<IModelActionContainer> {
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelActionContainer")]
#endif
	public interface IModelActionContainer : IModelNode, IModelList<IModelActionLink> {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionContainerId")]
#endif
		string Id { get; set; }
	}
	[KeyProperty("ActionId")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelActionLink")]
#endif
	public interface IModelActionLink : IModelNode, IModelIndexedNode {
		[Browsable(false)]
		string ActionId { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionLinkAction"),
#endif
 DataSourceProperty("Application.ActionDesign.Actions")]
		[Category("Data")]
		IModelAction Action { get; set; }
	}
	[DomainLogic(typeof(IModelActionLink))]
	public static class IModelActionLinkLogic {
		public static IModelAction Get_Action(IModelActionLink modelAction) {
			return modelAction.Application.ActionDesign.Actions[modelAction.ActionId];
		}
		public static void Set_Action(IModelActionLink modelAction, IModelAction value) {
			modelAction.ActionId = value.Id;
		}
	}
	[DomainLogic(typeof(IModelActionContainer))]
	public static class ModelActionContainerLogic {
		public static void AfterConstruction(IModelActionContainer actionContainer) {
			((ModelNode)actionContainer).NodeInfo.ChildNodesComparison = SortChildNodesHelper.DoSortNodesByIndex;
		}
	}
	public class CustomFillContainersEventArgs : HandledEventArgs {
		private IFrameTemplate template;
		private IList<Controller> controllers;
		private IModelActionToContainerMapping containersInfo;
		public CustomFillContainersEventArgs(IFrameTemplate template, IList<Controller> controllers, IModelActionToContainerMapping containersInfo) {
			this.template = template;
			this.controllers = controllers;
			this.containersInfo = containersInfo;
		}
		public IFrameTemplate Template {
			get { return template; }
		}
		public IList<Controller> Controllers {
			get { return controllers; }
		}
		public IModelActionToContainerMapping ContainersInfo {
			get { return containersInfo; }
		}
	}
	public class CustomRegisterActionInContainerEventArgs : HandledEventArgs {
		public CustomRegisterActionInContainerEventArgs(ActionBase action, IActionContainer container) {
			this.Action = action;
			this.Container = container;
		}
		public ActionBase Action { get; private set; }
		public IActionContainer Container { get; private set; }
	}
	public class ModelActionContainersGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			Tracing.Tracer.LogText("Generate default Model for Action containers");
			IModelActionDesign actionDesign = (IModelActionDesign)node.Parent;
			IModelActionToContainerMapping containers = (IModelActionToContainerMapping)node;
			foreach(IModelAction modelAction in actionDesign.Actions) {
				string category = FillActionContainersController.IsUnspecifiedCategory(modelAction.Category) ? PredefinedCategory.Unspecified.ToString() : modelAction.Category;
				IModelActionContainer container = containers[category];
				if(container == null) {
					container = containers.AddNode<IModelActionContainer>(category);
				}
				container.AddNode<IModelActionLink>(modelAction.Id);
			}
		}
	}
	public class FillActionContainersController : Controller, IModelExtender {
		private static readonly string UnspecifiedCategoryId = PredefinedCategory.Unspecified.ToString();
		private IModelActionToContainerMapping actionToContainerMapping;
		private IDynamicContainersTemplate dynamicContainersTemplate;
		private CompositeView currentFrameView;
		private void ReleaseDynamicContainersTemplate() {
			if(dynamicContainersTemplate != null) {
				dynamicContainersTemplate.ActionContainersChanged -= new EventHandler<ActionContainersChangedEventArgs>(dynamicContainersTemplate_ActionContainersChanged);
				dynamicContainersTemplate = null;
			}
		}
		private void Frame_TemplateChanged(Object sender, EventArgs e) {
			ReleaseDynamicContainersTemplate();
			IFrameTemplate frameTemplate = Frame != null ? Frame.Template : null;
			if(frameTemplate != null) {
				IList<Controller> controllers = Frame.Controllers.GetValues();
				CustomFillContainersEventArgs args = new CustomFillContainersEventArgs(frameTemplate, controllers, actionToContainerMapping);
				if(CustomFillContainers != null) {
					CustomFillContainers(this, args);
				}
				if(!args.Handled) {
					FillContainers(GetContainers(frameTemplate), controllers, actionToContainerMapping, frameTemplate);
					dynamicContainersTemplate = frameTemplate as IDynamicContainersTemplate;
					if(dynamicContainersTemplate != null) {
						dynamicContainersTemplate.ActionContainersChanged += new EventHandler<ActionContainersChangedEventArgs>(dynamicContainersTemplate_ActionContainersChanged);
					}
				}
			}
		}
		private IEnumerable<IActionContainer> GetContainers(IFrameTemplate frameTemplate) {
			List<IActionContainer> containers = new List<IActionContainer>(frameTemplate.GetContainers());
			if(frameTemplate.DefaultContainer != null && !containers.Contains(frameTemplate.DefaultContainer)) {
				containers.Add(frameTemplate.DefaultContainer);
			}
			return containers;
		}
		private void dynamicContainersTemplate_ActionContainersChanged(object sender, ActionContainersChangedEventArgs e) {
			if(e.ChangedType == ActionContainersChangedType.Added) {
				FillContainers(e.ActionContainers, Frame.Controllers.GetValues(), actionToContainerMapping, (IFrameTemplate)sender);
			}
		}
		private void Frame_ViewChanged(Object sender, ViewChangedEventArgs e) {
			if(currentFrameView != null) {
				currentFrameView.ControlsCreated -= new EventHandler(View_ControlsCreated);
			}
			currentFrameView = Frame != null ? Frame.View as CompositeView : null;
			if(currentFrameView != null) {
				currentFrameView.ControlsCreated += new EventHandler(View_ControlsCreated); 
				FillViewContainers(currentFrameView, Frame.Controllers.GetValues(), actionToContainerMapping);
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			FillViewContainers(currentFrameView, Frame.Controllers.GetValues(), actionToContainerMapping);
		}
		private ActionCollection CollectActions(IEnumerable<Controller> controllers) {
			ActionCollection actions = new ActionCollection();
			foreach(Controller controller in controllers) {
				actions.AddRange(controller.Actions);
			}
			return actions;
		}
		private void RegisterActions(IActionContainer container, string category, IModelActionContainer containerModel, ActionCollection allActions, ICollection<ActionBase> unassignedActions) {
			ICollection<ActionBase> actions = GetActions(allActions, containerModel);
			IEnumerable<ActionBase> registeredActions = RegisterActions(container, category, allActions, actions);
			foreach(ActionBase action in registeredActions) {
				unassignedActions.Remove(action);
			}
		}
		private ICollection<ActionBase> GetActions(ActionCollection allActions, IModelActionContainer containerModel) {
			List<ActionBase> actions = new List<ActionBase>();
			foreach(IModelActionLink actionLink in containerModel) {
				ActionBase action = allActions.Find(actionLink.ActionId);
				if(action != null) {
					actions.Add(action);
				}
			}
			return actions;
		}
		private IEnumerable<ActionBase> RegisterActions(IActionContainer container, string category, ActionCollection allActions, IEnumerable<ActionBase> containerActions) {
			IEnumerable<ActionBase> actions = containerActions;
			if(CustomizeContainerActions != null) {
				CustomizeContainerActionsEventArgs args = new CustomizeContainerActionsEventArgs(container, category, allActions, actionToContainerMapping, new List<ActionBase>(containerActions));
				CustomizeContainerActions(this, args);
				actions = args.ContainerActions;
			}
			RegisterActions(container, actions);
			return actions;
		}
		private void RegisterActions(IActionContainer container, IEnumerable<ActionBase> actions) {
			if(actions.Any()) {
				container.BeginUpdate();
				try {
					foreach(ActionBase action in actions) {
						if(!container.Actions.Contains(action)) {
							CustomRegisterActionInContainerEventArgs args = new CustomRegisterActionInContainerEventArgs(action, container);
							if(CustomRegisterActionInContainer != null) {
								CustomRegisterActionInContainer(this, args);
							}
							if(!args.Handled) {
								container.Register(action);
							}
						}
					}
				}
				finally {
					container.EndUpdate();
				}
			}
		}
		protected virtual void FillViewContainers(CompositeView view, IList<Controller> controllers, IModelActionToContainerMapping actionToContainerMapping) {
			IList<IActionContainer> items = view.GetItems<IActionContainer>();
			if(items.Count > 0) {
				FillContainers(items, controllers, actionToContainerMapping, null);
			}
		}
		protected virtual void FillContainers(IEnumerable<IActionContainer> containers, IList<Controller> controllers, IModelActionToContainerMapping actionToContainerMapping, IFrameTemplate template) {
			ActionCollection allActions = CollectActions(controllers);
			List<ActionBase> unassignedActions = new List<ActionBase>(allActions);
			ISupportUpdate updatable = template as ISupportUpdate;
			if(updatable != null) {
				updatable.BeginUpdate();
			}
			try {
				List<IActionContainer> containersWithoutModel = new List<IActionContainer>();
				foreach(IActionContainer container in containers) {
					IModelActionContainer containerModel = actionToContainerMapping != null ? actionToContainerMapping[container.ContainerId] : null;
					if(containerModel != null) {
						RegisterActions(container, container.ContainerId, containerModel, allActions, unassignedActions);
					}
					else {
						containersWithoutModel.Add(container);
					}
				}
				IActionContainer defaultContainer = template != null ? template.DefaultContainer : null;
				if(defaultContainer != null && containers.Contains(defaultContainer)) {
					IModelActionContainer unspecifiedContainerModel = actionToContainerMapping != null ? actionToContainerMapping[UnspecifiedCategoryId] : null;
					if(unspecifiedContainerModel != null) {
						RegisterActions(defaultContainer, UnspecifiedCategoryId, unspecifiedContainerModel, allActions, unassignedActions);
					}
					IEnumerable<ActionBase> unassignedActionsWithUnspecifiedCategory = unassignedActions.Where((action) => IsUnspecifiedCategory(action.Category));
					RegisterActions(defaultContainer, UnspecifiedCategoryId, allActions, unassignedActionsWithUnspecifiedCategory);
				}
				foreach(IActionContainer container in containersWithoutModel) {
					IEnumerable<ActionBase> actions = unassignedActions.Where((action) => action.Category == container.ContainerId);
					RegisterActions(container, container.ContainerId, allActions, actions);
				}
			}
			finally {
				if(updatable != null) {
					updatable.EndUpdate();
				}
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				ReleaseDynamicContainersTemplate();
				if(Frame != null) {
					Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
					Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if((Frame.Application != null) && (Frame.Application.Model != null) && (Frame.Application.Model.ActionDesign is IModelActionDesignContainerMapping)) {
				SetContainersInfo(((IModelActionDesignContainerMapping)Frame.Application.Model.ActionDesign).ActionToContainerMapping);
			}
			else {
				SetContainersInfo(null);
			}
			Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
		}
		protected internal void SetContainersInfo(IModelActionToContainerMapping actionToContainerMapping) {
			this.actionToContainerMapping = actionToContainerMapping;
		}
		public event EventHandler<CustomFillContainersEventArgs> CustomFillContainers;
		public event EventHandler<CustomRegisterActionInContainerEventArgs> CustomRegisterActionInContainer; 
		public event EventHandler<CustomizeContainerActionsEventArgs> CustomizeContainerActions;
		protected internal static bool IsUnspecifiedCategory(string category) {
			return String.IsNullOrEmpty(category) || (category == UnspecifiedCategoryId);
		}
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelActionDesign, IModelActionDesignContainerMapping>();
		}
	}
}
