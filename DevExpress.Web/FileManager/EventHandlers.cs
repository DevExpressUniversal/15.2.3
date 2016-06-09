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
using System.IO;
using System.Net;
using DevExpress.Data.Filtering;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public delegate void FileManagerFolderCreateEventHandler(object source, FileManagerFolderCreateEventArgs e);
	public class FileManagerActionEventArgsBase : EventArgs {
		bool cancel = false;
		string errorText;
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerActionEventArgsBaseCancel")]
#endif
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("FileManagerActionEventArgsBaseErrorText")]
#endif
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
	}
	public class FileManagerFolderCreateEventArgs : FileManagerActionEventArgsBase {
		string name;
		FileManagerFolder parentFolder;
		public FileManagerFolderCreateEventArgs(string name, FileManagerFolder parentFolder) {
			this.name = name;
			this.parentFolder = parentFolder;
		}
		public FileManagerFolder ParentFolder { get { return parentFolder; } }
		public string Name { get { return name; } }
	}
	public delegate void FileManagerFolderCreatedEventHandler(object source, FileManagerFolderCreatedEventArgs e);
	public class FileManagerFolderCreatedEventArgs : EventArgs {
		FileManagerFolder folder;
		FileManagerFolder parentFolder;
		public FileManagerFolderCreatedEventArgs(FileManagerFolder folder, FileManagerFolder parentFolder) {
			this.folder = folder;
			this.parentFolder = parentFolder;
		}
		public FileManagerFolder Folder { get { return folder; } }
		public FileManagerFolder ParentFolder { get { return parentFolder; } }
	}
	public delegate void FileManagerItemRenameEventHandler(object source, FileManagerItemRenameEventArgs e);
	public class FileManagerItemRenameEventArgs : FileManagerActionEventArgsBase {
		string newName;
		FileManagerItem item;
		public FileManagerItemRenameEventArgs(string newName, FileManagerItem item) {
			this.newName = newName;
			this.item = item;
		}
		public FileManagerItem Item { get { return item; } }
		public string NewName { get { return newName; } }
	}
	public delegate void FileManagerItemRenamedEventHandler(object source, FileManagerItemRenamedEventArgs e);
	public class FileManagerItemRenamedEventArgs : EventArgs {
		string oldName;
		FileManagerItem item;
		public FileManagerItemRenamedEventArgs(string oldName, FileManagerItem item) {
			this.oldName = oldName;
			this.item = item;
		}
		public FileManagerItem Item { get { return item; } }
		public string OldName { get { return oldName; } }
	}
	public delegate void FileManagerItemDeleteEventHandler(object source, FileManagerItemDeleteEventArgs e);
	public class FileManagerItemDeleteEventArgs : FileManagerActionEventArgsBase {
		FileManagerItem item;
		public FileManagerItemDeleteEventArgs(FileManagerItem item) {
			this.item = item;
		}
		public FileManagerItem Item { get { return item; } }
	}
	public delegate void FileManagerItemsDeletedEventHandler(object source, FileManagerItemsDeletedEventArgs e);
	public class FileManagerItemsDeletedEventArgs : EventArgs {
		FileManagerItem[] items;
		public FileManagerItemsDeletedEventArgs(FileManagerItem[] items) {
			this.items = items;
		}
		public FileManagerItem[] Items { get { return items; } }
	}
	public delegate void FileManagerItemMoveEventHandler(object source, FileManagerItemMoveEventArgs e);
	public class FileManagerItemMoveEventArgs : FileManagerActionEventArgsBase {
		FileManagerItem item;
		FileManagerFolder destinationFolder;
		public FileManagerItemMoveEventArgs(FileManagerFolder destinationFolder, FileManagerItem item) {
			this.item = item;
			this.destinationFolder = destinationFolder;
		}
		public FileManagerItem Item { get { return item; } }
		public FileManagerFolder DestinationFolder { get { return destinationFolder; } }
	}
	public delegate void FileManagerItemsMovedEventHandler(object source, FileManagerItemsMovedEventArgs e);
	public class FileManagerItemsMovedEventArgs : EventArgs {
		FileManagerItem[] items;
		FileManagerFolder sourceFolder;
		public FileManagerItemsMovedEventArgs(FileManagerFolder sourceFolder, FileManagerItem[] items) {
			this.items = items;
			this.sourceFolder = sourceFolder;
		}
		public FileManagerItem[] Items { get { return items; } }
		public FileManagerFolder SourceFolder { get { return sourceFolder; } }
	}
	public delegate void FileManagerItemCopyEventHandler(object source, FileManagerItemCopyEventArgs e);
	public class FileManagerItemCopyEventArgs : FileManagerActionEventArgsBase {
		FileManagerItem item;
		FileManagerFolder destinationFolder;
		public FileManagerItemCopyEventArgs(FileManagerFolder destinationFolder, FileManagerItem item) {
			this.item = item;
			this.destinationFolder = destinationFolder;
		}
		public FileManagerItem Item { get { return item; } }
		public FileManagerFolder DestinationFolder { get { return destinationFolder; } }
	}
	public delegate void FileManagerItemsCopiedEventHandler(object source, FileManagerItemsCopiedEventArgs e);
	public class FileManagerItemsCopiedEventArgs : EventArgs {
		FileManagerItem[] items;
		FileManagerFolder sourceFolder;
		public FileManagerItemsCopiedEventArgs(FileManagerFolder sourceFolder, FileManagerItem[] items) {
			this.items = items;
			this.sourceFolder = sourceFolder;
		}
		public FileManagerItem[] Items { get { return items; } }
		public FileManagerFolder SourceFolder { get { return sourceFolder; } }
	}
	public delegate void FileManagerThumbnailCreateEventHandler(object source, FileManagerThumbnailCreateEventArgs e);
	public class FileManagerThumbnailCreateEventArgs : EventArgs {
		FileManagerFile file;
		FileManagerItem item;
		FileManagerThumbnailProperties thumbnailImage;
		bool isParentFolder;
		public FileManagerThumbnailCreateEventArgs(FileManagerItem item, bool isParentFolder) {
			this.item = item;
			this.file = item as FileManagerFile;
			this.thumbnailImage = new FileManagerThumbnailProperties();
			this.isParentFolder = isParentFolder;
		}
		[Obsolete("This property is now obsolete. Use the Item property instead.")]
		public FileManagerFile File { get { return file; } }
		public FileManagerItem Item { get { return item; } }
		public FileManagerThumbnailProperties ThumbnailImage { get { return thumbnailImage; } }
		public bool IsParentFolder { get { return isParentFolder; } }
	}
	public delegate void FileManagerFileUploadEventHandler(object source, FileManagerFileUploadEventArgs e);
	public class FileManagerFileUploadEventArgs : FileManagerActionEventArgsBase {
		FileManagerFile file;
		Stream inputStream;
		Stream outputStream;
		string name;
		public FileManagerFileUploadEventArgs(FileManagerFile file, Stream inputStream) {
			this.file = file;
			this.inputStream = inputStream;
			this.outputStream = null;
			this.name = file.Name;
		}
		public FileManagerFile File { get { return file; } }
		public Stream InputStream { get { return inputStream; } }
		public Stream OutputStream { get { return outputStream; } set { outputStream = value; } }
		public string FileName { get { return name; } set { name = value; } }
	}
	public delegate void FileManagerFilesUploadedEventHandler(object source, FileManagerFilesUploadedEventArgs e);
	public class FileManagerFilesUploadedEventArgs : EventArgs {
		FileManagerFile[] files;
		public FileManagerFilesUploadedEventArgs(FileManagerFile[] files) {
			this.files = files;
		}
		public FileManagerFile[] Files { get { return files; } }
	}
	public delegate void FileManagerCustomErrorTextEventHandler(object source, FileManagerCustomErrorTextEventArgs e);
	public class FileManagerCustomErrorTextEventArgs : EventArgs {
		string errorText;
		Exception exception;
		public FileManagerCustomErrorTextEventArgs(Exception e, string errorText) {
			this.exception = e;
			this.errorText = errorText;
		}
		public Exception Exception { get { return exception; } }
		public string ErrorText {
			get { return errorText; }
			set { errorText = value == null ? string.Empty : value; }
		}
	}
	public delegate void FileManagerFileDownloadingEventHandler(object source, FileManagerFileDownloadingEventArgs e);
	public class FileManagerFileDownloadingEventArgs : FileManagerActionEventArgsBase {
		FileManagerFile file;
		Stream inputStream;
		Stream outputStream;
		public FileManagerFileDownloadingEventArgs(FileManagerFile file, Stream inputStream) {
			this.inputStream = inputStream;
			this.outputStream = null;
			this.file = file;
		}
		public FileManagerFile File { get { return file; } }
		public Stream InputStream { get { return inputStream; } }
		public Stream OutputStream { get { return outputStream; } set { outputStream = value; } }
	}
	public delegate void FileManagerFileOpenedEventHandler(object source, FileManagerFileOpenedEventArgs e);
	public class FileManagerFileOpenedEventArgs : EventArgs {
		FileManagerFile file;
		public FileManagerFileOpenedEventArgs(FileManagerFile file) {
			this.file = file;
		}
		public FileManagerFile File { get { return file; } }
	}
	public delegate void FileManagerCustomFileInfoDisplayTextEventHandler(object source, FileManagerCustomFileInfoDisplayTextEventArgs e);
	public class FileManagerCustomFileInfoDisplayTextEventArgs : EventArgs {
		public FileManagerCustomFileInfoDisplayTextEventArgs(FileManagerFile file, FileInfoType fileInfoType, string displayText) {
			File = file;
			FileInfoType = fileInfoType;
			DisplayText = displayText;
			EncodeHtml = true;
		}
		public FileManagerFile File { get; protected set; }
		public FileInfoType FileInfoType { get; protected set; }
		public string DisplayText { get; set; }
		public bool EncodeHtml { get; set; }
	}
	public delegate void FileManagerDetailsViewCustomColumnDisplayTextEventHandler(object source, FileManagerDetailsViewCustomColumnDisplayTextEventArgs e);
	public class FileManagerDetailsViewCustomColumnDisplayTextEventArgs : EventArgs {
		public FileManagerDetailsViewCustomColumnDisplayTextEventArgs(string displayText, FileManagerDetailsCustomColumn column, FileManagerFile file) {
			DisplayText = displayText;
			Column = column;
			File = file;
			EncodeHtml = true;
		}
		public FileManagerDetailsCustomColumn Column { get; protected set; }
		public FileManagerFile File { get; protected set; }
		public string DisplayText { get; set; }
		public bool EncodeHtml { get; set; }
	}
	public delegate void FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventHandler(object source, FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventArgs e);
	public class FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventArgs : EventArgs {
		public FileManagerDetailsViewCustomColumnHeaderFilterFillItemsEventArgs(FileManagerDetailsCustomColumn column, GridHeaderFilterValues values) {
			Values = values;
			Column = column;
		}
		public FileManagerDetailsCustomColumn Column { get; protected set; }
		public GridHeaderFilterValues Values { get; protected set; }
		public void AddValue(string displayText, string value) {
			Values.Add(new FilterValue(displayText, value));
		}
		public void AddValue(string displayText, string value, string query) {
			Values.Add(new FilterValue(displayText, value, CriteriaOperator.Parse(query).ToString()));
		}
		public void AddShowAll() {
			Values.Add(FilterValue.CreateShowAllValue(ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.HeaderFilterShowAllItem)));
		}
	}
	public delegate void FileManagerCloudProviderRequestEventHandler(object source, FileManagerCloudProviderRequestEventArgs e);
	public class FileManagerCloudProviderRequestEventArgs : EventArgs {
		public FileManagerCloudProviderRequestEventArgs(HttpWebRequest request) {
			Request = request;
		}
		public HttpWebRequest Request { get; protected set; }
	}
}
