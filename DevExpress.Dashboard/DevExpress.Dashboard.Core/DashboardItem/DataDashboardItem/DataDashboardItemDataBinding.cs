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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data.Filtering;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.Data;
namespace DevExpress.DashboardCommon.Native {
	public interface ISliceDataQueryProvider {
		SliceDataQuery GetDataQuery(IActualParametersProvider provider);
		MultidimensionalDataDTO PrepareData(DataQueryResult queryResult, SliceDataQuery query, IActualParametersProvider parameters, ColorRepository coloringCache, IDictionary<string, object> filter);
	}
	public class DataServerConfigurationError : Exception {
		public DataServerConfigurationError() : base("SliceDataQuery has invalid configuration (data differs)") { }
	}
}
namespace DevExpress.DashboardCommon {
	public abstract partial class DataDashboardItem : ISliceDataQueryProvider {
		internal IList FakeDataSource { get { return UseFakeData ? FakeDataGeneratorFactory.MakeGenerator(this, DataSource != null ? DataSource.GetDataSourceSchema(DataMember) : null).FakeListSource : null; } }
		internal bool UseFakeData { get { return DataSource != null && DataSource.GetShouldProvideFakeData(); } }
		protected DataSourceModel DataSourceModel {
			get {
				DashboardItemFakeDataBase fakeDataGenerator = UseFakeData ? FakeDataGeneratorFactory.MakeGenerator(this, DataSource.GetDataSourceSchema(DataMember)) : null;
				return new DataSourceModel(
					new DataSourceInfo(DataSource, DataMember),
					fakeDataGenerator != null ? fakeDataGenerator.FakeListSource : DataSource.GetListSource(DataMember),
					DataSource.GetPivotDataSource(DataMember)) {
						FakeData = UseFakeData
					};
			}
		}
		protected bool IsBackCompatibilityDataSlicesRequired { get { return calculateTotals; } }
		protected CriteriaOperator GetQueryFilterCriteria(IActualParametersProvider provider) { return GetFullFilterCriteria(provider); }
		protected IEnumerable<Dimension> QueryFilterDimensions { get { return HiddenDimensions.Concat(ExternalMasterFiltersDimensions); } }
		protected IEnumerable<Measure> QueryMeasures {
			get { return GetQueryVisibleMeasures().Concat(HiddenMeasures).ToList(); }
		}
		protected IEnumerable<DashboardItemFormatRule> QueryFormatRules {
			get {
				return formatRules == null ?
					new DashboardItemFormatRule[0] :
					formatRules.NotNull().Where(rule => rule.IsValid && rule.IsAggregationsRequired);
			}
		}
		protected abstract IEnumerable<Measure> GetQueryVisibleMeasures();
		protected abstract SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider);
		internal string GetDataItemUniqueName(DataItem dataItem) {
			return DataItemRepository.Contains(dataItem) ? DataItemRepository.GetActualID(dataItem) : "";
		}
		protected virtual MultidimensionalDataDTO PrepareDataInternal(DataQueryResult queryResult, SliceDataQuery query, IActualParametersProvider parameters, ColorRepository coloringCache, IDictionary<string, object> filter) {
			foreach(SummaryAggregationResult aggResult in queryResult.SummaryAggregationResults) {
				DashboardItemFormatRule formatRule = QueryFormatRules.Single(rule => rule.Name == aggResult.Name);
				IAggregationInfo aggregateInfo = formatRule.Condition;
				aggregateInfo[aggResult.AggModel.SummaryType] = aggResult.Value;
			}
			PrepareItemDataDTOInternal(queryResult.HierarchicalData, parameters, coloringCache);
			queryResult.HierarchicalData.Storage = CleanDataStorage(filter, queryResult.HierarchicalData.Storage, query, parameters);
			RemoveMeasuresFromDataStorage(queryResult.HierarchicalData.Storage, query, parameters);
			return new MultidimensionalDataDTO {
				HierarchicalDataParams = queryResult.HierarchicalData
			};
		}
		#region ISliceDataQueryProvider implementation
		SliceDataQuery ISliceDataQueryProvider.GetDataQuery(IActualParametersProvider provider) {
			if(DataSource == null)
				return null;
			RequestExternalMasterFilterDimensions();
			SliceDataQuery query = GetDataQueryInternal(provider);
			if(query == null)
				return null;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			SliceDataQueryBuilder queryBuilder = SliceDataQueryBuilder.CreateWithQuery(itemBuilder, QueryFilterDimensions, GetQueryFilterCriteria(provider), query);
			var sliceAggregations = QueryFormatRules.Select(rule => new { level = rule.LevelCore, aggInfo = (IAggregationInfo)rule.Condition, Name = rule.Name }).ToList();
			foreach(var sliceAgg in sliceAggregations) {
				DataItem item = sliceAgg.level.Item;
				Measure measure = item as Measure;
				if(measure != null)
					queryBuilder.AddSliceAggregation(sliceAgg.Name, sliceAgg.level.Axis1Item, sliceAgg.level.Axis2Item, measure, sliceAgg.aggInfo.Types, sliceAgg.aggInfo.Argument);
				else
					queryBuilder.AddSliceAggregation(sliceAgg.Name, (Dimension)item, sliceAgg.aggInfo.Types, sliceAgg.aggInfo.Argument);
			}
			return queryBuilder.FinalQuery();
		}
		MultidimensionalDataDTO ISliceDataQueryProvider.PrepareData(DataQueryResult queryResult, SliceDataQuery query, IActualParametersProvider parameters, ColorRepository coloringCache, IDictionary<string, object> filter) {
			return PrepareDataInternal(queryResult, query, parameters, coloringCache, filter);
		}
		#endregion
	}
}
