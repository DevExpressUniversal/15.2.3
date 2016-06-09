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
namespace DevExpress.Xpf.WindowsUI.ThemeKeys {
	public abstract class WindowsUIElementsThemeKeyExtensionBase<T> :
		ThemeKeyExtensionBase<T> {
		public WindowsUIElementsThemeKeyExtensionBase()
			: base() {
		}
	}
	public enum WindowsUIElements {
		BackButtonTemplate, ForwardButtonTemplate,
		FlipViewTemplate, FlipViewItemTemplate, FadingButtonTemplate,
		DXFrameTemplate,
		PageAdornerControlTemplate,
		NavigationErrorPageTemplate,
		FlyoutCommandButtonTemplate,
		ScrollableControlButtonTemplate,
		PopupWindowTemplate
	}
	public enum PageViewElements {
		Background, Foreground,
		Template, ItemTemplate, ItemsPanelHorizontal, ItemsPanelVertical, ItemTemplateSelector, ItemsPanel,
		TemplateLeft, TemplateRight, TemplateBottom,
		HeaderMargin,
		HeaderButtonTemplate
	}
	public enum SlideViewElements {
		Background,
		Template, ItemTemplate, ItemHeaderTemplate,
		InteractiveHeader,
	}
	public enum CommandButtonElements {
		Template, BackGlyph, ForwardGlyph,
		HoverBorderBrush, HoverBackBrush, HoverForeColor,
		PressedBorderBrush, PressedBackBrush, PressedForeColor,
		NormalBorderBrush, NormalBackBrush, NormalForeColor,
	}
	public enum AppBarElements {
		Background, AppBarSeparatorTemplate,
		AppBarButtonTemplate, AppBarButtonForeground, AppBarButtonForegroundColor,
		AppBarTileButtonTemplate, AppBarTileBorderButtonTemplate, AppBarTileDropDownButtonTemplate, AppBarTileButtonBackground, AppBarButtonDefaultWidth
	}
	public enum MessageBoxElements {
		Background, WindowBackground
	}
	public enum NavigationBarElements {
		ItemTemplate, ButtonTemplate,
		ItemHoverForegroundColor, ItemSelectedForegroundColor,
		CustomizationButtonContentTemplate,
		CustomizationArrowControlTemplate, CustomizationSeparatorControlTemplate,
		CustomizationFormFloatSize, CustomizationFormMinWidth, CustomizationFormMinHeight
	}
	public enum TileNavPaneElements {
		TileNavButtonTemplate, TileSelectorItemTemplate, TileBarArrowTemplate, TileButtonControlTemplate, TileNavButtonSeparatorTemplate,
		TileNavPaneBackground, TileNavPaneFlyoutBackground, TileSelectorItemBackground, TileSelectorItemSelection, TileSelectorItemForeground,
		TileNavButtonForegroundColor, TileNavButtonForeground, TileSelectorItemForegroundColor, TileNavButtonDropDownButtonMargin
	}
	public enum MenuFlyoutElements {
		Padding, FontSize, Background, SelectedBackground, Foreground, SelectedForeground, PressedForeground, SelectionPadding, PressedBackground,
		MenuFlyoutSeparatorTemplate, MenuFlyoutSeparatorForeground,  
	}
	public class WindowsUIElementsThemeKeyExtension :
		WindowsUIElementsThemeKeyExtensionBase<WindowsUIElements> {
		public WindowsUIElementsThemeKeyExtension() : base() { }
	}
	public class PageViewElementsThemeKeyExtension :
		WindowsUIElementsThemeKeyExtensionBase<PageViewElements> {
		public PageViewElementsThemeKeyExtension() : base() { }
	}
	public class SlideViewElementsThemeKeyExtension :
		WindowsUIElementsThemeKeyExtensionBase<SlideViewElements> {
		public SlideViewElementsThemeKeyExtension() : base() { }
	}
	public class CommandButtonElementsThemeKeyExtension :
		WindowsUIElementsThemeKeyExtensionBase<CommandButtonElements> {
		public CommandButtonElementsThemeKeyExtension() : base() { }
	}
	public class AppBarElementsThemeKeyExtension :
		WindowsUIElementsThemeKeyExtensionBase<AppBarElements> {
		public AppBarElementsThemeKeyExtension() : base() { }
	}
	public class MessageBoxElementsThemeKeyExtension :
	   WindowsUIElementsThemeKeyExtensionBase<MessageBoxElements> {
		public MessageBoxElementsThemeKeyExtension() : base() { }
	}
	public class NavigationBarElementsThemeKeyExtension :
		WindowsUIElementsThemeKeyExtensionBase<NavigationBarElements> {
		public NavigationBarElementsThemeKeyExtension()
			: base() {
		}
	}
	public class TileNavPaneElementsThemeKeyExtension :
		WindowsUIElementsThemeKeyExtensionBase<TileNavPaneElements> {
		public TileNavPaneElementsThemeKeyExtension() : base() { }
	}
	public class MenuFlyoutElementsThemeKeyExtension :
	   WindowsUIElementsThemeKeyExtensionBase<MenuFlyoutElements> {
		public MenuFlyoutElementsThemeKeyExtension() : base() { }
	}
}
