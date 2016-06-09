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
namespace DevExpress.Xpf.Ribbon.Themes {
	public enum RibbonThemeKeys {
		ControlTemplate,
		ItemsPanelTemplate,
		RibbonPanelBorderTemplate,
		FooterQuickAccessToolbarContainerStyle,
		HeaderQuickAccessToolbarContainerStyle,
		HeaderQuickAccessToolbar2007ContainerStyle,
		HeaderQuickAccessToolbar2010ContainerStyle,
		HeaderQuickAccessToolbar2007ContainerStyleInRibbonWindow,
		HeaderQuickAccessToolbar2010ContainerStyleInRibbonWindow,
		HeaderQuickAccessToolbarContainerStyleWithoutApplicationIcon,
		FooterQuickAccessToolbarContainerTemplate,
		PopupBorderTemplate,
		BackgroundTemplate,
		CollapsedSelectedPageBorderTemplate,
		ApplicationMenuBarControlBorderTemplate2010,
		ApplicationMenuBarControlBorderTemplate2007,
		ApplicationMenuPopupBorderTemplate,
		ApplicationMenuBarControlTemplate,
		ApplicationMenuBackgroundTemplate,
		ApplicationMenuVerticalOffset,
		ApplicationMenuHorizontalOffset,
		ApplicationMenuBottomPaneTemplate,
		ApplicationMenuRightPaneTemplate,
		ApplicationMenuRightPaneForeground,
		ApplicationMenuContentControlTemplate,
		ApplicationMenuContentControlBorderTemplate,
		KeyTipControlTemplate,
		LeftRepeatButtonStyle,
		RightRepeatButtonStyle,
		LeftRepeatButtonTemplate,
		RightRepeatButtonTemplate,
		SelectedPageBorderTemplate,
		SelectedPageBorderTemplateInPopup,
		SelectedPagePopupMargin,
		StandaloneHeaderBorderTemplate,
		HeaderBorderTemplateInRibbonWindow,
		AeroTemplate,
		HeaderBorderTemplateInRibbonAeroWindow,
		BackgroundTemplateInRibbonAeroWindow,
		TabPartBorderTemplateInRibbonAeroWindow,
		LeftRepeatButtonStyleInRibbonAeroWindow,
		RightRepeatButtonStyleInRibbonAeroWindow,
		LeftRepeatButtonTemplateInRibbonAeroWindow,
		RightRepeatButtonTemplateInRibbonAeroWindow,
		MinimizationButtonStyleInRibbonAeroWindow,
		MinimizationButtonTemplateInRibbonAeroWindow,
		CaptionStyleInRibbonAeroWindow,
		ApplicationIconContainerStyleInRibbonAeroWindow,
		IconAndCaptionAreaStyleInRibbonAeroWindow,
		HeaderQuickAccessToolbarContainerStyleWithoutAppIconInAeroWindow,
		HeaderQuickAccessToolbar2007ContainerStyleInAeroWindow,
		HeaderQuickAccessToolbar2010ContainerStyleInAeroWindow,
		NormalPageCategoryTextStyleInAeroWindow,
		SelectedPageCategoryTextStyleInAeroWindow,
		AeroBorderRibbonStyle,
		AeroBorderTopOffset,
		CollapsedRibbonAeroBorderTopOffset,
		CollapsedSelectedPageBorderTemplateInAeroWindow,
		HeaderAndTabsBorderTemplatedInAeroWindow,
		ApplicationMenuVerticalOffsetInAeroWindow,
		ApplicationMenuHorizontalOffsetInAeroWindow,
		TabPartBorderTemplate,
		NormalPageCategoryTextStyle,
		SelectedPageCategoryTextStyle,
		HoverPageCaptionTextStyle,
		NormalPageCaptionTextStyle,
		SelectedPageCaptionTextStyle,
		GroupCaptionTextStyle,
		MaxPageCaptionTextIndent,
		RowIndent,
		ColumnIndent,
		DefaultRightPaneWidth,
		HighlightedSelectedPageBorderTemplateInPopup,
		HighlightedSelectedPageBorderTemplate,
		ApplicationIconContainerStyle,
		CaptionStyle,
		IconAndCaptionAreaStyle,
		ShowAutoHidePopupButtonStyle,
		ShowAutoHidePopupButtonTemplate,
		RibbonAutoHidePopupStyle,
		RibbonShowModeSelectorItemTemplate,
		RibbonShowModeSelectorItemStyle,
		FloatingContainerTemplate,
		FloatingContainerHeaderBorderTemplate,
		FloatingContainerCaptionStyle,
		FloatingContainerBorderTemplate,
		FloatingContainerControlBoxStyle,
		FloatingContainerControlBoxTemplate,
		FloatingContainerMinimizeButtonStyle,
		FloatingContainerRestoreButtonStyle,
		FloatingContainerCloseButtonStyle,
		FloatingContainerMaximizeButtonStyle,
		FloatingContainerContentTemplate,
		FloatingContainerContentStyle,
		FloatingContainerBodyTemplate,
		FloatingContainerBodyStyle,
		FloatingContainerIconStyle,
		FloatingContainerIconAndCaptionAreaStyle,
		PagesControlTemplate,
		PagesControlItemsPanelTemplate,
		PageGroupsControlTemplate,
		PageGroupsControlItemsPanelTemplate,
		MinimizationButtonStyle,
		MinimizationButtonTemplate,
		ResizeBorderThickness,
		HeaderMinHeight,
		AeroHeaderMinHeight,
		HeaderMinHeightTouch,
		AeroHeaderMinHeightTouch,
	}
	public class RibbonThemeKeyExtension : ThemeKeyExtensionBase<RibbonThemeKeys> { }
	public enum KeyTipControlThemeKeys {
		Template,
		ToolTipTemplate,
		BorderTemplate,
		ContentStyle
	}
	public class KeyTipControlThemeKeyExtension : ThemeKeyExtensionBase<KeyTipControlThemeKeys> {
		public KeyTipControlThemeKeyExtension() { }
	}
	public enum RibbonCustomizationThemeKeys {
		UpArrowButtonStyle,
		UpArrowThemeKey,
		DownArrowThemeKey,
		CustomizationArrowControlTemplate,
		CustomizationSeparatorControlTemplate
	}
	public class RibbonCustomizationThemeKeyExtension : ThemeKeyExtensionBase<RibbonCustomizationThemeKeys> {
		public RibbonCustomizationThemeKeyExtension() { }
	}
}
