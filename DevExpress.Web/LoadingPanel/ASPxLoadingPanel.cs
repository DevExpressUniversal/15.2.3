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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxLoadingPanel"),
	Designer("DevExpress.Web.Design.ASPxLoadingPanelDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxLoadingPanel.bmp")
	]
	public class ASPxLoadingPanel : ASPxWebControl {
		protected internal const string LoadingPanelScriptResourceName = WebScriptsResourcePath + "LoadingPanel.js";
		private const string LoadingPanelTemplateContainerID = "LPTC";
		private ITemplate template = null;
		private static readonly object EventContainerElementResolve = new object();
		public ASPxLoadingPanel()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelImagePosition"),
#endif
		Category("Appearance"), DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition ImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelModal"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable, Localizable(false)]
		public bool Modal {
			get { return GetBoolProperty("Modal", false); }
			set {
				SetBoolProperty("Modal", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelHorizontalOffset"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int HorizontalOffset {
			get { return (int)GetIntProperty("HorizontalOffset", 0); }
			set { SetIntProperty("HorizontalOffset", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelVerticalOffset"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int VerticalOffset {
			get { return (int)GetIntProperty("VerticalOffset", 0); }
			set { SetEnumProperty("VerticalOffset", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelContainerElementID"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ContainerElementID {
			get { return GetStringProperty("ContainerElementID", ""); }
			set { SetStringProperty("ContainerElementID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelText"),
#endif
		DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string Text {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelShowImage"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public ClientSideEvents ClientSideEvents {
			get { return (ClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelImageFolder"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties Image {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(HorizontalAlign.NotSet), AutoFormatEnable]
		public HorizontalAlign HorizontalAlign {
			get { return ((AppearanceStyle)ControlStyle).HorizontalAlign; }
			set { ((AppearanceStyle)ControlStyle).HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelImageSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ImageSpacing {
			get { return ((AppearanceStyle)ControlStyle).ImageSpacing; }
			set { ((AppearanceStyle)ControlStyle).ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((AppearanceStyle)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelVerticalAlign"),
#endif
		Category("Layout"), DefaultValue(VerticalAlign.NotSet), AutoFormatEnable]
		public VerticalAlign VerticalAlign {
			get { return ((AppearanceStyle)ControlStyle).VerticalAlign; }
			set { ((AppearanceStyle)ControlStyle).VerticalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelLoadingDivStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new LoadingDivStyle LoadingDivStyle {
			get { return Styles.LoadingDivInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate Template {
			get { return template; }
			set {
				template = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxLoadingPanelContainerElementResolve"),
#endif
		Category("Events")]
		public event EventHandler<ControlResolveEventArgs> ContainerElementResolve
		{
			add { Events.AddHandler(EventContainerElementResolve, value); }
			remove { Events.RemoveHandler(EventContainerElementResolve, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
		protected LoadingPanelStyles Styles {
			get { return (LoadingPanelStyles)StylesInternal; }
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ClientSideEvents();
		}
		public new virtual Control FindControl(string id) {			
			if(Template != null) {
				TemplateContainerBase.FindTemplateControl(this, LoadingPanelTemplateContainerID, id);
			}
			return base.FindControl(id);
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected internal override void PrepareLoadingPanel(LoadingPanelControl loadingPanelControl) {
			base.PrepareLoadingPanel(loadingPanelControl);
			loadingPanelControl.DesignModeVisible = true;
			RenderUtils.AssignAttributes(this, loadingPanelControl); 
			loadingPanelControl.Width = Width;
			loadingPanelControl.Height = Height;
			if(!DesignMode)
				loadingPanelControl.StyleAttributes.Add("position", "absolute");
		}
		protected override bool HasLoadingPanel() {
			return true;
		}
		protected override bool HasLoadingDiv() {
			return Modal;
		}
		protected override string GetLoadingPanelID() {
			return string.Empty;
		}
		protected override TableCell CreateLoadingPanelTemplateCell(TableRow parent) {
			if(Template == null) return null;
			TableCell cell = RenderUtils.CreateTableCell();
			parent.Cells.Add(cell);
			TemplateContainerBase container = new TemplateContainerBase(0, this);
			container.AddToHierarchy(cell, LoadingPanelTemplateContainerID);
			Template.InstantiateIn(container);
			return cell;
		}
		protected bool CreateLoadingPanelTemplate(Control parent) {
			if(Template == null) return false;
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxLoadingPanel), LoadingPanelScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!string.IsNullOrEmpty(ContainerElementID))
				stb.AppendFormat("{0}.containerElementID='{1}';\n", localVarName,
					RenderUtils.GetReferentControlClientID(this, ContainerElementID, OnContainerElementResolve));
			if(HorizontalOffset != 0)
				stb.AppendFormat("{0}.horizontalOffset={1};\n", localVarName, HorizontalOffset.ToString());
			if(VerticalOffset != 0)
				stb.AppendFormat("{0}.verticalOffset={1};\n", localVarName, VerticalOffset.ToString());
			if(string.IsNullOrEmpty(Text))
				stb.Append(localVarName + ".isTextEmpty = true;\n");
			if(!ShowImage)
				stb.Append(localVarName + ".showImage = false;\n");
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientLoadingPanel";
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new LoadingPanelStyles(this);
		}
		protected override LoadingPanelStyle GetLoadingPanelStyle() {
			LoadingPanelStyle style = base.GetLoadingPanelStyle();
			style.CopyFrom(ControlStyle);
			style.CopyFrom(Styles.GetDefaultControlStyle());
			return style;
		}
		protected override LoadingDivStyle GetLoadingDivStyle() {
			LoadingDivStyle style = base.GetLoadingDivStyle();
			style.CopyFrom(Styles.GetDefaultControlStyle());
			return style;
		}
		protected void OnContainerElementResolve(ControlResolveEventArgs e) {
			EventHandler<ControlResolveEventArgs> handler = (EventHandler<ControlResolveEventArgs>)Events[EventContainerElementResolve];
			if(handler != null)
				handler(this, e);
		}
	}
}
