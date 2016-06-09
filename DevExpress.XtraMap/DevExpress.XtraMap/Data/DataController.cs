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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap.Native {
	public class MapDataController : ListSourceDataController, IMapDataController {
		MappingCollection mappings;
		MapItemAttributeMappingCollection attributeMappings;
		string argument = string.Empty;
		public MappingCollection Mappings { get { return mappings; } }
		public MapItemAttributeMappingCollection AttributeMappings { get { return attributeMappings; }  }
		public string Argument { get { return argument; } }
		public event EventHandler BindingListChanged;
		public MapDataController() {
			this.mappings = new MappingCollection();
			this.attributeMappings = new MapItemAttributeMappingCollection(null);
		}
		void RaiseBindingListChanged() {
			if(BindingListChanged != null) BindingListChanged(this, EventArgs.Empty);
		}
		object ReadRowAttributeValue(int rowIndex, MapItemAttributeMapping mapping) {
			if(string.IsNullOrEmpty(mapping.Name) || string.IsNullOrEmpty(mapping.Member))
				return null;
			return GetRowValue(rowIndex, mapping.Member);
		}
		void LoadObjectAttributes(IMapDataItem item, int rowIndex, MapItemAttributeMapping mapping) {
			if(string.IsNullOrEmpty(mapping.Name) || string.IsNullOrEmpty(mapping.Member))
				return;
			object attrValue = GetRowValue(rowIndex, mapping.Member);
			item.AddAttribute(new MapItemAttribute() { Name = mapping.Name, Type = mapping.Type, Value = attrValue });
		}
		void LoadObjectProperty(IMapDataItem item, int rowIndex, MapItemMappingBase mapping) {
			object propertyValue = GetRowValue(rowIndex, mapping.Member);
			mapping.SetValue(item, propertyValue);
		}
		protected override void OnBindingListChanged(ListChangedEventArgs e) {
			base.OnBindingListChanged(e);
			RaiseBindingListChanged();
		}
		internal void ApplyMappings(int rowIndex, IMapDataItem item, MappingCollection mappings) {
			foreach(MapItemMappingBase mapping in mappings)
				LoadObjectProperty(item, rowIndex, mapping);
		}
		internal void ApplyAttributeMappings(int rowIndex, IMapDataItem item, MapItemAttributeMappingCollection attributeMappings) {
			foreach(MapItemAttributeMapping attrMapping in attributeMappings)
				LoadObjectAttributes(item, rowIndex, attrMapping);
		}
		internal void SetAttributeMappings(MapItemAttributeMappingCollection attributeMappings) {
			this.attributeMappings = attributeMappings;
		}
		public void SetItemVisibleRowIndex(IMapDataItem item, int rowIndex, int[] listSourceIndices) {
			item.RowIndex = rowIndex;
			item.ListSourceRowIndices = listSourceIndices;
		}
		public void SetMapItemValueSummary(IMapDataItem item, object summary) {
			MapItemMappingBase mapping = Mappings[MapItemMappingNames.Value];
			if(item != null && mapping != null) mapping.SetValue(item, summary);
		}
		public void LoadItemProperties(IMapDataItem item, int rowIndex) {
			ApplyMappings(rowIndex, item, Mappings);
			ApplyAttributeMappings(rowIndex, item, AttributeMappings);
		}
		public void LoadItemAttributeArray(IMapDataItem item, int[] rowIndices) {
			foreach(MapItemAttributeMapping mapping in AttributeMappings) {
				IList<object> values = CreateComplexAttributeValues(rowIndices, mapping);
				item.AddAttribute(new MapItemAttribute() { Name = mapping.Name, Type = mapping.Type, Value = values });
			}
		}
		IList<object> CreateComplexAttributeValues(int[] rowIndices, MapItemAttributeMapping mapping) {
			int count = rowIndices.Length;
			object[] values = new object[count];
			for(int i = 0; i < rowIndices.Length; i++)
				values[i] = ReadRowAttributeValue(rowIndices[i], mapping);
			return values;
		}
	}
	public class LayerDataManager : MapDisposableObject, IDataControllerData, IDataControllerData2 {
		DataSourceAdapterBase dataAdapter;
		MapDataController dataController;
		bool shouldRePopulateColumns;
		bool shouldClearItems;
		protected internal bool IsDataLoaded { get; set; }
		protected MapDataController DataController { get { return dataController; } }
		protected DataSourceAdapterBase DataSourceAdapter { get { return DataAdapter as DataSourceAdapterBase; } }
		protected IMapDataAdapter DataAdapter { get { return dataAdapter; } }
		protected internal IList ListSource {
			get { return dataController.ListSource; }
			set {
				dataController.DataClient = this;
				dataController.ListSource = value;
			}
		}
		public virtual object DataSource { get { return DataSourceAdapter.DataSource; } }
		public virtual string DataMember { get { return DataSourceAdapter.DataMember; } }
		public MappingCollection Mappings { get { return DataController.Mappings; } }
		public MapItemAttributeMappingCollection AttributeMappings { get { return DataSourceAdapter.AttributeMappings; } }
		public LayerDataManager(DataSourceAdapterBase dataAdapter) {
			this.dataAdapter = dataAdapter;
			this.dataController = new MapDataController();
			dataController.BindingListChanged += OnBindingListChanged;
		}
		protected override void DisposeOverride() {
			if(dataController != null) {
				dataController.BindingListChanged -= OnBindingListChanged;
				dataController.Dispose();
				dataController = null;
			}
			base.DisposeOverride();
		}
		#region IDataControllerData2 Members
		void AddComplexColumnInfo(ComplexColumnInfoCollection collection, string item) {
			if(item.Contains(".") && dataController.Columns[item] == null && collection.IndexOf(item) < 0)
				collection.Add(item);
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection collection = new ComplexColumnInfoCollection();
			foreach(MapItemMappingBase mapping in Mappings) {
				AddComplexColumnInfo(collection, mapping.Member);
			}
			foreach(MapItemAttributeMapping attributeMapping in AttributeMappings) {
				AddComplexColumnInfo(collection, attributeMapping.Member);
			}
			return collection;
		}
		bool IDataControllerData2.CanUseFastProperties { get { return true; } }
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter { get { return false; } }
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) { return null; }
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) { return collection; }
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() { return null; }
		object IDataControllerData.GetUnboundData(int row, DataColumnInfo col, object value) { return null; }
		void IDataControllerData.SetUnboundData(int row, DataColumnInfo col, object value) { }
		#endregion
		internal void OnDataPropertyChanged() {
			UpdateDataState();
		}
		internal void UpdateDataState() {
			if(IsDataSourceListEmpty()) {
				ClearItemsCore();
				UpdateDataResetState();
			} else
				UpdateDataLoadState();
			DataSourceAdapter.OnDataChanged();
		}
		internal bool IsDataSourceListEmpty() {
			return ObtainIListFromDataSource(DataSource, DataMember) == null;
		}
		IList ObtainIListFromDataSource(object dataSource, string dataMember) {
			return DataHelper.GetList(dataSource, dataMember);
		}
		void OnBindingListChanged(object sender, EventArgs e) {
			UpdateDataLoadState();
			DataSourceAdapter.OnDataChanged();
		}
		internal void OnMappingsChanged() {
			shouldRePopulateColumns = true;
			UpdateDataState();
		}
		void UpdateDataLoadState() {
			IsDataLoaded = false;
			this.shouldClearItems = true;
		}
		void UpdateDataResetState() {
			IsDataLoaded = false;
			this.shouldClearItems = false;
		}
		void UpdateMappings() {
			Mappings.BeginUpdate();
			try {
				DataSourceAdapter.FillActualMappings(Mappings);
				Mappings.AddRange(DataSourceAdapter.PropertyMappings);
			} finally {
				Mappings.EndUpdate();
			}
		}
		void LoadMapItems(IList<MapItem> targetCollection, IMapItemFactory itemFactory) {
			IMapDataEnumerator en = CreateDataEnumerator();
			int iterateNumber = 0;
			while(en.MoveNext()) {
				MapItemType type = GetMapItemType(en.GetCurrentRowIndex());
				object currrent = ((IEnumerator)en).Current;
				MapItem item = itemFactory.CreateMapItem(type, currrent);
				en.Accept(item, iterateNumber++);
				targetCollection.Add(item);
			}
		}
		IMapDataEnumerator CreateDataEnumerator() {
			return DataSourceAdapter.CreateDataEnumerator(DataController);
		}
		MapItemType GetMapItemType(int i) {
			MapItemType type = TryGetMapItemTypeFromMapping(i);
			if(type != MapItemType.Unknown)
				return type;
			MapItemType defaultType = DataAdapter.DefaultMapItemType;
			return defaultType == MapItemType.Unknown ? MapItemType.Custom : defaultType;
		}
		MapItemType TryGetMapItemTypeFromMapping(int rowIndex) {
			MapItemMappingBase typeMapping = Mappings[MapItemMappingNames.Type];
			if(typeMapping == null)
				return MapItemType.Unknown;
			DataColumnInfo column = dataController.Columns[typeMapping.Member];
			if(column != null) {
				try {
					object propertyValue = dataController.GetRowValue(rowIndex, column);
					int typeInt = Convert.ToInt32(propertyValue);
					return (MapItemType)typeInt;
				} catch {
					;
				}
			}
			return MapItemType.Unknown;
		}
		void ClearItems() {
			if(shouldClearItems) ClearItemsCore();
		}
		void ClearItemsCore() {
			DataSourceAdapter.Clear();
			this.shouldClearItems = false;
		}
		void AggregateData() {
			DataSourceAdapter.Aggregate(DataController);
		}
		void LoadDataCore(IMapItemFactory itemFactory) {
			MapItemCollection items = DataAdapter.SourceItems as MapItemCollection;
			items.BeginUpdate();
			try {
				LoadMapItems(items, itemFactory);
			} finally {
				items.EndUpdate();
			}
		}
		internal object GetSourceObjectByRowIndex(int rowIndex) {
			return (rowIndex != MapItem.UndefinedRowIndex) ? dataController.GetRow(rowIndex) : null;
		}
		public void LoadData(IMapItemFactory itemFactory) {
			UpdateMappings();
			DataController.SetAttributeMappings(AttributeMappings);
			RePopulateColumns();
			EnsureListSourceAssigned();
			ClearItems();
			AggregateData();
			LoadDataCore(itemFactory);
			IsDataLoaded = true;
		}
		void RePopulateColumns() {
			if(shouldRePopulateColumns) {
				dataController.RePopulateColumns();
				shouldRePopulateColumns = false;
			}
		}
		void EnsureListSourceAssigned() {
			IList list = ObtainIListFromDataSource(DataSource, DataMember);
			if(!Object.Equals(ListSource, list) || shouldRePopulateColumns)
				ListSource = list;
		}
		public StringCollection GetColumnNames() {
			EnsureListSourceAssigned();
			return DataHelper.GetColumnNames(ListSource);
		}
		public MapItem GetMapItemBySourceObject(object sourceObject) {
			int rowIndex = dataController.FindRowByRowValue(sourceObject);
			if(rowIndex == DevExpress.Data.DataController.InvalidRow)
				return null;
			return DataSourceAdapter.GetItemByRowIndex(rowIndex);
		}
		public object GetItemSourceObject(MapItem item) {
			return GetSourceObjectByRowIndex(item.RowIndex);
		}
		public int[] GetItemListSourceIndices(object item) {
			IMapDataItem dataItem = item as IMapDataItem;
			return dataItem != null ? dataItem.ListSourceRowIndices : new int[0];
		}
		internal bool GetIsDataReady() {
			if(IsDataSourceListEmpty())
				return true;
			return IsDataLoaded;
		}
	}
}
