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
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web {
	public class FileManagerClientSideEvents : CallbackClientSideEventsBase {
		public FileManagerClientSideEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsSelectedFileChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectedFileChanged {
			get { return GetEventHandler("SelectedFileChanged"); }
			set { SetEventHandler("SelectedFileChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsFocusedItemChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FocusedItemChanged
		{
			get { return GetEventHandler("FocusedItemChanged"); }
			set { SetEventHandler("FocusedItemChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsSelectedFileOpened"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectedFileOpened {
			get { return GetEventHandler("SelectedFileOpened"); }
			set { SetEventHandler("SelectedFileOpened", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsFolderCreating"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FolderCreating {
			get { return GetEventHandler("FolderCreating"); }
			set { SetEventHandler("FolderCreating", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsFolderCreated"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FolderCreated {
			get { return GetEventHandler("FolderCreated"); }
			set { SetEventHandler("FolderCreated", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemRenaming"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemRenaming {
			get { return GetEventHandler("ItemRenaming"); }
			set { SetEventHandler("ItemRenaming", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemRenamed"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemRenamed {
			get { return GetEventHandler("ItemRenamed"); }
			set { SetEventHandler("ItemRenamed", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemDeleting"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemDeleting {
			get { return GetEventHandler("ItemDeleting"); }
			set { SetEventHandler("ItemDeleting", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemDeleted"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemDeleted {
			get { return GetEventHandler("ItemDeleted"); }
			set { SetEventHandler("ItemDeleted", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemsDeleted"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemsDeleted {
			get { return GetEventHandler("ItemsDeleted"); }
			set { SetEventHandler("ItemsDeleted", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemMoving"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemMoving {
			get { return GetEventHandler("ItemMoving"); }
			set { SetEventHandler("ItemMoving", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemMoved"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemMoved {
			get { return GetEventHandler("ItemMoved"); }
			set { SetEventHandler("ItemMoved", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemsMoved"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemsMoved {
			get { return GetEventHandler("ItemsMoved"); }
			set { SetEventHandler("ItemsMoved", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemCopying"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemCopying
		{
			get { return GetEventHandler("ItemCopying"); }
			set { SetEventHandler("ItemCopying", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemCopied"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemCopied
		{
			get { return GetEventHandler("ItemCopied"); }
			set { SetEventHandler("ItemCopied", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsItemsCopied"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemsCopied {
			get { return GetEventHandler("ItemsCopied"); }
			set { SetEventHandler("ItemsCopied", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsFileUploading"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Obsolete("This property is now obsolete. Use the FilesUploading property instead."),
		Browsable(false), 
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FileUploading {
			get { return GetEventHandler("FileUploading"); }
			set { SetEventHandler("FileUploading", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsFilesUploading"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FilesUploading {
			get { return GetEventHandler("FilesUploading"); }
			set { SetEventHandler("FilesUploading", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsFileUploaded"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FileUploaded {
			get { return GetEventHandler("FileUploaded"); }
			set { SetEventHandler("FileUploaded", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsFilesUploaded"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FilesUploaded {
			get { return GetEventHandler("FilesUploaded"); }
			set { SetEventHandler("FilesUploaded", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsErrorOccurred"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ErrorOccurred {
			get { return GetEventHandler("ErrorOccurred"); }
			set { SetEventHandler("ErrorOccurred", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsFileDownloading"),
#endif
DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FileDownloading
		{
			get { return GetEventHandler("FileDownloading"); }
			set { SetEventHandler("FileDownloading", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsCurrentFolderChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CurrentFolderChanged
		{
			get { return GetEventHandler("CurrentFolderChanged"); }
			set { SetEventHandler("CurrentFolderChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsSelectionChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectionChanged
		{
			get { return GetEventHandler("SelectionChanged"); }
			set { SetEventHandler("SelectionChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerClientSideEventsErrorAlertDisplaying"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ErrorAlertDisplaying
		{
			get { return GetEventHandler("ErrorAlertDisplaying"); }
			set { SetEventHandler("ErrorAlertDisplaying", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomCommand {
			get { return GetEventHandler("CustomCommand"); }
			set { SetEventHandler("CustomCommand", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ToolbarUpdating {
			get { return GetEventHandler("ToolbarUpdating"); }
			set { SetEventHandler("ToolbarUpdating", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string HighlightItemTemplate {
			get { return GetEventHandler("HighlightItemTemplate"); }
			set { SetEventHandler("HighlightItemTemplate", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("SelectedFileChanged");
			names.Add("FocusedItemChanged");
			names.Add("SelectedFileOpened");
			names.Add("FolderCreating");
			names.Add("FolderCreated");
			names.Add("ItemRenaming");
			names.Add("ItemRenamed");
			names.Add("ItemDeleting");
			names.Add("ItemDeleted");
			names.Add("ItemsDeleted");
			names.Add("ItemMoving");
			names.Add("ItemMoved");
			names.Add("ItemsMoved");
			names.Add("ItemCopying");
			names.Add("ItemCopied");
			names.Add("ItemsCopied");
			names.Add("FileUploading");
			names.Add("FilesUploading");
			names.Add("FileUploaded");
			names.Add("FilesUploaded");
			names.Add("ErrorCommand");
			names.Add("ErrorOccurred");
			names.Add("FileDownloading");
			names.Add("CurrentFolderChanged");
			names.Add("SelectionChanged");
			names.Add("ErrorAlertDisplaying");
			names.Add("CustomCommand");
			names.Add("ToolbarUpdating");
			names.Add("HighlightItemTemplate");
		}
	}
}
