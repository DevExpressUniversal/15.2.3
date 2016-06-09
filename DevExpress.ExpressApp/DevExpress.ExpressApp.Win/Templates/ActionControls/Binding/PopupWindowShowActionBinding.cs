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
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Templates.ActionControls.Binding;
namespace DevExpress.ExpressApp.Win.Templates.ActionControls.Binding {
	public class PopupWindowShowActionBinding : ActionBinding {
		public static void Register() {
			ActionBindingFactory.Instance.Register("PopupWindowShowActionBinding", CanCreate, Create);
		}
		private static bool CanCreate(ActionBase action, IActionControl actionControl) {
			return action is PopupWindowShowAction && actionControl is ISimpleActionControl;
		}
		private static ActionBinding Create(ActionBase action, IActionControl actionControl) {
			return new PopupWindowShowActionBinding((PopupWindowShowAction)action, (ISimpleActionControl)actionControl);
		}
		private void ActionControl_Execute(object sender, EventArgs e) {
			ShowPopupWindow();
		}
		protected virtual void ShowPopupWindow() {
			using(PopupWindowShowActionHelper helper = new PopupWindowShowActionHelper(Action)) {
				helper.ShowPopupWindow();
			}
		}
		public PopupWindowShowActionBinding(PopupWindowShowAction action, ISimpleActionControl actionControl)
			: base(action, actionControl) {
			ActionControl.Execute += ActionControl_Execute;
		}
		public override void Dispose() {
			ActionControl.Execute -= ActionControl_Execute;
			base.Dispose();
		}
		public new PopupWindowShowAction Action {
			get { return (PopupWindowShowAction)base.Action; }
		}
		public new ISimpleActionControl ActionControl {
			get { return (ISimpleActionControl)base.ActionControl; }
		}
	}
}
