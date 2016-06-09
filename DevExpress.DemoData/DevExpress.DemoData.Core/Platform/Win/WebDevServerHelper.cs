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
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml;
#if WPF || SL
using System.Windows;
using System.Windows.Threading;
#else
using System.Windows.Forms;
#endif
#if WPF || SL
namespace DevExpress.DemoData.Core {
#else
namespace DemoCenter {
#endif
	public static class WebDevServerHelper {
#if WPF
		public static MessageBoxResult CheckAgreeCloseWebServers() {
			return WebServers.Count < 1 ? MessageBoxResult.No : MessageBox.Show(Application.Current.MainWindow, string.Format("Do you want to stop Web Server demos associated with {0}?", Application.Current.MainWindow.Title),
				  string.Format("Exit {0}", Application.Current.MainWindow.Title), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
		}
#else
		public static bool CheckAgreeCloseWebServers() {
			return WebServers.Count <= 0 || MessageBox.Show(null, "There are Web Server demo applications currently running. Closing Demo Center will result in stopping all associated Web Servers. Are you sure you want to exit?",
				  "Exit DemoCenter", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
#endif
		const string
			iisExpressArgsFormat = "/port:{0} /path:\"{1}\" /clr:v4.0",
			webServerArgsFormat = "/port:{0} /path:\"{1}\" /vpath:\"/{2}/",
			vs5WebDevRelativePath = @"Microsoft.NET\Framework\v2.0.50727\WebDev.WebServer.exe",
			vs8WebDevRelativePath = @"Microsoft Shared\DevServer\9.0\WebDev.WebServer.exe",
			vs10WebDev20RelativePath = @"Microsoft Shared\DevServer\10.0\WebDev.WebServer20.exe",
			vs10WebDev40RelativePath = @"Microsoft Shared\DevServer\10.0\WebDev.WebServer40.exe",
			vs12WebDev20RelativePath = @"Microsoft Shared\DevServer\11.0\WebDev.WebServer20.exe",
			vs12WebDev40RelativePath = @"Microsoft Shared\DevServer\11.0\WebDev.WebServer40.exe",
			webDevWindowText = "ASP.NET Development Server";
		static Dictionary<string, Process> webServers;
		public static Dictionary<string, Process> WebServers {
			get {
				if(webServers == null)
					webServers = new Dictionary<string, Process>();
				return webServers;
			}
		}
		public static string GetProcessedDemoPath(string demoPath, string moduleName, bool forceUseDevelopmentServer) {
			return PrepareDemoPathForProcessingCore(demoPath, GetMVCVirtualPath(moduleName), GetStartFolder(moduleName), forceUseDevelopmentServer);
		}
		public static string PrepareDemoPathForProcessing(string demoPath, string moduleName, bool forceUseDevelopmentServer) {
			return PrepareDemoPathForProcessingCore(demoPath, GetVirtualPath(moduleName), GetStartFolder(moduleName), forceUseDevelopmentServer);
		}
		public static string PrepareDemoPathForProcessingCore(string demoPath, string virtualPath, bool forceUseDevelopmentServer) {
			return PrepareDemoPathForProcessingCore(demoPath, virtualPath, string.Empty, forceUseDevelopmentServer);
		}
		public static string PrepareDemoPathForProcessingCore(string demoPath, string virtualPath, string startFolder, bool forceUseDevelopmentServer) {
			bool useISSExpress = GetRegistryKeyByPath("/Microsoft/IISExpress") != null;
			string physicalDemoPath = Path.GetFullPath(demoPath).Trim();
			if(physicalDemoPath[physicalDemoPath.Length - 1] == Path.DirectorySeparatorChar)
				physicalDemoPath = Path.GetDirectoryName(physicalDemoPath);
			if(!Directory.Exists(physicalDemoPath))
				throw new Exception(string.Format("The demo directory \"{0}\" doesn't exists", physicalDemoPath));
			ushort port = GetPortNumber(physicalDemoPath, virtualPath, useISSExpress);
			WaitForWebServerStart(port);
			if(!string.IsNullOrEmpty(startFolder))
				startFolder = startFolder.Trim('/', '\\') + "/";
			if(useISSExpress)
				return string.Format("http://localhost:{0}/{1}", port, startFolder);
			return string.Format("http://localhost:{0}/{1}/{2}", port, virtualPath, startFolder);
		}
		[SecuritySafeCritical]
		public static void CloseWebServers() {
			List<Process> processes = new List<Process>(WebServers.Values);
			foreach(Process process in processes) {
				if(process != null && !process.HasExited) {
					if(string.Equals(process.ProcessName, "iisexpress", StringComparison.OrdinalIgnoreCase)) {
						CloseIISExpress(process);
						continue;
					}
					IntPtr handle = GetDevServerMainWindowHandle(process);
					if(handle != IntPtr.Zero) {
						PostMessage(handle, 0x11, IntPtr.Zero, IntPtr.Zero);
						PostMessage(handle, 0x12, IntPtr.Zero, IntPtr.Zero);
					}
				}
			}
		}
		static void CloseIISExpress(Process process) {
			try {
				process.Kill();
			}
			catch { }
		}
		[SecuritySafeCritical]
		static IntPtr GetDevServerMainWindowHandle(Process process) {
			IntPtr handle = IntPtr.Zero;
			foreach(ProcessThread thread in process.Threads) {
				EnumThreadWindows(thread.Id, (hWnd, lParam) => {
					string text = GetWindowText(hWnd);
					if(text.StartsWith(webDevWindowText))
						handle = hWnd;
					return handle == IntPtr.Zero;
				}, IntPtr.Zero);
				if(handle != IntPtr.Zero)
					break;
			}
			return handle;
		}
		[SecuritySafeCritical]
		static string GetWindowText(IntPtr hwnd) {
			int length = 256;
			StringBuilder stb = new StringBuilder(length);
			GetWindowText(hwnd, stb, length);
			return stb.ToString();
		}
		delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);
		[DllImport("user32.dll")]
		static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll")]
		private static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder text, Int32 StrLen);
		[DllImport("user32.dll")]
		static extern bool EnumThreadWindows(int threadId, EnumThreadWindowsCallback lpfn, IntPtr lParam);
		static string GetVirtualPath(string moduleName) {
			moduleName = moduleName.Replace("/", "\\");
			int queryStringStartPos = moduleName.LastIndexOf("?");
			if(queryStringStartPos > -1)
				moduleName = moduleName.Substring(0, queryStringStartPos);
			int startPos = moduleName.LastIndexOf("\\") + 1;
			int endPos = moduleName.LastIndexOf(ModuleUrlHelper.UrlExtension);
			return moduleName.Substring(startPos, endPos - startPos);
		}
		static string GetMVCVirtualPath(string moduleName) {
			string baseResult = GetVirtualPath(moduleName);
			return string.IsNullOrEmpty(baseResult) ? string.Empty : baseResult.Replace(".", string.Empty);
		}
		static string GetStartFolder(string moduleName) {
			int pos = moduleName.IndexOf(ModuleUrlHelper.StartFolderPrefix);
			if(pos > -1)
				return moduleName.Substring(pos + ModuleUrlHelper.StartFolderPrefix.Length);
			return string.Empty;
		}
		static ushort GetPortNumber(string physicalDemoPath, string virtualPath, bool useISSExpress) {
			ushort portNumber;
			if(WebServers.ContainsKey(virtualPath)) {
				portNumber = GetPortNumberOutOfArguments(WebServers[virtualPath].StartInfo.Arguments);
				if(WebServers[virtualPath].HasExited)
					WebServers[virtualPath] = StartWebServer(physicalDemoPath, virtualPath, portNumber, useISSExpress);
			} else {
				portNumber = GetIdlePort();
				WebServers[virtualPath] = StartWebServer(physicalDemoPath, virtualPath, portNumber, useISSExpress);
			}
			return portNumber;
		}
		static ushort GetPortNumberOutOfArguments(string arguments) {
			int startIndex = arguments.IndexOf("port:") + 5;
			int length = arguments.IndexOf(" /path:") - startIndex;
			return ushort.Parse(arguments.Substring(startIndex, length).Trim());
		}
		static Process StartWebServer(string physicalDemoPath, string virtualPath, ushort port, bool useISSExpress) {
			Process startedProcess = null;
			if(useISSExpress) {
				string filePath = GetIISApplicationPath();
				if(File.Exists(filePath))
					startedProcess = StartProcess(filePath, string.Format(iisExpressArgsFormat, port, physicalDemoPath));
			} else {
				startedProcess = StartProcess(GetWebDevWebServerPath(physicalDemoPath), string.Format(webServerArgsFormat, port, physicalDemoPath, virtualPath));
			}
			if(startedProcess != null) {
				startedProcess.EnableRaisingEvents = true;
				startedProcess.Exited += OnProcessExited;
			}
			return startedProcess;
		}
		static object lockObject = new object();
		static void OnProcessExited(object sender, EventArgs e) {
			Process process = sender as Process;
			if(process == null) return;
			process.Exited -= OnProcessExited;
			lock(lockObject) {
				foreach(KeyValuePair<string, Process> pair in WebServers) {
					if(pair.Value == process) {
						WebServers.Remove(pair.Key);
						return;
					}
				}
			}
		}
		static Process StartProcess(string filePath, string arguments) {
			ProcessStartInfo info = new ProcessStartInfo(filePath, arguments);
			info.UseShellExecute = true;
			info.WindowStyle = ProcessWindowStyle.Hidden;
			return Process.Start(info);
		}
		static string GetWebDevWebServerPath(string physicalDemoPath) {
			string path;
			bool devServerExists = TryGetWebDevWebServerPath(physicalDemoPath, out path);
			if(!devServerExists)
				throw new Exception("Can't find the Web Development Web Server");
			return path;
		}
		public static bool WebDevWebServerInstalled(){
			string path;
			return TryGetWebDevWebServerPath("", out path);
		}
		static bool TryGetWebDevWebServerPath(string physicalDemoPath, out string path){
			path = GetWebDevWebServer4Path(GetCommonProgramFilesPath(false));
			if(string.IsNullOrEmpty(path))
				path = GetWebDevWebServer4Path(GetCommonProgramFilesPath(true));
			Version targetFrameworkVersion = GetTargetFrameworkVersion(physicalDemoPath);
			if((targetFrameworkVersion != null && targetFrameworkVersion.Major >= 4 || string.IsNullOrEmpty(physicalDemoPath)) && !string.IsNullOrEmpty(path))
				return true;
			path = GetWebDevWebServer2Path(GetCommonProgramFilesPath(false));
			if(string.IsNullOrEmpty(path))
				path = GetWebDevWebServer2Path(GetCommonProgramFilesPath(true));
			return !string.IsNullOrEmpty(path);
		}
		static string GetWebDevWebServer4Path(string commonProgramFiles) {
			string path = Path.Combine(commonProgramFiles, vs10WebDev40RelativePath);
			if(File.Exists(path)) return path;
			path = Path.Combine(commonProgramFiles, vs12WebDev40RelativePath);
			if(File.Exists(path)) return path;
			return null;
		}
		static string GetWebDevWebServer2Path(string commonProgramFiles) {
			string path = Path.Combine(commonProgramFiles, vs10WebDev20RelativePath);
			if(File.Exists(path)) return path;
			path = Path.Combine(commonProgramFiles, vs12WebDev20RelativePath);
			if(File.Exists(path)) return path;
			path = Path.Combine(commonProgramFiles, vs8WebDevRelativePath);
			if(File.Exists(path)) return path;
			path = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), vs5WebDevRelativePath);
			if(File.Exists(path)) return path;
			return null;
		}
		static Version GetTargetFrameworkVersion(string physicalDemoPath) {
			Version resultVersion = null;
			XmlDocument configDocument = new XmlDocument();
			try {
				configDocument.Load(Path.Combine(physicalDemoPath, "web.config"));
			} catch {
				return resultVersion;
			}
			foreach(XmlNode compilationNode in configDocument.GetElementsByTagName("compilation")) {
				XmlAttribute targetFrameworkAttribute = compilationNode.Attributes["targetFramework"];
				if(targetFrameworkAttribute != null) {
					Version version = null;
					try {
						version = new Version(targetFrameworkAttribute.Value);
					} catch {
					}
					if(version != null && (resultVersion == null || version > resultVersion)) {
						resultVersion = version;
					}
				}
			}
			return resultVersion;
		}
		static string GetCommonProgramFilesPath(bool useEnviromentVariable) {
			string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
			if(!useEnviromentVariable && !string.IsNullOrEmpty(path))
				return path;
			path = Environment.GetEnvironmentVariable("CommonProgramFiles(x86)");
			if(string.IsNullOrEmpty(path))
				path = Environment.GetEnvironmentVariable("CommonProgramFiles");
			return path;
		}
		static void WaitForWebServerStart(ushort port) {
			do {
				if(IsPortBusy(port))
					return;
				System.Threading.Thread.Sleep(50);
			} while(true);
		}
		static ushort GetIdlePort() {
			return GetPort(0);
		}
		static ushort GetPortNumber(ushort port) {
			if(!IsPortBusy(port))
				return port;
			return GetIdlePort();
		}
		static bool IsPortBusy(ushort port) {
			bool result = false;
			try {
				GetPort(port);
			} catch {
				result = true;
			}
			return result;
		}
		static ushort GetPort(ushort port) {
			foreach(IPAddress address in Dns.GetHostEntry("localhost").AddressList) {
				TcpListener listener = new TcpListener(address, port);
				listener.Start();
				port = (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
				listener.Stop();
			}
			return port;
		}
		public static bool IISExpressInstalled() {
			return GetIISApplicationPath() != null;
		}
		static string GetIISApplicationPath() {
			RegistryKey key = GetRegistryKeyByPath("/Microsoft/IISExpress");
			if(key != null) {
				string[] subKeys = key.GetSubKeyNames();
				for(int i = subKeys.Length - 1; i >= 0; i--) {
					RegistryKey subKey = key.OpenSubKey(subKeys[i]);
					object installPath = subKey.GetValue("InstallPath");
					if(installPath != null)
						return string.Format("{0}\\iisexpress.exe", installPath);
				}
			}
			return null;
		}
		static RegistryKey GetRegistryKeyByPath(string path) {
			RegistryKey result = null;
			foreach(string softwareKey in new string[] { "Software", "Software/Wow6432Node" }) {
				if(result != null)
					break;
				result = GetRegistrySubKey(Registry.LocalMachine, softwareKey + path);
			}
			return result;
		}
		static RegistryKey GetRegistrySubKey(RegistryKey key, string path) {
			string[] pathParts = path.Replace('\\', '/').Split('/');
			RegistryKey currentKey = key;
			foreach(string pathPart in pathParts) {
				currentKey = currentKey.OpenSubKey(pathPart);
				if(currentKey == null)
					return null;
			}
			return currentKey;
		}
	}
	public static class ModuleUrlHelper {
		public const string UrlExtension = ".ur1";
		public const string StartFolderPrefix = "?StartFolder=";
		public static string CorrectURLByModuleName(string moduleName) {
			moduleName = moduleName.Replace("/", "\\");
			if(!IsUrl(moduleName))
				return moduleName;
			return moduleName.Substring(0, moduleName.IndexOf(UrlExtension));
		}
		public static bool IsUrl(string moduleName) {
			return moduleName.Contains(UrlExtension);
		}
	}
}
