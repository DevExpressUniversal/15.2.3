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
using System.Linq;
using DevExpress.XtraPivotGrid;
using DevExpress.Data;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	public static class DataItemModelToFlowHelper {
		public static SingleFlowOperation CreateDimensionFlow(DimensionModel dimension, SingleFlowByNameSource resolveDataSourceField) {
			SingleFlowOperation resultFlow;
			if(string.IsNullOrEmpty(dimension.UnboundExpression))
				resultFlow = resolveDataSourceField.Resolve(dimension.DataMember);
			else
				resultFlow = resolveDataSourceField.GetDataSourceLevelCriteria(dimension.UnboundExpression, dimension.UnboundType);
			if(ProjectDateTime.DateGroupIntervals.Contains(dimension.GroupInterval))
				return new ProjectDateTime(resultFlow, dimension.GroupInterval);
			if(dimension.GroupInterval == PivotGroupInterval.Alphabetical && resultFlow.OperationType == typeof(string))
				return CreateProject("Substring([v0], 0, 1)", resolveDataSourceField, resultFlow);
			return resultFlow;
		}
		public static SingleFlowOperation CreateMeasureFlow(MeasureModel measure, bool isAggregate, SummaryMeasureFlowSource resolveSummaryLevelMeasure, SingleFlowByNameSource resolveDataSourceField) {
			switch(measure.UnboundMode) {
				case ExpressionMode.DataSourceLevel:
					SingleFlowOperation flow;
					if(string.IsNullOrEmpty(measure.UnboundExpression))
						flow = resolveDataSourceField.Resolve(measure.DataMember);
					else
						flow = resolveDataSourceField.GetDataSourceLevelCriteria(measure.UnboundExpression, measure.UnboundType);
					return isAggregate ? resolveSummaryLevelMeasure.CreateAggregate(flow, measure.SummaryType) : flow;
				case ExpressionMode.SummaryLevel:
				case ExpressionMode.AggregateFunction:
					return SummaryLevelProjectCreator.Process(measure.UnboundExpression, measure.UnboundType, resolveDataSourceField, resolveSummaryLevelMeasure);
				default:
					throw new NotSupportedException();
			}
		}
		public static SingleBlockOperation CreateSummaryAggregationFlow(SummaryItemTypeEx summaryType, decimal argument, SingleFlowOperation flow) {
			switch(summaryType) {
				case DevExpress.Data.SummaryItemTypeEx.Average:
					return new Buffer(CreateAggregate(null, flow, SummaryType.Average));
				case DevExpress.Data.SummaryItemTypeEx.Count:
					return new Buffer(CreateAggregate(null, flow, SummaryType.Count));
				case DevExpress.Data.SummaryItemTypeEx.Max:
					return new Buffer(CreateAggregate(null, flow, SummaryType.Max));
				case DevExpress.Data.SummaryItemTypeEx.Min:
					return new Buffer(CreateAggregate(null, flow, SummaryType.Min));
				case DevExpress.Data.SummaryItemTypeEx.Sum:
					return new Buffer(CreateAggregate(null, flow, SummaryType.Sum));
				case DevExpress.Data.SummaryItemTypeEx.Top: {
						SingleFlowOperation sortedFlow = new Extract(new MultiScanBuffer(new Sort(new SingleFlowOperation[] { flow }, new int[] { 0 }, new SortOrder[] { SortOrder.Descending })), 0);
						return new Buffer(new Select(sortedFlow,
														new ScanBuffer(new TopN(
																		  sortedFlow, Convert.ToInt32(argument), TopNMode.Top))));
					}
				case DevExpress.Data.SummaryItemTypeEx.Bottom: {
						SingleFlowOperation sortedFlow = new Extract(new MultiScanBuffer(new Sort(new SingleFlowOperation[] { flow }, new int[] { 0 }, new SortOrder[] { SortOrder.Ascending })), 0);
						return new Buffer(new Select(sortedFlow,
														new ScanBuffer(new TopN(
																		  sortedFlow, Convert.ToInt32(argument), TopNMode.Bottom))));
					}
				case DevExpress.Data.SummaryItemTypeEx.BottomPercent: {
						SingleFlowOperation sortedFlow = new Extract(new MultiScanBuffer(new Sort(new SingleFlowOperation[] { flow }, new int[] { 0 }, new SortOrder[] { SortOrder.Ascending })), 0);
						return new Buffer(new Select(sortedFlow,
														new ScanBuffer(new TopNPercent(
																		  new Buffer(sortedFlow), Convert.ToDouble(argument) / 100, TopNMode.Bottom))));
					}
				case DevExpress.Data.SummaryItemTypeEx.TopPercent: {
						SingleFlowOperation sortedFlow = new Extract(new MultiScanBuffer(new Sort(new SingleFlowOperation[] { flow }, new int[] { 0 }, new SortOrder[] { SortOrder.Descending })), 0);
						return new Buffer(new Select(sortedFlow,
														new ScanBuffer(new TopNPercent(
																		  new Buffer(sortedFlow), Convert.ToDouble(argument) / 100, TopNMode.Top))));
					}
				case DevExpress.Data.SummaryItemTypeEx.Custom:
				case DevExpress.Data.SummaryItemTypeEx.Duplicate:
				case DevExpress.Data.SummaryItemTypeEx.None:
				case DevExpress.Data.SummaryItemTypeEx.Unique:
				default:
					throw new NotSupportedException(summaryType.ToString());
			}
		}
		public static SingleFlowOperation CreateAggregate(ScanBuffer group, SingleFlowOperation measure, SummaryType summaryType) {
			if(!AggregateBase.IsNumericAggregationSupported(summaryType, measure.OperationType))
				measure = new ConvertType(measure, typeof(decimal));
			AggregateBase agg;
			if(group == null)
				agg = new GrandTotalAggregate(measure, summaryType);
			else
				agg = new GroupAggregate(group, measure, summaryType);
			return new ScanBuffer(agg);
		}
		public static SingleFlowOperation CreateProject(string expression, SingleFlowByNameSource source, params SingleFlowOperation[] input) {
			if(input.Length == 0)
				input = new SingleFlowOperation[] { source.GetAnyFlow() };
			return new Project(expression, input);
		}
	}
}
