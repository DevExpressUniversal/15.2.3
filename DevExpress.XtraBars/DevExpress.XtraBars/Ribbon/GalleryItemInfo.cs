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
using System.Drawing.Imaging;
using System.ComponentModel;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Controls;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class GalleryItemAnimationInfo : CustomAnimationInfo {
		GalleryItemViewInfo itemInfo;
		public GalleryItemAnimationInfo(ISupportXtraAnimation obj, GalleryItemViewInfo item, CustomAnimationInvoker method)
			: base(obj, item.Item, (item as IAnimatedItem).AnimationIntervals, (item as IAnimatedItem).FramesCount, method) {
			this.itemInfo = item;
			this.AnimationType = DevExpress.Utils.Drawing.Animation.AnimationType.Cycle;
		}
		public GalleryItemViewInfo ItemInfo { get { return itemInfo; } }
		public GalleryItem Item { get { return AnimationId as GalleryItem; } }
	}
	public class GalleryItemViewInfo : IAnimatedItem, IAsyncImageItemViewInfo, ISupportContextItems { 
		GalleryItemGroupViewInfo groupInfo;
		GalleryItem item;
		Rectangle bounds, contentBounds;
		Rectangle imageBounds, textBounds, imageContentBounds, imageClientBounds;
		Rectangle captionBounds, descriptionBounds;
		Rectangle selectedImageBounds;
		Size imageBackgroundIndent;
		int row, column;
		public GalleryItemViewInfo(GalleryItemGroupViewInfo group, GalleryItem item) {
			this.groupInfo = group;
			this.item = item;
			this.imageClientBounds = this.imageContentBounds = this.bounds = this.contentBounds = Rectangle.Empty;
			this.imageBackgroundIndent = Size.Empty;
			this.imageBounds = this.textBounds = Rectangle.Empty;
			this.captionBounds = Rectangle.Empty;
			this.descriptionBounds = Rectangle.Empty;
			ForcedTextMaxWidth = -1;
		}
		AppearanceObject appearanceCaption;
		AppearanceObject appearanceDescription;
		AppearanceObject appearanceCaptionNormal;
		AppearanceObject appearanceDescriptionNormal;
		AppearanceObject appearanceCaptionHovered;
		AppearanceObject appearanceDescriptionHovered;
		AppearanceObject appearanceCaptionPressed;
		AppearanceObject appearanceDescriptionPressed;
		AppearanceObject appearanceCaptionDisabled;
		AppearanceObject appearanceDescriptionDisabled;
		protected internal virtual AppearanceObject AppearanceCaption {
			get {
				if(appearanceCaption == null)
					appearanceCaption = CreateCaptionAppearance();
				return appearanceCaption;
			}
		}
		protected internal virtual AppearanceObject AppearanceCaptionNormal {
			get {
				if(appearanceCaptionNormal == null)
					appearanceCaptionNormal = CreateCaptionAppearance(ObjectState.Normal);
				return appearanceCaptionNormal;
			}
		}
		protected internal virtual AppearanceObject AppearanceCaptionHovered {
			get {
				if(appearanceCaptionHovered == null)
					appearanceCaptionHovered = CreateCaptionAppearance(ObjectState.Hot);
				return appearanceCaptionHovered;
			}
		}
		protected internal virtual AppearanceObject AppearanceCaptionPressed {
			get {
				if(appearanceCaptionPressed == null)
					appearanceCaptionPressed = CreateCaptionAppearance(ObjectState.Pressed);
				return appearanceCaptionPressed;
			}
		}
		protected internal virtual AppearanceObject AppearanceCaptionDisabled {
			get {
				if(appearanceCaptionDisabled == null)
					appearanceCaptionDisabled = CreateCaptionAppearance(ObjectState.Disabled);
				return appearanceCaptionDisabled;
			}
		}
		protected virtual AppearanceObject CreateCaptionAppearance(ObjectState state) {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, Item.AppearanceCaption.GetAppearance(state), GalleryInfo.PaintAppearance.ItemCaptionAppearance.GetAppearance(state));
			return res;
		}
		protected virtual AppearanceObject CreateCaptionAppearance() {
			return CreateCaptionAppearance(GalleryInfo.CalcItemState(this));
		}
		protected internal virtual AppearanceObject AppearanceDescription {
			get {
				if(appearanceDescription == null)
					appearanceDescription = CreateDescriptionAppearance();
				return appearanceDescription;
			}
		}
		protected internal virtual AppearanceObject AppearanceDescriptionNormal {
			get {
				if(appearanceDescriptionNormal == null)
					appearanceDescriptionNormal = CreateDescriptionAppearance(ObjectState.Normal);
				return appearanceDescriptionNormal;
			}
		}
		protected internal virtual AppearanceObject AppearanceDescriptionHovered {
			get {
				if(appearanceDescriptionHovered == null)
					appearanceDescriptionHovered = CreateDescriptionAppearance(ObjectState.Hot);
				return appearanceDescriptionHovered;
			}
		}
		protected internal virtual AppearanceObject AppearanceDescriptionPressed {
			get {
				if(appearanceDescriptionPressed == null)
					appearanceDescriptionPressed = CreateDescriptionAppearance(ObjectState.Pressed);
				return appearanceDescriptionPressed;
			}
		}
		protected internal virtual AppearanceObject AppearanceDescriptionDisabled {
			get {
				if(appearanceDescriptionDisabled == null)
					appearanceDescriptionDisabled = CreateDescriptionAppearance(ObjectState.Disabled);
				return appearanceDescriptionDisabled;
			}
		}
		protected internal void ResetAppearances() {
			this.appearanceCaption = null;
			this.appearanceDescription = null;
		}
		protected virtual AppearanceObject CreateDescriptionAppearance(ObjectState state) {
			AppearanceObject res = new AppearanceObject();
			AppearanceHelper.Combine(res, Item.AppearanceDescription.GetAppearance(state), GalleryInfo.PaintAppearance.ItemDescriptionAppearance.GetAppearance(state));
			return res;			
		}
		protected virtual AppearanceObject CreateDescriptionAppearance() {
			return CreateDescriptionAppearance(GalleryInfo.CalcItemState(this));
		}
		object IAnimatedItem.Owner { get { return Item; } }
		bool IAnimatedItem.IsAnimated { get { return (Gallery == null || !Gallery.DesignModeCore) && ((IAnimatedItem)this).FramesCount > 1; } }
		int IAnimatedItem.AnimationInterval { get { return 0; } }
		int[] IAnimatedItem.AnimationIntervals { 
			get {
				Image img = GetImage();
				if(img == null) return null;
				int[] frameDelay = XtraAnimator.GetImageFrameDelay(GetImage());
				if(frameDelay == null) return null;
				for(int i = 0; i < frameDelay.Length; i++)
					frameDelay[i] *= 100000;
				return frameDelay;
			} 
		}
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		Rectangle IAnimatedItem.AnimationBounds { get { return ImageBounds; } }
		DevExpress.Utils.Drawing.Animation.AnimationType IAnimatedItem.AnimationType { get { return DevExpress.Utils.Drawing.Animation.AnimationType.Cycle; } }
		int IAnimatedItem.FramesCount { 
			get {
				Image img = GetImage();
				if(img == null) return 0;
				return XtraAnimator.GetImageFrameCount(GetImage()) + 1;
			} 
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			if(info.IsFinalFrame) return;
			if(!((IAnimatedItem)this).IsAnimated) return;
			Image img = GetImage();
			if(img == null) return;
			img.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
		}
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			int[] frameDelay = ((IAnimatedItem)this).AnimationIntervals;
			if(frameDelay == null || frameDelay.Length <= frameIndex) return 0;
			return frameDelay[frameIndex];
		}
		protected virtual Image GetImage() {
			if(Item == null) return null;
			if(Item.Image != null) return Item.Image;
			if(Item.Images != null) return ImageCollection.GetImageListImage(Item.Images, Item.ImageIndex);
			return null;
		}
		[EditorBrowsable(EditorBrowsableState.Never)] 
		public bool IsPartitallyVisible { get { return IsPartiallyVisible; } }
		public bool IsPartiallyVisible {
			get {
				if(GalleryInfo == null || GalleryInfo.GalleryContentBounds.IsEmpty) return false;
				return !Bounds.IsEmpty && (!GalleryInfo.GalleryContentBounds.Contains(Bounds) && GalleryInfo.GalleryContentBounds.IntersectsWith(Bounds));
			}
		}
		public bool IsEnabled {
			get {
				return Gallery.Enabled && Item.Enabled;
			}
		}
		public int Row { get { return row; } set { row = value; } }
		public int Column { get { return column; } set { column = value; } }
		public RibbonGalleryAppearances PaintAppearance { get { return GroupInfo.GalleryInfo.PaintAppearance; } }
		public GalleryItemGroupViewInfo GroupInfo { get { return groupInfo; } }
		public GalleryItem Item { get { return item; } }
		public Rectangle ImageContentBounds { get { return imageContentBounds; } set { imageContentBounds = value; } }
		public Rectangle ImageClientBounds { get { return imageClientBounds; } set { imageClientBounds = value; } }
		public Rectangle ImageBounds { get { return imageBounds; } }
		public Rectangle SelectedImageBounds { get { return selectedImageBounds; } }
		public Rectangle TextBounds { get { return textBounds; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle ContentBounds { get { return contentBounds; } }
		public BaseGallery Gallery { get { return GroupInfo.Group.Gallery; } }
		public BaseGalleryViewInfo GalleryInfo { get { return GroupInfo.GalleryInfo; } }
		public Rectangle CaptionBounds { get { return captionBounds; } }
		public StringInfo CaptionStringInfo { get; set; }
		public Rectangle DescriptionBounds { get { return descriptionBounds; } }
		public StringInfo DescriptionStringInfo { get; set; }
		public virtual bool IsImageExists { get { return ImageCollection.IsImageExists(Item.Image, Gallery.Images, Item.ImageIndex); } }
		public virtual bool IsLargeImageExists { get { return ImageCollection.IsImageExists(Item.HoverImage, Gallery.HoverImages, Item.HoverImageIndex); } }
		protected virtual Size GetImageSize() {
			if(!Gallery.ShowItemImage)
				return Size.Empty;
			if(Gallery.FixedImageSize) return GalleryInfo.DefaultItemImageSize;
			return GalleryInfo.GetImageSize(this);
		}
		protected virtual Size GetImageFullSize() {
			if(!Gallery.ShowItemImage)
				return Size.Empty;
			if(Gallery.FixedImageSize) return GalleryInfo.DefaultItemImageFullSize;
			return ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetImageBackgroundInfo(), new Rectangle(Point.Empty, GetImageSize())).Size;
		}
		public ImageLoadInfo ImageInfo {
			get {
				if(Item == null) return null;
				return Item.ImageInfo;
			}
			set {
				if(Item == null) return;
				Item.ImageInfo = value;
			}
		}
		protected SkinElementInfo GetImageBackgroundInfo() { return Gallery.Painter.ItemPainter.GetImageBackgroundInfo(Gallery, ImageBounds); }
		public virtual void CalcBestSize() {
			this.imageBounds = new Rectangle(Point.Empty, GetImageFullSize());
			this.imageClientBounds = new Rectangle(Point.Empty, GetImageSize()); 
			this.textBounds = Rectangle.Empty;
			if(Gallery.ShowItemText != true) return;
			this.textBounds = new Rectangle(Point.Empty, CalcTextSize());
			if(Gallery.MaxItemWidth == 0 && Gallery.ItemAutoSizeMode == GalleryItemAutoSizeMode.Default)
				return;
			int itemMaxWidth = GetItemMaxWidth();
			if(Gallery.ItemAutoSizeMode == GalleryItemAutoSizeMode.Default && itemMaxWidth <= Gallery.MaxItemWidth)
				return;
			try {
				ForcedTextMaxWidth = CalcForcedTextMaxWidth(itemMaxWidth);
				this.textBounds = new Rectangle(Point.Empty, CalcTextSize());
			}
			finally {
				ForcedTextMaxWidth = -1;
			}
		}
		protected Size ItemFixedSize {
			get {
				int itemFixedWidth = Gallery.ItemSize.Width == 0 ? DefaultItemSize.Width : Gallery.ItemSize.Width;
				int itemFixedHeight = Gallery.ItemSize.Height == 0 ? DefaultItemSize.Height : Gallery.ItemSize.Height;
				return new Size(itemFixedWidth, itemFixedHeight);
			}
		}
		protected int CalcForcedTextMaxWidth(int itemMaxWidth) { 
			if(Gallery.ItemAutoSizeMode == GalleryItemAutoSizeMode.Default)
				return Math.Max(0, this.textBounds.Width - (itemMaxWidth - Gallery.MaxItemWidth));
			return Math.Max(0, this.textBounds.Width - (itemMaxWidth - ItemFixedSize.Width));
		}
		protected GraphicsInfo GInfo { get { return GroupInfo.GalleryInfo.GInfo; } }
		protected int ForcedTextMaxWidth { get; set; }
		protected int GetTextMaxWidth() {
			if(ForcedTextMaxWidth >= 0)
				return ForcedTextMaxWidth;
			if(GalleryInfo.PaintAppearance.ItemCaptionAppearance.Normal.TextOptions.WordWrap != WordWrap.Wrap && GalleryInfo.PaintAppearance.ItemDescriptionAppearance.Normal.TextOptions.WordWrap != WordWrap.Wrap) return 0;
			int controlSize = GalleryInfo.GetSecondarySize(GalleryInfo.ControlSize);
			if(controlSize != 0) controlSize += GalleryInfo.IndentBetweenContentAndControl;
			int width = GalleryInfo.GetSecondarySize(GalleryInfo.ContentBounds);
			if(GetItemImageLocation() == Locations.Left || GetItemImageLocation() == Locations.Right) {
				int iwidth = ImageBounds.Width + GalleryInfo.ButtonPaddingWidth;
				if(GetItemImageLocation() == Locations.Left)
					iwidth += Gallery.ItemImagePadding.Left + DistanceItemImageToText + Gallery.ItemTextPadding.Right;
				else
					iwidth += Gallery.ItemTextPadding.Left + DistanceItemImageToText + Gallery.ItemImagePadding.Right;
				width -= iwidth;
			}
			else {
				width -= TextPaddingsWidth;
				if(ShouldStretchItems && IsScrollbarVisible) {
					width -= GetScrollbarWidth();
				}
			}
			return width;
		}
		Size CalcTextSizeCore(GraphicsInfo info, string text, AppearanceObject obj) {
			return obj.CalcTextSize(info.Graphics, text, GetTextMaxWidth()).ToSize();
		}
		protected virtual Size CalcCaptionSize(GraphicsInfo info) {
			if(Item.Gallery.AllowHtmlText) {
				CalcCaptionStringInfo(info.Graphics);
				return CaptionStringInfo.Bounds.Size;
			}
			Size res = Size.Empty;
			res = GetMaxSize(res, CalcTextSizeCore(info, Item.Caption, AppearanceCaptionNormal));
			if(!IsFontEquals(AppearanceCaptionNormal, AppearanceCaptionHovered))
				res = GetMaxSize(res, CalcTextSizeCore(info, Item.Caption, AppearanceCaptionHovered));
			if(!IsFontEquals(AppearanceCaptionNormal, AppearanceCaptionPressed))
				res = GetMaxSize(res, CalcTextSizeCore(info, Item.Caption, AppearanceCaptionPressed));
			if(!IsFontEquals(AppearanceCaptionNormal, AppearanceCaptionDisabled))
				res = GetMaxSize(res, CalcTextSizeCore(info, Item.Caption, AppearanceCaptionDisabled));
			return res;
		}
		private bool IsFontEquals(AppearanceObject app1, AppearanceObject app2) {
			return app1.Font.Height != app2.Font.Height || app1.Font.Style != app2.Font.Style || app1.Font.FontFamily != app2.Font.FontFamily;
		}
		private Size GetMaxSize(Size size1, Size size2) {
			return new Size(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height));
		}
		protected internal void CalcCaptionStringInfo(Graphics g) {
			AppearanceObject obj = AppearanceCaption;
			StringInfo stringInfo = StringPainter.Default.Calculate(g, obj, obj.GetTextOptions(), Item.Caption, CaptionBounds);
			CaptionStringInfo = stringInfo;
		}
		protected virtual Size CalcDescriptionSize(GraphicsInfo info) {
			AppearanceObject obj = GalleryInfo.PaintAppearance.ItemDescriptionAppearance.Normal;
			if(Item.Gallery.AllowHtmlText) {
				CalcDescriptionStringInfo(info.Graphics);
				return DescriptionStringInfo.Bounds.Size;
			}
			Size res = Size.Empty;
			res = GetMaxSize(res, CalcTextSizeCore(info, Item.Description, AppearanceDescriptionNormal));
			if(!IsFontEquals(AppearanceCaptionNormal, AppearanceCaptionHovered))
				res = GetMaxSize(res, CalcTextSizeCore(info, Item.Description, AppearanceDescriptionHovered));
			if(!IsFontEquals(AppearanceCaptionNormal, AppearanceCaptionPressed))
				res = GetMaxSize(res, CalcTextSizeCore(info, Item.Description, AppearanceDescriptionPressed));
			if(!IsFontEquals(AppearanceCaptionNormal, AppearanceCaptionDisabled))
				res = GetMaxSize(res, CalcTextSizeCore(info, Item.Description, AppearanceDescriptionDisabled));
			return res;
		}
		protected internal void CalcDescriptionStringInfo(Graphics g) {
			AppearanceObject obj = AppearanceDescription;
			StringInfo stringInfo = StringPainter.Default.Calculate(g, obj, obj.GetTextOptions(), Item.Description, DescriptionBounds);
			DescriptionStringInfo = stringInfo;
		}
		protected virtual Size CalcTextSize() {
			int indent = 0;
			GInfo.AddGraphics(null);
			try {
				this.captionBounds = this.descriptionBounds = Rectangle.Empty;
				if(!Gallery.ShowItemText) return Size.Empty;
				if(Item.Caption != string.Empty) this.captionBounds = new Rectangle(Point.Empty, CalcCaptionSize(GInfo));
				if(Item.Description != string.Empty) this.descriptionBounds = new Rectangle(Point.Empty, CalcDescriptionSize(GInfo));
				if(Item.Caption != string.Empty && Item.Description != string.Empty) indent = Gallery.DistanceItemCaptionToDescription;
				return new Size(Math.Max(this.captionBounds.Width, this.descriptionBounds.Width), this.captionBounds.Height + indent + this.descriptionBounds.Height);
			}
			finally { GInfo.ReleaseGraphics(); }
		}
		protected virtual int TextPaddingsHeight { get { return Gallery.ItemTextPadding.Height + GalleryInfo.ButtonPaddingHeight; } }
		protected virtual int TextHeightWithPaddings { get { return TextBounds.Height + TextPaddingsHeight; } }
		protected virtual int TextPaddingsWidth { get { return Gallery.ItemTextPadding.Width + GalleryInfo.ButtonPaddingWidth; } }
		protected virtual int TextWidthWithPaddings { get { return TextBounds.Width + TextPaddingsWidth; } }
		protected virtual int ImageHeightWithPaddings {
			get {
				if(!Gallery.ShowItemImage)
					return 0;
				int res = ImageBounds.Height + Gallery.ItemImagePadding.Height + GalleryInfo.ButtonPaddingHeight;
				return res;
			}
		}
		protected virtual int ImageWidthWithPaddings {
			get {
				if(!Gallery.ShowItemImage)
					return 0;
				int res = ImageBounds.Width + Gallery.ItemImagePadding.Width + GalleryInfo.ButtonPaddingWidth;
				return res;
			}
		}
		protected Locations GetItemImageLocation() { return GalleryInfo.GetItemImageLocation(); }
		public virtual Size GetItemSize() {
			return new Size(GetItemWidth(), GetItemHeight());
		}
		protected virtual Size DefaultItemSize { get { return Gallery.ShowItemText ? Gallery.DefaultItemSizeWithText : Gallery.DefaultItemSizeWithoutText; } }
		public virtual int GetItemMaxWidth() {
			return GetItemWidth(GalleryItemAutoSizeMode.Default);
		}
		public virtual int GetItemWidth() {
			return GetItemWidth(Gallery.ItemAutoSizeMode);
		}
		public virtual int GetItemWidth(GalleryItemAutoSizeMode sizeMode) {
			if(sizeMode != GalleryItemAutoSizeMode.Default)
				return ItemFixedSize.Width;
			if(GetItemImageLocation() == Locations.Left || GetItemImageLocation() == Locations.Right) {
				if(Gallery.ShowItemText == false) return ImageWidthWithPaddings;
				int width = ImageBounds.Width + TextBounds.Width;
				if(GetItemImageLocation() == Locations.Left)
					width += Gallery.ItemImagePadding.Left + DistanceItemImageToText + Gallery.ItemTextPadding.Right;
				else
					width += Gallery.ItemTextPadding.Left + DistanceItemImageToText + Gallery.ItemImagePadding.Right;
				return width + GalleryInfo.ButtonPaddingWidth;
			}
			return Math.Max(ImageWidthWithPaddings, TextWidthWithPaddings);
		}
		public virtual int GetItemHeight() {
			if(Gallery.ItemAutoSizeMode == GalleryItemAutoSizeMode.None)
				return ItemFixedSize.Height;
			if(GetItemImageLocation() == Locations.Left || GetItemImageLocation() == Locations.Right)
				return Math.Max(ImageHeightWithPaddings, TextHeightWithPaddings);
			if(Gallery.ShowItemText == false)
				return ImageHeightWithPaddings;
			int height = TextHeightWithPaddings + DistanceItemImageToText + ImageHeightWithPaddings;
			if(Gallery.ShowItemImage) 
				height -= GalleryInfo.ButtonPaddingHeight;
			if(GetItemImageLocation() == Locations.Bottom)
				height += Gallery.ItemTextPadding.Top + Gallery.ItemImagePadding.Bottom;
			else
				height += Gallery.ItemImagePadding.Top + Gallery.ItemTextPadding.Bottom;
			return height;
		}
		protected virtual Rectangle CalcContentBoundsByBounds(Rectangle rect) {
			SkinElementInfo info = Gallery.ItemPainter.GetButtonInfo(Gallery, rect, ObjectState.Hot);
			return SkinElementPainter.GetObjectClientRectangle(null, SkinElementPainter.Default, info);
		}
		protected internal virtual void UpdateLayout(Point offset) { 
			this.bounds.Offset(offset);
			this.contentBounds.Offset(offset);
			this.imageBounds.Offset(offset);
			this.textBounds.Offset(offset);
			this.imageContentBounds.Offset(offset);
			this.imageClientBounds.Offset(offset);
			this.captionBounds.Offset(offset);
			this.descriptionBounds.Offset(offset);
			this.selectedImageBounds.Offset(offset);
		}
		ContextItemCollectionViewInfo contextButtonsViewInfo;
		protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo {
			get {
				if(contextButtonsViewInfo == null)
					contextButtonsViewInfo = new ContextItemCollectionViewInfo(((ISupportContextItems)this).ContextItems, ((ISupportContextItems)this).Options, this);
				return contextButtonsViewInfo;
			}
		}
		ContextItemCollectionHandler contextButtonsHandler;
		protected internal ContextItemCollectionHandler ContextButtonsHandler {
			get {
				if(contextButtonsHandler == null)
					contextButtonsHandler = CreateContextButtonsHandler();
				return contextButtonsHandler;
			}
		}
		protected virtual ContextItemCollectionHandler CreateContextButtonsHandler() {
			return new ContextItemCollectionHandler(ContextButtonsViewInfo);
		}
		protected virtual void LayoutBounds(Size itemMaxSize, Point topLeft) {
			this.bounds = new Rectangle(topLeft, itemMaxSize);
			Size itemSize = GetItemSize();
			Point location = topLeft;
			location.Offset((itemMaxSize.Width - itemSize.Width) / 2, (itemMaxSize.Height - itemSize.Height) / 2);
			if(Gallery.ShowItemText && (GetItemImageLocation() == Locations.Left || GetItemImageLocation() == Locations.Right)) {
				this.contentBounds = CalcContentBoundsByBounds(new Rectangle(topLeft, itemMaxSize));
				this.textBounds.Width = ContentBounds.Width - ImageBounds.Width;
				if(GetItemImageLocation() == Locations.Left)
					this.textBounds.Width -= Gallery.ItemImagePadding.Left + DistanceItemImageToText + Gallery.ItemTextPadding.Right;
				else
					this.textBounds.Width -= Gallery.ItemImagePadding.Right + DistanceItemImageToText + Gallery.ItemTextPadding.Left;
			}
			else {
				this.contentBounds = CalcContentBoundsByBounds(new Rectangle(location, itemMaxSize));
				if(Gallery.ShowItemText)
					this.textBounds.Width = ContentBounds.Width - Gallery.ItemTextPadding.Width;
			}		   
		}
		Rectangle CalculatedContextButtonDisplayBounds { get; set; }
		protected internal virtual void CheckContextButtonsLayout() {
			if(CalculatedContextButtonDisplayBounds != ((ISupportContextItems)this).DisplayBounds)
				InvalidateContextButtonsViewInfo();
		}
		protected internal virtual void InvalidateContextButtonsViewInfo() {
			ContextButtonsViewInfo.InvalidateViewInfo();
			CalculatedContextButtonDisplayBounds = ((ISupportContextItems)this).DisplayBounds;
		}
		protected virtual void CalcContextButtonsViewInfo() {
			if(StandaloneGallery != null) {
				CheckContextButtonsLayout();
				ContextButtonsViewInfo.CalcItems();
			}
		}
		protected bool IsScrollbarVisible {
			get {
				StandaloneGallery sg = Gallery as StandaloneGallery;
				return sg != null ? sg.ScrollBarVisible : false;
			}
		}
		protected int GetScrollbarWidth() {
			StandaloneGallery sg = Gallery as StandaloneGallery;
			if(sg == null || sg.ScrollBar == null)
				return 0;
			return sg.ScrollBar.GetWidth();
		}
		protected virtual void LayoutCaption() {
			if(Item.Caption == string.Empty) return;
			this.captionBounds.Width = TextBounds.Width;
			this.captionBounds.Location = TextBounds.Location;
			if(Gallery.AllowHtmlText) {
				CalcCaptionStringInfo(GInfo.Graphics);
			}
		}
		protected virtual void LayoutDescription() {
			if(Item.Description == string.Empty) return;
			this.descriptionBounds = TextBounds;
			if(Item.Caption != string.Empty) {
				this.descriptionBounds.Y += CaptionBounds.Height + Gallery.DistanceItemCaptionToDescription;
				this.descriptionBounds.Height -= CaptionBounds.Height + Gallery.DistanceItemCaptionToDescription;
				if(Gallery.AllowHtmlText) {
					CalcDescriptionStringInfo(GInfo.Graphics);
				}
			}
		}
		int ImageMiddleXCoord { get { return ContentBounds.X + (ContentBounds.Width - ImageBounds.Width) / 2; } }
		Point TopImageCoords { get { return new Point(ImageMiddleXCoord, ImageTopYCoord); } }
		Point BottomImageCoords { get { return new Point(ImageMiddleXCoord, TextBounds.Bottom + DistanceItemImageToText); } }
		int TextLeftXCoord { get { return ContentBounds.X + Gallery.ItemTextPadding.Left; } }
		int TextMiddleXCoord { get { return TextLeftXCoord + (ContentBounds.Width - Gallery.ItemTextPadding.Width) / 2; } }
		int TextRightXCoord { get { return ContentBounds.Right - Gallery.ItemTextPadding.Right - TextBounds.Width; } }
		int TextXCoord { 
			get {
				if(GalleryInfo.PaintAppearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment == HorzAlignment.Center)
					return TextMiddleXCoord;
				else if(GalleryInfo.PaintAppearance.ItemCaptionAppearance.Normal.TextOptions.HAlignment == HorzAlignment.Far)
					return TextRightXCoord;
				return TextLeftXCoord;
			} 
		}
		Point BottomTextCoords { get { return new Point(TextLeftXCoord, ImageBounds.Bottom + DistanceItemImageToText); } }
		Point TopTextCoords { get { return new Point(TextLeftXCoord, ContentBounds.Y + Gallery.ItemTextPadding.Top); } }
		int ImageMiddleYCoord { get { return ContentBounds.Y + (ContentBounds.Height - ImageBounds.Height) / 2; } }
		int ImageTopYCoord { get { return ContentBounds.Y + Gallery.ItemImagePadding.Top; } }
		Point ImageLeftCoords { get { return new Point(ContentBounds.X + Gallery.ItemImagePadding.Left, ImageTopYCoord); } }
		Point ImageRightCoords { get { return new Point(ContentBounds.Right - Gallery.ItemImagePadding.Right - ImageBounds.Width, ImageTopYCoord); } }
		int TextTopYCoord { get { return ContentBounds.Y + Gallery.ItemTextPadding.Top; } }
		int TextMiddleYCoord { get { return TextTopYCoord + (ContentBounds.Height - Gallery.ItemTextPadding.Height - textBounds.Height) / 2; } }
		int TextBottomYCoord { get { return ContentBounds.Bottom - Gallery.ItemTextPadding.Bottom - textBounds.Height; } }
		int TextYCoord {
			get {
				if(GalleryInfo.PaintAppearance.ItemCaptionAppearance.Normal.TextOptions.VAlignment == VertAlignment.Center)
					return TextMiddleYCoord;
				else if(GalleryInfo.PaintAppearance.ItemCaptionAppearance.Normal.TextOptions.VAlignment == VertAlignment.Bottom)
					return TextBottomYCoord;
				return TextTopYCoord;
			}
		}
		Point TextRightCoords { 
			get { return new Point(ImageBounds.Right + DistanceItemImageToText, TextYCoord); } 
		}
		Point TextLeftCoords { 
			get { 
				return new Point(ContentBounds.X + Gallery.ItemTextPadding.Left, TextYCoord); 
			} 
		}
		Rectangle TopImageBounds { get { return new Rectangle(ContentBounds.X + Gallery.ItemImagePadding.Left, ContentBounds.Y + Gallery.ItemImagePadding.Top, ContentBounds.Width - Gallery.ItemImagePadding.Width, ContentBounds.Height - Gallery.ItemTextPadding.Bottom - TextBounds.Height - DistanceItemImageToText); } }
		Rectangle BottomImageBounds { get { return new Rectangle(ContentBounds.X + Gallery.ItemImagePadding.Left, ContentBounds.Y + Gallery.ItemTextPadding.Top + TextBounds.Height + DistanceItemImageToText, ContentBounds.Width - Gallery.ItemImagePadding.Width, ContentBounds.Height - Gallery.ItemTextPadding.Bottom - TextBounds.Height - DistanceItemImageToText); } }
		Rectangle LeftImageBounds { get { return new Rectangle(ContentBounds.X + Gallery.ItemImagePadding.Left, ContentBounds.Y + Gallery.ItemImagePadding.Top, ContentBounds.Width - TextBounds.Width - Gallery.ItemTextPadding.Right - DistanceItemImageToText, ContentBounds.Height - Gallery.ItemImagePadding.Height); } }
		Rectangle RightImageBounds { get { return new Rectangle(ContentBounds.X + TextBounds.Width + Gallery.ItemTextPadding.Left + DistanceItemImageToText, ContentBounds.Y + Gallery.ItemImagePadding.Top, ContentBounds.Width - TextBounds.Width - Gallery.ItemTextPadding.Right - DistanceItemImageToText, ContentBounds.Height - Gallery.ItemImagePadding.Height); } }
		protected virtual int DistanceItemImageToText {
			get {
				if(Gallery == null || !Gallery.ShowItemText || !Gallery.ShowItemImage) return 0;
				return Math.Max(Gallery.DistanceItemImageToText, 3);
			}
		}
		protected internal virtual void LayoutImageAndText() {
			LayoutImageAndTextCore();
			ImageClientBounds = ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, GetImageBackgroundInfo());
			UpdateImageContentBounds();
		}
		protected internal void UpdateImageContentBounds() {
			Size imageSize = (ImageInfo != null && ImageInfo.ThumbImage != null) ? ImageInfo.AnimatedRegion : GalleryInfo.GetImageSize(this, false);
			ImageContentBounds = ImageLayoutHelper.GetImageBounds(ImageClientBounds, imageSize, GetImageLayoutMode());
			CalcContextButtonsViewInfo();
		}
		ImageLayoutMode GetImageLayoutMode() {
			if(Gallery != null && Gallery.OptionsImageLoad.AsyncLoad && Item != null && !Item.ImageInfo.IsLoaded)
				return ImageLayoutMode.Squeeze;
			return Gallery.ItemImageLayout;
		}
		protected virtual void LayoutImageAndTextCore() {
			if(!Gallery.ShowItemText && Gallery.FixedImageSize) {
				this.imageBounds = new Rectangle(new Point(ContentBounds.X + Gallery.ItemImagePadding.Left, ContentBounds.Y + Gallery.ItemImagePadding.Top), ImageBounds.Size);
				this.selectedImageBounds = this.Bounds;
				return;
			}
			this.selectedImageBounds = Bounds;
			switch(GetItemImageLocation()) {
				case Locations.Default:
				case Locations.Top:
					this.imageBounds = ImageLayoutHelper.GetImageBounds(TopImageBounds, ImageBounds.Size, Gallery.ItemImageLayout); 
					this.textBounds = new Rectangle(BottomTextCoords, TextBounds.Size);
					this.selectedImageBounds.Height = (ImageBounds.Y - Bounds.Y) * 2 + ImageBounds.Height; 
					break;
				case Locations.Bottom:
					this.textBounds = new Rectangle(TopTextCoords, TextBounds.Size);
					this.imageBounds = ImageLayoutHelper.GetImageBounds(BottomImageBounds, ImageBounds.Size, Gallery.ItemImageLayout);
					this.selectedImageBounds.Height = (Bounds.Bottom - ImageBounds.Bottom) * 2 + ImageBounds.Height;
					break;
				case Locations.Left:
					this.imageBounds = ImageLayoutHelper.GetImageBounds(LeftImageBounds, ImageBounds.Size, Gallery.ItemImageLayout); 
					this.textBounds = new Rectangle(TextRightCoords, TextBounds.Size);
					this.selectedImageBounds.Width = (ImageBounds.X - Bounds.X) * 2 + ImageBounds.Width;
					break;
				case Locations.Right:
					this.textBounds = new Rectangle(TextLeftCoords, TextBounds.Size);
					this.imageBounds = ImageLayoutHelper.GetImageBounds(RightImageBounds, ImageBounds.Size, Gallery.ItemImageLayout);  
					this.selectedImageBounds.Width = (Bounds.Right - ImageBounds.Right) * 2 + ImageBounds.Width;
					break;
			}
			this.selectedImageBounds.Inflate(-2, -2);
		}
		protected internal virtual void LayoutContent() {
			LayoutContent(Bounds.Size, Bounds.Location);
		}
		protected internal virtual void LayoutContent(Size itemMaxSize, Point topLeft) {
			bool graphicsCreated = false;
			if(GInfo.Graphics == null) {
				GInfo.AddGraphics(null);
				graphicsCreated = true;
			}
			try {
				imageBounds.Size = Gallery.UseMaxImageSize ? GalleryInfo.ItemMaxImageSize : ImageBounds.Size;
				LayoutBounds(itemMaxSize, topLeft);
				LayoutImageAndText();
				LayoutCaption();
				LayoutDescription();
				CalcContextButtonsViewInfo();
			}
			finally {
				if(graphicsCreated) GInfo.ReleaseGraphics();
			}
		}
		public virtual bool IsInvisible {
			get {
				MacStyleInRibbonGalleryViewInfo macGallery = GalleryInfo as MacStyleInRibbonGalleryViewInfo;
				if(macGallery != null)
					return Bounds.Left >= GalleryContent.Right || Bounds.Right <= GalleryContent.Left;
				return Bounds.Top >= GalleryContent.Bottom ||
						Bounds.Bottom <= GalleryContent.Top;
			}
		}
		Rectangle GalleryContent { get { return GroupInfo.GalleryInfo.GalleryContentBounds; } }
		public virtual bool KeyboardSelected {
			get {
				StandaloneGalleryViewInfo gal = GalleryInfo as StandaloneGalleryViewInfo;
				if(gal == null || gal.KeyboardSelectedItem == null) return false;
				return gal.KeyboardSelectedItem.Item == this.Item;
			}
		}
		protected virtual bool ShouldStretchItems {
			get {
				StandaloneGallery sg = Gallery as StandaloneGallery;
				return sg != null ? sg.StretchItems : false;
			}
		}
		protected internal virtual Color ItemCaptionSelectedForeColor {
			get { return RibbonSkins.GetSkin(Gallery.Provider)[RibbonSkins.SkinPopupGalleryItemCaption].GetForeColor(ObjectState.Selected); }
		}
		protected internal virtual Color ItemDescriptionSelectedForeColor {
			get { return RibbonSkins.GetSkin(Gallery.Provider)[RibbonSkins.SkinPopupGalleryItemSubCaption].GetForeColor(ObjectState.Selected); }
		}
		protected internal virtual bool ShouldDrawSelectedItemCaption(GalleryItemInfoArgs e) {
			if((e.State & ObjectState.Selected) == 0 || Gallery.CheckDrawMode == CheckDrawMode.OnlyImage) return false;
			return ItemCaptionSelectedForeColor != Color.Empty;
		}
		protected internal virtual bool ShouldDrawSelectedItemDescription(GalleryItemInfoArgs e) {
			if((e.State & ObjectState.Selected) == 0 || Gallery.CheckDrawMode == CheckDrawMode.OnlyImage) return false;
			return ItemDescriptionSelectedForeColor != Color.Empty;
		}
		Rectangle ISupportContextItems.ActivationBounds {
			get { return Bounds; }
		}
		bool ISupportContextItems.CloneItems {
			get { return true; }
		}
		ContextItemCollection ISupportContextItems.ContextItems {
			get {
				return StandaloneGallery != null ? StandaloneGallery.ContextButtons : null; 
			}
		}
		Control ISupportContextItems.Control {
			get { return GalleryInfo.OwnerControl; }
		}
		bool ISupportContextItems.DesignMode { get { return Gallery.DesignModeCore; } }
		Rectangle ISupportContextItems.DisplayBounds {
			get { return new Rectangle(ImageClientBounds.X, ImageContentBounds.Y, ImageClientBounds.Width, ImageContentBounds.Height); }
		}
		Rectangle ISupportContextItems.DrawBounds {
			get { return ((ISupportContextItems)this).DisplayBounds; }
		}
		bool ISupportContextItems.ShowOutsideDisplayBounds { get { return false; } }
		ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) {
			return ItemLocation.Left;
		}
		int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) {
			return 3;
		}
		ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		LookAndFeel.UserLookAndFeel ISupportContextItems.LookAndFeel {
			get { return Gallery.GetController().LookAndFeel.ActiveLookAndFeel; }
		}
		protected StandaloneGallery StandaloneGallery { get { return Gallery as StandaloneGallery; } }
		ContextItemCollectionOptions ISupportContextItems.Options {
			get { return StandaloneGallery != null ? StandaloneGallery.ContextButtonOptions : null; }
		}
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
			if(StandaloneGallery != null) {
				e.DataItem = Item;
				StandaloneGallery.RaiseContextButtonClick(e);
			}
		}
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
			if(StandaloneGallery != null) {
				StandaloneGallery.RaiseCustomContextButtonToolTip(new GalleryContextButtonToolTipEventArgs(Item, e));
			}
		}
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) {
			if(StandaloneGallery != null)
				StandaloneGallery.RaiseContextButtonCustomize(new GalleryContextButtonCustomizeEventArgs(item, Item));
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			if(Gallery != null && Gallery.OwnerControl != null) {
				Gallery.OwnerControl.Invalidate(rect);
			}
		}
		void ISupportContextItems.Update() {
		}
		void ISupportContextItems.Redraw() {
			if(Gallery != null && Gallery.OwnerControl != null) {
				Gallery.OwnerControl.Invalidate();
			}
		}
		protected internal bool ShouldDrawContextButtons { get { return StandaloneGallery != null; } }
		public virtual ObjectState CalcState() {
			ObjectState res = ObjectState.Normal;
			if(!Item.Enabled) {
				res = ObjectState.Disabled;
				return res;
			}
			if(ForcedState.HasValue)
				return ForcedState.Value;
			if(Item.Checked) res = ObjectState.Selected;
			else if(GalleryInfo.IsItemHovered(this)) res = ObjectState.Hot;
			else if(Item == item.Gallery.DownItem) res = ObjectState.Pressed;
			return res;
		}
		ObjectState? ForcedState { get; set; }
		internal void UpdateContent(ObjectState state) {
			ForcedState = state;
			UpdateContent();
			ForcedState = null;
		}
		protected internal virtual void UpdateAppearance() {
			this.appearanceCaption = null;
			this.appearanceDescription = null;
			this.appearanceCaptionNormal = null;
			this.appearanceDescriptionNormal = null;
			this.appearanceCaptionHovered = null;
			this.appearanceDescriptionHovered = null;
			this.appearanceCaptionPressed = null;
			this.appearanceDescriptionNormal = null;
			this.appearanceCaptionDisabled = null;
			this.appearanceDescriptionDisabled = null;
		}
		internal void UpdateContent() {
			if(Gallery == null) return;
			UpdateAppearance();
			LayoutContent();
		}
	}
	public class GalleryItemViewInfoCollection : CollectionBase {
		GalleryItemGroupViewInfo group;
		public GalleryItemViewInfoCollection(GalleryItemGroupViewInfo group) {
			this.group = group;
		}
		public GalleryItemGroupViewInfo Group { get { return group; } }
		public int Add(GalleryItemViewInfo item) { return List.Add(item); }
		public void Insert(int index, GalleryItemViewInfo item) { List.Insert(index, item); }
		public void Remove(GalleryItemViewInfo item) { List.Remove(item); }
		public GalleryItemViewInfo this[int index] { get { return List[index] as GalleryItemViewInfo; } }
		public int IndexOf(GalleryItemViewInfo item) { return List.IndexOf(item); }
		public bool Contains(GalleryItemViewInfo item) { return List.Contains(item); }
	}
	public class MacStyleGalleryItemGroupViewInfo : GalleryItemGroupViewInfo {
		public MacStyleGalleryItemGroupViewInfo(BaseGalleryViewInfo galleryInfo, GalleryItemGroup group) : base(galleryInfo, group){ 
		}
		int colsCount;
		public override int LinesCount {
			get {
				return this.colsCount;
			}
		}
		protected internal override Size LayoutItems(Size itemMaxSize, Point topLeft) {
			Point loc = topLeft;
			int width = 0;
			for(int i = 0; i < Items.Count; i++) {
				Items[i].Row = 0;
				Items[i].Column = i;
				Items[i].LayoutContent(itemMaxSize, CheckLocationRTL(loc, itemMaxSize));
				GalleryInfo.ColCount++;
				loc.X += itemMaxSize.Width + GalleryInfo.HDistanceBetweenItems;
			}
			width += itemMaxSize.Width * Items.Count;
			GalleryInfo.ColCount += Items.Count;
			this.colsCount = Items.Count;
			if(Items.Count > 0)
				width += GalleryInfo.HDistanceBetweenItems * (Items.Count - 1);
			Size contSize = new Size(width, itemMaxSize.Height);
			SetBounds(new Rectangle(topLeft, contSize));
			return contSize;
		}
		protected override Point CheckLocationRTL(Point location, Size itemMaxSize) {
			if(!Gallery.IsRightToLeft) return location;
			Point pt = new Point(GalleryInfo.ContentBounds.Right - location.X + GalleryInfo.ContentBounds.X - itemMaxSize.Width, location.Y);
			return pt;
		}
		protected internal override void UpdateGroupLayout() {
			base.UpdateGroupLayout();
		}
		protected override bool StopCheckingItemsHitTest(GalleryItemViewInfo item) {
			return GalleryInfo.GalleryContentBounds.IntersectsWith(item.Bounds);
		}
	}
	public class GalleryItemGroupViewInfo {
		GalleryItemGroup group;
		GalleryItemViewInfoCollection items;
		Rectangle captionBounds, captionContentBounds;
		Rectangle bounds;
		BaseGalleryViewInfo galleryInfo;
		int rowsCount;
		public GalleryItemGroupViewInfo(BaseGalleryViewInfo galleryInfo, GalleryItemGroup group) {
			this.group = group;
			this.items = new GalleryItemViewInfoCollection(this);
			this.captionBounds = Rectangle.Empty;
			this.captionContentBounds = Rectangle.Empty;
			this.bounds = Rectangle.Empty;
			this.galleryInfo = galleryInfo;
		}
		public RibbonGalleryAppearances PaintAppearance { get { return GalleryInfo.PaintAppearance; } }
		public BaseGalleryViewInfo GalleryInfo { get { return galleryInfo; } }
		public BaseGallery Gallery { get { return GalleryInfo.Gallery; } }
		public GalleryItemGroup Group { get { return group; } }
		public GalleryItemViewInfoCollection Items { get { return items; } }
		public Rectangle CaptionBounds { get { return captionBounds; } }
		public StringInfo CaptionStringInfo { get; set; }
		public Rectangle CaptionContentBounds { get { return captionContentBounds; } }
		public Rectangle Bounds { get { return bounds; } }
		protected void SetBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		public virtual int LinesCount { get { return RowsCount; } }
		public int RowsCount { get { return rowsCount; } }
		protected virtual void ClearItemsViewInfo() {
			Items.Clear();
		}
		protected virtual GalleryItemViewInfo CreateItemViewInfo(GalleryItemGroupViewInfo groupInfo, GalleryItem item) {
			return new GalleryItemViewInfo(groupInfo, item);
		}
		protected internal virtual void CreateItemsViewInfo() {
			ClearItemsViewInfo();
			for(int i = 0; i < Group.Items.Count; i++) {
				if(Group.Items[i].Visible)
					Items.Add(CreateItemViewInfo(this, Group.Items[i]));
			}
		}
		protected virtual Rectangle CalcCaptionBoundsByContentBounds() {
			return ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, GalleryInfo.Gallery.Painter.GetCaptionPanelInfo(this), CaptionContentBounds);
		}
		protected internal Size CaptionBestSize { get; private set; }
		protected internal virtual void CalcBestSize() {
			if(Gallery.ShowGroupCaption) {
				this.captionContentBounds = CalcCaptionContentBounds();
				this.captionBounds = CalcCaptionBoundsByContentBounds();
				this.CaptionBestSize = this.captionBounds.Size;
			}
			for(int i = 0; i < Items.Count; i++) {
				Items[i].CalcBestSize();
			}
		}
		protected internal int GetMaxWidth() {
			int maxWidth = 0;
			for(int i = 0; i < Items.Count; i++) {
				maxWidth = Math.Max(maxWidth, Items[i].GetItemWidth());
			}
			return maxWidth;
		}
		protected internal int GetMaxHeight() {
			int maxHeight = 0;
			for(int i = 0; i < Items.Count; i++) {
				maxHeight = Math.Max(maxHeight, Items[i].GetItemHeight());
			}
			return maxHeight;
		}
		protected GraphicsInfo GInfo { get { return GalleryInfo.GInfo; } }
		protected virtual Size GetCaptionSize() {
			GInfo.AddGraphics(null);
			try {
				if(Group.CaptionControl != null) 
					return Group.CaptionControlSize;
				Size res = Size.Empty;
				if(Group.Gallery.AllowHtmlText) {
					if(GalleryInfo.IsVertical) {
						CalcCaptionStringInfo(GInfo.Graphics);
						res = CaptionStringInfo.Bounds.Size;
					}
					else { 
						res = PaintAppearance.GroupCaption.CalcTextSize(GInfo.Graphics, StringPainter.Default.RemoveFormat(Group.Caption), 0).ToSize(); }
				}
				else {
					res = PaintAppearance.GroupCaption.CalcTextSize(GInfo.Graphics, Group.Caption, 0).ToSize();
				}
				return new Size(GalleryInfo.GetSecondarySize(res), GalleryInfo.GetDefineSize(res));
			}
			finally { GInfo.ReleaseGraphics(); }
		}
		protected internal void CalcCaptionStringInfo(Graphics g) {
			StringInfo stringInfo = StringPainter.Default.Calculate(g, PaintAppearance.GroupCaption, PaintAppearance.GroupCaption.GetTextOptions(), Group.Caption, CaptionBounds);
			CaptionStringInfo = stringInfo;
		}
		protected virtual Rectangle CalcCaptionContentBounds() {
			return new Rectangle(Point.Empty, GetCaptionSize());
		}
		protected virtual Rectangle CalcCaptionContentBoundsByBounds() {
			Rectangle res = SkinElementPainter.GetObjectClientRectangle(null, SkinElementPainter.Default, GalleryInfo.Gallery.Painter.GetCaptionPanelInfo(this), CaptionBounds);
			int width = res.Width;
			if(Group.CaptionAlignment == GalleryItemGroupCaptionAlignment.Stretch)
				return res;
			res.Width = CaptionContentBounds.Width;
			if(Group.CaptionAlignment == GalleryItemGroupCaptionAlignment.Center) {
				res.X += (width - res.Width) / 2;
			}
			if(Group.CaptionAlignment == GalleryItemGroupCaptionAlignment.Right) {
				res.X = res.X + width - res.Width;
			}
			return res;
		}
		protected virtual Rectangle UpdateCaptionBounds(Point pt) {
			int width = Math.Max(GalleryInfo.GetSecondarySize(CaptionBounds), GalleryInfo.GetSecondarySize(GalleryInfo.GalleryContentBounds) - GalleryInfo.GetSecondarySize(GalleryInfo.ControlSize) - GalleryInfo.IndentBetweenContentAndControl);
			return new Rectangle(pt, GalleryInfo.ApplyOrientation(new Size(width, GalleryInfo.GetDefineSize(CaptionBounds))));
		}
		protected virtual int GetHorizontalContentOffset() {
			if(GalleryInfo.ItemHorizontalAlignmet == HorzAlignment.Near)
				return 0;
			if(GalleryInfo.ItemHorizontalAlignmet == HorzAlignment.Far)
				return GalleryInfo.GetSecondarySize(Bounds) - GalleryInfo.GetSecondarySize(ContentSize);
			return (GalleryInfo.GetSecondarySize(Bounds) - GalleryInfo.GetSecondarySize(ContentSize)) / 2;
		}
		protected internal virtual void UpdateGroupLayout() {
			Point offset = new Point(0, GalleryInfo.OffsetFromContentTop);
			offset = new Point(GalleryInfo.GetSecondaryCoor(offset), GalleryInfo.GetDefineCoor(offset));
			this.bounds.Offset(offset);
			this.captionBounds.Offset(offset);
			this.captionContentBounds.Offset(offset);
			if(GalleryInfo.IsVertical) 
				offset.X = GetHorizontalContentOffset();
			else
				offset.Y += GetHorizontalContentOffset();
			for(int i = 0; i < Items.Count; i++) {
				Items[i].UpdateLayout(offset);
			}
		}
		Size contentSize;
		protected Size ContentSize { get { return contentSize; } set { contentSize = value; } }
		protected virtual void LayoutGroupCaption(ref Point location, Point topLeft) {
			this.captionBounds = UpdateCaptionBounds(topLeft);
			GalleryInfo.OffsetDefineCoor(ref location, GalleryInfo.GetDefineSize(CaptionBounds));
			this.captionContentBounds = CalcCaptionContentBoundsByBounds();
			if(Group.CaptionControl != null)
				UpdateGroupCaptionControlBounds();
			if(Gallery.AllowHtmlText) {
				CalcCaptionStringInfo(GInfo.Graphics);
			}
			CheckCaptionRTL();
		}
		protected virtual void CheckCaptionRTL() {
			if(!Gallery.IsRightToLeft) return;
			captionContentBounds = BarUtilites.ConvertBoundsToRTL(captionContentBounds, galleryInfo.ContentBounds);
			captionBounds = BarUtilites.ConvertBoundsToRTL(captionBounds, galleryInfo.ContentBounds);
		}
		protected virtual bool ShouldStartNewRow(Size itemMaxSize, int availableWidth, int currentWidth, int itemsInRow) { 
			int contentX = GalleryInfo.GetSecondaryCoor(GalleryInfo.GalleryContentBounds) - GalleryInfo.GetSecondaryCoor(GalleryInfo.Bounds);
			return (contentX + currentWidth + GalleryInfo.GetSecondarySize(itemMaxSize) > availableWidth) || (itemsInRow >= Gallery.ColumnCount && !GalleryInfo.AutoFitColumns);
		}
		protected internal virtual Size LayoutItems(Size itemMaxSize, Point topLeft) {
			Point location = topLeft;
			GalleryInfo.OffsetSecondaryCoor(ref location, GalleryInfo.ActualGroupContentMargin.Left);
			int width = 0, realColCount = 0, itemsInRow = 0, maxWidth = 0;
			if(Group.Gallery.ShowGroupCaption)
				LayoutGroupCaption(ref location, topLeft);
			GalleryInfo.OffsetDefineCoor(ref location, GalleryInfo.ActualGroupContentMargin.Top);
			if(GalleryInfo.IsVertical)
				topLeft.X = location.X;
			else
				topLeft.Y = location.Y;
			int availableWidth = GalleryInfo.GetSecondarySize(GalleryInfo.VisibleSize) - GalleryInfo.GetSecondarySize(GalleryInfo.ControlSize) - GalleryInfo.ActualGroupContentMargin.Right;
			this.rowsCount = Items.Count > 0 ? 1 : 0;
			for(int i = 0; i < Items.Count; i++) {
				if(i > 0 && ShouldStartNewRow(itemMaxSize, availableWidth, width, itemsInRow)) {
					if(realColCount == 0) realColCount = i;
					GalleryInfo.OffsetDefineCoor(ref location, GalleryInfo.GetDefineSize(itemMaxSize) + GalleryInfo.VDistanceBetweenItems);
					this.rowsCount++;
					GalleryInfo.RowCount++;
					itemsInRow = 0;
					if(GalleryInfo.IsVertical)
						location.X = topLeft.X;
					else
						location.Y = topLeft.Y;
					width = 0;
				}
				Items[i].Row = GalleryInfo.RowCount;
				Items[i].Column = itemsInRow;
				Items[i].LayoutContent(itemMaxSize, CheckLocationRTL(location, itemMaxSize));
				width += GalleryInfo.GetSecondarySize(itemMaxSize)+ GalleryInfo.HDistanceBetweenItems;
				maxWidth = Math.Max(maxWidth, width);
				GalleryInfo.OffsetSecondaryCoor(ref location, GalleryInfo.GetSecondarySize(itemMaxSize) + GalleryInfo.HDistanceBetweenItems);
				itemsInRow++;
			}
			GalleryInfo.OffsetDefineCoor(ref location, GalleryInfo.ActualGroupContentMargin.Bottom);
			Size contSize = CalcRealGroupSize(GalleryInfo.GetSecondarySize(itemMaxSize), GalleryInfo.GetDefineCoor(location) - GalleryInfo.GetDefineCoor(topLeft) + GalleryInfo.GetDefineSize(itemMaxSize));
			contSize = GalleryInfo.ApplyOrientation(contSize);
			ContentSize = GalleryInfo.ApplyOrientation(new Size(maxWidth, contSize.Height));
			if(contSize == Size.Empty && Gallery.DesignModeCore)
				contSize = new Size(GalleryInfo.GetSecondarySize(GalleryInfo.GalleryContentBounds) - GalleryInfo.GetSecondarySize(GalleryInfo.ControlSize), 16);
			int contWidth = GalleryInfo.GetSecondarySize(GalleryInfo.GalleryContentBounds) - GalleryInfo.GetSecondarySize(GalleryInfo.ControlSize);
			if(!GalleryInfo.IsCalculatingBestSize) {
				if(GalleryInfo.IsVertical)
					contSize.Width = contWidth;
				else
					contSize.Height = contWidth;
			}
			SetBounds(new Rectangle(topLeft, contSize));
			CheckGroupBoundsRTL();
			GalleryInfo.RowCount++;
			return contSize;
		}
		protected virtual void CheckGroupBoundsRTL() {
			if(!Gallery.IsRightToLeft) return;
			SetBounds(BarUtilites.ConvertBoundsToRTL(Bounds, GalleryInfo.Bounds));
		}
		protected virtual Point CheckLocationRTL(Point location, Size itemMaxSize) {
			if(!Gallery.IsRightToLeft) return location;
			if(Gallery is InRibbonGallery) {
				return new Point(galleryInfo.ContentBounds.Right - location.X + galleryInfo.ContentBounds.X - itemMaxSize.Width, location.Y);
			}
			return new Point(galleryInfo.ContentBounds.Width - location.X - itemMaxSize.Width, location.Y);
		}
		protected virtual void UpdateGroupCaptionControlBounds() {
			Group.CaptionControl.Bounds = GalleryInfo.TranslateRect(CaptionContentBounds);
		}
		protected virtual Size CalcRealGroupSize(int itemMaxWidth, int height) {
			int colCnt = CalcRealColCount(itemMaxWidth);
			return new Size(colCnt == 0 ? 0 : itemMaxWidth * colCnt + (colCnt - 1) * GalleryInfo.HDistanceBetweenItems + GalleryInfo.ActualGroupContentMargin.Horizontal, height);
		}
		protected virtual int CalcRealColCount(int itemMaxWidth) {
			int width = itemMaxWidth + GalleryInfo.HDistanceBetweenItems;
			if(width == 0) return 0;
			return (GalleryInfo.VisibleSize.Width + GalleryInfo.HDistanceBetweenItems) / width;
		}
		protected virtual bool AllowPartitalItems { get { return GalleryInfo.AllowPartitalItems; } }
		public virtual bool IgnoreItem(GalleryItemViewInfo item) {
			return item.IsInvisible || ((item.IsPartiallyVisible && item.Bounds.Height < GalleryInfo.GalleryContentBounds.Height) && !item.GalleryInfo.AllowPartitalItems);
		}
		protected virtual bool StopCheckingItemsHitTest(GalleryItemViewInfo item) {
			return item.Bounds.Y > GalleryInfo.GalleryContentBounds.Bottom;
		}
		protected internal virtual bool CalcHitInfo(RibbonHitInfo hitInfo) {
			if(!hitInfo.ContainsSet(Bounds, RibbonHitTest.GalleryItemGroup)) return false;
			hitInfo.GalleryItemGroupInfo = this;
			for(int i = 0; i < Items.Count; i++) {
				if(IgnoreItem(Items[i])) {
					if(!StopCheckingItemsHitTest(Items[i])) continue;
				}
				if(hitInfo.ContainsSet(Items[i].Bounds, RibbonHitTest.GalleryItem)) {
					hitInfo.GalleryItemInfo = Items[i];
					hitInfo.ContainsSet(Items[i].ImageBounds, RibbonHitTest.GalleryImage);
				}
			}
			return true;
		}
		internal virtual bool ShouldDrawGroup() { return Bounds.IntersectsWith(GalleryInfo.GalleryContentBounds); }
	}
	public class GalleryItemGroupViewInfoCollection : CollectionBase {
		BaseGalleryViewInfo gallery;
		public GalleryItemGroupViewInfoCollection(BaseGalleryViewInfo gallery) {
			this.gallery = gallery;
		}
		public BaseGalleryViewInfo Gallery { get { return gallery; } }
		public int Add(GalleryItemGroupViewInfo item) { return List.Add(item); }
		public void Insert(int index, GalleryItemGroupViewInfo item) { List.Insert(index, item); }
		public void Remove(GalleryItemGroupViewInfo item) { List.Remove(item); }
		public GalleryItemGroupViewInfo this[int index] { get { return List[index] as GalleryItemGroupViewInfo; } }
		public int IndexOf(GalleryItemGroupViewInfo item) { return List.IndexOf(item); } 
	}
}
