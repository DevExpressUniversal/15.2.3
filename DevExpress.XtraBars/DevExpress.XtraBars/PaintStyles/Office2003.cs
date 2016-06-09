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
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraBars.InternalItems;
namespace DevExpress.XtraBars.ViewInfo {
	public class BarOffice2003Constants : BarConstants {
		public BarOffice2003Constants(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		public override void Init() {
			TopDockIndent = 0;
			SubMenuScrollLinkHeight = 10;
			SubMenuScrollLinkHeightInTouch = 15;
			BarDockedRowIndent = 1;
			BarDockedRowBarIndent = 3;
			DragBorderSize = 7;
			FloatingShadowSize = 4;
			SubMenuItemBottomIndent = SubMenuItemTopIndent = 2;
			BarItemVertIndent = 2; 
			BarItemHorzIndent = 3;
			BarCaptionGlyphIndent = 3;
			BarControlFormBorderIndent = 2;
			BarControlFormHasShadow = true;
			SubMenuQGlyphGlyphIndent = 2;
			SubMenuGlyphCaptionIndent = SubMenuGlyphHorzIndent = 3;
			SubMenuArrowWidth = 21;
			SubMenuHorzRightIndent = 22;
			SubMenuCaptionIndentShortCut = 19;
			BarQuickButtonWidth = 13;
			BarButtonArrowWidth = 11;
			BarMenuButtonArrowWidth = 5;
			BarMenuButtonArrowTextIndent = 2;
			BarButtonRealArrowWidth = 5;
			SubMenuRealArrowWidth = 7;
			SubMenuSeparatorHeight = 3;
			ImageShadowHIndent = ImageShadowVIndent = 2;
			ImageHIndent = ImageVIndent = 0;
			ImageOffset = 1;
			BarSeparatorWidth = 6;
			BarSeparatorLineThickness = 1;
			BarLinksVertIndent = 1;
			BarLinksHorzIndent = 1;
			BarControlFormHasShadow = true;
			SubMenuRecentExpanderHeight = PaintStyle.Controller.GetBitmap("Office2003ExpandButton").Height + 2;
		}
		public override DevExpress.XtraEditors.Controls.BorderStyles DefaultEditBorderStyle {
			get { return DevExpress.XtraEditors.Controls.BorderStyles.Default; }
		}
		public override bool AllowLinkGlyphShadow { get { return false; } }
		public override bool AllowLinkLighting { get { return false; } }
		public override bool AllowLinkShadows { get { return true; } }
	}
	public enum BarColor2003 { DockBarBorderColor = 103,
			 DragBorderColor1 = 104, DragBorderColor2 = 105, BarSeparatorColor1 = 106, BarSeparatorColor2 = 107,
			LinkHighlightBackColor2 = 108, LinkPressedBackColor2 = 109, LinkCheckedBackColor2 = 110, 
			QuickBarCustLinkBackColor1 = 111, QuickBarCustLinkBackColor2 = 112, QuickBarCustLinkArrow1 = 113, 
			QuickBarCustLinkArrow2 = 114, SubMenuGlyphBackColor2 = 115, SubMenuNonRecentBackColor2 = 116,
			FloatingTitleBarBackColor = 117, FloatingTitleBarBorderColor = 118, FloatingTitleBarForeColor = 119
	};
	public class BarOffice2003ColorConstants : BarColorConstants {
		internal static int Office11ColorCount {
			get {
				return Office11Colors[0].Length;
			}
		}
		static int[][] Office11Colors = new int[][] {  
			  new int[] {0xddecfe, 0x81a9e2, 0x9ebef5, 0xc3daf9, 0x3b619c, 0x274176, 0xFFFFFF, 0x6a8ccb, 0xf1f9ff,
			 0x000080, 0xFFF6CF, 0xffd091, 0xfe8e4b, 0xffd28e,
				 0x000000, 0x8D8D8D,
		0x75a6f1, 0x053995, 
			  0xF6F6F6, 0x002d96, 
	   0x2a66c9, 0xc4dbf9,
		0xbedafb
													}, 
			  new int[] {0xf4f7de, 0xb7c691, 0xd9d9a7, 0xf2f0e4, 0x608058, 0x515e33, 0xFFFFFF, 0x608058, 0xf4f7de,
			 0x3f5d38, 0xFFF6CF, 0xffd091, 0xfe8e4b, 0xffd28e,
				 0x000000, 0x8D8D8D,
		0xb0c28c, 0x60776b,
			  0xF4F4F4, 0x758d5e, 
	   0x74865e, 0xe1dead,
		0xafba91
													},
			  new int[] {0xf3f4fa, 0x9997b5, 0xd7d7e5, 0xf3f3f7, 0x7c7c94, 0x545475, 0xFFFFFF, 0x6e6d8f, 0xFFFFFF,
			 0x000080, 0xFFF6CF, 0xffd091, 0xfe8e4b, 0xffd28e,
				 0x000000, 0x8D8D8D,
		0xb3b2c8, 0x767492, 
			  0xfdfaff, 0x7c7c94, 
	   0x7a7999, 0xdbdae4,
		0xe5e5eb
													},
			  };
		ImageAttributes expanderAttributes;
		public BarOffice2003ColorConstants(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected virtual int[] GetOffice11Colors(int id) {
			if(id < 0) id = 0;
			if(id > 2) id = 2;
			return Office11Colors[id];
		}
		protected virtual void InitOfficeColors(int[] office) {
			Font font = DrawParameters.ItemsFont;
			this.fMenuAppearance = new MenuAppearance();
			fAppearances[BarAppearance.Dock] = new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, Color.Empty, font));
			fAppearances[BarAppearance.Bar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, DevExpress.Utils.ColorUtils.FlatBarBackColor, font));
			fAppearances[BarAppearance.MainMenu] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, Color.Transparent, font));
			fAppearances[BarAppearance.StatusBar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			StateAppearance(BarAppearance.Bar).Normal.BackColor = FromRgb(office[0]);
			StateAppearance(BarAppearance.Bar).Normal.BackColor2 = FromRgb(office[1]);
			StateAppearance(BarAppearance.Bar).Disabled.ForeColor = SystemColors.GrayText;
			StateAppearance(BarAppearance.MainMenu).Disabled.ForeColor = SystemColors.GrayText;
			StateAppearance(BarAppearance.StatusBar).Disabled.ForeColor = SystemColors.GrayText;
			Appearance(BarAppearance.Dock).BackColor = FromRgb(office[2]);
			Appearance(BarAppearance.Dock).BackColor2 = FromRgb(office[3]);
			this.fMenuAppearance.AppearanceMenu.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font)));
			this.fMenuAppearance.AppearanceMenu.Disabled.ForeColor = SystemColors.GrayText;
			this.fMenuAppearance.SideStrip.Assign(new AppearanceObject(
				new AppearanceDefault(SystemColors.ControlText, StateAppearance(BarAppearance.Bar).Normal.BackColor, Color.Empty, StateAppearance(BarAppearance.Bar).Normal.BackColor2)));
			this.fMenuAppearance.SideStripNonRecent.Assign(new AppearanceObject(
				new AppearanceDefault(SystemColors.ControlText, GetMiddleRGB(MenuAppearance.SideStrip.BackColor, Color.Black, 92), Color.Empty, 
																GetMiddleRGB(MenuAppearance.SideStrip.BackColor2, Color.Black, 92))));
			this.fMenuAppearance.MenuBar.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font)));
			this.fMenuAppearance.MenuCaption.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, FromRgb(office[0]), FromRgb(office[19]), Color.Empty, font, HorzAlignment.Center, VertAlignment.Center)));
			this.fMenuAppearance.Menu.BackColor = FromRgb(office[18]);
			this.fMenuAppearance.Menu.BorderColor = FromRgb(office[19]);
			this.fMenuAppearance.HeaderItemAppearance.Font = new Font(font, FontStyle.Bold);
			fColors[BarColor2003.DockBarBorderColor] = FromRgb(office[4]);
			fColors[BarColor2003.DragBorderColor1] = FromRgb(office[5]);
			fColors[BarColor2003.DragBorderColor2] = FromRgb(office[6]);
			fColors[BarColor2003.BarSeparatorColor1] = FromRgb(office[7]);
			fColors[BarColor2003.BarSeparatorColor2] = FromRgb(office[8]);
			fColors[BarColor.LinkBorderColor] = fColors[BarColor.LinkHighlightBorderColor] = FromRgb(office[9]);
			fColors[BarColor.LinkHighlightBackColor] = FromRgb(office[10]);
			fColors[BarColor2003.LinkHighlightBackColor2] = FromRgb(office[11]);
			fColors[BarColor.LinkPressedBackColor] = FromRgb(office[12]);
			fColors[BarColor2003.LinkPressedBackColor2] = FromRgb(office[13]);
			fColors[BarColor.LinkCheckedBackColor] = FromRgb(office[13]);
			fColors[BarColor2003.LinkCheckedBackColor2] = FromRgb(office[12]);
			fColors[BarColor.LinkCheckedPressedBackColor] = FromRgb(office[13]);
			fColors[BarColor.LinkForeColor] = FromRgb(office[14]);
			fColors[BarColor.LinkDisabledForeColor] = FromRgb(office[15]);
			fColors[BarColor.InStatusBarLinkCheckedForeColor] = FromRgb(office[14]);
			fColors[BarColor2003.QuickBarCustLinkBackColor1] = FromRgb(office[16]);
			fColors[BarColor2003.QuickBarCustLinkBackColor2] = FromRgb(office[17]);
			fColors[BarColor2003.FloatingTitleBarBackColor] = FromRgb(office[20]);
			fColors[BarColor2003.FloatingTitleBarBorderColor] = FromRgb(office[21]);
			fColors[BarColor2003.FloatingTitleBarForeColor] = Color.White;
			fColors[BarColor2003.QuickBarCustLinkArrow1] = Color.Black;
			fColors[BarColor2003.QuickBarCustLinkArrow2] = Color.White;
			InitSubMenuExpander();
		}
		float CalcPercent(int clr) {
			clr = clr - 235;
			if(clr == 0) return 1f;
			return (float)clr / 255f;
		}
		protected virtual void InitSubMenuExpander() {
			this.expanderAttributes = new ImageAttributes();
			Color c = StateAppearance(BarAppearance.Bar).Normal.BackColor;
			float r, g, b;
			r = CalcPercent(c.R);
			g = CalcPercent(c.G);
			b = CalcPercent(c.B);
			if(r < -0.5f && g < -0.5f && b < -0.5f) {
				r = g = b = -0.3f;
			}
			ColorMatrix matrix = new ColorMatrix();
			matrix.Matrix40 = r;
			matrix.Matrix41 = g;
			matrix.Matrix42 = b;
			this.expanderAttributes.SetColorMatrix(matrix);
		}
		public ImageAttributes ExpanderAttributes { get { return expanderAttributes; } }
		protected virtual void InitStandardColors() {
			Font font = DrawParameters.ItemsFont;
			this.fMenuAppearance = new MenuAppearance();
			fAppearances[BarAppearance.Dock] = new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, Color.Empty, font));
			fAppearances[BarAppearance.Bar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, DevExpress.Utils.ColorUtils.FlatBarBackColor, font));
			fAppearances[BarAppearance.MainMenu] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, Color.Transparent, font));
			fAppearances[BarAppearance.StatusBar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			StateAppearance(BarAppearance.Bar).Normal.BackColor = GetMiddleRGB(SystemColors.Control, SystemColors.Window, 22);
			StateAppearance(BarAppearance.Bar).Normal.BackColor2 = GetMiddleRGB(SystemColors.Control, SystemColors.Window, 96);
			StateAppearance(BarAppearance.Bar).Normal.BorderColor = Color.Empty;
			Appearance(BarAppearance.Dock).BackColor = SystemColors.Control;
			Appearance(BarAppearance.Dock).BackColor2 = GetMiddleRGB(SystemColors.Control, SystemColors.Window, 20);
			this.fMenuAppearance.AppearanceMenu.Normal.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font)));
			this.fMenuAppearance.AppearanceMenu.Hovered.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font)));
			this.fMenuAppearance.AppearanceMenu.Pressed.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font)));
			this.fMenuAppearance.AppearanceMenu.Disabled.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.GrayText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font)));
			this.fMenuAppearance.SideStrip.Assign(new AppearanceObject(
				new AppearanceDefault(SystemColors.ControlText, StateAppearance(BarAppearance.Bar).Normal.BackColor, Color.Empty, StateAppearance(BarAppearance.Bar).Normal.BackColor2)));
			this.fMenuAppearance.SideStripNonRecent.Assign(new AppearanceObject(
				new AppearanceDefault(SystemColors.ControlText, GetMiddleRGB(MenuAppearance.SideStrip.BackColor, Color.Black, 92), Color.Empty, 
				GetMiddleRGB(MenuAppearance.SideStrip.BackColor2, Color.Black, 92))));
			this.fMenuAppearance.MenuBar.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font)));
			this.fMenuAppearance.Menu.BackColor = SystemColors.Window;
			this.fMenuAppearance.Menu.BorderColor = SystemColors.ControlDark;
			fColors[BarColor2003.DockBarBorderColor] = GetMiddleRGB(SystemColors.Control, SystemColors.Window, 85);
			fColors[BarColor2003.DragBorderColor1] = GetMiddleRGB(SystemColors.ControlDark, SystemColors.Window, 76);
			fColors[BarColor2003.DragBorderColor2] = SystemColors.Window;
			fColors[BarColor2003.BarSeparatorColor1] = GetMiddleRGB(SystemColors.ControlDark, SystemColors.Window, 70);
			fColors[BarColor2003.BarSeparatorColor2] = Color.White;
			fColors[BarColor.LinkBorderColor] = fColors[BarColor.LinkHighlightBorderColor] = SystemColors.Highlight;
			fColors[BarColor2003.LinkHighlightBackColor2] = fColors[BarColor.LinkHighlightBackColor] = GetRealColor(GetLightColor(-2, 30, 72));
			fColors[BarColor.LinkPressedBackColor] = GetRealColor(GetLightColor(14, 44, 40));
			fColors[BarColor2003.LinkPressedBackColor2] = GetRealColor(GetLightColor(14, 44, 40));
			fColors[BarColor.LinkCheckedBackColor] = GetRealColor(GetLightColor(14, 44, 40));
			fColors[BarColor2003.LinkCheckedBackColor2] = GetRealColor(GetLightColor(14, 44, 40));
			fColors[BarColor.LinkCheckedPressedBackColor] = GetRealColor(GetLightColor(14, 44, 40));
			fColors[BarColor.LinkForeColor] = SystemColors.ControlText;
			fColors[BarColor.LinkDisabledForeColor] = SystemColors.GrayText;
			fColors[BarColor.InStatusBarLinkCheckedForeColor] = SystemColors.ControlText;
			fColors[BarColor2003.QuickBarCustLinkBackColor1] = GetMiddleRGB(SystemColors.ControlDark, SystemColors.Window, 74);
			fColors[BarColor2003.QuickBarCustLinkBackColor2] = SystemColors.ControlDark;
			fColors[BarColor2003.FloatingTitleBarBackColor] = SystemColors.ControlDark;
			fColors[BarColor2003.FloatingTitleBarBorderColor] = StateAppearance(BarAppearance.Bar).Normal.BackColor2;
			fColors[BarColor2003.FloatingTitleBarForeColor] = Color.White;
			fColors[BarColor2003.QuickBarCustLinkArrow1] = Color.Black;
			fColors[BarColor2003.QuickBarCustLinkArrow2] = Color.White;
			InitSubMenuExpander();
		}
		public Color GetRealColor(Color clr) {
			return DevExpress.Utils.ColorUtils.GetRealColor(clr);
		}
		public Color GetLightColor(int btnFaceColorPart, int highlightColorPart, int windowColorPart) {
			return DevExpress.Utils.ColorUtils.GetLightColor(btnFaceColorPart, highlightColorPart, windowColorPart);
		}
		protected override void InitColors() {
			InitThemeColors();
			fColors[BarColor.SubMenuSeparatorColor] = Colors(BarColor2003.BarSeparatorColor1);
			fColors[BarColor.SubMenuEmptyBorderColor] = MenuAppearance.Menu.BackColor;
			fColors[BarColor.DesignTimeSelectColor] = Color.LightPink;
			fColors[BarColor.TabBackColor] = DevExpress.Utils.ColorUtils.FlatTabBackColor;
		}
		protected virtual void InitThemeColors() {
			XPThemeType themeType = DevExpress.Utils.WXPaint.WXPPainter.Default.GetXPThemeType();
			if(themeType != XPThemeType.Unknown) {
				int id = ((int)themeType) - 1;
				InitOfficeColors(GetOffice11Colors(id));
			} else {
				InitStandardColors();
			}
		}
		protected Color FromRgb(int rgb) { return Color.FromArgb((int)(rgb + (uint)0xff000000)); }
		int CalcValue(int v1, int v2, int percent) {
			int i;
			i = (v1 * percent) / 100 + (v2 * (100 - percent)) / 100;
			if(i > 255) i = 255;
			return i;
		}
		protected Color GetMiddleRGB(Color clr1, Color clr2, int percent) {
			Color r = Color.FromArgb(
				CalcValue(clr1.R, clr2.R, percent),CalcValue(clr1.G, clr2.G, percent),CalcValue(clr1.B, clr2.B, percent));
			return r;
		}
	}
	public class BarOffice2003DrawParameters : BarDrawParameters {
		public BarOffice2003DrawParameters(BarManagerPaintStyle paintStyle) : base(paintStyle) { 
		}
		protected override BarConstants CreateConstants() { return new BarOffice2003Constants(PaintStyle); } 
		protected override BarColorConstants CreateColors() { return new BarOffice2003ColorConstants(PaintStyle); } 
	}
}
