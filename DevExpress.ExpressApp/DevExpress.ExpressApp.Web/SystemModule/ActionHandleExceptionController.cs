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
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class ActionHandleExceptionListner : IDisposable {
		private ActionBase action;
		public ActionHandleExceptionListner(ActionBase action) {
			if(action != null) {
				this.action = action;
				this.action.HandleException += new EventHandler<HandleExceptionEventArgs>(action_HandleException);
			}
		}
		public void Dispose() {
			if(action != null) {
				action.HandleException -= new EventHandler<HandleExceptionEventArgs>(action_HandleException);
				action = null;
			}
		}
		private void action_HandleException(object sender, HandleExceptionEventArgs e) {
			ActionExceptionHandler.HandleException(e);
		}
	}
	public class ActionHandleExceptionController : Controller {
		List<ActionHandleExceptionListner> listeners = new List<ActionHandleExceptionListner>();
		protected override void OnActivated() {
			base.OnActivated();
			foreach(Controller controller in Frame.Controllers.Values) {
				foreach(ActionBase action in controller.Actions) {
					listeners.Add(new ActionHandleExceptionListner(action));
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(listeners != null) {
					foreach(IDisposable item in listeners) {
						item.Dispose();
					}
					listeners.Clear();
					listeners = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
