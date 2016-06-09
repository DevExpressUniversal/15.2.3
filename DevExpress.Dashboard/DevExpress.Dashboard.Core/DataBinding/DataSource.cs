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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Helpers;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Data;
using DevExpress.PivotGrid.OLAP;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.Filtering;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon {
	[
	DXToolboxItem(false),
	]
	[Obsolete ("The DataSource class is obsolete now. Use the DashboardSqlDataSource, DashboardOlapDataSource, DashboardEFDataSource and DashboardObjectDataSource classes instead.") ]
	public class DataSource : DataSourceBase, IDashboardDataSourceInternal, IDashboardComponent, IDashboardDataSource {
		const string xmlFilter = "Filter";
		const string xmlDataSchema = "DataSchema";
		const string xmlDataProcessingMode = "DataProcessingMode";
		internal const string FilterDefaultValue = null;
		const DataProcessingMode defaultDataProcessingMode = DataProcessingMode.Server;
		readonly CalculatedFieldsController calculatedFieldsController;
		readonly DataSourceProperties properties;
		readonly PickManager pickManager;
		readonly BindingListController bindingListController;
		IDataProvider dataProvider;
		object data;
		object actualDataSchema;
		object dataSchemaCore;
		string dataSchemaString;
		object dataPreview;
		string dataMember;
		IList listSource;
		ObjectDataSourceListWrapper listWrapper;
		IEnumerable<IParameter> parameters;
		string filter;
		Dashboard dashboard;
		DataProcessingMode dataProcessingMode = defaultDataProcessingMode;
		ISite site;
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public SqlDataProvider SqlDataProvider { get { return dataProvider as SqlDataProvider; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public OlapDataProvider OlapDataProvider { get { return dataProvider as OlapDataProvider; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override object Data {
			get { return data; }
			set {
				if(dataProvider != null)
					throw new InvalidOperationException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectDataAssign));
				if (value != data) {
					SetData(value, value, null); 
				}
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string DataMember {
			get { return dataMember; }
			set {
				if(DataMember != value)
					SetData(data, dataSchemaCore, value);
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DataConnectionParametersBase ConnectionParameters {
			get {
				if(dataProvider != null) {
					DataConnection conntection = dataProvider.Connection as DataConnection;
					if(conntection != null)
						return conntection.ConnectionParameters;
				}
				return null;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public CalculatedFieldCollection CalculatedFields { get { return calculatedFieldsController.CalculatedFields; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string Filter {
			get {
				DataProviderBase dataProviderBase = dataProvider as DataProviderBase;
				if(dataProviderBase != null) {
					if(!dataProviderBase.DataSelection.IsEmpty)
						return dataProviderBase.GetFilterString(dataProviderBase.DataSelection[0].Alias);
				}
				return filter;
			}
			set {
				DataProviderBase dataProviderBase = dataProvider as DataProviderBase;
				if(dataProviderBase != null) {
					if(!dataProviderBase.DataSelection.IsEmpty)
						dataProviderBase.SetFilterString(dataProviderBase.DataSelection[0].Alias, value, !Loading);
				} else
					filter = value;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string DataSchema {
			get { return dataSchemaString; }
			set {
				if (DataProvider != null || dataSchemaString == value)
					return;
				dataSchemaString = value;
				DataSet dataSet = XmlDataHelper.CreateDataSetBySchema(value);
				SetData(data, dataSet, dataMember);
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DataProcessingMode DataProcessingMode {
			get {
				if (IsServerModeSupported)
					return dataProcessingMode;
				if (properties.IsOlap)
					return DataProcessingMode.Server;
				return DataProcessingMode.Client;
			}
			set { dataProcessingMode = value; }
		}
		[
		Browsable(false)
		]
		public bool HasDataProvider { get { return dataProvider != null; } }
		[
		Browsable(false)
		]
		public bool IsServerModeSupported { get { return SqlDataProvider != null && SqlDataProvider.IsServerModeSupported; } }
		[
		Browsable(false)
		]
		public bool IsConnected { get { return dataProvider == null || dataProvider.IsConnected; } }
		internal DataSourceNodeBase RootNode { get { return pickManager.RootNode; } }
		internal object DataSchemaCore { get { return dataSchemaCore; } }
		internal object DataPreview {
			get {
				if (dataPreview == null) { 
						dataPreview = data;
				}
				return dataPreview;
			}
		}
		internal IPivotGridDataSource PivotDataSource {
			get {
				if (properties.IsOlap)
					return OlapDataProvider.PivotDataSource;
				if (IsSqlServerMode)
					return SqlDataProvider.PivotServerModeDataSource;
				return null;
			}
		}
		protected internal override IDataProvider DataProvider {
			get { return dataProvider; }
			set {
				if (this.dataProvider != null)
					throw new InvalidOperationException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectDataAssign));
				SetDataProviderInternal(value);
			}
		}
		protected override XmlRepository<IDataProvider> DataProvidersRepository { get { return XmlRepository.DataProviderRepository; } }
		bool IsSqlServerMode { get { return GetIsSqlServerMode(DataProcessingMode); } }
		string PickManagerDisplayName {
			get {
				string cubeName = OlapDataProvider != null ? OlapDataProvider.CubeCaption : null;
				return String.IsNullOrEmpty(cubeName) ? Name : cubeName;
			}
		}
		Dashboard IDashboardDataSourceInternal.Dashboard { get { return dashboard; } set { dashboard = value; } }
		DataSourceProperties IDashboardDataSourceInternal.Properties { get { return properties; } }
		IEnumerable<IParameter> IDashboardDataSource.Parameters { get { return parameters; } }
		IDataProvider IDashboardDataSource.DataProvider { get { return DataProvider; } set { DataProvider = value; } }
		string IDashboardComponent.Name { get { return Name; } set { Name = value; } }
		ISite IComponent.Site { get { return site; } set { site = value; } }
		protected override ISite Site { get { return site; } }
		event EventHandler<NameChangingEventArgs> IDashboardComponent.NameChanging {
			add { AddNameChangingHandler(value); }
			remove { RemoveNameChangingHandler(value); }
		}
		event EventHandler<NameChangedEventArgs> IDashboardDataSourceInternal.NameChanged {
			add { AddNameChangedHandler(value); }
			remove { RemoveNameChangedHandler(value); }
		}
		event EventHandler<DataChangedEventArgs> IDashboardDataSourceInternal.DataSourceDataChanged {
			add { AddDataChangedHandler(value); }
			remove { RemoveDataChangedHandler(value); }
		}
		event EventHandler<DataProcessingModeChangedEventArgs> IDashboardDataSourceInternal.DataProcessingModeChanged { add { } remove { } }
		public DataSource()
			: this((string)null) {
		}
		public DataSource(string name)
			: this(name, (object)null) {
		}
		public DataSource(object data)
			: this((string)null, data) {
		}
		public DataSource(object data, string dataMember)
			: this((string)null, data, dataMember) {
		}
		public DataSource(string name, object data)
			: this(name, data, data, null) {
		}
		public DataSource(string name, object data, string dataMember)
			: this(name, data, data, dataMember) {
		}
		public DataSource(SqlDataProvider dataProvider)
			: this((string)null, dataProvider) {
		}
		public DataSource(string name, SqlDataProvider dataProvider)
			: this(name, (IDataProvider)dataProvider) {
		}
		public DataSource(OlapDataProvider dataProvider)
			: this((string)null, dataProvider) {
		}
		public DataSource(string name, OlapDataProvider dataProvider)
			: this(name, (IDataProvider)dataProvider) {
		}
		internal DataSource(string name, IDataProvider dataProvider)
			: this(name, dataProvider != null ? dataProvider.Data : null, dataProvider != null ? dataProvider.DataSchema : null, null) {
			SetDataProviderInternal(dataProvider);
		}
		DataSource(string name, object data, object dataSchema, string dataMember)
			: base(null, name) {
			bindingListController = new BindingListController(this);
			bindingListController.CustomDataChanged += bindingListController_CustomDataChanged;
			calculatedFieldsController = new CalculatedFieldsController(this);
			calculatedFieldsController.ConstructTree += calculatedFieldsController_ConstructTree;
			calculatedFieldsController.CalculatedFieldsChanged += calculatedFieldsController_CalculatedFieldsChanged;
			pickManager = new PickManagerWithCalcFields(name, this, null, calculatedFieldsController);
			if(string.IsNullOrEmpty(name))
				SetDataSourceCaptionFromData(data);
			SetDataInternal(data, dataSchema, dataMember);
			bindingListController.SubscribeDataEvents(listSource);
			properties = new DataSourceProperties(this);
		}
		void bindingListController_CustomDataChanged(object sender, DataSourceChangedEventArgs e) {
			RaiseDataChanged(true);
		}
		void calculatedFieldsController_CalculatedFieldsChanged(object sender, EventArgs e) {
			if(listWrapper != null)
				listWrapper.RePopulateColumns(parameters);
		}
		void calculatedFieldsController_ConstructTree(object sender, EventArgs e) {
			ConstructTree();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void BeginInit() {
			BeginLoading();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void EndInit() {
			EndLoading();
			ChangeActualSchema();
		}
		void IDashboardDataSourceInternal.SetParameters(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
		} 		
		 CalculatedFieldDataColumnInfo IDashboardDataSourceInternal.CreateCalculatedFieldColumnInfo(CalculatedField field, IEnumerable<IParameter> parameters) {
			return new CalculatedFieldDataColumnInfo(field, RootNode, calculatedFieldsController.CalculatedFields, parameters);
		}
		Type IDashboardDataSourceInternal.ServerGetUnboundExpressionType(string expression, string queryName) {
			if(!IsServerModeSupported)
				return typeof(object);
			return SqlDataProvider.PivotServerModeDataSource.GetUnboundExpressionType(expression, true);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(dataProvider != null && !dataProvider.IsDisposed) {
					}
				bindingListController.UnsubscribeDataEvents(listSource);
				calculatedFieldsController.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void LoadFromXmlCore(XElement dataSourceElement) {
			base.LoadFromXmlCore(dataSourceElement);
			calculatedFieldsController.LoadFromXml(dataSourceElement);
			XElement filterElement = dataSourceElement.Element(xmlFilter);
			if(filterElement != null)
				Filter = filterElement.Value;
			XElement dataSchemaElement = dataSourceElement.Element(xmlDataSchema);
			if(dataSchemaElement != null)
				DataSchema = dataSchemaElement.Value;
			string dataProcessingModeString = XmlHelper.GetAttributeValue(dataSourceElement, xmlDataProcessingMode);
			if(dataProcessingModeString != null)
				DataProcessingMode = XmlHelper.EnumFromString<DataProcessingMode>(dataProcessingModeString);
		}
		protected override XElement SaveToXmlCore() {
			XElement element = base.SaveToXmlCore();
			calculatedFieldsController.SaveToXml(element);
			if(ShouldSerializeFilter())
				element.Add(new XElement(xmlFilter, Filter));
			if(!string.IsNullOrEmpty(DataSchema))
				element.Add(new XElement(xmlDataSchema, DataSchema));
			if(ShouldSerializeDataProcessingMode())
				element.Add(new XAttribute(xmlDataProcessingMode, DataProcessingMode));
			return element;
		}
		protected override void SetParametersCore(IEnumerable<IParameter> parameters) {
			base.SetParametersCore(parameters);
			this.parameters = parameters;
		}
		protected override object RequestDataCore() {
			if(IsSqlServerMode)
				return null;
			return RaiseDataLoading();
		}
		protected override void ReloadDataCore(object newData) {
			if(dataProvider != null) {
 			} else {
				if(newData != null)
					SetData(newData, newData, dataMember);
				else
					SetData(data, dataSchemaCore, dataMember);
			}
		}
		object RaiseDataLoading() {
			if(dashboard == null || IsSqlServerMode)
				return null;
			return dashboard.RaiseDataLoading(this);
		}
		void SetDataProviderInternal(IDataProvider dataProvider) {
			this.dataProvider = dataProvider;
			if(dataProvider != null) {
  			}
		}
		void SetDataSourceCaptionFromData(object data) {
			IEnumerable enumerable = data as IEnumerable;
			if(enumerable != null) {
				IEnumerator enumerator = enumerable.GetEnumerator();
				if(enumerator != null && enumerator.MoveNext()) {
					object current = enumerator.Current;
					if(current != null) {
						DXDisplayNameAttribute attribute = TypeDescriptor.GetAttributes(current)[typeof(DXDisplayNameAttribute)] as DXDisplayNameAttribute;
						if(attribute != null)
							Name = attribute.DisplayName;
					}
				}
			}
		}
		void SetDataInternal(object data, object dataSchema, string dataMember) {
			this.data = data;
			this.dataSchemaCore = this.actualDataSchema = dataSchema;
			this.dataMember = dataMember;
			this.dataPreview = null;
			calculatedFieldsController.ClearCache();
			listSource = MasterDetailHelper.GetDataSource(data, dataMember);
			listWrapper = listSource != null ? new ObjectDataSourceListWrapper(listSource, this, parameters) : null;
			ChangeActualSchema();
		}
		void ChangeActualSchema() {
			try {
				bool isDataMemberSet = !string.IsNullOrEmpty(dataMember);
				if(listSource != null) {
					BindingSource bindingSource = listSource as BindingSource;
					if(!Loading && bindingSource != null && (isDataMemberSet || !string.IsNullOrEmpty(bindingSource.DataMember))) {
						try {
							using(DataContext dataContext = new DataContext(true)) {
								DataBrowser dataBrowser = dataContext[listSource, isDataMemberSet ? dataMember : bindingSource.DataMember];
								Type elemType = GenericTypeHelper.GetGenericIListTypeArgument(dataBrowser.DataSourceType);
								object elemInstance = Activator.CreateInstance(elemType);
								this.actualDataSchema = new List<object>() { elemInstance };
							}
						} catch {
							this.actualDataSchema = listSource;
						}
					} else if(isDataMemberSet) {
						this.actualDataSchema = listSource;
					}
				} else {
					if(isDataMemberSet && dataSchemaCore != null && dataProvider == null)
						this.actualDataSchema = MasterDetailHelper.GetDataSource(dataSchemaCore, dataMember);
				}
			} finally {
				ClearTree();
				ConstructTree();
			}
		}
		void SetData(object data, object dataSchema, string dataMember) {
			bindingListController.UnsubscribeDataEvents(listSource);
			SetDataInternal(data, dataSchema, dataMember);
			bindingListController.SubscribeDataEvents(listSource);
			RaiseDataChanged(true);
		}				
		void ClearTree() {
			pickManager.Clear();
		}
		void ConstructTree() {
			if(actualDataSchema != null)			  
				pickManager.ConstructTree(PickManagerDisplayName);			
		}
		bool ShouldSerializeData() {
			return dataProvider == null;
		}
		bool ShouldSerializeFilter() {
			return !(dataProvider is DataProviderBase) && !string.IsNullOrEmpty(Filter);
		}
		void ResetFilter() {
			Filter = null;
		}
		bool ShouldSerializeDataProcessingMode() {
			return IsServerModeSupported && DataProcessingMode != defaultDataProcessingMode;
		}
		bool IDashboardDataSourceInternal.IsSqlServerMode(string queryName) {
			return IsSqlServerMode;
		}
		bool IDashboardDataSourceInternal.GetIsSqlServerMode(DataProcessingMode dataProcessingMode, string queryName) {
			return GetIsSqlServerMode(dataProcessingMode);
		}
 		[
		Browsable(false)
		]
		public  bool GetIsSqlServerMode(DataProcessingMode dataProcessingMode) {
			return IsServerModeSupported ? dataProcessingMode == DataProcessingMode.Server : false;
		}
		internal string GetUniqueNamePropertyName(string propertyName) {
			return properties.IsOlap ? Helper.GetUniqueNamePropertyName(propertyName) : propertyName;
		}
		internal string GetDataItemDefinitionDisplayText(string dataMember) {
			if(properties.IsOlap) {
				string fieldCaption = PivotDataSource.GetFieldCaption(dataMember);
				return string.IsNullOrEmpty(fieldCaption) ? dataMember : fieldCaption;
			} else
				return dataMember;
		}
		bool IDashboardDataSourceInternal.ContainsParametersDisplayMember(string valueMember, string displayMember, string queryName) {
			if(properties.IsOlap)
				return true;
			else {
				IDataSourceSchema dataSourceSchema = ((IDashboardDataSource)this).GetDataSourceSchema(queryName);
				if(IsSqlServerMode) {
					return !string.IsNullOrEmpty(displayMember) && valueMember != displayMember && dataSourceSchema.GetField(displayMember) != null;
				} else {
					return DataSourceHelper.ContainsDisplayMember(dataSourceSchema, displayMember, valueMember);
				}
			}
		}
		List<ParameterValueViewModel> IDashboardDataSourceInternal.GetParameterValues(string valueMember, string displayMember,string queryName, IActualParametersProvider provider) {			
			if(properties.IsOlap) {
				List<ParameterValueViewModel> parameterValues = new List<ParameterValueViewModel>();				
				foreach(OLAPMember member in ((IPivotOLAPDataSource)PivotDataSource).GetUniqueMembers(valueMember))
					if(member.Column.AllMember != member)
						parameterValues.Add(new ParameterValueViewModel() {
							Value = member.UniqueName,
							DisplayText = member.Caption
						});
				return parameterValues;
			} else {
				if(IsSqlServerMode)
					return CreateModelValuesFromDynamicLookupSettingsServerMode(valueMember, displayMember,  queryName, provider);
				else
					return CreateModelValuesFromDynamicLookupSettingsNoServerMode(valueMember, displayMember, queryName);
			}
		}
		List<ParameterValueViewModel> CreateModelValuesFromDynamicLookupSettingsNoServerMode(string valueMember, string displayMember,  string queryName) {
			List<ParameterValueViewModel> parameterValues = new List<ParameterValueViewModel>();
			object dataSource = Data;
			using(DataContext dc = new DataContext()) {
				ListBrowser lb = dc[dataSource] as ListBrowser;				
				DataBrowser valueBrowser = dc[dataSource, valueMember];
				if(!string.IsNullOrWhiteSpace(displayMember) && pickManager.GetField(displayMember) != null) {		
					DataBrowser displayBrowser = dc[dataSource, displayMember];
					for(int i = 0; i < lb.Count; i++) {
						lb.Position = i;
						parameterValues.Add(new ParameterValueViewModel() {
							Value = valueBrowser.Current,
							DisplayText = displayBrowser.Current != null ? displayBrowser.Current.ToString() : string.Empty
						});
					}
				} else {					
					for(int i = 0; i < lb.Count; i++) {
						lb.Position = i;
						parameterValues.Add(new ParameterValueViewModel() {
							Value = valueBrowser.Current,
							DisplayText = string.Empty
						});
					}
				}
			}
			return parameterValues;
		}
		List<ParameterValueViewModel> CreateModelValuesFromDynamicLookupSettingsServerMode(string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			List<ParameterValueViewModel> parameterValues = new List<ParameterValueViewModel>();		
			if(PivotDataSource != null) {			   
				if(!string.IsNullOrEmpty(valueMember)) {
					PivotGridData data = new PivotGridData();
					data.PivotDataSource = (IPivotGridDataSource)((ICloneable)PivotDataSource).Clone();
					data.RetrieveFields(PivotArea.FilterArea, false);					
					PivotGridFieldBase fieldValue = data.Fields[valueMember] ?? CreateCalculatedField(valueMember,  queryName, data, provider);
					PivotGridFieldBase fieldDisplayText = null;
					if(!string.IsNullOrEmpty(displayMember))
						fieldDisplayText = data.Fields[displayMember] ?? CreateCalculatedField(displayMember, queryName, data, provider);
					if(fieldValue != null) {
						if(fieldDisplayText == null || fieldDisplayText == fieldValue) {
							foreach(object val in data.GetSortedUniqueValues(fieldValue))
								parameterValues.Add(new ParameterValueViewModel() {
									Value = val,
									DisplayText = Equals(null, val) ? string.Empty : val.ToString()
								});
						} else {														
							data.BeginUpdate();
							fieldValue.Area = PivotArea.RowArea;
							fieldValue.Visible = true;
							fieldDisplayText.Area = PivotArea.RowArea;
							fieldDisplayText.Visible = true;
							data.EndUpdate();
							for(int i = 0; i < data.PivotDataSource.GetCellCount(false); i++)
								if(data.PivotDataSource.GetObjectLevel(false, i) == 1)
									parameterValues.Add(new ParameterValueViewModel() {
										Value = data.PivotDataSource.GetFieldValue(false, i, 0),
										DisplayText = (data.PivotDataSource.GetFieldValue(false, i, 1) ?? string.Empty).ToString()
									});
						}
					}
				}
			}
			return parameterValues;
		}
		PivotGridFieldBase CreateCalculatedField(string displayText, string queryName, PivotGridData data, IActualParametersProvider parameters) {
			if(string.IsNullOrEmpty(displayText))
				return null;
			PivotGridFieldBase field = new PivotGridFieldBase();
			if(CalculatedFieldsController.PrepareCalculatedField(this, queryName, displayText, field, parameters)) {
				data.Fields.Add(field);
				return field;
			} else
				return null;
		}
		string IDashboardDataSourceInternal.GetName_13_1() {
			return NameController.Name_13_1;
		}
		internal void SetConnection(IEnumerable<DataConnectionBase> connections) {
			((IDataSource)this).SetConnection(connections);
		}	   
		IDataSourceSchema IDashboardDataSource.GetDataSourceSchema(string dataMember) {
			return pickManager;
		}
		IDashboardDataSourceInternal IDashboardDataSource.GetDataSourceInternal() {
			return this;
		}
		ICalculatedFieldsController IDashboardDataSource.GetCalculatedFieldsController() {
			return calculatedFieldsController;
	   }
		IEnumerable<string> IDashboardDataSourceInternal.GetDataSets() {
			return new string[] { string.Empty };
		}
		object IDashboardDataSourceInternal.GetDataSchema(string queryName) {
			return actualDataSchema;
		}
		IList IDashboardDataSourceInternal.GetListSource(string dataMember) {
			return listWrapper;
		}
		IPivotGridDataSource IDashboardDataSourceInternal.GetPivotDataSource(string dataMember) {
			return PivotDataSource;
		}
		IStorage IDashboardDataSourceInternal.GetStorage(string dataMember) {
			return null;
		}
	}
}
