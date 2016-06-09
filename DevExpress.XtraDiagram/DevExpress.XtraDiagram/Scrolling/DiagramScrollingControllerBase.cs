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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
namespace DevExpress.XtraDiagram.Scrolling {
	public abstract class DiagramScrollingControllerBase : IDisposable {
		BaseControl owner;
		HScrollBar hScroll;
		VScrollBar vScroll;
		bool hScrollVisible, vScrollVisible;
		Rectangle clientRect, hScrollRect, vScrollRect;
		public DiagramScrollingControllerBase(BaseControl owner) {
			this.owner = owner;
			this.clientRect = this.hScrollRect = this.vScrollRect = Rectangle.Empty;
			this.hScrollVisible = this.vScrollVisible = false;
			this.hScroll = CreateHScroll();
			InitDefaults(HScroll);
			this.vScroll = CreateVScroll();
			InitDefaults(VScroll);
			ScrollBarBase.ApplyUIMode(HScroll);
			ScrollBarBase.ApplyUIMode(VScroll);
		}
		protected virtual void InitDefaults(ScrollBarBase scrollBar) {
			scrollBar.Visible = false;
			scrollBar.SmallChange = 1;
			scrollBar.LookAndFeel.ParentLookAndFeel = owner.LookAndFeel;
		}
		protected virtual HScrollBar CreateHScroll() { return new HScrollBar(); }
		protected virtual VScrollBar CreateVScroll() { return new VScrollBar(); }
		public virtual void AddControls(BaseControl container) {
			if(container != null) {
				container.Controls.Add(HScroll);
				container.Controls.Add(VScroll);
			}
		}
		public virtual void RemoveControls(BaseControl container) {
			if(container != null) {
				container.Controls.Remove(HScroll);
				container.Controls.Remove(VScroll);
			}
		}
		public virtual int HScrollHeight {
			get { return HScroll.GetDefaultHorizontalScrollBarHeight(); }
		}
		public virtual int VScrollWidth {
			get { return VScroll.GetDefaultVerticalScrollBarWidth(); }
		}
		public int HScrollPosition { get { return HScroll.Value; } }
		public int VScrollPosition { get { return VScroll.Value; } }
		public HScrollBar HScroll { get { return hScroll; } }
		public VScrollBar VScroll { get { return vScroll; } }
		public Point GetScrollPos() {
			return new Point(HScrollPosition, VScrollPosition);
		}
		public BaseControl Owner { get { return owner; } }
		public ScrollArgs HScrollArgs {
			get { return new ScrollArgs(HScroll); }
			set { value.AssignTo(HScroll); }
		}
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { value.AssignTo(VScroll); }
		}
		public Rectangle HScrollRect { get { return hScrollRect; } }
		public Rectangle VScrollRect { get { return vScrollRect; } }
		public int HScrollMaximum { get { return HScroll.Maximum; } }
		public int VScrollMaximum { get { return VScroll.Maximum; } }
		public int HScrollLargeChange { get { return HScroll.LargeChange; } }
		public int VScrollLargeChange { get { return VScroll.LargeChange; } }
		public bool HScrollVisible {
			get { return hScrollVisible; }
			set {
				if(HScrollVisible == value) UpdateVisiblity();
				else {
					hScrollVisible = value;
					LayoutChanged();
				}
			}
		}
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set {
				if(VScrollVisible == value) UpdateVisiblity();
				else {
					vScrollVisible = value;
					LayoutChanged();
				}
			}
		}
		public Rectangle ClientRect {
			get { return clientRect; }
			set {
				if(ClientRect == value) return;
				clientRect = value;
				LayoutChanged();
			}
		}
		protected virtual void CalcRects() {
			this.vScrollRect = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			if(HScrollVisible) {
				r.Location = new Point(ClientRect.X, ClientRect.Bottom - HScrollHeight);
				r.Size = new Size((ClientRect.Width - (VScrollVisible ? VScrollWidth : 0)), HScrollHeight);
				hScrollRect = r;
			}
			if(VScrollVisible) {
				r.Location = new Point(ClientRect.Right - VScrollWidth, ClientRect.Y);
				r.Size = new Size(VScrollWidth, ClientRect.Height);
				if(HScrollVisible) {
					r.Height -= HScrollHeight;
				}
				vScrollRect = r;
			}
		}
		public void UpdateVisiblity() {
			HScroll.SetVisibility(hScrollVisible && !ClientRect.IsEmpty);
			HScroll.Bounds = HScrollRect;
			VScroll.SetVisibility(vScrollVisible && !ClientRect.IsEmpty);
			VScroll.Bounds = VScrollRect;
		}
		int lockLayout = 0;
		public virtual void LayoutChanged() {
			if(lockLayout != 0) return;
			lockLayout++;
			try {
				CalcRects();
				UpdateVisiblity();
				if(ClientRect.IsEmpty) {
					HScroll.SetVisibility(false);
					VScroll.SetVisibility(false);
				}
			}
			finally {
				lockLayout--;
			}
		}
		public event EventHandler HScrollValueChanged {
			add { HScroll.ValueChanged += value; }
			remove { HScroll.ValueChanged -= value; }
		}
		public event EventHandler VScrollValueChanged {
			add { VScroll.ValueChanged += value; }
			remove { VScroll.ValueChanged -= value; }
		}
		internal void OnAction(ScrollNotifyAction action) {
			HScroll.OnAction(action);
			VScroll.OnAction(action);
		}
		#region IDispose
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.hScroll != null) {
					this.hScroll.Dispose();
					this.hScroll = null;
				}
				if(this.vScroll != null) {
					this.vScroll.Dispose();
					this.vScroll = null;
				}
			}
		}
		#endregion
	}
}
