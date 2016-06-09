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

using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Web;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class SubreportService : ISubreportService {
		public Dictionary<string, string> GetUrls() {
			if(!ReportStorageWebService.IsRegister) {
				throw new FaultException("ReportStorageWebService is not registered");
			}
			return ReportStorageWebService.GetUrls();
		}
		public OpenSubreportResponse GetData(string reportUrl, IDataSourceFieldListService dataSourceFieldListService, IDataSourcesJSContentGenerator dataSourcesInfoJSContentGenerator) {
			if(!ReportStorageWebService.IsRegister) {
				throw new FaultException("ReportStorageWebService is not registered");
			}
			if(ReportStorageService.IsValidUrl(reportUrl)) {
				using(MemoryStream stream = new MemoryStream(ReportStorageService.GetData(reportUrl))) {
					var report = XtraReport.FromStream(stream, true);
					XtraReportsSerializationContext serializationContext;
					var reportModelJson = ReportLayoutJsonSerializer.GenerateReportLayoutJson(report, out serializationContext);
					IDictionary<string, object> dataSources = new Dictionary<string, object>();
					var reportDataSources = dataSourceFieldListService.GetReportDataSources(report, serializationContext, dataSources, true);
					var dataSourcesJson = dataSourcesInfoJSContentGenerator.Generate(reportDataSources.DataSources, report.Extensions);
					var dataSourceContracts = ContractConverter.ConvertToContracts(dataSourcesJson);
					var dataSourceRefInfo = dataSourceFieldListService.GetReportDataSourceRefInfo(report, serializationContext);
					return new OpenSubreportResponse() {
						DataSourceRefInfo = dataSourceRefInfo,
						DataSources = dataSourceContracts,
						ReportLayout = reportModelJson
					};
				}
			}
			else {
				throw new FaultException("Invalid URL");
			}
		}
		string setData(string reportLayoutJson, string reportUrl, Func<XtraReport, string, string> Func) {
			byte[] reportLayoutXml = ReportLayoutJsonSerializer.LoadFromJsonAndReturnXml(reportLayoutJson);
			using(MemoryStream stream = new MemoryStream(reportLayoutXml)) {
				return Func(XtraReport.FromStream(stream, true), reportUrl);
			}
		}
		public void SetData(string reportLayoutJson, string reportUrl) {
			if(!ReportStorageWebService.IsRegister) {
				throw new FaultException("ReportStorageWebService is not registered");
			}
			this.setData(reportLayoutJson, reportUrl, (x, y) => {
				if(ReportStorageService.IsValidUrl(reportUrl) && ReportStorageService.CanSetData(reportUrl)) {
					ReportStorageService.SetData(x, y);
				}
				else {
					throw new FaultException("Invalid URL");
				}
				return "";
			});
		}
		public string SetNewData(string reportLayoutJson, string reportUrl) {
			if(!ReportStorageWebService.IsRegister) {
				throw new FaultException("ReportStorageWebService is not registered");
			}
			return this.setData(reportLayoutJson, reportUrl, (x, y) => {
				return ReportStorageWebService.SetNewData(x, y);
			});
		}
	}
}
