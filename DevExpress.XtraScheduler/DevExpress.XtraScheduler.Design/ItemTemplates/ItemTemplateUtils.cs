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
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace DevExpress.XtraScheduler.Design.ItemTemplates {
	public abstract class FormProjectItemTemplate : ProjectItemTemplate {
		const string AssemblyResourceSuffix = "resxml";
		const string ProjectItemResourceSuffix = "resx";
		const string ProjectItemTemplateConfigFileSuffic = "vstemplate";
		protected FormProjectItemTemplate(ProjectLanguage projectItemLanguage)
			: base(projectItemLanguage) {
		}
		public sealed override string TemplateConfigFile { get { return ObtainTemplateConfigFileName(Name); } }
		protected sealed override void Initialize(Dictionary<string, string> dictionary) {
			base.Initialize(dictionary);
			AddLanguageSpecificItem(dictionary, Name, String.Empty);
			AddLanguageSpecificItem(dictionary, String.Format("{0}.Designer", Name), String.Empty);
			AddItem(dictionary, ObtainAssemblyResourceItemName(Name), ObtainProjectItemName(Name));
		}
		void AddLanguageSpecificItem(Dictionary<string, string> dictionary, string projectItemNameInAssembly, string projectItemName) {
			string assemblyName = GetLanguageSpecificName(projectItemNameInAssembly);
			string itemName = String.IsNullOrEmpty(projectItemName) ? String.Empty : GetLanguageSpecificName(projectItemName);
			AddItem(dictionary, assemblyName, itemName);
		}
		void AddItem(Dictionary<string, string> dictionary, string projectItemNameInAssembly, string projectItemName) {
			dictionary.Add(projectItemNameInAssembly, projectItemName);
		}
		string GetLanguageSpecificName(string projectItemNameInAssembly) {
			string extension = GetLanguageExtension();
			return String.Format("{0}.{1}", projectItemNameInAssembly, extension);
		}
		string ObtainProjectItemName(string itemName) {
			return GetItemName(itemName, ProjectItemResourceSuffix);
		}
		string ObtainAssemblyResourceItemName(string itemName) {
			return GetItemName(itemName, AssemblyResourceSuffix);
		}
		string ObtainTemplateConfigFileName(string itemName) {
			return GetItemName(itemName, ProjectItemTemplateConfigFileSuffic);
		}
		string GetItemName(string itemName, string suffix) {
			return String.Format("{0}.{1}", itemName, suffix);
		}
	}
	public abstract class ProjectItemTemplate {
		protected internal const string RootAssemblyPath = "DevExpress.XtraScheduler.Design.ItemTemplates";
		const string PrefixVB = "VB";
		const string PrefixCS = "CS";
		Dictionary<string, string> items;
		ProjectLanguage projectItemLanguage;
		private ProjectItemTemplate() {
		}
		protected ProjectItemTemplate(ProjectLanguage projectItemLanguage) {
			this.items = new Dictionary<string, string>();
			this.projectItemLanguage = projectItemLanguage;
			Initialize(this.items);
		}
		protected abstract string Name { get; }
		public abstract string TemplateConfigFile { get; }
		public string GetProjectItemName(int indx) {
			if (indx == 0)
				return String.Format("{0}.{1}", Name, GetLanguageExtension());
			return String.Format("{0}{1}.{2}", Name, indx, GetLanguageExtension());
		}
		public List<ItemInfo> GetItems() {
			List<ItemInfo> result = new List<ItemInfo>();
			foreach (KeyValuePair<string, string> item in this.items) {
				string actualName = String.IsNullOrEmpty(item.Value) ? item.Key : item.Value;
				string assemblyName = String.Format("{0}.{1}.{2}.{3}", RootAssemblyPath, GetLanguageAssemblyPathPart(), Name, item.Key);
				result.Add(new ItemInfo(assemblyName, actualName));
			}
			return result;
		}
		protected virtual void Initialize(Dictionary<string, string> dictionary) {
			dictionary.Add(TemplateConfigFile, String.Empty);
		}
		protected string GetLanguageAssemblyPathPart() {
			if (this.projectItemLanguage == ProjectLanguage.VB)
				return PrefixVB;
			if (this.projectItemLanguage == ProjectLanguage.CS)
				return PrefixCS;
			return String.Empty;
		}
		protected string GetLanguageExtension() {
			return GetLanguageAssemblyPathPart().ToLower();
		}		
	}
	public class ItemTemplateFromAssemblyExtractor : IDisposable {
		System.Reflection.Assembly assembly;
		ProjectItemTemplate projectItem;
		string templateFilePath;
		string templateFolderPath;
		public ItemTemplateFromAssemblyExtractor(System.Reflection.Assembly assembly, ProjectItemTemplate projectItem) {
			this.assembly = assembly;
			this.projectItem = projectItem;
			List<ItemInfo> items = projectItem.GetItems();
			this.templateFolderPath = ExpandProjectItemTemplate(items);
			this.templateFilePath = Path.Combine(templateFolderPath, this.projectItem.TemplateConfigFile);
		}
		public string GetTemplateFilePath() {
			return templateFilePath;
		}
		string ExpandProjectItemTemplate(List<ItemInfo> items) {
			string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			DirectoryInfo info = new DirectoryInfo(tempPath);
			if (info.Exists)
				info.Delete(true);
			info.Create();
			foreach (ItemInfo item in items) {
				string content = GetItemContent(item.AssemblyPath);
				if (content == null)
					continue;
				FileInfo fileInfo = new FileInfo(Path.Combine(tempPath, item.ActualName));
				using (FileStream fs = fileInfo.Create()) {
					StreamWriter writer = new StreamWriter(fs);
					writer.Write(content);
					writer.Close();
				}
			}
			return tempPath;
		}
		string GetItemContent(string assemblyPath) {
			String result = String.Empty;
			using (Stream stream = this.assembly.GetManifestResourceStream(assemblyPath)) {
				if (stream == null)
					return null;
				StreamReader reader = new StreamReader(stream);
				result = reader.ReadToEnd();
				reader.Dispose();
			}
			return result;
		}
		public void Dispose() {
			DirectoryInfo info = new DirectoryInfo(this.templateFolderPath);
			if (info.Exists) {
				foreach(FileInfo fileInfo in info.GetFiles()) {
					if (IsFileLocked(fileInfo)) {
						GC.Collect();
						GC.WaitForPendingFinalizers();
					}
					fileInfo.Delete();
				}
				info.Delete(true);
			}
		}
		bool IsFileLocked(FileInfo file) {
			FileStream stream = null;
			try {
				stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			} catch (IOException) {
				return true;
			} finally {
				if (stream != null)
					stream.Close();
			}
			return false;
		}
	}
	public class ItemInfo {
		public ItemInfo(string assemblyPath, string actualName) {
			AssemblyPath = assemblyPath;
			ActualName = actualName;
		}
		public string AssemblyPath { get; private set; }
		public string ActualName { get; private set; }
	}
}
