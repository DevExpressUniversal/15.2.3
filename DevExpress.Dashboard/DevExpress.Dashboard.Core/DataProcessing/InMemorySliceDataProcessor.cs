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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class InMemorySliceDataProcessor {
		readonly DataSourceModel dataSource;
		public InMemorySliceDataProcessor(DataSourceModel dataSource) {
			this.dataSource = dataSource;
		}
		public DataQueryResult GetData(SliceDataQuery query, CancellationToken cancellationToken) {
			if(dataSource.Storage == null)
				return DataQueryResult.Empty;
			HashSet<string> axis1Dims = new HashSet<string>(query.Axis1.Select(d=>d.Name));
			HashSet<string> axis2Dims = new HashSet<string>(query.Axis2.Select(d=>d.Name));
			string[][] sortOrderSlices =
				query.DataSlices.Select(s => s.Dimensions.Select(d => d.Name).ToArray()).Where(s => axis1Dims.ContainsAny(s) ^ axis2Dims.ContainsAny(s)).Select(s => s.ToArray()).ToArray();
			Tuple<DataStorage, List<SummaryAggregationResult>> result = Process(query, cancellationToken);
			return new DataQueryResult(
				new HierarchicalDataParams {
					SortOrderSlices = sortOrderSlices,
					Storage = result.Item1
				},
				result.Item2
			);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(UnderlyingDataQuery<SliceDataQuery> query) {
			OperationGraph graph = OperationGraphGenerator.GenerateUnderlying(query, dataSource.Storage);
			IDictionary<SingleBlockOperation, DataVectorBase> res = InMemoryDataProcessor.DataProcessor.ExecuteOperationGraph(graph.Output.Values.Concat(graph.SummaryAggregations.Values).ToList());
			return new DashboardUnderlyingDataSet(new InMemoryUnderlyingDataSet(graph, res, query.DataMembers, dataSource));
		}
		Tuple<DataStorage, List<SummaryAggregationResult>> Process(SliceDataQuery query, CancellationToken cancellationToken) {
			OperationGraph graph = OperationGraphGenerator.Generate(query, dataSource.Storage);
			List<SingleBlockOperation> resultOperations = graph.Output.Values.Concat(graph.SummaryAggregations.Values).ToList();
			IDictionary<SingleBlockOperation, DataVectorBase> res = InMemoryDataProcessor.DataProcessor.ExecuteOperationGraph(resultOperations, cancellationToken);
			if(cancellationToken.IsCancellationRequested)
				return new Tuple<DataStorage, List<SummaryAggregationResult>>(DataStorage.CreateEmpty(), new List<SummaryAggregationResult>());
			else
				return new Tuple<DataStorage, List<SummaryAggregationResult>>(CreateStorage(query, graph, res), CreateSummAggResults(graph, res));
		}
		DataStorage CreateStorage(SliceDataQuery query, OperationGraph graph, IDictionary<SingleBlockOperation, DataVectorBase> res) {
			DataStorage dataStorage = DataStorage.CreateEmpty();
			foreach(SliceModel sliceModel in query.DataSlices) {
				IEnumerable<StorageColumn> keyColumns = sliceModel.Dimensions.Select(d => dataStorage.CreateColumn(d.Name, true));
				StorageSlice slice = dataStorage.GetSlice(keyColumns);
				var resKeys = graph.Output.Keys.Where(key => key.SliceModel.Equals(sliceModel));
				var dimNames = sliceModel.Dimensions.Select(dim => dim.Name).ToList();
				var mNames = sliceModel.Measures.Select(m => m.Name).ToList();
				var measures = sliceModel.Measures.ToList();
				var dimKeys = resKeys.Where(key => dimNames.Contains(key.DataItemID));
				var mKeys = resKeys.Where(key => mNames.Contains(key.DataItemID));
				if(resKeys.Count() == 0)
					continue;
				List<DataVectorBase> dimVectors = new List<DataVectorBase>();
				List<IEnumerator<object>> dimEnums = new List<IEnumerator<object>>();
				List<DataVectorBase> mVectors = new List<DataVectorBase>();
				List<IEnumerator<object>> mEnums = new List<IEnumerator<object>>();
				List<StorageColumn> dimColumns = new List<StorageColumn>();
				List<StorageColumn> mColumns = new List<StorageColumn>();
				foreach(ResultKey dimKey in dimKeys) {
					DataVectorBase vector = res[graph.Output[dimKey]];
					dimVectors.Add(vector);
					dimEnums.Add(vector.GetUntypedData().GetEnumerator());
					dimColumns.Add(dataStorage.CreateColumn(dimKey.DataItemID, true));
				}
				foreach(ResultKey mKey in mKeys) {
					DataVectorBase vector = res[graph.Output[mKey]];
					mVectors.Add(vector);
					mEnums.Add(vector.GetUntypedData().GetEnumerator());
					mColumns.Add(dataStorage.CreateColumn(mKey.DataItemID, false));
				}
				int sliceLength = dimVectors.Count > 0 ? dimVectors[0].Count : mColumns.Count > 0 ? 1 : 0;
				for(int i = 0; i < sliceLength; i++) {
					StorageRow row = new StorageRow();
					for(int d_i = 0; d_i < dimEnums.Count; d_i++) {
						row[dimColumns[d_i]] = StorageValue.CreateUnbound(ToSpecialValue(GetNextValue(dimEnums[d_i]), true));
					}
					for(int m_i = 0; m_i < mEnums.Count; m_i++) {
						object mValue = GetNextValue(mEnums[m_i]);
						MeasureModel measure = measures[m_i];
						mValue = ToSpecialValue(mValue, false);
						row[mColumns[m_i]] = StorageValue.CreateUnbound(mValue);
					}
					slice.AddRow(row);
				}
			}
			return dataStorage;
		}
		List<SummaryAggregationResult> CreateSummAggResults(OperationGraph graph, IDictionary<SingleBlockOperation, DataVectorBase> res) {
			List<SummaryAggregationResult> summaryAggregations = new List<SummaryAggregationResult>();
			foreach(KeyValuePair<SummaryAggregationsKey, SingleBlockOperation> pair in graph.SummaryAggregations) {
				DataVectorBase vector = res[pair.Value];
				object value;
				SummaryItemTypeEx summaryType = pair.Key.SummaryAggregationModel.SummaryType;
				if(summaryType == SummaryItemTypeEx.TopPercent || summaryType == SummaryItemTypeEx.BottomPercent)
					value = new object[] { vector.GetUntypedData().Take(vector.Count).LastOrDefault() };
				else
					if(summaryType != SummaryItemTypeEx.Top && summaryType != SummaryItemTypeEx.Bottom)
						value = vector.GetUntypedData().First();
					else
						value = vector.GetUntypedData().Take(vector.Count).ToArray();
				SummaryAggregationResult res11 = new SummaryAggregationResult(pair.Key.SummaryAggregationModel.Name, pair.Key.SliceModel.Dimensions, pair.Key.SummaryAggregationModel, value);
				summaryAggregations.Add(res11);
			}
			return summaryAggregations;
		}
		static object ToSpecialValue(object value, bool processNull) {
			if(value is SpecialDataValue) {
				SpecialDataValue specValue = (SpecialDataValue)value;
				switch(specValue) {
					case SpecialDataValue.Null:
						return processNull ? DashboardSpecialValues.NullValue : null;
					case SpecialDataValue.Error:
						return DashboardSpecialValues.ErrorValue;
					case SpecialDataValue.Others:
						return DashboardSpecialValues.OthersValue;
					default:
						throw new DashboardInternalException("Incorrect DashboardSpecialValues value");
				}
			}
			return value;
		}
		static Type GetMeasureType(IDashboardDataSource dataSource, string dataSetName, MeasureModel measure) {
			Func<Type> resolveSourceType = () => {
				if(!String.IsNullOrEmpty(measure.UnboundExpression) && measure.UnboundMode != ExpressionMode.DataSourceLevel) {
					switch(measure.UnboundType) {
						case UnboundColumnType.Object:
							return typeof(object);
						case UnboundColumnType.Boolean:
							return typeof(bool);
						case UnboundColumnType.Integer:
							return typeof(int);
						case UnboundColumnType.Decimal:
							return typeof(decimal);
						case UnboundColumnType.String:
							return typeof(string);
						case UnboundColumnType.DateTime:
							return typeof(DateTime);
						default:
							throw new DashboardInternalException("Incorrect UnboundType");
					}
				}
				return dataSource.GetFieldSourceType(measure.DataMember, dataSetName);
			};
			if(measure.DecimalSummary) { 
				if(measure.SummaryType == SummaryType.Min || measure.SummaryType == SummaryType.Max)
					return typeof(decimal);
			}
			if(measure.UnboundMode != ExpressionMode.DataSourceLevel && !String.IsNullOrEmpty(measure.UnboundExpression))
				return resolveSourceType();
			return DataSourceHelper.GetSummaryType(measure.SummaryType, resolveSourceType);
		}
		static object GetNextValue(IEnumerator<object> enumerator) {
			if(!enumerator.MoveNext())
				throw new Exception("Incorrect result vector length!");
			return enumerator.Current;
		}
	}
}
