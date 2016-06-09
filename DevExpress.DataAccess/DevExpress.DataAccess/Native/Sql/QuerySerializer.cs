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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using DevExpress.Compatibility.System;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
using DevExpress.XtraReports.Native;
using DevExpress.Utils;
#if !DXPORTABLE
using System.Runtime.Serialization.Formatters.Binary;
#endif
namespace DevExpress.DataAccess.Native.Sql {
	public class QuerySerializer : IDataSerializer {
		enum QueryType { 
			TableQuery,
			CustomSqlQuery,
			StoredProcQuery
		}
		#region XML Tokens
		const string XML_Query = "Query";
		const string XML_QueryType = "Type";
		const string XML_QueryName = "Name";
		public const string XML_Parameter = "Parameter";
		const string XML_ParameterName = "Name";
		const string XML_ParameterType = "Type";
		const string XML_FilterString = "Filter";
		const string XML_Table = "Table";
		const string XML_TableName = "Name";
		const string XML_TableAlias = "Alias";
		const string XML_TableColumn = "Column";
		const string XML_ColumnName = "Name";
		const string XML_ColumnAggregate = "Aggregate";
		const string XML_ColumnAlias = "Alias";
		const string XML_Relation = "Relation";
		const string XML_JoinType = "Type";
		const string XML_JoinParentTable = "Parent";
		const string XML_JoinNestedTable = "Nested";
		const string XML_RelationKeyColumn = "KeyColumn";
		const string XML_ParentKeyColumn = "Parent";
		const string XML_NestedKeyColumn = "Nested";
		const string XML_CustomSql = "Sql";
		const string XML_StoredProcName = "ProcName";
		const string XML_StringNull = "IsNull";
		const string XML_ConditionOperator = "Operator";
		const string XML_SortingItem = "OrderBy";
		const string XML_SortingItemTable = "Table";
		const string XML_SortingItemColumn = "Column";
		const string XML_SortingItemDescending = "Descending";
		const string XML_GroupingItem = "GroupBy";
		const string XML_GroupingItemTable = XML_SortingItemTable;
		const string XML_GroupingItemColumn = XML_SortingItemColumn;
		const string XML_GroupFilterString = "GroupFilter";
		const string XML_Top = "Top";
		const string XML_Skip = "Skip";
		#endregion
		static readonly Dictionary<Type, QueryType> MAP_QueryTypes;
		static QuerySerializer() {
			MAP_QueryTypes = new Dictionary<Type,QueryType>(3) {
				{ typeof(TableQuery), QueryType.TableQuery },
				{ typeof(CustomSqlQuery), QueryType.CustomSqlQuery },
				{ typeof(StoredProcQuery), QueryType.StoredProcQuery }
			};
		}
		public static string XmlRootTag { get { return XML_Query; } }
		public static XElement SaveToXml(object data, IExtensionsProvider extensionProvider) {
			if(data == null)
				throw new ArgumentNullException("data");
			QueryType queryType;
			if(!MAP_QueryTypes.TryGetValue(data.GetType(), out queryType))
				throw new ArgumentException();
			SqlQuery query = (SqlQuery)data;
			XElement rootEl = new XElement(XML_Query);
			rootEl.Add(new XAttribute(XML_QueryType, queryType.ToString()));
			if(!string.IsNullOrEmpty(query.Name))
				rootEl.Add(new XAttribute(XML_QueryName, query.Name));
			CultureInfo savedCultureInfo = CultureInfo.CurrentCulture;
			CultureInfoExtensions.SetCurrentCulture(CultureInfo.InvariantCulture);
			try {
				foreach(QueryParameter parameter in query.Parameters)
					rootEl.Add(SerializeParameter(extensionProvider, parameter));
				switch(queryType) {
					case QueryType.TableQuery:
						TableQuery tableQuery = (TableQuery)query;
						SerializeTableQueryCore(rootEl, tableQuery);
						break;
					case QueryType.CustomSqlQuery:
						CustomSqlQuery customSqlQuery = (CustomSqlQuery)query;
						SerializeCustomSqlQuery(rootEl, customSqlQuery);
						break;
					case QueryType.StoredProcQuery:
						StoredProcQuery storedProcQuery = (StoredProcQuery)query;
						SerializeStoredProcQuery(rootEl, storedProcQuery);
						break;
				}
			}
			finally {
				CultureInfoExtensions.SetCurrentCulture(savedCultureInfo);
			}
			return rootEl;
		}
		public static SqlQuery LoadFromXml(XElement element, IExtensionsProvider extensionProvider) {
			if(!string.Equals(element.Name.LocalName, XML_Query, StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException(string.Format("Incorrect XML root element: <{0}>, <{1}> is expected.", element.Name.LocalName, XML_Query));
			QueryType queryType = XmlHelperBase.EnumFromString<QueryType>(element.GetAttributeValue(XML_QueryType));
			SqlQuery query = null;
			CultureInfo savedCultureInfo = CultureInfo.CurrentCulture;
			CultureInfoExtensions.SetCurrentCulture(CultureInfo.InvariantCulture);
			try {
				switch(queryType) {
					case QueryType.TableQuery:
						query = DeserializeTableQuery(element);
						break;
					case QueryType.CustomSqlQuery:
						query = DeserializeCustomSqlQuery(element);
						break;
					case QueryType.StoredProcQuery:
						query = DeserializeStoredProcQuery(element);
						break;
				}
				foreach(XElement paramEl in element.Elements(XML_Parameter)) {
					QueryParameter paramInfo = new QueryParameter();
					DeserializeParameter(paramInfo, paramEl, extensionProvider);
					query.Parameters.Add(paramInfo);
				}
			}
			finally {
				CultureInfoExtensions.SetCurrentCulture(savedCultureInfo);
			}
			query.Name = element.GetAttributeValue(XML_QueryName);
			return query;
		}
		static void SerializeTableQueryCore(XElement rootEl, TableQuery tableQuery) {
			if(!string.IsNullOrEmpty(tableQuery.FilterString)) {
				XElement filterEl = new XElement(XML_FilterString);
				filterEl.Add(tableQuery.FilterString);
				rootEl.Add(filterEl);
			}
			if(!string.IsNullOrEmpty(tableQuery.GroupFilterString)) {
				XElement havingEl = new XElement(XML_GroupFilterString);
				havingEl.Add(tableQuery.GroupFilterString);
				rootEl.Add(havingEl);
			}
			if(tableQuery.Top > 0) {
				XElement topEl = new XElement(XML_Top);
				topEl.Add(tableQuery.Top);
				rootEl.Add(topEl);
			}
			if(tableQuery.Skip > 0) {
				XElement skipEl = new XElement(XML_Skip);
				skipEl.Add(tableQuery.Skip);
				rootEl.Add(skipEl);
			}
			foreach(TableInfo tableInfo in tableQuery.Tables) {
				XElement tableEl = new XElement(XML_Table);
				if(!string.IsNullOrEmpty(tableInfo.Name))
					tableEl.Add(new XAttribute(XML_TableName, tableInfo.Name));
				if(!string.IsNullOrEmpty(tableInfo.Alias))
					tableEl.Add(new XAttribute(XML_TableAlias, tableInfo.Alias));
				foreach(ColumnInfo columnInfo in tableInfo.SelectedColumns) {
					XElement columnEl = new XElement(XML_TableColumn);
					if(!string.IsNullOrEmpty(columnInfo.Name))
						columnEl.Add(new XAttribute(XML_ColumnName, columnInfo.Name));
					if(columnInfo.Aggregation != AggregationType.None)
						columnEl.Add(new XAttribute(XML_ColumnAggregate, columnInfo.Aggregation.ToString()));
					if(!string.IsNullOrEmpty(columnInfo.Alias))
						columnEl.Add(new XAttribute(XML_ColumnAlias, columnInfo.Alias));
					tableEl.Add(columnEl);
				}
				rootEl.Add(tableEl);
			}
			foreach(RelationInfo relInfo in tableQuery.Relations) {
				XElement relEl = new XElement(XML_Relation);
				relEl.Add(new XAttribute(XML_JoinType, relInfo.JoinType));
				relEl.Add(new XAttribute(XML_JoinParentTable, relInfo.ParentTable ?? string.Empty));
				relEl.Add(new XAttribute(XML_JoinNestedTable, relInfo.NestedTable ?? string.Empty));
				foreach(RelationColumnInfo keyColumnInfo in relInfo.KeyColumns) {
					XElement columnEl = new XElement(XML_RelationKeyColumn);
					columnEl.Add(new XAttribute(XML_ParentKeyColumn, keyColumnInfo.ParentKeyColumn));
					columnEl.Add(new XAttribute(XML_NestedKeyColumn, keyColumnInfo.NestedKeyColumn));
					if(keyColumnInfo.ConditionOperator != RelationColumnInfo.ConditionType.Equal)
						columnEl.Add(new XAttribute(XML_ConditionOperator, Enum.GetName(typeof(RelationColumnInfo.ConditionType), keyColumnInfo.ConditionOperator)));
					relEl.Add(columnEl);
				}
				rootEl.Add(relEl);
			}
			foreach(SortingInfo sortInfo in tableQuery.Sorting) {
				XElement sortEl = new XElement(XML_SortingItem);
				sortEl.Add(new XAttribute(XML_SortingItemTable, sortInfo.Table ?? string.Empty));
				sortEl.Add(new XAttribute(XML_SortingItemColumn, sortInfo.Column ?? string.Empty));
				if(sortInfo.Direction == SortingInfo.SortingDirection.Descending)
					sortEl.Add(new XAttribute(XML_SortingItemDescending, true));
				rootEl.Add(sortEl);
			}
			foreach(GroupingInfo groupInfo in tableQuery.Grouping) {
				XElement groupEl = new XElement(XML_GroupingItem);
				groupEl.Add(new XAttribute(XML_GroupingItemTable, groupInfo.Table ?? string.Empty));
				groupEl.Add(new XAttribute(XML_GroupingItemColumn, groupInfo.Column ?? string.Empty));
				rootEl.Add(groupEl);
			}
		}
		static void SerializeCustomSqlQuery(XElement rootEl, CustomSqlQuery customSqlQuery) {
			if(string.IsNullOrEmpty(customSqlQuery.Sql))
				return;
			XElement customSqlEl = new XElement(XML_CustomSql);
			customSqlEl.Add(customSqlQuery.Sql);
			rootEl.Add(customSqlEl);
		}
		static void SerializeStoredProcQuery(XElement rootEl, StoredProcQuery storedProcQuery) {
			if(string.IsNullOrEmpty(storedProcQuery.StoredProcName))
				return;
			XElement storedProcEl = new XElement(XML_StoredProcName);
			storedProcEl.Add(storedProcQuery.StoredProcName);
			rootEl.Add(storedProcEl);
		}
		#region DeserializeTableQuery
		static TableQuery DeserializeTableQuery(XElement rootEl) {
			TableQuery tableQuery = new TableQuery();
			XElement filterEl = rootEl.Element(XML_FilterString);
			if(filterEl != null)
				tableQuery.FilterString = filterEl.Value;
			XElement havingEl = rootEl.Element(XML_GroupFilterString);
			if(havingEl != null)
				tableQuery.GroupFilterString = havingEl.Value;
			XElement topEl = rootEl.Element(XML_Top);
			if(topEl != null) {
				int top;
				if(int.TryParse(topEl.Value, out top) && top > 0)
					tableQuery.Top = top;
			}
			XElement skipEl = rootEl.Element(XML_Skip);
			if(skipEl != null) {
				int skip;
				if(int.TryParse(skipEl.Value, out skip) && skip > 0)
					tableQuery.Skip = skip;
			}
			foreach(XElement tableEl in rootEl.Elements(XML_Table))
				DeserializeTableQueryTable(tableEl, tableQuery);
			foreach(XElement relEl in rootEl.Elements(XML_Relation))
				DeserializeTableQueryRelation(relEl, tableQuery);
			foreach(XElement sortEl in rootEl.Elements(XML_SortingItem))
				DeserializeTableQuerySortingInfo(sortEl, tableQuery);
			foreach(XElement groupEl in rootEl.Elements(XML_GroupingItem))
				DeserializeTableQueryGroupInfo(groupEl, tableQuery);
			return tableQuery;
		}
		static void DeserializeTableQueryGroupInfo(XElement groupEl, TableQuery tableQuery) {
			string table = groupEl.GetAttributeValue(XML_GroupingItemTable);
			string column = groupEl.GetAttributeValue(XML_SortingItemColumn);
			GroupingInfo groupInfo = new GroupingInfo(table, column);
			tableQuery.Grouping.Add(groupInfo);
		}
		static void DeserializeTableQuerySortingInfo(XElement sortEl, TableQuery tableQuery) {
			string table = sortEl.GetAttributeValue(XML_SortingItemTable);
			string column = sortEl.GetAttributeValue(XML_SortingItemColumn);
			SortingInfo sortInfo = new SortingInfo(table, column);
			bool desc;
			if(bool.TryParse(sortEl.GetAttributeValue(XML_SortingItemDescending), out desc) && desc)
				sortInfo.Direction = SortingInfo.SortingDirection.Descending;
			tableQuery.Sorting.Add(sortInfo);
		}
		static void DeserializeTableQueryRelation(XElement relEl, TableQuery tableQuery) {
			RelationInfo relInfo = new RelationInfo();
			string joinType = relEl.GetAttributeValue(XML_JoinType);
			if(!string.IsNullOrEmpty(joinType))
				relInfo.JoinType = XmlHelperBase.EnumFromString<JoinType>(joinType);
			relInfo.ParentTable = relEl.GetAttributeValue(XML_JoinParentTable);
			relInfo.NestedTable = relEl.GetAttributeValue(XML_JoinNestedTable);
			foreach(XElement keyColEl in relEl.Elements(XML_RelationKeyColumn)) {
				RelationColumnInfo keyColInfo = new RelationColumnInfo();
				keyColInfo.ParentKeyColumn = keyColEl.GetAttributeValue(XML_ParentKeyColumn);
				keyColInfo.NestedKeyColumn = keyColEl.GetAttributeValue(XML_NestedKeyColumn);
				string conditionString = keyColEl.GetAttributeValue(XML_ConditionOperator);
				if(conditionString != null)
					keyColInfo.ConditionOperator =
						(RelationColumnInfo.ConditionType)
							Enum.Parse(typeof(RelationColumnInfo.ConditionType), conditionString);
				relInfo.KeyColumns.Add(keyColInfo);
			}
			tableQuery.Relations.Add(relInfo);
		}
		static void DeserializeTableQueryTable(XElement tableEl, TableQuery tableQuery) {
			TableInfo tableInfo = new TableInfo();
			string tableName = tableEl.GetAttributeValue(XML_TableName);
			if(!string.IsNullOrEmpty(tableName))
				tableInfo.Name = tableName;
			string tableAlias = tableEl.GetAttributeValue(XML_TableAlias);
			if(!string.IsNullOrEmpty(tableAlias))
				tableInfo.Alias = tableAlias;
			foreach(XElement columnEl in tableEl.Elements(XML_TableColumn)) {
				ColumnInfo columnInfo = new ColumnInfo();
				string columnName = columnEl.GetAttributeValue(XML_ColumnName);
				if(!string.IsNullOrEmpty(columnName))
					columnInfo.Name = columnName;
				string aggregateName = columnEl.GetAttributeValue(XML_ColumnAggregate);
				if(!string.IsNullOrEmpty(aggregateName))
					columnInfo.Aggregation = (AggregationType)Enum.Parse(typeof(AggregationType), aggregateName);
				string columnAlias = columnEl.GetAttributeValue(XML_ColumnAlias);
				if(!string.IsNullOrEmpty(columnAlias))
					columnInfo.Alias = columnAlias;
				tableInfo.SelectedColumns.Add(columnInfo);
			}
			tableQuery.Tables.Add(tableInfo);
		}
		#endregion
		static CustomSqlQuery DeserializeCustomSqlQuery(XElement element) {
			XElement sqlEl = element.Element(XML_CustomSql);
			if(sqlEl == null)
				return new CustomSqlQuery();
			return new CustomSqlQuery { Sql = sqlEl.Value };
		}
		static StoredProcQuery DeserializeStoredProcQuery(XElement element) {
			XElement storedProcEl = element.Element(XML_StoredProcName);
			if(storedProcEl == null)
				return new StoredProcQuery();
			return new StoredProcQuery{ StoredProcName = storedProcEl.Value };
		}
		public static XElement SerializeParameter(IExtensionsProvider extensionProvider, DataSourceParameterBase parameter) {
			XElement paramEl = new XElement(XML_Parameter);
			if(!string.IsNullOrEmpty(parameter.Name))
				paramEl.Add(new XAttribute(XML_ParameterName, parameter.Name));
			if(parameter.Type == null)
				return paramEl;
			paramEl.Add(new XAttribute(XML_ParameterType, parameter.Type.FullName));
			if(parameter.Value == null) {
				paramEl.Add((new XAttribute(XML_StringNull, true)));
				return paramEl;
			}
			string value;
			if(parameter.Type == typeof(string))
				value = (string)parameter.Value;
			else if(parameter.Type == typeof(bool))
				value = ((bool)parameter.Value).ToString();
			else if(parameter.Type == typeof(char))
				value = string.Format("{0:X4}", Convert.ToUInt16((char)parameter.Value));
			else if(parameter.Type == typeof(int))
				value = ((int)parameter.Value).ToString(CultureInfo.InvariantCulture);			
			else if(parameter.Type == typeof(uint))
				value = ((uint)parameter.Value).ToString(CultureInfo.InvariantCulture);
			else if(parameter.Type == typeof(short))
				value = ((short)parameter.Value).ToString(CultureInfo.InvariantCulture);
			else if(parameter.Type == typeof(ushort))
				value = ((ushort)parameter.Value).ToString(CultureInfo.InvariantCulture);
			else if(parameter.Type == typeof(long))
				value = ((long)parameter.Value).ToString(CultureInfo.InvariantCulture);
			else if(parameter.Type == typeof(ulong))
				value = ((ulong)parameter.Value).ToString(CultureInfo.InvariantCulture);
			else if(parameter.Type == typeof(byte))
				value = string.Format("{0:X2}", (byte)parameter.Value);
			else if(parameter.Type == typeof(sbyte))
				value = string.Format("{0:X2}", (sbyte)parameter.Value);
			else if(parameter.Type == typeof(double))
				value = Convert.ToBase64String(BitConverter.GetBytes((double)parameter.Value));
			else if(parameter.Type == typeof(float))
				value = Convert.ToBase64String(BitConverter.GetBytes((float)parameter.Value));
			else if(parameter.Type == typeof(decimal)) {
				byte[] buffer = new byte[16];
				int[] parts = decimal.GetBits((decimal)parameter.Value);
				for(int i = 0; i < 4; i++)
					BitConverter.GetBytes(parts[i]).CopyTo(buffer, 4 * i);
				value = Convert.ToBase64String(buffer);
			}
			else if(parameter.Type == typeof(DateTime))
				value = ((DateTime)parameter.Value).ToBinary().ToString(CultureInfo.InvariantCulture);
			else if(parameter.Type == typeof(Expression))
				value = Expression.ConvertToString(parameter.Value as Expression);
			else if(SerializationService.SerializeObject(parameter.Value, out value, extensionProvider)) {
			}
#if !DXPORTABLE
			else if(parameter.Type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0) {
				var formatter = new BinaryFormatter();
				using(MemoryStream stream = new MemoryStream()) {
					formatter.Serialize(stream, parameter.Value);
					value = Convert.ToBase64String(stream.GetBuffer());
				}
			}
#endif
			else
				throw new NotSupportedException(string.Format(
					"Cannot serialize a parameter of the specified type: {0}.", parameter.Type.FullName));
			paramEl.Add(value);
			return paramEl;
		}
		public static void DeserializeParameter(DataSourceParameterBase paramInfo, XElement paramEl, IExtensionsProvider extensionProvider) {
			paramInfo.Name = paramEl.GetAttributeValue(XML_ParameterName);			
			string paramTypeStr = paramEl.GetAttributeValue(XML_ParameterType);
			Type paramType = Type.GetType(paramTypeStr);
			if(paramType != null)
				paramInfo.Type = paramType;
			bool isNull = bool.TryParse(paramEl.GetAttributeValue(XML_StringNull), out isNull) && isNull;
			if(isNull) {
				paramInfo.Value = null;
				return;
			}
			string paramValueStr = paramEl.Value;
			object paramValue;
			if(paramType == typeof(string))
				paramValue = paramValueStr;
			else if(paramType == typeof(Expression))
				paramValue = Expression.ConvertFromString(paramValueStr);
			else if(paramType == typeof(bool))
				paramValue = bool.Parse(paramValueStr);
			else if(paramType == typeof(char))
				paramValue = Convert.ToChar(ushort.Parse(paramValueStr, NumberStyles.HexNumber));
			else if(paramType == typeof(int))
				paramValue = int.Parse(paramValueStr);
			else if(paramType == typeof(uint))
				paramValue = uint.Parse(paramValueStr);
			else if(paramType == typeof(short))
				paramValue = short.Parse(paramValueStr);
			else if(paramType == typeof(ushort))
				paramValue = ushort.Parse(paramValueStr);
			else if(paramType == typeof(long))
				paramValue = long.Parse(paramValueStr);
			else if(paramType == typeof(ulong))
				paramValue = ulong.Parse(paramValueStr);
			else if(paramType == typeof(byte))
				paramValue = byte.Parse(paramValueStr, NumberStyles.HexNumber);
			else if(paramType == typeof(sbyte))
				paramValue = sbyte.Parse(paramValueStr, NumberStyles.HexNumber);
			else if(paramType == typeof(double))
				paramValue = BitConverter.ToDouble(Convert.FromBase64String(paramValueStr.Trim()), 0);
			else if(paramType == typeof(float))
				paramValue = BitConverter.ToSingle(Convert.FromBase64String(paramValueStr.Trim()), 0);
			else if(paramType == typeof(decimal)) {
				byte[] buffer = Convert.FromBase64String(paramValueStr.Trim());
				int[] parts = new int[4];
				for(int i = 0; i < 4; i++)
					parts[i] = BitConverter.ToInt32(buffer, 4 * i);
				paramValue = new decimal(parts);
			}
			else if(paramType == typeof(DateTime))
				paramValue = DateTime.FromBinary(long.Parse(paramValueStr.Trim()));
			else if(SerializationService.DeserializeObject(paramValueStr, paramTypeStr, out paramValue, extensionProvider)) {
				if(paramValue != null)
					paramInfo.Type = paramValue.GetType();
			}
#if !DXPORTABLE
			else if(paramType.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0) {
				var formatter = new BinaryFormatter();
				byte[] buffer = Convert.FromBase64String(paramValueStr.Trim());
				using(MemoryStream stream = new MemoryStream(buffer))
					paramValue = formatter.Deserialize(stream);
			}
#endif
			else
				throw new NotSupportedException(
					string.Format("Cannot deserialize a parameter of the specified type: {0}.", paramTypeStr));
			paramInfo.Value = paramValue;			
		}
		#region IDataSerializer Members
		bool IDataSerializer.CanDeserialize(string value, string typeName, object extensionProvider) {
			if(typeof(SqlQuery).FullName == typeName || typeof(SqlQuery).FullName == typeName)
				return true;
			return MAP_QueryTypes.Keys.Any(t => t.FullName == typeName);
		}
		bool IDataSerializer.CanSerialize(object data, object extensionProvider) {
			if(data == null)
				return false;
			return MAP_QueryTypes.ContainsKey(data.GetType());
		}
		object IDataSerializer.Deserialize(string value, string typeName, object extensionProvider) { 
			return LoadFromXml(XElement.Parse(value, LoadOptions.None), extensionProvider as IExtensionsProvider); 
		}
		string IDataSerializer.Serialize(object data, object extensionProvider) { 
			return SaveToXml(data, extensionProvider as IExtensionsProvider).ToString(SaveOptions.DisableFormatting); 
		}
		#endregion
	}
}
