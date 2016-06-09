#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using DevExpress.XtraPrinting.Native.WebClientUIControl.DataContracts;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class ReportDesignerRequestController : IReportDesignerRequestController {
		readonly IDataSourceFieldListService dataSourceFieldListService;
		readonly IDataSourcesJSContentGenerator dataSourcesJSContentGenerator;
		readonly IXRControlRenderService xrControlRenderService;
		readonly IPreviewReportLayoutService previewReportLayoutService;
		readonly IReportWizardService reportWizardService;
		readonly IReportScriptsService reportScriptsService;
		readonly Func<ISqlDataSourceWizardService> sqlDataSourceWizardServiceFactory;
		readonly ISubreportService subreportService;
		public ReportDesignerRequestController(IDataSourceFieldListService dataSourceFieldListService, IXRControlRenderService xrControlRenderService, IPreviewReportLayoutService previewReportLayoutService, IReportWizardService reportWizardService, IReportScriptsService reportScriptsService, Func<ISqlDataSourceWizardService> sqlDataSourceWizardServiceFactory, IDataSourcesJSContentGenerator dataSourcesJSContentGenerator, ISubreportService subreportService) {
			this.dataSourceFieldListService = dataSourceFieldListService;
			this.dataSourcesJSContentGenerator = dataSourcesJSContentGenerator;
			this.xrControlRenderService = xrControlRenderService;
			this.previewReportLayoutService = previewReportLayoutService;
			this.reportWizardService = reportWizardService;
			this.reportScriptsService = reportScriptsService;
			this.sqlDataSourceWizardServiceFactory = sqlDataSourceWizardServiceFactory;
			this.subreportService = subreportService;
		}
		#region IReportDesignerRequestController
		[WebApiHttpAction("fieldList")]
		public FieldListNode[] GetFieldList(FieldListRequest fieldListRequest) {
			return dataSourceFieldListService.GetListItemProperties(fieldListRequest);
		}
		[WebHttpAction("shapeGlyph", "image/png")]
		public byte[] RenderShape(XRShapeLayout shape) {
			return xrControlRenderService.RenderShape(shape);
		}
		[WebApiHttpAction("chart")]
		public RenderedXRChart RenderChart(XRChartLayout chart) {
			return xrControlRenderService.RenderChartAsBase64(chart);
		}
		[WebApiHttpAction("initializePreview")]
		public ReportToPreview InitializePreview(string previewRequest) {
			return previewReportLayoutService.InitializePreview(previewRequest);
		}
		[WebApiHttpAction("generateReportFromWizardModel")]
		public WizardGeneratedReportDesignerModel CreateReportFromWizardModel(WizardReportModel reportModel) {
			return reportWizardService.CreateReportDesignerModel(reportModel);
		}
		[WebApiHttpAction("createDataSource")]
		public WizardGeneratedDataSourceModel CreateDataSourceModel(WizardReportModel model) {
			return reportWizardService.CreateDataSourceModel(model);
		}
		[WebApiHttpAction("getDBSchema")]
		public DBSchemaModel GetDBSchema(DBSchemaRequest model) {
			return sqlDataSourceWizardServiceFactory().GetDBSchema(model);
		}
		[WebApiHttpAction("createSqlDataSource")]
		public WizardGeneratedDataSourceModel CreateSqlDataSource(WizardSqlDataSourceModel model) {
			return sqlDataSourceWizardServiceFactory().CreateSqlDataSource(model);
		}
		[WebApiHttpAction("getSelectStatement")]
		public SelectStatementModel GetSelectStatement(SelectStatementRequest model) {
			return sqlDataSourceWizardServiceFactory().GetSelectStatement(model);
		}
		[WebApiHttpAction("validateScripts")]
		public ScriptsErrorModel[] GetScriptValidation(XRReportLayout reportLayout) {
			return reportScriptsService.Validate(reportLayout.Report);
		}
		[WebApiHttpAction("getSqlDataSourceStructure")]
		public SqlDataSourceStructure GetSqlDataSourceStructure(DataSourceModel model) {
			return sqlDataSourceWizardServiceFactory().GetSqlDataSourceStructure(model);
		}
		[WebApiHttpAction("renderRich")]
		public RichEditorResponse RenderRich(RichEditorRequest rich) {
			return xrControlRenderService.RenderRich(rich);
		}
		[WebApiHttpAction("getCompleters")]
		public ScriptsCompleteModel[] GetScriptCompleters(ReportScriptsIntellisenseContract model) {
			return reportScriptsService.GetCompleters(model);
		}
		[WebApiHttpAction("getData")]
		public OpenSubreportResponse GetData(OpenSubreportContract openSubreportContract) {
			return subreportService.GetData(openSubreportContract.ReportUrl, this.dataSourceFieldListService, this.dataSourcesJSContentGenerator);
		}
		[WebApiHttpAction("setData")]
		public void SetData(SaveSubreportContract saveSubreportContract) {
			subreportService.SetData(saveSubreportContract.ReportLayout, saveSubreportContract.ReportUrl);
		}
		[WebApiHttpAction("getUrls")]
		public Dictionary<string, string> GetUrls(bool isReady) {
			return subreportService.GetUrls();
		} 
		[WebApiHttpAction("setNewData")]
		public string SetNewData(SaveSubreportContract saveSubreportContract) {
			return subreportService.SetNewData(saveSubreportContract.ReportLayout, saveSubreportContract.ReportUrl);
		}
		#endregion
	}
}
