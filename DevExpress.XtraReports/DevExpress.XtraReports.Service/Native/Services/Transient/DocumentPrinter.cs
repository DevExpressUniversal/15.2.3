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
using System.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.Service.Native.Services.Factories;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public class DocumentPrinter : DocumentWorkerBase, IDocumentPrinter {
		#region static
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		static void UpdateProgressPosition(IPrintMediator mediator, int currentPageIndex, int pageCount) {
			var progress = (int)Math.Ceiling(100f * currentPageIndex / (pageCount - 1));
			progress = Math.Max(Math.Min(progress, 100), 0);
			mediator.Entity.ProgressPosition = progress;
		}
		static void PreparePagesComplete(IPrintMediator mediator, DocumentId documentId, Exception exception = null) {
			if(exception != null) {
				mediator.ReloadEntity();
				mediator.Entity.Status = TaskStatus.Fault;
				mediator.Fault = new ServiceFault(exception);
				Logger.Error("({0}) Prepare pages for printing is completed with exception - {1}", documentId.Value, exception);
			} else {
				mediator.Entity.Status = TaskStatus.Complete;
				Logger.Info("({0}) Prepare pages for printing is successfully completed", documentId.Value);
			}
			mediator.Save();
		}
		#endregion
		readonly IDocumentMediatorFactory documentMediatorFactory;
		readonly IExportFactory exportFactory;
		readonly IPrintFactory printFactory;
		readonly ISerializationService serializationService;
		readonly IEnumerable<IDocumentPrintInterceptor> documentPrintInterceptors;
		public DocumentPrinter(
			IDocumentMediatorFactory documentMediatorFactory,
			IExportFactory exportFactory,
			IPrintFactory printFactory,
			ISerializationService serializationService,
			IExtensionsFactory extensionsFactory) {
			Guard.ArgumentNotNull(documentMediatorFactory, "documentMediatorFactory");
			Guard.ArgumentNotNull(exportFactory, "exportFactory");
			Guard.ArgumentNotNull(printFactory, "printFactory");
			Guard.ArgumentNotNull(serializationService, "serializationService");
			this.documentMediatorFactory = documentMediatorFactory;
			this.exportFactory = exportFactory;
			this.printFactory = printFactory;
			this.serializationService = serializationService;
			this.documentPrintInterceptors = extensionsFactory.GetDocumentPrintInterceptors();
		}
		#region IDocumentPrinter Members
		public virtual void PreparePages(DocumentId documentId, PrintId printId, PageCompatibility compatibility) {
			Guard.ArgumentNotNull(printId, "printId");
			Guard.ArgumentNotNull(documentId, "documentId");
			Logger.Info("({0}) Prepare pages for printing is starting", documentId.Value);
			using(var documentMediator = documentMediatorFactory.Create(documentId))
			using(CultureSwitcher.FromMediator(documentMediator))
			using(var printMediator = printFactory.CreatePrintMediator(printId)) {
				try {
					var document = documentMediator.LoadFullDocument();
					using(var printingSystem = document.PrintingSystem) {
						RaiseConfigurePrintingSystem(printingSystem);
						NotifyInterceptorsBefore(printingSystem.Document);
						PreparePagesCore(printingSystem, printMediator, compatibility);
						PreparePagesComplete(printMediator, documentId);
					}
				} catch(Exception e) {
					PreparePagesComplete(printMediator, documentId, e);
				}
			}
		}
		#endregion
		void PreparePagesCore(PrintingSystemBase printingSystem, IPrintMediator mediator, PageCompatibility compatibility) {
			EventHandler positionChanged = (sender, e) => {
				mediator.Entity.ProgressPosition = ((ProgressReflector)sender).Position;
				mediator.Save();
			};
			printingSystem.ProgressReflector.PositionChanged += positionChanged;
			ProcessPreparePages(printingSystem, mediator, compatibility);
			printingSystem.ProgressReflector.PositionChanged -= positionChanged;
		}
		void SavePreparedPages(string[] xamlPages, IPrintMediator printMediator) {
			using(var stream = new MemoryStream()) {
				serializationService.Serialize(xamlPages, stream);
				stream.Position = 0;
				printMediator.Content = stream;
			}
		}
		void ProcessPreparePages(PrintingSystemBase printingSystem, IPrintMediator printMediator, PageCompatibility pageCompatibility) {
			if(pageCompatibility != PageCompatibility.Silverlight && pageCompatibility != PageCompatibility.WPF) {
				throw new FaultException(string.Format("PageCompatibility '{0}' is not supported", pageCompatibility));
			}
			var document = printingSystem.Document;
			var shouldStop = false;
			var xamlPages = new string[document.Pages.Count];
			var xamlExporter = exportFactory.CreateXamlPagesExporter();
			xamlExporter.Compatibility = pageCompatibility.ToXamlCompatibility();
			for(int i = 0; i < document.PageCount; i++) {
				shouldStop = printMediator.ShouldStop;
				if(shouldStop) {
					break;
				}
				using(var stream = new MemoryStream()) {
					xamlExporter.Export(document, i, stream);
					stream.Position = 0;
					xamlPages[i] = stream.GetUTF8String();
				}
				UpdateProgressPosition(printMediator, i, document.PageCount);
				printMediator.Save();
			}
			if(!shouldStop) {
				NotifyInterceptorsAfter(xamlPages);
				SavePreparedPages(xamlPages, printMediator);
			}
		}
		void NotifyInterceptorsBefore(Document document) {
			documentPrintInterceptors.ForEach(x => x.InvokeBefore(document));
		}
		void NotifyInterceptorsAfter(string[] xamlPages) {
			documentPrintInterceptors.ForEach(x => x.InvokeAfter(xamlPages));
		}
	}
}
