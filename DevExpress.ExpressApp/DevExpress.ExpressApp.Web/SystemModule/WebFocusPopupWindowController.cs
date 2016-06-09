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
using System.Web.UI;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebFocusPopupWindowController : WindowController {
		private void Window_TemplateViewChanged(object sender, EventArgs e) {
			ListView listView = Window.View as ListView;
			if(listView != null && listView.IsControlCreated) {
				Control control = (Control)listView.Editor.Control;
				if(control.Page != null) {
					if(control.Page.Form != null) {
						control.Page.Form.Focus();
					}
					else {
						control.Page.Load += new EventHandler(delegate(object page, EventArgs args) {
							((Page)page).Form.Focus();
						});
					}
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.TemplateViewChanged += new EventHandler(Window_TemplateViewChanged);
		}
		protected override void OnDeactivated() {
			Window.TemplateViewChanged -= new EventHandler(Window_TemplateViewChanged);
			base.OnDeactivated();
		}
		public WebFocusPopupWindowController() {
			TargetWindowType = WindowType.Child;
			Active["DisableInNewStyle"] = !WebApplicationStyleManager.IsNewStyle;  
		}
	}
}
