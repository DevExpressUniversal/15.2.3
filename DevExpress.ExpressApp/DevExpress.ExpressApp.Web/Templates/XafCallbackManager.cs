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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.ExpressApp.Web.Templates {
	public interface IXafCallbackHandler {
		void ProcessAction(string parameter);
	}
	public interface ICallbackManagerHolder {
		XafCallbackManager CallbackManager { get; }
	}
	interface IXafCallbackManager {
		string GetScript(string handlerId, string parameter, string confirmation, bool usePostBack);
	}
	public class XafCallbackManager : IDisposable, IXafCallbackManager {
		public const string CallbackControlID = "globalCallbackControl";
		public const string ControlsToUpdate = @"cpControlsToUpdate";
		public const string ScriptContainerPanelId = "ScriptContainer";
		private class HybridASPxCallback : ASPxCallback {
			protected override void RaisePostBackEvent(string eventArgument) {
				base.RaisePostBackEvent(eventArgument);
				if(PostBack != null) {
					PostBack(this, new CallbackEventArgs(eventArgument));
				}
			}
			public event EventHandler<CallbackEventArgs> PostBack;
		}
		public const string pageTitle = @"cpPageTitle";
		private string pageCaption;
		private HybridASPxCallback callbackControl;
		private XafUpdatePanel scriptContainer;
		private List<XafUpdatePanel> updatePanels = new List<XafUpdatePanel>();
		private Dictionary<Control, IXafCallbackHandler> controlToCallbackHandlerMap = new Dictionary<Control, IXafCallbackHandler>();
		private Dictionary<string, IXafCallbackHandler> stringToCallbackHandlerMap = new Dictionary<string, IXafCallbackHandler>();
		private Dictionary<string, string> clientScripts = new Dictionary<string, string>();
		private void callbackControl_PostBack(object sender, CallbackEventArgs e) {
			ProcessAction(e.Parameter);
			RaisePreRenderEvents();
		}
		private void callbackControl_Callback(object source, CallbackEventArgs e) {
			ProcessAction(e.Parameter);
			if(!NeedEndResponse) {
				RaisePreRenderEvents();
				if(ProcessPreRender != null) {
					ProcessPreRender(this, EventArgs.Empty);
				}
				RegisterUpdatePanels(callbackControl.Page as IXafUpdatePanelsProvider, ref updatePanels);
				string cpControlsToUpdate = "";
				XafUpdatePanel scriptContainerPanel = null;
				foreach(XafUpdatePanel updatePanel in updatePanels) {
					if(updatePanel != null) {
						if(updatePanel.ID == ScriptContainerPanelId) {
							scriptContainerPanel = updatePanel;
						}
						else {
							cpControlsToUpdate = ProcessUpdatePanel(cpControlsToUpdate, updatePanel);
						}
					}
				}
				if(scriptContainerPanel != null) {
					RegisterClientScripts();
					cpControlsToUpdate = ProcessUpdatePanel(cpControlsToUpdate, scriptContainerPanel);
				}
				if(cpControlsToUpdate.Length > 0) {
					cpControlsToUpdate = cpControlsToUpdate.Substring(0, cpControlsToUpdate.Length - 1);
				}
				callbackControl.JSProperties[ControlsToUpdate] = cpControlsToUpdate;
				callbackControl.JSProperties[pageTitle] = pageCaption;
			}
		}
		private string ProcessUpdatePanel(string cpControlsToUpdate, XafUpdatePanel updatePanel) {
			PartialRenderingEventArgs eventArgs = new PartialRenderingEventArgs(updatePanel);
			if(PartialRendering != null) {
				PartialRendering(this, eventArgs);
			}
			if(!eventArgs.Cancel && !updatePanel.IsRendered) {
				string newRender = GetControlChildrenRenderResult(updatePanel);
				callbackControl.JSProperties["cp" + updatePanel.ClientID] = newRender;
				cpControlsToUpdate += updatePanel.ClientID + ";";
			}
			return cpControlsToUpdate;
		}
		private void ProcessAction(string parameter) {
			string[] parts = parameter.Split(new char[] { ':' }, 2);
			if(parts.Length > 0) {
				string handlerId = parts[0];
				IXafCallbackHandler handler = FindHandler(handlerId);
				if(handler != null) {
					handler.ProcessAction(parts.Length > 1 ? parts[1] : string.Empty);
				}
			}
		}
		private void RegisterClientScripts() {
			foreach(string clientScript in clientScripts.Values) {
				scriptContainer.Controls.Add(new LiteralControl(clientScript));
			}
			clientScripts.Clear();
		}
		private IXafCallbackHandler FindHandler(string handlerId) {
			IXafCallbackHandler result = null;
			if(stringToCallbackHandlerMap.ContainsKey(handlerId)) {
				result = stringToCallbackHandlerMap[handlerId];
			}
			else {
				Control control = callbackControl.Page.FindControl(handlerId);
				if(control != null) {
					if(control is IXafCallbackHandler) {
						result = (IXafCallbackHandler)control;
					}
					else if(controlToCallbackHandlerMap.ContainsKey(control)) {
						result = controlToCallbackHandlerMap[control];
					}
				}
				if(result == null) {
					if(stringToCallbackHandlerMap.ContainsKey(handlerId)) {
						result = stringToCallbackHandlerMap[handlerId];
					}
				}
			}
			return result;
		}
		private void OnPreRender() {
			if(PreRender != null) {
				PreRender(this, EventArgs.Empty);
			}
		}
		private void OnPreRenderInternal() {
			if(PreRenderInternal != null) {
				PreRenderInternal(this, EventArgs.Empty);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal void RaisePreRenderEvents() {
			OnPreRenderInternal();
			OnPreRender();
		}
		protected internal ICollection<IXafCallbackHandler> Handlers {
			get {
				List<IXafCallbackHandler> result = new List<IXafCallbackHandler>();
				result.AddRange(controlToCallbackHandlerMap.Values);
				result.AddRange(stringToCallbackHandlerMap.Values);
				return result;
			}
		}
		protected internal ReadOnlyCollection<XafUpdatePanel> UpdatePanels {
			get { return new ReadOnlyCollection<XafUpdatePanel>(updatePanels); }
		}
		protected internal void Clear() {
			controlToCallbackHandlerMap.Clear();
			stringToCallbackHandlerMap.Clear();
			updatePanels.Clear();
			clientScripts.Clear();
		}
		internal void RegisterClientScript(string scriptKey, string clientScript) {
			RegisterClientScript(scriptKey, clientScript, false);
		}
		internal void RegisterClientScript(string scriptKey, string clientScript, bool overrideScript) {
			if((!clientScripts.ContainsKey(scriptKey) || overrideScript) && !String.IsNullOrEmpty(clientScript)) {
				clientScripts[scriptKey] = RenderUtils.GetScriptHtml(clientScript);
			}
		}
		internal void RegisterClientScriptInclude(string scriptKey, string url) {
			if(!clientScripts.ContainsKey(scriptKey) && !String.IsNullOrEmpty(url)) {
				clientScripts.Add(scriptKey, RenderUtils.GetIncludeScriptHtml(url));
			}
		}
		internal void RegisterClientScriptResource(Type type, string resourceName) {
			Guard.ArgumentNotNull(callbackControl, "CallbackControl");
			Guard.ArgumentNotNull(callbackControl.Page, "The CallbackControl control is not added to Page.");
			RegisterClientScriptInclude(resourceName, callbackControl.Page.ClientScript.GetWebResourceUrl(type, resourceName));
		}
		public static void RegisterUpdatePanels(IXafUpdatePanelsProvider xafUpdatePanelsProvider, ref List<XafUpdatePanel> updatePanels) {
			if(xafUpdatePanelsProvider != null) {
				foreach(XafUpdatePanel panel in xafUpdatePanelsProvider.GetUpdatePanels()) {
					if(!updatePanels.Contains(panel)) {
						updatePanels.Add(panel);
					}
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<EventArgs> PreRenderInternal;
		public string GetScript() {
			return GetScript(false);
		}
		public string GetScript(bool usePostback) {
			return GetScript(string.Empty, string.Empty, string.Empty, usePostback);
		}
		public string GetScript(string handlerId, string parameter) {
			return GetScript(handlerId, parameter, string.Empty);
		}
		public string GetScript(string handlerId, string parameter, string confirmation) {
			return GetScript(handlerId, parameter, confirmation, false);
		}
		public string GetScript(string handlerId, string parameter, string confirmation, bool usePostBack) {
			if(confirmation != null && !confirmation.StartsWith("'")) {
				confirmation = ConfirmationsHelper.FormatConfirmationMessage(confirmation);
			}
			return GetScriptCore(handlerId, parameter, confirmation, null, usePostBack);
		}
		public string GetScript(string handlerId, string parameter, string confirmationExpression, string usePostBackExpression) {
			return GetScriptCore(handlerId, parameter, confirmationExpression, usePostBackExpression, false);
		}
		private string GetScriptCore(string handlerId, string parameter, string confirmationExpression, string usePostBackExpression, bool usePostBack) {
#if DebugTest
			if(CustomGetScript != null) {
				CustomGetScriptEventArgs args = new CustomGetScriptEventArgs(handlerId, parameter, confirmationExpression, usePostBack);
				CustomGetScript(this, args);
				if(args.Handled) {
					return args.Script;
				}
			}
#endif
			if(!handlerId.StartsWith("'")) {
				handlerId = "'" + RenderHelper.QuoteJScriptString(handlerId) + "'";
			}
			if(string.IsNullOrEmpty(parameter)) {
				parameter = "''";
			}
			ScriptCreatingEventArgs creatingEventArgs = new ScriptCreatingEventArgs(handlerId, parameter, confirmationExpression, usePostBack);
			if(ScriptCreating != null) {
				ScriptCreating(this, creatingEventArgs);
			}
			string usePostBackString = !string.IsNullOrEmpty(usePostBackExpression) ? usePostBackExpression : creatingEventArgs.UsePostBack.ToString().ToLower();
			string script = ScriptGenerator.GetScript(handlerId, creatingEventArgs.Parameters, creatingEventArgs.Confirmation, usePostBackString);
			if(ScriptCreated != null) {
				ScriptCreatedEventArgs createdEventArgs = new ScriptCreatedEventArgs(handlerId, creatingEventArgs.Parameters, creatingEventArgs.Confirmation, creatingEventArgs.UsePostBack, script);
				ScriptCreated(this, createdEventArgs);
				script = createdEventArgs.Script;
			}
			return script;
		}
		[Browsable(false)]
		public void RegisterHandler(Control control, IXafCallbackHandler handler) {
			if(controlToCallbackHandlerMap.ContainsKey(control)) {
				controlToCallbackHandlerMap[control] = handler;
			}
			else {
				controlToCallbackHandlerMap.Add(control, handler);
			}
		}
		public void RegisterHandler(string handlerId, IXafCallbackHandler handler) {
			if(stringToCallbackHandlerMap.ContainsKey(handlerId)) {
				stringToCallbackHandlerMap[handlerId] = handler;
			}
			else {
				stringToCallbackHandlerMap.Add(handlerId, handler);
			}
		}
		public void CreateControls(Page page) {
			if(page != null && page.Form != null) {
				callbackControl = new HybridASPxCallback();
				callbackControl.ID = CallbackControlID;
				callbackControl.ClientInstanceName = CallbackControlID;
				callbackControl.Callback += new CallbackEventHandler(callbackControl_Callback);
				callbackControl.PostBack += new EventHandler<CallbackEventArgs>(callbackControl_PostBack);
				callbackControl.ClientSideEvents.CallbackComplete = @"function(s, e) { ProcessCallbackResult(s); }";
				page.Form.Controls.Add(callbackControl);
				scriptContainer = new XafUpdatePanel();
				scriptContainer.ID = ScriptContainerPanelId;
				page.Form.Controls.Add(scriptContainer);
			}
		}
		public string GetCompactRenderResult(Control control) {
			using(StringWriter sw = new StringWriter(CultureInfo.InvariantCulture)) {
				using(HtmlTextWriter writer = new HtmlTextWriter(sw, string.Empty)) {
					writer.NewLine = string.Empty;
					control.RenderControl(writer);
				}
				return sw.ToString();
			}
		}
		public string GetControlChildrenRenderResult(Control control) {
			StringBuilder sb = new StringBuilder();
			int count = control.Controls.Count;
			for(int i = 0; i < count; i++)
				sb.Append(GetCompactRenderResult(control.Controls[i]));
			return sb.ToString();
		}
		public ASPxCallback CallbackControl {
			get { return callbackControl; }
		}
		public void SetPageCaption(string caption) {
			pageCaption = caption;
		}
		public event EventHandler<EventArgs> PreRender;
		public event EventHandler<ScriptCreatingEventArgs> ScriptCreating;
		public event EventHandler<ScriptCreatedEventArgs> ScriptCreated;
		public event EventHandler<PartialRenderingEventArgs> PartialRendering;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler ProcessPreRender;
		#region IDisposable Members
		public void Dispose() {
			if(callbackControl != null) {
				callbackControl.Callback -= new CallbackEventHandler(callbackControl_Callback);
				callbackControl.PostBack -= new EventHandler<CallbackEventArgs>(callbackControl_PostBack);
				callbackControl = null;
			}
			controlToCallbackHandlerMap.Clear();
			stringToCallbackHandlerMap.Clear();
		}
		#endregion
#if DebugTest
		public event EventHandler<CustomGetScriptEventArgs> CustomGetScript;
#endif
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool NeedEndResponse { get; set; }
	}
	public class ScriptCreatingEventArgs : EventArgs {
		private string handlerId;
		private string parameters;
		private string confirmation;
		private bool usePostBack;
		public ScriptCreatingEventArgs(string handlerId, string parameters, string confirmation, bool usePostBack) {
			this.handlerId = handlerId;
			this.parameters = parameters;
			this.confirmation = confirmation;
			this.usePostBack = usePostBack;
		}
		public string HandlerId {
			get { return handlerId; }
		}
		public string Parameters {
			get { return parameters; }
			set { parameters = value; }
		}
		public string Confirmation {
			get { return confirmation; }
			set { confirmation = value; }
		}
		public bool UsePostBack {
			get { return usePostBack; }
			set { usePostBack = value; }
		}
	}
	public class ScriptCreatedEventArgs : EventArgs {
		private string handlerId;
		private string parameters;
		private string confirmation;
		private bool usePostBack;
		private string script;
		public ScriptCreatedEventArgs(string handlerId, string parameters, string confirmation, bool usePostBack, string script) {
			this.handlerId = handlerId;
			this.parameters = parameters;
			this.confirmation = confirmation;
			this.usePostBack = usePostBack;
			this.script = script;
		}
		public string HandlerId {
			get { return handlerId; }
		}
		public string Parameters {
			get { return parameters; }
		}
		public string Confirmation {
			get { return confirmation; }
		}
		public bool UsePostBack {
			get { return usePostBack; }
		}
		public string Script {
			get { return script; }
			set { script = value; }
		}
	}
	public class PartialRenderingEventArgs : CancelEventArgs {
		private Control control;
		public PartialRenderingEventArgs(Control control) {
			this.control = control;
		}
		public Control Control {
			get { return control; }
		}
	}
	public class ScriptGenerator {
		public const string JSEmptyString = "''";
		public static string GetScript() {
			return GetScript(JSEmptyString, JSEmptyString, JSEmptyString, false);
		}
		public static string GetScript(bool usePostback) {
			return GetScript(JSEmptyString, JSEmptyString, JSEmptyString, usePostback);
		}
		public static string GetScript(string handlerId, string parameter) {
			return GetScript(handlerId, parameter, JSEmptyString, false);
		}
		public static string GetScript(string handlerId, string parameter, string confirmation) {
			return GetScript(handlerId, parameter, confirmation, false);
		}
		public static string GetScript(string handlerId, string parameter, string confirmation, bool usePostBack) {
			if(confirmation != null && !confirmation.StartsWith("'")) {
				confirmation = ConfirmationsHelper.FormatConfirmationMessage(confirmation);
			}
			return GetScript(handlerId, parameter, confirmation, usePostBack.ToString().ToLower());
		}
		public static string GetScript(string handlerId, string parameter, string confirmationExpression, string usePostBackExpression) {
			return GetScript(handlerId, parameter, confirmationExpression, usePostBackExpression, JSEmptyString);
		}
		public static string GetScript(string handlerId, string parameter, string confirmationExpression, string usePostBackExpression, string endCallbackHandler) {
			if(!handlerId.StartsWith("'")) {
				handlerId = "'" + RenderHelper.QuoteJScriptString(handlerId) + "'";
			}
			if(string.IsNullOrEmpty(parameter)) {
				parameter = "''";
			}
			string confirmationMsg = ConfirmationsHelper.IsConfirmationsEnabled ? confirmationExpression : "''";
			string scriptPattern = "";
			if(string.IsNullOrEmpty(endCallbackHandler) || endCallbackHandler == JSEmptyString) {
				scriptPattern = "RaiseXafCallback({0}, {1}, {2}, {3}, {4});";
			}
			else {
				scriptPattern = "RaiseXafCallback({0}, {1}, {2}, {3}, {4}, {5});";
			}
			return string.Format(scriptPattern, XafCallbackManager.CallbackControlID, handlerId, parameter, confirmationMsg, usePostBackExpression, endCallbackHandler);
		}
	}
#if DebugTest
	public class CustomGetScriptEventArgs : HandledEventArgs {
		public CustomGetScriptEventArgs(string handlerId, string parameter, string confirmation, bool usePostBack) {
			this.HandlerId = handlerId;
			this.Parameter = parameter;
			this.Confirmation = confirmation;
			this.UsePostBack = usePostBack;
		}
		public string Script { get; set; }
		public string HandlerId { get; private set; }
		public string Parameter { get; private set; }
		public string Confirmation { get; private set; }
		public bool UsePostBack { get; private set; }
	}
#endif
}
