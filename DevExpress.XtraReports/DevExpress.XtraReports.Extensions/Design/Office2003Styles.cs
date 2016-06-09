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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design.Ruler;
using System.Drawing.Printing;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.WXPaint;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
namespace DevExpress.XtraReports.Design {
	public abstract class Office2003Colors {
		static XPThemeType xpThemeType;
		static Office2003Colors office2003Colors;
		public abstract Color ReportBackground { get; }
		public abstract Color RulerBackground { get; }
		public abstract Color CornerPanelOuterBorder { get; }
		public abstract Color CornerPanelInnerBorder { get; }
		public abstract Color RulerRightMargin { get; }
		public abstract Color PopupFormBorder { get; }
		public abstract Color PopupFormBackground1 { get; }
		public abstract Color PopupFormBackground2 { get; }
		public abstract Color PopupFormCaption1 { get; }
		public abstract Color PopupFormCaption2 { get; }
		public abstract Color PopupFormText { get; }
		public abstract Color ComponentTrayBackground { get; }
		public abstract Color SliderContentInner { get; }
		public abstract Color SliderContentOuter { get; }
		public abstract Color BandButtonSign { get; }
		public abstract Color SmartTagBackColor { get; }
		public abstract Color[] BandColorsLevel0 { get; }
		public abstract Color[] BandColorsLevel1 { get; }
		public abstract Color[] BandColorsLevel2 { get; }
		public abstract Color[] SelectedBandColors { get; }
		public abstract Color[] BandBorderColorsLevel0 { get; }
		public abstract Color[] BandBorderColorsLevel1 { get; }
		public abstract Color[] BandBorderColorsLevel2 { get; }
		public abstract Color[] SelectedBandBorderColors { get; }
		public abstract Color[] BandButtonBorderColorsLevel0 { get; }
		public abstract Color[] BandButtonBorderColorsLevel1 { get; }
		public abstract Color[] BandButtonBorderColorsLevel2 { get; }
		public abstract Color[] SelectedBandButtonBorderColors { get; }
		public static Office2003Colors Current { 
			get {
				XPThemeType currentXPThemeType = DevExpress.Utils.WXPaint.WXPPainter.Default.GetXPThemeType();
				if(currentXPThemeType == xpThemeType && office2003Colors != null) 
					return office2003Colors;
				xpThemeType = currentXPThemeType;
				switch(xpThemeType) {
					case XPThemeType.NormalColor :
						office2003Colors = new Office2003NormalColor();
						break;
					case XPThemeType.Homestead :
						office2003Colors = new Office2003Homestead();
						break;
					case XPThemeType.Metallic :
						office2003Colors = new Office2003Metallic();
						break;
					case XPThemeType.Unknown :
					default :
						office2003Colors = new Office2003Unknown();
						break;
				}
				return office2003Colors;
			}
		}
	}
	public class Office2003NormalColor : Office2003Colors {
		public override Color ReportBackground { get { return Color.FromArgb(0xF6, 0xFA, 0xFF); } }
		public override Color RulerBackground { get { return Color.FromArgb(0xD8, 0xE7, 0xFC); } }
		public override Color CornerPanelOuterBorder { get { return Color.FromArgb(0x4B, 0x78, 0xCA); } }
		public override Color CornerPanelInnerBorder { get { return Color.White; } }
		public override Color RulerRightMargin { get { return Color.FromArgb(0x9E, 0xBE, 0xF5); } }
		public override Color PopupFormBorder { get { return Color.White; } }
		public override Color PopupFormBackground1 { get { return Color.FromArgb(0xDD, 0xEC, 0xFE); } }
		public override Color PopupFormBackground2 { get { return Color.FromArgb(0xBA, 0xD1, 0xF1); } }
		public override Color PopupFormCaption1 { get { return Color.FromArgb(0xC4, 0xDB, 0xF9); } }
		public override Color PopupFormCaption2 { get { return Color.FromArgb(0x66, 0x8F, 0xE0); } }
		public override Color PopupFormText { get { return Color.FromArgb(0x33, 0x2C, 0x86); } }
		public override Color ComponentTrayBackground { get { return Color.FromArgb(0xBB, 0xD4, 0xF8); } }
		public override Color SliderContentInner { get { return Color.White; } }
		public override Color SliderContentOuter { get { return Color.FromArgb(0x6A, 0x8C, 0xCB); } }
		public override Color BandButtonSign { get { return Color.FromArgb(0x30, 0x48, 0x60); } }
		public override Color SmartTagBackColor { get { return Color.FromArgb(0xE3, 0xEF, 0xFD); } }
		public override Color[] BandColorsLevel0 { get { return new Color[] { Color.FromArgb(0xDA, 0xEA, 0xFD), Color.FromArgb(0xA5, 0xC3, 0xED), Color.FromArgb(0xE3, 0xEF, 0xFD), Color.FromArgb(0xBB, 0xD2, 0xF1) }; } }
		public override Color[] BandColorsLevel1 { get { return new Color[] { Color.FromArgb(0xD8, 0xF7, 0xA7), Color.FromArgb(0xA6, 0xD0, 0x63), Color.FromArgb(0xE2, 0xF9, 0xBD), Color.FromArgb(0xBC, 0xDC, 0x8A) }; } }
		public override Color[] BandColorsLevel2 { get { return new Color[] { Color.FromArgb(0xFF, 0xEB, 0xA4), Color.FromArgb(0xEF, 0xC7, 0x5B), Color.FromArgb(0xFF, 0xF0, 0xBA), Color.FromArgb(0xF2, 0xD3, 0x84) }; } }
		public override Color[] SelectedBandColors { get { return new Color[] { Color.FromArgb(0xFF, 0xC9, 0x85), Color.FromArgb(0xFE, 0x90, 0x4D), Color.FromArgb(0xFF, 0xC9, 0x85), Color.FromArgb(0xFE, 0x8F, 0x4D) }; } }
		public override Color[] BandBorderColorsLevel0 { get { return new Color[] { Color.White, Color.FromArgb(0x6A, 0x8C, 0xCB) }; } }
		public override Color[] BandBorderColorsLevel1 { get { return new Color[] { Color.White, Color.FromArgb(0x79, 0xA9, 0x2C) }; } }
		public override Color[] BandBorderColorsLevel2 { get { return new Color[] { Color.White, Color.FromArgb(0xC3, 0x92, 0x0F) }; } }
		public override Color[] SelectedBandBorderColors { get { return new Color[] { Color.White, Color.FromArgb(0xC4, 0x52, 0x0D) }; } }
		public override Color[] BandButtonBorderColorsLevel0 { get { return new Color[] { Color.FromArgb(0x90, 0xA8, 0xC0), Color.FromArgb(0x30, 0x48, 0x60) }; } }
		public override Color[] BandButtonBorderColorsLevel1 { get { return new Color[] { Color.FromArgb(0x91, 0xBB, 0x50), Color.FromArgb(0x56, 0x80, 0x12) }; } }
		public override Color[] BandButtonBorderColorsLevel2 { get { return new Color[] { Color.FromArgb(0xCD, 0xA6, 0x37), Color.FromArgb(0x96, 0x6F, 0x07) }; } }
		public override Color[] SelectedBandButtonBorderColors { get { return new Color[] { Color.FromArgb(0xE0, 0x7F, 0x3F), Color.FromArgb(0x91, 0x49, 0x1D) }; } }
	}
	public class Office2003Homestead : Office2003NormalColor {
		public override Color ReportBackground { get { return Color.FromArgb(0xF6, 0xFA, 0xE0); } }
		public override Color RulerBackground { get { return Color.FromArgb(0xE2, 0xE7, 0xBF); } }
		public override Color CornerPanelOuterBorder { get { return Color.FromArgb(0x75, 0x8D, 0x5E); } }
		public override Color CornerPanelInnerBorder { get { return Color.White; } }
		public override Color RulerRightMargin { get { return Color.FromArgb(0xAB, 0xC0, 0x8A); } }
		public override Color PopupFormBackground1 { get { return Color.FromArgb(0xE9, 0xE3, 0xBB); } }
		public override Color PopupFormBackground2 { get { return Color.FromArgb(0xE6, 0xEA, 0xD0); } }
		public override Color PopupFormCaption1 { get { return Color.FromArgb(0xD2, 0xDF, 0xAE); } }
		public override Color PopupFormCaption2 { get { return Color.FromArgb(0xA2, 0xB1, 0x81); } }
		public override Color PopupFormText { get { return Color.FromArgb(0x5A, 0x6B, 0x46); } }
		public override Color ComponentTrayBackground { get { return Color.FromArgb(0xDF, 0xDF, 0xB7); } }
		public override Color SliderContentOuter { get { return Color.FromArgb(0x60, 0x80, 0x58); } }
		public override Color BandButtonSign { get { return Color.FromArgb(0x60, 0x80, 0x58); } }
		public override Color SmartTagBackColor { get { return Color.FromArgb(0xE2, 0xE7, 0xBF); } }
		public override Color[] BandColorsLevel0 { get { return new Color[] { Color.FromArgb(0xF4, 0xF7, 0xDE), Color.FromArgb(0xB9, 0xC7, 0x94), Color.FromArgb(0xF7, 0xF9, 0xE6), Color.FromArgb(0xCA, 0xD6, 0xAE) }; } }
		public override Color[] BandBorderColorsLevel0 { get { return new Color[] { Color.White, Color.FromArgb(0x60, 0x80, 0x58) }; } }
		public override Color[] BandButtonBorderColorsLevel0 { get { return new Color[] { Color.FromArgb(0x88, 0xAC, 0x7F), Color.FromArgb(0x60, 0x80, 0x58) }; } }
	}
	public class Office2003Metallic : Office2003NormalColor {
		public override Color RulerBackground { get { return Color.FromArgb(0xDF, 0xDF, 0xEA); } }
		public override Color CornerPanelOuterBorder { get { return Color.FromArgb(0x7C, 0x7C, 0x94); } }
		public override Color CornerPanelInnerBorder { get { return Color.White; } }
		public override Color RulerRightMargin { get { return Color.FromArgb(0xB1, 0xB0, 0xC3); } }
		public override Color PopupFormBackground1 { get { return Color.FromArgb(0xD4, 0xD5, 0xE5); } }
		public override Color PopupFormBackground2 { get { return Color.FromArgb(0xE3, 0xE3, 0xEC); } }
		public override Color PopupFormCaption1 { get { return Color.FromArgb(0xD0, 0xD0, 0xDF); } }
		public override Color PopupFormCaption2 { get { return Color.FromArgb(0xA9, 0xA8, 0xBF); } }
		public override Color PopupFormText { get { return Color.FromArgb(0x5C, 0x5B, 0x79); } }
		public override Color ComponentTrayBackground { get { return Color.FromArgb(0xB8, 0xBB, 0xCD); } }
		public override Color SmartTagBackColor { get { return Color.FromArgb(0xDF, 0xDF, 0xEA); } }
		public override Color[] BandColorsLevel0 { get { return new Color[] { Color.FromArgb(0xF2, 0xF2, 0xF9), Color.FromArgb(0x9B, 0x9A, 0xB8), Color.FromArgb(0xF5, 0xF5, 0xFA), Color.FromArgb(0xB4, 0xB2, 0xC9) }; } }
		public override Color[] BandBorderColorsLevel0 { get { return new Color[] { Color.White, Color.FromArgb(0x7C, 0x7C, 0x94) }; } }
	}
	public class Office2003Unknown : Office2003Colors {
		public override Color ReportBackground { get { return SystemColors.ControlDark; } }
		public override Color RulerBackground { get { return SystemColors.Control; } }
		public override Color CornerPanelOuterBorder { get { return SystemColors.ControlDark; } }
		public override Color CornerPanelInnerBorder { get { return SystemColors.ControlLightLight; } }
		public override Color RulerRightMargin { get { return SystemColors.ControlDark; } }
		public override Color PopupFormBorder { get { return Color.White; } }
		public override Color PopupFormBackground1 { get { return SystemColors.ControlLight; } }
		public override Color PopupFormBackground2 { get { return SystemColors.Control; } }
		public override Color PopupFormCaption1 { get { return SystemColors.ControlLight; } }
		public override Color PopupFormCaption2 { get { return SystemColors.ControlDark; } }
		public override Color PopupFormText { get { return SystemColors.ControlText; } }
		public override Color ComponentTrayBackground { get { return SystemColors.ControlDark; } }
		public override Color SliderContentInner { get { return SystemColors.ControlLightLight; } }
		public override Color SliderContentOuter { get { return SystemColors.ControlDark; } }
		public override Color BandButtonSign { get { return SystemColors.ActiveCaption; } }
		public override Color SmartTagBackColor { get { return SystemColors.Control; } }
		public override Color[] BandColorsLevel0 { get { return new Color[] { SystemColors.Control,  SystemColors.ControlDark, SystemColors.ControlLight, SystemColors.Control}; } }
		public override Color[] BandColorsLevel1 { get { return BandColorsLevel0; } }
		public override Color[] BandColorsLevel2 { get { return BandColorsLevel0; } }
		public override Color[] SelectedBandColors { get { return new Color[] { SystemColors.Control, SystemColors.ControlDark, SystemColors.Control, SystemColors.ControlDark }; } }
		public override Color[] BandBorderColorsLevel0 { get { return new Color[] { Color.White, SystemColors.InactiveBorder }; } }
		public override Color[] BandBorderColorsLevel1 { get { return BandBorderColorsLevel0; } }
		public override Color[] BandBorderColorsLevel2 { get { return BandBorderColorsLevel0; } }
		public override Color[] SelectedBandBorderColors { get { return new Color[] { Color.White, SystemColors.ActiveBorder }; } }
		public override Color[] BandButtonBorderColorsLevel0 { get { return BandBorderColorsLevel0; } }
		public override Color[] BandButtonBorderColorsLevel1 { get { return BandButtonBorderColorsLevel0; } }
		public override Color[] BandButtonBorderColorsLevel2 { get { return BandButtonBorderColorsLevel0; } }
		public override Color[] SelectedBandButtonBorderColors { get { return SelectedBandBorderColors; } }
	}
}
