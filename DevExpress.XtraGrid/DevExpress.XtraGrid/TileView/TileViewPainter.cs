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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
namespace DevExpress.XtraGrid.Views.Tile.Drawing {
	public class TileViewPainter : BaseViewPainter {
		public TileViewPainter(BaseView view) : base(view) { }
		public override void Draw(ViewDrawArgs e) {
			var iTileControl = e.ViewInfo as ITileControl;
			if(iTileControl.AnimateArrival && !e.ViewInfo.View.IsDesignMode) {
				iTileControl.AnimateArrival = false;
				(iTileControl.ViewInfo as TileViewInfoCore).DoAnimateArrival();
				iTileControl.Invalidate(e.ViewInfo.View.ViewRect);
				return;
			}
			iTileControl.SourcePainter.Draw(new TileControlInfoArgs(e.Cache, iTileControl.ViewInfo));
		}
	}
	public class TileViewPainterCore : TileControlPainter {
		protected override void DrawBackground(TileControlInfoArgs e) {
			if(e.ViewInfo.Owner.BackgroundImage != null)
				DrawBackgroundImage(e);
			else {
				var vi = e.ViewInfo as TileViewInfoCore;
				vi.AppearanceEmptySpace.DrawBackground(e.Cache, e.ViewInfo.Bounds);
			}
		}
		protected override void DrawItemSelection(TileControlInfoArgs e, TileItemViewInfo itemInfo) { 
		}
		protected override void DrawBorder(TileControlInfoArgs e) {
			TileItemViewInfo focusItemInfo = null;
			var viewInfoCore = e.ViewInfo as TileViewInfoCore;
			if(viewInfoCore.View.CanHighlightFocusedItem) {
				var focusItem = viewInfoCore.GetFocusedItem();
				if(focusItem != null)
					focusItemInfo = focusItem.ItemInfo;
			}
			if(focusItemInfo == null) {
				base.DrawBorder(e);
				return;
			}
			GraphicsClipState state = null;
			if(RectangleIsOutsideVertically(e.ViewInfo.GroupsClipBounds, focusItemInfo.Bounds))
				state = e.Cache.ClipInfo.SaveAndSetClip(e.ViewInfo.GroupsClipBounds);
			try {
				base.DrawItemSelection(e, focusItemInfo);
			}
			finally {
				if(state != null) {
					e.Cache.ClipInfo.RestoreClip(state);
					state.Dispose();
				}
			}
			base.DrawBorder(e);
		}
		bool RectangleIsOutsideVertically(Rectangle mainRect, Rectangle checkedRect) {
			if(checkedRect.Y < mainRect.Y ||
			checkedRect.Bottom > mainRect.Bottom)
				return true;
			return false;
		}
		protected override void DrawItemBackgroundImage(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(ShouldDrawAsyncImageArrive(e.ViewInfo as TileViewInfoCore, itemInfo)) {
				ImageLoaderPaintHelper.DrawRenderImage(e.Cache.Graphics, itemInfo as TileViewItemInfo, itemInfo.PaintAppearance.GetBackColor(), itemInfo.Item.Enabled);
			}
			else {
				SelectActiveFrame(e.ViewInfo as TileViewInfoCore, itemInfo);
				base.DrawItemBackgroundImage(e, itemInfo);
			}
		}
		protected void SelectActiveFrame(TileViewInfoCore viewInfo, TileItemViewInfo itemInfo) {
			if(viewInfo == null || 
				viewInfo.View == null || 
				!viewInfo.View.OptionsImageLoad.AsyncLoad) return;
			ImageLoadInfo imgLoadInfo = (itemInfo.Item as TileViewItem).ImageInfo;
			var obj = viewInfo.View.ImageLoader as ISupportXtraAnimation;
			var objId = imgLoadInfo.AnimationObject;
			var info = XtraAnimator.Current.Get(obj, objId);
			if(info != null && imgLoadInfo.IsLoadingImage) 
				itemInfo.Item.BackgroundImage.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
		}
		protected bool ShouldDrawAsyncImageArrive(TileViewInfoCore viewInfo, TileItemViewInfo itemInfo) {
			if(viewInfo == null || viewInfo.View == null || !(viewInfo.View.ImageLoader is AsyncImageLoader)) return false;
			TileViewItem item = itemInfo.Item as TileViewItem;
			return item != null && 
				item.ImageInfo.ThumbImage != null && 
				item.ImageInfo.RenderImageInfo != null && 
				(item.ImageInfo.IsInAnimation && !item.ImageInfo.IsAnimationEnd);
		}
		protected override void DrawGroups(TileControlInfoArgs e) {
			base.DrawGroups(e);
			DrawEmptyArea(e);
		}
		protected virtual void DrawEmptyArea(TileControlInfoArgs e) {
			var viewInfoCore = e.ViewInfo as TileViewInfoCore;
			if(viewInfoCore.View.DataController.VisibleCount == 0) { 
				viewInfoCore.View.RaiseCustomDrawEmptyForeground(
					new CustomDrawEventArgs(e.Cache, viewInfoCore.ClientBounds, viewInfoCore.AppearanceEmptySpace));
			}
		}
	}
	class TileViewDesignTimePainter : TileViewPainterCore {
		protected override void DrawItemHtmlTextCore(TileControlInfoArgs e, TileItemElementViewInfo info) {
			var element = info.Element as TileViewItemElement;
			if(element != null && element.DrawAsField && !element.Column.Visible) return;
			base.DrawItemHtmlTextCore(e, info);
			DrawFieldDesignRect(e, info);
		}
		protected override void DrawItemSimpleTextCore(TileControlInfoArgs e, TileItemElementViewInfo elementInfo) {
			var element = elementInfo.Element as TileViewItemElement;
			if(element != null && element.DrawAsField && !element.Column.Visible) return;
			base.DrawItemSimpleTextCore(e, elementInfo);
			DrawFieldDesignRect(e, elementInfo);
		}
		protected virtual void DrawFieldDesignRect(TileControlInfoArgs e, TileItemElementViewInfo info) {
			var element = info.Element as TileViewItemElement;
			if(element == null || !element.DrawAsField) return;
			if(!element.DrawAsImageField)
				DrawFieldBindingIcon(e, info);
		}
		protected virtual void DrawFieldBindingIcon(TileControlInfoArgs e, TileItemElementViewInfo info) {
			var elemInfo = info as TileViewElementInfo;
			if(elemInfo == null) return;
			e.Cache.Graphics.DrawImageUnscaled(TileViewElementInfo.ImgBindingField, elemInfo.GetBindingIconRectangle().Location);
		}
		protected override void DrawElementImageCore(TileControlInfoArgs e, TileItemElementViewInfo elemInfo, Rectangle bounds) {
			var element = elemInfo.Element as TileViewItemElement;
			if(element == null || !element.DrawAsImageField) {
				base.DrawElementImageCore(e, elemInfo, bounds);
				return;
			}
			var imgSize = TileViewElementInfo.ImgBindingImage.Size;
			int x = bounds.Left + (bounds.Width / 2) - (imgSize.Width / 2);
			int y = bounds.Top + (bounds.Height / 2) - (imgSize.Height / 2);
			e.Cache.Graphics.DrawImageUnscaled(elemInfo.Image, new Point(x, y));
		}
	}
}
