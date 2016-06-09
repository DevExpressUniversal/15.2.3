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
using System.Reflection;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraEditors;
using DevExpress.Utils.Menu;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Handlers;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Customization;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Customization.Controls;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using DevExpress.Utils.Design.Internal;
namespace DevExpress.XtraLayout.Customization.Controls {
	public enum TreeViewRoles { LayoutTreeView, ResizeTreeH, ResizeTreeV, SE_Tree };
	public enum LayoutTreeViewHitType { Left, Right, Top, Bottom, None }
	public class LayoutTreeViewHitInfo {
		Point hitPointCore;
		TreeNode nodeCore;
		LayoutTreeViewHitType hitTypeCore;
		public LayoutTreeViewHitInfo(Point hitPoint, TreeNode node, LayoutTreeViewHitType hitType) {
			this.hitPointCore = hitPoint;
			this.nodeCore = node;
			this.hitTypeCore = hitType;
		}
		public Point HitPoint {
			get { return hitPointCore; }
		}
		public TreeNode Node {
			get { return nodeCore; }
		}
		public LayoutTreeViewHitType HitType {
			get { return hitTypeCore; }
		}
		public BaseLayoutItem Item {
			get {
				BaseLayoutItemTreeNode itemNode = Node as BaseLayoutItemTreeNode;
				return (itemNode != null) ? itemNode.Item : null;
			}
		}
		public InsertType InsertType {
			get {
				switch(HitType) {
					case LayoutTreeViewHitType.Left: return InsertType.Left;
					case LayoutTreeViewHitType.Right: return InsertType.Right;
					case LayoutTreeViewHitType.Top: return InsertType.Top;
				}
				return InsertType.Bottom;
			}
		}
		public override bool Equals(object obj) {
			LayoutTreeViewHitInfo hi = obj as LayoutTreeViewHitInfo;
			if(hi==null) return false;
			return (nodeCore == hi.nodeCore) && (hitTypeCore == hi.hitTypeCore);
		}
		public override int GetHashCode() {
			return ((Node != null) ? Node.GetHashCode() : 0) ^ HitType.GetHashCode();
		}
		public static bool operator ==(LayoutTreeViewHitInfo left, LayoutTreeViewHitInfo right) {
			if((null == (object)left) || (null == (object)right)) return (object)left == (object)right;
			return ((left.nodeCore == right.nodeCore) && (left.hitTypeCore == right.hitTypeCore));
		}
		public static bool operator !=(LayoutTreeViewHitInfo left, LayoutTreeViewHitInfo right) {
			return !(left == right);
		}
	}
	public class LayoutTreeViewHitInfoCalculator {
		LayoutTreeView ownerTreeView;
		public LayoutTreeViewHitInfoCalculator(LayoutTreeView treeView) { 
			ownerTreeView = treeView; 
		}
		public LayoutTreeViewHitInfo CalcHitInfo(Point hitPoint) {
			return CalcHitInfo(ownerTreeView, hitPoint);
		}
		public static LayoutTreeViewHitInfo CalcHitInfo(TreeView treeView, Point pt) {
			double LeftRightAreaRating = 0.25;
			TreeNode node = treeView.GetNodeAt(pt);
			LayoutTreeViewHitType hitType = LayoutTreeViewHitType.None;
			if(node != null) {
				int horizontalZoneBoundLeft = node.Bounds.Left + (int)(node.Bounds.Width * LeftRightAreaRating);
				int horizontalZoneBoundRight = node.Bounds.Right - (int)(node.Bounds.Width * LeftRightAreaRating);
				int verticalZoneBound = node.Bounds.Top + node.Bounds.Height / 2;
				if(pt.X < horizontalZoneBoundLeft) {
					hitType = LayoutTreeViewHitType.Left;
				}
				else if(pt.X > horizontalZoneBoundRight) {
					hitType = LayoutTreeViewHitType.Right;
				}
				else {
					if(pt.Y < verticalZoneBound) {
						hitType = LayoutTreeViewHitType.Top;
					}
					else {
						hitType = LayoutTreeViewHitType.Bottom;
					}
				}
			}
			return new LayoutTreeViewHitInfo(pt, node, hitType);
		}
	}
	public class BaseLayoutItemTreeNode : TreeNode {
		BaseLayoutItem itemCore;
		public BaseLayoutItemTreeNode(string text, BaseLayoutItem item)
			: base() {
			Text = ProcessText(text);
			this.itemCore = item;
			UpdateNodeImageIndexes();
		}
		protected virtual string ProcessText(string text) {
			StringBuilder sb = new StringBuilder();
			if(text != null)
				foreach(char c in text) {
					if(c != '&') sb.Append(c);
				}
			return sb.ToString();
		}
		protected internal void UpdateNodeImageIndexes() {
			ImageIndex = SelectedImageIndex = GetItemImageIndex(Item);
		}
		protected virtual int GetItemImageIndex(BaseLayoutItem item) {
			LayoutClassificationArgs itemClass = LayoutClassifier.Default.Classify(item);
			if(itemClass.IsGroup) return 1;
			if(itemClass.IsTabbedGroup) return 2;
			if(item is IFixedLayoutControlItem) {
				if(Owner != null) {
					string typeName = (item as IFixedLayoutControlItem).TypeName;
					if(Owner.FixedTypeImageIndexes.ContainsKey(typeName)) return Owner.FixedTypeImageIndexes[typeName];
				}
			}
			return 0;
		}
		public BaseLayoutItem Item {
			get { return itemCore; }
		}
		protected LayoutTreeView Owner { 
			get { return TreeView as LayoutTreeView; } 
		}
	}
	public class LayoutResizerTreeView : LayoutTreeView {
		public override void AddNode(object item, TreeNode node) {
			AddNodeInternal(item, node);
		}
		protected override void InitializeLayoutTreeViewAsDragDropClient() {
			(this as IDragDropDispatcherClient).AllowProcessDragging = false;
		}
	}
	[DesignTimeVisible(true), ToolboxItem(false)]
	[Designer(LayoutControlConstants.LayoutTreeViewDesignerName, typeof(System.ComponentModel.Design.IDesigner)), ToolboxBitmap(typeof(LayoutControl), "Images.tree-view.bmp")]
	public class LayoutTreeView : DevExpress.Utils.Design.DXTreeView, IDragDropDispatcherClient, IToolFrameOwner, ICustomizationFormControl {
		const string hiddenItemNodeString = "HiddenItemNode";
		protected internal DragDropClientDescriptor clientDescriptorCore = null;
		bool fCanProcessEvent = true;
		bool fAllowProcessDragging = true;
		bool fAllowNonItemDrop = false;
		DragFrameHitInfoPainter hitInfoPainter;
		Dictionary<string, int> fixedTypeImageIndexesCore = null;
		Timer autoScrollTimer;
		Point? autoScrollLastDragPoint;
		TreeNode nodeAutoScroll;
		public LayoutTreeView() {
			AllowSkinning = true;
			Initialize();
		}
		internal Control ContainerControl {
			get {
				Control result = base.containerCore as Control;
				if(result == this) return null;
				return result;
			}
		}
		bool IDragDropDispatcherClient.ExcludeChildren { get { return false; } }
		protected void SetTreeViewImages() {
			FixedTypeImageIndexes.Clear();
			ImageCollection ic = LayoutControlImageStorage.Default.CustomizationTreeView;
			if(ImageList == null) {
				ImageList = new ImageList();
				ImageList.ColorDepth = ColorDepth.Depth32Bit;
				ImageList.ImageSize = new Size(16, 16);
				int imageIndex = 0;
				for(imageIndex = 0; imageIndex <= 2; imageIndex++) {
					ImageList.Images.Add(ic.Images[imageIndex]);
				}
				foreach(IFixedLayoutControlItem fixedItem in OwnerControl.HiddenItems.FixedItems) {
					Image image = (fixedItem.CustomizationImage != null) ? fixedItem.CustomizationImage : ic.Images[0];
					ImageList.Images.Add(image);
					FixedTypeImageIndexes.Add(fixedItem.TypeName, imageIndex++);
				}
			}
		}
		protected internal Dictionary<string, int> FixedTypeImageIndexes {
			get {
				if(fixedTypeImageIndexesCore == null) fixedTypeImageIndexesCore = new Dictionary<string, int>();
				return fixedTypeImageIndexesCore;
			}
		}
		protected void OnShowRightButtonMenu(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				ShowMenuCore(e.Location);
			}
		}
		protected BaseLayoutItemTreeNode GetTreeViewNodeAtHitPoint(Point hitPoint) {
			return GetNodeAt(hitPoint) as BaseLayoutItemTreeNode;
		}
		protected virtual void ShowMenuCore(Point hitPoint) {
			if(Role != TreeViewRoles.LayoutTreeView || !Visible) return;
			BaseLayoutItemTreeNode targetNode = GetTreeViewNodeAtHitPoint(hitPoint);
			if(null != targetNode) {
				Rectangle startNodeRect = targetNode.Bounds;
				if(!targetNode.IsSelected) SelectNode(targetNode);
				Rectangle endNodeRect = targetNode.Bounds;
				Point tmpPoint = hitPoint;
				if(endNodeRect != startNodeRect) {
					tmpPoint = new Point(endNodeRect.Left + endNodeRect.Width / 2, endNodeRect.Top + endNodeRect.Height / 2);
				}
				Point popupPoint = PointOperations.Subtract(PointToScreen(tmpPoint), OwnerControl.Control.PointToScreen(Point.Empty));
				BaseLayoutItem item = targetNode.Item;
				if(item == null) return;
				LayoutGroup lg = item.IsGroup ? (item as LayoutGroup) : item.Parent;
				this.OwnerControl.CustomizationMenuManager.ShowPopUpMenu(new BaseLayoutItemHitInfo(hitPoint, LayoutItemHitTest.Item, item), lg, popupPoint, true);
			}
		}
		#region ICustomizationFormControl
		public void Register() {
			if(OwnerControl != null) {
				((ILayoutControlOwner)OwnerControl).ItemSelectionChanged += new EventHandler(OwnerSelectionChanged);
				if(Role == TreeViewRoles.LayoutTreeView) {
					SubscribeItemsChanged();
					this.KeyDown += new KeyEventHandler(LayoutTreeView_KeyDown);
					this.MouseUp += new MouseEventHandler(LayoutTreeView_MouseUp);
				}
				if(Role == TreeViewRoles.ResizeTreeH || Role == TreeViewRoles.ResizeTreeV)
					((ILayoutControlOwner)OwnerControl).LayoutUpdate += new EventHandler(OwnerItemsChanged);
				Initialize();
			}
		}
		protected virtual void SubscribeItemsChanged() {
			ILayoutControl lc = OwnerControl;
			ILayoutControlOwner lcOwner = lc as ILayoutControlOwner;
			ILayoutControlOwnerEx lcOwnerEx = lc as ILayoutControlOwnerEx;
			lcOwner.ItemAdded += OwnerItemsChanged;
			lcOwner.ItemRemoved += OwnerItemsChanged;
			if(lcOwnerEx!=null)
				lcOwnerEx.LayoutLoaded += lcOwnerEx_LayoutLoaded;
			lc.HiddenItems.Changed += OwnerHiddenItemsChanged;
		}
		protected virtual void UnSubscribeItemsChanged() {
			ILayoutControl lc = OwnerControl;
			ILayoutControlOwner lcOwner = lc as ILayoutControlOwner;
			ILayoutControlOwnerEx lcOwnerEx = lc as ILayoutControlOwnerEx;
			lcOwner.ItemAdded -= OwnerItemsChanged;
			lcOwner.ItemRemoved -= OwnerItemsChanged;
			if(lcOwnerEx != null)
				lcOwnerEx.LayoutLoaded -= lcOwnerEx_LayoutLoaded;
			lc.HiddenItems.Changed += OwnerHiddenItemsChanged;
		}
		public void UnRegister() {
			if(OwnerControl != null) {
				((ILayoutControlOwner)OwnerControl).ItemSelectionChanged -= new EventHandler(OwnerSelectionChanged);
				if(Role == TreeViewRoles.LayoutTreeView) {
					UnSubscribeItemsChanged();
					this.KeyDown -= new KeyEventHandler(LayoutTreeView_KeyDown);
					this.MouseUp -= new MouseEventHandler(LayoutTreeView_MouseUp);
				}
				if(Role == TreeViewRoles.ResizeTreeH || Role == TreeViewRoles.ResizeTreeV)
					((ILayoutControlOwner)OwnerControl).LayoutUpdate -= new EventHandler(OwnerItemsChanged);
			}
			TreeViewVisualizerFrame.Reset();
			DragDropItemCursor.Reset();
			if(clientDescriptorCore != null) DragDropDispatcherFactory.Default.UnRegisterClient(clientDescriptorCore);
		}
		void LayoutTreeView_MouseUp(object sender, MouseEventArgs e) { OnShowRightButtonMenu(sender, e); }
		void LayoutTreeView_KeyDown(object sender, KeyEventArgs e) { OnTreeViewKeyDown(sender, e); }
		protected virtual void OwnerSelectionChanged(object sender, EventArgs e) {
			UpdateSelection(sender as BaseLayoutItem);
		}
		protected virtual void OwnerHiddenItemsChanged(object sender, CollectionChangeEventArgs e) {
			ILayoutControlOwnerEx owner = OwnerControl as ILayoutControlOwnerEx;
			if(owner != null && owner.IsLayoutLoading) return;
			UpdateCoreTreeView();
		}
		protected virtual void OwnerItemsChanged(object sender, EventArgs e) {
			ILayoutControlOwnerEx owner = OwnerControl as ILayoutControlOwnerEx;
			if(owner != null && owner.IsLayoutLoading) return;
			UpdateCoreTreeView();
		}
		void lcOwnerEx_LayoutLoaded(object sender, EventArgs e) {
			UpdateCoreTreeView();
		}
		public ILayoutControl OwnerControl { get { return GetOwnerControl(); } }
		protected override void OnPaint(PaintEventArgs e) {
			if(OwnerControl == null) { WrongParentTypeMessagePainter.Default.Draw(new Rectangle(0, 0, Width, Height), e); return; }
			base.OnPaint(e);
		}
		protected virtual ILayoutControl GetOwnerControl() {
			ILayoutControl control = OwnerControlHelper.GetOwnerControl(Parent);
			return control ?? OwnerControlHelper.GetOwnerControl(ContainerControl);
		}
		public UserLookAndFeel ControlOwnerLookAndFeel { get { return OwnerControl != null ? OwnerControl.LookAndFeel : null; } }
		#endregion
		public virtual void Initialize() {
			if(OwnerControl == null || OwnerControl.RootGroup == null) return;
			if(hitInfoPainter != null) return;
			SetTreeViewImages();
			InitializeLayoutTreeViewAsDragDropClient();
			this.hitInfoPainter = new DragFrameHitInfoPainter(TreeViewVisualizerFrame.Default);
			UpdateCoreTreeView();
			RightToLeftLayout = true;
			autoScrollTimer = new Timer();
			autoScrollTimer.Interval = 500;
			autoScrollTimer.Tick += OnAutoScrollTimerTick;
		}
		protected virtual void InitializeLayoutTreeViewAsDragDropClient() {
			AllowCustomMouseMoveProcessing = false;
			this.AllowDrag = false;
			this.AllowDrop = false;
			clientDescriptorCore = DragDropDispatcherFactory.Default.RegisterClient(this as IDragDropDispatcherClient);
			DragDropDispatcherFactory.Default.SetClientToolFrame(clientDescriptorCore, DragDropItemCursor.Default);
			DragDropDispatcherFactory.Default.SetClientToolFrame(clientDescriptorCore, TreeViewVisualizerFrame.Default);
		}
		bool isDisposing = false;
		protected bool IsDisposingInProgress { get { return isDisposing; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				isDisposing = true;
				UnRegister();
				if(autoScrollTimer != null) {
					autoScrollTimer.Tick -= OnAutoScrollTimerTick;
					autoScrollTimer.Dispose();
					autoScrollTimer = null;
				}
			}
			base.Dispose(disposing);
		}
		public void BeginInit() { }
		public void EndInit() { }
#if DEBUGTEST
		protected internal void CallOnMouseDown(MouseEventArgs e) { OnMouseDown(e); }														   
		protected internal void CallOnMouseUp(MouseEventArgs e) { OnMouseUp(e); }
		protected internal void CallOnMouseMove(MouseEventArgs e) { OnMouseMove(e); }
#endif
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			DragDropDispatcherFactory.Default.ProcessMouseEvent(clientDescriptorCore, new ProcessEventEventArgs(EventType.MouseDown, e));
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Left) {
				DragDropDispatcherFactory.Default.ProcessMouseEvent(clientDescriptorCore, new ProcessEventEventArgs(EventType.MouseUp, e));
			}
			else {
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			BeginUpdateTreeView();
			if(e.Button == MouseButtons.Left) {
				ArrayList cNodes = new ArrayList(SelectedNodes.Keys);
				DragDropDispatcherFactory.Default.ProcessMouseEvent(clientDescriptorCore, new ProcessEventEventArgs(EventType.MouseMove, e));
				if(SelectedNodes.Count != cNodes.Count || (SelectedNodes.Count > 0 && cNodes.Count > 0 && new ArrayList(SelectedNodes.Keys)[0] != cNodes[0])) {
					ClearSelection();
					foreach(TreeNode tNode in cNodes) {
						AddSelection(tNode);
					}
				}
			}
			EndUpdateTreeView();
		}
		protected void Accept(Action<TreeNode> visit) {
			TreeNode root = Nodes.Count > 0 ? Nodes[0] : null;
			if(root != null) AcceptCore(root, visit);
		}
		void AcceptCore(TreeNode node, Action<TreeNode> visit) {
			if(node == null) return;
			if(visit != null) visit(node);
			AcceptNodes(node.Nodes, visit);
		}
		void AcceptNodes(TreeNodeCollection nodes, Action<TreeNode> visit) {
			if(nodes == null || nodes.Count == 0) return;
			for(int i = 0; i < nodes.Count; i++) {
				AcceptCore(nodes[i], visit);
			}
		}
		public TreeNode GetNodeByItem(TreeNode rootNode, BaseLayoutItem item) {
			BaseLayoutItemTreeNode root = rootNode as BaseLayoutItemTreeNode;
			if(root != null) {
				if(root.Item == item) return root;
				if(root.Nodes.Count > 0) {
					foreach(BaseLayoutItemTreeNode node in root.Nodes) {
						TreeNode result = GetNodeByItem(node, item);
						if(result != null) return result;
					}
				}
			}
			return null;
		}
		public int GetTreeViewContentHeight(TreeNode rootNode) {
			int strHeight = 0;
			if(rootNode != null) {
				if(!rootNode.IsExpanded || rootNode.Nodes.Count == 0) {
					return rootNode.TreeView.ItemHeight;
				}
				else {
					foreach(TreeNode node in rootNode.Nodes) {
						strHeight += GetTreeViewContentHeight(node);
					}
					return rootNode.TreeView.ItemHeight + strHeight;
				}
			}
			return 0;
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				const int TVS_NOHSCROLL = 0x8000;
				cp.Style &= ~TVS_NOHSCROLL;
				return cp;
			}
		}
		protected void AddNodeCore(TreeNode node, TreeNode tempNode) {
			if(node != null) node.Nodes.Add(tempNode);
			else Nodes.Add(tempNode);
			(tempNode as BaseLayoutItemTreeNode).UpdateNodeImageIndexes();
		}
		public virtual void AddNode(object item, TreeNode node) {
			if(item is BaseLayoutItem && !((BaseLayoutItem)item).ShowInCustomizationForm) return;
			AddNodeInternal(item, node);
		}
		protected void AddNodeInternal(object item, TreeNode node) {
			BaseLayoutItemTreeNode treeNode = item as BaseLayoutItemTreeNode;
			LayoutGroup group = item as LayoutGroup;
			TabbedGroup tgroup = item as TabbedGroup;
			LayoutControlItem citem = item as LayoutControlItem;
			Resizing.TableGroupResizeGroup tableResizeGroup = item as Resizing.TableGroupResizeGroup;
			Resizing.GroupResizeGroup resizerGroup = item as Resizing.GroupResizeGroup;
			Resizing.FakeGroup fakeResizeGroup = item as Resizing.FakeGroup;
			Resizing.FlowGroupResizeGroup flowResizeGroup = item as Resizing.FlowGroupResizeGroup;
			Resizing.ResizeGroup resizerSimpleGroup = item as Resizing.ResizeGroup;
			Adapters.XAFLayoutItemInfo xafItem = item as Adapters.XAFLayoutItemInfo;
			Adapters.XAFLayoutGroupInfo xafGroup = item as Adapters.XAFLayoutGroupInfo;
			Adapters.XAFTabbedGroupInfo xafTabbedGroup = item as Adapters.XAFTabbedGroupInfo;
			TreeNode tempNode = null;
			if(treeNode != null) {
				tempNode = treeNode;
				AddNodeCore(node, tempNode);
			}
			if(xafItem != null && xafGroup == null && xafTabbedGroup == null) {
				tempNode = new BaseLayoutItemTreeNode("XAFLayoutItemInfo for " + xafItem.Item.Text + " " + xafItem.RelativeSize.ToString(), xafItem.Item);
				AddNodeCore(node, tempNode);
			}
			if(xafGroup != null) {
				tempNode = new BaseLayoutItemTreeNode("XAFLayoutGroupInfo for " + xafGroup.ID + " " + xafItem.RelativeSize.ToString(), xafGroup.Item);
				AddNodeCore(node, tempNode);
				foreach(Adapters.XAFLayoutItemInfo titem in xafGroup.Items)
					AddNode(titem, tempNode);
			}
			if(xafTabbedGroup != null) {
				tempNode = new BaseLayoutItemTreeNode("XAFTabbedGroupInfo for " + xafTabbedGroup.ID + " " + xafItem.RelativeSize.ToString(), xafTabbedGroup.Item);
				AddNodeCore(node, tempNode);
				foreach(Adapters.XAFLayoutItemInfo titem in xafTabbedGroup.Items)
					AddNode(titem, tempNode);
			}
			if(flowResizeGroup != null) {
				tempNode = new BaseLayoutItemTreeNode("FlowGroupResizeGroup for " + resizerGroup.Group.Text, resizerGroup);
				AddNodeCore(node, tempNode);
				AddNode(resizerGroup.Item, tempNode);
			}
			if(tableResizeGroup != null) {
				tempNode = new BaseLayoutItemTreeNode("TableResizeGroup for " + tableResizeGroup.Group.Text, tableResizeGroup);
				AddNodeCore(node, tempNode);
				AddNode(tableResizeGroup.Group, tempNode);
			}
			if(fakeResizeGroup != null) {
				tempNode = new BaseLayoutItemTreeNode("FakeGroupResizeGroup for " + resizerGroup.Group.Text, resizerGroup);
				AddNodeCore(node, tempNode);
				AddNode(resizerGroup.Item, tempNode);
			}
			if(resizerGroup != null && fakeResizeGroup == null && flowResizeGroup == null && tableResizeGroup == null) {
				tempNode = new BaseLayoutItemTreeNode("GroupResizeGroup for " + resizerGroup.Group.Text, resizerGroup);
				AddNodeCore(node, tempNode);
				AddNode(resizerGroup.Item, tempNode);
			}
			if(resizerSimpleGroup != null) {
				tempNode = new BaseLayoutItemTreeNode(resizerSimpleGroup.CustomizationFormText, resizerSimpleGroup);
				AddNodeCore(node, tempNode);
				AddNode(resizerSimpleGroup.Item1, tempNode);
				AddNode(resizerSimpleGroup.Item2, tempNode);
			}
			if(group != null) {
				tempNode = AddGroupNode(node, group, tempNode);
			}
			if(tgroup != null) {
				tempNode = new BaseLayoutItemTreeNode(tgroup.CustomizationFormText, tgroup);
				AddNodeCore(node, tempNode);
				foreach(BaseLayoutItem titem in tgroup.TabPages)
					AddNode(titem, tempNode);
			}
			if(citem != null) {
				if(citem is IFixedLayoutControlItem) {
					tempNode = new BaseLayoutItemTreeNode((citem as IFixedLayoutControlItem).CustomizationName, citem);
				}
				else {
					tempNode = new BaseLayoutItemTreeNode(citem.CustomizationFormText, citem);
				}
				AddNodeCore(node, tempNode);
			}
			if(tempNode != null && item is BaseLayoutItem)
				if(((BaseLayoutItem)item).Selected) SelectedNode = tempNode;
		}
		private TreeNode AddGroupNode(TreeNode node, LayoutGroup group, TreeNode tempNode) {
			tempNode = new BaseLayoutItemTreeNode(group.CustomizationFormText, group);
			AddNodeCore(node, tempNode);
			Customization.LayoutControlWalker walker = new Customization.LayoutControlWalker(group);
			OptionsFocus options = new OptionsFocus(null);
			options.MoveFocusDirection = MoveFocusDirection.AcrossThenDown;
			options.MoveFocusRightToLeft = false;
			ArrayList list = walker.ArrangeElements(options);
			foreach(BaseLayoutItem titem in list)
				AddNode(titem, tempNode);
			return tempNode;
		}
		public void Rename() {
			if(SelectedNode != null) {
				if(!(SelectedNode is BaseLayoutItemTreeNode)) return;
				BaseLayoutItem selectedItem = ((BaseLayoutItemTreeNode)SelectedNode).Item;
				int iWidth = this.ClientRectangle.Width;
				Rectangle rect = new Rectangle(SelectedNode.Bounds.Location, new Size(iWidth - SelectedNode.Bounds.X - 2, SelectedNode.Bounds.Height));
				Point screenPoint = PointToScreen(SelectedNode.Bounds.Location);
				rect.Location = screenPoint;
				(OwnerControl as ISupportImplementor).Implementor.RenameItemManager.Rename(selectedItem, rect);
			}
		}
		#region IDragDropDispatcherClient
		IntPtr IDragDropDispatcherClient.ClientHandle {
			get { return this.Handle; }
		}
		DragDropClientDescriptor IDragDropDispatcherClient.ClientDescriptor {
			get { return clientDescriptorCore; }
		}
		Rectangle IDragDropDispatcherClient.ScreenBounds {
			get { return this.RectangleToScreen(this.ClientRectangle); }
		}
		bool IDragDropDispatcherClient.IsActiveAndCanProcessEvent {
			get { return fCanProcessEvent && this.Visible; }
		}
		bool IDragDropDispatcherClient.AllowProcessDragging {
			get { return fAllowProcessDragging; }
			set { fAllowProcessDragging = value; }
		}
		bool IDragDropDispatcherClient.AllowNonItemDrop {
			get { return fAllowNonItemDrop; }
			set { fAllowNonItemDrop = value; }
		}
		bool IDragDropDispatcherClient.IsPointOnItem(Point clientPoint) {
			LayoutTreeViewHitInfo hi = LayoutTreeViewHitInfoCalculator.CalcHitInfo(this, clientPoint);
			return hi.HitType != LayoutTreeViewHitType.None;
		}
		BaseLayoutItem IDragDropDispatcherClient.GetItemAtPoint(Point clientPoint) {
			LayoutTreeViewHitInfo hi = LayoutTreeViewHitInfoCalculator.CalcHitInfo(this, clientPoint);
			if(IsHiddenItemNode(hi)) return new EmptySpaceItem();
			return hi.Item;
		}
		BaseLayoutItem IDragDropDispatcherClient.ProcessDragItemRequest(Point clientPoint) {
			LayoutTreeViewHitInfo hi = LayoutTreeViewHitInfoCalculator.CalcHitInfo(this, clientPoint);
			return hi.Item;
		}
		DragDropClientGroupDescriptor IDragDropDispatcherClient.ClientGroup {
			get {
				IDragDropDispatcherClient groupOwner = OwnerControl as IDragDropDispatcherClient;
				if(groupOwner != null) return groupOwner.ClientGroup;
				return null;
			}
		}
		void IDragDropDispatcherClient.OnDragModeKeyDown(KeyEventArgs kea) {
			switch(kea.KeyCode) {
				case Keys.Tab:
					if(kea.Control) {
						DragFrameOff(TreeViewVisualizerFrame.Default);
						DragCursorOff(DragDropItemCursor.Default);
					}
					break;
			}
		}
		void IDragDropDispatcherClient.DoBeginDrag() {
			DragFrameOn(TreeViewVisualizerFrame.Default);
			DragCursorOn(DragDropItemCursor.Default);
			autoScrollTimer.Start();
		}
		void OnAutoScrollTimerTick(object sender, EventArgs e) {
			if(DragDropDispatcherFactory.Default.DragItem == null || autoScrollLastDragPoint == null) return;
			Rectangle topRect;
			Rectangle bottomRect;
			GetAutoScrollArea(out topRect, out bottomRect);
			if(ScrollableContainer != null && ScrollableContainer.ScrollHelper != null) AutoScrollByScrollableContainer(topRect, bottomRect);
			else
				AutoScrollWithoutScrollableContainer(topRect, bottomRect);
		}
		void AutoScrollWithoutScrollableContainer(Rectangle topRect, Rectangle bottomRect) {
			TreeNode nodeToSet = GetNodeAt(autoScrollLastDragPoint.GetValueOrDefault());
			nodeAutoScroll = nodeToSet == null ? SelectedNode : nodeToSet;
			if(nodeAutoScroll == null) return;
			if(topRect.Contains(autoScrollLastDragPoint.GetValueOrDefault())) {
				if(nodeAutoScroll.PrevVisibleNode != null) {
					nodeAutoScroll = nodeAutoScroll.PrevVisibleNode;
					nodeAutoScroll.EnsureVisible();
					(this as IDragDropDispatcherClient).DoDragging(autoScrollLastDragPoint.GetValueOrDefault());
				}
			}
			if(bottomRect.Contains(autoScrollLastDragPoint.GetValueOrDefault())) {
				nodeAutoScroll.EnsureVisible();
				(this as IDragDropDispatcherClient).DoDragging(autoScrollLastDragPoint.GetValueOrDefault());
			}
		}
		private void AutoScrollByScrollableContainer(Rectangle topRect, Rectangle bottomRect) {
			if(topRect.Contains(autoScrollLastDragPoint.GetValueOrDefault())) {
				(ScrollableContainer.ScrollHelper.VScroll as IScrollBar).ChangeValueBasedByState(XtraEditors.ViewInfo.ScrollBarState.DecButtonPressed);
				(this as IDragDropDispatcherClient).DoDragging(autoScrollLastDragPoint.GetValueOrDefault());
			}
			if(bottomRect.Contains(autoScrollLastDragPoint.GetValueOrDefault())) {
				(ScrollableContainer.ScrollHelper.VScroll as IScrollBar).ChangeValueBasedByState(XtraEditors.ViewInfo.ScrollBarState.IncButtonPressed);
				(this as IDragDropDispatcherClient).DoDragging(autoScrollLastDragPoint.GetValueOrDefault());
			}
		}
		void GetAutoScrollArea(out Rectangle topRect, out Rectangle bottomRect) {
			int constAreaDropSize = 10;
			topRect = ClientRectangle;
			topRect.Height = constAreaDropSize;
			bottomRect = ClientRectangle;
			bottomRect.Y = bottomRect.Bottom - constAreaDropSize;
			bottomRect.Height = constAreaDropSize;
		}
		void IDragDropDispatcherClient.DoDragCancel() {
			DragCursorOff(DragDropItemCursor.Default);
			DragFrameOff(TreeViewVisualizerFrame.Default);
			autoScrollLastDragPoint = null;
			autoScrollTimer.Stop();
			nodeAutoScroll = null;
		}
		void IDragDropDispatcherClient.OnDragEnter() {
			DragFrameOn(TreeViewVisualizerFrame.Default);
			DragCursorOn(DragDropItemCursor.Default);
			autoScrollTimer.Start();
		}
		void IDragDropDispatcherClient.OnDragLeave() {
			DragCursorOff(DragDropItemCursor.Default);
			DragFrameOff(TreeViewVisualizerFrame.Default);
			autoScrollTimer.Stop();
		}
		private static bool IsHiddenItemNode(LayoutTreeViewHitInfo hitInfo) {
			return hitInfo.Node != null && hitInfo.Node.Tag != null && hitInfo.Node.Tag.Equals(hiddenItemNodeString);
		}
		void IDragDropDispatcherClient.DoDragging(Point clientPoint) {
			autoScrollLastDragPoint = clientPoint;
			if(hitInfoPainter.Owner != TreeViewVisualizerFrame.Default) hitInfoPainter = new DragFrameHitInfoPainter(TreeViewVisualizerFrame.Default);
			LayoutTreeViewHitInfo hitInfo = LayoutTreeViewHitInfoCalculator.CalcHitInfo(this, clientPoint);
			DragFrameOn(TreeViewVisualizerFrame.Default);
			DragCursorOn(DragDropItemCursor.Default);
			bool draggingOnHiddenItems = hitInfo.Item != null && hitInfo.Item.IsHidden || IsHiddenItemNode(hitInfo);
			Graphics g = Graphics.FromHwnd(TreeViewVisualizerFrame.Default.Handle);
			DropType dropType = GetDropType(DragDropDispatcherFactory.Default.DragItem, hitInfo);
			LayoutItemDragController controller = GetLayoutItemDragController(hitInfo);
			if((OwnerControl is LayoutControl) && !(OwnerControl as LayoutControl).RaiseOnItemDragging(controller)) {
				dropType = DropType.Illegal;
			}
			if(dropType != DropType.Illegal) {
				if(draggingOnHiddenItems) {
					SetCursorCross(DragDropItemCursor.Default);
					DragCursorCenterAtPoint(DragDropItemCursor.Default, hitInfo.HitPoint);
					hitInfoPainter.Draw(g, hitInfo, true);
				}
				else {
					SetCursorDefault(DragDropItemCursor.Default);
					DragCursorSetPos(DragDropItemCursor.Default, hitInfo);
					hitInfoPainter.Draw(g, hitInfo, false);
				}
			}
			else {
				SetCursorIllegal(DragDropItemCursor.Default);
				DragCursorCenterAtPoint(DragDropItemCursor.Default, hitInfo.HitPoint);
				hitInfoPainter.Draw(g, hitInfo, true);
			}
			TreeViewVisualizerFrame.Default.Validate();
		}
		private static LayoutItemDragController GetLayoutItemDragController(LayoutTreeViewHitInfo hitInfo) {
			InsertLocation location;
			LayoutType type;
			GetLayoutTypeAndInsertLocation(hitInfo, out location, out type);
			bool fOutsise = (DragDropDispatcherFactory.Default.DragItem is SplitterItem || DragDropDispatcherFactory.Default.DragItem is SimpleSeparator);
			LayoutItemDragController controller = new LayoutItemDragController(
					DragDropDispatcherFactory.Default.DragItem, hitInfo.Item, fOutsise ? MoveType.Outside : MoveType.Inside, location, type
				);
			return controller;
		}
		void DragCursorCenterAtPoint(DragDropItemCursor cursor, Point pt) {
			Point p = PointToScreen(pt);
			p.Offset(new Point(-cursor.Width / 2, -cursor.Height / 2));
			cursor.Location = p;
		}
		void IDragDropDispatcherClient.DoDrop(Point clientPoint) {
			LayoutTreeViewHitInfo hitInfo = LayoutTreeViewHitInfoCalculator.CalcHitInfo(this, clientPoint);
			BaseLayoutItem dragItem = DragDropDispatcherFactory.Default.DragItem;
			if(hitInfo.Item == dragItem) return;
			LayoutItemDragController controller = GetLayoutItemDragController(hitInfo);
			if((OwnerControl is LayoutControl) && !(OwnerControl as LayoutControl).RaiseOnItemDragging(controller)) {
				return;
			}
			if(CanDropAt(dragItem, hitInfo)) {
				if(hitInfo.Item != null && hitInfo.Item.IsHidden || IsHiddenItemNode(hitInfo)) {
					OwnerControl.BeginUpdate();
					dragItem.HideToCustomization();
					OwnerControl.EndUpdate();
					return;
				}
				OwnerControl.BeginUpdate();
				controller.Drag();
				bool fDraggingFromHiddenItems = dragItem != null && dragItem.Parent != null && dragItem.IsHidden;
				if(fDraggingFromHiddenItems) {
					if(dragItem.Parent != null) dragItem.OnItemRestoredCore();
				}
				OwnerControl.EndUpdate();
				if(fDraggingFromHiddenItems) UpdateCoreTreeView();
			}
		}
		#endregion
		DropType GetDropType(BaseLayoutItem source, LayoutTreeViewHitInfo hitInfo) {
			if(IsHiddenItemNode(hitInfo)) return DropType.Custom; 
			BaseLayoutItem target = hitInfo.Item;
			if(source == target || target == null) return DropType.Illegal;
			if(source.IsHidden & target.IsHidden) return DropType.Illegal;
			if(target.IsHidden) return DropType.Custom;
			if(target != null && !target.OptionsCustomization.CanDrop()) return DropType.Illegal;
			LayoutClassificationArgs classifySource = LayoutClassifier.Default.Classify(source);
			if(classifySource.IsGroup) {
				ContainsItemVisitor v = new ContainsItemVisitor(target);
				classifySource.Group.Accept(v);
				if(v.Contains) return DropType.Illegal;
			}
			if(classifySource.IsTabbedGroup) {
				ContainsItemVisitor v = new ContainsItemVisitor(target);
				classifySource.TabbedGroup.Accept(v);
				if(v.Contains) return DropType.Illegal;
			}
			LayoutClassificationArgs classifyTarget = LayoutClassifier.Default.Classify(target);
			if(classifyTarget.IsGroup) {
				if(classifyTarget.Group.Count == 0) return DropType.Inside;
				else {
					if(classifyTarget.Group.ParentTabbedGroup != null && classifySource.Group == null) return DropType.Illegal;
					if(target.Parent != null) return DropType.OutSide;
					else return DropType.Illegal;
				}
			}
			if(classifyTarget.IsTabbedGroup) {
				if(classifyTarget.TabbedGroup.TabPages.Count == 0) return DropType.Inside;
				else return DropType.OutSide;
			}
			return DropType.Custom;
		}
		private void DragFrameOn(TreeViewVisualizerFrame frame) {
			if(!frame.Visible && !RDPHelper.IsRemoteSession) {
				frame.Visible = true;
				Rectangle screenBounds = RectangleToScreen(ClientRectangle);
				if(frame.Bounds != screenBounds) frame.Bounds = screenBounds;
				frame.Update();
			}
		}
		protected virtual void SetCursorCross(DragDropItemCursor cursor) {
			if(cursor.Cursor != DevExpress.Utils.DragDrop.DragManager.DragRemoveCursor)
				cursor.Cursor = DevExpress.Utils.DragDrop.DragManager.DragRemoveCursor;
		}
		protected virtual void SetCursorIllegal(DragDropItemCursor cursor) {
			if(cursor.Cursor != Cursors.No) cursor.Cursor = Cursors.No;
		}
		protected virtual void SetCursorDefault(DragDropItemCursor cursor) {
			if(cursor.Cursor != Cursors.Default) cursor.Cursor = Cursors.Default;
		}
		private void DragFrameOff(TreeViewVisualizerFrame frame) {
			if(frame.Visible) {
				Form framePrevOwner = frame.Owner;
				Form parentForm = FindParentForm();
				if(parentForm != null && frame.Owner != parentForm) frame.Owner = parentForm;
				frame.Visible = false;
				SelectedNode = GetNodeByItem(Nodes[0], DragDropDispatcherFactory.Default.DragItem);
				if(DragDropDispatcherFactory.Default.DragItem != null) DragDropDispatcherFactory.Default.DragItem.Selected = true;
				frame.Owner = framePrevOwner;
			}
		}
		private Form FindParentForm() {
			Control parentForm = this.Parent;
			while(true) {
				if(parentForm is Form || parentForm == null) break;
				parentForm = parentForm.Parent;
			}
			return parentForm as Form;
		}
		private void DragCursorOn(DragDropItemCursor cursor) {
			if(!cursor.Visible && !RDPHelper.IsRemoteSession) {
				cursor.FrameOwner = this;
				cursor.DragItem = DragDropDispatcherFactory.Default.DragItem;
				cursor.Visible = true;
				cursor.Update();
			}
		}
		private void DragCursorOff(DragDropItemCursor cursor) {
			cursor.Visible = false;
		}
		private void DragCursorSetPos(DragDropItemCursor cursor, LayoutTreeViewHitInfo hitInfo) {
			if(cursor.Visible) {
				Rectangle cursorRect = cursor.GetItemCursorRect(hitInfo);
				cursor.Location = PointToScreen(cursorRect.Location);
				cursor.Size = cursorRect.Size;
			}
		}
		bool CanDropAt(BaseLayoutItem source, LayoutTreeViewHitInfo target) {
			return GetDropType(source, target) != DropType.Illegal;
		}
		int lockUpdateCounter = 0;
		void UpdateSelection(BaseLayoutItem item) {
			if(IsUpdating) return;
			BeginUpdateTreeView();
			try {
				if(Nodes.Count == 0) return;
				TreeNode toSelect = GetNodeByItem(Nodes[0], item);
				if(toSelect != null) {
					if(Control.ModifierKeys == (Keys.Control) || Control.ModifierKeys == (Keys.Shift) || Control.ModifierKeys == (Keys.Control| Keys.Shift))
						SetNodeSelection(toSelect, item.Selected);
					else SelectNode(toSelect);
				}
			}
			finally {
				EndUpdateTreeView();
				InvalidateScrollableContainer();
			}
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			OnTreeViewVisibleChanged();
		}
		protected virtual void UpdateCoreTreeView(object baseItem, bool includeHiddenItems) {
			if(!Visible || IsDisposingInProgress) return;
			BeginUpdate();
			var criteriaOperator = helper.GetLastCriteriaOperator(false);
			Nodes.Clear();
			ExtendedNodes.Clear();
			AddNode(baseItem, null);
			if(includeHiddenItems) AddHiddenItemsToTreeViewCore();
			ExpandAll();
			helper.ApplyFilter(criteriaOperator);
			EndUpdate();
			ScrollAfterUpdate();
		}
		void ScrollAfterUpdate() {
			if(!allowScrollOnTop) return;
			if(IsHandleCreated) TopNode = SelectedNode != null ? SelectedNode : (Nodes.Count > 0 ? Nodes[0] : null);
			if(TopNode != null && IsHandleCreated) {
				TopNode.EnsureVisible();
				InvalidateScrollableContainer();
			}
		}
		void InvalidateScrollableContainer() {
			if(ScrollableContainer != null && ScrollableContainer.ScrollHelper != null && IsHandleCreated)
				ScrollableContainer.ScrollHelper.UpdateScrollBars();
		}
		protected void AddHiddenItemsToTreeViewCore() {
			if(OwnerControl.HiddenItems == null || OwnerControl.DisposingFlag) return;
			string hiddenItemsNodeText = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.HiddenItemsNodeText);
			BaseLayoutItemTreeNode hiddenItemsTreeNode = new BaseLayoutItemTreeNode(hiddenItemsNodeText, null) {Tag = hiddenItemNodeString };
			AddNode(hiddenItemsTreeNode, null);
			foreach(BaseLayoutItem fixedItem in OwnerControl.HiddenItems.FixedItems) AddNode(fixedItem, hiddenItemsTreeNode);
			ArrayList nonFixedItems = new ArrayList(OwnerControl.HiddenItems);
			if(OwnerControl.HiddenItemsSortComparer != null && nonFixedItems.Count > 0) {
				nonFixedItems.Sort(OwnerControl.HiddenItemsSortComparer);
			}
			foreach(BaseLayoutItem item in nonFixedItems) AddNode(item, hiddenItemsTreeNode);
		}
		TreeViewRoles role = TreeViewRoles.LayoutTreeView;
		public TreeViewRoles Role {
			get { return role; }
			set { role = value; }
		}
		protected void BeginUpdateTreeView() { lockUpdateCounter++; }
		protected void EndUpdateTreeView() { lockUpdateCounter--; }
		protected bool IsUpdating { get { return lockUpdateCounter > 0; } }
		bool showHiddenItemsInTreeView = true;
		public bool ShowHiddenItemsInTreeView {
			get { return showHiddenItemsInTreeView; }
			set { showHiddenItemsInTreeView = value; }
		}
		bool updateInvoked = false;
		protected internal void UpdateCoreTreeView() {
			if(IsUpdating) return;
			if(OwnerControl != null && (OwnerControl.UndoManager != null && OwnerControl.UndoManager.IsUndoLocked || OwnerControl.CustomizationMenuManager != null && OwnerControl.CustomizationMenuManager.LockUpdate)) {
				if(!updateInvoked && OwnerControl.Control != null && OwnerControl.Control.IsHandleCreated) {
					updateInvoked = true;
					OwnerControl.Control.BeginInvoke(new MethodInvoker(UpdateCoreTreeView));
				}
				return;
			}
			BeginUpdateTreeView();
			updateInvoked = false;
			try {
				if(OwnerControl != null) {
					switch(Role) {
						case TreeViewRoles.LayoutTreeView: UpdateCoreTreeView(OwnerControl.RootGroup, ShowHiddenItemsInTreeView); break;
						case TreeViewRoles.ResizeTreeH: UpdateCoreTreeView(OwnerControl.RootGroup.Resizer.resultH, false); break;
						case TreeViewRoles.ResizeTreeV: UpdateCoreTreeView(OwnerControl.RootGroup.Resizer.resultV, false); break;
						case TreeViewRoles.SE_Tree:  break;
					}
				}
			}
			finally { EndUpdateTreeView(); }
		}
		protected virtual void OnTreeViewVisibleChanged() {
			UpdateCoreTreeView();
		}
		protected internal IComponent lastSelectedItem = null;
		internal bool allowScrollOnTop = true;
		protected override void OnSelectionChanged() {
			base.OnSelectionChanged();
			if(IsUpdating || IsDisposingInProgress) return;
			BeginUpdateTreeView();
			try {
				OwnerControl.SelectedChangedCount++;
				Accept(
					delegate(TreeNode node) {
						BaseLayoutItem item = (node is BaseLayoutItemTreeNode) ? ((BaseLayoutItemTreeNode)node).Item : null;
						if(item != null && item.Owner != null) {
							bool fSelected = Array.IndexOf(SelNodes, node) != -1;
							PerformSelection(item, fSelected);
						}
					}
				);
				OwnerControl.SelectedChangedFlag = true;
				OwnerControl.SelectedChangedCount--;
			}
			finally { EndUpdateTreeView(); }
		}
		void PerformSelection(BaseLayoutItem item, bool selected) {
			if(item.Selected == selected) return;
			item.StartChangeSelection();
			if(!IsControlPressed && !IsShiftPressed) {
				OwnerControl.RootGroup.ClearSelection();
			}
			if(!item.IsHidden) { 
				item.Selected = selected;
				if(selected && item.Owner != null) item.Owner.FocusHelper.PlaceItemIntoViewRestricted(item);
			}
			lastSelectedItem = item;
			item.EndChangeSelection();
		}
		protected virtual void OnTreeViewKeyDown(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.F2: Rename(); break;
				case Keys.Delete:
					if(LayoutControl.AllowHandleDeleteKey(OwnerControl)) OwnerControl.HideSelectedItems();
					else if(OwnerControl.DesignMode && OwnerControl is ILayoutDesignerMethods) {
						(OwnerControl as ILayoutDesignerMethods).RaiseDeleteSelectedItems(new DeleteSelectedItemsEventArgs((OwnerControl as LayoutControl).Root.SelectedItems));
					}
					break;
				case Keys.Escape: OwnerControl.SelectParentGroup(); break;
			}
		}
		private static void GetLayoutTypeAndInsertLocation(LayoutTreeViewHitInfo hitInfo, out InsertLocation location, out LayoutType type) {
			location = InsertLocation.Before;
			type = LayoutType.Horizontal;
			LayoutGroup group = hitInfo.Item as LayoutGroup;
			switch(hitInfo.HitType) {
				case LayoutTreeViewHitType.Left:
					break;
				case LayoutTreeViewHitType.Right:
					location = InsertLocation.After;
					break;
				case LayoutTreeViewHitType.Top:
					type = LayoutType.Vertical;
					break;
				case LayoutTreeViewHitType.Bottom:
					type = LayoutType.Vertical;
					location = InsertLocation.After;
					break;
			}
		}
		internal IDXTreeViewFilteringHelper helper;
		protected override DevExpress.Utils.Design.Internal.IDXTreeViewFilteringHelper CreateFilteringHelper(DevExpress.Utils.Design.DXTreeView treeView) {
			helper = base.CreateFilteringHelper(treeView);
			return helper;
		}
	}
}
