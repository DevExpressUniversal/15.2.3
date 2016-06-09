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
using System.Text;
using DevExpress.Utils.Design;
using System.Collections.Generic;
namespace DevExpress.Xpf.Core.Design.Wizards.Utils {
	public class LicXFileCreator {
		public static void CreateLicXFile(EnvDTE.Project project, string propertiesFolderName) {
			WriteFile(GetLicXFilePath(project, propertiesFolderName), LicXFileContent, FileMode.Create, Encoding.ASCII);
			AddLicXFileToProject(project, propertiesFolderName);
		}
		public static bool IsLicXFileNeeded(EnvDTE.Project project, string propertiesFolderName) { 
			if(project == null)
				return false;
			string path = GetLicXFilePath(project, propertiesFolderName);
			if (string.IsNullOrEmpty(path))
				return false;
			if (!File.Exists(path))
				return true;
			string currentContent = ReadFile(path, Encoding.ASCII);
			if(!ContainsLicense(currentContent))
				return true;
			EnvDTE.ProjectItem propertiesFolder = GetPropertiesFolder(project, propertiesFolderName);
			return !FolderContains(LicxFileName, propertiesFolder);				
		}
		private static bool ContainsLicense(string currentContent) {
			if(string.IsNullOrEmpty(currentContent))
				return false;
			string[] lines = currentContent.Split(new string[] {"\n", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			if(lines == null || lines.Length == 0)
				return false;
			string licXFileContent = LicXFileContent.Trim();
			foreach(string item in lines)
				if(string.Compare(item, licXFileContent, true) == 0)
					return true;
			return false;
		}
		public static void CreateLicXFileIfNeeded(EnvDTE.Project project, string propertiesFolderName) {
			if(IsLicXFileNeeded(project, propertiesFolderName))
				CreateLicXFile(project, propertiesFolderName);
		}
		public static void AddLicenseToProject(EnvDTE.Project project, string propertiesFolderName) {
			if(project == null)
				return;
			string path = GetLicXFilePath(project, propertiesFolderName);
			if(string.IsNullOrEmpty(path))
				return;
			EnvDTE.ProjectItem propertiesFolder = GetPropertiesFolder(project, propertiesFolderName);
			if(!File.Exists(path) || !FolderContains(LicxFileName, propertiesFolder)) {
				WriteFile(path, LicXFileContent, FileMode.Create, Encoding.ASCII);
				AddLicXFileToProject(project, propertiesFolderName);
				return;
			}
			string currentContent = ReadFile(path, Encoding.ASCII);
			if(ContainsLicense(currentContent))
				return;
			if(string.IsNullOrEmpty(currentContent))
				currentContent = LicXFileContent;
			else
				if(currentContent.EndsWith("\n"))
					currentContent = currentContent + LicXFileContent;
				else
					currentContent = currentContent + '\n' + LicXFileContent;
			WriteFile(path, currentContent, FileMode.Create, Encoding.ASCII);
		}
		static EnvDTE.ProjectItem GetPropertiesFolder(EnvDTE.Project project, string propertiesFolderName) {
			if(string.IsNullOrEmpty(propertiesFolderName))
				return null;
			EnvDTE.ProjectItems items = project.ProjectItems;
			foreach (EnvDTE.ProjectItem item in items)
				if(item != null && DTEHelper.IsPhysicalFolder(item.Kind) && item.Name == propertiesFolderName)
					return item;
			return null;
		}
		static bool FolderContains(string fileName, EnvDTE.ProjectItem propertiesFolder) {
			if (String.IsNullOrEmpty(fileName) || propertiesFolder == null)
				return false;
			foreach (EnvDTE.ProjectItem item in propertiesFolder.ProjectItems)
				if (item != null && item.Name == fileName)
					return true;
			return false;
		}
		static void AddLicXFileToProject(EnvDTE.Project project, string propertiesFolderName) {
			AddFromFile(GetLicXFilePath(project, propertiesFolderName), project);
		}
		static string GetLicXFilePath(EnvDTE.Project project, string propertiesFolderName) {
			return Path.Combine(GetProjectPath(project), propertiesFolderName, LicxFileName);
		}
		static string GetProjectPath(EnvDTE.Project project) {			
			if(project == null)
				return null;
			string path = project.FullName;
			return path.Substring(0, path.LastIndexOf(project.Name));
		}
		static EnvDTE.ProjectItem AddFromFile(string fileName, EnvDTE.Project project) {
			try {
				if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName) || project == null)
					return null;
				return project.ProjectItems.AddFromFile(fileName);
			}
			catch {
				return null;
			}
		}
		static void WriteFile(string fileName, string fileData, FileMode mode, Encoding encoding) {
			using (FileStream fsWrite = new FileStream(fileName, mode, FileAccess.Write)) {
				using (StreamWriter streamWriter = new StreamWriter(fsWrite, encoding)) {
					streamWriter.Write(fileData);
				}
			}
		}
		static string ReadFile(string fileName, Encoding encoding) {
			try {
				return File.ReadAllText(fileName, encoding);
			}
			catch {
				return null;
			}
		}
		static string LicXFileContent {
			get {
				Type type = typeof(DevExpress.Xpf.Bars.BarManager);
				return string.Format("{0}, {1}{2}", type.FullName, type.Assembly.FullName, Environment.NewLine);
			}
		}
		static string LicxFileName { get { return "licenses.licx"; } }		
	}
}
