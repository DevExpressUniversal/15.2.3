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

using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.Forms {
	public class SelectFolderDialog : SpreadsheetDialogBase {
		protected const string FileManagerID = "FileManager",
							   FileManagerClientName = "FileManager",
							   SelectedFileOpenedEventHandler = "ASPx.SpreadsheetDialog.SelectorCompleted",
							   SelectedCanceledEventHandler = "ASPx.SpreadsheetDialog.SelectorCanceled";
		protected SpreadsheetFolderManager SSFolderManager { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxssDlgSelectFolderForm";
		}
		protected override string GetContentTableID() {
			return "dxSelectFolderForm";
		}
		protected override void PopulateContentArea(Control container) {
			SSFolderManager = new SpreadsheetFolderManager();
			SSFolderManager.ID = FileManagerID;
			SSFolderManager.ClientInstanceName = FileManagerClientName;
			SSFolderManager.ClientSideEvents.SelectedFileOpened = SelectedFileOpenedEventHandler;
			container.Controls.Add(SSFolderManager);
			InitializeFileManager();
		}
		protected void InitializeFileManager() {
			SSFolderManager.Styles.CopyFrom(Spreadsheet.StylesFileManager);
			SSFolderManager.ControlStyle.CopyFrom(Spreadsheet.StylesFileManager.Control);
			SSFolderManager.Images.CopyFrom(Spreadsheet.ImagesFileManager);
			SSFolderManager.Settings.Assign(Spreadsheet.SettingsDocumentSelector.CommonSettings);
			SSFolderManager.SettingsEditing.Assign(Spreadsheet.SettingsDocumentSelector.EditingSettings);
			SSFolderManager.SettingsFolders.Assign(Spreadsheet.SettingsDocumentSelector.FoldersSettings);
			SSFolderManager.SettingsToolbar.Assign(Spreadsheet.SettingsDocumentSelector.ToolbarSettings);
			SSFolderManager.SettingsUpload.Assign(Spreadsheet.SettingsDocumentSelector.UploadSettings);
			SSFolderManager.SettingsPermissions.Assign(Spreadsheet.SettingsDocumentSelector.PermissionSettings);
			SSFolderManager.SettingsFileList.Assign(Spreadsheet.SettingsDocumentSelector.FileListSettings);
			SSFolderManager.Settings.RootFolder = Spreadsheet.GetWorkDirectory();
			SSFolderManager.FolderCreating += new FileManagerFolderCreateEventHandler(FileManager_FolderCreating);
			SSFolderManager.ItemDeleting += new FileManagerItemDeleteEventHandler(FileManager_ItemDeleting);
			SSFolderManager.ItemMoving += new FileManagerItemMoveEventHandler(FileManager_ItemMoving);
			SSFolderManager.ItemRenaming += new FileManagerItemRenameEventHandler(FileManager_ItemRenaming);
			SSFolderManager.FileUploading += new FileManagerFileUploadEventHandler(FileManager_FileUploading);
			SSFolderManager.ItemCopying += new FileManagerItemCopyEventHandler(FileManager_ItemCopying);
			SSFolderManager.CloudProviderRequest += new FileManagerCloudProviderRequestEventHandler(FileManager_CloudProviderRequest);
		}
		protected override string GetDefaultSubmitButtonInitEventHandler() {
			return SelectedFileOpenedEventHandler;
		}
		protected override string GetDefaultCancelButtonInitEventHandler() {
			return SelectedCanceledEventHandler;
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			SSFolderManager.Width = Unit.Pixel(800);
			SSFolderManager.Height = Unit.Pixel(500);
			SSFolderManager.Styles.FolderContainer.Width = Unit.Pixel(250);
			SSFolderManager.Styles.File.Margins.Margin = Unit.Pixel(1);
		}
		#region FileManagerEvents
		protected void FileManager_FolderCreating(object sender, FileManagerFolderCreateEventArgs e) {
			Spreadsheet.RaiseDocumentSelectorFolderCreating(e);
		}
		protected void FileManager_ItemDeleting(object sender, FileManagerItemDeleteEventArgs e) {
			Spreadsheet.RaiseDocumentSelectorItemDeleting(e);
		}
		protected void FileManager_ItemMoving(object sender, FileManagerItemMoveEventArgs e) {
			Spreadsheet.RaiseDocumentSelectorItemMoving(e);
		}
		protected void FileManager_ItemRenaming(object sender, FileManagerItemRenameEventArgs e) {
			Spreadsheet.RaiseDocumentSelectorItemRenaming(e);
		}
		protected void FileManager_FileUploading(object sender, FileManagerFileUploadEventArgs e) {
			Spreadsheet.RaiseDocumentSelectorFileUploading(e);
		}
		protected void FileManager_ItemCopying(object sender, FileManagerItemCopyEventArgs e) {
			Spreadsheet.RaiseDocumentSelectorItemCopying(e);
		}
		protected void FileManager_CloudProviderRequest(object source, FileManagerCloudProviderRequestEventArgs e) {
			Spreadsheet.RaiseDocumentSelectorCloudProviderRequest(e);
		}
		#endregion
	}
}
