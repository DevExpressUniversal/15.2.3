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
using System.Xml.Linq;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Xpo.Helpers;
using DevExpress.XtraReports.Native;
namespace DevExpress.DataAccess.Native.Sql {
	public class SqlDataSourceSerializer : IDataSerializer {
		#region XML Tokens
		const string XML_SqlDataSource = "SqlDataSource";
		const string XML_Name = "Name";
		const string XML_Connection = "Connection";
		const string XML_Relation = "Relation";
		const string XML_RelationName = "Name";
		const string XML_MasterTable = "Master";
		const string XML_DetailTable = "Detail";
		const string XML_KeyColumn = "KeyColumn";
		const string XML_MasterKeyColumn = "Master";
		const string XML_DetailKeyColumn = "Detail";
		const string XML_ResultSchema = "ResultSchema";		
		#endregion
		public string XmlRootTag { get { return XML_SqlDataSource; } }
		#region Save to XML
		public static XElement SaveToXml(SqlDataSource data) { return SaveToXml(data, null); }
		public static XElement SaveToXml(object data, IExtensionsProvider extensionProvider) { return SaveToXml((SqlDataSource)data, extensionProvider); }
		public static XElement SaveToXml(SqlDataSource data, IExtensionsProvider extensionProvider) {
			Guard.ArgumentNotNull(data, "data");
			XElement rootEl = new XElement(XML_SqlDataSource);
			SaveName(data, rootEl);
			SaveConnection(data, rootEl);
			SaveQueries(data, extensionProvider, rootEl);
			SaveRelations(data, rootEl);
			SaveResultSchema(data, rootEl);
			return rootEl;
		}
		static void SaveName(SqlDataSource sqlDataSource, XElement rootEl) {
			if (!string.IsNullOrEmpty(sqlDataSource.Name)) {
				XElement name = new XElement(XML_Name) {Value = sqlDataSource.Name};
				rootEl.Add(name);
			}
		}
		static void SaveConnection(SqlDataSource sqlDataSource, XElement rootEl) {
			if (sqlDataSource.ConnectionParameters == null && string.IsNullOrEmpty(sqlDataSource.ConnectionName))
				return;
			if (sqlDataSource.ConnectionParameters == null) {
				var dataConnection = new SqlDataConnection() { Name = sqlDataSource.ConnectionName, StoreConnectionNameOnly = true };
				XElement dataConnectionEl = new XElement(XML_Connection);
				dataConnection.SaveToXml(dataConnectionEl);
				rootEl.Add(dataConnectionEl);
				return;
			}
			SqlDataConnection connection = sqlDataSource.Connection;
			if(connection == null)
				return;
			XElement connectEl = new XElement(XML_Connection);
			connection.SaveToXml(connectEl);
			if(!connection.StoreConnectionNameOnly && !connection.HasConnectionString && connection.ConnectionParameters != null) {
				XElement parametersEl = connectEl.Element(DataConnectionHelper.XmlParameters);
				if(parametersEl != null) {
					Dictionary<string, string> parameters =
						ConnectionParameter.GetParamsDict(connection.ConnectionParameters);
					string uid;
					if(parameters.TryGetValue(DataConnectionHelper.UserIdParam, out uid) && uid != null) {
						XElement uidEl = new XElement(DataConnectionHelper.XmlParameter);
						uidEl.Add(new XAttribute(DataConnectionHelper.XmlName, DataConnectionHelper.UserIdParam));
						uidEl.Add(new XAttribute(DataConnectionHelper.XmlValue, uid));
						parametersEl.Add(uidEl);
					}
					string pass;
					if(parameters.TryGetValue(DataConnectionHelper.PasswordParam, out pass) && pass != null) {
						XElement passEl = new XElement(DataConnectionHelper.XmlParameter);
						passEl.Add(new XAttribute(DataConnectionHelper.XmlName, DataConnectionHelper.PasswordParam));
						passEl.Add(new XAttribute(DataConnectionHelper.XmlValue, pass));
						parametersEl.Add(passEl);
					}
				}
			}
			rootEl.Add(connectEl);
		}
		static void SaveQueries(SqlDataSource sqlDataSource, IExtensionsProvider extensionProvider, XElement rootEl) {
			foreach(SqlQuery query in sqlDataSource.Queries)
				rootEl.Add(QuerySerializer.SaveToXml(query, extensionProvider));
		}
		static void SaveRelations(SqlDataSource sqlDataSource, XElement rootEl) {
			foreach(MasterDetailInfo relation in sqlDataSource.Relations) {
				XElement relEl = new XElement(XML_Relation);
				relEl.Add(new XAttribute(XML_MasterTable, relation.MasterQueryName ?? string.Empty));
				relEl.Add(new XAttribute(XML_DetailTable, relation.DetailQueryName ?? string.Empty));
				if(relation.HasCustomName)
					relEl.Add(new XAttribute(XML_RelationName, relation.Name));
				foreach(RelationColumnInfo keyColumn in relation.KeyColumns) {
					XElement keyColEl = new XElement(XML_KeyColumn);
					keyColEl.Add(new XAttribute(XML_MasterKeyColumn, keyColumn.ParentKeyColumn ?? string.Empty));
					keyColEl.Add(new XAttribute(XML_DetailKeyColumn, keyColumn.NestedKeyColumn ?? string.Empty));
					relEl.Add(keyColEl);
				}
				rootEl.Add(relEl);
			}
		}
		static void SaveResultSchema(SqlDataSource sqlDataSource, XElement rootEl) {
			XElement schema = sqlDataSource.GetResultSchemaXml();
			if(schema != null && !schema.IsEmpty) {
				XElement schemaEl = new XElement(XML_ResultSchema);
				schemaEl.Add(schema);
				rootEl.Add(schemaEl);
			}
		}
		#endregion
		#region Load from XML
		public static SqlDataSource LoadFromXml(XElement element) { return LoadFromXml(element, null); }
		public static SqlDataSource LoadFromXml(XElement element, IExtensionsProvider extensionProvider) {
			SqlDataSource sqlDataSource = new SqlDataSource();
			LoadFromXml(sqlDataSource, element, extensionProvider);
			return sqlDataSource;
		}
		public static void LoadFromXml(SqlDataSource target, XElement element) { LoadFromXml(target, element, null); }
		public static void LoadFromXml(SqlDataSource target, XElement element, IExtensionsProvider extensionProvider) {
			Guard.ArgumentNotNull(target, "target");
			Guard.ArgumentNotNull(element, "element");
			if(!string.Equals(element.Name.LocalName, XML_SqlDataSource, StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException(string.Format("Incorrect XML root element: <{0}>, <{1}> is expected.", element.Name.LocalName, XML_SqlDataSource));
			target.Queries.Clear();
			target.Relations.Clear();
			target.ResultSet.SetTablesCore(new ResultTable[] { });
			LoadName(element, target);
			LoadConnection(element, target);
			LoadQueries(element, extensionProvider, target);
			LoadRelations(element, target);
			LoadResultSchema(element, target);
		}
		static void LoadName(XElement element, SqlDataSource sqlDataSource) {
			var nameElement = element.Element(XML_Name);
			if(nameElement != null) {
				sqlDataSource.Name = nameElement.Value;
			}
		}
		static void LoadConnection(XElement element, SqlDataSource sqlDataSource) {
			XElement connectEl = element.Element(XML_Connection);
			if(connectEl != null) {
				SqlDataConnection connection = new SqlDataConnection();
				connection.LoadFromXml(connectEl);
				sqlDataSource.SetConnectionInfo(connection);
			}
		}
		static void LoadQueries(XElement element, IExtensionsProvider extensionProvider, SqlDataSource sqlDataSource) {
			foreach(XElement queryEl in element.Elements(QuerySerializer.XmlRootTag))
				sqlDataSource.Queries.Add(QuerySerializer.LoadFromXml(queryEl, extensionProvider));
		}
		static void LoadRelations(XElement element, SqlDataSource sqlDataSource) {
			foreach(XElement relEl in element.Elements(XML_Relation)) {
				MasterDetailInfo relation = new MasterDetailInfo() {
					MasterQueryName = XmlHelperBase.GetAttributeValue(relEl, XML_MasterTable),
					DetailQueryName = XmlHelperBase.GetAttributeValue(relEl, XML_DetailTable),
					Name = XmlHelperBase.GetAttributeValue(relEl, XML_RelationName)
				};
				foreach(XElement keyColEl in relEl.Elements(XML_KeyColumn)) {
					RelationColumnInfo keyColumn = new RelationColumnInfo();
					keyColumn.ParentKeyColumn = XmlHelperBase.GetAttributeValue(keyColEl, XML_MasterKeyColumn);
					keyColumn.NestedKeyColumn = XmlHelperBase.GetAttributeValue(keyColEl, XML_DetailKeyColumn);
					relation.KeyColumns.Add(keyColumn);
				}
				sqlDataSource.Relations.Add(relation);
			}
		}
		static void LoadResultSchema(XElement element, SqlDataSource sqlDataSource) {
			XElement schemaEl = element.Element(XML_ResultSchema);
			if(schemaEl != null && schemaEl.HasElements)
				sqlDataSource.SetResultSchemaXml(schemaEl.Elements().First());
		}
		#endregion
		#region IDataSerializer Members
		bool IDataSerializer.CanDeserialize(string value, string typeName, object extensionProvider) {
			return typeof(SqlDataSource).FullName == typeName;
		}
		bool IDataSerializer.CanSerialize(object data, object extensionProvider) {
			if(data == null)
				return false;
			return data.GetType() == typeof(SqlDataSource);
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
