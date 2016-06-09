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
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Data.Filtering.Helpers;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Xpo.Helpers;
using System.Collections.ObjectModel;
using DevExpress.Compatibility.System.Collections.Specialized;
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress.Xpo.DB.Helpers {
	public class IsTopLevelAggregateCheckerFull : IQueryCriteriaVisitor<bool> {
		public bool Visit(QuerySubQueryContainer theOperand) {
			if(theOperand.Node == null)
				return true;
			else
				return false;
		}
		public bool Visit(QueryOperand theOperand) {
			return false;
		}
		public bool Visit(FunctionOperator theOperator) {
			return Process(theOperator.Operands);
		}
		public bool Visit(OperandValue theOperand) {
			return false;
		}
		public bool Visit(GroupOperator theOperator) {
			return Process(theOperator.Operands);
		}
		public bool Visit(InOperator theOperator) {
			if(Process(theOperator.LeftOperand))
				return true;
			return Process(theOperator.Operands);
		}
		public bool Visit(UnaryOperator theOperator) {
			return Process(theOperator.Operand);
		}
		public bool Visit(BinaryOperator theOperator) {
			return Process(theOperator.LeftOperand) || Process(theOperator.RightOperand);
		}
		public bool Visit(BetweenOperator theOperator) {
			return Process(theOperator.TestExpression) || Process(theOperator.BeginExpression) || Process(theOperator.EndExpression);
		}
		bool Process(IEnumerable ops) {
			foreach(CriteriaOperator op in ops) {
				if(Process(op))
					return true;
			}
			return false;
		}
		bool Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return false;
			return op.Accept(this);
		}
		public static bool IsTopLevelAggregate(CriteriaOperator op) {
			return new IsTopLevelAggregateCheckerFull().Process(op);
		}
		public static bool IsGrouped(SelectStatement selectStatement) {
			if(selectStatement.GroupProperties.Count > 0)
				return true;
			if(!ReferenceEquals(selectStatement.GroupCondition, null))
				return true;
			int count = selectStatement.Operands.Count;
			for(int i = 0; i < count; i++) {
				if(IsTopLevelAggregate(selectStatement.Operands[i]))
					return true;
			}
			return false;
		}
	}
	public class ContextDependenceChecker : IQueryCriteriaVisitor<bool> {
		HashSet<string> upNodes;
		public ContextDependenceChecker(HashSet<string> upNodes) {
			this.upNodes = upNodes;
		}
		public bool Process(JoinNode root) {
			foreach(JoinNode node in DataSetStoreHelpersFull.GetAllNodes(root)) {
				if(Process(node.Condition)) return true;
			}
			return false;
		}
		public static bool Process(HashSet<string> upNodes, JoinNode root) {
			return new ContextDependenceChecker(upNodes).Process(root);
		}
		bool Process(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null))
				return false;
			if(upNodes == null) return false;
			return criteria.Accept(this);
		}
		public bool Visit(QuerySubQueryContainer theOperand) {
			return Process(theOperand.AggregateProperty) || Process(theOperand.Node);
		}
		public bool Visit(QueryOperand theOperand) {
			return upNodes.Contains(theOperand.NodeAlias);
		}
		public bool Visit(FunctionOperator theOperator) {
			return ProcessOperands(theOperator.Operands);
		}
		bool ProcessOperands(CriteriaOperatorCollection operands) {
			int count = operands.Count;
			for(int i = 0; i < count; i++) {
				if(Process(operands[i])) return true;
			}
			return false;
		}
		public bool Visit(OperandValue theOperand) {
			return false;
		}
		public bool Visit(GroupOperator theOperator) {
			return ProcessOperands(theOperator.Operands);
		}
		public bool Visit(InOperator theOperator) {
			if(Process(theOperator.LeftOperand)) return true;
			return ProcessOperands(theOperator.Operands);
		}
		public bool Visit(UnaryOperator theOperator) {
			return Process(theOperator.Operand);
		}
		public bool Visit(BinaryOperator theOperator) {
			return Process(theOperator.LeftOperand) || Process(theOperator.RightOperand);
		}
		public bool Visit(BetweenOperator theOperator) {
			return Process(theOperator.TestExpression) || Process(theOperator.BeginExpression) || Process(theOperator.EndExpression);
		}
	}
	public class IndexFinderResult {
		public readonly IndexFinderItem[][] Result;
		public IndexFinderResult(IndexFinderItem[][] result) {
			this.Result = result;
		}
	}
	public class IndexFinderItem {
		public readonly InMemoryColumn Column;
		public readonly object Value;
		public readonly bool ValueIsQueryOperand;
		public IndexFinderItem(InMemoryColumn column, object value, bool valueIsQueryOperand) {
			this.Column = column;
			this.Value = value;
			this.ValueIsQueryOperand = valueIsQueryOperand;
		}
		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "{0} = {1}", Column, Value);
		}
	}
	public class IndexFinderItemComparerByColumnIndex : IComparer<IndexFinderItem> {
		public int Compare(IndexFinderItem left, IndexFinderItem right) {
			if(ReferenceEquals(left, right)) return 0;
			if(ReferenceEquals(left, null)) return 1;
			if(ReferenceEquals(right, null)) return -1;
			return left.Column.ColumnIndex.CompareTo(right.Column.ColumnIndex);
		}
	}
	public class IndexFinder : IQueryCriteriaVisitor<object> {
		string alias;
		InMemoryTable table;
		Dictionary<string, InMemoryColumn> columns = new Dictionary<string, InMemoryColumn>();
		bool returnProperies;
		object Process(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null))
				return null;
			return criteria.Accept(this);
		}
		object ICriteriaVisitor<object>.Visit(FunctionOperator theOperator) {
			return null;
		}
		object ICriteriaVisitor<object>.Visit(OperandValue theOperand) {
			return theOperand.Value;
		}
		object ICriteriaVisitor<object>.Visit(GroupOperator theOperator) {
			if(theOperator.OperatorType == GroupOperatorType.And) return VisitGroupAnd(theOperator.Operands);
			return VisitGroupOr(theOperator.Operands);
		}
		object VisitGroupAnd(CriteriaOperatorCollection operands) {
			int count = operands.Count;
			List<List<IndexFinderItem>> result = null;
			List<IndexFinderItem> list = null;
			for(int operandIndex = 0; operandIndex < count; operandIndex++) {
				object val = Process((CriteriaOperator)operands[operandIndex]);
				IndexFinderItem key = val as IndexFinderItem;
				List<List<IndexFinderItem>> keyList = val as List<List<IndexFinderItem>>;
				if(result == null) {
					if(key != null) {
						if(list == null) list = new List<IndexFinderItem>();
						list.Add(key);
					} else if(keyList != null) {
						result = keyList;
						if(list == null) continue;
						for(int i = 0; i < keyList.Count; i++) {
							keyList[i].AddRange(list);
						}
					}
				} else {
					if(key != null) {
						for(int i = 0; i < result.Count; i++) {
							result[i].Add(key);
						}
					} else if(keyList != null) {
						List<List<IndexFinderItem>> newResult = new List<List<IndexFinderItem>>();
						for(int i = 0; i < result.Count; i++) {
							for(int j = 0; j < keyList.Count; j++) {
								List<IndexFinderItem> currentList = new List<IndexFinderItem>(result[i]);
								currentList.AddRange(keyList[j]);
								newResult.Add(currentList);
							}
						}
					}
				}
			}
			if(result == null && list != null) {
				result = new List<List<IndexFinderItem>>();
				result.Add(list);
			}
			return result;
		}
		object VisitGroupOr(CriteriaOperatorCollection operands) {
			int count = operands.Count;
			List<List<IndexFinderItem>> result = new List<List<IndexFinderItem>>();
			for(int operandIndex = 0; operandIndex < count; operandIndex++) {
				object val = Process((CriteriaOperator)operands[operandIndex]);
				IndexFinderItem key = val as IndexFinderItem;
				List<List<IndexFinderItem>> keyList = val as List<List<IndexFinderItem>>;
				if(key != null) {
					List<IndexFinderItem> list = new List<IndexFinderItem>();
					list.Add(key);
					result.Add(list);
				} else if(keyList != null) {
					result.AddRange(keyList);
				} else {
					return null;
				}
			}
			return result;
		}
		object ICriteriaVisitor<object>.Visit(InOperator theOperator) {
			object left = Process(theOperator.LeftOperand);
			if(!(left is InMemoryColumn))
				return null;
			int count = theOperator.Operands.Count;
			List<List<IndexFinderItem>> result = new List<List<IndexFinderItem>>(count);
			for(int i = 0; i < count; i++) {
				object val = Process((CriteriaOperator)theOperator.Operands[i]);
				if(val != null) {
					List<IndexFinderItem> list = new List<IndexFinderItem>();
					list.Add(new IndexFinderItem((InMemoryColumn)left, val, val is QueryOperand));
					result.Add(list);
				}
			}
			return result.Count > 0 ? result : null;
		}
		object ICriteriaVisitor<object>.Visit(UnaryOperator theOperator) {
			return null;
		}
		object ICriteriaVisitor<object>.Visit(BinaryOperator theOperator) {
			if(theOperator.OperatorType != BinaryOperatorType.Equal)
				return null;
			object left = Process(theOperator.LeftOperand);
			object right = Process(theOperator.RightOperand);
			if((left is InMemoryColumn) && right != null) {
				return new IndexFinderItem((InMemoryColumn)left, right, right is QueryOperand);
			}
			if((right is InMemoryColumn) && left != null) {
				return new IndexFinderItem((InMemoryColumn)right, left, left is QueryOperand);
			}
			return null;
		}
		object ICriteriaVisitor<object>.Visit(BetweenOperator theOperator) {
			return null;
		}
		public IndexFinder(string alias, InMemoryTable table, bool returnProperies)
			: this(alias, table) {
			this.returnProperies = returnProperies;
		}
		public IndexFinder(string alias, InMemoryTable table) {
			this.alias = alias;
			this.table = table;
			string aliasWithDot = alias + ".";
			foreach(InMemoryColumn column in table.Columns) {
				if(column.HasIndexes) columns.Add(aliasWithDot + column.Name, column);
			}
		}
		public static IndexFinderResult Find(string alias, InMemoryTable table, CriteriaOperator criteria) {
			return Find(alias, table, criteria, false);
		}
		public static IndexFinderResult Find(string alias, InMemoryTable table, CriteriaOperator criteria, bool returnProperties) {
			return new IndexFinder(alias, table, returnProperties).Find(criteria);
		}
		public IndexFinderResult Find(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null)) return null;
			if(table.Indexes.Count == 0) return null;
			object objResult = criteria.Accept(this);
			IndexFinderItem oneResult = objResult as IndexFinderItem;
			if(oneResult != null) {
				foreach(InMemoryIndexWrapper index in table.Indexes) {
					if(index.Columns.Count == 1 && ReferenceEquals(index.Columns[0], oneResult.Column))
						return new IndexFinderResult(new IndexFinderItem[][] { new IndexFinderItem[] { oneResult } });
				}
			}
			List<List<IndexFinderItem>> result = objResult as List<List<IndexFinderItem>>;
			if(result == null) return null;
			if(result.Count == 1 && result[0].Count == 1) {
				foreach(InMemoryIndexWrapper index in table.Indexes) {
					if(index.Columns.Count == 1 && ReferenceEquals(index.Columns[0], result[0][0].Column))
						return new IndexFinderResult(new IndexFinderItem[][] { new IndexFinderItem[] { result[0][0] } });
				}
				return null;
			}
			List<IndexFinderItem[]> resultChecked = new List<IndexFinderItem[]>();
			for(int i = 0; i < result.Count; i++) {
				List<IndexFinderItem> bestResultList = null;
				List<IndexFinderItem> currentList = result[i];
				currentList.Sort(new IndexFinderItemComparerByColumnIndex());
				foreach(InMemoryIndexWrapper index in table.Indexes) {
					List<IndexFinderItem> resultList = GetIndexIntersection(index, currentList);
					if(resultList == null) continue;
					if(bestResultList == null) bestResultList = resultList;
					if(bestResultList.Count < resultList.Count) bestResultList = resultList;
				}
				if(bestResultList == null) return null;
				resultChecked.Add(bestResultList.ToArray());
			}
			return new IndexFinderResult(resultChecked.ToArray());
		}
		static List<IndexFinderItem> GetIndexIntersection(InMemoryIndexWrapper index, List<IndexFinderItem> list) {
			int itemIndex = 0;
			List<IndexFinderItem> resultList = null;
			for(int i = 0; i < index.Columns.Count; i++) {
				InMemoryColumn indexColumn = index.Columns[i];
				while(itemIndex < list.Count) {
					if(ReferenceEquals(indexColumn, list[itemIndex].Column)) {
						if(resultList == null) resultList = new List<IndexFinderItem>();
						resultList.Add(list[itemIndex]);
						itemIndex++;
						break;
					}
					itemIndex++;
				}
			}
			if(resultList != null && resultList.Count == index.Columns.Count) return resultList;
			return null;
		}
		object IQueryCriteriaVisitor<object>.Visit(QuerySubQueryContainer theOperand) {
			return null;
		}
		object IQueryCriteriaVisitor<object>.Visit(QueryOperand theOperand) {
			InMemoryColumn column;
			if(columns.TryGetValue(string.Join(".", new string[] { theOperand.NodeAlias, theOperand.ColumnName }), out column)) return column;
			if(returnProperies) return theOperand;
			else return null;
		}
		public static IndexFinderItem[][] GetFullIndexResult(IndexFinderItem[][] inputResult) {
			List<IndexFinderItem[]> result = new List<IndexFinderItem[]>();
			List<IndexFinderItem> vector = new List<IndexFinderItem>(inputResult.Length);
			FillResultList(result, inputResult, vector, 0);
			return result.ToArray();
		}
		static void FillResultList(List<IndexFinderItem[]> resultList, IndexFinderItem[][] inputResult, List<IndexFinderItem> vector, int level) {
			IndexFinderItem[] currentList = inputResult[level];
			for(int i = 0; i < currentList.Length; i++) {
				vector.Add(currentList[i]);
				if(level < (inputResult.Length - 1)) FillResultList(resultList, inputResult, vector, level + 1);
				else resultList.Add(vector.ToArray());
				vector.RemoveAt(vector.Count - 1);
			}
		}
		public static bool HasQueryOperand(IndexFinderResult keys) {
			if(keys == null)
				return false;
			for(int j = 0; j < keys.Result.Length; j++) {
				IndexFinderItem[] key = keys.Result[j];
				for(int i = 0; i < key.Length; i++) {
					if(key[i].ValueIsQueryOperand)
						return true;
				}
			}
			return false;
		}
		public static InMemoryIndexWrapper[] GetIndexList(InMemoryTable table, IndexFinderResult keys) {
			if(keys == null)
				return null;
			InMemoryIndexWrapper[] result = new InMemoryIndexWrapper[keys.Result.Length];
			for(int j = 0; j < keys.Result.Length; j++) {
				IndexFinderItem[] key = keys.Result[j];
				InMemoryColumn[] columns = new InMemoryColumn[key.Length];
				for(int i = 0; i < key.Length; i++) {
					columns[i] = key[i].Column;
				}
				result[j] = table.Rows.FindIndex(columns);
			}
			return result;
		}
	}
	public class NodeCriteriaFinder: IQueryCriteriaVisitor<List<string>> {
		int inAtomicGroupLevel;
		Dictionary<string, PlanAliasCriteriaInfo> criteriaDict;
		string currentNodeAlias;
		public NodeCriteriaFinder() { }
		public static Dictionary<string, PlanAliasCriteriaInfo> FindCriteria(string currentNodeAlias, CriteriaOperator criteria) {
			return new NodeCriteriaFinder().Find(currentNodeAlias, criteria);
		}
		public static void FindCriteria(string currentNodeAlias, CriteriaOperator criteria, Dictionary<string, PlanAliasCriteriaInfo> previousDict) {
			new NodeCriteriaFinder().Find(currentNodeAlias, criteria, previousDict);
		}
		public Dictionary<string, PlanAliasCriteriaInfo> Find(string currentNodeAlias, CriteriaOperator criteria) {
			Find(currentNodeAlias, criteria, new Dictionary<string, PlanAliasCriteriaInfo>());
			return criteriaDict;
		}
		public void Find(string currentNodeAlias, CriteriaOperator criteria, Dictionary<string, PlanAliasCriteriaInfo> previousDict) {
			if(previousDict == null) throw new ArgumentNullException();
			this.currentNodeAlias = currentNodeAlias;
			criteriaDict = previousDict;
			ProcessAddOperands(new CriteriaOperator[] { criteria });
		}
		List<string> Process(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null))
				return null;
			return criteria.Accept(this);
		}
		List<string> IQueryCriteriaVisitor<List<string>>.Visit(QuerySubQueryContainer theOperand) {
			inAtomicGroupLevel++;
			try {
				List<string> internalNodes = new List<string>();
				List<string> result = new List<string>();
				List<string> propertyResult = Process(theOperand.AggregateProperty);
				if(propertyResult != null) result.AddRange(propertyResult);
				if(!ReferenceEquals(theOperand.Node, null)) {
					IEnumerable<JoinNode> allNodes = DataSetStoreHelpersFull.GetAllNodes(theOperand.Node);
					foreach(JoinNode node in allNodes) {
						internalNodes.Add(node.Alias);
						List<string> conditionResult = Process(node.Condition);
						if(conditionResult != null) result.AddRange(conditionResult);
					}
				}
				for(int i = result.Count - 1; i >= 0; i--) {
					if(internalNodes.Contains(result[i].Trim('*'))) {
						result.RemoveAt(i);
					}
				}
				return result;
			} finally {
				inAtomicGroupLevel--;
			}
		}
		List<string> IQueryCriteriaVisitor<List<string>>.Visit(QueryOperand theOperand) {
			List<string> list = new List<string>();
			list.Add(theOperand.NodeAlias);
			return list;
		}
		List<string> ICriteriaVisitor<List<string>>.Visit(FunctionOperator theOperator) {
			inAtomicGroupLevel++;
			try {
				return ProcessOperands(theOperator.Operands);
			} finally {
				inAtomicGroupLevel--;
			}
		}
		List<string> ICriteriaVisitor<List<string>>.Visit(OperandValue theOperand) {
			return null;
		}
		List<string> ICriteriaVisitor<List<string>>.Visit(GroupOperator theOperator) {
			if(theOperator.OperatorType == GroupOperatorType.Or) {
				inAtomicGroupLevel++;
				try {
					return ProcessOperands(theOperator.Operands);
				} finally {
					inAtomicGroupLevel--;
				}
			} else {
				if(inAtomicGroupLevel > 0) return ProcessOperands(theOperator.Operands);
				return ProcessAddOperands(theOperator.Operands.ToArray());
			}
		}
		List<string> ProcessAddOperands(CriteriaOperator[] operands) {
			int count = operands.Length;
			for(int i = 0; i < count; i++) {
				List<string> currentList = Process(operands[i]);
				if(currentList == null) continue;
				if(currentNodeAlias != null) currentList.Add("*" + currentNodeAlias);
				string nodesString = GetNodesString(currentList);
				PlanAliasCriteriaInfo list = null;
				if(!criteriaDict.TryGetValue(nodesString, out list)) {
					list = new PlanAliasCriteriaInfo(nodesString.Split('.'), new List<CriteriaOperator>());
					criteriaDict.Add(nodesString, list);
				}
				list.Criteria.Add(operands[i]);
			}
			return null;
		}
		public static string GetNodesString(List<string> list) {
			list.Sort();
			StringBuilder sb = new StringBuilder();
			int count = list.Count;
			string prevNode = null;
			for(int i = 0; i < count; i++) {
				if(list[i] == prevNode) continue;
				if(i > 0) sb.Append(".");
				sb.Append(list[i]);
				prevNode = list[i];
			}
			return sb.ToString();
		}
		List<string> ProcessOperands(List<CriteriaOperator> operands) {
			List<string> list = null;
			int count = operands.Count;
			for(int i = 0; i < count; i++) {
				List<string> currentList = Process(operands[i]);
				if(currentList == null) continue;
				if(list == null) list = currentList;
				else list.AddRange(currentList);
			}
			return list;
		}
		List<string> ICriteriaVisitor<List<string>>.Visit(InOperator theOperator) {
			inAtomicGroupLevel++;
			try {
				List<CriteriaOperator> operands = new List<CriteriaOperator>();
				operands.Add(theOperator.LeftOperand);
				operands.AddRange(theOperator.Operands);
				return ProcessOperands(operands);
			} finally {
				inAtomicGroupLevel--;
			}
		}
		List<string> ICriteriaVisitor<List<string>>.Visit(UnaryOperator theOperator) {
			inAtomicGroupLevel++;
			try {
				return Process(theOperator.Operand);
			} finally {
				inAtomicGroupLevel--;
			}
		}
		List<string> ICriteriaVisitor<List<string>>.Visit(BinaryOperator theOperator) {
			inAtomicGroupLevel++;
			try {
				List<string> leftResult = Process(theOperator.LeftOperand);
				List<string> rightResult = Process(theOperator.RightOperand);
				if(leftResult == null) return rightResult;
				if(rightResult == null) return leftResult;
				leftResult.AddRange(rightResult);
				return leftResult;
			} finally {
				inAtomicGroupLevel--;
			}
		}
		List<string> ICriteriaVisitor<List<string>>.Visit(BetweenOperator theOperator) {
			inAtomicGroupLevel++;
			try {
				List<string> beginResult = Process(theOperator.BeginExpression);
				List<string> endResult = Process(theOperator.EndExpression);
				List<string> testResult = Process(theOperator.TestExpression);
				List<string> tempResult;
				if(endResult == null) tempResult = testResult;
				else {
					tempResult = endResult;
					if(testResult != null) tempResult.AddRange(testResult);
				}
				if(beginResult == null) return tempResult;
				if(tempResult == null) return beginResult;
				beginResult.AddRange(tempResult);
				return beginResult;
			} finally {
				inAtomicGroupLevel--;
			}
		}
	}
	public class DataSetStoreHelpersFull {
		DataSetStoreHelpersFull() { }
		static InMemoryColumn[] GetColumns(InMemoryTable table, StringCollection columns) {
			InMemoryColumn[] dataColumns = new InMemoryColumn[columns.Count];
			int count = columns.Count;
			for(int i = 0; i < count; i++) {
				dataColumns[i] = table.Columns[columns[i]];
			}
			return dataColumns;
		}
		static string GetIndexName(InMemoryTable table, DBIndex index) {
			StringBuilder name = new StringBuilder(table.Name);
			int count = index.Columns.Count;
			for(int i = 0; i < count; i++) {
				name.Append(index.Columns[i]);
			}
			return name.ToString();
		}
		static bool IsEqual(DBIndex index, InMemoryIndexWrapper constraint) {
			int count = constraint.Columns.Count;
			if(index.Columns.Count != count)
				return false;
			List<string> indexNames = new List<string>();
			List<string> constraintNames = new List<string>();
			for(int i = 0; i < count; ++i) {
				constraintNames.Add(constraint.Columns[i].Name);
				indexNames.Add(index.Columns[i]);
			}
			indexNames.Sort();
			constraintNames.Sort();
			for(int i = 0; i < count; ++i) {
				if(indexNames[i] != constraintNames[i])
					return false;
			}
			return true;
		}
		static string IsExists(InMemoryTable table, DBIndex index) {
			foreach(InMemoryIndexWrapper currentIndex in table.Indexes) {
				if(IsEqual(index, currentIndex))
					return currentIndex.Name;
			}
			return null;
		}
		static bool IsExists(InMemoryTable table, DBPrimaryKey index) {
			if(table.PrimaryKey == null)
				return false;
			return IsEqual(index, table.PrimaryKey);
		}
		static bool IsEqual(DBForeignKey fk, InMemoryRelation relation) {
			if(fk.PrimaryKeyTable != relation.PTable.Name)
				return false;
			int count = relation.Pairs.Length;
			if(fk.Columns.Count != count)
				return false;
			for(int i = 0; i < count; ++i) {
				if(relation.Pairs[i].FKey.Name != fk.Columns[i] || fk.PrimaryKeyTableKeyColumns[i] != relation.Pairs[i].PKey.Name)
					return false;
			}
			return true;
		}
		static bool IsExists(InMemoryTable table, DBForeignKey fk) {
			for(int i = 0; i < table.FRelations.Count; i++) {
				if(IsEqual(fk, table.FRelations[i])) return true;
			}
			return false;
		}
		static bool IsExists(InMemoryTable table, DBColumn column) {
			return table.Columns[column.Name] != null;
		}
		public static InMemoryTable QueryTable(InMemorySet dataSet, string tableName) {
			return dataSet.GetTable(tableName);
		}
		public static InMemoryTable GetTable(InMemorySet dataSet, string tableName) {
			InMemoryTable result = QueryTable(dataSet, tableName);
			if(result == null)
				throw new SchemaCorrectionNeededException(tableName);
			return result;
		}
		static string Create(InMemoryTable table, DBIndex index) {
			int count = index.Columns.Count;
			InMemoryColumn[] columns = new InMemoryColumn[count];
			for(int i = 0; i < count; i++) {
				columns[i] = table.Columns[index.Columns[i]];
			}
			if(count > 1) Array.Sort<InMemoryColumn>(columns, new InMemoryColumnIndexComparer());
			return table.CreateIndex(columns, index.IsUnique);
		}
		static void Create(InMemoryTable table, DBPrimaryKey index) {
			string name = CreateIfNotExists(table, (DBIndex)index);
			table.SetPrimaryKey(name);
		}
		static void Create(InMemoryTable table, DBForeignKey fk) {
			var pkTable = table.BaseSet.GetTable(fk.PrimaryKeyTable);
			if(pkTable == null)
				throw new SchemaCorrectionNeededException(Res.GetString(Res.InMemoryFull_TableNotFound, fk.PrimaryKeyTable));
			InMemoryColumn[] pColumns = GetColumns(pkTable, fk.PrimaryKeyTableKeyColumns);
			InMemoryColumn[] fColumns = GetColumns(table, fk.Columns);
			if(pColumns.Length != fColumns.Length) throw new ArgumentException(Res.GetString(Res.InMemoryFull_DifferentColumnListLengths));
			InMemoryRelationPair[] pairs = new InMemoryRelationPair[pColumns.Length];
			for(int i = 0; i < pairs.Length; i++) {
				pairs[i] = new InMemoryRelationPair(pColumns[i], fColumns[i]);
			}
			table.BaseSet.AddRelation(pairs);
		}
		static void Create(InMemoryTable table, DBColumn column) {
			if(column.IsIdentity) {
				table.Columns.Add(column.Name, DBColumn.GetType(column.ColumnType), true);
			} else {
				table.Columns.Add(column.Name, DBColumn.GetType(column.ColumnType));
			}
			if(column.ColumnType == DBColumnType.String && column.Size != 0)
				table.Columns[column.Name].MaxLength = column.Size;
			table.Columns.CommitColumnsChanges();
		}
		static InMemoryTable Create(InMemorySet dataSet, DBTable table) {
			return dataSet.CreateTable(table.Name);
		}
		public static void CreateIfNotExists(InMemoryTable table, DBForeignKey dbObj) {
			if(!IsExists(table, dbObj))
				Create(table, dbObj);
		}
		public static string CreateIfNotExists(InMemoryTable table, DBIndex dbObj) {
			string name = IsExists(table, dbObj);
			if(string.IsNullOrEmpty(name))
				return Create(table, dbObj);
			return name;
		}
		public static void CreateIfNotExists(InMemoryTable table, DBPrimaryKey dbObj) {
			if(!IsExists(table, dbObj))
				Create(table, dbObj);
		}
		public static void CreateIfNotExists(InMemoryTable table, DBColumn dbObj) {
			if(!IsExists(table, dbObj))
				Create(table, dbObj);
		}
		public static InMemoryTable CreateIfNotExists(InMemorySet dataSet, DBTable table) {
			InMemoryTable result = QueryTable(dataSet, table.Name);
			if(result == null)
				result = Create(dataSet, table);
			return result;
		}
		static void UpdateRow(IInMemoryRow row, object[] values, InMemoryColumn[] columns) {
			int count = columns.Length;
			row.BeginEdit();
			try {
				for(int i = 0; i < count; i++) {
					row[columns[i].ColumnIndex] = values[i];
				}
				row.EndEdit();
			} catch {
				row.CancelEdit();
				throw;
			}
		}
		public static object[] GetResultRow(InMemoryComplexRow row, ExpressionEvaluator[] evaluators) {
			int count = evaluators.Length;
			object[] result = new object[count];
			for(int i = 0; i < count; i++) {
				result[i] = evaluators[i].Evaluate(row);
			}
			return result;
		}
		public static object[] GetResultRow(List<InMemoryComplexRow> rows, ExpressionEvaluator[] evaluators) {
			int count = evaluators.Length;
			object[] result = new object[count];
			for(int i = 0; i < count; i++) {
				result[i] = evaluators[i].Evaluate(rows);
			}
			return result;
		}
		public static SelectStatementResult DoGetData(IInMemoryDataElector dataElector, InMemoryDataElectorContextDescriptor descriptor, ExpressionEvaluator[] dataEvaluators, SortingComparerFull sortingComparer, int skipRecords, int topRecords) {
			InMemoryComplexSet rows = dataElector.Process(descriptor);
			if(sortingComparer != null) {
				rows.Sort(sortingComparer);
			} else {
				rows.Randomize();
			}
			if(skipRecords != 0) {
				if(skipRecords >= rows.Count)
					rows.Clear();
				else
					rows.RemoveRange(0, skipRecords);
			}
			if(topRecords != 0 && topRecords < rows.Count)
				rows.RemoveRange(topRecords, rows.Count - topRecords);
			int count = rows.Count;
			SelectStatementResultRow[] result = new SelectStatementResultRow[count];
			for(int i = 0; i < count; i++) {
				result[i] = new SelectStatementResultRow(GetResultRow(rows[i], dataEvaluators));
			}
			return new SelectStatementResult(result);
		}
		public static int DoInsertRecord(InMemoryTable table, QueryParameterCollection parameters, CriteriaOperatorCollection operands, ParameterValue identityParameter, TaggedParametersHolder identitiesByTag) {
			object[] values = new object[table.Columns.Count];
			int count = parameters.Count;
			identitiesByTag.ConsolidateIdentity(identityParameter);
			for(int i = 0; i < count; i++) {
				OperandValue parameter = identitiesByTag.ConsolidateParameter(parameters[i]);
				QueryOperand operand = (QueryOperand)operands[i];
				InMemoryColumn dColumn = table.Columns[operand.ColumnName];
				if(dColumn == null)
					throw new SchemaCorrectionNeededException(table.Name + "." + operand.ColumnName);
				values[dColumn.ColumnIndex] = parameter.Value;
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].AutoIncrement && ReferenceEquals(values[i], null)) values[i] = InMemoryAutoIncrementValue.Value;
			}
			InMemoryRow row = table.Rows.AddNewRow(values);
			if(!ReferenceEquals(identityParameter, null)) {
				int colCount = table.Columns.Count;
				for(int i = 0; i < colCount; i++) {
					InMemoryColumn column = table.Columns[i];
					if(column.AutoIncrement) {
						identityParameter.Value = row[column.ColumnIndex];
						break;
					}
				}
			}
			return 1;
		}
		public static int DoUpdateRecord(IInMemoryPlanner planner, InMemoryTable table, QueryParameterCollection parameters, CriteriaOperatorCollection operands, TaggedParametersHolder identitiesByTag, CriteriaOperator condition, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) {
			int count = operands.Count;
			object[] values = new object[count];
			InMemoryColumn[] columns = new InMemoryColumn[count];
			for(int i = 0; i < count; i++) {
				OperandValue parameter = identitiesByTag.ConsolidateParameter(parameters[i]);
				QueryOperand operand = (QueryOperand)operands[i];
				columns[i] = table.Columns[operand.ColumnName];
				if(columns[i] == null)
					throw new SchemaCorrectionNeededException(table.Name + "." + operand.ColumnName);
				values[i] = parameter.Value;
			}
			IInMemoryDataElector elector = planner.GetPlan(string.Empty, table, QueryParamsReprocessor.ReprocessCriteria(condition, identitiesByTag));
			InMemoryComplexSet rows = elector.Process(new InMemoryDataElectorContextDescriptor(planner, caseSensitive, customFunctions));
			int rowCount = rows.Count;
			for(int i = 0; i < rowCount; i++) {
				DataSetStoreHelpersFull.UpdateRow(rows[i][0], values, columns);
			}
			return rowCount;
		}
		public static int DoDeleteRecord(IInMemoryPlanner planner, InMemoryTable table, CriteriaOperator condition, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) {
			IInMemoryDataElector elector = planner.GetPlan(string.Empty, table, condition);
			InMemoryComplexSet rows = elector.Process(new InMemoryDataElectorContextDescriptor(planner, caseSensitive, customFunctions));
			int count = rows.Count;
			for(int i = 0; i < count; i++) {
				((InMemoryRow)rows[i][0]).Delete();
			}
			return count;
		}
		public static List<List<InMemoryComplexRow>> DoGetGroupedDataCore(ExpressionEvaluator[] groupEvaluators, ExpressionEvaluator havingEvaluator, SortingListComparerFull sortingComparer, int skipRecords, int topRecords, InMemoryComplexSet rows, bool doRandomizeIfNoSorting) {
			Dictionary<InMemoryGroupFull, List<InMemoryComplexRow>> groupingResult = new Dictionary<InMemoryGroupFull, List<InMemoryComplexRow>>();
			if(sortingComparer == null) {
				rows.Randomize();
			}
			if(groupEvaluators.Length == 0) {
				groupingResult.Add(new InMemoryGroupFull(new object[0]), new List<InMemoryComplexRow>());
			}
			int count = rows.Count;
			for(int i = 0; i < count; i++) {
				InMemoryComplexRow row = rows[i];
				InMemoryGroupFull group = new InMemoryGroupFull(GetResultRow(row, groupEvaluators));
				List<InMemoryComplexRow> groupList;
				if(!groupingResult.TryGetValue(group, out groupList)) {
					groupList = new List<InMemoryComplexRow>();
					groupingResult.Add(group, groupList);
				}
				groupList.Add(row);
			}
			List<List<InMemoryComplexRow>> havingResult = new List<List<InMemoryComplexRow>>();
			foreach(List<InMemoryComplexRow> group in groupingResult.Values) {
				if(group.Count > 0) {
					if(havingEvaluator.Fit(group))
						havingResult.Add(group);
				} else {
					if(havingEvaluator.Fit(null))
						havingResult.Add(group);
				}
			}
			if(sortingComparer != null) {
				havingResult.Sort(sortingComparer);
			}
			if(skipRecords != 0) {
				if(skipRecords >= havingResult.Count)
					havingResult.Clear();
				else
					havingResult.RemoveRange(0, skipRecords);
			}
			if(topRecords != 0 && topRecords < havingResult.Count) {
				havingResult.RemoveRange(topRecords, havingResult.Count - topRecords);
			}
			return havingResult;
		}
		public static SelectStatementResult DoGetGroupedData(IInMemoryDataElector dataElector, InMemoryDataElectorContextDescriptor descriptor, ExpressionEvaluator[] groupEvaluators, ExpressionEvaluator havingEvaluator, SortingListComparerFull sortingComparer, int skipRecords, int topRecords, ExpressionEvaluator[] dataEvaluators) {
			InMemoryComplexSet rows = dataElector.Process(descriptor);
			var havingResult = DoGetGroupedDataCore(groupEvaluators, havingEvaluator, sortingComparer, skipRecords, topRecords, rows, true);
			int count = havingResult.Count;
			SelectStatementResultRow[] result = new SelectStatementResultRow[count];
			for(int i = 0; i < count; i++) {
				result[i] = new SelectStatementResultRow(GetResultRow(havingResult[i], dataEvaluators));
			}
			return new SelectStatementResult(result);
		}
		public static IEnumerable<JoinNode> GetAllNodes(JoinNode node) {
			List<JoinNode> nodes = new List<JoinNode>();
			nodes.Add(node);
			GetAllNodesInternal(node, nodes);
			return nodes;
		}
		static void GetAllNodesInternal(JoinNode node, List<JoinNode> nodes) {
			foreach(JoinNode subNode in node.SubNodes) {
				nodes.Add(subNode);
				GetAllNodesInternal(subNode, nodes);
			}
		}
		public static ExpressionEvaluator PrepareDataEvaluator(CriteriaOperator operand, EvaluatorContextDescriptor descriptor, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) {
			return new QuereableExpressionEvaluator(descriptor, operand, caseSensitive, customFunctions);
		}
		public static ExpressionEvaluator[] PrepareDataEvaluators(CriteriaOperatorCollection operands, EvaluatorContextDescriptor descriptor, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) {
			int count = operands.Count;
			ExpressionEvaluator[] evaluators = new ExpressionEvaluator[count];
			for(int i = 0; i < count; i++) {
				evaluators[i] = PrepareDataEvaluator(operands[i], descriptor, caseSensitive, customFunctions);
			}
			return evaluators;
		}
		public static SortingComparerFull PrepareSortingComparer(QuerySortingCollection sortProperties, EvaluatorContextDescriptor descriptor, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) {
			if(sortProperties.Count == 0)
				return null;
			int count = sortProperties.Count;
			ExpressionEvaluator[] sortingEvaluators = new ExpressionEvaluator[count];
			for(int i = 0; i < count; i++) {
				SortingColumn sortingColumn = sortProperties[i];
				sortingEvaluators[i] = PrepareDataEvaluator(sortingColumn.Property, descriptor, caseSensitive, customFunctions);
			}
			return new SortingComparerFull(sortingEvaluators, sortProperties);
		}
		public static SortingListComparerFull PrepareSortingListComparer(QuerySortingCollection sortProperties, EvaluatorContextDescriptor descriptor, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) {
			if(sortProperties.Count == 0)
				return null;
			int count = sortProperties.Count;
			ExpressionEvaluator[] sortingEvaluators = new ExpressionEvaluator[count];
			for(int i = 0; i < count; i++) {
				SortingColumn sortingColumn = sortProperties[i];
				sortingEvaluators[i] = PrepareDataEvaluator(sortingColumn.Property, descriptor, caseSensitive, customFunctions);
			}
			return new SortingListComparerFull(sortingEvaluators, sortProperties);
		}
	}
	public class SortingComparerFull : IComparer<InMemoryComplexRow>, IComparer {
		ExpressionEvaluator[] evaluators;
		QuerySortingCollection operands;
		public SortingComparerFull(ExpressionEvaluator[] sortingEvaluators, QuerySortingCollection operands) {
			this.evaluators = sortingEvaluators;
			this.operands = operands;
		}
		int IComparer<InMemoryComplexRow>.Compare(InMemoryComplexRow x, InMemoryComplexRow y) {
			return ((IComparer)this).Compare(x, y);
		}
		int IComparer.Compare(object x, object y) {
			int i = 0;
			foreach (ExpressionEvaluator evaluator in evaluators) {
				int res = Comparer<object>.Default.Compare(evaluator.Evaluate(x), evaluator.Evaluate(y));
				if(res != 0)
					return operands[i].Direction == SortingDirection.Ascending ? res : -res;
				i++;
			}
			return 0;
		}
	}
	public class SortingListComparerFull : IComparer<List<InMemoryComplexRow>>, IComparer {
		ExpressionEvaluator[] evaluators;
		QuerySortingCollection operands;
		SortingComparerFull sortComparer;
		public SortingListComparerFull(ExpressionEvaluator[] sortingEvaluators, QuerySortingCollection operands) {
			this.evaluators = sortingEvaluators;
			this.operands = operands;
			this.sortComparer = new SortingComparerFull(sortingEvaluators, operands);
		}
		int IComparer<List<InMemoryComplexRow>>.Compare(List<InMemoryComplexRow> x, List<InMemoryComplexRow> y) {
			return ((IComparer)this).Compare(x, y);
		}
		int IComparer.Compare(object x, object y) {
			List<InMemoryComplexRow> xRows = x as List<InMemoryComplexRow>;
			List<InMemoryComplexRow> yRows = y as List<InMemoryComplexRow>;
			if(xRows == null && yRows == null) return 0;
			if(yRows == null) {
				xRows.Sort(sortComparer);
				return operands[0].Direction == SortingDirection.Ascending ? -1 : 1;
			}
			if(xRows == null) {
				yRows.Sort(sortComparer);
				return operands[0].Direction == SortingDirection.Ascending ? 1 : -1;
			}
			int i = 0;
			foreach(ExpressionEvaluator evaluator in evaluators) {
				int res = Comparer<object>.Default.Compare(evaluator.Evaluate(xRows), evaluator.Evaluate(yRows));
				if(res != 0)
					return operands[i].Direction == SortingDirection.Ascending ? res : -res;
				i++;
			}
			return 0;
		}
	}
	public class InMemoryGroupFull {
		public readonly object[] GroupValues;
		public InMemoryGroupFull(object[] groupValues) {
			this.GroupValues = groupValues;
		}
		public override bool Equals(object obj) {
			InMemoryGroupFull another = obj as InMemoryGroupFull;
			if(another == null)
				return false;
			if(this.GroupValues.Length != another.GroupValues.Length)
				return false;
			int count = GroupValues.Length;
			for(int i = 0; i < count; ++i) {
				if(!object.Equals(this.GroupValues[i], another.GroupValues[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int result = 0;
			foreach(object val in GroupValues) {
				if(val != null) {
					result ^= val.GetHashCode();
				}
			}
			return result;
		}
	}
	public class InMemoryDataElectorContextSource {
		public readonly string NodeAlias;
		public readonly InMemoryRow Row;
		public readonly InMemoryTable Table;
		public readonly InMemoryComplexRow ComplexRow;
		public readonly InMemoryComplexRow ComplexRowRight;
		public InMemoryDataElectorContextSource(InMemoryComplexRow complexRow, InMemoryComplexRow complexRowRight) {
			this.ComplexRow = complexRow;
			this.ComplexRowRight = complexRowRight;
		}
		public InMemoryDataElectorContextSource(string nodeAlias, InMemoryRow row, InMemoryTable table) : this(nodeAlias, row, table, null) { }
		public InMemoryDataElectorContextSource(string nodeAlias, InMemoryRow row, InMemoryTable table, InMemoryComplexRow complexRow) {
			if(ReferenceEquals(nodeAlias, null)) this.NodeAlias = string.Empty;
			else this.NodeAlias = nodeAlias;
			this.Row = row;
			this.Table = table;
			this.ComplexRow = complexRow;
		}
	}
	public class InMemoryDataElectorContextDescriptor : QuereableEvaluatorContextDescriptor {
		IInMemoryPlanner planner;
		bool caseSensitive;
		ICollection<ICustomFunctionOperator> customFunctions;
		public bool CaseSensitive { get { return caseSensitive; } }
		public ICollection<ICustomFunctionOperator> CustomFunctions { get { return customFunctions; } }
		List<object> nestedSource = new List<object>();
		Dictionary<JoinNode, InMemoryPlanCasheItem> existsPlanCashe = new Dictionary<JoinNode, InMemoryPlanCasheItem>();
		public InMemoryDataElectorContextDescriptor(IInMemoryPlanner planner, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) {
			this.planner = planner;
			this.caseSensitive = caseSensitive;
			this.customFunctions = customFunctions;
		}
		public override IEnumerable GetCollectionContexts(object source, string collectionName) {
			throw new NotSupportedException();
		}
		public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
			throw new NotSupportedException();
		}
		public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
			throw new NotSupportedException();
		}
		public override IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) {
			throw new NotSupportedException();
		}
		public override object GetOperandValue(object currentSource, QueryOperand theOperand) {
			if(currentSource == null && nestedSource.Count == 0)
				return null;
			string alias = theOperand.NodeAlias;
			if(alias == null) alias = string.Empty;
			int sourceCounter = -1;
			while(sourceCounter < nestedSource.Count) {
				object source;
				if(sourceCounter < 0) source = currentSource;
				else source = nestedSource[sourceCounter];
				InMemoryDataElectorContextSource contextSource = source as InMemoryDataElectorContextSource;
				try {
					if(contextSource == null) {
						InMemoryComplexRow row = source as InMemoryComplexRow;
						if(row != null) {
							object result;
							if(GetComplexRowData(alias, theOperand, row, out result)) return result;
						} else {
							IList<InMemoryComplexRow> list = source as IList<InMemoryComplexRow>;
							if(list != null) {
								object result;
								if(GetComplexRowData(alias, theOperand, list[0], out result)) return result;
							}
						}
					} else {
						if(string.Equals(alias, contextSource.NodeAlias)) {
							return contextSource.Row[theOperand.ColumnName];
						} else {
							object result;
							if(GetComplexRowData(alias, theOperand, contextSource.ComplexRowRight, out result)) return result;
							if(GetComplexRowData(alias, theOperand, contextSource.ComplexRow, out result)) return result;
						}
					}
				} catch(InMemorySetException) {
					throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, string.Join(".", new string[] { alias, theOperand.ColumnName })));
				}
				sourceCounter++;
			}
			throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, string.Join(".", new string[] { alias, theOperand.ColumnName })));
		}
		static bool GetComplexRowData(string alias, QueryOperand theOperand, InMemoryComplexRow complexRow, out object result) {
			result = null;
			if(ReferenceEquals(complexRow, null)) return false;
			int tableIndex = complexRow.ComplexSet.GetTableIndex(alias);
			if(tableIndex < 0) {
				tableIndex = complexRow.ComplexSet.GetTableIndex(alias, theOperand.ColumnName);
			}
			if(tableIndex >= 0) {
				IInMemoryRow row = complexRow[tableIndex];
				if(row == null) {
					if(!complexRow.ComplexSet.GetTable(tableIndex).ExistsColumn(theOperand.ColumnName))
						throw new InvalidPropertyPathException(string.Format(CultureInfo.InvariantCulture, FilteringExceptionsText.ExpressionEvaluatorInvalidPropertyPath, string.Join(".", new string[] { alias, theOperand.ColumnName })));
					return true;
				}
				result = row[theOperand.ColumnName];
				return true;
			}
			return false;
		}
		public override object GetQueryResult(JoinNode root) {
			IInMemoryDataElector elector;
			InMemoryPlanCasheItem planCasheItem;
			if(!existsPlanCashe.TryGetValue(root, out planCasheItem)) {
				HashSet<string> nodesSet = new HashSet<string>();
				for(int i = 0; i < nestedSource.Count; i++) {
					InMemoryDataElectorSource.FillNodesDict(nestedSource[i], nodesSet);
				}
				if(ContextDependenceChecker.Process(nodesSet, root)) {
					elector = planner.GetPlan(root, nodesSet);
					planCasheItem = new InMemoryPlanCasheItem(elector);
					existsPlanCashe.Add(root, planCasheItem);
				} else {
					elector = planner.GetPlan(root).Process(this);
					planCasheItem = new InMemoryPlanCasheItem(elector);
					existsPlanCashe.Add(root, planCasheItem);
				}
			} else {
				elector = planCasheItem.Elector;
			}
			return elector.Process(this);
		}
		class InMemoryPlanCasheItem {
			public readonly IInMemoryDataElector Elector;
			public InMemoryPlanCasheItem(IInMemoryDataElector elector) {
				this.Elector = elector;
			}
		}
		public override void PushNestedSource(object source) {
			nestedSource.Add(source);
		}
		public override void PopNestedSource() {
			nestedSource.RemoveAt(nestedSource.Count - 1);
		}
	}
	public interface IInMemoryPlanner {
		IInMemoryDataElector GetPlan(JoinNode root);
		IInMemoryDataElector GetPlan(JoinNode subSelectNode, IEnumerable<string> existsNodeAliases);
		IInMemoryDataElector GetPlan(string alias, InMemoryTable table, CriteriaOperator condition);
	}
	public class PlanAliasCriteriaInfo {
		public readonly string[] Aliases;
		public readonly List<CriteriaOperator> Criteria;
		public PlanAliasCriteriaInfo(string[] aliases, List<CriteriaOperator> criteria) {
			this.Aliases = aliases;
			this.Criteria = criteria;
		}
	}
	public class InMemoryAdvancedPlanner : IInMemoryPlanner {
		InMemorySet dataSet;
		bool caseSensitive;
		bool cashePlans;
		public InMemoryAdvancedPlanner(InMemorySet dataSet) {
			this.dataSet = dataSet;
		}
		public InMemoryAdvancedPlanner(InMemorySet dataSet, bool cashePlans, bool caseSensitive)
			: this(dataSet) {
			this.caseSensitive = caseSensitive;
			this.cashePlans = cashePlans;
		}
		public IInMemoryDataElector GetPlan(JoinNode root) {
			return GetPlanInternal(root, null);
		}
		public IInMemoryDataElector GetPlan(JoinNode subSelectNode, IEnumerable<string> existsNodeAliases) {
			List<string> existsNodeAliasList = existsNodeAliases as List<string>;
			return GetPlanInternal(subSelectNode, existsNodeAliasList == null ? new List<string>(existsNodeAliases) : existsNodeAliasList);
		}
		public IInMemoryDataElector GetPlan(string alias, InMemoryTable table, CriteriaOperator condition) {
			return new InMemoryDataElectorTableSearch(alias, table, condition);
		}
		IInMemoryDataElector GetPlanInternal(JoinNode root, List<string> leftUsedNodes) {
			NodeCriteriaFinder criteriaFinder = new NodeCriteriaFinder();
			Dictionary<string, PlanAliasCriteriaInfo> criteriaDict = new Dictionary<string, PlanAliasCriteriaInfo>();
			Dictionary<string, PlanNodeInfo> nodeDict = new Dictionary<string, PlanNodeInfo>();
			HashSet<string> nodeLeftJoinSet = new HashSet<string>();
			string lastAlias = string.Empty;
			int nodeIndex = 0;
			foreach(JoinNode node in DataSetStoreHelpersFull.GetAllNodes(root)) {
				criteriaFinder.Find(node.Alias, node.Condition, criteriaDict);
				DBProjection projection = node.Table as DBProjection;
				if(projection != null) {
					nodeDict.Add(node.Alias, new PlanNodeInfo(node.Alias, GetPlan(projection.Projection), node, nodeIndex++));
				} else {
					nodeDict.Add(node.Alias, new PlanNodeInfo(node.Alias, GetTable(node.Table.Name), node, nodeIndex++));
				}
				if(node.Type == JoinType.LeftOuter) nodeLeftJoinSet.Add(node.Alias);
				lastAlias = node.Alias;
			}
			if(nodeDict.Count == 1) {
				PlanNodeInfo nodeInfo = nodeDict[lastAlias];
				List<string> oneUsedNodes = leftUsedNodes == null ? new List<string>() : leftUsedNodes;
				oneUsedNodes.Add(nodeInfo.Alias);
				if(nodeInfo.IsSubquery()) {
					return new InMemoryDataElectorResultSearch(nodeInfo.SubqueryElector, TakeCriteria(oneUsedNodes, null, null, criteriaDict, nodeDict));
				} else {
					return new InMemoryDataElectorTableSearch(nodeInfo.Alias, nodeInfo.Table, TakeCriteria(oneUsedNodes, null, null, criteriaDict, nodeDict)); ;
				}
			}
			List<PlanAliasCriteriaInfo> aliasCriteriaDict = new List<PlanAliasCriteriaInfo>();
			foreach(KeyValuePair<string, PlanAliasCriteriaInfo> pair in criteriaDict) {
				aliasCriteriaDict.Add(pair.Value);
			}
			Dictionary<string, List<PlanRelationInfo>> relationDict = FindRelations(nodeDict, aliasCriteriaDict);
			PlanFinder planFinder = new PlanFinder(nodeDict, relationDict);
			string[] mainPath = planFinder.Find();
			if(mainPath == null) throw new InMemorySetException(Res.GetString(Res.InMemoryFull_CannotPrepareQueryPlan));
			IInMemoryDataElector mainElector = null;
			PlanNodeInfo mainPrevLeftInfo = null;
			List<string> mainUsedNodes = leftUsedNodes == null ? new List<string>() : new List<string>(leftUsedNodes);
			foreach(string[] path in BreakPlanPath(mainPath)) {
				if(mainElector != null && path.Length == 1) {
					PlanNodeInfo info = nodeDict[path[0]];
					mainUsedNodes.Add(info.Alias);
					bool isLeftTable = info.IsLeftFor(mainPrevLeftInfo);
					string currentAlias = info.Alias;
					JoinType currentJoinType = info.Node.Type;
					if(isLeftTable) {
						currentAlias = mainPrevLeftInfo.Alias;
						currentJoinType = mainPrevLeftInfo.Node.Type;
						mainPrevLeftInfo = info;
					}
					mainElector = new InMemoryDataElectorTableJoinSearch(info.Alias, info.Table, mainElector, TakeCriteria(mainUsedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType, isLeftTable);
					nodeLeftJoinSet.Remove(currentAlias);
					CriteriaOperator co = TakeCriteria(mainUsedNodes, null, nodeLeftJoinSet, criteriaDict, nodeDict);
					if(!ReferenceEquals(co, null)) mainElector = new InMemoryDataElectorResultSearch(mainElector, co);
					continue;
				}
				IInMemoryDataElector elector = null;
				PlanNodeInfo prevLeftInfo = null;
				List<string> usedNodes;
				if(mainElector == null && leftUsedNodes != null) usedNodes = new List<string>(leftUsedNodes);
				else usedNodes = new List<string>();
				if(path.Length >= 2) {
					PlanNodeInfo info1 = nodeDict[path[path.Length - 1]];
					PlanNodeInfo info2 = nodeDict[path[path.Length - 2]];
					bool isInfo2LeftForInfo1 = info2.IsLeftFor(info1);
					PlanNodeInfo leftInfo = isInfo2LeftForInfo1 ? info2 : info1;
					PlanNodeInfo rightInfo = isInfo2LeftForInfo1 ? info1 : info2;
					bool isSubqueryLeft = leftInfo.IsSubquery();
					bool isSubqueryRight = rightInfo.IsSubquery();
					string currentAlias = rightInfo.Alias;
					JoinType currentJoinType = rightInfo.Node.Type;
					usedNodes.Add(rightInfo.Alias);
					usedNodes.Add(leftInfo.Alias);
					if(!isSubqueryLeft && !isSubqueryRight) {
						elector = new InMemoryDataElectorTablesJoinSearch(leftInfo.Alias, leftInfo.Table, rightInfo.Alias, rightInfo.Table, TakeCriteria(usedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType);
					} else if(!isSubqueryLeft) {
						elector = new InMemoryDataElectorTableJoinSearch(leftInfo.Alias, leftInfo.Table, rightInfo.SubqueryElector, TakeCriteria(usedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType, true);
					} else if(!isSubqueryRight) {
						elector = new InMemoryDataElectorTableJoinSearch(rightInfo.Alias, rightInfo.Table, leftInfo.SubqueryElector, TakeCriteria(usedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType, false);
					} else {
						elector = new InMemoryDataElectorResultJoinSearch(leftInfo.SubqueryElector, rightInfo.SubqueryElector, TakeCriteria(usedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType);
					}
					prevLeftInfo = leftInfo;
					nodeLeftJoinSet.Remove(rightInfo.Alias);
					CriteriaOperator co = TakeCriteria(usedNodes, null, nodeLeftJoinSet, criteriaDict, nodeDict);
					if(!ReferenceEquals(co, null)) elector = new InMemoryDataElectorResultSearch(elector, co);
				}
				for(int n = path.Length - 3; n >= 0; n--) {
					PlanNodeInfo info = nodeDict[path[n]];
					usedNodes.Add(info.Alias);
					bool isLeftTable = info.IsLeftFor(prevLeftInfo);
					string currentAlias = info.Alias;
					JoinType currentJoinType = info.Node.Type;
					if(isLeftTable) {
						currentAlias = prevLeftInfo.Alias;
						currentJoinType = prevLeftInfo.Node.Type;
						prevLeftInfo = info;
					}
					if(info.IsSubquery()) {
						if(isLeftTable) {
							elector = new InMemoryDataElectorResultJoinSearch(info.SubqueryElector, elector, TakeCriteria(usedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType);
						} else {
							elector = new InMemoryDataElectorResultJoinSearch(elector, info.SubqueryElector, TakeCriteria(usedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType);
						}
					} else {
						elector = new InMemoryDataElectorTableJoinSearch(info.Alias, info.Table, elector, TakeCriteria(usedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType, isLeftTable);
					}
					nodeLeftJoinSet.Remove(currentAlias);
					CriteriaOperator co = TakeCriteria(usedNodes, null, nodeLeftJoinSet, criteriaDict, nodeDict);
					if(!ReferenceEquals(co, null)) elector = new InMemoryDataElectorResultSearch(elector, co);
				}
				if(mainElector == null) {
					mainElector = elector;
					mainPrevLeftInfo = prevLeftInfo;
					mainUsedNodes = usedNodes;
				} else {
					mainUsedNodes.AddRange(usedNodes);
					bool isLeftElector = prevLeftInfo.IsLeftFor(mainPrevLeftInfo);
					string currentAlias = prevLeftInfo.Alias;
					JoinType currentJoinType = prevLeftInfo.Node.Type;
					if(isLeftElector) {
						currentAlias = mainPrevLeftInfo.Alias;
						currentJoinType = mainPrevLeftInfo.Node.Type;
						mainPrevLeftInfo = prevLeftInfo;
						mainElector = new InMemoryDataElectorResultJoinSearch(elector, mainElector, TakeCriteria(mainUsedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType);
					} else {
						mainElector = new InMemoryDataElectorResultJoinSearch(mainElector, elector, TakeCriteria(mainUsedNodes, currentAlias, nodeLeftJoinSet, criteriaDict, nodeDict), currentJoinType);
					}
					nodeLeftJoinSet.Remove(currentAlias);
					CriteriaOperator co = TakeCriteria(usedNodes, null, nodeLeftJoinSet, criteriaDict, nodeDict);
					if(!ReferenceEquals(co, null)) mainElector = new InMemoryDataElectorResultSearch(mainElector, co);
				}
			}
			return mainElector;
		}
		static List<string[]> BreakPlanPath(string[] mainPath) {
			List<string[]> pathBreakList = new List<string[]>();
			int start = 0;
			for(int i = 0; i < mainPath.Length; i++) {
				if(mainPath[i] == null && start <= i) {
					if(start < i) {
						string[] buffer1 = new string[i - start];
						Array.Copy(mainPath, start, buffer1, 0, i - start);
						pathBreakList.Add(buffer1);
					}
					start = i + 1;
					string[] buffer2 = new string[1];
					Array.Copy(mainPath, start, buffer2, 0, 1);
					pathBreakList.Add(buffer2);
					start++;
					continue;
				}
				if(i == mainPath.Length - 1 && start <= i) {
					string[] temp = new string[i - start + 1];
					Array.Copy(mainPath, start, temp, 0, i - start + 1);
					pathBreakList.Add(temp);
				}
			}
			return pathBreakList;
		}
		static Dictionary<string, List<PlanRelationInfo>> FindRelations(Dictionary<string, PlanNodeInfo> nodeDict, List<PlanAliasCriteriaInfo> aliasCriteriaDict) {
			Dictionary<string, List<PlanRelationInfo>> relationDict = new Dictionary<string, List<PlanRelationInfo>>();
			foreach(PlanNodeInfo nodeInfo in nodeDict.Values) {
				List<PlanRelationInfo> relationList = new List<PlanRelationInfo>();
				IndexFinder indexFinder = nodeInfo.IsSubquery() ? null : new IndexFinder(nodeInfo.Alias, nodeInfo.Table, true);
				HashSet<string> groupExists = new HashSet<string>();
				foreach(PlanAliasCriteriaInfo aliasCriteriaInfo in aliasCriteriaDict) {
					if(Array.IndexOf(aliasCriteriaInfo.Aliases, nodeInfo.Alias) < 0) continue;
					if(indexFinder != null) {
						foreach(CriteriaOperator op in aliasCriteriaInfo.Criteria) {
							IndexFinderResult indexFinderResult = indexFinder.Find(op);
							if(indexFinderResult == null) continue;
							foreach(IndexFinderItem[] indexResultList in indexFinderResult.Result) {
								List<string> group = null;
								foreach(IndexFinderItem indexFinderItem in indexResultList) {
									if(group == null) group = new List<string>();
									if(indexFinderItem.ValueIsQueryOperand) {
										group.Add(((QueryOperand)indexFinderItem.Value).NodeAlias);
									}
								}
								if(group == null || group.Count == 0) continue;
								string groupKey = string.Join(".", group.ToArray());
								if(groupExists.Contains(groupKey)) continue;
								groupExists.Add(groupKey);
								relationList.Add(new PlanRelationInfo(nodeInfo.Alias, group, (group.Count + 1) * 100));
							}
						}
					}
					List<string> pureGroup = new List<string>(aliasCriteriaInfo.Aliases);
					pureGroup.Remove(nodeInfo.Alias);
					for(int i = pureGroup.Count - 1; i >= 0; i--) {
						string str = pureGroup[i];
						if(str.Length > 0 && str[0] == '*' || !nodeDict.ContainsKey(str)) {
							pureGroup.RemoveAt(i);
						}
					}
					if(pureGroup.Count == 0) continue;
					if(groupExists.Contains(string.Join(".", pureGroup.ToArray()))) continue;
					relationList.Add(new PlanRelationInfo(nodeInfo.Alias, pureGroup, (pureGroup.Count + 1)));
				}
				relationDict.Add(nodeInfo.Alias, relationList);
			}
			return relationDict;
		}
		static CriteriaOperator TakeCriteria(List<string> nodes, string currentAlias, HashSet<string> nodeLeftJoinSet, Dictionary<string, PlanAliasCriteriaInfo> criteriaDict, Dictionary<string, PlanNodeInfo> nodeDict) {
			List<CriteriaOperator> criteriaList = null;
			List<string> deleteList = null;
			foreach(KeyValuePair<string, PlanAliasCriteriaInfo> nodePair in criteriaDict) {
				string[] pairNodes = nodePair.Value.Aliases;
				bool fail = false;
				string starNode = null;
				for(int i = 0; i < pairNodes.Length; i++) {
					string node = pairNodes[i];
					if(node.Length > 0 && node[0] == '*') {
						string nodeAlias = node.Substring(1);
						if(nodeAlias != currentAlias && nodeLeftJoinSet != null && currentAlias != null && nodeLeftJoinSet.Contains(currentAlias)) {
							fail = true;
							break;
						} else {
							starNode = nodeAlias;
							continue;
						}
					}
					if(!nodes.Contains(node)) {
						fail = true;
						break;
					}
				}
				if(fail) continue;
				if(starNode != currentAlias && nodeLeftJoinSet != null && (currentAlias == null || (currentAlias != null && nodeLeftJoinSet.Contains(currentAlias)))) {
					bool allLeftJoin = true;
					foreach(string node in nodes) {
						PlanNodeInfo info;
						if(nodeDict.TryGetValue(node, out info) && info.Node.Type != JoinType.LeftOuter) {
							allLeftJoin = false;
							break;
						}
					}
					if(allLeftJoin) continue;
				}
				if(criteriaList == null) {
					deleteList = new List<string>();
					criteriaList = new List<CriteriaOperator>(nodePair.Value.Criteria);
				} else
					criteriaList.AddRange(nodePair.Value.Criteria);
				deleteList.Add(nodePair.Key);
			}
			if(criteriaList == null || criteriaList.Count == 0) return null;
			if(criteriaList.Count == 1) {
				criteriaDict.Remove(deleteList[0]);
				return criteriaList[0];
			}
			foreach(string deleteString in deleteList) {
				criteriaDict.Remove(deleteString);
			}
			GroupOperator group = new GroupOperator(GroupOperatorType.And, criteriaList);
			return group;
		}
		InMemoryTable GetTable(string tableName) {
			InMemoryTable table = dataSet.GetTable(tableName);
			if(table == null) throw new SchemaCorrectionNeededException(Res.GetString(Res.InMemoryFull_TableNotFound, tableName));
			return table;
		}
		class PlanRelationInfo {
			public readonly string Alone;
			public readonly ReadOnlyCollection<string> InGroup;
			public readonly int Cost;
			public PlanRelationInfo(string alone, List<string> inGroup, int cost) {
				this.Alone = alone;
				this.InGroup = new ReadOnlyCollection<string>(inGroup);
				this.Cost = cost;
			}
			public override string ToString() {
				StringBuilder sb = new StringBuilder();
				sb.Append(Alone);
				sb.Append(" -> (");
				for(int i = 0; i < InGroup.Count; i++) {
					if(i > 0) sb.Append(", ");
					sb.Append(InGroup[i]);
				}
				sb.Append(") = ");
				sb.Append(Cost);
				return sb.ToString();
			}
		}
		class PlanNodeInfo {
			public readonly string Alias;
			public readonly InMemoryTable Table;
			public readonly IInMemoryDataElector SubqueryElector;
			public readonly JoinNode Node;
			public readonly int Index;
			public PlanNodeInfo(string alias, InMemoryTable table, JoinNode node, int index) {
				this.Alias = alias;
				this.Table = table;
				this.Node = node;
				this.Index = index;
			}
			public PlanNodeInfo(string alias, IInMemoryDataElector subqueryElector, JoinNode node, int index) {
				this.Alias = alias;
				this.Node = node;
				this.Index = index;
				this.SubqueryElector = new ProjectionElector(subqueryElector, node.Table as DBProjection, node.Alias);
			}
			public bool IsLeftFor(PlanNodeInfo other) {
				return Index < other.Index;
			}
			public bool IsSubquery() {
				return SubqueryElector != null;
			}
			public override string ToString() {
				if(IsSubquery()) {
					return string.Format(CultureInfo.InvariantCulture, "{0} join ({1}) {2}({3})", Node.Type, Table.ToString(), Alias, Index);
				}
				return string.Format(CultureInfo.InvariantCulture, "{0} join {1} {2}({3})", Node.Type, Table.Name, Alias, Index);
			}
		}
		class ProjectionTable : IInMemoryTable {
			readonly string name;
			readonly List<string> columns;
			readonly Dictionary<string, int> columnDictionary;
			public readonly bool IsTransitive;
			public ProjectionTable(DBProjection projection, string projectionAlias) {
				name = projectionAlias;
				var select = projection.Projection;
				var selectOperands = select.Operands;
				columns = new List<string>(selectOperands.Count);
				columnDictionary = new Dictionary<string, int>(selectOperands.Count);
				IsTransitive = true;
				for(int i = 0; i < selectOperands.Count; i++) {
					var operand = selectOperands[i];
					string column;
					var queryOperand = operand as QueryOperand;
					if(projection.Columns != null && i < projection.Columns.Count) {
						column = projection.Columns[i].Name;
						if(ReferenceEquals(queryOperand, null) || (!ReferenceEquals(queryOperand, null)  && column != queryOperand.ColumnName)) {
							IsTransitive = false;
						}
					} else {
						if(!ReferenceEquals(queryOperand, null)) {
							column = queryOperand.ColumnName;
						} else {
							column = string.Concat("PrP", i.ToString());
							IsTransitive = false;
						}
					}
					columns.Add(column);
					columnDictionary.Add(column, i);
				}
			}
			public string Name {
				get { return name; }
			}
			public bool ExistsColumn(string columnName) {
				return columnDictionary.ContainsKey(columnName);
			}
			public IEnumerable<string> GetColumnNames() {
				return columns.ToArray();
			}
			public bool TryGetColumnIndex(string columnName, out int columnIndex) {
				return columnDictionary.TryGetValue(columnName, out columnIndex);
			}
			public int ColumnsCount{
				get { return columns.Count; }
			}
		}
		class ProjectionRow : IInMemoryRow {
			readonly ProjectionTable table;
			readonly object[] data;
			public ProjectionRow(ProjectionTable table)
				: this(table, new object[table.ColumnsCount]) {
			}
			public ProjectionRow(ProjectionTable table, object[] data) {
				this.table = table;
				this.data = data;
			}
			public IInMemoryTable Table {
				get { return table; }
			}
			public object this[int columnIndex] {
				get { return data[columnIndex]; }
				set { data[columnIndex] = value; }
			}
			public object this[string columnName] {
				get {
					int columnIndex;
					if(!table.TryGetColumnIndex(columnName, out columnIndex)) throw new InMemorySetException(string.Format(Res.GetString(Res.InMemorySet_ColumnNotFound), columnName));
					return data[columnIndex];
				}
				set {
					int columnIndex;
					if(!table.TryGetColumnIndex(columnName, out columnIndex)) throw new InMemorySetException(string.Format(Res.GetString(Res.InMemorySet_ColumnNotFound), columnName));
					data[columnIndex] = value;
				}
			}
			public void BeginEdit() {
				throw new NotSupportedException();
			}
			public void EndEdit() {
				throw new NotSupportedException();
			}
			public void CancelEdit() {
				throw new NotSupportedException();
			}
		}
		class ProjectionElector : IInMemoryDataElector {
			readonly string projectionAlias;
			readonly DBProjection projection;
			readonly IInMemoryDataElector inputData;
			Func<InMemoryComplexSet, InMemoryComplexSet> makeProjectionHandler;
			public ProjectionElector(IInMemoryDataElector inputData, DBProjection projection, string projectionAlias) {
				this.inputData = inputData;
				this.projectionAlias = projectionAlias;
				this.projection = projection;
			}
			static void PrepareRowsBeforeProjection(InMemoryComplexSet rows, SelectStatement select, SortingComparerFull sortingComparer) {
				if(sortingComparer != null)
					rows.Sort(sortingComparer);
				int skipRecords = select.SkipSelectedRecords;
				if(skipRecords > 0) {
					if(skipRecords >= rows.Count)
						rows.Clear();
					else
						rows.RemoveRange(0, skipRecords);
				}
				int topRecords = select.TopSelectedRecords;
				if(topRecords > 0 && topRecords < rows.Count)
					rows.RemoveRange(topRecords, rows.Count - topRecords);
			}
			public InMemoryComplexSet Process(InMemoryDataElectorContextDescriptor descriptor) {
				if(makeProjectionHandler == null) {
					var select = projection.Projection;
					var projectionTable = new ProjectionTable(projection, projectionAlias);
					if(IsTopLevelAggregateCheckerFull.IsGrouped(select)) {
						var groupEvaluators = DataSetStoreHelpersFull.PrepareDataEvaluators(select.GroupProperties, descriptor, descriptor.CaseSensitive, descriptor.CustomFunctions);
						var dataEvaluators = DataSetStoreHelpersFull.PrepareDataEvaluators(select.Operands, descriptor, descriptor.CaseSensitive, descriptor.CustomFunctions);
						var havingEvaluator = DataSetStoreHelpersFull.PrepareDataEvaluator(select.GroupCondition, descriptor, descriptor.CaseSensitive, descriptor.CustomFunctions);
						var sortingComparer = DataSetStoreHelpersFull.PrepareSortingListComparer(select.SortProperties, descriptor, descriptor.CaseSensitive, descriptor.CustomFunctions);
						makeProjectionHandler = (rows) => {
							var groups = DataSetStoreHelpersFull.DoGetGroupedDataCore(groupEvaluators, havingEvaluator, sortingComparer, select.SkipSelectedRecords, select.TopSelectedRecords, rows, false);
							var resultRows = new InMemoryComplexSet();
							resultRows.AddTable(projectionAlias, projectionTable);
							foreach(var group in groups) {
								var data = new ProjectionRow(projectionTable, DataSetStoreHelpersFull.GetResultRow(group, dataEvaluators));
								var resultRow = new InMemoryComplexRow(resultRows);
								resultRow[0] = data;
								resultRows.AddRow(resultRow);
							}
							return resultRows;
						};
					} else {
						ExpressionEvaluator[] dataEvaluators = !projectionTable.IsTransitive ? DataSetStoreHelpersFull.PrepareDataEvaluators(select.Operands, descriptor, descriptor.CaseSensitive, descriptor.CustomFunctions) : null;
						var sortingComparer = DataSetStoreHelpersFull.PrepareSortingComparer(select.SortProperties, descriptor, descriptor.CaseSensitive, descriptor.CustomFunctions);
						if(dataEvaluators == null) {
							makeProjectionHandler = (rows) => {
								PrepareRowsBeforeProjection(rows, select, sortingComparer);
								rows.MakeProjection(projectionAlias);
								return rows;
							};
						} else {
							makeProjectionHandler = (rows) => {
								PrepareRowsBeforeProjection(rows, select, sortingComparer);
								var resultRows = new InMemoryComplexSet();
								resultRows.AddTable(projectionAlias, projectionTable);
								foreach(var row in rows) {
									var data = new ProjectionRow(projectionTable, DataSetStoreHelpersFull.GetResultRow(row, dataEvaluators));
									var resultRow = new InMemoryComplexRow(resultRows);
									resultRow[0] = data;
									resultRows.AddRow(resultRow);
								}
								return resultRows;
							};
						}
					}
				}
				var resultSet = inputData.Process(descriptor);
				return makeProjectionHandler(resultSet);
			}
		}
		class PlanFinder {
			const int maxNodeCountForCalc = 4634;
			int maxPlanCostEqualsCount;
			int resultPlanCost;
			string[] resultPlanPath;
			PlanRelationInfo[] resultPlanInfoPath;
			int resultPlanCostEqualsCounter;
			bool isPrettyGoodResult;
			Dictionary<string, PlanNodeInfo> nodeDict;
			Dictionary<string, List<PlanRelationInfo>> relationDict;
			Dictionary<string, List<PlanRelationInfo>> backRelationDict;
			List<string> currentPlanPath = new List<string>();
			List<PlanRelationInfo> currentPlanInfoPath = new List<PlanRelationInfo>();
			HashSet<string> usedNodesSet = new HashSet<string>();
			HashSet<string> leftNodesSet = new HashSet<string>();
			public PlanFinder(Dictionary<string, PlanNodeInfo> nodeDict, Dictionary<string, List<PlanRelationInfo>> relationDict) {
				this.nodeDict = nodeDict;
				this.relationDict = relationDict;
				backRelationDict = new Dictionary<string, List<PlanRelationInfo>>();
				foreach(KeyValuePair<string, PlanNodeInfo> nodePair in nodeDict) {
					string alias = nodePair.Key;
					PlanNodeInfo nodeInfo = nodePair.Value;
					leftNodesSet.Add(alias);
					List<PlanRelationInfo> backRelations = null;
					foreach(KeyValuePair<string, List<PlanRelationInfo>> pair in relationDict) {
						foreach(PlanRelationInfo relationInfo in pair.Value) {
							if(relationInfo.InGroup.Contains(alias)) {
								if(backRelations == null) backRelations = new List<PlanRelationInfo>();
								backRelations.Add(relationInfo);
							}
						}
					}
					if(backRelations != null) backRelationDict.Add(alias, backRelations);
				}
			}
			public string[] Find() {
				resultPlanCost = 0;
				resultPlanPath = null;
				resultPlanInfoPath = null;
				resultPlanCostEqualsCounter = 0;
				if(nodeDict.Count > maxNodeCountForCalc) {
					maxPlanCostEqualsCount = int.MaxValue;
				} else {
					maxPlanCostEqualsCount = nodeDict.Count * nodeDict.Count * 100;
				}
				isPrettyGoodResult = false;
				foreach(PlanNodeInfo nodeInfo in nodeDict.Values) {
					currentPlanPath.Add(nodeInfo.Alias);
					currentPlanInfoPath.Add(null);
					usedNodesSet.Add(nodeInfo.Alias);
					leftNodesSet.Remove(nodeInfo.Alias);
					FindNext(nodeInfo.Alias);
					leftNodesSet.Add(nodeInfo.Alias);
					usedNodesSet.Remove(nodeInfo.Alias);
					currentPlanInfoPath.RemoveAt(currentPlanInfoPath.Count - 1);
					currentPlanPath.RemoveAt(currentPlanPath.Count - 1);
				}
				return resultPlanPath;
			}
			void FindNext(string node) {
				if(isPrettyGoodResult) return;
				if(usedNodesSet.Count == nodeDict.Count) {
					int currentCost = CalcPathCost();
					if(resultPlanPath == null || currentCost > resultPlanCost) {
						resultPlanCost = currentCost;
						resultPlanPath = currentPlanPath.ToArray();
						resultPlanInfoPath = currentPlanInfoPath.ToArray();
					} else if(currentCost == resultPlanCost) {
						resultPlanCostEqualsCounter++;
						if(resultPlanCostEqualsCounter > maxPlanCostEqualsCount) {
							isPrettyGoodResult = true;
						}
					}
					return;
				}
				List<PlanRelationInfo> relationList = relationDict[node];
				GoNextByRelation(relationList);
				List<string> leftList = new List<string>(leftNodesSet);
				foreach(string leftNode in leftList) {
					List<PlanRelationInfo> backRelationList;
					if(!backRelationDict.TryGetValue(leftNode, out backRelationList)) continue;
					GoNextByBackRelation(backRelationList);
				}
			}
			void GoNextByBackRelation(List<PlanRelationInfo> relationList) {
				if(isPrettyGoodResult) return;
				foreach(PlanRelationInfo relationInfo in relationList) {
					if(!usedNodesSet.Contains(relationInfo.Alone)) continue;
					foreach(string nextNode in relationInfo.InGroup) {
						if(usedNodesSet.Contains(nextNode) || !nodeDict.ContainsKey(nextNode)) continue;
						currentPlanPath.Add(null);
						currentPlanInfoPath.Add(null);
						currentPlanPath.Add(nextNode);
						currentPlanInfoPath.Add(relationInfo);
						usedNodesSet.Add(nextNode);
						leftNodesSet.Remove(nextNode);
						FindNext(nextNode);
						leftNodesSet.Add(nextNode);
						usedNodesSet.Remove(nextNode);
						currentPlanInfoPath.RemoveAt(currentPlanInfoPath.Count - 1);
						currentPlanPath.RemoveAt(currentPlanPath.Count - 1);
						currentPlanPath.RemoveAt(currentPlanPath.Count - 1);
						currentPlanInfoPath.RemoveAt(currentPlanInfoPath.Count - 1);
					}
				}
			}
			void GoNextByRelation(List<PlanRelationInfo> relationList) {
				if(isPrettyGoodResult) return;
				foreach(PlanRelationInfo relationInfo in relationList) {
					bool anyUsed = false;
					foreach(string nextNode in relationInfo.InGroup) {
						if(usedNodesSet.Contains(nextNode) || !nodeDict.ContainsKey(nextNode)) {
							anyUsed = true;
							break;
						}
					}
					if(anyUsed) continue;
					foreach(string nextNode in relationInfo.InGroup) {
						currentPlanPath.Add(nextNode);
						currentPlanInfoPath.Add(relationInfo);
						usedNodesSet.Add(nextNode);
						leftNodesSet.Remove(nextNode);
						FindNext(nextNode);
						leftNodesSet.Add(nextNode);
						usedNodesSet.Remove(nextNode);
						currentPlanInfoPath.RemoveAt(currentPlanInfoPath.Count - 1);
						currentPlanPath.RemoveAt(currentPlanPath.Count - 1);
					}
				}
			}
			int CalcPathCost() {
				int cost = 0;
				foreach(PlanRelationInfo info in currentPlanInfoPath) {
					if(info == null) continue;
					cost += info.Cost;
				}
				return cost;
			}
		}
	}
	public class InMemoryDataElectorTableSearch : IInMemoryDataElector {
		string tableAlias;
		InMemoryTable table;
		CriteriaOperator criteria;
		IndexFinderResult keys;
		InMemoryIndexWrapper[] indexList;
		public InMemoryDataElectorTableSearch(string tableAlias, InMemoryTable table, CriteriaOperator criteria) {
			if(ReferenceEquals(table, null)) throw new NullReferenceException();
			this.tableAlias = tableAlias;
			this.table = table;
			this.criteria = criteria;
			this.keys = new IndexFinder(tableAlias, table).Find(criteria);
			this.indexList = IndexFinder.GetIndexList(table, keys);
		}
		public InMemoryComplexSet Process(InMemoryDataElectorContextDescriptor descriptor) {
			InMemoryComplexSet result = new InMemoryComplexSet();
			ExpressionEvaluator eval = new QuereableExpressionEvaluator(descriptor, criteria, descriptor.CaseSensitive, descriptor.CustomFunctions);
			result.AddTable(tableAlias, table);
			InMemoryRowList rows = table.Rows;
			if(keys == null) {
				for(int i = 0; i < rows.Count; i++) {
					if(rows[i].State != InMemoryItemState.Deleted && eval.Fit(new InMemoryDataElectorContextSource(tableAlias, rows[i], table))) {
						result.AddNewRow(0, rows[i]);
					}
				}
			} else {
				HashSet<InMemoryRow> rowSet = new HashSet<InMemoryRow>();
				for(int j = 0; j < keys.Result.Length; j++) {
					IndexFinderItem[] key = keys.Result[j];
					object[] indexValues = new object[key.Length];
					for(int i = 0; i < key.Length; i++) {
						if(key[i].ValueIsQueryOperand) throw new InMemorySetException(Res.GetString(Res.InMemoryFull_WrongIndexInfo));
						indexValues[i] = key[i].Value;
					}
					InMemoryRow[] findRows = indexList[j].Find(indexValues, false);
					for(int i = 0; i < findRows.Length; i++) {
						InMemoryRow row = findRows[i];
						if(row != null && !rowSet.Contains(row) && eval.Fit(new InMemoryDataElectorContextSource(tableAlias, row, table))) {
							result.AddNewRow(0, row);
							rowSet.Add(row);
						}
					}
				}
			}
			return result;
		}
	}
	public delegate List<InMemoryComplexRow> InMemoryRowFitHandler(InMemoryRow row);
	public delegate List<InMemoryRow> InMemoryRowsFitHandler(InMemoryRow row);
	public delegate List<InMemoryRow> InMemoryComplexRowFitHandler(InMemoryComplexRow resultRow);
	public delegate List<InMemoryComplexRow> InMemoryComplexRowsFitHandler(InMemoryComplexRow resultRow);
	public class InMemoryDataElectorTablesJoinSearch : IInMemoryDataElector {
		string tableLeftAlias;
		InMemoryTable tableLeft;
		string tableRightAlias;
		InMemoryTable tableRight;
		CriteriaOperator criteria;
		JoinType joinType;
		IndexFinderResult keysLeft;
		IndexFinderResult keysRight;
		InMemoryIndexWrapper[] indexListLeft;
		InMemoryIndexWrapper[] indexListRight;
		bool queryOperandDetectedRight;
		bool queryOperandDetectedLeft;
		public InMemoryDataElectorTablesJoinSearch(string tableLeftAlias, InMemoryTable tableLeft, string tableRightAlias, InMemoryTable tableRight, CriteriaOperator criteria, JoinType joinType) {
			if(ReferenceEquals(tableLeft, null) || string.IsNullOrEmpty(tableLeftAlias)) throw new ArgumentNullException();
			this.tableLeftAlias = tableLeftAlias;
			this.tableLeft = tableLeft;
			this.tableRightAlias = tableRightAlias;
			this.tableRight = tableRight;
			this.criteria = criteria;
			this.joinType = joinType;
			this.keysLeft = new IndexFinder(tableLeftAlias, tableLeft, joinType == JoinType.Inner).Find(criteria);
			this.indexListLeft = IndexFinder.GetIndexList(tableLeft, keysLeft);
			queryOperandDetectedLeft = IndexFinder.HasQueryOperand(keysLeft);
			this.keysRight = new IndexFinder(tableRightAlias, tableRight, joinType == JoinType.LeftOuter || !queryOperandDetectedLeft).Find(criteria);
			this.indexListRight = IndexFinder.GetIndexList(tableRight, keysRight);
			queryOperandDetectedRight = IndexFinder.HasQueryOperand(keysRight);
		}
		public InMemoryComplexSet Process(InMemoryDataElectorContextDescriptor descriptor) {
			return joinType == JoinType.LeftOuter ? ProcessIfLeftJoin(descriptor) : ProcessIfInnerJoin(descriptor);
		}
		delegate void ProcessRow(InMemoryRow row);
		InMemoryComplexSet ProcessIfLeftJoin(InMemoryDataElectorContextDescriptor descriptor) {
			if(queryOperandDetectedLeft)
				throw new InMemorySetException(Res.GetString(Res.InMemoryFull_WrongIndexInfo));
			ExpressionEvaluator eval = new QuereableExpressionEvaluator(descriptor, criteria, descriptor.CaseSensitive, descriptor.CustomFunctions);
			InMemoryComplexSet resultSet = new InMemoryComplexSet();
			int tableLeftIndex = resultSet.AddTable(tableLeftAlias, tableLeft);
			int tableRightIndex = resultSet.AddTable(tableRightAlias, tableRight);
			IEnumerable<InMemoryRow> leftRows = GetRows(keysLeft, indexListLeft, tableLeft);
			if(!queryOperandDetectedRight) {
				IEnumerable<InMemoryRow> rightRows = GetRows(keysRight, indexListRight, tableRight);
				foreach(InMemoryRow leftRow in leftRows) {
					bool hasFit = false;
					foreach(InMemoryRow rightRow in rightRows)
						if(AddRow(eval, resultSet, tableLeftIndex, tableRightIndex, leftRow, rightRow))
							hasFit = true;
					if(!hasFit)
						resultSet.AddNewRow(tableLeftIndex, leftRow);
				}
			} else {
				foreach(InMemoryRow leftRow in leftRows) {
					bool hasFit = false;
					GetRows(keysRight, indexListRight, tableRight, descriptor, new InMemoryDataElectorContextSource(tableLeftAlias, leftRow, tableLeft), delegate(InMemoryRow rightRow) {
						if(AddRow(eval, resultSet, tableLeftIndex, tableRightIndex, leftRow, rightRow))
							hasFit = true;
					});
					if(!hasFit)
						resultSet.AddNewRow(tableLeftIndex, leftRow);
				}
			}
			return resultSet;
		}
		static bool AddRow(ExpressionEvaluator eval, InMemoryComplexSet resultSet, int tableLeftIndex, int tableRightIndex, InMemoryRow leftRow, InMemoryRow rightRow) {
			InMemoryComplexRow complexRow = new InMemoryComplexRow(resultSet);
			complexRow[tableLeftIndex] = leftRow;
			complexRow[tableRightIndex] = rightRow;
			if(!eval.Fit(complexRow))
				return false;
			resultSet.AddRow(complexRow);
			return true;
		}
		static IEnumerable<InMemoryRow> GetRows(IndexFinderResult keys, InMemoryIndexWrapper[] indexList, InMemoryTable table) {
			if(keys == null)
				return table.Rows;
			HashSet<InMemoryRow> rowSet = new HashSet<InMemoryRow>();
			for(int j = 0; j < keys.Result.Length; j++) {
				IndexFinderItem[] key = keys.Result[j];
				object[] keyValues = new object[key.Length];
				for(int i = 0; i < key.Length; i++) {
					IndexFinderItem item = key[i];
					keyValues[i] = item.Value;
				}
				InMemoryRow[] currentRows = indexList[j].Find(keyValues, false);
				for(int i = 0; i < currentRows.Length; i++) {
					InMemoryRow row = currentRows[i];
					if(!rowSet.Contains(row)) {
						rowSet.Add(row);
					}
				}
			}
			return rowSet;
		}
		static void GetRows(IndexFinderResult keys, InMemoryIndexWrapper[] indexList, InMemoryTable table, QuereableEvaluatorContextDescriptor descriptor,
			InMemoryDataElectorContextSource source, ProcessRow process) {
			HashSet<InMemoryRow> rowSet;
			if(keys.Result.Length > 1)
				rowSet = new HashSet<InMemoryRow>();
			else
				rowSet = null;
			for(int j = 0; j < keys.Result.Length; j++) {
				IndexFinderItem[] key = keys.Result[j];
				object[] keyValues = new object[key.Length];
				for(int i = 0; i < key.Length; i++) {
					IndexFinderItem item = key[i];
					if(item.ValueIsQueryOperand)
						keyValues[i] = descriptor.GetOperandValue(source, (QueryOperand)item.Value);
					else
						keyValues[i] = item.Value;
				}
				InMemoryRow[] currentRows = indexList[j].Find(keyValues, false);
				for(int i = 0; i < currentRows.Length; i++) {
					InMemoryRow row = currentRows[i];
					if(rowSet == null)
						process(row);
					else if(!rowSet.Contains(row)) {
						rowSet.Add(row);
						process(row);
					}
				}
			}
		}
		InMemoryComplexSet ProcessIfInnerJoin(InMemoryDataElectorContextDescriptor descriptor) {
			if(queryOperandDetectedLeft && queryOperandDetectedRight)
				throw new InMemorySetException(Res.GetString(Res.InMemoryFull_WrongIndexInfo));
			ExpressionEvaluator eval = new QuereableExpressionEvaluator(descriptor, criteria, descriptor.CaseSensitive, descriptor.CustomFunctions);
			InMemoryComplexSet resultSet = new InMemoryComplexSet();
			int tableLeftIndex = resultSet.AddTable(tableLeftAlias, tableLeft);
			int tableRightIndex = resultSet.AddTable(tableRightAlias, tableRight);
			if(queryOperandDetectedLeft) {
				IEnumerable<InMemoryRow> rightRows = GetRows(keysRight, indexListRight, tableRight);
				foreach(InMemoryRow rightRow in rightRows) {
					GetRows(keysLeft, indexListLeft, tableLeft, descriptor, new InMemoryDataElectorContextSource(tableRightAlias, rightRow, tableRight), delegate(InMemoryRow leftRow) {
						AddRow(eval, resultSet, tableLeftIndex, tableRightIndex, leftRow, rightRow);
					});
				}
			} else {
				if(queryOperandDetectedRight) {
					IEnumerable<InMemoryRow> leftRows = GetRows(keysLeft, indexListLeft, tableLeft);
					foreach(InMemoryRow leftRow in leftRows) {
						GetRows(keysRight, indexListRight, tableRight, descriptor, new InMemoryDataElectorContextSource(tableLeftAlias, leftRow, tableLeft), delegate(InMemoryRow rightRow) {
							AddRow(eval, resultSet, tableRightIndex, tableLeftIndex, rightRow, leftRow);
						});
					}
				} else {
					IEnumerable<InMemoryRow> rightRows = GetRows(keysRight, indexListRight, tableRight);
					IEnumerable<InMemoryRow> leftRows = GetRows(keysLeft, indexListLeft, tableLeft);
					foreach(InMemoryRow rightRow in rightRows) {
						foreach(InMemoryRow leftRow in leftRows)
							AddRow(eval, resultSet, tableLeftIndex, tableRightIndex, leftRow, rightRow);
					}
				}
			}
			return resultSet;
		}
	}
	public class InMemoryDataElectorTableJoinSearch : IInMemoryDataElector {
		bool isTableLeft;
		string tableAlias;
		InMemoryTable table;
		IInMemoryDataElector inputData;
		CriteriaOperator criteria;
		JoinType joinType;
		IndexFinderResult keys;
		InMemoryIndexWrapper[] indexList;
		public InMemoryDataElectorTableJoinSearch(string tableAlias, InMemoryTable table, IInMemoryDataElector inputData, CriteriaOperator criteria, JoinType joinType, bool isTableLeft) {
			if(ReferenceEquals(table, null) || string.IsNullOrEmpty(tableAlias)) throw new ArgumentNullException();
			this.tableAlias = tableAlias;
			this.table = table;
			this.inputData = inputData;
			this.criteria = criteria;
			this.joinType = joinType;
			this.isTableLeft = isTableLeft;
			bool searchOperandProperties = !isTableLeft || joinType == JoinType.Inner;
			this.keys = new IndexFinder(tableAlias, table, searchOperandProperties).Find(criteria);
			this.indexList = IndexFinder.GetIndexList(table, keys);
		}
		public InMemoryComplexSet Process(InMemoryDataElectorContextDescriptor descriptor) {
			if(isTableLeft && joinType == JoinType.LeftOuter) return ProcessIfTableLeft(descriptor);
			return ProcessIfTableRightOrInnerJoin(descriptor);
		}
		InMemoryComplexSet ProcessIfTableLeft(InMemoryDataElectorContextDescriptor descriptor) {
			InMemoryComplexSet resultSet = inputData.Process(descriptor);
			ExpressionEvaluator eval = new QuereableExpressionEvaluator(descriptor, criteria, descriptor.CaseSensitive, descriptor.CustomFunctions);
			int tableIndex = resultSet.AddTable(tableAlias, table);
			IEnumerable<InMemoryRow> rows;
			int rowsCount;
			if(keys == null) {
				rows = table.Rows;
				rowsCount = table.Rows.Count;
			} else {
				HashSet<InMemoryRow> rowSet = new HashSet<InMemoryRow>();
				for(int j = 0; j < keys.Result.Length; j++) {
					IndexFinderItem[] key = keys.Result[j];
					InMemoryIndexWrapper index = indexList[j];
					object[] keyValues = new object[key.Length];
					for(int i = 0; i < key.Length; i++) {
						if (key[i].ValueIsQueryOperand) throw new InMemorySetException(Res.GetString(Res.InMemoryFull_WrongIndexInfo));
						keyValues[i] = key[i].Value;
					}
					InMemoryRow[] currentRows = index.Find(keyValues, false);
					for(int i = 0; i < currentRows.Length; i++) {
						InMemoryRow row = currentRows[i];
						if(!rowSet.Contains(row)) {
							rowSet.Add(row);
						}
					}
				}
				rows = rowSet;
				rowsCount = rowSet.Count;
			}
			ComplexSetFitIfTableLeft(resultSet, rows, rowsCount, tableIndex, delegate(InMemoryRow tableRow) {
				List<InMemoryComplexRow> fitRows = null;
				for(int j = 0; j < resultSet.Count; j++) {
					InMemoryComplexRow complexRow = resultSet[j];
					if(eval.Fit(new InMemoryDataElectorContextSource(tableAlias, tableRow, table, complexRow))) {
						if(fitRows == null) fitRows = new List<InMemoryComplexRow>();
						fitRows.Add(complexRow);
					}
				}
				return fitRows;
			});
			return resultSet;
		}
		InMemoryComplexSet ProcessIfTableRightOrInnerJoin(InMemoryDataElectorContextDescriptor descriptor) {
			InMemoryComplexSet resultSet = inputData.Process(descriptor);
			ExpressionEvaluator eval = new QuereableExpressionEvaluator(descriptor, criteria, descriptor.CaseSensitive, descriptor.CustomFunctions);
			int tableIndex = resultSet.AddTable(tableAlias, table);
			InMemoryRowList rows = table.Rows;
			if(keys == null) {
				ComplexSetFitIfTableRight(resultSet, tableIndex, delegate(InMemoryComplexRow resultRow) {
					List<InMemoryRow> fitRows = null;
					for(int i = 0; i < rows.Count; i++) {
						if(rows[i].State != InMemoryItemState.Deleted && eval.Fit(new InMemoryDataElectorContextSource(tableAlias, rows[i], table, resultRow))) {
							if(fitRows == null) fitRows = new List<InMemoryRow>();
							fitRows.Add(rows[i]);
						}
					}
					return fitRows;
				});
			} else {
				ComplexSetFitIfTableRight(resultSet, tableIndex, delegate(InMemoryComplexRow resultRow) {
					List<InMemoryRow> fitRows = null;
					HashSet<InMemoryRow> rowSet = new HashSet<InMemoryRow>();
					for(int j = 0; j < keys.Result.Length; j++) {
						IndexFinderItem[] key = keys.Result[j];
						InMemoryIndexWrapper index = indexList[j];
						object[] keyValues = new object[key.Length];
						for(int i = 0; i < key.Length; i++) {
							keyValues[i] = ProcessKey(descriptor, key[i], resultRow);
						}
						InMemoryRow[] currentRows = index.Find(keyValues, false);
						for(int i = 0; i < currentRows.Length; i++) {
							InMemoryRow row = currentRows[i];
							if(row != null && !rowSet.Contains(row)
										&& eval.Fit(new InMemoryDataElectorContextSource(tableAlias, row, table, resultRow))) {
								if(fitRows == null) fitRows = new List<InMemoryRow>();
								fitRows.Add(row);
								rowSet.Add(row);
							}
						}
					}
					return fitRows;
				});
			}
			return resultSet;
		}
		object ProcessKey(InMemoryDataElectorContextDescriptor descriptor, IndexFinderItem keyItem, InMemoryComplexRow resultRow) {
			if(keyItem.ValueIsQueryOperand) {
				return descriptor.GetOperandValue(resultRow, (QueryOperand)keyItem.Value);
			} else
				return keyItem.Value;
		}
		void ComplexSetFitIfTableLeft(InMemoryComplexSet resultSet, IEnumerable<InMemoryRow> rows, int rowsCount, int tableIndex, InMemoryRowFitHandler fit) {
			HashSet<InMemoryComplexRow> complexRowSet = new HashSet<InMemoryComplexRow>();
			List<InMemoryComplexRow> insertList = new List<InMemoryComplexRow>(rowsCount);
			foreach(InMemoryRow resultRow in rows) {
				if(resultRow.State == InMemoryItemState.Deleted) continue;
				List<InMemoryComplexRow> fitRows = fit(resultRow);
				if(fitRows == null) {
					InMemoryComplexRow newRow = new InMemoryComplexRow(resultSet);
					newRow[tableIndex] = resultRow;
					insertList.Add(newRow);
				} else {
					for(int i = fitRows.Count - 1; i >= 0; i--) {
						InMemoryComplexRow fitRow = fitRows[i];
						if(i == 0 && !complexRowSet.Contains(fitRow)) {
							fitRow[tableIndex] = resultRow;
							insertList.Add(fitRow);
							complexRowSet.Add(fitRow);
							break;
						}
						InMemoryComplexRow newRow = new InMemoryComplexRow(fitRow);
						newRow[tableIndex] = resultRow;
						insertList.Add(newRow);
					}
				}
			}
			resultSet.Clear();
			resultSet.AddRows(insertList);
		}
		void ComplexSetFitIfTableRight(InMemoryComplexSet resultSet, int tableIndex, InMemoryComplexRowFitHandler fit) {
			int deleteFirstIndex = 0;
			int deleteCount = 0;
			List<InMemoryComplexRow> insertList = new List<InMemoryComplexRow>();
			for(int j = 0; j < resultSet.Count; j++) {
				InMemoryComplexRow resultRow = resultSet[j];
				List<InMemoryRow> fitRows = fit(resultRow);
				if(fitRows == null) {
					if(joinType == JoinType.Inner) {
						if(deleteCount == 0) deleteFirstIndex = j;
						deleteCount++;
					}
				} else {
					if(deleteCount > 0) {
						if(deleteCount == 1) {
							resultSet.RemoveAt(deleteFirstIndex);
							j--;
						} else {
							resultSet.RemoveRange(deleteFirstIndex, deleteCount);
							j -= deleteCount;
						}
						deleteCount = 0;
					}
					for(int i = fitRows.Count - 1; i >= 0; i--) {
						if(i == 0) {
							resultRow[tableIndex] = fitRows[i];
							break;
						}
						InMemoryComplexRow newRow = new InMemoryComplexRow(resultRow);
						newRow[tableIndex] = fitRows[i];
						insertList.Add(newRow);
					}
				}
			}
			if(deleteCount > 0) {
				if(deleteCount == 1) resultSet.RemoveAt(deleteFirstIndex);
				else resultSet.RemoveRange(deleteFirstIndex, deleteCount);
			}
			resultSet.AddRows(insertList);
		}
	}
	public class InMemoryDataElectorResultJoinSearch : IInMemoryDataElector {
		IInMemoryDataElector inputDataLeft;
		IInMemoryDataElector inputDataRight;
		CriteriaOperator criteria;
		JoinType joinType;
		public InMemoryDataElectorResultJoinSearch(IInMemoryDataElector inputDataLeft, IInMemoryDataElector inputDataRight, CriteriaOperator criteria, JoinType joinType) {
			this.inputDataLeft = inputDataLeft;
			this.inputDataRight = inputDataRight;
			this.criteria = criteria;
			this.joinType = joinType;
		}
		public InMemoryComplexSet Process(InMemoryDataElectorContextDescriptor descriptor) {
			InMemoryComplexSet resultSetLeft = inputDataLeft.Process(descriptor);
			InMemoryComplexSet resultSetRight = inputDataRight.Process(descriptor);
			ExpressionEvaluator eval = new QuereableExpressionEvaluator(descriptor, criteria, descriptor.CaseSensitive, descriptor.CustomFunctions);
			int leftOldTableCount = resultSetLeft.TableCount;
			int rightTableCount = resultSetRight.TableCount;
			ReadOnlyCollection<string> aliasRightList = resultSetRight.GetAliasList();
			ReadOnlyCollection<IInMemoryTable> tableRightList = resultSetRight.GetTableList();
			for(int i = 0; i < aliasRightList.Count; i++) {
				resultSetLeft.AddTable(aliasRightList[i], tableRightList[i]);
			}
			int deleteFirstIndex = 0;
			int deleteCount = 0;
			List<InMemoryComplexRow> insertList = new List<InMemoryComplexRow>();
			for(int j = 0; j < resultSetLeft.Count; j++) {
				InMemoryComplexRow leftRow = resultSetLeft[j];
				List<InMemoryComplexRow> fitRightRows = null;
				for(int i = 0; i < resultSetRight.Count; i++) {
					if(!eval.Fit(new InMemoryDataElectorContextSource(leftRow, resultSetRight[i]))) continue;
					if(fitRightRows == null) fitRightRows = new List<InMemoryComplexRow>();
					fitRightRows.Add(resultSetRight[i]);
				}
				if(fitRightRows == null) {
					if(joinType == JoinType.Inner) {
						if(deleteCount == 0) deleteFirstIndex = j;
						deleteCount++;
					}
				} else {
					if(deleteCount > 0) {
						if(deleteCount == 1) {
							resultSetLeft.RemoveAt(deleteFirstIndex);
							j--;
						} else {
							resultSetLeft.RemoveRange(deleteFirstIndex, deleteCount);
							j -= deleteCount;
						}
						deleteCount = 0;
					}
					for(int i = fitRightRows.Count - 1; i >= 0; i--) {
						if(i == 0) {
							AddRowToRow(leftRow, fitRightRows[i], leftOldTableCount, rightTableCount);
							break;
						}
						InMemoryComplexRow newRow = new InMemoryComplexRow(leftRow);
						AddRowToRow(newRow, fitRightRows[i], leftOldTableCount, rightTableCount);
						insertList.Add(newRow);
					}
				}
			}
			if(deleteCount > 0) {
				if(deleteCount == 1) resultSetLeft.RemoveAt(deleteFirstIndex);
				else resultSetLeft.RemoveRange(deleteFirstIndex, deleteCount);
			}
			resultSetLeft.AddRows(insertList);
			return resultSetLeft;
		}
		static void AddRowToRow(InMemoryComplexRow leftRow, InMemoryComplexRow rightRow, int leftCount, int rightCount) {
			for(int i = 0; i < rightCount; i++) {
				leftRow[leftCount + i] = rightRow[i];
			}
		}
	}
	public class InMemoryDataElectorSource : IInMemoryDataElector {
		const string WrongSource = "Wrong source.";
		object source;
		public object Source {
			get { return source; }
			set { source = value; }
		}
		public InMemoryDataElectorSource(object source) {
			this.source = source;
		}
		public static void FillNodesDict(object source, HashSet<string> nodesSet) {
			List<InMemoryComplexRow> list = source as List<InMemoryComplexRow>;
			if(list != null) {
				if(list.Count > 0) {
					foreach(string alias in list[0].ComplexSet.GetAliasList()) {
						nodesSet.Add(alias);
					}
				}
				return;
			}
			InMemoryComplexRow complexRow = source as InMemoryComplexRow;
			if(complexRow != null) {
				foreach(string alias in complexRow.ComplexSet.GetAliasList()) {
					nodesSet.Add(alias);
				}
				return;
			}
			InMemoryDataElectorContextSource contextSource = source as InMemoryDataElectorContextSource;
			if(contextSource != null) {
				if(contextSource.ComplexRow != null) {
					ReadOnlyCollection<string> aliasList = contextSource.ComplexRow.ComplexSet.GetAliasList();
					foreach(string alias in aliasList) {
						nodesSet.Add(alias);
					}
				}
				if(contextSource.ComplexRowRight != null) {
					ReadOnlyCollection<string> aliasList = contextSource.ComplexRowRight.ComplexSet.GetAliasList();
					foreach(string alias in aliasList) {
						nodesSet.Add(alias);
					}
				}
				if(contextSource.NodeAlias != null) nodesSet.Add(contextSource.NodeAlias);
				return;
			}
			throw new ArgumentException(WrongSource);
		}
		public InMemoryComplexSet Process(InMemoryDataElectorContextDescriptor descriptor) {
			if(source == null) return new InMemoryComplexSet();
			List<InMemoryComplexRow> list = source as List<InMemoryComplexRow>;
			if(list != null) {
				return ProcessComplexRowList(list);
			}
			InMemoryComplexRow complexRow = source as InMemoryComplexRow;
			if(complexRow != null) {
				return ProcessComplexRow(complexRow);
			}
			InMemoryDataElectorContextSource contextSource = source as InMemoryDataElectorContextSource;
			if(contextSource != null) {
				return ProcessContextSource(contextSource);
			}
			throw new InvalidOperationException(WrongSource);
		}
		static InMemoryComplexSet ProcessContextSource(InMemoryDataElectorContextSource source) {
			if(source.ComplexRow != null) {
				InMemoryComplexSet resultSet = ProcessComplexRow(source.ComplexRow);
				if(source.ComplexRowRight != null) {
					InMemoryComplexSet inputSetRight = source.ComplexRowRight.ComplexSet;
					int tableCount = inputSetRight.TableCount;
					ReadOnlyCollection<string> aliasList = inputSetRight.GetAliasList();
					ReadOnlyCollection<IInMemoryTable> tableList = inputSetRight.GetTableList();
					List<int> tableIndexList = new List<int>();
					for(int i = 0; i < tableCount; i++) {
						tableIndexList.Add(resultSet.AddTableIfNotExists(aliasList[i], tableList[i]));
					}
					for(int i = 0; i < tableCount; i++) {
						resultSet[0][tableIndexList[i]] = source.ComplexRowRight[i];
					}
				} else if(source.NodeAlias != null) {
					int tableIndex = resultSet.AddTableIfNotExists(source.NodeAlias, source.Table);
					resultSet[0][tableIndex] = source.Row;
				}
				return resultSet;
			} else {
				if(source.NodeAlias != null) {
					InMemoryComplexSet resultSet = new InMemoryComplexSet(1);
					resultSet.AddTable(source.NodeAlias, source.Table);
					resultSet.AddNewRow(0, source.Row);
					return resultSet;
				}
			}
			throw new InvalidOperationException(WrongSource);
		}
		static InMemoryComplexSet ProcessComplexRow(InMemoryComplexRow row) {
			InMemoryComplexSet resultSet = new InMemoryComplexSet(1);
			InMemoryComplexSet inputSet = row.ComplexSet;
			ReadOnlyCollection<string> aliasList = inputSet.GetAliasList();
			ReadOnlyCollection<IInMemoryTable> tableList = inputSet.GetTableList();
			for(int i = 0; i < aliasList.Count; i++) {
				resultSet.AddTable(aliasList[i], tableList[i]);
			}
			resultSet.AddRow(new InMemoryComplexRow(resultSet, row));
			return resultSet;
		}
		static InMemoryComplexSet ProcessComplexRowList(List<InMemoryComplexRow> list) {
			InMemoryComplexSet resultSet = new InMemoryComplexSet(list.Count);
			InMemoryComplexSet inputSet = list[0].ComplexSet;
			ReadOnlyCollection<string> aliasList = inputSet.GetAliasList();
			ReadOnlyCollection<IInMemoryTable> tableList = inputSet.GetTableList();
			for(int i = 0; i < aliasList.Count; i++) {
				resultSet.AddTable(aliasList[i], tableList[i]);
			}
			for(int i = 0; i < list.Count; i++) {
				resultSet.AddRow(new InMemoryComplexRow(resultSet, list[i]));
			}
			return resultSet;
		}
	}
	public class InMemoryDataElectorResultSearch : IInMemoryDataElector {
		IInMemoryDataElector inputData;
		CriteriaOperator criteria;
		public InMemoryDataElectorResultSearch(IInMemoryDataElector inputData, CriteriaOperator criteria) {
			this.inputData = inputData;
			this.criteria = criteria;
		}
		public InMemoryComplexSet Process(InMemoryDataElectorContextDescriptor descriptor) {
			InMemoryComplexSet resultSet = inputData.Process(descriptor);
			ExpressionEvaluator eval = new QuereableExpressionEvaluator(descriptor, criteria, descriptor.CaseSensitive, descriptor.CustomFunctions);
			int deleteFirstIndex = 0;
			int deleteCount = 0;
			for(int j = 0; j < resultSet.Count; j++) {
				if(!eval.Fit(resultSet[j])) {
					if(deleteCount == 0) deleteFirstIndex = j;
					deleteCount++;
				} else {
					if(deleteCount > 0) {
						if(deleteCount == 1) {
							resultSet.RemoveAt(deleteFirstIndex);
							j--;
						} else {
							resultSet.RemoveRange(deleteFirstIndex, deleteCount);
							j -= deleteCount;
						}
						deleteCount = 0;
					}
				}
			}
			if(deleteCount > 0) {
				if(deleteCount == 1) resultSet.RemoveAt(deleteFirstIndex);
				else resultSet.RemoveRange(deleteFirstIndex, deleteCount);
			}
			return resultSet;
		}
	}
	public class QueryParamsReprocessor : IQueryCriteriaVisitor<CriteriaOperator> {
		TaggedParametersHolder identitiesByTag;
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(OperandValue theOperand) {
			return identitiesByTag.ConsolidateParameter(theOperand);
		}
		CriteriaOperator IQueryCriteriaVisitor<CriteriaOperator>.Visit(QueryOperand operand) {
			return operand;
		}
		CriteriaOperator IQueryCriteriaVisitor<CriteriaOperator>.Visit(QuerySubQueryContainer container) {
			return container;
		}
		public static CriteriaOperator ReprocessCriteria(CriteriaOperator op, TaggedParametersHolder identitiesByTag) {
			return new QueryParamsReprocessor().Reprocess(op, identitiesByTag);
		}
		public CriteriaOperator Reprocess(CriteriaOperator op, TaggedParametersHolder identitiesByTag) {
			this.identitiesByTag = identitiesByTag;
			return Process(op);
		}
		CriteriaOperator Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return null;
			return op.Accept(this);
		}
		CriteriaOperator[] Process(CriteriaOperatorCollection operands) {
			CriteriaOperator[] newOperands = new CriteriaOperator[operands.Count];
			int count = newOperands.Length;
			for(int i = 0; i < count; ++i) {
				newOperands[i] = Process((CriteriaOperator)operands[i]);
			}
			return newOperands;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BetweenOperator theOperator) {
			return new BetweenOperator(Process(theOperator.TestExpression), Process(theOperator.BeginExpression), Process(theOperator.EndExpression));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BinaryOperator theOperator) {
			return new BinaryOperator(Process(theOperator.LeftOperand), Process(theOperator.RightOperand), theOperator.OperatorType);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(InOperator theOperator) {
			return new InOperator(Process(theOperator.LeftOperand), Process(theOperator.Operands));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(GroupOperator theOperator) {
			return GroupOperator.Combine(theOperator.OperatorType, Process(theOperator.Operands));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(UnaryOperator theOperator) {
			return new UnaryOperator(theOperator.OperatorType, Process(theOperator.Operand));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(FunctionOperator theOperator) {
			return new FunctionOperator(theOperator.OperatorType, Process(theOperator.Operands));
		}
	}
}
namespace DevExpress.Xpo.DB {
	using DevExpress.Xpo.DB.Helpers;
	using System.IO;
	using System.Collections.Generic;
	using DevExpress.Xpo.Helpers;
	using System.Xml;
	using DevExpress.Xpo.Logger;
	public class InMemoryDataStore : DataStoreBase, IDataStoreSchemaExplorer, IDataStoreSchemaExplorerSp {
		public const string XpoProviderTypeString = "InMemoryDataStore";
#if !SL
		public static string GetConnectionString(string path) {
			return String.Format("{0}={1};data source={2};", DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, path);
		}
		public static string GetConnectionString(string path, bool readOnly) {
			return String.Format("{0}={1};data source={2};read only={3}", DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, path, readOnly);
		}
		public static string GetConnectionStringInMemory(bool caseSensitive) {
			return String.Format("{0}={1};case sensitive={2}", DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString, caseSensitive);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			ConnectionStringParser parser = new ConnectionStringParser(connectionString);
			string path = parser.GetPartByName("data source");
			if(string.IsNullOrEmpty(path)) {
				objectsToDisposeOnDisconnect = new IDisposable[0];
				string caseSensitive = parser.GetPartByName("case sensitive");
				bool isCaseSensitive = caseSensitive == "1" || caseSensitive.ToLower() == "true";
				if(string.IsNullOrEmpty(caseSensitive)) {
					return new InMemoryDataStore(autoCreateOption);
				}
				return new InMemoryDataStore(autoCreateOption, isCaseSensitive);
			}
			string readOnly = parser.GetPartByName("read only");
			bool isReadOnly = readOnly == "1" || readOnly.ToLower() == "true";
			try {
				XmlFileDataStore result = new XmlFileDataStore(path, autoCreateOption, isReadOnly);
				objectsToDisposeOnDisconnect = new IDisposable[] { result };
				return result;
			} catch(Exception e) {
				throw new UnableToOpenDatabaseException(connectionString, (e is UnableToOpenDatabaseException) ? e.InnerException : e);
			}
		}
		static InMemoryDataStore() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterFactory(new InMemoryProviderFactory());
		}
		public static void Register() { }
#endif
		bool caseSensitive;
		readonly InMemorySet InMemorySet;
		readonly IInMemoryPlanner planner;
#if !SL
	[DevExpressXpoLocalizedDescription("InMemoryDataStoreCaseSensitive")]
#endif
		public bool CaseSensitive { get { return caseSensitive; } }
#if !SL
	[DevExpressXpoLocalizedDescription("InMemoryDataStoreCanCreateSchema")]
#endif
		public bool CanCreateSchema { get { return AutoCreateOption == AutoCreateOption.SchemaOnly || AutoCreateOption == AutoCreateOption.DatabaseAndSchema; } }
#if !SL
	[DevExpressXpoLocalizedDescription("InMemoryDataStoreSyncRoot")]
#endif
		public override object SyncRoot { get { return this.InMemorySet; } }
		public InMemoryDataStore() : this(AutoCreateOption.DatabaseAndSchema) { }
		public InMemoryDataStore(AutoCreateOption autoCreateOption) : this(autoCreateOption, true) { }
		public InMemoryDataStore(AutoCreateOption autoCreateOption, bool caseSensitive)
			: base(autoCreateOption) {
			this.InMemorySet = new InMemorySet(caseSensitive);
			this.planner = new InMemoryAdvancedPlanner(InMemorySet, true, caseSensitive);
			this.caseSensitive = caseSensitive;
		}
		public InMemoryDataStore(InMemoryDataStore originalStore, AutoCreateOption autoCreateOption)
			: base(autoCreateOption) {
			this.InMemorySet = originalStore.InMemorySet;
			this.planner = originalStore.planner;
			this.caseSensitive = originalStore.CaseSensitive;
		}
		protected override UpdateSchemaResult ProcessUpdateSchema(bool skipIfFirstTableNotExists, params DBTable[] tables) {
			return LogManager.Log<UpdateSchemaResult>(LogManager.LogCategorySQL, () => {
				if(skipIfFirstTableNotExists && tables.Length > 0) {
					IEnumerator te = tables.GetEnumerator();
					te.MoveNext();
					if(DataSetStoreHelpersFull.QueryTable(InMemorySet, ((DBTable)te.Current).Name) == null)
						return UpdateSchemaResult.FirstTableNotExists;
				}
				if(CanCreateSchema) {
					foreach(DBTable table in tables) {
						InMemoryTable newTable = DataSetStoreHelpersFull.CreateIfNotExists(InMemorySet, table);
						foreach(DBColumn column in table.Columns) {
							DataSetStoreHelpersFull.CreateIfNotExists(newTable, column);
						}
						if(table.PrimaryKey != null)
							DataSetStoreHelpersFull.CreateIfNotExists(newTable, table.PrimaryKey);
						foreach(DBIndex index in table.Indexes) {
							DataSetStoreHelpersFull.CreateIfNotExists(newTable, index);
						}
					}
					foreach(DBTable table in tables) {
						InMemoryTable newTable = DataSetStoreHelpersFull.GetTable(InMemorySet, table.Name);
						foreach(DBForeignKey fk in table.ForeignKeys) {
							DataSetStoreHelpersFull.CreateIfNotExists(newTable, fk);
						}
					}
				}
				return UpdateSchemaResult.SchemaExists;
			}, (d) => {
				return LogMessage.CreateMessage(this, string.Concat("UpdateSchema: ", 
					LogMessage.CollectionToString<DBTable>(tables, delegate(DBTable table) { return table.Name; })), d);
			});
		}
		readonly CustomFunctionCollection customFunctionCollection = new CustomFunctionCollection();
		public void RegisterCustomFunctionOperators(ICollection<ICustomFunctionOperator> customFunctions) {
			foreach(ICustomFunctionOperator function in customFunctions) {
				RegisterCustomFunctionOperator(function);
			}
		}
		public void RegisterCustomFunctionOperator(ICustomFunctionOperator customFunction) {
			if(customFunction == null) throw new ArgumentNullException();
			customFunctionCollection.Add(customFunction);
		}
		protected SelectStatementResult GetDataNormal(SelectStatement root) {
			CriteriaOperator condition = root.Condition;
			InMemoryDataElectorContextDescriptor descriptor = new InMemoryDataElectorContextDescriptor(planner, CaseSensitive, customFunctionCollection);
			ExpressionEvaluator[] dataEvaluators = DataSetStoreHelpersFull.PrepareDataEvaluators(root.Operands, descriptor, CaseSensitive, customFunctionCollection);
			SortingComparerFull sortingComparer = DataSetStoreHelpersFull.PrepareSortingComparer(root.SortProperties, descriptor, CaseSensitive, customFunctionCollection);
			IInMemoryDataElector dataElector = planner.GetPlan(root);
			return DataSetStoreHelpersFull.DoGetData(dataElector, descriptor, dataEvaluators, sortingComparer, root.SkipSelectedRecords, root.TopSelectedRecords);
		}
		protected SelectStatementResult GetDataGrouped(SelectStatement root) {
			CriteriaOperator condition = root.Condition;
			InMemoryDataElectorContextDescriptor descriptor = new InMemoryDataElectorContextDescriptor(planner, CaseSensitive, customFunctionCollection);
			ExpressionEvaluator[] groupEvaluators = DataSetStoreHelpersFull.PrepareDataEvaluators(root.GroupProperties, descriptor, CaseSensitive, customFunctionCollection);
			ExpressionEvaluator[] dataEvaluators = DataSetStoreHelpersFull.PrepareDataEvaluators(root.Operands, descriptor, CaseSensitive, customFunctionCollection);
			ExpressionEvaluator havingEvaluator = DataSetStoreHelpersFull.PrepareDataEvaluator(root.GroupCondition, descriptor, CaseSensitive, customFunctionCollection);
			SortingListComparerFull sortingComparer = DataSetStoreHelpersFull.PrepareSortingListComparer(root.SortProperties, descriptor, CaseSensitive, customFunctionCollection);
			IInMemoryDataElector dataElector = planner.GetPlan(root);
			return DataSetStoreHelpersFull.DoGetGroupedData(dataElector, descriptor, groupEvaluators, havingEvaluator, sortingComparer, root.SkipSelectedRecords, root.TopSelectedRecords, dataEvaluators);
		}
		protected override SelectStatementResult ProcessSelectData(SelectStatement selects) {
			return LogManager.Log<SelectStatementResult>(LogManager.LogCategorySQL, () => {
				try {
					if(IsTopLevelAggregateCheckerFull.IsGrouped(selects)) {
						return GetDataGrouped(selects);
					} else {
						return GetDataNormal(selects);
					}
				} catch(InvalidPropertyPathException e) {
					throw new SchemaCorrectionNeededException(e);
				}
			}, (d) => {
				if(selects == null) return null;
				return LogMessage.CreateMessage(this, selects.ToString(), d);
			});
		}
		protected override ModificationResult ProcessModifyData(params ModificationStatement[] dmlStatements) {
			return LogManager.LogMany<ModificationResult>(LogManager.LogCategorySQL, () => {
				BeginTransaction();
				try {
					TaggedParametersHolder identitiesByTag = new TaggedParametersHolder();
					List<ParameterValue> result = new List<ParameterValue>();
					int count = dmlStatements.Length;
					for(int i = 0; i < count; i++) {
						ModificationStatement root = dmlStatements[i];
						int recordsUpdated;
						try {
							InMemoryTable table = DataSetStoreHelpersFull.GetTable(InMemorySet, root.Table.Name);
							if(root is InsertStatement) {
								ParameterValue identityParameter = ((InsertStatement)root).IdentityParameter;
								recordsUpdated = DataSetStoreHelpersFull.DoInsertRecord(table, ((InsertStatement)root).Parameters, root.Operands, identityParameter, identitiesByTag);
								if(!ReferenceEquals(identityParameter, null))
									result.Add(identityParameter);
							} else if(root is UpdateStatement) {
								recordsUpdated = DataSetStoreHelpersFull.DoUpdateRecord(planner, table, ((UpdateStatement)root).Parameters, root.Operands, identitiesByTag, root.Condition, CaseSensitive, customFunctionCollection);
							} else if(root is DeleteStatement) {
								recordsUpdated = DataSetStoreHelpersFull.DoDeleteRecord(planner, table, root.Condition, CaseSensitive, customFunctionCollection);
							} else {
								throw new InvalidOperationException();	
							}
						} catch(InvalidPropertyPathException e) {
							throw new SchemaCorrectionNeededException(e.Message, e);
						} catch(SchemaCorrectionNeededException) {
							throw;
						} catch(InMemoryConstraintException e) {
							throw new ConstraintViolationException(root.ToString(), string.Empty, e);
						} catch(Exception e) {
							throw new SqlExecutionErrorException(root.ToString(), string.Empty, e);
						}
						if(root.RecordsAffected != 0 && root.RecordsAffected != recordsUpdated) {
							throw new LockingException();
						}
					}
					try {
						DoCommit();
					} catch(SchemaCorrectionNeededException) {
						throw;
					} catch(InMemoryConstraintException e) {
						throw new ConstraintViolationException(string.Empty, string.Empty, e);
					}
					return new ModificationResult(result);
				} catch(Exception e) {
					try {
						DoRollback();
					} catch(Exception e2) {
						throw new DevExpress.Xpo.Exceptions.ExceptionBundleException(e, e2);
					}
					throw;
				}
			}, (d) => {
				if(dmlStatements == null && dmlStatements.Length == 0) return null;
				LogMessage[] messages = new LogMessage[dmlStatements.Length];
				for(int i = 0; i < dmlStatements.Length; i++) {
					messages[i] = LogMessage.CreateMessage(this, dmlStatements[i].ToString(), d);
				}
				return messages;
			});
		}
		protected virtual void BeginTransaction() {
			InMemorySet.BeginTransaction();
		}
		protected virtual void DoCommit() {
			InMemorySet.Commit();
		}
		protected virtual void DoRollback() {
			InMemorySet.Rollback();
		}
		static void ClearDataSet(InMemorySet dataSet) {
			dataSet.ClearRelations();
			dataSet.ClearTables();
		}
		protected override void ProcessClearDatabase() {
			ClearDataSet(InMemorySet);
		}
		public DBTable GetTableSchema(string tableName) {
			DBTable result = new DBTable(tableName);
			InMemoryTable table = DataSetStoreHelpersFull.GetTable(InMemorySet, result.Name);
			foreach(InMemoryColumn column in table.Columns) {
				result.AddColumn(new DBColumn(column.Name, table.PrimaryKey != null && table.PrimaryKey.Columns.IndexOf(column) >= 0, null, column.Type == typeof(string) ? column.MaxLength : 0, DBColumn.GetColumnType(column.Type)));
			}
			if(table.PrimaryKey != null && table.PrimaryKey.Columns.Count > 0) {
				StringCollection pkcols = new StringCollection();
				foreach(InMemoryColumn column in table.PrimaryKey.Columns) {
					pkcols.Add(column.Name);
				}
				result.PrimaryKey = new DBPrimaryKey(pkcols);
			}
			foreach(InMemoryIndexWrapper index in table.Indexes) {
				StringCollection cols = new StringCollection();
				foreach(InMemoryColumn column in index.Columns) {
					cols.Add(column.Name);
				}
				result.AddIndex(new DBIndex(index.Name, cols, index.Unique));
			}
			foreach(InMemoryRelation rel in table.FRelations) {
				StringCollection cols = new StringCollection();
				StringCollection relcols = new StringCollection();
				foreach(InMemoryRelationPair pair in rel.Pairs) {
					cols.Add(pair.FKey.Name);
					relcols.Add(pair.PKey.Name);
				}
				result.AddForeignKey(new DBForeignKey(cols, rel.PTable.Name, relcols));
			}
			return result;
		}
		public string[] GetStorageTablesList(bool includeViews) {
			List<string> result = new List<string>();
			foreach(InMemoryTable table in InMemorySet.Tables)
				result.Add(table.Name);
			return result.ToArray();
		}
		public DBTable[] GetStorageTables(params string[] tables) {
			if(tables == null)
				tables = GetStorageTablesList(false);
			int count = tables.Length;
			DBTable[] result = new DBTable[count];
			for(int i = 0; i < count; i++)
				result[i] = GetTableSchema(tables[i]);
			return result;
		}
		public void WriteXml(string fileName) {
			lock(SyncRoot) {
				InMemorySet.WriteXml(fileName);
			}
		}
		public void WriteXml(XmlWriter writer) {
			lock(SyncRoot) {
				InMemorySet.WriteXml(writer);
			}
		}
		public void ReadXml(string fileName) {
			lock(SyncRoot) {
				InMemorySet.ReadXml(fileName);
			}
		}
		public void ReadXml(XmlReader reader) {
			lock(SyncRoot) {
				InMemorySet.ReadXml(reader);
			}
		}
		public void ReadFromInMemoryDataStore(InMemoryDataStore dataStore) {
			lock(SyncRoot) {
				lock(dataStore.SyncRoot) {
					InMemorySet.ReadFromInMemorySet(dataStore.InMemorySet);
				}
			}
		}
		class XmlFileDataStore : DataStoreSerializedBase, IDataStoreForTests, IDataStoreSchemaExplorer, IDataStoreSchemaExplorerSp, IDisposable {
			protected readonly InMemoryDataStore Nested;
			protected readonly FileStream fileStream;
			public XmlFileDataStore(string fileName, AutoCreateOption autoCreateOption, bool readOnly) {
				InMemoryDataStore dataStore = new InMemoryDataStore(autoCreateOption);
				try {
					FileMode mode = (autoCreateOption == AutoCreateOption.DatabaseAndSchema) ? FileMode.OpenOrCreate : FileMode.Open;
					fileStream = new FileStream(fileName, mode, readOnly ? FileAccess.Read : FileAccess.ReadWrite);
					if(fileStream.Length > 0) {
						dataStore.ReadXml(XmlReader.Create(fileStream));
					}
				} catch(Exception e) {
					throw new UnableToOpenDatabaseException(fileName, e);
				}
				Nested = dataStore;
			}
			void Flush() {
				fileStream.Position = 0;
				fileStream.SetLength(0);
				using(XmlWriter writer = XmlWriter.Create(fileStream)) {
					Nested.WriteXml(writer);
				}
				fileStream.Flush();
			}
			public override AutoCreateOption AutoCreateOption {
				get { return Nested.AutoCreateOption; }
			}
			protected override ModificationResult ProcessModifyData(params ModificationStatement[] dmlStatements) {
				if(!fileStream.CanWrite) throw new NotSupportedException(Res.GetString(Res.InMemory_IsReadOnly));
				ModificationResult result = Nested.ModifyData(dmlStatements);
				Flush();
				return result;
			}
			protected override SelectedData ProcessSelectData(params SelectStatement[] selects) {
				return Nested.SelectData(selects);
			}
			protected override UpdateSchemaResult ProcessUpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
				int cnt = Nested.InMemorySet.TablesCount;
				UpdateSchemaResult result = Nested.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
				if(result != UpdateSchemaResult.FirstTableNotExists && Nested.InMemorySet.TablesCount != cnt)
					Flush();
				return result;
			}
			public override object SyncRoot {
				get { return Nested.SyncRoot; }
			}
			public DBTable[] GetStorageTables(params string[] tables) {
				lock(SyncRoot) {
					return Nested.GetStorageTables(tables);
				}
			}
			public string[] GetStorageTablesList(bool includeViews) {
				lock(SyncRoot) {
					return Nested.GetStorageTablesList(includeViews);
				}
			}
			void IDataStoreForTests.ClearDatabase() {
				lock(SyncRoot) {
					((IDataStoreForTests)Nested).ClearDatabase();
					Flush();
				}
			}
			public void Dispose() {
#if !DXRESTRICTED
				fileStream.Close();
#endif
			}
			public DBStoredProcedure[] GetStoredProcedures() {
				lock(SyncRoot) {
					return Nested.GetStoredProcedures();
				}				
			}
		}
#region DataSet redirects
#if !SL && !DXPORTABLE
		[Obsolete("Use DataSetDataStore class instead of InMemoryDataStore", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DataSet Data { get { throw new NotSupportedException(); } }
		[Obsolete("Use DataSetDataStore class or .ctor without DataSet instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public InMemoryDataStore(DataSet data, AutoCreateOption autoCreateOption) : this(data, autoCreateOption, true) { }
		[Obsolete("Use DataSetDataStore class or .ctor without DataSet instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public InMemoryDataStore(DataSet data, AutoCreateOption autoCreateOption, bool caseSensitive)
			: this(autoCreateOption, caseSensitive) {
			if(data.Tables.Count > 0)
				throw new NotSupportedException(Res.GetString(Res.InMemoryFull_UseDataSetDataStoreOrCtor));
		}
#endif
#endregion
		public DBStoredProcedure[] GetStoredProcedures() {
			throw new NotSupportedException();
		}
	}
#if !SL
	public class InMemoryProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			throw new NotSupportedException();
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return InMemoryDataStore.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			if(!parameters.ContainsKey(DatabaseParamID)) { return null; }
			return InMemoryDataStore.GetConnectionString(parameters[DatabaseParamID], parameters.ContainsKey(ReadOnlyParamID) ? parameters[ReadOnlyParamID] != "0" : false);
		}
		public override IDataStore CreateProvider(Dictionary<string, string> parameters, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			string connectionString = GetConnectionString(parameters);
			if(connectionString == null) {
				objectsToDisposeOnDisconnect = new IDisposable[0];
				return null;
			}
			ConnectionStringParser helper = new ConnectionStringParser(connectionString);
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			return CreateProviderFromString(helper.GetConnectionString(), autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override bool HasUserName { get { return false; } }
		public override bool HasPassword { get { return false; } }
		public override bool HasIntegratedSecurity { get { return false; } }
		public override bool HasMultipleDatabases { get { return false; } }
		public override bool IsServerbased { get { return false; } }
		public override bool IsFilebased { get { return true; } }
		public override string ProviderKey { get { return "InMemorySetFull"; } }
		public override string[] GetDatabases(string server, string userid, string password) {
			return new string[1] { server };
		}
		public override string FileFilter { get { return "Xml files|*.xml"; } }
		public override bool MeanSchemaGeneration { get { return true; } }
	}
#endif
}
