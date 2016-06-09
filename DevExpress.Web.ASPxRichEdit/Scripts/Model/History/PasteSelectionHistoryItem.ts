module __aspxRichEdit {
    export class PasteSelectionHistoryItem extends HistoryItem {
        interval: FixedInterval;
        maskedCharacterProperties: MaskedCharacterProperties;
        characterStyle: CharacterStyle;
        historyRuns: HistoryRun[];
        historyTables: Table[];
        tablesInfo: { [position: number]: ImportedTableInfo } = {};

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval, historyRuns: HistoryRun[], tablesInfo: { [position: number]: ImportedTableInfo },
            maskedCharacterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle) {
            super(modelManipulator, boundSubDocument);
            this.interval = interval;
            this.maskedCharacterProperties = maskedCharacterProperties;
            this.characterStyle = characterStyle;
            this.historyRuns = historyRuns;
            this.tablesInfo = tablesInfo;
            this.historyTables = [];
        }

        public redo() {
            var fields: Field[] = this.boundSubDocument.fields;
            var fieldStackHistory: HistoryRunFieldCodeStart[] = [];
            for(var historyRunIndex: number = 0, historyRun: HistoryRun; historyRun = this.historyRuns[historyRunIndex]; historyRunIndex++) {
                switch(historyRun.type) {
                    case TextRunType.ParagraphRun:
                        var historyRunParagraph: HistoryRunParagraph = <HistoryRunParagraph>historyRun;
                        this.modelManipulator.text.insertParagraph(this.boundSubDocument, historyRunParagraph.offsetAtStartDocument, historyRunParagraph.characterProperties, historyRunParagraph.characterStyle,
                            historyRunParagraph.paragraphMaskedProperties, historyRunParagraph.paragraphStyle, historyRunParagraph.isInsertPropertiesAndStyleIndexToCurrentParagraph,
                            historyRunParagraph.numbericListIndex, historyRunParagraph.listLevelIndex, historyRunParagraph.tabs.clone());
                        break;
                    case TextRunType.InlinePictureRun:
                        var historyInlinePictureRun = <HistoryRunInlinePicture>historyRun;
                        this.modelManipulator.text.insertInlinePicture(this.boundSubDocument, historyInlinePictureRun.offsetAtStartDocument, historyInlinePictureRun.id,
                            historyInlinePictureRun.originalWidth, historyInlinePictureRun.originalHeight, historyInlinePictureRun.scaleX, historyInlinePictureRun.scaleY,
                            historyInlinePictureRun.lockAspectRatio, this.maskedCharacterProperties, this.characterStyle, historyInlinePictureRun.isLoaded);
                        if(!historyInlinePictureRun.isLoaded) {
                            this.modelManipulator.text.loadingInlinePicturesHashtable[historyInlinePictureRun.guid] = {
                                run: <InlinePictureRun>this.boundSubDocument.getRunByPosition(historyInlinePictureRun.offsetAtStartDocument),
                                historyRun: historyInlinePictureRun
                            };
                        }
                        break;
                    case TextRunType.FieldCodeStartRun:
                        fieldStackHistory.push(<HistoryRunFieldCodeStart>historyRun);
                        this.modelManipulator.text.insertText(this.boundSubDocument, historyRun.offsetAtStartDocument, historyRun.text, historyRun.characterProperties, historyRun.characterStyle, historyRun.type);
                        break;
                    case TextRunType.FieldResultEndRun:
                        this.modelManipulator.text.insertText(this.boundSubDocument, historyRun.offsetAtStartDocument, historyRun.text, historyRun.characterProperties, historyRun.characterStyle, historyRun.type);

                        var histFieldCodeStartRun: HistoryRunFieldCodeStart = fieldStackHistory.pop();

                        var fieldInsertIndex: number = 0;
                        if(fields.length > 0) {
                            fieldInsertIndex = Math.max(0, Field.normedBinaryIndexOf(fields, histFieldCodeStartRun.startPosition + 1));
                            if(histFieldCodeStartRun.startPosition > fields[fieldInsertIndex].getFieldStartPosition())
                                fieldInsertIndex++;
                        }

                        var field = new Field(this.boundSubDocument.positionManager, fieldInsertIndex, histFieldCodeStartRun.startPosition, histFieldCodeStartRun.separatorPosition,
                            histFieldCodeStartRun.endPosition, histFieldCodeStartRun.showCode, histFieldCodeStartRun.hyperlinkInfo ? histFieldCodeStartRun.hyperlinkInfo.clone() : undefined);
                        Field.addField(fields, field);

                        this.modelManipulator.dispatcher.notifyFieldInserted(this.boundSubDocument, histFieldCodeStartRun.startPosition, histFieldCodeStartRun.separatorPosition, histFieldCodeStartRun.endPosition);
                        if(histFieldCodeStartRun.hyperlinkInfo) {
                            this.modelManipulator.dispatcher.notifyHyperlinkInfoChanged(this.boundSubDocument, FixedInterval.fromPositions(histFieldCodeStartRun.separatorPosition + 1, histFieldCodeStartRun.endPosition - 1),
                                histFieldCodeStartRun.hyperlinkInfo);
                            this.modelManipulator.styles.setCharacterStyle(this.boundSubDocument, field.getResultInterval(), this.boundSubDocument.documentModel.getCharacterStyleByName("Hyperlink"));
                        }
                        break;
                    default:
                        this.modelManipulator.text.insertText(this.boundSubDocument, historyRun.offsetAtStartDocument, historyRun.text,
                            historyRun.characterProperties, historyRun.characterStyle, historyRun.type);
                        break;
                }
            }
            Field.DEBUG_FIELDS_CHECKS(this.boundSubDocument);

            for(var position in this.tablesInfo) {
                let patternTable = new Table(new TableProperties(), this.boundSubDocument.documentModel.getTableStyleByName(TableStyle.SIMPLE_STYLENAME));
                let tableInfo = this.tablesInfo[position];
                for(let i = 0; i < tableInfo.rowCount; i++) {
                    let row = new TableRow(patternTable, this.boundSubDocument.documentModel.cache.tableRowPropertiesCache.addItemIfNonExists(new TableRowProperties()));
                    patternTable.rows.push(row);
                    row.tablePropertiesException = new TableProperties();
                    for(let j = 0; j < tableInfo.cellCount; j++) {
                        let cell = new TableCell(row, this.boundSubDocument.documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(new TableCellProperties()));
                        row.cells.push(cell);
                    }
                }

                let firstParagraphIndex = Utils.normedBinaryIndexOf(this.boundSubDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - position);
                let extraParagraphCount: number = 0;
                for(let rowIndex = 0, rowInfo: ImportedTableRowInfo; rowInfo = tableInfo.rows[rowIndex]; rowIndex++) {
                    let mergingContinueCellCount = 0;
                    let tableRow = patternTable.rows[rowIndex];
                    for(let cellInfoIndex = 0; cellInfoIndex < tableInfo.cellCount; cellInfoIndex++) {
                        if(tableRow.cells[cellInfoIndex] && tableRow.cells[cellInfoIndex].verticalMerging == TableCellMergingState.Continue) {
                            let prevCellEndPosition = (cellInfoIndex == 0 ? patternTable.rows[rowIndex - 1].cells[patternTable.rows[rowIndex - 1].cells.length - 1]
                                : patternTable.rows[rowIndex].cells[cellInfoIndex - 1]).getInterval().end();

                            this.modelManipulator.text.insertParagraph(this.boundSubDocument, prevCellEndPosition, this.maskedCharacterProperties, this.characterStyle,
                                undefined, undefined, true, undefined, undefined, undefined);
                            let insertedParagraph = this.boundSubDocument.getParagraphByPosition(prevCellEndPosition);
                            tableRow.cells[cellInfoIndex].startParagraphPosition = new Position(insertedParagraph.startLogPosition.value);
                            tableRow.cells[cellInfoIndex].endParagrapPosition = new Position(insertedParagraph.getEndPosition());
                            tableRow.cells[cellInfoIndex].preferredWidth = patternTable.rows[rowIndex - 1].cells[cellInfoIndex].preferredWidth.clone();
                            mergingContinueCellCount++;
                            extraParagraphCount++;
                        }

                        let cellIndex = cellInfoIndex + mergingContinueCellCount;
                        let tableCell = tableRow.cells[cellIndex];
                        let cellInfo = rowInfo.cells[cellInfoIndex];

                        if(!tableCell || !cellInfo)
                            continue;

                        if(cellInfo.columnSpan > 1) {
                            tableCell.columnSpan = cellInfo.columnSpan;
                            tableRow.cells.splice(cellIndex + 1, cellInfo.columnSpan - 1);
                        }

                        let startParagraph = this.boundSubDocument.paragraphs[firstParagraphIndex + cellInfo.startParagraphIndex + extraParagraphCount];
                        if(startParagraph)
                            tableCell.startParagraphPosition = new Position(startParagraph.startLogPosition.value);
                        let endParagraph = this.boundSubDocument.paragraphs[firstParagraphIndex + cellInfo.endParagraphIndex + extraParagraphCount];
                        if(endParagraph)
                            tableCell.endParagrapPosition = new Position(endParagraph.getEndPosition());

                        if(cellInfo.rowSpan > 1) {
                            tableCell.verticalMerging = TableCellMergingState.Restart;
                            for(let i = rowIndex + 1; i < rowIndex + cellInfo.rowSpan; i++) {
                                let nextRow = patternTable.rows[i];
                                let nextRowCellIndex = cellIndex;
                                if(nextRow.cells.length != tableRow.cells.length) {
                                    let extraCellsCount = 0;
                                    let isNextRowLonger = nextRow.cells.length > tableRow.cells.length;
                                    let shorterRow = isNextRowLonger ? tableRow : nextRow;
                                    for(let j = 0; (j < cellIndex) && (j < shorterRow.cells.length); j++) {
                                        extraCellsCount += shorterRow.cells[j].columnSpan - 1;
                                        if(!isNextRowLonger)
                                            extraCellsCount -= tableRow.cells[j].columnSpan - 1;
                                    }
                                    nextRowCellIndex += (isNextRowLonger ? 1 : -1) * extraCellsCount;
                                }
                                let mergedCell = nextRow.cells[nextRowCellIndex];
                                if(mergedCell)
                                    mergedCell.verticalMerging = TableCellMergingState.Continue;
                            }
                        }
                    }
                }

                let positionToPaste = this.boundSubDocument.paragraphs[firstParagraphIndex].startLogPosition.value;
                this.modelManipulator.tables.pasteTable(this.boundSubDocument, patternTable, positionToPaste);
                var table = Table.getTableByPosition(this.boundSubDocument.tables, positionToPaste, true);
                if(table) {
                    this.modelManipulator.dispatcher.notifyTableCreated(this.boundSubDocument, table);
                    let tableWidth = tableInfo.width;
                    let totalColumnsInTable = TablesManipulator.findTotalColumnsCountInTable(table);
                    var widths = TablesManipulator.distributeWidthsToAllColumns(tableWidth, totalColumnsInTable);
                    TablesManipulator.forEachCell(table, (cell, ci, ri) => {
                        let cellWidth = widths[ci];
                        this.modelManipulator.tables.cellProperties.preferredWidth.setValue(this.boundSubDocument, table.index, ri, ci, TableWidthUnit.create(cellWidth, TableWidthUnitType.ModelUnits));
                    });
                    this.historyTables.push(table);
                }
            }
        }

        public undo() {
            this.modelManipulator.text.removeIntervalWithoutHistory(this.boundSubDocument, this.interval, false);
            for(let i = 0, table: Table; table = this.historyTables[i]; i++)
                this.modelManipulator.tables.removeTable(this.boundSubDocument, table);
        }
    }
} 