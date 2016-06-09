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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.DashboardCommon {
	public abstract class DataItemContainer : IDataItemHolder, IChangeService, IDataItemContext {
		readonly List<string> dataItemKeys = new List<string>();
		readonly List<string> dataItemCaptions = new List<string>();
		readonly List<string> dataItemCaptionsPlural = new List<string>();
		readonly Dictionary<string, DataItem> dataItems = new Dictionary<string, DataItem>();
		Dictionary<string, string> dataItemsNames;
		IDataItemContainerContext context;
		protected internal IDataItemContainerContext Context {
			get { return context; }
			set {
				context = value;
				OnDataItemContainerContextChanged();
			}
		}
		protected internal virtual int DataItemGroupIndex { get { return -1; } }
		internal IEnumerable<string> DataItemKeys { get { return dataItemKeys; } }
		internal IEnumerable<string> DataItemsCaptions { get { return dataItemCaptions; } }
		internal IEnumerable<string> DataItemsCaptionsPlural { get { return dataItemCaptionsPlural; } }
		internal int DataItemsCount { get { return dataItems.Count; } }
		internal bool HasDataItems { get { return DataItemsCount > 0; } }
		internal bool HasAllDataItems { get { return HasDataItems && DataItemsCount == dataItemKeys.Count; } }
		internal IEnumerable<Measure> Measures {
			get {
				foreach (Measure measure in dataItems.Values.OfType<Measure>())
					yield return measure;
			}
		}
		protected internal virtual IEnumerable<DataItem> DataItems {
			get {
				foreach(DataItem dataItem in dataItems.Values)
					yield return dataItem;
			}
		}
		protected IDataSourceSchema DataSource { 
			get {
				IDataSourceSchemaProvider dataSourceProvider = Context != null ? Context.DataSourceSchemaProvider : null;
				return dataSourceProvider != null ? dataSourceProvider.DataSourceSchema : null;
			} 
		}
		DataItemRepository DataItemRepository { 
			get {
				IDataItemRepositoryProvider repositoryProvider = Context != null ? Context.DataItemRepositoryProvider : null;
				return repositoryProvider != null ? repositoryProvider.DataItemRepository : null;
			} 
		}
		DataItem IDataItemHolder.this[string key] { get { return GetDataItem(key); } set { SetDataItem(key, value); } }
		int IDataItemHolder.GroupIndex { get { return DataItemGroupIndex; } }
		int IDataItemHolder.Count { get { return DataItemsCount; } }
		IEnumerable<string> IDataItemHolder.Captions { get { return DataItemsCaptions; } }
		IEnumerable<string> IDataItemHolder.CaptionsPlural { get { return DataItemsCaptionsPlural; } }
		IEnumerable<string> IDataItemHolder.Keys { get { return DataItemKeys; } }
		IChangeService ChangeService { get { return Context != null ? Context.ChangeService : null; } }
		IChangeService IDataItemContext.ChangeService { get { return ChangeService; } }
		IDataItemRepositoryProvider IDataItemContext.DataItemRepositoryProvider { get { return Context != null ? Context.DataItemRepositoryProvider : null; } }
		IDataSourceSchemaProvider IDataItemContext.DataSourceSchemaProvider { get { return Context != null ? Context.DataSourceSchemaProvider : null; } }
		ICurrencyCultureNameProvider IDataItemContext.CurrencyCultureNameProvider { get { return Context != null ? Context.CurrencyCultureNameProvider : null; } }
		event EventHandler IDataItemHolder.Changed {
			add { DataItemChanged += value; }
			remove { DataItemChanged -= value; }
		}
		internal event EventHandler DataItemChanged;
		event EventHandler<ChangedEventArgs> IChangeService.Changed { add { } remove { } }
		protected DataItemContainer(IEnumerable<DataItemDescription> dataItemDescriptions) {
			foreach(DataItemDescription description in dataItemDescriptions) {
				string name = description.Name;
				dataItemKeys.Add(name);
				dataItemCaptions.Add(description.Caption);
				dataItemCaptionsPlural.Add(description.CaptionPlural);
				DataItem initialDataItem = description.DataItem;
				if(initialDataItem != null) {
					initialDataItem.Context = this;
					dataItems.Add(name, initialDataItem);
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddDataItem(string key, DataItem dataItem) {
			dataItems.Add(key, dataItem);
			dataItem.Context = this;
		}
		internal void ForEach(Action<string, DataItem> action) {
			foreach(var pair in dataItems)
				action(pair.Key, pair.Value);
		}
		protected internal DataItem GetDataItem(string dataItemKey) {
			DataItem dataItem;
			return dataItems.TryGetValue(dataItemKey, out dataItem) ? dataItem : null;
		}
		protected internal void SetDataItem(string dataItemKey, DataItem dataItem) {
			if(dataItemKeys.Contains(dataItemKey)) {
				DataItem oldDataItem = GetDataItem(dataItemKey);
				DataItem newDataItem = dataItem;
				if(newDataItem != oldDataItem) {
					if(dataItem == null)
						dataItems.Remove(dataItemKey);
					else if(oldDataItem == null)
						dataItems.Add(dataItemKey, newDataItem);
					else
						dataItems[dataItemKey] = newDataItem;
					if(DataItemChanged != null)
						DataItemChanged(this, EventArgs.Empty);
					OnDataItemsChanged(newDataItem != null ? new DataItem[] { newDataItem } : null, oldDataItem != null ? new DataItem[] { oldDataItem } : null);
				}
			}
		}
		protected virtual void OnDataItemsChanged(IEnumerable<DataItem> addedDataItems, IEnumerable<DataItem> removedDataItems) {
			if(addedDataItems != null)
				foreach(DataItem dataItem in addedDataItems)
					dataItem.Context = this;
			if(removedDataItems != null)
				foreach(DataItem dataItem in removedDataItems)
					dataItem.Context = null;
			if(context != null)
				context.OnDataItemsChanged(addedDataItems, removedDataItems);
		}
		internal Measure GetMeasure(string measureKey) {
			return (Measure)GetDataItem(measureKey);
		}
		protected internal virtual void SaveToXml(XElement element) {
			foreach(KeyValuePair<string, DataItem> pair in dataItems) {
				XElement dataItemElement = new XElement(pair.Key);
				dataItemElement.SetUniqueNameAttribute(pair.Value.SerializableUniqueName);
				element.Add(dataItemElement);
			}
		}
		protected internal virtual void LoadFromXml(XElement element) {
			dataItemsNames = new Dictionary<string, string>();
			foreach(string dataItemKey in dataItemKeys) {
				List<string> expectedKeys = new List<string>() { dataItemKey };
				IList<string> alternateDataItemKeys = GetAlternateDataItemKeys(dataItemKey);
				if(alternateDataItemKeys != null)
					expectedKeys.AddRange(alternateDataItemKeys);
				XElement dataItemElement = element.Element(expectedKeys);
				if(dataItemElement != null)
					dataItemsNames.Add(dataItemKey, dataItemElement.GetUniqueNameAttribute());
			}
		}
		internal virtual void OnEndLoading() {
			DataItemRepository dataItemRepository = DataItemRepository;
			if(dataItemRepository != null && dataItemsNames != null) {
				foreach(KeyValuePair<string, string> pair in dataItemsNames) {
					DataItem dataItem = dataItemRepository.FindByUniqueName(pair.Value);
					if(dataItem != null)						
						dataItems.Add(pair.Key, dataItem);
				}
				dataItemsNames = null;
			}
			foreach(DataItem dataItem in dataItems.Values) {
				dataItem.Context = this;
				dataItem.OnEndLoading();
			}			
		}
		internal DataItemContainer Clone() {
			DataItemContainer container = CreateInstance();
			container.Assign(this);
			return container;
		}
		bool IDataItemHolder.IsCompatible(DataItem dataItem, IDataSourceSchema dataSource) {
			return true;
		}
		DataItem IDataItemHolder.Adjust(DataItem dataItem) {
			return dataItem;
		}
		void IChangeService.OnChanged(ChangedEventArgs e) {
			IChangeService changeService = ChangeService;
			if(changeService != null)
				changeService.OnChanged(e);
		}
		SummaryTypeInfo IDataItemContext.GetSummaryTypeInfo(Measure measure) {
			if(dataItems.ContainsValue(measure))
				return GetSummaryTypeInfo(measure);
			if(Context != null)
				return Context.GetSummaryTypeInfo(measure);
			return null;
		}
		DimensionGroupIntervalInfo IDataItemContext.GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(dataItems.ContainsValue(dimension))
				return GetDimensionGroupIntervalInfo(dimension);
			if(Context != null)
				return Context.GetDimensionGroupIntervalInfo(dimension);
			return null;
		}
		bool IDataItemContext.ColorDimension(Dimension dimension) {
			return false;
		}
		protected virtual SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			return null;
		}
		protected virtual DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			return null;
		}
		protected void OnChanged(ChangedEventArgs e) {
			((IChangeService)this).OnChanged(e);
		}
		protected void OnChanged(ChangeReason reason, object context, object param) {
			OnChanged(new ChangedEventArgs(reason, context, param));
		}
		protected internal void OnChanged(ChangeReason reason) {
			OnChanged(reason, this, null);
		}
		protected virtual IList<string> GetAlternateDataItemKeys(string dataItemKey) {
			return null;
		}
		protected virtual void OnDataItemContainerContextChanged() {
		}
		protected internal virtual void Assign(DataItemContainer container) {
			foreach (KeyValuePair<string, DataItem> pair in container.dataItems)
				dataItems.Add(pair.Key, pair.Value);
		}
		protected abstract DataItemContainer CreateInstance();
		protected string GetDataItemActualID(DataItem dataItem) {
			return DataItemRepository != null ? DataItemRepository.GetActualID(dataItem) : null; 
		}
		protected internal abstract DataItemContainerActualContent GetActualContent(); 
	}
}
