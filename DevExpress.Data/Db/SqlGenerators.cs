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
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Xml.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB.Helpers;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.Xpo.DB.Helpers {
	public interface ISqlGeneratorFormatter {
		string FormatTable(string schema, string tableName);
		string FormatTable(string schema, string tableName, string tableAlias);
		string FormatColumn(string columnName);
		string FormatColumn(string columnName, string tableAlias);
		string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords);
		string FormatInsertDefaultValues(string tableName);
		string FormatInsert(string tableName, string fields, string values);
		string FormatUpdate(string tableName, string sets, string whereClause);
		string FormatDelete(string tableName, string whereClause);
		string FormatUnary(UnaryOperatorType operatorType, string operand);
		string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand);
		string FormatFunction(FunctionOperatorType operatorType, params string[] operands);
		string FormatOrder(string sortProperty, SortingDirection direction);
		string GetParameterName(OperandValue parameter, int index, ref bool createParameter);
		string ComposeSafeTableName(string tableName);
		string ComposeSafeSchemaName(string tableName);
		string ComposeSafeColumnName(string columnName);
		bool BraceJoin { get; }
		bool SupportNamedParameters { get; }
	}
	public delegate string ProcessParameter(object parameter);
	public interface ISqlGeneratorFormatterEx : ISqlGeneratorFormatter {
		string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands);
	}
	public interface ISqlGeneratorFormatterSupportSkipTake : ISqlGeneratorFormatter {
		bool NativeSkipTakeSupported { get;}
		string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords);
	}
	public interface ISqlGeneratorFormatterSupportOuterApply : ISqlGeneratorFormatter {
		bool NativeOuterApplySupported { get; }
		string FormatOuterApply(string sql, string alias);
	}
	public abstract class BaseSqlGenerator : IQueryCriteriaVisitor<string> {
		BaseStatement root;
		bool hasSubQuery = false;
		bool inTopLevelAggregate;
		int outerApplyAliasCounter = 0;
		int outerApplyResultCounter = 0;
		Dictionary<OuterApplyCacheItem, OuterApplyInfo> outerApplyCache;
		protected int OuterApplyAliasCounter { get { return outerApplyAliasCounter; } }
		protected int OuterApplyResultCounter { get { return outerApplyResultCounter; } }
		Dictionary<OuterApplyCacheItem, OuterApplyInfo> OuterApplyCache {
			get {
				if(outerApplyCache == null) {
					outerApplyCache = new Dictionary<OuterApplyCacheItem, OuterApplyInfo>();
				}
				return outerApplyCache;
			}
		}
		protected virtual bool TryUseOuterApply { get { return false; } }
		protected virtual bool IsSubQuery { get { return false; } }
		protected virtual bool ForceOuterApply { get { return inTopLevelAggregate; } }
		protected BaseStatement Root { get { return root; } }
		protected void SetUpRootQueryStatement(BaseStatement root) { this.root = root; }
		protected readonly ISqlGeneratorFormatter formatter;
		protected readonly ISqlGeneratorFormatterEx formatterEx;
		public bool HasSubQuery { get { return hasSubQuery; } }
		public abstract string GetNextParameterName(OperandValue parameter);
		protected string Process(CriteriaOperator operand, bool nullOnNull) {
			if(ReferenceEquals(operand, null)) {
				if(nullOnNull)
					return null;
				else
					throw new InvalidOperationException("empty subcriteria");
			}
			return operand.Accept(this);
		}
		string Process(object operand) {
			return Process((CriteriaOperator)operand);
		}
		protected string Process(CriteriaOperator operand) {
			return Process(operand, false);
		}
		string GetNextOuterApplyAlias() {
			return string.Concat("OA", (outerApplyAliasCounter++).ToString());
		}
		protected List<string> GetOuterApplyAliasList() {
			if(TryUseOuterApply) {
				ISqlGeneratorFormatterSupportOuterApply formatterOuterApply = GetOuterApplyImpl(formatter);
				if(formatterOuterApply != null && OuterApplyCache.Count > 0) {
					List<string> result = new List<string>();
					foreach(OuterApplyInfo outerApply in OuterApplyCache.Values) {
						result.Add(outerApply.Alias);
					}
					return result;
				}
			}
			return null;
		}
		void AppendJoinNode(JoinNode node, StringBuilder joins) {
			if(formatter.BraceJoin)
				joins.Insert(0, "(");
			joins.Append("\n ");
			joins.Append(node.Type == JoinType.Inner ? "inner" : "left");
			joins.Append(" join ");
			DBProjection projection = node.Table as DBProjection;
			if(projection != null) {
				var gen = new SelectSqlGenerator(formatter, this, projection.Columns.Select(c => c == null ? string.Empty : c.Name).ToList());				
				var projectionQuery = gen.GenerateSql(projection.Projection);
				joins.Append(FormatSubQuery(projectionQuery.Sql, node.Alias));
			} else {
				joins.Append(formatter.FormatTable(formatter.ComposeSafeSchemaName(node.Table.Name), formatter.ComposeSafeTableName(node.Table.Name), node.Alias));
			}
			joins.Append(" on ");
			joins.Append(Process(node.Condition));
			if(formatter.BraceJoin)
				joins.Append(')');
			foreach(JoinNode subNode in node.SubNodes)
				AppendJoinNode(subNode, joins);
		}
		protected StringBuilder BuildJoins() {
			StringBuilder joins = new StringBuilder();
			DBProjection projection = Root.Table as DBProjection;
			if(projection != null) {
				var gen = new SelectSqlGenerator(formatter, this, projection.Columns.Select(c => c == null ? string.Empty : c.Name).ToList());
				var projectionQuery = gen.GenerateSql(projection.Projection);
				joins.Append(FormatSubQuery(projectionQuery.Sql, Root.Alias));
			} else {
				joins.Append(formatter.FormatTable(formatter.ComposeSafeSchemaName(Root.Table.Name), formatter.ComposeSafeTableName(Root.Table.Name), Root.Alias));
			}
			foreach(JoinNode subNode in Root.SubNodes)
				AppendJoinNode(subNode, joins);
			return joins;
		}
		public static ISqlGeneratorFormatterSupportOuterApply GetOuterApplyImpl(ISqlGeneratorFormatter formatter) {
			ISqlGeneratorFormatterSupportOuterApply result = formatter as ISqlGeneratorFormatterSupportOuterApply;
			if(result != null && result.NativeOuterApplySupported)
				return result;
			else
				return null;
		}
		protected string BuildOuterApply() {
			if(TryUseOuterApply) {
				ISqlGeneratorFormatterSupportOuterApply formatterOuterApply = GetOuterApplyImpl(formatter);
				if(formatterOuterApply != null && OuterApplyCache.Count > 0) {
					StringBuilder result = new StringBuilder();
					foreach(OuterApplyInfo outerApply in OuterApplyCache.Values) {
						result.AppendFormat(CultureInfo.InvariantCulture, "\n{0} ", formatterOuterApply.FormatOuterApply(outerApply.SubSql, outerApply.Alias));
					}
					result.Append("\n");
					return result.ToString();
				}
			}
			return string.Empty;
		}
		protected string BuildCriteria() {
			return Process(Root.Condition, true);
		}
		protected BaseSqlGenerator(ISqlGeneratorFormatter formatter) {
			this.formatter = formatter;
			this.formatterEx = formatter as ISqlGeneratorFormatterEx;
		}
		static BaseSqlGenerator() {
			groupOps = new string[2];
			groupOps[(int)GroupOperatorType.And] = "and";
			groupOps[(int)GroupOperatorType.Or] = "or";
		}
		static string[] groupOps;
		static string GetGroupOpName(GroupOperatorType type) {
			return groupOps[(int)type];
		}
		string ICriteriaVisitor<string>.Visit(OperandValue theOperand) {
			return GetNextParameterName(theOperand);
		}
		string IQueryCriteriaVisitor<string>.Visit(QueryOperand operand) {
			return operand.NodeAlias == null ?
				formatter.FormatColumn(formatter.ComposeSafeColumnName(operand.ColumnName)) :
				formatter.FormatColumn(formatter.ComposeSafeColumnName(operand.ColumnName), operand.NodeAlias);
		}
		string ICriteriaVisitor<string>.Visit(BetweenOperator theOperator) {
			return Process(GroupOperator.And(
				new BinaryOperator(theOperator.BeginExpression, theOperator.TestExpression, BinaryOperatorType.LessOrEqual),
				new BinaryOperator(theOperator.TestExpression, theOperator.EndExpression, BinaryOperatorType.LessOrEqual))
				);
		}
		string ICriteriaVisitor<string>.Visit(BinaryOperator theOperator) {
#pragma warning disable 618
			if(theOperator.OperatorType == BinaryOperatorType.Like)
				throw new InvalidOperationException("Custom function 'Like' expected instead of BinaryOperatorType.Like");
#pragma warning restore 618
			return formatter.FormatBinary(theOperator.OperatorType, Process(theOperator.LeftOperand), Process(theOperator.RightOperand));
		}
		string ICriteriaVisitor<string>.Visit(InOperator theOperator) {
			string left = Process(theOperator.LeftOperand);
			StringBuilder inString = new StringBuilder();
			foreach(CriteriaOperator value in theOperator.Operands) {
				if(inString.Length > 0)
					inString.Append(',');
				inString.Append(Process(value));
			}
			return String.Format(CultureInfo.InvariantCulture, "{0} in ({1})",
				left, inString.ToString());
		}
		string ICriteriaVisitor<string>.Visit(GroupOperator theOperator) {
			StringCollection list = new StringCollection();
			foreach(CriteriaOperator cr in theOperator.Operands) {
				string crs = Process(cr);
				if(crs != null)
					list.Add(crs);
			}
			return "(" + StringListHelper.DelimitedText(list, " " + GetGroupOpName(theOperator.OperatorType) + " ") + ")";
		}
		string ICriteriaVisitor<string>.Visit(UnaryOperator theOperator) {
			return formatter.FormatUnary(theOperator.OperatorType, Process(theOperator.Operand));
		}
		string ICriteriaVisitor<string>.Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.LocalDateTimeThisYear:
				case FunctionOperatorType.LocalDateTimeThisMonth:
				case FunctionOperatorType.LocalDateTimeLastWeek:
				case FunctionOperatorType.LocalDateTimeThisWeek:
				case FunctionOperatorType.LocalDateTimeYesterday:
				case FunctionOperatorType.LocalDateTimeToday:
				case FunctionOperatorType.LocalDateTimeNow:
				case FunctionOperatorType.LocalDateTimeTomorrow:
				case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
				case FunctionOperatorType.LocalDateTimeNextWeek:
				case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
				case FunctionOperatorType.LocalDateTimeNextMonth:
				case FunctionOperatorType.LocalDateTimeNextYear:
					if(theOperator.Operands.Count != 0)
						throw new ArgumentException("theOperator.Operands.Count != 0");
					return Process(new ConstantValue(DevExpress.Data.Filtering.Helpers.EvalHelpers.EvaluateLocalDateTime(theOperator.OperatorType)));
				case FunctionOperatorType.IsThisMonth:
				case FunctionOperatorType.IsThisWeek:
				case FunctionOperatorType.IsThisYear:
					return Process(DevExpress.Data.Filtering.Helpers.EvalHelpers.ExpandIsOutlookInterval(theOperator));
			}
			if(formatterEx != null) {
				object[] operands = new object[theOperator.Operands.Count];
				Array.Copy(theOperator.Operands.ToArray(), operands, operands.Length);
				if(theOperator.OperatorType == FunctionOperatorType.Custom || theOperator.OperatorType == FunctionOperatorType.CustomNonDeterministic) {
					if(operands.Length < 1 || !(theOperator.Operands[0] is OperandValue) || !(((OperandValue)theOperator.Operands[0]).Value is String))
						throw new Exception(); 
					operands[0] = ((OperandValue)theOperator.Operands[0]).Value;
				}
				return formatterEx.FormatFunction(Process, theOperator.OperatorType, operands);
			} else {
				string[] operands = new string[theOperator.Operands.Count];
				int i;
				if(theOperator.OperatorType == FunctionOperatorType.Custom || theOperator.OperatorType == FunctionOperatorType.CustomNonDeterministic) {
					i = 1;
					if(operands.Length < 1 || !(theOperator.Operands[0] is OperandValue) || !(((OperandValue)theOperator.Operands[0]).Value is String))
						throw new Exception(); 
					operands[0] = (string)((OperandValue)theOperator.Operands[0]).Value;
				} else
					i = 0;
				for(; i < theOperator.Operands.Count; ++i) {
					operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
				}
				return formatter.FormatFunction(theOperator.OperatorType, operands);
			}
		}
		string IQueryCriteriaVisitor<string>.Visit(QuerySubQueryContainer container) {
			if(container.Node == null) {
				inTopLevelAggregate = true;
				int memOuterApplyResultCounter = outerApplyResultCounter;
				try {
					return SubSelectSqlGenerator.GetSelectValue(container.AggregateProperty, container.AggregateType, this);
				} finally {
					outerApplyResultCounter = memOuterApplyResultCounter;
					inTopLevelAggregate = false;
				}
			}
			bool isExists = container.AggregateType == Aggregate.Exists;
			hasSubQuery = !isExists;
			CriteriaOperator prop = (container.Node.Operands.Count > 0) ? container.Node.Operands[0] : null;
			if(TryUseOuterApply) {
				ISqlGeneratorFormatterSupportOuterApply formatterOuterApply = GetOuterApplyImpl(formatter);
				if(formatterOuterApply != null) {
					OuterApplyCacheItem cacheItem = new OuterApplyCacheItem(container.Node);
					OuterApplyInfo info;
					if(!OuterApplyCache.TryGetValue(cacheItem, out info)) {
						SubSelectSqlGenerator outerGena = new SubSelectSqlGenerator(this, formatter, prop, container.AggregateType, !isExists);
						string subQuery = outerGena.GenerateSelect(container.Node, false);
						if(!outerGena.HasSubQuery && !(IsSubQuery && !isExists) && !ForceOuterApply) {
							return FormatSubQuery(container.AggregateType, subQuery);
						}
						hasSubQuery = true;
						if(isExists) {
							subQuery = new SubSelectSqlGenerator(this, formatter, prop, Aggregate.Count).GenerateSelect(container.Node, false);
						}
						info = new OuterApplyInfo(GetNextOuterApplyAlias(), subQuery);
						OuterApplyCache.Add(cacheItem, info);
					}
					outerApplyResultCounter++;
					if(isExists) {
						return string.Concat("(", info.Alias, ".Res > 0)");
					}
					return string.Concat(info.Alias, ".Res");
				}
			}
			SubSelectSqlGenerator gena = new SubSelectSqlGenerator(this, formatter, prop, container.AggregateType, !isExists);
			return FormatSubQuery(container.AggregateType, gena.GenerateSelect(container.Node, true));
		}
		static string FormatSubQuery(Aggregate aggregateType, string subQuery) {
			return string.Format(CultureInfo.InvariantCulture, "{1}({0})", subQuery,
				(aggregateType == Aggregate.Exists) ? "exists" : string.Empty);
		}
		static string FormatSubQuery(string subQuery, string alias) {
			return string.Concat("(", subQuery, ") ", alias);
		}
		struct OuterApplyInfo {
			public readonly string Alias;
			public readonly string SubSql;
			public OuterApplyInfo(string alias, string subSql) {
				Alias = alias;
				SubSql = subSql;
			}
		}
		struct OuterApplyCacheItem {
			public readonly JoinNode Node;
			int hash;
			public OuterApplyCacheItem(JoinNode node) {
				Node = node;
				hash = OuterApplyCacheComparer.GetHash(node);
			}
			public override int GetHashCode() {
				return hash;
			}
			public override bool Equals(object obj) {
				if(obj == null || !(obj is OuterApplyCacheItem)) return false;
				return OuterApplyCacheComparer.AreEquals(new OuterApplyCompareCache(), this.Node, ((OuterApplyCacheItem)obj).Node);
			}
		}
		class OuterApplyCompareCache {
			int nodeAliasCounter = 0;
			Dictionary<string, string> nodeAliasCache = new Dictionary<string, string>();
			public string GetCacheAliasName(string alias) {
				string result;
				if(!nodeAliasCache.TryGetValue(alias, out result)) {
					return alias;
				}
				return result;
			}
			public void AddNode(string nodeAlias, string nodeCacheAlias) {
				nodeAliasCache[nodeAlias] = nodeCacheAlias;
			}
			public string GetNextCacheAlias() {
				return string.Format("NCA{0}", nodeAliasCounter++);
			}
		}
		class OuterApplyCacheComparer {
			public static bool AreEquals(OuterApplyCompareCache compareCache, JoinNode x, JoinNode y) {
				if(x == y) return true;
				if(x == null || y == null) return false;
				BaseStatement baseX = x as BaseStatement;
				BaseStatement baseY = y as BaseStatement;
				if((baseX == null && baseY != null) || (baseX != null && baseY == null)) return false;
				if(Equals(x.Type, y.Type) && Equals(x.Table, y.Table)) {
					string nextCacheNodeAlias = compareCache.GetNextCacheAlias();
					compareCache.AddNode(x.Alias, nextCacheNodeAlias);
					compareCache.AddNode(y.Alias, nextCacheNodeAlias);
					JoinNodeCollection xConditionSubNodes;
					JoinNodeCollection yConditionSubNodes;
					if(!OuterApplyCacheComparer.AreEquals(compareCache, x.SubNodes, y.SubNodes)) return false;
					xConditionSubNodes = OuterApplyCacheCriteriaPreprocessor.CollectNodes(compareCache, x.Condition);
					yConditionSubNodes = OuterApplyCacheCriteriaPreprocessor.CollectNodes(compareCache, y.Condition);
					if(!OuterApplyCacheComparer.AreEquals(compareCache, xConditionSubNodes, yConditionSubNodes) ||
							!Equals(OuterApplyCacheCriteriaPreprocessor.Preprocess(compareCache, x.Condition),
										OuterApplyCacheCriteriaPreprocessor.Preprocess(compareCache, y.Condition))) return false;
					if(baseX != null && baseY != null) {
						if(baseX.Operands.Count != baseY.Operands.Count) return false;
						int operandsCount = baseX.Operands.Count;
						for(int i = 0; i < operandsCount; i++) {
							JoinNodeCollection xOperandsSupNodes = OuterApplyCacheCriteriaPreprocessor.CollectNodes(compareCache, baseX.Operands[i]);
							JoinNodeCollection yOperandsSupNodes = OuterApplyCacheCriteriaPreprocessor.CollectNodes(compareCache, baseY.Operands[i]);
							if(!OuterApplyCacheComparer.AreEquals(compareCache, xOperandsSupNodes, yOperandsSupNodes)
								|| !Equals(OuterApplyCacheCriteriaPreprocessor.Preprocess(compareCache, baseX.Operands[i]), OuterApplyCacheCriteriaPreprocessor.Preprocess(compareCache, baseY.Operands[i]))) 
										return false;
						}
					}
					return true;
				}
				return false;
			}
			static protected int GetHashCode(object obj) {
				return obj == null ? 0 : obj.GetHashCode();
			}
			public static int GetHash(JoinNode obj) {
				if(obj == null) return 0x64323421;				
				int hash = ((int)obj.Type).GetHashCode() ^ GetHashCode(obj.Table) 
					^ OuterApplyCacheComparer.GetHash(obj.SubNodes)
					^ OuterApplyCacheComparer.GetHash(OuterApplyCacheCriteriaPreprocessor.CollectNodes(null, obj.Condition))
					^ GetHashCode(OuterApplyCacheCriteriaPreprocessor.Preprocess(null, obj.Condition));
				BaseStatement baseS = obj as BaseStatement;
				if(baseS == null) return hash;
				for(int i = 0; i < baseS.Operands.Count; i++) {					
					hash ^= OuterApplyCacheComparer.GetHash(OuterApplyCacheCriteriaPreprocessor.CollectNodes(null, baseS.Operands[i]));
					hash ^= GetHashCode(OuterApplyCacheCriteriaPreprocessor.Preprocess(null, baseS.Operands[i]));					
				}
				return hash;
			}
			public static bool AreEquals(OuterApplyCompareCache compareCache, JoinNodeCollection x, JoinNodeCollection y) {
				if(x == y)
					return true;
				if(x == null || y == null) return false;
				if(x.Count != y.Count)
					return false;
				for(int i = 0; i < x.Count; ++i) {
					if(!OuterApplyCacheComparer.AreEquals(compareCache, x[i], y[i]))
						return false;
				}
				return true;
			}
			public static int GetHash(JoinNodeCollection obj) {
				int result = 0x1259321;
				if(obj != null) {
					foreach(JoinNode o in obj) {
						result ^= OuterApplyCacheComparer.GetHash(o);
					}
				}
				return result;
			}
		}
		class OuterApplyCacheCriteriaPreprocessor : DevExpress.Data.Filtering.Helpers.ClientCriteriaVisitorBase, IQueryCriteriaVisitor<CriteriaOperator> {
			bool collectNodes;
			OuterApplyCompareCache compareCache;
			JoinNodeCollection subNodes = null;
			public JoinNodeCollection SubNodes { get { return subNodes; } }
			OuterApplyCacheCriteriaPreprocessor(OuterApplyCompareCache compareCache)
				: this(compareCache, false) {
			}
			OuterApplyCacheCriteriaPreprocessor(OuterApplyCompareCache compareCache, bool collectNodes) {
				this.compareCache = compareCache;
				this.collectNodes = collectNodes;
			}
			public CriteriaOperator Visit(QueryOperand theOperand) {
				if(collectNodes) return null;
				if(compareCache == null) return new QueryOperand(theOperand.ColumnName, string.Empty, theOperand.ColumnType);
				string cacheAliasName = compareCache.GetCacheAliasName(theOperand.NodeAlias);
				if(cacheAliasName == theOperand.NodeAlias) return theOperand;
				return new QueryOperand(theOperand.ColumnName, cacheAliasName, theOperand.ColumnType);
			}
			public CriteriaOperator Visit(QuerySubQueryContainer theOperand) {
				if(collectNodes) {
					if(subNodes == null) subNodes = new JoinNodeCollection();
					subNodes.Add(theOperand.Node);
				}
				CriteriaOperator aggregateProperty = Process(theOperand.AggregateProperty);
				if(collectNodes) return null;
				return new QuerySubQueryContainer(null, aggregateProperty, theOperand.AggregateType);
			}
			public static CriteriaOperator Preprocess(OuterApplyCompareCache compareCache, CriteriaOperator criteria) {
				OuterApplyCacheCriteriaPreprocessor preprocessor = new OuterApplyCacheCriteriaPreprocessor(compareCache);
				return preprocessor.Process(criteria);
			}
			public static JoinNodeCollection CollectNodes(OuterApplyCompareCache compareCache, CriteriaOperator criteria) {
				OuterApplyCacheCriteriaPreprocessor preprocessor = new OuterApplyCacheCriteriaPreprocessor(compareCache, true);
				preprocessor.Process(criteria);				
				return preprocessor.SubNodes;
			}
		}
	}
	public abstract class BaseSqlGeneratorWithParameters : BaseSqlGenerator {
		QueryParameterCollection queryParams;
		List<string> queryParamsNames;
		TaggedParametersHolder identitiesByTag;
		Dictionary<OperandValue, string> parameters;
		protected BaseSqlGeneratorWithParameters(ISqlGeneratorFormatter formatter, TaggedParametersHolder identitiesByTag, Dictionary<OperandValue, string> parameters)
			: base(formatter) {
			this.identitiesByTag = identitiesByTag;
			this.parameters = parameters;
		}
		public override string GetNextParameterName(OperandValue parameter) {
			parameter = identitiesByTag.ConsolidateParameter(parameter);
			if(parameter.Value != null) {
				if(formatter.SupportNamedParameters) {
					string name;
					if(!parameters.TryGetValue(parameter, out name)) {
						bool createParameter = true;
						name = formatter.GetParameterName(parameter, parameters.Count, ref createParameter);
						if(createParameter) {
							parameters.Add(parameter, name);
							queryParams.Add(parameter);
							queryParamsNames.Add(name);
						}
					}
					return name;
				} else {
					bool createParameter = true;
					string name = formatter.GetParameterName(parameter, queryParams.Count, ref createParameter);
					if(createParameter) {
						queryParams.Add(parameter);
						queryParamsNames.Add(name);
					}
					return name;
				}
			} else
				return "null";
		}
		void SetUpParameters() {
			queryParams = new QueryParameterCollection();
			queryParamsNames = new List<string>(0);
		}
		protected virtual Query CreateQuery(string sql, QueryParameterCollection parameters, IList parametersNames) {
			return new Query(sql, parameters, parametersNames);
		}
		public Query GenerateSql(BaseStatement node) {
			SetUpRootQueryStatement(node);
			SetUpParameters();
			return CreateQuery(InternalGenerateSql(), queryParams, queryParamsNames);
		}
		protected abstract string InternalGenerateSql();
	}
	public class SubSelectSqlGenerator : BaseSqlGenerator {
		readonly BaseSqlGenerator parentGenerator;
		readonly Aggregate aggregate;
		readonly CriteriaOperator aggregateProperty;
		readonly bool forceOuterApply;
		protected override bool TryUseOuterApply { get { return true; } }
		protected override bool IsSubQuery { get { return true; } }
		protected override bool ForceOuterApply { 
			get { 
				return forceOuterApply || base.ForceOuterApply; 
			} 
		}
		public string GenerateSelect(BaseStatement node, bool subSelectUseOnly) {
			SetUpRootQueryStatement(node);
			StringBuilder joins = BuildJoins();
			string whereSql = BuildCriteria();
			string selectValue = GetSelectValue();
			string outerApply = BuildOuterApply();
			if(!string.IsNullOrEmpty(outerApply))
				joins.Append(outerApply);
			if(whereSql != null)
				joins.AppendFormat(CultureInfo.InvariantCulture, " where {0}", whereSql);
			return String.Format(CultureInfo.InvariantCulture, "select {0}{2} from {1}", selectValue, joins.ToString(), aggregate == Aggregate.Exists || subSelectUseOnly ? string.Empty : " as Res");
		}
		static SubSelectSqlGenerator() {
			agg = new string[6];
			agg[(int)Aggregate.Max] = "max({0})";
			agg[(int)Aggregate.Min] = "min({0})";
			agg[(int)Aggregate.Avg] = "avg({0})";
			agg[(int)Aggregate.Count] = "count({0})";
			agg[(int)Aggregate.Sum] = "sum({0})";
			agg[(int)Aggregate.Exists] = "{0}";
		}
		static string[] agg;
		static public string GetSelectValue(CriteriaOperator aggregateProperty, Aggregate aggregate, BaseSqlGenerator generator) {
			string property = ReferenceEquals(aggregateProperty, null) ? "*" : (string)aggregateProperty.Accept(generator);
			return String.Format(agg[(int)aggregate], property);
		}
		string GetSelectValue() {
			return GetSelectValue(aggregateProperty, aggregate, this);
		}
		public SubSelectSqlGenerator(BaseSqlGenerator parentGenerator, ISqlGeneratorFormatter formatter, CriteriaOperator aggregateProperty, Aggregate aggregate)
			: this(parentGenerator, formatter, aggregateProperty, aggregate, false) {
		}
		public SubSelectSqlGenerator(BaseSqlGenerator parentGenerator, ISqlGeneratorFormatter formatter, CriteriaOperator aggregateProperty, Aggregate aggregate, bool forceOuterApply)
			: base(formatter) {
			this.parentGenerator = parentGenerator;
			this.aggregate = aggregate;
			this.aggregateProperty = aggregateProperty;
			this.forceOuterApply = forceOuterApply;
		}
		public override string GetNextParameterName(OperandValue parameter) {
			return parentGenerator.GetNextParameterName(parameter);
		}
	}
	public class SelectSqlGenerator : BaseSqlGeneratorWithParameters {
		readonly BaseSqlGenerator parentGenerator;
		readonly IList<string> propertyAliases;
		new protected SelectStatement Root { get { return (SelectStatement)base.Root; } }
		protected override bool TryUseOuterApply { get { return true; } }
		readonly Dictionary<int, OperandValue> constantValues = new Dictionary<int, OperandValue>();
		readonly Dictionary<int, int> operandIndexes = new Dictionary<int, int>();
		HashSet<string> oaProperties;
		HashSet<string> groupProperties;
		string BuildSorting() {
			if(Root.SortProperties.Count == 0)
				return null;
			StringBuilder list = new StringBuilder();
			for(int i = 0; i < Root.SortProperties.Count; i++) {
				SortingColumn sp = Root.SortProperties[i];
				int j;
				for(j = 0; j < i; j++)
					if(Root.SortProperties[j].Property.Equals(sp.Property))
						break;
				if(j < i)
					continue;
				list.Append(formatter.FormatOrder(Process(sp.Property), sp.Direction));
				list.Append(',');
			}
			return list.ToString(0, list.Length - 1);
		}
		bool isBuildGrouping = false;
		protected override bool ForceOuterApply {
			get {
				return isBuildGrouping || base.ForceOuterApply;
			}
		}
		string BuildGrouping() {
			isBuildGrouping = true;
			try {
				if(Root.GroupProperties.Count == 0)
					return null;
				groupProperties = new HashSet<string>();
				StringBuilder list = new StringBuilder();
				foreach(CriteriaOperator sp in Root.GroupProperties) {
					string groupProperty = Process(sp);
					groupProperties.Add(groupProperty);
					list.AppendFormat(CultureInfo.InvariantCulture, "{0},", groupProperty);
				}
				return list.ToString(0, list.Length - 1);
			} finally {
				isBuildGrouping = false;
			}
		}
		string BuildAdditionalGroupingOuterApply() {
			if(oaProperties != null && groupProperties != null && TryUseOuterApply) {
				StringBuilder result = new StringBuilder();
				foreach(string outerApplyProperty in oaProperties) {
					if(!groupProperties.Contains(outerApplyProperty)) {
						result.AppendFormat(CultureInfo.InvariantCulture, ",{0}", outerApplyProperty);
					}
				}
				return result.ToString();
			}
			return null;
		}
		protected virtual string PatchProperty(CriteriaOperator propertyOperator, string propertyString) {
			return propertyString;
		}
		string BuildProperties() {
			int operandIndex = 0;
			StringBuilder list = new StringBuilder();
			for(int i = 0; i < Root.Operands.Count; i++) {
				CriteriaOperator mic = Root.Operands[i];
				if(mic is OperandValue && parentGenerator == null) {
					constantValues.Add(i, (OperandValue)mic);
				} else {
					if(list.Length > 0)
						list.Append(',');
					int oaResultCount = OuterApplyResultCounter;
					string property = Process(mic);
					if(OuterApplyResultCounter > oaResultCount) {
						if(oaProperties == null) oaProperties = new HashSet<string>();
						oaProperties.Add(property);
					}
					list.Append(PatchProperty(mic, property));
					if(propertyAliases != null) {
						string propertyAlias = operandIndex < propertyAliases.Count ? propertyAliases[operandIndex] : null;
						QueryOperand queryOperand = mic as QueryOperand;
						list.Append(" as ");
						string alias = "";
						if(ReferenceEquals(queryOperand, null)) {
							alias = propertyAlias ?? string.Concat("PrP", operandIndex.ToString(CultureInfo.InvariantCulture));
						} else {
#if DXPORTABLE
							alias = string.IsNullOrEmpty(propertyAlias) && !string.Equals(propertyAlias, queryOperand.ColumnName, StringComparison.OrdinalIgnoreCase) ? queryOperand.ColumnName : propertyAlias;
#else
							alias = string.IsNullOrEmpty(propertyAlias) && !string.Equals(propertyAlias, queryOperand.ColumnName, StringComparison.InvariantCultureIgnoreCase) ? queryOperand.ColumnName : propertyAlias;
#endif
						}
						list.Append(formatter.FormatColumn(formatter.ComposeSafeColumnName(alias)));
					}
					operandIndexes.Add(i, operandIndex++);
				}
			}
			if(operandIndex == 0) return "1";
			return list.ToString();
		}
		string BuildGroupCriteria() {
			return Process(Root.GroupCondition, true);
		}
		public static ISqlGeneratorFormatterSupportSkipTake GetSkipTakeImpl(ISqlGeneratorFormatter formatter) {
			ISqlGeneratorFormatterSupportSkipTake result = formatter as ISqlGeneratorFormatterSupportSkipTake;
			if(result != null && result.NativeSkipTakeSupported)
				return result;
			else
				return null;
		}
		protected override string InternalGenerateSql() {
			string groupBySql = BuildGrouping();
			string propertiesSql = BuildProperties();
			string fromSql = BuildJoins().ToString();
			string whereSql = BuildCriteria();
			string havingSql = BuildGroupCriteria();
			string sortingSql = BuildSorting();
			int skipSelectedRecords = ((SelectStatement)Root).SkipSelectedRecords;
			int topSelectedRecords = ((SelectStatement)Root).TopSelectedRecords;
			fromSql = string.Concat(fromSql, BuildOuterApply());
			if(groupBySql != null) {
				groupBySql = string.Concat(groupBySql, BuildAdditionalGroupingOuterApply());
			}
			if(skipSelectedRecords != 0) {
				ISqlGeneratorFormatterSupportSkipTake skipFormatter = GetSkipTakeImpl(formatter);
				if(skipFormatter != null) {
					return skipFormatter.FormatSelect(propertiesSql, fromSql, whereSql, sortingSql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
				} else if(topSelectedRecords != 0) {
					return formatter.FormatSelect(propertiesSql, fromSql, whereSql, sortingSql, groupBySql, havingSql, skipSelectedRecords + topSelectedRecords);
				}
			}
			return formatter.FormatSelect(propertiesSql, fromSql, whereSql, sortingSql, groupBySql, havingSql, topSelectedRecords);
		}
		public SelectSqlGenerator(ISqlGeneratorFormatter formatter)
			: this(formatter, null, null) {
		}
		public SelectSqlGenerator(ISqlGeneratorFormatter formatter, BaseSqlGenerator parentGenerator, IList<string> propertyAliases)
			: base(formatter, new TaggedParametersHolder(), new Dictionary<OperandValue, string>()) {
			this.parentGenerator = parentGenerator;
			this.propertyAliases = propertyAliases;
		}
		protected override Query CreateQuery(string sql, QueryParameterCollection parameters, IList parametersNames) {
			return new Query(sql, parameters, parametersNames, Root.SkipSelectedRecords, Root.TopSelectedRecords, constantValues, operandIndexes);
		}
		public override string GetNextParameterName(OperandValue parameter) {
			return parentGenerator == null ? base.GetNextParameterName(parameter) : parentGenerator.GetNextParameterName(parameter);
		}
	}
	public abstract class BaseObjectSqlGenerator : BaseSqlGeneratorWithParameters {
		public QueryCollection GenerateSql(ModificationStatement[] dmlStatements) {
			QueryCollection list = new QueryCollection();
			foreach(ModificationStatement node in dmlStatements) {
				Query q = base.GenerateSql(node);
				if(q.Sql != null)
					list.Add(q);
			}
			return list;
		}
		protected BaseObjectSqlGenerator(ISqlGeneratorFormatter formatter, TaggedParametersHolder identities, Dictionary<OperandValue, string> parameters) : base(formatter, identities, parameters) { }
	}
	public class InsertSqlGenerator : BaseObjectSqlGenerator {
		protected override string InternalGenerateSql() {
			if(Root.Table is DBProjection)
				throw new InvalidOperationException(); 
			if(Root.Operands.Count == 0)
				return formatter.FormatInsertDefaultValues(formatter.FormatTable(formatter.ComposeSafeSchemaName(Root.Table.Name), formatter.ComposeSafeTableName(Root.Table.Name)));
			StringBuilder names = new StringBuilder();
			StringBuilder values = new StringBuilder();
			for(int i = 0; i < Root.Operands.Count; i++) {
				names.Append(Process(Root.Operands[i]));
				names.Append(",");
				values.Append(GetNextParameterName(((InsertStatement)Root).Parameters[i]));
				values.Append(",");
			}
			return formatter.FormatInsert(formatter.FormatTable(formatter.ComposeSafeSchemaName(Root.Table.Name), formatter.ComposeSafeTableName(Root.Table.Name)),
				names.ToString(0, names.Length - 1),
				values.ToString(0, values.Length - 1));
		}
		public InsertSqlGenerator(ISqlGeneratorFormatter formatter, TaggedParametersHolder identities, Dictionary<OperandValue, string> parameters) : base(formatter, identities, parameters) { }
	}
	public sealed class UpdateSqlGenerator : BaseObjectSqlGenerator {
		protected override string InternalGenerateSql() {
			if(Root.Table is DBProjection)
				throw new InvalidOperationException(); 
			if(Root.Operands.Count == 0)
				return null;
			StringBuilder values = new StringBuilder();
			for(int i = 0; i < Root.Operands.Count; i++)
				values.AppendFormat(CultureInfo.InvariantCulture, "{0}={1},", Process(Root.Operands[i]), GetNextParameterName(((UpdateStatement)Root).Parameters[i]));
			values.Remove(values.Length - 1, 1);
			return formatter.FormatUpdate(formatter.FormatTable(formatter.ComposeSafeSchemaName(Root.Table.Name), formatter.ComposeSafeTableName(Root.Table.Name)),
				values.ToString(),
				BuildCriteria());
		}
		public UpdateSqlGenerator(ISqlGeneratorFormatter formatter, TaggedParametersHolder identities, Dictionary<OperandValue, string> parameters) : base(formatter, identities, parameters) { }
	}
	public sealed class DeleteSqlGenerator : BaseObjectSqlGenerator {
		protected override string InternalGenerateSql() {
			if(Root.Table is DBProjection)
				throw new InvalidOperationException(); 
			return formatter.FormatDelete(formatter.FormatTable(formatter.ComposeSafeSchemaName(Root.Table.Name), formatter.ComposeSafeTableName(Root.Table.Name)), BuildCriteria());
		}
		public DeleteSqlGenerator(ISqlGeneratorFormatter formatter, TaggedParametersHolder identities, Dictionary<OperandValue, string> parameters) : base(formatter, identities, parameters) { }
	}
	public class ProcessParameterInvariantCulture : IFormatProvider, ICustomFormatter {
		ProcessParameter processParameter;
		public ProcessParameterInvariantCulture(ProcessParameter processParameter) {
			this.processParameter = processParameter;
		}
		public object GetFormat(Type formatType) {
			if(formatType == typeof(ICustomFormatter))
				return this;
			else
				return null;
		}
		public string Format(string format, object arg, IFormatProvider formatProvider) {
			if(arg is string) {
				return (string)arg;
			}
			if(processParameter != null) {
				return processParameter(arg);
			}
			if(arg is IFormattable) {
				return ((IFormattable)arg).ToString(format, CultureInfo.InvariantCulture);
			}
			return arg.ToString();
		}
	}
}
namespace DevExpress.Xpo.Helpers {
	public class SimpleSqlParser {
		const string AsDelimiterString = " as ";
		string sql;
		List<string> result = new List<string>();
		StringBuilder curColumn = new StringBuilder();
		int inBrackets = 0;
		bool inQuotes = false;
		bool inDoubleQuotes = false;
		bool firstQuote = false;
		bool quoteEscaped = false;
		char chr;
		char? nextChr;
		char? prevChr;
		SimpleSqlParser(string sql) {
			this.sql = sql;
		}
		public static StringBuilder GetExpandedProperties(string[] properties, string expandingAlias) {
			StringBuilder expandedSelectedProperties = new StringBuilder();
			for(int i = 0; i < properties.Length; i++) {
				if(string.IsNullOrEmpty(properties[i].Trim())) { continue; }
				string field = properties[i];
				string existingAlias = null;
				int lastAsIndex = field.LastIndexOf(AsDelimiterString);
				if(lastAsIndex > 0) {
					string alias = field.Substring(lastAsIndex + AsDelimiterString.Length).Trim();
					if(!alias.Contains(" ")) {
						field = field.Remove(lastAsIndex);
						existingAlias = alias;
					}
				}
				if(string.IsNullOrEmpty(existingAlias)) {
					properties[i] = string.Format(CultureInfo.InvariantCulture, "{0} as F{1}", field, i);
					if(i > 0)
						expandedSelectedProperties.Append(", ");
					expandedSelectedProperties.Append(expandingAlias);
					expandedSelectedProperties.Append(".F");
					expandedSelectedProperties.Append(i.ToString(CultureInfo.InvariantCulture));
				} else {
					if(i > 0)
						expandedSelectedProperties.Append(", ");
					expandedSelectedProperties.Append(expandingAlias);
					expandedSelectedProperties.Append(".");
					expandedSelectedProperties.Append(existingAlias);
				}
			}
			return expandedSelectedProperties;
		}
		public static string[] GetColumns(string sql) {
			SimpleSqlParser parser = new SimpleSqlParser(sql);
			return parser.GetColumns();
		}
		string[] GetColumns() {
			for(int i = 0; i < sql.Length; i++) {
				chr = sql[i];
				nextChr = null;
				prevChr = null;
				if(i + 1 < sql.Length) {
					nextChr = sql[i + 1];
				}
				if(i - 1 > -1) {
					prevChr = sql[i - 1];
				}
				bool append = false;
				switch(chr) {
				case '\'':
					append = Quote();
					break;
				case '"':
					append = DoubleQuotes();
					break;
				case '(':
					append = OpeningBracket();
					break;
				case ')':
					append = ClosingBracket();
					break;
				case ',':
					append = Comma();
					break;
				default:
					append = Default();
					break;
				}
				Appender(append);
			}
			return result.ToArray();
		}
		bool Comma() {
			if(!nextChr.HasValue) { throw new FormatException("Statement not finished."); }
			if(inBrackets != 0) { return true; }
			if(inQuotes) { return true; }
			if(inDoubleQuotes) { return true; }
			return false;
		}
		void Appender(bool append) {
			if(append) {
				curColumn.Append(chr);
				return;
			}
			if(chr != ',') {
				curColumn.Append(chr);
			}
			result.Add(curColumn.ToString().Trim());
			curColumn.Remove(0, curColumn.Length);
		}
		bool OpeningBracket() {
			if(!nextChr.HasValue) { throw new FormatException("Statement not finished."); }
			if(inDoubleQuotes) {
				if(!nextChr.HasValue) { throw new FormatException("Statement not finished."); }
				return true;
			}
			if(inQuotes) {
				if(!nextChr.HasValue) { throw new FormatException("Statement not finished."); }
				return true;
			}
			inBrackets++;
			return true;
		}
		bool ClosingBracket() {
			if(inDoubleQuotes) {
				if(!nextChr.HasValue) { throw new FormatException("Statement not finished."); }
				return true;
			}
			if(inQuotes) {
				if(!nextChr.HasValue) { throw new FormatException("Statement not finished."); }
				return true;
			}
			if(inBrackets < 1) { throw new FormatException("Statement not finished."); }
			inBrackets--;
			if(nextChr.HasValue) { return true; }
			if(inBrackets != 0) { throw new FormatException("Statement not finished."); }
			return false;
		}
		bool Default() {
			if(firstQuote) {
				firstQuote = false;
			}
			return nextChr.HasValue;
		}
		bool Quote() {
			if(inDoubleQuotes) {
				if(!nextChr.HasValue) { throw new FormatException("Statement not finished."); }
				return true;
			}
			if(!inQuotes) {
				inQuotes = true;
				firstQuote = true;
			} else {
				quoteEscaped = !quoteEscaped;
				if(firstQuote) {
					firstQuote = false;
					if(!nextChr.HasValue || nextChr.Value != '\'') {
						inQuotes = false;
						quoteEscaped = false;
					}
				} else {
					if(quoteEscaped) {
						if(nextChr != '\'') {
							inQuotes = false;
							quoteEscaped = false;
						}
					}
				}
			}
			if(inQuotes && !nextChr.HasValue) { throw new FormatException("Statement not finished."); }
			return nextChr.HasValue;
		}
		bool DoubleQuotes() {
			if(inQuotes) {
				if(!nextChr.HasValue) { throw new FormatException("Statement not finished."); }
				return true;
			}
			if(!inDoubleQuotes) {
				inDoubleQuotes = true;
				firstQuote = true;
			} else {
				quoteEscaped = !quoteEscaped;
				if(firstQuote) {
					firstQuote = false;
					if(!nextChr.HasValue || nextChr.Value != '"') {
						inDoubleQuotes = false;
						quoteEscaped = false;
					}
				} else {
					if(quoteEscaped) {
						if(nextChr != '"') {
							inDoubleQuotes = false;
							quoteEscaped = false;
						}
					}
				}
			}
			if(inDoubleQuotes && !nextChr.HasValue) { throw new FormatException("Statement not finished."); }
			return nextChr.HasValue;
		}
	}
}
