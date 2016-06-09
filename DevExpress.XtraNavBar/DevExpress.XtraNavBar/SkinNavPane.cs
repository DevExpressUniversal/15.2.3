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
using DevExpress.Utils.Menu;
using System.Collections;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class SkinNavigationPaneViewInfo : NavigationPaneViewInfo {
		public SkinNavigationPaneViewInfo(NavBarControl navBar) : base(navBar) { }
		protected override bool IsSkinned { get { return true; } }
		public override bool AllowListViewMode { get { return false; } }
		protected override NavigationPaneHeaderPainter CreateNavPaneHeaderPainter() {
			return new SkinNavigationPaneHeaderPainter(this);
		}
		protected override NavigationPaneSplitterPainter CreateNavPaneSplitterPainter() {
			return new SkinNavigationPaneSplitterPainter(NavBar);
		}
		protected override NavigationPaneOverflowPanelPainter CreateNavPaneOverflowPanelPainter() {
			return new SkinNavigationPaneOverflowPanelPainter(NavBar);
		}
		protected override void ShowMenu(DXPopupMenu menu) {
			MenuManagerHelper.ShowMenu(menu, NavBar.LookAndFeel, NavBar.MenuManager, NavBar, MousePosition);
		}
		public SkinNavigationPaneViewInfoRegistrator View {
			get { return NavBar.View as SkinNavigationPaneViewInfoRegistrator; }
		}
		public Skin Skin { get { return View.GetSkin(NavBar); } }
		protected override BorderPainter CreateDefaultBorderPainter() {
			return new EmptyBorderPainter();
		}
		public override int GetCollapsedWidth() {
			NavigationPaneHeaderObjectInfo info = new NavigationPaneHeaderObjectInfo("", AppearanceObject.ControlAppearance, null);
			info.OptionsNavPane = new OptionsNavPane(NavBar.OptionsNavPane);
			info.OptionsNavPane.NavPaneState = NavPaneState.Collapsed;
			SkinElementInfo skin = new SkinElementInfo(NavPaneSkins.GetSkin(NavBar)[NavPaneSkins.SkinCaption], new Rectangle(Point.Empty, this.HeaderPainter.GetButtonSize(info)));
			Rectangle bounds = Rectangle.Empty;
			if(skin != null && skin.Element != null)
				bounds = ObjectPainter.CalcBoundsByClientRectangle(info.Graphics, SkinElementPainter.Default, skin, skin.Bounds);
			return Math.Max(info.OptionsNavPane.CollapsedWidth, Math.Max(base.GetCollapsedWidth(), bounds.Width));
		}
		protected override void CalcHeaderButtonInfo(NavigationPaneHeaderObjectInfo info) {
			SkinElementInfo skin = new SkinElementInfo(NavPaneSkins.GetSkin(NavBar)[NavPaneSkins.SkinCaption], info.Bounds);
			Rectangle clientBounds = Rectangle.Empty;
			if(skin != null && skin.Element != null)
				clientBounds = ObjectPainter.GetObjectClientRectangle(info.Graphics, SkinElementPainter.Default, skin);
			Size bSize = this.HeaderPainter.GetButtonSize(info);
			int offset = (clientBounds.Height - bSize.Height) / 2;
			int hzOffset = offset > bSize.Width / 2 ? bSize.Width / 2 : offset;
			if(offset < 0) offset = 0;
			if(info.OptionsNavPane.ShowExpandButton)
				HeaderInfo.ButtonBounds = new Rectangle(clientBounds.Right - hzOffset - bSize.Width, clientBounds.Top + hzOffset, bSize.Width, bSize.Height);
			else
				HeaderInfo.ButtonBounds = Rectangle.Empty;
			UpdateHeaderButtonState();
			HeaderInfo.OptionsNavPane = new OptionsNavPane(this.NavBar.OptionsNavPane);
		}
		protected internal override bool AllowDrawCaption { 
			get { return NavBar.IsDesignMode || NavBar.OptionsNavPane.CollapsedNavPaneContentControl == null || NavBar.OptionsNavPane.NavPaneState != NavPaneState.Collapsed; } 
		}
	}
	public class SkinNavigationPaneSplitterPainter : NavigationPaneSplitterPainter {
		ISkinProvider provider;
		public SkinNavigationPaneSplitterPainter(ISkinProvider provider) : base(null) {
			this.provider = provider;
		}
		protected ISkinProvider Provider { get { return provider; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SkinElementInfo skin = new SkinElementInfo(NavPaneSkins.GetSkin(Provider)[NavPaneSkins.SkinSplitter]);
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, skin);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementInfo skin = new SkinElementInfo(NavPaneSkins.GetSkin(Provider)[NavPaneSkins.SkinSplitter], e.Bounds);
			skin.State = e.State;
			if(skin.State != ObjectState.Normal) skin.ImageIndex = 1;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, skin);
		}
	}
	public class SkinNavigationPaneHeaderPainter : NavigationPaneHeaderPainter {
		public SkinNavigationPaneHeaderPainter(NavigationPaneViewInfo navPAneVi) : base(navPAneVi) {}
		public virtual NavBarControl NavBar { get { return parentVi.NavBar; } }
		protected Skin Skin { get { return (NavBar.ViewInfo as SkinNavigationPaneViewInfo).Skin; } }
		SkinElement GetButtonSkinElement(NavigationPaneHeaderObjectInfo info) {
			NavPaneState state = info.OptionsNavPane.NavPaneState;
			if(info.OptionsNavPane.ExpandButtonMode == DevExpress.Utils.Controls.ExpandButtonMode.Inverted)
				state = NavPaneStateConverter.Invert(state);
			if(state == NavPaneState.Collapsed)
				return Skin[NavPaneSkins.SkinExpandButton];
			else return Skin[NavPaneSkins.SkinCollapseButton];
		}
		public override void DrawButton(NavigationPaneHeaderObjectInfo info) {
			NavPaneState navPaneState = info.OptionsNavPane.NavPaneState;
			SkinElementInfo skin = new SkinElementInfo(GetButtonSkinElement(info), info.ButtonBounds);
			if(NavBar != null && NavBar.IsRightToLeft) skin.RightToLeft = true;
			skin.ImageIndex = CalcImageIndex(info.ButtonState);
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, skin);
		}
		int CalcImageIndex(ObjectState state) {
			if((state & ObjectState.Disabled) != 0) return 0;
			if((state & ObjectState.Pressed) != 0) return 2;
			if((state & ObjectState.Hot) != 0) return 1;
			return 0;
		}
		public override Size GetButtonSize(NavigationPaneHeaderObjectInfo info) {
			SkinElementInfo skin = new SkinElementInfo(GetButtonSkinElement(info));
			if(NavBar != null && NavBar.IsRightToLeft) skin.RightToLeft = true;
			if(skin == null || skin.Element == null || skin.Element.Image == null || skin.Element.Image.Image == null)
				return Size.Empty;
			return skin.Element.Image.GetImageBounds(CalcImageIndex(info.ButtonState)).Size;
		}
		protected internal override Rectangle GetCaptionBounds(NavigationPaneHeaderObjectInfo info) {
			SkinElementInfo skin = new SkinElementInfo(Skin[NavPaneSkins.SkinCaption]);
			skin.Bounds = info.Bounds;
			Rectangle r = ObjectPainter.GetObjectClientRectangle(info.Graphics, SkinElementPainter.Default, skin);
			if(info.OptionsNavPane.ShowExpandButton)
				r.Width = info.ButtonBounds.Left - r.Left - 6;
			if(info.OptionsNavPane.ShowGroupImageInHeader) r.X += info.OptionsNavPane.DefaultNavPaneHeaderImageSize.Width + r.X;
			r.Width = CalcTextSize(info, info.Caption, r.Width).Width;
			return r;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SkinElementInfo skin = new SkinElementInfo(Skin[NavPaneSkins.SkinCaption]);
			NavigationPaneHeaderObjectInfo info = e as NavigationPaneHeaderObjectInfo;
			int maxTextWidth = info.CaptionBounds.Width; 
			Size textSize = CalcTextSize(e, info.Caption == "" ? "Wg" : info.Caption, maxTextWidth);
			Size buttonSize = GetButtonSize(info);
			textSize = new Size(textSize.Width + buttonSize.Width, Math.Max(textSize.Height, buttonSize.Height));
			if(info.OptionsNavPane.NavPaneState == NavPaneState.Collapsed)
				textSize.Width = 0;
			return ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, skin, new Rectangle(Point.Empty, textSize));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			NavigationPaneHeaderObjectInfo info = e as NavigationPaneHeaderObjectInfo;
			Brush foreBrush;
			foreBrush = info.Appearance.GetForeBrush(e.Cache);
			SkinElementInfo skin = new SkinElementInfo(Skin[NavPaneSkins.SkinCaption], e.Bounds);
			if(NavBar != null && NavBar.IsRightToLeft) skin.RightToLeft = true;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, skin);
			DrawCaptionAndButton(info, foreBrush);
		}
	}
	public class SkinNavigationPaneOverflowPanelPainter : NavigationPaneOverflowPanelPainter {
		public SkinNavigationPaneOverflowPanelPainter(NavBarControl navBar) : base(navBar) { }
		protected override ObjectPainter CreatePanelButtonPainter() {
			return new SkinNavigationPaneOverflowPainter(NavBar);
		}
		protected override NavigationPaneOverflowPanelObjectPainter CreateButtonPainter() {
			return new SkinNavigationPaneOverflowPanelObjectPainter(NavBar);
		}
	}
	public class SkinNavigationPaneOverflowPainter : SkinCustomPainter {
		public SkinNavigationPaneOverflowPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(NavPaneSkins.GetSkin(Provider)[NavPaneSkins.SkinOverflowPanel], e.Bounds);
		}
	}
	public class SkinNavigationPaneScrollButtonPainter : SkinCustomPainter {
		public SkinNavigationPaneScrollButtonPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			UpDownButtonObjectInfoArgs args = e as UpDownButtonObjectInfoArgs;
			SkinElementInfo res = new SkinElementInfo(NavPaneSkins.GetSkin(Provider)[args.IsUpButton ? NavPaneSkins.SkinScrollUp : NavPaneSkins.SkinScrollDown], e.Bounds);
			res.State = e.State;
			return res;
		}
	}
	public class SkinNavigationPaneOverflowPanelObjectPainter : NavigationPaneOverflowPanelObjectPainter {
		public SkinNavigationPaneOverflowPanelObjectPainter(NavBarControl navBar) : base(navBar) { }
		SkinElementInfo CreateInfo(string element) { return CreateInfo(element, Rectangle.Empty); }
		SkinElementInfo CreateInfo(string element, Rectangle bounds) { 
			return new SkinElementInfo(NavPaneSkins.GetSkin(NavBar)[element], bounds); 
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			NavigationPaneOverflowPanelObjectInfo info = e as NavigationPaneOverflowPanelObjectInfo;
			SkinElementInfo skin = CreateInfo(NavPaneSkins.SkinOverflowPanelItem);
			return ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, skin, new Rectangle(Point.Empty, info.ImageSize));
		}
		int CalcImageIndex(ObjectState state) {
			if((state & ObjectState.Disabled) != 0) return 0;
			int res = 0;
			if((state & ObjectState.Hot) != 0) res = 1;
			if((state & ObjectState.Pressed) != 0) res = 2;
			if((state & ObjectState.Selected) != 0) res += 3;
			return res;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			NavigationPaneOverflowPanelObjectInfo info = e as NavigationPaneOverflowPanelObjectInfo;
			SkinElementInfo skin = CreateInfo(info.IsChevron ? NavPaneSkins.SkinOverflowPanelExpandItem : NavPaneSkins.SkinOverflowPanelItem, e.Bounds);
			skin.ImageIndex = CalcImageIndex(e.State);
			skin.State = e.State;
			if(NavBar != null && NavBar.IsRightToLeft) skin.RightToLeft = true;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, skin);
			Rectangle bounds = ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, skin);
			Image image = info.Image;
			if(image == null || info.IsChevron) return;
			Size imageSize = image.Size;
			Rectangle r = new Rectangle(
				bounds.X + (bounds.Width - imageSize.Width) / 2,
				bounds.Y + (bounds.Height - imageSize.Height) / 2, imageSize.Width, imageSize.Height);
			if(NavBar.AllowGlyphSkinning)
				e.Cache.Paint.DrawImage(e.Graphics, image, r, new Rectangle(Point.Empty, image.Size), ImageColorizer.GetColoredAttributes(info.Appearance.ForeColor));
			else
				e.Cache.Paint.DrawImage(e.Graphics, image, r, new Rectangle(Point.Empty, image.Size), true);
		}
	}
	public class SkinNavigationPaneLinkPainter : BaseNavLinkPainter {
		public SkinNavigationPaneLinkPainter(NavBarControl navBar) : base(navBar) {	}
		public override ObjectState CalcLinkState(NavBarItemLink link, ObjectState state) {
			return state;
		}
		protected override Brush GetLinkCaptionDisabledBrush(ObjectInfoArgs e, Brush foreBrush) {
			return null;
		}
		protected Skin Skin { get { return (NavBar.ViewInfo as SkinNavigationPaneViewInfo).Skin; } }
		protected override int CalcLinkIndentWithoutImage(int textX, NavLinkInfoArgs li) {
			SkinElement elem = Skin[NavPaneSkins.SkinCaption];
			return elem.ContentMargins.Left - textX;
		}
		protected SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(GetSkin(e), e.Bounds);
			info.State = e.State;
			info.ImageIndex = CalcImageIndex(e.State);
			return info;
		}
		int CalcImageIndex(ObjectState state) {
			if((state & ObjectState.Disabled) != 0) return 0;
			if((state & ObjectState.Pressed) != 0) return 2;
			if((state & ObjectState.Hot) != 0) return 1;
			return 0;
		}
		SkinElement GetSkin(ObjectInfoArgs e) {
			return NavPaneSkins.GetSkin(NavBar)[(e.State & ObjectState.Selected) != 0 ? NavPaneSkins.SkinItemSelected : NavPaneSkins.SkinItem];
		}
		protected override void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
			SkinElementInfo info = CreateInfo(e);
			info.Bounds = Rectangle.Inflate(e.Bounds, -1, 0);
			if(info.State == ObjectState.Normal) return; 
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected override int CalcLargeVertIndent(ObjectInfoArgs e) {
			return 4;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElementInfo info = CreateInfo(e);
			return ObjectPainter.CalcBoundsByClientRectangle(e.Graphics, SkinElementPainter.Default, info, client);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			SkinElementInfo info = CreateInfo(e);
			Rectangle r = info.Bounds;
			Rectangle realBounds = ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, info);
			e.Bounds = realBounds;
			Rectangle res = base.CalcObjectBounds(e);
			NavLinkInfoArgs linkInfo = e as NavLinkInfoArgs;
			if(linkInfo.Link.Item.IsSeparator())
				r.Height = res.Height;
			else
				r.Height = (res.Bottom + (r.Bottom - realBounds.Bottom)) - r.Top;
			e.Bounds = r;
			return r;
		}
	}
	public class SkinNavigationPaneGroupClientPainter : BaseNavGroupClientPainter {
		public SkinNavigationPaneGroupClientPainter(BaseNavGroupPainter groupPainter) : base(groupPainter) { }
		protected override void DrawBorder(NavGroupClientInfoArgs e) { }
		protected internal override int CalcGroupClientHeightByClientSize(Size clientSize) {
			SkinElementInfo info = GetClientInfo(Rectangle.Empty);
			return ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, info, new Rectangle(Point.Empty, clientSize)).Height;
		}
		protected override void CalcClientBounds(NavGroupClientInfoArgs e) {
			SkinElementInfo info = GetClientInfo(e.Bounds);
			e.ClientBounds = ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, info);
		}
		protected override AppearanceObject GetCollapsedAppearance(AppearanceObject src) {
			src = base.GetCollapsedAppearance(src);
			NavigationPaneViewInfo vi = NavBar.ViewInfo as NavigationPaneViewInfo;
			if(src.ForeColor == vi.HeaderInfo.Appearance.ForeColor)
				return NavPaneSkins.GetSkin(NavBar)[NavPaneSkins.SkinCollapsedGroupClient].GetForeColorAppearance(src, GetContentButtonState());
			return src;
		}
		protected override int CalcClientIndent(NavGroupClientInfoArgs e, IndentType indent) {
			if(e.Group.GroupStyle == NavBarGroupStyle.ControlContainer) return 0;
			if(indent == IndentType.Top) {
				return NavPaneSkins.GetSkin(NavBar).Properties.GetInteger(NavPaneSkins.OptGroupClientTopIndent);
			}
			return base.CalcClientIndent(e, indent);
		}
		ObjectState GetContentButtonState() {
			NavigationPaneViewInfo vi = NavBar.ViewInfo as NavigationPaneViewInfo;
			if(vi == null) return ObjectState.Normal;
			if (!NavBar.Enabled) return ObjectState.Disabled;
			if (vi.PressedInfo.HitTest == NavBarHitTest.ContentButton) return ObjectState.Pressed;
			if (vi.HotInfo.HitTest == NavBarHitTest.ContentButton) return ObjectState.Hot;
			if (NavBar.NavPaneForm != null && NavBar.NavPaneForm.Visible) return ObjectState.Selected;
			return ObjectState.Normal;
		}
		protected override void DrawCollapsedClient(ObjectInfoArgs e) {
			string skinName = Skins.NavPaneSkins.SkinCollapsedGroupClient;
			if (ShouldUseClientWithBorder()) skinName = Skins.NavPaneSkins.SkinCollapsedGroupClientWithBorder;
			SkinElementInfo info = new SkinElementInfo(Skins.NavPaneSkins.GetSkin(NavBar)[skinName], e.Bounds);
			if (info == null || info.Element == null) {
				base.DrawCollapsedClient(e);
				return;
			}
			if(NavBar != null && NavBar.IsRightToLeft) info.RightToLeft = true;
			info.ImageIndex = -1;
			info.State = GetContentButtonState();
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected override void DrawBackgroundCore(NavGroupClientInfoArgs e, CustomDrawObjectEventArgs custom) {
			SkinElementInfo info = GetClientInfo(e.Bounds);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected virtual bool HasVisibleGroups() {
			for(int i = 0; i < NavBar.ViewInfo.Groups.Count; i++) { 
				NavGroupInfoArgs e = (NavBar.ViewInfo.Groups[i] as NavGroupInfoArgs);
				if(e.Bounds.Height > 0 || e.Bounds.Width > 0) return true;
			}
			return false;
		}
		protected virtual bool ShouldUseClientWithBorder() {
			return !NavBar.OptionsNavPane.ShowSplitter && !HasVisibleGroups();
		}
		protected virtual SkinElementInfo GetClientInfo(Rectangle rect) {
			string skinName = NavPaneSkins.SkinGroupClient;
			if(ShouldUseClientWithBorder()) skinName = NavPaneSkins.SkinGroupClientWithBorder;
			SkinElementInfo info = new SkinElementInfo(NavPaneSkins.GetSkin(NavBar)[skinName], rect);
			if(NavBar != null && NavBar.IsRightToLeft) info.RightToLeft = true;
			return info;
		}
	}
	public class SkinNavigationPaneGroupPainter : BaseNavigationPaneGroupPainter {
		public SkinNavigationPaneGroupPainter(NavBarControl navBar) : base(navBar) { }
		protected override BaseNavGroupClientPainter CreateGroupClientPainter() { 
			return new SkinNavigationPaneGroupClientPainter(this); 
		}
		protected override void DrawButton(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			Rectangle bounds = e.Bounds;
			ButtonPainter.DrawObject(GetButtonArgs(e, bounds, custom.Appearance, e.State));
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			NavGroupInfoArgs ne = e as NavGroupInfoArgs;
			SkinElementInfo info = new SkinElementInfo(NavPaneSkins.GetSkin(ne.Group.NavBar)[NavPaneSkins.SkinGroupButton], e.Bounds);
			return ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, info);
		}
		protected override ObjectPainter CreateButtonPainter() { return new SkinNavigationGroupButton(NavBar); }
		protected override int TextHeightAdd { get { return 0; } }
		protected override int TextHeightAddExtra { get { return 0; } }
		protected override int TextHeightMin { get { return 0; } }
		protected override int HorzIndent { get { return 6; } }
		protected class SkinNavigationGroupButton : SkinCustomPainter {
			public SkinNavigationGroupButton(ISkinProvider provider) : base(provider) { }
			int CalcImageIndex(ObjectState state) {
				if((state & ObjectState.Disabled) != 0) return 0;
				if((state & ObjectState.Pressed) != 0) return 2;
				if((state & ObjectState.Hot) != 0) return 1;
				return 0;
			}
			SkinElement GetSkin(ObjectInfoArgs e) {
				return NavPaneSkins.GetSkin(Provider)[(e.State & ObjectState.Selected) != 0 ? NavPaneSkins.SkinGroupButtonSelected : NavPaneSkins.SkinGroupButton];
			}
			protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
				SkinElementInfo res = new SkinElementInfo(GetSkin(e), e.Bounds);
				res.State = e.State;
				res.ImageIndex = CalcImageIndex(e.State);
				return res;
			}
		}
	}
}
