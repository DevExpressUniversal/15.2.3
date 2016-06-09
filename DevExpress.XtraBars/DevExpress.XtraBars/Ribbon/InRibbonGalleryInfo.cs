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
using System.Collections;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Localization;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Ribbon.Helpers;
using System.Collections.Generic;
using DevExpress.XtraBars.Painters;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class MacStyleInRibbonGalleryViewInfo : InRibbonGalleryViewInfo {
		Rectangle leftButtonBounds, rightButtonBounds;
		InRibbonGalleryRibbonItemViewInfo itemInfo;
		public MacStyleInRibbonGalleryViewInfo(InRibbonGallery gallery, InRibbonGalleryRibbonItemViewInfo itemInfo) : base(gallery) {
			this.itemInfo = itemInfo;
		}
		public Rectangle LeftButtonBounds {
			get { return leftButtonBounds; }
			set { leftButtonBounds = value; }
		}
		public Rectangle RightButtonBounds {
			get { return rightButtonBounds; }
			set { rightButtonBounds = value; }
		}
		protected internal bool CommandButtonVisible { get { return ((RibbonGalleryBarItemLink)ItemInfo.Item).CommandButtonVisible; } }
		protected virtual int ButtonIndent { get { return 2; } }
		public virtual SkinElementInfo GetLeftButtonInfo() {
			return new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinLeftScrollButton], LeftButtonBounds);
		}
		public virtual SkinElementInfo GetRightButtonInfo() {
			return new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinRightScrollButton], RightButtonBounds);
		}
		protected virtual int CommandButtonTopIndent {
			get {
				object obj = RibbonSkins.GetSkin(Provider).Properties[RibbonSkins.OptMacStyleGalleryCommandButtonTopIndent];
				if(obj != null)
					return (int)obj;
				return 0;
			}
		}
		public virtual SkinElementInfo GetCommandButtonInfo() {
			SkinElement elem = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinMacStyleGalleryButtonDropDown];
			if(elem == null)
				elem = BarSkins.GetSkin(Provider)[BarSkins.SkinPopupMenuExpandButton];
			return new SkinElementInfo(elem, ButtonCommandBounds);
		}
		protected virtual Size CalcLeftButtonSize() {
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, GetLeftButtonInfo()).Size;
		}
		protected virtual Size CalcRightButtonSize() {
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, GetRightButtonInfo()).Size;
		}
		protected virtual Size CalcCommandButtonSize() {
			return ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, GetCommandButtonInfo()).Size;
		}
		protected override Size FixedImageDefaultSize {
			get { return new Size(32, 32); }
		}
		public override void LayoutButtonsBounds() {
			ButtonCommandBounds = new Rectangle(new Point(ContentBounds.X + (ContentBounds.Width - ButtonCommandBounds.Width) / 2, ContentBounds.Bottom + CommandButtonTopIndent), ButtonCommandBounds.Size);
			Rectangle rLeft = Rectangle.Empty, rRight = Rectangle.Empty;
			rLeft = new Rectangle(new Point(ContentBounds.X + ButtonIndent, ContentBounds.Y + (ContentBounds.Height - LeftButtonBounds.Height) / 2), LeftButtonBounds.Size);
			rRight = new Rectangle(new Point(ContentBounds.Right - ButtonIndent - RightButtonBounds.Width, ContentBounds.Y + (ContentBounds.Height - RightButtonBounds.Height) / 2), RightButtonBounds.Size);
			LeftButtonBounds = Gallery.IsRightToLeft ? rRight : rLeft;
			RightButtonBounds = Gallery.IsRightToLeft ? rLeft : rRight;
		}
		public override void Invalidate(RibbonHitInfo hitInfo) {
			base.Invalidate(hitInfo);
			if(hitInfo.HitTest == RibbonHitTest.GalleryLeftButton) {
				Invalidate(LeftButtonBounds);
			}
			else if(hitInfo.HitTest == RibbonHitTest.GalleryRightButton) {
				Invalidate(RightButtonBounds);
			}
		}
		protected override void SetScrollYPosition(GalleryItemViewInfo itemInfo, int pos) {
			if(ItemInfo != null && itemInfo != null) (ItemInfo.Item as RibbonGalleryBarItemLink).ScrollRowIndex = GetItemCol(itemInfo.Item);
		}
		protected virtual int GetItemCol(GalleryItem item) {
			int colCount = 0;
			foreach(GalleryItemGroupViewInfo gi in Groups) {
				foreach(GalleryItemViewInfo ii in gi.Items) {
					if(ii.Item == item)
						return colCount;
					colCount++;
				}
			}
			return 0;
		}
		protected override void CalcViewInfoCore(Rectangle bounds) {
			bounds.Height -= CommandButtonTopIndent + ButtonCommandBounds.Height;
			base.CalcViewInfoCore(bounds);
		}
		public override Size ControlSize { get { return Size.Empty; } }
		protected internal override void CalcHitInfo(RibbonHitInfo hitInfo) {
			base.CalcHitInfo(hitInfo);
			if(!Item.Enabled) return;
			if(hitInfo.ContainsSet(LeftButtonBounds, RibbonHitTest.GalleryLeftButton))
				return;
			if(hitInfo.ContainsSet(RightButtonBounds, RibbonHitTest.GalleryRightButton))
				return;
		}
		public override Size CalcGalleryBestSize(int row, int col) {
			LeftButtonBounds = new Rectangle(Point.Empty, CalcLeftButtonSize());
			RightButtonBounds = new Rectangle(Point.Empty, CalcRightButtonSize());
			ButtonCommandBounds = new Rectangle(Point.Empty, CalcCommandButtonSize());
			Size sz = base.CalcGalleryBestSize(row, col);
			sz.Height += CommandButtonTopIndent + ButtonCommandBounds.Height;
			return sz;
		}
		protected internal override Size CalcVisibleSize(int row, int col) {
			Size size = new Size(CalcWidthForColumns(col), CalcHeightForRows(row, col));
			size = new Size(GetSecondarySize(size), GetDefineSize(size));
			if(size.Width == 0 && Gallery.DesignModeCore)
				size.Width = GetDefaultSize().Width;
			if(size.Height == 0 && Gallery.DesignModeCore)
				size.Height = GetDefaultSize().Height;
			size.Width = Math.Max(size.Width, GetMaxCaptionWidth());
			size.Width += LeftButtonBounds.Width + ButtonIndent * 2;
			size.Width += RightButtonBounds.Width + ButtonIndent * 2;
			return ConstrainVisibleSize(size);
		}
		public override void MakeVisible(GalleryItem item) {
			if(item == null) return;
			MakeVisibleItem = item;
			if(!IsReady || GetSecondarySize(BestSize) == 0 || Bounds.IsEmpty) return;
			if(GetSecondarySize(BestSize) <= GetSecondarySize(GalleryContentBounds)) {
				MakeVisibleItem = null;
				return;
			}
			GalleryItemViewInfo itemInfo = FindItemInfo(item);
			if(itemInfo == null) return;
			MakeVisibleItem = null;
			if(itemInfo.GalleryInfo.GalleryContentBounds.Contains(itemInfo.Bounds)) return;
			int contentWidth = itemInfo.GalleryInfo.RealColsWidth;
			if((GetSecondaryEndCoor(itemInfo.Bounds) - GetSecondaryCoor(itemInfo.GroupInfo.Bounds.Location)) <= contentWidth && Gallery.ShowGroupCaption)
				SetScrollYPosition(itemInfo, GetSecondaryCoor(itemInfo.GroupInfo.Bounds.Location) - GetSecondaryCoor(GalleryContentBounds.Location) + ScrollYPosition);
			else if(GetSecondarySize(BestSize) - (GetSecondaryCoor(itemInfo.Bounds.Location) - GetSecondaryCoor(GalleryContentBounds.Location) + ScrollYPosition) < contentWidth)
				SetScrollYPosition(itemInfo, GetSecondarySize(BestSize) - contentWidth);
			else
				SetScrollYPosition(itemInfo, GetSecondaryCoor(itemInfo.Bounds.Location) - ScrollYPosition - GetSecondaryCoor(GalleryContentBounds.Location));
		}
		public override Rectangle GalleryContentBounds {
			get {
				Rectangle rect = base.GalleryContentBounds;
				int leftWidth = ButtonIndent * 2 + LeftButtonBounds.Width;
				int rightWidth = ButtonIndent * 2 + RightButtonBounds.Width;
				rect.X += leftWidth;
				rect.Width -= leftWidth + rightWidth;
				return rect;
			}
		}
		protected override GalleryItemGroupViewInfo CreateGroupInfo(BaseGalleryViewInfo galleryInfo, GalleryItemGroup group) {
			return new MacStyleGalleryItemGroupViewInfo(galleryInfo, group);
		}
		public override int MaxScrollYPosition {
			get {
				if(BestSize == Size.Empty) return int.MaxValue;
				if(BestSize.Width <= GalleryContentBounds.Width || ItemMaxSize.Width + HDistanceBetweenItems == 0) return 0;
				if(RealColCount == 0) return BestSize.Width - GalleryContentBounds.Width;
				return BestSize.Width - RealColsWidth;
			}
		}
		protected internal override bool CanScroll {
			get {
				return BestSize.Width > VisibleSize.Width;
			}
		}
		protected override void LayoutGroupsCore() {
			Point location = UpdateLocation(GalleryContentBounds.Location);
			Size size = Size.Empty;
			this.viBestSize = Size.Empty;
			RowCount = 1;
			ColCount = 0;
			for(int i = 0; i < Groups.Count; i++) {
				LoadImages(Groups[i]);
				size = Groups[i].LayoutItems(ItemMaxSize, location);
				OffsetSecondaryCoor(ref location, GetSecondarySize(size));
				this.viBestSize.Width += size.Width;
				this.viBestSize.Height = Math.Max(BestSize.Height, size.Height);
				ColCount += Groups[i].LinesCount;
			}
		}
		protected RibbonGalleryBarItemLink OwnerLink { get { return (RibbonGalleryBarItemLink)ItemInfo.Item; } }
	}
	public class InRibbonGalleryViewInfo : BaseGalleryViewInfo, ISupportXtraAnimation {
		public static int DefaultControlWidth = 15;
		public static int DefaultControlWidthInTouch = 30;
		Rectangle btnUpBounds, btnDownBounds, btnCmdBounds;
		RibbonItemViewInfo itemInfo;
		int colCount;
		public InRibbonGalleryViewInfo(InRibbonGallery gallery)
			: base(gallery) {
			this.btnUpBounds = this.btnDownBounds = this.btnCmdBounds = Rectangle.Empty;
			this.colCount = Gallery.ColumnCount;
			this.itemInfo = null;
		}
		protected internal override bool AllowPartitalItems {
			get {
				return ItemMaxSize.Height > ContentBounds.Height;
			}
		}
		protected internal override bool IsItemHovered(GalleryItemViewInfo itemInfo) {
			return RibbonViewInfo.HotObject.InGalleryItem && RibbonViewInfo.HotObject.GalleryItem == itemInfo.Item || itemInfo.KeyboardSelected;
		}
		public virtual RibbonControl CurrentRibbon {
			get {
				if(ItemInfo != null && (ItemInfo.OwnerControl is RibbonControl)) return ItemInfo.OwnerControl as RibbonControl;
				return Ribbon;
			}
		}
		public override Control OwnerControl { 
			get {
				if(ItemInfo.OwnerControl is RibbonMiniToolbarControl)
					return ItemInfo.OwnerControl;
				return CurrentRibbon; 
			} 
		}
		protected virtual int GalleryHeight { get { return RibbonViewInfo.GroupContentHeight - 4; } }
		protected virtual int ButtonHeight { get { return GalleryHeight / 3; } }
		public Rectangle ButtonUpBounds { get { return btnUpBounds; } set { btnUpBounds = value; } }
		public Rectangle ButtonDownBounds { get { return btnDownBounds; } set { btnDownBounds = value; } }
		public Rectangle ButtonCommandBounds { get { return btnCmdBounds; } set { btnCmdBounds = value; } }
		public virtual RibbonItemViewInfo ItemInfo { get { return itemInfo; } set { itemInfo = value; } }
		public IRibbonItem Item { get { return ItemInfo == null ? null : ItemInfo.Item; } }
		protected bool GetTouchUI() {
			if(Ribbon == null)
				return false;
			if(Ribbon.MergeOwner != null)
				return Ribbon.MergeOwner.GetController().LookAndFeel.GetTouchUI();
			return Ribbon.GetController().LookAndFeel.GetTouchUI();
		}
		protected virtual int ControlWidth {
			get {
				int width = 0;
				if(!GetTouchUI())
					width = DefaultControlWidth;
				else 
					width = DefaultControlWidthInTouch;
				return (int)(width * CommonSkins.GetSkin(Provider).DpiProvider.DpiScaleFactor);
			}
		}
		protected internal virtual SkinElementInfo GetDropDownButtonInfo() {
			string skinName = GetTouchUI() ? RibbonSkins.SkinGalleryButtonDropDownTouch : RibbonSkins.SkinGalleryButtonDropDown;
			SkinElement elem = RibbonSkins.GetSkin(Gallery.Provider)[skinName];
			if(elem == null)
				elem = RibbonSkins.GetSkin(Gallery.Provider)[RibbonSkins.SkinGalleryButtonDropDown];
			return new SkinElementInfo(elem, ButtonCommandBounds);
		}
		public virtual void LayoutButtonsBounds() {
			this.btnUpBounds = CalcBtnUpBounds();
			this.btnDownBounds = CalcBtnDownBounds();
			this.btnCmdBounds = CalcBtnCmdBounds();
		}
		protected virtual Rectangle CalcBtnUpBounds() {
			if(GetTouchUI()) return Rectangle.Empty;
			int x = Gallery.IsRightToLeft ? Bounds.Left : Bounds.Right - ControlWidth;
			return new Rectangle(x, Bounds.Y, ControlWidth, ButtonHeight);
		}
		protected virtual Rectangle CalcBtnDownBounds() {
			if(GetTouchUI()) return Rectangle.Empty;
			int x = Gallery.IsRightToLeft ? Bounds.Left : Bounds.Right - ControlWidth;
			return new Rectangle(x, btnUpBounds.Bottom, ControlWidth, ButtonHeight);
		}
		protected virtual Rectangle CalcBtnCmdBounds() {
			int x = Gallery.IsRightToLeft ? Bounds.Left : Bounds.Right - ControlWidth;
			if(GetTouchUI()) return new Rectangle(x, Bounds.Y, ControlWidth, Bounds.Height);
			return new Rectangle(x, btnDownBounds.Bottom, ControlWidth, Bounds.Height - (ButtonHeight * 2));
		}
		public override void Invalidate(Rectangle bounds) {
			if(ItemInfo != null && ItemInfo.OwnerControl != null) {
				ItemInfo.OwnerControl.Invalidate(bounds);
				return;
			}
			if(Ribbon != null) Ribbon.Invalidate(bounds);
		}
		protected internal override void LayoutGroups() {
			base.LayoutGroups();
			LayoutButtonsBounds();
		}
		public override Locations GetItemImageLocation() {
			if(Gallery.ItemImageLocation != Locations.Default) return Gallery.ItemImageLocation;
			return Locations.Top;
		}
		public override void Invalidate(RibbonHitInfo hitInfo) {
			if(hitInfo.HitTest == RibbonHitTest.GalleryDownButton) {
				Invalidate(ButtonDownBounds);
				return;
			}
			if(hitInfo.HitTest == RibbonHitTest.GalleryUpButton) {
				Invalidate(ButtonUpBounds);
				return;
			}
			if(hitInfo.HitTest == RibbonHitTest.GalleryDropDownButton) {
				Invalidate(ButtonCommandBounds);
				return;
			}
			base.Invalidate(hitInfo);
		}
		protected override RibbonGalleryAppearances CreateDefaultAppearance() {
			RibbonGalleryAppearances res = base.CreateDefaultAppearance();
			res.ItemCaptionAppearance.Normal.TextOptions.VAlignment = VertAlignment.Top;
			res.ItemCaptionAppearance.Normal.TextOptions.HAlignment = HorzAlignment.Center;
			res.ItemCaptionAppearance.Hovered.TextOptions.VAlignment = VertAlignment.Top;
			res.ItemCaptionAppearance.Hovered.TextOptions.HAlignment = HorzAlignment.Center;
			res.ItemCaptionAppearance.Pressed.TextOptions.VAlignment = VertAlignment.Top;
			res.ItemCaptionAppearance.Pressed.TextOptions.HAlignment = HorzAlignment.Center;
			res.ItemDescriptionAppearance.Normal.TextOptions.VAlignment = VertAlignment.Top;
			res.ItemDescriptionAppearance.Normal.TextOptions.HAlignment = HorzAlignment.Center;
			res.ItemDescriptionAppearance.Hovered.TextOptions.VAlignment = VertAlignment.Top;
			res.ItemDescriptionAppearance.Hovered.TextOptions.HAlignment = HorzAlignment.Center;
			res.ItemDescriptionAppearance.Pressed.TextOptions.VAlignment = VertAlignment.Top;
			res.ItemDescriptionAppearance.Pressed.TextOptions.HAlignment = HorzAlignment.Center;
			return res;
		}
		public new InRibbonGallery Gallery { get { return base.Gallery as InRibbonGallery; } }
		public override Size ControlSize { get { return new Size(ControlWidth, VisibleSize.Height); } }
		protected override Size ConstrainVisibleSize(Size size) {
			if(Groups.Count == 0) size.Width = 50;
			return new Size(size.Width, GalleryHeight);
		}
		bool TouchMode { get; set; }
		protected override void CheckBestSize() {
			if(TouchMode != GetTouchUI()) {
				IsCalculateBestSize = true;
				TouchMode = GetTouchUI();
			}
			base.CheckBestSize();
		}
		public virtual int Reduce() {
			this.colCount = Math.Min(this.colCount, ActualColCount);
			if(this.colCount == Gallery.MinimumColumnCount) return 0;
			if(!IsReady) CalcConstants();
			CheckBestSize();
			int prevWidth = CalcVisibleSize(Gallery.RowCount, ActualColCount).Width;
			this.colCount--;
			SetDirty();
			return prevWidth - CalcVisibleSize(Gallery.RowCount, ActualColCount).Width;
		}
		public override int ColCount { get { return colCount; } set { colCount = value; } }
		protected internal override int ActualColCount {
			get {
				List<GalleryItem> res = Gallery.GetAllItems();
				return Math.Min(res.Count, ColCount);
			}
		}
		protected internal override void CalcHitInfo(RibbonHitInfo hitInfo) {
			hitInfo.ContainsSet(Bounds, RibbonHitTest.Gallery);
			if(!Item.Enabled) return;
			if(hitInfo.ContainsSet(ButtonUpBounds, RibbonHitTest.GalleryUpButton)) return;
			if(hitInfo.ContainsSet(ButtonDownBounds, RibbonHitTest.GalleryDownButton)) return;
			if(hitInfo.ContainsSet(ButtonCommandBounds, RibbonHitTest.GalleryDropDownButton)) return;
			for(int i = 0; i < Groups.Count; i++) {
				if(Groups[i].CalcHitInfo(hitInfo)) break;
			}
		}
		public override int OffsetFromContentTop {
			get {
				if(RealRowCount == 0 || BestSize.Height == 0) return 0;
				if(BestSize.Height < GalleryContentBounds.Height) return (GalleryContentBounds.Height - BestSize.Height) / 2;
				return (GalleryContentBounds.Height - RealRowsHeight) / 2;
			}
		}
		public override int MaxScrollYPosition {
			get {
				if(BestSize == Size.Empty) return int.MaxValue;
				if(BestSize.Height <= GalleryContentBounds.Height || ItemMaxSize.Height + VDistanceBetweenItems == 0) return 0;
				if(RealRowCount == 0) return BestSize.Height - GalleryContentBounds.Height;
				return BestSize.Height - RealRowsHeight;
			}
		}
		protected internal override int ScrollYPosition {
			get {
				if(ItemInfo == null) return 0;
				RibbonGalleryBarItemLink link = ItemInfo.Item as RibbonGalleryBarItemLink;
				if(link == null) return 0;
				return link.ScrollYPosition;
			}
		}
		protected override Point UpdateLocation(Point location) {
			Point res = location;
			if(((InRibbonGallery)Gallery).GetRibbonStyle() == RibbonControlStyle.MacOffice)
				OffsetSecondaryCoor(ref res, -ScrollYPosition);
			else
				OffsetDefineCoor(ref res, -ScrollYPosition);
			return res;
		}
		internal void PrepareCached() {
			SetDirty();
			this.colCount = Gallery.ColumnCount;
		}
		protected override void SetScrollYPosition(GalleryItemViewInfo itemInfo, int pos) {
			if(ItemInfo != null && itemInfo != null) (ItemInfo.Item as RibbonGalleryBarItemLink).ScrollRowIndex = GetItemRow(itemInfo.Item);
		}
		protected internal override ObjectState CalcItemState(GalleryItemViewInfo itemInfo) {
			if(Gallery.DesignModeCore) return ObjectState.Normal;
			if(!ItemInfo.Enabled) return ObjectState.Disabled;
			if(ItemInfo.ViewInfo.PressedObject.InGalleryItem && ItemInfo.ViewInfo.PressedObject.GalleryItemInfo == itemInfo)
				return ObjectState.Pressed;
			if(ItemInfo.ViewInfo.HotObject.InGalleryItem && ItemInfo.ViewInfo.HotObject.GalleryItemInfo == itemInfo)
				return ObjectState.Hot;
			return base.CalcItemState(itemInfo);
		}
		protected internal override ImageGalleryInfoArgs CreateGalleryInfo(BaseRibbonViewInfo viewInfo) { 
			InRibbonImageGalleryInfoArgs res = new InRibbonImageGalleryInfoArgs(viewInfo, this);
			res.ActiveItemInfo = itemInfo.Enabled && viewInfo.HotObject.InGalleryItem ? viewInfo.HotObject.GalleryItemInfo : null;
			res.Hot = itemInfo.Enabled && viewInfo.HotObject.InGallery && viewInfo.HotObject.GalleryInfo == this;
			return res;
		}
		protected override bool CanAnimate { 
			get {
				if(!WindowsFormsSettings.GetAllowHoverAnimation(Provider))
					return false;
				return !Gallery.AllowHoverImages; 
			} 
		}
		static readonly double BitmapAnimationDivider = 1 / 6;
		protected internal override void AddBitmapFadeAnimation(RibbonHitInfo prev, RibbonHitInfo current) {
			if(!CanAnimate) return;
			if(prev.GalleryItemInfo != null) {
				int lenght = Ribbon.GalleryAnimationLength != -1 ? Ribbon.GalleryAnimationLength : 2000;
				lenght = (int)(lenght * BitmapAnimationDivider);
				XtraAnimator.Current.AddBitmapFadeAnimation(OwnerControl as ISupportXtraAnimation, prev.GalleryItemInfo, lenght, null, GetForeInfo(prev.GalleryItemInfo), false);
				prev.GalleryItemInfo.UpdateContent(ObjectState.Normal);
			}
			if(current.GalleryItemInfo != null) {
				current.GalleryItemInfo.UpdateContent(ObjectState.Normal);
				int lenght = Ribbon.GalleryAnimationLength != -1 ? (int)(Ribbon.GalleryAnimationLength * BitmapAnimationDivider) : XtraAnimator.Current.CalcBarAnimationLength(ObjectState.Normal, ObjectState.Hot);
				XtraAnimator.Current.AddBitmapAnimation(OwnerControl as ISupportXtraAnimation, current.GalleryItemInfo, lenght, null, GetForeInfo(current.GalleryItemInfo), new BitmapAnimationImageCallback(OnGetItemBitmap));
			}
		}
		ObjectPaintInfo GetForeInfo(GalleryItemViewInfo itemInfo) {
			RibbonMiniToolbarControl toolbarControl = OwnerControl as RibbonMiniToolbarControl;
			BaseRibbonViewInfo ownerViewInfo = RibbonViewInfo;
			if(toolbarControl != null)
				ownerViewInfo = toolbarControl.ViewInfo;
			itemInfo.UpdateContent();
			return new ObjectPaintInfo(new GalleryItemObjectPainter(), new GalleryItemGroupPainter().CreateItemInfo(itemInfo, CreateGalleryInfo(ownerViewInfo)));
		}
		Bitmap OnGetItemBitmap(BaseAnimationInfo info) {
			GalleryItemViewInfo itemInfo = info.AnimationId as GalleryItemViewInfo;
			if(itemInfo == null) return null;
			return XtraAnimator.Current.CreateBitmap(GetForeInfo(itemInfo));
		}
	}
}
