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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraTab.Drawing {
	public class TabDrawArgs : GraphicsInfoArgs {
		BaseTabControlViewInfo viewInfo;
		public TabDrawArgs(GraphicsCache graphicsCache, BaseTabControlViewInfo viewInfo, Rectangle bounds) : base(graphicsCache, bounds) {
			this.viewInfo = viewInfo;
		}
		public virtual BaseTabControlViewInfo ViewInfo { get { return viewInfo; } }
	}
	public class BaseTabPainter {
		IXtraTab tabControl;
		public BaseTabPainter(IXtraTab tabControl) {
			this.tabControl = tabControl;
		}
		protected bool IsNeedDrawRect(TabDrawArgs e, Rectangle r) {
			if(r.IsEmpty) return false;
			if(e.PaintArgs.ClipRectangle.IsEmpty) return true;
			if(e.PaintArgs.ClipRectangle.IntersectsWith(r)) 
				return true;
			return false;
		}
		public IXtraTab TabControl { get { return tabControl; } }
		public virtual void Draw(TabDrawArgs e) {
			if(!IsNeedDrawRect(e, e.Bounds)) return;
			DrawBackground(e);
			DrawForeground(e);
		}
		protected virtual Rectangle GetPageClientDrawBounds(TabDrawArgs e, BaseTabPageViewInfo pageInfo) {
			BaseTabControlViewInfo vi = e.ViewInfo as BaseTabControlViewInfo;
			Rectangle bounds = new Rectangle(e.Bounds.Location, vi.PageBounds.Size);
			bounds.Offset(vi.PageBounds.X - vi.PageClientBounds.X, vi.PageBounds.Y - vi.PageClientBounds.Y);
			bounds.Size = new Size(e.Bounds.Width + (vi.PageBounds.Width - vi.PageClientBounds.Width),
				e.Bounds.Height + (vi.PageBounds.Height - vi.PageClientBounds.Height));
			return bounds;
		}
		public virtual void DrawPageClientControl(TabDrawArgs info, BaseTabPageViewInfo pageInfo) { 
			Rectangle bounds = info.Bounds;
			pageInfo.PaintAppearanceClient.DrawBackground(info.Graphics, info.Cache, bounds);
		}
		protected virtual void DrawBackground(TabDrawArgs e) {
			GraphicsState state = null;
			if(!e.ViewInfo.FillPageClient) {
				state = e.Graphics.Save();
				e.Graphics.ExcludeClip(e.ViewInfo.PageClientBounds);
			}
			e.ViewInfo.TabControlBorderPainter.DrawObject(new TabBorderObjectInfoArgs(e.ViewInfo, e.Cache, e.ViewInfo.PaintAppearance, e.ViewInfo.Bounds));
			e.ViewInfo.PaintAppearance.FillRectangle(e.Cache, e.ViewInfo.Client);
			if(state != null) e.Graphics.Restore(state);
		}
		protected virtual void DrawForeground(TabDrawArgs e) {
			DrawTabPage(e);
			DrawHeader(e);
		}
		protected virtual void DrawTabPage(TabDrawArgs e) {
			if(!IsNeedDrawRect(e, e.ViewInfo.PageBounds)) return;
			BaseTabPageViewInfo page = e.ViewInfo.HeaderInfo.AllPages[e.ViewInfo.SelectedTabPage];
			AppearanceObject appearance = page == null ? e.ViewInfo.PaintAppearance : page.PaintAppearanceClient;
			ObjectInfoArgs args = new TabBorderObjectInfoArgs(e.ViewInfo, e.Cache, appearance, e.ViewInfo.PageBounds);
			e.ViewInfo.PageClientBorderPainter.DrawObject(args);
			if(e.ViewInfo.FillPageClient) {
				appearance.FillRectangle(e.Cache, e.ViewInfo.PageClientBounds);
			}
		}
		protected virtual void DrawHeader(TabDrawArgs e) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			if(hInfo.Bounds.IsEmpty || !e.ViewInfo.IsShowHeader) return;
			if(!IsNeedDrawRect(e, hInfo.Bounds)) return;
			DrawHeaderBackground(e);
			Rectangle clipBounds = GetHeaderClipBounds(e);
			if(SkinElementPainter.CorrectByRTL)
				clipBounds.X--;
			GraphicsClipState clipState = e.Cache.ClipInfo.SaveClip();
			if(!clipBounds.IsEmpty) {
				e.Cache.ClipInfo.SetClip(e.Cache.ClipInfo.CheckBounds(clipBounds));
			}
			ExcludeHeaderButtonsClip(e);
			foreach(BaseTabRowViewInfo rowInfo in hInfo.Rows) {
				DrawHeaderRow(e, rowInfo);
			}
			e.Cache.ClipInfo.RestoreClipRelease(clipState);
			UpdateClipRegion(e.Graphics);
			DrawHeaderButtons(e);
		}
		protected virtual Rectangle GetHeaderClipBounds(TabDrawArgs e) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			Rectangle r = Rectangle.Inflate(hInfo.Client, hInfo.IsSideLocation ? 1 : 0, hInfo.IsSideLocation ? 0 : 1); 
			Rectangle buttons = hInfo.ButtonsBounds;
			r.Size = hInfo.SetSizeWidth(r.Size, hInfo.GetSizeWidth(r.Size) - hInfo.GetSizeWidth(buttons.Size));
			return r;
		}
		protected virtual void DrawHeaderBackground(TabDrawArgs e) {
			e.ViewInfo.HeaderBorderPainter.DrawObject(new TabBorderObjectInfoArgs(e.ViewInfo, e.Cache, e.ViewInfo.HeaderInfo.PaintAppearance, e.ViewInfo.HeaderInfo.Bounds));
		}
		protected virtual void DrawHeaderButtons(TabDrawArgs e) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			if(hInfo.ButtonsBounds.IsEmpty) return;
			hInfo.Buttons.Draw(e.Cache);
		}
		protected virtual void ExcludeHeaderButtonsClip(TabDrawArgs e) {
			e.Cache.ClipInfo.ExcludeClip(e.ViewInfo.HeaderInfo.ButtonsBounds);
		}
		protected virtual void DrawHeaderRow(TabDrawArgs e, BaseTabRowViewInfo rowInfo) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			DrawHeaderRowBackground(e, rowInfo);
			BaseTabPageViewInfo selPage = e.ViewInfo.SelectedTabPageViewInfo;
			bool needDrawSelPage = false;
			for(int n = hInfo.UseReversePainter ? rowInfo.Pages.Count - 1 : 0; hInfo.UseReversePainter ? n >= 0 : n < rowInfo.Pages.Count; n += hInfo.UseReversePainter ? -1 : 1) {
				BaseTabPageViewInfo pInfo = rowInfo.Pages[n];
				if(!pInfo.AllowDraw || !rowInfo.Bounds.IntersectsWith(pInfo.Bounds)) continue;
				if(selPage == pInfo && hInfo.DrawSelectedPageLast) {
					needDrawSelPage = true;
					continue;
				}
				DrawPage(e, rowInfo, pInfo);
			}
			if(needDrawSelPage) DrawPage(e, rowInfo, selPage);
		}
		protected void DrawPage(TabDrawArgs e, BaseTabRowViewInfo rowInfo, BaseTabPageViewInfo pInfo) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			if(e.ViewInfo.IsLockUpdate && pInfo.Page.TabControl == null)
				return;
			GraphicsState state = null;
			if(pInfo.ClipRegion != null) {
				state = e.Graphics.Save();
				e.Graphics.SetClip(pInfo.ClipRegion, CombineMode.Intersect);
			}
			UpdateClipRegion(e.Graphics);
			DrawHeaderPage(e, rowInfo, pInfo);
			DrawHeaderPageControlBox(e, pInfo);
			if(state != null) {
				e.Graphics.Restore(state);
			}
			if(hInfo.ExcludeFromClipping(pInfo)) {
				e.Graphics.ExcludeClip(pInfo.Bounds);
			}
			UpdateClipRegion(e.Graphics);
		}
		protected virtual void DrawHeaderRowBackground(TabDrawArgs e, BaseTabRowViewInfo rowInfo) {
			e.ViewInfo.HeaderRowBorderPainter.DrawObject(new TabBorderObjectInfoArgs(e.ViewInfo, e.Cache, e.ViewInfo.HeaderInfo.PaintAppearance, rowInfo.Bounds));
		}
		protected virtual void UpdateClipRegion(Graphics g) {
		}
		protected internal static Border3DSide CalcBorderSides(BaseTabControlViewInfo viewInfo) {
			Border3DSide sides;
			if(viewInfo == null) return Border3DSide.All;
			if(viewInfo.HeaderInfo.IsSideLocation) {
				sides = (Border3DSide.Top | Border3DSide.Bottom);
				sides |= viewInfo.HeaderInfo.IsLeftLocation ? Border3DSide.Left : Border3DSide.Right;
			} else {
				sides = (Border3DSide.Left | Border3DSide.Right);
				sides |= viewInfo.HeaderInfo.IsTopLocation ? Border3DSide.Top : Border3DSide.Bottom;
			}
			return sides;
		}
		protected virtual void DrawHeaderBorder(TabDrawArgs e, BaseTabPageViewInfo pInfo, ref Rectangle r) {
			Border3DSide sides = CalcBorderSides(e.ViewInfo);
			StandardDrawHeaderBorder(e, pInfo, ref r, sides);
		}
		protected virtual void DrawHeaderPage(TabDrawArgs e, BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo) {
			Rectangle r = pInfo.Bounds;
			DrawHeaderBorder(e, pInfo, ref r);
			DrawHeaderBackground(e, pInfo, r);
			DrawHeaderFocus(e, pInfo);
			DrawHeaderPageImageText(e, pInfo);
		}
		protected virtual void DrawHeaderFocus(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			if(e.ViewInfo.SelectedTabPage == pInfo.Page && e.ViewInfo.IsTabHeaderFocused) {
				e.Cache.Paint.DrawFocusRectangle(e.Graphics, pInfo.Focus, pInfo.PaintAppearance.ForeColor, pInfo.PaintAppearance.BackColor);
			}
		}
		protected virtual void DrawHeaderPageControlBox(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			if(pInfo.ButtonsPanel == null || pInfo.ButtonsPanel.ViewInfo == null) return;
			ObjectState controlBoxState = ObjectState.Normal;
			if(pInfo.ButtonsPanel.Handler.HotButton != null)
				controlBoxState = ObjectState.Hot;
			((ObjectInfoArgs)pInfo.ButtonsPanel.ViewInfo).Cache = e.Cache;
			DevExpress.XtraEditors.ButtonPanel.BaseTabButtonsPanelPainter painter = pInfo.ButtonsPanel.Owner.GetPainter() as DevExpress.XtraEditors.ButtonPanel.BaseTabButtonsPanelPainter;
			if(painter != null) {
				if(controlBoxState == ObjectState.Normal && !pInfo.IsHotState && (pInfo.DisableDrawCloseButton || pInfo.DisableDrawPinButton)) {
					if(!pInfo.DisableDrawCloseButton)
						painter.DrawCloseButton((ObjectInfoArgs)pInfo.ButtonsPanel.ViewInfo);
					if(!pInfo.DisableDrawPinButton)
						painter.DrawPinButton((ObjectInfoArgs)pInfo.ButtonsPanel.ViewInfo);
					painter.DrawCheckedButton((ObjectInfoArgs)pInfo.ButtonsPanel.ViewInfo);
				}
				else
					ObjectPainter.DrawObject(e.Cache, pInfo.ButtonsPanel.Owner.GetPainter(), (ObjectInfoArgs)pInfo.ButtonsPanel.ViewInfo);
			}
		}
		protected virtual void StandardDrawHeaderBorder(TabDrawArgs e, BaseTabPageViewInfo pInfo, ref Rectangle bounds, Border3DSide sides) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			AppearanceObject style = pInfo.PaintAppearance; 
			BBrushes brushes = new BBrushes(e.Cache, style);
			bool left = (sides & Border3DSide.Left) != 0, right = (sides & Border3DSide.Right) != 0,
				top = (sides & Border3DSide.Top) != 0, bottom = (sides & Border3DSide.Bottom) != 0;
			int hLinesCount = (left ? 1 : 0) + (right ? 1 : 0),
				vLinesCount = (top ? 1 : 0) + (bottom ? 1 : 0);
			Rectangle r = new Rectangle(bounds.X, bounds.Y, 1, bounds.Height - vLinesCount * 2);
			if(top) r.Y += 2;
			if(pInfo.IsActiveState && hInfo.IsTopLocation) r.Height --;
			if(left) {
				e.Paint.FillRectangle(e.Graphics, brushes.LightLight, r);
				r.X ++;
				e.Paint.FillRectangle(e.Graphics, brushes.Light, r);
			}
			if(right) {
				r.X = bounds.Right - 2;
				e.Paint.FillRectangle(e.Graphics, brushes.Dark, r);
				r.X ++;
				e.Paint.FillRectangle(e.Graphics, brushes.DarkDark, r);
			}
			r.X = bounds.X + 1; r.Y = bounds.Y + 1;
			r.Size = new Size(1, 1);
			if(left && top) e.Paint.FillRectangle(e.Graphics, brushes.LightLight, r); 
			r.X = bounds.Right - 2;
			if(right && top) e.Paint.FillRectangle(e.Graphics, brushes.DarkDark, r); 
			r.Y = bounds.Bottom - 2;
			if(right && bottom) e.Paint.FillRectangle(e.Graphics, brushes.DarkDark, r); 
			r.X = bounds.X + 1;
			if(left && bottom) e.Paint.FillRectangle(e.Graphics, brushes.LightLight, r); 
			r.Width = bounds.Width - hLinesCount * 2;
			r.X = bounds.X;
			r.Y = bounds.Y;
			if(left) r.X = bounds.X + 2;
			if(top) {
				e.Paint.FillRectangle(e.Graphics, brushes.LightLight, r);
				r.Y ++;
				e.Paint.FillRectangle(e.Graphics, brushes.Light, r);
			}
			if(bottom) {
				r.Y = bounds.Bottom - 2;
				e.Paint.FillRectangle(e.Graphics, brushes.Dark, r);
				r.Y ++;
				e.Paint.FillRectangle(e.Graphics, brushes.DarkDark, r);
			}
			if(left) { bounds.X += 2; bounds.Width -= 2; }
			if(right) bounds.Width -= 2;
			if(top) { bounds.Y += 2; bounds.Height -= 2; }
			if(bottom) bounds.Height -= 2;
		}
		protected virtual void DrawHeaderBackground(TabDrawArgs e, BaseTabPageViewInfo pInfo, Rectangle bounds) {
			pInfo.PaintAppearance.FillRectangle(e.Cache, bounds);
		}
		protected virtual void DrawHeaderPageImageText(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			DrawHeaderPageText(e, pInfo);
			DrawHeaderPageImage(e, pInfo);
		}
		protected virtual void DrawHeaderPageImage(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			Size imgSize = pInfo.ImageSize;
			if(imgSize.IsEmpty || pInfo.Image.Size.IsEmpty) return;
			Rectangle r = pInfo.Image;
			if(pInfo.IsEmptyImagePadding) {
				r.X += (r.Width - imgSize.Width) / 2;
				r.Y += (r.Height - imgSize.Height) / 2;
			}
			else {
				r.X += pInfo.Page.ImagePadding.Left;
				r.Y += pInfo.Page.ImagePadding.Top;
			}
			if(GetAllowGlyphSkinning(e, pInfo)) {
				var attributes = ImageColorizer.GetColoredAttributes(CheckHeaderPageForeColor(e, pInfo));
				if(pInfo.Page.Image != null)
					e.Cache.Paint.DrawImage(e.Graphics, pInfo.Page.Image, r, new Rectangle(Point.Empty, imgSize), attributes);
				else {
					ImageCollection.DrawImageListImage(e.Cache, pInfo.Page.TabControl.Images, pInfo.Page.ImageIndex, r, attributes);
				}
			}
			else {
				if(pInfo.Page.Image != null)
					e.Cache.Paint.DrawImage(e.Graphics, pInfo.Page.Image, r.X, r.Y, new Rectangle(Point.Empty, imgSize), pInfo.Page.PageEnabled);
				else {
					ImageCollection.DrawImageListImage(e, pInfo.Page.TabControl.Images, pInfo.Page.ImageIndex, r, pInfo.Page.PageEnabled);
				}
			}
		}
		protected virtual void DrawHeaderPageText(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			int angle = 0;
			if(e.ViewInfo.HeaderInfo.RealPageOrientation == TabOrientation.Vertical) {
				angle = 90;
				if(e.ViewInfo.HeaderInfo.IsLeftLocation || e.ViewInfo.HeaderInfo.IsTopLocation) angle = 270;
			}
			AppearanceObject a = pInfo.PaintAppearance;
			System.Drawing.Text.HotkeyPrefix? hotKeyPrefixOverride = (a.TextOptions.HotkeyPrefix == HKeyPrefix.Default) && pInfo.UseHotkeyPrefixDrawModeOverride ?
				new System.Drawing.Text.HotkeyPrefix?(pInfo.HotkeyPrefixDrawModeOverride) : null;
			DrawVString(e.Cache, pInfo.Page.Text,
				a.GetFont(), e.Cache.GetSolidBrush(CheckHeaderPageForeColor(e, pInfo)),
				a.GetStringFormat(), pInfo.Text, angle, hotKeyPrefixOverride);
		}
		protected virtual bool GetAllowGlyphSkinning(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			return (pInfo.PageState != ObjectState.Disabled) &&
				(e.ViewInfo.HeaderInfo.GetAllowGlyphSkinning() ||
				e.ViewInfo.HeaderInfo.GetAllowGlyphSkinning(pInfo));
		}
		protected virtual Color CheckHeaderPageForeColor(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			return pInfo.PaintAppearance.ForeColor;
		}
		void DrawVString(GraphicsCache cache, string text, Font font, Brush foreBrush, StringFormat format, Rectangle bounds, int angle, 
			System.Drawing.Text.HotkeyPrefix? hotkeyDrawModeOverride) {
			using(StringFormat strFormat = format.Clone() as StringFormat) {
				if(hotkeyDrawModeOverride.HasValue)
					strFormat.HotkeyPrefix = hotkeyDrawModeOverride.Value;
				cache.DrawVString(text, font, foreBrush, bounds, strFormat, angle);
			}
		}
	}
	public class RotatedTabBorderPainter : RotatedBorderSidePainter {
		protected override int GetDegree(ObjectInfoArgs e) {
			TabBorderObjectInfoArgs ee = e as TabBorderObjectInfoArgs;
			if(ee == null) return 0;
			switch(ee.ViewInfo.HeaderInfo.HeaderLocation) {
				case TabHeaderLocation.Right : return 90;
				case TabHeaderLocation.Bottom : return 180;
				case TabHeaderLocation.Left : return 270;
			}
			return 0;
		}
	}
	public class Style3DTabPainter : BaseTabPainter {
		public Style3DTabPainter(IXtraTab tabControl) : base(tabControl) { }
		protected override Rectangle GetHeaderClipBounds(TabDrawArgs e) {
			Rectangle res = base.GetHeaderClipBounds(e);
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			if(hInfo.IsSideLocation) {
				res.Width ++;
				if(hInfo.IsRightLocation) res.X --;
			}
			else {
				res.Height ++;
				if(hInfo.IsBottomLocation) res.Y --;
			}
			return res;
		}
	}
}
