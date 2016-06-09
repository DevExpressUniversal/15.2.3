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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data;
using System.Reflection;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Data;
namespace DevExpress.XtraReports.Native {
	public class XSDHelper {
		const string defaultNamespace = "http://www.w3.org/2001/XMLSchema";
		static string[][] predefinedNamespaces = new string[][] { new string[2] {"xs", "http://www.w3.org/2001/XMLSchema" }, 
																	new string[2] {"msdata", "urn:schemas-microsoft-com:xml-msdata" }, 
																	new string[2] {"msprop", "urn:schemas-microsoft-com:xml-msprop"}, 
																	new string[2] {"mstns", "http://tempuri.org/nwindDataSet.xsd"} 
																};
		public static void Generate(IServiceProvider provider, string fileName, System.Data.Common.DbConnection connection, string connectionString, string dataSourceName, string tableName, string tableSchemaName) {
			MemoryStream dataAdapterSchemaMS = new MemoryStream();
			StreamWriter dataAdapterSchemaSW = new StreamWriter(dataAdapterSchemaMS);
			try {
				string selectCommandText = ManagedProviderHelper.GetSelectQuery(provider, connection, tableName, tableSchemaName, null);
				DataSet dataSet = CreateDataSet(selectCommandText, connection, connectionString, tableName, dataSourceName);
				XmlDocument dataSetSchema = CreateDataSetSchemaDocument(dataSet);
				string[] dataSetColumnNames = CreateDataSetColumnNames(dataSet);
				XmlSerializer serializer = new XmlSerializer(typeof(Schema), defaultNamespace);
				serializer.Serialize(dataAdapterSchemaSW, new Schema(connectionString, selectCommandText, dataSourceName, tableName, dataSetColumnNames), GetNamespaces());
				XmlDocument mainXmlDoc = new XmlDocument();
				dataAdapterSchemaMS.Position = 0;
				mainXmlDoc.Load(dataAdapterSchemaMS);
				ImportDataSetSchema(dataSetSchema, mainXmlDoc);
				mainXmlDoc.Save(fileName);
			}
			finally {
				dataAdapterSchemaSW.Close();
				dataAdapterSchemaMS.Close();
			}
		}
		private static string[] CreateDataSetColumnNames(DataSet dataSet) {
			System.Collections.ArrayList columnNames = new System.Collections.ArrayList();
			foreach (System.Data.DataColumn dataColumn in dataSet.Tables[0].Columns) {
				columnNames.Add(dataColumn.ColumnName);
			}
			return (string[])columnNames.ToArray(typeof(string));
		}
		public static DataSet CreateDataSet(IServiceProvider provider, System.Data.Common.DbConnection connection, string connectionString, string tableName, string tableSchemaName, string dataSourceName) {
			string selectCommandText = ManagedProviderHelper.GetSelectQuery(provider, connection, tableName, tableSchemaName, null);
			return CreateDataSet(selectCommandText, connection, connectionString, tableName, dataSourceName);
		}
		static void ImportDataSetSchema(XmlDocument source, XmlDocument destination) {
			foreach (XmlElement element in source.LastChild.ChildNodes) {
				XmlNode xmlNodeToInsert = destination.ImportNode(element, true);
				destination.LastChild.AppendChild(xmlNodeToInsert);
			}
		}
		static XmlDocument CreateDataSetSchemaDocument(DataSet dataSet) {
			MemoryStream dataSetMS = new MemoryStream();
			dataSet.WriteXmlSchema(dataSetMS);
			XmlDocument xmlDocument = new XmlDocument();
			dataSetMS.Position = 0;
			try {
				xmlDocument.Load(dataSetMS);
				return xmlDocument;
			}
			finally {
				dataSetMS.Close();
			}
		}
		static DataSet CreateDataSet(string selectCommandText, System.Data.Common.DbConnection connection, string connectionString, string tableName, string dataSourceName) {
			DataSet dataSet = new DataSet(dataSourceName);
			try {
				System.Data.Common.DbDataAdapter dataAdapter = (System.Data.Common.DbDataAdapter)DevExpress.XtraReports.Design.WizPageTables.CreateDataAdapter(connectionString, selectCommandText);
				dataAdapter.FillSchema(dataSet, SchemaType.Mapped, tableName);
			}
			catch { 
			}
			return dataSet;
		}
		static XmlSerializerNamespaces GetNamespaces() {
			XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
			foreach (string[] ns in predefinedNamespaces) {
				namespaces.Add(ns[0], ns[1]);
			}
			return namespaces;
		}
	}
	public class ManagedProviderHelper {
		static string vsDesignerAssemblyName = "Microsoft.VSDesigner";
		static Assembly vsDesignerAssembly;
		static Type configurationContextType;
		static Type managedProviderType;
		static Type iDBSchemaServerType;
		static Type iDBSchemaTablesType;
		static MethodInfo getCmdTextMI;
		static readonly IDbSchemaHelper dbSchemaHelper = new DbSchemaHelperEx();
		static ManagedProviderHelper() {
			vsDesignerAssembly = DbObjectType.Type.Assembly;
			System.Diagnostics.Debug.Assert(vsDesignerAssembly != null);
			if (vsDesignerAssembly == null) return;
			try {
				configurationContextType = vsDesignerAssembly.GetType(vsDesignerAssemblyName + ".Data.ConfigurationContext");
				managedProviderType = vsDesignerAssembly.GetType(vsDesignerAssemblyName + ".Data.ManagedProvider");
				getCmdTextMI = managedProviderType.GetMethod("GetCmdText", new Type[] { typeof(string), typeof(string[]), DbObjectType.Type });
				iDBSchemaServerType = vsDesignerAssembly.GetType(vsDesignerAssemblyName + ".Data.Schema.IDBSchemaServer");
				iDBSchemaTablesType = vsDesignerAssembly.GetType(vsDesignerAssemblyName + ".Data.Schema.IDBSchemaTables");
			} catch { }
		}
		static bool IsAccessibleWithoutSchemaName(object configurationContext, string tableName) {
			try {
				PropertyInfo schemaServerPropertyInfo = configurationContextType.GetProperty("SchemaServer", iDBSchemaServerType);
				object schemaServer = schemaServerPropertyInfo.GetValue(configurationContext, null);
				PropertyInfo tablesPropertyInfo = iDBSchemaServerType.GetProperty("Tables", iDBSchemaTablesType);
				object tables = tablesPropertyInfo.GetValue(schemaServer, null);
				PropertyInfo itemPropertyInfo = iDBSchemaTablesType.GetProperty("Item", new Type[] { typeof(string) });
				object table = itemPropertyInfo.GetValue(tables, new object[] { tableName });
				return table != null;
			}
			catch { }
			return true;
		}
		public static string GetSelectQuery(IServiceProvider provider, System.Data.Common.DbConnection dbConnection, string tableName, string tableSchemaName, string[] columnNames) {
			System.Diagnostics.Debug.Assert(configurationContextType != null && managedProviderType != null);
			try {
				object configurationContext = Activator.CreateInstance(configurationContextType, dbConnection, provider);
				if (IsAccessibleWithoutSchemaName(configurationContext, tableName)) {
					object managedProvider = Activator.CreateInstance(managedProviderType, configurationContext);
					return (string)getCmdTextMI.Invoke(managedProvider, new object[] { tableName, columnNames, DbObjectType.Table });
				}
			}
			catch { }
			return dbSchemaHelper.GetSelectColumnsQuery(dbConnection, tableName, tableSchemaName, columnNames);
		}
	}
	public interface IInfoProvider {
		object GetValue(string key);
	}
	[
	XmlRoot("schema")
	]
	public class Schema : IInfoProvider {
		System.Collections.Hashtable hashTable = new System.Collections.Hashtable();
		[
		XmlAttribute,
		]
		public string id {
			get { return (string)((IInfoProvider)this).GetValue("DataSetName"); }
			set { }
		}
		[XmlElement("annotation")]
		public Annotation Annotation {
			get { return new Annotation(this); }
			set { }
		}
		object IInfoProvider.GetValue(string val) {
			return hashTable[val];
		}
		public Schema(string connectionString, string selectCommandText, string dataSetName, string tableName, string[] columnNames) {
			int index = connectionString.IndexOf(";Prompt=CompleteRequired");
			string patchedConnectionString = index > 0 ? connectionString.Remove(index, connectionString.Length - index) : connectionString;
			hashTable["SelectCommandText"] = selectCommandText;
			hashTable["DataSetName"] = dataSetName;
			hashTable["TableName"] = tableName;
			hashTable["ColumnNames"] = columnNames;
			hashTable["ConnectionString"] = GetConnectionString(patchedConnectionString);
			hashTable["ConnectionName"] = "Connection";
			hashTable["Provider"] = GetProvider(patchedConnectionString);
		}
		string GetProvider(string connectionString) {
			string provider = GetProviderName(connectionString);
			return provider.IndexOf("MSDASQL") >= 0 ? "System.Data.Odbc" : provider.IndexOf("SQLOLED") >= 0 ? "System.Data.SqlClient" : provider.IndexOf("MSDAORA") >= 0 ? "System.Data.OracleClient" : "System.Data.OleDb";
		}
		string GetTruncatedConnectionString(string connectionString) {
			return connectionString.Remove(0, connectionString.IndexOf(";") + 1);
		}
		string GetConnectionString(string patchedConnectionString) {
			string providerName = GetProviderName(patchedConnectionString);
			return (providerName.Equals("Microsoft.Jet.OLEDB.4.0") || providerName.StartsWith("SQLNCLI")) ?
				patchedConnectionString : GetTruncatedConnectionString(patchedConnectionString);
		}
		string GetProviderName(string connectionString) {
			int index = connectionString.IndexOf(";");
			if(index < 0)
				return string.Empty;
			string providerStr = connectionString.Substring(0, index);
			string[] pair = providerStr.Split('=');
			return pair != null && pair.Length == 2 ? pair[1] : string.Empty;
		}
		public Schema() { 
		}
	}
	public class Annotation {
		IInfoProvider infoProvider = null;
		[XmlElement("appinfo")]
		public AppInfo AppInfo {
			get { return new AppInfo(infoProvider); }
			set { }
		}
		public Annotation(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
		public Annotation() { 
		}
	}
	public class AppInfo {
		IInfoProvider infoProvider = null;
		[XmlElement(Namespace = "urn:schemas-microsoft-com:xml-msdatasource")]
		public DataSource DataSource {
			get { return new DataSource(infoProvider); }
			set { }
		}
		[XmlAttribute]
		public string source {
			get { return "urn:schemas-microsoft-com:xml-msdatasource"; }
			set { }
		}
		public AppInfo(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
		public AppInfo() { 
		}
	}
	public class DataSource {
		IInfoProvider infoProvider = null;
		[XmlArrayAttribute("Connections")]
		public Connection[] Connections {
			get { return new Connection[] { new Connection(infoProvider) }; }
			set { }
		}
		[XmlArrayAttribute("Tables")]
		public TableAdapter[] TableAdapters {
			get { return new TableAdapter[] { new TableAdapter(infoProvider) }; }
			set { }
		}
		[XmlAttribute]
		public int DefaultConnectionIndex {
			get { return 0; }
			set { }
		}
		[XmlAttribute]
		public string FunctionsComponentName {
			get { return "QueriesTableAdapter"; }
			set { }
		}
		[XmlAttribute]
		public string Modifier {
			get { return "AutoLayout, AnsiClass, Class, Public"; }
			set { }
		}
		[XmlAttribute]
		public string SchemaSerializationMode {
			get { return "IncludeSchema"; }
			set { }
		}
		public DataSource(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
		public DataSource() { }
	}
	public class TableAdapter {
		IInfoProvider infoProvider = null;
		[XmlAttribute]
		public string BaseClass { get { return "System.ComponentModel.Component"; } set { } }
		[XmlAttribute]
		public string DataAccessorModifier { get { return "AutoLayout, AnsiClass, Class, Public"; } set { } }
		[XmlAttribute]
		public string DataAccessorName { get { return TableAdapterName; } set { } }
		[XmlAttribute]
		public string GeneratorDataComponentClassName { get { return TableAdapterName; } }
		[XmlAttribute]
		public string Name { get { return (string)infoProvider.GetValue("TableName"); } set { } }
		[XmlAttribute]
		public string UserDataComponentName { get { return TableAdapterName; } set { } }
		string TableAdapterName {
			get { return infoProvider.GetValue("TableName") + "Adapter"; }
		}
		public MainSource MainSource {
			get { return new MainSource(infoProvider); }
			set { }
		}
		[XmlArrayAttribute("Mappings")]
		public Mapping[] Mappings {
			get { return mappings; }
			set { }
		}
		Mapping[] mappings = null;
		public TableAdapter(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
			string[] columnNames = (string[])infoProvider.GetValue("ColumnNames");
			System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
			foreach (string columnName in columnNames) { 
				arrayList.Add(new Mapping(columnName, columnName));
			}
			mappings = (Mapping[])arrayList.ToArray(typeof(Mapping));
		}
		public TableAdapter() { 
		}
	}
	public class Mapping {
		string sourceColumn = string.Empty;
		string dataSetColumn = string.Empty;
		[XmlAttribute]
		public string SourceColumn {
			get { return sourceColumn; }
			set { sourceColumn = value; }
		}
		[XmlAttribute]
		public string DataSetColumn {
			get { return dataSetColumn; }
			set { dataSetColumn = value; }
		}
		public Mapping(string sourceColumn, string dataSetColumn) {
			this.sourceColumn = sourceColumn;
			this.dataSetColumn = dataSetColumn;
		}
		public Mapping() {
		}
	}
	public class MainSource {
		IInfoProvider infoProvider = null;
		public DbSource DbSource {
			get { return new DbSource(infoProvider); }
			set { }
		}
		public MainSource(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
		public MainSource() { 
		}
	}
	public class DbSource {
		IInfoProvider infoProvider;
		[XmlAttribute]
		public string ConnectionRef {
			get { return (string)infoProvider.GetValue("ConnectionName"); }
			set { }
		}
		[XmlAttribute]
		public string DbObjectType {
			get { return "Table"; }
			set { }
		}
		[XmlAttribute]
		public string FillMethodModifier {
			get { return "Public"; }
			set { }
		}
		[XmlAttribute]
		public string FillMethodName {
			get { return "Fill"; }
			set { }
		}
		[XmlAttribute]
		public string GenerateMethods {
			get { return "Both"; }
			set { }
		}
		[XmlAttribute]
		public bool GenerateShortCommands {
			get { return true; }
			set { }
		}
		[XmlAttribute]
		public string GeneratorGetMethodName {
			get { return "GetData"; }
			set { }
		}
		[XmlAttribute]
		public string GeneratorSourceName {
			get { return "Fill"; }
			set { }
		}
		[XmlAttribute]
		public string GetMethodModifier {
			get { return "Public"; }
			set { }
		}
		[XmlAttribute]
		public string GetMethodName {
			get { return "GetData"; }
			set { }
		}
		[XmlAttribute]
		public string QueryType {
			get { return "Rowset"; }
			set { }
		}
		[XmlAttribute]
		public string ScalarCallRetval {
			get { return "System.Object, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"; }
			set { }
		}
		[XmlAttribute]
		public bool UseOptimisticConcurrency {
			get { return true; }
			set { }
		}
		[XmlAttribute]
		public string UserGetMethodName {
			get { return "GetData"; }
			set { }
		}
		[XmlAttribute]
		public string UserSourceName {
			get { return "Fill"; }
			set { }
		}
		public Command SelectCommand {
			get { return new Command(infoProvider); }
			set { }
		}
		public DbSource(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
		public DbSource() { 
		}
	}
	public class Command {
		IInfoProvider infoProvider;
		public DbCommand DbCommand {
			get { return new DbCommand(infoProvider); }
			set { }
		}
		public Command(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
		public Command() { 
		}
	}
	public class DbCommand {
		IInfoProvider infoProvider;
		[XmlAttribute]
		public string CommandType {
			get { return "Text"; }
			set { }
		}
		[XmlAttribute]
		public bool ModifiedByUser {
			get { return false; }
			set { }
		}
		public CommandText CommandText {
			get { return new CommandText(infoProvider); }
			set { }
		}
		public DbCommand(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
		public DbCommand() { 
		}
	}
	public class CommandText {
		IInfoProvider infoProvider;
		[XmlText]
		public string Text {
			get { return (string)infoProvider.GetValue("SelectCommandText"); }
			set { }
		}
		public CommandText(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
		public CommandText() { }
	}
	public class Connection {
		IInfoProvider infoProvider;
		[XmlAttribute]
		public string ConnectionStringObject { get { return (string)infoProvider.GetValue("ConnectionString"); } set { } }
		[XmlAttribute]
		public bool IsAppSettingsProperty { get { return false; } set { } }
		[XmlAttribute]
		public string Modifier { get { return "Assembly"; } set { } }
		[XmlAttribute]
		public string ParameterPrefix { get { return "@"; } set { } }
		[XmlAttribute]
		public string Provider { get { return (string)infoProvider.GetValue("Provider"); } set { } }
		[XmlAttribute]
		public string Name {
			get { return (string)infoProvider.GetValue("ConnectionName"); }
			set { }
		}
		public Connection() { 
		}
		public Connection(IInfoProvider infoProvider) {
			this.infoProvider = infoProvider;
		}
	}
}
