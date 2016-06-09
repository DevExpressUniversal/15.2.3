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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data.PivotGrid {
	public class PivotColumnInfo : DataColumnSortInfo {
		public static PivotColumnInfo CreatePivotColumnInfo(DataColumnInfo dataColumnInfo, ColumnSortOrder sortOrder, PivotGridFieldBase field) {
			if(dataColumnInfo == null)
				return null;
			PivotColumnInfo columnInfo = new PivotColumnInfo(dataColumnInfo, sortOrder);
			columnInfo.Tag = field;
			return columnInfo;
		}
		DataColumnInfo sortbyColumn;
		PivotSummaryType summaryType;
		PivotSummaryType sortbySummaryType;
		List<PivotSortByCondition> sortbyCondition;
		int showTopRows;
		bool showTopRowsAbsolute = true;
		bool showOthersValue;
		object tag;
		object sortByColumnField;
		public PivotColumnInfo(DataColumnInfo columnInfo) : this(columnInfo, ColumnSortOrder.Ascending) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder) : this(columnInfo, sortOrder, null, PivotSummaryType.Sum) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, int showTopRows)
			: this(columnInfo, sortOrder, null, PivotSummaryType.Sum, showTopRows) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType)
			: this(columnInfo, sortOrder, sortbyColumn, sortbySummaryType, 0) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn,
			PivotSummaryType sortbySummaryType, int showTopRows)
			: this(columnInfo, PivotSummaryType.Sum, sortOrder, sortbyColumn, null, sortbySummaryType,
			null, showTopRows, true, false, false, false) { }
		public PivotColumnInfo(DataColumnInfo columnInfo, PivotSummaryType summaryType, ColumnSortOrder sortOrder,
			DataColumnInfo sortbyColumn, object sortByColumnField,
			PivotSummaryType sortbySummaryType, List<PivotSortByCondition> sortbyCondition,
			int showTopRows, bool showTopRowsAbsolute, bool showOthersValue, bool runningSummary,
			bool crossGroupRunningSummary)
			: base(columnInfo, sortOrder) {
			this.sortbyColumn = sortbyColumn;
			this.sortByColumnField = sortByColumnField;
			this.sortbySummaryType = sortbySummaryType;
			this.sortbyCondition = sortbyCondition;
			this.showTopRows = showTopRows;
			this.showTopRowsAbsolute = showTopRowsAbsolute;
			this.showOthersValue = showOthersValue;
			this.summaryType = summaryType;
			RunningSummary = runningSummary;
			CrossGroupRunningSummary = crossGroupRunningSummary;
		}
		public bool ContainsSortSummaryOrOthersValue { get { return ContainsSortSummary || ShowOthersValue; } }
		public bool ContainsSortSummary { get { return SortbyColumn != null; } }
		public bool ContainsSortSummaryConditions { get { return SortbyConditions != null && SortbyConditions.Count > 0; } }
		public PivotSummaryType SummaryType { get { return summaryType; } }
		public DataColumnInfo SortbyColumn { get { return sortbyColumn; } }
		public object SortByColumnField { get { return sortByColumnField; } }
		public PivotSummaryType SortbySummaryType { get { return sortbySummaryType; } }
		public List<PivotSortByCondition> SortbyConditions { get { return sortbyCondition; } }
		public int ShowTopRows { get { return showTopRows; } }
		public bool ShowTopRowsAbsolute { get { return showTopRowsAbsolute; } }
		public bool ShowOthersValue { get { return showOthersValue; } }
		public int GetTopRowsCount(int rowCount) {
			if(ShowTopRows <= 0) return 0;
			int count = ShowTopRows;
			if(!ShowTopRowsAbsolute) {
				if(count > 100)
					count = 100;
				int prevCount = count;
				count = (int)(prevCount * rowCount / 100);
				if((int)(count * 100 / prevCount) != rowCount) {
					count++;
				}
			}
			return rowCount > count ? count : rowCount;
		}
		public object Tag { get { return tag; } set { tag = value; } }
		public PivotColumnInfo Clone(ColumnSortOrder sortOrder) {
			return Clone(ColumnInfo, sortOrder);
		}
		public PivotColumnInfo Clone(DataColumnInfo columnInfo) {
			return Clone(columnInfo, SortOrder);
		}
		PivotColumnInfo Clone(DataColumnInfo dataColumnInfo, ColumnSortOrder sortOrder) {
			PivotColumnInfo columnInfo = new PivotColumnInfo(dataColumnInfo, summaryType, sortOrder, SortbyColumn,
				SortByColumnField, SortbySummaryType, SortbyConditions, ShowTopRows, ShowTopRowsAbsolute, ShowOthersValue,
				RunningSummary, CrossGroupRunningSummary);
			columnInfo.Tag = tag;
			return columnInfo;
		}
	}
	public class PivotColumnInfoCollection : ColumnInfoNotificationCollection {
		public PivotColumnInfoCollection(DataControllerBase controller) : this(controller, null) { }
		public PivotColumnInfoCollection(DataControllerBase controller, CollectionChangeEventHandler collectionChanged) : base(controller, collectionChanged) { }
		public PivotColumnInfo this[int index] { get { return (PivotColumnInfo)List[index]; } }
		public void ClearAndAddRange(PivotColumnInfo[] columnInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(columnInfos);
			} finally {
				EndUpdate();
			}
		}
		protected internal void ClearAndAddRangeSilent(PivotColumnInfo[] columnInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(columnInfos);
			} finally {
				CancelUpdate();
			}
		}
		public void AddRange(PivotColumnInfo[] columnInfos) {
			BeginUpdate();
			try {
				foreach(PivotColumnInfo columnInfo in columnInfos) {
					if(columnInfo == null) continue;
					List.Add(columnInfo);
				}
			} finally {
				EndUpdate();
			}
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo) {
			return Add(columnInfo, ColumnSortOrder.Ascending);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder) {
			return Add(columnInfo, sortOrder, null, PivotSummaryType.Sum);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, int showTopRows) {
			return Add(columnInfo, sortOrder, null, PivotSummaryType.Sum, showTopRows);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType) {
			return Add(columnInfo, ColumnSortOrder.Ascending, sortbyColumn, sortbySummaryType, 0);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, DataColumnInfo sortbyColumn, PivotSummaryType sortbySummaryType, int showTopRows) {
			return Add(columnInfo, ColumnSortOrder.Ascending, sortbyColumn, sortbySummaryType, showTopRows);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn,
			PivotSummaryType sortbySummaryType) {
			return Add(columnInfo, sortOrder, sortbyColumn, sortbySummaryType, 0);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn,
			PivotSummaryType sortbySummaryType, int showTopRows) {
			return Add(columnInfo, PivotSummaryType.Sum, sortOrder, sortbyColumn, null, sortbySummaryType, null,
				showTopRows, true, false, false, false);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, ColumnSortOrder sortOrder, DataColumnInfo sortbyColumn,
			PivotSummaryType sortbySummaryType, int showTopRows, bool showTopRowsAbsolute, bool showOthersValue) {
			return Add(columnInfo, PivotSummaryType.Sum, sortOrder, sortbyColumn, null, sortbySummaryType, null,
				showTopRows, showTopRowsAbsolute, showOthersValue, false, false);
		}
		public PivotColumnInfo Add(DataColumnInfo columnInfo, PivotSummaryType summaryType, ColumnSortOrder sortOrder,
			DataColumnInfo sortbyColumn, object sortByColumnField,
			PivotSummaryType sortbySummaryType, List<PivotSortByCondition> sortbyConditions,
			int showTopRows, bool showTopRowsAbsolute, bool showOthersValue, bool runningSummary,
			bool crossGroupRunningSummary) {
			PivotColumnInfo pivotColumnInfo = new PivotColumnInfo(columnInfo, summaryType, sortOrder, sortbyColumn,
				sortByColumnField, sortbySummaryType, sortbyConditions, showTopRows, showTopRowsAbsolute, showOthersValue,
				runningSummary, crossGroupRunningSummary);
			List.Add(pivotColumnInfo);
			return pivotColumnInfo;
		}
		public bool IsEquals(PivotColumnInfoCollection collection) {
			if(collection == null || collection.Count != Count) return false;
			for(int i = 0; i < Count; i++) {
				if(!this[i].IsEquals(collection[i]))
					return false;
			}
			return true;
		}
		public void ChangeSortOrder(int index) {
			if(index < 0 || index >= Count) return;
			InnerList[index] = this[index].Clone(this[index].SortOrder == ColumnSortOrder.Ascending ? ColumnSortOrder.Descending : ColumnSortOrder.Ascending);
		}
		public PivotColumnInfo[] ToArray() {
			return (PivotColumnInfo[])InnerList.ToArray(typeof(PivotColumnInfo));
		}
		protected override DataColumnInfo GetColumnInfo(int index) { return this[index].ColumnInfo; }
	}
}
