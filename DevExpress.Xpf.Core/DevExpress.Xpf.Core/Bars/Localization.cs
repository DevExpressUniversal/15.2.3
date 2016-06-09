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
using System.Windows;
namespace DevExpress.Xpf.Bars {
	public enum BarsStringId {
		CustomizationMenu_AddOrRemoveItemCaption,
		CustomizationForm_Caption,
		CustomizationMenu_VisibleItemCaption,
		CustomizationMenu_BeginGroupItemCaption,
		CustomizationMenu_DeleteItemCaption,
		CustomizationMenu_ResetItemCaption,
		CustomizationMenu_LinkItemCaption,
		CustomizationMenu_DisplayModeItemCaption,
		BarItemDisplayMode_Default,
		BarItemDisplayMode_Content,
		BarItemDisplayMode_ContentAndGlyph,
		CustomizationMenu_GlyphAlignmentItemCaption,
		CustomizationMenu_AlignmentItemCaption,
		CustomizationMenu_GlyphSizeItemCaption,
		Dock_Left,
		Dock_Right,
		Dock_Top,
		Dock_Bottom,
		BarItemAlignment_Default,
		BarItemAlignment_Near,
		BarItemAlignment_Far,
		GlyphSize_Default,
		GlyphSize_Small,
		GlyphSize_Large,
		GalleryControlFilterMenu_ShowAllItemCaption,
		BarManagerCategory_UnassignedCategoryCaption,
		BarManagerCategory_AllItemsCategoryCaption,
		ToolbarsCustomizationControl_EditorWindowCaption,
		LinkListItem_CustomizationItemCaption,
		ToolbarListItem_CustomizationItemCaption,
		ToolbarCaptionEditor_Caption,
		Ok,
		Cancel,
		New,
		Rename,
		Delete,
		Close,
		OptionsControl_IconsGroupCaption,
		OptionsControl_ScreenTipsGroupCaption,
		OptionsControl_LargeIconsInToolbarsCaption,
		OptionsControl_LargeIconsInMenuCaption,
		OptionsControl_ShowScreenTipsOnToolbarsCaption,
		OptionsControl_ShowShortcutKeysOnScreenTipsCaption,
		CommandsCustomizationControl_CategoriesListCaption,
		CommandsCustomizationControl_CommandsListCaption,
		CommandsCustomizationControl_DescriptionCaption,
		CustomizationControl_ToolbarsTabCaption,
		CustomizationControl_CommandsTabCaption,
		CustomizationControl_OptionsTabCaption,
		ToolbarsCustomizationControl_ToolbarsListCaption
	}
	public class BarsLocalizer : DXLocalizer<BarsStringId> {
		public new static XtraLocalizer<BarsStringId> Active {
			get { return XtraLocalizer<BarsStringId>.Active; }
			set { XtraLocalizer<BarsStringId>.Active = value; }
		}		
		static BarsLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<BarsStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(BarsStringId.CustomizationForm_Caption, "Customization");
			AddString(BarsStringId.CustomizationMenu_AddOrRemoveItemCaption, "Add or Remove Buttons");
			AddString(BarsStringId.CustomizationMenu_VisibleItemCaption, "Visible");
			AddString(BarsStringId.CustomizationMenu_BeginGroupItemCaption, "Begin a group");
			AddString(BarsStringId.CustomizationMenu_DeleteItemCaption, "Delete");
			AddString(BarsStringId.CustomizationMenu_ResetItemCaption, "Reset");
			AddString(BarsStringId.CustomizationMenu_LinkItemCaption, "Caption:");
			AddString(BarsStringId.CustomizationMenu_DisplayModeItemCaption, "Display mode:");
			AddString(BarsStringId.BarItemDisplayMode_Default, "Default");
			AddString(BarsStringId.BarItemDisplayMode_Content, "Content");
			AddString(BarsStringId.BarItemDisplayMode_ContentAndGlyph, "ContentAndGlyph");
			AddString(BarsStringId.CustomizationMenu_GlyphAlignmentItemCaption, "Glyph alignment:");
			AddString(BarsStringId.CustomizationMenu_AlignmentItemCaption, "Alignment:");
			AddString(BarsStringId.CustomizationMenu_GlyphSizeItemCaption, "Glyph size:");
			AddString(BarsStringId.Dock_Left, "Left");
			AddString(BarsStringId.Dock_Right, "Right");
			AddString(BarsStringId.Dock_Top, "Top");
			AddString(BarsStringId.Dock_Bottom, "Bottom");
			AddString(BarsStringId.BarItemAlignment_Default, "Default");
			AddString(BarsStringId.BarItemAlignment_Near, "Near");
			AddString(BarsStringId.BarItemAlignment_Far, "Far");
			AddString(BarsStringId.GlyphSize_Default, "Default");
			AddString(BarsStringId.GlyphSize_Small, "Small");
			AddString(BarsStringId.GlyphSize_Large, "Large");
			AddString(BarsStringId.GalleryControlFilterMenu_ShowAllItemCaption, "Show all");
			AddString(BarsStringId.BarManagerCategory_UnassignedCategoryCaption, "Unassigned Items");
			AddString(BarsStringId.BarManagerCategory_AllItemsCategoryCaption, "All Items");
			AddString(BarsStringId.ToolbarsCustomizationControl_EditorWindowCaption, "Toolbar Caption Editor");
			AddString(BarsStringId.LinkListItem_CustomizationItemCaption, "Customize...");
			AddString(BarsStringId.ToolbarListItem_CustomizationItemCaption, "Customize...");
			AddString(BarsStringId.ToolbarCaptionEditor_Caption, "Toolbar Name:");
			AddString(BarsStringId.Ok, "OK");
			AddString(BarsStringId.Cancel, "Cancel");
			AddString(BarsStringId.New, "New");
			AddString(BarsStringId.Rename, "Rename");
			AddString(BarsStringId.Delete, "Delete");
			AddString(BarsStringId.Close, "Close");
			AddString(BarsStringId.OptionsControl_IconsGroupCaption, "Icons");
			AddString(BarsStringId.OptionsControl_ScreenTipsGroupCaption, "ScreenTips");
			AddString(BarsStringId.OptionsControl_LargeIconsInMenuCaption, "Large icons in Menu");
			AddString(BarsStringId.OptionsControl_LargeIconsInToolbarsCaption, "Large icons in Toolbars");
			AddString(BarsStringId.OptionsControl_ShowScreenTipsOnToolbarsCaption, "Show ScreenTips on toolbars");
			AddString(BarsStringId.OptionsControl_ShowShortcutKeysOnScreenTipsCaption, "Show shortcut keys on ScreenTips");
			AddString(BarsStringId.CommandsCustomizationControl_CategoriesListCaption, "Categories:");
			AddString(BarsStringId.CommandsCustomizationControl_CommandsListCaption, "Commands:");
			AddString(BarsStringId.CommandsCustomizationControl_DescriptionCaption, "Description");
			AddString(BarsStringId.CustomizationControl_ToolbarsTabCaption, "Toolbars");
			AddString(BarsStringId.CustomizationControl_CommandsTabCaption, "Commands");
			AddString(BarsStringId.CustomizationControl_OptionsTabCaption, "Options");
			AddString(BarsStringId.ToolbarsCustomizationControl_ToolbarsListCaption, "Toolbars:");
		}
		#endregion
		public static XtraLocalizer<BarsStringId> CreateDefaultLocalizer() {
			return new BarsResXLocalizer();
		}
		public static string GetString(BarsStringId id) {
			return Active.GetLocalizedString(id);
		}
		internal static string GetString(string stringId) {
			return GetString((BarsStringId)Enum.Parse(typeof(BarsStringId), stringId, false));
		}
		public override XtraLocalizer<BarsStringId> CreateResXLocalizer() {
			return new BarsResXLocalizer();
		}
		public static string GetStringFromEnumItem(Enum enumItem) {
			return Active.GetLocalizedString((BarsStringId)Enum.Parse(typeof(BarsStringId), enumItem.GetType().Name + "_" + enumItem.ToString()));
		}
		public static T CreateEnumItemFromIndex<T>(int index) {
			return (T)Enum.ToObject(typeof(T), (int)index);
		}
	}
	public class BarsResXLocalizer : DXResXLocalizer<BarsStringId> {
		public BarsResXLocalizer()
			: base(new BarsLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Core.Bars.LocalizationRes", typeof(BarsResXLocalizer).Assembly);
		}
	}
}
