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
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon {
	public class RecentControlContainerItem : RecentItemBase {
		const int DefaultClientHeight = 20;
		RecentControlItemControlContainer controlContainer;
		int clientHeight;
		bool fillSpace;
		public RecentControlContainerItem()
			: base() {
			this.fillSpace = false;
			this.clientHeight = DefaultClientHeight;
		}
		[DefaultValue(false)]
		public bool FillSpace {
			get { return fillSpace; }
			set {
				if(FillSpace == value) return;
				fillSpace = value;
				OnItemChanged();
			}
		}
		[DefaultValue(DefaultClientHeight)]
		public int ClientHeight {
			get { return clientHeight; }
			set {
				if(ClientHeight == value) return;
				clientHeight = value;
				OnItemChanged();
			}
		}
		protected override RecentItemViewInfoBase CreateItemViewInfo() {
			return new RecentControlContainerItemViewInfo(this);
		}
		protected override RecentItemPainterBase CreateItemPainter() {
			return new RecentControlContainerItemPainter();
		}
		protected override RecentItemHandlerBase CreateItemHandler() {
			return new RecentControlContainerItemHandler(this);
		}
		[DefaultValue(null), Browsable(false)]
		public virtual RecentControlItemControlContainer ControlContainer {
			get { return controlContainer; }
			set {
				if(ControlContainer == value) return;
				DestroyContainer();
				controlContainer = value;
				if(ControlContainer != null) {
					InitContainer(ControlContainer);
				}
				OnItemChanged();
			}
		}
		protected virtual void DestroyContainer() {
			if(ControlContainer == null) return;
			ControlContainer.SetOwnerItem(null);
			ControlContainer.Dispose();
			this.controlContainer = null;
		}
		protected virtual void InitContainer(RecentControlItemControlContainer container) {
			if(container == null) container = CreateControlContainer();
			this.controlContainer = container;
			ControlContainer.SetOwnerItem(this);
			ControlContainer.Visible = false;
			if(RecentControl == null ) return;
			RecentControl.Controls.Add(ControlContainer);
		}
		protected internal override void OnRecentControlChanged(RecentItemControl rc) {
			base.OnRecentControlChanged(rc);
			if(rc != null && ControlContainer == null) InitContainer(null);
		}
		protected override void OnOwnerPanelChanged() {
			base.OnOwnerPanelChanged();
			if(OwnerPanel != null && OwnerPanel.RecentControl != null) InitContainer(null);
		}
		private RecentControlItemControlContainer CreateControlContainer() {
			IDesignerHost host = RecentControl.InternalGetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return new RecentControlItemControlContainer();
			return host.CreateComponent(typeof(RecentControlItemControlContainer)) as RecentControlItemControlContainer;
		}
		protected internal virtual void UpdateControlContainer(Rectangle clientBounds) {
			if(ControlContainer == null) return;
			if(clientBounds.IsEmpty) SetControlContainerVisibility(false);
			else {
				SetControlContainerBounds(clientBounds);
				SetControlContainerVisibility(true);
			}
		}
		protected void SetControlContainerBounds(Rectangle bounds) {
			if(ControlContainer == null) return;
			if(RecentControl != null && RecentControl.IsDesignMode) {
				ControlContainer.SetBounds(bounds);
				return;
			}
			Point location = Point.Empty;
			Size containerSize = bounds.Size;
			controlContainer.SetBounds(bounds);
		}
		protected virtual void SetControlContainerVisibility(bool visible) {
			if(!CanUpdateControlContainerVisibility(visible)) return;
			ControlContainer.Visible = visible;
		}
		protected virtual bool CanUpdateControlContainerVisibility(bool visible) {
			if(ControlContainer == null) return false;
			if(visible || RecentControl == null) return true;
			return false;
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				DestroyContainer();
			base.Dispose(disposing);
		}
	}
	[ToolboxItem(false), Designer("DevExpress.XtraBars.Ribbon.Design.RecentControlContentContainerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner))]
	public class RecentControlItemControlContainer : XtraScrollableControl, ITransparentBackgroundManager, ITransparentBackgroundManagerEx {
		RecentControlContainerItem ownerItem;
		public RecentControlItemControlContainer() {
			this.TabStop = false;
			SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw, true);
			base.BackColor = SystemColors.Control;
			this.ownerItem = null;
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
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DockStyle Dock {
			get { return base.Dock; }
			set { base.Dock = DockStyle.None; }
		}
		public RecentControlContainerItem OwnerItem { get { return ownerItem; } }
		protected internal void SetOwnerItem(RecentControlContainerItem ownerItem) {
			this.ownerItem = ownerItem;
		}
		protected RecentItemControl RecentControl { get { return OwnerItem != null ? OwnerItem.RecentControl : null; } }
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e); 
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			height = Math.Max(20, height);
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
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
		protected override void OnResize(EventArgs e) {
			bool res = this.internalSizing;
			base.OnResize(e);
			if(res) {
				this.internalSizing = res;
				return;
			}
			if(ShouldAdjustGroupHeight) {
				OwnerItem.ClientHeight = Height;
			}
		}
		protected virtual bool ShouldAdjustGroupHeight {
			get {
				if(DesignMode) return true;
				return OwnerItem != null && IsHandleCreated;
			}
		}
		#region TransparentBackground
		Color ITransparentBackgroundManager.GetForeColor(object childObject) {
			return Color.Empty;
		}
		Color ITransparentBackgroundManager.GetForeColor(Control childControl) {
			return Color.Transparent;
		}
		Color ITransparentBackgroundManagerEx.GetEmptyBackColor(Control childControl) {
			return Color.Transparent;
		}
		#endregion
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor {
			get {
				if(OwnerItem != null && OwnerItem.RecentControl != null)
					return CommonSkins.GetSkin(OwnerItem.RecentControl.LookAndFeel.ActiveLookAndFeel).Colors[CommonColors.Control];
				return base.BackColor;
			}
			set { base.BackColor = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get {
				if(OwnerItem != null && OwnerItem.RecentControl != null)
					return CommonSkins.GetSkin(OwnerItem.RecentControl.LookAndFeel.ActiveLookAndFeel).Colors[CommonColors.ControlText];
				return base.ForeColor;
			}
			set { base.ForeColor = value; }
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RecentControlContainerItemViewInfo : RecentItemViewInfoBase {
		public RecentControlContainerItemViewInfo(RecentControlContainerItem item) : base(item) { }
		RecentControlContainerItem ContainerItem { get { return Item as RecentControlContainerItem; } }
		Size DefaultClientSize { get { return new Size(300, 50); } }
		protected override Size CalcBestSizeCore(int width) {
			Size size = base.CalcBestSizeCore(width);
			return new Size(width, ContainerItem.ClientHeight);
		}
		protected override Padding GetSkinItemPadding() {
			return new Padding(5, 5, 5, 5);
		}
		public override void CalcViewInfo(Rectangle bounds) {
			base.CalcViewInfo(bounds);
			UpdateContainerBounds();
		}
		void UpdateContainerBounds() {
			if(ContainerItem.ControlContainer != null)
				ContainerItem.UpdateControlContainer(ContentBounds);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RecentControlContainerItemHandler : RecentItemHandlerBase {
		public RecentControlContainerItemHandler(RecentControlContainerItem item) : base(item) { }
	}
}
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class RecentControlContainerItemPainter : RecentItemPainterBase {
		protected override void DrawBackground(DevExpress.Utils.Drawing.GraphicsCache cache, RecentItemViewInfoBase itemBaseInfo) {
			base.DrawBackground(cache, itemBaseInfo);
		}
	}
}
