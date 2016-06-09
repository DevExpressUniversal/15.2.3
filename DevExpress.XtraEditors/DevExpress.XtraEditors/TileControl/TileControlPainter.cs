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
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.Utils;
namespace DevExpress.XtraEditors {
	public class TileControlPainter {
		protected Rectangle InvalidateRect { get; set; }
		public virtual void Draw(TileControlInfoArgs e) {
			InvalidateRect = Rectangle.Round(e.Cache.Graphics.ClipBounds);
			DrawBackground(e);
			if(e.ViewInfo.Owner.IsDesignMode) {
				Rectangle rect = e.ViewInfo.Bounds;
				rect.Width -= 1;
				rect.Height -= 1;
				e.ViewInfo.DesignTimeManager.DrawSelectionBounds(e.Cache, rect, Color.Red);
			}
			DrawText(e);
			DrawGroups(e);
			DrawScrollArrows(e);
			if(e.ViewInfo.DragItem != null && e.ViewInfo.Owner.Handler.State == TileControlHandlerState.DragMode) {
				DrawItem(e, e.ViewInfo.DragItem);
			}
			DrawDisabledMask(e);
			DrawBorder(e);
		}
		protected virtual void DrawDisabledMask(TileControlInfoArgs e) {
			if(e.ViewInfo.Owner.Enabled || !e.ViewInfo.Owner.AllowDisabledStateIndication)
				return;
			e.Cache.FillRectangle(e.Cache.GetSolidBrush(Color.FromArgb(120, 255, 255, 255)), e.ViewInfo.Bounds);
		}
		protected virtual void DrawText(TileControlInfoArgs e) {
			if(e.ViewInfo.Owner.Properties.ShowText)
				e.ViewInfo.AppearanceText.DrawString(e.Cache, e.ViewInfo.Owner.Text, e.ViewInfo.TextBounds, e.ViewInfo.AppearanceTextStringFormat);
		}
		protected virtual void DrawBorder(TileControlInfoArgs e) {
			e.ViewInfo.BorderPainter.DrawObject(new BorderObjectInfoArgs(e.Cache, e.ViewInfo.PaintAppearance, e.ViewInfo.Bounds));
		}
		protected virtual void DrawScrollArrows(TileControlInfoArgs e) {
			if(e.ViewInfo.ScrollMode == TileControlScrollMode.ScrollButtons && e.ViewInfo.ScrollArrowsCurrentOpacity != .0f) {
				DrawBackArrow(e);
				DrawForwardArrow(e);
			}
		}
		protected virtual void DrawGroups(TileControlInfoArgs e) {
			GraphicsClipState state = e.Cache.ClipInfo.SaveAndSetClip(e.ViewInfo.GroupsClipBounds);
			try {
				bool groupDraw = false;
				foreach(TileGroupViewInfo groupInfo in e.ViewInfo.Groups) {
					if(!groupInfo.IsVisible) {
						if(groupDraw)
							break;
						continue;
					}
					DrawGroup(e, groupInfo, groupDraw);
					groupDraw = true;
				}
				if(e.ViewInfo.ShouldDrawRectangleNewGroup)
					DrawRectangleNewGroup(e, e.ViewInfo.Owner.Properties.AppearanceGroupHighlighting.HoverColorGroup, e.ViewInfo.CalcNewGroupHighlightedBounds);
			} finally {
				e.Cache.ClipInfo.RestoreClip(state);
				state.Dispose();
			}
		}
		protected virtual void DrawGroup(TileControlInfoArgs e, TileGroupViewInfo groupInfo, bool groupDraw) {
			DrawGroupCaption(e, groupInfo);
			DrawItems(e, groupInfo, groupDraw);
			if(e.ViewInfo.ShouldDrawGroupHighlighting(groupInfo))
				DrawHighLightGroup(e, groupInfo);
			if(e.ViewInfo.Owner.IsDesignMode) 
				DrawGroupDesignRectangle(e, groupInfo);
			if(groupInfo.ShouldDrawDragItemPlacement)
				DrawDropItemPlacement(e, groupInfo);
		}
		protected virtual void DrawHighLightGroup(TileControlInfoArgs e, TileGroupViewInfo groupInfo) {
			Point pt = e.ViewInfo.DragItem.DragOrigin;
			DrawHighlight(e, e.ViewInfo.HighLightColor(pt, groupInfo, e), e.ViewInfo.CalcGroupHighlightedRectangle(groupInfo));
		}
		protected virtual void DrawHighlight(TileControlInfoArgs e, Color color, Rectangle rec) {
			e.Cache.FillRectangle(color, rec);
		}
		protected virtual void DrawRectangleNewGroup(TileControlInfoArgs e, Color color, Rectangle rect) {
			foreach(TileGroupViewInfo groupInfo in e.ViewInfo.Groups) {
				if(rect.IntersectsWith(groupInfo.TotalBounds))
					return;
			}
			DrawHighlight(e, color, rect);
		}
		private static void DrawDropItemPlacement(TileControlInfoArgs e, TileGroupViewInfo groupInfo) {
			Rectangle rect = groupInfo.Bounds;
			rect.Inflate(-groupInfo.ControlInfo.DragItemBoundsScaleDelta, -groupInfo.ControlInfo.DragItemBoundsScaleDelta);
			e.Cache.Graphics.DrawImage(groupInfo.ControlInfo.DragItem.RenderImage, rect);
		}
		protected virtual void DrawGroupDesignRectangle(TileControlInfoArgs e, TileGroupViewInfo groupInfo) {
			if(e.ViewInfo.DesignTimeManager.IsComponentSelected(groupInfo.Group))
				e.ViewInfo.DesignTimeManager.DrawSelection(e.Cache, groupInfo.DesignTimeBounds, Color.Magenta);
			else {
				e.ViewInfo.DesignTimeManager.DrawSelectionBounds(e.Cache, groupInfo.DesignTimeBounds, Color.Red);
			}
		}
		protected virtual void DrawItemDesignRectangle(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(e.ViewInfo.DesignTimeManager.IsComponentSelected(e.ViewInfo.GetTileComponent(itemInfo.Item)))
				e.ViewInfo.DesignTimeManager.DrawSelection(e.Cache, itemInfo.Bounds, Color.Magenta);
		}
		protected virtual bool InvalidatedAreaHasItem(TileItemViewInfo itemInfo) {
			return InvalidateRect.IntersectsWith(itemInfo.Bounds);
		}
		protected virtual void DrawItems(TileControlInfoArgs e, TileGroupViewInfo groupInfo, bool groupDraw) {
			int firstInvisibleColumnIndex = -1;
			foreach(TileItemViewInfo itemInfo in groupInfo.Items) {
				if(!itemInfo.ShouldProcessItem || !InvalidatedAreaHasItem(itemInfo)) continue;
				if(!itemInfo.IsVisible) {
					if(groupDraw) {
						if(firstInvisibleColumnIndex == -1)
							firstInvisibleColumnIndex = itemInfo.Position.GroupColumn;
						else if(firstInvisibleColumnIndex < itemInfo.Position.GroupColumn)
							return;
					}
					continue;
				}
				if(!itemInfo.IsDragging)
					DrawItem(e, itemInfo);
			}
		}
		protected virtual void DrawBackArrow(TileControlInfoArgs e) {
			if(e.ViewInfo.IsVertical)
				new RotateObjectPaintHelper().DrawRotated(e.Cache, e.ViewInfo.GetBackArrowInfo(), SkinElementPainter.Default, RotateFlipType.Rotate90FlipNone);
			else
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, e.ViewInfo.GetBackArrowInfo());
		}
		protected virtual void DrawForwardArrow(TileControlInfoArgs e) {
			if(e.ViewInfo.IsVertical)
				new RotateObjectPaintHelper().DrawRotated(e.Cache, e.ViewInfo.GetForwardArrowInfo(), SkinElementPainter.Default, RotateFlipType.Rotate90FlipNone);
			else 
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, e.ViewInfo.GetForwardArrowInfo());
		}
		Rectangle GetClipRectFor(TileControlInfoArgs e, Rectangle rect) {
			Rectangle res = e.ViewInfo.GroupsClipBounds;
			res.Intersect(rect);
			return res;
		}
		protected internal virtual void DrawItemInTransition(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			GraphicsClipState state = e.Cache.ClipInfo.SaveAndSetClip(GetClipRectFor(e,itemInfo.Bounds));
			try {
				e.Cache.ClipInfo.SetClip(GetClipRectFor(e, itemInfo.PrevBounds));
				e.Cache.Graphics.DrawImage(itemInfo.RenderImage, new Rectangle(itemInfo.PrevTransitionBounds.Location, itemInfo.DragStateBounds.Size));
				e.Cache.ClipInfo.SetClip(GetClipRectFor(e, itemInfo.Bounds));
				e.Cache.Graphics.DrawImage(itemInfo.RenderImage, new Rectangle(itemInfo.NextTransitionBounds.Location, itemInfo.DragStateBounds.Size));
			} finally {
				e.Cache.ClipInfo.RestoreClip(state);
				state.Dispose();
			}
		}
		protected internal virtual void DrawItemRenderImage(TileControlInfoArgs e, float opacity, Rectangle bounds, Image image) {
			if(image == null) return;
			if(opacity == 1.0f)
				e.Cache.Graphics.DrawImage(image, bounds);
			else {
				ImageAttributes attr = new ImageAttributes();
				ColorMatrix mat = new ColorMatrix();
				mat.Matrix33 = opacity;
				attr.SetColorMatrix(mat);
				e.Cache.Graphics.DrawImage(image, bounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attr);
			}
		}
		protected internal virtual void DrawItemRenderImageOptimized(TileControlInfoArgs e, Point point, Image image) {
			e.Cache.Graphics.DrawImageUnscaled(image, point);
		}
		protected internal virtual void DrawItemRenderImage(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(itemInfo.RenderImageOpacity == 1 && itemInfo.UseOptimizedRenderImage && itemInfo.OptimizedRenderImage.Size == itemInfo.RenderImageBounds.Size)
				DrawItemRenderImageOptimized(e, itemInfo.RenderImageBounds.Location, itemInfo.OptimizedRenderImage);
			else
				DrawItemRenderImage(e, itemInfo.RenderImageOpacity, itemInfo.RenderImageBounds, itemInfo.RenderImage);
		}
		protected virtual void DrawItemText(TileControlInfoArgs e, TileItemViewInfo itemInfo, TileItemElementViewInfo elemInfo, Point location) {
			string text = elemInfo.Text;
			StringInfo info = elemInfo.StringInfo;
			Rectangle originBounds = elemInfo.TextBounds;
			if(itemInfo.AllowHtmlText) {
				if(info == null)
					return;
				XPaint prev = e.Cache.Paint;
				e.Cache.Paint = TileItemViewInfo.GetGdiPaint(e.ViewInfo);
				try {
					info.SetLocation(location);
					StringPainter.Default.DrawString(e.Cache, info);
				} finally {
					e.Cache.Paint = prev;
				}
			} else {
				if(string.IsNullOrEmpty(text))
					return;
				e.Cache.Graphics.DrawString(text, elemInfo.PaintAppearance.Font, e.Cache.GetSolidBrush(elemInfo.PaintAppearance.ForeColor), new Rectangle(location, originBounds.Size), elemInfo.PaintAppearance.GetStringFormat());
			}
		}
		protected internal virtual void DrawItemContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(itemInfo.CurrentContentAnimationType == TileItemContentAnimationType.Fade || 
				itemInfo.CurrentContentAnimationType == TileItemContentAnimationType.SegmentedFade || 
				itemInfo.CurrentContentAnimationType == TileItemContentAnimationType.RandomSegmentedFade) {
				DrawItemFadeContentChanging(e, itemInfo);
			} else {
				DrawItemScrollContentChanging(e, itemInfo);
			}
		}
		protected virtual void DrawSegment(TileControlInfoArgs e, TileItemViewInfo itemInfo, int x, int y, int index) {
			float opSize = 1.0f / (itemInfo.SegmentColumn * itemInfo.SegmentRow + 10);
			float start = index * opSize;
			float opacity = 0.0f;
			if(itemInfo.RenderImageOpacity > start) {
				opacity = itemInfo.RenderImageOpacity > start + opSize * 10 ? 1.0f : (itemInfo.RenderImageOpacity - start) / (opSize * 10);
			}
			if(opacity == 0.0f)
				return;
			ImageAttributes attr = new ImageAttributes();
			ColorMatrix mat = new ColorMatrix();
			mat.Matrix33 = opacity;
			attr.SetColorMatrix(mat);
			e.Cache.Graphics.DrawImage(itemInfo.RenderImage,
				new Rectangle(itemInfo.Bounds.X + x * itemInfo.SegmentSize, itemInfo.Bounds.Y + y * itemInfo.SegmentSize, itemInfo.SegmentSize, itemInfo.SegmentSize),
				x * itemInfo.SegmentSize, y * itemInfo.SegmentSize, itemInfo.SegmentSize, itemInfo.SegmentSize, GraphicsUnit.Pixel, attr);
		}
		protected virtual void DrawSegmentedBackgroundInContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(itemInfo.AnimateBackgroundImage) {
				if(itemInfo.PrevRenderImage != null)
					e.Cache.Graphics.DrawImage(itemInfo.PrevRenderImage, itemInfo.PrevRenderImageBounds);
				if(itemInfo.CurrentContentAnimationType == TileItemContentAnimationType.SegmentedFade) {
					for(int j = 0; j < itemInfo.SegmentColumn; j++) {
						for(int i = 0; i < itemInfo.SegmentRow; i++) {
							DrawSegment(e, itemInfo, j, i, i * j);
						}
					}
				} else if(itemInfo.CurrentContentAnimationType == TileItemContentAnimationType.RandomSegmentedFade) {
					for(int i = 0; i < itemInfo.RandomSegments.Count; i++) {
						DrawSegment(e, itemInfo, itemInfo.RandomSegments[i].X, itemInfo.RandomSegments[i].Y, i);
					}
				}
			} else {
				e.Cache.Graphics.DrawImage(itemInfo.PrevRenderImage, itemInfo.Bounds);
			}
		}
		protected virtual void DrawItemFadeContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			GraphicsClipState state = e.Cache.ClipInfo.SaveAndSetClip(GetClipRectFor(e, itemInfo.Bounds));
			try {
				if(itemInfo.CurrentContentAnimationType == TileItemContentAnimationType.Fade)
					DrawItemBackgroundInContentChanging(e, itemInfo, itemInfo.RenderImageOpacity);
				else {
					DrawSegmentedBackgroundInContentChanging(e, itemInfo);
				}
			} finally {
				e.Cache.ClipInfo.RestoreClip(state);
				state.Dispose();
			}
		}
		protected internal virtual void DrawItemScrollContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			GraphicsClipState state = e.Cache.ClipInfo.SaveAndSetClip(GetClipRectFor(e, itemInfo.Bounds));
			try {
				DrawItemBackgroundInContentChanging(e, itemInfo);
				DrawItemImagesInContentChanging(e, itemInfo);
				DrawItemTextsInContentChanging(e, itemInfo);
			} finally {
				e.Cache.ClipInfo.RestoreClip(state);
				state.Dispose();
			}
		}
		protected virtual void DrawItemBackgroundInContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			DrawItemBackgroundInContentChanging(e, itemInfo, 1.0f);
		}
		protected virtual void DrawItemBackgroundInContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo, float opacity) {
			if(itemInfo.AnimateBackgroundImage) {
				if(itemInfo.PrevRenderImage != null)
					e.Cache.Graphics.DrawImage(itemInfo.PrevRenderImage, itemInfo.PrevRenderImageBounds);
				if(itemInfo.RenderImage != null)
					DrawItemRenderImage(e, opacity, itemInfo.RenderImageBounds, itemInfo.RenderImage);
			} else {
				e.Cache.Graphics.DrawImage(itemInfo.PrevRenderImage, itemInfo.Bounds);
			}
		}
		protected virtual void CalcOffsets(TileItemViewInfo itemInfo, ref Point offset, ref Point prevOffset) {
			switch(itemInfo.CurrentContentAnimationType) { 
				case TileItemContentAnimationType.Default:
				case TileItemContentAnimationType.ScrollTop:
					prevOffset = new Point(0,itemInfo.PrevRenderImageBounds.Y - itemInfo.Bounds.Y);
					offset = new Point(0, prevOffset.Y + itemInfo.Bounds.Height);	
				break;
				case TileItemContentAnimationType.ScrollDown:
					prevOffset = new Point(0, itemInfo.PrevRenderImageBounds.Y - itemInfo.Bounds.Y);
					offset = new Point(0, prevOffset.Y - itemInfo.Bounds.Height);	
				break;
				case TileItemContentAnimationType.ScrollLeft:
					prevOffset = new Point(itemInfo.PrevRenderImageBounds.X - itemInfo.Bounds.X, 0);
					offset = new Point(prevOffset.X + itemInfo.Bounds.Width, 0);	
				break;
				case TileItemContentAnimationType.ScrollRight:
					prevOffset = new Point(itemInfo.PrevRenderImageBounds.X - itemInfo.Bounds.X, 0);
					offset = new Point(prevOffset.X - itemInfo.Bounds.Width, 0);	
				break;
			}
		}
		bool ShouldDrawText(TileItemViewInfo itemInfo, TileItemElementViewInfo elemInfo) {
			return (itemInfo.AnimateBackgroundImage && !elemInfo.AnimateText) || 
				!itemInfo.AnimateBackgroundImage && !elemInfo.AnimateText && elemInfo.AnimateImage;
		}
		protected virtual void DrawElementText(TileControlInfoArgs e, TileItemViewInfo itemInfo, TileItemElementViewInfo elemInfo, Point offset, bool useTextInTransition) {
			if(elemInfo.TextPrerendered || !useTextInTransition)
				return;
			bool shouldDrawText = ShouldDrawText(itemInfo, elemInfo);
			if(string.IsNullOrEmpty(elemInfo.Text))
				return;
			Point pt = elemInfo.TextBounds.Location;
			var backgroundPt = elemInfo.BackgroundBounds.Location;
			if(elemInfo.AnimateText) {
				pt.Offset(offset);
				backgroundPt.Offset(offset);
			}
			if(elemInfo.CanDrawBackground)
				elemInfo.PaintAppearanceBackground.DrawBackground(e.Cache, new Rectangle(backgroundPt, elemInfo.BackgroundBounds.Size));
			DrawItemText(e, itemInfo, elemInfo, pt);
		}
		protected virtual void DrawItemTextsInContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			Point offset = Point.Empty, prevOffset = Point.Empty;
			CalcOffsets(itemInfo, ref offset, ref prevOffset);
			for(int i = 0; i < itemInfo.PrevItems.Count; i++) {
				TileItemElementViewInfo elemInfo = itemInfo.PrevItems[i];
				DrawElementText(e, itemInfo, elemInfo, prevOffset, elemInfo.Element.UseTextInTransition);
			}
			for(int i = 0; i < itemInfo.Elements.Count; i++) {
				TileItemElementViewInfo elemInfo = itemInfo.Elements[i];
				DrawElementText(e, itemInfo, elemInfo, offset, elemInfo.Element.UseTextInTransition);
			}
		}
		protected virtual void DrawElementImageInContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo, TileItemElementViewInfo elemInfo, Point offset, bool useImageInTransition) {
			if(elemInfo.ImagePrerendered || elemInfo.Image == null)
				return;
			DrawItemImageCore(e, elemInfo, itemInfo.Bounds, offset);
		}
		protected virtual  void DrawItemImagesInContentChanging(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			Point offset = Point.Empty, prevOffset = Point.Empty;
			CalcOffsets(itemInfo, ref offset, ref prevOffset);
			foreach(TileItemElementViewInfo elemInfo in itemInfo.PrevItems) {
				DrawElementImageInContentChanging(e, itemInfo, elemInfo, prevOffset, elemInfo.Element.UseImageInTransition);
			}
			foreach(TileItemElementViewInfo elemInfo in itemInfo.Elements) {
				DrawElementImageInContentChanging(e, itemInfo, elemInfo, offset, elemInfo.Element.UseImageInTransition);
			}
		}
		protected internal virtual void DrawItem(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(itemInfo.IsHoveringAnimation) {
				DrawItemHovering(e, itemInfo);
				DrawItemCheck(e, itemInfo);
				DrawContextButtons(e, itemInfo);
			}
			else if(itemInfo.IsInTransition) {
				DrawItemInTransition(e, itemInfo);
			} else if(itemInfo.IsChangingContent) {
				if(ItemIsVisibleAfterClipping(e, itemInfo))
					DrawItemContentChanging(e, itemInfo);
				DrawItemCheck(e, itemInfo);
				DrawContextButtons(e, itemInfo);
			} else if(itemInfo.UseRenderImage) {
				DrawItemRenderImage(e, itemInfo);
			} else {
				GraphicsClipState state = itemInfo.RenderToBitmap ? null : e.Cache.ClipInfo.SaveAndSetClip(GetClipRectFor(e, itemInfo.Bounds));
				try {
					if(itemInfo.DrawOnlyText) {
						DrawItemElementsBackground(e, itemInfo);
						if(!string.IsNullOrEmpty(itemInfo.Item.Text))
							DrawItemTexts(e, itemInfo);
						return;
					}
					if(itemInfo.DrawBackgroundImage) {
						DrawItemBackground(e, itemInfo);
						if(itemInfo.Item.BackgroundImage != null)
							DrawItemBackgroundImage(e, itemInfo);
					}
					if(!itemInfo.RenderToBitmap) {
						e.Cache.ClipInfo.RestoreClip(state);
						state.Dispose();
					}
					DrawSelected(e, itemInfo);
					if(itemInfo.DrawOnlyBackgroundImage) {
						DrawHover(e, itemInfo);
						return;
					}
					state = itemInfo.RenderToBitmap ? null : e.Cache.ClipInfo.SaveAndSetClip(GetClipRectFor(e, itemInfo.Bounds));
					if(itemInfo.DrawImage)
						DrawItemImage(e, itemInfo);
					if(itemInfo.DrawText) {
						DrawItemElementsBackground(e, itemInfo);
						DrawItemTexts(e, itemInfo);
					}
					DrawHover(e, itemInfo);
					DrawDropDownButton(e, itemInfo);
					if(itemInfo.IsKeyboardSelected) {
						if(!itemInfo.RenderToBitmap) {
							e.Cache.ClipInfo.RestoreClip(state);
							state.Dispose();
						}
						DrawItemSelection(e, itemInfo);
					}
					if(!itemInfo.Item.Enabled)
						DrawTransparentLayer(e, itemInfo);
					if(!itemInfo.RenderToBitmap) {
						DrawContextButtons(e, itemInfo);
					}
				} finally {
					if(state != null && state.ClipRegion != null) {
						e.Cache.ClipInfo.RestoreClip(state);
						state.Dispose();
					}
					if(itemInfo.DrawBackgroundImage)
						DrawItemBorder(e, itemInfo);
					if(!itemInfo.DrawOnlyText && !itemInfo.DrawOnlyBackgroundImage)
						DrawItemCheck(e, itemInfo);
					if(e.ViewInfo.Owner.IsDesignMode) {
						DrawItemDesignRectangle(e, itemInfo);
					}
				}
			}
		}
		protected virtual void DrawContextButtons(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if((itemInfo as ISupportContextItems).ContextItems != null)
				new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(itemInfo.ContextButtonsViewInfo, e.Cache, itemInfo.Bounds));
		}
		protected virtual bool ItemIsVisibleAfterClipping(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			var clipRect = GetClipRectFor(e, itemInfo.Bounds);
			return clipRect.Width != 0 && clipRect.Height != 0;
		}
		protected internal virtual void DrawDropDownButton(TileControlInfoArgs e, TileItemViewInfo itemInfo) { }
		protected virtual void DrawItemHovering(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			DrawItemRenderImage(e, 1.0f, itemInfo.Bounds, itemInfo.PrevRenderImage);
			DrawItemRenderImage(e, itemInfo.IsHovered? itemInfo.HoverOpacity: 1.0f - itemInfo.HoverOpacity, itemInfo.Bounds, itemInfo.RenderImage);
			DrawSelected(e, itemInfo);
		}
		protected virtual void DrawSelected(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(!itemInfo.IsSelected)
				return;
			SkinElementInfo info = itemInfo.GetItemSelectedInfo();
			if(info != null) {
				GraphicsClipState state = e.Cache.ClipInfo.SaveAndSetClip(GetClipRectFor(e,info.Bounds));
				try {
					ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
				}
				finally {
					e.Cache.ClipInfo.RestoreClip(state);
					state.Dispose();
				}
			}
			else e.Cache.FillRectangle(e.Cache.GetSolidBrush(itemInfo.SelectedColor), itemInfo.Bounds);
		}
		protected virtual void DrawHover(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(!itemInfo.CanDrawHoverOverlay) return;
			SkinElementInfo info = itemInfo.GetItemHoveredInfo();
			if(info != null) 
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			else 
				e.Cache.FillRectangle(e.Cache.GetSolidBrush(itemInfo.HoverColor), itemInfo.Bounds);
		}
		protected virtual void DrawItemCheck(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(!itemInfo.Item.Checked || !itemInfo.AllowItemCheck)
				return;
			SkinElementInfo info = itemInfo.GetItemCheckedInfo();
			if(info != null) {
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			} else {
				Pen checkPen = e.Cache.GetPen(e.ViewInfo.CheckBorderColor, e.ViewInfo.CheckBorderWidth);
				Rectangle rect = itemInfo.Bounds;
				rect.Inflate(-e.ViewInfo.CheckBorderWidth / 2, -e.ViewInfo.CheckBorderWidth / 2);
				e.Cache.Graphics.DrawRectangle(checkPen, rect);
			}
		}
		protected virtual void DrawItemTexts(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(!itemInfo.GroupInfo.ControlInfo.Owner.IsDesignMode && itemInfo.TextShowMode != TileItemContentShowMode.Always && !itemInfo.IsHovered && itemInfo.HoverOpacity == 0.0f)
				return;
			if(itemInfo.AllowHtmlText)
				DrawItemHtmlTexts(e, itemInfo);
			else
				DrawItemSimpleTexts(e, itemInfo);
		}
		protected virtual void DrawItemHtmlTexts(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			XPaint prev = e.Cache.Paint;
			e.Cache.Paint = TileItemViewInfo.GetGdiPaint(e.ViewInfo);
			try {
				foreach(TileItemElementViewInfo info in itemInfo.Elements) {
					DrawItemHtmlTextCore(e, info);
				}
			} finally {
				e.Cache.Paint = prev;
			}
		}
		protected virtual void DrawItemHtmlTextCore(TileControlInfoArgs e, TileItemElementViewInfo info) {
			if(info != null && info.StringInfo != null)
				StringPainter.Default.DrawString(e.Cache, info.StringInfo);
		}
		protected virtual void DrawItemSimpleTexts(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			DrawItemSimpleTextsBackground(e, itemInfo);
			foreach(TileItemElementViewInfo elementInfo in itemInfo.Elements)
				DrawItemSimpleTextCore(e, elementInfo);
		}
		protected virtual void DrawItemSimpleTextCore(TileControlInfoArgs e, TileItemElementViewInfo elementInfo) {
			e.Cache.Graphics.DrawString(elementInfo.Text, elementInfo.PaintAppearance.Font, e.Cache.GetSolidBrush(elementInfo.PaintAppearance.ForeColor), elementInfo.TextBounds, elementInfo.PaintAppearance.GetStringFormat());
		}
		protected virtual void DrawItemSimpleTextsBackground(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			foreach(TileItemElementViewInfo vi in itemInfo.Elements) {
				if(vi.ShouldDrawSimpleTextBackground) e.Cache.FillRectangle(e.Cache.GetSolidBrush(vi.PaintAppearance.BackColor), vi.TextBounds);
			}
		}
		protected virtual void DrawItemSelection(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			e.Cache.DrawRectangle(e.Cache.GetPen(itemInfo.GetSelectionColor(), e.ViewInfo.SelectionWidth), itemInfo.SelectionBounds);
		}
		protected virtual void DrawItemBorder(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(itemInfo.ShouldDrawBorder)
				e.Cache.DrawRectangle(itemInfo.PaintAppearance.GetBorderPen(e.Cache), itemInfo.Bounds);
		}
		protected virtual void DrawItemBackground(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			AppearanceObject currentAppearance;
			currentAppearance = itemInfo.PaintAppearance;
			currentAppearance.DrawBackground(e.Cache, itemInfo.Bounds);
		}
		protected virtual void DrawTransparentLayer(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			itemInfo.PaintAppearanceForDisabledItems.DrawBackground(e.Cache, itemInfo.Bounds);
		}
		protected virtual void DrawItemImage(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			foreach(TileItemElementViewInfo elemInfo in itemInfo.Elements) {
				if(elemInfo.Image == null) continue;
				DrawItemImageCore(e, elemInfo, itemInfo.Bounds, Point.Empty);
			}
		}
		protected virtual void DrawItemImageCore(TileControlInfoArgs e, TileItemElementViewInfo elemInfo, Rectangle itemBounds, Point offset) {
			GraphicsClipState state = null;
			Rectangle bounds = elemInfo.ImageContentBounds;
			if(elemInfo.AnimateImage) {
				bounds.Y += offset.Y;
				bounds.X += offset.X;
			}
			Rectangle border = bounds;
			if(elemInfo.ImageNotCroppedSize != Size.Empty) {
				state = e.Cache.ClipInfo.SaveAndSetClip(GetClipRectFor(e,IntersectRectangles(itemBounds, bounds)));
				bounds = new Rectangle(GetCenterImageLocation(elemInfo, bounds), elemInfo.ImageNotCroppedSize);
			}
			DrawElementImageCore(e, elemInfo, bounds);
			if(state != null) {
				e.Cache.ClipInfo.RestoreClip(state);
				state.Dispose();
			}
			DrawElementImageBorder(e, elemInfo, border);
		}
		protected virtual void DrawElementImageBorder(TileControlInfoArgs e, TileItemElementViewInfo elemInfo, Rectangle bounds) {
			if(!elemInfo.ShouldDrawImageBorder)
				return;
			Pen pen = elemInfo.ImageBorderPen;
			if(elemInfo.UseAppearanceForImageBorder)
				pen = elemInfo.ItemInfo.PaintAppearance.GetBorderPen(e.Cache);
			e.Cache.Graphics.DrawRectangle(pen, bounds);
		}
		protected virtual void DrawElementImageCore(TileControlInfoArgs e, TileItemElementViewInfo elemInfo, Rectangle bounds) {
			if(elemInfo.AllowGlyphSkinning)
				DrawElementImageColorized(e, elemInfo, bounds);
			else {
				if(elemInfo.UseOptimizedImage)
					e.Cache.Graphics.DrawImageUnscaled(elemInfo.Element.OptimizedImage, bounds.Location);
				else
					e.Cache.Graphics.DrawImage(elemInfo.Image, bounds);
			}
		}
		protected virtual void DrawElementImageColorized(TileControlInfoArgs e, TileItemElementViewInfo elemInfo, Rectangle bounds) {
			ImageAttributes attr = ImageColorizer.GetColoredAttributes(elemInfo.PaintAppearance.ForeColor);
			DrawImageColorizedCore(e, elemInfo.Image, bounds, attr);
		}
		protected virtual void DrawImageColorizedCore(TileControlInfoArgs e, Image image, Rectangle bounds, ImageAttributes attr) {
			e.Cache.Graphics.DrawImage(image, bounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attr);
		}
		protected virtual void DrawItemElementsBackground(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			foreach(TileItemElementViewInfo elemInfo in itemInfo.Elements) {
				if(!elemInfo.CanDrawBackground)
					continue;
				elemInfo.PaintAppearanceBackground.DrawBackground(e.Cache, elemInfo.BackgroundBounds);
			}
		}
		Point GetCenterImageLocation(TileItemElementViewInfo elemInfo, Rectangle croppedRect) {
			Point point = croppedRect.Location;
			if (elemInfo.Element.ImageScaleMode != TileItemImageScaleMode.ZoomOutside) 
				return point;
			int dx = (int)(elemInfo.ImageNotCroppedSize.Width - croppedRect.Width) / 2;
			int dy = (int)(elemInfo.ImageNotCroppedSize.Height - croppedRect.Height) / 2;
			if(dx > 0)
				point = new Point(point.X - dx, point.Y);
			if(dy > 0)
				point = new Point(point.X, point.Y - dy);
			return point;
		}
		Rectangle IntersectRectangles(Rectangle rect1, Rectangle rect2) {
			rect1.Intersect(rect2);
			return rect1;
		}
		protected virtual void DrawItemBackgroundImage(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			GraphicsClipState state = itemInfo.RenderToBitmap? null: e.Cache.ClipInfo.SaveAndSetClip(GetClipRectFor(e, itemInfo.Bounds));
			try {
				if(itemInfo.UseOptimizedBackgroundImage)
					e.Cache.Graphics.DrawImageUnscaled(itemInfo.Item.OptimizedBackgroundImage, itemInfo.BackgroundImageContentBounds.Location);
				else
					e.Cache.Graphics.DrawImage(itemInfo.Item.BackgroundImage, itemInfo.BackgroundImageContentBounds);
			} finally {
				if(state != null) {
					e.Cache.ClipInfo.RestoreClip(state);
					state.Dispose();
				}
			}
		}
		protected virtual void DrawGroupCaption(TileControlInfoArgs e, TileGroupViewInfo groupInfo) {
			if(groupInfo.CanDrawCaption)
				e.ViewInfo.AppearanceGroupText.DrawString(e.Cache, groupInfo.Group.Text, groupInfo.TextBounds);
		}
		protected virtual void DrawBackground(TileControlInfoArgs e) {
			if(e.ViewInfo.Owner.BackgroundImage != null)
				DrawBackgroundImage(e);
			else e.Cache.FillRectangle(e.ViewInfo.PaintAppearance.BackColor, e.ViewInfo.Bounds);
		}
		protected virtual void DrawBackgroundImage(TileControlInfoArgs e) {
			ImageLayout bil = e.ViewInfo.Owner.Properties.BackgroundImageLayout;
			if(bil == ImageLayout.Tile)
				DrawBackgroundImageCore(e);
			else if(bil == ImageLayout.Stretch || bil == ImageLayout.Zoom) {
				if(bil == ImageLayout.Zoom) e.Cache.FillRectangle(e.ViewInfo.PaintAppearance.BackColor, e.ViewInfo.Bounds);
				e.Cache.Graphics.DrawImageUnscaled(e.ViewInfo.BackgroundImageStretched, e.ViewInfo.GetBackgroundImageLocation(e.ViewInfo.BackgroundImageStretched.Size));
			}
		}
		protected virtual void DrawBackgroundImageCore(TileControlInfoArgs e) {
			Point loc = e.ViewInfo.BackgroundImageStartPoint;
			for(; loc.Y < e.ViewInfo.Bounds.Bottom; loc.Y += e.ViewInfo.Owner.BackgroundImage.Height) {
				loc.X = e.ViewInfo.BackgroundImageStartPoint.X;
				for(; loc.X < e.ViewInfo.Bounds.Right; loc.X += e.ViewInfo.Owner.BackgroundImage.Width) {
					Rectangle origin = new Rectangle(loc, e.ViewInfo.Owner.BackgroundImage.Size);
					Rectangle dest = origin;
					dest.Intersect(e.ViewInfo.Bounds);
					e.Cache.Graphics.DrawImage(e.ViewInfo.Owner.BackgroundImage, dest, dest.X - origin.X, 0, dest.Width, dest.Height, GraphicsUnit.Pixel);
				}
			}
		}
	}
	public class TileControlInfoArgs : EventArgs {
		public TileControlInfoArgs(GraphicsCache cache, TileControlViewInfo viewInfo) {
			ViewInfo = viewInfo;
			Cache = cache;
		}
		public GraphicsCache Cache { get; private set; }
		public TileControlViewInfo ViewInfo { get; private set; }
	}
}
