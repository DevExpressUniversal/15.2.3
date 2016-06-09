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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Security;
using System.Text.RegularExpressions;
using System.Linq;
using System.Security.Principal;
using DevExpress.Internal;
using Microsoft.Win32;
using System.Data.Common;
using System.Data.EntityClient;
using System.Reflection;
using System.Collections;
using System.Globalization;
#if !DATA
namespace DevExpress.DemoData.Helpers
#else
namespace DevExpress.Internal
#endif
{
	public static class StartApplicationHelper {
		static EscapeHelper urlHelper;
		static StartApplicationHelper() {
			urlHelper = new EscapeHelper();
			UrlHelper.AddPair(":", "/p");
			UrlHelper.AddPair(" ", "_");
			UrlHelper.AddPair("_", "/_");
			UrlHelper.AddPair("\\", "/-");
			UrlHelper.AddPair("+", "/*");
			UrlHelper.AddPair("/", "//");
		}
		public static EscapeHelper UrlHelper { get { return urlHelper; } }
		static string commonDocuments;
		public static string GetDemoFullPath(string demoName) {
#if DEBUGTEST
			if(demoName.Contains(@"\CS\"))
				demoName = demoName.Substring(@"\WinForms\CS\".Length);
			if(demoName.Contains(@"\VB\"))
				demoName = demoName.Substring(@"\WinForms\VB\".Length);
#endif
			return Path.Combine(DemosPath, demoName.TrimStart('/', '\\'));
		}
		public static string CommonDocuments {
			get {
				if(commonDocuments == null) {
					try {
						commonDocuments = WinApiHelper.GetFolderPath(WinApiHelper.SpecialFolderType.CommonDocuments);
					} catch { }
					if(commonDocuments == null)
						commonDocuments = "C:\\Users\\Public\\Documents";
				}
				return commonDocuments;
			}
		}
		public static string DemosPath {
			get {
#if DEBUGTEST
				var executinglocation = Assembly.GetEntryAssembly().Location;
				string demoDir = null;
				if(executinglocation.Contains(@"\CS\"))
					demoDir = @"Demos\CS\";
				if(executinglocation.Contains(@"\VB\"))
					demoDir = @"Demos\VB\";
				if (demoDir != null)
					return executinglocation.Substring(0, executinglocation.LastIndexOf(demoDir) + demoDir.Length);
#endif
				return Path.Combine(CommonDocuments, string.Format("DevExpress Demos {0}\\Components", AssemblyInfo.VersionShort));
			}
		}
		public static string StartVS(string solution, string[] filesToOpen) {
			solution = string.IsNullOrEmpty(solution) ? string.Empty : StartApplicationHelper.GetDemoFullPath(solution);
#if DEBUGTEST
			var fileName = Path.GetFileNameWithoutExtension(solution);
			string ext = null;
			if(solution.Contains(@"\CS\"))
				ext = "_CS.sln";
			if(solution.Contains(@"\VB\"))
				ext = "_VB.sln";
			string demosFileName = fileName.Replace("MainDemo", "Demos") + ext;
			if(File.Exists(Path.Combine(DemosPath + @"..\", demosFileName)))
				solution = Path.Combine(DemosPath + @"..\", demosFileName);
			string samplesFileName = fileName.Replace("MainDemo", "Samples") + ext;
			if(File.Exists(Path.Combine(DemosPath + @"..\", samplesFileName)))
				solution = Path.Combine(DemosPath + @"..\", samplesFileName);
#endif
			if(string.IsNullOrEmpty(solution) || !File.Exists(solution) && !Directory.Exists(solution))
				return string.Format("File {0} is not found.", solution);
			var workingDirectory = Path.GetDirectoryName(solution);
			string arguments = new[] { solution }.Concat(filesToOpen).Select(f => "\"" + f + "\"").Aggregate((l, r) => l + " " + r);
			string devenvPath = GetDevenvPathFromFileAssociation() ?? GetLatestDevenv(solution);
			if(devenvPath == null || !File.Exists(devenvPath)) {
				Process.Start(solution);
				return null;
			}
			DoStart(devenvPath, workingDirectory, arguments);
			return null;
		}
		public static string Start(string fileName, string[] arguments, bool isUrl) {
			string workingDirectory = null;
			if(!isUrl) {
				fileName = string.IsNullOrEmpty(fileName) ? string.Empty : StartApplicationHelper.GetDemoFullPath(fileName);
				if(string.IsNullOrEmpty(fileName) || !File.Exists(fileName) && !Directory.Exists(fileName))
					return string.Format("File {0} is not found.", fileName);
				workingDirectory = Path.GetDirectoryName(fileName);
			}
			string argumentsString = JoinArguments(arguments);
			string error = string.Empty;
			try {
				StartCore(fileName, workingDirectory, argumentsString);
			} catch (Exception e) {
				error = e.Message;
			}
			return error;
		}
		static void StartCore(string fileName, string workingDirectory, string argumentsString) {
			if(!fileName.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)) {
				DoStart(fileName, workingDirectory, argumentsString);
				return;
			}
			string devenvPath = GetDevenvPathFromFileAssociation() ?? GetLatestDevenv(fileName);
			if(devenvPath == null) {
				DoStart("\"" + fileName + "\"", workingDirectory, "");
				return;
			}
			DoStart(devenvPath, workingDirectory, fileName);
		}
		static string GetDevenvPathFromFileAssociation() {
			try {
				string slnApp = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.sln\\UserChoice", "ProgId", "") as string;
				if(string.IsNullOrEmpty(slnApp)) {
					RegistryKey openWithProgids = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.sln\\OpenWithProgids");
					if(openWithProgids == null) return null;
					if(openWithProgids.ValueCount != 1) return null;
					slnApp = openWithProgids.GetValueNames()[0] as string;
				}
				string vsPath = Registry.GetValue("HKEY_CLASSES_ROOT\\" + slnApp + "\\shell\\Open\\command", null, "") as string;
				const string vsVersionSelector = "VSLauncher.exe";
				if(vsPath.IndexOf(vsVersionSelector, 0) != (-1)) return null;
				if(string.IsNullOrEmpty(vsPath)) return null;
				return vsPath.Replace("\"%1\"", "").Replace("\"", "");
			} catch {
				return null;
			}
		}
		static string GetLatestDevenv(string fileName) {
			var knownVersions = new[] { "14.0", "12.0", "11.0", "10.0" };
			var paths = knownVersions.Select(p => GetDevenvPath(p));
			return paths.FirstOrDefault(p => p != null);
		}
		static string GetSolutionVersion(string solutionPath) {
			string text = File.ReadAllText(solutionPath);
			Match match = Regex.Match(text, @"Microsoft Visual Studio Solution File, Format Version (\d\d\.\d)");
			return match.Success ? match.Groups[1].Value : null;
		}
		static string GetDevenvPath(string version) {
			try {
				string path = RegistryViewer.Current.GetSzValue(RegistryHive.LocalMachine, string.Format("SOFTWARE/Microsoft/VisualStudio/{0}/Setup/VS", version), "EnvironmentPath");
				if(path == null)
					return null;
				return path.Trim('\0');
			} catch {
				return null;
			}
		}
		static void DoStart(string fileName, string workingDirectory, string argumentsString) {
			ProcessStartInfo psi = new ProcessStartInfo(fileName);
			psi.WorkingDirectory = workingDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
			psi.Arguments = argumentsString;
			WinApiHelper.WaitForWindowAppears(Process.Start(psi));
		}
		static string JoinArguments(string[] arguments) {
			if(arguments == null) return string.Empty;
			StringBuilder s = new StringBuilder();
			foreach(string argument in arguments) {
				if(string.IsNullOrEmpty(argument)) continue;
				if(s.Length != 0)
					s.Append(' ');
				s.Append(argument);
			}
			return s.ToString();
		}
		class WinApiHelper {
			[SecuritySafeCritical]
			public static void WaitForWindowAppears(Process process) {
				if(process == null) return;
				IntPtr[] handles = new IntPtr[] { process.Handle };
				try {
					while(true) {
						process.Refresh();
						Import.MsgWaitForMultipleObjects((uint)handles.Length, handles, true, 250, 0xff);
						if(!process.MainWindowHandle.Equals(IntPtr.Zero)) {
							Import.SwitchToThisWindow(process.MainWindowHandle, true);
							return;
						}
					}
				} catch (Exception) { }
			}
			public enum SpecialFolderType {
				CommonDocuments = Import.SpecialFolderType.CommonDocuments
			}
			[SecuritySafeCritical]
			public static string GetFolderPath(SpecialFolderType folderType) {
				StringBuilder path = new StringBuilder(Import.MAX_PATH);
				int result = Import.SHGetFolderPath(IntPtr.Zero, (int)folderType, IntPtr.Zero, 0, path);
				if(result != 0)
					throw new Win32Exception(result);
				return path.ToString();
			}
			class Import {
				[DllImport("user32.dll")]
				public static extern uint MsgWaitForMultipleObjects(uint nCount, IntPtr[] pHandles, bool bWaitAll, uint dwMilliseconds, uint dwWakeMask);
				[DllImport("user32.dll", SetLastError = true)]
				public static extern void SwitchToThisWindow(IntPtr hwind, bool fAltTab);
				public const int MAX_PATH = 260;
				[DllImport("shell32.dll")]
				public static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, uint dwFlags, [Out] StringBuilder pszPath);
				public enum SpecialFolderType {
					AdministrativeTools = 0x0030,
					CommonAdministrativeTools = 0x002f,
					ApplicationData = 0x001a,
					CommonAppData = 0x0023,
					CommonDocuments = 0x002e,
					Cookies = 0x0021,
					CreateFlag = 0x8000,
					History = 0x0022,
					InternetCache = 0x0020,
					LocalApplicationData = 0x001c,
					MyPictures = 0x0027,
					Personal = 0x0005,
					ProgramFiles = 0x0026,
					CommonProgramFiles = 0x002b,
					System = 0x0025,
					Windows = 0x0024,
					Fonts = 0x0014
				}
			}
		}
	}
	public class EscapeHelper {
		List<Dictionary<string, string>> replaces = new List<Dictionary<string, string>>();
		List<Dictionary<string, string>> inverts = new List<Dictionary<string, string>>();
		public void AddPair(string original, string escaped) {
			AddReplace(original, escaped, replaces);
			AddReplace(escaped, original, inverts);
		}
		public string Screen(string s) {
			return Replace(s, replaces);
		}
		public string Unscreen(string s) {
			return Replace(s, inverts);
		}
		static void AddReplace(string from, string to, List<Dictionary<string, string>> replaces) {
			Dictionary<string, string> replace = GetDictionary(replaces, from.Length - 1);
			replace.Add(from, to);
		}
		static string Replace(string s, List<Dictionary<string, string>> replaces) {
			StringBuilder result = new StringBuilder();
			for(int i = 0; i < s.Length; ++i) {
				int originalLength_1 = -1;
				string escaped = null;
				for(int length_1 = replaces.Count - 1; length_1 >= 0; --length_1) {
					if(i + length_1 + 1 > s.Length) continue;
					Dictionary<string, string> dictionary = GetDictionary(replaces, length_1);
					string t = s.Substring(i, length_1 + 1);
					string r = null;
					if(dictionary.TryGetValue(t, out r)) {
						originalLength_1 = length_1;
						escaped = r;
						break;
					}
				}
				if(originalLength_1 >= 0) {
					result.Append(escaped);
					i += originalLength_1;
				} else {
					result.Append(s[i]);
				}
			}
			return result.ToString();
		}
		static Dictionary<string, string> GetDictionary(List<Dictionary<string, string>> list, int i) {
			lock (list) {
				int listCount = list.Count;
				if(i < listCount) return list[i];
				for(int k = 0; k < i - listCount + 1; ++k) {
					list.Add(new Dictionary<string, string>());
				}
				return list[i];
			}
		}
	}
	public abstract class DisposableBase : IDisposable {
		bool disposed = false;
		~DisposableBase() { Dispose(false); }
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public bool Disposed { get { return disposed; } }
		protected virtual void DisposeManaged() { }
		protected virtual void DisposeUnmanaged() { }
		void Dispose(bool disposing) {
			if(Disposed) return;
			disposed = true;
			if(disposing)
				DisposeManaged();
			DisposeUnmanaged();
		}
	}
	public abstract class RegistryViewerBase {
		public abstract bool IsKeyExists(RegistryHive hive, string key);
		public abstract string[] GetMultiSzValue(RegistryHive hive, string key, string name);
		public abstract string GetSzValue(RegistryHive hive, string key, string name);
	}
	public static class WinApiRegistryHelper {
		[Flags]
		public enum ResigtryAccess {
			QueryValue = Import.KEY_QUERY_VALUE,
			SetValue = Import.KEY_SET_VALUE,
			CreateSubKey = Import.KEY_CREATE_SUB_KEY,
			EnumerateSubKeys = Import.KEY_ENUMERATE_SUB_KEYS,
			Notify = Import.KEY_NOTIFY,
			CreateLink = Import.KEY_CREATE_LINK,
			Read = Import.KEY_READ,
			WOW64_32Key = Import.KEY_WOW64_32KEY,
			WOW64_64Key = Import.KEY_WOW64_64KEY,
			WOW64_Res = Import.KEY_WOW64_RES
		}
		[SecuritySafeCritical]
		public static IntPtr OpenRegistryKey(RegistryHive hkey, string subkey, ResigtryAccess access) {
			IntPtr handle;
			if(Import.RegOpenKeyEx(HkeyToPtr(hkey), subkey, 0, (int)access, out handle) != 0)
				handle = IntPtr.Zero;
			return handle;
		}
		[SecuritySafeCritical]
		public static void CloseRegistryKey(IntPtr key) {
			Import.RegCloseKey(key);
		}
		static IntPtr HkeyToPtr(RegistryHive hkey) {
			return hkey == RegistryHive.CurrentUser ? Import.HKEY_CURRENT_USER : Import.HKEY_LOCAL_MACHINE;
		}
		[SecuritySafeCritical]
		public static string[] ReadRegistryKeyMultiSzValue(IntPtr key, string name) {
			Import.RType type = Import.RType.RegMultiSz;
			uint size = 0;
			if(Import.RegQueryValueEx(key, name, 0, ref type, null, ref size) != 0) return null;
			byte[] d = new byte[(int)size];
			if(Import.RegQueryValueEx(key, name, 0, ref type, d, ref size) != 0) return null;
			List<string> strings = new List<string>();
			string s = Encoding.Unicode.GetString(d, 0, (int)size);
			int start;
			int end = -1;
			while(true) {
				start = end + 1;
				end = s.IndexOf('\0', start);
				if(end <= start) break;
				strings.Add(s.Substring(start, end - start));
			}
			return strings.ToArray();
		}
		[SecuritySafeCritical]
		public static string ReadRegistryKeySzValue(IntPtr key, string name) {
			Import.RType type = Import.RType.RegSz;
			uint size = 0;
			if(Import.RegQueryValueEx(key, name, 0, ref type, null, ref size) != 0) return null;
			byte[] d = new byte[(int)size];
			if(Import.RegQueryValueEx(key, name, 0, ref type, d, ref size) != 0) return null;
			return Encoding.Unicode.GetString(d, 0, (int)size);
		}
		static class Import {
			[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
			public static extern uint RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, ref RType lpType, byte[] pvData, ref uint pcbData);
			public enum RFlags {
				Any = 65535,
				RegNone = 1,
				Noexpand = 268435456,
				RegBinary = 8,
				Dword = 24,
				RegDword = 16,
				Qword = 72,
				RegQword = 64,
				RegSz = 2,
				RegMultiSz = 32,
				RegExpandSz = 4,
				RrfZeroonfailure = 536870912
			}
			public enum RType {
				RegNone = 0,
				RegSz = 1,
				RegExpandSz = 2,
				RegMultiSz = 7,
				RegBinary = 3,
				RegDword = 4,
				RegQword = 11,
				RegQwordLittleEndian = 11,
				RegDwordLittleEndian = 4,
				RegDwordBigEndian = 5,
				RegLink = 6,
				RegResourceList = 8,
				RegFullResourceDescriptor = 9,
				RegResourceRequirementsList = 10
			}
			[DllImport("advapi32.dll", SetLastError = true)]
			public static extern int RegCloseKey(IntPtr hKey);
			[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
			public static extern int RegOpenKeyEx(IntPtr hKey, string subKey, int ulOptions, int samDesired, out IntPtr hkResult);
			public static IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);
			public static IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
			public const int KEY_QUERY_VALUE = 0x1;
			public const int KEY_SET_VALUE = 0x2;
			public const int KEY_CREATE_SUB_KEY = 0x4;
			public const int KEY_ENUMERATE_SUB_KEYS = 0x8;
			public const int KEY_NOTIFY = 0x10;
			public const int KEY_CREATE_LINK = 0x20;
			public const int KEY_WOW64_32KEY = 0x200;
			public const int KEY_WOW64_64KEY = 0x100;
			public const int KEY_WOW64_RES = 0x300;
			public const int KEY_READ = 0x20019;
		}
	}
	public class WinApiRegistryKey : DisposableBase {
		IntPtr handle;
		public WinApiRegistryKey(RegistryHive hive, string key, WinApiRegistryHelper.ResigtryAccess access) {
			handle = WinApiRegistryHelper.OpenRegistryKey(hive, key, access);
		}
		protected override void DisposeUnmanaged() {
			if(handle != IntPtr.Zero)
				WinApiRegistryHelper.CloseRegistryKey(handle);
			handle = IntPtr.Zero;
			base.DisposeUnmanaged();
		}
		public bool Exists { get { return handle != IntPtr.Zero; } }
		public string[] GetMultiSzValue(string name) {
			if(!Exists) return null;
			return WinApiRegistryHelper.ReadRegistryKeyMultiSzValue(handle, name);
		}
		public string GetSzValue(string name) {
			if(!Exists) return null;
			return WinApiRegistryHelper.ReadRegistryKeySzValue(handle, name);
		}
	}
	public class WinApiRegistryMultiKey : DisposableBase {
		List<WinApiRegistryKey> keys = new List<WinApiRegistryKey>();
		public WinApiRegistryMultiKey(RegistryHive hive, string key, WinApiRegistryHelper.ResigtryAccess access) {
			WinApiRegistryKey wkey = new WinApiRegistryKey(hive, key, access | WinApiRegistryHelper.ResigtryAccess.WOW64_32Key);
			if(wkey.Exists)
				keys.Add(wkey);
			wkey = new WinApiRegistryKey(hive, key, access | WinApiRegistryHelper.ResigtryAccess.WOW64_64Key);
			if(wkey.Exists)
				keys.Add(wkey);
		}
		protected override void DisposeManaged() {
			foreach(WinApiRegistryKey wkey in keys)
				wkey.Dispose();
			keys.Clear();
			base.DisposeManaged();
		}
		public bool Exists { get { return keys.Count > 0; } }
		public string[] GetMultiSzValue(string name) {
			var values = new List<string>();
			foreach(WinApiRegistryKey wkey in keys) {
				string[] s = wkey.GetMultiSzValue(name);
				if(s != null) {
					values.AddRange(s);
				}
			}
			return values.Count > 0 ? values.ToArray() : null;
		}
		public string GetSzValue(string name) {
			foreach(WinApiRegistryKey wkey in keys) {
				string s = wkey.GetSzValue(name);
				if(s != null) return s;
			}
			return null;
		}
	}
	public class RegistryViewer : RegistryViewerBase {
		public static RegistryViewer Current = new RegistryViewer();
		public override bool IsKeyExists(RegistryHive hive, string key) {
			using (WinApiRegistryMultiKey wkey = GetWKey(hive, key)) {
				return wkey.Exists;
			}
		}
		public override string[] GetMultiSzValue(RegistryHive hive, string key, string name) {
			using (WinApiRegistryMultiKey wkey = GetWKey(hive, key)) {
				return wkey.GetMultiSzValue(name);
			}
		}
		public override string GetSzValue(RegistryHive hive, string key, string name) {
			using (WinApiRegistryMultiKey wkey = GetWKey(hive, key)) {
				return wkey.GetSzValue(name);
			}
		}
		WinApiRegistryMultiKey GetWKey(RegistryHive hive, string key) {
			key = key.Replace('/', '\\');
			return new WinApiRegistryMultiKey(hive, key, WinApiRegistryHelper.ResigtryAccess.Read);
		}
	}
	class SqlServerDetector : IDisposable {
		internal class RegistryKeyProxy : IDisposable {
			private readonly RegistryKey _key;
			protected RegistryKeyProxy() {
			}
			public RegistryKeyProxy(RegistryKey key) {
				_key = key;
			}
			public static implicit operator RegistryKeyProxy(RegistryKey key) {
				return new RegistryKeyProxy(key);
			}
			public virtual int SubKeyCount {
				get { return _key == null ? 0 : _key.SubKeyCount; }
			}
			public virtual string[] GetSubKeyNames() {
				return _key == null ? new string[0] : _key.GetSubKeyNames();
			}
			public virtual RegistryKeyProxy OpenSubKey(string name) {
				return new RegistryKeyProxy(_key == null ? null : _key.OpenSubKey(name));
			}
			public virtual void Dispose() {
				if(_key != null) {
					_key.Dispose();
				}
			}
		}
		private readonly RegistryKeyProxy _localMachine;
		internal SqlServerDetector(RegistryKeyProxy localMachine) {
			_localMachine = localMachine;
		}
		public virtual string GetLocalDBVersionInstalled() {
			var key = OpenLocalDBInstalledVersions(useWow6432Node: false);
			if(key.SubKeyCount == 0) {
				key = OpenLocalDBInstalledVersions(useWow6432Node: true);
			}
			var orderableVersions = new List<Tuple<decimal, string>>();
			foreach(var subKey in key.GetSubKeyNames()) {
				decimal decimalVersion;
				if(Decimal.TryParse(subKey, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimalVersion)) {
					orderableVersions.Add(Tuple.Create(decimalVersion, subKey));
				}
			}
			var highestVersion = orderableVersions.OrderByDescending(v => v.Item1).FirstOrDefault();
			if(highestVersion == null || highestVersion.Item2 == null)
				return null;
			if(highestVersion.Item1 >= 12.0m) {
				return "mssqllocaldb";
			}
			return "v" + highestVersion.Item2;
		}
		private RegistryKeyProxy OpenLocalDBInstalledVersions(bool useWow6432Node) {
			var key = _localMachine.OpenSubKey("SOFTWARE");
			if(useWow6432Node) {
				key = key.OpenSubKey("Wow6432Node");
			}
			return key
				.OpenSubKey("Microsoft")
				.OpenSubKey("Microsoft SQL Server Local DB")
				.OpenSubKey("Installed Versions");
		}
		public void Dispose() {
			_localMachine.Dispose();
		}
	}
	public static class DbEngineDetector {
		static readonly string
			currentUserName,
			networkServiceLocalizedName;
		static SqlServerDetector detector;
		static SqlServerDetector Detector {
			get {
				if(detector == null) {
					detector = new SqlServerDetector(Registry.LocalMachine);
				}
				return detector;
			}
		}
		static DbEngineDetector() {
			currentUserName = WindowsIdentity.GetCurrent().Name;
			networkServiceLocalizedName = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null)
				.Translate(typeof(NTAccount))
				.Value;
		}
		public static string LocalDbVersion {
			get { return Detector.GetLocalDBVersionInstalled(); }
		}
		public static bool IsSqlExpressInstalled {
			get {
				string[] instances = RegistryViewer.Current.GetMultiSzValue(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Microsoft SQL Server", "InstalledInstances");
				return instances != null && instances.Contains("SQLEXPRESS");
			}
		}
		public static bool IsLocalDbInstalled {
			get { return !string.IsNullOrEmpty(LocalDbVersion); }
		}
		public static string[] GetSclCEInstalledVersions() {
			List<string> result = new List<string>();
			string keybase = "SOFTWARE\\Microsoft\\Microsoft SQL Server Compact Edition\\";
			if(RegistryViewer.Current.IsKeyExists(RegistryHive.LocalMachine, keybase + "v3.5"))
				result.Add("v3.5");
			if(RegistryViewer.Current.IsKeyExists(RegistryHive.LocalMachine, keybase + "v4.0"))
				result.Add("v4.0");
			return result.ToArray();
		}
		static string PatchInnerString(string connectionString, bool entitySyntax, PatchSimpleString patchFunction) {
			if(entitySyntax) {
				var entityBuilder = new EntityConnectionStringBuilder { ConnectionString = connectionString };
				entityBuilder.ProviderConnectionString = patchFunction.Patch(entityBuilder.ProviderConnectionString);
				return entityBuilder.ConnectionString;
			}
			return patchFunction.Patch(connectionString);
		}
		class PatchSimpleString {
			public string Patch(string connectionString) {
				var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
				string instanceName = GetSqlServerInstanceName();
				builder["Data Source"] = instanceName;
				if(!instanceName.ToUpper().Contains(".\\SQLEXPRESS")) {
					builder.Remove("User Instance");
				}
				return builder.ConnectionString;
			}
		}
		public static string PatchConnectionString(string rawConnectionString, bool entitySyntax = false) {
			if(rawConnectionString.ToLower().Contains("sqlite"))
				return rawConnectionString;
			PatchSimpleString patchSimple = new PatchSimpleString();
			return PatchInnerString(rawConnectionString, entitySyntax, patchSimple);
		}
		public static string SelectConnectionStringName(string sqlexpressName, string localdbName) {
			string dbversion = LocalDbVersion;
			if(IsSqlExpressInstalled) {
				return sqlexpressName;
			}
			if(dbversion != null) {
				return localdbName;
			}
			return sqlexpressName;
		}
		public static string GetSqlServerInstanceName() {
			string localdbVersion = LocalDbVersion;
			if(localdbVersion != null && !IsExecutingUnderService)
				return "(localdb)\\" + localdbVersion;
			if(IsSqlExpressInstalled)
				return ".\\SQLEXPRESS";
			return "(local)";
		}
		static bool IsExecutingUnderService {
			get {
				return currentUserName.StartsWith("IIS APPPOOL\\", StringComparison.InvariantCultureIgnoreCase)
					|| string.Equals(currentUserName, networkServiceLocalizedName, StringComparison.InvariantCultureIgnoreCase);
			}
		}
#if DATA
		public static bool PatchConnectionStringsAndConfigureEntityFrameworkDefaultConnectionFactory() {
			PatchAppConfigConnectionStrings();
			return ConfigureEntityFrameworkDefaultConnectionFactory();
		}
		#region ConfigureEntityFrameworkDefaultConnectionFactory
		public static bool ConfigureEntityFrameworkDefaultConnectionFactory() {
			string localDbVersion = LocalDbVersion;
			if(localDbVersion != null)
				return ConfigureDefaultConnectionFactory("LocalDbConnectionFactory", localDbVersion);
			return ConfigureDefaultConnectionFactory("SqlConnectionFactory");
		}
		static bool ConfigureDefaultConnectionFactory(string connectionFactoryTypeName, params object[] connectionFactoryArguments) {
			object dbConfiguration = CreateDbConfiguration();
			if(dbConfiguration == null) return false;
			Type connectionFactoryType = dbConfiguration.GetType().Assembly.GetType("System.Data.Entity.Infrastructure." + connectionFactoryTypeName);
			object connectionFactory = Activator.CreateInstance(connectionFactoryType, connectionFactoryArguments);
			var setDefaultConnectionFactory = dbConfiguration.GetType().GetMethod("SetDefaultConnectionFactory", BindingFlags.NonPublic | BindingFlags.Instance);
			setDefaultConnectionFactory.Invoke(dbConfiguration, new object[] { connectionFactory });
			var setConfigurationMethod = dbConfiguration.GetType().GetMethod("SetConfiguration", BindingFlags.Static | BindingFlags.Public);
			setConfigurationMethod.Invoke(null, new object[] { dbConfiguration });
			return true;
		}
		static object CreateDbConfiguration() {
			Assembly entityFrameworkAssembly = GetEntityFrameworkAssembly();
			if(entityFrameworkAssembly == null) return null;
			Type dbConfigurationType = entityFrameworkAssembly.GetType("System.Data.Entity.DbConfiguration");
			var ctor = dbConfigurationType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
			return ctor.Invoke(new object[] { });
		}
		static Assembly GetEntityFrameworkAssembly() { return GetLoadedAssembly("EntityFramework"); }
		static Assembly GetLoadedAssembly(string asmName) {
			foreach(var asm in GetLoadedAssemblies()) {
				if(PartialNameEquals(asm.FullName, asmName))
					return asm;
			}
			return null;
		}
		static IEnumerable<Assembly> GetLoadedAssemblies() {
			return AppDomain.CurrentDomain.GetAssemblies();
		}
		static bool PartialNameEquals(string asmName0, string asmName1) {
			return string.Equals(GetPartialName(asmName0), GetPartialName(asmName1), StringComparison.InvariantCultureIgnoreCase);
		}
		static string GetPartialName(string asmName) {
			int nameEnd = asmName.IndexOf(',');
			return nameEnd < 0 ? asmName : asmName.Remove(nameEnd);
		}
		#endregion
		#region PatchAppConfigConnectionStrings
		public static void PatchAppConfigConnectionStrings() {
			Type initStateType = typeof(System.Configuration.ConfigurationManager).GetNestedType("InitState", BindingFlags.NonPublic);
			object initStateNotStarted = initStateType.GetEnumValues().GetValue(0);
			FieldInfo initStateField = typeof(System.Configuration.ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static);
			initStateField.SetValue(null, initStateNotStarted);
			FieldInfo configSystemField = typeof(System.Configuration.ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
			Type clientConfigSystemType = typeof(System.Configuration.ConfigurationManager).Assembly.GetType("System.Configuration.ClientConfigurationSystem");
			configSystemField.SetValue(null, null);
			Type configSettingsFactoryType = typeof(System.Configuration.ConfigurationManager).Assembly.GetType("System.Configuration.Internal.InternalConfigSettingsFactory");
			var configSettingsFactory = (System.Configuration.Internal.IInternalConfigSettingsFactory)Activator.CreateInstance(configSettingsFactoryType, true);
			var internalConfigSystem = (System.Configuration.Internal.IInternalConfigSystem)Activator.CreateInstance(clientConfigSystemType, true);
			configSettingsFactory.SetConfigurationSystem(new ConnectionStringPatcherConfigSystem(internalConfigSystem), false);
		}
		class ConnectionStringPatcherConfigSystem : System.Configuration.Internal.IInternalConfigSystem {
			System.Configuration.Internal.IInternalConfigSystem internalConfigSystem;
			System.Configuration.ConnectionStringsSection connectionStringsSection;
			const string ConnectionStringsSectionName = "connectionStrings";
			const string EntityFrameworkConnectionStringProviderName = "System.Data.EntityClient";
			public ConnectionStringPatcherConfigSystem(System.Configuration.Internal.IInternalConfigSystem internalConfigSystem) {
				this.internalConfigSystem = internalConfigSystem;
			}
			System.Configuration.ConnectionStringsSection CreateConnectionStringsSection() {
				var internalConnectionStringsSection = (System.Configuration.ConnectionStringsSection)internalConfigSystem.GetSection(ConnectionStringsSectionName);
				var connectionStringsSection = new System.Configuration.ConnectionStringsSection();
				foreach(var connectionStringSettings in internalConnectionStringsSection.ConnectionStrings.Cast<System.Configuration.ConnectionStringSettings>()) {
					var patchedConnectionStringSettings = new System.Configuration.ConnectionStringSettings(
						connectionStringSettings.Name,
						DbEngineDetector.PatchConnectionString(connectionStringSettings.ConnectionString, connectionStringSettings.ProviderName == EntityFrameworkConnectionStringProviderName),
						connectionStringSettings.ProviderName
					);
					connectionStringsSection.ConnectionStrings.Add(patchedConnectionStringSettings);
				}
				return connectionStringsSection;
			}
			System.Configuration.ConnectionStringsSection ConnectionStringSection {
				get {
					if(connectionStringsSection == null)
						connectionStringsSection = CreateConnectionStringsSection();
					return connectionStringsSection;
				}
			}
			object System.Configuration.Internal.IInternalConfigSystem.GetSection(string configKey) {
				return configKey == ConnectionStringsSectionName ? ConnectionStringSection : internalConfigSystem.GetSection(configKey);
			}
			void System.Configuration.Internal.IInternalConfigSystem.RefreshConfig(string sectionName) {
				if(sectionName == ConnectionStringsSectionName)
					this.connectionStringsSection = null;
				internalConfigSystem.RefreshConfig(sectionName);
			}
			bool System.Configuration.Internal.IInternalConfigSystem.SupportsUserConfig { get { return internalConfigSystem.SupportsUserConfig; } }
		}
		#endregion
#endif
	}
}
