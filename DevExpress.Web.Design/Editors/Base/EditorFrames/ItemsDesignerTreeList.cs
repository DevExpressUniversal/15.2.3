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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Handler;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.ViewInfo;
namespace DevExpress.Web.Design {
	public class ItemsDesignerTreeList : TreeList, IUpdatableViewControl {
		Pen DrawSelectionPen = new Pen(Color.FromArgb(51, 153, 255), 1);
		SolidBrush DrawSelectionBrush = new SolidBrush(Color.FromArgb(50, 170, 204, 238));
		Point DragSelectionStart = Point.Empty;
		Rectangle DragSelectionRect = Rectangle.Empty;
		DXPopupMenu itemsContextMenu;
		TreeListNode newFocusedNode = null;
		bool shouldRaiseFocusedNodeChanged = false;
		public ItemsDesignerTreeList(ItemsEditorOwner dataOwner)
			: base() {
			BeginUpdate();
			ItemsOwner = dataOwner;
			OptionsSelection.MultiSelect = true;
			OptionsSelection.EnableAppearanceFocusedRow = true;
			OptionsSelection.EnableAppearanceFocusedCell = false;
			KeyFieldName = "ID";
			ParentFieldName = "ParentID";
			var captionColumn = Columns.AddField("Caption");
			captionColumn.Visible = true;
			captionColumn.OptionsColumn.AllowSort = false;
			captionColumn.OptionsColumn.AllowEdit = false;
			captionColumn.OptionsColumn.ReadOnly = true;
			OptionsMenu.EnableColumnMenu = true;
			RootValue = -1;
			OptionsBehavior.AllowIncrementalSearch = true;
			OptionsDragAndDrop.DragNodesMode = XtraTreeList.DragNodesMode.Single;
			OptionsDragAndDrop.CanCloneNodesOnDrop = false;
			OptionsView.ShowRoot = true;
			DataSource = ItemsOwner.TreeListItems;
			ItemsOwner.OnBeginDataUpdate = BeginDataOwnerUpdate;
			CustomDrawNodeCell += ColumnsDesignerTreeList_CustomDrawNodeCell;
			GetStateImage += ColumnsDesignerTreeList_GetStateImage;
			BeforeDragNode += ColumnsDesignerTreeList_BeforeDragNode;
			NodesReloaded += ColumnsDesignerTreeList_NodesReloaded;
			ToolTipController = new ItemToolTip(this);
			EndUpdate();
		}
		public void SetRowFont(Font font) {
			Appearance.Row.Font = font;
			RowHeight = Appearance.Row.Font.Height + SystemInformation.Border3DSize.Height * 2;
		}
		void ColumnsDesignerTreeList_NodesReloaded(object sender, EventArgs e) {
			RestoreFocusedNode(true);
		}
		protected override void Dispose(bool disposing) {
			shouldRaiseFocusedNodeChanged = false;
			shouldRaiseSelectionChanged = false;
			base.Dispose(disposing);
		}
		public ItemsEditorOwner ItemsOwner { get; private set; }
		public DXPopupMenu ItemsContextMenu {
			get {
				if(itemsContextMenu == null)
					itemsContextMenu = new DXPopupMenu();
				return itemsContextMenu;
			}
		}
		public bool IsDragSelection { get; set; }
		protected override TreeListViewInfo CreateViewInfo() {
			return new ItemsDesignerTreeListViewInfo(this);
		}
		protected override void RaiseFocusedNodeChanged(TreeListNode oldNode, TreeListNode newNode) {
			if(IsLockUpdate) {
				newFocusedNode = newNode;
				shouldRaiseFocusedNodeChanged = true;
				return;
			}
			if(oldNode == null)
				newNode = FindNodeByID(ItemsOwner.FocusedNodeID);
			else
				ItemsOwner.FocusedNodeID = GetDataItemID(newNode);
			base.RaiseFocusedNodeChanged(oldNode, newNode);
			shouldRaiseFocusedNodeChanged = false;
		}
		bool shouldRaiseSelectionChanged = false;
		protected override void RaiseSelectionChanged() {
			if(IsDragSelection || IsLockUpdate) {
				shouldRaiseSelectionChanged = true;
				return;
			}
			var selectedIDs = GetFilteredSelectedItems().Select(n => GetDataItemID(n)).ToList();
			ItemsOwner.SetSelection(selectedIDs);
			base.RaiseSelectionChanged();
			shouldRaiseSelectionChanged = false;
		}
		private IEnumerable<TreeListNode> GetFilteredSelectedItems() {
			var result = new HashSet<TreeListNode>();
			foreach(TreeListNode node in Selection) {
				TreeListNode selectedNode = null;
				var item = node;
				while(item != null) {
					if(item.Selected)
						selectedNode = item;
					item = item.ParentNode;
				}
				if(selectedNode != null)
					result.Add(selectedNode);
			}
			return result;
		}
		void ColumnsDesignerTreeList_GetStateImage(object sender, GetStateImageEventArgs e) {
			e.NodeImageIndex = GetDataItem(e.Node).ImageIndex;
		}
		protected override TreeListHandler CreateHandler() {
			return new ColumnsDesignerTreeListHandler(this);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(!IsDragSelection || DragSelectionRect.IsEmpty)
				return;
			e.Graphics.DrawRectangle(this.DrawSelectionPen, DragSelectionRect);
			e.Graphics.FillRectangle(this.DrawSelectionBrush, Rectangle.Inflate(DragSelectionRect, -1, -1));
		}
		protected override void OnLostFocus(EventArgs e) {
			EndDragSelection();
			base.OnLostFocus(e);
		}
		protected override void OnMouseWheelCore(MouseEventArgs e) {
			if(!IsDragSelection) {
				base.OnMouseWheelCore(e);
				return;
			}
			var rowHeight = ViewInfo.RowsInfo[TopVisibleNode].Bounds.Height;
			var nodeIndex = TopVisibleNodeIndex;
			base.OnMouseWheelCore(e);
			RefreshRowsInfo();
			DragSelectionStart.Y += rowHeight * (nodeIndex - TopVisibleNodeIndex);
			ProcessDragSelection(e.Location);
		}
		Point MouseDownPoint = Point.Empty;
		List<TreeListNode> SelectedNodesOnMouseDown = new List<TreeListNode>();
		Dictionary<TreeListNode, int> CaptionWidth = new Dictionary<TreeListNode, int>();
		void ColumnsDesignerTreeList_BeforeDragNode(object sender, BeforeDragNodeEventArgs e) {
			if(IsDragSelection) {
				e.CanDrag = false;
				return;
			}
			if(ModifierKeys != Keys.Control && Selection.Count > 1)
				return;
			var captionWidth = CaptionWidth[e.Node];
			var rowInfo = ViewInfo.RowsInfo[e.Node];
			var textStartX = rowInfo.StateImageBounds.Left;
			var textEndX = rowInfo.StateImageBounds.Right + captionWidth;
			if(MouseDownPoint.X >= textStartX && MouseDownPoint.X <= textEndX)
				return;
			if(ModifierKeys != Keys.Control && SelectedNodesOnMouseDown.Count > 0 && SelectedNodesOnMouseDown.Contains(e.Node))
				return;
			BeginDragSelection(MouseDownPoint);
			e.CanDrag = false;
		}
		void ColumnsDesignerTreeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e) {
			CaptionWidth[e.Node] = e.Appearance.CalcTextSize(e.Cache, e.CellText, e.Bounds.Width).ToSize().Width;
			var nodeItem = GetDataItem(e.Node);
			if(!nodeItem.Visible)
				e.Appearance.ForeColor = Color.Gray;
			if(e.Appearance.Font.Bold != nodeItem.Selected)
				e.Appearance.FontStyleDelta = FontStyle.Bold;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && (ModifierKeys == Keys.None || ModifierKeys == Keys.Control)) {
				if(CalcHitInfo(e.Location).Node == null) {
					if(ModifierKeys != Keys.Control)
						Selection.Clear();
					BeginDragSelection(e.Location);
				}
				SelectedNodesOnMouseDown.Clear();
				SelectedNodesOnMouseDown.AddRange(Selection.OfType<TreeListNode>());
				MouseDownPoint = e.Location;
			}
			base.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(e.Button == MouseButtons.Left && IsDragSelection)
				ProcessDragSelection(e.Location);
			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			SelectedNodesOnMouseDown.Clear();
			switch(e.Button) {
				case MouseButtons.Left:
					OnLeftMouseButtonUp(e.Location);
					break;
				case MouseButtons.Right:
					OnRightMouseButtonUp(e.Location);
					return;
			}
			base.OnMouseUp(e);
			Invalidate();
		}
		void OnLeftMouseButtonUp(Point cursorPosition) {
			if(IsDragSelection) {
				EndDragSelection();
			} else if(ModifierKeys == Keys.None) {
				var node = CalcHitInfo(cursorPosition).Node;
				if(node != null) {
					BeginUpdate();
					Selection.Clear();
					Selection.Add(node);
					EndUpdate();
				}
			}
		}
		void OnRightMouseButtonUp(Point cursorPosition) {
			TreeListHitInfo info = CalcHitInfo(cursorPosition);
			BeginUpdate();
			if(info.Node != null) {
				var selectionState = new List<TreeListNode>(Selection.OfType<TreeListNode>());
				Selection.Clear();
				FocusedNode = info.Node;
				FocusedNode.Selected = true;
				if(selectionState.Contains(info.Node)) {
					Selection.Clear();
					selectionState.ForEach(n => Selection.Add(n));
				}
			} else {
				FocusedNode = null;
				Selection.Clear();
			}
			EndUpdate();
			ShowContextMenu(false);
			Invalidate();
		}
		protected void BeginDragSelection(Point start) {
			IsDragSelection = true;
			DragSelectionStart = start;
		}
		protected void ProcessDragSelection(Point end) {
			DragSelectionRect = GetDragSelectionRect(DragSelectionStart, end);
			SelectDragSelectedNode();
			Invalidate();
		}
		protected void EndDragSelection() {
			DragSelectionRect = Rectangle.Empty;
			IsDragSelection = false;
			Invalidate();
			RaiseSelectionChanged();
		}
		Rectangle GetDragSelectionRect(Point start, Point end) {
			var x = Math.Min(start.X, end.X);
			var y = Math.Min(start.Y, end.Y);
			var width = Math.Abs(start.X - end.X);
			var height = Math.Abs(start.Y - end.Y);
			return new Rectangle(x, y, width, height);
		}
		void SelectDragSelectedNode() { 
			BeginUpdate();
			try {
				foreach(RowInfo row in ViewInfo.RowsInfo.Rows) {
					var xDiff = row.StateImageLocation.X - row.Bounds.X;
					var x = row.Bounds.X + xDiff;
					var y = row.Bounds.Y;
					var width = row.Bounds.Width - xDiff;
					var height = row.Bounds.Height;
					var rect = new Rectangle(x, y, width, height);
					var nodeIntersected = DragSelectionRect.IntersectsWith(rect);
					if(ModifierKeys == Keys.Control && SelectedNodesOnMouseDown.Contains(row.Node))
						row.Node.Selected = !nodeIntersected;
					else
						row.Node.Selected = nodeIntersected;
				}
			} finally {
				EndUpdate();
			}
		}
		public void ShowContextMenu(bool fromKeyboard) {
			if(ItemsContextMenu == null)
				return;
			Point position = fromKeyboard ? GetPositionFromKeyboard(MousePosition) : PointToClient(MousePosition);
			MenuManagerHelper.GetMenuManager(LookAndFeel, this).ShowPopupMenu(ItemsContextMenu, this, position);
		}
		Point GetPositionFromKeyboard(Point position) {
			if(FocusedNode != null) {
				var cellBounds = (ViewInfo.RowsInfo[FocusedNode].Cells[0] as CellInfo).Bounds;
				position = new Point(cellBounds.Left, cellBounds.Bottom);
			} else {
				position = new Point(Bounds.Width / 2, Bounds.Height / 2);
			}
			return position;
		}
		public TreeListItemNode GetDataItem(TreeListNode node) {
			return GetDataRecordByNode(node) as TreeListItemNode;
		}
		public void SelectAll(TreeListNodes nodes = null) {
			var op = new TreeListAllVisibleNodesOperation();
			NodesIterator.DoOperation(op);
			BeginUpdate();
			Selection.Clear();
			Selection.Add(op.Nodes);
			EndUpdate();
		}
		TreeListNodesState BeginDataOwnerUpdate() {
			BeginUpdate();
			var savedState = new TreeListNodesState();
			NodesIterator.DoOperation(savedState);
			savedState.TopVisibleNodeIndex = TopVisibleNodeIndex;
			savedState.VisibleRowCount = ViewInfo.VisibleRowCount;
			return savedState;
		}
		public void EndDataOwnerUpdate(TreeListNodesState validatedState) {
			RestoreNodesState(validatedState);
			EndUpdate();
		}
		void RestoreNodesState(TreeListNodesState validatedState) {
			Selection.Clear();
			DataSource = ItemsOwner.TreeListItems;
			RestoreExpandedNodes(validatedState);
			RestoreFocusedNode(false);
			Selection.Add(validatedState.Selected.Select(id => FindNodeByKeyID(id)));
			if(validatedState.IsNodeVisibleInSavedState(FocusedNode))
				TopVisibleNodeIndex = validatedState.TopVisibleNodeIndex;
		}
		void RestoreExpandedNodes(TreeListNodesState validatedState) {
			foreach(int id in validatedState.Expanded) {
				var node = FindNodeByKeyID(id);
				if(node != null)
					node.Expanded = true;
			}
		}
		void RestoreFocusedNode(bool selected) {
			FocusedNode = FindNodeByKeyID(ItemsOwner.FocusedNodeID);
			if(FocusedNode != null && !selected)
				FocusedNode.Selected = false;
		}
		public override void EndUpdate() {
			base.EndUpdate();
			if(shouldRaiseSelectionChanged)
				RaiseSelectionChanged();
			if(shouldRaiseFocusedNodeChanged)
				RaiseFocusedNodeChanged(FocusedNode, newFocusedNode);
		}
		protected int GetDataItemID(TreeListNode node) {
			var item = GetDataItem(node);
			return item != null ? item.ID : -1;
		}
		protected override void RaiseAfterCollapse(TreeListNode node) {
			base.RaiseAfterCollapse(node);
			FocusedNode = node;
		}
		protected override void RaiseAfterExpand(TreeListNode node) {
			base.RaiseAfterExpand(node);
			FocusedNode = node;
		}
		void IUpdatableViewControl.UpdateView() {
			if(GetDataItemID(FocusedNode) != ItemsOwner.FocusedNodeID)
				ItemsOwner.RecreateTreeListItems();
		}
	}
	public class TreeListNodesState : TreeListVisibleNodeOperation {
		public TreeListNodesState() {
			Expanded = new List<int>();
			Selected = new List<int>();
		}
		public int TopVisibleNodeIndex { get; set; }
		public int VisibleRowCount { get; set; }			
		public List<int> Expanded { get; private set; }
		public List<int> Selected { get; private set; }		
		public override void Execute(TreeListNode node) {
			if(node.Expanded)
				Expanded.Add(GetID(node));
			if(node.Selected && !node.Focused)
				Selected.Add(GetID(node));
		}
		public bool IsNodeVisibleInSavedState(TreeListNode node) {
			if(node == null)
				return false;
			TreeList tree = node.TreeList;
			int visibleIndex = tree.GetVisibleIndexByNode(node);
			return visibleIndex >= TopVisibleNodeIndex && visibleIndex < 
				TopVisibleNodeIndex + VisibleRowCount;
		}
		int GetID(TreeListNode node) {
			return Convert.ToInt32(node.GetValue("ID"));
		}
	}
	public class TreeListAllVisibleNodesOperation : TreeListVisibleNodeOperation {
		public ArrayList Nodes = new ArrayList();
		public override void Execute(TreeListNode node) { Nodes.Add(node); }
	}
	public class DraggingNodesState : DevExpress.XtraTreeList.Handler.TreeListHandler.NodeDraggingState {
		DragInsertPosition Direction = DragInsertPosition.None;
		public DraggingNodesState(XtraTreeList.Handler.TreeListHandler handler)
			: base(handler) {
		}
		public new ItemsDesignerTreeList TreeList { get { return Data.TreeList as ItemsDesignerTreeList; } }
		ItemsEditorOwner ItemsOwner { get { return TreeList.ItemsOwner; } }
		protected override DragInsertPosition GetDragInsertPosition(System.Windows.Forms.DragEventArgs e) {
			Direction = GetDirection(e.X, e.Y, base.GetDragInsertPosition(e));
			return Direction;
		}
		protected override TreeListHitTest OnNodesDragging(DragEventArgs drgevent) {
			var hitInfo = base.OnNodesDragging(drgevent);
			if(hitInfo.Node != null) {
				var dataItem = TreeList.GetDataItem(hitInfo.Node);
				if(dataItem != null && ItemsOwner.CanMoveSelectedItems(dataItem.ID, GetInsertDirection()))
					return hitInfo;
			}
			hitInfo.Clear();
			return hitInfo;
		}
		protected override void OnEndNodesDragging(DragEventArgs drgevent) {
			TreeListNode nodeTo = GetDestNode(drgevent);
			TreeList.ItemsOwner.MoveSelectedItemsTo(TreeList.GetDataItem(nodeTo).ID, GetInsertDirection());
		}		
		DragInsertPosition GetDirection(int x, int y, DragInsertPosition direction) {
			if(direction != DragInsertPosition.None)
				return direction;
			Point pt = TreeList.PointToClient(new Point(x, y));
			var hitInfo = TreeList.CalcHitInfo(pt);
			var nodeID = TreeList.GetDataItem(hitInfo.Node).ID;
			if(ItemsOwner.IsItemSupportHierarchy(nodeID))
				return direction;
			var nodeBounds = hitInfo.Bounds;
			var middleY = nodeBounds.Y + nodeBounds.Height / 2;
			return pt.Y > middleY ? DragInsertPosition.After : DragInsertPosition.Before;						
		}
		InsertDirection GetInsertDirection() {
			switch(Direction) {
				case DragInsertPosition.After:
					return InsertDirection.After;
				case DragInsertPosition.Before:
					return InsertDirection.Before;
			}
			return InsertDirection.Inside;
		}
	}
	public class ColumnsDesignerTreeListHandler : XtraTreeList.Handler.TreeListHandler {
		public ColumnsDesignerTreeListHandler(TreeList treelist) : base(treelist) { }
		protected override XtraTreeList.Handler.TreeListHandler.TreeListControlState CreateState(TreeListState state) {
			if(state == TreeListState.NodeDragging) {
				return new DraggingNodesState(this);
			}
			return base.CreateState(state);
		}
	}
	public class ItemToolTip : ToolTipController {
		public ItemToolTip(ItemsDesignerTreeList treeList) 
			: base() {
				TreeList = treeList;
				GetActiveObjectInfo += ItemToolTip_GetActiveObjectInfo;
		}
		ItemsDesignerTreeList TreeList { get; set; }
		void ItemToolTip_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e) {
			var hitInfo = TreeList.CalcHitInfo(e.ControlMousePosition);
			if(hitInfo.HitInfoType != HitInfoType.StateImage && hitInfo.HitInfoType != HitInfoType.SelectImage)
				return;
			var dataItem = TreeList.GetDataItem(hitInfo.Node);
			var designTimeItem = TreeList.ItemsOwner.FindDesignTimeColumnType(dataItem.ID);
			if(designTimeItem != null) 
				e.Info = new ToolTipControlInfo(hitInfo.Node, designTimeItem.ColumnType.Name.ToString(), designTimeItem.Text);
		}
	}
	public class ItemsDesignerTreeListViewInfo : TreeListViewInfo {
		public ItemsDesignerTreeListViewInfo(TreeList treeList)
			: base(treeList) {
		}
		new ItemsDesignerTreeList TreeList { get { return (ItemsDesignerTreeList)base.TreeList; } }
		protected override Point GetDataBoundsLocation(TreeListNode node, int top) {
			Point result = base.GetDataBoundsLocation(node, top);
			if(GetDataItemImageIndex(node) == -1)
				result.X -= RC.StateImageSize.Width;
			return result;
		}
		protected override void CalcStateImage(RowInfo ri) {
			base.CalcStateImage(ri);
			if(GetDataItemImageIndex(ri.Node) == -1)
				ri.StateImageLocation.X -= RC.SelectImageSize.Width;
		}
		protected override void CalcSelectImage(RowInfo ri) {
			base.CalcSelectImage(ri);
			var dataItem = TreeList.GetDataItem(ri.Node);
			if(GetDataItemImageIndex(ri.Node) == -1)
				ri.SelectImageLocation = Point.Empty;
		}
		int GetDataItemImageIndex(TreeListNode node) { 
			var dataItem = TreeList.GetDataItem(node);
			return dataItem != null ? dataItem.ImageIndex : -1;
		}
	}
}
