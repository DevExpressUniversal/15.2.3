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

namespace DevExpress.Design.DataAccess {
	using DevExpress.Utils.Localization;
	using DevExpress.Utils.Localization.Internal;
	#region DataAccessLocalizerStringId
	public enum DataAccessLocalizerStringId {
		CreateDataSourceActionName,
		DataSourceSmartTagActionName,
		DataSourceSmartTagActionNameXAML,
		GenerateDataSourceTransactionName,
		GenerateCodeBehindTransactionName,
		CreateSqlDataSourceTransactionName,
		DataSourceConfiguratorTitle,
		DataSourceConfiguratorTitleXAML,
		DataSourceConfiguratorSupportMail,
		DataSourceConfiguratorSupportTextLine1,
		DataSourceConfiguratorSupportTextLine2,
		XmlDataSetDataAccessTechnologyName,
		XmlDataSetDataAccessTechnologyDescription,
		SQLDataSourceDataAccessTechnologyName,
		SQLDataSourceDataAccessTechnologyDescription,
		ExcelDataSourceDataAccessTechnologyName,
		ExcelDataSourceDataAccessTechnologyDescription,
		TypedDataSetDataAccessTechnologyName,
		TypedDataSetDataAccessTechnologyDescription,
		LinqToSqlDataAccessTechnologyName,
		LinqToSqlDataAccessTechnologyDescription,
		EntityFrameworkDataAccessTechnologyName,
		EntityFrameworkDataAccessTechnologyDescription,
		WcfDataAccessTechnologyName,
		WcfDataAccessTechnologyDescription,
		IEnumerableDataAccessTechnologyName,
		IEnumerableDataAccessTechnologyDescription,
		EnumDataAccessTechnologyName,
		EnumDataAccessTechnologyDescription,
		RiaDataAccessTechnologyName,
		RiaDataAccessTechnologyDescription,
		OLAPDataAccessTechnologyName,
		OLAPDataAccessTechnologyDescription,
		XPODataAccessTechnologyName,
		XPODataAccessTechnologyDescription,
		TechnologiesCaption,
		SourcesCaption,
		NewSourceCaption,
		NextToProceedText,
		NoSourcesAvailableText,
		NewDataSourceMessageFormat,
		NewDataSourceMessageFormatSQLDataSource,
		NewDataSourceMessageFormatExcelDataSource,
		NewDataSourceTitle,
		NewDataSourceTitleSQLDataSource,
		NewDataSourceTitleExcelDataSource,
		NewDataSourceButtonText,
		LoadXmlCaption,
		RetrieveSchemaCaption,
		TestConnectionCaption,
		XmlPathPrompt,
		ExcelPathUnspecified,
		ElementTypePrompt,
		ObjectTypePrompt,
		PrevCaption,
		NextCaption,
		FinishCaption,
		AddSortCaption,
		DeleteSortCaption,
		InvertSortCaption,
		AddGroupCaption,
		DeleteGroupCaption,
		SortingCaption,
		GroupingCaption,
		DirectBindingDataProcessingModeName,
		DirectBindingDataProcessingModeDescription,
		SimpleBindingDataProcessingModeName,
		SimpleBindingDataProcessingModeDescription,
		InMemoryCollectionViewDataProcessingModeName,
		InMemoryCollectionViewDataProcessingModeDescription,
		InMemoryBindingSourceDataProcessingModeName,
		InMemoryBindingSourceDataProcessingModeDescription,
		InstantFeedbackDataProcessingModeName,
		InstantFeedbackDataProcessingModeDescription,
		ServerModeDataProcessingModeName,
		ServerModeDataProcessingModeDescription,
		PLinqInstantFeedbackDataProcessingModeName,
		PLinqInstantFeedbackDataProcessingModeDescription,
		PLinqServerModeDataProcessingModeName,
		PLinqServerModeDataProcessingModeDescription,
		XMLtoDataSetDataProcessingModeName,
		XMLtoDataSetDataProcessingModeDescription,
		OLEDBforOLAPDataProcessingModeName,
		OLEDBforOLAPDataProcessingModeDescription,
		ADOMDforOLAPDataProcessingModeName,
		ADOMDforOLAPDataProcessingModeDescription,
		XMLAforOLAPDataProcessingModeName,
		XMLAforOLAPDataProcessingModeDescription,
		XPCollectionForXPODataProcessingModeName,
		XPCollectionForXPODataProcessingModeDescription,
		XPViewForXPODataProcessingModeName,
		XPViewForXPODataProcessingModeDescription,
		TablesDataSourcePropertyName,
		TablesDataSourcePropertyDescription,
		ElementTypeDataSourcePropertyName,
		ElementTypeDataSourcePropertyDescription,
		ObjectTypeDataSourcePropertyName,
		ObjectTypeDataSourcePropertyDescription,
		ServiceRootDataSourcePropertyName,
		ServiceRootDataSourcePropertyDescription,
		DefaultSortingDataSourcePropertyName,
		DefaultSortingDataSourcePropertyDescription,
		SortDescriptionsDataSourcePropertyName,
		SortDescriptionsDataSourcePropertyDescription,
		FilterDataSourcePropertyName,
		FilterDataSourcePropertyDescription,
		GroupAndSortDescriptionsDataSourcePropertyName,
		GroupAndSortDescriptionsDataSourcePropertyDescription,
		CultureDataSourcePropertyName,
		CultureDataSourcePropertyDescription,
		IsSynchronizedWithCurrentItemDataSourcePropertyName,
		IsSynchronizedWithCurrentItemDataSourcePropertyDescription,
		KeyExpressionDataSourcePropertyName,
		KeyExpressionDataSourcePropertyDescription,
		AreSourceRowsThreadSafeDataSourcePropertyName,
		AreSourceRowsThreadSafeDataSourcePropertyDescription,
		QueryDataSourcePropertyName,
		QueryDataSourcePropertyDescription,
		AutoLoadDataSourcePropertyName,
		AutoLoadDataSourcePropertyDescription,
		LoadDelayDataSourcePropertyName,
		LoadDelayDataSourcePropertyDescription,
		LoadIntervalDataSourcePropertyName,
		LoadIntervalDataSourcePropertyDescription,
		LoadSizeDataSourcePropertyName,
		LoadSizeDataSourcePropertyDescription,
		RefreshIntervalDataSourcePropertyName,
		RefreshIntervalDataSourcePropertyDescription,
		ProviderDataSourcePropertyName,
		ProviderDataSourcePropertyDescription,
		ServerDataSourcePropertyName,
		ServerDataSourcePropertyDescription,
		CatalogDataSourcePropertyName,
		CatalogDataSourcePropertyDescription,
		CubeDataSourcePropertyName,
		CubeDataSourcePropertyDescription,
		QueryTimeoutDataSourcePropertyName,
		QueryTimeoutDataSourcePropertyDescription,
		ConnectionTimeoutDataSourcePropertyName,
		ConnectionTimeoutDataSourcePropertyDescription,
		UserIdDataSourcePropertyName,
		UserIdDataSourcePropertyDescription,
		PasswordDataSourcePropertyName,
		PasswordDataSourcePropertyDescription,
		ConnectionStringDataSourcePropertyName,
		ConnectionStringDataSourcePropertyDescription,
		XmlPathDataSourcePropertyName,
		XmlPathDataSourcePropertyDescription,
		ExcelPathDataSourcePropertyName,
		ExcelPathDataSourcePropertyDescription,
		ShowDesignDataDataSourcePropertyName,
		ShowDesignDataDataSourcePropertyDescription,
		ShowCodeBehindDataSourcePropertyName,
		ShowCodeBehindDataSourcePropertyDescription,
		CollectionViewSourceRestrictions,
	}
	#endregion DataAccessLocalizerStringId
	public class DataAccessLocalizer : XtraLocalizer<DataAccessLocalizerStringId> {
		#region static
		static DataAccessLocalizer() {
			SetActiveLocalizerProvider(
					new DefaultActiveLocalizerProvider<DataAccessLocalizerStringId>(CreateDefaultLocalizer())
				);
		}
		public static XtraLocalizer<DataAccessLocalizerStringId> CreateDefaultLocalizer() {
			return new DataAccessResXLocalizer();
		}
		public new static XtraLocalizer<DataAccessLocalizerStringId> Active {
			get { return XtraLocalizer<DataAccessLocalizerStringId>.Active; }
			set { XtraLocalizer<DataAccessLocalizerStringId>.Active = value; }
		}
		public static string GetString(DataAccessLocalizerStringId id) {
			return Active.GetLocalizedString(id);
		}
		#endregion static
		public override XtraLocalizer<DataAccessLocalizerStringId> CreateResXLocalizer() {
			return new DataAccessResXLocalizer();
		}
		protected override void PopulateStringTable() {
			#region AddString
			AddString(DataAccessLocalizerStringId.CreateDataSourceActionName, "Data Source Wizard");
			AddString(DataAccessLocalizerStringId.DataSourceSmartTagActionName, "Data Source Wizard");
			AddString(DataAccessLocalizerStringId.DataSourceSmartTagActionNameXAML, "Items Source Wizard");
			AddString(DataAccessLocalizerStringId.GenerateDataSourceTransactionName, "Data Source Generation");
			AddString(DataAccessLocalizerStringId.GenerateCodeBehindTransactionName, "Code-Behind Generation");
			AddString(DataAccessLocalizerStringId.CreateSqlDataSourceTransactionName, "Creating a new SqlDataSource component");
			AddString(DataAccessLocalizerStringId.DataSourceConfiguratorTitle, "Data Source Configuration Wizard");
			AddString(DataAccessLocalizerStringId.DataSourceConfiguratorTitleXAML, "Items Source Configuration Wizard");
			AddString(DataAccessLocalizerStringId.DataSourceConfiguratorSupportMail, "support@devexpress.com");
			AddString(DataAccessLocalizerStringId.DataSourceConfiguratorSupportTextLine1, "If you have questions regarding this wizard or need any assistance");
			AddString(DataAccessLocalizerStringId.DataSourceConfiguratorSupportTextLine2, "with using our products, feel free to contact us at");
			AddString(DataAccessLocalizerStringId.XmlDataSetDataAccessTechnologyName, "XML Data");
			AddString(DataAccessLocalizerStringId.XmlDataSetDataAccessTechnologyDescription, "To bind a control to XML data, you need to create a <b>DataSet</b> object. This object provides methods that let you load XML files, streams containing XML data, etc.</br>Please refer to the [Loading a DataSet from XML](http://msdn.microsoft.com/en-us/library/fx29c3yd.aspx) article to learn more.");
			AddString(DataAccessLocalizerStringId.SQLDataSourceDataAccessTechnologyName, "SQL Data Connection");
			AddString(DataAccessLocalizerStringId.SQLDataSourceDataAccessTechnologyDescription, "To create a new SQL data connection, click New Data Source.</br>The <b>SqlDataSource</b> component allows you to add multiple queries and specify master-detail relations between them, or combine data records from different tables by using the [INNER JOIN](http://msdn.microsoft.com/en-us/library/bb208854.aspx) operation.");
			AddString(DataAccessLocalizerStringId.ExcelDataSourceDataAccessTechnologyName, "Excel Data Source");
			AddString(DataAccessLocalizerStringId.ExcelDataSourceDataAccessTechnologyDescription, "To create a new Excel data source, click New Data Source.</br>The <b>ExcelDataSource</b> component provides the capability to select data from Microsoft Excel workbooks (XLS, XLSX or XLSM files) or CSV files. You can select all data from the specified worksheet/CSV file or select a cell range referenced by the specified defined/table name.");
			AddString(DataAccessLocalizerStringId.TypedDataSetDataAccessTechnologyName, "ADO.NET Typed DataSet");
			AddString(DataAccessLocalizerStringId.TypedDataSetDataAccessTechnologyDescription, "The [ADO.NET Typed DataSet](http://msdn.microsoft.com/en-us/library/esbykkzb.aspx) is a memory-resident representation of data that provides a consistent relational programming model regardless of the source of the data it contains. A DataSet represents a complete set of data including the tables that contain, order, and constrain the data, as well as the relationships between the tables.");
			AddString(DataAccessLocalizerStringId.LinqToSqlDataAccessTechnologyName, "LINQ to SQL");
			AddString(DataAccessLocalizerStringId.LinqToSqlDataAccessTechnologyDescription, "[Language-Integrated Query (LINQ) to SQL](http://msdn.microsoft.com/en-us/library/bb425822.aspx) provider is an object-relational mapper that enables managing relational data as objects without losing the ability to query. It does this by translating language-integrated queries into SQL for execution by the database, and then translating the tabular results back into objects you define.");
			AddString(DataAccessLocalizerStringId.EntityFrameworkDataAccessTechnologyName, "Entity Framework");
			AddString(DataAccessLocalizerStringId.EntityFrameworkDataAccessTechnologyDescription, "[Entity Framework (EF)](http://msdn.microsoft.com/en-US/data/ef) is an object-relational mapper that enables .NET developers to work with relational data using domain-specific objects. It eliminates the need for most of the data-access code that developers usually need to write.");
			AddString(DataAccessLocalizerStringId.WcfDataAccessTechnologyName, "WCF Data Services");
			AddString(DataAccessLocalizerStringId.WcfDataAccessTechnologyDescription, "[Windows Communication Foundation (WCF)](http://msdn.microsoft.com/en-us/library/dd456779.aspx) is Microsoft’s unified programming model for building service-oriented applications. WCF Data Services allows the use of the Open Data (OData) protocol to query data over the HTTP protocol.");
			AddString(DataAccessLocalizerStringId.IEnumerableDataAccessTechnologyName, "IList/IEnumerable");
			AddString(DataAccessLocalizerStringId.IEnumerableDataAccessTechnologyDescription, "Any data source implementing the [IList](http://msdn.microsoft.com/en-us/library/system.collections.ilist.aspx) or [IEnumerable](http://msdn.microsoft.com/en-us/library/system.collections.ienumerable.aspx) interface (e.g. List<T>, ObservableCollection<T>, etc.).");
			AddString(DataAccessLocalizerStringId.EnumDataAccessTechnologyName, "Enumeration");
			AddString(DataAccessLocalizerStringId.EnumDataAccessTechnologyDescription, "A data source that is an [Enumeration](http://msdn.microsoft.com/en-us/library/vstudio/cc138362.aspx).");
			AddString(DataAccessLocalizerStringId.RiaDataAccessTechnologyName, "RIA Services");
			AddString(DataAccessLocalizerStringId.RiaDataAccessTechnologyDescription, "[Microsoft WCF RIA Services](http://msdn.microsoft.com/en-us/library/ee707344(v=vs.91).aspx) simplifies the development of Rich Internet Applications (RIA), such as Silverlight applications, by offering framework components, tools and services that provide a comfortable mechanism for data handling. The application logic is stored on the server and RIA Services client applications have access to this logic.");
			AddString(DataAccessLocalizerStringId.OLAPDataAccessTechnologyName, "OLAP Cube");
			AddString(DataAccessLocalizerStringId.OLAPDataAccessTechnologyDescription, "[Online Analytical Processing (OLAP)](https://technet.microsoft.com/en-us/library/bb522607) is a technology that is used to organize large business databases and support business intelligence. You can establish a connection to a cube on an OLAP server using the OLE DB for OLAP, ADOMD.NET or XMLA data providers.");
			AddString(DataAccessLocalizerStringId.XPODataAccessTechnologyName, "DevExpress ORM Tool (XPO)");
			AddString(DataAccessLocalizerStringId.XPODataAccessTechnologyDescription, "[XPO (eXpress Persistent Objects)](https://www.devexpress.com/Products/NET/ORM/) bridges the gap between relational databases and object oriented software constructs found in custom software. Using XPO, you can build applications that are compatible with [multiple database systems](https://documentation.devexpress.com/#CoreLibraries/CustomDocument2114) without having to make ANY changes in your code.");
			AddString(DataAccessLocalizerStringId.TechnologiesCaption, "Technologies");
			AddString(DataAccessLocalizerStringId.SourcesCaption, "Data Sources");
			AddString(DataAccessLocalizerStringId.NewSourceCaption, "New Data Source...");
			AddString(DataAccessLocalizerStringId.NextToProceedText, "Click Next to proceed to configure the data source.");
			AddString(DataAccessLocalizerStringId.NoSourcesAvailableText, "No available data sources are found in the solution. Please create a new data source.");
			AddString(DataAccessLocalizerStringId.NewDataSourceMessageFormat, "After a new data source is created, rebuild the current solution and run the {0} to continue configuring the data source.");
			AddString(DataAccessLocalizerStringId.NewDataSourceMessageFormatSQLDataSource, "After the control has been assigned the created data source, you can continue configuring the SqlDataSource by using the verbs available in its smart tag.</br>For example, to add more queries to the data source, click <b>Manage Queries</b>.");
			AddString(DataAccessLocalizerStringId.NewDataSourceMessageFormatExcelDataSource, "After the created data source has been assigned to the control, you can continue configuring the ExcelDataSource using the <b>Edit...</b> task available in its smart tag.");
			AddString(DataAccessLocalizerStringId.NewDataSourceTitle, "New Data Source");
			AddString(DataAccessLocalizerStringId.NewDataSourceTitleSQLDataSource, "New SQL Data Connection");
			AddString(DataAccessLocalizerStringId.NewDataSourceTitleExcelDataSource, "New Excel Data Connection");
			AddString(DataAccessLocalizerStringId.NewDataSourceButtonText, "Ok");
			AddString(DataAccessLocalizerStringId.LoadXmlCaption, "Load Xml...");
			AddString(DataAccessLocalizerStringId.RetrieveSchemaCaption, "Retrieve Schema");
			AddString(DataAccessLocalizerStringId.TestConnectionCaption, "Test Connection");
			AddString(DataAccessLocalizerStringId.ElementTypePrompt, "Start typing Element Type");
			AddString(DataAccessLocalizerStringId.ObjectTypePrompt, "Start typing Object Type");
			AddString(DataAccessLocalizerStringId.GroupingCaption, "Group Descriptions");
			AddString(DataAccessLocalizerStringId.SortingCaption, "Sort Descriptions");
			AddString(DataAccessLocalizerStringId.PrevCaption, "Back");
			AddString(DataAccessLocalizerStringId.NextCaption, "Next");
			AddString(DataAccessLocalizerStringId.FinishCaption, "Finish");
			AddString(DataAccessLocalizerStringId.AddSortCaption, "Add");
			AddString(DataAccessLocalizerStringId.DeleteSortCaption, "Delete");
			AddString(DataAccessLocalizerStringId.InvertSortCaption, "Invert");
			AddString(DataAccessLocalizerStringId.AddGroupCaption, "Add");
			AddString(DataAccessLocalizerStringId.DeleteGroupCaption, "Delete");
			AddString(DataAccessLocalizerStringId.DirectBindingDataProcessingModeName, "Direct Binding to Data Source");
			AddString(DataAccessLocalizerStringId.DirectBindingDataProcessingModeDescription, "Data-aware controls usually provide the <b>DataSource</b> and <b>DataMember</b> properties that let you bind them to a data source. In most cases, a control can be bound to a data source using the DataSource property alone (the DataMember property is left set to an empty string).");
			AddString(DataAccessLocalizerStringId.SimpleBindingDataProcessingModeName, "Simple Binding");
			AddString(DataAccessLocalizerStringId.SimpleBindingDataProcessingModeDescription, "A control will be bound to a plain collection of data objects.");
			AddString(DataAccessLocalizerStringId.InMemoryCollectionViewDataProcessingModeName, "Manipulating Data via ICollectionView");
			AddString(DataAccessLocalizerStringId.InMemoryCollectionViewDataProcessingModeDescription, "Enables collections to have the functionalities of current record management, custom sorting, filtering, and grouping.</br>For information on this component, please refer to the [corresponding topic](http://msdn.microsoft.com/en-us/library/system.componentmodel.icollectionview.aspx) in MSDN.");
			AddString(DataAccessLocalizerStringId.InMemoryBindingSourceDataProcessingModeName, "Binding via the BindingSource Component");
			AddString(DataAccessLocalizerStringId.InMemoryBindingSourceDataProcessingModeDescription, "The [System.Windows.Forms.BindingSource](http://msdn.microsoft.com/en-us/library/system.windows.forms.bindingsource.aspx) component simplifies design-time data binding to a database or any class declared in code. All DevExpress data-aware controls can be bound to data with the help of this component.</br>For information on this component, please refer to the corresponding topic in MSDN.");
			AddString(DataAccessLocalizerStringId.InstantFeedbackDataProcessingModeName, "Asynchronous Server-Side Data Processing");
			AddString(DataAccessLocalizerStringId.InstantFeedbackDataProcessingModeDescription, "Activates [Instant Feedback UI Mode](http://documentation.devexpress.com/#WPF/CustomDocument9565).</br>You'll no longer experience any UI lock-ups when operating with large volumes of data. All data intensive operations will be delegated to the server and performed asynchronously - in a background thread. This is a read only mode.");
			AddString(DataAccessLocalizerStringId.ServerModeDataProcessingModeName, "Server-Side Data Processing");
			AddString(DataAccessLocalizerStringId.ServerModeDataProcessingModeDescription, "Activates the [Server Mode](http://documentation.devexpress.com/#WPF/CustomDocument6279) designed to work with extremely large volumes of data.</br>Bound data is not loaded into memory in its entirety. A control synchronously loads data in small chunks and delegates data processing to the data server. This ensures quick access to data, even if the sorting, grouping, filtering and summary features are used.");
			AddString(DataAccessLocalizerStringId.PLinqInstantFeedbackDataProcessingModeName, "Asynchronous Parallel In-Memory DataProcessing");
			AddString(DataAccessLocalizerStringId.PLinqInstantFeedbackDataProcessingModeDescription, "[Async Parallel In-Memory Data Processing](http://documentation.devexpress.com/#WPF/CustomDocument10472).</br>All data-intensive operations (e.g. sorting, grouping, filtering, etc.) on in-memory data are delegated to a parallel implementation of the LINQ to Objects and performed asynchronously - in a background thread, by making full use of all the available processors/cores on the system.");
			AddString(DataAccessLocalizerStringId.PLinqServerModeDataProcessingModeName, "Parallel In-Memory Data Processing");
			AddString(DataAccessLocalizerStringId.PLinqServerModeDataProcessingModeDescription, "[Parallel In-Memory Data Processing](http://documentation.devexpress.com/#WPF/CustomDocument10472).</br>All data-intensive operations (e.g. sorting, grouping, filtering, etc.) on in-memory data are delegated to a parallel implementation of the LINQ to Objects by making full use of all the available processors/cores on the system.");
			AddString(DataAccessLocalizerStringId.XMLtoDataSetDataProcessingModeName, "Loading a DataSet from XML");
			AddString(DataAccessLocalizerStringId.XMLtoDataSetDataProcessingModeDescription, "To fill a DataSet with XML data from a file or stream, use the [ReadXml method](http://msdn.microsoft.com/en-us/library/360dye2a.aspx) of the DataSet object.</br>The method loads the contents of a XML file or stream and creates a relational schema of the DataSet depending on the XmlReadMode parameter and whether or not the relational schema already exists.");
			AddString(DataAccessLocalizerStringId.OLEDBforOLAPDataProcessingModeName, "OLE DB for OLAP");
			AddString(DataAccessLocalizerStringId.OLEDBforOLAPDataProcessingModeDescription, "[The Analysis Services OLE DB Provider](http://msdn.microsoft.com/en-us/library/ms146862.aspx) is the primary method for interacting with Analysis Services in order to accomplish such tasks as connecting to a cube or data mining model, querying a cube or data mining model, and retrieving schema information.");
			AddString(DataAccessLocalizerStringId.ADOMDforOLAPDataProcessingModeName, "ADOMD.NET");
			AddString(DataAccessLocalizerStringId.ADOMDforOLAPDataProcessingModeDescription, "[ADOMD.NET](http://msdn.microsoft.com/en-us/library/ms123483.aspx) is a Microsoft .NET Framework data provider that is designed to communicate with Microsoft SQL Server Analysis Services. ADOMD.NET uses the XML for Analysis protocol to communicate with analytical data sources by using either TCP/IP or HTTP connections to transmit and receive SOAP requests and responses. Analytical data, key performance indicators (KPIs), and mining models can be queried and manipulated by using the ADOMD.NET object model.");
			AddString(DataAccessLocalizerStringId.XMLAforOLAPDataProcessingModeName, "XMLA");
			AddString(DataAccessLocalizerStringId.XMLAforOLAPDataProcessingModeDescription, "[XML for Analysis (XMLA)](http://msdn.microsoft.com/en-us/library/ms186654.aspx) is a SOAP-based XML protocol, designed specifically for universal data access to any standard multidimensional data source that can be accessed over an HTTP connection.</br>XMLA allows you to integrate a client application with Analysis Services, without any dependencies on the .NET Framework or COM interfaces.");
			AddString(DataAccessLocalizerStringId.XPCollectionForXPODataProcessingModeName, "Binding via the XPCollection Component");
			AddString(DataAccessLocalizerStringId.XPCollectionForXPODataProcessingModeDescription, "The [XPCollection](https://documentation.devexpress.com/#CoreLibraries/clsDevExpressXpoXPCollectiontopic) component represent a collection of persistent objects. Can serve as a data source for data-aware controls.");
			AddString(DataAccessLocalizerStringId.XPViewForXPODataProcessingModeName, "Binding via the XPView Component");
			AddString(DataAccessLocalizerStringId.XPViewForXPODataProcessingModeDescription, "The [XPView](https://documentation.devexpress.com/#CoreLibraries/clsDevExpressXpoXPViewtopic) component represent a view that stores data retrieved from persistent objects. Allows arbitrary combinations of calculated and aggregated values to be retrieved from a data store. Can serve as a data source for data-aware controls.");
			AddString(DataAccessLocalizerStringId.TablesDataSourcePropertyName, "Table");
			AddString(DataAccessLocalizerStringId.TablesDataSourcePropertyDescription, "Select a table from the data source.");
			AddString(DataAccessLocalizerStringId.ElementTypeDataSourcePropertyName, "Element Type");
			AddString(DataAccessLocalizerStringId.ElementTypeDataSourcePropertyDescription, "Specify the type of objects that will be retrieved from the data source.");
			AddString(DataAccessLocalizerStringId.ObjectTypeDataSourcePropertyName, "Object Type");
			AddString(DataAccessLocalizerStringId.ObjectTypeDataSourcePropertyDescription, "Specify the type of objects that will be retrieved from the data source.");
			AddString(DataAccessLocalizerStringId.ServiceRootDataSourcePropertyName, "Service Root");
			AddString(DataAccessLocalizerStringId.ServiceRootDataSourcePropertyDescription, "An absolute URI that identifies the root of a data service.");
			AddString(DataAccessLocalizerStringId.DefaultSortingDataSourcePropertyName, "Default Sorting");
			AddString(DataAccessLocalizerStringId.DefaultSortingDataSourcePropertyDescription, "Enables you to specify how data source contents are sorted by default, when the sort order is not specified by a control.");
			AddString(DataAccessLocalizerStringId.GroupAndSortDescriptionsDataSourcePropertyName, "Collection View Type");
			AddString(DataAccessLocalizerStringId.GroupAndSortDescriptionsDataSourcePropertyDescription, "Specify the group and sort settings for CollectionViews.");
			AddString(DataAccessLocalizerStringId.SortDescriptionsDataSourcePropertyName, "Sorting");
			AddString(DataAccessLocalizerStringId.SortDescriptionsDataSourcePropertyDescription, "Select the sort order for fields.");
			AddString(DataAccessLocalizerStringId.FilterDataSourcePropertyName, "Filter");
			AddString(DataAccessLocalizerStringId.FilterDataSourcePropertyDescription, "Specify the expression used to filter which rows are viewed.");
			AddString(DataAccessLocalizerStringId.CultureDataSourcePropertyName, "Culture");
			AddString(DataAccessLocalizerStringId.CultureDataSourcePropertyDescription, "The preferred culture for your application.");
			AddString(DataAccessLocalizerStringId.IsSynchronizedWithCurrentItemDataSourcePropertyName, "Synchronize with Current Item");
			AddString(DataAccessLocalizerStringId.IsSynchronizedWithCurrentItemDataSourcePropertyDescription, "Enable this option to keep the focused record synchronized with the current item in the data source.");
			AddString(DataAccessLocalizerStringId.KeyExpressionDataSourcePropertyName, "Key Expression");
			AddString(DataAccessLocalizerStringId.KeyExpressionDataSourcePropertyDescription, "Specifies the primary key column in the data source.");
			AddString(DataAccessLocalizerStringId.AreSourceRowsThreadSafeDataSourcePropertyName, "Source Rows are Thread Safe");
			AddString(DataAccessLocalizerStringId.AreSourceRowsThreadSafeDataSourcePropertyDescription, "Specifies whether elements retrieved by the DataSource's queryable source are thread-safe.");
			AddString(DataAccessLocalizerStringId.QueryDataSourcePropertyName, "Query");
			AddString(DataAccessLocalizerStringId.QueryDataSourcePropertyDescription, "Specify the name of Query that will be used to retrieve objects from the data source.");
			AddString(DataAccessLocalizerStringId.AutoLoadDataSourcePropertyName, "AutoLoad");
			AddString(DataAccessLocalizerStringId.AutoLoadDataSourcePropertyDescription, "AutoLoad Description");
			AddString(DataAccessLocalizerStringId.LoadDelayDataSourcePropertyName, "LoadDelay");
			AddString(DataAccessLocalizerStringId.LoadDelayDataSourcePropertyDescription, "LoadDelay Description");
			AddString(DataAccessLocalizerStringId.LoadIntervalDataSourcePropertyName, "LoadInterval");
			AddString(DataAccessLocalizerStringId.LoadIntervalDataSourcePropertyDescription, "LoadInterval Description");
			AddString(DataAccessLocalizerStringId.LoadSizeDataSourcePropertyName, "LoadSize");
			AddString(DataAccessLocalizerStringId.LoadSizeDataSourcePropertyDescription, "LoadSize Description");
			AddString(DataAccessLocalizerStringId.RefreshIntervalDataSourcePropertyName, "RefreshInterval");
			AddString(DataAccessLocalizerStringId.RefreshIntervalDataSourcePropertyDescription, "RefreshInterval Description");
			AddString(DataAccessLocalizerStringId.ProviderDataSourcePropertyName, "Provider");
			AddString(DataAccessLocalizerStringId.ProviderDataSourcePropertyDescription, "The data provider to be used. The “MSOLAP” string identifies the latest version of the OLE DB provider.");
			AddString(DataAccessLocalizerStringId.ServerDataSourcePropertyName, "Server Name");
			AddString(DataAccessLocalizerStringId.ServerDataSourcePropertyDescription, "The name of a server that runs an instance of Microsoft SQL Server Analysis Services (SSAS) or the path to the data pump.");
			AddString(DataAccessLocalizerStringId.CatalogDataSourcePropertyName, "Catalog Name");
			AddString(DataAccessLocalizerStringId.CatalogDataSourcePropertyDescription, "The data catalog that contains the required cube.");
			AddString(DataAccessLocalizerStringId.CubeDataSourcePropertyName, "Cube Name");
			AddString(DataAccessLocalizerStringId.CubeDataSourcePropertyDescription, "The name of a cube that provides OLAP data.");
			AddString(DataAccessLocalizerStringId.QueryTimeoutDataSourcePropertyName, "Query Timeout");
			AddString(DataAccessLocalizerStringId.QueryTimeoutDataSourcePropertyDescription, "(Optional)</br>The maximum amount of time, in seconds, allowed for a query to SSAS to complete.</br>If the parameter is set to 0, each query can last indefinitely.");
			AddString(DataAccessLocalizerStringId.ConnectionTimeoutDataSourcePropertyName, "Connection Timeout");
			AddString(DataAccessLocalizerStringId.ConnectionTimeoutDataSourcePropertyDescription, "(Optional)</br>The maximum amount of time, in seconds, allowed for a connection to be established.</br>If the parameter is set to 0, establishing a connection can last indefinitely.");
			AddString(DataAccessLocalizerStringId.UserIdDataSourcePropertyName, "User Id");
			AddString(DataAccessLocalizerStringId.UserIdDataSourcePropertyDescription, "(Optional)</br>The user ID or name that will be used to establish a connection.");
			AddString(DataAccessLocalizerStringId.PasswordDataSourcePropertyName, "Password");
			AddString(DataAccessLocalizerStringId.PasswordDataSourcePropertyDescription, "(Optional)</br>The password that will be used to establish a connection with the specified user name.");
			AddString(DataAccessLocalizerStringId.ConnectionStringDataSourcePropertyName, "Connection String");
			AddString(DataAccessLocalizerStringId.ConnectionStringDataSourcePropertyDescription, "The resulting connection string.");
			AddString(DataAccessLocalizerStringId.XmlPathDataSourcePropertyName, "Xml Data Path");
			AddString(DataAccessLocalizerStringId.XmlPathDataSourcePropertyDescription, "The full name of the file (including the path) that stores data.");
			AddString(DataAccessLocalizerStringId.ExcelPathDataSourcePropertyName, "Excel File");
			AddString(DataAccessLocalizerStringId.ExcelPathDataSourcePropertyDescription, "The full path to the file that contains data.");
			AddString(DataAccessLocalizerStringId.XmlPathPrompt, "Enter the name of the file (including the path) or use Load Xml button");
			AddString(DataAccessLocalizerStringId.ExcelPathUnspecified, "A path to a data file is not specified. You can specify it later using the ExcelDataSource component designer.");
			AddString(DataAccessLocalizerStringId.ShowDesignDataDataSourcePropertyName, "Sample Data at Design-Time");
			AddString(DataAccessLocalizerStringId.ShowDesignDataDataSourcePropertyDescription, "Show Sample Data at Design Time");
			AddString(DataAccessLocalizerStringId.ShowCodeBehindDataSourcePropertyName, "Show Code-Behind");
			AddString(DataAccessLocalizerStringId.ShowCodeBehindDataSourcePropertyDescription, "Show generated code-behind after Wizard is closed");
			AddString(DataAccessLocalizerStringId.CollectionViewSourceRestrictions, "<b>Important note:</b> The [CollectionView](http://msdn.microsoft.com/en-us/library/system.windows.data.collectionview.aspx) is a base class for all ICollectionView sources, and does not provide grouping and sorting features. You won't be able to sort and group data in data-aware controls, such as the GridControl, if you are using the CollectionView class.</br>" +
"If you need grouping and sorting in your application, consider using the [ListCollectionView](http://msdn.microsoft.com/en-us/library/system.windows.data.listcollectionview.aspx) or [BindingListCollectionView](http://msdn.microsoft.com/en-us/library/system.windows.data.bindinglistcollectionview.aspx) instead.");
			#endregion AddString
		}
	}
	public class DataAccessResXLocalizer : XtraResXLocalizer<DataAccessLocalizerStringId> {
		const string baseName = "DevExpress.Design.DataAccess.LocalizationRes";
		public DataAccessResXLocalizer()
			: base(new DataAccessLocalizer()) {
		}
		protected override System.Resources.ResourceManager CreateResourceManagerCore() {
			return new System.Resources.ResourceManager(baseName, typeof(DataAccessResXLocalizer).Assembly);
		}
	}
}
namespace DevExpress.Design.DataAccess.UI {
	public class DataAccessLocalizerStringIdExtension : System.Windows.Markup.MarkupExtension {
		public DataAccessLocalizerStringId ID { get; set; }
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return DataAccessLocalizer.GetString(ID);
		}
	}
}
