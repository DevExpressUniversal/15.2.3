module __aspxRichEdit {
    export class ChangeTableCellAlignmentCommandBase extends CommandBase<SimpleCommandState> {
        horizontalAlignment: ParagraphAlignment;
        verticalAlignment: TableCellVerticalAlignment;

        getState(): SimpleCommandState {
            let state = new SimpleCommandState(true, false);
            let selectedCells = this.control.selection.getSelectedCells();
            state.enabled = this.isEnabled() && selectedCells.length > 0;
            state.value = this.isChecked(selectedCells);
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables);
        }
        isChecked(selectedCells: TableCell[][]): boolean {
            for(let i = 0, horCells: TableCell[]; horCells = selectedCells[i]; i++) {
                for(let j = 0, cell: TableCell; cell = horCells[j]; j++) {
                    let cellVerticalAlignment = new TableCellVerticalAlignmentMerger().getProperty(cell.properties, cell.parentRow.parentTable.style, cell.conditionalFormatting, this.control.model.defaultTableCellProperties);
                    if(cellVerticalAlignment !== this.verticalAlignment)
                        return false;
                    let fstParagraph = this.control.model.activeSubDocument.getParagraphByPosition(cell.startParagraphPosition.value);
                    if(fstParagraph.getParagraphMergedProperies().alignment !== this.horizontalAlignment)
                        return false;
                }
            }
            return true;
        }
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.history.beginTransaction();
            let selectedCells = this.control.selection.getSelectedCells();
            let subDocument = this.control.model.activeSubDocument;
            let table = selectedCells[0][0].parentRow.parentTable;
            for(var i = 0, horCells: TableCell[]; horCells = selectedCells[i]; i++) {
                let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - horCells[0].startParagraphPosition.value);
                for(var j = 0, cell: TableCell; cell = horCells[j]; j++) {
                    if(cell.properties.verticalAlignment !== this.verticalAlignment || !cell.properties.getUseValue(TableCellPropertiesMask.UseVerticalAlignment)) {
                        let cellIndex = Utils.normedBinaryIndexOf(table.rows[rowIndex].cells, c => c.startParagraphPosition.value - cell.startParagraphPosition.value);
                        this.control.history.addAndRedo(new TableCellVerticalAlignmentHistoryItem(this.control.modelManipulator, subDocument, table.index, rowIndex, cellIndex, this.verticalAlignment, true));
                    }
                    let paragraphIndices = subDocument.getParagraphsIndices(cell.getInterval());
                    for(let parIndex = paragraphIndices.start; parIndex <= paragraphIndices.end; parIndex++) {
                        let paragraph = subDocument.paragraphs[parIndex];
                        if(paragraph.getParagraphMergedProperies().alignment !== this.horizontalAlignment)
                            this.control.history.addAndRedo(new ParagraphAlignmentHistoryItem(this.control.modelManipulator, subDocument, paragraph.getInterval(), this.horizontalAlignment, true));
                    }
                }
            }
            this.control.history.endTransaction();
            return true;
        }
    }

    export class ChangeTableCellTopLeftAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Left;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Top;
    }

    export class ChangeTableCellTopCenterAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Center;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Top;
    }

    export class ChangeTableCellTopRightAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Right;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Top;
    }

    export class ChangeTableCellMiddleLeftAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Left;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Center;
    }

    export class ChangeTableCellMiddleCenterAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Center;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Center;
    }

    export class ChangeTableCellMiddleRightAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Right;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Center;
    }

    export class ChangeTableCellBottomLeftAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Left;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Bottom;
    }

    export class ChangeTableCellBottomCenterAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Center;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Bottom;
    }

    export class ChangeTableCellBottomRightAlignmentCommand extends ChangeTableCellAlignmentCommandBase {
        horizontalAlignment: ParagraphAlignment = ParagraphAlignment.Right;
        verticalAlignment: TableCellVerticalAlignment = TableCellVerticalAlignment.Bottom;
    }
}