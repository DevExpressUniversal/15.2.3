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
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Utils;
#if ASP
using System.Web;
#else
using DevExpress.vNext.Internal;
#endif
namespace DevExpress.Web.Internal {
	public abstract class ResourceData {
		public abstract string Key { get; }
		public override string ToString() {
			return Key;
		}
	}
	public class FileResourceData : ResourceData {
		private string url;
		public FileResourceData(string url) {
			this.url = url;
		}
		public string Url {
			get { return url; }
		}
		public override string Key {
			get { return Url; }
		}
	}
	public abstract class EmbeddedResourceData : ResourceData {
		private int index;
		private Assembly assembly;
		private string name;
		private string contentType;
		private object rawContent = null;
		private string themesRootedUrl = null;
		public EmbeddedResourceData(int index, Assembly assembly, string name, string contentType) {
			this.index = index;
			this.assembly = assembly;
			this.name = name;
			this.contentType = contentType;
		}
		public int Index {
			get { return index; }
		}
		public Assembly Assembly {
			get { return assembly; }
		}
		public string Name {
			get { return name; }
		}
		public string ContentType {
			get { return contentType; }
		}
		public object RawContent {
			get {
				if(ResourceManager.UsePhysicalResources)
					return GetRawContent();
				if(rawContent == null)
					rawContent = GetRawContent();
				return rawContent;
			}
		}
		public override string Key {
			get { return ResourceManager.GetResourceKey(Assembly, Name); }
		}
		public string ID {
			get { return ResourceManager.GetResourceID(Assembly, Index); }
		}
		public string ThemesRootedUrl {
			get {
				if(themesRootedUrl == null)
					themesRootedUrl = GetThemesRootedUrl();
				return themesRootedUrl;
			}
		}
		protected abstract object GetRawContent();
		public abstract void WriteContent(Stream stream);
		protected virtual Stream GetResourceStream() {
			if(ResourceManager.UsePhysicalResources)
				return GetPhysicalResourceStream();
			return Assembly.GetManifestResourceStream(Name);
		}
		protected Stream GetPhysicalResourceStream() {
			string fileName = GetResourcePath();
			return File.OpenRead(fileName);
		}
		protected string GetResourcePath() {
			string productFolder = ResourceManager.GetAssemblyShortName(Assembly).Replace(AssemblyInfo.VSuffix, "");
			string name = Name;
			if(name.StartsWith(productFolder))
				name = name.Substring(productFolder.Length + 1);
			List<string> nameParts = new List<string>(name.Split('.'));
			List<string> folders = new List<string>(nameParts);
			folders.RemoveAt(folders.Count - 1);
			FixFrameworks(folders);
			string relativeFilePath = string.Join(@"\", folders.ToArray()) + "." + nameParts[nameParts.Count - 1];
			string[] physicalPaths = ResourceManager.ResourcesPhysicalPath.Split(';');
			string physicalPath = physicalPaths[0];
			if (productFolder.StartsWith("DevExpress.Web.ASPxScheduler")) {
				string specialPysicalPath = FindPhysicalPath(physicalPaths, productFolder);
				if (String.IsNullOrEmpty(specialPysicalPath))
					productFolder = productFolder + @"\" + productFolder;
				else
					physicalPath = specialPysicalPath;
			}
			if(productFolder.StartsWith("DevExpress.XtraCharts.Web"))
				productFolder = @"DevExpress.XtraCharts\" + productFolder;
			return physicalPath + @"\" + productFolder + @"\" + relativeFilePath;
		}
		string FindPhysicalPath(string[] physicalPaths, string productFolder) {
			foreach (string path in physicalPaths)
				if (path.Contains(productFolder))
					return path;
			return String.Empty;
		}
		protected string GetThemesRootedUrl() {
			if(!Name.Contains(ResourceManager.ThemesResourcesRootFolder))
				return string.Empty;
			List<string> nameParts = new List<string>(Name.Split('.'));
			while(nameParts[0] != ResourceManager.ThemesResourcesRootFolder)
				nameParts.Remove(nameParts[0]);
			string extension = nameParts[nameParts.Count - 1];
			nameParts.Remove(extension);
			return string.Join("/", nameParts.ToArray()) + "." + extension;
		}
		static readonly string[] JSLibs = { "globalize", "jquery", "knockout" };
		internal static void FixFrameworks(List<string> folders) {
			int frameworkIndex = 0;
			if(TryGetFrameworkIndex(folders, out frameworkIndex)) {
				MergeFrameworkFolders(folders, frameworkIndex);
			}
			FixFrameworksWithPostfix(folders, "min", "small");
			if(folders.Count >= 3 && folders[folders.Count - 3] == "DevExtreme" && folders[folders.Count - 2] == "dx") {
				folders.RemoveAt(folders.Count - 2);
				folders[folders.Count - 1] = "dx." + folders[folders.Count - 1];
			}
		}
		static void FixFrameworksWithPostfix(List<string> folders, params string[] postfixes) {
			int postfixIndex = folders.Count - 1;
			foreach(var postfix in postfixes) {
				if(folders.Count >= 2 && folders[postfixIndex] == postfix) {
					folders.RemoveAt(folders.Count - 1);
					folders[postfixIndex - 1] += "." + postfix;
					return;
				}
			}
		}
		static bool TryGetFrameworkIndex(List<string> folders, out int index) {
			for(int i = 0; i < folders.Count; i++) {
				var folder = folders[i];
				foreach(var jsLib in JSLibs) {
					if(folder.StartsWith(jsLib, StringComparison.Ordinal)) {
						index = i;
						return true;
					}
				}
			}
			index = 0;
			return false;
		}
		static void MergeFrameworkFolders(List<string> folders, int frameworkIndex) {
			if(frameworkIndex == folders.Count - 1) { 
				return;
			}
			if(folders.Count >= 2 && frameworkIndex == folders.Count - 2) { 
				folders[folders.Count - 2] += "." + folders[folders.Count - 1];
				folders.RemoveAt(folders.Count - 1);
			} else if(frameworkIndex < folders.Count - 2) { 
				var tailArray = new string[folders.Count - frameworkIndex];
				folders.CopyTo(frameworkIndex, tailArray, 0, tailArray.Length);
				var tail = string.Join(".", tailArray);
				folders[frameworkIndex] = tail;
				for(int i = folders.Count - 1; i > frameworkIndex; i--) {
					folders.RemoveAt(i);
				}
			}
		}
	}
	public class StringResourceData : EmbeddedResourceData {
		private bool performSubstitution;
		private Encoding contentEncoding = null;
		public StringResourceData(int id, Assembly assembly, string name, string contentType, bool performSubstitution)
			: base(id, assembly, name, contentType) {
			this.performSubstitution = performSubstitution;
		}
		public Encoding ContentEncoding {
			get { return contentEncoding; }
			protected set { contentEncoding = value; }
		}
		public bool PerformSubstitution {
			get { return performSubstitution; }
			set { performSubstitution = value; }
		}
		public virtual string GetContent() {
			return GetContent(false);
		}
		public int GetContentLength() {
			return GetContent().Length;
		}
		public string GetContent(bool usePhysicalUrls) {
			string content = (string)RawContent;
			if(PerformSubstitution) {
				if(usePhysicalUrls)
					content = ResourceContentParser.ReplaceResourceReferencesWithPhysicalUrls(Name, content);
				else
					content = ResourceContentParser.ReplaceResourceReferencesWithResourceUrls(Name, Assembly, content);
			}
			return content;
		}
		protected override object GetRawContent() {
			string content = string.Empty;
			Stream stream = null;
			StreamReader reader = null;
			try {
				stream = GetResourceStream();
				if(stream != null) {
					reader = new StreamReader(stream, true);
					content = reader.ReadToEnd() + Environment.NewLine;
					ContentEncoding = reader.CurrentEncoding;
				}
			} finally {
				if(reader != null)
					reader.Close();
				if(stream != null)
					stream.Close();
			}
			return content;
		}
		public override void WriteContent(Stream stream) {
			WriteContent(stream, false);
		}
		public void WriteContent(Stream stream, bool usePhysicalUrls) {
			WriteContent(stream, usePhysicalUrls, null);
		}
		public void WriteContent(Stream stream, bool usePhysicalUrls, Function<string, string> replaceMethod) {
			string content = GetContent(usePhysicalUrls);
			if(replaceMethod != null)
				content = replaceMethod(content);
			WriteContentInternal(stream, content);
		}
		protected void WriteContentInternal(Stream stream, string content) {
			if(string.IsNullOrEmpty(content))
				return;
			byte[] bytes = ContentEncoding.GetBytes(content);
			stream.Write(bytes, 0, bytes.Length);
		}
	}
	public class SkinFileResourceData : StringResourceData {
		protected internal const string SharePointSignature = ".SPx";
		public SkinFileResourceData(int id, Assembly assembly, string name, string contentType, bool performSubstitution)
			: base(id, assembly, name, contentType, performSubstitution) {
		}
		protected bool IsMvcExtension {
			get { return Name.Contains(MvcUtils.MvcSignature); }
		}
		protected string MvcSiblingName {
			get { return Name.Replace(MvcUtils.MvcSignature, MvcUtils.AspSignature).Replace(MvcUtils.AspSignature + "Report", ".Report").Replace(MvcUtils.AspSignature + "ChartControl", ".WebChartControl"); }
		}
		protected bool IsSharePointExtension {
			get { return Name.Contains(SharePointSignature); }
		}
		protected string SharePointSiblingName {
			get { return Name.Replace(SharePointSignature, MvcUtils.AspSignature); }
		}
		protected override Stream GetResourceStream() {
			if(IsMvcExtension)
				return Assembly.GetManifestResourceStream(MvcSiblingName);
			if(IsSharePointExtension)
				return Assembly.GetManifestResourceStream(SharePointSiblingName);
			return base.GetResourceStream();
		}
		public override string GetContent() {
			return GetSkinContent(true);
		}
		private string GetSkinContent(bool useSiteRelatedUrls) {
			string content = (string)RawContent;
			if(IsMvcExtension)
				content = MvcUtils.GetMvcSkinContent(content);
			if(!useSiteRelatedUrls)
				content = ResourceContentParser.ReplaceUrlsWithShortUrls(content);
			return AspxCodeUtils.GetPatchedDxControlVersionAndTokenInContent(content);
		}
		public override void WriteContent(Stream stream) {
			WriteContent(stream, false);
		}
		public new void WriteContent(Stream stream, bool useSiteRelatedUrls) {
			WriteContent(stream, useSiteRelatedUrls, null);
		}
		public new void WriteContent(Stream stream, bool useSiteRelatedUrls, Function<string, string> replaceMethod) {
			string content = GetSkinContent(useSiteRelatedUrls);
			if(replaceMethod != null)
				content = replaceMethod(content);
			WriteContentInternal(stream, content);
		}
	}
	public class BinaryResourceData : EmbeddedResourceData {
		public BinaryResourceData(int id, Assembly assembly, string name, string contentType)
			: base(id, assembly, name, contentType) {
		}
		protected override object GetRawContent() {
			byte[] content = new byte[0];
			Stream stream = null;
			try {
				stream = GetResourceStream();
				if(stream != null) {
					content = new byte[stream.Length];
					stream.Read(content, 0, (int)stream.Length);
				}
			} finally {
				if(stream != null)
					stream.Close();
			}
			return content;
		}
		public override void WriteContent(Stream stream) {
			byte[] bytes = (byte[])RawContent;
			if(bytes != null && bytes.Length > 0)
				stream.Write(bytes, 0, bytes.Length);
		}
	}
	public static class ResourceContentParser {
		public static string ReplaceUrlsWithShortUrls(string text) {
			Regex regex = new Regex("~/" + ResourceManager.ThemesResourcesRootFolder + "/[^/]+/", RegexOptions.Singleline | RegexOptions.Multiline);
			return regex.Replace(text, "");
		}
		public static string ReplaceResourceReferencesWithResourceUrls(string resName, Assembly resAssembly, string text) {
			Regex regex = new Regex("<%\\s*=\\s*WebResource\\(\"(?<resourceName>[^\"]*)\"\\)\\s*%>", RegexOptions.Singleline | RegexOptions.Multiline);
			MatchCollection matches = regex.Matches(text);
			int pos = 0;
			StringBuilder stb = new StringBuilder();
			foreach(Match match in matches) {
				stb.Append(text.Substring(pos, match.Index - pos));
				Group group = match.Groups["resourceName"];
				if(group != null) {
					string resourceName = group.ToString();
					if(!string.IsNullOrEmpty(resourceName)) {
						if(string.Equals(resourceName, resName, StringComparison.Ordinal))
							throw new HttpException("Internal error. The circular references was found in the '" + resName + "' resource.");
						if(resourceName.Contains(":")) {
							string[] resourceNameParts = resourceName.Split(':');
							resourceName = resourceNameParts[1];
							if(resourceNameParts[0] == "custom")
								stb.Append(ResourceManager.GetCustomWebResource(resourceName));
							else {
								string assemblyName = resourceNameParts[0];
								if(!ResourceManager.LoadedAssembliesNames.ContainsKey(assemblyName))
									assemblyName += AssemblyInfo.VSuffix;
								if(ResourceManager.LoadedAssembliesNames.ContainsKey(assemblyName))
									stb.Append(ResourceManager.GetResourceUrl(null, ResourceManager.LoadedAssembliesNames[assemblyName], resourceName));
							}
						} else
							stb.Append(ResourceManager.GetResourceUrl(null, resAssembly, resourceName));
					}
				}
				pos = match.Index + match.Length;
			}
			stb.Append(text.Substring(pos, text.Length - pos));
			return stb.ToString();
		}
		public static string ReplaceResourceReferencesWithPhysicalUrls(string resName, string text) {
			Regex regex = new Regex("<%\\s*=\\s*WebResource\\(\"(?<resourceName>[^\"]*)\"\\)\\s*%>", RegexOptions.Singleline | RegexOptions.Multiline);
			MatchCollection matches = regex.Matches(text);
			int pos = 0;
			StringBuilder stb = new StringBuilder();
			foreach(Match match in matches) {
				stb.Append(text.Substring(pos, match.Index - pos));
				Group group = match.Groups["resourceName"];
				if(group != null) {
					string resourceName = group.ToString();
					if(!string.IsNullOrEmpty(resourceName))
						stb.Append(GetResourceRelatedUrl(resName, resourceName));
				}
				pos = match.Index + match.Length;
			}
			stb.Append(text.Substring(pos, text.Length - pos));
			return stb.ToString();
		}
		internal static string GetResourceRelatedUrl(string baseResourceName, string resourceName) {
			if(baseResourceName.Contains("\\")) {
				string[] parts = baseResourceName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0; i < parts.Length; i++) {
					if(resourceName.StartsWith(parts[i])) {
						baseResourceName = string.Join(".", parts, i, parts.Length - i);
						break;
					}
				}
			}
			string[] pathParts = baseResourceName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			string path = string.Join(".", pathParts, 0, pathParts.Length - 2);
			string urlPrefix = "";
			while(true) {
				if(!resourceName.Contains(ResourceManager.ThemesResourcesRootFolder)) { 
					string[] urlParts = resourceName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
					return urlParts[urlParts.Length - 2] + "." + urlParts[urlParts.Length - 1];
				} else if(resourceName.IndexOf(path + ".") == 0) {
					string relatedName = resourceName.Replace(path + ".", string.Empty);
					string[] urlParts = relatedName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
					return urlPrefix + string.Join("/", urlParts, 0, urlParts.Length - 1) + "." + urlParts[urlParts.Length - 1];
				} else {
					urlPrefix += "../";
					int dotPos = path.LastIndexOf(".");
					if(dotPos > -1)
						path = path.Substring(0, dotPos);
					else {
						string[] urlParts = resourceName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
						return string.Join("/", urlParts, 0, urlParts.Length - 1) + "." + urlParts[urlParts.Length - 1];
					}
				}
			}
		}
	}
}
