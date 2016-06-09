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
using System.Collections.Generic;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public interface ICustomRenderUpdatePanel {
		event EventHandler<GetCustomRenderResultsArgs> GetRenderResults;
	}
	public class GetCustomRenderResultsArgs : EventArgs {
		private Dictionary<string, object> renderResults;
		public GetCustomRenderResultsArgs() {
			renderResults = new Dictionary<string, object>();
		}
		public Dictionary<string, object> RenderResults {
			get { return renderResults; }
		}
	}
	public class ListEditorCustomRenderController : ViewController<ListView> {
		private static bool IsPanelRenderable(XafUpdatePanel panel) {
			return panel.UpdateAlways && panel.UpdatePanelForASPxGridListCallback;
		}
		void editor_GetRenderResults(object sender, GetCustomRenderResultsArgs e) {
			Page page = WebWindow.CurrentRequestPage;
			Control control = View.Editor.Control as Control;
			List<XafUpdatePanel> updatePanels = new List<XafUpdatePanel>();
			ICallbackManagerHolder callbackManagerHolder = page as ICallbackManagerHolder;
			if(callbackManagerHolder != null) {
				callbackManagerHolder.CallbackManager.RaisePreRenderEvents();
			}
			XafCallbackManager.RegisterUpdatePanels(page as IXafUpdatePanelsProvider, ref updatePanels);
			XafUpdatePanel viewSitePanel = GetViewSiteControlUpdatePanel(control);
			if(viewSitePanel == null && WebWindow.CurrentRequestWindow != null && WebWindow.CurrentRequestWindow.View != null) {
				viewSitePanel = GetViewSiteControlUpdatePanel((Control)WebWindow.CurrentRequestWindow.View.Control);
			}
			updatePanels.Remove(viewSitePanel);
			foreach(XafUpdatePanel panel in GetFilteredUpdatePanelsForRenderResults(updatePanels)) {
				e.RenderResults.Add(panel.ClientID, RenderUtils.GetControlChildrenRenderResult(panel));
			}
		}
		protected virtual List<XafUpdatePanel> GetFilteredUpdatePanelsForRenderResults(List<XafUpdatePanel> updatePanels) {
			return updatePanels.FindAll(IsPanelRenderable);
		}
		protected override void OnViewChanged() {
			base.OnViewChanged();
			if(Editor != null) {
				Editor.GetRenderResults += new EventHandler<GetCustomRenderResultsArgs>(editor_GetRenderResults);
			}
		}
		private XafUpdatePanel GetViewSiteControlUpdatePanel(Control control) {
			Control currentControl = control;
			Control topUpdatePanel = null;
			while(currentControl != null) {
				if(currentControl is XafUpdatePanel) {
					topUpdatePanel = currentControl;
				}
				currentControl = currentControl.Parent;
			}
			return topUpdatePanel as XafUpdatePanel;
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			if(Editor != null) {
				Editor.GetRenderResults -= new EventHandler<GetCustomRenderResultsArgs>(editor_GetRenderResults);
			}
		}
		private ICustomRenderUpdatePanel Editor {
			get {
				if(View == null) return null;
				return View.Editor as ICustomRenderUpdatePanel;
			}
		}
	}
}
