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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.IO;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon {
	[
	DXToolboxItem(false),
	SuppressMessage("Microsoft.Design", "CA1039")
	]
	public class DashboardObjectDataSource : ObjectDataSource, IDashboardDataSource, IDashboardDataSourceInternal, IExternalSchemaConsumer {
		const string xmlFilter = "Filter";
		const string xmlDataSchema = "DataSchema";
		string filter;
		string dataSchemaString;
		readonly Locker loadingLocker = new Locker();
		readonly Locker pickManagerLocker = new Locker();
		readonly CalculatedFieldsController calculatedFieldsController;
		readonly DataSourceProperties properties;
		readonly DataSourceComponentNameController componentNameController;
		readonly BindingListController bindingListController;
		readonly PickManagerWithCalcFields pickManager;
		IEnumerable<IParameter> parameters;
		ObjectDataSourceListWrapper listWrapper;
		IStorage storage;
		string[] schema;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardObjectDataSourceCalculatedFields"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public CalculatedFieldCollection CalculatedFields { get { return calculatedFieldsController.CalculatedFields; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardObjectDataSourceFilter"),
#endif
		Category(CategoryNames.Data),
		Editor("DevExpress.DashboardWin.Design.DataSourceFilterCriteriaEditor," + AssemblyInfo.SRAssemblyDashboardWinDesign, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false)
		]
		public string Filter { get { return filter; } set { filter = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardObjectDataSourceDataSchema"),
#endif
		Category(CategoryNames.Data),
		Editor("DevExpress.Utils.UI.XmlSchemaEditor," + AssemblyInfo.SRAssemblyUtilsUI, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false)
		]
		public string DataSchema {
			get { return dataSchemaString; }
			set {
				dataSchemaString = value;
				DataSource = XmlDataHelper.CreateDataSetBySchema(dataSchemaString);
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardObjectDataSourceName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		Browsable(true)
		]
		public override string Name { get { return componentNameController.Name; } set { componentNameController.Name = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardObjectDataSourceComponentName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		]
		public string ComponentName { get { return componentNameController.ComponentName; } set { componentNameController.ComponentName = value; } }
		string PickManagerDisplayName { get { return string.IsNullOrEmpty(DataMember) ? Name : DataMember; } }
		event EventHandler<DataChangedEventArgs> DataSourceDataChanged;
		event EventHandler<NameChangingEventArgs> NameChanging;
		event EventHandler<NameChangedEventArgs> NameChanged;
		event EventHandler CaptionChanged;
		public DashboardObjectDataSource(string name)
			: this(name, null) {
		}
		public DashboardObjectDataSource(object data)
			: this(null, data) {
		}
		public DashboardObjectDataSource()
			: this(null) {
		}
		public DashboardObjectDataSource(string name, object data)
			: base() {
			properties = new DataSourceProperties(this);
			calculatedFieldsController = new CalculatedFieldsController(this);
			pickManager = new PickManagerWithCalcFields(string.Empty, this, null, calculatedFieldsController);
			bindingListController = new BindingListController(this);
			bindingListController.CustomDataChanged += bindingListController_CustomDataChanged;
			componentNameController = new DataSourceComponentNameController(name, loadingLocker, () => Site);
			componentNameController.NameChanged += componentNameController_NameChanged;
			componentNameController.NameChanging += componentNameController_NameChanging;
			componentNameController.CaptionChanged += componentNameController_CaptionChanged;
			calculatedFieldsController.ConstructTree += calculatedFieldsController_ConstructTree;
			calculatedFieldsController.CalculatedFieldsChanged += calculatedFieldsController_CalculatedFieldsChanged;
			((IDashboardDataSource)this).Data = data;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override XElement SaveToXml() {
			XElement element = base.SaveToXml();
			componentNameController.SaveComponentNameToXml(element);
			calculatedFieldsController.SaveToXml(element);
			if(!string.IsNullOrEmpty(Filter))
				element.Add(new XElement(xmlFilter, Filter));
			if(!string.IsNullOrEmpty(DataSchema))
				element.Add(new XElement(xmlDataSchema, DataSchema));
			return element;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void LoadFromXml(XElement element) {
			try {
				base.LoadFromXml(element);
			} catch(BadImageFormatException) {
			} catch(FileLoadException) {
			} catch(FileNotFoundException) {
			} catch(TypeLoadException) {
			}
			componentNameController.LoadComponentNameFromXml(element);
			calculatedFieldsController.LoadFromXml(element);
			XElement filterElement = element.Element(xmlFilter);
			if(filterElement != null)
				Filter = filterElement.Value;
			XElement dataSchemaElement = element.Element(xmlDataSchema);
			if(dataSchemaElement != null)
				DataSchema = dataSchemaElement.Value;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void BeginInit() {
			base.BeginInit();
			loadingLocker.Lock();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void EndInit() {
			loadingLocker.Unlock();
			base.EndInit();
		}
		internal bool SetData(object value) {
			bindingListController.UnsubscribeDataEvents(value);
			bindingListController.SubscribeDataEvents(value);
			BeginUpdate();
			DataSource = value;
			DataMember = null;
			EndUpdate();
			return true;
		}
		internal object RequestData() {
			return RaiseDataLoading();
		}
		protected override bool ShouldSerializeDataSourceType() {
			return false;
		}
		protected override void OnAfterFill(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
			listWrapper = new ObjectDataSourceListWrapper(this, this, parameters);
			ConstructTree();
			storage = null;
			if(DataSourceDataChanged != null)
				DataSourceDataChanged(this, new DataChangedEventArgs(true));
		}
		protected virtual IStorage GetStorageInternal() {
			if(storage == null) {
				if(schema != null) {
					PropertyDescriptorCollection properties = ((ITypedList)this).GetItemProperties(null);
					using(DataReaderExEnumerableWrapper dataReaderEx = new DataReaderExEnumerableWrapper(properties, schema, listWrapper)) {
						DataStorageProcessor processor = new DataStorageProcessor(dataReaderEx);
						storage = processor.ProcessTables();
					}
				}
			}
			return storage;
		}
		void bindingListController_CustomDataChanged(object sender, DataSourceChangedEventArgs e) {
			Fill();
		}
		void calculatedFieldsController_CalculatedFieldsChanged(object sender, EventArgs e) {
			if(listWrapper != null)
				listWrapper.RePopulateColumns(parameters);
		}
		void calculatedFieldsController_ConstructTree(object sender, EventArgs e) {
			ConstructTree();
		}
		void componentNameController_CaptionChanged(object sender, EventArgs e) {
			ConstructTree();
			if(CaptionChanged != null)
				CaptionChanged(this, e);
		}
		void componentNameController_NameChanging(object sender, NameChangingEventArgs e) {
			if(NameChanging != null)
				NameChanging(this, e);
		}
		void componentNameController_NameChanged(object sender, NameChangedEventArgs e) {
			if(NameChanged != null)
				NameChanged(this, e);
		}
		void ConstructTree() {
			if(pickManagerLocker.IsLocked)
				return;
			try {
				pickManagerLocker.Lock();
				pickManager.ConstructTree(PickManagerDisplayName);
			} finally {
				pickManagerLocker.Unlock();
			}
		}
		object RaiseDataLoading() {
			if(((IDashboardDataSourceInternal)this).Dashboard == null)
				return null;
			return ((IDashboardDataSourceInternal)this).Dashboard.RaiseDataLoading(this);
		}
		#region IDashboardDataSource
		IDataProvider IDashboardDataSource.DataProvider { get { return null; } set { throw new NotSupportedException(); } }
#pragma warning disable 612, 618
		OlapDataProvider IDashboardDataSource.OlapDataProvider { get { return null; } }
		SqlDataProvider IDashboardDataSource.SqlDataProvider { get { return null; } }
#pragma warning restore 612, 618
		bool IDashboardDataSource.HasDataProvider { get { return false; } }
		bool IDashboardDataSource.IsConnected { get { return true; } }
		bool IDashboardDataSource.IsServerModeSupported { get { return false; } }
		DataProcessingMode IDashboardDataSource.DataProcessingMode { get { return DataProcessingMode.Client; } set { throw new NotSupportedException(); } }
		IEnumerable<IParameter> IDashboardDataSource.Parameters { get { return parameters; } }
		object IDashboardDataSource.Data {
			get { return DataSource; }
			set {
				if(SetData(value))
					Fill();
				else
					DataSource = value;
			}
		}
		IDataSourceSchema IDashboardDataSource.GetDataSourceSchema(string dataMember) {
			return pickManager;
		}
		ICalculatedFieldsController IDashboardDataSource.GetCalculatedFieldsController() {
			return calculatedFieldsController;
		}
		IDashboardDataSourceInternal IDashboardDataSource.GetDataSourceInternal() {
			return this;
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
		IPivotGridDataSource IDashboardDataSourceInternal.GetPivotDataSource(string dataMember) {
			return null;
		}
		IList IDashboardDataSourceInternal.GetListSource(string dataMember) {
			return listWrapper;
		}
		IStorage IDashboardDataSourceInternal.GetStorage(string dataMember) {
			return GetStorageInternal();
		}
		bool IDashboardDataSourceInternal.GetIsSqlServerMode(DataProcessingMode dataProcessingMode, string queryName) {
			return false;
		}
		Type IDashboardDataSourceInternal.ServerGetUnboundExpressionType(string expression, string queryName) {
			return typeof(object);
		}
		CalculatedFieldDataColumnInfo IDashboardDataSourceInternal.CreateCalculatedFieldColumnInfo(CalculatedField field, IEnumerable<IParameter> parameters) {
			return new CalculatedFieldDataColumnInfo(field, pickManager.RootNode, calculatedFieldsController.CalculatedFields, parameters, true);
		}
		bool IDashboardDataSourceInternal.ContainsParametersDisplayMember(string valueMember, string displayMember, string queryName) {
			IDataSourceSchema dataSourceSchema = ((IDashboardDataSource)this).GetDataSourceSchema(queryName);
			return DataSourceHelper.ContainsDisplayMember(dataSourceSchema, displayMember, valueMember);
		}
		List<ViewModel.ParameterValueViewModel> IDashboardDataSourceInternal.GetParameterValues(string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			return DataSourceHelper.GetDynamicLookupValues(this, queryName, displayMember, valueMember);
		}
		string IDashboardDataSourceInternal.GetName_13_1() {
			return componentNameController.Name_13_1;
		}
		bool IDashboardDataSourceInternal.IsSqlServerMode(string queryName) {
			return false;
		}
		IEnumerable<string> IDashboardDataSourceInternal.GetDataSets() {
			return new string[] { string.Empty };
		}
		object IDashboardDataSourceInternal.GetDataSchema(string dataMember) {
			return this;
		}
		void IDashboardDataSourceInternal.SetParameters(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
		}
		#endregion
		#region IDashboardComponent
		event EventHandler<NameChangingEventArgs> IDashboardComponent.NameChanging {
			add { NameChanging += value; }
			remove { NameChanging -= value; }
		}
		#endregion
		#region ISupportPrefix
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultObjectDataSourceName); } }
		#endregion        
		#region ISupportStorageUpdate
		void IExternalSchemaConsumer.SetSchema(string dataMember, string[] newSchema) {
			if(schema != null && newSchema != null) {
				if(newSchema.IsSubsetOf(schema))
					return;
			}
			schema = newSchema;
			storage = null;
		}		
		#endregion
	}
}
