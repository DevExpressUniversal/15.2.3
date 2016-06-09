#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.FileAttachments.Win {
	public interface IFileDataManager {
		void Open(IFileData fileData);
		void Save(IFileData fileData);
		void SaveFiles(List<IFileData> fileDataList);
	}
	public class CustomFileOperationEventArgs : HandledEventArgs {
		private IFileData fileData;
		public CustomFileOperationEventArgs(IFileData fileData) {
			this.fileData = fileData;
		}
		public IFileData FileData {
			get { return fileData; }
		}
	}
	public class CustomFileListOperationEventArgs : HandledEventArgs {
		private List<IFileData> fileDataList;
		public CustomFileListOperationEventArgs(List<IFileData> fileDataList) {
			this.fileDataList = fileDataList;
		}
		public List<IFileData> FileDataList {
			get { return fileDataList; }
		}
	}
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Includes Property Editors and Controllers to attach, save, open and download files in Windows Forms applications. Works with the Business Class Library's file-related data types.")]
	[ToolboxBitmap(typeof(FileAttachmentsWindowsFormsModule), "Resources.Toolbox_Module_FileAttachment_Win.ico")]
	[ToolboxItemFilter("Xaf.Platform.Win")]
	public sealed class FileAttachmentsWindowsFormsModule : ModuleBase, IFileDataManager {
		public const string LocalizationGroup = "FileAttachments";
		public static IFileDataManager GetFileDataManager(XafApplication application) {
			FileAttachmentsWindowsFormsModule fileAttachmentsWindowsFormsModule = null;
			if(application != null) {
				fileAttachmentsWindowsFormsModule = (FileAttachmentsWindowsFormsModule)application.Modules.FindModule(typeof(FileAttachmentsWindowsFormsModule));
			}
			if(fileAttachmentsWindowsFormsModule == null) {
				Tracing.Tracer.LogWarning("FileAttachmentsWindowsFormsModule is not found.");
			}
			return fileAttachmentsWindowsFormsModule as IFileDataManager;
		}
		public static IFileData CreateFileData(IObjectSpace objectSpace, IMemberInfo memberDescriptor) {
			IFileData result = null;
			if(!memberDescriptor.IsReadOnly && DataManipulationRight.CanInstantiate(memberDescriptor.MemberType, objectSpace)) {
				result = objectSpace.CreateObject(memberDescriptor.MemberType) as IFileData;
			}
			else {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToInitializeFileDataProperty, memberDescriptor.Name, memberDescriptor.Owner.Type, memberDescriptor.MemberType));
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetFileTypesFilter(IModelClass modelClass, string propertyName) {
			Guard.ArgumentNotNull(modelClass,"modelClass");
			string result = "";
			IModelCommonFileTypeFilters extender;
			IModelMember modelMember = modelClass.FindMember(propertyName);
			if(modelMember != null) {
				extender = modelMember as IModelCommonFileTypeFilters;
				result = GetFileTypesFilterCore(extender);
			}
			if(string.IsNullOrEmpty(result)) {
				extender = modelClass as IModelCommonFileTypeFilters;
				result = GetFileTypesFilterCore(extender);
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		private static string GetFileTypesFilterCore(IModelCommonFileTypeFilters extender) {
			string result = "";
			if(extender != null && extender.FileTypeFilters != null && extender.FileTypeFilters.Count != 0) {
				result = extender.FileTypeFilters.FileTypesFilter;
			}
			return result;
		}
		private void OnCustomOpenFileWithDefaultProgram(CustomFileOperationEventArgs args) {
			if(CustomOpenFileWithDefaultProgram != null) {
				CustomOpenFileWithDefaultProgram(this, args);
			}
		}
		private void OnCustomSaveFiles(CustomFileListOperationEventArgs args) {
			if(CustomSaveFiles != null) {
				CustomSaveFiles(this, args);
			}
		}
		private bool CancelFile(string fileName) {
			if(!File.Exists(fileName))
				return false;
			string message = string.Format(CaptionHelper.GetLocalizedText(LocalizationGroup, "OverwritePrompt"), fileName);
			Messaging messaging = Messaging.GetMessaging((WinApplication)Application);
			return messaging.Show(message, CaptionHelper.GetLocalizedText(LocalizationGroup, "OverwritePromptCaption"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes;
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			return new Type[]{
			typeof(IModelFileTypeFilterExtension),
			typeof(IModelFileTypeFilterExtensions),
			typeof(IModelFileTypeFilter),
			typeof(IModelFileTypeFilters),
			typeof(IModelCommonFileTypeFilters),
			typeof(IModelOptionsFileAttachment),
			typeof(IModelOptionsFileAttachments),
			typeof(ModelOptionsFileAttachmentLogic),
			typeof(FileTypeFiltersLogic),
			typeof(ModelFileTypeFilterLogic)
			};
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return Type.EmptyTypes;
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(FileAttachmentController),
				typeof(FileAttachmentControllerBase),
				typeof(FileAttachmentListViewController)
			};
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasAndEditorTypeRegistration(EditorAliases.FileDataPropertyEditor, typeof(IFileData), true, typeof(FileDataPropertyEditor), true)));
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelOptions, IModelOptionsFileAttachments>();
			extenders.Add<IModelClass, IModelCommonFileTypeFilters>();
			extenders.Add<IModelMember, IModelCommonFileTypeFilters>();
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		#region IFileDataManager Members
		void IFileDataManager.Open(IFileData fileData) {
			Guard.ArgumentNotNull(fileData, "fileData");
			if(!FileDataHelper.IsFileDataEmpty(fileData)) {
				CustomFileOperationEventArgs args = new CustomFileOperationEventArgs(fileData);
				OnCustomOpenFileWithDefaultProgram(args);
				if(!args.Handled) {
					string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("B"));
					try {
						Directory.CreateDirectory(tempDirectory);
					}
					catch {
						Tracing.Tracer.LogValue("tempDirectory", tempDirectory);
						throw;
					}
					string tempFileName = Path.Combine(tempDirectory, fileData.FileName);
					try {
						using(FileStream stream = new FileStream(tempFileName, FileMode.CreateNew)) {
							fileData.SaveToStream(stream);
						}
						Process.Start(tempFileName);
					}
					catch {
						Tracing.Tracer.LogValue("tempFileName", tempFileName);
						throw;
					}
				}
			}
		}
		void IFileDataManager.Save(IFileData fileData) {
			using(SaveFileDialog dialog = new SaveFileDialog()) {
				dialog.CreatePrompt = false;
				dialog.OverwritePrompt = true;
				string fileExtension = Path.GetExtension(fileData.FileName).TrimStart('.');
				string wordAll = CaptionHelper.GetLocalizedText(LocalizationGroup, "WordAll");
				string wordFiles = CaptionHelper.GetLocalizedText(LocalizationGroup, "WordFiles");
				dialog.Filter = fileExtension.ToUpper() + " " + wordFiles + " (*." + fileExtension + ")|*." + fileExtension +
					"|" + wordAll + " " + wordFiles + " (*.*)|*.*";
				IModelOptionsFileAttachments fileAttachmentOptions = Application.Model.Options as IModelOptionsFileAttachments;
				dialog.InitialDirectory = fileAttachmentOptions.Attachments.DefaultDirectory;
				dialog.FileName = fileData.FileName;
				dialog.Title = CaptionHelper.GetLocalizedText(LocalizationGroup, "OverwritePromptCaption");
				if(dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) {
					fileAttachmentOptions.Attachments.DefaultDirectory = Path.GetDirectoryName(dialog.FileName);
					using(FileStream stream = new FileStream(dialog.FileName, FileMode.Create)) {
						fileData.SaveToStream(stream);
					}
				}
			}
		}
		void IFileDataManager.SaveFiles(List<IFileData> fileDataList) {
			Guard.ArgumentNotNull(fileDataList, "fileData");
			CustomFileListOperationEventArgs args = new CustomFileListOperationEventArgs(fileDataList);
			OnCustomSaveFiles(args);
			if(!args.Handled) {
				if(fileDataList.Count == 1) {
					((IFileDataManager)this).Save(fileDataList[0]);
				}
				else {
					using(FolderBrowserDialog dialog = new FolderBrowserDialog()) {
						IModelOptionsFileAttachments fileAttachmentOptions = Application.Model.Options as IModelOptionsFileAttachments;
						dialog.SelectedPath = fileAttachmentOptions.Attachments.DefaultDirectory;
						if(dialog.ShowDialog() == DialogResult.OK) {
							string folderName = dialog.SelectedPath;
							foreach(IFileData fileData in fileDataList) {
								string fileName = Path.Combine(folderName, fileData.FileName);
								if(!CancelFile(fileName)) {
									using(FileStream stream = new FileStream(fileName, FileMode.Create)) {
										fileData.SaveToStream(stream);
									}
								}
							}
							fileAttachmentOptions.Attachments.DefaultDirectory = folderName;
						}
					}
				}
			}
		}
		#endregion
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("FileAttachmentsWindowsFormsModuleCustomOpenFileWithDefaultProgram")]
#endif
		public event EventHandler<CustomFileOperationEventArgs> CustomOpenFileWithDefaultProgram;
#if !SL
	[DevExpressExpressAppFileAttachmentWinLocalizedDescription("FileAttachmentsWindowsFormsModuleCustomSaveFiles")]
#endif
		public event EventHandler<CustomFileListOperationEventArgs> CustomSaveFiles;
	}
}
