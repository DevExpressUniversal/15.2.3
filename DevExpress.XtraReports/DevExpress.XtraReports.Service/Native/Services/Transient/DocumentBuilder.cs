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
using System.Drawing.Printing;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.Native.DrillDown;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.Service.Native.Services.Domain;
using DevExpress.XtraReports.Service.Native.Services.Factories;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public class DocumentBuilder : DocumentWorkerBase, IDocumentBuilder {
		enum DocumentBuildResult {
			None,
			Success,
			Cancelled
		}
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		readonly IDocumentMediatorFactory documentMediatorFactory;
		readonly IDocumentBuildFactory documentBuildFactory;
		readonly IThreadFactoryService threadFactoryService;
		readonly IDocumentBuildService documentBuildService;
		readonly IPagesRequestProcessor pagesRequestProcessor;
		readonly IEnumerable<IReportBuildInterceptor> reportBuildInterceptors;
		public bool ProcessPageRequests { get; set; }
		public DocumentBuilder(
			IDocumentMediatorFactory documentMediatorFactory,
			IDocumentBuildFactory documentBuildFactory,
			IThreadFactoryService threadFactoryService,
			IDocumentBuildService documentBuildService,
			IPagesRequestProcessor pagesRequestProcessor,
			IExtensionsFactory extensionsFactory) {
			Guard.ArgumentNotNull(documentMediatorFactory, "documentMediatorFactory");
			Guard.ArgumentNotNull(documentBuildFactory, "documentBuildFactory");
			Guard.ArgumentNotNull(threadFactoryService, "threadFactoryService");
			Guard.ArgumentNotNull(documentBuildService, "documentBuildService");
			Guard.ArgumentNotNull(pagesRequestProcessor, "pagesRequestProcessor");
			Guard.ArgumentNotNull(extensionsFactory, "extensionsFactory");
			this.documentMediatorFactory = documentMediatorFactory;
			this.documentBuildFactory = documentBuildFactory;
			this.threadFactoryService = threadFactoryService;
			this.documentBuildService = documentBuildService;
			this.pagesRequestProcessor = pagesRequestProcessor;
			this.reportBuildInterceptors = extensionsFactory.GetReportBuildInterceptors();
			ProcessPageRequests = true;
		}
		#region IDocumentBuilder Members
		public virtual void Build(DocumentId documentId, XtraReport report, ReportConfiguration reportConfiguration) {
			Guard.ArgumentNotNull(documentId, "documentId");
			Guard.ArgumentNotNull(report, "report");
			Logger.Info("({0}) Build is starting", documentId.Value);
			var buildResult = DocumentBuildResult.None;
			using(var mediator = documentMediatorFactory.Create(documentId)) {
				try {
					report.PrintingSystem.ReplaceService<IBackgroundService>(new BackgroundService(mediator, report));
					buildResult = BuildCore(report, mediator, reportConfiguration, documentId);
				} catch(DocumentDoesNotExistException e) {
					mediator.DocumentFault = new ServiceFault(e);
					mediator.Entity.Status = TaskStatus.Fault;
				} finally {
					if(buildResult == DocumentBuildResult.Cancelled && !mediator.Entity.IsPermanent) {
						mediator.Delete();
					} else {
						mediator.Save();
					}
				}
			}
		}
		#endregion
		protected virtual void ConfigureReport(XtraReport report, Watermark watermark, PageData pageData, Dictionary<DrillDownKey, bool> drillDownKeys) {
			AssignReportPageData(report, pageData);
			if(watermark != null) {
				report.Watermark.CopyFrom(watermark);
			}
			AssignDrillDownKeys(report, drillDownKeys);
			var exportOptions = report.ExportOptions;
			exportOptions.Html.EmbedImagesInHTML = true;
			if(exportOptions.Image.ExportMode == ImageExportMode.DifferentFiles) {
				exportOptions.Image.ExportMode = ImageExportMode.SingleFilePageByPage;
			}
			if(exportOptions.Xls.ExportMode == XlsExportMode.DifferentFiles) {
				exportOptions.Xls.ExportMode = XlsExportMode.SingleFile;
			}
			if(exportOptions.Xlsx.ExportMode == XlsxExportMode.DifferentFiles) {
				exportOptions.Xlsx.ExportMode = XlsxExportMode.SingleFilePageByPage;
			}
			if(exportOptions.Html.ExportMode == HtmlExportMode.DifferentFiles) {
				exportOptions.Html.ExportMode = HtmlExportMode.SingleFilePageByPage;
			}
			exportOptions.HiddenOptions.Add(ExportOptionKind.HtmlEmbedImagesInHTML);
			exportOptions.HiddenOptions.Add(ExportOptionKind.XlsPageRange);
			report.PrintingSystem.ProgressReflector = new SingleThreadProgressReflector();
		}
		protected virtual WebBackgroundPageBuildEngineStrategy ConfigureBuildStrategy(PrintingSystemBase printingSystem) {
			var buildStrategy = documentBuildFactory.CreatePageBuildStrategy();
			printingSystem.ReplaceService<BackgroundPageBuildEngineStrategy>(buildStrategy);
			return buildStrategy;
		}
		protected virtual void SetFaultState(IDocumentMediator mediator, Exception exception) {
			mediator.ReloadEntity();
			mediator.DocumentFault = new ServiceFault(exception);
			mediator.Entity.Status = TaskStatus.Fault;
		}
		protected void ProcessAsyncPagesRequest(PrintingSystemBase printingSystem, DocumentId documentId, IDuplexEventWaitHandle eventWaitHandle) {
			threadFactoryService.Start(() => pagesRequestProcessor.Process(printingSystem, documentId, eventWaitHandle));
		}
		DocumentBuildResult BuildCore(XtraReport report, IDocumentMediator mediator, ReportConfiguration reportConfiguration, DocumentId documentId) {
			Exception exception = null;
			using(var eventWaitHandle = CreateDuplexEventWaitHandle()) {
				try {
					return ProcessBuild(report, reportConfiguration, documentId, mediator, eventWaitHandle);
				} catch(Exception e) {
					Logger.Error("({0}) Build is completed with exception - {1}", documentId.Value, e);
					exception = e;
					return DocumentBuildResult.None;
				} finally {
					CompleteBuild(documentId, mediator, exception);
					threadFactoryService.Sleep(); 
					eventWaitHandle.Raise();
					if(reportConfiguration.Lifetime.ShouldDispose) {
						report.Dispose();
					}
				}
			}
		}
		IDuplexEventWaitHandle CreateDuplexEventWaitHandle() {
			return ProcessPageRequests
				? (IDuplexEventWaitHandle)new DuplexEventWaitHandle()
				: new FakeDuplexEventWaitHandle();
		}
		void CompleteBuild(DocumentId documentId, IDocumentMediator mediator, Exception exception) {
			lock(documentBuildFactory.SyncRoot) {
				if(exception != null) {
					SetFaultState(mediator, exception);
				} else {
					Logger.Info("({0}) Build is successfully completed", documentId.Value);
					mediator.Entity.Status = TaskStatus.Complete;
				}
				mediator.Save();
			}
		}
		DocumentBuildResult ProcessBuild(XtraReport report, ReportConfiguration reportConfiguration, DocumentId documentId, IDocumentMediator mediator, IDuplexEventWaitHandle eventWaitHandle) {
			Debug.Assert(mediator.Entity.Status == TaskStatus.InProgress, ReportServiceMessages.DocumentIsNotInProgress);
			ConfigureReport(report, reportConfiguration.Watermark, reportConfiguration.PageData, reportConfiguration.DrillDownKeys);
			NotifyInterceptorsBefore(report, reportConfiguration.CustomArgs);
			var printingSystem = report.PrintingSystem;
			var buildEngineSvc = ConfigureBuildStrategy(printingSystem);
			var result = DocumentBuildResult.None;
			EventHandler saveBuildProgress = (s, e) => {
				mediator.PageCount = printingSystem.Document.Pages.Count;
				mediator.Entity.ProgressPosition = printingSystem.ProgressReflector.Position;
				mediator.Save();
			};
			printingSystem.ProgressReflector.PositionChanged += saveBuildProgress;
			try {
				if(ProcessPageRequests) {
					ProcessAsyncPagesRequest(printingSystem, documentId, eventWaitHandle);
				}
				if(printingSystem.Document.PageCount == 0) {
					report.CreateDocument(true);
					RaiseConfigurePrintingSystem(report.PrintingSystem);
					result = ProcessDocumentTicking(report, mediator, buildEngineSvc);
				}
				mediator.PageCount = printingSystem.Document.PageCount;
			} finally {
				printingSystem.ProgressReflector.PositionChanged -= saveBuildProgress;
			}
			if(result != DocumentBuildResult.Cancelled) {
				NotifyInterceptorsAfter(report, reportConfiguration.CustomArgs);
				mediator.SetDocument(printingSystem.Document);
				mediator.SetWatermark(report.Watermark);
				mediator.SetExportOptions(printingSystem.ExportOptions);
				mediator.SetDrillDownKeys(GetDrillDownKeys(report.GetService<IDrillDownService>()));
				mediator.Entity.CanChangePageSettings = report.PrintingSystem.Document.CanChangePageSettings;
				mediator.Save();
			}
			return result;
		}
		void NotifyInterceptorsBefore(XtraReport report, object customArgs) {
			reportBuildInterceptors.ForEach(x => x.InvokeBefore(report, customArgs));
		}
		void NotifyInterceptorsAfter(XtraReport report, object customArgs) {
			reportBuildInterceptors.ForEach(x => x.InvokeAfter(report, customArgs));
		}
		static void AssignDrillDownKeys(XtraReport report, Dictionary<DrillDownKey, bool> clientDrillDownKeys) {
			if(clientDrillDownKeys == null || clientDrillDownKeys.Count == 0) {
				return;
			}
			var drillDownService = report.GetService<IDrillDownService>();
			if(drillDownService == null) {
				return;
			}
			var groupExpanded = drillDownService.Keys;
			foreach(var pair in clientDrillDownKeys) {
				groupExpanded[pair.Key] = pair.Value;
			}
			drillDownService.IsDrillDowning = true;
		}
		static Dictionary<string, bool> GetDrillDownKeys(IDrillDownService drillDownService) {
			if(drillDownService == null) {
				return null;
			}
			var groupExpanded = drillDownService.Keys;
			var result = new Dictionary<string, bool>(groupExpanded.Count);
			foreach(var pair in groupExpanded) {
				result.Add(pair.Key.ToString(), pair.Value);
			}
			return result;
		}
		static void AssignReportPageData(XtraReport report, PageData pageData) {
			if(pageData == null) {
				return;
			}
			report.Margins = XRConvert.ConvertMargins(pageData.Margins, GraphicsDpi.HundredthsOfAnInch, report.Dpi);
			report.PaperKind = pageData.PaperKind;
			if(report.PaperKind == PaperKind.Custom) {
				report.PageSize = XRConvert.Convert(pageData.Size, GraphicsDpi.HundredthsOfAnInch, report.Dpi);
			}
			report.Landscape = pageData.Landscape;
		}
		DocumentBuildResult ProcessDocumentTicking(XtraReport report, IDocumentMediator mediator, WebBackgroundPageBuildEngineStrategy buildEngineSvc) {
			var printingSystem = report.PrintingSystem;
			bool isStopping = false;
			while(printingSystem.Document.IsCreating) {
				if(!ProcessDocumentTickingCore(mediator.DocumentRequestingAction, report, buildEngineSvc, ref isStopping))
					return DocumentBuildResult.Cancelled;
			}
			return DocumentBuildResult.Success;
		}
		protected virtual bool ProcessDocumentTickingCore(RequestingAction documentAction, XtraReport report, WebBackgroundPageBuildEngineStrategy buildEngineSvc, ref bool isStopping) {
			if(documentAction == RequestingAction.Delete) {
				return false;
			}
			if(!isStopping && documentAction == RequestingAction.Stop) {
				report.StopPageBuilding();
				isStopping = true;
			} else {
				buildEngineSvc.DoTick();
			}
			return true;
		}
	}
}
