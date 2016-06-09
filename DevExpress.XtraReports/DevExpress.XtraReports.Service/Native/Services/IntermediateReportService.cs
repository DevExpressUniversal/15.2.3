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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraReports.Service.Native.Services.Domain;
using DevExpress.XtraReports.Service.Native.Services.Factories;
using DevExpress.XtraReports.Service.Native.Services.Transient;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Service.Native.Services {
	public class IntermediateReportService : IIntermediateReportService {
		readonly ISerializationService serializationService;
		readonly IDocumentBuildService documentBuildService;
		readonly IDocumentPrintService documentPrintService;
		readonly IPrintingSystemConfigurationService printingSystemConfigurationService;
		readonly IDocumentBuildFactory documentBuildFactory;
		readonly IReportFactory reportFactory;
		readonly IExtensionsFactory extensionsFactory;
		readonly IResourceProviderService resourceProvider;
		readonly IDocumentStoreService documentStoreService;
		readonly IDocumentExportService documentExportService;
		public IntermediateReportService(
			ISerializationService serializationService,
			IDocumentBuildService documentBuildService,
			IDocumentPrintService documentPrintService,
			IPrintingSystemConfigurationService printingSystemConfigurationService,
			IDocumentBuildFactory documentBuildFactory,
			IReportFactory reportFactory,
			IExtensionsFactory extensionsFactory,
			IResourceProviderService resourceProvider,
			IDocumentStoreService documentStoreService,
			IDocumentExportService documentExportService) {
			Guard.ArgumentNotNull(serializationService, "serializationService");
			Guard.ArgumentNotNull(documentBuildService, "documentBuildService");
			Guard.ArgumentNotNull(documentPrintService, "documentPrintService");
			Guard.ArgumentNotNull(printingSystemConfigurationService, "printingSystemConfigurationService");
			Guard.ArgumentNotNull(documentBuildFactory, "documentBuildFactory");
			Guard.ArgumentNotNull(reportFactory, "reportFactory");
			Guard.ArgumentNotNull(extensionsFactory, "extensionsFactory");
			Guard.ArgumentNotNull(resourceProvider, "resourceProvider");
			Guard.ArgumentNotNull(documentStoreService, "documentStoreService");
			Guard.ArgumentNotNull(documentExportService, "documentExportService");
			this.serializationService = serializationService;
			this.documentBuildService = documentBuildService;
			this.documentPrintService = documentPrintService;
			this.printingSystemConfigurationService = printingSystemConfigurationService;
			this.documentBuildFactory = documentBuildFactory;
			this.reportFactory = reportFactory;
			this.extensionsFactory = extensionsFactory;
			this.resourceProvider = resourceProvider;
			this.documentStoreService = documentStoreService;
			this.documentExportService = documentExportService;
		}
		#region IIntermediateReportService
		public ReportParameterContainer GetReportParameters(InstanceIdentity instanceIdentity, IDelegateOverriddenFunctionalityReportService functionality) {
			var configuredReport = CreateReport(instanceIdentity, null, ReportCreationReason.GetReportParameters, functionality);
			return GetReportParameters(configuredReport);
		}
		public ParameterLookUpValues[] GetLookUpValues(InstanceIdentity instanceIdentity, ReportParameter[] parameterValues, string[] requiredParameterPaths, IDelegateOverriddenFunctionalityReportService functionality) {
			var configuredReport = CreateReport(instanceIdentity, parameterValues, ReportCreationReason.GetLookUpValues, functionality);
			return GetLookUpValues(configuredReport, requiredParameterPaths);
		}
		public virtual DocumentId StartBuild(InstanceIdentity instanceIdentity, ReportBuildArgs buildArgs, IDelegateOverriddenFunctionalityReportService functionality) {
			var configuredReport = CreateReport(instanceIdentity, buildArgs.Parameters, ReportCreationReason.StartBuild, functionality);
			var configuration = new ReportConfiguration(
				configuredReport.Lifetime,
				serializationService.DeserializePageData(buildArgs.SerializedPageData),
				serializationService.DeserializeWatermark(buildArgs.SerializedWatermark),
				RestoreDrillDownKeys(buildArgs.DrillDownKeys),
				buildArgs.CustomArgs);
			return documentBuildService.StartBuild(configuredReport.Report, configuration);
		}
		public void StopBuild(DocumentId documentId) {
			documentBuildService.StopBuild(documentId);
		}
		public BuildStatus GetBuildStatus(DocumentId documentId) {
			return documentBuildService.GetBuildStatus(documentId);
		}
		public string GetPage(DocumentId documentId, int pageIndex, PageCompatibility compatibility) {
			return documentBuildService.GetPage(documentId, pageIndex, compatibility, null);
		}
		public byte[] GetPages(DocumentId documentId, int[] pageIndexes, PageCompatibility compatibility) {
			return documentBuildService.GetPages(documentId, pageIndexes, compatibility, null);
		}
		public DocumentData GetDocumentData(DocumentId documentId) {
			return documentBuildService.GetDocumentData(documentId);
		}
		public virtual PrintId StartPrint(DocumentId documentId, PageCompatibility compatibility) {
			return documentPrintService.StartPrint(documentId, compatibility);
		}
		public void StopPrint(PrintId printId) {
			documentPrintService.StopPrint(printId);
		}
		public PrintStatus GetPrintStatus(PrintId printId) {
			return documentPrintService.GetPrintStatus(printId);
		}
		public Stream GetPrintDocument(PrintId printId) {
			return documentPrintService.GetPrintDocument(printId);
		}
		#endregion
		ReportParameterContainer GetReportParameters(ConfiguredReport configuredReport) {
			var report = configuredReport.Report;
			try {
				return reportFactory.GetReportParameters(report);
			} finally {
				if(configuredReport.Lifetime.ShouldDispose) {
					report.Dispose();
				}
			}
		}
		ConfiguredReport CreateReport(InstanceIdentity instanceIdentity, ReportParameter[] parameters, ReportCreationReason reason, IDelegateOverriddenFunctionalityReportService functionality) {
			var instanceName = instanceIdentity.ToString(); 
			XtraReport report;
			ConfiguredReport result = TryCustomProcessInstanceIdentity(instanceIdentity, functionality, out report)
				? new ConfiguredReport(report)
				: CreateReport_Regular(instanceName, parameters, reason, functionality);
			return result;
		}
		bool TryCustomProcessInstanceIdentity(InstanceIdentity instanceIdentity, IDelegateOverriddenFunctionalityReportService functionality, out XtraReport report) {
			report = extensionsFactory.GetInstanceIdentityResolvers()
				.Select(r => r.Resolve(instanceIdentity))
				.FirstOrDefault(r => r != null);
			if(report != null) {
				return true;
			}
			return functionality.TryCustomProcessInstanceIdentity(instanceIdentity, out report);
		}
		ConfiguredReport CreateReport_Regular(string reportName, ReportParameter[] parameters, ReportCreationReason reason, IDelegateOverriddenFunctionalityReportService functionality) {
			var getParameters = reason == ReportCreationReason.GetReportParameters;
			var report = CreateReport_RegularCore(reportName, getParameters, functionality);
			if(reason == ReportCreationReason.GetReportParameters || reason == ReportCreationReason.GetLookUpValues) {
				printingSystemConfigurationService.EnableDocumentBuildImageUrlResolving(report.PrintingSystem);
			}
			var layoutData = functionality.LoadReportLayout(reportName);
			if(layoutData != null) {
				using(var stream = new MemoryStream(layoutData)) {
					report.LoadLayout(stream);
				}
			}
			if(reason != ReportCreationReason.GetReportParameters) {
				ReportServiceHelper.AssignParameters(report, parameters);
				FillDataSourcesInternal(report, reportName, reason, functionality);
			}
			var lifetime = new ReportLifetimeConfiguration(shouldDispose: true);
			return new ConfiguredReport(report, lifetime);
		}
		void FillDataSourcesInternal(XtraReport report, string reportName, ReportCreationReason reason, IDelegateOverriddenFunctionalityReportService functionality) {
			foreach(var service in extensionsFactory.GetDataSourceServices()) {
				service.FillDataSources(report, reportName);
			}
			functionality.FillDataSources(report, reportName);
		}
		XtraReport CreateReport_RegularCore(string reportName, bool getParameters, IDelegateOverriddenFunctionalityReportService functionality) {
			var report = CreateReportByName(reportName, getParameters, functionality);
			if(report == null) {
				throw new FaultException(ReportServiceMessages.NoReport);
			}
			return report;
		}
		XtraReport CreateReportByName(string reportName, bool getParameters, IDelegateOverriddenFunctionalityReportService functionality) {
			var report = extensionsFactory.GetReportResolvers()
					.Select(r => r.Resolve(reportName, getParameters))
					.FirstOrDefault(r => r != null)
				?? functionality.CreateReportByName(reportName)
				?? reportFactory.Create(reportName);
			return report;
		}
		ParameterLookUpValues[] GetLookUpValues(ConfiguredReport configuredReport, string[] requiredParameterPaths) {
			var report = configuredReport.Report;
			try {
				return reportFactory.GetLookUpValues(configuredReport.Report, requiredParameterPaths);
			} finally {
				if(configuredReport.Lifetime.ShouldDispose) {
					report.Dispose();
				}
			}
		}
		static Dictionary<DrillDownKey, bool> RestoreDrillDownKeys(Dictionary<string, bool> drillDownKeys) {
			if(drillDownKeys == null) {
				return new Dictionary<DrillDownKey, bool>();
			}
			var result = new Dictionary<DrillDownKey, bool>(drillDownKeys.Count);
			foreach(var pair in drillDownKeys) {
				var key = DrillDownKey.Parse(pair.Key);
				result.Add(key, pair.Value);
			}
			return result;
		}
		#region ExportService
		public void ClearDocument(DocumentId documentId) {
			documentStoreService.ClearDocument(documentId);
		}
		public virtual ExportId StartExport(DocumentId documentId, DocumentExportArgs exportArgs) {
			var exportOptions = serializationService.Deserialize(exportArgs);
			var configuration = new DocumentExportConfiguration(exportOptions, exportArgs.CustomArgs);
			return documentExportService.StartExport(documentId, configuration);
		}
		public ExportStatus GetExportStatus(ExportId exportId) {
			return documentExportService.GetExportStatus(exportId);
		}
		public Stream GetExportedDocument(ExportId exportId) {
			return documentExportService.GetExportedDocument(exportId);
		}
		#endregion
	}
}
