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
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
using DevExpress.Design.Mvvm;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	public class TemplatesPaths {
		const string STR_Views = "Views";
		const string STR_ViewModels = "ViewModels";
		static bool IsCommonFolder(string directoryPath) {
			return IsSubFolder(directoryPath, TemplatesConstants.STR_Common);
		}
		static bool IsDataModelFolder(string directoryPath) {
			return IsSubFolder(directoryPath, TemplatesConstants.STR_DataModel);
		}
		static bool IsViewFolder(string directoryPath) {
			return IsSubFolder(directoryPath, STR_Views);
		}
		static bool IsViewModelFolder(string directoryPath) {
			return IsSubFolder(directoryPath, STR_ViewModels);
		}
		static bool IsSubFolder(string directoryPath, string supposedRoot) {
			if(string.IsNullOrEmpty(directoryPath))
				return false;
			return GetRootFolder(directoryPath) == supposedRoot;
		}
		static string GetRootFolder(string directoryPath) {
			string[] subFolders = directoryPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
			if(subFolders == null || subFolders.Length == 0)
				return null;
			return subFolders[0];
		}
		public static string GetCommonDataModelNamespace(ITemplatesPlatform templatesPlatform) {
			return GetNamespace(templatesPlatform.GetProjectNamespace(), TemplatesPaths.GetDirectory(typeof(Resources.Common.DataModel.IRepositoryTemplate)));
		}
		public static string GetCommonViewModelNamespace(ITemplatesPlatform templatesPlatform) {
			return GetNamespace(templatesPlatform.GetProjectNamespace(), TemplatesPaths.GetDirectory(typeof(Resources.Common.ViewModel.MessagesTemplate)));
		}
		public static string GetCommonUtilsNamespace(ITemplatesPlatform templatesPlatform) {
			return GetNamespace(templatesPlatform.GetProjectNamespace(), TemplatesPaths.GetDirectory(typeof(Resources.Common.Utils.DbExtensionsTemplate)));
		}
		internal static string GetNamespace(string rootNameSpace, string directoryPath) {
			string namespaceName = GetNamespaceNameFromPath(directoryPath);
			if(string.IsNullOrEmpty(namespaceName))
				return rootNameSpace;
			else if(string.IsNullOrEmpty(rootNameSpace))
				return namespaceName;
			else
				return rootNameSpace + "." + namespaceName;
		}
		static string GetNamespaceNameFromPath(string directoryPath) {
			if(string.IsNullOrEmpty(directoryPath))
				return null;
			string[] subFolders = directoryPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
			return string.Join(".", subFolders);
		}
		static string GetUniqueFolderName(string parentDirectory, string directoryName, bool rewriteExisting) {
			if(string.IsNullOrEmpty(parentDirectory))
				return null;
			string result = directoryName;
			string candidate = Path.Combine(parentDirectory, result);
			if(rewriteExisting)
				return result;
			int i = 0;
			while(Directory.Exists(candidate)) {
				i++;
				result = directoryName + i;
				candidate = Path.Combine(parentDirectory, result);
			}
			return result;
		}
		static string GetLastDirectory(string directoryPath) {
			string[] subFolders = directoryPath.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
			if(subFolders == null || subFolders.Length == 0) {
				return directoryPath;
			}
			return subFolders[subFolders.Length - 1];
		}
		public static string GetDirectory(Type templateType) {
			if(templateType.Namespace.Contains("Views.WinForms.")){
				string result =  templateType.Namespace.Replace(TemplatesResources.STR_TemplatesNamespace, string.Empty);
				result = result.Replace(".WinForms.Outlook", string.Empty);
				result = result.Replace(".WinForms.WinUI", string.Empty);
				result = result.Replace(".WinForms.Standart", string.Empty);
				return result.Replace(".", "\\").TrimStart('\\');
			}
			else
			return templateType.Namespace.Replace(TemplatesResources.STR_TemplatesNamespace, string.Empty).Replace(".", "\\").TrimStart('\\');
		}
		static string GetTargetDirectoryForViewCore(ITemplatesPlatform templatesPlatform, T4TemplateInfo templateInfo, string rootFolder, string directoryPath, string folderName) {
			string lastDirectory = GetLastDirectory(rootFolder);
			if(string.IsNullOrEmpty(lastDirectory))
				return rootFolder;
			if(IsSubFolder(directoryPath, lastDirectory))
				return Path.Combine(rootFolder, folderName);
			templateInfo.Properties[TemplatesConstants.STR_Namespace] = GetNamespace(templatesPlatform.GetDefaultNamespace(), directoryPath);
			return Path.Combine(rootFolder, directoryPath, folderName);
		}
		static string GetTargetDirectoryForViewModel(ITemplatesPlatform templatesPlatform, T4TemplateInfo templateInfo, string rootFolder, string directoryPath, string folderName) {
			string result = GetTargetDirectoryForViewCore(templatesPlatform, templateInfo, rootFolder, directoryPath, folderName);
			if(string.IsNullOrEmpty(templateInfo.GetProperty(TemplatesConstants.STR_Namespace) as string))
				templateInfo.Properties[TemplatesConstants.STR_Namespace] = STR_ViewModels;
			return result;
		}
		public static bool IsCommon(Type templateType) {
			return IsCommonFolder(GetDirectory(templateType));
		}
		public virtual string GetTemplateTargetDirectory(ITemplatesPlatform templatesPlatform, Type templateType, T4TemplateInfo templateInfo, string folderName, UIType uiType) {
			templateInfo.Properties[TemplatesConstants.STR_Namespace] = templatesPlatform.GetDefaultNamespace();
			string rootFolder = templatesPlatform.GetTargetFolder();
			if(templateType == null)
				return rootFolder;
			string directoryPath = GetDirectory(templateType);
			if(string.IsNullOrEmpty(directoryPath)) 
			{
				if(string.IsNullOrEmpty(templateInfo.GetProperty(TemplatesConstants.STR_Namespace) as string)) {
					if(TemplatesResources.IsView(templateType))
						templateInfo.Properties[TemplatesConstants.STR_Namespace] = STR_Views;
					else if(TemplatesResources.IsViewModel(templateType))
						templateInfo.Properties[TemplatesConstants.STR_Namespace] = STR_ViewModels;
					else
						templateInfo.Properties[TemplatesConstants.STR_Namespace] = "GlobalNamespace";
				}
				return rootFolder;
			}
			if(IsCommonFolder(directoryPath)) {
				templateInfo.Properties[TemplatesConstants.STR_Namespace] = GetNamespace(templatesPlatform.GetProjectNamespace(), directoryPath);
				templateInfo.Properties["EmbeddedResourcesPath"] = templatesPlatform.GetEmbeddedResourcesPath(directoryPath);
				return Path.Combine(templatesPlatform.GetProjectFolder(), directoryPath);
			}
			if(IsDataModelFolder(directoryPath)) {
				string dataModelFolder = templateInfo.GetProperty(TemplatesConstants.STR_DataModelFolder) as string;
				if(string.IsNullOrEmpty(dataModelFolder)) {
					EntityModelData data = templateInfo.GetProperty(TemplatesConstants.STR_EntityModelData) as EntityModelData;
					dataModelFolder = data.Name + TemplatesConstants.STR_DataModel;
					dataModelFolder = GetUniqueFolderName(rootFolder, dataModelFolder, templatesPlatform.RewriteExistingFiles);
					templateInfo.Properties[TemplatesConstants.STR_DataModelFolder] = dataModelFolder;
				}
				directoryPath = dataModelFolder + directoryPath.Remove(0, TemplatesConstants.STR_DataModel.Length);
				templateInfo.Properties[TemplatesConstants.STR_Namespace] = GetNamespace(templatesPlatform.GetDefaultNamespace(), dataModelFolder);
				return Path.Combine(rootFolder, directoryPath);
			}
			if(IsViewFolder(directoryPath))
				return GetTargetDirectoryForView(templatesPlatform, templateInfo, rootFolder, directoryPath, templateType, folderName, uiType);
			if(IsViewModelFolder(directoryPath))
				return GetTargetDirectoryForViewModel(templatesPlatform, templateInfo, rootFolder, directoryPath, folderName);
			return rootFolder;
		}
		protected virtual string GetTargetDirectoryForView(ITemplatesPlatform templatesPlatform, T4TemplateInfo templateInfo, string rootFolder, string directoryPath, Type templateType, string folderName, UIType uiType) {
			string result = GetTargetDirectoryForViewCore(templatesPlatform, templateInfo, rootFolder, directoryPath, folderName);
			if(string.IsNullOrEmpty(templateInfo.GetProperty(TemplatesConstants.STR_Namespace) as string))
				templateInfo.Properties[TemplatesConstants.STR_Namespace] = STR_Views;
			return result;
		}
	}
}
