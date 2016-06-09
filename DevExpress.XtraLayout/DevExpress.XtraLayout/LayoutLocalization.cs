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
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using DevExpress.XtraLayout;
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraLayout.Localization {
	public class LayoutLocalizer : XtraLocalizer<LayoutStringId> {
		static LayoutLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<LayoutStringId>(CreateDefaultLocalizer()));
		}
		public new static XtraLocalizer<LayoutStringId> Active { 
			get { return XtraLocalizer<LayoutStringId>.Active; }
			set { XtraLocalizer<LayoutStringId>.Active = value; }
		}
		public override XtraLocalizer<LayoutStringId> CreateResXLocalizer() {
			return new LayoutResLocalizer();
		}
		public static XtraLocalizer<LayoutStringId> CreateDefaultLocalizer() { return new LayoutResLocalizer(); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
#pragma warning disable 618
			AddString(LayoutStringId.CustomizationParentName, "Customization");
#pragma warning restore 618
			AddString(LayoutStringId.DefaultItemText, "Item ");
			AddString(LayoutStringId.DefaultActionText, "DefaultAction");
			AddString(LayoutStringId.DefaultEmptyText, "none");
			AddString(LayoutStringId.LayoutItemDescription, "Layout control item element");
			AddString(LayoutStringId.LayoutGroupDescription, "Layout control group element");
			AddString(LayoutStringId.TabbedGroupDescription, "Layout control tabbedGroup element");
			AddString(LayoutStringId.LayoutControlDescription, "Layout control");
			AddString(LayoutStringId.CustomizationFormTitle, "Customization");
			AddString(LayoutStringId.TreeViewPageTitle, "Layout Tree View");
			AddString(LayoutStringId.HiddenItemsPageTitle, "Hidden Items");
			AddString(LayoutStringId.HiddenItemsNodeText, "Hidden Items");
			AddString(LayoutStringId.RenameSelected, "Rename");
			AddString(LayoutStringId.HideItemMenutext, "Hide Item");
			AddString(LayoutStringId.LockItemSizeMenuText, "Lock Item Size");
			AddString(LayoutStringId.UnLockItemSizeMenuText, "UnLock Item Size");
			AddString(LayoutStringId.GroupItemsMenuText, "Group");
			AddString(LayoutStringId.UnGroupItemsMenuText, "Ungroup");
			AddString(LayoutStringId.CreateTabbedGroupMenuText, "Create Tab Control");
			AddString(LayoutStringId.AddTabMenuText, "Add Tab");
			AddString(LayoutStringId.UnGroupTabbedGroupMenuText, "Remove Tab Control");
			AddString(LayoutStringId.TreeViewRootNodeName, "Root");
			AddString(LayoutStringId.ShowCustomizationFormMenuText, "Customize Layout");
			AddString(LayoutStringId.HideCustomizationFormMenuText, "Hide Customization Form");
			AddString(LayoutStringId.EmptySpaceItemDefaultText, "Empty Space Item");
			AddString(LayoutStringId.SplitterItemDefaultText, "Splitter");
			AddString(LayoutStringId.SimpleLabelItemDefaultText, "Label");
			AddString(LayoutStringId.SimpleSeparatorItemDefaultText, "Separator");
			AddString(LayoutStringId.ControlGroupDefaultText, "Group ");
			AddString(LayoutStringId.EmptyRootGroupText, "Drop controls here");
			AddString(LayoutStringId.EmptyTabbedGroupText, "Drag drop groups into the caption area");
			AddString(LayoutStringId.ResetLayoutMenuText, "Reset Layout");
			AddString(LayoutStringId.RenameMenuText, "Rename");
			AddString(LayoutStringId.TextPositionMenuText, "Text Position");
			AddString(LayoutStringId.TextPositionLeftMenuText, "Left");
			AddString(LayoutStringId.TextPositionRightMenuText, "Right");
			AddString(LayoutStringId.TextPositionTopMenuText, "Top");
			AddString(LayoutStringId.TextPositionBottomMenuText, "Bottom");
			AddString(LayoutStringId.ShowTextMenuItem, "Show Text");
			AddString(LayoutStringId.HideTextMenuItem, "Hide Text");
			AddString(LayoutStringId.ShowSpaceMenuItem, "Show Text PlaceHolder");
			AddString(LayoutStringId.HideSpaceMenuItem, "Collapse Text Placeholder");
			AddString(LayoutStringId.LockSizeMenuItem, "Lock Size");
			AddString(LayoutStringId.LockWidthMenuItem, "Lock Width");
			AddString(LayoutStringId.LockHeightMenuItem, "Lock Height");
			AddString(LayoutStringId.FreeSizingMenuItem, "Free sizing");
			AddString(LayoutStringId.CreateEmptySpaceItem, "Create EmptySpace Item");
			AddString(LayoutStringId.UndoButtonHintText, "Undo last action");
			AddString(LayoutStringId.RedoButtonHintText, "Redo last action");
			AddString(LayoutStringId.SaveButtonHintText, "Save layout to XML file");
			AddString(LayoutStringId.LoadButtonHintText, "Load layout from XML file");
			AddString(LayoutStringId.UndoHintCaption, "Undo(Ctrl+Z)");
			AddString(LayoutStringId.RedoHintCaption, "Redo(Ctrl+Y)");
			AddString(LayoutStringId.LoadHintCaption, "Load layout(Ctrl+O)");
			AddString(LayoutStringId.SaveHintCaption, "Save layout(Ctrl+S)");
			AddString(LayoutStringId.LayoutResetConfirmationText, "You are about to reset your layout customizations. Do you want to proceed?");
			AddString(LayoutStringId.LayoutResetConfirmationDialogCaption, "Layout reset confirmation");
		}
		#endregion
	}
	public class LayoutResLocalizer : LayoutLocalizer {
		ResourceManager manager = null;
		public LayoutResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if(manager != null) this.manager.ReleaseAllResources();
			this.manager = new ResourceManager("DevExpress.XtraLayout.LocalizationRes", typeof(LayoutResLocalizer).Assembly);
		}
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; }}
		public override string GetLocalizedString(LayoutStringId id) {
			string resStr = "LayoutStringId." + id.ToString();
			string ret = Manager.GetString(resStr);
			if(ret == null) ret = "";
			return ret;
		}
	}
	#region enum LayoutStringId
	public enum LayoutStringId {
		[Obsolete("Callers should not use the LayoutStringId.CustomizationParentName value.")]
		CustomizationParentName,
		DefaultItemText,
		DefaultActionText,
		DefaultEmptyText,
		LayoutItemDescription,
		LayoutGroupDescription,
		TabbedGroupDescription,
		LayoutControlDescription,
		CustomizationFormTitle,
		HiddenItemsPageTitle,
		HiddenItemsNodeText,
		TreeViewPageTitle,
		RenameSelected,
		HideItemMenutext,
		LockItemSizeMenuText,
		UnLockItemSizeMenuText,
		GroupItemsMenuText,
		UnGroupItemsMenuText,
		CreateTabbedGroupMenuText,
		AddTabMenuText,
		UnGroupTabbedGroupMenuText,
		TreeViewRootNodeName,
		ShowCustomizationFormMenuText,
		HideCustomizationFormMenuText,
		EmptySpaceItemDefaultText,
		SplitterItemDefaultText,
		SimpleLabelItemDefaultText,
		SimpleSeparatorItemDefaultText,
		ControlGroupDefaultText,
		EmptyRootGroupText,
		EmptyTabbedGroupText,
		ResetLayoutMenuText,
		RenameMenuText,
		CreateTemplate,
		TextPositionMenuText,
		TextPositionLeftMenuText,
		TextPositionRightMenuText,
		TextPositionTopMenuText,
		TextPositionBottomMenuText,
		ShowTextMenuItem,
		HideTextMenuItem,
		ShowSpaceMenuItem,
		HideSpaceMenuItem,
		LockSizeMenuItem,
		LockWidthMenuItem,
		LockHeightMenuItem, 
		LockMenuGroup,
		ResetConstraintsToDefaultsMenuItem,
		FreeSizingMenuItem,
		CreateEmptySpaceItem,
		UndoHintCaption,
		RedoHintCaption,
		LoadHintCaption,
		SaveHintCaption,
		UndoButtonHintText,
		RedoButtonHintText,
		SaveButtonHintText,
		LoadButtonHintText,
		LayoutResetConfirmationText,
		LayoutResetConfirmationDialogCaption,
		BestFitMenuText,
		ConvertToTableLayoutText,
		ConvertToRegularLayoutText,
		ConvertToText,
		FlowLayoutText,
		RegularLayoutText,
		TableLayoutText,
		DeleteTemplateText
	}
	#endregion
}
