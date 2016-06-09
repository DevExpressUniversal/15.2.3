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
using System.Text;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using DevExpress.XtraReports.Configuration;
namespace DevExpress.XtraReports.Configuration {
	public class Settings {
		static Settings defaultInstance;
		public static Settings Default {
			get {
				if(defaultInstance == null)
					defaultInstance = new Settings();
				return defaultInstance;
			}
			set {
				defaultInstance = value;
			}
		}
		public bool ShowUserFriendlyNamesInUserDesigner {
			get;
			set;
		}
		public StorageOptions StorageOptions { get; private set; }
		public Settings() {
			StorageOptions = new StorageOptions();
			ShowUserFriendlyNamesInUserDesigner = true;
		}
	}
	public class StorageOptions {
		string defaultRootDirectory = string.Empty;
		string defaultExtension = ".repx";
		public string Extension {
			get { return defaultExtension; }
			set { defaultExtension = value; }
		}
		public string RootDirectory {
			get {
				return !string.IsNullOrEmpty(defaultRootDirectory) ?
					defaultRootDirectory :
					Application.StartupPath;
			}
			set {
				defaultRootDirectory = value;
			}
		}
	}
}
namespace DevExpress.XtraReports.Native {
	public static class ReportStorageService {
		static readonly object padlock = new object();
		static IReportStorageTool tool;
		static IReportStorageTool Tool {
			get {
				if(tool == null)
					tool = new ReportStorageTool();
				return tool;
			}
		}
		public static void RegisterTool(IReportStorageTool tool) {
			lock(padlock) {
				ReportStorageService.tool = tool;
			}
		}
		public static byte[] GetData(string url) {
			lock(padlock) {
				return Tool.GetData(url);
			}
		}
		public static void SetData(XtraReport report, Stream stream) {
			lock(padlock) {
				Tool.SetData(report, stream);
			}
		}
		public static void SetData(XtraReport report, string url) {
			lock(padlock) {
				Tool.SetData(report, url);
			}
		}
		public static bool CanSetData(string url) {
			lock(padlock) {
				return Tool.CanSetData(url);
			}
		}
		public static bool IsValidUrl(string url) {
			lock(padlock) {
				return Tool.IsValidUrl(url);
			}
		}
	}
	public interface IReportStorageTool {
		byte[] GetData(string url);
		bool CanSetData(string url);
		void SetData(XtraReport report, string url);
		void SetData(XtraReport report, Stream stream);
		bool IsValidUrl(string url);
	}
	public abstract class ReportStorageToolBase {
		[Obsolete("Use the DevExpress.XtraReports.Configuration.Settings.StorageOptions property instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public static string DefaultExtension {
			get;
			set;
		}
		[Obsolete("Use the DevExpress.XtraReports.Configuration.Settings.StorageOptions property instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public static string DefaultRootDirectory {
			get;
			set;
		}
		const string rootDirectoryChar = "~";
		protected string rootDirectory;
		protected ReportStorageToolBase(string rootDirectory) {
			this.rootDirectory = rootDirectory;
		}
		protected string MakeUrlRelative(string url) {
			return ReplaceLeftSubstring(url, rootDirectory, rootDirectoryChar);
		}
		protected string MakeUrlAbsolute(string url) {
			return ReplaceLeftSubstring(url, rootDirectoryChar, rootDirectory);
		}
		static string ReplaceLeftSubstring(string source, string substring, string newSubstring) {
			if(substring.Length > 0 && source.StartsWith(substring, true, null)) {
				string s = source.Remove(0, substring.Length).TrimStart(Path.DirectorySeparatorChar);
				return string.Join(Path.DirectorySeparatorChar.ToString(), new string[] { newSubstring, s });
			}
			return source;
		}
	}
	public class ReportStorageTool : ReportStorageToolBase, IReportStorageTool {
		public ReportStorageTool()
			: this(Settings.Default.StorageOptions.RootDirectory) {
		}
		public ReportStorageTool(string rootDirectory)
			: base(rootDirectory) {
		}
		byte[] IReportStorageTool.GetData(string url) {
			if(!string.IsNullOrEmpty(url)) {
				using(FileStream stream = new FileStream(MakeUrlAbsolute(url), FileMode.Open, FileAccess.Read)) {
					byte[] buffer = new byte[stream.Length];
					stream.Read(buffer, 0, buffer.Length);
					return buffer;
				}
			}
			return new byte[] {};
		}
		void IReportStorageTool.SetData(XtraReport report, Stream stream) {
			SetDataCore(report, stream);
		}
		protected virtual void SetDataCore(XtraReport report, Stream stream) {
			report.SaveLayout(stream);
		}
		void IReportStorageTool.SetData(XtraReport report, string url) {
			if(!string.IsNullOrEmpty(url)) {
				using(Stream stream = File.Create(MakeUrlAbsolute(url))) {
					ReportStorageService.SetData(report, stream);
				}
			}
		}
		bool IReportStorageTool.CanSetData(string url) {
			string fileName = MakeUrlAbsolute(url);
			return !File.Exists(fileName) || (File.GetAttributes(fileName) & FileAttributes.ReadOnly) == 0;
		}
		bool IReportStorageTool.IsValidUrl(string url) {
			return File.Exists(MakeUrlAbsolute(url));
		}
	}
}
