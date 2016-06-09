#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.NodesTree {
	public class ModelTreeList : ObjectTreeList {
		private int ShowViewByKeyNavigationDelay = 50;
		private TreeListNode oldNode;
		private bool navigationByKeyOn;
		private Timer showViewByKeyNavigationTimer;
		private bool raiseFocusedNodeChanged = true;
		private int lockPaint = 0;
		private bool lockFocusedNode = false;
		private bool checkBoxesMode;
		private TreeListColumn displayColumn = null;
		private int lockRefreshChildNodes = 0;
		protected class FindModelNodeByObjectOperation : FindNodeByObjectOperation {
			ModelTreeListNode targetNode = null;
			public FindModelNodeByObjectOperation(ModelTreeListNode tagValue)
				: base(tagValue) {
				targetNode = tagValue;
			}
			public override void Execute(TreeListNode node) {
				ModelTreeListNode findNode = (ModelTreeListNode)((ObjectTreeListNode)node).Object;
				if(tagValue != null && findNode.ModelNode != null && targetNode.ModelNode != null && findNode.ModelNode.GetType() == targetNode.ModelNode.GetType() && findNode.ModelNode.Id == targetNode.ModelNode.Id) {
					if(IsParentEquals(findNode, targetNode)) {
						this.node = node;
					}
				}
			}
			private bool IsParentEquals(ModelTreeListNode findNode, ModelTreeListNode targetNode) {
				bool result = false;
				if(findNode.Parent != null && targetNode.Parent != null) {
					if((findNode.Parent.ModelNode.Id == targetNode.Parent.ModelNode.Id) || (targetNode.Parent.ModelNode is IModelApplication && targetNode.Parent.ModelNode is IModelApplication)) {
						result = IsParentEquals(findNode.Parent, targetNode.Parent);
					}
				}
				else {
					if(findNode.Parent == null && targetNode.Parent == null) {
						result = true;
					}
				}
				return result;
			}
		}
		public ModelTreeList()
			: this(new ExtendModelInterfaceAdapter()) {
		}
		public ModelTreeList(ExtendModelInterfaceAdapter adapter)
			: base(adapter) {
			this.Tag = "TESTTABLE=ModelTreeList";
			this.Name = "ModelTreeList";
			CreateShowViewByKeyNavigationTimer();
			navigationByKeyOn = false;
			BuildChildNodesRecursive = false;
			OptionsView.ShowIndicator = false;
			OptionsView.ShowHorzLines = false;
			OptionsView.ShowVertLines = false;
			OptionsView.ShowButtons = true;
			OptionsView.FocusRectStyle = DrawFocusRectStyle.CellFocus;
			OptionsView.ShowColumns = true;
			OptionsView.ShowRoot = true;
			OptionsView.AutoWidth = true;
			OptionsBehavior.Editable = false;
			OptionsDragAndDrop.DragNodesMode = XtraTreeList.DragNodesMode.Single;
			OptionsDragAndDrop.CanCloneNodesOnDrop = true;
			OptionsDragAndDrop.ExpandNodeOnDrag = true;
			OptionsSelection.MultiSelect = true;
			OptionsBehavior.KeepSelectedOnClick = true;
			OptionsBehavior.AllowIncrementalSearch = true;
			OptionsBehavior.ExpandNodesOnIncrementalSearch = true;
			OptionsSelection.EnableAppearanceFocusedRow = true;
			OptionsSelection.EnableAppearanceFocusedCell = false;
			CustomDrawNodeCell += new CustomDrawNodeCellEventHandler(ModelTreeList_CustomDrawNodeCell);
			adapter.Changed += new EventHandler<ModelTreeListNodeChangedEventArgs>(Adapter_NodeChanged);
			BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(modelTreeList_BeforeCheckNode);
			AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(modelTreeList_AfterCheckNode);
			AfterExpand += new DevExpress.XtraTreeList.NodeEventHandler(modelTreeList_AfterExpand);
		}
		private void ModelTreeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
			ModelTreeListNode modelNode = ((ObjectTreeListNode)e.Node).Object as ModelTreeListNode;
			if(modelNode != null && modelNode.HasModification) {
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
			}
			else {
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Regular);
			}
		}
		private void focusedNodeChangedOnKeyUpTimer_Tick(object sender, EventArgs e) {
			raiseFocusedNodeChanged = true;
			RaiseFocusedNodeChanged(oldNode, FocusedNode);
		}
		private void CreateShowViewByKeyNavigationTimer() {
			showViewByKeyNavigationTimer = new Timer();
			showViewByKeyNavigationTimer.Interval = ShowViewByKeyNavigationDelay;
			showViewByKeyNavigationTimer.Tick += new EventHandler(focusedNodeChangedOnKeyUpTimer_Tick);
		}
		private void StopShowViewByKeyNavigationTimer() {
			if(showViewByKeyNavigationTimer != null) {
				showViewByKeyNavigationTimer.Stop();
				showViewByKeyNavigationTimer.Enabled = false;
			}
		}
		private void StartShowViewByKeyNavigationTimer() {
			showViewByKeyNavigationTimer.Enabled = true;
			showViewByKeyNavigationTimer.Start();
		}
		private void ReleaseShowViewByKeyNavigationTimer() {
			StopShowViewByKeyNavigationTimer();
			showViewByKeyNavigationTimer.Tick -= new EventHandler(focusedNodeChangedOnKeyUpTimer_Tick);
		}
		private void CollectCheckedNodes(TreeListNode node, List<ModelTreeListNode> selectedNodes) {
			ObjectTreeListNode objectTreeListNode = (ObjectTreeListNode)node;
			ModelTreeListNode modelNode = ((ModelTreeListNode)objectTreeListNode.Object);
			if((node.CheckState == CheckState.Checked && modelNode.ModelTreeListNodeType == ModelTreeListNodeType.Group) || node.CheckState == CheckState.Indeterminate) {
				if(modelNode.ModelTreeListNodeType == ModelTreeListNodeType.Group && node.Nodes.Count == 0) {
					selectedNodes.Add(modelNode);
				}
				else {
					foreach(TreeListNode childNode in node.Nodes) {
						CollectCheckedNodes(childNode, selectedNodes);
					}
				}
			}
			else if(node.CheckState == CheckState.Checked) {
				selectedNodes.Add(modelNode);
			}
		}
		internal int LockRefreshChildNodes {
			get {
				return lockRefreshChildNodes;
			}
			set {
				lockRefreshChildNodes = value;
			}
		}
		internal void RefreshChildNodes(ModelTreeListNode modelNode) {
			if(lockRefreshChildNodes == 0) {
				try {
					BeginUpdate();
					ObjectTreeListNode node = FindBuiltAncestorNode(modelNode);
					if(node == null) { return; }
					Dictionary<Object, ObjectTreeListNode> list = new Dictionary<object, ObjectTreeListNode>();
					foreach(ObjectTreeListNode child in node.Nodes) {
						list.Add(child.Object, child);
					}
					List<object> allChilds = new List<object>();
					foreach(object obj in nodeObjectAdapter.GetChildren(modelNode)) {
						allChilds.Add(obj);
						if(!list.ContainsKey(obj)) {
							BuildControlNode(obj, node);
						}
					}
					ObjectTreeListNode delayedDeletedObject = null;
					foreach(ObjectTreeListNode objNode in list.Values) {
						if(!allChilds.Contains(objNode.Object)) {
							if(FocusedObject != objNode.Object) {
								DeleteNode(objNode);
							}
							else {
								delayedDeletedObject = objNode;
							}
						}
					}
					if(delayedDeletedObject != null) {
						DeleteNode(delayedDeletedObject);
					}
					node.IsChildrenPopulated = true;
					RefreshChildrenOrder(node);
				}
				finally {
					EndUpdate();
				}
			}
		}
		private TreeListColumn DisplayColumn {
			get {
				if(displayColumn == null) {
					foreach(TreeListColumn column in Columns) {
						if(column.FieldName == nodeObjectAdapter.DisplayPropertyName) {
							displayColumn = column;
							break;
						}
					}
				}
				return displayColumn;
			}
		}
		private void Adapter_NodeChanged(object sender, ModelTreeListNodeChangedEventArgs e) {
			switch(e.NodeChangedType) {
				case NodeChangedType.CollectionChanged: {
						RefreshChildNodes(e.Node);
						ObjectTreeListNode objectTreeListNode = FindBuiltAncestorNode(e.Node);
						if(objectTreeListNode != null) {
							if(objectTreeListNode.CheckState == CheckState.Checked) {
								CheckNode(objectTreeListNode, true);
							}
							else {
								CheckNode(objectTreeListNode, false);
							}
							UpdateCheckState(objectTreeListNode);
						}
						break;
					}
				case NodeChangedType.ObjectChanged: {
						RefreshObject(e.Node);
						break;
					}
				case NodeChangedType.GroupChanged: {
						ObjectTreeListNode objectTreeListNode = FindBuiltAncestorNode(e.Node);
						FocusedNode = objectTreeListNode;
						break;
					}
				case NodeChangedType.DeleteNode: {
						raiseFocusedNodeChanged = true;
						break;
					}
			}
		}
		private void modelTreeList_AfterExpand(object sender, NodeEventArgs e) {
			if(CheckBoxesMode && e.Node.CheckState == CheckState.Checked) {
				CheckNode(e.Node, true);
			}
			UpdateCheckState(e.Node);
		}
		private void modelTreeList_BeforeCheckNode(object sender, CheckNodeEventArgs e) {
			if(!isUpdateCheckBoxes) {
				e.State = (e.PrevState == CheckState.Unchecked ? CheckState.Checked : CheckState.Unchecked);
			}
		}
		private void CheckNode(TreeListNode node, bool isCheck) {
			if(OptionsView.ShowCheckBoxes) {
				node.CheckState = isCheck ? CheckState.Checked : CheckState.Unchecked;
				if(!((ObjectTreeListNode)node).IsChildrenPopulated) {
					Adapter.GetChildren(((ObjectTreeListNode)node).Object);
				}
				foreach(TreeListNode childNode in node.Nodes) {
					CheckNode(childNode, isCheck);
				}
			}
		}
		private void UpdateCheckState(TreeListNode node) {
			if(OptionsView.ShowCheckBoxes && node != null) {
				bool hasCheckedChildNodes = false;
				bool hasUncheckedChildNodes = false;
				foreach(TreeListNode childNode in node.Nodes) {
					if(childNode.CheckState != CheckState.Unchecked) {
						hasCheckedChildNodes = true;
					}
					else {
						hasUncheckedChildNodes = true;
					}
				}
				if(hasCheckedChildNodes) {
					node.CheckState = hasUncheckedChildNodes ? CheckState.Indeterminate : CheckState.Checked;
				}
				else if(hasUncheckedChildNodes) {
					node.CheckState = CheckState.Unchecked;
				}
				UpdateCheckState(node.ParentNode);
			}
		}
		private bool isUpdateCheckBoxes;
		private void modelTreeList_AfterCheckNode(object sender, NodeEventArgs e) {
			if(!isUpdateCheckBoxes) {
				isUpdateCheckBoxes = true;
				CheckNode(e.Node, e.Node.Checked);
				UpdateCheckState(e.Node.ParentNode);
				isUpdateCheckBoxes = false;
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(nodeObjectAdapter != null) {
				if(Adapter != null) {
					Adapter.Changed -= new EventHandler<ModelTreeListNodeChangedEventArgs>(Adapter_NodeChanged);
					nodeObjectAdapter = null;
				}
			}
			CustomDrawNodeCell -= new CustomDrawNodeCellEventHandler(ModelTreeList_CustomDrawNodeCell);
			BeforeCheckNode -= new DevExpress.XtraTreeList.CheckNodeEventHandler(modelTreeList_BeforeCheckNode);
			AfterCheckNode -= new DevExpress.XtraTreeList.NodeEventHandler(modelTreeList_AfterCheckNode);
			AfterExpand -= new DevExpress.XtraTreeList.NodeEventHandler(modelTreeList_AfterExpand);
			if(showViewByKeyNavigationTimer != null) {
				ReleaseShowViewByKeyNavigationTimer();
				showViewByKeyNavigationTimer.Dispose();
				showViewByKeyNavigationTimer = null;
			}
		}
		protected override ObjectTreeListNode FindControlNode(object nodeObject, ObjectTreeList.SearchTreeListNodeMode mode) {
			FindModelNodeByObjectOperation findOperation = new FindModelNodeByObjectOperation((ModelTreeListNode)nodeObject);
			NodesIterator.DoOperation(findOperation);
			return (ObjectTreeListNode)findOperation.Node;
		}
		protected override void ColumnCreated(TreeListColumn column) {
			base.ColumnCreated(column);
			column.MinWidth = 200; 
			column.SortOrder = SortOrder.None;
			column.OptionsColumn.AllowFocus = true;
		}
		protected override void RaiseFocusedNodeChanged(TreeListNode oldNode, TreeListNode newNode) {
			this.oldNode = oldNode;
			if(navigationByKeyOn) {
				StopShowViewByKeyNavigationTimer();
				StartShowViewByKeyNavigationTimer();
			}
			if(raiseFocusedNodeChanged || this.State == TreeListState.IncrementalSearch) {
				StopShowViewByKeyNavigationTimer();
				raiseFocusedNodeChanged = false;
				base.RaiseFocusedNodeChanged(oldNode, newNode);
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(!e.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)) {
				navigationByKeyOn = true;
				if(State == TreeListState.Regular && FocusedNode != null) {
					if(e.KeyCode == Keys.Right && FocusedNode.HasChildren) {
						if(!FocusedNode.Expanded) {
							FocusedNode.Expanded = true;
						}
						else {
							FocusedNode = (ObjectTreeListNode)FocusedNode.Nodes[0];
						}
						e.Handled = true;
					}
					if(e.KeyCode == Keys.Left) {
						if(FocusedNode.Expanded) {
							FocusedNode.Expanded = false;
							e.Handled = true;
						}
						else if(FocusedNode.ParentNode != null) {
							FocusedNode = (ObjectTreeListNode)FocusedNode.ParentNode;
							e.Handled = true;
						}
					}
				}
			}
			if(e.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)) {
				if(NodeIndexChanged != null) {
					NodeIndexChanged(this, new NodeIndexChangedEventArgs(e.KeyCode == Keys.Up));
				}
			}
			else {
				base.OnKeyDown(e);
			}
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space) {
				raiseFocusedNodeChanged = true;
				RaiseFocusedNodeChanged(oldNode, FocusedNode);
			}
			else
				if(!e.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)) {
					StopShowViewByKeyNavigationTimer();
					StartShowViewByKeyNavigationTimer();
				}
			navigationByKeyOn = false;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right && State == TreeListState.Regular) {
				TreeListHitInfo info = CalcHitInfo(e.Location);
				if(info.HitInfoType == HitInfoType.Cell ||
					info.HitInfoType == HitInfoType.RowIndicator ||
					info.HitInfoType == HitInfoType.StateImage) {
					raiseFocusedNodeChanged = true;
					bool canFocus = true;
					if(FocusedNode != info.Node) {
						RaiseBeforeFocusNode(FocusedNode, info.Node, ref canFocus);
					}
					if(canFocus) {
						FocusedNode = (ObjectTreeListNode)info.Node;
					}
				}
			}
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left && !lockFocusedNode) {
				TreeListHitInfo li = CalcHitInfo(e.Location);
				if((li.HitInfoType == HitInfoType.Row || li.HitInfoType == HitInfoType.RowIndent ||
					li.HitInfoType == HitInfoType.RowIndicator || li.HitInfoType == HitInfoType.RowPreview ||
					li.HitInfoType == HitInfoType.Cell || li.HitInfoType == HitInfoType.StateImage ||
					li.HitInfoType == HitInfoType.SelectImage || li.HitInfoType == HitInfoType.RowIndicatorEdge) ||
					(li.HitInfoType == HitInfoType.Button && (oldNode == null || li.Node.Id != oldNode.Id))) {
					bool canFocus = true;
					if(oldNode != li.Node) {
						RaiseBeforeFocusNode(oldNode, li.Node, ref canFocus);
						if(canFocus) {
							raiseFocusedNodeChanged = true;
							RaiseFocusedNodeChanged(oldNode, li.Node);
						}
					}
				}
			}
			lockFocusedNode = false;
		}
		protected override void RaiseBeforeFocusNode(TreeListNode oldNode, TreeListNode node, ref bool canFocus) {
			base.RaiseBeforeFocusNode(oldNode, node, ref canFocus);
			lockFocusedNode = !canFocus;
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(lockPaint == 0) {
				lockPaint++;
				try {
					base.OnPaint(e);
				}
				finally {
					lockPaint--;
				}
			}
		}
		protected override ObjectTreeListNode BuildControlNode(object nodeObject, ObjectTreeListNode parentNode) {
			ObjectTreeListNode controlNode = (ObjectTreeListNode)AppendNode(null, parentNode, nodeObject);
			controlNode.HasChildren = nodeObjectAdapter.HasChildren(nodeObject);
			OnControlNodeBuilt(controlNode);
			return controlNode;
		}
		internal void RefreshNodesDisplayValue(ModelTreeListNode rootNode) {
			try {
				BeginUpdate();
				RefreshDisplayValue(rootNode);
			}
			finally {
				EndUpdate();
			}
		}
		internal ExtendModelInterfaceAdapter Adapter {
			get {
				return (ExtendModelInterfaceAdapter)nodeObjectAdapter;
			}
		}
		private void RefreshDisplayValue(ModelTreeListNode rootNode) {
			ObjectTreeListNode node = FindBuiltAncestorNode(rootNode);
			node.SetValue(DisplayColumn, Adapter.fastModelEditorHelper.GetModelNodeDisplayValue(rootNode.ModelNode));
			foreach(ObjectTreeListNode child in node.Nodes) {
				ModelTreeListNode childModelNode = (ModelTreeListNode)child.Object;
				child.SetValue(DisplayColumn, Adapter.fastModelEditorHelper.GetModelNodeDisplayValue(childModelNode.ModelNode));
				RefreshChildDisplayValue(child);
			}
		}
		private void RefreshChildDisplayValue(TreeListNode node) {
			foreach(ObjectTreeListNode child in node.Nodes) {
				ModelTreeListNode childModelNode = (ModelTreeListNode)child.Object;
				child.SetValue(DisplayColumn, Adapter.fastModelEditorHelper.GetModelNodeDisplayValue(childModelNode.ModelNode));
				RefreshChildDisplayValue(child);
			}
		}
		public void RefreshChildrenOrder(ObjectTreeListNode controlNode) {
			if(controlNode != null) {
				int counter = 0;
				BeginUpdate();
				try {
					foreach(ModelTreeListNode node in nodeObjectAdapter.GetChildren(controlNode.Object)) {
						ObjectTreeListNode childTreeListNode = FindBuiltAncestorNode(node);
						if(childTreeListNode != null &&
							childTreeListNode.Object.GetType() == node.GetType()) {
							SetNodeIndex(childTreeListNode, counter);
						}
						counter++;
					}
				}
				finally {
					EndUpdate();
				}
			}
		}
		new public object FocusedObject {
			get { return base.FocusedObject; }
			set {
				raiseFocusedNodeChanged = true;
				base.FocusedObject = value;
				oldNode = this.FocusedNode;
			}
		}
		public bool CheckBoxesMode {
			get {
				return checkBoxesMode;
			}
			set {
				checkBoxesMode = value;
				this.OptionsView.ShowCheckBoxes = checkBoxesMode;
				this.OptionsBehavior.AllowIndeterminateCheckState = checkBoxesMode;
			}
		}
		public IEnumerable<ModelTreeListNode> GetCheckedNodes() {
			List<ModelTreeListNode> result = new List<ModelTreeListNode>();
			foreach(TreeListNode childNode in Nodes) {
				CollectCheckedNodes(childNode, result);
			}
			return result;
		}
#if DebugTest
		public void DoCheckNode(TreeListNode node, CheckState checkState) {
			if(node == null) return;
			CheckNodeEventArgs e = RaiseBeforeCheckNode(node, node.CheckState, checkState);
			if(!e.CanCheck) return;
			node.CheckState = e.State;
			RaiseAfterCheckNode(node);
		}
		public ExtendModelInterfaceAdapter DebugTest_Adapter {
			get { return Adapter; }
		}
#endif
		internal event EventHandler<NodeIndexChangedEventArgs> NodeIndexChanged;
	}
	internal class NodeIndexChangedEventArgs : EventArgs {
		private bool isIndexUp;
		public NodeIndexChangedEventArgs(bool isIndexUp) {
			this.isIndexUp = isIndexUp;
		}
		public bool IsIndexUp {
			get { return isIndexUp; }
		}
	}
}
