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

using DevExpress.Web.Internal;
using DevExpress.Web.Office.Internal;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public static class RichEditDownloadManager {
		static Dictionary<DownloadRequestType, Type> CommandsDictionary = new Dictionary<DownloadRequestType, Type>() {
			{ DownloadRequestType.DownloadCurrentDocument, typeof(DownloadCurrentDocumentCommand) },
			{ DownloadRequestType.DownloadMergedDocument, typeof(DownloadMergedDocumentCommand) },
			{ DownloadRequestType.PrintCurrentDocument, typeof(PrintCurrentDocumentCommand) }
		};
		static RichEditDownloadCommand CreateCommand(RichEditWorkSession workSession, NameValueCollection parameters) {
			DownloadRequestType downloadRequestType = (DownloadRequestType)Enum.Parse(typeof(DownloadRequestType), parameters["downloadRequestType"]);
			Type commandType;
			if(CommandsDictionary.TryGetValue(downloadRequestType, out commandType))
				return Activator.CreateInstance(commandType, new object[] { workSession, parameters }) as RichEditDownloadCommand;
			throw new Exception("Command Not Found");
		}
		public static DocumentHandlerResponse GetResponse(RichEditWorkSession workSession, NameValueCollection parameters) {
			RichEditDownloadCommand command = CreateCommand(workSession, parameters);
			return command.Execute();
		}
	}
	public abstract class RichEditDownloadCommand {
		public RichEditDownloadCommand(RichEditWorkSession workSession, NameValueCollection parameters) {
			RichEdit = workSession.RichEdit;
			Parameters = parameters["parameters"] != null ? HtmlConvertor.FromJSON<Hashtable>(parameters["parameters"]) : null;
			BehaviorOptions = workSession.GetClient(Guid.Parse(parameters["c"])).RichEditBehaviorOptions;
		}
		protected InternalRichEditDocumentServer RichEdit { get; private set; }
		protected Hashtable Parameters { get; private set; }
		protected RichEditBehaviorOptions BehaviorOptions { get; private set; }
		protected abstract bool IsEnabled();
		protected abstract string GetFileName();
		protected abstract string GetFileExtension();
		protected abstract bool IsAsAttachment();
		protected abstract void FillStream(Stream stream);
		protected internal DocumentHandlerResponse Execute() {
			using(MemoryStream stream = new MemoryStream()) {
				if(IsEnabled())
					FillStream(stream);
				ResponseFileInfo fileInfo = new ResponseFileInfo(GetFileName() + GetFileExtension(), stream.ToArray());
				fileInfo.AsAttachment = IsAsAttachment();
				AttachmentDocumentHandlerResponse documentResponse = new AttachmentDocumentHandlerResponse();
				documentResponse.ContentEncoding = Encoding.UTF8;
				documentResponse.ResponseFile = fileInfo;
				documentResponse.AutodetectContentType = true;
				return documentResponse;
			}
		}
	}
	public class DownloadCurrentDocumentCommand : RichEditDownloadCommand {
		public DownloadCurrentDocumentCommand(RichEditWorkSession workSession, NameValueCollection parameters)
		: base(workSession, parameters) { }
		protected override bool IsEnabled() {
			return BehaviorOptions.SaveAsAllowed;
		}
		protected override string GetFileName() {
			string fileName = Path.GetFileNameWithoutExtension(RichEdit.DocumentModel.DocumentSaveOptions.CurrentFileName);
			if(string.IsNullOrEmpty(fileName))
				fileName = "Document1";
			return fileName;
		}
		protected override string GetFileExtension() {
			return Parameters["fileExtension"].ToString();
		}
		protected override bool IsAsAttachment() {
			return true;
		}
		protected override void FillStream(Stream stream) {
			RichEdit.SaveDocument(stream, RichEdit.DocumentModel.AutodetectDocumentFormat(GetFileExtension(), false));
		}
	}
	public class DownloadMergedDocumentCommand : RichEditDownloadCommand {
		public DownloadMergedDocumentCommand(RichEditWorkSession workSession, NameValueCollection parameters)
		: base(workSession, parameters) { }
		protected override bool IsEnabled() {
			return BehaviorOptions.SaveAsAllowed;
		}
		protected override string GetFileName() {
			return "MergedDocument";
		}
		protected override string GetFileExtension() {
			return Parameters["fileExtension"].ToString();
		}
		protected override bool IsAsAttachment() {
			return true;
		}
		protected override void FillStream(Stream stream) {
			XtraRichEdit.API.Native.MailMergeOptions mailMergeOptions = RichEdit.CreateMailMergeOptions();
			mailMergeOptions.FirstRecordIndex = (int)Parameters["firstRecordIndex"];
			mailMergeOptions.LastRecordIndex = (int)Parameters["lastRecordIndex"];
			mailMergeOptions.MergeMode = (XtraRichEdit.API.Native.MergeMode)Parameters["mergeMode"];
			RichEdit.MailMerge(mailMergeOptions, stream, RichEdit.DocumentModel.AutodetectDocumentFormat(GetFileExtension(), false));
		}
	}
	public class PrintCurrentDocumentCommand : RichEditDownloadCommand {
		public PrintCurrentDocumentCommand(RichEditWorkSession workSession, NameValueCollection parameters)
		: base(workSession, parameters) { }
		protected override bool IsEnabled() {
			return BehaviorOptions.PrintingAllowed;
		}
		protected override string GetFileName() {
			return "Print";
		}
		protected override string GetFileExtension() {
			return ".pdf";
		}
		protected override bool IsAsAttachment() {
			return false;
		}
		protected override void FillStream(Stream stream) {
			XtraPrinting.PdfExportOptions options = new XtraPrinting.PdfExportOptions();
			options.ShowPrintDialogOnOpen = true;
			RichEdit.DocumentModel.ToggleAllFieldCodes(false);
			RichEdit.ExportToPdf(stream, options);
		}
	}
}
