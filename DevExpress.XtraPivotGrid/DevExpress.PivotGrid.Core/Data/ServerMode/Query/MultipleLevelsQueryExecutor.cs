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
using System.Reflection;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.TuplesTree;
using DevExpress.PivotGrid.ServerMode.TuplesTree;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.ServerMode {
	abstract class MultipleLevelsQueryExecutor : QueryExecutorBase<ServerModeColumn, IServerModeHelpersOwner>, IServerModeQueryExecutor {
		public static SortCriteria CreateSortByCriteria(QueryColumn column, List<ServerModeColumn> datas) {
			ISelectCriteriaConvertible conv = column.SortBySummary as ISelectCriteriaConvertible;
			if(conv == null && datas.Count > 0)
				conv = datas[0];
			if(conv != null)
				return new SortCriteria(conv, column.SortOrder);
			return
				null;
		}
		protected MultipleLevelsQueryExecutor(QueryMetadata<ServerModeColumn> metadata) : base(metadata) { }
		protected override CellSet<ServerModeColumn> QueryData(IQueryContext<ServerModeColumn> context) {
			if(context.ColumnArea.Count == 0 && context.RowArea.Count == 0 && context.Areas.DataArea.Count == 0)
				return null;
			IServerQueryContext serverQueryContext = (IServerQueryContext)context;
			CellSet<ServerModeColumn> set = new CellSet<ServerModeColumn>(serverQueryContext, CurrentOwner);
			IEnumerable<CrossAreaQueryContext> areaContexts = serverQueryContext.IsFullExpand ?
															(IEnumerable<CrossAreaQueryContext>) new FullExpandQueryContextCreator(CurrentOwner, serverQueryContext, this) :
															new CrossAreaQueryContextCreator(CurrentOwner, serverQueryContext, this);
			foreach(CrossAreaQueryContext crossContext in areaContexts)
				QueryOneLevelData(CurrentOwner, serverQueryContext, set, crossContext);
			return set;
		}
		protected internal List<object> GetTopSelectedRows(IServerModeHelpersOwner owner, QueryColumn topColumn, QueryTuple filter, SortCriteria sort) {
			return GetTopSelectedRows(owner, topColumn, new QueryTupleListToCriteriaOperatorConverter(filter), sort);
		}
		bool IServerModeQueryExecutor.ValidateExpression(IServerModeHelpersOwner owner, CriteriaOperator criteria, UnboundColumnType columnType) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				return GetUnboundExpressionTypeCore(owner, ColumnCriteriaHelper.WrapToType(criteria, columnType), null) != null;
		}
		Type IServerModeQueryExecutor.GetUnboundExpressionType(IServerModeHelpersOwner owner, CriteriaOperator criteria, bool makeQuery) {
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner)) {
				if(IsLogicalCriteriaChecker.GetBooleanState(criteria) == BooleanCriteriaState.Logical) {
					IServerModeQueryExecutor exec = this;
					if(exec.ValidateExpression(owner, criteria, UnboundColumnType.Boolean))
						return typeof(bool);
				}
				return GetUnboundExpressionTypeCore(owner, criteria, makeQuery);
			}
		}
		Type GetUnboundExpressionTypeCore(IServerModeHelpersOwner owner, CriteriaOperator criteria, bool? raw) {
			Type resultType = null;
			try {
				resultType = owner.Executor.ValidateCriteria((CurrentOwner.Executor.CriteriaSyntax & CriteriaSyntax.ServerCriteria) != 0 ? PropertyToQueryOperandPatcher.Patch(criteria, owner) : criteria, raw);
			} catch {
			}
			return resultType;
		}
		protected abstract IPivotQueryResult QueryDataCore(List<ISelectCriteriaConvertible> operands, List<IGroupCriteriaConvertible> grouping, int aggragetesIndex, List<IRawCriteriaConvertible> filters,
																	 bool isTopPercentage = false, SortCriteria sorting = null, bool detalizeRows = false, int maxRowCount = 0);
		protected abstract bool SupporsNestedSummaries { get; }
		protected abstract object QueryTopBottomAggregations(bool isTop, int number, List<CriteriaOperator> grouping, CriteriaOperator select, CriteriaOperator filter);
		protected abstract void QueryAggregationsCalculation(CriteriaOperator selectCriteria, List<CriteriaOperator> groupingCriteria, IEnumerable<AggregationItemValue> items, CriteriaOperator filter);
		protected internal List<object> GetTopSelectedRows(IServerModeHelpersOwner owner, QueryColumn topColumn, IRawCriteriaConvertible filter, SortCriteria sort) {
			List<IRawCriteriaConvertible> filters = new List<IRawCriteriaConvertible>();
			filters.Add(new CriteriaOperatorToServerModeCriteria(owner.FilterHelper.Criteria, null, owner, true));
			if(filter != null)
				filters.Add(filter);
			return QueryValuesCore((ServerModeColumn)topColumn,  sort, filters, topColumn.TopValueCount, topColumn.TopValueType == PivotTopValueType.Percent);
		}
		void QueryOneLevelData(IServerModeHelpersOwner owner, IServerQueryContext context, CellSet<ServerModeColumn> set, CrossAreaQueryContext crossAreaContext, SortCriteria sort = null) {
			AreaQueryContext rowContext = crossAreaContext.Row;
			AreaQueryContext columnContext = crossAreaContext.Column;
			List<QueryColumn> dataArea = new List<QueryColumn>();
			foreach(QueryColumn column in context.Areas.ServerSideDataArea)
				dataArea.Add(column);
			IPivotQueryResult reader = QueryOneLevelDataCore(owner, set, rowContext, columnContext, crossAreaContext.DataArea);
			TuplesIndexedTreeCache<ServerModeColumn> rowCache = set.RowIndexes;
			TuplesIndexedTreeCache<ServerModeColumn> columnCache = set.ColumnIndexes;
			Dictionary<IQueryMetadataColumn, int> dataIndexes = new Dictionary<IQueryMetadataColumn, int>();
			for(int i = 0; i < crossAreaContext.DataArea.Count; i++)
				dataIndexes.Add(crossAreaContext.DataArea[i].Metadata, i);
			if(reader != null && reader.Data != null) {
				MemberCreator rowCreator = new MemberCreator(rowContext.Grouping, context.RowArea, context.Areas.RowArea, rowContext.Others, 0);
				MemberCreator columnCreator = new MemberCreator(columnContext.Grouping, context.ColumnArea, context.Areas.ColumnArea, columnContext.Others, rowCreator.RealCount);
				Type[] types = reader.AggregatesSchema;
				for(int i = 0; i < types.Length; i++) {
					if(types[i] == null)
						types[i] = typeof(object);
					if(Nullable.GetUnderlyingType(types[i]) == null && !types[i].IsClass())
						types[i] = typeof(Nullable<>).MakeGenericType(new Type[] { types[i] });
				}
				Func<object[], MeasuresStorage> materialize = MeasureStorageMaterializer.Create(types, rowCreator.RealCount + columnCreator.RealCount, dataIndexes);
				try {
				foreach(object[] vals in reader.Data) {
					rowCreator.SetValues(vals);
					LevelRecord rowIndex = rowCache.AddResultValues(rowCreator);
					columnCreator.SetValues(vals);
					LevelRecord columnIndex = columnCache.AddResultValues(columnCreator);
					if(crossAreaContext.DataArea.Count > 0)
						set.SetRowValue(rowIndex, columnIndex, materialize(vals), 0);
				}
				} catch (Exception ex) {
					 if(TryHandleException(new QueryHandleableException("Query error", ex, true)))
						 return;
					throw ex;
				}
			}
		}
		IPivotQueryResult QueryOneLevelDataCore(IServerModeHelpersOwner owner, CellSet<ServerModeColumn> set, AreaQueryContext rowContext, AreaQueryContext columnContext, List<ServerModeColumn> dataArea) {
			List<IGroupCriteriaConvertible> grouping = new List<IGroupCriteriaConvertible>();
			grouping.AddRange(rowContext.Grouping);
			grouping.AddRange(columnContext.Grouping);
			List<ISelectCriteriaConvertible> operands = new List<ISelectCriteriaConvertible>();
			foreach(ISelectCriteriaConvertible conv in rowContext.Grouping)
				operands.Add(conv);
			foreach(ISelectCriteriaConvertible conv in columnContext.Grouping)
				operands.Add(conv);
			for(int i = 0; i < dataArea.Count; i++)
				operands.Add((ISelectCriteriaConvertible)dataArea[i]);
			List<IRawCriteriaConvertible> filters = new List<IRawCriteriaConvertible>();
			set.ColumnIndexes.Others.Clear();
			set.RowIndexes.Others.Clear();
			foreach(ServerModeMember member in rowContext.Others)
				PrepareOthersColumn(set, grouping, operands, member, false);
			foreach(ServerModeMember member in columnContext.Others)
				PrepareOthersColumn(set, grouping, operands, member, true);
			int counter = 0;
			for(int i = rowContext.Grouping.Count + columnContext.Grouping.Count - 1; i >= 0; i--) {
				IGroupCriteriaConvertible conv = operands[i];
				if(!ReferenceEquals(null, conv.GetGroupCriteria() as OperandValue)) {
					grouping.Remove(conv);
					operands.RemoveAt(i);
					counter++;
				}
			}
			try {
				filters.Add(new CriteriaOperatorToServerModeCriteria(owner.FilterHelper.Criteria, null, owner, true));
			} catch {
			}
			filters.Add(rowContext.Filter);
			filters.Add(columnContext.Filter);
			return QueryDataCore(operands, grouping, rowContext.Grouping.Count + columnContext.Grouping.Count - counter, filters);
		}
		void PrepareOthersColumn(CellSet<ServerModeColumn> set, List<IGroupCriteriaConvertible> grouping, List<ISelectCriteriaConvertible> operands, ServerModeMember member, bool isColumn) {
			TuplesIndexedTreeCache<ServerModeColumn> rowIndexes = set.RowIndexes;
			TuplesIndexedTreeCache<ServerModeColumn> columnIndexes = set.ColumnIndexes;
			if(rowIndexes != null && columnIndexes != null) {
				Dictionary<IQueryMetadataColumn, QueryMember> dic;
				if(isColumn)
					dic = columnIndexes.Others;
				else
					dic = rowIndexes.Others;
				if(!dic.ContainsKey(member.Column))
					dic.Add(member.Column, member);
				else
					dic[member.Column] = member;
				int index = operands.FindIndex(
												  (col) => {
													  QueryColumn qCol = col as QueryColumn;
													  return qCol != null && qCol.Metadata == member.Column;
												  });
				if(index < 0)
					return;
				grouping.Remove(operands[index]);
				operands[index] = new OthersValueColumn();
			}
		}
		protected override IDataTable QueryDrillDown(QueryMember[] columnMembers, QueryMember[] rowMembers, ServerModeColumn measure,
			int maxRowCount, List<string> customColumns) {
			List<CriteriaOperator> ops = new List<CriteriaOperator>();
			List<ISelectCriteriaConvertible> operands = new List<ISelectCriteriaConvertible>();
			if(!object.ReferenceEquals(null, CurrentOwner.FilterHelper.Criteria)) {
				ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
				CurrentOwner.FilterHelper.Criteria.Accept(visitor);
				foreach(string name in visitor.ColumnNames) {
					ServerModeColumn column = null;
					if(CurrentOwner.CubeColumns.TryGetValue(name, out column))
						if(NeedAddColumn(ops, column))
							operands.Add(column);
					MetadataColumnBase metadataColumn = null;
					if(CurrentOwner.Metadata.Columns.TryGetValue(name, out metadataColumn))
						if(NeedAddColumn(ops, column))
							operands.Add(column);
				}
			}
			foreach(ServerModeColumn column in CurrentOwner.Areas.ColumnArea)
				if(NeedAddColumn(ops, column))
					operands.Add(column);
			foreach(ServerModeColumn column in CurrentOwner.Areas.RowArea)
				if(NeedAddColumn(ops, column))
					operands.Add(column);
			if(NeedAddColumn(ops, measure))
				operands.Add(measure);
			if(customColumns != null)
				foreach(string dataMember in customColumns) {
					MetadataColumnBase column = null;
					if(CurrentOwner.Metadata.Columns.TryGetValue(dataMember, out column)) {
						IGroupCriteriaConvertible conv = (IGroupCriteriaConvertible)column;
						if(NeedAddColumn(ops, conv))
							operands.Add(new GroupToSelectCriteria(conv));
					}
				}
			List<IRawCriteriaConvertible> filters = new List<IRawCriteriaConvertible>();
			filters.Add(new CriteriaOperatorToServerModeCriteria(CurrentOwner.FilterHelper.Criteria, null, CurrentOwner, true));
			foreach(ServerModeMember member in columnMembers)
				filters.Add(member);
			foreach(ServerModeMember member in rowMembers)
				filters.Add(member);
			List<IColumn> columns = new List<IColumn>();
			foreach(IDrillDownProvider drillDownProvider in operands)
				columns.Add(new ColumnWrapper(drillDownProvider.DrillDownName, CurrentOwner.GetFieldCaption(drillDownProvider.Name), drillDownProvider.DataType));
			return new DataTableWrapper(columns, QueryDataCore(operands, new List<IGroupCriteriaConvertible>(), -1, filters, false, null, true, Math.Max(maxRowCount, 0)).Data.ToList());
		}
		bool NeedAddColumn(List<CriteriaOperator> list, IRawCriteriaConvertible conv) {
			if(conv == null)
				return false;
			bool result = false;
			CriteriaOperator op = conv.GetRawCriteria();
			if(!list.Contains(op)) {
				result = true;
				list.Add(op);
			}
			return result;
		}
		protected override object[] QueryAvailableValues(ServerModeColumn column, bool deferUpdates, List<ServerModeColumn> customFilters) {
			return QueryValuesCore(column, null, new CriteriaOperatorToServerModeCriteria(CurrentOwner.FilterHelper.Criteria, column, CurrentOwner, true)).ToArray();
		}
		protected override List<object> QueryVisibleValues(ServerModeColumn column) {
			return QueryValuesCore(column, null, new CriteriaOperatorToServerModeCriteria(CurrentOwner.FilterHelper.Criteria, null, CurrentOwner, true));
		}
		List<object> IServerModeQueryExecutor.QueryValues(IServerModeHelpersOwner owner, QueryColumn column, Dictionary<QueryColumn, object> values) {
			List<IRawCriteriaConvertible> filters = new List<IRawCriteriaConvertible>();
			if(values != null)
				filters.Add(new QueryTupleListToCriteriaOperatorConverter(new QueryTuple(values.Select((pair) => new ServerModeMember(pair.Key.Metadata, pair.Value)).ToArray())));
			using(OwnerSetter ownerSetter = new OwnerSetter(this, owner))
				return QueryValuesCore((ServerModeColumn)column, null, filters, 0, false);
		}
		List<object> QueryValuesCore(ISelectCriteriaConvertible column, SortCriteria sort, IRawCriteriaConvertible filter) {
			return QueryValuesCore(column, sort, new List<IRawCriteriaConvertible>() { filter }, 0, false);
		}
		List<object> QueryValuesCore(ISelectCriteriaConvertible column, SortCriteria sort, List<IRawCriteriaConvertible> filter, int topCount, bool isTopPercentage) {
			return QueryDataCore(new List<ISelectCriteriaConvertible>() { column }, new List<IGroupCriteriaConvertible>() { column }, -1, filter, isTopPercentage, sort, false, topCount).Data.Select((i) => i[0]).ToList();
		}
		protected override bool QueryNullValues(IQueryMetadataColumn column) {
			VirtualMetadataColumn virtualColumn = column as VirtualMetadataColumn;
			if(virtualColumn != null && virtualColumn.Column != null)
				column = virtualColumn.Column;
			if(HasAggregateCriteriaChecker.Check(((IGroupCriteriaConvertible)column).GetGroupCriteria()))
				return false;
			return QueryDataCore(new List<ISelectCriteriaConvertible>() { new GroupToSelectCriteria((IGroupCriteriaConvertible)column) }, new List<IGroupCriteriaConvertible>(), -1, new List<IRawCriteriaConvertible>() { new ServerModeMember(column, null) }, false, null, false, 1).Data.Any();
		}
		protected override void QueryAggregations(IList<AggregationLevel> aggregationLevels) {
			if(!Metadata.Connected) {
				AggregationLevel.SetErrorValue(aggregationLevels);
				return;
			}
			foreach(AggregationLevel level in aggregationLevels) {
				int rowLevel = level.Row;
				int columnLevel = level.Column;
				List<ServerModeColumn> columns = new List<ServerModeColumn>();
				for(int i = 0; i <= columnLevel; i++)
					columns.Add(CurrentOwner.Areas.ColumnArea[i]);
				for(int i = 0; i <= rowLevel; i++)
					columns.Add(CurrentOwner.Areas.RowArea[i]);
				CriteriaOperator filter = null;
				foreach(ServerModeColumn column in CurrentOwner.Areas.RowArea.Concat(CurrentOwner.Areas.ColumnArea)) {
					if(column.TopValueMode != TopValueMode.ParentFieldValues && !column.TopValueShowOthers && column.TopValueCount > 0) {
						filter = CriteriaOperator.And(filter, 
								((IRawCriteriaConvertible)new InMembers(column,
											DXListExtensions.ConvertAll<object, QueryMember>(GetTopSelectedRows(CurrentOwner, column, (IRawCriteriaConvertible)null, CreateSortByCriteria(column, CurrentOwner.Areas.ServerSideDataArea)), (val) => (QueryMember)new ServerModeMember(column.Metadata, val)).ToList()
											)).GetRawCriteria());
					}
				}
				foreach(AggregationCalculation calc in level) {
					if(calc.Target != AggregationCalculatationTarget.Data)
						throw new NotImplementedException("Target");
					List<CriteriaOperator> groupingCriteria = DXListExtensions.ConvertAll<IGroupCriteriaConvertible, CriteriaOperator>(DXListExtensions.ConvertAll<ServerModeColumn, IGroupCriteriaConvertible>(columns, (c) => c), (g) => g.GetGroupCriteria());
					CriteriaOperator selectCriteria = ClientCriteriaToServerCriteriaVisitor.Visit(CurrentOwner.Areas.DataArea[calc.Index], CurrentOwner);
					if(SupporsNestedSummaries) {
						QueryAggregationsCalculation(selectCriteria, groupingCriteria, calc, filter);
					} else {
						foreach(AggregationItemValue item2 in calc) {
							object result;
							switch(item2.SummaryType) {
								case SummaryItemTypeEx.Top:
									result = QueryTopBottomAggregations(true, Convert.ToInt32(item2.SummaryArgument), groupingCriteria, selectCriteria, filter);
									break;
								case SummaryItemTypeEx.Bottom:
									result = QueryTopBottomAggregations(false, Convert.ToInt32(item2.SummaryArgument), groupingCriteria, selectCriteria, filter);
									break;
								case SummaryItemTypeEx.Min:
									result = QueryTopBottomAggregations(false, 1, groupingCriteria, selectCriteria, filter);
									break;
								case SummaryItemTypeEx.Max:
									result = QueryTopBottomAggregations(true, 1, groupingCriteria, selectCriteria, filter);
									break;
								default:
									result = PivotCellValue.ErrorValue.Value;
									break;
							}
							if(item2.SummaryType == SummaryItemTypeEx.Top || item2.SummaryType == SummaryItemTypeEx.Bottom) {
								object[] arr = new object[Convert.ToInt32(item2.SummaryArgument)];
								arr[Convert.ToInt32(item2.SummaryArgument) - 1] = result;
								result = arr;
							}
							item2.SetValue(result);
						}
					}
				}
			}
		}
	}
}
