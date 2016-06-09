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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
using System.Reflection;
using DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardCommon {
	[
	DXToolboxItem(false),
	]
	public class ExtractFileDataSource : IDashboardDataSource, IDashboardDataSourceInternal, IDataComponent {
		internal static ConcurrentDictionary<string, AsyncSource> FileSources = new ConcurrentDictionary<string, AsyncSource>();
		const string xmlFileName = "FileName";
		const string xmlFilter = "Filter";
		const string xmlDataSource = "ExtractFileDataSource";
		readonly CalculatedFieldsController calculatedFieldsController;
		readonly DataSourceProperties properties;
		readonly DataSourceComponentNameController componentNameController;		
		readonly PickManagerWithCalcFields pickManager;
		readonly Locker loadingLocker = new Locker();
		readonly Locker pickManagerLocker = new Locker();
		IEnumerable<IParameter> parameters;
		IStorage storage;
		ISite site;
		string filter;
		StorageListWrapper list;
		[
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		Browsable(true)
		]
		public string FileName { get; set; }
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
	DevExpressDashboardCoreLocalizedDescription("DashboardObjectDataSourceName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		Browsable(true)
		]
		public string Name { get { return componentNameController.Name; } set { componentNameController.Name = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardObjectDataSourceComponentName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		]
		public string ComponentName { get { return componentNameController.ComponentName; } set { componentNameController.ComponentName = value; } }
		string PickManagerDisplayName { get { return Name; } }
		public event EventHandler Disposed;
		event EventHandler<DataChangedEventArgs> DataSourceDataChanged;
		event EventHandler<NameChangingEventArgs> NameChanging;
		event EventHandler<NameChangedEventArgs> NameChanged;
		event EventHandler CaptionChanged;
		public ExtractFileDataSource()
			: this(null) {
		}
		public ExtractFileDataSource(string dataSourceName) {
			properties = new DataSourceProperties(this);
			calculatedFieldsController = new CalculatedFieldsController(this);
			pickManager = new PickManagerWithCalcFields(string.Empty, this, null, calculatedFieldsController);
			componentNameController = new DataSourceComponentNameController(dataSourceName, loadingLocker, () => ((IComponent)this).Site);
			componentNameController.NameChanged += componentNameController_NameChanged;
			componentNameController.NameChanging += componentNameController_NameChanging;
			componentNameController.CaptionChanged += componentNameController_CaptionChanged;
			calculatedFieldsController.ConstructTree += calculatedFieldsController_ConstructTree;			
		}
		public ExtractFileDataSource(string dataSourceName, string fileName)
			: this(dataSourceName) {
			FileName = fileName;
		}
		public void Dispose() {
			Dispose(true);
		}
		public void Fill(IEnumerable<IParameter> parameters) {
			if(DataSourceDataChanged != null)
				DataSourceDataChanged(this, new DataChangedEventArgs(false));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void LoadFromXml(XElement element) {
			componentNameController.LoadFromXml(element, false);
			calculatedFieldsController.LoadFromXml(element);
			XElement filterElement = element.Element(xmlFilter);
			if(filterElement != null)
				Filter = filterElement.Value;
			XElement fileNameElement = element.Element(xmlFileName);
			if(fileNameElement != null)
				FileName = fileNameElement.Value;		 
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public XElement SaveToXml() {
			XElement element = new XElement(ExtractFileDataSource.xmlDataSource);
			componentNameController.SaveToXml(element);
			calculatedFieldsController.SaveToXml(element);
			if(!string.IsNullOrEmpty(Filter))
				element.Add(new XElement(xmlFilter, Filter));			
			if(!string.IsNullOrEmpty(FileName))
				element.Add(new XElement(xmlFileName, FileName));
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
		protected virtual void Dispose(bool disposing) {
			if(disposing) {				
			}			
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
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
		IStorage GetStorage() {
			if(storage == null) {
				AsyncSource source;
				if(!ExtractFileDataSource.FileSources.TryGetValue(FileName, out source)) {
					source = new AsyncSource(FileName);
					ExtractFileDataSource.FileSources.AddOrUpdate(FileName, source, (name, s) => source);
				}
				storage = source;
			}
			return storage;
		}
		#region IDashboardDataSource
#if !DXPORTABLE
		IDataProvider IDashboardDataSource.DataProvider { get { return null; } set { throw new NotSupportedException(); } }
#pragma warning disable 612, 618
		OlapDataProvider IDashboardDataSource.OlapDataProvider { get { return null; } }
		SqlDataProvider IDashboardDataSource.SqlDataProvider { get { return null; } }
#pragma warning restore 612, 618
#endif
		bool IDashboardDataSource.HasDataProvider { get { return false; } }
		bool IDashboardDataSource.IsConnected { get { return true; } }
		bool IDashboardDataSource.IsServerModeSupported { get { return false; } }
		DataProcessingMode IDashboardDataSource.DataProcessingMode { get { return DataProcessingMode.Client; } set { throw new NotSupportedException(); } }
		IEnumerable<IParameter> IDashboardDataSource.Parameters { get { return parameters; } }
		object IDashboardDataSource.Data {
			get { return null; }
			set { throw new NotSupportedException(); }
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
			if(list == null && DataSession.CalculateOldClientModeEngine) {
				list = new StorageListWrapper(GetStorage(), FileName);
			}
			return list;
		}
		IStorage IDashboardDataSourceInternal.GetStorage(string dataMember) {
			return GetStorage();
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
		#region IDataComponent
		string IDataComponent.DataMember { get { throw new NotSupportedException(); } } 
		#endregion
		#region IComponent
		ISite IComponent.Site { get { return site; } set { site = value; } }
		#endregion
		#region ISupportPrefix
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultFileExtractDataSourceName); } }
		#endregion        
	}
}
namespace DevExpress.DashboardCommon.Native.Extensions {
	public static class ExtractFileDataSourceExtensions {
		public static void CreateFileStorage(this DashboardSqlDataSource dataSource, string queryName, string fileName) {
			dataSource.CreateFileStorage(queryName, fileName, Enumerable.Empty<IParameter>());
		}
		public static void CreateFileStorage(this DashboardSqlDataSource dataSource, string queryName, string fileName, IEnumerable<IParameter> parameters) {
			dataSource.CreateFileStorage(queryName, fileName, parameters);
		}
		public static void DisposeAsyncSources(this ExtractFileDataSource dataSource) {
			var sources = ExtractFileDataSource.FileSources.Select(kvp => kvp.Value).ToList();
			ExtractFileDataSource.FileSources.Clear();
			foreach(IDisposable source in sources)
				source.Dispose();
		}
	}
}
namespace DevExpress.DashboardCommon.Native{
	public class StoragePropertyDescriptor : PropertyDescriptor {
		int i;
		Type type;
		public StoragePropertyDescriptor(string name, int i, Type type): base (name, null) {
			this.i = i;
			this.type = type;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override Type ComponentType {
			get { return typeof(StorageListWrapper); }
		}
		public override object GetValue(object component) {
			return ((object[])component)[i];			
		}
		public override bool IsReadOnly {
			get { return true ; }
		}
		public override Type PropertyType {
			get { return type; }
		}
		public override void ResetValue(object component) {
			throw new NotSupportedException();
		}
		public override void SetValue(object component, object value) {
			throw new NotSupportedException();
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
	public class StorageListWrapper : ITypedList, IList {
		IStorage storage;
		string listName;
		object[] data;
		public StorageListWrapper(IStorage storage, string listName) {
			this.storage = storage;
			this.listName = listName;			
			int storageColumnsCount = storage.Columns.Count;
			List<object[]> tempData = new List<object[]>();
			for(int i = 0; i < storageColumnsCount; i++) {
				string columnName = storage.Columns[i];
				tempData.Add(ReadUntypedData(columnName));
			}
			if(storageColumnsCount > 0) {
				int dataLength = tempData[0].Length;
				data = new object[dataLength];
				for(int i = 0; i < dataLength; i++) {
					data[i] = new object[storageColumnsCount];
					for(int j = 0; j < storageColumnsCount; j++)
						((object[])data[i])[j] = tempData[j][i];
				}
			}
		}
		object[] ReadUntypedData(string columnName) {
			var buf = new DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Buffer(new Scan(columnName, storage));
			 IDictionary<SingleBlockOperation, DataVectorBase> results = DataProcessor.ExecuteOperationGraph(buf);
			return results[buf].GetUntypedData().Take(results[buf].Count).ToArray();
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			PropertyDescriptor[] properties = new PropertyDescriptor[storage.Columns.Count];
			for(int i = 0; i < storage.Columns.Count; i++)
				properties[i] = new StoragePropertyDescriptor(storage.Columns[i], i, storage.GetColumnType(storage.Columns[i]));			
			return new PropertyDescriptorCollection(properties);
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return listName;
		}
		public int Add(object value) {
			throw new NotSupportedException();
		}
		public void Clear() {
			throw new NotSupportedException();
		}
		public bool Contains(object value) {
			throw new NotSupportedException();
		}
		public int IndexOf(object value) {
			throw new NotSupportedException();
		}
		public void Insert(int index, object value) {
			throw new NotSupportedException();
		}
		public bool IsFixedSize {
			get { return true;}
		}
		public bool IsReadOnly {
			get { return true; }
		}
		public void Remove(object value) {
			throw new NotSupportedException();
		}
		public void RemoveAt(int index) {
			throw new NotSupportedException();
		}
		public object this[int index] {
			get {
				return data[index];
			}
			set {
				throw new NotSupportedException();
			}
		}
		public void CopyTo(Array array, int index) {
			throw new NotSupportedException();
		}
		public int Count {
			get { return data != null ? data.Length : 0; }
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get {return new object(); }
		}
		public IEnumerator GetEnumerator() {
			return data.GetEnumerator();
		}
	}
}
