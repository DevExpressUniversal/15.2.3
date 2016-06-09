#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardColorSchemeProvider : ColorShemeProvider, IColorSchemeProvider {
		#region static
		static IList<DataDashboardItem> GetGloballyColoredItems(IEnumerable<DimensionDefinition> definitions, IList<DataDashboardItem> dashboardItems) {
			if (definitions.Count() == 0)
				return new List<DataDashboardItem>();
			return dashboardItems
				.Where(dashboardItem => dashboardItem.IsGloballyColored)
				.Where(dashboardItem => new UnorderedArrayComparer().Equals(definitions.ToArray(), dashboardItem.GetColoringDimensionDefinitions().ToArray()))
				.ToList();
		}
		static IEnumerable<Measure> GetMeasures(IList<DataDashboardItem> dashboardItems) {
			MeasureDefinition[] measureDefinitions = dashboardItems
				.SelectMany(dashboardItem => dashboardItem.Measures)
				.Select(measure => measure.GetMeasureDefinition())
				.Distinct()
				.ToArray();
			return measureDefinitions.Select(measure => new Measure(measure.DataMember, measure.SummaryType));
		}
		static void AddOthersValues(HashSet<object[]> res, IList<DimensionDefinition> dimensionDefinitions, IList<DataDashboardItem> dashboardItems) {
			IList<int> othersIndexes = GetOthersIndexes(dimensionDefinitions, dashboardItems);
			foreach (int index in othersIndexes)
				AddValues(res, DashboardSpecialValues.OthersValue, index);
		}
		static IList<int> GetOthersIndexes(IList<DimensionDefinition> dimensionDefinitions, IList<DataDashboardItem> dashboardItems) {
			IList<int> othersIndexes = new List<int>();
			for (int i = 0; i < dimensionDefinitions.Count; i++) {
				foreach (Dimension dimension in dashboardItems.SelectMany(item => item.Dimensions).ToList()) {
					if (dimension.ColorByHue && dimension.TopNOptions.ActualShowOthers && dimensionDefinitions[i].Equals(dimension.GetDimensionDefinition())) {
						othersIndexes.Add(i);
						break;
					}
				}
			}
			return othersIndexes;
		}
		static void AddOlapNullValues(HashSet<object[]> res, IList<DimensionDefinition> dimensionDefinitions, IEnumerable<DashboardItem> items) {
			if (res.Count > 0 && dimensionDefinitions.Count > 0) {
				DimensionDefinition firstDefinition = dimensionDefinitions.First();
				if (OlapDefinitionWithRaggedNulls(firstDefinition, items))
					AddValues(res, DashboardSpecialValues.OlapNullValue, 0);
				DimensionDefinition lastDefinition = dimensionDefinitions.Last();
				if (firstDefinition != lastDefinition && OlapDefinitionWithRaggedNulls(lastDefinition, items))
					AddValues(res, DashboardSpecialValues.OlapNullValue, res.First().Length - 1);
			}
		}
		static bool OlapDefinitionWithRaggedNulls(DimensionDefinition definition, IEnumerable<DashboardItem> items) {
			return items
				.OfType<DataDashboardItem>()
				.Where(dashboardItem => dashboardItem.IsGloballyColored)
				.Where(dashboardItem => dashboardItem.LastSingleColoredDefinition(definition))
				.Count() > 0;
		}
		static void AddValues(HashSet<object[]> res, object value, int index) {
			HashSet<object[]> addList = new HashSet<object[]>(Helper.EnumerableObjectComparer);
			foreach (object[] row in res) {
				object[] copy = (object[])row.Clone();
				copy[index] = value;
				addList.Add(copy);
			}
			foreach (object[] row in addList)
				res.Add(row);
		}
		#endregion
		readonly IEnumerable<DashboardItem> items;
		public DashboardColorSchemeProvider(IEnumerable<DashboardItem> items, IDataSessionProvider dataSessionProvider, IDataSourceInfoProvider dataInfoProvider)
			: base(dataSessionProvider, dataInfoProvider) {
			Guard.ArgumentNotNull(items, "items");
			this.items = items;
		}
		IList<object[]> GetColoringValuesCore(DataSourceInfo dataSourceInfo, IEnumerable<DimensionDefinition> definitions, IEnumerable<Dimension> filterDimensions, CriteriaOperator filter, IList<DataDashboardItem> dashboardItems, IEnumerable<DashboardItem> items, IActualParametersProvider parameters) {
			IList<DimensionDefinition> dimensionDefinitions = definitions.Where(d => !d.Equals(DimensionDefinition.MeasureNamesDefinition)).ToList();
			HashSet<object[]> res = new HashSet<object[]>(Helper.EnumerableObjectComparer);
			if (dataSourceInfo.GetIsConnected() && dimensionDefinitions.Count > 0) { 
				IDataSession session = DataSessionProvider.GetDataSession(null, dataSourceInfo, null);
				int counter = 0;
				Dictionary<DataItem, string> ids = new Dictionary<DataItem, string>();
				ItemModelBuilder builder = new ItemModelBuilder(dataSourceInfo, (a) => {
					string name;
					if (ids.TryGetValue(a, out name))
						return name;
					do
						name = "Dim" + counter++;
					while (definitions.Any(d => d.DataMember == name) || ids.ContainsValue(name));
					ids[a] = name;
					return name;
				}, parameters);
				SliceDataQueryBuilder queryBuilder = SliceDataQueryBuilder.CreateEmpty(builder, filterDimensions, filter);
				IEnumerable<Dimension> dims = dimensionDefinitions.Select((d) => new Dimension(d.DataMember) {
					TextGroupInterval = d.TextGroupInterval,
					DateTimeGroupInterval = d.DateTimeGroupInterval
				}).ToList();
				queryBuilder.AddSlice(dims, dataSourceInfo.GetIsOlap() ? GetMeasures(dashboardItems) : new Measure[0]);
				queryBuilder.SetAxes(dims, new Dimension[0]);
				DataStorage storage = session.GetData(queryBuilder.FinalQuery()).HierarchicalData.Storage;
				List<StorageColumn> columns = dims.Select((d) => storage.GetColumn(ids[d])).ToList();
				if (storage.Count() != 0) {
					foreach (StorageRow storageRow in storage.First()) {
						object[] row = new object[columns.Count];
						for (int j = 0; j < columns.Count; j++)
							row[j] = storageRow[columns[j]].MaterializedValue;
						res.Add(row);
					}
					AddOthersValues(res, dimensionDefinitions, dashboardItems);
					if (dataSourceInfo.GetIsOlap())
						AddOlapNullValues(res, dimensionDefinitions, items);
				}
			}
			return res.ToList();
		}
		#region IColorSchemeProvider
		ColoringSchemeDefinition IColorSchemeProvider.GetColoringScheme() {
			ColoringSchemeDefinition schemes = new ColoringSchemeDefinition();
			foreach (DataDashboardItem dashboardItem in items.OfType<DataDashboardItem>()) {
				if (dashboardItem.IsGloballyColored) {
					ColoringSchemeDefinition itemScheme = dashboardItem.GetColoringScheme();
					foreach (KeyValuePair<ColorRepositoryKey, MeasureDefinition[][]> itemSchemeRow in itemScheme) {
						MeasureDefinition[][] itemMeasures = itemSchemeRow.Value;
						MeasureDefinition[][] existingMeasures;
						if (schemes.TryGetValue(itemSchemeRow.Key, out existingMeasures)) {
							schemes.Remove(itemSchemeRow.Key);
							itemMeasures = existingMeasures.Union<MeasureDefinition[]>(itemMeasures, new UnorderedArrayComparer()).ToArray();
						}
						schemes[itemSchemeRow.Key] = itemMeasures;
					}
				}
			}
			return schemes;
		}
		IList<object[]> IColorSchemeProvider.GetColoringValues(ColorRepositoryKey repositoryKey, IActualParametersProvider parameters) {
			if (repositoryKey.IsDataSourceSpecified) {
				DataSourceInfo dataInfo = DataInfoProvider.GetDataSourceInfo(repositoryKey.DataSourceName, repositoryKey.DataMember);
				if (dataInfo != null) {
					IList<DimensionDefinition> dimensionDefinitions = repositoryKey.DimensionDefinitions.Where(d => !d.Equals(DimensionDefinition.MeasureNamesDefinition)).ToList();
					IList<DataDashboardItem> dashboardItems = GetGloballyColoredItems(dimensionDefinitions, items.OfType<DataDashboardItem>().ToList());
					if (!dataInfo.DataSource.GetShouldProvideFakeData()) {
						return GetColoringValuesCore(dataInfo, repositoryKey.DimensionDefinitions, new Dimension[0], null, dashboardItems, items, parameters);
					}
					else {
						HashSet<object[]> res = new HashSet<object[]>(Helper.EnumerableObjectComparer);
						foreach (DataDashboardItem dashboardItem in dashboardItems) {
							IList<object[]> values = GetDashboardItemColoringValues(dashboardItem, dimensionDefinitions);
							foreach (object[] value in values)
								res.Add(value);
						}
						return res.ToList();
					}
				}
			}
			return new object[0][];
		}
		#endregion
	}
}
