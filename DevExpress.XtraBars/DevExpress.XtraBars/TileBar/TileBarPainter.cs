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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Navigation {
	public class TileBarPainter : TileControlPainter {
		protected override void DrawDropDownButton(TileControlInfoArgs e, TileItemViewInfo tileItemInfo) {
			TileBarItemViewInfo itemInfo = tileItemInfo as TileBarItemViewInfo;
			if(itemInfo == null) return;
			if(!itemInfo.ShouldDrawDropDown) return;
			DrawDropDownBackground(e, itemInfo);
			DrawDropDownSplitLine(e, itemInfo);
			DrawDropDownGlyph(e, itemInfo);
		}
		void DrawDropDownBackground(TileControlInfoArgs e, TileBarItemViewInfo itemInfo) {
			AppearanceObject currentAppearance;
			currentAppearance = itemInfo.DropDownAppearance;
			currentAppearance.DrawBackground(e.Cache, itemInfo.DropDownBounds);
		}
		void DrawDropDownSplitLine(TileControlInfoArgs e, TileBarItemViewInfo itemInfo) {
			e.Cache.Graphics.DrawLine(e.Cache.GetPen(itemInfo.DropDownSplitLineColor),
				itemInfo.DropDownSplitLineStartPt,
				itemInfo.DropDownSplitLineEndPt);
		}
		void DrawDropDownGlyph(TileControlInfoArgs e, TileBarItemViewInfo itemInfo) {
			ImageAttributes attr = ImageColorizer.GetColoredAttributes(itemInfo.DropDownAppearance.ForeColor);
			e.Cache.Graphics.DrawImage(itemInfo.DropDownGlyph, new Rectangle(itemInfo.DropDownGlyphPosition, itemInfo.DropDownGlyph.Size), 0, 0,
				itemInfo.DropDownGlyph.Width, itemInfo.DropDownGlyph.Height, GraphicsUnit.Pixel, attr);
		}
		protected override void DrawScrollArrows(TileControlInfoArgs e) {
			if((e.ViewInfo as TileBarViewInfo).CanDrawScrollButtons) {
				DrawBackArrow(e);
				DrawForwardArrow(e);
			}
		}
		protected override void DrawBackArrow(TileControlInfoArgs e) {
			TileBarViewInfo viewInfo = e.ViewInfo as TileBarViewInfo;
			if(!viewInfo.BackArrowVisible)
				return;
			e.Cache.Graphics.FillRectangle(new SolidBrush(e.ViewInfo.PaintAppearance.BackColor), e.ViewInfo.BackArrowBounds);
			ImageAttributes attr = ImageColorizer.GetColoredAttributes(viewInfo.ScrollArrowsColor);
			DrawImageColorizedCore(e, viewInfo.ScrollBackGlyph, new Rectangle(viewInfo.ScrollBackGlyphPosition, viewInfo.ScrollBackGlyph.Size), attr);
		}
		protected override void DrawForwardArrow(TileControlInfoArgs e) {
			if(!((TileBarViewInfo)e.ViewInfo).ForwardArrowVisible)
				return;
			var viewInfo = e.ViewInfo as TileBarViewInfo;
			e.Cache.Graphics.FillRectangle(new SolidBrush(e.ViewInfo.PaintAppearance.BackColor), e.ViewInfo.ForwardArrowBounds);
			ImageAttributes attr = ImageColorizer.GetColoredAttributes(viewInfo.ScrollArrowsColor);
			DrawImageColorizedCore(e, viewInfo.ScrollForwardGlyph, new Rectangle(viewInfo.ScrollForwardGlyphPosition, viewInfo.ScrollForwardGlyph.Size), attr);
		}
		void DrawScrollButtonHover(TileControlInfoArgs e, Rectangle bounds, float opacity) {
			TileBarViewInfo viewInfo = e.ViewInfo as TileBarViewInfo;
			e.Cache.FillRectangle(e.Cache.GetSolidBrush(viewInfo.GetScrollArrowHoverColor(opacity)), bounds);
		}
		protected override void DrawBorder(TileControlInfoArgs e) {
			DrawBeak(e);
			base.DrawBorder(e);
		}
		protected virtual void DrawBeak(TileControlInfoArgs e) {
			TileBarViewInfo viewInfo = e.ViewInfo as TileBarViewInfo;
			if(viewInfo.ShouldDrawBeak)
				e.Cache.Graphics.FillPolygon(new SolidBrush(viewInfo.GetDropDownWindowBackColor(viewInfo.Beak.ItemInfo)), viewInfo.GetBeakPoints());
		}
		protected override void DrawGroups(TileControlInfoArgs e) {
			DrawShadows(e);
			base.DrawGroups(e);
		}
		void DrawShadows(TileControlInfoArgs e) {
			TileBarViewInfo viewInfoCore = e.ViewInfo as TileBarViewInfo;
			if(!viewInfoCore.CanDrawShadows) return;
			foreach(TileGroupViewInfo group in e.ViewInfo.Groups) {
				foreach(TileItemViewInfo item in group.Items) {
					DrawItemShadow(e, item);
				}
			}
		}
		void DrawItemShadow(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			TileBarItemViewInfo item = itemInfo as TileBarItemViewInfo;
			if(item == null || !item.IsVisible || !item.ShouldShowItemShadow) return;
			AppearanceObject shadowAppearance = ((TileBarViewInfo)e.ViewInfo).ShadowAppearance;
			Rectangle shadowBounds = new Rectangle(itemInfo.Bounds.X, itemInfo.Bounds.Bottom, itemInfo.Bounds.Width, 25);
			shadowAppearance.DrawBackground(e.Cache, shadowBounds);
		}
		protected override void DrawSelected(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(itemInfo.IsSelected)
				DrawItemSelection(e, itemInfo);
		}
	}
}
