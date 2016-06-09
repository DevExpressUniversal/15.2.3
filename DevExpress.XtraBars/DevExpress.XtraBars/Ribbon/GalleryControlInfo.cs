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
using DevExpress.XtraBars.Controls;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Ribbon.Helpers;
using System.Collections.Generic;
using DevExpress.XtraBars.Utils;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public abstract class BaseGalleryViewInfo : ISupportXtraAnimation {
		BaseGallery gallery;
		GalleryItemGroupViewInfoCollection groups;
		RibbonGalleryAppearances paintAppearance;
		Rectangle bounds, contentBounds;
		bool paintAppearanceDirty = true;
		protected Size viVisibleSize, viBestSize, viItemMaxSize, viItemMaxImageSize;
		GraphicsInfo ginfo;
		bool isReady;
		bool calculateBestSize;
		int buttonPaddingWidth, buttonPaddingHeight;
		Size imageBackgroundIndent, defaultImageSize, defaultImageFullSize;
		int startRowIndex, visibleRowCount;
		int rowsCount;
		Rectangle selection;
		public BaseGalleryViewInfo(BaseGallery gallery) {
			this.ginfo = new GraphicsInfo();
			this.paintAppearance = new RibbonGalleryAppearances(null);
			this.gallery = gallery;
			this.groups = new GalleryItemGroupViewInfoCollection(this);
			this.isReady = false;
			this.viVisibleSize = Size.Empty;
			this.viBestSize = Size.Empty;
			this.viItemMaxSize = Size.Empty;
			this.calculateBestSize = true;
			this.defaultImageFullSize = this.imageBackgroundIndent = this.defaultImageSize = Size.Empty;
		}
		public virtual void UpdateVisual() {
			foreach(GalleryItemGroupViewInfo group in Groups) {
				foreach(GalleryItemViewInfo item in group.Items) {
					item.UpdateAppearance();
				}
			}
		}
		protected internal virtual ObjectState CalcItemState(GalleryItemViewInfo itemInfo) {
			ObjectState res = ObjectState.Normal;
			if(Gallery.DesignModeCore)
				return ObjectState.Normal;
			if(!itemInfo.Item.Enabled)
				return ObjectState.Disabled;
			if(itemInfo.KeyboardSelected || Selection.IntersectsWith(itemInfo.Bounds) || IsItemHovered(itemInfo)) res = ObjectState.Hot;
			if(itemInfo.Item == itemInfo.Item.Gallery.DownItem) res = ObjectState.Pressed;
			return res; 
		}
		protected internal bool AllowGlyphSkinning { get { return Gallery.AllowGlyphSkinning; } }
		protected virtual Rectangle GetSelectionRect(Point pt1, Point pt2) {
			int x = pt1.X < pt2.X ? pt1.X : pt2.X;
			int y = pt1.Y < pt2.Y ? pt1.Y : pt2.Y;
			int width = Math.Abs(pt2.X - pt1.X);
			int height = Math.Abs(pt2.Y - pt1.Y);
			return new Rectangle(x, y, width, height);
		}
		protected virtual GalleryItemViewInfo GetHoveredItem() { return null; }
		protected virtual GalleryItemViewInfo GetFirstCheckedItem() {
			foreach(GalleryItemGroupViewInfo groupInfo in Groups) {
				foreach(GalleryItemViewInfo itemInfo in groupInfo.Items) {
					if(itemInfo.Item.Checked)
						return itemInfo;
				}
			}
			return null;
		}
		protected internal virtual void UpdateSelectionRect(Point pt1, Point pt2) {
			Selection = GetSelectionRect(pt1, pt2);
		}
		protected internal virtual bool CanScroll {
			get {
				return BestSize.Height > VisibleSize.Height;
			}
		}
		protected internal virtual bool AutoFitColumns { get { return true; } }
		protected internal virtual HorzAlignment ItemHorizontalAlignmet { get { return HorzAlignment.Near; } }
		public virtual void Invalidate(RibbonHitInfo hitInfo) {
			if(hitInfo.InGalleryItem) {
				if(hitInfo.GalleryItemInfo != null)
					Invalidate(hitInfo.GalleryItemInfo.Bounds);
				else
					Gallery.Invalidate(hitInfo.GalleryItem);
				return;
			}
		}
		public abstract void Invalidate(Rectangle bounds);
		public abstract Control OwnerControl { get; }
		public Rectangle Selection { get { return selection; } set { selection = value; } }
		public bool IsItemInSelection(GalleryItemViewInfo itemInfo) {
			Rectangle rect = itemInfo.Bounds;
			OffsetDefineCoor(ref rect, ScrollYPosition);
			return Selection.IntersectsWith(rect);
		}
		protected virtual Size CalcImageBackgroundIndent() {
			if(!Gallery.DrawImageBackground) return Size.Empty;
			Size res = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, Gallery.Painter.ItemPainter.GetImageBackgroundInfo(Gallery, Rectangle.Empty), new Rectangle(0, 0, 100, 100)).Size;
			res.Width -= 100; res.Height -= 100;
			return res;
		}
		protected virtual Size CalcDefaultImageFullSize() {
			Size res  = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, Gallery.Painter.ItemPainter.GetImageBackgroundInfo(Gallery, Rectangle.Empty), new Rectangle(Point.Empty, DefaultItemImageSize)).Size;
			return res;
		}
		protected virtual GalleryItemViewInfo GetFirstVisibleItem() {
			foreach(GalleryItemGroupViewInfo groupInfo in Groups) {
				if(!groupInfo.Group.Visible || groupInfo.Items.Count == 0)
					continue;
				return groupInfo.Items[0];
			}
			return null;
		}
		protected virtual GalleryItemViewInfo GetLastVisibleItem() {
			for(int i = Groups.Count - 1; i >= 0; i--) {
				GalleryItemGroupViewInfo groupInfo = Groups[i];
				if(!groupInfo.Group.Visible || groupInfo.Items.Count == 0)
					continue;
				return groupInfo.Items[groupInfo.Items.Count - 1];
			}
			return null;
		}
		public int RowCount { get { return rowsCount; } set { rowsCount = value; } }
		protected virtual int CalcButtonPaddingWidth() {
			SkinElementInfo info = Gallery.ItemPainter.GetButtonInfo(Gallery, Rectangle.Empty, ObjectState.Hot);
			return SkinElementPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, info, new Rectangle(0, 0, 100, 100)).Width - 100;
		}
		protected virtual int CalcButtonPaddingHeight() {
			SkinElementInfo info = Gallery.ItemPainter.GetButtonInfo(Gallery, Rectangle.Empty, ObjectState.Hot);
			if(info.Element != null)
			info.Element.Size.MinSize = Size.Empty;
			return SkinElementPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, info, new Rectangle(0, 0, 100, 100)).Height - 100;
		}
		protected virtual void CalcConstants() {
			this.buttonPaddingHeight = CalcButtonPaddingHeight();
			this.buttonPaddingWidth = CalcButtonPaddingWidth();
			this.imageBackgroundIndent = CalcImageBackgroundIndent();
			this.defaultImageSize = GetImageSizeCore(null, Gallery.Images, -1, Gallery.FixedImageSize, Gallery.ImageSize, new Size(16, 16));
			this.defaultImageFullSize = CalcDefaultImageFullSize();
		}
		public Size DefaultItemImageSize { get { return defaultImageSize; } }
		public Size DefaultItemImageFullSize { get { return defaultImageFullSize; } }
		public Size ImageBackgroundIndent { get { return imageBackgroundIndent; } }
		public int ButtonPaddingHeight { get { return buttonPaddingHeight; } set { buttonPaddingHeight = value; } }
		public int ButtonPaddingWidth { get { return buttonPaddingWidth; } set { buttonPaddingWidth = value; } }
		protected int StartRowIndex { get { return startRowIndex; } set { startRowIndex = value; } }
		protected int VisibleRowCount { get { return visibleRowCount; } set { visibleRowCount = value; } }
		public GraphicsInfo GInfo { get { return ginfo; } }
		public GalleryItemGroupViewInfoCollection Groups { get { return groups; } }
		public virtual BaseGallery Gallery { get { return gallery; } }
		public virtual Size ControlSize { get { return Size.Empty; } }
		public virtual Size VisibleSize { get { return viVisibleSize; } }
		public Size ItemMaxImageSize { get { return viItemMaxImageSize; } }
		public Size ItemMaxSize { get { return viItemMaxSize; } }
		public virtual Size BestSize { get { return viBestSize; } }
		public bool IsReady { get { return isReady; } set { isReady = value; }  }
		public bool IsCalculateBestSize { get { return calculateBestSize; } set { calculateBestSize = value; } }
		public void Reset() { 
			IsReady = false;
			IsCalculateBestSize = true;
			SetAppearanceDirty();
		}
		public  virtual void SetDirty() {
			IsReady = false;
		}
		public abstract Locations GetItemImageLocation();
		public void CalcViewInfo(Rectangle bounds) {
			if(!Gallery.ForceCalcViewInfo && (IsReady || Gallery.IsLockUpdate)) return;
			SetAppearanceDirty(); 
			GInfo.AddGraphics(null);
			try {
				CalcConstants();
				CalcViewInfoCore(bounds);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			if(bounds.Size.IsEmpty) {
				Bounds = Rectangle.Empty;
				return;
			}
			this.isReady = true;
			MakeVisible(MakeVisibleItem);
		}
		protected virtual void CheckBestSize() {
			CreateGroupsInfo();
			if(IsCalculateBestSize == true) {
				CalcBestSize();
				IsCalculateBestSize = false;
			}
		}
		Control ISupportXtraAnimation.OwnerControl { get { return OwnerControl; } }
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		CustomAnimationInvoker animationInvoker = null;
		internal CustomAnimationInvoker AnimationInvoker { 
			get {
				if(animationInvoker == null) animationInvoker = new CustomAnimationInvoker(OnAnimation);
				return animationInvoker;
			} 
		}
		protected virtual bool ContainsItem(GalleryItem item) {
			for(int i = 0; i < Groups.Count; i++) {
				if(Groups[i].Group.Items.Contains(item)) return true;
			}
			return false;
		}
		protected virtual void RemoveInvisibleAnimatedItems() {
			for(int i = 0; i < XtraAnimator.Current.Animations.Count; i++) {
				GalleryItemAnimationInfo info = XtraAnimator.Current.Animations[i] as GalleryItemAnimationInfo;
				if(info == null) continue;
				if(info.ItemInfo.IsInvisible || !ContainsItem(info.Item))
					RemoveAnimatedItem(info);
			}
		}
		protected virtual void RemoveAnimatedItem(GalleryItemAnimationInfo info) {
			XtraAnimator.RemoveObject(info.AnimatedObject, info.AnimationId);
		}
		protected virtual bool ShouldAddAnimatedItem(GalleryItemViewInfo info) {
			if(info.IsInvisible) return false;
			IAnimatedItem item = info as IAnimatedItem;
			if(!item.IsAnimated) return false;
			return XtraAnimator.Current.Get(this, info.Item) == null;
		}
		protected virtual void AddAnimatedItems() {
			for(int i = 0; i < Groups.Count; i++) {
				for(int j = 0; j < Groups[i].Items.Count; j++) {
					if(!ShouldAddAnimatedItem(Groups[i].Items[j])) continue;
						XtraAnimator.Current.AddAnimation(new GalleryItemAnimationInfo(this, Groups[i].Items[j], AnimationInvoker));	   
				}
			}
		}
		protected internal void RemoveAnimatedItem(GalleryItem item) {
			XtraAnimator.Current.Animations.Remove(this, item);
		}
		protected internal virtual void ClearAnimatedItems() {
			XtraAnimator.RemoveObject(this);
		}
		protected virtual void UpdateAimatedItems() {
			RemoveInvisibleAnimatedItems();
			AddAnimatedItems();
		}
		protected virtual void OnAnimation(BaseAnimationInfo info) {
			GalleryItemAnimationInfo itemInfo = info as GalleryItemAnimationInfo;
			if(itemInfo == null || OwnerControl == null) return;
			IAnimatedItem item = itemInfo.ItemInfo as IAnimatedItem;
			if(item == null) return;
			item.UpdateAnimation(info);
			OwnerControl.Invalidate(item.AnimationBounds);
			if(!Gallery.AllowHoverImages) return;
			GalleryItemImagePopupForm form = Gallery.FindForm(itemInfo.ItemInfo);
			if(form != null) form.Invalidate(form.ClientRectangle);
		}
		protected virtual void CalcViewInfoCore(Rectangle bounds) {
			this.bounds = bounds;
			this.contentBounds = CalcContentBounds();
			CheckBestSize();
			if(Bounds.Size.IsEmpty) return;
			this.viVisibleSize = CalcVisibleSize(Gallery.RowCount, ActualColCount);
			if(!IsAutoSizeGallery)
				this.viVisibleSize = contentBounds.Size;
			else {
				this.bounds = new Rectangle(bounds.Location, VisibleSize);
				this.contentBounds = CalcContentBounds();
			}
			LayoutGroups();
			UpdateGroupsLayout();
			UpdateScrollYPosition();
			UpdateRowsInfo();
		}
		protected virtual bool IsAutoSizeGallery { get { return false; } }
		protected internal virtual int ActualColCount { get { return ColCount; } }
		protected virtual int GetStartRowIndex() {
			int rowIndex = -1;
			int prevDefineRowCoor = int.MinValue;
			foreach(GalleryItemGroupViewInfo groupInfo in Groups) {
				foreach(GalleryItemViewInfo itemInfo in groupInfo.Items) {
					if(GetDefineCoor(itemInfo.Bounds.Location) != prevDefineRowCoor) {
						rowIndex++;
						prevDefineRowCoor = GetDefineCoor(itemInfo.Bounds.Location);
					}
					if(GetDefineCoor(itemInfo.Bounds) >= GetDefineCoor(ContentBounds)) {
						return rowIndex;
					}
				}
			}
			return 0;
		}
		protected virtual int GetEndRowIndex() {
			int rowIndex = -1;
			int prevDefineRowCoor = int.MinValue;
			foreach(GalleryItemGroupViewInfo groupInfo in Groups) {
				foreach(GalleryItemViewInfo itemInfo in groupInfo.Items) {
					if(GetDefineCoor(itemInfo.Bounds.Location) != prevDefineRowCoor) {
						rowIndex++;
						prevDefineRowCoor = GetDefineCoor(itemInfo.Bounds.Location);
					}
					if(GetDefineEndCoor(itemInfo.Bounds) > GetDefineEndCoor(ContentBounds)) {
						return rowIndex;
					}
				}
			}
			return rowIndex;
		}
		protected void UpdateRowsInfo() {
			StartRowIndex = GetStartRowIndex();
			VisibleRowCount = GetEndRowIndex() - StartRowIndex;
		}
		public virtual int OffsetFromContentTop { get { return 0; } }
		public virtual int RealColsWidth {
			get {
				if(RealColCount == 0)
					return 0;
				return RealColCount * GetSecondarySize(ItemMaxSize) + (RealColCount - 1) * HDistanceBetweenItems;
			}
		}
		public virtual int RealRowsHeight {
			get {
				if(RealRowCount == 0) return 0;
				return RealRowCount * GetDefineSize(ItemMaxSize) + (RealRowCount - 1) * VDistanceBetweenItems;
			}
		}
		public virtual int RealRowCount {
			get {
				if(GetDefineSize(ItemMaxSize) + VDistanceBetweenItems == 0) return 0; 
				return (GetDefineSize(GalleryContentBounds) + VDistanceBetweenItems) / (GetDefineSize(ItemMaxSize) + VDistanceBetweenItems); 
			}
		}
		public virtual int RealColCount { 
			get {
				int width = GetSecondarySize(ItemMaxSize) + HDistanceBetweenItems;
				if(width == 0) return 0;
				return (GetSecondarySize(VisibleSize) + HDistanceBetweenItems) / width;
			}
		}
		public virtual int MinScrollYPosition {
			get { return 0; }
		}
		public virtual int MaxScrollYPosition {
			get { 
				if(GetDefineSize(BestSize) == 0) return 0;
				return Math.Max(0, GetDefineSize(BestSize) - GetDefineSize(GalleryContentBounds));  
			}
		}
		public virtual void UpdateScrollYPosition() {
			if(ScrollYPosition > MaxScrollYPosition) {
				SetScrollYPosition(null, MaxScrollYPosition);
				LayoutGroups();
				UpdateGroupsLayout();
			}
		}
		protected virtual int CalcGalleryAutoHeight() {
			isReady = false;
			IsCalculateBestSize = true;
			CalcViewInfo(new Rectangle(0, 0, 10000, 10000));
			return GetDefineSize(BestSize);
		}
		public virtual Size CalcGalleryBestSize(int row, int col) {
			IsCalculatingBestSize = true;
			GInfo.AddGraphics(null);
			try {
				if(!IsReady) CalcConstants();
				CheckBestSize();
				Size sz = CalcVisibleSize(row, col);
				Rectangle rect = CalcBoundsByContentBounds(new Rectangle(Point.Empty, sz));
				return new Size(rect.Width, sz.Height);
			}
			finally {
				IsCalculatingBestSize = false;
				GInfo.ReleaseGraphics();
			}
		}
		public virtual Size CalcGalleryBestSize() {
			return CalcGalleryBestSize(Gallery.RowCount, ActualColCount);
		}
		public RibbonControl Ribbon { get { return Gallery.Ribbon; } }
		protected RibbonViewInfo RibbonViewInfo { get { return Ribbon == null ? null : Ribbon.ViewInfo; } }
		public bool IsPaintAppearanceDirty { get { return paintAppearanceDirty; } }
		public RibbonGalleryAppearances PaintAppearance {
			get {
				if(IsPaintAppearanceDirty) UpdatePaintAppearance();
				return paintAppearance;
			}
		}
		public void SetAppearanceDirty() {
			this.paintAppearanceDirty = true;
			this.defaultAppearance = null;
			this.largestCaptionFontAppearance = null;
			this.largestDescriptionFontAppearance = null;
		}
		protected virtual void UpdatePaintAppearance() {
			this.paintAppearanceDirty = false;
			this.largestCaptionFontAppearance = null;
			this.largestDescriptionFontAppearance = null;
			PaintAppearance.Combine(Gallery.Appearance, Gallery.GetController().AppearancesRibbon.Gallery, DefaultAppearance);
		}
		RibbonGalleryAppearances defaultAppearance;
		protected internal RibbonGalleryAppearances DefaultAppearance {
			get {
				if(defaultAppearance == null) defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
			set {
				defaultAppearance = value;
			}
		}
		protected virtual ISkinProvider Provider { get { return Gallery.Provider; } }
		AppearanceDefault GetAppearance(string skinName, AppearanceDefault defApp) {
			SkinElement element = RibbonSkins.GetSkin(Provider)[skinName];
			if(element != null)
				element.Apply(defApp);
			return defApp;
		}
		protected virtual RibbonGalleryAppearances CreateDefaultAppearance() {
			RibbonGalleryAppearances res = new RibbonGalleryAppearances(null);
			AppearanceDefault item = GetAppearance(RibbonSkins.SkinPopupGalleryFilterPanel, new AppearanceDefault(Color.Black, Color.Empty));
			res.FilterPanelCaption.Assign(GetAppearance(RibbonSkins.SkinPopupGalleryFilterPanel, new AppearanceDefault(Color.Black, Color.Empty)));
			res.GroupCaption.Assign(GetAppearance(RibbonSkins.SkinPopupGalleryGroupCaption, new AppearanceDefault(Color.Black, Color.Empty)));
			AppearanceDefault app = GetAppearance(RibbonSkins.SkinPopupGalleryItemCaption, new AppearanceDefault(Color.Black, Color.Empty));
			if(Gallery.ItemImageLocation == Locations.Top) {
				app.VAlignment = VertAlignment.Top;
				app.HAlignment = HorzAlignment.Center;
			}
			ApplyAlignmentByImageLocation(app);
			res.ItemCaptionAppearance.Assign(app);
			ApplyWordWrapByItemAutoSize(res.ItemCaptionAppearance);
			res.ItemCaptionAppearance.Hovered.ForeColor = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinPopupGalleryItemCaption].GetForeColor(res.ItemCaptionAppearance.Hovered, ObjectState.Hot);
			res.ItemCaptionAppearance.Pressed.ForeColor = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinPopupGalleryItemCaption].GetForeColor(res.ItemCaptionAppearance.Pressed, ObjectState.Pressed);
			res.ItemCaptionAppearance.Disabled.ForeColor = Color.Empty;
			res.ItemCaptionAppearance.Disabled.ForeColor = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinPopupGalleryItemCaption].GetForeColor(res.ItemCaptionAppearance.Disabled, ObjectState.Disabled);
			if(res.ItemCaptionAppearance.Disabled.ForeColor.IsEmpty)
				res.ItemCaptionAppearance.Disabled.ForeColor = RibbonSkins.GetSkin(Provider).GetSystemColor(SystemColors.GrayText);
			app = GetAppearance(RibbonSkins.SkinPopupGalleryItemSubCaption, new AppearanceDefault(Color.Black, Color.Empty));
			ApplyAlignmentByImageLocation(app);
			res.ItemDescriptionAppearance.Assign(app);
			ApplyWordWrapByItemAutoSize(res.ItemDescriptionAppearance);
			res.ItemDescriptionAppearance.Hovered.ForeColor = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinPopupGalleryItemSubCaption].GetForeColor(res.ItemDescriptionAppearance.Hovered, ObjectState.Hot);
			res.ItemDescriptionAppearance.Pressed.ForeColor = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinPopupGalleryItemSubCaption].GetForeColor(res.ItemDescriptionAppearance.Pressed, ObjectState.Pressed);
			res.ItemDescriptionAppearance.Disabled.ForeColor = Color.Empty;
			res.ItemDescriptionAppearance.Disabled.ForeColor = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinPopupGalleryItemSubCaption].GetForeColor(res.ItemDescriptionAppearance.Disabled, ObjectState.Disabled);
			if(res.ItemDescriptionAppearance.Disabled.ForeColor.IsEmpty)
				res.ItemDescriptionAppearance.Disabled.ForeColor = RibbonSkins.GetSkin(Provider).GetSystemColor(SystemColors.GrayText);
			return res;
		}
		void ApplyWordWrapByItemAutoSize(StateAppearances app) {
			if(Gallery.ItemAutoSizeMode != GalleryItemAutoSizeMode.Default || Gallery.MaxItemWidth > 0) {
				app.Normal.TextOptions.WordWrap = WordWrap.Wrap;
				app.Hovered.TextOptions.WordWrap = WordWrap.Wrap;
				app.Pressed.TextOptions.WordWrap = WordWrap.Wrap;
				app.Disabled.TextOptions.WordWrap = WordWrap.Wrap;
			}
		}
		void ApplyAlignmentByImageLocation(AppearanceDefault app) {
			if(Gallery.ItemImageLocation == Locations.Top || Gallery.ItemImageLocation == Locations.Default) {
				app.VAlignment = VertAlignment.Top;
				app.HAlignment = HorzAlignment.Center;
			}
			else if(Gallery.ItemImageLocation == Locations.Bottom) {
				app.VAlignment = VertAlignment.Bottom;
				app.HAlignment = HorzAlignment.Center;
			}
			else if(Gallery.ItemImageLocation == Locations.Left) {
				app.VAlignment = VertAlignment.Center;
				app.HAlignment = HorzAlignment.Near;
			}
			else if(Gallery.ItemImageLocation == Locations.Right) {
				app.VAlignment = VertAlignment.Center;
				app.HAlignment = HorzAlignment.Far;
			}
		}
		protected virtual Rectangle CalcContentBounds() {
			return SkinElementPainter.GetObjectClientRectangle(null, SkinElementPainter.Default, Gallery.Painter.GetBackgroundInfo(this), Bounds);
		}
		protected virtual void CalcBestSize() {
			for(int i = 0; i < Groups.Count; i++) {
				Groups[i].CalcBestSize();
			}
			this.viItemMaxImageSize = GetItemMaxImageSize();
			this.viItemMaxSize = GetItemMaxSize();
		}
		protected virtual int GetMaxCaptionWidth() {
			int maxWidth = 0;
			for(int i = 0; i < Groups.Count; i++) {
				maxWidth = Math.Max(maxWidth, GetSecondarySize(Groups[i].CaptionBestSize));
			}
			return maxWidth;
		}
		protected virtual Size ConstrainVisibleSize(Size size) {
			return size; 
		}
		protected internal virtual int HDistanceBetweenItems { get { return Gallery.DistanceBetweenItems; } }
		protected internal virtual int VDistanceBetweenItems { get { return Gallery.DistanceBetweenItems; } }
		public virtual int IndentBetweenContentAndControl { get { return 0; } }
		protected virtual int CalcRowsInGroup(int index, int col) {
			int groupItemCount = Groups[index].Items.Count;
			if(col == 0) return 0;
			int rowCount = groupItemCount / col;
			rowCount += groupItemCount % col > 0 ? 1 : 0;
			return rowCount;
		}
		protected virtual int CalcHeightForRows(int row, int col) {
			if(!Gallery.ShowGroupCaption)
				return GetDefineSize(ItemMaxSize) * row + Math.Max((row - 1) * VDistanceBetweenItems, 0);
			int height = 0;
			int rc = 0;
			for(int i = 0; i < Groups.Count; i++) {
				Groups[i].CalcBestSize();
				int rowsInGroup = CalcRowsInGroup(i, col);
				if(rc + rowsInGroup > row) rowsInGroup = row - rc;
				rc += rowsInGroup;
				height += GetDefineSize(Groups[i].CaptionBounds) + rowsInGroup * GetDefineSize(ItemMaxSize);
				if(rowsInGroup > 1) height += (rowsInGroup - 1) * VDistanceBetweenItems;
				if(rc == row)
					break;
			}
			if(rc < row) {
				int emptyRowCount = row - rc;
				height += GetDefineSize(ItemMaxSize) * emptyRowCount + Math.Max(emptyRowCount * VDistanceBetweenItems, 0);
			}
			return height;
		}
		protected virtual int CalcWidthForColumns(int col) {
			return ItemMaxSize.Width * col + Math.Max((col - 1) * HDistanceBetweenItems, 0);
		}
		protected virtual Size GetDefaultSize() { 
			Size res = new Size(32, Ribbon.ViewInfo.GroupContentHeight);
			return new Size(GetSecondarySize(res), GetDefineSize(res));
		}
		protected internal virtual Padding ActualGroupContentMargin {
			get {
				return Gallery.ShowGroupCaption ? Gallery.GroupContentMargin : Padding.Empty;
			}
		}
		protected internal virtual Size CalcVisibleSize(int row, int col) {
			Size sz = new Size(CalcWidthForColumns(col), CalcHeightForRows(row, col));
			Rectangle rect = new Rectangle(0, 0, GetSecondarySize(sz), GetDefineSize(sz));
			if(Gallery.DesignModeCore) { 
				if(rect.Width == 0) rect.Width = GetDefaultSize().Width;
				if(rect.Height == 0) rect.Height = GetDefaultSize().Height;
			}
			Size size = ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, Gallery.Painter.GetBackgroundInfo(this), rect).Size;
			if(IsVertical) {
				size.Width = Math.Max(size.Width, GetMaxCaptionWidth());
				size.Width += ActualGroupContentMargin.Horizontal;
			}
			else {
				size.Height = Math.Max(size.Height, GetMaxCaptionWidth());
				size.Height += ActualGroupContentMargin.Horizontal;
			}
			UpdateControlVisibility(ConstrainVisibleSize(size));
			if(IsVertical)
				size.Width += ControlSize.Width + IndentBetweenContentAndControl;
			else
				size.Height += ControlSize.Height + IndentBetweenContentAndControl;
			return ConstrainVisibleSize(size);
		}
		protected virtual void UpdateControlVisibility(Size size) {
		}
		protected virtual Size GetItemMaxImageSize() {
			int maxWidth = 0, maxHeight = 0;
			foreach(GalleryItemGroupViewInfo groupInfo in Groups) {
				foreach(GalleryItemViewInfo itemInfo in groupInfo.Items) {
					maxWidth = Math.Max(maxWidth, itemInfo.ImageBounds.Width);
					maxHeight = Math.Max(maxHeight, itemInfo.ImageBounds.Height);
				}
			}
			return new Size(maxWidth, maxHeight);
		}
		protected internal virtual Size GetItemMaxSize() {
			return new Size(GetItemMaxWidth(), GetItemMaxHeight());
		}
		protected virtual int GetItemMaxWidth() {
			int maxWidth = 0;
			for(int i = 0; i < Groups.Count; i++) {
				maxWidth = Math.Max(maxWidth, Groups[i].GetMaxWidth());
			}
			return maxWidth;
		}
		protected virtual int GetItemMaxHeight() {
			int maxHeight = 0;
			for(int i = 0; i < Groups.Count; i++) {
				maxHeight = Math.Max(maxHeight, Groups[i].GetMaxHeight());
			}
			return maxHeight;
		}
		protected internal virtual void UpdateGroupsLayout() {
			for(int i = 0; i < Groups.Count; i++) {
				Groups[i].UpdateGroupLayout();
			}
			UpdateAimatedItems();
		}
		protected internal virtual bool IsVertical { get { return true; } }
		protected internal virtual Point ApplyOrientation(Point pt) { return IsVertical ? pt : new Point(pt.Y, pt.X); }
		protected internal virtual Size ApplyOrientation(Size sz) { return IsVertical ? sz : new Size(sz.Height, sz.Width); }
		protected internal virtual int GetDefineCoor(Point pt) { return IsVertical ? pt.Y : pt.X; }
		protected internal virtual int GetDefineCoor(Rectangle rect) { return GetDefineCoor(rect.Location); }
		protected internal virtual int GetSecondaryCoor(Rectangle rect) { return GetSecondaryCoor(rect.Location); }
		protected internal virtual int GetSecondaryCoor(Point pt) { return IsVertical ? pt.X : pt.Y; }
		protected internal virtual int GetDefineSize(Size sz) { return IsVertical ? sz.Height : sz.Width; }
		protected internal virtual int GetDefineSize(Rectangle rect) { return GetDefineSize(rect.Size); }
		protected internal virtual int GetSecondarySize(Size sz) { return IsVertical ? sz.Width : sz.Height; }
		protected internal virtual int GetSecondarySize(Rectangle rect) { return GetSecondarySize(rect.Size); }
		protected internal virtual int GetDefineEndCoor(Rectangle rect) { return IsVertical ? rect.Bottom : rect.Right; }
		protected internal virtual int GetSecondaryEndCoor(Rectangle rect) { return IsVertical ? rect.Right: rect.Bottom; }
		protected internal virtual void OffsetDefineSize(ref Size sz, int offset) {
			if(IsVertical)
				sz.Height += offset;
			else
				sz.Width += offset;
		}
		protected internal virtual void OffsetDefineCoor(ref Point pt, int offset) {
			if(IsVertical)
				pt.Y += offset;
			else
				pt.X += offset;
		}
		protected internal virtual void OffsetDefineCoor(ref Rectangle rect, int offset) {
			if(IsVertical)
				rect.Y += offset;
			else
				rect.X += offset;
		}
		protected internal virtual void OffsetSecondaryCoor(ref Point pt, int offset) {
			if(IsVertical)
				pt.X += offset;
			else
				pt.Y += offset;
		}
		protected virtual Point UpdateLocation(Point location) {
			Point res = location;
			OffsetDefineCoor(ref res, -ScrollYPosition);
			return res;
		}
		protected void LoadImage(GalleryItemViewInfo itemInfo) {
			itemInfo.Item.LockRefresh();
			itemInfo.Item.ImageInfo.ImageMaxSize = itemInfo.Gallery.ImageSize;
			itemInfo.Item.Image = itemInfo.Gallery.ImageLoader.LoadImage(itemInfo.Item.ImageInfo);
			itemInfo.Item.UnlockRefresh();
		}
		protected bool ShouldLoadImage(GalleryItemViewInfo info) {
			return !info.IsInvisible && info.Item != null && !info.Item.ImageInfo.IsLoaded && info.Item.Image == null && info.Gallery != null;
		}
		protected void LoadImages(GalleryItemGroupViewInfo group) {
			if(Gallery == null || !Gallery.OptionsImageLoad.AsyncLoad) return;
			for(int j = 0; j < group.Items.Count; j++) {
				if(ShouldLoadImage(group.Items[j]))
					LoadImage(group.Items[j]);
			}
		}
		protected virtual void LayoutGroupsCore() {
			Point location = UpdateLocation(GalleryContentBounds.Location);
			Size size = Size.Empty;
			this.viBestSize = Size.Empty;
			RowCount = 0;
			for(int i = 0; i < Groups.Count; i++) {
				LoadImages(Groups[i]);
				size = Groups[i].LayoutItems(ItemMaxSize, location);
				OffsetDefineCoor(ref location, GetDefineSize(size));
				if(IsVertical) {
					this.viBestSize.Width = size.Width;
					this.viBestSize.Height += size.Height;
				}
				else {
					this.viBestSize.Width += size.Width;
					this.viBestSize.Height = size.Height;
				}
			}
		}
		protected internal virtual void LayoutGroups() {
			LayoutGroupsCore(); 
			LayoutGroupsCore(); 
		}
		public virtual SkinPaddingEdges BackgroundPaddings {
			get {
				SkinElementInfo info = Gallery.Painter.GetBackgroundInfo(this);
				return info.Element.ContentMargins;
			}
		}
		protected virtual Rectangle CalcContentBoundsByBounds() {
			SkinElementInfo info = Gallery.Painter.GetBackgroundInfo(this);
			info.Bounds = Bounds;
			return ObjectPainter.GetObjectClientRectangle(null, SkinElementPainter.Default, info);
		}
		protected virtual Rectangle CalcBoundsByContentBounds(Rectangle cb) {
			SkinElementInfo info = Gallery.Painter.GetBackgroundInfo(this);
			info.Bounds = cb;
			return ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, info, cb);
		}
		protected virtual Rectangle CalcBoundsByContentBounds() {
			return CalcBoundsByContentBounds(ContentBounds);
		}
		public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual Rectangle ContentBounds { get { return contentBounds; } set { contentBounds = value; }  }
		public virtual Rectangle GalleryContentBounds { get { return ContentBounds; } }
		protected virtual void ClearGroupsInfo() {
			Groups.Clear();
		}
		protected virtual bool ShouldFilterGroups { get { return true; } }
		protected virtual GalleryItemGroupViewInfo CreateGroupInfo(BaseGalleryViewInfo galleryInfo, GalleryItemGroup group) { 
			return new GalleryItemGroupViewInfo(galleryInfo, group);
		}
		protected virtual void CreateGroupsInfo() {
			if(!IsCalculateBestSize && Gallery.Groups.Count != 0) return;
			IsCalculateBestSize = true;
			ClearGroupsInfo();
			for(int i = 0; i < Gallery.Groups.Count; i++) {
				if(!Gallery.Groups[i].Visible) continue;
				Groups.Add(CreateGroupInfo(this, Gallery.Groups[i]));
				Groups[Groups.Count - 1].CreateItemsViewInfo();
			}	
		}
		public virtual int ColCount { get { return Gallery.ColumnCount; } set { } }
		protected internal virtual void CalcHitInfo(RibbonHitInfo hitInfo) { }
		protected internal abstract int ScrollYPosition { get; }
		protected internal Size GetHoverImageSize(GalleryItemViewInfo itemInfo) {
			Size small = GetImageSize(itemInfo);
			Size res = GetImageSizeCore(itemInfo.Item.HoverImage, Gallery.HoverImages, itemInfo.Item.HoverImageIndex, Gallery.FixedHoverImageSize, Gallery.HoverImageSize, new Size(32, 32));
			if(res.Width / 0.5f < small.Width || res.Height / 0.5f < small.Height) {
				res = new Size(small.Width * 2, small.Height * 2);
			}
			return res;
		}
		protected internal GalleryItemGroupViewInfo GetGroupInfo(GalleryItemGroup group) {
			foreach(GalleryItemGroupViewInfo vi in Groups) {
				if(vi.Group == group)
					return vi;
			}
			return null;
		}
		protected internal GalleryItemViewInfo GetItemInfo(GalleryItem item) {
			foreach(GalleryItemGroupViewInfo gi in Groups) {
				foreach(GalleryItemViewInfo ii in gi.Items) {
					if(ii.Item == item)
						return ii;
				}
			}
			return null;
		}
		protected virtual Size FixedImageDefaultSize { get { return new Size(16, 16); } }
		protected internal Size GetImageSize(GalleryItemViewInfo itemInfo, bool useFixedSize) {
			return GetImageSizeCore(itemInfo.Item.Image, Gallery.Images, itemInfo.Item.ImageIndex, useFixedSize, Gallery.ImageSize, FixedImageDefaultSize);
		}
		protected internal Size GetImageSize(GalleryItemViewInfo itemInfo) {
			return GetImageSize(itemInfo, Gallery.FixedImageSize);
		}
		protected virtual Size GetImageSizeCore(Image image, object images, int imageIndex, bool isFixedSize, Size fixedSize, Size defaultSize) {
			Size res = Size.Empty;
			if(!isFixedSize) {
				res = ImageCollection.GetImageListSize(image, images, imageIndex);
				if(res == Size.Empty) res = ImageCollection.GetImageListSize(images);
				if(res != Size.Empty) return res;
			}
			res = fixedSize;
			if(res.Width < 1 || res.Height < 1) res = ImageCollection.GetImageListSize(images);
			if(res.IsEmpty) return defaultSize;
			return res;
		}
		protected internal void CheckViewInfo() {
			if(IsReady) return;
			CalcViewInfo(Bounds);
		}
		protected virtual GalleryItemViewInfo FindItemInfo(GalleryItem item) {
			for(int i = 0; i < Groups.Count; i++) {
				for(int j = 0; j < Groups[i].Items.Count; j++) {
					if(Groups[i].Items[j].Item == item) return Groups[i].Items[j];
				}
			}
			return null;
		}
		GalleryItem makeVisibleItem;
		protected GalleryItem MakeVisibleItem { get { return makeVisibleItem; } set { makeVisibleItem = value; } }
		protected internal virtual bool AllowPartitalItems { get { return false; } }
		protected abstract void SetScrollYPosition(GalleryItemViewInfo itemInfo, int pos);
		protected virtual int GetItemRow(GalleryItem item) {
			int rowCount = 0;
			GalleryItemViewInfo lastItem = null;
			foreach(GalleryItemGroupViewInfo gi in groups) {
				foreach(GalleryItemViewInfo ii in gi.Items) {
					if(lastItem == null) {
						lastItem = ii;
						continue;
					}
					if(GetDefineCoor(lastItem.Bounds.Location) != GetDefineCoor(ii.Bounds.Location)) {
						rowCount++;
						lastItem = ii;
					}
					if(ii.Item == item)
						return rowCount;
				}
			}
			return 0;
		}
		public virtual void MakeVisible(GalleryItem item) {
			if(item == null) return;
			this.makeVisibleItem = item;
			if(!IsReady || GetDefineSize(BestSize) == 0 || Bounds.IsEmpty) return;
			if(GetDefineSize(BestSize) <= GetDefineSize(GalleryContentBounds)) {
				this.makeVisibleItem = null;
				return;
			}
			GalleryItemViewInfo itemInfo = FindItemInfo(item);
			if(itemInfo == null) return;
			this.makeVisibleItem = null;
			if(itemInfo.GalleryInfo.GalleryContentBounds.Contains(itemInfo.Bounds)) return;
			int contentHeight = itemInfo.GalleryInfo.RealRowsHeight;
			if((GetDefineEndCoor(itemInfo.Bounds)  - OffsetFromContentTop - GetDefineCoor(itemInfo.GroupInfo.Bounds.Location)) <= contentHeight && Gallery.ShowGroupCaption)
				SetScrollYPosition(itemInfo, GetDefineCoor(itemInfo.GroupInfo.Bounds.Location) - OffsetFromContentTop - GetDefineCoor(GalleryContentBounds.Location) + ScrollYPosition);
			else if(GetDefineSize(BestSize) - (GetDefineCoor(itemInfo.Bounds.Location) - OffsetFromContentTop - GetDefineCoor(GalleryContentBounds.Location) + ScrollYPosition) < contentHeight)
				SetScrollYPosition(itemInfo, GetDefineSize(BestSize) - contentHeight);
			else
				SetScrollYPosition(itemInfo, GetDefineCoor(itemInfo.Bounds.Location) - OffsetFromContentTop + ScrollYPosition - GetDefineCoor(GalleryContentBounds.Location));
		}
		protected internal virtual void AddBitmapFadeAnimation(RibbonHitInfo prev, RibbonHitInfo current) { }
		protected virtual bool CanAnimate { get { return false; } }
		protected internal abstract ImageGalleryInfoArgs CreateGalleryInfo(BaseRibbonViewInfo viewInfo);
		protected internal virtual Rectangle TranslateRect(Rectangle captionContentBounds) {
			return captionContentBounds; 
		}
		protected internal virtual void NotifyHotTrack(RibbonHitInfo hitInfo, bool hotTracked) {
		}
		AppearanceObject largestCaptionFontAppearance = null;
		protected internal virtual AppearanceObject LargestCaptionFontAppearance {
			get {
				if(largestCaptionFontAppearance == null) {
					largestCaptionFontAppearance = GetLargestFontAppearance(PaintAppearance.ItemCaptionAppearance);
				}
				return largestCaptionFontAppearance;
			} 
		}
		AppearanceObject GetLargestFontAppearance(StateAppearances app) {
			AppearanceObject maxApp = null;
			int max, curr;
			max = app.GetAppearance(ObjectState.Normal).CalcTextSize(GInfo.Graphics, "Wg", 0).ToSize().Width;
			maxApp = app.GetAppearance(ObjectState.Normal);
			curr = app.GetAppearance(ObjectState.Hot).CalcTextSize(GInfo.Graphics, "Wg", 0).ToSize().Width;
			if(curr > max) {
				max = curr;
				maxApp = app.GetAppearance(ObjectState.Hot);
			}
			curr = app.GetAppearance(ObjectState.Pressed).CalcTextSize(GInfo.Graphics, "Wg", 0).ToSize().Width;
			if(curr > max) {
				max = curr;
				maxApp = app.GetAppearance(ObjectState.Pressed);
			}
			curr = app.GetAppearance(ObjectState.Disabled).CalcTextSize(GInfo.Graphics, "Wg", 0).ToSize().Width;
			if(curr > max) {
				max = curr;
				maxApp = app.GetAppearance(ObjectState.Disabled);
			}
			return maxApp;
		}
		AppearanceObject largestDescriptionFontAppearance;
		protected internal virtual AppearanceObject LargestDescriptionFontAppearance {
			get {
				if(largestDescriptionFontAppearance == null) {
					largestDescriptionFontAppearance = GetLargestFontAppearance(PaintAppearance.ItemDescriptionAppearance);
				}
				return largestDescriptionFontAppearance;
			}
		}
		protected internal virtual bool IsItemHovered(GalleryItemViewInfo itemInfo) {
			return false;
		}
		protected internal bool IsCalculatingBestSize { get; set; }
	}
	public class InDropDownGalleryViewInfo : StandaloneGalleryViewInfo {
		Rectangle resizeRectBounds, resizeCornerBounds;
		public InDropDownGalleryViewInfo(InDropDownGallery gallery) : base(gallery) { }
		public Rectangle ResizeRectBounds { get { return resizeRectBounds; } }
		public Rectangle ResizeCornerBounds { get { return resizeCornerBounds; } }
		public new InDropDownGallery Gallery { get { return base.Gallery as InDropDownGallery; } }
		public override Size CalcGalleryBestSize() {
			Size size = base.CalcGalleryBestSize();
			CalcViewInfo(new Rectangle(Point.Empty, size));
			size = base.CalcGalleryBestSize();
			return size;
		}
		protected internal override Size CalcVisibleSize(int row, int col) {
			Size res = base.CalcVisibleSize(row, col);
			if(!IsCalculatingBestSize && Gallery.GetAutoSize() == GallerySizeMode.Vertical) {
				res.Height = ContentBounds.Height;
				UpdateControlVisibility(res);
			}
			return res;
		}
		public virtual void CalcSizerInfo(Rectangle sizerBounds) {
			this.resizeRectBounds = sizerBounds;
			Rectangle r = ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, Gallery.Painter.GetSizerPanelInfo(this));
			this.resizeCornerBounds.Size = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, Gallery.Painter.GetSizerCornerInfo(this)).Size;
			this.resizeCornerBounds = RectangleHelper.GetCenterBounds(sizerBounds, this.resizeCornerBounds.Size);
			if(Gallery.SizeMode == GallerySizeMode.Both) this.resizeCornerBounds.X = r.Right - this.resizeCornerBounds.Width;
		}
		protected internal override int ActualColCount {
			get {
				List<GalleryItem> res = Gallery.GetAllItems();
				return Math.Max(1, Math.Min(res.Count, ColCount));
			}
		}
		protected internal virtual bool IsGalleryResizing {
			get { return !(Gallery.SizeMode == GallerySizeMode.Default || Gallery.SizeMode == GallerySizeMode.None); }
		}
		protected internal override void CalcHitInfo(RibbonHitInfo hitInfo) {
			if(hitInfo.ContainsSet(ResizeCornerBounds, RibbonHitTest.GallerySizeGrip)) return;
			if(hitInfo.ContainsSet(ResizeRectBounds, RibbonHitTest.GallerySizingPanel)) return;
			base.CalcHitInfo(hitInfo);
		}
		protected internal override void UpdateHitPoint(DXMouseEventArgs e) {
			if(Gallery.SizerBelow) HitPoint = new Point(e.X, e.Y);
			else HitPoint = new Point(Bounds.Size.Width - PointOffset.Width, PointOffset.Height);
		}
	}
	public class GalleryControlGalleryViewInfo : StandaloneGalleryViewInfo {
		Rectangle backgroundImageRect;
		public GalleryControlGalleryViewInfo(GalleryControlGallery gallery) : base(gallery) { }
		public Rectangle BackgroundImageRect { get { return backgroundImageRect; } }
		public new GalleryControlGallery Gallery { get { return base.Gallery as GalleryControlGallery; } }
		protected internal override bool IsVertical { get { return Gallery.Orientation == Orientation.Vertical; } }
		protected GalleryControl GalleryControl { get { return Gallery.GalleryControl; } }
		internal Size CalcGalleryBestSizeCore(Size sz, bool useDocking) {
			if(useDocking) {
				if(GalleryControl.Dock == DockStyle.Top || GalleryControl.Dock == DockStyle.Bottom || GalleryControl.Dock == DockStyle.Fill)
					sz.Width = GalleryControl.Width;
				if(GalleryControl.Dock == DockStyle.Left || GalleryControl.Dock == DockStyle.Right || GalleryControl.Dock == DockStyle.Fill)
					sz.Height = GalleryControl.Height;
			}
			return base.CalcGalleryBestSize(sz);
		}
		public override Size CalcGalleryBestSize(Size sz) {
			return CalcGalleryBestSizeCore(sz, true);
		}
		protected override void CalcViewInfoCore(Rectangle bounds) {
			base.CalcViewInfoCore(bounds);
			this.backgroundImageRect = CalcBackgroundImageRect();
		}
		protected internal Rectangle BackgroundRect { get { return ((GalleryControlViewInfo)GalleryControl.ViewInfoCore).BackgroundRect; } }
		public override void Invalidate(Rectangle bounds) {
			base.Invalidate(bounds);
			if(GalleryControl.Client.Bounds.IntersectsWith(bounds)) {
				GalleryControl.Client.Invalidate(GalleryControl.Client.RectangleToClient(GalleryControl.RectangleToScreen(bounds)));
			}
		}
		protected virtual Rectangle CalcBackgroundImageRect() {
			Rectangle res = Rectangle.Empty;
			if(Gallery.BackgroundImage != null)
				res = ImageLayoutHelper.GetImageBounds(GalleryContentBounds, Gallery.BackgroundImage.Size, Gallery.BackgroundImageLayout);
			return res;
		}
		protected internal override Rectangle TranslateRect(Rectangle rect) {
			Rectangle res = rect;
			if(!res.IsEmpty) {
				res.Offset(-ControlClientBounds.X, -ControlClientBounds.Y);
			}
			return res;
		}
		protected internal override bool AutoFitColumns { get { return Gallery.AutoFitColumns; } }
		protected internal override HorzAlignment ItemHorizontalAlignmet {
			get {
				if(Gallery.ContentHorzAlignment == HorzAlignment.Default)
					return HorzAlignment.Near;
				return Gallery.ContentHorzAlignment;
			}
		}
	}
	public class StandaloneGalleryViewInfo : BaseGalleryViewInfo, ISupportXtraAnimation {
		Rectangle controlBounds;
		Rectangle filterAreaBounds, filterTextBounds, filterImageBounds;
		Size userDefinedSize;
		Size pointOffset;
		Point hitPoint;
		RibbonHitInfo hitInfo;
		GalleryItemViewInfo keyboardSelectedItem;
		int colCount;
		public StandaloneGalleryViewInfo(StandaloneGallery gallery)
			: base(gallery) {
			this.hitInfo = new RibbonHitInfo();
			this.userDefinedSize = Size.Empty;
			this.hitPoint = new Point(-10000, -10000);
			this.controlBounds = Rectangle.Empty;
			this.filterAreaBounds = Rectangle.Empty;
			this.filterTextBounds = Rectangle.Empty;
			this.filterImageBounds = Rectangle.Empty;
			this.pointOffset = Size.Empty;
			this.keyboardSelectedItem = null;
			this.colCount = Gallery.ColumnCount;
		}
		protected override bool IsAutoSizeGallery {
			get { return Gallery.GetAutoSize() == GallerySizeMode.Both; }
		}
		protected internal override bool IsItemHovered(GalleryItemViewInfo itemInfo) {
			if(HitInfo == null)
				return false;
			return (HitInfo.InGalleryItem && HitInfo.GalleryItem == itemInfo.Item) ||
				itemInfo.KeyboardSelected || Selection.IntersectsWith(itemInfo.Bounds);
		}
		protected override void UpdateControlVisibility(Size size) {
			if(Gallery.ShowScrollBar != ShowScrollBar.Auto)
				return;
			if(Gallery.GetAutoSize() == GallerySizeMode.Vertical && IsCalculateBestSize) {
				ScrollBarVisible = false;
				return;
			}
			ScrollBarVisible = GetDefineSize(BestSize) > GetDefineSize(size);
		}
		protected Rectangle ItemsAreaBounds {
			get {
				Rectangle rect = ContentBounds;
				if(ControlWidth > 0) {
					if(IsVertical)
						rect.Width -= ControlWidth + IndentBetweenContentAndControl;
					rect.Height -= ControlWidth + IndentBetweenContentAndControl;
				}
				return rect;
			}
		}
		protected override int GetItemMaxWidth() {
			int res = base.GetItemMaxWidth();
			if(Gallery.StretchItems)
				res = Math.Max(res, GetSecondarySize(ItemsAreaBounds));
			return res;
		}
		protected internal virtual GalleryItemViewInfo GetFirstNavigatableItem() {
			if(HitInfo.InGalleryItem)
				return hitInfo.GalleryItemInfo;
			GalleryItemViewInfo itemInfo = GetFirstCheckedItem();
			if(itemInfo != null)
				return itemInfo;
			return GetFirstItem();
		}
		protected virtual bool ShouldShowScrollBar { 
			get {
				if(Gallery.ShowScrollBar == ShowScrollBar.Hide) return false;
				else if(Gallery.ShowScrollBar == ShowScrollBar.Show) return true;
				return ScrollBarVisible;
			} 
		}
		protected override Point UpdateLocation(Point location) {
			Point res = base.UpdateLocation(location);
			OffsetDefineCoor(ref res, CalcFirstItemIndent());
			return res;
		}
		protected override void LayoutGroupsCore() {
			base.LayoutGroupsCore();
			UpdateVisibleSizeByItemAlignment();
		}
		protected virtual void UpdateVisibleSizeByItemAlignment() {
			int firstItemIndent = CalcFirstItemIndent();
			int lastItemIndent = CalcLastItemIndent();
			OffsetDefineSize(ref this.viBestSize, firstItemIndent + lastItemIndent);
		}
		protected virtual int CalcFirstItemIndent() {
			if(Gallery.FirstItemVertAlignment == GalleryItemAlignment.Custom)
				return Gallery.LastItemVertIndent;
			GalleryItemViewInfo item = GetFirstVisibleItem();
			if(item == null)
				return 0;
			int groupHeight = GetDefineSize(item.GroupInfo.CaptionBounds);
			if(Gallery.FirstItemVertAlignment == GalleryItemAlignment.Center)
				return (GetDefineSize(GalleryContentBounds) - GetDefineSize(ItemMaxSize)) / 2 - groupHeight;
			if(Gallery.FirstItemVertAlignment == GalleryItemAlignment.Far)
				return GetDefineSize(GalleryContentBounds) - GetDefineSize(ItemMaxSize) - groupHeight;
			return 0;
		}
		protected virtual int CalcLastItemIndent() {
			if(Gallery.LastItemVertAlignment == GalleryItemAlignment.Custom)
				return Gallery.LastItemVertIndent;
			if(Gallery.LastItemVertAlignment == GalleryItemAlignment.Center)
				return (GetDefineSize(GalleryContentBounds) - GetDefineSize(ItemMaxSize)) / 2;
			if(Gallery.LastItemVertAlignment == GalleryItemAlignment.Near)
				return GetDefineSize(GalleryContentBounds) - GetDefineSize(ItemMaxSize);
			return 0;
		}
		protected override Size GetDefaultSize() {
			return new Size(100, 100);
		}
		public override int ColCount { get { return colCount; } set { colCount = value; } }
		public int ControlWidth { get { return ShouldShowScrollBar ? SystemInformation.VerticalScrollBarWidth : 0; } }
		public override Locations GetItemImageLocation() {
			if(Gallery.ItemImageLocation != Locations.Default) return Gallery.ItemImageLocation;
			return Locations.Top;
		}
		public override void Invalidate(Rectangle bounds) {
			if(Gallery.OwnerControl != null) Gallery.OwnerControl.Invalidate(bounds);
		}
		public override Control OwnerControl { get { return Gallery.OwnerControl; } }
		protected internal override int ScrollYPosition { get { return Gallery.ScrollYPosition; } }
		public Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public Rectangle ControlBounds { get { return controlBounds; } }
		protected internal Rectangle FilterAreaBounds { get { return filterAreaBounds; } }
		protected internal Rectangle FilterTextBounds { get { return filterTextBounds; } }
		protected internal Rectangle FilterImageBounds { get { return filterImageBounds; } }
		public new StandaloneGallery Gallery { get { return base.Gallery as StandaloneGallery; } }
		public Size UserDefinedSize { get { return userDefinedSize; } set { userDefinedSize = value; } }
		protected virtual int GetBestHeight() {
			if(IsVertical) {
				if(Gallery.AllowFilter)
					return BestSize.Height + FilterAreaBounds.Height;
				return BestSize.Height;
			}
			return BestSize.Width;
		}
		bool scrollBarVisible;
		protected internal virtual bool ScrollBarVisible {
			get { return scrollBarVisible; }
			set {
				scrollBarVisible = value;
				Gallery.ScrollBar.Visible = value;
			}
		}
		protected virtual void UpdateScrollBarVisibility() {
			if(Gallery.ShowScrollBar != ShowScrollBar.Auto) return;
			if(ScrollBarVisible == false && GetBestHeight() > GetDefineSize(VisibleSize)) {
				ScrollBarVisible = true;
				LayoutGroups();
				base.UpdateGroupsLayout();
			}
			else if(ScrollBarVisible == true && GetBestHeight() <= GetDefineSize(VisibleSize)) {
				ScrollBarVisible = false;
				LayoutGroups();
				base.UpdateGroupsLayout();
			}
		}
		protected internal override void UpdateGroupsLayout() {
			base.UpdateGroupsLayout();
			UpdateScrollBarVisibility();
		}
		public override Rectangle GalleryContentBounds { 
			get {
				Rectangle res = ContentBounds;
				if(res.Size.IsEmpty) return res;
				res.Y += FilterAreaBounds.Height;
				res.Height -= FilterAreaBounds.Height;
				if(res.Height < 0) res.Height = 0;
				return res; 
			} 
		}
		public virtual Rectangle ControlClientBounds {
			get {
				Rectangle res = GalleryContentBounds;
				if(GetSecondarySize(ControlBounds) == 0) return res;
				if(IsVertical) {
					res.Width -= ControlBounds.Width;
					if(Gallery.IsRightToLeft) res.X += ControlBounds.Width;
				}
				else {
					res.Height -= ControlBounds.Height;
				}
				return res;
			}
		}
		protected internal void SetKeyboardSelectedItem(GalleryItemViewInfo itemInfo) {
			if(KeyboardSelectedItem == itemInfo) return;
			if(KeyboardSelectedItem != null && itemInfo != null && KeyboardSelectedItem.Item == itemInfo.Item) {
				this.keyboardSelectedItem = itemInfo;
				return;
			}
			if(KeyboardSelectedItem != null) Invalidate(Bounds);
			this.keyboardSelectedItem = itemInfo;
			if(KeyboardSelectedItem != null && (KeyboardSelectedItem.IsInvisible || KeyboardSelectedItem.IsPartiallyVisible)) MakeVisible(KeyboardSelectedItem.Item);
			if(KeyboardSelectedItem != null) {
				Invalidate(Bounds);
				if(ShouldProcessKeyboardSelectedItem())
					Gallery.SetItemCheck(KeyboardSelectedItem.Item, true);
#if DXWhidbey
				if(KeyboardSelectedGroup == null) return;
				int groupIndex = Gallery.Groups.IndexOf(KeyboardSelectedGroup.Group);
				Gallery.AccessibleNotifyClients(AccessibleEvents.Focus, GalleryDropDownBarControl.AccessibleGroupBeginId + groupIndex, KeyboardSelectedItemIndex);
#endif            
			}
		}
		protected virtual bool ShouldProcessKeyboardSelectedItem() {
			return Gallery.CheckSelectedItemViaKeyboard && (Gallery.ItemCheckMode == ItemCheckMode.SingleCheck || Gallery.ItemCheckMode == ItemCheckMode.SingleCheckInGroup || Gallery.ItemCheckMode == ItemCheckMode.SingleRadio || Gallery.ItemCheckMode == ItemCheckMode.SingleRadioInGroup);
		}
		public virtual GalleryItemViewInfo GetFirstItemInPosition(int index) {
			if (FirstMove) return GetFirstItem();
			if(index < 0) index = 0;
			if(RealColCount == 0)
				return null;
			int groupIndex = GetVisibleNotEmptyGroup(-1, +1, index % RealColCount);
			if(groupIndex < 0) return null;
			return Groups[groupIndex].Items[index % RealColCount];
		}
		public virtual GalleryItemViewInfo GetLastItemInPosition(int index) {
			if (FirstMove) return GetLastItem();
			if(RealColCount == 0)
				return null;
			int groupIndex = GetVisibleNotEmptyGroup(Groups.Count, -1, index % RealColCount);
			if(groupIndex < 0) return null;
			return Groups[groupIndex].Items[GetLastItemIndex(Groups[groupIndex], index, true)];
		}
		protected internal virtual GalleryItemViewInfo GetLastItem() {
			if (FirstMove && Gallery.SelectedItemInfo != null) {
				FirstMove = false;
				return Gallery.SelectedItemInfo;
			}
			FirstMove = false;
			return GetLastItemInPosition(0); 
		}
		internal bool FirstMove { get { return Gallery.FirstMove; } set { Gallery.FirstMove = value; } }
		protected internal virtual GalleryItemViewInfo GetFirstKeyboardItem() {
			if (HitInfo.InGalleryItem) return HitInfo.GalleryItemInfo;
			return GetFirstItem();
		}
		protected internal virtual GalleryItemViewInfo GetFirstItem() {
			if (FirstMove && Gallery.SelectedItemInfo != null) {
				FirstMove = false;
				return Gallery.SelectedItemInfo;
			}
			FirstMove = false;
			return GetFirstItemInPosition(0); 
		}
		public GalleryItemViewInfo KeyboardSelectedItem { 
			get { return keyboardSelectedItem; } 
		}
		public GalleryItemGroupViewInfo KeyboardSelectedGroup { 
			get { 
				if(KeyboardSelectedItem == null)return null;
				return KeyboardSelectedItem.GroupInfo;
			} 
		}
		int GetItemIndex(GalleryItem item) {
			int index;
			foreach (GalleryItemGroupViewInfo groupInfo in Groups)
			{
				index = 0;
				foreach (GalleryItemViewInfo itemInfo in groupInfo.Items)
				{
					if (itemInfo.Item == item) return index;
					index++;
				}
			}
			return -1;
		}
		int GetKeyboardSelectedItemIndex() { return GetItemIndex(KeyboardSelectedItem.Item); }
		int GetSelectedItemIndex() {
			if (Gallery.SelectedItemInfo == null) return -1;
			return GetItemIndex(Gallery.SelectedItemInfo.Item); 
		}
		public int KeyboardSelectedItemIndex { 
			get {
				if(KeyboardSelectedGroup == null) return -1;
				int selItemIndex = KeyboardSelectedGroup.Items.IndexOf(KeyboardSelectedItem);
				if(selItemIndex < 0) selItemIndex = GetKeyboardSelectedItemIndex();
				return selItemIndex;
			} 
		}
		public int KeyboardSelectedGroupIndex { get { return Groups.IndexOf(KeyboardSelectedGroup); } }
		int GetFirstItemIndex(int currPosition, bool vertical) {
			if(!vertical) return 0;
			return currPosition % RealColCount;
		}
		int GetLastItemIndex(GalleryItemGroupViewInfo groupInfo, int currPosition, bool vertical) {
			if(!vertical)
				return groupInfo.Items.Count - 1;
			currPosition %= RealColCount;
			int index = ((groupInfo.Items.Count / RealColCount)) * RealColCount + currPosition;
			if(index >= groupInfo.Items.Count) index = Math.Max(0, index - RealColCount);
			return index;
		}
		protected virtual bool ShouldCycleCurrentIndex(int currIndex, int direction) { 
			if(currIndex % RealColCount == 0 && direction < 0)return true;
			if(direction > 0) { 
				if(currIndex > 0 && (currIndex + 1) % RealColCount == 0) return true;
				if(currIndex == KeyboardSelectedGroup.Items.Count - 1) return true;
			}
			return false;
		}
		protected int CalcRowsCount() {
			int res = 0;
			for(int i = 0; i < Groups.Count; i++) {
				res += CalcRowsInGroup(i, RealColCount);
			}
			return res;
		}
		public virtual void MoveItem(GalleryItem item) {
			List<GalleryItem> items = Gallery.GetVisibleItems();
			if(items.Contains(item))
				SetKeyboardSelectedItem(GetItemInfo(item));
		}
		public virtual void MoveFirstVisibleItem() { 
			List<GalleryItem> items = Gallery.GetVisibleItems();
			if(items.Count > 0)
				SetKeyboardSelectedItem(GetItemInfo(items[0]));
		}
		public virtual void MoveLastVisibleItem() {
			List<GalleryItem> items = Gallery.GetVisibleItems();
			if(items.Count > 0)
				SetKeyboardSelectedItem(GetItemInfo(items[items.Count - 1]));
		}
		public virtual bool MovePageDown() {
			if(KeyboardSelectedItem == null) {
				SetKeyboardSelectedItem(GetFirstVisibleItem());
			}
			else {
				int endRowIndex = StartRowIndex + VisibleRowCount;
				int itemRowIndex = GetItemRow(KeyboardSelectedItem.Item);
				if(itemRowIndex < endRowIndex - 1)
					MoveVertical(endRowIndex - 1 - itemRowIndex);
				else
					MoveVertical(VisibleRowCount);
			}
			return true;
		}
		public virtual bool MovePageUp() {
			if(KeyboardSelectedItem == null) {
				SetKeyboardSelectedItem(GetFirstVisibleItem());
			}
			else {
				int endRowIndex = StartRowIndex + VisibleRowCount;
				int itemRowIndex = GetItemRow(KeyboardSelectedItem.Item);
				if(itemRowIndex > StartRowIndex)
					MoveVertical(StartRowIndex - itemRowIndex);
				else
					MoveVertical(-VisibleRowCount);
			}
			return true;
		}
		public GalleryItemViewInfo GetFirstItemInRow(GalleryItemViewInfo itemInRow) {
			int itemIndex = itemInRow.GroupInfo.Items.IndexOf(itemInRow);
			return itemInRow.GroupInfo.Items[itemIndex - itemInRow.Column];
		}
		public GalleryItemViewInfo GetLastItemInRow(GalleryItemViewInfo itemInRow) {
			int itemIndex = itemInRow.GroupInfo.Items.IndexOf(itemInRow);
			int lastItemIndex = Math.Min(itemIndex + RealColCount - itemInRow.Column - 1, itemInRow.GroupInfo.Items.Count - 1);
			return itemInRow.GroupInfo.Items[lastItemIndex];
		}
		protected void SelectFirstVisibleItem() {
			List<GalleryItem> items = Gallery.GetVisibleItems();
			if(items.Count > 0) {
				SetKeyboardSelectedItem(Gallery.ViewInfo.GetItemInfo(items[0]));
			}
		}
		public virtual bool MoveHorizontal(int delta) {
			if(KeyboardSelectedItemIndex < 0) {
				SetKeyboardSelectedItem(GetFirstNavigatableItem());
				if(KeyboardSelectedItem == null)
					return false;
			}
			if(RealColCount == 0) return false;
			int nextItemColumn = KeyboardSelectedItem.Column + delta;
			if(nextItemColumn < 0) {
				SetKeyboardSelectedItem(GetLastItemInRow(KeyboardSelectedItem));
				return true;
			}
			if(nextItemColumn > 0) nextItemColumn %= RealColCount;
			int nextItemIndex = KeyboardSelectedItemIndex + nextItemColumn - KeyboardSelectedItem.Column;
			if(nextItemIndex >= KeyboardSelectedItem.GroupInfo.Items.Count) { 
				SetKeyboardSelectedItem(GetFirstItemInRow(KeyboardSelectedItem));
				return true;
			}
			SetKeyboardSelectedItem(KeyboardSelectedItem.GroupInfo.Items[nextItemIndex]);
			return true;
		}
		protected GalleryItemViewInfo GetItemAt(GalleryItemViewInfo startItem, int row, int column) {
			int startGroupIndex = Groups.IndexOf(startItem.GroupInfo);
			int startItemIndex = Groups[startGroupIndex].Items.IndexOf(startItem);
			bool goBack = (startItem.Row > row) || (startItem.Row == row && startItem.Column > column);
			if(goBack) {
				for(int groupIndex = startGroupIndex; groupIndex >= 0; groupIndex--) {
					int si = groupIndex == startGroupIndex ? startItemIndex : Groups[groupIndex].Items.Count - 1;
					for(int itemIndex = si; itemIndex >= 0; itemIndex--) {
						GalleryItemViewInfo itemInfo = Groups[groupIndex].Items[itemIndex];
						if(itemInfo.Row == row && itemInfo.Column <= column)
							return itemInfo;
					}
				}
				return null;
			}
			else {
				GalleryItemViewInfo lastItemInRow = null;
				for(int groupIndex = startGroupIndex; groupIndex < Groups.Count; groupIndex++) {
					int si = groupIndex == startGroupIndex ? startItemIndex : 0;
					for(int itemIndex = si; itemIndex < Groups[groupIndex].Items.Count; itemIndex++) {
						GalleryItemViewInfo itemInfo = Groups[groupIndex].Items[itemIndex];
						if(itemInfo.Row == row) {
							lastItemInRow = itemInfo;
							if(lastItemInRow.Column == column)
								return itemInfo;
						}
						else if(itemInfo.Row > row) {
							return lastItemInRow;
						}
					}
				}
				return null;
			}
		}
		public virtual bool MoveVertical(int delta) {
			if(KeyboardSelectedItemIndex < 0) {
				SetKeyboardSelectedItem(GetFirstNavigatableItem());
				if(KeyboardSelectedItem == null)
					return false;
			}
			int nextRowIndex = KeyboardSelectedItem.Row + delta;
			if(nextRowIndex < 0 || nextRowIndex >= RowCount)
				return false;
			if(Groups.IndexOf(KeyboardSelectedItem.GroupInfo) < 0)
				return false;
			GalleryItemViewInfo nextItem = GetItemAt(KeyboardSelectedItem, nextRowIndex, KeyboardSelectedItem.Column);
			SetKeyboardSelectedItem(nextItem);
			return true;
		}
		int GetVisibleNotEmptyGroup(int currIndex, int direction, int minItemCount) {
			for(int i = currIndex + direction; i >= 0 && i < Groups.Count; i += direction) {
				if(Groups[i].Group.Visible && Groups[i].Items.Count >= minItemCount) return i;
			}
			return -1;
		}
		int GetVisibleNotEmptyGroup(int currIndex, int direction) {
			return GetVisibleNotEmptyGroup(currIndex, direction, 1);
		}
		protected internal virtual Size PointOffset { get { return pointOffset; } set { pointOffset = value; } }
		protected internal virtual void UpdateHitPoint(DXMouseEventArgs e) {
		}
		public virtual string GetFilterCaption() {
			if(Gallery.FilterCaption != string.Empty) return Gallery.FilterCaption;
			if(Gallery.Groups.GetGroupsVisibility()) return BarLocalizer.Active.GetLocalizedString(BarString.RibbonGalleryFilter);
			string res = null;
			foreach(GalleryItemGroup group in Gallery.Groups) {
				if(group.Visible) {
					if(res == null || res.Length == 0) res = group.Caption;
					else res += ", " + group.Caption;
				}
			}
			if(res != null) return res;
			return BarLocalizer.Active.GetLocalizedString(BarString.RibbonGalleryFilterNone);
		}
		protected virtual Size CalcFilterTextSize() {
			GInfo.AddGraphics(null);
			try {
				return PaintAppearance.FilterPanelCaption.CalcTextSize(GInfo.Graphics, GetFilterCaption(), 0).ToSize();
			}
			finally { GInfo.ReleaseGraphics(); }
		}
		protected virtual Size CalcFilterAreaSize() {
			return ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, Gallery.Painter.GetFilterPanelInfo(this), FilterTextBounds).Size;
		}
		protected virtual Size CalcFilterImageSize() {
			return new Size(10, 10);
		}
		protected virtual bool GetAllowFilter() {
			if(!Gallery.AllowFilter) return false;
			if(Gallery.Groups.Count > 1) return true;
			return false;
		}
		protected virtual void CalcFilterSize() {
			this.filterAreaBounds = this.filterTextBounds = Rectangle.Empty;
			if(GetAllowFilter()) {
				this.filterTextBounds.Size = CalcFilterTextSize();
				this.filterAreaBounds.Size = CalcFilterAreaSize();
				this.filterImageBounds.Size = CalcFilterImageSize();
			}
		}
		protected virtual int CalcMaxColCount() {
			int columnCount = Gallery.GetMaxColumnCount();
			return Math.Max(1, Math.Min(columnCount, Gallery.ColumnCount));
		}
		public virtual Size CalcGalleryBestSize(Size sz) {
			IsCalculatingBestSize = true;
			Size res = sz;
			if(Gallery.AutoSize == GallerySizeMode.Vertical) {
				if(IsVertical) {
					Gallery.ForceCalcViewInfo = BestSize.Width != sz.Width;
					CalcViewInfo(new Rectangle(0, 0, res.Width, 10000));
					Gallery.ForceCalcViewInfo = false;
					res.Height = BestSize.Height;
					res.Width = BestSize.Width;
				}
				else {
					Gallery.ForceCalcViewInfo = BestSize.Height != sz.Height;
					CalcViewInfo(new Rectangle(0, 0, 10000, res.Height));
					Gallery.ForceCalcViewInfo = false;
					res.Width = BestSize.Width;
					res.Height = BestSize.Height;
				}
			}
			res.Height += FilterAreaBounds.Height;
			IsCalculatingBestSize = false;
			return res;
		}
		public override Size CalcGalleryBestSize() {
			ColCount = Gallery.ColumnCount;
			if(Gallery.AutoSize == GallerySizeMode.Both) ColCount = CalcMaxColCount();
			Size res = base.CalcGalleryBestSize();
			return CalcGalleryBestSize(res);
		}
		protected override void CalcViewInfoCore(Rectangle bounds) {
			CalcFilterSize();
			int selGroupIndex = KeyboardSelectedGroupIndex;
			int selItemIndex = KeyboardSelectedItemIndex;
			base.CalcViewInfoCore(bounds);
			this.controlBounds = CalcControlBounds();
			UpdateFilterLayout();
			CalcScrollInfo();
			RestoreSelectedItems(selGroupIndex, selItemIndex);
		}
		internal virtual void RestoreSelectedItems(int selGroupIndex, int selItemIndex) {
			if(selGroupIndex < 0 || selGroupIndex >= Groups.Count || selItemIndex < 0 || selItemIndex >= Groups[selGroupIndex].Items.Count)
				SetKeyboardSelectedItem(null);
			else
				SetKeyboardSelectedItem(Groups[selGroupIndex].Items[selItemIndex]);
		}
		protected virtual Rectangle CalcControlBounds() {
			if(!IsVertical)
				return new Rectangle(GalleryContentBounds.X, ContentBounds.Bottom - ControlWidth, GalleryContentBounds.Width, ControlWidth);
			int x = Gallery.IsRightToLeft ? ContentBounds.X : ContentBounds.Right - ControlWidth;
			return new Rectangle(x, GalleryContentBounds.Y, ControlWidth, GalleryContentBounds.Height);
		}
		protected virtual void UpdateFilterLayout() {
			this.filterAreaBounds.Location = Bounds.Location;
			this.filterAreaBounds.Width = Bounds.Width;
			this.filterTextBounds.Location = SkinElementPainter.GetObjectClientRectangle(null, SkinElementPainter.Default, Gallery.Painter.GetFilterPanelInfo(this), FilterAreaBounds).Location;
			this.filterImageBounds.Location = new Point(FilterTextBounds.Right, FilterAreaBounds.Y + (FilterAreaBounds.Height - FilterImageBounds.Height) / 2);
			CheckFilterRTL();
		}
		protected virtual void CheckFilterRTL() {
			if(!Gallery.IsRightToLeft) return;
			this.filterTextBounds = BarUtilites.ConvertBoundsToRTL(this.filterTextBounds, this.filterAreaBounds);
			this.filterImageBounds = BarUtilites.ConvertBoundsToRTL(this.filterImageBounds, this.filterAreaBounds);
		}
		protected internal virtual void CalcFilterTextBounds() { }
		protected internal virtual void CalcFilterAreaBounds() { }
		protected internal virtual void CalcFilterImageBounds() { }
		public override int IndentBetweenContentAndControl { get { return ShouldShowScrollBar? 1: 0; } }
		public override Size ControlSize {
			get { 
				Size res = new Size(ControlWidth, VisibleSize.Height); 
				return new Size(GetSecondarySize(res), GetDefineSize(res));
			}
		}
		protected override Size ConstrainVisibleSize(Size size) {
			if(UserDefinedSize == Size.Empty)return size;
			return UserDefinedSize;
		}
		public RibbonHitInfo HitInfo { get { return hitInfo; } set { hitInfo = value; } }
		protected internal virtual RibbonHitInfo CalcHitInfo(DXMouseEventArgs e) { return CalcHitInfo(new Point(e.X, e.Y)); }
		protected internal virtual RibbonHitInfo CalcHitInfo(Point pt) {
			RibbonHitInfo hi = new RibbonHitInfo();
			hi.HitPoint = pt;
			CalcHitInfo(hi);
			return hi;
		}
		protected internal override void CalcHitInfo(RibbonHitInfo hitInfo) {
			if(!FilterAreaBounds.IsEmpty && hitInfo.ContainsSet(FilterAreaBounds, RibbonHitTest.GalleryFilter)) return;
			if(hitInfo.ContainsSet(Bounds, RibbonHitTest.Gallery)) {
				hitInfo.GalleryInfo = this;
				for(int i = 0; i < Groups.Count; i++) {
					if(Groups[i].CalcHitInfo(hitInfo)) break;
				}
			}
		}
		protected override bool ShouldFilterGroups { get { return Gallery.AllowFilter; } }
		protected virtual void CalcScrollInfo() { 
			if(Gallery.ScrollBar == null) return;
			ScrollArgs scroll = new ScrollArgs();
			scroll.Minimum = 0;
			scroll.Maximum = Math.Max(GetDefineSize(BestSize), 0);
			scroll.LargeChange = GetDefineSize(GalleryContentBounds);
			if(scroll.Maximum < scroll.LargeChange) scroll.LargeChange = scroll.Maximum;
			scroll.SmallChange = StandaloneGallery.ScrollBarSmallChanged;
			if(GetDefineSize(GalleryContentBounds) >= GetDefineSize(BestSize)) {
				scroll.Enabled = false; scroll.Maximum = 0;
				SetScrollYPosition(null, 0);
			}
			else if(ScrollYPosition > GetDefineSize(BestSize) - GetDefineSize(GalleryContentBounds)) SetScrollYPosition(null, GetDefineSize(BestSize) - GetDefineSize(GalleryContentBounds));
			scroll.Value = Gallery.ScrollYPosition;
			scroll.AssignTo(Gallery.ScrollBar);
			if(ControlBounds.Height < 10000) {
				Gallery.ScrollBar.Bounds = ControlBounds;
				Gallery.ScrollBar.LookAndFeel.ParentLookAndFeel = Gallery.LookAndFeelCore;
				if(GetDefineSize(ControlBounds) < GetDefineSize(Gallery.ScrollBar.Bounds)) { 
					ScrollBarVisible = false;
				}
				else if(Gallery.ShowScrollBar == ShowScrollBar.Show)
					ScrollBarVisible = true;
				else if(Gallery.ShowScrollBar == ShowScrollBar.Auto) {
					ScrollBarVisible = GetDefineSize(GalleryContentBounds) < GetDefineSize(BestSize);
				}
				else {
					ScrollBarVisible = false;
				}
			}
			if(OwnerControl != null && !OwnerControl.Controls.Contains(Gallery.ScrollBar)) {
				OwnerControl.Controls.Add(Gallery.ScrollBar);
			}
		}
		protected internal override bool AllowPartitalItems { get { return true; } }
		protected override void SetScrollYPosition(GalleryItemViewInfo itemInfo, int pos) {
			Gallery.ScrollYPosition = pos;
		}
		protected internal override ImageGalleryInfoArgs CreateGalleryInfo(BaseRibbonViewInfo viewInfo) { 
			PopupImageGalleryInfoArgs res = new PopupImageGalleryInfoArgs(this);
			if (KeyboardSelectedItem == null) {
				res.Hot = HitInfo.InGallery;
				res.ActiveItemInfo = HitInfo.InGalleryItem ? HitInfo.GalleryItemInfo : null;
			}
			return res;
		}
		protected internal int GetMaxRowCount() {
			int res = 0;
			foreach(GalleryItemGroupViewInfo group in Groups) {
				res += group.RowsCount;
			}
			return res;
		}
	}
}
