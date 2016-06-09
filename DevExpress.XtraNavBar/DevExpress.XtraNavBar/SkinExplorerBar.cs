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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using System.Collections;
using DevExpress.XtraEditors;
using DevExpress.Utils.Text;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class SkinExplorerBarNavBarViewInfo : ExplorerBarNavBarViewInfo {
		#region OldSkins
		#endregion
		public SkinExplorerBarViewInfoRegistrator View {
			get { return NavBar.View as SkinExplorerBarViewInfoRegistrator; }
		}
		protected override bool IsSkinned { get { return true; } }
		public Skin Skin { get { return View.GetSkin(NavBar); } }
		public SkinExplorerBarNavBarViewInfo(NavBarControl navBar) : base(navBar) { 
			ScrollBar.LookAndFeel.SetDefaultStyle();
			if(View.GetSkin(NavBar) != null) {
				ScrollBar.LookAndFeel.SetSkinStyle(View.GetSkin(NavBar).Name);
				ScrollBar.LookAndFeel.SkinMaskColor = ((ISkinProviderEx)NavBar).GetMaskColor();
				ScrollBar.LookAndFeel.SkinMaskColor2 = ((ISkinProviderEx)NavBar).GetMaskColor2();
			}
		}
		protected override int ButtonAutoRepeatInterval { get { return 20; } }
		const int ScrollStep = 10;
		protected internal override void OnButtonPress(NavBarHitInfo hitInfo, bool isUp) {
			if(isUp) 
				TopY = Math.Max(0, TopY - ScrollStep);
			else
				TopY = Math.Min(ScrollRange, TopY + ScrollStep);
		}
		protected virtual bool AllowSkinViewScrollBar { 
			get {
				if(NavBar.SkinExplorerBarViewScrollStyle == SkinExplorerBarViewScrollStyle.Default && ScrollBarBase.GetUIMode(ScrollUIMode.Default) == ScrollUIMode.Touch) return true;
				return NavBar.SkinExplorerBarViewScrollStyle == SkinExplorerBarViewScrollStyle.ScrollBar; 
			} 
		}
		protected override void OnScrollBarValueChanged(object sender, EventArgs e) { 
			if(AllowSkinViewScrollBar) base.OnScrollBarValueChanged(sender, e);
		}
		public override bool ScrollBarVisible { 
			get { 
				if(!AllowSkinViewScrollBar) return false;
				return base.ScrollBarVisible;
			}
			set { 
				if(!AllowSkinViewScrollBar) return;
				base.ScrollBarVisible = value;
			} 
		}
		public override NavBarHitInfo CreateHitInfo() { return new SkinExplorerBarHitInfo(NavBar); }
		protected override void DrawButtons(GraphicsCache e) { 	}
		protected internal override ObjectState UpButtonState {
			get {
				if(TopY == 0 || UpButtonBounds.IsEmpty) return ObjectState.Disabled;
				return base.UpButtonState;
			}
		}
		protected internal override ObjectState DownButtonState {
			get {
				if(DownButtonBounds.IsEmpty || TopY >= ScrollRange - ScrollBar.LargeChange + 2) return ObjectState.Disabled;
				return base.DownButtonState;
			}
		}
		protected override void OnAfterDraw(GraphicsCache e) {
			SkinElementInfo info = new SkinElementInfo(Skin["ScrollUpButton"], UpButtonBounds);
			info.ImageIndex = -1;
			info.State = UpButtonState;
			if(!info.Bounds.IsEmpty) ObjectPainter.DrawObject(e, SkinElementPainter.Default, info);
			info = new SkinElementInfo(Skin["ScrollDownButton"], DownButtonBounds);
			info.ImageIndex = -1;
			info.State = DownButtonState;
			if(!info.Bounds.IsEmpty) ObjectPainter.DrawObject(e, SkinElementPainter.Default, info);
			base.OnAfterDraw(e);
		}
		protected override void DrawBorderAndBackground(GraphicsCache e, Rectangle r) {
			DrawBackground(e, r);
		}
		protected override void DrawBackgroundCore(GraphicsCache e, CustomDrawObjectEventArgs custom, Rectangle r) {
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinBackground], r);
			if(IsRightToLeft) info.RightToLeft = true;
			ObjectPainter.DrawObject(e, SkinElementPainter.Default, info);
		}
		protected override Rectangle CalcClientRectangle(Rectangle bounds) {
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinBackground], bounds);
			return ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, info);
		}
		protected override int CalcGroupHorzIndent() {
			if(NavBar.ExplorerBarGroupOuterIndent > -1) return NavBar.ExplorerBarGroupOuterIndent;
			return 0; 
		}
		protected override int CalcGroupVertIndent(NavGroupInfoArgs groupInfo) {
			int res = 0;
			if(NavBar.ExplorerBarGroupInterval > -1) 
				res = NavBar.ExplorerBarGroupInterval;
			else 
				res = Groups.Count == 0 ? 0 : Skin.Properties.GetInteger("GroupInterval");
			Size minSize = NavBar.GroupPainter.CalcObjectMinBounds(groupInfo).Size;
			Size imgSize = groupInfo.Group.GetPrefferedImageSize();
			imgSize.Height = Math.Max(NavBar.GroupPainter.CalcBoundsByClientRectangle(groupInfo, new Rectangle(Point.Empty, imgSize)).Height, imgSize.Height);
			if(imgSize.Height > minSize.Height) {
				res += (imgSize.Height - minSize.Height) + 1; 
			}
			return res;
		}
		public override NavBarGroupStyle GetDefaultGroupStyle() { return NavBarGroupStyle.SmallIconsText; }
		protected override void UpdateScrollBar(int maxBottom, ref int savedY) {
			if(!AllowSkinViewScrollBar) {
				UpdateScrollButtons(maxBottom, ref savedY);
				return;
			}
			base.UpdateScrollBar(maxBottom, ref savedY);
			if(!ScrollBarRectangle.IsEmpty && !ScrollBar.IsOverlapScrollBar) {
				int clientX = IsRightToLeft ? ScrollBarRectangle.Right : Client.X;
				int clientWidth = IsRightToLeft ? Client.Right - scrollBarRectangle.Right : scrollBarRectangle.X - Client.X;
				Client = new Rectangle(clientX, Client.Y, clientWidth, Client.Height);
			}
		}
		protected virtual void UpdateScrollButtons(int maxBottom, ref int savedY) {
			DownButtonBounds = UpButtonBounds = Rectangle.Empty;
			if(maxBottom > Client.Bottom) {
				GInfo.AddGraphics(null);
				try {
					int upHeight = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin["ScrollUpButton"])).Height;
					int downHeight = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin["ScrollDownButton"])).Height;
					Rectangle up = new Rectangle(Client.X, Client.Y, Client.Width, upHeight);
					Rectangle down = new Rectangle(Client.X, Client.Bottom - downHeight, Client.Width, downHeight);
					Client = new Rectangle(Client.X, Client.Y + up.Height, Client.Width, Client.Height - (up.Height + down.Height));
					UpButtonBounds = up;
					DownButtonBounds = down;
				} finally {
					GInfo.ReleaseGraphics();
				}
			} else {
				savedY = 0;
				ScrollRange = 0;
			}
		}
		protected override void UpdateScrollRange(int newRange) {
			if(newRange > 0) newRange = Math.Max(newRange - DownButtonBounds.Height, 0);
			ScrollRange = newRange;
		}
	}
	public class SkinExplorerBarNavLinkPainter : ExplorerBarNavLinkPainter {
		public SkinExplorerBarNavLinkPainter(NavBarControl navBar) : base(navBar) {
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
		}
		protected override void DrawLinkBackground(NavLinkInfoArgs linkInfo) {
			SkinElement elem = Skin[NavBarSkins.SkinItem];
			SkinElementInfo info = new SkinElementInfo(elem, linkInfo.Bounds);
			if(IsRightToleft) info.RightToLeft = true;
			ObjectPainter.DrawObject(linkInfo.Cache, SkinElementPainter.Default, info);
		}
		protected override Brush GetLinkCaptionBrush(ObjectInfoArgs e) { 
			NavBarItemLink link = GetLink(e);
			AppearanceObject appearance = GetLinkAppearance(e);
			return ExplorerBarColorHelper.CalcTextBrush(e, appearance.GetForeColor(), link.Enabled);
		}
		public override int GetImageIndent(ObjectInfoArgs e) { return 4; }
		protected Skin Skin { get { return (NavBar.ViewInfo as SkinExplorerBarNavBarViewInfo).Skin; } }
		protected SkinPaddingEdges GetContentMargins() {
			return Skin[NavBarSkins.SkinItem].ContentMargins;
		}
		protected int Image2TextIndent { get { return 4; } }
		bool HasCustomContentMargins { get { return !GetContentMargins().IsEmpty; } }
		protected override int CalcLinkIndentWithoutImage(int textX, NavLinkInfoArgs li) {
			SkinElement elem = Skin[NavBarSkins.SkinGroupHeader];
			return elem.ContentMargins.Left - textX;
		}
		protected override Rectangle CalcSmallBounds(ObjectInfoArgs e, Rectangle r) {
			if(!HasCustomContentMargins)
				return base.CalcSmallBounds(e, r);
			Rectangle imageRect = r, textRect = r;
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			NavBarItemLink link = GetLink(e);
			Size textSize;
			Size imageSize = IsItemExists(li) ? li.Link.Item.GetPrefferedImageSize(CalcImageSize(e)) : CalcImageSize(e);
			SkinPaddingEdges margins = GetContentMargins();
			imageRect.X += margins.Left;
			imageRect.Height = imageSize.Height + margins.Top + margins.Bottom;
			imageRect.Width = imageSize.Width;
			textRect.X = imageRect.Right + Image2TextIndent;
			textRect.Width = r.Right - margins.Right - textRect.X;
			textSize = CalcBestTextSize(e, textRect.Width);
			if(imageRect.Height < textSize.Height + margins.Top + margins.Bottom)
				imageRect.Y += ((textSize.Height + margins.Top + margins.Bottom) - imageRect.Height) / 2;
			textRect.Height = Math.Max(imageRect.Height, textSize.Height + margins.Top + margins.Bottom);
			textRect.Width = textSize.Width;
			li.ImageRectangle = CheckBounds(imageRect);
			li.CaptionRectangle = CheckBounds(textRect);
			li.RealCaptionRectangle = li.CaptionRectangle;
			li.HitRectangle = new Rectangle(imageRect.Location, new Size(textRect.Right - imageRect.Location.X, imageRect.Bottom - textRect.Y));
			r.Height = textRect.Height;
			li.Bounds = r;
			return r;
		}
		protected override Rectangle CalcLargeBounds(ObjectInfoArgs e, Rectangle source) {
			if(!HasCustomContentMargins)
				return base.CalcLargeBounds(e, source);
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			Rectangle r = source;
			SkinPaddingEdges margins = GetContentMargins();
			r.Y += margins.Top;
			Rectangle res = CalcLargeImageRect(e, r);
			li.ImageRectangle = res;
			res = r;
			res.Y = li.ImageRectangle.Bottom + Image2TextIndent;
			res.X += margins.Left;
			res.Width -= margins.Left + margins.Right;
			r.Height = res.Height;
			Size textSize = CalcTextSize(e, res.Width);
			res.Height = textSize.Height + margins.Bottom;
			li.CaptionRectangle = res;
			AppearanceObject app = GetLinkAppearance(e);
			switch(app.HAlignment) {
				case HorzAlignment.Near:
					break;
				case HorzAlignment.Far:
					res.X = res.Right - textSize.Width;
					break;
				default:
					res.X = res.X + (res.Width - textSize.Width) / 2;
					break;
			}
			res.Width = textSize.Width;
			li.RealCaptionRectangle = res;
			r.Y = source.Y;
			r.Height = (li.CaptionRectangle.Bottom - source.Top) + CalcLargeVertIndent(e);
			li.Bounds = r;
			li.HitRectangle = new Rectangle(li.ImageRectangle.Location, new Size(res.Right - li.ImageRectangle.Location.X, res.Bottom - li.ImageRectangle.Location.Y));
			return r;
		}
		protected override int CalcContentX(Rectangle bounds) {
			int contentWidth = NavBar.Width;
			if(!IsOverlapScrollBar) contentWidth += NavBar.ViewInfo.ScrollBarRectangle.Width;
			return contentWidth - bounds.Right;
		}
	}
	public class SkinExplorerBarNavGroupClientPainter : ExplorerBarNavGroupClientPainter {
		public SkinExplorerBarNavGroupClientPainter(BaseNavGroupPainter groupPainter) : base(groupPainter) { }
		protected override void DrawBorder(NavGroupClientInfoArgs e) { }
		protected internal override int CalcGroupClientHeightByClientSize(Size clientSize) {
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupClient]);
			return ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, info, new Rectangle(Point.Empty, clientSize)).Height;
		}
		protected override void CalcClientBounds(NavGroupClientInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupClient]);
			info.Bounds = e.Bounds;
			e.ClientBounds = ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, info);
		}
		protected override int CalcClientIndent(NavGroupClientInfoArgs e, IndentType indent) {
			if(e.Group.GroupStyle == NavBarGroupStyle.ControlContainer) return 0;
			if(e.Group.GetLinksUseSmallImage() && Skin[NavBarSkins.SkinGroupClientLinksStrip] != null) {
				if(indent == IndentType.Right || indent == IndentType.Top || indent == IndentType.Bottom) return 5;
				return 0;
			}
			return base.CalcClientIndent(e, indent);
		}
		protected override void DrawBackgroundCore(NavGroupClientInfoArgs e, CustomDrawObjectEventArgs custom) {
			bool isRightToLeft = e.NavBar != null && e.NavBar.IsRightToLeft;
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupClient]);
			if(isRightToLeft) info.RightToLeft = true;
			info.Bounds = e.Bounds;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			SkinElement indent = Skin[NavBarSkins.SkinGroupClientLinksStrip];
			if(e.Group.GetLinksUseSmallImage() && indent != null && e.Group.GetShowIcons()) {
				info = new SkinElementInfo(indent);
				if(isRightToLeft) info.RightToLeft = true;
				int width = (e.ClientInnerBounds.X - e.ClientBounds.X) + 
					CalcImageSize(e, e.Group).Width + NavBar.LinkPainter.GetImageIndent(e) * 2;
				int x = isRightToLeft ? e.ClientBounds.Right - width : e.ClientBounds.X;
				info.Bounds = new Rectangle(x, e.ClientBounds.Y, width, e.ClientBounds.Height);
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			}
		}
		public override Size CalcImageSize(ObjectInfoArgs e, NavBarGroup group) {
			if(!group.GetShowIcons())
				return Size.Empty;
			Size size = base.CalcImageSize(e, group);
			if(group.GetLinksUseSmallImage()) {
				SkinElement indent = Skin[NavBarSkins.SkinGroupClientLinksStrip];
				SkinElementInfo info = new SkinElementInfo(indent);
				info.Bounds = new Rectangle(Point.Empty, size);
				if(indent != null) {
					Size max = ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, info, info.Bounds).Size;
					size.Width = max.Width;
				}
			}
			return size;
		}
		protected Skin Skin { get { return (NavBar.ViewInfo as SkinExplorerBarNavBarViewInfo).Skin; } }
	}
	public class SkinExplorerBarNavGroupPainter : ExplorerBarNavGroupPainter {
		public SkinExplorerBarNavGroupPainter(NavBarControl navBar) : base(navBar) {	}
		protected Skin Skin { get { return (NavBar.ViewInfo as SkinExplorerBarNavBarViewInfo).Skin; } }
		protected override ObjectPainter CreateOpenCloseButtonPainter() { return new ObjectPainter(); }
		protected override BaseNavGroupClientPainter CreateGroupClientPainter() { 
			return new SkinExplorerBarNavGroupClientPainter(this); 
		}
		protected override int GetLeftTextIndent() {
			return 0;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle res = base.CalcObjectMinBounds(e);
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupHeader]);
			Size size = ObjectPainter.CalcObjectMinBounds(groupInfo.Graphics, SkinElementPainter.Default, info).Size;
			res.Height = Math.Max(res.Height, size.Height);
			return res;
		}
		protected override Size CalcExpandCollapseButtonSize(ObjectInfoArgs e, int textHeight) {
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupCloseButton], Rectangle.Empty);
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, info).Size;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupHeader], client);
			return ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, info, client);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupHeader], e.Bounds);
			return ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, info);
		}
		protected override bool AllowShowGroupButtonInCaption {
			get {
				if(!NavBar.ExplorerBarShowGroupButtons) return false;
				return !Skin.Properties.GetBoolean("ShowButtonInFooter");
			}
		}
		protected override Rectangle CalcGroupCaptionButtonBounds(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			if(Skin.Properties.GetBoolean("ShowButtonInFooter") || !NavBar.ExplorerBarShowGroupButtons) return Rectangle.Empty;
			Size buttonSize = CalcGroupButtonSize(groupInfo);
			if(buttonSize.IsEmpty) return Rectangle.Empty;
			Rectangle captionClient = groupInfo.CaptionClientBounds;
			return Skin[NavBarSkins.SkinGroupOpenButton].Offset.GetBounds(captionClient, buttonSize, SkinOffsetKind.Far);
		}
		protected virtual Size CalcGroupButtonSize(NavGroupInfoArgs e) {
			SkinElementInfo buttonInfo = new SkinElementInfo(Skin[NavBarSkins.SkinGroupOpenButton]);
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, buttonInfo).Size;
		}
		public override void CalcFooterBounds(NavGroupInfoArgs groupInfo, Rectangle bounds, Size buttonSize) {
			groupInfo.FooterBounds = bounds;
			if(buttonSize == Size.Empty) return;
			groupInfo.ButtonBounds = Skin[NavBarSkins.SkinGroupCloseButton].Offset.GetBounds(bounds, buttonSize, SkinOffsetKind.Center);
		}
		public override int CalcFooterHeight(NavGroupInfoArgs groupInfo, out Size buttonSize) {
			buttonSize = Size.Empty;
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupFooter]);
			if(!Skin.Properties.GetBoolean("ShowFooter")) return 0;
			int height = ObjectPainter.CalcObjectMinBounds(groupInfo.Graphics, SkinElementPainter.Default, info).Height;
			bool showButtonInFooter = Skin.Properties.GetBoolean("ShowButtonInFooter") && NavBar.ExplorerBarShowGroupButtons;
			if(showButtonInFooter) {
				buttonSize = CalcGroupButtonSize(groupInfo);
				info.Bounds = new Rectangle(0, 0, 100, buttonSize.Height);
				height = Math.Max(height, ObjectPainter.CalcBoundsByClientRectangle(groupInfo.Graphics, SkinElementPainter.Default, info, info.Bounds).Height);
			}
			return height;
		}
		protected virtual void FillGroupCaption(CustomDrawNavBarElementEventArgs custom) {
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupHeader]);
			if(IsRightToLeft) info.RightToLeft = true;
			info.Bounds = custom.ObjectInfo.Bounds;
			ObjectPainter.DrawObject(custom.ObjectInfo.Cache, SkinElementPainter.Default, info);
		}
		protected override void DrawGroupButton(NavGroupInfoArgs e) {
			if(e.ButtonBounds.IsEmpty) return;
			string name = e.Group.Expanded ? NavBarSkins.SkinGroupCloseButton : NavBarSkins.SkinGroupOpenButton;
			SkinElementInfo buttonInfo = new SkinElementInfo(Skin[name]);
			if(IsRightToLeft) buttonInfo.RightToLeft = true;
			buttonInfo.Bounds = e.ButtonBounds;
			if(NavBar.ViewInfo.HotInfo.InGroupButton && NavBar.ViewInfo.HotInfo.Group == e.Group) buttonInfo.State = ObjectState.Hot;
			if(NavBar.ViewInfo.PressedInfo.InGroupButton && NavBar.ViewInfo.PressedInfo.Group == e.Group) buttonInfo.State = ObjectState.Pressed;
			buttonInfo.ImageIndex = -1;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, buttonInfo);
		}
		protected override void DrawGroupCaptionCore(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			FillGroupCaption(custom);
			DrawGroupImage(e, custom);
			if(!Skin.Properties.GetBoolean("ShowButtonInFooter")) DrawGroupButton(e);
			if (e.Group.GetAllowHtmlString()) {
				CalcHtmlStringInfo(e, custom.Caption, 0, custom.Appearance);
				StringPainter.Default.UpdateLocation(e.StringInfo, e.CaptionBounds.Location);
				StringPainter.Default.DrawString(e.Cache, e.StringInfo);
			} else 
				custom.Appearance.DrawString(custom.Cache, custom.Caption, e.CaptionBounds, custom.Appearance.GetForeBrush(e.Cache));
			DrawGroupFooter(e);
		}
		protected virtual void DrawGroupFooter(NavGroupInfoArgs e) {
			if(e.FooterBounds.IsEmpty) return;
			SkinElementInfo info = new SkinElementInfo(Skin[NavBarSkins.SkinGroupFooter]);
			if(IsRightToLeft) info.RightToLeft = true;
			info.Bounds = e.FooterBounds;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			if(Skin.Properties.GetBoolean("ShowButtonInFooter")) DrawGroupButton(e);
		}
	}
	public class SkinExplorerBarHitInfo : NavBarHitInfo {
		public SkinExplorerBarHitInfo(NavBarControl navBar) : base(navBar) { }
		protected override NavBarHitInfo CreateHitInfo() { return new SkinExplorerBarHitInfo(NavBar); }
		public override void CalcHitInfo(Point p, NavBarHitTest[] validLinkHotTracks) {
			if(CheckAndSetHitTest(ViewInfo.UpButtonBounds, p, NavBarHitTest.UpButton)) {
				return;
			}
			if(CheckAndSetHitTest(ViewInfo.DownButtonBounds, p, NavBarHitTest.DownButton)) {
				return;
			}
			base.CalcHitInfo(p, validLinkHotTracks);
		}
	}
}
