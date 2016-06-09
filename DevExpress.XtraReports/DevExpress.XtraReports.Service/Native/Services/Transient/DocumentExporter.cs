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
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.Service.Native.Services.Factories;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public class DocumentExporter : DocumentWorkerBase, IDocumentExporter {
		const string TextContentTypePrefix = "text/";
		const string ImageContentTypePrefix = "image/";
		const string ApplicationContentTypePrefix = "application/";
		static readonly Dictionary<Type, ExportOptionsInfo> ExportOptionsByInfo = new Dictionary<Type, ExportOptionsInfo> {
			{ typeof(CsvExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToCsv(stream, (CsvExportOptions)eo), TextContentTypePrefix + "csv") },
			{ typeof(HtmlExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToHtml(stream, (HtmlExportOptions)eo), TextContentTypePrefix + "html") },
			{ typeof(XpsExportOptions), new XpsExportOptionsInfo((ps, stream, eo) => ps.ExportToXps(stream, (XpsExportOptions)eo), ApplicationContentTypePrefix + "vnd.ms-xpsdocument") },
			{ typeof(MhtExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToMht(stream, (MhtExportOptions)eo), "message/rfc822") },
			{ typeof(PdfExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToPdf(stream, (PdfExportOptions)eo), ApplicationContentTypePrefix + "pdf") },
			{ typeof(ImageExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToImage(stream, (ImageExportOptions)eo)) },
			{ typeof(RtfExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToRtf(stream, (RtfExportOptions)eo), ApplicationContentTypePrefix + "rtf") },
			{ typeof(TextExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToText(stream, (TextExportOptions)eo), TextContentTypePrefix + "plain") },
			{ typeof(XlsxExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToXlsx(stream, (XlsxExportOptions)eo), ApplicationContentTypePrefix + "vnd.openxmlformats-officedocument.spreadsheetml.sheet") },
			{ typeof(XlsExportOptions), new ExportOptionsInfo((ps, stream, eo) => ps.ExportToXls(stream, (XlsExportOptions)eo), ApplicationContentTypePrefix + "excel") },
			{ typeof(NativeFormatOptions), new ExportOptionsInfo((ps, stream, eo) => ps.SaveDocument(stream, (NativeFormatOptions)eo), ApplicationContentTypePrefix + "native") }
		};
		static readonly Dictionary<ImageFormat, ExportedDocumentResponseInfo> ResponseInfoByImageFormat;
		static DocumentExporter() {
			var imageContentTypes = new Dictionary<ImageFormat, string> {
				{ ImageFormat.Bmp, ImageContentTypePrefix + "bmp" },
				{ ImageFormat.Gif, ImageContentTypePrefix + "gif" },
				{ ImageFormat.Jpeg, ImageContentTypePrefix + "jpeg" },
				{ ImageFormat.Png, ImageContentTypePrefix + "png" },
				{ ImageFormat.Emf, ImageContentTypePrefix + "emf" },
				{ ImageFormat.Wmf, ApplicationContentTypePrefix + "x-msmetafile" },
				{ ImageFormat.Tiff, ImageContentTypePrefix + "tiff" }
			};
			ResponseInfoByImageFormat = new Dictionary<ImageFormat, ExportedDocumentResponseInfo>();
			foreach(var imageFormat in ImageExportOptions.ImageFormats) {
				string contentType;
				if(!imageContentTypes.TryGetValue(imageFormat.Value, out contentType)) {
					Debug.Fail(string.Format("Not supported ImageFormat '{0}'", imageFormat.Value));
				}
				ResponseInfoByImageFormat[imageFormat.Value] = new ExportedDocumentResponseInfo {
					FileExt = imageFormat.Key,
					ContentType = contentType
				};
			}
		}
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		static void SaveResult(IExportMediator exportMediator, Stream stream, PrintingSystemBase printingSystem, ExportedDocumentResponseInfo responseInfo) {
			var documentName = printingSystem.Document.Name;
			documentName = string.IsNullOrWhiteSpace(documentName)
				? "Document"
				: documentName;
			exportMediator.Content = stream;
			exportMediator.Entity.Name = GetSafeFileName(documentName, responseInfo.FileExt);
			exportMediator.Entity.ContentType = responseInfo.ContentType ?? string.Empty;
		}
		static string GetSafeFileName(string documentName, string fileExt) {
			const char Replacement = '_';
			foreach(var invalidChar in Path.GetInvalidPathChars()) {
				documentName = documentName.Replace(invalidChar, Replacement);
			}
			return string.IsNullOrEmpty(fileExt)
				? documentName
				: documentName + fileExt;
		}
		static ExportedDocumentResponseInfo GetExportedDocumentResponseInfo(ExportOptionsBase exportOptions, string contentType) {
			var imageExportOptions = exportOptions as ImageExportOptions;
			ExportedDocumentResponseInfo responseInfo;
			if(imageExportOptions != null && ResponseInfoByImageFormat.TryGetValue(imageExportOptions.Format, out responseInfo)) {
				return responseInfo;
			}
			var availableExtensions = ExportOptionsControllerBase.GetControllerByOptions(exportOptions).FileExtensions;
			var fileExtension = availableExtensions.FirstOrDefault() ?? string.Empty;
			return new ExportedDocumentResponseInfo {
				FileExt = fileExtension,
				ContentType = contentType
			};
		}
		readonly IDocumentMediatorFactory documentMediatorFactory;
		readonly IExportFactory exportFactory;
		readonly IDALService dalService;
		readonly IPrintingSystemFactory printingSystemFactory;
		readonly IEnumerable<IDocumentExportInterceptor> documentExportInterceptors;
		DocumentId documentId;
		ExportId exportId;
		public DocumentExporter(
			IDocumentMediatorFactory documentMediatorFactory,
			IExportFactory exportFactory,
			IDALService dalService,
			IPrintingSystemFactory printingSystemFactory,
			IExtensionsFactory extensionsFactory) {
			Guard.ArgumentNotNull(documentMediatorFactory, "documentMediatorFactory");
			Guard.ArgumentNotNull(exportFactory, "exportFactory");
			Guard.ArgumentNotNull(dalService, "dalService");
			Guard.ArgumentNotNull(printingSystemFactory, "printingSystemFactory");
			Guard.ArgumentNotNull(extensionsFactory, "extensionsFactory");
			this.documentMediatorFactory = documentMediatorFactory;
			this.exportFactory = exportFactory;
			this.dalService = dalService;
			this.printingSystemFactory = printingSystemFactory;
			this.documentExportInterceptors = extensionsFactory.GetDocumentExportInterceptors();
		}
		#region IDocumentExporter Members
		public void Export(DocumentId documentId, ExportId exportId, DocumentExportConfiguration configuration) {
			Guard.ArgumentNotNull(documentId, "documentId");
			Guard.ArgumentNotNull(exportId, "exportId");
			this.documentId = documentId;
			this.exportId = exportId;
			Logger.Info("({0}) Export is starting", documentId.Value);
			using(var documentMediator = documentMediatorFactory.Create(documentId))
			using(var exportMediator = exportFactory.CreateExportMediator(exportId)) {
				try {
					var document = documentMediator.LoadFullDocument();
					using(var printingSystem = document.PrintingSystem) {
						RaiseConfigurePrintingSystem(printingSystem);
						ProcessExport(printingSystem, exportMediator, configuration);
						SaveExportStatus(exportMediator);
					}
				} catch(Exception e) {
					SaveExportStatus(exportMediator, e);
				}
			}
		}
		#endregion
		protected virtual void ExportComplete(IExportMediator exportMediator) {
			exportMediator.Save();
		}
		void SaveExportStatus(IExportMediator exportMediator, Exception exception = null) {
			if(exception == null) {
				exportMediator.Entity.Status = TaskStatus.Complete;
				Logger.Info("({0}) Export is successfully completed", documentId.Value);
			} else {
				exportMediator.ReloadEntity();
				exportMediator.Entity.Status = TaskStatus.Fault;
				exportMediator.Fault = new ServiceFault(exception);
				Logger.Error("({0}) Export is completed with exception - {1}", documentId.Value, exception);
			}
			ExportComplete(exportMediator);
		}
		void ExportCore(PrintingSystemBase printingSystem, IExportMediator exportMediator, DocumentExportConfiguration configuration) {
			var exportOptions = configuration.ExportOptions;
			using(var ms = new MemoryStream()) {
				ExportOptionsInfo info;
				if(!ExportOptionsByInfo.TryGetValue(exportOptions.GetType(), out info)) {
					throw new Exception(Messages.UnregisteredExportOption);
				}
				NotifyInterceptorsBefore(printingSystem.Document, configuration);
				info.Export(printingSystem, ms, exportOptions, printingSystemFactory);
				ms.Position = 0;
				NotifyInterceptorsAfter(ms, configuration);
				ms.Position = 0;
				var responseInfo = GetExportedDocumentResponseInfo(exportOptions, info.ContentType);
				SaveResult(exportMediator, ms, printingSystem, responseInfo);
			}
		}
		void ProcessExport(PrintingSystemBase printingSystem, IExportMediator exportMediator, DocumentExportConfiguration configuration) {
			EventHandler positionChanged = (s, e) => {
				exportMediator.Entity.ProgressPosition = ((ProgressReflector)s).Position;
				exportMediator.Save();
			};
			printingSystem.ProgressReflector.PositionChanged += positionChanged;
			ExportCore(printingSystem, exportMediator, configuration);
			printingSystem.ProgressReflector.PositionChanged -= positionChanged;
		}
		void NotifyInterceptorsBefore(Document document, DocumentExportConfiguration configuration) {
			documentExportInterceptors.ForEach(x => x.InvokeBefore(document, configuration.ExportOptions, configuration.CustomArgs));
		}
		void NotifyInterceptorsAfter(Stream stream, DocumentExportConfiguration configuration) {
			documentExportInterceptors.ForEach(x => x.InvokeAfter(stream, configuration.ExportOptions, configuration.CustomArgs));
		}
	}
}
