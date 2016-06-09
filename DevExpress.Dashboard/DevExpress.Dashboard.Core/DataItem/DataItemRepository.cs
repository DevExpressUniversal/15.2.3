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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardCommon {
	[	
	DesignerSerializer(TypeNames.DataItemRepositoryCodeDomSerializer, TypeNames.CodeDomSerializer),
	EditorBrowsable(EditorBrowsableState.Never)
	]
	public class DataItemRepository {
		readonly IDataItemSerializationContext context;
		readonly OrderedDictionary<DataItem, string> dictionary = new OrderedDictionary<DataItem, string>();
		readonly PrefixNameGenerator nameGenerator = new PrefixNameGenerator("DataItem");
		internal int Count { get { return dictionary.Count; } }
		internal ICollection<DataItem> DataItems { get { return dictionary.Keys; } }
		internal IEnumerable<Dimension> Dimensions { get { return GetDataItems<Dimension>(); } }
		internal IEnumerable<Measure> Measures { get { return GetDataItems<Measure>(); } }
		internal ICollection<DataItem> UniqueDataItems { get { return GetUniqueDataItems<DataItem>(); } }
		internal ICollection<Dimension> UniqueDimensions { get { return GetUniqueDataItems<Dimension>(); } }
		internal ICollection<Measure> UniqueMeasures { get { return GetUniqueDataItems<Measure>(); } }
		internal IEnumerable<KeyValuePair<DataItem, string>> DataItemsSerializable {
			get {
				foreach(KeyValuePair<DataItem, string> pair in dictionary)
					if(context == null || context.ShouldSerializeDataItem(pair.Key))
						yield return pair;
			}
		}
		internal DataItemRepository(IDataItemSerializationContext context) {
			this.context = context;
		}
		public void Clear() {
			List<DataItem> itemsToRemove = new List<DataItem>();
			foreach(KeyValuePair<DataItem, string> pair in DataItemsSerializable)
				itemsToRemove.Add(pair.Key);
			foreach(DataItem dataItem in itemsToRemove)
				Remove(dataItem);
		}
		public void Add(DataItem dataItem, string uniqueName) {
			dictionary.Add(dataItem, uniqueName);
		}
		public void AddDataItem(DataItem dataItem) {
			Add(dataItem, nameGenerator.GenerateNameFromStart(name => FindByUniqueName(name) != null));
		}
		public string GetSerializableUniqueName(DataItem dataItem) {
			return dictionary[dataItem];
		}
		internal void Remove(DataItem dataItem) {
			dictionary.Remove(dataItem);
		}
		internal bool Contains(DataItem dataItem) {
			return dictionary.ContainsKey(dataItem);
		}
		internal DataItem FindByUniqueName(string uniqueName) {
			return FindByUniqueName<DataItem>(uniqueName);
		}
		internal TDataItem FindByUniqueName<TDataItem>(string uniqueName) where TDataItem : DataItem {
			return (TDataItem)dictionary.FirstOrDefault(pair => pair.Key is TDataItem && pair.Value == uniqueName).Key;
		}
		internal string GetActualID(DataItemDefinition definition) {
			DataItem dataItem = dictionary.FirstOrDefault(pair => pair.Key.EqualsByDefinition(definition)).Key;
			return dataItem != null ? GetActualID(dataItem) : null;
		}
		public string GetActualID(DataItem dataItem) {
			if(dataItem == null)
				return null;
			string id = dataItem.ID;
			return String.IsNullOrEmpty(id) ? GetSerializableUniqueName(dataItem) : id;
		}
		internal void SaveToXml(XElement element) {
			foreach(KeyValuePair<DataItem, string> pair in DataItemsSerializable) {
				DataItem dataItem = pair.Key;
				string uniqueName = pair.Value;
				DataItemXmlSerializationContext context = new DataItemXmlSerializationContext { UniqueName = uniqueName };
				XElement dataItemElement = DataItemXmlHelper.SaveDataItemToXml(dataItem, context);
				if(dataItemElement != null)
					element.Add(dataItemElement);
			}
		}
		internal void LoadFromXml(XElement element) {
			foreach(XElement dataItemElement in element.Elements()) {
				DataItemXmlSerializationContext context = new DataItemXmlSerializationContext();
				DataItem dataItem = DataItemXmlHelper.LoadDataItemFromXml(dataItemElement, context);
				if(dataItem != null)
					Add(dataItem, context.UniqueName);
			}
		}
		IEnumerable<TDataItem> GetDataItems<TDataItem>() where TDataItem : DataItem {
			foreach(DataItem dataItem in DataItems) {
				TDataItem di = dataItem as TDataItem;
				if(di != null)
					yield return di;
			}
		}
		ICollection<TDataItem> GetUniqueDataItems<TDataItem>() where TDataItem : DataItem {
			List<TDataItem> uniqueDataItems = new List<TDataItem>();
			foreach(TDataItem dataItem in GetDataItems<TDataItem>())
				if(uniqueDataItems.Find(uniqueDataItem => uniqueDataItem.EqualsByDefinition(dataItem)) == null)
					uniqueDataItems.Add(dataItem);
			return uniqueDataItems;
		}
	}
}
