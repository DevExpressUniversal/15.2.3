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
using System.ComponentModel;
using System.Windows;
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.Parameters.Models;
using DevExpress.Xpf.Printing.Parameters.Models.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.XamlExport;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Parameters.Native;
namespace DevExpress.Xpf.Printing {
	public class XtraReportPreviewModel : LegacyLinkPreviewModel {
		#region Fields
		IReport report;
		bool autoShowParametersPanel = true;
		readonly ParametersModel parametersModel;
		#endregion
		#region ctor
		public XtraReportPreviewModel() : this(null) { }
		public XtraReportPreviewModel(IReport report)
			: base(report) {
			this.report = report;
			parametersModel = ParametersModel.CreateParametersModel();
			parametersModel.Submit += OnSubmit;
			PrepareParametersModel();
			WatermarkService = new WatermarkService();
			OnSourceChanged();
		}
		internal XtraReportPreviewModel(IReport report,
										IPageSettingsConfiguratorService pageSettingsConfiguratorService,
										IPrintService printService,
										IExportSendService exportSendService,
										IHighlightingService highlightService,
										IScaleService scaleService,
										IWatermarkService watermarkService)
			: base(report, pageSettingsConfiguratorService, printService, exportSendService, highlightService, scaleService) {
			this.report = report;
			WatermarkService = watermarkService;
			OnSourceChanged();
		}
		#endregion
		#region Properties
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("XtraReportPreviewModelReport")]
#endif
		public IReport Report {
			get {
				return report;
			}
			set {
				if(report == value)
					return;
				OnSourceChanging();
				report = value;
				PrepareParametersModel();
				OnSourceChanged();
				UpdateCurrentPageContent();
			}
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("XtraReportPreviewModelAutoShowParametersPanel")]
#endif
		public bool AutoShowParametersPanel { get { return autoShowParametersPanel; } set { autoShowParametersPanel = value; } }
		public override bool IsEmptyDocument {
			get { return !IsCreating && PageCount == 0 && !parametersModel.IsSubmitted ; }
		}
		protected internal override PrintingSystemBase PrintingSystem {
			get { return report == null ? null : report.PrintingSystemBase; }
		}
		internal IWatermarkService WatermarkService { get; set; }
		public override bool IsSetWatermarkVisible { get { return true; } }
		public override ParametersModel ParametersModel {
			get {
				return parametersModel;
			}
		}
		#endregion
		#region Methods
		void PrepareParametersModel() {
			var models = new List<ParameterModel>();
			if(Report == null) {
				parametersModel.AssignParameters(models);
				return;
			}
			var parameters = CollectReportParameters(Report);
			if(parameters.Count == 0) {
				parametersModel.AssignParameters(models);
				return;
			}
			string validationError;
			if(!CascadingParametersService.ValidateFilterStrings(parameters, out validationError)) {
				parametersModel.AssignParameters(models);
				ShowError(validationError);
				return;
			}
			Report.RaiseParametersRequestBeforeShow(parameters.ConvertAll(x => ParameterInfoFactory.CreateWithoutEditor(x)));
			var dataContext = Report.GetService<DataContext>();
			models.AddRange(ModelsCreator.CreateParameterModels(parameters, dataContext));
			var lookUpsProvider = new LookUpValuesProvider(parameters, dataContext);
			parametersModel.AssignParameters(models);
			parametersModel.LookUpValuesProvider = lookUpsProvider;
			if(AutoShowParametersPanel == true && parametersModel.HasVisibleParameters) {
				IsParametersPanelVisible = true;
				RaisePropertyChanged(() => IsParametersPanelVisible);
			}
		}
		void OnSubmit(object sender, EventArgs e) {
			SubmitParameters(CollectReportParameters(Report));
		}
		protected override FrameworkElement VisualizePage(int pageIndex) {
			var pageVisualizationStrategy = new BrickPageVisualizer(TextMeasurementSystem.GdiPlus);
			var page = (PSPage)PrintingSystem.Pages[pageIndex];
			page.PerformLayout(new PrintingSystemContextWrapper(PrintingSystem, page));
			return pageVisualizationStrategy.Visualize(page, pageIndex, PrintingSystem.Pages.Count);
		}
		protected override void HookPrintingSystem() {
			base.HookPrintingSystem();
			Report.PrintTool = new DevExpress.XtraReports.UI.ReportPrintToolWpf(Report, this);
		}
		protected override void UnhookPrintingSystem() {
			base.UnhookPrintingSystem();
			PrintingSystem.RemoveService(typeof(XpsExportServiceBase));
		}
		protected override void CreateDocument(bool buildPagesInBackground) {
			Report.CreateDocument(buildPagesInBackground);
		}
		protected override bool CanShowParametersPanel(object parameter) {
			if(Report == null || CollectReportParameters(Report).Count == 0)
				return false;
			foreach(Parameter reportParameter in CollectReportParameters(Report)) {
				if(reportParameter.Visible)
					return true;
			}
			return false;
		}
		protected override DataContext GetDataContext() {
			return Report.GetService(typeof(DataContext)) as DataContext;
		}
		static List<Parameter> CollectReportParameters(IReport report) {
			List<Parameter> parameters = new List<Parameter>();
			report.CollectParameters(parameters, (parameter) => true);
			return parameters;
		}
		void SubmitParameters(List<Parameter> parameters) {
			try {
				Report.RaiseParametersRequestSubmit(parameters.ConvertAll((parameter) => new ParameterInfo(parameter, (type) => null)), true);
			} catch(Exception e) {
				if(Report != null)
					Report.PrintingSystemBase.OnCreateDocumentException(new ExceptionEventArgs(e));
			}
		}
		protected override bool CanSetWatermark(object parameter) {
			return BuildPagesComplete && !IsExporting && !IsSaving;
		}
		protected override void SetWatermark(object parameter) {
			WatermarkService.EditCompleted += watermarkService_EditCompleted;
			WatermarkService.Edit(DialogService.GetParentWindow(), PrintingSystem.Pages[CurrentPageIndex], PrintingSystem.PageCount, PrintingSystem.Watermark);
		}
		void watermarkService_EditCompleted(object sender, WatermarkServiceEventArgs e) {
			WatermarkService.EditCompleted -= watermarkService_EditCompleted;
			if(e.IsWatermarkAssigned == true) {
				PrintingSystem.Watermark.CopyFrom((Watermark)e.Watermark);
				report.Watermark.CopyFrom((Watermark)e.Watermark);
				ClearCache();
				UpdateCurrentPageContent();
			}
		}
		#endregion
	}
}
