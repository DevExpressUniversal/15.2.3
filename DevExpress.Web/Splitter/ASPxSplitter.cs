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
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using System.Collections.Specialized;
	using DevExpress.Utils;
	using DevExpress.Web.Design;
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxSplitter"), 
	DefaultProperty("Panes"), DefaultEvent(""),
	Designer("DevExpress.Web.Design.ASPxSplitterDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxSplitter.bmp"),
	ToolboxData("<{0}:ASPxSplitter runat=\"server\"><Panes><{0}:SplitterPane></{0}:SplitterPane><{0}:SplitterPane></{0}:SplitterPane></Panes></{0}:ASPxSplitter>")
	]
	public class ASPxSplitter : ASPxWebControl, IRequiresLoadPostDataControl, IEndInitAccessorContainer, IControlDesigner {
		protected internal const string
			ScriptName = WebScriptsResourcePath + "Splitter.js";
		SplitterPane rootPane;
		SplitterRenderHelper renderHelper;
		SplitterControl rootControl;
		public ASPxSplitter()
			: this(null) {
		}
		protected internal ASPxSplitter(ASPxWebControl ownerControl)
			: base(ownerControl) {
			this.rootPane = new SplitterPane(this);
			this.renderHelper = new SplitterRenderHelper(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterEnableHierarchyRecreation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableHierarchyRecreation {
			get { return EnableHierarchyRecreationInternal; }
			set { EnableHierarchyRecreationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterPanes"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public SplitterPaneCollection Panes {
			get { return RootPane.Panes; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterOrientation"),
#endif
		Category("Layout"), DefaultValue(Orientation.Horizontal), AutoFormatDisable]
		public Orientation Orientation {
			get { return (Orientation)GetEnumProperty("Orientation", Orientation.Horizontal); }
			set {
				if(value == Orientation)
					return;
				SetEnumProperty("Orientation", Orientation.Horizontal, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterSeparatorVisible"),
#endif
		Category("Layout"), DefaultValue(true), AutoFormatDisable]
		public bool SeparatorVisible {
			get { return GetBoolProperty("SeparatorVisible", true); }
			set {
				if(value == SeparatorVisible)
					return;
				SetBoolProperty("SeparatorVisible", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterShowSeparatorImage"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatDisable]
		public bool ShowSeparatorImage {
			get { return GetBoolProperty("ShowSeparatorImage", true); }
			set { SetBoolProperty("ShowSeparatorImage", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterShowCollapseForwardButton"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ShowCollapseForwardButton {
			get { return GetBoolProperty("ShowCollapseForwardButton", false); }
			set { SetBoolProperty("ShowCollapseForwardButton", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterShowCollapseBackwardButton"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ShowCollapseBackwardButton {
			get { return GetBoolProperty("ShowCollapseBackwardButton", false); }
			set { SetBoolProperty("ShowCollapseBackwardButton", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterSeparatorSize"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public Unit SeparatorSize {
			get { return GetUnitProperty("SeparatorSize", Unit.Empty); }
			set {
				SplitterRenderHelper.CheckSizeType(value, false, false, false, "SeparatorSize");
				SetUnitProperty("SeparatorSize", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterPaneMinSize"),
#endif
		Category("Behavior"), DefaultValue(typeof(Unit), "40px"), AutoFormatDisable]
		public Unit PaneMinSize {
			get { return GetUnitProperty("PaneMinSize", Unit.Pixel(40)); }
			set {
				CommonUtils.CheckNegativeValue(value.Value, "PaneMinSize");
				SetUnitProperty("PaneMinSize", Unit.Pixel(40), value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterResizingMode"),
#endif
		Category("Behavior"), DefaultValue(ResizingMode.Postponed), AutoFormatDisable]
		public ResizingMode ResizingMode {
			get { return (ResizingMode)GetEnumProperty("ResizingMode", ResizingMode.Postponed); }
			set { SetEnumProperty("ResizingMode", ResizingMode.Postponed, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterAllowResize"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool AllowResize {
			get { return GetBoolProperty("AllowResize", true); }
			set { SetBoolProperty("AllowResize", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterFullscreenMode"),
#endif
Category("Layout"), DefaultValue(false), AutoFormatDisable]
		public bool FullscreenMode
		{
			get { return GetBoolProperty("FullscreenMode", false); }
			set
			{
				if (value == FullscreenMode)
					return;
				SetBoolProperty("FullscreenMode", false, value);
				if (value)
				{
					Width = Unit.Percentage(100);
					PropertyChanged("Width");
					Height = Unit.Percentage(100);
					PropertyChanged("Height");
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public SplitterClientSideEvents ClientSideEvents {
			get { return (SplitterClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), Localizable(false), AutoFormatDisable]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[Browsable(false), UrlProperty]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { get { return base.DisabledStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterStyles Styles {
			get { return (SplitterStyles)StylesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterImages Images {
			get { return (SplitterImages)ImagesInternal; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSplitterCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxSplitterClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		protected internal SplitterPane RootPane { get { return rootPane; } }
		protected internal SplitterRenderHelper RenderHelper { get { return renderHelper; } }
		protected override Style CreateControlStyle() {
			return new SplitterStyle();
		}
		protected override StylesBase CreateStyles() {
			return new SplitterStyles(this);
		}
		protected internal T InternalCreateStyle<T>(CreateStyleHandler handler, params object[] keys) where T : AppearanceStyleBase {
			return (T)base.CreateStyle(handler, keys);
		}
		protected override ImagesBase CreateImages() {
			return new SplitterImages(this);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new SplitterClientSideEvents();
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Panes });
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxSplitter), ScriptName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(Width != Unit.Percentage(100))
				stb.Append(localVarName + ".width = '" + Width.ToString() + "';\n");
			if(Height != Unit.Pixel(200))
				stb.Append(localVarName + ".height = '" + Height.ToString() + "';\n");
			if(ResizingMode == ResizingMode.Live)
				stb.Append(localVarName + ".liveResizing = true;\n");
			if(!AllowResize)
				stb.Append(localVarName + ".allowResize = false;\n");
			if(ShowSeparatorImage)
				stb.Append(localVarName + ".showSeparatorImage = true;\n");
			if(ShowCollapseBackwardButton)
				stb.Append(localVarName + ".showCollapseBackwardButton = true;\n");
			if(ShowCollapseForwardButton)
				stb.Append(localVarName + ".showCollapseForwardButton = true;\n");
			if(PaneMinSize != Unit.Pixel(5))
				stb.Append(String.Format(localVarName + ".defaultMinSize = {0};\n", UnitUtils.ConvertToPixels(PaneMinSize).Value));
			if(Orientation == Orientation.Horizontal)
				stb.Append(localVarName + ".rootPane.isVertical = true;\n");
			if(FullscreenMode)
				stb.Append(localVarName + ".fullScreen = true;\n");
			stb.Append(localVarName + ".CreatePanes(" + HtmlConvertor.ToJSON(RenderHelper.GetStateObject(RootPane.Panes, true), true) + ");\n");
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSplitter";
		}
		protected override bool HasHoverScripts() {
			return true;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			AddPaneHoverItems(helper, RootPane);
		}
		protected void AddPaneHoverItems(StateScriptRenderHelper helper, SplitterPane pane) {
			if(pane.Parent != null && pane.Index > 0) {
				object[] buttonImageObjects = new object[3];
				string[] buttonImagePostfixes = new string[3];
				for(int i = 0; i < 3; i++) {
					SplitterButtons buttonType = (SplitterButtons)i;
					buttonImageObjects[i] = RenderHelper.GetButtonImage(buttonType, pane).GetHottrackedScriptObject(Page);
					buttonImagePostfixes[i] = RenderHelper.GetButtonImageFullPostfix(buttonType);
				}
				AppearanceStyleBase separatorStyle = RenderHelper.GetSeparatorHoverStyle(pane);
				if(RenderHelper.IsButtonsVisible(pane)) {
					if(RenderHelper.IsBackwardForwardButtonsVisible(pane))
						helper.AddStyle(separatorStyle, RenderHelper.GetSeparatorID(pane), new string[0], buttonImageObjects, buttonImagePostfixes, IsEnabled());
					else
						helper.AddStyle(separatorStyle, RenderHelper.GetSeparatorID(pane), new string[0],
							buttonImageObjects[(int)SplitterButtons.Separator], buttonImagePostfixes[(int)SplitterButtons.Separator], IsEnabled());
				}
				else
					helper.AddStyle(separatorStyle, RenderHelper.GetSeparatorID(pane), true);
				if(RenderHelper.IsBackwardForwardButtonsVisible(pane)) {
					AppearanceStyleBase style = RenderHelper.GetSeparatorButtonHoverStyle(pane);
					foreach(SplitterButtons buttonType in new SplitterButtons[] { SplitterButtons.Backward, SplitterButtons.Forward })
						helper.AddStyle(style, RenderHelper.GetButtonID(buttonType, pane), new string[0],
							buttonImageObjects[(int)buttonType], RenderHelper.GetImageFullPostfix(), IsEnabled());
				}
			}
			foreach(SplitterPane child in pane.Panes.GetVisibleItems())
				AddPaneHoverItems(helper, child);
		}
		protected override bool HasSelectedScripts() {
			return true;
		}
		protected override void AddSelectedItems(StateScriptRenderHelper helper) {
			AddPaneSelectedItems(helper, RootPane);
		}
		protected void AddPaneSelectedItems(StateScriptRenderHelper helper, SplitterPane pane) {
			if(pane.Parent != null) {
				helper.AddStyle(RenderHelper.GetPaneCollapsedStyle(pane), RenderHelper.GetPaneID(pane), new string[0], IsEnabled());
				helper.AddStyle(RenderHelper.GetSeparatorCollapsedStyle(pane), RenderHelper.GetSeparatorID(pane), new string[0], IsEnabled());
			}
			foreach(SplitterPane child in pane.Panes.GetVisibleItems())
				AddPaneSelectedItems(helper, child);
		}
		protected override bool HasContent() {
			return HasVisiblePanes();
		}
		protected override void ClearControlFields() {
			this.rootControl = null;
		}
		protected override void CreateControlHierarchy() {
			this.rootControl = new SplitterControl(RootPane);
			Controls.Add(this.rootControl);
			if(!DesignMode && ResizingMode == ResizingMode.Postponed)
				Controls.Add(new ResizingPointerControl(this));
			ClearChildViewState();
		}
		protected override void PrepareControlHierarchy() {
			if(Width.IsEmpty)
				Width = Unit.Percentage(100);
			if(DesignMode)
				Height = Unit.Empty;
			else if(Height.IsEmpty)
				Height = Unit.Pixel(200);
			if(DesignMode)
				RenderHelper.PrepareChildrenDesignModeSize(this.RootPane);
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return true;
		}
		protected internal void PanesChanged() {
			if (!IsLoading()) {
				ResetViewStateStoringFlag();
				ResetControlHierarchy();
			}
		}
		protected void ApplyClientState(SplitterPaneCollection panes, ArrayList state) {
			for(int i = 0; i < panes.Count; i++)
				ApplyClientState(panes[i], (Hashtable)state[i]);
		}
		protected void ApplyClientState(SplitterPane pane, Hashtable state) {
			if(state.ContainsKey("st")) {
				string sizeType = (string)state["st"];
				if(sizeType == "px")
					pane.Size = Unit.Pixel(Convert.ToInt32(state["s"]));
				else
					pane.Size = Unit.Percentage(Convert.ToDouble(state["s"]));
			}
			if(state.ContainsKey("c"))
				pane.Collapsed = ((int)state["c"]) == 1;
			if(state.ContainsKey("spt"))
				pane.ScrollTop = (int)state["spt"];
			if(state.ContainsKey("spl"))
				pane.ScrollLeft = (int)state["spl"];
			if(state.ContainsKey("i")) {
				ArrayList childrenState = (ArrayList)state["i"];
				ApplyClientState(pane.Panes, childrenState);
			}
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			ArrayList clientState = GetClientObjectStateValue<ArrayList>("state");
			ApplyClientState(rootPane.Panes, clientState);
			return false;
		}
		protected internal override void LoadClientState(string state) {
			ApplyClientState(rootPane.Panes, HtmlConvertor.FromJSON<ArrayList>(state));
		}
		protected internal override string SaveClientState() {
			return HtmlConvertor.ToJSON(RenderHelper.GetStateObject(rootPane.Panes, false), true, false, true);
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected internal bool HasVisiblePanes() {
			return Panes.GetVisibleItemCount() > 0;
		}
		public SplitterPane GetPaneByPath(int[] path) {
			SplitterPane result = RootPane;
			for(int i = 0; i < path.Length; i++)
				result = result.Panes[path[i]];
			return result;
		}
		public SplitterPane GetPaneByStringPath(string stringPath, string pathSeparator) {
			string[] path = stringPath.Split(new string[] { pathSeparator }, StringSplitOptions.RemoveEmptyEntries);
			int[] intPath = new int[path.Length];
			for(int i = 0; i < path.Length; i++)
				intPath[i] = Int32.Parse(path[i]);
			return GetPaneByPath(intPath);
		}
		public SplitterPane GetPaneByStringPath(string stringPath) {
			return GetPaneByStringPath(stringPath, RenderUtils.IndexSeparator.ToString());
		}
		public SplitterPane GetPaneByName(string name) {
			return Panes.FindByName(name);
		}
		protected internal Orientation ReverseOrientation(Orientation orientation) {
			if(orientation == Orientation.Vertical)
				return Orientation.Horizontal;
			return Orientation.Vertical;
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.SplitterPanesOwner"; } }
	}
}
