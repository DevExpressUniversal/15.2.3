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
using System.Collections;
using System.Reflection;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum FrameworkVersion
  {
	Unknown,
	Version10,
	Version11,
	Version20,
	Version30,
	Version35,
	Version40,
	Version45
  }
#if !SL
	public class FrameworkHelper
	{
		const string STR_NETFrameWork = "SOFTWARE\\Microsoft\\.NETFramework";
		const string STR_NETFremeworkAssemblyFolders = "SOFTWARE\\Microsoft\\.NETFramework\\AssemblyFolders";
		const string STR_GACKey = "SOFTWARE\\Microsoft\\Fusion";
		const string STR_CacheLocation = "CacheLocation";
		const string STR_InstallRoot = "InstallRoot";
		const string STR_MsCorLib = "mscorlib.dll";
		const string STR_MicrosoftVisualBasic = "Microsoft.VisualBasic.dll";
		const string STR_Star = "*";
		const string Framework10 = "v1.0";
		const string Framework11 = "v1.1";
		const string Framework20 = "v2.0";
		const string Framework30 = "v3.0";
		const string Framework35 = "v3.5";
	const string Framework40 = "v4.0";
	const string Framework45 = "v4.5";
	const string STR_NETSilverlightSdk = "SOFTWARE\\Microsoft\\Microsoft SDKs\\Silverlight";
	const string STR_AssemblyFoldersEx = "AssemblyFoldersEx";
	const string STR_ReferenceAssemblies = "ReferenceAssemblies";
	const string STR_SLRuntimeInstallPath = "SLRuntimeInstallPath";
	const string STR_VS2010Key = "SOFTWARE\\Microsoft\\VisualStudio\\10.0";
		const string STR_VS2008Key = "SOFTWARE\\Microsoft\\VisualStudio\\9.0";
		const string STR_VS2005Key = "SOFTWARE\\Microsoft\\VisualStudio\\8.0";
		private FrameworkHelper(){}
		static string GetVersionPrefix(FrameworkVersion version)
		{
			switch (version)
			{
				case FrameworkVersion.Version10:
					return Framework10;
				case FrameworkVersion.Version11:
					return Framework11;
				case FrameworkVersion.Version20:
					return Framework20;
				case FrameworkVersion.Version30:
					return Framework30;
				case FrameworkVersion.Version35:
					return Framework35;
		case FrameworkVersion.Version40:
		  return Framework40;
		case FrameworkVersion.Version45:
		  return Framework45;
			}
			return String.Empty;
		}
		static bool ContainsKey(string[] names, string key)
		{
			if (names == null || names.Length == 0 || key == null)
				return false;
			int count = names.Length;
			for (int i = 0; i < count; i++)
			{
				string name = names[i];
				if (String.Compare(name, key, true) == 0)
					return true;
			}
			return false;
		}
		static string GetGACPath()
		{
			RegistryPermission permission = new RegistryPermission(RegistryPermissionAccess.Read, STR_GACKey);
			permission.Demand();
			string cacheLocation = String.Empty;
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(STR_GACKey))
			{
				string[] subKeys = key.GetSubKeyNames();
				if (ContainsKey(subKeys, STR_CacheLocation))
					cacheLocation = (string)key.GetValue(STR_CacheLocation);
			}
			if (cacheLocation == null || cacheLocation.Length == 0)
			{
				string systemDir = Environment.SystemDirectory;
				string parentSystemDir = Path.GetDirectoryName(systemDir);
				cacheLocation = Path.Combine(parentSystemDir, "Assembly");
			}
			return cacheLocation;
		}
		static string[] GetAssemblySearchPaths()
		{
			ArrayList lPaths = new ArrayList();
			lPaths.Add(GetActiveFrameworkPath());
			lPaths.AddRange(GetFrameworkPaths(FrameworkVersion.Version10));
			lPaths.AddRange(GetFrameworkPaths(FrameworkVersion.Version11));
			lPaths.AddRange(GetFrameworkPaths(FrameworkVersion.Version20));
			lPaths.AddRange(GetFrameworkPaths(FrameworkVersion.Version30));
			lPaths.AddRange(GetFrameworkPaths(FrameworkVersion.Version35));
	  lPaths.AddRange(GetFrameworkPaths(FrameworkVersion.Version40));
			return (string[])lPaths.ToArray(typeof(string));
		}
		static string[] FindAssemblyFiles(string path, string pattern)
		{
			if (path == null || path.Length == 0)
				return new string[0];
			if (pattern == null || pattern.Length == 0)
				return new string[0];
			ArrayList result = new ArrayList();
			FindAssemblyFiles(result, path, pattern);
			return (string[])result.ToArray(typeof(string));
		}
		static void FindAssemblyFiles(ArrayList result, string path, string pattern)
		{
	  try
	  {
		DirectoryInfo info = new DirectoryInfo(path);
		if (!info.Exists)
		  return;
		string[] files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
		if (files != null && files.Length > 0)
		{
		  result.AddRange(files);
		  return;
		}
	  }
	  catch
	  {
		return;
	  }
		}
		static string[] GetAssemblyFiles(string path, string name)
		{
	  if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name))
		return null;
			string assemblyFile = name.EndsWith(".dll") ? name : String.Concat(name, ".dll");
			return FindAssemblyFiles(path, assemblyFile);			
		}
		static string SearchForAssembly(string[] paths, string assemblyFileName)
		{
			if (paths == null || paths.Length == 0)
				return null;
			if (assemblyFileName == null || assemblyFileName.Length == 0)
				return null;
			for (int i = 0; i < paths.Length; i ++)
			{
				string lPath = paths[i];
				string[] lFiles = GetAssemblyFiles(lPath, assemblyFileName);
				if (lFiles != null && lFiles.Length > 0)
					return lFiles[0];
			}
			return null;
		}
		static string SearchForAssembly(string[] paths, AssemblyName name)
		{
			if (paths == null || paths.Length == 0)
				return null;
			if (name == null)
				return null;
			for (int i = 0; i < paths.Length; i ++)
			{
				string lPath = paths[i];
				string[] lFiles = GetAssemblyFiles(lPath, name.Name);
				string lAssemblyPath = SearchForAssemblyInsideFiles(lFiles, name);
				if (lAssemblyPath != null)
					return lAssemblyPath;
			}
			return null;
		}
		static string SearchForAssemblyInsideFiles(string[] files, AssemblyName name)
		{
			if (files == null || files.Length == 0)
				return null;
			for (int i = 0; i < files.Length; i++)
			{
				string lFile = files[i];
				AssemblyName lAssemblyName = null;
				try
				{
					lAssemblyName = AssemblyName.GetAssemblyName(lFile);
				}
				catch
				{
					continue;
				}
				if (lAssemblyName == null)
					continue;
				if (lAssemblyName.FullName == name.FullName)
					return lFile;
			}
			return null;
		}
	static void ReadAssemblyFolders(List<string> result, RegistryKey key)
	{
	  if (result == null || key == null)
		return;
	  using (RegistryKey assemblyFoldersKey = key.OpenSubKey(STR_AssemblyFoldersEx))
	  {
		if (assemblyFoldersKey == null)
		  return;
		foreach (string subKeyName in assemblyFoldersKey.GetSubKeyNames())
		{
		  using (RegistryKey subKey = assemblyFoldersKey.OpenSubKey(subKeyName))
		  {
			if (subKey == null)
			  continue;
			string path = subKey.GetValue(String.Empty) as string;
			if (!string.IsNullOrEmpty(path))
			  result.Add(path);
		  }
		}
	  }
	}
	static void ReadReferenceAssemblies(List<string> result, RegistryKey key, string valueName)
	{
	  if (result == null || key == null)
		return;
	  using (RegistryKey referenceAssembliesKey = key.OpenSubKey(STR_ReferenceAssemblies))
	  {
		string path = (string)referenceAssembliesKey.GetValue(valueName);
		if (!string.IsNullOrEmpty(path))
		  result.Add(path);
	  }
	}
	static string[] GetSilverlightAssemblySearchPaths()
	{
	  List<string> result = new List<string>();
	  RegistryPermission regPermission = new RegistryPermission(RegistryPermissionAccess.Read, STR_NETSilverlightSdk);
	  regPermission.Demand();
	  using (RegistryKey key = Registry.LocalMachine.OpenSubKey(STR_NETSilverlightSdk))
	  {
		string[] versions = key.GetSubKeyNames();
		if (versions == null || versions.Length == 0)
		  return null;
		string version = versions[versions.Length - 1];
		using (RegistryKey versionKey = key.OpenSubKey(version))
		{
		  ReadAssemblyFolders(result, versionKey);
		  ReadReferenceAssemblies(result, versionKey, STR_SLRuntimeInstallPath);
		}
	  }
	  return result.ToArray();
	}
		static string GetInstallDirKey(string registryFolder)
		{
			if (string.IsNullOrEmpty(registryFolder))
				return string.Empty;
			RegistryPermission regPermission = new RegistryPermission(RegistryPermissionAccess.Read, registryFolder);
			regPermission.Demand();
	  using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryFolder))
	  {
		if (key == null)
		  return string.Empty;
		return key.GetValue("InstallDir") as string;
	  }
		}
	static string GetFrameworkIntstallRootPath()
	{
	  RegistryPermission lRegPermission = new RegistryPermission(RegistryPermissionAccess.Read, STR_NETFrameWork);
	  lRegPermission.Demand();
	  using (RegistryKey lKey = Registry.LocalMachine.OpenSubKey(STR_NETFrameWork))
	  {
		return (string)lKey.GetValue(STR_InstallRoot);
	  }
	}
		public static string GetFrameworkRootPath()
		{
	  return GetFrameworkIntstallRootPath();
	}
	public static string GetFrameworkRootPath(FrameworkVersion version)
	{
	  if (version == FrameworkVersion.Version20 || version == FrameworkVersion.Version30 || version == FrameworkVersion.Version35)
		return GetFrameworkIntstallRootPath();
	  const string referenceAssembliesFolder = @"\Reference Assemblies\Microsoft\Framework\.NETFramework\";
	  string programFiles = Environment.GetEnvironmentVariable("programfiles");
	  string path = programFiles + referenceAssembliesFolder;
	  if (Directory.Exists(path))
		return path;
	  programFiles = Environment.GetEnvironmentVariable("programfiles(x86)");
	  path = programFiles + referenceAssembliesFolder;
	  if (Directory.Exists(path))
		return path;
	  return GetFrameworkIntstallRootPath();
		}
		public static string[] GetFrameworkPaths(FrameworkVersion version)
		{
			string root = GetFrameworkRootPath(version);
			string versionPrefix = GetVersionPrefix(version);
			if (versionPrefix == null || versionPrefix.Length == 0)
				return new string[0];
			string[] directories = Directory.GetDirectories(root, String.Concat(versionPrefix, STR_Star));
			if (version != FrameworkVersion.Version30 && version != FrameworkVersion.Version35 && version != FrameworkVersion.Version40)
				return directories;
			ArrayList result = new ArrayList();
			result.AddRange(directories);
			try
			{
				using (RegistryKey assemblyFolders = Registry.LocalMachine.OpenSubKey(STR_NETFremeworkAssemblyFolders + "\\" + versionPrefix, false))
					if (assemblyFolders != null)
					{
						string key = assemblyFolders.GetValue("All Assemblies In") as string;
						if (!string.IsNullOrEmpty(key))
							result.Add(key);
					}
			}
			catch { }
			return (string[])result.ToArray(typeof(string));
		}
		public static string GetFrameworkPath(FrameworkVersion version)
		{
			string[] directories = GetFrameworkPaths(version);
			if (directories == null || directories.Length == 0)
				return String.Empty;
			return directories[0];
		}
		public static string GetActiveFrameworkPath()
		{
			string path = GetActiveMSCorLibPath();
			if (string.IsNullOrEmpty(path))
				return null;
			return Path.GetDirectoryName(path);
		}
		public static string GetMSCorLibPath(FrameworkVersion version)
		{
	  if (version == FrameworkVersion.Unknown)
		return GetActiveMSCorLibPath();
			string[] paths = GetFrameworkPaths(version);
	  if (paths == null || paths.Length == 0)
				return String.Empty;
	  for (int i = paths.Length - 1; i >= 0; i--)
	  {
		string msCorLibPath = Path.Combine(paths[i], STR_MsCorLib);
		if (File.Exists(msCorLibPath))
		  return msCorLibPath;
	  }
	  return string.Empty;
		}
	public static string GetVBRuntimeLibPath()
	{
	  return GetVBRuntimeLibPath(FrameworkVersion.Unknown);
	}
		public static string GetVBRuntimeLibPath(FrameworkVersion frameworkVersion)
		{
			string mscorlibPath = GetMSCorLibPath(frameworkVersion);
			if (string.IsNullOrEmpty(mscorlibPath))
				return null;
			int slashIndex = mscorlibPath.LastIndexOf('\\');
			if (slashIndex < 0)
				return null;
			return String.Format("{0}\\{1}", mscorlibPath.Substring(0, slashIndex), STR_MicrosoftVisualBasic);
		}
		public static string GetActiveMSCorLibPath()
		{
			Type lType = typeof(int);
			if (lType == null)
				return null;
			Assembly lAssembly = lType.Assembly;
			if (lAssembly == null)
				return null;
			return lAssembly.Location;
		}
	public static string[] GetAssemblyFoldersPaths()
	{
	  ArrayList result = new ArrayList();
	  try
	  {
		using (RegistryKey assemblyFolders = Registry.LocalMachine.OpenSubKey(STR_NETFremeworkAssemblyFolders, false))
		{
		  if (assemblyFolders == null)
			return new string[0];
		  foreach (string subKeyName in assemblyFolders.GetSubKeyNames())
			using (RegistryKey subKey = assemblyFolders.OpenSubKey(subKeyName, false))
			{
			  string key = subKey.GetValue("All Assemblies In") as string;
			  if (string.IsNullOrEmpty(key))
				key = subKey.GetValue("") as string;
			  if (!string.IsNullOrEmpty(key))
				result.Add(key);
			}
		}
	  }
	  catch { }
	  return (string[])result.ToArray(typeof(string));
	}
	public static string[] GetAssemblyFoldersPaths(FrameworkVersion version)
	{
	  List<string> result = new List<string>();
	  try
	  {
		using (RegistryKey assemblyFolders = Registry.LocalMachine.OpenSubKey(STR_NETFrameWork, false))
		{
		  if (assemblyFolders == null)
			return result.ToArray();
		  string[] subKeys = assemblyFolders.GetSubKeyNames();
		  if (subKeys == null || subKeys.Length == 0)
			return result.ToArray();
		  string versionPrefix = GetVersionPrefix(version);
		  if (string.IsNullOrEmpty(versionPrefix))
			return result.ToArray();
		  List<string> versionSubKeys = new List<string>();
		  foreach(string subKey in subKeys)
		  {
			if(string.IsNullOrEmpty(subKey))
			  continue;
			if (subKey.StartsWith(versionPrefix))
			  versionSubKeys.Add(subKey);
		  }
		  foreach (string versionSubKey in versionSubKeys)
		  {
			try
			{
			  using (RegistryKey versionKey = assemblyFolders.OpenSubKey(versionSubKey))
			  {
				ReadAssemblyFolders(result, versionKey);
			  }
			}
			catch 
			{
			  continue;
			}
		  }
		}
	  }
	  catch { }
	  return result.ToArray();
	}
		public static string[] GetVSInstallFoldersPaths()
		{
			ArrayList result = new ArrayList();
	  string path = GetInstallDirKey(STR_VS2010Key);
	  if (!string.IsNullOrEmpty(path))
		result.Add(path);
			path = GetInstallDirKey(STR_VS2008Key);
			if (!string.IsNullOrEmpty(path))
				result.Add(path);
			path = GetInstallDirKey(STR_VS2005Key);
			if (!string.IsNullOrEmpty(path))
				result.Add(path);
			return (string[])result.ToArray(typeof(string));
		}
		public static bool IsInstalled(FrameworkVersion version)
		{
			string lPath = GetFrameworkPath(version);
			return lPath != null && lPath.Length > 0;
		}
		public static string GetAssemblyPath(AssemblyName name)
		{
			if (name == null)
				return null;
			string lPath = name.EscapedCodeBase;
			if (lPath != null)
				return PathUtils.GetLocalPath(lPath);
			string[] lSearchPaths = GetAssemblySearchPaths();
			return SearchForAssembly(lSearchPaths, name);
		}
		public static string GetAssemblyPath(string assemblyFileName)
		{
			if (assemblyFileName == null || assemblyFileName.Length == 0)
				return null;
			string[] lSearchPaths = GetAssemblySearchPaths();
			return SearchForAssembly(lSearchPaths, assemblyFileName);
		}
	public static string GetAssemblyPath(string assemblyFileName, string[] paths)
	{
	  if (assemblyFileName == null || assemblyFileName.Length == 0)
		return null;
	  return SearchForAssembly(paths, assemblyFileName);
	}
	public static string GetSilverlightAssemblyPath(string assemblyFileName)
	{
	  if (string.IsNullOrEmpty(assemblyFileName))
		return null;
	  string[] paths;
	  try
	  {
		paths = GetSilverlightAssemblySearchPaths();
	  }
	  catch
	  {
		paths = new string[0];
	  }
	  return SearchForAssembly(paths, assemblyFileName);
	}
	}
#endif
}
