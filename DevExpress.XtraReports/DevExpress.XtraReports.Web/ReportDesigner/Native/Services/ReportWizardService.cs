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

using System;
using System.Collections.Generic;
using DevExpress.XtraPrinting.WebClientUIControl.DataContracts;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.ReportDesigner.DataContracts;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using DevExpress.XtraReports.Wizards.Builder;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class ReportWizardService : IReportWizardService {
		readonly IDataSourceFieldListService dataSourceFieldListService;
		public ReportWizardService(IDataSourceFieldListService dataSourceFieldListService) {
			this.dataSourceFieldListService = dataSourceFieldListService;
		}
		#region IReportWizardService
		public WizardGeneratedReportDesignerModel CreateReportDesignerModel(WizardReportModel reportModel) {
			var model = reportModel.ReportModel;
			using(var report = CreateReportWithDataSourceFromModel(reportModel)) {
				string dataMember = model.DataMemberName != null ? model.DataMemberName.Name : "";
				report.DataMember = dataMember;
				var builder = new WizardReportBuilder(WizardReportBuilder.CreateDefaultComponentFactory, false);
				builder.Build(report, model);
				XtraReportsSerializationContext serializationContext;
				string reportModelJson = ReportLayoutJsonSerializer.GenerateReportLayoutJson(report, out serializationContext);
				DataSourceRefInfo[] dataSourceRefInfo = this.dataSourceFieldListService.GetReportDataSourceRefInfo(report, serializationContext);
				return new WizardGeneratedReportDesignerModel {
					ReportModelJson = reportModelJson,
					DataSourceRefInfo = dataSourceRefInfo
				};
			}
		}
		public virtual WizardGeneratedDataSourceModel CreateDataSourceModel(WizardReportModel model) {
			using(var report = CreateReportWithDataSourceFromModel(model)) {
				var json = DataSourcesJSContentGeneratorLogic.GenerateDataSourceInfoJson(report.DataSource, model.State.SafeGetReportExtensions());
				var dataSourceInfo = new DataSourceInfo {
					Id = Guid.NewGuid().ToString("N"),
					Name = DataSourceFieldListServiceLogic.GetDataSourceDisplayName(report.DataSource),
					Data = json
				};
				return new WizardGeneratedDataSourceModel { DataSource = dataSourceInfo };
			}
		}
		#endregion
		protected virtual XtraReport CreateReportWithDataSourceFromModel(WizardReportModel reportModel) {
			return ReportRestorer.RestoreWithDataSource(reportModel.DataSourceJson, reportModel.State.SafeGetReportExtensions());
		}
	}
}
