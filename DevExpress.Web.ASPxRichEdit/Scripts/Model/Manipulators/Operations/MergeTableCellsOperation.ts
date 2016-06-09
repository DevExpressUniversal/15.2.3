module __aspxRichEdit {
    export class MergeTableCellsOperationBase {
        control: IRichEditControl;
        subDocument: SubDocument;
        needDeleteNextTableCell: boolean = false;

        constructor(control: IRichEditControl, subDocument: SubDocument) {
            this.control = control;
            this.subDocument = subDocument;
        }

        execute(position: TablePosition, suppressNormalizeTableRows: boolean) {
            let nextCellPosition = this.calculateNextCell(position);
            this.updateCellsProperties(position, nextCellPosition);

            let nextCell = nextCellPosition.cell;
            let cell = position.cell;

            let isEmptyCell = this.isEmptyCell(cell);
            let isEmptyNextCell = this.isEmptyCell(nextCell);

            if(!isEmptyNextCell) {
                let insertPosition = cell.endParagrapPosition.value - 1;
                let nextCellLastParagraph = this.subDocument.paragraphs[this.getCellLastParagraphIndex(nextCell)];
                if(!isEmptyCell) {
                    ModelManipulator.insertParagraph(this.control, this.subDocument, new FixedInterval(insertPosition, 0));
                    insertPosition++;
                }
                this.applyParagraphProperties(this.getCellLastParagraphIndex(cell), nextCellLastParagraph);
                ModelManipulator.copyIntervalTo(this.control, this.subDocument, FixedInterval.fromPositions(nextCell.startParagraphPosition.value, nextCell.endParagrapPosition.value - 1), insertPosition);
            }
            this.deleteTableCellWithContent(nextCellPosition);
            TablesManipulator.normalizeCellColumnSpans(this.control, this.subDocument, position.table, true);
            if(!suppressNormalizeTableRows)
                TablesManipulator.normalizeRows(this.control, this.subDocument, cell.parentRow.parentTable);
        }

        private getCellLastParagraphIndex(cell: TableCell): number {
            return this.subDocument.getParagraphIndexByPosition(cell.endParagrapPosition.value - 1);
        }

        private applyParagraphProperties(targetIndex: number, source: Paragraph) {
            let target = this.subDocument.paragraphs[targetIndex];
            if(!target.maskedParagraphProperties.equals(source.maskedParagraphProperties) || target.numberingListIndex !== source.numberingListIndex || target.listLevelIndex !== source.listLevelIndex || target.tabs.equals(source.tabs) || target.paragraphStyle !== source.paragraphStyle)
                this.control.history.addAndRedo(new ParagraphPropertiesHistoryItem(this.control.modelManipulator, this.subDocument, targetIndex, source.maskedParagraphProperties, source.paragraphStyle, source.numberingListIndex, source.listLevelIndex, source.tabs));
        }
        protected deleteTableCellWithContent(nextCellPosition: TablePosition) {
            if(this.needDeleteNextTableCell)
                TablesManipulator.removeTableCellWithContent(this.control, this.subDocument, nextCellPosition.table, nextCellPosition.rowIndex, nextCellPosition.cellIndex);
            else {
                let nextCell = nextCellPosition.cell;
                ModelManipulator.removeInterval(this.control, this.subDocument, FixedInterval.fromPositions(nextCell.startParagraphPosition.value, nextCell.endParagrapPosition.value - 1), true);
            }
        }
        protected isEmptyCell(cell: TableCell) {
            return cell.endParagrapPosition.value - cell.startParagraphPosition.value === 1;
        }
        protected calculateNextCell(position: TablePosition): TablePosition {
            throw new Error(Errors.NotImplemented);
        }
        protected updateCellsProperties(patternCellPosition: TablePosition, nextCellPosition: TablePosition) {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class MergeTwoTableCellsHorizontallyOperation extends MergeTableCellsOperationBase {
        needDeleteNextTableCell = true;
        protected calculateNextCell(position: TablePosition): TablePosition {
            let nextCellIndex = position.cellIndex + 1;
            return TablePosition.createAndInit(position.table, position.rowIndex, nextCellIndex);
        }
        protected updateCellsProperties(patternCellPosition: TablePosition, nextCellPosition: TablePosition) {
            let patternCell = patternCellPosition.cell;
            let nextCell = nextCellPosition.cell;
            this.control.history.addAndRedo(new TableCellColumnSpanHistoryItem(this.control.modelManipulator, this.subDocument, patternCellPosition.table.index, patternCellPosition.rowIndex, patternCellPosition.cellIndex, patternCell.columnSpan + nextCell.columnSpan));

            let patternCellPreferredWidth = patternCell.preferredWidth;
            let nextCellPreferredWidth = nextCell.preferredWidth;

            if(patternCellPreferredWidth && nextCellPreferredWidth && nextCellPreferredWidth.type === patternCellPreferredWidth.type) {
                let newPreferredWidth = TableWidthUnit.create(patternCellPreferredWidth.value + nextCellPreferredWidth.value, patternCellPreferredWidth.type);
                this.control.history.addAndRedo(new TableCellPreferredWidthHistoryItem(this.control.modelManipulator, this.subDocument, patternCellPosition.table.index, patternCellPosition.rowIndex, patternCellPosition.cellIndex, newPreferredWidth));
            }
        }
    }

    export class MergeTwoTableCellsVerticallyOperation extends MergeTableCellsOperationBase {
        protected calculateNextCell(position: TablePosition): TablePosition {
            let nextRowIndex = position.rowIndex + 1;
            let nextRow = position.table.rows[nextRowIndex];
            let columnIndex = TableCellUtils.getStartColumnIndex(position.cell);
            return TablePosition.createAndInit(position.table, nextRowIndex, TableCellUtils.getCellIndexByColumnIndex(nextRow, columnIndex));
        }
        protected updateCellsProperties(patternCellPosition: TablePosition, nextCellPosition: TablePosition) {
            this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, this.subDocument, patternCellPosition.table.index, patternCellPosition.rowIndex, patternCellPosition.cellIndex, TableCellMergingState.Restart));
            this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, this.subDocument, nextCellPosition.table.index, nextCellPosition.rowIndex, nextCellPosition.cellIndex, TableCellMergingState.Continue));
        }
    }

    export class InsertTableCellWithShiftToTheDownOperation extends MergeTableCellsOperationBase {
        protected calculateNextCell(position: TablePosition): TablePosition {
            return TablePosition.createAndInit(position.table, position.rowIndex - 1, position.cellIndex);
        }
        protected updateCellsProperties(patternCellPosition: TablePosition, nextCellPosition: TablePosition) {
            let manipulator = this.control.modelManipulator;
            let subDocument = this.subDocument;
            this.control.history.addAndRedo(new TableCellBordersHistoryItem(manipulator, subDocument, patternCellPosition.table.index, patternCellPosition.rowIndex, patternCellPosition.cellIndex,
                [
                    nextCellPosition.cell.properties.borders.topBorder.clone(),
                    nextCellPosition.cell.properties.borders.rightBorder.clone(),
                    nextCellPosition.cell.properties.borders.bottomBorder.clone(),
                    nextCellPosition.cell.properties.borders.leftBorder.clone(),
                    nextCellPosition.cell.properties.borders.topLeftDiagonalBorder.clone(),
                    nextCellPosition.cell.properties.borders.topRightDiagonalBorder.clone()
                ], [true, true, true, true, true, true]));
            this.control.history.addAndRedo(new TableCellBackgroundColorHistoryItem(manipulator, subDocument, patternCellPosition.table.index, patternCellPosition.rowIndex, patternCellPosition.cellIndex, nextCellPosition.cell.properties.backgroundColor, true));
            this.control.history.addAndRedo(new TableCellVerticalAlignmentHistoryItem(manipulator, subDocument, patternCellPosition.table.index, patternCellPosition.rowIndex, patternCellPosition.cellIndex, nextCellPosition.cell.properties.verticalAlignment, true));
        }
        protected deleteTableCellWithContent(nextCellPosition: TablePosition) {
            let nextCell = nextCellPosition.cell;
            if(nextCell.endParagrapPosition.value - nextCell.startParagraphPosition.value > 1)
                ModelManipulator.removeInterval(this.control, this.subDocument, FixedInterval.fromPositions(nextCell.startParagraphPosition.value, nextCell.endParagrapPosition.value - 1), true);
        }
    }

    export class DeleteOneTableCellWithShiftToTheUpOperation extends MergeTwoTableCellsVerticallyOperation {
        execute(position: TablePosition, suppressNormalizeTableRows: boolean) {
            if(position.rowIndex === position.table.rows.length - 1)
                this.deleteContentFromCell(position.cell);
            else {
                this.deleteContentFromCell(position.cell);
                super.execute(position, suppressNormalizeTableRows);
            }
        }
        protected updateCellsProperties(patternCellPosition: TablePosition, nextCellPosition: TablePosition) {
            if(patternCellPosition.cell.verticalMerging === TableCellMergingState.Restart) {
                this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, this.subDocument, patternCellPosition.table.index, patternCellPosition.rowIndex, patternCellPosition.cellIndex, TableCellMergingState.None));
                if(nextCellPosition.rowIndex === nextCellPosition.table.rows.length - 1)
                    this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, this.subDocument, nextCellPosition.table.index, nextCellPosition.rowIndex, nextCellPosition.cellIndex, TableCellMergingState.None));
                else {
                    let afterNextCellPosition = this.calculateNextCell(nextCellPosition);
                    if(afterNextCellPosition.cell && afterNextCellPosition.cell.verticalMerging === TableCellMergingState.Continue)
                        this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, this.subDocument, nextCellPosition.table.index, nextCellPosition.rowIndex, nextCellPosition.cellIndex, TableCellMergingState.Restart));
                    else
                        this.control.history.addAndRedo(new TableCellVerticalMergingHistoryItem(this.control.modelManipulator, this.subDocument, nextCellPosition.table.index, nextCellPosition.rowIndex, nextCellPosition.cellIndex, TableCellMergingState.None));
                }
            }
        }
        deleteContentFromCell(cell: TableCell) {
            let cellInterval = cell.getInterval();
            if(--cellInterval.length > 0)
                ModelManipulator.removeInterval(this.control, this.subDocument, cellInterval, true);
        }
    }
}