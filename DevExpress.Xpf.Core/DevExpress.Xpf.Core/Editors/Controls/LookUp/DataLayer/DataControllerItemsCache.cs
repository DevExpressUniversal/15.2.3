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
using DevExpress.Data;
using DevExpress.Xpf.Editors.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public class DataControllerItemsCache {
		readonly object nullableValue = new object();
		int valueIndex;
		int itemIndex;
		public HashSet<object> NotAvailableValues { get; set; }
		public Dictionary<object, int> ValueToIndex { get; private set; }
		public Dictionary<object, int> ItemToIndex { get; private set; }
		public Dictionary<int, object> IndexToItem { get; private set; }
		public Dictionary<int, object> IndexToValue { get; private set; }
		public int ValueIndex { get { return valueIndex; } }
		public int ItemIndex { get { return itemIndex; } }
		IDataControllerAdapter DataControllerAdapter { get; set; }
		public DataControllerItemsCache(IDataControllerAdapter dataControllerAdapter) {
			DataControllerAdapter = dataControllerAdapter;
			ValueToIndex = new Dictionary<object, int>();
			ItemToIndex = new Dictionary<object, int>();
			IndexToItem = new Dictionary<int, object>();
			IndexToValue = new Dictionary<int, object>();
			NotAvailableValues = new HashSet<object>();
		}
		public int IndexOfValue(object value) {
			object currentValue = GetWrapperValue(value);
			int result;
			if (ValueToIndex.TryGetValue(currentValue, out result)) 
				return result;
			return FindIndexByValue(value);
		}
		public int IndexByItem(object item) {
			object currentItem = GetWrapperValue(item);
			int result;
			return ItemToIndex.TryGetValue(currentItem, out result) ? result : FindIndexByItem(item);
		}
		int FindIndexByValue(object value) {
			if (NotAvailableValues.Contains(value))
				return -1;
			return DataControllerAdapter.IsOwnSearchProcessing ? GetListSourceIndexAsync(value) : GetListSourceIndex(value);
		}
		int GetListSourceIndexAsync(object value) {
			int listSourceIndex = DataControllerAdapter.GetListSourceIndex(value);
			if (listSourceIndex == Data.DataController.OperationInProgress) {
				NotAvailableValues.Add(value);
				return -1;
			}
			UpdateItem(listSourceIndex);
			return listSourceIndex;
		}
		int GetListSourceIndex(object value) {
			for (int i = valueIndex; i < DataControllerAdapter.VisibleRowCount; i++) {
				valueIndex = itemIndex = i + 1;
				int index = i;
				object currentItem = AddItemToCache(index);
				object currentValue = AddValueToCache(index);
				AddIndexToCache(i, currentItem, currentValue);
				if (object.Equals(value, currentValue))
					return index;
			}
			return -1;
		}
		object AddItemToCache(int listSourceIndex) {
			object item = DataControllerAdapter.GetRow(listSourceIndex);
			if (!ItemToIndex.ContainsKey(GetWrapperValue(item)))
				ItemToIndex[GetWrapperValue(item)] = listSourceIndex;
			return item;
		}
		void AddIndexToCache(int listSourceIndex, object item, object value) {
			IndexToItem[listSourceIndex] = item;
			IndexToValue[listSourceIndex] = value;
		}
		object AddValueToCache(int listSourceIndex) {
			object value = DataControllerAdapter.GetRowValue(listSourceIndex);
			if (!ValueToIndex.ContainsKey(GetWrapperValue(value))) {
				ValueToIndex[GetWrapperValue(value)] = LookUpPropertyDescriptorBase.IsUnsetValue(value) ? -1 : listSourceIndex;
				NotAvailableValues.Remove(value);
			}
			return value;
		}
		object GetWrapperValue(object value) {
			return value ?? nullableValue;
		}
		int FindIndexByItem(object item) {
			if (DataControllerAdapter.IsOwnSearchProcessing)
				return FindIndexByItemServerMode(item);
			return FindIndexByItemLocal(item);
		}
		int FindIndexByItemLocal(object item) {
			for (int i = itemIndex; i < DataControllerAdapter.VisibleRowCount; i++, itemIndex++) {
				int index = i;
				object current = DataControllerAdapter.GetRow(index);
				ItemToIndex[GetWrapperValue(current)] = index;
				IndexToItem[index] = current;
				if (object.Equals(current, item))
					return index;
			}
			return -1;
		}
		int FindIndexByItemServerMode(object item) {
			object value = DataControllerAdapter.GetRowValue(item);
			return FindIndexByValue(value);
		}
		public void ResetIndexes() {
			itemIndex = 0;
			valueIndex = 0;
		}
		public void Reset() {
			ResetIndexes();
			ValueToIndex.Clear();
			ItemToIndex.Clear();
			IndexToItem.Clear();
			IndexToValue.Clear();
			NotAvailableValues.Clear();
		}
		public void UpdateItemOnAdding(int newIndex) {
			Reset();
		}
		public void UpdateItemOnDeleting(int newIndex) {
			Reset();
		}
		public void UpdateItemOnMoving(int oldIndex, int newIndex) {
			Reset();
		}
		public void UpdateItem(int listSourceIndex) {
			if (listSourceIndex < 0)
				return;
			RemoveItem(listSourceIndex);
			AddItemToCache(listSourceIndex);
			AddValueToCache(listSourceIndex);
			AddIndexToCache(listSourceIndex, DataControllerAdapter.GetRow(listSourceIndex), DataControllerAdapter.GetRowValue(listSourceIndex));
		}
		void RemoveItem(int listSourceIndex) {
			if (IndexToItem.ContainsKey(listSourceIndex)) {
				ItemToIndex.Remove(GetWrapperValue(IndexToItem[listSourceIndex]));
				IndexToItem.Remove(listSourceIndex);
			}
			if (IndexToValue.ContainsKey(listSourceIndex)) {
				ValueToIndex.Remove(GetWrapperValue(IndexToValue[listSourceIndex]));
				IndexToValue.Remove(listSourceIndex);
			}
		}
		public void UpdateItemValue(int listSourceIndex, object value) {
			ValueToIndex[GetWrapperValue(value)] = listSourceIndex;
			if (listSourceIndex < 0)
				return;
			if (IndexToValue.ContainsKey(listSourceIndex))
				IndexToValue[listSourceIndex] = value;
		}
	}
}
