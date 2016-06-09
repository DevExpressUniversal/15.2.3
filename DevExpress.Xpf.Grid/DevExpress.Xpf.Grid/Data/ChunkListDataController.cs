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

using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Data.Selection;
using DevExpress.Xpf.Grid;
using System.ComponentModel;
using DevExpress.Xpf.Core.Mvvm.UI.ViewGenerator.Metadata;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Data.Helpers;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Xpf.ChunkList;
namespace DevExpress.Xpf.Data {
	public class ChunkListDataController : StateGridDataController {
#if DEBUGTEST
		public
#endif
		static int MinMaxValueCacheCount = 10;
		IListChanging ListChanging { get { return DataSource as IListChanging; } }
		public ChunkListDataController(IDataProviderOwner owner) : base(owner) { }
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			ListChanging.ListChanging += ListChanging_ListChanging;
		}
		public override void Dispose() {
			ListChanging.ListChanging -= ListChanging_ListChanging;
			base.Dispose();
		}
#if DEBUGTEST
		internal static int CalcSummaryCount { get; private set; }
#endif
		protected override object CalcSummaryValue(SummaryItem summaryItem, SummaryItemType summaryType, bool ignoreNullValues, Type valueType, IEnumerable valuesEnumerable, Func<string[]> exceptionAuxInfoGetter, GroupRowInfo groupRowInfo) {
#if DEBUGTEST
			CalcSummaryCount++;
#endif      
			Dictionary<SummaryItem, object> summaryCache = GetSummaryCache(groupRowInfo, true);
			if(summaryItem.ColumnInfo.Unbound)
				return base.CalcSummaryValue(summaryItem, summaryType, ignoreNullValues, valueType, valuesEnumerable, exceptionAuxInfoGetter, groupRowInfo);
			switch(summaryItem.SummaryType) {
				case SummaryItemType.Min:
				case SummaryItemType.Max:
					return CalcMinMax(valuesEnumerable, summaryItem.SummaryType == SummaryItemType.Max, GetOrCreateMinMaxValuesCache(summaryItem, summaryCache), ignoreNullValues);
				case SummaryItemType.Average:
					object value;
					int nullCount;
					if(!TryCalcAssociativeAverage(valuesEnumerable, out value, out nullCount)) {
						value = base.CalcSummaryValue(summaryItem, SummaryItemType.Sum, ignoreNullValues, valueType, valuesEnumerable, exceptionAuxInfoGetter, groupRowInfo);
						nullCount = ignoreNullValues ? GetNullCount(valuesEnumerable) : 0;
					}
					summaryCache[summaryItem] = new SummaryCache<object>() { Value = value, NullCount = nullCount };
					int count = groupRowInfo == null ? VisibleListSourceRowCount : groupRowInfo.ChildControllerRowCount;
					return CalcAverage(value, count - nullCount);
				case SummaryItemType.Sum:
					object sum = base.CalcSummaryValue(summaryItem, SummaryItemType.Sum, ignoreNullValues, valueType, valuesEnumerable, exceptionAuxInfoGetter, groupRowInfo);
					summaryCache[summaryItem] = sum;
					return sum;
				default:
					return base.CalcSummaryValue(summaryItem, summaryType, ignoreNullValues, valueType, valuesEnumerable, exceptionAuxInfoGetter, groupRowInfo);
			}
		}
		bool TryCalcAssociativeAverage(IEnumerable values, out object value, out int nullCount) {
			value = 0;
			nullCount = 0;
			foreach(object current in values) {
				SummaryCache<object> cached = current as SummaryCache<object>;
				if(cached == null)
					return false;
				value = CalcSumOnAdded(cached.Value, value);
				nullCount += cached.NullCount;
			}
			return true;
		}
		int GetNullCount(IEnumerable values) {
			int count = 0;
			foreach(object value in values)
				if(value == null)
					count++;
			return count;
		}
		protected override bool IsAssociativeSummary(SummaryItemType summaryType) {
			return summaryType == SummaryItemType.Sum || summaryType == SummaryItemType.Average;
		}
		protected override object GetSummaryShortcut(GroupRowInfo groupRowInfo, SummaryItem summaryItem, out bool isValid) {
			if(summaryItem.SummaryType == SummaryItemType.Average) {
				Dictionary<SummaryItem, object> summaryCache = GetSummaryCache(groupRowInfo);
				object value;
				if(summaryCache == null || !summaryCache.TryGetValue(summaryItem, out value)) {
					isValid = false;
					return null;
				}
				isValid = true;
				return value;
			}
			return base.GetSummaryShortcut(groupRowInfo, summaryItem, out isValid);
		}
		Dictionary<SummaryItem, object> totalSummaryCache = new Dictionary<SummaryItem, object>();
		Dictionary<GroupRowInfo, Dictionary<SummaryItem, object>> groupSummaryCache = new Dictionary<GroupRowInfo, Dictionary<SummaryItem, object>>();
#if DEBUGTEST
		public Dictionary<SummaryItem, object> GetTotalSummaryCacheDebug() {
			return totalSummaryCache;
		}
		public Dictionary<GroupRowInfo, Dictionary<SummaryItem, object>> GetGroupSummaryCacheDebug() {
			return groupSummaryCache;
		}
#endif
		public override void UpdateTotalSummary(List<SummaryItem> changedItems) {
			base.UpdateTotalSummary(changedItems);
			List<SummaryItem> cachedItems = totalSummaryCache.Keys.ToList();
			foreach(SummaryItem summaryItem in TotalSummary) {
				int index = cachedItems.IndexOf(summaryItem);
				if(index != -1)
					cachedItems.RemoveAt(index);
			}
			foreach(SummaryItem summaryItem in cachedItems) {
				totalSummaryCache.Remove(summaryItem);
			}
		}
		public override void UpdateGroupSummary(List<SummaryItem> changedItems) {
			groupSummaryCache.Clear();
			base.UpdateGroupSummary(changedItems);
		}
		protected override void OnPreRefresh(bool useRowsKeeper) {
			groupSummaryCache.Clear();
			base.OnPreRefresh(useRowsKeeper);
		}
		protected override void OnGroupDeleted(GroupRowInfo groupInfo) {
			groupSummaryCache.Remove(groupInfo);
			base.OnGroupDeleted(groupInfo);
		}
		Dictionary<SummaryItem, object> GetSummaryCache(GroupRowInfo groupRowInfo, bool allowCreate = false) {
			if(groupRowInfo == null)
				return totalSummaryCache;
			Dictionary<SummaryItem, object> summaryCache;
			if(!groupSummaryCache.TryGetValue(groupRowInfo, out summaryCache) && allowCreate) {
				summaryCache = new Dictionary<SummaryItem, object>();
				groupSummaryCache[groupRowInfo] = summaryCache;
			}
			return summaryCache;
		}
		SummaryCache<SortedList<object, int>> GetOrCreateMinMaxValuesCache(SummaryItem summaryItem, Dictionary<SummaryItem, object> summaryCache) {
			object cached = null;
			if(!summaryCache.TryGetValue(summaryItem, out cached)) {
				return CreateMinMaxValueCache(summaryItem, summaryCache);
			} else {
				return (SummaryCache<SortedList<object, int>>)cached;
			}
		}
		SummaryCache<SortedList<object, int>> CreateMinMaxValueCache(SummaryItem summaryItem, Dictionary<SummaryItem, object> summaryCache) {
			SummaryCache<SortedList<object, int>> tuple = new SummaryCache<SortedList<object, int>>() { Value = new SortedList<object, int>(MinMaxValueCacheCount) };
			summaryCache[summaryItem] = tuple;
			return tuple;
		}
		protected override void UpdateTotalSummaryOnItemDeleted(int controllerRow) {
		}
		protected override void UpdateTotalSummaryOnItemFilteredOut(int listSourceRow) {
			ProcessItemDeleted(GetRowByListSourceIndex(listSourceRow), string.Empty, listSourceRow, true, true);
		}
		void ListChanging_ListChanging(object sender, ListChangingEventArgs e) {
			object row = GetRowByListSourceIndex(e.Index);
			if(IsCurrentRowEditing && row == CurrentControllerRowObject)
				return;
			ProcessItemDeleted(row, e.PropertyDescriptor != null ? e.PropertyDescriptor.Name : string.Empty, e.Index, false, true);
		}
		public override void BeginCurrentRowEdit() {
			if(!IsCurrentRowEditing)
				ProcessItemDeleted(CurrentControllerRowObject, string.Empty, CurrentListSourceIndex, false, false);
			base.BeginCurrentRowEdit();
		}
		public override void UpdateGroupSummary(GroupRowInfo groupRowInfo, DataControllerChangedItemCollection changedItems) {
			while(groupRowInfo != null) {
				if(changedItems != null) changedItems.AddItem(groupRowInfo.Handle, NotifyChangeType.ItemChanged, groupRowInfo.ParentGroup);
				groupRowInfo = groupRowInfo.ParentGroup;
			}
		}
		protected override void UpdateTotalSummaryOnItemAdded(int listSourceRow) {
			ProcessItemAdded(listSourceRow, string.Empty, true);
		}
		void ProcessItemAdded(int listSourceRow, string propertyName, bool updateSummary) {
			if(IsGrouped) {
				int rowHandle = GetControllerRow(listSourceRow);
				while(true) {
					rowHandle = GetParentRowHandle(rowHandle);
					if(rowHandle == InvalidRow)
						break;
					GroupRowInfo groupRowInfo = GroupInfo.GetGroupRowInfoByHandle(rowHandle);
					Dictionary<SummaryItem, object> cache = GetSummaryCache(groupRowInfo, true);
					ProcessItemAdded(listSourceRow, IsGroupedColumn(propertyName) || cache.Count == 0 ? string.Empty : propertyName, GroupSummary, cache, groupRowInfo, updateSummary);
				}
			}
			ProcessItemAdded(listSourceRow, propertyName, TotalSummary, totalSummaryCache, null, updateSummary);
		}
		bool IsGroupedColumn(string propertyName) {
			for(int i = 0; i < SortInfo.GroupCount; i++) {
				if(SortInfo[i].ColumnInfo.Name == propertyName)
					return true;
			}
			return false;
		}
		void ProcessItemAdded(int listSourceRow, string propertyName, SummaryItemCollection summaryCollection, Dictionary<SummaryItem, object> summaryCache, GroupRowInfo groupRowInfo, bool updateSummary) {
			foreach(SummaryItem summaryItem in summaryCollection) {
				if(propertyName != string.Empty && summaryItem.FieldName != propertyName && !summaryItem.ColumnInfo.Unbound) 
					continue;
				object summaryValue = CalcSummaryValueOnAdded(listSourceRow, summaryCache, groupRowInfo, summaryItem);
				if(!updateSummary)
					continue;
				if(groupRowInfo == null)
					summaryItem.SummaryValue = summaryValue;
				else
					groupRowInfo.SetSummaryValue(summaryItem, summaryValue);
			}
		}
		object CalcSummaryValueOnAdded(int listSourceRow, Dictionary<SummaryItem, object> summaryCache, GroupRowInfo groupRowInfo, SummaryItem summaryItem) {
			if(summaryItem.ColumnInfo.Unbound)
				return CalcSummaryValueCore(groupRowInfo, summaryItem);
			object summaryValue;
			summaryCache.TryGetValue(summaryItem, out summaryValue);
			object value = GetRowValue(GetControllerRow(listSourceRow), summaryItem.ColumnInfo);
			switch(summaryItem.SummaryType) {
				case SummaryItemType.Min:
				case SummaryItemType.Max:
					if(summaryValue == null) {
						summaryValue = CreateMinMaxValueCache(summaryItem, summaryCache);
					}
					summaryValue = GetMinMaxValueOnAdded(summaryItem, value, (SummaryCache<SortedList<object, int>>)summaryValue, groupRowInfo);
					break;
				case SummaryItemType.Sum:
					summaryValue = CalcSumOnAdded(summaryValue != null ? summaryValue : 0, value);
					summaryCache[summaryItem] = summaryValue;
					break;
				case SummaryItemType.Average:
					SummaryCache<object> cached = (SummaryCache<object>)summaryValue;
					if(cached == null) {
						cached = new SummaryCache<object>() { Value = 0 };
						summaryCache[summaryItem] = cached;
					}
					if(value == null && SummariesIgnoreNullValues)
						cached.NullCount++;
					object sum = CalcSumOnAdded(cached.Value, value);
					summaryValue = CalcAverage(sum, (groupRowInfo == null ? VisibleListSourceRowCount : groupRowInfo.ChildControllerRowCount) - cached.NullCount);
					cached.Value = sum;
					break;
				case SummaryItemType.Count:
					summaryValue = groupRowInfo == null ? VisibleListSourceRowCount : groupRowInfo.ChildControllerRowCount;
					break;
				default:
					summaryValue = CalcSummaryValueCore(groupRowInfo, summaryItem);
					break;
			}
			return summaryValue;
		}
		object CalcSummaryValueCore(GroupRowInfo groupRow, SummaryItem summaryItem) {
			bool isValid = false;
			return CalcSummaryInfo(groupRow, summaryItem, ref isValid);
		}
		protected override void UpdateTotalSummaryOnItemChanged(int listSourceRow, string propertyName) {
			bool updateCurrentRowValues = IsCurrentRowEditing && CurrentListSourceIndex != listSourceRow;
			if(updateCurrentRowValues)
				ProcessItemAdded(CurrentListSourceIndex, string.Empty, false);
			ProcessItemAdded(listSourceRow, propertyName, true);
			if(updateCurrentRowValues)
				ProcessItemDeleted(CurrentControllerRowObject, string.Empty, CurrentListSourceIndex, false, false);
		}
		List<Tuple<GroupRowInfo, SummaryItem>> recalcSummaryCache = new List<Tuple<GroupRowInfo, SummaryItem>>();
		protected override void OnBindingListChangedCore(ListChangedEventArgs e) {
			ChunkListChangedEventArgs args = e as ChunkListChangedEventArgs;
			if(args != null) {
				ProcessItemDeleted(args.OldItem, string.Empty, e.NewIndex, false, true);
			}
			base.OnBindingListChangedCore(e);
		}
		protected override void OnVisibleIndexesUpdated() {
			foreach(Tuple<GroupRowInfo, SummaryItem> tuple in recalcSummaryCache) {
				GroupRowInfo groupRowInfo = tuple.Item1;
				SummaryItem summaryItem = tuple.Item2;
				object value = CalcSummaryValueCore(groupRowInfo, summaryItem);
				if(groupRowInfo == null)
					summaryItem.SummaryValue = value;
				else
					groupRowInfo.SetSummaryValue(summaryItem, value);
			}
			recalcSummaryCache.Clear();
		}
		void ProcessItemDeleted(object row, string propertyName, int listSourceRow, bool allowRecalculate, bool updateSummary) {
			if(IsGrouped) {
				int rowHandle = GetControllerRow(listSourceRow);
				while(true) {
					rowHandle = GetParentRowHandle(rowHandle);
					if(rowHandle == InvalidRow)
						break;
					GroupRowInfo groupRowInfo = GroupInfo.GetGroupRowInfoByHandle(rowHandle);
					if(groupRowInfo.ChildControllerRowCount == 1)
						groupSummaryCache.Remove(groupRowInfo);
					else
						ProcessItemDeleted(row, IsGroupedColumn(propertyName) ? string.Empty : propertyName, GroupSummary, GetSummaryCache(groupRowInfo), groupRowInfo, allowRecalculate, updateSummary);
				}
			}
			ProcessItemDeleted(row, propertyName, TotalSummary, totalSummaryCache, null, allowRecalculate, updateSummary);
		}
		void ProcessItemDeleted(object row, string propertyName, SummaryItemCollection summaryCollection, Dictionary<SummaryItem, object> summaryCache, GroupRowInfo groupRowInfo, bool allowRecalculate, bool updateSummary) {
			foreach(SummaryItem summaryItem in summaryCollection) {
				if(propertyName != string.Empty && summaryItem.FieldName != propertyName && !summaryItem.ColumnInfo.Unbound)
					continue;
				if(summaryItem.ColumnInfo.Unbound || summaryItem.SummaryType == SummaryItemType.Custom) {
					recalcSummaryCache.Add(new Tuple<GroupRowInfo, SummaryItem>(groupRowInfo, summaryItem));
					continue;
				}
				object summaryValue;
				object value = summaryItem.ColumnInfo.PropertyDescriptor.GetValue(row);
				int visibleCount;
				if(groupRowInfo == null) {
					visibleCount = VisibleListSourceRowCount;
					if(IsGrouped)
						visibleCount--;
				} else
					visibleCount = groupRowInfo.ChildControllerRowCount - 1;
				switch(summaryItem.SummaryType) {
					case SummaryItemType.Min:
					case SummaryItemType.Max:
						summaryValue = GetMinMaxValueOnDeleted(summaryItem, value, (SummaryCache<SortedList<object, int>>)summaryCache[summaryItem], allowRecalculate, groupRowInfo);
						break;
					case SummaryItemType.Sum:
						summaryValue = CalcSumOnDeleted(summaryCache[summaryItem], value);
						summaryCache[summaryItem] = summaryValue;
						break;
					case SummaryItemType.Average:
						SummaryCache<object> cached = (SummaryCache<object>)summaryCache[summaryItem];
						if(value == null && SummariesIgnoreNullValues)
							cached.NullCount--;
						object sum = CalcSumOnDeleted(cached.Value, value);
						summaryValue = CalcAverage(sum, visibleCount - cached.NullCount);
						cached.Value = sum;
						break;
					case SummaryItemType.Count:
						summaryValue = visibleCount > 0 ? visibleCount : 0;
						break;
					default:
						summaryValue = CalcSummaryValueCore(groupRowInfo, summaryItem);
						break;
				}
				if(!updateSummary)
					continue;
				if(groupRowInfo == null)
					summaryItem.SummaryValue = summaryValue;
				else
					groupRowInfo.SetSummaryValue(summaryItem, summaryValue);
			}
		}
		object CalcMinMax(IEnumerable valuesEnumerable, bool isMax, SummaryCache<SortedList<object, int>> summaryCache, bool ignoreNullValues) {
			summaryCache.Value.Clear();
			summaryCache.NullCount = 0;
			foreach(object item in valuesEnumerable) {
				if(!ignoreNullValues || item != null)
					ProcessMinMaxValue(summaryCache, item, isMax, 1, true);
			}
			return GetSummaryValue(isMax, summaryCache);
		}
		void ProcessMinMaxValue(SummaryCache<SortedList<object, int>> summaryCache, object item, bool isMax, int increment, bool force) {
			if(item == null) {
				if(!SummariesIgnoreNullValues)
					summaryCache.NullCount++;
				return;
			}
			if(summaryCache.Value.ContainsKey(item)) {
				summaryCache.Value[item] += increment;
				return;
			}
			int index = isMax ? 0 : summaryCache.Value.Count - 1;
			if((force && summaryCache.Value.Count < summaryCache.Value.Capacity) || (summaryCache.Value.Count > 0 && Comparer.Default.Compare(summaryCache.Value.Keys[index], item) > 0 ^ isMax)) {
				if(summaryCache.Value.Count == summaryCache.Value.Capacity)
					summaryCache.Value.RemoveAt(index);
				summaryCache.Value[item] = increment;
			}
		}
		object GetMinMaxValueOnDeleted(SummaryItem summaryItem, object value, SummaryCache<SortedList<object, int>> summaryCache, bool allowRecalculate, GroupRowInfo groupRowInfo) {
			if(value == null) {
				if(!SummariesIgnoreNullValues)
					summaryCache.NullCount--;
			} else {
				if(summaryCache.Value.ContainsKey(value)) {
					if(summaryCache.Value[value] == 1)
						summaryCache.Value.Remove(value);
					else
						summaryCache.Value[value]--;
				}
			}
			bool isMax = summaryItem.SummaryType == SummaryItemType.Max;
			if(summaryCache.Value.Count == 0 && (isMax || summaryCache.NullCount == 0)) {
				if(allowRecalculate)
					CalcSummaryValue(summaryItem, groupRowInfo);
				else
					recalcSummaryCache.Add(new Tuple<GroupRowInfo, SummaryItem>(groupRowInfo, summaryItem));
			}
			return GetSummaryValue(isMax, summaryCache);
		}
		object GetMinMaxValueOnAdded(SummaryItem summaryItem, object value, SummaryCache<SortedList<object, int>> summaryCache, GroupRowInfo groupRowInfo) {
			bool isMax = summaryItem.SummaryType == SummaryItemType.Max;
			ProcessMinMaxValue(summaryCache, value, isMax, 1, false);
			if(summaryCache.Value.Count == 0) {
				CalcSummaryValue(summaryItem, groupRowInfo);
				Tuple<GroupRowInfo, SummaryItem> tuple = new Tuple<GroupRowInfo, SummaryItem>(groupRowInfo, summaryItem);
				if(recalcSummaryCache.Contains(tuple))
					recalcSummaryCache.Remove(tuple);
			}
			return GetSummaryValue(isMax, summaryCache);
		}
		object GetSummaryValue(bool isMax, SummaryCache<SortedList<object, int>> summaryCache) {
			if(!isMax && summaryCache.NullCount > 0 && !SummariesIgnoreNullValues)
				return null;
			if(summaryCache.Value.Count == 0)
				return null;
			return summaryCache.Value.Keys[isMax ? summaryCache.Value.Count - 1 : 0];
		}
		object CalcSumOnAdded(object summaryValue, object value) {
			if(value == null)
				return summaryValue;
			Type type = summaryValue.GetType();
			switch(DXTypeExtensions.GetTypeCode(type)) {
				case TypeCode.Int32:
				case TypeCode.Int64:
					long sum = Convert.ToInt64(summaryValue) + Convert.ToInt64(value);
					return sum > int.MaxValue || sum < int.MinValue ? sum : (int)sum;
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					ulong total = Convert.ToUInt64(summaryValue) + Convert.ToUInt64(value);
					return total > uint.MaxValue ? total : (uint)total;
				case TypeCode.Decimal:
					return Convert.ToDecimal(summaryValue) + Convert.ToDecimal(value);
				case TypeCode.Double:
					return Convert.ToDouble(summaryValue) + Convert.ToDouble(value);
				case TypeCode.Single:
					return Convert.ToSingle(summaryValue) + Convert.ToSingle(value);
				case TypeCode.Object:
					if(type == typeof(TimeSpan)) {
						double current = ((TimeSpan)summaryValue).TotalMilliseconds + ((TimeSpan)value).TotalMilliseconds;
						return TimeSpan.FromMilliseconds(current);
					}
					break;
				default:
					break;
			}
			throw new NotSupportedException();
		}
		object CalcSumOnDeleted(object summaryValue, object value) {
			if(value == null)
				return summaryValue;
			Type type = summaryValue.GetType();
			switch(DXTypeExtensions.GetTypeCode(type)) {
				case TypeCode.Int32:
				case TypeCode.Int64:
					long sum = Convert.ToInt64(summaryValue) - Convert.ToInt64(value);
					return sum > int.MaxValue || sum < int.MinValue ? sum : (int)sum;
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					ulong total = Convert.ToUInt64(summaryValue) - Convert.ToUInt64(value);
					return total > uint.MaxValue ? total : (uint)total;
				case TypeCode.Decimal:
					return Convert.ToDecimal(summaryValue) - Convert.ToDecimal(value);
				case TypeCode.Double:
					return Convert.ToDouble(summaryValue) - Convert.ToDouble(value);
				case TypeCode.Single:
					return Convert.ToSingle(summaryValue) - Convert.ToSingle(value);
				case TypeCode.Object:
					if(type == typeof(TimeSpan)) {
						double current = ((TimeSpan)summaryValue).TotalMilliseconds - ((TimeSpan)value).TotalMilliseconds;
						return TimeSpan.FromMilliseconds(current);
					}
					break;
				default:
					break;
			}
			throw new NotSupportedException();
		}
		object CalcAverage(object total, int count) {
			if(count <= 0)
				return null;
			Type type = total.GetType();
			switch(DXTypeExtensions.GetTypeCode(type)) {
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Decimal:
					return Convert.ToDecimal(total) / count;
				case TypeCode.Double:
					return Convert.ToDecimal(total) / count;
				case TypeCode.Single:
					return Convert.ToDecimal(total) / count;
				case TypeCode.Object:
					if(type == typeof(TimeSpan)) {
						return TimeSpan.FromMilliseconds(((TimeSpan)total).TotalMilliseconds / count);
					}
					break;
				default:
					break;
			}
			throw new NotSupportedException();
		}
	}
	class SummaryCache<T> {
		public T Value { get; set; }
		public int NullCount { get; set; } 
	}
}
