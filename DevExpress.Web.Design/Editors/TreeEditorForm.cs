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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Windows.Forms;
using System.Text;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public enum DragNodeState { AddChild, AddToEnd, AddRoot, Insert, DropToScanTreeView, None };
	public abstract class TreeItemsEditorForm : TreeEditorForm {
		ToolStripDropDownButton convertDropDown = null;
		ToolStripMenuItem convertMenuItem = null;
		protected TreeViewEx TreeView { get { return fItemsTreeView; } }
		public TreeItemsEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
		}
		protected ToolStripDropDownButton ConvertDropDown { get { return convertDropDown; } }
		protected ToolStripMenuItem ConvertMenuItem { get { return convertMenuItem; } }
		protected override Size FormDefaultSize { get { return new Size(860, 500); } }
		protected override Size FormMinimumSize { get { return new Size(700, 500); } }
		protected override int LeftPanelDefaultWidth { get { return 400; } }
		protected override void CreateCustomMenuItems(List<ToolStripItem> buttons) {
			base.CreateCustomMenuItems(buttons);
			this.convertMenuItem = new ToolStripMenuItem(GetConvertMenuItemText());
			ConvertMenuItem.DropDownItems.AddRange(GetConvertDropDownItems(GetCollectionItemTypes(), OnChangeColumnType));
			buttons.Add(ConvertMenuItem);
		}
		protected abstract void OnChangeColumnType(object sender, EventArgs e);
		protected abstract string GetConvertMenuItemText();
		protected ToolStripItem[] GetConvertDropDownItems(List<CollectionItemType> items, EventHandler onClick) {
			List<ToolStripItem> result = new List<ToolStripItem>();
			foreach (CollectionItemType item in items) {
				if (item.BeginGroup && result.Count > 0)
					result.Add(new ToolStripSeparator());
				ToolStripMenuItem toolStripItem = new ToolStripMenuItem(item.Text, null, onClick);
				toolStripItem.Tag = item;
				result.Add(toolStripItem);
			}
			return result.ToArray();
		}
		protected override void AddToolStripButtons(List<ToolStripItem> buttons) {
			base.AddToolStripButtons(buttons);
			buttons.Add(CreateToolStripSeparator());
			this.convertDropDown = new ToolStripDropDownButton("Change To");
			buttons.Add(ConvertDropDown);
			ConvertDropDown.DropDownItems.AddRange(GetConvertDropDownItems(GetCollectionItemTypes(), OnChangeColumnType));
		}
	}
	public abstract class TreeEditorForm : ItemsEditorFormBase {
		private const string AddCursorResource = "DevExpress.Web.Design.Cursors.ADD.cur";
		private const string AddChildCursorResource = "DevExpress.Web.Design.Cursors.ADDCHILD.cur";
		private const string InsertCursorResource = "DevExpress.Web.Design.Cursors.INSERT.cur";
		private const string AddChildItemImageResource = "DevExpress.Web.Design.Images.AddChildItem.png";
		private const string IncreaseIndentImageResource = "DevExpress.Web.Design.Images.IncreaseIndent.png";
		private const string DecreaseIndentImageResource = "DevExpress.Web.Design.Images.DecreaseIndent.png";
		private const int TreeEditorItemsPanelMinimizeWidth = 225;
		private Cursor fCurrentCursor; 
		private DragNodeState fDragNodeState;
		private object fSelectedObject = null;
		protected TreeViewEx fItemsTreeView = null;		
		protected override int LeftPanelMinimizeWidth {
			get { return TreeEditorItemsPanelMinimizeWidth; }
		}
		protected virtual IHierarchicalEnumerable Tree { 
			get { return PropertyValue as IHierarchicalEnumerable; } 
		}
		protected object SelectedObject {
			get { return fSelectedObject; }
		}
		public TreeEditorForm(object component, ITypeDescriptorContext context,
			IServiceProvider provider, object propertyValue)
			: base(component, context, provider, propertyValue) {
		}
		protected override void AddMenuItems(List<ToolStripItem> buttons) {
			base.AddMenuItems(buttons);
			AddChildMenuItem(buttons);
			AddHorzMoveMenuItems(buttons);
		}
		protected virtual void AddChildMenuItem(List<ToolStripItem> buttons) {
			List<CollectionItemType> items = GetCollectionItemTypes();
			ToolStripItem menuItem = items.Count > 0
				? CreateMenuSplitItem(StringResources.ItemsEditorPopupMenu_AddChildItemButtonText, AddChildItemImageResource, items)
				: CreateMenuItem(StringResources.ItemsEditorPopupMenu_AddChildItemButtonText, AddChildItemImageResource, OnAddChild, Keys.None);
			buttons.Insert(1, menuItem);
		}
		protected virtual ToolStripItem CreateMenuSplitItem(string toolTip, string image, List<CollectionItemType> items) {
			return new ToolStripMenuItem(); 
		}
		protected virtual void AddHorzMoveMenuItems(List<ToolStripItem> buttons) {
			buttons.Insert(7, CreateMenuItem(StringResources.TreeEditorPopupMenu_MoveLeftItemButtonText, DecreaseIndentImageResource,
				OnDecreaseIndent, Keys.None));
			buttons.Insert(8, CreateMenuItem(StringResources.TreeEditorPopupMenu_MoveRightItemButtonText, IncreaseIndentImageResource,
				OnIncreaseIndent, Keys.None));
		}
		protected override void AddToolStripButtons(List<ToolStripItem> buttons) {
			base.AddToolStripButtons(buttons);
			AddChildToolStripButton(buttons);
			AddHorzMoveToolStripButtons(buttons);
		}
		protected virtual void AddChildToolStripButton(List<ToolStripItem> buttons) {
			List<CollectionItemType> items = GetCollectionItemTypes();
			ToolStripItem button = items.Count > 0 
				? CreateToolStripSplitButton(StringResources.TreeEditor_AddChildButtonText, AddChildItemImageResource, items)
				: CreatePushButton(StringResources.TreeEditor_AddChildButtonText, AddChildItemImageResource, OnAddChild);
			buttons.Insert(1, button);
		}
		protected virtual ToolStripItem CreateToolStripSplitButton(string toolTip, string image, List<CollectionItemType> items) {
			return new ToolStripButton(); 
		}
		protected virtual void AddHorzMoveToolStripButtons(List<ToolStripItem> buttons) {
			buttons.Insert(7, CreatePushButton(StringResources.TreeEditor_IncreaseIndentButtonText,
				IncreaseIndentImageResource, OnIncreaseIndent));
			buttons.Insert(8, CreatePushButton(StringResources.TreeEditor_DecreaseIndentButtonText,
				DecreaseIndentImageResource, OnDecreaseIndent));
		}
		protected override void AssignControls() {
			FillItemsViewer();
			if (fItemsTreeView.Nodes.Count > 0)
				fItemsTreeView.SelectedNode = fItemsTreeView.Nodes[0];
			base.AssignControls();
		}
		protected override System.Windows.Forms.Control CreateItemViewer() {
			fItemsTreeView = new TreeViewEx();
			fItemsTreeView.ContextMenuStrip = PopupMenu;
			fItemsTreeView.Dock = DockStyle.Fill;
			fItemsTreeView.Margin = new Padding(0);
			fItemsTreeView.HideSelection = false;
			fItemsTreeView.AllowDrop = true;
			fItemsTreeView.AfterSelect += new TreeViewEventHandler(OnAfterSelect);
			fItemsTreeView.DragDrop += new DragEventHandler(OnDragDropTreeView);
			fItemsTreeView.DragOver += new DragEventHandler(OnDragOverTreeView);
			fItemsTreeView.ItemDrag += new ItemDragEventHandler(OnItemDragTreeView);
			fItemsTreeView.GiveFeedback += new GiveFeedbackEventHandler(OnGiveFeedbackTreeView);
			fItemsTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(OnTreeNodeMouseClick);						
			return fItemsTreeView;
		}
		protected override object CreateNewItem() {
			throw new Exception(StringResources.Serializer_OperationNotImplemented);
		}
		protected override bool IsInsertButtonEnable() {
			return fItemsTreeView.SelectedNode != null;
		}
		protected override bool IsMoveDownButtonEnabled() {
			return fItemsTreeView.SelectedNode != null && fItemsTreeView.SelectedNode.NextNode != null;
		}
		protected override bool IsMoveUpButtonEnabled() {
			return fItemsTreeView.SelectedNode != null && fItemsTreeView.SelectedNode.PrevNode != null;
		}
		protected override void FocusItemViewer() {
			fItemsTreeView.Focus();
		}
		protected override string GetPropertyStorePathPrefix() {
			return "TreeEditorForm";
		}
		protected override void SetVisiblePropertyInViewer() {
			fItemsTreeView.SelectedNode.Text = SelectedObject.ToString();
		}
		protected override IList GetParentViewerItemCollection() {
			return GetTreeViewNodesInSameLevel(fItemsTreeView.SelectedNode); 
		}
		protected override void MoveViewerItem(int oldIndex, int newIndex) {
			IList collection = GetParentViewerItemCollection();
			object item = collection[oldIndex];
			collection.Remove(item);
			collection.Insert(newIndex, item);
		}		
		protected override void MoveUpViewerItem() {
			TreeNode node = fItemsTreeView.SelectedNode;
			int index = GetViewerItemIndex(fItemsTreeView.SelectedNode);
			MoveViewerItem(index, index - 1);
			fItemsTreeView.SelectedNode = node ;
		}
		protected override void MoveDownViewerItem() {
			TreeNode node = fItemsTreeView.SelectedNode;
			int index = GetViewerItemIndex(fItemsTreeView.SelectedNode);
			MoveViewerItem(index, index + 1);
			fItemsTreeView.SelectedNode = node;
		}
		protected override void MoveUpItem() {
			MoveUpItem(GetParentItem(), fItemsTreeView.SelectedNode.Tag);
			base.MoveUpItem();
		}
		protected override void MoveDownItem() {
			MoveDownItem(GetParentItem(), fItemsTreeView.SelectedNode.Tag);
			base.MoveDownItem();
		}
		protected override ToolStripItem CreateSelectAllMenuItem() { return null; }
		protected override void SelectAll() { }
		protected override void SetTabIndexes() {
			base.SetTabIndexes();
			fItemsTreeView.TabStop = true;
			fItemsTreeView.TabIndex = TabOrder[0];
		}
		protected override void OnAddNewItem(object item) {
			AddChild(GetParentItem(), item);
			TreeNode node = CreateTreeNode(item);
			if (fItemsTreeView.SelectedNode == null || fItemsTreeView.SelectedNode.Level == 0)
				AddNewNode(fItemsTreeView.Nodes, node);
			else
				AddNewNode(fItemsTreeView.SelectedNode.Parent.Nodes, node);
			fItemsTreeView.SelectedNode = node;
			fItemsTreeView.Update();
		}
		protected override void OnInsertItem(object item) {
			InsertItem(GetParentItem(), item, SelectedObject);
			int index = fItemsTreeView.SelectedNode.Index;
			TreeNode node = CreateTreeNode(item);
			if (fItemsTreeView.SelectedNode == null || fItemsTreeView.SelectedNode.Level == 0)
				InsertNode(index, fItemsTreeView.Nodes, node);			
			else
				InsertNode(index, fItemsTreeView.SelectedNode.Parent.Nodes, node);
			fItemsTreeView.SelectedNode = node;
			fItemsTreeView.Update();
		}
		protected override void FillItemsViewer() {
			FillTreeView(fItemsTreeView, Tree);
			fItemsTreeView.Update();
			UpdateTools();
		}
		protected override void RemoveAllItems() {
			fItemsTreeView.Nodes.Clear();
			base.RemoveAllItems();
		}
		protected override void RemoveItem() {
			TreeNode newSelectedNode = fItemsTreeView.SelectedNode.PrevNode != null ?
				fItemsTreeView.SelectedNode.PrevNode : fItemsTreeView.SelectedNode.Parent != null ?
				fItemsTreeView.SelectedNode.Parent : fItemsTreeView.SelectedNode.NextNode;
			RemoveItem(GetParentItem(), fItemsTreeView.SelectedNode.Tag);
			fItemsTreeView.SelectedNode.Remove();
			if (newSelectedNode != null)
				fItemsTreeView.SelectedNode = newSelectedNode;
			base.RemoveItem();
		}
		protected override void UpdateToolStrip() {
			UpdateAddChildToolStrip();
			UpdateHorzMoveToolStripItems();
			base.UpdateToolStrip();
		}
		protected virtual void UpdateAddChildToolStrip() {
			ToolStripItem item = FindToolItemByText(StringResources.TreeEditor_AddChildButtonText);
			item.Enabled = fItemsTreeView.Nodes.Count != 0;
		}
		protected virtual void UpdateHorzMoveToolStripItems() {
			ToolStripItem item = FindToolItemByText(StringResources.TreeEditor_DecreaseIndentButtonText);
			item.Enabled = CanDecreaseIndent();
			item = FindToolItemByText(StringResources.TreeEditor_IncreaseIndentButtonText);
			item.Enabled = CanIncreaseIndent();
		}
		protected override void UpdateMenuStrip() {
			UpdateAddChildMenuStrip();
			UpdateHorzMoveMenuStripItems();
			base.UpdateMenuStrip();
		}
		protected virtual void UpdateAddChildMenuStrip() {
			ToolStripItem item = FindPopupMenuItemByText(StringResources.ItemsEditorPopupMenu_AddChildItemButtonText);
			item.Enabled = fItemsTreeView.Nodes.Count != 0;
		}
		protected virtual void UpdateHorzMoveMenuStripItems() {
			ToolStripItem item = FindPopupMenuItemByText(StringResources.TreeEditorPopupMenu_MoveLeftItemButtonText);
			item.Enabled = CanDecreaseIndent();
			item = FindPopupMenuItemByText(StringResources.TreeEditorPopupMenu_MoveRightItemButtonText);
			item.Enabled = CanIncreaseIndent();
		}
		protected virtual void AddChild(object parent, object child) { }
		protected virtual bool CanDecreaseIndent() {
			return GetParentNode() != null;
		}
		protected virtual bool CanIncreaseIndent() {
			return fItemsTreeView.SelectedNode != null && fItemsTreeView.SelectedNode.PrevNode != null;
		}
		protected virtual bool CanDragNode(TreeView treeView, TreeNode draggedNode, TreeNode dragOverNode) {			
			return (draggedNode != null && dragOverNode != draggedNode && !IsNodeIsChildOfParent(dragOverNode, draggedNode));
		}
		protected virtual void CollapseNodesInTreeView(TreeView treeView) { }
		protected virtual TreeNode CreateTreeNode(object item) {
			TreeNode node = new TreeNode(GetItemText(item));
			node.Tag = item;
			return node;
		}
		protected virtual DragNodeState GetDragNodeState(TreeView dragOverTreeView, Point point, TreeNode draggedNode, TreeNode dragOverNode) {
			TreeViewHitTestLocations hitTestInfo = dragOverTreeView.HitTest(point).Location;
			DragNodeState ret = DragNodeState.None;
			if (CanDragNode(dragOverTreeView, draggedNode, dragOverNode)) {
				if (hitTestInfo == TreeViewHitTestLocations.Label)
					ret = DragNodeState.AddChild;
				else
					if ((hitTestInfo == TreeViewHitTestLocations.Indent) || 
						(hitTestInfo == TreeViewHitTestLocations.PlusMinus))
						ret = DragNodeState.Insert;
					else
						if (hitTestInfo == TreeViewHitTestLocations.None) {
							if (dragOverTreeView.Nodes.Count != 0)
								ret = DragNodeState.AddToEnd;
							else
								ret = DragNodeState.AddRoot;
						}
			}
			else
				ret = DragNodeState.None;
			return ret;
		}
		protected virtual string GetItemText(object item) {
			return item.ToString();
		}
		protected virtual void DoDragDropTreeView(TreeNode draggedNode, TreeNode overNode, TreeView dropTreeView, DragNodeState dragNodeState) {
			if (dragNodeState == DragNodeState.AddChild)
				ReplaceToNewParent(draggedNode, overNode);
			else
				if (dragNodeState == DragNodeState.Insert)
					ReplaceInLevel(draggedNode, overNode);
				else
					if (dragNodeState == DragNodeState.AddToEnd)
						ReplaceToEnd(draggedNode, dropTreeView.Nodes[dropTreeView.Nodes.Count - 1]);
		}
		protected virtual void DoGiveFeedbackTreeView(DragNodeState dragNodeState) {
			if (dragNodeState == DragNodeState.AddChild)
				Cursor.Current = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(AddChildCursorResource));
			else
				if (dragNodeState == DragNodeState.Insert)
					Cursor.Current = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(InsertCursorResource)); 
				else
					if ((dragNodeState == DragNodeState.AddToEnd) || (dragNodeState == DragNodeState.AddRoot))
						Cursor.Current = new Cursor(Assembly.GetExecutingAssembly().GetManifestResourceStream(AddCursorResource));  
		}
		protected virtual void DecreaseIndent(object newParent, object item) { }
		protected virtual void IncreaseIndent(object newParent, object item) { }
		protected virtual void InsertItem(object parent, object insertingItem, object currentItem) { }
		protected virtual bool IsValidData(TreeView treeView) { return true; }
		protected virtual TreeNodeCollection GetRootTreeNodes(TreeView treeView) {
			return treeView.Nodes;
		}
		protected virtual void MoveUpItem(object parent, object item) { }
		protected virtual void MoveDownItem(object parent, object item) { }
		protected virtual void RemoveItem(object parent, object item) { }
		protected virtual void ReplaceItemInLevel(object draggedObj, object overObj) { }
		protected virtual void ReplaceItemToEnd(object item, object lastDestObj) { }
		protected virtual void ReplaceItemToNewParent(object newParent, object item) { }
		protected virtual void AddRootNode(TreeNode draggedNode) {
			AddRootTreeNode(draggedNode, fItemsTreeView);
		}
		protected virtual void ReplaceInLevel(TreeNode draggedNode, TreeNode overNode) {
			ReplaceItemInLevel(draggedNode.Tag, overNode.Tag);
			ReplaceTreeNodeInLevel(draggedNode, overNode);
		}
		protected virtual void ReplaceToEnd(TreeNode draggedNode, TreeNode lastDestNode) {
			ReplaceItemToEnd(draggedNode.Tag, lastDestNode.Tag);
			ReplaceTreeNodeToEnd(draggedNode, lastDestNode);
		}
		protected virtual void ReplaceToNewParent(TreeNode draggedNode, TreeNode overNode) {
			ReplaceItemToNewParent(overNode.Tag, draggedNode.Tag);
			ReplaceTreeNodeToNewParent(draggedNode, overNode);
		}
		protected void DecreaseIndentViewerItem() {
			TreeNode selectedNode = fItemsTreeView.SelectedNode;
			TreeNodeCollection nodes = selectedNode.Parent.Parent != null ? selectedNode.Parent.Parent.Nodes :
				fItemsTreeView.Nodes;
			int index = nodes.IndexOf(selectedNode.Parent);
			selectedNode.Remove();
			nodes.Insert(index + 1, selectedNode);
			fItemsTreeView.SelectedNode = selectedNode;
		}
		protected void IncreaseIndentViewerItem() {
			TreeNode selectedNode = fItemsTreeView.SelectedNode;
			TreeNode prevSibling = selectedNode.PrevNode;
			selectedNode.Remove();
			prevSibling.Nodes.Add(selectedNode);
			fItemsTreeView.SelectedNode = selectedNode;
		}
		protected void FillTreeView(TreeView treeView, IHierarchicalEnumerable tree) {
			if (IsValidData(treeView)) {
				TreeNode treeNode = null;
				treeView.BeginUpdate();
				BeginUpdateAppearance();
				treeView.Nodes.Clear();
				foreach (IHierarchyData node in tree) {
					treeNode = CreateTreeNode(node);
					treeView.Nodes.Add(treeNode);
					AddChilds(treeNode, node);
				}
				if (treeView.Nodes.Count != 0) {
					treeView.SelectedNode = treeView.Nodes[0];
				}
				CollapseNodesInTreeView(treeView);
				EndUpdateAppearance();
				treeView.EndUpdate();
			}
		}
		protected IList GetTreeViewNodesInSameLevel(TreeNode node) {
			return node.Parent != null ? node.Parent.Nodes :
				node.TreeView.Nodes;
		}
		protected void OnAfterSelect(object sender, TreeViewEventArgs e) {
			PropertyGrid.SelectedObject = e.Node.Tag;
			fSelectedObject = e.Node.Tag;
			UpdateTools();
		}
		protected void OnDragDropTreeView(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(typeof(TreeNode))) {
				TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
				TreeNode overNode = null;
				if (sender is TreeView) {
					TreeView dropTreeView = sender as TreeView;
					overNode = dropTreeView.GetNodeAt(dropTreeView.PointToClient(new Point(e.X, e.Y)));
					DoDragDropTreeView(draggedNode, overNode, dropTreeView, fDragNodeState);
					ComponentChanged(false, true);
				}
			}
			fDragNodeState = DragNodeState.None;
		}
		protected void OnItemDragTreeView(object sender, ItemDragEventArgs e) {
			if (sender is TreeView) {
				TreeView treeView = sender as TreeView;
				if (treeView.SelectedNode != null) {
					treeView.SelectedNode = e.Item as TreeNode;
					treeView.DoDragDrop(treeView.SelectedNode, DragDropEffects.Move);
				}
			}
		}
		protected void OnDragOverTreeView(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.None;
			if (sender is TreeView) {
				TreeView dragOverTreeView = sender as TreeView;
				Point point = dragOverTreeView.PointToClient(new Point(e.X, e.Y));
				TreeNode dragOverNode = dragOverTreeView.GetNodeAt(point);
				TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
				ScrollTreeViewContent(dragOverTreeView, dragOverTreeView.PointToClient(new Point(e.X, e.Y)));
				fDragNodeState = GetDragNodeState(dragOverTreeView, point, draggedNode, dragOverNode);
				TreeViewEx dragOverTreeViewEx = dragOverTreeView as TreeViewEx;
				if (fDragNodeState != DragNodeState.None) {
					e.Effect = DragDropEffects.Move;
					if (dragOverTreeViewEx != null)
						dragOverTreeViewEx.DragHighlighting = true;
				}
				else
					if (dragOverTreeViewEx != null)
						dragOverTreeViewEx.DragHighlighting = false;
			}
		}
		protected void OnGiveFeedbackTreeView(object sender, GiveFeedbackEventArgs e) {
			if (fDragNodeState != DragNodeState.None) {
				e.UseDefaultCursors = false;
				DoGiveFeedbackTreeView(fDragNodeState);
			}
		}
		protected void BeginUpdateAppearance() {
			fCurrentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
		}
		protected void EndUpdateAppearance() {
			Cursor.Current = fCurrentCursor;
		}
		private void AddNewNode(TreeNodeCollection nodes, TreeNode node) {
			nodes.Add(node);
		}
		private void InsertNode(int index, TreeNodeCollection nodes, TreeNode node) {
			nodes.Insert(index, node);
		}
		private object GetParentItem() {
			TreeNode node = fItemsTreeView.SelectedNode;
			if (node != null && node.Parent != null)
				return node.Parent.Tag;
			return null;
		}
		private TreeNode GetParentNode() {
			TreeNode node = fItemsTreeView.SelectedNode;
			if (node != null && node.Parent != null)
				return node.Parent;
			return null;
		}
		private void OnAddChild(object sender, EventArgs e) {
			object newItem = CreateNewItem();
			AddChild(fItemsTreeView.SelectedNode.Tag, newItem);
			TreeNode node = CreateTreeNode(newItem);
			AddNewNode(fItemsTreeView.SelectedNode.Nodes, node);
			fItemsTreeView.SelectedNode = node;
			fItemsTreeView.Update();
			ComponentChanged(false, true);
		}
		private void OnDecreaseIndent(object sender, EventArgs e) {
			TreeNode parentNode = GetParentNode();
			DecreaseIndent(GetParentItem(), fItemsTreeView.SelectedNode.Tag);
			DecreaseIndentViewerItem();
			ComponentChanged(false, true);
		}
		private void OnIncreaseIndent(object sender, EventArgs e) {
			TreeNode newParentNode = fItemsTreeView.SelectedNode.PrevNode;
			IncreaseIndent(newParentNode.Tag, fItemsTreeView.SelectedNode.Tag);
			IncreaseIndentViewerItem();
			ComponentChanged(false, true);
		}
		private void OnTreeNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
			TreeView treeView = sender as TreeView;
			if (e.Button == MouseButtons.Right) {
				if (treeView != null) {
					TreeNode clickedTreeNode = treeView.GetNodeAt(new Point(e.X, e.Y));
					if (clickedTreeNode != null)
						treeView.SelectedNode = clickedTreeNode;
				}
			}
		}
		private void AddRootTreeNode(TreeNode draggedNode, TreeView destenTreeView) {
			draggedNode.Remove();
			destenTreeView.Nodes.Add(draggedNode);
			destenTreeView.SelectedNode = draggedNode;
		}
		private void ReplaceTreeNodeInLevel(TreeNode draggedNode, TreeNode overNode) {
			IList newNodes = GetTreeViewNodesInSameLevel(draggedNode);
			IList oldNodes = GetTreeViewNodesInSameLevel(overNode);
			int index = oldNodes.IndexOf(overNode);
			int dragItemIndex = oldNodes.IndexOf(draggedNode);
			draggedNode.Remove();
			if (newNodes == oldNodes) {
				if ((index > dragItemIndex) && (newNodes == oldNodes))
					index--;
			}
			oldNodes.Insert(index, draggedNode);
			draggedNode.TreeView.SelectedNode = draggedNode;
		}
		private void ReplaceTreeNodeToEnd(TreeNode draggedNode, TreeNode lastDestNode) {
			if (draggedNode != lastDestNode) {
				draggedNode.Remove();
				GetRootTreeNodes(lastDestNode.TreeView).Add(draggedNode);
				lastDestNode.TreeView.SelectedNode = draggedNode;
				lastDestNode.TreeView.Update();
			}
		}
		private void ReplaceTreeNodeToNewParent(TreeNode draggedNode, TreeNode overNode) {
			draggedNode.Remove();
			if (overNode != null)
				overNode.Nodes.Add(draggedNode);
			overNode.TreeView.SelectedNode = draggedNode;
		}
		private void ScrollTreeViewContent(TreeView treeView, Point currentPoint) {
			int d = treeView.Height - currentPoint.Y;
			if ((d < treeView.Height / 2) && (d > 0)) {
				TreeNode dragOverNode = treeView.GetNodeAt(currentPoint.X, currentPoint.Y);
				if ((dragOverNode != null) && (dragOverNode.NextVisibleNode != null))
					dragOverNode.NextVisibleNode.EnsureVisible();
			}
			if ((d > treeView.Height / 2) && (d < treeView.Height)) {
				TreeNode dragOverNode = treeView.GetNodeAt(currentPoint.X, currentPoint.Y);
				if ((dragOverNode != null) && (dragOverNode.PrevVisibleNode != null)) {
					dragOverNode.PrevVisibleNode.EnsureVisible();
				}
			}
		}
		private void UpdateNodeText(TreeNode node) {
			node.Text = node.Tag.ToString();
			foreach (TreeNode childNode in node.Nodes) {
				UpdateNodeText(childNode);
			}
		}
		private void AddChilds(TreeNode treeNode, IHierarchyData node) {
			TreeNode childNode = null;
			foreach (IHierarchyData item in node.GetChildren()) {
				childNode = CreateTreeNode(item);
				if (childNode != null) {
					treeNode.Nodes.Add(childNode);
					AddChilds(childNode, item);
				}
			}
		}
		private bool IsNodeIsChildOfParent(TreeNode childNode, TreeNode parentNode) {
			if (childNode != null) {
				TreeNode currentParentNode = childNode.Parent;
				while (currentParentNode != null) {
					if (currentParentNode == parentNode)
						return true;
					currentParentNode = currentParentNode.Parent;
				}
				return false;
			}
			else
				return false;
		}
	}
}
