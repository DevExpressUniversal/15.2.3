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
using DevExpress.Data.Filtering;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map.Native {
	public class MapDataController : ListSourceDataController, IDataControllerData, IDataControllerData2, IMapDataController {
		readonly BindingBehavior bindingBehavior;
		readonly DataSourceAdapterBase dataAdapter;
		readonly MappingDictionary mappings;
		MapItemAttributeMappingCollection attributeMappings;
		string argument = string.Empty;
		bool shouldRePopulateColumns;
		DataSourceAdapterBase DataAdapter { get { return dataAdapter; } }
		string DataMember { get { return DataAdapter.DataMember; } }
		MapVectorItemCollection Items { get { return DataAdapter.ItemsCollection; } }
		public MappingDictionary Mappings { get { return mappings; } }
		public MapItemAttributeMappingCollection AttributeMappings { get { return DataAdapter.AttributeMappings; } }
		public string Argument { get { return argument; } }
		public object DataSource { get { return bindingBehavior.ActualDataSource; } set { bindingBehavior.UpdateActualDataSource(value); } }
		public MapDataController(DataSourceAdapterBase dataAdapter) {
			this.mappings = new MappingDictionary();
			this.attributeMappings = new MapItemAttributeMappingCollection();
			this.dataAdapter = dataAdapter;
			this.bindingBehavior = new BindingBehavior();
			this.bindingBehavior.ActualDataSourceChanged += OnActualDataSourceChanged;
			DataClient = this;
		}
		void OnActualDataSourceChanged() {
			DataAdapter.LoadDataInternal();
		}
		object ReadRowAttributeValue(IMapDataItem item, int rowIndex, MapItemAttributeMapping mapping) {
			if(string.IsNullOrEmpty(mapping.Name) || string.IsNullOrEmpty(mapping.Member))
				return null;
			return GetRowValue(rowIndex, mapping.Member);
		}
		void LoadObjectAttributes(IMapDataItem item, int rowIndex, MapItemAttributeMapping mapping) {
			if(string.IsNullOrEmpty(mapping.Name) || string.IsNullOrEmpty(mapping.Member))
				return;
			object attrValue = GetRowValue(rowIndex, mapping.Member);
			Type valueType = attrValue != null ? attrValue.GetType() : null;
			item.AddAttribute(new MapItemAttribute() { Name = mapping.Name, Type = valueType, Value = attrValue });
		}
		void LoadObjectProperty(IMapDataItem item, int rowIndex, MapItemMappingBase mapping) {
			object propertyValue = GetRowValue(rowIndex, mapping.Member);
			mapping.SetValue(item, propertyValue);
		}
		void RePopulateColumnsCore() {
			if(shouldRePopulateColumns) {
				RePopulateColumns();
				shouldRePopulateColumns = false;
			}
		}
		void EnsureListSourceAssigned() {
			IList list = ObtainIListFromDataSource(DataSource, DataMember);
			if(!Object.Equals(ListSource, list) || shouldRePopulateColumns)
				ListSource = list;
		}
		void ClearItems() {
			DataAdapter.Clear();
		}
		void UpdateMappings() {
			Mappings.Clear();
			DataAdapter.FillActualMappings(Mappings);
		}
		void ApplyMappings(int rowIndex, IMapDataItem item, MappingDictionary mappings) {
			foreach(MapItemMappingBase mapping in mappings.Values)
				LoadObjectProperty(item, rowIndex, mapping);
		}
		void ApplyAttributeMappings(int rowIndex, IMapDataItem item, MapItemAttributeMappingCollection attributeMappings) {
			foreach(MapItemAttributeMapping attrMapping in attributeMappings)
				LoadObjectAttributes(item, rowIndex, attrMapping);
		}
		IList ObtainIListFromDataSource(object dataSource, string dataMember) {
			return DataHelper.GetList(dataSource, dataMember);
		}
		void LoadMapItems(MapVectorItemCollection targetCollection, MapItemSettingsBase itemSettings) {
			IMapDataEnumerator en = DataAdapter.CreateDataEnumerator(this);
			int iterateNumber = 0;
			while(en.MoveNext()) {
				object source = ((IEnumerator)en).Current;
				MapItem item = CreateAndCustomizeMapItem(source, itemSettings);
				en.Accept(item, iterateNumber++);
				targetCollection.Add(item);
			}
		}
		MapItem CreateAndCustomizeMapItem(object source, MapItemSettingsBase itemSettings) {
			MapItem item = source is MapItem ? (MapItem)source : CreateMapItem(source, itemSettings);
			RaiseCustomizeMapItem(item, source);
			return item;
		}
		MapItem CreateMapItem(object source, MapItemSettingsBase itemSettings) {
			MapItem mapItem = itemSettings.CreateItem();
			itemSettings.ApplySource(mapItem, source);
			mapItem.Tag = source;
			return mapItem;
		}
		void RaiseCustomizeMapItem(MapItem mapItem, object sourceObject) {
			DataAdapter.RaiseCustomizeMapItem(mapItem, sourceObject);
		}
		bool IsItemsChanged(ListChangedEventArgs e) {
			return DataAdapter.CanUpdateItemsOnly && e.PropertyDescriptor == null;
		}
		object GetSourceObjectByRowIndex(int rowIndex) {
			return (rowIndex != MapItem.UndefinedRowIndex) ? GetRow(rowIndex) : null;
		}
		IList<object> CreateComplexAttributeValues(IMapDataItem item, int[] rowIndices, MapItemAttributeMapping mapping) {
			int count = rowIndices.Length;
			object[] values = new object[count];
			for(int i = 0; i < rowIndices.Length; i++)
				values[i] = ReadRowAttributeValue(item, rowIndices[i], mapping);
			return values;
		}
		void OnItemsChanged(ListChangedEventArgs e) {
			switch(e.ListChangedType) {
				case ListChangedType.ItemAdded: {
						object source = GetListSourceRow(e.NewIndex);
						MapItem mapItem = CreateAndCustomizeMapItem(source, DataAdapter.ActualItemSettings);
						LoadItemProperties(mapItem, e.NewIndex);
						Items.Insert(e.NewIndex, mapItem);
					}
					break;
				case ListChangedType.ItemMoved:
					Items.Move(e.OldIndex, e.NewIndex);
					break;
				case ListChangedType.ItemDeleted:
					Items.RemoveAt(e.OldIndex);
					break;
				case ListChangedType.ItemChanged: {
						object source = GetListSourceRow(e.NewIndex);
						MapItem mapItem = CreateAndCustomizeMapItem(source, DataAdapter.ActualItemSettings);
						LoadItemProperties(mapItem, e.NewIndex);
						Items[e.NewIndex] = mapItem;
					}
					break;
				case ListChangedType.Reset:
				default:
					Items.Clear();
					DataAdapter.LoadDataInternal(); 
					break;
			}
		}
		protected override void OnBindingListChanged(ListChangedEventArgs e) {
			base.OnBindingListChanged(e);
			if(IsItemsChanged(e))
				OnItemsChanged(e);
			else
				DataAdapter.LoadDataInternal();
		}
		#region IDataControllerData2 Members
		void AddComplexColumnInfo(ComplexColumnInfoCollection collection, string item) {
			if(item.Contains(".") && Columns[item] == null && collection.IndexOf(item) < 0)
				collection.Add(item);
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection collection = new ComplexColumnInfoCollection();
			foreach(MapItemMappingBase mapping in Mappings.Values) {
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
		public void OnMappingsChanged() {
			shouldRePopulateColumns = true;
			DataAdapter.LoadDataInternal();
		}
		public void SetItemVisibleRowIndex(IMapDataItem item, int rowIndex, int[] listSourceIndices) {
			item.RowIndex = rowIndex;
			item.ListSourceRowIndices = listSourceIndices;
		}
		public void LoadItemProperties(IMapDataItem item, int rowIndex) {
			ApplyMappings(rowIndex, item, Mappings);
			ApplyAttributeMappings(rowIndex, item, AttributeMappings);
		}
		public void LoadData() {
			Items.BeginUpdate(DataAdapter);
			if(DataSource != null) {
				UpdateMappings();
				RePopulateColumnsCore();
				EnsureListSourceAssigned();
				ClearItems();
				DataAdapter.Aggregate(this);
				LoadMapItems(Items, DataAdapter.ActualItemSettings);
			}
			else
				ClearItems();
			DataAdapter.OnDataLoaded();
			Items.EndUpdate();
		}
		public object GetItemSourceObject(MapItem item) {
			return GetSourceObjectByRowIndex(item.RowIndex);
		}
		public int[] GetItemListSourceIndices(object item) {
			IMapDataItem dataItem = item as IMapDataItem;
			return dataItem != null ? dataItem.ListSourceRowIndices : new int[0];
		}
		public void SetMapItemValueSummary(IMapDataItem item, object summary) {
			MapItemMappingBase mapping = Mappings[MappingType.Value];
			if(item != null && mapping != null) mapping.SetValue(item, summary);
		}
		public void LoadItemAttributeArray(IMapDataItem item, int[] rowIndices) {
			foreach(MapItemAttributeMapping mapping in AttributeMappings) {
				IList<object> values = CreateComplexAttributeValues(item, rowIndices, mapping);
				Type valueType = values != null ? values.GetType() : null;
				item.AddAttribute(new MapItemAttribute() { Name = mapping.Name, Type = valueType, Value = values });
			}
		}
		public StringCollection GetColumnNames() {
			EnsureListSourceAssigned();
			return DataHelper.GetColumnNames(ListSource);
		}
		public MapItem GetMapItemBySourceObject(object sourceObject) {
			int rowIndex = FindRowByRowValue(sourceObject);
			if(rowIndex == DataController.InvalidRow)
				return null;
			return DataAdapter.GetItemByRowIndex(rowIndex);
		}
	}
}
