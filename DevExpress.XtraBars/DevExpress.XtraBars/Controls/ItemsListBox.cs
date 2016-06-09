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
using System.ComponentModel;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Customization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Styles;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Customization {
	[ToolboxItem(false)]
	public class BarItemsListBox : ItemsListBox {
		BarManager manager;
		bool allowDrag = true;
		Control destinationDropControl = null;
		public BarItemsListBox() {
			Appearance.BackColor = Color.White;
			Appearance.Options.UseBackColor = true;
			ItemHeight = 16;
		}
		[DefaultValue(true)]
		public bool AllowDrag { get { return allowDrag; } set { allowDrag = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control DestinationDropControl {
			get { return destinationDropControl; }
			set { destinationDropControl = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarManager Manager {
			get { return manager; }
			set { manager = value; }
		}
		public void AddItems(ICollection items) {
			BeginUpdate();
			try {
				Items.Clear();
				foreach(BarItem item in items) {
					if(item.ShowInCustomizationForm)
						Items.Add(item);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public void FillListBox(BarManagerCategory category, int index) { 
			if(Manager == null) return;
			BeginUpdate();
			try {
				Items.BeginUpdate();
				Items.Clear();
				foreach(BarItem barItem in Manager.Items) {
					if(!barItem.ShowInCustomizationForm)
						continue;
					if(barItem.Category == category || category == BarManagerCategory.TotalCategory) {
						Items.Add(barItem);
					}
				}
			} finally { 
				Items.EndUpdate();
				EndUpdate(); 
			}
			if(index > Items.Count)
				index = Items.Count - 1;
			if(index > -1 && index < Items.Count)
				SelectedIndex = index;
		}
		protected override void RaiseMeasureItem(MeasureItemEventArgs e) {
			base.RaiseMeasureItem(e);
			e.ItemHeight = CalcItemSize(GetBarItem(e.Index)).Height;
		}
		protected override void RaiseDrawItem(DevExpress.XtraEditors.ListBoxDrawItemEventArgs e) {
			base.RaiseDrawItem(e);
			DrawBarItem(e.Graphics, e.Bounds, GetBarItem(e.Index), (e.State & DrawItemState.Selected) != 0);
			e.Handled = true;
		}
		public void CreateNewItem(BarItemInfo info, object arguments, RibbonControl ribbon, BarManagerCategory category) { 
			BarItem item = info.CreateItem();
			ribbon.Container.Add(item);
			item.Caption = item.Name;
			if(category != null) item.Category = category;
			ribbon.Items.Add(item);
			if(!Items.Contains(item)) Items.Add(item); 
			item.UpdateId();
			item.OnItemCreated(arguments);
			SelectedItem = item;
		}
		public BarItem GetBarItem(int index) {
			if(index < 0 || index >= Items.Count) return null;
			BarItemLink link = Items[index] as BarItemLink;
			if(link != null) return link.Item;
			return Items[index] as BarItem;
		}
		void OnCalcGlyphSize(object sender, BarLinkGetValueEventArgs e) {
			Size size = (Size)e.Value;
			if(!size.IsEmpty) {
				BarLinkViewInfo vi = sender as BarLinkViewInfo;
				size = vi.UpdateGlyphSize(new Size(16, 16), true);
				e.Value = size;
			}
		}
		protected virtual BarItemLink CreateCustomLink(BarItemLink link) {
			BarQMenuCustomizationItem qi = new BarQMenuCustomizationItem(null, Manager, false);
			qi.ButtonStyle = BarButtonStyle.Default;
			qi.Tag = link;
			return qi.CreateLink(null, this);
		}
		void SubscribeEvents(BarLinkViewInfo linkInfo) {
			BarQMenuCustomizationLinkViewInfo vi = linkInfo as BarQMenuCustomizationLinkViewInfo;
			if(linkInfo != null) linkInfo.GlyphSizeEvent += new BarLinkGetValueEventHandler(OnCalcGlyphSize);
			if(vi != null && vi.InnerViewInfo != null) vi.InnerViewInfo.GlyphSizeEvent += new BarLinkGetValueEventHandler(OnCalcGlyphSize);
		}
		void UnSubscribeEvents(BarLinkViewInfo linkInfo) {
			BarQMenuCustomizationLinkViewInfo vi = linkInfo as BarQMenuCustomizationLinkViewInfo;
			if(linkInfo != null) linkInfo.GlyphSizeEvent -= new BarLinkGetValueEventHandler(OnCalcGlyphSize);
			if(vi != null && vi.InnerViewInfo != null) vi.InnerViewInfo.GlyphSizeEvent -= new BarLinkGetValueEventHandler(OnCalcGlyphSize);
		}
		protected Size CalcItemSize(BarItem barItem) {
			if(barItem == null) return Size.Empty;
			BarQMenuCustomizationLinkViewInfo vi = (BarQMenuCustomizationLinkViewInfo)CreateCustomLink(barItem.CreateLink(null, this)).CreateViewInfo();
			UpdateLinkViewInfo(vi);
			SubscribeEvents(vi);
			Size res = vi.CalcLinkSize(null, null);
			UnSubscribeEvents(vi);
			return res;
		}
		protected virtual void UpdateLinkViewInfo(BarQMenuCustomizationLinkViewInfo vi) {
			if(Manager.IsDesignMode) {
				vi.InnerViewInfo.ShortCutDisplayText = string.Format("({0})", vi.InnerLink.Item.Name);
			}
		}
		public override Color BackColor {
			get {
				if(Manager != null) {
					return BarSkins.GetSkin(Manager.GetController().LookAndFeel.ActiveLookAndFeel).GetSystemColor(base.BackColor);
				}
				return base.BackColor;
			}
			set {
				base.BackColor = value;
			}
		}
		protected void DrawBarItem(Graphics g, Rectangle bounds, BarItem barItem, bool focused) {
			if(bounds.Width < 1 || bounds.Height < 1 || barItem == null) return;
			BarItemLink link = CreateCustomLink(barItem.CreateLink(null, this));
			if(link == null) return;
			BarQMenuCustomizationLinkViewInfo vi = (BarQMenuCustomizationLinkViewInfo)link.CreateViewInfo();
			UpdateLinkViewInfo(vi);
			SubscribeEvents(vi);
			vi.Bounds = bounds;
			vi.CalcViewInfo(g, this, bounds);
			vi.UpdateLinkWidthInSubMenu(bounds.Width);
			if(focused) vi.LinkState |= BarLinkState.Highlighted;
			GraphicsInfoArgs dra = new GraphicsInfoArgs(new GraphicsCache(new PaintEventArgs(g, Rectangle.Empty)), Rectangle.Empty);
			vi.Painter.Draw(dra, vi, null);
			UnSubscribeEvents(vi);
			dra.Cache.Dispose();
		}
		Point downPoint = Point.Empty;
		BarItem downItem = null, draggingItem;
		public BarItem DraggingItem { get { return draggingItem; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if((e.Button & MouseButtons.Left) != 0) {
				this.downPoint = Control.MousePosition;
				this.downItem = GetBarItem(IndexFromPoint(new Point(e.X, e.Y)));
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			Capture = false;
			this.downItem = null;
		}
		protected virtual void DoDragDrop(MouseEventArgs e) { 
			Manager.SelectionInfo.CustomizeSelectedLink = null;
			Manager.SelectionInfo.SelectedItem = this.downItem;
			try {
				this.draggingItem = this.downItem;
				if(DestinationDropControl != null) DestinationDropControl.AllowDrop = true;
				Manager.Helper.DragManager.UseDefaultCursors = true;
				Manager.Helper.DragManager.StartDragging(null, e, downItem, this);
			}
			finally {
				Manager.Helper.DragManager.UseDefaultCursors = false;
				if(DestinationDropControl != null) DestinationDropControl.AllowDrop = false;
				Manager.SelectionInfo.SelectedItem = null;
				this.draggingItem = null;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if((e.Button & MouseButtons.Left) != 0 && this.downItem != null) {
				Point p = Control.MousePosition;
				p.Offset(-downPoint.X, -downPoint.Y);
				if(Math.Abs(p.X) > 4 || Math.Abs(p.Y) > 4) DoDragDrop(e);
			}
		}
		protected virtual void OnQueryContinueDragCore(QueryContinueDragEventArgs e) { 
			Manager.Helper.DragManager.ItemOnQueryContinueDrag(e, null);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			base.OnQueryContinueDrag(e);
			OnQueryContinueDragCore(e);
		}
		protected virtual void OnGiveFeedbackCore(GiveFeedbackEventArgs e) {
			Manager.Helper.DragManager.ItemOnGiveFeedback(e, null);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			base.OnGiveFeedback(e);
			OnGiveFeedbackCore(e);
		}
	}
	[ToolboxItem(false)]
	public class BarItemLinksControl : CustomPopupBarControl {
		BarLinksHolder holder;
		BarManager manager;
		public BarItemLinksControl() : base(null, null) { 
			Enabled = false;
		}
		public override BarManager Manager {
			get { return manager; }
		}
		public BarLinksHolder Holder { get { return holder; } }
		public BarItemLinkCollection Links {
			get { return Holder == null ? null : Holder.ItemLinks; }
		}
		public void Setup(BarManager manager, BarLinksHolder holder) {
			this.manager = manager;
			this.holder = holder;
			Init();
			LayoutChanged();
			Enabled = Manager != null;
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(Manager == null || Links == null) {
				e.Graphics.FillRectangle(Brushes.White, ClientRectangle);
				return;
			}
			try {
				base.OnPaint(e);
			}
			catch {
				LayoutChanged();
			}
		}
		protected override bool AllowAnimation { get { return false; } }
		protected override bool IsAllowShowMenusOnRightClick { get { return false; } } 
		public override bool CanOpenAsChild(IPopup popup) {
			SubMenuBarControl sub = popup as SubMenuBarControl;
			if(sub != null && VisibleLinks.Contains(sub.ContainerLink)) return true;
			if(VisibleLinks.Contains(popup.OwnerLink)) return true;
			return false;
		}
		protected internal override void UpdateVisibleLinks() {
			if(ViewInfo == null) return;
			base.UpdateVisibleLinks();
			AddVisibleLinks(VisibleLinks, Links);
			BarItemLink designLink = ControlLinks[ControlLink.DesignTimeLink];
			if(designLink != null) VisibleLinks.AddItem(designLink);
			ControlLinks.UpdateLinkedObject(Holder);
		}
		protected internal override void MakeLinkVisible(BarItemLink link) {
			CustomSubMenuBarControlViewInfo vi = ViewInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null) return;
			vi.MakeLinkVisible(link);
		}
		protected internal virtual void ForceLayout() {
			int prevIndex = TopLinkIndex;
			LayoutChanged();
			TopLinkIndex = prevIndex;
		}
		protected override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);
			ForceLayout();
		}
		protected override void OnLeftMouseDown(DXMouseEventArgs e, BarItemLink link) {
			if(CanHighlight(link)) {
				base.OnLeftMouseDown(e, link);
				return;
			}
			if(link != null && Manager != null) {
				OnSelectLink(link);
			}
		}
		protected virtual void OnSelectLink(BarItemLink link) {
			Manager.SelectionInfo.CustomizeSelectLink(link);
		}
		protected override void OnRightMouseDown(DXMouseEventArgs e, BarItemLink link) {
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(ViewInfo == null) return;
			BarItemLink link = GetLinkByPoint(Cursor.Position);
			if(CanHighlight(link)) {
				base.OnMouseMove(e);
				return;
			}
			if(Manager.SelectionInfo.CustomizeSelectedLink != null) {
				if(link != null) {
					bool prevAllowDrop = AllowDrop;
					AllowDrop = true;
					if(Manager.Helper.CustomizationManager.CheckAndCustomizationMouseMove(this, e, MouseDownPoint, Manager.SelectionInfo.CustomizeSelectedLink)) {
						ForceLayout();
					}
					AllowDrop = prevAllowDrop;
				}
			}
		}
		protected override void OnMouseEnter(EventArgs e) {
			if(Manager != null && !CanHighlight(Manager.SelectionInfo.HighlightedLink))
				Manager.SelectionInfo.HighlightedLink = null;
		}
		public override bool CanHighlight(BarItemLink link) {
			return base.CanHighlight(link) && (
				(link is BarScrollItemLink) || (link is BarDesignTimeItemLink));
		}
	}
	public class BarItemLinksControlViewInfo : CustomSubMenuBarControlViewInfo {
		public virtual new BarItemLinksControl BarControl { get { return base.BarControl as BarItemLinksControl; } }
		public BarItemLinksControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar)
			: base(manager, parameters, bar) {
		}
		public override void CalcMaxWidthes() {
			base.CalcMaxWidthes();
			this.MaxLinkWidth = BarControl.Width;
		}
		protected internal override MenuDrawMode DrawMode { 
			get {
				if(BarControl == null || BarControl.Holder == null) return MenuDrawMode.Default;
				MenuDrawMode res = BarControl.Holder.MenuDrawMode;
				if(res != MenuDrawMode.Default) return res;
				if(BarControl.Holder is ApplicationMenu) return MenuDrawMode.LargeImagesText;
				return MenuDrawMode.Default; 
			} 
		}
		protected internal override bool ShowMenuCaption {
			get { return false; }
		}
		public override string MenuCaption {
			get { return string.Empty; }
		}
	}
}
