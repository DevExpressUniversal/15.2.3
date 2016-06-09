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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils.Commands;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Native.Templates;
namespace DevExpress.Snap.Core.Commands {
	#region ColumnIndexesInfo
	public struct ColumnIndexesInfo {
		public int StartColumnIndex { get; set; }
		public int EndColumnIndex { get; set; }
	}
	#endregion
	#region SnapDeleteTableCellsWithShiftToTheHorizontallyCommand
	public class SnapDeleteTableCellsWithShiftToTheHorizontallyCommand : DeleteTableCellsWithShiftToTheHorizontallyCommand {
		public SnapDeleteTableCellsWithShiftToTheHorizontallyCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void PerformModifyModel() {
			SelectedCellsCollection cellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			List<TableCell> modifiedCells = CalculateModifiedCells(cellsCollection);
			base.PerformModifyModel();
			TableCommandsHelper.InsertSeparators(ActivePieceTable, modifiedCells);
		}
		List<TableCell> CalculateModifiedCells(SelectedCellsCollection selectedCells) {
			List<TableCell> result = new List<TableCell>();
			SnapObjectModelController controller = new SnapObjectModelController((SnapPieceTable)ActivePieceTable);
			DocumentModel.BeginUpdate();
			try {
				for (int i = 0; i < selectedCells.RowsCount; i++) {
					SelectedCellsIntervalInRow interval = selectedCells[i];
					TableCellCollection cells = interval.Row.Cells;
					for (int cellIndex = interval.NormalizedStartCellIndex; cellIndex <= interval.NormalizedEndCellIndex; cellIndex++)
						if (controller.GetFirstCellRun(cells[cellIndex]) is SeparatorTextRun)
							result.Add(cells[interval.NormalizedEndCellIndex + 1]);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
			return result;
		}
	}
	#endregion
	#region SnapDeleteTableCellsDispatcherCommand
	public class SnapDeleteTableCellsDispatcherCommand : DeleteTableCellsDispatcherCommand {
		public SnapDeleteTableCellsDispatcherCommand(IRichEditControl control)
			: base(control) {
		}
		SnapSelectionInfo SelectionInfo { get { return ((SnapDocumentModel)DocumentModel).SelectionInfo; } }
		protected internal override void TableCellsOperationWithShiftToTheHorizontally() {
			SelectionInfo.PerformModifyModelBySelection(() => new SnapDeleteTableCellsWithShiftToTheHorizontallyCommand(Control).ExecuteCore());
		}
		protected internal override void TableCellsOperationWithRow() {
			SelectionInfo.PerformModifyModelBySelection(() => new SnapDeleteTableCellsWithShiftToTheHorizontallyCommand(Control).ExecuteCore());
		}
		protected internal override void TableCellsOperationWithColumn() {
			if (!HasGroups())
				SelectionInfo.PerformModifyModelBySelection(() => new SnapDeleteTableColumnsCommand(Control).ExecuteCore());
			else
				SelectionInfo.PerformModifyModelBySelection(() => {
					List<RowTemplateUpdaterBase> updaters = GetColumnTemplateUpdaters();
					new SnapDeleteTableCellsWithShiftToTheHorizontallyCommand(Control).ExecuteCore();
					for (int i = 0; i < updaters.Count; i++)
						updaters[i].UpdateTemplates(false);
				});
		}
		List<RowTemplateUpdaterBase> GetColumnTemplateUpdaters() {
			List<RowTemplateUpdaterBase> result = new List<RowTemplateUpdaterBase>();
			SelectedCellsCollection cellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			SelectedCellsIntervalInRow interval = cellsCollection.First;
			for (int i = interval.NormalizedEndCellIndex; i >= interval.NormalizedStartCellIndex; i--)
				result.Add(new DeleteColumnTemplateUpdater(interval.Row.Cells[i], Control));
			result.Add(new ResizeGroupHeaderFooterTemplateUpdater(interval, Control));
			return result;
		}
		bool HasGroups() {
			IFieldDataAccessService service = DocumentModel.GetService<IFieldDataAccessService>();
			if (service == null)
				return false;
			IFieldPathService fieldPathSrv = service.FieldPathService;
			SnapListFieldInfo fieldInfo = FieldsHelper.GetSelectedSNListField((SnapDocumentModel)DocumentModel);
			if (fieldInfo == null)
				return false;
			InstructionController controller = ((SnapPieceTable)ActivePieceTable).CreateFieldInstructionController(fieldInfo.Field);
			if (controller == null)
				return false;
			FieldPathDataMemberInfo dataMemberInfo = fieldPathSrv.FromString(controller.GetArgumentAsString(0)).DataMemberInfo;
			if (dataMemberInfo.IsEmpty)
				return false;
			return dataMemberInfo.LastItem.HasGroups;
		}
	}
	#endregion
	#region SnapShowDeleteTableCellsFormCommand
	public class SnapShowDeleteTableCellsFormCommand : ShowDeleteTableCellsFormCommand {
		public SnapShowDeleteTableCellsFormCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!state.Enabled)
				return;
			ApplySnapBookmarksIntegrityRestrictions(state);
		}
		void ApplySnapBookmarksIntegrityRestrictions(ICommandUIState state) {
			state.Enabled = TableCommandsHelper.CanDeleteTableCells(DocumentModel);
		}
	}
	#endregion
	public class SnapShowDeleteTableCellsFormMenuCommand : ShowDeleteTableCellsFormMenuCommand {
		public SnapShowDeleteTableCellsFormMenuCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!state.Enabled)
				return;
			state.Enabled = TableCommandsHelper.CanDeleteTableCells(DocumentModel);
			state.Visible = state.Enabled;
		}
	}
	#region SnapDeleteTableCommand
	public class SnapDeleteTableCommand : DeleteTableCommand {
		Field fieldToDelete;
		public SnapDeleteTableCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			if (fieldToDelete != null) {
				ISelectedTableStructureBase selectedCells = (ISelectedTableStructureBase)DocumentModel.Selection.SelectedCells;
				Table table = selectedCells.FirstSelectedCell.Table;
				this.DocumentModel.BeginUpdate();
				DocumentModelPosition startPosition = DocumentModelPosition.FromRunStart(table.PieceTable, fieldToDelete.Code.Start);
				DocumentModelPosition endPosition = DocumentModelPosition.FromRunEnd(table.PieceTable, fieldToDelete.Result.End);
				table.PieceTable.DeleteContent(startPosition.LogPosition, endPosition.LogPosition - startPosition.LogPosition, false, true, true, false);
				this.DocumentModel.EndUpdate();
			}
			else
				base.ExecuteCore();
		}
		public override void UpdateUIState(Utils.Commands.ICommandUIState state) {
			ISelectedTableStructureBase selectedCells = (ISelectedTableStructureBase)DocumentModel.Selection.SelectedCells;
			if (selectedCells == null || selectedCells.FirstSelectedCell == null || selectedCells.FirstSelectedCell.Table == null)
				return;
			Table table = selectedCells.FirstSelectedCell.Table;
			RunIndex firstRunIndex = table.PieceTable.Paragraphs[table.FirstRow.FirstCell.StartParagraphIndex].FirstRunIndex;
			RunIndex lastRunIndex = table.PieceTable.Paragraphs[table.LastRow.LastCell.StartParagraphIndex].LastRunIndex;
			fieldToDelete = FindFieldToDelete(table, firstRunIndex, lastRunIndex);
			if (fieldToDelete != null) {
				state.Enabled = true;
				return;
			}
			SnapBookmark firstBookmark = ((SnapPieceTable)table.PieceTable).FindBookmarkByPosition(DocumentModelPosition.FromRunStart(table.PieceTable, firstRunIndex).LogPosition);
			SnapBookmark lastBookmark = ((SnapPieceTable)table.PieceTable).FindBookmarkByPosition(DocumentModelPosition.FromRunStart(table.PieceTable, lastRunIndex).LogPosition);
			if (firstBookmark == lastBookmark) {
				state.Enabled = false;
				return;
			}
			state.Enabled = true;
		}
		Field FindFieldToDelete(Table table, RunIndex firstRunIndex, RunIndex lastRunIndex) {
			for (int i = 0; i < table.PieceTable.Fields.Count; i++) {
				if (firstRunIndex == table.PieceTable.Fields[i].Result.Start + 1 && table.PieceTable.Fields[i].Result.End == lastRunIndex + 1) {
					return table.PieceTable.Fields[i];
				}
			}
			return null;
		}
	}
	#endregion
}
