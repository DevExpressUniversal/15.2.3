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
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum AllowedDockState { All, DockedOnly, FloatOnly }
	public enum LoadPanelContentViaCallback { None, OnFirstShow, OnPageLoad, OnDock, OnFloating, OnDockStateChange }
	[DXWebToolboxItem(true), ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxDockPanel.bmp"),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxDockPanel"),
	Designer("DevExpress.Web.Design.ASPxDockPanelDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DefaultProperty("PanelUID")
	]
	public class ASPxDockPanel : ASPxPopupControlBase, IControlDesigner {
		protected internal const string ScriptResourceName = WebScriptsResourcePath + "DockPanel.js";
		const string
			ClientObjectClassName = "ASPxClientDockPanel",
			StateHiddenFieldNamePostfix = "_SHF",
			RaiseBeforeDockEventCommand = "EBD",
			RaiseAfterDockEventCommand = "EAD",
			RaiseBeforeFloatEventCommand = "EBF",
			RaiseAfterFloatEventCommand = "EAF",
			BeforeDockEventName = "BeforeDock",
			AfterDockEventName = "AfterDock",
			BeforeFloatEventName = "BeforeFloat",
			AfterFloatEventName = "AfterFloat";
		bool layoutWasResetToInitial = false;
		string ownerZoneUIDFromMarkup = string.Empty;
		ASPxDockZone objOwnerZone = null;
		ForbiddenZoneCollection forbiddenZones = null;
		static readonly object EventBeforeDock = new object();
		static readonly object EventAfterDock = new object();
		static readonly object EventBeforeFloat = new object();
		static readonly object EventAfterFloat = new object();
		public ASPxDockPanel()
			: this(null) {
			base.CloseActionInternal = CloseAction.CloseButton;
			base.ModalInternal = false;
		}
		protected ASPxDockPanel(ASPxWebControl owner)
			: base(owner) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DataSourceID {
			get { return ""; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object DataSource {
			get { return null; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DataMember {
			get { return ""; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelVisibleIndex"),
#endif
		Category("Appearance"), DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int VisibleIndex
		{
			get { return GetIntProperty("VisibleIndex", 0); }
			set { SetIntProperty("VisibleIndex", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelLoadContentViaCallback"),
#endif
		Category("Behavior"), DefaultValue(LoadPanelContentViaCallback.None), AutoFormatDisable]
		public LoadPanelContentViaCallback LoadContentViaCallback {
			get {
				return string.IsNullOrEmpty(ContentUrl)	?
					(LoadPanelContentViaCallback)GetEnumProperty("LoadContentViaCallback", LoadPanelContentViaCallback.None) :
					LoadPanelContentViaCallback.None;
			}
			set { SetEnumProperty("LoadContentViaCallback", LoadPanelContentViaCallback.None, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelAllowDragging"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public override bool AllowDragging
		{
			get { return GetBoolProperty("AllowDragging", true); }
			set { SetBoolProperty("AllowDragging", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelShowOnPageLoad"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public override bool ShowOnPageLoad
		{
			get { return base.ShowOnPageLoad; }
			set { base.ShowOnPageLoad = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupControlImages Images
		{
			get { return (PopupControlImages)ImagesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupControlStyles Styles
		{
			get { return (PopupControlStyles)StylesInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { 
			get { return base.DisabledStyle; } 
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelAllowedDockState"),
#endif
		Category("Docking"), DefaultValue(AllowedDockState.All), AutoFormatDisable]
		public AllowedDockState AllowedDockState
		{
			get { return (AllowedDockState)GetEnumProperty("DockMode", AllowedDockState.All); }
			set { SetEnumProperty("DockMode", AllowedDockState.All, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelPanelUID"),
#endif
		Category("Docking"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string PanelUID
		{
			get { return GetStringProperty("PanelUID", string.Empty); }
			set { SetStringProperty("PanelUID", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelOwnerZoneUID"),
#endif
		Category("Docking"), DefaultValue(""), Localizable(false),
		Editor("DevExpress.Web.Design.OwnerZoneUIDEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)), AutoFormatDisable]
		public string OwnerZoneUID
		{
			get {
				if(!this.Initialized)
					return this.ownerZoneUIDFromMarkup;
				return PanelZoneRelationsMediator.GetPanelZoneID(PanelUID, Page); 
			}
			set
			{
				if (!Initialized)
					this.ownerZoneUIDFromMarkup = value;
				else
					PanelZoneRelationsMediator.AddRelation(PanelUID, value, Page);
			}
		}
		protected string LastDockedZoneUID {
			get { return GetStringProperty("LastDockedZoneUID", ""); }
			set { SetStringProperty("LastDockedZoneUID", "", value); }
		}
		protected int LastDockedVisibleIndex {
			get { return GetIntProperty("LastDockedVisibleIndex", 0); }
			set { SetIntProperty("LastDockedVisibleIndex", 0, value); }
		}
		protected int LastFloatLeft {
			get { return GetIntProperty("LastFloatLeft", 0); }
			set { SetIntProperty("LastFloatLeft", 0, value); }
		}
		protected int LastFloatTop {
			get { return GetIntProperty("LastFloatTop", 0); }
			set { SetIntProperty("LastFloatTop", 0, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxDockZone OwnerZone {
			get {
				if (!this.Initialized)
					return this.objOwnerZone;
				return PanelZoneRelationsMediator.GetPanelZone(PanelUID, Page); 
			}
			set {
				if (!Initialized)
					this.objOwnerZone = value;
				else
					PanelZoneRelationsMediator.AddRelation(PanelUID, value.ZoneUID, Page); 
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelForbiddenZones"),
#endif
		Category("Docking"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public ForbiddenZoneCollection ForbiddenZones
		{
			get
			{
				if (this.forbiddenZones == null)
					this.forbiddenZones = new ForbiddenZoneCollection(this);
				return this.forbiddenZones;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelBeforeDock"),
#endif
		Category("Behavior")]
		public event DockPanelCancelEventHandler BeforeDock {
			add { Events.AddHandler(EventBeforeDock, value); }
			remove { Events.RemoveHandler(EventBeforeDock, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelAfterDock"),
#endif
		Category("Behavior")]
		public event EventHandler AfterDock {
			add { Events.AddHandler(EventAfterDock, value); }
			remove { Events.RemoveHandler(EventAfterDock, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelBeforeFloat"),
#endif
		Category("Behavior")]
		public event DockPanelCancelEventHandler BeforeFloat {
			add { Events.AddHandler(EventBeforeFloat, value); }
			remove { Events.RemoveHandler(EventBeforeFloat, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelAfterFloat"),
#endif
		Category("Behavior")]
		public event EventHandler AfterFloat {
			add { Events.AddHandler(EventAfterFloat, value); }
			remove { Events.RemoveHandler(EventAfterFloat, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockPanelClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public DockPanelClientSideEvents ClientSideEvents
		{
			get { return (DockPanelClientSideEvents)base.ClientSideEventsInternal; }
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			SetupRelationWithZone();
		}
		protected void SetupRelationWithZone() {
			if (string.IsNullOrEmpty(PanelUID))
				PanelUID = ID;
			if(!string.IsNullOrEmpty(this.ownerZoneUIDFromMarkup))
				PanelZoneRelationsMediator.AddRelation(PanelUID, this.ownerZoneUIDFromMarkup, Page);
			if(this.objOwnerZone != null)
				PanelZoneRelationsMediator.AddRelation(PanelUID, this.objOwnerZone.ZoneUID, Page);
			PanelZoneRelationsMediator.RegisterPanel(this, Page);
		}
		protected internal bool LayoutWasResetToInitial {
			get { return this.layoutWasResetToInitial; }
			set { this.layoutWasResetToInitial = true; }
		}
		protected bool LayoutFreezed {
			get {
				ASPxDockManager manager = PanelZoneRelationsMediator.GetManager(Page);
				return manager != null && manager.FreezeLayout;
			}
		}
		protected internal virtual void OnBeforeDock(DockPanelCancelEventArgs e) {
			DockPanelCancelEventHandler handler = (DockPanelCancelEventHandler)Events[EventBeforeDock];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnAfterDock(EventArgs e) {
			EventHandler handler = (EventHandler)Events[EventAfterDock];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnBeforeFloat(DockPanelCancelEventArgs e) {
			DockPanelCancelEventHandler handler = (DockPanelCancelEventHandler)Events[EventBeforeFloat];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnAfterFloat(EventArgs e) {
			EventHandler handler = (EventHandler)Events[EventAfterFloat];
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
		protected override StylesBase CreateStyles() {
			return new DockPanelStyles(this);
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(LayoutWasResetToInitial)
				return true;
			if(ClientObjectState == null) return false;
			SyncState(GetClientObjectStateValue<ArrayList>("dockState"));
			return base.LoadPostData(postCollection);
		}
		protected object[] GetSerializedState() {
			return new object[] { OwnerZoneUID, VisibleIndex, LastDockedZoneUID, LastDockedVisibleIndex, LastFloatLeft, LastFloatTop };
		}
		protected void SyncState(ArrayList state) {
			if(state.Count != 6) return;
			OwnerZoneUID = state[0] as string;
			VisibleIndex = (int)state[1];
			LastDockedZoneUID = state[2] as string;
			LastDockedVisibleIndex = (int)state[3];
			LastFloatLeft = (int)state[4];
			LastFloatTop = (int)state[5];
		}
		protected internal object[] GetLayoutState() {
			return new object[] { 
				ShowOnPageLoad,
				AllowedDockState.ToString(),
				OwnerZoneUID,
				Width.ToString(),
				Height.ToString(),
				Left,
				Top,
				VisibleIndex
			};
		}
		protected internal void ApplyLayoutState(IList state) {
			if(state.Count != 8)
				return;
			ShowOnPageLoad = (bool)state[0];
			AllowedDockState = (AllowedDockState)Enum.Parse(typeof(AllowedDockState), state[1] as string);
			OwnerZoneUID = state[2] as string;
			Width = Unit.Parse(state[3] as string);
			Height = Unit.Parse(state[4] as string);
			Left = (int)state[5];
			Top = (int)state[6];
			VisibleIndex = (int)state[7];
		}
		protected internal override bool GetIsWindowContentVisible(PopupWindow window) {
			return DesignMode || LoadContentViaCallback == LoadPanelContentViaCallback.None ||
				DefaultWindow.GetIsClientContentLoadedOrVisible() ||
			   (WindowIndexCallbackRequested.HasValue && WindowIndexCallbackRequested.Value == DefaultWindow.Index);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ForbiddenZones });
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new DockPanelClientSideEvents();
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
					ProcessRaiseAfterDockEventCommand();
					return;
				case RaiseBeforeFloatEventCommand:
					ProcessRaiseBeforeFloatEventCommand(args);
					return;
				case RaiseAfterFloatEventCommand:
					ProcessRaiseAfterFloatEventCommand(args);
					return;
			}
		}
		protected void ProcessRaiseAfterDockEventCommand() {
			OnAfterDock(new EventArgs());
			if(OwnerZone != null)
				OwnerZone.OnAfterDock(new DockZoneEventArgs(this));
			ASPxDockManager manager = PanelZoneRelationsMediator.GetManager(Page);
			if(manager != null)
				manager.OnAfterDock(new DockManagerEventArgs(this, OwnerZone));
		}
		protected void ProcessRaiseBeforeDockEventCommand(ArrayList args) {
			ASPxDockZone newOwnerZone = PanelZoneRelationsMediator.GetZone(args[1] as string, Page);
			if(newOwnerZone == null)
				return;
			DockPanelCancelEventArgs beforeDockEventArgs = new DockPanelCancelEventArgs(newOwnerZone);
			OnBeforeDock(beforeDockEventArgs);
			if(beforeDockEventArgs.Cancel)
				return;
			DockZoneCancelEventArgs zoneBeforeDockEventArgs = new DockZoneCancelEventArgs(this);
			newOwnerZone.OnBeforeDock(zoneBeforeDockEventArgs);
			if(zoneBeforeDockEventArgs.Cancel)
				return;
			ASPxDockManager manager = PanelZoneRelationsMediator.GetManager(Page);
			if(manager != null) {
				DockManagerCancelEventArgs managerBeforeDockEventArgs = new DockManagerCancelEventArgs(this, newOwnerZone);
				manager.OnBeforeDock(managerBeforeDockEventArgs);
				if(managerBeforeDockEventArgs.Cancel)
					return;
			}
			DockPanel((int)args[2], newOwnerZone);
		}
		protected void ProcessRaiseBeforeFloatEventCommand(ArrayList args) {
			ASPxDockZone zone = PanelZoneRelationsMediator.GetZone(args[1] as string, Page);
			DockPanelCancelEventArgs beforeFloatEventArgs = new DockPanelCancelEventArgs(zone);
			OnBeforeFloat(beforeFloatEventArgs);
			if(beforeFloatEventArgs.Cancel) {
				DockPanel(VisibleIndex, zone);
				return;
			}
			ASPxDockManager manager = PanelZoneRelationsMediator.GetManager(Page);
			if(manager != null) {
				DockManagerCancelEventArgs managerBeforeFloatEventArgs = new DockManagerCancelEventArgs(this, zone);
				manager.OnBeforeFloat(managerBeforeFloatEventArgs);
				if(managerBeforeFloatEventArgs.Cancel) {
					DockPanel(VisibleIndex, zone);
					return;
				}
			}
			MakePanelFloat();
			RaiseAfterFloatEvent(args);
		}
		protected void ProcessRaiseAfterFloatEventCommand(ArrayList args) {
			ASPxDockZone zone = PanelZoneRelationsMediator.GetZone(args[1] as string, Page);
			OnAfterFloat(new EventArgs());
			ASPxDockManager manager = PanelZoneRelationsMediator.GetManager(Page);
			if(manager != null) 
				manager.OnAfterFloat(new DockManagerEventArgs(this, zone));
		}
		protected internal void DockPanel(int visibleIndex, ASPxDockZone newOwnerZone) {
			OwnerZoneUID = newOwnerZone.ZoneUID;
			VisibleIndex = visibleIndex;
			foreach(ASPxDockPanel panel in newOwnerZone.Panels) {
				if(panel.VisibleIndex >= VisibleIndex && panel != this)
					panel.VisibleIndex++;
			}
			ProcessRaiseAfterDockEventCommand();
		}
		protected internal void MakePanelFloat() {
			OwnerZoneUID = null;
		}
		protected internal void RaiseAfterFloatEvent(ArrayList args) {
			ProcessRaiseAfterFloatEventCommand(args);
		}
		public override bool IsClientSideAPIEnabled() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return Visible;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxDockZone), ASPxDockZone.ScriptResourceName);
			RegisterIncludeScript(typeof(ASPxDockPanel), ScriptResourceName);
			RegisterIncludeScript(typeof(ASPxDockManager), ASPxDockManager.ScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return ClientObjectClassName;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName,
		   string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(localVarName + ".panelUID='" + PanelUID + "';\n");
			stb.Append(localVarName + ".dockState=" + HtmlConvertor.ToJSON(GetSerializedState()) + ";\n");
			if(ForbiddenZones.Count != 0)
				stb.Append(localVarName + ".forbiddenZones=" + GetForbiddenZonesScript() + ";\n");
			if(AllowedDockState != AllowedDockState.All)
				stb.Append(localVarName + ".mode='" + AllowedDockState.ToString() + "';\n");
			if(LoadContentViaCallback != LoadPanelContentViaCallback.None)
				stb.AppendFormat("{0}.contentLoadingMode = '{1}';\n", localVarName, LoadContentViaCallback.ToString());
			if(LayoutFreezed)
				stb.Append(localVarName + ".requireFreezingLayout=true;\n");
			if(!Width.IsEmpty)
				stb.Append(localVarName + ".widthFixed=true;\n");
		}
		protected string GetForbiddenZonesScript() {
			List<string> forbiddenZoneUIDs = new List<string>();
			foreach(ForbiddenZoneItem item in ForbiddenZones)
				forbiddenZoneUIDs.Add(item.ZoneUID);
			return HtmlConvertor.ToJSON(forbiddenZoneUIDs);
		}
		protected override DefaultPopupWindow CreateDefaultWindow() {
			return new DockPanelDefaultWindow(this);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.DockPanelCommonFormDesigner"; } }
	}
}
namespace DevExpress.Web.Internal {
	public class DockPanelDefaultWindow : DefaultPopupWindow {
		public DockPanelDefaultWindow(ASPxDockPanel dockPanel) : base(dockPanel) {
		}
		[DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public override bool ShowOnPageLoad {
			get { return GetBoolProperty("ShowOnPageLoad", true); }
			set { SetBoolProperty("ShowOnPageLoad", true, value); }
		}
	}
}
