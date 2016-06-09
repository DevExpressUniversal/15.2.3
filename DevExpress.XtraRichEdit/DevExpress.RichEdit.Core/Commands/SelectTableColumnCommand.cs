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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	#region SelectTableColumnsCommand
	public class SelectTableColumnsCommand : RichEditSelectionCommand {
		#region Fields
		TableRowCollection rows;
		int startColumnIndex;
		int endColumnIndex;
		bool canCalculateExecutionParameters;
		#endregion
		public SelectTableColumnsCommand(IRichEditControl control)
			: base(control) {
			this.canCalculateExecutionParameters = true;
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.SelectTableColumns; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableColumns; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SelectTableColumnsDescription; } }
		public override string ImageName { get { return "SelectTableColumn"; } }
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public TableRowCollection Rows { get { return rows; } set { rows = value; } }
		public int StartColumnIndex { get { return startColumnIndex; } set { startColumnIndex = value; } }
		public int EndColumnIndex { get { return endColumnIndex; } set { endColumnIndex = value; } }
		public bool CanCalculateExecutionParameters { get { return canCalculateExecutionParameters; } set { canCalculateExecutionParameters = value; } }
		#endregion
		protected internal override bool PerformChangeSelection() {
			if (CanCalculateExecutionParameters)
				CalculateExecutionParameters();
			return base.PerformChangeSelection();
		}
		protected internal virtual void CalculateExecutionParameters() {
			if (!DocumentModel.Selection.IsWholeSelectionInOneTable())
				return;
			SelectedCellsCollection cells = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			Rows = cells.NormalizedFirst.Table.Rows;
			StartColumnIndex = cells.NormalizedFirst.NormalizedStartCell.GetStartColumnIndexConsiderRowGrid();
			EndColumnIndex = cells.NormalizedFirst.NormalizedEndCell.GetEndColumnIndexConsiderRowGrid();
			for (int i = cells.GetTopRowIndex() + 1; i <= cells.GetBottomRowIndex(); i++) {
				SelectedCellsIntervalInRow interval = cells[i];
				StartColumnIndex = Algorithms.Min(StartColumnIndex, interval.NormalizedStartCell.GetStartColumnIndexConsiderRowGrid());
				EndColumnIndex = Algorithms.Max(EndColumnIndex, interval.NormalizedEndCell.GetEndColumnIndexConsiderRowGrid());
			}
		}
		protected internal override void ChangeSelection(Selection selection) {
			selection.ClearSelectionInTable();
			TableCell startCell = GetStartCell();
			TableCell endCell = GetCellByColumnIndex(Rows.First, EndColumnIndex);
			selection.ManualySetTableSelectionStructureAndChangeSelection(startCell, endCell, true);
			ValidateSelection(selection, true);
		}
		protected internal virtual TableCell GetStartCell() {
			int count = Rows.Count;
			for (int i = 0; i < count; i++) {
				TableCell cell = TableCellVerticalBorderCalculator.GetCellByColumnIndex(Rows[i], StartColumnIndex);
				if (cell != null)
					return cell;
			}
			return null;
		}
		protected internal virtual TableCell GetCellByColumnIndex(TableRow row, int columnIndex) {
			TableCell cell = TableCellVerticalBorderCalculator.GetCellByColumnIndex(row, columnIndex);
			if (cell != null)
				return cell.Table.GetFirstCellInVerticalMergingGroup(cell);
			TableCellCollection cells = row.Cells;
			Debug.Assert(cells.Count > 0);
			if (columnIndex < cells.First.GetStartColumnIndexConsiderRowGrid())
				return cells.First;
			else
				return GetCellByColumnIndex(row.Previous, columnIndex);
		}
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return pos.LogPosition;
		}
		protected internal override void EnsureCaretVisibleVertically() {
		}
	}
	#endregion
}
