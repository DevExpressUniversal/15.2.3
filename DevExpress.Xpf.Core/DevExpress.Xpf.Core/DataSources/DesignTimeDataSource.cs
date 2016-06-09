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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using System.Reflection;
using System.Dynamic;
using System.Windows.Data;
using System.Collections.Specialized;
using DevExpress.Data.Linq;
using DevExpress.Mvvm.Native;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Core.Native {
	public class ItemsSourceWizardIgnoredAttribute : Attribute { }
	public class DesignTimePropertyInfo {
		public DesignTimePropertyInfo(string name, Type propertyType, bool isReadonly) {
			this.Name = name;
			this.PropertyType = propertyType;
			this.IsReadonly = isReadonly;
		}
		public string Name { get; private set; }
		public Type PropertyType { get; private set; }
		public bool IsReadonly { get; private set; }
	}
	public class DesignTimeDataSource : IList, ITypedList, IListSource, IBindingList, ICollectionViewFactory {
		internal static DateTime Today { get { return DesignTimeValuesProvider.Today; } }
		protected class DesignTimePropertyDescriptor : PropertyDescriptor {
			readonly object[] values;
			readonly object[][] distinctValues;
			readonly Type propertyType;
			readonly bool useDistinctValues;
			readonly bool isReadonly;
			public DesignTimePropertyDescriptor(string name, Type propertyType, bool useDistinctValues, bool isReadonly)
				: base(name, null) {
				this.propertyType = propertyType;
				this.useDistinctValues = useDistinctValues;
				this.isReadonly = isReadonly;
				this.values = CreateValues();
				this.distinctValues = CreateDistinctValues();
			}
			protected virtual object[] CreateValues() {
				return DesignTimeValuesProvider.CreateValues();
			}
			protected virtual object[][] CreateDistinctValues() {
				return DesignTimeValuesProvider.CreateDistinctValues();
			}
			public override bool CanResetValue(object component) { return false; }
			public override Type ComponentType { get { return typeof(int); } }
			public override bool IsReadOnly { get { return isReadonly; } }
			public override void ResetValue(object component) { throw new NotImplementedException(); }
			public override void SetValue(object component, object value) { throw new NotImplementedException(); }
			public override bool ShouldSerializeValue(object component) { return false; }
			public override Type PropertyType { get { return propertyType; } }
			public override object GetValue(object component) {
				if(component == null)
					return null;
				if(component is ExpandoObject) return ((IDictionary<string, object>)component)[Name];
				return DesignTimeValuesProvider.GetDesignTimeValue(propertyType, component, values, distinctValues, useDistinctValues);
			}
		}
		class DummyDataClient : IDataControllerData, IDataControllerData2 {
			public UnboundColumnInfoCollection GetUnboundColumns() {
				return null;
			}
			public object GetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
				return null;
			}
			public void SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			}
			public bool CanUseFastProperties {
				get { return false; }
			}
			public ComplexColumnInfoCollection GetComplexColumns() {
				return null;
			}
			public void SubstituteFilter(SubstituteFilterEventArgs args) { }
			public bool HasUserFilter {
				get { return false; }
			}
			public bool? IsRowFit(int listSourceRow, bool fit) {
				return false;
			}
			public PropertyDescriptorCollection PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
				return collection;
			}
		}
		readonly List<ExpandoObject> internalList = new List<ExpandoObject>();
		protected readonly Type dataObjectType;
		private readonly int rowCount;
		private PropertyDescriptorCollection properties;
		protected readonly bool useDistinctValues;
		public bool AreRealColumnsAvailable { get; private set; }
		protected readonly List<DesignTimePropertyInfo> Properties;
		protected DesignTimeDataSource(Type dataObjectType, int rowCount, bool useDistinctValues) {
			this.dataObjectType = dataObjectType;
			this.rowCount = rowCount;
			this.useDistinctValues = useDistinctValues;
		}
		public DesignTimeDataSource(IEnumerable<DesignTimePropertyInfo> columns, int rowCount, bool useDistinctValues = false) {
			this.rowCount = rowCount;
			this.useDistinctValues = useDistinctValues;
			CreateProperties(columns, rowCount, useDistinctValues);
		}
		public DesignTimeDataSource(Type dataObjectType, int rowCount, bool useDistinctValues = false, object originalDataSource = null, IEnumerable<DesignTimePropertyInfo> defaultColumns = null, List<DesignTimePropertyInfo> properties = null)
			: this(dataObjectType, rowCount, useDistinctValues) {
			Properties = properties;
			Initialize(dataObjectType, rowCount, useDistinctValues, originalDataSource, defaultColumns);
		}
		protected void Initialize(Type dataObjectType, int rowCount, bool useDistinctValues, object originalDataSource, IEnumerable<DesignTimePropertyInfo> defaultColumns) {
			IEnumerable<DesignTimePropertyInfo> columns = GetColumns(dataObjectType, originalDataSource, defaultColumns);
			CreateProperties(columns, rowCount, useDistinctValues);
		}
		private void CreateProperties(IEnumerable<DesignTimePropertyInfo> columns, int rowCount, bool useDistinctValues) {
			List<PropertyDescriptor> propertiesList = new List<PropertyDescriptor>();
			if(columns != null)
				foreach(DesignTimePropertyInfo propertyInfo in columns)
					if(!string.IsNullOrEmpty(propertyInfo.Name))
						propertiesList.Add(CreatePropertyDescriptor(useDistinctValues, propertyInfo));
			properties = new PropertyDescriptorCollection(propertiesList.ToArray(), true);
			PopulateInternalList(rowCount);
		}
		protected virtual DesignTimePropertyDescriptor CreatePropertyDescriptor(bool useDistinctValues, DesignTimePropertyInfo propertyInfo) {
			return new DesignTimePropertyDescriptor(propertyInfo.Name, propertyInfo.PropertyType, useDistinctValues, propertyInfo.IsReadonly);
		}
		void PopulateInternalList(int rowCount) {
			this.internalList.Clear();
			for(int i = 0; i < rowCount; i++) {
				ExpandoObject newObj = new ExpandoObject();
				foreach(PropertyDescriptor descriptor in properties) {
					IDictionary<string, object> dict = (IDictionary<string, object>)newObj;
					dict[descriptor.Name] = descriptor.GetValue(i % 3);
				}
				this.internalList.Add(newObj);
			}
		}
		protected virtual DXGridDataController CreateDataController() {
			return new DummyDataController();
		}
		internal IEnumerable<DesignTimePropertyInfo> GetColumns(Type dataObjectType, object originalDataSource, IEnumerable<DesignTimePropertyInfo> defaultColumns) {
			object dataSource = GetDataSource(dataObjectType, originalDataSource);
			if(dataSource != null) {
				AreRealColumnsAvailable = true;
				using(DXGridDataController dataController = CreateDataController()) {
					dataController.DataClient = new DummyDataClient();
					dataController.DataSource = dataSource;
					PopulateColumns(dataController);
					return GetDesignTimePropertyInfo(dataController.Columns);
				}
			}
			return defaultColumns;
		}
		protected virtual void PopulateColumns(DXGridDataController dataController) {
			dataController.PopulateColumns();
		}
		IEnumerable<DesignTimePropertyInfo> GetDesignTimePropertyInfo(DataColumnInfoCollection columns) {
			foreach(DataColumnInfo column in columns) {
				yield return new DesignTimePropertyInfo(column.Name, column.Type, column.ReadOnly);
			}
		}
		object GetDataSource(Type dataObjectType, object originalDataSource) {
			if(dataObjectType != null) {
				Type genericListType = typeof(List<>).MakeGenericType(new Type[] { dataObjectType });
				return (IList)Activator.CreateInstance(genericListType);
			}
			return DataBindingHelper.ExtractDataSource(originalDataSource);
		}
		#region IList Members
		public int Add(object value) { throw new NotImplementedException(); }
		public void Clear() { throw new NotImplementedException(); }
		public void Insert(int index, object value) { throw new NotImplementedException(); }
		public void Remove(object value) { throw new NotImplementedException(); }
		public void RemoveAt(int index) { throw new NotImplementedException(); }
		public void CopyTo(Array array, int index) {
			for(int i = 0; i < Count; i++)
				array.SetValue(this[i], i);
		}
		public object SyncRoot { get { return this; } }
		public bool IsSynchronized { get { return false; } }
		public bool IsFixedSize { get { return true; } }
		public bool IsReadOnly { get { return true; } }
		public int Count { get { return rowCount; } }
		public object this[int index] {
			get {
				if(index < 0 || index >= this.internalList.Count)
					throw new IndexOutOfRangeException();
				return this.internalList[index];
			}
			set { throw new NotImplementedException(); }
		}
		public bool Contains(object value) {
			return this.internalList.Contains(value);
		}
		public int IndexOf(object value) {
			ExpandoObject obj = value as ExpandoObject;
			if(obj == null) return -1;
			return this.internalList.IndexOf(obj);
		}
		public IEnumerator GetEnumerator() {
			foreach(var item in this.internalList)
				yield return item;
		}
		#endregion
		#region ITypedList Members
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return properties;
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return null;
		}
		#endregion
		#region IListSource Members
		public bool ContainsListCollection { get { return false; } }
		public IList GetList() { return this; }
		#endregion
		#region IBindingList Members
		public void AddIndex(PropertyDescriptor property) { throw new NotImplementedException(); }
		public object AddNew() { throw new NotImplementedException(); }
		public bool AllowEdit { get { return false; } }
		public bool AllowNew { get { return false; } }
		public bool AllowRemove { get { return false; } }
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) { }
		public int Find(PropertyDescriptor property, object key) { throw new NotImplementedException(); }
		public bool IsSorted { get { throw new NotImplementedException(); } }
		public event ListChangedEventHandler ListChanged;
		public void RemoveIndex(PropertyDescriptor property) { throw new NotImplementedException(); }
		public void RemoveSort() { }
		public ListSortDirection SortDirection { get { throw new NotImplementedException(); } }
		public PropertyDescriptor SortProperty { get { throw new NotImplementedException(); } }
		public bool SupportsChangeNotification { get { return false; } }
		public bool SupportsSearching { get { return false; } }
		public bool SupportsSorting { get { return true; } }
		void RaiseListChanged() { ListChanged(this, null); }
		#endregion
		#region ICollectionViewFactory Members
		ICollectionView ICollectionViewFactory.CreateView() {
			ICollectionView view = new CollectionViewSource() { Source = this.internalList }.View;
			return new DesignTimeCollectionViewWrapper(view);
		}
		#endregion
#if DEBUGTEST
		internal PropertyDescriptorCollection PropertiesInternal { get { return this.properties; } }
		internal Type DataObjectTypeInternal { get { return this.dataObjectType; } }
#endif
	}
	public interface IDesignTimeDataSource { }
	public class BaseGridDesignTimeDataSource : DesignTimeDataSource, IDesignTimeDataSource {
		private readonly bool flattenHierarchy;
		public BaseGridDesignTimeDataSource(Type dataObjectType, int rowCount, bool useDistinctValues, object originalDataSource = null,
			IEnumerable<DesignTimePropertyInfo> defaultColumns = null, List<DesignTimePropertyInfo> properties = null)
			: base(dataObjectType, rowCount, useDistinctValues, originalDataSource, defaultColumns, properties) { }
		public BaseGridDesignTimeDataSource(Type dataObjectType, int rowCount, bool useDistinctValues, bool flattenHierarchy) :
			base(dataObjectType, rowCount, useDistinctValues) {
			this.flattenHierarchy = flattenHierarchy;
			Initialize(dataObjectType, rowCount, useDistinctValues, null, null);
		}
		protected override void PopulateColumns(DXGridDataController dataController) {
			if(this.dataObjectType == null) {
				base.PopulateColumns(dataController);
				return;
			}
			dataController.Columns.Clear();
			if(Properties != null)
				PopultateColumnsFromDesignProperties(dataController);
			else
				PopultateColumnsFromDataObjectType(dataController);
		}
		void PopultateColumnsFromDataObjectType(DXGridDataController dataController) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			flags |= this.flattenHierarchy ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly;
			PropertyInfo[] props = this.dataObjectType.GetProperties(flags);
			foreach(PropertyInfo property in props) {
				DesignTimePropertyInfo info = new DesignTimePropertyInfo(property.Name, property.PropertyType, IsReadonlyProperty(property));
				dataController.Columns.Add(CreatePropertyDescriptor(this.useDistinctValues, info));
			}
		}
		void PopultateColumnsFromDesignProperties(DXGridDataController dataController) {
			foreach(DesignTimePropertyInfo info in Properties)
				dataController.Columns.Add(CreatePropertyDescriptor(this.useDistinctValues, info));
		}
		protected bool IsReadonlyProperty(PropertyInfo property) {
			return property.GetSetMethod() == null;
		}
	}
	class CollectionViewDesignTimeDataSource : BaseGridDesignTimeDataSource {
		class CollectionViewDesignTimePropertyDescriptor : DesignTimePropertyDescriptor {
			public CollectionViewDesignTimePropertyDescriptor(string name, Type propertyType, bool useDistinctValues, bool isReadonly) : base(name, propertyType, useDistinctValues, isReadonly) { }
			protected override object[] CreateValues() {
				return new object[] { "string", Today, 123, 123, 123, 123, 123, false };
			}
			protected override object[][] CreateDistinctValues() {
				return new object[][] { 
					new object[] { "string1", Today, 123, 123, (byte)123, 123, 123, true },
					new object[] { "string2", Today.AddDays(1), 456, 456, (byte)124, 456, 456, false },
					new object[] { "string3", Today.AddDays(2), 789, 789, (byte)125, 789, 789, true },
				};
			}
		}
		public CollectionViewDesignTimeDataSource(Type dataObjectType, int rowCount, bool useDistinctValues, object originalDataSource = null, IEnumerable<DesignTimePropertyInfo> defaultColumns = null)
			: base(dataObjectType, rowCount, useDistinctValues, originalDataSource, defaultColumns) { }
		public CollectionViewDesignTimeDataSource(Type dataObjectType, int rowCount, bool useDistinctValues, bool flattenHierarchy)
			: base(dataObjectType, rowCount, useDistinctValues, flattenHierarchy) { }
		protected override DesignTimePropertyDescriptor CreatePropertyDescriptor(bool useDistinctValues, DesignTimePropertyInfo propertyInfo) {
			return new CollectionViewDesignTimePropertyDescriptor(propertyInfo.Name, propertyInfo.PropertyType, useDistinctValues, propertyInfo.IsReadonly);
		}
	}
	public class DummyDataController : DXGridDataController {
#if !SL
		protected override System.Windows.Threading.Dispatcher Dispatcher { get { return null; } }
#endif
	}
	class DesignTimeCollectionViewWrapper : IDesignTimeDataSource, ICollectionViewWrapper {
		readonly ICollectionView view;
		public DesignTimeCollectionViewWrapper(ICollectionView view) {
			this.view = view;
		}
		public bool CanFilter { get { return this.view.CanFilter; } }
		public bool CanGroup { get { return this.view.CanGroup; } }
		public bool CanSort { get { return this.view.CanSort; } }
		public bool Contains(object item) {
			return this.view.Contains(item);
		}
		public System.Globalization.CultureInfo Culture {
			get { return this.view.Culture; }
			set { this.view.Culture = value; }
		}
		public event EventHandler CurrentChanged {
			add { this.view.CurrentChanged += value; }
			remove { this.view.CurrentChanged -= value; }
		}
		public event CurrentChangingEventHandler CurrentChanging {
			add { this.view.CurrentChanging += value; }
			remove { this.view.CurrentChanging -= value; }
		}
		public object CurrentItem { get { return this.view.CurrentItem; } }
		public int CurrentPosition { get { return this.view.CurrentPosition; } }
		public IDisposable DeferRefresh() {
			return this.view.DeferRefresh();
		}
		public Predicate<object> Filter {
			get { return this.view.Filter; }
			set { this.view.Filter = value; }
		}
		public System.Collections.ObjectModel.ObservableCollection<GroupDescription> GroupDescriptions { get { return this.view.GroupDescriptions; } }
		public System.Collections.ObjectModel.ReadOnlyObservableCollection<object> Groups { get { return this.view.Groups; } }
		public bool IsCurrentAfterLast { get { return this.view.IsCurrentAfterLast; } }
		public bool IsCurrentBeforeFirst { get { return this.view.IsCurrentBeforeFirst; } }
		public bool IsEmpty { get { return this.view.IsEmpty; } }
		public bool MoveCurrentTo(object item) {
			return this.view.MoveCurrentTo(item);
		}
		public bool MoveCurrentToFirst() {
			return this.view.MoveCurrentToFirst();
		}
		public bool MoveCurrentToLast() {
			return this.view.MoveCurrentToLast();
		}
		public bool MoveCurrentToNext() {
			return this.view.MoveCurrentToNext();
		}
		public bool MoveCurrentToPosition(int position) {
			return this.view.MoveCurrentToPosition(position);
		}
		public bool MoveCurrentToPrevious() {
			return this.view.MoveCurrentToPrevious();
		}
		public void Refresh() {
			this.view.Refresh();
		}
		public SortDescriptionCollection SortDescriptions { get { return this.view.SortDescriptions; } }
		public IEnumerable SourceCollection { get { return this.view.SourceCollection; } }
		public IEnumerator GetEnumerator() {
			return this.view.GetEnumerator();
		}
		public event NotifyCollectionChangedEventHandler CollectionChanged {
			add { this.view.CollectionChanged += value; }
			remove { this.view.CollectionChanged -= value; }
		}
		ICollectionView ICollectionViewWrapper.WrappedView { get { return this.view; } }
	}
}
