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

using System.ComponentModel;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	[ToolboxItem(false)]
	public class MVCxFileManager : ASPxFileManager {
		public MVCxFileManager()
			: base() {
			Settings.RootFolder = string.Empty;
		}
		public object CallbackRouteValues { get; set; }
		public object DownloadRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public new MVCxFileManagerSettingsUpload SettingsUpload { get { return (MVCxFileManagerSettingsUpload)base.SettingsUpload; } }
		public override bool IsCallback {
			get { return MvcUtils.CallbackName == ID; }
		}
		protected internal override bool IsCallBacksEnabled() {
			return CallbackRouteValues != null;
		}
		protected internal override string GetCallbackContentControlResult() {
			return Utils.CallbackHtmlContentPlaceholder;
		}
		protected internal new Control GetCallbackResultControl() {
			return base.GetCallbackResultControl();
		}
		protected internal bool IsFirstLoad {
			get { return !IsCallback; }
		}
		protected internal new RestrictedAccessFileSystemProvider FileSystemProvider {
			get { return base.FileSystemProvider; }
			set { base.FileSystemProvider = value; }
		}
		protected internal new FileManagerHelper Helper { get { return base.Helper; } }
		protected internal override bool IsFilterAvailable() {
			return base.IsFilterAvailable() && IsCallBacksEnabled();
		}
		protected internal override bool IsItemCreatingAvailable() {
			return base.IsItemCreatingAvailable() && IsCallBacksEnabled();
		}
		protected internal override bool IsItemDeletingAvailable() {
			return base.IsItemDeletingAvailable() && IsCallBacksEnabled();
		}
		protected internal override bool IsItemDownloadAvailable() {
			return base.IsItemDownloadAvailable() && DownloadRouteValues != null;
		}
		protected internal override bool IsItemMovingAvailable() {
			return base.IsItemMovingAvailable() && IsCallBacksEnabled();
		}
		protected internal override bool IsItemRenamingAvailable() {
			return base.IsItemRenamingAvailable() && IsCallBacksEnabled();
		}
		protected internal override bool IsRefreshAvailable() {
			return base.IsRefreshAvailable() && IsCallBacksEnabled();
		}
		protected internal new void CreateRestrictedAccessFileSystemProvider(FileSystemProviderBase provider) {
			base.CreateRestrictedAccessFileSystemProvider(provider);
		}
		protected override void CreateFileSystemProvider() {
			CreateRestrictedAccessFileSystemProvider(new PhysicalFileSystemProvider(Settings.RootFolder));
		}
		protected override DevExpress.Web.FileManagerSettings CreateSettings() {
			return new MVCxFileManagerSettings(this);
		}
		protected override FileManagerSettingsUpload CreateSettingsUpload() {
			return new MVCxFileManagerSettingsUpload(this);
		}
		protected internal override ASPxUploadControl CreateUploadControl(ASPxWebControl owner) {
			return new MVCxFileManagerUploadControl(owner);
		}
		protected override FileManagerControl CreateFileManagerControl() {
			return new MVCxFileManagerControl(this);
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if(DownloadRouteValues != null)
				stb.AppendFormat(localVarName + ".downloadUrl=\"" + Utils.GetUrl(DownloadRouteValues) + "\";\n");
			if(CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientFileManager";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxFileManager), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxFileManager), Utils.FileManagerScriptResourceName);
		}
	}
	public class MVCxDataSourceFileSystemProvider : DataSourceFileSystemProvider {
		public MVCxDataSourceFileSystemProvider(object dataSource, FileManagerSettingsDataSource settingsDataSource)
			: this(string.Empty, dataSource, settingsDataSource) {
		}
		public MVCxDataSourceFileSystemProvider(string rootFolder, object dataSource, FileManagerSettingsDataSource settingsDataSource)
			: base(rootFolder) {
			DataSource = dataSource;
			IsFolderFieldName = settingsDataSource.IsFolderFieldName;
			KeyFieldName = settingsDataSource.KeyFieldName;
			ParentKeyFieldName = settingsDataSource.ParentKeyFieldName;
			NameFieldName = settingsDataSource.NameFieldName;
			LastWriteTimeFieldName = settingsDataSource.LastWriteTimeFieldName;
			FileBinaryContentFieldName = settingsDataSource.FileBinaryContentFieldName;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new DataHelperCore DataHelper { get { return base.DataHelper; } set { base.DataHelper = value; } }
		public object DataSource { get; set; }
	}
}
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	[ToolboxItem(false)]
	public class MVCxFileManagerFolders : FileManagerFolders {
		public MVCxFileManagerFolders(MVCxFileManager fileManager, bool isMoving)
			: base(fileManager, isMoving) {
		}
		protected override bool IsFoldersCallbacksEnabled() {
			return ((MVCxFileManager)base.FileManager).CallbackRouteValues != null && base.IsFoldersCallbacksEnabled(); 
		}
	}
	[ToolboxItem(false)]
	public class MVCxFileManagerUploadControl : MVCxUploadControl {
		public MVCxFileManagerUploadControl(ASPxWebControl owner)
			: base(owner) {
			MVCxFileManager fileManager = FindParentFileManager();
			if(fileManager != null) {
				CallbackRouteValues = fileManager.CallbackRouteValues;
				ValidationSettings.Assign(fileManager.SettingsUpload.ValidationSettings);
			}
		}
		protected MVCxFileManager FindParentFileManager() {
			Control curControl = OwnerControl;
			while(curControl != null) {
				if(curControl is MVCxFileManagerContainer) {
					return ((MVCxFileManagerContainer)curControl).FileManager;
				}
				curControl = curControl.Parent;
			}
			return null;
		}
		protected override string GetClientObjectClassName() {
			return "MVCx.FileManagerUploadControl";
		}
	}
	[ToolboxItem(false)]
	public class MVCxFileManagerContainer : FileManagerContainer {
		public MVCxFileManagerContainer(FileManagerControl owner)
			: base(owner) {
		}
		public new MVCxFileManager FileManager { get { return (MVCxFileManager)base.FileManager; } }
		protected override FileManagerFolders InitializeFolders() {
			return new MVCxFileManagerFolders(FileManager, false);
		}
	}
	[ToolboxItem(false)]
	public class MVCxFileManagerControl : FileManagerControl {
		public MVCxFileManagerControl(MVCxFileManager fileManager)
			: base(fileManager) {
		}
		protected override FileManagerContainer CreateFileManagerContainer() {
			return new MVCxFileManagerContainer(this);
		}
	}
	public class MVCxFileManagerDownloadCommand : FileManagerDownloadCommand {
		public MVCxFileManagerDownloadCommand(MVCxFileManager fileManager, string commandArgs)
			: base() {
			Initialize(fileManager, FileManagerCommandId.Download, commandArgs);
		}
		public FileStreamResult GetFileStreamResult() {
			string fileName;
			string fileExt;
			Stream fileStream = GetDownloadStream(out fileName, out fileExt);
			if(fileStream == null)
				return null;
			FileStreamResult downloadResult = new FileStreamResult(fileStream, HttpUtils.GetContentType(fileExt));
			ExportUtils.PrepareDownloadResult(null, fileName, true, fileExt, ref downloadResult);
			return downloadResult;
		}
		public string GetCloudDownloadUrl() {
			return GetDownloadUrl();
		}
	}
}
