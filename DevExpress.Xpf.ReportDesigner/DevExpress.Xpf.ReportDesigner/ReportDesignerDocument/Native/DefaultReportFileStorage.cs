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
using System.Linq;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.Configuration;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner {
	public class DefaultReportStorage : IReportStorage {
		public string GetErrorMessage(Exception exception) {
			return ExceptionHelper.GetInnerErrorMessage(exception);
		}
		bool IReportStorage.CanCreateNew() {
			return true;
		}
		public XtraReport CreateNew() {
			return new XtraReport();
		}
		public XtraReport CreateNewSubreport() {
			return new XtraReport();
		}
		bool IReportStorage.CanOpen() {
			return true;
		}
		public string Open(IReportDesignerUI designer) {
			string selectedFile = null;
			designer.DoWithOpenFileDialogService(dialog => {
				dialog.Filter = GetDialogFilter();
				if(!dialog.ShowDialog()) return;
				selectedFile = Path.Combine(dialog.File.DirectoryName, dialog.File.Name);
			});
			return selectedFile;
		}
		public XtraReport Load(string filePath, IReportSerializer designerReportSerializer) {
			using(FileStream stream = OpenFile(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				return designerReportSerializer.Load(stream);
			}
		}
		public string Save(string reportID, IReportProvider reportProvider, bool saveAs, string reportTitle, IReportDesignerUI designer) {
			if(saveAs || reportID == null) {
				string selectedFilePath = null;
				designer.DoWithSaveFileDialogService(dialog => {
					dialog.Filter = GetDialogFilter();
					dialog.Title = string.Format(ReportLocalizer.GetString(ReportStringId.Dlg_SaveFile_Title), reportTitle);
					var dialogResult = dialog.ShowDialog(e => {
						if(!dialog.File.Exists || (dialog.File.Attributes & FileAttributes.ReadOnly) == 0) return;
						e.Cancel = true;
						string caption = !string.IsNullOrEmpty(dialog.Title) ? dialog.Title : "Save As";
						string message = Path.GetFileNameWithoutExtension(dialog.File.Name) + "\nThis file is set to read-only.\nTry again with a different file name.";
						designer.DoWithMessageBoxService(x => x.ShowMessage(message, caption, MessageButton.OK, MessageIcon.Exclamation));
					}, Path.GetDirectoryName(reportID), Path.GetFileName(reportID));
					if(!dialogResult) return;
					selectedFilePath = Path.Combine(dialog.File.DirectoryName, dialog.File.Name);
				});
				reportID = selectedFilePath;
			}
			if(reportID == null) return null;
			using(FileStream stream = OpenFile(reportID, FileMode.Create, FileAccess.Write, FileShare.None)) {
				var data = reportProvider.GetReport();
				designer.ReportSerializer.Save(stream, data);
				return reportID;
			}
		}
		static string GetDialogFilter() {
			return string.Format(ReportLocalizer.GetString(ReportStringId.UD_SaveFileDialog_DialogFilter), Settings.Default.StorageOptions.Extension, Settings.Default.StorageOptions.Extension);
		}
		protected virtual FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share) {
			return new FileStream(path, mode, access, share);
		}
	}
	public class DefaultReportSerializer : IReportSerializer {
		public void Save(Stream stream, XtraReport report) {
			XtraReportSerializer.Save(stream, report);
		}
		public XtraReport Load(Stream stream) {
			return XtraReportSerializer.Load(stream);
		}
	}
}
