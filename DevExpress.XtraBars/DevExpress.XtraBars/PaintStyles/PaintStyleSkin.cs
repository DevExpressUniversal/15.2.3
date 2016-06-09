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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Objects;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraBars.InternalItems;
using DevExpress.Skins.XtraForm;
namespace DevExpress.XtraBars.ViewInfo {
	public class SkinBarColorConstants : BarOffice2003ColorConstants {
		public SkinBarColorConstants(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected override void InitColors() {
			Skin skin = GetSkin();
			Font font = DrawParameters.ItemsFont;
			this.fMenuAppearance = new MenuAppearance();
			fAppearances[BarAppearance.Dock] = new AppearanceObject(skin[BarSkins.SkinDock].GetAppearanceDefault());
			fAppearances[BarAppearance.Bar] = new StateAppearances(skin[BarSkins.SkinBar].GetAppearanceDefault());
			fAppearances[BarAppearance.BarNoBorder] = new StateAppearances(skin[BarSkins.SkinBar].GetAppearanceDefault());
			fAppearances[BarAppearance.MainMenu] = new StateAppearances(skin[BarSkins.SkinMainMenu].GetAppearanceDefault());
			fAppearances[BarAppearance.StatusBar] = new StateAppearances(skin[BarSkins.SkinStatusBar].GetAppearanceDefault());
			StateAppearance(BarAppearance.StatusBar).SetFont(font);
			StateAppearance(BarAppearance.MainMenu).SetFont(font);
			Appearance(BarAppearance.Dock).Font = font;
			StateAppearance(BarAppearance.Bar).SetFont(font);
			StateAppearance(BarAppearance.Bar).Disabled.ForeColor = skin.GetSystemColor(SystemColors.GrayText);
			StateAppearance(BarAppearance.Bar).Hovered.ForeColor = skin[BarSkins.SkinBar].GetForeColor(StateAppearance(BarAppearance.Bar).Hovered, ObjectState.Hot);
			StateAppearance(BarAppearance.Bar).Pressed.ForeColor = skin[BarSkins.SkinBar].GetForeColor(StateAppearance(BarAppearance.Bar).Pressed, ObjectState.Pressed);
			StateAppearance(BarAppearance.Bar).Disabled.ForeColor = skin[BarSkins.SkinBar].GetForeColor(StateAppearance(BarAppearance.Bar).Disabled, ObjectState.Disabled);
			StateAppearance(BarAppearance.BarNoBorder).Disabled.ForeColor = skin.GetSystemColor(SystemColors.GrayText);
			if(skin[BarSkins.SkinBar].Properties.ContainsProperty(BarSkins.OptBarNoBorderForeColor))
				StateAppearance(BarAppearance.BarNoBorder).Normal.ForeColor = skin[BarSkins.SkinBar].Properties.GetColor(BarSkins.OptBarNoBorderForeColor);
			if(skin[BarSkins.SkinBar].Properties.ContainsProperty(BarSkins.OptBarNoBorderForeColorHot))
				StateAppearance(BarAppearance.BarNoBorder).Hovered.ForeColor = skin[BarSkins.SkinBar].Properties.GetColor(BarSkins.OptBarNoBorderForeColorHot);
			if(skin[BarSkins.SkinBar].Properties.ContainsProperty(BarSkins.OptBarNoBorderForeColorPressed))
				StateAppearance(BarAppearance.BarNoBorder).Pressed.ForeColor = skin[BarSkins.SkinBar].Properties.GetColor(BarSkins.OptBarNoBorderForeColorPressed);
			if(skin[BarSkins.SkinBar].Properties.ContainsProperty(BarSkins.OptBarNoBorderForeColorDisabled))
				StateAppearance(BarAppearance.BarNoBorder).Disabled.ForeColor = skin[BarSkins.SkinBar].Properties.GetColor(BarSkins.OptBarNoBorderForeColorDisabled);
			StateAppearance(BarAppearance.MainMenu).Disabled.ForeColor = skin.GetSystemColor(SystemColors.GrayText);
			StateAppearance(BarAppearance.MainMenu).Hovered.ForeColor = skin[BarSkins.SkinMainMenu].GetForeColor(StateAppearance(BarAppearance.MainMenu).Hovered, ObjectState.Hot);
			StateAppearance(BarAppearance.MainMenu).Pressed.ForeColor = skin[BarSkins.SkinMainMenu].GetForeColor(StateAppearance(BarAppearance.MainMenu).Pressed, ObjectState.Pressed);
			StateAppearance(BarAppearance.MainMenu).Disabled.ForeColor = skin[BarSkins.SkinMainMenu].GetForeColor(StateAppearance(BarAppearance.MainMenu).Disabled, ObjectState.Disabled);
			StateAppearance(BarAppearance.StatusBar).Disabled.ForeColor = skin.GetSystemColor(SystemColors.GrayText);
			StateAppearance(BarAppearance.StatusBar).Hovered.ForeColor = skin[BarSkins.SkinStatusBar].GetForeColor(StateAppearance(BarAppearance.StatusBar).Hovered, ObjectState.Hot);
			StateAppearance(BarAppearance.StatusBar).Pressed.ForeColor = skin[BarSkins.SkinStatusBar].GetForeColor(StateAppearance(BarAppearance.StatusBar).Pressed, ObjectState.Pressed);
			StateAppearance(BarAppearance.StatusBar).Disabled.ForeColor = skin[BarSkins.SkinStatusBar].GetForeColor(StateAppearance(BarAppearance.StatusBar).Disabled, ObjectState.Disabled);
			this.fMenuAppearance.AppearanceMenu.Assign(skin[BarSkins.SkinPopupMenu].GetAppearanceDefault());
			this.fMenuAppearance.AppearanceMenu.SetFont(font);
			this.fMenuAppearance.AppearanceMenu.Hovered.ForeColor = skin[BarSkins.SkinPopupMenu].GetForeColor(this.fMenuAppearance.AppearanceMenu.Hovered, ObjectState.Hot);
			this.fMenuAppearance.AppearanceMenu.Pressed.ForeColor = skin[BarSkins.SkinPopupMenu].GetForeColor(this.fMenuAppearance.AppearanceMenu.Pressed, ObjectState.Pressed);
			this.fMenuAppearance.AppearanceMenu.Disabled.ForeColor = skin.GetSystemColor(SystemColors.GrayText);
			this.fMenuAppearance.AppearanceMenu.Disabled.ForeColor = skin[BarSkins.SkinPopupMenu].GetForeColor(this.fMenuAppearance.AppearanceMenu.Disabled, ObjectState.Disabled);
			this.fMenuAppearance.SideStrip.Assign(skin[BarSkins.SkinPopupMenuSideStrip].GetAppearanceDefault());
			this.fMenuAppearance.SideStripNonRecent.Assign(skin[BarSkins.SkinPopupMenuSideStripNonRecent].GetAppearanceDefault());
			this.fMenuAppearance.MenuBar.Assign(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font));
			this.fMenuAppearance.HeaderItemAppearance.Font = new Font(font, FontStyle.Bold);
			AppearanceDefault caption = new AppearanceDefault(Color.Black, Color.Empty);
			SkinElement element = RibbonSkins.GetSkin(PaintStyle as SkinBarManagerPaintStyle)[RibbonSkins.SkinPopupGalleryItemCaption];
			if(element != null)
				element.Apply(caption);
			MenuItemCaptionWithDescription.Assign(caption);
			caption = new AppearanceDefault(Color.Black, Color.Empty);
			element = RibbonSkins.GetSkin(PaintStyle as SkinBarManagerPaintStyle)[RibbonSkins.SkinAppMenuItemDescription];
			if(element == null)
				element = RibbonSkins.GetSkin(PaintStyle as SkinBarManagerPaintStyle)[RibbonSkins.SkinPopupGalleryItemSubCaption];
			if(element != null)
				element.Apply(caption);
			MenuItemDescription.Assign(caption);
			MenuItemDescription.SetWordWrap(WordWrap.Wrap);
			MenuItemDescription.SetVAlignment(VertAlignment.Top);
			if(element != null) {
				MenuItemDescription.Hovered.ForeColor = element.GetForeColor(MenuItemDescription.Hovered, ObjectState.Hot);
				MenuItemDescription.Pressed.ForeColor = element.GetForeColor(MenuItemDescription.Pressed, ObjectState.Pressed);
				MenuItemDescription.Disabled.ForeColor = element.GetForeColor(MenuItemDescription.Disabled, ObjectState.Disabled);
			}
			caption = new AppearanceDefault(Color.Black, Color.Empty);
			element = RibbonSkins.GetSkin(PaintStyle as SkinBarManagerPaintStyle)[RibbonSkins.SkinPopupGalleryGroupCaption];
			if(element != null)
				element.Apply(caption);
			this.fMenuAppearance.MenuCaption.Assign(caption);
			fColors[BarColor.LinkForeColor] = StateAppearance(BarAppearance.Bar).Normal.ForeColor;
			fColors[BarColor.LinkDisabledForeColor] = GetSkinColor(skin, BarSkins.ColorLinkDisabledForeColor);
			fColors[BarColor.InStatusBarLinkCheckedForeColor] = GetSkinColor(skin, BarSkins.ColorInStatusBarLinkCheckedForeColor, (Color)fColors[BarColor.LinkForeColor]);
			AppearanceDefault 
				floating = skin[BarSkins.SkinFloatingBar].GetAppearanceDefault();
			fColors[BarColor.FloatingBarBorderColor] = floating.BorderColor;
			fColors[BarColor2003.FloatingTitleBarBackColor] = floating.BackColor;
			fColors[BarColor2003.FloatingTitleBarBorderColor] = floating.BorderColor;
			fColors[BarColor2003.FloatingTitleBarForeColor] = floating.ForeColor;
			fColors[BarColor.DesignTimeSelectColor] = Color.LightPink;
			fColors[BarColor.TabBackColor] = DevExpress.Utils.ColorUtils.FlatTabBackColor;
		}
		Skin GetSkin() { return BarSkins.GetSkin(PaintStyle as SkinBarManagerPaintStyle); }
		Color GetSkinColor(Skin skin, string color) {
			return skin.Colors.GetColor(color);
		}
		Color GetSkinColor(Skin skin, string color, Color defaultColor) {
			return skin.Colors.GetColor(color, defaultColor);
		}
	}
	public class SkinBarDrawParameters : BarDrawParameters {
		public SkinBarDrawParameters(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected override BarColorConstants CreateColors() { return new SkinBarColorConstants(PaintStyle); } 
		protected override BarConstants CreateConstants() { return new SkinBarConstants(PaintStyle); } 
		public override Color GetLinkForeColor(BarLinkViewInfo linkInfo, Color foreColor, BarLinkState state) {
			string elementName = BarSkins.SkinBar;
			if(linkInfo.Bar == null && !linkInfo.IsLinkInMenu) return foreColor;
			if(linkInfo.ShouldDrawCheckedCaption(linkInfo, state))
				return GetSkin().Colors[BarSkins.ColorInStatusBarLinkCheckedForeColor];
			if(linkInfo.Bar != null) {
				if(linkInfo.Bar.DockStyle == BarDockStyle.None) return foreColor;
				if(linkInfo.Bar.IsStatusBar) elementName = BarSkins.SkinStatusBar;
				if(linkInfo.Bar.IsMainMenu) elementName = BarSkins.SkinMainMenu;
			}
			if(linkInfo.IsLinkInMenu) elementName = BarSkins.SkinPopupMenu;
			Color res = GetSkin()[elementName].GetForeColor(GetState(state));
			if(res.IsEmpty) return foreColor;
			return res;
		}
		public override bool CanDrawCheckedCaption {
			get { return GetSkin().Colors.Contains(BarSkins.ColorInStatusBarLinkCheckedForeColor); }
		}
		public override Size GetCheckSize() {
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(PaintStyle as SkinBarManagerPaintStyle)[EditorsSkins.SkinCheckBox]);
			return ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, info).Size;
		}
		public override void UpdateMdiGlyphs(BarMdiButtonItem item, string name) {
			FormCaptionButtonKind kind = FormCaptionButtonKind.None;
			switch(name) {
				case "MinImage": kind = FormCaptionButtonKind.MdiMinimize; break;
				case "RestoreImage": kind = FormCaptionButtonKind.MdiRestore; break;
				case "CloseImage": kind = FormCaptionButtonKind.MdiClose; break;
			}
			if(kind == FormCaptionButtonKind.None) {
				base.UpdateMdiGlyphs(item, name);
				return;
			}
			FormCaptionButtonSkinPainter buttonPainter = new FormCaptionButtonSkinPainter(PaintStyle as SkinBarManagerPaintStyle);
			FormCaptionButton info = new FormCaptionButton(kind);
			item.LargeGlyph = buttonPainter.DrawElementIntoBitmap(info, ObjectState.Normal);
			item.LargeGlyphPressed = buttonPainter.DrawElementIntoBitmap(info, ObjectState.Pressed);
			item.LargeGlyphHot = buttonPainter.DrawElementIntoBitmap(info, ObjectState.Hot);
			item.LargeGlyphDisabled = buttonPainter.DrawElementIntoBitmap(info, ObjectState.Disabled);
		}
		public override Color GetLinkForeColorDisabled(BarLinkViewInfo linkInfo, BarLinkState state) {
			return GetLinkForeColor(linkInfo, Colors[BarColor.LinkDisabledForeColor], state);
		}
		public override Color GetInStatusBarLinkForeColorChecked(BarLinkViewInfo linkInfo, BarLinkState state) {
			return GetLinkForeColor(linkInfo, Colors[BarColor.InStatusBarLinkCheckedForeColor], state);
		}
		public override Font ItemsFont { 
			get {
				return GetSkin()[BarSkins.SkinBar].GetFont(Controller.AppearancesBar.ItemsFont);
			} 
		}
		ObjectState GetState(BarLinkState linkState) {
			ObjectState res = ObjectState.Normal;
			if((linkState & BarLinkState.Disabled) != 0) return ObjectState.Disabled;
			if((linkState & BarLinkState.Pressed) != 0) res |= ObjectState.Pressed;
			if((linkState & BarLinkState.Highlighted) != 0) res |= ObjectState.Hot;
			if((linkState & BarLinkState.Checked) != 0) res |= ObjectState.Hot;
			if((linkState & BarLinkState.Selected) != 0) res |= ObjectState.Selected;
			return res;
		}
		Skin GetSkin() { return BarSkins.GetSkin(PaintStyle as SkinBarManagerPaintStyle); }
	}
	public class SkinFloatingBarControlViewInfo : FloatingBarControlViewInfo {
		public SkinFloatingBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) { }
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		protected Skin Skin { get { return PaintStyle.Skin; } }
		protected override bool AllowRowSeparator { get { return false; } }
	}
	public class SkinFloatingBarControlFormViewInfo : FloatingBarControlFormViewInfo {
		public SkinFloatingBarControlFormViewInfo(BarManager manager, BarDrawParameters parameters, ControlForm controlForm) : base(manager, parameters, controlForm) {
		}
		public override TitleBarEl CreateTitleBarInstance() { return new SkinFloatingBarTitleBarEl(ControlForm.Manager); }
		protected override Size CalcBorderSize() { return new Size(1, 1); }
		protected override int CalcContentIndent(BarIndent indent) {
			return 1;
		}
	}
	public class SkinDockedBarControlViewInfo : DockedBarControlViewInfo {
		public SkinDockedBarControlViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) { }
		public override void UpdateControlRegion(Control control) { control.Region = null; }
		protected override Rectangle CalcQuickCustomizationLinkBounds(Rectangle barBounds, Rectangle linkBounds, Size linkSize) {
			Rectangle r = linkBounds;
			if(IsVertical) {
				r.Height = linkSize.Height;
				r.Y = (barBounds.Bottom - r.Height) + CalcIndent(BarIndent.Right); 
			} else {
				r.Width = linkSize.Width;
				r.X = (barBounds.Right - r.Width) + (IsRightToLeft ? 0 : CalcIndent(BarIndent.Right));
			}
			return r;
		}
		public override bool LinksUseRealBounds { get { return true; } }
	}
	public class SkinBarConstants : BarConstants {
		public SkinBarConstants(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		protected Skin Skin { get { return PaintStyle.Skin; } }
		public override void Init() {
			base.Init();
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				BarItemVertIndent = -1;
				BarItemHorzIndent = -1;
				SubMenuScrollLinkHeight = 10;
				SubMenuScrollLinkHeightInTouch = 15;
				BarDockedRowIndent = GetIntProperty(BarSkins.SkinBarDockedRowIndent, 1);
				BarDockedRowBarIndent = GetIntProperty(BarSkins.SkinBarDockedRowBarIndent, BarDockedRowBarIndent);
				DragBorderSize = ObjectPainter.CalcObjectMinBounds(ginfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin[BarSkins.SkinBarDrag])).Width;
				BarQuickButtonWidth = ObjectPainter.CalcObjectMinBounds(ginfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin[BarSkins.SkinBarCustomize])).Width;
				BarSeparatorLineThickness = ObjectPainter.CalcObjectMinBounds(ginfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin[BarSkins.SkinBarSeparator])).Width;
				SubMenuSeparatorHeight = ObjectPainter.CalcObjectMinBounds(ginfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin[BarSkins.SkinPopupMenuSeparator])).Height;
				BarSeparatorWidth = BarSeparatorLineThickness;
				int w = ObjectPainter.CalcBoundsByClientRectangle(ginfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin[BarSkins.SkinPopupMenuSideStrip]), Rectangle.Empty).Width;
				SubMenuGlyphHorzIndent = w / 2;
				SubMenuGlyphCaptionIndent = w - SubMenuGlyphHorzIndent;
				SubMenuItemBottomIndent = SubMenuItemTopIndent = ObjectPainter.CalcBoundsByClientRectangle(ginfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin[BarSkins.SkinPopupMenuLinkSelected]), Rectangle.Empty).Height / 2;
				SubMenuRecentExpanderHeight = ObjectPainter.CalcObjectMinBounds(ginfo.Graphics, SkinElementPainter.Default, new SkinElementInfo(Skin[BarSkins.SkinPopupMenuExpandButton])).Height;
			} finally {
				ginfo.ReleaseGraphics();
			}
		}
		ISkinProviderEx SkinProviderEx { get { return PaintStyle as ISkinProviderEx; } }
		public override int GetBarButtonArrowWidth() {
			int res = base.GetBarButtonArrowWidth();
			if(SkinProviderEx.GetTouchUI())
				return (int)(res * SkinProviderEx.GetTouchScaleFactor());
			return res;
		}
		public override int GetBarMenuButtonArrowWidth() {
			int res = base.GetBarMenuButtonArrowWidth();
			if(SkinProviderEx.GetTouchUI())
				return (int)(res * SkinProviderEx.GetTouchScaleFactor());
			return res;
		}
		public override int GetSubMenuArrowWidth() {
			int res = base.GetSubMenuArrowWidth();
			if(SkinProviderEx.GetTouchUI())
				return (int)(res * SkinProviderEx.GetTouchScaleFactor());
			return res;
		}
		public override bool AllowLinkGlyphShadow { get { return false; } }
		public override bool AllowLinkLighting { get { return false; } }
		public override bool AllowLinkShadows { get { return false; } }
		protected virtual object GetProperty(string propName) {
			if(Skin == null) return null;
			return Skin.Properties[propName];
		}
		protected virtual int GetIntProperty(string propName, int defValue) {
			object prop = GetProperty(propName);
			if(prop == null || !(prop is int) ) return defValue;
			return (int)prop;
		}
	}
}
namespace DevExpress.XtraBars.Painters {
	public class SkinBarRecentExpanderLinkPainter : BarRecentExpanderLinkPainter {
		protected Skin Skin { get { return PaintStyle.Skin; } }
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		public SkinBarRecentExpanderLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected override void DrawLinkAdornmentsInMenu(BarLinkPaintArgs e, BarLinkState drawState) {
			Rectangle r = e.LinkInfo.Bounds;
			SkinElementInfo info = new SkinElementInfo(Skin[BarSkins.SkinPopupMenuExpandButton], r);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class SkinControlFormPainter : ControlFormPainter {
		public SkinControlFormPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected Skin Skin { get { return PaintStyle.Skin; } }
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		public override void DrawBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) { 
			DrawRealBorder(e, vi);
		}
		protected override void DrawBackground(GraphicsInfoArgs e, ControlFormViewInfo vi) {
		}
		protected override void DrawRealBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			SkinElementInfo info = new SkinElementInfo(Skin[BarSkins.SkinPopupMenu], vi.WindowRect);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class SkinPopupControlContainerBarPainter : SkinBarSubMenuPainter {
		public SkinPopupControlContainerBarPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
			this.sizeGripPainter = new SkinSizeGripObjectPainter(paintStyle.Controller.LookAndFeel);
		}
		SizeGripObjectPainter sizeGripPainter;
		public SizeGripObjectPainter SizeGripPainer { get { return sizeGripPainter; } }
		public override void Draw(GraphicsInfoArgs e, CustomViewInfo info, object sourceInfo) {
			PopupContainerControlViewInfo vi = info as PopupContainerControlViewInfo;
			if(vi == null) {
				base.Draw(e, info, sourceInfo);
				return;
			}
			DrawBackground(e, vi);
			DrawNavigationBackground(e, vi);
			DrawNavigationItems(e, vi);
			if(vi.ShowSizeGrip)
				DrawSizeGrip(e, vi);
		}
		protected override void DrawBackArrow(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetBackArrowInfo(vi.NavigationArrowBounds));
		}
		protected virtual void DrawSizeGrip(GraphicsInfoArgs e, PopupContainerControlViewInfo vi) {
			vi.SizeGripInfo.SetAppearance(vi.Appearance.Normal);
			vi.SizeGripInfo.Cache = e.Cache;
			SizeGripPainer.DrawObject(vi.SizeGripInfo);
		}
		protected virtual void DrawBackground(GraphicsInfoArgs e, PopupContainerControlViewInfo vi) { 
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetBackgroundInfo(vi.ButtonsRect));
		}
		protected virtual SkinElementInfo GetBackgroundInfo(Rectangle rect) {
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(PaintStyle.Controller.LookAndFeel)[CommonSkins.SkinGroupPanelNoBorder], rect);
			info.BackAppearance.BackColor = Color.Transparent;
			return info;
		}
	}
	public class SkinBarSubMenuPainter : BarSubMenuPainter {
		protected Skin Skin { get { return PaintStyle.Skin; } }
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		public SkinBarSubMenuPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {	}
		protected virtual SkinElementInfo GetBackgroundInfo(CustomSubMenuBarControlViewInfo vi) {
			return new SkinElementInfo(Skin[BarSkins.SkinPopupMenu], vi.Bounds);
		}
		protected override void DrawNavigationBackground(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
			if(!vi.ShowNavigationHeader)
				return;
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinPopupGalleryGroupCaption], vi.NavigationHeaderBounds);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected virtual SkinElementInfo GetBackArrowInfo(Rectangle rect) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle.Controller.LookAndFeel)[BarSkins.SkinNavigationHeaderArrowBack], rect);
			return info;
		}
		protected override void DrawBackArrow(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetBackArrowInfo(vi.NavigationArrowBounds));
		}
		protected internal override bool DrawEditorBackground(Control ownerEdit, CustomControlViewInfo info, GraphicsCache cache) {
			SkinElementInfo backInfo = GetBackgroundInfo(info as CustomSubMenuBarControlViewInfo);
			Rectangle local = new Rectangle(backInfo.Bounds.X - ownerEdit.Bounds.X, backInfo.Bounds.Y - ownerEdit.Bounds.Y, backInfo.Bounds.Width, backInfo.Bounds.Height);
			backInfo.Bounds = local;
			if(backInfo.Element.Image == null)
				info.Appearance.Normal.DrawBackground(cache.Graphics, cache, local);
			else 
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, backInfo);
			return true;
		}
		public override void DrawMenuBackground(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
			SkinElementInfo info = GetBackgroundInfo(vi);
			if(info.Element.Image == null)
				vi.Appearance.Normal.DrawBackground(e.Graphics, e.Cache, vi.Bounds);
			else {
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			}
		}
		public override Size CalcCaptionSize(Graphics g, CustomSubMenuBarControlViewInfo vi, string caption) {
			Size res = vi.AppearanceMenu.MenuCaption.CalcTextSize(g, caption, 0).ToSize();
			res.Height += 1;
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinPopupGalleryGroupCaption]);
			return ObjectPainter.CalcBoundsByClientRectangle(g, SkinElementPainter.Default, info, new Rectangle(Point.Empty, res)).Size;
		}
		public override void DrawMenuCaption(GraphicsInfoArgs e, CustomSubMenuBarControlViewInfo vi) {
			if(vi.MenuCaptionBounds.IsEmpty) return;
			Rectangle r = vi.MenuCaptionBounds;
			r.Height --;
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(PaintStyle)[RibbonSkins.SkinPopupGalleryGroupCaption], r);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			r = ObjectPainter.GetObjectClientRectangle(e.Graphics, SkinElementPainter.Default, info);
			vi.AppearanceMenu.MenuCaption.DrawString(e.Cache, vi.MenuCaption, r);
		}
	}
	public class SkinFloatingBarPainter : FloatingBarPainter { 
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		protected Skin Skin { get { return PaintStyle.Skin; } }
		public SkinFloatingBarPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		public override void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetBackgroundInfo(info));
		}
		protected SkinElementInfo GetBackgroundInfo(BarControlViewInfo info) {
			return new SkinElementInfo(Skin[BarSkins.SkinFloatingBar], info.Bounds);
		} 
		public override void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
		}
		protected internal override bool DrawEditorBackground(Control ownerEdit, CustomControlViewInfo info, GraphicsCache cache) {
			SkinElementInfo backInfo = GetBackgroundInfo(info as BarControlViewInfo);
			Rectangle local = new Rectangle(backInfo.Bounds.X - ownerEdit.Bounds.X, backInfo.Bounds.Y - ownerEdit.Bounds.Y, backInfo.Bounds.Width, backInfo.Bounds.Height);
			backInfo.Bounds = local;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, backInfo);
			return true;
		}
		public override int CalcIndent(BarIndent indent, BarControlViewInfo viewInfo) {
			int res = 0;
			viewInfo.GInfo.AddGraphics(null);
			try {
				SkinPaddingEdges margins = Skin[BarSkins.SkinFloatingBar].ContentMargins;
				switch(indent) {
					case BarIndent.Left : res += margins.Left; break;
					case BarIndent.Right : res += margins.Right; break;
					case BarIndent.Top : res += margins.Top; break;
					case BarIndent.Bottom : res += margins.Bottom; break;
				}
			} finally {
				viewInfo.GInfo.ReleaseGraphics();
			}
			return res;
		}
	}
	public class SkinTitleBarPainter : BarPainter {
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		protected Skin Skin { get { return PaintStyle.Skin; } }
		public SkinTitleBarPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		protected override void DrawLink(GraphicsInfoArgs e, BarControlViewInfo viewInfo, BarLinkViewInfo item) {
			TitleBarControl titleBar = item.BarControl as TitleBarControl;
			SkinTitleBarButtonPainter bp = new SkinTitleBarButtonPainter(PaintStyle);
			StyleObjectInfoArgs info = new StyleObjectInfoArgs(e.Cache);
			if((item.LinkState & BarLinkState.Highlighted) != 0) info.State = ObjectState.Hot;
			if((item.LinkState & BarLinkState.Pressed) != 0) info.State = ObjectState.Pressed;
			info.Bounds = item.Bounds;
			ObjectPainter.DrawObject(e.Cache, bp, info);
			Rectangle image = ObjectPainter.GetObjectClientRectangle(e.Graphics, bp, info);
			image = RectangleHelper.GetCenterBounds(image, item.LinkGlyphSize);
			e.Cache.Paint.DrawImage(e.Graphics, item.Link.Glyph, image);
		}
		class SkinTitleBarButtonPainter : SkinCustomPainter {
			public SkinTitleBarButtonPainter(ISkinProvider provider) : base(provider) { }
			protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) { 
				SkinElementInfo info = new SkinElementInfo(DockingSkins.GetSkin(Provider)[DockingSkins.SkinDockWindowButton]);
				info.ImageIndex = -1;
				return info;
			}
		}
	}
	public class SkinBarPainter : BarPainter {
		public SkinBarPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		protected Skin Skin { get { return PaintStyle.Skin; } }
		protected virtual void DrawMainMenuBackground(GraphicsInfoArgs e, BarControlViewInfo info) {
			DrawBarBackgroundCore(e, info, true);
		}
		protected virtual void DrawBarBackground(GraphicsInfoArgs e, BarControlViewInfo info) {
			DrawBarBackgroundCore(e, info, false);
		}
		protected internal override bool DrawEditorBackground(Control ownerEdit, CustomControlViewInfo info, GraphicsCache cache) {
			SkinElementInfo backInfo = GetBarBackgroundInfo(info as BarControlViewInfo);
			Rectangle local = new Rectangle(backInfo.Bounds.X - ownerEdit.Bounds.X, backInfo.Bounds.Y - ownerEdit.Bounds.Y, backInfo.Bounds.Width, backInfo.Bounds.Height);
			BarControlViewInfo vi = (BarControlViewInfo)info;
			if(vi.Bar != null && !vi.Bar.OptionsBar.DrawBorder) {
				((DockControlViewInfo)((BarDockControl)vi.BarControl.Parent).ViewInfo).Painter.DrawEditorBackground(ownerEdit, ((BarDockControl)vi.BarControl.Parent).ViewInfo, cache);
				return true;
			}
			backInfo.Bounds = local;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, backInfo);
			return true;
		}
		protected virtual SkinElementInfo GetBarBackgroundInfo(BarControlViewInfo info) {
			bool isVerticalBar = info.BarControl != null && info.BarControl.IsVertical;
			SkinElement element = GetSkinElement(info);
			Rectangle bounds = info.Bounds;
			if(!info.DragBorder.IsEmpty) {
				if(isVerticalBar) {
					bounds.Y = info.DragBorder.Bottom;
					bounds.Height = info.Bounds.Bottom - bounds.Y;
				}
				else {
					if(info.IsRightToLeft) {
						bounds.Width = info.DragBorder.X - bounds.X;
					}
					else {
						bounds.X = info.DragBorder.Right;
						bounds.Width = info.Bounds.Right - bounds.X;
					}
				}
			}
			DockedBarControlViewInfo docked = info as DockedBarControlViewInfo;
			if(docked != null && docked.GetQuickCustomizationLink() != null) {
				Rectangle quick = docked.GetQuickCustomizationLink().SelectRect;
				if(isVerticalBar) {
					bounds.Height = quick.Y - bounds.Y;
				}
				else {
					if(info.IsRightToLeft) {
						bounds.Width = bounds.Right - quick.Right;
						bounds.X = quick.Right;
					} else
						bounds.Width = quick.X - bounds.X;
				}
			}
			SkinElementInfo res = new SkinElementInfo(element, bounds) { RightToLeft = info.IsRightToLeft };
			UpdateAllowedParts(info, res);
			return res;
		}
		private void UpdateAllowedParts(BarControlViewInfo info, SkinElementInfo sinfo) {
			if(info.IsVertical) {
				sinfo.AllowedParts = SkinImagePart.MiddleLeft | SkinImagePart.MiddleCenter | SkinImagePart.MiddleRight;
				if(info.Bar == null)
					return;
				if(!info.Bar.OptionsBar.AllowQuickCustomization) {
					sinfo.AllowedParts |= SkinImagePart.BottomRight | SkinImagePart.BottomCenter | SkinImagePart.BottomLeft;
				}
				if(!info.Bar.OptionsBar.DrawDragBorder) {
					sinfo.AllowedParts |= SkinImagePart.TopLeft | SkinImagePart.TopCenter | SkinImagePart.TopRight;
				}
				return;
			}
			sinfo.AllowedParts = SkinImagePart.TopCenter | SkinImagePart.BottomCenter | SkinImagePart.MiddleCenter;
			if(info.Bar == null)
				return;
			if(!info.Bar.OptionsBar.AllowQuickCustomization) {
				sinfo.AllowedParts |= SkinImagePart.TopRight | SkinImagePart.BottomRight | SkinImagePart.MiddleRight;
			}
			if(!info.Bar.OptionsBar.DrawDragBorder) {
				sinfo.AllowedParts |= SkinImagePart.TopLeft | SkinImagePart.BottomLeft | SkinImagePart.MiddleLeft;
			}
		}
		protected virtual void DrawBarBackgroundCore(GraphicsInfoArgs e, BarControlViewInfo info, bool isMainMenu) {
			SkinElementInfo backInfo = GetBarBackgroundInfo(info);
			if(info.IsVertical)
				(new RotateObjectPaintHelper()).DrawRotated(e.Cache, backInfo, SkinElementPainter.Default, RotateFlipType.Rotate90FlipX, false);
			else 
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, backInfo);
		}
		protected virtual SkinElement GetSkinElement(BarControlViewInfo info) {
			bool isVertical = info.IsVertical;
			SkinElement element = Skin[BarSkins.SkinBar];
			if(info.Bar != null && info.IsStatusBar && !isVertical && !info.Bar.IsFloating) {
				element = Skin[BarSkins.SkinStatusBar];
			}
			if(info.Bar != null && info.IsMainMenu) {
				element = Skin[BarSkins.SkinMainMenu];
			}
			if(!isVertical && info.IsStatusBar)
				element = Skin[BarSkins.SkinStatusBar];
			return element;
		}
		public override int CalcIndent(BarIndent indent, BarControlViewInfo viewInfo) {
			int res = 0;
			bool isVertical = viewInfo.IsVertical;
			viewInfo.GInfo.AddGraphics(null);
			try {
				SkinPaddingEdges margins = GetSkinElement(viewInfo).ContentMargins;
				switch(indent) {
					case BarIndent.SizeGrip:
						if(viewInfo.DrawSizeGrip) res += viewInfo.CalcSizeGripSize().Width;
						break;
					case BarIndent.DragBorder:
						if(viewInfo.DrawDragBorder) res += DrawParameters.Constants.DragBorderSize;
						break;
					case BarIndent.Left: res += margins.Left; break;
					case BarIndent.Right: res += margins.Right; break;
					case BarIndent.Top: res += margins.Top; break;
					case BarIndent.Bottom: res += margins.Bottom; break;
				}
			} finally {
				viewInfo.GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected virtual void DrawStatusBarBackground(GraphicsInfoArgs e, BarControlViewInfo info) {
			DrawBarBackgroundCore(e, info, false);
		}
		public override void DrawBackgroundSurfaceCore(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			if(info.Bar != null) {
				if(info.Bar.IsStatusBar)
					DrawStatusBarBackground(e, info);
				else {
					if(info.Bar.IsMainMenu)
						DrawMainMenuBackground(e, info);
					else
						DrawBarBackground(e, info);
				}
			}
			else {
				DrawBarBackgroundCore(e, info, false);
			}
		}
		public override void DrawBackground(GraphicsInfoArgs e, BarControlViewInfo info, object sourceInfo) {
			DrawBackgroundSurface(e, info, sourceInfo);
			DrawDragBorder(e, info);
			if(!info.SizeGrip.IsEmpty)
				DrawSizeGrip(e, info);
		}
		protected override void DrawDragBorder(GraphicsInfoArgs e, BarControlViewInfo info) {
			bool isVerticalBar = info.BarControl != null && info.BarControl.IsVertical;
			if(info.DragBorder.IsEmpty) return;
			string elementName = BarSkins.SkinBarDrag;
			if(info.Bar != null && info.Bar.IsMainMenu)
				elementName = BarSkins.SkinMainMenuDrag;
			if(Skin[elementName] == null) elementName = BarSkins.SkinBarDrag;
			SkinElement element = Skin[elementName];
			if(isVerticalBar) 
				(new RotateObjectPaintHelper()).DrawRotated(e.Cache, new SkinElementInfo(element, info.DragBorder), SkinElementPainter.Default, RotateFlipType.Rotate90FlipX, false);
			else 
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, new SkinElementInfo(element, info.DragBorder) { RightToLeft = info.IsRightToLeft });
		}
	}
	public class BarQBarCustomizationSkinLinkPainter : BarQBarCustomizationLinkPainter { 
		public BarQBarCustomizationSkinLinkPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		Skin Skin { get { return ((SkinBarManagerPaintStyle)PaintStyle).Skin; } }
		protected override void DrawLinkAdornments(BarLinkPaintArgs e, BarLinkState drawState) { }
		protected override void DrawLinkHorizontal(BarLinkPaintArgs e) {
			string element = BarSkins.SkinBarCustomize;
			if(e.LinkInfo.Bar != null && e.LinkInfo.Bar.IsMainMenu)
				element = BarSkins.SkinMainMenuCustomize;
			if(Skin[element] == null) element = BarSkins.SkinBarCustomize;
			DrawLinkSkin(e, Skin[element]);
		}
		protected override void DrawLinkVertical(BarLinkPaintArgs e) {
			DrawLinkHorizontal(e);
		}
		protected void DrawLinkSkin(BarLinkPaintArgs e, SkinElement element) {
			Rectangle bounds = e.LinkInfo.SelectRect;
			SkinElementInfo info = new SkinElementInfo(element, bounds);
			info.RightToLeft = e.LinkInfo.IsRightToLeft;
			if(e.LinkInfo.LinkState != BarLinkState.Normal) info.ImageIndex = 1;
			if(e.LinkInfo.DrawMode == BarLinkDrawMode.Vertical) {
				(new RotateObjectPaintHelper()).DrawRotated(e.Cache, info, SkinElementPainter.Default, RotateFlipType.Rotate90FlipX, false);
			}
			else 
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class SkinFloatingBarControlFormPainter : FloatingBarControlFormPainter {
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		public SkinFloatingBarControlFormPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		public override void DrawBorder(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			FloatingBarControlFormViewInfo v = vi as FloatingBarControlFormViewInfo;
			Pen pen = DrawParameters.Colors.Pens(BarColor.FloatingBarBorderColor);
			Rectangle r = vi.WindowRect;
			PaintHelper.DrawRectangle(e.Graphics, pen, r);
		}
		protected override void DrawBackground(GraphicsInfoArgs e, ControlFormViewInfo vi) {
			AppearanceDefault app = BarSkins.GetSkin(PaintStyle)[BarSkins.SkinFloatingBar].GetAppearanceDefault();
			FloatingBarControlFormViewInfo v = vi as FloatingBarControlFormViewInfo;
			PaintHelper.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(app.BackColor), v.ContentRect);
		}
	}
	public class SkinLinkBorderPainter : SkinCustomPainter {
		public SkinLinkBorderPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) { 
			return new SkinElementInfo(BarSkins.GetSkin(Provider)[BarSkins.SkinLinkBorderPainter]);
		}
	}
	public class SkinPrimitivesPainter : PrimitivesPainterOffice2003 { 
		public new SkinBarManagerPaintStyle PaintStyle { get { return base.PaintStyle as SkinBarManagerPaintStyle; } }
		Skin Skin { get { return PaintStyle.Skin; } }
		public SkinPrimitivesPainter(BarManagerPaintStyle paintStyle) : base(paintStyle) { 	}
		protected override MenuHeaderPainter CreateDefaultMenuHeaderPainter() {
			return new SkinMenuHeaderPainter();
		}
		public override void DrawLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkEmptyBorder border, Brush backBrush, BarLinkState drawState) {
			DrawLinkBackgroundInMenu(e);
			if(e.LinkInfo.Link.Item.ItemInMenuAppearance.GetAppearance(drawState).BackColor != Color.Empty) {
				e.LinkInfo.OwnerAppearances.GetAppearance(drawState).FillRectangle(e.Cache, e.LinkInfo.SelectRect);
				return;
			}
			Rectangle r = e.LinkInfo.SelectRect;
			SkinElementInfo info = new SkinElementInfo(Skin[BarSkins.SkinPopupMenuLinkSelected], r);
			info.ImageIndex = e.LinkInfo.DrawDisabled ? 1 : 0;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		public override void DrawSplitLinkHighlightedBackgroundInMenu(BarLinkPaintArgs e, BarLinkEmptyBorder border, Brush backBrush, BarLinkState drawState) {
			BarButtonLinkViewInfo linkInfo = e.LinkInfo as BarButtonLinkViewInfo;
			Rectangle arrow = e.LinkInfo.Rects[BarLinkParts.OpenArrow];
			Rectangle bounds = e.LinkInfo.SelectRect;
			if(linkInfo == null || arrow.IsEmpty) {
				DrawLinkHighlightedBackgroundInMenu(e, border, backBrush, drawState);
				return;
			}
			DrawLinkBackgroundInMenu(e);
			Rectangle r = bounds;
			r.Width = arrow.X - r.X;
			SkinElementInfo info = new SkinElementInfo(Skin[BarSkins.SkinPopupMenuSplitButton], r);
			info.ImageIndex = e.LinkInfo.DrawDisabled ? 1 : 0;
			if(linkInfo.Link.IsPopupVisible || arrow.Contains(linkInfo.MousePosition)) info.ImageIndex = 0;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			info.Element = Skin[BarSkins.SkinPopupMenuSplitButton2];
			r.X = r.Right; r.Width = bounds.Right - r.X;
			info.Bounds = r;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		public override void DrawLinkHighlightedFrame(BarLinkPaintArgs e, ref Rectangle r, BarLinkEmptyBorder border) { }
		protected override ObjectPainter CreateDefaultLinkBorderPainter() {
			return new SkinLinkBorderPainter(PaintStyle);
		}
		protected override void  DrawLinkSideStrip(BarLinkPaintArgs e) {
			Rectangle r = e.LinkInfo.SelectRect;
			r.Width = e.LinkInfo.GlyphRect.Width;
			if(r.Width != 0) {
				r.Width = e.LinkInfo.GlyphRect.Width + DrawParameters.Constants.SubMenuGlyphHorzIndent + DrawParameters.Constants.SubMenuGlyphCaptionIndent;
				SkinElementInfo info = new SkinElementInfo(Skin[e.LinkInfo.IsRecentLink ? BarSkins.SkinPopupMenuSideStrip : BarSkins.SkinPopupMenuSideStripNonRecent], r);
				if(e.LinkInfo.IsRightToLeft) {
					r.X = e.LinkInfo.SelectRect.Right - r.Width;
					info.RightToLeft = true;
					info.Bounds = r;
				}
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			}
		}
		public override void DrawSubmenuSeparator(GraphicsInfoArgs e, Rectangle rect, BarLinkViewInfo li, object sourceInfo) {
			CustomSubMenuBarControlViewInfo menuInfo = sourceInfo as CustomSubMenuBarControlViewInfo;
			if(menuInfo == null) return;
			Rectangle r = rect;
			r.Width = menuInfo.GlyphWidth;
			if(li.IsRightToLeft) r.X = rect.Right - r.Width;
			SkinElementInfo info;
			if(IsAllowDrawSideStrip(li)) {
				info = new SkinElementInfo(Skin[li.IsRecentLink ? BarSkins.SkinPopupMenuSideStrip : BarSkins.SkinPopupMenuSideStripNonRecent], r);
				info.RightToLeft = li.IsRightToLeft;
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
			}
			if(li.IsRightToLeft) {
				r.Width = r.X - rect.X;
				r.X = rect.X;
			}
			else {
			r.X = r.Right + 2;
			r.Width = rect.Right - r.Left;
			}
			r.Height = DrawParameters.Constants.SubMenuSeparatorHeight;
			info = new SkinElementInfo(Skin[BarSkins.SkinPopupMenuSeparator], r);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		public override void DrawLinkNormalBackground(BarLinkPaintArgs e, Brush brush, Rectangle r) {
			ObjectState res = e.LinkInfo.DrawDisabled ? ObjectState.Disabled : ObjectState.Normal;
			if(e.LinkInfo.Link.Item.ItemAppearance.GetAppearance(res).BackColor != Color.Empty) {
				e.LinkInfo.OwnerAppearances.GetAppearance(ObjectState.Normal).FillRectangle(e.Cache, e.LinkInfo.SelectRect);
				return;
			}
			base.DrawLinkNormalBackground(e, brush, r);
		}
		public override void DrawSeparator(GraphicsInfoArgs e, RectInfo info, BarLinkViewInfo li, object sourceInfo) {
			Rectangle r = info.Rect;
			bool vert = info.Type == RectInfoType.VertSeparator;
			if(vert) {
				r.Inflate(0, -2);
			} else {
				r.Inflate(-2, 0);
			}
			SkinElementInfo element = new SkinElementInfo(Skin[BarSkins.SkinBarSeparator], r);
			if(!vert)
				(new RotateObjectPaintHelper()).DrawRotated(e.Cache, element, SkinElementPainter.Default, RotateFlipType.Rotate90FlipX, false);
			else {
				element.RightToLeft = li.IsRightToLeft;
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, element);
			}
		}
		public override void DrawLinkCheckInMenu(BarLinkPaintArgs e, BarLinkState state, Brush backBrush, BarLinkPainter painter) {
			Rectangle r = e.LinkInfo.GlyphRect;
			SkinElementInfo element = new SkinElementInfo(Skin[BarSkins.SkinPopupMenuCheck], r);
			Size imageSize = element.Element.Glyph.GetImageBounds(0).Size;
			if(imageSize.Width > r.Width)
				r.Inflate((imageSize.Width - r.Width) / 2, (imageSize.Height - r.Height) / 2);
			r.Inflate(1, 1);
			element.Bounds = r;
			bool drawCheck = !e.LinkInfo.IsDrawPart(BarLinkParts.Glyph) || !IsExistsLinkImage(e.LinkInfo);
			element.ImageIndex = drawCheck ? 0 : 1;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, element);
			if(!drawCheck)
				painter.DrawLinkNormalGlyph(e, false);
		}
		public override void DrawButtonLinkArrowAdormentsInMenu(BarLinkPaintArgs e, BarLinkState drawState, Brush brush) {
			Rectangle ar = e.LinkInfo.Rects[BarLinkParts.OpenArrow], r;
			r = ar;
			r.Width = DrawParameters.Constants.SubMenuRealArrowWidth;
			r.Y += (ar.Height - DrawParameters.Constants.SubMenuRealArrowWidth) / 2;
			r.X += (ar.Width - DrawParameters.Constants.SubMenuRealArrowWidth) / 2;
			DrawArrow(e, CalcLinkForeBrush(e, drawState), r.Location, r.Width, e.LinkInfo.IsRightToLeft ? MarkArrowDirection.Left : MarkArrowDirection.Right);
		}
		protected override void DrawLinkBackgroundInMenuCore(BarLinkPaintArgs e) {
			ObjectState res = e.LinkInfo.DrawDisabled ? ObjectState.Disabled : ObjectState.Normal;
			if(e.LinkInfo.Link.Item.ItemInMenuAppearance.GetAppearance(res).BackColor != Color.Empty) {
				e.LinkInfo.OwnerAppearances.GetAppearance(ObjectState.Normal).FillRectangle(e.Cache, e.LinkInfo.SelectRect);
				return;
			}
			base.DrawLinkBackgroundInMenuCore(e);
		}
		protected internal virtual SkinElementInfo GetLinkHighlightedBackgroundInfo(BarLinkPaintArgs e, Rectangle r) {
			bool isMainMenu = e.LinkInfo.Bar != null && e.LinkInfo.Bar.IsMainMenu;
			bool isFloaging = e.LinkInfo.Bar != null && e.LinkInfo.Bar.IsFloating;
			bool isStatusBar = e.LinkInfo.Bar != null && e.LinkInfo.Bar.IsStatusBar;
			string skinName = BarSkins.SkinLinkSelected;
			if((e.LinkInfo.IsLinkInMenu || e.LinkInfo.DrawMode == BarLinkDrawMode.InMenuGallery) && !(e.LinkInfo.BarControl is QuickCustomizationBarControl)) {
				skinName = BarSkins.SkinPopupMenuLinkSelected;
			}
			else if(isMainMenu && !isFloaging) {
				skinName = BarSkins.SkinMainMenuLinkSelected;
			}
			else if(isStatusBar && Skin[BarSkins.SkinInStatusBarLinkSelected] != null) {
				skinName = BarSkins.SkinInStatusBarLinkSelected;
			}
			return new SkinElementInfo(Skin[skinName], r);
		}
		public override void DrawLinkCheckedInGalleryMenu(BarLinkPaintArgs e, Rectangle r, BarLinkEmptyBorder border, BarLinkState realState, Brush backBrush) {
			DrawLinkHighLightedBackground(e, r, border, realState, backBrush);
		}
		public override void DrawLinkHighLightedBackground(BarLinkPaintArgs e, Rectangle r, BarLinkEmptyBorder border, BarLinkState realState, Brush backBrush) {
			if(e.LinkInfo.Link.Item.ItemAppearance.GetAppearance(realState).BackColor != Color.Empty) {
				e.LinkInfo.OwnerAppearances.GetAppearance(realState).FillRectangle(e.Cache, r);
				return;
			}
			SkinElementInfo info = GetLinkHighlightedBackgroundInfo(e, r);
			int imageIndex = 0;
			if((e.LinkInfo.LinkState & BarLinkState.Checked) != 0) {
				imageIndex = info.Element.Image != null && info.Element.Image.ImageCount > 2 ? 2 : 1;
				if((e.LinkInfo.LinkState & BarLinkState.Highlighted) != 0)
					imageIndex = 3;
				if((e.LinkInfo.LinkState & BarLinkState.Pressed) != 0)
					imageIndex = 4;
			} else {
				if(realState == BarLinkState.Pressed) 
					imageIndex = 1;
			}
			info.ImageIndex = imageIndex;
			if(e.LinkInfo.IsDrawVerticalRotated)
				(new RotateObjectPaintHelper()).DrawRotated(e.Cache, info, SkinElementPainter.Default, RotateFlipType.Rotate90FlipX, false);
			else
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class SkinFloatingBarTitleBarEl : FloatingBarTitleBarEl {
		public SkinBarManagerPaintStyle PaintStyle { get { return Manager.PaintStyle as SkinBarManagerPaintStyle; } }
		protected Skin Skin { get { return PaintStyle.Skin; } }
		protected override ObjectElViewInfo CreateViewInfoCore() {
			return new SkinFloatingBarTitleBarElViewInfo(this);
		}
		protected override Size ButtonImageSize { 
			get {
				GraphicsInfo ginfo = new GraphicsInfo();
				ginfo.AddGraphics(null);
				SkinButtonObjectPainter btn = new SkinButtonObjectPainter(PaintStyle);
				ObjectInfoArgs info = new ObjectInfoArgs();
				Size imageSize = new Size(11, 11);
				SkinElement element = GetImageElement();
				if(element.Image != null) imageSize = element.Image.GetImages().ImageSize;
				Size res = ObjectPainter.CalcBoundsByClientRectangle(ginfo.Graphics, SkinElementPainter.Default, info, new Rectangle(Point.Empty, imageSize)).Size;
				ginfo.ReleaseGraphics();
				return res;
			} 
		}
		public SkinFloatingBarTitleBarEl(BarManager manager) : base(manager) {
			Border.Border = BarItemBorderStyle.None;
			Font = Skin[BarSkins.SkinFloatingBarTitle].GetFont(Control.DefaultFont, FontStyle.Bold);
			SelectedForeColor = ForeColor = Skin[BarSkins.SkinFloatingBarTitle].Color.GetForeColor();
			SelectedBackColor = BackColor = Skin[BarSkins.SkinFloatingBarTitle].Color.GetBackColor();
		}
		protected override void UpdateBrush() {
			CurrentBackColor = SelectedBackColor;
		}
		SkinElement GetImageElement() { return BarSkins.GetSkin(PaintStyle)[BarSkins.SkinDockWindowButtons]; }
		protected override Bitmap GetImage(int index, bool selected) {
			SkinElement element = GetImageElement();
			if(element.Image == null) return base.GetImage(index, selected);
			Bitmap res = null;
			switch(index) {
				case 0 : res = element.Image.GetImages().Images[5] as Bitmap; break;
				case 1 : res = element.Image.GetImages().Images[6] as Bitmap; break;
			}
			if(res != null) {
				res = res.Clone() as Bitmap;
				return res;
			}
			return base.GetImage(index, selected);
		}
	}
	public class BarMdiButtonLinkSkinViewInfo : BarMdiButtonLinkViewInfo {
		public BarMdiButtonLinkSkinViewInfo(BarDrawParameters parameters, BarItemLink link)
			: base(parameters, link) {
		}
		protected override BarLinkState CalcLinkState() {
			if(!Link.IsLargeImageExist) return base.CalcLinkState();
			return BarLinkState.Normal;
		}
		public override Image GetLinkImage(BarLinkState state) {
			if(!Link.IsLargeImageExist || IsLinkInMenu) return base.GetLinkImage(state);
			state = base.CalcLinkState();
			if(DrawDisabled) return Link.Item.LargeGlyphDisabled;
			if(state == BarLinkState.Pressed) return Link.Item.LargeGlyphPressed;
			if(state == BarLinkState.Highlighted) return Link.Item.LargeGlyphHot;
			return Link.Item.LargeGlyph;
		}
		public override bool UpdateLinkState() {
			if(!Link.IsLargeImageExist) return base.UpdateLinkState();
			BarLinkState newState = base.CalcLinkState();
			if(newState == LinkState) return false;
			BarControlViewInfo vi = BarControlInfo as BarControlViewInfo;
			if(vi != null)
				vi.OnLinkStateUpdated(this);
			return true;
		}
	}
}
