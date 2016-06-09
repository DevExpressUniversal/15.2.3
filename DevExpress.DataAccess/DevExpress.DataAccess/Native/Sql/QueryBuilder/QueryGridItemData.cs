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
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public class QueryGridItemData {
		public class List : ItemDataList<QueryGridItemData> {
			static readonly PropertyDescriptor outputPropertyDescriptor;
			static readonly PropertyDescriptor aliasPropertyDescriptor;
			static readonly PropertyDescriptor sortingTypePropertyDescriptor;
			static readonly PropertyDescriptor sortOrderPropertyDescriptor;
			static readonly PropertyDescriptor aggregatePropertyDescriptor;
			static readonly PropertyDescriptor groupByPropertyDescriptor;
			static readonly PropertyDescriptor columnPropertyDescriptor;
			static readonly PropertyDescriptor columnDataPropertyDescriptor;
			static List() {
				PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(QueryGridItemData));
				outputPropertyDescriptor = pdc["Output"];
				aliasPropertyDescriptor = pdc["Alias"];
				sortingTypePropertyDescriptor = pdc["SortingType"];
				sortOrderPropertyDescriptor = pdc["SortOrder"];
				aggregatePropertyDescriptor = pdc["Aggregate"];
				groupByPropertyDescriptor = pdc["GroupBy"];
				columnPropertyDescriptor = pdc["Column"];
				columnDataPropertyDescriptor = pdc["ColumnData"];
			}
			public List(QueryBuilderViewModel owner) : base(owner) { AllowNew = true; }
			public void Initialize(IEnumerable<TableInfo> tables, IEnumerable<SortingInfo> sortingInfos, IEnumerable<GroupingInfo> groupingInfos) {
				foreach(TableInfo table in tables)
					foreach(ColumnInfo column in table.SelectedColumns)
						Add(new QueryGridItemData(this, column.Name, table.ActualName, column.Alias, column.Aggregation));
				InitSorting(sortingInfos);
				InitGrouping(groupingInfos);
			}
			void InitSorting(IEnumerable<SortingInfo> sortingInfos) {
				int sortOrder = 1;
				foreach(SortingInfo sorting in sortingInfos) {
					QueryGridItemData row = this.FirstOrDefault(item =>
						item.Aggregate == AggregationType.None &&
						string.Equals(item.ActualName, sorting.Column, StringComparison.Ordinal) &&
						string.Equals(item.Table, sorting.Table, StringComparison.Ordinal));
					if(row != null) {
						SetSortingType(row, sorting.Direction, false);
						SetSortOrder(row, sortOrder, false);
					}
					else
						Add(new QueryGridItemData(this, sorting.Column, sorting.Table, null, false, sorting.Direction,
							sortOrder, false, AggregationType.None));
					sortOrder++;
				}
			}
			void InitGrouping(IEnumerable<GroupingInfo> groupingInfos) {
				foreach(GroupingInfo grouping in groupingInfos) {
					QueryGridItemData row = this.FirstOrDefault(item =>
						string.Equals(item.Column, grouping.Column, StringComparison.Ordinal) &&
						string.Equals(item.Table, grouping.Table, StringComparison.Ordinal));
					if(row != null)
						SetGroupBy(row, true, false);
					else
						Add(new QueryGridItemData(this, grouping.Column, grouping.Table, null, false, null, null, true, AggregationType.None));
				}
			}
			#region Overrides of BindingList<QueryGridItemData>
			protected override void RemoveItem(int index) {
				QueryGridItemData item = this[index];
				Owner.BeginUpdate(false);
				try {
					if(item.Output)
						SetOutputFull(item, false);
					if(item.SortingType != null)
						SetSortingTypeFull(item, null);
					if(item.GroupBy)
						SetGroupByFull(item, false);
				}
				finally { Owner.EndUpdate(false); }
				base.RemoveItem(index);
			}
			protected override void OnAddingNew(AddingNewEventArgs e) {
				e.NewObject = new QueryGridItemData(this, null, null);
			}
			#endregion
			internal void SetColumnData(QueryGridItemData item, ColumnDataItem value, bool update) {
				SetItemProperty<ColumnDataItem>(item, value,
					new ColumnDataItemComparer(),
					columnPropertyDescriptor, 
					data => data.ColumnData, 
					update 
						? SetColumnDataFull
						: (Action<QueryGridItemData, ColumnDataItem>)((data, s) => data.columnData = s));
			}
			internal void SetOutput(QueryGridItemData item, bool value, bool update) {
				SetItemProperty(item, value, outputPropertyDescriptor,
					data => data.Output,
					update
						? SetOutputFull
						: (Action<QueryGridItemData, bool>)((data, b) => data.output = b));
			}
			internal void SetAlias(QueryGridItemData item, string value, bool update) {
				SetItemProperty(item, value, aliasPropertyDescriptor,
					data => data.Alias,
					update
						? SetAliasFull
						: (Action<QueryGridItemData, string>)((data, s) => {
							PatchGroupFilterString(data, s);
							data.alias = s;
						}));
			}
			internal void SetSortingType(QueryGridItemData item, SortingInfo.SortingDirection? value, bool update) {
				SetItemProperty(item, value, EqualityComparer<SortingInfo.SortingDirection?>.Default, sortingTypePropertyDescriptor,
					data => data.SortingType,
					update
						? SetSortingTypeFull
						: (Action<QueryGridItemData, SortingInfo.SortingDirection?>)((data, direction) => data.sortingType = direction));
			}
			internal void SetSortOrder(QueryGridItemData item, int? value, bool update) {
				SetItemProperty(item, value, EqualityComparer<int?>.Default, sortOrderPropertyDescriptor,
					data => data.SortOrder,
					update
						? SetSortOrderFull
						: (Action<QueryGridItemData, int?>)((data, i) => data.sortOrder = i));
			}
			internal void SetAggregate(QueryGridItemData item, AggregationType value, bool update) {
				SetItemProperty(item, value, EqualityComparer<AggregationType>.Default, aggregatePropertyDescriptor, 
					data => data.Aggregate,
					update
						? SetAggregateFull
						: (Action<QueryGridItemData, AggregationType>)((data, type) => data.aggregate = type));
			}
			internal void SetGroupBy(QueryGridItemData item, bool value, bool update) {
				SetItemProperty(item, value, groupByPropertyDescriptor, 
					data => data.GroupBy,
					update 
						? SetGroupByFull 
						: (Action<QueryGridItemData, bool>)((data, b) => data.groupBy = b));
			}
			void SetOutputFull(QueryGridItemData item, bool value) {
				item.output = value;
				TableInfo tableInfo = Owner.GetTableInfo(item.Table);
				SelectionItemData selectionItemData = Owner.Selection.FindNode(item.Table, item.Column);
				if(value) {
					string alias;
					if(item.Aggregate != AggregationType.None)
						alias = Owner.CreateColumnAlias(string.Format("{0}_{1}", item.Aggregate, item.Column));
					else if(Owner.GetColumnInfo(item.Column) == null)
						alias = null;
					else
						alias = Owner.CreateColumnAlias(string.Format("{0}_{1}", item.Table, item.Column));
					SetAlias(item, alias, false);
					tableInfo.SelectColumn(item.Column, item.Aggregate, alias);
					if(item.Aggregate == AggregationType.None)
						Owner.Selection.SetSelected(selectionItemData, true, false);
					else
						Owner.Selection.SetAggregated(selectionItemData, true);
				}
				else {
					tableInfo.SelectedColumns.RemoveAll(column => string.Equals(column.ActualName, item.ActualName, StringComparison.Ordinal));
					SetAlias(item, null, false);
					if(item.Aggregate == AggregationType.None)
						Owner.Selection.SetSelected(selectionItemData, tableInfo.SelectedColumns.Any(ci => ci.Aggregation == AggregationType.None && string.Equals(ci.Name, item.Column, StringComparison.Ordinal)), false);
					else
						Owner.Selection.SetAggregated(selectionItemData, tableInfo.SelectedColumns.Any(ci => ci.Aggregation != AggregationType.None && string.Equals(ci.Name, item.Column, StringComparison.Ordinal)));
				}
			}
			void SetAliasFull(QueryGridItemData item, string value) {
				if(Owner.GetColumnInfo(value ?? item.Column) != null)
					throw new ArgumentException(
						DataAccessLocalizer.GetString(DataAccessStringId.QueryBuilderAliasAlreadyExists));
				ColumnInfo columnInfo = Owner.GetColumnInfo(item.Table, item.ActualName);
				if(columnInfo == null) {
					Owner.GetTableInfo(item.Table).SelectColumn(item.Column, item.Aggregate, value);
					SetOutput(item, true, false);
					SelectionItemData selectionItemData = Owner.Selection.FindNode(item.Table, item.Column);
					if(item.Aggregate != AggregationType.None)
						Owner.Selection.SetAggregated(selectionItemData, true);
					else
						Owner.Selection.SetSelected(selectionItemData, true, false);
				}
				else
					columnInfo.Alias = value;
				PatchGroupFilterString(item, value);
				item.alias = value;
			}
			void PatchGroupFilterString(QueryGridItemData item, string value) {
				if(string.IsNullOrEmpty(value))
					return;
				Owner.ModelQuery.GroupFilterString = GroupFilterPatcher.RenameColumn(Owner.ModelQuery.GroupFilterString, Owner.ModelQuery, item.ActualName, value);
			}
			void SetSortingTypeFull(QueryGridItemData item, SortingInfo.SortingDirection? value) {
				SortingInfo.SortingDirection? columnSorted = value;
				SortingInfoList sorting = Owner.ModelQuery.Sorting;
				if(item.sortingType != null && value.HasValue) {
					item.sortingType = value;
					sorting.First(info =>
						string.Equals(info.Table, item.Table, StringComparison.Ordinal) &&
						string.Equals(info.Column, item.Column, StringComparison.Ordinal)).Direction = (SortingInfo.SortingDirection)value;
				}
				else {
					item.sortingType = value;
					if(value.HasValue) {
						sorting.Add(item.Table, item.Column, value.Value);
						SetSortOrder(item, sorting.Count, false);
					}
					else {
						int index = sorting.FindIndex(
							info =>
								string.Equals(info.Table, item.Table, StringComparison.Ordinal) &&
								string.Equals(info.Column, item.Column, StringComparison.Ordinal));
						sorting.RemoveAt(index);
						SetSortOrder(item, null, false);
						for(int i = index; i < sorting.Count; i++)
							SetSortOrder(this.First(data => data.SortOrder == i + 2), i + 1, false);
						SortingInfo sorted = sorting.FirstOrDefault(
							info => {
								ColumnInfo columnInfo = Owner.GetColumnInfo(info.Table, info.Column);
								string columnName = columnInfo != null ? columnInfo.Name : info.Column;
								return string.Equals(info.Table, item.Table) &&
									   string.Equals(columnName, item.Column, StringComparison.Ordinal);
							});
						columnSorted = sorted != null ? sorted.Direction : (SortingInfo.SortingDirection?)null;
					}
				}
				SelectionItemData selectionItemData = Owner.Selection.FindNode(item.Table, item.Column);
				Owner.Selection.SetSortedAsc(selectionItemData, columnSorted == SortingInfo.SortingDirection.Ascending);
				Owner.Selection.SetSortedDesc(selectionItemData, columnSorted == SortingInfo.SortingDirection.Descending);
			}
			void SetSortOrderFull(QueryGridItemData item, int? value) {
				if(item.SortOrder == null || value == null)
					return;
				List<QueryGridItemData> sortQueue =
					this.Where(data => data.SortOrder != null).OrderBy(data => data.SortOrder).ToList();
				int val = value.Value;
				if(val < 1)
					val = 1;
				if(val > sortQueue.Count)
					val = sortQueue.Count;
				int oldValue = (int)item.SortOrder;
				if(val > oldValue)
					for(int i = oldValue; i < val ; i++)
						SetSortOrder(sortQueue[i], i, false);
				else
					for(int i = val - 1; i < oldValue; i++)
						SetSortOrder(sortQueue[i], i + 2, false);
				item.sortOrder = val;
				SortingInfoList sorting = Owner.ModelQuery.Sorting;
				SortingInfo sortingInfo = sorting[oldValue - 1];
				sorting.RemoveAt(oldValue - 1);
				sorting.Insert(val - 1, sortingInfo);
			}
			void SetAggregateFull(QueryGridItemData item, AggregationType value) {
				item.aggregate = value;
				ColumnInfo columnInfo = Owner.GetColumnInfo(item.Table, item.ActualName);
				if(columnInfo != null) {
					if(item.Output) {
						if(value != AggregationType.None) {
							if((item.Alias == null || IsAutoAlias(item.Alias, columnInfo.Aggregation, item.Column))) {
								string alias = Owner.CreateColumnAlias(string.Format("{0}_{1}", item.Aggregate, item.Column));
								columnInfo.Alias = alias;
								SetAlias(item, alias, false);
							}
						}
						else if(item.Alias != null && IsAutoAlias(item.Alias, columnInfo.Aggregation, item.Column)) {
							columnInfo.Alias = null;
							SetAlias(item, null, false);
						}
					}
					columnInfo.Aggregation = value;
					bool aggregated = false;
					bool selected = false;
					TableInfo tableInfo = Owner.GetTableInfo(item.Table);
					foreach(ColumnInfo column in tableInfo.SelectedColumns.Where(column => string.Equals(column.Name, item.Column, StringComparison.Ordinal))) {
						if(column.Aggregation == AggregationType.None)
							selected = true;
						else
							aggregated = true;
						if(selected && aggregated)
							break;
					}
					SelectionItemData selectionItemData = Owner.Selection.FindNode(item.Table, item.Column);
					Owner.Selection.SetAggregated(selectionItemData, aggregated);
					Owner.Selection.SetSelected(selectionItemData, selected, false);
				}
				if(value != AggregationType.None)
					SetGroupBy(item, false, true);
			}
			bool IsAutoAlias(string alias, AggregationType aggregate, string column) {
				string auto = string.Format("{0}_{1}", aggregate, column);
				if(string.Equals(alias, auto, StringComparison.Ordinal))
					return true;
				if(new Regex(string.Format(@"\A{0}_\d+\z", auto)).IsMatch(alias))
					return true;
				if(Owner.AliasFormatter == null || Owner.AliasFormatter.MaxColumnAliasLength == 0)
					return false;
				Match postfix = new Regex(@"_\d+\z").Match(alias);
				if(!postfix.Success)
					return false;
				return auto.StartsWith(alias.Substring(0, alias.Length - postfix.Length));
			}
			void SetGroupByFull(QueryGridItemData item, bool value) {
				item.groupBy = value;
				SelectionItemData selectionItemData = Owner.Selection.FindNode(item.Table, item.Column);
				GroupingInfoList grouping = Owner.ModelQuery.Grouping;
				if(value) {
					if(!grouping.Any(info => GroupingInfo.EqualityComparer.Equals(info, item.Table, item.Column)))
						grouping.Add(item.Table, item.Column);
					Owner.Selection.SetGroupedBy(selectionItemData, true);
					SetAggregate(item, AggregationType.None, true);
				}
				else {
					bool itWasLast = !this.Any(data => 
						data.GroupBy && 
						string.Equals(data.Table, item.Table, StringComparison.Ordinal) && 
						string.Equals(data.Column, item.Column, StringComparison.Ordinal));
					if(!itWasLast)
						return;
					grouping.RemoveAll(info => GroupingInfo.EqualityComparer.Equals(info, item.Table, item.Column));
					Owner.Selection.SetGroupedBy(selectionItemData, false);
				}
			}
			void SetColumnDataFull(QueryGridItemData item, ColumnDataItem value) {
				bool output = item.Output;
				SortingInfo.SortingDirection? sortingType = item.SortingType;
				int? sortOrder = item.SortOrder;
				bool groupBy = item.GroupBy;
				SetOutput(item, false, true);
				SetSortingType(item, null, true);
				SetGroupBy(item, false, true);
				item.columnData = value;
				SetOutput(item, output, true);
				if(sortingType != null) {
					SetSortingType(item, sortingType, true);
					SetSortOrder(item, sortOrder, true);
				}
				SetGroupBy(item, groupBy, true);
			}
		}
		readonly List owner;
		string alias;
		bool output;
		SortingInfo.SortingDirection? sortingType;
		int? sortOrder;
		bool groupBy;
		AggregationType aggregate;
		ColumnDataItem columnData;
		QueryGridItemData(List owner, string column, string table) : this(owner, column, table, null, false, null, null, false, AggregationType.None) { }
		public QueryGridItemData(List owner, string column, string table, string alias, AggregationType aggregate) : this(owner, column, table, alias, true, null, null, false, aggregate) { }
		QueryGridItemData(List owner, string column, string table, string alias, bool output, SortingInfo.SortingDirection? sortingType, int? sortOrder, bool groupBy, AggregationType aggregate) {
			this.owner = owner;
			this.columnData = new ColumnDataItem(table, column);
			this.alias = alias;
			this.output = output;
			this.sortingType = sortingType;
			this.sortOrder = sortOrder;
			this.groupBy = groupBy;
			this.aggregate = aggregate;
		}
		public ColumnDataItem ColumnData { get { return this.columnData; } set { this.owner.SetColumnData(this, value, true); } }
		public string Column { get { return this.columnData.Column; } }
		public string Table { get { return this.columnData.Table; } set { this.owner.SetColumnData(this, new ColumnDataItem(value, this.columnData.Column), false); } }
		public string Alias {
			get { return this.alias; }
			set { this.owner.SetAlias(this, string.IsNullOrEmpty(value) ? null : value, true); }
		}
		internal string ActualName { get { return this.alias ?? Column; } }
		public bool Output { get { return this.output; } set { this.owner.SetOutput(this, value, true); } }
		public SortingInfo.SortingDirection? SortingType { get { return this.sortingType; } set { this.owner.SetSortingType(this, value, true); } }
		public int? SortOrder { get { return this.sortOrder; } set { this.owner.SetSortOrder(this, value, true); } }
		public bool GroupBy { get { return this.groupBy; } set { this.owner.SetGroupBy(this, value, true); } }
		public AggregationType Aggregate { get { return this.aggregate; } set { this.owner.SetAggregate(this, value, true); } }
	}
}
