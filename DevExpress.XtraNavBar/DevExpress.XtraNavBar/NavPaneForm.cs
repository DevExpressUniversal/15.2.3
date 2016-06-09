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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Menu;
using System.Collections;
using DevExpress.XtraNavBar.ViewInfo;
using DevExpress.Utils.Win;
using System.ComponentModel;
namespace DevExpress.XtraNavBar.Forms {
	public enum NavPaneFormHitTest { None, SizeGrip, VerticalSizeGrip, Link }
	public class NavPaneFormHitInfo : NavBarNavigationPaneHitInfo {
		NavPaneForm form;
		NavPaneFormHitTest formHitTest = NavPaneFormHitTest.None;
		Rectangle bounds;
		NavLinkInfoArgs linkInfo;
		public NavPaneFormHitInfo(NavPaneForm form) : base(form.NavBar) {
			this.form = form;
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public NavPaneForm Form { get { return form; } }
		public NavPaneFormHitTest FormHitTest { get { return formHitTest; } set { formHitTest = value; } }
		protected override NavBarHitInfo CreateHitInfo() {
			return new NavPaneFormHitInfo(form);
		}
		public override void CalcHitInfo(Point p, NavBarHitTest[] validLinkHotTracks) {
			CalcHitInfo(p);
		}
		protected override void Assign(NavBarHitInfo info) {
			base.Assign(info);
			NavPaneFormHitInfo hi = info as NavPaneFormHitInfo;
			if(hi == null) return;
			this.form = hi.form;
			this.formHitTest = hi.formHitTest;
			this.bounds = hi.bounds;
			this.linkInfo = hi.linkInfo;
		}
		public virtual void CalcHitInfo(Point p) {
			this.HitPoint = p;
			this.FormHitTest = NavPaneFormHitTest.None;
			this.bounds = Rectangle.Empty;
			this.linkInfo = null;
			if(FormViewInfo.SizeGripBounds.Contains(p)) {
				FormHitTest = NavPaneFormHitTest.SizeGrip;
				Bounds = FormViewInfo.SizeGripBounds;
				return;
			}
			else if(FormViewInfo.VerticalSizeGripBounds.Contains(p)) {
				FormHitTest = NavPaneFormHitTest.SizeGrip;
				Bounds = FormViewInfo.VerticalSizeGripBounds;
				return;
			}
			else if(FormViewInfo.Bounds.Contains(p)) {
				SetGroup(GroupViewInfo.Group);
			}
			if(CalcGroupHitInfo(GroupViewInfo, p, NavBar.ViewInfo.ValidHotTracks)) {
				if(InLink) {
					FormHitTest = NavPaneFormHitTest.Link;
					if(Link != null) {
						foreach(NavLinkInfoArgs info in GroupViewInfo.Links) {
							if(info.Link == Link) {
								linkInfo = info;
								break;
							}
						}
						if(LinkInfo.Bounds.Bottom > GroupViewInfo.ClientInfo.ClientInnerBounds.Bottom) {
							HitTestCore = NavBarHitTest.None;
						}
						else 
						Bounds = LinkInfo.Bounds;
					}
				}
			}
		}
		public virtual NavLinkInfoArgs LinkInfo { get { return linkInfo; } }
		public override bool Equals(object obj) {
			NavPaneFormHitInfo hi = obj as NavPaneFormHitInfo;
			if(hi.FormHitTest != hi.FormHitTest) return false;
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public virtual NavGroupInfoArgs GroupViewInfo { get { return Form.ExpandedGroupInfo; } }
		public virtual NavPaneFormViewInfo FormViewInfo { get { return Form.ViewInfo; } }
	}
	public class NavPaneFormViewInfo { 
		NavPaneForm form;
		Rectangle bounds, clientBounds, contentBounds, sizeGripBounds, verticalSizeGripBounds;
		bool isReady;
		NavPaneFormHitInfo hotInfo, pressedInfo, selectedInfo;
		GraphicsInfo gInfo;
		public static int DefaultContentIndent = 3;
		public NavPaneFormViewInfo(NavPaneForm form) {
			this.form = form;
		}
		public NavPaneFormHitInfo SelectedInfo {
			get { return selectedInfo; }
			set { selectedInfo = value; }
		}
		public NavPaneFormHitInfo HotInfo {
			get {
				if(hotInfo == null) hotInfo = new NavPaneFormHitInfo(Form);
				return hotInfo;
			}
			set { hotInfo = value; }
		}
		public NavPaneFormHitInfo PressedInfo {
			get {
				if(pressedInfo == null) pressedInfo = new NavPaneFormHitInfo(Form);
				return pressedInfo;
			}
			set { pressedInfo = value; }
		}
		public virtual void UpdateSelectedInfo(NavPaneFormHitInfo hi) {
			if(SelectedInfo != null) InvalidateHitObject(SelectedInfo);
			else Form.Invalidate();
			selectedInfo = hi;
		}
		public virtual void UpdateHotInfo(NavPaneFormHitInfo hi) {
			hotInfo = hi;
			if(HotInfo.LinkInfo != null) HotInfo.LinkInfo.State = ObjectState.Hot;
			InvalidateHitObject(HotInfo);
		}
		public virtual NavBarControl NavBar { get { return Form.NavBar; } }
		public virtual int ContentIndent { get { return DefaultContentIndent; } }
		public virtual void UpdateHotInfo(Point p) {
			NavPaneFormHitInfo hitInfo = CalcHitInfo(p), oldInfo = HotInfo;
			if(!HotInfo.IsEquals(hitInfo)) {
				ClearHotInfo(false);
				UpdateHotInfo(hitInfo);
				NavBar.ViewInfo.RaiseHotEvent(oldInfo, HotInfo);
			}
			hotInfo = hitInfo;
		}
		public virtual ToolTipControlInfo GetTooltipObjectInfo(Point pt) {
			NavPaneFormHitInfo hitInfo = CalcHitInfo(pt);
			return NavBar.ViewInfo.GetTooltipObjectInfo(hitInfo);
		}
		public virtual void ClearHotInfo(bool hideHint) {
			if(hideHint) {
				NavBar.ViewInfo.StopTimer();
				NavBar.ViewInfo.HideHint();
			}
			if(HotInfo.LinkInfo != null) HotInfo.LinkInfo.State = ObjectState.Normal;
			NavPaneFormHitInfo oldInfo = HotInfo;
			this.hotInfo = new NavPaneFormHitInfo(Form);
			InvalidateHitObject(oldInfo);
		}
		public virtual void UpdatePressedInfo(NavPaneFormHitInfo hi) {
			this.pressedInfo = hi;
			if(PressedInfo.Link != null) PressedInfo.LinkInfo.State = ObjectState.Pressed;
			InvalidateHitObject(PressedInfo);
			int index = Array.IndexOf(NavBar.ViewInfo.ValidPressedInfo, hi.HitTest);
			if(hi.Link != null && !hi.Link.Enabled && !NavBar.ViewInfo.AllowSelectDisabledLink) return;
			if(index == -1) return;
			NavBar.ViewInfo.DoPress(hi, NavBar.ViewInfo.ValidPressedStateInfo[index]);
		}
		public virtual void ClearPressedInfo() {
			NavPaneFormHitInfo oldInfo = PressedInfo;
			if(oldInfo.LinkInfo != null) oldInfo.LinkInfo.State = ObjectState.Normal;
			this.pressedInfo = new NavPaneFormHitInfo(Form);
			InvalidateHitObject(oldInfo);
		}
		public virtual bool InSizeGrip {
			get {
				return PressedInfo.FormHitTest == NavPaneFormHitTest.SizeGrip || PressedInfo.FormHitTest == NavPaneFormHitTest.VerticalSizeGrip;
			}
		}
		protected virtual void InvalidateHitObject(NavPaneFormHitInfo hInfo) {
			if(hInfo == null || hInfo.Bounds == Rectangle.Empty) return;
			Form.Invalidate(hInfo.Bounds);
		}
		protected internal virtual NavPaneFormHitInfo CalcHitInfo(Point p) {
			NavPaneFormHitInfo hInfo = new NavPaneFormHitInfo(Form);
			hInfo.CalcHitInfo(p);
			return hInfo;
		}
		public NavPaneForm Form { get { return form; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Rectangle VerticalSizeGripBounds { get { return verticalSizeGripBounds; } set { verticalSizeGripBounds = value; } }
		public Rectangle SizeGripBounds { get { return sizeGripBounds; } set { sizeGripBounds = value; } }
		public bool IsReady { get { return isReady; } set { isReady = value; } }
		public GraphicsInfo GInfo { 
			get {
				if (gInfo == null) gInfo = new GraphicsInfo();
				return gInfo; 
			} 
		}
		public virtual void CalcViewInfo(Rectangle bounds, Graphics g) {
			GInfo.AddGraphics(g);
			try
			{
				this.bounds = bounds;
				ClientBounds = CalcClientBoundsByBounds(Bounds);
				SizeGripBounds = CalcSizeGripBounds(ClientBounds);
				ContentBounds = CalcContentBoundsByClient(ClientBounds);
				VerticalSizeGripBounds = CalcVerticalSizeGripBounds(Bounds, ClientBounds);
				UpdateExpandedGroupInfo();
				IsReady = true;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected internal virtual void UpdateExpandedGroupInfo() {
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				Rectangle rect = ContentBounds;
				NavGroupClientInfoArgs args = Form.ExpandedGroupInfo.ClientInfo;
				args.Cache = new GraphicsCache(gInfo.Graphics);
				if(args.ClientInnerBounds.IsEmpty) {
					NavBar.GroupPainter.ClientPainter.CalcObjectBounds(args);
				}
				rect.Inflate(args.ClientInnerBounds.Left - args.Bounds.Left, args.ClientInnerBounds.Top - args.Bounds.Top);
				args.Bounds = rect;
				NavBar.GroupPainter.ClientPainter.CalcObjectBounds(args);
			}
			finally {
				if(Form.ExpandedGroupInfo.ClientInfo.Cache != null) Form.ExpandedGroupInfo.ClientInfo.Cache.Dispose();
				Form.ExpandedGroupInfo.ClientInfo.Cache = null;
				gInfo.ReleaseGraphics(); 
			}
			if(Form.ExpandedGroup.ControlContainer != null) Form.ExpandedGroup.ControlContainer.SetBounds(ContentBounds);
		}
		protected virtual Rectangle CalcClientBoundsByBounds(Rectangle rect) {
			rect.Inflate(-10, -9);
			return rect; 
		}
		protected virtual Rectangle CalcBoundsByClientBounds(Rectangle rect) {
			rect.Inflate(10, 9);
			return rect; 
		}
		protected virtual Rectangle CalcContentBoundsByClient(Rectangle rect) {
			rect.Height = Math.Max(0, rect.Height - GetSizeGripSize().Height);
			rect.Inflate(-ContentIndent, -ContentIndent);
			return rect; 
		}
		protected virtual Rectangle CalcClientBoundsByContent(Rectangle rect) {
			rect.Inflate(ContentIndent, ContentIndent);
			rect.Height += GetSizeGripSize().Height;
			return rect; 
		}
		protected virtual Rectangle CalcSizeGripBounds(Rectangle rect) {
			Size gripSize = GetSizeGripSize();
			if(NavBar.IsRightToLeftDirection) {
				return new Rectangle(rect.Left, rect.Bottom - gripSize.Height, gripSize.Width, gripSize.Height);
			}
			return new Rectangle(rect.Right - gripSize.Width, rect.Bottom - gripSize.Height, gripSize.Width, gripSize.Height);
		}
		protected virtual Rectangle CalcVerticalSizeGripBounds(Rectangle bounds, Rectangle clientBounds) {
			if(NavBar.IsRightToLeftDirection) return new Rectangle(0, bounds.Y, clientBounds.Left, bounds.Height);
			return new Rectangle(clientBounds.Right, bounds.Y, bounds.Right - clientBounds.Right, bounds.Height);
		}
		protected virtual Size GetSizeGripSize() { return new Size(12, 12); }
		public virtual Size CalcBestSize() {
			NavigationPaneViewInfo navPane = NavBar.ViewInfo as NavigationPaneViewInfo;
			if(navPane == null || Form.ExpandedGroupInfo == null) return Size.Empty;
			bool graphicsAdd = false;
			if (GInfo.Graphics == null)
			{
				GInfo.AddGraphics(null);
				graphicsAdd = true;
			}
			GraphicsCache cache = new GraphicsCache(gInfo.Graphics);
			Rectangle rect = Rectangle.Empty;
			try {
				Form.ExpandedGroupInfo.Cache = cache;
				rect.Size = navPane.CalcExpandedGroupBestSize(Form.ExpandedGroupInfo);
			}
			finally {
				cache.Dispose();
				Form.ExpandedGroupInfo.Cache = null;
				if(graphicsAdd) GInfo.ReleaseGraphics();
			}
			return CalcBoundsByClientBounds(CalcClientBoundsByContent(rect)).Size;
		}
		public virtual NavPaneFormInfoArgs GetInfoArgs(GraphicsCache cache) {
			NavPaneFormInfoArgs args = new NavPaneFormInfoArgs(cache, Bounds, ObjectState.Normal);
			args.ClientBounds = ClientBounds;
			args.ContentBounds = ContentBounds;
			args.SizeGripBounds = SizeGripBounds;
			args.NavPaneViewInfo = NavBar.ViewInfo as NavigationPaneViewInfo;
			args.ExpandedGroupInfo = Form.ExpandedGroupInfo;
			return args;
		}
		internal NavPaneFormHitInfo CreateHitInfo() {
			return new NavPaneFormHitInfo(Form);
		}
		public virtual NavLinkInfoArgs GetLinkInfo(NavBarItemLink link) {
			foreach(NavLinkInfoArgs li in Form.ExpandedGroupInfo.Links) {
				if(li.Link == link) return li;
			}
			return null;
		}
	}
	public class NavPaneFormPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			NavPaneFormInfoArgs npe = e as NavPaneFormInfoArgs;
			if(npe == null) return;
			DrawBackground(npe);
			DrawContent(npe);
			DrawSizeGrip(npe);
			DrawFormBorder(npe);
		}
		public virtual void DrawBackground(NavPaneFormInfoArgs e) {
			if(e.NavPaneViewInfo == null) return;
			e.Cache.FillRectangle(e.Cache.GetSolidBrush(e.NavPaneViewInfo.PaintAppearance.Background.BackColor), e.Bounds);
		}
		protected virtual void DrawFormBorder(NavPaneFormInfoArgs e) { 
			if(e.NavPaneViewInfo == null) return;
			e.NavPaneViewInfo.BorderPainter.DrawObject(new BorderObjectInfoArgs(e.Cache, e.Bounds, e.NavPaneViewInfo.PaintAppearance.NavigationPaneHeader));
			e.NavPaneViewInfo.BorderPainter.DrawObject(new BorderObjectInfoArgs(e.Cache, e.ClientBounds, e.NavPaneViewInfo.PaintAppearance.NavigationPaneHeader));
		}
		protected virtual void DrawSizeGrip(NavPaneFormInfoArgs e) {
			if(e.NavPaneViewInfo == null) return;
			for(int i = 0; i < e.SizeGripBounds.Width; i+=4) {
				DrawSizeGripLine(e, i);
			}
		}
		protected virtual void DrawSizeGripLine(NavPaneFormInfoArgs e, int i) {
			if(e.NavPaneViewInfo.IsRightToLeft)
				e.Cache.Graphics.DrawLine(Pens.Black, new Point(e.SizeGripBounds.Right - i, e.SizeGripBounds.Bottom - 1), new Point(e.SizeGripBounds.X, e.SizeGripBounds.Y - 1 + i));
			else e.Cache.Graphics.DrawLine(Pens.Black, new Point(e.SizeGripBounds.X - 2 + i, e.SizeGripBounds.Bottom - 1), new Point(e.SizeGripBounds.Right - 2, e.SizeGripBounds.Y - 1 + i));
		}
		protected virtual void CalcLinksState(NavPaneFormInfoArgs e) {
			foreach(NavLinkInfoArgs link in e.ExpandedGroupInfo.Links) {
				if(e.NavPaneViewInfo.NavBar.SelectedLink == link.Link) link.State |= ObjectState.Selected;
				else link.State &= ~ObjectState.Selected;
			}
		}
		protected virtual void DrawContent(NavPaneFormInfoArgs e) {
			if(e.NavPaneViewInfo == null || e.NavPaneViewInfo.Groups.Count == 0) return;
			CalcLinksState(e);
			bool prev = e.NavPaneViewInfo.NavBar.GroupPainter.ClientPainter.AllowPartitallyVisibleLinks;
			e.NavPaneViewInfo.NavBar.GroupPainter.ClientPainter.AllowPartitallyVisibleLinks = false;
			e.NavPaneViewInfo.NavBar.GroupPainter.ClientPainter.DrawObject(e.ExpandedGroupInfo.ClientInfo);
			e.NavPaneViewInfo.NavBar.GroupPainter.ClientPainter.AllowPartitallyVisibleLinks = prev;
		}
	}	
	public class NavPaneFormInfoArgs : ObjectInfoArgs {
		public NavPaneFormInfoArgs(GraphicsCache cache) : base(cache) { }
		public NavPaneFormInfoArgs(GraphicsCache cache, Rectangle bounds, ObjectState state) : base(cache, bounds, state) { }
		Rectangle clientBounds, contentBounds, sizeGripBounds;
		NavigationPaneViewInfo navPaneViewInfo;
		NavGroupInfoArgs expandedGroupInfo;
		public Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public Rectangle SizeGripBounds { get { return sizeGripBounds; } set { sizeGripBounds = value; } }
		public NavigationPaneViewInfo NavPaneViewInfo { get { return navPaneViewInfo; } set { navPaneViewInfo = value; } }
		public NavGroupInfoArgs ExpandedGroupInfo { get { return expandedGroupInfo; } set { expandedGroupInfo = value; } }
	}
	[ToolboxItem(false)]
	public class NavPaneForm : TopFormBase, IToolTipControlClient {
		static Point EmptyDragPoint = new Point(-10000, -10000);
		static int FormMinWidth = 64;
		NavBarControl navBar;
		NavBarGroup group;
		NavGroupInfoArgs groupInfo;
		NavPaneFormViewInfo viewInfo;
		NavPaneFormPainter painter;
		Point downPoint = EmptyDragPoint;
		Rectangle controlBounds;
		public NavPaneForm(NavBarControl navBar) {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.navBar = navBar;
			this.AllowDrop = NavBar.AllowDrop;
			this.group = NavBar.ViewInfo.GetExpandedGroup();
			if(group.ControlContainer != null) {
				NavBar.Controls.Remove(group.ControlContainer);
				Controls.Add(group.ControlContainer);
				group.ControlContainer.Visible = true;
				controlBounds = group.ControlContainer.Bounds;
			}
			this.groupInfo = new NavGroupInfoArgs(ExpandedGroup, Rectangle.Empty);
			if(this.groupInfo != null) this.groupInfo.ClientInfo.InForm = true;
			NavBar.GetToolTipController().AddClientControl(this);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				NavBar.GetToolTipController().RemoveClientControl(this);
			}
		}
		public NavBarControl NavBar { get { return navBar; } }
		public NavBarGroup ExpandedGroup { get { return group; } }
		public NavGroupInfoArgs ExpandedGroupInfo { get { return groupInfo; } }
		public virtual Size CalcBestSize() {
			Size res = ViewInfo.CalcBestSize();
			if(NavBar.OptionsNavPane.PopupFormSize != Size.Empty) return NavBar.OptionsNavPane.PopupFormSize;
			Rectangle r = Screen.GetWorkingArea(NavBar);
			res.Height = Math.Min(res.Height + 1, r.Height);
			return res;
		}
		protected virtual NavPaneFormViewInfo CreateViewInfo() { return new NavPaneFormViewInfo(this); }
		protected virtual NavPaneFormPainter CreatePainter() { return new NavPaneFormPainter(); }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return ViewInfo == null ? null : ViewInfo.GetTooltipObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return NavBar.ViewInfo != null && NavBar.ViewInfo.AllowTooltipController; } }
		public virtual NavPaneFormViewInfo ViewInfo {
			get {
				if(viewInfo == null) viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		public virtual NavPaneFormPainter Painter {
			get {
				if(painter == null) painter = CreatePainter();
				return painter;
			}
		}
		protected virtual void CheckViewInfo(Graphics g) {
			if(ViewInfo.IsReady) return;
			ViewInfo.CalcViewInfo(new Rectangle(Point.Empty, Size), g);
		}
		protected virtual void UpdateExpandedGroupInfo(GraphicsCache cache) {
			if(ExpandedGroupInfo == null) return;
			ExpandedGroupInfo.Cache = cache;
			ExpandedGroupInfo.ClientInfo.Cache = cache;
			foreach(NavLinkInfoArgs link in ExpandedGroupInfo.Links) {
				link.Cache = cache;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			CheckViewInfo(e.Graphics);
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				UpdateExpandedGroupInfo(cache);
				Painter.DrawObject(ViewInfo.GetInfoArgs(cache));
			}
		}
		protected internal virtual void OnItemChanged(object sender, EventArgs e) {
			Invalidate();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			bool focused = NavBar.Focused;
			base.OnMouseDown(e);
			NavPaneFormHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			NavPaneFormHitInfo selectedInfo = hitInfo.Clone() as NavPaneFormHitInfo;
			ViewInfo.ClearHotInfo(true);
			if(e.Button == MouseButtons.Left) {
				ViewInfo.ClearPressedInfo();
				ViewInfo.UpdatePressedInfo(hitInfo);
				if(NavBar.GetAllowSelectedLink()) ViewInfo.UpdateSelectedInfo(selectedInfo);
				if(ViewInfo.InSizeGrip) {
					this.downPoint = e.Location;
					OnBeginDragging();
				}
			}
			else {
				ViewInfo.UpdateHotInfo(hitInfo);
				if(ViewInfo.HotInfo.FormHitTest == NavPaneFormHitTest.None) Close();
			}
			if(focused)
				NavBar.Focus();
		}
		bool inDragging = false;
		protected virtual void OnBeginDragging() {
			this.inDragging = true;
		}
		protected virtual void OnEndDragging() {
			this.inDragging = false;
			DoRefreshIfRequired();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			NavBarHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(e.Button == MouseButtons.Left) {
				bool equals = hitInfo.IsEquals(ViewInfo.PressedInfo);
				ViewInfo.ClearPressedInfo();
				if(equals ) NavBar.ViewInfo.DoClick(hitInfo);
			}
			this.downPoint = EmptyDragPoint;
			if(this.inDragging) OnEndDragging();
		}
		bool resizeForm = false;
		protected virtual void ResizeForm(MouseEventArgs e) {
			if (resizeForm) return;
			Rectangle rect;
			if (this.downPoint == EmptyDragPoint) return;
			resizeForm = true;
			if(NavBar.IsRightToLeftDirection) {
				int deltaWidth = this.downPoint.X - e.Location.X;
				if(e.Location.X > ViewInfo.SizeGripBounds.Right && deltaWidth > 0)
					deltaWidth = 0;
				int delta = Math.Max(FormMinWidth, Size.Width + deltaWidth) - Width;
				rect = Bounds;
				rect.Width += delta;
				rect.X -= delta;
				Bounds = rect;
			}
			else {
				int deltaWidth = e.Location.X - this.downPoint.X;
				if(e.Location.X < ViewInfo.SizeGripBounds.Left && deltaWidth > 0)
					deltaWidth = 0;
				Size = new Size(Math.Max(FormMinWidth, Size.Width + deltaWidth), Size.Height);
				this.downPoint = e.Location;
			}
			resizeForm = false;
		}
		public override Size MaximumSize {
			get {
				if(NavBar.OptionsNavPane.MaxPopupFormWidth == 0 || Size.Height == 0)
					return base.MaximumSize;
				return new Size(NavBar.OptionsNavPane.MaxPopupFormWidth, Size.Height);
			}
			set { base.MaximumSize = value; }
		}
		NavBarDragDropInfo dragDropInfo;
		protected NavBarDragDropInfo DragDropInfo { get { return dragDropInfo; } }
		public void ClearPressedInfo() {
			NavBarHitInfo oldInfo = ViewInfo.PressedInfo;
			ViewInfo.PressedInfo = ViewInfo.CreateHitInfo();
		}
		protected virtual void StartDragDrop(NavBarHitInfo pressedInfo) {
			if(ViewInfo.PressedInfo.InLink && NavBar.ViewInfo.AllowLinkDrag && ViewInfo.PressedInfo.Link.Item.CanDrag) {
				if((ViewInfo.PressedInfo.Group.GetDragDropFlags() & NavBarDragDrop.AllowDrag) == 0) return;
				if(dragDropInfo != null)
					dragDropInfo.Dispose();
				dragDropInfo = new NavBarDragDropInfo(this, ViewInfo.PressedInfo.Clone() as NavBarHitInfo);
				DragDropInfo.Start();
				ClearPressedInfo();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ViewInfo.UpdateHotInfo(e.Location);
			if (ViewInfo.HotInfo.FormHitTest == NavPaneFormHitTest.SizeGrip || this.downPoint != EmptyDragPoint) Cursor = Cursors.SizeWE;
			else Cursor = Cursors.Default;
			ResizeForm(e);
			if(ViewInfo.PressedInfo != null || ViewInfo.PressedInfo.InLink || ViewInfo.PressedInfo.InGroup) {
				e.Location.Offset(-ViewInfo.PressedInfo.HitPoint.X, -ViewInfo.PressedInfo.HitPoint.Y);
				if(Math.Abs(e.Location.X) > 5 || Math.Abs(e.Location.Y) > 5) {
					StartDragDrop(ViewInfo.PressedInfo);
				}
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ViewInfo.ClearHotInfo(true);
			Cursor = Cursors.Default;
			if(this.inDragging) OnEndDragging();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			ViewInfo.IsReady = false;
			Invalidate();
		}
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			if(group.ControlContainer != null) {
				Controls.Remove(group.ControlContainer);
				NavBar.Controls.Add(group.ControlContainer);
				group.ControlContainer.Visible = false;
				group.ControlContainer.SetBounds(controlBounds);
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			DoRefreshIfRequired();
		}
		protected virtual void DoRefreshIfRequired() {
			if(NavBar != null && NavBar.IsRightToLeftDirection && this.inDragging) return;
			NavBar.OptionsNavPane.ExpandedWidth = Width;
			ViewInfo.IsReady = false;
			ViewInfo.CalcViewInfo(ClientRectangle, null);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			DragDropInfo.OnDragEnter(e);
			base.OnDragEnter(e);
		}
		protected override void OnDragLeave(EventArgs e) {
			DragDropInfo.OnDragLeave(e);
			base.OnDragLeave(e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			DragDropInfo.OnDragOver(e);
			base.OnDragOver(e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			DragDropInfo.OnDragDrop(e);
			base.OnDragDrop(e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			DragDropInfo.OnGiveFeedback(e);
			base.OnGiveFeedback(e);
		}
		protected internal virtual void UpdateExpandedGroupInfo() {
			CalcBestSize();
			Invalidate();
		}
	}
	public class SkinNavPaneFormPainter : NavPaneFormPainter {
		public virtual SkinElementInfo GetBackgroundInfo(NavBarControl navBar, Rectangle bounds) {
			return new SkinElementInfo(NavPaneSkins.GetSkin(navBar)[NavPaneSkins.SkinGroupClient], bounds); 
		}
		public override void DrawBackground(NavPaneFormInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetBackgroundInfo(e.NavPaneViewInfo.NavBar, e.Bounds));		
		}
		public virtual SkinElementInfo GetFormBorderInfo(NavBarControl navBar, Rectangle bounds) {
			return new SkinElementInfo(NavPaneSkins.GetSkin(navBar)[NavPaneSkins.SkinNavPaneFormBorder], bounds);
		} 
		protected override void DrawFormBorder(NavPaneFormInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetFormBorderInfo(e.NavPaneViewInfo.NavBar, e.Bounds));
		}
		public virtual SkinElementInfo GetFormSizeGripInfo(NavBarControl navBar, Rectangle bounds) {
			if(navBar.IsRightToLeftDirection)
				return new SkinElementInfo(NavPaneSkins.GetSkin(navBar)[NavPaneSkins.SkinNavPaneFormLeftSizeGrip], bounds);
			return new SkinElementInfo(NavPaneSkins.GetSkin(navBar)[NavPaneSkins.SkinNavPaneFormSizeGrip], bounds);
		}
		protected override void DrawSizeGrip(NavPaneFormInfoArgs e) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetFormSizeGripInfo(e.NavPaneViewInfo.NavBar, e.SizeGripBounds));
		}
	}
	public class SkinNavPaneFormViewInfo : NavPaneFormViewInfo {
		public SkinNavPaneFormViewInfo(NavPaneForm form)
			: base(form) {
		}
		protected override Size GetSizeGripSize() {
			SkinNavPaneFormPainter p = Form.Painter as SkinNavPaneFormPainter;
			SkinElementInfo info = p.GetFormSizeGripInfo(NavBar, Rectangle.Empty);
			if (info.Element == null || info.Element.Image == null || info.Element.Image.Image == null) return base.GetSizeGripSize();
			return info.Element.Image.GetImageBounds(0).Size;
		}
		protected virtual SkinElementInfo GetFormBorderInfo() {
			SkinNavPaneFormPainter p = Form.Painter as SkinNavPaneFormPainter;
			return p.GetFormBorderInfo(NavBar, Rectangle.Empty);
		}
		protected override Rectangle CalcBoundsByClientBounds(Rectangle rect)
		{
			SkinElementInfo info = GetFormBorderInfo();
			if (info == null || info.Element == null) return base.CalcClientBoundsByBounds(rect);
			info.Bounds = rect;
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info, rect);
		}
		protected override Rectangle CalcClientBoundsByBounds(Rectangle rect)
		{
			SkinElementInfo info = GetFormBorderInfo();
			if(info == null || info.Element == null) return base.CalcClientBoundsByBounds(rect);
			info.Bounds = rect;
			return ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info);
		}
		public override int ContentIndent { get { return 0; } }	
	}
	public class SkinNavPaneForm : NavPaneForm {
		public SkinNavPaneForm(NavBarControl navBar)
			: base(navBar) {
		}
		protected override NavPaneFormViewInfo CreateViewInfo() {
			return new SkinNavPaneFormViewInfo(this);
		}
		protected override NavPaneFormPainter CreatePainter() {
			return new SkinNavPaneFormPainter();
		}
	}
}
