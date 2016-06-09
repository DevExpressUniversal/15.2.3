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
using DevExpress.Data.Helpers;
using DevExpress.Utils;
namespace DevExpress.Map.Native {
public enum AggregationSortOrder {
	Ascending,
	Descending
}
public class GroupInfo {
	internal const AggregationSortOrder DefaultSortOrder = AggregationSortOrder.Ascending;
	readonly string columnName;
	readonly AggregationSortOrder sortOrder;
	public string ColumnName { get { return columnName; } }
	public AggregationSortOrder SortOrder { get { return sortOrder; } }
	public GroupInfo()
		: this("") {
	}
	public GroupInfo(string column)
		: this(column, DefaultSortOrder) {
	}
	public GroupInfo(string column, AggregationSortOrder sortOrder) {
		this.columnName = column;
		this.sortOrder = sortOrder;
	}
	public static bool operator ==(GroupInfo a, GroupInfo b) {
		if(Object.ReferenceEquals(a, b))
			return true;
		if(Object.Equals(a, null) || Object.Equals(b, null))
			return false;
		if(a.SortOrder != b.SortOrder)
			return false;
		return String.Compare(a.ColumnName, b.ColumnName) == 0;
	}
	public static bool operator !=(GroupInfo a, GroupInfo b) {
		return !(a == b);
	}
	public override bool Equals(object obj) {
		if(Object.ReferenceEquals(this, obj))
			return true;
		GroupInfo other = obj as GroupInfo;
		if(Object.ReferenceEquals(other, null))
			return false;
		return ColumnName == other.ColumnName && SortOrder == other.SortOrder;
	}
	public override int GetHashCode() {
		return !string.IsNullOrEmpty(ColumnName) ? ColumnName.GetHashCode() ^ SortOrder.GetHashCode() : base.GetHashCode();
	}
}
public class GroupInfoCollection : NotificationCollection<GroupInfo> {
}
public class MapDataAggregator : IMapDataAggregator {
	SummaryItemType summaryFunction;
	DataController controller;
	GroupInfoCollection aggregationGroups;
	string summaryColumn;
	public DataController Controller { get { return controller; } }
	public GroupInfoCollection AggregationGroups { get { return aggregationGroups; } }
	public string SummaryColumn { get { return summaryColumn; } set { summaryColumn = value; } }
	public SummaryItemType SummaryFunction { get { return summaryFunction; } set { summaryFunction = value; } }
	public MapDataAggregator(SummaryItemType summaryFunction) {
		this.summaryFunction = summaryFunction;
		aggregationGroups = new GroupInfoCollection();
	}
	public void Aggregate(DataController controller) {
		if(controller == null) return;
		this.controller = controller;
		AggregateCore();
	}
	protected internal virtual void AggregateCore() {
		SummaryItemType summaryItemType = summaryFunction;
		if(summaryItemType == SummaryItemType.None)
			return;
		ApplySortGroup(AggregationGroups, SummaryColumn, summaryItemType);
	}
	protected void ApplySortGroup(GroupInfoCollection groups, string summaryColumn, SummaryItemType summaryType) {
		int count = groups.Count;
		List<DataColumnSortInfo> sortInfo = new List<DataColumnSortInfo>();
		for(int i = 0; i < count; i++) {
			GroupInfo groupInfo = groups[i];
			if(CanSort(groupInfo)) {
				Data.ColumnSortOrder sortOrder = groupInfo.SortOrder == AggregationSortOrder.Ascending ? Data.ColumnSortOrder.Ascending : Data.ColumnSortOrder.Descending;
				sortInfo.Add(new DataColumnSortInfo(Controller.Columns[groupInfo.ColumnName], sortOrder));
			}
		}
		Controller.SortInfo.ClearAndAddRange(sortInfo.ToArray(), count);
		Controller.GroupSummary.Clear();
		DataColumnInfo groupColumn = Controller.Columns[summaryColumn];
		Controller.GroupSummary.Add(new SummaryItem(groupColumn, summaryType));
	}
	public bool CanSort(GroupInfo groupInfo) {
		return !string.IsNullOrEmpty(groupInfo.ColumnName);
	}
}
}
