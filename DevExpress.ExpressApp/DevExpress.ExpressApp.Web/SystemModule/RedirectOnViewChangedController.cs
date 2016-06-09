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
using System.Web;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class RedirectOnViewChangedController : Controller {
		private const string ActiveKeyNotWindow = "NotWindow";
		[DefaultValue(false)]
		public static bool ActiveDefaultValue = false;
		private bool isViewChanged;
		private Page page;
		private void Instance_ControlsCreating(object sender, ControlsCreatingEventArgs e) {
			isViewChanged = false;
			page = e.Page;
			page.PreRender += new EventHandler(Page_PreRender);
			ISupportViewChanged supportViewChanged = page as ISupportViewChanged;
			if(supportViewChanged != null) {
				supportViewChanged.ViewChanged += new EventHandler<TemplateViewChangedEventArgs>(supportViewChanged_ViewChanged);
			}
		}
		private void supportViewChanged_ViewChanged(object sender, TemplateViewChangedEventArgs e) {
			isViewChanged = false; 
		}
		private void Page_PreRender(object sender, EventArgs e) {
			page = null;
			if(Frame != null && Frame.View != null && !Frame.View.IsRoot && Frame.View.IsControlCreated && isViewChanged) {
				if(HttpContext.Current != null) {
					WebApplication.Redirect(HttpContext.Current.Request.RawUrl);
				}
			}
		}
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			isViewChanged = true;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			((WebApplication)Application).ControlsCreating += new EventHandler<ControlsCreatingEventArgs>(Instance_ControlsCreating);
		}
		protected override void OnDeactivated() {
			WebApplication webApplication = ((WebApplication)Application);
			if(webApplication != null) {
				webApplication.ControlsCreating -= new EventHandler<ControlsCreatingEventArgs>(Instance_ControlsCreating);
			}
			if(page != null) {
				ISupportViewChanged supportViewChanged = page as ISupportViewChanged;
				if(supportViewChanged != null) {
					supportViewChanged.ViewChanged -= new EventHandler<TemplateViewChangedEventArgs>(supportViewChanged_ViewChanged);
				}
				page.PreRender -= new EventHandler(Page_PreRender);
				page = null;
			}
			base.OnDeactivated();
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			Active[ActiveKeyNotWindow] = !(Frame is Window); 
		}
		public RedirectOnViewChangedController() {
			Active["Default"] = ActiveDefaultValue;
			Active[ActiveKeyNotWindow] = false;
		}
	}
}
