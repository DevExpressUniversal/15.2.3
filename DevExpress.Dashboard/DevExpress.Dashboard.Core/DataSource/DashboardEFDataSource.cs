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
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.PivotGrid.ServerMode.Queryable;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Server;
namespace DevExpress.DashboardCommon {
	[
	DXToolboxItem(false),
	SuppressMessage("Microsoft.Design", "CA1039")
	]
	public class DashboardEFDataSource : EFDataSource, IDashboardDataSource, IDashboardDataSourceInternal, IExternalSchemaConsumer {
		readonly Locker loadingLocker = new Locker();
		readonly CalculatedFieldsController calculatedFieldsController;
		readonly Dictionary<string, string[]> schemas = new Dictionary<string, string[]>();
		readonly Dictionary<string, Lazy<IList>> lists = new Dictionary<string, Lazy<IList>>();
		readonly Dictionary<string, Lazy<IStorage>> storages = new Dictionary<string, Lazy<IStorage>>();
		readonly Dictionary<string, Lazy<ServerModeDataSource>> serverModeSources = new Dictionary<string, Lazy<ServerModeDataSource>>();
		readonly Dictionary<string, PickManager> pickManagers = new Dictionary<string, PickManager>();
		readonly DataSourceProperties properties;
		readonly DataSourceComponentNameController componentNameController;
		IEnumerable<IParameter> parameters;
		event EventHandler<DataChangedEventArgs> DataSourceDataChanged;
		event EventHandler<NameChangingEventArgs> NameChanging;
		event EventHandler<NameChangedEventArgs> NameChanged;
		event EventHandler CaptionChanged;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardEFDataSourceCalculatedFields"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public CalculatedFieldCollection CalculatedFields { get { return calculatedFieldsController.CalculatedFields; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardEFDataSourceName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		Browsable(true)
		]
		public override string Name { get { return componentNameController.Name; } set { componentNameController.Name = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardEFDataSourceComponentName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		]
		public string ComponentName { get { return componentNameController.ComponentName; } set { componentNameController.ComponentName = value; } }
		[
		Browsable(false)
		]
		public bool IsConnected { get { return Connection != null ? Connection.IsConnected : false; } }		
		public DashboardEFDataSource() 
			: this(null, null) {
		}
		public DashboardEFDataSource(string name)
			: this(name, null) {
		}
		public DashboardEFDataSource(EFConnectionParameters connectionParameters)
			: this(null, connectionParameters) {
		}
		public DashboardEFDataSource(string name, EFConnectionParameters connectionParameters)
			: base(connectionParameters) {
			properties = new DataSourceProperties(this);
			calculatedFieldsController = new CalculatedFieldsController(this);
			componentNameController = new DataSourceComponentNameController(name, loadingLocker, () => Site);
			componentNameController.NameChanged += componentNameController_NameChanged;
			componentNameController.NameChanging += componentNameController_NameChanging;
			componentNameController.CaptionChanged += componentNameController_CaptionChanged;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override XElement SaveToXml() {
			XElement element = base.SaveToXml();
			componentNameController.SaveComponentNameToXml(element);
			calculatedFieldsController.SaveToXml(element);
			return element;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			componentNameController.LoadComponentNameFromXml(element);
			calculatedFieldsController.LoadFromXml(element);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void BeginInit() {
			loadingLocker.Lock();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void EndInit() {
			loadingLocker.Unlock();
		}
		protected override void OnAfterFill(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
			Refresh();
			if(DataSourceDataChanged != null) {
				DataSourceDataChanged(this, new DataChangedEventArgs(true));
			}
		}
		protected override EFContextWrapper CreateContextWrapperCore() {
			EFContextWrapper wrapper = base.CreateContextWrapperCore();
			wrapper.UseLocalStorage = false;
			return wrapper;
		}
		void Refresh() {
			PropertyDescriptorCollection pds = ((ITypedList)this).GetItemProperties(null);
			pickManagers.Clear();
			lists.Clear();
			serverModeSources.Clear();			
			foreach(PropertyDescriptor pd in pds) {
				Type propertyType = pd.PropertyType;
				if(propertyType.GetInterfaces().Any(t => t == typeof(IQueryable)))
					serverModeSources.Add(pd.Name, new Lazy<ServerModeDataSource>(() => new ServerModeDataSource(new QueryableQueryExecutor((IQueryable)pd.GetValue(this)))));
				else if(propertyType.GetInterfaces().Any(t => t == typeof(IEnumerable)))
					lists.Add(pd.Name, new Lazy<IList>(() => {
						object propertyValue = pd.GetValue(this);
						IList list = propertyValue as IList;
						return list == null ? new ListIEnumerable((IEnumerable)propertyValue) : list;
					}));					
				PickManagerWithCalcFields pickManager = new PickManagerWithCalcFields(pd.Name, this, pd.Name, calculatedFieldsController);
				pickManager.ConstructTree(pd.Name);
				pickManagers.Add(pd.Name, pickManager);
			}
			InitStorages(pds);
			RaiseDataChaged();
		}
		void ResetStorage(PropertyDescriptor pd) {
			Type propertyType = pd.PropertyType;
			if(propertyType.GetInterfaces().Any(t => t == typeof(IEnumerable)) &&
			   !propertyType.GetInterfaces().Any(t => t == typeof(IQueryable))) {
				Lazy<IStorage> storageLazy = new Lazy<IStorage>(() => {
					IStorage storage = null;
					string[] schema = null;
					schemas.TryGetValue(pd.Name, out schema);
					if(schema != null) {
						IEnumerable<PropertyDescriptor> properties = ((ITypedList)this).GetItemProperties(new PropertyDescriptor[] { pd }).Cast<PropertyDescriptor>();
						IEnumerable propertyValue = (IEnumerable)pd.GetValue(this);
						using(DataReaderExEnumerableWrapper dataReaderEx = new DataReaderExEnumerableWrapper(properties, schema, propertyValue)) {
							DataStorageProcessor processor = new DataStorageProcessor(dataReaderEx);
							storage = processor.ProcessTables();
						}
					}
					return storage;
				});
				if(storages.ContainsKey(pd.Name))
					storages[pd.Name] = storageLazy;
				else
					storages.Add(pd.Name, storageLazy);
			}
		}
		void InitStorages(PropertyDescriptorCollection pds) {
			storages.Clear();
			foreach(PropertyDescriptor pd in pds) {
				ResetStorage(pd);
			}
		}
		void componentNameController_NameChanged(object sender, NameChangedEventArgs e) {
			if (NameChanged != null)
				NameChanged(this, e);
		}
		void componentNameController_CaptionChanged(object sender, EventArgs e) {
			if(CaptionChanged != null)
				CaptionChanged(this, e);
		}
		void componentNameController_NameChanging(object sender, NameChangingEventArgs e) {
			if(NameChanging != null)
				NameChanging(this, e);
		}
		void RaiseDataChaged() {
			if(DataSourceDataChanged != null) {
				DataSourceDataChanged(this, new DataChangedEventArgs(true));
			}
		}
		public DataProcessingMode GetDataProcessingMode(string dataMember) {
			if (dataMember != null && serverModeSources.ContainsKey(dataMember))
				return DataProcessingMode.Server;
			return DataProcessingMode.Client;
		}
		List<ParameterValueViewModel> CreateModelValuesFromDynamicLookupSettingsServerMode(string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			List<ParameterValueViewModel> parameterValues = new List<ParameterValueViewModel>();			
			if(((IDashboardDataSource)this).GetPivotDataSource(queryName) != null) {
				if(!string.IsNullOrEmpty(valueMember)) {
					PivotGridData data = new PivotGridData();
					data.PivotDataSource = (IPivotGridDataSource)((ICloneable)((IDashboardDataSource)this).GetPivotDataSource(queryName)).Clone();
					data.RetrieveFields(XtraPivotGrid.PivotArea.FilterArea, false);
					PivotGridFieldBase fieldValue = data.Fields[valueMember] ?? CreateCalculatedField(valueMember, queryName, data, provider);
					PivotGridFieldBase fieldDisplayText = null;
					if(!string.IsNullOrEmpty(displayMember))
						fieldDisplayText = data.Fields[displayMember] ?? CreateCalculatedField(displayMember, queryName, data, provider);
					if(fieldValue != null) {
						if(fieldDisplayText == null || fieldDisplayText == fieldValue) {
							object[] sortedUniqueValues = new object[0];
							try {
								sortedUniqueValues = data.GetSortedUniqueValues(fieldValue);
							}
							catch { }
							foreach(object val in sortedUniqueValues)
								parameterValues.Add(new ParameterValueViewModel() {
									Value = val,
									DisplayText = object.Equals(null, val) ? string.Empty : val.ToString()
								});
						} else {							
							data.BeginUpdate();
							fieldValue.Area = XtraPivotGrid.PivotArea.RowArea;
							fieldValue.Visible = true;
							fieldDisplayText.Area = XtraPivotGrid.PivotArea.RowArea;
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
		PivotGridFieldBase CreateCalculatedField(string displayText, string queryName, PivotGridData data, IActualParametersProvider provider) {
			if(string.IsNullOrEmpty(displayText))
				return null;
			PivotGridFieldBase field = new XtraPivotGrid.PivotGridFieldBase();
			if(CalculatedFieldsController.PrepareCalculatedField(this, queryName, displayText, field, provider)) {
				data.Fields.Add(field);
				return field;
			} else
				return null;
		}
		IList GetListByQuery(string queryName) {
			if(queryName != null) {
				Lazy<IList> list = null;
				lists.TryGetValue(queryName, out list);
				if(list != null)
					return list.Value;
			}
			return null;
		}
		#region  IDashboardDataSource
		IDataProvider IDashboardDataSource.DataProvider { get { return null; } set { throw new NotSupportedException(); } }
#pragma warning disable 612, 618
		OlapDataProvider IDashboardDataSource.OlapDataProvider { get { return null; } }
		SqlDataProvider IDashboardDataSource.SqlDataProvider { get { return null; } }
#pragma warning restore 612, 618
		bool IDashboardDataSource.HasDataProvider { get { return false; } }
		bool IDashboardDataSource.IsServerModeSupported { get { return false; } }
		string IDashboardDataSource.Filter { get { return null; } set {  } }
		object IDashboardDataSource.Data { get { return this; } set { throw new NotSupportedException(); } }
		DataProcessingMode IDashboardDataSource.DataProcessingMode { get { return DataProcessingMode.Client; } set { throw new NotSupportedException(); } }
		IEnumerable<IParameter> IDashboardDataSource.Parameters { get { return parameters; } }
		IDashboardDataSourceInternal IDashboardDataSource.GetDataSourceInternal() { 
			return this; 
		}
		IDataSourceSchema IDashboardDataSource.GetDataSourceSchema(string dataMember) {
			if (dataMember == null)
				return null;
			PickManager dataSchemaProvider;
			if (pickManagers.TryGetValue(dataMember, out dataSchemaProvider))
				return dataSchemaProvider;
			return null;
		}
		ICalculatedFieldsController IDashboardDataSource.GetCalculatedFieldsController() {
			return calculatedFieldsController;
		}
		#endregion
		#region IDashboardDataSourceInternal
		Dashboard IDashboardDataSourceInternal.Dashboard { get; set; }
		DataSourceProperties IDashboardDataSourceInternal.Properties { get { return properties; } }
		event EventHandler<DataChangedEventArgs> IDashboardDataSourceInternal.DataSourceDataChanged {
			add { DataSourceDataChanged += value; }
			remove { DataSourceDataChanged -= value; }
		}
		event EventHandler<NameChangedEventArgs> IDashboardDataSourceInternal.NameChanged { 
			add { NameChanged += value; } 
			remove { NameChanged -= value; } 
		}
		event EventHandler IDashboardDataSourceInternal.CaptionChanged { 
			add { CaptionChanged += value; } 
			remove { CaptionChanged -= value; } 
		}
		event EventHandler<DataProcessingModeChangedEventArgs> IDashboardDataSourceInternal.DataProcessingModeChanged { add { } remove { } }
		Type IDashboardDataSourceInternal.ServerGetUnboundExpressionType(string expression, string queryName) {
			if (GetDataProcessingMode(queryName) == DataProcessingMode.Server) {
				Lazy<ServerModeDataSource> serverModeSource = null;
				if (serverModeSources.TryGetValue(queryName, out serverModeSource)) {
					if (serverModeSource.Value != null)
						return serverModeSource.Value.GetUnboundExpressionType(expression, true);
				}
			}
			return typeof(object);
		}
		CalculatedFieldDataColumnInfo IDashboardDataSourceInternal.CreateCalculatedFieldColumnInfo(CalculatedField field, IEnumerable<IParameter> parameters) {
			return new CalculatedFieldDataColumnInfo(field, GetRootNode(field.DataMember), calculatedFieldsController.CalculatedFields, parameters);
		}
		DataNode GetRootNode(string dataMember) {
			IDataSourceSchema dataSourceSchema = ((IDashboardDataSource)this).GetDataSourceSchema(dataMember);
			return dataSourceSchema == null ? null : dataSourceSchema.RootNode;
		}
		bool IDashboardDataSourceInternal.ContainsParametersDisplayMember(string valueMember, string displayMember, string queryName) {			
			IDataSourceSchema dataSourceSchema = ((IDashboardDataSource)this).GetDataSourceSchema(queryName);
			return DataSourceHelper.ContainsDisplayMember(dataSourceSchema, displayMember, valueMember);
		}
		List<ParameterValueViewModel> IDashboardDataSourceInternal.GetParameterValues(string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			if (GetDataProcessingMode(queryName) == DataProcessingMode.Server)
				return CreateModelValuesFromDynamicLookupSettingsServerMode(valueMember, displayMember, queryName, provider);
			else
				return DataSourceHelper.GetDynamicLookupValues(this, queryName, displayMember, valueMember);
		}
		bool IDashboardDataSourceInternal.GetIsSqlServerMode(DataProcessingMode dataProcessingMode, string queryName) {
			return false;
		}
		bool IDashboardDataSourceInternal.IsSqlServerMode(string queryName) {
			return GetDataProcessingMode(queryName) == DataProcessingMode.Server;
		}
		void IDashboardDataSourceInternal.SetParameters(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
		}
		IPivotGridDataSource IDashboardDataSourceInternal.GetPivotDataSource(string queryName) {
			if (queryName != null) {
				Lazy<ServerModeDataSource> serverModeSource = null;
				serverModeSources.TryGetValue(queryName, out serverModeSource);
				if (serverModeSource != null)
					return serverModeSource.Value;
			}
			return null;
		}
		IStorage IDashboardDataSourceInternal.GetStorage(string dataMember) {
			if(dataMember != null) {
				Lazy<IStorage> storage = null;
				storages.TryGetValue(dataMember, out storage);
				if(storage != null)
					return storage.Value;									
			}
			return null;
		}
		IList IDashboardDataSourceInternal.GetListSource(string dataMember) {
			return DataSession.CalculateOldClientModeEngine ? GetListByQuery(dataMember) : null;
		}
		string IDashboardDataSourceInternal.GetName_13_1() {
			return componentNameController.Name_13_1;
		}
		IEnumerable<string> IDashboardDataSourceInternal.GetDataSets() {
			return lists.Select(lw => lw.Key).Union(serverModeSources.Select(sm => sm.Key));
		}
		object IDashboardDataSourceInternal.GetDataSchema(string queryName) {
			PropertyDescriptor pds = ((ITypedList)this).GetItemProperties(null).Cast<PropertyDescriptor>().Where(property => property.Name == queryName).FirstOrDefault();
			Type propertyType = pds.PropertyType;
			if (propertyType.IsGenericType)
				return propertyType.GetGenericArguments()[0];
			return null;
		}
		#endregion
		#region IDashboardComponent
		event EventHandler<NameChangingEventArgs> IDashboardComponent.NameChanging { 
			add { NameChanging += value; } 
			remove { NameChanging -= value; } 
		}
		#endregion
		#region ISupportPrefix
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultEFDataSourceName); } }
		#endregion
		void IExternalSchemaConsumer.SetSchema(string dataMember, string[] newSchema) {
			string [] schema = null;
			schemas.TryGetValue(dataMember, out schema);
			if(schema != null && newSchema != null) {
				if(newSchema.IsSubsetOf(schema))
					return;
			}
			schemas[dataMember] = newSchema;
			PropertyDescriptor pd = ((ITypedList)this).GetItemProperties(null).OfType<PropertyDescriptor>().FirstOrDefault(p=>p.Name == dataMember);
			if(pd!=null)
				ResetStorage(pd);
		}		
	}
}
