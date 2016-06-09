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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.Utils.Serializing.Helpers;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.PivotGrid.Internal.ThinClientDataSource {
	class CollapsedCache {
		readonly Dictionary<int, IList<FieldValueItem>> cacheLevels = new Dictionary<int, IList<FieldValueItem>>();
		readonly Dictionary<FieldValueItem, IList<FieldValueItem>> cache = new Dictionary<FieldValueItem, IList<FieldValueItem>>();
		public IList<FieldValueItem> this[FieldValueItem key] {
			get { return cache[key]; }
		}
		public bool ContainsKey(FieldValueItem key) {
			return cache.ContainsKey(key);
		}
		public bool TryGetValue(FieldValueItem key, out IList<FieldValueItem> value) {
			return cache.TryGetValue(key, out value);
		}
		public void Remove(int level) {
			foreach(KeyValuePair<int, IList<FieldValueItem>> pair in cacheLevels) {
				if(level <= pair.Key)
					continue;
				List<FieldValueItem> fieldValues = pair.Value.ToList();
				foreach(FieldValueItem cache in fieldValues) {
					cache.Children = null;
					Remove(cache);
				}
			}
		}
		public void Remove(FieldValueItem fieldValue) {
			cache.Remove(fieldValue);
			if(cacheLevels.ContainsKey(fieldValue.Level))
				cacheLevels[fieldValue.Level].Remove(fieldValue);
		}
		public void Add(FieldValueItem fieldValue) {
			if(!cache.ContainsKey(fieldValue))
				cache.Add(fieldValue, fieldValue.Children);
			if(cacheLevels.ContainsKey(fieldValue.Level))
				cacheLevels[fieldValue.Level].Add(fieldValue);
			else
				cacheLevels.Add(fieldValue.Level, new List<FieldValueItem> { fieldValue });
		}
		public IList<FieldValueItem> Clear() {
			List<FieldValueItem> allCache = cache.SelectMany(c => c.Value).ToList();
			List<FieldValueItem> result = new List<FieldValueItem>();
			allCache.ForEach(valueItem => Calculate(valueItem, result));
			cache.Clear();
			cacheLevels.Clear();
			return result;
		}
		void Calculate(FieldValueItem current, IList<FieldValueItem> valueItems) {
			valueItems.Add(current);
			if(current.Children != null) {
				foreach(FieldValueItem child in current.Children)
					Calculate(child, valueItems);
			}
		}
	}
	public abstract class PivotThinClientDataSourcePropertyDescriptor : PropertyDescriptor {
		public override Type ComponentType { get { return typeof(object); } }
		public override bool IsReadOnly { get { return true; } }
		bool IsColumn { get; set; }
		protected PivotThinClientDataSourcePropertyDescriptor(bool isColumn, string name)
			: base(String.Format("{0}{1}", isColumn ? "Column" : "Row", name), new Attribute[0]) {
				this.IsColumn = isColumn;
		}
		protected FieldValueItem GetItem(object component) {
			PivotDrillDownDataRow dsRow = (PivotDrillDownDataRow)component;
			PivotThinDrillDownDataSource ds = (PivotThinDrillDownDataSource)(dsRow.DataSource);
			return IsColumn ? ds.ColumnItem : ds.RowItem;
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override void SetValue(object component, object value) {
			throw new NotSupportedException();
		}
		public override void ResetValue(object component) {
			throw new NotSupportedException();
		}
	}
	public class PivotThinClientDataSourceValuePropertyDescriptor : PivotThinClientDataSourcePropertyDescriptor {
		public PivotThinClientDataSourceValuePropertyDescriptor(bool isColumn)
			: base(isColumn, "Value") {
		}
		public override object GetValue(object component) {
			FieldValueItem item = GetItem(component);
			return item != null ? item.Value.Value : null;
		}
		public override Type PropertyType { get { return typeof(object); } }
	}
	public class PivotThinClientDataSourceDisplayTextPropertyDescriptor : PivotThinClientDataSourcePropertyDescriptor {
		public PivotThinClientDataSourceDisplayTextPropertyDescriptor(bool isColumn)
			: base(isColumn, "DisplayText") {
		}
		public override object GetValue(object component) {
			FieldValueItem item = GetItem(component);
			return item != null ? item.Value.DisplayText : null;
		}
		public override Type PropertyType { get { return typeof(string); } }
	}
	public class PivotThinClientDataSourceTagPropertyDescriptor : PivotThinClientDataSourcePropertyDescriptor {
		public PivotThinClientDataSourceTagPropertyDescriptor(bool isColumn)
			: base(isColumn, "Tag") {
		}
		public override object GetValue(object component) {
			FieldValueItem item = GetItem(component);
			return item != null ? item.Value.Tag : null;
		}
		public override Type PropertyType { get { return typeof(object); } }
	}
	public class PivotThinDrillDownDataSource : PivotDrillDownDataSource {
		public FieldValueItem ColumnItem { get; private set; }
		public FieldValueItem RowItem { get; private set; }
		protected override int RowCountInternal {
			get { return 1; }
		}
		internal PivotThinDrillDownDataSource(FieldValueItem columnItem, FieldValueItem rowItem) {
			this.ColumnItem = columnItem;
			this.RowItem = rowItem;
		}
		protected internal override PropertyDescriptorCollection GetDescriptorCollection() {
			PropertyDescriptor[] descriptors = new PropertyDescriptor[] { 
				new PivotThinClientDataSourceValuePropertyDescriptor(true), 
				new PivotThinClientDataSourceDisplayTextPropertyDescriptor(true),
				new PivotThinClientDataSourceTagPropertyDescriptor(true),
				new PivotThinClientDataSourceValuePropertyDescriptor(false), 
				new PivotThinClientDataSourceDisplayTextPropertyDescriptor(false),
				new PivotThinClientDataSourceTagPropertyDescriptor(false)
			};
			return new PropertyDescriptorCollection(descriptors);
		}
		public override object GetValue(int rowIndex, PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		public override object GetValue(int rowIndex, int columnIndex) {
			return this;
		}
		public override object GetValue(int rowIndex, string fieldName) {
			throw new NotImplementedException();
		}
		internal override int GetListSourceRowIndex(int rowIndex) {
			throw new NotImplementedException();
		}
		public override void SetValue(int rowIndex, PivotGridFieldBase field, object value) {
			throw new NotImplementedException();
		}
		public override void SetValue(int rowIndex, int columnIndex, object value) {
			throw new NotImplementedException();
		}
		public override void SetValue(int rowIndex, string fieldName, object value) {
			throw new NotImplementedException();
		}
	}
	public class PivotGridThinClientDataSource : IPivotGridDataSource, IPivotGridDataSourceAsyncExpand {
		readonly Dictionary<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>> data;
		readonly Dictionary<FieldValueModelKey, FieldValueItem> columnValues = new Dictionary<FieldValueModelKey, FieldValueItem>();
		readonly Dictionary<FieldValueModelKey, FieldValueItem> rowValues = new Dictionary<FieldValueModelKey, FieldValueItem>();
		readonly CollapsedCache columnCollapsedCache = new CollapsedCache();
		readonly CollapsedCache rowCollapsedCache = new CollapsedCache();
		IList<FieldValueItem> columnData;
		IList<FieldValueItem> rowData;
		IPivotGridDataSourceOwner owner;
		bool autoExpandGroups;
		ICustomObjectConverter customObjectConverter;
		int columnCount;
		int rowCount;
		event EventHandler<ExpandValueRequestedEventArgs> expandValueRequested;
		event EventHandler<ExpandLevelRequestedEventArgs> expandLevelRequested;
		event EventHandler<CollapseValueRequestedEventArgs> collapseValueRequested;
		event EventHandler<CollapseLevelRequestedEventArgs> collapseLevelRequested;
		public event EventHandler<ExpandValueRequestedEventArgs> ExpandValueRequested {
			add { expandValueRequested += value; }
			remove { expandValueRequested -= value; }
		}
		public event EventHandler<ExpandLevelRequestedEventArgs> ExpandLevelRequested {
			add { expandLevelRequested += value; }
			remove { expandLevelRequested -= value; }
		}
		public event EventHandler<CollapseValueRequestedEventArgs> CollapseValueRequested {
			add { collapseValueRequested += value; }
			remove { collapseValueRequested -= value; }
		}
		public event EventHandler<CollapseLevelRequestedEventArgs> CollapseLevelRequested {
			add { collapseLevelRequested += value; }
			remove { collapseLevelRequested -= value; }
		}
		Dictionary<FieldValueModelKey, FieldValueItem> GetValues(bool isColumn) {
			return isColumn ? columnValues : rowValues;
		}
		int GetValuesCount(bool isColumn) {
			return isColumn ? columnCount : rowCount;
		}
		CollapsedCache GetCollapsedCache(bool isColumn) {
			return isColumn ? columnCollapsedCache : rowCollapsedCache;
		}
		public PivotGridThinClientDataSource(PivotGridThinClientData data) {
			DataTransformer transformer = new DataTransformer(data);
			this.columnData = transformer.ColumnData;
			this.rowData = transformer.RowData;
			this.data = transformer.Data;
			FillFieldValues();
		}
		void FillFieldValues() {
			columnCount = FillFieldValues(columnValues, columnData) + 1;
			rowCount = FillFieldValues(rowValues, rowData) + 1;
		}
		int FillFieldValues(Dictionary<FieldValueModelKey, FieldValueItem> fieldValues, IList<FieldValueItem> data) {
			fieldValues.Clear();
			int visibleIndex = 0;
			if(data == null)
				return visibleIndex;
			List<FieldValueItem> parentValues = new List<FieldValueItem>();
			for(int i = 0; i < data.Count; i++) {
				FillFieldValues(fieldValues, ref visibleIndex, data[i], parentValues);
				visibleIndex++;
			}
			return visibleIndex;
		}
		void FillFieldValues(Dictionary<FieldValueModelKey, FieldValueItem> fieldValues, ref int visibleIndex, FieldValueItem column,
			List<FieldValueItem> parentValues) {
			fieldValues.Add(new FieldValueModelKey(column.Level, visibleIndex), column);
			parentValues.Add(column);
			if(column.Children != null) {
				for(int i = 0; i < column.Children.Count; i++) {
					visibleIndex++;
					for(int j = 0; j < parentValues.Count; j++)
						fieldValues.Add(new FieldValueModelKey(j, visibleIndex), parentValues[j]);
					FillFieldValues(fieldValues, ref visibleIndex, column.Children[i], parentValues);
				}
			}
			parentValues.RemoveAt(parentValues.Count - 1);
		}
		FieldValueItem GetFromValues(Dictionary<FieldValueModelKey, FieldValueItem> values, int visibleIndex) {
			FieldValueItem result = null;
			int level = 0;
			while(values.ContainsKey(new FieldValueModelKey(level, visibleIndex))) {
				result = values[new FieldValueModelKey(level, visibleIndex)];
				level++;
			}
			return result;
		}
		IList<FieldValueItem> GetFieldValueItemsByLevelIndex(bool isColumn, int level) {
			Dictionary<FieldValueModelKey, FieldValueItem> values = GetValues(isColumn);
			List<FieldValueItem> result = new List<FieldValueItem>();
			int maxCount = GetValuesCount(isColumn);
			for(int visibleIndex = 0; visibleIndex < maxCount; visibleIndex++) {
				FieldValueItem cellKey;
				if(!values.TryGetValue(new FieldValueModelKey(level, visibleIndex), out cellKey))
					continue;
				if(!result.Contains(cellKey))
					result.Add(cellKey);
			}
			return result;
		}
		object[] GetValuesByVisibleIndex(bool isColumn, int visibleIndex) {
			List<object> list = new List<object>();
			int level = 0;
			object value;
			while((value = (this as IPivotGridDataSource).GetFieldValue(isColumn, visibleIndex, level)) != null) {
				list.Add(value);
				level++;
			}
			return list.ToArray();
		}
		void FillData(IDictionary<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>> subData) {
			foreach(KeyValuePair<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>> dictPair in subData) {
				foreach(KeyValuePair<int, ThinClientValueItem> valuePair in dictPair.Value) {
					FieldValueItem column = dictPair.Key.First;
					FieldValueItem row = dictPair.Key.Second;
					int dataIndex = valuePair.Key;
					ThinClientValueItem cellValue = valuePair.Value;
					PairReferenceKey<FieldValueItem, FieldValueItem> key = new PairReferenceKey<FieldValueItem, FieldValueItem>(column, row);
					Dictionary<int, ThinClientValueItem> dict;
					if(!data.TryGetValue(key, out dict))
						dict = new Dictionary<int, ThinClientValueItem>();
					dict[dataIndex] = cellValue;
					data[key] = dict;
				}
			}
		}
		void AddToCollapsedCache(FieldValueItem fieldValue, bool isColumn) {
			CollapsedCache cache = GetCollapsedCache(isColumn);
			cache.Add(fieldValue);
			fieldValue.Children = null;
		}
		void ClearCollapsedCache(bool isColumn) {
			CollapsedCache crossCache = GetCollapsedCache(isColumn);
			IList<FieldValueItem> crossCacheFieldValues = crossCache.Clear();
			PairReferenceKey<FieldValueItem, FieldValueItem>[] keysToRemove = data.Keys
				.Where(k => isColumn && crossCacheFieldValues.Contains(k.First) || !isColumn && crossCacheFieldValues.Contains(k.Second)).ToArray();
			foreach(PairReferenceKey<FieldValueItem, FieldValueItem> key in keysToRemove)
				data.Remove(key);
		}
		bool IPivotGridDataSource.AutoExpandGroups {
			get { return autoExpandGroups; }
			set { autoExpandGroups = value; }
		}
		PivotDataSourceCaps IPivotGridDataSource.Capabilities {
			get { return PivotDataSourceCaps.DenyExpandValuesAllowed; }
		}
		bool IPivotGridDataSource.ChangeExpanded(bool isColumn, int visibleIndex, bool expanded) {
			Dictionary<FieldValueModelKey, FieldValueItem> fieldValues = GetValues(isColumn);
			FieldValueItem cellItem = GetFromValues(fieldValues, visibleIndex);
			object[] values = GetValuesByVisibleIndex(isColumn, visibleIndex);
			CollapsedCache cache = GetCollapsedCache(isColumn);
			if(expanded) {
				IList<FieldValueItem> childs;
				bool isFromCache = GetCollapsedCache(isColumn).TryGetValue(cellItem, out childs);
				if(isFromCache) {
					cellItem.Children = childs;
					cache.Remove(cellItem);
				}
				bool isDataRequired = !isFromCache;
				if(expandValueRequested != null) {
					ExpandValueRequestedEventArgs eventArgs = new ExpandValueRequestedEventArgs(isColumn, values, isDataRequired);
					expandValueRequested(this, eventArgs);
				}
				if(!isDataRequired)
					FillFieldValues();
			} else {
				AddToCollapsedCache(cellItem, isColumn);
				if(collapseValueRequested != null) {
					CollapseValueRequestedEventArgs collapseEventArgs = new CollapseValueRequestedEventArgs(isColumn, values);
					collapseValueRequested(this, collapseEventArgs);
				}
				FillFieldValues();
			}
			return true;
		}
		public void ProcessExpandValue(ExpandValueRequestedEventArgs eventArgs) {
			if(!eventArgs.Result)
				return;
			if(eventArgs.IsDataRequired) {
				Dictionary<FieldValueModelKey, FieldValueItem> fieldValues = GetValues(eventArgs.IsColumn);
				int visibleIndex = GetVisibleIndexByValues(eventArgs.IsColumn, eventArgs.Values);
				FieldValueItem cellItem = GetFromValues(fieldValues, visibleIndex);
				int level = eventArgs.Values != null ? eventArgs.Values.Length : 0;
				IList<FieldValueItem> crossValues = eventArgs.IsColumn ? rowData : columnData;
				EventArgsDataTransformer transformer = new EventArgsDataTransformer(eventArgs, level, crossValues);
				if(transformer.ExpandHierarchy.Count == 0) {
					RaiseExpandValueDenied(eventArgs.IsColumn, visibleIndex, cellItem.Level);
					RaiseLayoutChanged();
					return;
				}
				cellItem.Children = transformer.ExpandHierarchy;
				FillData(transformer.Data);
			}
			ClearCollapsedCache(!eventArgs.IsColumn);
			FillFieldValues();
			RaiseLayoutChanged();
		}
		bool IPivotGridDataSource.ChangeExpandedAll(bool isColumn, bool expanded) {
			return true;
		}
		bool IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			throw new NotImplementedException();
		}
		bool IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			bool isColumn = field.Area == PivotArea.ColumnArea;
			int level = field.AreaIndex;
			IList<FieldValueItem> cellItems = GetFieldValueItemsByLevelIndex(isColumn, level);
			CollapsedCache cache = GetCollapsedCache(isColumn);
			if(expanded) {
				bool isFromCache = cellItems.All(cell => GetCollapsedCache(isColumn).ContainsKey(cell));
				if(isFromCache) {
					foreach(FieldValueItem cellItem in cellItems) {
						cellItem.Children = GetCollapsedCache(isColumn)[cellItem];
						cache.Remove(cellItem);
					}
				}
				if(expandLevelRequested != null) {
					ExpandLevelRequestedEventArgs eventArgs = new ExpandLevelRequestedEventArgs(isColumn, level, !isFromCache);
					expandLevelRequested(this, eventArgs);
					if(!isFromCache) {
						IList<FieldValueItem> crossValues = isColumn ? rowData : columnData;
						EventArgsDataTransformer transformer = new EventArgsDataTransformer(eventArgs, eventArgs.Level, crossValues);
						if(eventArgs.IsColumn)
							columnData = transformer.ExpandHierarchy;
						else
							rowData = transformer.ExpandHierarchy;
						FillData(transformer.Data);
					}
				}
			}
			else {
				cache.Remove(level);
				foreach(FieldValueItem cellItem in cellItems)
					AddToCollapsedCache(cellItem, isColumn);
				if(collapseLevelRequested != null) {
					CollapseLevelRequestedEventArgs collapseEventArgs = new CollapseLevelRequestedEventArgs(isColumn, level);
					collapseLevelRequested(this, collapseEventArgs);
				}
			}
			FillFieldValues();
			return true;
		}
		bool IPivotGridDataSource.ChangeFieldSortOrder(PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		bool IPivotGridDataSource.ChangeFieldSummaryType(PivotGridFieldBase field, PivotSummaryType oldSummaryType) {
			throw new NotImplementedException();
		}
		ICustomObjectConverter IPivotGridDataSource.CustomObjectConverter {
			get { return customObjectConverter; }
			set { customObjectConverter = value; }
		}
		event EventHandler<PivotDataSourceEventArgs> IPivotGridDataSource.DataChanged {
			add { }
			remove { }
		}
		void IPivotGridDataSource.DoRefresh() {
		}
		object[] IPivotGridDataSource.GetAvailableFieldValues(PivotGridFieldBase field, bool deferUpdates, ICustomFilterColumnsProvider customFilters) {
			throw new NotImplementedException();
		}
		int IPivotGridDataSource.GetCellCount(bool isColumn) {
			return GetValuesCount(isColumn);
		}
		PivotCellValue IPivotGridDataSource.GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			FieldValueItem column = GetFromValues(columnValues, columnIndex);
			FieldValueItem row = GetFromValues(rowValues, rowIndex);
			Dictionary<int, ThinClientValueItem> cellValues = null;
			if(!data.TryGetValue(new PairReferenceKey<FieldValueItem, FieldValueItem>(column, row), out cellValues))
				return null;
			ThinClientValueItem result;
			if(!cellValues.TryGetValue(dataIndex, out result))
				return null;
			return new PivotCellValue(result.Value);
		}
		PivotDrillDownDataSource IPivotGridDataSource.GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount) {
			FieldValueItem columnItem = GetFromValues(GetValues(true), columnIndex);
			FieldValueItem rowItem = GetFromValues(GetValues(false), rowIndex);
			return new PivotThinDrillDownDataSource(columnItem, rowItem);
		}
		string IPivotGridDataSource.GetLocalizedFieldCaption(string fieldName) {
			return null;
		}
		string IPivotGridDataSource.GetFieldCaption(string fieldName) {
			throw new NotImplementedException();
		}
		string[] IPivotGridDataSource.GetFieldList() {
			throw new NotImplementedException();
		}
		Type IPivotGridDataSource.GetFieldType(PivotGridFieldBase field, bool raw) {
			return typeof(object);
		}
		object IPivotGridDataSource.GetFieldValue(bool isColumn, int visibleIndex, int areaIndex) {
			Dictionary<FieldValueModelKey, FieldValueItem> values = GetValues(isColumn);
			FieldValueItem res;
			if(values.TryGetValue(new FieldValueModelKey(areaIndex, visibleIndex), out res))
				return res.Value.Value;
			return null;
		}
		bool IPivotGridDataSource.GetIsEmptyGroupFilter(PivotGridGroup group) {
			throw new NotImplementedException();
		}
		bool IPivotGridDataSource.GetIsOthersFieldValue(bool isColumn, int visibleIndex, int levelIndex) {
			return false;
		}
		int IPivotGridDataSource.GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext) {
			throw new NotImplementedException();
		}
		int IPivotGridDataSource.GetObjectLevel(bool isColumn, int visibleIndex) {
			Dictionary<FieldValueModelKey, FieldValueItem> values = GetValues(isColumn);
			int maxLevel = 0;
			while(values.ContainsKey(new FieldValueModelKey(maxLevel, visibleIndex)))
				maxLevel++;
			return maxLevel - 1;
		}
		List<object> IPivotGridDataSource.GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues) {
			throw new NotImplementedException();
		}
		object[] IPivotGridDataSource.GetSortedUniqueValues(PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		PivotSummaryInterval IPivotGridDataSource.GetSummaryInterval(PivotGridFieldBase dataField, bool visibleValuesOnly, bool customLevel, PivotGridFieldBase rowField, PivotGridFieldBase columnField) {
			throw new NotImplementedException();
		}
		object[] IPivotGridDataSource.GetUniqueFieldValues(PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		List<object> IPivotGridDataSource.GetVisibleFieldValues(PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		int GetVisibleIndexByValues(bool isColumn, object[] values) {
			IPivotGridDataSource iPivot = (IPivotGridDataSource)this;
			if(values == null || values.Length < 1)
				return -1;
			int maxCount = GetValuesCount(isColumn);
			for(int i = 0; i < maxCount; i++) {
				bool equals = true;
				for(int j = 0; j < values.Length; j++) {
					if(!Object.Equals(iPivot.GetFieldValue(isColumn, i, j), values[j])) {
						equals = false;
						break;
					}
				}
				if(equals)
					return i;
			}
			return -1;
		}
		int IPivotGridDataSource.GetVisibleIndexByValues(bool isColumn, object[] values) {
			return GetVisibleIndexByValues(isColumn, values);
		}
		bool IPivotGridDataSource.HasNullValues(PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		bool IPivotGridDataSource.HasNullValues(string dataMember) {
			throw new NotImplementedException();
		}
		void IPivotGridDataSource.HideDataField(PivotGridFieldBase field, int dataIndex) {
			throw new NotImplementedException();
		}
		bool IPivotGridDataSource.IsAreaAllowed(PivotGridFieldBase field, PivotArea area) {
			return true;
		}
		bool IPivotGridDataSource.IsFieldReadOnly(PivotGridFieldBase field) {
			return true;
		}
		bool IPivotGridDataSource.IsFieldTypeCheckRequired(PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		bool? IPivotGridDataSource.IsGroupFilterValueChecked(PivotGridGroup group, object[] parentValues, object value) {
			throw new NotImplementedException();
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, object[] values) {
			throw new NotImplementedException();
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, int visibleIndex) {
			Dictionary<FieldValueModelKey, FieldValueItem> values = GetValues(isColumn);
			FieldValueItem cellItem = GetFromValues(values, visibleIndex);
			return cellItem.Children == null;
		}
		bool IPivotGridDataSource.IsUnboundExpressionValid(PivotGridFieldBase field) {
			throw new NotImplementedException();
		}
		public event EventHandler<PivotDataSourceEventArgs> LayoutChanged;
		public event EventHandler<PivotDataSourceExpandValueDeniedEventArgs> ExpandValueDenied;
		void IPivotGridDataSource.LoadCollapsedStateFromStream(Stream stream) {
		}
		void RaiseLayoutChanged() {
			LayoutChanged.Invoke(this, new PivotDataSourceEventArgs(this));
		}
		void RaiseExpandValueDenied(bool isColumn, int visibleIndex, int level) {
			ExpandValueDenied.Invoke(this, new PivotDataSourceExpandValueDeniedEventArgs(isColumn, visibleIndex, level));
		}
		void IPivotGridDataSource.MoveDataField(PivotGridFieldBase field, int oldIndex, int newIndex) {
			throw new NotImplementedException();
		}
		void IPivotGridDataSource.OnInitialized() {
			PivotGridFieldReadOnlyCollection sortedFields = owner.GetSortedFields();
			for(int i = 0; i < sortedFields.Count; i++)
				sortedFields[i].SetColumnHandle(i);
		}
		IPivotGridDataSourceOwner IPivotGridDataSource.Owner {
			get { return owner; }
			set { owner = value; }
		}
		void IPivotGridDataSource.ReloadData() {
		}
		void IPivotGridDataSource.RetrieveFields(PivotArea area, bool visible) {
			throw new NotImplementedException();
		}
		void IPivotGridDataSource.SaveCollapsedStateToStream(Stream stream) {
		}
		void IPivotGridDataSource.SaveDataToStream(Stream stream, bool compressed) {
			throw new NotImplementedException();
		}
		void IPivotGridDataSource.SetAutoExpandGroups(bool value, bool reloadData) {
			throw new NotImplementedException();
		}
		bool IPivotGridDataSource.ShouldCalculateRunningSummary {
			get { throw new NotImplementedException(); }
		}
		void IPivotGridDataSource.WebLoadCollapsedStateFromStream(Stream stream) {
			throw new NotImplementedException();
		}
		void IPivotGridDataSource.WebSaveCollapsedStateToStream(Stream stream) {
			throw new NotImplementedException();
		}
		void IDisposable.Dispose() {
		}
	}
	class FieldValueModelKey {
		readonly int levelIndex;
		readonly int visibleIndex;
		public FieldValueModelKey(int levelIndex, int visibleIndex) {
			this.levelIndex = levelIndex;
			this.visibleIndex = visibleIndex;
		}
		public override bool Equals(object obj) {
			FieldValueModelKey key = obj as FieldValueModelKey;
			return key != null && key.levelIndex == levelIndex && key.visibleIndex == visibleIndex;
		}
		public override int GetHashCode() {
			int columnHash = levelIndex.GetHashCode();
			int rowHash = visibleIndex.GetHashCode();
			return columnHash ^ rowHash;
		}
		public override string ToString() {
			return String.Format("levelIndex:{0};visibleIndex:{1}", levelIndex, visibleIndex);
		}
	}
	abstract class DataTransformerBase {
		protected void CalculateTransforms(ThinClientFieldValueItem current, Dictionary<ThinClientFieldValueItem, FieldValueItem> transformes) {
			if(current.Children != null) {
				for(int i = 0; i < current.Children.Count; i++) {
					transformes[current.Children[i]] = transformes[current].Children[i];
					CalculateTransforms(current.Children[i], transformes);
				}
			}
		}
		protected FieldValueItem Transform(ThinClientFieldValueItem valueItem, int level) {
			return new FieldValueItem(valueItem.Value, valueItem.TotalDisplayText, level) {
				Children = valueItem.Children != null ? valueItem.Children.Select(c => Transform(c, level + 1)).ToList() : null
			};
		}
	}
	class DataTransformer : DataTransformerBase {
		readonly PivotGridThinClientData thinClientData;
		IList<FieldValueItem> columnData;
		IList<FieldValueItem> rowData;
		Dictionary<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>> data;
		public IList<FieldValueItem> ColumnData { get { return columnData; } }
		public IList<FieldValueItem> RowData { get { return rowData; } }
		public Dictionary<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>> Data { get { return data; } }
		public DataTransformer(PivotGridThinClientData thinClientData) {
			this.thinClientData = thinClientData;
			TransformData();
		}
		void TransformData() {
			Dictionary<ThinClientFieldValueItem, FieldValueItem> columnTransforms = CalculateTransforms(thinClientData.ColumnFieldValues);
			Dictionary<ThinClientFieldValueItem, FieldValueItem> rowTransforms = CalculateTransforms(thinClientData.RowFieldValues);
			columnData = thinClientData.ColumnFieldValues != null ? thinClientData.ColumnFieldValues.Select(c => columnTransforms[c]).ToList() : null;
			rowData = thinClientData.RowFieldValues != null ? thinClientData.RowFieldValues.Select(r => rowTransforms[r]).ToList() : null;
			data = new Dictionary<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>>();
			foreach(ThinClientFieldValueItem column in columnTransforms.Keys.Union(new ThinClientFieldValueItem[] { null })) {
				foreach(ThinClientFieldValueItem row in rowTransforms.Keys.Union(new ThinClientFieldValueItem[] { null })) {
					Dictionary<int, ThinClientValueItem> cellSet;
					if(thinClientData.Data.TryGetValue(new PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem>(column, row), out cellSet)) {
						foreach(KeyValuePair<int, ThinClientValueItem> pair in cellSet) {
							FieldValueItem columnTransform = column != null ? columnTransforms[column] : null;
							FieldValueItem rowTransform = row != null ? rowTransforms[row] : null;
							int dataIndex = pair.Key;
							ThinClientValueItem cellValue = pair.Value;
							PairReferenceKey<FieldValueItem, FieldValueItem> key = new PairReferenceKey<FieldValueItem, FieldValueItem>(columnTransform, rowTransform);
							Dictionary<int, ThinClientValueItem> cellValues;
							if(!data.TryGetValue(key, out cellValues))
								cellValues = new Dictionary<int, ThinClientValueItem>();
							cellValues[dataIndex] = cellValue;
							data[key] = cellValues;
						}
					}
				}
			}
		}
		Dictionary<ThinClientFieldValueItem, FieldValueItem> CalculateTransforms(IList<ThinClientFieldValueItem> fieldValueItems) {
			Dictionary<ThinClientFieldValueItem, FieldValueItem> result = new Dictionary<ThinClientFieldValueItem, FieldValueItem>();
			if(fieldValueItems != null) {
				foreach(ThinClientFieldValueItem item in fieldValueItems) {
					result[item] = Transform(item, 0);
					CalculateTransforms(item, result);
				}
			}
			return result;
		}
	}
	class EventArgsDataTransformer : DataTransformerBase {
		readonly ExpandRequestedEventArgsBase eventArgs;
		readonly int level;
		readonly IList<FieldValueItem> crossHierarchy;
		IList<FieldValueItem> expandHierarchy;
		IDictionary<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>> data;
		public IList<FieldValueItem> ExpandHierarchy { get { return expandHierarchy; } }
		public IDictionary<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>> Data { get { return data; } }
		public EventArgsDataTransformer(ExpandRequestedEventArgsBase eventArgs, int level, IList<FieldValueItem> crossHierarchy) {
			this.eventArgs = eventArgs;
			this.level = level;
			this.crossHierarchy = crossHierarchy;
			TransformData();
		}
		void TransformData() {
			Dictionary<ThinClientFieldValueItem, FieldValueItem> expandCache = new Dictionary<ThinClientFieldValueItem, FieldValueItem>();
			expandHierarchy = eventArgs.ExpandHierarchy.Select(h => Transform(h, level)).ToList();
			for(int i = 0; i < eventArgs.ExpandHierarchy.Count; i++) {
				expandCache[eventArgs.ExpandHierarchy[i]] = expandHierarchy[i];
				CalculateTransforms(eventArgs.ExpandHierarchy[i], expandCache);
			}
			data = new Dictionary<PairReferenceKey<FieldValueItem, FieldValueItem>, Dictionary<int, ThinClientValueItem>>();
			foreach(KeyValuePair<int, Dictionary<PairReferenceKey<ThinClientFieldValueItem, object[]>, ThinClientValueItem>> pair in eventArgs.Data) {
				int dataIndex = pair.Key;
				foreach(KeyValuePair<PairReferenceKey<ThinClientFieldValueItem, object[]>, ThinClientValueItem> subPair in pair.Value) {
					FieldValueItem expand = subPair.Key.First != null ? expandCache[subPair.Key.First] : null;
					FieldValueItem cross = GetFieldValueItemByPath(subPair.Key.Second);
					PairReferenceKey<FieldValueItem, FieldValueItem> cloneCellKey = eventArgs.IsColumn ? new PairReferenceKey<FieldValueItem, FieldValueItem>(expand, cross) :
						new PairReferenceKey<FieldValueItem, FieldValueItem>(cross, expand);
					if(!data.ContainsKey(cloneCellKey))
						data[cloneCellKey] = new Dictionary<int, ThinClientValueItem>();
					data[cloneCellKey][dataIndex] = subPair.Value;
				}
			}
		}
		FieldValueItem GetFieldValueItemByPath(object[] values) {
			if(values == null)
				return null;
			FieldValueItem result = null;
			IList<FieldValueItem> childs = crossHierarchy;
			foreach(object o in values) {
				if(childs == null)
					return null;
				result = childs.FirstOrDefault(ch => Object.Equals(ch.Value.Value, o));
				if(result == null)
					return null;
				childs = result.Children;
			}
			return result;
		}
	}
}
