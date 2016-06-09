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
namespace DevExpress.Spreadsheet {
	public interface TableColumn {
		Range DataRange { get; }
		Range Range { get; }
		Range Total { get; }
		int Index { get; }
		string Name { get; set; }
		TotalRowFunction TotalRowFunction { get; set; }
		string TotalRowLabel { get; set; }
		string TotalRowFormula { get; set; }
		string TotalRowArrayFormula { get; set; }
		string Formula { get; set; }
		string ArrayFormula { get; set; }
		void Delete();
	}
	public enum TotalRowFunction {
		None = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.None,
		Sum = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.Sum,
		Min = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.Min,
		Max = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.Max,
		Average = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.Average,
		Count = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.Count,
		CountNums = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.CountNums,
		StdDev = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.StdDev,
		Var = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.Var,
		Custom = DevExpress.XtraSpreadsheet.Model.TotalsRowFunctionType.Custom,
	}
	public interface TableColumnCollection : ISimpleCollection<TableColumn> {
		TableColumn Add();
		TableColumn Add(int position);
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
	using System.Collections;
	#endregion
	#region NativeTableColumn
	partial class NativeTableColumn : TableColumn {
		Model.TableColumn modelTableColumn;
		NativeTable nativeTable;
		bool isValid;
		public NativeTableColumn(NativeTable nativeTable, Model.TableColumn modelTableColumn) {
			this.nativeTable = nativeTable;
			this.modelTableColumn = modelTableColumn;
			this.isValid = true;
		}
		#region TableColumn Members
		public Range DataRange { get { return nativeTable.CreateRange(modelTableColumn.GetColumnRangeWithoutHeadersAndTotalsRows()); } }
		public Range Range { get { return nativeTable.CreateRange(modelTableColumn.GetColumnRange()); } }
		public Range Total { get { return nativeTable.CreateRange(modelTableColumn.GetTotalRange()); } }
		public int Index { get { return modelTableColumn.FindColumnIndex(); } }
		protected internal bool IsValid { get { return isValid; } }
		protected internal void Invalidate() {
			isValid = false;
		}
		public string Name { get { return modelTableColumn.Name; } set { modelTableColumn.Name = value; } }
		public TotalRowFunction TotalRowFunction {
			get { return (TotalRowFunction)modelTableColumn.TotalsRowFunction; }
			set { modelTableColumn.TotalsRowFunction = (Model.TotalsRowFunctionType)value; }
		}
		public string TotalRowLabel {
			get { return modelTableColumn.TotalsRowLabel; }
			set { modelTableColumn.TotalsRowLabel = value; }
		}
		public string TotalRowFormula {
			get {
				string formula = modelTableColumn.TotalsRowFormulaText;
				if (formula == null)
					return string.Empty;
				if (!String.IsNullOrEmpty(formula))
					return "=" + formula;
				return string.Empty;
			}
			set {
				modelTableColumn.SetTotalsRowFormula(value, false);
			}
		}
		public string TotalRowArrayFormula {
			get {
				string formula = modelTableColumn.TotalsRowFormulaIsArray ? modelTableColumn.TotalsRowFormulaText : string.Empty;
				if (!String.IsNullOrEmpty(formula))
					return "=" + formula;
				return string.Empty;
			}
			set {
				modelTableColumn.SetTotalsRowFormula(value, true);
			}
		}
		public string Formula {
			get {
				string formula = modelTableColumn.ColumnFormulaText;
				if (!String.IsNullOrEmpty(formula))
					return "=" + formula;
				return string.Empty;
			}
			set {
				modelTableColumn.SetColumnFormula(value, false);
			}
		}
		public string ArrayFormula {
			get {
				string formula = modelTableColumn.ColumnFormulaIsArray ? modelTableColumn.ColumnFormulaText : string.Empty;
				if (!String.IsNullOrEmpty(formula))
					return "=" + formula;
				return string.Empty;
			}
			set {
				modelTableColumn.SetColumnFormula(value, true);
			}
		}
		public void Delete() {
			nativeTable.RemoveColumn(this);
		}
		#endregion
	}
	#endregion
	#region NativeTableColumnCollection
	partial class NativeTableColumnCollection : NativeCollectionBase<TableColumn, NativeTableColumn, Model.TableColumn>, TableColumnCollection {
		readonly NativeTable nativeTable;
		public NativeTableColumnCollection(NativeWorksheet nativeWorksheet, NativeTable nativeTable)
			: base(nativeWorksheet) {
			Guard.ArgumentNotNull(nativeTable, "nativeTable");
			this.nativeTable = nativeTable;
		}
		#region Properties
		public override int ModelCollectionCount { get { return ModelTable.Columns.Count; } }
		Model.Table ModelTable { get { return nativeTable.ModelTable; } }
		#endregion
		public override IEnumerable<Model.TableColumn> GetModelItemEnumerable() {
			return ModelTable.Columns;
		}
		protected override void InvalidateItem(NativeTableColumn item) {
			item.Invalidate();
		}
		protected override NativeTableColumn CreateNativeObject(Model.TableColumn modelObject) {
			return new NativeTableColumn(nativeTable, modelObject);
		}
		protected override void RemoveModelObjectAt(int index) {
			ModelTable.RemoveColumn(index, ApiErrorHandler.Instance);
		}
		protected override void ClearModelObjects() {
			ModelTable.Columns.Clear();
		}
		protected internal void OnAdd(Model.TableColumn modelObject, int index) {
			InnerList.Insert(index, CreateNativeObject(modelObject));
		}
		#region ListColumnCollection Members
		public TableColumn Add() {
			return AddCore(Count);
		}
		public TableColumn Add(int position) {
			return AddCore(position);
		}
		TableColumn AddCore(int position) {
			ModelTable.InsertColumn(position, ApiErrorHandler.Instance);
			return InnerList[position];
		}
		#endregion
	}
	#endregion
}
