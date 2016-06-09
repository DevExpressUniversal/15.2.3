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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(true), DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation), NonVisualControl,
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxDockManager.bmp"),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxDockManager"),
	Designer("DevExpress.Web.Design.ASPxDockManagerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
	]
	public class ASPxDockManager : ASPxWebComponent {
		protected internal const string ScriptResourceName = WebScriptsResourcePath + "DockManager.js";
		const string
			ClientObjectClassName = "ASPxClientDockManager",
			RaiseBeforeDockEventCommand = "EBD",
			RaiseAfterDockEventCommand = "EAD",
			RaiseBeforeFloatEventCommand = "EBF",
			RaiseAfterFloatEventCommand = "EAF",
			BeforeDockEventName = "BeforeDock",
			AfterDockEventName = "AfterDock",
			BeforeFloatEventName = "BeforeFloat",
			AfterFloatEventName = "AfterFloat";
		bool layoutWasResetToInitial = false;
		string clientLayoutState = string.Empty;
		string callbackParameter;
		static readonly object EventBeforeDock = new object();
		static readonly object EventAfterDock = new object();
		static readonly object EventBeforeFloat = new object();
		static readonly object EventAfterFloat = new object();
		static readonly object CallbackEvent = new object();
		public ASPxDockManager()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies
		{
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID
		{
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerFreezeLayout"),
#endif
		Category("Docking"), DefaultValue(false), AutoFormatEnable]
		public bool FreezeLayout
		{
			get { return GetBoolProperty("FreezeLayout", false); }
			set { SetBoolProperty("FreezeLayout", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName
		{
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase Callback
		{
			add { Events.AddHandler(CallbackEvent, value); }
			remove { Events.RemoveHandler(CallbackEvent, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDockManagerClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerBeforeDock"),
#endif
		Category("Behavior")]
		public event DockManagerCancelEventHandler BeforeDock
		{
			add { Events.AddHandler(EventBeforeDock, value); }
			remove { Events.RemoveHandler(EventBeforeDock, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerAfterDock"),
#endif
		Category("Behavior")]
		public event DockManagerEventHandler AfterDock
		{
			add { Events.AddHandler(EventAfterDock, value); }
			remove { Events.RemoveHandler(EventAfterDock, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerBeforeFloat"),
#endif
		Category("Behavior")]
		public event DockManagerCancelEventHandler BeforeFloat
		{
			add { Events.AddHandler(EventBeforeFloat, value); }
			remove { Events.RemoveHandler(EventBeforeFloat, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerAfterFloat"),
#endif
		Category("Behavior")]
		public event DockManagerEventHandler AfterFloat
		{
			add { Events.AddHandler(EventAfterFloat, value); }
			remove { Events.RemoveHandler(EventAfterFloat, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<ASPxDockPanel> Panels {
			get { return PanelZoneRelationsMediator.GetPanels(Page); }
		}
		public ASPxDockPanel FindPanelByUID(string panelUID) {
			return PanelZoneRelationsMediator.GetPanel(panelUID, Page);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable<ASPxDockZone> Zones {
			get { return PanelZoneRelationsMediator.GetZones(Page); }
		}
		public ASPxDockZone FindZoneByUID(string zoneUID) {
			return PanelZoneRelationsMediator.GetZone(zoneUID, Page);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockManagerClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public DockManagerClientSideEvents ClientSideEvents
		{
			get { return (DockManagerClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new DockManagerClientSideEvents();
		}
		protected override void OnInit(EventArgs e) {
			if(!DesignMode)
				PanelZoneRelationsMediator.RegisterManager(this, Page);
			base.OnInit(e);
		}
		protected virtual void OnCallback(CallbackEventArgsBase e) {
			CallbackEventHandlerBase handler = Events[CallbackEvent] as CallbackEventHandlerBase;
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnBeforeDock(DockManagerCancelEventArgs e) {
			DockManagerCancelEventHandler handler = (DockManagerCancelEventHandler)Events[EventBeforeDock];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnAfterDock(DockManagerEventArgs e) {
			DockManagerEventHandler handler = (DockManagerEventHandler)Events[EventAfterDock];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnBeforeFloat(DockManagerCancelEventArgs e) {
			DockManagerCancelEventHandler handler = (DockManagerCancelEventHandler)Events[EventBeforeFloat];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnAfterFloat(DockManagerEventArgs e) {
			DockManagerEventHandler handler = (DockManagerEventHandler)Events[EventAfterFloat];
			if(handler != null)
				handler(this, e);
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			base.GetClientObjectAssignedServerEvents(eventNames);
			if(Events[EventBeforeDock] != null)
				eventNames.Add(BeforeDockEventName);
			if(Events[EventAfterDock] != null)
				eventNames.Add(AfterDockEventName);
			if(Events[EventBeforeFloat] != null)
				eventNames.Add(BeforeFloatEventName);
			if(Events[EventAfterFloat] != null)
				eventNames.Add(AfterFloatEventName);
		}
		protected internal override bool IsCallBacksEnabled() {
			return true;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			CallbackEventArgsBase args = new CallbackEventArgsBase(eventArgument);
			OnCallback(args);
			this.callbackParameter = args.Parameter;
		}
		public override bool IsClientSideAPIEnabled() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return Visible;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterPopupUtilsScripts();
			RegisterIncludeScript(typeof(ASPxDockManager), ScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return ClientObjectClassName;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!string.IsNullOrEmpty(ClientLayoutState))
				stb.Append(localVarName + ".clientLayoutState=" + ClientLayoutState + ";\n");
		}
		protected internal bool LayoutWasResetToInitial {
			get { return this.layoutWasResetToInitial; }
			set { this.layoutWasResetToInitial = true; }
		}
		protected string ClientLayoutState {
			get { return this.clientLayoutState; }
			set { this.clientLayoutState = value; }
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[UniqueID]); 
		}
		protected internal override string SaveClientState() {
			Hashtable state = new Hashtable();
			foreach(ASPxDockPanel panel in PanelZoneRelationsMediator.GetPanels(Page))
				state[panel.PanelUID] = panel.GetLayoutState();
			return HtmlConvertor.ToJSON(state, false, false, true);
		}
		protected internal override void LoadClientState(string serializedState) {
			Hashtable state = HtmlConvertor.FromJSON<Hashtable>(serializedState) as Hashtable;
			if(string.IsNullOrEmpty(serializedState))
				return;
			ClientLayoutState = serializedState;
			foreach(string panelUID in state.Keys) {
				ASPxDockPanel panel = PanelZoneRelationsMediator.GetPanel(panelUID, Page);
				if(panel != null)
					panel.ApplyLayoutState(state[panelUID] as IList);
			}
		}
		protected override void CreateControlHierarchy() {
			Control field = RenderUtils.CreateHiddenFieldLiteral(UniqueID, "FakeHiddenFieldValue");
			Controls.Add(field);
		}
		public void ResetLayoutToInitial() {
			LayoutWasResetToInitial = true;
			Hashtable panelInitialLayoutStateDictionary = 
				PanelZoneRelationsMediator.GetPanelInitialLayoutStateDictionary(Page);
			foreach(DictionaryEntry entry in panelInitialLayoutStateDictionary) {
				ASPxDockPanel panel = PanelZoneRelationsMediator.GetPanel(entry.Key as string, Page);
				if(panel != null) {
					IList state = panelInitialLayoutStateDictionary[entry.Key] as IList;
					panel.ApplyLayoutState(state);
					panel.LayoutWasResetToInitial = true;
				}
			}
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			base.RaisePostBackEvent(eventArgument);
			if(string.IsNullOrEmpty(eventArgument))
				return;
			ArrayList args = HtmlConvertor.FromJSON<ArrayList>(eventArgument);
			if(args == null)
				return;
			switch(args[0] as string) {
				case RaiseBeforeDockEventCommand:
					ProcessRaiseBeforeDockEventCommand(args);
					return;
				case RaiseAfterDockEventCommand:
					ProcessRaiseAfterDockEventCommand(args);
					return;
				case RaiseBeforeFloatEventCommand:
					ProcessRaiseBeforeFloatEventCommand(args);
					return;
				case RaiseAfterFloatEventCommand:
					ProcessRaiseAfterFloatEventCommand(args);
					return;
			}
		}
		protected void ProcessRaiseBeforeDockEventCommand(ArrayList args) {
			ASPxDockPanel panel = PanelZoneRelationsMediator.GetPanel(args[1] as string, Page);
			ASPxDockZone zone = PanelZoneRelationsMediator.GetZone(args[2] as string, Page);
			if(panel == null || zone == null)
				return;
			DockManagerCancelEventArgs beforeDockEventArgs = new DockManagerCancelEventArgs(panel, zone);
			OnBeforeDock(beforeDockEventArgs);
			if(beforeDockEventArgs.Cancel)
				return;
			panel.DockPanel((int)args[3], zone);
		}
		protected void ProcessRaiseAfterDockEventCommand(ArrayList args) {
			ASPxDockPanel panel = PanelZoneRelationsMediator.GetPanel(args[1] as string, Page);
			ASPxDockZone zone = PanelZoneRelationsMediator.GetZone(args[2] as string, Page);
			if(panel == null || zone == null)
				return;
			OnAfterDock(new DockManagerEventArgs(panel, zone));
		}
		protected void ProcessRaiseBeforeFloatEventCommand(ArrayList args) {
			ASPxDockPanel panel = PanelZoneRelationsMediator.GetPanel(args[1] as string, Page);
			ASPxDockZone zone = PanelZoneRelationsMediator.GetZone(args[2] as string, Page);
			if(panel == null || zone == null)
				return;
			DockManagerCancelEventArgs beforeFloatEventArgs = new DockManagerCancelEventArgs(panel, zone);
			OnBeforeFloat(beforeFloatEventArgs);
			if(beforeFloatEventArgs.Cancel)
				return;
			panel.MakePanelFloat();
			panel.RaiseAfterFloatEvent(args);
		}
		protected void ProcessRaiseAfterFloatEventCommand(ArrayList args) {
			ASPxDockPanel panel = PanelZoneRelationsMediator.GetPanel(args[1] as string, Page);
			ASPxDockZone zone = PanelZoneRelationsMediator.GetZone(args[2] as string, Page);
			if(panel == null || zone == null)
				return;
			OnAfterFloat(new DockManagerEventArgs(panel, zone));
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
	}
}
