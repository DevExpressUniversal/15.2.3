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

using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Docking.Platform;
namespace DevExpress.Xpf.Docking.ThemeKeys {
	public enum DockElements {
		SplitterTemplate,
		CaptionControl,
		CustomizationControl,
		ButtonsPanel,
		TreeView,
		HiddenItemsPanel,
		DragCursor,
		DropBoundsControl,
		HiddenItem,
		HiddenItemsPanelItem,
		ElementSizer
	}
	public enum OtherResoucesEnum {
		CustomizationTreeViewTemplate, HiddenItemTemplate
	}
	public class OtherResourcesThemeKeyExtension :
		ThemeKeyExtensionBase<OtherResoucesEnum> {
	}
	public class DockElementsThemeKeyExtension :
		ThemeKeyExtensionBase<DockElements> {
	}
	public enum ControlItemElements {
		Template, Control, Caption
	}
	public class ControlItemElementsThemeKeyExtension :
		ThemeKeyExtensionBase<ControlItemElements> {
	}
	public enum FixedItemElements {
		Template, EmptySpaceTemplate, LabelTemplate, SeparatorHorizontalTemplate, SeparatorVerticalTemplate
	}
	public class FixedItemElementsThemeKeyExtension :
		ThemeKeyExtensionBase<FixedItemElements> {
	}
	public enum SplitterControlElements {
		Template,
	}
	public class SplitterControlElementsThemeKeyExtension :
		ThemeKeyExtensionBase<SplitterControlElements> {
	}
	public enum EmptySpaceControlElements {
		Template,
	}
	public class EmptySpaceControlElementsThemeKeyExtension :
		ThemeKeyExtensionBase<EmptySpaceControlElements> {
	}
	public enum LabelControlElements {
		Template, Caption, Content
	}
	public class LabelControlElementsThemeKeyExtension :
		ThemeKeyExtensionBase<LabelControlElements> {
	}
	public enum SeparatorControlElements {
		Template,
	}
	public class SeparatorControlElementsThemeKeyExtension :
		ThemeKeyExtensionBase<SeparatorControlElements> {
	}
	public enum DockPaneElements {
		Template, Content, Header, CloseButton, PinButton, MaximizeButton, RestoreButton, HideButton, ExpandButton, CollapseButton,
		ContentPreview,
		ControlHostTemplate, LayoutHostTemplate, DataHostTemplate,
		HeaderMargin,
		BorderBrush, Background, CaptionCornerRadius, FloatingCaptionCornerRadius,
		CaptionActiveBackground, CaptionNormalBackground, CaptionActiveForeground, CaptionNormalForeground,
		BorderThickness, BorderMargin, BorderPadding, BarContainerMargin, ContentMargin
	}
	public class DockPaneElementsThemeKeyExtension :
		ThemeKeyExtensionBase<DockPaneElements> {
	}
	public enum GroupPaneElements {
		Template, Content, NoBorderTemplate, GroupTemplate, GroupBoxTemplate, TabbedTemplate, ExpandButton
	}
	public class GroupPaneElementsThemeKeyExtension :
		ThemeKeyExtensionBase<GroupPaneElements> {
	}
	public enum TabbedLayoutGroupPaneElements {
		Template, Caption, Content, PageHeader,
	}
	public class TabbedLayoutGroupPaneElementsThemeKeyExtension :
		ThemeKeyExtensionBase<TabbedLayoutGroupPaneElements> {
	}
	public enum TabbedPaneElements {
		Template,
		TabbedTemplate,
		TabContainerTemplate, Content, PageHeader,
	}
	public class TabbedPaneElementsThemeKeyExtension :
		ThemeKeyExtensionBase<TabbedPaneElements> {
	}
	public enum DocumentElements {
		Template, Content, MinimizeButton, MaximizeButton, RestoreButton, CloseButton, ContentPreview, ClosePageButton, PinPageButton,
		ControlHostTemplate, LayoutHostTemplate, DataHostTemplate,
		FloatDocument,
	}
	public class DocumentElementsThemeKeyExtension :
		ThemeKeyExtensionBase<DocumentElements> {
	}
	public enum DocumentPaneElements {
		Template, 
		TabbedTemplate, MDITemplate,
		TabContainerTemplate, Content, PageHeader,
		MDIContainerTemplate, MDIDocument,
		ScrollPrevButton, ScrollNextButton, DropDownButton, RestoreButton, CloseButton, MDICloseButton,
		TabHeadersClipMargin, TabHeadersTransparencySize,
		TabbedBackground, MDIBackground
	}
	public class DocumentPaneElementsThemeKeyExtension :
		ThemeKeyExtensionBase<DocumentPaneElements> {
	}
	public enum AutoHideTrayElements {
		Template, HeadersGroup, Caption,
	}
	public class AutoHideTrayElementsThemeKeyExtension :
		ThemeKeyExtensionBase<AutoHideTrayElements> {
	}
	public enum AutoHideTrayHeadersGroupElements {
		LeftMargin, TopMargin, RightMargin, BottomMargin
	}
	public class AutoHideTrayHeadersGroupElementsThemeKeyExtension :
		ThemeKeyExtensionBase<AutoHideTrayHeadersGroupElements> {
	}
	public enum AutoHidePaneElements {
		Template, Content, SizerThickness, TouchSizerThickness
	}
	public class AutoHidePaneElementsThemeKeyExtension :
		ThemeKeyExtensionBase<AutoHidePaneElements> {
	}
	public enum FloatPaneElements {
		Template, CloseButton, MaximizeButton, RestoreButton,
		FormBorderMargin, SingleBorderMargin, MaximizedBorderMargin, ShadowMargin, BorderMargin,
		SingleBorderTemplate, FormBorderTemplate, EmptyBorderTemplate,
		FormBorderContentTemplate,
		ResizingSideLength, ResizingCornerLength
	}
	public class FloatPaneElementsThemeKeyExtension :
		ThemeKeyExtensionBase<FloatPaneElements> {
	}
	public enum BrushElements {
		PanelForeground, PanelBackground, CaptionActive, CaptionInactive, TreeItemSelected, TreeItemHovered, TreeItemDragged, HiddenItem,
		HiddenItemBorder, HiddenItemCaption, MoveTargetBackground
	}
	public class BrushElementsThemeKeyExtension :
		ThemeKeyExtensionBase<BrushElements> {
	}
	public enum GlyphElements {
		MDIButtonMinimize, MDIButtonRestore, MDIButtonClose
	}
	public class GlyphElementsThemeKeyExtension :
		ThemeKeyExtensionBase<GlyphElements> {
	}
	public enum MDIButtonElements {
		ButtonBorder, ButtonBorderStyle
	}
	public class MDIButtonElementsThemeKeyExtension :
		ThemeKeyExtensionBase<MDIButtonElements> {
	}
}
