#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Collections;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	[ContentProperty("Items")]
	public class ActionGroup : DXFrameworkContentElement, IActionContainer {
		DependencyObject associatedObject;
		IActionContainer container;
		ObservableCollection<IControllerAction> actions;
		ObservableCollection<ActionTrigger> triggers;
		public ActionExecutionMode ExecutionMode { get; set; }
		public ActionExecutionMode NewActionsExecutionMode { get; set; }
		public ObservableCollection<ActionTrigger> Triggers {
			get { return triggers; }
		}
		public ObservableCollection<IControllerAction> Actions {
			get { return actions; }
			set {
				if (actions == value)
					return;
				var oldValue = actions;
				actions = value;
				OnActionsChanged(oldValue, value);
			}
		}
		public ActionGroup() {
			ExecutionMode = ActionExecutionMode.Manual | ActionExecutionMode.OnEvent;
			NewActionsExecutionMode = ActionExecutionMode.Manual;
			triggers = new ObservableCollection<ActionTrigger>();
			triggers.CollectionChanged += OnTriggersCollectionChanged;
			Actions = new ObservableCollection<IControllerAction>();
		}
		protected virtual void OnTriggersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			DetachTriggers(e.OldItems);
			AttachTriggers(e.NewItems);
		}
		protected virtual void AttachTriggers(IList newItems) {
			if (newItems == null)
				return;
			foreach (ActionTrigger trigger in newItems)
				trigger.AttachToContainer(this);
		}
		protected virtual void DetachTriggers(IList oldItems) {
			if (oldItems == null)
				return;
			foreach (ActionTrigger trigger in oldItems)
				trigger.DetachFromContainer();
		}
		protected virtual void OnActionsChanged(ObservableCollection<IControllerAction> oldValue, ObservableCollection<IControllerAction> newValue) {
			if (oldValue != null)
				oldValue.CollectionChanged -= OnActionsCollectionChanged;
			if (newValue != null)
				newValue.CollectionChanged += OnActionsCollectionChanged;
			if (oldValue == null && newValue == null)
				return;
			if (oldValue == null)
				OnActionsCollectionChanged(null, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Add, newValue));
			else if (newValue == null)
				OnActionsCollectionChanged(null, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, oldValue));
			else
				OnActionsCollectionChanged(null, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Replace, newValue, oldValue));
		}
		private void OnActionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null) {
				foreach(IControllerAction element in e.OldItems) {
					OnElementRemoved(element);
				}
			}
			if (e.NewItems != null) {
				foreach (IControllerAction element in e.NewItems) {
					OnElementAdded(element);
				}
			}
		}
		List<object> logicalChildren = new List<object>();
		protected override IEnumerator LogicalChildren {
			get {
				return logicalChildren.GetEnumerator();
			}
		}
		protected virtual void OnElementAdded(IControllerAction element) {
			SetElementProperties(element);
			logicalChildren.Add(element);
			AddLogicalChild(element);
		}		
		protected virtual void OnElementRemoved(IControllerAction element) {
			ResetElementProperties(element);
			logicalChildren.Remove(element);
			RemoveLogicalChild(element);
		}		
		protected virtual void SetElementProperties(IControllerAction element) {
			element.Container = this;
			var iac = element as IActionContainer;
			if (iac != null) {
				iac.AssociatedObject = associatedObject;
			}
		}
		protected virtual void ResetElementProperties(IControllerAction element) {
			element.Container = null;
			var iac = element as IActionContainer;
			if (iac != null) {
				iac.AssociatedObject = null;
			}
		}
		DependencyObject IActionContainer.AssociatedObject {
			get { return associatedObject; }
			set {
				if (associatedObject == value)
					return;
				associatedObject = value;
				OnAssociatedObjectChanged();
			}
		}		
		IActionContainer IControllerAction.Container {
			get { return container; }
			set {
				if (container == value)
					return;
				container = value;
			}
		}
		void IControllerAction.Execute(DependencyObject context) {
			Execute(context, ActionExecutionMode.Manual);
		}
		protected internal void Execute(DependencyObject context, ActionExecutionMode reason) {
			if (ExecutionMode.HasFlag(reason))
				ExecuteCore(context);
		}
		protected virtual void ExecuteCore(DependencyObject context) {
			if (Actions == null)
				return;
			foreach (var element in Actions) {
				element.Execute(context ?? associatedObject);
				(element as DependencyObject).Do(x => CollectionActionHelper.SetActionExecuted(x, true));
			}
		}
		void OnAssociatedObjectChanged() {
			if (Actions != null) {
				foreach (var element in Actions) {
					if (element is IActionContainer) {
						((IActionContainer)element).AssociatedObject = associatedObject;
					}
				}
			}
			DetachTriggers(triggers);
			AttachTriggers(triggers);			
		}
	}
}
