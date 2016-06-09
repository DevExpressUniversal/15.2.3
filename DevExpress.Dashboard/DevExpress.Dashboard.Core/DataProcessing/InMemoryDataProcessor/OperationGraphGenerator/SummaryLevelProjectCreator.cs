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
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.ServerMode;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor {
	class SummaryLevelProjectCreator : MixedSummaryLevelCriteriaVisitor, IUniqueFieldNameGenerator {
		readonly SummaryMeasureFlowSource resolveSummaryMeasure;
		int summaryNamesCounter = 0;
		int dataSourceNamesCounter = 0;
		Dictionary<string, SingleFlowOperation> selfFlows = new Dictionary<string, SingleFlowOperation>();
		readonly Stack<string> onResolve = new Stack<string>();
		readonly Dictionary<string, string> summaryCache = new Dictionary<string, string>();
		public static SingleFlowOperation Process(string criteria, UnboundColumnType type, SingleFlowByNameSource resolveDataSourceField, SummaryMeasureFlowSource resolveSummaryMeasure) {
			return new SummaryLevelProjectCreator(resolveSummaryMeasure).Calculate(criteria, type, resolveDataSourceField);
		}
		public SummaryLevelProjectCreator(SummaryMeasureFlowSource resolveSummaryMeasure) : base(null) {
			SetUniqueFieldNameGenerator(this);
			this.resolveSummaryMeasure = resolveSummaryMeasure;
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			if(onResolve.Contains(theOperand.PropertyName))
				throw new ArgumentException(string.Join(",", onResolve.ToArray()));
			string name;
			if(!summaryCache.TryGetValue(theOperand.PropertyName, out name)) {
				name = "v" + summaryNamesCounter++;
				onResolve.Push(theOperand.PropertyName);
				selfFlows[name] = resolveSummaryMeasure.Resolve(theOperand.PropertyName);
				summaryCache[theOperand.PropertyName] = name;
				onResolve.Pop();
			}
			return new OperandProperty(name);
		}
		SingleFlowOperation Calculate(string criteria, UnboundColumnType type, SingleFlowByNameSource resolveDataSourceField) {
			CriteriaOperator newCriteria = ColumnCriteriaHelper.WrapToType(Process(CriteriaOperator.Parse(criteria)), type);
			string newCriteriaStr = CriteriaOperator.ToString(newCriteria);
			Dictionary<int, SingleFlowOperation> flows = SummaryLevel.SelectMany(dataSourceCriteria => {
				SingleFlowOperation dataSourceFlow = resolveDataSourceField.GetDataSourceLevelCriteria(CriteriaOperator.ToString(dataSourceCriteria.Key), UnboundColumnType.Object);
				return dataSourceCriteria.Value.SummaryCriterias.Select(
				v2 => new { Flow = resolveSummaryMeasure.CreateAggregate(dataSourceFlow, ToDashboardType(v2.SummaryType, v2.CustomAggregate)), Name = v2.SummaryLevelName });
			}).ToDictionary(pair => int.Parse(pair.Name.Substring(1)), pair => pair.Flow);
			foreach(KeyValuePair<string, SingleFlowOperation> val in selfFlows)
				flows.Add(int.Parse(val.Key.Substring(1)), val.Value);
			return DataItemModelToFlowHelper.CreateProject(newCriteriaStr, resolveSummaryMeasure, flows.OrderBy((pair) => pair.Key).Select(pair => pair.Value).ToArray());
		}
		SummaryType ToDashboardType(DevExpress.Data.PivotGrid.PivotSummaryType pivotSummaryType, IPivotCustomSummaryAggregate customAggregate) {
			switch(pivotSummaryType) {
				case DevExpress.Data.PivotGrid.PivotSummaryType.Average:
					return SummaryType.Average;
				case DevExpress.Data.PivotGrid.PivotSummaryType.Count:
					return SummaryType.Count;
				case DevExpress.Data.PivotGrid.PivotSummaryType.Custom: 
					return SummaryType.CountDistinct;
				case DevExpress.Data.PivotGrid.PivotSummaryType.Max:
					return SummaryType.Max;
				case DevExpress.Data.PivotGrid.PivotSummaryType.Min:
					return SummaryType.Min;
				case DevExpress.Data.PivotGrid.PivotSummaryType.StdDev:
					return SummaryType.StdDev;
				case DevExpress.Data.PivotGrid.PivotSummaryType.StdDevp:
					return SummaryType.StdDevp;
				case DevExpress.Data.PivotGrid.PivotSummaryType.Sum:
					return SummaryType.Sum;
				case DevExpress.Data.PivotGrid.PivotSummaryType.Var:
					return SummaryType.Var;
				case DevExpress.Data.PivotGrid.PivotSummaryType.Varp:
					return SummaryType.Varp;
				default:
					throw new ArgumentException();
			}
		}
		string IUniqueFieldNameGenerator.Generate(string prefix) {
			if(prefix == MixedSummaryLevelCriteriaVisitor.DataSourceNamePrefix)
				return "v" + dataSourceNamesCounter++;
			else
				return "v" + summaryNamesCounter++;
		}
	}
}
