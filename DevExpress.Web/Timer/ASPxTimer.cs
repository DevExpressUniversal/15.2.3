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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxTimer"), 
	DefaultProperty("Interval"), DefaultEvent("Tick"),
	Designer("DevExpress.Web.Design.ASPxTimerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabComponents),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxTimer.bmp")
	]
	public class ASPxTimer : ASPxWebComponent, IRequiresLoadPostDataControl {
		protected internal const string TimerScriptResourceName = WebScriptsResourcePath + "Timer.js";
		protected const int DefaultInterval = 60000;
		private static readonly object EventTick = new object();
		private TimerClientSideEvents fClientSideEvents = new TimerClientSideEvents();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimerInterval"),
#endif
		Category("Behavior"), DefaultValue(DefaultInterval), AutoFormatDisable]
		public int Interval {
			get { return GetIntProperty("Interval", DefaultInterval); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "Interval");
				SetIntProperty("Interval", DefaultInterval, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimerClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public TimerClientSideEvents ClientSideEvents {
			get { return (TimerClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimerClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxTimerEnabled")]
#endif
		public override bool Enabled { 
			get { return ClientEnabledInternal; }
			set { ClientEnabledInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimerEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable,
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The client-side API is always available for this control.")]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimerJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimerCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTimerTick"),
#endif
		Category("Action")]
		public event EventHandler Tick
		{
			add { Events.AddHandler(EventTick, value); }
			remove { Events.RemoveHandler(EventTick, value); }
		}
		public ASPxTimer()
			: base() {
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new TimerClientSideEvents();
		}
		protected override bool IsServerSideEventsAssigned() {
			return IsServerSideTickAssigned();
		}
		protected bool IsServerSideTickAssigned() {
			return HasEvents() && Events[EventTick] != null;
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || IsServerSideEventsAssigned();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxTimer), TimerScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (Interval != DefaultInterval)
				stb.Append(localVarName + ".interval = " + Interval + ";\n");
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasEvents() && Events[EventTick] != null)
				eventNames.Add("Tick");
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTimer";
		}
		protected virtual void OnTick(EventArgs e) {
			EventHandler handler = Events[EventTick] as EventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			if (eventArgument == "TICK") {
				OnTick(EventArgs.Empty);
			}
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			Enabled = GetClientObjectStateValue<bool>("enabled");
			Interval = GetClientObjectStateValue<int>("interval");
			return false;
		}
	}
}
