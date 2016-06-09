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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Internal;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraBars.Ribbon.Helpers {
	public class RibbonDesignTimeHelpers {
		public static BarItemInfo[] GetItemInfoList(RibbonControl ribbon) {
			ArrayList list = new ArrayList();
			for(int n = 0; n < ribbon.Manager.PaintStyle.ItemInfoCollection.Count; n++) {
				BarItemInfo info = ribbon.Manager.PaintStyle.ItemInfoCollection[n];
				if(info.SupportRibbon) list.Add(info);
			}
			return list.ToArray(typeof(BarItemInfo)) as BarItemInfo[];
		}
	}
	public abstract class RibbonBasePopupForm : CustomTopForm, IBarObject, IFocusablePopupForm, ISupportToolTipsForm {
		RibbonControl sourceRibbon, control;
		public RibbonBasePopupForm(RibbonControl sourceRibbon) {
			MinimumSize = new Size(1, 1);
			this.sourceRibbon = sourceRibbon;
			this.control = CreateRibbon();
			this.control.ShowCustomizationMenu += new RibbonCustomizationMenuEventHandler(OnShowCustomizationMenu);
			Controls.Add(control);
		}
		public virtual void UpdateRibbon() {
			Control.ItemAnimationLength = SourceRibbon.ItemAnimationLength;
			Control.GroupAnimationLength = SourceRibbon.GroupAnimationLength;
			Control.PageAnimationLength = SourceRibbon.PageAnimationLength;
			Control.ApplicationButtonAnimationLength = SourceRibbon.ApplicationButtonAnimationLength;
			Control.GalleryAnimationLength = SourceRibbon.GalleryAnimationLength;
			Control.TransparentEditors = SourceRibbon.TransparentEditors;
			Control.ShowToolbarCustomizeItem = SourceRibbon.ShowToolbarCustomizeItem;
		}
		protected virtual void OnShowCustomizationMenu(object sneder, RibbonCustomizationMenuEventArgs e) {
			sourceRibbon.RaiseShowCustomizationMenu(e);
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ClassStyle |= 0x0800; 
				if((System.Environment.OSVersion.Version.Major > 5 || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor > 0)))
					cp.ClassStyle |= 0x20000; 
				return cp;
			}
		}
		protected RibbonControl SourceRibbon { get { return sourceRibbon; } set { sourceRibbon = value; } }
		protected abstract RibbonControl CreateRibbon();
		protected abstract void ClosePopupForm();
		protected internal virtual void FocusForm() {
			XtraForm form = SourceRibbon.FindForm() as XtraForm;
			if(form != null) form.SuspendRedraw();
			try {
				Focus();
				PopupControlContainer.NCActivateForm(SourceRibbon.FindForm());
			}
			finally {
				if(form != null) form.ResumeRedraw();
			}
		}
		public virtual void ShowPopup() {
			Form frm = SourceRibbon.FindForm();
			if(frm != null)
				frm.AddOwnedForm(this);
			SourceRibbon.Manager.SelectionInfo.internalFocusLock++;
			try {
				Show();
				if(IsFocusRequired) {
					FocusForm();
				}
			}
			finally {
				SourceRibbon.Manager.SelectionInfo.internalFocusLock--;
			}
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			Form frm = SourceRibbon.FindForm();
			if(frm != null)
				frm.RemoveOwnedForm(this);
		}
		protected virtual bool IsFocusRequired { get { return true; } }
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape || keyData == (Keys.Alt | Keys.F4)) {
				if(keyData == Keys.Escape) {
					bool keyTipShow = Control.KeyTipManager.Show;
					ClosePopupForm();
					if(keyTipShow) {
						if(Control is RibbonMinimizedControl) 
							SourceRibbon.KeyTipManager.ActivatePageKeyTips();
						else
							SourceRibbon.KeyTipManager.ActivatePanelKeyTips();
					}
					return true;
				}
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyData == (Keys.F4 | Keys.Alt)) {
				ClosePopupForm();
				e.Handled = true;
				return;
			}
			if(e.KeyData == Keys.Escape) {
				e.Handled = true;
				ClosePopupForm();
				return;
			}
			base.OnKeyDown(e);
		}
		protected override bool AllowMouseActivate { get { return true; } }
		public override Rectangle RealBounds {
			get { return Bounds; }
			set { Bounds = value; }
		}
		public RibbonControl Control { get { return control; } }
		protected virtual BarMenuCloseType ShouldCloseMenuOnClickCore(MouseInfoArgs e, Control child) {
 			return BarMenuCloseType.All;
		}
		#region IBarObject Members
		bool IBarObject.IsBarObject {
			get { return true; }
		}
		BarManager IBarObject.Manager { get { return SourceRibbon.Manager; } }
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			return ShouldCloseMenuOnClickCore(e, child);
		}
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			return false;
		}
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) {
			return false;
		}
		#endregion
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateRegion();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			UpdateRegion();
		}
		protected internal virtual void OnPrepareDisposing() {
		}
		protected internal virtual void OnControllerChanged() {
		}
		protected virtual void UpdateRegion() { }
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		bool IFocusablePopupForm.AllowFocus {
			get { return false; }
		}
		bool ISupportToolTipsForm.ShowToolTipsWhenInactive {
			get { return false; }
		}
		bool ISupportToolTipsForm.ShowToolTipsFor(Form form) {
			return true;
		}
	}
	public class GalleryItemImagePopupForm : CustomTopForm, ISupportXtraAnimation, IXtraObjectWithBounds {
		BaseGalleryViewInfo viewInfo;
		GalleryItemViewInfo itemInfo;
		Rectangle beginBounds, endBounds;
		RibbonControl imageRibbon;
		bool grew, active;
		public GalleryItemImagePopupForm() : this(null, null, null) { }
		public GalleryItemImagePopupForm(RibbonControl ribbon, BaseGalleryViewInfo viewInfo, GalleryItemViewInfo itemInfo) {
			this.viewInfo = viewInfo;
			this.itemInfo = itemInfo;
			this.beginBounds = this.endBounds = Rectangle.Empty;
			this.grew = this.active = false;
			this.imageRibbon = ribbon;
			SetStyle(ControlStyles.AllPaintingInWmPaint | DevExpress.Utils.ControlConstants.DoubleBuffer, true);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!Visible) {
				this.viewInfo = null;
				this.itemInfo = null;
				this.imageRibbon = null;
			}
		}
		public BaseGalleryViewInfo ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
		protected BaseGallery Gallery { get { return ViewInfo != null ? ViewInfo.Gallery : null; } }
		protected GalleryItem Item { get { return ItemInfo.Item; } }
		public GalleryItemViewInfo ItemInfo { get { return itemInfo; } set { itemInfo = value; } }
		public RibbonControl ImageRibbon { get { return imageRibbon; } set { imageRibbon = value; } }
		public Rectangle BeginBounds { get { return beginBounds; } }
		public Rectangle EndBounds { get { return endBounds; } }
		protected internal bool Grew { get { return grew; } }
		protected internal bool Active {
			get {
				if(!IsHandleCreated) return false;
				return active;
			}
		}
		protected virtual void CalcBounds() {
			CalcBeginBounds();
			CalcEndBounds();
			Bounds = BeginBounds;
		}
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			const int WM_NCHITTEST = 0x84, HTTRANSPARENT = (-1);
			if(m.Msg == WM_NCHITTEST) {
				m.Result = new IntPtr(HTTRANSPARENT);
				return;
			}
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		public virtual void HidePopup() {
			BoundsAnimationInfo info = XtraAnimator.Current.Get(this, ItemInfo) as BoundsAnimationInfo;
			if(info == null && !Grew) Hide();
			else {
				int frameCount = info != null ? info.CurrentFrame : BoundsAnimationInfo.MaxFrameCount / 8;
				XtraAnimator.Current.AddBoundsAnimation(this, this, ItemInfo, false, Bounds, BeginBounds, frameCount);
			}
			this.grew = this.active = false;
		}
		public virtual void ShowPopup() {
			BoundsAnimationInfo info = XtraAnimator.Current.Get(this, ItemInfo) as BoundsAnimationInfo;
			if(info != null) {
				XtraAnimator.Current.AddBoundsAnimation(this, this, ItemInfo, true, Bounds, EndBounds, info.CurrentFrame);
			} else {
				CalcBounds();
				Show();
				XtraAnimator.Current.AddBoundsAnimation(this, this, ItemInfo, true, BeginBounds, EndBounds, 16);
			}
			this.active = true;
		}
		protected internal int DeltaWidth { 
			get {
				return Math.Max(0, ((ItemInfo.GalleryInfo.ImageBackgroundIndent.Width + ViewInfo.GetHoverImageSize(ItemInfo).Width) - ItemInfo.ImageContentBounds.Width) / 2); 
			} 
		}
		protected internal int DeltaHeight { 
			get {
				return Math.Max(0, ((ItemInfo.GalleryInfo.ImageBackgroundIndent.Height + ViewInfo.GetHoverImageSize(ItemInfo).Height) - ItemInfo.ImageContentBounds.Height) / 2); 
			} 
		}
		protected internal Control OwnerControl {
			get {
				InRibbonGalleryViewInfo inRibbonInfo = ViewInfo as InRibbonGalleryViewInfo;
				if(inRibbonInfo != null) {
					if(inRibbonInfo.ItemInfo.ViewInfo.OwnerControl is RibbonMiniToolbarControl)
						return inRibbonInfo.ItemInfo.ViewInfo.OwnerControl;
					return ((RibbonGalleryBarItemLink)inRibbonInfo.Item).Ribbon;
				}
				return (ViewInfo as StandaloneGalleryViewInfo).Gallery.OwnerControl;
			}
		}
		protected virtual SkinElementInfo GetBackgroundInfo(Rectangle r){
			SkinElement elem = Gallery.GetSkin(RibbonSkins.SkinGalleryHoverImageBackground);
			if(elem != null) return new SkinElementInfo(elem, r);
			return Gallery.ItemPainter.GetImageBackgroundInfo(Gallery, r);
		}
		public virtual void CalcBeginBounds() {
			this.beginBounds = OwnerControl.RectangleToScreen(ItemInfo.ImageContentBounds);
			this.beginBounds = ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, GetBackgroundInfo(this.beginBounds), this.beginBounds);
		}
		protected virtual Size GetImageSize() {
			return ItemInfo.ImageClientBounds.Size;
		}
		public virtual void CalcEndBounds() {
			Rectangle res = Rectangle.Inflate(BeginBounds, DeltaWidth, DeltaHeight);
			if(ActAsPopup) {
				Rectangle formBounds = CalcDesktopRect();
				if(res.Left < formBounds.Left)
					res.Offset(formBounds.Left - res.Left, 0);
				if(res.Top < formBounds.Top)
					res.Offset(0, formBounds.Top - res.Top);
				if(res.Right > formBounds.Right)
					res.Offset(-(res.Right - formBounds.Right), 0);
				if(res.Bottom > formBounds.Bottom)
					res.Offset(0, -(res.Bottom - formBounds.Bottom));
			}
			this.endBounds = res;
		}
		protected virtual bool ActAsPopup {
			get { return Gallery != null && Gallery.ImagePopupFormActAsPopup; }
		}
		protected Rectangle CalcDesktopRect() {
			return Screen.FromRectangle(BeginBounds).Bounds;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e))) {
				Rectangle r = new Rectangle(Point.Empty, Size);
				if(Gallery.DrawImageBackground) {
					ObjectInfoArgs backInfo = GetBackgroundInfo(r);
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, backInfo);
					r = ObjectPainter.GetObjectClientRectangle(cache.Graphics, SkinElementPainter.Default, backInfo);
				}
				if(ImageCollection.IsImageExists(Item.HoverImage, Gallery.HoverImages, Item.HoverImageIndex))
					ImageCollection.DrawImageListImage(cache, Item.HoverImage, Gallery.HoverImages, Item.HoverImageIndex, r, true);
				else
					ImageCollection.DrawImageListImage(cache, Item.Image, Gallery.Images, Item.ImageIndex, r, true);
			}
		}
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		Rectangle IXtraObjectWithBounds.AnimatedBounds {
			get { return Bounds; }
			set { Bounds = value; }
		}
		void IXtraObjectWithBounds.OnEndBoundAnimation(BoundsAnimationInfo info) {
			if(info.GrowUp) this.grew = true;
			if(!info.GrowUp) {
				Hide();
				this.active = false;
			}
		}
		protected override bool HasSystemShadow { get { return true; } }
	}
	public class RibbonQuickToolbarPopupForm : RibbonBasePopupForm {
		RibbonQuickAccessToolbarViewInfo toolbarInfo;
		public RibbonQuickToolbarPopupForm(RibbonControl sourceRibbon)
			: base(sourceRibbon) {
			ToolbarInfo = SourceRibbon.ViewInfo.Toolbar;
		}
		protected override void ClosePopupForm() { SourceRibbon.PopupToolbar = null; }
		protected override RibbonControl CreateRibbon() { return new RibbonPopupToolbarControl(SourceRibbon); }
		public RibbonQuickAccessToolbarViewInfo ToolbarInfo {
			get { return toolbarInfo; }
			set {
				toolbarInfo = value;
				if(toolbarInfo == null) return;
				CalcBounds();
			}
		}
		protected internal void CalcLocation() {
			if(ToolbarInfo != null) {
				Rectangle link = ToolbarInfo.DropDownButtonBounds;
				Location = DevExpress.Utils.ControlUtils.CalcLocation(
					SourceRibbon.PointToScreen(new Point(link.X, link.Bottom)), 
					SourceRibbon.PointToScreen(link.Location), Size);
			}
		}
		protected internal void CalcSize() {
			Size = ((RibbonPopupToolbarControl)Control).CalcPopupSize();
			Control.ViewInfo.CalcViewInfo(new Rectangle(0, 0, Size.Width, Size.Height));
			Size = Control.ViewInfo.Toolbar.ContentBackgroundBounds.Size;
		}
		protected internal void CalcBounds() {
			if(ToolbarInfo == null) return;
			CalcSize();
			CalcLocation();
			Control.Refresh();
		}
	}
	public class MinimizedRibbonPopupForm : RibbonBasePopupForm {
		Form parentForm;
		public MinimizedRibbonPopupForm(RibbonControl sourceRibbon)
			: base(sourceRibbon) {
			RibbonMinimizedControl minimizedControl = (RibbonMinimizedControl)Control;
			SourceRibbon.SubscribeMouseEvents(minimizedControl);
			SourceRibbon.AssignToMinimized(minimizedControl);
			SourceRibbon.VisibleChanged += new EventHandler(OnSourceVisibleChanged);
			this.parentForm = SourceRibbon.FindForm();
			if(parentForm != null) {
				this.parentForm.SizeChanged += new EventHandler(OnParentSizeChanged);
				this.parentForm.VisibleChanged += new EventHandler(OnParentEvent);
			}
		}
		void OnParentSizeChanged(object sender, EventArgs e) {
			RibbonForm form = sender as RibbonForm;
			if(form == null || form.Size != form.PreviousSize)
				ClosePopupForm();
		}
		void OnParentEvent(object sender, EventArgs e) {
			ClosePopupForm();
		}
		void OnSourceVisibleChanged(object sender, EventArgs e) {
			ClosePopupForm();
		}
		protected override void Dispose(bool disposing) {
			if(SourceRibbon != null) {
				SourceRibbon.UnsubscribeMouseEvents((RibbonMinimizedControl)Control);
				SourceRibbon.VisibleChanged -= new EventHandler(OnSourceVisibleChanged);
			}
			if(parentForm != null) {
				this.parentForm.SizeChanged -= new EventHandler(OnParentSizeChanged);
				this.parentForm.VisibleChanged -= new EventHandler(OnParentEvent);
			}
			base.Dispose(disposing);
		}
		protected override RibbonControl CreateRibbon() {
			return new RibbonMinimizedControl(SourceRibbon.Manager, SourceRibbon);
		}
		protected override void ClosePopupForm() {
			SourceRibbon.MinimizedRibbonPopupForm = null;
		}
		public override void UpdateRibbon() {
			base.UpdateRibbon();
			Control.Dock = DockStyle.None;
			Control.ClosePopupForms();
			RibbonPage prevPage = Control.SelectedPage;
			if(prevPage != null)
				prevPage.Dispose();
			Control.PageCategories.Clear();
			RibbonPageCategory cat = Control.DefaultPageCategory;
			cat.Pages.Clear();
			if(SourceRibbon.SelectedPage != null && !SourceRibbon.SelectedPage.Category.IsDefaultColor) {
				cat = new RibbonPageCategory(SourceRibbon.SelectedPage.Category.Text, SourceRibbon.SelectedPage.Category.Color);
				Control.PageCategories.Add(cat);
			}
			cat.Pages.Add(new RibbonPage());
			cat.Pages[0].Groups.Clear();
			if(SourceRibbon.SelectedPage != null) {
				foreach(RibbonPageGroup group in SourceRibbon.SelectedPage.Groups) {
					if(!group.Visible) continue;
					cat.Pages[0].Groups.Add((RibbonPageGroup)group.Clone());
				}
				foreach(RibbonPageGroup group in SourceRibbon.SelectedPage.MergedGroups) {
					if(!group.Visible) continue;
					cat.Pages[0].MergedGroups.Add((RibbonPageGroup)group.Clone());
				}
			}
			Control.SelectedPage = cat.Pages[0];
			Rectangle bounds = new Rectangle(0, 0, SourceRibbon.Width, 0);
			Control.ViewInfo.CalcViewInfo(bounds);
			bounds.Height = Control.ViewInfo.CalcMinHeight();
			Control.ViewInfo.CalcViewInfo(bounds);
			CalcBounds();
		}
		protected internal virtual void CalcLocation() {
			Location = CalcPopupLocationCore();
			Control.Location = CalcControlLocationCore();
		}
		protected virtual Point CalcPopupLocationCore() {
			Point pt = ControlUtils.CalcLocation(
				SourceRibbon.PointToScreen(new Point(SourceRibbon.ViewInfo.Header.Bounds.X, SourceRibbon.ViewInfo.Header.Bounds.Bottom - Control.ViewInfo.PanelYOffset)),
				SourceRibbon.PointToScreen(SourceRibbon.ViewInfo.Header.Bounds.Location),
				Control.ViewInfo.Bounds.Size, false);
			RibbonControlStyle rs = SourceRibbon.GetRibbonStyle();
			if(rs != RibbonControlStyle.Office2007 && rs != RibbonControlStyle.MacOffice) pt.Offset(0, 1);
			return pt;
		}
		protected virtual Point CalcControlLocationCore() {
			SkinElement tabPanel = GetPanelElement();
			return new Point(-tabPanel.Properties.GetInteger(RibbonSkins.SkinTabPanelLeftIndent), -tabPanel.Properties.GetInteger(RibbonSkins.SkinTabPanelTopIndent));
		}
		protected internal virtual void CalcSize() {
			Size size = CalcControlSize();
			CalcPopupSize(size);
		}
		protected virtual Size CalcControlSize() {
			Size sz = CalcControlSizeCore();
			Control.Size = sz;
			return sz;
		}
		protected virtual void CalcPopupSize(Size controlDesiredSize) {
			Size = DoFitPopupSize(CalcPopupSizeCore(controlDesiredSize));
		}
		protected virtual Size CalcControlSizeCore() {
			SkinElement tabPanel = GetPanelElement();
			return new Size(SourceRibbon.Width, Control.ViewInfo.Panel.Bounds.Height);
		}
		protected virtual Size CalcPopupSizeCore(Size controlSize) {
			Size size = controlSize;
			SkinElement tabPanel = GetPanelElement();
			size.Height -= tabPanel.Properties.GetInteger(RibbonSkins.SkinTabPanelBottomIndent) + tabPanel.Properties.GetInteger(RibbonSkins.SkinTabPanelTopIndent);
			size.Width -= tabPanel.Properties.GetInteger(RibbonSkins.SkinTabPanelRightIndent) + tabPanel.Properties.GetInteger(RibbonSkins.SkinTabPanelLeftIndent);
			return size;
		}
		protected virtual Size DoFitPopupSize(Size proposedSize) {
			return proposedSize;
		}
		protected internal virtual void CalcBounds() {
			CalcLocation();
			CalcSize();
		}
		protected virtual SkinElement GetPanelElement() { 
			return RibbonSkins.GetSkin(Control.ViewInfo.Provider)[RibbonSkins.SkinTabPanel];
		}
		protected override void UpdateRegion() {
			SkinElement tabPanel = GetPanelElement();
			int rr = tabPanel.Properties.GetInteger(RibbonSkins.SkinTabPanelRegionRadius);
			if(rr == 0) Region = null;
			Region = NativeMethods.CreateRoundRegion(new Rectangle(Point.Empty, Size), rr);
		}
	}
	public class FullScreenMinimizedRibbonPopupForm : MinimizedRibbonPopupForm, ISupportXtraAnimation, IXtraAnimationListener {
		Image renderPic;
		DXLayeredImageWindow layer;
		RibbonPopupFormFadeAnimationInfoBase.ImageContainer imageContainer;
		public FullScreenMinimizedRibbonPopupForm(RibbonControl sourceRibbon, Image sourceRibbonPic) : base(sourceRibbon) {
			this.renderPic = sourceRibbonPic;
			this.layer = CreateLayer(sourceRibbonPic);
			this.imageContainer = CreateImageContainer(sourceRibbonPic);
			Controls.Add(imageContainer);
		}
		protected override RibbonControl CreateRibbon() {
			return new RibbonFullscreenMinimizedControl(SourceRibbon.Manager, SourceRibbon);
		}
		protected virtual DXLayeredImageWindow CreateLayer(Image image) {
			return new DXLayeredImageWindow(image, Control);
		}
		protected virtual void DestroyImageContainer() {
			if(imageContainer == null) return;
			Controls.Remove(imageContainer);
			imageContainer.Dispose();
			imageContainer = null;
		}
		protected virtual RibbonPopupFormFadeAnimationInfoBase.ImageContainer CreateImageContainer(Image image) {
			return new RibbonPopupFormFadeAnimationInfoBase.ImageContainer(image, this);
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ClassStyle &= ~0x20000; 
				return cp;
			}
		}
		protected override Point CalcPopupLocationCore() {
			Point loc = base.CalcPopupLocationCore();
			RibbonPopupFormFadeAnimationInfoBase info = GetFadeAnimationInfo();
			if(info != null) {
				return new Point(loc.X, info.Current);
			}
			return loc;
		}
		protected override Size CalcPopupSizeCore(Size controlSize) {
			RibbonPopupFormFadeAnimationInfoBase info = GetFadeAnimationInfo();
			if(info != null) {
				return renderPic.Size;
			}
			return base.CalcPopupSizeCore(controlSize);
		}
		protected override Size CalcControlSizeCore() {
			SkinElement tabPanel = GetPanelElement();
			return SourceRibbon.ViewInfo.Panel.Bounds.Size;
		}
		protected RibbonPopupFormFadeAnimationInfoBase GetFadeAnimationInfo() {
			RibbonPopupFormFadeAnimationInfoBase info = XtraAnimator.Current.Get(this, FadeInAnimationId) as RibbonPopupFormFadeAnimationInfoBase;
			return info != null && info.InProgress ? info : null;
		}
		protected override Size DoFitPopupSize(Size proposedSize) {
			int bottom = GetPopupBottomLine();
			if(bottom > proposedSize.Height) {
				return new Size(proposedSize.Width, bottom);
			}
			return base.DoFitPopupSize(proposedSize);
		}
		protected int GetPopupBottomLine() {
			if(!SourceRibbon.FormStateObj.HasPanel) return 0;
			return Control.Bottom;
		}
		bool fadeInCompleted = false;
		protected internal override void CalcBounds() {
			if(!fadeInCompleted) {
				DoFadeIn();
				fadeInCompleted = true;
			}
			base.CalcBounds();
		}
		protected internal virtual void UpdateBoundsCore() {
			RibbonPopupFormFadeAnimationInfoBase info = XtraAnimator.Current.Get(this, FadeInAnimationId) as RibbonPopupFormFadeAnimationInfoBase;
			if(info == null) return;
			CheckImageLayer();
			CalcBounds();
		}
		protected void CheckImageLayer() {
			if(!ShouldDisplayImageLayer) return;
			layer.Show(ImageLayerLocation);
		}
		protected virtual Point ImageLayerLocation {
			get {
				Point res = SourceRibbon.PointToScreen(SourceRibbon.Location);
				return new Point(res.X, Math.Max(res.Y, 0));
			}
		}
		protected virtual bool ShouldDisplayImageLayer {
			get { return !IsAnimationActive; }
		}
		protected virtual bool IsAnimationActive {
			get { return GetFadeAnimationInfo() != null; }
		}
		protected void BringRibbonToFront() {
			Control.BringToFront();
			DestroyImageContainer();
			if(layer != null && layer.IsActive) {
				layer.Close(500);
			}
		}
		protected void BringImageContainerToFront() {
			imageContainer.BringToFront();
		}
		protected virtual void OnFadeInAnimationFinished() {
			SourceRibbon.OnMinimizedRibbonPopupFadeInAnimationFinished();
			BringRibbonToFront();
		}
		protected internal override void OnControllerChanged() {
			base.OnControllerChanged();
			Control.OnControllerChanged();
			MethodInvoker callback = () => {
				SourceRibbon.ViewInfo.IsReady = false;
				SourceRibbon.Update();
				UpdateRibbon();
				Refresh();
			};
			BeginInvoke(callback);
		}
		protected internal override void OnPrepareDisposing() {
			base.OnPrepareDisposing();
			if(disposed) return;
			if(ShouldUpdateRenderImage) SourceRibbon.UpdateRenderImage(GetRenderImage());
		}
		protected bool ShouldUpdateRenderImage {
			get {
				if(SourceRibbon == null || SourceRibbon.ViewInfo == null || SourceRibbon.ViewInfo.Form == null)
					return false;
				if(SourceRibbon.ViewInfo.Form.WindowState != FormWindowState.Maximized)
					return false;
				return !IsAnimationActive;
			}
		}
		protected virtual Image GetRenderImage() {
			Bitmap top = BarUtilites.RenderToBitmap(SourceRibbon, SourceRibbon.ViewInfo.RenderImageOffset);
			if(Control.Width == 0) return top;
			Bitmap bottom = BarUtilites.RenderToBitmap(Control, 0);
			return BarUtilites.Merge(top, bottom);
		}
		#region Closing Controller
		protected override BarMenuCloseType ShouldCloseMenuOnClickCore(MouseInfoArgs e, Control child) {
			if(!CanClosePopup) return BarMenuCloseType.None;
			return base.ShouldCloseMenuOnClickCore(e, child);
		}
		protected virtual bool CanClosePopup {
			get {
				if(IsAnimationActive) return false;
				return true;
			}
		}
		#endregion
		#region Fade In Animation
		static readonly int FadeAnimationLenght = 500;
		protected internal readonly object FadeInAnimationId = new object();
		protected virtual void DoFadeIn() {
			BringImageContainerToFront();
			RemoveFadeIn();
			RibbonPopupFormFadeAnimationInfoBase info = new RibbonPopupFormFadeInAnimationInfo(this, FadeInStartValue, FadeInEndValue, FadeAnimationLenght);
			XtraAnimator.Current.AddAnimation(info);
		}
		protected virtual void RemoveFadeIn() {
			if(XtraAnimator.Current.Get(this, FadeInAnimationId) == null)
				return;
			XtraAnimator.Current.Animations.Remove(this, FadeInAnimationId);
		}
		protected virtual int FadeInStartValue { get { return SourceRibbon.Location.Y - renderPic.Height; } }
		protected virtual int FadeInEndValue { get { return SourceRibbon.Location.Y; } }
		#endregion
		#region ISupportXtraAnimation
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		#region IXtraAnimationListener
		public void OnAnimation(BaseAnimationInfo info) {
		}
		public void OnEndAnimation(BaseAnimationInfo info) {
			if(info is RibbonPopupFormFadeInAnimationInfo) {
				OnFadeInAnimationFinished();
			}
		}
		#endregion
		#region Disposing
		bool disposed = false;
		protected override void Dispose(bool disposing) {
			disposed = true;
			SourceRibbon.OnMinimizedRibbonPopupFormDisposed();
			RemoveFadeIn();
			if(disposing) {
				if(layer != null) layer.Dispose();
			}
			layer = null;
			renderPic = null;
			base.Dispose(disposing);
		}
		#endregion
		protected override bool IsTopMost {
			get {
				Form parentForm = SourceRibbon.FindForm();
				return parentForm != null ? parentForm.TopMost : false;
			}
		}
		protected override bool HasSystemShadow { get { return false; } }
	}
	public abstract class RibbonPopupFormFadeAnimationInfoBase : BaseAnimationInfo {
		bool inProgress;
		int start, end, current;
		SplineAnimationHelper splineAnimationHelper;
		FullScreenMinimizedRibbonPopupForm popupForm;
		public RibbonPopupFormFadeAnimationInfoBase(FullScreenMinimizedRibbonPopupForm popupForm, int start, int end, int lenght)
			: base(popupForm, popupForm.FadeInAnimationId, 5, (int)(TimeSpan.TicksPerMillisecond * lenght / 5)) {
			this.start = start;
			this.end = end;
			this.current = start;
			this.popupForm = popupForm;
			inProgress = true;
			this.splineAnimationHelper = new SplineAnimationHelper();
			this.splineAnimationHelper.Init(Math.Min(Start, End), Math.Max(Start, End), 1);
		}
		public override void FrameStep() {
			double k = ((double)(CurrentFrame)) / FrameCount;
			current = CalcCurrent(k);
			if(IsFinalFrame) {
				current = end;
				inProgress = false;
			}
			popupForm.UpdateBoundsCore();
		}
		public abstract int CalcCurrent(double k);
		public int Start { get { return start; } }
		public int End { get { return end; } }
		public int Current { get { return current; } }
		public bool InProgress { get { return inProgress; } }
		public SplineAnimationHelper SplineAnimationHelper { get { return splineAnimationHelper; } }
		#region ImageContainer
		[ToolboxItem(false)]
		public class ImageContainer : Control {
			Image image;
			public ImageContainer(Image image, Control parent) {
				this.image = image;
				this.Parent = parent;
			}
			protected override void OnHandleCreated(EventArgs e) {
				base.OnHandleCreated(e);
				Size = Image.Size;
			}
			protected override void OnPaint(PaintEventArgs e) {
				base.OnPaint(e);
				e.Graphics.DrawImage(Image, Point.Empty);
			}
			protected override void Dispose(bool disposing) {
				this.image = null;
				this.Parent = null;
				base.Dispose(disposing);
			}
			public Image Image { get { return image; } }
		}
		#endregion
	}
	public class RibbonPopupFormFadeInAnimationInfo : RibbonPopupFormFadeAnimationInfoBase {
		public RibbonPopupFormFadeInAnimationInfo(FullScreenMinimizedRibbonPopupForm popupForm, int start, int end, int lenght)
			: base(popupForm, start, end, lenght) {
		}
		public override int CalcCurrent(double k) {
			return (int)SplineAnimationHelper.CalcSpline(k);
		}
	}
	public class RibbonMinimizedGroupPopupForm : RibbonBasePopupForm {
		RibbonPageGroupViewInfo groupInfo;
		RibbonToolbarPopupItemLink groupToolbarLink = null;
		public RibbonMinimizedGroupPopupForm(RibbonControl sourceRibbon) : base(sourceRibbon) {
		}
		protected override RibbonControl CreateRibbon() {
			return new RibbonOneGroupControl(SourceRibbon.Manager, SourceRibbon);
		}
		protected internal virtual void UnpressGroupToolbarItem() {
			if(GroupInfo == null && Group == null) return;
			Group.GetOriginalPageGroup().ToolbarContentButton.Down = false;
		}
		protected override void ClosePopupForm() {
			UnpressGroupToolbarItem();
			SourceRibbon.PopupGroupForm = null;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnpressGroupToolbarItem();
				if(Group != null) SourceRibbon.ViewInfo.Invalidate(Group);
			} 
			base.Dispose(disposing);
		}
		protected virtual SkinElement GetPanelGroupElement() {
			return RibbonSkins.GetSkin(SourceRibbon.ViewInfo.Provider)[RibbonSkins.SkinTabPanelGroupCaption];
		}
		protected virtual SkinElement GetPanelElement() {
			string skinName = Group.Page.Category.IsDefaultColor? RibbonSkins.SkinTabPanel: RibbonSkins.SkinContextTabPanel;
			return RibbonSkins.GetSkin(SourceRibbon.ViewInfo.Provider)[skinName];
		}
		protected override void OnVisibleChanged(EventArgs e) {
			if(!Visible) {
				Control.KeyTipManager.HideKeyTips();
				UnpressGroupToolbarItem();
		}
		}
		protected override void UpdateRegion() {
			SkinElement tabPanel = GetPanelGroupElement();
			int rr = tabPanel.Properties.GetInteger(RibbonSkins.SkinTabPanelGroupRegionRadius);
			if(rr == 0) Region = null;
			Region = NativeMethods.CreateRoundRegion(new Rectangle(Point.Empty, Size), rr);
		}
		public RibbonPageGroup Group { 
			get {
				if(GroupInfo != null) return GroupInfo.PageGroup;
				if(GroupToolbarLink != null) return GroupToolbarLink.PageGroup;
				return null;
			} 
		}
		public RibbonPageGroupViewInfo GroupInfo {
			get { return groupInfo; }
			set {
				groupInfo = value;
				if(groupInfo == null) return;
				UpdateRibbon();
				CalcBounds();
			}
		}
		public RibbonToolbarPopupItemLink GroupToolbarLink {
			get { return groupToolbarLink; }
			set {
				groupToolbarLink = value;
				if(groupToolbarLink == null) return;
				UpdateRibbon();
				CalcBounds();
			}
		}
		protected virtual int CalcBestWidth() {
			if(Control.ViewInfo == null) return 0;
			Control.ViewInfo.CalcViewInfo(new Rectangle(0, 0, 10000, SourceRibbon.ViewInfo.PanelHeight));
			SkinElement elem = GetPanelElement();
			return Control.ViewInfo.Panel.Groups[0].CalcBestWidth(SourceRibbon.ViewInfo.GroupContentHeight) + elem.ContentMargins.Width;
		}
		public override void UpdateRibbon() {
			base.UpdateRibbon();
			if(Control.Pages.Count == 0) Control.Pages.Add(new RibbonPage());
			if(SourceRibbon.SelectedPage != null)
				Control.Pages[0].KeyTip = SourceRibbon.SelectedPage.KeyTip;
			Control.Pages[0].Groups.Clear();
			Control.Pages[0].Groups.Add((RibbonPageGroup)Group.Clone());
			Control.SelectedPage = Control.Pages[0];
			int width = CalcBestWidth();
			ClientSize = new Size(width, ClientSize.Height);
			Control.Width = width;
			Control.ViewInfo.CalcViewInfo(new Rectangle(0, 0, Control.Width, SourceRibbon.ViewInfo.PanelHeight));
		}
		protected internal void CalcLocation() {
			if(GroupToolbarLink != null) {
				Location = DevExpress.Utils.ControlUtils.CalcLocation(SourceRibbon.PointToScreen(new Point(GroupToolbarLink.LinkViewInfo.Bounds.X, GroupToolbarLink.LinkViewInfo.Bounds.Bottom)),
					SourceRibbon.PointToScreen(GroupToolbarLink.LinkViewInfo.Bounds.Location),
					Control.ViewInfo.Bounds.Size);
			}
			if(GroupInfo != null) {
				Location = DevExpress.Utils.ControlUtils.CalcLocation(
					SourceRibbon.PointToScreen(new Point(GroupInfo.Bounds.X, GroupInfo.Bounds.Bottom)),
					SourceRibbon.PointToScreen(GroupInfo.Bounds.Location),
					Control.ViewInfo.Bounds.Size);
			}
			SkinElement tg = GetPanelGroupElement();
			Control.Location = new Point(-Control.ViewInfo.Panel.Groups[0].Bounds.X - tg.Properties.GetInteger(RibbonSkins.SkinTabPanelGroupLeftIndent), -Control.ViewInfo.Panel.Groups[0].Bounds.Y - tg.Properties.GetInteger(RibbonSkins.SkinTabPanelGroupTopIndent));
			Control.Dock = DockStyle.None;
		}
		protected internal void CalcSize() {
			Size sz = Control.ViewInfo.Panel.Groups[0].Bounds.Size;
			Rectangle panelRect = Control.ViewInfo.Panel.Bounds;
			SkinElement tg = GetPanelGroupElement();
			sz.Height -= tg.Properties.GetInteger(RibbonSkins.SkinTabPanelGroupBottomIndent) + tg.Properties.GetInteger(RibbonSkins.SkinTabPanelGroupTopIndent);
			sz.Width -= tg.Properties.GetInteger(RibbonSkins.SkinTabPanelGroupRightIndent) + tg.Properties.GetInteger(RibbonSkins.SkinTabPanelGroupLeftIndent);
			Size = sz;
			Control.Size = panelRect.Size;
			Control.ViewInfo.CalcViewInfo(panelRect);
		}
		protected internal void CalcBounds() {
			if(GroupInfo == null && GroupToolbarLink == null) return;
			CalcLocation();
			CalcSize();
		}
	}
	[ToolboxItem(false)]
	public class CustomBaseRibbonControl : RibbonControl {
		RibbonBarManager manager;
		RibbonControl sourceRibbon;
		public CustomBaseRibbonControl() { 
		}
		protected override bool IsAutoSizeRibbon {
			get {
				return false;
			}
		}
		protected override Rectangle RibbonClientRectangle {
			get {
				return ClientRectangle;
			}
		}
		protected override bool ShouldClosePopupsOnDisabledItemClick(BarItemLink barItemLink) {
			return false;
		}
		public void Initialize(RibbonBarManager manager, RibbonControl sourceRibbon) {
			this.sourceRibbon = sourceRibbon;
			this.manager = manager;
			Assign(sourceRibbon);
		}
		public CustomBaseRibbonControl(RibbonBarManager manager, RibbonControl sourceRibbon) {
			Initialize(manager, sourceRibbon);
		}
		protected virtual void Assign(RibbonControl srcRibbon) {
			if(srcRibbon == null) return;
			ItemsVertAlign = srcRibbon.ItemsVertAlign;
			ButtonGroupsVertAlign = srcRibbon.ButtonGroupsVertAlign;
			AutoSizeItems = srcRibbon.AutoSizeItems;
			RibbonStyle = srcRibbon.RibbonStyle;
			AutoHideEmptyItems = srcRibbon.AutoHideEmptyItems;
			OptionsTouch.Assign(srcRibbon.OptionsTouch);
			AllowInplaceLinks = srcRibbon.AllowInplaceLinks;
		}
		protected internal override void OnBeforeKeyTipClick() {
		}
		protected internal RibbonControl SourceRibbon { get { return sourceRibbon; } }
		protected override RibbonBarManager CreateBarManager() { return null; }
		public override RibbonBarManager Manager { get { return manager; } }
		protected override bool ShouldInitOrDestroyBarManager { get { return false; } }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			this.manager = null;
		}
		public override bool IsDesignMode { get { return sourceRibbon.IsDesignMode; } }
		protected internal override object InternalGetService(Type type) {
			return SourceRibbon.InternalGetService(type);
		}
		protected override RibbonViewInfo CreateViewInfo() {
			return new CustomBaseRibbonViewInfo(this);
		}
		public override BarAndDockingController GetController() {
			if(Manager != null) return base.GetController();
			return SourceRibbon != null ? SourceRibbon.GetController() : base.GetController();
		}
	}
	[ToolboxItem(false)]
	public class PreviewRibbon : CustomBaseRibbonControl {
		public PreviewRibbon(RibbonBarManager manager, RibbonControl sourceRibbon) : base(manager, sourceRibbon) { }
		public override bool IsDesignMode {
			get {
				return false;
			}
		}
		protected override RibbonViewInfo CreateViewInfo() {
			return new PreviewRibbonViewInfo(this);
		}
		bool InCheckViewInfo { get; set; }
		protected internal override void CheckViewInfo() {
			if(InCheckViewInfo)
				return;
			InCheckViewInfo = true;
			try {
				base.CheckViewInfo();
				if(Manager == null || SourceRibbon == null)
					return;
				int width = ViewInfo.Panel.CalcTotalWidth();
				SkinElementInfo info = ViewInfo.GetPanelInfo();
				info.Bounds = new Rectangle(info.Bounds.X, info.Bounds.Y, width, info.Bounds.Height);
				Width = ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, info, info.Bounds).Width;
				Height = ViewInfo.CalcMinHeight();
			}
			finally {
				InCheckViewInfo = false;
			}
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
	[ToolboxItem(false)]
	public class RibbonFullscreenMinimizedControl : RibbonMinimizedControl {
		public RibbonFullscreenMinimizedControl(RibbonBarManager manager, RibbonControl sourceRibbon) : base(manager, sourceRibbon) { }
		protected override RibbonViewInfo CreateViewInfo() {
			return new RibbonFullscreenMinimizedControlViewInfo(this);
		}
		protected override void Assign(RibbonControl srcRibbon) {
			base.Assign(srcRibbon);
			foreach(BarItemLink link in srcRibbon.Toolbar.ItemLinks) {
				BarItemLink cloned = Toolbar.ItemLinks.Add(link.Item);
				cloned.BeginGroup = link.BeginGroup;
			}
		}
	}
	[ToolboxItem(false)]
	public class RibbonMinimizedControl : CustomBaseRibbonControl {
		public RibbonMinimizedControl(RibbonBarManager manager, RibbonControl sourceRibbon) : base(manager, sourceRibbon) { }
		protected override RibbonViewInfo CreateViewInfo() { return new RibbonMinimizedControlViewInfo(this); }
		protected internal new RibbonMinimizedControlViewInfo ViewInfo {
			get { return base.ViewInfo as RibbonMinimizedControlViewInfo; }
		}
		protected override RibbonKeyTipManager CreateKeyTipManager() {
			return new MinimizedRibbonKeyTipManager(this);
		}
		protected internal override bool IsSystemLink(BarItemLink link) {
			return link == MDICloseItemLink || link == MDIMinimizeItemLink || link == MDIRestoreItemLink;
		}
		protected internal override void ActivateKeyTips() {
			KeyTipManager.ActivatePanelKeyTips();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!Visible) {
				KeyTipManager.HideKeyTips();
			}
		}
		protected override BarMenuCloseType RibbonShouldCloseMenuOnClick(MouseInfoArgs e) {
			Point p = PointToClient(new Point(e.X, e.Y));
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(p);
			if(hitInfo.HitTest == RibbonHitTest.Panel)
				return BarMenuCloseType.AllExceptMinimized;
			if(hitInfo.HitTest == RibbonHitTest.PanelRightScroll || hitInfo.HitTest == RibbonHitTest.PanelLeftScroll)
				return BarMenuCloseType.AllExceptMinimized;
			BarMenuCloseType res = base.RibbonShouldCloseMenuOnClick(e);
			if(res == BarMenuCloseType.None) return res;
			if(hitInfo.InPageGroup) {
				if(IsPopupFormOpened) {
					ClosePopupForms(false);
					return BarMenuCloseType.AllExceptMinimized;
				}
				return BarMenuCloseType.Standard;
			}
			if(RibbonStyle == RibbonControlStyle.Office2013 && hitInfo.HitTest == RibbonHitTest.None) {
				Point pos = PointToScreen(hitInfo.HitPoint);
				Screen screen = Screen.FromControl(this);
				if(screen != null) pos.X -= screen.Bounds.X;
				if(Bounds.Contains(pos) || SourceRibbon.Bounds.Contains(pos)) {
					return BarMenuCloseType.None;
				}
			}
			return BarMenuCloseType.All; 
		}
		protected override bool ShouldActivateKeyTipsWithNavigation { get { return false; } }
		protected override DevExpress.XtraBars.Ribbon.Handler.BaseHandler CreateHandler() {
			if(IsDesignMode) return new EmptyHandler();
			return new RibbonMinimizedHandler(this);
		}
		protected override void KeyboardSelectFirstObject() {
			NavigatableObjects = null;
			NavigatableObjectList = null;
			CheckViewInfo();
			if(NavigatableObjects != null && NavigatableObjects.Count > 0) {
				NavigationObjectRow row = NavigatableObjects[0];
				if(row.Count > 0) {
					ViewInfo.KeyboardActiveObject = row[0];
					return;
				}
			}
			this._isKeyboardActive = false;
		}
		protected internal override void DeactivateKeyboardNavigation(bool clearSelection, bool closePopupForms) {
			DeactivateNavigationCore();
			if(IsKeyboardActive && closePopupForms) {
				SourceRibbon.ClosePopupForms();
				return;
			}
		}
		public override bool Minimized {
			get { return SourceRibbon.Minimized; }
			set { SourceRibbon.Minimized = value; }
		}
		protected internal virtual bool IsFullScreenModeActive {
			get { return SourceRibbon != null && SourceRibbon.ViewInfo.IsPopupFullScreenModeActive; }
		}
		protected internal override void OnAddToToolbar(BarItemLink link) {
			SourceRibbon.OnAddToToolbar(link);
		}
		protected internal override void OnRemoveFromToolbar(BarItemLink link) {
			SourceRibbon.OnRemoveFromToolbar(link);
		}
		protected internal override RibbonQuickAccessToolbar SourceToolbar {
			get { return SourceRibbon.Toolbar; }
		}
		protected internal override void OnChangeQuickToolbarPosition() {
			SourceRibbon.OnChangeQuickToolbarPosition();
		}
		public override RibbonQuickAccessToolbarLocation ToolbarLocation {
			get { return SourceRibbon.ToolbarLocation; }
			set { SourceRibbon.ToolbarLocation = value; }
		}
	}
	[ToolboxItem(false)]
	public class RibbonOneGroupControl : CustomBaseRibbonControl {
		public RibbonOneGroupControl(RibbonBarManager manager, RibbonControl sourceRibbon) : base(manager, sourceRibbon) { 
			ShowPageHeadersMode = ShowPageHeadersMode.Hide;
		}
		protected internal override bool IsExpandButtonInPanel {
			get { return false; }
		}
		protected override RibbonViewInfo CreateViewInfo() {
			return new RibbonOneGroupControlViewInfo(this);
		}
		protected internal new RibbonOneGroupControlViewInfo ViewInfo {
			get { return base.ViewInfo as RibbonOneGroupControlViewInfo; }
		}
		protected internal override bool IsSystemLink(BarItemLink link) {
			return link == MDICloseItemLink || link == MDIMinimizeItemLink || link == MDIRestoreItemLink;
		}
		protected override DevExpress.XtraBars.Ribbon.Handler.BaseHandler CreateHandler() {
			if(IsDesignMode) return new EmptyHandler();
			return new RibbonPopupGroupHandler(this); 
		}
		protected internal override void OnAddToToolbar(BarItemLink link) {
			SourceRibbon.OnAddToToolbar(link);
		}
		protected internal override void OnRemoveFromToolbar(BarItemLink link) {
			SourceRibbon.OnRemoveFromToolbar(link);
		}
		protected internal override RibbonQuickAccessToolbar SourceToolbar {
			get { return SourceRibbon.SourceToolbar; }
		}
		protected override RibbonKeyTipManager CreateKeyTipManager() {
			return new RibbonPopupKeyTipManager(this);
		}
		protected override bool ShouldActivateKeyTipsWithNavigation { get { return false; } }
		protected override BarMenuCloseType RibbonShouldCloseMenuOnClick(MouseInfoArgs e) {
			Point p = PointToClient(new Point(e.X, e.Y));
			RibbonHitInfo hitInfo = ViewInfo.CalcHitInfo(p);
			if(hitInfo.HitTest == RibbonHitTest.PageGroupCaptionButton) {
				Manager.SelectionInfo.CloseAllPopups();
				return BarMenuCloseType.None;
			}
			else if(hitInfo.HitTest == RibbonHitTest.PageGroupCaption || hitInfo.HitTest == RibbonHitTest.PageGroup) 
				return BarMenuCloseType.None;
			return base.RibbonShouldCloseMenuOnClick(e);
		}
		protected internal override void DeactivateKeyboardNavigation(bool clearSelection, bool closePopupForms) {
			DeactivateNavigationCore();
			if(IsKeyboardActive && closePopupForms) {
				SourceRibbon.ClosePopupForms();
				return;
			}
		}
		protected override void KeyboardSelectFirstObject() {
			NavigatableObjects = null;
			NavigatableObjectList = null;
			CheckViewInfo();
			if(NavigatableObjects != null && NavigatableObjects.Count > 0) {
				NavigationObjectRow row = NavigatableObjects[0];
				if(row.Count > 0) {
					ViewInfo.KeyboardActiveObject = row[0];
					return;
				}
			}
			this._isKeyboardActive = false;
		}
		public override bool Minimized {
			get { return SourceRibbon.Minimized; }
			set { SourceRibbon.Minimized = value; }
		}
		protected internal override void OnChangeQuickToolbarPosition() {
			SourceRibbon.OnChangeQuickToolbarPosition();
		}
		public override RibbonQuickAccessToolbarLocation ToolbarLocation {
			get { return SourceRibbon.ToolbarLocation; }
			set { SourceRibbon.ToolbarLocation = value; }
		}
	}
	[ToolboxItem(false)]
	public class RibbonPopupToolbarControl : RibbonControl {
		RibbonControl sourceRibbon;
		public RibbonPopupToolbarControl(RibbonControl sourceRibbon) {
			RibbonPopupToolbarControl popupRibbon = sourceRibbon as RibbonPopupToolbarControl;
			this.sourceRibbon = sourceRibbon;
			Toolbar.ShowCustomizeItem = SourceRibbon.Toolbar.ShowCustomizeItem;
			AllowCustomization = SourceRibbon.AllowCustomization;		 
			SetupItems();
			ShowPageHeadersMode = ShowPageHeadersMode.Hide;
			ItemsVertAlign = sourceRibbon.ItemsVertAlign;
			ButtonGroupsVertAlign = sourceRibbon.ButtonGroupsVertAlign;
			AutoSizeItems = sourceRibbon.AutoSizeItems;
			RibbonStyle = sourceRibbon.RibbonStyle;
			if(IsOfficeTablet)
				RibbonStyle = RibbonControlStyle.Office2013;
			AutoHideEmptyItems = sourceRibbon.AutoHideEmptyItems;
			OptionsTouch.Assign(sourceRibbon.OptionsTouch);
			OptionsTouch.ShowTouchUISelectorInQAT = false;
		}
		protected internal override bool IsExpandButtonInPanel {
			get { return false; }
		}
		protected internal override bool IsSystemLink(BarItemLink link) {
			return link == MDICloseItemLink || link == MDIMinimizeItemLink || link == MDIRestoreItemLink;
		}
		protected internal override bool AllowChangeToolbarLocationMenuItem {
			get {
				return ShowToolbarCustomizeItem && ToolbarLocation != RibbonQuickAccessToolbarLocation.Hidden &&
				ShowQatLocationSelector && ViewInfo.Toolbar.SupportForRibbonStyle(GetRibbonStyle()) && sourceRibbon.GetRibbonStyle() != RibbonControlStyle.TabletOffice;
			}
		}
		public override RightToLeft RightToLeft {
			get {
				if(SourceRibbon == null) return base.RightToLeft;
				return SourceRibbon.RightToLeft;
			}
			set { base.RightToLeft = value; }
		}
		protected override bool ShouldInitOrDestroyBarManager { get { return false; } }
		protected internal override void OnChangeQuickToolbarPosition() {
			if(SourceRibbon is RibbonPopupToolbarControl) {
				SourceRibbon.OnChangeQuickToolbarPosition();
				return;
			}
			if(SourceRibbon.ViewInfo.GetToolbarLocation() == RibbonQuickAccessToolbarLocation.Below)
				SourceRibbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Above;
			else
				SourceRibbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Below;
		}
		public RibbonControl SourceRibbon { get { return sourceRibbon; } }
		public Size CalcPopupSize() {
			CheckViewInfo();
			return ((RibbonViewInfoPopupToolbarOnly)ViewInfo).CalcPopupToolbarSize(); 
		}
		public RibbonQuickAccessToolbarViewInfo SourceToolbarInfo { get { return SourceRibbon.ViewInfo.Toolbar; } }
		protected override RibbonBarManager CreateBarManager() { return null; }
		public override RibbonBarManager Manager { get { return SourceRibbon == null ? null : SourceRibbon.Manager; } }
		protected override RibbonViewInfo CreateViewInfo() {
			return new RibbonViewInfoPopupToolbarOnly(this, SourceToolbarInfo);
		}
		protected internal virtual void SetupItems() {
			for(int i = SourceToolbarInfo.VisibleButtonCount; i < SourceToolbarInfo.Items.Count; i++) {
				RibbonItemViewInfo itemInfo = SourceToolbarInfo.Items[i];
				if(SourceToolbarInfo.IsToolbarCustomizationItem(i))
					continue;
				if(itemInfo.IsSeparator) continue;
				Toolbar.ItemLinks.Add(((BarItemLink)itemInfo.Item).Item);
			}
		}
		protected internal override void OnRemoveFromToolbar(BarItemLink link) {
			if(SourceRibbon.Toolbar.Contains(link.Item)) SourceRibbon.Toolbar.Remove(link.Item);
		}
		public override RibbonQuickAccessToolbarLocation ToolbarLocation {
			get { return SourceRibbon.ToolbarLocation; }
			set { SourceRibbon.ToolbarLocation = value; }
		}
		protected internal override RibbonQuickAccessToolbar SourceToolbar {
			get { return SourceRibbon.SourceToolbar; }
		}
	}
	public class RibbonConverter {
		public virtual void ConvertToRibbon(BarManager manager, RibbonControl ribbon) {
			if(manager == null || ribbon == null) throw new ArgumentNullException();
			ManagerBeginUpdate(manager);
			ConvertRefProperties(manager, ribbon);
			ConvertCategories(manager, ribbon);
			ConvertItems(manager, ribbon);
			ConvertRepositoryItems(manager, ribbon);
			ConvertToolbars(manager, ribbon);
			ConvertMenus(manager, ribbon);
			CleanUp(manager);
			ribbon.CheckHeight();
		}
		IContainer GetContainer(BarManager manager) {
			if(manager.Site == null) return null;
			return manager.Site.Container;
		}
		void ConvertMenus(BarManager manager, RibbonControl ribbon) {
			IContainer container = GetContainer(manager);
			if(container == null) return;
			foreach(IComponent component in container.Components) {
				PopupMenuBase menu = component as PopupMenuBase;
				PopupControlContainer pc = component as PopupControlContainer;
				if(menu != null && menu.Manager == manager) {
					TypeDescriptor.GetProperties(menu)["Ribbon"].SetValue(menu, ribbon);
				}
				if(pc != null && pc.Manager == manager) {
					TypeDescriptor.GetProperties(pc)["Ribbon"].SetValue(pc, ribbon);
				}
			}
		}
		void ManagerBeginUpdate(BarManager manager) {
			for(int n = 0; n < manager.Bars.Count; n++) {
				manager.Bars[n].BeginUpdate();
			}
			foreach(BarDockControl dock in manager.DockControls) {
				dock.BeginUpdate();
			}
		}
		protected virtual void CleanUp(BarManager manager) {
			manager.Items.RibbonClearItems();
			manager.Dispose();
		}
		protected virtual void ConvertRefProperties(BarManager manager, RibbonControl ribbon) {
			ribbon.LargeImages = manager.LargeImages;
			ribbon.Images = manager.Images;
			ribbon.ItemClick += (ItemClickEventHandler)manager.GetEventItemClick();
			ribbon.ItemPress += (ItemClickEventHandler)manager.GetEventItemPress();
			foreach(DictionaryEntry entry in manager.PopupContextMenus)
				ribbon.Manager.PopupContextMenus.Add(entry.Key, entry.Value);
		}
		protected virtual void ConvertCategories(BarManager manager, RibbonControl ribbon) {
			foreach(BarManagerCategory category in manager.Categories) {
				ribbon.Categories.Add(category);
			}
		}
		protected virtual void ConvertRepositoryItems(BarManager manager, RibbonControl ribbon) {
			while(manager.RepositoryItems.Count > 0) {
				RepositoryItem item = manager.RepositoryItems[0];
				manager.RepositoryItems.RemoveAt(0);
				ribbon.RepositoryItems.Add(item);
			}
		}
		protected virtual void ConvertItems(BarManager manager, RibbonControl ribbon) {
			ArrayList list = new ArrayList(manager.Items), unsupported = new ArrayList();
			for(int n = list.Count - 1; n >=0 ; n--) {
				BarItem item = (BarItem)list[n];
				if(!CanConvert(item)) {
					unsupported.Add(item);
					list.RemoveAt(n);
					continue;
				}
				item.RibbonSetManager(ribbon.Manager);
			}
			BarItem[] items = list.ToArray(typeof(BarItem)) as BarItem[];
			ribbon.Items.AddRange(items);
			DestroyItems(manager, unsupported);
		}
		protected virtual void DestroyItems(BarManager manager, ArrayList list) {
			if(list.Count == 0) return;
			foreach(BarItem item in list) {
				item.Dispose();
			}
		}
		protected virtual void ConvertToolbars(BarManager manager, RibbonControl ribbon) {
			if(manager.MainMenu != null) ConvertMainMenu(manager, ribbon, manager.MainMenu);
			RibbonPage page = null;
			foreach(Bar bar in manager.Bars) {
				if(bar.IsMainMenu) continue;
				if(bar.IsStatusBar) {
					ConvertStatusBar(manager, ribbon, bar);
					continue;
				}
				if(page == null) {
					page = new RibbonPage("ConvertedFromBarManager");
					AddToContainer(ribbon, page);
					ribbon.Pages.Add(page);
				}
				ConvertBar(manager, ribbon, page, bar);
			}
		}
		protected virtual bool CanConvert(BarItem item) {
			BarItemInfo info = BarAndDockingController.Default.PaintStyle.ItemInfoCollection.GetInfoByItem(item.GetType());
			if(info == null) return false;
			if(info.SupportRibbon) return true;
			if(item is BarLargeButtonItem) return true; 
			return false;
		}
		protected virtual void ConvertBar(BarManager manager, RibbonControl ribbon, RibbonPage page, Bar bar) {
			RibbonPageGroup group = new RibbonPageGroup(bar.Text.Replace("&", ""));
			AddToContainer(ribbon, group);
			foreach(BarItemLink link in bar.ItemLinks) {
				BarItemLink rlink = group.ItemLinks.Add(link.Item);
				rlink.BeginGroup = link.BeginGroup;
			}
			page.Groups.Add(group);
		}
		protected virtual void ConvertMainMenu(BarManager manager, RibbonControl ribbon, Bar mainMenu) {
			RibbonPage page = null;
			RibbonPageGroup otherGroup = null;
			foreach(BarItemLink link in mainMenu.ItemLinks) {
				BarSubItemLink menuLink = link as BarSubItemLink;
				if(page == null) {
					page = new RibbonPage("MainMenu");
					AddToContainer(ribbon, page);
					ribbon.Pages.Add(page);
				}
				if(menuLink == null) {
					if(otherGroup == null) {
						otherGroup = new RibbonPageGroup("MainMenu");
						AddToContainer(ribbon, otherGroup);
						page.Groups.Add(otherGroup);
					}
					BarItemLink rlink = otherGroup.ItemLinks.Add(link.Item);
					rlink.BeginGroup = link.BeginGroup;
					continue;
				}
				RibbonPageGroup group = new RibbonPageGroup(menuLink.Item.Caption.Replace("&", ""));
				AddToContainer(ribbon, group);
				foreach(BarItemLink il in menuLink.Item.ItemLinks) {
					BarItemLink rlink = group.ItemLinks.Add(il.Item);
					rlink.BeginGroup = il.BeginGroup;
				}
				page.Groups.Add(group);
			}
		}
		protected virtual void ConvertStatusBar(BarManager manager, RibbonControl ribbon, Bar statusBar) {
			RibbonStatusBar sb = new RibbonStatusBar(ribbon);
			AddToContainer(ribbon, sb);
			ribbon.Parent.Controls.Add(sb);
			foreach(BarItemLink link in statusBar.ItemLinks) {
				BarItemLink rlink = sb.ItemLinks.Add(link.Item);
				rlink.BeginGroup = link.BeginGroup;
			}
			sb.Parent = ribbon.Parent;
		}
		protected virtual void AddToContainer(RibbonControl ribbon, IComponent component) {
			if(ribbon.Site != null && ribbon.Site.Container != null) ribbon.Site.Container.Add(component);
		}
	}
	public class RibbonStatusBarDesignTimeManager : BaseRibbonDesignTimeManager {
		bool isRightPart = false;
		public RibbonStatusBarDesignTimeManager() { }
		public RibbonStatusBarDesignTimeManager(RibbonStatusBar statusBar)
			: base(statusBar, null) { }
		public override RibbonControl Ribbon { get { return Owner.Ribbon; } }
		public new RibbonStatusBar Owner { get { return base.Owner as RibbonStatusBar; } }
		protected override RibbonHitInfo CalcHitInfo(Point p) { return Owner.CalcHitInfo(p); }
		public override ISite Site { get { return Owner == null ? null : Owner.Site; } }
		protected virtual bool IsInRightPart(RibbonHitInfo hitInfo) {
			if(hitInfo.HitTest == RibbonHitTest.StatusBar) return Owner.ViewInfo.DesignerRightRect.Contains(hitInfo.HitPoint);
			if(hitInfo.InItem) return hitInfo.Item.Alignment == BarItemLinkAlignment.Right;
			return false;
		}
		protected internal override bool ProcessRightMouseDown(MouseEventArgs e, RibbonHitInfo hitInfo) {
			this.lastButtonGroup = GetLastButtonGroup(hitInfo);
			if(hitInfo.HitTest == RibbonHitTest.StatusBar || hitInfo.InItem) {
				isRightPart = IsInRightPart(hitInfo);
				UpdateGroupMenu(true, this.lastButtonGroup == null);
				PopulateEditorsMenu();
				MenuManagerHelper.StandardEx.ShowPopupMenu(this.groupMenu, Owner, new Point(e.X, e.Y));
				return true;
			}
			return base.ProcessRightMouseDown(e, hitInfo);
		}
		protected override BarItemLink GetLinkByPoint(MouseEventArgs e) {
			return Owner.ViewInfo.GetLinkByPoint(e.Location, EnterButtonGroup);
		}
		protected override bool AllowMenu { get { return SelectionService != null && Owner != null && DesignerHost != null && DesignerHost.Container != null; } }
		protected override BarItem CreateItem(object sender) {
			DXMenuItem item = (DXMenuItem)sender;
			BarItemInfo info = (BarItemInfo)item.Tag;
			BarItem bi = null;
			if (info != null)
			{
				bi = CreateItemByInfo(info);
				bi.Alignment = isRightPart ? BarItemLinkAlignment.Right : BarItemLinkAlignment.Default;
				if(lastButtonGroup != null) lastButtonGroup.ItemLinks.Add(bi);
				else Owner.ItemLinks.Add(bi);
				SelectComponentDelayed(bi);
				if(lastButtonGroup != null) FireChanged(this.lastButtonGroup);
				else FireChanged(Owner);
			}
			this.lastButtonGroup = null;
			return bi;
		}
		protected override void OnAddItem(object sender, EventArgs e) {
			if(!AllowMenu) return;
			CreateItem(sender);
		}
		protected override void StartDragging(MouseEventArgs e) {
			DragManager.StartDragging(Owner, e, DragItem, Owner);   
		}
	}
	public class RibbonDesignTimeDXMenuItem : DXMenuItem {
		public RibbonDesignTimeDXMenuItem(string name, EventHandler click) : base(name, click) { }
		public bool IsAddPageGroupItem { get; set; }
	}
	public class RibbonDesignTimeManager : BaseRibbonDesignTimeManager {
		DXPopupMenu pageMenu, galleryMenu, galleryGroupMenu, galleryItemMenu, applicationButtonMenu, galleryDropDownMenu;
		ISite site;
		static int galleryItemIndex = 1, galleryGroupIndex = 1;
		public RibbonDesignTimeManager() { }
		public RibbonDesignTimeManager(Control owner, ISite site)
			: base(owner, site) {
		}
		public override void Initialize(object control, ISite site) {
			base.Initialize(control, site);
			this.site = site;
			this.pageMenu = CreatePageMenu();
			this.galleryMenu = CreateGalleryMenu();
			this.galleryGroupMenu = CreateGalleryGroupMenu();
			this.galleryItemMenu = CreateGalleryItemMenu();
			this.applicationButtonMenu = CreateApplicationButtonMenu();
			this.galleryDropDownMenu = CreateGalleryDropDownMenu();
		}
		public override ISite Site { get { return site == null ? Owner.Site : site; } }
		public override RibbonControl Ribbon { get { return Owner as RibbonControl; } }
		public GalleryControl GalleryControl { get { return Owner as GalleryControl; } }
		public static int GalleryGroupIndex { get { return galleryGroupIndex; } set { galleryGroupIndex = value; } }
		public static int GalleryItemIndex { get { return galleryItemIndex; } set { galleryItemIndex = value; } }
		public static string GetGalleryGroupName() {
			string res = "Group" + GalleryGroupIndex;
			GalleryGroupIndex++;
			return res;
		}
		public static string GetGalleryItemName() {
			string res = "Item" + GalleryItemIndex;
			GalleryItemIndex++;
			return res;
		}
		public override void Dispose() {
			if(this.pageMenu != null) this.pageMenu.Dispose();
			if(this.galleryMenu != null) this.galleryMenu.Dispose();
			if(this.galleryGroupMenu != null) this.galleryGroupMenu.Dispose();
			if(this.galleryItemMenu != null) this.galleryItemMenu.Dispose();
			if(this.applicationButtonMenu != null) this.applicationButtonMenu.Dispose();
			if(this.galleryDropDownMenu != null) this.galleryDropDownMenu.Dispose();
			base.Dispose();
		}
		protected virtual RibbonGalleryBarItem GetLastGallery(RibbonHitInfo hitInfo) {
			if(!hitInfo.InGallery) return null;
			return (hitInfo.Item as RibbonGalleryBarItemLink).Item;
		}
		protected virtual GalleryControlGallery GetLastControlGallery(RibbonHitInfo hitInfo) {
			return hitInfo.Gallery as GalleryControlGallery;
		}
		protected virtual InDropDownGallery GetLastDropDownGallery(RibbonHitInfo hitInfo) {
			return hitInfo.Gallery as InDropDownGallery;
		}
		protected virtual GalleryItemGroup GetLastGalleryGroup(RibbonHitInfo hitInfo) {
			if(!hitInfo.InGalleryGroup) return null;
			return hitInfo.GalleryItemGroup;
		}
		protected virtual RibbonPageCategory GetLastCategory(RibbonHitInfo hitInfo) {
			if(!hitInfo.InPageCategory) return null;
			return hitInfo.PageCategory;
		}
		protected virtual GalleryItem GetLastGalleryItem(RibbonHitInfo hitInfo) {
			if(!hitInfo.InGalleryItem) return null;
			return hitInfo.GalleryItem;
		}
		protected virtual RibbonGalleryBarItem GetLastGalleryBarItem(RibbonHitInfo hitInfo) { 
			if(!hitInfo.InGallery || hitInfo.Item == null) return null;
			return (hitInfo.Item as RibbonGalleryBarItemLink).Item;
		}
		protected virtual RibbonGalleryBarItemLink GetLastGalleryBarItemLink(RibbonHitInfo hitInfo) {
			if(!hitInfo.InGallery || hitInfo.Item == null) return null;
			return (hitInfo.Item as RibbonGalleryBarItemLink);
		}
		protected virtual Control GetMenuOwnerControl(BaseGallery gallery) {
			InDropDownGallery ddGallery = gallery as InDropDownGallery;
			GalleryControlGallery cgallery = gallery as GalleryControlGallery;
			if(ddGallery != null) return ddGallery.BarControl;
			if(cgallery != null) return cgallery.GalleryControl;
			return Owner;
		}
		protected internal override bool ProcessRightMouseDown(MouseEventArgs e, RibbonHitInfo hitInfo) {
			if(hitInfo.Item != null && hitInfo.Item is SkinRibbonGalleryBarItemLink) return false;
			if(hitInfo.HitTest == RibbonHitTest.PageHeader || (Ribbon != null && Ribbon.SelectedPage == null) ||
				hitInfo.HitTest == RibbonHitTest.Panel) {
				this.lastCategory = null;
				this.lastPage = hitInfo.Page;
				MenuManagerHelper.StandardEx.ShowPopupMenu(this.pageMenu, Ribbon, new Point(e.X, e.Y));
				return true;
			}
			if(hitInfo.InPageCategory) {
				this.lastCategory = GetLastCategory(hitInfo);
				this.lastPage = null;
				MenuManagerHelper.StandardEx.ShowPopupMenu(this.pageMenu, Ribbon, new Point(e.X, e.Y));
				return true;
			}
			if(hitInfo.HitTest == RibbonHitTest.GalleryDropDownButton) {
				RibbonGalleryBarItem gitem = GetLastGallery(hitInfo);
				this.lastGallery = gitem == null ? null : gitem.Gallery;
				if(this.lastGallery != null && this.lastGallery.GalleryDropDown == null) {
					MenuManagerHelper.StandardEx.ShowPopupMenu(this.galleryDropDownMenu, Ribbon, new Point(e.X, e.Y));
					return true;
				}
			}
			if(hitInfo.InGalleryItem) {
				this.lastGalleryItemLink = GetLastGalleryBarItemLink(hitInfo);
				this.lastGalleryBarItem = GetLastGalleryBarItem(hitInfo);
				this.lastGalleryControlGallery = GetLastControlGallery(hitInfo);
				this.lastDropDownGallery = GetLastDropDownGallery(hitInfo); 
				this.lastGalleryItemGroup = GetLastGalleryGroup(hitInfo);
				this.lastGalleryItem = GetLastGalleryItem(hitInfo);
				if(lastDropDownGallery != null && lastDropDownGallery.GalleryDropDown.OwnerGallery != null) return false;
				MenuManagerHelper.StandardEx.ShowPopupMenu(this.galleryItemMenu, GetMenuOwnerControl(lastDropDownGallery), new Point(e.X, e.Y));
				return true;
			}
			if(hitInfo.InGalleryGroup) {
				this.lastGalleryItemLink = GetLastGalleryBarItemLink(hitInfo);
				this.lastGalleryBarItem = GetLastGalleryBarItem(hitInfo);
				this.lastDropDownGallery = GetLastDropDownGallery(hitInfo);
				this.lastGalleryControlGallery = GetLastControlGallery(hitInfo);
				this.lastGalleryItemGroup = GetLastGalleryGroup(hitInfo);
				if(lastDropDownGallery != null && lastDropDownGallery.GalleryDropDown.OwnerGallery != null) return false;
				MenuManagerHelper.StandardEx.ShowPopupMenu(this.galleryGroupMenu, GetMenuOwnerControl(lastDropDownGallery), new Point(e.X, e.Y));
				return true;
			}
			if(hitInfo.InGallery) {
				this.lastGalleryItemLink = GetLastGalleryBarItemLink(hitInfo);
				this.lastGalleryBarItem = GetLastGalleryBarItem(hitInfo);
				this.lastDropDownGallery = GetLastDropDownGallery(hitInfo);
				this.lastGalleryControlGallery = GetLastControlGallery(hitInfo);
				MenuManagerHelper.StandardEx.ShowPopupMenu(this.galleryMenu, GetMenuOwnerControl(lastDropDownGallery), new Point(e.X, e.Y));
				return true;
			}
			if(hitInfo.HitTest == RibbonHitTest.ApplicationButton) {
				if(Ribbon.ApplicationButtonDropDownControl != null) return true;
				MenuManagerHelper.StandardEx.ShowPopupMenu(this.applicationButtonMenu, Ribbon, new Point(e.X, e.Y));
			}
			if(hitInfo.InPageGroup || Ribbon.ViewInfo.Toolbar.DesignerRect.Contains(hitInfo.HitPoint) || Ribbon.ViewInfo.Header.DesignerRect.Contains(hitInfo.HitPoint)) {
				this.lastMenuGroup = hitInfo.PageGroup;
				this.lastButtonGroup = GetLastButtonGroup(hitInfo);
				this.lastPageHeader = GetLastPageHeader(hitInfo);
				this.lastToolbar = GetLastToolbar(hitInfo);
				UpdateGroupMenu(this.lastButtonGroup == null, this.lastButtonGroup == null);
				PopulateEditorsMenu();
				MenuManagerHelper.StandardEx.ShowPopupMenu(this.groupMenu, Ribbon, new Point(e.X, e.Y));
				return true;
			}
			return false;
		}
		protected override RibbonHitInfo CalcHitInfo(Point p) {
			if(Ribbon != null)
				return Ribbon.CalcHitInfo(p);
			if(GalleryControl != null)
				return GalleryControl.CalcHitInfo(p);
			return new RibbonHitInfo(); 
		}
		protected virtual DXPopupMenu CreateApplicationButtonMenu() {
			DXPopupMenu res = new DXPopupMenu();
			res.Items.Add(new DXMenuItem("Add Application Menu (Office 2007 Style Menu)", OnAddApplicationMenu));
			res.Items.Add(new DXMenuItem("Add Backstage View (Office 2010/13 Style Menu)", OnAddBackstageViewControl));
			return res;
		}
		protected virtual DXPopupMenu CreatePageMenu() {
			DXPopupMenu res = new DXPopupMenu(new EventHandler(OnBeforePopupPageMenu));
			foreach(RibbonRegistrationInfo info in Ribbon.RegistrationInfo) { 
				if(info.PageType != null)
					res.Items.Add(new RibbonDesignTimeDXMenuItem("Add " + info.GetPageName(), new EventHandler(OnAddPage)) { Tag = info });
			}
			foreach(RibbonRegistrationInfo info in Ribbon.RegistrationInfo) { 
				if(info.PageCategoryType != null)
					res.Items.Add(new RibbonDesignTimeDXMenuItem("Add " + info.GetPageCategoryName(), new EventHandler(OnAddPageCategory)) { Tag = info });
			}
			foreach(RibbonRegistrationInfo info in Ribbon.RegistrationInfo) {
				if(info.PageGroupType != null)
					res.Items.Add(new RibbonDesignTimeDXMenuItem("Add " + info.GetPageGroupName(), new EventHandler(OnAddPageGroup)) { Tag = info, IsAddPageGroupItem = true });
			}
			return res;
		}
		protected virtual DXPopupMenu CreateGalleryDropDownMenu() {
			DXPopupMenu res = new DXPopupMenu();
			res.Items.Add(new DXMenuItem("Add GalleryDropDown", new EventHandler(OnAddGalleryDropDown)));
			return res;
		}
		protected virtual DXPopupMenu CreateGalleryMenu() {
			DXPopupMenu res = new DXPopupMenu();
			res.Items.Add(new DXMenuItem("Add Gallery Group", new EventHandler(OnAddGalleryGroup)));
			return res;
		}
		protected virtual DXPopupMenu CreateGalleryGroupMenu() {
			DXPopupMenu res = new DXPopupMenu();
			res.Items.Add(new DXMenuItem("Add Gallery Item", new EventHandler(OnAddGalleryItem)));
			res.Items.Add(new DXMenuItem("Remove Gallery Group", new EventHandler(OnRemoveGalleryGroup)));
			return res;
		}
		protected virtual DXPopupMenu CreateGalleryItemMenu() {
			DXPopupMenu res = new DXPopupMenu();
			res.Items.Add(new DXMenuItem("Add Gallery Item", new EventHandler(OnAddGalleryItem)));
			res.Items.Add(new DXMenuItem("Remove Gallery Item", new EventHandler(OnRemoveGalleryItem)));
			return res;
		}
		void OnBeforePopupPageMenu(object sender, EventArgs e) {
			foreach(RibbonDesignTimeDXMenuItem item in ((DXPopupMenu)sender).Items) {
				if(item.IsAddPageGroupItem) {
					item.Visible = this.lastCategory == null;
					item.Enabled = Ribbon.SelectedPage != null;
				}
			}
		}
		RibbonPageGroup lastMenuGroup = null;
		RibbonGalleryBarItem lastGalleryBarItem = null;
		RibbonGalleryBarItemLink lastGalleryItemLink = null;
		GalleryItemGroup lastGalleryItemGroup = null;
		GalleryItem lastGalleryItem = null;
		RibbonPage lastPage = null;
		RibbonPageCategory lastCategory = null;
		InRibbonGallery lastGallery = null;
		InDropDownGallery lastDropDownGallery = null;
		GalleryControlGallery lastGalleryControlGallery = null;
		protected override bool AllowMenu { get { return SelectionService != null && Ribbon != null && DesignerHost != null && DesignerHost.Container != null; } }
		void OnAddApplicationMenu(object sender, EventArgs e) {
			AddApplicationMenu();
		}
		void OnAddBackstageViewControl(object sender, EventArgs e) {
			AddBackstageView();
		}
		public void AddApplicationMenu() {
			ApplicationMenu appMenu = new ApplicationMenu();
			Ribbon.ApplicationButtonDropDownControl = appMenu;
			Ribbon.Container.Add(appMenu);
			appMenu.Ribbon = Ribbon;
			FireChanged(appMenu);
			FireChanged(Ribbon);
		}
		public void AddBackstageView() {
			BackstageViewControl backstageView = new BackstageViewControl();
			Ribbon.ApplicationButtonDropDownControl = backstageView;
			Ribbon.Container.Add(backstageView);
			Control parent = Ribbon.Parent;
			if(parent != null) {
				parent.Controls.Add(backstageView);
			}
			backstageView.Location = new Point(Ribbon.Height / 2, Ribbon.Height + Ribbon.Height / 2); ;
			Size defaultSize = backstageView.DefaultSizeCore;
			backstageView.Size = new Size(defaultSize.Width * 2, defaultSize.Height);
			BackstageViewTabItem item = new BackstageViewTabItem();
			backstageView.Container.Add(item.ContentControl);
			backstageView.Container.Add(item);
			backstageView.Items.Add(item);
			item.Caption = item.Name;
			backstageView.Ribbon = Ribbon;
			FireChanged(backstageView);
			FireChanged(Ribbon);
		}
		public virtual GalleryDropDown AddGalleryDropDown() {
			if(Ribbon == null) return null;
			GalleryDropDown dropDown = new GalleryDropDown();
			dropDown.Ribbon = Ribbon;
			Ribbon.Container.Add(dropDown);
			return dropDown;
		}
		void OnRemoveGalleryGroup(object sender, EventArgs e) {
			if((lastGalleryBarItem == null && lastDropDownGallery == null && lastGalleryControlGallery == null) || lastGalleryItemGroup == null) return;
			RemoveGalleryGroupCore(lastGalleryBarItem, lastDropDownGallery, lastGalleryControlGallery, lastGalleryItemGroup);
		}
		protected internal void RemoveGalleryGroupCore(RibbonGalleryBarItem galleryBarItem, InDropDownGallery inDropDownGallery, GalleryControlGallery galleryControlGallery, GalleryItemGroup itemGroup) {
			if(galleryBarItem != null) {
				galleryBarItem.Gallery.Groups.Remove(itemGroup);
				FireChanged(galleryBarItem);
			}
			else if(galleryControlGallery != null) {
				galleryControlGallery.Groups.Remove(itemGroup);
				FireChanged(galleryControlGallery.GalleryControl);
			}
			else {
				inDropDownGallery.Groups.Remove(itemGroup);
				FireChanged(inDropDownGallery);
			}
			DragObject = null;
		}
		void OnAddGalleryGroup(object sender, EventArgs e) {
			if(lastGalleryBarItem == null && lastDropDownGallery == null && lastGalleryControlGallery == null) return;
			GalleryItemGroup group = OnAddGalleryGroupCore(lastGalleryBarItem, lastDropDownGallery, lastGalleryControlGallery);
			SelectedGalleryObject = new GalleryObjectDescriptor(group, GalleryParentComponent, this.lastGalleryItemLink);
			SelectComponentDelayed(SelectedGalleryObject);
		}
		public GalleryItemGroup OnAddGalleryGroupCore(RibbonGalleryBarItem galleryBarItem, BaseGallery inDropDownGallery, GalleryControlGallery galleryControlGallery) {
			GalleryItemGroup group = new GalleryItemGroup();
			group.Caption = GetGalleryGroupName();
			if(galleryBarItem != null) {
				galleryBarItem.Gallery.Groups.Add(group);
				FireChanged(galleryBarItem);
			}
			else if(galleryControlGallery != null) {
				galleryControlGallery.Groups.Add(group);
				FireChanged(galleryControlGallery.GalleryControl);
			}
			else {
				inDropDownGallery.Groups.Add(group);
				FireChanged(inDropDownGallery);
			}
			return group;
		}
		void OnRemoveGalleryItem(object sender, EventArgs e) {
			if((lastGalleryBarItem == null && lastDropDownGallery == null && lastGalleryControlGallery == null) || lastGalleryItemGroup == null || lastGalleryItem == null) return;
			RemoveGalleryItemCore(lastGalleryBarItem, lastDropDownGallery, lastGalleryControlGallery, lastGalleryItemGroup, lastGalleryItem);
		}
		protected internal void RemoveGalleryItemCore(RibbonGalleryBarItem item, InDropDownGallery inDropDownGallery, GalleryControlGallery galleryControlGallery, GalleryItemGroup itemGroup, GalleryItem galleryItem) {
			itemGroup.Items.Remove(galleryItem);
			if(item != null) FireChanged(item);
			else if(galleryControlGallery != null) FireChanged(galleryControlGallery.GalleryControl);
			else FireChanged(inDropDownGallery);
			DragObject = null;
		}
		void OnAddGalleryItem(object sender, EventArgs e) {
			if(lastGalleryItemGroup == null && lastDropDownGallery == null && lastGalleryControlGallery == null) return;
			GalleryItem item = OnAddGalleryItemCore(lastGalleryItemGroup);
			SelectedGalleryObject = new GalleryObjectDescriptor(item, GalleryParentComponent, this.lastGalleryItemLink);
			SelectComponentDelayed(SelectedGalleryObject);
		}
		protected internal GalleryItem OnAddGalleryItemCore(GalleryItemGroup group) {
			GalleryItem item = new GalleryItem();
			item.Caption = GetGalleryItemName();
			group.Items.Add(item);
			FireChanged(group.Gallery);
			return item;
		}
		protected override BarItem CreateItem(object sender) {
			DXMenuItem item = (DXMenuItem)sender;
			BarItemInfo info = (BarItemInfo)item.Tag;
			BarItem bi = null;
			if (info != null)
			{
				bi = CreateItemByInfo(info);
				if(lastButtonGroup != null)
					lastButtonGroup.ItemLinks.Add(bi);
				else if(lastPageHeader != null)
					lastPageHeader.Add(bi);
				else if(lastToolbar != null)
					lastToolbar.Add(bi);
				else
					lastMenuGroup.ItemLinks.Add(bi);
				SelectComponentDelayed(bi);
				if(lastButtonGroup != null)
					FireChanged(lastButtonGroup);
				else if(lastPageHeader != null || lastToolbar != null)
					FireChanged(Ribbon);
				else
					FireChanged(lastMenuGroup);
			}
			this.lastMenuGroup = null;
			this.lastButtonGroup = null;
			return bi;
		}
		protected override void OnAddItem(object sender, EventArgs e) {
			if(!AllowMenu || (this.lastMenuGroup == null && this.lastToolbar == null && this.lastPageHeader == null)) return;
			CreateItem(sender);
		}
		void OnAddGalleryDropDown(object sender, EventArgs e) {
			if(this.lastGallery == null || this.lastGallery.GalleryDropDown != null) return;
			GalleryDropDown gdd = DesignerHost.CreateComponent(typeof(GalleryDropDown)) as GalleryDropDown;
			if(gdd == null) return;
			this.lastGallery.GalleryDropDown = gdd;
			FireChanged(Ribbon);
		}
		public void OnAddPage(object sender, RibbonRegistrationInfo info) {
			if(!AllowMenu) return;
			RibbonPage page = CreateRibbonPage(info);
			if(page == null) return;
			if(this.lastCategory != null) this.lastCategory.Pages.Add(page);
			else if(lastPage == null || lastPage.IsInDefaultCategory)
				Ribbon.Pages.Add(page);
			else if(lastPage != null)
				lastPage.Category.Pages.Add(page);
			Ribbon.SelectedPage = page;
			SelectComponentDelayed(page);
			FireChanged(Ribbon);
		}
		public void OnAddPage(object sender, EventArgs e) {
			OnAddPage(sender, (RibbonRegistrationInfo)((RibbonDesignTimeDXMenuItem)sender).Tag);
		}
		protected internal virtual RibbonPage CreateRibbonPage(RibbonRegistrationInfo info) {
			RibbonPage page = DesignerHost.CreateComponent(info.PageType != null? info.PageType: Ribbon.DefaultPageType) as RibbonPage;
			if(page == null) return null;
			page.Text = page.Name;
			return page;
		}
		internal void OnAddPageCategory(object sender, RibbonRegistrationInfo info) {
			if(!AllowMenu) return;
			RibbonPageCategory category = CreateRibbonPageCategory(info);
			if(category == null) return;
			Ribbon.PageCategories.Add(category);
			SelectComponentDelayed(category);
			FireChanged(Ribbon);
		}
		internal void OnAddPageCategory(object sender, EventArgs e) {
			OnAddPageCategory(sender, (RibbonRegistrationInfo)((RibbonDesignTimeDXMenuItem)sender).Tag);
		}
		protected internal virtual RibbonPageCategory CreateRibbonPageCategory(RibbonRegistrationInfo info) {
			RibbonPageCategory category = DesignerHost.CreateComponent(info.PageCategoryType != null? info.PageCategoryType: Ribbon.DefaultPageCategoryType) as RibbonPageCategory;
			if(category == null) return null;
			category.Text = category.Name;
			return category;
		}
		public void OnAddPageGroup(object sender, RibbonRegistrationInfo info) {
			if(!AllowMenu || Ribbon.SelectedPage == null) return;
			RibbonPageGroup group = CreateRibbonPageGroup(info);
			Ribbon.SelectedPage.Groups.Add(group);
			SelectComponentDelayed(group);
			FireChanged(Ribbon.SelectedPage);
		}
		public void OnAddPageGroup(object sender, EventArgs e) {
			OnAddPageGroup(sender, (RibbonRegistrationInfo)((RibbonDesignTimeDXMenuItem)sender).Tag);
		}
		protected internal virtual RibbonPageGroup CreateRibbonPageGroup(RibbonRegistrationInfo info) {
			RibbonPageGroup group = DesignerHost.CreateComponent(info.PageGroupType != null? info.PageGroupType: Ribbon.DefaultPageGroupType) as RibbonPageGroup;
			if(group == null) return null;
			group.Text = group.Name;
			return group;
		}
		public void OnAddStatusBar(object sender, EventArgs e) {
			RibbonStatusBar statusBar = DesignerHost.CreateComponent(typeof(RibbonStatusBar)) as RibbonStatusBar;
			if(statusBar == null) return;
			statusBar.Text = statusBar.Name;
			statusBar.Ribbon = Ribbon;
			Ribbon.StatusBar = statusBar;
			Control topControl = DesignerHost.RootComponent as Control;
			if(topControl != null) {
				topControl.Controls.Add(statusBar);
				FireChanged(topControl);
			}
			SelectComponentDelayed(statusBar);
			FireChanged(statusBar);
		}
		protected RibbonGalleryBarItemLink GetSelectedGalleryBarItemLink() {
			if(Ribbon == null) return null;
			RibbonHitInfo hitInfo = Ribbon.CalcHitInfo(Ribbon.GetLastHitPoint());
			return hitInfo.Item as RibbonGalleryBarItemLink;
		}
		public virtual void SelectRibbonGalleryGroup(RibbonGalleryBarItem item, GalleryItemGroup group) {
			RibbonGalleryBarItemLink link = GetSelectedGalleryBarItemLink();
			if(link == null) return;
			item.Gallery.RefreshGallery();
			RibbonHitInfo hitInfo = new RibbonHitInfo();
			hitInfo.SetHitTest(RibbonHitTest.GalleryItemGroup);
			InRibbonGalleryViewInfo viewinfo = link.GalleryInfo as InRibbonGalleryViewInfo;
			hitInfo.GalleryInfo = viewinfo;
			GalleryItemGroupViewInfo grVi = viewinfo.GetGroupInfo(group);
			hitInfo.GalleryItemGroupInfo = grVi;
			DragObject = hitInfo;
			SelectedGalleryObject = (new GalleryObjectDescriptor(group, GalleryParentComponent, null));
		}
		public virtual void SelectRibbonGalleryBarItem(RibbonGalleryBarItem item) {
			RibbonGalleryBarItemLink link = GetSelectedGalleryBarItemLink();
			if(link == null) return;
			RibbonHitInfo hitInfo = new RibbonHitInfo();
			hitInfo.SetHitTest(RibbonHitTest.Item);
			hitInfo.SetItem(link.ViewInfo);
			DragObject = hitInfo;
			SelectedGalleryObject = (new GalleryObjectDescriptor(item, GalleryParentComponent, null));
		}
		[ToolboxItem(false)]
		public class RibbonDesignTimeCreateItemMenu : DevExpress.XtraBars.Customization.Helpers.DesignTimeManager.DesignTimeCreateItemMenu {
			public RibbonDesignTimeCreateItemMenu(BarManager designManager, BarManager sourceManager, BarItemLink link) : base(designManager, sourceManager, link) { }
			public override void InitMenu(BarLinksHolder holder) {
				base.InitMenu(holder);
				FilterMenu();
			}
			protected virtual bool ShouldFilter(BarItemInfoCollection coll, BarItemLink link) {
				if(coll == null || coll.Count == 0) return false;
				foreach(BarItemInfo info in coll) {
					if(info.GetCaption() == link.Caption && !info.SupportRibbon) return true;
				}
				return false;
			}
			public virtual void FilterMenu() {
				RibbonBarManager rm = SourceManager as RibbonBarManager;
				if(rm == null) return;
				BarItemInfoCollection coll = rm.Ribbon.GetController().PaintStyle.ItemInfoCollection;
				for(int i = 0; i < ItemLinks.Count;) {
					if(ShouldFilter(coll, ItemLinks[i])) {
						ItemLinks.RemoveAt(i);
						continue;
					}
					i++;
				}
			}
		}
	}
	public class AllowTimer {
		System.Windows.Forms.Timer timer;
		bool allowDrag = false;
		public bool AllowDrag { get { return allowDrag; } }
		public virtual void ClearTimer() {
			if(this.timer == null) return;
			this.timer.Tick -= new EventHandler(OnTimer);
			this.timer.Dispose();
			this.timer = null;
			this.allowDrag = false;
		}
		public virtual void InitializeTimer() {
			if(this.timer != null) ClearTimer();
			this.timer = new System.Windows.Forms.Timer();
			this.timer.Interval = 100;
			this.timer.Tick += new EventHandler(OnTimer);
			this.timer.Start();
		}
		protected virtual void OnTimer(object sender, EventArgs e) {
			this.allowDrag = true;
			this.timer.Stop();
		}
	}
	public abstract class BaseRibbonDesignTimeManager : BaseDesignTimeManager {
		AllowTimer timer;
		protected DXPopupMenu groupMenu;
		protected DXPopupMenu editorsMenu;
		protected BarButtonGroup lastButtonGroup = null;
		protected RibbonPageHeaderItemLinkCollection lastPageHeader = null;
		protected RibbonQuickToolbarItemLinkCollection lastToolbar = null;
		protected EditorClassInfo lastEditorClassInfo = null;
		public BaseRibbonDesignTimeManager(Control control, ISite site)
			: base(control, site) { }
		public BaseRibbonDesignTimeManager() { }
		public override void Initialize(object control, ISite site) {
			base.Initialize(control, site);
			timer = new AllowTimer();
			this.groupMenu = CreateGroupMenu();
		}
		public override void Dispose() {
			base.Dispose();
			if(this.groupMenu != null) this.groupMenu.Dispose();
		}
		protected virtual bool AllowMenu { get { return false; } }
		protected virtual IComponent GalleryParentComponent { get { return Ribbon; } }
		protected internal void FireChanged(IComponent component) {
			if(DesignerHost == null) return;
			IComponentChangeService srv = DesignerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(component, null, null, null);
		}
		protected internal virtual BarItem CreateItemByInfo(BarItemInfo info) {
			object arguments = null; 
			if(info.ItemType.Equals(typeof(BarEditItem))) arguments = lastEditorClassInfo.EditorType.Name;
			return DevExpress.XtraBars.Customization.Helpers.DesignTimeManager.DesignTimeCreateItemMenu.CreateItem(Manager, info, arguments);
		}
		protected virtual BarItem CreateItem(object sender) { return null; }
		protected virtual void OnAddItem(object sender, EventArgs e) { }
		protected virtual void OnAddEditor(object sender, EventArgs e) {
			DXMenuItem item = (DXMenuItem)sender;
			lastEditorClassInfo = item.Tag as EditorClassInfo;
			BarEditItem bi = CreateItem(editorsMenu) as BarEditItem;
			lastEditorClassInfo = null;
		}
		protected virtual RibbonPageHeaderItemLinkCollection GetLastPageHeader(RibbonHitInfo hitInfo) {
			if(!Ribbon.ViewInfo.Header.DesignerRect.Contains(hitInfo.HitPoint)) return null;
			return Ribbon.PageHeaderItemLinks;
		}
		protected virtual RibbonQuickToolbarItemLinkCollection GetLastToolbar(RibbonHitInfo hitInfo) {
			if(!Ribbon.ViewInfo.Toolbar.DesignerRect.Contains(hitInfo.HitPoint)) return null;
			return Ribbon.Toolbar.ItemLinks;
		}
		protected virtual BarButtonGroup GetLastButtonGroup(RibbonHitInfo hitInfo) {
			if(!hitInfo.InItem) return null;
			BarButtonGroupLink buttonGroupLink = hitInfo.Item as BarButtonGroupLink;
			if(buttonGroupLink != null) return buttonGroupLink.Item as BarButtonGroup;
			return hitInfo.Item.Holder as BarButtonGroup;
		}
		protected virtual void UpdateGroupMenu(bool enableGallery, bool enableButtonGroup) {
			this.groupMenu.Items[this.groupMenu.Items.Count - 1].Enabled = enableButtonGroup;
			this.groupMenu.Items[this.groupMenu.Items.Count - 2].Enabled = enableGallery;
			this.editorsMenu.Enabled = enableButtonGroup;
		}
		protected virtual DXPopupMenu CreateEditorsMenu() {
			this.editorsMenu = new DXPopupMenu();
			PopulateEditorsMenu();
			return this.editorsMenu;
		}
		protected virtual void PopulateEditorsMenu() {
			this.editorsMenu.Items.Clear();
			EditorClassInfoCollection editors = EditorRegistrationInfo.Default.Editors;
			foreach(EditorClassInfo editor in editors) {
				if(!editor.DesignTimeVisible || editor.AllowInplaceEditing == ShowInContainerDesigner.Never) continue;
				DXMenuItem item = new DXMenuItem(editor.Name, new EventHandler(OnAddEditor), editor.Image);
				item.Tag = editor;
				this.editorsMenu.Items.Add(item);
			}
		}
		protected virtual DXPopupMenu CreateGroupMenu() {
			DXPopupMenu menu = new DXPopupMenu();
			foreach(RibbonRegistrationInfo info in Ribbon.RegistrationInfo)
				AddBarItemsToMenu(menu, info.ItemInfoCollection);
			return menu;
		}
		private void AddBarItemsToMenu(DXPopupMenu menu, BarItemInfoCollection coll) {
			if(coll == null)
				return;
			foreach(BarItemInfo info in coll) {
				if(!info.SupportRibbon) continue;
				DXMenuItem item = null;
				if(info.ItemType.Equals(typeof(BarEditItem))) {
					item = CreateEditorsMenu();
					item.Caption = "Add Editor";
				}
				else item = new DXMenuItem(string.Format("Add {0} ({1})", info.GetShortCaption(), info), new EventHandler(OnAddItem));
				item.Image = ImageCollection.GetImageListImage(Manager.BarItemsImages, info.ImageIndex);
				item.Tag = info;
				menu.Items.Add(item);
			}
		}
		protected BarItem GetBarItem(BarItemLink link) { return link == null ? null : link.Item; }
		protected abstract RibbonHitInfo CalcHitInfo(Point p);
		protected internal virtual bool ProcessRightMouseDown(MouseEventArgs e, RibbonHitInfo hitInfo) {
			downPoint = e.Location;
			return false;
		}
		Point downPoint, lastPoint;
		internal Point DownPoint { get { return downPoint; } set { downPoint = value; } }
		RibbonHitInfo dragObject = null;
		public RibbonHitInfo DragObject { get { return dragObject; } set { dragObject = value; } }
		protected internal virtual RibbonLinkDragManager DragManager { get { return Manager.Helper.DragManager as RibbonLinkDragManager; } }
		public object DropSelectedObject { get { return DragManager.DropSelectedObject; } }
		public LinkDropTargetEnum DropSelectStyle { get { return DragManager.DropSelectStyle; } }
		BarItemLink GetBarItemLink(RibbonHitInfo hitInfo) {
			if(!hitInfo.InItem) return null;
			BarItemLink link = hitInfo.Item as BarItemLink;
			if(link.ClonedFromLink != null && link.ClonedFromLink.Holder is BarButtonGroup) return link.ClonedFromLink;
			return link;
		}
		GalleryObjectDescriptor selectedGalleryObject;
		internal GalleryObjectDescriptor SelectedGalleryObject { get { return selectedGalleryObject; } set { selectedGalleryObject = value; } }
		protected virtual bool EnterButtonGroup { get { return (Control.ModifierKeys & Keys.ShiftKey) != 0; } }
		protected virtual BarItemLink GetLinkByPoint(MouseEventArgs e) {
			return Ribbon.ViewInfo.GetLinkByPoint(e.Location, EnterButtonGroup);
		}
		public virtual bool ProcessMouseDown(MouseEventArgs e) {
			timer.InitializeTimer();
			((RibbonLinkDragManager)DragManager).HideSelectedDropDownGallery();
			downPoint = new Point(-10000, -10000);
			if(e.Button == MouseButtons.Left) downPoint = e.Location;
			bool res = false;
			RibbonHitInfo hitInfo = CalcHitInfo(new Point(e.X, e.Y));
			if(hitInfo.InItem) {
				if((hitInfo.Item as BarItemLink).Holder is BarButtonGroup) {
					BarItemLink link = GetLinkByPoint(e);
					if(link != null)hitInfo.SetItem(link.RibbonItemInfo);
				}
				Manager.SelectionInfo.CustomizeSelectedLink = hitInfo.Item as BarItemLink;
				BarItem item = GetBarItem(hitInfo.Item as BarItemLink);
				if(item != null && item.Site != null) {
					SetItemLink(item, GetBarItemLink(hitInfo));
					if(IsComponentSelected(item)) InvalidateComponent(item);
					SelectComponent(item);
					InvalidateComponent(hitInfo.Item);
					if(DesignerHost != null && e.Clicks > 1) {
						IDesigner designer = DesignerHost.GetDesigner(item) as IDesigner;
						if(designer != null) designer.DoDefaultAction();
						return true;
					}
				}
				res = true;
			}
			else if(hitInfo.InGalleryItem) {
				SelectedGalleryObject = new GalleryObjectDescriptor(DragManager.GetOriginalItem(hitInfo.GalleryItem), GalleryParentComponent, hitInfo.Item as RibbonGalleryBarItemLink);
				SelectComponent(SelectedGalleryObject);
			}
			else if(hitInfo.InGalleryGroup) {
				SelectedGalleryObject = new GalleryObjectDescriptor(DragManager.GetOriginalGroup(hitInfo.GalleryItemGroup), GalleryParentComponent, hitInfo.Item as RibbonGalleryBarItemLink);
				SelectComponent(SelectedGalleryObject);
			}
			else if(hitInfo.InGallery){
				if(hitInfo.HitTest != RibbonHitTest.GalleryDownButton && hitInfo.HitTest != RibbonHitTest.GalleryDropDownButton && hitInfo.HitTest != RibbonHitTest.GalleryUpButton) {
					BarItem item = GetBarItem(hitInfo.Item as BarItemLink);
					if(item != null) {
						SelectComponent(item);
						Manager.SelectionInfo.CustomizeSelectedLink = hitInfo.Item as BarItemLink;
					}
				}
			}
			else {
				if(hitInfo.InPageGroup) { SelectComponent(hitInfo.PageGroup); }
				if(hitInfo.HitTest == RibbonHitTest.PageHeader) { SelectComponent(hitInfo.Page); }
				else if(hitInfo.InPageCategory) { SelectComponent(hitInfo.PageCategory); }
			}
			if(e.Button == MouseButtons.Left) {
				dragObject = hitInfo;
			}
			else if(e.Button == MouseButtons.Right) {
				res |= ProcessRightMouseDown(e, hitInfo);
			}
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			ee.Handled = res;
			ee.Sync();
			return res;
		}
		public override void DrawSelection(GraphicsCache cache, Rectangle bounds, int alpha, Color color) {
			base.DrawSelection(cache, bounds, alpha, color);
		}
		public virtual int DragInterval { get { return 5; } }
		public virtual object DragItem {
			get {
				if(dragObject == null) return null;
				else if(dragObject.InGalleryItem) return dragObject.GalleryItem;
				else if(dragObject.InGalleryGroup) return dragObject.GalleryItemGroup;
				else if((dragObject.InItem || dragObject.InGallery) && dragObject.Item != Ribbon.Toolbar.CustomizeItemLink && dragObject.Item != Ribbon.Toolbar.DropDownItemLink) return dragObject.Item;
				else if(dragObject.InPageGroup) return dragObject.PageGroup;
				else if(dragObject.InPage) return dragObject.Page;
				else if(dragObject.InPageCategory) return dragObject.PageCategory;
				else if(dragObject.InToolbar) return dragObject.Toolbar.Toolbar;
				return null;
			}
		}
		protected virtual void StartDragging(MouseEventArgs e) {
			DragManager.StartDragging(Ribbon, e, DragItem, Ribbon);   
		}
		public virtual bool ProcessMouseUp(DXMouseEventArgs e) {
			downPoint = new Point(-10000, -10000);
			return false;
		}
		public virtual bool ProcessMouseMove(DXMouseEventArgs e) {
			if(downPoint == new Point(-10000, -10000)) return false;
			Point p = lastPoint = e.Location;
			p.Offset(-downPoint.X, -downPoint.Y);
			if(e.Button == MouseButtons.Left) {
				if((Math.Abs(p.X) > DragInterval || Math.Abs(p.Y) > DragInterval) && DragItem != null && timer.AllowDrag) {
					StartDragging(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta));
					dragObject = null;
					return true;
				}
			}
			else {
				downPoint = e.Location;
			}
			return false;
		}
		public override void InvalidateComponent(object component) {
			Owner.Invalidate(false);
		}
		public BarManager Manager { get { return Ribbon == null ? null : Ribbon.Manager; } }
		public abstract RibbonControl Ribbon { get; }
		public new Control Owner { get { return base.Owner as Control; } }
		protected override void OnDesignTimeSelectionChanged(object component) {
			BarItem item = component as BarItem;
			RibbonPage page = component as RibbonPage;
			RibbonPageGroup group = component as RibbonPageGroup;
			if(item != null && item.Manager == Manager) {
				OnItemSelectionChanged(item);
				InvalidateComponent(component);
			}
			if(page != null && page.Ribbon == Ribbon) InvalidateComponent(component);
			if(group != null && group.Ribbon == Ribbon) InvalidateComponent(component);
		}
		protected virtual void OnItemSelectionChanged(BarItem item) {
			if(IsComponentSelected(item)) return;
			SetItemLink(item, null);
		}
		protected void SetItemLink(BarItem item, BarItemLink link) {
			if(item == null) return;
			item.LinkProvider = link == null ? null : item.CreateLinkInfoProvider(link);
		}
	}
	public class GalleryControlDesignTimeManager : RibbonDesignTimeManager {
		public GalleryControlDesignTimeManager(Control owner, ISite site) : base(owner, site) { }
		protected override DXPopupMenu CreateEditorsMenu() {
			return null;
		}
		protected override DXPopupMenu CreateGroupMenu() {
			return null;
		}
		protected override DXPopupMenu CreateApplicationButtonMenu() {
			return null;
		}
		protected override DXPopupMenu CreateGalleryDropDownMenu() {
			return null;
		}
		protected override DXPopupMenu CreatePageMenu() {
			return null;
		}
		protected override IComponent GalleryParentComponent { get { return GalleryControl; } }
		public GalleryItemGroup SelectedGroup { get { return DragItem as GalleryItemGroup; } }
		public GalleryItem SelectedItem { get { return DragItem as GalleryItem; } }
		public void SelectGalleryGroup(GalleryItemGroup group) {
			GalleryControl.Gallery.RefreshGallery();
			RibbonHitInfo hitInfo = new RibbonHitInfo();
			hitInfo.SetHitTest(RibbonHitTest.GalleryItemGroup);
			hitInfo.GalleryInfo = GalleryControl.Gallery.ViewInfo;
			hitInfo.GalleryItemGroupInfo = GalleryControl.Gallery.ViewInfo.GetGroupInfo(group);
			DragObject = hitInfo;
			SelectedGalleryObject = (new GalleryObjectDescriptor(group, GalleryParentComponent, null));
		}
		public void SelectGalleryItem(GalleryItem item) {
			GalleryControl.Gallery.RefreshGallery();
			RibbonHitInfo hitInfo = new RibbonHitInfo();
			hitInfo.SetHitTest(RibbonHitTest.GalleryItem);
			hitInfo.GalleryInfo = GalleryControl.Gallery.ViewInfo;
			hitInfo.GalleryItemGroupInfo = GalleryControl.Gallery.ViewInfo.GetGroupInfo(item.GalleryGroup);
			hitInfo.GalleryItemInfo = GalleryControl.Gallery.ViewInfo.GetItemInfo(item);
			DragObject = hitInfo;
			SelectedGalleryObject = (new GalleryObjectDescriptor(item, GalleryParentComponent, null));
		}
		public void SelectNextGalleryGroup(GalleryItemGroup group) {
			int index = GalleryControl.Gallery.Groups.IndexOf(group);
			if(index > 0) {
				SelectGalleryGroup(GalleryControl.Gallery.Groups[index - 1]);
			}
			else if(index < GalleryControl.Gallery.Groups.Count - 1) {
				SelectGalleryGroup(GalleryControl.Gallery.Groups[index + 1]);
			}
		}
		public void SelectNextGalleryItem(GalleryItem item) {
			List<GalleryItem> items = GalleryControl.Gallery.GetAllItems();
			int index = items.IndexOf(item);
			if(index > 0) {
				SelectGalleryItem(items[index - 1]);
			}
			else if(index < items.Count - 1) {
				SelectGalleryItem(items[index + 1]);
			}
		}
		GalleryControlDragManager dragManager;
		protected internal override RibbonLinkDragManager DragManager {
			get {
				if(dragManager == null)
					dragManager = new GalleryControlDragManager(GalleryControl);
				return dragManager;
			}
		}
		protected override void StartDragging(MouseEventArgs e) {
			DragManager.StartDragging(GalleryControl, e, DragItem, GalleryControl);
		}
	}
	public class GalleryControlDragManager : RibbonLinkDragManager {
		GalleryControl galleryControl;
		public GalleryControlDragManager(GalleryControl control) : base(null) {
			this.galleryControl = control;
		}
		object dropSelectedObject;
		public override object DropSelectedObject {
			get { return dropSelectedObject; }
			set {
				InvalidateDropSelectedObject(false);
				dropSelectedObject = value;
				InvalidateDropSelectedObject(true);
			}
		}
		LinkDropTargetEnum dropSelectedStyle;
		public override LinkDropTargetEnum DropSelectStyle {
			get { return dropSelectedStyle; }
			set { dropSelectedStyle = value; }
		}
		void InvalidateDropSelectedObject(bool showMark) {
			GalleryItemGroup galleryGroup = DropSelectedObject as GalleryItemGroup;
			GalleryItem item = DropSelectedObject as GalleryItem;
			if(galleryGroup != null && galleryGroup.Gallery != null) galleryGroup.Gallery.Invalidate();
			else if(item != null && item.Gallery != null) item.Gallery.Invalidate(item);
		}
		protected GalleryControl GalleryControl { get { return galleryControl; } }
		protected override bool ShouldStartDragging() {
			return true;
		}
		protected override void OnStartDragging(Control ABarControl, MouseEventArgs e, object dragObject, Control ctrl) {
		}
		protected override void OnEndDragging(Control ABarControl, MouseEventArgs e, object dragObject, Control ctrl) {
			GalleryControl.FireGalleryChanged();
		}
		protected void FireGalleryChanged() {
			GalleryControl.FireGalleryChanged();
			GalleryControl.Gallery.RefreshGallery();
		}
		protected override void RemoveGalleryItem(GalleryItemGroup group, GalleryItem item) {
			group.Items.Remove(item);
			FireGalleryChanged();   
		}
		protected override void RemoveGalleryItemGroup(BaseGallery gallery, GalleryItemGroup group) {
			gallery.Groups.Remove(group);
			FireGalleryChanged();
		}
		protected override void CopyGalleryItem(GalleryItem item, GalleryItemGroup group, GalleryItem insertItem) {
			if(insertItem == null) group.Items.Add(item);
			else group.Items.Insert(group.Items.IndexOf(insertItem), item);
			FireGalleryChanged();
		}
		protected override void CopyGalleryItemGroup(GalleryItemGroup group, BaseGallery gallery, GalleryItemGroup insertGroup) {
			if(insertGroup == null) gallery.Groups.Add(group);
			else gallery.Groups.Insert(gallery.Groups.IndexOf(insertGroup), group);
			FireGalleryChanged();
		}
		public override void StopDragging(Control control, DragDropEffects effects, bool cancelDrag) {
			if(DragObject is GalleryItem)
				StopDraggingGalleryItem(GalleryControl, effects, cancelDrag);
			if(DragObject is GalleryItemGroup)
				StopDraggingGalleryItemGroup(GalleryControl, effects, cancelDrag);
			DropSelectedObject = null;
			FireGalleryChanged();
		}
		protected override ArrayList DragCursors {
			get {
				return GalleryControl.GetController().DragCursors;
			}
		}
	}
	public delegate bool ShouldCreateItemInfoDelegate(BarItemLink link);
	public static class InplaceLinksHelper {
		public static List<BarItemLink> GetLinks(BarManager manager, BarItemLinkReadOnlyCollection coll, bool allowInplaceLinks, bool isDesignMode, ShouldCreateItemInfoDelegate del) {
			if(coll == null)
				return new List<BarItemLink>();
			List<BarItemLink> res = new List<BarItemLink>();
			foreach(BarItemLink link in coll) {
				if(!del(link)) continue;
				BarLinkContainerExItemLink ciLink = link as BarLinkContainerExItemLink;
				if(!(ciLink is BarButtonGroupLink) && ciLink != null && !isDesignMode && allowInplaceLinks) {
					ciLink.Item.OnGetItemData();
					BarItemLinkReadOnlyCollection ccoll = new BarItemLinkReadOnlyCollection();
					foreach(BarItemLink clink in ciLink.InplaceLinks) {
						BarItemLink cloned = (BarItemLink)((ICloneable)clink).Clone();
						cloned.Manager = manager;
						ccoll.AddItem(cloned);
					}
					res.AddRange(GetLinks(manager, ccoll, allowInplaceLinks, isDesignMode, del));
				}
				else
					res.Add(link);
			}
			return res;
		}
	}
}
