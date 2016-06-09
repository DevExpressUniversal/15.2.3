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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraTab.ViewInfo {
	public class SkinTabHeaderViewInfo : BaseTabHeaderViewInfo {
		public SkinTabHeaderViewInfo(BaseTabControlViewInfo viewInfo) : base(viewInfo) { }
		protected override Rectangle GetPageClipRectangle(BaseTabPageViewInfo info) { return Rectangle.Empty; }
		public override bool DrawSelectedPageLast { get { return true; } }
		static int magicLayoutSpacing = 2;
		public override void Dispose() {
			foreach(BaseTabPageViewInfo pageInfo in AllPages) {
				ColoredTabElementsCache.Reset(pageInfo.PaintAppearance.BackColor);
			}
			base.Dispose();
		}
		protected internal override int GetButtonsIndent() {
			Skin skin = CommonSkins.GetSkin(TabControl.LookAndFeel);
			if(skin != null) {
				SkinElement tab = skin[CommonSkins.SkinLayoutTabbedGroupSpacing];
				if(tab != null) {
					return magicLayoutSpacing + (IsSideLocation ? -tab.ContentMargins.Bottom : -tab.ContentMargins.Right);
				}
			}
			return 0;
		}
		protected internal SkinTabPageObjectInfo UpdatePageInfo(BaseTabPageViewInfo pInfo) {
			SkinTabControlViewInfo vi = ViewInfo as SkinTabControlViewInfo;
			SkinTabPageObjectInfo info = new SkinTabPageObjectInfo(vi.SkinHeader, pInfo);
			info.HeaderLocation = HeaderLocation;
			info.ImageIndex = 0;
			if((pInfo.PageState & ObjectState.Selected) != 0) {
				info.ImageIndex = 2;
				if(vi.AllowInactiveState && !vi.IsActive) {
					if(info.Element.Image != null && info.Element.Image.ImageCount > 3)
						info.ImageIndex = 4;
				}
			}
			else
				if((pInfo.PageState & ObjectState.Disabled) != 0)
					info.ImageIndex = 3;
				else
					if((pInfo.PageState & ObjectState.Hot) != 0)
						info.ImageIndex = 1;
			return info;
		}
		protected override Rectangle CalcRowBoundsByClient(BaseTabRowViewInfo row, Rectangle rowClient) {
			SkinTabControlViewInfo vi = ViewInfo as SkinTabControlViewInfo;
			Rectangle res = base.CalcRowBoundsByClient(row, rowClient);
			int near = vi.Skin.Properties.GetInteger(TabSkinProperties.RowIndentNear),
				far = (vi.Skin.Properties.GetInteger(TabSkinProperties.RowIndentNear) + vi.Skin.Properties.GetInteger(TabSkinProperties.RowIndentFar));
			if(IsSideLocation) {
				res.Y -= near;
				res.Height += far;
			} else {
				res.X -= near;
				res.Width += far;
			}
			return res;
		}
		protected override Rectangle CalcRowClientBounds(BaseTabRowViewInfo row, Rectangle headerClient) {
			SkinTabControlViewInfo vi = ViewInfo as SkinTabControlViewInfo;
			Rectangle res = base.CalcRowClientBounds(row, headerClient);
			int near = vi.Skin.Properties.GetInteger(TabSkinProperties.RowIndentNear),
				far = (vi.Skin.Properties.GetInteger(TabSkinProperties.RowIndentNear) + vi.Skin.Properties.GetInteger(TabSkinProperties.RowIndentFar));
			if(IsSideLocation) {
				res.Y += near;
				res.Height -= far;
			} else {
				res.X += near;
				res.Width -= far;
			}
			return res;
		}
		protected internal override EditorButtonPainter OnHeaderButtonGetPainter(TabButtonInfo button) {
			if(button.ButtonType == TabButtonType.User)
				return new SkinTabCustomHeaderButtonPainter(ViewInfo as SkinTabControlViewInfo, button.Button.Kind);
			return new SkinTabHeaderButtonPainter(ViewInfo as SkinTabControlViewInfo);
		}
		SkinElement GetSkinElement(BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo) {
			SkinTabControlViewInfo vi = ViewInfo as SkinTabControlViewInfo;
			return vi.SkinHeader;
		}
		protected SkinElement GetSkinElement(BaseTabPageViewInfo info) {
			return GetSkinElement(info.Row, info);
		}
		protected int GetTabContentMargin(IndentType indent, BaseTabPageViewInfo info) {
			SkinElement tab = GetSkinElement(info);
			int result = int.MinValue;
			switch(indent) {
				case IndentType.Left: result = tab.ContentMargins.Left; break;
				case IndentType.Right: result = tab.ContentMargins.Right; break;
				case IndentType.Top: result = tab.ContentMargins.Top; break;
				case IndentType.Bottom: result = tab.ContentMargins.Bottom; break;
			}
			if(result != int.MinValue) result += CalcAdditionalIndent(info, indent);
			return result;
		}
		protected int CalcAdditionalIndent(BaseTabPageViewInfo info, IndentType indent) {
			if(HeaderLocation == TabHeaderLocation.Top && RealPageOrientation == TabOrientation.Vertical && indent == IndentType.Top) return 5;
			if(HeaderLocation == TabHeaderLocation.Bottom && RealPageOrientation == TabOrientation.Vertical && indent == IndentType.Bottom) return 5;
			if(HeaderLocation == TabHeaderLocation.Left && RealPageOrientation == TabOrientation.Horizontal && indent == IndentType.Top) return 5;
			if(HeaderLocation == TabHeaderLocation.Right && RealPageOrientation == TabOrientation.Horizontal && indent == IndentType.Top) return 5;
			return 0;
		}
		protected override int CalcHPageIndent(BaseTabPageViewInfo info, IndentType indent) {
			if(IsSideLocation) return CalcVPageIndent(info, indent);
			int result = GetTabContentMargin(indent, info);
			if(result != int.MinValue) return result;
			return base.CalcHPageIndent(info, indent);
		}
		protected override int CalcVPageIndent(BaseTabPageViewInfo info, IndentType indent) {
			if(!IsSideLocation) return CalcHPageIndent(info, indent);
			int result = int.MinValue;
			switch(indent) {
				case IndentType.Top: result = IsLeftLocation ? GetTabContentMargin(IndentType.Right, info) : GetTabContentMargin(IndentType.Left, info); break;
				case IndentType.Bottom: result = IsLeftLocation ? GetTabContentMargin(IndentType.Left, info) : GetTabContentMargin(IndentType.Right, info); break;
				case IndentType.Left: result = IsLeftLocation ? GetTabContentMargin(IndentType.Top, info) : GetTabContentMargin(IndentType.Bottom, info); break;
				case IndentType.Right: result = IsLeftLocation ? GetTabContentMargin(IndentType.Bottom, info) : GetTabContentMargin(IndentType.Top, info); break;
			}
			if(result != int.MinValue) return result;
			return base.CalcVPageIndent(info, indent);
		}
		public override bool DefaultShowHeaderFocus { get { return false; } }
		protected virtual string GetHeaderDownGrow(BaseTabPageViewInfo info) {
			if(info.Page == info.ViewInfo.SelectedTabPage) {
				if(HeaderLocation == TabHeaderLocation.Top || HeaderLocation == TabHeaderLocation.Left) return TabSkinProperties.SelectedHeaderDownGrow;
				return TabSkinProperties.SelectedHeaderDownGrowBottomRight;
			}
			if(info.Row.Header.Rows.IndexOf(info.Row) != info.Row.Header.Rows.Count - 1) {
				if(HeaderLocation == TabHeaderLocation.Top || HeaderLocation == TabHeaderLocation.Left) return TabSkinProperties.UpperHeaderDownGrow;
				return TabSkinProperties.UpperHeaderDownGrowBottomRight;
			}
			if(HeaderLocation == TabHeaderLocation.Top || HeaderLocation == TabHeaderLocation.Left) return TabSkinProperties.HeaderDownGrow;
			return TabSkinProperties.HeaderDownGrowBottomRight;
		}
		protected virtual string GetContentIndent() {
			if(HeaderLocation == TabHeaderLocation.Left) return TabSkinProperties.LeftContentIndent;
			if(HeaderLocation == TabHeaderLocation.Right) return TabSkinProperties.RightContentIndent;
			if(HeaderLocation == TabHeaderLocation.Bottom) return TabSkinProperties.BottomContentIndent;
			return string.Empty;
		}
		protected override int BorderToTabHeadersIndent {
			get {
				Skins.Skin tabSkin = Skins.TabSkins.GetSkin(TabControl.LookAndFeel);
				if(tabSkin == null) return base.BorderToTabHeadersIndent;
				return tabSkin.Properties.GetInteger(TabSkinProperties.BorderToTabHeadersIndent, 2);
			}
		}
		protected override void UpdatePageBounds(BaseTabPageViewInfo info) {
			SkinTabControlViewInfo vi = ViewInfo as SkinTabControlViewInfo;
			Rectangle r = info.Bounds;
			Skin skin = vi.Skin;
			Rectangle controlBoxRect = info.ControlBox;
			int hgrow = skin.Properties.GetInteger(TabSkinProperties.SelectedHeaderHGrow);
			int delta = skin.Properties.GetInteger(TabSkinProperties.SelectedHeaderUpGrow);
			int contentIndent = skin.Properties.GetInteger(GetContentIndent());
			if((info.PageState & ObjectState.Selected) == 0) {
				hgrow = 0;
				delta = 0;
			}
			if(IsSideLocation) {
				r.Height += hgrow * 2; r.Y -= hgrow;
				r.Width += delta + skin.Properties.GetInteger(GetHeaderDownGrow(info));
				if(HeaderLocation == TabHeaderLocation.Left) {
					delta *= -1;
					r.X += delta;
				} else {
					r.X -= skin.Properties.GetInteger(GetHeaderDownGrow(info));
				}
				info.Text = info.OffsetRect(info.Text, delta + contentIndent, 0);
				info.Image = info.OffsetRect(info.Image, delta + contentIndent, 0);
				if(!controlBoxRect.IsEmpty) controlBoxRect = info.OffsetRect(controlBoxRect, delta + contentIndent, 0);
			} else {
				r.Width += hgrow * 2; r.X -= hgrow;
				r.Height += delta + skin.Properties.GetInteger(GetHeaderDownGrow(info));
				if(HeaderLocation == TabHeaderLocation.Top) {
					delta *= -1;
					r.Y += delta;
				} else {
					r.Y -= skin.Properties.GetInteger(GetHeaderDownGrow(info));
				}
				info.Image = info.OffsetRect(info.Image, 0, delta / 2 + contentIndent);
				info.Text = info.OffsetRect(info.Text, 0, delta / 2 + contentIndent);
				if(CanShowCloseButtonForPage(info) || CanShowPinButtonForPage(info)) {
					controlBoxRect = info.OffsetRect(controlBoxRect, 0, delta / 2 + contentIndent);
				}
			}
			if(!controlBoxRect.IsEmpty) {
				info.ButtonsPanel.ViewInfo.SetDirty();
				info.ButtonsPanel.ViewInfo.Calc(GraphicsInfo.Graphics, controlBoxRect);
			}
			info.Bounds = r;
		}
		protected override bool UpdateFillPageBounds(BaseTabRowViewInfo row, BaseTabPageViewInfo page, int headerBounds) {
			SkinTabControlViewInfo vi = ViewInfo as SkinTabControlViewInfo;
			int deltaSize = vi.Skin.Properties.GetInteger(IsRightLocation || IsBottomLocation ? TabSkinProperties.HeaderDownGrowBottomRight : TabSkinProperties.HeaderDownGrow);
			if(IsSideLocation) {
				deltaSize += IsRightLocation ? page.Bounds.Left : page.Bounds.Right;
			} else {
				deltaSize += IsBottomLocation ? page.Bounds.Top : page.Bounds.Bottom;
			}
			deltaSize = (IsRightLocation || IsBottomLocation) ? deltaSize - headerBounds : headerBounds - deltaSize;
			if(deltaSize < 1) {
				if(vi.IsMultiLine && vi.HeaderInfo.Rows.Count > 0) {
					if(vi.HeaderInfo.Rows.LastRow == row) 
						return false;
				}
				else return false;
			}
			Rectangle bounds = page.Bounds;
			if(IsSideLocation) {
				if(IsRightLocation) bounds.X -= deltaSize;
				bounds.Width += deltaSize;
			}
			else {
				if(IsBottomLocation) bounds.Y -= deltaSize;
				bounds.Height += deltaSize;
			}
			bool changed = page.Bounds != bounds;
			page.Bounds = bounds;
			return changed;
		}
	}
	public class SkinTabControlViewInfo : BaseTabControlViewInfo, ISkinProvider {
		SkinTabPagePainter pagePainter;
		public SkinTabControlViewInfo(IXtraTab tabControl)
			: base(tabControl) {
			this.pagePainter = new SkinTabPagePainter();
		}
		public override bool IsAllowPageCustomBackColor { get { return false; } }
		public SkinTabPagePainter PagePainter { get { return pagePainter; } }
		public virtual Skin Skin { get { return TabSkins.GetSkin(TabControl.LookAndFeel); } }
		public virtual SkinElement SkinPane { get { return Skin[TabSkins.SkinTabPane]; } }
		public virtual SkinElement SkinHeader { get { return Skin[TabSkins.SkinTabHeader]; } }
		public virtual Color GetSkinColoredTabAdjustForeColor() {
			return Skin.Properties.GetColor(TabSkinProperties.ColoredTabAdjustForeColor);
		}
		public virtual Color GetSkinColoredTabBaseForeColor() {
			return Skin.Properties.GetColor(TabSkinProperties.ColoredTabBaseForeColor);
		}
		public string SkinName { get { return Skin.Name; } }
		protected override Rectangle CalcPageClientBounds() {
			GraphicsInfo.AddGraphics(null);
			Rectangle res = PageBounds;
			try {
				res = ObjectPainter.GetObjectClientRectangle(GraphicsInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(SkinPane, PageBounds));
			} finally {
				GraphicsInfo.ReleaseGraphics();
			}
			return res;
		}
	}
}
namespace DevExpress.XtraTab.Drawing {
	public class SkinTabPainter : BaseTabPainter {
		public SkinTabPainter(IXtraTab tabControl)
			: base(tabControl) {
		}
		protected override void DrawTabPage(TabDrawArgs e) {
			if(!IsNeedDrawRect(e, e.ViewInfo.PageBounds)) return;
			SkinTabControlViewInfo vi = e.ViewInfo as SkinTabControlViewInfo;
			UpdateClipRegion(e.Graphics);
			DrawTabSkinPane(e, vi, vi.PageBounds);
		}
		public override void DrawPageClientControl(TabDrawArgs e, BaseTabPageViewInfo pageInfo) {
			SkinTabControlViewInfo vi = e.ViewInfo as SkinTabControlViewInfo;
			Rectangle bounds = GetPageClientDrawBounds(e, pageInfo);
			UpdateClipRegion(e.Graphics);
			DrawTabSkinPane(e, vi, bounds);
			if(vi.SkinPane.Properties.GetBoolean("AllowFillBackground"))
				pageInfo.PaintAppearanceClient.DrawBackground(e.Graphics, e.Cache, e.Bounds);
		}
		protected void DrawTabSkinPane(TabDrawArgs e, SkinTabControlViewInfo vi, Rectangle bounds) {
			Color color = (vi.SelectedTabPageViewInfo != null) ?
				SkinPageClientObjectInfo.CalcColor(vi.SelectedTabPageViewInfo) : Color.Empty;
			SkinPageClientObjectInfo info = new SkinPageClientObjectInfo(vi.SkinPane, bounds, color);
			if(vi.AllowInactiveState && !vi.IsActive) {
				if(info.Element.Image != null && info.Element.Image.ImageCount > 1)
					info.ImageIndex = 1;
			}
			ColoredTabSkinElementPainter.Draw(e.Cache, info,
				ColoredTabElementsCache.GetTabPaneImage(info));
		}
		protected virtual string GetHeaderDownGrow(TabDrawArgs args) {
			if(args.ViewInfo.HeaderInfo.HeaderLocation == TabHeaderLocation.Left || args.ViewInfo.HeaderInfo.HeaderLocation == TabHeaderLocation.Top) return TabSkinProperties.SelectedHeaderDownGrow;
			return TabSkinProperties.SelectedHeaderDownGrowBottomRight;
		}
		protected override Rectangle GetHeaderClipBounds(TabDrawArgs e) {
			BaseTabHeaderViewInfo hInfo = e.ViewInfo.HeaderInfo;
			SkinTabControlViewInfo vi = e.ViewInfo as SkinTabControlViewInfo;
			int grow = vi.Skin.Properties.GetInteger(GetHeaderDownGrow(e));
			Rectangle r = Rectangle.Inflate(hInfo.Client, hInfo.IsSideLocation ? grow : 0, hInfo.IsSideLocation ? 0 : grow);
			Rectangle buttons = hInfo.ButtonsBounds;
			if(e.ViewInfo.HeaderInfo.IsRightToLeftLocation) {
				if(buttons.Width > 0) {
					int right = r.Right;
					r.X += buttons.Width + hInfo.GetButtonsIndent();
					r.Width = right - r.X;
				}
			}
			else {
				r.Size = hInfo.SetSizeWidth(r.Size, hInfo.GetSizeWidth(r.Size) - hInfo.GetSizeWidth(buttons.Size) - hInfo.GetButtonsIndent());
			}
			return r;
		}
		protected override void DrawHeaderBackground(TabDrawArgs e) {
			if(e.ViewInfo.HeaderInfo.FillTransparentBackground) {
				Color color = LookAndFeelHelper.GetSystemColor(e.ViewInfo.TabControl.LookAndFeel, SystemColors.Control);
				e.Cache.FillRectangle(color, e.ViewInfo.HeaderInfo.Bounds);
			}
			base.DrawHeaderBackground(e);
		}
		protected override void DrawHeaderPage(TabDrawArgs e, BaseTabRowViewInfo row, BaseTabPageViewInfo pInfo) {
			SkinTabHeaderViewInfo header = e.ViewInfo.HeaderInfo as SkinTabHeaderViewInfo;
			SkinTabControlViewInfo vi = e.ViewInfo as SkinTabControlViewInfo;
			SkinTabPageObjectInfo pObjInfo = header.UpdatePageInfo(pInfo);
			ObjectPainter.DrawObject(e.Cache, vi.PagePainter, pObjInfo);
			DrawHeaderPageImageText(e, pInfo);
			DrawHeaderFocus(e, pInfo);
		}
		protected override Color CheckHeaderPageForeColor(TabDrawArgs e, BaseTabPageViewInfo pInfo) {
			SkinTabControlViewInfo vi = e.ViewInfo as SkinTabControlViewInfo;
			Color baseColor = base.CheckHeaderPageForeColor(e, pInfo);
			Color coloredColor = SkinTabPageObjectInfo.CalcColor(pInfo);
			if(SkinTabPageObjectInfo.IsEmptyOrTransparent(coloredColor)) 
				return baseColor;
			Color coloredBaseForeColor = vi.GetSkinColoredTabBaseForeColor();
			if(!SkinTabPageObjectInfo.IsEmptyOrTransparent(coloredBaseForeColor)) 
				return ColoredTabElementsCache.ConvertColor(coloredBaseForeColor, new Color?(coloredColor));
			Color coloredForeColor = vi.GetSkinColoredTabAdjustForeColor();
			if(coloredForeColor.IsEmpty) 
				return baseColor;
			if(coloredForeColor != Color.Transparent) 
				return coloredForeColor;
			return ColoredTabElementsCache.ConvertColor(baseColor, new Color?(coloredColor));
		}
	}
	public abstract class ColoredTabSkinElementInfo : SkinElementInfo {
		protected Color colorCore;
		protected ColoredTabSkinElementInfo(SkinElement element, Rectangle bounds)
			: base(element, bounds) {
		}
		public static bool IsEmptyOrTransparent(Color color) {
			return color.IsEmpty || color == System.Drawing.Color.Transparent;
		}
		public Color? Color {
			get { return !IsEmptyOrTransparent(colorCore) ? new Color?(colorCore) : null; }
		}
	}
	public class SkinPageClientObjectInfo : ColoredTabSkinElementInfo {
		public SkinPageClientObjectInfo(SkinElement element, Rectangle bounds, Color color)
			: base(element, bounds) {
			this.colorCore = color;
		}
		public static Color CalcColor(BaseTabPageViewInfo pInfo) {
			Color result = pInfo.PaintAppearance.BackColor;
			if((pInfo.PageState & ObjectState.Selected) != 0) {
				if(pInfo.ViewInfo != null) {
					Color activeColorTab = pInfo.ViewInfo.Properties.AppearancePage.HeaderActive.BackColor;
					Color activeColorPage = (pInfo.Page.Appearance != null) ?
						pInfo.Page.Appearance.HeaderActive.BackColor : System.Drawing.Color.Empty;
					if(IsEmptyOrTransparent(activeColorTab) && IsEmptyOrTransparent(activeColorPage))
						result = System.Drawing.Color.Empty;
				}
			}
			return result;
		}
	}
	public class SkinTabPageObjectInfo : ColoredTabSkinElementInfo {
		TabHeaderLocation headerLocation;
		AppearanceObject appearance;
		public SkinTabPageObjectInfo(SkinElement element, BaseTabPageViewInfo pInfo)
			: base(element, pInfo.Bounds) {
			this.headerLocation = TabHeaderLocation.Top;
			this.appearance = pInfo.PaintAppearance;
			this.colorCore = CalcColor(pInfo);
			this.State = pInfo.PageState;
		}
		public AppearanceObject Appearance { get { return appearance; } }
		public TabHeaderLocation HeaderLocation { get { return headerLocation; } set { headerLocation = value; } }
		public static Color CalcColor(BaseTabPageViewInfo pInfo) {
			Color result = pInfo.PaintAppearance.BackColor;
			if((pInfo.PageState & ObjectState.Selected) != 0) {
				if(pInfo.ViewInfo != null) {
					Color activeColorTab = pInfo.ViewInfo.Properties.AppearancePage.HeaderActive.BackColor;
					Color activeColorPage = (pInfo.Page.Appearance != null) ?
						pInfo.Page.Appearance.HeaderActive.BackColor : System.Drawing.Color.Empty;
					if(IsEmptyOrTransparent(activeColorTab) && IsEmptyOrTransparent(activeColorPage))
						result = System.Drawing.Color.Empty;
				}
			}
			if(pInfo.PageState == ObjectState.Hot) {
				if(pInfo.ViewInfo != null) {
					Color hotColorTab = pInfo.ViewInfo.Properties.AppearancePage.HeaderHotTracked.BackColor;
					Color hotColorPage = (pInfo.Page.Appearance != null) ?
						pInfo.Page.Appearance.HeaderHotTracked.BackColor : System.Drawing.Color.Empty;
					if(IsEmptyOrTransparent(hotColorTab) && IsEmptyOrTransparent(hotColorPage))
						result = System.Drawing.Color.Empty;
				}
			}
			return result;
		}
	}
	public class SkinTabPagePainter : ObjectPainter {
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			SkinTabPageObjectInfo page = e as SkinTabPageObjectInfo;
			Rectangle r = SkinElementPainter.Default.GetObjectClientRectangle(e), res;
			res = r;
			switch(page.HeaderLocation) {
				case TabHeaderLocation.Right:
					res = Rectangle.FromLTRB(e.Bounds.Left + (r.Bottom - e.Bounds.Bottom), e.Bounds.Top + (r.Left - e.Bounds.Left), e.Bounds.Right + (e.Bounds.Top - r.Top), e.Bounds.Bottom + (r.Right - e.Bounds.Right));
					break;
				case TabHeaderLocation.Bottom:
					res = Rectangle.FromLTRB(r.X, e.Bounds.Top + (r.Bottom - e.Bounds.Bottom), r.Right, e.Bounds.Bottom + (r.Top - e.Bounds.Top));
					break;
				case TabHeaderLocation.Left:
					res = Rectangle.FromLTRB(e.Bounds.Left + (r.Top - e.Bounds.Top), e.Bounds.Top + (r.Right - e.Bounds.Right), e.Bounds.Right + (r.Bottom - e.Bounds.Bottom), e.Bounds.Bottom + (r.Left - e.Bounds.Left));
					break;
			}
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return SkinElementPainter.Default.CalcBoundsByClientRectangle(e, client);
		}
		protected virtual void DrawTab(ObjectInfoArgs e) {
			SkinTabPageObjectInfo info = e as SkinTabPageObjectInfo;
			Size topSize = e.Bounds.Size;
			if(info.HeaderLocation == TabHeaderLocation.Left || info.HeaderLocation == TabHeaderLocation.Right) {
				topSize.Width = e.Bounds.Height;
				topSize.Height = e.Bounds.Width;
			}
			bool shouldEnlargeTabHeader = SkinElementPainter.CorrectByRTL && (info.HeaderLocation == TabHeaderLocation.Top || info.HeaderLocation == TabHeaderLocation.Bottom);
			if(shouldEnlargeTabHeader)
				topSize.Width += 2;
			Bitmap bmp = BitmapRotate.CreateBufferBitmap(topSize, true);
			if(shouldEnlargeTabHeader)
				e.Bounds = new Rectangle(1, 0, topSize.Width - 2, topSize.Height + 1);
			else 
				e.Bounds = new Rectangle(0, 0, topSize.Width, topSize.Height + 1);
			bool prevCorrectRTL = SkinElementPainter.CorrectByRTL;
			SkinElementPainter.CorrectByRTL = false;
			try {
				ColoredTabSkinElementPainter.Draw(BitmapRotate.BufferCache, info,
					ColoredTabElementsCache.GetTabHeaderImage(info));
			}
			finally {
				SkinElementPainter.CorrectByRTL = prevCorrectRTL;
			}
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinTabPageObjectInfo ee = e as SkinTabPageObjectInfo;
			GraphicsCache save = e.Cache;
			Rectangle saveBounds = e.Bounds;
			try {
				DrawTab(e);
			} finally {
				e.Bounds = saveBounds;
				e.Cache = save;
			}
			RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone;
			switch(ee.HeaderLocation) {
				case TabHeaderLocation.Left:
					rotate = RotateFlipType.Rotate270FlipNone;
					break;
				case TabHeaderLocation.Right:
					rotate = RotateFlipType.Rotate90FlipNone;
					break;
				case TabHeaderLocation.Bottom:
					rotate = RotateFlipType.RotateNoneFlipY;
					break;
			}
			BitmapRotate.RotateBitmap(rotate);
			Rectangle imageBounds = new Rectangle(new Point(0, 0), e.Bounds.Size);
			bool shouldEnlargeTabHeader = SkinElementPainter.CorrectByRTL && (ee.HeaderLocation == TabHeaderLocation.Top || ee.HeaderLocation == TabHeaderLocation.Bottom);
			if(shouldEnlargeTabHeader) {
				imageBounds.Width += 2;
				e.Bounds = new Rectangle(e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 2, e.Bounds.Height);
			}
			e.Paint.DrawImage(e.Graphics, BitmapRotate.BufferBitmap, e.Bounds, imageBounds, true);
			BitmapRotate.RestoreBitmap(rotate);
		}
	}
}
