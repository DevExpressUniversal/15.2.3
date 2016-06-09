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
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.Utils.Text;
using System.Drawing.Imaging;
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public enum InRibbonGalleryControlButtons { None, UpButton, DownButton, DropDownButton, LeftButton, RightButton }
	public class PopupImageGalleryInfoArgs : ImageGalleryInfoArgs {
		public PopupImageGalleryInfoArgs(BaseGalleryViewInfo viewInfo) : base(viewInfo) { }
		public new StandaloneGalleryViewInfo ViewInfo {
			get { return base.ViewInfo as StandaloneGalleryViewInfo; }
			set { base.ViewInfo = value; }
		}
	}
	public class InRibbonImageGalleryInfoArgs : ImageGalleryInfoArgs {
		InRibbonGalleryControlButtons controlButtons;
		BaseRibbonViewInfo ribbonInfo;   
		public InRibbonImageGalleryInfoArgs(BaseRibbonViewInfo ribbonInfo, BaseGalleryViewInfo viewInfo) : this(ribbonInfo, viewInfo, null, false, false) { }
		public InRibbonImageGalleryInfoArgs(BaseRibbonViewInfo ribbonInfo, BaseGalleryViewInfo viewInfo, GalleryItemViewInfo itemInfo, bool hot, bool itemPressed)
			: this(ribbonInfo, viewInfo, itemInfo, hot, itemPressed, InRibbonGalleryControlButtons.None) { 
		}
		public InRibbonImageGalleryInfoArgs(BaseRibbonViewInfo ribbonInfo, BaseGalleryViewInfo viewInfo, GalleryItemViewInfo itemInfo, bool hot, bool itemPressed, InRibbonGalleryControlButtons controlButtons)
			: base(viewInfo, itemInfo, hot, itemPressed) {
			this.controlButtons = controlButtons;
			this.ribbonInfo = ribbonInfo;
		}
		public InRibbonGalleryControlButtons ControlButtons { get { return controlButtons; } set { controlButtons = value; } }
		public new InRibbonGalleryViewInfo ViewInfo {
			get { return base.ViewInfo as InRibbonGalleryViewInfo; }
			set { base.ViewInfo = value; }
		}
		public BaseRibbonViewInfo RibbonInfo { get { return ribbonInfo; } set { ribbonInfo = value; } }
	}
	public class GalleryItemInfoArgs : ObjectInfoArgs {
		GalleryItemViewInfo viewInfo;
		public GalleryItemInfoArgs(GalleryItemViewInfo info, ObjectState state) : base(null, info.Bounds, state) {
			this.viewInfo = info;
			this.Bounds = info.Bounds;
		}
		public BaseGallery Gallery { get { return ViewInfo == null ? null : ViewInfo.Gallery; } }
		public virtual GalleryItemViewInfo ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
	}
	public class ImageGalleryInfoArgs : EventArgs {
		BaseGalleryViewInfo viewInfo;
		GalleryItemViewInfo itemInfo;
		bool hot, itemPressed;
		bool drawContent = true;
		bool drawFilter = true;
		public ImageGalleryInfoArgs(BaseGalleryViewInfo viewInfo) : this(viewInfo, null, false, false) { }
		public ImageGalleryInfoArgs(BaseGalleryViewInfo viewInfo, GalleryItemViewInfo itemInfo, bool hot, bool itemPressed) {
			this.viewInfo = viewInfo;
			this.itemInfo = itemInfo;
			this.hot = hot;
			this.itemPressed = itemPressed;
		}
		public virtual BaseGalleryViewInfo ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
		public GalleryItemViewInfo ActiveItemInfo { get { return itemInfo; } set { itemInfo = value; } }
		public bool ItemPressed { get { return itemPressed; } set { itemPressed = value; } }
		public bool Hot { get { return hot; } set { hot = value; } }
		public bool DrawFilter { get { return drawFilter; } set { drawFilter = value; } }
		public bool DrawContent { get { return drawContent; } set { drawContent = value; } }
		public ObjectState ActiveItemState { 
			get {
				ObjectState res = ObjectState.Normal;
				if(ViewInfo.Gallery.DesignModeCore) return ObjectState.Normal;
				if(ActiveItemInfo == null) return ObjectState.Normal;
				if(ItemPressed) return ObjectState.Pressed;
				if(Hot) res |= ObjectState.Hot;
				if(ActiveItemInfo.Item.Checked) res |= ObjectState.Selected;
				return res;
			} 
		}
	}
	public abstract class GalleryItemPainter {
		public virtual SkinElementInfo GetButtonInfo(BaseGallery gallery, Rectangle bounds, ObjectState state) {
			SkinElementInfo info = null;
			if(state != ObjectState.Disabled && state != ObjectState.Normal) {
				info = new SkinElementInfo(gallery.GetSkin(GetButtonSkinName()), bounds);
				info.RightToLeft = gallery.IsRightToLeft;
				info.State = state;
				info.ImageIndex = CalcButtonImageIndex(state);
			}
			return info;
		}
		public virtual SkinElementInfo GetSelectedImageInfo(BaseGallery gallery, Rectangle bounds, ObjectState state) {
			SkinElementInfo info = null;
			if(state != ObjectState.Disabled && state != ObjectState.Normal) {
				info = new SkinElementInfo(BarSkins.GetSkin(gallery.Provider)[BarSkins.SkinPopupMenuCheck], bounds);
				info.RightToLeft = gallery.IsRightToLeft;
				info.State = state;
				info.ImageIndex = 1;
			}
			return info;
		}
		protected virtual int CalcButtonImageIndex(ObjectState state) {
			if(state == ObjectState.Normal) return 0;
			if(state == ObjectState.Selected) return 3;
			if((state & ObjectState.Selected) != 0) return 4;
			if(state == ObjectState.Hot) return 1;
			return 2;
		}
		public virtual SkinElementInfo GetImageBackgroundInfo(BaseGallery gallery, Rectangle imageBounds) {
			if(!gallery.DrawImageBackground || !gallery.ShowItemImage) return new SkinElementInfo(null, imageBounds);
			SkinElementInfo info = new SkinElementInfo(gallery.GetSkin(GetImageBackgroundSkinName()), imageBounds);
			info.RightToLeft = gallery.IsRightToLeft;
			return info;
		}
		protected abstract string GetImageBackgroundSkinName();
		protected abstract string GetButtonSkinName();
		public virtual bool DrawButtonLast(GalleryItemInfoArgs e) { return (e.State & ObjectState.Pressed) != 0; }
		public virtual void DrawButton(GalleryItemInfoArgs e) {
			if(e.Gallery.CheckDrawMode != CheckDrawMode.OnlyImage || (e.State & ObjectState.Hot) != 0 || (e.State & ObjectState.Pressed) != 0)
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetButtonInfo(e.Gallery, e.Bounds, e.State));
			if(e.Gallery.CheckDrawMode == CheckDrawMode.OnlyImage && (e.State & ObjectState.Selected) != 0)
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetSelectedImageInfo(e.Gallery, e.ViewInfo.SelectedImageBounds, e.State));
		}
		public virtual void DrawText(GraphicsCache cache, GalleryItemInfoArgs e) {
			if(e.ViewInfo.Gallery.ShowItemText) {
				if(e.ViewInfo.Gallery.RaiseCustomDrawItemText(cache, e.ViewInfo, e.ViewInfo.TextBounds)) return;
				if(!string.IsNullOrEmpty(e.ViewInfo.Item.Caption)) { 
					DrawItemCaption(cache, e);
				}
				if(!string.IsNullOrEmpty(e.ViewInfo.Item.Description)) {
					DrawItemDescription(cache, e);
				}
			}
		}
		protected virtual void DrawItemCaption(GraphicsCache cache, GalleryItemInfoArgs e) {
			AppearanceObject appearance = e.ViewInfo.AppearanceCaption; 
			string str = e.ViewInfo.Item.Caption;
			if(e.ViewInfo.Item.Gallery.AllowHtmlText) {
				StringPainter.Default.DrawString(cache, e.ViewInfo.CaptionStringInfo);
				return;
			}
			if(e.ViewInfo.ShouldDrawSelectedItemCaption(e)) {
				appearance.DrawString(cache, str, e.ViewInfo.CaptionBounds, cache.GetSolidBrush(e.ViewInfo.ItemCaptionSelectedForeColor));
			}
			else {
				appearance.DrawString(cache, str, e.ViewInfo.CaptionBounds);
			}
		}
		protected virtual void DrawItemDescription(GraphicsCache cache, GalleryItemInfoArgs e) {
			AppearanceObject appearance = e.ViewInfo.AppearanceDescription; 
			string str = e.ViewInfo.Item.Description;
			if(e.ViewInfo.Item.Gallery.AllowHtmlText) {
				StringPainter.Default.DrawString(cache, e.ViewInfo.DescriptionStringInfo);
				return;
			}
			if(e.ViewInfo.ShouldDrawSelectedItemDescription(e)) {
				appearance.DrawString(cache, str, e.ViewInfo.DescriptionBounds, cache.GetSolidBrush(e.ViewInfo.ItemDescriptionSelectedForeColor));
			}
			else {
				appearance.DrawString(cache, str, e.ViewInfo.DescriptionBounds);
			}
		}
		protected virtual void DrawSelection(GalleryItemInfoArgs e) {
			if(e.Gallery.Ribbon == null)
				return;
			BaseRibbonViewInfo ribbonInfo = e.Gallery.Ribbon.ViewInfo;
			if(!ribbonInfo.IsDesignMode || ribbonInfo.DesignTimeManager.DragItem != e.ViewInfo.Item) return;
			ribbonInfo.DesignTimeManager.DrawSelection(e.Cache, e.ViewInfo.Bounds, Color.Magenta);
		}
		protected virtual void DrawDesignerBounds(GalleryItemInfoArgs e) {
			using(Pen pen = new Pen(Color.Gray)) {
				Rectangle bounds = e.Bounds;
				bounds.Inflate(-2, -2);
				pen.DashPattern = new float[] { 2.0f, 2.0f };
				e.Cache.Graphics.DrawRectangle(pen, bounds);
			}
		}
		RibbonSelectionInfo GetSelectionInfo(GalleryItemInfoArgs e) {
			return e.ViewInfo.Gallery.Ribbon.Manager.SelectionInfo as RibbonSelectionInfo;
		}
		public virtual void Draw(GalleryItemInfoArgs e) {
			if(!e.Cache.IsNeedDrawRect(e.Bounds)) return;
			if(!DrawButtonLast(e)) DrawButton(e);
			if(e.Gallery.DrawImageBackground && e.Gallery.ShowItemImage)
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetImageBackgroundInfo(e.Gallery, e.ViewInfo.ImageBounds));
			DrawImage(e);
			DrawText(e.Cache, e);
			if(DrawButtonLast(e)) DrawButton(e);
			DrawContextButtons(e);
			if(e.Gallery.DesignModeCore) DrawDesignerBounds(e);
			DrawSelection(e);
			if(e.Gallery.DesignModeCore)
				DrawDropMark(e);
		}
		protected virtual void DrawContextButtons(GalleryItemInfoArgs e) {
			if(e.ViewInfo.ShouldDrawContextButtons)
				new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(e.ViewInfo.ContextButtonsViewInfo, e.Cache, e.ViewInfo.Bounds));
		}
		protected virtual void DrawImage(GalleryItemInfoArgs e) {
			if(!e.ViewInfo.Gallery.ShowItemImage)
				return;
			RectangleF clipBounds = e.Graphics.ClipBounds;
			Image img = e.ViewInfo.Item.Image != null? e.ViewInfo.Item.Image: ImageCollection.GetImageListImage(e.Gallery.Images, e.ViewInfo.Item.ImageIndex);
			bool shouldClipImage = e.ViewInfo.Gallery.ItemImageLayout != ImageLayoutMode.Stretch && e.ViewInfo.Gallery.ItemImageLayout != ImageLayoutMode.ZoomInside && img != null && (e.ViewInfo.ImageClientBounds.Width < img.Width || e.ViewInfo.ImageClientBounds.Height < img.Height);
			if(shouldClipImage) {
				e.Graphics.SetClip(e.ViewInfo.ImageClientBounds);
			}
			if(ShouldDrawRenderImage(e.ViewInfo)) {
				Rectangle rect = e.Gallery.GetInvalidateRectangle(e.ViewInfo);
				e.Graphics.SetClip(rect);
				Color backColor = GetBackColor(e.ViewInfo);
				ImageLoaderPaintHelper.DrawRenderImage(e.Graphics, e.ViewInfo, backColor, e.ViewInfo.IsEnabled);
				e.Graphics.SetClip(clipBounds);
			}
			else if(!e.ViewInfo.Gallery.RaiseCustomDrawItemImage(e.Cache, e.ViewInfo, e.ViewInfo.ImageContentBounds)) {
				if(e.ViewInfo.GalleryInfo.AllowGlyphSkinning) {
					ImageAttributes attr = ImageColorizer.GetColoredAttributes(e.ViewInfo.AppearanceCaption.ForeColor);
					ImageCollection.DrawImageListImage(e.Cache, e.ViewInfo.Item.Image, e.Gallery.Images, e.ViewInfo.Item.ImageIndex, e.ViewInfo.ImageContentBounds, attr);
				}
				else {
					ImageCollection.DrawImageListImage(e.Cache, e.ViewInfo.Item.Image, e.Gallery.Images, e.ViewInfo.Item.ImageIndex, e.ViewInfo.ImageContentBounds, e.ViewInfo.IsEnabled);
				}
			}
			if(shouldClipImage) {
				e.Graphics.SetClip(clipBounds);
			}
		}
		protected bool ShouldDrawRenderImage(GalleryItemViewInfo viewInfo) {
			if(viewInfo.Gallery == null || !viewInfo.Gallery.OptionsImageLoad.AsyncLoad) return false;
			ImageLoadInfo info =  viewInfo.Item.ImageInfo;
			return info.ThumbImage != null && info.RenderImageInfo != null && info.IsLoaded && !info.IsAnimationEnd;
		}
		Color GetBackColor(GalleryItemViewInfo itemInfo) {
			if(itemInfo.Gallery is GalleryControlGallery) {
				GalleryControlGallery gallery = (GalleryControlGallery)itemInfo.Gallery;
				if(gallery.BackColor != Color.Empty) return gallery.BackColor;
			}
			return RibbonSkins.GetSkin(itemInfo.Gallery.Provider).GetSystemColor(SystemColors.Window);
		}
		protected virtual void DrawDropMark(GalleryItemInfoArgs e) {
			RibbonDesignTimeManager designTimeManager = (RibbonDesignTimeManager)e.Gallery.Ribbon.ViewInfo.DesignTimeManager;
			if(designTimeManager.DropSelectedObject == e.ViewInfo.Item)
				new PrimitivesPainter(GetSelectionInfo(e).Manager.PaintStyle).DrawGalleryItemDropMark(e.Graphics, e.ViewInfo, designTimeManager.DropSelectStyle);
		}
	}
	public class GalleryItemObjectPainter : ObjectPainter {
		GalleryItemPainter itemPainter;
		public GalleryItemObjectPainter() : this(null) { }
		public GalleryItemObjectPainter(GalleryItemPainter itemPainter) {
			this.itemPainter = itemPainter;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GalleryItemInfoArgs ee = (GalleryItemInfoArgs)e;
			if(itemPainter != null)
				itemPainter.Draw(ee);
			else {
				if(ee.Gallery != null)
					ee.Gallery.Painter.ItemPainter.Draw(ee);
			}
		}
	}
	public class GalleryItemGroupPainter {
		protected virtual GalleryControlPainter GalleryPainter(GalleryItemGroupViewInfo viewInfo) { return viewInfo.GalleryInfo.Gallery.Painter; }
		public virtual void DrawCaption(GraphicsCache cache, GalleryItemGroupViewInfo viewInfo) {
			if(!viewInfo.Gallery.ShowGroupCaption) return;
			if(viewInfo.PaintAppearance.GroupCaption.BackColor != Color.Empty)
				cache.FillRectangle(cache.GetSolidBrush(viewInfo.PaintAppearance.GroupCaption.BackColor), viewInfo.CaptionBounds);
			else
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GalleryPainter(viewInfo).GetCaptionPanelInfo(viewInfo));
			if(viewInfo.Group.CaptionControl == null) {
				if(viewInfo.Group.Gallery.AllowHtmlText) {
					if(viewInfo.GalleryInfo.IsVertical) {
						StringPainter.Default.DrawString(cache, viewInfo.CaptionStringInfo);
				   }
					else {
						string newCaption = StringPainter.Default.RemoveFormat(viewInfo.Group.Caption);
						StringFormat str = viewInfo.PaintAppearance.GroupCaption.GetStringFormat();
						cache.DrawVString(newCaption, viewInfo.PaintAppearance.GroupCaption.Font, viewInfo.PaintAppearance.GroupCaption.GetForeBrush(cache), viewInfo.CaptionContentBounds, str, 270);
					}
				}
				else {
					if(viewInfo.GalleryInfo.IsVertical)
						viewInfo.PaintAppearance.GroupCaption.DrawString(cache, viewInfo.Group.Caption, viewInfo.CaptionContentBounds);
					else {
						StringFormat str = viewInfo.PaintAppearance.GroupCaption.GetStringFormat();
						cache.DrawVString(viewInfo.Group.Caption, viewInfo.PaintAppearance.GroupCaption.Font, viewInfo.PaintAppearance.GroupCaption.GetForeBrush(cache), viewInfo.CaptionContentBounds, str, 270);
					}
				}
			}
		}
		RibbonSelectionInfo GetSelectionInfo(GalleryItemGroupViewInfo info) {
			return info.Gallery.Ribbon.Manager.SelectionInfo as RibbonSelectionInfo;
		}
		protected virtual bool ShouldStopItemDrawIteration(GalleryItemViewInfo item, GalleryItemGroupViewInfo viewInfo) {
			return item.Bounds.Y >= viewInfo.GalleryInfo.GalleryContentBounds.Y;
		}
		protected virtual bool CanReverseIteration(GalleryItemGroupViewInfo vi) {
			return false;
		}
		public virtual void Draw(GraphicsCache cache, GalleryItemGroupViewInfo viewInfo, ImageGalleryInfoArgs gi) {
			DrawCaption(cache, viewInfo);
			GalleryItemPainter itemPainter = GalleryPainter(viewInfo).ItemPainter;
			for(int i = 0; i < viewInfo.Items.Count; i++) {
				GalleryItemViewInfo item = viewInfo.Items[CanReverseIteration(viewInfo) ? viewInfo.Items.Count - 1 - i : i];
				if(viewInfo.IgnoreItem(item)) {
					if(!ShouldStopItemDrawIteration(item, viewInfo)) continue;
					else break;
				}
				GalleryItemInfoArgs e = CreateItemInfo(item, gi);
				e.Cache = cache;
				XtraAnimator.Current.DrawAnimationHelper(cache, gi.ViewInfo.OwnerControl as ISupportXtraAnimation, e.ViewInfo,
					new GalleryItemObjectPainter(itemPainter), e,
					new DrawTextInvoker(DrawItemText), e);
			}
			if(viewInfo.Gallery.DesignModeCore) DrawDesignerBounds(cache, viewInfo);				
			DrawSelection(cache, viewInfo);
			if(viewInfo.Gallery.DesignModeCore)
				DrawDropMark(cache, viewInfo);
		}
		protected virtual void DrawDropMark(GraphicsCache cache, GalleryItemGroupViewInfo viewInfo) {
			if(GetSelectionInfo(viewInfo).DropSelectedObject == viewInfo.Group)
				new PrimitivesPainter(GetSelectionInfo(viewInfo).Manager.PaintStyle).DrawGalleryItemGroupDropMark(cache.Graphics, viewInfo, GetSelectionInfo(viewInfo).DropSelectStyle);
		}
		void DrawDesignerBounds(GraphicsCache cache, GalleryItemGroupViewInfo viewInfo) {
			using(Pen pen = new Pen(Color.Red)) {
				pen.DashPattern = new float[] { 5.0f, 5.0f };
				cache.Graphics.DrawRectangle(pen, viewInfo.Bounds);
			}
		}
		void DrawItemText(GraphicsCache cache, object info) {
			GalleryItemInfoArgs item = (GalleryItemInfoArgs)info;
			item.Gallery.Painter.ItemPainter.DrawText(cache, item);
		}
		public virtual void DrawText(GraphicsCache cache, GalleryItemGroupViewInfo viewInfo) {
			DrawCaption(cache, viewInfo);
			GalleryItemPainter itemPainter = GalleryPainter(viewInfo).ItemPainter;
			foreach(GalleryItemViewInfo item in viewInfo.Items) {
				if(viewInfo.IgnoreItem(item)) {
					if(item.Bounds.Y < viewInfo.GalleryInfo.GalleryContentBounds.Y) continue;
					else break;
				}
				itemPainter.DrawText(cache, new GalleryItemInfoArgs(item, viewInfo.GalleryInfo.CalcItemState(item)));
			}
		}
		protected internal virtual GalleryItemInfoArgs CreateItemInfo(GalleryItemViewInfo item, ImageGalleryInfoArgs gi) {
			GalleryItemInfoArgs e = new GalleryItemInfoArgs(item, item == gi.ActiveItemInfo ? gi.ActiveItemState : ObjectState.Normal);
			if(item.Gallery == null || item.Gallery.DesignModeCore)
				return e;
			e.State = item.CalcState();
			return e;
		}
		protected virtual void DrawSelection(GraphicsCache cache, GalleryItemGroupViewInfo viewInfo) {
			if(viewInfo.Gallery.Ribbon == null)
				return;
			BaseRibbonViewInfo ribbonInfo = viewInfo.Gallery.Ribbon.ViewInfo;
			if(!ribbonInfo.IsDesignMode || ribbonInfo.DesignTimeManager.DragItem != viewInfo.Group) return;
			ribbonInfo.DesignTimeManager.DrawSelection(cache, viewInfo.Bounds, Color.Magenta);
		}
	}
	public abstract class GalleryControlPainter {
		GalleryItemPainter itemPainter;
		public virtual SkinElementInfo GetBackgroundInfo(BaseGalleryViewInfo viewInfo) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinPopupGalleryBackground], viewInfo.Bounds);
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			if(viewInfo.OwnerControl != null && !viewInfo.OwnerControl.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		public virtual SkinElementInfo GetBackgroundInfo(ImageGalleryInfoArgs args) {
			SkinElementInfo info = GetBackgroundInfo(args.ViewInfo);
			info.RightToLeft = args.ViewInfo.Gallery.IsRightToLeft;
			if(args.Hot) info.ImageIndex = 1;
			if(args.ViewInfo.OwnerControl != null && !args.ViewInfo.OwnerControl.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		public virtual SkinElementInfo GetCaptionPanelInfo(GalleryItemGroupViewInfo viewInfo) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinPopupGalleryGroupCaption], viewInfo.CaptionBounds);
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			if(viewInfo.GalleryInfo.OwnerControl != null && !viewInfo.GalleryInfo.OwnerControl.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected virtual void DrawBackground(GraphicsCache cache, ImageGalleryInfoArgs args) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetBackgroundInfo(args));
		}
		public virtual void DrawSizer(GraphicsCache cache, ImageGalleryInfoArgs args) { 
		}
		public virtual void Draw(GraphicsCache cache, ImageGalleryInfoArgs args) {
			if(!cache.IsNeedDrawRect(args.ViewInfo.Bounds)) return;
			DrawBackground(cache, args);
			GraphicsClipState clip = cache.ClipInfo.SaveAndSetClip(args.ViewInfo.GalleryContentBounds);
			for(int i = 0; i < args.ViewInfo.Groups.Count; i++) {
				GalleryItemGroupViewInfo group = args.ViewInfo.Groups[i];
				if(!group.ShouldDrawGroup()) continue;
				GroupPainter.Draw(cache, group, args);
			}
			cache.ClipInfo.RestoreClipRelease(clip);
			DrawSelection(cache, args);
		}
		protected virtual void DrawSelection(GraphicsCache cache, ImageGalleryInfoArgs args) {
			if(args.ViewInfo.Selection == Rectangle.Empty)
				return;
			SkinElement elem = CommonSkins.GetSkin(args.ViewInfo.Gallery.Provider)[CommonSkins.SkinSelection];
			if(elem == null) {
				cache.FillRectangle(cache.GetSolidBrush(Color.FromArgb(0x40, Color.Pink)), args.ViewInfo.Selection);
				cache.DrawRectangle(cache.GetPen(Color.Pink), args.ViewInfo.Selection);
				return;
			}
			SelectionPainter.Default.Draw(cache, args.ViewInfo.Gallery.Provider, args.ViewInfo.Selection);
		}
		protected virtual void DrawSideControl(GraphicsCache cache, ImageGalleryInfoArgs args) { }
		public GalleryItemPainter ItemPainter {
			get {
				if(itemPainter == null) itemPainter = CreateItemPainter();
				return itemPainter;
			}
		}
		GalleryItemGroupPainter groupPainter;
		public GalleryItemGroupPainter GroupPainter { 
			get {
				if(groupPainter == null) groupPainter = CreateGroupPainter();
				return groupPainter;
			}
		}
		protected abstract GalleryItemPainter CreateItemPainter();
		protected abstract GalleryItemGroupPainter CreateGroupPainter();
		public virtual void DrawText(GraphicsCache cache, ImageGalleryInfoArgs e) {
			GraphicsClipState clip = cache.ClipInfo.SaveAndSetClip(e.ViewInfo.GalleryContentBounds);
			GalleryItemGroupPainter groupPainter = new GalleryItemGroupPainter();
			foreach(GalleryItemGroupViewInfo group in e.ViewInfo.Groups) { 
				if(group.Bounds.Y > e.ViewInfo.GalleryContentBounds.Bottom) break;
				groupPainter.DrawText(cache, group);
			}
			cache.ClipInfo.RestoreClipRelease(clip);
		}
	}
	public class InRibbonGalleryItemPainter : GalleryItemPainter {
		protected override string GetImageBackgroundSkinName() { return RibbonSkins.SkinGalleryImageBackground; }
		protected override string GetButtonSkinName() { return RibbonSkins.SkinGalleryButton; }
	}
	public class MacStyleInRibbonGalleryItemGroupPainter : GalleryItemGroupPainter {
		protected override bool CanReverseIteration(GalleryItemGroupViewInfo vi) {
			return vi.Gallery.IsRightToLeft;
		}
		protected override bool ShouldStopItemDrawIteration(GalleryItemViewInfo item, GalleryItemGroupViewInfo viewInfo) {
			return item.Bounds.X >= viewInfo.GalleryInfo.Bounds.Right;
		}
	}
	public class MacStyleInRibbonGalleryPainter : InRibbonGalleryPainter {
		protected override GalleryItemGroupPainter CreateGroupPainter() {
			return new MacStyleInRibbonGalleryItemGroupPainter();
		}
		protected virtual SkinElementInfo GetLeftButtonInfo(MacStyleInRibbonGalleryViewInfo viewInfo) {
			SkinElementInfo info = viewInfo.GetLeftButtonInfo();
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			UpdateImageIndexByMouseButton(viewInfo, info, RibbonHitTest.GalleryLeftButton);
			if(!viewInfo.Gallery.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info; 
		}
		protected virtual SkinElementInfo GetRightButtonInfo(MacStyleInRibbonGalleryViewInfo viewInfo) {
			SkinElementInfo info = viewInfo.GetRightButtonInfo();
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			UpdateImageIndexByMouseButton(viewInfo, info, RibbonHitTest.GalleryRightButton);
			if(!viewInfo.Gallery.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected virtual SkinElementInfo GetCommandButtonInfo(MacStyleInRibbonGalleryViewInfo viewInfo) {
			SkinElementInfo info = viewInfo.GetCommandButtonInfo();
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			UpdateImageIndexByMouseButton(viewInfo, info, RibbonHitTest.GalleryDropDownButton);
			if(!viewInfo.Gallery.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			FadeAnimationInfo fadeInfo = XtraAnimator.Current.Get(viewInfo.Ribbon, viewInfo.Item) as FadeAnimationInfo;
			if(fadeInfo != null)
				info.Attributes = fadeInfo.Attributes;
			return info;
		}
		protected virtual void DrawLeftButton(GraphicsCache cache, InRibbonImageGalleryInfoArgs args) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetLeftButtonInfo((MacStyleInRibbonGalleryViewInfo)args.ViewInfo));
		}
		protected virtual void DrawRightButton(GraphicsCache cache, InRibbonImageGalleryInfoArgs args) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetRightButtonInfo((MacStyleInRibbonGalleryViewInfo)args.ViewInfo));
		}
		protected virtual void DrawCommandButton(GraphicsCache cache, InRibbonImageGalleryInfoArgs args) {
			MacStyleInRibbonGalleryViewInfo viewInfo = (MacStyleInRibbonGalleryViewInfo)args.ViewInfo;
			FadeAnimationInfo info = XtraAnimator.Current.Get(viewInfo.Ribbon, viewInfo) as FadeAnimationInfo;
			if(viewInfo.CommandButtonVisible || info != null)
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetCommandButtonInfo((MacStyleInRibbonGalleryViewInfo)args.ViewInfo));
		}
		protected override void DrawSideControl(GraphicsCache cache, ImageGalleryInfoArgs args) {
			InRibbonImageGalleryInfoArgs iargs = args as InRibbonImageGalleryInfoArgs;
			if(iargs == null) return;
			MacStyleInRibbonGalleryViewInfo viewInfo = (MacStyleInRibbonGalleryViewInfo)iargs.ViewInfo;
			if(viewInfo.ScrollYPosition > 0)
				DrawLeftButton(cache, iargs);
			if(viewInfo.ScrollYPosition < viewInfo.MaxScrollYPosition)
				DrawRightButton(cache, iargs);
			DrawCommandButton(cache, iargs);
		}
	}
	public class InRibbonGalleryPainter : GalleryControlPainter {
		protected override GalleryItemPainter CreateItemPainter() {
			return new InRibbonGalleryItemPainter();
		}
		protected override GalleryItemGroupPainter CreateGroupPainter() {
			return new GalleryItemGroupPainter();
		}
		public override SkinElementInfo GetBackgroundInfo(BaseGalleryViewInfo viewInfo) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinGalleryPane], viewInfo.Bounds);
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			return info;
		}
		protected virtual void UpdateImageIndexByMouseButton(InRibbonGalleryViewInfo viewInfo, SkinElementInfo info, RibbonHitTest hitTest) {
			if(viewInfo.ItemInfo.ViewInfo.HotObject.Item == viewInfo.Item && viewInfo.ItemInfo.ViewInfo.HotObject.HitTest == hitTest) {
				info.ImageIndex = 1; 
			}
			if(viewInfo.ItemInfo.ViewInfo.PressedObject.Item == viewInfo.Item && viewInfo.ItemInfo.ViewInfo.PressedObject.HitTest == hitTest) {
				info.ImageIndex = 2;
			}
		}
		protected virtual SkinElementInfo GetUpButtonInfo(InRibbonGalleryViewInfo viewInfo) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinGalleryButtonUp], viewInfo.ButtonUpBounds);
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			UpdateImageIndexByMouseButton(viewInfo, info, RibbonHitTest.GalleryUpButton);
			if(viewInfo.ScrollYPosition == 0) info.ImageIndex = 3;
			if(!viewInfo.Gallery.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected virtual SkinElementInfo GetDownButtonInfo(InRibbonGalleryViewInfo viewInfo) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinGalleryButtonDown], viewInfo.ButtonDownBounds);
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			UpdateImageIndexByMouseButton(viewInfo, info, RibbonHitTest.GalleryDownButton);
			if(viewInfo.ScrollYPosition >= viewInfo.MaxScrollYPosition) info.ImageIndex = 3;
			if(!viewInfo.Gallery.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected virtual SkinElementInfo GetDropDownButtonInfo(InRibbonGalleryViewInfo viewInfo) {
			SkinElementInfo info = viewInfo.GetDropDownButtonInfo();
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			UpdateImageIndexByMouseButton(viewInfo, info, RibbonHitTest.GalleryDropDownButton);
			if(viewInfo.CurrentRibbon.ViewInfo.KeyboardActiveInfo.Item == viewInfo.Item) {
				info.ImageIndex = 1;
				if(viewInfo.Item.IsDroppedDown) info.ImageIndex = 2;
			}
			if(!viewInfo.Gallery.Ribbon.Enabled)
				info.Attributes = PaintHelper.RibbonDisabledAttributes;
			return info;
		}
		protected virtual void DrawUpButton(GraphicsCache cache, InRibbonImageGalleryInfoArgs args) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetUpButtonInfo(args.ViewInfo));
		}
		protected virtual void DrawDownButton(GraphicsCache cache, InRibbonImageGalleryInfoArgs args) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetDownButtonInfo(args.ViewInfo));
		}
		protected virtual void DrawDropDownButton(GraphicsCache cache, InRibbonImageGalleryInfoArgs args) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetDropDownButtonInfo(args.ViewInfo));
		}
		protected override void DrawSideControl(GraphicsCache cache, ImageGalleryInfoArgs args) {
			InRibbonImageGalleryInfoArgs iargs = args as InRibbonImageGalleryInfoArgs;
			if(iargs == null) return;
			DrawUpButton(cache, iargs);
			DrawDownButton(cache, iargs);
			DrawDropDownButton(cache, iargs);
		}
		protected override void DrawBackground(GraphicsCache cache, ImageGalleryInfoArgs args) {
			base.DrawBackground(cache, args);
		}
		protected virtual bool ShouldDrawSelection(InRibbonImageGalleryInfoArgs iargs) {
			if(iargs == null) return false;
			if(!iargs.RibbonInfo.IsDesignMode) return false;
			return !(iargs.RibbonInfo.DesignTimeManager.DragItem is GalleryItem || iargs.RibbonInfo.DesignTimeManager.DragItem is GalleryItemGroup);
		}
		protected virtual void DrawReduceOperationSelection(GraphicsCache cache, InRibbonImageGalleryInfoArgs iargs) {
			if(RibbonReduceOperationHelper.SelectedOperation == null || RibbonReduceOperationHelper.Ribbon != ((RibbonViewInfo)iargs.RibbonInfo).Ribbon)
				return;
			int linkIndex = iargs.ViewInfo.ItemInfo.OwnerPageGroup.ItemLinks.IndexOf((BarItemLink)iargs.ViewInfo.Item);
			if(RibbonReduceOperationHelper.SelectedOperation.Operation != ReduceOperationType.Gallery || 
				RibbonReduceOperationHelper.SelectedOperation.ItemLinkIndex != linkIndex)
				return;
			iargs.RibbonInfo.DesignTimeManager.DrawSelection(cache, iargs.ViewInfo.Bounds, Color.Magenta);
		}
		protected virtual void DrawSelection(GraphicsCache cache, InRibbonImageGalleryInfoArgs iargs) {
			if(iargs == null) return;
			if(iargs.RibbonInfo.IsDesignMode && iargs.RibbonInfo.DesignTimeManager.IsComponentSelected(iargs.ViewInfo.Gallery.OwnerItem) && ShouldDrawSelection(iargs)) {
				iargs.RibbonInfo.DesignTimeManager.DrawSelection(cache, iargs.ViewInfo.Bounds, Color.Magenta);
			}
		}
		public override void Draw(GraphicsCache cache, ImageGalleryInfoArgs args) {
			base.Draw(cache, args);
			DrawSideControl(cache, args);
			DrawSelection(cache, args as InRibbonImageGalleryInfoArgs);
			DrawReduceOperationSelection(cache, args as InRibbonImageGalleryInfoArgs);
		}		 
	}
	public class StandaloneGalleryItemPainter : GalleryItemPainter {
		protected override string GetImageBackgroundSkinName() { return RibbonSkins.SkinPopupGalleryImageBackground; }
		protected override string GetButtonSkinName() { return RibbonSkins.SkinPopupGalleryPopupButton; }
	}
	public class GalleryControlGalleryItemPainter : StandaloneGalleryItemPainter {
		protected override void DrawSelection(GalleryItemInfoArgs e) {
			GalleryControl galleryControl = ((GalleryControlGallery)e.ViewInfo.Gallery).GalleryControl;
			if(galleryControl.DesignTimeManager.DragItem != e.ViewInfo.Item)
				return;
			galleryControl.DesignTimeManager.DrawSelection(e.Cache, e.ViewInfo.Bounds, Color.Magenta);
		}
		protected override void DrawDropMark(GalleryItemInfoArgs e) {
			GalleryControl galleryControl = ((GalleryControlGallery)e.Gallery).GalleryControl;
			BaseRibbonDesignTimeManager designTimeManager = galleryControl.DesignTimeManager;
			if(designTimeManager.DropSelectedObject == e.ViewInfo.Item)
				new PrimitivesPainter(galleryControl.GetController().PaintStyle).DrawGalleryItemDropMark(e.Graphics, e.ViewInfo, designTimeManager.DropSelectStyle);
		}
	}
	public class PopupGalleryItemPainter : StandaloneGalleryItemPainter {
		protected override string GetImageBackgroundSkinName() { return RibbonSkins.SkinPopupGalleryImageBackground; }
		protected override string GetButtonSkinName() { return RibbonSkins.SkinPopupGalleryPopupButton; }
	}
	public class StandaloneGalleryPainter : GalleryControlPainter {
		protected override GalleryItemPainter CreateItemPainter() {
			return new PopupGalleryItemPainter();
		}
		protected override GalleryItemGroupPainter CreateGroupPainter() {
			return new GalleryItemGroupPainter();
		}
		public virtual SkinElementInfo GetFilterPanelInfo(StandaloneGalleryViewInfo viewInfo) {
			SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinPopupGalleryFilterPanel], viewInfo.FilterAreaBounds);
			res.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			res.ImageIndex = -1;
			res.State = ObjectState.Normal;
			if(viewInfo.HitInfo.InGalleryFilter)
				res.State = ObjectState.Hot;
			if(viewInfo.Gallery.MenuOpened) res.State = ObjectState.Pressed;
			return res;
		}
		public virtual void DrawFilter(GraphicsCache cache, ImageGalleryInfoArgs args) {
			StandaloneGalleryViewInfo viewInfo = args.ViewInfo as StandaloneGalleryViewInfo;
			if(viewInfo == null || !viewInfo.Gallery.AllowFilter) return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetFilterPanelInfo(viewInfo));
			viewInfo.PaintAppearance.FilterPanelCaption.DrawString(cache, viewInfo.GetFilterCaption(), viewInfo.FilterTextBounds);
		}
		public override void DrawSizer(GraphicsCache cache, ImageGalleryInfoArgs args) {
		}
		public override void Draw(GraphicsCache cache, ImageGalleryInfoArgs args) {
			if(args.DrawContent) {
				base.Draw(cache, args);
			}
			if(args.DrawFilter) {
				if(!args.DrawContent)
					DrawBackground(cache, args);
				DrawFilter(cache, args);
			}
		}
	}
	public class InDropDownGalleryPainter : StandaloneGalleryPainter {
		public virtual SkinElementInfo GetSizerCornerInfo(InDropDownGalleryViewInfo viewInfo) {
			SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinPopupGallerySizerGrips], viewInfo.ResizeCornerBounds);
			res.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			res.ImageIndex = 0;
			if(((InDropDownGallery)viewInfo.Gallery).SizeMode == GallerySizeMode.Both) res.ImageIndex = viewInfo.Gallery.SizerBelow ? 1 : 2;
			return res;
		}
		public virtual SkinElementInfo GetSizerPanelInfo(InDropDownGalleryViewInfo viewInfo) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinPopupGallerySizerPanel], viewInfo.ResizeRectBounds);
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			return info;
		}
		public override void DrawSizer(GraphicsCache cache, ImageGalleryInfoArgs args) {
			InDropDownGalleryViewInfo viewInfo = args.ViewInfo as InDropDownGalleryViewInfo;
			if(viewInfo == null || !viewInfo.IsGalleryResizing) return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetSizerPanelInfo(viewInfo));
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetSizerCornerInfo(viewInfo));
		}
	}
	public class GalleryControlItemGroupPainter : GalleryItemGroupPainter {
		protected override void DrawSelection(GraphicsCache cache, GalleryItemGroupViewInfo viewInfo) {
			GalleryControl galleryControl = ((GalleryControlGallery)viewInfo.Gallery).GalleryControl;
			if(galleryControl.DesignTimeManager.DragItem != viewInfo.Group)
				return;
			galleryControl.DesignTimeManager.DrawSelection(cache, viewInfo.Bounds, Color.Magenta);
		}
		protected override void DrawDropMark(GraphicsCache cache, GalleryItemGroupViewInfo viewInfo) {
			GalleryControl galleryControl = ((GalleryControlGallery)viewInfo.Gallery).GalleryControl;
			BaseRibbonDesignTimeManager designTimeManager = galleryControl.DesignTimeManager;
			if(designTimeManager.DropSelectedObject == viewInfo.Group)
				new PrimitivesPainter(galleryControl.GetController().PaintStyle).DrawGalleryItemGroupDropMark(cache.Graphics, viewInfo, designTimeManager.DropSelectStyle);
		}
	}
	public class GalleryControlGalleryPainter : StandaloneGalleryPainter {
		public override SkinElementInfo GetBackgroundInfo(BaseGalleryViewInfo viewInfo) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Gallery.Provider)[RibbonSkins.SkinPopupGalleryBackground], ((GalleryControlGalleryViewInfo)viewInfo).BackgroundRect);
			info.RightToLeft = viewInfo.Gallery.IsRightToLeft;
			return info;
		}
		protected override GalleryItemGroupPainter CreateGroupPainter() {
			return new GalleryControlItemGroupPainter();
		}
		protected override GalleryItemPainter CreateItemPainter() {
			return new GalleryControlGalleryItemPainter();
		}
		protected override void DrawBackground(GraphicsCache cache, ImageGalleryInfoArgs args) {
			GalleryControlGalleryViewInfo galleryInfo = (GalleryControlGalleryViewInfo)args.ViewInfo;
			if(galleryInfo.Gallery.BackColor.Equals(Color.Empty))
				base.DrawBackground(cache, args);
			else {
				cache.FillRectangle(cache.GetSolidBrush(galleryInfo.Gallery.BackColor), galleryInfo.BackgroundRect);
			}
			if(galleryInfo.Gallery.BackgroundImage != null) {
				cache.Graphics.DrawImage(galleryInfo.Gallery.BackgroundImage, galleryInfo.BackgroundImageRect);
			}
		}
	}
}
