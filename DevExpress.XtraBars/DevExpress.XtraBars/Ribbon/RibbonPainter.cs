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
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils.Drawing.Helpers;
using System.Drawing.Imaging;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.Utils.Text;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Utils.Text.Internal;
using DevExpress.XtraBars.Ribbon.Internal;
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class RibbonDrawInfo : ObjectInfoArgs {
		object viewInfo;
		public RibbonDrawInfo(RibbonPageViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.Bounds = viewInfo.Bounds;
		}
		public RibbonDrawInfo(GraphicsCache cache, RibbonPageGroupViewInfo viewInfo) {
			this.Cache = cache;
			this.viewInfo = viewInfo;
			this.Bounds = viewInfo.Bounds;
		}
		public RibbonDrawInfo(RibbonItemViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.Bounds = viewInfo.Bounds;
		}
		public RibbonDrawInfo(object viewInfo) {
			this.viewInfo = viewInfo;
		}
		public object ViewInfo { get { return viewInfo; } }
	}
	public class RibbonHeaderPainter : ObjectPainter {
		public void DrawBackground(ObjectInfoArgs e, bool upperPart) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			if(vi.IsEmptyTabHeader) return;
			if(!vi.Ribbon.AllowGlassTabHeader && upperPart) 
				return;
			GraphicsClipState state = null;
			if(vi.Ribbon.AllowGlassTabHeader && vi.GetRibbonStyle() != RibbonControlStyle.OfficeUniversal) {
				Rectangle rect = upperPart? vi.Caption.Bounds: vi.Header.Bounds;
				state = e.Cache.ClipInfo.SaveAndSetClip(rect);
			}
			SkinElementInfo info = GetHeaderInfo(vi);
			if(info.Bounds.Height > 0)
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			if(state != null)
				e.Cache.ClipInfo.RestoreClip(state);
		}
		protected virtual SkinElementInfo GetHeaderInfo(RibbonViewInfo vi) {
			SkinElementInfo res = vi.Header.GetHeaderInfo();
			if(vi.ShouldUseAppButtonContainerControlBkgnd) {
				return vi.GetAppButtonContainerInfo(Rectangle.Inflate(res.Bounds, 1, 0));
			}
			return res;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			DrawHeaderCore(e, vi);
			DrawPages(e, vi);
			DrawCategories(e, vi);
			DrawPageHeaderItems(e, vi);
			DrawScrollButtons(e, vi);
			DrawDesignerRect(vi, e.Cache, vi.Header.DesignerRect);
		}
		protected virtual void DrawScrollButtons(ObjectInfoArgs e, RibbonViewInfo vi) {
			if(vi.IsEmptyTabHeader) return;
			if(vi.Header.ShowLeftScrollButton) DrawLeftScrollButton(vi, e.Cache);
			if(vi.Header.ShowRightScrollButton) DrawRightScrollButton(vi, e.Cache);
		}
		protected virtual void DrawPageHeaderItems(ObjectInfoArgs e, RibbonViewInfo vi) {
			foreach(RibbonItemViewInfo itemInfo in vi.Header.PageHeaderItems) {
				if(vi.IsExpandItemInRibbonPanel(itemInfo)) continue;
				DrawItem(vi, e.Cache, itemInfo);
			}
		}
		protected virtual void DrawCategories(ObjectInfoArgs e, RibbonViewInfo vi) {
			if(vi.IsDesignMode && vi.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal) {
				foreach(RibbonPageCategoryViewInfo categoryInfo in vi.Header.PageCategories) {
					DrawDesignerRect(vi, e.Cache, categoryInfo.LowerBounds);
				}
			}
			if(vi.IsEmptyTabHeader || !vi.ShouldDrawPageCategories()) return;
			foreach(RibbonPageCategoryViewInfo categoryInfo in vi.Header.PageCategories) {
				DrawPageCategory(e.Cache, vi, categoryInfo);
			}
		}
		protected virtual void DrawPages(ObjectInfoArgs e, RibbonViewInfo vi) {
			if(vi.IsEmptyTabHeader) return;
			switch(vi.GetRibbonStyle()) {
				case RibbonControlStyle.MacOffice:
					DrawMacStyleRibbonPages(e);
					break;
				case RibbonControlStyle.OfficeUniversal:
					DrawUniversalRibbonPages(e);
					break;
				default:
					DrawSimpleStyleRibbonPages(e);
					break;
			}
		}
		protected virtual void DrawHeaderCore(ObjectInfoArgs e, RibbonViewInfo vi) {
			if(vi.IsEmptyTabHeader || !vi.Header.ViewInfo.GetIsMinimized()) return;
			Rectangle clip = vi.Header.GetHeaderInfo().Bounds;
			int height = RibbonSkins.GetSkin(vi.Header.ViewInfo.Provider).Properties.GetInteger(RibbonSkins.OptTabHeaderDownGrow) - 1;
			clip.Y = clip.Bottom - height; clip.Height = height;
			e.Graphics.ExcludeClip(clip);
		}
		protected virtual void DrawUniversalRibbonPages(ObjectInfoArgs e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			foreach(RibbonPageViewInfo pageInfo in vi.Header.Pages) {
				if(pageInfo.Bounds.IsEmpty) continue;
				DrawPage(e.Cache, vi, pageInfo);
			}
			DrawItem(vi, e.Cache, vi.Ribbon.AutoHiddenPagesMenuItemLink.RibbonItemInfo);
		}
		protected virtual void DrawSimpleStyleRibbonPages(ObjectInfoArgs e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			Rectangle rect = vi.Header.PageHeaderBounds;
			if(vi.Header.ShowLeftScrollButton) {
				rect.Width = vi.Header.PageHeaderBounds.Right - vi.Header.LeftScrollBounds.Right;
				rect.X = vi.Header.LeftScrollBounds.Right;
			}
			if(vi.Header.ShowRightScrollButton) {
				rect.Width = vi.Header.RightScrollBounds.X - rect.X;
			}
			GraphicsClipState clipState = e.Cache.ClipInfo.SaveAndSetClip(rect);
			foreach(RibbonPageViewInfo pageInfo in vi.Header.Pages) {
				DrawPage(e.Cache, vi, pageInfo);
			}
			e.Cache.ClipInfo.RestoreClip(clipState);
		}
		protected virtual void DrawMacStyleRibbonPages(ObjectInfoArgs e) {
			RibbonDrawInfo re = (RibbonDrawInfo)e;
			RibbonViewInfo viewInfo = (RibbonViewInfo)re.ViewInfo;
			foreach(RibbonPageViewInfo pageInfo in viewInfo.Header.Pages) {
				if(pageInfo.Category != viewInfo.Ribbon.DefaultPageCategory)
					break;
				DrawPage(e.Cache, viewInfo, pageInfo);
			}
			foreach(RibbonPageCategoryViewInfo catInfo in viewInfo.Header.PageCategories) {
				if(catInfo.Pages.Count > 0) {
					DrawPage(e.Cache, viewInfo, catInfo.Pages[0]);
				}
				if(catInfo.Pages.Count == 1)
					continue;
				GraphicsClipState prev = null;
				if(!catInfo.Category.Expanded || catInfo.Animated)
					prev = e.Cache.ClipInfo.SaveAndSetClip(catInfo.CollapsedPagesBounds);
				for(int i = 1; i < catInfo.Pages.Count; i++ ) {
					DrawPage(e.Cache, viewInfo, catInfo.Pages[i]);
				}
				if(prev != null)
					e.Cache.ClipInfo.RestoreClip(prev);
			}
		}
		protected virtual void DrawDesignerRect(RibbonViewInfo vi, GraphicsCache cache, Rectangle rect) {
			if(!vi.IsDesignMode) return;
			Pen pen = new Pen(Color.Red);
			pen.DashPattern = new float[] { 5.0f, 5.0f };
			cache.DrawRectangle(pen, rect);
		}
		protected virtual void DrawLeftScrollButton(RibbonViewInfo viewInfo, GraphicsCache cache) {
			SkinElementInfo info = viewInfo.Header.GetLeftScrollButtonInfo();
			if(info == null || info.Element == null) return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected virtual void DrawRightScrollButton(RibbonViewInfo viewInfo, GraphicsCache cache) {
			SkinElementInfo info = viewInfo.Header.GetRightScrollButtonInfo();
			if(info == null || info.Element == null) return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected virtual void DrawItem(RibbonViewInfo viewInfo, GraphicsCache cache, RibbonItemViewInfo itemInfo) {
			RibbonItemViewInfoCalculator.DrawItem(cache, itemInfo);
		}
		protected virtual SkinElementInfo GetPageSeparatorInfo(RibbonViewInfo vi, RibbonPageViewInfo pageInfo) {
			SkinElement se = RibbonSkins.GetSkin(vi.Provider)[RibbonSkins.SkinPageSeparator];
			if(se == null || se.Image == null || se.Image.Image == null) return null;
			Rectangle bounds = se.Image.GetImageBounds(0);
			int pageIndex = vi.Header.Pages.IndexOf(pageInfo);
			int indent = pageIndex == vi.Header.Pages.Count -1 ? vi.Header.IndentBetweenPages: vi.Header.Pages[pageIndex+1].Bounds.X - pageInfo.Bounds.Right;
			bounds.Offset(pageInfo.Bounds.Right + (indent - bounds.Width) / 2, vi.Header.Bounds.Y);
			bounds.Height = vi.Header.Bounds.Height - 1;
			return new SkinElementInfo(se, bounds);
		}
		protected virtual void DrawSeparatorLine(GraphicsCache cache, RibbonViewInfo vi, RibbonPageViewInfo pageInfo) {
			if(vi.Header.PageSeparateLineOpacity == 0.0f) return;
			SkinElementInfo info = GetPageSeparatorInfo(vi, pageInfo);
			if(info == null) return ;
			lock(OpacityProvider.DefaultAttributes) {
				PaintHelper.DrawImageCore(cache.Graphics, info.Element.Image.Image, info.Bounds.X, info.Bounds.Y, info.Bounds.Width, info.Bounds.Height, info.Element.Image.GetImageBounds(0), OpacityProvider.GetAttributes(vi.Header.PageSeparateLineOpacity));
			}
		}
		protected virtual SkinElementInfo GetPageCategorySeparatorInfo(RibbonViewInfo vi, Rectangle bounds) {
			SkinElement se = RibbonSkins.GetSkin(vi.Provider)[RibbonSkins.SkinContextTabCategorySeparator];
			if(se == null || se.Image == null) return null;
			return new SkinElementInfo(se, bounds);
		}
		protected virtual void DrawPageCategoryBackground(GraphicsCache cache, RibbonViewInfo vi, RibbonPageCategoryViewInfo categoryInfo) {
			object obj = RibbonSkins.GetSkin(vi.Provider).Properties[RibbonSkins.SkinDrawCategoryBackground];
			if((obj != null && !((bool)obj)) || !vi.SupportTransparentPages)
				return;
			SkinElementInfo info = vi.Header.GetContextPageInfo();
			if(categoryInfo.Pages.Count > 0) {
				RibbonPageViewInfo pi = categoryInfo.Pages[categoryInfo.Pages.Count - 1];
				if(vi.IsRightToLeft)
					info.Bounds = new Rectangle(categoryInfo.LowerBounds.Left + GetPageCategorySeparatorWidth(vi), categoryInfo.LowerBounds.Y, pi.Bounds.X - categoryInfo.LowerBounds.Left - GetPageCategorySeparatorWidth(vi), categoryInfo.LowerBounds.Height);
				else
					info.Bounds = new Rectangle(pi.Bounds.Right, categoryInfo.LowerBounds.Y, categoryInfo.LowerBounds.Right - pi.Bounds.Right - GetPageCategorySeparatorWidth(vi), categoryInfo.LowerBounds.Height);
			}
			else {
				info.Bounds = categoryInfo.LowerBounds;
			}
			if(info.Bounds.Width > 0) {
				RibbonPainter.DrawColoredPage(cache, info, categoryInfo.Category.Color, categoryInfo.ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice);
			}
		}
		protected virtual void DrawPageCategory(GraphicsCache cache, RibbonViewInfo vi, RibbonPageCategoryViewInfo categoryInfo) {
			if(!vi.ShouldDrawPageCategories()) return;
			DrawPageCategoryBackground(cache, vi, categoryInfo);
			if(vi.Ribbon.ShowCategoryInCaption)
				DrawPageCategorySeparators(cache, vi, categoryInfo);
			if(!vi.IsDesignMode || !vi.DesignTimeManager.IsComponentSelected(categoryInfo.Category) || (vi.Form != null && vi.Ribbon.ShowCategoryInCaption)) return;
			vi.DesignTimeManager.DrawSelection(cache, categoryInfo.LowerBounds, Color.Magenta);
		}
		protected int GetPageCategorySeparatorWidth(RibbonViewInfo vi) {
			SkinElement se = RibbonSkins.GetSkin(vi.Provider)[RibbonSkins.SkinContextTabCategorySeparator];
			if(se == null || se.Image == null) return 1;
			return se.Image.GetImageBounds(0).Width;
		}
		protected virtual void DrawPageCategorySeparator(GraphicsCache cache, RibbonViewInfo vi, RibbonPageCategoryViewInfo categoryInfo, SkinElementInfo info) {
			if (vi.SupportTransparentPages)
				RibbonPainter.DrawColoredPageCategorySeparator(cache, info, categoryInfo.Category.Color);
			else
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected virtual void DrawPageCategorySeparators(GraphicsCache cache, RibbonViewInfo vi, RibbonPageCategoryViewInfo categoryInfo) { 
			Rectangle rect = categoryInfo.LowerBounds;
			int width = GetPageCategorySeparatorWidth(vi);
			rect.Width = width;
			SkinElementInfo sepInfo = GetPageCategorySeparatorInfo(vi, rect);
			if(sepInfo == null) return;
			DrawPageCategorySeparator(cache, vi, categoryInfo, sepInfo);
			rect.X = categoryInfo.LowerBounds.Right - width;
			sepInfo.Bounds = rect;
			sepInfo.ImageIndex = 1;
			DrawPageCategorySeparator(cache, vi, categoryInfo, sepInfo);
		}
		protected virtual void DrawPage(GraphicsCache cache, RibbonViewInfo vi, RibbonPageViewInfo pageInfo) {
			XtraAnimator.Current.DrawAnimationHelper(cache, vi.OwnerControl as ISupportXtraAnimation, pageInfo,
				new RibbonHeaderPagePainter(), new RibbonDrawInfo(pageInfo),
				new RibbonHeaderPageTextPainter(), new RibbonDrawInfo(pageInfo));
			DrawSeparatorLine(cache, vi, pageInfo);
			if(!pageInfo.Page.Visible) cache.FillRectangle(cache.GetSolidBrush(Color.FromArgb(50, Color.Gray)), pageInfo.Bounds);
			if(vi.IsDesignMode && vi.DesignTimeManager.IsComponentSelected(pageInfo.Page)) {
				vi.DesignTimeManager.DrawSelection(cache, pageInfo.Bounds, Color.Magenta);
			}
		}
	}
	public class RibbonCaptionPainter : ObjectPainter {
		public virtual void DrawCaption(ObjectInfoArgs e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			if(!vi.Caption.TextBounds.IsEmpty) {
				if(vi.Caption.IsGlassForm) {
					DrawGlassCaption(e);		
				}
				else {
					if(vi.Form != null && vi.Form.FormPainter != null && vi.Form.FormPainter.AllowHtmlText)
						DrawHtmlCaption(e);
					else
						DrawSimpleCaption(e);
				}
			}
		}
		TextFormatFlags CaptionTextFormatFlags { get { return TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter; } }
		Rectangle GetGlassCaptionTextBounds(RibbonViewInfo vi) {
			return GetGlassCaptionTextBounds(vi, vi.Caption.TextBounds);
		}
		Rectangle GetGlassCaptionTextBounds(RibbonViewInfo vi, Rectangle rect) {
			Rectangle text = rect;
			text.Y = vi.Caption.ClientBounds.Top;
			text.Height = vi.Caption.ClientBounds.Height;
			return text;
		}
		Color GetGlassCaptionColor(RibbonViewInfo vi) {
			Color textColor = SystemColors.ControlText;
			if(!vi.Caption.IsWindowActive) textColor = SystemColors.InactiveCaptionText;
			if(vi.Caption.IsZoomedWindow && !NativeVista.IsWindows7)
				textColor = SystemColors.Window;
			return textColor;
		}
		protected virtual void DrawGlassCaption(ObjectInfoArgs e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			if(vi.Form == null || vi.Form.FormPainter == null) return;
			Rectangle text = GetGlassCaptionTextBounds(vi);
			Color textColor = GetGlassCaptionColor(vi);
			if(vi.Caption.IsZoomedWindow && !NativeVista.IsWindows7) {
				if(vi.Form.FormPainter.AllowHtmlText)
					StringPainter.Default.DrawString(e.Cache, vi.Caption.StringInfo);
				else
					e.Cache.DrawString(vi.Caption.GetFormText(), vi.PaintAppearance.FormCaption.Font, e.Cache.GetSolidBrush(textColor), text, vi.PaintAppearance.FormCaption.GetStringFormat());
			} else {
				if(vi.Form.FormPainter.AllowHtmlText && vi.Caption.StringInfo.Blocks != null) {
					for(int i = 0; i < vi.Caption.StringInfo.Blocks.Count; i++) {
						StringBlock block = vi.Caption.StringInfo.Blocks[i];
						Rectangle rect = GetGlassCaptionTextBounds(vi, vi.Caption.StringInfo.BlocksBounds[i]);
						NativeVista.DrawGlowingText(e.Cache.Graphics, block.Text, block.Font, rect, block.FontSettings.Color, CaptionTextFormatFlags);
					}
				}
				else 
					NativeVista.DrawGlowingText(e.Cache.Graphics, vi.Caption.GetFormText(), vi.PaintAppearance.FormCaption.Font, text, textColor, CaptionTextFormatFlags);
			}
		}
		protected virtual void DrawSimpleCaption(ObjectInfoArgs e) {
			RibbonViewInfo info = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			DrawSimpleCaptionOwnerText(e.Cache, info);
			if(!info.Caption.HasTextChild) return;
			DrawSimpleCaptionDelimiter(e.Cache, info);
			DrawSimpleCaptionChildText(e.Cache, info);
		}
		protected virtual void DrawSimpleCaptionChildText(GraphicsCache cache, RibbonViewInfo info) {
			Rectangle bounds = info.Caption.GetTextChildBounds(cache.Graphics);
			cache.DrawString(info.Caption.GetTextChild(), info.PaintAppearance.FormCaption.Font, cache.GetSolidBrush(GetTextPart1ForeColor(info)), bounds, info.PaintAppearance.FormCaption.GetStringFormat());
		}
		protected virtual void DrawSimpleCaptionDelimiter(GraphicsCache cache, RibbonViewInfo info) {
			Rectangle bounds = info.Caption.GetTextDelimiterBounds(cache.Graphics);
			cache.DrawString(info.Caption.Delimiter, info.PaintAppearance.FormCaption.Font, cache.GetSolidBrush(GetTextPart2ForeColor(info)), bounds, info.PaintAppearance.FormCaption.GetStringFormat());
		}
		protected virtual void DrawSimpleCaptionOwnerText(GraphicsCache cache, RibbonViewInfo info) {
			Rectangle bounds = info.Caption.GetTextOwnerBounds(cache.Graphics);
			cache.DrawString(info.Caption.GetTextOwner(), info.PaintAppearance.FormCaption.Font, cache.GetSolidBrush(GetTextPart2ForeColor(info)), bounds, info.PaintAppearance.FormCaption.GetStringFormat());
		}
		protected internal virtual Color GetTextPart1ForeColor(RibbonViewInfo vi) {
			return vi.PaintAppearance.FormCaption.ForeColor;
		}
		protected internal virtual Color GetTextPart2ForeColor(RibbonViewInfo vi) {
			Color color = vi.PaintAppearance.FormCaptionForeColor2;
			if(!vi.Caption.IsWindowActive) color = vi.PaintAppearance.FormCaptionForeColorInactive;
			return color;
		}
		protected virtual void DrawHtmlCaption(ObjectInfoArgs e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			StringPainter.Default.DrawString(e.Cache, vi.Caption.StringInfo);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			if(vi.Caption.Bounds.IsEmpty || !e.Cache.IsNeedDrawRect(vi.Caption.Bounds)) return;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, vi.Caption.GetCaptionDrawInfo());
			if(vi.ShouldUseAppButtonContainerControlBkgnd && vi.Caption.CanGetFormButtonsBounds) {
				Rectangle rect = vi.Caption.FormButtonsBounds;
				if(!rect.IsEmpty)
					ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, vi.GetAppButtonContainerInfo(Rectangle.Inflate(rect, 1, 1)));
			}
			DrawCaption(e);
			DrawPageHeaderBackground(e, true);
			if(vi.Caption.FormPainter != null) vi.Caption.FormPainter.DrawIcon(e.Cache);
			if(vi.Caption.FormPainter != null) vi.Caption.FormPainter.DrawButtons(e.Cache, false);
			DrawPageCategories(e as RibbonDrawInfo);
		}
		protected virtual void DrawPageHeaderBackground(ObjectInfoArgs e, bool upperPart) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			if(!vi.Ribbon.AllowGlassTabHeader)
				return;
			vi.Header.HeaderPainter.DrawBackground(e, upperPart);
		}
		protected virtual void DrawPageCategories(RibbonDrawInfo e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			if(!vi.ShouldDrawPageCategories()) return;
			new RibbonPageCategoryUpperPartPainter().DrawObject(e);
		}
	}
	public class RibbonPageCategoryUpperPartPainter : ObjectPainter {
		protected virtual void DrawPageCategoryBackground(GraphicsCache cache, RibbonPageCategoryViewInfo categoryInfo) {
			SkinElementInfo info = categoryInfo.GetPageCategoryUpperInfo(categoryInfo.ViewInfo, categoryInfo.UpperBounds);
			if(info == null)
				return;
			Color cl = categoryInfo.Category.Color;
			if(categoryInfo.ViewInfo.Form != null && categoryInfo.ViewInfo.Form.IsGlassForm && cl.A == 255)
				cl = Color.FromArgb(220, cl.R, cl.G, cl.B);
			if(categoryInfo.ViewInfo.SupportTransparentPages)
				RibbonPainter.DrawColoredPageCategory(cache, info, cl);
			else {
				Color prevColor = info.BackAppearance.BackColor;
				try {
					info.BackAppearance.BackColor = cl;
					SkinElementPainter.UseAppearanceBackColor2 = true;
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
					SkinElementPainter.UseAppearanceBackColor2 = false;
				} finally {
					info.BackAppearance.BackColor = prevColor;
				}
			}
		}
		public virtual void DrawPageCategory(GraphicsCache cache, RibbonPageCategoryViewInfo categoryInfo) {
			RibbonViewInfo vi = categoryInfo.ViewInfo;
			if(vi == null) return;
			DrawPageCategoryBackground(cache, categoryInfo);
			using(StringFormat fmt = (StringFormat)vi.PaintAppearance.PageCategory.GetStringFormat().Clone()) {
				if(categoryInfo.TextBounds.Width < categoryInfo.BestTextWidth)
					fmt.Alignment = StringAlignment.Near;
				DrawPageCategoryTextShadow(cache, categoryInfo, fmt);
				DrawPageCategoryText(cache, categoryInfo, fmt);
			}
			RibbonSelectionInfo selInfo = vi.Manager.SelectionInfo as RibbonSelectionInfo;
			if(selInfo != null && selInfo.DropSelectedObject == categoryInfo.Category)
				new PrimitivesPainter(vi.Manager.PaintStyle).DrawRibbonCategoryDropMark(cache.Graphics, categoryInfo, vi.Manager.SelectionInfo.DropSelectStyle);
			if(!vi.IsDesignMode || !vi.DesignTimeManager.IsComponentSelected(categoryInfo.Category)) return;
			vi.DesignTimeManager.DrawSelection(cache, categoryInfo.Bounds, Color.Magenta);
		}
		protected virtual void DrawPageCategoryText(GraphicsCache cache, RibbonPageCategoryViewInfo categoryInfo, StringFormat fmt) {
			RibbonViewInfo vi = categoryInfo.ViewInfo;
			SkinElementInfo info = categoryInfo.GetPageCategoryUpperInfo(vi, categoryInfo.UpperBounds);
			if(info == null) return;
			if(vi.Form != null && vi.Form.IsGlassForm)
				ObjectPainter.DrawTextOnGlass(cache.Graphics, vi.PaintAppearance.PageCategory, categoryInfo.Category.Text, categoryInfo.TextBounds, fmt);
			else
				vi.PaintAppearance.PageCategory.DrawString(cache, categoryInfo.Category.Text, categoryInfo.TextBounds, cache.GetSolidBrush(info.Element.Color.ForeColor), fmt);
		}
		protected virtual Color GetPageCategoryTextShadowColor(RibbonPageCategoryViewInfo categoryInfo) {
			object res = RibbonSkins.GetSkin(categoryInfo.ViewInfo.Provider).Properties[RibbonSkins.OptPageCategoryTextShadowColor];
			if(res == null) return Color.Empty;
			return (Color)res;
		}
		protected virtual int GetPageCategoryTextShadowOffset(RibbonPageCategoryViewInfo categoryInfo) {
			object res = RibbonSkins.GetSkin(categoryInfo.ViewInfo.Provider).Properties[RibbonSkins.OptPageCategoryTextShadowOffset];
			if(res == null) return 1;
			return (int)res;
		}
		protected virtual void DrawPageCategoryTextShadow(GraphicsCache cache, RibbonPageCategoryViewInfo categoryInfo, StringFormat fmt) {
			RibbonViewInfo vi = categoryInfo.ViewInfo;
			if(vi == null) return;
			Color foreColor = GetPageCategoryTextShadowColor(categoryInfo);
			if(foreColor == Color.Empty)
				return;
			int offset = GetPageCategoryTextShadowOffset(categoryInfo);
			Rectangle rect = categoryInfo.TextBounds;
			rect.Offset(offset, offset);
			if(vi.Form != null && vi.Form.IsGlassForm)
				ObjectPainter.DrawTextOnGlass(cache.Graphics, vi.PaintAppearance.PageCategory.Font, categoryInfo.Category.Text, rect, fmt, foreColor);
			else
				vi.PaintAppearance.PageCategory.DrawString(cache, categoryInfo.Category.Text, rect, cache.GetSolidBrush(foreColor), fmt);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonDrawInfo ri = e as RibbonDrawInfo;
			if(ri == null) return;
			DrawPageCategories(e.Cache, ri.ViewInfo as RibbonViewInfo);
		}
		public virtual void DrawPageCategories(GraphicsCache cache, RibbonViewInfo viewInfo) {
			foreach(RibbonPageCategoryViewInfo vi in viewInfo.Header.PageCategories) {
				DrawPageCategory(cache, vi);
			}
		 }
	}
	public class RibbonHeaderPageTextPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonPageViewInfo page = (RibbonPageViewInfo)((RibbonDrawInfo)e).ViewInfo;
			SkinElementInfo hinfo = page.GetHeaderPageInfo();
			if(hinfo.Element == null)
				hinfo = page.GetHeaderPageInfo(true);
			Rectangle r = ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, hinfo);
			bool restoreClip = false;
			Rectangle clipRect, headerBounds = page.ViewInfo.Header.PageHeaderBounds;
			Rectangle clipBounds = new Rectangle((int)e.Graphics.ClipBounds.X, (int)e.Graphics.ClipBounds.Y, (int)e.Graphics.ClipBounds.Width, (int)e.Graphics.ClipBounds.Height);
			GraphicsClipState clipState = null;
			if(!clipBounds.IntersectsWith(headerBounds) || !e.Cache.IsNeedDrawRect(clipBounds)) return;
			if(clipBounds != headerBounds && (page.Bounds.X < headerBounds.X || page.Bounds.Right > headerBounds.Right)) {
				clipRect = page.Bounds;
				clipRect.Intersect(page.ViewInfo.Header.PageHeaderBounds);
				if(clipRect.IsEmpty) return;
				clipState = e.Cache.ClipInfo.SaveAndSetClip(clipRect);
				restoreClip = true;
			}
			using(StringFormat sf = (StringFormat)page.ViewInfo.PaintAppearance.PageHeader.TextOptions.GetStringFormat().Clone()) {
				sf.Alignment = StringAlignment.Center;
				sf.Trimming = StringTrimming.Character;
				sf.FormatFlags |= StringFormatFlags.NoWrap;
				if(page.ViewInfo.SupportTransparentPages && page.ViewInfo.Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007 && page.ViewInfo.Form != null && page.ViewInfo.Form.IsGlassForm) {
					ObjectPainter.DrawTextOnGlass(e.Graphics, page.PaintAppearance, page.Page.Text, page.TextBounds, sf);
				} else
					page.PaintAppearance.DrawString(e.Cache, page.Page.Text, page.TextBounds, sf);
			}
			if(restoreClip) {
				e.Cache.ClipInfo.RestoreClipRelease(clipState);
			}
		}
	}
	public class RibbonHeaderPagePainter : ObjectPainter {
		static ImageAttributes Attributes = new ImageAttributes();
		static ColorMatrix Matrix = new ColorMatrix();
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonPageViewInfo page = (RibbonPageViewInfo)((RibbonDrawInfo)e).ViewInfo;
			SkinElementInfo hinfo = page.GetHeaderPageInfo();
			if(page.ViewInfo.SupportTransparentPages) {
				hinfo = page.GetHeaderPageInfo(false);
				SkinElementInfo maskInfo = page.GetHeaderPageMaskInfo();
				if (page.Page.Category != null && !page.Page.Category.IsDefaultColor) {
					RibbonPainter.DrawColoredPage(e.Cache, hinfo, page.Page.Category.Color, page.ViewInfo.GetRibbonStyle() == RibbonControlStyle.MacOffice);
					if(maskInfo != null)
						ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, maskInfo);
				}
				else 
					ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, hinfo);
			}
			else {
				Color prevColor = hinfo.BackAppearance.BackColor;
				if(!page.Page.Category.IsDefaultColor) hinfo.BackAppearance.BackColor = page.Category.Color;
				if(!page.ViewInfo.GetIsMinimized() ||
					(hinfo.State != ObjectState.Normal)) {
					ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, hinfo);
				}
				hinfo.BackAppearance.BackColor = prevColor;
			}
			if(page.ImageBounds.Size != Size.Empty)
				e.Graphics.DrawImage(page.Page.GetImage(), page.ImageBounds);
			ObjectPainter.DrawObject(e.Cache, new RibbonHeaderPageTextPainter(), e);
			if(page.Page.Ribbon == null) return;
			RibbonSelectionInfo selInfo = page.Page.Ribbon.Manager.SelectionInfo as RibbonSelectionInfo;
			if(selInfo != null && selInfo.DropSelectedObject == page.Page)
				new PrimitivesPainter(page.Page.Ribbon.Manager.PaintStyle).DrawRibbonPageDropMark(e.Graphics, page, page.Page.Ribbon.Manager.SelectionInfo.DropSelectStyle);
		}
	}
	public class RibbonItemPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonItemViewInfo item = (RibbonItemViewInfo)((RibbonDrawInfo)e).ViewInfo;
			BarItemLink link = item.Item as BarItemLink;
			if(!(item is RibbonButtonGroupItemViewInfo) && !(item is InRibbonGalleryRibbonItemViewInfo) && !(item is RibbonSeparatorItemViewInfo)) {
				if(link == null || link.Ribbon == null) return;
				BarItemCustomDrawEventArgs ie = new BarItemCustomDrawEventArgs(e.Cache, item);
				item.CheckViewInfo(e.Graphics);
				link.Ribbon.Manager.RaiseCustomDrawItem(ie);
				if(ie.Handled)
					return;
			}
			item.GetInfo().Draw(e.Cache, item);
			if(link == null || link.Ribbon == null) return;
			RibbonSelectionInfo selInfo = link.Manager.SelectionInfo as RibbonSelectionInfo;
			if(selInfo != null && selInfo.DropSelectedObject == link)
				new PrimitivesPainter(link.Manager.PaintStyle).DrawRibbonLinkDropMark(e.Graphics, link, link.Manager.SelectionInfo.DropSelectStyle);
		}
	}
	public class RibbonPanelPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonViewInfo vi = (RibbonViewInfo)((RibbonDrawInfo)e).ViewInfo;
			GraphicsClipState state = null;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, vi.GetPanelInfo());
			if(vi.SelectedPage != null && vi.SelectedPage.Category != null && vi.SelectedPage.Category.Color != vi.Ribbon.DefaultPageCategory.Color && vi.SupportTransparentPages) {
				SkinElementInfo borderInfo = vi.GetPanelBorderInfo();
				if(borderInfo != null)
					RibbonPainter.DrawColoredPanel(e.Cache, borderInfo, vi.SelectedPage.Category.Color);
			}
			if(vi.Panel.ShowLeftScrollButton || vi.Panel.ShowRightScrollButton) {
				Rectangle rect = vi.Panel.ContentBounds;
				rect.Width = vi.Panel.ContentWidth;
				if(vi.Panel.ShowLeftScrollButton) {
					if(!vi.IsRightToLeft)
						rect.X += vi.Panel.LeftScrollButtonBounds.Width;
					rect.Width -= vi.Panel.LeftScrollButtonBounds.Width;
				}
				if(vi.Panel.ShowRightScrollButton) {
					if(vi.IsRightToLeft)
						rect.X += vi.Panel.RightScrollButtonBounds.Width + vi.Panel.GetExpandItemRegionWidth();
					rect.Width -= vi.Panel.RightScrollButtonBounds.Width;
				}
				state = e.Cache.ClipInfo.SaveAndSetClip(rect);
				if(IsExpandButtonInPanel(vi)) {
					e.Cache.ClipInfo.ExcludeClip(vi.Panel.PanelItems.ExpandItemInfo.Bounds);
				}
			}
			foreach(RibbonPageGroupViewInfo groupInfo in vi.Panel.Groups) {
				DrawGroup(vi, e.Cache, groupInfo);
			}
			if(state != null) e.Cache.ClipInfo.RestoreClip(state);
			if(vi.Panel.ShowLeftScrollButton)
				DrawLeftScrollButton(vi, e.Cache);
			if(vi.Panel.ShowRightScrollButton)
				DrawRightScrollButton(vi, e.Cache);
			if(IsExpandButtonInPanel(vi))
				DrawItem(vi, e.Cache, vi.Panel.PanelItems.ExpandItemInfo);
		}
		protected bool IsExpandButtonInPanel(RibbonViewInfo vi) {
			return vi.Ribbon.IsExpandButtonInPanel && vi.Panel.PanelItems.ExpandItemInfo != null;
		}
		protected virtual void DrawItem(RibbonViewInfo viewInfo, GraphicsCache cache, RibbonItemViewInfo itemInfo) {
			RibbonItemViewInfoCalculator.DrawItem(cache, itemInfo);
		}
		protected virtual void DrawLeftScrollButton(RibbonViewInfo viewInfo, GraphicsCache cache) {
			SkinElementInfo info = viewInfo.Panel.GetLeftScrollButtonInfo();
			if(info == null || info.Element == null) return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected virtual void DrawRightScrollButton(RibbonViewInfo viewInfo, GraphicsCache cache) {
			SkinElementInfo info = viewInfo.Panel.GetRightScrollButtonInfo();
			if(info == null || info.Element == null) return;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected virtual void DrawActiveControl(RibbonViewInfo viewInfo, RibbonPageGroupViewInfo groupInfo) {
			if(viewInfo.Manager.ActiveEditor != null && groupInfo.PageGroup.ItemLinks.Contains(viewInfo.Manager.ActiveEditItemLink))
				viewInfo.Manager.ActiveEditor.Invalidate();	
		}
		protected virtual void DrawGroup(RibbonViewInfo viewInfo, GraphicsCache cache, RibbonPageGroupViewInfo groupInfo) {
			if(!cache.IsNeedDrawRect(groupInfo.Bounds)) return;
			groupInfo.UpdateImageAttributes(null);
			groupInfo.ForceStateImageIndex = -1;
			FadeAnimationInfo fade = XtraAnimator.Current.Get(viewInfo.OwnerControl as ISupportXtraAnimation, groupInfo) as FadeAnimationInfo;
			if(fade != null) {
				if(fade.FadeIn)
					groupInfo.ForceStateImageIndex = groupInfo.GetDrawInfo().StateIndex == 0 ? 1 : 0;
				XtraAnimator.Current.LockDrawString();
				try {
					viewInfo.Panel.GroupPainter.DrawBackground(new RibbonDrawInfo(cache, groupInfo));
					groupInfo.ForceStateImageIndex = -1;
					if(!fade.FadeIn)
						groupInfo.ForceStateImageIndex = groupInfo.GetDrawInfo().StateIndex == 0 ? 1 : 0;
					fade.UpdateAttributes();
					groupInfo.UpdateImageAttributes(fade.Attributes);
				}
				finally {
					XtraAnimator.Current.UnlockDrawString();
				}
			}
			DrawActiveControl(viewInfo, groupInfo);
			viewInfo.Panel.GroupPainter.DrawBackground(new RibbonDrawInfo(cache, groupInfo));
			if(groupInfo.ViewInfo.IsDesignMode) {
				if(!groupInfo.ViewInfo.ShouldDrawGroupBorder) {
					DrawDesignerBounds(cache, groupInfo);
				}
			}
			groupInfo.UpdateImageAttributes(null);
			viewInfo.Panel.GroupPainter.DrawItems(new RibbonDrawInfo(cache, groupInfo));
			if(!groupInfo.PageGroup.Visible) cache.FillRectangle(cache.GetSolidBrush(Color.FromArgb(50, Color.Gray)), groupInfo.Bounds);
			if(viewInfo.IsDesignMode && viewInfo.DesignTimeManager.IsComponentSelected(groupInfo.PageGroup) || 
				ShouldDrawReduceOperationSelection(groupInfo))
				viewInfo.DesignTimeManager.DrawSelection(cache, groupInfo.Bounds, Color.Magenta);
		}
		void DrawDesignerBounds(GraphicsCache cache, RibbonPageGroupViewInfo viewInfo) {
			using(Pen pen = new Pen(Color.Red)) {
				pen.DashPattern = new float[] { 5.0f, 5.0f };
				cache.Graphics.DrawRectangle(pen, viewInfo.Bounds);
			}
		}
		protected virtual void DrawCaption(GraphicsCache cache, RibbonPageGroupViewInfo group) {
			group.ViewInfo.PaintAppearance.PageGroupCaption.DrawString(cache, group.PageGroup.Text, group.CaptionBounds);
		}
		protected virtual void DrawCaptionSeparator(GraphicsCache cache, RibbonPageGroupViewInfo group) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, group.GetCaptionSeparatorInfo());
		}
		bool ShouldDrawReduceOperationSelection(RibbonPageGroupViewInfo groupInfo) {
			return RibbonReduceOperationHelper.Ribbon == groupInfo.ViewInfo.Ribbon &&
				RibbonReduceOperationHelper.SelectedOperation != null &&
				RibbonReduceOperationHelper.SelectedOperation.Operation == ReduceOperationType.CollapseGroup &&
				RibbonReduceOperationHelper.SelectedOperation
				.Group == groupInfo.PageGroup;
		}
	}
	public class RibbonPanelGroupPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			RibbonPageGroupViewInfo group = (RibbonPageGroupViewInfo)((RibbonDrawInfo)e).ViewInfo;
			if(group.PageGroup.Destroing) return;
			DrawBackground(e);
			DrawItems(e);
		}
		public virtual void DrawBackground(ObjectInfoArgs e) {
			RibbonPageGroupViewInfo group = (RibbonPageGroupViewInfo)((RibbonDrawInfo)e).ViewInfo;
			if(!group.ViewInfo.ShouldDrawGroupBorder)
				return;
			GroupObjectInfoArgs infoArgs = group.GetDrawInfo();
			if(!group.ViewInfo.Ribbon.Enabled)
				infoArgs.Attributes = PaintHelper.RibbonDisabledAttributes;
			ObjectPainter.DrawObject(e.Cache, group.Painter, infoArgs);
		}
		public virtual void DrawItems(ObjectInfoArgs e) {
			RibbonPageGroupViewInfo group = (RibbonPageGroupViewInfo)((RibbonDrawInfo)e).ViewInfo;
			foreach(RibbonItemViewInfo itemInfo in group.Items) {
				DrawItem(group.ViewInfo, e.Cache, itemInfo);
			}
			if(group.PageGroup.Ribbon == null) return;
			RibbonSelectionInfo selInfo = group.PageGroup.Ribbon.Manager.SelectionInfo as RibbonSelectionInfo;
			if(selInfo != null && selInfo.DropSelectedObject == group.PageGroup)
				new PrimitivesPainter(group.PageGroup.Ribbon.Manager.PaintStyle).DrawRibbonPageGroupDropMark(e.Graphics, group, group.PageGroup.Ribbon.Manager.SelectionInfo.DropSelectStyle);
		}
		protected virtual void DrawItem(RibbonViewInfo viewInfo, GraphicsCache cache, RibbonItemViewInfo itemInfo) {
			RibbonItemViewInfoCalculator.DrawItem(cache, itemInfo);
		}
	}
	public class RibbonPainter {
		internal static int ImageIndexByColorScheme(RibbonControlColorScheme colorScheme, ObjectState state) {
			return ImageIndexByColorScheme(colorScheme, state, 4);
		}
		internal static int ImageIndexByColorScheme(RibbonControlColorScheme colorScheme, ObjectState state, int imagesPerColor) {
			int baseImageIndex = (int)colorScheme * imagesPerColor;
			int stateIndex = 0;
			switch(state) { 
				case ObjectState.Hot:
					stateIndex = 1;
					break;
				case ObjectState.Pressed:
				case ObjectState.Selected:
					stateIndex = 2;
					break;
				case ObjectState.Disabled:
					stateIndex = 3;
					break;
			}
			return baseImageIndex + stateIndex;
		}
		public static void DrawColoredPageCategory(GraphicsCache cache, SkinElementInfo info, Color color) {
			Image img = ColoredRibbonElementsCache.GetPageCategoryImage(color, info);
			DrawColoredSkinElement(cache, info, img);
		}
		public static void DrawColoredPage(GraphicsCache cache, SkinElementInfo info, Color color, bool macStyle) {
			Image img = macStyle?ColoredRibbonElementsCache.GetMacTabHeaderPageImage(color, info): ColoredRibbonElementsCache.GetTabHeaderPageImage(color, info);
			DrawColoredSkinElement(cache, info, img);
		}
		public static void DrawColoredPanel(GraphicsCache cache, SkinElementInfo info, Color color) {
			Image img = ColoredRibbonElementsCache.GetTabPanelBorderImage(color, info);
			DrawColoredSkinElement(cache, info, img);
		}
		public static void DrawColoredPageCategorySeparator(GraphicsCache cache, SkinElementInfo info, Color color) {
			Image img = ColoredRibbonElementsCache.GetPageCategorySeparatorImage(color, info);
			DrawColoredSkinElement(cache, info, img);
		}
		protected static void DrawColoredSkinElement(GraphicsCache cache, SkinElementInfo info, Image img) {
			Image oldImage = info.Element.Image.Image;
			SkinImageState state = info.Element.Image.SaveImage();
			info.Element.Image.SetImage(img, Color.Empty);
			info.Element.Image.UseOwnImage = true;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			info.Element.Image.RestoreImage(state);
			info.Element.Image.UseOwnImage = false;
		}
		protected bool IsEmptySkinElement(SkinElementInfo info) {
			return (info.Element.Image == null || info.Element.Image.Image == null) && info.Element.Color.BackColor.IsEmpty && info.Element.Color.BackColor2.IsEmpty;
		}
		protected virtual bool DrawToolbarToControl(Control ctrl, GraphicsCache cache, RibbonViewInfo viewInfo) {
			SkinElementInfo info = null;
			bool res = false;
			if(viewInfo.Ribbon.GetRibbonStyle() != RibbonControlStyle.Office2007 && viewInfo.Header.IsGlassHeader) {
				info = viewInfo.Header.GetHeaderInfo();
				info.Bounds = ctrl.RectangleToClient(viewInfo.Ribbon.RectangleToScreen(info.Bounds));
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
				res = true;
			}
			info = viewInfo.Toolbar.GetToolbarInfo();
			if(info == null) return res;
			if(viewInfo.Form != null && !viewInfo.Form.IsGlassForm && IsEmptySkinElement(info)) {
				info = new SkinElementInfo(RibbonSkins.GetSkin(viewInfo.Provider)[RibbonSkins.SkinFormCaption], viewInfo.Caption.Bounds);
				info.Bounds = ctrl.RectangleToClient(viewInfo.Ribbon.RectangleToScreen(info.Bounds));
			}
			else {
				info.Bounds = ctrl.RectangleToClient(viewInfo.Ribbon.RectangleToScreen(info.Bounds));
			}
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			return true;
		}
		protected virtual void DrawPanelToControl(Control ctrl, GraphicsCache cache, RibbonViewInfo viewInfo) {
			if(viewInfo.Ribbon.SelectedPage != null && !viewInfo.Ribbon.SelectedPage.Category.IsDefaultColor)
				cache.Graphics.FillRectangle(cache.GetSolidBrush(viewInfo.Ribbon.SelectedPage.Category.Color), new Rectangle(Point.Empty, ctrl.Size));
			SkinElementInfo info = viewInfo.GetPanelInfo();
			if(info == null) return;
			info.Bounds = ctrl.RectangleToClient(viewInfo.Ribbon.RectangleToScreen(info.Bounds));
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
		protected virtual void DrawPanelGroupToControl(Control ctrl, GraphicsCache cache, RibbonViewInfo viewInfo) {
			SkinElementInfo info = null;
			RibbonPageGroupViewInfo groupInfo = null;
			foreach(RibbonPageGroupViewInfo vi in viewInfo.Panel.Groups) {
				if(vi.Bounds.Contains(ctrl.Bounds)) {
					groupInfo = vi;
					break;
				}
			}
			if(groupInfo == null) return;
			info = groupInfo.GetBackgroundInfo();
			if(info == null) return;
			info.Bounds = ctrl.RectangleToClient(viewInfo.Ribbon.RectangleToScreen(info.Bounds));
			groupInfo.ForceStateImageIndex = -1;
			FadeAnimationInfo fade = XtraAnimator.Current.Get(viewInfo.OwnerControl as ISupportXtraAnimation, groupInfo) as FadeAnimationInfo;
			if(fade != null) {
				info.ImageIndex = 0;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			info.ImageIndex = 1;
				fade.UpdateAttributes();
				info.Attributes = fade.Attributes;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
		}
			else {
				info.ImageIndex = groupInfo.GetDrawInfo().StateIndex;
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
			}
		}
		public bool DrawControlBackground(Control ctrl, GraphicsCache cache, RibbonViewInfo viewInfo) {
			if(viewInfo.GetToolbarLocation() != RibbonQuickAccessToolbarLocation.Hidden && viewInfo.Toolbar.Bounds.Contains(ctrl.Bounds)) {
				return DrawToolbarToControl(ctrl, cache, viewInfo);
			}
			else {
				DrawPanelToControl(ctrl, cache, viewInfo);				
				DrawPanelGroupToControl(ctrl, cache, viewInfo);	
			}
			return true;
		}
		public virtual void Draw(GraphicsCache cache, RibbonViewInfo viewInfo) {
			RibbonDrawInfo info = new RibbonDrawInfo(viewInfo);
			if(viewInfo.IsFullScreenModeActive) {
				ObjectPainter.DrawObject(cache, viewInfo.FullScreenBarPainter, info);
				return;
			}
			info.Cache = cache;
			viewInfo.Header.HeaderPainter.DrawBackground(info, false);
			if(viewInfo.GetRibbonStyle() == RibbonControlStyle.OfficeUniversal)
				ObjectPainter.DrawObject(cache, viewInfo.Caption.CaptionPainter, info);
			ObjectPainter.DrawObject(cache, viewInfo.Panel.PanelPainter, info);
			if(viewInfo.GetRibbonStyle() != RibbonControlStyle.OfficeUniversal)
				ObjectPainter.DrawObject(cache, viewInfo.Caption.CaptionPainter, info);
			ObjectPainter.DrawObject(cache, viewInfo.Header.HeaderPainter, info);
			ObjectPainter.DrawObject(cache, viewInfo.Toolbar.Toolbar.Painter, new RibbonQuickAccessToolbarInfoArgs(viewInfo.Toolbar));
			DrawApplicationButton(cache, viewInfo);
		}
		protected virtual void DrawApplicationButton(GraphicsCache cache, RibbonViewInfo viewInfo) {
			if(viewInfo.ApplicationButton.Bounds.IsEmpty || !cache.IsNeedDrawRect(viewInfo.ApplicationButton.Bounds)) return;
			if(!viewInfo.IsApplicationMenuOpened) {
				XtraAnimator.Current.DrawAnimationHelper(cache, viewInfo.OwnerControl as ISupportXtraAnimation, RibbonHitTest.ApplicationButton,
				new RibbonApplicationButtonPainter(), viewInfo.GetApplicationButtonInfo(),
				new RibbonApplicationButtonTextPainter(), viewInfo.GetApplicationButtonInfo());
				return;
			}
			ObjectPainter.DrawObject(cache, new RibbonApplicationButtonPainter(), viewInfo.GetApplicationButtonInfo());
		}
	}
	public class SkinRibbonGroupPainterEx : SkinGroupObjectPainter {
		RibbonViewInfo viewInfo;
		public SkinRibbonGroupPainterEx(ISkinProvider provider, RibbonViewInfo viewInfo)
			: base(null, provider) {
			this.viewInfo = viewInfo;
		}
		protected override SkinElement GetPanelCaptionSkinElement(GroupObjectInfoArgs info) {
			return RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinTabPanelGroupCaption];
		}
		internal SkinPaddingEdges GetCaptionMargins(GroupObjectInfoArgs info) {
			SkinElement element = GetPanelCaptionSkinElement(info);
			return element.ContentMargins;
		}
		protected override void CalcButtonsPanelLocation(GroupObjectInfoArgs info, ref Rectangle caption, System.Drawing.Printing.Margins margins) {
			int buttonToBorder = ButtonToBorderDistance;
			margins.Right = buttonToBorder;
			base.CalcButtonsPanelLocation(info, ref caption, margins);
		}
		protected RibbonViewInfo ViewInfo { get { return viewInfo; } }
		protected override ObjectInfoArgs CreateButtonInfo(GroupObjectInfoArgs info) {
			SkinElementInfo e = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinTabPanelGroupButton], info.ButtonBounds);
			e.Attributes = info.Attributes;
			e.ImageIndex = -1;
			e.Cache = info.Cache;
			e.State = info.ButtonState;
			if(!ViewInfo.Ribbon.Enabled)
				e.Attributes = PaintHelper.RibbonDisabledAttributes;
			e.RightToLeft = ViewInfo.IsRightToLeft;
			return e;
		}
		public override int ButtonToBorderDistance { get { return 3; } }
		public override int ButtonToTextBlockDistance { get { return 3; } }
		protected override bool CanUseRotateDrawing { get { return false; } }
		protected override TextOptions GetTextOptions() { return new TextOptions(HorzAlignment.Near, VertAlignment.Center, WordWrap.NoWrap, Trimming.EllipsisCharacter, HKeyPrefix.Hide); }
		public override void DrawVString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle rect, int angle) {
			using(StringFormat format = appearance.GetStringFormat(GetTextOptions()).Clone() as StringFormat) {
				appearance.DrawVString(cache, text, appearance.GetFont(), appearance.GetForeBrush(cache), rect, format, angle);
			}
		}
		protected override ObjectPainter GetBorderPainter(ObjectInfoArgs e) { return new SkinRibbonGroupBorderPainter(Provider); }
		protected override void DrawCaptionSkinElement(GroupObjectInfoArgs info) {
			info.RightToLeft = ViewInfo.IsRightToLeft;
			if(info.CaptionLocation == Locations.Top) {
				SkinElementInfo skinInfo = GetCaptionElement(info);
				new RotateObjectPaintHelper().DrawRotated(info.Cache, skinInfo, SkinElementPainter.Default, RotateFlipType.RotateNoneFlipY);
			}
			else
				base.DrawCaptionSkinElement(info);
		}
		protected class SkinRibbonGroupBorderPainter : SkinBorderPainter {
			public SkinRibbonGroupBorderPainter(ISkinProvider provider)
				: base(provider) {
			}
			protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
				SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinTabPanelGroupBody]);
				SkinGroupBorderObjectInfoArgs sb = e as SkinGroupBorderObjectInfoArgs;
				RibbonPageGroupViewInfo vi = (RibbonPageGroupViewInfo)sb.GroupInfo.Tag;
				if(vi.Minimized) res.Element = RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinTabPanelGroupMinimized];
				res.ImageIndex = sb.GroupInfo.StateIndex;
				res.Attributes = sb.Attributes;
				if(!vi.ViewInfo.Ribbon.Enabled)
					res.Attributes = PaintHelper.RibbonDisabledAttributes;
				res.RightToLeft = vi.ViewInfo.IsRightToLeft;
				return res;
			}
			protected override void DrawObjectCore(SkinElementInfo info, ObjectInfoArgs e) {
				SkinGroupBorderObjectInfoArgs sb = e as SkinGroupBorderObjectInfoArgs;
				if(sb.GroupInfo.CaptionLocation == Locations.Top) {
					new RotateObjectPaintHelper().DrawRotated(info.Cache, info, SkinElementPainter.Default, RotateFlipType.RotateNoneFlipY);
				}
				else 
					base.DrawObjectCore(info, e);
			}
		}
		protected class SkinRibbonGroupButtonPainter : SkinCustomPainter {
			RibbonViewInfo viewInfo;
			public SkinRibbonGroupButtonPainter(ISkinProvider provider, RibbonViewInfo viewInfo)
				: base(provider) {
				this.viewInfo = viewInfo;
			}
			protected RibbonViewInfo ViewInfo { get { return viewInfo; } }
			public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
				return new Rectangle(0, 0, 13, 13);
			}
			protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
				SkinElementInfo res = new SkinElementInfo(RibbonSkins.GetSkin(Provider)[RibbonSkins.SkinTabPanelGroupButton], e.Bounds);
				res.State = e.State;
				res.ImageIndex = -1;
				if(!ViewInfo.Ribbon.Enabled)
					res.Attributes = PaintHelper.RibbonDisabledAttributes;
				return res;
			}
		}
	}
	internal static class ColoredRibbonElementsCache {
		[ThreadStatic]
		static Dictionary<Color, Image> contextTabHeaderPageImageHash;
		[ThreadStatic]
		static Dictionary<Color, Image> contextMacTabHeaderPageImageHash;
		[ThreadStatic]
		static Dictionary<Color, Image> contextTabPanelBorderImageHash;
		[ThreadStatic]
		static Dictionary<Color, Image> pageCategoryImageHash;
		[ThreadStatic]
		static Dictionary<Color, Image> pageCategorySeparatorImageHash;
		static Dictionary<Color, Image> ContextTabHeaderPageImageHash {
			get {
				if(contextTabHeaderPageImageHash == null)
					contextTabHeaderPageImageHash = new Dictionary<Color, Image>();
				return contextTabHeaderPageImageHash;
			}
		}
		static Dictionary<Color, Image> ContextMacTabHeaderPageImageHash {
			get {
				if(contextMacTabHeaderPageImageHash == null)
					contextMacTabHeaderPageImageHash = new Dictionary<Color, Image>();
				return contextMacTabHeaderPageImageHash;
			}
		}
		static Dictionary<Color, Image> ContextTabPanelBorderImageHash {
			get {
				if(contextTabPanelBorderImageHash == null)
					contextTabPanelBorderImageHash = new Dictionary<Color, Image>();
				return contextTabPanelBorderImageHash;
			}
		}
		static Dictionary<Color, Image> PageCategoryImageHash {
			get {
				if(pageCategoryImageHash == null)
					pageCategoryImageHash = new Dictionary<Color, Image>();
				return pageCategoryImageHash;
			}
		}
		static Dictionary<Color, Image> PageCategorySeparatorImageHash {
			get {
				if(pageCategorySeparatorImageHash == null)
					pageCategorySeparatorImageHash = new Dictionary<Color, Image>();
				return pageCategorySeparatorImageHash;
			}
		}
		static int CalcChannelValue(float middleVal, float val) {
			if(val < 0.5f) return (int)((middleVal - middleVal / 0.25f * (val - 0.5f) * (val - 0.5f)) * 255.0f);
			return (int)((middleVal + (1 - middleVal) / 0.25f * (val - 0.5f) * (val - 0.5f)) * 255.0f);
		}
		static Image GetColoredImage(SkinElementInfo info, Color color) {
			if(info.Element == null && !info.HasActualImage)
				return null;
			Bitmap img = info.GetActualImage().Clone() as Bitmap;
			Color c = Color.Empty;
			int r, g, b;
			float pr = color.R / 255.0f, pg = color.G / 255.0f, pb = color.B / 255.0f;
			for(int row = 0; row < img.Height; row++) {
				for(int col = 0; col < img.Width; col++) {
					c = img.GetPixel(col, row);
					r = CalcChannelValue(pr, c.R / 255.0f);
					g = CalcChannelValue(pg, c.G / 255.0f);
					b = CalcChannelValue(pb, c.B / 255.0f);
					img.SetPixel(col, row, Color.FromArgb(c.A, r, g, b));
				}
			}
			return img;
		}
		static Image GetCachedImage(Dictionary<Color, Image> dic, Color color, SkinElementInfo info) {
			Image res;
			dic.TryGetValue(color, out res);
			if(res == null) {
				res = GetColoredImage(info, color);
				dic[color] = res;
			}
			return res;
		}
		public static Image GetPageCategoryImage(Color color, SkinElementInfo info) {
			return GetCachedImage(PageCategoryImageHash, color, info);
		}
		public static Image GetTabHeaderPageImage(Color color, SkinElementInfo info) {
			return GetCachedImage(ContextTabHeaderPageImageHash, color, info);
		}
		public static Image GetMacTabHeaderPageImage(Color color, SkinElementInfo info) {
			return GetCachedImage(ContextMacTabHeaderPageImageHash, color, info);
		}
		public static Image GetTabPanelBorderImage(Color color, SkinElementInfo info) {
			return GetCachedImage(ContextTabPanelBorderImageHash, color, info);
		}
		public static Image GetPageCategorySeparatorImage(Color color, SkinElementInfo info) {
			return GetCachedImage(PageCategorySeparatorImageHash, color, info);
		}
		static void RemoveImage(Dictionary<Color, Image> dic, Color color) {
			Image res;
			dic.TryGetValue(color, out res);
			if(res == null) {
				color = Color.FromArgb(220, color);
				dic.TryGetValue(color, out res);
			}
			if(res != null)
				res.Dispose();
			dic.Remove(color);
		}
		public static void RemovePageCategoryImage(Color color) {
			RemoveImage(PageCategoryImageHash, color);
		}
		public static void RemoveTabHeaderPageImage(Color color) {
			RemoveImage(ContextTabHeaderPageImageHash, color);
		}
		public static void RemoveMacTabHeaderPageImage(Color color) {
			RemoveImage(ContextMacTabHeaderPageImageHash, color);
		}
		public static void RemoveTabPanelBorderImage(Color color) {
			RemoveImage(ContextTabPanelBorderImageHash, color);
		}
		public static void RemovePageCategorySeparatorImage(Color color) {
			RemoveImage(PageCategorySeparatorImageHash, color);
		}
		public static void RemoveColoredImages(Color color) {
			RemovePageCategoryImage(color);
			RemoveTabHeaderPageImage(color);
			RemoveMacTabHeaderPageImage(color);
			RemoveTabPanelBorderImage(color);
			RemovePageCategorySeparatorImage(color);
		}
		static void Clear(Dictionary<Color, Image> dic) {
			foreach(Image img in dic.Values) {
				if(img != null) 
					img.Dispose();
			}
			dic.Clear();
		}
		public static void ClearRibbonItems() {
			Clear(PageCategoryImageHash);
			Clear(ContextTabHeaderPageImageHash);
			Clear(ContextMacTabHeaderPageImageHash);
			Clear(ContextTabPanelBorderImageHash);
			Clear(PageCategorySeparatorImageHash);
		}
		public static void Clear() {
			ClearRibbonItems();
		}
		public static int PageCategoryImageCount { get { return PageCategoryImageHash.Count; } }
		public static int TabHeaderPageImageCount { get { return ContextTabHeaderPageImageHash.Count; } }
		public static int MacTabHeaderPageImageCount { get { return ContextMacTabHeaderPageImageHash.Count; } }
		public static int TabPanelBorderImageCount { get { return ContextTabPanelBorderImageHash.Count; } }
		public static int PageCategorySeparatorImageCount { get { return PageCategorySeparatorImageHash.Count; } }
	}
}
