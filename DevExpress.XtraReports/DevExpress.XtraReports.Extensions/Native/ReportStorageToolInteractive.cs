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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Configuration;
namespace DevExpress.XtraReports.Native {
	interface IReportStorageToolInteractive {
		string GetNewUrl();
		string SetNewData(XtraReport report, string defaultUrl);
		string[] GetStandardUrls(ITypeDescriptorContext context);
		bool GetStandardUrlsSupported(ITypeDescriptorContext context);
	}
	static class ReportStorageServiceInteractive {
		static readonly object padlock = new object();
		static IReportStorageToolInteractive tool;
		static IReportStorageToolInteractive Tool {
			get {
				if(tool == null)
					tool = new ReportStorageToolInteractive();
				return tool;
			}
		}
		public static void RegisterTool(IReportStorageToolInteractive tool) {
			lock(padlock) {
				ReportStorageServiceInteractive.tool = tool;
			}
		}
		public static string GetNewUrl() {
			lock(padlock) {
				return Tool.GetNewUrl();
			}
		}
		public static string SetNewData(XtraReport report, string defaultUrl) {
			lock(padlock) {
				return Tool.SetNewData(report, defaultUrl);
			}
		}
		public static string[] GetStandardUrls(ITypeDescriptorContext context) {
			lock(padlock) {
				return Tool.GetStandardUrls(context);
			}
		}
		public static bool GetStandardUrlsSupported(ITypeDescriptorContext context) {
			lock(padlock) {
				return Tool.GetStandardUrlsSupported(context);
			}
		}
	}
	class ReportStorageToolInteractive : ReportStorageToolBase, IReportStorageToolInteractive {
		public ReportStorageToolInteractive()
			: this(Settings.Default.StorageOptions.RootDirectory) {
		}
		public ReportStorageToolInteractive(string rootDirectory)
			: base(rootDirectory) {
		}
		string IReportStorageToolInteractive.GetNewUrl() {
			using(OpenFileDialog fileDialog = FileDialogCreator.CreateOpenDialog(rootDirectory, Settings.Default.StorageOptions.Extension)) {
				return DialogRunner.ShowDialog(fileDialog) == DialogResult.OK ?
					MakeUrlRelative(fileDialog.FileName) :
					string.Empty;
			}
		}
		string IReportStorageToolInteractive.SetNewData(XtraReport report, string defaultUrl) {
			using(SaveFileDialog fileDialog = FileDialogCreator.CreateSaveDialog(MakeUrlAbsolute(defaultUrl), rootDirectory, Settings.Default.StorageOptions.Extension)) {
				fileDialog.Title = string.Format(ReportLocalizer.GetString(ReportStringId.Dlg_SaveFile_Title), report.EffectiveDisplayName);
				fileDialog.FileOk += new CancelEventHandler(fileDialog_FileOk);
				try {
					if(DialogRunner.ShowDialog(fileDialog) == DialogResult.OK) {
						string url = FileHelper.SetValidExtension(fileDialog.FileName, Settings.Default.StorageOptions.Extension, new string[] { Settings.Default.StorageOptions.Extension });
						string relativeUrl = MakeUrlRelative(url);
						ReportStorageService.SetData(report, relativeUrl);
						return relativeUrl;
					}
					return string.Empty;
				} finally {
					fileDialog.FileOk -= new CancelEventHandler(fileDialog_FileOk);
				}
			}
		}
		void fileDialog_FileOk(object sender, CancelEventArgs e) {
			SaveFileDialog d = (SaveFileDialog)sender;
			if(File.Exists(d.FileName)) {
				FileAttributes attr = File.GetAttributes(d.FileName);
				if((attr & FileAttributes.ReadOnly) > 0) {
					e.Cancel = true;
					string caption = !string.IsNullOrEmpty(d.Title) ? d.Title : "Save As";
					MessageBoxWithFocusRestore(Path.GetFileNameWithoutExtension(d.FileName) + "\nThis file is set to read-only.\nTry again with a different file name.",
						caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}
		DialogResult MessageBoxWithFocusRestore(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
			IntPtr focus = FocusHelper.GetFocus();
			try {
				return MessageBox.Show(null, message, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0);
			} finally {
				FocusHelper.SetFocus(new HandleRef(null, focus));
			}
		}
		string[] IReportStorageToolInteractive.GetStandardUrls(ITypeDescriptorContext context) {
			return new string[] { };
		}
		bool IReportStorageToolInteractive.GetStandardUrlsSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
	static class FileDialogCreator {
		public static OpenFileDialog CreateOpenDialog(string defaultDirectory, string defaultExt) {
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = GetDialogFilter(defaultExt);
			fileDialog.InitialDirectory = GetInitialDirectory(string.Empty, defaultDirectory);
			return fileDialog;
		}
		public static SaveFileDialog CreateSaveDialog(string fileName, string defaultDirectory, string defaultExt) {
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Filter = GetDialogFilter(defaultExt);
			fileDialog.InitialDirectory = GetInitialDirectory(fileName, defaultDirectory);
			fileDialog.FileName = fileName;
			return fileDialog;
		}
		static string GetDialogFilter(string defaultExt) {
			return String.Format(ReportLocalizer.GetString(ReportStringId.UD_SaveFileDialog_DialogFilter), defaultExt, defaultExt);
		}
		static string GetInitialDirectory(string fileName, string defaultDirectory) {
			try {
				if(string.IsNullOrEmpty(fileName))
					return defaultDirectory;
				string fileDirectory = Path.GetDirectoryName(fileName);
				return !string.IsNullOrEmpty(fileDirectory) ? fileDirectory : defaultDirectory;
			} catch {
				return defaultDirectory;
			}
		}
	}
}
