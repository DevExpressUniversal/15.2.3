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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Browsing.Design;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native.WebClientUIControl.DataContracts;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner.DataContracts;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class DataSourceFieldListService : IDataSourceFieldListService {
		#region IDataSourceFieldListService
		public ReportDataSources GetReportDataSources(XtraReport report, XtraReportsSerializationContext serializationContext, IDictionary<string, object> dataSources, bool shouldShareReportDataSources) {
			dataSources = dataSources ?? new Dictionary<string, object>();
			KeyValuePair<DataSourceRefInfo, object>[] allDataSourceRefInfo = GetAllReportDataSourceRefInfo(report, serializationContext);
			if(shouldShareReportDataSources) {
				dataSources = MergeDataSources(dataSources, allDataSourceRefInfo);
			}
			Dictionary<string, DataSourceRefInfo[]> dataSourceRefs = new Dictionary<string, DataSourceRefInfo[]>();
			dataSourceRefs.Add(report.Name, GetReportDataSourceRefInfo(report, serializationContext));
			GetSubreportDataSources(report, dataSourceRefs, serializationContext);
			return new ReportDataSources(dataSourceRefs, dataSources);
		}
		public FieldListNode[] GetListItemProperties(FieldListRequest fieldListRequest) {
			Guard.ArgumentNotNull(fieldListRequest, "fieldListRequest");
			Guard.ArgumentIsNotNullOrEmpty(fieldListRequest.DataSourceJson, "fieldListRequest.DataSourceJson");
			using(var report = ReportRestorer.RestoreWithDataSource(fieldListRequest.DataSourceJson, fieldListRequest.State.SafeGetReportExtensions())) {
				return GetListItemProperties(report.DataSource, fieldListRequest.DataMember);
			}
		}
		public DataSourceRefInfo[] GetReportDataSourceRefInfo(XtraReport report, XtraReportsSerializationContext serializationContext) {
			object[] reportDataSources = CollectDataSources(report, false);
			return ConvertDataSourceToRefInfo(reportDataSources, serializationContext).Select(x => x.Key).ToArray();
		}
		#endregion
		static KeyValuePair<DataSourceRefInfo, object>[] GetAllReportDataSourceRefInfo(XtraReport report, XtraReportsSerializationContext serializationContext) {
			object[] reportDataSources = CollectDataSources(report, true);
			return ConvertDataSourceToRefInfo(reportDataSources, serializationContext);
		}
		static KeyValuePair<DataSourceRefInfo, object>[] ConvertDataSourceToRefInfo(object[] reportDataSources, XtraReportsSerializationContext serializationContext) {
			var result = new KeyValuePair<DataSourceRefInfo, object>[reportDataSources.Length];
			for(int i = 0; i < reportDataSources.Length; i++) {
				object dataSource = reportDataSources[i];
				var referenceId = serializationContext.GetReference(dataSource);
				var dataSourceRefInfo = new DataSourceRefInfo {
					Ref = referenceId,
					Name = DataSourceFieldListServiceLogic.GetDataSourceDisplayName(dataSource),
					IsSqlDataSource = dataSource is DevExpress.DataAccess.Sql.SqlDataSource
				};
				result[i] = new KeyValuePair<DataSourceRefInfo, object>(dataSourceRefInfo, dataSource);
			}
			return result;
		}
		string GetNewKey(XtraReport report, Dictionary<XtraReport, string> subreportSources) {
			return report.MasterReport != null ? (GetNewKey(report.MasterReport, subreportSources) + "." + subreportSources[report]) : report.Name;
		}
		void GetSubreports(XRControl control, Dictionary<XtraReport, string> subreportSources) {
			IEnumerable<XRSubreport> subreportCollection =  control.AllControls<XRSubreport>();
			foreach(XRSubreport subreport in subreportCollection) {
				if(subreport.ReportSource != null && string.IsNullOrEmpty(subreport.ReportSourceUrl)) {
					subreportSources.Add(subreport.ReportSource, subreport.Name);
					GetSubreports(subreport.ReportSource, subreportSources);
				}
			}
		}
		void GetSubreportDataSources(XtraReport mainReport, Dictionary<string, DataSourceRefInfo[]> reportDataSources, XtraReportsSerializationContext serializationContext) {
			Dictionary<XtraReport, string> subreportSources = new Dictionary<XtraReport, string>();
			GetSubreports(mainReport, subreportSources);
			foreach(KeyValuePair<XtraReport, string> pair in subreportSources) {
				var dataSourceRefResult = GetReportDataSourceRefInfo(pair.Key, serializationContext);
				reportDataSources.Add(GetNewKey(pair.Key, subreportSources), dataSourceRefResult);
			}
		}
		static FieldListNode[] GetListItemProperties(object dataSource, string dataMember) {
			GetPropertiesEventArgs args = null;
			DataSourceFieldListServiceLogic.DoDataContextAction(x => x.GetListItemProperties(dataSource, dataMember, (_, e) => args = e));
			return args.Properties
				.Select(x => new FieldListNode {
					Name = x.Name,
					DisplayName = x.DisplayName,
					IsList = x.IsListType || x.IsComplex,
					Specifics = x.Specifics.ToString()
				})
				.ToArray();
		}
		static object[] CollectDataSources(XtraReport report, bool includeSubreports) {
			return new UniqueDataSourceEnumerator()
				.EnumerateDataSources(report, includeSubreports)
				.Where(x => !(x is ParametersDataSource) && !(x is XRPivotGrid))
				.ToArray();
		}
		static IDictionary<string, object> MergeDataSources(IDictionary<string, object> dataSources, KeyValuePair<DataSourceRefInfo, object>[] dataSourceRefInfo) {
			var result = dataSources.Clone();
			foreach(var dataSourceRefInfoItem in dataSourceRefInfo) {
				if(!result.ContainsKey(dataSourceRefInfoItem.Key.Name)) {
					result.Add(dataSourceRefInfoItem.Key.Name, dataSourceRefInfoItem.Value);
				}
			}
			return result;
		}
	}
}
