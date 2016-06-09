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
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.WinExplorer;
using DevExpress.XtraGrid.Views.WinExplorer.Drawing;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
namespace DevExpress.XtraGrid.Registrator {
	public class WinExplorerViewPaintStyle : ViewPaintStyle {
		public override bool CanUsePaintStyle {
			get {
				return true;
			}
		}
		public override BaseViewInfo CreateViewInfo(BaseView view) {
			return new WinExplorerViewInfo((WinExplorerView)view);
		}
		public override BaseViewPainter CreatePainter(BaseView view) {
			return new WinExplorerViewPainter(view);
		}
		public override DevExpress.Utils.Drawing.BorderPainter GetBorderPainter(BaseView view, BorderStyles border) {
			return null;
		}
		public override bool IsSkin {
			get {
				return false;
			}
		}
		public override string Name {
			get {
				return "";
			}
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			WinExplorerView iconView = (WinExplorerView)view;
			HorzAlignment itemTextAlignment = GetItemTextAlignment(iconView.OptionsView.Style);
			VertAlignment itemTextVertAlignment = GetItemTextVertAlignment(iconView.OptionsView.Style);
			Font fontText = GetTextFont(iconView.OptionsView.Style);
			Font fontDescription = GetDescriptionFont(iconView.OptionsView.Style);
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo("ItemNormal", new AppearanceDefault() { Font = fontText, ForeColor = Color.Black, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemHovered", new AppearanceDefault() { Font = fontText, ForeColor = Color.Black, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault() { Font = fontText, ForeColor = Color.Black, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }),
				new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault() {Font = fontText,ForeColor = SystemColors.GrayText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment}),
				new AppearanceDefaultInfo("ItemDescriptionNormal", new AppearanceDefault() { Font = fontDescription, ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemDescriptionHovered", new AppearanceDefault() { Font = fontDescription, ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				 new AppearanceDefaultInfo("ItemDescriptionDisabled", new AppearanceDefault() {Font = fontText,ForeColor = SystemColors.GrayText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment}),
				new AppearanceDefaultInfo("ItemDescriptionPressed", new AppearanceDefault() { Font = fontDescription, ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }),
				new AppearanceDefaultInfo("GroupNormal", new AppearanceDefault() { ForeColor = Color.Black }), 
				new AppearanceDefaultInfo("GroupHovered", new AppearanceDefault() { ForeColor = Color.Black }), 
				new AppearanceDefaultInfo("GroupPressed", new AppearanceDefault() { ForeColor = Color.Black }),
				new AppearanceDefaultInfo("EmptySpace", new AppearanceDefault() { ForeColor = SystemColors.Window, BackColor = SystemColors.Window, HAlignment = HorzAlignment.Center, VAlignment = VertAlignment.Center })};
		}
		protected virtual Font GetDescriptionFont(WinExplorerViewStyle winExplorerViewStyle) {
			return AppearanceDefault.Control.Font;
		}
		protected virtual Font GetTextFont(WinExplorerViewStyle winExplorerViewStyle) {
			if(winExplorerViewStyle == WinExplorerViewStyle.Content)
				return new Font(AppearanceObject.DefaultFont.FontFamily, 12.0f);
			return AppearanceDefault.Control.Font;
		}
		protected virtual HorzAlignment GetItemTextAlignment(WinExplorerViewStyle itemType) {
			if(itemType == WinExplorerViewStyle.Content)
				return HorzAlignment.Near;
			return itemType == WinExplorerViewStyle.Small || itemType == WinExplorerViewStyle.Tiles || itemType == WinExplorerViewStyle.List ? HorzAlignment.Near : HorzAlignment.Center;
		}
		protected virtual VertAlignment GetItemTextVertAlignment(WinExplorerViewStyle itemType) {
			if(itemType == WinExplorerViewStyle.Content)
				return VertAlignment.Top;
			return itemType == WinExplorerViewStyle.Tiles ? VertAlignment.Top : VertAlignment.Default;
		}
	}
	public class FlatWinExplorerViewPaintStyle : WindowsClassicPaintStyle {
		public override string Name {
			get {
				return "Flat";
			}
		}
		public override BaseViewPainter CreatePainter(BaseView view) {
			return new FlatWinExplorerViewPainter(view);
		}
	}
	public class SkinWinExplorerViewPaintStyle : WinExplorerViewPaintStyle {
		public override bool IsSkin {
			get {
				return true;
			}
		}
		public override string Name {
			get {
				return "Skin";
			}
		}
		public override BaseViewInfo CreateViewInfo(BaseView view) {
			return new SkinWinExplorerViewInfo((WinExplorerView)view);
		}
		public override BorderPainter GetBorderPainter(BaseView view, BorderStyles border) {
			if(border == BorderStyles.NoBorder) return null;
			return new SkinGridBorderPainter(view);
		}
		public override BaseViewPainter CreatePainter(BaseView view) {
			return new SkinWinExplorerViewPainter(view);
		}
		protected internal virtual SkinElement GetGroupHeaderElement(BaseView view) {
			SkinElement elem = GridSkins.GetSkin(view)[GridSkins.SkinWinExplorerViewGroupHeader];
			if(elem == null)
				elem = RibbonSkins.GetSkin(view)[RibbonSkins.SkinPopupGalleryGroupCaption];
			return elem;
		}
		protected internal virtual SkinElement GetItemElement(BaseView view) {
			SkinElement elem = GridSkins.GetSkin(view)[GridSkins.SkinWinExplorerViewItem];
			if(elem == null)
				elem = RibbonSkins.GetSkin(view)[RibbonSkins.SkinPopupGalleryPopupButton];
			return elem;
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			WinExplorerView iconView = (WinExplorerView)view;
			HorzAlignment itemTextAlignment = GetItemTextAlignment(iconView.OptionsView.Style);
			VertAlignment itemTextVertAlignment = GetItemTextVertAlignment(iconView.OptionsView.Style);
			Font fontText = GetTextFont(iconView.OptionsView.Style);
			Font fontDescription = GetDescriptionFont(iconView.OptionsView.Style);
			SkinElement itemElem = GetItemElement(view);
			SkinElement groupElem = GetGroupHeaderElement(view);
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo("ItemNormal", new AppearanceDefault() { Font = fontText, ForeColor = itemElem.GetForeColor(ObjectState.Normal), HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemHovered", new AppearanceDefault() { Font = fontText, ForeColor = itemElem.GetForeColor(ObjectState.Hot), HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault() { Font = fontText, ForeColor = itemElem.GetForeColor(ObjectState.Pressed), HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }),
				new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault() { Font = fontText, ForeColor = itemElem.Owner.GetSystemColor(SystemColors.GrayText), HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }),
				new AppearanceDefaultInfo("ItemDescriptionNormal", new AppearanceDefault() { Font = fontDescription, ForeColor = itemElem.Properties.GetColor("ForeColorDescription"), HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemDescriptionHovered", new AppearanceDefault() { Font = fontDescription, ForeColor = itemElem.Properties.GetColor("ForeColorDescriptionHot"), HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemDescriptionDisabled", new AppearanceDefault() { Font = fontText, ForeColor = itemElem.Owner.GetSystemColor(SystemColors.GrayText), HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }),
				new AppearanceDefaultInfo("ItemDescriptionPressed", new AppearanceDefault() { Font = fontDescription, ForeColor = itemElem.Properties.GetColor("ForeColorDescriptionPressed"), HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }),
				new AppearanceDefaultInfo("GroupNormal", new AppearanceDefault() { ForeColor = groupElem.GetForeColor(ObjectState.Normal) }), 
				new AppearanceDefaultInfo("GroupHovered", new AppearanceDefault() { ForeColor = groupElem.GetForeColor(ObjectState.Hot) }), 
				new AppearanceDefaultInfo("GroupPressed", new AppearanceDefault() { ForeColor = groupElem.GetForeColor(ObjectState.Pressed) }),
				new AppearanceDefaultInfo("EmptySpace", GridSkinPaintStyle.UpdateAppearanceEx(view, GridSkins.SkinGridEmptyArea, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, HorzAlignment.Near, VertAlignment.Center)))};
		}
	}
	public class WindowsXPPaintStyle : WinExplorerViewPaintStyle {
		public WindowsXPPaintStyle() : base() { }
		public override BaseViewInfo CreateViewInfo(BaseView view) {
			return new WindowsXPWinExplorerViewInfo((WinExplorerView)view);
		}
		public override BaseViewPainter CreatePainter(BaseView view) {
			return new WindowsXPWinExplorerViewPainter(view);
		}
		public override BorderPainter GetBorderPainter(BaseView view, BorderStyles border) {
			if(border == BorderStyles.NoBorder) return null;
			return new WindowsXPTextBorderPainter();
		}
		public override string Name {
			get {
				return "WindowsXP";
			}
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			WinExplorerView iconView = (WinExplorerView)view;
			HorzAlignment itemTextAlignment = GetItemTextAlignment(iconView.OptionsView.Style);
			VertAlignment itemTextVertAlignment = GetItemTextVertAlignment(iconView.OptionsView.Style);
			Font itemFont = new Font("Tahoma", 8.0F, FontStyle.Regular);
			Font groupFont = new Font("Segoe UI", 9.0F, FontStyle.Bold);
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo("ItemNormal", new AppearanceDefault() { ForeColor = SystemColors.WindowText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = Color.FromArgb(236, 233, 216), Font = itemFont }), 
				new AppearanceDefaultInfo("ItemHovered", new AppearanceDefault() { ForeColor = SystemColors.WindowText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = Color.FromArgb(49,106,197), Font = itemFont }), 
				new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault() { ForeColor = SystemColors.Window, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = Color.FromArgb(49,106,197), BackColor2 = Color.FromArgb(150, 49, 106, 197), Font = itemFont }),
				new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault() {ForeColor = SystemColors.GrayText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = 
				Color.FromArgb(49,106,197), BackColor2 = Color.FromArgb(150, 49, 106, 197), Font = itemFont }),
				new AppearanceDefaultInfo("ItemDescriptionNormal", new AppearanceDefault() { ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, Font = itemFont }), 
				new AppearanceDefaultInfo("ItemDescriptionHovered", new AppearanceDefault() { ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, Font = itemFont }), 
				 new AppearanceDefaultInfo("ItemDescriptionDisabled", new AppearanceDefault() {ForeColor = SystemColors.GrayText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = 
				Color.FromArgb(49,106,197), BackColor2 = Color.FromArgb(150, 49, 106, 197), Font = itemFont }),
				new AppearanceDefaultInfo("ItemDescriptionPressed", new AppearanceDefault() { ForeColor = SystemColors.Window, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, Font = itemFont }),
				new AppearanceDefaultInfo("GroupNormal", new AppearanceDefault() { ForeColor = Color.Black, Font = groupFont }), 
				new AppearanceDefaultInfo("GroupHovered", new AppearanceDefault() { ForeColor = Color.Black }), 
				new AppearanceDefaultInfo("GroupPressed", new AppearanceDefault() { ForeColor = Color.Black })};
		}
	}
	public class WindowsClassicPaintStyle : WinExplorerViewPaintStyle {
		public WindowsClassicPaintStyle() : base() { }
		public override BaseViewPainter CreatePainter(BaseView view) {
			return new WindowsClassicWinExplorerViewPainter(view);
		}
		public override BaseViewInfo CreateViewInfo(BaseView view) {
			return new WindowsClassicWinExplorerViewInfo((WinExplorerView)view);
		}
		public override string Name {
			get {
				return "Classic";
			}
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			WinExplorerView iconView = (WinExplorerView)view;
			HorzAlignment itemTextAlignment = GetItemTextAlignment(iconView.OptionsView.Style);
			VertAlignment itemTextVertAlignment = GetItemTextVertAlignment(iconView.OptionsView.Style);
			Font itemFont = new Font("Tahoma", 8.0F, FontStyle.Regular);
			Font groupFont = new Font("Segoe UI", 9.0F, FontStyle.Bold);
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo("ItemNormal", new AppearanceDefault() { ForeColor = SystemColors.WindowText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = Color.FromArgb(212, 208, 200), Font = itemFont }), 
				new AppearanceDefaultInfo("ItemHovered", new AppearanceDefault() { ForeColor = SystemColors.WindowText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = Color.FromArgb(10, 36, 106), Font = itemFont }), 
				new AppearanceDefaultInfo("ItemDisabled", new AppearanceDefault() {ForeColor = SystemColors.GrayText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = 
				Color.FromArgb(212, 208, 200), Font = itemFont }),
				new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault() { ForeColor = SystemColors.Window, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = Color.FromArgb(10, 36, 106), BackColor2 = Color.FromArgb(150, 49, 106, 197), Font = itemFont }),
				new AppearanceDefaultInfo("ItemDescriptionNormal", new AppearanceDefault() { ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, Font = itemFont }), 
				new AppearanceDefaultInfo("ItemDescriptionHovered", new AppearanceDefault() { ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, Font = itemFont }), 
				  new AppearanceDefaultInfo("ItemDescriptionDisabled", new AppearanceDefault() {ForeColor = SystemColors.GrayText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BackColor = 
				Color.FromArgb(212, 208, 200), Font = itemFont }),
				new AppearanceDefaultInfo("ItemDescriptionPressed", new AppearanceDefault() { ForeColor = SystemColors.Window, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, Font = itemFont }),
				new AppearanceDefaultInfo("GroupNormal", new AppearanceDefault() { ForeColor = Color.Black, Font = groupFont }), 
				new AppearanceDefaultInfo("GroupHovered", new AppearanceDefault() { ForeColor = Color.Black, Font = groupFont }), 
				new AppearanceDefaultInfo("GroupPressed", new AppearanceDefault() { ForeColor = Color.Black, Font = groupFont })};
		}
	}
	public class Windows8PaintStyle : WindowsXPPaintStyle {
		public Windows8PaintStyle() : base() { }
		public override BaseViewPainter CreatePainter(BaseView view) {
			return new Windows8WinExplorerViewPainter(view);
		}
		public override string Name {
			get {
				return "Win8";
			}
		}
		public override BaseViewInfo CreateViewInfo(BaseView view) {
			return new Windows8WinExplorerViewInfo((WinExplorerView)view);
		}
		public override AppearanceDefaultInfo[] GetAppearanceDefaultInfo(BaseView view) {
			WinExplorerView iconView = (WinExplorerView)view;
			HorzAlignment itemTextAlignment = GetItemTextAlignment(iconView.OptionsView.Style);
			VertAlignment itemTextVertAlignment = GetItemTextVertAlignment(iconView.OptionsView.Style);
			Font groupFont = new Font("Tahoma", 8.5F, FontStyle.Regular);
			return new AppearanceDefaultInfo[] { 
				new AppearanceDefaultInfo("ItemNormal", new AppearanceDefault() { ForeColor = SystemColors.WindowText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemHovered", new AppearanceDefault() { ForeColor = SystemColors.WindowText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BorderColor = Color.FromArgb(112, 192, 231), BackColor = Color.FromArgb(100, 112, 192, 231) }), 
				new AppearanceDefaultInfo("ItemPressed", new AppearanceDefault() { ForeColor = SystemColors.WindowText, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment, BorderColor = Color.FromArgb(102,167,232), BackColor = Color.FromArgb(100,102,167,232) }),
				new AppearanceDefaultInfo("ItemDescriptionNormal", new AppearanceDefault() { ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemDescriptionHovered", new AppearanceDefault() { ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }), 
				new AppearanceDefaultInfo("ItemDescriptionPressed", new AppearanceDefault() { ForeColor = Color.Gray, HAlignment = itemTextAlignment, VAlignment = itemTextVertAlignment }),
				new AppearanceDefaultInfo("GroupNormal", new AppearanceDefault() { ForeColor = Color.Blue, Font = groupFont }), 
				new AppearanceDefaultInfo("GroupHovered", new AppearanceDefault() { ForeColor = Color.Black }), 
				new AppearanceDefaultInfo("GroupPressed", new AppearanceDefault() { ForeColor = Color.Black })};
		}
	}
}
