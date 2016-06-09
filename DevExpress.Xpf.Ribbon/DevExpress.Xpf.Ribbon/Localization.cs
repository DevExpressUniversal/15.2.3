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

using DevExpress.Xpf.Core;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Localization;
using System.Resources;
using System;
using System.Windows.Markup;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
namespace DevExpress.Xpf.Ribbon {
	public enum RibbonControlStringId {
		RibbonPopupMenuItemText_ShowQuickAccessToolbarBelowTheRibbon,
		RibbonPopupMenuItemText_ShowQuickAccessToolbarAboveTheRibbon,
		RibbonPopupMenuItemText_MinimizeRibbon,
		RibbonPopupMenuItemText_RemoveFromToolbar,
		RibbonPopupMenuItemText_AddToToolbar,
		RibbonCustomizationStrings_ToolbarCaptionEditorWindowCaption,
		RibbonCustomizationStrings_AddNewPageMenuItemCoreContent,
		RibbonCustomizationStrings_AddNewGroupMenuItemCoreContent,
		RibbonCustomizationStrings_ShowPageMenuItemCoreContent,
		RibbonCustomizationStrings_RenameMenuItemCoreContent,
		RibbonCustomizationStrings_RemoveMenuItemCoreContent,
		RibbonCustomizationStrings_ResetMenuItemCoreContent,
		RibbonCustomizationStrings_MoveUpMenuItemCoreContent,
		RibbonCustomizationStrings_MoveDownMenuItemCoreContent,
		RibbonCustomizationStrings_DropDownNewCaption,
		RibbonCustomizationStrings_NewPageCaption,
		RibbonCustomizationStrings_NewPageGroupCation,
		RibbonCustomizationStrings_AllItemsComboBoxItemContent,
		RibbonCustomizationStrings_AllTabsComboBoxItemContent,
		RibbonCustomizationStrings_MainTabsComboBoxItemContent,
		RibbonCustomizationStrings_ToolTabsComboBoxItemContent,
		RibbonCustomizationStrings_ResetOnlySelectedRibbonPageString,
		RibbonCustomizationStrings_ResetAllCustomizationsString,
		RibbonCustomizationStrings_ImportCustomizationFileString,
		RibbonCustomizationStrings_ExportAllCustomizationsString,
		RibbonCustomizationStrings_IsCustomTextString,
		RibbonCustomizationStrings_BarButtonGroupString,
		RibbonCustomizationStrings_IsUnnamedItemString,
		RibbonCustomizationStrings_ImportExportString,
		RibbonCustomizationStrings_AddItemButtonContentString,
		RibbonCustomizationStrings_RemoveItemButtonContentString,
		RibbonCustomizationStrings_CustomizationsStringTextBlockText,
		RibbonCustomizationStrings_ApplyChangesButtonContent,
		RibbonCustomizationStrings_CloseButtonContent,
		RibbonCustomizationStrings_CustomizationFormCaption,
		RibbonCustomizationStrings_ToolbarCaptionEditorWindowCaptionForm,
		RibbonCustomizationStrings_SeparatorString,
		RibbonCustomizationStrings_ResetAllCustomizationsQuestion,
		RibbonPopupMenuItemText_CustomizeRibbonMenuItem,
		RibbonCustomizationStrings_ChooseCommandsFrom,
		RibbonCustomizationStrings_CustomizeTheRibbon,
		SpacingModeStrings_MenuHeaderCaption,
		SpacingModeStrings_MouseModeContent,
		SpacingModeStrings_MouseModeDescription,
		SpacingModeStrings_TouchModeContent,
		SpacingModeStrings_TouchModeDescription,
		RibbonShowModeSelector_AutoHideMode,
		RibbonShowModeSelector_MinimizedMode,
		RibbonShowModeSelector_NormalMode
	}
	public class RibbonControlLocalizer : DXLocalizer<RibbonControlStringId> {
		static RibbonControlLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<RibbonControlStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(RibbonControlStringId.RibbonPopupMenuItemText_ShowQuickAccessToolbarBelowTheRibbon, "Show Quick Access Toolbar Below the Ribbon");
			AddString(RibbonControlStringId.RibbonPopupMenuItemText_ShowQuickAccessToolbarAboveTheRibbon, "Show Quick Access Toolbar Above the Ribbon");
			AddString(RibbonControlStringId.RibbonPopupMenuItemText_MinimizeRibbon, "Minimize the Ribbon");
			AddString(RibbonControlStringId.RibbonPopupMenuItemText_RemoveFromToolbar, "Remove from Quick Access Toolbar");
			AddString(RibbonControlStringId.RibbonPopupMenuItemText_AddToToolbar, "Add to Quick Access Toolbar");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_AddNewGroupMenuItemCoreContent, "Add new group");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_AddNewPageMenuItemCoreContent, "Add new page");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_AllItemsComboBoxItemContent, "All items");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_AllTabsComboBoxItemContent, "All tabs");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ExportAllCustomizationsString, "Export all customizations");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ImportCustomizationFileString, "Import customization file");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_MainTabsComboBoxItemContent, "Main tabs");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_MoveDownMenuItemCoreContent, "Move down");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_MoveUpMenuItemCoreContent, "Move up");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_NewPageCaption, "New Page");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_DropDownNewCaption, "New");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_NewPageGroupCation, "New Group");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_RemoveMenuItemCoreContent, "Remove");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_RenameMenuItemCoreContent, "Rename");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ResetAllCustomizationsString, "Reset all customizations");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ResetMenuItemCoreContent, "Reset");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ResetOnlySelectedRibbonPageString, "Reset only selected Ribbon page");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ShowPageMenuItemCoreContent, "Show page");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ToolbarCaptionEditorWindowCaption, "Set element name");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ToolbarCaptionEditorWindowCaptionForm, "Element name:");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ToolTabsComboBoxItemContent, "Tool tabs");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_IsCustomTextString, "(Custom)");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_BarButtonGroupString, "Bar Button Group");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_IsUnnamedItemString, "Unnamed Item");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ImportExportString, @"Import/Export");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_AddItemButtonContentString, @"Add >>");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_RemoveItemButtonContentString, @"<< Remove");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_CustomizationsStringTextBlockText, @"Customizations:");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ApplyChangesButtonContent, "OK");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_CloseButtonContent, "Cancel");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_CustomizationFormCaption, "Ribbon customization window");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_SeparatorString, "Separator");
			AddString(RibbonControlStringId.RibbonPopupMenuItemText_CustomizeRibbonMenuItem, "Customize Ribbon");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ResetAllCustomizationsQuestion, "Delete all Ribbon and Quick Access Toolbar customizations for this program?");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_ChooseCommandsFrom, "Choose commands from:");
			AddString(RibbonControlStringId.RibbonCustomizationStrings_CustomizeTheRibbon, "Customize the Ribbon:");
			AddString(RibbonControlStringId.SpacingModeStrings_MenuHeaderCaption, "Optimize spacing between commands");
			AddString(RibbonControlStringId.SpacingModeStrings_MouseModeContent, "Mouse");
			AddString(RibbonControlStringId.SpacingModeStrings_MouseModeDescription, "Standard ribbon and commands. Optimized for use with mouse.");
			AddString(RibbonControlStringId.SpacingModeStrings_TouchModeContent, "Touch");
			AddString(RibbonControlStringId.SpacingModeStrings_TouchModeDescription, "More space between commands. Optimized for use with touch.");
			AddString(RibbonControlStringId.RibbonShowModeSelector_AutoHideMode, "Auto Hide Ribbon");
			AddString(RibbonControlStringId.RibbonShowModeSelector_MinimizedMode, "Minimized Mode");
			AddString(RibbonControlStringId.RibbonShowModeSelector_NormalMode, "Normal Mode");
		}
		#endregion
		public static XtraLocalizer<RibbonControlStringId> CreateDefaultLocalizer() {
			return new RibbonControlResXLocalizer();
		}
		public static string GetString(RibbonControlStringId id) {
			return Active.GetLocalizedString(id);
		}
		internal static string GetString(string stringId) {
			return GetString((RibbonControlStringId)Enum.Parse(typeof(RibbonControlStringId), stringId, false));
		}
		public override XtraLocalizer<RibbonControlStringId> CreateResXLocalizer() {
			return new RibbonControlResXLocalizer();
		}
	}
	public class RibbonControlResXLocalizer : DXResXLocalizer<RibbonControlStringId> {
		public RibbonControlResXLocalizer()
			: base(new RibbonControlLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Ribbon.LocalizationRes", typeof(RibbonControlResXLocalizer).Assembly);
		}
	}
	public class RibbonControlStringIdConverter : StringIdConverter<RibbonControlStringId> {
		protected override XtraLocalizer<RibbonControlStringId> Localizer { get { return RibbonControlLocalizer.Active; } }
	}
}
