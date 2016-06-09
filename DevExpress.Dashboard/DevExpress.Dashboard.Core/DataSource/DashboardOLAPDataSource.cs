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
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data;
using DevExpress.Data.Entity;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.PivotGrid.OLAP;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon {	
	[
	DXToolboxItem(false),
	Designer(TypeNames.DashboardOlapDataSourceComponentDesigner),
	]
	public class DashboardOlapDataSource : IDisposable, IDashboardDataSource, IDashboardDataSourceInternal, IDataComponent, IDataConnectionParametersService {
		#region DisableTypeConverter class
		class DisableTypeConverter : TypeConverter {
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				if (sourceType == typeof(string))
					return false;
				return base.CanConvertFrom(context, sourceType);
			}
		}
		#endregion
		const string XmlDataSource = "OLAPDataSource";
		const string XmlConnectionName = "ConnectionName";
		const string XmlConnectionString = "ConnectionString";
		internal const string DefaultConnectionName = "connection";
		readonly Locker loadingLocker = new Locker();
		readonly DataSourceProperties properties;
		readonly DataSourceComponentNameController componentNameController;
		PivotCustomizationFieldsTreeBase dataSchema;
		OlapDataConnection connection;
		PickManager pickManager;
		IEnumerable<IParameter> parameters;
		ISite site;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardOlapDataSourceName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		]
		public string Name { get { return componentNameController.Name; } set { componentNameController.Name = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardOlapDataSourceComponentName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		]
		public string ComponentName { get { return componentNameController.ComponentName; } set { componentNameController.ComponentName = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardOlapDataSourceConnectionName"),
#endif
		Category(CategoryNames.Connection),
		TypeConverter(typeof(DisableTypeConverter)),
		DefaultValue(null),
		Localizable(false),
		]
		public string ConnectionName { get { return connection.Name; } set { connection.Name = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardOlapDataSourceConnectionString"),
#endif
		Category(CategoryNames.Connection),
		DefaultValue(null),
		Localizable(false),
		TypeConverter(typeof(DisableTypeConverter))
		]
		public string ConnectionString {
			get { return (connection != null && !connection.StoreConnectionNameOnly) ? connection.ConnectionString : null; }
			set {
				if(connection == null || connection.ConnectionString == value)
					return;
				connection.SetConnectionString((string)value);
				dataSchema = null;
			}
		}
		[
		Browsable(false)
		]
		public bool IsDisposed { get; private set; }
		[
		Browsable(false)
		]
		public bool IsConnected { get { return connection != null && connection.IsConnected; } }
		string PickManagerDisplayName { get { return connection != null ? connection.OlapDataSource.CubeCaption : Name; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardOlapDataSourceConfigureOlapConnection")
#else
	Description("")
#endif
		]
		public event ConfigureOlapConnectionEventHandler ConfigureOlapConnection;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardOlapDataSourceConnectionError")
#else
	Description("")
#endif
		]
		public event ConnectionErrorEventHandler ConnectionError;		
		[
		Browsable(false)
		]
		public event EventHandler Disposed;
		internal IConnectionStringsProvider ConnectionStringsProvider { get; set; }
		internal IConnectionStorageService ConnectionStorageService { get; set; }
		internal OlapDataConnection Connection { get { return connection; } set { connection = value; } }
		internal bool StoreConnectionNameOnly { get { return connection.StoreConnectionNameOnly; } set { connection.StoreConnectionNameOnly = value; } }
		event EventHandler<NameChangedEventArgs> NameChanged;
		event EventHandler<NameChangingEventArgs> NameChanging;
		event EventHandler CaptionChanged;
		event EventHandler<DataChangedEventArgs> DataSourceDataChanged;
		public DashboardOlapDataSource() 
			: this(null, null, null) {
		}
		public DashboardOlapDataSource(string name, string connectionName)
			: this(name, connectionName, null) {
		}
		public DashboardOlapDataSource(OlapConnectionParameters connectionParameters)
			: this(null, connectionParameters) {
		}
		public DashboardOlapDataSource(string name, OlapConnectionParameters connectionParameters)
			: this(name, null, connectionParameters) {
		}
		internal DashboardOlapDataSource(string name, string connectionName, OlapConnectionParameters connectionParameters) {
			ConnectionStringsProvider = new RuntimeConnectionStringsProvider();
			ConnectionStorageService = new ConnectionStorageService();
			connection = new OlapDataConnection(connectionName ?? DefaultConnectionName, connectionParameters ?? new OlapConnectionParameters());
			LoadDataSchema();
			properties = new OlapDataSourceProperties(this);
			componentNameController = new DataSourceComponentNameController(name, loadingLocker, () => site);
			componentNameController.NameChanged += componentNameController_NameChanged;
			componentNameController.NameChanging += componentNameController_NameChanging;
			componentNameController.CaptionChanged += componentNameController_CaptionChanged;
			pickManager = new OlapPickManager(PickManagerDisplayName, this);
			pickManager.Clear();
			if(dataSchema != null)			
				pickManager.ConstructTree(PickManagerDisplayName);
			connection.Close();
		}
		public void Dispose() {
			Dispose(true);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void LoadFromXml(XElement element) {
			componentNameController.LoadFromXml(element, false);
			string connectionName = XmlHelperBase.GetAttributeValue(element, XmlConnectionName);
			string connectionString = XmlHelperBase.GetAttributeValue(element, XmlConnectionString);
			connection = new OlapDataConnection(connectionName, new OlapConnectionParameters(connectionString));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public XElement SaveToXml() {
			XElement element = new XElement(DashboardOlapDataSource.XmlDataSource);
			componentNameController.SaveToXml(element);
			if (connection != null) {
				element.Add(new XAttribute(XmlConnectionName, connection.Name));
				if(!connection.StoreConnectionNameOnly)
					element.Add(new XAttribute(XmlConnectionString, connection.ConnectionStringSerializable));
			}
			return element;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void BeginInit() {
			loadingLocker.Lock();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void EndInit() {
			loadingLocker.Unlock();
		}
		public void Fill() {
			((IDataComponent)this).Fill(null);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing && connection != null) {
				connection.Close();
			}
			IsDisposed = true;
			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		void IDataComponent.Fill(IEnumerable<IParameter> sourceParameters) {
			parameters = sourceParameters;
			if(string.IsNullOrEmpty(ConnectionString))
				connection.ConnectionString = ConnectionStringsProvider.GetConnectionString(ConnectionName);
			connection.CreateDataStore(this);
			LoadDataSchema();
			RaiseDataProviderDataChanged();
		}
		void LoadDataSchema() { 
			if(connection == null)
				return;
			IPivotOLAPDataSource ds = connection.OlapDataSource;
			if(ds != null)
				ds.ReloadData();
			try {
				PivotGridData pivotData = new PivotGridData() { PivotDataSource = ds };
			pivotData.RetrieveFields(PivotArea.FilterArea, false);
			dataSchema = new PivotCustomizationFieldsTreeBase(new CustomizationFormFields(pivotData), new PivotCustomizationTreeNodeFactoryBase(pivotData));
			dataSchema.Update(true);
			} catch { }
		}
		void componentNameController_NameChanged(object sender, NameChangedEventArgs e) {
			if(NameChanged != null)
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
		void RaiseDataProviderDataChanged() {
			if(dataSchema != null)
				pickManager.ConstructTree(PickManagerDisplayName);
			RaiseDataSourceDataChanged();
		}
		void RaiseDataSourceDataChanged() {
			if(DataSourceDataChanged!= null)
				DataSourceDataChanged(this, new DataChangedEventArgs(true));
		}
		#region IDashboardDataSource
		IDataProvider IDashboardDataSource.DataProvider { get { return null; } set { throw new NotSupportedException(); } }
#pragma warning disable 612, 618
		OlapDataProvider IDashboardDataSource.OlapDataProvider { get { return null; } }
		SqlDataProvider IDashboardDataSource.SqlDataProvider { get { return null; } }
#pragma warning restore 612, 618
		bool IDashboardDataSource.HasDataProvider { get { return false; } }
		bool IDashboardDataSource.IsServerModeSupported { get { return false; } }
		CalculatedFieldCollection IDashboardDataSource.CalculatedFields { get { return null; } }
		IEnumerable<IParameter> IDashboardDataSource.Parameters { get { return parameters; } }
		string IDashboardDataSource.Filter { get { return null; } set {  } }
		object IDashboardDataSource.Data { get { return null; } set { throw new NotSupportedException(); } }
		DataProcessingMode IDashboardDataSource.DataProcessingMode { get { return DataProcessingMode.Server; } set { throw new NotSupportedException(); } }
		IDataSourceSchema IDashboardDataSource.GetDataSourceSchema(string dataMember) {
			return pickManager;
		}
		ICalculatedFieldsController IDashboardDataSource.GetCalculatedFieldsController() {
			return null;
		}
		IDashboardDataSourceInternal IDashboardDataSource.GetDataSourceInternal() {
			return this;
		}
		#endregion
		#region IDashboardDataSourceInternal
		Dashboard IDashboardDataSourceInternal.Dashboard { get; set; }
		DataSourceProperties IDashboardDataSourceInternal.Properties { get { return properties; } }
		event EventHandler<NameChangedEventArgs> IDashboardDataSourceInternal.NameChanged { 
			add { NameChanged += value; } 
			remove { NameChanged -= value; } 
		}
		event EventHandler IDashboardDataSourceInternal.CaptionChanged { 
			add { CaptionChanged += value; } 
			remove { CaptionChanged -= value; } 
		}		
		event EventHandler<DataChangedEventArgs> IDashboardDataSourceInternal.DataSourceDataChanged { 
			add { DataSourceDataChanged += value; } 
			remove { DataSourceDataChanged -= value; } 
		}
		event EventHandler<DataProcessingModeChangedEventArgs> IDashboardDataSourceInternal.DataProcessingModeChanged { add { } remove { } }
		IPivotGridDataSource IDashboardDataSourceInternal.GetPivotDataSource(string dataMember) {
			return connection != null ? connection.DataSource : null;
		}
		bool IDashboardDataSourceInternal.GetIsSqlServerMode(DataProcessingMode dataProcessingMode, string queryName) {
			return true;
		}
		Type IDashboardDataSourceInternal.ServerGetUnboundExpressionType(string expression, string queryName) {
			return typeof(object);
		}
		CalculatedFieldDataColumnInfo IDashboardDataSourceInternal.CreateCalculatedFieldColumnInfo(CalculatedField field, IEnumerable<IParameter> parameters) {
			throw new NotSupportedException();
		}
		bool IDashboardDataSourceInternal.ContainsParametersDisplayMember(string valueMember, string displayMember, string queryName) {
			return true;
		}
		List<ParameterValueViewModel> IDashboardDataSourceInternal.GetParameterValues(string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			List<ParameterValueViewModel> parameterValues = new List<ParameterValueViewModel>();
			foreach(OLAPMember member in ((IPivotOLAPDataSource)connection.DataSource).GetUniqueMembers(valueMember))
				if (member.Column.AllMember != member)
					parameterValues.Add(new ParameterValueViewModel() {
						Value = member.UniqueName,
						DisplayText = member.Caption
					});
			return parameterValues;
		}
		void IDashboardDataSourceInternal.SetParameters(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
		}
		string IDashboardDataSourceInternal.GetName_13_1() {
			return componentNameController.Name_13_1;
		}
		IList IDashboardDataSourceInternal.GetListSource(string queryName) {
			return null;
		}
		IStorage IDashboardDataSourceInternal.GetStorage(string dataMember) {
			return null;
		}
		IEnumerable<string> IDashboardDataSourceInternal.GetDataSets() {
			return new string[] { string.Empty };
		}
		bool IDashboardDataSourceInternal.IsSqlServerMode(string queryName) {
			return true;
		}
		object IDashboardDataSourceInternal.GetDataSchema(string dataMember) {
			return dataSchema;
		}  
		#endregion
		#region IDataConnectionParametersService
		DataConnectionParametersBase IDataConnectionParametersService.RaiseConfigureDataConnection(string connectionName, DataConnectionParametersBase parameters) {
			if (ConfigureOlapConnection != null) {
				OlapConnectionParameters olapParameters = parameters as OlapConnectionParameters;
				ConfigureOlapConnectionEventArgs args = new ConfigureOlapConnectionEventArgs(connectionName, olapParameters != null ? olapParameters.ConnectionString : string.Empty);
				ConfigureOlapConnection(this, args);
				return new OlapConnectionParameters(args.ConnectionString);
			}
			return parameters;
		}
		DataConnectionParametersBase IDataConnectionParametersService.RaiseHandleConnectionError(ConnectionErrorEventArgs eventArgs) {
			if (ConnectionError != null)
				ConnectionError(this, eventArgs);
			if (eventArgs.Handled && eventArgs.Cancel)
				return null;
			return eventArgs.ConnectionParameters;
		}
		#endregion
		#region IDashboardComponent
		event EventHandler<NameChangingEventArgs> IDashboardComponent.NameChanging { 
			add { NameChanging += value; } 
			remove { NameChanging -= value; } 
		}
		#endregion
		#region IDataComponent
		string IDataComponent.DataMember { get { throw new NotSupportedException(); } } 
		#endregion
		#region IComponent
		ISite IComponent.Site { get { return site; } set { site = value; } }
		#endregion
		#region ISupportPrefix
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultOlapDataSourceName); } }
		#endregion
	}
	public delegate  void ConfigureOlapConnectionEventHandler (object sender, ConfigureOlapConnectionEventArgs eventArgs);
	public class ConfigureOlapConnectionEventArgs : EventArgs {
		public string ConnectionString { get; set; }
		public string ConnectionName { get; set; }
		public ConfigureOlapConnectionEventArgs(string connectionName, string connectionString) {
			ConnectionString = connectionString;
			ConnectionName = connectionName;
		}
	}
}
