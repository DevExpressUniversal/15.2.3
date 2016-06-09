module __aspxRichEdit {
    export class TextManipulator {
        manipulator: ModelManipulator;
        chunkSizeCorrector: ChunkSizeCorrector;
        loadingInlinePicturesHashtable: { [key: string]: { run: InlinePictureRun; historyRun: HistoryRunInlinePicture } } = {};

        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
            this.chunkSizeCorrector = new ChunkSizeCorrector();
        }

        public removeInterval(subDocument: SubDocument, interval: FixedInterval, setPropertiesSecondParagraph: boolean): RemoveIntervalOperationResult {
            var operation = new RemoveIntervalOperation(this.manipulator, subDocument);
            return operation.execute(interval, setPropertiesSecondParagraph, true);
        }

        public removeIntervalWithoutHistory(subDocument: SubDocument, interval: FixedInterval, setPropertiesSecondParagraph: boolean) {
            var operation = new RemoveIntervalOperation(this.manipulator, subDocument);
            operation.execute(interval, setPropertiesSecondParagraph, false);
        }
        
        // Insert Text (insertPositionAtStartDocument = [0, number symbols in document - 1)) (We put run in the beginning of the chunk. Not End )
        public insertText(subDocument: SubDocument, insertPositionAtStartDocument: number, text: string, maskedCharacterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle,
                runType: TextRunType) {
            if (!!maskedCharacterProperties == false)
                throw new Error("In insertText need set clearly maskedCharacterProperties");
            var insertedRun: { chunkIndex: number; runIndex: number } = this.insertRunInternal(subDocument, insertPositionAtStartDocument, runType, text, maskedCharacterProperties, characterStyle);
            var textRun = subDocument.chunks[insertedRun.chunkIndex].textRuns[insertedRun.runIndex];
            textRun.paragraph.length += text.length;
            this.chunkSizeCorrector.correctChunkSizeAtInsertPosition(subDocument, insertPositionAtStartDocument);
            this.manipulator.dispatcher.notifySimpleRunInserted(subDocument, insertPositionAtStartDocument, text.length, textRun.maskedCharacterProperties, textRun.characterStyle, runType);
        }

        public insertInlinePicture(subDocument: SubDocument, insertPositionAtStartDocument: number, id: number, originalWidth: number, originalHeight: number, scaleX: number, scaleY: number,
            lockAspectRatio: boolean, maskedCharacterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle, isLoaded: boolean = true) {
            if (!!maskedCharacterProperties == false)
                throw new Error("In insertInlinePicture need set clearly maskedCharacterProperties");
            var insertedRun: { chunkIndex: number; runIndex: number } = this.insertRunInternal(subDocument, insertPositionAtStartDocument, TextRunType.InlinePictureRun,
                subDocument.documentModel.specChars.ObjectMark, maskedCharacterProperties, characterStyle);
            var pictureRun: InlinePictureRun = <InlinePictureRun>subDocument.chunks[insertedRun.chunkIndex].textRuns[insertedRun.runIndex];
            pictureRun.id = id;
            pictureRun.lockAspectRatio = lockAspectRatio;
            pictureRun.originalHeight = originalHeight;
            pictureRun.originalWidth = originalWidth;
            pictureRun.scaleX = scaleX;
            pictureRun.scaleY = scaleY;
            pictureRun.isLoaded = isLoaded;
            subDocument.chunks[insertedRun.chunkIndex].textRuns[insertedRun.runIndex].paragraph.length++;
            this.manipulator.dispatcher.notifyInlinePictureInserted(subDocument, insertPositionAtStartDocument, id, scaleX, scaleY);
        }

        public applyLoadedImages(subDocument: SubDocument, loadedImagesInfo: { [guid: string]: { imageCacheId: number; originalWidth: number; originalHeight: number } }) {
            var updatedImagesInfo: { position: number; id: number; scaleX: number; scaleY: number }[] = [];
            var columnsCalculator = new ColumnsCalculator(new TwipsUnitConverter());
            for(var guid in loadedImagesInfo) {
                if(this.loadingInlinePicturesHashtable[guid]) {
                    var pictureRun = this.loadingInlinePicturesHashtable[guid].run;
                    var historyRun = this.loadingInlinePicturesHashtable[guid].historyRun;

                    pictureRun.isLoaded = true;

                    var imageCacheId = loadedImagesInfo[guid].imageCacheId;
                    pictureRun.id = imageCacheId;
                    historyRun.id = imageCacheId;

                    var originalWidth = loadedImagesInfo[guid].originalWidth;
                    var originalHeight = loadedImagesInfo[guid].originalHeight;
                    pictureRun.originalHeight = originalHeight;
                    pictureRun.originalWidth = originalWidth;
                    historyRun.originalHeight = originalHeight;
                    historyRun.originalWidth = originalWidth;

                    var position = historyRun.offsetAtStartDocument;
                    if(subDocument.getRunByPosition(position) != pictureRun) {
                        var chunkIndex = Utils.normedBinaryIndexOf(subDocument.chunks, (c: Chunk) => c.startLogPosition.value - pictureRun.paragraph.startLogPosition.value);
                        for(var i = chunkIndex, chunk: Chunk; chunk = subDocument.chunks[i]; i++) {
                            if(chunk.textRuns.indexOf(pictureRun) > -1) {
                                position = chunk.startLogPosition.value + pictureRun.startOffset;
                                break;
                            }
                        }
                    }

                    var minimalSize = columnsCalculator.findMinimalColumnSize(subDocument.documentModel.getSectionByPosition(position).sectionProperties);
                    var scale = 100 * Math.min(UnitConverter.pixelsToTwips(minimalSize.width) / Math.max(1, originalWidth),
                        UnitConverter.pixelsToTwips(minimalSize.height) / Math.max(1, originalHeight));
                    var resultScale = Math.max(1, Math.min(scale, 100));
                    pictureRun.scaleX = resultScale;
                    pictureRun.scaleY = resultScale;
                    historyRun.scaleX = resultScale;
                    historyRun.scaleY = resultScale;

                    updatedImagesInfo.push({
                        position: position,
                        id: pictureRun.id,
                        scaleX: pictureRun.scaleX,
                        scaleY: pictureRun.scaleY
                    });
                }
            }
            if(updatedImagesInfo.length)
                this.manipulator.dispatcher.notifyInlinePicturesUpdated(subDocument, updatedImagesInfo);
        }
        
        // Insert paragraph mark // paragraphFormatting can be null, paragraphStyleIndex can be -1
        public insertParagraph(subDocument: SubDocument, insertPositionAtStartDocument: number, maskedCharacterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle,
            paragraphMaskedProperties: MaskedParagraphProperties, paragraphStyle: ParagraphStyle, isInsertPropertiesAndStyleIndexToCurrentParagraph: boolean, numberingListIndex: number, listLevelIndex: number, tabs: TabProperties) {
            if (!!maskedCharacterProperties == false)
                throw new Error("In insertParagraph need set clearly maskedCharacterProperties");

            // [..]+[.......]+[........]+                 runs
            // oooonooooooooonoooooooooon                 text
            // [...][........][.........]                 paragraphs
            //                                            insert at index 10 - >
            // oooonooooonoooonoooooooooon
            // [...][.new][old][.........]                pragraphs
            // [..]+[...]*[..]+[........]+                runs   (* - mean new ParagraphRun)

            if (this.insertParagraphInEnd(subDocument, insertPositionAtStartDocument, maskedCharacterProperties, characterStyle)) {
                this.manipulator.dispatcher.notifyParagraphInserted(subDocument, insertPositionAtStartDocument);
                return;
            }

            var insertedRun: { chunkIndex: number; runIndex: number } = this.insertRunInternal(subDocument, insertPositionAtStartDocument, TextRunType.ParagraphRun,
                subDocument.documentModel.specChars.ParagraphMark, maskedCharacterProperties, characterStyle);
            var currentRun = subDocument.chunks[insertedRun.chunkIndex].textRuns[insertedRun.runIndex];

            var oldParagraphIndex: number = Utils.normedBinaryIndexOf(subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - (insertPositionAtStartDocument + 1));
            var oldParagraph: Paragraph = subDocument.paragraphs[oldParagraphIndex];
            subDocument.positionManager.unregisterPosition(oldParagraph.startLogPosition);//  mean we calcuate position later

            var newParagraph: Paragraph = new Paragraph(subDocument, null, 1, null, null); // null mean we calcuate position later, 1 - length paragraph
            newParagraph.onParagraphPropertiesChanged();

            this.applyParagraphProperties(newParagraph, oldParagraph, !isInsertPropertiesAndStyleIndexToCurrentParagraph, paragraphStyle, paragraphMaskedProperties, numberingListIndex, listLevelIndex, tabs);
            currentRun.paragraph = newParagraph;

            subDocument.paragraphs.splice(oldParagraphIndex, 0, newParagraph);

            // why "-1"?
            // o o o n o o o o n  text
            // [ ] [           ]  sections
            // o ono n o o o o n insert here
            //    ^ - here offsetAtStartChunk. Then we use -1 to find prev section

            var indexRun: number = insertedRun.runIndex - 1; // "-1" because we don't touch inserted "\n"
            completeExecuteAllCycles:
            for (var indexChunk: number = insertedRun.chunkIndex, chunk: Chunk; chunk = subDocument.chunks[indexChunk]; indexChunk--) {
                for (var run: TextRun; run = chunk.textRuns[indexRun]; indexRun--) {
                    if (run.type == TextRunType.ParagraphRun || run.type == TextRunType.SectionRun)
                        break completeExecuteAllCycles;
                    run.paragraph = newParagraph;
                    run.onCharacterPropertiesChanged();
                    newParagraph.length += run.length;
                    oldParagraph.length -= run.length;
                }
                if (indexChunk > 0)
                    indexRun = subDocument.chunks[indexChunk - 1].textRuns.length - 1;
            }
            
            newParagraph.startLogPosition = subDocument.positionManager.registerPosition(run != undefined ? chunk.startLogPosition.value + run.startOffset + 1 : 0);
            oldParagraph.startLogPosition = subDocument.positionManager.registerPosition(insertPositionAtStartDocument + 1);

            // reset mergedCharProps for runs old paragraph
            indexRun = insertedRun.runIndex + 1;
            completeExecuteAllCycles:
            for (var indexChunk: number = insertedRun.chunkIndex, chunk: Chunk; chunk = subDocument.chunks[indexChunk]; indexChunk++) {
                for (var run: TextRun; run = chunk.textRuns[indexRun]; indexRun++) {
                    run.onCharacterPropertiesChanged();
                    if (run.type == TextRunType.ParagraphRun || run.type == TextRunType.SectionRun)
                        break completeExecuteAllCycles;
                }
                indexRun = 0;
            }

            this.manipulator.dispatcher.notifyParagraphInserted(subDocument, insertPositionAtStartDocument);
        }

        // Insert section mark (insertPositionAtStartDocument = [0, number symbols in document - 1)) 
        // sectionProperties, paragraphStyle, paragraphMaskedProperties can be null
        public insertSection(subDocument: SubDocument, insertPositionAtStartDocument: number, maskedCharacterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle,
            sectionProperties: SectionProperties, isInsertPropertiesToCurrentSection: boolean,
            paragraphStyle: ParagraphStyle, paragraphMaskedProperties: MaskedParagraphProperties, isInsertPropertiesAndStyleIndexToCurrentParagraph: boolean, numberingListIndex: number, listLevelIndex: number, tabs: TabProperties) {
            if (!!maskedCharacterProperties == false)
                throw new Error("In insertSection need set clearly maskedCharacterProperties");
            if(!subDocument.isMain())
                throw new Error("Section cannot be inserted in a non-main subDocument");
            // get section props from section in position insertPositionAtStartDocument. [new section][old section]

            var oldSectionIndex: number = Utils.normedBinaryIndexOf(subDocument.documentModel.sections, (s: Section) => s.startLogPosition.value - insertPositionAtStartDocument);
            var oldSection: Section = subDocument.documentModel.sections[oldSectionIndex];

            var lengthNewSection: number = insertPositionAtStartDocument - oldSection.startLogPosition.value + 1;

            var insertedRun: { chunkIndex: number; runIndex: number } = this.insertRunInternal(subDocument, insertPositionAtStartDocument, TextRunType.SectionRun,
                subDocument.documentModel.specChars.SectionMark, maskedCharacterProperties, characterStyle);
            var currentChunk: Chunk = subDocument.chunks[insertedRun.chunkIndex];
            var currentRun: TextRun = currentChunk.textRuns[insertedRun.runIndex];

            // [old paragraph][new paragraph]. Section mark mean not only new section, but new paragraph too
            var oldParagraphIndex: number = Utils.normedBinaryIndexOf(subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - insertPositionAtStartDocument);
            var oldParagraph: Paragraph = subDocument.paragraphs[oldParagraphIndex];
            
            var newLengthOldParagraph: number = insertPositionAtStartDocument - oldParagraph.startLogPosition.value + 1;
            var newLengthNewParagraph: number = oldParagraph.length - newLengthOldParagraph + 1;
            var newParagraphStartPosition: Position = subDocument.positionManager.registerPosition(insertPositionAtStartDocument + 1);
            var newParagraph: Paragraph = new Paragraph(subDocument, newParagraphStartPosition, newLengthNewParagraph, null, null);

            oldParagraph.length = newLengthOldParagraph;
            subDocument.paragraphs.splice(oldParagraphIndex + 1, 0, newParagraph);

            this.applyParagraphProperties(newParagraph, oldParagraph, isInsertPropertiesAndStyleIndexToCurrentParagraph, paragraphStyle, paragraphMaskedProperties, numberingListIndex, listLevelIndex, tabs);
            
            var indexRun: number = insertedRun.runIndex + 1;
            completeExecuteAllCycles:
            for (var indexChunk: number = insertedRun.chunkIndex, chunk: Chunk; chunk = subDocument.chunks[indexChunk]; indexChunk++) {
                for (var run: TextRun; run = chunk.textRuns[indexRun]; indexRun++) {
                    run.paragraph = newParagraph;
                    run.onCharacterPropertiesChanged();
                    if (run.type == TextRunType.ParagraphRun || run.type == TextRunType.SectionRun)
                        break completeExecuteAllCycles;
                }
                indexRun = 0;
            }

            indexRun = insertedRun.runIndex - 1; // "-1" because we don't touch inserted "\n"
            completeExecuteAllCycles:
            for (var indexChunk: number = insertedRun.chunkIndex, chunk: Chunk; chunk = subDocument.chunks[indexChunk]; indexChunk--) {
                for (var run: TextRun; run = chunk.textRuns[indexRun]; indexRun--) {
                    if (run.type == TextRunType.ParagraphRun || run.type == TextRunType.SectionRun)
                        break completeExecuteAllCycles;
                    run.onCharacterPropertiesChanged();
                }
                if (indexChunk > 0)
                    indexRun = subDocument.chunks[indexChunk - 1].textRuns.length - 1;
            }

            var offsetAtStartChunk: number = insertPositionAtStartDocument - currentChunk.startLogPosition.value;
            
            subDocument.positionManager.unregisterPosition(oldSection.startLogPosition);

            var newSectionProperties: SectionProperties;
            if (sectionProperties) {
                if (isInsertPropertiesToCurrentSection)
                    newSectionProperties = sectionProperties;
                else {
                    newSectionProperties = oldSection.sectionProperties;
                    oldSection.sectionProperties = sectionProperties;
                }
            }
            else
                newSectionProperties = oldSection.sectionProperties;

            var newSection: Section = new Section(subDocument.documentModel, subDocument.positionManager.registerPosition(oldSection.startLogPosition.value), lengthNewSection,
                newSectionProperties);

            newSection.headers = oldSection.headers;
            newSection.footers = oldSection.footers;
            oldSection.headers = new SectionHeaders(oldSection);
            oldSection.footers = new SectionFooters(oldSection);

            oldSection.startLogPosition = subDocument.positionManager.registerPosition(insertPositionAtStartDocument + 1);
            oldSection.setLength(subDocument, oldSection.getLength() - lengthNewSection - 1 + 1); // second "+1" because in "insertRun" we increase section.length

            subDocument.documentModel.sections.splice(oldSectionIndex, 0, newSection);

            this.manipulator.dispatcher.notifySectionInserted(subDocument, insertPositionAtStartDocument);
        }

        private applyParagraphProperties(newParagraph: Paragraph, oldParagraph: Paragraph, copyPropertiesToOldParagraph: boolean,
            paragraphStyle: ParagraphStyle, paragraphMaskedProperties: MaskedParagraphProperties, numberingListIndex: number, listLevelIndex: number, tabs: TabProperties) {

            if(paragraphMaskedProperties) {
                if(copyPropertiesToOldParagraph) {
                    oldParagraph.onParagraphPropertiesChanged();
                    newParagraph.setParagraphProperties(oldParagraph.maskedParagraphProperties);
                    oldParagraph.setParagraphProperties(paragraphMaskedProperties);
                }
                else {
                    newParagraph.setParagraphProperties(paragraphMaskedProperties);
                }
            }
            else
                newParagraph.setParagraphProperties(oldParagraph.maskedParagraphProperties);

            if(paragraphStyle) {
                if(copyPropertiesToOldParagraph) {
                    newParagraph.paragraphStyle = oldParagraph.paragraphStyle;
                    oldParagraph.paragraphStyle = paragraphStyle;
                }
                else {
                    newParagraph.paragraphStyle = paragraphStyle;
                }
            }
            else
                newParagraph.paragraphStyle = oldParagraph.paragraphStyle;

            if(numberingListIndex !== undefined) {
                if(copyPropertiesToOldParagraph) {
                    newParagraph.numberingListIndex = oldParagraph.numberingListIndex;
                    newParagraph.listLevelIndex = oldParagraph.listLevelIndex;
                    oldParagraph.numberingListIndex = numberingListIndex;
                    oldParagraph.listLevelIndex = listLevelIndex;
                }
                else {
                    newParagraph.numberingListIndex = numberingListIndex;
                    newParagraph.listLevelIndex = listLevelIndex;
                }
            }
            else {
                newParagraph.numberingListIndex = oldParagraph.numberingListIndex;
                newParagraph.listLevelIndex = oldParagraph.listLevelIndex;
            }

            if(tabs) {
                if(copyPropertiesToOldParagraph) {
                    newParagraph.tabs = oldParagraph.tabs.clone();
                    oldParagraph.tabs = tabs;
                }
                else {
                    newParagraph.tabs = tabs;
                }
            }
            else
                newParagraph.tabs = oldParagraph.tabs.clone();
        }

        public updateSymbol(subDocument: SubDocument, position: number, symbol: string): string {
            if(symbol.length !== 1)
                throw new Error("symbol length should be equal to 1");
            var state = new HistoryItemIntervalState<HistoryItemTextBufferStateObject>();
            var chunkIndex = Utils.normedBinaryIndexOf(subDocument.chunks, c => c.startLogPosition.value - position);
            var chunk = subDocument.chunks[chunkIndex];
            var chunkRelativePosition = position - chunk.startLogPosition.value;

            var oldSymbol = chunk.textBuffer.substr(chunkRelativePosition, 1);
            state.register(new HistoryItemTextBufferStateObject(position, symbol));
            chunk.textBuffer = chunk.textBuffer.substr(0, chunkRelativePosition) + symbol + chunk.textBuffer.substr(chunkRelativePosition + 1);
            this.manipulator.dispatcher.notifyTextBufferChanged(state, subDocument);
            return oldSymbol;
        }

        restoreRemovedInterval(subDocument: SubDocument, removeOperationResult: RemoveIntervalOperationResult) {
            new RestoreRemovedIntervalOperation(this.manipulator, subDocument).execute(removeOperationResult);
        }

        public unpackHistoryRunsToModel(subDocument: SubDocument, historyRuns: HistoryRun[]) {
            var fields: Field[] = subDocument.fields;
            var fieldStackHistory: HistoryRunFieldCodeStart[] = [];

            for (var historyRunIndex: number = 0, historyRun: HistoryRun; historyRun = historyRuns[historyRunIndex]; historyRunIndex++) { 
                switch (historyRun.type) {
                    case TextRunType.ParagraphRun:
                        if (!(historyRun instanceof HistoryRunParagraph))
                            throw new Error("In unpackHistoryRunsToModel type text run = TextRunType.ParagraphRun, but type historyRun != HistoryRunParagraph. historyRun.offsetAtStartDocument = " +
                                historyRun.offsetAtStartDocument + ", historyRun.text = " + historyRun.text);
                        var historyRunParagraph: HistoryRunParagraph = <HistoryRunParagraph>historyRun;
                        this.insertParagraph(subDocument, historyRunParagraph.offsetAtStartDocument, historyRunParagraph.characterProperties, historyRunParagraph.characterStyle,
                            historyRunParagraph.paragraphMaskedProperties, historyRunParagraph.paragraphStyle, historyRunParagraph.isInsertPropertiesAndStyleIndexToCurrentParagraph, historyRunParagraph.numbericListIndex, historyRunParagraph.listLevelIndex, historyRunParagraph.tabs.clone());
                        break;
                    case TextRunType.SectionRun:
                        if (!(historyRun instanceof HistoryRunSection))
                            throw new Error("In unpackHistoryRunsToModel type text run = TextRunType.SectionRun, but type historyRun != HistoryRunSection. historyRun.offsetAtStartDocument = " +
                                historyRun.offsetAtStartDocument + ", historyRun.text = " + historyRun.text);
                        var historyRunSection: HistoryRunSection = <HistoryRunSection>historyRun;
                        this.insertSection(subDocument, historyRunSection.offsetAtStartDocument, historyRunSection.characterProperties, historyRunSection.characterStyle,
                            historyRunSection.sectionProperties, true, historyRunSection.paragraphStyle, historyRunSection.paragraphMaskedProperties,
                            historyRunSection.isInsertPropertiesAndStyleIndexToCurrentParagraph, historyRunSection.numbericListIndex, historyRunSection.listLevelIndex, historyRunSection.tabs.clone());
                        break;
                    case TextRunType.InlinePictureRun:
                        if(!(historyRun instanceof HistoryRunInlinePicture))
                            throw new Error("In unpackHistoryRunsToModel type text run = TextRunType.InlinePictureRun, but type historyRun != HistoryRunInlinePicture. historyRun.offsetAtStartDocument = " +
                                historyRun.offsetAtStartDocument + ", historyRun.text = " + historyRun.text);
                        this.insertInlinePicture(subDocument, historyRun.offsetAtStartDocument, (<HistoryRunInlinePicture>historyRun).id, (<HistoryRunInlinePicture>historyRun).originalWidth,
                            (<HistoryRunInlinePicture>historyRun).originalHeight, (<HistoryRunInlinePicture>historyRun).scaleX, (<HistoryRunInlinePicture>historyRun).scaleY,
                            (<HistoryRunInlinePicture>historyRun).lockAspectRatio, historyRun.characterProperties, historyRun.characterStyle);
                        break;
                    case TextRunType.FieldCodeStartRun:
                        if (!(historyRun instanceof HistoryRunFieldCodeStart))
                            throw new Error("In unpackHistoryRunsToModel type text run = TextRunType.HistoryRunFieldCodeStart, but type historyRun != HistoryRunFieldCodeStart. historyRun.offsetAtStartDocument = " +
                                historyRun.offsetAtStartDocument + ", historyRun.text = " + historyRun.text);
                        fieldStackHistory.push(<HistoryRunFieldCodeStart>historyRun);
                        this.insertText(subDocument, historyRun.offsetAtStartDocument, historyRun.text, historyRun.characterProperties, historyRun.characterStyle, historyRun.type);
                        break;
                    case TextRunType.FieldResultEndRun:
                        this.insertText(subDocument, historyRun.offsetAtStartDocument, historyRun.text, historyRun.characterProperties, historyRun.characterStyle, historyRun.type);

                        var histFieldCodeStartRun: HistoryRunFieldCodeStart = fieldStackHistory.pop();
                        
                        var fieldInsertIndex: number = 0;
                        if (fields.length > 0) {
                            fieldInsertIndex = Math.max(0, Field.normedBinaryIndexOf(fields, histFieldCodeStartRun.startPosition + 1));
                            if (histFieldCodeStartRun.startPosition > fields[fieldInsertIndex].getFieldStartPosition())
                                fieldInsertIndex++;
                        }
                        
                        var newField: Field = new Field(subDocument.positionManager, fieldInsertIndex, histFieldCodeStartRun.startPosition, histFieldCodeStartRun.separatorPosition,
                            histFieldCodeStartRun.endPosition, histFieldCodeStartRun.showCode, histFieldCodeStartRun.hyperlinkInfo ? histFieldCodeStartRun.hyperlinkInfo.clone() : undefined);

                        Field.addField(fields, newField);

                        this.manipulator.dispatcher.notifyFieldInserted(subDocument, histFieldCodeStartRun.startPosition, histFieldCodeStartRun.separatorPosition, histFieldCodeStartRun.endPosition);
                        if (histFieldCodeStartRun.hyperlinkInfo)
                            this.manipulator.dispatcher.notifyHyperlinkInfoChanged(subDocument, FixedInterval.fromPositions(histFieldCodeStartRun.separatorPosition + 1, histFieldCodeStartRun.endPosition - 1),
                                histFieldCodeStartRun.hyperlinkInfo);
                        break;
                    default:
                        this.insertText(subDocument, historyRun.offsetAtStartDocument, historyRun.text, historyRun.characterProperties, historyRun.characterStyle, historyRun.type);
                        break;
                }
            }
        }

        // from and to can be equal
        public insertSubDocumentInOtherSubDocument(targetSubDocument: SubDocument, targetPosition: number, sourceSubDocument: SubDocument, sourceInterval: FixedInterval) {
            var fromDocumentModel: DocumentModel = sourceSubDocument.documentModel,
                toDocumentModel: DocumentModel = targetSubDocument.documentModel,
                constRunIterator: RunIterator = sourceSubDocument.getConstRunIterator(sourceInterval),
                toCurrentPosition: number = targetPosition,
                fromFieldIndexesWhatNeedCopyInfo: number[] = [],
                fromFields: Field[] = sourceSubDocument.fields,
                toFields: Field[] = targetSubDocument.fields;

            while (constRunIterator.moveNext()) {
                var currentRun: TextRun = constRunIterator.currentRun;
                switch (currentRun.type) {
                    case TextRunType.FieldCodeStartRun:
                        var fromGlobPos: number = constRunIterator.currentChunk.startLogPosition.value + currentRun.startOffset;
                        fromFieldIndexesWhatNeedCopyInfo.push(Field.normedBinaryIndexOf(sourceSubDocument.fields, fromGlobPos + 1));
                    case TextRunType.FieldCodeEndRun:
                    case TextRunType.FieldResultEndRun:
                    case TextRunType.TextRun:
                        var insertedText: string = constRunIterator.currentChunk.getRunText(currentRun);
                        var insertedMaskedCharacterProperties: MaskedCharacterProperties = toDocumentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(currentRun.maskedCharacterProperties);
                        var insertedCharacterStyle: CharacterStyle = toDocumentModel.stylesManager.addCharacterStyle(currentRun.characterStyle);
                        this.insertText(targetSubDocument, toCurrentPosition, insertedText, insertedMaskedCharacterProperties, insertedCharacterStyle, currentRun.type);
                        break;
                    case TextRunType.InlinePictureRun: 
                        var currentPictureRun: InlinePictureRun = <InlinePictureRun>currentRun;
                        if (!(currentPictureRun instanceof InlinePictureRun))
                            throw new Error("In TexManipulator.insertPartSubDocumentInOtherSubDocument currentPictureRun not have type InlinePictureRun");
                        this.insertInlinePicture(targetSubDocument, toCurrentPosition,
                            currentPictureRun.id, currentPictureRun.originalWidth, currentPictureRun.originalHeight,
                            currentPictureRun.scaleX, currentPictureRun.scaleY, currentPictureRun.lockAspectRatio,
                            toDocumentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(currentPictureRun.maskedCharacterProperties),
                            toDocumentModel.stylesManager.addCharacterStyle(currentPictureRun.characterStyle));
                        break;
                    case TextRunType.ParagraphRun:
                        var toNumberingListIndex = -1;
                        var toListLevelIndex = currentRun.paragraph.listLevelIndex;
                        if(currentRun.paragraph.numberingListIndex >= 0) {
                            var fromNumberingList = fromDocumentModel.numberingLists[currentRun.paragraph.numberingListIndex];
                            toNumberingListIndex = toDocumentModel.getNumberingListIndexById(fromNumberingList.getId());
                            if(toNumberingListIndex < 0) {
                                var toAbstractNumberingListIndex = toDocumentModel.getAbstractNumberingListIndexById(fromDocumentModel.abstractNumberingLists[fromNumberingList.abstractNumberingListIndex].getId());
                                if(toAbstractNumberingListIndex < 0) {
                                    var toAbstractNumberingList = new AbstractNumberingList(toDocumentModel);
                                    toAbstractNumberingList.copyFrom(fromDocumentModel.abstractNumberingLists[fromNumberingList.abstractNumberingListIndex]);
                                    toAbstractNumberingListIndex = toDocumentModel.abstractNumberingLists.push(toAbstractNumberingList) - 1;
                                }
                                var toNumberingList = new NumberingList(toDocumentModel, toAbstractNumberingListIndex);
                                toNumberingList.copyFrom(fromNumberingList);
                                toNumberingListIndex = toDocumentModel.numberingLists.push(toNumberingList) - 1;
                            }
                        }
                        if(toNumberingListIndex < 0) {
                            var toParagraph = targetSubDocument.getParagraphByPosition(toCurrentPosition);
                            var toParagraphNumberingListIndex = toParagraph.getNumberingListIndex();
                            if(toParagraphNumberingListIndex >= 0) {
                                if(targetPosition === toParagraph.startLogPosition.value) {
                                    toNumberingListIndex = toParagraphNumberingListIndex;
                                    toListLevelIndex = toParagraph.getListLevelIndex();
                                }
                            }
                        }
                        this.insertParagraph(targetSubDocument, toCurrentPosition,
                            toDocumentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(currentRun.maskedCharacterProperties),
                            toDocumentModel.stylesManager.addCharacterStyle(currentRun.characterStyle),
                            toDocumentModel.cache.maskedParagraphPropertiesCache.addItemIfNonExists(currentRun.paragraph.maskedParagraphProperties),
                            currentRun.paragraph.paragraphStyle,
                            true, toNumberingListIndex, toListLevelIndex, currentRun.paragraph.tabs.clone());
                        break;
                    case TextRunType.SectionRun:
                        this.insertSection(targetSubDocument, toCurrentPosition,
                            toDocumentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(currentRun.maskedCharacterProperties),
                            toDocumentModel.stylesManager.addCharacterStyle(currentRun.characterStyle),
                            constRunIterator.currentSection.sectionProperties.clone(),
                            true,
                            currentRun.paragraph.paragraphStyle,
                            toDocumentModel.cache.maskedParagraphPropertiesCache.addItemIfNonExists(currentRun.paragraph.maskedParagraphProperties),
                            true,
                            currentRun.paragraph.numberingListIndex, currentRun.paragraph.listLevelIndex, currentRun.paragraph.tabs.clone());
                        break;
                    default: throw new Error("In TextManipulator.insertPartSubDocumentInOtherSubDocument need insert some inknown paragraphRunType = " + currentRun.type);
                }
                toCurrentPosition += currentRun.length;
            }

            for(let i = 0, table: Table; table = sourceSubDocument.tables[i]; i++) {
                this.manipulator.tables.pasteTable(targetSubDocument, table, targetPosition + table.getStartPosition());
            }

            if (fromFieldIndexesWhatNeedCopyInfo.length > 0) {
                var modelsConstOffset: number = targetPosition - sourceInterval.start;
                var toStartCodePosFirstField: number = modelsConstOffset + fromFields[fromFieldIndexesWhatNeedCopyInfo[0]].getCodeStartPosition();
                var toFieldIndex: number = Field.normedBinaryIndexOf(toFields, toStartCodePosFirstField);
                if (toFieldIndex < 0 || toFields[toFieldIndex].getCodeStartPosition() < toStartCodePosFirstField)
                    toFieldIndex++;
                while (fromFieldIndexesWhatNeedCopyInfo.length > 0) {
                    var fromField: Field = sourceSubDocument.fields[fromFieldIndexesWhatNeedCopyInfo.shift()];
                    var newField: Field = new Field(targetSubDocument.positionManager, toFieldIndex, fromField.getFieldStartPosition() + modelsConstOffset,
                        fromField.getSeparatorPosition() + modelsConstOffset, fromField.getFieldEndPosition() + modelsConstOffset, fromField.showCode,
                        fromField.isHyperlinkField() ? fromField.getHyperlinkInfo().clone() : undefined);

                    Field.addField(toFields, newField);
                    toFieldIndex++;
                    this.manipulator.dispatcher.notifyFieldInserted(targetSubDocument, newField.getFieldStartPosition(), newField.getSeparatorPosition(), newField.getFieldEndPosition());
                    if (newField.isHyperlinkField())
                        this.manipulator.dispatcher.notifyHyperlinkInfoChanged(targetSubDocument, newField.getResultInterval(), newField.getHyperlinkInfo());
                }
            }
        }

        // ******************************************************************************************************************************************************************************
        //                                                                           PRIVATE FUNCS
        // ******************************************************************************************************************************************************************************
        //after this function we have INCORRECT insertedRun.paragraph.length - it OK
        private insertRunInternal(subDocument: SubDocument, insertPositionAtStartDocument: number, type: TextRunType, text: string, characterProperties: MaskedCharacterProperties,
                characterStyle: CharacterStyle): { chunkIndex: number; runIndex: number } {
            var lastCharacterIndex: number = subDocument.getDocumentEndPosition();
            if (insertPositionAtStartDocument >= lastCharacterIndex)
                insertPositionAtStartDocument --; // insert this run before last paragraph mark

            var chunkIndex: number = Utils.normedBinaryIndexOf(subDocument.chunks, (c: Chunk) => c.startLogPosition.value - insertPositionAtStartDocument);
            var chunk: Chunk = subDocument.chunks[chunkIndex];

            var startOffsetAtChunk: number = insertPositionAtStartDocument - chunk.startLogPosition.value;

            var currentRunIndex: number = Utils.normedBinaryIndexOf(chunk.textRuns, (r: TextRun) => r.startOffset - startOffsetAtChunk);
            var currentRun: TextRun = chunk.textRuns[currentRunIndex];

            var sectionIndex: number = Utils.normedBinaryIndexOf(subDocument.documentModel.sections,(s: Section) => s.startLogPosition.value - insertPositionAtStartDocument);
            subDocument.documentModel.sections[sectionIndex].setLength(subDocument, subDocument.documentModel.sections[sectionIndex].getLength() + text.length);

            chunk.textBuffer = [chunk.textBuffer.substr(0, startOffsetAtChunk), text, chunk.textBuffer.substr(startOffsetAtChunk)].join('');
            if (startOffsetAtChunk != currentRun.startOffset) {  // need split
                if (type == TextRunType.TextRun && currentRun.type == TextRunType.TextRun && currentRun.characterStyle.equalsByName(characterStyle) &&
                        currentRun.maskedCharacterProperties.equals(characterProperties)) {
                    // not need create new run. simple insert
                    currentRun.length += text.length;
                    TextManipulator.moveRunsInChunk(chunk, currentRunIndex + 1, text.length);
                    subDocument.positionManager.advance(insertPositionAtStartDocument + 1, text.length);
                    return {chunkIndex: chunkIndex, runIndex: currentRunIndex};
                }
                chunk.splitRun(currentRunIndex, startOffsetAtChunk - currentRun.startOffset);
                currentRunIndex++;
                currentRun = chunk.textRuns[currentRunIndex];
            }
            subDocument.positionManager.advance(insertPositionAtStartDocument + 1, text.length);

            var newTextRun: TextRun;
            var prevRun: TextRun = chunk.textRuns[currentRunIndex - 1];
            if (prevRun && type == TextRunType.TextRun && prevRun.type == TextRunType.TextRun && prevRun.characterStyle.equalsByName(characterStyle) &&
                    prevRun.maskedCharacterProperties.equals(characterProperties)) {
                prevRun.length += text.length;// combine with prev run
                TextManipulator.moveRunsInChunk(chunk, currentRunIndex, text.length);
                return {chunkIndex: chunkIndex, runIndex: currentRunIndex - 1};
            }
            else {
                if (type == TextRunType.TextRun && currentRun.type == TextRunType.TextRun && currentRun.characterStyle.equalsByName(characterStyle) &&
                        currentRun.maskedCharacterProperties.equals(characterProperties)) {
                    currentRun.length += text.length;// combine with next run
                    TextManipulator.moveRunsInChunk(chunk, currentRunIndex + 1, text.length);
                    return {chunkIndex: chunkIndex, runIndex: currentRunIndex};
                }
                else {
                    newTextRun = TextRun.create(startOffsetAtChunk, text.length, type, currentRun.paragraph, characterStyle, characterProperties);
                    chunk.textRuns.splice(currentRunIndex, 0, newTextRun);

                    TextManipulator.moveRunsInChunk(chunk, currentRunIndex + 1, text.length);
                    return {chunkIndex: chunkIndex, runIndex: currentRunIndex};
                }
            }
        }

        public static moveRunsInChunk(chunk: Chunk, startRunIndex: number, offset: number) {
            for (var i: number = startRunIndex, run: TextRun; run = chunk.textRuns[i]; i++)
                run.startOffset += offset;
        }

        private insertParagraphInEnd(subDocument: SubDocument, insertPositionAtStartDocument: number, characterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle): boolean {
            var lastChunk: Chunk = subDocument.chunks[subDocument.chunks.length - 1];
            var offsetAtStartDocumentLastSymbol = lastChunk.startLogPosition.value + lastChunk.textBuffer.length;
            if (insertPositionAtStartDocument >= offsetAtStartDocumentLastSymbol) { // insert after last symbol
                insertPositionAtStartDocument = offsetAtStartDocumentLastSymbol;

                var prevParagraph: Paragraph = subDocument.paragraphs[subDocument.paragraphs.length - 1],
                    newParagraph: Paragraph = new Paragraph(subDocument, subDocument.positionManager.registerPosition(insertPositionAtStartDocument), 1, // 1 - length (paragraph)
                                                prevParagraph.paragraphStyle, prevParagraph.maskedParagraphProperties),
                    newTextRun: TextRun = TextRun.create(insertPositionAtStartDocument - lastChunk.startLogPosition.value, 1, TextRunType.ParagraphRun, newParagraph, characterStyle,
                        characterProperties); // 0 - offset. 1 - length

                subDocument.paragraphs.push(newParagraph);
                lastChunk.textRuns.push(newTextRun);
                lastChunk.textBuffer = lastChunk.textBuffer + Utils.specialCharacters.ParagraphMark;
                var lastSection = subDocument.documentModel.sections[subDocument.documentModel.sections.length - 1];
                lastSection.setLength(subDocument, lastSection.getLength() + 1);
                return true;
            }
            return false;
        }
    }
} 