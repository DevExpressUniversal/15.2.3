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
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using DevExpress.Skins;
namespace DevExpress.XtraBars.ViewInfo {
	public class BarConstants : IDisposable {
		public int FloatingShadowSize, BarItemVertIndent, BarItemHorzIndent, BarCaptionGlyphIndent,
			BarControlFormBorderIndent, ImageShadowHIndent, ImageShadowVIndent, ImageOffset, BarSeparatorWidth, BarSeparatorLineThickness, DragBorderSize;
		public int SubMenuGlyphCaptionIndent, SubMenuGlyphHorzIndent, SubMenuArrowWidth, SubMenuRealArrowWidth,  BarQuickButtonWidth, BarButtonArrowWidth,
			BarMenuButtonArrowWidth, BarMenuButtonArrowTextIndent, BarButtonRealArrowWidth, 
			SubMenuQGlyphGlyphIndent, SubMenuCaptionIndentShortCut, SubMenuHorzRightIndent, TopDockIndent, BarDockedRowIndent, BarDockedRowBarIndent, BarLinksVertIndent, BarLinksHorzIndent, ImageHIndent, ImageVIndent,
			SubMenuItemBottomIndent, SubMenuItemTopIndent, SubMenuScrollLinkHeight, SubMenuScrollLinkHeightInTouch, SubMenuRecentExpanderHeight;
		[Obsolete("Use SubMenuSeparatorHeight"), System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public int SubMenuSepartorHeight;
		public int SubMenuSeparatorHeight;
		public bool BarControlFormHasShadow;
		BarManagerPaintStyle paintStyle;
		public BarConstants(BarManagerPaintStyle paintStyle) {
			this.paintStyle = paintStyle;
		}
		public BarManagerPaintStyle PaintStyle { get { return paintStyle; } }
		public virtual void Dispose() {
		}
		public virtual bool AllowLinkGlyphShadow { get { return true; } }
		public virtual bool AllowLinkLighting { get { return true; } }
		public virtual bool AllowLinkShadows { get { return true; } }
		public virtual DevExpress.XtraEditors.Controls.BorderStyles DefaultEditBorderStyle {
			get { return DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat; }
		}
		public virtual int GetBarButtonArrowWidth() { return BarButtonArrowWidth; }
		public virtual int GetBarMenuButtonArrowWidth() { return BarMenuButtonArrowWidth; }
		public virtual int GetSubMenuArrowWidth() { return SubMenuArrowWidth; }
		public virtual void Init() {
			TopDockIndent = 0;
			SubMenuScrollLinkHeight = 10;
			SubMenuScrollLinkHeightInTouch = 15;
			BarDockedRowIndent = 2;
			BarDockedRowBarIndent = 2;
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
			BarQuickButtonWidth = BarButtonArrowWidth = 11;
			BarMenuButtonArrowWidth = 5;
			BarMenuButtonArrowTextIndent = 2;
			BarButtonRealArrowWidth = 5;
			SubMenuRealArrowWidth = 7;
			SubMenuSeparatorHeight = 3;
			ImageShadowHIndent = ImageShadowVIndent = 2;
			ImageHIndent = ImageVIndent = 0;
			ImageOffset = 1;
			BarSeparatorWidth = 5;
			BarSeparatorLineThickness = 1;
			BarLinksVertIndent = 1;
			BarLinksHorzIndent = 1;
			BarControlFormHasShadow = true;
			SubMenuRecentExpanderHeight = BarButtonRealArrowWidth * 2 + 2;
		}
		public int GetSubMenuScrollLinkHeight() {
			if(PaintStyle.Controller.LookAndFeel.GetTouchUI())
				return (int)(SubMenuScrollLinkHeightInTouch * PaintStyle.Controller.LookAndFeel.GetTouchScaleFactor());
			return SubMenuScrollLinkHeight;
		}
	}
	public enum BarColor { FloatingBarBorderColor, LinkBorderColor,
		SubMenuSeparatorColor, 
		LinkHighlightBackColor, LinkHighlightBorderColor, LinkPressedBackColor,
		LinkForeColor, LinkDisabledForeColor, LinkCheckedBackColor, LinkCheckedPressedBackColor,
		TabBackColor, SubMenuEmptyBorderColor, DesignTimeSelectColor, InStatusBarLinkCheckedForeColor
	}
	public enum BarAppearance { Bar, BarHovered, BarPressed, BarDisabled, MainMenu, MainMenuHovered, MainMenuPressed, MainMenuDisabled, StatusBar, StatusBarHovered, StatusBarPressed, StatusBarDisabled, Dock, BarNoBorder, BarNoBorderHovered, BarNoBorderPressed, BarNoBorderDisabled };
	public class BarColorConstants : IDisposable {
		protected Hashtable fPens, fBrushes, fColors, fAppearances;
		protected MenuAppearance fMenuAppearance;
		BarManagerPaintStyle paintStyle;
		StateAppearances menuItemCaptionWithDescriptionAppearance;
		StateAppearances menuItemDescriptionAppearance;
		public BarColorConstants(BarManagerPaintStyle paintStyle) {
			this.menuItemCaptionWithDescriptionAppearance = new StateAppearances();
			this.menuItemDescriptionAppearance = new StateAppearances();
			this.fMenuAppearance = null;
			this.fAppearances = new Hashtable();
			this.fPens = new Hashtable();
			this.fBrushes = new Hashtable();
			this.fColors = new Hashtable();
			this.paintStyle = paintStyle;
		}
		public StateAppearances MenuItemCaptionWithDescription { get { return menuItemCaptionWithDescriptionAppearance; } }
		public StateAppearances MenuItemDescription { get { return menuItemDescriptionAppearance; } }
		public BarDrawParameters DrawParameters { get { return PaintStyle.DrawParameters; } }
		public MenuAppearance MenuAppearance { get { return fMenuAppearance; } }
		public BarManagerPaintStyle PaintStyle { get { return paintStyle; } }
		public BarAndDockingController Controller { get { return PaintStyle.Controller; } }
		public virtual void Dispose() {
			DestroyGdi();
		}
		public virtual AppearanceObject Appearance(object obj) { 
			return fAppearances[obj] as AppearanceObject; 
		}
		public virtual StateAppearances StateAppearance(object obj) {
			return fAppearances[obj] as StateAppearances;
		}
		public virtual Color this[object color] { get { return Colors(color); } }
		public virtual Pen Pens(object color) { return fPens[color] as Pen; }
		public virtual Brush Brushes(object color) { return fBrushes[color] as Brush; }
		public virtual Color Colors(object color) { 
			if(fColors.Contains(color)) return (Color)fColors[color]; 
			return Color.Empty;
		}
		public virtual void Init() {
			DestroyGdi();
			InitAppearance();
			InitColors();
			CreateBrushes();
			CreatePens();
		}
		protected virtual void InitAppearance() {
			Font font = DrawParameters.ItemsFont;
			this.fMenuAppearance = new MenuAppearance();
			this.fMenuAppearance.AppearanceMenu.Normal.Assign(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font));
			this.fMenuAppearance.AppearanceMenu.Pressed.Assign(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font));
			this.fMenuAppearance.AppearanceMenu.Hovered.Assign(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font));
			this.fMenuAppearance.AppearanceMenu.Disabled.Assign(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font));
			this.fMenuAppearance.SideStrip.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)));
			this.fMenuAppearance.SideStripNonRecent.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, GetNonRecentSubMenuBackColor())));
			this.fMenuAppearance.MenuBar.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Window, font)));
			this.fMenuAppearance.MenuCaption.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDark, Color.Empty, font, HorzAlignment.Center, VertAlignment.Center)));
			this.fMenuAppearance.HeaderItemAppearance.Font = new Font(font, FontStyle.Bold);
			this.fMenuAppearance.HeaderItemAppearance.Options.UseFont = true;
			MenuItemCaptionWithDescription.Assign(MenuAppearance.Menu);
			FontStyle fontStyle = FontStyle.Bold;
			if(font.Italic)
				fontStyle |= FontStyle.Italic;
			MenuItemCaptionWithDescription.SetFont(new Font(MenuItemCaptionWithDescription.Normal.Font, fontStyle));
			MenuItemDescription.Assign(MenuAppearance.Menu);
			MenuItemDescription.SetWordWrap(WordWrap.Wrap);
			MenuItemDescription.SetVAlignment(VertAlignment.Top);
			fAppearances[BarAppearance.Dock] = new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			fAppearances[BarAppearance.Bar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, DevExpress.Utils.ColorUtils.FlatBarBackColor, font));
			fAppearances[BarAppearance.BarNoBorder] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, DevExpress.Utils.ColorUtils.FlatBarBackColor, font));
			fAppearances[BarAppearance.MainMenu] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			fAppearances[BarAppearance.StatusBar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
		}
		protected virtual void InitColors() {
			fColors[BarColor.DesignTimeSelectColor] = ColorUtils.FlatBarItemHighLightBackColor;
			fColors[BarColor.SubMenuSeparatorColor] = SystemColors.ControlDark;
			fColors[BarColor.LinkHighlightBackColor] = DevExpress.Utils.ColorUtils.FlatBarItemHighLightBackColor;
			fColors[BarColor.LinkHighlightBorderColor] = SystemColors.Highlight;
			fColors[BarColor.LinkPressedBackColor] = DevExpress.Utils.ColorUtils.FlatBarItemPressedBackColor;
			fColors[BarColor.LinkForeColor] = SystemColors.ControlText;
			fColors[BarColor.LinkDisabledForeColor] = SystemColors.GrayText;
			fColors[BarColor.InStatusBarLinkCheckedForeColor] = SystemColors.ControlText;
			fColors[BarColor.LinkCheckedBackColor] = DevExpress.Utils.ColorUtils.FlatBarItemDownedColor;
			fColors[BarColor.LinkCheckedPressedBackColor] = DevExpress.Utils.ColorUtils.FlatBarItemPressedBackColor;
			fColors[BarColor.TabBackColor] = DevExpress.Utils.ColorUtils.FlatTabBackColor;
			fColors[BarColor.FloatingBarBorderColor] = SystemColors.ControlDarkDark;
			fColors[BarColor.LinkBorderColor] = DevExpress.Utils.ColorUtils.FlatBarBorderColor;
			fColors[BarColor.SubMenuEmptyBorderColor] = StateAppearance(BarAppearance.Bar).Normal.BackColor;
		}
		protected virtual void CreateBrushes() {
			foreach(object clr in fColors.Keys) {
				fBrushes[clr] = CreateBrush(clr);
			}
		}
		protected virtual Brush CreateBrush(object clr) {
			return new SolidBrush(Colors(clr));
		}
		protected virtual Pen CreatePen(object clr) {
			return new Pen(Colors(clr));
		}
		protected virtual void CreatePens() {
			foreach(object clr in fColors.Keys) {
				fPens[clr] = CreatePen(clr);
			}
		}
		protected virtual void DestroyGdi() {
			DestroyBrushes();
			DestroyPens();
			DestroyAppearances();
		}
		protected virtual void DestroyAppearances() {
			if(this.fMenuAppearance != null) {
				this.fMenuAppearance.Dispose();
				this.fMenuAppearance = null;
			}
			foreach(IDisposable dis in this.fAppearances.Values) {
				dis.Dispose();
			}
			fAppearances.Clear();
		}
		protected virtual void DestroyBrushes() {
			foreach(Brush brush in fBrushes.Values) {
				brush.Dispose();
			}
			fBrushes.Clear();
		}
		protected virtual void DestroyPens() {
			foreach(Pen pen in fPens.Values) {
				pen.Dispose();
			}
			fPens.Clear();
		}
		protected virtual Color GetNonRecentSubMenuBackColor() {
			Color res = SystemColors.Control;
			const int delta = -20;
			int R = Math.Abs(res.R + delta);
			int G = Math.Abs(res.G + delta);
			int B = Math.Abs(res.B + delta);
			if(R > 255) R = 255;
			if(G > 255) G = 255;
			if(B > 255) B = 255;
			res = Color.FromArgb(R, G, B);
			return res;
		}
	}
	public class BarOfficeXPDrawParameters : BarDrawParameters {
		public BarOfficeXPDrawParameters(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
	}
	public abstract class BarDrawParameters : IDisposable {
		BarManagerPaintStyle paintStyle;
		public Font  FloatingCaptionFont;
		public StringFormat SingleLineStringFormat, SingleLineVerticalStringFormat, ShortCutStringFormat, 
			SingleLineEllipsisFormat, SingleLineEllipsisPathFormat;
		protected float fBarImageScaleFactor;
		protected float fRibbonImageScaleFactor;
		BarConstants constants;
		BarColorConstants fColors;
		public BarDrawParameters(BarManagerPaintStyle paintStyle) {
			this.paintStyle = paintStyle;
			InitGDIResources(false);
		}
		public BarAndDockingController Controller { get { return PaintStyle.Controller; } }
		public BarManagerPaintStyle PaintStyle { get { return paintStyle; } }
		public bool ScaleImages { get { return BarManagerImageScaleFactor != 1f; } }
		public bool RibbonScaleImages { get { return RibbonImageScaleFactor != 1f; } }
		public float BarManagerImageScaleFactor { get { return fBarImageScaleFactor; } }
		public float RibbonImageScaleFactor { get { return fRibbonImageScaleFactor; } }
		public virtual Font ItemsFont { get { return Controller.AppearancesBar.ItemsFont; } }
		public virtual Size GetCheckSize() {
			return SystemInformation.MenuCheckSize;
		}
		public virtual BarColorConstants Colors {
			get {
				if(fColors == null) {
					fColors = CreateColors();
					fColors.Init();
				}
				return fColors;
			}
		}
		public virtual BarConstants Constants {
			get {
				if(constants == null) {
					constants = CreateConstants();
					constants.Init();
				}
				return constants;
			}
		}
		public AppearanceObject Appearance(object appearance) { return Colors.Appearance(appearance); }
		public StateAppearances StateAppearance(object appearance) { return Colors.StateAppearance(appearance); }
		public Brush GetBackBrush(object appearance, GraphicsCache cache) {
			return GetBackBrush(appearance, BarLinkState.Normal, cache);
		}
		public Brush GetBackBrush(object appearance, BarLinkState state, GraphicsCache cache) {
			if(StateAppearance(appearance) != null)
				return StateAppearance(appearance).GetAppearance(state).GetBackBrush(cache);
			return Appearance(appearance).GetBackBrush(cache); 
		}
		public Brush GetBackBrush(object appearance, BarLinkState state, GraphicsCache cache, Rectangle bounds) {
			if(StateAppearance(appearance) != null)
				return StateAppearance(appearance).GetAppearance(state).GetBackBrush(cache, bounds);
			return Appearance(appearance).GetBackBrush(cache, bounds); 
		}
		public virtual Rectangle CalcLinkSourceRectangle(Rectangle source) { return source; }
		public virtual Color GetLinkForeColor(BarLinkViewInfo linkInfo, Color foreColor, BarLinkState state) { return foreColor; }
		public virtual Color GetLinkForeColorDisabled(BarLinkViewInfo linkInfo, BarLinkState state) {
			return Colors[BarColor.LinkDisabledForeColor];
		}
		public virtual Color GetInStatusBarLinkForeColorChecked(BarLinkViewInfo linkInfo, BarLinkState state) {
			return Colors[BarColor.InStatusBarLinkCheckedForeColor];
		}
		protected virtual BarConstants CreateConstants() { return new BarConstants(PaintStyle); } 
		protected virtual BarColorConstants CreateColors() { return new BarColorConstants(PaintStyle); } 
		protected virtual void InitGDIResources(bool designTime) {
			FloatingCaptionFont = AppearanceObject.DefaultMenuFont;
			ShortCutStringFormat = new StringFormat(StringFormatFlags.NoWrap);
			ShortCutStringFormat.LineAlignment = StringAlignment.Center;
			ShortCutStringFormat.Alignment = StringAlignment.Far;
			SingleLineStringFormat = TextOptions.DefaultStringFormat.Clone() as StringFormat;
			SingleLineStringFormat.FormatFlags |= StringFormatFlags.NoWrap;
			SingleLineStringFormat.HotkeyPrefix = designTime? System.Drawing.Text.HotkeyPrefix.Show: System.Drawing.Text.HotkeyPrefix.Hide;
			SingleLineStringFormat.LineAlignment = StringAlignment.Center;
			SingleLineEllipsisFormat = (StringFormat)SingleLineStringFormat.Clone();
			SingleLineEllipsisFormat.Trimming = StringTrimming.EllipsisCharacter;
			SingleLineEllipsisPathFormat = (StringFormat)SingleLineStringFormat.Clone();
			SingleLineEllipsisPathFormat.Trimming = StringTrimming.EllipsisPath;
			SingleLineVerticalStringFormat = (StringFormat)SingleLineStringFormat.Clone();
			SingleLineVerticalStringFormat.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.NoWrap;
			this.fBarImageScaleFactor = 1f;
			this.fRibbonImageScaleFactor = 1f;
			float scaleFactor = GetScaleFactor();
			if(Controller.PropertiesBar.ScaleIcons)
				this.fBarImageScaleFactor = scaleFactor;
			if(Controller.PropertiesRibbon.ScaleIcons)
				this.fRibbonImageScaleFactor = scaleFactor;
		}
		protected virtual float GetScaleFactor() {
				Graphics g = Graphics.FromHwnd(IntPtr.Zero);
				float dpi = g.DpiX;
				g.Dispose();
			return Math.Abs(((dpi - 96f) / 96f) + 1f);
		}
		void IDisposable.Dispose() {
			Colors.Dispose();
			DestroyFormats();
		}
		protected virtual void DestroyFormats() {
			if(SingleLineStringFormat != null) SingleLineStringFormat.Dispose();
			if(SingleLineEllipsisFormat != null) SingleLineEllipsisFormat.Dispose();
			if(SingleLineEllipsisPathFormat != null) SingleLineEllipsisPathFormat.Dispose();
			if(SingleLineVerticalStringFormat != null) SingleLineVerticalStringFormat.Dispose();
		}
		public virtual void UpdateScheme(bool designTime) {
			InitDefaults();
			InitGDIResources(designTime);
		}
		public virtual void UpdateScheme() {
			UpdateScheme(false);
		}
		protected virtual void InitDefaults() {
			if(fColors != null)
				Constants.Init();
			if(constants != null)
				Colors.Init();
		}
		public virtual void UpdateMdiGlyphs(BarMdiButtonItem item, string name) {
			item.LargeGlyph = item.LargeGlyphPressed = item.LargeGlyphDisabled = item.LargeGlyphHot = null;
			item.Glyph = (Controller.GetBitmap(name).Clone() as Bitmap);
		}
		public virtual bool CanDrawCheckedCaption { get { return false; } }
	}
}
