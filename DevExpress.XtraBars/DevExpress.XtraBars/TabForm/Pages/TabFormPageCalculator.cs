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

using DevExpress.Skins;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraBars {
	public class TabFormPageCalculator {
		TabFormControlViewInfoBase viewInfo;
		public TabFormPageCalculator(TabFormControlViewInfoBase viewInfo) {
			this.viewInfo = viewInfo;
		}
		public List<TabFormPageViewInfo> PageInfos { get { return ViewInfo.PageInfos; } }
		public TabFormPageViewInfo AddPageInfo { get { return ViewInfo.AddPageInfo; } }
		public TabFormControlViewInfoBase ViewInfo { get { return viewInfo; } }
		public TabFormPage AddPage { get { return ViewInfo.AddPage; } }
		public TabFormControlBase Owner { get { return ViewInfo.Owner; } }
		protected internal void Calculate() {
			int left = ViewInfo.LinkInfoProvider.TabLeftItemLinks.Bounds.Width + Owner.LeftTabIndent;
			Point p = new Point(left, ViewInfo.GetPageTop());
			int pageBestHeight = ViewInfo.CalcBestPageHeight();
			CalcPagesSize(pageBestHeight);
			for(int i = 0; i < PageInfos.Count; i++) {
				SetPageBounds(PageInfos[i], p);
				p.X += PageInfos[i].Bounds.Width;
				if(i != PageInfos.Count - 1)
					p.X += ViewInfo.GetDistanceBetweenTabs();
				else p.X += ViewInfo.GetDistanceToAddTab();
			}
			if(ViewInfo.ShouldShowAddPage()) {
				p = new Point(p.X, CalcAddPageTop(p.Y, pageBestHeight));
				SetPageBounds(AddPageInfo, p);
			}
		}
		protected int CalcAddPageTop(int pagesTop, int pageBestHeight) {
			SkinElement elem = ViewInfo.GetAddPageSkinElement();
			int offset = elem.Offset.Offset.Y;
			if(elem.Properties.GetBoolean("ShouldStickBottom", false))
				return pagesTop + offset + pageBestHeight - AddPageInfo.Bounds.Height;
			return pagesTop + offset + (pageBestHeight - AddPageInfo.Bounds.Height) / 2;
		}
		public void SetPageBounds(TabFormPageViewInfo pageInfo, Point loc) {
			SkinElement elem = ViewInfo.GetPageSkinElement();
			if(elem == null) {
				pageInfo.Bounds = Rectangle.Empty;
				pageInfo.TextContentBounds = Rectangle.Empty;
				return;
			}
			pageInfo.Bounds = new Rectangle(loc.X, loc.Y, pageInfo.BestWidth, pageInfo.Bounds.Height);
			CalcPageContent(pageInfo, elem);
		}
		public virtual void CalcPageContent(TabFormPageViewInfo pageInfo, SkinElement elem) {
			SkinPaddingEdges margins = ViewInfo.GetPageMargins(elem);
			int contentWidth = pageInfo.Bounds.Width - margins.Width;
			int contentHeight = pageInfo.Bounds.Height - margins.Height;
			int textOffset = 0;
			int closeButtonWidth = CalcCloseButtonWidth(pageInfo, margins, contentWidth, contentHeight);
			if(pageInfo.Page.GetImage() != null && contentWidth - closeButtonWidth >= ViewInfo.ImageSize.Width) {
				int imageTop = margins.Top + (contentHeight - ViewInfo.ImageSize.Height) / 2;
				pageInfo.ImageContentBounds = new Rectangle(margins.Left, imageTop, ViewInfo.ImageSize.Width, ViewInfo.ImageSize.Height);
				textOffset += ViewInfo.ImageSize.Width + Owner.ViewInfo.GetImageToTextIndent();
			}
			else pageInfo.ImageContentBounds = Rectangle.Empty;
			int textWidth = Math.Max(0, pageInfo.Bounds.Width - margins.Width - textOffset - closeButtonWidth);
			pageInfo.TextContentBounds = new Rectangle(margins.Left + textOffset, margins.Top, textWidth, contentHeight);
		}
		public virtual int CalcCloseButtonWidth(TabFormPageViewInfo pageInfo, SkinPaddingEdges margins, int contentWidth, int contentHeight) {
			if(pageInfo.Page.ShouldShowCloseButton()) {
				Point closeButtonOffset = ViewInfo.GetCloseButtonOffset();
				Size closeButtonSize = ViewInfo.GetCloseButtonSize();
				if(contentWidth >= closeButtonSize.Width) {
					int closeButtonTop = margins.Top + (contentHeight - closeButtonSize.Height) / 2 + closeButtonOffset.Y;
					int closeButtonLeft = pageInfo.Bounds.Width - margins.Right - closeButtonSize.Width;
					pageInfo.CloseButtonBounds = new Rectangle(closeButtonLeft, closeButtonTop, closeButtonSize.Width, closeButtonSize.Height);
					return pageInfo.CloseButtonBounds.Width + closeButtonOffset.X;
				}
				else pageInfo.CloseButtonBounds = Rectangle.Empty;
			}
			else pageInfo.CloseButtonBounds = Rectangle.Empty;
			return 0;
		}
		protected void CalcPagesSize(int pageBestHeight) {
			SkinElement elem = ViewInfo.GetPageSkinElement();
			int horizontalMargin = ViewInfo.GetPageMargins(elem).Width;
			int bestWidth = CalcPagesBestWidth(pageBestHeight, horizontalMargin);
			int availableWidth = CalcAvailableWidth();
			int overflow = bestWidth - availableWidth;
			if(overflow <= 0) return;
			if(PagesReduction(horizontalMargin, overflow)) return;
			HideExtraPages(availableWidth);
		}
		protected int CalcAvailableWidth() {
			int linksWidth = ViewInfo.LinkInfoProvider.TabLeftItemLinks.Bounds.Width + ViewInfo.LinkInfoProvider.TabRightItemLinks.Bounds.Width;
			return ViewInfo.ClientRect.Width - linksWidth - ViewInfo.Owner.LeftTabIndent - ViewInfo.Owner.RightTabIndent;
		}
		protected void HideExtraPages(int availableWidth) {
			int maxPagesWidth = availableWidth;
			if(ViewInfo.ShouldShowAddPage())
				maxPagesWidth -= (AddPageInfo.Bounds.Width + ViewInfo.GetDistanceToAddTab());
			RemoveExtraPageInfos(maxPagesWidth);
		}
		protected void RemoveExtraPageInfos(int maxPagesWidth) {
			int distance = ViewInfo.GetDistanceBetweenTabs();
			int width = -distance;
			TabFormPageViewInfo selectedPage = ViewInfo.GetPageInfo(Owner.SelectedPage);
			if(selectedPage != null) {
				width += selectedPage.BestWidth + distance;
				if(width > maxPagesWidth) {
					PageInfos.Clear();
					return;
				}
			}
			for(int i = 0; i < PageInfos.Count;) {
				if(!object.Equals(PageInfos[i], selectedPage)) {
					width += PageInfos[i].BestWidth + distance;
					if(width > maxPagesWidth) {
						PageInfos.Remove(PageInfos[i]);
						continue;
					}
				}
				i++;
			}
		}
		protected bool PagesReduction(int minWidth, int overflow) {
			if(PageInfos.Count == 0) return true;
			int maxWidth = CalcMaxPageWidth();
			while(overflow > 0) {
				int decrement = overflow / PageInfos.Count;
				decrement = Math.Max(1, Math.Min(maxWidth - minWidth, decrement));
				foreach(TabFormPageViewInfo pageInfo in PageInfos) {
					int newPageWidth = Math.Min(maxWidth - decrement, pageInfo.BestWidth);
					if(newPageWidth < minWidth) return false;
					if(newPageWidth == pageInfo.BestWidth) continue;
					overflow -= pageInfo.BestWidth - newPageWidth;
					pageInfo.BestWidth = newPageWidth;
					if(overflow <= 0) return true;
				}
				maxWidth -= decrement;
			}
			return true;
		}
		protected int CalcMaxPageWidth() {
			int maxWidth = 0;
			foreach(TabFormPageViewInfo pageInfo in PageInfos) {
				maxWidth = Math.Max(maxWidth, pageInfo.BestWidth);
			}
			return maxWidth;
		}
		protected int CalcPagesBestWidth(int pageBestHeight, int horizontalMargin) {
			int res = 0;
			foreach(TabFormPageViewInfo pageInfo in PageInfos) {
				CalcPageBestSize(pageInfo, false, pageBestHeight, horizontalMargin);
				res += pageInfo.Bounds.Width;
			}
			if(ViewInfo.ShouldShowAddPage()) {
				SkinElement elem = ViewInfo.GetAddPageSkinElement();
				int addPageHeight = ViewInfo.CalcAddPageHeight(elem);
				CalcPageBestSize(AddPageInfo, true, addPageHeight, elem.ContentMargins.Width);
				res += AddPageInfo.Bounds.Width;
				res += ViewInfo.GetDistanceToAddTab();
			}
			res += ViewInfo.GetDistanceBetweenTabs() * (PageInfos.Count - 1);
			return res;
		}
		protected int GetContentBestWidth(TabFormPageViewInfo pageInfo, bool isAddPage) {
			if(isAddPage) {
				return ViewInfo.CalcAddPageContentSize(ViewInfo.GetAddPageSkinElement()).Width;
			}
			return CalcPageContentBestWidth(pageInfo);
		}
		protected void CalcPageBestSize(TabFormPageViewInfo pageInfo, bool isAddPage, int pageHeight, int pageWidthMargin) {
			ViewInfo.UpdatePagePaintAppearance(pageInfo);
			int contentWidth = GetContentBestWidth(pageInfo, isAddPage);
			Size pageSize = new Size(pageWidthMargin + contentWidth, pageHeight);
			pageInfo.Bounds = new Rectangle(Point.Empty, pageSize);
			pageInfo.BestWidth = pageSize.Width;
		}
		protected virtual int CalcPageContentBestWidth(TabFormPageViewInfo pageInfo) {
			int contentWidth = CalcBestPageTextSize(pageInfo).Width;
			if(pageInfo.Page.ShouldShowCloseButton())
				contentWidth += ViewInfo.GetCloseButtonSize().Width + ViewInfo.GetCloseButtonOffset().X;
			int maxWidth = ViewInfo.Owner.MaxTabWidth - ViewInfo.GetPageMargins(ViewInfo.GetPageSkinElement()).Width;
			if(maxWidth > 0)
				contentWidth = Math.Min(maxWidth, contentWidth);
			Image img = pageInfo.Page.GetImage();
			if(img == null) return contentWidth;
			return ViewInfo.ImageSize.Width + contentWidth + Owner.ViewInfo.GetImageToTextIndent();
		}
		Size CalcBestPageTextSize(TabFormPageViewInfo pageInfo) {
			return pageInfo.PaintAppearance.CalcTextSizeInt(ViewInfo.GInfo.Graphics, pageInfo.Page.Text, 0);
		}
	}
}
