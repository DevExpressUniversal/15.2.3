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
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Skins;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.Accessible;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Text.Internal;
namespace DevExpress.XtraBars.Ribbon.Handler {
	public abstract class BaseHandler {
		public abstract Control OwnerControl { get; }
		public virtual void UpdateHotObject(DXMouseEventArgs e, bool forceUpdate) { }
		public virtual void OnMouseLeave(DXMouseEventArgs e) { }
		public virtual void OnMouseEnter(DXMouseEventArgs e) { }
		public virtual void OnMouseMove(DXMouseEventArgs e) { }
		public virtual void OnLostCapture() { }
		public virtual void OnMouseDown(DXMouseEventArgs e) { }
		public virtual void OnMouseUp(DXMouseEventArgs e) { }
		public virtual void OnMouseWheel(DXMouseEventArgs e) { }
		public virtual void OnKeyDown(KeyEventArgs e) { }
		public virtual void OnKeyUp(KeyEventArgs e) { }
		public virtual void OnKeyPress(KeyPressEventArgs e) { }
		public virtual void OnFullScreenButtonClicked(MouseEventArgs e) { }
		public virtual bool IsNeededKey(KeyEventArgs e) { return false; }
		public virtual bool IsInterceptKey(KeyEventArgs e) { return false; }
		public virtual NavigationObjectRowCollection GetNavObjectGrid() { return null; }
		public virtual bool ProcessMouseWheel(Point pt, int delta) { return false; }
		protected internal virtual NavigationObjectRow GetNavObjectList() {
			return new NavigationObjectRow();
		}
	}
	public class EmptyHandler : BaseHandler {
		public override Control OwnerControl { get { return null; } }
	}
	public abstract class BaseRibbonHandler : BaseHandler {
		LinksNavigation navigation;
		public BaseRibbonHandler() { }
		protected abstract BarSelectionInfo SelectionInfo { get; }
		protected abstract BaseRibbonViewInfo ViewInfo { get; }
		protected abstract bool IsDesignTime { get; }
		public override void OnMouseLeave(DXMouseEventArgs e) {
			ClearGalleriesClickedItems();
			UpdateHotObject(new DXMouseEventArgs( MouseButtons.None, 0, -10000, -10000, 0), true);
		}
		Point hitPoint;
		protected Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		protected internal Point LastHitPoint { get; private set; } 
		protected abstract LinksNavigation CreateLinksNavigator();
		public LinksNavigation Navigation {
			get {
				if(navigation == null) navigation = CreateLinksNavigator();
				return navigation;
			}
		}
		public override void OnMouseEnter(DXMouseEventArgs e) {
			UpdateHotObject(e, false);
		}
		public override void OnMouseMove(DXMouseEventArgs e) {
			if(IsDesignTime) {
				if(ViewInfo.DesignTimeManager.ProcessMouseMove(e)) return;
			}
			RibbonHitInfo oldInfo = ViewInfo.HotObject;
			UpdateHotObject(e, false);
			CheckUpdateHyperlink(e);
			if(SelectionInfo.HighlightedLink != null && SelectionInfo.HighlightedLink == ViewInfo.HotObject.Item) {
				SelectionInfo.HighlightedLink.OnLinkActionCore(BarLinkAction.MouseMove, e);
			}
			if(ViewInfo.HotObject.InGallery && e.Button == MouseButtons.Left && !IsDesignTime && ViewInfo.HotObject.Gallery.AllowMarqueeSelection) {
				ViewInfo.HotObject.GalleryInfo.UpdateSelectionRect(HitPoint, e.Location);
			}
			UpdateImageGalleries();
			if(ShouldRaiseItemLeave(oldInfo, ViewInfo.HotObject)) oldInfo.Gallery.RaiseGalleryItemLeave(oldInfo.GalleryItem);
			if(ViewInfo.HotObject.InGallery) ViewInfo.HotObject.Gallery.InitHoverTimer(ViewInfo.HotObject);
		}
		Cursor SavedCursor { get; set; }
		protected virtual void CheckUpdateHyperlink(DXMouseEventArgs e) {
			StringBlock block = null;
			if(ViewInfo.HotObject.InItem && ViewInfo.HotObject.ItemInfo.HtmlStringInfo != null)
				block = ViewInfo.HotObject.ItemInfo.HtmlStringInfo.GetLinkByPoint(e.Location);
			if(block != null) { 
				if(SavedCursor == null)
					SavedCursor = OwnerControl.Cursor;
				OwnerControl.Cursor = Cursors.Hand;
			}
			else {
				if(SavedCursor != null)
					OwnerControl.Cursor = SavedCursor;
				SavedCursor = null;
			}
		}
		protected virtual bool ShouldRaiseItemLeave(RibbonHitInfo oldInfo, RibbonHitInfo newInfo) {
			if(!oldInfo.InGalleryItem) return false;
			if(!newInfo.InGalleryItem || oldInfo.GalleryItem != newInfo.GalleryItem) return true;
			return false;
		}
		protected virtual void UpdateImageGallery(InRibbonGalleryRibbonItemViewInfo vi) {
			if(vi == null) return;
			vi.GalleryInfo.Gallery.HideImageForms(ViewInfo.HotObject.GalleryItemInfo, true);
		}
		protected internal virtual void UpdateImageGalleries() {
			RibbonViewInfo vi = ViewInfo as RibbonViewInfo;
			if(vi == null) return;
			for(int i = 0; i < vi.Panel.Groups.Count; i++) {
				for(int j = 0; j < vi.Panel.Groups[i].Items.Count; j++) {
					UpdateImageGallery(vi.Panel.Groups[i].Items[j] as InRibbonGalleryRibbonItemViewInfo);
				}
			}
			if(ViewInfo.HotObject.HitTest == RibbonHitTest.GalleryImage && ViewInfo.HotObject.GalleryInfo.Gallery.AllowHoverImages) {
				if(!ViewInfo.HotObject.GalleryInfo.AllowPartitalItems && ViewInfo.HotObject.GalleryItemInfo.IsPartiallyVisible) return;
				if(!ShouldShowImageForm) return;
				ViewInfo.HotObject.GalleryInfo.Gallery.ShowImageForm(ViewInfo.HotObject.GalleryInfo, ViewInfo.HotObject.GalleryItemInfo);
			}
		}
		protected virtual bool ShouldShowImageForm {
			get {
				Form form1 = OwnerControl.FindForm();
				Form activeForm = Form.ActiveForm;
				if(form1 == null) return false;
				if(form1 == activeForm) return true;
				if(form1.Parent != null) form1 = form1.Parent.FindForm();
				if(form1 == null || form1 != activeForm) return false;
				return true;
			}
		}
		protected internal virtual void ClearGalleriesClickedItems() {
			RibbonViewInfo vi = ViewInfo as RibbonViewInfo;
			if(vi == null) return;
			for(int i = 0; i < vi.Panel.Groups.Count; i++) {
				for(int j = 0; j < vi.Panel.Groups[i].Items.Count; j++) {
					InRibbonGalleryRibbonItemViewInfo galleryInfo = vi.Panel.Groups[i].Items[j] as InRibbonGalleryRibbonItemViewInfo;
					if(galleryInfo == null) continue;
					galleryInfo.GalleryInfo.Gallery.DownItem = null;
				}
			}
		}
		public override void UpdateHotObject(DXMouseEventArgs e, bool forceUpdate) {
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(e.X, e.Y);
			if(!ViewInfo.IsRibbonFormActive && !(Form.ActiveForm is MinimizedRibbonPopupForm)) hitInfo.SetHitTest(RibbonHitTest.None);
			if(CanHotTrack(hitInfo) || forceUpdate) ViewInfo.HotObject = hitInfo;
		}
		protected virtual void OnClickItem(RibbonHitInfo hitInfo) {
			BarItemLink link = hitInfo.Item as BarItemLink;
			if(link != null) {
				if(hitInfo.HitTest != RibbonHitTest.ItemDrop)
					SelectionInfo.ClickLink(link);
			}
		}
		protected virtual void OnUnPressItem(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			OwnerControl.Capture = false;
			BarItemLink link = hitInfo.Item as BarItemLink;
			if(e.Button == MouseButtons.Left && link != null) {
				CheckNavigateHyperlink(e, hitInfo);
				SelectionInfo.UnPressLink(link);
				if(SelectionInfo == null || SelectionInfo.Manager.IsDestroying)
					return;
				if(OwnerControl.FindForm() != null && !(OwnerControl.FindForm() is RibbonBasePopupForm)) {
					if(ViewInfo.OwnerControl != null) {
						Point pt = ViewInfo.OwnerControl.PointToClient(Control.MousePosition);
						if(pt != hitInfo.HitPoint) hitInfo.HitPoint = pt;
					}
					UpdateHotObject(new DXMouseEventArgs(MouseButtons.None, 0, hitInfo.HitPoint.X, hitInfo.HitPoint.Y, 0), true);
					ViewInfo.OnHotObjectChanged(new RibbonHitInfo(), ViewInfo.HotObject);
				}
			}
			if(hitInfo.InGallery) OnUnPressImageGallery(e, hitInfo);
		}
		protected virtual void CheckNavigateHyperlink(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			if(!hitInfo.InItem || hitInfo.ItemInfo.HtmlStringInfo == null)
				return;
			StringBlock block = hitInfo.ItemInfo.HtmlStringInfo.GetLinkByPoint(e.Location);
			if(block == null)
				return;
			hitInfo.Item.Item.OnHyperLinkClick(hitInfo.Item, block);
		}
		public override void OnLostCapture() {
			Point p = OwnerControl.PointToClient(Control.MousePosition);
			if(!SuppressClearPressedInfo) {
			ViewInfo.PressedObject = null;
			UpdateHotObject(new DXMouseEventArgs(MouseButtons.None, 0, p.X, p.Y, 0), false);
			ViewInfo.OnHotObjectChanged(new RibbonHitInfo(), ViewInfo.HotObject);
		}
		}
		public override void OnMouseDown(DXMouseEventArgs e) {
			HitPoint = LastHitPoint = e.Location;
			if(IsDesignTime) {
				if(ViewInfo.DesignTimeManager.ProcessMouseDown(e)) return;
			}
			UpdateHotObject(e, false);
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(e.X, e.Y);
			if(hitInfo.HitTest == RibbonHitTest.PageGroupCaptionButton) UpdateHotObject(e, true);
			if(e.Button == MouseButtons.Left) {
				if(CanPress(hitInfo)) {
					OnPress(e, hitInfo);
				}
			}
			else if(e.Button == MouseButtons.Right && hitInfo.InGallery) {
				OnPress(e, hitInfo);
			}
		}
		protected bool IsApplicationButtonDoubleClickRaised = false;
		public override void OnMouseUp(DXMouseEventArgs e) {
			if(IsDesignTime) {
				if(ViewInfo.DesignTimeManager.ProcessMouseUp(e)) {
					IsApplicationButtonDoubleClickRaised = false;
					return;
				}
			}
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(e.X, e.Y);
			if(ViewInfo.PressedObject.IsEmpty) {
				IsApplicationButtonDoubleClickRaised = false;
				if(hitInfo.InGallery)
					OnUnPressImageGallery(e, hitInfo);
				return;
			}
			RibbonHitInfo pressed = ViewInfo.PressedObject;
			ViewInfo.PressedObject = null;
			if(e.Button == MouseButtons.Left) {
				bool isSame = hitInfo.Equals(pressed);
				OnUnPress(e, hitInfo);
				if(isSame) OnClick(pressed);
			}
			else if(e.Button == MouseButtons.Right) {
				OnUnPress(e, hitInfo);
			}
			if(ViewInfo != null && ViewInfo.Manager != null)
				ViewInfo.Manager.SelectionInfo.PressedLink = null;
			IsApplicationButtonDoubleClickRaised = false;
		}
		protected virtual bool CanHotTrack(RibbonHitInfo hitInfo) {
			if(!ViewInfo.PressedObject.IsEmpty) return false;
			if(SelectionInfo.OpenedPopups.Count > 0) return false;
			if(IsDesignTime) return false;
			return true;
		}
		protected virtual bool CanPress(RibbonHitInfo hitInfo) {
			if(hitInfo.InItem) return CanPressItem(hitInfo);
			if(hitInfo.HitTest == RibbonHitTest.PageHeaderCategory) return true;
			if(hitInfo.PageGroupInfo != null && hitInfo.PageGroupInfo.Minimized) return true;
			if(hitInfo.HitTest == RibbonHitTest.PageHeader) return true;
			if(hitInfo.HitTest == RibbonHitTest.PageGroupCaptionButton) return true;
			if(hitInfo.HitTest == RibbonHitTest.GalleryDropDownButton ||
				hitInfo.HitTest == RibbonHitTest.GalleryUpButton ||
				hitInfo.HitTest == RibbonHitTest.GalleryDownButton || 
				hitInfo.HitTest == RibbonHitTest.GalleryLeftButton || 
				hitInfo.HitTest == RibbonHitTest.GalleryRightButton) return true;
			if(hitInfo.HitTest == RibbonHitTest.GalleryItem || 
				hitInfo.HitTest == RibbonHitTest.GalleryImage) return true;
			if(hitInfo.HitTest == RibbonHitTest.PageHeaderLeftScroll || 
				hitInfo.HitTest == RibbonHitTest.PageHeaderRightScroll) return true;
			if(hitInfo.HitTest == RibbonHitTest.PanelLeftScroll ||
				hitInfo.HitTest == RibbonHitTest.PanelRightScroll) return true;
			return false;
		}
		protected virtual bool CanPressItem(RibbonHitInfo hitInfo) {
			BarItemLink link = hitInfo.Item as BarItemLink;
			return hitInfo.Item.Enabled && (link == null || link.Enabled);
		}
		protected bool SuppressClearPressedInfo = false;
		protected virtual void OnPress(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			ViewInfo.PressedObject = hitInfo;
			if(hitInfo.InPageHeaderScrollButtons) OnScrollPageHeader(e, hitInfo);
			if(hitInfo.InPanelScrollButtons) OnScrollPanel(e, hitInfo);
			if(hitInfo.InPageCategory && !hitInfo.InPage) OnPageCategoryPress(hitInfo.PageCategory);
			if(hitInfo.InItem) OnPressItem(e, hitInfo);
			if(SelectionInfo == null)
				return;
			if(hitInfo.InGallery) OnPressImageGallery(e, hitInfo);
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) {
				SuppressClearPressedInfo = true;
				OwnerControl.Capture = false;
				SuppressClearPressedInfo = false;
			}
			else if(SelectionInfo.EditingLink == null) OwnerControl.Capture = true;
		}
		protected virtual void OnPageCategoryPress(RibbonPageCategory cat) { }
		protected virtual void OnPageCategoryUnPress(RibbonPageCategory cat) { }
		protected virtual void OnScrollPageHeader(DXMouseEventArgs e, RibbonHitInfo hitInfo) { }
		protected virtual void OnScrollPanel(DXMouseEventArgs e, RibbonHitInfo hitInfo) { }
		protected virtual void OnUnPressPageHeaderScroll(DXMouseEventArgs e, RibbonHitInfo hitInfo) { }
		protected virtual void OnUnPressPanelScroll(DXMouseEventArgs e, RibbonHitInfo hitInfo) { }
		protected virtual void OnClick(RibbonHitInfo hitInfo) {
		}
		protected virtual void OnUnPress(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			OnUnPressPageHeaderScroll(e, hitInfo);
			OnUnPressPanelScroll(e, hitInfo);
			if(hitInfo.InItem) OnUnPressItem(e, hitInfo);
			else if(hitInfo.InGallery) OnUnPressImageGallery(e, hitInfo);
			else if(hitInfo.InPageCategory && !hitInfo.InPage) OnPageCategoryUnPress(hitInfo.PageCategory);
		}
		protected virtual void OnUnPressImageGallery(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			(hitInfo.GalleryBarItemInfo.Item as RibbonGalleryBarItemLink).OnUnPressImageGallery(e, hitInfo);
		}
		protected virtual void OnPressImageGallery(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			if(!SelectionInfo.CloseEditor()) return;
			(hitInfo.GalleryBarItemInfo.Item as RibbonGalleryBarItemLink).OnPressImageGallery(e, hitInfo);
		}
		protected virtual void OnPressItem(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			if(e.Clicks == 1) OnPressItem(hitInfo);
			else OnDblClickItem(hitInfo);
		}
		protected virtual void OnPressApplicationButton(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			if(e.Clicks > 1) OnDblClickApplicationButton(hitInfo); 
		}
		protected virtual void OnDblClickApplicationButton(RibbonHitInfo hitInfo) { }
		protected virtual void OnDblClickItem(RibbonHitInfo hitInfo) {
			BarItemLink link = hitInfo.Item as BarItemLink;
			if(link != null) {
				if(hitInfo.HitTest != RibbonHitTest.ItemDrop)
					SelectionInfo.DoubleClickLink(link); 
			}
		}
		protected virtual void OnPressItem(RibbonHitInfo hitInfo) {
			BarItemLink link = hitInfo.Item as BarItemLink;
			if(link != null) {
				if(hitInfo.HitTest == RibbonHitTest.ItemDrop)
					SelectionInfo.PressLinkArrow(link);
				else {
					SelectionInfo.PressLink(link);
				}
			}
		}
		protected virtual BarItemLink ActiveLink { 
			get {
				if(SelectionInfo == null) return null;
				if(SelectionInfo.EditingLink == null) return null;
				if(SelectionInfo.EditingLink.GetRibbonViewInfo() == ViewInfo) return SelectionInfo.EditingLink;
				return null;
			}
		}
		protected virtual bool IsKeyboardActive { get { return false; } }
		protected virtual bool IsEditorActive { get { return false; } }
		public override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled || (!IsKeyboardActive && !IsEditorActive)) return;
			if(Navigation != null) Navigation.ProcessKeyDown(e, ActiveLink);
		}
		public override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled || (!IsKeyboardActive && !IsEditorActive)) return;
			if(Navigation != null) Navigation.ProcessKeyUp(e, ActiveLink);
		}
		public override bool IsNeededKey(KeyEventArgs e) {
			if((!IsKeyboardActive && !IsEditorActive)) return false;
			if(base.IsNeededKey(e)) return true;
			if(Navigation != null) return Navigation.IsNeededKey(e, ActiveLink);
			return false;
		}
		public override bool IsInterceptKey(KeyEventArgs e) {
			if((!IsKeyboardActive && !IsEditorActive)) return false;
			if(Navigation != null) return Navigation.IsInterceptKey(e, ActiveLink);
			return false;
		}
	}
	public class RibbonMinimizedHandler : RibbonHandler {
		public RibbonMinimizedHandler(RibbonControl ribbon)
			: base(ribbon) {
		}
		protected override NavigationObjectRow CreatePagesRow() { return null; }
		protected new RibbonMinimizedControl Ribbon { get { return base.Ribbon as RibbonMinimizedControl; } }
		protected internal override void OnClickPageGroupCaptionButton(RibbonPageGroup pageGroup) {
			RibbonControl sourceRibbon = Ribbon.SourceRibbon;
			((RibbonHandler)sourceRibbon.Handler).OnClickPageGroupCaptionButton(pageGroup);
		}
	}
	public class RibbonPopupGroupHandler : RibbonHandler {
		public RibbonPopupGroupHandler(RibbonControl ribbon) : base(ribbon) { }
		protected new RibbonOneGroupControl Ribbon { get { return base.Ribbon as RibbonOneGroupControl; } }
		protected internal override void OnClickPageGroupCaptionButton(RibbonPageGroup pageGroup) {
			RibbonControl sourceRibbon = Ribbon.SourceRibbon;
			((RibbonHandler)sourceRibbon.Handler).OnClickPageGroupCaptionButton(pageGroup);
		}
		protected override NavigationObjectRow CreatePagesRow() { return null; }
	}
	internal delegate bool ShouldIgnoreToolbarItem(RibbonItemViewInfo itemInfo, object toolbar);
	public class RibbonHandler : BaseRibbonHandler {
		RibbonControl ribbon;
		public RibbonHandler(RibbonControl ribbon) {
			this.ribbon = ribbon;
		}
		protected override LinksNavigation CreateLinksNavigator() { return new RibbonLinksNavigation(Ribbon); }
		protected override BarItemLink ActiveLink {
			get {
				if(base.ActiveLink != null) return base.ActiveLink;
				NavigationObjectRibbonItem item = ViewInfo.KeyboardActiveObject as NavigationObjectRibbonItem;
				if(item != null) return item.ItemLink;
				return null;
			}
		}
		protected RibbonControl Ribbon { get { return ribbon; } }
		protected override bool IsKeyboardActive { get { return Ribbon.IsKeyboardActive; } }
		protected override bool IsEditorActive { get { return Ribbon.Manager.ActiveEditor != null; } }
		protected override bool IsDesignTime { get { return Ribbon.IsDesignMode; } }
		protected override bool CanHotTrack(RibbonHitInfo hitInfo) {
			if(Ribbon.IsPopupFormOpened) return false;
			if(!IsDesignTime && hitInfo.HitTest == RibbonHitTest.ApplicationButton) return true;
			return base.CanHotTrack(hitInfo);
		}
		protected override bool CanPress(RibbonHitInfo hitInfo) {
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) return true;
			return base.CanPress(hitInfo);
		}
		protected override void OnPressItem(RibbonHitInfo hitInfo) {
			BarItemLink link = hitInfo.Item as BarItemLink;
			if(link != null) {
				if(Ribbon.IsSystemLink(link)) {
					Ribbon.UpdateSystemLinkGlyph(link, ObjectState.Pressed);
				}
			}
			base.OnPressItem(hitInfo);
		}
		public override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled) return;
			if(Ribbon.KeyTipManager.Show) Ribbon.KeyTipManager.AddChar(e.KeyChar);
		}
		public override bool ProcessMouseWheel(Point pt, int delta) {
			ArrayList pages = Ribbon.TotalPageCategory.GetVisiblePages();
			if(pages.Count == 0) return false;
			pt = Ribbon.PointToClient(pt);
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(pt.X, pt.Y);
			if(hitInfo.HitTest == RibbonHitTest.HeaderPanel || hitInfo.InPage) {
				if(Ribbon.SelectedPage == null) {
					if(delta > 0) Ribbon.SelectedPage =  pages[0] as RibbonPage;
					else Ribbon.SelectedPage = pages[pages.Count - 1] as RibbonPage;
				}
				int pageIndex = pages.IndexOf(Ribbon.SelectedPage);
				pageIndex -= delta / 120;
				if(pageIndex < 0) pageIndex = 0;
				else if(pageIndex >= pages.Count) pageIndex = pages.Count - 1;
				Ribbon.SelectedPage = pages[pageIndex] as RibbonPage;
				return true;
			}
			return false;
		}
		public override void OnMouseWheel(DXMouseEventArgs e) {
			ProcessMouseWheel(Ribbon.PointToScreen(e.Location), e.Delta);
		}
		protected bool CanShowCustomizationMenuForItem(RibbonHitInfo hitInfo) {
			return hitInfo.InItem || hitInfo.HitTest == RibbonHitTest.PageHeader || hitInfo.HitTest == RibbonHitTest.PageGroup || hitInfo.HitTest == RibbonHitTest.PageGroupCaption || hitInfo.InGallery || hitInfo.HitTest == RibbonHitTest.Panel || hitInfo.HitTest == RibbonHitTest.ApplicationButton;
		}
		public override void OnMouseUp(DXMouseEventArgs e) {
			base.OnMouseUp(e);
			if(IsDesignTime || Ribbon.IsDisposed) return;
			if(RibbonViewInfo.Caption.OnMouseUp(e)) return;
			if(e.Button != MouseButtons.Right)
				return;
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(e.X, e.Y);
			if(!CanShowCustomizationMenuForItem(hitInfo))
				return;
			Point screen = Ribbon.PointToScreen(new Point(e.X, e.Y));
			BarItemLink link = hitInfo.Item;
			if(Ribbon.CustomizationPopupMenu.Visible)
				Ribbon.CustomizationPopupMenu.HidePopup();
			Ribbon.CustomizationPopupMenu.UpdateMenu(Ribbon.AllowChangeToolbarLocationMenuItem, link);
			RibbonCustomizationMenuEventArgs args = new RibbonCustomizationMenuEventArgs(hitInfo, Ribbon);
			args.Link = link;
			Ribbon.RaiseShowCustomizationMenu(args);
			if((args.ShowCustomizationMenu.HasValue && !args.ShowCustomizationMenu.Value) || (!args.ShowCustomizationMenu.HasValue && !CanShowCustomizationMenu))
				return;
			if(Ribbon.ContextMenu == null) {
				if(Ribbon.ContextMenuStrip == null) {
					if(hitInfo.HitTest == RibbonHitTest.PageGroup || hitInfo.HitTest == RibbonHitTest.PageGroupCaption) {
						link = hitInfo.PageGroup.GetOriginalPageGroup().ToolbarContentButtonLink;
					}
					CheckCloseLinkOpenInTabletStyle(link);
					Ribbon.CustomizationPopupMenu.Show(screen.X, screen.Y, Ribbon.AllowChangeToolbarLocationMenuItem, link);
				}
			}
			else Ribbon.CustomizationPopupMenu.HidePopup();
		}
		private void CheckCloseLinkOpenInTabletStyle(BarItemLink link) {
			if(Ribbon.GetRibbonStyle() != RibbonControlStyle.TabletOffice)
				return;
			BarButtonItemLink buttonLink = link as BarButtonItemLink;
			if(buttonLink != null && buttonLink.IsPopupVisible) {
				buttonLink.HidePopup();
				return;
			}
			BarSubItemLink subItemLink = link as BarSubItemLink;
			if(subItemLink != null && subItemLink.Opened) {
				subItemLink.Opened = false;
			}
		}
		public override void OnFullScreenButtonClicked(MouseEventArgs e) {
			base.OnFullScreenButtonClicked(e);
			if(RibbonViewInfo.CanUseFullScreenMode) Ribbon.OnFullScreenModeChangeCore();
		}
		protected virtual bool CanShowCustomizationMenu {
			get { return !IsDesignTime && Ribbon.ToolbarLocation != RibbonQuickAccessToolbarLocation.Hidden; }
		}
		protected override void OnPressApplicationButton(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			base.OnPressApplicationButton(e, hitInfo);
			if(e.Clicks == 1)
				ShowApplicationButtonPopup();
		}
		protected override void OnPress(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			base.OnPress(e, hitInfo);
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) {
				if(RibbonViewInfo.ApplicationButtonPopupActive) {
					if(e.Clicks > 1) {
						OnPressApplicationButton(e, hitInfo);
					}
					SelectionInfo.CloseAllPopups();
					RibbonViewInfo.ApplicationButtonPopupActive = false;
					return;
				}
				else {
					OnPressApplicationButton(e, hitInfo);
				}
			}
			if(hitInfo.HitTest == RibbonHitTest.PageHeader) {
				Ribbon.HideApplicationButtonContentControl();
				if(e.Clicks > 1) {
					Ribbon.SwitchMinimized();
					return;
				}
				OnPressHeaderPage(hitInfo);
			}
			if(hitInfo.HitTest == RibbonHitTest.PageGroup) OnPressPageGroup(hitInfo);
		}
		protected override void OnClick(RibbonHitInfo hitInfo) {
			base.OnClick(hitInfo);
			if(hitInfo.HitTest == RibbonHitTest.PageGroupCaptionButton) OnClickPageGroupCaptionButton(hitInfo.PageGroup);
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) OnApplicationButtonClick();
		}
		protected override void OnPageCategoryPress(RibbonPageCategory cat) {
			if(cat == null || Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				return;
			if(RibbonForm != null && RibbonForm.ShouldDraggingByPageCategory())
				return;
			OnPageCategoryPressCore(cat);
		}
		protected override void OnPageCategoryUnPress(RibbonPageCategory cat) {
			if(cat == null || Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice)
				return;
			if(RibbonForm != null && !RibbonForm.ShouldDraggingByPageCategory())
				return;
			OnPageCategoryPressCore(cat);
		}
		protected virtual void OnPageCategoryPressCore(RibbonPageCategory cat) {
			foreach(RibbonPageViewInfo vi in Ribbon.ViewInfo.Header.Pages) {
				if(vi.Page.Category == cat) {
					Ribbon.SelectedPage = vi.Page;
					break;
				}
			}
		}
		protected override void OnDblClickApplicationButton(RibbonHitInfo hitInfo) {
			base.OnDblClickApplicationButton(hitInfo);
			Ribbon.RaiseApplicationButtonDoubleClick(EventArgs.Empty);
			IsApplicationButtonDoubleClickRaised = true;
		}
		protected internal virtual void ShowApplicationButtonPopup() {
			if(Ribbon.IsDesignMode || (!Ribbon.IsHandleCreated && Ribbon.ApplicationButtonDropDownControl == null))
				return;
			Ribbon.ToggleApplicationButtonContentControlVisibility();
		}
		protected internal virtual void OnApplicationButtonClickCore() {
			if(!IsApplicationButtonDoubleClickRaised)
			Ribbon.RaiseApplicationButtonClick(EventArgs.Empty);
		}
		protected virtual void OnApplicationButtonClick() {
			OnApplicationButtonClickCore();
		}
		protected virtual void ShowContentDropDown(RibbonPageGroupViewInfo groupInfo) {
			if(groupInfo.IsDroppedDown) {
				if(Ribbon.Manager.ActiveEditor != null)
					Ribbon.Manager.ActiveEditItemLink.CloseEditor();
				Ribbon.PopupGroupForm = null;
				return;
			}
			ViewInfo.HotObject = null;
			groupInfo.ShowContentDropDown();
		}
		protected virtual void OnPressPageGroup(RibbonHitInfo hitInfo) {
			if(hitInfo.PageGroupInfo.Minimized) ShowContentDropDown(hitInfo.PageGroupInfo);
		}
		protected override void OnUnPressPanelScroll(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			XtraAnimator.RemoveObject(ViewInfo, RibbonHitTest.PanelLeftScroll);
			XtraAnimator.RemoveObject(ViewInfo, RibbonHitTest.PanelRightScroll);
		}
		protected override void OnUnPressPageHeaderScroll(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			XtraAnimator.RemoveObject(ViewInfo, RibbonHitTest.PageHeaderLeftScroll);
			XtraAnimator.RemoveObject(ViewInfo, RibbonHitTest.PageHeaderRightScroll);
		}
		protected override void OnScrollPageHeader(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			RibbonViewInfo vi = (RibbonViewInfo)ViewInfo;
			if(hitInfo.HitTest == RibbonHitTest.PageHeaderLeftScroll) {
				vi.Header.InitializeLeftScroll();
			}
			else if(hitInfo.HitTest == RibbonHitTest.PageHeaderRightScroll) {
				vi.Header.InitializeRightScroll();
			}
		}
		protected override void OnScrollPanel(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			RibbonViewInfo vi = (RibbonViewInfo)ViewInfo;
			if(hitInfo.HitTest == RibbonHitTest.PanelLeftScroll) {
				vi.Panel.InitializeLeftScroll();
			}
			else if(hitInfo.HitTest == RibbonHitTest.PanelRightScroll) {
				vi.Panel.InitializeRightScroll();
			}
		}
		protected virtual void OnPressHeaderPage(RibbonHitInfo hitInfo) {
			if(Ribbon.IsOfficeTablet && !IsDesignTime) {
				OnPressHeaderPageTablet(hitInfo);
				return;
			}
			OnPressPageHeaderDescktop(hitInfo);
		}
		private void OnPressPageHeaderDescktop(RibbonHitInfo hitInfo) {
			if(RibbonViewInfo.GetIsMinimized()) {
				if(Ribbon.SelectedPage == hitInfo.Page && Ribbon.MinimizedRibbonPopupForm != null) {
					Ribbon.MinimizedRibbonPopupForm = null;
					return;
				}
			}
			if(!hitInfo.Page.Category.Expanded)
				return;
			Ribbon.SelectedPage = hitInfo.Page;
			if(RibbonViewInfo.GetIsMinimized() && !Ribbon.IsMinimizedRibbonOpened) {
				Ribbon.DeactivateKeyboardNavigation();
				Ribbon.ShowMinimizedRibbon();
				return;
			}
		}
		protected virtual void OnPressHeaderPageTablet(RibbonHitInfo hitInfo) {
			if(hitInfo.Page == Ribbon.SelectedPage && Ribbon.AllowMinimizeRibbon) {
				Ribbon.Minimized = !Ribbon.Minimized;
				return;
			}
			if(!hitInfo.Page.Category.Expanded)
				return;
			Ribbon.SelectedPage = hitInfo.Page;
			if(Ribbon.AllowMinimizeRibbon && Ribbon.Minimized) Ribbon.Minimized = false;
		}
		protected internal virtual void OnClickPageGroupCaptionButton(RibbonPageGroup pageGroup) {
			Ribbon.ClosePopupForms();
			Ribbon.RaisePageGroupCaptionButtonClick(new RibbonPageGroupEventArgs(pageGroup));
		}
		public override Control OwnerControl { get { return Ribbon; } }
		protected override BarSelectionInfo SelectionInfo { get { return Ribbon.Manager == null ? null : Ribbon.Manager.SelectionInfo; } }
		protected override BaseRibbonViewInfo ViewInfo { get { return Ribbon.ViewInfo; } }
		protected RibbonViewInfo RibbonViewInfo { get { return Ribbon.ViewInfo; } }
		protected RibbonForm RibbonForm { get { return RibbonViewInfo != null ? RibbonViewInfo.Form : null; } }
		public override void OnMouseMove(DXMouseEventArgs e) {
			base.OnMouseMove(e);
			if(!IsDesignTime) RibbonViewInfo.Caption.OnMouseMove(e);
		}
		public override void OnMouseDown(DXMouseEventArgs e) {
			Ribbon.DeactivateKeyboardNavigation(false, false);
			base.OnMouseDown(e);
			if(!IsDesignTime) RibbonViewInfo.Caption.OnMouseDown(e);
		}
		public override void OnMouseLeave(DXMouseEventArgs e) {
			if(Ribbon.Manager.ActiveEditor != null && Ribbon.Manager.ActiveEditor.Bounds.Contains(e.Location))
				return;
			base.OnMouseLeave(e);
			RibbonViewInfo.Caption.OnMouseLeave(e);
		}
		int itemLinksBeginRow, itemLinksRowCount;
		protected internal int ItemLinksBeginRow { get { return itemLinksBeginRow; } }
		protected internal int ItemLinksRowCount { get { return itemLinksRowCount; } }
		public override NavigationObjectRowCollection GetNavObjectGrid() {
			NavigationObjectRowCollection grid = new NavigationObjectRowCollection();
			if(Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Above || Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Default)
				grid.Add(CreateQuickAccessToolbarRow());
			grid.Add(CreatePagesRow());
			this.itemLinksBeginRow = grid.Count;
			grid.Add(CreateItemsFirstRow());
			grid.Add(CreateItemsSecondRow());
			grid.Add(CreateItemsThirdRow());
			this.itemLinksRowCount = grid.Count - ItemLinksBeginRow;
			grid.Add(CreateGroupButtonsRow());
			if(Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Below)
				grid.Add(CreateQuickAccessToolbarRow());
			return grid;
		}
		protected virtual void AddItemCollectionToRow(RibbonItemViewInfoCollection coll, NavigationObjectRow row) {
			foreach(RibbonItemViewInfo itemInfo in coll) {
				RibbonButtonGroupItemViewInfo buttonInfo = itemInfo as RibbonButtonGroupItemViewInfo;
				InRibbonGalleryRibbonItemViewInfo galleryInfo = itemInfo as InRibbonGalleryRibbonItemViewInfo;
				if(buttonInfo != null) {
					foreach(RibbonItemViewInfo buttonChildInfo in buttonInfo.Items) {
						NavigationObject obj = GetNavigationObjectByObject(buttonChildInfo);
						if(obj != null)
							row.Add(obj);
					}
				}
				else if(galleryInfo != null)
					continue;
				else {
					NavigationObject obj = GetNavigationObjectByObject(itemInfo);
					if(obj != null)
						row.Add(obj);
				}
			}
		}
		protected virtual void AddPanelItemsToRow(NavigationObjectRow row) { 
			foreach(RibbonPageGroupViewInfo groupInfo in ((RibbonViewInfo)ViewInfo).Panel.Groups) {
				AddItemCollectionToRow(groupInfo.Items, row);
				if(groupInfo.PageGroup.ShowCaptionButton) {
					NavigationObject obj = GetNavigationObjectByObject(groupInfo);
					if(obj != null)
						row.Add(obj);
				}
			}
		}
		protected internal override NavigationObjectRow GetNavObjectList() {
			NavigationObjectRow row = new NavigationObjectRow();
			RibbonViewInfo vi = (RibbonViewInfo)ViewInfo;
			if(Ribbon.SelectedPage != null) {
				NavigationObject page = GetNavigationObjectByObject(Ribbon.SelectedPage.PageInfo);
				if(page != null)
					row.Add(page);
				AddPanelItemsToRow(row);
				if(vi.IsAllowApplicationButton) {
					NavigationObject obj = GetNavigationObjectByObject(vi.ApplicationButton);
					if(obj != null)
						row.Add(obj);
				}
				if(Ribbon.ToolbarLocation != RibbonQuickAccessToolbarLocation.Hidden )
					AddItemCollectionToRow(((RibbonViewInfo)ViewInfo).Toolbar.Items, row);
			}
			return row;
		}
		protected virtual Point GetNavigationObjectPosByObject(object obj) {
			return Ribbon.NavigatableObjects.FindNavObjectByObject(obj);
		}
		protected virtual NavigationObject GetNavigationObjectByObject(object obj) {
			Point pt = GetNavigationObjectPosByObject(obj);
			if(pt.X == -1)
				return null;
			return Ribbon.NavigatableObjects[pt.Y][pt.X];
		}
		protected virtual NavigationObjectRow CreateToolbarRow(RibbonQuickAccessToolbarViewInfo toolbar ) {
			NavigationObjectRow row = new NavigationObjectRow();
			RibbonItemViewInfoEnumerator itemEnum = new RibbonItemViewInfoEnumerator(toolbar.Items);
			for(; !itemEnum.End; itemEnum.Next()) {
				if(toolbar.Items.IndexOf(itemEnum.CurrentItem) >= toolbar.VisibleButtonCount)break;
				row.Add(new NavigationObjectRibbonToolbarItem(Ribbon, itemEnum.CurrentItem));
			}
			return row;
		}
		protected virtual NavigationObjectRow CreateQuickAccessToolbarRow() {
			NavigationObjectRow row = CreateToolbarRow(Ribbon.ViewInfo.Toolbar);
			if((Ribbon.ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Default || Ribbon.ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Above) && Ribbon.ViewInfo.Form != null) {
				row.Insert(0, new NavigationObjectApplicationButton(Ribbon, Ribbon.ViewInfo.ApplicationButton));	
			}
			return row;
		}
		protected virtual NavigationObjectRow CreatePagesRow() {
			NavigationObjectRow row = new NavigationObjectRow();
			if(Ribbon.ViewInfo.IsAllowApplicationButton)
				row.Add(new NavigationObjectApplicationButton(Ribbon, Ribbon.ViewInfo.ApplicationButton));	
			if(Ribbon.ViewInfo.Header.SelectedPageInfo != null) row.Add(new NavigationObjectPage(Ribbon, Ribbon.ViewInfo.Header.SelectedPageInfo));
			return row;
		}
		protected virtual NavigationObjectRow CreateItemsFirstRow() {
			NavigationObjectRow row = new NavigationObjectRow();
			RibbonPageContentEnumerator pageContentEnum = new RibbonPageContentEnumerator(Ribbon.ViewInfo.Panel, 0);
			for(; !pageContentEnum.End; pageContentEnum.Next()) {
				row.Add(new NavigationObjectRibbonPageGroupItem(Ribbon, pageContentEnum.CurrentItem));
			}
			return row;
		}
		protected virtual NavigationObjectRow CreateItemsSecondRow() {
			NavigationObjectRow row = new NavigationObjectRow();
			RibbonPageContentEnumerator pageContentEnum = new RibbonPageContentEnumerator(Ribbon.ViewInfo.Panel, 1);
			for(; !pageContentEnum.End; pageContentEnum.Next()) {
				row.Add(new NavigationObjectRibbonPageGroupItem(Ribbon, pageContentEnum.CurrentItem));
			}
			return row;
		}
		protected virtual NavigationObjectRow CreateItemsThirdRow() {
			NavigationObjectRow row = new NavigationObjectRow();
			RibbonPageContentEnumerator pageContentEnum = new RibbonPageContentEnumerator(Ribbon.ViewInfo.Panel, 2);
			for(; !pageContentEnum.End; pageContentEnum.Next()) {
				row.Add(new NavigationObjectRibbonPageGroupItem(Ribbon, pageContentEnum.CurrentItem));
			}
			return row;
		}
		protected virtual NavigationObjectRow CreateGroupButtonsRow() {
			NavigationObjectRow row = new NavigationObjectRow();
			for(int i = 0; i < Ribbon.ViewInfo.Panel.Groups.Count; i++) {
				if(Ribbon.ViewInfo.Panel.Groups[i].ShowCaptionButton) row.Add(new NavigationObjectPageGroupButton(Ribbon, Ribbon.ViewInfo.Panel.Groups[i]));
			}
			return row;
		}
	}
	public class RibbonLinksNavigation : BaseRibbonNavigation {
		public RibbonLinksNavigation(RibbonControl ribbon) : base(ribbon) { }
		public override bool IsNeededKey(KeyEventArgs e, BarItemLink activeLink) { 
			if(activeLink != null && activeLink.IsNeededKey(e)) return true;
			if(Manager.ActiveEditItemLink != null && IsNavigationKey(e.KeyData))
				return true;
			if(e.KeyData == Keys.Escape && !Ribbon.KeyTipManager.Show) return true;
			if(Ribbon.KeyTipManager.Show && !IsNavigationKey(e.KeyData)) return false;
			if(IsNavigationActive) return true;
			return false;
		}
		protected RibbonViewInfo ViewInfo { get { return Ribbon.ViewInfo; } }
		protected override RibbonKeyTipManager KeyTipManager {
			get { return Ribbon.KeyTipManager; }
		}
#if DXWhidbey
		protected virtual void UpdateAccessibleRibbonPage(NavigationObjectPage obj) {
			Ribbon.AccessibleNotifyClients(AccessibleEvents.Selection, RibbonControl.AccessibleObjectRibbonPageList, Ribbon.Pages.IndexOf(obj.Page));
			Ribbon.AccessibleNotifyClients(AccessibleEvents.Focus, RibbonControl.AccessibleObjectRibbonPageList, Ribbon.Pages.IndexOf(obj.Page));
		}
		protected virtual void UpdateAccessibleRibbonApplicationButton(NavigationObjectApplicationButton obj) {
			Ribbon.AccessibleNotifyClients(AccessibleEvents.Focus, RibbonControl.AccessibleObjectRibbonApplicationButton, -1);
		}
		protected virtual void UpdateAccessibleToolbarItem(NavigationObjectRibbonToolbarItem obj) {
			int childIndex = 0;
			if(obj.ItemInfo.Item == Ribbon.Toolbar.CustomizeItemLink || obj.ItemInfo.Item == Ribbon.Toolbar.DropDownItemLink) childIndex = Ribbon.AccessibleRibbon.Children[1].ChildCount - 1;
			else childIndex = Ribbon.Toolbar.ItemLinks.IndexOf(obj.ItemInfo.Item as BarItemLink);
			Ribbon.AccessibleNotifyClients(AccessibleEvents.Focus, RibbonControl.AccessibleObjectRibbonToolbarItem, childIndex);
		}
		protected internal virtual void UpdateAccessiblePageGroupButton(NavigationObjectPageGroupButton obj) {
			int objectIndex = Ribbon.SelectedPage.Groups.IndexOf(obj.PageGroup);
			if(objectIndex == -1)
				objectIndex = Ribbon.SelectedPage.Groups.Count + Ribbon.SelectedPage.MergedGroups.IndexOf(obj.PageGroup);
			Ribbon.AccessibleNotifyClients(AccessibleEvents.Focus, objectIndex + RibbonControl.AccessibleGroupsBeginIndex, Ribbon.AccessibleRibbon.Children[3].Children[0].Children[objectIndex].ChildCount - 2);
		}
		protected virtual RibbonPageGroupViewInfo GetActivePageGroupInfo(NavigationObjectRibbonPageGroupItem obj) {
			RibbonPageGroupViewInfo pageGroupInfo = obj.ItemInfo.Owner as RibbonPageGroupViewInfo;
			RibbonButtonGroupItemViewInfo buttonGroup = obj.ItemInfo.Owner as RibbonButtonGroupItemViewInfo;
			if(buttonGroup != null) pageGroupInfo = buttonGroup.Owner as RibbonPageGroupViewInfo;
			return pageGroupInfo;
		}
		protected internal virtual void UpdateAccessibleRibbonPageGroupItem(NavigationObjectRibbonPageGroupItem obj) {
			AccessibleSelectedPageContent pageContent = Ribbon.AccessibleRibbon.Children[3].Children[0] as AccessibleSelectedPageContent;
			RibbonPageGroupViewInfo pageGroupInfo = GetActivePageGroupInfo(obj);
			int objIndex = Ribbon.SelectedPage.Groups.IndexOf(pageGroupInfo.PageGroup);
			if(objIndex == -1)
				objIndex = Ribbon.SelectedPage.MergedGroups.IndexOf(pageGroupInfo.PageGroup) + Ribbon.SelectedPage.Groups.Count;
			AccessiblePageGroup pageGroup = Ribbon.AccessibleRibbon.Children[3].Children[0].Children[objIndex] as AccessiblePageGroup;
			RibbonGalleryBarItemLink galleryLink = obj.ItemLink as RibbonGalleryBarItemLink;
			if(galleryLink != null)
				Ribbon.AccessibleNotifyClients(AccessibleEvents.Focus, objIndex * 1000 + RibbonControl.AccessibleGalleryItemsBeginIndex + pageGroup.GetIndexByLink(obj.ItemLink), -1);
			else 
			Ribbon.AccessibleNotifyClients(AccessibleEvents.Focus, objIndex + RibbonControl.AccessibleItemsBeginIndex, pageGroup.GetIndexByLink(obj.ItemLink));
		}
		protected virtual void UpdateAccessibleRibbon(NavigationObject obj) {
			if(obj is NavigationObjectPage)
				UpdateAccessibleRibbonPage(obj as NavigationObjectPage);
			else if(obj is NavigationObjectApplicationButton) {
				UpdateAccessibleRibbonApplicationButton(obj as NavigationObjectApplicationButton);
			}
			else if(obj is NavigationObjectRibbonToolbarItem) {
				UpdateAccessibleToolbarItem(obj as NavigationObjectRibbonToolbarItem);
			}
			else if(obj is NavigationObjectPageGroupButton) {
				UpdateAccessiblePageGroupButton(obj as NavigationObjectPageGroupButton);
			}
			else if(obj is NavigationObjectRibbonPageGroupItem) {
				UpdateAccessibleRibbonPageGroupItem(obj as NavigationObjectRibbonPageGroupItem);
			}
		}
#endif
		protected NavigationObject ActiveObject {
			get { return ViewInfo.KeyboardActiveObject; }
			set {
				ViewInfo.KeyboardActiveObject = value;
				UpdateAccessibleRibbon(value);
			}
		}
		protected bool IsNavigationActive { get { return Ribbon.IsKeyboardActive; } }
		protected override void Escape() {
			if(IsNavigationActive) {
				Ribbon.DeactivateKeyboardNavigation();
				return;
			}
			base.Escape();
		}
		public override bool ProcessKeyDown(KeyEventArgs e, BarItemLink activeLink) {
			if (!IsNavigationActive) {
				if(Manager.ActiveEditItemLink != null && IsNavigationKey(e.KeyData)) {
					Ribbon.ActivateKeyboardNavigationForEditors();
					int navObjectIndex = Ribbon.NavigatableObjectList.FindNavObjectByLink(activeLink);
					if(navObjectIndex < 0) {
						if(base.ProcessKeyDown(e, activeLink))
							return true;
					}
					else {
						ViewInfo.KeyboardActiveObject = Ribbon.NavigatableObjectList[navObjectIndex];
						if(Manager == null)
							return false;
					}
				}
				else if (base.ProcessKeyDown(e, activeLink))
					return true;
			}
			if(activeLink != null) {
				if(e.KeyData != Keys.Down || activeLink is BarEditItemLink) {
					bool res = base.ProcessKeyDown(e, activeLink);
					if(res) {
						Ribbon.KeyTipManager.HideKeyTips();
						return true;
					}
				}
			}
			if(e.KeyData == Keys.Escape) {
				if(Ribbon.KeyTipManager.Show) Ribbon.KeyTipManager.CancelKeyTip();
				if(!Ribbon.KeyTipManager.Show) Escape();
				return true;
			}
			Keys shiftTab = Keys.Tab | Keys.Shift;
			if(IsNavigationKey(e.KeyData)) {
				Ribbon.KeyTipManager.HideKeyTips();
				if(e.KeyData == Keys.Tab) return OnTabPress(e, true);
				if(e.KeyData == shiftTab) return OnTabPress(e, false);
				if(e.KeyData == Keys.Up || e.KeyData == Keys.Down) return OnVertMove(e, e.KeyData == Keys.Up ? -1 : 1);
				if(e.KeyData == Keys.Left || e.KeyData == Keys.Right) return OnHorzMove(e, e.KeyCode == Keys.Left ? -1 : 1);
				if((e.KeyData == Keys.Enter || e.KeyData == Keys.Space) && ActiveObject != null) ActiveObject.Click();
			}
			return true;
		}
		protected virtual bool OnTabPress(KeyEventArgs e, bool forward) {
			NavigationObject obj = Ribbon.NavigatableObjectList.Move(CurrentObject, forward);
			if(obj == null)
				return false;
			ActiveObject = obj;
			return true;
		}
		protected override bool IsNavigationKey(Keys key) {
			Keys shiftTab = Keys.Tab | Keys.Shift;
			return key == Keys.Space || key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down || key == Keys.Enter || key == Keys.Tab || key == shiftTab;
		}
		protected NavigationObject CurrentObject {
			get {
				Point pos = CurrentObjectPosition;
				if(pos.X < 0) return null;
				return Ribbon.NavigatableObjects[pos.Y][pos.X];
			}
		}
		protected Point CurrentObjectPosition {
			get {
				if(Ribbon.NavigatableObjects == null) return new Point(-1, -1);
				return Ribbon.NavigatableObjects.FindNavObject(ViewInfo.KeyboardActiveObject);
			}
		}
		protected bool OnVertMove(KeyEventArgs e, int delta) {
			NavigationObject prevObject = CurrentObject;
			NavigationObject newObject = Ribbon.NavigatableObjects.MoveVert(CurrentObject, delta);
			if(prevObject != null && !prevObject.CanMoveTo(newObject)) return true;
			ActiveObject = newObject;
			return true;
		}
		protected bool OnHorzMove(KeyEventArgs e, int delta) {
			NavigationObject obj = CurrentObject;
			if(obj is NavigationObjectPage) {
				OnPageHorzMove(e, delta);
				return true;
			}
			else if(obj is NavigationObjectApplicationButton) {
				OnApplicationButtonHorzMove(e, delta);
				return true;
			}
			NavigationObject prevObject = CurrentObject;
			ActiveObject = Ribbon.NavigatableObjects.MoveHorz(CurrentObject, delta);
			if(prevObject == CurrentObject) {
				MoveHorizontalCycle(delta);
			}
			return true;
		}
		const int MaxTryCount = 50;
		private void MoveHorizontalCycle(int delta) {
			Point pos = CurrentObjectPosition;
			if(pos == new Point(-1, -1)) pos = Point.Empty;
			NavigationObjectRow row = Ribbon.NavigatableObjects[pos.Y];
			int currentIndex = row.IndexOf(CurrentObject);
			int index = currentIndex;
			int tryCount = 0;
			while(tryCount < MaxTryCount) {
				index += delta;
				if(index < 0) index = row.Count - 1;
				if(index >= row.Count) index = 0;
				if(index == currentIndex)
					break;
				if(row[index].Enabled) {
					ActiveObject = row[index];
					break;
				}
				tryCount++;
			}
		}
		protected void OnApplicationButtonPageRowHorzMove(KeyEventArgs e, int delta) {
			ArrayList pages = Ribbon.Pages.VisiblePages;
			if(pages.Count == 0) return;
			if(delta > 0) SelectPage(pages[0] as RibbonPage);
			else SelectPage(pages[pages.Count - 1] as RibbonPage);
		}
		protected void OnApplicationButtonToolbarRowHorzMove(KeyEventArgs e, int delta) { 
			NavigationObjectRow row = Ribbon.NavigatableObjects[0];
			if(row.Count <= 1)
				row = Ribbon.NavigatableObjects[1];
			if(row.Count <= 1)
				return;
			if(delta > 0) ActiveObject = row[1];
			else ActiveObject = row[row.Count - 1];
		}
		protected void OnApplicationButtonHorzMove(KeyEventArgs e, int delta) {
			Point pos = CurrentObjectPosition;
			NavigationObjectRow row = Ribbon.NavigatableObjects[pos.Y];
			if(row.Count > 0 && row[0] is NavigationObjectPage) OnApplicationButtonPageRowHorzMove(e, delta);
			else OnApplicationButtonToolbarRowHorzMove(e, delta);
		}   
		protected void OnPageHorzMove(KeyEventArgs e, int delta) {
			ArrayList pages = Ribbon.Pages.VisiblePages;
			if(pages.Count == 0) return;
			int current = pages.IndexOf(Ribbon.SelectedPage) + delta;
			if(current >= pages.Count || current < 0) {
				Point pos = CurrentObjectPosition;
				NavigationObjectRow row = Ribbon.NavigatableObjects[pos.Y];
				ActiveObject = row[0];
				return;
			}
			SelectPage(pages[current] as RibbonPage);
		}
		protected void SelectPage(RibbonPage page) {
			if(page == null) return;
			SelectionInfo.KeyboardHighlightedLink = null;
			RibbonPage prev = Ribbon.SelectedPage;
			Ribbon.SelectedPage = page;
			if(prev != page) Ribbon.OnSelectedPageChangedCore();
		}
	}
	public class RibbonStatusBarNavigation : BaseRibbonNavigation {
		public RibbonStatusBarNavigation(RibbonControl ribbon) : base(ribbon) { }
	}
	public class RibbonMiniToolbarNavigation : BaseRibbonNavigation {
		RibbonMiniToolbar toolbar;
		public RibbonMiniToolbarNavigation(RibbonMiniToolbar toolbar) : base(toolbar.Ribbon) {
			this.toolbar = toolbar;
		}
		public RibbonMiniToolbar Toolbar { get { return toolbar; } }
	}
	public class BaseRibbonNavigation : LinksNavigation {
		RibbonControl ribbon;
		public BaseRibbonNavigation(RibbonControl ribbon) {
			this.ribbon = ribbon;
		}
		protected RibbonControl Ribbon { get { return ribbon; } }
		public override BarManager Manager { get { return ribbon.Manager; } }
		public override bool IsNeededKey(KeyEventArgs e, BarItemLink activeLink) {
			if(activeLink != null && activeLink.IsNeededKey(e)) return true;
			if(e.KeyData == Keys.Escape) return true;
			return false;
		}
		protected override void Escape() {
			Manager.SelectionInfo.Clear();
		}
	}
}
