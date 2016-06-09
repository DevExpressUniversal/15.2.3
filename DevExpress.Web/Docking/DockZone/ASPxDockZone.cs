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
using System.Text;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
namespace DevExpress.Web {
	public enum DockZoneOrientation { Vertical, Horizontal, Fill }
	[DXWebToolboxItem(true), DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxDockZone.bmp"),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxDockZone"),
	Designer("DevExpress.Web.Design.ASPxDockZoneDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DefaultProperty("ZoneUID")
	]
	public class ASPxDockZone : ASPxWebControl {
		protected internal const string ScriptResourceName = WebScriptsResourcePath + "DockZone.js";
		const string
			ClientObjectClassName = "ASPxClientDockZone",
			RaiseBeforeDockEventCommand = "EBD",
			RaiseAfterDockEventCommand = "EAD",
			BeforeDockEventName = "BeforeDock",
			AfterDockEventName = "AfterDock";
		DockPanelCollection panels = null;
		static readonly object EventBeforeDock = new object();
		static readonly object EventAfterDock = new object();
		public ASPxDockZone()
			: this(null) {
		}
		protected ASPxDockZone(ASPxWebControl owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneOrientation"),
#endif
		Category("Layout"), DefaultValue(DockZoneOrientation.Vertical), AutoFormatDisable]
		public DockZoneOrientation Orientation {
			get { return (DockZoneOrientation)GetEnumProperty("Orientation", DockZoneOrientation.Vertical); }
			set {
				SetEnumProperty("Orientation", DockZoneOrientation.Vertical, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZonePanelSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public Unit PanelSpacing {
			get { return GetUnitProperty("PanelSpacing", Unit.Empty); }
			set { SetEnumProperty("PanelSpacing", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneZoneUID"),
#endif
		Category("Docking"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ZoneUID {
			get { return GetStringProperty("ZoneUID", string.Empty); }
			set { SetStringProperty("ZoneUID", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneAllowGrowing"),
#endif
		Category("Docking"), DefaultValue(true), AutoFormatDisable]
		public bool AllowGrowing {
			get { return GetBoolProperty("AllowGrow", true); }
			set { SetBoolProperty("AllowGrow", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DockZoneStyles Styles {
			get { return (DockZoneStyles)StylesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZonePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((AppearanceStyleBase)ControlStyle).Paddings; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FontInfo Font {
			get { return base.Font; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Drawing.Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneBeforeDock"),
#endif
		Category("Behavior")]
		public event DockZoneCancelEventHandler BeforeDock
		{
			add { Events.AddHandler(EventBeforeDock, value); }
			remove { Events.RemoveHandler(EventBeforeDock, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneAfterDock"),
#endif
		Category("Behavior")]
		public event DockZoneEventHandler AfterDock
		{
			add { Events.AddHandler(EventAfterDock, value); }
			remove { Events.RemoveHandler(EventAfterDock, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public DockZoneClientSideEvents ClientSideEvents {
			get { return (DockZoneClientSideEvents)base.ClientSideEventsInternal; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DockPanelCollection Panels {
			get {
				if(this.panels == null)
					this.panels = new DockPanelCollection(this);
				return this.panels;
			}
		}
		protected override void OnInit(EventArgs e) {
			FillZoneUIDIfRequired();
			PanelZoneRelationsMediator.RegisterZone(this, Page);
			base.OnInit(e);
		}
		protected void FillZoneUIDIfRequired() {
			if (string.IsNullOrEmpty(ZoneUID))
				ZoneUID = ID;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDockZoneClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new DockZoneClientSideEvents();
		}
		protected internal virtual void OnBeforeDock(DockZoneCancelEventArgs e) {
			DockZoneCancelEventHandler handler = (DockZoneCancelEventHandler)Events[EventBeforeDock];
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void OnAfterDock(DockZoneEventArgs e) {
			DockZoneEventHandler handler = (DockZoneEventHandler)Events[EventAfterDock];
			if(handler != null)
				handler(this, e);
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			base.GetClientObjectAssignedServerEvents(eventNames);
			if(Events[EventBeforeDock] != null)
				eventNames.Add(BeforeDockEventName);
			if(Events[EventAfterDock] != null)
				eventNames.Add(AfterDockEventName);
		}
		public override bool IsClientSideAPIEnabled() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return Visible;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxDockZone), ScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return ClientObjectClassName;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName,
		   string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(localVarName + ".zoneUID='" + ZoneUID + "';\n");
			if(!AllowGrowing || Orientation == DockZoneOrientation.Fill)
				stb.Append(localVarName + ".allowGrow=false;\n");
			if(!Width.IsEmpty)
				stb.AppendFormat("{0}.initialWidth={1};\n", localVarName, UnitUtils.ConvertToPixels(Width).Value);
			if(!Height.IsEmpty)
				stb.AppendFormat("{0}.initialHeight={1};\n", localVarName, UnitUtils.ConvertToPixels(Height).Value);
			if(!PanelSpacing.IsEmpty)
				stb.AppendFormat("{0}.panelSpacing={1};\n", localVarName, UnitUtils.ConvertToPixels(PanelSpacing).Value);
			if(HasClientScriptStylesObject)
				stb.AppendFormat("{0}.CreateClientCssStyles({1});\n", localVarName, GetClientScriptStylesObject());
		}
		protected bool HasClientScriptStylesObject {
			get {
				return !Styles.DockingForbiddenStyle.IsEmpty || !Styles.DockingAllowedStyle.IsEmpty;
			}
		}
		protected override bool HasRenderCssFile() {
			return false;
		}
		protected string GetClientScriptStylesObject() {
			Hashtable stylesTable = new Hashtable();
			stylesTable["dfs"] = GetClientScriptStyleObject(Styles.DockingForbiddenStyle);
			stylesTable["das"] = GetClientScriptStyleObject(Styles.DockingAllowedStyle);
			return HtmlConvertor.ToJSON(stylesTable);
		}
		protected Hashtable GetClientScriptStyleObject(AppearanceStyleBase style) {
			Hashtable result = new Hashtable();
			result["className"] = style.CssClass;
			result["inlineStyle"] = style.GetStyleAttributes(this).Value;
			return result;
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
			}
		}
		protected void ProcessRaiseBeforeDockEventCommand(ArrayList args) {
			ASPxDockPanel panel = PanelZoneRelationsMediator.GetPanel(args[1] as string, Page);
			if(panel == null)
				return;
			DockZoneCancelEventArgs beforeDockEventArgs = new DockZoneCancelEventArgs(panel);
			OnBeforeDock(beforeDockEventArgs);
			if(beforeDockEventArgs.Cancel)
				return;
			ASPxDockManager manager = PanelZoneRelationsMediator.GetManager(Page);
			if(manager != null) {
				DockManagerCancelEventArgs managerBeforeDockEventArgs = new DockManagerCancelEventArgs(panel, this);
				manager.OnBeforeDock(managerBeforeDockEventArgs);
				if(managerBeforeDockEventArgs.Cancel)
					return;
			}
			panel.DockPanel((int)args[2], this);
		}
		protected void ProcessRaiseAfterDockEventCommand(ArrayList args) {
			ASPxDockPanel panel = PanelZoneRelationsMediator.GetPanel(args[1] as string, Page);
			if(panel == null)
				return;
			OnAfterDock(new DockZoneEventArgs(panel));
			ASPxDockManager manager = PanelZoneRelationsMediator.GetManager(Page);
			if(manager != null)
				manager.OnAfterDock(new DockManagerEventArgs(panel, this));
		}
		protected override StylesBase CreateStyles() {
			return new DockZoneStyles(this);
		}
		protected T GetStyle<T>(T defaultStyle, T userDefinedStyle) where T :
		  AppearanceStyleBase, new() {
			T style = new T();
			if(defaultStyle != null)
				style.CopyFrom(defaultStyle);
			if(userDefinedStyle != null)
				style.CopyFrom(userDefinedStyle);
			return style;
		}
		protected internal AppearanceSelectedStyle GetPanelPlaceholderStyle() {
			return GetStyle<AppearanceSelectedStyle>(Styles.GetDefaultPanelPlaceholderStyle(), Styles.PanelPlaceholder);
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new DockZoneControl(this));
		}
	}
}
