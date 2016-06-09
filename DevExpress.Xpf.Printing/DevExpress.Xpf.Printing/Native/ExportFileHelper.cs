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
using System.IO;
using System.Windows;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using Microsoft.Win32;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.Native.Lines;
#if !SILVERLIGHT
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Printing.Native {
	class ExportFileHelper : ExportFileHelperBase {
		IDialogService dialogService;
#if !SILVERLIGHT
		readonly Window ownerWindow;
		public ExportFileHelper(PrintingSystemBase ps, EmailSenderBase emailSender, Window ownerWindow, IDialogService dialogService)
			: base(ps, emailSender) {
			this.ownerWindow = ownerWindow;
			this.dialogService = dialogService;
		}
#else
		public ExportFileHelper(IDialogService dialogService) {
			this.dialogService = dialogService;
		}
#endif
		protected override void CreateExportFiles(ExportOptionsBase options, System.Collections.Generic.IDictionary<Type, object[]> disabledExportModes, Action<string[]> callback) {
			if(options == null)
				throw new ArgumentNullException("options");
			PrintPreviewOptions printPreviewOptions = ps.ExportOptions.PrintPreview;
			ExportOptionsControllerBase controller = ExportOptionsControllerBase.GetControllerByOptions(options);
			if(ExportOptionsHelper.GetShowOptionsBeforeExport(options, printPreviewOptions.ShowOptionsBeforeExport)) {
				ExportOptionsBase clonedOptions = ExportOptionsHelper.CloneOptions(options);
				LineBase[] lines = Array.ConvertAll(controller.GetExportLines(clonedOptions, new LineFactory(), ps.Document.AvailableExportModes, ps.ExportOptions.HiddenOptions), line => (LineBase)line);
				if(lines.Length > 0) {
					LinesWindow linesWindow = new LinesWindow();
					linesWindow.WindowStyle = WindowStyle.ToolWindow;
					linesWindow.Title = PreviewLocalizer.GetString(controller.CaptionStringId);
					linesWindow.Owner = ownerWindow;
					if(ownerWindow != null) {
						linesWindow.FlowDirection = ownerWindow.FlowDirection;
					}
					linesWindow.SetLines(lines);
					if(linesWindow.ShowDialog() == true) {
						options.Assign(clonedOptions);
					} else
						return;
				}
			}
			callback(CreateExportFilesCore(options, printPreviewOptions, controller));
		}
		protected override bool ShouldOpenExportedFile(PreviewStringId messageId, PreviewStringId captionId) {
			return DXMessageBox.Show(
				PreviewLocalizer.GetString(messageId),
				PreviewLocalizer.GetString(captionId),
				MessageBoxButton.YesNo,
				MessageBoxImage.Question) == MessageBoxResult.Yes;
		}
		string[] CreateExportFilesCore(ExportOptionsBase options, PrintPreviewOptions printPreviewOptions, ExportOptionsControllerBase controller) {
			string proposedFileName = ValidateFileName(printPreviewOptions.DefaultFileName, PrintPreviewOptions.DefaultFileNameDefault);
			string fileName = string.Empty;
			if(ExportOptionsHelper.GetUseActionAfterExportAndSaveModeValue(options) && printPreviewOptions.SaveMode == SaveMode.UsingDefaultPath) {
				string directory = string.IsNullOrEmpty(printPreviewOptions.DefaultDirectory) ? Directory.GetCurrentDirectory() : printPreviewOptions.DefaultDirectory;
				if(!Directory.Exists(directory))
					Directory.CreateDirectory(directory);
				fileName = Path.Combine(directory, proposedFileName + controller.GetFileExtension(options));
			} else {
				SaveFileDialog dlg = CreateSaveFileDialog(options, printPreviewOptions, controller, proposedFileName);
				if(dlg.ShowDialog() == true && !string.IsNullOrEmpty(dlg.FileName))
					fileName = FileHelper.SetValidExtension(dlg.FileName, controller.GetFileExtension(options), controller.FileExtensions);
				else
					return EmptyStrings;
			}
			if(controller.ValidateInputFileName(options) && IsFileReadOnly(fileName)) {
				MessageBoxHelper.Show(MessageBoxButton.OK, MessageBoxImage.Exclamation, PreviewStringId.Msg_FileReadOnly, fileName);
				return EmptyStrings;
			}
			try {
				return controller.GetExportedFileNames(ps, options, fileName);
			} catch(IOException) {
				dialogService.ShowError(string.Format(PreviewLocalizer.GetString(PreviewStringId.Msg_CannotAccessFile), fileName), PrintingLocalizer.GetString(PrintingStringId.Error));
				return EmptyStrings;
			} catch(OutOfMemoryException) {
				dialogService.ShowError(PreviewLocalizer.GetString(PreviewStringId.Msg_BigFileToCreate), PrintingLocalizer.GetString(PrintingStringId.Error));
				return EmptyStrings;
			} catch(Exception exception) {
				dialogService.ShowError(exception.Message, PrintingLocalizer.GetString(PrintingStringId.Error));
				return EmptyStrings;
			}
		}
		static SaveFileDialog CreateSaveFileDialog(ExportOptionsBase options, PrintPreviewOptions printPreviewOptions, ExportOptionsControllerBase controller, string proposedFileName) {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Title = PreviewLocalizer.GetString(PreviewStringId.SaveDlg_Title);
			dlg.ValidateNames = true;
			dlg.FileName = proposedFileName;
			dlg.Filter = controller.Filter;
			dlg.InitialDirectory = printPreviewOptions.DefaultDirectory;
			dlg.FilterIndex = controller.GetFilterIndex(options);
			dlg.OverwritePrompt = controller.ValidateInputFileName(options);
			return dlg;
		}
	}
}
