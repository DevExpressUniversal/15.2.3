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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.EasyTest.Utils;
namespace DevExpress.ExpressApp.Templates.ActionControls.Binding {
	public abstract class ActionBinding : IDisposable {
		private ActionBase action;
		private IActionControl actionControl;
		private void Action_Changed(object sender, ActionChangedEventArgs e) {
			OnActionChanged(e.ChangedPropertyType);
		}
		protected virtual void OnActionChanged(ActionChangedType changesType) {
			switch(changesType) {
				case ActionChangedType.Active: {
						UpdateControlVisibility();
						break;
					}
				case ActionChangedType.Enabled: {
						UpdateControlEnabledState();
						break;
					}
				case ActionChangedType.Caption: {
						UpdateControlCaption();
						break;
					}
				case ActionChangedType.ToolTip: {
						UpdateControlToolTip();
						break;
					}
				case ActionChangedType.Image: {
						UpdateControlImage();
						break;
					}
				case ActionChangedType.ConfirmationMessage: {
						UpdateControlConfirmationMessage();
						break;
					}
				case ActionChangedType.Shortcut: {
						UpdateControlShortcut();
						break;
					}
				case ActionChangedType.PaintStyle: {
						UpdateControlPaintStyle();
						break;
					}
			}
		}
		protected virtual void UpdateControlVisibility() {
			ActionControl.SetVisible(Action.Active);
		}
		protected virtual void UpdateControlEnabledState() {
			ActionControl.SetEnabled(Action.Enabled);
		}
		protected virtual void UpdateControlCaption() {
			ActionControl.SetCaption(Action.Caption);
		}
		protected virtual void UpdateControlToolTip() {
			ActionControl.SetToolTip(Action.GetTotalToolTip());
		}
		protected virtual void UpdateControlImage() {
			ActionControl.SetImage(Action.ImageName);
		}
		protected virtual void UpdateControlConfirmationMessage() {
			ActionControl.SetConfirmationMessage(Action.GetFormattedConfirmationMessage());
		}
		protected virtual void UpdateControlShortcut() {
			ActionControl.SetShortcut(Action.Shortcut);
		}
		protected virtual void UpdateControlPaintStyle() {
			ActionControl.SetPaintStyle(Action.PaintStyle);
		}
		protected ActionBinding(ActionBase action, IActionControl actionControl) {
			this.action = action;
			this.actionControl = actionControl;
			Action.Changed += Action_Changed;
			UpdateControlVisibility();
			UpdateControlEnabledState();
			UpdateControlCaption();
			UpdateControlToolTip();
			UpdateControlImage();
			UpdateControlConfirmationMessage();
			UpdateControlShortcut();
			UpdateControlPaintStyle();
			if(actionControl is ITestableControl) { 
				((ITestableControl)actionControl).SetTestName(Action.TestName);
			}
		}
		public virtual void Dispose() {
			Action.Changed -= Action_Changed;
			this.action = null;
			this.actionControl = null;
		}
		public ActionBase Action {
			get { return action; }
		}
		public IActionControl ActionControl {
			get { return actionControl; }
		}
	}
}
