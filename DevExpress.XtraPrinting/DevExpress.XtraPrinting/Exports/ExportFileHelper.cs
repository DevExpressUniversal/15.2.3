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

using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Preview;
namespace DevExpress.XtraPrinting.Export {
	class ExportFileHelper : ExportFileHelperBase {
		public ExportFileHelper(PrintingSystemBase ps, EmailSenderBase emailSender)
			: base(ps, emailSender) {
		}
		protected override bool ShouldOpenExportedFile(PreviewStringId messageId, PreviewStringId captionId) {
			return NotificationService.ShowMessage<PrintingSystemBase>(ps.Extend().LookAndFeel, ps.Extend().FindForm(), messageId.GetString(), captionId.GetString(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
		protected override void CreateExportFiles(ExportOptionsBase options, IDictionary<Type, object[]> disabledExportModes, Action<string[]> callback) {
			PrintPreviewOptions printPreviewOptions = ps.ExportOptions.PrintPreview;
			ExportOptionsControllerBase controller = ExportOptionsControllerBase.GetControllerByOptions(options);
			if(ExportOptionsHelper.GetShowOptionsBeforeExport(options, printPreviewOptions.ShowOptionsBeforeExport)) {
				if(DialogResult.OK != (disabledExportModes != null ? ExportOptionsTool.EditExportOptions(options, ps, disabledExportModes) : ExportOptionsTool.EditExportOptions(options, ps))) {
					callback(EmptyStrings);
					return;
				}
			}
			string proposedFileName = ValidateFileName(printPreviewOptions.DefaultFileName, PrintPreviewOptions.DefaultFileNameDefault);
			string fileName = string.Empty;
			IPrintingSystemExtender psExtender = ps.Extend();
			if(ExportOptionsHelper.GetUseActionAfterExportAndSaveModeValue(options) && printPreviewOptions.SaveMode == SaveMode.UsingDefaultPath) {
				string directory = string.IsNullOrEmpty(printPreviewOptions.DefaultDirectory) ? Directory.GetCurrentDirectory() : printPreviewOptions.DefaultDirectory;
				if(!Directory.Exists(directory))
					Directory.CreateDirectory(directory);
				fileName = Path.Combine(directory, proposedFileName + controller.GetFileExtension(options));
			} else {
				using(SaveFileDialog dlg = new SaveFileDialog()) {
					dlg.Title = PreviewLocalizer.GetString(PreviewStringId.SaveDlg_Title);
					dlg.ValidateNames = true;
					dlg.FileName = proposedFileName;
					dlg.Filter = controller.Filter;
					dlg.InitialDirectory = printPreviewOptions.DefaultDirectory;
					dlg.FilterIndex = controller.GetFilterIndex(options);
					dlg.OverwritePrompt = controller.ValidateInputFileName(options);
					try {
						if(DialogRunner.ShowDialog(dlg) == DialogResult.OK && !string.IsNullOrEmpty(dlg.FileName))
							fileName = FileHelper.SetValidExtension(dlg.FileName, controller.GetFileExtension(options), controller.FileExtensions);
						else {
							callback(EmptyStrings);
							return;
						}
					} catch(PathTooLongException ex) {
						Tracer.TraceError(NativeSR.TraceSource, ex);
						NotificationService.ShowException<PrintingSystemBase>(psExtender.LookAndFeel, psExtender.FindForm(), new Exception(PreviewStringId.Msg_PathTooLong.GetString(), ex));
						callback(EmptyStrings);
					}
				}
			}
			if(controller.ValidateInputFileName(options) && IsFileReadOnly(fileName)) {
				NotificationService.ShowMessage<PrintingSystemBase>(
					psExtender.LookAndFeel, psExtender.FindForm(),
					PreviewStringId.Msg_FileReadOnly.GetString(fileName),
					PreviewStringId.Msg_Caption.GetString(),
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				callback(EmptyStrings);
			}
			try {
				CreateExportFilesCore(controller, options, fileName, callback);
			} catch(Exception ex) {
				if(ex is IOException)
					ex = ExceptionHelper.CreateFriendlyException((IOException)ex, fileName);
				else if(ex is OutOfMemoryException)
					ex = ExceptionHelper.CreateFriendlyException((OutOfMemoryException)ex);
				Tracer.TraceError(NativeSR.TraceSource, ex);
				NotificationService.ShowException<PrintingSystemBase>(psExtender.LookAndFeel, psExtender.FindForm(), ex);
			}
		}
		protected virtual void CreateExportFilesCore(ExportOptionsControllerBase controller, ExportOptionsBase options, string fileName, Action<string[]> callback) {
			object result = ps.GetService<IWaitIndicator>().TryShow(PreviewStringId.Msg_ExportingDocument.GetString());
			try {
				callback(controller.GetExportedFileNames(ps, options, fileName));
			} finally {
				ps.GetService<IWaitIndicator>().TryHide(result);
			}
		}
	}
}
