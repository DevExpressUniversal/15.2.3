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
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxTreeView"),
	Designer("DevExpress.Web.Design.ASPxTreeViewDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxTreeView.bmp")
	]
	public class ASPxTreeView : ASPxHierarchicalDataWebControl, IRequiresLoadPostDataControl, IControlDesigner {
		protected internal const string ScriptResourceName = WebScriptsResourcePath + "TreeView.js";
		protected const string
			DefaultNavigateUrlFormatString = "{0}",
			DefaultTextFormatString = "{0}",
			ClientObjectClassName = "ASPxClientTreeView",
			NodeTemplateContainerIDPrefix = "NTC",
			NodeTextTemplateContainerIDPrefix = "NTTC",
			NodeImageIDPostfix = "I",
			NodesStateHiddenFieldIDPostfix = "_NSHF",
			NodeCheckboxIDPostfix = "_CHK",
			ExpandNodeCommand = "E",
			ExpandAllNodesCommand = "EA",
			RaiseNodeClickEventCommand = "NCLK",
			RaiseExpandedChangingEventCommand = "ECHANGING",
			RaiseCheckedChangedEventCommand = "CCHNGD",
			CheckNodeRecursiveCommand = "CHKNR",
			NodeClickEventName = "NodeClick",
			ExpandedChangingEventName = "ExpandedChanging",
			CheckedChangedEventName = "CheckedChanged",
			NodesStateKey = "nodesState";
		const char PostRequestArgsSeparator = '|';
		string nodeExpandedOnCallbackID = string.Empty;
		string nodeCheckedOnCallbackID = string.Empty;
		ITemplate nodeTemplate = null;
		ITemplate nodeTextTemplate = null;
		TreeViewDataMediator dataMediator = null;
		TreeViewNode nodeWithCurrentPath = null;
		TreeViewNodeCollection callbackNodes = null;
		ImagePropertiesCache<TreeViewNode, ItemImageProperties> nodeImagePropertiesCache = null;
		static readonly object EventNodeClick = new object();
		static readonly object EventExpandedChanging = new object();
		static readonly object EventExpandedChanged = new object();
		static readonly object EventCheckedChanged = new object();
		static readonly object EventNodeCommand = new object();
		static readonly object EventNodeDataBound = new object();
		static readonly object EventVirtualModeCreateChildren = new object();
		public ASPxTreeView()
			: this(null) {
		}
		public ASPxTreeView(ASPxWebControl owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNodeImagePosition"),
#endif
		Category("Appearance"), DefaultValue(TreeViewNodeImagePosition.Left), AutoFormatEnable]
		public TreeViewNodeImagePosition NodeImagePosition {
			get {
				return (TreeViewNodeImagePosition)GetEnumProperty("NodeImagePosition",
					TreeViewNodeImagePosition.Left);
			}
			set {
				SetEnumProperty("NodeImagePosition", TreeViewNodeImagePosition.Left, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewShowTreeLines"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowTreeLines {
			get { return GetBoolProperty("ShowTreeLines", true); }
			set {
				SetBoolProperty("ShowTreeLines", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewShowExpandButtons"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowExpandButtons {
			get { return GetBoolProperty("ShowExpandButtons", true); }
			set {
				SetBoolProperty("ShowExpandButtons", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNodeLinkMode"),
#endif
		Category("Behavior"), DefaultValue(ItemLinkMode.ContentBounds), AutoFormatEnable]
		public ItemLinkMode NodeLinkMode {
			get { return (ItemLinkMode)GetEnumProperty("NodeLinkMode", ItemLinkMode.ContentBounds); }
			set {
				SetEnumProperty("NodeLinkMode", ItemLinkMode.ContentBounds, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewAllowCheckNodes"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AllowCheckNodes {
			get { return GetBoolProperty("AllowCheckNodes", false); }
			set {
				SetBoolProperty("AllowCheckNodes", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewAllowSelectNode"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AllowSelectNode {
			get { return GetBoolProperty("AllowSelectNode", false); }
			set { SetBoolProperty("AllowSelectNode", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewCheckNodesRecursive"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool CheckNodesRecursive {
			get { return GetBoolProperty("CheckNodesRecursive", false); }
			set {
				SetBoolProperty("CheckNodesRecursive", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewEnableAnimation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableAnimation {
			get { return GetBoolProperty("EnableAnimation", true); }
			set { SetBoolProperty("EnableAnimation", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewEnableHotTrack"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatEnable]
		public bool EnableHotTrack {
			get { return EnableHotTrackInternal; }
			set { EnableHotTrackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewAutoPostBack"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoPostBack {
			get { return base.AutoPostBackInternal; }
			set { base.AutoPostBackInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewEnableCallBacks"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewSyncSelectionMode"),
#endif
		Category("Behavior"), DefaultValue(SyncSelectionMode.CurrentPathAndQuery), AutoFormatDisable]
		public new SyncSelectionMode SyncSelectionMode {
			get { return base.SyncSelectionMode; }
			set { base.SyncSelectionMode = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TreeViewNodeTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate NodeTemplate {
			get { return this.nodeTemplate; }
			set {
				this.nodeTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TreeViewNodeTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate NodeTextTemplate {
			get { return this.nodeTextTemplate; }
			set {
				this.nodeTextTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNavigateUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string NavigateUrlField {
			get { return GetStringProperty("NavigateUrlField", string.Empty); }
			set {
				SetStringProperty("NavigateUrlField", string.Empty, value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNavigateUrlFormatString"),
#endif
		Category("Data"), DefaultValue(DefaultNavigateUrlFormatString), Localizable(true), AutoFormatEnable]
		public string NavigateUrlFormatString {
			get { return GetStringProperty("NavigateUrlFormatString", DefaultNavigateUrlFormatString); }
			set { SetStringProperty("NavigateUrlFormatString", DefaultNavigateUrlFormatString, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string TextField {
			get { return GetStringProperty("TextField", string.Empty); }
			set {
				SetStringProperty("TextField", string.Empty, value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewTextFormatString"),
#endif
		Category("Data"), DefaultValue(DefaultTextFormatString), Localizable(true), AutoFormatEnable]
		public string TextFormatString {
			get { return GetStringProperty("TextFormatString", DefaultTextFormatString); }
			set { SetStringProperty("TextFormatString", DefaultTextFormatString, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewToolTipField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ToolTipField {
			get { return GetStringProperty("ToolTipField", string.Empty); }
			set {
				SetStringProperty("ToolTipField", string.Empty, value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ImageUrlField {
			get { return GetStringProperty("ImageUrlField", string.Empty); }
			set {
				SetStringProperty("ImageUrlField", string.Empty, value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNameField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string NameField {
			get { return GetStringProperty("NameField", string.Empty); }
			set {
				SetStringProperty("NameField", string.Empty, value);
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public TreeViewClientSideEvents ClientSideEvents {
			get { return (TreeViewClientSideEvents)base.ClientSideEventsInternal; }
		}
		[Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeViewImages Images {
			get { return (TreeViewImages)ImagesInternal; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), UrlProperty]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeViewStyles Styles {
			get { return (TreeViewStyles)StylesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new TreeViewSettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel as TreeViewSettingsLoadingPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewTarget"),
#endif
		Category("Misc"), AutoFormatDisable, DefaultValue(""), Localizable(false),
		TypeConverter(typeof(TargetConverter))]
		public string Target {
			get { return GetStringProperty("Target", string.Empty); }
			set { SetStringProperty("Target", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies
		{
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID
		{
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNodeClick"),
#endif
		Category("Action")]
		public event TreeViewNodeEventHandler NodeClick
		{
			add { Events.AddHandler(EventNodeClick, value); }
			remove { Events.RemoveHandler(EventNodeClick, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNodeCommand"),
#endif
		Category("Action")]
		public event TreeViewNodeCommandEventHandler NodeCommand
		{
			add { Events.AddHandler(EventNodeCommand, value); }
			remove { Events.RemoveHandler(EventNodeCommand, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewExpandedChanging"),
#endif
		Category("Behavior")]
		public event TreeViewNodeCancelEventHandler ExpandedChanging
		{
			add { Events.AddHandler(EventExpandedChanging, value); }
			remove { Events.RemoveHandler(EventExpandedChanging, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewExpandedChanged"),
#endif
		Category("Behavior")]
		public event TreeViewNodeEventHandler ExpandedChanged
		{
			add { Events.AddHandler(EventExpandedChanged, value); }
			remove { Events.RemoveHandler(EventExpandedChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewCheckedChanged"),
#endif
		Category("Behavior")]
		public event TreeViewNodeEventHandler CheckedChanged
		{
			add { Events.AddHandler(EventCheckedChanged, value); }
			remove { Events.RemoveHandler(EventCheckedChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNodeDataBound"),
#endif
		Category("Data")]
		public event TreeViewNodeEventHandler NodeDataBound
		{
			add { Events.AddHandler(EventNodeDataBound, value); }
			remove { Events.RemoveHandler(EventNodeDataBound, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewVirtualModeCreateChildren"),
#endif
		Category("VirtualMode")]
		public event TreeViewVirtualModeCreateChildrenEventHandler VirtualModeCreateChildren
		{
			add { Events.AddHandler(EventVirtualModeCreateChildren, value); }
			remove { Events.RemoveHandler(EventVirtualModeCreateChildren, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxTreeViewBeforeGetCallbackResult")]
#endif
		public event EventHandler BeforeGetCallbackResult {
			add { Events.AddHandler(EventBeforeGetCallbackResult, value); }
			remove { Events.RemoveHandler(EventBeforeGetCallbackResult, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxTreeViewClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		protected internal bool HasExpandedChangedHandler {
			get { return Events[EventExpandedChanged] != null; }
		}
		protected internal bool HasNodeClickHandler {
			get { return Events[EventNodeClick] != null; }
		}
		protected internal bool HasExpandingChangingHandler {
			get { return Events[EventExpandedChanging] != null; }
		}
		protected internal bool HasCheckedChangedHandler {
			get { return Events[EventCheckedChanged] != null; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTreeViewNodes"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public TreeViewNodeCollection Nodes {
			get { return RootNode.Nodes; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeViewNode RootNode {
			get { return DataMediator.RootNode; }
		}
		protected internal TreeViewNode FindNodeByID(string id) {
			if (string.IsNullOrEmpty(id))
				return null;
			string indexPath = TreeViewNode.GetIndexPathByID(id);
			string[] pathIndices = indexPath.Split(TreeViewNode.IndexPathSeparator);
			TreeViewNode node = RootNode;
			for (int i = 0; i < pathIndices.Length; i++) {
				int index = int.Parse(pathIndices[i]);
				if (IsVirtualMode() && !node.Expanded)
					(node as TreeViewVirtualNode).ForceNodesPopulation();
				if (node.Nodes.Count - 1 < index)
					return null;
				node = node.Nodes[index];
			}
			return node;
		}
		protected override void EnsurePreRender() {
			if (IsVirtualMode() && !IsCallback)
				EnsureNodes(RootNode);
			if(CheckNodesRecursive && !IsVirtualMode())
				ValidateNodeCheckStates(RootNode);
			base.EnsurePreRender();
		}
		protected void EnsureNodes(TreeViewNode node) {
			foreach (TreeViewNode childNode in node.Nodes)
				EnsureNodes(childNode);
		}
		protected void ValidateNodeCheckStates(TreeViewNode node) {
			if (node.Nodes.Count == 0)
				node.UpdateCheckedStateRecursive();
			else {
				foreach (TreeViewNode childNode in node.Nodes)
					ValidateNodeCheckStates(childNode);
			}
		}
		protected virtual TreeViewDataMediator DataMediator {
			get {
				if (IsVirtualMode() && !(this.dataMediator is VirtualModeTreeViewDataMediator))
					this.dataMediator = new VirtualModeTreeViewDataMediator(this);
				else if (!IsVirtualMode() && !(this.dataMediator is RealModeTreeViewDataMediator))
					this.dataMediator = new RealModeTreeViewDataMediator(this);
				return this.dataMediator;
			}
		}
		protected internal HierarchicalDataSourceView GetTreeViewData(string viewPath) {
			return base.GetData(viewPath);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeViewNode SelectedNode {
			get { return DataMediator.SelectedNode; }
			set { DataMediator.SelectedNode = value; }
		}
		protected internal TreeViewNode NodeWithCurrentPath {
			get { return this.nodeWithCurrentPath; }
			set { this.nodeWithCurrentPath = value; }
		}
		protected string NodeExpandedOnCallbackID {
			get { return this.nodeExpandedOnCallbackID; }
			set { this.nodeExpandedOnCallbackID = value; }
		}
		protected string NodeCheckedRecursiveOnCallbackID {
			get { return this.nodeCheckedOnCallbackID; }
			set { this.nodeCheckedOnCallbackID = value; }
		}
		protected internal TreeViewNodeCollection CallbackNodes {
			get { return this.callbackNodes; }
			set { this.callbackNodes = value; }
		}
		protected string CheckboxIDPrefix {
			get { return ClientID + NodeCheckboxIDPostfix; }
		}
		protected string GetNodeIDByCheckboxID(string checkboxID) {
			return TreeViewNode.NodeIDPrefix + checkboxID.Replace(CheckboxIDPrefix, string.Empty);
		}
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() {
			return new TreeViewSettingsLoadingPanel(this);
		}
		protected internal new bool IsRightToLeft() {
			return base.IsRightToLeft();
		}
		protected static internal string SerializeBooleanValue(bool value) {
			return value ? "T" : string.Empty;
		}
		protected static internal bool DeserializeBooleanValue(string serializedValue) {
			return serializedValue == "T";
		}
		protected static internal string SerializeCheckStateEnumValue(CheckState checkState) {
			return InternalCheckboxControl.GetCheckStateKey(checkState);
		}
		protected static internal CheckState DeserializeCheckStateEnumValue(string key) {
			return InternalCheckboxControl.GetCheckStateByKey(key);
		}
		protected string[] GetPostRequestCommandArgs(string eventArgument) {
			if (string.IsNullOrEmpty(eventArgument))
				return null;
			string[] args = eventArgument.Split(PostRequestArgsSeparator);
			return args.Length < 2 ? null : args;
		}
		protected void PerformActionOnNodesRecursive(TreeViewNodeCollection nodes,
		   Action<TreeViewNode> action) {
			foreach (TreeViewNode node in nodes) {
				if (!node.Visible)
					continue;
				action(node);
				if (node.Nodes.Count != 0 && (node.Expanded ||
					(!IsVirtualMode() && !AutoPostBack && !IsCallBacksEnabled()))) {
					PerformActionOnNodesRecursive(node.Nodes, action);
				}
			}
		}
		protected internal virtual void OnNodeClick(TreeViewNodeEventArgs e) {
			TreeViewNodeEventHandler handler = (TreeViewNodeEventHandler)Events[EventNodeClick];
			if (handler != null)
				handler(this, e);
		}
		protected internal virtual void OnExpandedChanging(TreeViewNodeCancelEventArgs e) {
			TreeViewNodeCancelEventHandler handler = (TreeViewNodeCancelEventHandler)Events[EventExpandedChanging];
			if (handler != null)
				handler(this, e);
		}
		protected internal virtual void OnExpandedChanged(TreeViewNodeEventArgs e) {
			TreeViewNodeEventHandler handler = (TreeViewNodeEventHandler)Events[EventExpandedChanged];
			if (handler != null)
				handler(this, e);
		}
		protected internal virtual void OnCheckedChanged(TreeViewNodeEventArgs e) {
			TreeViewNodeEventHandler handler = (TreeViewNodeEventHandler)Events[EventCheckedChanged];
			if (handler != null)
				handler(this, e);
		}
		protected internal virtual void OnNodeDataBound(TreeViewNodeEventArgs e) {
			TreeViewNodeEventHandler handler = (TreeViewNodeEventHandler)Events[EventNodeDataBound];
			if (handler != null)
				handler(this, e);
		}
		protected internal virtual void OnVirtualModeCreateChildren(TreeViewVirtualModeCreateChildrenEventArgs e) {
			TreeViewVirtualModeCreateChildrenEventHandler handler =
				(TreeViewVirtualModeCreateChildrenEventHandler)Events[EventVirtualModeCreateChildren];
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnNodeCommand(TreeViewNodeCommandEventArgs e) {
			TreeViewNodeCommandEventHandler handler = (TreeViewNodeCommandEventHandler)Events[EventNodeCommand];
			if (handler != null)
				handler(this, e);
		}
		protected override bool OnBubbleEvent(object source, EventArgs e) {
			TreeViewNodeCommandEventArgs args = e as TreeViewNodeCommandEventArgs;
			if (args == null)
				return false;
			OnNodeCommand(args);
			return true;
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents() && (HasNodeClickHandler || HasExpandingChangingHandler ||
				HasCheckedChangedHandler || IsVirtualMode());
		}
		public bool IsVirtualMode() {
			return Events[EventVirtualModeCreateChildren] != null;
		}
		public void RefreshVirtualTree() {
			RefreshVirtualTree(RootNode);
		}
		public void RefreshVirtualTree(TreeViewNode startNode) {
			if(!IsVirtualMode() || startNode.TreeView != this)
				return;
			VirtualModeTreeViewDataMediator dataMediator = DataMediator as VirtualModeTreeViewDataMediator;
			if(startNode == RootNode)
				dataMediator.RemoveNodesFromState();
			else
				dataMediator.RemoveNodesFromState(startNode);
			startNode.Nodes.Clear();
		}
		protected override bool IsAnimationScriptNeeded() {
			return EnableAnimation;
		}
		public override bool IsClientSideAPIEnabled() {
			return IsVirtualMode() || IsClientSideAPIEnabled(Nodes) || base.IsClientSideAPIEnabled();
		}
		protected bool IsClientSideAPIEnabled(TreeViewNodeCollection nodes) {
			foreach (TreeViewNode node in nodes) {
				if (!node.Visible)
					continue;
				if (!node.ClientEnabled || !node.ClientVisible || IsClientSideAPIEnabled(node.Nodes))
					return true;
			}
			return false;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new TreeViewClientSideEvents();
		}
		protected override bool HasFunctionalityScripts() {
			return Enabled && Visible;
		}
		protected override string GetClientObjectClassName() {
			return ClientObjectClassName;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxWebControl), StateControllerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxTreeView), ScriptResourceName);
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			base.GetClientObjectAssignedServerEvents(eventNames);
			if (!HasEvents())
				return;
			if (HasNodeClickHandler)
				eventNames.Add(NodeClickEventName);
			if (HasExpandingChangingHandler)
				eventNames.Add(ExpandedChangingEventName);
			if (HasCheckedChangedHandler)
				eventNames.Add(CheckedChangedEventName);
		}
		protected bool RequireWidthRecalculationOnHover() {
			bool require = false;
			PerformActionOnNodesRecursive(Nodes, delegate(TreeViewNode node) {
				if (GetNodeTemplate(node) != null)
					return;
				AppearanceSelectedStyle hoverStyle = GetNodeHoverStyle(node);
				if (hoverStyle.Width != Unit.Empty || hoverStyle.BorderWidth != Unit.Empty || hoverStyle.Font.Size != FontUnit.Empty || hoverStyle.Font.Bold)
					require = true;
			});
			return require;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName,
			string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat(localVarName + ".nodesState={0};\n", HtmlConvertor.ToJSON(DataMediator.CreateSerializedNodesState()));
			if (!EnableAnimation)
				stb.Append(localVarName + ".enableAnimation=false;\n");
			if (IsClientSideAPIEnabled())
				stb.Append(localVarName + ".nodesInfo=" + HtmlConvertor.ToJSON(GetNodesInfo(Nodes)) + ";\n");
			else if (NodeLinkMode == ItemLinkMode.ContentBounds)
				stb.Append(localVarName + ".nodesUrls=" + HtmlConvertor.ToJSON(GetNodesUrls(Nodes)) + ";\n");
			if (NodeLinkMode != ItemLinkMode.ContentBounds)
				stb.Append(localVarName + ".contentBoundsMode=false;\n");
			if (AllowCheckNodes) {
				stb.Append(localVarName + ".imageProperties = " + ImagePropertiesSerializer.GetImageProperties(GetCheckBoxImages(), this) + ";\n");
				stb.Append(localVarName + ".icbFocusedStyle = " + InternalCheckboxControl.SerializeFocusedStyle(GetICBFocusedStyle(), this) + ";\n");
				if (CheckNodesRecursive)
					stb.Append(localVarName + ".checkNodesRecursive=true;\n");
			}
			if (AllowSelectNode)
				stb.Append(localVarName + ".allowSelectNode=true;\n");
			if (IsVirtualMode())
				stb.Append(localVarName + ".virtualMode=true;\n");
			if (RequireWidthRecalculationOnHover())
				stb.Append(localVarName + ".requireWidthRecalculationOnHover=true;\n");
		}
		protected void AddDisabledItems(StateScriptRenderHelper helper, TreeViewNodeCollection nodes) {
			PerformActionOnNodesRecursive(nodes,
			   delegate(TreeViewNode node) {
				   if (GetNodeTemplate(node) != null)
					   return;
				   AppearanceStyle nodeStyle = GetNodeTextTemplate(node) != null ?
					   GetNodeTemplateStyle() : GetNodeStyle(node);
				   MergeWithDisabledStyle<AppearanceStyle>(nodeStyle);
				   helper.AddStyle(nodeStyle, node.GetID(), new string[0],
					   GetNodeImageProperties(node).GetDisabledScriptObject(Page),
					   NodeImageIDPostfix, IsEnabled() && node.Enabled);
			   });
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			AddDisabledItems(helper, Nodes);
		}
		protected override bool HasHoverScripts() {
			return EnableHotTrack && NodeLinkMode == ItemLinkMode.ContentBounds;
		}
		protected void AddHoverItems(StateScriptRenderHelper helper, TreeViewNodeCollection nodes) {
			PerformActionOnNodesRecursive(nodes,
			   delegate(TreeViewNode node) {
				   if (GetNodeTemplate(node) != null)
					   return;
				   AppearanceSelectedStyle hoverStyle = GetNodeHoverStyle(node);
				   helper.AddStyle(hoverStyle, node.GetID(), new string[0],
					   GetNodeImageProperties(node).GetHottrackedScriptObject(Page),
					   NodeImageIDPostfix, IsEnabled() && node.Enabled);
			   });
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			AddHoverItems(helper, Nodes);
		}
		protected override bool HasSelectedScripts() {
			return AllowSelectNode || SelectedNode != null;
		}
		protected void AddSelectedItems(StateScriptRenderHelper helper, TreeViewNodeCollection nodes) {
			PerformActionOnNodesRecursive(nodes,
				delegate(TreeViewNode node) {
					if (GetNodeTemplate(node) != null || (!AllowSelectNode && SelectedNode != node))
						return;
					AppearanceSelectedStyle selectedStyle = GetNodeSelectedStyle(node);
					helper.AddStyle(selectedStyle, node.GetID(), new string[0],
						GetNodeImageProperties(node).GetSelectedScriptObject(Page),
						NodeImageIDPostfix, IsEnabled() && node.Enabled);
				});
		}
		protected override void AddSelectedItems(StateScriptRenderHelper helper) {
			AddSelectedItems(helper, Nodes);
		}
		protected Dictionary<string, object[]> GetNodesInfo(TreeViewNodeCollection nodes) {
			Dictionary<string, object[]> nodesInfo = new Dictionary<string, object[]>();
			PerformActionOnNodesRecursive(nodes,
				delegate(TreeViewNode node) {
					nodesInfo.Add(node.GetIndexPath(), new object[] { 
						SerializeBooleanValue(node.ClientEnabled),
						SerializeBooleanValue(node.ClientVisible),
						node.Name,
						NodeLinkMode == ItemLinkMode.ContentBounds && !IsCurrentPathNode(node) ? 
							node.GetRenderingNavigateUrl() : string.Empty,
						NodeLinkMode == ItemLinkMode.ContentBounds ? node.GetRenderingTarget() : string.Empty
					});
				});
			return nodesInfo;
		}
		protected bool IsCurrentPathNode(TreeViewNode node) {
			return NodeLinkMode == ItemLinkMode.ContentBounds &&
						SyncSelectionMode != SyncSelectionMode.None && node == SelectedNode;
		}
		protected Dictionary<string, string[]> GetNodesUrls(TreeViewNodeCollection nodes) {
			Dictionary<string, string[]> nodesUrls = new Dictionary<string, string[]>();
			bool hasControlLevelTemplates = NodeTemplate != null || NodeTextTemplate != null;
			PerformActionOnNodesRecursive(nodes,
				delegate(TreeViewNode node) {
					bool hasNodeLevelTemplates = node.Template != null || node.TextTemplate != null;
					if (!string.IsNullOrEmpty(node.GetRenderingNavigateUrl()) &&
						(hasControlLevelTemplates || hasNodeLevelTemplates) && !IsCurrentPathNode(node)) {
						nodesUrls.Add(node.GetIndexPath(), new string[] { ResolveUrl(node.GetRenderingNavigateUrl()), 
							node.GetRenderingTarget()});
					}
				});
			return nodesUrls;
		}
		protected override void SelectCurrentPath(bool ignoreQueryString) {
			SelectCurrentPathRecursive(Nodes, ignoreQueryString);
		}
		protected bool SelectCurrentPathRecursive(TreeViewNodeCollection nodes, bool ignoreQueryString) {
			foreach (TreeViewNode node in nodes) {
				if (node.Visible && UrlUtils.IsCurrentUrl(ResolveUrl(node.GetRenderingNavigateUrl()),
					ignoreQueryString)) {
					SelectedNode = node;
					NodeWithCurrentPath = node;
					return true;
				}
				if (node.Nodes.Count != 0) {
					if (SelectCurrentPathRecursive(node.Nodes, ignoreQueryString))
						return true;
				}
			}
			return false;
		}
		public void ExpandAll() {
			EnsureDataBound();
			SetNodesExpandedRecursive(Nodes, true, -1);
		}
		public void CollapseAll() {
			EnsureDataBound();
			SetNodesExpandedRecursive(Nodes, false, -1);
		}
		public void ExpandToDepth(int depth) {
			EnsureDataBound();
			SetNodesExpandedRecursive(Nodes, true, depth);
		}
		protected void SetNodesExpandedRecursive(TreeViewNodeCollection nodes, bool expanded, int maxDepth) {
			foreach (TreeViewNode node in nodes) {
				if (maxDepth > -1 && node.Depth > maxDepth)
					return;
				if (node.Visible && node.Enabled && !node.IsLeaf)
					node.Expanded = expanded;
				if (node.Nodes.Count > 0)
					SetNodesExpandedRecursive(node.Nodes, expanded, maxDepth);
			}
		}
		public void ExpandToNode(TreeViewNode node) {
			if (node == null || node.TreeView != this)
				return;
			for (TreeViewNode parentNode = node.Parent; parentNode != null; parentNode = parentNode.Parent)
				parentNode.Expanded = true;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				DataMediator.GetStateManagedDataObjects());
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			DataMediator.OnLoadViewState();
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			EnsureDataBound();
			DataMediator.SyncNodesState(GetClientObjectStateValue < ArrayList>(NodesStateKey));
			ResetViewStateStoringFlag();
			return false;
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected internal bool CanLoadClientState() {
			return IsClientLayoutExists || IsStateSavedToCookies;
		}
		protected internal override string SaveClientState() {
			return DataMediator.CreateSerializedNodesStateString();
		}
		protected internal override void LoadClientState(string state) {
			EnsureDataBound();
			if (!string.IsNullOrEmpty(state))
				DataMediator.SyncNodesState(state);
			if (IsVirtualMode())
				ResetControlHierarchy();
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			string[] postbackArgs = GetPostRequestCommandArgs(eventArgument);
			if (postbackArgs != null) {
				switch (postbackArgs[0]) {
					case ExpandAllNodesCommand:
						ExpandAll();
						return;
					case CheckNodeRecursiveCommand:
						TreeViewNode node = FindNodeByID(postbackArgs[1]);
						if (node != null)
							node.UpdateCheckedStateRecursive();
						DataMediator.OnCheckedChanged(postbackArgs[1]);
						return;
					case ExpandNodeCommand:
						DataMediator.OnExpandedChanged(postbackArgs[1]);
						return;
					case RaiseCheckedChangedEventCommand:
						DataMediator.OnCheckedChanged(postbackArgs[1]);
						return;
					case RaiseNodeClickEventCommand:
						DataMediator.OnNodeClick(postbackArgs[1]);
						return;
					case RaiseExpandedChangingEventCommand:
						DataMediator.OnExpandedChanging(postbackArgs[1], DeserializeBooleanValue(postbackArgs[2]));
						return;
				}
			}
			base.RaisePostBackEvent(eventArgument);
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			string[] callbackArgs = GetPostRequestCommandArgs(eventArgument);
			if (callbackArgs != null) {
				switch (callbackArgs[0]) {
					case ExpandNodeCommand:
						NodeExpandedOnCallbackID = callbackArgs[1];
						CallbackNodes = DataMediator.GetNodeChildrenOnCallback(NodeExpandedOnCallbackID);
						DataMediator.OnExpandedChanged(NodeExpandedOnCallbackID);
						return;
					case ExpandAllNodesCommand:
						NodeExpandedOnCallbackID = null;
						CallbackNodes = Nodes;
						return;
					case CheckNodeRecursiveCommand:
						NodeCheckedRecursiveOnCallbackID = callbackArgs[1];
						DataMediator.OnCheckedChanged(NodeCheckedRecursiveOnCallbackID);
						return;
				}
			}
			base.RaiseCallbackEvent(eventArgument);
		}
		protected object[] GetNodeCallbackData(string nodeID) {
			object[] data = new object[5];
			data[0] = nodeID;
			data[1] = GetNodeContainerRenderResult();
			if (IsClientSideAPIEnabled())
				data[2] = GetNodesInfo(CallbackNodes);
			else if (NodeLinkMode == ItemLinkMode.ContentBounds)
				data[2] = GetNodesUrls(CallbackNodes);
			data[3] = DataMediator.GetNodeNames();
			data[4] = DataMediator.CreateSerializedNodesState();
			return data;
		}
		protected virtual object GetExpandNodeOnCallbackResult() {
			object[] result = new object[6];
			result[0] = ExpandNodeCommand;
			if (string.IsNullOrEmpty(NodeExpandedOnCallbackID))
				ExpandAllNodesRecursive(CallbackNodes);
			GetNodeCallbackData(NodeExpandedOnCallbackID).CopyTo(result, 1);
			return result;
		}
		protected void ExpandAllNodesRecursive(TreeViewNodeCollection nodes) {
			foreach (TreeViewNode node in nodes) {
				if(IsVirtualMode() && !node.Expanded)
					(node as TreeViewVirtualNode).ForceNodesPopulation();
				if (!node.Visible || (!node.Enabled && !node.Expanded) || node.Nodes.Count == 0)
					continue;
				if (!node.Expanded)
					node.Expanded = true;
				ExpandAllNodesRecursive(node.Nodes);
			}
		}
		protected object GetCheckNodeOnCallbackResult() {
			TreeViewNode node = FindNodeByID(NodeCheckedRecursiveOnCallbackID);
			if (node == null || node.Nodes.Count == 0)
				return string.Empty;
			object[] result = new object[3];
			result[0] = CheckNodeRecursiveCommand;
			result[1] = SerializeCheckStateEnumValue(node.CheckState);
			List<string> checkedNodeIDs = new List<string>();
			CheckAndPopulateCheckedNodeIDsRecursive(node.Nodes, checkedNodeIDs, node.CheckState);
			result[2] = checkedNodeIDs;
			return result;
		}
		protected void CheckAndPopulateCheckedNodeIDsRecursive(TreeViewNodeCollection nodes, List<string> checkedNodeIDs,
			CheckState checkState) {
			foreach (TreeViewNode node in nodes) {
				if (node.CheckState != checkState) {
					node.SetCheckState(checkState);
					checkedNodeIDs.Add(node.GetID());
				}
				if (node.Nodes.Count > 0)
					CheckAndPopulateCheckedNodeIDsRecursive(node.Nodes, checkedNodeIDs, checkState);
			}
		}
		protected override object GetCallbackResult() {
			if (!string.IsNullOrEmpty(NodeCheckedRecursiveOnCallbackID))
				return GetCheckNodeOnCallbackResult();
			return GetExpandNodeOnCallbackResult();
		}
		protected virtual string GetCallbackResultHtml() {
			TreeViewNodesListControl nodeContainer = GetCallbackResultControl();
			return RenderUtils.GetRenderResult(nodeContainer);
		}
		protected virtual TreeViewNodesListControl GetCallbackResultControl() {
			ResetControlHierarchy();
			EnsureChildControls();
			DataBindContainers(this, true, true);
			return (Controls[0] as TreeViewControl).RootNodesContainer;
		}
		protected internal override void PerformDataBinding(string dataHelperName) {
			DataMediator.PerformDataBinding();
		}
		protected override void OnDataBinding(EventArgs e) {
			EnsureChildControls();
			base.OnDataBinding(e);
		}
		protected internal void CreateTemplate(ITemplate template, TreeViewNodeTemplateContainer container,
			string containerID, WebControl parent) {
			template.InstantiateIn(container);
			container.AddToHierarchy(parent, containerID);
		}
		protected internal string GetNodeTemplateContainerID(TreeViewNode node) {
			return NodeTemplateContainerIDPrefix + node.GetIndexPath();
		}
		protected internal string GetNodeTextTemplateContainerID(TreeViewNode node) {
			return NodeTextTemplateContainerIDPrefix + node.GetIndexPath();
		}
		protected internal ITemplate GetNodeTemplate(TreeViewNode node) {
			return node.Template ?? NodeTemplate;
		}
		protected internal ITemplate GetNodeTextTemplate(TreeViewNode node) {
			return node.TextTemplate ?? NodeTextTemplate;
		}
		internal virtual InternalCheckBoxImageProperties GetCheckBoxImage(CheckState checkState) {
			InternalCheckBoxImageProperties result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			switch (checkState) {
				case CheckState.Checked:
					imageName = InternalCheckboxControl.CheckBoxCheckedImageName;
					result.MergeWith(Images.CheckBoxChecked);
					break;
				case CheckState.Unchecked:
					imageName = InternalCheckboxControl.CheckBoxUncheckedImageName;
					result.MergeWith(Images.CheckBoxUnchecked);
					break;
				default:
					imageName = InternalCheckboxControl.CheckBoxGrayedImageName;
					result.MergeWith(Images.CheckBoxGrayed);
					break;
			}
			result.MergeWith(Images.GetImageProperties(Page, imageName));
			return result;
		}
		protected List<InternalCheckBoxImageProperties> GetCheckBoxImages() {
			return new List<InternalCheckBoxImageProperties>(new InternalCheckBoxImageProperties[] { GetCheckBoxImage(CheckState.Checked), 
				GetCheckBoxImage(CheckState.Unchecked), GetCheckBoxImage(CheckState.Indeterminate) });
		}
		protected override ImagesBase CreateImages() {
			return new TreeViewImages(this);
		}
		protected internal ItemImageProperties GetNodeImageProperties(TreeViewNode node) {
			if (this.nodeImagePropertiesCache == null) {
				this.nodeImagePropertiesCache =
					new ImagePropertiesCache<TreeViewNode, ItemImageProperties>(
					delegate(CacheKey<TreeViewNode> nodeKey) {
						ItemImageProperties properties = new ItemImageProperties();
						properties.MergeWith(nodeKey.Item.Image);
						properties.MergeWith(Images.GetImageProperties(Page, TreeViewImages.NodeImageName));
						properties.MergeWith(Images.NodeImage);
						return properties;
					});
			}
			return GetItemImage(this.nodeImagePropertiesCache, node);
		}
		protected internal ItemImageProperties GetRenderingNodeImageProperties(TreeViewNode node) {
			ItemImageProperties properties = GetNodeImageProperties(node);
			if (node.Checked) {
				if (!string.IsNullOrEmpty(properties.UrlSelected))
					properties.Url = properties.UrlSelected;
				if (!string.IsNullOrEmpty(properties.SpriteProperties.SelectedCssClass))
					properties.SpriteProperties.CssClass = properties.SpriteProperties.SelectedCssClass;
				if (!properties.SpriteProperties.SelectedLeft.IsEmpty)
					properties.SpriteProperties.Left = properties.SpriteProperties.Left;
				if (!properties.SpriteProperties.SelectedTop.IsEmpty)
					properties.SpriteProperties.Top = properties.SpriteProperties.SelectedTop;
			}
			if (!node.Enabled) {
				if (!string.IsNullOrEmpty(properties.UrlDisabled))
					properties.Url = properties.UrlDisabled;
				if (!string.IsNullOrEmpty(properties.SpriteProperties.DisabledCssClass))
					properties.SpriteProperties.CssClass = properties.SpriteProperties.DisabledCssClass;
				if (!properties.SpriteProperties.DisabledLeft.IsEmpty)
					properties.SpriteProperties.Left = properties.SpriteProperties.DisabledLeft;
				if (!properties.SpriteProperties.DisabledTop.IsEmpty)
					properties.SpriteProperties.Top = properties.SpriteProperties.DisabledTop;
			}
			return properties;
		}
		protected override StylesBase CreateStyles() {
			return new TreeViewStyles(this);
		}
		protected T GetStyle<T>(T defaultStyle, T userDefinedStyle) where T :
			AppearanceStyleBase, new() {
			T style = new T();
			if (defaultStyle != null)
				style.CopyFrom(defaultStyle);
			if (userDefinedStyle != null)
				style.CopyFrom(userDefinedStyle);
			return style;
		}
		protected T GetStyle<T>(T defaultStyle, T userDefinedStyle, T nodeStyle) where T :
			AppearanceStyleBase, new() {
			T style = GetStyle<T>(defaultStyle, userDefinedStyle);
			style.CopyFrom(nodeStyle);
			return style;
		}
		protected internal void MergeWithDisabledStyle<T>(T style) where T :
			AppearanceStyleBase, new() {
			MergeDisableStyle(style, false);
		}
		protected internal AppearanceStyle GetElbowStyle() {
			return GetStyle<AppearanceStyle>(ShowTreeLines ? Styles.GetDefaultElbowStyle() :
				Styles.GetDefaultElbowWithoutLineStyle(), Styles.Elbow);
		}
		protected virtual AppearanceStyleBase GetICBFocusedStyle() {
			return GetStyle<AppearanceStyleBase>(Styles.GetDefaultNodeCheckBoxFocusedStyle(), Styles.NodeCheckBoxFocused);
		}
		protected internal AppearanceStyle GetExpandButtonStyle() {
			return GetStyle<AppearanceStyle>(Styles.GetDefaultButtonStyle(), null);
		}
		protected internal AppearanceStyle GetNodeTextStyle(TreeViewNode node) {
			return GetStyle<AppearanceStyle>(Styles.GetDefaultNodeTextStyle(), Styles.NodeText,
				node.TextStyle);
		}
		protected internal AppearanceStyle GetNodeImageStyle(TreeViewNode node) {
			return GetStyle<AppearanceStyle>(Styles.GetDefaultNodeImageStyle(), Styles.NodeImage,
				node.ImageStyle);
		}
		protected internal TreeViewNodeCheckBoxStyle GetNodeCheckBoxStyle(TreeViewNode node) {
			TreeViewNodeCheckBoxStyle result = GetStyle<TreeViewNodeCheckBoxStyle>(Styles.GetDefaultNodeCheckBoxStyle(),
				Styles.NodeCheckBox, node.CheckBoxStyle);
			result.MergeWith(Styles.GetSystemNodeCheckBoxStyle());
			return result;
		}
		protected internal AppearanceStyle GetSubnodeStyle() {
			return Styles.GetDefaultSubnodeStyle();
		}
		protected internal AppearanceStyle GetNodeTemplateStyle() {
			return Styles.GetDefaultNodeTemplateStyle();
		}
		protected internal AppearanceStyle GetLineStyle() {
			return Styles.GetDefaultLineStyle();
		}
		protected internal AppearanceSelectedStyle GetNodeHoverStyle(TreeViewNode node) {
			return GetStyle<AppearanceSelectedStyle>(Styles.GetDefaultNodeHoverStyle(), Styles.Node.HoverStyle,
				node.NodeStyle.HoverStyle);
		}
		protected internal AppearanceSelectedStyle GetNodeSelectedStyle(TreeViewNode node) {
			return GetStyle<AppearanceSelectedStyle>(Styles.GetDefaultNodeSelectedStyle(), Styles.Node.SelectedStyle,
				node.NodeStyle.SelectedStyle);
		}
		protected internal AppearanceStyle GetNodeStyle(TreeViewNode node) {
			AppearanceStyle style = GetStyle<AppearanceStyle>(Styles.GetDefaultNodeStyle(), Styles.Node, node.NodeStyle);
			MergeDisableStyle(style, node.Enabled, DisabledStyle);
			return style;
		}
		protected internal AppearanceStyle GetNodeLinkStyle(TreeViewNode node) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFontAndCursorFrom(GetNodeTextStyle(node));
			style.CopyFrom(LinkStyle.Style);
			MergeDisableStyle(style, node.Enabled, DisabledStyle);
			return style;
		}
		protected override bool HasContent() {
			return IsVirtualMode() || Nodes.GetVisibleItemCount() > 0;
		}
		protected string GetStateItemsCallbackRenderResult(TreeViewNodeCollection nodes) {
			StringBuilder scriptBuilder = new StringBuilder();
			StateScriptRenderHelper renderHelper = new StateScriptRenderHelper(Page, ClientID);
			AddDisabledItems(renderHelper, nodes);
			renderHelper.GetCreateDisabledScript(scriptBuilder);
			if (HasSelectedScripts()) {
				renderHelper.Items.Clear();
				AddSelectedItems(renderHelper, nodes);
				renderHelper.GetCreateSelectedScript(scriptBuilder);
			}
			if (CanHoverScript() && HasHoverScripts()) {
				renderHelper.Items.Clear();
				AddHoverItems(renderHelper, nodes);
				renderHelper.GetCreateHoverScript(scriptBuilder);
			}
			return RenderUtils.GetScriptHtml(scriptBuilder.ToString());
		}
		protected string GetNodeContainerRenderResult() {
			StringBuilder renderResultBuilder = new StringBuilder();
			BeginRendering();
			try {
				renderResultBuilder.Append(GetCallbackResultHtml());
				renderResultBuilder.Append(GetStateItemsCallbackRenderResult(CallbackNodes));
			} finally {
				EndRendering();
			}
			return renderResultBuilder.ToString();
		}
		protected override bool HasLoadingPanel() {
			return IsCallBacksEnabled() && SettingsLoadingPanel.Mode != TreeViewLoadingPanelMode.Disabled &&
				(IsClientSideAPIEnabled() || (AllowCheckNodes && CheckNodesRecursive) ||
				SettingsLoadingPanel.Mode == TreeViewLoadingPanelMode.ShowAsPopup);
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		TreeViewControl treeViewControl = null;
		protected override void ClearControlFields() {
			this.treeViewControl = null;
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			this.treeViewControl = new TreeViewControl(this);
			Controls.Add(this.treeViewControl);
			base.CreateControlHierarchy();
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.TreeViewNodesOwner"; } }
	}
}
