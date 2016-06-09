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
using System.Text;
using DevExpress.Web;
using System.Collections;
using DevExpress.Web.Internal;
using System.IO;
using System.Linq;
using DevExpress.Web.Localization;
using DevExpress.Utils.Zip;
namespace DevExpress.Web.Internal {
	public enum FileManagerCommandId {
		GetFileList = 0,
		Refresh = 1,
		DeleteItems = 2,
		RenameItem = 3,
		ShowFolderBrowserDialog = 4,
		MoveItems = 5,
		CreateQuery = 6,
		Create = 7,
		FoldersTvCallback = 8,
		FolderBrowserFoldersTvCallback = 9,
		Download = 10,
		ServerProcessFileOpened = 11,
		GridView = 12,
		ChangeFolderTvCallback = 13,
		CopyItems = 14,
		CustomCallback = 15,
		ChangeCurrentFolderCallback = 16,
		VirtualScrolling = 17,
		GridViewVirtualScrollingCallback = 18,
		ApiCommandCallback = 19
	}
	public static class FileManagerCommandsHelper {
		public const string ArgumentSeparator = "|";
		public const string LinkedArgumentSeparator = "||ls||";
		public const string ItemPropertySeparator = "::";
		public static FileManagerCommand CreateCommand(ASPxFileManager fileManager, FileManagerCommandId commandId, string commandArgs) {
			switch(commandId) {
				case FileManagerCommandId.GetFileList:
					return new FileManagerGetFileListCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.Refresh:
					return new FileManagerRefreshCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.DeleteItems:
					return new FileManagerDeleteItemsCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.RenameItem:
					return new FileManagerRenameItemCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.ShowFolderBrowserDialog:
					return new FileManagerShowFolderBrowserDialogCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.MoveItems:
					return new FileManagerMoveItemsCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.CreateQuery:
					return new FileManagerCreateQueryCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.Create:
					return new FileManagerCreateCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.FoldersTvCallback:
					return new FileManagerFoldersTvCallbackCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.FolderBrowserFoldersTvCallback:
					return new FileManagerFolderBrowserFoldersTvCallbackCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.Download:
					return new FileManagerDownloadCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.ServerProcessFileOpened:
					return new FileManagerServerProcessFileOpenedCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.GridView:
					return new FileManagerGridViewCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.ChangeFolderTvCallback:
					return new FileManagerChangeFolderTvCallbackCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.CopyItems:
					return new FileManagerCopyItemsCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.CustomCallback:
					return new FileManagerCustomCallback().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.ChangeCurrentFolderCallback:
					return new FileManagerChangeCurrentFolderCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.VirtualScrolling:
					return new FileManagerVirtualScrollingCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.GridViewVirtualScrollingCallback:
					return new FileManagerGridViewVirtualScrollingCommand().Initialize(fileManager, commandId, commandArgs);
				case FileManagerCommandId.ApiCommandCallback:
					return new FileManagerApiCommandCallback().Initialize(fileManager, commandId, commandArgs);
				default:
					throw new NotImplementedException();
			}
		}
	}
	public abstract class FileManagerCommand {
		public const string
			ItemsListParam = "items",
			ItemsListRequestParam = "itemRequest",
			ItemsRenderParam = "itemsRender",
			ThumbnailsList = "thumbnails",
			PathParam = "path",
			AllowUploadParam = "allowUpload",
			FoldersRenderParam = "foldersRender",
			CommandParam = "command",
			EditErrorText = "editErrorText",
			FolderBrowserFoldersRenderParam = "folderBrowserFoldersRender",
			IsSuccess = "isSuccess",
			EditErrorCode = "editErrorCode",
			FolderRightsParam = "folderRights",
			FilesRulesParam = "filesRules",
			UploadSuccessParam = "uploadSuccess",
			UploadErrorCodeParam = "errorCode", 
			UploadErrorTextParam = "uploadErrorText",
			SelectedAreaParam = "selectedArea",
			GridViewParam = "gridViewResult",
			IsNewFileList = "isNewFileList",
			SelectedItems = "selectedItems",
			TreeViewCallbackResult = "treeViewResult",
			CustomCallbackResult = "customCallbackResult",
			ApiCommandCallbackResult = "apiCommandResult",
			ChangeCurrentFolderParam = "changeCurrentFolder",
			CurrentFolderId = "currentFolderId",
			VirtScrollItemIndexParam = "virtScrollItemIndex",
			VirtScrollPageItemsCountParam = "virtScrollPageItemsCount",
			VirtScrollResetStateParam = "virtScrollResetState",
			ItemsCountParam = "itemsCount";
		ASPxFileManager fileManager;
		FileManagerCommandId commandId;
		Hashtable result;
		string commandArgs;
		protected internal FileManagerCommand Initialize(ASPxFileManager fileManager, FileManagerCommandId commandId, string commandArgs) {
			this.fileManager = fileManager;
			this.commandId = commandId;
			this.commandArgs = commandArgs;
			this.result = new Hashtable();
			FileManager.Helper.ClientState.SyncCurrentPath(); 
			return this;
		}
		protected ASPxFileManager FileManager { get { return fileManager; } }
		protected FileManagerHelper Helper { get { return FileManager.Helper; } }
		protected internal FileManagerCommandId CommandId { get { return commandId; } }
		protected Hashtable Result { get { return result; } }
		protected string CommandArgs { get { return commandArgs; } }
		protected string[] SplittedArgs { get { return SplitArgs(CommandArgs, FileManagerCommandsHelper.ArgumentSeparator); } }
		protected IEnumerable<string> SelectedItemsIds {
			get { return Helper.Data.SelectedItemsIds; }
			set { Helper.Data.SelectedItemsIds = value; }
		}
		public virtual void Execute() { }
		public virtual object GetCallbackResult() {
			Result.Add(CommandParam, (int)CommandId);
			ProcessAddItemsListRequest();
			if(FileManager.SettingsFileList.View == FileListView.Details && Result.ContainsKey(ItemsListParam))
				Result.Add(GridViewParam, FileManager.FilesGridView.GetCallbackResultCore());
			if(!FileManager.SettingsFolders.Visible && (FileManager.SettingsFileList.ShowFolders || FileManager.SettingsFileList.ShowParentFolder || FileManager.SettingsBreadcrumbs.Visible))
				Result.Add(FileManagerCommand.ChangeCurrentFolderParam, FileManager.SelectedFolder.RelativeName);
			return Result;
		}
		void ProcessAddItemsListRequest() {
			if(Result.ContainsKey(ItemsListRequestParam)) {
				bool isNewList = (bool)Result[ItemsListRequestParam];
				Result.Remove(ItemsListRequestParam);
				OnAddItemsListRequest(isNewList);
			}
		}
		protected virtual void OnAddItemsListRequest(bool isNewList) {
			AddItemsList(isNewList);
		}
		protected void AddItemsListRequest(bool isNewFileList) {
			Result.Add(ItemsListRequestParam, isNewFileList);
		}
		protected void AddItemsList(bool isNewFileList) {
			AddItemsList(isNewFileList, true);
		}
		protected void AddItemsList(bool isNewFileList, bool immediately) {
			if(isNewFileList)
				ResetItemFilter();
			IEnumerable<string> selectedItemsIds = null;
			if(FileManager.Helper.Data.ForcedlySelectedFiles != null)
				selectedItemsIds = FileManager.Helper.Data.GetVisibleForcedlySelectedFilesIds().ToArray();
			else if(SelectedItemsIds.Count() > 0)   
				selectedItemsIds = SelectedItemsIds;
			if(selectedItemsIds != null)
				Result[SelectedItems] = selectedItemsIds;
			AddItemsListCore(immediately);
			if(FileManager.Settings.UseAppRelativePath)
				Result.Add(PathParam, FileManager.Helper.GetCurrentRelativePath());
			Result.Add(AllowUploadParam, FileManager.FileSystemProvider.CanUpload(FileManager.SelectedFolder));
			Result.Add(FolderRightsParam, FileManager.Helper.Data.GetClientFolderRightsScript(FileManager.SelectedFolder));
			Result.Add(IsNewFileList, isNewFileList);
			Result.Add(CurrentFolderId, FileManager.GetCurrentFolderId());
		}
		protected void AddItemsListCore(bool immediately) {
			Result.Add(ItemsListParam, Helper.Data.GetItemsClientHashtable(immediately));
			if(FileManager.IsThumbnailsViewFileAreaItemTemplate) {
				FileManager.Items.LayoutChanged();
				Result.Add(ItemsRenderParam, FileManager.GetCallbackContentControlResult());
			}
			Result.Add(ThumbnailsList, Helper.Data.CustomThumbnails);
			if(FileManager.IsClientUploadAccessRulesValidationEnabled()) {
				var filesRules = FileManager.Helper.Data.GetFilesAccessRulesClientArray(FileManager.SelectedFolder);
				if(filesRules != null)
					Result.Add(FilesRulesParam, filesRules);
			}
			AddVirtScrollResult();
		}
		protected void AddVirtScrollResult() {
			if(FileManager.IsVirtualScrollingEnabled()) {
				Result.Add(VirtScrollItemIndexParam, FileManager.Helper.Data.VirtScrollItemIndex);
				Result.Add(VirtScrollPageItemsCountParam, FileManager.Helper.Data.VirtScrollPageItemsCount);
				Result.Add(ItemsCountParam, FileManager.Helper.Data.ItemsCount);
			}
		}
		protected void ResetItemFilter() {
			Helper.ClientState.ResetItemFilter();
			if(!FileManager.IsThumbnailsViewMode)
				FileManager.FilesGridView.FilterExpression = string.Empty;
		}
		protected void ResetVirtScrollState() {
			Helper.Data.ResetVirtScrollState();
			Result.Add(VirtScrollResetStateParam, true);
		}
		protected void AddFoldersList() {
			if(FileManager.SettingsFolders.Visible && FileManager.IsCallBacksEnabled())
				Result.Add(FoldersRenderParam, RenderUtils.GetRenderResult(FileManager.Folders));
		}
		protected void AddErrorInfo(Exception e) {
			AppendResult(EditErrorCode, GetErrorCodeFromException(e));
			AppendResult(EditErrorText, FileManager.HandleCallbackException(e));
			if(!Result.ContainsKey(IsSuccess))
				Result.Add(IsSuccess, false);
			else
				Result[IsSuccess] = false;
		}
		protected internal static int GetErrorCodeFromException(Exception e) {
			return ASPxFileManager.IsFileManagerException(e) ? (int)((FileManagerException)e).Error : (int)FileManagerErrors.Unspecified;
		}
		protected bool ContainsErrorInfo() {
			return Result.ContainsKey(IsSuccess) && !(bool)Result[IsSuccess];
		}
		protected void AddSuccessInfo() {
			if(!Result.ContainsKey(IsSuccess))
				Result.Add(IsSuccess, true);
		}
		void AppendResult(string key, object value) {
			if(Result.ContainsKey(key))
				Result[key] = Result[key].ToString() + "|" + value.ToString();
			else
				Result.Add(key, value.ToString());
		}
		protected void ValidateStateSynchronizer() {
			FileManager.Helper.Edit.ValidateStateSynchronizer();
		}
		protected string[] SplitArgs(string args, string separator) {
			return args.Split(new string[] { separator }, StringSplitOptions.None); 
		}
		protected string[] SplitItemProperties(string itemProperties) {
			return SplitArgs(itemProperties, FileManagerCommandsHelper.ItemPropertySeparator);
		}
	}
	public class FileManagerGetFileListCommand : FileManagerCommand {
		public override void Execute() {
			try {
				ValidateStateSynchronizer();
				ResetVirtScrollState();
				AddSuccessInfo();
				if(!string.IsNullOrEmpty(CommandArgs))
					SelectedItemsIds = new string[] { CommandArgs };
			}
			catch(Exception e) {
				AddErrorInfo(e);
				AddFoldersList();
			}
			AddItemsListRequest(true);
		}
	}
	public class FileManagerRefreshCommand : FileManagerCommand {
		public override void Execute() {
			try {
				ValidateStateSynchronizer();
				AddSuccessInfo();
			}
			catch(Exception e) {
				AddErrorInfo(e);
			}
			AddFoldersList();
			AddItemsListRequest(true);
		}
	}
	public class FileManagerDeleteItemsCommand : FileManagerCommand {
		FileManagerItem[] DeletedItems { get; set; }
		public override void Execute() {
			try {
				ValidateStateSynchronizer();
				if(bool.Parse(SplittedArgs[0]))
					DeletedSelectedFolder();
				else
					DeleteSelectedItems();
				if(DeletedItems.Length > 0)
					FileManager.RaiseItemsDeleted(DeletedItems);
				AddSuccessInfo();
			}
			catch(Exception e) {
				AddErrorInfo(e);
			}
			AddFoldersList();
			AddItemsListRequest(true);
		}
		void DeleteSelectedItems() {
			string[] itemNames = SplitItemProperties(SplittedArgs[1]);
			string[] itemIDs = SplitItemProperties(SplittedArgs[2]);
			if(itemNames.Length > 1 && !FileManager.Settings.EnableMultiSelect)
				throw new FileManagerException(FileManagerErrors.Unspecified);
			FileManagerFolder parentFolder = FileManager.SelectedFolder;
			string[] itemIsFolderProperties = SplittedArgs[3].Split(new string[] { FileManagerCommandsHelper.ItemPropertySeparator }, StringSplitOptions.None);
			List<FileManagerItem> deletedItemsList = new List<FileManagerItem>();
			for(int i = 0; i < itemNames.Length; i++) {
				try {
					string itemName = itemNames[i];
					string itemID = itemIDs[i];
					string[] itemIdPath = FileManager.Helper.Data.GetItemIdPath(parentFolder.RelativeName, itemID);
					FileManagerItem deletedItem;
					if(bool.Parse(itemIsFolderProperties[i])) {
						string targetPath = Path.Combine(parentFolder.RelativeName, itemName);
						FileManagerFolder targetFolder = new FileManagerFolder(FileManager.FileSystemProvider, targetPath, itemIdPath);
						FileManager.Helper.Edit.DeleteFolder(targetFolder);
						deletedItem = targetFolder;
					}
					else {
						FileManagerFile targetFile = FileManagerFile.Create(FileManager.FileSystemProvider, parentFolder, itemName, itemIdPath);
						FileManager.Helper.Edit.DeleteFile(targetFile);
						deletedItem = targetFile;
					}
					deletedItemsList.Add(deletedItem);
				}
				catch(Exception e) {
					AddErrorInfo(e);
				}
			}
			if(itemIsFolderProperties.Contains("true"))
				FileManager.Folders.RepopulateTree(true);
			DeletedItems = deletedItemsList.ToArray();
		}
		void DeletedSelectedFolder() {
			FileManagerFolder selectedFolder = FileManager.SelectedFolder;
			FileManagerFolder parentFolder = selectedFolder.Parent;
			DeletedItems = new FileManagerItem[] { selectedFolder };
			FileManager.Helper.Edit.DeleteFolder(selectedFolder);
			if(FileManager.Folders != null)
				FileManager.Folders.SelectedNode.Visible = false;
			FileManager.Helper.Data.SelectFolder(FileManager.Folders, parentFolder, true);
			ResetVirtScrollState();
		}
	}
	public class FileManagerRenameItemCommand : FileManagerCommand {
		FileManagerItem RenamedItem { get; set; }
		string TargetItemName { get; set; }
		public override void Execute() {
			try {
				ValidateStateSynchronizer();
				bool isFolderOperation = bool.Parse(SplittedArgs[0]);
				string name = SplittedArgs[1].Trim(' ', '.');
				if(isFolderOperation)
					RenameSelectedFolder(name);
				else
					isFolderOperation = RenameSelectedItem(name);
				FileManager.RaiseItemRenamed(TargetItemName, RenamedItem);
				if(isFolderOperation)
					AddFoldersList();
				AddSuccessInfo();
			}
			catch(Exception e) {
				AddErrorInfo(e);
			}
			AddItemsListRequest(true);
		}
		void RenameSelectedFolder(string name) {
			FileManagerFolder targetFolder = FileManager.SelectedFolder;
			FileManagerFolder parentFolder = targetFolder.Parent;
			TargetItemName = targetFolder.Name;
			FileManager.Helper.Edit.RenameFolder(targetFolder, name);
			FileManager.Folders.RepopulateTree(true);
			FileManagerFolder renamedFolder = new FileManagerFolder(FileManager.FileSystemProvider, parentFolder, name);
			FileManager.Helper.Data.SelectFolder(FileManager.Folders, renamedFolder, true);
			RenamedItem = renamedFolder;
		}
		bool RenameSelectedItem(string newName) {
			FileManagerFolder selectedFolder = FileManager.SelectedFolder;
			bool isFolderOperation = bool.Parse(SplittedArgs[3]);
			string itemId = SplittedArgs[2];
			string[] itemIdPath = Helper.Data.GetItemIdPath(selectedFolder.RelativeName, itemId);
			TargetItemName = FileManager.Helper.ClientState.GetSelectedItemsInfo(isFolderOperation)[0].Name;
			if(isFolderOperation) {
				FileManagerFolder targetFolder = FileManagerFolder.Create(FileManager.FileSystemProvider, selectedFolder, TargetItemName, itemIdPath);
				FileManager.Helper.Edit.RenameFolder(targetFolder, newName);
				RenamedItem = FileManagerFolder.Create(FileManager.FileSystemProvider, selectedFolder, newName, itemIdPath);
				FileManager.Folders.RepopulateTree(true);
			}
			else {
				FileManagerFile targetFile = FileManagerFile.Create(FileManager.FileSystemProvider, selectedFolder, TargetItemName, itemIdPath);
				FileManager.Helper.Edit.RenameFile(newName, targetFile);
				RenamedItem = FileManagerFile.Create(FileManager.FileSystemProvider, selectedFolder, newName, itemIdPath);
			}
			string renamedItemRelativeName = Path.Combine(selectedFolder.RelativeName, newName);
			SelectedItemsIds = new string[] { renamedItemRelativeName };
			return isFolderOperation;
		}
	}
	public class FileManagerShowFolderBrowserDialogCommand : FileManagerCommand {
		public bool IsCopyFolder { get { return CommandArgs == "true"; } }
		public bool IsFolderOperation {get {return CommandArgs != ""; } }
		public override void Execute() {
			if(!(FileManager.SettingsEditing.AllowMove || FileManager.SettingsEditing.AllowCopy))
				return;
			try {
				ValidateStateSynchronizer();
				Result.Add(SelectedAreaParam, !IsFolderOperation);
				FileManager.FolderBrowserFolders.Visible = true;
				FileManager.FolderBrowserFolders.RepopulateTree(false);
				FileManager.Helper.Data.DisableNode(FileManager.FolderBrowserFolders, FileManager.SelectedFolder, IsCopyFolder);
				if(IsFolderOperation && FileManager.SelectedFolder.Parent != null)
					FileManager.Helper.Data.DisableNode(FileManager.FolderBrowserFolders, FileManager.SelectedFolder.Parent, false);
				FileManager.Helper.Data.ExpandNode(FileManager.FolderBrowserFolders, FileManager.SelectedFolder, !IsFolderOperation || IsCopyFolder);
				if(FileManager.FolderBrowserFolders.IsVirtualMode())
					FileManager.Helper.Data.SetVirtualNodesEnabled(FileManager.FolderBrowserFolders.RootNode.Nodes);
				if(FileManager.IsNeedToAddCallbackCommandResult())
					Result.Add(FolderBrowserFoldersRenderParam, RenderUtils.GetRenderResult(FileManager.FolderBrowserFolders));
				AddSuccessInfo();
			}
			catch(Exception e) {
				AddErrorInfo(e);
				AddFoldersList();
				AddItemsListRequest(true);
			}
		}
	}
	public class FileManagerMoveItemsCommand : FileManagerChangePositionItemsCommand {
		protected override void ChangeFolderPosition(FileManagerFolder targetFolder, FileManagerFolder folderToMove) {
			FileManager.Helper.Edit.MoveFolder(targetFolder, folderToMove);
		}
		protected override void ChangeFilePosition(FileManagerFolder target, FileManagerFile file) {
			FileManager.Helper.Edit.MoveFile(target, file);
		}
		protected override void OnItemsPositionChanged(FileManagerFolder sourceFolder, FileManagerItem[] items) {
			FileManager.RaiseItemsMoved(sourceFolder, items);
		}
		protected override bool NeedSaveSourceItemId() {
			return true;
		}
	}
	public class FileManagerCopyItemsCommand : FileManagerChangePositionItemsCommand {
		protected override void ChangeFolderPosition(FileManagerFolder targetFolder, FileManagerFolder folderToCopy) {
			FileManager.Helper.Edit.CopyFolder(targetFolder, folderToCopy);
		}
		protected override void ChangeFilePosition(FileManagerFolder target, FileManagerFile file) {
			FileManager.Helper.Edit.CopyFile(target, file);
		}
		protected override void OnItemsPositionChanged(FileManagerFolder sourceFolder, FileManagerItem[] items) {
			FileManager.RaiseItemsCopied(sourceFolder, items);
		}
		protected override bool NeedSaveSourceItemId() {
			return false;
		}
	}
	public abstract class FileManagerChangePositionItemsCommand : FileManagerCommand {
		List<FileManagerItem> ProcessedItems { get; set; }
		public override void Execute() {
			try {
				ValidateStateSynchronizer();
				string[] splittedFirstArg = SplitItemProperties(SplittedArgs[0]);
				string targetPath = splittedFirstArg[1];
				string targetIdPathStr = splittedFirstArg[2];
				string[] targetIdPath = targetIdPathStr.Split(new string[] { "\\\\" }, StringSplitOptions.None);
				FileManagerFolder sourceFolder = FileManager.SelectedFolder;
				FileManagerFolder targetFolder = new FileManagerFolder(FileManager.FileSystemProvider, targetPath, targetIdPath);
				Helper.Data.RegisterFolderIdPath(targetPath, targetIdPath);
				try {
					if(bool.Parse(splittedFirstArg[0])) {
						sourceFolder = sourceFolder.Parent;
						ChangePositionForSelectedFolder(targetFolder);
					}
					else
						ChangePositionForSelectedItems(targetFolder);
				}
				catch(Exception e) {
					AddErrorInfo(e);
				}
				if(ProcessedItems != null && ProcessedItems.Count > 0)
					OnItemsPositionChanged(sourceFolder, ProcessedItems.ToArray());
				FileManager.Folders.RepopulateTree(true);
				FileManager.Helper.Data.SelectFolder(FileManager.Folders, targetFolder, true);
				AddSuccessInfo();
			}
			catch(Exception e) {
				AddErrorInfo(e);
			}
			AddFoldersList();
			AddItemsListRequest(true);
		}
		protected abstract void ChangeFolderPosition(FileManagerFolder targetFolder, FileManagerFolder folderToCopy);
		protected abstract void ChangeFilePosition(FileManagerFolder target, FileManagerFile file);
		protected abstract void OnItemsPositionChanged(FileManagerFolder sourceFolder, FileManagerItem[] items);
		protected abstract bool NeedSaveSourceItemId();
		void ChangePositionForSelectedFolder(FileManagerFolder targetFolder) {
			FileManagerFolder selectedFolder = FileManager.SelectedFolder;
			FileManagerFolder parentFolder = selectedFolder.Parent;
			ChangeFolderPosition(targetFolder, selectedFolder);
			FileManagerFolder processedFolder = new FileManagerFolder(FileManager.FileSystemProvider, targetFolder, selectedFolder.Name);
			ProcessedItems = new List<FileManagerItem> { processedFolder };
		}
		void ChangePositionForSelectedItems(FileManagerFolder targetFolder) {
			string[] itemNames = SplitItemProperties(SplittedArgs[1]);
			if(itemNames.Length > 1 && !FileManager.Settings.EnableMultiSelect)
				throw new FileManagerException(FileManagerErrors.Unspecified);
			string[] itemIDs = SplitItemProperties(SplittedArgs[2]);
			string[] itemIsFolderProperties = SplitItemProperties(SplittedArgs[3]);
			int itemCount = itemNames.Length;
			string[] needSelectItemIDs = new string[itemCount];
			FileManagerFolder selectedFolder = FileManager.SelectedFolder;
			ProcessedItems = new List<FileManagerItem>();
			for(int i = 0; i < itemCount; i++) {
				string itemName = itemNames[i];
				string itemID = itemIDs[i];
				string[] itemIdPath = Helper.Data.GetItemIdPath(selectedFolder.RelativeName, itemID);
				FileManagerItem processedItem;
				if(bool.Parse(itemIsFolderProperties[i])) {
					FileManagerFolder folder = FileManagerFolder.Create(FileManager.FileSystemProvider, selectedFolder, itemName, itemIdPath);
					ChangeFolderPosition(targetFolder, folder);
					processedItem = new FileManagerFolder(FileManager.FileSystemProvider, targetFolder, itemName, GetProcessedItemId(itemID));
				}
				else {
					FileManagerFile file = FileManagerFile.Create(FileManager.FileSystemProvider, selectedFolder, itemName, itemIdPath);
					ChangeFilePosition(targetFolder, file);
					processedItem = new FileManagerFile(FileManager.FileSystemProvider, targetFolder, itemName, GetProcessedItemId(itemID));
				}
				ProcessedItems.Add(processedItem);
				needSelectItemIDs[i] = Path.Combine(targetFolder.RelativeName, itemName);
			}
			SelectedItemsIds = needSelectItemIDs;
		}
		string GetProcessedItemId(string sourceItemId) {
			return NeedSaveSourceItemId() ? sourceItemId : string.Empty;
		}
	}
	public class FileManagerCreateQueryCommand : FileManagerCommand {
		public override void Execute() {
			if(!FileManager.SettingsEditing.AllowCreate)
				return;
			try {
				ValidateStateSynchronizer();
				ResetVirtScrollState();
				if(FileManager.Settings.UseAppRelativePath)
					Result.Add(PathParam, FileManager.Helper.GetCurrentRelativePath() + "/New Folder");
				if(FileManager.Folders.EnableCallBacks) {
					FileManagerFolder selectedFolder = FileManager.SelectedFolder;
					FileManager.Folders.CreateNodePath = string.Format("{0}\\{1}", selectedFolder.RelativeName, FileManagerDataHelper.NewFolderNodeName);
					FileManager.Folders.CreateNodeParentName = selectedFolder.Id;
					var virtualNode = FileManager.Folders.SelectedNode as TreeViewVirtualNode;
					virtualNode.IsLeaf = false;
					virtualNode.Nodes.Clear();
					(FileManager.Folders.SelectedNode as TreeViewVirtualNode).ForceNodesPopulation();
					Helper.Data.SelectFolder(FileManager.Folders, FileManager.Folders.CreateNodePath, true, false);
				}
				else {
					TreeViewNode createNode = Helper.CreateCreateFolderNode();
					FileManager.Folders.SelectedNode.Nodes.Add(createNode);
					FileManager.Folders.SelectedNode = createNode;
					FileManager.Folders.ExpandToNode(createNode);
				}
				AddFoldersList();
				AddVirtScrollResult();
				Result.Add(ItemsListParam, new object[] { });
				Result.Add(IsNewFileList, true);
				Result.Add(AllowUploadParam, false);
				if(FileManager.FilesGridView != null)
					FileManager.FilesGridView.DataSource = new object[0];
				AddSuccessInfo();
			}
			catch(Exception e) {
				AddErrorInfo(e);
				AddFoldersList();
				AddItemsListRequest(true);
			}
		}
	}
	public class FileManagerCreateCommand : FileManagerCommand {
		public override void Execute() {
			try {
				var args = CommandArgs.Split(new string[] { FileManagerCommandsHelper.ArgumentSeparator }, 2, StringSplitOptions.None);
				var name = args[0].Trim(' ', '.');
				ValidateStateSynchronizer();
				Helper.Edit.CreateFolder(name);
				FileManagerFolder curFolder = FileManager.SelectedFolder;
				FileManagerFolder createdFolder = new FileManagerFolder(FileManager.FileSystemProvider, curFolder, name);
				FileManager.RaiseFolderCreated(createdFolder, curFolder);
				FileManager.Folders.RepopulateTree(false);
				if(bool.Parse(args[1]))
					FileManager.Helper.Data.SelectFolder(FileManager.Folders, createdFolder, true);
				else
					SelectedItemsIds = new string[] { createdFolder.RelativeName };
				AddSuccessInfo();
			}
			catch(Exception e) {
				FileManager.Helper.Data.SelectFolder(FileManager.Folders, FileManager.SelectedFolder, true);
				AddErrorInfo(e);
			}
			AddFoldersList();
			AddItemsListRequest(true);
		}
	}
	public class FileManagerChangeFolderTvCallbackCommand : FileManagerCommand {
		protected string TreeViewCallbackArgument { get; set; }
		public override void Execute() {
			try {
				ValidateStateSynchronizer();
				var args = CommandArgs.Split(new string[] { FileManagerCommandsHelper.ArgumentSeparator }, 2, StringSplitOptions.None);
				if(args.Length != 2)
					throw new Exception();
				TreeViewCallbackArgument = args[1];
				var folder = FileManager.Helper.Data.SelectFolder(FileManager.Folders, new FileManagerFolder(FileManager.FileSystemProvider, args[0]), true);
				if(!folder)
					throw new FileManagerException(FileManagerErrors.FolderNotFound);
				AddSuccessInfo();
				AddItemsListRequest(true);
			}
			catch(Exception e) {
				HandleException(e);
			}
		}
		protected override void OnAddItemsListRequest(bool isNewList) {
			try {
				AddItemsList(isNewList);
			}
			catch(Exception e) {
				HandleException(e);
			}
		}
		void HandleException(Exception e) {
			FileManager.Helper.Data.SelectFolder(FileManager.Folders, FileManager.SelectedFolder, true);
			AddErrorInfo(e);
		}
	}
	public class FileManagerFoldersTvCallbackCommand : FileManagerCommand {
		public override object GetCallbackResult() {
			return FileManager.Folders.GetCallbackResult(CommandArgs);
		}
	}
	public class FileManagerFolderBrowserFoldersTvCallbackCommand : FileManagerCommand {
		public override void Execute() {
			FileManager.FolderBrowserFolders.Visible = true;
			if(!FileManager.FolderBrowserFolders.EnableCallBacks)
				FileManager.FolderBrowserFolders.RepopulateTree(false);
		}
		public override object GetCallbackResult() {
			return FileManager.FolderBrowserFolders.GetCallbackResult(CommandArgs);
		}
	}
	public class FileManagerDownloadCommand : FileManagerCommand {
		string[] fileNames;
		string[] fileIDs;
		string[] FileNames {
			get {
				if(fileNames == null)
					fileNames = SplitItemProperties(SplittedArgs[0]);
				return fileNames;
			}
		}
		string[] FileIDs {
			get {
				if(fileIDs == null)
					fileIDs = SplitItemProperties(SplittedArgs[1]);
				return fileIDs;
			}
		}
		FileSystemProviderBase Provider { get { return FileManager.FileSystemProvider.Provider; } }
		public override void Execute() {
			Helper.Edit.DownloadError = null;
			var cloudProvider = Provider as CloudFileSystemProviderBase;
			bool useDownloadUrl = FileNames.Length == 1 && cloudProvider != null;
			try {
				if(useDownloadUrl) {
					string downloadUrl = GetDownloadUrl();
					FileManager.Page.Response.Redirect(downloadUrl);
				}
				else {
					string fileName;
					string fileFormat;
					using(Stream downloadStream = GetDownloadStream(out fileName, out fileFormat)) {
						if(downloadStream != null)
							WriteStreamToResponse(downloadStream, fileName, fileFormat);
					}
				}
			}
			catch(Exception e) {
				Helper.Edit.DownloadError = GetDownloadError(e);
			}
			finally {
				if(!useDownloadUrl)
					FileManager.Helper.Edit.ClenUpDownloadTempDirectory();
			}
		}
		protected Stream GetDownloadStream(out string fileName, out string fileFormat) {
			if(FileNames.Length == 1)
				return FileManager.Helper.Edit.DownloadFile(FileNames[0], FileIDs[0], out fileName, out fileFormat);
			else
				return FileManager.Helper.Edit.DownloadFile(FileNames, FileIDs, out fileName, out fileFormat);
		}
		protected string GetDownloadUrl() {
			return FileManager.Helper.Edit.GetCloudDownloadUrl(FileNames[0], FileIDs[0]);
		}
		string GetDownloadError(Exception e) {
			return HtmlConvertor.ToJSON(new Hashtable() {
				{ EditErrorText, FileManager.HandleCallbackException(e) },
				{ EditErrorCode, GetErrorCodeFromException(e) }
			});
		}
		protected virtual void WriteStreamToResponse(Stream stream, string fileName, string fileExt) {
			HttpUtils.WriteFileToResponse(FileManager.Page, stream, fileName, true, fileExt, HttpUtils.GetContentType(fileExt), true);
		}
		public override object GetCallbackResult() {
			return string.Empty;
		}
	}
	public class FileManagerCustomCallback : FileManagerCommand {
		public override void Execute() {
			try {
				FileManager.RaiseCustomCallback(new CallbackEventArgsBase(CommandArgs));
				ValidateStateSynchronizer();
				AddSuccessInfo();
				SelectedItemsIds = Helper.ClientState.GetSelectedItemsIds();
			} catch(Exception e) {
				AddErrorInfo(e);
			}
		}
		public override object GetCallbackResult() {
			if(FileManager.IsThumbnailsViewMode)
				AddItemsList(true);
			AddFoldersList();
			Result.Add(CommandParam, (int)CommandId);
			var renderResult = "";
			if(!FileManager.IsThumbnailsViewMode) {
				FileManager.FilesGridView.DataBind();
				AddItemsList(true);
				renderResult += RenderUtils.GetRenderResult(FileManager.FilesGridView as ASPxWebControlBase);
			}
			renderResult += RenderUtils.GetScriptHtml(string.Format("{0}.AfterCustomCallback();\r\n", FileManager.GetClientInstanceName()));
			Result.Add(FileManagerCommand.CustomCallbackResult, renderResult + FileManagerCommandsHelper.LinkedArgumentSeparator +
				(FileManager.SettingsFileList.View == FileListView.Thumbnails ? "Thumbnail" : "Grid") + FileManagerCommandsHelper.LinkedArgumentSeparator +
				HtmlConvertor.ToJSON(Helper.GetClientScriptStylesObject()));
			return Result;
		}
	}
	public class FileManagerApiCommandCallback : FileManagerCommand {
		public override void Execute() {
			try {
				ValidateStateSynchronizer();
				AddSuccessInfo();
			}
			catch(Exception e) {
				AddErrorInfo(e);
			}
		}
		public override object GetCallbackResult() {
			ResetItemFilter();
			Result.Add(CommandParam, (int)CommandId);
			Result.Add(FileManagerCommand.ApiCommandCallbackResult, Helper.Data.GetApiCallAllItemsClientHashtable());
			return Result;
		}
	}
	public class FileManagerChangeCurrentFolderCommand : FileManagerCommand {
		public override void Execute() {
			try {
				ValidateStateSynchronizer();
				FileManager.Helper.Data.SelectFolder(null, new FileManagerFolder(FileManager.FileSystemProvider, SplittedArgs[0], SplittedArgs[1]), true);
				AddSuccessInfo();
			} catch(Exception e) {
				AddErrorInfo(e);
			}
			AddItemsListRequest(true);
		}
	}
	public class FileManagerVirtualScrollingCommand : FileManagerCommand {
		public override void Execute() {
			try {
				int itemIndex = 0;
				int pageItemsCount = FileManager.SettingsFileList.PageSize;
				if(!string.IsNullOrEmpty(CommandArgs)) {
					itemIndex = int.Parse(SplittedArgs[0]);
					pageItemsCount = int.Parse(SplittedArgs[1]);
				}
				FileManager.Helper.Data.VirtScrollItemIndex = itemIndex;
				FileManager.Helper.Data.VirtScrollPageItemsCount = pageItemsCount;
				ValidateStateSynchronizer();
				AddItemsListRequest(false);
				AddSuccessInfo();
			}
			catch(Exception e) {
				HandleException(e);
			}
		}
		protected override void OnAddItemsListRequest(bool isNewList) {
			try {
				AddItemsList(isNewList);
			}
			catch(Exception e) {
				HandleException(e);
			}
		}
		void HandleException(Exception e) {
			AddErrorInfo(e);
		}
	}
	public class FileManagerServerProcessFileOpenedCommand : FileManagerCommand {
		public override void Execute() {
			if(FileManager.SelectedFile != null)
				FileManager.RaiseSelectedFileOpened(FileManager.SelectedFile);
		}
		public override object GetCallbackResult() {
			return string.Empty;
		}
	}
	public class FileManagerGridViewCommand : FileManagerCommand {
		public override void Execute() {
			AddItemsListRequest(false);
			FileManager.FilesGridView.DataBind();
			FileManager.FilesGridView.RaiseCallbackEventCore(CommandArgs);
		}
	}
	public class FileManagerGridViewVirtualScrollingCommand : FileManagerCommand {
		public override void Execute() {
			FileManager.FilesGridView.RaiseCallbackEventCore(CommandArgs);
			FileManager.FilesGridView.DataSource = FileManager.Helper.Data.GetItemsList(true);
			FileManager.FilesGridView.DataBind();
			AddItemsListRequest(false);
		}
	}
	public class FileManagerUploadCommand {
		ASPxFileManager FileManager { get; set; }
		List<string> UploadedFilesNames { get; set; }
		public FileManagerUploadCommand(ASPxFileManager fileManager) {
			FileManager = fileManager;
			UploadedFilesNames = new List<string>();
		}
		public void UploadFile(FileUploadCompleteEventArgs eventArgs) {
			if(eventArgs.IsValid) {
				try {
					string fileName = FileManager.Helper.Edit.UploadFile(eventArgs.UploadedFile);
					UploadedFilesNames.Add(fileName);
				}
				catch(Exception exc) {
					eventArgs.ErrorText = FileManager.HandleCallbackException(exc);
					eventArgs.IsValid = false;
					int errorCode = FileManagerCommand.GetErrorCodeFromException(exc);
					eventArgs.CallbackData = errorCode.ToString(); 
				}
			}
		}
		public string GetCallbackResult(ASPxUploadControl uploadControl) {
			OnFilesUploaded();
			Hashtable result = new Hashtable();
			StringBuilder errorText = new StringBuilder();
			bool isSuccess = true;
			if(uploadControl.UploadedFiles.Length == 1) {
				isSuccess = uploadControl.UploadedFiles[0].IsValid;
				errorText.Append(uploadControl.ErrorTexts[0]);
			}
			else {
				int errorsCount = 0;
				isSuccess = false;
				for(int i = 0; i < uploadControl.UploadedFiles.Length; i++) {
					if(!uploadControl.UploadedFiles[i].IsValid) {
						errorsCount++;
						errorText.AppendFormat(" '{0}' - {1}; \r\n", uploadControl.UploadedFiles[i].FileName, uploadControl.ErrorTexts[i]);
					}
					else
						isSuccess = true;
				}
				if(errorText.Length > 0) {
					errorText = errorText.Replace(";", ".", errorText.Length - 4, 1);
					errorText.Insert(0, string.Format(
						ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorUploadSeveralFiles) + ":\r\n",
						errorsCount, uploadControl.UploadedFiles.Length)
					);
				}
			}
			result.Add(FileManagerCommand.UploadSuccessParam, isSuccess);
			result.Add(FileManagerCommand.UploadErrorTextParam, errorText.ToString());
			string errorCode = Array.Find(uploadControl.CallbackDataArray, a => (!string.IsNullOrEmpty(a)));
			if(errorCode != null)
				result.Add(FileManagerCommand.UploadErrorCodeParam, errorCode); 
			return HtmlConvertor.ToJSON(result);
		}
		void OnFilesUploaded() {
			if(UploadedFilesNames.Count > 0) {
				FileManagerFolder parentFolder = FileManager.SelectedFolder;
				FileManagerFile[] files = UploadedFilesNames.
					Select(name => new FileManagerFile(FileManager.FileSystemProvider, parentFolder, name)).
					ToArray();
				FileManager.RaiseFilesUploaded(files);
			}
		}
	}
}
