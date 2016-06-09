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
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Customization.Helpers;
using DevExpress.XtraBars.InternalItems;
namespace DevExpress.XtraBars.Ribbon {
	public class RibbonLinkDragManager : BarLinkDragManager {
		internal class RibbonDragControl : BarDragControl {
			Control ctrl = null;
			internal RibbonDragControl(BarLinkDragManager dragManager, Control ctrl) : base(dragManager) {
				this.ctrl = ctrl;
			}
			public RibbonControl Ribbon { get { return Control as RibbonControl; } }
			public override Control Control { get { return ctrl; } }
		}
		protected internal class RibbonDragHitInfo : DragHitInfo {
			RibbonPage page;
			RibbonPageGroup pageGroup;
			RibbonQuickAccessToolbar toolbar;
			GalleryItem galleryItem;
			GalleryItemGroup galleryItemGroup;
			BaseGallery gallery;
			RibbonPageCategory pageCategory;
			RibbonStatusBar statusBar;
			RibbonPageHeaderItemLinkCollection pageHeaderItemLinks;
			public RibbonDragHitInfo() : base() { }
			public RibbonPage Page { get { return page; } set { page = value; } }
			public RibbonStatusBar StatusBar { get { return statusBar; } set { statusBar = value; } }
			public RibbonPageCategory PageCategory { get { return pageCategory; } set { pageCategory = value; } }
			public BaseGallery Gallery { get { return gallery; } set { gallery = value; } }
			public RibbonPageGroup PageGroup { get { return pageGroup; } set { pageGroup = value; } }
			public RibbonQuickAccessToolbar Toolbar { get { return toolbar; } set { toolbar = value; } }
			public RibbonPageHeaderItemLinkCollection PageHeaderItemLinks { get { return pageHeaderItemLinks; } set { pageHeaderItemLinks = value; } }
			public GalleryItem GalleryItem { get { return galleryItem; } set { galleryItem = value; } }
			public GalleryItemGroup GalleryItemGroup { get { return galleryItemGroup; } set { galleryItemGroup = value; } }
		}
		public GalleryItem DragGalleryItem { get { return DragObject as GalleryItem; } }
		public GalleryItemGroup DragGalleryItemGroup { get { return DragObject as GalleryItemGroup; } }
		public RibbonPage DragPage { get { return DragObject as RibbonPage; } }
		public RibbonPageCategory DragPageCategory { get { return DragObject as RibbonPageCategory; } }
		public RibbonPageGroup DragPageGroup { get { return DragObject as RibbonPageGroup; } }
		public RibbonQuickAccessToolbar DragToolbar { get { return DragObject as RibbonQuickAccessToolbar; } }
		public BarItemLink DragLink { get { return DragObject as BarItemLink; } }
		public override bool CanAcceptDragObject(IDataObject data) {
			object obj = GetDraggingObject(data);
			if(obj is RibbonPage || obj is RibbonPageGroup || obj is RibbonQuickAccessToolbar) return true;
			if(obj is GalleryItem || obj is GalleryItemGroup || obj is BaseGallery) return true;
			if(obj is RibbonPageCategory) return true;
			return base.CanAcceptDragObject(data);
		}
		GalleryDropDown selectedDropDownGallery = null;
		System.Windows.Forms.Timer scrollTimer;
		public RibbonLinkDragManager(RibbonBarManager manager) : base(manager) { }
		public new RibbonBarManager Manager { get { return base.Manager as RibbonBarManager; } }
		protected internal GalleryDropDown SelectedDropDownGallery { get { return selectedDropDownGallery; } set { selectedDropDownGallery = value; } }
		public RibbonControl Ribbon { get { return Manager.Ribbon; } }
		protected virtual bool CalcInsertType(Point pt, BarItemLink link) {
			if(link == null || link.RibbonItemInfo == null) return false;
			return CalcInsertType(link.RibbonItemInfo.Bounds, pt);
		}
		protected internal override BarLinkDragManager.BarDragControl CreateDragControl(Control ctrl) { return new RibbonDragControl(this, ctrl); }
		Point scrollPoint = Point.Empty;
		Rectangle upScrollRect = Rectangle.Empty, downScrollRect = Rectangle.Empty;
		protected void InitScrollTimer() {
			int interval = 50;
			if(this.scrollTimer == null) {
				interval = 300;
				DestroyScrollTimer();
				this.scrollTimer = new System.Windows.Forms.Timer();
				this.scrollTimer.Tick += new EventHandler(OnScrollTimerTick);
			}
			this.scrollTimer.Interval = interval;
			this.scrollTimer.Enabled = true;
		}
		protected void DestroyScrollTimer() {
			if(ScrollTimer != null) scrollTimer.Dispose();
			this.scrollTimer = null;
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			if(SelectedDropDownGallery == null || !UpdateDropDownGalleryScroll()) DestroyScrollTimer();
		}
		protected Rectangle DropDownGalleryUpScrollRect { get { return upScrollRect; } }
		protected Rectangle DropDownGalleryDownScrollRect { get { return downScrollRect; } }
		protected bool UpdateDropDownGalleryScroll() {
			int value = SelectedDropDownGallery.Gallery.ScrollBar.SmallChange;
			if(DropDownGalleryUpScrollRect.Contains(ScrollPoint)) {
				SelectedDropDownGallery.Gallery.ScrollBar.Value -= value;
				SelectedDropDownGallery.Gallery.OnScrollCore(SelectedDropDownGallery.Gallery.ScrollBar, new ScrollEventArgs(ScrollEventType.SmallDecrement, SelectedDropDownGallery.Gallery.ScrollBar.Value));
			}
			else if(DropDownGalleryDownScrollRect.Contains(ScrollPoint)) {
				SelectedDropDownGallery.Gallery.ScrollBar.Value += value;
				SelectedDropDownGallery.Gallery.OnScrollCore(SelectedDropDownGallery.Gallery.ScrollBar, new ScrollEventArgs(ScrollEventType.SmallIncrement, SelectedDropDownGallery.Gallery.ScrollBar.Value));
			}
			else return false;
			return true;
		}
		protected Point ScrollPoint { get { return scrollPoint; } }
		protected System.Windows.Forms.Timer ScrollTimer { get { return scrollTimer; } }
		protected virtual void CheckScrollDropDownGallery(RibbonHitInfo hitInfo, RibbonControl rcontrol) {
			if(SelectedDropDownGallery == null) return;
			int value =  SelectedDropDownGallery.Gallery.ScrollBar.SmallChange;
			this.scrollPoint = hitInfo.HitPoint;
			ScrollEventType eventType;
			if(DropDownGalleryUpScrollRect.Contains(hitInfo.HitPoint)) {
				value = -value;
				eventType = ScrollEventType.SmallDecrement;
			}
			else if(DropDownGalleryDownScrollRect.Contains(hitInfo.HitPoint)) {
				eventType = ScrollEventType.SmallIncrement;
			}
			else {
				DestroyScrollTimer();
				return;
			}
			if(ScrollTimer == null) {
				SelectedDropDownGallery.Gallery.ScrollBar.Value += value;
				SelectedDropDownGallery.Gallery.OnScrollCore(SelectedDropDownGallery.Gallery.ScrollBar, new ScrollEventArgs(eventType, SelectedDropDownGallery.Gallery.ScrollBar.Value));
			}
			InitScrollTimer();
		}
		protected virtual bool ShouldOpenDropDown(RibbonHitInfo hitInfo) {
			RibbonGalleryBarItemLink galleryLink = hitInfo.Item as RibbonGalleryBarItemLink;
			if(galleryLink == null) return false;
			if(DragLink != null && (DragLink is BarButtonGroupLink || DragLink is RibbonGalleryBarItemLink || galleryLink.Gallery.GalleryDropDown == null)) return false;
			if(DragLink == null && DragGalleryItem == null && DragGalleryItemGroup == null) return false;
			if(hitInfo.HitTest != RibbonHitTest.GalleryDropDownButton) return false;
			return true;
		}
		public virtual void CalcDropDownScrollRects() {
			this.upScrollRect = SelectedDropDownGallery.Gallery.ViewInfo.GalleryContentBounds;
			this.upScrollRect.Height = 16;
			this.downScrollRect = DropDownGalleryUpScrollRect;
			this.downScrollRect.Height = 16;
			this.downScrollRect.Y = SelectedDropDownGallery.Gallery.ViewInfo.GalleryContentBounds.Bottom - this.downScrollRect.Height;
		}
		protected virtual void CheckDropDownGallery(RibbonHitInfo hitInfo, RibbonControl rcontrol) {
			RibbonGalleryBarItemLink galleryLink = hitInfo.Item as RibbonGalleryBarItemLink;
			if(galleryLink == null) return;
			if(ShouldOpenDropDown(hitInfo)) {
				if(galleryLink.Opened) return;
				galleryLink.OnPressImageGallery(new DXMouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), hitInfo);
				SelectedDropDownGallery = galleryLink.DropDownCore;
				CalcDropDownScrollRects();	
			}
		}
		protected virtual void CheckScrollGallery(RibbonHitInfo hitInfo, RibbonControl rcontrol) {
			if(DragGalleryItem == null && DragGalleryItemGroup == null) return;
			RibbonGalleryBarItemLink galleryLink = hitInfo.Item as RibbonGalleryBarItemLink;
			if(galleryLink == null) return;
			if(hitInfo.HitTest == RibbonHitTest.GalleryLeftButton ||
				hitInfo.HitTest == RibbonHitTest.GalleryRightButton ||
				hitInfo.HitTest == RibbonHitTest.GalleryDownButton ||
				hitInfo.HitTest == RibbonHitTest.GalleryUpButton) {
				galleryLink.OnPressImageGallery(new DXMouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), hitInfo);
			}
			else {
				galleryLink.OnUnPressImageGallery(new DXMouseEventArgs(MouseButtons.None, 1, 0, 0, 0), hitInfo);
			}
		}
		protected RibbonSelectionInfo RibbonSelectionInfo { get { return Manager.SelectionInfo as RibbonSelectionInfo; } }
		protected virtual bool ShouldProcessHitInfo(RibbonHitInfo info) { return info != null && info.HitTest != RibbonHitTest.None; }
		protected virtual RibbonDragHitInfo CalcDragGalleryInfo(RibbonDragHitInfo hitInfo, RibbonControl rcontrol, GalleryControl galleryControl, Point p, object galleryObject) {
			RibbonHitInfo info = galleryControl != null? galleryControl.CalcHitInfo(p): rcontrol.CalcHitInfo(p);
			RibbonHitInfo dropDownInfo = null;
			if(SelectedDropDownGallery != null && SelectedDropDownGallery.Visible)
				dropDownInfo = SelectedDropDownGallery.Gallery.ViewInfo.CalcHitInfo(SelectedDropDownGallery.Gallery.BarControl.PointToClient(rcontrol.PointToScreen(p)));
			if(ShouldProcessHitInfo(dropDownInfo)) {
				hitInfo.Gallery = dropDownInfo.Gallery;
				hitInfo.GalleryItemGroup = dropDownInfo.GalleryItemGroup;
				hitInfo.GalleryItem = dropDownInfo.GalleryItem;
			}
			else {
				hitInfo.GalleryItemGroup = info.GalleryItemGroup;
				hitInfo.Gallery = info.Gallery;
				hitInfo.GalleryItem = info.GalleryItem;
				hitInfo.Link = info.Item is RibbonGalleryBarItemLink? info.Item: null;
				if(Manager != null) {
					Manager.SelectionInfo.DropSelectedLink = hitInfo.Link;
				}
				HideSelectedDropDownGallery();
			}
			if(DragGalleryItem != null)
				DropSelectedObject = hitInfo.GalleryItem;
			else
				DropSelectedObject = hitInfo.GalleryItemGroup;
			if(hitInfo.GalleryItemGroup == null && ((galleryObject is GalleryItemGroup && hitInfo.Gallery == null) || (galleryObject is GalleryItem && hitInfo.GalleryItem == null)))
				hitInfo.Cursor = BarManager.NoDropCursor;
			else
				hitInfo.AfterCenter = CalcGalleryDropLocation(hitInfo, ShouldProcessHitInfo(dropDownInfo) ? dropDownInfo : info);
			return hitInfo;
		}
		public virtual object DropSelectedObject {
			get { return RibbonSelectionInfo.DropSelectedObject; }
			set { RibbonSelectionInfo.DropSelectedObject = value; }
		}
		public virtual LinkDropTargetEnum DropSelectStyle {
			get { return RibbonSelectionInfo.DropSelectStyle; }
			set { RibbonSelectionInfo.DropSelectStyle = value; }
		}
		protected virtual bool CalcGalleryDropLocation(RibbonDragHitInfo hitInfo, RibbonHitInfo info) {
			if(hitInfo.GalleryItem != null && DragGalleryItem != null) {
				return CalcInsertType(info.GalleryItemInfo.Bounds, info.HitPoint);
			}
			else if(hitInfo.GalleryItemGroup != null && DragGalleryItemGroup != null) {
				return CalcVertInsertType(info.GalleryItemGroupInfo.Bounds, info.HitPoint);
			}
			return false;
		}
		protected virtual bool CalcVertInsertType(Rectangle bounds, Point pt) { return bounds.Y + bounds.Height / 2 < pt.Y; }
		protected virtual RibbonDragHitInfo CalcDragLinkInfo(RibbonDragHitInfo hitInfo, Control control, Point p, BarItemLink itemLink) {
			RibbonControl rcontrol = control as RibbonControl;
			RibbonStatusBar scontrol = control as RibbonStatusBar;
			RibbonHitInfo info = null;
			if(rcontrol != null) info = rcontrol.CalcHitInfo(p);
			else if(scontrol != null) info = scontrol.CalcHitInfo(p);
			RibbonHitInfo dropDownInfo = null;
			Point galleryPoint = Point.Empty;
			BarLinkContainerItemLink contLink = RibbonSelectionInfo.DropSelectedLink as BarLinkContainerItemLink;
			bool inPopup = false;
			if(contLink != null && contLink.Opened) {
				Point subp = contLink.SubControl.Form.PointToClient(p);
				inPopup = contLink.SubControl.Form.ClientRectangle.Contains(subp);
			}
			if(SelectedDropDownGallery != null && SelectedDropDownGallery.Visible) {
				galleryPoint = SelectedDropDownGallery.Gallery.BarControl.PointToClient(control.PointToScreen(p));
			}
			if(galleryPoint != Point.Empty && SelectedDropDownGallery.Gallery.BarControl.Bounds.Contains(galleryPoint)) {
				dropDownInfo = SelectedDropDownGallery.Gallery.ViewInfo.CalcHitInfo(galleryPoint);
				hitInfo.Link = dropDownInfo.Item;
				DropSelectedObject = hitInfo.Link;
			}
			else if(!inPopup){
				if(rcontrol != null) {
					hitInfo.Link = rcontrol.ViewInfo.GetLinkByPoint(p, (Control.ModifierKeys & Keys.Shift) != 0);
					if(rcontrol.ViewInfo.Toolbar.DesignerRect.Contains(p))
						hitInfo.Toolbar = rcontrol.Toolbar;
					else if(rcontrol.ViewInfo.Header.DesignerRect.Contains(p))
						hitInfo.PageHeaderItemLinks = rcontrol.PageHeaderItemLinks;
				}
				else if(scontrol != null) {
					hitInfo.Link = scontrol.ViewInfo.GetLinkByPoint(p, (Control.ModifierKeys & Keys.Shift) != 0);
					hitInfo.StatusBar = scontrol;
				}
				DropSelectedObject = hitInfo.Link;
				hitInfo.PageGroup = info.PageGroup;
				if(DragLink.Holder != SelectedDropDownGallery)
					HideSelectedDropDownGallery();
			}
			hitInfo.AfterCenter = CalcInsertType(p, hitInfo.Link);
			hitInfo.Cursor = BarManager.DragCursor;
			if(DragObject is BarItem || (Control.ModifierKeys & Keys.Control) == Keys.Control) 
				hitInfo.Cursor = BarManager.CopyCursor;
			if(hitInfo.Link == null && hitInfo.PageGroup == null && (SelectedDropDownGallery == null || !SelectedDropDownGallery.ClonedFromInRibbonGallery)) {
				if(!InDesignerRects(p, rcontrol, scontrol))hitInfo.Cursor = BarManager.NoDropCursor;
			}
			if(IsEditorOnButtonGroup(DragLink, hitInfo))
				hitInfo.Cursor = BarManager.NoDropCursor;
			Manager.SelectionInfo.DropSelectedLink = hitInfo.Link;
			return hitInfo;
		}
		protected virtual bool IsEditorOnButtonGroup(BarItemLink checkLink, DragHitInfo hitInfo) {
			if(!(checkLink is BarEditItemLink) || hitInfo == null || hitInfo.Link == null || ((Control.ModifierKeys & Keys.Shift) == 0)) return false;
			return hitInfo.Link is BarButtonGroupLink || hitInfo.Link.Holder is BarButtonGroup;
		}
		protected virtual bool IsGalleryOnPageGroup(BarItemLink checkLink, DragHitInfo hitInfo) {
			RibbonDragHitInfo ribbonHitInfo = hitInfo as RibbonDragHitInfo;
			if(!(checkLink is RibbonGalleryBarItemLink) || ribbonHitInfo == null || hitInfo.Link == null) return false;
			return ribbonHitInfo.PageGroup != null;
		}
		protected virtual bool InDesignerRects(Point pt, RibbonControl rcontrol, RibbonStatusBar scontrol) {
			if(rcontrol != null && rcontrol.InDesignerRect(pt)) return true;
			if(scontrol != null && scontrol.InDesignerRect(pt)) return true;
			return false;
		}
		protected virtual bool CalcInsertType(Rectangle bounds, Point pt) {
			if(bounds.X + bounds.Width / 2 > pt.X) return false;
			return true;
		}
		protected virtual RibbonDragHitInfo CalcDragPageGroupInfo(RibbonDragHitInfo hitInfo, RibbonControl rcontrol, Point p, RibbonPageGroup pageGroup) {
			RibbonHitInfo info = rcontrol.CalcHitInfo(p);
			hitInfo.Page = info.Page;
			hitInfo.PageGroup = info.PageGroup;
			hitInfo.Cursor = BarManager.DragCursor;
			if(hitInfo.PageGroup != null) {
				hitInfo.AfterCenter = CalcInsertType(info.PageGroupInfo.Bounds, p);
				DropSelectedObject = hitInfo.PageGroup;
			}
			else if(hitInfo.Page == null && info.HitTest != RibbonHitTest.Panel) hitInfo.Cursor = BarManager.NoDropCursor;
			if((Control.ModifierKeys & Keys.Control) == Keys.Control)
				hitInfo.Cursor = BarManager.CopyCursor;
			return hitInfo;	
		}
		protected virtual RibbonDragHitInfo CalcDragToolbarInfo(RibbonDragHitInfo hitInfo, RibbonControl rcontrol, Point p, RibbonQuickAccessToolbar toolbar) {
			if(p.Y > rcontrol.Bounds.Y + rcontrol.Bounds.Height / 2)
				hitInfo.AfterCenter = true;
			hitInfo.Toolbar = toolbar;
			return hitInfo;
		}
		protected virtual RibbonDragHitInfo CalcDragPageCategoryInfo(RibbonDragHitInfo hitInfo, RibbonControl rcontrol, Point p, RibbonPageCategory category) {
			RibbonHitInfo info = rcontrol.CalcHitInfo(p);
			hitInfo.PageCategory = info.PageCategory;
			if(hitInfo.PageCategory == null) {
				hitInfo.Cursor = BarManager.NoDropCursor;
			}
			else {
				DropSelectedObject = hitInfo.PageCategory;
				if(info.PageCategoryInfo != null)
					hitInfo.AfterCenter = CalcInsertType(info.PageCategoryInfo.Bounds, p);
			}
			return hitInfo;
		}
		protected virtual RibbonDragHitInfo CalcDragPageInfo(RibbonDragHitInfo hitInfo, RibbonControl rcontrol, Point p, RibbonPage page) {
			RibbonHitInfo info = rcontrol.CalcHitInfo(p);
			hitInfo.Page = info.Page;
			if(hitInfo.Page == null) {
				if(info.PageCategory != null) {
					hitInfo.PageCategory = info.PageCategory;
					DropSelectedObject = hitInfo.PageCategory;
				}
				else
					hitInfo.Cursor = BarManager.NoDropCursor;
			}
			else {
				DropSelectedObject = hitInfo.Page;
				hitInfo.AfterCenter = CalcInsertType(info.Page.PageInfo.Bounds, p);
			}
			return hitInfo;
		}
		protected virtual DragHitInfo GetEmptyHitInfo() {
			RibbonDragHitInfo hitInfo = new RibbonDragHitInfo();
			hitInfo.Cursor = BarManager.NoDropCursor;
			return hitInfo;
		}
		protected override BarLinkDragManager.DragHitInfo CalcDragHitInfo(Control control, Point p, object dragObject) {
			RibbonDragHitInfo hitInfo = null;
			RibbonControl rcontrol = control as RibbonControl;
			RibbonStatusBar scontrol = control as RibbonStatusBar;
			GalleryDropDownBarControl galleryBarControl = control as GalleryDropDownBarControl;
			GalleryControl galleryControl = control as GalleryControl;
			if(galleryBarControl != null && (DragGalleryItem != null || DragGalleryItemGroup != null)) rcontrol = galleryBarControl.Gallery.Ribbon;
			if(rcontrol == null && scontrol == null && galleryControl == null) {
				DragHitInfo baseInfo = base.CalcDragHitInfo(control, p, dragObject);
				if(baseInfo != null) return baseInfo;
				return GetEmptyHitInfo();
			}
			if(galleryControl != null) p = galleryControl.PointToClient(p);
			else if(scontrol != null) p = scontrol.PointToClient(p);
			else p = rcontrol.PointToClient(p);
			hitInfo = new RibbonDragHitInfo();
			hitInfo.Cursor = BarManager.DragCursor;
			if(dragObject is GalleryItem || dragObject is GalleryItemGroup) return CalcDragGalleryInfo(hitInfo, rcontrol, galleryControl, p, dragObject);
			else if(dragObject is BarItemLink) return CalcDragLinkInfo(hitInfo, control, p, dragObject as BarItemLink);
			else if(dragObject is RibbonPageCategory) return CalcDragPageCategoryInfo(hitInfo, rcontrol, p, dragObject as RibbonPageCategory);
			else if(dragObject is RibbonPage) return CalcDragPageInfo(hitInfo, rcontrol, p, dragObject as RibbonPage);
			else if(dragObject is RibbonPageGroup) return CalcDragPageGroupInfo(hitInfo, rcontrol, p, dragObject as RibbonPageGroup);
			else if(dragObject is RibbonQuickAccessToolbar) return CalcDragToolbarInfo(hitInfo, rcontrol, p, dragObject as RibbonQuickAccessToolbar);
			return GetEmptyHitInfo();
		}
		protected virtual bool CanDragOnObject(RibbonDragHitInfo rhitInfo) {
			return (DragPageCategory != null && rhitInfo.PageCategory != null) || 
				(DragPage != null && rhitInfo.Page != null) || 
				(DragPageGroup != null && rhitInfo.PageGroup != null) ||
				(DragGalleryItem != null && rhitInfo.GalleryItem != null) || 
				(DragGalleryItemGroup != null && rhitInfo.GalleryItemGroup != null) || 
				(DragToolbar != null);
		}
		public override void DoDragging(Control control, MouseEventArgs e) {
			DragHitInfo hitInfo = CalcDragHitInfo(control, Control.MousePosition, DragObject);
			HitInfo = hitInfo;			
			Cursor.Current = (Cursor)DragCursors[hitInfo.Cursor];
			if(hitInfo.Link != null && DragObject is BarItemLink) {
				Manager.SelectionInfo.DropSelectStyle = hitInfo.AfterCenter ? LinkDropTargetEnum.After : LinkDropTargetEnum.Before;
				DropSelectedObject = hitInfo.Link;
			}
			RibbonDragHitInfo rhitInfo = hitInfo as RibbonDragHitInfo;
			if(hitInfo != null && CanDragOnObject(rhitInfo)) {
				DropSelectStyle = hitInfo.AfterCenter ? LinkDropTargetEnum.After : LinkDropTargetEnum.Before;
			}
			RibbonControl rcontrol = control as RibbonControl;
			if(SelectedDropDownGallery != null && SelectedDropDownGallery.Visible) {
				RibbonHitInfo dropDownInfo = SelectedDropDownGallery.Gallery.ViewInfo.CalcHitInfo(SelectedDropDownGallery.Gallery.BarControl.PointToClient(e.Location));
				CheckScrollDropDownGallery(dropDownInfo, rcontrol);
			}
			if(rcontrol == null) return ;
			RibbonHitInfo info = rcontrol.CalcHitInfo(rcontrol.PointToClient(e.Location));
			CheckOpenApplicationMenu(info, rcontrol);
			CheckDropDownGallery(info, rcontrol);
			CheckScrollGallery(info, rcontrol);
			CheckChangePage(info);
		}
		protected virtual void CheckChangePage(RibbonHitInfo hitInfo) {
			if(hitInfo.Page == null || (DragPageGroup == null && DragLink == null)) return;
			Ribbon.SelectedPage = hitInfo.Page;
		}
		protected virtual void CheckOpenApplicationMenu(RibbonHitInfo hitInfo, RibbonControl rcontrol) {
			if(rcontrol.ApplicationButtonDropDownControl == null) return;
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) {
				if(rcontrol.ApplicationButtonPopupControl != null && !rcontrol.ApplicationButtonPopupControl.Visible) { 
					(rcontrol.Handler as RibbonHandler).OnApplicationButtonClickCore();
				}
			}
		}
		protected virtual bool IsRibbonToolbarCustomizeItemLink(BarItemLink link) {
			return link == Ribbon.Toolbar.CustomizeItemLink || link == Ribbon.Toolbar.DropDownItemLink;
		}
		protected override BarLinksHolder GetLinkHolder(BarItemLink link) {
			if(IsRibbonToolbarCustomizeItemLink(link)) return Ribbon.Toolbar.ItemLinks;
			return base.GetLinkHolder(link);
		}
		protected override int GetLinkIndex(BarLinksHolder holder, BarItemLink link) {
			if(link == Ribbon.Toolbar.CustomizeItemLink) return Ribbon.Toolbar.ItemLinks.Count;
			else if(link == Ribbon.Toolbar.DropDownItemLink) return Ribbon.ViewInfo.Toolbar.VisibleButtonCount;
			return base.GetLinkIndex(holder, link);
		}
		protected override BarItemLink InsertItem(BarLinksHolder holder, BarItemLink beforeLink, BarItem item) {
			if(beforeLink == Ribbon.Toolbar.CustomizeItemLink) return holder.ItemLinks.Add(item);
			else if(beforeLink == Ribbon.Toolbar.DropDownItemLink) return holder.ItemLinks.Insert(Ribbon.ViewInfo.Toolbar.VisibleButtonCount - 1, item);
			else if(holder is BarButtonGroup) return holder.InsertItem(beforeLink.ClonedFromLink, item);
			return base.InsertItem(holder, beforeLink, item);
		}
		protected override void RemoveLink(BarLinksHolder holder, BarItemLink link) {
			if(holder is BarButtonGroup) holder.RemoveLink(link.ClonedFromLink);
			else base.RemoveLink(holder, link);
		}
		protected virtual bool ShouldInsertLink(BarItemLink link, BarButtonGroupLink buttonLink) {
			if(buttonLink == null || link == null || link == buttonLink) return false;
			if((Control.ModifierKeys & Keys.Shift) == 0) return false;
			if(link is RibbonGalleryBarItemLink || link is BarEditItemLink) return false;
			return true;
		}
		protected override void CopyLink(object dragObject, BarItemLink dest, bool move, bool afterDest) {
			BarButtonGroupLink buttonLink = dest as BarButtonGroupLink;
			if(ShouldInsertLink(DragLink, buttonLink)) {
				BarItem item = DragLink.Item;
				if(move) DragLink.Holder.ItemLinks.Remove(DragLink);
				buttonLink.Item.ItemLinks.Add(item);
				Ribbon.FireRibbonChanged();
				return;
			}
			else if(dest.Holder is RibbonStatusBarItemLinkCollection) {
				if(dest.Alignment == BarItemLinkAlignment.Right) DragLink.Item.Alignment = BarItemLinkAlignment.Right;
				else DragLink.Item.Alignment = BarItemLinkAlignment.Default;
			}
			base.CopyLink(dragObject, dest, move, afterDest);
		}
		protected virtual void RemovePageCategory(RibbonPageCategory pageCategory) {
			pageCategory.Collection.Remove(pageCategory);
			Ribbon.FireRibbonChanged();
		}
		protected virtual void RemovePage(RibbonPage page) {
			RibbonPageCategory cat = page.Category;
			if( cat == null) return;
			page.Category.Pages.Remove(page);
			Ribbon.FireRibbonChanged(cat);
		}
		protected virtual void AddPageCategory(RibbonPageCategory pageCategory) {
			Ribbon.PageCategories.Add(pageCategory);
		}
		protected virtual void AddPage(RibbonPage page, RibbonPageCategory pageCategory) {
			if(pageCategory == null) return;
			pageCategory.Pages.Add(page);
			Ribbon.SelectedPage = page;
		}
		protected virtual void CopyPageCategory(RibbonPageCategory category, RibbonPageCategory insertAtCategory) {
			if(insertAtCategory == null) {
				Ribbon.PageCategories.Add(category);
			}
			else {
				insertAtCategory.Collection.Insert(insertAtCategory.Collection.IndexOf(insertAtCategory), category);
			}
			Ribbon.FireRibbonChanged();
		}
		protected virtual void CopyPage(RibbonPage page, RibbonPage insertAtPage) {
			if(insertAtPage == null) {
				if(Ribbon.PageCategories.Count > 0) Ribbon.PageCategories[Ribbon.PageCategories.Count - 1].Pages.Add(page);
				Ribbon.Pages.Add(page);
				if(page.Category != null) Ribbon.FireRibbonChanged(page.Category);
			}
			else {
				insertAtPage.Category.Pages.Insert(insertAtPage.Category.Pages.IndexOf(insertAtPage), page);
				Ribbon.FireRibbonChanged(insertAtPage.Category);
			}
			Ribbon.SelectedPage = page;
		}
		protected virtual void RemovePageGroup(RibbonPageGroup pageGroup) {
			RibbonPage page = pageGroup.Page;
			page.Groups.Remove(pageGroup);
			Ribbon.FireRibbonChanged();
		}
		protected virtual void AddPageGroup(RibbonPageGroup pageGroup, RibbonPage page) {
			page.Groups.Add(pageGroup);
		}
		protected virtual void CopyPageGroup(RibbonPageGroup pageGroup, RibbonPage page, RibbonPageGroup insertAtGroup) {
			if(insertAtGroup == null)
				page.Groups.Add(pageGroup);
			else
				page.Groups.Insert(page.Groups.IndexOf(insertAtGroup), pageGroup);
			Ribbon.FireRibbonChanged(page);
		}
		protected virtual RibbonPageGroup GetFinalDropPageGroup(RibbonDragHitInfo hitInfo) {
			if(!hitInfo.AfterCenter || hitInfo.PageGroup == null) return hitInfo.PageGroup;
			int pageGroupIndex = hitInfo.PageGroup.Page.Groups.IndexOf(hitInfo.PageGroup) + 1;
			if(pageGroupIndex >= hitInfo.PageGroup.Page.Groups.Count) return null;
			return hitInfo.PageGroup.Page.Groups[pageGroupIndex];
		}
		protected virtual void StopDraggingPageGroup(Control control, DragDropEffects effects, bool cancelDrag) {
			if(!IsDragging) return;
			IsDragging = false;
			Cursor.Current = Cursors.Default;
			Manager.SelectionInfo.DropSelectStyle = LinkDropTargetEnum.None;
			if(cancelDrag || DragPageGroup == null) return;
			DragHitInfo hitInfo = CalcDragHitInfo(control, Control.MousePosition, DragObject);
			if(hitInfo.Cursor == BarManager.NoDropCursor) {
				RemovePageGroup(DragPageGroup);
				Ribbon.Container.Remove(DragPageGroup);
				DragPageGroup.Dispose();
				return;
			}
			RibbonDragHitInfo rhitInfo = hitInfo as RibbonDragHitInfo;
			RibbonHitInfo info = Ribbon.CalcHitInfo(Ribbon.PointToClient(Control.MousePosition));
			RibbonPageGroup res = DragPageGroup;
			if(hitInfo.Cursor == BarManager.CopyCursor) {
				res = Ribbon.ViewInfo.DesignTimeManager.DesignerHost.CreateComponent(typeof(RibbonPageGroup)) as RibbonPageGroup;
				res.Assign(DragPageGroup);
			}
			if(info.HitTest == RibbonHitTest.Panel && Ribbon.SelectedPage != null) {
				if(rhitInfo.Cursor != BarManager.CopyCursor)
					RemovePageGroup(DragPageGroup);
				AddPageGroup(res, Ribbon.SelectedPage);
				return;
			}
			if(rhitInfo != null && (rhitInfo.PageGroup != null || rhitInfo.Page != null)) {
				RibbonPageGroup finalGroup = GetFinalDropPageGroup(rhitInfo);
				if(finalGroup == DragPageGroup) return;
				if(rhitInfo.Cursor != BarManager.CopyCursor)
					RemovePageGroup(DragPageGroup);
				if(rhitInfo.PageGroup != null && DragPageGroup != finalGroup) {
					CopyPageGroup(res, rhitInfo.PageGroup.Page, finalGroup);
				}
				else if(rhitInfo.Page != null){
					AddPageGroup(res, rhitInfo.Page);
				}
			}
		}
		protected virtual RibbonPageCategory GetFinalDropPageCategory(RibbonDragHitInfo hitInfo) {
			if(!hitInfo.AfterCenter) return hitInfo.PageCategory;
			int categoryIndex = Ribbon.PageCategories.IndexOf(hitInfo.PageCategory) + 1;
			if(categoryIndex >= Ribbon.PageCategories.Count) return null;
			return Ribbon.PageCategories[categoryIndex];
		}
		protected virtual RibbonPage GetFinalDropPage(RibbonDragHitInfo hitInfo) {
			if(!hitInfo.AfterCenter) return hitInfo.Page;
			int pageIndex = Ribbon.Pages.IndexOf(hitInfo.Page) + 1;
			if(pageIndex >= Ribbon.Pages.Count) return null;
			return Ribbon.Pages[pageIndex];
		}
		protected virtual void StopDraggingPageCategory(Control control, DragDropEffects effects, bool cancelDrag) {
			if(!IsDragging) return;
			IsDragging = false;
			Cursor.Current = Cursors.Default;
			Manager.SelectionInfo.DropSelectStyle = LinkDropTargetEnum.None;
			if(cancelDrag || DragPageCategory == null) return;
			DragHitInfo hitInfo = CalcDragHitInfo(control, Control.MousePosition, DragObject);
			if(hitInfo.Cursor == BarManager.NoDropCursor) {
				RemovePageCategory(DragPageCategory);
				Ribbon.Container.Remove(DragPageCategory);
				DragPageCategory.Dispose();
				return;
			}
			RibbonDragHitInfo rhitInfo = hitInfo as RibbonDragHitInfo;
			if(rhitInfo != null && rhitInfo.PageCategory != null) {
				RibbonPageCategory finalCategory = GetFinalDropPageCategory(rhitInfo);
				if(finalCategory == DragPageCategory) return;
				RemovePageCategory(DragPageCategory);
				if(rhitInfo.PageCategory != null) CopyPageCategory(DragPageCategory, finalCategory);
				else AddPageCategory(DragPageCategory);
			}
		}
		protected virtual void StopDraggingPage(Control control, DragDropEffects effects, bool cancelDrag) {
			if(!IsDragging) return;
			IsDragging = false;
			Cursor.Current = Cursors.Default;
			Manager.SelectionInfo.DropSelectStyle = LinkDropTargetEnum.None;
			if(cancelDrag || DragPage == null) return;
			DragHitInfo hitInfo = CalcDragHitInfo(control, Control.MousePosition, DragObject);
			if(hitInfo.Cursor == BarManager.NoDropCursor) {
				RemovePage(DragPage);
				Ribbon.Container.Remove(DragPage);
				DragPage.Dispose();
				return;
			}
			RibbonDragHitInfo rhitInfo = hitInfo as RibbonDragHitInfo;
			if(rhitInfo != null && (rhitInfo.Page != null || rhitInfo.PageCategory != null)) {
				RibbonPage finalPage = GetFinalDropPage(rhitInfo);
				if(finalPage == DragPage) return;
				RemovePage(DragPage);
				if(rhitInfo.Page != null) CopyPage(DragPage, finalPage);
				else AddPage(DragPage, rhitInfo.PageCategory);
			}
		}
		protected virtual void StopDraggingToolbar(Control control, DragDropEffects effects, bool cancelDrag) {
			if(!IsDragging) return;
			IsDragging = false;
			Manager.SelectionInfo.DropSelectStyle = LinkDropTargetEnum.None;
			if(cancelDrag || Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Hidden) return;
			DragHitInfo hitInfo = CalcDragHitInfo(control, Control.MousePosition, DragObject);
			if(hitInfo.AfterCenter) Ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Below;
			else Ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Above;
			Ribbon.FireRibbonChanged();
		}
		protected virtual void AddLink(BarItemLinkCollection coll, BarItem item) {
			coll.Add(item);
			Ribbon.FireRibbonChanged();
		}
		protected virtual void AddLink(RibbonPageGroup group, BarItem item) {
			AddLink(group.ItemLinks, item);
		}
		protected virtual void AddLink(RibbonStatusBar statusBar, BarItem item) {
			AddLink(statusBar.ItemLinks, item);
		}
		public virtual void StopDraggingLink(Control control, DragDropEffects effects, bool cancelDrag) {
			DragHitInfo hitInfo = CalcDragHitInfo(control, Control.MousePosition, DragObject);
			if(IsEditorOnButtonGroup(DragLink, hitInfo)) cancelDrag = true;
			if(hitInfo.Link != null) {
				base.StopDragging(control, effects, cancelDrag);
				return;
			}
			if(!IsDragging) return;
			IsDragging = false;
			Cursor.Current = Cursors.Default;
			LinkDropTargetEnum mark = Manager.SelectionInfo.DropSelectStyle;
			Manager.SelectionInfo.CustomizeSelectedLink = null;
			Manager.SelectionInfo.DropSelectStyle = LinkDropTargetEnum.None;
			if(cancelDrag || DragLink == null) return;
			if (hitInfo.Cursor == BarManager.NoDropCursor && (Control.ModifierKeys & Keys.Control) == 0) {
				RemoveLink(DragLink, DragLink.GetBeginGroup());
				return;
			}
			RibbonDragHitInfo rhitInfo = hitInfo as RibbonDragHitInfo;
			if(rhitInfo != null && DragLink != null) {
				BarItem item = DragLink.Item;
				if((Control.ModifierKeys & Keys.Control) == 0) RemoveLink(DragLink, DragLink.GetBeginGroup());
				if(rhitInfo.PageGroup != null) {
					AddLink(rhitInfo.PageGroup, item);
				}
				else if(rhitInfo.PageHeaderItemLinks != null) {
					AddLink(rhitInfo.PageHeaderItemLinks, item);
				}
				else if(rhitInfo.Toolbar != null) {
					AddLink(rhitInfo.Toolbar.ItemLinks, item);
				}
				else if(rhitInfo.StatusBar != null) {
					Point pt = rhitInfo.StatusBar.PointToClient(Control.MousePosition);
					if(rhitInfo.StatusBar.ViewInfo.DesignerRightRect.Contains(pt)) item.Alignment = BarItemLinkAlignment.Right;
					else item.Alignment = BarItemLinkAlignment.Default;
					AddLink(rhitInfo.StatusBar, item);
				}
			}
		}
		protected virtual void RemoveGalleryItem(GalleryItemGroup group, GalleryItem item) {
			group.Items.Remove(item);
			Manager.FireManagerChanged();
			Ribbon.ViewInfo.CalcViewInfo(Ribbon.ClientRectangle);
		}
		protected virtual void CopyGalleryItem(GalleryItem item, GalleryItemGroup group, GalleryItem insertItem) {
			if(insertItem == null) group.Items.Add(item);
			else group.Items.Insert(group.Items.IndexOf(insertItem), item);
			Manager.FireManagerChanged();
			Ribbon.ViewInfo.CalcViewInfo(Ribbon.ClientRectangle);
		}
		protected virtual GalleryItem GetFinalDropGalleryItem(RibbonDragHitInfo rhitInfo) {
			int index;
			if(rhitInfo.GalleryItem == null) return null;
			if(!rhitInfo.AfterCenter) return rhitInfo.GalleryItem;
			index = rhitInfo.GalleryItem.GalleryGroup.Items.IndexOf(rhitInfo.GalleryItem) + 1;
			if(index >= rhitInfo.GalleryItem.GalleryGroup.Items.Count) return null;
			return rhitInfo.GalleryItem.GalleryGroup.Items[index];
		}
		protected virtual void MoveGalleryItem(GalleryItem item, GalleryItemGroup group, GalleryItem insertItem) {
			GalleryItemGroup originalGroup = GetOriginalGroup(item.GalleryGroup);
			GalleryItemGroup originalInsertGroup = GetOriginalGroup(group);
			GalleryItem originalItem = GetOriginalItem(item);
			GalleryItem originalInsertItem = GetOriginalItem(insertItem);
			if(!ShouldCopy) RemoveGalleryItem(originalGroup, originalItem);
			CopyGalleryItem(originalItem, originalInsertGroup, originalInsertItem);
		}
		protected internal virtual BaseGallery GetOriginalGallery(BaseGallery gallery) {
			InDropDownGallery ddGallery = gallery as InDropDownGallery;
			if(ddGallery != null && ddGallery.GalleryDropDown.OwnerGallery != null) return ddGallery.GalleryDropDown.OwnerGallery;
			return gallery;
		}
		protected internal virtual GalleryItemGroup GetOriginalGroup(GalleryItemGroup group) {
			if(group == null) return null;
			InDropDownGallery gallery = group.Gallery as InDropDownGallery;
			if(gallery == null || gallery.GalleryDropDown.OwnerGallery == null) return group;
			int index = group.Gallery.Groups.IndexOf(group);
			if(index >= gallery.GalleryDropDown.OwnerGallery.Groups.Count) return group;
			return gallery.GalleryDropDown.OwnerGallery.Groups[index];
		}
		protected internal virtual GalleryItem GetOriginalItem(GalleryItem item) {
			if(item == null) return null;
			GalleryItemGroup group = GetOriginalGroup(item.GalleryGroup);
			if(group == item.GalleryGroup) return item;
			int index = item.GalleryGroup.Items.IndexOf(item);
			if(index >= item.GalleryGroup.Items.Count) return item;
			return group.Items[index];
		}
		protected virtual bool IsSameGroup(RibbonDragHitInfo rhitInfo) {
			return rhitInfo.GalleryItemGroup != null && GetOriginalGroup(GetFinalDropGalleryGroup(rhitInfo)) == GetOriginalGroup(DragGalleryItemGroup);
		} 
		protected virtual bool IsSameItem(RibbonDragHitInfo rhitInfo) {
			return rhitInfo.GalleryItem != null && GetOriginalItem(GetFinalDropGalleryItem(rhitInfo)) == GetOriginalItem(DragGalleryItem);
		}
		protected virtual bool CanAcceptGalleryItem(RibbonDragHitInfo rhitInfo) {
			if(rhitInfo == null) return false;
			if(rhitInfo.GalleryItem != null && IsSameItem(rhitInfo)) return false;
			return rhitInfo.GalleryItem != null || rhitInfo.GalleryItemGroup != null;
		}
		protected virtual void StopDraggingGalleryItem(Control control, DragDropEffects effects, bool cancelDrag) {
			DragHitInfo hitInfo = CalcDragHitInfo(control, Control.MousePosition, DragObject);
			if(!IsDragging) return;
			IsDragging = false;
			Cursor.Current = Cursors.Default;
			if(hitInfo.Cursor == BarManager.NoDropCursor) {
				RemoveGalleryItem(DragGalleryItem.GalleryGroup, DragGalleryItem);
				HideSelectedDropDownGallery();
				return;
			}
			RibbonDragHitInfo rhitInfo = hitInfo as RibbonDragHitInfo;
			if(cancelDrag || rhitInfo == null || !CanAcceptGalleryItem(rhitInfo)) {
				return;
			}
			MoveGalleryItem(DragGalleryItem, rhitInfo.GalleryItemGroup, GetFinalDropGalleryItem(rhitInfo));
		}
		protected internal virtual void HideSelectedDropDownGallery() {
			if(SelectedDropDownGallery != null) SelectedDropDownGallery.HidePopup();
			SelectedDropDownGallery = null;
			DestroyScrollTimer();
		}
		protected virtual void RemoveGalleryItemGroup(BaseGallery gallery, GalleryItemGroup group) {
			gallery.Groups.Remove(group);
			Manager.FireManagerChanged();
			Ribbon.ViewInfo.CalcViewInfo(Ribbon.ClientRectangle);
		}
		protected virtual void CopyGalleryItemGroup(GalleryItemGroup group, BaseGallery gallery, GalleryItemGroup insertGroup) {
			if(insertGroup == null) gallery.Groups.Add(group);
			else gallery.Groups.Insert(gallery.Groups.IndexOf(insertGroup), group);
			Manager.FireManagerChanged();
			Ribbon.ViewInfo.CalcViewInfo(Ribbon.ClientRectangle);
		}
		protected virtual void MoveGalleryItemGroup(GalleryItemGroup group, BaseGallery gallery, GalleryItemGroup insertGroup) {
			GalleryItemGroup originalGroup = GetOriginalGroup(group);
			GalleryItemGroup originalInsertGroup = GetOriginalGroup(insertGroup);
			BaseGallery originalInsertGallery = GetOriginalGallery(gallery);
			BaseGallery originalGroupGallery = GetOriginalGallery(group.Gallery);
			if(!ShouldCopy) RemoveGalleryItemGroup(originalGroupGallery, originalGroup);
			CopyGalleryItemGroup(originalGroup, originalInsertGallery, originalInsertGroup);
		}
		protected virtual GalleryItemGroup GetFinalDropGalleryGroup(RibbonDragHitInfo rhitInfo) {
			int index;
			if(rhitInfo.GalleryItemGroup == null) return null;
			if(!rhitInfo.AfterCenter) return rhitInfo.GalleryItemGroup;
			index = rhitInfo.GalleryItemGroup.Gallery.Groups.IndexOf(rhitInfo.GalleryItemGroup) + 1;
			if(index >= rhitInfo.GalleryItemGroup.Gallery.Groups.Count) return null;
			return rhitInfo.GalleryItemGroup.Gallery.Groups[index];
		}
		protected virtual bool CanAcceptGalleryItemGroup(RibbonDragHitInfo rhitInfo) {
			if(rhitInfo == null) return false;
			if(rhitInfo.GalleryItemGroup != null && IsSameGroup(rhitInfo)) return false;
			return rhitInfo.GalleryItemGroup != null || rhitInfo.Gallery != null;
		}
		protected virtual bool ShouldCopy { get { return (Control.ModifierKeys & Keys.Control) != 0; } }
		protected virtual bool ShouldEnterButtonGroup { get { return (Control.ModifierKeys & Keys.Shift) != 0; } }
		protected virtual void StopDraggingGalleryItemGroup(Control control, DragDropEffects effects, bool cancelDrag) {
			DragHitInfo hitInfo = CalcDragHitInfo(control, Control.MousePosition, DragObject);
			if(!IsDragging) return;
			IsDragging = false;
			Cursor.Current = Cursors.Default;
			if(hitInfo.Cursor == BarManager.NoDropCursor) {
				RemoveGalleryItemGroup(DragGalleryItemGroup.Gallery, DragGalleryItemGroup);
				return;
			}
			RibbonDragHitInfo rhitInfo = hitInfo as RibbonDragHitInfo;
			if(cancelDrag || rhitInfo == null || !CanAcceptGalleryItemGroup(rhitInfo)) {
				return;
			}
			MoveGalleryItemGroup(DragGalleryItemGroup, rhitInfo.Gallery, GetFinalDropGalleryGroup(rhitInfo));
		}
		public override void StopDragging(Control control, DragDropEffects effects, bool cancelDrag) {
			if(control == null && Ribbon.ClientRectangle.Contains(Ribbon.PointToClient(Control.MousePosition)))
				control = Ribbon;
			if(DragObject is RibbonPageCategory)
				StopDraggingPageCategory(control, effects, cancelDrag);
			else if(DragObject is RibbonPage)
				StopDraggingPage(control, effects, cancelDrag);
			else if(DragObject is GalleryItem)
				StopDraggingGalleryItem(control, effects, cancelDrag);
			else if(DragObject is GalleryItemGroup)
				StopDraggingGalleryItemGroup(control, effects, cancelDrag);
			else if(DragObject is RibbonPageGroup)
				StopDraggingPageGroup(control, effects, cancelDrag);
			else if(DragObject is RibbonQuickAccessToolbar)
				StopDraggingToolbar(control, effects, cancelDrag);
			else
				StopDraggingLink(control, effects, cancelDrag);
			Ribbon.ViewInfo.CalcViewInfo(Ribbon.ClientRectangle);
			DropSelectedObject = null;
		}
	}
	public class RibbonBarDesignTimeManager : DesignTimeManager {
		public RibbonBarDesignTimeManager(BarManager manager) : base(manager) { }
		protected override DesignTimeCreateItemMenu CreateItemsMenu(DevExpress.XtraBars.InternalItems.BarDesignTimeItemLink link) {
			return new RibbonDesignTimeManager.RibbonDesignTimeCreateItemMenu(DesignManager, Manager, link); 
		}	
	}
	public class RibbonCustomizationManager : BarCustomizationManager {
		public RibbonCustomizationManager(BarManager manager) : base(manager) { }
		internal override DevExpress.XtraBars.Customization.Helpers.DesignTimeManager CreateDesignTimeManagerCore() {
			return new RibbonBarDesignTimeManager(Manager);
		}
		public override bool IsCanHotCustomizing {
			get { return false; }
		}
	}
	public class RibbonBarManagerHelpers : BarManagerHelpers {
		public RibbonBarManagerHelpers(RibbonBarManager ribbonManager) : base(ribbonManager) { }
		public new RibbonBarManager Manager { get { return base.Manager as RibbonBarManager; } }
		protected override BarLinkDragManager CreateLinkDragManager() { return new RibbonLinkDragManager(Manager); }
		protected override BarCustomizationManager CreateCustomizationManager() {
			return new RibbonCustomizationManager(Manager);
		}
	}
	[ToolboxItem(false)]
	public class RibbonBarManager : BarManager {
		RibbonControl ribbon;
		public RibbonBarManager(RibbonControl ribbon) {
			this.ribbon = ribbon;
			SetForm(ribbon);
			((DevExpress.XtraEditors.Repository.PersistentRepository) EditorHelper.InternalRepository).SetParentComponent(Ribbon);
		}
		protected internal bool GetIsInitialized() {
			return IsInitialized;
		}
		protected internal override BarItems MergedItems {
			get {
				if(Ribbon.MergedRibbon != null)
					return Ribbon.MergedRibbon.Items;
				return base.MergedItems;
			}
		}
		public override BarItemLink KeyboardHighlightedLink {
			get {
				NavigationObjectRibbonItem item = Ribbon.ViewInfo.KeyboardActiveObject as NavigationObjectRibbonItem;
				if(item != null) 
					return item.ItemLink;
				return base.KeyboardHighlightedLink;
			}
		}
		public override void SelectLink(BarItemLink link) {
			base.SelectLink(link);
			Ribbon.ActivateKeyboardNavigation(true);
			foreach(NavigationObject obj in Ribbon.NavigatableObjectList) {
				NavigationObjectRibbonItem item = obj as NavigationObjectRibbonItem;
				if(item != null && item.ItemLink == link) {
					item.Select(Ribbon.ViewInfo);
					break;
				}
			}
		}
		protected override object ShowDXDropDownMenu(DevExpress.Utils.Menu.DXPopupMenu menu, Control control, Point pos) {
			if(menu == null)
				return null;
			if(menu.MenuViewType == DevExpress.Utils.Menu.MenuViewType.RibbonMiniToolbar) {
				DXRibbonMiniToolbar toolbar = new DXRibbonMiniToolbar(Ribbon, menu);
				toolbar.Show(control, pos);
				return toolbar.Toolbar;
			}
			return base.ShowDXDropDownMenu(menu, control, pos);
		}
		protected override BarManagerHelpers CreateHelpers() { return new RibbonBarManagerHelpers(this) as BarManagerHelpers; }
		protected internal override void AddToContainer(BarItem item) {
			if(Ribbon.IsDesignMode && Ribbon.Container != null) Ribbon.Container.Add(item);
		}
		protected override DevExpress.XtraBars.ViewInfo.BarSelectionInfo CreateSelectionInfo() {
			return new DevExpress.XtraBars.ViewInfo.RibbonSelectionInfo(this);
		}
		public override BarMdiMenuMergeStyle MdiMenuMergeStyle {
			get { return BarMdiMenuMergeStyle.Never; }
			set { }
		}
		public override bool IsLoading {
			get { return base.IsLoading || (Ribbon != null && Ribbon.IsLoading); }
		}
		protected internal override Docking2010.Views.BaseDocument GetDocument() {
			if(Ribbon != null && Ribbon.Parent != null) {
				var documentContainer = Docking2010.DocumentContainer.FromControl(Ribbon.Parent);
				if(documentContainer != null)
					return documentContainer.Document;
				var documentForm = Ribbon.Parent.Parent as Docking2010.FloatDocumentForm;
				if(documentForm != null)
					return documentForm.Document;
			}
			return null;
		}
		protected override void OnHighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e) {
			if(e.Link == null) return;
			RibbonPageGroupItemLinkCollection pageGroupColl = e.Link.LinkedObject as RibbonPageGroupItemLinkCollection;
			if(pageGroupColl != null) {
				Ribbon.AccessibleNotifyClients(AccessibleEvents.Focus, RibbonControl.AccessibleGroupsBeginIndex + pageGroupColl.PageGroup.Page.Groups.IndexOf(pageGroupColl.PageGroup), pageGroupColl.IndexOf(e.Link));	
			}
			RibbonQuickToolbarItemLinkCollection toolbarColl = e.Link.LinkedObject as RibbonQuickToolbarItemLinkCollection;
			if(toolbarColl != null) {
				Ribbon.AccessibleNotifyClients(AccessibleEvents.Focus, RibbonControl.AccessibleObjectRibbonToolbarItem, toolbarColl.IndexOf(e.Link));	
			}
		}
		public override void ShowToolBarsPopup(BarItemLink link) {
			if(link == null) return;
			Ribbon.CustomizationPopupMenu.Activator = link;
			Ribbon.CustomizationPopupMenu.UpdateMenu(Ribbon.AllowChangeToolbarLocationMenuItem, link);
			RibbonCustomizationMenuEventArgs args = new RibbonCustomizationMenuEventArgs(link, Ribbon);
			Ribbon.RaiseShowCustomizationMenu(args);
			if(!args.ShowCustomizationMenu.HasValue || args.ShowCustomizationMenu.Value)
				Ribbon.CustomizationPopupMenu.Show(Control.MousePosition.X, Control.MousePosition.Y, Ribbon.AllowChangeToolbarLocationMenuItem, link);
			else
				Ribbon.CustomizationPopupMenu.HidePopup();
		}
		public void ClosePopupForms(bool closeMinimizedRibbon) {
			if(Ribbon.MinimizedRibbonPopupForm != null && closeMinimizedRibbon)
				Ribbon.MinimizedRibbonPopupForm.Close();
			if(Ribbon.PopupGroupForm != null)
				Ribbon.PopupGroupForm.Close();
			if(Ribbon.PopupToolbar != null)
				Ribbon.PopupToolbar.Close();
		}
		public void ClosePopupForms() {
			ClosePopupForms(true);
		}
		public override bool AllowGlyphSkinning {
			get {
				return Ribbon.AllowGlyphSkinning;
			}
			set {
				Ribbon.AllowGlyphSkinning = value;
			}
		}
		public override bool AllowItemAnimatedHighlighting {
			get { return true; }
			set { }
		}
		protected override BarItems CreateItems() { return new RibbonBarItems(this); }
		public RibbonControl Ribbon { get { return ribbon; } }
		protected internal virtual RibbonItemViewInfo CreateItemViewInfo(BaseRibbonViewInfo viewInfo, IRibbonItem item) {
			IRibbonItemViewInfoProvider provider = item as IRibbonItemViewInfoProvider;
			if(provider != null) return provider.CreateViewInfo(viewInfo, item);
			SkinRibbonGalleryBarItemLink skinGalleryLink = item as SkinRibbonGalleryBarItemLink;
			if(skinGalleryLink != null) {
				if(skinGalleryLink.Holder is RibbonPageGroupItemLinkCollection && !viewInfo.IsOfficeTablet)
					return new SkinRibbonGalleryRibbonItemViewInfo(viewInfo, item);
				return new RibbonDropDownItemViewInfo(viewInfo, item);
			}
			RibbonGalleryBarItemLink galleryLink = item as RibbonGalleryBarItemLink;
			if(galleryLink != null) {
				if(galleryLink.Holder is RibbonPageGroupItemLinkCollection && !viewInfo.IsOfficeTablet)
					return new InRibbonGalleryRibbonItemViewInfo(viewInfo, item);
				return new RibbonDropDownItemViewInfo(viewInfo, item);
			}
			BarItemLink link = item as BarItemLink;
			BarLargeButtonItemLink largeLink = item as BarLargeButtonItemLink;
			BarCheckItemLink checkButtonLink = item as BarCheckItemLink;
			RibbonExpandCollapseItemLink expandCollapseLink = item as RibbonExpandCollapseItemLink;
			AutoHiddenPagesMenuItemLink autoHiddenPagesMenuLink = item as AutoHiddenPagesMenuItemLink;
			BarButtonItemLink buttonLink = item as BarButtonItemLink;
			BarToggleSwitchItemLink toggleSwitchLink = item as BarToggleSwitchItemLink;
			if(expandCollapseLink != null)
				return new RibbonExpandCollapseItemViewInfo(viewInfo, item);
			if(autoHiddenPagesMenuLink != null)
				return new AutoHiddenPagesMenuItemViewInfo(viewInfo, item);
			if(largeLink != null || buttonLink != null) {
				if(buttonLink.Item.ActAsDropDown) return new RibbonDropDownItemViewInfo(viewInfo, item);
				if(buttonLink.Item.ButtonStyle == BarButtonStyle.DropDown) return new RibbonSplitButtonItemViewInfo(viewInfo, item);
				if(buttonLink.Item.ButtonStyle == BarButtonStyle.CheckDropDown) return new RibbonCheckDropDownButtonItemViewInfo(viewInfo, item);
				return new RibbonButtonItemViewInfo(viewInfo, item);
			}
			if(checkButtonLink != null) return new RibbonCheckItemViewInfo(viewInfo, item);
			BarButtonGroupLink bg = item as BarButtonGroupLink;
			if(bg != null) { return new RibbonButtonGroupItemViewInfo(viewInfo, item); }
			BarCustomContainerItemLink container = item as BarCustomContainerItemLink;
			if(container != null) {
				BarSubItemLink subLink = container as BarSubItemLink;
				if(subLink != null) {
					BarSubItem subItem = subLink.Item as BarSubItem;
					if(!subItem.InplaceVisible) return null;
				}
				return new RibbonDropDownItemViewInfo(viewInfo, item);
			}
			BarEditItemLink editLink = item as BarEditItemLink;
			if(editLink != null && editLink.Edit != null) {
				return new RibbonEditItemViewInfo(viewInfo, item);
			}
			BarStaticItemLink stat = item as BarStaticItemLink;
			if(stat != null) return new RibbonStaticItemViewInfo(viewInfo, item);
			if(toggleSwitchLink != null)
				return new RibbonToggleSwitchItemViewInfo(viewInfo, item);
			return null;
		}
		public override PopupShowMode PopupShowMode {
			get { return Ribbon.PopupShowMode; }
			set { Ribbon.PopupShowMode = value; }
		}
		protected internal override PopupShowMode GetPopupShowMode(IPopup popup) {
			if(popup != null) {
				PopupMenu menu = popup.PopupCreator as PopupMenu;
				if(menu != null && menu.MenuCreator is ImageGalleryFilterMenu)
					return PopupShowMode.Classic;
			}
			if(PopupShowMode == PopupShowMode.Default) {
				SubMenuBarControl link = popup as SubMenuBarControl;
				if(link != null) {
					foreach(var obj in Ribbon.Manager.SelectionInfo.OpenedPopups) {
						if(obj is AppMenuBarControl) return PopupShowMode.Classic;
					}
				}
				return Ribbon.IsOfficeTablet ? PopupShowMode.Inplace : PopupShowMode.Classic;
			}
			return PopupShowMode;
		}
		public override bool DockingEnabled { get { return false; } set { } }
		protected internal override Control FilterForm {
			get {
				return Ribbon == null ? base.Form : Ribbon.Parent;
			}
		}
		protected override bool IsRibbonManager { get { return true; } }
		protected internal override void Deactivate() {
			base.Deactivate();
			Ribbon.ClosePopupForms();
			((RibbonSelectionInfo)SelectionInfo).DestroyKeyboardTimer();
			Ribbon.DeactivateKeyboardNavigation();
			Ribbon.ResetMiniToolbars();
		}
		protected override void OnFireManagerChanged() {
			if(Ribbon.Site == null) return;
			IComponentChangeService srv = Ribbon.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				srv.OnComponentChanged(Ribbon, null, null, null);
			}
		}
		protected override void OnControllerChanged() {
			base.OnControllerChanged();
			Ribbon.OnControllerChanged();
		}
		protected override void OnControllerChanged(bool setNewController) {
			base.OnControllerChanged(setNewController);
			Ribbon.OnControllerChanged(setNewController);
		}
		protected internal override bool ProcessCommandKey(Keys key) {
			if(key == Keys.F1 && Control.ModifierKeys == Keys.Control) {
				Ribbon.Minimized = !Ribbon.Minimized;
				return true;
			}
			return base.ProcessCommandKey(key);
		}
		RibbonDesignTimeManager RibbonDesignTimeManager { get { return Ribbon.ViewInfo.DesignTimeManager as RibbonDesignTimeManager; } }
		public int DesignGalleryItemIndex { get { return RibbonDesignTimeManager.GalleryItemIndex; } set { RibbonDesignTimeManager.GalleryItemIndex = value; } }
		public int DesignGalleryGroupIndex { get { return RibbonDesignTimeManager.GalleryGroupIndex; } set { RibbonDesignTimeManager.GalleryGroupIndex = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool TransparentEditors {
			get { return Ribbon.TransparentEditors; }
			set { Ribbon.TransparentEditors = value; }
		}
		protected internal override bool GetScaleEditors() {
			return GetController().PropertiesRibbon.ScaleEditors;
		}
		internal override IBindableComponent GetParentBindableComponent() {
			if(Ribbon != null) {
				if(Ribbon.IsHandleCreated)
					return Ribbon;
				else {
					if(Ribbon.IsMerged) {
						if(Ribbon.Parent != null && Ribbon.Parent.IsHandleCreated)
							return Ribbon.Parent;
					}
				}
			}
			return null;
		}
	}
}
namespace DevExpress.XtraBars.ViewInfo {
	public class RibbonSelectionInfo : BarSelectionInfo {
		public new RibbonBarManager Manager { get { return base.Manager as RibbonBarManager; } }
		protected virtual RibbonControl Ribbon { get { return Manager == null ? null : Manager.Ribbon; } }
		public RibbonSelectionInfo(RibbonBarManager manager) : base(manager) { }
		public override bool CloseEditor(bool restoreFocus) {
			Ribbon.ViewInfo.SuppressInvalidate = true;
			try {
				return base.CloseEditor(restoreFocus);
			}
			finally {
				Ribbon.ViewInfo.SuppressInvalidate = false;
			}
		}
		public override bool HideEditor(bool restoreFocus) {
			Ribbon.ViewInfo.SuppressInvalidate = true;
			try {
				return base.HideEditor(restoreFocus);
			}
			finally {
				Ribbon.ViewInfo.SuppressInvalidate = false;
			}
		}
		protected RibbonControl ActualRibbon {
			get {
				if(Ribbon.MergeOwner != null)
					return Ribbon.MergeOwner;
				return Ribbon;
			}
		}
		public override void ShowEditor(BarEditItemLink link, BaseEdit edit) {
			ActualRibbon.ViewInfo.SuppressInvalidate = true;
			try {
				base.ShowEditor(link, edit);
			}
			finally {
				ActualRibbon.ViewInfo.SuppressInvalidate = false;
			}
		}
		protected internal override void OnCloseAll(BarMenuCloseType closeType) {
			base.OnCloseAll(closeType);
			if(Ribbon != null) {
				if(closeType.HasFlag(BarMenuCloseType.All)) {
					Ribbon.ClosePopupForms();
					Ribbon.DeactivateKeyboardNavigation();
				}
				else if(closeType.HasFlag(BarMenuCloseType.AllExceptMinimized)) {
					Ribbon.ClosePopupForms(false);
				}
				else if(closeType.HasFlag(BarMenuCloseType.AllExceptMiniToolbars)) {
					Ribbon.ClosePopupForms(true, false, true);
				}
				else if(closeType.HasFlag(BarMenuCloseType.AllExceptMiniToolbarsAndDXToolbars)) {
					Ribbon.ClosePopupForms(true, false, false);
				}
			}
		}
		protected override bool CanHighlight(BarItemLink link) {
			foreach(RibbonMiniToolbar tb in Ribbon.MiniToolbars) {
				if(tb.ToolbarControl.ViewInfo.ContainsLink(link))
					return true;
			}
			return base.CanHighlight(link);
		}
		protected override void OnOpenedPopupsCollectionChanged() {
			base.OnOpenedPopupsCollectionChanged();
			foreach(RibbonMiniToolbar tb in Ribbon.MiniToolbars) {
				if(!tb.Form.ToolbarHasOpenedPopups() && OpenedPopups.Contains(tb))
					tb.ResetForm();
				tb.UpdateVisibility(Control.MousePosition);
			}
		}
		protected override bool HasOpenedPopups {
			get { return OpenedPopups.Count > 0 || Ribbon.IsPopupFormOpened; }
		}
		object dropSelectedObject = null;
		bool showDropMark = true;
		internal bool ShowDropMark { get { return Manager.IsCustomizing && showDropMark; } set { showDropMark = value; } }
		void InvalidateDropSelectedObject(bool showMark) {
			ShowDropMark = showMark;
			RibbonPage page = DropSelectedObject as RibbonPage;
			RibbonPageCategory category = DropSelectedObject as RibbonPageCategory;
			RibbonPageGroup group = DropSelectedObject as RibbonPageGroup;
			GalleryItemGroup galleryGroup = DropSelectedObject as GalleryItemGroup;
			GalleryItem item = DropSelectedObject as GalleryItem;
			try {
				if(page != null && page.PageInfo != null) Ribbon.Invalidate(page.PageInfo.Bounds);
				else if(category != null && category.CategoryInfo != null) Ribbon.Invalidate(category.CategoryInfo.Bounds);
				else if(group != null && group.GroupInfo != null) Ribbon.Invalidate(group.GroupInfo.Bounds);
				else if(galleryGroup != null && galleryGroup.Gallery != null) galleryGroup.Gallery.Invalidate();
				else if(item != null && item.Gallery != null) item.Gallery.Invalidate(item);
				else if(DropSelectedLink != null) DropSelectedLink.Invalidate();
			}
			finally { ShowDropMark = true; }
		}
		protected bool CheckKeyTipAsShortcut(BarShortcut shortcut) {
			if(!Ribbon.AllowKeyTips || !Ribbon.Visible || Ribbon.MergeOwner != null)
				return false;
			if(!Ribbon.KeyTipManager.Show)
				Ribbon.KeyTipManager.GeneratePageKeyTips();
			Keys key = shortcut.Key & (~Keys.Alt);
			foreach(ISupportRibbonKeyTip kt in Ribbon.KeyTipManager.Items) {
				if(kt.KeyTipEnabled && kt.ItemKeyTip == key.ToString()) {
					kt.Click();
					if(!kt.IsCommandItem) {
						Ribbon.ActivateKeyboardNavigation(true);	
					}
					DestroyKeyboardTimer();
					return true;
				}
			}
			if(key != Keys.None)
				DestroyKeyboardTimer();
			return false;
		}
		protected override DevExpress.XtraBars.MessageFilter.BarManagerHookResult OnShortcutItemClick(BarItem item, BarShortcut shortcut) {
			if(item == null && ((shortcut.Key & Keys.Alt) != 0)) {
				if(CheckKeyTipAsShortcut(shortcut))
					return DevExpress.XtraBars.MessageFilter.BarManagerHookResult.ProcessedExit;
			}
			if(item != null)
				DestroyKeyboardTimer();
			return base.OnShortcutItemClick(item, shortcut);
		}
		public override DevExpress.XtraBars.MessageFilter.BarManagerHookResult CheckShortcut(BarManager manager, KeyEventArgs e) {
			if(e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey) {
				DestroyKeyboardTimer();
			}
			return base.CheckShortcut(manager, e);
		}
		public override LinkDropTargetEnum DropSelectStyle {
			get { return base.DropSelectStyle; }
			set {
				base.DropSelectStyle = value;
				if(DropSelectedObject != null) InvalidateDropSelectedObject(true);
			}
		}
		public virtual object DropSelectedObject {
			get {
				if(dropSelectedObject is BarItemLink) return DropSelectedLink;
				return dropSelectedObject;
			}
			set { 
				if(DropSelectedObject == value) return;
				if(DropSelectedObject != null) InvalidateDropSelectedObject(false);
				dropSelectedObject = value;
				if(value is BarItemLink) DropSelectedLink = value as BarItemLink;
				if(DropSelectedObject != null) InvalidateDropSelectedObject(true);
			}
		}
		protected internal override void OnLinkClicked(BarItemLink itemLink) {
			base.OnLinkClicked(itemLink);
			if(itemLink is BarCustomContainerItemLink) return;
			if(Ribbon != null && itemLink.RibbonItemInfo != null ) {
				Ribbon.DeactivateKeyboardNavigation();
			}
		}
		protected override void RestoreActiveBarControl(IPopup p) {
			if(p.OwnerLink != null && p.OwnerLink.RibbonItemInfo != null) {
				RibbonControl ribbon = p.OwnerLink.RibbonItemInfo.OwnerControl as RibbonControl;
				if(ribbon == null) ribbon = Ribbon;
				ActiveBarControl = ribbon.IsKeyboardActive ? ribbon : null;
				return;
			}
			else {
				if(p.PopupCreator == Ribbon.ApplicationButtonDropDownControl)
					ActiveBarControl = Ribbon.IsKeyboardActive ? Ribbon : null;
			}
			base.RestoreActiveBarControl(p);
		}
		protected override void OnPopupClosed(IPopup popup, BarItemLink link) {
			base.OnPopupClosed(popup, link);
			if(link != null && link.RibbonItemInfo != null && !link.IsDisposed) {
				link.RibbonItemInfo.UpdatePaintAppearance();
				link.RibbonItemInfo.InvalidateOwner();
			}
		}
		protected override ICustomBarControl GetEditContainer(BarEditItemLink link) {
			if(link.RibbonItemInfo == null || link.IsLinkInMenu) {
				return base.GetEditContainer(link);
			}
			RibbonItemViewInfo itemInfo = link.RibbonItemInfo;
			return (ICustomBarControl)itemInfo.ViewInfo.OwnerControl;
		}
		protected override void OnActiveEditor_LostFocus(object sender, EventArgs e) {
			if(ActiveEditor != null && !ActiveEditor.EditorContainsFocus) {
				CloseEditor();
				ActiveBarControl = null;
			}
		}
		protected override void OnEditorHidden(BarItemLink link) {
			if(link.RibbonItemInfo == null) {
				base.OnEditorHidden(link);
				return;
			}
			link.Invalidate();
			if(EditingLink == null && link.RibbonItemInfo != null && !link.RibbonItemInfo.ViewInfo.OwnerControl.IsDisposed) 
				link.RibbonItemInfo.ViewInfo.UpdateHotObject(true);
		}
		protected internal override void OnEditorEnter(BarEditItemLink link) {
			if(link.RibbonItemInfo == null) {
				base.OnEditorEnter(link);
				return;
			}
			if(!link.CloseEditor()) return;
			ClearUpdateHotObject(link);
		}
		protected internal override void OnEditorEscape(BarEditItemLink link) {
			if(link.RibbonItemInfo == null) {
				base.OnEditorEscape(link);
				return;
			}
			ClearUpdateHotObject(link);
		}
		void ClearUpdateHotObject(BarItemLink link) {
			link.Invalidate();
			Clear();
			if(EditingLink == null && link.RibbonItemInfo != null) link.RibbonItemInfo.ViewInfo.UpdateHotObject(true);
		}
		protected override void AddLinkAnimation(BarItemLink link, ObjectState oldState, ObjectState newState, bool fade, bool fadeIn) {
			if(link.RibbonItemInfo == null) {
				base.AddLinkAnimation(link, oldState, newState, fade, fadeIn);
				return;
			}
			if(!CanAnimate) return;
		}
		protected override bool IsAllowActivateMenu {
			get {
				return Ribbon != null && Ribbon.IsHandleCreated && Ribbon.Visible && Ribbon.IsAllowDisplayRibbon;
			}
		}
		protected internal override void HideToolTip() {
			base.HideToolTip();
			ToolTipControlInfo info = Manager.GetToolTipController().ActiveObjectInfo;
			if(Manager.GetToolTipController().ActiveControlClient == Ribbon) {
				Manager.GetToolTipController().HideHint();
				Manager.GetToolTipController().ShowHint((ToolTipControlInfo)null);
			}
		}
		protected override bool IsMainMenuActive { get { return Ribbon.IsKeyboardActive; } }
		System.Windows.Forms.Timer keyboardTimer = null;
		protected System.Windows.Forms.Timer KeyboardTimer {
			get { return keyboardTimer; }
		}
		protected void DelayedActivateKeyboardNavigation() {
			if(KeyboardTimer == null) {
				this.keyboardTimer = new System.Windows.Forms.Timer();
				this.keyboardTimer.Interval = 1000;
				this.keyboardTimer.Tick += new EventHandler(OnKeyboardTimerTick);
				KeyboardTimer.Start();
			}
		}
		protected internal void DestroyKeyboardTimer() {
			if(KeyboardTimer == null)
				return;
			KeyboardTimer.Stop();
			KeyboardTimer.Tick -= new EventHandler(OnKeyboardTimerTick);
			KeyboardTimer.Dispose();
			this.keyboardTimer = null;
		}
		void OnKeyboardTimerTick(object sender, EventArgs e) {
			DestroyKeyboardTimer();
			if(Form.ActiveForm != Ribbon.FindForm())
				return;
			ignoreNextAltUp = true;
			Ribbon.ActivateKeyboardNavigation();
		}
		protected void ActivateMainMenu(bool delayedShowKeyTips) {
			if(Ribbon.IsKeyboardActive)
				return;
			OnCloseAll(BarMenuCloseType.All);
			if(delayedShowKeyTips)
				DelayedActivateKeyboardNavigation();
			else {
				DestroyKeyboardTimer();
				Ribbon.ActivateKeyboardNavigation();
			}
		}
		protected override void ActivateMainMenu() {
			ActivateMainMenu(false);
		}
		protected override void DeactivateMainMenu() {
			OnCloseAll(BarMenuCloseType.All);
			DestroyKeyboardTimer();
			Ribbon.DeactivateKeyboardNavigation();
		}
		protected override bool CheckActivateMainMenu(bool keyUp) {
			if(Control.ModifierKeys != Keys.None && Control.ModifierKeys != Keys.Alt) return false;
			if(!keyUp && !RibbonControl.AllowSystemShortcuts && !Ribbon.IsKeyboardActive) {
				ActivateMainMenu(true);
				return false;
			}
			if(ignoreNextAltUp && keyUp) {
				if(Ribbon.KeyTipManager.Show)
					Ribbon.KeyTipManager.ShowKeyTips();
				ignoreNextAltUp = false;
				return false;
			}
			if(keyUp) {
				if(IsMainMenuActive)
					DeactivateMainMenu();
				else
					ActivateMainMenu();
			}
			return !RibbonControl.AllowSystemShortcuts;
		}
		protected internal override void OnItemClickClear(BarItemLink link) {
			base.OnItemClickClear(link);
			Ribbon.ClosePopupForms();
			Ribbon.DeactivateKeyboardNavigation();
		}
		protected internal override void InitHighligthTimer(BarItemLink link) {
			if(Ribbon != null && Ribbon.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal && Ribbon.Manager.GetPopupShowMode(link as IPopup) == PopupShowMode.Inplace) return;
			base.InitHighligthTimer(link);
		}
	}
}
