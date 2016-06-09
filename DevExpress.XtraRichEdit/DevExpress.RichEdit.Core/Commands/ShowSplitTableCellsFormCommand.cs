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
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.Office.History;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Tables.Native;
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowSplitTableCellsFormCommand
	public class ShowSplitTableCellsFormCommand : RichEditCommand {
		public ShowSplitTableCellsFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowSplitTableCellsFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowSplitTableCellsForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowSplitTableCellsFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SplitTableCells; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowSplitTableCellsFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SplitTableCellsDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowSplitTableCellsFormCommandImageName")]
#endif
		public override string ImageName { get { return "SplitTableCells"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowSplitTableCellsFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<SplitTableCellsParameters>();
		}
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<SplitTableCellsParameters> valueBasedState = state as IValueBasedCommandUIState<SplitTableCellsParameters>;
				valueBasedState.Value = GetSplitTableCellsParameters();
				ShowSplitTableCellsForm(valueBasedState.Value, ShowSplitTableCellsFormCallback, state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowSplitTableCellsForm(SplitTableCellsParameters parameters, ShowSplitTableCellsFormCallback callback, object callbackData) {
			Control.ShowSplitTableCellsForm(parameters, callback, callbackData);
		}
		protected internal virtual SplitTableCellsParameters GetSplitTableCellsParameters() {
			SelectedCellsCollection selectedCellsCollection = (SelectedCellsCollection)DocumentModel.Selection.SelectedCells;
			int selectedCellsInRowCount = selectedCellsCollection.NormalizedFirst.NormalizedLength;
			bool isSelectedCellsSquare = selectedCellsCollection.IsSquare();
			bool isMergeCellsBeforeSplit = isSelectedCellsSquare && (selectedCellsCollection.RowsCount > 1 || selectedCellsInRowCount > 0);
			int columnsCount = (selectedCellsInRowCount + 1) * 2;
			int rowCountAfterMerge = -1;
			if (isMergeCellsBeforeSplit)
				rowCountAfterMerge = CalculateRowCountAfterMerge(selectedCellsCollection);
			else
				rowCountAfterMerge = GetActualRowsCount(selectedCellsCollection, isSelectedCellsSquare);
			SplitTableCellsParameters result = new SplitTableCellsParameters(columnsCount, selectedCellsCollection.RowsCount, isMergeCellsBeforeSplit, rowCountAfterMerge);
			result.IsSelectedCellsSquare = isSelectedCellsSquare;
			return result;
		}
		private int CalculateRowCountAfterMerge(SelectedCellsCollection selectedCellsCollection) {
			int count = selectedCellsCollection.RowsCount;
			int result = 0;
			for (int i = 0; i < count; i++) {
				SelectedCellsIntervalInRow selectedCellsInRow = selectedCellsCollection[i];
				if (!ShouldIgnoreRow(selectedCellsInRow))
					result++;
			}
			return Math.Max(1, result);
		}
		protected virtual bool ShouldIgnoreRow(SelectedCellsIntervalInRow selectedCellsInRow) {
			int startCellIndex = selectedCellsInRow.NormalizedStartCellIndex;
			int endCellIndex = selectedCellsInRow.NormalizedEndCellIndex;
			TableCellCollection cells = selectedCellsInRow.Row.Cells;
			int cellCount = cells.Count;
			for (int i = 0; i < cellCount; i++) {
				if (i >= startCellIndex && i <= endCellIndex)
					continue;
				if (cells[i].VerticalMerging != MergingState.Continue)
					return false;
			}
			return true;
		}
		protected internal virtual int GetActualRowsCount(SelectedCellsCollection selectedCellsCollection, bool isSelectedCellsSquare) {
			if (isSelectedCellsSquare) {
				int columnIndex = TableCellVerticalBorderCalculator.GetStartColumnIndex(selectedCellsCollection.NormalizedFirst.NormalizedStartCell, false);
				return selectedCellsCollection.GetSelectedRowsCount(columnIndex);
			}
			return 0;
		}
		protected internal virtual void ShowSplitTableCellsFormCallback(SplitTableCellsParameters parameters, object callbackData) {
			Control.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					SplitTableCellsCommand command = DocumentModel.CommandsCreationStrategy.CreateSplitTableCellsCommand(Control);
					if (command.CanExecute()) {
						DefaultValueBasedCommandUIState<SplitTableCellsParameters> state = new DefaultValueBasedCommandUIState<SplitTableCellsParameters>();
						state.Value = parameters;
						command.ForceExecute(state);
					}
				}
			}
			finally {
				Control.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = DocumentModel.Selection.IsWholeSelectionInOneTable();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
	#region ShowSplitTableCellsFormMenuCommand
	public class ShowSplitTableCellsFormMenuCommand : ShowSplitTableCellsFormCommand {
		public ShowSplitTableCellsFormMenuCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowSplitTableCellsFormMenuCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowSplitTableCellsFormMenuItem; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowSplitTableCellsFormMenuCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SplitTableCellsMenuItem; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			bool isSelectionInTable = DocumentModel.Selection.IsWholeSelectionInOneTable();
			state.Enabled = isSelectionInTable && DocumentModel.Selection.SelectedCells.SelectedOnlyOneCell;
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Tables, state.Enabled);
			ApplyDocumentProtectionToSelectedParagraphs(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
	#region SplitTableCellsParameters
	public class SplitTableCellsParameters {
		#region Fields
		int columnsCount;
		int rowsCount;
		bool mergeCellsBeforeSplit;
		bool isSelectedCellsSquare;
		int rowCountAfterMerge;
		#endregion
		public SplitTableCellsParameters(int columnsCount, int rowsCount, bool mergeCellsBeforeSplit, int rowCountAfterMerge) {
			this.columnsCount = columnsCount;
			this.rowsCount = rowsCount;
			this.mergeCellsBeforeSplit = mergeCellsBeforeSplit;
			this.rowCountAfterMerge = rowCountAfterMerge;
		}
		#region Properties
		public int ColumnsCount { get { return columnsCount; } set { columnsCount = value; } }
		public int RowsCount { get { return rowsCount; } set { rowsCount = value; } }
		public bool MergeCellsBeforeSplit { get { return mergeCellsBeforeSplit; } set { mergeCellsBeforeSplit = value; } }
		public bool IsSelectedCellsSquare { get { return isSelectedCellsSquare; } set { isSelectedCellsSquare = value; } }
		public int RowCountAfterMerge { get { return rowCountAfterMerge; } set { rowCountAfterMerge = value; } }
		#endregion
	}
	#endregion
}
