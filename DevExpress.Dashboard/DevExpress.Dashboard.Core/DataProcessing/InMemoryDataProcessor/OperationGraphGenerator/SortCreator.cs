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

using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public class SortCreator {
		public static List<NamedFlow> CreateSort(SliceModel slice, List<NamedFlow> outputs, SliceFlowSourceCreator sourceFlowSource, SummaryMeasureFlowSource summaryMeasureFlowSource) {
			if(slice.DimensionsSort.Count > 0) {
				List<int> sortIndex = new List<int>();
				List<SortOrder> sortOrder = new List<SortOrder>();
				List<SingleFlowOperation> toSort = outputs.Select(flow => flow.Flow).ToList();
				foreach(DimensionSortModel sortModel in slice.DimensionsSort) {
					DimensionTopNModel topNModel = slice.DimensionsTopN.FirstOrDefault(m => m.TopNShowOthers && m.DimensionModel.Name == sortModel.SortedDimension.Name && m.TopNEnabled);
					if(topNModel != null) {
						sortIndex.Add(toSort.Count);
						toSort.Add(
								 summaryMeasureFlowSource.CreateAggregate(
										 sourceFlowSource.TopNCreator.GetBitFlow(topNModel)
								, SummaryType.Max));
						sortOrder.Add(SortOrder.Descending);
					}
					if(sortModel.SortByMeasure == null || sortModel.SortByMeasure.Dimensions == null || sortModel.SortByMeasure.Dimensions.SequenceEqual(slice.Dimensions)) {
						string itemId = sortModel.SortByMeasure != null ? sortModel.SortByMeasure.Measure.Name : sortModel.SortedDimension.Name;
						int sortByIndex = outputs.FindIndex(flow => flow.DataItemID == itemId);
						if(sortByIndex == -1) {
							sortByIndex = toSort.Count;
							toSort.Add(summaryMeasureFlowSource.Resolve(sortModel.SortByMeasure.Measure));
						}
						sortIndex.Add(sortByIndex);
					} else {
						IList<DimensionModel> dimensions = sortModel.SortByMeasure.Dimensions;
						MeasureModel measure = sortModel.SortByMeasure.Measure;
						Group ngroup = new Group(dimensions.Select(d => sourceFlowSource.DataSourceDimensionResolver.Resolve(d)).ToArray());
						SummaryMeasureFlowSource resolveMeasure = new SummaryMeasureFlowSource(slice.Measures, ngroup, sourceFlowSource.DataSourceMeasureResolver);
						SingleFlowOperation[] criteriaFlows = dimensions.Select(d => outputs.Where(o => o.DataItemID == d.Name).Single().Flow).ToArray();
						SingleBlockOperation[] blocks = Enumerable.Range(0, dimensions.Count()).Select(i => (SingleBlockOperation)new Buffer(new Extract(new MultiScanBuffer(ngroup), i))).ToArray();
						SingleFlowOperation measureFlow = resolveMeasure.Resolve(measure);
						Extract extract = new Extract(new Join(criteriaFlows, blocks, new SingleBlockOperation[] { new Buffer(measureFlow) }), 0);
						sortIndex.Add(toSort.Count);
						toSort.Add(extract);
					}
					sortOrder.Add(sortModel.SortOrder == XtraPivotGrid.PivotSortOrder.Ascending ? SortOrder.Ascending : SortOrder.Descending);
				}
				MultiFlowOperation sort = new MultiScanBuffer(new Sort(toSort.ToArray(), sortIndex.ToArray(), sortOrder.ToArray()));
				outputs = outputs.Select((flow, index) => {
					return new NamedFlow(flow.DataItemID, new Extract(sort, index));
				}).ToList();
			}
			return outputs;
		}
	}
}
