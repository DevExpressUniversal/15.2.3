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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public class SelectionItemData : INotifyPropertyChanged {
		public class List : ItemDataList<SelectionItemData> {
			static readonly PropertyDescriptor selectedPropertyDescriptor;
			static readonly PropertyDescriptor aggregatedPropertyDescriptor;
			static readonly PropertyDescriptor sortedAscPropertyDescriptor;
			static readonly PropertyDescriptor sortedDescPropertyDescriptor;
			static readonly PropertyDescriptor groupedByPropertyDescriptor;
			static readonly PropertyDescriptor foreignKeyPropertyDescriptor;
			static readonly PropertyDescriptor namePropertyDescriptor;
			static readonly PropertyDescriptor conditionPropertyDescriptor;
			static List() {
				PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(SelectionItemData));
				selectedPropertyDescriptor = pdc["Selected"];
				aggregatedPropertyDescriptor = pdc["Aggregated"];
				sortedAscPropertyDescriptor = pdc["SortedAsc"];
				sortedDescPropertyDescriptor = pdc["SortedDesc"];
				groupedByPropertyDescriptor = pdc["GroupedBy"];
				foreignKeyPropertyDescriptor = pdc["ForeignKey"];
				namePropertyDescriptor = pdc["Name"];
				conditionPropertyDescriptor = pdc["Condition"];
			}	   
			int selectionItemsCnt;
			public List(QueryBuilderViewModel owner) : base(owner) { }
			public void Initialize(IEnumerable<TableInfo> tableInfos, IEnumerable<SortingInfo> sortingInfos, IEnumerable<RelationInfo> relationInfos, IEnumerable<GroupingInfo> groupingInfos) {
				Dictionary<string, HashSet<string>> allSortedAscColumns;
				Dictionary<string, HashSet<string>> allSortedDescColumns;
				GetAllSortedColumns(sortingInfos, out allSortedAscColumns, out allSortedDescColumns);
				Dictionary<string, HashSet<string>> allGroupedByColumns = GetAllGroupedByColumns(groupingInfos);
				IList<TableInfo> tables = tableInfos as IList<TableInfo> ?? tableInfos.ToList();
				Dictionary<string, HashSet<string>> columnsInRelations = tables.ToDictionary(table => table.ActualName, _ => new HashSet<string>());
				foreach(RelationInfo relationInfo in relationInfos.Where(info => info.KeyColumns.Count == 1))
					columnsInRelations[relationInfo.ParentTable].Add(relationInfo.KeyColumns[0].ParentKeyColumn);
				foreach(TableInfo table in tables) {
					string actualName = table.ActualName;
					HashSet<string> selectedColumns = new HashSet<string>();
					HashSet<string> aggregatedColumns = new HashSet<string>();
					HashSet<string> sortedAscColumns;
					HashSet<string> sortedDescColumns;
					HashSet<string> groupedByColumns;
					foreach(ColumnInfo column in table.SelectedColumns)
						(column.Aggregation == AggregationType.None ? selectedColumns : aggregatedColumns).Add(column.Name);
					if(!allSortedAscColumns.TryGetValue(actualName, out sortedAscColumns))
						sortedAscColumns = new HashSet<string>(Enumerable.Empty<string>());
					if(!allSortedDescColumns.TryGetValue(actualName, out sortedDescColumns))
						sortedDescColumns = new HashSet<string>(Enumerable.Empty<string>());
					if(!allGroupedByColumns.TryGetValue(actualName, out groupedByColumns))
						groupedByColumns = new HashSet<string>(Enumerable.Empty<string>());
					InitializeTable(table, selectedColumns, aggregatedColumns, sortedAscColumns, sortedDescColumns, groupedByColumns, columnsInRelations);
				}
			}
			static Dictionary<string, HashSet<string>> GetAllGroupedByColumns(IEnumerable<GroupingInfo> groupingInfos) {
				Dictionary<string, HashSet<string>> allGroupedByColumns = new Dictionary<string, HashSet<string>>();
				foreach(GroupingInfo groupingInfo in groupingInfos) {
					string table = groupingInfo.Table;
					HashSet<string> set;
					if(!allGroupedByColumns.TryGetValue(table, out set))
						allGroupedByColumns.Add(table, set = new HashSet<string>());
					set.Add(groupingInfo.Column);
				}
				return allGroupedByColumns;
			}
			static void GetAllSortedColumns(IEnumerable<SortingInfo> sortingInfos, out Dictionary<string, HashSet<string>> allSortedAscColumns,
				out Dictionary<string, HashSet<string>> allSortedDescColumns) {
				allSortedAscColumns = new Dictionary<string, HashSet<string>>();
				allSortedDescColumns = new Dictionary<string, HashSet<string>>();
				foreach(SortingInfo sortingInfo in sortingInfos) {
					string table = sortingInfo.Table;
					Dictionary<string, HashSet<string>> target =
						sortingInfo.Direction == SortingInfo.SortingDirection.Ascending
							? allSortedAscColumns
							: allSortedDescColumns;
					HashSet<string> set;
					if(!target.TryGetValue(table, out set))
						target.Add(table, set = new HashSet<string>());
					set.Add(sortingInfo.Column);
				}
			}
			void InitializeTable(TableInfo table, HashSet<string> selectedColumns, HashSet<string> aggregatedColumns, HashSet<string> sortedAscColumns, HashSet<string> sortedDescColumns, HashSet<string> groupedByColumns, Dictionary<string, HashSet<string>> columnsInRelations) {
				string actualName = table.ActualName;
				SelectionItemData tableSelectionData = AddNew(actualName, Owner.GetConditionString(table), true);
				bool anySelected = false;
				foreach(string column in Owner.GetColumns(table.Name).Select(column => column.Name)) {
					bool selected = selectedColumns.Contains(column);
					bool aggregated = aggregatedColumns.Contains(column);
					bool sortedAsc = sortedAscColumns.Contains(column);
					bool sortedDesc = sortedDescColumns.Contains(column);
					bool groupedBy = groupedByColumns.Contains(column);
					FKState foreignKey;
					if(!Owner.ColumnsWithFKs[table.Name].Contains(column))
						foreignKey = FKState.NoFK;
					else if(columnsInRelations[actualName].Contains(column) &&
							Owner.ModelQuery.Relations.Any(
								rel =>
									rel.KeyColumns.Count == 1 &&
									string.Equals(rel.ParentTable, table.ActualName, StringComparison.Ordinal) &&
									string.Equals(rel.KeyColumns[0].ParentKeyColumn, column, StringComparison.Ordinal) &&
									Owner.IsJoinedWithFK(rel)))
						foreignKey = FKState.AlreadyJoined;
					else
						foreignKey = FKState.CanBeJoined;
					AddNew(tableSelectionData.Id, column, null, selected, aggregated, sortedAsc, sortedDesc,
						groupedBy,
						foreignKey);
					if(!selected)
						SetSelected(tableSelectionData, null, false);
					else
						anySelected = true;
				}
				if(!anySelected)
					SetSelected(tableSelectionData, false, false);
			}
			public SelectionItemData AddNew(string name, ConditionStringInfo condition, bool? selected) {
				SelectionItemData item = new SelectionItemData(this, this.selectionItemsCnt++, name, condition, selected);
				Add(item);
				return item;
			}
			public void AddNew(int parent, string name, ConditionStringInfo condition, bool? selected, bool aggregated, bool sortedAsc, bool sortedDesc, bool groupedBy, FKState foreignKey) {
				Add(new SelectionItemData(this, this.selectionItemsCnt++, parent, name, condition, selected, aggregated, sortedAsc, sortedDesc, groupedBy, foreignKey));
			}
			internal void SetSelected(SelectionItemData item, bool? value, bool update) {
				SetItemProperty(item, value,
					EqualityComparer<bool?>.Default,
					selectedPropertyDescriptor,
					data => data.Selected,
					update 
						? (Action<SelectionItemData, bool?>)SetSelectedFull 
						: SetSelectedLight);
			}
			void SetSelectedFull(SelectionItemData item, bool? value) {
				int itemParentId = item.Parent;
				SelectionItemData parent = this.FirstOrDefault(child => child.Id == itemParentId);
				SetSelectedValue(item, value);
				if(parent == null) {
					string tableName = item.Name;
					foreach(SelectionItemData child in this.Where(child => child.Parent == item.Id)) {
						if(child.selected == value)
							continue;
						SetSelectedValue(child, value);
						SetColumnSelected(child, value, tableName);
					}
				}
				else {
					SetSelected(parent,
						this.Where(child => child.Parent == itemParentId).All(child => child.Selected == value)
							? value
							: null, false);
					SetColumnSelected(item, value, parent.Name);
				}
			}
			void SetColumnSelected(SelectionItemData item, bool? value, string tableName) {
				string columnName = item.Name;
				Debug.Assert(value != null, "value != null");
				if(value.Value) {
					string alias = null;
					if(Owner.GetColumnInfo(columnName) != null)
						alias = Owner.CreateColumnAlias(string.Format("{0}_{1}", tableName, columnName));
					Owner.GetTableInfo(tableName).SelectColumn(columnName, AggregationType.None, alias);
					QueryGridItemData row =
						Owner.QueryGrid.FirstOrDefault(
							data =>
								!data.Output && data.Aggregate == AggregationType.None &&
								string.Equals(data.Table, tableName, StringComparison.Ordinal) &&
								string.Equals(data.Column, columnName, StringComparison.Ordinal));
					if(row == null)
						Owner.QueryGrid.Add(new QueryGridItemData(Owner.QueryGrid, columnName, tableName, alias, AggregationType.None));
					else {
						Owner.QueryGrid.SetOutput(row, true, false);
						Owner.QueryGrid.SetAlias(row, alias, false);
					}
				}
				else {
					Owner.GetTableInfo(tableName)
						.SelectedColumns.RemoveAll(
							columnInfo =>
								string.Equals(columnInfo.Name, columnName, StringComparison.Ordinal) &&
								columnInfo.Aggregation == AggregationType.None);
					HashSet<QueryGridItemData> toRemove = new HashSet<QueryGridItemData>();
					foreach(
						QueryGridItemData row in
							Owner.QueryGrid.Where(
								row =>
									row.Aggregate == AggregationType.None &&
									string.Equals(row.Table, tableName, StringComparison.Ordinal) &&
									string.Equals(row.Column, columnName, StringComparison.Ordinal)))
						if(row.SortingType == null && !row.GroupBy)
							toRemove.Add(row);
						else
							Owner.QueryGrid.SetOutput(row, false, false);
					Owner.QueryGrid.RemoveAll(toRemove.Contains);
				}
			}
			void SetSelectedLight(SelectionItemData item, bool? value) { SetSelectedCore(item, value); }
			void SetSelectedCore(SelectionItemData item, bool? value) {
				SetSelectedValue(item, value);
				SelectionItemData parent = this.FirstOrDefault(child => child.Id == item.Parent);
				if(parent == null)
					return;
				SetSelected(parent,
					this.Where(child => child.Parent == item.Parent).All(child => child.Selected == value)
						? value
						: null, false);
			}
			void SetSelectedValue(SelectionItemData item, bool? value) {
				item.selected = value;
				item.RaisePropertyChanged("Selected");
			}
			internal void SetAggregated(SelectionItemData item, bool value) {
				SetItemProperty(item, value, aggregatedPropertyDescriptor, 
					data => data.Aggregated,
					(data, b) => data.aggregated = b);
				item.RaisePropertyChanged("Aggregated");
			}
			internal void SetSortedAsc(SelectionItemData item, bool value) {
				SetItemProperty(item, value, sortedAscPropertyDescriptor,
					data => data.SortedAsc,
					(data, b) => data.sortedAsc = b);
				item.RaisePropertyChanged("SortedAsc");
			}
			internal void SetSortedDesc(SelectionItemData item, bool value) {
				SetItemProperty(item, value, sortedDescPropertyDescriptor,
					data => data.SortedDesc,
					(data, b) => data.sortedDesc = b);
				item.RaisePropertyChanged("SortedDesc");
			}
			internal void SetGroupedBy(SelectionItemData item, bool value) {
				SetItemProperty(item, value, groupedByPropertyDescriptor,
					data => data.GroupedBy,
					(data, b) => data.groupedBy = b);
				item.RaisePropertyChanged("GroupedBy");
			}
			internal void SetForeignKey(SelectionItemData item, FKState value) {
				SetItemProperty(item, value, EqualityComparer<FKState>.Default, foreignKeyPropertyDescriptor,
					data => data.ForeignKey, 
					(data, state) => data.foreignKey = state);
				item.RaisePropertyChanged("ForeignKey");
			}
			internal void SetName(SelectionItemData item, string value, bool update) {
				if(item.Parent >=  0)
					return;
				SetItemProperty(item, value, namePropertyDescriptor,
					data => data.Name,
					update
						? SetNameFull
						: (Action<SelectionItemData, string>)((data, s) => data.name = s));
			}
			internal void SetCondition(SelectionItemData item, ConditionStringInfo condition) {
				SetItemProperty(item, condition,
					new ConditionStringInfoComparer(), 
					conditionPropertyDescriptor,
					data => data.condition,
					(data, value) => data.condition = value);
				item.RaisePropertyChanged("Condition");
			}
			void SetNameFull(SelectionItemData item, string value) {
				if(this.Any(data => data.Parent == -1 && string.Equals(data.Name, value)))
					throw new InvalidNameException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageDuplicateItemName), value));
				string oldValue = item.Name;
				foreach(QueryGridItemData gridItemData in Owner.QueryGrid.Where(gridItemData => string.Equals(gridItemData.Table, oldValue, StringComparison.Ordinal)))
					gridItemData.Table = value;
				foreach(SortingInfo sortingInfo in Owner.ModelQuery.Sorting.Where(sortingInfo => string.Equals(sortingInfo.Table, oldValue, StringComparison.Ordinal)))
					sortingInfo.Table = value;
				foreach(GroupingInfo groupingInfo in Owner.ModelQuery.Grouping.Where(groupingInfo => string.Equals(groupingInfo.Table, oldValue, StringComparison.Ordinal)))
					groupingInfo.Table = value;
				foreach(RelationInfo relationInfo in Owner.ModelQuery.Relations) {
					if(string.Equals(relationInfo.ParentTable, oldValue, StringComparison.Ordinal)) {
						relationInfo.ParentTable = value;
						FindTableNode(relationInfo.NestedTable).condition =
							Owner.GetConditionString(Owner.GetTableInfo(relationInfo.NestedTable));
					}
					else if(string.Equals(relationInfo.NestedTable, oldValue, StringComparison.Ordinal))
						relationInfo.NestedTable = value;
				}
				Owner.GetTableInfo(oldValue).Alias = value;
				item.name = value;
				Owner.UpdateConditions();
			}
			internal SelectionItemData FindTableNode(string table) { return FindNode(-1, table); }
			internal SelectionItemData FindNode(string table, string column) {
				SelectionItemData tableItem = FindNode(-1, table);
				return tableItem == null ? null : FindNode(tableItem.Id, column);
			}
			SelectionItemData FindNode(int parentId, string name) {
				return
					this.FirstOrDefault(
						item => item.Parent == parentId && string.Equals(item.Name, name, StringComparison.Ordinal));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public enum FKState {
			NoFK,
			CanBeJoined,
			AlreadyJoined
		};
		readonly List owner;
		readonly int id;
		readonly int parent;
		bool? selected;
		string name;
		ConditionStringInfo condition;
		bool aggregated;
		bool sortedAsc;
		bool sortedDesc;
		bool groupedBy;
		FKState foreignKey;
		SelectionItemData(List owner, int id, string name, ConditionStringInfo condition, bool? selected) : this(owner, id, -1, name, condition, selected, false, false, false, false, FKState.NoFK) { }
		SelectionItemData(List owner, int id, int parent, string name, ConditionStringInfo condition, bool? selected, bool aggregated, bool sortedAsc, bool sortedDesc, bool groupedBy, FKState foreignKey) {
			this.owner = owner;
			this.id = id;
			this.parent = parent;
			this.name = name;
			this.condition = condition;
			this.selected = selected;
			this.aggregated = aggregated;
			this.sortedAsc = sortedAsc;
			this.sortedDesc = sortedDesc;
			this.groupedBy = groupedBy;
			this.foreignKey = foreignKey;
		}
		public int Id { get { return this.id; } }
		public int Parent { get { return this.parent; } }
		public bool? Selected { get { return this.selected; } set { this.owner.SetSelected(this, value, true); } }
		public string Name { get { return this.name; } set { this.owner.SetName(this, value, true); } }
		public ConditionStringInfo Condition { get { return this.condition; } }
		public bool Aggregated { get { return this.aggregated; } }
		public bool SortedAsc { get { return this.sortedAsc; } }
		public bool SortedDesc { get { return this.sortedDesc; } }
		public bool GroupedBy { get { return this.groupedBy; } }
		public FKState ForeignKey { get { return this.foreignKey; } }
	}
}
