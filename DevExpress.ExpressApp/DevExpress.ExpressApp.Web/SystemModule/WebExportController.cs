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
using System.IO;
using System.Web;
using DevExpress.XtraPrinting;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;
using System.Drawing.Imaging;
using DevExpress.ExpressApp.Utils;
using System.ComponentModel;
using DevExpress.Web.Internal;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class CustomWriteToResponseEventArgs : HandledEventArgs {
		public MemoryStream Stream { get; private set; }
		public string FileName { get; private set; }
		public CustomWriteToResponseEventArgs(MemoryStream stream, string fileName) {
			Stream = stream;
			FileName = fileName;
		}
	}
	public class WebExportController : ExportController {
		protected override void Export(ExportTarget exportTarget) {
			string fileExtention, fileName;
			using(MemoryStream stream = new MemoryStream()) {
				fileExtention = GetDefaultExtension(exportTarget);
				fileName = GetDefaultFileName();
				ExportCore(exportTarget, stream, ImageFormat.Png);
				CustomWriteToResponseEventArgs args = new CustomWriteToResponseEventArgs(stream, String.Concat(fileName, ".", fileExtention));
				if(CustomWriteToResponse != null) {
					CustomWriteToResponse(this, args);
				}
				if(!args.Handled) {
					HttpContext.Current.Response.ClearHeaders(); 
					ResponseWriter.WriteFileToResponse(stream, args.FileName);
				}
			}
		}
		public event EventHandler<CustomWriteToResponseEventArgs> CustomWriteToResponse;
	}
	public static class ResponseWriter {
		public static void WriteFileToResponse(IFileData fileData) {
			MemoryStream stream = new MemoryStream();
			fileData.SaveToStream(stream);
			WriteFileToResponse(stream, fileData.FileName);
		}
		public static void WriteFileToResponse(Stream stream, string fileName) {
			string fileFormat = Path.GetExtension(fileName).TrimStart('.');
			WriteFileToResponse(stream, Path.GetFileNameWithoutExtension(fileName), fileFormat, HttpUtils.GetContentType(fileFormat));
		}
		private static void WriteFileToResponse(Stream stream, string fileName, string fileFormat, string contentType) {
			stream.Position = 0;
			CustomWriteFileToResponseEventArgs args = new CustomWriteFileToResponseEventArgs(stream, fileName, fileFormat, contentType);
			if(CustomWriteFileToResponse != null) {
				CustomWriteFileToResponse(null, args);
			}
			if(!args.Handled) {
				HttpUtils.WriteFileToResponse(null, args.Stream, args.FileName, true, args.FileFormat, args.ContentType, false);
			}
		}
		public static void WriteTextFileToResponse(string fileName, string content) {
			if(!string.IsNullOrEmpty(content)) {
				using(MemoryStream stream = new MemoryStream()) {
					StreamWriter sw = new StreamWriter(stream);
					sw.WriteLine(content);
					sw.Flush();
					string fileFormat = Path.GetExtension(fileName).TrimStart('.');
					WriteFileToResponse(stream, Path.GetFileNameWithoutExtension(fileName), fileFormat, HttpUtils.GetContentType("txt"));
				}
			}
		}
		public static event EventHandler<CustomWriteFileToResponseEventArgs> CustomWriteFileToResponse;
	}
	public class CustomWriteFileToResponseEventArgs : HandledEventArgs{
		public Stream Stream { get; set; }
		public string FileName { get; set; }
		public string FileFormat { get; set; }
		public string ContentType { get; set; }
		public CustomWriteFileToResponseEventArgs(Stream stream, string fileName, string fileFormat, string contentType) {
			Stream = stream;
			FileName = fileName;
			FileFormat = fileFormat;
			ContentType = contentType;
		}
	}
	public class WebExportAnalysisController : ExportAnalysisController {
		private IList<PrintingSystemCommand> allExportTypes;
		private IList<PrintingSystemCommand> supportedExportTypes;
		private IList<PrintingSystemCommand> CreateAllExportTypes() {
			IList<PrintingSystemCommand> exportTypes = new List<PrintingSystemCommand>();
			exportTypes.Add(PrintingSystemCommand.ExportXls);
			exportTypes.Add(PrintingSystemCommand.ExportXlsx);
			exportTypes.Add(PrintingSystemCommand.ExportHtm);
			exportTypes.Add(PrintingSystemCommand.ExportTxt);
			exportTypes.Add(PrintingSystemCommand.ExportMht);
			exportTypes.Add(PrintingSystemCommand.ExportPdf);
			exportTypes.Add(PrintingSystemCommand.ExportRtf);
			exportTypes.Add(PrintingSystemCommand.ExportGraphic);
			return exportTypes;
		}
		private string GetFileExtension(PrintingSystemCommand exportType) {
			string result = "";
			switch(exportType) {
				case PrintingSystemCommand.ExportMht:
					result = ".mht";
					break;
				case PrintingSystemCommand.ExportXls:
					result = ".xls";
					break;
				case PrintingSystemCommand.ExportXlsx:
					result = ".xlsx";
					break;
				case PrintingSystemCommand.ExportPdf:
					result = ".pdf";
					break;
				case PrintingSystemCommand.ExportRtf:
					result = ".rtf";
					break;
				case PrintingSystemCommand.ExportTxt:
					result = ".txt";
					break;
				case PrintingSystemCommand.ExportHtm:
					result = ".htm";
					break;
				case PrintingSystemCommand.ExportGraphic:
					result = ".png";
					break;
			}
			return result;
		}
		protected override IList<PrintingSystemCommand> GetExportTypes() {
			supportedExportTypes = base.GetExportTypes();
			return allExportTypes;
		}
		protected override void OnExported(CustomExportAnalysisEventArgs args) {
			base.OnExported(args);
			ResponseWriter.WriteFileToResponse(args.Stream, String.Concat(DefaultFileName, GetFileExtension(args.ExportType)));
		}
		protected override IStreamProvider CreateStreamProvider() {
			return new MemoryStreamProvider();
		}
		protected override void UpdateActionState() {
			foreach(ChoiceActionItem item in ExportAction.Items) {
				item.Active.SetItemValue("Export type is visible", supportedExportTypes.Contains((PrintingSystemCommand)item.Data));
			}
			base.UpdateActionState();
		}
		protected override void OnActivated() {
			base.OnActivated();
			ExportAction.Model.SetValue<bool>("IsPostBackRequired", true);
		}
		public WebExportAnalysisController()
			: base() {
			allExportTypes = CreateAllExportTypes();
		}
	}
}
