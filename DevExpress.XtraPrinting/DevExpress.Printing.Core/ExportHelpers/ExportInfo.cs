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
using System.Linq;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using DevExpress.XtraPrinting;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Printing.ExportHelpers {
	public struct Group {
		public int Start { get; set; }
		public int End { get; set; }
	};
	public struct XlGroup {
		public IXlGroup Group { get; set; }
		public int GroupId { get; set; }
		public int StartGroup { get; set; }
	};
	public enum EventType {
		Header,
		Footer
	}
	public class ExporterInfo<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		int gridRowsCount;
		XlsExportOptionsBase optionsBase;
		XlsExportOptions optionsXls;
		XlsxExportOptionsEx optionsXlsx;
		internal Lazy<Stack<XlGroup>> groupsStack;
		internal Lazy<List<Group>> groupsList;
		GridViewExcelExporter<TCol, TRow> helper;
		public ExporterInfo(GridViewExcelExporter<TCol,TRow> helper) {
			this.helper = helper;
			gridRowsCount = CalcAllRows();
			optionsBase = helper.Options as XlsExportOptionsBase;
			optionsXls = helper.Options as XlsExportOptions;
			optionsXlsx = helper.Options as XlsxExportOptionsEx;
			this.groupsStack = new Lazy<Stack<XlGroup>>(CreateStackInstance());
			this.groupsList = new Lazy<List<Group>>(CreateGroupsListInstance());
		}
		static Func<List<Group>> CreateGroupsListInstance(){
			return () => new List<Group>();
		}
		static Func<Stack<XlGroup>> CreateStackInstance(){
			return ()=>new Stack<XlGroup>();
		}
		public Stack<XlGroup> GroupsStack{
			get { return groupsStack.Value; }
		}
		public List<Group> GroupsList{
			get { return groupsList.Value; }
		}
		internal int ExportRowIndex { get; set; }
		internal int DataAreaBottomRowIndex { get; set;}
		public GridViewExcelExporter<TCol, TRow> Helper { get { return helper; } }
		public IGridView<TCol, TRow> View { get { return helper.View; } }
		public IXlSheet Sheet { get { return helper.Sheet; } }
		public IDataAwareExportOptions Options { get { return helper.Options; } }
		public IXlExport Exporter { get { return helper.Exporter; } }
		public DefaultBoolean SuppressMaxRowsWarning{
			get{
				if(optionsXls != null) {
					return optionsXls.Suppress65536RowsWarning ? DefaultBoolean.True : DefaultBoolean.False;
				}
				if(optionsXlsx != null) {
					return optionsXlsx.SuppressMaxRowsWarning ? DefaultBoolean.True : DefaultBoolean.False;
				}
				return DefaultBoolean.Default;
			}
		}
		public DefaultBoolean SuppressMaxColumnsWarning{
			get {
				if(optionsXls != null) {
					return optionsXls.Suppress256ColumnsWarning ? DefaultBoolean.True : DefaultBoolean.False;
				}
				if(optionsXlsx != null) {
					return optionsXlsx.SuppressMaxColumnsWarning ? DefaultBoolean.True : DefaultBoolean.False;
				}
				return DefaultBoolean.Default;
			}
		}
		public bool RawDataMode{
			get { return optionsBase != null && optionsBase.RawDataMode; }
		}
		public bool ShowGridLines{
			get { return optionsBase != null && optionsBase.ShowGridLines; }
		}
		[DefaultValue(false)]
		public bool ColumnGrouping { get; set; }
		public bool OptionsAllowAddAutoFilter{
			get{
				return Options.ShowColumnHeaders == DefaultBoolean.True && Options.AllowSortingAndFiltering == DefaultBoolean.True;
			}
		}
		public bool OptionsAllowSetFixedHeader{
			get { return (ExportColumnHeaders || View.FixedRowsCount > 1) && Options.AllowFixedColumnHeaderPanel == DefaultBoolean.True; }
		}
		public bool ExportColumnHeaders{
			get{
				return Options.ShowColumnHeaders == DefaultBoolean.True;
			}
		}
		public bool СomplyWithFormatLimits(){
			if(SuppressMaxRowsWarning != DefaultBoolean.True) return true;
			return ExportRowIndex < Exporter.DocumentOptions.MaxRowCount;
		}
		public bool ComplyWithFormatLimits(int index) {
			if (SuppressMaxColumnsWarning != DefaultBoolean.True) return true;
			return index < Exporter.DocumentOptions.MaxColumnCount;
		}
		int CalcAllRows(){
			int rowsCount = (int)helper.View.RowCount;
			if(Options.AllowLookupValues == DefaultBoolean.True)
				rowsCount += LookUpValuesExporter<TCol,TRow>.AuxiliarySheetRowsCount;
			return rowsCount;
		}
		ExportColumnsCollection<TCol> gridColumns;
		public ExportColumnsCollection<TCol> ColumnsInfoColl {
			get {
				if(gridColumns == null)
					gridColumns = Create();
				return gridColumns;
			}
		}
		ExportColumnsCollection<TCol> Create() {
			ExportColumnsCollection<TCol> columns = new ExportColumnsCollection<TCol>();
			Helper.ForAllColumns(helper.View, gridColumn => columns.Add(gridColumn));
			return columns;
		}
		IEnumerable<TCol> _gridColumns;
		public IEnumerable<TCol> GridColumns {
			get {
				if(_gridColumns == null) _gridColumns = CreateGridColumnsCollection();
				return _gridColumns;
			}
		}
		IEnumerable<TCol> CreateGridColumnsCollection() {
			List<TCol> columns = new List<TCol>();
			Helper.ForAllColumns(helper.View, gridColumn => columns.Add(gridColumn));
			return columns;
		}
		public void ReportProgress(int percentage) {
			helper.Options.ReportProgress(new ProgressChangedEventArgs(percentage, null));
		}
		public void ReportProgress(int exportRowIndex, ref int percentage) {
			if(exportRowIndex >= (gridRowsCount / 100) * percentage) {
				ReportProgress(percentage);
				percentage++;
			}
		}
		public void RaiseCustomizeSheetSettingsEvent(DataAwareExportContext<TCol, TRow> context) {
			var ea = new CustomizeSheetEventArgs { ExportContext = context };
			Options.RaiseCustomizeSheetSettingsEvent(ea);
		}
		public void RaiseAfterAddRowEvent(IRowBase row, DataAwareExportContext<TCol, TRow> context) {
			var ea = new AfterAddRowEventArgs {
				DataSourceOwner = helper.View,
				ExportContext = context
			};
			AssignRowSettings(ea, row);
			Options.RaiseAfterAddRowEvent(ea);
		}
		public void OnContextCustomizationEvent(EventType type, DataAwareExportContext<TCol, TRow> context) {
			var ea = new ContextEventArgs { ExportContext = context };
			switch(type) {
				case EventType.Header: Options.RaiseCustomizeSheetHeaderEvent(ea);
					break;
				case EventType.Footer: Options.RaiseCustomizeSheetFooterEvent(ea);
					break;
			}
		}
		void AssignRowSettings(IDataAwareEventArgsBase e, IRowBase row) {
			e.DocumentRow = ExportRowIndex;
			if(row != null) {
				e.RowHandle = row.LogicalPosition;
				e.DataSourceRowIndex = row.DataSourceRowIndex;
			} else e.DataSourceRowIndex = -1;
		}
		public object RaiseCustomizeCellEvent(SheetAreaType areaType, IXlCell cell, object cellValue, TCol gridColumn, IRowBase gridRow, ref string hyperlink, ref bool isHandled) {
			var cl = new CellObject { Value = cellValue, Hyperlink = hyperlink };
			var ea = CreateEventArgs(areaType, cl, gridColumn, gridRow, null);
			AssignRowSettings(ea, gridRow);
			Options.RaiseCustomizeCellEvent(ea);
			if(ea.Handled && ea.AreaType == areaType) {
				hyperlink = ea.Hyperlink;
				cellValue = ea.Value;
				cell.Formatting = ea.Formatting.ConvertWith(Options);
				if(cell.Formatting != null && cell.Formatting.NumberFormat == null && string.IsNullOrWhiteSpace(cell.Formatting.NetFormatString)) FormatExport.PrimaryFormatColumn(gridColumn, cell.Formatting);
				isHandled = true;
			}
			return cellValue;
		}
		public CustomizeCellEventArgsExtended CreateEventArgs(SheetAreaType aType, CellObject cell, TCol col, IRowBase row, ExportSummaryItem item) {
			var format = new XlFormattingObject();
			format.CopyFrom(col.Appearance, FormatType.Custom);
			CustomizeCellEventArgsExtended e = new CustomizeCellEventArgsExtended();
			e.AreaType = aType;
			e.Column = col;
			e.Row = row;
			e.ColumnFieldName = col.FieldName;
			e.DataSourceOwner = helper.View;
			e.Formatting = format;
			e.Hyperlink = cell.Hyperlink;
			e.SummaryItem = item;
			e.Value = cell.Value;
			AssignRowSettings(e, row);
			return e;
		}
	}
	class ClipboardExportInfo<TCol, TRow> :ExporterInfo<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		public ClipboardExportInfo(GridViewExcelExporter<TCol, TRow> helper) :
			base(helper) {
		}
	}
	public class ExportColumnsCollection<TCol> :IEnumerable<TCol>
		where TCol : class, IColumn {
		Dictionary<int, TCol> _indexTable = new Dictionary<int, TCol>();
		Dictionary<string, TCol> _columnTable = new Dictionary<string, TCol>();
		Dictionary<string, List<object>> _itemsTable = new Dictionary<string, List<object>>();
		public TCol this[int index] {
			get {
				TCol col;
				if(index < 0) return null;
				if(_indexTable.TryGetValue(index, out col)) return col;
				return null;
			}
		}
		public TCol this[string fieldName] {
			get {
				TCol col;
				if(string.IsNullOrEmpty(fieldName)) return null;
				if(_columnTable.TryGetValue(fieldName, out col)) return col;
				return null;
			}
		}
		public List<object> ColumnDvItemsByFieldName(string fieldName) {
			List<object> items;
			if(_itemsTable.TryGetValue(fieldName, out items)) return items;
			return null;
		}
		public void Add(TCol column) {
			if(!_indexTable.ContainsKey(column.LogicalPosition))
				_indexTable.Add(column.LogicalPosition, column);
			if(!_columnTable.ContainsKey(column.FieldName))
				_columnTable.Add(column.FieldName, column);
			if(!_itemsTable.ContainsKey(column.FieldName)) {
				var gridColumnItems = column.DataValidationItems;
				_itemsTable.Add(column.FieldName, gridColumnItems != null ? gridColumnItems.ToList() : null);
			}
		}
		public IEnumerator<TCol> GetEnumerator() {
			for(int i = 0; i < _indexTable.Count; i++) {
				yield return this[i];
			}
		}
		public int Count {
			get { return _indexTable.Count; }
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
