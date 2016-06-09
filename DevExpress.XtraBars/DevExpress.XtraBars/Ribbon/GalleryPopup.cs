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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Gesture;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Accessible;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.XtraBars.Ribbon {
	[DXToolboxItem(true), ToolboxTabName(AssemblyInfo.DXTabNavigation), Designer("DevExpress.XtraBars.Ribbon.Design.RibbonGalleryDropDownDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)), ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "GalleryDropDown")]
	public class GalleryDropDown : PopupMenu {
		InDropDownGallery gallery;
		DropDownGalleryControlHandler galleryHandler;
		bool resizeForm;
		InRibbonGallery ownerGallery;
		RibbonGalleryBarItemLink ownerGalleryLink;
		public GalleryDropDown(IContainer container)
			: this() {
			container.Add(this);
		}
		public GalleryDropDown() {
			this.gallery = new InDropDownGallery(this);
			this.galleryHandler = new DropDownGalleryControlHandler(this);
			this.resizeForm = false;
		}
		[Browsable(false)]
		public override BarManager Manager {
			get { return base.Manager; }
			set { base.Manager = value; }
		}
		internal int MinimumWidth { get { return Gallery.MinimumWidth; } set { Gallery.MinimumWidth = value; } }
		protected internal InRibbonGallery OwnerGallery { get { return ownerGallery; } set { ownerGallery = value; } }
		protected internal bool ClonedFromInRibbonGallery { get { return OwnerGallery != null && OwnerGallery.GalleryDropDown == null; } }
		protected internal RibbonGalleryBarItemLink OwnerGalleryLink { get { return ownerGalleryLink; } set { ownerGalleryLink = value; } }
		protected override PopupMenuBarControl CreatePopupControl(BarManager manager) { return new GalleryDropDownBarControl(manager, this); }
		protected override SubMenuControlForm CreateForm(BarManager manager, PopupMenuBarControl pc) { return new SubMenuControlForm(manager, pc, FormBehavior.Gallery); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("GalleryDropDownGallery"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Behavior")]
		public virtual InDropDownGallery Gallery { get { return gallery; } }
		protected internal virtual DropDownGalleryControlHandler GalleryHandler { get { return galleryHandler; } }
		protected internal virtual bool ResizeForm { get { return resizeForm; } }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing && Gallery != null) {
				Gallery.Dispose();
			}
		}
		protected override void ShowDXDropDownControlCore(Control control, Point pos) {
			MinWidth = control.Width;
			base.ShowDXDropDownControlCore(control, pos);
		}
		public override void ShowPopup(BarManager manager, Point p) {
			Gallery.FirstMove = true;
			base.ShowPopup(manager, p);
			PressGalleryToolbarLink();
			Gallery.MakeVisibleSelectedItem();
			Gallery.RefreshGallery();
			if(Gallery.BarControl != null && Gallery.BarControl.IsHandleCreated) {
				Gallery.BarControl.Invalidate();
			}
			SubscribeDesignModeEvents();
		}
		internal virtual void SubscribeDesignModeEvents() {
			if(Gallery.DesignModeCore) {
				(Manager.Helper.DragManager as RibbonLinkDragManager).SelectedDropDownGallery = this;
				if(OwnerGallery == null)
					ItemLinks.CollectionChanged += new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
			}
		}
		protected internal SubMenuControlForm Form { 
			get { 
				if(Gallery == null || Gallery.BarControl == null)return null;
				return Gallery.BarControl.Form as SubMenuControlForm;
			} 
		}
		protected override void OnItemLinksCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(Manager == null || !Manager.IsCustomizing) return;
			if(e.Action == CollectionChangeAction.Refresh) return;
			if(Gallery.BarControl == null) return;
			Gallery.BarControl.OnLinkCountChanged(e.Element as BarItemLink, e.Action == CollectionChangeAction.Add); 
		}
		protected internal virtual void ResizeMenu(Rectangle bounds) {
			if(Gallery.BarControl == null) return;
			if(this.resizeForm) return;
			this.resizeForm = true;
			Form.Bounds = bounds;
			Application.DoEvents(); 
			this.resizeForm = false;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("GalleryDropDownCanShowPopup")]
#endif
		public override bool CanShowPopup { get { return base.CanShowPopup && Ribbon != null; } }
		public override void BeginUpdate() {
			Gallery.BeginUpdate();
			base.BeginUpdate();
		}
		public override void EndUpdate() {
			Gallery.EndUpdate();
			base.EndUpdate();
		}
		protected internal override LocationInfo CalcLocationInfo(Point p) {
			LocationInfo info = base.CalcLocationInfo(p);
			RibbonGalleryBarItemLink link = Activator as RibbonGalleryBarItemLink;
			if(link != null) {
				if(link.ToolbarLink != null) {
					p = link.ToolbarLink.Ribbon.PointToScreen(new Point(link.ToolbarLink.Bounds.X, link.ToolbarLink.Bounds.Bottom));
					return new LocationInfo(p, new Point(p.X, p.Y - link.ToolbarLink.Bounds.Height), true, false);
				}
				if(Ribbon != null && Ribbon.GetRibbonStyle() == RibbonControlStyle.MacOffice && link.ToolbarLink == null)
					p = new Point(p.X + (link.Bounds.Width - Form.Width) / 2, p.Y + link.Bounds.Height + 10);
				info = new LocationInfo(p, p, true, false);
			}
			bool isInMenu = (Activator as BarItemLink) != null && (Activator as BarItemLink).IsLinkInMenu;
			return new GalleryLocationInfo(info.Location, info.AltLocation, info.VerticalOpen, info.OpenXBack, isInMenu);
		}
		protected override void OnCloseUp(CustomPopupBarControl prevControl) {
			if(IsDesignMode && OwnerGallery == null)
				ItemLinks.CollectionChanged -= new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
			Gallery.HideImageForms(null, false);
			Gallery.ClearAnimatedItems();
			UnpressGalleryToolbarLink();
			base.OnCloseUp(prevControl);
		}
		public override void HidePopup() {
			if(Visible)
				UnpressGalleryToolbarLink();
			base.HidePopup();
		}
		internal virtual void UnpressGalleryToolbarLink() {
		}
		internal virtual void PressGalleryToolbarLink() {
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemClickEventHandler GalleryItemClick {
			add { Gallery.ItemClick += value; }
			remove { Gallery.ItemClick -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemClickEventHandler GalleryItemDoubleClick {
			add { Gallery.ItemDoubleClick += value; }
			remove { Gallery.ItemDoubleClick -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler GalleryItemCheckedChanged {
			add { Gallery.ItemCheckedChanged += value; }
			remove { Gallery.ItemCheckedChanged -= value; }
		}
		[System.ComponentModel.Category("Events")]
		public event GalleryItemCustomDrawEventHandler GalleryCustomDrawItemImage {
			add { Gallery.CustomDrawItemImage += value; }
			remove { Gallery.CustomDrawItemImage -= value; }
		}
		[System.ComponentModel.Category("Events")]
		public event GalleryItemCustomDrawEventHandler GalleryCustomDrawItemText {
			add { Gallery.CustomDrawItemText += value; }
			remove { Gallery.CustomDrawItemText -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryFilterMenuItemClickEventHandler GalleryFilterMenuItemClick {
			add { Gallery.FilterMenuItemClick += value; }
			remove { Gallery.FilterMenuItemClick -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler GalleryItemHover {
			add { Gallery.GalleryItemHover += value; }
			remove { Gallery.GalleryItemHover -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GalleryItemEventHandler GalleryItemLeave {
			add { Gallery.GalleryItemLeave += value; }
			remove { Gallery.GalleryItemLeave -= value; }
		}
		[System.ComponentModel.Category("Action")]
		public event GallerySelectionEventHandler MarqueeSelectionCompleted {
			add { Gallery.MarqueeSelectionCompleted += value; }
			remove { Gallery.MarqueeSelectionCompleted -= value; }
		}
		[System.ComponentModel.Category("Layout")]
		public event GalleryFilterMenuEventHandler GalleryFilterMenuPopup {
			add { Gallery.FilterMenuPopup += value; }
			remove { Gallery.FilterMenuPopup -= value; }
		}
		internal void FireChanged() {
			if(!DesignMode)
				return;
			IComponentChangeService componentChangeService = Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null)
				componentChangeService.OnComponentChanged(this, null, null, null);
		}
	}
}
namespace DevExpress.XtraBars.Controls {
	public class GalleryDropDownBarControl : PopupMenuBarControl, IToolTipControlClient, IGestureClient {
		internal GalleryDropDownBarControl(BarManager barManager, GalleryDropDown menu)
			: base(barManager, menu) {
			SetStyle(ControlStyles.ResizeRedraw | ControlConstants.DoubleBuffer | ControlStyles.UserPaint, true);
			GalleryMenu.GalleryHandler.BarControl = this;
			Gallery.BarControl = this;
		}
		GalleyDropDownBarControlPainter skinPainter = null;
		internal virtual GalleyDropDownBarControlPainter CreateGallerySkinPainter() {
			return new GalleyDropDownBarControlPainter(Manager.PaintStyle);
		}
		internal GalleyDropDownBarControlPainter GallerySkinPainter { 
			get {
				if(skinPainter == null) skinPainter = CreateGallerySkinPainter();
				return skinPainter; 
			} 
		}
		protected override LinksNavigation CreateLinksNavigator() {
			return new DropDownGalleryLinksNavigation(this);
		}
		protected override bool AllowAnimation { get { return true; } }
		protected override AnimationType GetAnimationType() { return AnimationType.Fade; }
		protected internal override bool AllowOpenPopup { get { return true; } }
		public GalleryDropDown GalleryMenu { get { return (GalleryDropDown)Menu; } }
		public InDropDownGallery Gallery { get { return GalleryMenu.Gallery; } }
		public new GalleryDropDownControlViewInfo ViewInfo { get { return base.ViewInfo as GalleryDropDownControlViewInfo; } }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new GalleryDropDownBarControlAccessible(this);
		}
		bool IToolTipControlClient.ShowToolTips { get { return !DesignMode; } }
		protected virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo res = Gallery.GetToolTipInfo(point);
			if(res == null) return res;
			if(res.ToolTipPosition.X != -10000) {
				res.ToolTipPosition = Gallery.ToolTipPointToScreen(point);
				return res;
			}
			return res;
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			ToolTipControlInfo res = GetToolTipInfo(point);
			if(res != null) res.ToolTipType = ToolTipType.SuperTip;
			return res;
		}
		public static int AccessibleGalleryObjectId = 1000;
		public static int AccessibleGalleryFilterId = 1001;
		public static int AccessibleGroupBeginId = 1002;
#if DXWhidbey        
		protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
			int groupIndex = 0;
			if(objectId == AccessibleGalleryObjectId) return DXAccessible.Children[0].Accessible;
			else if(objectId == AccessibleGalleryFilterId) return DXAccessible.Children[0].Children[0].Accessible;
			else if(objectId >= AccessibleGroupBeginId && objectId - AccessibleGroupBeginId >= 0) {
				if(Gallery.AllowFilter) groupIndex = 1;
				return DXAccessible.Children[0].Children[groupIndex + objectId - AccessibleGroupBeginId].Accessible;
			}
			return base.GetAccessibilityObjectById(objectId);
		}
		protected internal void AccessibleNotifyClients(AccessibleEvents accEvent,int objectId, int childId) {
			AccessibilityNotifyClients(accEvent, objectId, childId);
		}
#endif
		protected override void Dispose(bool disposing) {
			Controls.Remove(Gallery.ScrollBar);
			base.Dispose(disposing);
		}
		protected internal void ResizeGallery() {
			LayoutChanged();
			if(FindForm() != null)
				FindForm().Update();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseDown(ee);
			if(ee.Handled) return;
			GalleryMenu.GalleryHandler.OnMouseDown(ee);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseMove(ee);
			if(ee.Handled) return;
			GalleryMenu.GalleryHandler.OnMouseMove(ee);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseUp(ee);
			GalleryMenu.GalleryHandler.OnMouseUp(ee);
		}
		protected override void OnMouseEnter(EventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			base.OnMouseEnter(ee);
			GalleryMenu.GalleryHandler.OnMouseEnter(ee);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(this, e);
			base.OnMouseWheel(ee);
			GalleryMenu.GalleryHandler.OnMouseWheel(ee);
			if(Gallery != null && Gallery.Ribbon != null && Gallery.Ribbon.ToolTipController != null)
				Gallery.Ribbon.ToolTipController.HideHint();
			else 
				ToolTipController.DefaultController.HideHint();
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			GalleryMenu.GalleryHandler.OnMouseLeave(new DXMouseEventArgs(MouseButtons.None, 0, -10000, -10000, 0));
		}
		protected virtual void CalcLocationInfo() {
			if(LocationInfo == null) return;
			if(LocationInfo != null && !GalleryMenu.ResizeForm) {
				LocationInfo.AboveItem = LocationInfo.ShowPoint.Y != LocationInfo.Location.Y ? true : false;
				LocationInfo.OriginXPosition = LocationInfo.ShowPoint.X;
				Gallery.SizerBelow = !LocationInfo.AboveItem;
				if(!Gallery.SizerBelow) UpdateViewInfo();
			}
		}
		protected override void UpdateFormHeight(int maxHeight) {
			base.UpdateFormHeight(maxHeight);
			CalcLocationInfo();
		}
		public override bool CanOpenAsChild(IPopup popup) {
			PopupMenu filterMenu = popup.PopupCreator as PopupMenu;
			if(filterMenu != null && filterMenu.MenuCreator != null && filterMenu.MenuCreator == Gallery.FilterMenu) return true;
			return base.CanOpenAsChild(popup);
		}
		protected internal virtual GalleryLocationInfo LocationInfo { get { return Form.LocationInfo as GalleryLocationInfo; } }
		#region DragEvents
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			Manager.Helper.DragManager.ItemOnQueryContinueDrag(e, this);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			Manager.Helper.DragManager.ItemOnGiveFeedback(e, this);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			Manager.Helper.DragManager.FireDoDragging = false;
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
		}
		protected override void OnDragLeave(EventArgs e) {
			Manager.Helper.DragManager.FireDoDragging = true;
		}
		protected override void OnDragDrop(DragEventArgs e) {
			Manager.Helper.DragManager.StopDragging(this, e.Effect, false);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
			Manager.Helper.DragManager.DoDragging(this, new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
		}
		#endregion
		protected internal virtual void OnLinkCountChanged(BarItemLink link, bool bAdd) {
			int prevLinksHeight = ViewInfo.LinksBounds.Height;
			LayoutChanged();
			Rectangle rect = Form.Bounds;
			rect.Height += ViewInfo.LinksBounds.Height - prevLinksHeight;
			GalleryMenu.ResizeMenu(rect);
		}
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		protected override void WndProc(ref Message m) {
			if(GestureHelper.WndProc(ref m))
				return;
			base.WndProc(ref m);
		}
		#region IGestureClient Members
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(ViewInfo.GalleryBounds.Contains(point))
				return new GestureAllowArgs[] { GestureAllowArgs.PanVertical };
			return new GestureAllowArgs[] { };
		}
		IntPtr IGestureClient.Handle {
			get { return IsHandleCreated ? Handle : IntPtr.Zero; }
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
		}
		int yOverPan;
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				yOverPan = 0;
				return;
			}
			if(delta.Y == 0) return;
			int prevTopY = Gallery.ScrollYPosition;
			Gallery.ScrollYPosition -= delta.Y;
			if(prevTopY == Gallery.ScrollYPosition) {
				yOverPan += delta.Y;
			}
			else { yOverPan = 0; }
			overPan.Y = yOverPan;
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
		}
		IntPtr IGestureClient.OverPanWindowHandle {
			get { return GestureHelper.FindOverpanWindow(this); }
		}
		Point IGestureClient.PointToClient(Point p) {
			return PointToClient(p);
		}
		#endregion
	}
	public class DropDownGalleryLinksNavigation : VerticalLinksNavigation {
		public DropDownGalleryLinksNavigation(GalleryDropDownBarControl linksControl)
			: base(linksControl) {
		}
		protected new GalleryDropDownBarControl LinksControl { get { return (GalleryDropDownBarControl)base.LinksControl; } }
		public InDropDownGallery Gallery { get { return LinksControl.Gallery; } }
		public StandaloneGalleryViewInfo GalleryInfo { get { return Gallery.ViewInfo; } }
		public virtual BarItemLink FirstLink { get { return ReallyVisibleLinks[0]; } }
		public virtual BarItemLink LastLink { get { return ReallyVisibleLinks[ReallyVisibleLinks.Count - 1]; } }
		public virtual BarItemLink HighlightedLink { get { return LinksControl.GalleryMenu.Manager.SelectionInfo.HighlightedLink; } set { LinksControl.GalleryMenu.Manager.SelectionInfo.HighlightedLink = value; } }
		public virtual bool IsFirstLinkHighlighted { get { return LinksControl.GalleryMenu.Manager.HighlightedLink == ReallyVisibleLinks[0]; } }
		public virtual bool IsLastLinkHighlighted { get { return LinksControl.GalleryMenu.Manager.HighlightedLink == ReallyVisibleLinks[ReallyVisibleLinks.Count - 1]; } }
		public virtual bool IsLinkHighlighted { get { return LinksControl.GalleryMenu.Manager.HighlightedLink != null; } }
		public virtual bool MoveVertical(BarLinkNavigation nav) {
			if(nav == BarLinkNavigation.Up) return Gallery.ViewInfo.MoveVertical(-1);
			else if(nav == BarLinkNavigation.Down) return Gallery.ViewInfo.MoveVertical(+1);
			return false;
		}
		public virtual bool MoveHorizontal(BarLinkNavigation nav) {
			if(nav == BarLinkNavigation.Left) return GalleryInfo.MoveHorizontal(-1);
			else if(nav == BarLinkNavigation.Right) return GalleryInfo.MoveHorizontal(+1);
			return false;
		}
		protected virtual void OnFailGalleryNavigation(BarLinkNavigation nav, bool forward, bool back, int keybIndex) {
			OnFailGalleryNavigation(nav, forward, back, keybIndex, 1);
		}
		protected virtual void OnFailGalleryNavigation(BarLinkNavigation nav, bool forward, bool back, int keybIndex, int delta) {
			if(LinksControl.GalleryMenu.ItemLinks.Count != 0) {
				if(forward && IsLastLinkHighlighted) {
					HighlightedLink = null;
					GalleryInfo.SetKeyboardSelectedItem(GalleryInfo.GetFirstItem());
				}
				else if(back && IsFirstLinkHighlighted) {
					HighlightedLink = null;
					GalleryInfo.SetKeyboardSelectedItem(GalleryInfo.GetLastItem());
				}
				else {
					if(HighlightedLink != null) base.MoveLinkSelectionVertical(nav);
					else if(back) HighlightedLink = LastLink;
					else if(forward) HighlightedLink = FirstLink;
				}
			}
			else {
				if(forward) {
					GalleryInfo.SetKeyboardSelectedItem(delta == 1 ? GalleryInfo.GetFirstItemInPosition(keybIndex) : GalleryInfo.GetLastItemInPosition(keybIndex));
				}
				else GalleryInfo.SetKeyboardSelectedItem(delta == 1 ? GalleryInfo.GetLastItemInPosition(keybIndex) : GalleryInfo.GetFirstItemInPosition(keybIndex));
			}	
		}
		protected override void MoveLinkSelectionVertical(BarLinkNavigation nav) {
			int keybIndex = GalleryInfo.KeyboardSelectedItemIndex;
			if(!IsLinkHighlighted && GalleryInfo.KeyboardSelectedItem == null) {
				MoveVertical(nav);
				return;
			}
			if(!MoveVertical(nav))
				OnFailGalleryNavigation(nav, nav == BarLinkNavigation.Down, nav == BarLinkNavigation.Up, keybIndex);
		}
		protected override void MoveLinkSelectionHorizontal(BarLinkNavigation nav) {
			if(!IsLinkHighlighted && GalleryInfo.KeyboardSelectedItem == null) {
				MoveHorizontal(nav);
				return;
			}
			if(!MoveHorizontal(nav))
				OnFailGalleryNavigation(nav, nav == BarLinkNavigation.Right, nav == BarLinkNavigation.Left, 0);
		}
		public override bool IsNeededKey(KeyEventArgs e, BarItemLink activeLink) {
			if(e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.Home || e.KeyCode == Keys.End) return true;
			return base.IsNeededKey(e, activeLink);
		}
		public override bool ProcessKeyDown(KeyEventArgs e, BarItemLink activeLink) {
			if(e.KeyCode == Keys.Enter && GalleryInfo.KeyboardSelectedItem != null) {
				Gallery.OnItemClick(null, Gallery, GalleryInfo.KeyboardSelectedItem.Item);
				if(Gallery.AutoHideGallery) Gallery.GalleryDropDown.HidePopup();
				return true;
			}
			else if(e.KeyCode == Keys.PageDown) {
				if(!GalleryInfo.MoveVertical(GalleryInfo.RealRowCount))
					OnFailGalleryNavigation(BarLinkNavigation.Down, true, false, GalleryInfo.KeyboardSelectedItemIndex, GalleryInfo.RealRowCount);
				return true;
			}
			else if(e.KeyCode == Keys.PageUp) {
				if(!GalleryInfo.MoveVertical(-GalleryInfo.RealRowCount))
					OnFailGalleryNavigation(BarLinkNavigation.Down, false, true, GalleryInfo.KeyboardSelectedItemIndex, GalleryInfo.RealRowCount);
				return true;
			}
			else if(e.KeyCode == Keys.End && HighlightedLink == null) {
				GalleryInfo.SetKeyboardSelectedItem(GalleryInfo.GetLastItem());
				return true;
			}
			else if(e.KeyCode == Keys.Home && HighlightedLink == null) {
				GalleryInfo.SetKeyboardSelectedItem(GalleryInfo.GetFirstItem());
				return true;
			}
			return base.ProcessKeyDown(e, activeLink);
		}
	}
}
namespace DevExpress.XtraBars.ViewInfo {
	public class GalleryDropDownControlViewInfo : PopupMenuBarControlViewInfo {
		const int GalleryToMenuIndent = 1;
		const int SizerTopIndent = 1;
		Rectangle galleryBounds, menuSeparatorBounds, sizerBounds;
		public new GalleryDropDownBarControl BarControl { get { return base.BarControl as GalleryDropDownBarControl; } }
		public GalleryDropDownControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar)
			: base(manager, parameters, bar) {
		}
		public override void Clear() {
			base.Clear();
			this.sizerBounds = Rectangle.Empty;
			this.galleryBounds = Rectangle.Empty;
			this.menuSeparatorBounds = Rectangle.Empty;
		}
		protected internal override bool ShowMenuCaption { get { return false; } }
		protected override bool IsAllowEmptyItemLink { get { return false; } }
		public Rectangle MenuSeparatorBounds { get { return menuSeparatorBounds; } set { menuSeparatorBounds = value; } }
		public Rectangle GalleryBounds { get { return galleryBounds; } set { galleryBounds = value; } }
		public Rectangle SizerBounds { get { return sizerBounds; } set { sizerBounds = value; } }
		InDropDownGalleryViewInfo GalleryInfo { get { return BarControl.Gallery.ViewInfo; } }
		protected override Size BarMinSize { get { return new Size(16, 0); } }
		public override Size CalcBarSize(Graphics g, object sourceObject, int width, int maxHeight) {
			Size res = base.CalcBarSize(g, sourceObject, width, maxHeight);
			Size size = CalcGallerySize();
			int sizerHeight = CalcSizerHeight();
			if(sizerHeight != 0)
				res.Height += SizerTopIndent;
			res.Height += sizerHeight;
			res.Width = Math.Max(size.Width, res.Width);
			res.Height += size.Height;
			if(BarControl.VisibleLinks.Count > 0) {
				res.Height += CalcMenuSeparatorHeight() + GalleryToMenuIndent;
			}
			return res;
		}
		public override void CalcMaxWidthes() {
			base.CalcMaxWidthes();
			if(Bounds.Width > 0)
				MaxLinkWidth = Bounds.Width;
		}
		protected internal virtual Size CalcGalleryAvailSize(Rectangle bounds) {
			int sizerHeight = CalcSizerHeight();
			int linksHeight = base.CalcLinksBounds(bounds).Height;
			return new Size(CalcGallerySize().Width, bounds.Height - linksHeight - sizerHeight);
		}
		protected override Rectangle CalcLinksBounds(Rectangle bounds) {
			this.menuSeparatorBounds = this.sizerBounds = this.galleryBounds = Rectangle.Empty;
			Size gallerySize = CalcGallerySize();
			if(bounds.IsEmpty || gallerySize.IsEmpty) {
				return base.CalcLinksBounds(bounds);
			}
			Size linksSize = base.CalcBarSize(GInfo.Graphics, null, bounds.Width, bounds.Height);
			int sizerHeight = CalcSizerHeight();
			if(sizerHeight > 0) {
				this.sizerBounds = bounds;
				this.sizerBounds.Height = sizerHeight;
				if(BarControl.Gallery.SizerBelow) {
					this.sizerBounds.Y = bounds.Bottom - sizerBounds.Height;
				}
				else {
					int delta = sizerBounds.Height + SizerTopIndent;
					bounds.Y += delta;
					if(ShowNavigationHeader)
						NavigationHeaderBounds = new Rectangle(NavigationHeaderBounds.X, NavigationHeaderBounds.Y + delta, NavigationHeaderBounds.Width, NavigationHeaderBounds.Height);
				}
				bounds.Height -= sizerBounds.Height + SizerTopIndent;
			}
			if(ShowNavigationHeader) {
				bounds.Y += NavigationHeaderBounds.Height;
			}
			if(linksSize.Height >= bounds.Height) return bounds;
			this.galleryBounds = bounds;
			this.galleryBounds.Height = bounds.Height - linksSize.Height;
			if(BarControl.VisibleLinks.Count > 0) {
				int sepHeight = CalcMenuSeparatorHeight();
				if(sepHeight > 0) {
					galleryBounds.Height -= sepHeight + GalleryToMenuIndent;
					this.menuSeparatorBounds = galleryBounds;
					this.menuSeparatorBounds.Y = galleryBounds.Bottom;
					this.menuSeparatorBounds.Height = sepHeight;
				}
			}
			bounds.Y = bounds.Bottom - linksSize.Height;
			bounds.Height = linksSize.Height;
			if(ShowNavigationHeader) {
				bounds.Height -= NavigationHeaderBounds.Height;
			}
			return bounds;
		}
		public override int MenuBarWidth { get { return 0; } }
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle rect) {
			base.CalcViewInfo(g, sourceObject, rect);
			CalcGallery(GalleryBounds);
			CalcSizerInfo(SizerBounds);
		}
		protected virtual void CalcGallery(Rectangle bounds) {
			GalleryInfo.Reset();
			GalleryInfo.CalcViewInfo(bounds);
		}
		protected virtual Size CalcGallerySize() {
			return GalleryInfo.CalcGalleryBestSize();
		}
		protected virtual int CalcSizerHeight() {
			if(!GalleryInfo.IsGalleryResizing) return 0;
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(GalleryInfo.Gallery.Provider)[RibbonSkins.SkinPopupGallerySizerPanel]);
			return SkinElementPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, info).Height;
		}
		protected virtual int CalcMenuSeparatorHeight() {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(GalleryInfo.Gallery.Provider)[RibbonSkins.SkinPopupGalleryMenuSeparator]);
			return SkinElementPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, info).Height;
		}
		protected virtual void CalcSizerInfo(Rectangle bounds) {
			GalleryInfo.CalcSizerInfo(bounds);
		}
	}
}
namespace DevExpress.XtraBars.Painters {
	public class NonSkinGalleryDropDownBarControlPainter : BarSubMenuPainter {
		public NonSkinGalleryDropDownBarControlPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			base.Draw(e, info, sourceInfo);
			DrawSkinGallery(e, info, sourceInfo);
		}
		internal virtual void DrawSkinGallery(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			GalleryDropDownControlViewInfo viewInfo = info as GalleryDropDownControlViewInfo;
			if(viewInfo == null) return;
			GalleryDropDownBarControl barControl = viewInfo.BarControl as GalleryDropDownBarControl;
			if(barControl == null) return;
			barControl.GallerySkinPainter.DrawGallery(e, info, sourceInfo);
		}
	}
	public class GalleyDropDownBarControlPainter : SkinBarSubMenuPainter {
		public GalleyDropDownBarControlPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		public virtual void DrawGallery(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			GalleryDropDownControlViewInfo viewInfo = info as GalleryDropDownControlViewInfo;
			if(!viewInfo.GalleryBounds.IsEmpty && e.Cache.IsNeedDrawRect(viewInfo.GalleryBounds)) DrawGallery(e, viewInfo);
			if(!viewInfo.SizerBounds.IsEmpty && e.Cache.IsNeedDrawRect(viewInfo.SizerBounds)) DrawSizer(e, viewInfo);
			if(!viewInfo.MenuSeparatorBounds.IsEmpty) DrawMenuSeparator(e, viewInfo);
		}
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			base.Draw(e, info, sourceInfo);
			DrawGallery(e, info, sourceInfo);
		}
		protected virtual void DrawGallery(GraphicsInfoArgs e, GalleryDropDownControlViewInfo viewInfo) {
			Rectangle r = viewInfo.GalleryBounds;
			viewInfo.BarControl.Gallery.ViewInfo.CheckViewInfo();
			viewInfo.BarControl.Gallery.Painter.Draw(e.Cache, viewInfo.BarControl.Gallery.ViewInfo.CreateGalleryInfo(null));
		}
		protected virtual void DrawSizer(GraphicsInfoArgs e, GalleryDropDownControlViewInfo viewInfo) {
			Rectangle r = viewInfo.SizerBounds;
			viewInfo.BarControl.Gallery.Painter.DrawSizer(e.Cache, viewInfo.BarControl.Gallery.ViewInfo.CreateGalleryInfo(null));
		}
		protected virtual void DrawMenuSeparator(GraphicsInfoArgs e, GalleryDropDownControlViewInfo viewInfo) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.BarControl.Gallery.Provider)[RibbonSkins.SkinPopupGalleryMenuSeparator], viewInfo.MenuSeparatorBounds));
		}
	}
}
namespace DevExpress.XtraBars.Forms {
	public class GalleryLocationInfo : LocationInfo {
		bool galleryMenuLayout;
		bool aboveItem;
		bool isInMenu;
		int originXPosition;
		public GalleryLocationInfo(Point onePoint) : base(onePoint) { }
		public GalleryLocationInfo(Point location, Point altLocation, bool verticalOpen, bool openXBack, bool isInMenu)
			: base(location, altLocation, verticalOpen, openXBack) {
			this.galleryMenuLayout = false;
			this.originXPosition = 0;
			this.aboveItem = false;
			this.isInMenu = isInMenu;
		}
		protected internal virtual bool GalleryMenuLayout { get { return galleryMenuLayout; } set { galleryMenuLayout = value; } }
		protected internal virtual int OriginXPosition { get { return originXPosition; } set { originXPosition = value; } }
		protected internal virtual bool AboveItem { get { return aboveItem; } set { aboveItem = value; } }
		protected internal virtual bool IsInMenu { get { return isInMenu; } set { isInMenu = value; } }
		protected override int CalcY() {
			if(GalleryMenuLayout) return CalcGalleryMenuY();
			return base.CalcY();
		}
		protected override int CalcX() {
			if(GalleryMenuLayout) {
				return OriginXPosition;
			}
			int xPos = base.CalcX();
			int deltaRight = WorkingArea.Right - (xPos + WindowSize.Width);
			if(deltaRight < 0) xPos += deltaRight;
			return Math.Max(WorkingArea.Left, xPos);
		}
		protected internal virtual int CalcGalleryMenuY() {
			if(AboveItem) {
				if(IsInMenu) return WorkingArea.Bottom - WindowSize.Height;
				return Location.Y - WindowSize.Height;
			}
			return Location.Y;
		}
	}
	public class GalleryDropDownFormBehavior : SubMenuControlFormBehavior {
		GalleryDropDown galleryDropDown;
		public GalleryDropDownFormBehavior(GalleryDropDown galleryDropDown, ControlForm form, BarManager manager, Control control) : base(form, manager, control) {
			Form.SetStyleCore(ControlStyles.Selectable, false);
			this.galleryDropDown = galleryDropDown;
			Form.UpdateSizeT();
		}
		public override void OnVisibleChanged() {
			Form.OnVisibleChangedCore(EventArgs.Empty);
			if(!Form.Visible)
				return;
			Screen scr = Screen.FromControl(Form);
			Size formSize = Form.Size;
			if(scr != null) {
				formSize.Width = Math.Min(formSize.Width, scr.WorkingArea.Width);
				formSize.Height = Math.Min(formSize.Height, scr.WorkingArea.Height);
			}
			if(!Form.Size.Equals(formSize)) {
				GalleryDropDown.ResizeMenu(new Rectangle(Form.Location, formSize));
				Form.LayoutChanged();
			}
		}
		public GalleryDropDown GalleryDropDown { get { return galleryDropDown; } }
		public override bool AllowMouseActivate {
			get {
				return false;
			}
		}
		public override bool AllowInitialize {
			get {
				return false;
			}
		}
		public virtual IFormContainedControl IContainedControl { get { return ContainedControl as IFormContainedControl; } }
		public override Size CalcSizeByWidth(int width, int maxFormHeight) {
			if(IContainedControl != null && GalleryDropDown != null) {
				Size res = IContainedControl.CalcSize(width, maxFormHeight);
				Form.CalcFormSize(ref res, true);
				res.Width = Math.Max(res.Width, GalleryDropDown.MinimumWidth);
				return res;
			}
			return Size.Empty;
		}
		public override int MaxFormHeight {
			get {
				if(GalleryDropDown != null && GalleryDropDown.ResizeForm) return -1;
				return base.MaxFormHeight;
			}
		}
		protected internal GalleryLocationInfo LocationInfo { get { return Form.LocationInfo as GalleryLocationInfo; } }
		public override void UpdateLocationInfo() {
			base.UpdateLocationInfo();
			if(LocationInfo != null)
				LocationInfo.GalleryMenuLayout = GalleryDropDown.ResizeForm;
			else
				LocationInfo.GalleryMenuLayout = false;
		}
		public override void LayoutChangedCore() {
			Form.UpdateTitleBar();
			Form.UpdateViewInfo();
			Form.Shadows.Move(Form.ShadowOwnerBounds);
		}
		public override void OnPaint(PaintEventArgs e) {
			e.Graphics.ExcludeClip(Form.ViewInfo.ControlRect);
			base.OnPaint(e);
		}
		public override void OnPaintBackground(PaintEventArgs e) {
			e.Graphics.ExcludeClip(Form.ViewInfo.ControlRect);
			base.OnPaintBackground(e);
		}
	}
}
