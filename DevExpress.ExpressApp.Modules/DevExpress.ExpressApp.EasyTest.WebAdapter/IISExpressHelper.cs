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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DevExpress.EasyTest.Framework;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public class IISExpressHelper {
		public static Process RunIISExpressServer(string physicalPath, string port) {
			string arguments = "/port:" + port + " /path:\"" + physicalPath + "\"";
			EasyTestTracer.Tracer.InProcedure(string.Format("RunWebDevWebServer({0})", arguments));
			try {
				string iisExpressPath = GetIISExpressServerPath(physicalPath);
				if(!File.Exists(iisExpressPath)) {
					throw new Exception("Can not run the IIS express server, because it is not found.");
				}
				Process IISExpressServerProcess = new Process();
				physicalPath = physicalPath.Trim();
				if(physicalPath[physicalPath.Length - 1] == Path.DirectorySeparatorChar) {
					physicalPath = Path.GetDirectoryName(physicalPath);
				}
				IISExpressServerProcess.StartInfo.FileName = iisExpressPath;
				IISExpressServerProcess.StartInfo.Arguments = "/port:" + port + " /path:\"" + physicalPath + "\"";
				IISExpressServerProcess.StartInfo.UseShellExecute = true;
				IISExpressServerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				IISExpressServerProcess.Start();
				return IISExpressServerProcess;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure(string.Format("RunWebDevWebServer({0})", arguments));
			}
		}
		public static void KillIISExpressServer() {
			try {
				EasyTestTracer.Tracer.InProcedure("KillIISExpressServer");
				string[] iisExpressServerNames = new string[] { "iisexpress" };
				foreach(string name in iisExpressServerNames) {
					Process[] otherIISExpressServerProcesses = Process.GetProcessesByName(name);
					if(otherIISExpressServerProcesses.Length > 0) {
						for(int i = 0; i < otherIISExpressServerProcesses.Length; i++) {
							otherIISExpressServerProcesses[i].Refresh();
							bool processExited = false;
							if(!otherIISExpressServerProcesses[i].HasExited) {
								otherIISExpressServerProcesses[i].Kill();
								processExited = otherIISExpressServerProcesses[i].WaitForExit(10000);
							}
							else {
								processExited = otherIISExpressServerProcesses[i].WaitForExit(10000);
							}
							if(!processExited) {
								throw new Exception(string.Format("The '{0}' process has not exit.", otherIISExpressServerProcesses[i].Id));
							}
						}
						WebDevWebServerHelper.ClearTray();
					}
				}
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("KillIISExpressServer");
			}
		}
		public static bool IsIISExpressServerStarted(Uri address) {
			EasyTestTracer.Tracer.InProcedure(string.Format("IsIISExpressServerStarted({0})", address));
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
				EasyTestTracer.Tracer.OutProcedure(string.Format("IsIISExpressServerStarted({0})", address));
			}
		}
		public static string GetIISExpressServerPath(string applicationPhysicalPath) {
			string commonProgramFiles = GetProgramFilesPath();
			string vs12WebDev40RelativePath = @"IIS Express\iisexpress.exe";
			string path = Path.Combine(commonProgramFiles, vs12WebDev40RelativePath);
			return path;
		}
		private static string GetProgramFilesPath() {
			string commonProgramFiles = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
			if(string.IsNullOrEmpty(commonProgramFiles)) {
				commonProgramFiles = Environment.GetEnvironmentVariable("ProgramFiles");
			}
			if(string.IsNullOrEmpty(commonProgramFiles)) {
				commonProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			}
			return commonProgramFiles;
		}
	}
}
