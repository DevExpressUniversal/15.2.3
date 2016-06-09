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

using DevExpress.Design.Properties;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
namespace DevExpress.Utils.Design.Taskbar {
	class TaskbarAssistantDesignerHelper {
		public static void AddNativeResourceFile(Project currentProject) {
			List<string> list = (from Project project in currentProject.DTE.Solution.Projects where project.Name != "Miscellaneous Files" select project.Name).ToList();
			using(NativeResTemplateCreateFileDialog dialog = new NativeResTemplateCreateFileDialog(list)) {
				dialog.ShowDialog();
				if(dialog.DialogResult != DialogResult.OK) return;
				Project selectedProject = null;
				foreach(Project project in currentProject.DTE.Solution.Projects.Cast<Project>().Where(project => dialog.ProjectName == project.Name))
					selectedProject = project;
				string win32resFilePath = CreateNativeResourceFile(selectedProject, dialog.FileName);
				selectedProject.ProjectItems.AddFromFile(win32resFilePath);
				selectedProject.DTE.ItemOperations.OpenFile(win32resFilePath);
				foreach(Property property in selectedProject.Properties) {
					try {
						if(property.Name != "Win32ResourceFile") continue;
						property.Value = win32resFilePath;
						return;
					}
					catch {
					}
				}
			}
		}
		protected static string CreateNativeResourceFile(Project project, string fileName) {
			bool isRestemplFileExist = File.Exists(Path.Combine(Path.GetDirectoryName(project.DTE.FullName), "NewFileItems", "restempl.rct"));
			string resFilePath = CalcResFilePath(RootProjectDirectory(project), fileName);
			if(isRestemplFileExist) {
				project.DTE.ItemOperations.NewFile(@"General\Native Resource Template", fileName);
				project.DTE.ActiveDocument.Save(resFilePath);
				return resFilePath;
			}
			string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "restempl.rct");
			Directory.CreateDirectory(Path.GetDirectoryName(tempFilePath));
			File.WriteAllBytes(tempFilePath, Resources.restempl);
			File.Copy(tempFilePath, resFilePath);
			Directory.Delete(Path.GetDirectoryName(tempFilePath), true);
			return resFilePath;
		}
		protected static string CalcResFilePath(string rootProjectFolder, string resName) {
			string result = Path.Combine(rootProjectFolder, resName);
			int count = 1;
			string ext = Path.GetExtension(result);
			string name = Path.GetFileNameWithoutExtension(result);
			while(File.Exists(result))
				result = Path.Combine(rootProjectFolder, string.Format("{0} ({1}){2}", name, (++count).ToString(), ext));
			return result;
		}
		public static string RootProjectDirectory(Project project) {
			for(int i = 0; i < project.Properties.Count; i++) {
				try {
					if(project.Properties.Item(i).Name != "LocalPath") continue;
					object value = project.Properties.Item(i).Value;
					return value.ToString();
				}
				catch {
				}
			}
			return null;
		}
		public static string ConfigurationPath(Project project) {
			string configuration = project.ConfigurationManager.ActiveConfiguration.ConfigurationName;
			XElement root = XElement.Load(project.FullName);
			var elements = root.Descendants(root.GetDefaultNamespace() + "OutputPath");
			foreach(XElement item in elements.Where(item => item.Value.Contains(configuration)))
				return item.Value;
			throw new Exception("Not found configuration");
		}
	}
}
