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

namespace DevExpress.XtraTreeList.Scrolling
{
	using System.Windows.Forms;
	using System.Drawing;
	using System.ComponentModel;
	using System;
	using DevExpress.XtraEditors;
	internal class VTLScrollBar : DevExpress.XtraEditors.VScrollBar {
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(specified == BoundsSpecified.None || specified == BoundsSpecified.All) return;
			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
	internal class HTLScrollBar : DevExpress.XtraEditors.HScrollBar {
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(specified == BoundsSpecified.None || specified == BoundsSpecified.All) return;
			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
	internal delegate void ScrollInfoChangedHandler();
	public class ScrollInfo {
		public readonly DevExpress.XtraEditors.HScrollBar HScroll;
		public readonly DevExpress.XtraEditors.VScrollBar VScroll;
		internal ScrollInfoChangedHandler ScrollInfoChanged;
		Rectangle clientRect;
		bool hScrollVisible, vScrollVisible, rightToLeft;
		int footerHeight;
		public ScrollInfo(TreeList treeList) {
			clientRect = new Rectangle();
			HScroll = CreateHScroll();
			VScroll = CreateVScroll();
			HScroll.SetVisibility(false);
			VScroll.SetVisibility(false);
			HScroll.TabStop = VScroll.TabStop = false;
			VScroll.SmallChange = HScroll.SmallChange = 1;
			HScroll.LargeChange = VScroll.LargeChange = 1;
			HScroll.Anchor = VScroll.Anchor = AnchorStyles.None;
			HScrollPos = HScrollRange = VScrollPos = VScrollRange = 0;
			HScroll.MouseEnter += new EventHandler(treeList.Scrollbar_MouseEnter);
			VScroll.MouseEnter += new EventHandler(treeList.Scrollbar_MouseEnter);
			HScroll.MouseLeave += new EventHandler(treeList.Scrollbar_MouseLeave);
			VScroll.MouseLeave += new EventHandler(treeList.Scrollbar_MouseLeave);
			hScrollVisible = vScrollVisible = false;
			ScrollBarBase.ApplyUIMode(HScroll);
			ScrollBarBase.ApplyUIMode(VScroll);
			ScrollInfoChanged = null;
		}
		public virtual int VScrollWidth { get { return VScroll.GetDefaultVerticalScrollBarWidth(); } }
		public virtual int HScrollHeight { get { return HScroll.GetDefaultHorizontalScrollBarHeight(); } }
		protected virtual DevExpress.XtraEditors.VScrollBar CreateVScroll() {
			return new VTLScrollBar();
		}
		protected virtual DevExpress.XtraEditors.HScrollBar CreateHScroll() {
			return new HTLScrollBar();
		}
		protected internal virtual int FooterHeight {
			get { return footerHeight; }
			set {
				if(FooterHeight == value)
					return;
				footerHeight = value;
				UpdateScrollBarRects();
			}
		}
		public virtual Rectangle ClientRect {
			get { return clientRect; }
			set {
				if(ClientRect == value)
					return;
				clientRect = value;
				UpdateScrollBarRects();
			}
		}
		public bool RightToLeft {
			get { return rightToLeft; }
			set {
				if(RightToLeft == value) return;
				rightToLeft = value;
				OnRightToLeftChanged();
			}
		}
		protected virtual void OnRightToLeftChanged(){
			HScroll.RightToLeft = RightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
			UpdateScrollBarRects();
		}
		Rectangle GetHScrollBounds() {
			Rectangle rect = Rectangle.Empty;
			rect.Size = new Size(ClientRect.Width - (VScrollVisible && FooterHeight == 0 ? VScrollWidth : 0), HScrollHeight);
			rect.X = RightToLeft ? ClientRect.Right - rect.Width : ClientRect.X;
			rect.Y = ClientRect.Bottom - HScrollHeight;
			return rect;
		}
		Rectangle GetVScrollBounds() {
			Rectangle rect = Rectangle.Empty;
			rect.Size = new Size(VScrollWidth, ClientRect.Height - FooterHeight - (HScrollVisible && !IsOverlapScrollbar ? HScrollHeight : 0));
			rect.X = RightToLeft ? ClientRect.X : ClientRect.Right - VScrollWidth;
			rect.Y = ClientRect.Y;
			return rect;
		}
		void SetScrollBounds(ScrollTouchBase scroll, Rectangle Bounds) {
			scroll.Location = Bounds.Location;
			scroll.Size = Bounds.Size;
		}
		protected virtual void UpdateScrollBarRects() {
			if(HScroll.Anchor == AnchorStyles.None)
				SetScrollBounds(HScroll, GetHScrollBounds());
			if(VScroll.Anchor == AnchorStyles.None)
				SetScrollBounds(VScroll, GetVScrollBounds());
			UpdateVisibility();
			if(ClientRect.IsEmpty) {
				VScroll.Visible = false;
				HScroll.Visible = false;
			}
		}
		public virtual bool HScrollVisible {
			get { return hScrollVisible; }
			set { 
				if(HScrollVisible == value) {
					UpdateVisibility();
					return;
				}
				hScrollVisible = value;
				if(value && !ClientRect.IsEmpty)
					HScroll.SetVisibility(value);
				DoScrollInfoChanged();
			}
		}
		public virtual bool VScrollVisible {
			get { return vScrollVisible; }
			set { 
				if(VScrollVisible == value) {
					UpdateVisibility();
					return;
				}
				vScrollVisible = value;
				if(value && !ClientRect.IsEmpty) 
					VScroll.SetVisibility(value);
				DoScrollInfoChanged();
			}
		}
		public virtual void UpdateVisibility() {
			if(HScroll.ActualVisible != hScrollVisible) { 
				if(!hScrollVisible || !ClientRect.IsEmpty)
					HScroll.SetVisibility(hScrollVisible);
			}
			if(VScroll.ActualVisible != vScrollVisible) {
				if(!vScrollVisible || !ClientRect.IsEmpty) 
					VScroll.SetVisibility(vScrollVisible);
			}
		}
		public ScrollArgs HScrollArgs {
			get { return new ScrollArgs(HScroll); }
			set { value.AssignTo(HScroll); }
		}
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { value.AssignTo(VScroll); }
		}
		public virtual int HScrollPos {
			get { return HScroll.Value; }
			set {
				if(value < 0) value = 0;
				if(value > HScroll.Maximum) value = HScroll.Maximum;
				HScroll.Value = value;
			}
		}
		public virtual int VScrollPos {
			get { return VScroll.Value; }
			set {
				if(value < 0) value = 0;
				if(value > VScroll.Maximum) value = VScroll.Maximum;
				VScroll.Value = value;
			}
		}
		public virtual int HScrollRange {
			get { return HScroll.Maximum; }
			set { 
				if(HScrollRange == value ) return;
				if(value <= 0) HScroll.Maximum = HScroll.Value = 0;
				else if(value < HScroll.Value) HScroll.Maximum = HScroll.Value = value;
				else HScroll.Maximum = value; 
			}
		}
		public virtual int VScrollRange {
			get { return VScroll.Maximum; }
			set { 
				if(VScrollRange == value ) return;
				if(value <= 0) VScroll.Maximum = VScroll.Value = 0;
				else if(value < VScroll.Value) VScroll.Maximum = VScroll.Value = value;
				else VScroll.Maximum = value;
			}
		}
		public bool IsOverlapScrollbar { get { return HScroll.IsOverlapScrollBar || VScroll.IsOverlapScrollBar; } }
		public void CheckHScroll() {
			int newPos = HScrollPos;
			if(HScrollPos + HScroll.LargeChange > HScroll.Maximum + 1)
				newPos = HScroll.Maximum + 1 - HScroll.LargeChange;
			HScrollPos = newPos;
		}
		public TreeListHitTest GetHitTest(Point pt) {
			TreeListHitTest result = new TreeListHitTest();
			result.MousePoint = pt;
			if(VScrollVisible && VScroll.Bounds.Contains(pt)) {
				result.HitInfoType = HitInfoType.VScrollBar;
				result.MouseDest = VScroll.Bounds;
				return result;
			}
			if(HScrollVisible && HScroll.Bounds.Contains(pt)) {
				result.HitInfoType = HitInfoType.HScrollBar;
				result.MouseDest = HScroll.Bounds;
				return result;
			}
			return result;
		}
		internal void OnAction(ScrollNotifyAction action) {
			VScroll.OnAction(action);
			HScroll.OnAction(action);
		}
		protected virtual void DoScrollInfoChanged() {
			if(ScrollInfoChanged != null)
				ScrollInfoChanged();
		}
		protected internal void CreateHandles() {
			if(VScroll.TouchMode) return;
			IntPtr fakeHandle;
			if(!VScroll.IsHandleCreated) fakeHandle = VScroll.Handle;
			if(!HScroll.IsHandleCreated) fakeHandle = HScroll.Handle;
		}
	}
}
