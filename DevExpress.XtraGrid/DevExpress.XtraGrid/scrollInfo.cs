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

using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Skins;
namespace DevExpress.XtraGrid.Scrolling {
	[ToolboxItem(false)]
	public class VCrkScrollBar : DevExpress.XtraEditors.VScrollBar {
		ScrollInfo scrollInfo;
		public VCrkScrollBar(ScrollInfo scrollInfo) {
			SetStyle(ControlStyles.Selectable, false);
			TabStop = false;
			this.scrollInfo = scrollInfo;
		}
		protected BaseView View { get { return scrollInfo.View; } }
		protected override void OnMouseEnter(EventArgs e) {
			scrollInfo.SetScrollCursor(Cursors.Arrow);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			scrollInfo.SetScrollCursor(Cursors.Arrow);
			base.OnMouseMove(e);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(specified == BoundsSpecified.None || specified == BoundsSpecified.All) return;
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				scrollInfo = null;
			base.Dispose(disposing);
		}
	}
	[ToolboxItem(false)]
	public class HCrkScrollBar : DevExpress.XtraEditors.HScrollBar {
		ScrollInfo scrollInfo;
		public HCrkScrollBar(ScrollInfo scrollInfo) {
			SetStyle(ControlStyles.Selectable, false);
			TabStop = false;
			this.scrollInfo = scrollInfo;
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			scrollInfo.SetScrollCursor(Cursors.Arrow);
			base.OnMouseMove(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			scrollInfo.SetScrollCursor(Cursors.Arrow);
			base.OnMouseEnter(e);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(specified == BoundsSpecified.None || specified == BoundsSpecified.All) return;
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				scrollInfo = null;
			base.Dispose(disposing);
		}
	}
	public class ScrollInfo : IDisposable {
		HCrkScrollBar hScroll;
		VCrkScrollBar vScroll;
		Rectangle clientRect, vScrollSuggestedBounds, hScrollSuggestedBounds;
		bool hScrollVisible, vScrollVisible, prevShowDataNavigator;
		Rectangle hscrollRect, vscrollRect, navigatorRect, sizeGripBounds, hBounds;
		BaseView view;
		bool allowDataNavigator;
		public ScrollInfo(BaseView view) {
			this.hBounds = this.vScrollSuggestedBounds = this.hScrollSuggestedBounds = this.clientRect = this.hscrollRect = this.vscrollRect = this.navigatorRect = Rectangle.Empty;
			this.allowDataNavigator = true;
			this.view = view;
			this.prevShowDataNavigator = false;
			this.hScroll = CreateHScroll();
			this.vScroll = CreateVScroll();
			this.HScroll.SetVisibility(false);
			this.VScroll.SetVisibility(false);
			this.HScroll.Anchor = AnchorStyles.None;
			this.VScroll.Anchor = AnchorStyles.None;
			this.VScroll.SmallChange = this.HScroll.SmallChange = 1;
			this.HScroll.LargeChange = this.VScroll.LargeChange = 1;
			this.HScroll.Value = this.HScroll.Maximum = this.VScroll.Value = this.VScroll.Maximum = 0;
			this.hScrollVisible = this.vScrollVisible = false;
		}
		bool UseWaitCursor { 
			get {
				if(view == null || view.GridControl == null) return false;
				return view.GridControl.UseWaitCursor;
			} 
		}
		internal void SetScrollCursor(Cursor cursor) {
			if(!UseWaitCursor)
			Cursor.Current = cursor;
		}
		protected virtual HCrkScrollBar CreateHScroll() {
			HCrkScrollBar scroll = new HCrkScrollBar(this);
			ScrollBarBase.ApplyUIMode(scroll);
			scroll.RightToLeft = View.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
			return scroll;
		}
		protected virtual VCrkScrollBar CreateVScroll() {
			VCrkScrollBar scroll = new VCrkScrollBar(this); 
			ScrollBarBase.ApplyUIMode(scroll);
			scroll.RightToLeft = RightToLeft.No;
			return scroll;
		}
		public void CreateHandles() {
			if(VScroll.TouchMode) return;
			IntPtr fakeHandle;
			if(!VScroll.IsHandleCreated) fakeHandle = VScroll.Handle;
			if(!HScroll.IsHandleCreated) fakeHandle = HScroll.Handle;
		}
		public event EventHandler VScroll_ValueChanged {
			add { VScroll.ValueChanged += value; }
			remove { VScroll.ValueChanged -= value; }
		}
		public event EventHandler HScroll_ValueChanged {
			add { HScroll.ValueChanged += value; }
			remove { HScroll.ValueChanged -= value; }
		}
		public event ScrollEventHandler HScroll_Scroll { 
			add { HScroll.Scroll += value; }
			remove { HScroll.Scroll -= value; }
		}
		public event ScrollEventHandler VScroll_Scroll { 
			add { VScroll.Scroll += value; }
			remove { VScroll.Scroll -= value; }
		}
		public virtual HCrkScrollBar HScroll { get { return hScroll; } }
		public virtual VCrkScrollBar VScroll { get { return vScroll; } }
		public virtual void Dispose() {
			if(HScroll != null) {
				HScroll.Dispose();
				VScroll.Dispose();
				this.hScroll = null;
				this.vScroll = null;
			}
		}
		protected virtual bool RealShowDataNavigator { 
			get { return ShowDataNavigator && Navigator != null; }
		}
		protected virtual bool ShowDataNavigator {
			get {
				return AllowDataNavigator && View.GridControl != null && View.GridControl.DefaultView == View;
			}
		}
		protected virtual UserLookAndFeel LookAndFeel { get { return View.ElementsLookAndFeel; } }
		public virtual BaseView View { get { return view; } }
		public virtual bool AllowDataNavigator { 
			get { return allowDataNavigator; }
			set {
				if(AllowDataNavigator == value) return;
				allowDataNavigator = value;
				LayoutChanged();
			}
		}
		protected virtual void UpdateNavigatorProperties() {
			if(Navigator == null) return;
			Navigator.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			Navigator.LookAndFeel.Assign(View.ElementsLookAndFeel);
		}
		protected virtual void CalcRects() {
			this.hBounds = this.sizeGripBounds = this.hscrollRect = this.vscrollRect = this.navigatorRect = Rectangle.Empty;
			Rectangle r = Rectangle.Empty, sizeGrip = Rectangle.Empty;
			int hscrollHeight = HScrollSize;
			if(HScrollVisible) {
				if(RealShowDataNavigator) {
					r.Size = Navigator.MinSize;
					r.Height = hscrollHeight;
					r.Location = new Point(ClientRect.X, ClientRect.Bottom - r.Height);
					if(r.Width > ClientRect.Width) r = Rectangle.Empty; 
					else {
					}
				}
				this.navigatorRect = r;
				int navigatorWidth = this.navigatorRect.Width;
				if(HScrollSuggestedBounds.IsEmpty) {
					r.Location = new Point(ClientRect.X + navigatorWidth, ClientRect.Bottom - hscrollHeight);
					r.Size = new Size(ClientRect.Width - navigatorWidth, HScrollHeight);
				}
				else {
					r = HScrollSuggestedBounds;
				}
				this.hBounds = new Rectangle(ClientRect.X, ClientRect.Bottom - Math.Max(r.Height, navigatorRect.Height), ClientRect.Right, Math.Max(navigatorRect.Height, r.Height));
				if(r.Height > 0 & navigatorRect.Height > 0 && r.Height < navigatorRect.Height) {
					r.Y += (navigatorRect.Height - r.Height) / 2;
				}
				this.hscrollRect = r;
			}
			if(VScrollVisible) {
				if(VScrollSuggestedBounds.IsEmpty) {
					r.Location = View.IsRightToLeft? new Point(ClientRect.X, ClientRect.Y) : new Point(ClientRect.Right - VScrollSize, ClientRect.Y);
					r.Size = new Size(VScrollSize, ClientRect.Height);
				}
				else {
					r = VScrollSuggestedBounds;
				}
				this.vscrollRect = r;
			}
			if(HScrollVisible && VScrollVisible) {
				r = Rectangle.Empty;
				if(hscrollRect.IntersectsWith(vscrollRect) || (!IsOverlapScrollBar && hscrollRect.Y == VScrollRect.Bottom)) { 
					if(VScrollRect.Bottom >= HScrollRect.Y) {
						if(!IsOverlapScrollBar) vscrollRect.Height = HScrollRect.Y - vscrollRect.Y;
						hscrollRect.Width -= VScrollSize;
						if(View.IsRightToLeft) hscrollRect.X += VScrollSize;
						r.Location = new Point(vscrollRect.Left, vscrollRect.Bottom);
						r.Size = new Size(VScrollSize, HScrollSize);
					}
				}
				if(!IsOverlapScrollBar) this.sizeGripBounds = r;
			}
		}
		public virtual bool IsOverlapHScrollBar {
			get {
				if(RealShowDataNavigator) return false;
				return IsOverlapScrollBar;
			}
		}
		public virtual bool IsOverlapScrollBar {
			get {
				if(!VScroll.IsOverlapScrollBar) return false;
				if(View != null && View.ParentView != null) {
					return false;
				}
				if(View.IsAnyDetails) return false;
				return true;
			}
		}
		public bool IsNeedToDrawScrollbar { 
			get {
				if(IsOverlapScrollBar) return false;
				return true;
			}
		}
		int lockLayout = 0;
		public virtual void LayoutChanged() {
			if(lockLayout != 0) return;
			lockLayout ++;
			try {
				CalcRects();
				UpdateNavigatorProperties();
				UpdateScrollRects();
				UpdateVisibility();
				if(ClientRect.IsEmpty) {
					VScroll.SetVisibility(false);
					HScroll.SetVisibility(false);
				}
				UpdateNavigatorVisiblitiy();
				this.prevShowDataNavigator = RealShowDataNavigator;
			}
			finally {
				lockLayout --;
			}
		}
		protected virtual void UpdateNavigatorVisiblitiy() {
			if(!ShowDataNavigator) return;
			ControlNavigator navigator;
			if(TryGetNavigator(out navigator)) {
				if(!HScroll.ActualVisible || NavigatorRect.IsEmpty)
					navigator.Visible = false;
				else navigator.Visible = true;
			}
		}
		public virtual ControlNavigator Navigator {
			get { return View.GridControl == null || !View.GridControl.UseEmbeddedNavigator ? null : View.GridControl.EmbeddedNavigator; } 
		}
		bool TryGetNavigator(out ControlNavigator navigator) {
			navigator = null;
			if(View.GridControl != null)
				navigator = view.GridControl.EmbeddedNavigator;
			return navigator != null;
		}
		void AssignLookAndFeel(UserLookAndFeel dst, UserLookAndFeel src) {
			dst.Assign(src);
			if(src is ISkinProviderEx) {
				dst.SkinMaskColor = ((ISkinProviderEx)src).GetMaskColor();
				dst.SkinMaskColor2 = ((ISkinProviderEx)src).GetMaskColor2();
			}
		}
		public virtual void UpdateLookAndFeel(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel == null) return;
			if(VScroll.LookAndFeel != null) {
				AssignLookAndFeel(VScroll.LookAndFeel, lookAndFeel);
			}
			if(HScroll.LookAndFeel != null) {
				AssignLookAndFeel(HScroll.LookAndFeel, lookAndFeel);
			}
			if(Navigator != null) {
				AssignLookAndFeel(Navigator.LookAndFeel, lookAndFeel);
			}
		}
		public virtual int VScrollSize {
			get {
				return VScroll.GetDefaultVerticalScrollBarWidth();
			}
		}
		protected virtual int HScrollHeight { get { return HScroll.GetDefaultHorizontalScrollBarHeight(); } }
		public virtual int HScrollSize {
			get { 
				int hscrollHeight = HScrollHeight;
				if(RealShowDataNavigator) {
					hscrollHeight = Math.Max(Navigator.MinSize.Height, hscrollHeight);
				}
				return hscrollHeight;
			}
		}
		public Rectangle VScrollSuggestedBounds {
			get { return vScrollSuggestedBounds; }
			set {
				if(VScrollSuggestedBounds == value) return;
				vScrollSuggestedBounds = value;
				LayoutChanged();
			}
		}
		public Rectangle HScrollSuggestedBounds {
			get { return hScrollSuggestedBounds; }
			set {
				if(HScrollSuggestedBounds == value) return;
				hScrollSuggestedBounds = value;
				LayoutChanged();
			}
		}
		public Rectangle ClientRect {
			get { return clientRect; }
			set {
				if(ClientRect == value && RealShowDataNavigator == this.prevShowDataNavigator) return;
				clientRect = value;
				LayoutChanged();
			}
		}
		public virtual Rectangle NavigatorRect {
			get { return this.navigatorRect; }
		}
		public virtual void UpdateScrollRects() {
			HScroll.RightToLeft = View.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
			if(!VScrollVisible) VScroll.SetVisibility(false);
			if(!HScrollVisible) HScroll.SetVisibility(false);
			if(HScroll.Anchor == AnchorStyles.None) {
				Rectangle r = HScrollRect;
				HScroll.Location = r.Location;
				HScroll.Size = r.Size;
			}
			if(VScroll.Anchor == AnchorStyles.None) {
				Rectangle r = VScrollRect;
				VScroll.Location = r.Location;
				VScroll.Size = r.Size;
			}
			if(ShowDataNavigator && Navigator != null) {
				Rectangle r = NavigatorRect;
				if(r.IsEmpty) 
					Navigator.Visible = false;
				else {
					Navigator.Location = r.Location;
					Navigator.Size = r.Size;
				}
			}
		}
		public Rectangle HBounds {
			get { return hBounds; }
		}
		public Rectangle HScrollRect {
			get { return hscrollRect; }
		}
		public Rectangle VScrollRect {
			get { return vscrollRect; }
		}
		public Rectangle SizeGripBounds {
			get { return sizeGripBounds; }
		}
		public bool HScrollVisible {
			get { return hScrollVisible; }
			set { 
				if(HScrollVisible == value) {
					UpdateVisibility();
					return;
				}
				hScrollVisible = value;
				LayoutChanged();
			}
		}
		public int VScrollPosition { get { return VScroll.Value; } }
		public int HScrollPosition { get { return HScroll.Value; } }
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set { 
				if(VScrollVisible == value) {
					UpdateVisibility();
					return;
				}
				vScrollVisible = value;
				LayoutChanged();
			}
		}
		public void UpdateVisibility() {
			if(HScroll.Visible != hScrollVisible) { 
				if(!hScrollVisible || !ClientRect.IsEmpty)
					HScroll.SetVisibility(hScrollVisible);
			}
			if(VScroll.Visible != vScrollVisible) {
				if(!vScrollVisible || !ClientRect.IsEmpty) 
					VScroll.SetVisibility(vScrollVisible);
			}
			UpdateNavigatorVisiblitiy();
		}
		public ScrollArgs HScrollArgs {
			get { return new ScrollArgs(HScroll); }
			set {
				value.AssignTo(HScroll);
			}
		}
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set {
				value.AssignTo(VScroll);
			}
		}
		public virtual void RemoveControls(Control container) {
			if(container == null) return;
			container.Controls.Remove(HScroll);
			container.Controls.Remove(VScroll);
		}
		public virtual void AddControls(Control container) {
			if(container == null) return;
			if(container.InvokeRequired) return;
			container.Controls.Add(HScroll);
			container.Controls.Add(VScroll);
		}
		public virtual void OnAction(ScrollNotifyAction action) {
			if(VScrollVisible) VScroll.OnAction(action);
			if(HScrollVisible) HScroll.OnAction(action);
		}
		public bool IsTouchMode { get { return VScroll.TouchMode || HScroll.TouchMode; } }
	}
}
