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
using DevExpress.Office;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Spreadsheet.Functions;
namespace DevExpress.Spreadsheet {
	public interface Table {
		string Name { get; set; }
		Range Range { get; set; }
		Range TotalRowRange { get; }
		Range DataRange { get; } 
		Range HeaderRowRange { get; } 
		TableColumnCollection Columns { get; }
		TableStyle Style { get; set; }
		TableAutoFilter AutoFilter { get; }
		bool ShowHeaders { get; set; }
		bool ShowTableStyleColumnStripes { get; set; }
		bool ShowTableStyleFirstColumn { get; set; }
		bool ShowTableStyleLastColumn { get; set; }
		bool ShowTableStyleRowStripes { get; set; }
		bool ShowTotals { get; set; }
		string Comment { get; set; } 
		void Delete();
		void ConvertToRange();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region usings
	using DevExpress.Utils;
	using System.Collections.Generic;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Internal;
	using DevExpress.XtraSpreadsheet.Localization;
	using ModelDefinedName = DevExpress.XtraSpreadsheet.Model.DefinedName;
	using ModelDefinedNameCollection = DevExpress.XtraSpreadsheet.Model.DefinedNameCollection;
	using ModelDefinedNameDase = DevExpress.XtraSpreadsheet.Model.DefinedNameBase;
	using ModelStyleSheet = DevExpress.XtraSpreadsheet.Model.StyleSheet;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using DevExpress.Spreadsheet;
	using DevExpress.Office.Utils;
	#endregion
	#region NativeTable
	partial class NativeTable : Table {
		readonly Model.Table modelTable;
		NativeWorksheet worksheet;
		bool isValid;
		NativeTableColumnCollection listColumns;
		NativeTableAutoFilter autoFilter;
		public NativeTable(NativeWorksheet worksheet, Model.Table modelTable) {
			Guard.ArgumentNotNull(modelTable, "modelTable");
			this.worksheet = worksheet;
			this.modelTable = modelTable;
			this.isValid = true;
			CreateApiObjects();
			SubscribeInternalAPIEvents();
		}
		protected internal bool IsValid { get { return isValid; } }
		protected internal Model.Table ModelTable { get { return modelTable; } }
		#region CreateApiObjects
		protected internal virtual void CreateApiObjects() {
			listColumns = new NativeTableColumnCollection(worksheet, this);
			listColumns.Initialize();
		}
		#endregion
		#region SubscribeInternalAPIEvents
		protected internal virtual void SubscribeInternalAPIEvents() {
			ModelTable.Worksheet.Workbook.InternalAPI.TableColumnAdd += OnTableColumnAdd;
			ModelTable.Worksheet.Workbook.InternalAPI.TableColumnRemoveAt += OnTableColumnRemoveAt;
		}
		#endregion
		#region UnsubscribeInternalAPIEvents
		protected internal virtual void UnsubscribeInternalAPIEvents() {
			ModelTable.Worksheet.Workbook.InternalAPI.TableColumnAdd -= OnTableColumnAdd;
			ModelTable.Worksheet.Workbook.InternalAPI.TableColumnRemoveAt -= OnTableColumnRemoveAt;
		}
		#endregion
		public void Invalidate() {
			UnsubscribeInternalAPIEvents();
			this.isValid = false;
			if (autoFilter != null) {
				autoFilter.IsValid = false;
				autoFilter = null;
			}
		}
		#region ListObject Members
		public bool ShowTotals {
			get { return modelTable.HasTotalsRow; }
			set {
				if (ShowTotals != value)
					modelTable.ChangeTotals(value, ApiErrorHandler.Instance);
			}
		}
		public bool ShowHeaders {
			get { return modelTable.HasHeadersRow; }
			set {
				if (value != ShowHeaders)
					modelTable.ChangeHeaders(value, value, value, ApiErrorHandler.Instance);
			}
		}
		public TableColumnCollection Columns { get { return listColumns; } }
		public TableAutoFilter AutoFilter {
			get {
				if (autoFilter == null)
					autoFilter = new NativeTableAutoFilter(modelTable.AutoFilter, worksheet);
				return autoFilter;
			} 
		}
		public string Name { get { return modelTable.Name; } set { modelTable.Name = value; } }
		public Range Range {
			get { return CreateRange(modelTable.Range); }
			set { modelTable.Range = worksheet.GetModelSingleRange(value); }
		}
		public TableStyle Style {
			get { return new NativeTableStyle(worksheet.NativeWorkbook, ModelTable.Style); }
			set {
				if (!value.IsTableStyle)
					SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorInvalidTableStyleType);
				Model.TableStyle modelStyle = ((NativeTableStyle)value).ModelTableStyle;
				ModelTable.Style = modelStyle;
			}
		}
		public void Delete() {
			if (isValid) {
				Model.TableRemoveApiCommand command = new Model.TableRemoveApiCommand(ApiErrorHandler.Instance, modelTable);
				command.Execute();
			}
		}
		public void ConvertToRange() {
			if (isValid) {
				int index = worksheet.ModelWorksheet.Tables.IndexOf(modelTable);
				if (index < 0)
					return;
				Model.TableConvertToRangeCommand command = new Model.TableConvertToRangeCommand(modelTable, index, ApiErrorHandler.Instance);
				command.Execute();
			}
		}
		public Range TotalRowRange { get { return CreateRange(ModelTable.TryGetTotalsRowRange()); } }
		public Range DataRange { get { return CreateRange(ModelTable.GetDataRange()); } }
		public Range HeaderRowRange { get { return CreateRange(ModelTable.TryGetHeadersRowRange()); } }
		public bool ShowTableStyleColumnStripes {
			get { return ModelTable.ShowColumnStripes; }
			set { ModelTable.ShowColumnStripes = value; }
		}
		public bool ShowTableStyleFirstColumn {
			get { return ModelTable.ShowFirstColumn; }
			set { ModelTable.ShowFirstColumn = value; }
		}
		public bool ShowTableStyleLastColumn {
			get { return ModelTable.ShowLastColumn; }
			set { ModelTable.ShowLastColumn = value; }
		}
		public bool ShowTableStyleRowStripes {
			get { return ModelTable.ShowRowStripes; }
			set { ModelTable.ShowRowStripes = value; }
		}
		public string Comment {
			get {
				string result = ModelTable.Comment;
				return String.IsNullOrEmpty(result) ? String.Empty : result;
			}
			set { ModelTable.Comment = value; }
		}
		#endregion
		protected internal Range CreateRange(Model.CellRange range) {
			if (range == null)
				return null;
			return worksheet.CreateRange(range);
		}
		protected internal void RemoveColumn(NativeTableColumn column) {
			listColumns.Remove(column);
		}
		#region OnTableColumnAdd
		void OnTableColumnAdd(object sender, Model.TableColumnAddEventArgs e) {
			if (!Object.ReferenceEquals(e.TableColumn.Table, modelTable))
				return;
			((NativeTableColumnCollection)Columns).OnAdd(e.TableColumn, e.Index);
		}
		#endregion
		#region OnTableColumnRemoveAt
		void OnTableColumnRemoveAt(object sender, Model.TableColumnRemoveAtEventArgs e) {
			if (!Object.ReferenceEquals(e.Table, modelTable))
				return;
			((NativeTableColumnCollection)Columns).OnRemoveAt(e.Index);
		}
		#endregion
	}
	#endregion
}
