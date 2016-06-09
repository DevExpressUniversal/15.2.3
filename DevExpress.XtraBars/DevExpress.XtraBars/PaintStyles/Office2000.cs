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
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils;
using DevExpress.XtraBars.InternalItems;
namespace DevExpress.XtraBars.ViewInfo {
	public class BarOffice2000Constants : BarConstants {
		public BarOffice2000Constants(BarManagerPaintStyle paintStyle) : base(paintStyle) { }
		public override void Init() {
			TopDockIndent = 0;
			SubMenuScrollLinkHeight = 10;
			SubMenuScrollLinkHeightInTouch = 15;
			BarDockedRowIndent = BarDockedRowBarIndent = 0;
			DragBorderSize = 7;
			FloatingShadowSize = 4;
			BarItemVertIndent = 2; 
			SubMenuItemBottomIndent = 2;
			SubMenuItemTopIndent = 0; 
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
			BarQuickButtonWidth = BarButtonArrowWidth = 11;
			BarMenuButtonArrowWidth = 5;
			BarMenuButtonArrowTextIndent = 2; 
			BarButtonRealArrowWidth = 5;
			SubMenuRealArrowWidth = 7;
			SubMenuSeparatorHeight = 6;
			ImageShadowHIndent = ImageShadowVIndent = 2;
			ImageHIndent = ImageVIndent = 0;
			ImageOffset = 1;
			BarSeparatorWidth = 6;
			BarSeparatorLineThickness = 2;
			BarControlFormHasShadow = true;
			BarLinksVertIndent = 2;
			BarLinksHorzIndent = 2;
			SubMenuRecentExpanderHeight = BarButtonRealArrowWidth * 2 + 2;
		}
		public override DevExpress.XtraEditors.Controls.BorderStyles DefaultEditBorderStyle {
			get { return DevExpress.XtraEditors.Controls.BorderStyles.Flat; }
		}
		public override bool AllowLinkGlyphShadow { get { return false; } }
		public override bool AllowLinkLighting { get { return false; } }
		public override bool AllowLinkShadows { get { return false; } }
	}
	public class BarOffice2000ColorConstants : BarColorConstants {
		public BarOffice2000ColorConstants(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		protected override void InitAppearance() {
			Font font = DrawParameters.ItemsFont;
			this.fMenuAppearance = new MenuAppearance();
			this.fMenuAppearance.AppearanceMenu.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.Control, font)));
			this.fMenuAppearance.SideStrip.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control)));
			this.fMenuAppearance.SideStripNonRecent.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, PaintHelper.RealLight(SystemColors.Control))));
			this.fMenuAppearance.MenuBar.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, DevExpress.Utils.ColorUtils.FlatBarBorderColor, font)));
			this.fMenuAppearance.MenuCaption.Assign(new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDark, Color.Empty, font, HorzAlignment.Center, VertAlignment.Center)));
			this.fMenuAppearance.HeaderItemAppearance.Font = new Font(font, FontStyle.Bold);
			MenuItemCaptionWithDescription.Assign(MenuAppearance.Menu);
			MenuItemCaptionWithDescription.SetFont(new Font(MenuItemCaptionWithDescription.Normal.Font, FontStyle.Bold));
			MenuItemDescription.Assign(MenuAppearance.Menu);
			MenuItemDescription.SetWordWrap(WordWrap.Wrap);
			MenuItemDescription.SetVAlignment(VertAlignment.Top);
			fAppearances[BarAppearance.Dock] = new AppearanceObject(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			fAppearances[BarAppearance.Bar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			fAppearances[BarAppearance.BarNoBorder] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			fAppearances[BarAppearance.MainMenu] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
			fAppearances[BarAppearance.StatusBar] = new StateAppearances(new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, font));
		}
		protected override void InitColors() {
			fColors[BarColor.DesignTimeSelectColor] = ColorUtils.FlatBarItemHighLightBackColor;
			fColors[BarColor.SubMenuSeparatorColor] = PaintHelper.RealDark(MenuAppearance.Menu.BackColor);
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
		}
	}
	public class BarOffice2000DrawParameters : BarDrawParameters {
		public BarOffice2000DrawParameters(BarManagerPaintStyle paintStyle) : base(paintStyle) { 
		}
		protected override BarConstants CreateConstants() { return new BarOffice2000Constants(PaintStyle); } 
		protected override BarColorConstants CreateColors() { return new BarOffice2000ColorConstants(PaintStyle); } 
	}
}
