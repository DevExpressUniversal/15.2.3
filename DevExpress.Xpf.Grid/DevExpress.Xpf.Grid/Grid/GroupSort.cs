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
using System.Linq;
using System.Text;
using DevExpress.Data.Helpers;
using DevExpress.Data;
using DevExpress.Xpf.Data;
using DevExpress.XtraGrid;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System.Collections;
namespace DevExpress.Xpf.Grid.Helpers {
	public class BaseGridColumnData : BaseFilterData {
		GridControl grid;
		public BaseGridColumnData(GridControl grid) {
			this.grid = grid;
		}
		protected DataProviderBase DataProvider { get { return grid.DataProviderBase; } }
		protected GridControl Grid { get { return grid; } }
		protected DataViewBase View { get { return Grid.DataView; } }
		protected override void OnFillColumns() {
			foreach(GridColumn column in Grid.Columns) {
				DataColumnInfo colInfo = DataProvider.Columns[column.FieldName];
				if(colInfo == null) continue;
				GridDataColumnInfo dataInfo = CreateColumnInfo(column);
				if(!dataInfo.Required) continue;
				Columns[GetKey(colInfo)] = dataInfo;
			}
		}
		protected virtual GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			return new GridDataColumnInfo(this, column);
		}
		public override int GetSortIndex(object column) {
			GridColumn col = column as GridColumn;
			return Grid.ActualSortInfo.IndexOf(grid.ActualSortInfo[col.FieldName]);
		}
		public override int GroupCount { get { return Grid.ActualGroupCount; } }
		public override int SortCount { get { return Grid.ActualSortInfo.Count; } }
		protected internal void RaiseCustomSort(CustomColumnSortEventArgs e) {
			Grid.RaiseCustomColumnSort(e);
		}
		protected internal void RaiseCustomGroup(CustomColumnSortEventArgs e) {
			Grid.RaiseCustomColumnGroup(e);
		}
		protected internal string GetColumnDisplayText(int listSourceRowIndex, GridColumn column, object value) {
			object displayObject = View.GetDisplayObject(value, column);
			return Grid.RaiseCustomDisplayText(null, listSourceRowIndex, column, value, displayObject != null ? displayObject.ToString() : string.Empty);
		}
	}
	public class GridFilterData : BaseGridColumnData {
		public GridFilterData(GridControl grid) : base(grid) { }
		protected override GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			GridDataColumnInfo info = new GridDataColumnInfo(this, column);
			info.Required = column.ColumnFilterMode == ColumnFilterMode.DisplayText;
			return info;
		}
	}
	public class GridSearchFilterData : GridFilterData {
		public GridSearchFilterData(GridControl grid) : base(grid) { }
		protected override GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			IDataControllerSort dataControllerSort = (IDataControllerSort)View.DataProviderBase as IDataControllerSort;
			string[] columns = dataControllerSort.GetFindByPropertyNames();
			GridDataColumnInfo info = new GridDataColumnInfo(this, column);
			info.Required = columns != null && Array.IndexOf(columns, column.FieldName) != -1;
			return info;
		}
	}
	public class GridColumnSortData : BaseGridColumnData {
		public GridColumnSortData(GridControl grid) : base(grid) { }
		protected override GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			GridDataColumnSortInfo sortInfo = new GridDataColumnSortInfo(this, column);
			sortInfo.SortMode = column.GetSortMode();
			sortInfo.GroupInterval = column.GroupInterval;
			return sortInfo;
		}
		public GridDataColumnSortInfo GetSortInfo(DataColumnInfo column) {
			return (GetInfo(column) as GridDataColumnSortInfo);
		}
		protected override string[] GetOutlookLocalizedStrings() {
			return GridControlLocalizer.GetString(GridControlStringId.GridOutlookIntervals).Split(new char[] { ';' });
		}
	}
	public class GridDataColumnInfo : BaseGridColumnInfo {
		CustomColumnSortEventArgs sortArgs;
		public GridDataColumnInfo(BaseGridColumnData data, GridColumn column)
			: base(data, column) {
			this.sortArgs = new CustomColumnSortEventArgs(column, -1, -1, null, null, ColumnSortOrder.Ascending);
		}
		protected new BaseGridColumnData Data { get { return base.Data as BaseGridColumnData; } }
		protected CustomColumnSortEventArgs SortArgs { get { return sortArgs; } }
		public new GridColumn Column { get { return base.Column as GridColumn; } }
		public override string GetDisplayText(int listSourceIndex, object val) {
			return Data.GetColumnDisplayText(listSourceIndex, Column, val);
		}
		protected override int? RaiseCustomGroup(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder columnSortOrder) {
			SortArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, ColumnSortOrder.None);
			Data.RaiseCustomGroup(SortArgs);
			return SortArgs.GetSortResult();
		}
		protected override int? RaiseCustomSort(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder) {
			SortArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
			Data.RaiseCustomSort(SortArgs);
			return SortArgs.GetSortResult();
		}
	}
	public class GridDataColumnSortInfo : GridDataColumnInfo {
		public GridDataColumnSortInfo(GridColumnSortData data, GridColumn column)
			: base(data, column) {
		}
		public override bool Required {
			get {
				if(base.Required) return true;
				if(SortMode == ColumnSortMode.Custom || SortMode == ColumnSortMode.DisplayText) return true;
				if(GroupInterval != ColumnGroupInterval.Default && GroupInterval != ColumnGroupInterval.Value) return true;
				return false;
			}
			set { base.Required = value; }
		}
		public string GetColumnGroupFormatString() {
			DevExpress.Utils.FormatInfo fi = GetColumnGroupFormat();
			if(fi == null) return string.Empty;
			return fi.FormatString;
		}
	}
}
