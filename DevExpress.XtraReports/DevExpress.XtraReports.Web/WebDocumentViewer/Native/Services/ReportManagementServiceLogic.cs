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
using System.Drawing;
using System.Linq;
using DevExpress.Data.Browsing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.DrillDown;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Parameters.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services {
	public static class ReportManagementServiceLogic {
		public static ReportToPreview GetReportToPreview(string reportId, XtraReport report, ReportParametersInfo parametersInfo) {
			Size sizeInPixels = XRConvert.Convert(report.PageSize, report.ReportUnit.ToDpi(), GraphicsDpi.Pixel);
			ExportOptions fixedExportOptions = ExportingStrategy.GetFixedExportOptions(report.ExportOptions);
			string exportOptionsJson = ReportLayoutJsonSerializer.GetExportOptionsJson(fixedExportOptions);
			return new ReportToPreview {
				ReportId = reportId,
				PageHeight = sizeInPixels.Height,
				PageWidth = sizeInPixels.Width,
				ExportOptions = exportOptionsJson,
				ParametersInfo = parametersInfo
			};
		}
		public static bool IsNotDisposed(XtraReport report) {
			return !report.IsDisposed && !report.PrintingSystem.IsDisposed;
		}
		public static ReportParametersInfo GetParametersInfo(XtraReport report) {
			report.ValidateScripts();
			var parameterPaths = new NestedParameterPathCollector().EnumerateParameters(report);
			var parameterInfos = parameterPaths
				.Distinct(ParameterPathEqualityComparer.Instance)
				.Select(x => x.Parameter)
				.Select(ParameterInfoFactory.CreateWithoutEditor)
				.ToArray();
			((IReport)report).RaiseParametersRequestBeforeShow(parameterInfos);
			var dataContext = report.GetService<DataContext>();
			var resultParameters = parameterPaths
				.Distinct(ParameterPathEqualityComparer.Instance)
				.Select(x => ContractConverter.GetTypedReportParameter(x, dataContext))
				.ToArray();
			var knownEnums = EnumInfoCollector.Collect(parameterPaths);
			Type[] reportParametersEnumTypes = parameterPaths
				.Where(x => x.Parameter.Type.IsEnum)
				.Select(x => GetParameterType(x.Parameter))
				.ToArray();
			return new ReportParametersInfo(reportParametersEnumTypes) {
				ShouldRequestParameters = report.RequestParameters,
				Parameters = resultParameters,
				KnownEnums = knownEnums
			};
		}
		static Type GetParameterType(Parameter parameter) {
			return parameter.MultiValue
				? parameter.Type.MakeArrayType()
				: parameter.Type;
		}
		public static LookUpValuesResponse GetLookUpValues(XtraReport report, Dictionary<string, object> parameters, string[] requiredParameterPaths) {
			AssignClientParametersToReport(report, parameters);
			Dictionary<string, Parameter> reportParametersDictionary = new NestedParameterPathCollector().GetParametersAsDictionary(report);
			var dependentParameters = reportParametersDictionary
				.Where(x => requiredParameterPaths.Contains(x.Key));
			var parametersDictionary = new Dictionary<string, object>();
			var normalizer = new WebClientParameterValueNormalizer();
			foreach(var parameter in parameters) {
				Parameter matchedParameter;
				if(!reportParametersDictionary.TryGetValue(parameter.Key, out matchedParameter)) {
					throw new ArgumentException("Parameter '{0}' not found", parameter.Key);
				}
				object value;
				if(!dependentParameters.Any(x => x.Key == parameter.Key)) {
					value = normalizer.Normalize(parameter.Value, matchedParameter.Type, matchedParameter.MultiValue);
				} else {
					value = matchedParameter.Value;
				}
				parametersDictionary.Add(parameter.Key, value);
			}
			var dataContext = report.GetService<DataContext>();
			var result = dependentParameters.ToDictionary(
				x => x.Value.Name,
				x => GetParameterLookUpValues(x.Value, parametersDictionary, dataContext));
			return new LookUpValuesResponse { Parameters = result };
		}
		static LookUpValueCollection GetParameterLookUpValues(Parameter parameter, Dictionary<string, object> parameterValueDictionary, DataContext dataContext) {
			var parameterValueProvider = new WebDocumentViewerParameterValueProvider(parameterValueDictionary);
			var lookUps = LookUpHelper.GetLookUpValues(parameter.LookUpSettings, dataContext, parameterValueProvider);
			foreach(var lookUpValue in lookUps)
				lookUpValue.Value = ContractConverter.ToJsParameterValue(lookUpValue.Value);
			if(lookUps != null && !lookUps.Any(x => object.Equals(x.Value, parameter.Value)))
				parameterValueDictionary[parameter.Name] = lookUps.Count == 0 ? null : lookUps[0].Value;
			return lookUps;
		}
		static void AssignClientParametersToReport(XtraReport report, Dictionary<string, object> parameters) {
			var normalizer = new WebClientParameterValueNormalizer();
			var reportParametersDictionary = new NestedParameterPathCollector().GetParametersAsDictionary(report);
			foreach(var parameter in parameters) {
				Parameter matchedParameter;
				if(!reportParametersDictionary.TryGetValue(parameter.Key, out matchedParameter)) {
					throw new ArgumentException(string.Format("Parameter '{0}' not found", parameter.Key), parameter.Key);
				}
				matchedParameter.Value = normalizer.Normalize(parameter.Value, matchedParameter.Type, matchedParameter.MultiValue);
			}
		}
		public static StartBuildResponse StartBuild(IDocumentBuilder buildManager, XtraReport report, Dictionary<string, object> parameters, Dictionary<string, bool> drillDownState) {
			if(parameters != null) {
				AssignClientParametersToReport(report, parameters);
			}
			string documentId = buildManager.StartBuild(report, drillDownState);
			return new StartBuildResponse(documentId);
		}
		public static void UpdateDrillDownState(IDrillDownService drillDownService, Dictionary<string, bool> state) {
			if(drillDownService == null || state == null || state.Count == 0) {
				return;
			}
			foreach(var key in state) {
				var ddk = DrillDownKey.Parse(key.Key);
				drillDownService.Keys[ddk] = key.Value;
			}
			drillDownService.IsDrillDowning = true;
		}
	}
}
