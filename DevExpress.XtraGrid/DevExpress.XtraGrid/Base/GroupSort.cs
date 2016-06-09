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
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data.Helpers;
namespace DevExpress.XtraGrid.Helpers {
	public class GridColumnData : BaseFilterData {
		ColumnView view;
		public GridColumnData(ColumnView view) {
			this.view = view;
		}
		public ColumnView View { get { return view; } }
		public GridDataColumnInfo GetInfo(GridColumn column) {
			if(column == null) return null;
			return GetInfo(column.ColumnInfo) as GridDataColumnInfo;
		}
		protected override object GetKey(DataColumnInfo column) { return column; }
		protected override void OnFillColumns() {
			foreach(GridColumn column in View.Columns) {
				if(column.ColumnInfo == null) continue;
				GridDataColumnInfo info = CreateColumnInfo(column);
				if(!info.Required) continue;
				info.EditViewInfo = View.CreateColumnEditViewInfo(column, DataController.InvalidRow);
				Columns[GetKey(column.ColumnInfo)] = info;
				if(column.ColumnInfo != column.ColumnInfoSort && column.ColumnInfoSort != null) {
					Columns[GetKey(column.ColumnInfoSort)] = info;
				}
			}
		}
		protected virtual GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			return new GridDataColumnInfo(this, column);
		}
		public override int SortCount { get { return View.SortInfo.Count; } }
		public override int GroupCount { get { return View.SortInfo.GroupCount; } }
		public override int GetSortIndex(object column) {
			GridColumn col = (GridColumn)column;
			return View.SortInfo.IndexOf(View.SortInfo[col]);
		}
	}
	public class GridDataColumnInfo : BaseGridColumnInfo {
		CustomColumnSortEventArgs sortArgs;
		public GridDataColumnInfo(GridColumnData data, GridColumn column) : base(data, column) {
			this.sortArgs = new CustomColumnSortEventArgs(Column, null, null, ColumnSortOrder.Ascending);
		}
		public override void Dispose() {
			if(EditViewInfo != null) EditViewInfo.Dispose();
			this.EditViewInfo = null;
			base.Dispose();
		}
		protected new GridColumnData Data { get { return base.Data as GridColumnData; } }
		public BaseEditViewInfo EditViewInfo = null;
		public ColumnView View { get { return Data.View; } }
		public new GridColumn Column { get { return base.Column as GridColumn; } }
		public override string GetDisplayText(int listSourceIndex, object val) {
			EditViewInfo.EditValue = val;
			EditViewInfo.SetDisplayText(View.RaiseCustomColumnDisplayText(listSourceIndex, DataController.InvalidRow, Column, val, EditViewInfo.DisplayText, false));
			return EditViewInfo.DisplayText;
		}
		protected override int? RaiseCustomSort(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder) {
			this.sortArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
			View.RaiseCustomColumnSort(this.sortArgs);
			return this.sortArgs.GetSortResult();
		}
		protected override int? RaiseCustomGroup(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder columnSortOrder) {
			this.sortArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, ColumnSortOrder.None);
			View.RaiseCustomColumnGroup(this.sortArgs);
			return this.sortArgs.GetSortResult();
		}
	}
	public class GridFilterData : GridColumnData {
		public GridFilterData(ColumnView view) : base(view) { }
		protected override GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			GridDataColumnInfo info = new GridDataColumnInfo(this, column);
			info.Required = column.GetFilterMode() == ColumnFilterMode.DisplayText;
			return info;
		}
	}
	public class GridFindFilterData : GridFilterData {
		public GridFindFilterData(ColumnView view) : base(view) { 
		}
		protected override GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			string[] columns = ((IDataControllerSort)View).GetFindByPropertyNames();
			GridDataColumnInfo info = new GridDataColumnInfo(this, column);
			info.Required = columns != null && Array.IndexOf(columns, column.FieldName) != -1;
			return info;
		}
	}
	public class GridSortData : GridColumnData {
		public GridSortData(ColumnView view) : base(view) {
		}
		public GridDataColumnSortInfo GetSortInfo(GridColumn column) { return GetInfo(column) as GridDataColumnSortInfo; } 
		public GridDataColumnSortInfo GetSortInfo(DataColumnInfo column) { return GetInfo(column) as GridDataColumnSortInfo; } 
		protected override GridDataColumnInfo CreateColumnInfo(GridColumn column) {
			GridDataColumnSortInfo info = new GridDataColumnSortInfo(this, column);
			info.SortMode = column.GetSortMode();
			info.GroupInterval = GetColumnGroupInterval(column);
			return info;
		}
		protected ColumnGroupInterval GetColumnGroupInterval(GridColumn column) {
			return column.GetGroupInterval();
		}
		protected override string[] GetOutlookLocalizedStrings() {
			return GridLocalizer.Active.GetLocalizedString(GridStringId.GridOutlookIntervals).Split(';'); 
		}
	}
	public class GridDataColumnSortInfo : GridDataColumnInfo {
		public GridDataColumnSortInfo(GridSortData columnData, GridColumn column) : base(columnData, column) {
		}
		public override bool Required {
			get {
				if(base.Required) return true;
				if(SortMode == ColumnSortMode.DisplayText || SortMode == ColumnSortMode.Custom) return true;
				if(GroupInterval != ColumnGroupInterval.Default && GroupInterval != ColumnGroupInterval.Value) return true;
				return false;
			}
			set { base.Required = value; }
		}
		public override FormatInfo GetColumnGroupFormat() {
			if(Column.GroupFormat == null || Column.GroupFormat.IsEmpty) return GetDefaultFormat();
			return Column.GroupFormat;
		}
	}
}
