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
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region InsertHyperlinkCommand
	public class InsertHyperlinkCommand : ErrorHandledWorksheetCommand, IHyperlinkViewInfo {
		#region Fields
		CellRange range;
		string targetUri;
		bool isExternal;
		string displayText;
		string tooltipText = String.Empty;
		ParsedExpression expression;
		bool isDrawingObject;
		bool setDisplayTextToTopLeftCell;
		bool setHyperlinkStyleToRange;
		#endregion
		public InsertHyperlinkCommand(IErrorHandler errorHandler, Worksheet worksheet, CellRange range, string targetUri, bool isExternal, string displayText, bool isDrawingObject)
			: base(worksheet, errorHandler) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			Guard.ArgumentNotNull(targetUri, "targetUri");
			this.range = range;
			this.targetUri = targetUri;
			this.isExternal = isExternal;
			this.displayText = displayText;
			this.expression = null;
			this.isDrawingObject = isDrawingObject;
			this.setDisplayTextToTopLeftCell = true;
			this.setHyperlinkStyleToRange = true;
		}
		#region Properties
		public bool SetDisplayTextToTopLeftCell { get { return setDisplayTextToTopLeftCell; } set { setDisplayTextToTopLeftCell = value; } }
		public bool SetHyperlinkStyleToRange { get { return setHyperlinkStyleToRange; } set { setHyperlinkStyleToRange = value; } }
		#region IHyperlinkViewInfo
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public string TooltipText { get { return tooltipText; } set { tooltipText = value; } }
		public bool IsExternal { get { return isExternal; } set { isExternal = value; } }
		public CellRange Range { get { return range; } }
		public DocumentModel Workbook { get { return DocumentModel; } }
		public bool IsDrawingObject { get { return isDrawingObject; } }
		public string TargetUri { get { return targetUri; } }
		public ParsedExpression Expression {
			get {
				if (expression == null)
					expression = HyperlinkExpressionParser.Parse(Workbook.DataContext, targetUri, IsExternal);
				return expression;
			}
		}
		#endregion
		#endregion
		protected internal override bool Validate() {
			if (Worksheet.PivotTables.ContainsItemsInRange(range, true)) {
				if (!HandleError(new ModelErrorInfo(ModelErrorType.PivotTableCanNotBeChanged)))
					return false;
			}
			return true;
		}
		protected internal override void ExecuteCore() {
			ModelHyperlink hyperlink = Worksheet.CreateHyperlinkCoreFromImport(range, targetUri, isExternal);
			hyperlink.DisplayText = displayText;
			Worksheet.Hyperlinks.Add(hyperlink);
			if (SetHyperlinkStyleToRange)
				SetHyperlinkStyleToHyperlinkRange(hyperlink);
			if (SetDisplayTextToTopLeftCell)
				SetTextToTopLeftCell(hyperlink);
			hyperlink.TooltipText = TooltipText;
			Result = hyperlink;
		}
		void SetTextToTopLeftCell(ModelHyperlink hyperlink) {
			SetTextToHyperlinkTopLeftCellCommand command = new SetTextToHyperlinkTopLeftCellCommand(Worksheet, hyperlink.Range, hyperlink.DisplayText, hyperlink.TargetUri);
			command.Execute();
		}
		public void SetHyperlinkStyleToHyperlinkRange(ModelHyperlink existingHyperlink) {
			DocumentModel.BeginUpdate();
			try {
				CellStyleBase hyperlinkStyle = DocumentModel.StyleSheet.CellStyles.GetCellStyleByName("Hyperlink");
				if (hyperlinkStyle == null) {
					hyperlinkStyle = new BuiltInCellStyle(DocumentModel, (int)DevExpress.Spreadsheet.BuiltInStyleId.Hyperlink);
					DocumentModel.StyleSheet.CellStyles.Add(hyperlinkStyle);
				}
				else {
					if (hyperlinkStyle.IsHidden)
						hyperlinkStyle.SetHidden(false);
				}
				Worksheet sheet = (Worksheet)existingHyperlink.Range.Worksheet;
				CellRange range = existingHyperlink.Range;
				CellIntervalRange intervalRange = range as CellIntervalRange;
				if (intervalRange != null) {
					if (intervalRange.IsColumnInterval) {
						IList<Column> columns = sheet.Columns.GetColumnRangesEnsureExist(intervalRange.TopLeft.Column, intervalRange.BottomRight.Column);
						foreach (Column column in columns)
							column.Style = hyperlinkStyle;
					}
					else
						for (int i = intervalRange.TopLeft.Row; i <= intervalRange.BottomRight.Row; i++)
							sheet.Rows[i].Style = hyperlinkStyle;
				}
				else
					foreach (CellBase cellbase in range.GetAllCellsEnumerable()) {
						ICell cell = cellbase as ICell;
						if (cell != null)
							cell.Style = hyperlinkStyle;
					}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override void ApplyChanges() {
		}
		#region IHyperlinkViewInfo
		public void SetTargetUriWithoutHistory(string uri) {
			this.targetUri = uri;
			this.expression = null;
		}
		public CellRangeBase GetTargetRange() {
			return HyperlinkExpressionParser.GetTargetRange(this.Worksheet.DataContext, Expression, isExternal);
		}
		#endregion
	}
	#endregion
	#region DeleteWorksheetHyperlinkCommand
	public class DeleteWorksheetHyperlinkCommand : SpreadsheetModelCommand {
		int index;
		public DeleteWorksheetHyperlinkCommand(Worksheet worksheet, int index)
			: base(worksheet) {
			Guard.ArgumentNonNegative(index, "index");
			this.index = index;
		}
		public bool ClearFormats { get; set; }
		protected internal override void ExecuteCore() {
			ModelHyperlink hyperlink = Worksheet.Hyperlinks[index];
			Worksheet.Hyperlinks.RemoveAt(index);
			if (ClearFormats)
				SetNormalStyleToHyperlinkRange(hyperlink);
		}
		public void SetNormalStyleToHyperlinkRange(ModelHyperlink existingHyperlink) {
			DocumentModel.BeginUpdate();
			try {
				CellStyleBase normalStyle = DocumentModel.StyleSheet.CellStyles[0];
				CellRange range = existingHyperlink.Range;
				foreach (CellBase cellbase in range.GetAllCellsEnumerable()) {
					ICell cell = cellbase as ICell;
					if (cell != null)
						cell.Style = normalStyle;
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal override void ApplyChanges() {
		}
	}
	#endregion
	#region DeleteAllWorksheetHyperlinkCommand
	public class DeleteAllWorksheetHyperlinkCommand : SpreadsheetModelCommand {
		ModelHyperlinkCollection hyperlinks;
		public DeleteAllWorksheetHyperlinkCommand(Worksheet worksheet, ModelHyperlinkCollection comments)
			: base(worksheet) {
			Guard.ArgumentNotNull(comments, "comments");
			this.hyperlinks = comments;
		}
		protected internal override void ExecuteCore() {
			foreach (ModelHyperlink hyperlink in hyperlinks)
				Worksheet.ClearFormats(hyperlink.Range);
			Worksheet.Hyperlinks.Clear();
		}
		protected internal override void ApplyChanges() {
		}
	}
	#endregion
	#region SetTextToHyperlinkTopLeftCellCommand
	public class SetTextToHyperlinkTopLeftCellCommand : SpreadsheetModelCommand {
		readonly string displayText;
		readonly CellRange range;
		readonly string targetUri;
		public SetTextToHyperlinkTopLeftCellCommand(Worksheet worksheet, CellRange range, string displayText, string targetUri)
			: base(worksheet) {
			Guard.ArgumentNotNull(range, "range");
			Guard.ArgumentNotNull(targetUri, "targetUri");
			this.range = range;
			this.displayText = displayText;
			this.targetUri = targetUri;
		}
		public bool ShouldRaiseCellValueChanged { get; set; }
		protected internal override void ExecuteCore() {
			ICell firstCell = range.GetCellRelative(0, 0) as ICell;
			if (firstCell == null)
				return;
			if (firstCell.HasFormula) {
				FormulaBase oldCellFormula = firstCell.GetFormula();
				if (oldCellFormula is ArrayFormula || oldCellFormula is ArrayFormulaPart)
					return;
			}
			if (String.IsNullOrEmpty(displayText)) {
				CellContentSnapshot snapshot = new CellContentSnapshot(firstCell);
				firstCell.Value = targetUri;
				RaiseCellValueChanged(snapshot);
			}
			else if (IsFormula(displayText)) {
				CellContentSnapshot snapshot = new CellContentSnapshot(firstCell);
				Formula cellFormula = new Formula(firstCell, displayText);
				firstCell.SetFormula(cellFormula);
				RaiseCellValueChanged(snapshot);
			}
			else if (firstCell.Value.IsEmpty || ((displayText == targetUri || DisplayTextChanged(firstCell)))) {
				CellContentSnapshot snapshot = new CellContentSnapshot(firstCell);
				firstCell.Text = displayText;
				RaiseCellValueChanged(snapshot);
			}
		}
		void RaiseCellValueChanged(CellContentSnapshot snapshot) {
			if (ShouldRaiseCellValueChanged)
				DocumentModel.InternalAPI.RaiseCellValueChanged(snapshot);
		}
		bool DisplayTextChanged(ICell firstCell) {
			return firstCell.Text != displayText;
		}
		bool IsFormula(string value) {
			return (value.Length > 1) && (value[0] == '=');
		}
		protected internal override void ApplyChanges() {
		}
	}
	#endregion
}
