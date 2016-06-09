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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Templates.ActionControls.Binding {
	public class ActionToControlBinder {
		private ActionCollection actions;
		private bool isActionsInitialized;
		private IDictionary<IActionControl, ActionBinding> bindingByControl;
		private void unbindedActionControl_NativeControlDisposed(object sender, EventArgs e) {
			IActionControl actionControl = (IActionControl)sender;
			actionControl.NativeControlDisposed -= unbindedActionControl_NativeControlDisposed;
			bindingByControl[actionControl].Dispose();
			bindingByControl.Remove(actionControl);
		}
		private bool RegisterActionControlCore(IActionControl actionControl) {
			Guard.ArgumentNotNull(actionControl, "actionControl");
			Guard.ArgumentNotNull(actionControl.ActionId, "actionControl.ActionId");
			if(!isActionsInitialized) {
				throw new InvalidOperationException("Actions are not initialized.");
			}
			if(!bindingByControl.ContainsKey(actionControl)) {
				bindingByControl.Add(actionControl, null);
				return true;
			}
			return false;
		}
		private void UpdateBindings() {
			foreach(IActionControl unbindedActionControl in GetUnbindedControls()) {
				ActionBase action = actions.Find(unbindedActionControl.ActionId);
				if(action == null) {
					string message = string.Format("Cannot find the Action for the '{0}' Action Control.", unbindedActionControl.ActionId);
					throw new InvalidOperationException(message);
				}
				bindingByControl[unbindedActionControl] = Bind(action, unbindedActionControl);
				unbindedActionControl.NativeControlDisposed += unbindedActionControl_NativeControlDisposed;
			}
		}
		private IEnumerable<IActionControl> GetUnbindedControls() {
			List<IActionControl> unbindedControls = new List<IActionControl>();
			foreach(KeyValuePair<IActionControl, ActionBinding> entry in bindingByControl) {
				if(entry.Value == null) {
					unbindedControls.Add(entry.Key);
				}
			}
			return unbindedControls;
		}
		private ActionBinding Bind(ActionBase action, IActionControl actionControl) {
			if(CustomBind != null) {
				CustomBindEventArgs args = new CustomBindEventArgs(action, actionControl);
				CustomBind(this, args);
				if(args.Binding != null) {
					return args.Binding;
				}
			}
			ActionBinding binding = BindCore(action, actionControl);
			if(binding == null) {
				string message = string.Format("Cannot bind the '{0}' Action to the '{1}' Action Control.", action, actionControl);
				throw new InvalidOperationException(message);
			}
			return binding;
		}
		protected virtual ActionBinding BindCore(ActionBase action, IActionControl actionControl) {
			return ActionBindingFactory.Instance.Create(action, actionControl);
		}
		public ActionToControlBinder() {
			actions = new ActionCollection();
			bindingByControl = new Dictionary<IActionControl, ActionBinding>();
		}
		public void SetActions(IEnumerable<ActionBase> actions) {
			Guard.ArgumentNotNull(actions, "actions");
			if(isActionsInitialized) {
				throw new InvalidOperationException("Actions are already initialized.");
			}
			this.actions.AddRange(actions);
			isActionsInitialized = true;
		}
		public void RegisterActionControl(IActionControl actionControl) {
			if(RegisterActionControlCore(actionControl)) {
				UpdateBindings();
			}
		}
		public void RegisterActionControls(IEnumerable<IActionControl> actionControls) {
			Guard.ArgumentNotNull(actionControls, "actionControls");
			bool wasRegistered = false;
			foreach(IActionControl actionControl in actionControls) {
				if(RegisterActionControlCore(actionControl)) {
					wasRegistered = true;
				}
			}
			if(wasRegistered) {
				UpdateBindings();
			}
		}
		public void Clear() {
			foreach(KeyValuePair<IActionControl, ActionBinding> entry in bindingByControl) {
				entry.Key.NativeControlDisposed -= unbindedActionControl_NativeControlDisposed;
				entry.Value.Dispose();
			}
			bindingByControl.Clear();
			actions.Clear();
			isActionsInitialized = false;
		}
		public event EventHandler<CustomBindEventArgs> CustomBind;
	}
	public class CustomBindEventArgs : EventArgs {
		private ActionBase action;
		private IActionControl actionControl;
		public CustomBindEventArgs(ActionBase action, IActionControl actionControl) {
			this.action = action;
			this.actionControl = actionControl;
		}
		public ActionBase Action {
			get { return action; }
		}
		public IActionControl ActionControl {
			get { return actionControl; }
		}
		public ActionBinding Binding { get; set; }
	}
}
