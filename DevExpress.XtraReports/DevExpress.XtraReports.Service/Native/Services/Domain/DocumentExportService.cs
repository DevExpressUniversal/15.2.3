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
using System.Drawing.Imaging;
using System.IO;
using System.ServiceModel;
using System.Threading;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.XamlExport;
using DevExpress.XtraReports.Service.Native.Services.Factories;
using DevExpress.XtraReports.Service.Native.Services.Transient;
namespace DevExpress.XtraReports.Service.Native.Services.Domain {
	public class DocumentExportService : IDocumentExportService {
		readonly IDocumentMediatorFactory documentMediatorFactory;
		readonly IExportFactory exportFactory;
		readonly IThreadFactoryService threadFactoryService;
		readonly IPrintingSystemFactory printingSystemFactory;
		readonly ISerializationService serializationService;
		readonly Dictionary<PageCompatibility, Func<int?, IPagesExporter>> pagesExporterCreator;
		public DocumentExportService(
			IDocumentMediatorFactory documentMediatorFactory,
			IExportFactory exportFactory,
			IThreadFactoryService threadFactoryService,
			IPrintingSystemFactory printingSystemFactory,
			ISerializationService serializationService) {
			Guard.ArgumentNotNull(documentMediatorFactory, "documentMediatorFactory");
			Guard.ArgumentNotNull(exportFactory, "exportFactory");
			Guard.ArgumentNotNull(threadFactoryService, "threadFactoryService");
			Guard.ArgumentNotNull(printingSystemFactory, "printingSystemFactory");
			Guard.ArgumentNotNull(serializationService, "serializationService");
			this.documentMediatorFactory = documentMediatorFactory;
			this.exportFactory = exportFactory;
			this.threadFactoryService = threadFactoryService;
			this.printingSystemFactory = printingSystemFactory;
			this.serializationService = serializationService;
			pagesExporterCreator = new Dictionary<PageCompatibility, Func<int?, IPagesExporter>> {
				{ PageCompatibility.Silverlight, _ => CreateXamlPagesExporter(XamlCompatibility.Silverlight) },
				{ PageCompatibility.WPF, _ => CreateXamlPagesExporter(XamlCompatibility.WPF) },
				{ PageCompatibility.Prnx, _ => exportFactory.CreatePrnxPagesExporter() },
				{ PageCompatibility.HTML, _ => exportFactory.CreateHtmlPagesExporter() },
				{ PageCompatibility.ImagePng, addition => exportFactory.CreateImagePagesExporter(ImageFormat.Png, resolution: addition) }
			};
		}
		#region IExporterService Members
		public ExportId StartExport(DocumentId documentId, DocumentExportConfiguration configuration) {
			using(var documentMediator = documentMediatorFactory.Create(documentId)) {
				if(documentMediator.Entity.Status != TaskStatus.Complete) {
					throw new FaultException(Messages.FaultDocumentBuildingIsNotCompleted);
				}
				var exportId = ExportId.GenerateNew();
				using(var exportMediator = exportFactory.CreateExportMediator(exportId, MediatorInitialization.New)) {
					exportMediator.Entity.Status = TaskStatus.InProgress;
					exportMediator.Save();
				}
				var documentExporter = exportFactory.CreateDocumentExporter();
				using(CultureSwitcher.FromMediator(documentMediator)) {
					ApartmentState apartmentState = configuration.ExportOptions is XpsExportOptions
						? ApartmentState.STA
						: ApartmentState.MTA;
					threadFactoryService.Start(() => documentExporter.Export(documentId, exportId, configuration), apartmentState);
				}
				return exportId;
			}
		}
		public ExportStatus GetExportStatus(ExportId exportId) {
			using(var exportMediator = exportFactory.CreateExportMediator(exportId)) {
				return new ExportStatus {
					ExportId = exportId,
					ProgressPosition = exportMediator.Entity.ProgressPosition,
					Status = exportMediator.Entity.Status,
					Fault = exportMediator.Fault
				};
			}
		}
		public Stream GetExportedDocument(ExportId exportId) {
			using(var exportMediator = exportFactory.CreateExportMediator(exportId)) {
				return exportMediator.Content;
			}
		}
		public string ExportPage(Document document, int pageIndex, PageCompatibility compatibility, int? addition, bool isThreadSafe) {
			var exporter = CreatePagesExporter(compatibility, addition, isThreadSafe);
			return exporter.Export(document, pageIndex);
		}
		public byte[] ExportPages(Document document, int[] pageIndexes, PageCompatibility compatibility, int? addition, bool isThreadSafe) {
			var exporter = CreatePagesExporter(compatibility, addition, isThreadSafe);
			return exporter.Export(document, pageIndexes);
		}
		#endregion
		IPagesExporter CreatePagesExporter(PageCompatibility compatibility, int? addition, bool isThreadSafe) {
			var exporter = pagesExporterCreator[compatibility](addition);
			return isThreadSafe && exporter.ExclusivelyDocumentUsing
				? (IPagesExporter)new ExclusivelyDocumentPagesExporterWrapper(exporter, serializationService.PrintingSystemSerializer, printingSystemFactory)
				: exporter;
		}
		IXamlPagesExporter CreateXamlPagesExporter(XamlCompatibility compatibility) {
			var exporter = exportFactory.CreateXamlPagesExporter();
			exporter.Compatibility = compatibility;
			return exporter;
		}
	}
}
