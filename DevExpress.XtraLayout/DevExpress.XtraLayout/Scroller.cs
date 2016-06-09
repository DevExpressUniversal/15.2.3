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

namespace DevExpress.XtraLayout.Scrolling {
	using System.Windows.Forms;
	using System.Drawing;
	using System.ComponentModel;
	using System;
	using DevExpress.XtraEditors;
	internal class VTLScrollBar : DevExpress.XtraEditors.VScrollBar {
		public VTLScrollBar() {
			SetStyle(ControlStyles.Selectable, false);
		}
	}
	internal class DPILabel : Label {
		public override Size MaximumSize {
			get {
				return base.MaximumSize;
			}
			set {
				if(base.MaximumSize != value) {
					base.MaximumSize = value;
					LayoutControl lc = Parent as LayoutControl;
					if(lc != null) lc.Refresh();
				}
			}
		}
	}
	internal class HTLScrollBar : DevExpress.XtraEditors.HScrollBar {
		public HTLScrollBar() {
			SetStyle(ControlStyles.Selectable, false);
		}
	}
	internal delegate void ScrollInfoChangedHandler();
	public class ScrollInfo {
		public  DevExpress.XtraEditors.HScrollBar HScroll;
		public  DevExpress.XtraEditors.VScrollBar VScroll;
		public  Label EmptyArea;
		public int ScrollSize {
			get {
				if(HScroll == null) return SystemInformation.HorizontalScrollBarHeight;
				return HScroll.GetDefaultHorizontalScrollBarHeight();
			}
		}
		Rectangle clientRect;
		bool hScrollVisible, vScrollVisible;
		int autoScrollStep = 20;
		ILayoutControl control;
		public ScrollInfo(ILayoutControl control) {
			this.control = control;
			clientRect = new Rectangle();
			HScroll = new HTLScrollBar();
			VScroll = new VTLScrollBar();
			ScrollBarBase.ApplyUIMode(HScroll);
			ScrollBarBase.ApplyUIMode(VScroll);
			EmptyArea = new DPILabel();
			EmptyArea.MaximumSize = new Size(1000, 1000);
			VScroll.SmallChange = HScroll.SmallChange = 1;
			HScroll.LargeChange = VScroll.LargeChange = 1;
			HScrollPos = HScrollRange = VScrollPos = VScrollRange = 0;
			LayoutControl lc = control as LayoutControl;
			if(lc!=null){
				HScroll.MouseEnter += new EventHandler(lc.ScrollBarMouseEnter);
				VScroll.MouseEnter += new EventHandler(lc.ScrollBarMouseEnter);
				HScroll.MouseLeave += new EventHandler(lc.ScrollBarMouseLeave);
				VScroll.MouseLeave += new EventHandler(lc.ScrollBarMouseLeave);
			}
			SetUseAnchors(false);
		}
		public void Dispose() {
			LayoutControl lc = control as LayoutControl;
			if(lc!=null){
				HScroll.MouseEnter -= new EventHandler(lc.ScrollBarMouseEnter);
				VScroll.MouseEnter -= new EventHandler(lc.ScrollBarMouseEnter);
				HScroll.MouseLeave -= new EventHandler(lc.ScrollBarMouseLeave);
				VScroll.MouseLeave -= new EventHandler(lc.ScrollBarMouseLeave);
			}
			lc=null;
			control = null;
			HScroll.Dispose();
			HScroll = null;
			EmptyArea.Dispose();
			EmptyArea = null;
			VScroll.Dispose();
			VScroll = null;
		}
		public int HorizontalAutoscrollSize {
			get
			{
				return control.Bounds.Width / 6; 
			}
		}
		public int VerticalAutoscrollSize
		{
			get
			{
				return control.Bounds.Height / 6;
			}
		}
		public int AutoScrollStep {
			get { return autoScrollStep;}	 
			set { autoScrollStep = value;}	 
		}
		public void UpdateClientRect(Rectangle rect) {
			if(control != null && WindowsFormsSettings.GetIsRightToLeft(control.Control)) {
				clientRect.Location = Point.Empty;
				clientRect.Width = !VScroll.IsOverlapScrollBar && VScroll.ActualVisible ? rect.Width - VScroll.GetDefaultVerticalScrollBarWidth() : rect.Width;
				clientRect.Height = !HScroll.IsOverlapScrollBar && HScroll.ActualVisible ? rect.Height - HScroll.GetDefaultHorizontalScrollBarHeight() : rect.Height;
				clientRect.Offset(!VScroll.IsOverlapScrollBar && VScroll.ActualVisible ? VScroll.GetDefaultVerticalScrollBarWidth() : 0, 0);
			} else {
				clientRect.Location = Point.Empty;
				clientRect.Width = !VScroll.IsOverlapScrollBar && VScroll.ActualVisible ? rect.Width - VScroll.GetDefaultVerticalScrollBarWidth() : rect.Width;
				clientRect.Height = !VScroll.IsOverlapScrollBar && HScroll.ActualVisible ? rect.Height - HScroll.GetDefaultHorizontalScrollBarHeight() : rect.Height;
			}
			UpdateScrollsPositions();
			UpdateVisibility();
		}
		protected void UpdateScrollsPositions(){
			if(control != null && WindowsFormsSettings.GetIsRightToLeft(control.Control)) {
				if(HScroll.Anchor == AnchorStyles.None) {
					HScroll.SetBounds(ClientRect.X, ClientRect.Bottom - (!HScroll.IsOverlapScrollBar ? 0 : HScroll.GetDefaultHorizontalScrollBarHeight()), ClientRect.Width , HScroll.GetDefaultHorizontalScrollBarHeight());
				}
				if(VScroll.Anchor == AnchorStyles.None) {
					VScroll.SetBounds(ClientRect.X - VScroll.GetDefaultVerticalScrollBarWidth(), ClientRect.Y, VScroll.GetDefaultVerticalScrollBarWidth(), ClientRect.Height);
				}
				EmptyArea.SetBounds(ClientRect.X - VScroll.GetDefaultVerticalScrollBarWidth(), HScroll.Top, VScroll.Width, HScroll.Height);
			} else {
				if(HScroll.Anchor == AnchorStyles.None) {
					HScroll.SetBounds(ClientRect.X, ClientRect.Bottom - (!HScroll.IsOverlapScrollBar ? 0 : HScroll.GetDefaultHorizontalScrollBarHeight()), ClientRect.Width, HScroll.GetDefaultHorizontalScrollBarHeight());
				}
				if(VScroll.Anchor == AnchorStyles.None) {
					VScroll.SetBounds(ClientRect.Right - (!VScroll.IsOverlapScrollBar ? 0 : VScroll.GetDefaultVerticalScrollBarWidth()), ClientRect.Y, VScroll.GetDefaultVerticalScrollBarWidth(), ClientRect.Height);
				}
				EmptyArea.SetBounds(HScroll.Right, HScroll.Top, VScroll.Width, HScroll.Height);
			}
		}
		public Rectangle ClientRect {
			get { return clientRect; }
			set { UpdateClientRect(value);}
		}
		public void SetUseAnchors(bool use) {
			if(use) { 
				HScroll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				VScroll.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			}
			else {
				HScroll.Anchor = AnchorStyles.None;
				VScroll.Anchor = AnchorStyles.None; 
			}
		}
		public bool HScrollVisible {
			get { return hScrollVisible; }
			set { 
				if(HScrollVisible == value) {
					UpdateVisibility();
					return;
				}
				hScrollVisible = value;														 
				if(value && !ClientRect.IsEmpty) 
					HScroll.SetVisibility(value);
			}
		}
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set { 
				if(VScrollVisible == value) {
					UpdateVisibility();
					return;
				}
				vScrollVisible = value;
				if(value && !ClientRect.IsEmpty) 
					VScroll.SetVisibility(value);
				if(WindowsFormsSettings.GetIsRightToLeft(control.Control)) {
					if(value) HScroll.Value += VScroll.GetDefaultVerticalScrollBarWidth();
					else HScroll.Value -= VScroll.GetDefaultVerticalScrollBarWidth();
				}
			}
		}
		public void UpdateVisibility() {
			if(HScroll.ActualVisible != hScrollVisible) { 
				if(!hScrollVisible || !ClientRect.IsEmpty)
					HScroll.SetVisibility(hScrollVisible);
			}
			if(VScroll.ActualVisible != vScrollVisible) {
				if(!vScrollVisible || !ClientRect.IsEmpty) 
					VScroll.SetVisibility(vScrollVisible);
			}
			if(!HScroll.IsOverlapScrollBar)
				EmptyArea.Visible = HScroll.ActualVisible && VScroll.ActualVisible;
			else
				EmptyArea.Visible = false;
			if(HScroll.ActualVisible) HScroll.BringToFront();
			if(VScroll.ActualVisible) VScroll.BringToFront();
			if(EmptyArea.Visible) EmptyArea.BringToFront();
		}
		public ScrollArgs HScrollArgs {
			get { return new ScrollArgs(HScroll); }
			set { if(value != null) value.AssignTo(HScroll); }
		}
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { if(value != null) value.AssignTo(VScroll); }
		}
		public int HScrollPos {
			get { return HScroll.Value; }
			set {
				if(value < 0) value = 0;
				if(value > HScroll.Maximum - HScroll.LargeChange) value = HScroll.Maximum - HScroll.LargeChange;
				if(value < 0) value = 0;
				HScroll.Value = value;
			}
		}
		public int VScrollPos {
			get { return VScroll.Value; }
			set {
				if(value < 0) value = 0;
				if(value > VScroll.Maximum - VScroll.LargeChange) value = VScroll.Maximum - VScroll.LargeChange;
				if(value < 0) value = 0;
				VScroll.Value = value;
			}
		}
		public int HScrollRange {
			get { return HScroll.Maximum; }
			set { 
				if(HScrollRange == value ) return;
				if(value <= 0) HScroll.Maximum = HScroll.Value = 0;
				else if(value < HScroll.Value) HScroll.Maximum = HScroll.Value = value;
				else HScroll.Maximum = value;
			}
		}
		public int VScrollRange {
			get { return VScroll.Maximum; }
			set { 
				if(VScrollRange == value ) return;
				if(value <= 0) VScroll.Maximum = VScroll.Value = 0;
				else if(value < VScroll.Value) VScroll.Maximum = VScroll.Value = value;
				else VScroll.Maximum = value;
			}
		}
		public void CheckHScroll() {
			int newPos = HScrollPos;
			if(HScrollPos + HScroll.LargeChange > HScroll.Maximum + 1)
				newPos = HScroll.Maximum + 1 - HScroll.LargeChange;
			HScrollPos = newPos;
		}
		internal void OnAction(ScrollNotifyAction action) {
			HScroll.OnAction(action);
			VScroll.OnAction(action);
		}
	}
}
