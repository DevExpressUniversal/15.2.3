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
using System.ComponentModel;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using DevExpress.Web.Design;
	[DXWebToolboxItem(DXToolboxItemKind.Free), DefaultProperty("TabPages"),
	ToolboxData("<{0}:ASPxPageControl runat=\"server\"></{0}:ASPxPageControl>"),
	Designer("DevExpress.Web.Design.ASPxPageControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxPageControl.bmp")
	]
	public class ASPxPageControl : ASPxTabControlBase, ICallbackEventHandler, IEndInitAccessorContainer, IControlDesigner {
		private int callbackTabIndex = -1;
		private static readonly object EventCallback = new object();
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlActivateTabPageAction"),
#endif
		Category("Behavior"), DefaultValue(ActivateTabPageAction.Click), AutoFormatDisable]
		public ActivateTabPageAction ActivateTabPageAction {
			get { return (ActivateTabPageAction)GetEnumProperty("ActivateTabPageAction", ActivateTabPageAction.Click); }
			set { SetEnumProperty("ActivateTabPageAction", ActivateTabPageAction.Click, value); }
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TabPage ActiveTabPage {
			get { return ActiveTabItem as TabPage; }
			set { ActiveTabItem = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlEnableCallBacks"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlEnableHierarchyRecreation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableHierarchyRecreation {
			get { return EnableHierarchyRecreationInternal; }
			set { EnableHierarchyRecreationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlShowTabs"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool ShowTabs {
			get { return ShowTabsInternal; }
			set {
				ShowTabsInternal = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlTabPages"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public TabPageCollection TabPages {
			get { return TabItems as TabPageCollection; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlSettingsLoadingPanel"),
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
	DevExpressWebLocalizedDescription("ASPxPageControlLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override string DataSourceID {
			get { return ""; }
			set { }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public override ITemplate ActiveTabTemplate {
			get { return base.ActiveTabTemplate; }
			set { base.ActiveTabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public override ITemplate TabTemplate {
			get { return base.TabTemplate; }
			set { base.TabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public override ITemplate ActiveTabTextTemplate {
			get { return base.ActiveTabTextTemplate; }
			set { base.ActiveTabTextTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public override ITemplate TabTextTemplate {
			get { return base.TabTextTemplate; }
			set { base.TabTextTemplate = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxPageControlClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPageControlCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase Callback
		{
			add { Events.AddHandler(EventCallback, value); }
			remove { Events.RemoveHandler(EventCallback, value); }
		}
		protected int CallbackTabIndex {
			get { return callbackTabIndex; }
		}
		protected internal override bool ShowTabsInternal {
			get { return GetBoolProperty("ShowTabsInternal", true); }
			set { SetBoolProperty("ShowTabsInternal", true, value); }
		}
		public ASPxPageControl()
			: base() {
		}
		protected override TabCollectionBase CreateTabItemsCollection() {
			return new TabPageCollection(this);
		}
		protected virtual void OnCallback(CallbackEventArgsBase e) {
			CallbackEventHandlerBase handler = Events[EventCallback] as CallbackEventHandlerBase;
			if(handler != null)
				handler(this, e);
		}
		protected override internal Paddings GetContentPaddings() {
			return GetContentStyle().Paddings;
		}
		protected override internal ActivateTabPageAction GetActivateTabPageAction() {
			return ActivateTabPageAction;
		}
		protected internal override bool IsCallBacksEnabled() {
			return true;
		}
		protected internal override bool IsLoadTabByCallbackInternal() {
			return EnableCallBacks && !AutoPostBack;
		}
		protected internal override void LoadClientState(string state) {
			ActiveTabIndex = int.Parse(state);
		}
		protected internal override string SaveClientState() {
			return ActiveTabIndex.ToString();
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return true;
		}
		protected override void CreateTabControlHierarchy() {
			fMainControl = CreatePageControl();
			Controls.Add(fMainControl);
			Controls.Add(RenderUtils.CreateClearElement());
			ClearChildViewState();
		}
		protected PageControlLite CreatePageControl() {
			return DesignMode ? new PageControlLiteDesignMode(this) : new PageControlLite(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientPageControl";
		}
		protected internal ContentControl GetPCContentControlForTabPage(TabPage tabPage) {
			return FindControl(GetContentControlID(tabPage)) as ContentControl;
		}
		protected internal override TabControlTemplateContainerBase CreateTemplateContainer(TabBase tab, bool active) {
			return new PageControlTemplateContainer(tab as TabPage, active);
		}
		protected override string GetContentStyleNamePrefix() {
			return "Page";
		}
		protected override object GetCallbackResult() {
			Hashtable result = new Hashtable();
			result[CallbackResultProperties.Html] = GetCallbackResultHtml();
			result[CallbackResultProperties.Index] = CallbackTabIndex;
			return result;
		}
		protected virtual string GetCallbackResultHtml() {
			string result = string.Empty;
			ContentControl contentControl = GetCallbackResultControl();
			if (contentControl != null) {
				contentControl.Visible = true;
				BeginRendering();
				try {
					result = RenderUtils.GetRenderResult(contentControl);
				}
				finally {
					EndRendering();
				}
			}
			return result;
		}
		protected virtual ContentControl GetCallbackResultControl() {
			if (0 <= CallbackTabIndex && CallbackTabIndex < TabPages.Count) {
				TabPage tab = TabPages[CallbackTabIndex];
				return GetPCContentControlForTabPage(tab);
			}
			return null;
		}
		internal virtual ContentControlLite CreateContentControl(int visibleIndex) {
			return new ContentControlLite(TabPages.GetVisibleTabPage(visibleIndex));
		}
		protected override object GetCallbackErrorData() {
			return CallbackTabIndex;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			string[] args = eventArgument.Split(new char[] { '|' }, StringSplitOptions.None);
			this.callbackTabIndex = int.Parse(args[0]);
			bool performCallback = args.Length > 1;
			if(performCallback) {
				CallbackEventArgsBase e = new CallbackEventArgsBase(args[1]);
				OnCallback(e);
			}
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			if (ViewStateLoaded != null)
				ViewStateLoaded(this, EventArgs.Empty);
		}
		internal bool IsNotBindingContainer { get; set; }
		protected internal event EventHandler ViewStateLoaded;
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.PageControlTabsOwner"; } }
	}
}
