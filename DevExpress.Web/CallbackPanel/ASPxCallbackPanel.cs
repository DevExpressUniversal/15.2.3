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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(true), DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxCallbackPanel"), 
	DefaultProperty("ClientSideEvents"), DefaultEvent("Callback"),
	ToolboxData("<{0}:ASPxCallbackPanel Width=\"200px\" runat=\"server\"></{0}:ASPxCallbackPanel>"),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxCallbackPanel.bmp"),
	Designer("DevExpress.Web.Design.ASPxCallbackPanelDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation)
	]
	public class ASPxCallbackPanel : ASPxCollapsiblePanel {
		protected internal const string CallbackPanelScriptResourceName = WebScriptsResourcePath + "CallbackPanel.js";
		private static readonly object EventCallback = new object();
		public ASPxCallbackPanel()
			:
			base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(SettingsLoadingPanel.DefaultDelay), AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatDisable]
		public bool ShowLoadingPanel {
			get { return SettingsLoadingPanel.Enabled; }
			set { SettingsLoadingPanel.Enabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public new CallbackPanelClientSideEvents ClientSideEvents {
			get { return (CallbackPanelClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase Callback
		{
			add { Events.AddHandler(EventCallback, value); }
			remove { Events.RemoveHandler(EventCallback, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxCallbackPanelBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CallbackPanelImages Images {
			get { return (CallbackPanelImages)ImagesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the Images.LoadingPanel property instead.")]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CallbackPanelStyles Styles {
			get { return (CallbackPanelStyles)StylesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the Styles.LoadingPanel property instead.")]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelLoadingDivStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the Styles.LoadingDiv property instead.")]
		public new LoadingDivStyle LoadingDivStyle {
			get { return base.LoadingDivStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCallbackPanelHideContentOnCallback"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool HideContentOnCallback {
			get { return GetBoolProperty("HideContentOnCallback", false); }
			set {
				SetBoolProperty("HideContentOnCallback", false, value);
				LayoutChanged();
			}
		}
		[Obsolete("This property is now obsolete. Use the EnableCallbackAnimation property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool EnableAnimation {
			get { return EnableCallbackAnimation; }
			set { EnableCallbackAnimation = value; }
		}
		protected bool ShouldSerializeEnableAnimation() {
			return false;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
		protected override ImagesBase CreateImages() {
			return new CallbackPanelImages(this);
		}
		protected override StylesBase CreateStyles() {
			return new CallbackPanelStyles(this);
		}
		protected override bool HasLoadingPanel() {
			return !DesignMode || IsAutoFormatPreview;
		}
		protected override bool HasLoadingDiv() {
			return !DesignMode && !HideContentOnCallback;
		}
		protected internal override void PrepareLoadingPanel(LoadingPanelControl loadingPanelControl) {
			base.PrepareLoadingPanel(loadingPanelControl);
			if(IsAutoFormatPreview)
				loadingPanelControl.DesignModeVisible = true;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CallbackPanelClientSideEvents();
		}
		protected internal override bool IsCallBacksEnabled() {
			return true;
		}
		protected internal override bool IsCallbackAnimationEnabled() {
			return base.IsCallbackAnimationEnabled() && !HideContentOnCallback;
		}
		protected override bool IsServerSideEventsAssigned() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientCallbackPanel";
		}
		protected override bool HasPanelFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxCallbackPanel), CallbackPanelScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!HideContentOnCallback)
				stb.Append(localVarName + ".hideContentOnCallback=false;\n");
		}
		protected virtual void OnCallback(CallbackEventArgsBase e) {
			CallbackEventHandlerBase handler = Events[EventCallback] as CallbackEventHandlerBase;
			if(handler != null)
				handler(this, e);
		}
		protected override object GetCallbackResult() {
			return GetCallbackResultHtml();
		}
		protected virtual string GetCallbackResultHtml() {
			PanelContent panelContent = GetCallbackResultControl();
			if(panelContent == null) return string.Empty;
			BeginRendering();
			try {
				return RenderUtils.GetRenderResult(panelContent);
			}
			finally {
				EndRendering();
			}
		}
		protected virtual PanelContent GetCallbackResultControl() {
			return PanelContent;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			CallbackEventArgsBase args = new CallbackEventArgsBase(eventArgument);
			OnCallback(args);
		}
	}
}
