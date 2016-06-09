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
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
namespace DevExpress.ExpressApp.Web {
	public interface ISupportCallbackStartupScriptRegistering {
		event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;
	}
	public class RegisterCallbackStartupScriptEventArgs : EventArgs {
		private string startupScript;
		public string StartupScript {
			get { return startupScript; }
			set { startupScript = value; }
		}
	}
	public class ASPxTreeListSupportCallbackStartupScriptRegisteringImpl : ASPxTreeControlSupportCallbackStartupScriptRegisteringImplBase {
		private ASPxTreeList treeList;
		private void treeList_CustomJSProperties(object sender, TreeListCustomJSPropertiesEventArgs e) {
		   ProcessCustomJSProperties(e);
		}
		public ASPxTreeListSupportCallbackStartupScriptRegisteringImpl(ASPxTreeList treeList) {
			this.treeList = treeList;
			treeList.ClientSideEvents.EndCallback = GetEndCallbackHandler();
			treeList.CustomJSProperties += new TreeListCustomJSPropertiesEventHandler(treeList_CustomJSProperties);
		}
		public override void Dispose() {
			if(treeList != null) {
				treeList.CustomJSProperties -= new TreeListCustomJSPropertiesEventHandler(treeList_CustomJSProperties);
				treeList = null;
			}
			base.Dispose();
		}
		public ASPxTreeList TreeList {
			get { return treeList; }
		}
	}
	public class ASPxTreeViewSupportCallbackStartupScriptRegisteringImpl : ASPxTreeControlSupportCallbackStartupScriptRegisteringImplBase {
		private ASPxTreeView treeView;
		private void treeList_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e) {
			ProcessCustomJSProperties(e);
		}
		public ASPxTreeViewSupportCallbackStartupScriptRegisteringImpl(ASPxTreeView treeView) {
			this.treeView = treeView;
			treeView.ClientSideEvents.EndCallback = GetEndCallbackHandler();
			treeView.CustomJSProperties += new CustomJSPropertiesEventHandler(treeList_CustomJSProperties);
		}
		public override void Dispose() {
			if(treeView != null) {
				treeView.CustomJSProperties -= new CustomJSPropertiesEventHandler(treeList_CustomJSProperties);
				treeView = null;
			}
			base.Dispose();
		}
		public ASPxTreeView TreeView {
			get { return treeView; }
		}
	}
	public class ASPxTreeControlSupportCallbackStartupScriptRegisteringImplBase : ISupportCallbackStartupScriptRegistering, IDisposable {
		private const string CallbackStartupScriptPropertyName = @"cpCallbackStartupScript";
		protected string GetEndCallbackHandler() {
			return string.Format(
@"function(s, e) {{
	ProcessMarkup(s, true);
	if(s.{0}) {{
		eval(s.{0});
	}}
}}", CallbackStartupScriptPropertyName);
		}
		protected virtual void ProcessCustomJSProperties(CustomJSPropertiesEventArgs e) {
			RegisterCallbackStartupScriptEventArgs args = new RegisterCallbackStartupScriptEventArgs();
			OnRegisterCallbackStartupScript(args);
			e.Properties[CallbackStartupScriptPropertyName] = args.StartupScript;
		}
		protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e) {
			if(RegisterCallbackStartupScript != null) {
				RegisterCallbackStartupScript(this, e);
			}
		}
		public virtual void Dispose() {
			RegisterCallbackStartupScript = null;
		}
		public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;
	}
	public class ASPxGridViewSupportCallbackStartupScriptRegisteringImpl : ISupportCallbackStartupScriptRegistering, IDisposable {
		private const string CallbackStartupScriptPropertyName = @"cpCallbackStartupScript";
		private ASPxGridView grid;
		private void grid_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e) {
			RegisterCallbackStartupScriptEventArgs args = new RegisterCallbackStartupScriptEventArgs();
			OnRegisterCallbackStartupScript(args);
			e.Properties[CallbackStartupScriptPropertyName] = args.StartupScript;
		}
		private string GetEndCallbackScript() {
			return string.Format(
@"function(s,e) {{
	var endCallbackHandlers = s.{0}.split(';');
	for(var i = 0; i < endCallbackHandlers.length; i++) {{
		eval(s[endCallbackHandlers[i]]);
	}}
}}", ASPxGridListEditor.EndCallbackHandlers);
		}
		private string GetEndCallbackHandlers() {
			string endCallbackHandlers = null;
			if(grid.JSProperties.ContainsKey(ASPxGridListEditor.EndCallbackHandlers)) {
				endCallbackHandlers = (string)grid.JSProperties[ASPxGridListEditor.EndCallbackHandlers];
			}
			if(string.IsNullOrEmpty(endCallbackHandlers)) {
				endCallbackHandlers = "";
			}
			else if(!endCallbackHandlers.EndsWith(";")) {
				endCallbackHandlers += ";";
			}
			endCallbackHandlers += "cpEndCallbackHandlerCallbackStartupScript";
			return endCallbackHandlers;
		}
		protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e) {
			if(RegisterCallbackStartupScript != null) {
				RegisterCallbackStartupScript(this, e);
			}
		}
		public ASPxGridViewSupportCallbackStartupScriptRegisteringImpl(ASPxGridView grid) {
			this.grid = grid;
			grid.CustomJSProperties += new ASPxGridViewClientJSPropertiesEventHandler(grid_CustomJSProperties);
			grid.ClientSideEvents.EndCallback = GetEndCallbackScript();
			grid.JSProperties[ASPxGridListEditor.EndCallbackHandlers] = GetEndCallbackHandlers();
			grid.JSProperties["cpEndCallbackHandlerCallbackStartupScript"] = string.Format("if(s.{0}) {{ window.setTimeout(s.{0}, 1); }}", CallbackStartupScriptPropertyName);
		}
		public void Dispose() {
			if(grid != null) {
				grid.CustomJSProperties -= new ASPxGridViewClientJSPropertiesEventHandler(grid_CustomJSProperties);
				grid = null;
			}
			RegisterCallbackStartupScript = null;
		}
		public ASPxGridView Grid {
			get { return grid; }
		}
		public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;
	}
	public class CallbackStartupScriptController : Controller {
		private List<ISupportCallbackStartupScriptRegistering> subscribedContainers = new List<ISupportCallbackStartupScriptRegistering>();
		private void SubscribeToListEditor() {
			if(Frame.View is ListView) {
				ISupportCallbackStartupScriptRegistering listEditor = ((ListView)Frame.View).Editor as ISupportCallbackStartupScriptRegistering;
				if(listEditor != null) {
					listEditor.RegisterCallbackStartupScript += new EventHandler<RegisterCallbackStartupScriptEventArgs>(RegisterCallbackStartupScript);
				}
			}
		}
		private void UnsubscribeFromListEditor() {
			if(Frame.View is ListView) {
				ISupportCallbackStartupScriptRegistering listEditor = ((ListView)Frame.View).Editor as ISupportCallbackStartupScriptRegistering;
				if(listEditor != null) {
					listEditor.RegisterCallbackStartupScript -= new EventHandler<RegisterCallbackStartupScriptEventArgs>(RegisterCallbackStartupScript);
				}
			}
		}
		private void UnsubscribeFromContainers() {
			foreach(ISupportCallbackStartupScriptRegistering container in subscribedContainers) {
				container.RegisterCallbackStartupScript -= new EventHandler<RegisterCallbackStartupScriptEventArgs>(RegisterCallbackStartupScript);
			}
			subscribedContainers.Clear();
		}
		private void Frame_ProcessActionContainer(object sender, ProcessActionContainerEventArgs e) {
			ISupportCallbackStartupScriptRegistering callbackStartupScriptRegistering = e.ActionContainer as ISupportCallbackStartupScriptRegistering;
			if(callbackStartupScriptRegistering != null) {
				callbackStartupScriptRegistering.RegisterCallbackStartupScript += new EventHandler<RegisterCallbackStartupScriptEventArgs>(RegisterCallbackStartupScript);
				subscribedContainers.Add(callbackStartupScriptRegistering);
			}
		}
		private void Frame_TemplateChanging(object sender, EventArgs e) {
			UnsubscribeFromContainers();
		}
		private void Frame_ViewChanging(object sender, ViewChangingEventArgs e) {
			UnsubscribeFromListEditor();
		}
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			SubscribeToListEditor();
		}
		private void RegisterCallbackStartupScript(object sender, RegisterCallbackStartupScriptEventArgs e) {
			string resultScript = "";
			WebWindow currentWindow = WebWindow.CurrentRequestWindow;
			if(currentWindow != null) {
				foreach(string script in currentWindow.StartUpScripts.Values) {
					resultScript += script + ";\r\n";
				}
				currentWindow.StartUpScripts.Clear();
			}
			e.StartupScript = resultScript;
		}
		protected override void OnActivated() {
			base.OnActivated();
			SubscribeToListEditor();
			Frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			Frame.ViewChanging += new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			Frame.TemplateChanging += new EventHandler(Frame_TemplateChanging);
			Frame.ProcessActionContainer += new EventHandler<ProcessActionContainerEventArgs>(Frame_ProcessActionContainer);
		}
		protected override void OnDeactivated() {
			UnsubscribeFromListEditor();
			UnsubscribeFromContainers();
			Frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			Frame.ViewChanging -= new EventHandler<ViewChangingEventArgs>(Frame_ViewChanging);
			Frame.TemplateChanging -= new EventHandler(Frame_TemplateChanging);
			Frame.ProcessActionContainer -= new EventHandler<ProcessActionContainerEventArgs>(Frame_ProcessActionContainer);
			base.OnDeactivated();
		}
	}
}
