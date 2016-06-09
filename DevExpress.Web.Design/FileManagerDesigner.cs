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
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Linq;
namespace DevExpress.Web.Design {
	public class ASPxFileManagerDesigner : ASPxDataWebControlDesigner {
		ASPxFileManager fileManager = null;
		public override void Initialize(IComponent component) {
			this.fileManager = (ASPxFileManager)component;
			base.Initialize(component);
		}
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("AccessRules", "AccessRules");
			propertyNameToCaptionMap.Add("Columns", "Columns");
			propertyNameToCaptionMap.Add("Items", "Items");
		}
		public ASPxFileManager FileManager {
			get { return fileManager; }
		}
		protected override bool IsControlRequireHttpHandlerRegistration() {
			return true;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new FileManagerDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new FileManagerCommonFormDesigner(FileManager, DesignerHost)));
		}
	}
	public class FileManagerDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxFileManagerDesigner designer = null;
		public FileManagerDesignerActionList(ASPxFileManagerDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public new ASPxFileManagerDesigner Designer {
			get { return this.designer; }
		}
		public ASPxFileManager FileManager {
			get { return Designer.FileManager; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("FileListView",
				StringResources.FileManagerActionList_FileListView));
			collection.Add(new DesignerActionPropertyItem("EnableUpload",
				StringResources.FileManagerActionList_EnableUpload));
			collection.Add(new DesignerActionPropertyItem("AllowCreate",
				StringResources.FileManagerActionList_AllowCreate));
			collection.Add(new DesignerActionPropertyItem("AllowRename",
				StringResources.FileManagerActionList_AllowRename));
			collection.Add(new DesignerActionPropertyItem("AllowMove",
				StringResources.FileManagerActionList_AllowMove));
			collection.Add(new DesignerActionPropertyItem("AllowCopy",
				StringResources.FileManagerActionList_AllowCopy));
			collection.Add(new DesignerActionPropertyItem("AllowDelete",
				StringResources.FileManagerActionList_AllowDelete));
			collection.Add(new DesignerActionPropertyItem("ShowDownloadButton",
				StringResources.FileManagerActionList_ShowDownloadButton));
			collection.Add(new DesignerActionPropertyItem("EnableMultiUpload",
				StringResources.FileManagerActionList_EnableMultiFileUpload, "",
				StringResources.FileManagerActionList_EnableMultiFileUploadDescription));
			collection.Add(new DesignerActionMethodItem(this, "OpenConfigureMaximumUploadLimitsHelp",
				StringResources.FileManagerActionList_HowConfigureMaximumUploadLimitsActionItem,
				StringResources.FileManagerActionList_HowConfigureMaximumUploadLimitsActionItem,
				StringResources.ActionList_OpenHelpActionItemDescription, false));			
			return collection;
		}
		protected void OpenConfigureMaximumUploadLimitsHelp() {
			ShowHelpFromUrl("#AspNet/CustomDocument9822");
		}
		public bool EnableUpload {
			get { return FileManager.SettingsUpload.Enabled; }
			set {
				FileManager.SettingsUpload.Enabled = value;
				FireControlPropertyChanged("SettingsUpload");
			}
		}
		public bool AllowCreate {
			get { return FileManager.SettingsEditing.AllowCreate; }
			set {
				FileManager.SettingsEditing.AllowCreate = value;
				FireControlPropertyChanged("SettingsEditing");
			}
		}
		public bool AllowRename {
			get { return FileManager.SettingsEditing.AllowRename; }
			set {
				FileManager.SettingsEditing.AllowRename = value;
				FireControlPropertyChanged("SettingsEditing");
			}
		}
		public bool AllowMove {
			get { return FileManager.SettingsEditing.AllowMove; }
			set {
				FileManager.SettingsEditing.AllowMove = value;
				FireControlPropertyChanged("SettingsEditing");
			}
		}
		public bool AllowCopy {
			get { return FileManager.SettingsEditing.AllowCopy; }
			set {
				FileManager.SettingsEditing.AllowCopy = value;
				FireControlPropertyChanged("SettingsEditing");
			}
		}
		public bool AllowDelete {
			get { return FileManager.SettingsEditing.AllowDelete; }
			set {
				FileManager.SettingsEditing.AllowDelete = value;
				FireControlPropertyChanged("SettingsEditing");
			}
		}
		public bool ShowDownloadButton {
			get { return FileManager.SettingsEditing.AllowDownload; }
			set {
				FileManager.SettingsEditing.AllowDownload = value;
				FireControlPropertyChanged("SettingsToolbar");
			}
		}
		public bool EnableMultiUpload {
			get { return FileManager.SettingsUpload.UseAdvancedUploadMode && FileManager.SettingsUpload.AdvancedModeSettings.EnableMultiSelect; }
			set {
				FileManager.SettingsUpload.AdvancedModeSettings.EnableMultiSelect = value;
				if(value)
					FileManager.SettingsUpload.UseAdvancedUploadMode = true;
				FireControlPropertyChanged("SettingsUpload");
			}
		}
		public FileListView FileListView {
			get { return FileManager.SettingsFileList.View; }
			set {
				FileManager.SettingsFileList.View = value;
				FireControlPropertyChanged("SettingsFileList");
			}
		}
		void FireControlPropertyChanged(string propertyName) {
			System.Web.UI.Design.ControlDesigner.InvokeTransactedChange(Component, delegate(object arg) {
				Designer.PropertyChanged(propertyName);
				return true;
			}, null, string.Format("{0} changed", propertyName));
		}
	}
	public class FileManagerCommonFormDesigner : CommonFormDesigner {
		public FileManagerCommonFormDesigner(object control, IServiceProvider provider)
			: base(new FileManagerAccessRulesOwner((ASPxFileManager)control, provider, ((ASPxFileManager)control).SettingsPermissions.AccessRules)) {
		}
		ASPxFileManager FileManager { get { return (ASPxFileManager)Control; } }
		protected override void CreateMainGroupItems() {
			AddAccessRulesItem();
			AddContextMenuItem();
			AddDetailsViewSettingsColumn();
			AddToolbarItem();			
			base.CreateClientSideEventsItem();
		}
		protected void AddAccessRulesItem() {
			MainGroup.Add(CreateDesignerItem(new FileManagerAccessRulesOwner(FileManager, Provider, FileManager.SettingsPermissions.AccessRules), typeof(ItemsEditorFrame), AccessRulesImageIndex));
		}
		protected void AddToolbarItem() {
			MainGroup.Add(CreateDesignerItem(new FileManagerToolbarItemsOwner(FileManager, "Toolbar Items", Provider, FileManager.SettingsToolbar.Items), typeof(ToolbarSettingsItemsEditorFrame), ToolbarItemsImageIndex));
		}
		protected void AddContextMenuItem() {
			MainGroup.Add(CreateDesignerItem(new FileManagerToolbarItemsOwner(FileManager, "Context Menu Items", Provider, FileManager.SettingsContextMenu.Items), typeof(ToolbarSettingsItemsEditorFrame), MenuImageIndex));
		}
		protected void AddDetailsViewSettingsColumn() {
			MainGroup.Add(CreateDesignerItem(new DetailsViewSettingsColumnsOwner(FileManager, Provider, FileManager.SettingsFileList.DetailsViewSettings.Columns), typeof(DetailsViewSettingsColumnsEditorFrame), DetailsViewSettingsImageIndex));
		}		
		protected override bool CanCreateItemsGroup { get { return true; } }
	}
	public class FileManagerAccessRulesOwner : FlatCollectionOwner {
		public FileManagerAccessRulesOwner(object component, IServiceProvider provider, AccessRulesCollection accessRules)
			: base(component, "AccessRules", provider, accessRules) {
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(FileManagerFileAccessRule), "File Access Rule", FileManagerFileAccessRuleItemImageResource);
			AddItemType(typeof(FileManagerFolderAccessRule), "Folder Access Rule", FileManagerFolderAccessRuleItemImageResource);
		}
		protected override List<DesignEditorMenuRootItemActionType> GetContextMenuActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.InsertChild,
				DesignEditorMenuRootItemActionType.ChangeTo,
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp,
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.MoveLeft,
				DesignEditorMenuRootItemActionType.MoveRight,
				DesignEditorMenuRootItemActionType.SelectAll
			};
		}
		protected override DesignEditorDescriptorItem CreateAddItemMenuItem() {
			var item = base.CreateAddItemMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertBeforeMenuItem() {
			var item = base.CreateInsertBeforeMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			return item;
		}
		public override Type GetDefaultItemType() {
			return typeof(FileManagerFileAccessRule);			
		}
		protected override internal string GetNavBarItemsGroupName() {
			return "Access Rules";
		}		
	}
	public class FileManagerToolbarItemsOwner : ItemsEditorOwner {
		public FileManagerToolbarItemsOwner(object component, string collectionPropertyName, IServiceProvider provider, FileManagerToolbarItemCollection items)
			: base(component, collectionPropertyName, provider, items) {			
		}
		public FileManagerToolbarItemsOwner(FileManagerToolbarItemCollection items)
			: base(null, null, null, items) {
		}
		protected override void FillItemTypes() {		
			AddItemType(typeof(FileManagerToolbarCreateButton), "Create Button", FileManagerCreateButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarRenameButton), "Rename Button", FileManagerRenameButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarMoveButton), "Move Button", FileManagerMoveButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarCopyButton), "Copy Button", FileManagerCopyButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarDeleteButton), "Delete Button", FileManagerDeleteButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarRefreshButton), "Refresh Button", FileManagerRefreshButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarDownloadButton), "Download Button", FileManagerDownloadButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarUploadButton), "Upload Button", FileManagerUploadButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarCustomButton), "Custom Button", FileManagerCustomButtonItemImageResource);
			AddItemType(typeof(FileManagerToolbarCustomDropDownButton), "Custom Dropdown Button", FileManagerCustomDropdownButtonItemImageResource);
		}
		protected override DesignEditorDescriptorItem CreateEditorDescriptorItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return CreateDefaultToolbarItemsItem();
			return base.CreateEditorDescriptorItem(actionType, isToolbarMenu);
		}
		protected DesignEditorDescriptorItem CreateDefaultToolbarItemsItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.CreateDefaultItems };
			item.Caption = "Default Items";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = true;
			item.BeginGroup = true;
			PopulateChildItems(item);
			return item;
		}
		public void CreateDefaultItems() {
			BeginUpdate();
			(Items as FileManagerToolbarItemCollection).CreateDefaultItems();
			SetFocusedItem(Items.OfType<IDesignTimeCollectionItem>().FirstOrDefault(), true);
			EndUpdate();
		}
		public override Type GetDefaultItemType() {
			return typeof(FileManagerToolbarCustomButton);
		}
		protected virtual bool IsBeginGroup(Type type) {
			if(type == typeof(FileManagerToolbarCustomButton))
				return true;
			return false;
		}
		protected override DesignEditorDescriptorItem CreateSubmenuItem(DesignEditorDescriptorItem parent, IDesignTimeColumnAndEditorItem designTimeItem, bool isToolbar) {
			DesignEditorDescriptorItem menuItem = base.CreateSubmenuItem(parent, designTimeItem, isToolbar);
			menuItem.BeginGroup = IsBeginGroup(designTimeItem.ColumnType);
			return menuItem;
		}
	}
	public class DetailsViewSettingsColumnsOwner : FlatCollectionOwner {
		public DetailsViewSettingsColumnsOwner(object component, IServiceProvider provider, FileManagerDetailsColumnCollection Columns)
			: base(component, "Columns", provider, Columns) {
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(FileManagerDetailsColumn), "Details Column", FileManagerDetailsColumnItemImageResource);
			AddItemType(typeof(FileManagerDetailsCustomColumn), "Details Custom Column", FileManagerDetailsCustomColumnItemImageResource);
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			var list = base.GetToolbarActionTypes();
			list.Add(DesignEditorMenuRootItemActionType.CreateDefaultItems);
			return list;
		}
		protected override DesignEditorDescriptorItem CreateAddItemMenuItem() {
			var item = base.CreateAddItemMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateInsertBeforeMenuItem() {
			var item = base.CreateInsertBeforeMenuItem();
			item.EditorType = DesignEditorDescriptorItemType.DropDownButton;
			return item;
		}
		protected override DesignEditorDescriptorItem CreateEditorDescriptorItem(DesignEditorMenuRootItemActionType actionType, bool isToolbarMenu) {
			if(actionType == DesignEditorMenuRootItemActionType.CreateDefaultItems)
				return CreateDefaultRibbonTabsItem();
			return base.CreateEditorDescriptorItem(actionType, isToolbarMenu);
		}
		protected DesignEditorDescriptorItem CreateDefaultRibbonTabsItem() {
			var item = new DesignEditorDescriptorItem() { ActionType = DesignEditorMenuRootItemActionType.CreateDefaultItems };
			item.Caption = "Default Columns";
			item.EditorType = DesignEditorDescriptorItemType.Button;
			item.Enabled = true;
			item.BeginGroup = true;
			PopulateChildItems(item);
			return item;
		}
		public override Type GetDefaultItemType() {
			return typeof(FileManagerDetailsColumn);
		}
		protected override internal string GetNavBarItemsGroupName() {
			return "Details View Settings Columns";
		}
		public void CreateDefaultColumns() {
			BeginUpdate();
			(Items as FileManagerDetailsColumnCollection).CreateDefaultColumns();
			SetFocusedItem(Items.OfType<IDesignTimeCollectionItem>().FirstOrDefault(), true);
			EndUpdate();
		}
		public override List<string> GetViewDependedProperties() {
			var list = base.GetViewDependedProperties();
			list.Add("FileInfoType");
			return list;
		}
	}
	public partial class DetailsViewSettingsColumnsEditorFrame : ItemsEditorFrame {
		public DetailsViewSettingsColumnsEditorFrame()
			: base() {
		}
		public DetailsViewSettingsColumnsEditorFrame(DetailsViewSettingsColumnsOwner columnsOwner) 
			: base(columnsOwner) {
		}
		public DetailsViewSettingsColumnsOwner ColumnsOwner { get { return ItemsOwner as DetailsViewSettingsColumnsOwner; } }
		protected override void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			if(ItemsOwner.TreeListItems.Count > 0) {
				DialogResult dialogResult = MessageBox.Show(string.Format(string.Format("Do you want to delete existing columns?")),
					string.Format("Create default columns "), MessageBoxButtons.YesNoCancel);
				if(dialogResult == DialogResult.Cancel)
					return;
				if(dialogResult == DialogResult.Yes) {
					ItemsOwner.SetSelection(ItemsOwner.TreeListItemsHash.Keys.ToList());
					ItemsOwner.RemoveSelectedItems();
				}
			}
			ColumnsOwner.CreateDefaultColumns();
		}
	}
	public partial class ToolbarSettingsItemsEditorFrame : ItemsEditorFrame {
		public ToolbarSettingsItemsEditorFrame()
			: base() {
		}
		public ToolbarSettingsItemsEditorFrame(FileManagerToolbarItemsOwner itemsOwner)
			: base(itemsOwner) {
		}
		public FileManagerToolbarItemsOwner ToolbarItemsOwner { get { return ItemsOwner as FileManagerToolbarItemsOwner; } }
		protected override void MenuItemClick_CreateDefaultItems(DesignEditorDescriptorItem menuItem) {
			if(ItemsOwner.TreeListItems.Count > 0) {
				DialogResult dialogResult = MessageBox.Show(string.Format(string.Format("Do you want to delete existing items?")),
					string.Format("Create default items "), MessageBoxButtons.YesNoCancel);
				if(dialogResult == DialogResult.Cancel)
					return;
				if(dialogResult == DialogResult.Yes) {
					ItemsOwner.SetSelection(ItemsOwner.TreeListItemsHash.Keys.ToList());
					ItemsOwner.RemoveSelectedItems();
				}
			}
			ToolbarItemsOwner.CreateDefaultItems();
		}
	}
	public class DocumentSelectorPermissionSettingsItemsOwner : FileManagerAccessRulesOwner {
		public DocumentSelectorPermissionSettingsItemsOwner(object component, IServiceProvider provider, AccessRulesCollection accessRules)
			: base(component, provider, accessRules) {
		}
		public override string GetDependendPropertyName() {
			return "SettingsDocumentSelector.PermissionSettings.Role";
		}
	}
}
