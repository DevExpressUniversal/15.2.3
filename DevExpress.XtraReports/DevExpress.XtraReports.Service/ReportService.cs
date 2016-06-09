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
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Service.Native;
using DevExpress.XtraReports.Service.Native.ServiceContainer;
using DevExpress.XtraReports.Service.Native.Services;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Service {
	public class ReportService : IReportService, IDelegateOverriddenFunctionalityReportService {
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		static ReportService() {
			XtraReport.EnsureStaticConstructor();
			StaticSetContainer();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void StaticSetContainer() {
			DefaultContainer.SafeSetContainer(XRBootstrapper.CreateInitializedContainer());
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void StaticInitialize() {
		}
		readonly IIntermediateReportService intermediateService;
		public ReportService()
			: this(DefaultContainer.Current) {
		}
		public ReportService(IServiceProvider serviceProvider)
			: this(
			serviceProvider.GetService<IIntermediateReportService>(),
			serviceProvider.GetService<IDALService>(),
			serviceProvider.GetService<ICleanService>()) {
		}
		public ReportService(
			IIntermediateReportService intermediateService,
			IDALService dalService,
			ICleanService cleanService) {
			Guard.ArgumentNotNull(intermediateService, "intermediateService");
			Guard.ArgumentNotNull(dalService, "dalService");
			Guard.ArgumentNotNull(cleanService, "cleanService");
			this.intermediateService = intermediateService;
			dalService.SafeInitialize();
			cleanService.SafeStart();
		}
		#region IReportService
		public virtual ReportParameterContainer GetReportParameters(InstanceIdentity instanceIdentity) {
			return intermediateService.GetReportParameters(instanceIdentity, this);
		}
		public virtual ParameterLookUpValues[] GetLookUpValues(InstanceIdentity identity, ReportParameter[] parameterValues, string[] requiredParameterPaths) {
			return intermediateService.GetLookUpValues(identity, parameterValues, requiredParameterPaths, this);
		}
		public virtual DocumentId StartBuild(InstanceIdentity instanceIdentity, ReportBuildArgs buildArgs) {
			try {
				return intermediateService.StartBuild(instanceIdentity, buildArgs, this);
			} catch(ParameterValidationException e) {
				throw new FaultException(e.Message);
			}
		}
		public virtual void StopBuild(DocumentId documentId) {
			intermediateService.StopBuild(documentId);
		}
		public virtual BuildStatus GetBuildStatus(DocumentId documentId) {
			return intermediateService.GetBuildStatus(documentId);
		}
		public virtual string GetPage(DocumentId documentId, int pageIndex, PageCompatibility compatibility) {
			return intermediateService.GetPage(documentId, pageIndex, compatibility);
		}
		public virtual byte[] GetPages(DocumentId documentId, int[] pageIndexes, PageCompatibility compatibility) {
			return intermediateService.GetPages(documentId, pageIndexes, compatibility);
		}
		public virtual DocumentData GetDocumentData(DocumentId documentId) {
			return intermediateService.GetDocumentData(documentId);
		}
		public virtual PrintId StartPrint(DocumentId documentId, PageCompatibility compatibility) {
			return intermediateService.StartPrint(documentId, compatibility);
		}
		public virtual void StopPrint(PrintId printId) {
			intermediateService.StopPrint(printId);
		}
		public virtual PrintStatus GetPrintStatus(PrintId printId) {
			return intermediateService.GetPrintStatus(printId);
		}
		public virtual Stream GetPrintDocument(PrintId printId) {
			return intermediateService.GetPrintDocument(printId);
		}
		#endregion
		void IDelegateOverriddenFunctionalityReportService.SaveReportLayout(string reportName, byte[] layoutData) {
			SaveReportLayout(reportName, layoutData);
		}
		protected virtual void SaveReportLayout(string reportName, byte[] layoutData) {
			throw new FaultException(ReportServiceMessages.SaveLoadFunctionalityNotImplemented);
		}
		byte[] IDelegateOverriddenFunctionalityReportService.LoadReportLayout(string reportName) {
			return LoadReportLayout(reportName);
		}
		protected virtual byte[] LoadReportLayout(string reportName) {
			return null;
		}
		void IDelegateOverriddenFunctionalityReportService.FillDataSources(XtraReport report, string reportName) {
			FillDataSources(report, reportName);
		}
		protected virtual void FillDataSources(XtraReport report, string reportName) {
		}
		XtraReport IDelegateOverriddenFunctionalityReportService.CreateReportByName(string reportName) {
			return CreateReportByName(reportName);
		}
		protected virtual XtraReport CreateReportByName(string reportName) {
			return null;
		}
		bool IDelegateOverriddenFunctionalityReportService.TryCustomProcessInstanceIdentity(InstanceIdentity identity, out XtraReport result) {
			return TryCustomProcessInstanceIdentity(identity, out result);
		}
		protected virtual bool TryCustomProcessInstanceIdentity(InstanceIdentity identity, out XtraReport result) {
			result = null;
			return false;
		}
		#region ExportService
		public virtual void ClearDocument(DocumentId documentId) {
			intermediateService.ClearDocument(documentId);
		}
		public virtual ExportId StartExport(DocumentId documentId, DocumentExportArgs exportArgs) {
			return intermediateService.StartExport(documentId, exportArgs);
		}
		public virtual ExportStatus GetExportStatus(ExportId exportId) {
			return intermediateService.GetExportStatus(exportId);
		}
		public virtual Stream GetExportedDocument(ExportId exportId) {
			return intermediateService.GetExportedDocument(exportId);
		}
		#endregion
	}
}
