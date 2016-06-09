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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design.Internal;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ScrollHelpers;
using System.Linq;
namespace DevExpress.Utils.Design {
	public class TreeViewGetNodeEditTextEventArgs : EventArgs {
		TreeNode node;
		string text;
		bool modified = false;
		public TreeViewGetNodeEditTextEventArgs(TreeNode node) {
			this.node = node;
			this.text = node.Text;
		}
		public string Text {
			get { return text; }
			set {
				if(value == null) value = string.Empty;
				if(Text == value) return;
				text = value;
				modified = true;
			}
		}
		public TreeNode Node { get { return node; } }
		internal bool Modified { get { return modified; } }
	}
	public delegate void TreeViewGetNodeEditTextEventHandler(object sender, TreeViewGetNodeEditTextEventArgs e);
	public interface IImageProvider {
		Image Image { get; }
		bool IsImageExist { get; }
	}
	public class ScrollBarUtilsAPIHelper : ScrollBarAPIHelper {
		protected override bool IsTextBoxMaskBox(Control control) {
			return false;
		}
	}
	public interface IDXTreeViewContainer {
		void OnWndProc(ref Message msg);
		void UpdateScrollBars();
	}
	[ToolboxItem(false)]
	public class DXScrollableTreeView : ControlBase, IDXTreeViewContainer {
		public DXScrollableTreeView(DXTreeView tree) {
			this.MaskTreeView = tree;
			this.ScrollHelper = new ScrollBarUtilsAPIHelper();
			this.ScrollHelper.Init(MaskTreeView, this);
			this.ScrollHelper.LookAndFeel = tree.LookAndFeel;
			this.Dock = DockStyle.Fill;
		}
		protected internal void OnLookAndFeelStyleChanged() {
			if(MaskTreeView == null) return;
			ScrollHelper.LookAndFeel = MaskTreeView.LookAndFeel;
		}
		public ScrollBarUtilsAPIHelper ScrollHelper { get; set; }
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			RefreshContainerState();
		}
		public virtual void RefreshContainerState() {
			Location = MaskTreeView.Location;
			Size = MaskTreeView.Size;
			Dock = MaskTreeView.Dock;
			Anchor = MaskTreeView.Anchor;
			Controls.Add(MaskTreeView);
			MaskTreeView.Dock = DockStyle.Fill;
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			ScrollHelper.UpdateScrollBars();
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			ScrollHelper.UpdateScrollBars();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ScrollHelper.Dispose();
			}
			base.Dispose(disposing);
		}
		protected virtual DXTreeView CreateMaskTreeView() {
			if(MaskTreeView != null)
				return MaskTreeView;
			return new DXTreeView(this);
		}
		public DXTreeView MaskTreeView {
			get;
			private set;
		}
		#region IDXTreeViewContainer
		void IDXTreeViewContainer.OnWndProc(ref Message msg) {
			ScrollHelper.WndProc(ref msg);
		}
		void IDXTreeViewContainer.UpdateScrollBars() {
			ScrollHelper.UpdateScrollBars();
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class DXDemoTreeView : DXTreeView {
		public DXDemoTreeView()
			: this(null) {
		}
		public DXDemoTreeView(IDXTreeViewContainer container)
			: base(container) {
			AllowSkinning = true;
		}
	}
	[ToolboxItem(false)]
	public class DXTreeView : TreeView, ISearchControlClient {
		Hashtable selectedNodes;
		DXTreeSelectionMode selectionMode;
		bool repaintingSelection = false, allowDrag = false, dragAutoScroll, dragAutoExpand, allowHScrollBar;
		public event TreeViewGetNodeEditTextEventHandler GetNodeEditText;
		public event EventHandler SelectionChanged, DragNodeStart;
		public event DragEventHandler DragNodeEnd;
		public event TreeViewGetDragObjectEventHandler DragNodeGetObject;
		public event CancelEventHandler DragNodeAllow;
		int lockSelection = 0;
		Timer mouseUpTimer, dragAutoScrollTimer, dragAutoExpandTimer;
		TreeNode anchorNode;
		public event TreeViewCancelEventHandler AllowMultiSelectNode;
		const int AutoExpandInterval = 1000, AutoScrollTimerStartInterval = 400, AutoScrollHeight = 10;
		protected IDXTreeViewContainer containerCore { get; private set; }
		DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel lookAndFeelCore;
		public DXTreeView()
			: this(null) {
		}
		public DXTreeView(IDXTreeViewContainer container) {
			this.allowHScrollBar = false;
			this.dragAutoScroll = true;
			this.dragAutoExpand = true;
			this.dragAutoScrollTimer = new Timer();
			this.dragAutoScrollTimer.Tick += new EventHandler(OnAutoScrollTimer);
			this.dragAutoExpandTimer = new Timer();
			this.dragAutoExpandTimer.Tick += new EventHandler(OnAutoExpandTimer);
			this.dragAutoExpandTimer.Interval = AutoExpandInterval;
			this.mouseUpTimer = new Timer();
			this.mouseUpTimer.Interval = 50;
			this.mouseUpTimer.Tick += new EventHandler(OnMouseUpTimerTick);
			this.anchorNode = null;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer | ControlStyles.UserPaint, true);
			this.selectedNodes = new Hashtable();
			this.selectionMode = DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.DefaultExpandCollapseButtonOffset = 5;
			this.containerCore = container;
			lookAndFeelCore = new LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			LookAndFeel.StyleChanged += OnLookAndFeelStyleChanged;
			AllowSkinning = false;
			searchHelper = CreateFilteringHelper(this);
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			if(ScrollableContainer != null) ScrollableContainer.OnLookAndFeelStyleChanged();
			Invalidate();
			Update();
		}
		protected internal UserLookAndFeel LookAndFeel { get { return lookAndFeelCore; } }
		#region Theming Support
		[DefaultValue(false)]
		public bool AllowSkinning { get; set; }
		protected virtual bool UseThemes {
			get { return AllowSkinning; }
		}
		protected override void WndProc(ref Message msg) {
			if(!UseThemes) {
				base.WndProc(ref msg);
				return;
			}
			switch(msg.Msg) {
				case MSG.WM_ERASEBKGND:
					msg.Result = new IntPtr(1);
					return;
				case MSG.WM_NCCALCSIZE:
					OnWmNcCalcsize(ref msg);
					break;
				case MSG.WM_NCPAINT:
					base.WndProc(ref msg);
					int flags = NativeMethods.DC.DCX_CACHE | NativeMethods.DC.DCX_WINDOW | NativeMethods.DC.DCX_VALIDATE;
					IntPtr hrgnCopy = IntPtr.Zero;
					if(msg.WParam != new IntPtr(1)) {
						flags |= NativeMethods.DC.DCX_INTERSECTRGN;
						hrgnCopy = NativeMethods.CreateRectRgn(0, 0, 1, 1);
						NativeMethods.CombineRgn(hrgnCopy, msg.WParam, IntPtr.Zero, NativeMethods.RGN_COPY);
					}
					IntPtr hDC = NativeMethods.GetDCEx(Handle, hrgnCopy, flags);
					try {
						DoNCPaint(msg, hDC);
					}
					finally { NativeMethods.ReleaseDC(Handle, hDC); }
					msg.Result = IntPtr.Zero;
					return;
			}
			base.WndProc(ref msg);
			if(containerCore != null) containerCore.OnWndProc(ref msg);
		}
		protected void DoNCPaint(Message msg, IntPtr dc) {
			if(BorderStyle == System.Windows.Forms.BorderStyle.None) return;
			NativeMethods.RECT boundsRect = new NativeMethods.RECT();
			NativeMethods.GetWindowRect(msg.HWnd, ref boundsRect);
			Rectangle bounds = Rectangle.FromLTRB(boundsRect.Left, boundsRect.Top, boundsRect.Right, boundsRect.Bottom);
			bounds = RectangleToClient(bounds);
			bounds.Offset(ClientRectDeflate, ClientRectDeflate);
			using(Graphics graphics = Graphics.FromHdc(dc)) {
				Rectangle clip = bounds;
				clip.Inflate(-ClientRectDeflate, -ClientRectDeflate);
				Region region = new Region(bounds);
				region.Exclude(clip);
				graphics.SetClip(region, CombineMode.Replace);
				graphics.Clear(BackColor);
				OnNcPaint(new PaintEventArgs(graphics, bounds));
			}
		}
		[System.Security.SecuritySafeCritical]
		protected virtual void OnWmNcCalcsize(ref Message msg) {
			if(msg.WParam != IntPtr.Zero || BorderStyle == System.Windows.Forms.BorderStyle.None)
				return;
			NativeMethods.RECT rect = (NativeMethods.RECT)Marshal.PtrToStructure(msg.LParam, typeof(NativeMethods.RECT));
			rect.Inflate(-ClientRectDeflate, -ClientRectDeflate);
			Marshal.StructureToPtr(rect, msg.LParam, false);
			msg.Result = IntPtr.Zero;
		}
		protected virtual void OnNcPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				DrawNcBackground(cache);
				DrawBorder(cache);
			}
		}
		protected virtual void DrawNcBackground(GraphicsCache cache) {
			for(int i = ClientRectDeflate; i > 0; i--) {
				Rectangle rect = cache.PaintArgs.ClipRectangle;
				rect.Inflate(-i, -i);
				DrawBackgroundCore(cache, rect);
			}
		}
		protected virtual void DrawBorder(GraphicsCache cache) {
			Rectangle bounds = cache.PaintArgs.ClipRectangle;
			BorderPainter.DrawObject(new BorderObjectInfoArgs(cache, null, bounds));
		}
		protected virtual int ClientRectDeflate {
			get { return 2; }
		}
		protected virtual BorderPainter BorderPainter {
			get { return BorderHelper.GetPainter(BorderStyles.Default, LookAndFeel); }
		}
		protected virtual bool ShouldProcessMouseMove(Message msg) {
			NativeMethods.POINT pt = new NativeMethods.POINT();
			int pos = GetInt(msg.LParam);
			pt.X = pos & 0x0000FFFF;
			pt.Y = pos >> 16;
			return NativeMethods.DragDetect(Handle, pt);
		}
		static int GetInt(IntPtr ptr) {
			return (IntPtr.Size == 8) ? (int)(ptr.ToInt64() & 0x00000000FFFFFFFFL) : ptr.ToInt32();
		}
		protected virtual Rectangle CalcClipRectangle(NativeMethods.RECT rect) {
			return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			OnContainerChangingCore();
		}
		bool containerChanging = false;
		protected DXScrollableTreeView ScrollableContainer { get; private set; }
		protected virtual void OnContainerChangingCore() {
			if(DesignMode || !UseThemes || Parent == null || Parent is DXScrollableTreeView || containerChanging)
				return;
			containerChanging = true;
			try {
				Control parent = Parent;
				parent.Controls.Remove(this);
				ScrollableContainer = new DXScrollableTreeView(this);
				containerCore = ScrollableContainer;
				parent.Controls.Add(ScrollableContainer);
			}
			finally {
				containerChanging = false;
			}
		}
		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e) {
			base.OnBeforeCollapse(e);
			BeginUpdate();
		}
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e) {
			base.OnBeforeExpand(e);
			BeginUpdate();
		}
		protected override void OnAfterCollapse(TreeViewEventArgs e) {
			base.OnAfterCollapse(e);
			EndUpdate();
			if(containerCore != null) containerCore.UpdateScrollBars();
		}
		protected override void OnAfterExpand(TreeViewEventArgs e) {
			base.OnAfterExpand(e);
			EndUpdate();
			if(containerCore != null) containerCore.UpdateScrollBars();
		}
		protected SelectionInfo SelectionInfoCore { get; private set; }
		protected override void OnAfterSelect(TreeViewEventArgs e) {
			base.OnAfterSelect(e);
			if(SelectionInfoCore != null && SelectedNode != null) {
				SelectionInfoCore.Target = SelectedNode;
			}
		}
		protected virtual TreeViewDrawMode GetDrawModeOption() {
			return TreeViewDrawMode.OwnerDrawAll;
		}
		protected override void OnPaint(PaintEventArgs e) {
			SetClip(e);
			CheckControlStyle();
			try {
				base.OnPaint(e);
				using(GraphicsCache cache = new GraphicsCache(e)) {
					DrawBackground(cache, Nodes);
					DrawNodes(cache, Nodes);
				}
			}
			finally {
				RestoreClip(e);
			}
			if(SelectionInfoCore != null && SelectionInfoCore.IsReady)
				SelectionInfoCore = null;
		}
		void CheckControlStyle() {
			bool onStyleChanged = false;
			if(UseThemes && !GetStyle(ControlStyles.UserPaint)) {
				onStyleChanged = true;
				SetStyle(ControlStyles.UserPaint, true);
			}
			if(!UseThemes && GetStyle(ControlStyles.UserPaint)) {
				onStyleChanged = true;
				SetStyle(ControlStyles.UserPaint, false);
			}
			if(onStyleChanged)
				UpdateStyles();
		}
		protected virtual void DrawBackground(GraphicsCache cache, TreeNodeCollection nodes) {
			Rectangle bounds = CalcBackgroundBounds(Nodes);
			DrawBackgroundCore(cache, bounds);
		}
		protected virtual Rectangle CalcBackgroundBounds(TreeNodeCollection nodes) {
			Rectangle res = ClientRectangle;
			res.Y = CalcBackgroundTop(Nodes);
			return res;
		}
		protected virtual int CalcBackgroundTop(TreeNodeCollection nodes) {
			int res = 0;
			foreach(TreeNode node in nodes) {
				if(node.IsVisible && node.Bounds.Bottom > res)
					res = node.Bounds.Bottom;
				int temp = CalcBackgroundTop(node.Nodes);
				if(temp > res) res = temp;
			}
			return res;
		}
		Region clipRegion = null;
		protected virtual void SetClip(PaintEventArgs e) {
			clipRegion = e.Graphics.Clip;
			e.Graphics.SetClip(GetClipRegion(e.Graphics, e.ClipRectangle), CombineMode.Replace);
		}
		protected virtual Region GetClipRegion(Graphics graphics, Rectangle bounds) {
			if(SelectionInfoCore != null)
				return GetRestrictedClipRegion(graphics);
			return GetDefaultClipRegion(bounds);
		}
		protected virtual Region GetDefaultClipRegion(Rectangle bounds) {
			return new Region(bounds);
		}
		protected virtual Region GetRestrictedClipRegion(Graphics graphics) {
			Region region = new Region();
			region.Union(GetNodeBackgroundBounds(graphics, SelectionInfoCore.Source));
			region.Union(GetNodeBackgroundBounds(graphics, SelectionInfoCore.Target));
			return region;
		}
		protected virtual void RestoreClip(PaintEventArgs e) {
			e.Graphics.SetClip(clipRegion, CombineMode.Replace);
		}
		protected virtual void DrawNodes(GraphicsCache cache, TreeNodeCollection nodes) {
			for(int i = 0; i < nodes.Count; i++) {
				TreeNode node = nodes[i];
				DrawNodes(cache, node.Nodes);
				DrawNodeCore(cache, node);
			}
		}
		protected virtual void DrawNodeCore(GraphicsCache cache, TreeNode node) {
			if(!ShouldDrawNode(node))
				return;
			OnDrawNodeBackground(cache, node);
			OnDrawNodeImage(cache, node);
			OnDrawNodeCheckBox(cache, node);
			OnDrawNodeText(cache, node);
			OnDrawNodeGlyph(cache, node);
		}
		protected virtual Rectangle GetNodeBounds(Graphics graphics, TreeNode node) {
			Rectangle bounds = node.Bounds;
			Size size = TextUtils.GetStringSize(graphics, GetNodeText(node), Font, StringFormat.GenericDefault);
			return new Rectangle(bounds.X, bounds.Y, Math.Max(bounds.Width, size.Width), Math.Max(bounds.Height, size.Height));
		}
		protected virtual string GetNodeText(TreeNode node) {
			return node.Text.Replace(Environment.NewLine, string.Empty);
		}
		protected virtual Size GetNodeSize(Graphics graphics, TreeNode node) {
			Rectangle bounds = GetNodeBounds(graphics, node);
			return bounds.Size;
		}
		protected virtual bool ShouldDrawNode(TreeNode node) {
			return node.IsVisible && !node.Bounds.IsEmpty;
		}
		protected virtual void DrawBackgroundCore(GraphicsCache cache, Rectangle bounds) {
			Color color = CommonSkins.GetSkin(LookAndFeel).GetSystemColor(SystemColors.Window);
			using(SolidBrush br = new SolidBrush(color)) {
				cache.FillRectangle(br, bounds);
			}
		}
		#region Node Text
		protected virtual void OnDrawNodeText(GraphicsCache cache, TreeNode node) {
			Rectangle bounds = CalcTextBounds(cache, node);
			using(SolidBrush brush = new SolidBrush(GetNodeTextColor(node))) {
				using(StringFormat format = new StringFormat()) {
					format.LineAlignment = StringAlignment.Center;
					cache.DrawString(GetNodeText(node), GetNodeTextFont(node), brush, bounds, format);
				}
			}
		}
		protected virtual Rectangle CalcTextBounds(GraphicsCache cache, TreeNode node) {
			Point loc = GetParentNodeLocation(node);
			int offset = IsNodeImageExist(node) ? DefaultNodeImageSize.Width + DistanceBetweenImageAndText : 0;
			if(node.Parent != null && ShouldDrawCheck(node.Parent) && !ShouldDrawCheck(node))
				offset -= DefaultNodeCheckSize.Width;
			Size nodeSize = GetNodeSize(cache.Graphics, node);
			return new Rectangle(loc.X + offset, loc.Y, nodeSize.Width, nodeSize.Height);
		}
		protected virtual Color GetNodeTextColor(TreeNode node) {
			Color sytemColor = node.IsSelected || SelectedNodes.Contains(node) ? SystemColors.HighlightText : SystemColors.WindowText;
			return CommonSkins.GetSkin(LookAndFeel).GetSystemColor(sytemColor);
		}
		protected virtual Font GetNodeTextFont(TreeNode node) {
			return node.NodeFont ?? Font;
		}
		#endregion
		#region Node Background
		protected virtual void OnDrawNodeBackground(GraphicsCache cache, TreeNode node) {
			if(node.IsSelected || SelectedNodes.Contains(node))
				OnDrawNodeHighlightedBackground(cache, node);
			else OnDrawNodeBaseBackground(cache, node);
		}
		protected virtual void OnDrawNodeBaseBackground(GraphicsCache cache, TreeNode node) {
			DrawBackgroundCore(cache, GetNodeBackgroundBounds(cache.Graphics, node));
		}
		protected virtual void OnDrawNodeHighlightedBackground(GraphicsCache cache, TreeNode node) {
			Rectangle bounds = GetNodeBackgroundBounds(cache.Graphics, node);
			using(SolidBrush br = new SolidBrush(SelectedNodeBackgroundColor)) {
				cache.FillRectangle(br, bounds);
			}
		}
		protected virtual Rectangle GetNodeBackgroundBounds(Graphics graphics, TreeNode node) {
			if(node == null) return Rectangle.Empty;
			Rectangle nodeBounds = GetNodeBounds(graphics, node);
			return new Rectangle(0, nodeBounds.Y, ClientRectangle.Width, nodeBounds.Height);
		}
		protected virtual Color SelectedNodeBackgroundColor {
			get { return CommonSkins.GetSkin(LookAndFeel).GetSystemColor(SystemColors.Highlight); }
		}
		#endregion
		#region Node Image
		protected virtual void OnDrawNodeImage(GraphicsCache cache, TreeNode node) {
			Rectangle bounds = CalcImageBounds(node);
			Image image = GetNodeImage(node);
			if(image != null) cache.Graphics.DrawImage(image, bounds);
		}
		protected virtual Image GetNodeImage(TreeNode node) {
			IImageProvider info = node.Tag as IImageProvider;
			if(info != null && info.IsImageExist)
				return info.Image;
			if(node.ImageIndex != -1 && ImageList != null && ImageList.Images.Count > node.ImageIndex)
				return ImageList.Images[node.ImageIndex];
			return null;
		}
		protected virtual Rectangle CalcImageBounds(TreeNode node) {
			Point loc = GetParentNodeLocation(node);
			return new Rectangle(loc, DefaultNodeImageSize);
		}
		#endregion
		#region Node Check
		protected virtual void OnDrawNodeCheckBox(GraphicsCache cache, TreeNode node) {
			if(!ShouldDrawCheck(node))
				return;
			SkinElementInfo elementInfo = GetCheckBoxElementInfo(cache, node, CalcCheckBounds(cache, node));
			SkinElementPainter.Default.DrawObject(elementInfo);
		}
		protected virtual SkinElementInfo GetCheckBoxElementInfo(GraphicsCache cache, TreeNode node, Rectangle bounds) {
			SkinElementInfo elementInfo = GetCheckBoxElementInfoCore(bounds);
			elementInfo.Cache = cache;
			elementInfo.ImageIndex = CalcCheckBoxElementImageIndex(node, elementInfo);
			return elementInfo;
		}
		protected virtual SkinElementInfo GetCheckBoxElementInfoCore(Rectangle bounds) {
			SkinElement skinElement = EditorsSkins.GetSkin(LookAndFeel)[EditorsSkins.SkinCheckBox];
			return new SkinElementInfo(skinElement, bounds);
		}
		protected virtual int CalcCheckBoxElementImageIndex(TreeNode node, SkinElementInfo elementInfo) {
			if(node is DXSolutionTreeNode) {
				if(!((DXSolutionTreeNode)node).Enabled) return 7;
			}
			return node.Checked ? 4 : 0;
		}
		protected virtual bool ShouldDrawCheck(TreeNode node) {
			return CheckBoxes && node.Level < 2;
		}
		protected virtual Rectangle CalcCheckBounds(GraphicsCache cache, TreeNode node) {
			Rectangle textBounds = CalcTextBounds(cache, node);
			Point loc = new Point(textBounds.X - DefaultNodeCheckSize.Width, textBounds.Y + textBounds.Height / 2 - DefaultNodeCheckSize.Height / 2);
			return new Rectangle(loc, DefaultNodeCheckSize);
		}
		#endregion
		#region Node Glyph
		protected virtual void OnDrawNodeGlyph(GraphicsCache cache, TreeNode node) {
			if(!ShouldDrawGlyph(node))
				return;
			Rectangle bounds = CalcGlyphBounds(node);
			OpenCloseButtonInfoArgs args = new OpenCloseButtonInfoArgs(cache);
			args.Bounds = bounds;
			args.State = ObjectState.Normal;
			args.Opened = node.IsExpanded;
			args.BackAppearance = AppearanceObject.ControlAppearance;
			SkinOpenCloseButtonObjectPainter painter = new SkinOpenCloseButtonObjectPainter(LookAndFeel);
			painter.DrawObject(args);
		}
		protected virtual bool ShouldDrawGlyph(TreeNode node) {
			return node.Nodes.Count != 0;
		}
		protected virtual Rectangle CalcGlyphBounds(TreeNode node) {
			Rectangle res = new Rectangle(0, node.Bounds.Y, DefaultExpandCollapseButtonSize.Width, DefaultNodeImageSize.Height);
			res.X = node.Bounds.X - DefaultExpandCollapseButtonSize.Width - DefaultExpandCollapseButtonOffset;
			if(ImageList != null)
				res.X -= ImageCollection.GetImageListSize(ImageList).Width;
			if(ShouldDrawCheck(node)) res.X -= DefaultNodeCheckSize.Width;
			if(res.Height < node.Bounds.Height) {
				res.Y += (node.Bounds.Height - res.Height) / 2;
			}
			return res;
		}
		#endregion
		#region Common
		protected Point GetParentNodeLocation(TreeNode node) {
			TreeNode parent = node.Parent;
			Point result = node.Bounds.Location;
			if(parent != null)
				result = new Point(parent.Bounds.X + node.TreeView.Indent, node.Bounds.Y);
			if(ImageList != null)
				result.X -= ImageCollection.GetImageListSize(ImageList).Width;
			return result;
		}
		protected virtual bool IsNodeImageExist(TreeNode node) {
			IImageProvider info = node.Tag as IImageProvider;
			if(info != null)
				return info.IsImageExist;
			return ImageCollection.IsImageListImageExists(ImageList, node.ImageIndex);
		}
		protected virtual Size DefaultNodeImageSize {
			get { return new Size(16, 16); }
		}
		protected virtual Size DefaultNodeCheckSize {
			get { return new Size(16, 16); }
		}
		public virtual int DefaultExpandCollapseButtonOffset {
			get;
			set;
		}
		protected virtual Size DefaultExpandCollapseButtonSize {
			get { return new Size(13, 15); }
		}
		protected virtual int DistanceBetweenImageAndText {
			get { return 5; }
		}
		#endregion
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.mouseUpTimer != null) this.mouseUpTimer.Dispose();
				if(this.dragAutoExpandTimer != null) this.dragAutoExpandTimer.Dispose();
				if(this.dragAutoScrollTimer != null) this.dragAutoScrollTimer.Dispose();
				this.dragAutoExpandTimer = this.dragAutoScrollTimer = null;
				LookAndFeel.StyleChanged -= OnLookAndFeelStyleChanged;
				lookAndFeelCore.Dispose();
				this.mouseUpTimer = null;
				SelectedNodes.Clear();
				this.anchorNode = null;
				if(searchHelper != null)
					searchHelper.Clear();
			}
			base.Dispose(disposing);
		}
		protected TreeNode AnchorNode { get { return anchorNode; } set { anchorNode = value; } }
		protected bool IsSelectionLocked { get { return this.lockSelection != 0; } }
		protected override CreateParams CreateParams {
			get {
				const int TVS_NOHSCROLL = 0x8000;
				CreateParams cp = base.CreateParams;
				if(!AllowHScrollBar) cp.Style |= TVS_NOHSCROLL;
				return cp;
			}
		}
		protected override void OnBeforeSelect(TreeViewCancelEventArgs e) {
			if(SelectedNode != null)
				SelectionInfoCore = new SelectionInfo(SelectedNode);
			if(IsStandardSelection || this.repaintingSelection) {
				base.OnBeforeSelect(e);
				return;
			}
			if(e.Action == TreeViewAction.ByMouse) {
				e.Cancel = true;
				base.OnBeforeSelect(e);
				return;
			}
			switch(SelectionMode) {
				case DXTreeSelectionMode.MultiSelectChildren:
					OnBeforeSelectMulti(e);
					break;
				case DXTreeSelectionMode.MultiSelectChildrenSameBranch:
					OnBeforeSelectMultiSameBranch(e);
					break;
			}
			base.OnBeforeSelect(e);
		}
		protected void ForceSelectNode(TreeNode node) {
			this.repaintingSelection = true;
			try {
				if(node != null) AnchorNode = node;
				SelectedNode = node;
			}
			finally {
				this.repaintingSelection = false;
			}
		}
		protected virtual void OnBeforeSelectMulti(TreeViewCancelEventArgs e) {
			OnBeforeSelectMultiSameBranch(e); 
		}
		protected virtual void OnBeforeSelectMultiSameBranch(TreeViewCancelEventArgs e) {
			if(e.Node == null) return;
			e.Cancel = true;
			SelectNode(e.Node);
		}
		protected virtual bool AllowNodeBranches(TreeNode checkNode) {
			if(SelectedNodes.Count == 0) return true;
			foreach(TreeNode node in SelectedNodes.Keys) {
				if(node.Parent != checkNode.Parent) return false;
			}
			return true;
		}
		protected virtual bool HasNonMultiSelectNodesSelected {
			get {
				if(SelectedNodes.Count == 0) return false;
				foreach(TreeNode node in SelectedNodes.Keys) {
					if(!CanMultiSelectNode(node)) return true;
				}
				return false;
			}
		}
		protected virtual bool CanMultiSelectNode(TreeNode node) {
			TreeViewCancelEventArgs e = new TreeViewCancelEventArgs(node, node.Nodes.Count != 0, TreeViewAction.Unknown);
			if(AllowMultiSelectNode != null) AllowMultiSelectNode(this, e);
			return !e.Cancel;
		}
		Point mouseDownPoint = new Point(-10000, -10000);
		TreeNode mouseDownNode = null;
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button != MouseButtons.Left) return;
			this.mouseDownNode = null;
			this.mouseDownPoint = new Point(e.X, e.Y);
			TreeNode node = GetNodeAt(e.X, e.Y);
			if(node == null) return;
			if(IsStandardSelection) {
				if(AllowDrag) SelectedNode = node;
				this.mouseDownNode = node;
				return;
			}
			if(IsControlPressed) {
				if(!CanMultiSelectNode(node)) SelectNode(node);
				else {
					if(HasNonMultiSelectNodesSelected || !AllowNodeBranches(node))
						SelectNode(node);
					else {
						if(!IsNodeSelected(node)) ForceSelectNode(node);
						SetNodeSelection(node, !IsNodeSelected(node));
					}
				}
				Invalidate();
				return;
			}
			if(IsShiftPressed) {
				if(SelectedNodes.Count > 0) {
					SelectRangeCore(AnchorNode, node, true);
					return;
				}
				SelectNode(node);
				return;
			}
			this.mouseDownNode = node;
			if(IsNodeSelected(node) && SelectedNodes.Count > 1) {
				if(AllowDrag) return; 
			}
			SelectNode(node);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			if(IsStandardSelection || AnchorNode == null) return;
			if(e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) {
				DoArrowNavigation(e);
			}
		}
		protected virtual void DoArrowNavigation(KeyEventArgs e) {
			TreeNode actionNode = e.KeyCode == Keys.Down ? AnchorNode.NextVisibleNode : AnchorNode.PrevVisibleNode;
			if(actionNode == null) return;
			if(e.Control) {
				ForceSelectNode(actionNode);
				e.Handled = true;
				return;
			}
			if(e.Shift && CanMultiSelectNode(actionNode) && !HasNonMultiSelectNodesSelected) {
				ForceSelectNode(actionNode);
				AddSelection(actionNode);
			}
			else
				SelectNode(actionNode);
			e.Handled = true;
		}
		private bool allowCustomMouseMoveProcesssingCore = true;
		protected bool AllowCustomMouseMoveProcessing {
			get { return allowCustomMouseMoveProcesssingCore; }
			set { allowCustomMouseMoveProcesssingCore = value; }
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!AllowCustomMouseMoveProcessing) return;
			if(e.Button != MouseButtons.Left || this.mouseDownNode == null) return;
			if(Math.Abs(e.X - this.mouseDownPoint.X) > 5 || Math.Abs(e.Y - this.mouseDownPoint.Y) > 5) {
				DoDragDropNodes();
				OnMouseUp(e);
			}
		}
		protected virtual void DoDragDropNodes() {
			if(!AllowDrag) return;
			if((IsStandardSelection && SelectedNode == null) || (!IsStandardSelection && SelCount == 0)) return;
			CancelEventArgs e = new CancelEventArgs(false);
			if(DragNodeAllow != null) DragNodeAllow(this, e);
			if(e.Cancel) return;
			this.mouseDownNode = null; 
			DragDropEffects effect = DragDropEffects.None;
			OnStartDrag();
			try {
				TreeNode[] nodes = IsStandardSelection ? new TreeNode[] { SelectedNode } : SelNodes;
				TreeViewGetDragObjectEventArgs de = new TreeViewGetDragObjectEventArgs(nodes);
				if(DragNodeGetObject != null) DragNodeGetObject(this, de);
				effect = DoDragDrop(de.DragObject, de.AllowEffects);
			}
			finally {
				OnEndDrag(effect);
			}
		}
		protected virtual void OnStartDrag() {
			if(DragNodeStart != null) DragNodeStart(this, EventArgs.Empty);
		}
		void StopTimers() {
			if(this.dragAutoExpandTimer != null) this.dragAutoExpandTimer.Stop();
			if(this.dragAutoScrollTimer != null) this.dragAutoScrollTimer.Stop();
		}
		protected virtual void OnEndDrag(DragDropEffects effect) {
			StopTimers();
			if(DragNodeEnd != null) DragNodeEnd(this, new DragEventArgs(null, 0, 0, 0, DragDropEffects.All, effect));
		}
		protected override void OnDragDrop(DragEventArgs e) {
			StopTimers();
			base.OnDragDrop(e);
		}
		protected override void OnEnter(EventArgs e) {
			SetSelectedNodesColor(NodeColor.Selected);
			base.OnEnter(e);
		}
		protected override void OnLeave(EventArgs e) {
			if(mouseUpTimer != null && this.mouseUpTimer.Enabled) OnMouseUpTimerAction();
			SetSelectedNodesColor(NodeColor.HideSelection);
			base.OnLeave(e);
		}
		Keys autoScrollDirection = Keys.None;
		TreeNode autoExpandNode = null;
		protected override void OnDragOver(DragEventArgs e) {
			base.OnDragOver(e);
			Point pt = PointToClient(new Point(e.X, e.Y));
			if(CheckAutoScroll(pt)) return;
			CheckAutoExpand(pt);
		}
		bool CheckAutoExpand(Point pt) {
			if(this.dragAutoExpandTimer == null) return false;
			if(!DragAutoExpand) {
				this.dragAutoExpandTimer.Stop();
				return false;
			}
			TreeNode node = GetNodeAt(pt);
			if(node == this.autoExpandNode) {
				if(node == null) return false;
				return true;
			}
			else {
				if(this.autoExpandNode == null) {
					if(GetNodePlusMinusBounds(node).Contains(pt)) {
						this.autoExpandNode = node;
						this.dragAutoExpandTimer.Interval = AutoExpandInterval;
						this.dragAutoExpandTimer.Start();
						return true;
					}
				}
				this.autoExpandNode = null;
				this.dragAutoExpandTimer.Stop();
			}
			return false;
		}
		Rectangle GetNodePlusMinusBounds(TreeNode node) {
			if(node == null) return Rectangle.Empty;
			Rectangle r = node.Bounds;
			r.Width = 20 + (node.ImageIndex > -1 ? 16 : 0);
			r.Offset(-r.Width, 0);
			return r;
		}
		bool CheckAutoScroll(Point pt) {
			if(CheckAutoScrollCore(new Rectangle(0, 0, Width, AutoScrollHeight), pt, Keys.Up)) {
				return true;
			}
			if(CheckAutoScrollCore(new Rectangle(0, ClientRectangle.Height - AutoScrollHeight, Width, AutoScrollHeight), pt, Keys.Down)) {
				return true;
			}
			this.autoScrollDirection = Keys.None;
			if(this.dragAutoScrollTimer != null) this.dragAutoScrollTimer.Stop();
			return false;
		}
		bool CheckAutoScrollCore(Rectangle scroll, Point pt, Keys direction) {
			if(this.dragAutoScrollTimer == null) return false;
			if(!DragAutoScroll) {
				this.dragAutoScrollTimer.Stop();
				return false;
			}
			if(scroll.Contains(pt)) {
				if(this.autoScrollDirection == direction) return true;
				this.autoScrollDirection = direction;
				this.dragAutoScrollTimer.Interval = AutoScrollTimerStartInterval;
				this.dragAutoScrollTimer.Start();
				return true;
			}
			return false;
		}
		void OnAutoScrollTimer(object sender, EventArgs e) {
			if(!IsHandleCreated || !Visible || this.autoScrollDirection == Keys.None) return;
			Rectangle scroll = new Rectangle(0, 0, Width, AutoScrollHeight);
			if(this.autoScrollDirection == Keys.Down) scroll.Y = ClientRectangle.Height - AutoScrollHeight;
			if(scroll.Contains(PointToClient(Control.MousePosition))) {
				if(TopNode != null) {
					TreeNode newNode = null;
					if(this.autoScrollDirection == Keys.Up) {
						newNode = TopNode.PrevVisibleNode;
					}
					else {
						newNode = GetNodeAt(scroll.Location);
						if(newNode != null) {
							if(newNode.NextVisibleNode == null) newNode.EnsureVisible();
							newNode = newNode.NextVisibleNode;
						}
					}
					if(newNode != null) {
						newNode.EnsureVisible();
						if(this.dragAutoScrollTimer.Interval > 50) this.dragAutoScrollTimer.Interval -= 70;
						this.dragAutoScrollTimer.Start();
					}
				}
			}
			else {
				this.autoScrollDirection = Keys.None;
				this.dragAutoScrollTimer.Stop();
			}
		}
		void OnAutoExpandTimer(object sender, EventArgs e) {
			if(this.autoExpandNode == null || !IsHandleCreated || !Visible) return;
			if(GetNodePlusMinusBounds(this.autoExpandNode).Contains(PointToClient(Control.MousePosition))) {
				if(this.autoExpandNode.IsExpanded) this.autoExpandNode.Collapse();
				else this.autoExpandNode.Expand();
				this.dragAutoExpandTimer.Start();
			}
			else {
				this.autoExpandNode = null;
				this.dragAutoExpandTimer.Stop();
			}
		}
		void OnMouseUpTimerTick(object sender, EventArgs e) {
			if((Control.MouseButtons & MouseButtons.Left) != 0) {
				this.mouseUpTimer.Start();
				return;
			}
			OnMouseUpTimerAction();
		}
		void OnMouseUpTimerAction() {
			this.mouseUpTimer.Stop();
			this.mouseUpTimer.Enabled = false;
			if(!IsHandleCreated) return;
			Point pt = PointToClient(Control.MousePosition);
			OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, pt.X, pt.Y, 0));
		}
		void StartMouseUpTimer() {
			if(this.mouseUpTimer == null) return;
			this.mouseUpTimer.Start();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if((Control.MouseButtons & MouseButtons.Left) != 0) {
				StartMouseUpTimer();
				return;
			}
			TreeNode oldDownNode = this.mouseDownNode;
			this.mouseDownNode = null;
			if(IsStandardSelection || e.Button != MouseButtons.Left) return;
			TreeNode node = GetNodeAt(e.X, e.Y);
			if(node == null) return;
			if(node == oldDownNode && IsNodeSelected(node)) SelectNode(node);
		}
		protected bool IsControlPressed { get { return (Control.ModifierKeys & Keys.Control) != 0; } }
		protected bool IsShiftPressed { get { return (Control.ModifierKeys & Keys.Shift) != 0; } }
		protected virtual void SelectRangeCore(TreeNode start, TreeNode end, bool byAction) {
			if(start == null || end == null || IsStandardSelection) return;
			if(!CanMultiSelectNode(end)) {
				SelectNode(end);
				return;
			}
			BeginSelection();
			try {
				if(start.Nodes.Contains(end)) {
					RemoveSelection(start);
					start = start.Nodes[0];
				}
				if(end.Parent == start.Parent && end.Parent != null) {
					if(byAction) ForceSelectNode(end);
					int s = end.Parent.Nodes.IndexOf(start), e = end.Parent.Nodes.IndexOf(end);
					for(int n = Math.Min(s, e); n <= Math.Max(s, e); n++) {
						AddSelection(end.Parent.Nodes[n]);
					}
				}
				else {
					SelectNode(end);
				}
			}
			finally {
				EndSelection();
			}
		}
		protected void SetNodeSelection(TreeNode node, bool selected) {
			if(selected) AddSelection(node);
			else RemoveSelection(node);
		}
		protected virtual void AddSelection(TreeNode node) {
			if(SelectedNodes.ContainsKey(node)) return;
			if(AnchorNode == null) this.anchorNode = node;
			if(SelectedNode == null) ForceSelectNode(node);
			SelectedNodes.Add(node, 0);
			SetNodeColor(node, GetSelectionColor());
			OnSelectionChanged();
		}
		protected virtual void RemoveSelection(TreeNode node) {
			if(!SelectedNodes.ContainsKey(node)) return;
			if(node == AnchorNode) this.anchorNode = null;
			SelectedNodes.Remove(node);
			if(node == SelectedNode) {
				ForceSelectNode(FindNearestSelectedNode(node));
			}
			SetNodeColor(node, NodeColor.Default);
			OnSelectionChanged();
		}
		TreeNodeCollection GetCollection(TreeNode node) {
			if(node.Parent == null) return node.TreeView.Nodes;
			return node.Parent.Nodes;
		}
		TreeNode FindNearestSelectedNode(TreeNode source) {
			TreeNode res = null, first = null;
			int indexDelta = 100000, i = GetCollection(source).IndexOf(source);
			foreach(TreeNode node in SelectedNodes.Keys) {
				first = node;
				if(node.Parent == source.Parent) {
					int delta = Math.Abs(GetCollection(node).IndexOf(node) - i);
					if(delta < indexDelta) {
						res = node;
						indexDelta = delta;
					}
				}
			}
			if(res != null) return res;
			return first;
		}
		protected NodeColor GetSelectionColor() {
			if(!Focused) return NodeColor.HideSelection;
			return NodeColor.Selected;
		}
		protected virtual void ClearSelection() {
			this.anchorNode = null;
			SetSelectedNodesColor(NodeColor.Default);
			SelectedNodes.Clear();
		}
		protected virtual void SetSelectedNodesColor(NodeColor color) {
			if(SelectedNodes.Count > 1) BeginUpdate();
			try {
				foreach(TreeNode node in SelectedNodes.Keys) {
					SetNodeColor(node, color);
				}
			}
			finally {
				if(SelectedNodes.Count > 1) EndUpdate();
			}
		}
		protected virtual void SetNodeColor(TreeNode node, NodeColor color) {
			Color fore = Color.Empty, back = Color.Empty;
			switch(color) {
				case NodeColor.HideSelection:
					fore = SystemColors.ControlText;
					back = SystemColors.ControlLight;
					break;
				case NodeColor.Selected:
					fore = SystemColors.HighlightText;
					back = SystemColors.Highlight;
					break;
			}
			node.ForeColor = fore;
			node.BackColor = back;
		}
		const int TVM_GETEDITCONTROL = (0x1100 + 15), WM_SETTEXT = 0x000C;
		[System.Security.SecuritySafeCritical]
		protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e) {
			base.OnBeforeLabelEdit(e);
			if(e.CancelEdit || e.Node == null || GetNodeEditText == null) return;
			IntPtr handle = SendMessage(Handle, TVM_GETEDITCONTROL, IntPtr.Zero, IntPtr.Zero);
			if(handle != IntPtr.Zero) {
				TreeViewGetNodeEditTextEventArgs et = new TreeViewGetNodeEditTextEventArgs(e.Node);
				GetNodeEditText(this, et);
				if(et.Modified) SendMessage(handle, WM_SETTEXT, IntPtr.Zero, et.Text);
			}
		}
		[DllImport("USER32.dll")]
		static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("USER32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
		protected virtual void OnSelectionChanged() {
			if(IsSelectionLocked) return;
			if(SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
		}
		protected Hashtable SelectedNodes { get { return selectedNodes; } }
		protected bool IsStandardSelection { get { return SelectionMode == DXTreeSelectionMode.Standard; } }
		public virtual void BeginSelection() {
			this.lockSelection++;
		}
		public virtual void EndSelection() {
			if(--this.lockSelection == 0) {
				OnSelectionChanged();
			}
		}
		public virtual void UpdateSelection() {
			if(Nodes.Count == 0) {
				if(SelectedNodes.Count > 0) {
					ClearSelection();
					OnSelectionChanged();
				}
				return;
			}
			if(SelCount == 0) return;
			bool fireChanged = false;
			TreeNode[] selNodes = SelNodes;
			foreach(TreeNode node in selNodes) {
				if(node.TreeView == null) {
					SelectedNodes.Remove(node);
					fireChanged = true;
				}
			}
			if(fireChanged) OnSelectionChanged();
		}
		public virtual void SelectNode(TreeNode node) {
			if(IsStandardSelection) SelectedNode = node;
			else {
				if(SelCount != 1 || !IsNodeSelected(node)) {
					ClearSelection();
					AddSelection(node);
				}
				if(SelectedNode != node) ForceSelectNode(node);
			}
		}
		[DefaultValue(true)]
		public bool DragAutoScroll { get { return dragAutoScroll; } set { dragAutoScroll = value; } }
		[DefaultValue(true)]
		public bool DragAutoExpand { get { return dragAutoExpand; } set { dragAutoExpand = value; } }
		[DefaultValue(false)]
		public bool AllowDrag { get { return allowDrag; } set { allowDrag = value; } }
		[DefaultValue(false)]
		public bool AllowHScrollBar {
			get { return allowHScrollBar; }
			set {
				if(AllowHScrollBar == value) return;
				allowHScrollBar = value;
				if(IsHandleCreated) UpdateStyles();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNode[] SelNodes {
			get {
				if(SelectedNodes.Count == 0) return new TreeNode[] { };
				TreeNode[] nodes = new TreeNode[SelectedNodes.Count];
				SelectedNodes.Keys.CopyTo(nodes, 0);
				if(nodes.Length > 1) Array.Sort(nodes, new TreeNodeComparer());
				return nodes;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeNode SelNode {
			get {
				if(SelCount == 0) return null;
				return SelNodes[0];
			}
			set {
				SelectNode(value);
			}
		}
		public virtual bool IsNodeSelected(TreeNode node) { return SelectedNodes.ContainsKey(node); }
		public void SelectRange(TreeNode start, TreeNode end) {
			SelectRangeCore(start, end, false);
		}
		[Browsable(false)]
		public int SelCount { get { return SelectedNodes.Count; } }
		public DXTreeSelectionMode SelectionMode {
			get { return selectionMode; }
			set {
				if(SelectionMode == value) return;
				selectionMode = value;
				OnSelectionModeChanged();
			}
		}
		protected virtual void OnSelectionModeChanged() {
		}
		protected enum NodeColor { Default, Selected, HideSelection }
		class TreeNodeComparer : IComparer {
			public int Compare(object a, object b) {
				TreeNode node1 = a as TreeNode, node2 = b as TreeNode;
				return Comparer.Default.Compare(node1.Index, node2.Index);
			}
		}
		#region Nested Types
		public class SelectionInfo {
			public SelectionInfo(TreeNode source) {
				this.Source = source;
			}
			public TreeNode Source { get; set; }
			public TreeNode Target { get; set; }
			public bool IsReady {
				get { return Source != null && Target != null; }
			}
		}
		#endregion
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
		}
		#region Search
		ExtendedTreeNodeCollection extendedNodesCore;
		public ExtendedTreeNodeCollection ExtendedNodes {
			get {
				if(this.extendedNodesCore == null)
					extendedNodesCore = new ExtendedTreeNodeCollection(null, this.Nodes);
				return extendedNodesCore;
			}
		}
		IDXTreeViewFilteringHelper searchHelper;
		ISearchControl searchControl;
		protected virtual bool CanFilterItems { get { return (searchControl != null); } }
		protected virtual void ApplyItemsFilter(DevExpress.Data.Filtering.CriteriaOperator criteriaOperator) {
			if(!CanFilterItems || searchHelper == null) return;
			searchHelper.ApplyFilter(criteriaOperator);
		}
		protected virtual IDXTreeViewFilteringHelper CreateFilteringHelper(DXTreeView treeView) {
			return new TreeViewNodeCollectionFilteringHelper(treeView);
		}
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) {
			if(this.searchControl == searchControl) return;
			this.searchControl = searchControl;
			ApplyItemsFilter(null);
		}
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			SearchCriteriaInfo infoArgs = searchInfo as SearchCriteriaInfo;
			DevExpress.Data.Filtering.CriteriaOperator criteriaOperator = infoArgs != null ? infoArgs.CriteriaOperator : null;
			ApplyItemsFilter(criteriaOperator);
		}
		bool ISearchControlClient.IsAttachedToSearchControl {
			get { return this.searchControl != null; }
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return new DXTreeViewSearchControlProvider(this);
		}
		protected class DXTreeViewSearchControlProvider : SearchControlCriteriaProviderBase {
			public DXTreeViewSearchControlProvider(DXTreeView treeView)
				: base() {
				this.treeView = treeView;
			}
			DXTreeView treeView;
			protected override void DisposeCore() {
				treeView = null;
			}
			protected override Data.Filtering.CriteriaOperator CalcActiveCriteriaOperatorCore(SearchControlQueryParamsEventArgs args, Data.Helpers.FindSearchParserResults result) {
				string[] searchFields = new string[] { "Text" };
				return DevExpress.Data.Filtering.DxFtsContainsHelper.Create(searchFields, result, args.FilterCondition);
			}
			protected override Data.Helpers.FindSearchParserResults CalcResultCore(SearchControlQueryParamsEventArgs args) {
				return new DevExpress.Data.Helpers.FindSearchParser().Parse(args.SearchText);
			}
		}
		protected class TreeViewNodeCollectionFilteringHelper : IDXTreeViewFilteringHelper {
			public TreeViewNodeCollectionFilteringHelper(DXTreeView treeView) {
				this.treeView = treeView;
				treeView.ExtendedNodes.CollectionChanged += TreeNodeCollectoinHelper_CollectionChanged;
			}
			DXTreeView treeView;
			List<HiddenNodeWrapper> hiddenNodes = new List<HiddenNodeWrapper>();
			List<TreeNode> selectedNodes = new List<TreeNode>();
			Data.Filtering.CriteriaOperator criteriaOperator;
			Data.Filtering.CriteriaOperator notNullCriteriaOperator;
			Func<TreeNode, bool> CreatePredicate(Data.Filtering.CriteriaOperator criteriaOperator) {
				var descriptor = Data.Filtering.CriteriaCompilerDescriptor.Get(typeof(TreeNode));
				return DevExpress.Data.Filtering.CriteriaCompiler.ToPredicate<TreeNode>(criteriaOperator, descriptor);
			}
			void RestoreHiddenNodes() {
				foreach(HiddenNodeWrapper hiddenNode in hiddenNodes) {
					TreeNodeCollection insertCollection = hiddenNode.VisibleParent == null ? treeView.Nodes : hiddenNode.VisibleParent.Nodes;
					insertCollection.Insert(hiddenNode.Position, hiddenNode.HiddenNode);
				}
				hiddenNodes.Clear();
			}
			int ApplyNodesFilter(TreeNode currentNode, int currentNodePosition, TreeNodeCollection filteringCollection, Func<TreeNode, bool> predicate, List<HiddenNodeWrapper> hiddenNodes) {
				int beginOffset = 0;
				for(int i = 0; i < filteringCollection.Count; i++) {
					int offsetAfterFilter = ApplyNodesFilter(filteringCollection[i], i + beginOffset, filteringCollection[i].Nodes, predicate, hiddenNodes);
					if(offsetAfterFilter != 0) {
						i--;
						beginOffset += offsetAfterFilter;
					}
				}
				int off = 0;
				if(currentNode != null) {
					if(IsMatch(currentNode, currentNodePosition, predicate, hiddenNodes))
						off++;
				}
				return off;
			}
			bool IsMatch(TreeNode currentNode, int currentNodePosition, Func<TreeNode, bool> predicate, List<HiddenNodeWrapper> hiddenNodes) {
				bool isRemove = false;
				bool isCurrentNodeMatch = predicate(currentNode);
				bool isChildsZero = currentNode.Nodes.Count == 0;
				if(isChildsZero && !isCurrentNodeMatch) {
					isRemove = true;
					hiddenNodes.Add(new HiddenNodeWrapper(currentNode, currentNode.Parent, currentNodePosition));
					TreeNodeCollection collection = currentNode.Parent == null ? this.treeView.Nodes : currentNode.Parent.Nodes;
					collection.Remove(currentNode);
					for(int i = 0; i < hiddenNodes.Count; i++) {
						if(hiddenNodes[i].VisibleParent == currentNode) {
							currentNode.Nodes.Insert(hiddenNodes[i].Position, hiddenNodes[i].HiddenNode);
							hiddenNodes.Remove(hiddenNodes[i]);
							i--;
						}
					}
				}
				else {
					if(isCurrentNodeMatch)
						selectedNodes.Add(currentNode);
				}
				return isRemove;
			}
			void TreeNodeCollectoinHelper_CollectionChanged(object sender, ExtendedTreeNodeCollectionChangedEventArgs<ExtendedTreeNode> e) {
				OnCollectionChanged(sender as ExtendedTreeNodeCollection, e);
			}
			void DestroyStore() {
				hiddenNodes.Clear();
				hiddenNodes = null;
				selectedNodes.Clear();
				selectedNodes = null;
				treeView.ExtendedNodes.CollectionChanged -= TreeNodeCollectoinHelper_CollectionChanged;
				criteriaOperator = null;
				notNullCriteriaOperator = null;
			}
			public virtual void ApplyFilter(Data.Filtering.CriteriaOperator criteriaOperator) {
				SetCriteriaOperator(criteriaOperator);
				Func<TreeNode, bool> predicate = CreatePredicate(criteriaOperator);
				treeView.BeginUpdate();
				treeView.SuspendLayout();
				RestoreHiddenNodes();
				bool isPredicateNull = predicate == null;
				if(!isPredicateNull) {
					selectedNodes.Clear();
					ApplyNodesFilter(null, 0, this.treeView.Nodes, predicate, hiddenNodes);
					treeView.ExpandAll();
					SetSelectedNodes();
					selectedNodes.Clear();
				}
				treeView.ResumeLayout(false);
				treeView.EndUpdate();
			}
			public virtual Data.Filtering.CriteriaOperator GetLastCriteriaOperator(bool notNull) {
				return notNull ? notNullCriteriaOperator : criteriaOperator;
			}
			public virtual void Reset() {
				ApplyFilter(null);
			}
			public virtual void Clear() {
				DestroyStore();
			}
			public virtual ActionAfterChanging ActionAfterChanging {
				get { return ActionAfterChanging.Reset; }
			}
			public virtual FilteringSelctionMode SelectionMode {
				get { return FilteringSelctionMode.First; }
			}
			void SetCriteriaOperator(Data.Filtering.CriteriaOperator criteriaOperator) {
				this.criteriaOperator = criteriaOperator;
				if(Object.ReferenceEquals(criteriaOperator, null)) return;
				this.notNullCriteriaOperator = criteriaOperator;
			}
			protected virtual void OnCollectionChanged(ExtendedTreeNodeCollection collection, ExtendedTreeNodeCollectionChangedEventArgs<ExtendedTreeNode> e) {
				UpdateInnerCollection(collection, e);
				UpdateSearch();
			}
			protected virtual void UpdateInnerCollection(ExtendedTreeNodeCollection collection, ExtendedTreeNodeCollectionChangedEventArgs<ExtendedTreeNode> e) {
				HiddenNodeWrapper wrapNode = null;
				switch(e.Action) {
					case TreeNodeExtensionCollectionAction.Clear:
						this.hiddenNodes.Clear();
						break;
					case TreeNodeExtensionCollectionAction.Remove:
						try {
							wrapNode = this.hiddenNodes.Find(wn => wn.HiddenNode.Equals(e.Element.InnerNode));
							this.hiddenNodes.Remove(wrapNode);
						}
						catch { }
						break;
					case TreeNodeExtensionCollectionAction.RemoveAt:
						wrapNode = this.hiddenNodes.Find(wn => wn.HiddenNode.Equals(e.Element.InnerNode));
						this.hiddenNodes.Remove(wrapNode);
						break;
					default:
						break;
				}
			}
			protected virtual void UpdateSearch() {
				switch(ActionAfterChanging) {
					case ActionAfterChanging.Reset:
						Reset();
						break;
					case ActionAfterChanging.Research:
						ApplyFilter(criteriaOperator);
						break;
					default:
						break;
				}
			}
			protected virtual void SetSelectedNodes() {
				if(selectedNodes.Count == 0) return;
				var sortedArray = selectedNodes.OrderBy(n => n.Level).ToArray();
				switch(SelectionMode) {
					case FilteringSelctionMode.First:
						treeView.SelNode = sortedArray[0];
						break;
					case FilteringSelctionMode.Last:
						treeView.SelNode = sortedArray[sortedArray.Length - 1];
						break;
					default:
						break;
				}
			}
			class HiddenNodeWrapper {
				public HiddenNodeWrapper(TreeNode hiddenNode, TreeNode visibleParent, int position) {
					this.Position = position;
					this.HiddenNode = hiddenNode;
					this.VisibleParent = visibleParent;
				}
				public int Position { get; private set; }
				public TreeNode HiddenNode { get; private set; }
				public TreeNode VisibleParent { get; private set; }
				public int Level {
					get { return this.HiddenNode.Level; }
				}
			}
		}
		#endregion
	}
	public class TreeViewGetDragObjectEventArgs : EventArgs {
		object dragObject;
		DragDropEffects allowEffects;
		public TreeViewGetDragObjectEventArgs(object dragObject) {
			this.dragObject = dragObject;
			this.allowEffects = DragDropEffects.All;
		}
		public object DragObject { get { return dragObject; } set { dragObject = value; } }
		public DragDropEffects AllowEffects { get { return allowEffects; } set { allowEffects = value; } }
	}
	public delegate void TreeViewGetDragObjectEventHandler(object sender, TreeViewGetDragObjectEventArgs e);
	public enum DXTreeSelectionMode { Standard, MultiSelectChildren, MultiSelectChildrenSameBranch }
	public class DXTreeViewControl : DXTreeView {
		protected override bool UseThemes {
			get { return true; }
		}
	}
	public class DXSolutionTreeNode : TreeNode {
		bool enabled;
		bool isIndeterminate;
		public DXSolutionTreeNode(string text)
			: base(text) {
			this.enabled = true;
			this.isIndeterminate = false;
		}
		public bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value)
					return;
				enabled = value;
				OnEnabledChanged();
			}
		}
		public bool IsIndeterminate {
			get { return isIndeterminate; }
			set {
				if(IsIndeterminate == value)
					return;
				isIndeterminate = value;
				OnIndeterminateChanged(value);
			}
		}
		protected virtual void OnEnabledChanged() {
		}
		protected virtual void OnIndeterminateChanged(bool value) {
		}
		public new DXSolutionTreeNode Parent { get { return base.Parent as DXSolutionTreeNode; } }
	}
	[ToolboxItem(false)]
	public class DXSolutionTreeView : DXTreeView {
		public DXSolutionTreeView() {
			this.AllowSkinning = true;
			this.CheckBoxes = true;
		}
		public void CheckBranch(TreeNode node, bool check) {
			if(this.updateState) return;
			BeginUpdate();
			try {
				EnumChildren(node, n => n.Checked = check);
			}
			finally {
				EndUpdate();
			}
		}
		public void SelectFirstNode() {
			if(Nodes.Count > 0) SelectedNode = Nodes[0];
		}
		protected void EnumChildren(TreeNode node, Action<TreeNode> handler) {
			foreach(TreeNode childNode in node.Nodes) {
				handler(childNode);
				EnumChildren(childNode, handler);
			}
		}
		protected override bool ShouldDrawCheck(TreeNode node) {
			return CheckBoxes;
		}
		protected override void OnBeforeCheck(TreeViewCancelEventArgs e) {
			DXSolutionTreeNode node = e.Node as DXSolutionTreeNode;
			if(node != null) {
				if(!node.Enabled) {
					e.Cancel = true;
					return;
				}
				if(node.IsIndeterminate) {
					DoClickOnIndeterminateNode(node);
					e.Cancel = true;
					return;
				}
			}
			base.OnBeforeCheck(e);
		}
		protected virtual void DoClickOnIndeterminateNode(DXSolutionTreeNode node) {
			node.IsIndeterminate = false;
			node.Checked = true;
			CheckBranch(node, true);
		}
		protected override void OnAfterCheck(TreeViewEventArgs e) {
			base.OnAfterCheck(e);
			UpdateNodeState(e.Node);
		}
		public void UpdateStates() {
			BeginUpdate();
			try {
				DoUpdateStates();
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void DoUpdateStates() {
			foreach(TreeNode leaf in GetLeafs()) {
				UpdateNodeState(leaf);
			}
		}
		protected IEnumerable<TreeNode> GetLeafs() {
			List<TreeNode> leafs = new List<TreeNode>();
			foreach(TreeNode node in Nodes) {
				EnumChildren(node, n =>
				{
					if(n.Nodes.Count == 0) leafs.Add(n);
				});
			}
			return leafs;
		}
		bool updateState = false;
		protected virtual void UpdateNodeState(TreeNode treeNode) {
			if(this.updateState) return;
			DXSolutionTreeNode node = treeNode as DXSolutionTreeNode;
			if(node == null || node.Parent == null) return;
			this.updateState = true;
			BeginUpdate();
			try {
				DoUpdateNodeState(node.Parent);
			}
			finally {
				this.updateState = false;
				EndUpdate();
			}
		}
		protected virtual void DoUpdateNodeState(DXSolutionTreeNode node) {
			int checkedSiblings = 0, uncheckedSiblings = 0;
			foreach(DXSolutionTreeNode child in node.Nodes) {
				if(child.IsIndeterminate) break;
				if(child.Checked) checkedSiblings++;
				else uncheckedSiblings++;
			}
			int childrenCount = node.Nodes.Count;
			node.IsIndeterminate = (checkedSiblings != childrenCount && uncheckedSiblings != childrenCount);
			if(!node.IsIndeterminate) {
				node.Checked = (checkedSiblings == childrenCount);
			}
			if(node.Parent != null) DoUpdateNodeState(node.Parent);
		}
		protected override int CalcCheckBoxElementImageIndex(TreeNode node, SkinElementInfo elementInfo) {
			DXSolutionTreeNode n = node as DXSolutionTreeNode;
			if(n != null && n.IsIndeterminate) return 8;
			return base.CalcCheckBoxElementImageIndex(node, elementInfo);
		}
	}
}
namespace DevExpress.Utils.Design.Internal {
	public enum ActionAfterChanging { Reset, Research, None }
	public enum TreeNodeExtensionCollectionAction { Add, Remove, Insert, RemoveAt, Clear }
	public enum FilteringSelctionMode { First, Last }
	public interface IDXTreeViewFilteringHelper {
		void ApplyFilter(Data.Filtering.CriteriaOperator criteriaOperator);
		Data.Filtering.CriteriaOperator GetLastCriteriaOperator(bool notNull);
		void Reset();
		void Clear();
		ActionAfterChanging ActionAfterChanging { get; }
		FilteringSelctionMode SelectionMode { get; }
	}
	public class ExtendedTreeNodeCollection : IEnumerable, IEnumerable<ExtendedTreeNode> {
		public ExtendedTreeNodeCollection(ExtendedTreeNode parent, TreeNodeCollection collection) {
			this.collection = collection;
			Parent = parent;
			map = new Dictionary<TreeNode, ExtendedTreeNode>();
		}
		TreeNodeCollection collection;
		Dictionary<TreeNode, ExtendedTreeNode> map;
		public TreeNodeCollection InnerCollection {
			get { return this.collection; }
		}
		public ExtendedTreeNode Parent { get; private set; }
		public event EventHandler<ExtendedTreeNodeCollectionChangedEventArgs<ExtendedTreeNode>> CollectionChanged;
		public int Add(TreeNode node) {
			int result = this.collection.Add(node);
			ExtendedTreeNode eNode = AddNodeCore(node);
			RaiseChanges(this, eNode, TreeNodeExtensionCollectionAction.Add, -1);
			return result;
		}
		public void Insert(int index, TreeNode node) {
			this.collection.Insert(index, node);
			ExtendedTreeNode eNode = AddNodeCore(node);
			RaiseChanges(this, eNode, TreeNodeExtensionCollectionAction.Insert, index);
		}
		protected internal ExtendedTreeNode AddNodeCore(TreeNode node) {
			ExtendedTreeNode eNode = null;
			if(!map.ContainsKey(node)) {
				eNode = new ExtendedTreeNode(node);
				SubscribeNodeEvents(eNode);
				map.Add(node, eNode);
			}
			return eNode;
		}
		void SubscribeNodeEvents(ExtendedTreeNode node) {
			node.Nodes.CollectionChanged += ChildNodes_CollectionChanged;
			foreach(ExtendedTreeNode childNode in node.Nodes as IEnumerable<ExtendedTreeNode>) {
				childNode.Nodes.CollectionChanged += ChildNodes_CollectionChanged;
			}
		}
		void UnSubscribeNodeEvents(ExtendedTreeNode node) {
			foreach(ExtendedTreeNode childNode in node.Nodes as IEnumerable<ExtendedTreeNode>) {
				childNode.Nodes.CollectionChanged -= ChildNodes_CollectionChanged;
			}
			node.Nodes.CollectionChanged -= ChildNodes_CollectionChanged;
		}
		public void Remove(TreeNode node) {
			this.collection.Remove(node);
			ExtendedTreeNode eNode = RemoveCore(node);
			RaiseChanges(this, eNode, TreeNodeExtensionCollectionAction.Remove, -1);
		}
		public void RemoveAt(int index) {
			TreeNode node = this.collection[index];
			this.collection.RemoveAt(index);
			ExtendedTreeNode eNode = RemoveCore(node);
			RaiseChanges(this, eNode, TreeNodeExtensionCollectionAction.RemoveAt, index);
		}
		protected ExtendedTreeNode RemoveCore(TreeNode node) {
			foreach(TreeNode childNode in node.Nodes) {
				RemoveCore(node);
			}
			ExtendedTreeNode eNode = GetExtendedNode(node, false);
			UnSubscribeNodeEvents(eNode);
			map.Remove(node);
			return eNode;
		}
		public void Clear() {
			collection.Clear();
			ClearChilds(map.Values);
			map.Clear();
			RaiseChanges(this, null, TreeNodeExtensionCollectionAction.Clear, -1);
		}
		void ClearChilds(IEnumerable<ExtendedTreeNode> nodes) {
			foreach(var node in nodes) {
				ClearChilds(node.Nodes);
				UnSubscribeNodeEvents(node);
				node.Nodes.map.Clear();
			}
		}
		void ChildNodes_CollectionChanged(object sender, ExtendedTreeNodeCollectionChangedEventArgs<ExtendedTreeNode> e) {
			RaiseChanges(sender, e);
		}
		public TreeNode this[int index] {
			get { return this.InnerCollection[index]; }
			set { }
		}
		void RaiseChanges(object changedCollection, ExtendedTreeNode node, TreeNodeExtensionCollectionAction action, int index) {
			ExtendedTreeNodeCollectionChangedEventArgs<ExtendedTreeNode> e = new ExtendedTreeNodeCollectionChangedEventArgs<ExtendedTreeNode>(action, node, index);
			RaiseChanges(changedCollection, e);
		}
		void RaiseChanges(object changedCollection, ExtendedTreeNodeCollectionChangedEventArgs<ExtendedTreeNode> e) {
			if(CollectionChanged != null) {
				CollectionChanged(changedCollection, e);
			}
		}
		public IEnumerator GetEnumerator() {
			return this.collection.GetEnumerator();
		}
		IEnumerator<ExtendedTreeNode> IEnumerable<ExtendedTreeNode>.GetEnumerator() {
			return this.map.Values.GetEnumerator();
		}
		public ExtendedTreeNode GetExtendedNode(TreeNode key, bool inDeep) {
			ExtendedTreeNode value = null;
			if(!inDeep) map.TryGetValue(key, out value);
			else FindInDeep(key, ref value);
			return value;
		}
		void FindInDeep(TreeNode key, ref ExtendedTreeNode value) {
			FindInDeep(key, map.Values, ref value);
		}
		bool FindInDeep(TreeNode key, IEnumerable<ExtendedTreeNode> eNodes, ref ExtendedTreeNode value) {
			bool result = false;
			foreach(ExtendedTreeNode eNode in eNodes) {
				if(IsMatch(key, eNode.InnerNode)) {
					value = eNode;
					return true;
				}
				result = FindInDeep(key, eNode.Nodes, ref value);
				if(result == true)
					return true;
			}
			return result;
		}
		bool IsMatch(TreeNode key, TreeNode currentValue) {
			return currentValue.Equals(key);
		}
	}
	public class ExtendedTreeNode {
		public ExtendedTreeNode(TreeNode node) {
			InnerNode = node;
			nodesCore = new ExtendedTreeNodeCollection(this, node.Nodes);
			CreateChildsTree(this, node.Nodes);
		}
		void CreateChildsTree(ExtendedTreeNode parent, TreeNodeCollection nodes) {
			foreach(TreeNode child in nodes) {
				ExtendedTreeNode current = this.Nodes.AddNodeCore(child);
				CreateChildsTree(current, child.Nodes);
			}
		}
		public TreeNode InnerNode { get; private set; }
		ExtendedTreeNodeCollection nodesCore;
		public ExtendedTreeNodeCollection Nodes {
			get { return this.nodesCore; }
		}
	}
	public class ExtendedTreeNodeCollectionChangedEventArgs<T> : EventArgs {
		private readonly TreeNodeExtensionCollectionAction action;
		private readonly T element;
		private readonly int index;
		public ExtendedTreeNodeCollectionChangedEventArgs(TreeNodeExtensionCollectionAction action, T element, int index = -1) {
			this.action = action;
			this.element = element;
			this.index = index;
		}
		public TreeNodeExtensionCollectionAction Action {
			get { return this.action; }
		}
		public T Element {
			get { return this.element; }
		}
		public int Index {
			get {
				if(index == -1) throw new InvalidOperationException("Property index is not initialized!");
				return this.index;
			}
		}
	}
}
