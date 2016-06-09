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
using DevExpress.ExpressApp.Actions;
using System.Windows.Forms;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class WaitCursorController : Controller {
		private Cursor savedCursor;
		private void UnsubscribeFromEvents() {
			foreach(Controller controller in Frame.Controllers) {
				foreach(ActionBase action in controller.Actions) {
					action.Disposed -= new EventHandler(action_Disposed);
					action.Executing -= new System.ComponentModel.CancelEventHandler(action_Executing);
					action.Executed -= new EventHandler<ActionBaseEventArgs>(action_Executed);
					action.HandleException -= new EventHandler<HandleExceptionEventArgs>(action_HandleException);
					action.ExecuteCanceled -= new EventHandler<ActionBaseEventArgs>(action_ExecuteCanceled);
				}
			}
		}
		private void action_Disposed(object sender, EventArgs e) {
			ActionBase action = (ActionBase)sender;
			action.Disposed -= new EventHandler(action_Disposed);
			action.Executing -= new System.ComponentModel.CancelEventHandler(action_Executing);
			action.Executed -= new EventHandler<ActionBaseEventArgs>(action_Executed);
			action.HandleException -= new EventHandler<HandleExceptionEventArgs>(action_HandleException);
			action.ExecuteCanceled -= new EventHandler<ActionBaseEventArgs>(action_ExecuteCanceled);
		}
		private void SubscribeToEvents() {
			foreach(Controller controller in Frame.Controllers) {
				foreach(ActionBase action in controller.Actions) {
					action.Disposed += new EventHandler(action_Disposed);
					action.Executing += new System.ComponentModel.CancelEventHandler(action_Executing);
					action.Executed += new EventHandler<ActionBaseEventArgs>(action_Executed);
					action.HandleException += new EventHandler<HandleExceptionEventArgs>(action_HandleException);
					action.ExecuteCanceled += new EventHandler<ActionBaseEventArgs>(action_ExecuteCanceled);
				}
			}
		}
		private void Frame_ViewChanging(object sender, ViewChangingEventArgs e) {
			UnsubscribeFromEvents();
		}
		private void Frame_ViewChanged(object sender, EventArgs e) {
			SubscribeToEvents();
		}
		private void SetWaitCursor() {
			savedCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
		}
		private void RestoreCursor() {
			Cursor.Current = savedCursor;
		}
		private void action_Executing(object sender, System.ComponentModel.CancelEventArgs e) {
			SetWaitCursor();
		}
		private void action_Executed(object sender, ActionBaseEventArgs e) {
			RestoreCursor();
		}
		private void action_ExecuteCanceled(object sender, ActionBaseEventArgs e) {
			RestoreCursor();
		}
		private void action_HandleException(object sender, HandleExceptionEventArgs e) {
			RestoreCursor();
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(Frame.View != null) {
				SubscribeToEvents();
			}
			Frame.ViewChanging += new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
		}
		protected override void OnDeactivated() {
			UnsubscribeFromEvents();
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			Frame.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			if(savedCursor != null) {
				savedCursor.Dispose();
				savedCursor = null;
			}
			base.OnDeactivated();
		}
	}
}
