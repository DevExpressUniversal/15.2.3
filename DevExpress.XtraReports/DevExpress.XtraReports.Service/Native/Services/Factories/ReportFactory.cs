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
using System.Linq;
using System.ServiceModel;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Parameters.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Service.Native.Services.Factories {
	public class ReportFactory : IReportFactory, IDisposable {
		static bool IsValidReportType(Type reportType) {
			return reportType != null && typeof(XtraReport).IsAssignableFrom(reportType);
		}
		readonly ITypeResolver typeResolverByTypeName = new SpecificNameTypeResolver();
		readonly DomainTypeResolver typeResolverSearchTypeInDomain = new DomainTypeResolver();
		readonly IDocumentBuildFactory documentBuildFactory;
		public ReportFactory(IDocumentBuildFactory documentBuildFactory) {
			Guard.ArgumentNotNull(documentBuildFactory, "documentBuildFactory");
			this.documentBuildFactory = documentBuildFactory;
		}
		#region IReportFactory
		public XtraReport Create(string reportTypeName) {
			var reportType = ResolveType(reportTypeName);
			return Create(reportType);
		}
		public XtraReport Create(Type reportType) {
			if(!IsValidReportType(reportType)) {
				return null;
			}
			return (XtraReport)Activator.CreateInstance(reportType);
		}
		public ReportParameterContainer GetReportParameters(XtraReport report) {
			IEnumerable<ParameterPath> parameterPaths = RaiseParametersRequestBeforeShowAndGetParameterPaths(report);
			var parameterValueDictionary = parameterPaths.ToDictionary<ParameterPath, IParameter, object>(x => x.Parameter, x => x.Parameter.Value);
			var dataContext = report.GetService<DataContext>();
			ReportParameter[] resultParameters = parameterPaths
				.Select(x => CreateReportParameter(x, parameterValueDictionary, dataContext))
				.ToArray();
			return new ReportParameterContainer {
				ShouldRequestParameters = report.RequestParameters,
				Parameters = resultParameters
			};
		}
		public ParameterLookUpValues[] GetLookUpValues(XtraReport report, string[] requiredParameterPaths) {
			IEnumerable<ParameterPath> parameterPaths = RaiseParametersRequestBeforeShowAndGetParameterPaths(report);
			Dictionary<IParameter, object> parameterValueDictionary = parameterPaths.ToDictionary<ParameterPath, IParameter, object>(x => x.Parameter, x => x.Parameter.Value);
			IEnumerable<ParameterPath> filteredParameterPaths = parameterPaths
				.Where(x => requiredParameterPaths.Contains(x.Path))
				.Where(x => x.Parameter.HasCascadeLookUpSettings());
			var dataContext = report.GetService<DataContext>();
			var result = filteredParameterPaths
				.Select(x => new ParameterLookUpValues {
					Path = x.Path,
					LookUpValues = GetLookUps(x.Parameter, parameterValueDictionary, dataContext)
				})
				.ToArray();
			return result;
		}
		#endregion
		#region IDisposable
		bool disposed;
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposed) {
				return;
			}
			if(disposing) {
				typeResolverSearchTypeInDomain.Dispose();
			}
			disposed = true;
		}
		~ReportFactory() {
			Dispose(false);
		}
		#endregion
		static ReportParameter CreateReportParameter(ParameterPath path, Dictionary<IParameter, object> parameterValueDictionary, DataContext dataContext) {
			var parameter = path.Parameter;
			var lookUpSettings = parameter.LookUpSettings;
			var lookUpValues = GetLookUps(parameter, parameterValueDictionary, dataContext, false);
			var isFiltered = lookUpSettings != null && !string.IsNullOrEmpty(lookUpSettings.FilterString);
			return new ReportParameter(path, lookUpValues, isFiltered);
		}
		static LookUpValueCollection GetLookUps(Parameter parameter, Dictionary<IParameter, object> parameterValueDictionary, DataContext dataContext, bool needAssignValue = true) {
			var valueProvider = new EditorValuesProviderSimple(parameterValueDictionary);
			var valuesCollection = LookUpHelper.GetLookUpValues(parameter.LookUpSettings, dataContext, valueProvider);
			if(needAssignValue && valuesCollection != null && !valuesCollection.Any(x => object.Equals(x.Value, parameter.Value)))
				parameterValueDictionary[parameter] = valuesCollection.Count == 0
					? null
					: valuesCollection[0].Value;
			return valuesCollection;
		}
		protected virtual Type ResolveType(string typeName) {
			var resolver = GetTypeResolver(typeName);
			return resolver.Resolve(typeName);
		}
		ITypeResolver GetTypeResolver(string typeName) {
			return typeName.Contains(",")
				? typeResolverByTypeName
				: typeResolverSearchTypeInDomain;
		}
		static IEnumerable<ParameterPath> RaiseParametersRequestBeforeShowAndGetParameterPaths(XtraReport report) {
			report.ValidateScripts();
			List<ParameterPath> parameterPaths = new NestedParameterPathCollector()
				.EnumerateParameters(report)
				.ToList();
			var parameters = parameterPaths
				.Select(x => x.Parameter)
				.ToList();
			string errorMessage;
			if(!CascadingParametersService.ValidateFilterStrings(parameters, out errorMessage))
				throw new FaultException(errorMessage);
			ParameterInfo[] parameterInfo = parameters
				.Select(ParameterInfoFactory.CreateWithoutEditor)
				.ToArray();
			((IReport)report).RaiseParametersRequestBeforeShow(parameterInfo);
			return parameterPaths.Distinct(ParameterPathEqualityComparer.Instance);
		}
	}
}
