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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class TreeViewNode : CollectionItem, IEnumerable, IHierarchyData {
		const string DefaultText = "Node";
		protected internal const char IndexPathSeparator = '_';
		protected internal const string NodeIDPrefix = "N";
		ASPxTreeView treeView = null;
		TreeViewNodeCollection nodes = null;
		ItemImageProperties image = null;
		TreeViewNodeStyle nodeStyle = null;
		TreeViewNodeTextStyle textStyle = null;
		AppearanceStyle imageStyle = null;
		TreeViewNodeCheckBoxStyle checkBoxStyle = null;
		object dataItem = null;
		bool isLeaf = false;
		ITemplate template = null;
		ITemplate textTemplate = null;
		public TreeViewNode()
			: base() {
		}
		public TreeViewNode(string text)
			: this(text, string.Empty, string.Empty, string.Empty, string.Empty) {
		}
		public TreeViewNode(string text, string name)
			: this(text, name, string.Empty, string.Empty, string.Empty) {
		}
		public TreeViewNode(string text, string name, string imageUrl)
			: this(text, name, imageUrl, string.Empty, string.Empty) {
		}
		public TreeViewNode(string text, string name, string imageUrl, string navigateUrl)
			: this(text, name, imageUrl, navigateUrl, string.Empty) {
		}
		public TreeViewNode(string text, string name, string imageUrl, string navigateUrl, string target)
			: base() {
			Text = text;
			Name = name;
			Image.Url = imageUrl;
			NavigateUrl = navigateUrl;
			Target = target;
		}
		protected internal TreeViewNode(ASPxTreeView treeView) {
			this.treeView = treeView;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeAllowCheck"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool AllowCheck {
			get { return GetBoolProperty("AllowCheck", true); }
			set {
				SetBoolProperty("AllowCheck", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeClientVisible"),
#endif
		Category("Client-Side"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeClientEnabled"),
#endif
		Category("Client-Side"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public virtual string Name {
			get { return GetStringProperty("Name", string.Empty); }
			set { SetStringProperty("Name", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeNavigateUrl"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor),
		typeof(System.Drawing.Design.UITypeEditor)), AutoFormatDisable]
		public string NavigateUrl {
			get { return GetStringProperty("NavigateUrl", string.Empty); }
			set {
				SetStringProperty("NavigateUrl", string.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeTarget"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeText"),
#endif
		DefaultValue(DefaultText), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string Text {
			get { return GetStringProperty("Text", DefaultText); }
			set {
				SetStringProperty("Text", DefaultText, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", string.Empty); }
			set {
				SetStringProperty("ToolTip", string.Empty, value);
				LayoutChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TreeViewNodeTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public virtual ITemplate Template {
			get { return this.template; }
			set {
				this.template = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TreeViewNodeTemplateContainer)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public virtual ITemplate TextTemplate {
			get { return this.textTemplate; }
			set {
				this.textTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ItemImageProperties Image {
			get {
				if(this.image == null)
					this.image = new ItemImageProperties(this);
				return this.image;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeNodeStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public TreeViewNodeStyle NodeStyle {
			get {
				if(this.nodeStyle == null)
					this.nodeStyle = new TreeViewNodeStyle();
				return this.nodeStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeTextStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public TreeViewNodeTextStyle TextStyle {
			get {
				if(this.textStyle == null)
					this.textStyle = new TreeViewNodeTextStyle();
				return this.textStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeImageStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyle ImageStyle {
			get {
				if(this.imageStyle == null)
					this.imageStyle = new AppearanceStyle();
				return this.imageStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeCheckBoxStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public TreeViewNodeCheckBoxStyle CheckBoxStyle {
			get {
				if(this.checkBoxStyle == null)
					this.checkBoxStyle = new TreeViewNodeCheckBoxStyle();
				return this.checkBoxStyle;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeExpanded"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool Expanded {
			get {
				return GetBoolProperty("Expanded", false);
			}
			set {
				SetBoolProperty("Expanded", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeEnabled"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				SetBoolProperty("Enabled", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeChecked"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool Checked {
			get { return CheckState == CheckState.Checked; }
			set {
				CheckState = value ? CheckState.Checked : CheckState.Unchecked;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeCheckState"),
#endif
		DefaultValue(CheckState.Unchecked), NotifyParentProperty(true), AutoFormatDisable]
		public virtual CheckState CheckState
		{
			get { return (CheckState)GetEnumProperty("CheckState", CheckState.Unchecked); }
			internal set
			{
				if (CheckState == value)
					return;
				SetCheckState(value);
				if (TreeView != null && TreeView.CheckNodesRecursive)
					UpdateCheckedStateRecursive();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeVisible"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool Visible {
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		[DefaultValue(""), Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(false)]
		public string DataPath {
			get { return GetStringProperty("DataPath", string.Empty); }
			protected internal set { SetStringProperty("DataPath", string.Empty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem {
			get { return dataItem; }
			set { dataItem = value; }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			TreeViewNode srcNode = source as TreeViewNode;
			if (srcNode == null)
				return;
			AllowCheck = srcNode.AllowCheck;
			ClientVisible = srcNode.ClientVisible;
			ClientEnabled = srcNode.ClientEnabled;
			Name = srcNode.Name;
			NavigateUrl = srcNode.NavigateUrl;
			Target = srcNode.Target;
			Text = srcNode.Text;
			ToolTip = srcNode.ToolTip;
			Template = srcNode.Template;
			TextTemplate = srcNode.TextTemplate;
			Image.Assign(srcNode.Image);
			NodeStyle.Assign(srcNode.NodeStyle);
			TextStyle.Assign(srcNode.TextStyle);
			ImageStyle.Assign(srcNode.ImageStyle);
			CheckBoxStyle.Assign(srcNode.CheckBoxStyle);
			Expanded = srcNode.Expanded;
			Enabled = srcNode.Enabled;
			CheckState = srcNode.CheckState;
			Visible = srcNode.Visible;
			DataItem = srcNode.DataItem;
			Nodes.Assign(srcNode.Nodes);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewNodeNodes"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public virtual TreeViewNodeCollection Nodes {
			get {
				if(this.nodes == null)
					this.nodes = CreateNodesCollection();
				return this.nodes;
			}
		}
		protected virtual TreeViewNodeCollection CreateNodesCollection() {
			return new TreeViewNodeCollection(this);
		}
		protected virtual internal int Depth {
			get {
				if (TreeView != null)
					return Parent == null ? -1 : Parent.Depth + 1;
				return -1;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeViewNode Parent {
			get { return OwningCollection != null ? OwningCollection.OwningNode : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ASPxTreeView TreeView {
			get {
				if (this.treeView != null)
					return this.treeView;
				return OwningCollection != null ? OwningCollection.TreeView : null;
			}
		}
		protected TreeViewNodeCollection OwningCollection {
			get { return Collection as TreeViewNodeCollection; }
		}
		protected internal bool IsLeaf {
			get { return this.isLeaf; }
			set { this.isLeaf = value; }
		}
		protected internal virtual string GetIndexPath() {
			StringBuilder indexPathBuilder = new StringBuilder();
			indexPathBuilder.Append(Index);
			for (TreeViewNode node = this; node.Parent.Depth != -1; node = node.Parent)
				indexPathBuilder.Insert(0, node.Parent.Index.ToString() + IndexPathSeparator);
			return indexPathBuilder.ToString();
		}
		protected internal string GetID() {
			return NodeIDPrefix + GetIndexPath();
		}
		static internal string GetIndexPathByID(string id) {
			return id.Replace(NodeIDPrefix, string.Empty);
		}
		protected internal virtual void SetCheckState(CheckState value) {
			SetEnumProperty("CheckState", CheckState.Unchecked, value);
		}
		protected internal void UpdateCheckedStateRecursive() {
			UpdateDescendantsCheckedState(this);
			UpdateAncestorsCheckedState(this);
		}
		static internal void UpdateAncestorsCheckedState(TreeViewNode node) {
			if (node.Parent == null)
				return;
			bool parentChecked = true;
			bool parentUnchecked = true;
			foreach (TreeViewNode siblingNode in node.Parent.Nodes) {
				if(siblingNode.CheckState != CheckState.Checked)
					parentChecked = false;
				if(siblingNode.CheckState != CheckState.Unchecked)
					parentUnchecked = false;
			}
			node.Parent.SetCheckState(parentChecked ? CheckState.Checked : (parentUnchecked ? CheckState.Unchecked : CheckState.Indeterminate));
			UpdateAncestorsCheckedState(node.Parent);
		}
		static internal void UpdateDescendantsCheckedState(TreeViewNode node) {
			if (!(node as IHierarchyData).HasChildren)
				return;
			foreach (TreeViewNode subnode in node.Nodes) {
				subnode.SetCheckState(node.CheckState);
				UpdateDescendantsCheckedState(subnode);
			}
		}
		protected internal string GetRenderingText() {
			return string.Format(TreeView.TextFormatString, Text);
		}
		protected internal string GetRenderingNavigateUrl() {
			if (string.IsNullOrEmpty(NavigateUrl) && TreeView.IsAccessibilityCompliantRender())
				return RenderUtils.AccessibilityEmptyUrl;
			return string.Format(TreeView.NavigateUrlFormatString, NavigateUrl);
		}
		protected internal string GetRenderingTarget() {
			return string.IsNullOrEmpty(Target) ? TreeView.Target : Target;
		}
		protected override bool IsLoading() {
			if (TreeView != null)
				return (TreeView as IWebControlObject).IsLoading();
			return base.IsLoading();
		}
		protected override bool IsDesignMode() {
			if (TreeView != null)
				return (TreeView as IWebControlObject).IsDesignMode();
			return base.IsDesignMode();
		}
		protected override void LayoutChanged() {
			if (TreeView != null) {
				(TreeView as IWebControlObject).LayoutChanged();
				return;
			}
			base.LayoutChanged();
		}
		protected override void TemplatesChanged() {
			if (TreeView != null) {
				(TreeView as IWebControlObject).TemplatesChanged();
				return;
			}
			base.TemplatesChanged();
		}
		protected Control FindControl(string id, string templateContainerID) {
			return TemplateContainerBase.FindTemplateControl(TreeView, templateContainerID, id);			
		}
		public Control FindControl(string id) {
			if (TreeView == null)
				return null;
			return FindControl(id, TreeView.GetNodeTextTemplateContainerID(this)) ??
				FindControl(id, TreeView.GetNodeTemplateContainerID(this));
		}
		public override string ToString() {
			if (!string.IsNullOrEmpty(Text))
				return Text;
			return GetType().Name;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Nodes, Image, NodeStyle, TextStyle, ImageStyle, CheckBoxStyle };
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return Nodes.GetEnumerator();
		}
		IHierarchicalEnumerable IHierarchyData.GetChildren() {
			return Nodes;
		}
		IHierarchyData IHierarchyData.GetParent() {
			return Parent;
		}
		bool IHierarchyData.HasChildren { get { return Nodes.Count > 0; } }
		object IHierarchyData.Item { get { return this; } }
		string IHierarchyData.Path { get { return string.Empty; } }
		string IHierarchyData.Type { get { return GetType().Name; } }
		protected override IList GetDesignTimeItems() {
			return (IList)Nodes;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Nodes" };
		}
	}
	public class TreeViewVirtualNode : TreeViewNode {
		VirtualModeTreeViewDataMediator dataMediator = null;
		VirtualModeTreeViewDataMediator innerDataMediator = new VirtualModeTreeViewDataMediator(null);
		bool syncedWithMediator = false;
		CheckState checkState = CheckState.Unchecked;
		bool expanded = false;
		string name = null;
		string parentIndexPath = null;
		public TreeViewVirtualNode(string name) {
			Name = name;
		}
		public TreeViewVirtualNode(string name, string text)
			: this(name, text, string.Empty, string.Empty, string.Empty) {
		}
		public TreeViewVirtualNode(string name, string text, string imageUrl)
			: this(name, text, imageUrl, string.Empty, string.Empty) {
		}
		public TreeViewVirtualNode(string name, string text, string imageUrl, string navigateUrl)
			: this(name, text, imageUrl, navigateUrl, string.Empty) {
		}
		public TreeViewVirtualNode(string name, string text, string imageUrl, string navigateUrl, string target)
			: base(text, name, imageUrl, navigateUrl, target) {
		}
		protected internal TreeViewVirtualNode(ASPxTreeView treeView)
			: base(treeView) {
		}
		protected internal bool SyncedWithMediator {
			get { return this.syncedWithMediator; }
			set { this.syncedWithMediator = value; }
		}
		protected internal VirtualModeTreeViewDataMediator DataMediator {
			get {
				if (this.dataMediator == null)
					throw new InvalidOperationException(StringResources.TreeView_VirtualNodeCreatedOutsideEvent);
				return this.dataMediator;
			}
			set { this.dataMediator = value; }
		}
		protected internal VirtualModeTreeViewDataMediator InnerDataMediator {
			get { return this.innerDataMediator; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("TreeViewVirtualNodeTreeView")]
#endif
		public override ASPxTreeView TreeView {
			get {
				if (this.dataMediator != null)
					return DataMediator.TreeView;
				return base.TreeView;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("TreeViewVirtualNodeNodes")]
#endif
		public override TreeViewNodeCollection Nodes {
			get {
				if(NeedForceNodesPopulation())
					ForceNodesPopulation();
				return base.Nodes;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsLeaf {
			get { return base.IsLeaf; }
			set { base.IsLeaf = value; }
		}
		protected internal override int Depth {
			get {
				if (Parent == null && !string.IsNullOrEmpty(ParentIndexPath))
					return 1;
				return base.Depth;
			}
		}
		protected internal void ForceNodesPopulation() {
			if (base.Nodes.Count != 0 || IsLeaf)
				return;
			base.Nodes.AddRange(DataMediator.PopulateVirtualNodes(Name));
			DataMediator.SyncNodesInnerStateWithMediator(base.Nodes);
			DataMediator.SyncNodesWithMediator(GetID(), base.Nodes);
		}
		protected internal string ParentIndexPath {
			get { return this.parentIndexPath; }
			set { this.parentIndexPath = value; }
		}
		protected internal override string GetIndexPath() {
			StringBuilder indexPathBuilder = new StringBuilder();
			indexPathBuilder.Append(Index);
			TreeViewVirtualNode node = this;
			for (; node.Parent != null && node.Parent.Depth != -1; node = node.Parent as TreeViewVirtualNode)
				indexPathBuilder.Insert(0, node.Parent.Index.ToString() + IndexPathSeparator);
			if (!string.IsNullOrEmpty(node.ParentIndexPath))
				indexPathBuilder.Insert(0, node.ParentIndexPath + IndexPathSeparator);
			return indexPathBuilder.ToString();
		}
		internal static string GetParentID(string id) {
			if (string.IsNullOrEmpty(id))
				return null;
			string indexPath = GetIndexPathByID(id);
			string[] pathIndices = indexPath.Split(IndexPathSeparator);
			if (pathIndices.Length < 2)
				return null;
			return NodeIDPrefix + string.Join(IndexPathSeparator.ToString(), pathIndices, 0, pathIndices.Length - 1);
		}
#if !SL
	[DevExpressWebLocalizedDescription("TreeViewVirtualNodeName")]
#endif
		public override string Name {
			get {
				if (!SyncedWithMediator)
					return this.name;
				return DataMediator.GetNodeName(GetID());
			}
			set {
				if (!SyncedWithMediator)
					this.name = value;
				else
					DataMediator.SetNodeName(GetID(), value);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("TreeViewVirtualNodeExpanded")]
#endif
		public override bool Expanded {
			get {
				if (!SyncedWithMediator)
					return this.expanded;
				return DataMediator.GetNodeExpanded(GetID());
			}
			set {
				if (!SyncedWithMediator) {
					this.expanded = value;
					InnerDataMediator.SetNodeExpanded(GetID(), value);
				}
				else
					DataMediator.SetNodeExpanded(GetID(), value);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("TreeViewVirtualNodeChecked")]
#endif
		public override bool Checked {
			get { return CheckState == CheckState.Checked; }
			set { CheckState = value ? CheckState.Checked : CheckState.Unchecked; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("TreeViewVirtualNodeCheckState")]
#endif
		public override CheckState CheckState {
			get {
				if(!SyncedWithMediator)
					return this.checkState;
				return DataMediator.GetNodeCheckState(GetID());
			}
			internal set {
				if(SyncedWithMediator && CheckState == value)
					return;
				SetCheckState(value);
			}
		}
		protected internal override void SetCheckState(CheckState value) {
			if (!SyncedWithMediator) {
				this.checkState = value;
				InnerDataMediator.SetNodeCheckState(GetID(), value);
			}
			else
				DataMediator.SetNodeCheckState(GetID(), value);
		}
		protected bool NeedForceNodesPopulation() {
			if(Expanded || this == DataMediator.RootNode)
				return true;
			if(TreeView.CanLoadClientState()) {
				string childNodeIDPrefix = GetID() + IndexPathSeparator;
				foreach(var key in DataMediator.GetNodeNames().Keys) {
					if(key.ToString().StartsWith(childNodeIDPrefix))
						return true;
				}
			}
			return false;
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class TreeViewNodeCollection : HierarchicalCollection<TreeViewNode> {
		public TreeViewNodeCollection()
			: base() {
		}
		public TreeViewNodeCollection(TreeViewNode node)
			: base(node) {
		}
		protected internal TreeViewNode OwningNode {
			get { return Owner as TreeViewNode; }
		}
		protected internal ASPxTreeView TreeView {
			get { return OwningNode != null ? OwningNode.TreeView : null; }
		}
		protected override void OnBeforeAdd(CollectionItem item) {
			if (TreeView == null)
				return;
			if (TreeView.IsVirtualMode() && !(item is TreeViewVirtualNode))
				throw new ArgumentException(StringResources.TreeView_NonVirtualNodeAddedInVirtualMode);
			else if (!TreeView.IsVirtualMode() && (item is TreeViewVirtualNode))
				throw new ArgumentException(StringResources.TreeView_VirtualNodeAddedInRealMode);
		}
		public TreeViewNode Add() {
			return AddInternal(new TreeViewNode());
		}
		public TreeViewNode Add(string text) {
			return Add(text, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public TreeViewNode Add(string text, string name) {
			return Add(text, name, string.Empty, string.Empty, string.Empty);
		}
		public TreeViewNode Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, string.Empty, string.Empty);
		}
		public TreeViewNode Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, string.Empty);
		}
		public TreeViewNode Add(string text, string name, string imageUrl, string navigateUrl,
			string target) {
			return AddInternal(new TreeViewNode(text, name, imageUrl, navigateUrl, target));
		}
		public int IndexOfName(string name) {
			return IndexOf(delegate(TreeViewNode node) {
				return node.Name == name;
			});
		}
		public int IndexOfText(string text) {
			return IndexOf(delegate(TreeViewNode node) {
				return node.Text == text;
			});
		}
		public TreeViewNode FindByName(string name) {
			return FindRecursive(delegate(TreeViewNode node) {
				return node.Name == name;
			});
		}
		public TreeViewNode FindByText(string text) {
			return FindRecursive(delegate(TreeViewNode node) {
				return node.Text == text;
			});
		}
		protected override void OnChanged() {
			if(TreeView != null && !TreeView.IsLoading() && !(TreeView.IsRendering && TreeView.PreRendered))  {
				TreeView.ResetViewStateStoringFlag();
				TreeView.ResetControlHierarchy();
			}
		}
		protected internal TreeViewNode CreateNode() {
			return CreateKnownType(0) as TreeViewNode;
		}
	}
}
