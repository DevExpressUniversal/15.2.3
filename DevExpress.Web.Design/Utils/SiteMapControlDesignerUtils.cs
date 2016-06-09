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
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using DevExpress.Web.Internal;
using DevExpress.Web.Design;
using EnvDTE;
namespace DevExpress.Web.Design.Utils {
	public class DesignerUtils {
		public static string fDefaultSiteMapFileNameFormat = "web{0}.sitemap";
		private static string fSiteMapFileRegEx = "web\\d*.sitemap";
		private static string fSiteMapFileNamePrefix = "web";
		private static string fSiteMapFileExtension = ".sitemap";
		private static int fDefaultSiteMapNodeCount = 2;
		public static object AddNewSiteMapFileToProject(object curProjectObj, 
			IWebApplication webApplication, string siteMapFileName, bool needToFillByDefaultData) {
			EnvDTE.Project curProject = curProjectObj as EnvDTE.Project;
			string tempFileName = webApplication.RootProjectItem.PhysicalPath +
				DesignerUtils.GenerateSiteMapFileName();
			File.Create(tempFileName).Close();
			if (needToFillByDefaultData) {
				UnboundSiteMapProvider provider = CreateDefaultDataSiteMapProvider();
				provider.SaveToFile(tempFileName);
			}
			object parentDirObject = EnvDTEHelper.CreateDirectoryProjectItemByPath(siteMapFileName, curProject, webApplication);
			EnvDTE.ProjectItem newProjectItem = parentDirObject != null ? (parentDirObject as EnvDTE.ProjectItem).ProjectItems.AddFromFileCopy(tempFileName) :
				curProject.ProjectItems.AddFromFileCopy(tempFileName);
			newProjectItem.Name = Path.GetFileName(siteMapFileName);
			File.Delete(tempFileName);
			return newProjectItem;
		}
		public static List<TreeNode> GetAllTreeNodes(TreeView treeView) {
			List<TreeNode> ret = new List<TreeNode>();
			AddTreeNodesRecursive(treeView.Nodes, ret);
			return ret;
		}
		public static string GetNextSiteMapFileName(string[] fileNames) {
			int nextFileNumber = 0;
			string ret = string.Format(fDefaultSiteMapFileNameFormat, "");
			if (fileNames.Length != 0) {
				List<int> fileNumbers = new List<int>();
				Regex regExMasterPage = new Regex(fSiteMapFileRegEx, RegexOptions.IgnoreCase);
				for (int i = 0; i < fileNames.Length; i++) {
					string curFileName = fileNames[i].ToLower();
					if (regExMasterPage.IsMatch(curFileName)) {
						string numberString = curFileName.Replace(fSiteMapFileNamePrefix, "");
						numberString = numberString.Replace(fSiteMapFileExtension, "");
						int fileNumber = 0;
						if (!string.IsNullOrEmpty(numberString))
							fileNumber = int.Parse(numberString);
						fileNumbers.Add(fileNumber);
					}
				}
				fileNumbers.Sort();
				nextFileNumber = GetNextSmallestNumber(fileNumbers.ToArray());
			}
			if (nextFileNumber != 0)
				ret = string.Format(fDefaultSiteMapFileNameFormat, nextFileNumber.ToString());
			return ret;
		}
		public static bool IsAbsolutePhysicalPath(string path) { 
			if ((path != null) && (path.Length >= 3)) {
				if (path.StartsWith(@"\\", StringComparison.Ordinal))
					return true;
				if (char.IsLetter(path[0]) && (path[1] == ':'))
					return (path[2] == '\\');
			}
			return false;
		}
		public static bool IsExistProjectItem(string url, IWebApplication webApplication) {
			return EnvDTEHelper.IsExistProjectItem(url, webApplication);
		}
		private static UnboundSiteMapProvider CreateDefaultDataSiteMapProvider() {
			UnboundSiteMapProvider ret = new UnboundSiteMapProvider("", "");
			for (int i = 0; i < fDefaultSiteMapNodeCount; i++)
				ret.AddSiteMapNode(ret.CreateNode("", ""));
			return ret;
		}
		private static string GenerateSiteMapFileName() {
			Guid guid = new Guid();
			return guid.ToString() + ".sitemap";
		}
		private static int GetNextSmallestNumber(int [] numbers) {
			int ret = 0;
			int count = numbers[0] != 0 ? numbers.Length : numbers.Length - 1;
			if (count == numbers[numbers.Length - 1])
				ret = count + 1;
			else {
				if (numbers[0] >= count) 
					ret = 0;
				else {
					int index = 0;
					while (numbers[index + 1] - numbers[index] == 1)
						index++;
					ret = numbers[index] + 1;
				}
			}
			return ret;
		}
		private static void AddTreeNodesRecursive(TreeNodeCollection nodes, List<TreeNode> collection) {
			foreach (TreeNode node in nodes) {
				if (node.Nodes.Count != 0)
					AddTreeNodesRecursive(node.Nodes, collection);
				collection.Add(node);
			}
		}
	}
	public class SiteMapAutoGenerator {
		private const string MasterPageRegEx = @"\<%@( )*Page[^(%\>)]+%\>";
		private const string AttributeRegEx = "{0}( )*=( )*\"[^\"]+\\\"";
		private const string TagRegEx = @"<{0}>.*?</{0}>";
		private const string TagValueRegEx = @">.*?<";
		private static string RootNodeTitle = "Web Site";
		private static List<string> fProjectItemsExtension =
			new List<string>(new string[] { ".htm", ".html", ".aspx", ".xml" });
		public void GenerateSiteMapByProject(object projectObj, UnboundSiteMapProviderBase provider,
			IWebApplication webApplication) {
			Project project = projectObj as Project;
			provider.RootNode.Url = "";
			provider.RootNode.Title = RootNodeTitle;
			foreach (ProjectItem item in project.ProjectItems)
				GenerateSiteMapInternal(item, provider, provider.RootNode, webApplication);
			RemoveNodeWithoutExtension(provider.RootNode);
		}
		protected SiteMapNode CreateSiteMapNode(string projectItemName, 
			string projectItemPath, SiteMapNode parentNode) {
			UnboundSiteMapProviderBase provider = parentNode.Provider as UnboundSiteMapProviderBase;
			string url = "";
			if (parentNode.Url != "")
				url = parentNode.Url + projectItemName;
			else
				url = "~/" + projectItemName;
			if (Path.GetFileName(projectItemPath) == "")
				url += "/";
			string title = Path.GetFileName(projectItemPath) != "" ? GetTitleByFile(projectItemPath) : projectItemName;
			return provider.CreateNode(url, title);
		}
		private bool IsContainChildWithExtension(SiteMapNode parentNode) {
			foreach (SiteMapNode node in parentNode.ChildNodes) {
				if (Path.GetExtension(node.Url) == "") {
					if (IsContainChildWithExtension(node))
						return true;
				}
				else
					return true;
			}
			return false;
		}
		private bool IsFolderItem(string name) {
			return Path.GetExtension(name) == "";
		}
		private void GenerateSiteMapInternal(ProjectItem parentProjectItem, UnboundSiteMapProviderBase provider,
			SiteMapNode parentNode, IWebApplication webApplication) {
			SiteMapNode newNode = null;
			string projectItemFileName = parentProjectItem.get_FileNames(0);
			string projectItemFileNameExtension = Path.GetExtension(projectItemFileName);
			if (fProjectItemsExtension.Contains(projectItemFileNameExtension) ||
				(IsFolderItem(projectItemFileName) && (parentProjectItem.ProjectItems != null))) {
				newNode = CreateSiteMapNode(parentProjectItem.Name, projectItemFileName, parentNode);
				provider.AddSiteMapNode(newNode, parentNode);
			}
			if ((newNode != null) && IsFolderItem(projectItemFileName))
				foreach (ProjectItem item in parentProjectItem.ProjectItems)
					GenerateSiteMapInternal(item, provider, newNode, webApplication);
		}
		private string GetTitleByFile(string fileName) {
			using (TextReader sr = new StreamReader(fileName, Encoding.Default)) {
				string title = "";
				string content = sr.ReadToEnd();
				title = GetPageTitle(content);
				if (title == "")
					title = GetPageWithMasterPageTitle(content);
				if (title == "")
					title = Path.GetFileName(fileName);
				return title;
			}
		}
		private string GetPageTitle(string fileContentString) {
			string ret = "";
			Regex regExTitleTag = new Regex(string.Format(TagRegEx, "title"), RegexOptions.IgnoreCase);
			MatchCollection titleTagMatch = regExTitleTag.Matches(fileContentString);
			if (titleTagMatch.Count != 0) {
				Regex titleTagValueRegEx = new Regex(TagValueRegEx, RegexOptions.IgnoreCase);
				MatchCollection titleValueMath = titleTagValueRegEx.Matches(titleTagMatch[0].Value);
				if (titleValueMath.Count != 0)
					ret = titleValueMath[0].Value.Substring(1, titleValueMath[0].Value.Length - 2);
			}
			return ret;
		}
		private string GetPageWithMasterPageTitle(string fileContentString) {
			string ret = "";
			Regex regExMasterPage = new Regex(MasterPageRegEx, RegexOptions.IgnoreCase);
			MatchCollection masterPageMatch = regExMasterPage.Matches(fileContentString);
			if (masterPageMatch.Count != 0) {
				Regex regExAttribute = new Regex(string.Format(AttributeRegEx, "title"), RegexOptions.IgnoreCase);
				MatchCollection attributeTitleMatch = regExAttribute.Matches(masterPageMatch[0].Value);
				if (attributeTitleMatch.Count != 0) {
					int startIndex = attributeTitleMatch[0].Value.IndexOf("\"");
					int endIndex = attributeTitleMatch[0].Value.IndexOf("\"", startIndex + 1);
					if ((startIndex != -1) && (endIndex != -1))
						ret = attributeTitleMatch[0].Value.Substring(startIndex + 1, endIndex - startIndex - 1);
				}
			}
			return ret;
		}
		private void RemoveNodeWithoutExtension(SiteMapNode parentNode) {
			int i = 0;
			while (i < parentNode.ChildNodes.Count) {
				SiteMapNode node = parentNode.ChildNodes[i];
				string str = Path.GetFileName(node.Url);
				if ((!node.Url.Contains(".")) && (!IsContainChildWithExtension(node))) {
					UnboundSiteMapProviderBase provider = node.Provider as UnboundSiteMapProviderBase;
					provider.RemoveSiteMapNode(node);
					i--;
				}
				else
					RemoveNodeWithoutExtension(node);
				i++;
			}
		}
	}
	public class SiteMapScanner {
		private SiteMapAutoGenerator fSiteMapAutoGenerator = null;
		private ScanSiteMapProvider fSiteMapProvider = null;
		public ScanSiteMapProvider SiteMapProvider {
			get { return fSiteMapProvider; }
		}
		public SiteMapScanner(IDesignerHost designerHost) {
			fSiteMapAutoGenerator = new SiteMapAutoGenerator();
			fSiteMapProvider = new ScanSiteMapProvider(designerHost, ProviderDataState.Empty);
		}
		public void ScanSiteMap(object projectObj, EditableSiteMapProvider destProvider, IWebApplication webApplication) {
			Project project = projectObj as Project;
			fSiteMapAutoGenerator.GenerateSiteMapByProject(project, fSiteMapProvider, webApplication);
			Dictionary<string, SiteMapNode> destUrlTable = CreateUrlTable(destProvider);
			Dictionary<string, SiteMapNode> urlTable = CreateUrlTable(fSiteMapProvider);
			foreach (string url in destUrlTable.Keys) {
				if (urlTable.ContainsKey(url))
					RemoveNodeWithDuplicatedUrl(urlTable[url], urlTable, destUrlTable);
			}
			RemoveEmptyFolders(urlTable);
		}
		protected void RemoveEmptyFolders(Dictionary<string, SiteMapNode> urlTable) {
			foreach (SiteMapNode node in urlTable.Values) {
				if (IsFolder(node) && ContainOnlyEmptyFolder(node)) {
					(node.Provider as UnboundSiteMapProviderBase).RemoveSiteMapNode(node);
				}
			}
		}
		protected bool ContainOnlyEmptyFolder(SiteMapNode parentNode) {
			SiteMapNodeCollection nodes = parentNode.GetAllNodes();
			foreach (SiteMapNode node in nodes) {
				if (!IsFolder(node))
					return false;
			}
			return true;
		}
		protected static bool ContainUnduplictedUrl(SiteMapNode parentNode, Dictionary<string, SiteMapNode> destUrlTable) {
			if (parentNode.HasChildNodes) {
				foreach (SiteMapNode node in parentNode.ChildNodes)
					if (!destUrlTable.ContainsKey(GetNodeUrl(node)))
						return true;
					else
						if (ContainUnduplictedUrl(node, destUrlTable))
							return true;
			}
			if (Path.GetExtension(parentNode.Url) == "")
				return false;
			return !destUrlTable.ContainsKey(GetNodeUrl(parentNode));
		}
		protected static void RemoveNodeWithDuplicatedUrl(SiteMapNode parentNode, Dictionary<string, SiteMapNode> urlTable, Dictionary<string, SiteMapNode> destUrlTable) {
			SiteMapNodeCollection nodeBranch = new SiteMapNodeCollection(parentNode.ChildNodes);
			UnboundSiteMapProviderBase provider = parentNode.Provider as UnboundSiteMapProviderBase;
			foreach (SiteMapNode node in nodeBranch) {
				if (!ContainUnduplictedUrl(node, destUrlTable)) {
					provider.RemoveSiteMapNode(node);
					urlTable.Remove(GetNodeUrl(node));
				}
			}
			if ((!parentNode.HasChildNodes) && (destUrlTable.ContainsKey(GetNodeUrl(parentNode)))) {
				provider.RemoveSiteMapNode(parentNode);
				urlTable.Remove(GetNodeUrl(parentNode));
			}
		}
		protected static Dictionary<string, SiteMapNode> CreateUrlTable(UnboundSiteMapProviderBase provider) {
			Dictionary<string, SiteMapNode> ret = new Dictionary<string, SiteMapNode>();
			ret.Add(GetNodeUrl(provider.RootNode), provider.RootNode);
			SiteMapNodeCollection nodes = provider.RootNode.GetAllNodes();
			foreach (SiteMapNode node in nodes) {
				string nodeUrl = GetNodeUrl(node);
				if ((nodeUrl != "") && (!ret.ContainsKey(nodeUrl)))
					ret.Add(nodeUrl, node);
			}
			return ret;
		}
		private bool IsFolder(SiteMapNode node) {
			return Path.GetExtension(node.Url) == "";
		}
		private static string GetNodeUrl(SiteMapNode node) {
			return UrlInfo.GetNormilizedUrl(node.Url);
		}
	}
	public class UrlInfo {
		private const string WString = "www.";
		private Dictionary<string, int> fStringCounts = null;
		private Regex fValidURLRegEx = new Regex(@"^(http|https|ftp)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&amp;%\$#\=~_\-]+))*$");
		public UrlInfo(string[] strings) {
			for (int i = 0; i < strings.Length; i++)
				strings[i] = GetNormilizedUrl(strings[i]);
			fStringCounts = GetStringCounts(strings);
		}
		public void AddString(string str) {
			if (!string.IsNullOrEmpty(str)) {
				string normStr = GetNormilizedUrl(str);
				if (fStringCounts.ContainsKey(normStr))
					fStringCounts[normStr]++;
				else
					fStringCounts.Add(normStr, 1);
			}
		}
		public void ClearAll() {
			fStringCounts.Clear();
		}
		public bool ContainDuplicatedStrings() {
			foreach (string str in fStringCounts.Keys)
				if (IsDuplicatedString(str))
					return true;
			return false;
		}
		public bool ContainInvalidUrl() {
			foreach (string str in fStringCounts.Keys)
				if (!IsValidUrl(str))
					return true;
			return false;
		}
		public bool ContainString(string str) {
			return fStringCounts.ContainsKey(GetNormilizedUrl(str));
		}
		public void DeleteString(string str) {
			string normStr = GetNormilizedUrl(str);
			if (fStringCounts.ContainsKey(normStr)) {
				if (fStringCounts[normStr] == 1)
					fStringCounts.Remove(normStr);
				else
					fStringCounts[normStr]--;
			}
		}
		public static string GetNormilizedUrl(string rawUrl) {
			rawUrl = rawUrl.ToLower();
			if (!DesignerUtils.IsAbsolutePhysicalPath(rawUrl) && !UrlUtils.IsAbsoluteUrl(rawUrl)
				&& !rawUrl.StartsWith(WString)) {
				if (UrlUtils.IsAppRelativePath(rawUrl))
					return rawUrl;
				else {
					if (!UrlUtils.IsAbsoluteVirtualPath(rawUrl))
						return "~/" + rawUrl;
					else
						return rawUrl;
				}
			}
			return rawUrl; 
		}
		public static string GetFullUrl(string rawUrl) {
			if (!UrlUtils.IsAbsoluteUrl(rawUrl)) {
				if (!DesignerUtils.IsAbsolutePhysicalPath(rawUrl)) {
					if (UrlUtils.IsAppRelativePath(rawUrl)) {
						if (rawUrl.Length == 1)
							return "http://localhost/";
						else
							return rawUrl.Replace("~/", "http://localhost/");
					}
					else {
						if (!UrlUtils.IsAbsoluteVirtualPath(rawUrl))
							return rawUrl.StartsWith(WString) ? "http://" + rawUrl:"http://localhost/" + rawUrl;
						else
							return "http://localhost" + rawUrl;
					}
				}
			}
			return rawUrl;
		}
		public bool IsDuplicatedString(string str) {
			if (str != "") {
				if (fStringCounts.ContainsKey(GetNormilizedUrl(str.ToLower())))
					if (fStringCounts[GetNormilizedUrl(str.ToLower())] > 1)
						return true;
			}
			return false;
		}
		public bool IsValidUrl(string url) {
			if (url != "")
				return fValidURLRegEx.IsMatch(GetFullUrl(url));
			return true;			
		}
		protected Dictionary<string, int> GetStringCounts(string[] strings) {
			Dictionary<string, int> ret = new Dictionary<string, int>();
			for (int i = 0; i < strings.Length; i++) {
				if (!ret.ContainsKey(strings[i])) {
					ret.Add(strings[i], 1);
					for (int j = i + 1; j < strings.Length; j++)
						if (String.Compare(strings[i], strings[j]) == 0)
							ret[strings[i]]++;
				}
			}
			return ret;
		}
	}
}
