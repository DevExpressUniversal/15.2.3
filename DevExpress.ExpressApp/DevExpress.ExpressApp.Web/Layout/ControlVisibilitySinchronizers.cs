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
using System.Web.UI;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Layout {
	internal abstract class ControlVisibilitySynchronizerBase<ControlType> : IDisposable where ControlType : class {
		private LayoutGroupTemplateContainer templateContainer;
		private ControlType control;
		private Page page;
		private void Subscribe() {
			if(page != null) {
				if(!page.IsCallback) {
					page.PreRenderComplete += new EventHandler(CurrentRequestPage_PreRenderComplete);
				}
				else if(page is ICallbackManagerHolder) {
					((ICallbackManagerHolder)page).CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
				}
			}
		}
		private void Unsubscribe() {
			if(page != null) {
				page.PreRenderComplete -= new EventHandler(CurrentRequestPage_PreRenderComplete);
				if(page is ICallbackManagerHolder) {
					((ICallbackManagerHolder)page).CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
				}
			}
		}
		private void CurrentRequestPage_PreRenderComplete(object sender, EventArgs e) {
			Unsubscribe();
			UpdateControlVisibility();
		}
		private void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			Unsubscribe();
			UpdateControlVisibility();
		}
		private void UpdateControlVisibility() {
			SetControlVisibility(control, templateContainer.Visibility);
		}
		protected abstract void SetControlVisibility(ControlType control, bool visible);
		public ControlVisibilitySynchronizerBase(LayoutGroupTemplateContainer templateContainer, ControlType control) {
			Guard.ArgumentNotNull(templateContainer, "templateContainer");
			Guard.ArgumentNotNull(control, "control");
			this.templateContainer = templateContainer;
			this.control = control;
			this.page = WebWindow.CurrentRequestPage;
			Subscribe();
			UpdateControlVisibility();
		}
		public void Dispose() {
			Unsubscribe();
			templateContainer = null;
			control = null;
			page = null;
		}
	}
	internal class LayoutGroupTemplateContainerVisibilitySynchronizer : ControlVisibilitySynchronizerBase<Control> {
		protected override void SetControlVisibility(Control control, bool visible) {
			control.Visible = visible;
		}
		public LayoutGroupTemplateContainerVisibilitySynchronizer(LayoutGroupTemplateContainer templateContainer, Control control) : base(templateContainer, control) { }
	}
	internal class TabPageVisibilitySynchronizer : ControlVisibilitySynchronizerBase<TabPage> {
		protected override void SetControlVisibility(TabPage control, bool visible) {
			control.Visible = visible;
		}
		public TabPageVisibilitySynchronizer(LayoutGroupTemplateContainer templateContainer, TabPage control) : base(templateContainer, control) { }
	}
}
