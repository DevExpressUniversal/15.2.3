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
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Localization;
using DevExpress.Web.Rendering;
using DevExpress.XtraGrid;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Web.Data;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using DevExpress.Data.IO;
namespace DevExpress.Web.Internal {
	public class GridFilterData : BaseFilterData {
		public GridFilterData(ASPxGridBase grid) {
			Grid = grid;
		}
		public ASPxGridBase Grid { get; private set; }
		protected override void OnFillColumns() {
			foreach(IWebGridDataColumn column in Grid.ColumnHelper.AllDataColumns) {
				if(string.IsNullOrEmpty(column.FieldName)) continue;
				GridColumnInfo info = CreateColumnInfo(column);
				if(!info.Required) continue;
				Columns[column.FieldName] = info;
			}
		}
		protected virtual GridColumnInfo CreateColumnInfo(IWebGridDataColumn column) {
			GridColumnInfo info = new GridColumnInfo(this, column);
			info.Required = column.Adapter.FilterMode == ColumnFilterMode.DisplayText;
			return info;
		}
		public override int SortCount { get { return Grid.SortCount; } }
		public override int GroupCount { get { return Grid.GroupCountInternal; } }
		public override int GetSortIndex(object column) {
			var dataColumn = column as IWebGridDataColumn;
			return dataColumn != null ? Grid.SortedColumns.IndexOf(dataColumn) : -1;
		}
		public GridColumnInfo GetInfo(IWebGridDataColumn column) {
			return GetInfoCore(column.FieldName) as GridColumnInfo;
		}
		protected override object GetKey(DataColumnInfo column) { return column.Name; }
	}
	public class GridSearchFilterData : GridFilterData {
		public GridSearchFilterData(ASPxGridBase grid)
			: base(grid) {
		}
		protected override GridColumnInfo CreateColumnInfo(IWebGridDataColumn column) {
			var info = new GridColumnInfo(this, column);
			info.Required = Grid.ColumnHelper.SearchPanelColumns.Contains(column);
			return info;
		}
	}
	public class GridViewSortData : GridSortData {
		public GridViewSortData(ASPxGridView grid)
			: base(grid) {
		}
		protected override GridColumnInfo CreateColumnInfo(IWebGridDataColumn column) {
			var info = base.CreateColumnInfo(column);
			info.GroupInterval = GetColumnGroupInterval(column as GridViewDataColumn);
			return info;
		}
		protected virtual ColumnGroupInterval GetColumnGroupInterval(GridViewDataColumn column) {
			var edit = Grid.RenderHelper.GetColumnEdit(column);
			if(edit == null) return ColumnGroupInterval.Value;
			ColumnGroupInterval group = column.Settings.GroupInterval;
			if(group == ColumnGroupInterval.Default) {
				if(edit is DateEditProperties) {
					group = ColumnGroupInterval.Date;
				}
			}
			if(group == ColumnGroupInterval.Default) {
				group = ColumnGroupInterval.Value;
			}
			return group;
		}
		protected override string[] GetOutlookLocalizedStrings() {
			return new string[] {
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_Older),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_LastMonth),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_EarlierThisMonth),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_ThreeWeeksAgo),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_TwoWeeksAgo),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_LastWeek),
				"", "", "", "", "", "", "",
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_Yesterday),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_Today),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_Tomorrow),
				"", "", "", "", "", "", "",
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_NextWeek),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_TwoWeeksAway),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_ThreeWeeksAway),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_LaterThisMonth),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_NextMonth),
				ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Outlook_BeyondNextMonth)
			};
		}
	}
	public class GridSortData : GridFilterData {
		public GridSortData(ASPxGridBase grid)
			: base(grid) {
		}
		protected override GridColumnInfo CreateColumnInfo(IWebGridDataColumn column) {
			GridColumnSortInfo info = new GridColumnSortInfo(this, column);
			info.SortMode = GetSortMode(column);
			return info;
		}
		protected virtual ColumnSortMode GetSortMode(IWebGridDataColumn column) {
			var edit = Grid.RenderHelper.GetColumnEdit(column);
			if(edit == null) return ColumnSortMode.Value;
			var sort = column.Adapter.Settings.SortMode;
			if(sort == ColumnSortMode.Default)
				sort = Grid.SettingsBehavior.SortMode;
			if(sort == ColumnSortMode.Default)
				sort = ColumnSortMode.Value;
			return sort;
		}
	}
	public class GridColumnSortInfo : GridColumnInfo {
		public GridColumnSortInfo(GridSortData columnData, IWebGridDataColumn column)
			: base(columnData, column) {
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
	}
	public class GridColumnInfo : BaseGridColumnInfo, IValueProvider {
		public GridColumnInfo(GridFilterData data, IWebGridDataColumn column)
			: base(data, column) {
			Properties = RenderHelper.GetColumnEdit(Column);
			SortArgs = Grid.CreateCustomColumnSortEventArgs(Column, null, null, ColumnSortOrder.None);
		}
		public new GridFilterData Data { get { return base.Data as GridFilterData; } }
		protected ASPxGridBase Grid { get { return Data.Grid; } }
		protected GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		public new IWebGridDataColumn Column { get { return base.Column as IWebGridDataColumn; } }
		public EditPropertiesBase Properties { get; private set; }
		protected GridCustomColumnSortEventArgs SortArgs { get; private set; }
		public override string GetDisplayText(int listSourceIndex, object val) {
			this.listSourceRow = listSourceIndex;
			var args = RenderHelper.TextBuilder.GetDisplayControlArgsCore(Column, -1, this, Data.Grid, val);
			args.DecodeDisplayFormat = true;
			string res = Properties.GetDisplayText(args, false); 
			this.listSourceRow = DataController.InvalidRow;
			return res;
		}
		protected override int? RaiseCustomSort(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder sortOrder) {
			SortArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
			Grid.RaiseCustomColumnSort(SortArgs);
			return SortArgs.GetSortResult();
		}
		protected override int? RaiseCustomGroup(int listSourceRow1, int listSourceRow2, object value1, object value2, ColumnSortOrder columnSortOrder) {
			SortArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, ColumnSortOrder.None);
			Grid.RaiseCustomColumnGroup(SortArgs);
			return SortArgs.GetSortResult();
		}
		#region IValueProvider Members
		int listSourceRow = DataController.InvalidRow;
		object IValueProvider.GetValue(string fieldName) {
			return Data.Grid.DataProxy.GetListSourceRowValue(listSourceRow, fieldName);
		}
		#endregion
	}
	public class GridEventsHelper {
		public GridEventsHelper() {
			PendingEvents = new Dictionary<object, bool>(2);
		}
		Dictionary<object, bool> PendingEvents { get; set; }
		public void SetPending(object evt) {
			PendingEvents[evt] = true;
		}
		public bool CheckClear(object evt) {
			if(!PendingEvents.ContainsKey(evt)) return false;
			PendingEvents.Remove(evt);
			return true;
		}
	}
	public class GridCommandButtonHelper {
		public GridCommandButtonHelper(ASPxGridBase grid) {
			Grid = grid;
			CommandButtonClientIDList = new HashSet<string>();
		}
		int CommandButtonsCount { get; set; }
		int ClientCommandButtonsCount { get; set; }
		public HashSet<string> CommandButtonClientIDList { get; private set; }
		protected ASPxGridBase Grid { get; private set; }
		protected bool UseEndlessPaging { get { return Grid.RenderHelper.UseEndlessPaging; } }
		public int GetNewIndex() {
			return CommandButtonsCount++;
		}
		public void ResetIndices() {
			CommandButtonsCount = 0;
			CommandButtonClientIDList.Clear();
		}
		public void Invalidate() {
			if(Grid.ChildControlsCreated)
				CommandButtonsCount = ClientCommandButtonsCount;
			CommandButtonClientIDList.Clear();
		}
		public void Save(TypedBinaryWriter writer) {
			int count = UseEndlessPaging ? CommandButtonsCount : 0;
			writer.WriteObject(count);
		}
		public void Load(TypedBinaryReader reader) {
			ClientCommandButtonsCount = reader.ReadObject<int>();
		}
	}
}
