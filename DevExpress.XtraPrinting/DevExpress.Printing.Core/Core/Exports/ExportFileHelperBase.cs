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
using DevExpress.XtraPrinting;
using System.IO;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Export;
namespace DevExpress.XtraPrinting.Export {
	public abstract class ExportFileHelperBase {
		protected static readonly string[] EmptyStrings = new String[0];
		protected PrintingSystemBase ps;
#if !SILVERLIGHT
		EmailSenderBase emailSender;
#endif
		protected static string ValidateFileName(string fileName, string defaultFileName) {
			if(File.Exists(fileName))
				return fileName;
			if(string.IsNullOrEmpty(fileName))
				return defaultFileName;
			try {
				string tempFileName = Path.Combine(Path.GetTempPath(), fileName);
				File.Create(tempFileName).Close();
				File.Delete(tempFileName);
			} catch(Exception) {
				return defaultFileName;
			}
			return fileName;
		}
		protected static bool IsFileReadOnly(string fileName) {
			return File.Exists(fileName) && (File.GetAttributes(fileName) & FileAttributes.ReadOnly) != 0;
		}
		public void ExecExport(ExportOptionsBase options, IDictionary<Type, object[]> disabledExportModes) {
			CreateExportFiles(options, disabledExportModes, names => {
				if(ExportOptionsHelper.GetUseActionAfterExportAndSaveModeValue(options) && names.Length > 0)
					StartProcess(names[0]);
			});
		}
		void StartProcess(string fileName) {
#if !SILVERLIGHT
			if(File.Exists(fileName)) {
				switch(ps.ExportOptions.PrintPreview.ActionAfterExport) {
					case ActionAfterExport.Open:
						ProcessLaunchHelper.StartProcess(fileName, false);
						break;
					case ActionAfterExport.AskUser:
						if(ShouldOpenExportedFile(PreviewStringId.Msg_OpenFileQuestion, PreviewStringId.Msg_OpenFileQuestionCaption))
							ProcessLaunchHelper.StartProcess(fileName, false);
						break;
				}
			}
#endif
		}
		protected abstract bool ShouldOpenExportedFile(PreviewStringId messageId, PreviewStringId captionId);
		protected abstract void CreateExportFiles(ExportOptionsBase options, IDictionary<Type, object[]> disabledExportModes, Action<string[]> callback);
#if !SILVERLIGHT
		protected ExportFileHelperBase(PrintingSystemBase ps, EmailSenderBase emailSender) {
			if(ps == null)
				throw new ArgumentNullException("ps");
			if(emailSender == null)
				throw new ArgumentNullException("emailSender");
			this.ps = ps;
			this.emailSender = emailSender;
		}
		public void SendFileByEmail(ExportOptionsBase options, IDictionary<Type, object[]> disabledExportModes) {
			CreateExportFiles(options, disabledExportModes, fileNames => {
				if(fileNames.Length > 0)
					emailSender.Send(fileNames, ps.ExportOptions.Email);
			});
		}
#else
		protected ExportFileHelperBase(PrintingSystemBase ps) {
			if(ps == null)
				throw new ArgumentNullException("ps");
			this.ps = ps;
		}
#endif
	}
}
