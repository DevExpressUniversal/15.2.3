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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	[DesignTimeVisible(false), ToolboxItem(false), Designer("DevExpress.XtraBars.Navigation.Design.AccordionContentContainerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner))]
	public class AccordionContentContainer : XtraScrollableControl {
		AccordionControlElementBase ownerElement;
		internal bool useTransparentColor = false;
		public AccordionContentContainer() {
			this.TabStop = false;
			SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw, true);
			base.BackColor = SystemColors.Control;
			this.ownerElement = null;
			this.padding = new Padding(0);
			LookAndFeel.StyleChanged += OnLookAndFeelStyleChanged;
			ControlAdded += OnControlAdded;
			ControlRemoved += OnControlRemoved;
		}
		void OnControlRemoved(object sender, ControlEventArgs e) {
			UnsubscribeOnXtraResizableControlEvents(e.Control as IXtraResizableControl);
			if(Controls.Count == 1) SubscribeOnXtraResizableControlEvents();
		}
		void OnControlAdded(object sender, ControlEventArgs e) {
			if(Controls.Count == 1) SubscribeOnXtraResizableControlEvents();
			else UnsubscribeOnXtraResizableControlEvents(xtraResizebleControl);
		}
		IXtraResizableControl xtraResizebleControl;
		protected void SubscribeOnXtraResizableControlEvents() {
			if(Controls.Count == 0) return;
			xtraResizebleControl = Controls[0] as IXtraResizableControl;
			if(xtraResizebleControl != null) {
				xtraResizebleControl.Changed += OnXtraResizableControlSizeChanged;
			}
		}
		protected void UnsubscribeOnXtraResizableControlEvents(IXtraResizableControl ctrl) {
			if(ctrl == null) return;
			ctrl.Changed -= OnXtraResizableControlSizeChanged;
			if(object.Equals(ctrl, xtraResizebleControl)) {
				xtraResizebleControl = null;
			}
		}
		void OnXtraResizableControlSizeChanged(object sender, EventArgs e) {
			CalcHeightByContentHeight();
		}
		void CalcHeightByContentHeight() {
			if(((Control)xtraResizebleControl).Dock != DockStyle.Fill)
				return;
			int minHeight = Height;
			int paddingVertical = Math.Max(0, base.Padding.Vertical);
			if(xtraResizebleControl.MaxSize != Size.Empty)
				minHeight = Math.Min(minHeight, xtraResizebleControl.MaxSize.Height + paddingVertical);
			Height = Math.Max(minHeight, xtraResizebleControl.MinSize.Height + paddingVertical);
		}
		AccordionControl ParentAccordionControl { get { return Parent as AccordionControl; } }
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			if(LookAndFeel != null) {
				if(ParentAccordionControl == null) LookAndFeel.ParentLookAndFeel = null;
				else {
					LookAndFeel.ParentLookAndFeel = ParentAccordionControl.LookAndFeel;
					CheckPadding();
				}
			}
		}
		protected void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			CheckPadding();
		}
		protected void CheckPadding() {
			if(this.padding == new Padding(-1)) SetSkinPadding();
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			if(this.xtraResizebleControl != null) {
				CalcHeightByContentHeight();
			}
		}
		Padding padding;
		void ResetPadding() { this.padding = new Padding(0); }
		bool ShouldSerializePadding() { return this.padding != new Padding(0); }
		public new Padding Padding {
			get {
				return padding;
			}
			set {
				base.Padding = padding = value;
				CheckPadding();
			}
		}
		protected System.Windows.Forms.Padding GetSkinPadding() {
			if(ParentAccordionControl == null) return new Padding();
			return ParentAccordionControl.ControlInfo.GetContentContainerPadding();
		}
		protected void SetSkinPadding() {
			base.Padding = GetSkinPadding();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeOnXtraResizableControlEvents(xtraResizebleControl);
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= OnLookAndFeelStyleChanged;
				}
				if(OwnerElement != null) {
					OwnerElement.ResetContentContainer();
					RefreshAccordionControl();
				}
			}
			base.Dispose(disposing);
		}
		protected override bool ShouldSerializeLookAndFeel() {
			if(LookAndFeel == null || LookAndFeel.UseDefaultLookAndFeel) return false;
			return true;
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			return DXAccessible.Accessible;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new AnchorStyles Anchor { get { return base.Anchor; } set { base.Anchor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Location {
			get { return base.Location; }
			set { base.Location = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size Size {
			get { return base.Size; }
			set {
				SuspendLayout();
				try {
					SetBounds(new Rectangle(Location, value));
				}
				finally {
					ResumeLayout(true);
				}
			}
		}
		[Browsable(false)]
		public override bool AutoScroll {
			get { return base.AutoScroll; }
			set { base.AutoScroll = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[ DefaultValue(false)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[Browsable(false)]
		public AccordionControlElement OwnerElement { get { return ownerElement as AccordionControlElement; } }
		protected internal void SetOwnerElement(AccordionControlElementBase ownerElement) {
			if(OwnerElement == ownerElement) return;
			if(OwnerElement != null) OwnerElement.ResetContentContainer();
			this.ownerElement = ownerElement;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = DockStyle.None; }
		}
		protected AccordionControl AccordionControl { get { return OwnerElement == null ? null : OwnerElement.AccordionControl; } }
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(AccordionControl != null) {
				Rectangle destRect = new Rectangle(Point.Empty, Size);
				e.Graphics.FillRectangle(new SolidBrush(BackColor), destRect);
			}
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			height = Math.Max(20, height);
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(OwnerElement != null && OwnerElement.Expanded)
				OwnerElement.InnerContentHeight = Height;
			RefreshAccordionControl();
		}
		protected void RefreshAccordionControl() {
			if(AccordionControl != null) AccordionControl.ForceLayoutChanged();
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor {
			get {
				if(AccordionControl != null)
					return AccordionControl.ControlInfo.GetBackColor();
				return base.BackColor; 
			}
			set { base.BackColor = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get {
				if(AccordionControl != null)
					return AccordionControl.ControlInfo.GetForeColor();
				return base.ForeColor;
			}
			set { base.ForeColor = value; }
		}
		bool internalSizing = false;
		protected internal void SetBounds(Rectangle bounds) {
			if(this.internalSizing)
				return;
			this.internalSizing = true;
			try {
				Bounds = bounds;
			}
			finally {
				this.internalSizing = false;
			}
		}
		protected override Size DefaultSize { get { return new Size(260, 76); } }
		protected override void OnResize(EventArgs e) {
			bool res = this.internalSizing;
			base.OnResize(e);
			if(res) {
				this.internalSizing = res;
				return;
			}
		}
		protected internal virtual void DrawScrollBars(Graphics g) {
			bool vScroll = DrawScrollBar(g, VScrollBar);
			bool hScroll = DrawScrollBar(g, HScrollBar);
			if(vScroll && hScroll && AccordionControl != null) {
				FillTransparentRegion(g);
			}
		}
		protected void FillTransparentRegion(Graphics g) {
			if(HScrollBar.Bounds.X > VScrollBar.Bounds.X) return;
			Rectangle destRect = new Rectangle(HScrollBar.Bounds.Right, VScrollBar.Bounds.Bottom, VScrollBar.Bounds.Width, HScrollBar.Bounds.Height);
			g.FillRectangle(new SolidBrush(BackColor), destRect);
		}
		[SecuritySafeCritical]
		protected bool DrawScrollBar(Graphics g, IScrollView scrollBar) {
			if(scrollBar == null || scrollBar.Bounds == Rectangle.Empty) return false;
			if(scrollBar.TouchMode && ((IScrollView)scrollBar).IsOverlap) return false;
			try {
				using(GraphicsCache tempCache = new GraphicsCache(g)) {
					if(scrollBar.TouchMode) {
						Rectangle bounds = scrollBar.Bounds;
						BackgroundPaintHelper.PaintTransparentBackground(this, new PaintEventArgs(tempCache.Graphics, bounds), bounds);
					}
					else
						scrollBar.DoDraw(tempCache, scrollBar.Bounds);
				}
			}
			catch(Exception e) {
				if(ControlPaintHelper.IsCriticalException(e)) throw;
			}
			return true;
		}
	}
}
