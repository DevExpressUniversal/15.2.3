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
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.StateMachine {
	public class StateMachineCacheController : ViewController {
		private Type stateMachineStorageType;
		public StateMachineCacheController() : base() { }
#if DebugTest
		internal StateMachineCacheController(Type stateMachineStorageType)
			: base() {
			this.stateMachineStorageType = stateMachineStorageType;
		}
		public List<IStateMachine> Cache {
			get {
				return cache;
			}
		}
#endif
		private List<IStateMachine> cache = new List<IStateMachine>();
		internal bool isCompleteCache = false;
		public ReadOnlyCollection<IStateMachine> CachedStateMachines {
			get {
				return cache.AsReadOnly();
			}
		}
		public void ClearCache() {
			cache.Clear();
			isCompleteCache = false;
		}
		private void EnsureCache() {
			if(!isCompleteCache) {
				if(stateMachineStorageType == null) {
					stateMachineStorageType = ((StateMachineModule)Application.Modules.FindModule(typeof(StateMachineModule))).StateMachineStorageType;
				}
				if(ObjectSpace.CanInstantiate(stateMachineStorageType)) {
					IList stateMachines = ObjectSpace.GetObjects(stateMachineStorageType, null);
					if(stateMachines != null) {
						foreach(object stateMachineObject in stateMachines) {
							IStateMachine stateMachine = (IStateMachine)stateMachineObject;
							cache.Add(stateMachine);
						}
						isCompleteCache = true;
					}
				}
			}
		}
		public virtual IList<IStateMachine> GetStateMachinesByType(Type targetObjectType) {
			EnsureCache();
			List<IStateMachine> result = new List<IStateMachine>();
			foreach(IStateMachine stateMachine in cache) {
				if(stateMachine.Active && stateMachine.TargetObjectType.IsAssignableFrom(targetObjectType)) {
					result.Add(stateMachine);
				}
			}
			return result;
		}
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.Reloaded += new EventHandler(ObjectSpace_Reloaded);
		}
		protected override void OnDeactivated() {
			ObjectSpace.Reloaded -= new EventHandler(ObjectSpace_Reloaded);
			base.OnDeactivated();
			ClearCache();
		}
		private void ObjectSpace_Reloaded(object sender, EventArgs e) {
			ClearCache();
		}
	}
	public abstract class StateMachineControllerBase<T> : ViewController<T> where T : ObjectView {
		internal Object FindSampleTargetObject() {
			Object result = (View.SelectedObjects.Count == 1) ? View.SelectedObjects[0] : null;
			if(result == null && View is ListView
				&& (View as ListView).CollectionSource != null
				&& (View as ListView).CollectionSource.List != null
				&& (View as ListView).CollectionSource.List.Count > 0) {
				result = (View as ListView).CollectionSource.List[0];
			}
			return result;
		}
		protected void OnGetStateMachines(StateMachinesEventArgs args) {
			if(GetStateMachinesByType != null) {
				GetStateMachinesByType(this, args);
			}
		}
		protected StateMachineCacheController StateMachineCacheController {
			get {
				if(Frame != null) {
					return Frame.GetController<StateMachineCacheController>();
				}
				return null;
			}
		}
		protected IList<IStateMachine> GetStateMachines() {
			List<IStateMachine> stateMachines = new List<IStateMachine>();
			if(View != null) {
				object sampleTargetObject = FindSampleTargetObject();
				if(sampleTargetObject is IStateMachineProvider) {
					foreach(IStateMachine stateMachine in ((IStateMachineProvider)(sampleTargetObject)).GetStateMachines()) {
						if(stateMachine.Active) {
							stateMachines.Add(stateMachine);
						}
					}
				}
				Type currentObjectType = sampleTargetObject != null ? sampleTargetObject.GetType() : View.ObjectTypeInfo.Type;
				StateMachinesEventArgs stateMachinesEventArgs = new StateMachinesEventArgs(currentObjectType);
				OnGetStateMachines(stateMachinesEventArgs);
				if(stateMachinesEventArgs.Handled) {
					stateMachines.AddRange(stateMachinesEventArgs.StateMachines);
				}
				else {
					StateMachineCacheController stateMachineCacheController = StateMachineCacheController;
					if(stateMachineCacheController != null) {
						stateMachines.AddRange(stateMachineCacheController.GetStateMachinesByType(currentObjectType));
					}
				}
			}
			return stateMachines;
		}
		public event EventHandler<StateMachinesEventArgs> GetStateMachinesByType;
	}
	public class StateMachineController : StateMachineControllerBase<ObjectView> {
		private const string EditModeKey = "ViewIsInEditMode";
		private const string SecurityKey = "EnabledBySecurity";
		private SingleChoiceAction changeStateAction;
		private Dictionary<object, List<SimpleAction>> panelActions = new Dictionary<object, List<SimpleAction>>();
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void changeStateAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			ITransition transition = e.SelectedChoiceActionItem.Data as ITransition;
			if(transition != null) {
				ExecuteTransition(e.CurrentObject, transition);
				if(Frame != null) {
					UpdateActionState(); 
				}
			}
		}
		private void SimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ChoiceActionItem transitionItem = e.Action.Tag as ChoiceActionItem;
			if(transitionItem != null) {
				changeStateAction.DoExecute(transitionItem);
			}
		}
		private int TransitionsComparison(ITransition t1, ITransition t2) {
			ITransitionUISettings tui1 = t1 as ITransitionUISettings;
			ITransitionUISettings tui2 = t2 as ITransitionUISettings;
			if(tui1 == null) {
				if(tui2 == null) {
					return 0;
				}
				else {
					return 1;
				}
			}
			if(tui2 == null) {
				return -1;
			}
			if(originalTransitionsList != null && tui1.Index == 0 && tui2.Index == 0) {
				return originalTransitionsList.IndexOf(t1) - originalTransitionsList.IndexOf(t2);
			}
			return tui1.Index - tui2.Index;
		}
		private IList<ITransition> originalTransitionsList = null;
		private IEnumerable<ITransition> GetOrderedTransitions(IList<ITransition> transitionsList) {
			List<ITransition> sortedTransitionsList = new List<ITransition>(transitionsList);
			originalTransitionsList = transitionsList;
			sortedTransitionsList.Sort(TransitionsComparison);
			originalTransitionsList = null;
			return sortedTransitionsList;
		}
		private void ResetActionsPanel() {
			DetailView detailView = View as DetailView;
			if(detailView != null) {
				foreach(string key in panelActions.Keys) {
					ActionContainerViewItem actionContainer = detailView.FindItem(key) as ActionContainerViewItem;
					if(actionContainer != null) {
						actionContainer.ClearActions();  
					}
				}
				foreach(List<SimpleAction> actionsList in panelActions.Values) {
					foreach(SimpleAction simpleAction in actionsList) {
						simpleAction.Execute -= new SimpleActionExecuteEventHandler(SimpleAction_Execute);
					}
				}
			}
			panelActions.Clear();
		}
		private void IntializeActionsPanel(IStateMachine stateMachine, ChoiceActionItemCollection transitionItems) {
			List<SimpleAction> actionsList = new List<SimpleAction>();
			foreach(ChoiceActionItem transitionItem in transitionItems) {
				SimpleAction action = CreateSimpleTransitionAction(stateMachine, transitionItem);
				actionsList.Add(action);
			}
			string actionsContainerId = "StateMachineActionContainer_" + stateMachine.Name.Replace(" ", "_");
			panelActions[actionsContainerId] = actionsList;
			AddStateMachineActionsContainerToDetailViewLayout((DetailView)View, actionsContainerId, stateMachine.Name);
		}
		private SimpleAction CreateSimpleTransitionAction(IStateMachine stateMachine, ChoiceActionItem transitionItem) {
			SimpleAction action = new SimpleAction(this, Guid.NewGuid().ToString(), "StateMachineActions");
			action.Enabled[EditModeKey] = ((DetailView)View).ViewEditMode == ViewEditMode.Edit;
			action.Enabled[SecurityKey] = DataManipulationRight.CanEdit(stateMachine.TargetObjectType, stateMachine.StatePropertyName, View.CurrentObject, null, View.ObjectSpace);
			action.Tag = transitionItem;
			action.Caption = transitionItem.Caption;
			action.Execute += new SimpleActionExecuteEventHandler(SimpleAction_Execute);
			return action;
		}
		private void AddStateMachineActionsContainerToDetailViewLayout(DetailView detailView, string actionsContainerId, string caption) {
			IModelActionContainerViewItem modelActionContainerViewItem = (IModelActionContainerViewItem)detailView.Model.Items[actionsContainerId];
			if(modelActionContainerViewItem == null) {
				modelActionContainerViewItem = detailView.Model.Items.AddNode<IModelActionContainerViewItem>(actionsContainerId);
				ModelApplicationBase modelApplication = (ModelApplicationBase)detailView.Model.Application;
				string currentAspect = modelApplication.CurrentAspect;
				modelApplication.SetCurrentAspect("");
				modelActionContainerViewItem.Caption = caption;
				modelApplication.SetCurrentAspect(currentAspect);
				IModelViewLayoutElement rootLayoutElement = detailView.Model.Layout.Count > 0 ? detailView.Model.Layout[0] : null;
				IModelLayoutViewItem containerLayoutItem = null;
				if(rootLayoutElement is IModelLayoutGroup) {
					containerLayoutItem = ((IModelLayoutGroup)rootLayoutElement).AddNode<IModelLayoutViewItem>(modelActionContainerViewItem.Id);
				}
				else {
					containerLayoutItem = detailView.Model.Layout.AddNode<IModelLayoutViewItem>(modelActionContainerViewItem.Id);
				}
				containerLayoutItem.ViewItem = modelActionContainerViewItem;
				containerLayoutItem.ShowCaption = true;
				detailView.AddItem(modelActionContainerViewItem);
			}
		}
		private void detailView_ViewEditModeChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_ModelChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			List<string> stateProperties = new List<string>();
			foreach(IStateMachine stateMachine in GetStateMachines()) {
				stateProperties.Add(stateMachine.StatePropertyName);
			}
			if(stateProperties.Count > 0 && e.Object == View.CurrentObject && stateProperties.Contains(e.PropertyName)) {
				UpdateActionState();
			}
		}
		protected internal virtual void UpdateActionState() {
			if(!StateMachineCacheController.Active) {
				StateMachineCacheController.Activated += new EventHandler(StateMachineCacheController_Activated);
			}
			else {
				ISupportUpdate updatable = null;
				try {
					DetailView detailView = View as DetailView;
					UnsubscribeChangeStateActionChanged();
					changeStateAction.Items.Clear();
					if(detailView != null) {
						updatable = detailView.LayoutManager.Container as ISupportUpdate;
						if(updatable != null) {
							updatable.BeginUpdate();
						}
						ResetActionsPanel();
					}
					RefreshChangeStateActionItems();
					RefreshActionsPanel(detailView);
					if(ActionStateUpdated != null) {
						ActionStateUpdated(this, EventArgs.Empty);
					}
				}
				finally {
					if(updatable != null) {
						updatable.EndUpdate();
					}
					SubscribeChangeStateActionChanged();
				}
			}
		}
		protected override void BeginUpdate() {
			base.BeginUpdate();
			UnsubscribeChangeStateActionChanged();
		}
		protected override void EndUpdate() {
			base.EndUpdate();
			SubscribeChangeStateActionChanged();
			UpdatePanelActions();
		}
		private void RefreshChangeStateActionItems() {
			Object viewCurrentObject = (View != null && View.SelectedObjects.Count == 1) ? View.SelectedObjects[0] : null;
			if(viewCurrentObject != null) {
				IList<IStateMachine> stateMachines = GetStateMachines();
				foreach(IStateMachine stateMachine in stateMachines) {
					IState currentState = stateMachine.FindCurrentState(viewCurrentObject);
					if((currentState != null) && (currentState.Transitions.Count > 0)) {
						ChoiceActionItem machineAction = new ChoiceActionItem(stateMachine.Name, stateMachine);
						machineAction.Enabled[SecurityKey] = DataManipulationRight.CanEdit(stateMachine.TargetObjectType, stateMachine.StatePropertyName, viewCurrentObject, null, View.ObjectSpace);
						changeStateAction.Items.Add(machineAction);
						foreach(ITransition transition in GetOrderedTransitions(currentState.Transitions)) {
							machineAction.Items.Add(new ChoiceActionItem(transition.Caption, transition));
						}
						if(View is DetailView && (stateMachine is IStateMachineUISettings) && ((IStateMachineUISettings)stateMachine).ExpandActionsInDetailView) {
							IntializeActionsPanel(stateMachine, machineAction.Items);
						}
					}
				}
			}
		}
		private void RefreshActionsPanel(DetailView detailView) {
			if(detailView != null) {
				changeStateAction.Enabled[EditModeKey] = detailView.ViewEditMode == ViewEditMode.Edit;
				bool allContainersAreOnTheView = true;
				foreach(string key in panelActions.Keys) {
					ActionContainerViewItem actionContainer = detailView.FindItem(key) as ActionContainerViewItem;
					if(actionContainer == null) {
						allContainersAreOnTheView = false;
						break;
					}
				}
				if(allContainersAreOnTheView) {
					RegisterActionsInPanelContainers(detailView);
				}
				else if(detailView.IsControlCreated) {
					View.BreakLinksToControls();
					View.LoadModel();
					View.CreateControls();
				}
			}
			else {
				changeStateAction.Enabled.RemoveItem(EditModeKey);
			}
		}
		private void StateMachineCacheController_Activated(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void RegisterActionsInPanelContainers(DetailView detailView) {
			foreach(string key in panelActions.Keys) {
				ActionContainerViewItem actionContainer = detailView.FindItem(key) as ActionContainerViewItem;
				if(actionContainer != null) {
					actionContainer.ClearActions();  
					foreach(SimpleAction action in panelActions[key]) {
						actionContainer.Register(action);
					}
				}
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			DetailView detailView = View as DetailView;
			if(detailView != null) {
				RegisterActionsInPanelContainers(detailView);
			}
		}
		private void SubscribeChangeStateActionChanged() {
			if(View is DetailView && changeStateAction.LockCount == 0) {
				changeStateAction.Changed -= new EventHandler<ActionChangedEventArgs>(stateMachineAction_Changed);
				changeStateAction.Changed += new EventHandler<ActionChangedEventArgs>(stateMachineAction_Changed);
			}
		}
		private void UnsubscribeChangeStateActionChanged() {
			changeStateAction.Changed -= new EventHandler<ActionChangedEventArgs>(stateMachineAction_Changed);
		}
		private void stateMachineAction_Changed(object sender, ActionChangedEventArgs e) {
			if(e.ChangedPropertyType == ActionChangedType.Active || e.ChangedPropertyType == ActionChangedType.Enabled) {
				UpdatePanelActions();
			}
		}
		private void UpdatePanelActions() {
			if(View is DetailView) {
				foreach(List<SimpleAction> actionsList in panelActions.Values) {
					foreach(SimpleAction simpleAction in actionsList) {
						simpleAction.Enabled["StateMachineActionEnabled"] = changeStateAction.Enabled;
						simpleAction.Active["StateMachineActionActive"] = changeStateAction.Active;
					}
				}
			}
		}
		protected internal void ExecuteTransition(object targetObject, ITransition transition) {
			ExecuteTransitionEventArgs args = new ExecuteTransitionEventArgs(targetObject, transition);
			OnStateMachineTransitionExecuting(args);
			if(!args.Cancel) {
				transition.TargetState.StateMachine.ExecuteTransition(targetObject, transition.TargetState);
				ObjectSpace.SetModified(targetObject);
				OnStateMachineTransitionExecuted(args);
				ITransitionUISettings transitionSettings = transition as ITransitionUISettings;
				if(transitionSettings.SaveAndCloseView) {
					View.ObjectSpace.CommitChanges();
					View.Close();
				}
				if(View is ListView && Frame != null) {
					ModificationsController autocommitController = Frame.GetController<ModificationsController>();
					if((autocommitController != null) && (autocommitController.ModificationsHandlingMode == ModificationsHandlingMode.AutoCommit)) {
						View.ObjectSpace.CommitChanges();
					}
				}
			}
		}
		protected virtual void OnStateMachineTransitionExecuting(ExecuteTransitionEventArgs args) {
			if(TransitionExecuting != null) {
				TransitionExecuting(this, args);
			}
		}
		protected virtual void OnStateMachineTransitionExecuted(ExecuteTransitionEventArgs args) {
			if(TransitionExecuted != null) {
				TransitionExecuted(this, args);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.SelectionChanged += new EventHandler(View_SelectionChanged);
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
			View.ModelChanged += new EventHandler(View_ModelChanged);
			View.ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			DetailView detailView = View as DetailView;
			if(detailView != null) {
				detailView.ViewEditModeChanged += new EventHandler<EventArgs>(detailView_ViewEditModeChanged);
			}
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			DetailView detailView = View as DetailView;
			if(detailView != null) {
				detailView.ViewEditModeChanged -= new EventHandler<EventArgs>(detailView_ViewEditModeChanged);
			}
			View.ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			View.ModelChanged -= new EventHandler(View_ModelChanged);
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			StateMachineCacheController.Activated -= new EventHandler(StateMachineCacheController_Activated);
			base.OnDeactivated();
		}
		public StateMachineController() {
			changeStateAction = new SingleChoiceAction(this, "ChangeStateAction", PredefinedCategory.Edit);
			changeStateAction.ImageName = "Action_StateMachine";
			changeStateAction.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Image;
			changeStateAction.Caption = "Change State";
			changeStateAction.ToolTip = "Change state of the current object";
			changeStateAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			changeStateAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			changeStateAction.Execute += new SingleChoiceActionExecuteEventHandler(changeStateAction_Execute);
			SubscribeChangeStateActionChanged();
		}
		public SingleChoiceAction ChangeStateAction {
			get { return changeStateAction; }
		}
		public event EventHandler<ExecuteTransitionEventArgs> TransitionExecuting;
		public event EventHandler<ExecuteTransitionEventArgs> TransitionExecuted;
		public event EventHandler<EventArgs> ActionStateUpdated;
#if DebugTest
		public SimpleAction DebugTest_CreateSimpleTransitionAction(IStateMachine stateMachine, ChoiceActionItem transitionItem) {
			return CreateSimpleTransitionAction(stateMachine, transitionItem);
		}
		public void DebugTest_AddStateMachineActionsContainerToDetailViewLayout(DetailView detailView, string actionsContainerId, string caption) {
			AddStateMachineActionsContainerToDetailViewLayout(detailView, actionsContainerId, caption);
		}
		public void DebugTest_ChangeStateAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
			changeStateAction_Execute(sender, e);
		}
#endif
	}
	public class StateMachinesEventArgs : HandledEventArgs {
		Type targetObjectType;
		private List<IStateMachine> _stateMachines = new List<IStateMachine>();
		public StateMachinesEventArgs(Type targetObjectType)
			: base(false) {
			this.targetObjectType = targetObjectType;
		}
		public void Add(IStateMachine stateMachine) {
			_stateMachines.Add(stateMachine);
		}
		public void AddRange(IEnumerable<IStateMachine> stateMachines) {
			_stateMachines.AddRange(stateMachines);
		}
		public Type TargetObjectType {
			get {
				return targetObjectType;
			}
		}
		public ReadOnlyCollection<IStateMachine> StateMachines {
			get { return _stateMachines.AsReadOnly(); }
		}
	}
	public class ExecuteTransitionEventArgs : CancelEventArgs {
		public ExecuteTransitionEventArgs(object targetObject, ITransition transition)
			: base(false) {
			Guard.ArgumentNotNull(transition, "transition");
			TargetObject = targetObject;
			Transition = transition;
		}
		public object TargetObject { get; private set; }
		public ITransition Transition { get; private set; }
	}
}
