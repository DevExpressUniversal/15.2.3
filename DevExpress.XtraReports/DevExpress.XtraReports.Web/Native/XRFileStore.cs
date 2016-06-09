#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;
namespace DevExpress.XtraReports.Web.Native {
	public class XRFileStore {
		const string subdirectoryName = "XtraReports";
		const string fileStoreName = "FileStore";
		const string instanceKey = "XRFileStoreCleaner";
		public const int defaultCheckInterval = 60000;
		public const long defaultKeepInterval = 120000;
		public const string directoryCfgKey = "XtraReportsFileStore-Directory";
		public const string checkIntervalCfgKey = "XtraReportsFileStore-CheckInterval";
		public const string keepIntervalCfgKey = "XtraReportsFileStore-KeepInterval";
		static TimeSpan checkInterval;
		static string directory;
		static string baseTempDirectory;
		internal static bool runCleaner = true;
		readonly string directoryName;
		readonly Timer timer;
#if DEBUGTEST
		internal static void SetDirectory_TEST(string value) {
			directory = value;
		}
#endif
		internal static TimeSpan KeepInterval { get; private set; }
		static XRFileStore() {
			var appSettings = ConfigurationManager.AppSettings;
			checkInterval = ReadInterval(appSettings[checkIntervalCfgKey], defaultCheckInterval);
			KeepInterval = ReadInterval(appSettings[keepIntervalCfgKey], defaultKeepInterval);
			directory = GetTempDirectory(appSettings[directoryCfgKey]);
		}
		public XRFileStore(string directoryName) {
			this.directoryName = directoryName;
			timer = new Timer(_ => ClearFiles(), null, Timeout.Infinite, Timeout.Infinite);
		}
		public static TimeSpan ReadInterval(string value, long defaultMilliseconds) {
			long interval;
			if(string.IsNullOrEmpty(value) || !long.TryParse(value, out interval)) {
				interval = defaultMilliseconds;
			}
			return TimeSpan.FromMilliseconds(interval);
		}
		public static string GetTempDirectory(string value) {
			var tempDirectory = string.Join("/", new[] { "~", "App_Data", subdirectoryName, fileStoreName });
			if(!string.IsNullOrEmpty(value))
				tempDirectory = value;
			return tempDirectory;
		}
		public static string GetTempDirectory() {
			if(string.IsNullOrEmpty(baseTempDirectory))
				baseTempDirectory = GetTempDirectoryCore();
			return baseTempDirectory;
		}
		static string GetTempDirectoryCore() {
			string tempDirectory = (directory.IndexOf('~') == 0) ?
				Path.Combine(HttpRuntime.AppDomainAppPath, directory.Substring(2)) :
				directory;
			Exception exception;
			if(CanUseDirectory(tempDirectory, out exception))
				return tempDirectory;
			tempDirectory = Path.Combine(HttpRuntime.CodegenDir, Path.Combine(subdirectoryName, fileStoreName));
			if(!CanUseDirectory(tempDirectory, out exception))
				throw new UnauthorizedAccessException("There is no writable temporary directory", exception);
			return tempDirectory;
		}
		static bool CanUseDirectory(string directoryName,out Exception exception) {
			exception = null;
			try {
				EnsureDirectory(directoryName);
				string testFilePath = Path.Combine(directoryName, "test");
				File.Create(testFilePath).Close();
				File.Delete(testFilePath);
			} catch(Exception e) {
				exception = e;
				return false;
			}
			return true;
		}
		static void EnsureDirectory(string directoryName) {
			if(!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);
		}
		public static string GetFullName(string fileName) {
			return Path.Combine(GetTempDirectory(), fileName);
		}
		public static void Start(HttpApplicationState application) {
			var cleaner = (XRFileStore)application.Get(instanceKey);
			if(cleaner == null && runCleaner) {
				cleaner = new XRFileStore(GetTempDirectory());
				application.Add(instanceKey, cleaner);
				cleaner.Start();
			}
		}
		void Start() {
			timer.Change(100, (int)checkInterval.TotalMilliseconds);
		}
		void ClearFiles() {
			var now = DateTime.UtcNow;
			try {
				foreach(string file in Directory.EnumerateFiles(directoryName)) {
					try {
						DateTime lastWriteTime = File.GetLastWriteTimeUtc(file);
						if((now - lastWriteTime) > KeepInterval)
							File.Delete(file);
					} catch {
						break;
					}
				}
			} catch {
			}
		}
	}
}
