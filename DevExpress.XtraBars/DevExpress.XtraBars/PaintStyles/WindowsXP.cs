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
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.InternalItems;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.ViewInfo {
	public class BarWindowsXPConstants : BarConstants {
		public int DragBorderRealWidth;
		Bitmap mdiGlyphs, buffer;
		public Size MdiButtonSize;
		public BarWindowsXPConstants(BarManagerPaintStyle paintStyle)
			: base(paintStyle) {
			this.MdiButtonSize = Size.Empty;
			this.mdiGlyphs = null;
			this.buffer = null;
		}
		public virtual Bitmap Buffer {
			get {
				UpdateBufferBitmap();
				return buffer;
			}
			set {
				if(buffer == value) return;
				if(buffer != null) buffer.Dispose();
				buffer = null;
			}
		}
		public virtual void UpdateBufferBitmap() {
			if(MdiGlyphs == null) {
				Buffer = null;
				return;
			}
			if(buffer == null || buffer.Width != MdiButtonSize.Width || buffer.Height != MdiButtonSize.Height) {
				buffer = new Bitmap(MdiButtonSize.Width, MdiButtonSize.Height);
			}
		}
		public Bitmap MdiGlyphs {
			get { return mdiGlyphs; }
			set {
				if(MdiGlyphs == value) return;
				if(mdiGlyphs != null) mdiGlyphs.Dispose();
				mdiGlyphs = value;
			}
		}
		public override void Init() {
			SubMenuScrollLinkHeight = 10;
			SubMenuScrollLinkHeightInTouch = 15;
			MdiButtonSize = Size.Empty;
			TopDockIndent = 2;
			BarDockedRowIndent = 2;
			BarDockedRowBarIndent = 2;
			FloatingShadowSize = 4;
			SubMenuItemBottomIndent = SubMenuItemTopIndent = 2;
			BarItemVertIndent = 2;
			BarItemHorzIndent = 3;
			SubMenuQGlyphGlyphIndent = 2;
			BarCaptionGlyphIndent = 3;
			BarControlFormBorderIndent = 2;
			BarControlFormHasShadow = true;
			SubMenuGlyphHorzIndent = 5;
			SubMenuGlyphCaptionIndent = 0;
			SubMenuArrowWidth = 21;
			SubMenuHorzRightIndent = 22;
			SubMenuCaptionIndentShortCut = 19;
			BarMenuButtonArrowWidth = 5;
			BarQuickButtonWidth = BarButtonArrowWidth = 11;
			BarMenuButtonArrowTextIndent = 2;
			BarButtonRealArrowWidth = 5;
			SubMenuRealArrowWidth = 7;
			SubMenuSeparatorHeight = 6;
			ImageOffset = 1;
			BarSeparatorWidth = 6;
			BarSeparatorLineThickness = 2;
			BarControlFormHasShadow = true;
			SubMenuRecentExpanderHeight = BarButtonRealArrowWidth * 2 + 2;
			ImageShadowHIndent = ImageShadowVIndent = 2;
			NativeControlAdvPaintArgs args = PrimitivesPainterWindowsXP.CreateArgsCore(null, "toolbar", PrimitivesPainterWindowsXP.XP_TOOLBAR.SEPARATORVERT, 0);
			BarSeparatorWidth = Painter.CalcSize(args, Painter.XP_THEME_SIZE.TS_TRUE).Width + 1;
			if(BarSeparatorWidth < 5) BarSeparatorWidth = 5;
			BarSeparatorLineThickness = BarSeparatorWidth / 2;
			Rectangle r = Painter.CalcThemeMargins(PrimitivesPainterWindowsXP.CreateArgsCore(null, "toolbar", 0, 0), Painter.XP_TMT_CONTENTMARGINS);
			BarLinksVertIndent = r.Right / 2;
			BarLinksHorzIndent = r.Bottom / 2;
			if(BarLinksHorzIndent == 0) BarLinksHorzIndent = 1;
			if(BarLinksVertIndent == 0) BarLinksVertIndent = 1;
			args = PrimitivesPainterWindowsXP.CreateArgsCore(null, "rebar", PrimitivesPainterWindowsXP.XP_REBAR.GRIPPER, 0);
			Size size = Painter.CalcSize(args, Painter.XP_THEME_SIZE.TS_TRUE);
			if(size.Width == 0) size.Width = 3;
			DragBorderRealWidth = size.Width;
			DragBorderSize = DragBorderRealWidth * 2;
			ImageHIndent = ImageVIndent = 0;
			args = PrimitivesPainterWindowsXP.CreateArgsCore(null, "toolbar", PrimitivesPainterWindowsXP.XP_TOOLBAR.BUTTON, PrimitivesPainterWindowsXP.XPState_TOOLBAR.HOT);
			args.Bounds = new Rectangle(0, 0, 16, 16);
			Painter.CalcBounds(args);
			ImageHIndent = args.Bounds.Width - 16;
			ImageVIndent = args.Bounds.Height - 16;
			args = PrimitivesPainterWindowsXP.CreateArgsCore(null, "toolbar", PrimitivesPainterWindowsXP.XP_TOOLBAR.SPLITBUTTONDROPDOWN, 0);
			BarButtonArrowWidth = Painter.CalcSize(args, Painter.XP_THEME_SIZE.TS_TRUE).Width + 3;
			InitMdiGlyphs();
		}
		string[] names = new string[] { "MinImage", "RestoreImage", "CloseImage" };
		BarLinkState[] states = new BarLinkState[] { BarLinkState.Normal, BarLinkState.Highlighted, BarLinkState.Pressed, BarLinkState.Disabled };
		object[] mdiButtons = new object[] { PrimitivesPainterWindowsXP.XP_WINDOW.MDIMINBUTTON, PrimitivesPainterWindowsXP.XP_WINDOW.MDIRESTOREBUTTON, PrimitivesPainterWindowsXP.XP_WINDOW.MDICLOSEBUTTON };
		object[] mdiButtonStates = new object[] { PrimitivesPainterWindowsXP.XPState_Button.NORMAL, PrimitivesPainterWindowsXP.XPState_Button.HOT,
													PrimitivesPainterWindowsXP.XPState_Button.PUSHED, PrimitivesPainterWindowsXP.XPState_Button.DISABLED};
		public override void Dispose() {
			MdiGlyphs = null;
			Buffer = null;
			base.Dispose();
		}
		protected virtual void InitMdiGlyphs() {
			MdiGlyphs = null;
			Buffer = null;
			NativeControlAdvPaintArgs args = PrimitivesPainterWindowsXP.CreateArgsCore(null, "window", PrimitivesPainterWindowsXP.XP_WINDOW.MDICLOSEBUTTON, 0);
			MdiButtonSize = Painter.CalcSize(args, Painter.XP_THEME_SIZE.TS_TRUE);
			if(MdiButtonSize.Width < 4 || MdiButtonSize.Height < 4) {
				MdiGlyphs = null;
				return;
			}
			if(MdiButtonSize.Width < 16) {
				MdiButtonSize.Width = 16;
				MdiButtonSize.Height = 16;
			}
			MdiGlyphs = new Bitmap(MdiButtonSize.Width * mdiButtonStates.Length * mdiButtons.Length, MdiButtonSize.Height);
			Graphics g = Graphics.FromImage(MdiGlyphs);
			g.FillRectangle(Brushes.Magenta, new Rectangle(Point.Empty, MdiGlyphs.Size));
			g.Dispose();
			MdiGlyphs.MakeTransparent();
			g = Graphics.FromImage(MdiGlyphs);
			Rectangle r = new Rectangle(Point.Empty, MdiButtonSize);
			args = PrimitivesPainterWindowsXP.CreateArgsCore(g, "window", PrimitivesPainterWindowsXP.XP_WINDOW.MDICLOSEBUTTON, 0);
			for(int n = 0; n < mdiButtons.Length; n++) {
				args.Part = (int)mdiButtons[n];
				for(int s = 0; s < mdiButtonStates.Length; s++) {
					args.PartState = (int)mdiButtonStates[s];
					args.Bounds = r;
					Painter.Draw(args);
					r.X += r.Width;
				}
			}
			g.Dispose();
		}
		public Bitmap GetBitmap(string name, BarLinkState state) {
			if(Buffer == null) {
				if(MdiButtonSize.IsEmpty) return new Bitmap(1, 1);
				return new Bitmap(MdiButtonSize.Width, MdiButtonSize.Height);
			}
			Graphics g = Graphics.FromImage(Buffer);
			g.FillRectangle(Brushes.Magenta, new Rectangle(Point.Empty, MdiButtonSize));
			g.Dispose();
			Buffer.MakeTransparent();
			g = Graphics.FromImage(Buffer);
			int index = Array.IndexOf(names, name) * mdiButtonStates.Length + Array.IndexOf(states, state);
			Rectangle from = new Rectangle(index * MdiButtonSize.Width, 0, MdiButtonSize.Width, MdiButtonSize.Height);
			PaintHelper.DrawImageCore(g, MdiGlyphs, 0, 0, from.Width, from.Height, from, null);
			g.Dispose();
			return (Bitmap)Buffer.Clone();
		}
		public override DevExpress.XtraEditors.Controls.BorderStyles DefaultEditBorderStyle {
			get { return DevExpress.XtraEditors.Controls.BorderStyles.Default; }
		}
		public override bool AllowLinkGlyphShadow { get { return false; } }
		public override bool AllowLinkLighting { get { return true; } }
		public override bool AllowLinkShadows { get { return false; } }
	}
	public enum BarXPColor { ToolbarEdgeShadow = 100, ToolbarEdgeHighlight = 101, MainMenuContainerLinkBackColor = 102, MainMenuContainerLinkForeColor = 103 }
	public class BarWindowsXPColorConstants : BarColorConstants {
		public BarWindowsXPColorConstants(BarManagerPaintStyle paintStyle)
			: base(paintStyle) {
		}
		protected virtual Color GetXPColor(string themeName, int propId) { return GetXPColor(themeName, 0, 0, propId); }
		protected virtual Color GetXPColor(string themeName, int part, int state, int propId) {
			NativeControlAdvPaintArgs args = PrimitivesPainterWindowsXP.CreateArgsCore(null, themeName, part, state);
			return Painter.GetThemeColor(args, propId);
		}
		protected override void InitAppearance() {
			Font font = DrawParameters.ItemsFont;
			this.fMenuAppearance = new MenuAppearance();
			this.fMenuAppearance.AppearanceMenu.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.MenuText, SystemColors.Menu, SystemColors.Control, font)));
			this.fMenuAppearance.SideStrip.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, MenuAppearance.Menu.BackColor)));
			this.fMenuAppearance.SideStripNonRecent.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, ControlPaint.LightLight(SystemColors.Highlight))));
			this.fMenuAppearance.MenuBar.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, font)));
			this.fMenuAppearance.MenuCaption.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDark, Color.Empty, font, HorzAlignment.Center, VertAlignment.Center)));
			this.fMenuAppearance.HeaderItemAppearance.Font = new Font(font, FontStyle.Bold);
			MenuItemCaptionWithDescription.Assign(MenuAppearance.Menu);
			MenuItemCaptionWithDescription.SetFont(new Font(MenuItemCaptionWithDescription.Normal.Font, FontStyle.Bold));
			MenuItemDescription.Assign(MenuAppearance.Menu);
			MenuItemDescription.SetWordWrap(WordWrap.Wrap);
			MenuItemDescription.SetVAlignment(VertAlignment.Top);
			fAppearances[BarAppearance.Dock] = new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			fAppearances[BarAppearance.Bar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, Color.Transparent, font));
			fAppearances[BarAppearance.BarNoBorder] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, Color.Transparent, font));
			fAppearances[BarAppearance.MainMenu] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, Color.Transparent, font));
			fAppearances[BarAppearance.StatusBar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
		}
		protected override void InitColors() {
			fColors[BarColor.DesignTimeSelectColor] = ColorUtils.FlatBarItemHighLightBackColor;
			fColors[BarColor.SubMenuSeparatorColor] = SystemColors.ControlDark;
			fColors[BarColor.LinkHighlightBackColor] = StateAppearance(BarAppearance.Bar).Normal.BackColor;
			fColors[BarColor.LinkHighlightBorderColor] = StateAppearance(BarAppearance.Bar).Normal.BackColor;
			fColors[BarColor.LinkPressedBackColor] = StateAppearance(BarAppearance.Bar).Normal.BackColor;
			fColors[BarColor.LinkForeColor] = SystemColors.ControlText;
			fColors[BarColor.LinkDisabledForeColor] = SystemColors.GrayText;
			fColors[BarColor.InStatusBarLinkCheckedForeColor] = SystemColors.ControlText;
			fColors[BarColor.LinkCheckedBackColor] = StateAppearance(BarAppearance.Bar).Normal.BackColor;
			fColors[BarColor.LinkCheckedPressedBackColor] = StateAppearance(BarAppearance.Bar).Normal.BackColor;
			fColors[BarColor.TabBackColor] = DevExpress.Utils.ColorUtils.FlatTabBackColor;
			fColors[BarColor.FloatingBarBorderColor] = SystemColors.Control;
			fColors[BarColor.LinkBorderColor] = SystemColors.Control;
			fColors[BarColor.SubMenuEmptyBorderColor] = StateAppearance(BarAppearance.Bar).Normal.BackColor;
			fColors[BarXPColor.ToolbarEdgeShadow] = GetXPColor("toolbar", Painter.XP_TMT_EDGESHADOWCOLOR);
			fColors[BarXPColor.ToolbarEdgeHighlight] = GetXPColor("toolbar", Painter.XP_TMT_EDGEHIGHLIGHTCOLOR);
			fColors[BarXPColor.MainMenuContainerLinkBackColor] = SystemColors.Highlight;
			fColors[BarXPColor.MainMenuContainerLinkForeColor] = SystemColors.HighlightText;
		}
	}
	public class BarWindowsXPDrawParameters : BarDrawParameters {
		public BarWindowsXPDrawParameters(BarManagerPaintStyle paintStyle)
			: base(paintStyle) { 
		}
		public new virtual BarWindowsXPConstants Constants { get { return base.Constants as BarWindowsXPConstants; } }
		protected override BarConstants CreateConstants() { return new BarWindowsXPConstants(PaintStyle); }
		protected override BarColorConstants CreateColors() { return new BarWindowsXPColorConstants(PaintStyle); }
		public override void UpdateMdiGlyphs(BarMdiButtonItem item, string name) {
			if(Constants.MdiGlyphs == null) {
				base.UpdateMdiGlyphs(item, name);
				return;
			}
			item.LargeGlyph = Constants.GetBitmap(name, BarLinkState.Normal);
			item.LargeGlyphPressed = Constants.GetBitmap(name, BarLinkState.Pressed);
			item.LargeGlyphDisabled = Constants.GetBitmap(name, BarLinkState.Disabled);
			item.LargeGlyphHot = Constants.GetBitmap(name, BarLinkState.Highlighted);
		}
	}
}
