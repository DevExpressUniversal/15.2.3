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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FormatCellsCommandBase (abstract class)
	public abstract class FormatCellsCommandBase : SpreadsheetCommand {
		protected FormatCellsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public abstract FormatCellsFormInitialTabPage InitialTabPage { get; }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<FormatCellsFormProperties> valueBasedState = state as IValueBasedCommandUIState<FormatCellsFormProperties>;
				if (valueBasedState == null)
					return;
				if (InnerControl.AllowShowingForms)
					Control.ShowFormatCellsForm(GetFormatCellsFormProperties(), InitialTabPage, ShowFormatCellsFormCallback);
				else
					ShowFormatCellsFormCallback(valueBasedState.Value);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		FormatCellsFormProperties GetFormatCellsFormProperties() {
			MergeCellFormatCommand command = new MergeCellFormatCommand(Control);
			command.Execute();
			DocumentModel documentModel = command.DocumentModel;
			MergedCellFormat mergedFormat = command.MergedCellFormat;
			mergedFormat.ActiveCellPosition = ActiveSheet.Selection.ActiveCell;
			FormatCellsFormProperties properties = new FormatCellsFormProperties(mergedFormat, documentModel);
			return properties;
		}
		public void ShowFormatCellsFormCallback(FormatCellsFormProperties properties) {
			DocumentModel.BeginUpdateFromUI();
			try {
				ChangeCellFormatCommand command = new ChangeCellFormatCommand(Control);
				IValueBasedCommandUIState<MergedCellFormat> valueBasedState = command.CreateDefaultCommandUIState() as IValueBasedCommandUIState<MergedCellFormat>;
				if (valueBasedState == null)
					return;
				valueBasedState.Value = properties.SourceFormat;
				command.ForceExecute(valueBasedState);
				CellRangeBase ranges = ActiveSheet.Selection.AsRange();
				ranges.Worksheet = ActiveSheet;
				ChangeRangeBordersCommand modelCommand = new ChangeRangeBordersCommand(ranges, properties.GetFinalMergedBorderInfo());
				modelCommand.Execute();
				ActiveSheet.TryBestFitColumn(ranges, ColumnBestFitMode.InplaceEditorMode);
				#region (Un)MergeCommands
				if (properties.MergeCells == CheckState.Checked) {
					EditingMergeCellsCommand commandMerge = new EditingMergeCellsCommand(Control);
					commandMerge.ForceExecute(valueBasedState);
				}
				if (properties.MergeCells == CheckState.Unchecked) {
					EditingUnmergeCellsCommand commandUnmerge = new EditingUnmergeCellsCommand(Control);
					commandUnmerge.ForceExecute(valueBasedState);
				}
				#endregion
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			IValueBasedCommandUIState<FormatCellsFormProperties> state = new DefaultValueBasedCommandUIState<FormatCellsFormProperties>();
			if (!InnerControl.AllowShowingForms)
				state.Value = GetFormatCellsFormProperties();
			return state;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentCapability.Default, !ActiveSheet.Selection.IsDrawingSelected && !InnerControl.IsAnyInplaceEditorActive);
			ApplyActiveSheetProtection(state, !Protection.FormatCellsLocked);
		}
	}
	#endregion
	#region FormatCellsNumberCommand
	public class FormatCellsNumberCommand : FormatCellsCommandBase {
		public FormatCellsNumberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override FormatCellsFormInitialTabPage InitialTabPage { get { return FormatCellsFormInitialTabPage.Number; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatCellsNumber; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsNumber; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsNumberDescription; } }
		#endregion
	}
	#endregion
	#region FormatCellsAlignmentCommand
	public class FormatCellsAlignmentCommand : FormatCellsCommandBase {
		public FormatCellsAlignmentCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override FormatCellsFormInitialTabPage InitialTabPage { get { return FormatCellsFormInitialTabPage.Alignment; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatCellsAlignment; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsAlignment; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsAlignmentDescription; } }
		#endregion
	}
	#endregion
	#region FormatCellsFontCommand
	public class FormatCellsFontCommand : FormatCellsCommandBase {
		public FormatCellsFontCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override FormatCellsFormInitialTabPage InitialTabPage { get { return FormatCellsFormInitialTabPage.Font; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatCellsFont; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsFont; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsFontDescription; } }
		#endregion
	}
	#endregion
	#region FormatCellsBorderCommand
	public class FormatCellsBorderCommand : FormatCellsCommandBase {
		public FormatCellsBorderCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override FormatCellsFormInitialTabPage InitialTabPage { get { return FormatCellsFormInitialTabPage.Border; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatCellsBorder; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsBorder; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsBorderDescription; } }
		#endregion
	}
	#endregion
	#region FormatCellsFillCommand
	public class FormatCellsFillCommand : FormatCellsCommandBase {
		public FormatCellsFillCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override FormatCellsFormInitialTabPage InitialTabPage { get { return FormatCellsFormInitialTabPage.Fill; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatCellsFill; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsFill; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsFillDescription; } }
		#endregion
	}
	#endregion
	#region FormatCellsProtectionCommand
	public class FormatCellsProtectionCommand : FormatCellsCommandBase {
		public FormatCellsProtectionCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override FormatCellsFormInitialTabPage InitialTabPage { get { return FormatCellsFormInitialTabPage.Protection; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatCellsProtection; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsProtection; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsProtectionDescription; } }
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region MergeCellFormatCommand
	public class MergeCellFormatCommand : SpreadsheetChangeSelectedCellsFormattingCommand<ICellFormat> {
		#region Fields
		CellFormatMerger merger;
		#endregion
		public MergeCellFormatCommand(ISpreadsheetControl control)
			: base(control) {
			merger = new CellFormatMerger();
		}
		#region Properties
		protected internal MergedCellFormat MergedCellFormat { get { return merger.MergedCellFormat; } }
		#endregion
		protected internal override void ModifyDocumentModelItems(IEnumerator<ICellFormat> enumerator, ICommandUIState state) {
			while (enumerator.MoveNext()) {
				Modify(enumerator.Current);
			}
		}
		protected internal void Modify(ICellFormat accessor) {
			merger.Merge(accessor);
		}
		protected internal override IEnumerator<Column> GetColumnsAccessorsCore(Worksheet sheet, int nearColumnIndex, int farColumnIndex) {
			return sheet.Columns.GetColumnsForReading(nearColumnIndex, farColumnIndex).GetEnumerator();
		}
		protected internal override IEnumerator<Row> GetRowsAccessorsCore(Worksheet sheet, int nearRowIndex, int farRowIndex) {
			return sheet.Rows.GetRowsForReading(nearRowIndex, farRowIndex).GetEnumerator();
		}
		protected internal override IEnumerator<ICellBase> GetRangeAllCellsAccessorsCore(CellRange range) {
			return range.GetCellsForReadingEnumerator();
		}
		protected override void CreateRowsCellsInterceptWithColumnCells(Worksheet sheet, int leftColumnIndex, int rightColumnIndex) {
		}
		protected internal override ICellFormat GetCellAccessor(ICell cell) {
			return cell;
		}
		protected internal override ICellFormat GetColumnAccessor(Column column) {
			return column;
		}
		protected internal override ICellFormat GetRowAccessor(Row row) {
			return row;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
		}
	}
	#endregion
	#region ChangeCellFormatCommand
	public class ChangeCellFormatCommand : SpreadsheetChangeSelectedCellsFormattingCommand<ICellFormat, MergedCellFormat> {
		#region Fields
		CellFormatModifier modifier;
		#endregion
		public ChangeCellFormatCommand(ISpreadsheetControl control)
			: base(control) {
			this.modifier = new CellFormatModifier();
		}
		#region Properties
		protected internal CellFormatModifier Modifier { get { return modifier; } }
		#endregion
		protected internal override ICellFormat GetCellAccessor(ICell cell) {
			return cell;
		}
		protected internal override ICellFormat GetRowAccessor(Row row) {
			return row;
		}
		protected internal override ICellFormat GetColumnAccessor(Column column) {
			return column;
		}
		protected internal override MergedCellFormat GetActiveCellValue(ICell cell) {
			return null;
		}
		protected internal override void SetValue(ICellFormat accessor, MergedCellFormat value) {
			Modifier.Modify(accessor, value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
		}
	}
	#endregion
	#region FormatCellsContextMenuItemCommand
	public class FormatCellsContextMenuItemCommand : FormatCellsCommandBase {
		public FormatCellsContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override FormatCellsFormInitialTabPage InitialTabPage { get { return FormatCellsFormInitialTabPage.Number; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FormatCellsContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsContextMenuItem; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FormatCellsContextMenuItemDescription; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
