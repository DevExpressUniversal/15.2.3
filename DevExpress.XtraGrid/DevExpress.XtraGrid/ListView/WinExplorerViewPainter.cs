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

using System.Diagnostics;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using System.Drawing.Imaging;
using System;
using DevExpress.Skins;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraGrid.Views.WinExplorer.Drawing {
	public class WinExplorerViewPainter : BaseViewPainter {
		public WinExplorerViewPainter(BaseView view) : base(view) { }
		public override void Draw(ViewDrawArgs e) {
			WinExplorerViewInfo viewInfo = (WinExplorerViewInfo)e.ViewInfo;
			base.Draw(e);
			if(e.ViewInfo.View.GridControl.BackgroundImage == null)
				DrawBackground(e, viewInfo.Bounds);
			DrawBorder(e, viewInfo.Bounds);
			Rectangle clipRect = new Rectangle((int)e.Cache.Graphics.ClipBounds.X, (int)e.Cache.Graphics.ClipBounds.Y, (int)e.Cache.Graphics.ClipBounds.Width, (int)e.Cache.Graphics.ClipBounds.Height);
			clipRect.Intersect(viewInfo.ClientBounds);
			GraphicsClipState clipState = e.Cache.ClipInfo.SaveAndSetClip(clipRect, true, true);
			DrawIcons(e);
			if(viewInfo.DrawMarqueeSelection)
				DrawSelection(e);
			DrawEmptyArea(e);
			e.Cache.ClipInfo.RestoreClipRelease(clipState);
			viewInfo.WinExplorerView.SkipScrollingUntilPaint = false;
		}
		protected virtual void DrawEmptyArea(ViewDrawArgs e) {
			AppearanceObject appearance = e.ViewInfo.PaintAppearance.GetAppearance("EmptySpace");
		   if(View.DataController.VisibleCount == 0) {
			   View.RaiseCustomDrawEmptyForeground(new CustomDrawEventArgs(e.Cache,((WinExplorerViewInfo)e.ViewInfo).ClientBounds, appearance));
		   }
		}
		protected virtual void DrawBackground(ViewDrawArgs e, Rectangle bounds) {
			e.Graphics.FillRectangle(SystemBrushes.Window, bounds);
		}
		protected bool IsRectangleContainsRectangle(Rectangle rect1, Rectangle rect2) {
			return rect2.X >= rect1.X && rect2.Right <= rect1.Right && rect2.Y >= rect1.Y && rect2.Bottom <= rect1.Bottom;
		}
		protected virtual void DrawSelection(ViewDrawArgs e) {
		}
		protected virtual void DrawIcons(ViewDrawArgs e) {
			WinExplorerViewInfo viewInfo = (WinExplorerViewInfo)e.ViewInfo;
			int renderedItemsCount = 0;
			foreach(WinExplorerItemViewInfo itemInfo in viewInfo.VisibleItems) {
				if(!itemInfo.IsVisible)
					continue;
				if(!e.Cache.IsNeedDrawRect(itemInfo.Bounds))
					continue;
				if(itemInfo.IsGroupItem)
					DrawGroup(e, itemInfo);
				else
					DrawItem(e, itemInfo);
				renderedItemsCount++;
			}
		}
		protected virtual void DrawGroup(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			WinExplorerGroupViewInfo groupInfo = (WinExplorerGroupViewInfo)itemInfo;
			WinExplorerViewCustomDrawGroupItemEventArgs args = new WinExplorerViewCustomDrawGroupItemEventArgs(e, groupInfo);
			View.RaiseCustomDrawGroupItem(args);
			if (args.Handled)
				return;
			DrawGroupBackground(e, groupInfo);
			DrawGroupCaptionLine(e, groupInfo);
			if(groupInfo.ViewInfo.ShowExpandCollapseButtons)
				DrawGroupCaptionButton(e, groupInfo);
			if(groupInfo.ViewInfo.ShowCheckBoxInGroupCaption)
				DrawGroupCaptionCheckBox(e, groupInfo);
			DrawGroupCaption(e, groupInfo);
		}
		protected internal virtual void DrawGroupCaptionLine(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			LayoutTextIfNeeded(e, groupInfo);
			DrawGroupCaptionLineCore(e, groupInfo);
		}
		protected virtual void DrawGroupCaptionLineCore(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
		}
		protected internal virtual void DrawGroupCaptionButton(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			e.Graphics.DrawRectangle(Pens.Red, groupInfo.CaptionButtonBounds);
		}
		protected internal virtual void DrawGroupCaptionCheckBox(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
		}
		protected virtual void LayoutTextIfNeeded(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			if(groupInfo.ShouldLayoutText) {
				groupInfo.ShouldLayoutText = false;
				groupInfo.LayoutText(groupInfo.TextClientRect, e.Graphics);
				groupInfo.CalcTextInfo(e.Graphics);
			}
		}
		protected internal virtual void DrawGroupCaption(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			WinExplorerViewInfo vi = (WinExplorerViewInfo)e.ViewInfo;
			LayoutTextIfNeeded(e, groupInfo);			
			if(vi.WinExplorerView.OptionsView.AllowHtmlText) {
				if(groupInfo.TextInfo == null)
					groupInfo.CalcTextInfo(e.Cache.Graphics);
				groupInfo.TextInfo.UpdateAppearanceColors(vi.GetGroupCaptionAppearance(groupInfo));
				StringPainter.Default.DrawString(e.Cache, groupInfo.TextInfo);
			}
			else {
				vi.GetGroupCaptionAppearance(groupInfo).DrawString(e.Cache, groupInfo.Text, groupInfo.TextBounds);
			}
		}
		protected internal virtual void DrawGroupBackground(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			if(groupInfo.IsPressed)
				e.Graphics.FillRectangle(e.ViewInfo.PaintAppearance.GetAppearance("GroupPressed").GetBackBrush(e.Cache), groupInfo.Bounds);
			else if(groupInfo.IsHovered)
				e.Graphics.FillRectangle(e.ViewInfo.PaintAppearance.GetAppearance("GroupHovered").GetBackBrush(e.Cache), groupInfo.Bounds);
			else
				e.Graphics.FillRectangle(e.ViewInfo.PaintAppearance.GetAppearance("GroupNormal").GetBackBrush(e.Cache), groupInfo.Bounds);
		}
		protected virtual void DrawFocusedRect(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
		}
		public virtual void DrawItem(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			WinExplorerViewInfo vi = (WinExplorerViewInfo)e.ViewInfo;
			if(itemInfo.NeedDrawFocusedRect)
				DrawFocusedRect(e, itemInfo);
			WinExplorerViewCustomDrawItemEventArgs args = new WinExplorerViewCustomDrawItemEventArgs(e, itemInfo) { IsAnimated = IsItemAnimated(vi, itemInfo) };
			View.RaiseCustomDrawItem(args);
			if(args.Handled)
				return;
			if(!itemInfo.IsHovered || itemInfo.IsSelected || vi.ItemHoverBordersShowMode != XtraGrid.WinExplorer.ItemHoverBordersShowMode.ContextButtons)
				DrawItemBackground(e, itemInfo);
			DrawItemImage(e, itemInfo);
			if(itemInfo.AllowDrawCheckBox)
				DrawItemCheck(e, itemInfo);
			DrawItemText(e, itemInfo, args);
			DrawItemSeparator(e, itemInfo);
			DrawContextButtons(e, itemInfo);
		}
		private bool IsItemAnimated(WinExplorerViewInfo vi, WinExplorerItemViewInfo itemInfo) {
			if(!ShouldDrawRenderImage(vi, itemInfo))
				return false;
			return itemInfo.ImageInfo.IsInAnimation;
		}
		protected internal virtual void DrawContextButtons(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			itemInfo.CheckContextButtonsLayout();
			ContextItemCollectionInfoArgs info = new ContextItemCollectionInfoArgs(itemInfo.ContextButtonsViewInfo, e.Cache, itemInfo.ImageBounds) { SuppressDrawAutoItems = itemInfo.ViewInfo.SuppressDrawContextButtons };
			new ContextItemCollectionPainter().Draw(info);
		}
		protected internal virtual void DrawItemCheck(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
		}
		protected internal virtual void DrawItemBackground(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
		}
		protected internal virtual void DrawItemText(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo, WinExplorerViewCustomDrawItemEventArgs args) {
			WinExplorerViewInfo vi = (WinExplorerViewInfo)e.ViewInfo;
			if(vi.WinExplorerView.OptionsView.AllowHtmlText) {
				if(itemInfo.TextInfo == null)
					itemInfo.CalcTextInfo(e.Cache.Graphics);
				itemInfo.TextInfo.UpdateAppearanceColors(args.AppearanceText);
				StringPainter.Default.DrawString(e.Cache, itemInfo.TextInfo);
				if(itemInfo.AllowDescription) {
					if(itemInfo.DescriptionInfo == null)
						itemInfo.CalcDescriptionInfo(e.Cache.Graphics);
					itemInfo.DescriptionInfo.UpdateAppearanceColors(args.AppearanceDescription);
					StringPainter.Default.DrawString(e.Cache, itemInfo.DescriptionInfo);
				}
			}
			else {
				args.AppearanceText.DrawString(e.Cache, itemInfo.Text, itemInfo.TextBounds);
				if (itemInfo.AllowDescription)
					args.AppearanceDescription.DrawString(e.Cache, itemInfo.Description, itemInfo.DescriptionBounds);
			}
		}
		protected internal virtual void DrawItemImage(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			WinExplorerViewInfo viewInfo = (WinExplorerViewInfo)e.ViewInfo;
			if(itemInfo.Image == null)
				return;
			RectangleF clipBounds = e.Graphics.ClipBounds;
			bool isClipped = false;
			if(itemInfo.ImageContentBounds.Width > itemInfo.ImageBounds.Width || itemInfo.ImageContentBounds.Height > itemInfo.ImageBounds.Height) {
				isClipped = true;
				e.Graphics.SetClip(itemInfo.ImageBounds);
			}
			if(ShouldDrawRenderImage(viewInfo, itemInfo)) {
				isClipped = true;
				e.Graphics.SetClip(itemInfo.GetInvalidateRectangle(false));
				ImageLoaderPaintHelper.DrawRenderImage(e.Graphics, itemInfo, GetBackColor(), itemInfo.IsEnabled);
			}
			else {
				SelectActiveFrame(viewInfo, itemInfo);
				e.Cache.Paint.DrawImage(e.Graphics, itemInfo.Image, itemInfo.ImageContentBounds, new Rectangle(Point.Empty, itemInfo.Image.Size), itemInfo.IsEnabled);
			}
			if(isClipped) {
				e.Graphics.SetClip(clipBounds);
			}
		}
		protected void SelectActiveFrame(WinExplorerViewInfo viewInfo, WinExplorerItemViewInfo itemInfo) {
			if(viewInfo == null || viewInfo.WinExplorerView == null || !viewInfo.WinExplorerView.OptionsImageLoad.AsyncLoad) return;
			BaseAnimationInfo info = XtraAnimator.Current.Get(viewInfo.ImageLoader as ISupportXtraAnimation, itemInfo.ImageInfo.AnimationObject);
			if(info != null) itemInfo.Image.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
		}
		protected bool ShouldDrawRenderImage(WinExplorerViewInfo viewInfo, WinExplorerItemViewInfo itemInfo) {
			if(viewInfo == null || viewInfo.WinExplorerView == null || !(viewInfo.ImageLoader is AsyncImageLoader)) return false;
			return itemInfo.ImageInfo.ThumbImage != null && itemInfo.ImageInfo.RenderImageInfo != null;
		}
		protected Color GetBackColor() {
			return GridSkins.GetSkin(View).GetSystemColor(SystemColors.Window);
		}
		protected internal virtual void DrawItemSeparator(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
		}
		public new WinExplorerView View {
			get {
				return base.View as WinExplorerView;
			}
		}
	}
}
