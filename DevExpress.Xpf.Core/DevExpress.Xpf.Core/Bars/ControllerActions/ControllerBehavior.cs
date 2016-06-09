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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Mvvm.UI.Interactivity;
using System.Globalization;
namespace DevExpress.Xpf.Bars {
	public class ActionTrigger : Mvvm.UI.Interactivity.EventTrigger {
		ActionGroup owner;
		bool executed = false;
		public bool ExecuteOnce { get; set; }
		protected internal void AttachToContainer(ActionGroup owner) {
			var iActionContainer = (IActionContainer)owner;
			if (iActionContainer==null || iActionContainer.AssociatedObject == null)
				return;
			this.owner = owner;
			Attach(iActionContainer.AssociatedObject);
		}
		protected internal void DetachFromContainer() {
			Detach();
			owner = null;
		}
		protected override void OnEvent(object sender, object eventArgs) {
			base.OnEvent(sender, eventArgs);
			if (owner != null && (!ExecuteOnce || !executed)) {
				executed = true;
				owner.Execute(null, ActionExecutionMode.OnEvent);				
			}				
		}		
	}
	[Flags]
	public enum ActionExecutionMode {
		Manual = 0x0,
		OnAssociatedObjectChanged = 0x1,
		OnEvent = 0x2,
		All = OnAssociatedObjectChanged | OnEvent | Manual
	}	
	[ContentProperty("Actions")]
	public class ControllerBehavior : Behavior {
		public static readonly DependencyProperty ActionsProperty;
		protected static readonly DependencyPropertyKey TriggersPropertyKey;
		public static readonly DependencyProperty TriggersProperty;
		public static readonly DependencyProperty NewActionsExecutionModeProperty;
		public static readonly DependencyProperty ExecutionModeProperty;
		public static readonly DependencyProperty ForcedDataContextProperty;		
		ActionGroup rootGroup;
		ActionGroup RootGroup { get { return rootGroup; } }
		IActionContainer RootActionContainer { get { return rootGroup; } }
		static ControllerBehavior() {
			ForcedDataContextProperty = DependencyProperty.Register("ForcedDataContext", typeof(object), typeof(ControllerBehavior), new FrameworkPropertyMetadata(null, (d,e)=>((ControllerBehavior)d).OnForcedDataContextChanged(e.OldValue, e.NewValue)));
			NewActionsExecutionModeProperty = DependencyPropertyManager.Register("NewActionsExecutionMode", typeof(ActionExecutionMode), typeof(ControllerBehavior), new FrameworkPropertyMetadata(ActionExecutionMode.Manual));
			ExecutionModeProperty = DependencyPropertyManager.Register("ExecutionMode", typeof(ActionExecutionMode), typeof(ControllerBehavior), new FrameworkPropertyMetadata(ActionExecutionMode.Manual));
			ActionsProperty = DependencyPropertyManager.Register("Actions", typeof(ObservableCollection<IControllerAction>), typeof(ControllerBehavior), new FrameworkPropertyMetadata(null, (d, e) => ((ControllerBehavior)d).OnActionsChanged((ObservableCollection<IControllerAction>)e.OldValue)));
			TriggersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Triggers", typeof(ObservableCollection<ActionTrigger>), typeof(ControllerBehavior), new FrameworkPropertyMetadata(null));
			TriggersProperty = TriggersPropertyKey.DependencyProperty;
		}
		protected virtual void OnForcedDataContextChanged(object oldValue, object newValue) {
			if (newValue == System.Windows.Data.Binding.DoNothing)
				RootGroup.DataContext = null;
			else if (newValue != null)
				RootGroup.DataContext = newValue;
			else
				RootGroup.ClearValue(FrameworkElement.DataContextProperty);
		}
		protected virtual void OnActionsChanged(ObservableCollection<IControllerAction> oldValue) {
			RootGroup.Actions = Actions;
		}
		public ControllerBehavior() : base(typeof(DependencyObject)) {
			rootGroup = new ActionGroup();
			Actions = new ObservableCollection<IControllerAction>();
			SetValue(TriggersPropertyKey, new ObservableCollection<ActionTrigger>());
			Triggers.CollectionChanged += OnEventTriggersCollectionChanged;
		}
		public ActionExecutionMode ExecutionMode {
			get { return (ActionExecutionMode)GetValue(ExecutionModeProperty); }
			set { SetValue(ExecutionModeProperty, value); }
		}
		public ActionExecutionMode NewActionsExecutionMode {
			get { return (ActionExecutionMode)GetValue(NewActionsExecutionModeProperty); }
			set { SetValue(NewActionsExecutionModeProperty, value); }
		}
		public object ForcedDataContext {
			get { return (object)GetValue(ForcedDataContextProperty); }
			set { SetValue(ForcedDataContextProperty, value); }
		}
		public ObservableCollection<IControllerAction> Actions {
			get { return (ObservableCollection<IControllerAction>)GetValue(ActionsProperty); }
			set { SetValue(ActionsProperty, value); }
		}
		public ObservableCollection<ActionTrigger> Triggers {
			get { return (ObservableCollection<ActionTrigger>)GetValue(TriggersProperty); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			AddLogicalChild(RootGroup);
			RootActionContainer.AssociatedObject = AssociatedObject;
			if (ExecutionMode.HasFlag(ActionExecutionMode.OnAssociatedObjectChanged)) {
				ExecuteOnAssociatedObjectChanged();
			}
		}
		protected virtual void ExecuteOnAssociatedObjectChanged() {
			if (AssociatedObject == null)
				return;
			Execute();
		}
		protected override void OnDetaching() {
			RemoveLogicalChild(RootGroup);
			RootActionContainer.AssociatedObject = null;
			base.OnDetaching();
		}
		void OnEventTriggersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null) {
				foreach(ActionTrigger trigger in e.OldItems) 
					RootGroup.Triggers.Remove(trigger);
			}
			if (e.NewItems != null) {
				foreach (ActionTrigger trigger in e.NewItems)
					RootGroup.Triggers.Add(trigger);
			}
		}
		const string invalidAssociatedObjectExceptionString = "AssociatedObject should be UIElement or implement the ILogicalChildrenContainer or the ILogicalOwner interface";
		void ClearLogicalChildren() {
			while (addedLogicalChildren.Count != 0)
				RemoveLogicalChild(addedLogicalChildren[0]);
		}
		readonly List<DependencyObject> addedLogicalChildren = new List<DependencyObject>();
		void AddLogicalChild(DependencyObject child) {
			if (child==null || LogicalTreeHelper.GetParent(child) != null)
				return;
			try {
				if (AssociatedObject is ILogicalChildrenContainer) {
					((ILogicalChildrenContainer)AssociatedObject).AddLogicalChild(child);
					return;
				}
				if (AssociatedObject is ILogicalOwner) {
					((ILogicalOwner)AssociatedObject).AddChild(child);
					return;
				}
				if (AssociatedObject is UIElement) {
					LogicalTreeWrapper.SetRoot(child, AssociatedObject as UIElement);
					return;
				}
			} finally {
				addedLogicalChildren.Add(child);
			}
			addedLogicalChildren.Remove(child);
			throw new NotImplementedException(invalidAssociatedObjectExceptionString);
		}
		void RemoveLogicalChild(DependencyObject child) {
			if (child == null)
				return;
			try {
				if (AssociatedObject is ILogicalChildrenContainer) {
					((ILogicalChildrenContainer)AssociatedObject).RemoveLogicalChild(child);
					return;
				}
				if (AssociatedObject is ILogicalOwner) {
					((ILogicalOwner)AssociatedObject).RemoveChild(child);
					return;
				}
				if (AssociatedObject is UIElement) {
					LogicalTreeWrapper.SetRoot(child, null);
					return;
				}
			} finally {
				addedLogicalChildren.Remove(child);
			}
			throw new NotImplementedException(invalidAssociatedObjectExceptionString);
		}		
		public void Execute(DependencyObject context = null) {
			RootGroup.Execute(context, ActionExecutionMode.Manual);
		}		
	}	
}
namespace DevExpress.Xpf.Bars.Native {
	public class CustomizationActionsField {
		ControllerBehavior cBehavior;
		bool clearDataContext = false;
		DependencyObject owner;
		public ObservableCollection<IControllerAction> Value { get { return (cBehavior ?? (cBehavior = CreateControllerBehavior())).Actions; } }
		public bool Execute(DependencyObject context) {
			if (cBehavior == null)
				return false;
			cBehavior.Execute(context);
			return true;
		}
		ControllerBehavior CreateControllerBehavior() {
			var cBeh = new ControllerBehavior();
			if (clearDataContext)
				cBeh.ForcedDataContext = System.Windows.Data.Binding.DoNothing;
			cBeh.Attach(owner);
			return cBeh;
		}
		public CustomizationActionsField(DependencyObject owner, bool clearDataContext = false) {
			this.owner = owner;
			this.clearDataContext = clearDataContext;
		}
	}
}
