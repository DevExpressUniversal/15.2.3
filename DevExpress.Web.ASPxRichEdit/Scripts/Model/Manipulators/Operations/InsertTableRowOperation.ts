module __aspxRichEdit {
    export class InsertTableRowOperationBase {
        control: IRichEditControl;
        subDocument: SubDocument;

        constructor(control: IRichEditControl, subDocument: SubDocument) {
            this.control = control;
            this.subDocument = subDocument;
        }

        execute(table: Table, patternRowIndex: number) {
            let patternRow = table.rows[patternRowIndex];
            let insertParagraphsPositionInfo = this.getInsertParagraphsPositionInfo(table, patternRowIndex);
            this.insertParagraphs(this.subDocument, insertParagraphsPositionInfo, patternRow);
            patternRowIndex = this.insertRowsCore(table, patternRowIndex);
            this.correctVerticalMerging(this.subDocument, table, patternRowIndex);
        }

        protected insertParagraphs(subDocument: SubDocument, insertPosition: { position: number; cell: TableCell }, patternRow: TableRow) {
            var needShiftCellContent = !!insertPosition.cell;
            for(let i = patternRow.cells.length - 1, patternCell: TableCell; patternCell = patternRow.cells[i]; i--) {
                if(needShiftCellContent) {
                    TablesManipulator.insertParagraphToTheCellStartAndShiftContent(this.control, this.subDocument, insertPosition.cell);
                    needShiftCellContent = false;
                    continue;
                }
                let patternCellRun = subDocument.getRunByPosition(patternCell.startParagraphPosition.value);
                let patternCellParagraph = subDocument.getParagraphByPosition(patternCell.startParagraphPosition.value);
                this.control.history.addAndRedo(new InsertParagraphHistoryItem(this.control.modelManipulator, subDocument, insertPosition.position,
                    patternCellRun.maskedCharacterProperties, patternCellRun.characterStyle,
                    patternCellParagraph.maskedParagraphProperties, patternCellParagraph.paragraphStyle, -1, -1, patternCellParagraph.tabs.clone()));
            }
        }
        protected correctVerticalMerging(subDocument: SubDocument, table: Table, patternRowIndex: number) { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
        protected getInsertParagraphsPositionInfo(table: Table, patternRowIndex: number): { position: number; cell: TableCell } {
            throw new Error(Errors.NotImplemented);
        }
        protected insertRowsCore(table: Table, patternRowIndex: number): number {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class InsertTableRowAboveOperation extends InsertTableRowOperationBase {
        protected insertRowsCore(table: Table, patternRowIndex: number): number {
            var newCellsIntervals: FixedInterval[] = [];
            var patternRow = table.rows[patternRowIndex];
            var newCellStartPosition = patternRow.getStartPosition();
            var cellsCount = patternRow.cells.length;
            for(var i = 0; i < cellsCount; i++)
                newCellsIntervals.push(new FixedInterval(newCellStartPosition + i, 1));
            this.control.history.addAndRedo(new InsertTableRowHistoryItem(this.control.modelManipulator, this.subDocument, table.index, table.rows[patternRowIndex], patternRowIndex, newCellsIntervals));
            return patternRowIndex + 1;
        }
        protected getInsertParagraphsPositionInfo(table: Table, patternRowIndex: number): { position: number; cell: TableCell } {
            return {
                cell: table.rows[patternRowIndex].cells[0],
                position: table.rows[patternRowIndex].getStartPosition()
            };
        }
        protected correctVerticalMerging(subDocument: SubDocument, table: Table, patternRowIndex: number) {
            let patternRow = table.rows[patternRowIndex];
            let newRow = table.rows[patternRowIndex - 1];
            for(let i = 0, patternCell: TableCell; patternCell = patternRow.cells[i]; i++) {
                if(patternCell.verticalMerging === TableCellMergingState.Restart)
                    this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, table.index, patternRowIndex - 1, i, TableCellMergingState.None));
            }
        }
    }
    export class InsertTableRowBelowOperation extends InsertTableRowOperationBase {
        protected insertRowsCore(table: Table, patternRowIndex: number): number {
            var newCellsIntervals: FixedInterval[] = [];
            var patternRow = table.rows[patternRowIndex];
            var newCellStartPosition = patternRow.getEndPosition();
            var cellsCount = patternRow.cells.length;
            for(var i = 0; i < cellsCount; i++)
                newCellsIntervals.push(new FixedInterval(newCellStartPosition + i, 1));
            this.control.history.addAndRedo(new InsertTableRowHistoryItem(this.control.modelManipulator, this.subDocument, table.index, table.rows[patternRowIndex], patternRowIndex + 1, newCellsIntervals));
            return patternRowIndex;
        }
        protected getInsertParagraphsPositionInfo(table: Table, patternRowIndex: number): { position: number; cell: TableCell } {
            return {
                cell: table.rows[patternRowIndex + 1] ? table.rows[patternRowIndex + 1].cells[0] : null,
                position: table.rows[patternRowIndex].getEndPosition()
            };
        }
        protected correctVerticalMerging(subDocument: SubDocument, table: Table, patternRowIndex: number) {
            let patternRow = table.rows[patternRowIndex];
            let newRowIndex = patternRowIndex + 1;
            let newRow = table.rows[newRowIndex];
            let nextRow = table.rows[newRowIndex + 1];
            for(let i = 0, patternCell: TableCell; patternCell = patternRow.cells[i]; i++) {
                if(patternCell.verticalMerging === TableCellMergingState.Continue) {
                    if(!nextRow)
                        this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, table.index, newRowIndex, i, TableCellMergingState.None));
                    else {
                        let sourceCellStartColumnIndex = TableCellUtils.getStartColumnIndex(patternCell);
                        let indexInNextRow = TableCellUtils.getAbsoluteCellIndexInRow(nextRow, sourceCellStartColumnIndex);
                        if(nextRow.cells[indexInNextRow].verticalMerging !== TableCellMergingState.Continue)
                            this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, table.index, newRowIndex, i, TableCellMergingState.None));
                    }
                }
                else if(patternCell.verticalMerging === TableCellMergingState.Restart)
                    this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, subDocument, table.index, newRowIndex, i, TableCellMergingState.Continue));
            }
        }
    }
}