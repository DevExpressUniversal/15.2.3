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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Xml;
using DevExpress.EasyTest.Framework;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public static class WebDevWebServerHelper {
		public static void KillWebDevWebServer() {
			try {
				EasyTestTracer.Tracer.InProcedure("KillWebDevWebServer");
				string[] webServerNames = new string[] { "WebDev.WebServer", "WebDev.WebServer20", "WebDev.WebServer40" };
				foreach(string name in webServerNames) {
					Process[] otherWebDevWebServerProcesses = Process.GetProcessesByName(name);
					if(otherWebDevWebServerProcesses.Length > 0) {
						for(int i = 0; i < otherWebDevWebServerProcesses.Length; i++) {
							otherWebDevWebServerProcesses[i].Refresh();
							bool processExited = false;
							if(!otherWebDevWebServerProcesses[i].HasExited) {
								otherWebDevWebServerProcesses[i].Kill();
								processExited = otherWebDevWebServerProcesses[i].WaitForExit(10000);
							}
							else {
								processExited = otherWebDevWebServerProcesses[i].WaitForExit(10000);
							}
							if(!processExited) {
								throw new Exception(string.Format("The '{0}' process has not exit.", otherWebDevWebServerProcesses[i].Id));
							}
						}
						ClearTray();
					}
				}
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("KillWebDevWebServer");
			}
		}
		[DllImport("user32.dll")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll", EntryPoint = "FindWindowEx")]
		public static extern int FindWindowExA(int hWnd1, int hWnd2, string lpsz1, string lpsz2);
		[DllImport("user32.dll")]
		public static extern int PostMessage(int hWnd, int msg, int wParam, int lParam);
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
		[DllImport("user32.dll")]
		public static extern int GetWindowRect(int hWnd, ref RECT rect);
		public static void ClearTray() {
			int toolBarHwnd = 0;
			int tempWnd = 0;
			switch(Environment.OSVersion.Version.Major * 10 + Environment.OSVersion.Version.Minor) {
				case 50:
					tempWnd = FindWindow("Shell_TrayWnd", null);
					tempWnd = FindWindowExA(tempWnd, 0, "TrayNotifyWnd", null);
					toolBarHwnd = FindWindowExA(tempWnd, 0, "ToolbarWindow32", null);
					break;
				case 51:
				case 52:
					tempWnd = FindWindow("Shell_TrayWnd", null);
					tempWnd = FindWindowExA(tempWnd, 0, "TrayNotifyWnd", null);
					tempWnd = FindWindowExA(tempWnd, 0, "SysPager", null);
					toolBarHwnd = FindWindowExA(tempWnd, 0, "ToolbarWindow32", "Notification Area");
					break;
				default:
					break;
			}
			if(toolBarHwnd != 0) {
				RECT toolBarRect = new RECT();
				int WM_MOUSEMOVE = 0x0200;
				GetWindowRect(toolBarHwnd, ref toolBarRect);
				for(int x = toolBarRect.left; x < toolBarRect.right; x += 4) {
					PostMessage(toolBarHwnd, WM_MOUSEMOVE, 0, ((toolBarRect.bottom - toolBarRect.top) / 2) << 16 + x - toolBarRect.left);
				}
			}
		}
		private static string GetCommonProgramFilesPath() {
			string commonProgramFiles = Environment.GetEnvironmentVariable("CommonProgramFiles(x86)");
			if(string.IsNullOrEmpty(commonProgramFiles)) {
				commonProgramFiles = Environment.GetEnvironmentVariable("CommonProgramFiles");
			}
			if(string.IsNullOrEmpty(commonProgramFiles)) {
				commonProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
			}
			return commonProgramFiles;
		}
		private static Version GetTargetFrameworkVersion(string applicationPhysicalPath) {
			Version resultVersion = null;
			XmlDocument configDocument = new XmlDocument();
			try {
				configDocument.Load(Path.Combine(applicationPhysicalPath, "web.config"));
			}
			catch {
				return resultVersion;
			}
			foreach(XmlNode compilationNode in configDocument.GetElementsByTagName("compilation")) {
				XmlAttribute targetFrameworkAttribute = compilationNode.Attributes["targetFramework"];
				if(targetFrameworkAttribute != null) {
					Version version = null;
					try {
						version = new Version(targetFrameworkAttribute.Value);
					}
					catch {
					}
					if(version != null && (resultVersion == null || version > resultVersion)) {
						resultVersion = version;
					}
				}
			}
			return resultVersion;
		}
		public static string GetWebDevWebServerPath(string applicationPhysicalPath) {
			string commonProgramFiles = GetCommonProgramFilesPath();
			string vs12WebDev40RelativePath = @"Microsoft Shared\DevServer\11.0\WebDev.WebServer40.exe";
			string vs12WebDev20RelativePath = @"Microsoft Shared\DevServer\11.0\WebDev.WebServer20.exe";
			string vs10WebDev40RelativePath = @"Microsoft Shared\DevServer\10.0\WebDev.WebServer40.exe";
			string vs10WebDev20RelativePath = @"Microsoft Shared\DevServer\10.0\WebDev.WebServer20.exe";
			string vs9WebDevRelativePath = @"Microsoft Shared\DevServer\9.0\WebDev.WebServer.exe";
			string path = Path.Combine(commonProgramFiles, vs12WebDev40RelativePath);
			Version targetFrameworkVersion = GetTargetFrameworkVersion(applicationPhysicalPath);
			if(targetFrameworkVersion != null && targetFrameworkVersion.Major >= 4) {
				if(File.Exists(path)) {
					return path;
				}
				path = Path.Combine(commonProgramFiles, vs10WebDev40RelativePath);
				if(File.Exists(path)) {
					return path;
				}
			}
			path = Path.Combine(commonProgramFiles, vs12WebDev20RelativePath);
			if(File.Exists(path)) {
				return path;
			}
			path = Path.Combine(commonProgramFiles, vs10WebDev20RelativePath);
			if(File.Exists(path)) {
				return path;
			}
			path = Path.Combine(commonProgramFiles, vs9WebDevRelativePath);
			if(File.Exists(path)) {
				return path;
			}
			string windowsDirectory = Environment.SystemDirectory.Substring(0, Environment.SystemDirectory.ToUpper().IndexOf("SYSTEM"));
			string dotNetVersion = Environment.Version.ToString(3);
			string webDevPath = windowsDirectory + "Microsoft.NET\\Framework\\v" + dotNetVersion + "\\WebDev.WebServer.exe";
			path = windowsDirectory + "Microsoft.NET\\Framework\\v" + dotNetVersion + "\\WebDev.WebServer.exe";
			return path;
		}
		public static bool DevWebServerExist(string physicalPath) {
			if(!string.IsNullOrEmpty(physicalPath)) {
				string webDevPath = GetWebDevWebServerPath(Path.GetFullPath(physicalPath));
				return File.Exists(webDevPath);
			}
			return false;
		}
		public static Process RunWebDevWebServer(string physicalPath, string port) {
			string arguments = "/port:" + port + " /path:\"" + physicalPath + "\"";
			EasyTestTracer.Tracer.InProcedure(string.Format("RunWebDevWebServer({0})", arguments));
			try {
				string webDevPath = GetWebDevWebServerPath(physicalPath);
				if(!File.Exists(webDevPath)) {
					throw new Exception("Cannot run the web dev server, because it is not found.");
				}
				Process webDevWebServerProcess = new Process();
				physicalPath = physicalPath.Trim();
				if(physicalPath[physicalPath.Length - 1] == Path.DirectorySeparatorChar) {
					physicalPath = Path.GetDirectoryName(physicalPath);
				}
				webDevWebServerProcess.StartInfo.FileName = webDevPath;
				webDevWebServerProcess.StartInfo.Arguments = "/port:" + port + " /path:\"" + physicalPath + "\"";
				webDevWebServerProcess.StartInfo.UseShellExecute = true;
				webDevWebServerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				webDevWebServerProcess.Start();
				return webDevWebServerProcess;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure(string.Format("RunWebDevWebServer({0})", arguments));
			}
		}
		public static bool IsWebDevServerStarted(Uri address) {
			EasyTestTracer.Tracer.InProcedure(string.Format("IsWebDevServerStarted({0})", address));
			try {
				using(Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
					IPHostEntry ipHostEntry = Dns.GetHostEntry(address.DnsSafeHost);
					foreach(IPAddress ipAddress in ipHostEntry.AddressList)
						try {
							socket.Connect(ipAddress, address.Port);
							return true;
						}
						catch(Exception) {
						}
				}
				return false;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure(string.Format("IsWebDevServerStarted({0})", address));
			}
		}
	}
}
