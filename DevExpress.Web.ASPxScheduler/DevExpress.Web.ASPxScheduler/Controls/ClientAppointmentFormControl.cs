#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Web.ASPxScheduler.Localization;
namespace DevExpress.Web.ASPxScheduler {
	#region ISupportClientInstanceName
	interface ISupportClientInstanceName {
		string ClientInstanceName { get; set; }
		string ActualClientInstanceName { get; }
	}
	#endregion
	#region SchedulerFormClientSideEvents
	public class SchedulerFormClientSideEvents : ClientSideEventsBase {
		#region FormClosed
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string FormClosed {
			get { return GetEventHandler("FormClosed"); }
			set { SetEventHandler("FormClosed", value); }
		}
		#endregion
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("FormClosed");
		}
	}
	#endregion
	public abstract class ASPxSchedulerFormWithClientScriptSupportBase: SchedulerUserControl, ISupportClientInstanceName {
		string clientInstanceName = String.Empty;
		Dictionary<String, Control> controlsMap;
		Dictionary<String, String> functionMap;
		Control[] childControls;
		SchedulerFormClientSideEvents clientSideEvents;
		Dictionary<string, object> jsProperties;
		protected ASPxSchedulerFormWithClientScriptSupportBase() {
			this.jsProperties = new Dictionary<string, object>();
		}
		public string ClientInstanceName { get { return clientInstanceName; } set { clientInstanceName = value; } }
		public string ActualClientInstanceName {
			get {
				if(!String.IsNullOrEmpty(ClientInstanceName))
					return ClientInstanceName;
				return ClientID;
			}
		}
		public virtual Control[] ChildControls {
			get {
				if(childControls == null)
					childControls = GetAllChildren();
				return childControls;
			}
		}
		public virtual string ClassName { get { return String.Empty; } }
		public Dictionary<String, Control> Mapping {
			get {
				if(controlsMap == null)
					this.controlsMap = new Dictionary<string, Control>();
				return controlsMap;
			}
		}
		public Dictionary<String, String> FunctionMap {
			get {
				if(functionMap == null)
					functionMap = new Dictionary<string, string>();
				return functionMap;
			}
		}
		[AutoFormatEnable, PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SchedulerFormClientSideEvents ClientSideEvents {
			get {
				if(clientSideEvents == null)
					this.clientSideEvents = CreateClientSideEvents();
				return clientSideEvents;
			}
		}
		public virtual Dictionary<string, object> JSProperties {
			get { return jsProperties; }
		}
		protected override void Render(HtmlTextWriter writer) {
			RegisterClientScript();
			if(!DesignMode && !IsMvcRender()) {
				ResourceManager.RenderScriptResources(Page, writer);
			}
			base.Render(writer);
			RenderUtils.WriteScriptHtml(writer, GetFormScript());
		}
		protected virtual void RegisterClientScript() {
		}
		string GetFormScript() {
			EnsureChildControls();
			if (!String.IsNullOrEmpty(ClassName)) {
				StringBuilder sb = new StringBuilder();
				RenderInitialBaseScript(sb, ActualClientInstanceName);
				RenderInitialScript(sb, ActualClientInstanceName);
				return sb.ToString();
			}
			return String.Empty;
		}
		private void RenderInitialBaseScript(StringBuilder sb, string instanceName) {
			sb.AppendFormat("var {0} = new {1}('{2}');\n", instanceName, ClassName, ActualClientInstanceName);
			sb.Append(ClientSideEvents.GetStartupScript(instanceName));
			BindToOwner(sb, instanceName);
			sb.AppendFormat("{0}.controls = new Object();\n", instanceName);
			BindScriptLocalization(sb, instanceName);
			BindControlsToClientSideObject(sb, instanceName);
		}
		private void BindScriptLocalization(StringBuilder sb, string instanceName) {
			SchedulerLocalizationCache localizationCache = new SchedulerLocalizationCache();
			PrepareLocalization(localizationCache);
			string json = DevExpress.Web.Internal.HtmlConvertor.ToJSON(localizationCache);
			sb.AppendFormat("{0}.localization = {1};\n", instanceName, json);
		}
		protected virtual void PrepareLocalization(SchedulerLocalizationCache localizationCache) { 
		}
		protected virtual void BindToOwner(StringBuilder sb, string instanceName) {			
		}
		protected virtual void RenderInitialScript(StringBuilder sb, string instanceName) {
			sb.AppendFormat("{0}.Initialize();\n", instanceName);
			if (Controls.Count > 0)
				sb.AppendFormat("{0}.linkToDOM = ASPx.GetElementById('{1}');\n", instanceName, Controls[0].ClientID);
		}
		void BindControlsToClientSideObject(StringBuilder sb, string clientInstanceName) {
			sb.AppendFormat("{0}.RefreshControlBindings = function() {{ \n", clientInstanceName);
			foreach(Control control in ChildControls) {
				if(!IsRenderedASPxWebControl(control))
					continue;
				string controlInstanceName = GetControlClientName(control);
				if(String.IsNullOrEmpty(controlInstanceName))
					sb.AppendFormat("this.controls.{0} = ASPx.GetElementById('{1}');\n", control.ID, control.ClientID);
				else {
					sb.AppendFormat("this.controls.{0} = ASPx.GetControlCollection().Get('{1}');\n", control.ID, controlInstanceName);
				}
			}
			sb.AppendFormat("\n }};");
			sb.AppendFormat("{0}.RefreshControlBindings();\n", clientInstanceName);
			GetCustomJSPropertiesScript(sb, clientInstanceName);
		}
		protected string GetControlClientName(Control control) {
			ISupportClientInstanceName instanceNameSupportControl = control as ISupportClientInstanceName;
			if(instanceNameSupportControl != null)
				return instanceNameSupportControl.ActualClientInstanceName;
			ASPxWebControl webControl = control as ASPxWebControl;
			if(webControl != null)
				return webControl.ClientID;
			return String.Empty;
		}
		bool IsRenderedASPxWebControl(Control control) {
			if (control == null)
				return false;
			if(!control.Visible)
				return false;
			ASPxWebControl webControl = control as ASPxWebControl;
			if(webControl != null)
				return webControl.Enabled && webControl.Visible;
			return true;
		}
		protected virtual Control[] GetAllChildren() {
			return GetChildControls();
		}
		protected virtual Control[] GetChildControls() {
			return new Control[] { };
		}
		SchedulerFormClientSideEvents CreateClientSideEvents() {
			return new SchedulerFormClientSideEvents();
		}
		public override Control FindControl(string id) {
			return base.FindControl(id);
		}
		public Control FindControlById(string id) {
			return FindControlHelper.LookupControl(this, id);
		}
		protected void RegisterIncludeScript(Type type, string resourceName) {
			ResourceManager.RegisterScriptResource(Page, type, resourceName);
		}
		protected void GetCustomJSPropertiesScript(StringBuilder stb, string localVarName) {
			IDictionary<string, object> properties = JSProperties;
			if (properties != null) {
				CheckCustomJSProperties(properties);
				foreach (string name in properties.Keys)
					stb.AppendFormat("{0}.{1}={2};\n", localVarName, name, HtmlConvertor.ToJSON(properties[name]));
			}
		}
		protected void CheckCustomJSProperties(IDictionary<string, object> properties) {
			foreach (string name in properties.Keys)
				CommonUtils.CheckCustomPropertyName(name);
		}
		internal WebControl ObtainDefaultButton() {
			return GetDefaultButton();
		}
		protected virtual WebControl GetDefaultButton() {
			return null;
		}
	}
	#region ASPxSchedulerClientScriptSupportUserControlBase
	public abstract class ASPxSchedulerClientScriptSupportUserControlBase : ASPxSchedulerFormWithClientScriptSupportBase, ISupportClientInstanceName {
		protected override void RegisterClientScript() {
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptCommonResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptClientAppointmentResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptGlobalFunctionsResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptViewInfosResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptAPIResourceName);
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptRecurrenceControlsResourceName);
		}
	}
	#endregion
	#region ASPxSchedulerClientFormBase
	public class ASPxSchedulerClientFormBase : ASPxSchedulerClientScriptSupportUserControlBase {
		string schedulerId;
		public string SchedulerId { get { return schedulerId; } set { schedulerId = value; } }
		protected override void RenderInitialScript(StringBuilder sb, string instanceName) {
			ASPxScheduler scheduler = FindControlHelper.LookupControl(this, SchedulerId) as ASPxScheduler;
			if(scheduler != null && scheduler.Visible) {
				string actualSchedulerClientInstanceName = (String.IsNullOrEmpty(scheduler.ClientInstanceName)) ? scheduler.ClientID : scheduler.ClientInstanceName;
				sb.AppendFormat("{0}.scheduler = {1};\n", instanceName, actualSchedulerClientInstanceName);
			}
			base.RenderInitialScript(sb, instanceName);
		}
		protected override void RegisterClientScript() {
			base.RegisterClientScript();
			RegisterIncludeScript(typeof(ASPxScheduler), ASPxScheduler.SchedulerScriptRecurrenceControlsResourceName);
		}
	}
	#endregion
	public class SchedulerLocalizationCache {
		public SchedulerLocalizationCache() {
			SchedulerLocalizer = new Dictionary<SchedulerStringId, string>();
			ASPxSchedulerLocalizer = new Dictionary<ASPxSchedulerStringId, string>();
		}
		public Dictionary<SchedulerStringId, string> SchedulerLocalizer { get; private set; }
		public Dictionary<ASPxSchedulerStringId, string> ASPxSchedulerLocalizer { get; private set; }
		public void Add(SchedulerStringId stringId) {
			SchedulerLocalizer.Add(stringId, DevExpress.XtraScheduler.Localization.SchedulerLocalizer.GetString(stringId));
		}
		public void Add(ASPxSchedulerStringId stringId) {
			ASPxSchedulerLocalizer.Add(stringId, DevExpress.Web.ASPxScheduler.Localization.ASPxSchedulerLocalizer.GetString(stringId));
		}
	}
}
