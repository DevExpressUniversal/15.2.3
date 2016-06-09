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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.ViewInfo;
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class DropDownGalleryControlHandler : GalleryControlHandlerBase {
		GalleryDropDown galleryDropDown;
		GalleryDropDownBarControl galleryBar;
		bool sizeGrip;
		bool mustUpdateUserDefinedSize;
		public DropDownGalleryControlHandler(GalleryDropDown galleryDropDown) : base(galleryDropDown.Gallery) {
			this.galleryDropDown = galleryDropDown;
			this.mustUpdateUserDefinedSize = false;
			this.sizeGrip = false;
		}
		protected internal bool MustUpdateUserDefinedSize { get { return mustUpdateUserDefinedSize; } }
		public GalleryDropDownBarControl BarControl { get { return galleryBar; } set { galleryBar = value; } }
		GalleryDropDown GalleryDropDown { get { return galleryDropDown; } }
		protected internal bool SizeGrip { get { return sizeGrip; } }
		protected override BaseRibbonDesignTimeManager DesignTimeManager { get { return Ribbon.ViewInfo.DesignTimeManager; } }
		protected override RibbonLinkDragManager DragManager { get { return Manager.Helper.DragManager as RibbonLinkDragManager; } }
		protected override bool CanUpdateHotObject {
			get {
				return GalleryDropDown.Visible;
			}
		}
		protected override bool CanHotTrack(RibbonHitInfo hitInfo) {
			bool res = base.CanHotTrack(hitInfo);
			if(!res) return res;
			if(hitInfo.InGalleryItem) return true;
			if(Manager == null) return false;
			if(Manager.ActiveEditor != null) return false;
			return true;
		}
		protected override IComponent ParentComponent { get { return Ribbon; } }
		protected override bool IsCustomizing { get { return Manager.IsCustomizing; } }
		protected override bool ShouldProcessMouseDown {
			get {
				if(Manager != null && Manager.ActiveEditor != null) {
					return Manager.SelectionInfo.CloseEditor();
				}
				return true;
			}
		}
		protected override bool IsInGallerySizingArea(DXMouseEventArgs e) {
			return (Gallery.SizeMode == GallerySizeMode.Both && HitInfo.InGallerySizeGrip) || (Gallery.SizeMode == GallerySizeMode.Vertical && HitInfo.InGallerySizingPanel);
		}
		protected override void ProcessPressInGallerySizingArea(DXMouseEventArgs e) {
			this.mustUpdateUserDefinedSize = true;
			if(Gallery.SizeMode == GallerySizeMode.Both) {
				this.sizeGrip = true;
				SetGalleryCornerSizerCursor();
			}
			else {
				Cursor.Current = Cursors.SizeNS;
			}
			if(!Gallery.SizerBelow) PointOffset = new Size(Form.Size.Width - e.X, e.Y);
			else PointOffset = new Size(Form.Size.Width - e.X, Form.Size.Height - e.Y);
			HitPoint = new Point(e.X, e.Y);
		}
		protected override void OnMouseMoveCore(DXMouseEventArgs e) {
			base.OnMouseMoveCore(e);
			if(MustUpdateUserDefinedSize)
				UpdateFormSize(e);
			else
				UpdateCursor();
		}
		protected override bool ProcessCustomizationMouseMove(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			if(!Gallery.OwnerControl.Visible) return false;
			Point p = e.Location;
			p.Offset(-DesignTimeManager.DownPoint.X, -DesignTimeManager.DownPoint.Y);
			if(e.Button == MouseButtons.Left) {
				if(ShouldStartDragging(p)) {
					DragManager.SelectedDropDownGallery = Gallery.GalleryDropDown;
					DragManager.CalcDropDownScrollRects();
					DragManager.StartDragging(Gallery.OwnerControl, new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta), DesignTimeManager.DragItem, Gallery.OwnerControl);
					return true;
				}
			}
			else {
				DesignTimeManager.DownPoint = e.Location;
			}
			return false;
		}
		protected virtual Point ResizePoint {
			get { return new Point(Gallery.ViewInfo.ResizeCornerBounds.X + Gallery.ViewInfo.ResizeCornerBounds.Width / 2, Gallery.ViewInfo.ResizeCornerBounds.Y + Gallery.ViewInfo.ResizeCornerBounds.Height / 2); }
		}
		protected override void OnUnpressItem(DXMouseEventArgs e) {
			CustomPopupBarControl popupControl = BarControl as CustomPopupBarControl;
			if(popupControl != null) popupControl.LockCloseUp();
			try {
				BarManager manager = GalleryDropDown.SubControl == null ? null : GalleryDropDown.SubControl.Manager;
				if(Gallery.AutoHideGallery) GalleryDropDown.HidePopup();
				if(manager != null && Gallery.AutoHideGallery) {
					RibbonControl ribbon = manager is RibbonBarManager ? ((RibbonBarManager)manager).Ribbon : null;
					if(ribbon == null || ribbon.ViewInfo.CanCloseAllPopups)
						manager.SelectionInfo.OnCloseAll(BarMenuCloseType.All);
				}
				if(e.Button == MouseButtons.Left) {
					if(HitInfo.GalleryItemInfo.ContextButtonsHandler.OnMouseUp(e))
						return;
					Gallery.OnItemClick(null, Gallery, HitInfo.GalleryItemInfo.Item);
				}
			}
			finally {
				if(popupControl != null) popupControl.UnLockCloseUp();
				Cursor.Current = Cursors.Arrow;
			}
		}
		public override void OnUnpress(DXMouseEventArgs e) {
			base.OnUnpress(e);
			this.mustUpdateUserDefinedSize = false;
			this.sizeGrip = false;
		}
		protected virtual int GetDeltaWidth(Point e) {
			if(Gallery.SizeMode != GallerySizeMode.Both) return 0;
			if(!SizeGrip) return 0;
			if(e.X < ResizePoint.X && Form.Width <= FormMinSize.Width) {
				return 0;
			}
			return (e.X + PointOffset.Width) - Form.Width;
		}
		protected virtual int GetDeltaHeight(Point e) {
			if((Gallery.SizeMode == GallerySizeMode.Both || Gallery.SizeMode == GallerySizeMode.Vertical) && Form.Size.Height <= FormMinSize.Height) {
				if((Gallery.SizerBelow && e.Y < ResizePoint.Y) || !Gallery.SizerBelow && e.Y > ResizePoint.Y)
					return 0;
			}
			if(!Gallery.SizerBelow) {
				return -(e.Y - PointOffset.Height);
			}
			return (e.Y + PointOffset.Height) - Form.Height;
		}
		protected virtual Form Form { get { return Gallery.BarControl.Form; } }
		protected internal virtual void UpdateHitPoint(Point e) {
			if(Form == null) return;
			if(Gallery.SizerBelow) HitPoint = new Point(e.X, e.Y);
			else HitPoint = new Point(Form.Size.Width - PointOffset.Width, PointOffset.Height);
		}
		protected virtual Size FormMinSize {
			get {
				Size minSize = new Size(Gallery.MinimumWidth, Gallery.ViewInfo.ItemMaxSize.Height);
				minSize.Width += Gallery.ViewInfo.BackgroundPaddings.Width;
				minSize.Width += Gallery.BarControl.Bounds.X + Form.Width - Gallery.BarControl.Bounds.Right;
				minSize.Height += Gallery.BarControl.ViewInfo.LinksBounds.Height + Gallery.ViewInfo.BackgroundPaddings.Height;
				if(Gallery.ShowGroupCaption && Gallery.ViewInfo.Groups.Count > 0) minSize.Height += Gallery.ViewInfo.Groups[0].CaptionBounds.Height;
				if(Gallery.AllowFilter) minSize.Height += Gallery.ViewInfo.FilterAreaBounds.Height + Gallery.BarControl.Bounds.Y + Form.Height - Gallery.BarControl.Bounds.Bottom;
				if(Gallery.SizeMode != GallerySizeMode.None && Gallery.SizeMode != GallerySizeMode.Default) minSize.Height += Gallery.ViewInfo.ResizeRectBounds.Height;
				return minSize;
			}
		}
		protected virtual Size ConstrainFormSize(Size sz) {
			return new Size(Math.Max(FormMinSize.Width, sz.Width), Math.Max(FormMinSize.Height, sz.Height));
		}
		protected virtual void UpdateFormSize(DXMouseEventArgs e) {
			Point pt = Form.PointToClient(Control.MousePosition);
			e.Handled = true;
			Rectangle bounds = Form.Bounds;
			bounds.Size = ConstrainFormSize(new Size(Form.Size.Width + GetDeltaWidth(pt), Form.Size.Height + GetDeltaHeight(pt)));
			if(!Gallery.SizerBelow)
				bounds.Y -= GetDeltaHeight(pt);
			Gallery.GalleryDropDown.ResizeMenu(bounds);
			UpdateHitPoint(pt);
		}
		protected virtual void SetGalleryCornerSizerCursor() {
			if(Gallery.SizerBelow) Cursor.Current = Cursors.SizeNWSE;
			else Cursor.Current = Cursors.SizeNESW;
		}
		protected virtual void UpdateCursor() {
			if(!HitInfo.InGallerySizingPanel || Gallery.SizeMode == GallerySizeMode.None || Gallery.SizeMode == GallerySizeMode.Default) Cursor.Current = Cursors.Arrow;
			else if(HitInfo.InGallerySizingPanel && Gallery.SizeMode == GallerySizeMode.Vertical) Cursor.Current = Cursors.SizeNS;
			else if(HitInfo.InGallerySizeGrip && Gallery.SizeMode == GallerySizeMode.Both) SetGalleryCornerSizerCursor();
		}
		public new InDropDownGallery Gallery { get { return base.Gallery as InDropDownGallery; } }
		RibbonControl Ribbon { get { return Gallery.Ribbon; } }
		BarManager Manager { get { return Ribbon == null ? Gallery.GalleryDropDown.Manager : Ribbon.Manager; } }
	}
	public class MarqueeAutoScrollHelper {
		Point hitPt;
		GalleryControl gallery;
		System.Threading.Timer tm;
		int offset, oldScrollYPos;
		bool timerEnabled, isLeftButtonPress;
		DXMouseEventArgs mouseEventArg;
		public MarqueeAutoScrollHelper(GalleryControl gallery) {
			tm = new System.Threading.Timer(OnTimedEvent, gallery, System.Threading.Timeout.Infinite, 30);
			this.gallery = gallery;
		}
		public virtual void OnUpdateHitPoint(DXMouseEventArgs ee) {
			if(gallery == null) return;
			if(!isLeftButtonPress) return;
			int delta = gallery.Gallery.ScrollYPosition - oldScrollYPos;
			if(gallery.Gallery.Orientation == Orientation.Vertical)
				hitPt.Y -= delta;
			else 
				hitPt.X -= delta;
			oldScrollYPos = gallery.Gallery.ScrollYPosition;
			gallery.Gallery.ViewInfo.UpdateSelectionRect(hitPt, ee.Location);
		}
		public enum ActionAutoScroll{ ScrollUp, ScrollDown, ScrollLeft, ScrollRight, Exit, StopTimer }
		public void CheckAutoScroll(DXMouseEventArgs e){
			if(gallery == null) return;
			mouseEventArg = e;
			ActionAutoScroll action = GetActionAutoScroll(e);
			if(action == ActionAutoScroll.Exit) return;
			if(action == ActionAutoScroll.StopTimer) {
				Stop();
				return;
			}
			offset = (action == ActionAutoScroll.ScrollUp || action == ActionAutoScroll.ScrollLeft) ? -25 : 25;
			Start();
		}
		protected virtual ActionAutoScroll GetActionAutoScroll(DXMouseEventArgs e) {
			if(e.Button != MouseButtons.Left || !gallery.Gallery.AllowMarqueeSelection) return ActionAutoScroll.Exit;
			if(gallery.Gallery.Orientation == Orientation.Vertical) {
				if(ShouldScrollUp(e)) return timerEnabled ? ActionAutoScroll.Exit : ActionAutoScroll.ScrollUp;
				if(ShouldScrollDown(e)) return timerEnabled ? ActionAutoScroll.Exit : ActionAutoScroll.ScrollDown;
			}
			else {
				if(ShouldScrollLeft(e)) return timerEnabled ? ActionAutoScroll.Exit : ActionAutoScroll.ScrollLeft;
				if(ShouldScrollRight(e)) return timerEnabled ? ActionAutoScroll.Exit : ActionAutoScroll.ScrollRight;
			}
			return ActionAutoScroll.StopTimer;
		}
		protected virtual bool ShouldScrollUp(DXMouseEventArgs e) {
			return e.Location.Y < 1 && e.Button == MouseButtons.Left;
		}
		protected virtual bool ShouldScrollDown(DXMouseEventArgs e) {
			return e.Location.Y > gallery.Height - 1 && e.Button == MouseButtons.Left;
		}
		protected virtual bool ShouldScrollLeft(DXMouseEventArgs e) {
			return e.Location.X < 1 && e.Button == MouseButtons.Left;
		}
		protected virtual bool ShouldScrollRight(DXMouseEventArgs e) {
			return e.Location.X > gallery.Width - 1 && e.Button == MouseButtons.Left;
		}
		protected virtual void OnTimedEvent(object source) {
			if(gallery.IsHandleCreated)
				gallery.BeginInvoke((MethodInvoker)Callback);
		}
		protected virtual void Callback() {
			int newValue = gallery.Gallery.ScrollYPosition + offset;
			if(!ShouldPerformScrolling(newValue))
				return;
			gallery.Gallery.ScrollYPosition = newValue;
			gallery.Refresh();
			OnUpdateHitPoint(mouseEventArg);
		}
		protected virtual bool ShouldPerformScrolling(int value) {
			if(gallery == null || gallery.Gallery == null)
				return false;
			StandaloneGalleryViewInfo vi = gallery.Gallery.ViewInfo;
			return value >= vi.MinScrollYPosition && value <= vi.MaxScrollYPosition;
		}
		protected void Start() {
			tm.Change(0, 30);
			timerEnabled = true;
		}
		protected void Stop() {
			if(!timerEnabled) return;
			tm.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
			timerEnabled = false;
		}
		public void OnMouseDown(DXMouseEventArgs e) {
			if(gallery == null || (gallery.Gallery.ItemCheckMode != ItemCheckMode.Multiple && gallery.Gallery.ItemCheckMode != ItemCheckMode.MultipleInGroup)) return;
			hitPt = e.Location;
			oldScrollYPos = gallery.Gallery.ScrollYPosition;
			isLeftButtonPress = true;
			OnUpdateHitPoint(e);
		}
		public void OnMouseUp() {
			isLeftButtonPress = false;
			Stop();
		}
		public void OnMouseWheel(DXMouseEventArgs e) {
			if(gallery == null || !gallery.Gallery.AllowMarqueeSelection) return;
			OnUpdateHitPoint(e);
		}
		public void OnMouseMove(DXMouseEventArgs e) {
			OnUpdateHitPoint(e);
		}
	}
	public class GalleryControlHandler : GalleryControlHandlerBase {
		public GalleryControlHandler(GalleryControlGallery gallery) : base(gallery) {
		}
		public new GalleryControlGallery Gallery { get { return base.Gallery as GalleryControlGallery; } }
		public GalleryControl GalleryControl { get { return Gallery == null ? null : Gallery.GalleryControl; } }
		protected override bool IsCustomizing {
			get {
				return GalleryControl == null ? false : GalleryControl.DesignModeCore;
			}
		}
		public override void OnMouseLeave(DXMouseEventArgs e) {
			base.OnMouseLeave(e);
			BaseGallery.HideHoverForms();
		}
		protected override void InvalidateOwnerControl(Rectangle bounds) {
			GalleryControl.Invalidate(bounds);
			Rectangle rect = Gallery.ViewInfo.TranslateRect(bounds);
			GalleryControl.Client.Invalidate(rect);
		}
		protected override BaseRibbonDesignTimeManager DesignTimeManager { get { return GalleryControl.DesignTimeManager; } }
		protected override void OnSelectionChanged() {
			GalleryControl.Client.Invalidate();
		}
		public override void OnMouseMove(DXMouseEventArgs e) {
			if(IsCustomizing) {
				if(DesignTimeManager.ProcessMouseMove(e))
					return;
			}
			base.OnMouseMove(e);
			AutoScrollHelper.CheckAutoScroll(e);
		}
		public override void OnMouseDown(DXMouseEventArgs e) {
			if(IsCustomizing) {
				if(DesignTimeManager.ProcessMouseDown(e))
					return;
			}
			base.OnMouseDown(e);
			if(e.Clicks <= 1 && GalleryControl.AllowFocus)
				GalleryControl.Focus();
		}
		public override void OnMouseUp(DXMouseEventArgs e) {
			if(IsCustomizing) {
				if(DesignTimeManager.ProcessMouseUp(e))
					return;
			}
			base.OnMouseUp(e);
		}
		bool IsHorizontalGallery { get { return Gallery.Orientation == Orientation.Horizontal; } }
		public override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			CreateNavigator().OnKeyDown(e);
		}
		protected virtual GalleryNavigationBase CreateNavigator() {
			return GalleryNavigationBase.CreateNavigation(Gallery);
		}
	}
	public class GalleryControlHandlerBase : BaseHandler {
		protected static Point InvalidHitPoint { get { return new Point(-10000, -10000); } }
		bool itemPressed;
		Point hitPoint;
		Size pointOffset;
		GalleryItem activeItem;
		AllowTimer timer = null;
		StandaloneGallery gallery;
		public GalleryControlHandlerBase(StandaloneGallery gallery) {
			this.gallery = gallery;
			this.itemPressed = false;
			this.pointOffset = Size.Empty;
			this.hitPoint = Point.Empty;
			this.activeItem = null;
			timer = new AllowTimer();
			HitPoint = InvalidHitPoint;
			LastHoveredPoint = Point.Empty;
			AutoScrollHelper = CreateAutoScrollHelper();
		}
		protected MarqueeAutoScrollHelper AutoScrollHelper {
			get;
			private set;
		}
		protected virtual MarqueeAutoScrollHelper CreateAutoScrollHelper() {
			return new MarqueeAutoScrollHelper(OwnerControl as GalleryControl);
		}
		public override Control OwnerControl { get { return Gallery.OwnerControl; } }
		protected virtual void InvalidateOwnerControl(Rectangle bounds) {
			OwnerControl.Invalidate(bounds);
		}
		protected StandaloneGallery Gallery { get { return gallery; } }
		internal RibbonHitInfo HitInfo { get { return Gallery.ViewInfo.HitInfo; } set { Gallery.ViewInfo.HitInfo = value; } }
		protected Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		protected Point LastHoveredPoint { get; set; }
		protected internal bool ItemPressed { get { return itemPressed; } }
		internal void SetItemPressed(bool value) { itemPressed = value; }
		public override void UpdateHotObject(DXMouseEventArgs e, bool forceUpdate) {
			if(LastHoveredPoint != e.Location)
				Gallery.ViewInfo.SetKeyboardSelectedItem(null);
			LastHoveredPoint = e.Location;
			if(Gallery.MenuOpened) e = new DXMouseEventArgs(MouseButtons.None, 0, -10000, -10000, 0);
			RibbonHitInfo prevInfo = HitInfo;
			RibbonHitInfo hInfo = Gallery.ViewInfo.CalcHitInfo(e);
			if(!CanHotTrack(hInfo)) return;
			UpdateContextButtonsByMouse(e, prevInfo, hInfo);
			if(hInfo.Equals(prevInfo)) return;
			if(ShouldRaiseItemLeave(prevInfo, hInfo)) prevInfo.Gallery.RaiseGalleryItemLeave(prevInfo.GalleryItem);
			HitInfo = null;
			OnObjectChanging(prevInfo);
			HitInfo = hInfo;
			OnObjectChanged();
			if(HitInfo.InGallery) HitInfo.Gallery.InitHoverTimer(HitInfo);
		}
		protected virtual void UpdateContextButtonsByMouse(DXMouseEventArgs e, RibbonHitInfo prevInfo, RibbonHitInfo newInfo) {
			if(prevInfo.Equals(newInfo)) {
				if(prevInfo.GalleryItemInfo != null)
					prevInfo.GalleryItemInfo.ContextButtonsHandler.OnMouseMove(e);
				return;
			}
			if(!prevInfo.Equals(newInfo)) {
				if(prevInfo.GalleryItemInfo != null)
					prevInfo.GalleryItemInfo.ContextButtonsHandler.OnMouseLeave(e);
				if(newInfo.GalleryItemInfo != null) {
					newInfo.GalleryItemInfo.ContextButtonsHandler.OnMouseEnter(e);
					newInfo.GalleryItemInfo.ContextButtonsHandler.OnMouseMove(e);
				}
			}
		}
		protected virtual bool ShouldRaiseItemLeave(RibbonHitInfo oldInfo, RibbonHitInfo newInfo) {
			if(!oldInfo.InGalleryItem) return false;
			if(!newInfo.InGalleryItem || oldInfo.GalleryItem != newInfo.GalleryItem) return true;
			return false;
		}
		GalleryItem ActiveItem { get { return activeItem; } set { activeItem = value; } }
		protected virtual bool CanHotTrack(RibbonHitInfo hitInfo) {
			return true;
		}
		public Size PointOffset { get { return pointOffset; } set { pointOffset = value; } }
		public override void OnMouseEnter(DXMouseEventArgs e) {
			UpdateHotObject(e, false);
		}
		public override void OnMouseLeave(DXMouseEventArgs e) {
			UpdateHotObject(e, false);
		}
		bool ProcessCustomizationRightMouseDown(DXMouseEventArgs e, RibbonHitInfo dragInfo) { 
			return (DesignTimeManager as RibbonDesignTimeManager).ProcessRightMouseDown(e, dragInfo);
		}
		protected virtual IComponent ParentComponent { get { return Gallery.OwnerControl; } }
		bool ProcessCustomizationMouseDown(DXMouseEventArgs e) {
			timer.InitializeTimer();
			RibbonHitInfo dragInfo = Gallery.ViewInfo.CalcHitInfo(e);
			if(dragInfo.InGalleryItem || dragInfo.InGalleryGroup) {
				DesignTimeManager.DragObject = dragInfo;
				if(dragInfo.InGalleryItem) {
					DesignTimeManager.SelectedGalleryObject = new GalleryObjectDescriptor(dragInfo.GalleryItem, ParentComponent, null);
					DesignTimeManager.SelectComponent(DesignTimeManager.SelectedGalleryObject);
				}
				else {
					DesignTimeManager.SelectedGalleryObject = new GalleryObjectDescriptor(dragInfo.GalleryItemGroup, ParentComponent, null);
					DesignTimeManager.SelectComponent(DesignTimeManager.SelectedGalleryObject);
				}
				if(e.Button == MouseButtons.Left) return true;
			}
			if(e.Button == MouseButtons.Right) {
				ProcessCustomizationRightMouseDown(e, dragInfo);
				return true;
			}
			DesignTimeManager.DragObject = null;
			return false;
		}
		protected virtual bool IsCustomizing { get { return false; } }
		protected virtual bool ShouldProcessMouseDown {
			get { return true; }
		}
		Point downPoint;
		protected Point DownPoint { get { return downPoint; } set { downPoint = value; } }
		public override void OnMouseDown(DXMouseEventArgs e) {
			AutoScrollHelper.OnMouseDown(e);
			if(IsCustomizing) {
				if(ProcessCustomizationMouseDown(e)) {
					Gallery.Invalidate();
					return;
				}
			}
			if(!ShouldProcessMouseDown)
				return;
			UpdateHotObject(e, false);
			OnPress(e);
		}
		protected virtual bool CanUpdateHotObject { get { return true; } }
		public override void OnMouseUp(DXMouseEventArgs e) {
			AutoScrollHelper.OnMouseUp();
			OnUnpress(e);
			if(CanUpdateHotObject)
				UpdateHotObject(e, false);
		}
		public virtual void UpdatePopupImageForms(GalleryItemViewInfo itemInfo) {
			Gallery.HideImageForms(itemInfo, true);
		}
		public override void OnMouseWheel(DXMouseEventArgs ee) {
			Gallery.ScrollYPosition -= (int)(((float)ee.Delta / 120) * 20);
			UpdateHotObject(ee, false);
			AutoScrollHelper.OnMouseWheel(ee);
		}
		protected virtual BaseRibbonDesignTimeManager DesignTimeManager { get { return null; } }
		protected virtual RibbonLinkDragManager DragManager { get { return null; } }
		internal virtual bool ShouldStartDragging(Point p) {
			if(DesignTimeManager.DragObject == null || (DesignTimeManager.DragObject.GalleryItem == null && DesignTimeManager.DragObject.GalleryItemGroup == null)) return false;
			return (Math.Abs(p.X) > DesignTimeManager.DragInterval || Math.Abs(p.Y) > DesignTimeManager.DragInterval) && timer.AllowDrag;
		}
		protected virtual bool ProcessCustomizationMouseMove(DXMouseEventArgs e, RibbonHitInfo hitInfo) {
			return false;
		}
		protected virtual void OnMouseMoveCore(DXMouseEventArgs e) {
			if(e.Button == MouseButtons.Left && Gallery.AllowMarqueeSelection && !Gallery.DesignModeCore && HitPoint != InvalidHitPoint) {
				AutoScrollHelper.OnMouseMove(e);
				OnSelectionChanged();
			}
		}
		protected virtual void OnSelectionChanged() {
			Gallery.OwnerControl.Invalidate();
		}
		public override void OnMouseMove(DXMouseEventArgs e) {
			if(IsCustomizing) {
				if(ProcessCustomizationMouseMove(e, DesignTimeManager.DragObject)) return;
			}
			OnMouseMoveCore(e);
			UpdateHotObject(e, false);
		}
		protected virtual bool IsInGallerySizingArea(DXMouseEventArgs e) {
			return false;
		}
		protected virtual void ProcessPressInGallerySizingArea(DXMouseEventArgs e) {
		}
		bool suppressUnpressItem = false;
		public virtual void OnPress(DXMouseEventArgs e) {
			suppressUnpressItem = false;
			const int MenuOpenDeltaTime = 30000;
			if(Gallery.MenuOpened) Gallery.CloseMenu();
			if(IsInGallerySizingArea(e)) {
				ProcessPressInGallerySizingArea(e);
			}
			else if(HitInfo.InGalleryFilter) {
				if(DateTime.Now.Ticks - Gallery.MenuCloseTime > MenuOpenDeltaTime)
					Gallery.ShowFilterMenu();
			}
			HitPoint = e.Location;
			if(HitInfo.InGalleryItem) {
				if(!HitInfo.GalleryItem.Enabled) return;
				this.itemPressed = true;
				if(e.Button == MouseButtons.Left) {
					if(HitInfo.GalleryItemInfo.ContextButtonsHandler.OnMouseDown(e))
						return;
					Gallery.DownItem = HitInfo.GalleryItemInfo.Item;
					HitInfo.GalleryItemInfo.UpdateContent();
					InvalidateOwnerControl(HitInfo.GalleryInfo.Bounds);
					if(e.Clicks > 1) {
						Gallery.OnItemDoubleClick(null, Gallery, HitInfo.GalleryItemInfo.Item);
						suppressUnpressItem = true;
					}
				}
			}
		}
		protected virtual void OnUnpressItem(DXMouseEventArgs e) {
			if(suppressUnpressItem) {
				suppressUnpressItem = false;
				return;
			}
			if(e.Button == MouseButtons.Left) {
				if(HitInfo.GalleryItemInfo.ContextButtonsHandler.OnMouseUp(e))
					return;
				HitInfo.GalleryItemInfo.UpdateContent();
				Gallery.OnItemClick(null, Gallery, HitInfo.GalleryItemInfo.Item);
			}
		}
		public virtual void OnUnpress(DXMouseEventArgs e) {
			Gallery.DownItem = null;
			if(Gallery.IsMarqueeSelection(HitPoint, e.Location) && HitPoint != InvalidHitPoint && Gallery.AllowMarqueeSelection && !Gallery.DesignModeCore) {
				Gallery.OnSelectionComplete(Gallery.ViewInfo);
			}
			else if(ItemPressed && HitInfo.InGalleryItem) {
				Gallery.HideImageForms(null, false);
				if(e.Button == MouseButtons.Left) {
					OnUnpressItem(e);
				}
				else if(e.Button == MouseButtons.Right) {
					Gallery.OnRightItemClick(Gallery, HitInfo.GalleryItemInfo.Item);
				}
			}
			else if(!ItemPressed && Gallery.ClearSelectionOnClickEmptySpace) {
				Gallery.SetItemsCheckState(false);
			}
			Gallery.ViewInfo.Selection = Rectangle.Empty;
			HitPoint = InvalidHitPoint;
			this.itemPressed = false;
		}
		public virtual void OnObjectChanging(RibbonHitInfo hitInfo) {
			Rectangle bounds = Rectangle.Empty;
			switch(hitInfo.HitTest) {
				case RibbonHitTest.Gallery:
					bounds = hitInfo.GalleryInfo.Bounds;
					hitInfo.GalleryInfo = null;
					break;
				case RibbonHitTest.GalleryItem:
					hitInfo.GalleryItemInfo.UpdateContent();
					bounds = hitInfo.GalleryItemInfo.Bounds;
					hitInfo.GalleryItemInfo = null;
					break;
				case RibbonHitTest.GalleryImage:
					hitInfo.GalleryItemInfo.UpdateContent();
					Gallery.HideImageForms(null, true);					
					break;
				case RibbonHitTest.GalleryFilter:
					bounds = Gallery.ViewInfo.FilterAreaBounds;
					break;
			}
			UpdatePopupImageForms(hitInfo.GalleryItemInfo);
			InvalidateOwnerControl(bounds);
		}
		public virtual void OnObjectChanged() { 
			switch(HitInfo.HitTest) {
				case RibbonHitTest.Gallery:
					InvalidateOwnerControl(HitInfo.GalleryInfo.Bounds);
					break;
				case RibbonHitTest.GalleryItem:
				case RibbonHitTest.GalleryImage:
					HitInfo.GalleryItemInfo.UpdateContent();	
					InvalidateOwnerControl(HitInfo.GalleryItemInfo.Bounds);
					if(HitInfo.HitTest == RibbonHitTest.GalleryImage && Gallery.AllowHoverImages) {
						Gallery.ShowImageForm(Gallery.ViewInfo, HitInfo.GalleryItemInfo);
					}
					if(ActiveItem != HitInfo.GalleryItem) {
						ActiveItem = HitInfo.GalleryItem;
						int groupIndex = Gallery.Groups.IndexOf(HitInfo.GalleryItem.GalleryGroup);
						int itemIndex = HitInfo.GalleryItem.GalleryGroup.Items.IndexOf(HitInfo.GalleryItem);
						if(groupIndex == -1 || itemIndex == -1) Gallery.FindItem(HitInfo.GalleryItem, ref groupIndex, ref itemIndex);
						if(groupIndex != -1 && itemIndex != -1)
							Gallery.AccessibleNotifyClients(AccessibleEvents.Focus, GalleryDropDownBarControl.AccessibleGroupBeginId + groupIndex, itemIndex);
					}
					break;
				case RibbonHitTest.GalleryFilter:
					InvalidateOwnerControl(Gallery.ViewInfo.FilterAreaBounds);
					Gallery.AccessibleNotifyClients(AccessibleEvents.Focus, GalleryDropDownBarControl.AccessibleGalleryFilterId, -1);
					break;
			}
		}
	}
	public class GalleryNavigationBase {
		GalleryControlGallery gallery;
		public GalleryNavigationBase(GalleryControlGallery gallery) {
			this.gallery = gallery;
		}
		public GalleryControlGallery Gallery { get { return gallery; } }
		public virtual void OnKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Space && Gallery.ViewInfo.KeyboardSelectedItem != null) {
				Gallery.OnItemClick(null, Gallery, Gallery.ViewInfo.KeyboardSelectedItem.Item);
			}
		}
		public static GalleryNavigationBase CreateNavigation(GalleryControlGallery gallery) {
			if(gallery.Orientation == Orientation.Horizontal)
				return new GalleryNavigationHorizontal(gallery);
			return new GalleryNavigationVertical(gallery);
		}
	}
	public class GalleryNavigationVertical : GalleryNavigationBase {
		public GalleryNavigationVertical(GalleryControlGallery gallery) : base(gallery) { }
		public override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			switch(e.KeyCode) {
				case Keys.Left:
					if(Gallery.IsRightToLeft)
						Gallery.ViewInfo.MoveHorizontal(+1);
					else
						Gallery.ViewInfo.MoveHorizontal(-1);
					break;
				case Keys.Right:
					if(Gallery.IsRightToLeft)
						Gallery.ViewInfo.MoveHorizontal(-1);
					else
						Gallery.ViewInfo.MoveHorizontal(+1);
					break;
				case Keys.Up:
					Gallery.ViewInfo.MoveVertical(-1);
					break;
				case Keys.Down:
					Gallery.ViewInfo.MoveVertical(+1);
					break;
				case Keys.Home:
					Gallery.ViewInfo.MoveFirstVisibleItem();
					break;
				case Keys.End:
					Gallery.ViewInfo.MoveLastVisibleItem();
					break;
				case Keys.PageDown:
					Gallery.ViewInfo.MovePageDown();
					break;
				case Keys.PageUp:
					Gallery.ViewInfo.MovePageUp();
					break;
			}
		}
	}
	public class GalleryNavigationHorizontal : GalleryNavigationBase {
		public GalleryNavigationHorizontal(GalleryControlGallery gallery) : base(gallery) { }
		public override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			switch(e.KeyCode) {
				case Keys.Left:
					if(Gallery.IsRightToLeft)
						Gallery.ViewInfo.MoveVertical(+1);
					else
						Gallery.ViewInfo.MoveVertical(-1);
					break;
				case Keys.Right:
					if(Gallery.IsRightToLeft)
						Gallery.ViewInfo.MoveVertical(-1);
					else
						Gallery.ViewInfo.MoveVertical(+1);
					break;
				case Keys.Up:
					Gallery.ViewInfo.MoveHorizontal(-1);
					break;
				case Keys.Down:
					Gallery.ViewInfo.MoveHorizontal(+1);
					break;
				case Keys.Home:
					Gallery.ViewInfo.MoveFirstVisibleItem();
					break;
				case Keys.End:
					Gallery.ViewInfo.MoveLastVisibleItem();
					break;
				case Keys.PageDown:
					Gallery.ViewInfo.MovePageDown();
					break;
				case Keys.PageUp:
					Gallery.ViewInfo.MovePageUp();
					break;
			}
		}
	}
}
