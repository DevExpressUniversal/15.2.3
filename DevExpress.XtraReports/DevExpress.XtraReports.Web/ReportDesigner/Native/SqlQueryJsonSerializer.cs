#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.Parameters;
using Sql = DevExpress.DataAccess.Sql;
namespace DevExpress.XtraReports.Web.ReportDesigner.Native {
	public class SqlQueryJsonSerializer {
		static readonly string[] collectionTokens = new string[] { "Query", "Table", "Column", "Relation", "KeyColumn", "Parameter", "View", "Field", "OrderBy", "GroupBy" };
		static readonly Type[] parameterTypesToPatch = new Type[] { typeof(char), typeof(byte), typeof(sbyte), typeof(double), typeof(float), typeof(decimal), typeof(DateTime), typeof(Guid) };
		readonly List<Sql.QueryParameter> deserializedQueryParameters = new List<Sql.QueryParameter>();
		readonly SqlDataSource serializableDataSource;
		#region from JSON to XML
		XElement JsonToXmlElement(string json) {
			return TransformRecursively((XElement)JsonGenerator.JsonToCleanXmlDeclaration(json).Root.FirstNode);
		}
		XElement TransformRecursively(XElement originalElement, XName newElementName = null) {
			var transformedElement = new XElement(newElementName ?? originalElement.Name);
			foreach(XElement element in originalElement.Elements()) {
				string attrName;
				if(JsonGenerator.TryProcessJsonAttribute(element, out attrName)) {
					transformedElement.SetAttributeValue(attrName, element.Value);
					continue;
				}
				if(TryProcessCollection(element, transformedElement))
					continue;
				if(TryProcessJsonSimpleProperty(element, transformedElement))
					continue;
				transformedElement.Add(TransformRecursively(element));
			}
			return transformedElement;
		}
		private static bool TryProcessJsonSimpleProperty(XElement element, XElement transformedElement) {
			string type = XmlHelperBase.GetAttributeValue(element, "type");
			if(type == "string" || type == "number" || type == "boolean") {
				transformedElement.Add(new XElement(element.Name.ToString()) { Value = element.Value });
				return true;
			}
			return false;
		}
		bool TryProcessCollection(XElement collection, XElement transformedElement) {
			if(Array.IndexOf(collectionTokens, collection.Name.ToString()) < 0)
				return false;
			bool isQueryParameter = collection.Name.ToString() == "Parameter" && transformedElement.Name.ToString() == "Query";
			foreach(XElement element in collection.Elements()) {
				XElement transformedChild = TransformRecursively(element, collection.Name);
				if(isQueryParameter) {
					ProcessQueryParameter(transformedChild);
				} else {
					transformedElement.Add(transformedChild);
				}
			}
			return true;
		}
		void ProcessQueryParameter(XElement parameterEl) {
			var paramInfo = new Sql.QueryParameter();
			Type paramType = Type.GetType(XmlHelperBase.GetAttributeValue(parameterEl, "Type"));
			if(paramType == null) {
				QuerySerializer.DeserializeParameter(paramInfo, parameterEl, null);
			} else {
				object result = ParameterHelper.ConvertFrom(parameterEl.Value, paramType, null);
				if(result == null && !string.IsNullOrEmpty(parameterEl.Value)) {
					QuerySerializer.DeserializeParameter(paramInfo, parameterEl, null);
				} else {
					paramInfo.Name = XmlHelperBase.GetAttributeValue(parameterEl, "Name");
					paramInfo.Type = paramType;
					paramInfo.Value = result;
				}
			}
			deserializedQueryParameters.Add(paramInfo);
		}
		#endregion
		#region from XML to JSON
		string XmlToJson(XElement element) {
			var builder = new StringBuilder();
			builder.Append('{');
			XmlToJsonNode(builder, element, true);
			return builder.Append('}').ToString();
		}
		void XmlToJsonNode(StringBuilder builder, XElement node, bool showNodeName) {
			if(showNodeName) {
				JsonGenerator.AppendQuotedValue(builder, node.Name.ToString(), isKey: true);
			}
			builder.Append('{');
			foreach(XAttribute attr in node.Attributes()) {
				JsonGenerator.AppendQuotedValue(builder, "@" + attr.Name.ToString(), isKey: true);
				JsonGenerator.AppendQuotedValue(builder, attr.Value);
				builder.Append(',');
			}
			var processedCollections = new List<string>();
			foreach(XNode cnode in node.Nodes()) {
				if(cnode.NodeType == XmlNodeType.Text) {
					builder.Append("\"Value\":");
					string value = IsQueryParameterNode(node)
						? PatchQueryParameterValue(node)
						: node.Value;
					JsonGenerator.AppendQuotedValue(builder, value);
				} else if(cnode.NodeType == XmlNodeType.Element) {
					XElement celement = cnode as XElement;
					int index = Array.IndexOf(collectionTokens, celement.Name.ToString());
					if(index > -1) {
						if(processedCollections.Contains(collectionTokens[index]))
							continue;
						processedCollections.Add(collectionTokens[index]);
						JsonGenerator.AppendQuotedValue(builder, collectionTokens[index], isKey: true);
						builder.Append('{');
						var j = 1;
						foreach(XElement item in node.Elements(collectionTokens[index])) {
							JsonGenerator.AppendQuotedValue(builder, "Item" + j++, isKey: true);
							XmlToJsonNode(builder, item, false);
							builder.Append(',');
						}
						builder.Remove(builder.Length - 1, 1);
						builder.Append('}');
					} else if(celement.HasAttributes || celement.HasElements) {
						XmlToJsonNode(builder, celement, true);
					} else {
						JsonGenerator.AppendQuotedValue(builder, celement.Name.ToString(), isKey: true);
						JsonGenerator.AppendQuotedValue(builder, celement.Value);
					}
				}
				builder.Append(',');
			}
			builder.Remove(builder.Length - 1, 1);
			builder.Append('}');
		}
		static bool IsQueryParameterNode(XElement node) {
			return node.Name.ToString() == "Parameter" && node.Parent != null && node.Parent.Name.ToString() == "Query";
		}
		string PatchQueryParameterValue(XElement parameterNode) {
			var queryName = XmlHelperBase.GetAttributeValue(parameterNode.Parent, "Name");
			var parameterName = XmlHelperBase.GetAttributeValue(parameterNode, "Name");
			if(queryName == null || parameterName == null)
				return parameterNode.Value;
			IParameter paramInfo = ParameterHelper.GetByName(serializableDataSource.Queries[queryName].Parameters, parameterName);
			if(paramInfo.Value == null)
				return parameterNode.Value;
			if(paramInfo.Type == typeof(Expression)) {
				return ((Expression)paramInfo.Value).ExpressionString;
			}
			if(Array.IndexOf(parameterTypesToPatch, paramInfo.Type) > -1) {
				return ParameterHelper.ConvertValueToString(paramInfo.Value);
			}
			return parameterNode.Value;
		}
		#endregion
		SqlQueryJsonSerializer(SqlDataSource dataSource = null) {
			serializableDataSource = dataSource;
		}
		public static string GenerateSqlDataSourceJson(SqlDataSource dataSource) {
			var serializer = new SqlQueryJsonSerializer(dataSource);
			return serializer.XmlToJson(SqlDataSourceSerializer.SaveToXml(dataSource, null));
		}
		public static SqlQuery CreateSqlQueryFromJson(string json) {
			var serializer = new SqlQueryJsonSerializer();
			SqlQuery sqlQuery = QuerySerializer.LoadFromXml(serializer.JsonToXmlElement(json), null);
			sqlQuery.Parameters.AddRange(serializer.deserializedQueryParameters);
			return sqlQuery;
		}
		public static MasterDetailInfo[] CreateRelationsFromJson(string json) {
			var serializer = new SqlQueryJsonSerializer();
			SqlDataSource sqlDataSource = SqlDataSourceSerializer.LoadFromXml(serializer.JsonToXmlElement(json), null);
			var result = new MasterDetailInfo[sqlDataSource.Relations.Count];
			for(var i = 0; i < sqlDataSource.Relations.Count; i++) {
				result[i] = sqlDataSource.Relations[i];
			}
			return result;
		}
	}
	public class JsonXMLConverter {
		string[] collectionTokens;
		public JsonXMLConverter(string[] collectionTokens) {
			this.collectionTokens = collectionTokens;
		}
		#region from JSON to XML
		public XElement JsonToXmlElement(string json) {
			return TransformRecursively((XElement)JsonGenerator.JsonToCleanXmlDeclaration(json).Root.FirstNode);
		}
		XElement TransformRecursively(XElement originalElement, XName newElementName = null) {
			var transformedElement = new XElement(newElementName ?? originalElement.Name);
			foreach(XElement element in originalElement.Elements()) {
				string attrName;
				if(JsonGenerator.TryProcessJsonAttribute(element, out attrName)) {
					transformedElement.SetAttributeValue(attrName, element.Value);
					continue;
				}
				if(TryProcessCollection(element, transformedElement))
					continue;
				if(TryProcessJsonSimpleProperty(element, transformedElement))
					continue;
				transformedElement.Add(TransformRecursively(element));
			}
			return transformedElement;
		}
		static bool TryProcessJsonSimpleProperty(XElement element, XElement transformedElement) {
			string type = XmlHelperBase.GetAttributeValue(element, "type");
			if(type == "string" || type == "number" || type == "boolean") {
				transformedElement.Add(new XElement(element.Name.ToString()) { Value = element.Value });
				return true;
			}
			return false;
		}
		bool TryProcessCollection(XElement collection, XElement transformedElement) {
			if(Array.IndexOf(collectionTokens, collection.Name.ToString()) < 0)
				return false;
			foreach(XElement element in collection.Elements()) {
				transformedElement.Add(TransformRecursively(element, collection.Name));
			}
			return true;
		}
		#endregion
		#region from XML to JSON
		public string XmlToJson(XElement element) {
			var builder = new StringBuilder();
			builder.Append('{');
			XmlToJsonNode(builder, element, true);
			return builder.Append('}').ToString();
		}
		void XmlToJsonNode(StringBuilder builder, XElement node, bool showNodeName) {
			if(showNodeName) {
				JsonGenerator.AppendQuotedValue(builder, node.Name.ToString(), isKey: true);
			}
			builder.Append('{');
			foreach(XAttribute attr in node.Attributes()) {
				JsonGenerator.AppendQuotedValue(builder, "@" + attr.Name.ToString(), isKey: true);
				JsonGenerator.AppendQuotedValue(builder, attr.Value);
				builder.Append(',');
			}
			var processedCollections = new List<string>();
			foreach(XNode cnode in node.Nodes()) {
				if(cnode.NodeType == XmlNodeType.Text) {
					builder.Append("\"Value\":");
					JsonGenerator.AppendQuotedValue(builder, node.Value);
				} else if(cnode.NodeType == XmlNodeType.Element) {
					XElement celement = cnode as XElement;
					int index = Array.IndexOf(collectionTokens, celement.Name.ToString());
					if(index > -1) {
						if(processedCollections.Contains(collectionTokens[index]))
							continue;
						processedCollections.Add(collectionTokens[index]);
						JsonGenerator.AppendQuotedValue(builder, collectionTokens[index], isKey: true);
						builder.Append('{');
						var j = 1;
						foreach(XElement item in node.Elements(collectionTokens[index])) {
							JsonGenerator.AppendQuotedValue(builder, "Item" + j++, isKey: true);
							XmlToJsonNode(builder, item, false);
							builder.Append(',');
						}
						builder.Remove(builder.Length - 1, 1);
						builder.Append('}');
					} else if(celement.HasAttributes || celement.HasElements) {
						XmlToJsonNode(builder, celement, true);
					} else {
						JsonGenerator.AppendQuotedValue(builder, celement.Name.ToString(), isKey: true);
						JsonGenerator.AppendQuotedValue(builder, celement.Value);
					}
				}
				builder.Append(',');
			}
			builder.Remove(builder.Length - 1, 1);
			builder.Append('}');
		}
		#endregion
	}
}
