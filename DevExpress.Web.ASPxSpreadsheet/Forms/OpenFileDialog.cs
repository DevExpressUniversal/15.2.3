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
	public class OpenFileDialog : SpreadsheetDialogBase {
		protected const string FileManagerID = "FileManager",
							   FileManagerClientName = "_dxFileManager",
							   SelectedFileChangedEvent = "ASPx.SpreadsheetDialog.OpenDialogSelectedFileChanged",
							   SelectedFileOpenedEvent = "ASPx.SpreadsheetDialog.OpenDialogSelectedFileOpened";
		protected SpreadsheetFileManager SSFileManager { get; private set; }
		protected override string GetDialogCssClassName() {
			return "dxssDlgOpenFileForm";
		}
		protected override string GetContentTableID() {
			return "dxSelectFileForm";
		}
		protected override void PopulateContentArea(Control container) {
			SSFileManager = Spreadsheet.CreateFileManager();
			SSFileManager.ID = FileManagerID;
			SSFileManager.ClientInstanceName = GetControlClientInstanceName(FileManagerClientName);
			SSFileManager.ClientSideEvents.SelectedFileChanged = SelectedFileChangedEvent;
			SSFileManager.ClientSideEvents.SelectedFileOpened = SelectedFileOpenedEvent;
			container.Controls.Add(SSFileManager);
			InitializeFileManager();
		}
		protected void InitializeFileManager() {
			SSFileManager.Styles.CopyFrom(Spreadsheet.StylesFileManager);
			SSFileManager.ControlStyle.CopyFrom(Spreadsheet.StylesFileManager.Control);
			SSFileManager.Images.CopyFrom(Spreadsheet.ImagesFileManager);
			SSFileManager.Settings.Assign(Spreadsheet.SettingsDocumentSelector.CommonSettings);
			SSFileManager.SettingsEditing.Assign(Spreadsheet.SettingsDocumentSelector.EditingSettings);
			SSFileManager.SettingsFolders.Assign(Spreadsheet.SettingsDocumentSelector.FoldersSettings);
			SSFileManager.SettingsToolbar.Assign(Spreadsheet.SettingsDocumentSelector.ToolbarSettings);
			SSFileManager.SettingsUpload.Assign(Spreadsheet.SettingsDocumentSelector.UploadSettings);
			SSFileManager.SettingsPermissions.Assign(Spreadsheet.SettingsDocumentSelector.PermissionSettings);
			SSFileManager.SettingsFileList.Assign(Spreadsheet.SettingsDocumentSelector.FileListSettings);
			SSFileManager.Settings.RootFolder = Spreadsheet.GetWorkDirectory();
			SSFileManager.FolderCreating += new FileManagerFolderCreateEventHandler(FileManager_FolderCreating);
			SSFileManager.ItemDeleting += new FileManagerItemDeleteEventHandler(FileManager_ItemDeleting);
			SSFileManager.ItemMoving += new FileManagerItemMoveEventHandler(FileManager_ItemMoving);
			SSFileManager.ItemRenaming += new FileManagerItemRenameEventHandler(FileManager_ItemRenaming);
			SSFileManager.FileUploading += new FileManagerFileUploadEventHandler(FileManager_FileUploading);
			SSFileManager.ItemCopying += new FileManagerItemCopyEventHandler(FileManager_ItemCopying);
			SSFileManager.CloudProviderRequest += new FileManagerCloudProviderRequestEventHandler(FileManager_CloudProviderRequest);
			SSFileManager.ClientSideEvents.CustomCommand = Spreadsheet.SettingsDocumentSelector.CustomCommand;
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			SSFileManager.Width = Unit.Pixel(800);
			SSFileManager.Height = Unit.Pixel(500);
			SSFileManager.Styles.FolderContainer.Width = Unit.Pixel(250);
			SSFileManager.Styles.File.Margins.Margin = Unit.Pixel(1);
			SSFileManager.Border.BorderStyle = BorderStyle.None;
		}
		protected override void PopulateFooterArea(Control container) {
			base.PopulateFooterArea(container);
			SubmitButton.ClientEnabled = false;
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
