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
using System.Resources;
using System.Windows.Markup;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Docking.Base {
	public enum DockingStringId {
		MenuItemShowCaption,
		MenuItemShowControl,
		MenuItemCaptionImageLocation,
		MenuItemBeforeText,
		MenuItemAfterText,
		MenuItemCaptionLocation,
		MenuItemLeft,
		MenuItemRight,
		MenuItemTop,
		MenuItemBottom,
		MenuItemHideCustomizationWindow,
		MenuItemShowCustomizationWindow,
		MenuItemBeginCustomization,
		MenuItemEndCustomization,
		MenuItemDock,
		MenuItemFloat,
		MenuItemAutoHide,
		MenuItemHide,
		MenuItemClose,
		MenuItemClosedPanels,
		MenuItemExpandGroup,
		MenuItemCollapseGroup,
		MenuItemHideItem,
		MenuItemRestoreItem,
		MenuItemGroupItems,
		MenuItemGroupOrientation,
		MenuItemHorizontal,
		MenuItemVertical,
		MenuItemRename,
		MenuItemShowCaptionImage,
		MenuItemStyle,
		MenuItemStyleNoBorder,
		MenuItemStyleGroup,
		MenuItemStyleGroupBox,
		MenuItemStyleTabbed,
		MenuItemUngroup,
		MenuItemHorizontalAlignment,
		MenuItemHorizontalAlignmentLeft,
		MenuItemHorizontalAlignmentRight,
		MenuItemHorizontalAlignmentCenter,
		MenuItemHorizontalAlignmentStretch,
		MenuItemVerticalAlignment,
		MenuItemVerticalAlignmentTop,
		MenuItemVerticalAlignmentBottom,
		MenuItemVerticalAlignmentCenter,
		MenuItemVerticalAlignmentStretch,
		MenuItemNewHorizontalTabGroup,
		MenuItemNewVerticalTabGroup,
		MenuItemMoveToPreviousTabGroup,
		MenuItemMoveToNextTabGroup,
		MenuItemCloseAllButThis,
		TitleCustomizationForm,
		TitleHiddenItemsList,
		TitleLayoutTreeView,
		ButtonSave,
		ButtonRestore,
		CheckBoxShowInvisibleItems,
		LayoutPanelCaptionFormat,
		LayoutGroupCaptionFormat,
		LayoutControlItemCaptionFormat,
		TabCaptionFormat,
		WindowTitleFormat,
		DefaultLabelContent,
		DefaultEmptySpaceContent,
		DefaultSeparatorContent,
		DefaultSplitterContent,
		NewGroupCaption,
		ControlButtonClose,
		ControlButtonAutoHide,
		ControlButtonMinimize,
		ControlButtonMaximize,
		ControlButtonRestore,
		ControlButtonScrollNext,
		ControlButtonScrollPrev,
		ControlButtonHide,
		ControlButtonExpand,
		ControlButtonCollapse,
		ControlButtonTogglePinStatus,
		DocumentSelectorPanels,
		DocumentSelectorDocuments,
		ClosedPanelsCategory,
		DTLoadLayoutWarning,
		DTLoadLayoutWarningCaption,
		DTEmptyPanelText,
		DTEmptyGroupText,
		DTLayoutControlItemCaption,
		DTLayoutPanelCaption,
		DTDocumentPanelCaption,
		MenuItemAddPanel,
		MenuItemRemovePanel,
		MenuItemHidePanel,
		MenuItemAddDocument,
		MenuItemRemoveDocument,
		MenuItemHideDocument,
		MenuItemCreateDefaultLayout,
		MenuItemDTCaptionLocation,
		MenuItemDTGroupStyle,
		MenuItemDTGroupOrientation,
		MenuItemCaptionHorizontalAlignment,
		MenuItemCaptionVerticalAlignment,
		MenuItemControlHorizontalAlignment,
		MenuItemControlVerticalAlignment,
		MenuItemResetLayout,
		MenuItemRemoveItem,
		MenuItemRemoveAll,
		MenuItemContentHorizontalAlignment,
		MenuItemContentVerticalAlignment,
		MenuItemResetCustomization,
		MenuItemMDIStyle,
		ReplaceDialogTitle,
		ReplaceDialogText,
		ShowCustomizationPanel,
		NoItemSelected,
		ToolTipEditCaption,
		ToolTipDeleteItem,
		ToolTipChangeItemType,
		ToolTipCreateItem,
		DockingOperations,
		FloatingOperations,
		AutoHideOperations,
		CloseOperations,
		LoadLayoutOperation,
		ResetLayoutOperation,
		ButtonNewLayoutItemFormat,
		ButtonCreateLayoutItem,
		ButtonCreateLayoutItemDetails,
		ButtonReplaceControl,
		ButtonReplaceControlDetails,
		ButtonDoNothing,
		ButtonDoNothingDetails,
		CheckBoxAskNextTime,
	}
	public class DockingLocalizer : DXLocalizer<DockingStringId> {
		static DockingLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DockingStringId>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<DockingStringId> CreateDefaultLocalizer() {
			return new DockingResXLocalizer();
		}
		public static string GetString(DockingStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<DockingStringId> CreateResXLocalizer() {
			return new DockingResXLocalizer();
		}
		protected override void PopulateStringTable() {
			AddString(DockingStringId.MenuItemShowCaption, "Show caption");
			AddString(DockingStringId.MenuItemShowControl, "Show control");
			AddString(DockingStringId.MenuItemCaptionImageLocation, "Caption image location");
			AddString(DockingStringId.MenuItemBeforeText, "Before text");
			AddString(DockingStringId.MenuItemAfterText, "After text");
			AddString(DockingStringId.MenuItemCaptionLocation, "Caption location");
			AddString(DockingStringId.MenuItemLeft, "Left");
			AddString(DockingStringId.MenuItemRight, "Right");
			AddString(DockingStringId.MenuItemTop, "Top");
			AddString(DockingStringId.MenuItemBottom, "Bottom");
			AddString(DockingStringId.MenuItemHideCustomizationWindow, "Hide customization window");
			AddString(DockingStringId.MenuItemShowCustomizationWindow, "Show customization window");
			AddString(DockingStringId.MenuItemBeginCustomization, "Begin customization");
			AddString(DockingStringId.MenuItemEndCustomization, "End customization");
			AddString(DockingStringId.MenuItemDock, "Dock");
			AddString(DockingStringId.MenuItemFloat, "Float");
			AddString(DockingStringId.MenuItemAutoHide, "Auto Hide");
			AddString(DockingStringId.MenuItemHide, "Hide");
			AddString(DockingStringId.MenuItemClose, "Close");
			AddString(DockingStringId.MenuItemClosedPanels, "Closed panels");
			AddString(DockingStringId.MenuItemExpandGroup, "Expand");
			AddString(DockingStringId.MenuItemCollapseGroup, "Collapse");
			AddString(DockingStringId.MenuItemHideItem, "Hide item");
			AddString(DockingStringId.MenuItemRestoreItem, "Restore item");
			AddString(DockingStringId.MenuItemGroupItems, "Group");
			AddString(DockingStringId.MenuItemGroupOrientation, "Group orientation");
			AddString(DockingStringId.MenuItemHorizontal, "Horizontal");
			AddString(DockingStringId.MenuItemVertical, "Vertical");
			AddString(DockingStringId.MenuItemRename, "Rename");
			AddString(DockingStringId.MenuItemShowCaptionImage, "Show caption image");
			AddString(DockingStringId.MenuItemStyle, "Group style");
			AddString(DockingStringId.MenuItemStyleNoBorder, "No border");
			AddString(DockingStringId.MenuItemStyleGroup, "Group");
			AddString(DockingStringId.MenuItemStyleGroupBox, "GroupBox");
			AddString(DockingStringId.MenuItemStyleTabbed, "Tabbed");
			AddString(DockingStringId.MenuItemUngroup, "Ungroup");
			AddString(DockingStringId.MenuItemHorizontalAlignment, "Horizontal alignment");
			AddString(DockingStringId.MenuItemHorizontalAlignmentLeft, "Left");
			AddString(DockingStringId.MenuItemHorizontalAlignmentRight, "Right");
			AddString(DockingStringId.MenuItemHorizontalAlignmentCenter, "Center");
			AddString(DockingStringId.MenuItemHorizontalAlignmentStretch, "Stretch");
			AddString(DockingStringId.MenuItemVerticalAlignment, "Vertical alignment");
			AddString(DockingStringId.MenuItemVerticalAlignmentTop, "Top");
			AddString(DockingStringId.MenuItemVerticalAlignmentBottom, "Bottom");
			AddString(DockingStringId.MenuItemVerticalAlignmentCenter, "Center");
			AddString(DockingStringId.MenuItemVerticalAlignmentStretch, "Stretch");
			AddString(DockingStringId.MenuItemNewHorizontalTabGroup, "New horizontal tab group");
			AddString(DockingStringId.MenuItemNewVerticalTabGroup, "New vertical tab group");
			AddString(DockingStringId.MenuItemMoveToPreviousTabGroup, "Move to previous tab group");
			AddString(DockingStringId.MenuItemMoveToNextTabGroup, "Move to next tab group");
			AddString(DockingStringId.MenuItemCloseAllButThis, "Close all but this");
			AddString(DockingStringId.TitleCustomizationForm, "Customization");
			AddString(DockingStringId.TitleHiddenItemsList, "Hidden Items");
			AddString(DockingStringId.TitleLayoutTreeView, "Layout Tree");
			AddString(DockingStringId.CheckBoxShowInvisibleItems, "Show invisible items");
			AddString(DockingStringId.ButtonSave, "Save");
			AddString(DockingStringId.ButtonRestore, "Restore");
			AddString(DockingStringId.LayoutPanelCaptionFormat, "{0}");
			AddString(DockingStringId.LayoutGroupCaptionFormat, "{0}");
			AddString(DockingStringId.LayoutControlItemCaptionFormat, "{0}:");
			AddString(DockingStringId.TabCaptionFormat, "{0}");
			AddString(DockingStringId.WindowTitleFormat, "{0} - [{1}]");
			AddString(DockingStringId.DefaultLabelContent, "Label");
			AddString(DockingStringId.DefaultEmptySpaceContent, "Empty Space Item");
			AddString(DockingStringId.DefaultSeparatorContent, "Separator");
			AddString(DockingStringId.DefaultSplitterContent, "Splitter");
			AddString(DockingStringId.NewGroupCaption, "Group");
			AddString(DockingStringId.ControlButtonClose, "Close");
			AddString(DockingStringId.ControlButtonAutoHide, "Auto Hide");
			AddString(DockingStringId.ControlButtonTogglePinStatus, "Toggle pin status");
			AddString(DockingStringId.ControlButtonMinimize, "Minimize");
			AddString(DockingStringId.ControlButtonMaximize, "Maximize");
			AddString(DockingStringId.ControlButtonRestore, "Restore");
			AddString(DockingStringId.ControlButtonScrollNext, "Scroll next");
			AddString(DockingStringId.ControlButtonScrollPrev, "Scroll previous");
			AddString(DockingStringId.ControlButtonHide, "Hide");
			AddString(DockingStringId.ControlButtonExpand, "Expand");
			AddString(DockingStringId.ControlButtonCollapse, "Collapse");
			AddString(DockingStringId.DocumentSelectorPanels, "Active Panels");
			AddString(DockingStringId.DocumentSelectorDocuments, "Active Documents");
			AddString(DockingStringId.ClosedPanelsCategory, "Closed Panels");
			AddString(DockingStringId.DTLoadLayoutWarning, "The current layout will be cleared. Do you want to continue?");
			AddString(DockingStringId.DTLoadLayoutWarningCaption, "Loading Layout");
			AddString(DockingStringId.DTEmptyPanelText, "Drag and drop controls here to build your layout.");
			AddString(DockingStringId.DTEmptyGroupText, "Right-click here to create panels via a context menu.");
			AddString(DockingStringId.DTLayoutControlItemCaption, "Layout Item");
			AddString(DockingStringId.DTDocumentPanelCaption, "Document");
			AddString(DockingStringId.DTLayoutPanelCaption, "Panel");
			AddString(DockingStringId.MenuItemAddPanel, "Add Panel");
			AddString(DockingStringId.MenuItemRemovePanel, "Remove Panel");
			AddString(DockingStringId.MenuItemHidePanel, "Close Panel");
			AddString(DockingStringId.MenuItemAddDocument, "Add Document");
			AddString(DockingStringId.MenuItemRemoveDocument, "Remove Document");
			AddString(DockingStringId.MenuItemHideDocument, "Close Document");
			AddString(DockingStringId.MenuItemCreateDefaultLayout, "Create Default Layout");
			AddString(DockingStringId.MenuItemDTCaptionLocation, "Caption Location");
			AddString(DockingStringId.MenuItemDTGroupStyle, "Group Style");
			AddString(DockingStringId.MenuItemDTGroupOrientation, "Group Orientation");
			AddString(DockingStringId.MenuItemCaptionHorizontalAlignment, "Caption Horizontal Alignment");
			AddString(DockingStringId.MenuItemCaptionVerticalAlignment, "Caption Vertical Alignment");
			AddString(DockingStringId.MenuItemControlHorizontalAlignment, "Control Horizontal Alignment");
			AddString(DockingStringId.MenuItemControlVerticalAlignment, "Control Vertical Alignment");
			AddString(DockingStringId.MenuItemResetLayout, "Reset Layout");
			AddString(DockingStringId.MenuItemRemoveItem, "Remove Item");
			AddString(DockingStringId.MenuItemRemoveAll, "Clear");
			AddString(DockingStringId.MenuItemContentHorizontalAlignment, "Content Horizontal Alignment");
			AddString(DockingStringId.MenuItemContentVerticalAlignment, "Content Vertical Alignment");
			AddString(DockingStringId.ReplaceDialogTitle, "Add new control");
			AddString(DockingStringId.ReplaceDialogText, "Choose an action:");
			AddString(DockingStringId.ShowCustomizationPanel, "Show Customization Panel");
			AddString(DockingStringId.NoItemSelected, "(No item selected)");
			AddString(DockingStringId.ToolTipEditCaption, "Click here to edit item's caption");
			AddString(DockingStringId.ToolTipDeleteItem, "Click here to delete selected item");
			AddString(DockingStringId.ToolTipChangeItemType, "Click here to change item type");
			AddString(DockingStringId.ToolTipCreateItem, "Click here to create new item");
			AddString(DockingStringId.DockingOperations, "Docking operations");
			AddString(DockingStringId.FloatingOperations, "Floating operations");
			AddString(DockingStringId.AutoHideOperations, "Auto-hide operations");
			AddString(DockingStringId.CloseOperations, "Close operations");
			AddString(DockingStringId.LoadLayoutOperation, "Load layout");
			AddString(DockingStringId.ResetLayoutOperation, "Reset layout");
			AddString(DockingStringId.ButtonNewLayoutItemFormat, "New ({0})");
			AddString(DockingStringId.ButtonCreateLayoutItem, "Append the control to the panel");
			AddString(DockingStringId.ButtonCreateLayoutItemDetails, "Choosing the option creates a new LayoutGroup and assigns it to the panel's Content. "+
				"The new and existing controls will be moved to the created LayoutGroup.");
			AddString(DockingStringId.ButtonReplaceControl, "Replace the existing control");
			AddString(DockingStringId.ButtonReplaceControlDetails, "Choose this option to replace the existing control with the new one.");
			AddString(DockingStringId.ButtonDoNothing, "Cancel");
			AddString(DockingStringId.ButtonDoNothingDetails, "Do not add the control to the panel");
			AddString(DockingStringId.CheckBoxAskNextTime, "Ask next time");
			AddString(DockingStringId.MenuItemResetCustomization, "Reset Customization Settings");
			AddString(DockingStringId.MenuItemMDIStyle, "MDI Style");
		}
	}
	public class DockingResXLocalizer : DXResXLocalizer<DockingStringId> {
		public DockingResXLocalizer()
			: base(new DockingLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Docking.LocalizationRes", typeof(DockingResXLocalizer).Assembly);
		}
	}
	public class DockingStringIdExtension : MarkupExtension {
		public DockingStringId StringId { get; set; }
		public DockingStringIdExtension(DockingStringId stringId) {
			StringId = stringId;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return DockingLocalizer.GetString(StringId);
		}
	} 
}
