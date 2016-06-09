module __aspxRichEdit {
    export class RestoreRemovedIntervalOperation {
        manipulator: ModelManipulator;
        subDocument: SubDocument;
        currentTableIndex: number = undefined;

        constructor(manipulator: ModelManipulator, subDocument: SubDocument) {
            this.manipulator = manipulator;
            this.subDocument = subDocument;
        }
        execute(removeOperationResult: RemoveIntervalOperationResult) {
            var iterator = removeOperationResult.getIterator();
            var subDocument = this.subDocument;
            let textManipulator = this.manipulator.text;
            var fields: Field[] = this.subDocument.fields;
            var fieldStackHistory: HistoryRunFieldCodeStart[] = [];
            while(iterator.moveNext()) {
                var historyRun = iterator.currentHistoryRun;
                switch(historyRun.type) {
                    case TextRunType.ParagraphRun:
                        if(!(historyRun instanceof HistoryRunParagraph))
                            throw new Error("In unpackHistoryRunsToModel type text run = TextRunType.ParagraphRun, but type historyRun != HistoryRunParagraph. historyRun.offsetAtStartDocument = " +
                                historyRun.offsetAtStartDocument + ", historyRun.text = " + historyRun.text);
                        let historyRunParagraph: HistoryRunParagraph = <HistoryRunParagraph>historyRun;
                        let currentTable = this.getTableForShifting(historyRunParagraph.offsetAtStartDocument);
                        textManipulator.insertParagraph(subDocument, historyRunParagraph.offsetAtStartDocument, historyRunParagraph.characterProperties, historyRunParagraph.characterStyle,
                            historyRunParagraph.paragraphMaskedProperties, historyRunParagraph.paragraphStyle, historyRunParagraph.isInsertPropertiesAndStyleIndexToCurrentParagraph, historyRunParagraph.numbericListIndex, historyRunParagraph.listLevelIndex, historyRunParagraph.tabs.clone());
                        if(currentTable && currentTable.nestedLevel > iterator.currentNestingLevel)
                            this.shiftTablesToPosition(currentTable, historyRunParagraph.offsetAtStartDocument + 1, iterator.currentNestingLevel);
                        break;
                    case TextRunType.SectionRun:
                        if(!(historyRun instanceof HistoryRunSection))
                            throw new Error("In unpackHistoryRunsToModel type text run = TextRunType.SectionRun, but type historyRun != HistoryRunSection. historyRun.offsetAtStartDocument = " +
                                historyRun.offsetAtStartDocument + ", historyRun.text = " + historyRun.text);
                        var historyRunSection: HistoryRunSection = <HistoryRunSection>historyRun;
                        textManipulator.insertSection(subDocument, historyRunSection.offsetAtStartDocument, historyRunSection.characterProperties, historyRunSection.characterStyle,
                            historyRunSection.sectionProperties, true, historyRunSection.paragraphStyle, historyRunSection.paragraphMaskedProperties,
                            historyRunSection.isInsertPropertiesAndStyleIndexToCurrentParagraph, historyRunSection.numbericListIndex, historyRunSection.listLevelIndex, historyRunSection.tabs.clone());
                        break;
                    case TextRunType.InlinePictureRun:
                        if(!(historyRun instanceof HistoryRunInlinePicture))
                            throw new Error("In unpackHistoryRunsToModel type text run = TextRunType.InlinePictureRun, but type historyRun != HistoryRunInlinePicture. historyRun.offsetAtStartDocument = " +
                                historyRun.offsetAtStartDocument + ", historyRun.text = " + historyRun.text);
                        textManipulator.insertInlinePicture(subDocument, historyRun.offsetAtStartDocument, (<HistoryRunInlinePicture>historyRun).id, (<HistoryRunInlinePicture>historyRun).originalWidth,
                            (<HistoryRunInlinePicture>historyRun).originalHeight, (<HistoryRunInlinePicture>historyRun).scaleX, (<HistoryRunInlinePicture>historyRun).scaleY,
                            (<HistoryRunInlinePicture>historyRun).lockAspectRatio, historyRun.characterProperties, historyRun.characterStyle);
                        break;
                    case TextRunType.FieldCodeStartRun:
                        if(!(historyRun instanceof HistoryRunFieldCodeStart))
                            throw new Error("In unpackHistoryRunsToModel type text run = TextRunType.HistoryRunFieldCodeStart, but type historyRun != HistoryRunFieldCodeStart. historyRun.offsetAtStartDocument = " +
                                historyRun.offsetAtStartDocument + ", historyRun.text = " + historyRun.text);
                        fieldStackHistory.push(<HistoryRunFieldCodeStart>historyRun);
                        textManipulator.insertText(subDocument, historyRun.offsetAtStartDocument, historyRun.text, historyRun.characterProperties, historyRun.characterStyle, historyRun.type);
                        break;
                    case TextRunType.FieldResultEndRun:
                        textManipulator.insertText(subDocument, historyRun.offsetAtStartDocument, historyRun.text, historyRun.characterProperties, historyRun.characterStyle, historyRun.type);

                        var histFieldCodeStartRun: HistoryRunFieldCodeStart = fieldStackHistory.pop();

                        var fieldInsertIndex: number = 0;
                        if(fields.length > 0) {
                            fieldInsertIndex = Math.max(0, Field.normedBinaryIndexOf(fields, histFieldCodeStartRun.startPosition + 1));
                            if(histFieldCodeStartRun.startPosition > fields[fieldInsertIndex].getFieldStartPosition())
                                fieldInsertIndex++;
                        }

                        var newField: Field = new Field(subDocument.positionManager, fieldInsertIndex, histFieldCodeStartRun.startPosition, histFieldCodeStartRun.separatorPosition,
                            histFieldCodeStartRun.endPosition, histFieldCodeStartRun.showCode, histFieldCodeStartRun.hyperlinkInfo ? histFieldCodeStartRun.hyperlinkInfo.clone() : undefined);

                        Field.addField(fields, newField);

                        this.manipulator.dispatcher.notifyFieldInserted(subDocument, histFieldCodeStartRun.startPosition, histFieldCodeStartRun.separatorPosition, histFieldCodeStartRun.endPosition);
                        if(histFieldCodeStartRun.hyperlinkInfo)
                            this.manipulator.dispatcher.notifyHyperlinkInfoChanged(subDocument, FixedInterval.fromPositions(histFieldCodeStartRun.separatorPosition + 1, histFieldCodeStartRun.endPosition - 1),
                                histFieldCodeStartRun.hyperlinkInfo);
                        break;
                    default:
                        textManipulator.insertText(subDocument, historyRun.offsetAtStartDocument, historyRun.text, historyRun.characterProperties, historyRun.characterStyle, historyRun.type);
                        break;
                }
            }
        }

        private shiftTablesToPosition(table: Table, position: number, minNestingLevel: number) {
            this.manipulator.tables.changeTableStartPosition(this.subDocument, table, position);
            var prevTable = this.subDocument.tables[table.index - 1];
            if(prevTable && prevTable.nestedLevel > minNestingLevel)
                this.shiftTablesToPosition(prevTable, position, minNestingLevel);
        }

        private getTableForShifting(position: number): Table {
            if(this.currentTableIndex === undefined) {
                this.currentTableIndex = Utils.normedBinaryIndexOf(this.subDocument.tables, t => t.getStartPosition() - position);
                while(this.currentTableIndex > -1 && this.subDocument.tables[this.currentTableIndex].nestedLevel > 0)
                    this.currentTableIndex--;
            }
            var table: Table;
            while(table = this.subDocument.tables[this.currentTableIndex]) {
                if(position >= table.getEndPosition()) {
                    this.currentTableIndex++;
                    continue;
                }
                else if(position < table.getStartPosition())
                    return null;
                var nextTable = this.subDocument.tables[this.currentTableIndex + 1];
                if(!nextTable || nextTable.getStartPosition() > position)
                    return table;
                this.currentTableIndex++;
            }
            return null;
        }
    }
}