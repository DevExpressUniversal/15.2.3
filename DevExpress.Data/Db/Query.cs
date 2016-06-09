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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DevExpress.Data.Filtering;
using System.Collections.Specialized;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System;
using DevExpress.Xpo.DB;
using System.Xml.Serialization;
using DevExpress.Utils;
namespace DevExpress.Xpo.DB.Helpers {
	public sealed class StringListHelper {
		public static void AddFormat(StringCollection collection, string s, params object[] parameters) {
			collection.Add(String.Format(CultureInfo.InvariantCulture, s, parameters));
		}
		public static StringCollection CreateStringCollection(params string[] strings) {
			StringCollection collection = new StringCollection();
			collection.AddRange(strings);
			return collection;
		}
		public static string DelimitedText(StringCollection collection, string delimiter) {
			StringBuilder b = new StringBuilder();
			for(Int32 i = 0; i < collection.Count; ++i) {
				b.Append(collection[i]);
				if(i != collection.Count - 1)
					b.Append(delimiter);
			}
			return b.ToString();
		}
	}
	public sealed class ConnectionStringParser {
		Dictionary<string, string> propertyTable = new Dictionary<string, string>(StringExtensions.ComparerInvariantCultureIgnoreCase);
		string[] ExtractParts(string connectionString) {
			List<string> list = new List<string>();
			int count = connectionString.Length;
			int lastPos = 0;
			bool inValue = false;
			for(int i = 0; i < count; ++i) {
				if(connectionString[i].Equals('\"')) {
					inValue = !inValue;
				}
				if(connectionString[i].Equals(';') && !inValue) {
					list.Add(connectionString.Substring(lastPos, (i - lastPos)));
					lastPos = i + 1;
				}
			}
			if(lastPos < count) {
				list.Add(connectionString.Substring(lastPos, (count - lastPos)));
			}
			return list.ToArray();
		}
		public ConnectionStringParser(string connectionString) {
			string[] parts = ExtractParts(connectionString);
			for(int i = 0; i < parts.Length; ++i) {
				string text = parts[i];
				int ind = text.IndexOf("=");
				if(ind != -1) {
					string name = text.Substring(0, ind).Trim();
					this.propertyTable.Add(name, text.Substring(ind + 1).Trim());
				}
			}
		}
		public string GetPartByName(string partName) {
			string s;
			return propertyTable.TryGetValue(partName, out s) ? s : string.Empty;
		}
		public bool PartExists(string partName) {
			return propertyTable.ContainsKey(partName);
		}
		public void UpdatePartByName(string partName, string partValue) {
			if(propertyTable.ContainsKey(partName)) {
				propertyTable[partName] = partValue;
			}
		}
		public void RemovePartByName(string partName) {
			propertyTable.Remove(partName);
		}
		public string GetConnectionString() {
			StringBuilder builder = new StringBuilder();
			foreach(KeyValuePair<string, string> de in propertyTable) {
				builder.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}", de.Key, de.Value);
				builder.Append(";");
			}
			return builder.ToString();
		}
	}
	public class QueryStatementToStringFormatter : ISqlGeneratorFormatterSupportSkipTake {
		private QueryStatementToStringFormatter() { }
		public static QueryStatementToStringFormatter Instance = new QueryStatementToStringFormatter();
		public static string GetString(SelectStatement select) {
			Query query = new SelectSqlGenerator(Instance).GenerateSql(select);
			string parameters = FormatParameters(query.Parameters);
			string constants = FormatConstants(query.ConstantValues);
			return query.Sql + " params(" + parameters + ")" + " constants(" + constants + ")";
		}
		public static string GetString(ModificationStatement statement, TaggedParametersHolder identities) {
			Query query;
			if(statement is InsertStatement) {
				query = new InsertSqlGenerator(Instance, identities, new Dictionary<OperandValue, string>()).GenerateSql(statement);
			} else if(statement is UpdateStatement) {
				query = new UpdateSqlGenerator(Instance, identities, new Dictionary<OperandValue, string>()).GenerateSql(statement);
			} else if(statement is DeleteStatement) {
				query = new DeleteSqlGenerator(Instance, identities, new Dictionary<OperandValue, string>()).GenerateSql(statement);
			} else {
				throw new NotImplementedException();
			}
			string parameters = FormatParameters(query.Parameters);
			string constants = FormatConstants(query.ConstantValues);
			return query.Sql + " params(" + parameters + ")" + " constants(" + constants + ")";
		}
		#region ISqlGeneratorFormatter
		static string FormatEscape(string escaped) {
			return "'" + escaped.Replace("'", "''") + "'";
		}
		string ISqlGeneratorFormatter.FormatTable(string schema, string tableName) {
			string res = FormatEscape(tableName);
			return String.IsNullOrEmpty(schema) ? res : FormatEscape(schema) + "." + res;
		}
		string ISqlGeneratorFormatter.FormatTable(string schema, string tableName, string tableAlias) {
			string res = FormatEscape(tableName) + "." + FormatEscape(tableAlias);
			return String.IsNullOrEmpty(schema) ? res : FormatEscape(schema) + "." + res;
		}
		string ISqlGeneratorFormatter.FormatColumn(string columnName) {
			return FormatEscape(columnName);
		}
		string ISqlGeneratorFormatter.FormatColumn(string columnName, string tableAlias) {
			return FormatEscape(tableAlias) + "." + FormatEscape(columnName);
		}
		string ISqlGeneratorFormatter.FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int topSelectedRecords) {
			string modificatorsString = string.Empty;
			if(topSelectedRecords != 0)
				modificatorsString = " top " + topSelectedRecords.ToString() + " ";
			return string.Format(CultureInfo.InvariantCulture, "select{0}({1}) from({2}) where({3}) order({4}) group({5}) having({6})",
				modificatorsString, selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql);
		}
		string ISqlGeneratorFormatter.FormatInsertDefaultValues(string tableName) {
			return string.Format(CultureInfo.InvariantCulture, "insert default values into {0}", tableName);
		}
		string ISqlGeneratorFormatter.FormatInsert(string tableName, string fields, string values) {
			return string.Format(CultureInfo.InvariantCulture, "insert into {0}({1}) values({2})", tableName, fields, values);
		}
		string ISqlGeneratorFormatter.FormatUpdate(string tableName, string sets, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "update {0} {1} where {2}", tableName, sets, whereClause);
		}
		string ISqlGeneratorFormatter.FormatDelete(string tableName, string whereClause) {
			return string.Format(CultureInfo.InvariantCulture, "delete from {0} where {1}", tableName, whereClause);
		}
		string ISqlGeneratorFormatter.FormatUnary(UnaryOperatorType operatorType, string operand) {
			return operatorType.ToString() + "(" + operand + ")";
		}
		string ISqlGeneratorFormatter.FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			return operatorType.ToString() + "(" + leftOperand + "," + rightOperand + ")";
		}
		string ISqlGeneratorFormatter.FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			string result = operatorType.ToString() + "(";
			bool first = true;
			foreach(string operand in operands) {
				if(first)
					first = false;
				else
					result += ",";
				result += operand;
			}
			return result;
		}
		string ISqlGeneratorFormatter.FormatOrder(string sortProperty, SortingDirection direction) {
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", sortProperty, direction == SortingDirection.Ascending ? "asc" : "desc");
		}
		bool ISqlGeneratorFormatter.BraceJoin {
			get { return true; }
		}
		bool ISqlGeneratorFormatter.SupportNamedParameters { get { return false; } }
		class CacheParameterNames : IList {
			CacheParameterNames() { }
			public static readonly CacheParameterNames Instance = new CacheParameterNames();
			int ICollection.Count {
				get {
					throw new NotImplementedException();
				}
			}
			IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
			bool ICollection.IsSynchronized {
				get {
					return false;
				}
			}
			object ICollection.SyncRoot {
				get {
					return this;
				}
			}
			void ICollection.CopyTo(Array array, int index) {
				throw new NotImplementedException();
			}
			bool IList.IsFixedSize { get { return false; } }
			bool IList.IsReadOnly { get { return true; } }
			object IList.this[int index] {
				get { return this[index]; }
				set { throw new NotImplementedException(); }
			}
			public string this[int index] {
				get { return "?"; }
				set { throw new NotImplementedException(); }
			}
			int IList.Add(object value) { throw new NotImplementedException(); }
			void IList.Clear() { throw new NotImplementedException(); }
			bool IList.Contains(object value) { throw new NotImplementedException(); }
			int IList.IndexOf(object value) { throw new NotImplementedException(); }
			void IList.Insert(int index, object value) { throw new NotImplementedException(); }
			void IList.Remove(object value) { throw new NotImplementedException(); }
			void IList.RemoveAt(int index) { throw new NotImplementedException(); }
		}
		string ISqlGeneratorFormatter.ComposeSafeTableName(string tableName) {
			return tableName;
		}
		string ISqlGeneratorFormatter.ComposeSafeSchemaName(string tableName) {
			return String.Empty;
		}
		string ISqlGeneratorFormatter.ComposeSafeColumnName(string columnName) {
			return columnName;
		}
		string ISqlGeneratorFormatter.GetParameterName(OperandValue parameter, int index, ref bool createPrameter) {
			return CacheParameterNames.Instance[index];
		}
		static string FormatParameters(QueryParameterCollection parameters) {
			StringBuilder result = new StringBuilder();
			foreach(OperandValue parameter in parameters) {
				if(result.Length != 0)
					result.Append(',');
				if(parameter.Value != null && !(parameter.Value is DBNull))
					result.Append(FormatEscape(parameter.Value.ToString()));
			}
			return result.ToString();
		}
		static string FormatConstants(Dictionary<int, OperandValue> constantValues) {
			if (constantValues == null) return string.Empty;
			StringBuilder result = new StringBuilder();
			List<int> constantKeys = new List<int>(constantValues.Keys);
			constantKeys.Sort();
			for (int i = 0; i < constantKeys.Count; i++) {
				if (result.Length != 0)
					result.Append(',');
				OperandValue constantValue = constantValues[constantKeys[i]];
				if (((object)constantValue) != null && constantValue.Value != null && !(constantValue.Value is DBNull))
					result.Append(FormatEscape(constantValue.ToString()));
			}
			return result.ToString();
		}
		bool ISqlGeneratorFormatterSupportSkipTake.NativeSkipTakeSupported {
			get { return true; }
		}
		string ISqlGeneratorFormatterSupportSkipTake.FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords) {
			string modificatorsString = string.Empty;
			if(skipSelectedRecords != 0)
				modificatorsString += " skip " + skipSelectedRecords.ToString(CultureInfo.InvariantCulture);
			if(topSelectedRecords != 0)
				modificatorsString += " top " + topSelectedRecords.ToString(CultureInfo.InvariantCulture);
			return string.Format(CultureInfo.InvariantCulture, "select{0} ({1}) from({2}) where({3}) order({4}) group({5}) having({6})",
				modificatorsString, selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql);
		}
		#endregion ISqlGeneratorFormatter
	}
	public class SubQueriesFinder : IQueryCriteriaVisitor {
		JoinNodeCollection result;
		SubQueriesFinder() {
			result = new JoinNodeCollection();
		}
		public static JoinNodeCollection FindSubQueries(CriteriaOperator criteria) {
			SubQueriesFinder finder = new SubQueriesFinder();
			finder.Process(criteria);
			return finder.result;
		}
		public static JoinNodeCollection FindSubQueries(CriteriaOperatorCollection criterias) {
			SubQueriesFinder finder = new SubQueriesFinder();
			foreach(CriteriaOperator criteria in criterias)
				finder.Process(criteria);
			return finder.result;
		}
		public static JoinNodeCollection FindSubQueries(QuerySortingCollection sortings) {
			SubQueriesFinder finder = new SubQueriesFinder();
			foreach(SortingColumn sort in sortings)
				finder.Process(sort.Property);
			return finder.result;
		}
		void Process(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null))
				return;
			criteria.Accept(this);
		}
		void Process(IEnumerable<CriteriaOperator> criterias) {
			if(criterias == null)
				return;
			foreach(CriteriaOperator criteria in criterias) {
				Process(criteria);
			}
		}
		void ICriteriaVisitor.Visit(BetweenOperator theOperator) {
			Process(theOperator.TestExpression);
			Process(theOperator.BeginExpression);
			Process(theOperator.EndExpression);
		}
		void ICriteriaVisitor.Visit(BinaryOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.RightOperand);
		}
		void ICriteriaVisitor.Visit(UnaryOperator theOperator) {
			Process(theOperator.Operand);
		}
		void ICriteriaVisitor.Visit(InOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.Operands);
		}
		void ICriteriaVisitor.Visit(GroupOperator theOperator) {
			Process(theOperator.Operands);
		}
		void ICriteriaVisitor.Visit(OperandValue theOperand) {
		}
		void ICriteriaVisitor.Visit(FunctionOperator theOperator) {
			Process(theOperator.Operands);
		}
		void IQueryCriteriaVisitor.Visit(QueryOperand theOperand) {
		}
		void IQueryCriteriaVisitor.Visit(QuerySubQueryContainer theOperand) {
			if(theOperand.Node != null)
				result.Add(theOperand.Node);
			else
				Process(theOperand.AggregateProperty);
		}
	}
	public class TopLevelQueryOperandsFinder : IQueryCriteriaVisitor {
		List<QueryOperand> result = new List<QueryOperand>();
		private TopLevelQueryOperandsFinder() { }
		public static List<QueryOperand> Find(CriteriaOperator criteria) {
			TopLevelQueryOperandsFinder finder = new TopLevelQueryOperandsFinder();
			finder.Process(criteria);
			return finder.result;
		}
		public static List<QueryOperand> Find(IEnumerable<CriteriaOperator> criteria) {
			TopLevelQueryOperandsFinder finder = new TopLevelQueryOperandsFinder();
			foreach(CriteriaOperator c in criteria)
				finder.Process(c);
			return finder.result;
		}
		void Process(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null))
				return;
			criteria.Accept(this);
		}
		void Process(IEnumerable<CriteriaOperator> criterias) {
			if(criterias == null)
				return;
			foreach(CriteriaOperator criteria in criterias) {
				Process(criteria);
			}
		}
		void ICriteriaVisitor.Visit(BetweenOperator theOperator) {
			Process(theOperator.TestExpression);
			Process(theOperator.BeginExpression);
			Process(theOperator.EndExpression);
		}
		void ICriteriaVisitor.Visit(BinaryOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.RightOperand);
		}
		void ICriteriaVisitor.Visit(UnaryOperator theOperator) {
			Process(theOperator.Operand);
		}
		void ICriteriaVisitor.Visit(InOperator theOperator) {
			Process(theOperator.LeftOperand);
			Process(theOperator.Operands);
		}
		void ICriteriaVisitor.Visit(GroupOperator theOperator) {
			Process(theOperator.Operands);
		}
		void ICriteriaVisitor.Visit(OperandValue theOperand) {
		}
		void ICriteriaVisitor.Visit(FunctionOperator theOperator) {
			Process(theOperator.Operands);
		}
		void IQueryCriteriaVisitor.Visit(QueryOperand theOperand) {
			result.Add(theOperand);
		}
		void IQueryCriteriaVisitor.Visit(QuerySubQueryContainer theOperand) {
			Process(theOperand.AggregateProperty);
		}
	}
	public class TaggedParametersHolder {
		Dictionary<int, ParameterValue> parametersByTag = new Dictionary<int, ParameterValue>();
		public TaggedParametersHolder() { }
		public OperandValue ConsolidateParameter(OperandValue deserializedParameter) {
			ParameterValue v = deserializedParameter as ParameterValue;
			if(ReferenceEquals(v, null))
				return deserializedParameter;
			ParameterValue res;
			if(!parametersByTag.TryGetValue(v.Tag, out res)) {
				parametersByTag.Add(v.Tag, v);
				return v;
			}
			else {
				return res;
			}
		}
		public void ConsolidateIdentity(ParameterValue identityInsertParameter) {
			OperandValue consolidated = ConsolidateParameter(identityInsertParameter);
			System.Diagnostics.Debug.Assert(ReferenceEquals(consolidated, identityInsertParameter));
		}
	}
	public sealed class QueryCollection : List<Query> {
		public QueryCollection() : base() { }
		public QueryCollection(params Query[] queries) : base() { AddRange(queries); }
	}
	public sealed class Query {
		string sqlString;
		QueryParameterCollection parameters;
		IList parametersNames;
		int skipSelectedRecords;
		int topSelectedRecords;
		Dictionary<int, OperandValue> constantValues = null;
		Dictionary<int, int> operandIndexes = null;
		public Dictionary<int, OperandValue> ConstantValues { get { return constantValues; } }
		public Dictionary<int, int> OperandIndexes { get { return operandIndexes; } }
		public string Sql { get { return sqlString; } }
		public QueryParameterCollection Parameters { get { return parameters; } }
		public IList ParametersNames { get { return parametersNames; } }
		public int SkipSelectedRecords { get { return skipSelectedRecords; } }
		public int TopSelectedRecords { get { return topSelectedRecords; } }
		public Query(string sql) : this(sql, new QueryParameterCollection(), null) { }
		public Query(string sql, QueryParameterCollection parameters, IList parametersNames) : this(sql, parameters, parametersNames, 0, 0) { }
		public Query(string sql, QueryParameterCollection parameters, IList parametersNames, int topSelectedRecords)
			: this(sql, parameters, parametersNames, 0, topSelectedRecords) { }
		public Query(string sql, QueryParameterCollection parameters, IList parametersNames, int skipSelectedRecords, int topSelectedRecords) {
			this.sqlString = sql;
			this.parameters = parameters;
			this.parametersNames = parametersNames;
			this.skipSelectedRecords = skipSelectedRecords;
			this.topSelectedRecords = topSelectedRecords;
		}
		public Query(string sql, QueryParameterCollection parameters, IList parametersNames, int topSelectedRecords, Dictionary<int, OperandValue> constantValues, Dictionary<int, int> operandIndexes)
			: this(sql, parameters, parametersNames, 0, topSelectedRecords, constantValues, operandIndexes) { }
		public Query(string sql, QueryParameterCollection parameters, IList parametersNames, int skipSelectedRecords, int topSelectedRecords, Dictionary<int, OperandValue> constantValues, Dictionary<int, int> operandIndexes)
			: this(sql, parameters, parametersNames, skipSelectedRecords, topSelectedRecords) {
			this.constantValues = constantValues;
			this.operandIndexes = operandIndexes;
		}
	}
}
namespace DevExpress.Xpo.DB {
	using System;
	using System.Collections;
	using DevExpress.Xpo.DB.Exceptions;
	using System.Xml.Serialization;
	using System.Globalization;
	using DevExpress.Data.Filtering;
	using System.ComponentModel;
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Compatibility.System;
	using DevExpress.Compatibility.System.Xml.Serialization;
	[Serializable]
	public sealed class QueryParameterCollection : List<OperandValue> {
		public QueryParameterCollection() : base() { }
		public QueryParameterCollection(params OperandValue[] parameters) : base() { AddRange(parameters); }
		public override bool Equals(object obj) {
			QueryParameterCollection another = obj as QueryParameterCollection;
			if(another == null)
				return false;
			if(this.Count != another.Count)
				return false;
			for(int i = 0; i < this.Count; ++i) {
				if(!Equals(this[i], another[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int result = 0;
			foreach(object o in this) {
				result ^= o.GetHashCode();
			}
			return result;
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach(object o in this) {
				if(sb.Length > 0) sb.Append(", ");
				sb.Append(o.ToString());
			}
			return sb.ToString();
		}
	}
	[Serializable]
	public sealed class JoinNodeCollection : List<JoinNode> {
		public JoinNodeCollection() { }
		public override bool Equals(object obj) {
			JoinNodeCollection another = obj as JoinNodeCollection;
			if(another == null)
				return false;
			if(this.Count != another.Count)
				return false;
			for(int i = 0; i < this.Count; ++i) {
				if(!Equals(this[i], another[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int result = 0;
			foreach(object o in this) {
				result ^= o.GetHashCode();
			}
			return result;
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			foreach(JoinNode node in this) {
				result.Append(node.ToString());
			}
			return result.ToString();
		}
	}
	public enum JoinType { Inner, LeftOuter }
	[Serializable]
	public class JoinNode {		
		public JoinNodeCollection SubNodes = new JoinNodeCollection();
		[XmlAttribute]
		public string Alias;
		[XmlAttribute]
		[DefaultValue(JoinType.Inner)]
		public JoinType Type = JoinType.Inner;
		public DBTable Table;
		public JoinNode(DBTable table, string alias, JoinType type) {
			this.Type = type;
			this.Alias = alias;
			this.Table = table;
		}
		public DBColumn GetColumn(string name) {
			if(Table == null)
				throw new InvalidOperationException();
			return Table.GetColumn(name);
		}
		public CriteriaOperator Condition;
		public JoinNode() { }
		public override bool Equals(object obj) {
			JoinNode another = obj as JoinNode;
			if(another == null)
				return false;
			return Equals(Alias, another.Alias)
				&& Equals(Type, another.Type)
				&& Equals(Table, another.Table)
				&& Equals(Condition, another.Condition)
				&& Equals(SubNodes, another.SubNodes);
		}
		static protected int GetHashCode(object obj) {
			return obj == null ? 0 : obj.GetHashCode();
		}
		public override int GetHashCode() {
			return GetHashCode(Alias) ^ ((int)Type).GetHashCode() ^ GetHashCode(Table) ^ GetHashCode(Condition) ^ GetHashCode(SubNodes);
		}
		internal virtual void CollectJoinNodesAndCriteriaInternal(List<JoinNode> nodes, List<CriteriaOperator> criteria) {
			nodes.Add(this);
			criteria.Add(this.Condition);
			foreach(JoinNode nextNode in SubQueriesFinder.FindSubQueries(Condition)) {
				nextNode.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
			}
			foreach(JoinNode nextNode in this.SubNodes) {
				nextNode.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
			}
		}
		public void CollectJoinNodesAndCriteria(out List<JoinNode> nodes, out List<CriteriaOperator> criteria) {
			nodes = new List<JoinNode>();
			criteria = new List<CriteriaOperator>();
			CollectJoinNodesAndCriteriaInternal(nodes, criteria);
		}
		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "\t{0} join \"{1}\" {2} on {3}\n{4}"
				, this.Type, Table, Alias, Condition, SubNodes);
		}
	}
	[Serializable]
	public abstract class BaseStatement : JoinNode {
		protected BaseStatement(DBTable table, string alias) : base(table, alias, JoinType.Inner) { }
		protected BaseStatement() { }
		[XmlArrayItem(typeof(ContainsOperator))]
		[XmlArrayItem(typeof(BetweenOperator))]
		[XmlArrayItem(typeof(BinaryOperator))]
		[XmlArrayItem(typeof(UnaryOperator))]
		[XmlArrayItem(typeof(InOperator))]
		[XmlArrayItem(typeof(GroupOperator))]
		[XmlArrayItem(typeof(OperandValue))]
		[XmlArrayItem(typeof(ConstantValue))]
		[XmlArrayItem(typeof(OperandProperty))]
		[XmlArrayItem(typeof(AggregateOperand))]
		[XmlArrayItem(typeof(JoinOperand))]
		[XmlArrayItem(typeof(FunctionOperator))]
		[XmlArrayItem(typeof(NotOperator))]
		[XmlArrayItem(typeof(NullOperator))]
		[XmlArrayItem(typeof(QueryOperand))]
		[XmlArrayItem(typeof(QuerySubQueryContainer))]
		public CriteriaOperatorCollection Operands = new CriteriaOperatorCollection();
		public override bool Equals(object obj) {
			BaseStatement another = obj as BaseStatement;
			if(another == null)
				return false;
			return base.Equals(another)
				&& Equals(Operands, another.Operands)
				;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public string[] GetTablesNames() {
			return GetTablesNames(this);
		}
		public static string[] GetTablesNames(params BaseStatement[] statements) {
			HashSet<string> tablesNames = new HashSet<string>();
			foreach(BaseStatement statement in statements) {
				List<JoinNode> nodes = new List<JoinNode>();
				List<CriteriaOperator> criteria = new List<CriteriaOperator>();
				statement.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
				foreach(JoinNode node in nodes){
					DBTable table = node.Table;
					if(table == null)
						continue;
					DBProjection projection = node.Table as DBProjection;
					if(projection == null) {
						tablesNames.Add(table.Name);
					} else {
						foreach(var tableName in GetTablesNames(projection.Projection)) {
							tablesNames.Add(tableName);
						}
					}
				}
			}
			string[] result = new string[tablesNames.Count];
			tablesNames.CopyTo(result);
			return result;
		}
		internal override void CollectJoinNodesAndCriteriaInternal(List<JoinNode> nodes, List<CriteriaOperator> criteria) {
			base.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
			foreach(CriteriaOperator c in Operands) {
				criteria.Add(c);
				foreach(JoinNode nextNode in SubQueriesFinder.FindSubQueries(c)) {
					nextNode.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
				}
			}
		}
		public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetTablesColumns(params BaseStatement[] statements) {
			Dictionary<string, Dictionary<string, object>> result = new Dictionary<string, Dictionary<string, object>>();
			List<BaseStatement> statementList = new List<BaseStatement>(statements);
			for(int i = 0; i < statementList.Count; i++) {
				BaseStatement statement = statementList[i];
				List<JoinNode> nodes = new List<JoinNode>();
				List<CriteriaOperator> criteria = new List<CriteriaOperator>();
				statement.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
				Dictionary<string, DBTable> aliasesToTableNames = new Dictionary<string, DBTable>();
				foreach(JoinNode node in nodes) {
					string alias = node.Alias;
					if(alias == null)
						alias = string.Empty;
					DBTable oldTable;
					if(aliasesToTableNames.TryGetValue(alias, out oldTable)) {
						if(!object.Equals(oldTable,node.Table))
							throw new InvalidOperationException("different tables with the same alias -- possible internal error");
					} else {
						aliasesToTableNames.Add(alias, node.Table);
						DBProjection projection = node.Table as DBProjection;
						if(projection != null)
							statementList.Add(projection.Projection);
					}
				}
				foreach(DBTable table in aliasesToTableNames.Values) {
					if(table is DBProjection)
						continue;
					if(!result.ContainsKey(table.Name))
						result.Add(table.Name, new Dictionary<string, object>());
				}
				foreach(QueryOperand op in TopLevelQueryOperandsFinder.Find(criteria)) {
					string alias = op.NodeAlias;
					if(alias == null)
						alias = string.Empty;
					var table = aliasesToTableNames[alias];
					string tableName = table.Name;
					string columnName = op.ColumnName;
					Dictionary<string, object> columnsForTable;
					if(!result.TryGetValue(tableName, out columnsForTable)) {
						columnsForTable = new Dictionary<string, object>();
						result.Add(tableName, columnsForTable);
					}
					columnsForTable[columnName] = columnName;
				}
			}
			foreach(KeyValuePair<string, Dictionary<string, object>> row in result)
				yield return new KeyValuePair<string, IEnumerable<string>>(row.Key, row.Value.Keys);
		}
	}
	[Serializable]
	public class SelectStatement : BaseStatement {
		public SelectStatement() { }
		public SelectStatement(DBTable table, string alias) : base(table, alias) { }
		public int SkipSelectedRecords;
		public int TopSelectedRecords;
		QuerySortingCollection sortProperties;
		CriteriaOperatorCollection groupProperties;
		public QuerySortingCollection SortProperties {
			get {
				if(sortProperties == null) {
					sortProperties = new QuerySortingCollection();
				}
				return sortProperties;
			}
		}
		public CriteriaOperatorCollection GroupProperties {
			get {
				if(groupProperties == null) {
					groupProperties = new CriteriaOperatorCollection();
				}
				return groupProperties;
			}
		}
		public CriteriaOperator GroupCondition;
		public override bool Equals(object obj) {
			SelectStatement another = obj as SelectStatement;
			if(another == null)
				return false;
			return base.Equals(another)
				&& SkipSelectedRecords == another.SkipSelectedRecords
				&& TopSelectedRecords == another.TopSelectedRecords
				&& Equals(SortProperties, another.SortProperties)
				&& Equals(GroupProperties, another.GroupProperties)
				&& Equals(GroupCondition, another.GroupCondition);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder("Select ");
			if(SkipSelectedRecords != 0)
				result.Append("skip(" + SkipSelectedRecords.ToString() + ") ");
			if(TopSelectedRecords != 0)
				result.Append("top(" + TopSelectedRecords.ToString() + ") ");
			result.Append(Operands.ToString());
			result.Append("\n  from \"" + Table.ToString() + "\" " + Alias + "\n" + SubNodes.ToString());
			if(!ReferenceEquals(Condition, null))
				result.Append(" where " + CriteriaOperator.ToString(Condition));
			if(GroupProperties.Count > 0)
				result.Append(" group by " + GroupProperties.ToString() + "\n");
			if(!ReferenceEquals(GroupCondition, null))
				result.Append("having " + CriteriaOperator.ToString(GroupCondition) + "\n");
			return result.ToString();
		}
		internal override void CollectJoinNodesAndCriteriaInternal(List<JoinNode> nodes, List<CriteriaOperator> criteria) {
			base.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
			foreach(SortingColumn sort in SortProperties) {
				criteria.Add(sort.Property);
				foreach(JoinNode nextNode in SubQueriesFinder.FindSubQueries(sort.Property)) {
					nextNode.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
				}
			}
			foreach(CriteriaOperator c in GroupProperties) {
				criteria.Add(c);
				foreach(JoinNode nextNode in SubQueriesFinder.FindSubQueries(c)) {
					nextNode.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
				}
			}
			criteria.Add(this.GroupCondition);
			foreach(JoinNode nextNode in SubQueriesFinder.FindSubQueries(GroupCondition)) {
				nextNode.CollectJoinNodesAndCriteriaInternal(nodes, criteria);
			}
		}
	}
	[Serializable]
	[XmlInclude(typeof(InsertStatement))]
	[XmlInclude(typeof(DeleteStatement))]
	[XmlInclude(typeof(UpdateStatement))]
	public abstract class ModificationStatement : BaseStatement {
		protected ModificationStatement() { }
		protected ModificationStatement(DBTable table, string alias) : base(table, alias) { }
		[XmlArrayItem(typeof(OperandValue))]
		[XmlArrayItem(typeof(ConstantValue))]
		[XmlArrayItem(typeof(ParameterValue))]
		public QueryParameterCollection Parameters = new QueryParameterCollection();
		[XmlAttribute]
		[DefaultValue(0)]
		public int RecordsAffected;
		public override string ToString() {
			return this.GetType().ToString();
		}
	}
	[Serializable]
	public class InsertStatement : ModificationStatement {
		public InsertStatement() { }
		public InsertStatement(DBTable table, string alias) : base(table, alias) { }
		public ParameterValue IdentityParameter;
		[XmlAttribute]
		public string IdentityColumn;
		[XmlAttribute]
		public DBColumnType IdentityColumnType;
		public override bool Equals(object obj) {
			InsertStatement another = obj as InsertStatement;
			if(another == null)
				return false;
			return base.Equals(another)
				&& IdentityColumn == another.IdentityColumn
				&& IdentityColumnType == another.IdentityColumnType
				&& Equals(Parameters, another.Parameters)
			;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return string.Format("insert into {0} values ({1})", Table.Name, Parameters.ToString());
		}
	}
	[Serializable]
	public class UpdateStatement : ModificationStatement {
		public UpdateStatement() { }
		public UpdateStatement(DBTable table, string alias) : base(table, alias) { }
		public override bool Equals(object obj) {
			UpdateStatement another = obj as UpdateStatement;
			if(another == null)
				return false;
			return base.Equals(another)
				&& Equals(Parameters, another.Parameters)
			;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return string.Format("update {0} set {1} where {2}", Table.Name, Parameters.ToString(), ReferenceEquals(Condition, null) ? string.Empty: Condition.ToString());
		}
	}
	[Serializable]
	public class DeleteStatement : ModificationStatement {
		public DeleteStatement() { }
		public DeleteStatement(DBTable table, string alias) : base(table, alias) { }
		public override bool Equals(object obj) {
			DeleteStatement another = obj as DeleteStatement;
			if(another == null)
				return false;
			return base.Equals(another);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return string.Format("delete from {0} where {1}", Table.Name, ReferenceEquals(Condition, null) ? string.Empty : Condition.ToString());
		}
	}
	[Serializable]
	public sealed class QueryOperandCollection : List<QueryOperand> {
		public QueryOperandCollection() { }
		public override bool Equals(object obj) {
			QueryOperandCollection another = obj as QueryOperandCollection;
			if(another == null)
				return false;
			if(this.Count != another.Count)
				return false;
			for(int i = 0; i < this.Count; ++i) {
				if(!Equals(this[i], another[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int result = 0;
			foreach(object o in this) {
				result ^= o.GetHashCode();
			}
			return result;
		}
	}
	[Serializable]
	public sealed class ParameterValue : OperandValue {
		[XmlAttribute]
		public int Tag;
		public ParameterValue(int tag) {
			this.Tag = tag;
		}
		public ParameterValue() { }
		public override bool Equals(object obj) {
			ParameterValue another = obj as ParameterValue;
			if(ReferenceEquals(another, null))
				return false;
			return this.Tag == another.Tag && base.Equals(another);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ Tag.GetHashCode();
		}
	}
	[Serializable]
	public sealed class QueryOperand : CriteriaOperator {
		[XmlAttribute]
		public string ColumnName;
		[XmlAttribute]
		public DBColumnType ColumnType;
		[XmlAttribute]
		public string NodeAlias;
		public QueryOperand() : this(null, null, DBColumnType.Unknown) { }
		public QueryOperand(DBColumn column, string nodeAlias) : this(column.Name, nodeAlias, column.ColumnType) { }
		public QueryOperand(string columnName, string nodeAlias) : this(columnName, nodeAlias, DBColumnType.Unknown) { }
		public QueryOperand(string columnName, string nodeAlias, DBColumnType columnType) {
			this.ColumnName = columnName;
			this.NodeAlias = nodeAlias;
			this.ColumnType = columnType;
		}
		public override void Accept(ICriteriaVisitor visitor) {
			IQueryCriteriaVisitor queryVisitor = (IQueryCriteriaVisitor)visitor;
			queryVisitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var queryVisitor = (IQueryCriteriaVisitor<T>)visitor;
			return queryVisitor.Visit(this);
		}
		public override bool Equals(object obj) {
			QueryOperand another = obj as QueryOperand;
			if(ReferenceEquals(another, null))
				return false;
			return ColumnName == another.ColumnName && NodeAlias == another.NodeAlias;
		}
		public override int GetHashCode() {
			if(ColumnName != null)
				return ColumnName.GetHashCode();
			return -1;
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public QueryOperand Clone() {
			return new QueryOperand(this.ColumnName, this.NodeAlias, this.ColumnType);
		}
	}
	[Serializable]
	public class QuerySubQueryContainer : CriteriaOperator {
		BaseStatement node;
		Aggregate aggregateType;
		CriteriaOperator aggregateProperty;
		public QuerySubQueryContainer(BaseStatement node, CriteriaOperator aggregateProperty, Aggregate aggregateType) {
			this.node = node;
			this.aggregateProperty = aggregateProperty;
			this.aggregateType = aggregateType;
		}
		public QuerySubQueryContainer() : this(null, null, Aggregate.Exists) { }
		public BaseStatement Node {
			get { return this.node; }
			set { this.node = value; }
		}
		[XmlAttribute]
		public Aggregate AggregateType {
			get { return this.aggregateType; }
			set { this.aggregateType = value; }
		}
		public CriteriaOperator AggregateProperty {
			get { return this.aggregateProperty; }
			set { this.aggregateProperty = value; }
		}
		public override void Accept(ICriteriaVisitor visitor) {
			IQueryCriteriaVisitor queryVisitor = (IQueryCriteriaVisitor)visitor;
			queryVisitor.Visit(this);
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var queryVisitor = (IQueryCriteriaVisitor<T>)visitor;
			return queryVisitor.Visit(this);
		}
		public override bool Equals(object obj) {
			QuerySubQueryContainer another = obj as QuerySubQueryContainer;
			if(ReferenceEquals(another, null))
				return false;
			return Equals(Node, another.node)
				&& Equals(AggregateProperty, another.AggregateProperty)
				&& AggregateType == another.AggregateType;
		}
		public override int GetHashCode() {
			if(Node != null)
				return Node.GetHashCode();
			else if(!ReferenceEquals(AggregateProperty, null))
				return AggregateProperty.GetHashCode();
			else
				return AggregateType.GetHashCode();
		}
		protected override CriteriaOperator CloneCommon() {
			throw new NotSupportedException();
		}
	}
	[Serializable]
	public sealed class QuerySortingCollection : List<SortingColumn> {
		public QuerySortingCollection() { }
		public override bool Equals(object obj) {
			QuerySortingCollection another = obj as QuerySortingCollection;
			if(another == null)
				return false;
			if(this.Count != another.Count)
				return false;
			for(int i = 0; i < this.Count; ++i) {
				if(!Equals(this[i], another[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int result = 0;
			foreach(object o in this) {
				result ^= o.GetHashCode();
			}
			return result;
		}
	}
	public enum SortingDirection { Ascending, Descending };
	[Serializable]
	public sealed class SortingColumn {
		SortingDirection direction;
		CriteriaOperator property;
		public SortingDirection Direction {
			get { return direction; }
			set { direction = value; }
		}
		public CriteriaOperator Property {
			get { return property; }
			set { property = value; }
		}
		public SortingColumn(string columnName, string nodeAlias, SortingDirection direction)
			: this(new QueryOperand(columnName, nodeAlias), direction) {
		}
		public SortingColumn(CriteriaOperator property, SortingDirection direction) {
			this.property = property;
			this.direction = direction;
		}
		public SortingColumn() : this(null, null, SortingDirection.Ascending) { }
		public override bool Equals(object obj) {
			SortingColumn another = obj as SortingColumn;
			if(ReferenceEquals(another, null))
				return false;
			return ReferenceEquals(Property, another.Property) && ReferenceEquals(Direction, another.Direction);
		}
		public override int GetHashCode() {
			if(ReferenceEquals(Property, null))
				return -1;
			return Property.GetHashCode();
		}
	}
}
namespace DevExpress.Data.Filtering {
	public interface IQueryCriteriaVisitor : ICriteriaVisitor {
		void Visit(QueryOperand theOperand);
		void Visit(QuerySubQueryContainer theOperand);
	}
	public interface IQueryCriteriaVisitor<T> : ICriteriaVisitor<T> {
		T Visit(QueryOperand theOperand);
		T Visit(QuerySubQueryContainer theOperand);
	}
	[XmlInclude(typeof(ParameterValue))]
	[XmlInclude(typeof(QueryOperand))]
	[XmlInclude(typeof(QuerySubQueryContainer))]
	public abstract partial class CriteriaOperator {
	}
}
namespace DevExpress.Data.Filtering.Helpers {
	public abstract partial class CriteriaToStringBase : IQueryCriteriaVisitor<CriteriaToStringVisitResult> {
		public virtual CriteriaToStringVisitResult Visit(QueryOperand operand) {
			string columnType = string.Empty;
			if (operand.ColumnType != DBColumnType.Unknown)
				columnType = ',' + operand.ColumnType.ToString();
			string result = operand.NodeAlias + ".{" + operand.ColumnName + columnType + '}';
			return new CriteriaToStringVisitResult(result);
		}
		public virtual CriteriaToStringVisitResult Visit(QuerySubQueryContainer operand) {
			string result = "SubQuery(";
			result += operand.AggregateType.ToString();
			result += ',';
			if (!ReferenceEquals(operand.AggregateProperty, null))
				result += Process(operand.AggregateProperty).Result;
			result += ',';
			if (operand.Node != null)
				result += operand.Node.ToString();
			result += ')';
			return new CriteriaToStringVisitResult(result);
		}
	}
	public partial class BooleanComplianceChecker : IQueryCriteriaVisitor<BooleanCriteriaState> {
		public BooleanCriteriaState Visit(DevExpress.Xpo.DB.QueryOperand theOperand) {
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(DevExpress.Xpo.DB.QuerySubQueryContainer theOperand) {
			if (theOperand.AggregateType == Aggregate.Exists) {
				return BooleanCriteriaState.Logical;
			}
			return BooleanCriteriaState.Value;
		}
	}
	public partial class IsLogicalCriteriaChecker : IQueryCriteriaVisitor<BooleanCriteriaState> {
		public BooleanCriteriaState Visit(DevExpress.Xpo.DB.QueryOperand theOperand) {
			return BooleanCriteriaState.Value;
		}
		public BooleanCriteriaState Visit(DevExpress.Xpo.DB.QuerySubQueryContainer theOperand) {
			if (theOperand.AggregateType == Aggregate.Exists) {
				return BooleanCriteriaState.Logical;
			}
			return BooleanCriteriaState.Value;
		}
	}
}
