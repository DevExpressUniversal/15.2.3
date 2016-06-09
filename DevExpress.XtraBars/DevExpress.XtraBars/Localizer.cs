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

using System.Diagnostics;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using DevExpress.XtraBars.Customization;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraBars.Localization {
	[ToolboxItem(false)]
	public class BarLocalizer : XtraLocalizer<BarString> {
		static BarLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<BarString>(CreateDefaultLocalizer()));
		}
		public new static BarLocalizer Active { 
			get { return XtraLocalizer<BarString>.Active as BarLocalizer; }
			set { XtraLocalizer<BarString>.Active = value; }
		}
		public override XtraLocalizer<BarString> CreateResXLocalizer() {
			return new BarResLocalizer();
		}
		public static XtraLocalizer<BarString> CreateDefaultLocalizer() { return new BarResLocalizer(); }
		CustomizationControl customization;
		public CustomizationControl Customization {
			get {
				if(customization == null)
					this.customization = CreateCustomizationControl();
				return customization;
			}
			set { customization = value; }
		}
		protected virtual CustomizationControl CreateCustomizationControl() { return new CustomizationControl(); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(BarString.None, ""); 
			AddString(BarString.PopupMenuEditor, "Popup Menu Editor");
			AddString(BarString.AddOrRemove, "&Add or Remove Buttons");
			AddString(BarString.Visible, "Visible");
			AddString(BarString.ResetBar, "Are you sure you want to reset the changes made to the '{0}' toolbar?");
			AddString(BarString.ResetBarCaption, "Customize");
			AddString(BarString.ResetButton, "&Reset Toolbar");
			AddString(BarString.CustomizeButton, "&Customize ...");
			AddString(BarString.CancelButton, "Cancel");
			AddString(BarString.ToolBarMenu, "&Reset$&Delete$!&Name$!Defau&lt style$&Text Only (Always)$Text &Only (in Menus)$" +
					"Image &and Text$!Begin a &Group$&Visible$&Most recently used");
			AddString(BarString.ToolbarNameCaption, "&Toolbar Name:");
			AddString(BarString.NewToolbarCaption, "New Toolbar");
			AddString(BarString.NewToolbarCustomNameFormat, "Custom {0}");
			AddString(BarString.NewMenuName, "Main menu");
			AddString(BarString.NewToolbarName, "Tools");
			AddString(BarString.NewStatusBarName, "Status bar");
			AddString(BarString.RenameToolbarCaption, "Rename Toolbar");
			AddString(BarString.CustomizeWindowCaption, "Customization");
			AddString(BarString.MenuAnimationSystem, "(System default)");
			AddString(BarString.MenuAnimationNone, "None");
			AddString(BarString.MenuAnimationSlide, "Slide");
			AddString(BarString.MenuAnimationFade, "Fade");
			AddString(BarString.MenuAnimationUnfold, "Unfold");
			AddString(BarString.MenuAnimationRandom, "Random");
			AddString(BarString.RibbonToolbarAbove, "&Place Quick Access Toolbar above the Ribbon");
			AddString(BarString.RibbonToolbarBelow, "&Place Quick Access Toolbar below the Ribbon");
			AddString(BarString.RibbonToolbarRemove, "&Remove from Quick Access Toolbar");
			AddString(BarString.RibbonToolbarAdd, "&Add to Quick Access Toolbar");
			AddString(BarString.RibbonToolbarMinimizeRibbon, "Mi&nimize the Ribbon");
			AddString(BarString.RibbonGalleryFilter, "All groups");
			AddString(BarString.RibbonGalleryFilterNone, "None");
			AddString(BarString.BarUnassignedItems, "(Unassigned Items)");
			AddString(BarString.BarAllItems, "(All Items)");
			AddString(BarString.RibbonUnassignedPages, "(Unassigned Pages)");
			AddString(BarString.RibbonAllPages, "(All Pages)");
			AddString(BarString.SkinsMain, "Standard Skins");
			AddString(BarString.SkinsOffice, "Office Skins");
			AddString(BarString.SkinsTheme, "Theme Skins");
			AddString(BarString.SkinsBonus, "Bonus Skins");
			AddString(BarString.SkinsCustom, "Custom Skins");
			AddString(BarString.SkinCaptions, "|DevExpress Style|Caramel|Money Twins|DevExpress Dark Style|iMaginary|Lilian|Black|Blue|Office 2010 Blue|Office 2010 Black|Office 2010 Silver|Office 2007 Blue|Office 2007 Black|Office 2007 Silver|Office 2007 Green|Office 2007 Pink|Seven|Seven Classic|Darkroom|McSkin|Sharp|Sharp Plus|Foggy|Dark Side|Xmas (Blue)|Springtime|Summer|Pumpkin|Valentine|Stardust|Coffee|Glass Oceans|High Contrast|Liquid Sky|London Liquid Sky|The Asphalt World|Blueprint|Whiteprint|Visual Studio 2010|Metropolis|Metropolis Dark|Office 2013 White|Office 2013 Dark Gray|Office 2013 Light Gray|Visual Studio 2013 Blue|Visual Studio 2013 Dark|Visual Studio 2013 Light|");
			AddString(BarString.ExpandRibbonSuperTipHeader, "Expand the Ribbon (Ctrl+F1)");
			AddString(BarString.CollapseRibbonSuperTipHeader, "Minimize the Ribbon (Ctrl+F1)");
			AddString(BarString.ExpandRibbonSuperTipText, "Show the Ribbon so that it is always expanded even after you click a command");
			AddString(BarString.CollapseRibbonSuperTipText, "Only show tab names on the Ribbon");
			AddString(BarString.CustomizeQuickAccessToolbar, "&Customize Quick Access Toolbar...");
			AddString(BarString.CustomizeRibbon, "Customize the &Ribbon...");
			AddString(BarString.MoreCommands, "&More Commands...");
			AddString(BarString.CustomizeToolbarText, "Customize Toolbar");
			AddString(BarString.CustomizeToolbarSuperTipText, "Customize Quick Access Toolbar");
			AddString(BarString.RibbonCustomizationOptionAllTabs, "All Tabs");
			AddString(BarString.RibbonCustomizationOptionAllCommands, "All Commands");
			AddString(BarString.RibbonCustomizationStandardCustomItemSuffix, "Custom");
			AddString(BarString.RibbonCustomizationResetSelectedTabSettingsCommand, "Reset only selected Ribbon tab");
			AddString(BarString.RibbonCustomizationResetSettingsCommand, "Reset all customizations");
			AddString(BarString.RibbonCustomizationImportSettingsCommand, "Import customization file");
			AddString(BarString.RibbonCustomizationExportSettingsCommand, "Export all customizations");
			AddString(BarString.RibbonCustomizationNewTabDefaultAlias, "New Tab");
			AddString(BarString.RibbonCustomizationNewGroupDefaultAlias, "New Group");
			AddString(BarString.RibbonCustomizationNewCategoryDefaultAlias, "New Category");
			AddString(BarString.RibbonCustomizationNewCategoryCommand, "New Category");
			AddString(BarString.RibbonCustomizationNewPageCommand, "New Tab");
			AddString(BarString.RibbonCustomizationNewGroupCommand, "New Group");
			AddString(BarString.RibbonCustomizationRenameText, "Rename");
			AddString(BarString.RibbonCustomizationRemoveText, "Remove");
			AddString(BarString.RibbonCustomizationAddText, "Add");
			AddString(BarString.RibbonCustomizationDownText, "Move Down");
			AddString(BarString.RibbonCustomizationUpText, "Move Up");
			AddString(BarString.RibbonCaptionTextNone, "No name");
			AddString(BarString.ColorAuto, "Auto");
			AddString(BarString.AccordionControlSearchBoxPromptText, "Type keywords here");
		}
		#endregion
	}
	public class BarResLocalizer : BarLocalizer {
		ResourceManager manager = null;
		public BarResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if(manager != null) this.manager.ReleaseAllResources();
			this.manager = new ResourceManager("DevExpress.XtraBars.LocalizationRes", typeof(BarResLocalizer).Assembly);
		}
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; }}
		public override string GetLocalizedString(BarString id) {
			if(id == BarString.None) return "";
			string resStr = "BarString." + id.ToString();
			string ret = Manager.GetString(resStr);
			if(ret == null) ret = "";
			return ret;
		}
	}
	#region enum BarString
	public enum BarString { 
		None,
		PopupMenuEditor,
		AddOrRemove, 
		Visible,
		ResetBar, 
		ResetBarCaption, 
		ResetButton, 
		CustomizeButton,
		CancelButton, 
		ToolBarMenu, 
		ToolbarNameCaption,
		NewToolbarCaption,
		NewToolbarCustomNameFormat,
		RenameToolbarCaption,
		CustomizeWindowCaption,
		MenuAnimationSystem,
		MenuAnimationNone,
		MenuAnimationSlide,
		MenuAnimationFade,
		MenuAnimationUnfold,
		MenuAnimationRandom,
		RibbonToolbarAbove,
		RibbonToolbarBelow,
		RibbonToolbarAdd,
		RibbonToolbarMinimizeRibbon,
		RibbonToolbarRemove,
		RibbonGalleryFilter,
		RibbonGalleryFilterNone,
		BarUnassignedItems,
		BarAllItems,
		RibbonUnassignedPages,
		RibbonAllPages,
		NewToolbarName,
		NewMenuName,
		NewStatusBarName,
		CloseButton,
		MinimizeButton,
		MaximizeButton,
		RestoreButton,
		HelpButton,
		FullScreenButton,
		SkinsMain,
		SkinsOffice,
		SkinsTheme,
		SkinsBonus,
		SkinsCustom,
		SkinCaptions,
		ShowScreenTipsOnToolbarsName,
		ShowShortcutKeysOnScreenTipsName,
		ExpandRibbonSuperTipHeader,
		CollapseRibbonSuperTipHeader,
		ExpandRibbonSuperTipText,
		CollapseRibbonSuperTipText,
		MoreCommands,
		CustomizeRibbon,
		CustomizeQuickAccessToolbar,
		CustomizeToolbarText,
		CustomizeToolbarSuperTipText,
		RibbonCustomizationOptionAllTabs,
		RibbonCustomizationOptionAllCommands,
		RibbonCustomizationStandardCustomItemSuffix,
		RibbonCustomizationResetSelectedTabSettingsCommand,
		RibbonCustomizationResetSettingsCommand,
		RibbonCustomizationImportSettingsCommand,
		RibbonCustomizationExportSettingsCommand,
		RibbonCustomizationNewTabDefaultAlias,
		RibbonCustomizationNewGroupDefaultAlias,
		RibbonCustomizationNewCategoryDefaultAlias,
		RibbonCustomizationNewCategoryCommand,
		RibbonCustomizationNewPageCommand,
		RibbonCustomizationNewGroupCommand,
		RibbonCustomizationRenameText,
		RibbonCustomizationRemoveText,
		RibbonCustomizationAddText,
		RibbonCustomizationDownText,
		RibbonCustomizationUpText,
		RibbonCaptionTextNone,
		RibbonTouchMouseModeCommandText,
		RibbonTouchMouseModeGalleryGroupText,
		RibbonTouchMouseModeTouchItemText,
		RibbonTouchMouseModeMouseItemText,
		RibbonTouchMouseModeTouchItemDescription,
		RibbonTouchMouseModeMouseItemDescription,
		ColorAuto,
		AccordionControlSearchBoxPromptText,
	}
	#endregion
}
