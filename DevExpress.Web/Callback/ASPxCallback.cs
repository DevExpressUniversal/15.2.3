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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	[DXWebToolboxItem(true), DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxCallback"), 
	NonVisualControl, DefaultProperty("ClientSideEvents"),
	Designer("DevExpress.Web.Design.ASPxCallbackDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull), 
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabComponents),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxCallback.bmp")
	]
	public class ASPxCallback : ASPxWebComponent {
		protected internal const string CallbackScriptResourceName = WebScriptsResourcePath + "Callback.js";
		private static readonly object CallbackEvent = new object();
		private string callbackParameter;
		private string callbackResult;
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public CallbackClientSideEvents ClientSideEvents {
			get { return (CallbackClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandler Callback
		{
			add { Events.AddHandler(CallbackEvent, value); }
			remove { Events.RemoveHandler(CallbackEvent, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		public ASPxCallback() 
			: base() {
		}
		public static string GetRenderResult(Control control) {
			return RenderUtils.GetRenderResult(control);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CallbackClientSideEvents();
		}
		protected internal override bool IsCallBacksEnabled() {
			return true;
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents() && Events[CallbackEvent] != null;
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientCallback";
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || IsServerSideEventsAssigned();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxCallback), CallbackScriptResourceName);
		}
		protected virtual void OnCallback(CallbackEventArgs e) {
			CallbackEventHandler handler = Events[CallbackEvent] as CallbackEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected override object GetCallbackResult() {
			Hashtable result = new Hashtable();
			result[CallbackCallbackResultProperties.Data] = this.callbackResult;
			result[CallbackCallbackResultProperties.Parameter] = this.callbackParameter;
			return result;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			CallbackEventArgs args = new CallbackEventArgs(eventArgument);
			OnCallback(args);
			this.callbackParameter = args.Parameter;
			this.callbackResult = args.Result;
		}
	}
}
namespace DevExpress.Web.Internal {
	public static class CallbackCallbackResultProperties {
		public const string Data = "data";
		public const string Parameter = "parameter";
	}
}
