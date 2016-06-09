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
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	#region PivotReportLayout
	public enum PivotReportLayout {
		Compact,
		Outline,
		Tabular
	}
	#endregion
	#region PivotPageOrder
	public enum PivotPageOrder {
		OverThenDown,
		DownThenOver
	}
	#endregion
	#region PivotLayout
	public interface PivotLayout {
		bool ShowColumnGrandTotals { get; set; }
		bool ShowRowGrandTotals { get; set; }
		bool CompactNewFields { get; set; }
		bool OutlineNewFields { get; set; }
		bool DataOnRows { get; set; }
		bool MergeTitles { get; set; }
		int IndentInCompactForm { get; set; }
		PivotPageOrder PageOrder { get; set; }
		int PageWrap { get; set; }
		bool SubtotalIncludeHiddenItems { get; set; }
		void ShowAllSubtotals(bool topOfGroup);
		void HideAllSubtotals();
		void SetReportLayout(PivotReportLayout layout);
		void RepeatAllItemLabels(bool repeat);
		void InsertBlankRows();
		void RemoveBlankRows();
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Office.Utils;
	partial class NativePivotTable {
		public PivotLayout Layout { get { return this; } }
		#region ShowColumnGrandTotals
		bool PivotLayout.ShowColumnGrandTotals {
			get {
				CheckValid();
				return modelPivotTable.ColumnGrandTotals;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.ColumnGrandTotals = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region ShowRowGrandTotals
		bool PivotLayout.ShowRowGrandTotals {
			get {
				CheckValid();
				return modelPivotTable.RowGrandTotals;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.RowGrandTotals = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region CompactNewFields
		bool PivotLayout.CompactNewFields {
			get {
				CheckValid();
				return modelPivotTable.Compact;
			}
			set {
				CheckValid();
				modelPivotTable.Compact = value;
			}
		}
		#endregion
		#region OutlineNewFields
		bool PivotLayout.OutlineNewFields {
			get {
				CheckValid();
				return modelPivotTable.Outline;
			}
			set {
				CheckValid();
				modelPivotTable.Outline = value;
			}
		}
		#endregion
		#region DataOnRows
		bool PivotLayout.DataOnRows {
			get {
				CheckValid();
				return modelPivotTable.DataOnRows;
			}
			set {
				CheckValid();
				bool dataOnRows = modelPivotTable.DataOnRows;
				if (dataOnRows == value)
					return;
				if (((NativePivotFieldReferenceCollection)ColumnFields).HasValuesField ||
					((NativePivotFieldReferenceCollection)RowFields).HasValuesField)
					AddFieldToKeyFields(Model.PivotTable.ValuesFieldFakeIndex, dataOnRows ? PivotAxisType.Column : PivotAxisType.Row);
				else {
					modelPivotTable.BeginUpdate();
					try {
						modelPivotTable.DataOnRows = value;
						modelPivotTable.DataPosition = -1;
					}
					finally {
						modelPivotTable.EndUpdate();
					}
				}
			}
		}
		#endregion
		#region MergeTitles
		bool PivotLayout.MergeTitles {
			get {
				CheckValid();
				return modelPivotTable.MergeItem;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.MergeItem = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region IndentInCompactForm
		int PivotLayout.IndentInCompactForm {
			get {
				CheckValid();
				return modelPivotTable.Indent;
			}
			set {
				CheckValid();
				ApiValueChecker.CheckValue(value, 0, 127);
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.Indent = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region PageOrder
		PivotPageOrder PivotLayout.PageOrder {
			get {
				CheckValid();
				return modelPivotTable.PageOverThenDown ? PivotPageOrder.OverThenDown : PivotPageOrder.DownThenOver;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.PageOverThenDown = value == PivotPageOrder.OverThenDown;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region PageWrap
		int PivotLayout.PageWrap {
			get {
				CheckValid();
				return modelPivotTable.PageWrap;
			}
			set {
				CheckValid();
				ApiValueChecker.CheckValue(value, 0, 255);
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.PageWrap = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		#region SubtotalIncludeHiddenItems
		bool PivotLayout.SubtotalIncludeHiddenItems {
			get {
				CheckValid();
				return modelPivotTable.SubtotalHiddenItems;
			}
			set {
				CheckValid();
				modelPivotTable.BeginTransaction(ErrorHandler);
				try {
					modelPivotTable.SubtotalHiddenItems = value;
				}
				finally {
					modelPivotTable.EndTransaction();
				}
			}
		}
		#endregion
		void PivotLayout.ShowAllSubtotals(bool topOfGroup) {
			CheckValid();
			modelPivotTable.ShowAllSubtotals(topOfGroup, ErrorHandler);
		}
		void PivotLayout.HideAllSubtotals() {
			CheckValid();
			modelPivotTable.HideAllSubtotals(ErrorHandler);
		}
		void PivotLayout.SetReportLayout(PivotReportLayout layout) {
			CheckValid();
			if (layout == PivotReportLayout.Compact)
				modelPivotTable.SetCompactForm(ErrorHandler);
			else if (layout == PivotReportLayout.Outline)
				modelPivotTable.SetOutlineForm(ErrorHandler);
			else
				modelPivotTable.SetTabularForm(ErrorHandler);
		}
		void PivotLayout.RepeatAllItemLabels(bool repeat) {
			CheckValid();
			modelPivotTable.RepeatAllItemLabels(repeat, ErrorHandler);
		}
		void PivotLayout.InsertBlankRows() {
			CheckValid();
			modelPivotTable.InsertBlankRows(ErrorHandler);
		}
		void PivotLayout.RemoveBlankRows() {
			CheckValid();
			modelPivotTable.RemoveBlankRows(ErrorHandler);
		}
	}
}
