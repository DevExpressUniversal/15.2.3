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
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native.Services {
	public class ReportDesignerModelGenerator : IReportDesignerModelGenerator {
		static ReportDesignerModelGenerator() {
			DefaultDataSerializer.SafeInitialize();
		}
		readonly IDataSourceFieldListService dataSourceFieldListService;
		readonly IDataSourcesJSContentGenerator dataSourcesInfoJSContentGenerator;
		readonly IDataSourceWizardConnectionStringsProvider dataSourceWizardConnectionStringsProvider;
		readonly ISqlDataSourceWizardCustomizationService dataSourceWizardCustomizationService;
		public ReportDesignerModelGenerator(IDataSourceFieldListService dataSourceFieldListService, IDataSourcesJSContentGenerator dataSourcesInfoJSContentGenerator, IDataSourceWizardConnectionStringsProvider dataSourceWizardConnectionStringsProvider, ISqlDataSourceWizardCustomizationService dataSourceWizardCustomizationService) {
			this.dataSourceFieldListService = dataSourceFieldListService;
			this.dataSourcesInfoJSContentGenerator = dataSourcesInfoJSContentGenerator;
			this.dataSourceWizardConnectionStringsProvider = dataSourceWizardConnectionStringsProvider;
			this.dataSourceWizardCustomizationService = dataSourceWizardCustomizationService;
		}
		public ReportDesignerModel Generate(XtraReport report, IDictionary<string, object> dataSources, IDictionary<string, string> subreports, IEnumerable<ClientControlsMenuItemModel> menuItems, ReportDesignerModelSettings settings) {
			menuItems = menuItems ?? Enumerable.Empty<ClientControlsMenuItemModel>();
			if(settings.TryAddDefaultDataSerializer) {
				TryAddDefaultDataSerializer(report);
			}
			XtraReportsSerializationContext serializationContext;
			var reportModelJson = ReportLayoutJsonSerializer.GenerateReportLayoutJson(report, out serializationContext);
			var reportDataSources = dataSourceFieldListService.GetReportDataSources(report, serializationContext, dataSources, settings.ShouldShareReportDataSources);
			var dataSourcesJson = dataSourcesInfoJSContentGenerator.Generate(reportDataSources.DataSources, report.Extensions);
			var dataSourceContracts = ContractConverter.ConvertToContracts(dataSourcesJson);
			var dataSourceData = ContractConverter.GetJsonData(dataSourcesJson);
			var menuItemsContracts = ContractConverter.ConvertToContracts(menuItems);
			var menuItemsJsClickActions = ContractConverter.GetMenuItemsJSClickActions(menuItems);
			var knownEnums = EnumInfoCollector.Collect(EnumerateParameterPaths(report));
			var clonedReportExtensions = DictionaryExtensions.Clone(report.Extensions);
			var clonedSubreports = subreports != null ? DictionaryExtensions.Clone(subreports) : new Dictionary<string, string>();
			var wizardConnections = ContractConverter.ConvertDataConnectionsToContracts(dataSourceWizardConnectionStringsProvider.GetConnectionDescriptions());
			return new ReportDesignerModel {
				ReportModelJson = reportModelJson,
				DataSourceRefInfo = reportDataSources.DataSourceRefs,
				DataSources = dataSourceContracts,
				DataSourcesData = dataSourceData,
				Subreports = clonedSubreports,
				MenuActions = menuItemsContracts,
				MenuItemJSClickActions = menuItemsJsClickActions,
				KnownEnums = knownEnums,
				ReportExtensions = clonedReportExtensions,
				WizardConnections = wizardConnections,
				Internals = new ReportDesignerModelInternals(),
				IsCustomSqlDisabled = dataSourceWizardCustomizationService.IsCustomSqlDisabled
			};
		}
		static void TryAddDefaultDataSerializer(XtraReport report) {
			var extensions = report.Extensions;
			if(!extensions.ContainsKey(SerializationService.Guid)) {
				extensions.Add(SerializationService.Guid, DefaultDataSerializer.Name);
			}
		}
		static IEnumerable<ParameterPath> EnumerateParameterPaths(XtraReport report) {
			return new NestedParameterPathCollector().EnumerateParameters(report);
		}
	}
}
