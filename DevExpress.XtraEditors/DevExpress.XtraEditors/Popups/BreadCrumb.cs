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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Editors;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Properties;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemBreadCrumbEdit : RepositoryItemComboBox, IBreadCrumbValidationClient {
		string pathSeparator;
		Image rootGlyph;
		bool showRootGlyph;
		bool sortNodesByCaption;
		string path;
		int rootImageIndex;
		BreadCrumbNode selectedNode;
		BreadCrumbMode breadCrumbMode;
		object images;
		int imageIndex, nodeDropDownRowCount;
		BreadCrumbNodeCollection nodes;
		BreadCrumbValidationController validationController;
		BreadCrumbNavigationController navigationController;
		BreadCrumbHistory history;
		public const string DefaultPathSeparator = "\\";
		const int DefaultNodeDropDownRowCount = 20;
		static readonly object pathChanged = new object();
		static readonly object nodeClick = new object();
		static readonly object nodeDropDownButtonClick = new object();
		static readonly object nodeChanged = new object();
		static readonly object selectedNodeChanged = new object();
		static readonly object rootGlyphClick = new object();
		static readonly object selectorClientEmptySpaceClick = new object();
		static readonly object queryChildNodes = new object();
		static readonly object validatePath = new object();
		static readonly object pathRejected = new object();
		static readonly object newNodeAdding = new object();
		static readonly object showUserActionMenu = new object();
		static readonly object shownNodeDropdown = new object();
		static readonly object hiddenNodeDropdown = new object();
		public RepositoryItemBreadCrumbEdit() {
			this.pathSeparator = DefaultPathSeparator;
			this.breadCrumbMode = BreadCrumbMode.Select;
			this.rootGlyph = null;
			this.showRootGlyph = true;
			this.sortNodesByCaption = false;
			this.rootImageIndex = -1;
			this.path = string.Empty;
			this.selectedNode = null;
			this.images = null;
			this.imageIndex = -1;
			this.nodeDropDownRowCount = DefaultNodeDropDownRowCount;
			this.validationController = CreateValidationController();
			this.navigationController = CreateNavigationController();
			this.history = CreateHistory();
			this.nodes = CreateNodeCollection();
			this.nodes.CollectionChanged += OnNodeCollectionChanged;
		}
		protected virtual BreadCrumbHistory CreateHistory() {
			return new BreadCrumbHistory(this);
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditHistory"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BreadCrumbHistory History {
			get { return history; }
		}
		protected virtual BreadCrumbNodeCollection CreateNodeCollection() {
			return new BreadCrumbNodeCollection(this);
		}
		protected virtual BreadCrumbValidationController CreateValidationController() {
			return new BreadCrumbValidationController(this);
		}
		protected virtual BreadCrumbNavigationController CreateNavigationController() {
			return new BreadCrumbNavigationController(this);
		}
		protected internal BreadCrumbValidationController ValidationController { get { return validationController; } }
		protected internal BreadCrumbNavigationController NavigationController { get { return navigationController; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditBreadCrumbMode"),
#endif
 DefaultValue(BreadCrumbMode.Select)]
		public virtual BreadCrumbMode BreadCrumbMode {
			get { return breadCrumbMode; }
			set {
				if(BreadCrumbMode == value) return;
				breadCrumbMode = value;
				OnBreadCrumbModeChanged();
			}
		}
		protected virtual void OnBreadCrumbModeChanged() {
			OnPropertiesChanged();
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditPathSeparator"),
#endif
 DefaultValue(DefaultPathSeparator)]
		public virtual string PathSeparator {
			get { return pathSeparator; }
			set {
				if(PathSeparator == value)
					return;
				pathSeparator = value;
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditRootImageIndex"),
#endif
 DefaultValue(-1), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images")]
		public virtual int RootImageIndex {
			get { return rootImageIndex; }
			set {
				if(RootImageIndex == value)
					return;
				rootImageIndex = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditRootGlyph"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image RootGlyph {
			get { return rootGlyph; }
			set {
				if(RootGlyph == value)
					return;
				rootGlyph = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditShowRootGlyph"),
#endif
 DefaultValue(true)]
		public virtual bool ShowRootGlyph {
			get { return showRootGlyph; }
			set {
				if(ShowRootGlyph == value)
					return;
				showRootGlyph = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(false)]
		public virtual bool SortNodesByCaption {
			get { return sortNodesByCaption; }
			set {
				if(SortNodesByCaption == value)
					return;
				sortNodesByCaption = value;
				OnSortNodesByCaptionChanged();
			}
		}
		protected virtual void OnSortNodesByCaptionChanged() {
			if(SortNodesByCaption)
				Nodes.SortIfRequired();
			OnPropertiesChanged();
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditPath"),
#endif
 DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string Path {
			get { return path; }
			set {
				if(Path == value)
					return;
				if(!OnPathChanging(value))
					return;
				path = value;
				OnPathChanged();
			}
		}
		protected virtual bool OnPathChanging(string newPath) {
			return CheckPath(newPath);
		}
		protected virtual bool CheckPath(string newPath) {
			if(!ValidatePathCore(newPath)) {
				if(OwnerEdit != null) {
					OwnerEdit.EditValue = (newPath == null ? string.Empty : Path);
				}
				RaisePathRejected(new BreadCrumbPathRejectedEventArgs(newPath));
				return false;
			}
			return true;
		}
		protected virtual void OnPathChanged() {
			RaisePathChanged(new BreadCrumbPathChangedEventArgs(Path));
			if(OwnerEdit != null) {
				OwnerEdit.EditValue = Path;
			}
			NavigationController.PathChanged(Path);
			OnPropertiesChanged();
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditSelectedNode"),
#endif
 DefaultValue(null), TypeConverter("DevExpress.XtraEditors.Design.BreadCrumbSelectedNodeTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
		public virtual BreadCrumbNode SelectedNode {
			get { return selectedNode; }
			set {
				if(SelectedNode == value)
					return;
				BreadCrumbNode prev = SelectedNode;
				if(!OnSelectedNodeChanging(value))
					return;
				selectedNode = value;
				OnSelectedNodeChanged(prev, SelectedNode);
			}
		}
		protected virtual bool OnSelectedNodeChanging(BreadCrumbNode newNode) {
			if(newNode == null) return true;
			return CheckPath(newNode.Path);
		}
		protected virtual void OnSelectedNodeChanged(BreadCrumbNode prev, BreadCrumbNode current) {
			Path = current != null ? current.Path : null;
			OnPropertiesChanged();
		}
		public ReadOnlyCollection<BreadCrumbNode> GetAllNodes() {
			return new ReadOnlyCollection<BreadCrumbNode>(Nodes.GetAllNodes());
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditCaseSensitiveSearch"),
#endif
 DefaultValue(false)]
		public override bool CaseSensitiveSearch {
			get { return base.CaseSensitiveSearch; }
			set { base.CaseSensitiveSearch = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditImages"),
#endif
 DefaultValue(null), DXCategory(CategoryName.Appearance), TypeConverter(typeof(ImageCollectionImagesConverter))]
		public virtual object Images {
			get { return images; }
			set { images = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditImageIndex"),
#endif
 DefaultValue(-1), DXCategory(CategoryName.Appearance), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images")]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditNodeDropDownRowCount"),
#endif
 DefaultValue(DefaultNodeDropDownRowCount), DXCategory(CategoryName.Appearance)]
		public int NodeDropDownRowCount {
			get { return nodeDropDownRowCount; }
			set {
				if(NodeDropDownRowCount == value) return;
				nodeDropDownRowCount = value;
				OnPropertiesChanged();
			}
		}
		public virtual void GoUp() {
			if(!CanGoUp) return;
			Path = GetSelectedNode().Parent.Path;
		}
		[ Browsable(false)]
		public virtual bool CanGoUp {
			get {
				BreadCrumbNode selNode = GetSelectedNode();
				if(selNode == null || selNode.Persistent) return false;
				BreadCrumbNode parent = selNode.Parent;
				return parent != null && !parent.Persistent;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditCanGoBack"),
#endif
 Browsable(false)]
		public virtual bool CanGoBack {
			get { return NavigationController.CanGoBack; }
		}
		public virtual void GoBack() { NavigationController.GoBack(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditCanGoForward"),
#endif
 Browsable(false)]
		public virtual bool CanGoForward {
			get { return NavigationController.CanGoForward; }
		}
		public virtual void GoForward() { NavigationController.GoForward(); }
		public virtual BreadCrumbHistory GetNavigationHistory() {
			return NavigationController.GetNavigationHistory();
		}
		public virtual int GetNavigationHistoryCurrentItemIndex() { return NavigationController.GetNavigationHistoryCurrentItemIndex(); }
		public virtual void SetNavigationHistoryCurrentItemIndex(int itemIndex) { NavigationController.SetNavigationHistoryCurrentItemIndex(itemIndex); }
		public virtual void ResetNavigationHistory() { NavigationController.ResetNavigationHistory(); }
		protected void OnNodeCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Refresh) {
				if(Nodes.Count == 0) OnResetNodeCollection();
			}
		}
		protected virtual void OnResetNodeCollection() {
			if(OwnerEdit != null) OwnerEdit.OnResetNodeCollection();
		}
		[Localizable(true), DXCategory(CategoryName.Data), RefreshProperties(RefreshProperties.All), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditNodes"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BreadCrumbNodeCollection Nodes { get { return nodes; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditPathChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbPathChangedEventHandler PathChanged {
			add { Events.AddHandler(pathChanged, value); }
			remove { Events.RemoveHandler(pathChanged, value); }
		}
		protected internal virtual void RaisePathChanged(BreadCrumbPathChangedEventArgs e) {
			BreadCrumbPathChangedEventHandler handler = (BreadCrumbPathChangedEventHandler)this.Events[pathChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditNodeClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbNodeClickEventHandler NodeClick {
			add { Events.AddHandler(nodeClick, value); }
			remove { Events.RemoveHandler(nodeClick, value); }
		}
		protected internal virtual void RaiseNodeClick(BreadCrumbNodeClickEventArgs e) {
			BreadCrumbNodeClickEventHandler handler = (BreadCrumbNodeClickEventHandler)this.Events[nodeClick];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditNodeDropDownButtonClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbNodeClickEventHandler NodeDropDownButtonClick {
			add { Events.AddHandler(nodeDropDownButtonClick, value); }
			remove { Events.RemoveHandler(nodeDropDownButtonClick, value); }
		}
		protected internal virtual void RaiseNodeDropDownButtonClick(BreadCrumbNodeClickEventArgs e) {
			BreadCrumbNodeClickEventHandler handler = (BreadCrumbNodeClickEventHandler)Events[nodeDropDownButtonClick];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditNodeChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbNodeChangedEventHandler NodeChanged {
			add { Events.AddHandler(nodeChanged, value); }
			remove { Events.RemoveHandler(nodeChanged, value); }
		}
		protected internal virtual void RaiseNodeChanged(BreadCrumbNodeChangedEventArgs e) {
			if(IsLockUpdate) return;
			CheckProperties(e);
			BreadCrumbNodeChangedEventHandler handler = (BreadCrumbNodeChangedEventHandler)this.Events[nodeChanged];
			if(handler != null) handler(GetEventSender(), e);
			LayoutChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditSelectedNodeChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbSelectedNodeChangedEventHandler SelectedNodeChanged {
			add { Events.AddHandler(selectedNodeChanged, value); }
			remove { Events.RemoveHandler(selectedNodeChanged, value); }
		}
		protected internal virtual void RaiseSelectedNodeChanged(BreadCrumbSelectedNodeChangedEventArgs e) {
			BreadCrumbSelectedNodeChangedEventHandler handler = (BreadCrumbSelectedNodeChangedEventHandler)this.Events[selectedNodeChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditRootGlyphClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler RootGlyphClick {
			add { Events.AddHandler(rootGlyphClick, value); }
			remove { Events.RemoveHandler(rootGlyphClick, value); }
		}
		protected internal virtual void RaiseRootGlyphClick(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[rootGlyphClick];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditSelectorClientEmptySpaceClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event MouseEventHandler SelectorClientEmptySpaceClick {
			add { Events.AddHandler(selectorClientEmptySpaceClick, value); }
			remove { Events.RemoveHandler(selectorClientEmptySpaceClick, value); }
		}
		protected internal virtual void RaiseSelectorClientEmptySpaceClick(MouseEventArgs e) {
			MouseEventHandler handler = (MouseEventHandler)this.Events[selectorClientEmptySpaceClick];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditQueryChildNodes"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbQueryChildNodesEventHandler QueryChildNodes {
			add { Events.AddHandler(queryChildNodes, value); }
			remove { Events.RemoveHandler(queryChildNodes, value); }
		}
		bool inQueryChildren = false;
		protected internal virtual void RaiseQueryChildNodes(BreadCrumbQueryChildNodesEventArgs e) {
			this.inQueryChildren = true;
			try {
				BreadCrumbQueryChildNodesEventHandler handler = (BreadCrumbQueryChildNodesEventHandler)this.Events[queryChildNodes];
				if(handler != null) handler(GetEventSender(), e);
			}
			finally {
				this.inQueryChildren = false;
			}
		}
		internal bool InQueryChildren {  get { return inQueryChildren; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditValidatePath"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbValidatePathEventHandler ValidatePath {
			add { Events.AddHandler(validatePath, value); }
			remove { Events.RemoveHandler(validatePath, value); }
		}
		protected internal virtual void RaiseValidatePath(BreadCrumbValidatePathEventArgs e) {
			BreadCrumbValidatePathEventHandler handler = (BreadCrumbValidatePathEventHandler)this.Events[validatePath];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual bool RaiseValidatePath(string path) {
			BreadCrumbValidatePathEventArgs e = new BreadCrumbValidatePathEventArgs(path, this);
			RaiseValidatePath(e);
			return e.ValidationResult == BreadCrumbValidatePathResult.CreateNodes;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditPathRejected"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbPathRejectedEventHandler PathRejected {
			add { Events.AddHandler(pathRejected, value); }
			remove { Events.RemoveHandler(pathRejected, value); }
		}
		protected internal virtual void RaisePathRejected(BreadCrumbPathRejectedEventArgs e) {
			BreadCrumbPathRejectedEventHandler handler = (BreadCrumbPathRejectedEventHandler)this.Events[pathRejected];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditNewNodeAdding"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbNewNodeAddingEventHandler NewNodeAdding {
			add { Events.AddHandler(newNodeAdding, value); }
			remove { Events.RemoveHandler(newNodeAdding, value); }
		}
		protected internal virtual void RaiseNewNodeAdding(BreadCrumbNewNodeAddingEventArgs e) {
			BreadCrumbNewNodeAddingEventHandler handler = (BreadCrumbNewNodeAddingEventHandler)this.Events[newNodeAdding];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditShowUserActionMenu"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbShowUserActionMenuEventHandler ShowUserActionMenu {
			add { Events.AddHandler(showUserActionMenu, value); }
			remove { Events.RemoveHandler(showUserActionMenu, value); }
		}
		protected internal virtual void RaiseShowUserActionMenu(BreadCrumbShowUserActionMenuEventArgs e) {
			BreadCrumbShowUserActionMenuEventHandler handler = (BreadCrumbShowUserActionMenuEventHandler)Events[showUserActionMenu];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditShownNodeDropdown"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbShownNodeDropDownEventHandler ShownNodeDropdown {
			add { Events.AddHandler(shownNodeDropdown, value); }
			remove { Events.RemoveHandler(shownNodeDropdown, value); }
		}
		protected internal virtual void RaiseShownNodeDropdown(BreadCrumbShownNodeDropDownEventArgs e) {
			BreadCrumbShownNodeDropDownEventHandler handler = (BreadCrumbShownNodeDropDownEventHandler)Events[shownNodeDropdown];
			if(handler != null) handler(GetEventSender(), e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBreadCrumbEditHiddenNodeDropDown"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbHiddenNodeDropDownEventHandler HiddenNodeDropDown {
			add { Events.AddHandler(hiddenNodeDropdown, value); }
			remove { Events.RemoveHandler(hiddenNodeDropdown, value); }
		}
		protected internal virtual void RaiseHiddenNodeDropDown(BreadCrumbHiddenNodeDropDownEventArgs e) {
			BreadCrumbHiddenNodeDropDownEventHandler handler = (BreadCrumbHiddenNodeDropDownEventHandler)Events[hiddenNodeDropdown];
			if(handler != null) handler(GetEventSender(), e);
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemBreadCrumbEdit source = item as RepositoryItemBreadCrumbEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.breadCrumbMode = source.BreadCrumbMode;
				this.pathSeparator = source.PathSeparator;
				this.rootGlyph = source.RootGlyph;
				this.rootImageIndex = source.RootImageIndex;
				this.showRootGlyph = source.ShowRootGlyph;
				this.sortNodesByCaption = source.SortNodesByCaption;
				this.path = source.Path;
				this.selectedNode = source.SelectedNode;
				this.imageIndex = source.ImageIndex;
				this.nodeDropDownRowCount = source.NodeDropDownRowCount;
				this.images = source.images;
				this.AssignNodes(source);
				this.history.Clear();
				this.history.AddRange(source.history);
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(pathChanged, source.Events[pathChanged]);
			Events.AddHandler(nodeClick, source.Events[nodeClick]);
			Events.AddHandler(nodeDropDownButtonClick, source.Events[nodeDropDownButtonClick]);
			Events.AddHandler(nodeChanged, source.Events[nodeChanged]);
			Events.AddHandler(selectedNodeChanged, source.Events[selectedNodeChanged]);
			Events.AddHandler(rootGlyphClick, source.Events[rootGlyphClick]);
			Events.AddHandler(selectorClientEmptySpaceClick, source.Events[selectorClientEmptySpaceClick]);
			Events.AddHandler(queryChildNodes, source.Events[queryChildNodes]);
			Events.AddHandler(validatePath, source.Events[validatePath]);
			Events.AddHandler(pathRejected, source.Events[pathRejected]);
			Events.AddHandler(newNodeAdding, source.Events[newNodeAdding]);
			Events.AddHandler(showUserActionMenu, source.Events[showUserActionMenu]);
			Events.AddHandler(shownNodeDropdown, source.Events[shownNodeDropdown]);
			Events.AddHandler(hiddenNodeDropdown, source.Events[hiddenNodeDropdown]);
		}
		protected virtual void AssignNodes(RepositoryItemBreadCrumbEdit source) {
			if(InBars) {
				Nodes.Clear(true);
				BreadCrumbNodeCollection sourceCol = new BreadCrumbNodeCollection();
				foreach(BreadCrumbNode n in source.Nodes) {
					sourceCol.Add(n.Clone(true));
				}
				foreach(BreadCrumbNode sourceNode in sourceCol) {
					Nodes.Add(sourceNode);
					sourceNode.SetProperties(this);
				}
			}
			else {
				if(!InGrid) {
					Nodes.Clear();
				}
				Nodes.FastCopy(source.nodes);
			}
		}
		internal bool InBars {
			get { return OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Bars; }
		}
		internal bool InGrid {
			get { return OwnerEdit != null && OwnerEdit.InplaceType == InplaceType.Grid; }
		}
		protected virtual void CheckProperties(BreadCrumbNodeChangedEventArgs e) {
			if(e.ChangeType == BreadCrumbNodeChangeType.NodeDeleted) {
				if(SelectedNode != null && (object.ReferenceEquals(SelectedNode, e.Node) || e.Node.IsParentOf(SelectedNode))) {
					SelectedNode = null;
				}
			}
		}
		protected override ComboBoxItemCollection CreateItemCollection() {
			return new BreadCrumbItemCollection(this);
		}
		protected internal override bool UpdateEditValueFromPopup {
			get {
				if(OwnerEdit != null) return OwnerEdit.UpdateEditValueFromPopup;
				return base.UpdateEditValueFromPopup;
			}
		}
		#region Internal
		protected internal virtual bool IsPartOfPath(BreadCrumbNode node) {
			BreadCrumbNode selNode = GetSelectedNode();
			if(selNode == null) return false;
			BreadCrumbNode current = selNode;
			while(current != null) {
				if(current.Equals(node)) return true;
				current = current.Parent;
			}
			return false;
		}
		protected internal BreadCrumbNode GetSelectedNode() {
			if(OwnerEdit != null) {
				return OwnerEdit.ViewInfo.GetSelectedNode();
			}
			return SelectedNode;
		}
		protected internal static bool IsStringEquals(RepositoryItemBreadCrumbEdit properties, string val1, string val2) {
			if(properties != null) return string.Equals(val1, val2, properties.GetStringComparisonOptions());
			return string.Equals(val1, val2, StringComparison.Ordinal);
		}
		protected virtual StringComparison GetStringComparisonOptions() {
			return CaseSensitiveSearch ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
		}
		protected internal virtual bool GetIsFlagged(BreadCrumbNode node) {
			if(node.InOverflow) return false;
			BreadCrumbNode current = GetSelectedNode();
			while(current != null) {
				if(IsStringEquals(this, current.NormalizedPath, node.NormalizedPath))
					return true;
				current = current.Parent;
			}
			return false;
		}
		protected internal virtual bool ValidatePathCore(string newPath) {
			if(IsDesignMode) return true;
			return DoValidate(newPath);
		}
		protected internal virtual void OnAddPathToHistory(string path) {
			if(DoValidate(path)) History.Insert(0, new BreadCrumbHistoryItem(path));
		}
		protected virtual bool DoValidate(string newPath) {
			return ValidationController.Validate(BreadCrumbValidationOptions.GetOptions(this), newPath);
		}
		protected internal virtual void InitAutoCompleteItems(BreadCrumbNode node) {
			BeginUpdate();
			try {
				Items.Clear();
				node.EnsureRaiseNodePopulate();
				foreach(BreadCrumbNode childNode in node.ChildNodes) {
					Items.Add(childNode.Path);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void InitHistoryItems() {
			BeginUpdate();
			try {
				Items.Clear();
				foreach(BreadCrumbHistoryItem item in History) {
					Items.Add(new ImageComboBoxItem(item, ImageIndex));
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void AcceptPopupValue(object value) {
			if(value == null) return;
			bool nodePopupClosed = (OwnerEdit != null && OwnerEdit.IsNodeExpanded);
			if(nodePopupClosed) {
				BreadCrumbNode node = value as BreadCrumbNode;
				if(node != null) {
					RaiseNodeClick(new BreadCrumbNodeClickEventArgs(node));
					if(node.CanMove()) SelectedNode = node;
					else {
						Path = node.GetValue();
					}
				}
				else Path = value as string;
			}
			else {
				History.MovePathOnTop(Path = value.ToString());
			}
		}
		protected internal virtual void EnsureEditMode() {
			if(OwnerEdit != null) OwnerEdit.EnsureEditMode();
		}
		#endregion
		#region IBreadCrumbValidationClient
		void IBreadCrumbValidationClient.EnsureSelectNode(BreadCrumbNode node) {
			EnsureSelectNode(node);
		}
		protected virtual void EnsureSelectNode(BreadCrumbNode node) {
			if(node == null) return;
			BreadCrumbNode prev = this.selectedNode;
			this.selectedNode = node;
			if(node != prev) {
				RaiseSelectedNodeChanged(new BreadCrumbSelectedNodeChangedEventArgs(prev, node));
			}
		}
		bool IBreadCrumbValidationClient.RaiseValidatePath(string path) {
			return RaiseValidatePath(path);
		}
		void IBreadCrumbValidationClient.RaiseNewNodeAdding(string value, BreadCrumbNode newNode) {
			RaiseNewNodeAdding(new BreadCrumbNewNodeAddingEventArgs(value, newNode));
		}
		#endregion
		#region Hidden Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ImmediatePopup {
			get { return true; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ComboBoxItemCollection Items { get { return base.Items; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Sorted {
			get { return base.Sorted; }
			set { base.Sorted = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TextEditStyles TextEditStyle {
			get { return TextEditStyles.Standard; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoComplete {
			get { return base.AutoComplete; }
			set { base.AutoComplete = value; }
		}
		#endregion
		#region Repository Item Common
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemBreadCrumbEdit Properties { get { return this; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "BreadCrumbEdit"; } }
		[Browsable(false)]
		public override bool RequireDisplayTextSorting { get { return true; } }
		public override bool IsFilterLookUp { get { return true; } }
		#endregion
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.nodes != null) {
					this.nodes.CollectionChanged -= OnNodeCollectionChanged;
				}
				if(ValidationController != null) {
					ValidationController.Dispose();
				}
				if(NavigationController != null) {
					NavigationController.Dispose();
				}
				if(History != null) {
					History.Dispose();
				}
			}
			this.rootGlyph = null;
			this.images = null;
			this.validationController = null;
			this.navigationController = null;
			this.history = null;
			base.Dispose(disposing);
		}
		#endregion
		[Browsable(false)]
		public new BreadCrumbEdit OwnerEdit { get { return base.OwnerEdit as BreadCrumbEdit; } }
	}
}
namespace DevExpress.XtraEditors {
	[
	Designer("DevExpress.XtraEditors.Design.BreadCrumbDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	SmartTagAction(typeof(BreadCrumbActions), "Nodes", "Edit Nodes...", SmartTagActionType.CloseAfterExecute),
	SmartTagFilter(typeof(BreadCrumbEditFilter)),
	DefaultEvent("PathChanged"),
	ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "BreadCrumbEdit"),
	]
	public class BreadCrumbEdit : ComboBoxEdit {
		BreadCrumbEditHandler handler;
		BreadCrumbAutoCompleteController autoCompleteController;
		bool isNodeExpanded;
		BreadCrumbNodePopupForm nodePopupForm;
		Func<PopupBaseForm> popupFormCreator, defaultPopupFormCreator, nodePopupFormCreator;
		public BreadCrumbEdit() {
			this.isNodeExpanded = false;
			this.nodePopupForm = null;
			this.handler = CreateHandler();
			this.autoCompleteController = CreateAutoCompleteController();
			this.popupFormCreator = this.defaultPopupFormCreator = new Func<PopupBaseForm>(CreatePopupForm);
			this.nodePopupFormCreator = new Func<PopupBaseForm>(CreateBreadCrumbNodePopupForm);
		}
		protected virtual BreadCrumbEditHandler CreateHandler() {
			return new BreadCrumbEditHandler(this);
		}
		protected virtual void DoSetDefaultPopupCreator() { this.popupFormCreator = this.defaultPopupFormCreator; }
		protected virtual void DoSetNodePopupCreator() { this.popupFormCreator = this.nodePopupFormCreator; }
		protected internal BreadCrumbAutoCompleteController AutoCompleteController { get { return autoCompleteController; } }
		protected virtual BreadCrumbAutoCompleteController CreateAutoCompleteController() {
			return new BreadCrumbAutoCompleteController(this);
		}
		protected override bool AllowAutoSearchSelectionLength {
			get { return AutoCompleteController.AllowAutoSearchSelectionLength; }
		}
		protected override TextBoxMaskBox CreateMaskBoxInstance() {
			return new BreadCrumbMaskBox(this);
		}
		#region Events
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BreadCrumbEditPathChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event BreadCrumbPathChangedEventHandler PathChanged {
			add { Properties.PathChanged += value; }
			remove { Properties.PathChanged -= value; }
		}
		#endregion
		protected override void OnMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			Handler.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Handler.OnMouseLeave(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Handler.OnMouseMove(e);
		}
		protected override void OnLostFocus(EventArgs e) {
			if(Disposing) return;
			Handler.OnLostFocus();
			base.OnLostFocus(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			Handler.OnKeyDown(e);
		}
		protected override void OnMaskBox_LostFocus(object sender, EventArgs e) {
			base.OnMaskBox_LostFocus(sender, e);
			if(IsDisposed) return;
			SetPathInternal(Text);
		}
		protected internal BreadCrumbEditHandler Handler { get { return handler; } }
		protected internal override void LayoutChanged() {
			if(IsDisposing)
				return;
			base.LayoutChanged();
		}
		protected internal virtual void OnResetNodeCollection() {
			if(Path != null) Path = null;
		}
		protected internal virtual void OnRootGlyphClick() {
			Properties.RaiseRootGlyphClick(EventArgs.Empty);
			EnsureEditMode();
		}
		protected internal virtual void EnsureEditMode() {
			if(Properties.BreadCrumbMode == BreadCrumbMode.Edit)
				return;
			Properties.BreadCrumbMode = BreadCrumbMode.Edit;
			UpdateMaskBox(true);
			SelectEditValue();
		}
		protected internal virtual void EnsureSelectMode() {
			if(Properties.BreadCrumbMode == BreadCrumbMode.Select)
				return;
			Properties.BreadCrumbMode = BreadCrumbMode.Select;
		}
		protected internal override void OnPopupSelectedIndexChanged() {
			LayoutChanged();
		}
		public override void SelectAll() {
			MaskBox.Focus();
			base.SelectAll();
		}
		protected virtual void SelectEditValue() {
			SelectAll();
		}
		protected internal virtual void PostEditValue() {
			EnsureSelectMode();
			string text = Text;
			Properties.OnAddPathToHistory(text);
			Path = text;
		}
		protected internal void RejectEditValue() {
			EnsureSelectMode();
			Text = Properties.Path;
		}
		protected override void OnPressButton(EditorButtonObjectInfoArgs buttonInfo) {
			Properties.InitHistoryItems();
			base.OnPressButton(buttonInfo);
		}
		protected override void AcceptPopupValue(object val) {
			base.AcceptPopupValue(val);
			Properties.AcceptPopupValue(val);
		}
		protected override bool IsAcceptCloseMode(PopupCloseMode closeMode) {
			return closeMode != PopupCloseMode.Cancel;
		}
		protected internal virtual void ResetCaret() {
			SelectionStart = Text.Length;
			SelectionLength = 0;
		}
		protected override bool CanProcessAutoSearchText {
			get { return AutoCompleteController.CanProcessAutoSearchText; }
		}
		protected override void ProcessAutoSearchChar(KeyPressEventArgs e) {
			AutoCompleteController.ProcessAutoSearchChar(e);
		}
		protected override void UpdatePopupEditValueIndex(int prevIndex) {
		}
		protected override void OnPopupFormValueChanged() {
			EditValue = PopupForm.ResultValue;
		}
		protected override bool AllowSelectAllOnPopupClose {
			get { return false; }
		}
		protected override bool CanShowPopup {
			get {
				if(Properties.ReadOnly && Properties.ForceDisableButtonOnReadOnly) return false;
				return true;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BreadCrumbNode AddDefaultRootNode() {
			BreadCrumbNodeCollection nodes = Properties.Nodes;
			if(!nodes.IsEmpty) return null;
			BreadCrumbNode root = CreateDefaultRootNode();
			nodes.Add(root);
			return root;
		}
		protected virtual BreadCrumbNode CreateDefaultRootNode() {
			return new BreadCrumbNode("Root", "Root");
		}
		protected internal virtual void ShowNodePopup(BreadCrumbNode node, Point loc) {
			this.isNodeExpanded = true;
			EnsureNodePopupForm(loc, node);
			DoShowPopup();
			Properties.RaiseShownNodeDropdown(new BreadCrumbShownNodeDropDownEventArgs(node));
		}
		protected internal virtual void HideNodePopup() {
			if(IsPopupOpen) DoClosePopup(PopupCloseMode.Cancel);
		}
		protected override void DoClosePopup(PopupCloseMode closeMode) {
			base.DoClosePopup(closeMode);
			this.isNodeExpanded = false;
			DestroyPopupFormCore(false);
		}
		protected override void DestroyPopupFormCore(bool dispose) {
			base.DestroyPopupFormCore(dispose);
			if(dispose) this.nodePopupForm = null;
		}
		protected internal BreadCrumbNodePopupForm NodePopupForm { get { return nodePopupForm; } }
		protected void EnsureNodePopupForm(Point loc, BreadCrumbNode node) {
			if(NodePopupForm == null) {
				this.nodePopupForm = CreateBreadCrumbNodePopupForm();
			}
			NodePopupForm.Initialize(loc, node);
			DoSetNodePopupCreator();
			try {
				var popupForm = GetPopupForm();
			}
			finally {
				DoSetDefaultPopupCreator();
			}
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new BreadCrumbPopupForm(this);
		}
		protected BreadCrumbNodePopupForm CreateBreadCrumbNodePopupForm() {
			return NodePopupForm ?? CreateBreadCrumbNodePopupFormInstance();
		}
		protected virtual BreadCrumbNodePopupForm CreateBreadCrumbNodePopupFormInstance() {
			return new BreadCrumbNodePopupForm(this);
		}
		protected override PopupBaseForm GetPopupFormCore() {
			return this.popupFormCreator();
		}
		[Browsable(false)]
		public virtual bool IsNodeExpanded { get { return this.isNodeExpanded; } }
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			if(IsNodeExpanded) {
				BreadCrumbNode node = NodePopupForm != null ? NodePopupForm.Node : null;
				Properties.RaiseHiddenNodeDropDown(new BreadCrumbHiddenNodeDropDownEventArgs(node));
				ViewInfo.ResetExpandedNode();
			}
			else {
				Properties.RaiseClosed(new ClosedEventArgs(closeMode));
			}
		}
		protected internal override Rectangle CalcPopupFormBounds(Size size) {
			if(!IsNodeExpanded) return base.CalcPopupFormBounds(size);
			Point loc = PointToScreen(NodePopupForm.TargetLocation);
			Point newLoc = ControlUtils.CalcLocation(loc, loc, size);
			return ConstrainFormBounds(new Rectangle(newLoc, size));
		}
		protected internal void SetPathInternal(string newPath) {
			BeginInternalTextChange();
			try {
				Path = newPath;
			}
			finally {
				EndInternalTextChange();
			}
		}
		protected internal void SetEditValueInternal(object newValue) {
			BeginInternalTextChange();
			try {
				EditValue = newValue;
			}
			finally {
				EndInternalTextChange();
			}
		}
		protected internal virtual bool UpdateEditValueFromPopup {
			get {
				if(IsNodeExpanded) return false;
				return true;
			}
		}
		#region Hidden Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string Path {
			get { return Properties.Path; }
			set { Properties.Path = value; }
		}
		[RefreshProperties(RefreshProperties.All), DXCategory(CategoryName.Data), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object EditValue {
			get { return base.EditValue; }
			set { base.EditValue = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		#endregion
		#region Editor Common
		[Browsable(false)]
		public override string EditorTypeName { get { return "BreadCrumbEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BreadCrumbEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemBreadCrumbEdit Properties { get { return base.Properties as RepositoryItemBreadCrumbEdit; } }
		#endregion
		public BreadCrumbEditHitInfo CalcHitInfo(Point pt) {
			return (BreadCrumbEditHitInfo)ViewInfo.CalcHitInfo(pt);
		}
		public BreadCrumbEditHitInfo CalcHitInfo(int x, int y) {
			return CalcHitInfo(new Point(x, y));
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BreadCrumbEdit Selector { get { return this; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Handler != null) {
					Handler.Dispose();
				}
				if(AutoCompleteController != null) {
					AutoCompleteController.Dispose();
				}
				if(NodePopupForm != null) {
					DestroyPopupFormCore(false);
					NodePopupForm.Dispose();
					this.nodePopupForm = null;
				}
			}
			this.handler = null;
			this.autoCompleteController = null;
			base.Dispose(disposing);
		}
		protected internal new BreadCrumbEditViewInfo ViewInfo { get { return base.ViewInfo as BreadCrumbEditViewInfo; } }
	}
	public class BreadCrumbMaskBox : TextBoxMaskBox {
		public BreadCrumbMaskBox(BreadCrumbEdit ownerEdit)
			: base(ownerEdit) {
		}
		public new BreadCrumbEdit OwnerEdit { get { return base.OwnerEdit as BreadCrumbEdit; } }
	}
	public class BreadCrumbEditHandler : IDisposable {
		BreadCrumbEdit owner;
		BreadCrumbSelectorStateController stateController;
		BreadCrumbSelectorDropDownController dropDownController;
		public BreadCrumbEditHandler(BreadCrumbEdit owner) {
			this.owner = owner;
			this.stateController = CreateStateController();
			this.dropDownController = CreateDropDownController();
		}
		protected virtual BreadCrumbSelectorStateController CreateStateController() {
			return new BreadCrumbSelectorStateController(this);
		}
		protected virtual BreadCrumbSelectorDropDownController CreateDropDownController() {
			return new BreadCrumbSelectorDropDownController(this);
		}
		BreadCrumbEditHitInfo mouseDownHitInfo = null;
		public virtual void OnMouseDown(MouseEventArgs e) {
			this.mouseDownHitInfo = Owner.CalcHitInfo(e.Location);
			StateController.OnMouseDown(e);
			DropDownController.OnMouseDown(e);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			BreadCrumbEditHitInfo hitInfo = Owner.CalcHitInfo(e.Location);
			if(this.mouseDownHitInfo != null && hitInfo.IsMatch(this.mouseDownHitInfo)) {
				if(hitInfo.HitTest == EditHitTest.Glyph) {
					OnGlyphClick();
				}
			}
			if(this.mouseDownHitInfo != null) this.mouseDownHitInfo.Clear();
			StateController.OnMouseUp(e);
			DropDownController.OnMouseUp(e);
		}
		public virtual void OnMouseEnter(EventArgs e) {
			StateController.OnMouseEnter(e);
		}
		public virtual void OnMouseLeave(EventArgs e) {
			StateController.OnMouseLeave(e);
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			StateController.OnMouseMove(e);
			DropDownController.OnMouseMove(e);
		}
		public virtual void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter)
				OnEnterKeyDown();
			if(e.KeyCode == Keys.Escape)
				OnEscKeyDown();
		}
		protected virtual void OnEnterKeyDown() {
			if(Owner.Properties.BreadCrumbMode != BreadCrumbMode.Edit)
				return;
			Owner.PostEditValue();
		}
		public virtual void OnLostFocus() {
			if(!Owner.MaskBox.Focused) Owner.EnsureSelectMode();
		}
		protected virtual void OnEscKeyDown() {
			Owner.RejectEditValue();
		}
		protected virtual void OnGlyphClick() {
			Owner.OnRootGlyphClick();
		}
		public BreadCrumbSelectorStateController StateController { get { return stateController; } }
		public BreadCrumbSelectorDropDownController DropDownController { get { return dropDownController; } }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.stateController != null) {
					this.stateController.Dispose();
				}
				if(this.dropDownController != null) {
					this.dropDownController.Dispose();
				}
			}
			this.stateController = null;
			this.dropDownController = null;
			this.owner = null;
			this.mouseDownHitInfo = null;
		}
		public BreadCrumbEdit Owner { get { return owner; } }
	}
	public class BreadCrumbSelectorStateController : IDisposable {
		BreadCrumbEditHandler handler;
		public BreadCrumbSelectorStateController(BreadCrumbEditHandler handler) {
			this.handler = handler;
		}
		public virtual void OnMouseEnter(EventArgs e) {
		}
		public virtual void OnMouseLeave(EventArgs e) {
			ResetHotNode();
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			UpdateHotNode(e);
		}
		BreadCrumbEditHitInfo mouseDownInfo = null;
		public virtual void OnMouseDown(MouseEventArgs e) {
			this.mouseDownInfo = Handler.Owner.CalcHitInfo(e.Location);
			UpdateHotNode(e);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			BreadCrumbEditHitInfo hitInfo = Handler.Owner.CalcHitInfo(e.Location);
			if(this.mouseDownInfo != null && hitInfo.IsMatch(this.mouseDownInfo)) {
				if(e.Button == MouseButtons.Left && hitInfo.InNodeButton)
					OnNodeLButtonClick(e, hitInfo.Node);
				else if(e.Button == MouseButtons.Left && hitInfo.InClientEmptySpace)
					OnClientEmptySpaceLButtonClick(e);
			}
			if(this.mouseDownInfo != null) this.mouseDownInfo.Clear();
			UpdateHotNode(e);
		}
		protected virtual void UpdateHotNode(MouseEventArgs e) {
			BreadCrumbEditHitInfo hitInfo = Handler.Owner.CalcHitInfo(e.Location);
			if(!hitInfo.InNode) {
				ResetHotNode();
				return;
			}
			Handler.Owner.ViewInfo.SetHotNode(hitInfo);
		}
		protected virtual void ResetHotNode() {
			Handler.Owner.ViewInfo.ResetHotNode();
		}
		protected virtual void OnNodeLButtonClick(MouseEventArgs e, BreadCrumbNode node) {
			Handler.Owner.Properties.RaiseNodeClick(new BreadCrumbNodeClickEventArgs(node));
			Handler.Owner.Properties.SelectedNode = node;
		}
		protected virtual void OnClientEmptySpaceLButtonClick(MouseEventArgs e) {
			Handler.Owner.Properties.RaiseSelectorClientEmptySpaceClick(e);
			Handler.Owner.Properties.EnsureEditMode();
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
			}
			this.handler = null;
		}
		public BreadCrumbEditHandler Handler { get { return handler; } }
	}
	public class BreadCrumbSelectorDropDownController : IDisposable {
		BreadCrumbEditHandler handler;
		BreadCrumbPopupMenuCreatorBase userActionMenuCreator;
		public BreadCrumbSelectorDropDownController(BreadCrumbEditHandler handler) {
			this.handler = handler;
			this.userActionMenuCreator = CreateUserActionMenuCreator();
		}
		protected BreadCrumbPopupMenuCreatorBase UserActionMenuCreator { get { return userActionMenuCreator; } }
		protected virtual BreadCrumbPopupMenuCreatorBase CreateUserActionMenuCreator() {
			return new BreadCrumbUserActionPopupMenuCreator(this);
		}
		BreadCrumbEditHitInfo mouseDownInfo = null;
		public virtual void OnMouseDown(MouseEventArgs e) {
			BreadCrumbEditHitInfo hotNode = Handler.Owner.CalcHitInfo(e.Location);
			if(e.Button == MouseButtons.Left && hotNode.InDropDownButton) {
				Handler.Owner.Properties.RaiseNodeDropDownButtonClick(new BreadCrumbNodeClickEventArgs(hotNode.Node));
				if(!Handler.Owner.IsNodeExpanded) {
					EnsureNodePopup(hotNode, e.Location, false);
				}
				else {
					CloseNodePopup();
				}
			}
			this.mouseDownInfo = hotNode.Clone();
		}
		protected internal virtual void CloseNodePopup() {
			Handler.Owner.HideNodePopup();
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			BreadCrumbEditHitInfo hitInfo = Handler.Owner.CalcHitInfo(e.Location);
			if(this.mouseDownInfo != null && hitInfo.IsMatch(this.mouseDownInfo)) {
				if(e.Button == MouseButtons.Right && hitInfo.InClientArea) {
					ShowUserActionMenu(e.Location);
				}
			}
			if(this.mouseDownInfo != null) this.mouseDownInfo.Clear();
		}
		protected virtual void ShowUserActionMenu(Point pt) {
			DXPopupMenu popupMenu = UserActionMenuCreator.Create();
			BreadCrumbShowUserActionMenuEventArgs arg = new BreadCrumbShowUserActionMenuEventArgs(popupMenu);
			Handler.Owner.Properties.RaiseShowUserActionMenu(arg);
			if(!AllowDropDown || arg.Cancel || arg.PopupMenu.Items.Count == 0) return;
			MenuManagerHelper.ShowMenu(popupMenu, Handler.Owner.LookAndFeel, Handler.Owner.MenuManager, Handler.Owner, pt);
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			BreadCrumbEditHitInfo hitInfo = Handler.Owner.CalcHitInfo(e.Location);
			if(ShouldAutoOpenPopup(hitInfo)) {
				EnsureNodePopup(hitInfo, e.Location, true);
			}
		}
		protected virtual bool ShouldAutoOpenPopup(BreadCrumbEditHitInfo hitInfo) {
			if(!hitInfo.InNode) return false;
			BreadCrumbEditHitInfo hi = Handler.Owner.ViewInfo.ExpandedNode;
			return hi.NodeInfo != null && !object.ReferenceEquals(hitInfo.Node, hi.Node);
		}
		protected virtual void EnsureNodePopup(BreadCrumbEditHitInfo hotNode, Point pt, bool hidePopupIfOpen) {
			BreadCrumbNode node = hotNode.Node;
			node.EnsureRaiseNodePopulate();
			if(node.ChildNodes.IsEmpty) return;
			if(AllowDropDown) {
				CloseNodePopup();
				Handler.Owner.ShowNodePopup(node, CalcPopupLocation(hotNode.NodeInfo));
			}
			Handler.Owner.ViewInfo.SetExpandedNode(hotNode);
		}
		protected virtual bool AllowDropDown { get { return true; } }
		protected virtual Point CalcPopupLocation(BreadCrumbNodeInfo nodeInfo) {
			return new Point(CalcPopupHorzLoc(nodeInfo), nodeInfo.Bounds.Bottom);
		}
		protected virtual int CalcPopupHorzLoc(BreadCrumbNodeInfo nodeInfo) {
			if(RightToLeft) {
				return Math.Min(nodeInfo.Bounds.Right, nodeInfo.Bounds.Left + DropDownHorzIndent);
			}
			return Math.Max(nodeInfo.Bounds.X, nodeInfo.Bounds.Right - DropDownHorzIndent);
		}
		protected bool RightToLeft {
			get { return Handler.Owner.ViewInfo.RightToLeft; }
		}
		protected virtual int DropDownHorzIndent { get { return 45; } } 
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(UserActionMenuCreator != null) UserActionMenuCreator.Dispose();
			}
			this.handler = null;
			this.userActionMenuCreator = null;
		}
		public BreadCrumbEditHandler Handler { get { return handler; } }
	}
	public enum BreadCrumbMode { Edit, Select }
	public class BreadCrumbItemCollection : ComboBoxItemCollection {
		public BreadCrumbItemCollection(RepositoryItemBreadCrumbEdit item)
			: base(item) {
		}
		protected override object ExtractItem(object item) {
			return item;
		}
	}
	public class BreadCrumbHistoryItem : ICloneable {
		string path;
		BreadCrumbHistory owner;
		public BreadCrumbHistoryItem() : this(string.Empty) { }
		public BreadCrumbHistoryItem(string path) {
			this.owner = null;
			this.path = path;
		}
		public void MoveOnTop() {
			BreadCrumbHistory ownerCore = Owner;
			if(ownerCore == null || ownerCore.Count <= 1) return;
			ownerCore.Remove(this);
			ownerCore.Insert(0, this);
		}
		public int GetIndex() {
			return Owner != null ? Owner.IndexOf(this) : -1;
		}
		protected internal void SetOwner(BreadCrumbHistory owner) { this.owner = owner; }
		protected internal void ResetOwner() { SetOwner(null); }
		public override bool Equals(object obj) {
			BreadCrumbHistoryItem item = obj as BreadCrumbHistoryItem;
			if(item == null)
				return false;
			return RepositoryItemBreadCrumbEdit.IsStringEquals(Owner != null ? Owner.Properties : null, Path, item.Path);
		}
		public override int GetHashCode() {
			return Path.GetHashCode();
		}
		public override string ToString() {
			return Path;
		}
		[Category("Data")]
		public string Path { get { return path; } set { path = value; } }
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		#endregion
		public virtual BreadCrumbHistoryItem Clone() {
			BreadCrumbHistoryItem item = new BreadCrumbHistoryItem();
			item.Assign(this);
			return item;
		}
		protected virtual void Assign(BreadCrumbHistoryItem item) {
			this.path = item.Path;
		}
		protected BreadCrumbHistory Owner { get { return owner; } }
	}
	[Editor("DevExpress.XtraEditors.Design.BreadCrumbHistoryEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class BreadCrumbHistory : CollectionBase, IDisposable {
		RepositoryItemBreadCrumbEdit properties;
		public BreadCrumbHistory(RepositoryItemBreadCrumbEdit properties) {
			this.properties = properties;
		}
		public virtual void Add(BreadCrumbHistoryItem item) {
			Add(item, true);
		}
		protected internal virtual void Add(BreadCrumbHistoryItem item, bool checkExisting) {
			if(checkExisting && Contains(item)) return;
			List.Add(item);
		}
		public virtual void AddRange(IEnumerable items) {
			foreach(BreadCrumbHistoryItem item in items) Add(item);
		}
		public virtual void AddRange(BreadCrumbHistoryItem[] items) {
			AddRange((IEnumerable)items);
		}
		public virtual void Remove(BreadCrumbHistoryItem item) {
			if(List.Contains(item)) List.Remove(item);
		}
		public virtual void Insert(int position, BreadCrumbHistoryItem item) {
			if(List.Contains(item)) return;
			List.Insert(position, item);
		}
		public virtual bool Contains(BreadCrumbHistoryItem item) {
			return List.Contains(item);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		public virtual int IndexOf(BreadCrumbHistoryItem item) {
			return List.IndexOf(item);
		}
		public bool IsEmpty {
			get { return List.Count == 0; }
		}
		public void MovePathOnTop(string path) {
			BreadCrumbHistoryItem item = FindItem(path);
			if(item != null) item.MoveOnTop();
		}
		public BreadCrumbHistoryItem FindItem(string path) {
			foreach(BreadCrumbHistoryItem item in this) {
				if(RepositoryItemBreadCrumbEdit.IsStringEquals(Properties, item.Path, path)) return item;
			}
			return null;
		}
		protected override void OnInsert(int position, object value) {
			if(!(value is BreadCrumbHistoryItem)) return;
			base.OnInsert(position, value);
		}
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			BreadCrumbHistoryItem item = (BreadCrumbHistoryItem)value;
			item.SetOwner(this);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			BreadCrumbHistoryItem item = (BreadCrumbHistoryItem)value;
			item.ResetOwner();
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public virtual BreadCrumbHistoryItem this[int index] { get { return List[index] as BreadCrumbHistoryItem; } }
		#region Collection Changed
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		public event CollectionChangeEventHandler CollectionChanged;
		#endregion
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) { }
			this.properties = null;
		}
		public RepositoryItemBreadCrumbEdit Properties { get { return properties; } }
	}
	public class BreadCrumbNavigationController : IDisposable {
		List<BreadCrumbHistoryItem> list;
		int currentPos;
		RepositoryItemBreadCrumbEdit properties;
		public BreadCrumbNavigationController(RepositoryItemBreadCrumbEdit properties) {
			this.properties = properties;
			this.currentPos = -1;
			this.list = new List<BreadCrumbHistoryItem>();
		}
		public virtual void PathChanged(string path) {
			if(IsLocked) return;
			if(CurrentPos != -1 && CurrentPos < List.Count - 1) {
				List.RemoveRange(CurrentPos + 1, List.Count - CurrentPos - 1);
			}
			List.Add(new BreadCrumbHistoryItem(path));
			this.currentPos = CurrentPos == -1 ? 0 : List.Count - 1;
		}
		public virtual void GoBack() {
			if(!CanGoBack) return;
			LockPosChange();
			try {
				this.currentPos--;
				Properties.Path = List[CurrentPos].Path;
			}
			finally {
				UnlockPosChange();
			}
		}
		public virtual void GoForward() {
			if(!CanGoForward) return;
			LockPosChange();
			try {
				this.currentPos++;
				Properties.Path = List[CurrentPos].Path;
			}
			finally {
				UnlockPosChange();
			}
		}
		public virtual void SetNavigationHistoryCurrentItemIndex(int itemIndex) {
			if(itemIndex < 0 || itemIndex > List.Count - 1 || CurrentPos == itemIndex) return;
			LockPosChange();
			try {
				this.currentPos = itemIndex;
				Properties.Path = List[CurrentPos].Path;
			}
			finally {
				UnlockPosChange();
			}
		}
		public virtual void ResetNavigationHistory() {
			List.Clear();
			this.currentPos = 0;
		}
		public bool CanGoBack { get { return CurrentPos > 0; } }
		public bool CanGoForward { get { return CurrentPos < List.Count - 1; } }
		public int GetNavigationHistoryCurrentItemIndex() { return CurrentPos; }
		public virtual BreadCrumbHistory GetNavigationHistory() {
			BreadCrumbHistory res = new BreadCrumbHistory(Properties);
			foreach(BreadCrumbHistoryItem item in List) {
				res.Add(item.Clone(), false);
			}
			return res;
		}
		int lockPosChange = 0;
		protected void LockPosChange() {
			lockPosChange++;
		}
		protected void UnlockPosChange() {
			lockPosChange--;
		}
		protected bool IsLocked { get { return lockPosChange > 0; } }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) { }
			this.properties = null;
		}
		protected int CurrentPos { get { return currentPos; } }
		protected List<BreadCrumbHistoryItem> List {
			get { return list; }
		}
		public RepositoryItemBreadCrumbEdit Properties { get { return properties; } }
	}
	public enum BreadCrumbValidationResult { Match, PartialMatch, NotMatch, Overflow }
	public class BreadCrumbValidationInfo {
		string[] values;
		BreadCrumbNode targetNode;
		BreadCrumbValidationResult resultType;
		RepositoryItemBreadCrumbEdit properties;
		public BreadCrumbValidationInfo(RepositoryItemBreadCrumbEdit properties, BreadCrumbValidationResult resultType)
			: this(properties, resultType, null, null) {
		}
		public BreadCrumbValidationInfo(RepositoryItemBreadCrumbEdit properties, BreadCrumbValidationResult resultType, BreadCrumbNode targetNode)
			: this(properties, resultType, targetNode, null) {
		}
		public BreadCrumbValidationInfo(RepositoryItemBreadCrumbEdit properties, BreadCrumbValidationResult resultType, string[] values)
			: this(properties, resultType, null, values) {
		}
		public BreadCrumbValidationInfo(RepositoryItemBreadCrumbEdit properties, BreadCrumbValidationResult resultType, BreadCrumbNode targetNode, string[] values) {
			this.resultType = resultType;
			this.targetNode = targetNode;
			this.values = values;
			this.properties = properties;
		}
		public BreadCrumbNode TargetNode { get { return targetNode; } }
		public string[] Values { get { return values; } }
		public BreadCrumbValidationResult Result { get { return resultType; } }
		public bool IsMatch {
			get { return Result == BreadCrumbValidationResult.Match; }
		}
		public bool IsPartialMatch {
			get { return Result == BreadCrumbValidationResult.PartialMatch; }
		}
		public bool IsNotMatch {
			get { return Result == BreadCrumbValidationResult.NotMatch; }
		}
		public bool IsOverflow {
			get { return Result == BreadCrumbValidationResult.Overflow; }
		}
		public bool FatalMismatch {
			get { return IsNotMatch && TargetNode == null && Values == null; }
		}
		public bool HasNode { get { return TargetNode != null; } }
		public bool HasPersistentNode {
			get { return TargetNode != null && TargetNode.Persistent; }
		}
		public string MakeValuesPath(string pathSeparator) {
			if(Values == null || Values.Length == 0) return null;
			return BreadCrumbNode.MakePath(Values, pathSeparator);
		}
		protected internal void SetTargetNode(BreadCrumbNode node) { this.targetNode = node; }
		protected internal void SetValues(string[] values) { this.values = values; }
		public override bool Equals(object obj) {
			BreadCrumbValidationInfo sample = obj as BreadCrumbValidationInfo;
			if(sample == null)
				return false;
			if(!MatchArray(Values, sample.Values))
				return false;
			return Result == sample.Result && object.ReferenceEquals(TargetNode, sample.TargetNode);
		}
		protected bool MatchArray(string[] l, string[] r) {
			if(l == null || r == null)
				return l == r;
			if(l.Length != r.Length) return false;
			for(int i = 0; i < l.Length; i++) {
				if(!RepositoryItemBreadCrumbEdit.IsStringEquals(properties, l[i], r[i])) return false;
			}
			return true;
		}
		internal bool IsPersistentChild {
			get { return TargetNode != null && TargetNode.IsPersistentChild; }
		}
		internal bool CheckValues(params string[] values) {
			return MatchArray(Values, values);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return string.Format("Result: {0}, TargetNode: {1}", Result.ToString(), TargetNode != null ? TargetNode.ToString() : "null");
		}
	}
	public class BreadCrumbValidationHelper : IDisposable {
		BreadCrumbValidationOptions options;
		public BreadCrumbValidationHelper(BreadCrumbValidationOptions options) {
			this.options = options;
		}
		public BreadCrumbValidationInfo IsMatch(string path) {
			return IsMatch(BreadCrumbNode.Split(path, Options.PathSeparator));
		}
		protected IList<BreadCrumbNode> GetNodes(string[] values, bool checkPersistent) {
			BreadCrumbNode selNode = Options.SelectedNode;
			if(selNode != null && !selNode.Persistent) return selNode.GetParentNodes(true, checkPersistent);
			return Options.Nodes.GetNodesByValues(values, checkPersistent);
		}
		protected internal virtual BreadCrumbValidationInfo IsMatch(string[] values) {
			BreadCrumbValidationInfo vInfo = IsMatchCore(values, false);
			if(!Options.Nodes.HasPersistentNodes) return vInfo;
			if(vInfo.FatalMismatch) return vInfo;
			BreadCrumbValidationInfo persVInfo = IsMatchCore(values, true);
			return CheckResults(vInfo, persVInfo);
		}
		protected virtual BreadCrumbValidationInfo CheckResults(BreadCrumbValidationInfo vInfo, BreadCrumbValidationInfo persVInfo) {
			if(persVInfo.IsNotMatch && !vInfo.IsNotMatch) return vInfo;
			if(persVInfo.IsNotMatch && vInfo.IsNotMatch) {
				if(persVInfo.HasPersistentNode && !vInfo.HasPersistentNode) return vInfo;
			}
			if(persVInfo.IsPersistentChild && (persVInfo.IsMatch || persVInfo.IsOverflow)) return vInfo;
			return persVInfo;
		}
		protected virtual BreadCrumbValidationInfo IsMatchCore(string[] values, bool checkPersistent) {
			if(values == null) return IsMatchEmptyValuesCore();
			IList<BreadCrumbNode> nodes = GetNodes(values, checkPersistent);
			int i = 0, j = 0;
			for(; i < values.Length && j < nodes.Count; i++, j++) {
				BreadCrumbNode node = nodes[j];
				if(!node.IsMatch(values[i])) return ProcessNodeMismatch(node, values, i);
			}
			if(i == values.Length && j == nodes.Count) {
				return new BreadCrumbValidationInfo(Options.Properties, BreadCrumbValidationResult.Match, j >= 1 ? nodes[j - 1] : null);
			}
			if(i == values.Length) {
				BreadCrumbNode targetNode = j > 0 ? nodes[j - 1] : null;
				return new BreadCrumbValidationInfo(Options.Properties, BreadCrumbValidationResult.PartialMatch, targetNode);
			}
			return new BreadCrumbValidationInfo(Options.Properties, BreadCrumbValidationResult.Overflow, j >= 1 ? nodes[j - 1] : null, CreateValues(values, i));
		}
		protected virtual BreadCrumbValidationInfo ProcessNodeMismatch(BreadCrumbNode node, string[] values, int valuePos) {
			if(node.Persistent && node.Index > 0) {
				return new BreadCrumbValidationInfo(Options.Properties, BreadCrumbValidationResult.NotMatch);
			}
			return new BreadCrumbValidationInfo(Options.Properties, BreadCrumbValidationResult.NotMatch, node, CreateValues(values, valuePos));
		}
		protected virtual BreadCrumbValidationInfo IsMatchEmptyValuesCore() {
			if(Options.Nodes.Count == 0 || Options.Nodes.AllNodesPersistent()) {
				return new BreadCrumbValidationInfo(Options.Properties, BreadCrumbValidationResult.Match);
			}
			return new BreadCrumbValidationInfo(Options.Properties, BreadCrumbValidationResult.PartialMatch);
		}
		protected string[] CreateValues(string[] srcValues, int itemPos) {
			string[] values = new string[Math.Max(0, srcValues.Length - itemPos)];
			Array.Copy(srcValues, itemPos, values, 0, values.Length);
			return values;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
			}
		}
		public BreadCrumbValidationOptions Options { get { return options; } }
	}
	public interface IBreadCrumbValidationClient {
		void EnsureSelectNode(BreadCrumbNode breadCrumbNode);
		bool RaiseValidatePath(string path);
		void RaiseNewNodeAdding(string value, BreadCrumbNode newNode);
	}
	public class BreadCrumbValidationOptions {
		BreadCrumbNode selectedNode;
		string pathSeparator;
		BreadCrumbNodeCollection nodes;
		RepositoryItemBreadCrumbEdit properties;
		private BreadCrumbValidationOptions(RepositoryItemBreadCrumbEdit properties) {
			this.selectedNode = properties.GetSelectedNode();
			this.nodes = properties.Nodes;
			this.pathSeparator = properties.PathSeparator;
			this.properties = properties;
		}
		public BreadCrumbNode SelectedNode { get { return selectedNode; } }
		public string PathSeparator { get { return pathSeparator; } }
		public BreadCrumbNodeCollection Nodes { get { return nodes; } }
		public RepositoryItemBreadCrumbEdit Properties { get { return properties; } }
		public static BreadCrumbValidationOptions GetOptions(RepositoryItemBreadCrumbEdit properties) {
			return new BreadCrumbValidationOptions(properties);
		}
	}
	public class BreadCrumbValidationController : IDisposable {
		IBreadCrumbValidationClient client;
		BreadCrumbValidationOptions options;
		public BreadCrumbValidationController(IBreadCrumbValidationClient client) {
			this.client = client;
		}
		public virtual bool Validate(BreadCrumbValidationOptions options, string path) {
			this.options = options;
			if(string.IsNullOrEmpty(path)) {
				if(!EnsureRaiseValidatePath(path)) return false;
			}
			BreadCrumbValidationInfo vInfo = CreateValidationHelper(options).IsMatch(path);
			if(vInfo.IsMatch || vInfo.IsPartialMatch) {
				ProcessMatch(vInfo);
				return true;
			}
			if(vInfo.IsOverflow) {
				return ProcessOverflow(vInfo, path);
			}
			if(vInfo.FatalMismatch) return false;
			return ProcessMismatch(vInfo, path);
		}
		protected virtual BreadCrumbValidationHelper CreateValidationHelper(BreadCrumbValidationOptions options) {
			return new BreadCrumbValidationHelper(options);
		}
		protected internal virtual void ProcessMatch(BreadCrumbValidationInfo vInfo) {
			Client.EnsureSelectNode(vInfo.TargetNode);
		}
		protected internal bool ProcessOverflow(BreadCrumbValidationInfo vInfo, string path) {
			if(TryResolveFromCache(vInfo.TargetNode, vInfo, path)) return true;
			if(!EnsureRaiseValidatePath(path)) return false;
			return AddNewNodesCore(vInfo.TargetNode, vInfo);
		}
		protected internal virtual bool ProcessMismatch(BreadCrumbValidationInfo vInfo, string path) {
			BreadCrumbNode target = vInfo.HasNode ? vInfo.TargetNode.Parent : null;
			if(TryResolveFromCache(target, vInfo, path)) return true;
			if(!EnsureRaiseValidatePath(path)) return false;
			return AddNewNodesCore(target, vInfo);
		}
		protected virtual bool AddNewNodesCore(BreadCrumbNode parent, BreadCrumbValidationInfo vInfo) {
			BreadCrumbNode current = parent;
			foreach(string value in vInfo.Values) {
				BreadCrumbNodeCollection col = (current == null || current.Persistent) ? this.options.Nodes : current.ChildNodes;
				BreadCrumbNode newNode = col.FindNode(value, false);
				if(newNode == null) {
					newNode = new BreadCrumbNode(value, value);
					col.Add(newNode);
					Client.RaiseNewNodeAdding(value, newNode);
				}
				current = newNode;
			}
			Client.EnsureSelectNode(current);
			return true;
		}
		protected virtual bool TryResolveFromCache(BreadCrumbNode node, BreadCrumbValidationInfo vInfo, string fullPath) {
			IList<BreadCrumbNode> nodes = (node != null) ? node.GetParentNodes(true, false) : this.options.Nodes.GetNodes(false);
			string subPath = vInfo.MakeValuesPath(this.options.PathSeparator);
			foreach(BreadCrumbNode current in nodes) {
				BreadCrumbNode child = current.GetChild(subPath, fullPath);
				if(child != null) {
					Client.EnsureSelectNode(child);
					return true;
				}
			}
			return false;
		}
		protected virtual bool EnsureRaiseValidatePath(string path) {
			if(!AllowRaiseValidatePath) return true;
			return Client.RaiseValidatePath(path);
		}
		protected virtual bool AllowRaiseValidatePath {
			get {
				if(this.options.Nodes.AllNodesPersistent()) return false;
				return this.options.Nodes.Count > 0;
			}
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
			}
		}
		public IBreadCrumbValidationClient Client { get { return client; } }
	}
	public class BreadCrumbPathChangedEventArgs : EventArgs {
		string path;
		public BreadCrumbPathChangedEventArgs(string path) {
			this.path = path;
		}
		public string Path { get { return path; } }
	}
	public delegate void BreadCrumbPathChangedEventHandler(object sender, BreadCrumbPathChangedEventArgs e);
	public class BreadCrumbNodeClickEventArgs : EventArgs {
		BreadCrumbNode node;
		public BreadCrumbNodeClickEventArgs(BreadCrumbNode node) {
			this.node = node;
		}
		public BreadCrumbNode Node { get { return node; } }
	}
	public delegate void BreadCrumbNodeClickEventHandler(object sender, BreadCrumbNodeClickEventArgs e);
	public enum BreadCrumbNodeChangeType { NodeChanged, Reset, NodeAdded, NodeDeleted }
	public class BreadCrumbNodeChangedEventArgs : EventArgs {
		BreadCrumbNode node;
		BreadCrumbNode oldNode;
		BreadCrumbNodeChangeType changeType;
		public BreadCrumbNodeChangedEventArgs(BreadCrumbNodeChangeType changeType)
			: this(changeType, null) {
		}
		public BreadCrumbNodeChangedEventArgs(BreadCrumbNodeChangeType changeType, BreadCrumbNode node)
			: this(changeType, node, null) {
		}
		public BreadCrumbNodeChangedEventArgs(BreadCrumbNodeChangeType changeType, BreadCrumbNode node, BreadCrumbNode oldNode) {
			this.node = node;
			this.oldNode = oldNode;
			this.changeType = changeType;
		}
		public BreadCrumbNodeChangeType ChangeType { get { return changeType; } }
		public BreadCrumbNode Node { get { return node; } }
		public BreadCrumbNode OldNode { get { return oldNode; } }
	}
	public delegate void BreadCrumbNodeChangedEventHandler(object sender, BreadCrumbNodeChangedEventArgs e);
	public class BreadCrumbSelectedNodeChangedEventArgs : EventArgs {
		BreadCrumbNode oldNode, newNode;
		public BreadCrumbSelectedNodeChangedEventArgs(BreadCrumbNode oldNode, BreadCrumbNode newNode) {
			this.oldNode = oldNode;
			this.newNode = newNode;
		}
		public BreadCrumbNode OldNode { get { return oldNode; } }
		public BreadCrumbNode NewNode { get { return newNode; } }
	}
	public delegate void BreadCrumbSelectedNodeChangedEventHandler(object sender, BreadCrumbSelectedNodeChangedEventArgs e);
	public class BreadCrumbQueryChildNodesEventArgs : EventArgs {
		BreadCrumbNode node;
		RepositoryItemBreadCrumbEdit properties;
		public BreadCrumbQueryChildNodesEventArgs(BreadCrumbNode node, RepositoryItemBreadCrumbEdit properties) {
			this.node = node;
			this.properties = properties;
		}
		public BreadCrumbNode Node { get { return node; } }
		public RepositoryItemBreadCrumbEdit Properties { get { return properties; } }
	}
	public delegate void BreadCrumbQueryChildNodesEventHandler(object sender, BreadCrumbQueryChildNodesEventArgs e);
	public enum BreadCrumbValidatePathResult { CreateNodes, Cancel }
	public class BreadCrumbValidatePathEventArgs : EventArgs {
		string path;
		RepositoryItemBreadCrumbEdit properties;
		BreadCrumbValidatePathResult validationResult;
		public BreadCrumbValidatePathEventArgs(string path, RepositoryItemBreadCrumbEdit properties) {
			this.path = path;
			this.properties = properties;
			this.validationResult = BreadCrumbValidatePathResult.Cancel;
		}
		public string Path { get { return path; } }
		public RepositoryItemBreadCrumbEdit Properties { get { return properties; } }
		public BreadCrumbValidatePathResult ValidationResult { get { return validationResult; } set { validationResult = value; } }
	}
	public delegate void BreadCrumbValidatePathEventHandler(object sender, BreadCrumbValidatePathEventArgs e);
	public class BreadCrumbPathRejectedEventArgs : EventArgs {
		string path;
		public BreadCrumbPathRejectedEventArgs(string path) {
			this.path = path;
		}
		public string Path { get { return path; } }
	}
	public delegate void BreadCrumbPathRejectedEventHandler(object sender, BreadCrumbPathRejectedEventArgs e);
	public class BreadCrumbNewNodeAddingEventArgs : EventArgs {
		string value;
		BreadCrumbNode node;
		public BreadCrumbNewNodeAddingEventArgs(string value, BreadCrumbNode node) {
			this.node = node;
			this.value = value;
		}
		public string Value { get { return value; } }
		public BreadCrumbNode Node { get { return node; } }
	}
	public delegate void BreadCrumbNewNodeAddingEventHandler(object sender, BreadCrumbNewNodeAddingEventArgs e);
	public class BreadCrumbShowUserActionMenuEventArgs : EventArgs {
		bool cancel;
		DXPopupMenu popupMenu;
		public BreadCrumbShowUserActionMenuEventArgs(DXPopupMenu popupMenu) {
			this.popupMenu = popupMenu;
			this.cancel = false;
		}
		public DXPopupMenu PopupMenu { get { return popupMenu; } }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public delegate void BreadCrumbShowUserActionMenuEventHandler(object sender, BreadCrumbShowUserActionMenuEventArgs e);
	public class BreadCrumbShownNodeDropDownEventArgs : EventArgs {
		BreadCrumbNode node;
		public BreadCrumbShownNodeDropDownEventArgs(BreadCrumbNode node) {
			this.node = node;
		}
		public BreadCrumbNode Node { get { return node; } }
	}
	public delegate void BreadCrumbShownNodeDropDownEventHandler(object sender, BreadCrumbShownNodeDropDownEventArgs e);
	public class BreadCrumbHiddenNodeDropDownEventArgs : EventArgs {
		BreadCrumbNode node;
		public BreadCrumbHiddenNodeDropDownEventArgs(BreadCrumbNode node) {
			this.node = node;
		}
		public BreadCrumbNode Node { get { return node; } }
	}
	public delegate void BreadCrumbHiddenNodeDropDownEventHandler(object sender, BreadCrumbHiddenNodeDropDownEventArgs e);
	public class BreadCrumbNode : ICloneable {
		object userData, tag;
		bool populateOnDemand, showCaption;
		string caption;
		int position, imageIndex;
		bool persistent;
		bool inOverflow;
		BreadCrumbNode parent;
		RepositoryItemBreadCrumbEdit properties;
		public BreadCrumbNode()
			: this(string.Empty, null) {
		}
		public BreadCrumbNode(string caption)
			: this(caption, null) {
		}
		public BreadCrumbNode(string caption, object value)
			: this(caption, value, false) {
		}
		public BreadCrumbNode(string caption, object value, bool populateOnDemand) {
			this.caption = caption;
			this.userData = value;
			this.populateOnDemand = populateOnDemand;
			this.showCaption = true;
			this.parent = null;
			this.position = UnassignedPos;
			this.persistent = false;
			this.imageIndex = DefaultImageIndex;
			this.tag = null;
			this.inOverflow = false;
			this.properties = null;
		}
		protected internal virtual void SetParent(BreadCrumbNode parent) {
			this.parent = parent;
		}
		protected internal virtual void ResetParent() {
			SetParent(null);
		}
		static readonly int UnassignedPos = -1;
		protected internal virtual void SetPos(int pos) {
			this.position = pos;
		}
		protected internal virtual void ResetPos() { SetPos(UnassignedPos); }
		internal bool InOverflow {
			get { return inOverflow; }
			set { inOverflow = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetProperties(RepositoryItemBreadCrumbEdit properties) {
			this.properties = properties;
		}
		protected internal virtual void ResetProperties() {
			if(this.properties == null || this.properties.InBars) return;
			SetProperties(null);
		}
		protected internal virtual bool IsOverflowNode { get { return false; } }
		[Browsable(false)]
		public int Depth {
			get { return GetDepth(); }
		}
		[Browsable(false)]
		public int Index {
			get { return position; }
		}
		[Browsable(false)]
		public BreadCrumbNode Parent {
			get { return parent; }
		}
		[Browsable(false)]
		public RepositoryItemBreadCrumbEdit Properties {
			get {
				if(this.properties == null) {
					this.properties = FindPropertiesCore();
				}
				return this.properties;
			}
		}
		[Browsable(false)]
		public string Path {
			get { return GetNodePath(); }
		}
		protected internal bool IsPathEquals(string path) {
			return RepositoryItemBreadCrumbEdit.IsStringEquals(Properties, Path, path);
		}
		[Browsable(false)]
		public bool IsTopNode {
			get { return Parent == null; }
		}
		[DXCategory(CategoryName.Data), DefaultValue(null), Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public object Value {
			get { return userData; }
			set {
				if(Value == value)
					return;
				userData = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Data), DefaultValue(null), Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set {
				if(Tag == value)
					return;
				tag = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(false)]
		public bool PopulateOnDemand {
			get { return populateOnDemand; }
			set {
				if(PopulateOnDemand == value)
					return;
				populateOnDemand = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(true)]
		public bool ShowCaption {
			get { return showCaption; }
			set {
				if(ShowCaption == value)
					return;
				showCaption = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Appearance)]
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value)
					return;
				caption = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(false)]
		public bool Persistent {
			get { return persistent; }
			set {
				if(Persistent == value)
					return;
				persistent = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(DefaultImageIndex), Editor("DevExpress.XtraEditors.Design.BreadCrumbNodeImageIndexesEditor," + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor)), TypeConverter("DevExpress.XtraEditors.Design.BreadCrumbNodeImageIndexesTypeConverter," + AssemblyInfo.SRAssemblyEditorsDesign)]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				OnChanged();
			}
		}
		BreadCrumbNodeCollection childNodes = null;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BreadCrumbNodeCollection ChildNodes {
			get {
				if(childNodes == null) {
					childNodes = CreateChildNodesCollection();
				}
				return childNodes;
			}
		}
		protected virtual BreadCrumbNodeCollection CreateChildNodesCollection() {
			return new BreadCrumbNodeCollection(this);
		}
		[Browsable(false)]
		public BreadCrumbNode PrevNode {
			get {
				if(Index == UnassignedPos)
					return null;
				BreadCrumbNodeCollection nodes = GetParentNodeCollection();
				return nodes.InRange(Index - 1) ? nodes[Index - 1] : null;
			}
		}
		[Browsable(false)]
		public BreadCrumbNode NextNode {
			get {
				if(Index == UnassignedPos)
					return null;
				BreadCrumbNodeCollection nodes = GetParentNodeCollection();
				return nodes.InRange(Index + 1) ? nodes[Index + 1] : null;
			}
		}
		protected BreadCrumbNodeCollection GetParentNodeCollection() {
			if(IsTopNode) return Properties.Nodes;
			return Parent.ChildNodes;
		}
		internal ReadOnlyCollection<BreadCrumbNode> GetParentNodes(bool includeSelf, bool includePersistent) {
			List<BreadCrumbNode> list = new List<BreadCrumbNode>();
			BreadCrumbNode current = (includeSelf && !Persistent) ? this : Parent;
			while(current != null) {
				if(!current.Persistent) list.Add(current);
				current = current.Parent;
			}
			if(list.Count > 1) list.Reverse();
			if(includePersistent && Properties != null) {
				list.InsertRange(0, Properties.Nodes.GetPersistentNodes());
			}
			return new ReadOnlyCollection<BreadCrumbNode>(list);
		}
		public Image GetImage() {
			if(HasDefaultImageIndex) return GetOwnerDefaultImage();
			if(HasNoneImageIndex) return null;
			return ImageCollection.GetImageListImage(Properties.Images, ImageIndex);
		}
		public BreadCrumbNode GetChild(string path, string fullPath) {
			string[] values = BreadCrumbNode.Split(path, Properties.PathSeparator);
			if(values == null || values.Length == 0) return null;
			BreadCrumbNode current = GetChildCore(values);
			if(current == null) return null;
			if(string.IsNullOrEmpty(fullPath)) return current;
			List<BreadCrumbNode> col = new List<BreadCrumbNode>();
			while(current != null) {
				col.Add(current);
				current = current.GetChildCore(values);
			}
			BreadCrumbNode candidate = col.Count == 1 ? col.First() : col.Find(n => n.IsPathEquals(fullPath));
			return candidate != null && candidate.IsPathEquals(fullPath) ? candidate : null;
		}
		protected BreadCrumbNode GetChildCore(string[] values) {
			BreadCrumbNode current = this;
			for(int i = 0; i < values.Length; i++) {
				current = current.FindChild(values[i]);
				if(current == null) return null;
			}
			return current;
		}
		protected virtual BreadCrumbNode FindChild(string value) {
			foreach(BreadCrumbNode childNode in ChildNodes) {
				if(RepositoryItemBreadCrumbEdit.IsStringEquals(Properties, childNode.GetValue(), value)) return childNode;
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const int DefaultImageIndex = -2;
		public static bool IsDefaultImageIndex(int imageIndex) {
			return imageIndex == DefaultImageIndex;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public const int NoneImageIndex = -1;
		public static bool IsNoneImageIndex(int imageIndex) {
			return imageIndex == NoneImageIndex;
		}
		internal static string[] Split(string path, string pathSeparator) {
			if(path == null) return null;
			return path.Split(new string[] { pathSeparator }, StringSplitOptions.RemoveEmptyEntries);
		}
		internal static string MakePath(string[] values, string pathSeparator) {
			StringBuilder builder = new StringBuilder();
			for(int i = 0; i < values.Length; i++) {
				builder.Append(values[i]);
				if(i < values.Length - 1) builder.Append(pathSeparator);
			}
			return builder.ToString();
		}
		public static string NoneItemText { get { return "(None)"; } }
		public static string DefaultItemText { get { return "(Default)"; } }
		#region Internal
		protected internal virtual void OnChanged() {
			if(Properties != null)
				Properties.RaiseNodeChanged(new BreadCrumbNodeChangedEventArgs(BreadCrumbNodeChangeType.NodeChanged, this));
		}
		protected virtual RepositoryItemBreadCrumbEdit FindPropertiesCore() {
			BreadCrumbNode parent = this;
			while(parent.Parent != null) {
				parent = parent.Parent;
			}
			return parent.properties;
		}
		protected virtual string GetNodePath() {
			RepositoryItemBreadCrumbEdit properties = Properties;
			if(properties == null) {
				throw new InvalidOperationException("BreadCrumbNode has no parent");
			}
			StringBuilder builder = new StringBuilder();
			GetNodePathCore(builder, properties.PathSeparator);
			return builder.ToString();
		}
		protected internal virtual string NormalizedPath {
			get { return (string.IsNullOrEmpty(Path) || Path.EndsWith(Properties.PathSeparator)) ? Path : Path + Properties.PathSeparator; }
		}
		protected internal bool HasSeparator {
			get {
				string value = GetValue();
				if(string.IsNullOrEmpty(value))
					return false;
				return value.EndsWith(Properties.PathSeparator);
			}
		}
		public bool IsParentOf(BreadCrumbNode node) {
			if(node == null) return false;
			BreadCrumbNode parent = node.Parent;
			while(parent != null) {
				if(object.ReferenceEquals(parent, this))
					return true;
				parent = parent.Parent;
			}
			return false;
		}
		internal static readonly int EmptyDepth = -1;
		protected internal int GetDepth() {
			if(Properties == null) return EmptyDepth;
			int depth = 0;
			BreadCrumbNode current = Parent;
			while(current != null) {
				current = current.Parent;
				++depth;
			}
			return depth;
		}
		protected internal BreadCrumbNode GetTopNode() {
			if(IsTopNode) return this;
			return GetTopNodeCore();
		}
		protected internal virtual bool CanMove() {
			if(Persistent) return false;
			if(IsTopNode) return true;
			BreadCrumbNode topNode = GetTopNode();
			if(topNode == null) return true;
			return topNode.CanMove();
		}
		protected BreadCrumbNode GetTopNodeCore() {
			BreadCrumbNode current = this;
			while(current.Parent != null) {
				current = current.Parent;
			}
			return current;
		}
		internal bool IsPersistentChild {
			get {
				BreadCrumbNode current = Parent;
				while(current != null) {
					if(current.Persistent) return true;
					current = current.Parent;
				}
				return false;
			}
		}
		protected virtual void GetNodePathCore(StringBuilder builder, string pathSeparator) {
			ReadOnlyCollection<BreadCrumbNode> parentNodes = GetParentNodes(true, false);
			foreach(BreadCrumbNode node in parentNodes) {
				builder.Append(node.GetValue());
				if(!object.ReferenceEquals(node, this) && !node.HasSeparator) builder.Append(pathSeparator);
			}
			if(parentNodes.Count == 1 && !parentNodes[0].HasSeparator) builder.Append(pathSeparator);
		}
		public virtual string GetValue() {
			string value = Value != null ? Value.ToString() : string.Empty;
			if(string.IsNullOrEmpty(value)) {
				value = Caption;
			}
			return value;
		}
		protected internal Image GetOwnerDefaultImage() {
			if(Properties == null || Properties.Images == null) return null;
			return ImageCollection.GetImageListImage(Properties.Images, Properties.ImageIndex);
		}
		protected internal bool HasNoneImageIndex { get { return BreadCrumbNode.IsNoneImageIndex(ImageIndex); } }
		protected internal bool HasDefaultImageIndex { get { return BreadCrumbNode.IsDefaultImageIndex(ImageIndex); } }
		#endregion
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		#endregion
		public virtual BreadCrumbNode Clone() {
			BreadCrumbNode node = new BreadCrumbNode();
			node.Assign(this);
			return node;
		}
		public BreadCrumbNode Clone(bool recursive) {
			BreadCrumbNode clone = Clone();
			CloneRecursiveCore(this, clone);
			return clone;
		}
		protected internal void CloneRecursiveCore(BreadCrumbNode source, BreadCrumbNode res) {
			if(source.ChildNodes.Count == 0) return;
			foreach(BreadCrumbNode sourceChild in source.ChildNodes) {
				BreadCrumbNode clonedSourceChild = sourceChild.Clone();
				res.ChildNodes.Add(clonedSourceChild);
				CloneRecursiveCore(sourceChild, clonedSourceChild);
			}
		}
		protected virtual void Assign(BreadCrumbNode node) {
			this.userData = node.Value;
			this.caption = node.Caption;
			this.populateOnDemand = node.PopulateOnDemand;
			this.showCaption = node.ShowCaption;
			this.imageIndex = node.ImageIndex;
			this.persistent = node.Persistent;
			this.tag = node.Tag;
		}
		bool populatedOnDemand = false;
		protected internal void EnsureRaiseNodePopulate() {
			if(!PopulateOnDemand || this.populatedOnDemand)
				return;
			if(Properties != null) {
				ChildNodes.BeginUpdate();
				try {
					Properties.RaiseQueryChildNodes(new BreadCrumbQueryChildNodesEventArgs(this, Properties));
					ChildNodes.SortIfRequired();
				}
				finally {
					ChildNodes.EndUpdate(false);
				}
			}
			this.populatedOnDemand = true;
		}
		protected internal virtual bool IsMatch(string val) {
			return RepositoryItemBreadCrumbEdit.IsStringEquals(Properties, GetValue(), val);
		}
		public override bool Equals(object obj) {
			BreadCrumbNode sample = obj as BreadCrumbNode;
			if(sample == null) return false;
			if(Value != null) {
				if(!Value.Equals(sample.Value)) return false;
			}
			else {
				if(sample.Value != null) return false;
			}
			if(Tag != null) {
				if(!Tag.Equals(sample.Tag)) return false;
			}
			else {
				if(sample.Tag != null) return false;
			}
			if(Persistent != sample.Persistent) return false;
			if(ImageIndex != sample.ImageIndex) return false;
			if(PopulateOnDemand != sample.PopulateOnDemand) return false;
			return ShowCaption == sample.ShowCaption && RepositoryItemBreadCrumbEdit.IsStringEquals(Properties, Caption, sample.Caption);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption)) {
				return Caption;
			}
			return base.ToString();
		}
		internal bool IsParentPersistent {
			get { return Parent != null && Parent.Persistent; }
		}
	}
	public class BreadCrumbOverflowNode : BreadCrumbNode {
		public BreadCrumbOverflowNode() {
		}
		protected override BreadCrumbNodeCollection CreateChildNodesCollection() {
			return new BreadCrumbOverflowNodeCollection();
		}
		protected internal override bool IsOverflowNode { get { return true; } }
	}
	public class BreadCrumbOverflowNodeCollection : BreadCrumbNodeCollection {
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			BreadCrumbNode node = (BreadCrumbNode)value;
			node.InOverflow = true;
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			BreadCrumbNode node = (BreadCrumbNode)value;
			node.InOverflow = false;
		}
		protected override bool AllowSetParent { get { return false; } }
	}
	[
	ListBindable(false), Editor("DevExpress.XtraEditors.Design.BredCrumbNodeCollectionEditor," + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)),
	]
	public class BreadCrumbNodeCollection : CollectionBase, IEnumerable<BreadCrumbNode> {
		BreadCrumbNode owner;
		RepositoryItemBreadCrumbEdit properties;
		public BreadCrumbNodeCollection() : this((BreadCrumbNode)null) { }
		public BreadCrumbNodeCollection(BreadCrumbNode owner) {
			this.owner = owner;
		}
		public BreadCrumbNodeCollection(RepositoryItemBreadCrumbEdit properties) {
			this.properties = properties;
		}
		int lockUpdate = 0;
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			EndUpdate(true);
		}
		public virtual void EndUpdate(bool raiseChanged) {
			if(--lockUpdate == 0) {
				if(raiseChanged) OnChanged(BreadCrumbNodeChangeType.Reset, CollectionChangeAction.Refresh, null, null);
			}
		}
		public event CollectionChangeEventHandler CollectionChanged;
		public virtual BreadCrumbNode this[int index] {
			get { return List[index] as BreadCrumbNode; }
		}
		public virtual void Add(BreadCrumbNode node) {
			if(!this.supressChecking && !CanAddNode(node)) return;
			List.Add(node);
		}
		protected virtual bool CanAddNode(BreadCrumbNode node) {
			RepositoryItemBreadCrumbEdit prop = GetProperties();
			if(prop == null || prop.IsDesignMode) return true;
			if(prop.InQueryChildren) {
				if(IsExists(node.GetValue(), false)) return false;
			}
			return true;
		}
		public virtual void AddRange(IEnumerable nodes) {
			BeginUpdate();
			try {
				foreach(BreadCrumbNode node in nodes) Add(node);
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void AddRange(BreadCrumbNode[] nodes) {
			AddRange((IEnumerable)nodes);
		}
		internal void Reverse() {
			InnerList.Reverse();
		}
		bool supressChecking = false;
		internal void FastCopy(BreadCrumbNodeCollection source) {
			this.supressChecking = true;
			try {
				AddRange(source);
			}
			finally {
				this.supressChecking = false;
			}
		}
		public virtual void Remove(BreadCrumbNode node) {
			if(Contains(node)) List.Remove(node);
		}
		public virtual void Insert(int position, BreadCrumbNode node) {
			if(Contains(node)) return;
			List.Insert(position, node);
		}
		protected internal bool IsExists(string value, bool recursive) {
			if(string.IsNullOrEmpty(value)) return false;
			return FindNode(value, recursive) != null;
		}
		public virtual BreadCrumbNode FindNode(string value, bool recursive) {
			IEnumerable nodes = List;
			if(recursive) nodes = GetAllNodes();
			foreach(BreadCrumbNode node in nodes) {
				if(RepositoryItemBreadCrumbEdit.IsStringEquals(GetProperties(), node.GetValue(), value)) return node;
			}
			return null;
		}
		protected override void OnInsert(int position, object value) {
			if(!(value is BreadCrumbNode)) return;
			base.OnInsert(position, value);
		}
		protected virtual bool AllowSetParent { get { return true; } }
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			if(AllowSetParent) {
				BreadCrumbNode node = value as BreadCrumbNode;
				node.SetParent(owner);
				node.SetPos(position);
				if(this.properties != null) node.SetProperties(properties);
			}
			OnChanged(BreadCrumbNodeChangeType.NodeAdded, CollectionChangeAction.Add, value as BreadCrumbNode, null);
		}
		protected override void OnClear() {
			try {
				for(int n = Count - 1; n >= 0; n--) {
					RemoveAt(n);
				}
			}
			finally {
			}
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			if(this.supressEvents) return;
			if(AllowSetParent) {
				BreadCrumbNode node = value as BreadCrumbNode;
				node.ResetParent();
				node.ResetPos();
				node.ResetProperties();
			}
			OnChanged(BreadCrumbNodeChangeType.NodeDeleted, CollectionChangeAction.Remove, value as BreadCrumbNode, null);
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnChanged(BreadCrumbNodeChangeType.Reset, CollectionChangeAction.Refresh, null, null);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			OnChanged(BreadCrumbNodeChangeType.NodeChanged, CollectionChangeAction.Refresh, newValue as BreadCrumbNode, oldValue as BreadCrumbNode);
		}
		public virtual bool Contains(BreadCrumbNode node) {
			return ContainsNodeCore(node, this);
		}
		public virtual bool Contains(BreadCrumbNode node, bool recursive) {
			return Contains(node, this, recursive);
		}
		protected bool Contains(BreadCrumbNode node, BreadCrumbNodeCollection nodes, bool recursive) {
			if(ContainsNodeCore(node, nodes)) return true;
			if(!recursive) return false;
			foreach(BreadCrumbNode childNode in nodes) {
				if(Contains(node, childNode.ChildNodes, recursive)) return true;
			}
			return false;
		}
		static bool ContainsNodeCore(BreadCrumbNode node, BreadCrumbNodeCollection nodes) {
			for(int i = 0; i < nodes.Count; i++) {
				BreadCrumbNode child = nodes[i];
				if((node == null && child == null) || (child != null && child.Equals(node))) return true;
			}
			return false;
		}
		public virtual int IndexOf(BreadCrumbNode node) {
			return List.IndexOf(node);
		}
		public bool InRange(int index) {
			return index >= 0 && index < Count;
		}
		public void RemoveStartAt(int startIndex) {
			if(!InRange(startIndex)) {
				throw new ArgumentOutOfRangeException("startIndex");
			}
			for(int i = Count - 1; i >= startIndex; i--) {
				if(this[i].Persistent) continue;
				RemoveAt(i);
			}
		}
		public BreadCrumbNode FirstNode {
			get { return Count > 0 ? this[0] : null; }
		}
		public BreadCrumbNode LastNode {
			get { return Count > 0 ? this[Count - 1] : null; }
		}
		protected internal IList<BreadCrumbNode> GetAllNodes() {
			List<BreadCrumbNode> list = new List<BreadCrumbNode>();
			GetNodesCore(this, list);
			return list;
		}
		protected internal void GetNodesCore(BreadCrumbNodeCollection nodes, List<BreadCrumbNode> list) {
			for(int i = 0; i < nodes.Count; i++) {
				BreadCrumbNode node = nodes[i];
				list.Add(node);
				GetNodesCore(node.ChildNodes, list);
			}
		}
		protected internal bool AllNodesPersistent() {
			if(Count == 0) return false;
			foreach(BreadCrumbNode node in this) {
				if(!node.Persistent) return false;
			}
			return true;
		}
		protected internal IEnumerable<BreadCrumbNode> GetPersistentNodes() {
			foreach(BreadCrumbNode node in this) {
				if(node.Persistent) yield return node;
			}
		}
		protected internal IList<BreadCrumbNode> GetNodesByValues(string[] values, bool includePersistent) {
			List<BreadCrumbNode> list = new List<BreadCrumbNode>();
			BreadCrumbNodeCollection col = this;
			for(int i = 0; i < values.Length; i++) {
				BreadCrumbNode node = col.FindNode(values[i], false);
				if(node == null || (node.Persistent && !includePersistent)) break;
				list.Add(node);
				col = node.ChildNodes;
			}
			return list;
		}
		internal ReadOnlyCollection<BreadCrumbNode> GetNodes(bool includePersistent = true) {
			List<BreadCrumbNode> list = new List<BreadCrumbNode>();
			foreach(BreadCrumbNode node in List) {
				if(node.Persistent && !includePersistent) continue;
				list.Add(node);
			}
			return new ReadOnlyCollection<BreadCrumbNode>(list);
		}
		protected internal bool HasPersistentNodes {
			get { return GetPersistentNodes().Count() > 0; }
		}
		public static int NoneIndex = int.MinValue;
		protected virtual void OnChanged(BreadCrumbNodeChangeType changeType, CollectionChangeAction action, BreadCrumbNode node, BreadCrumbNode oldNode) {
			if(lockUpdate != 0) return;
			RepositoryItemBreadCrumbEdit properties = GetProperties();
			if(properties != null) {
				SortIfRequired();
				properties.RaiseNodeChanged(new BreadCrumbNodeChangedEventArgs(changeType, node, oldNode));
			}
			if(CollectionChanged != null) CollectionChanged(this, new CollectionChangeEventArgs(action, node));
		}
		protected internal void SortIfRequired() {
			if(AllowSort()) DoSort();
			for(int i = 0; i < Count; i++) {
				var children = this[i].ChildNodes;
				children.SortIfRequired();
			}
		}
		protected bool AllowSort() {
			if(owner == null) return false;
			RepositoryItemBreadCrumbEdit properties = GetProperties();
			if(properties == null || properties.IsDesignMode) return false;
			return properties.SortNodesByCaption && Count > 1;
		}
		protected void DoSort() {
			InnerList.Sort(new BreadCrumbNodeComparer(GetProperties().CaseSensitiveSearch));
		}
		protected RepositoryItemBreadCrumbEdit GetProperties() {
			RepositoryItemBreadCrumbEdit res = properties;
			if(res == null) {
				if(owner != null) res = owner.Properties;
			}
			return res;
		}
		public bool IsEmpty { get { return Count == 0; } }
		bool supressEvents = false;
		internal void Clear(bool supressEvents) {
			this.supressEvents = true;
			try {
				foreach(BreadCrumbNode n in this) {
					Clear(this);
				}
				Clear();
			}
			finally {
				this.supressEvents = false;
			}
		}
		internal void Clear(BreadCrumbNodeCollection col) {
			for(int i = 0; i < col.Count; i++) {
				BreadCrumbNode child = col[i];
				Clear(child.ChildNodes);
				child.ChildNodes.Clear();
				child.ResetProperties();
				child.ResetParent();
			}
		}
		#region Comparer
		protected class BreadCrumbNodeComparer : IComparer {
			readonly StringComparer comparer;
			public BreadCrumbNodeComparer(bool caseSensitive) {
				this.comparer = caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			}
			int IComparer.Compare(object x, object y) {
				string val1 = ((BreadCrumbNode)x).Caption;
				string val2 = ((BreadCrumbNode)y).Caption;
				return this.comparer.Compare(val1, val2);
			}
		}
		#endregion
		#region IEnumerable<BreadCrumbNode>
		IEnumerator<BreadCrumbNode> IEnumerable<BreadCrumbNode>.GetEnumerator() {
			foreach(BreadCrumbNode node in List) {
				yield return node;
			}
		} 
		#endregion
	}
	public class BreadCrumbAutoCompleteState : IDisposable {
		bool isDirty;
		bool forceEditValue;
		BreadCrumbNode node;
		char keyChar;
		string editText;
		int caretPos;
		BreadCrumbValidationInfo vInfo;
		RepositoryItemBreadCrumbEdit properties;
		public BreadCrumbAutoCompleteState(RepositoryItemBreadCrumbEdit properties) {
			Reset();
			this.properties = properties;
		}
		public void SetNode(BreadCrumbNode node) {
			if(object.ReferenceEquals(Node, node)) return;
			this.node = node;
			OnChanged();
		}
		public void SetVInfo(BreadCrumbValidationInfo vInfo) {
			this.vInfo = vInfo;
		}
		public void SetKeyChar(char keyChar) {
			this.keyChar = keyChar;
		}
		public void SetEditText(string editText) {
			this.editText = editText;
		}
		public void SetForceEditValueFlag(bool forceEditValue) {
			this.forceEditValue = forceEditValue;
		}
		public bool IsKeySeparator(string pathSeparator) {
			if(KeyChar == '\b') return false;
			return EditText.EndsWith(pathSeparator);
		}
		public bool IsExactPath(string pathSeparator) {
			return VInfo.IsPartialMatch && !EditText.EndsWith(pathSeparator);
		}
		public void Reset() {
			this.node = null;
			this.vInfo = null;
			this.keyChar = (char)0;
			this.editText = string.Empty;
			this.isDirty = false;
			this.forceEditValue = false;
			this.caretPos = 0;
		}
		public bool IsReady { get { return Node != null && VInfo != null; } }
		public bool IsDirty { get { return isDirty; } }
		public bool ForceEditValue { get { return forceEditValue; } }
		public void SetDirty() { this.isDirty = true; }
		public void ResetDirty() { this.isDirty = false; }
		public void SetCaretPos(int caretPos) { this.caretPos = caretPos; }
		public int CaretPos { get { return caretPos; } }
		protected virtual void OnChanged() {
			this.isDirty = true;
		}
		public override bool Equals(object obj) {
			BreadCrumbAutoCompleteState sample = obj as BreadCrumbAutoCompleteState;
			if(sample == null) return false;
			if(VInfo != null) {
				if(!VInfo.Equals(sample.VInfo)) return false;
			}
			else {
				if(sample.VInfo != null) return false;
			}
			if(!RepositoryItemBreadCrumbEdit.IsStringEquals(properties, EditText, sample.EditText)) return false;
			return object.ReferenceEquals(Node, sample.Node) && KeyChar == sample.KeyChar;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("Node: {0}", Node != null ? Node.ToString() : "null");
			builder.Append(", ");
			builder.AppendFormat("IsDirty: {0}", IsDirty.ToString());
			builder.Append(", ");
			builder.AppendFormat("VInfo: {0}", VInfo != null ? VInfo.ToString() : "null");
			builder.Append(", ");
			builder.AppendFormat("KeyChar: {0}", KeyChar.ToString());
			builder.Append(", ");
			builder.AppendFormat("EditText: {0}", EditText.ToString());
			builder.Append(", ");
			builder.AppendFormat("ForceEditValue: {0}", ForceEditValue.ToString());
			builder.Append(", ");
			builder.AppendFormat("CaretPos: {0}", CaretPos.ToString());
			return builder.ToString();
		}
		public BreadCrumbNode Node { get { return node; } }
		public char KeyChar { get { return keyChar; } }
		public string EditText { get { return editText; } }
		public BreadCrumbValidationInfo VInfo { get { return vInfo; } }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
			}
			Reset();
		}
	}
	public class BreadCrumbAutoCompleteController : IDisposable {
		BreadCrumbEdit owner;
		BreadCrumbAutoCompleteState state;
		BreadCrumbAutoCompleteStateCalculator stateCalculator;
		public BreadCrumbAutoCompleteController(BreadCrumbEdit owner) {
			this.owner = owner;
			this.state = CreateStateObject();
			this.stateCalculator = CreateStateCalculator();
		}
		protected virtual BreadCrumbAutoCompleteState CreateStateObject() { return new BreadCrumbAutoCompleteState(owner.Properties); }
		protected virtual BreadCrumbAutoCompleteStateCalculator CreateStateCalculator() {
			return new BreadCrumbAutoCompleteStateCalculator(Owner);
		}
		protected BreadCrumbAutoCompleteStateCalculator StateCalculator { get { return stateCalculator; } }
		public virtual void ProcessAutoSearchChar(KeyPressEventArgs e) {
			if(Owner.Properties.ReadOnly || !AcceptChar(e)) return;
			ProcessAutoSearchCharCore(GetEditText(e), e);
		}
		public virtual void ProcessAutoSearchCharCore(string editText, KeyPressEventArgs e) {
			StateCalculator.CalcState(editText, e.KeyChar, ref state);
			if(!State.IsReady) {
				if(!State.VInfo.IsNotMatch || !GetValidationController().ProcessMismatch(State.VInfo, editText)) return;
				State.SetNode(Owner.Properties.GetSelectedNode());
			}
			Owner.AutoSearchText = State.EditText;
			if(State.IsDirty)
				Owner.Properties.InitAutoCompleteItems(State.Node);
			if(!Owner.IsPopupOpen) {
				Owner.ShowPopup();
			}
			else {
				if(Owner.PopupForm != null) Owner.PopupForm.ListBox.SetFilter(State.EditText);
				Owner.RefreshPopup();
			}
			if(OnProcessKeyCore()) {
				UpdateEditValue(editText);
				e.Handled = true;
			}
			State.ResetDirty();
			Owner.LayoutChanged();
		}
		protected BreadCrumbValidationController GetValidationController() {
			return Owner.Properties.ValidationController;
		}
		protected void UpdateEditValue(string editText) {
			Owner.EditValue = editText;
			Owner.MaskBox.MaskBoxSelectionStart = State.CaretPos;
		}
		protected virtual bool OnProcessKeyCore() {
			bool handlingRes = false;
			if(State.KeyChar == BackSpaceChar) {
				OnProcessBackSpaceKey();
				handlingRes = true;
			}
			if(State.IsKeySeparator(Owner.Properties.PathSeparator)) {
				if(OnProcessSeparatorKey()) handlingRes = true;
			}
			if(State.IsExactPath(Owner.Properties.PathSeparator)) {
				if(OnProcessExactPath()) handlingRes = true;
			}
			if(State.ForceEditValue) {
				handlingRes = true;
			}
			return handlingRes;
		}
		protected virtual bool OnProcessSeparatorKey() {
			Owner.ClosePopup(PopupCloseMode.Immediate);
			Owner.SetPathInternal(State.EditText);
			BreadCrumbNode node = Owner.Properties.GetSelectedNode();
			if(node != null) {
				node.EnsureRaiseNodePopulate();
				Owner.Properties.InitAutoCompleteItems(node);
				Owner.ShowPopup();
				ResetCaret();
			}
			return true;
		}
		protected virtual void ResetCaret() {
			Owner.ResetCaret();
			State.SetCaretPos(Owner.Text.Length);
		}
		protected virtual bool OnProcessExactPath() {
			if(!Owner.IsPopupOpen) return false;
			Owner.ClosePopup(PopupCloseMode.Normal);
			Owner.SetPathInternal(State.EditText);
			ResetCaret();
			return true;
		}
		protected virtual void OnProcessBackSpaceKey() {
			BreadCrumbPopupFormBase popupForm = (BreadCrumbPopupFormBase)Owner.PopupForm;
			if(popupForm == null || popupForm.ListBox.SelectedIndex == -1) return;
			Owner.EditValue = State.EditText;
			Owner.PopupForm.ListBox.SelectedIndex = -1;
		}
		protected virtual bool AcceptChar(KeyPressEventArgs e) {
			if(e.KeyChar == BackSpaceChar) return true;
			if(char.IsControl(e.KeyChar)) {
				return false;
			}
			return true;
		}
		static readonly char BackSpaceChar = '\b';
		protected internal string GetEditText(KeyPressEventArgs e) {
			char charCode = e.KeyChar;
			int caretPos = MaskBox.MaskBoxSelectionStart;
			if(Owner.Properties.CharacterCasing != CharacterCasing.Normal) {
				charCode = Owner.Properties.CharacterCasing == CharacterCasing.Lower ? Char.ToLower(e.KeyChar) : Char.ToUpper(e.KeyChar);
			}
			string text = ClearSelection(Owner.Text);
			if(charCode == BackSpaceChar) {
				if(caretPos == 0) return text;
				if(MaskBox.MaskBoxSelectionLength > 0) return text;
				if(caretPos > 0 && caretPos < MaskBox.TextLength) {
					return text.Remove(MaskBox.MaskBoxSelectionStart - 1, 1);
				}
				return text.Substring(0, Math.Max(0, text.Length - 1));
			}
			return text.Insert(MaskBox.MaskBoxSelectionStart, charCode.ToString());
		}
		protected TextBoxMaskBox MaskBox { get { return Owner.MaskBox; } }
		protected string ClearSelection(string editText) {
			int selStart = MaskBox.MaskBoxSelectionStart, selLength = MaskBox.MaskBoxSelectionLength;
			if(selLength == 0) return editText;
			return Owner.Text.Remove(selStart, selLength);
		}
		public virtual bool CanProcessAutoSearchText { get { return true; } }
		public virtual bool AllowAutoSearchSelectionLength { get { return false; } }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(State != null) State.Reset();
				if(StateCalculator != null) StateCalculator.Dispose();
			}
			this.owner = null;
			this.state = null;
			this.stateCalculator = null;
		}
		public BreadCrumbEdit Owner { get { return owner; } }
		public BreadCrumbAutoCompleteState State { get { return state; } }
	}
	public class BreadCrumbAutoCompleteStateCalculator : IDisposable {
		BreadCrumbEdit owner;
		public BreadCrumbAutoCompleteStateCalculator(BreadCrumbEdit owner) {
			this.owner = owner;
		}
		public virtual void CalcState(string editText, char keyChar, ref BreadCrumbAutoCompleteState state) {
			BreadCrumbValidationInfo vInfo = GetValidationHelper().IsMatch(editText);
			BreadCrumbNode currentNode = GetCurrentNode(editText, vInfo);
			state.SetNode(currentNode);
			state.SetVInfo(vInfo);
			state.SetKeyChar(keyChar);
			state.SetEditText(editText);
			state.SetForceEditValueFlag(CalcForceEditValue());
			state.SetCaretPos(GetCaretPos(keyChar));
			if(state.IsReady && SelectedNode != null) {
				if(currentNode.IsParentOf(SelectedNode.Parent)) state.SetDirty();
			}
		}
		protected int GetCaretPos(char keyChar) {
			int newPos = Owner.MaskBox.MaskBoxSelectionStart;
			if(keyChar == '\b' && newPos > 0 && Owner.MaskBox.MaskBoxSelectionLength == 0) {
				newPos--;
			}
			return newPos;
		}
		protected virtual BreadCrumbNode GetCurrentNode(string editText, BreadCrumbValidationInfo vInfo) {
			BreadCrumbNodeCollection nodes = Owner.Properties.Nodes;
			if(vInfo.IsNotMatch) return vInfo.TargetNode.Parent;
			if(vInfo.IsPartialMatch) return vInfo.TargetNode;
			return Owner.Properties.GetSelectedNode();
		}
		protected virtual bool CalcForceEditValue() {
			TextBoxMaskBox maskBox = Owner.MaskBox;
			if(maskBox.MaskBoxSelectionLength > 0) return true;
			if(maskBox.MaskBoxSelectionStart < maskBox.MaskBoxText.Length) return true;
			return false;
		}
		protected BreadCrumbNode SelectedNode { get { return Owner.Properties.GetSelectedNode(); } }
		protected BreadCrumbValidationHelper GetValidationHelper() {
			return new BreadCrumbValidationHelper(BreadCrumbValidationOptions.GetOptions(Owner.Properties));
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			this.owner = null;
			if(disposing) { }
		}
		public BreadCrumbEdit Owner { get { return owner; } }
	}
	public abstract class BreadCrumbPopupMenuCreatorBase : IDisposable {
		BreadCrumbSelectorDropDownController controller;
		public BreadCrumbPopupMenuCreatorBase(BreadCrumbSelectorDropDownController controller) {
			this.controller = controller;
		}
		public DXPopupMenu Create() { return Create(null); }
		public abstract DXPopupMenu Create(object arg);
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
			}
			this.controller = null;
		}
		public BreadCrumbSelectorDropDownController Controller { get { return controller; } }
	}
	public class BreadCrumbUserActionPopupMenuCreator : BreadCrumbPopupMenuCreatorBase {
		BreadCrumbUserActionPopupMenuHandler handler;
		public BreadCrumbUserActionPopupMenuCreator(BreadCrumbSelectorDropDownController controller)
			: base(controller) {
			this.handler = CreateHandler();
		}
		protected virtual BreadCrumbUserActionPopupMenuHandler CreateHandler() {
			return new BreadCrumbUserActionPopupMenuHandler(this);
		}
		public override DXPopupMenu Create(object arg) {
			return CreateCore();
		}
		protected virtual DXPopupMenu CreateCore() {
			DXPopupMenu popupMenu = new DXPopupMenu();
			Handler.Initialize(popupMenu);
			return popupMenu;
		}
		public BreadCrumbUserActionPopupMenuHandler Handler { get { return handler; } }
	}
	public class BreadCrumbUserActionPopupMenuHandler {
		BreadCrumbPopupMenuCreatorBase menuCreator;
		public BreadCrumbUserActionPopupMenuHandler(BreadCrumbPopupMenuCreatorBase menuCreator) {
			this.menuCreator = menuCreator;
		}
		public void Initialize(DXPopupMenu popupMenu) {
			popupMenu.Items.Add(CreateCopyAddressItem());
			popupMenu.Items.Add(CreateEditAddressItem());
			popupMenu.Items.Add(CreateDeleteHistoryItem());
		}
		protected virtual DXMenuItem CreateCopyAddressItem() {
			DXMenuItem item = new DXMenuItem();
			item.Caption = Resources.CopyAddressCaption;
			item.Click += OnCopyAddressHandler;
			return item;
		}
		protected virtual DXMenuItem CreateEditAddressItem() {
			DXMenuItem item = new DXMenuItem();
			item.Caption = Resources.EditAddressCaption;
			item.Click += EditAddressHandler;
			return item;
		}
		protected virtual DXMenuItem CreateDeleteHistoryItem() {
			DXMenuItem item = new DXMenuItem();
			item.Caption = Resources.DeleteHistoryCaption;
			item.Click += DeleteHistoryHandler;
			return item;
		}
		protected virtual void OnCopyAddressHandler(object sender, EventArgs e) {
			BreadCrumbEdit owner = GetEdit();
			if(owner != null)
				Clipboard.SetText(owner.Properties.Path);
		}
		protected virtual void EditAddressHandler(object sender, EventArgs e) {
			BreadCrumbEdit owner = GetEdit();
			if(owner != null)
				owner.Properties.EnsureEditMode();
		}
		protected virtual void DeleteHistoryHandler(object sender, EventArgs e) {
			BreadCrumbEdit owner = GetEdit();
			if(owner != null)
				owner.Properties.History.Clear();
		}
		protected BreadCrumbEdit GetEdit() {
			return MenuCreator.Controller.Handler.Owner;
		}
		protected BreadCrumbPopupMenuCreatorBase MenuCreator { get { return menuCreator; } }
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class BreadCrumbEditViewInfo : ComboBoxViewInfo, IBreadCrumbValidationClient {
		Hashtable nodeInfo;
		Rectangle glyphAreaBounds;
		Rectangle glyphEmptySpaceBounds;
		BaseBreadCrumbNodePainter nodePainter;
		BaseBreadCrumbNodePainter overflowNodePainter;
		BreadCrumbNodeObjectInfoArgs breadCrumbNodeObjectInfoArgs;
		BreadCrumbEditHitInfo hotNode;
		BreadCrumbEditHitInfo expandedNode;
		BreadCrumbNode selectedNode;
		BreadCrumbOverflowNode overflowNode;
		public BreadCrumbEditViewInfo(RepositoryItem item)
			: base(item) {
			this.nodeInfo = new Hashtable();
			this.glyphAreaBounds = Rectangle.Empty;
			this.glyphEmptySpaceBounds = Rectangle.Empty;
			this.breadCrumbNodeObjectInfoArgs = CreateBreadCrumbNodeObjectInfoArgs();
			this.hotNode = new BreadCrumbEditHitInfo();
			this.expandedNode = new BreadCrumbEditHitInfo();
			this.overflowNode = CreateOverflowNode();
		}
		protected virtual BreadCrumbOverflowNode CreateOverflowNode() {
			return new BreadCrumbOverflowNode();
		}
		protected virtual BreadCrumbNodeObjectInfoArgs CreateBreadCrumbNodeObjectInfoArgs() {
			return new BreadCrumbNodeObjectInfoArgs(this);
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			Rectangle rect = ContentRect;
			this.glyphAreaBounds = CalcGlyphAreaBounds(ContentRect);
			if(!RightToLeft) {
				this.fContentRect.X = Bounds.X;
			}
			else {
				this.fContentRect.X = rect.X;
			}
			if(AllowRootGlyph) {
				if(!RightToLeft) this.fContentRect.X = this.glyphAreaBounds.Right + 1;
			}
			this.fContentRect.Y = Bounds.Y;
			if(!RightToLeft) {
				this.fContentRect.Width = (rect.Right - this.fContentRect.X);
			}
			else {
				this.fContentRect.Width = (GlyphBounds.Left - this.fContentRect.X - 1);
			}
			this.fContentRect.Height = bounds.Height + 3;
			this.glyphEmptySpaceBounds = CalcGlyphEmptySpaceBounds();
			CalcOverflowNode();
			CalcNodes();
			if(RightToLeft) CheckRTL(ContentRect);
		}
		public override TextOptions DefaultTextOptions {
			get {
				TextOptions options = base.DefaultTextOptions;
				options.Trimming = Trimming.EllipsisCharacter;
				return options;
			} 
		}
		protected internal BreadCrumbOverflowNode OverflowNode { get { return overflowNode; } }
		protected virtual void CheckRTL(Rectangle rect) {
			if(NodeInfo.Count == 0) return;
			foreach(BreadCrumbNodeInfo nodeInfo in VisibleItems) {
				nodeInfo.CheckRTL(rect);
			}
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			this.nodePainter = CreateNodePainter();
			this.overflowNodePainter = CreateOverflowNodePainter();
		}
		protected override string GetDisplayText() {
			if(IsShowNullValuePrompt()) return Item.NullValuePrompt;
			return string.Empty;
		}
		public override void Reset() {
			base.Reset();
			if(this.nodeInfo != null) {
				this.nodeInfo.Clear();
			}
			this.glyphAreaBounds = Rectangle.Empty;
			if(this.hotNode != null) {
				this.hotNode.Clear();
			}
			if(this.expandedNode != null) {
				this.expandedNode.Clear();
			}
			this.selectedNode = null;
		}
		protected internal Hashtable NodeInfo { get { return nodeInfo; } }
		public ICollection VisibleItems {
			get { return NodeInfo.Values; }
		}
		protected internal BreadCrumbNodeObjectInfoArgs BreadCrumbNodeObjectInfoArgs {
			get { return breadCrumbNodeObjectInfoArgs; }
		}
		public BaseBreadCrumbNodePainter NodePainter { get { return nodePainter; } }
		public BaseBreadCrumbNodePainter OverflowNodePainter { get { return overflowNodePainter; } }
		public virtual BaseBreadCrumbNodePainter GetNodePainter(BreadCrumbNodeInfo nodeInfo) {
			if(nodeInfo.Node.IsOverflowNode) return OverflowNodePainter;
			return NodePainter;
		}
		protected virtual BaseBreadCrumbNodePainter CreateNodePainter() {
			return new SkinBreadCrumbNodePainter();
		}
		protected virtual BaseBreadCrumbNodePainter CreateOverflowNodePainter() {
			return new SkinBreadCrumbOverflowNodePainter();
		}
		public override bool AllowMaskBox {
			get {
				if(Item.BreadCrumbMode == BreadCrumbMode.Select) return false;
				return base.AllowMaskBox;
			}
		}
		protected override bool IsButtonPressed(EditorButtonObjectInfoArgs info) {
			if(OwnerEdit.IsNodeExpanded) return false;
			return base.IsButtonPressed(info);
		}
		protected virtual bool AllowInflateHeight { get { return true; } }
		protected override Size CalcContentSize(Graphics g) {
			Size size = base.CalcContentSize(g);
			if(AllowInflateHeight) {
				size.Height += GetInflateValue();
			}
			return size;
		}
		protected virtual int GetInflateValue() { return 2; }
		public override TextGlyphDrawModeEnum GlyphDrawMode {
			get {
				if(AllowRootGlyph)
					return TextGlyphDrawModeEnum.TextGlyph;
				return TextGlyphDrawModeEnum.Text;
			}
		}
		public override bool IsExistImage { get { return AllowRootGlyph; } }
		public override Size ImageSize {
			get { return AllowRootGlyph ? RootGlyph.Size : Size.Empty; }
		}
		public override HorzAlignment GlyphAlignment { get { return HorzAlignment.Near; } }
		#region Calc Nodes
		protected virtual void CalcOverflowNode() {
			BreadCrumbNodeCollection children = OverflowNode.ChildNodes;
			children.Clear();
			if(!HasSelectedNode) return;
			int requiredWidth = CalcBestContentWidth();
			int avaliableWidth = CalcAvaliableContentWidth();
			if(requiredWidth <= avaliableWidth) return;
			children.BeginUpdate();
			List<BreadCrumbNode> persistentList = new List<BreadCrumbNode>();
			try {
				ReadOnlyCollection<BreadCrumbNode> nodes = GetAllNodes();
				for(int i = 0; i < nodes.Count - 2; i++) {
					BreadCrumbNode node = nodes[i];
					if(requiredWidth < avaliableWidth) break;
					requiredWidth -= CalcNodeWidth(node);
					if(node.Persistent)
						persistentList.Add(node);
					else
						children.Add(node);
				}
				if(children.Count > 0) children.Reverse();
				for(int i = persistentList.Count - 1; i >= 0; i--) {
					persistentList[i].EnsureRaiseNodePopulate();
					children.AddRange(persistentList[i].ChildNodes);
				}
			}
			finally {
				children.EndUpdate();
			}
		}
		protected virtual void CalcNodes() {
			int leftPt = ContentRect.Left;
			Rectangle ownerBounds = ContentRect;
			NodeInfo.Clear();
			if(!HasSelectedNode) return;
			int extraWidth = CalcExtraWidth();
			foreach(BreadCrumbNode breadCrumbNode in GetVisibleNodes()) {
				BreadCrumbNodeInfo nodeInfo = CalcNode(breadCrumbNode, ownerBounds, ref extraWidth, ref leftPt);
				UpdateNode(nodeInfo);
				NodeInfo.Add(breadCrumbNode, nodeInfo);
			}
		}
		protected int CalcExtraWidth() {
			if(!IsOverflowNodeRequired) return 0;
			return Math.Max(0, CalcVisibleContentWidth() - CalcAvaliableContentWidth());
		}
		protected bool HasSelectedNode { get { return GetSelectedNode() != null; } }
		protected ReadOnlyCollection<BreadCrumbNode> GetVisibleNodes() {
			if(!HasSelectedNode) return null;
			if(IsOverflowNodeRequired) {
				List<BreadCrumbNode> list = new List<BreadCrumbNode>();
				list.Add(OverflowNode);
				foreach(BreadCrumbNode node in GetAllNodes()) {
					if(IsVisibleInOverflowView(node)) list.Add(node);
				}
				return new ReadOnlyCollection<BreadCrumbNode>(list);
			}
			return GetAllNodes();
		}
		protected bool IsVisibleInOverflowView(BreadCrumbNode node) {
			if(node.InOverflow) return false;
			if(node.Persistent) {
				var child = node.ChildNodes.FirstNode;
				if(child != null && child.InOverflow) return false;
			}
			return true;
		}
		protected ReadOnlyCollection<BreadCrumbNode> GetAllNodes() {
			if(!HasSelectedNode) return null;
			return GetSelectedNode().GetParentNodes(true, true);
		}
		protected internal bool IsOverflowNodeRequired { get { return OverflowNode.ChildNodes.Count > 0; } }
		protected int CalcBestContentWidth() {
			ReadOnlyCollection<BreadCrumbNode> visibleNodes = GetAllNodes();
			if(visibleNodes == null || visibleNodes.Count == 0) return 0;
			int requiredWidth = 0;
			foreach(BreadCrumbNode node in visibleNodes) {
				requiredWidth += CalcNodeWidth(node);
			}
			requiredWidth += GetOverflowButtonWidth();
			return requiredWidth;
		}
		protected int CalcVisibleContentWidth() {
			ReadOnlyCollection<BreadCrumbNode> visibleNodes = GetVisibleNodes();
			if(visibleNodes == null || visibleNodes.Count == 0) return 0;
			int requiredWidth = 0;
			foreach(BreadCrumbNode node in visibleNodes) {
				requiredWidth += CalcNodeWidth(node);
			}
			return requiredWidth;
		}
		protected int GetOverflowButtonWidth() {
			return NodePainter.GetDropDownButtonWidth(BreadCrumbNodeObjectInfoArgs);
		}
		protected virtual int CalcAvaliableContentWidth() {
			return MaskBoxRect.Width;
		}
		protected internal virtual BreadCrumbNode GetSelectedNode() {
			BreadCrumbNode node = this.selectedNode;
			if(node == null && Item.SelectedNode != null) {
				node = Item.SelectedNode;
			}
			return node;
		}
		protected virtual BreadCrumbNodeInfo CalcNode(BreadCrumbNode node, Rectangle ownerBounds, ref int extraWidth, ref int leftPt) {
			Size captionSize = CalcNodeCaptionSize(node, ref extraWidth);
			Rectangle nodeBounds = CalcNodeBounds(node, ownerBounds, captionSize, leftPt);
			Rectangle captionBounds = CalcNodeCaptionBounds(node, nodeBounds, captionSize);
			Rectangle dropDownButtonBounds = CalcDropDownButtonBounds(node, nodeBounds);
			leftPt = nodeBounds.Right;
			return CreateNodeInfo(node, nodeBounds, captionBounds, dropDownButtonBounds);
		}
		protected virtual int CalcNodeWidth(BreadCrumbNode node) {
			int leftPt = ContentRect.Left, extraWidth = 0;
			Rectangle ownerBounds = ContentRect;
			BreadCrumbNodeInfo nodeInfo = CalcNode(node, ownerBounds, ref extraWidth, ref leftPt);
			return nodeInfo.Bounds.Width;
		}
		protected virtual bool IsLeaf(BreadCrumbNode node) {
			BreadCrumbNode selNode = this.selectedNode;
			if(!object.ReferenceEquals(selNode, node)) {
				return false;
			}
			node.EnsureRaiseNodePopulate();
			return node.ChildNodes.IsEmpty;
		}
		protected virtual Size CalcNodeCaptionSize(BreadCrumbNode node, ref int extraWidth) {
			if(!node.ShowCaption || node.IsOverflowNode) return Size.Empty;
			Size res = Size.Empty;
			Graphics g = GInfo.AddGraphics(null);
			try {
				res = PaintAppearance.CalcTextSize(g, node.Caption, Bounds.Width).ToSize();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			if(extraWidth > 0) {
				int allowOptimize = Math.Max(0, res.Width - NodeCaptionMinWidth);
				if(allowOptimize > 0) {
					res.Width -= Math.Min(extraWidth, allowOptimize);
					extraWidth -= Math.Min(extraWidth, allowOptimize);
				}
			}
			return res;
		}
		protected virtual int NodeCaptionMinWidth { get { return 30; } }
		protected virtual Rectangle CalcNodeBounds(BreadCrumbNode node, Rectangle ownerBounds, Size captionSize, int left) {
			Rectangle nodeBounds = new Rectangle(ContentRect.X, ownerBounds.Y - 1, ContentRect.Width, ownerBounds.Height);
			if(ShouldOverlapBorder(node)) {
				nodeBounds.X--;
				nodeBounds.Width++;
			}
			nodeBounds.X = left;
			nodeBounds.Width = IsLeaf(node) ? 0 : NodePainter.GetDropDownButtonWidth(BreadCrumbNodeObjectInfoArgs);
			if(node.ShowCaption && !node.IsOverflowNode) {
				nodeBounds.Width += captionSize.Width + NodePainter.GetHorzTextIndent(BreadCrumbNodeObjectInfoArgs) * 2 + 1;
			}
			return nodeBounds;
		}
		protected virtual bool ShouldOverlapBorder(BreadCrumbNode node) {
			if(AllowRootGlyph) return false;
			if(node.IsTopNode && ContainsLeftAlignedButtons) return false;
			return true;
		}
		protected virtual Rectangle CalcNodeCaptionBounds(BreadCrumbNode node, Rectangle nodeBounds, Size captionSize) {
			if(!node.ShowCaption || node.IsOverflowNode) return Rectangle.Empty;
			Rectangle captionBounds = nodeBounds;
			captionBounds.Inflate(-NodePainter.GetHorzTextIndent(BreadCrumbNodeObjectInfoArgs), 0);
			captionBounds.X++;
			captionBounds.Y = nodeBounds.Y + nodeBounds.Height / 2 - captionSize.Height / 2;
			captionBounds.Width = captionSize.Width + 1;
			captionBounds.Height = captionSize.Height;
			return captionBounds;
		}
		protected virtual Rectangle CalcDropDownButtonBounds(BreadCrumbNode node, Rectangle nodeBounds) {
			if(IsLeaf(node)) return Rectangle.Empty;
			int width = NodePainter.GetDropDownButtonWidth(BreadCrumbNodeObjectInfoArgs);
			return new Rectangle(nodeBounds.Right - width, nodeBounds.Y, width, nodeBounds.Height);
		}
		protected virtual BreadCrumbNodeInfo CreateNodeInfo(BreadCrumbNode node, Rectangle bounds, Rectangle textBounds, Rectangle dropDownButtonBounds) {
			return new BreadCrumbNodeInfo(node, bounds, textBounds, dropDownButtonBounds);
		}
		#endregion
		#region IBreadCrumbValidationClient
		void IBreadCrumbValidationClient.EnsureSelectNode(BreadCrumbNode newNode) {
			DoSetNode(newNode);
		}
		bool IBreadCrumbValidationClient.RaiseValidatePath(string path) {
			return Item.RaiseValidatePath(path);
		}
		void IBreadCrumbValidationClient.RaiseNewNodeAdding(string value, BreadCrumbNode newNode) {
			Item.RaiseNewNodeAdding(new BreadCrumbNewNodeAddingEventArgs(value, newNode));
		}
		#endregion
		protected void DoSetNode(BreadCrumbNode newNode) {
			if(this.selectedNode != newNode) {
				this.selectedNode = newNode;
				UpdateViewInfo();
			}
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			string val = GetPath(EditValue);
			Item.BeginUpdate();
			try {
				if(string.IsNullOrEmpty(val)) {
					DoSetNode(null);
				}
				else {
					Validator.Validate(BreadCrumbValidationOptions.GetOptions(Item), val);
				}
			}
			finally {
				Item.CancelUpdate();
			}
		}
		public static string GetPath(object editValue) {
			if(editValue is BreadCrumbNode) {
				return ((BreadCrumbNode)editValue).Path;
			}
			if(editValue is ComboBoxItem) {
				object val = ((ComboBoxItem)editValue).Value;
				if(val is BreadCrumbHistoryItem) {
					return ((BreadCrumbHistoryItem)val).Path;
				}
			}
			return editValue as string;
		}
		protected void UpdateViewInfo() { CalcViewInfo(null); }
		BreadCrumbValidationController validator = null;
		protected BreadCrumbValidationController Validator {
			get {
				if(validator == null) {
					validator = new BreadCrumbValidationController(this);
				}
				return validator;
			}
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			foreach(BreadCrumbNodeInfo nodeInfo in VisibleItems) {
				nodeInfo.Offset(x, y);
			}
		}
		protected void UpdateNodes(bool doInvalidate) {
			if(NodeInfo.Count == 0) return;
			foreach(BreadCrumbNodeInfo nodeInfo in VisibleItems) {
				UpdateNode(nodeInfo);
			}
			if(doInvalidate) DoInvalidate();
		}
		protected internal virtual void UpdateNode(BreadCrumbNodeInfo nodeInfo) {
			UpdateNodeState(nodeInfo);
			UpdateNodeAppearance(nodeInfo);
		}
		protected virtual void UpdateNodeAppearance(BreadCrumbNodeInfo nodeInfo) {
			nodeInfo.PaintAppearance.Assign(PaintAppearance);
			nodeInfo.PaintAppearance.ForeColor = GetNodeForeColor(nodeInfo);
			nodeInfo.PaintAppearance.BackColor = GetNodeBackColor(nodeInfo);
		}
		protected virtual Color GetNodeForeColor(BreadCrumbNodeInfo nodeInfo) {
			Color color = NodePainter.GetNodeForeColor(BreadCrumbNodeObjectInfoArgs, nodeInfo.State);
			if(!color.IsEmpty) return color;
			return PaintAppearance.ForeColor;
		}
		protected virtual Color GetNodeBackColor(BreadCrumbNodeInfo nodeInfo) {
			return PaintAppearance.BackColor;
		}
		protected virtual void UpdateNodeState(BreadCrumbNodeInfo nodeInfo) {
			if(!Enabled) {
				nodeInfo.State = ObjectState.Disabled;
				return;
			}
			if(Item.IsDesignMode) return;
			if(ExpandedNode.NodeInfo != null && ExpandedNode.NodeInfo.IsMatch(nodeInfo)) {
				nodeInfo.State = ObjectState.Hot | ObjectState.Pressed;
				return;
			}
			ObjectState nodeState = ObjectState.Normal;
			if((HotNode.NodeInfo != null && HotNode.NodeInfo.IsMatch(nodeInfo)) || nodeInfo.Bounds.Contains(MousePosition)) {
				nodeState = ObjectState.Hot;
				if((Control.MouseButtons & MouseButtons.Left) != 0) {
					nodeState |= ObjectState.Pressed;
				}
			}
			else {
				nodeState &= ~ObjectState.Hot;
			}
			nodeInfo.State = nodeState;
		}
		protected override bool UpdateObjectState() {
			bool changed = base.UpdateObjectState();
			foreach(BreadCrumbNodeInfo nodeInfo in VisibleItems) {
				if(UpdateNodeObjectState(nodeInfo)) changed = true;
			}
			return changed;
		}
		protected bool UpdateNodeObjectState(BreadCrumbNodeInfo nodeInfo) {
			ObjectState prev = nodeInfo.State;
			UpdateNodeState(nodeInfo);
			return prev != nodeInfo.State;
		}
		protected internal void SetHotNode(BreadCrumbEditHitInfo hitInfo) {
			BreadCrumbEditHitInfo prevHotNode = HotNode;
			this.hotNode = hitInfo;
			ObjectState prevState = hitInfo.NodeInfo.State;
			if(!HotNode.IsMatch(prevHotNode) || prevState != hitInfo.NodeInfo.State) {
				OnHotNodeChanged(prevHotNode != null ? prevHotNode.NodeInfo : null, HotNode != null ? HotNode.NodeInfo : null);
			}
		}
		protected internal virtual void SetExpandedNode(BreadCrumbEditHitInfo hitInfo) {
			BreadCrumbEditHitInfo prevExpandedNode = ExpandedNode;
			this.expandedNode = hitInfo;
			if(!ExpandedNode.IsMatch(prevExpandedNode)) {
				OnExpandedNodeChanged(prevExpandedNode != null ? prevExpandedNode.NodeInfo : null, ExpandedNode != null ? ExpandedNode.NodeInfo : null);
			}
		}
		protected internal void ResetHotNode() {
			if(HotNode == null) return;
			BreadCrumbNodeInfo prev = HotNode.NodeInfo;
			HotNode.Clear();
			if(prev != null) {
				UpdateNode(prev);
				DoInvalidate();
			}
		}
		protected internal virtual void ResetExpandedNode() {
			if(ExpandedNode == null) return;
			ExpandedNode.Clear();
			UpdateNodes(true);
		}
		protected virtual void OnHotNodeChanged(BreadCrumbNodeInfo prev, BreadCrumbNodeInfo next) {
			UpdateNodes(true);
		}
		protected virtual void OnExpandedNodeChanged(BreadCrumbNodeInfo prev, BreadCrumbNodeInfo next) {
			UpdateNodes(true);
		}
		protected void DoInvalidate() {
			if(OwnerControl != null) OwnerControl.Invalidate();
		}
		public BreadCrumbEditHitInfo HotNode { get { return hotNode; } }
		public BreadCrumbEditHitInfo ExpandedNode { get { return expandedNode; } }
		protected override void UpdateFromOwner() {
			base.UpdateFromOwner();
		}
		new protected internal BreadCrumbEdit OwnerControl { get { return base.OwnerEdit as BreadCrumbEdit; } }
		protected internal bool ContainsLeftAlignedButtons {
			get {
				foreach(EditorButton button in Item.Buttons) {
					if(button.IsLeft) return true;
				}
				return false;
			}
		}
		public Rectangle ContentClipRect {
			get { return Rectangle.Intersect(Bounds, ContentRect); }
		}
		protected virtual Rectangle CalcGlyphAreaBounds(Rectangle bounds) {
			if(!AllowRootGlyph) return Rectangle.Empty;
			Size glyphSize = GlyphBounds.Size;
			if(!RightToLeft) {
				return new Rectangle(new Point(bounds.X + 1, bounds.Y), GlyphBounds.Size);
			}
			return new Rectangle(new Point(bounds.Right - glyphSize.Width - 1, bounds.Y), glyphSize);
		}
		protected virtual Rectangle CalcGlyphEmptySpaceBounds() {
			if(!AllowRootGlyph) return Rectangle.Empty;
			Rectangle bounds = Rectangle.Empty;
			if(!RightToLeft) {
				return new Rectangle(ClientRect.X, ClientRect.Y, Math.Max(0, ContentRect.X - ClientRect.X), ClientRect.Height);
			}
			return new Rectangle(GlyphAreaBounds.Left, ClientRect.Y, Math.Max(0, ClientRect.Right - GlyphAreaBounds.Left), ClientRect.Height);
		}
		public override EditHitInfo CalcHitInfo(Point pt) {
			BreadCrumbEditHitInfo hitInfo = (BreadCrumbEditHitInfo)base.CalcHitInfo(pt);
			if(AllowMaskBox) return hitInfo;
			if(hitInfo.Check(GlyphAreaBounds, EditHitTest.Glyph)) {
				return hitInfo;
			}
			if(!ContentRect.Contains(pt)) {
				return hitInfo.SetHitTest(BreadCrumbSelectorHitTest.None);
			}
			hitInfo.SetHitTest(EditHitTest.None);
			foreach(DictionaryEntry entry in NodeInfo) {
				BreadCrumbNodeInfo nodeInfo = (BreadCrumbNodeInfo)entry.Value;
				if(nodeInfo.Bounds.Contains(pt)) {
					if(nodeInfo.DropDownButtonBounds.Contains(pt)) {
						hitInfo.SetHitTest(BreadCrumbSelectorHitTest.NodeDropDownButton);
					}
					else {
						hitInfo.SetHitTest(BreadCrumbSelectorHitTest.NodeButton);
					}
					hitInfo.SetNode(nodeInfo.Node);
					hitInfo.SetNodeInfo(nodeInfo);
					return hitInfo;
				}
			}
			hitInfo.SetHitTest(BreadCrumbSelectorHitTest.ClientEmptySpace);
			return hitInfo;
		}
		protected override EditHitInfo CreateHitInfo(Point pt) {
			return new BreadCrumbEditHitInfo(pt);
		}
		public virtual bool AllowSelector {
			get { return Item.BreadCrumbMode == BreadCrumbMode.Select; }
		}
		public virtual bool AllowRootGlyph {
			get { return Item.ShowRootGlyph && (Item.RootGlyph != null || ImageCollection.IsImageListImageExists(Item.Images, Item.RootImageIndex)); }
		}
		public Rectangle GlyphEmptySpaceRect { get { return glyphEmptySpaceBounds; } }
		public Rectangle GlyphAreaBounds { get { return glyphAreaBounds; } }
		public Image RootGlyph {
			get {
				Image img = Item.RootGlyph;
				if(img == null)
					img = ImageCollection.GetImageListImage(Item.Images, Item.RootImageIndex);
				return img;
			}
		}
		public override void Dispose() {
			if(this.nodeInfo != null) {
				this.nodeInfo.Clear();
				this.nodeInfo = null;
			}
			if(this.hotNode != null) {
				this.hotNode.Clear();
				this.hotNode = null;
			}
			this.nodePainter = null;
			this.overflowNodePainter = null;
			if(this.overflowNode != null) {
				this.overflowNode.ChildNodes.Clear(true);
				this.overflowNode = null;
			}
			this.breadCrumbNodeObjectInfoArgs = null;
			base.Dispose();
		}
		public new BreadCrumbEdit OwnerEdit { get { return base.OwnerEdit as BreadCrumbEdit; } }
		public new RepositoryItemBreadCrumbEdit Item { get { return base.Item as RepositoryItemBreadCrumbEdit; } }
	}
	public enum BreadCrumbSelectorHitTest { None, NodeButton, NodeDropDownButton, ClientEmptySpace }
	public class BreadCrumbEditHitInfo : EditHitInfo, ICloneable {
		BreadCrumbNode node;
		BreadCrumbNodeInfo nodeInfo;
		BreadCrumbSelectorHitTest selectorHitTest;
		public BreadCrumbEditHitInfo() : this(Point.Empty) { }
		public BreadCrumbEditHitInfo(Point pt)
			: base(pt) {
			this.node = null;
			this.nodeInfo = null;
			this.selectorHitTest = BreadCrumbSelectorHitTest.None;
		}
		public virtual bool Check(Rectangle bounds, EditHitTest hitTest) {
			if(bounds.Contains(HitPoint)) {
				SetHitTest(hitTest);
				return true;
			}
			return false;
		}
		public virtual bool Check(Rectangle bounds, BreadCrumbSelectorHitTest hitTest) {
			if(bounds.Contains(HitPoint)) {
				this.selectorHitTest = hitTest;
				return true;
			}
			return false;
		}
		public BreadCrumbEditHitInfo SetHitTest(BreadCrumbSelectorHitTest hitTest) {
			this.selectorHitTest = hitTest;
			return this;
		}
		public void SetNode(BreadCrumbNode node) {
			this.node = node;
		}
		public void SetNodeInfo(BreadCrumbNodeInfo nodeInfo) {
			this.nodeInfo = nodeInfo;
		}
		public override void Clear() {
			base.Clear();
			this.node = null;
			this.nodeInfo = null;
			this.selectorHitTest = BreadCrumbSelectorHitTest.None;
		}
		public override void Assign(EditHitInfo hitInfo) {
			base.Assign(hitInfo);
			BreadCrumbEditHitInfo hi = hitInfo as BreadCrumbEditHitInfo;
			if(hi != null) {
				this.node = hi.node;
				this.nodeInfo = hi.nodeInfo;
				this.selectorHitTest = hi.selectorHitTest;
			}
		}
		public bool InDropDownButton { get { return SelectorHitTest == BreadCrumbSelectorHitTest.NodeDropDownButton; } }
		public bool InNodeButton { get { return SelectorHitTest == BreadCrumbSelectorHitTest.NodeButton; } }
		public bool InNode { get { return SelectorHitTest == BreadCrumbSelectorHitTest.NodeDropDownButton || SelectorHitTest == BreadCrumbSelectorHitTest.NodeButton; } }
		public bool InClientEmptySpace { get { return SelectorHitTest == BreadCrumbSelectorHitTest.ClientEmptySpace; } }
		public bool InClientArea { get { return SelectorHitTest != BreadCrumbSelectorHitTest.None; } }
		public BreadCrumbNode Node { get { return node; } }
		public BreadCrumbNodeInfo NodeInfo { get { return nodeInfo; } }
		public BreadCrumbSelectorHitTest SelectorHitTest { get { return selectorHitTest; } }
		public bool IsMatch(BreadCrumbEditHitInfo hitInfo) {
			if(hitInfo.HitTest != HitTest || hitInfo.SelectorHitTest != SelectorHitTest) return false;
			if(!object.ReferenceEquals(HitObject, hitInfo.HitObject)) return false;
			if(!object.ReferenceEquals(Node, hitInfo.Node)) return false;
			if(hitInfo.NodeInfo != null && NodeInfo != null) {
				return object.ReferenceEquals(NodeInfo.Node, hitInfo.NodeInfo.Node);
			}
			return NodeInfo == null && hitInfo.NodeInfo == null;
		}
		#region ICloneable
		object ICloneable.Clone() {
			return Clone();
		}
		#endregion
		public virtual BreadCrumbEditHitInfo Clone() {
			BreadCrumbEditHitInfo obj = new BreadCrumbEditHitInfo();
			obj.Assign(this);
			return obj;
		}
	}
	public class BreadCrumbNodeInfo {
		BreadCrumbNode node;
		Rectangle bounds, textBounds, dropDownButtonBounds;
		ObjectState state;
		AppearanceObject paintAppearance;
		public BreadCrumbNodeInfo(BreadCrumbNode node, Rectangle bounds, Rectangle textBounds, Rectangle dropDownButtonBounds) {
			this.node = node;
			this.bounds = bounds;
			this.textBounds = textBounds;
			this.dropDownButtonBounds = dropDownButtonBounds;
			this.paintAppearance = new AppearanceObject();
		}
		protected internal virtual void CheckRTL(Rectangle ownerBounds) {
			Rectangle prevBounds = Bounds;
			int dx = ownerBounds.Right - Bounds.Left - Bounds.Right + ownerBounds.X;
			this.bounds.Offset(dx, 0);
			if(!this.dropDownButtonBounds.IsEmpty) {
				this.dropDownButtonBounds.X = this.bounds.Left;
			}
			if(!this.textBounds.IsEmpty) {
				int textIndent = TextBounds.Left - prevBounds.Left - 1;
				if(this.dropDownButtonBounds.Width != 0) {
					this.textBounds.X = this.dropDownButtonBounds.Right + textIndent;
				}
				else {
					this.textBounds.X = this.bounds.Left + textIndent;
				}
			}
		}
		public string Caption { get { return Node.Caption; } }
		public BreadCrumbNode Node { get { return node; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle TextBounds { get { return textBounds; } }
		public Rectangle DropDownButtonBounds { get { return dropDownButtonBounds; } }
		public bool ShowCaption { get { return node.ShowCaption; } }
		public ObjectState State { get { return state; } set { state = value; } }
		public AppearanceObject PaintAppearance { get { return paintAppearance; } }
		public bool IsMatch(BreadCrumbNodeInfo nodeInfo) {
			if(nodeInfo == null || Node == null) return false;
			return object.ReferenceEquals(Node, nodeInfo.Node);
		}
		public override string ToString() {
			return string.Format("Node: {0}, Bounds: {1}, TextBounds: {2}", Node.ToString(), Bounds.ToString(), TextBounds.ToString());
		}
		protected internal virtual void Offset(int x, int y) {
			if(!this.bounds.IsEmpty) {
				this.bounds.Offset(x, y);
			}
			if(!this.textBounds.IsEmpty) {
				this.textBounds.Offset(x, y);
			}
			if(!this.dropDownButtonBounds.IsEmpty) {
				this.dropDownButtonBounds.Offset(x, y);
			}
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class BreadCrumbEditPainter : ButtonEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			BreadCrumbEditViewInfo viewInfo = (BreadCrumbEditViewInfo)info.ViewInfo;
			base.DrawContent(info);
			if(viewInfo.AllowSelector) {
				DrawSelector(info);
			}
			DrawButtons(info);
			DrawGlyph(info);
		}
		protected override void DrawEmptyArea(ControlGraphicsInfoArgs info, Rectangle maskBoxRect) {
			BreadCrumbEditViewInfo viewInfo = (BreadCrumbEditViewInfo)info.ViewInfo;
			base.DrawEmptyArea(info, maskBoxRect);
			if(viewInfo.AllowRootGlyph) {
				info.Paint.FillRectangle(info.Graphics, viewInfo.PaintAppearance.GetBackBrush(info.Cache, viewInfo.Bounds), viewInfo.GlyphEmptySpaceRect);
			}
		}
		protected virtual void DrawSelector(ControlGraphicsInfoArgs info) {
			DrawItems(info);
		}
		protected virtual void DrawItems(ControlGraphicsInfoArgs info) {
			BreadCrumbEditViewInfo viewInfo = (BreadCrumbEditViewInfo)info.ViewInfo;
			GraphicsClipState clipState = info.Cache.ClipInfo.SaveAndSetClip(viewInfo.ContentClipRect);
			try {
				foreach(BreadCrumbNodeInfo nodeInfo in viewInfo.VisibleItems) {
					DrawItem(info, nodeInfo);
				}
			}
			finally {
				info.Cache.ClipInfo.RestoreClipRelease(clipState);
			}
		}
		protected virtual void DrawItem(ControlGraphicsInfoArgs info, BreadCrumbNodeInfo nodeInfo) {
			BreadCrumbEditViewInfo viewInfo = (BreadCrumbEditViewInfo)info.ViewInfo;
			viewInfo.BreadCrumbNodeObjectInfoArgs.Cache = info.Cache;
			viewInfo.BreadCrumbNodeObjectInfoArgs.Init(nodeInfo);
			try {
				viewInfo.GetNodePainter(nodeInfo).DrawObject(viewInfo.BreadCrumbNodeObjectInfoArgs);
			}
			finally {
				viewInfo.BreadCrumbNodeObjectInfoArgs.Cache = null;
				viewInfo.BreadCrumbNodeObjectInfoArgs.ResetAppearance();
			}
		}
		protected override void DrawGlyphCore(ControlGraphicsInfoArgs info, ButtonEditViewInfo _) {
			BreadCrumbEditViewInfo vi = (BreadCrumbEditViewInfo)info.ViewInfo;
			info.Cache.Paint.FillRectangle(info.Graphics, vi.PaintAppearance.GetBackBrush(info.Cache), vi.GlyphAreaBounds);
			info.Cache.Paint.DrawImage(info.Graphics, vi.RootGlyph, vi.GlyphBounds, new Rectangle(Point.Empty, vi.ImageSize), vi.State != ObjectState.Disabled);
		}
	}
	public class BreadCrumbNodeObjectInfoArgs : ObjectInfoArgs {
		string caption;
		bool showCaption;
		Rectangle textBounds, dropDownButtonBounds;
		BreadCrumbEditViewInfo viewInfo;
		AppearanceObject paintAppearance;
		public BreadCrumbNodeObjectInfoArgs(BreadCrumbEditViewInfo viewInfo) {
			this.caption = string.Empty;
			this.textBounds = Rectangle.Empty;
			this.viewInfo = viewInfo;
			this.paintAppearance = null;
		}
		public virtual void Init(BreadCrumbNodeInfo nodeInfo) {
			this.Bounds = nodeInfo.Bounds;
			this.State = nodeInfo.State;
			this.caption = nodeInfo.Caption;
			this.textBounds = nodeInfo.TextBounds;
			this.showCaption = nodeInfo.ShowCaption;
			this.dropDownButtonBounds = nodeInfo.DropDownButtonBounds;
			this.paintAppearance = nodeInfo.PaintAppearance;
		}
		public virtual void ResetAppearance() {
			this.paintAppearance = null;
		}
		public string Caption { get { return caption; } }
		public bool ShowCaption { get { return showCaption; } }
		public Rectangle TextBounds { get { return textBounds; } }
		public Rectangle DropDownButtonBounds { get { return dropDownButtonBounds; } }
		public BreadCrumbEditViewInfo ViewInfo { get { return viewInfo; } }
		public AppearanceObject PaintAppearance { get { return paintAppearance; } }
		public bool HasDropDownButton { get { return DropDownButtonBounds != Rectangle.Empty; } }
	}
	public abstract class BaseBreadCrumbNodePainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			BreadCrumbNodeObjectInfoArgs info = (BreadCrumbNodeObjectInfoArgs)e;
			DrawBackground(info);
			DrawCaption(info);
			if(info.HasDropDownButton) DrawDropDownButton(info);
		}
		public void DrawCaption(BreadCrumbNodeObjectInfoArgs info) {
			if(info.ShowCaption) DrawCaptionCore(info);
		}
		public virtual void DrawDropDownButton(BreadCrumbNodeObjectInfoArgs info) {
			DrawDropDownButtonBackground(info);
			DrawDropDownButtonArrow(info);
		}
		public virtual int GetHorzTextIndent(BreadCrumbNodeObjectInfoArgs info) {
			return 4;
		}
		public virtual int GetDropDownButtonWidth(BreadCrumbNodeObjectInfoArgs info) {
			return 20;
		}
		public abstract void DrawBackground(BreadCrumbNodeObjectInfoArgs info);
		public abstract void DrawCaptionCore(BreadCrumbNodeObjectInfoArgs info);
		public abstract void DrawDropDownButtonBackground(BreadCrumbNodeObjectInfoArgs info);
		public abstract void DrawDropDownButtonArrow(BreadCrumbNodeObjectInfoArgs info);
		public abstract Color GetNodeForeColor(BreadCrumbNodeObjectInfoArgs info, ObjectState state);
	}
	public class SkinBreadCrumbNodePainter : BaseBreadCrumbNodePainter {
		protected SkinElement GetSkinElement(BaseControlViewInfo viewInfo, string element) {
			return EditorsSkins.GetSkin(viewInfo.LookAndFeel)[element];
		}
		protected Color GetCommonSkinColor(BaseControlViewInfo viewInfo, string color) {
			return CommonSkins.GetSkin(viewInfo.LookAndFeel).Colors[color];
		}
		protected Color GetElementColor(SkinElement element, string propName) {
			object prop = element.Properties[propName];
			if(prop == null) return Color.Empty;
			return (Color)prop;
		}
		public override void DrawBackground(BreadCrumbNodeObjectInfoArgs info) {
			if(info.State == ObjectState.Normal) return;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, GetEditorButtonInfo(info, info.Bounds));
		}
		public override void DrawCaptionCore(BreadCrumbNodeObjectInfoArgs info) {
			info.PaintAppearance.DrawString(info.Cache, info.Caption, info.TextBounds, info.PaintAppearance.GetStringFormat(info.ViewInfo.DefaultTextOptions));
		}
		public override void DrawDropDownButtonBackground(BreadCrumbNodeObjectInfoArgs info) {
			if(info.State == ObjectState.Normal) return;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, GetEditorButtonInfo(info, info.DropDownButtonBounds));
		}
		public override void DrawDropDownButtonArrow(BreadCrumbNodeObjectInfoArgs info) {
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, GetDropDownButtonArrowInfo(info));
		}
		public override int GetHorzTextIndent(BreadCrumbNodeObjectInfoArgs info) {
			SkinElement element = GetSkinElement(info.ViewInfo, EditorsSkins.SkinEditorButton);
			if(element != null)
				return element.ContentMargins.Left + 2;
			return base.GetHorzTextIndent(info);
		}
		public override int GetDropDownButtonWidth(BreadCrumbNodeObjectInfoArgs info) {
			SkinElement element = GetSkinElement(info.ViewInfo, EditorsSkins.SkinEditorButton);
			if(element != null)
				return Math.Max(element.Size.MinSize.Width, 19);
			return base.GetDropDownButtonWidth(info);
		}
		protected virtual SkinElementInfo GetEditorButtonInfo(BreadCrumbNodeObjectInfoArgs info, Rectangle bounds) {
			ObjectState state = info.State;
			SkinElementInfo elementInfo = new SkinElementInfo(GetSkinElement(info.ViewInfo, EditorsSkins.SkinEditorButton), bounds);
			int imageIndex = -1;
			if(state == ObjectState.Normal)
				imageIndex = 0;
			else if(state == ObjectState.Hot)
				imageIndex = 1;
			else if((state & ObjectState.Pressed) != 0)
				imageIndex = 2;
			else if(state == ObjectState.Disabled)
				imageIndex = 4;
			if(imageIndex == -1) return null;
			elementInfo.ImageIndex = imageIndex;
			elementInfo.RightToLeft = info.ViewInfo.RightToLeft;
			return elementInfo;
		}
		protected virtual SkinElementInfo GetDropDownButtonArrowInfo(BreadCrumbNodeObjectInfoArgs info) {
			ObjectState state = info.State;
			SkinElementInfo elementInfo = new SkinElementInfo(GetSkinElement(info.ViewInfo, EditorsSkins.SkinNavigator), info.DropDownButtonBounds);
			int imageIndex = -1;
			if((state & ObjectState.Pressed) != 0) {
				imageIndex = 16;
			}
			else if(info.State == ObjectState.Normal || info.State == ObjectState.Disabled || info.State == ObjectState.Hot) {
				imageIndex = info.ViewInfo.RightToLeft ? 2 : 3;
			}
			if(imageIndex == -1) return null;
			elementInfo.ImageIndex = imageIndex;
			return elementInfo;
		}
		public override Color GetNodeForeColor(BreadCrumbNodeObjectInfoArgs info, ObjectState state) {
			Color color = Color.Empty;
			SkinElement editorButton = GetSkinElement(info.ViewInfo, EditorsSkins.SkinEditorButton);
			if(((state & ObjectState.Pressed) != 0) || ((state & ObjectState.Hot) != 0)) {
				color = editorButton.Color.GetForeColor();
				if(color.IsEmpty) color = GetCommonSkinColor(info.ViewInfo, CommonColors.HighlightText);
			}
			else {
				color = GetCommonSkinColor(info.ViewInfo, CommonColors.WindowText);
			}
			return color;
		}
	}
	public class SkinBreadCrumbOverflowNodePainter : SkinBreadCrumbNodePainter {
		public override void DrawDropDownButtonArrow(BreadCrumbNodeObjectInfoArgs info) {
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, GetOverflowButtonArrowInfo(info));
		}
		protected virtual SkinElementInfo GetOverflowButtonArrowInfo(BreadCrumbNodeObjectInfoArgs info) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetSkinElement(info.ViewInfo, EditorsSkins.SkinNavigator), info.DropDownButtonBounds);
			elementInfo.ImageIndex = 1;
			return elementInfo;
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public abstract class BreadCrumbPopupFormBase : ComboBoxPopupListBoxForm {
		public BreadCrumbPopupFormBase(BreadCrumbEdit owner)
			: base(owner) {
		}
		[Browsable(false)]
		public new BreadCrumbEdit OwnerEdit { get { return base.OwnerEdit as BreadCrumbEdit; } }
	}
	[ToolboxItem(false)]
	public class BreadCrumbPopupForm : BreadCrumbPopupFormBase {
		public BreadCrumbPopupForm(BreadCrumbEdit owner)
			: base(owner) {
		}
		protected override PopupListBox CreateListBox() {
			return new BreadCrumbPopupListBox(this);
		}
		protected internal override void OnBeforeShowPopup() {
			base.OnBeforeShowPopup();
			AssignListBoxFilter();
		}
		protected override void OnListBox_SelectedIndexChanged(object sender, EventArgs e) {
			base.OnListBox_SelectedIndexChanged(sender, e);
			OwnerEdit.ResetCaret();
		}
		protected override bool AllowUpdateSelectedItem { get { return false; } }
		protected virtual void AssignListBoxFilter() {
			ListBox.SetFilter(OwnerEdit.AutoSearchText);
		}
	}
	[ToolboxItem(false)]
	public class BreadCrumbNodePopupForm : BreadCrumbPopupFormBase {
		Point loc;
		BreadCrumbNode node;
		public BreadCrumbNodePopupForm(BreadCrumbEdit owner)
			: base(owner) {
		}
		public virtual void Initialize(Point loc, BreadCrumbNode node) {
			this.loc = loc;
			this.node = node;
			UpdateDataSource();
		}
		protected override PopupListBox CreateListBox() {
			return new BreadCrumbNodePopupListBox(this);
		}
		protected virtual void UpdateDataSource() {
			ListBox.DataSource = GetDataSourceCore(this.node);
		}
		protected virtual ComboBoxItemCollection GetDataSourceCore(BreadCrumbNode node) {
			ComboBoxItemCollection items = new ComboBoxItemCollection(Properties);
			foreach(BreadCrumbNode child in node.ChildNodes) {
				items.Add(new ImageComboBoxItem(child, child.ImageIndex));
			}
			return items;
		}
		public BreadCrumbNode Node { get { return node; } }
		public Point TargetLocation { get { return loc; } }
		protected override int CalcFormWidth(Size desiredSize, Size minSize) {
			int baseWidth = PopupListFormHelper.CalcMinimumComboWidth((IList)ListBox.DataSource, Properties.NodeDropDownRowCount);
			return (int)(baseWidth * 1.8f);
		}
		protected override int CalcFormHeight(int itemCount) {
			int height = base.CalcFormHeight(itemCount);
			if(itemCount > 0) {
				height += ListBox.ViewInfo.CalcGroupCount(itemCount) * ListBox.ViewInfo.CalcItemSeparatorHeight();
			}
			return height;
		}
		public new virtual BreadCrumbNodePopupListBox ListBox { get { return base.ListBox as BreadCrumbNodePopupListBox; } }
		protected override int DropDownRows {
			get { return Properties.NodeDropDownRowCount; }
		}
		public override object ResultValue {
			get {
				return base.ResultValue;
			}
		}
		[DXCategory(CategoryName.Properties)]
		public new RepositoryItemBreadCrumbEdit Properties { get { return base.Properties as RepositoryItemBreadCrumbEdit; } }
	}
	[ToolboxItem(false)]
	public class BreadCrumbNodePopupListBox : BreadCrumbPopupListBox {
		public BreadCrumbNodePopupListBox(BreadCrumbNodePopupForm popupForm)
			: base(popupForm) {
		}
		protected override object GetDataSource() {
			return null;
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new BreadCrumbNodePopupListBoxViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new BreadCrumbNodePopupListBoxPainter();
		}
		protected internal new BreadCrumbNodePopupListBoxViewInfo ViewInfo { get { return base.ViewInfo as BreadCrumbNodePopupListBoxViewInfo; } }
		[Browsable(false)]
		new public BreadCrumbNodePopupForm OwnerForm { get { return base.OwnerForm as BreadCrumbNodePopupForm; } }
	}
	public class BreadCrumbNodePopupListBoxPainter : PopupImageListBoxPainter {
		protected override void DrawItemImageCore(GraphicsInfoArgs ginfo, PopupImageComboBoxEditListBoxViewInfo vi, ImageListBoxViewInfo.ImageItemInfo item) {
			BreadCrumbNodePopupListBoxViewInfo viewInfo = (BreadCrumbNodePopupListBoxViewInfo)vi;
			Image img = ((BreadCrumbNode)item.Item).GetImage();
			Utils.Paint.XPaint.Graphics.DrawImage(ginfo.Cache.Graphics, img, item.ImageRect);
		}
		protected override bool AllowDrawItemImage(PopupImageComboBoxEditListBoxViewInfo vi, ImageListBoxViewInfo.ImageItemInfo item) {
			BreadCrumbNodePopupListBoxViewInfo viewInfo = (BreadCrumbNodePopupListBoxViewInfo)vi;
			return viewInfo.AllowDrawNodeImage(item);
		}
		protected override void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			var item = (BreadCrumbNodePopupListBoxViewInfo.BreadCrumbNodeImageItemInfo)itemInfo;
			base.DrawItemCore(info, itemInfo, e);
			if(item.BeginGroup) {
				DrawItemSeparator(info, item);
			}
		}
		protected virtual void DrawItemSeparator(ControlGraphicsInfoArgs info, BreadCrumbNodePopupListBoxViewInfo.BreadCrumbNodeImageItemInfo itemInfo) {
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, GetItemSepatorInfo(info, itemInfo));
		}
		protected virtual SkinElementInfo GetItemSepatorInfo(ControlGraphicsInfoArgs info, BreadCrumbNodePopupListBoxViewInfo.BreadCrumbNodeImageItemInfo itemInfo) {
			BreadCrumbPopupListBoxViewInfo viewInfo = (BreadCrumbPopupListBoxViewInfo)info.ViewInfo;
			return new SkinElementInfo(BarSkins.GetSkin(viewInfo.LookAndFeel)[BarSkins.SkinPopupMenuSeparator], itemInfo.SeparatorRect);
		}
	}
	public class BreadCrumbNodePopupListBoxViewInfo : BreadCrumbPopupListBoxViewInfo {
		public BreadCrumbNodePopupListBoxViewInfo(BreadCrumbPopupListBox owner)
			: base(owner) {
		}
		protected override Rectangle CalcItemBounds(int itemIndex, int yCoord) {
			Rectangle r = base.CalcItemBounds(itemIndex, yCoord);
			if(ShouldBeginGroup(itemIndex)) {
				r.Offset(0, CalcItemSeparatorHeight());
			}
			return r;
		}
		protected override ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
			BreadCrumbNodeImageItemInfo itemInfo = (BreadCrumbNodeImageItemInfo)base.CreateItemInfo(bounds, item, text, index);
			if(ShouldBeginGroup(index)) {
				itemInfo.SetBeginGroup();
				itemInfo.SetSeparatorRect(CalcItemSeparatorBounds(itemInfo));
			}
			return itemInfo;
		}
		protected Rectangle CalcItemSeparatorBounds(BreadCrumbNodeImageItemInfo itemInfo) {
			Rectangle rect = Rectangle.Empty;
			rect.X = itemInfo.Bounds.X;
			rect.Y = itemInfo.Bounds.Y - CalcItemSeparatorHeight();
			rect.Width = itemInfo.Bounds.Width;
			rect.Height = CalcItemSeparatorHeight();
			return rect;
		}
		protected internal int CalcGroupCount(int itemCount) {
			int groupCount = 0;
			for(int i = 0; i < itemCount; i++) {
				if(ShouldBeginGroup(i)) groupCount++;
			}
			return groupCount;
		}
		protected bool ShouldBeginGroup(int itemIndex) {
			if(itemIndex == 0) return false;
			BreadCrumbNode current = GetNode(itemIndex);
			BreadCrumbNode prev = GetNode(itemIndex - 1);
			if(current.IsParentPersistent) {
				if(!prev.IsParentPersistent) return true;
				else {
					if(!object.ReferenceEquals(current.Parent, prev.Parent)) return true;
				}
			}
			return false;
		}
		protected BreadCrumbNode GetNode(int itemIndex) {
			return (BreadCrumbNode)OwnerControl.GetItem(itemIndex);
		}
		protected internal int CalcItemSeparatorHeight() {
			return BarSkins.GetSkin(LookAndFeel)[BarSkins.SkinPopupMenuSeparator].Size.MinSize.Height;
		}
		protected override void UpdateItemAppearance(BaseListBoxViewInfo.ItemInfo itemInfo) {
			itemInfo.PaintAppearance.Assign(GetItemAppearance(itemInfo));
			itemInfo.PaintAppearance.ForeColor = GetItemForeColor(itemInfo);
			itemInfo.PaintAppearance.BackColor = GetItemBackColor(itemInfo);
		}
		protected virtual AppearanceObject GetItemAppearance(ItemInfo itemInfo) {
			BreadCrumbNodeImageItemInfo info = (BreadCrumbNodeImageItemInfo)itemInfo;
			if(!info.IsFlagged) return PaintAppearance;
			AppearanceObject obj = (AppearanceObject)PaintAppearance.Clone();
			obj.Font = new Font(PaintAppearance.Font, FontStyle.Bold);
			return obj;
		}
		protected override ImageListBoxViewInfo.ImageItemInfo CreateImageInfoInstance(Rectangle bounds, object item, string text, int index) {
			return new BreadCrumbNodeImageItemInfo(OwnerControl, bounds, item, text, index, GetIsFlagged((BreadCrumbNode)item));
		}
		protected internal virtual bool GetIsFlagged(BreadCrumbNode node) {
			return OwnerEdit.Properties.GetIsFlagged(node);
		}
		public bool AllowDrawNodeImage(ImageListBoxViewInfo.ImageItemInfo item) {
			BreadCrumbNode node = item.Item as BreadCrumbNode;
			return node != null ? node.GetImage() != null : false;
		}
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if(IsSkinnedHighlightingEnabled) return new BreadCrumbNodePopupSkinItemPainter();
			return new BreadCrumbNodePopupItemPainter();
		}
		new protected BreadCrumbEdit OwnerEdit { get { return base.OwnerEdit as BreadCrumbEdit; } }
		public class BreadCrumbNodeImageItemInfo : ImageListBoxViewInfo.ImageItemInfo {
			bool isFlagged;
			bool beginGroup;
			Rectangle separatorRect;
			public BreadCrumbNodeImageItemInfo(BaseListBoxControl ownerControl, Rectangle rect, object item, string text, int index, bool isFlagged) : base(ownerControl, rect, item, text, index) {
				this.beginGroup = false;
				this.isFlagged = isFlagged;
				this.separatorRect = Rectangle.Empty;
			}
			internal void SetBeginGroup() {
				this.beginGroup = true;
			}
			internal void SetSeparatorRect(Rectangle rect) {
				this.separatorRect = rect;
			}
			public bool IsFlagged { get { return isFlagged; } }
			public bool BeginGroup { get { return beginGroup; } }
			public Rectangle SeparatorRect { get { return separatorRect; } }
		}
		protected internal new BreadCrumbNodePopupListBox OwnerControl { get { return base.OwnerControl as BreadCrumbNodePopupListBox; } }
	}
	public class BreadCrumbNodePopupSkinItemPainter : BreadCrumbPopupSkinItemPainter {
		public override int GetVertPadding(ListBoxItemObjectInfoArgs e) {
			return 8;
		}
	}
	public class BreadCrumbNodePopupItemPainter : BreadCrumbPopupItemPainter {
		public override int GetVertPadding(ListBoxItemObjectInfoArgs e) {
			return 8;
		}
	}
	[ToolboxItem(false)]
	public class BreadCrumbPopupListBox : PopupImageComboBoxEditListBox {
		public BreadCrumbPopupListBox(BreadCrumbPopupFormBase ownerForm)
			: base(ownerForm) {
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new BreadCrumbPopupListBoxViewInfo(this);
		}
		public override HorzAlignment GlyphAlignment {
			get { return HorzAlignment.Near; }
		}
		public override object Images { get { return Properties.Images; } }
		[Browsable(false)]
		new public RepositoryItemBreadCrumbEdit Properties { get { return OwnerEdit.Properties; } }
		[Browsable(false)]
		public new BreadCrumbEdit OwnerEdit { get { return OwnerForm.OwnerEdit; } }
		[Browsable(false)]
		new public BreadCrumbPopupFormBase OwnerForm { get { return base.OwnerForm as BreadCrumbPopupFormBase; } }
	}
	public class BreadCrumbPopupListBoxViewInfo : PopupImageComboBoxEditListBoxViewInfo {
		public BreadCrumbPopupListBoxViewInfo(BreadCrumbPopupListBox owner)
			: base(owner) {
		}
		protected override BaseListBoxItemPainter CreateItemPainter() {
			if(IsSkinnedHighlightingEnabled) return new BreadCrumbPopupSkinItemPainter();
			return new BreadCrumbPopupItemPainter();
		}
	}
	public class BreadCrumbPopupItemPainter : PopupImageListBoxItemPainter {
		public override int GetVertPadding(ListBoxItemObjectInfoArgs e) {
			return 4;
		}
	}
	public class BreadCrumbPopupSkinItemPainter : PopupImageListBoxSkinItemPainter {
		public override int GetVertPadding(ListBoxItemObjectInfoArgs e) {
			return 4;
		}
	}
}
