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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
namespace DevExpress.XtraGauges.Win.Wizard {
	[System.ComponentModel.ToolboxItem(false)]
	public class NavigationControl : Control {
		GaugeDesignerControl designerControlCore;
		internal Font Font1;
		Font Font2;
		Brush TextBrush1;
		Brush TextBrush2;
		Brush TextBrush3;
		Brush BackgroundBrush;
		NavigationControlInfo viewInfoCore;
		PageNavigationNodeCollection nodesCore;
		BorderPainter BorderPainter;
		public NavigationControl() {
			this.SetStyle(
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.SupportsTransparentBackColor,
				true);
			OnCreate();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				OnDispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnCreate() {
			this.viewInfoCore = new NavigationControlInfo(this);
			this.nodesCore = new PageNavigationNodeCollection();
		}
		protected virtual void OnDispose() {
			if(Nodes != null) {
				Nodes.Dispose();
				nodesCore = null;
			}
			BorderPainter = null;
			if(Font1 != null) {
				Font1.Dispose();
				Font1 = null;
			}
			if(Font2 != null) {
				Font2.Dispose();
				Font2 = null;
			}
			if(TextBrush1 != null) {
				TextBrush1.Dispose();
				TextBrush1 = null;
			}
			if(TextBrush2 != null) {
				TextBrush2.Dispose();
				TextBrush2 = null;
			}
		}
		internal void SetDesignerControl(GaugeDesignerControl designer) {
			this.designerControlCore = designer;
			this.BorderPainter = BorderHelper.GetPainter(DevExpress.XtraEditors.Controls.BorderStyles.NoBorder, LookAndFeel);
			Color bgColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Window);
			this.Font1 = new Font("Tahoma", 8.25f, FontStyle.Regular);
			this.Font2 = new Font("Tahoma", 8.25f, FontStyle.Underline);
			this.TextBrush1 = new SolidBrush(Color.Black);
			this.TextBrush2 = new SolidBrush(Color.Blue);
			this.TextBrush3 = new SolidBrush(Color.Gray);
			this.BackgroundBrush = new SolidBrush(bgColor);
			LayoutChanged();
		}
		protected GaugeDesignerControl DesignerControl {
			get { return designerControlCore; }
		}
		protected NavigationControlInfo ViewInfo {
			get { return viewInfoCore; }
		}
		protected UserLookAndFeel LookAndFeel {
			get { return (DesignerControl == null) ? UserLookAndFeel.Default : DesignerControl.LookAndFeel; }
		}
		public PageNavigationNodeCollection Nodes {
			get { return nodesCore; }
		}
		public void LayoutChanged() {
			UpdateNodes();
			ViewInfo.SetDirty();
			ViewInfo.CalcInfo(null, Bounds);
			Invalidate();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		protected void UpdateBeforePaint(GraphicsInfoArgs e) {
			UpdateNodes();
			if(!ViewInfo.IsReady) ViewInfo.CalcInfo(e.Graphics, Bounds);
		}
		protected void UpdateNodes() {
			foreach(PageNavigationNode n in Nodes) n.Accept(
					delegate(PageNavigationNode node) {
						if(node.Nodes.Count > 0) node.Image = node.Expanded ? UIHelper.ExpandCollapseImages[1] : UIHelper.ExpandCollapseImages[0];
						else if(node.Image == null) node.Image = UIHelper.ExpandCollapseImages[1];
						node.ButtonImage = IsNodeButtonAvail(node) ? UIHelper.UIActionImages[5] : null;
					}
				);
		}
		bool IsNodeButtonAvail(PageNavigationNode node) {
			return (node.Parent != null && node.Parent.Caption == "Elements" && node.LinkPage != null && !node.LinkPage.IsAllowed);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e)) {
				GraphicsInfoArgs ea = new GraphicsInfoArgs(cache, e.ClipRectangle);
				UpdateBeforePaint(ea);
				DrawBackground(ea);
				DrawNodes(ea);
			}
		}
		protected void DrawBackground(GraphicsInfoArgs e) {
			e.Graphics.FillRectangle(BackgroundBrush, e.Bounds);
			BorderObjectInfoArgs info = new BorderObjectInfoArgs(e.Cache, null, ViewInfo.Rects.Bounds, ObjectState.Normal);
			BorderPainter.DrawObject(info);
		}
		protected void DrawNodes(GraphicsInfoArgs e) {
			foreach(PageNavigationNode n in Nodes) n.Accept(
					delegate(PageNavigationNode node) {
						DrawNodeIcon(e, node);
						DrawNodeText(e, node);
						DrawNodeButton(e, node);
					}
				);
		}
		protected void DrawNodeButton(GraphicsInfoArgs e, PageNavigationNode node) {
			if(node.Info.Button.IsEmpty) return;
			e.Graphics.DrawImageUnscaled(node.ButtonImage, node.Info.Button);
		}
		protected void DrawNodeIcon(GraphicsInfoArgs e, PageNavigationNode node) {
			if(node.Info.Icon.IsEmpty) return;
			e.Graphics.DrawImageUnscaled(node.Image, node.Info.Icon);
		}
		protected void DrawNodeText(GraphicsInfoArgs e, PageNavigationNode node) {
			if(node.Info.Text.IsEmpty) return;
			Font font = Font1;
			Brush brush = TextBrush3;
			if((node.State & ObjectState.Disabled) == 0) {
				bool selectedOrHot = (((node.State & ObjectState.Hot) != 0) ||
					(node.LinkPage != null && node.LinkPage.Owner != null && node.LinkPage.Owner.SelectedPage == node.LinkPage));
				font = selectedOrHot ? Font2 : Font1;
				brush = selectedOrHot ? TextBrush2 : TextBrush1;
			}
			e.Graphics.DrawString(node.Caption, font, brush, node.Info.Text);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ViewInfo.SetHotTrackedInfo(NavigationControlHitInfo.Empty);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			NavigationControlHitInfo downInfo = ViewInfo.CalcHitInfo(e.Location);
			if(e.Button == MouseButtons.Left) {
				ViewInfo.SetPressedInfo(downInfo.InNode ? downInfo : NavigationControlHitInfo.Empty);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			NavigationControlHitInfo upInfo = ViewInfo.CalcHitInfo(e.Location);
			if(e.Button == MouseButtons.Left) {
				if(ViewInfo.States.Pressed.HitTest == upInfo.HitTest) DoClickAction(upInfo);
			}
			ViewInfo.SetPressedInfo(NavigationControlHitInfo.Empty);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			NavigationControlHitInfo moveInfo = ViewInfo.CalcHitInfo(e.Location);
			if(ViewInfo.States.Pressed.HitTest != moveInfo.HitTest) {
				ViewInfo.SetHotTrackedInfo(moveInfo.InNode ? moveInfo : NavigationControlHitInfo.Empty);
			}
		}
		protected void DoClickAction(NavigationControlHitInfo hitInfo) {
			if(hitInfo.InNodeIcon) OnNodeIconClick(hitInfo.HitNode);
			if(hitInfo.InNodeText) OnNodeTextClick(hitInfo.HitNode);
			if(hitInfo.InNodeButton) OnNodeButtonClick(hitInfo.HitNode);
		}
		protected void OnNodeButtonClick(PageNavigationNode node) {
			BaseGaugeDesignerPage page = node.LinkPage;
			BaseElement<IRenderableElement> newElement = null;
			if(page == null || !page.ProcessElementAddNewCommand(out newElement)) return;
			if(newElement != null) {
				DesignerControl.SetSelectedPage(page);
				page.UpdateContent();
				UpdateContent();
			}
		}
		protected void OnNodeIconClick(PageNavigationNode node) {
			ChangeNodeExpandedStateCore(node);
		}
		protected void ChangeNodeExpandedStateCore(PageNavigationNode node) {
			node.Expanded = !node.Expanded;
			LayoutChanged();
		}
		protected void OnNodeTextClick(PageNavigationNode node) {
			if(node.LinkPage != null) node.LinkPage.Owner.SetSelectedPage(node.LinkPage);
			else ChangeNodeExpandedStateCore(node);
		}
		protected internal void UpdateContent() {
			ViewInfo.SetDirty();
			Invalidate();
		}
	}
	public class NavigationControlInfo : BaseViewInfo {
		NavigationControl ownerCore;
		NavigationControlViewRects rectsCore;
		NavigationControlViewStates statesCore;
		public NavigationControlInfo(NavigationControl owner)
			: base() {
			this.ownerCore = owner;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.rectsCore = new NavigationControlViewRects();
			this.statesCore = new NavigationControlViewStates();
		}
		protected override void OnDispose() {
			this.ownerCore = null;
			this.rectsCore = null;
			this.statesCore = null;
			base.OnDispose();
		}
		protected NavigationControl Owner {
			get { return ownerCore; }
		}
		public NavigationControlViewRects Rects {
			get { return rectsCore; }
		}
		public NavigationControlViewStates States {
			get { return statesCore; }
		}
		protected override void CalcViewRects(Rectangle bounds) {
			Rects.Clear();
			Rects.Bounds = bounds;
			int i = 0;
			int left = int.MaxValue;
			int right = int.MinValue;
			int top = int.MaxValue;
			int bottom = int.MinValue;
			Size nodeSize = new Size(18, 18);
			foreach(PageNavigationNode n in Owner.Nodes) n.Accept(
				delegate(PageNavigationNode node) {
					bool nodeHasRect = CalcNodeHasRect(node);
					Rectangle nodeRect = Rectangle.Empty;
					if(nodeHasRect) {
						nodeRect = CalcNodeBounds(node, bounds, i++, nodeSize);
						left = Math.Min(left, nodeRect.Left);
						top = Math.Min(top, nodeRect.Top);
						right = Math.Max(right, nodeRect.Right);
						bottom = Math.Max(bottom, nodeRect.Bottom);
					}
					node.Info.Calc(nodeRect, nodeSize);
					if(nodeHasRect && node.ButtonImage != null) node.Info.CalcButton(Graphics, Owner.Font1);
				}
			);
			Rects.Nodes = new Rectangle(left, top, right - left, bottom - top);
		}
		bool CalcNodeHasRect(PageNavigationNode node) {
			return (node.Parent == null) ||
						(node.Parent != null && node.Parent.Expanded &&
						((node.LinkPage != null) ? !node.LinkPage.IsHidden : true)
					);
		}
		Rectangle CalcNodeBounds(PageNavigationNode node, Rectangle bounds, int i, Size nodeSize) {
			return new Rectangle(
					bounds.Left + node.Level * nodeSize.Width,
					bounds.Top + 2 + nodeSize.Height * i,
					bounds.Width - node.Level * nodeSize.Width,
					nodeSize.Height
				);
		}
		protected override void CalcViewStates() {
			foreach(PageNavigationNode n in Owner.Nodes) n.Accept(
				delegate(PageNavigationNode node) {
					node.State = ObjectState.Normal;
					node.ButtonState = ObjectState.Normal;
				}
			);
			CheckAndSetState(States.Pressed, ObjectState.Pressed);
			CheckAndSetState(States.HotTracked, ObjectState.Hot);
			foreach(PageNavigationNode n in Owner.Nodes) n.Accept(
				delegate(PageNavigationNode node) {
					if(node.LinkPage != null && !node.LinkPage.IsAllowed) {
						node.State = ObjectState.Disabled;
					}
				}
			);
			InvalidateCursor(States.HotTracked);
		}
		protected void CheckAndSetState(NavigationControlHitInfo hitInfo, ObjectState state) {
			if(hitInfo != null && hitInfo.InNode) {
				hitInfo.HitNode.State = state;
				if(hitInfo.InNodeButton) hitInfo.HitNode.ButtonState = state;
			}
		}
		public NavigationControlHitInfo CalcHitInfo(Point pt) {
			NavigationControlHitInfo hitInfo = new NavigationControlHitInfo(pt);
			if(hitInfo.CheckAndSetHitTest(Rects.Bounds, NavigationControlHitTest.Bounds)) {
				bool inNodes = hitInfo.CheckAndSetHitTest(Rects.Nodes, NavigationControlHitTest.Nodes);
				if(inNodes) {
					foreach(PageNavigationNode n in Owner.Nodes) n.Accept(
						delegate(PageNavigationNode node) {
							bool inNode = hitInfo.CheckAndSetHitTest(node.Info.NodeRect, NavigationControlHitTest.Node);
							if(inNode) {
								hitInfo.SetHitNode(node);
								hitInfo.CheckAndSetHitTest(node.Info.Icon, NavigationControlHitTest.NodeIcon);
								hitInfo.CheckAndSetHitTest(node.Info.Text, NavigationControlHitTest.NodeText);
								hitInfo.CheckAndSetHitTest(node.Info.Button, NavigationControlHitTest.NodeButton);
							}
						}
						);
				}
			}
			return hitInfo;
		}
		public void SetPressedInfo(NavigationControlHitInfo hitInfo) {
			NavigationControlHitInfo prevHitInfo = States.Pressed;
			if(hitInfo.IsEquals(prevHitInfo)) return;
			States.HotTracked = NavigationControlHitInfo.Empty;
			States.Pressed = hitInfo;
			CalcViewStates();
			InvalidateHitObject(prevHitInfo);
			InvalidateHitObject(States.Pressed);
		}
		public void SetHotTrackedInfo(NavigationControlHitInfo hitInfo) {
			NavigationControlHitInfo prevHitInfo = States.HotTracked;
			if(hitInfo.IsEquals(prevHitInfo)) return;
			States.HotTracked = hitInfo;
			CalcViewStates();
			InvalidateHitObject(prevHitInfo);
			InvalidateHitObject(States.HotTracked);
		}
		protected void InvalidateCursor(NavigationControlHitInfo hitInfo) {
			Owner.Cursor = hitInfo.IsEmpty ? Cursors.Default :
				((hitInfo.HitNode != null &&
					(((hitInfo.HitNode.State & ObjectState.Hot) != 0) || ((hitInfo.HitNode.ButtonState & ObjectState.Hot) != 0))
					) ? Cursors.Hand : Cursors.Default);
		}
		protected void InvalidateHitObject(NavigationControlHitInfo hitInfo) {
			if(hitInfo != null) Owner.Invalidate(hitInfo.HitRect);
		}
	}
	public class NavigationControlViewRects {
		public Rectangle Bounds;
		public Rectangle Nodes;
		public void Clear() {
			Bounds = Nodes =
			Rectangle.Empty;
		}
	}
	public enum NavigationControlHitTest {
		None, Bounds, Nodes, Node, NodeIcon, NodeText, NodeButton
	}
	public class NavigationControlViewStates {
		public NavigationControlHitInfo HotTracked;
		public NavigationControlHitInfo Pressed;
		public NavigationControlViewStates() {
			HotTracked = NavigationControlHitInfo.Empty;
			Pressed = NavigationControlHitInfo.Empty;
		}
	}
	public class NavigationControlHitInfo {
		public static readonly NavigationControlHitInfo Empty;
		static NavigationControlHitInfo() {
			Empty = new NavigationControlEmptyHitInfo();
		}
		class NavigationControlEmptyHitInfo : NavigationControlHitInfo {
			public NavigationControlEmptyHitInfo() : base(new Point(-10000, -10000)) { }
		}
		NavigationControlHitTest hitTestCore;
		Rectangle hitRectCore;
		Point hitPointCore;
		PageNavigationNode nodeCore;
		public NavigationControlHitInfo(Point hitPoint) {
			this.hitPointCore = hitPoint;
			this.hitTestCore = NavigationControlHitTest.None;
		}
		public Rectangle HitRect {
			get { return hitRectCore; }
		}
		public PageNavigationNode HitNode {
			get { return nodeCore; }
		}
		public Point HitPoint {
			get { return hitPointCore; }
		}
		public NavigationControlHitTest HitTest {
			get { return hitTestCore; }
		}
		public bool IsEmpty {
			get { return this is NavigationControlEmptyHitInfo; }
		}
		public void SetHitNode(PageNavigationNode node) {
			this.nodeCore = node;
		}
		public bool CheckAndSetHitTest(Rectangle bounds, NavigationControlHitTest hitTest) {
			bool contains = !bounds.IsEmpty && bounds.Contains(HitPoint);
			if(contains) {
				hitRectCore = bounds;
				hitTestCore = hitTest;
			}
			return contains;
		}
		public bool InBounds {
			get { return (HitTest == NavigationControlHitTest.Bounds) || InNode; }
		}
		public bool InNodes {
			get { return (HitTest == NavigationControlHitTest.Nodes) || InNode; }
		}
		public bool InNode {
			get { return (HitTest == NavigationControlHitTest.Node) || InNodeText || InNodeIcon || InNodeButton; }
		}
		public bool InNodeIcon {
			get { return (HitTest == NavigationControlHitTest.NodeIcon); }
		}
		public bool InNodeText {
			get { return (HitTest == NavigationControlHitTest.NodeText); }
		}
		public bool InNodeButton {
			get { return (HitTest == NavigationControlHitTest.NodeButton); }
		}
		public virtual bool IsEquals(NavigationControlHitInfo hitInfo) {
			return hitInfo != null && !hitInfo.IsEmpty && this.HitTest == hitInfo.HitTest && this.HitNode == hitInfo.HitNode;
		}
	}
	public class PageNavigationNode : BaseObject, ISupportAcceptOrder, ISupportVisitor<PageNavigationNode> {
		PageNavigationNodeCollection nodesCore;
		PageNavigationNodeInfo infoCore;
		PageNavigationNode parentCore;
		BaseGaugeDesignerPage linkPageCore;
		bool expandedCore = true;
		int nodeIndexCore;
		string captionCore;
		Image imageCore;
		Image butonImageCore;
		ObjectState stateCore;
		ObjectState btnStateCore;
		public PageNavigationNode(BaseGaugeDesignerPage linkPage, string caption, Image img)
			: base() {
			this.linkPageCore = linkPage;
			this.captionCore = caption;
			this.imageCore = img;
		}
		protected override void OnCreate() {
			this.parentCore = null;
			this.nodesCore = new PageNavigationNodeCollection();
			this.infoCore = new PageNavigationNodeInfo(this);
			Nodes.CollectionChanged += OnNodesChanged;
		}
		protected override void OnDispose() {
			this.linkPageCore = null;
			if(Nodes != null) {
				Nodes.CollectionChanged -= OnNodesChanged;
				Nodes.Dispose();
				nodesCore = null;
			}
			if(Info != null) {
				Info.Dispose();
				infoCore = null;
			}
			this.parentCore = null;
			this.imageCore = null;
			this.captionCore = null;
		}
		void OnNodesChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<PageNavigationNode> ea) {
			switch(ea.ChangedType) {
				case ElementChangedType.ElementAdded: OnNodeAdded(ea.Element); break;
				case ElementChangedType.ElementRemoved: OnNodeRemoved(ea.Element); break;
			}
		}
		void OnNodeAdded(PageNavigationNode node) {
			node.parentCore = this;
		}
		void OnNodeRemoved(PageNavigationNode node) {
			node.parentCore = null; ;
		}
		public PageNavigationNode Parent {
			get { return parentCore; }
		}
		public BaseGaugeDesignerPage LinkPage {
			get { return linkPageCore; }
		}
		protected int NodeIndex {
			get { return nodeIndexCore; }
			set { nodeIndexCore = value; }
		}
		public ObjectState State {
			get { return stateCore; }
			set { stateCore = value; }
		}
		public ObjectState ButtonState {
			get { return btnStateCore; }
			set { btnStateCore = value; }
		}
		public bool Expanded {
			get { return expandedCore; }
			set {
				if(Expanded == value || Nodes.Count == 0) return;
				expandedCore = value;
			}
		}
		public int Level {
			get { return (Parent == null) ? 0 : Parent.Level + 1; }
		}
		public PageNavigationNodeInfo Info {
			get { return infoCore; }
		}
		public PageNavigationNodeCollection Nodes {
			get { return nodesCore; }
		}
		public string Caption {
			get { return captionCore; }
		}
		public Image Image {
			get { return imageCore; }
			set { imageCore = value; }
		}
		public Image ButtonImage {
			get { return butonImageCore; }
			set { butonImageCore = value; }
		}
		public void Accept(IVisitor<PageNavigationNode> visitor) {
			if(visitor == null) return;
			visitor.Visit(this);
			foreach(PageNavigationNode node in Nodes) node.Accept(visitor);
		}
		public void Accept(VisitDelegate<PageNavigationNode> visit) {
			if(visit == null) return;
			visit(this);
			foreach(PageNavigationNode node in Nodes) node.Accept(visit);
		}
		int ISupportAcceptOrder.AcceptOrder {
			get { return NodeIndex; }
			set { NodeIndex = value; }
		}
	}
	public class PageNavigationNodeCollection :
		BaseChangeableList<PageNavigationNode> {
	}
	public class PageNavigationNodeInfo : BaseObject {
		PageNavigationNode nodeCore;
		public PageNavigationNodeInfo(PageNavigationNode node)
			: base() {
			this.nodeCore = node;
		}
		protected override void OnCreate() {
			NodeRect = Icon = Text = Button = Rectangle.Empty;
		}
		protected override void OnDispose() {
			this.nodeCore = null;
		}
		public Rectangle NodeRect;
		public Rectangle Icon;
		public Rectangle Text;
		public Rectangle Button;
		protected PageNavigationNode Node {
			get { return nodeCore; }
		}
		public void Clear() {
			NodeRect = Icon = Text = Button = Rectangle.Empty;
		}
		public void Calc(Rectangle bounds, Size nodeSize) {
			Clear();
			if(bounds.IsEmpty) return;
			NodeRect = bounds;
			int elementsTop = NodeRect.Top + (NodeRect.Height - nodeSize.Height) / 2;
			Icon = new Rectangle(NodeRect.Left + 2, elementsTop, 15, 15);
			int textLeft = NodeRect.Left + 2 + Icon.Width + 3;
			Text = new Rectangle(textLeft, elementsTop, NodeRect.Right - textLeft, 16);
		}
		public void CalcButton(Graphics g, Font font) {
			Bitmap bmp = null;
			if(g == null) {
				bmp = new Bitmap(200, 100);
				g = Graphics.FromImage(bmp);
			}
			SizeF textSize = g.MeasureString(Node.Caption, font);
			Size imageSize = Node.ButtonImage.Size;
			Button = new Rectangle(Text.Left + (int)textSize.Width + 2, Text.Top + (Text.Height - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
			if(bmp != null) {
				bmp.Dispose();
				g.Dispose();
			}
		}
	}
}
