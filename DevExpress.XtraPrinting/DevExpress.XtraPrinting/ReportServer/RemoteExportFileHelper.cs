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
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using System.IO;
using DevExpress.XtraPrinting.Native;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.ReportServer.ServiceModel.Native.RemoteOperations;
using DevExpress.ReportServer.Printing.Services;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.ReportServer.Printing {
	class RemoteExportFileHelper : ExportFileHelper {
		readonly RemoteOperationFactory factory;
		Action<string[]> callback;
		string fileName;
		ExportDocumentOperation operation;
		IDialogService DialogService { get { return ((IServiceProvider)ps).GetService<IDialogService>(); } }
		public RemoteExportFileHelper(PrintingSystemBase ps, EmailSenderBase sender, RemoteOperationFactory factory)
			: base(ps, sender) {
			this.factory = factory;
		}
		protected override void CreateExportFilesCore(ExportOptionsControllerBase controller, ExportOptionsBase options, string fileName, Action<string[]> callback) {
			this.callback = callback;
			this.fileName = fileName;
			ExportOptions exportOptions = new ExportOptions();
			exportOptions.Options.Clear();
			exportOptions.Options.Add(options.GetType(), options);
			operation = factory.CreateExportDocumentOperation(options.GetFormat(), exportOptions);
			operation.Progress += operation_Progress;
			operation.Completed += operation_Completed;
			operation.Start();
			ps.ProgressReflector.SetProgressRanges(new float[] { 100 });
		}
		void operation_Completed(object sender, ExportDocumentCompletedEventArgs e) {
			operation.Completed -= operation_Completed;
			operation.Progress -= operation_Progress;
			ps.ProgressReflector.MaximizeRange();
			if(e.Error != null) {
				DialogService.ShowErrorMessage(e.Error);
				return;
			}
			try {
				using(Stream stream = new FileStream(fileName, FileMode.Create)) {
					stream.Write(e.Data, 0, e.Data.Length);
				}
			} catch(Exception ex) {
				if(ex is IOException)
					ex = ExceptionHelper.CreateFriendlyException((IOException)ex, fileName);
				else if(ex is OutOfMemoryException)
					ex = ExceptionHelper.CreateFriendlyException((OutOfMemoryException)ex);
				DialogService.ShowErrorMessage(ex);
				return;
			}
			callback(new string[] { fileName });					
		}
		void operation_Progress(object sender, ExportDocumentProgressEventArgs e) {
			ps.ProgressReflector.SetPosition(e.ProgressPosition);
		}
	}
}
