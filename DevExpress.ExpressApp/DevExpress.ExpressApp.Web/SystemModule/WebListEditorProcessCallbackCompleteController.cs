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
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using System.Web.UI;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public interface IProcessCallbackComplete {
		void ProcessCallbackComplete();
	}
	public class WebListEditorProcessCallbackCompleteController : ViewController<ListView> {
		private void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			if(View != null && View.Editor != null) {
				if(View.Editor.Control != null) {
					((IProcessCallbackComplete)View.Editor).ProcessCallbackComplete();
				}
				else {
					View.Editor.ControlsCreated += new EventHandler(ListEditor_ControlsCreated);
				}
			}
		}
		private void ListEditor_ControlsCreated(object sender, EventArgs e) {
			ListEditor editor = (ListEditor)sender;
			editor.ControlsCreated -= new EventHandler(ListEditor_ControlsCreated);
			((IProcessCallbackComplete)editor).ProcessCallbackComplete();
		}
		private XafCallbackManager GetCallbackManager(Page page) {
			Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), page.GetType(), "Page");
			return ((ICallbackManagerHolder)page).CallbackManager;
		}
		private void UnsubscribeFromEvents() {
			Page page = WebWindow.CurrentRequestPage;
			if(page != null) {
				GetCallbackManager(page).PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
				page.Unload -= new EventHandler(Page_Unload);
			}
		}
		private void Page_Unload(object sender, EventArgs e) {
			UnsubscribeFromEvents();
		}
		protected override void OnDeactivated() {
			UnsubscribeFromEvents();
			base.OnDeactivated();
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			Page page = WebWindow.CurrentRequestPage;
			if(View.Editor is IProcessCallbackComplete && page != null) {
				GetCallbackManager(page).PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
				page.Unload += new EventHandler(Page_Unload);
			}
		}
	}
}
