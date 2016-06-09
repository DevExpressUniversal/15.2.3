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
using System.Collections;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.SystemModule {
	public class ActionsCriteriaViewController : ViewController {
		public const string EnabledByCriteriaKey = "By Criteria";
		private ICollection<ActionBase> actions;
		private Dictionary<ActionBase, String> actionsAndExpressions;
		public ObjectView ObjectView {
			get { return (ObjectView)base.View; }
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			UpdateActions();
		}
		private void ObjectSpace_ObjectSaved(object sender, ObjectManipulatingEventArgs e) {
			UpdateActions();
		}
		private void ObjectSpace_ObjectReloaded(object sender, ObjectManipulatingEventArgs e) {
			UpdateActions();
		}
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActions();
		}
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			UpdateActions();
		}
		private void SubscribeToContextMenuTemplate() {
			ListView listView = View as ListView;
			if(listView != null) {
				DevExpress.ExpressApp.Editors.ListEditor listEditor = listView.Editor;
				if(listEditor != null) {
					IContextMenuTemplate contextMenuTemplate = listEditor.ContextMenuTemplate;
					if(contextMenuTemplate != null) {
						contextMenuTemplate.BoundItemCreating += new EventHandler<BoundItemCreatingEventArgs>(contextMenuTemplate_BoundItemCreating);
					}
				}
			}
		}
		private void UnsubscribeFromContextMenuTemplate() {
			ListView listView = View as ListView;
			if(listView != null) {
				DevExpress.ExpressApp.Editors.ListEditor listEditor = listView.Editor;
				if(listEditor != null) {
					IContextMenuTemplate contextMenuTemplate = listEditor.ContextMenuTemplate;
					if(contextMenuTemplate != null) {
						contextMenuTemplate.BoundItemCreating -= new EventHandler<BoundItemCreatingEventArgs>(contextMenuTemplate_BoundItemCreating);
					}
				}
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			SubscribeToContextMenuTemplate();
		}
		private void View_ControlsCreating(object sender, EventArgs e) {
			UnsubscribeFromContextMenuTemplate();
		}
		private void contextMenuTemplate_BoundItemCreating(object sender, BoundItemCreatingEventArgs args) {
			UpdateContextMenuEntry(args);
		}
		private void ClearEventActions() {
			if(actions != null) {
				foreach(ActionBase action in actions) {
					action.Changed -= new EventHandler<ActionChangedEventArgs>(action_Changed);
					action.Enabled.RemoveItem(EnabledByCriteriaKey);
				}
				actions.Clear();
				actions = null;
			}
			if(actionsAndExpressions != null) {
				actionsAndExpressions.Clear();
				actionsAndExpressions = null;
			}
		}
		private void action_Changed(object sender, ActionChangedEventArgs e) {
			if(e.ChangedPropertyType == ActionChangedType.TargetObjectsCriteria || e.ChangedPropertyType == ActionChangedType.TargetObjectsCriteriaMode) {
				ActionBase action = sender as ActionBase;
				if(actionsAndExpressions.ContainsKey(action)) {
					actionsAndExpressions.Remove(action);
				}
				if(AddActionWithCriteria(action)) {
					UpdateAction(action, actionsAndExpressions[action]);
				}
			}
		}
		private void UpdateActions() {
			Dictionary<ActionBase, string> actionsAndExpressions = CollectActionExpressions();
			foreach(KeyValuePair<ActionBase, string> actionAndExpression in actionsAndExpressions) {
				UpdateAction(actionAndExpression.Key, actionAndExpression.Value);
			}
		}
		protected ExpressionEvaluator GetCriteriaExpression(String criteria) {
			EvaluatorContextDescriptor evaluatorContextDescriptor = null;
			if((View is ListView) && (((ListView)View).CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView)) {
				evaluatorContextDescriptor = new DataViewEvaluatorContextDescriptor(ObjectSpace);
			}
			else {
				evaluatorContextDescriptor = ObjectSpace.GetEvaluatorContextDescriptor(((ObjectView)View).ObjectTypeInfo.Type);
			}
			ExpressionEvaluator expression = new ExpressionEvaluator(evaluatorContextDescriptor,
				LocalizedCriteriaWrapper.ParseCriteriaWithReadOnlyParameters(criteria, ((ObjectView)View).ObjectTypeInfo.Type));
			return expression;
		}
		protected virtual void UpdateContextMenuEntry(BoundItemCreatingEventArgs args) {
			String criteria = args.Action.TargetObjectsCriteria;
			if(!String.IsNullOrEmpty(criteria)) {
				args.Enabled = args.Enabled && GetCriteriaExpression(criteria).Fit(args.Object);
			}
		}
		protected virtual ICollection<ActionBase> CollectAllActions() {
			List<ActionBase> result = new List<ActionBase>();
			if(Frame != null) {
				foreach(Controller controller in Frame.Controllers) {
					if(controller is ViewController) {
						ViewController viewController = (ViewController)controller;
						if((viewController.TargetObjectType == null)
								|| viewController.TargetObjectType.IsAssignableFrom(((ObjectView)View).ObjectTypeInfo.Type)) {
							foreach(ActionBase action in controller.Actions) {
								if((action.TargetObjectType == null)
										|| action.TargetObjectType.IsAssignableFrom(((ObjectView)View).ObjectTypeInfo.Type)) {
									result.Add(action);
								}
							}
						}
					}
				}
			}
			return result;
		}
		protected Dictionary<ActionBase, string> CollectActionExpressions() {
			if(actionsAndExpressions == null) {
				actionsAndExpressions = new Dictionary<ActionBase, string>();
				if(ObjectSpace != null) {
					actions = CollectAllActions();
					foreach(ActionBase action in actions) {
						action.Changed += new EventHandler<ActionChangedEventArgs>(action_Changed);
						AddActionWithCriteria(action);
					}
				}
			}
			return actionsAndExpressions;
		}
		private bool AddActionWithCriteria(ActionBase action) {
			bool result = false;
			if(action != null) {
				string criteria = action.TargetObjectsCriteria;
				if(!string.IsNullOrEmpty(criteria)) {
					actionsAndExpressions[action] = criteria;
					result = true;
				}
			}
			return result;
		}
		protected Boolean IsObjectFitToCriteria(ExpressionEvaluator criteriaExpression, object obj) {
			bool isFit = false;
			try {
				isFit = criteriaExpression.Fit(obj);
			}
			catch { }
			return isFit;
		}
		protected virtual void UpdateAction(ActionBase action, string criteria) {
			bool enable = true;
			if(IsActionForUpdate(action) && View != null) {
				IList selectedObjects = ObjectView.SelectedObjects;
				if(selectedObjects == null || selectedObjects.Count == 0) {
					enable = false;
				}
				else {
					ExpressionEvaluator criteriaExpression = GetCriteriaExpression(criteria);
					if(action.TargetObjectsCriteriaMode == TargetObjectsCriteriaMode.TrueAtLeastForOne) {
						enable = false;
						foreach(object obj in selectedObjects) {
							if(IsObjectFitToCriteria(criteriaExpression, obj)) {
								enable = true;
								break;
							}
						}
					}
					else {
						enable = true;
						foreach(object obj in selectedObjects) {
							if(!IsObjectFitToCriteria(criteriaExpression, obj)) {
								enable = false;
								break;
							}
						}
					}
				}
			}
			action.Enabled.SetItemValue(EnabledByCriteriaKey, enable);
		}
		protected virtual bool IsActionForUpdate(ActionBase action) {
			return true;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			ObjectView.SelectionChanged += new EventHandler(View_SelectionChanged);
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
			View.ControlsCreating += new EventHandler(View_ControlsCreating);
			ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			ObjectSpace.ObjectReloaded += new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectReloaded);
			ObjectSpace.ObjectSaved += new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaved);
			SubscribeToContextMenuTemplate();
			UpdateActions();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ClearEventActions();
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			ObjectView.SelectionChanged -= new EventHandler(View_SelectionChanged);
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			View.ControlsCreating -= new EventHandler(View_ControlsCreating);
			ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			ObjectSpace.ObjectReloaded -= new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectReloaded);
			ObjectSpace.ObjectSaved -= new EventHandler<ObjectManipulatingEventArgs>(ObjectSpace_ObjectSaved);
			ListView listView = View as ListView;
			if(listView != null && listView.Editor != null) {
				IContextMenuTemplate contextMenuTemplate = listView.Editor.ContextMenuTemplate;
				if(contextMenuTemplate != null) {
					contextMenuTemplate.BoundItemCreating -= new EventHandler<BoundItemCreatingEventArgs>(contextMenuTemplate_BoundItemCreating);
				}
			}
		}
		public ActionsCriteriaViewController()
			: base() {
			TypeOfView = typeof(ObjectView);
		}
	}
}
