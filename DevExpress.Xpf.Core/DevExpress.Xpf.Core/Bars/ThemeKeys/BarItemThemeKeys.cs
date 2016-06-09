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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Utils.Themes;
namespace DevExpress.Xpf.Bars.Themes {
	public enum BarButtonItemThemeKeys {
		Template,
		InMenuTemplate,
		InMenuHorizontalTemplate,
		InRibbonPageGroupTemplate,
		TextSplitterStyleInRibbonPageGroup,
		SelectedTextStyleInTextSplitter,
		NormalTextStyleInTextSplitter,
		ControlTemplateInRadialMenu
	}
	public enum BarCheckItemThemeKeys {
		Template,
		InMenuTemplate,
		InMenuHorizontalTemplate,
		InRibbonPageGroupTemplate,
		BorderMarginInMenu,
		TriStateBorderInMenu,
		GlyphCheckBorderInMenu,
		GlyphTriStateBorderInMenu,
		CheckInMenu,
		TextSplitterStyleInRibbonPageGroup,
		SelectedTextStyleInTextSplitter,
		NormalTextStyleInTextSplitter,
		ControlTemplateInRadialMenu
	}
	public enum BarSubItemThemeKeys {
		Template,
		InMenuTemplate,
		InMenuHorizontalTemplate,
		InRibbonPageGroupTemplate,
		ArrowStyle,
		ArrowTemplate,
		ArrowStyleInStatusBar,
		ArrowTemplateInStatusBar,
		ArrowStyleInMainMenu,
		ArrowTemplateInMainMenu,
		ArrowStyleInMenu,
		ArrowTemplateInMenu,
		ContentToArrowIndent,
		ArrowStyleInRibbonQuickAccessToolbarFooter,
		ArrowStyleInRibbonStatusBarLeft,
		ArrowStyleInRibbonStatusBarRight,
		ArrowStyleInRibbonPageGroup,
		ArrowStyleInRibbonPageHeader,
		ArrowStyleInRibbonQuickAccessToolbar,
		ArrowStyleInBarButtonGroup,
		ArrowTemplateInRibbonQuickAccessToolbarFooter,
		ArrowTemplateInRibbonStatusBarLeft,
		ArrowTemplateInRibbonStatusBarRight,
		ArrowTemplateInRibbonPageGroup,
		ArrowTemplateInRibbonPageHeader,
		ArrowTemplateInRibbonQuickAccessToolbar,
		ArrowTemplateInBarButtonGroup,
		TextSplitterStyleInRibbonPageGroup,
		NormalArrowTemplateInTextSplitter,
		SelectedArrowTemplateInTextSplitter,
		SelectedTextStyleInTextSplitter,
		NormalTextStyleInTextSplitter,
		BottomSideArrowContainerStyleInTextSplitter,
		RightSideArrowContainerStyleInTextSplitter,
		LayoutPanelStyle,
		LayoutPanelStyleInMainMenu,
		LayoutPanelStyleInStatusBar,
		LayoutPanelStyleInRibbonPageGroup,
		LayoutPanelStyleInRibbonToolbar,
		LayoutPanelStyleInRibbonToolbarFooter,
		LayoutPanelStyleInRibbonPageHeader,
		LayoutPanelStyleInRibbonStatusBarLeft,
		LayoutPanelStyleInRibbonStatusBarRight,
		LayoutPanelStyleInButtonGroup,
		GlyphBorderStyle,
		ContentMargin,
		ControlTemplateInRadialMenu,
	}
	public enum BarSplitButtonItemThemeKeys {
		Template,
		InMenuTemplate,
		InMenuHorizontalTemplate,
		InPageGroupTemplate,
		InRibbonPageHeaderTemplate,
		InRibbonQuickAccessToolbarFooterTemplate,
		ArrowLayoutPanelStyle,
		ArrowLayoutPanelStyleInMainMenu,
		ArrowLayoutPanelStyleInStatusBar,
		ArrowLayoutPanelStyleInRibbonToolbar,
		ArrowLayoutPanelStyleInRibbonStatusBarLeft,
		ArrowLayoutPanelStyleInRibbonStatusBarRight,
		ArrowLayoutPanelStyleInRibbonPageGroup,
		ArrowLayoutPanelStyleInBarButtonGroup,
		ArrowStyle,
		ArrowTemplate,
		ArrowStyleInMenu,
		ArrowTemplateInMenu,
		ArrowStyleInMainMenu,
		ArrowTemplateInMainMenu,
		ArrowStyleInStatusBar,
		ArrowTemplateInStatusBar,
		ArrowStyleInRibbonQuickAccessToolbarFooter,
		ArrowTemplateInRibbonQuickAccessToolbarFooter,
		ArrowStyleInRibbonStatusBarLeft,
		ArrowTemplateInRibbonStatusBarLeft,
		ArrowStyleInRibbonStatusBarRight,
		ArrowTemplateInRibbonStatusBarRight,
		ArrowStyleInRibbonPageGroup,
		ArrowTemplateInRibbonPageGroup,
		ArrowStyleInRibbonPageHeader,
		ArrowTemplateInRibbonPageHeader,
		ArrowStyleInRibbonQuickAccessToolbar,
		ArrowTemplateInRibbonQuickAccessToolbar,
		ArrowStyleInBarButtonGroup,
		ArrowTemplateInBarButtonGroup,
		ContentBorderTemplateInRibbonPageGroup,
		ArrowBorderTemplateInRibbonPageGroup,
		LargeContentBorderTemplateInRibbonPageGroup,
		LargeArrowBorderTemplateInRibbonPageGroup,
		LargeArrowBorderStyleInRibbonPageGroup,
		LargeContentBorderStyleInRibbonPageGroup,
		TextSplitterStyleInRibbonPageGroup,
		NormalArrowTemplateInTextSplitter,
		SelectedArrowTemplateInTextSplitter,
		SelectedTextStyleInTextSplitter,
		NormalTextStyleInTextSplitter,
		BottomSideArrowContainerStyleInTextSplitter,
		RightSideArrowContainerStyleInTextSplitter,
		ControlTemplateInRadialMenu,
	}
	public enum BarSplitCheckItemThemeKeys {
		Template,
		InMenuTemplate,
		InMenuHorizontalTemplate,
		InPageGroupTemplate,
		ArrowLayoutPanelStyle,
		ArrowLayoutPanelStyleInMainMenu,
		ArrowLayoutPanelStyleInStatusBar,
		ArrowLayoutPanelStyleInRibbonToolbar,
		ArrowLayoutPanelStyleInRibbonStatusBarLeft,
		ArrowLayoutPanelStyleInRibbonStatusBarRight,
		ArrowLayoutPanelStyleInRibbonPageGroup,
		ArrowLayoutPanelStyleInBarButtonGroup,
		ArrowStyle,
		ArrowTemplate,
		ArrowTemplateInMenu,
		ArrowTemplateInMainMenu,
		ArrowTemplateInStatusBar,
		ArrowTemplateInRibbonStatusBarLeft,
		ArrowTemplateInRibbonStatusBarRight,
		ArrowTemplateInRibbonPageGroup,
		ArrowTemplateInBarButtonGroup,
		ArrowTemplateInRibbonQuickAccessToolbar,
		ContentBorderTemplateInRibbonPageGroup,
		ArrowBorderTemplateInRibbonPageGroup,
		LargeContentBorderTemplateInRibbonPageGroup,
		LargeContentBorderStyleInRibbonPageGroup,
		LargeArrowBorderTemplateInRibbonPageGroup,
		LargeArrowBorderStyleInRibbonPageGroup,
		TextSplitterStyleInRibbonPageGroup,
		NormalArrowTemplateInTextSplitter,
		SelectedArrowTemplateInTextSplitter,
		SelectedTextStyleInTextSplitter,
		NormalTextStyleInTextSplitter,
		BottomSideArrowContainerStyleInTextSplitter,
		RightSideArrowContainerStyleInTextSplitter,
		ControlTemplateInRadialMenu
	}
	public enum BarEditItemThemeKeys {
		Template,
		InMenuTemplate,
		LayoutPanelStyle,
		LayoutPanelStyleInMenu,
		LayoutPanelStyleInMainMenu,
		LayoutPanelStyleInStatusBar,
		LayoutPanelStyleInRibbonToolbar,
		LayoutPanelStyleInRibbonPageHeader,
		LayoutPanelStyleInRibbonStatusBarLeft,
		LayoutPanelStyleInRibbonStatusBarRight,
		LayoutPanelStyleInButtonGroup,
		NormalContentStyleInButtonGroup,
		HotContentStyleInButtonGroup,
		PressedContentStyleInButtonGroup,
		DisabledContentStyleInButtonGroup,
		NormalContentStyleInMenu,
		HotContentStyleInMenu,
		PressedContentStyleInMenu,
		DisabledContentStyleInMenu,
		NormalEditStyle,
		HotEditStyle,
		PressedEditStyle,
		DisabledEditStyle,
		NormalEditStyleInMenu,
		HotEditStyleInMenu,
		PressedEditStyleInMenu,
		DisabledEditStyleInMenu,
		NormalEditStyleInMainMenu,
		HotEditStyleInMainMenu,
		PressedEditStyleInMainMenu,
		DisabledEditStyleInMainMenu,
		NormalEditStyleInStatusBar,
		HotEditStyleInStatusBar,
		PressedEditStyleInStatusBar,
		DisabledEditStyleInStatusBar,
		NormalEditStyleInButtonGroup,
		HotEditStyleInButtonGroup,
		PressedEditStyleInButtonGroup,
		DisabledEditStyleInButtonGroup,
		NormalEditStyleInRibbonPageGroup,
		HotEditStyleInRibbonPageGroup,
		PressedEditStyleInRibbonPageGroup,
		DisabledEditStyleInRibbonPageGroup,
		InMenuClientPadding,
		EditContentMargin,
		InRibbonEditContentMargin,
		ControlTemplateInRadialMenu
	}
	public enum BarStaticItemThemeKeys {
		Template,
		InMenuTemplate,
		BorderTemplate,
		BorderPadding,
		ControlTemplateInRadialMenu
	}
	public enum BarItemSeparatorThemeKeys {
		Template,
		InMenuTemplate,
		InMenuHorizontalTemplate,
		InRibbonStatusBarLeftPartTemplate,
		InRibbonStatusBarRightPartTemplate,
		InStatusBarTemplate,
		InRibbonQuickAccessToolbarTemplate,
		InRibbonQuickAccessToolbarCommonTemplate,
		InRibbonQuickAccessToolbarFooterTemplate,
		InRibbonPageGroupTemplate,
		InRibbonPageHeaderTemplate
	}
	public enum BarItemLinkMenuHeaderThemeKeys {
		Template,
		HeaderTemplate,
		HeaderStyle,
		ItemsControlTemplate,
		HorizontalGlyphPadding,
		RowIndent,
		StatesHolder,
		ColumnIndent
	}
	public enum GlyphSideControlThemeKeys {
		Template
	}
	public enum BarItemThemeKeys { 
		BorderStyle,
		BorderTemplate,
		BorderStyleInMenu,
		BorderTemplateInMenu,
		BorderStyleInApplicationMenu,
		BorderTemplateInApplicationMenu,
		BorderStyleInMainMenu,
		BorderTemplateInMainMenu,
		BorderStyleInStatusBar,
		BorderTemplateInStatusBar,
		BorderStyleInRibbonStatusBarLeft,
		BorderTemplateInRibbonStatusBarLeft,
		BorderStyleInRibbonStatusBarRight,
		BorderTemplateInRibbonStatusBarRight,
		BorderStyleInRibbonPageHeader,
		BorderTemplateInRibbonPageHeader,
		BorderStyleInMenuHorizontal,
		BorderTemplateInMenuHorizontal,
		NormalContentStyle,
		HotContentStyle,
		PressedContentStyle,
		DisabledContentStyle,
		NormalContentStyleInMenu,
		HotContentStyleInMenu,
		PressedContentStyleInMenu,
		DisabledContentStyleInMenu,
		NormalContentStyleInMainMenu,
		HotContentStyleInMainMenu,
		PressedContentStyleInMainMenu,
		DisabledContentStyleInMainMenu,
		NormalContentStyleInStatusBar,
		HotContentStyleInStatusBar,
		PressedContentStyleInStatusBar,
		DisabledContentStyleInStatusBar,
		NormalContentStyleInRibbonStatusBarLeft,
		HotContentStyleInRibbonStatusBarLeft,
		PressedContentStyleInRibbonStatusBarLeft,
		DisabledContentStyleInRibbonStatusBarLeft,
		NormalContentStyleInRibbonStatusBarRight,
		HotContentStyleInRibbonStatusBarRight,
		PressedContentStyleInRibbonStatusBarRight,
		DisabledContentStyleInRibbonStatusBarRight,
		NormalContentStyleInRibbonPageHeader,
		HotContentStyleInRibbonPageHeader,
		PressedContentStyleInRibbonPageHeader,
		DisabledContentStyleInRibbonPageHeader,
		NormalDescriptionStyleInApplicationMenu,
		HotDescriptionStyleInApplicationMenu,
		PressedDescriptionStyleInApplicationMenu,
		DisabledDescriptionStyleInApplicationMenu,
		LayoutPanelStyle,
		LayoutPanelStyleInMainMenu,
		LayoutPanelStyleInStatusBar,
		LayoutPanelStyleInRibbonStatusBarLeft,
		LayoutPanelStyleInRibbonStatusBarRight,
		LayoutPanelStyleInRibbonPageHeader,
		ToolTipTemplate,
		InMenuClientPadding,
		InMenuContentMargin,
		InMenuKeyGestureMargin,
		DisableStateOpacityValue,
		SnappedContentTemplate,
		Margin,
		BorderTemplateInRibbonToolbar,
		BorderStyleInRibbonToolbar,
		BorderTemplateInRibbonToolbarFooter,
		BorderStyleInRibbonToolbarFooter,
		NormalContentStyleInRibbonToolbar,
		HotContentStyleInRibbonToolbar,
		PressedContentStyleInRibbonToolbar,
		DisabledContentStyleInRibbonToolbar,
		LayoutPanelStyleInRibbonToolbar,
		BorderStyleInRibbonPageGroup,
		BorderTemplateInRibbonPageGroup,
		NormalContentStyleInRibbonPageGroup,
		HotContentStyleInRibbonPageGroup,
		PressedContentStyleInRibbonPageGroup,
		DisabledContentStyleInRibbonPageGroup,
		LayoutPanelStyleInRibbonPageGroup,
		BorderStyleInButtonGroup,
		BorderTemplateInButtonGroup,
		NormalContentStyleInButtonGroup,
		HotContentStyleInButtonGroup,
		PressedContentStyleInButtonGroup,
		DisabledContentStyleInButtonGroup,
		LayoutPanelStyleInButtonGroup,
		CenterEditorBorderInPageGroup,
		LeftEditorBorderInPageGroup,
		RightEditorBorderInPageGroup,
		SingleEditorBorderInPageGroup,
		NormalContentStyleInApplicationMenu,
		HotContentStyleInApplicationMenu,
		PressedContentStyleInApplicationMenu,
		DisabledContentStyleInApplicationMenu,
		NormalContentStyleInRibbonToolbarFooter,
		HotContentStyleInRibbonToolbarFooter,
		PressedContentStyleInRibbonToolbarFooter,
		DisabledContentStyleInRibbonToolbarFooter,
		LayoutPanelStyleInRibbonToolbarFooter,
		ControlTemplateInRadialMenu,
	}
	public enum BarItemTouchSplitterThemeKeys {
		Template,
		InMenuTemplate,
		InMenuHorizontalTemplate,
		InRibbonStatusBarLeftPartTemplate,
		InRibbonStatusBarRightPartTemplate,
		InStatusBarTemplate,
		InRibbonQuickAccessToolbarTemplate,
		InRibbonQuickAccessToolbarCommonTemplate,
		InRibbonQuickAccessToolbarFooterTemplate,
		InRibbonPageGroupTemplate,
		InRibbonPageHeaderTemplate
	}
	public enum BarItemLayoutPanelThemeKeys {
		StyleInBar,
		StyleInMainMenu,
		StyleInStatusBar,
		StyleInMenu,
		StyleInApplicationMenu,
		StyleInDropDownGallery,
		StyleInRibbonPageGroup,
		StyleInButtonGroup,
		StyleInQAT,
		StyleInQATFooter,
		StyleInRibbonPageHeader,
		StyleInRibbonStatusBarLeft,
		StyleInRibbonStatusBarRight,
		StyleInMenuHeader
	}	
	public class BarItemLayoutPanelThemeKeyExtension : ThemeKeyExtensionBase<BarItemLayoutPanelThemeKeys> {
		public BarItemLayoutPanelThemeKeyExtension() {  }
	}
	public class BarButtonItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public BarButtonItemLayoutPanelThemeKeyExtension() {  }
	}
	public class BarCheckItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public BarCheckItemLayoutPanelThemeKeyExtension() {  }
	}
	public class BarSubItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public BarSubItemLayoutPanelThemeKeyExtension() {  }
	}
	public class BarSplitButtonItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public BarSplitButtonItemLayoutPanelThemeKeyExtension() {  }
	}
	public class BarEditItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public BarEditItemLayoutPanelThemeKeyExtension() {  }
	}
	public class BarSplitCheckItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public BarSplitCheckItemLayoutPanelThemeKeyExtension() {  }
	}
	public class BarStaticItemLayoutPanelThemeKeyExtension : BarItemLayoutPanelThemeKeyExtension {
		public BarStaticItemLayoutPanelThemeKeyExtension() {  }
	}
	public class BarButtonItemThemeKeyExtension : ThemeKeyExtensionBase<BarButtonItemThemeKeys> {
		public BarButtonItemThemeKeyExtension() { }
	}
	public class BarCheckItemThemeKeyExtension : ThemeKeyExtensionBase<BarCheckItemThemeKeys> {
		public BarCheckItemThemeKeyExtension() { }
	}
	public class BarSubItemThemeKeyExtension : ThemeKeyExtensionBase<BarSubItemThemeKeys> {
		public BarSubItemThemeKeyExtension() { }
	}
	public class BarSplitButtonItemThemeKeyExtension : ThemeKeyExtensionBase<BarSplitButtonItemThemeKeys> {
		public BarSplitButtonItemThemeKeyExtension() { }
	}
	public class BarSplitCheckItemThemeKeyExtension : ThemeKeyExtensionBase<BarSplitCheckItemThemeKeys> {
		public BarSplitCheckItemThemeKeyExtension() { }
	}
	public class BarEditItemThemeKeyExtension : ThemeKeyExtensionBase<BarEditItemThemeKeys> {
		public BarEditItemThemeKeyExtension() { }
	}
	public class BarStaticItemThemeKeyExtension : ThemeKeyExtensionBase<BarStaticItemThemeKeys> {
		public BarStaticItemThemeKeyExtension() { }
	}
	public class BarItemSeparatorThemeKeyExtension : ThemeKeyExtensionBase<BarItemSeparatorThemeKeys> {
		public BarItemSeparatorThemeKeyExtension() { }
	}
	public class BarItemTouchSplitterThemeKeyExtension : ThemeKeyExtensionBase<BarItemTouchSplitterThemeKeys> {
		public BarItemTouchSplitterThemeKeyExtension() { }
	}
	public class BarItemLinkMenuHeaderThemeKeyExtension: ThemeKeyExtensionBase<BarItemLinkMenuHeaderThemeKeys> {
		public BarItemLinkMenuHeaderThemeKeyExtension() { }
	}
	public class GlyphSideControlThemeKeyExtension: ThemeKeyExtensionBase<GlyphSideControlThemeKeys> {
		public GlyphSideControlThemeKeyExtension() { }
	}
	public class BarItemThemeKeyExtension : ThemeKeyExtensionBase<BarItemThemeKeys> {
		public BarItemThemeKeyExtension() { }
	}
	public enum BarItemBorderThemeKeys { Normal, Hover, Pressed, Customization }
	public class BarItemBorderThemeKeyExtension : ThemeKeyExtensionBase<BarItemBorderThemeKeys> {
		public BarItemBorderThemeKeyExtension() {  }
	}
	public class BarItemBorderInMenuThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public BarItemBorderInMenuThemeKeyExtension() {  }
	}
	public class BarItemBorderInMainMenuThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public BarItemBorderInMainMenuThemeKeyExtension() {  }
	}
	public class BarItemBorderInStatusBarThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public BarItemBorderInStatusBarThemeKeyExtension() {  }
	}
	public class BarItemBorderInRibbonPageGroupThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public BarItemBorderInRibbonPageGroupThemeKeyExtension() {  }
	}
	public class BarItemBorderInButtonGroupThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public BarItemBorderInButtonGroupThemeKeyExtension() {  }
	}
	public class BarItemBorderInRibbonStatusBarLeftThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public BarItemBorderInRibbonStatusBarLeftThemeKeyExtension() {  }
	}
	public class BarItemBorderInRibbonStatusBarRightThemeKeyExtension : BarItemBorderThemeKeyExtension {
		public BarItemBorderInRibbonStatusBarRightThemeKeyExtension() {  }
	}
	public enum BarItemFontKeys {
		BarEditItemFontSettingsInButtonGroup,
		BarEditItemEditStyle,
		BarEditItemEditStyleInButtonGroup,
		BarEditItemEditStyleInMainMenu,
		BarEditItemEditStyleInMenu,
		BarEditItemEditStyleInStatusBar,
		BarEditItemFontSettingsInMenu,
		BarEditItemEditStyleInRibbonPageGroup,
		BarItemFontSettings,
		BarItemFontSettingsInButtonGroup,
		BarItemFontSettingsInMainMenu,
		BarItemFontSettingsInMenu,
		BarItemFontSettingsInApplicationMenu,
		BarItemFontSettingsInRibbonPageGroup,
		BarItemFontSettingsInRibbonStatusBarLeft,
		BarItemFontSettingsInRibbonStatusBarRight,
		BarItemFontSettingsInRibbonToolbar,
		BarItemFontSettingsInRibbonToolbarFooter,
		BarItemFontSettingsInRibbonPageHeader,
		BarItemFontSettingsInStatusBar,
		BarItemDescriptionStyleInApplicationMenu,
		BarButtonItemTextStyleInTextSplitter,
		BarSubItemTextStyleInTextSplitter,
		BarSplitButtonItemTextStyleInTextSplitter,
		BarCheckItemTextStyleInTextSplitter
	}
	public class BarItemFontThemeKeyExtension : ThemeKeyExtensionBase<BarItemFontKeys> {
		public BarItemFontThemeKeyExtension() { }
		public BarItemFontThemeKeyExtension(BarItemFontKeys key) { ResourceKey = key; }
	}
	public enum BarItemImageColorizerSettingsKeys {
		SettingsInMainMenu,
		SettingsInStatusBar,
		SettingsInMenu,
		SettingsInRibbonToolbar,
		SettingsInRibbonPageGroup,
		SettingsInButtonGroup,
		SettingsInRibbonPageHeader,
		SettingsInRibbonStatusBarLeft,
		SettingsInRibbonStatusBarRight,
		SettingsInApplicationMenu,
		SettingsInRibbonToolbarFooter,
		DefaultSettings
	}
	public class BarItemImageColorizerSettingsThemeKeyExtension : ThemeKeyExtensionBase<BarItemImageColorizerSettingsKeys> {
		public BarItemImageColorizerSettingsThemeKeyExtension() { }
		public BarItemImageColorizerSettingsThemeKeyExtension(BarItemImageColorizerSettingsKeys key) { ResourceKey = key; }
	}
	public enum TextSplitterThemeKeys {
		BarButtonItem,
		BarCheckItem,
		BarSubItem,
		BarSplitButtonItem,
		BarSplitCheckItem
	}
	public class TextSplitterThemeKeyExtension : ThemeKeyExtensionBase<TextSplitterThemeKeys> {
		public TextSplitterThemeKeyExtension() { }
	}
}
