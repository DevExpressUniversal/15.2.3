module __aspxRichEdit {
    export class ModelManipulator {
        dispatcher: ModelChangesDispatcher;
        model: DocumentModel;
        characterPropertiesManipulator: CharacterPropertiesManipulator;
        paragraphPropertiesManipulator: ParagraphPropertiesManipulator;
        listLevelPropertiesManipulator: ListLevelPropertiesManipulator;
        listLevelCharacterPropertiesManipulator: ListLevelCharacterPropertiesManipulator;
        listLevelParagraphPropertiesManipulator: ListLevelParagraphPropertiesManipulator;
        sectionProperties: SectionPropertiesManipulator;
        text: TextManipulator;
        inlineObjectManipulator: InlineObjectManipulator;
        numberingListManipulator: NumberingListManipulator;
        fieldsManipulator: FieldsManipulator;
        textCaseManipulator: TextCaseManipulator;
        styles: StylesManipulator;
        documentPropertiesManipulator: DocumentPropertiesManipulator;
        headerManipulator: HeaderManipulator;
        footerManipulator: FooterManipulator;
        tabManipulator: TabsManipulator;
        bookmarksManipulator: BookmarksManipulator;
        tables: TablesManipulator;

        lastTextInsertDate: Date;

        constructor(model: DocumentModel) {
            this.dispatcher = new ModelChangesDispatcher();
            this.model = model;
            this.characterPropertiesManipulator = new CharacterPropertiesManipulator(this);
            this.paragraphPropertiesManipulator = new ParagraphPropertiesManipulator(this);
            this.listLevelPropertiesManipulator = new ListLevelPropertiesManipulator(this);
            this.listLevelCharacterPropertiesManipulator = new ListLevelCharacterPropertiesManipulator(this);
            this.listLevelParagraphPropertiesManipulator = new ListLevelParagraphPropertiesManipulator(this);
            this.numberingListManipulator = new NumberingListManipulator(this);
            this.sectionProperties = new SectionPropertiesManipulator(this);
            this.text = new TextManipulator(this);
            this.inlineObjectManipulator = new InlineObjectManipulator(this);
            this.styles = new StylesManipulator(this);
            this.documentPropertiesManipulator = new DocumentPropertiesManipulator(this);
            this.fieldsManipulator = new FieldsManipulator(this);
            this.textCaseManipulator = new TextCaseManipulator(this);
            this.headerManipulator = new HeaderManipulator(this);
            this.footerManipulator = new FooterManipulator(this);
            this.tabManipulator = new TabsManipulator(this);
            this.bookmarksManipulator = new BookmarksManipulator(this);
            this.tables = new TablesManipulator(this);
            this.lastTextInsertDate = new Date(0);
        }

        // ADD TO HISTORY
        public static addToHistoryRemoveBookmark(control: IRichEditControl, interval: FixedInterval) {
            var start = interval.start,
                end = interval.end();
            //var index = Utils.binaryIndexOf(bookmarks, function (b: Bookmark) {
            //    var bStart = b.start.value, bEnd = b.end.value;
            //    if (bStart < start)
            //        return -1;
            //    else if (bEnd > end)
            //        return -1;
            //    return 0;
            //});

            //if (index > -1) //TODO

            var names = [];
            for (var i = 0, b: Bookmark; b = control.model.activeSubDocument.bookmarks[i]; i++) {
                var bStart = b.start.value, bEnd = b.end.value;
                if (bStart >= start && bStart <= end && bEnd >= start && bEnd <= end)
                    names.push(b.name);
            }
            for (var j = 0, n: string; n = names[j]; j++)
                control.history.addAndRedo(new DeleteBookmarkHistoryItem(control.modelManipulator, control.model.activeSubDocument, n));
        }

        public static addToHistorySelectionHistoryItem(control: IRichEditControl, interval: FixedInterval, upd: UpdateInputPositionProperties, endOfLine: boolean) {
            control.history.addAndRedo(new SetSelectionHistoryItem(control.modelManipulator, control.model.activeSubDocument, [interval], control.selection, upd, endOfLine));
        }

        // INSERT SOMETHING

        // if last inserted text was < 2.7 secs back and insert text last elem in history then we modificate last history items
        private isAddedToPrevInsertedTextHistoryItem(control: IRichEditControl, interval: FixedInterval, text: string,
            canAddToPreviousText: boolean, currDate: Date, characterStyle: CharacterStyle, maskedCharacterProperties: MaskedCharacterProperties): boolean {
            if (!canAddToPreviousText || interval.length != 0 || currDate.getTime() - this.lastTextInsertDate.getTime() > 2.7 * 1000) // 2.7 secs
                return false;
            var insertTextHistoryInfo = this.getPreviousInsertTextHistoryItems(control);
            if(!insertTextHistoryInfo)
                return false;
            var insertTextHistoryItem: InsertTextHistoryItem = insertTextHistoryInfo.insertTextHistoryItem;
            var setSelectionHistoryItem: SetSelectionHistoryItem = insertTextHistoryInfo.setSelectionHistoryItem;

            if(setSelectionHistoryItem.intervals.length > 1 || interval.start != setSelectionHistoryItem.intervals[0].start ||
                setSelectionHistoryItem.intervals[0].length != 0 ||
                !insertTextHistoryItem.characterStyle.equalsByName(characterStyle) ||
                !insertTextHistoryItem.maskedCharacterProperties.equals(maskedCharacterProperties))
                return false;

            this.text.insertText(insertTextHistoryItem.boundSubDocument, interval.start, text,
                insertTextHistoryItem.maskedCharacterProperties, insertTextHistoryItem.characterStyle, TextRunType.TextRun);

            setSelectionHistoryItem.intervals[0].start += text.length;
            insertTextHistoryItem.text += text;
            var newPositionSelection: number = setSelectionHistoryItem.intervals[0].start;
            control.selection.setSelection(newPositionSelection, newPositionSelection, control.selection.endOfLine, control.selection.keepX, UpdateInputPositionProperties.No);
            return true;
        }

        private getPreviousInsertTextHistoryItems(control: IRichEditControl): { insertTextHistoryItem: InsertTextHistoryItem; setSelectionHistoryItem: SetSelectionHistoryItem } {
            var history: History = (<History>control.history);
            if(!history)
                return null;
            var historyItems: HistoryItem[] = history.historyItems;
            if(history.currentIndex != historyItems.length - 1)
                return null;
            var lastHistoryItem: HistoryItem = historyItems[historyItems.length - 1];
            if(!(lastHistoryItem instanceof CompositionHistoryItem))
                return null;
            var compositeHistoryItem: CompositionHistoryItem = <CompositionHistoryItem>lastHistoryItem;
            var compositeHistoryItems: HistoryItem[] = compositeHistoryItem.historyItems;
            var possibleSetSelectionHistoryItem: HistoryItem = compositeHistoryItems[compositeHistoryItems.length - 1];
            var possibleInsertTextHistoryItem: HistoryItem = compositeHistoryItems[compositeHistoryItems.length - 2];
            if(!(possibleInsertTextHistoryItem instanceof InsertTextHistoryItem) ||
                !(possibleSetSelectionHistoryItem instanceof SetSelectionHistoryItem))
                return null;
            return {
                insertTextHistoryItem: <InsertTextHistoryItem>possibleInsertTextHistoryItem,
                setSelectionHistoryItem: <SetSelectionHistoryItem>possibleSetSelectionHistoryItem
            };
        }

        public insertText(control: IRichEditControl, interval: FixedInterval, text: string, canAddToPreviousText: boolean) {
            if (text.length == 0)
                throw new Error("Inserted run can't have length == 0");
            var subDocument: SubDocument = control.model.activeSubDocument;
            var documentEndPosition: number = subDocument.getDocumentEndPosition();
            if (interval.start >= documentEndPosition)
                throw "ModelManipulator.insertText interval.start >= documentEndPosition";
            if (interval.length > 0) {
                if (interval.end() > documentEndPosition)
                    throw "ModelManipulator.insertText interval.end() > documentEndPosition";
                var run: TextRun = subDocument.getRunByPosition(interval.end() - 1);
                if (run.type == TextRunType.ParagraphRun || run.type == TextRunType.SectionRun)
                    interval.length -= 1;
            }

            var currDate: Date = new Date();
            var characterStyle: CharacterStyle = control.inputPosition.getCharacterStyle();
            var maskedCharacterProperties: MaskedCharacterProperties = control.inputPosition.getMaskedCharacterProperties().clone();

            if (!this.isAddedToPrevInsertedTextHistoryItem(control, interval, text, canAddToPreviousText, currDate, characterStyle, maskedCharacterProperties)) {
                control.history.beginTransaction();
                if (interval.length > 0) {
                    ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start, 0), UpdateInputPositionProperties.No, control.selection.endOfLine);
                    ModelManipulator.removeInterval(control, subDocument, interval, false);
                }

                control.history.addAndRedo(new InsertTextHistoryItem(control.modelManipulator, control.model.activeSubDocument, interval.start, text,
                    maskedCharacterProperties, characterStyle));
                ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start + text.length, 0), UpdateInputPositionProperties.No, control.selection.endOfLine);
                control.history.endTransaction();
            }
            this.lastTextInsertDate = currDate;
        }

        public insertLayoutDependentText(control: IRichEditControl, interval: FixedInterval) {
            var subDocument: SubDocument = control.model.activeSubDocument;
            var documentEndPosition: number = subDocument.getDocumentEndPosition();
            if(interval.start >= documentEndPosition)
                throw "ModelManipulator.insertLayoutDependentText interval.start >= documentEndPosition";
            if(interval.end() > documentEndPosition)
                throw "ModelManipulator.insertLayoutDependentText interval.end() > documentEndPosition";
            if(interval.end() == documentEndPosition)
                interval.length -= 1;

            var characterStyle: CharacterStyle = control.inputPosition.getCharacterStyle();
            var maskedCharacterProperties: MaskedCharacterProperties = control.inputPosition.getMaskedCharacterProperties().clone();

            control.history.beginTransaction();
            if(interval.length > 0) {
                ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start, 0), UpdateInputPositionProperties.Yes, control.selection.endOfLine);
                ModelManipulator.removeInterval(control, subDocument, interval, false);
            }
            control.history.addAndRedo(new InsertLayoutDependentTextItem(control.modelManipulator, control.model.activeSubDocument, interval.start,
                maskedCharacterProperties, characterStyle));
            ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start + control.model.specChars.LayoutDependentText.length, 0),
                UpdateInputPositionProperties.No, control.selection.endOfLine);
            control.history.endTransaction();
        }

        public static modifyLastInsertedSymbol(control: IRichEditControl, symbol: string) {
            if(symbol.length !== 1)
                throw "Updated symbol length should be equal to 1";
            var insertTextHistoryInfo = control.modelManipulator.getPreviousInsertTextHistoryItems(control);
            if(!insertTextHistoryInfo)
                throw "Last inserted history items were not found";
            insertTextHistoryInfo.insertTextHistoryItem.text = insertTextHistoryInfo.insertTextHistoryItem.text.substr(0, insertTextHistoryInfo.insertTextHistoryItem.text.length - 1) + symbol;
            control.modelManipulator.text.updateSymbol(control.model.activeSubDocument, insertTextHistoryInfo.setSelectionHistoryItem.intervals[0].start - 1, symbol);
        }

        public static insertInlinePicture(control: IRichEditControl, interval: FixedInterval, id: number, originalWidth: number, originalHeight: number, scaleX: number, scaleY: number) {
            var subDocument: SubDocument = control.model.activeSubDocument;
            var documentEndPosition: number = subDocument.getDocumentEndPosition();
            if (interval.start >= documentEndPosition)
                throw "ModelManipulator.insertInlinePicture interval.start >= documentEndPosition";
            if (interval.end() > documentEndPosition)
                throw "ModelManipulator.insertInlinePicture interval.end() > documentEndPosition";
            if (interval.end() == documentEndPosition)
                interval.length -= 1;

            var characterStyle: CharacterStyle = control.inputPosition.getCharacterStyle();

            control.history.beginTransaction();
            if (interval.length > 0) {
                ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start, 0), UpdateInputPositionProperties.Yes, control.selection.endOfLine);
                ModelManipulator.removeInterval(control, subDocument, interval, false);
            }
            control.history.addAndRedo(new InsertInlinePictureHistoryItem(control.modelManipulator, subDocument, interval.start, id, originalWidth, originalHeight, scaleX, scaleY,
                control.inputPosition.getMaskedCharacterProperties().clone(), characterStyle));
            ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start + 1, 0), UpdateInputPositionProperties.Yes, control.selection.endOfLine);
            control.history.endTransaction();
        }

        public static insertParagraph(control: IRichEditControl, subDocument: SubDocument, interval: FixedInterval) {
            var model: DocumentModel = subDocument.documentModel,
                documentEndPosition: number = subDocument.getDocumentEndPosition();
            if (interval.start >= documentEndPosition)
                throw "ModelManipulator.insertParagraph interval.start >= documentEndPosition";
            if (interval.end() > documentEndPosition)
                throw "ModelManipulator.insertParagraph interval.end() > documentEndPosition";
            var characterStyle: CharacterStyle = control.inputPosition.getCharacterStyle();
            var maskedCharacterProperties: MaskedCharacterProperties = control.inputPosition.getMaskedCharacterProperties().clone();
            if (interval.end() == documentEndPosition)
                interval.length -= 1;

            var currentParagraph = subDocument.getRunByPosition(interval.start).paragraph;
            var needToUseCurrentParagraphLastRunAsSource = currentParagraph.isInList() && !interval.containsInterval(currentParagraph.startLogPosition.value, currentParagraph.getEndPosition());
            control.history.beginTransaction();
            if (interval.length > 0) {
                ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start, 0), UpdateInputPositionProperties.No, false);
                ModelManipulator.removeInterval(control, subDocument, interval, false);
            }

            if(needToUseCurrentParagraphLastRunAsSource) {
                if(interval.length > 0)
                    currentParagraph = subDocument.getRunByPosition(interval.start).paragraph;
                var paragraphLastRun = subDocument.getRunByPosition(currentParagraph.getEndPosition() - 1);
                control.history.addAndRedo(new InsertParagraphHistoryItem(control.modelManipulator, subDocument, interval.start,
                    paragraphLastRun.maskedCharacterProperties.clone(), paragraphLastRun.characterStyle, undefined, undefined, undefined, undefined, undefined));
            }
            else {
                control.history.addAndRedo(new InsertParagraphHistoryItem(control.modelManipulator, subDocument, interval.start,
                    maskedCharacterProperties, characterStyle, undefined, undefined, undefined, undefined, undefined));
            }

            ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start + model.specChars.ParagraphMark.length, 0), UpdateInputPositionProperties.No, false);
            control.history.endTransaction();
        }

        public static insertSectionAndSetStartType(control: IRichEditControl, position: number, startType: SectionStartType) {
            var model: DocumentModel = control.model,
                subDocument: SubDocument = model.activeSubDocument,
                documentEndPosition: number = subDocument.getDocumentEndPosition();
            if (position >= documentEndPosition)
                throw "ModelManipulator.insertSectionAndSetStartType position >= documentEndPosition";
            var characterStyle: CharacterStyle = control.inputPosition.getCharacterStyle();
            var maskedCharacterProperties: MaskedCharacterProperties = control.inputPosition.getMaskedCharacterProperties().clone();
            var sectionProperties: SectionProperties = control.model.activeSubDocument.getSectionByPosition(position).sectionProperties.clone();
            sectionProperties.startType = startType;

            // unlike InsertText in word 2010 we not need delete selection interval 
            control.history.beginTransaction();
            control.history.addAndRedo(new InsertSectionHistoryItem(control.modelManipulator, control.model.activeSubDocument, position,
                maskedCharacterProperties, characterStyle, sectionProperties, false, null, null, true, undefined, undefined, undefined));
            ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(position + model.specChars.SectionMark.length, 0), UpdateInputPositionProperties.No, false);
            control.history.endTransaction();
        }

        // only for testing. Need delete this later
        public static insertSection(control: IRichEditControl, position: number) {
            var model: DocumentModel = control.model,
                subDocument: SubDocument = model.activeSubDocument,
                documentEndPosition: number = subDocument.getDocumentEndPosition();
            if (position >= documentEndPosition)
                throw "ModelManipulator.insertSection position >= documentEndPosition";
            var characterStyle: CharacterStyle = control.inputPosition.getCharacterStyle();
            var maskedCharacterProperties: MaskedCharacterProperties = control.inputPosition.getMaskedCharacterProperties().clone();
            var sectionProperties: SectionProperties = control.model.activeSubDocument.getSectionByPosition(position).sectionProperties.clone();

            // unlike InsertText in word 2010 we not need delete selection interval 
            control.history.beginTransaction();
            control.history.addAndRedo(new InsertSectionHistoryItem(control.modelManipulator, control.model.activeSubDocument, position,
                maskedCharacterProperties, characterStyle, sectionProperties, false, null, null, true, undefined, undefined, undefined));
            ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(position + model.specChars.SectionMark.length, 0), UpdateInputPositionProperties.No, false);
            control.history.endTransaction();
        }

        public static removeInterval(control: IRichEditControl, subDocument: SubDocument, interval: FixedInterval, removeTableIfItMatchesWithInterval: boolean) {
            var documentEndPosition: number = subDocument.getDocumentEndPosition();
            if (interval.start >= documentEndPosition)
                throw "ModelManipulator.removeInterval interval.start >= documentEndPosition";
            if (interval.end() > documentEndPosition)
                throw "ModelManipulator.removeInterval interval.end() > documentEndPosition";
            if(interval.length === 0 || (interval.length === 1 && interval.end() === documentEndPosition))
                return;
            control.history.beginTransaction();

            TablesManipulator.removeTablesOnInterval(control, subDocument, interval, removeTableIfItMatchesWithInterval);

            if(interval.end() == documentEndPosition)
                interval.length -= 1;

            var setPropertiesSecondParagraph: boolean = Utils.binaryIndexOf(subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - interval.start) >= 0;
            ModelManipulator.addToHistoryRemoveBookmark(control, interval);
            control.history.addAndRedo(new RemoveIntervalHistoryItem(control.modelManipulator, control.model.activeSubDocument, interval, setPropertiesSecondParagraph));
            control.history.endTransaction();
        }

        public static copyIntervalTo(control: IRichEditControl, subDocument: SubDocument, interval: FixedInterval, toPosition: number) {
            var documentEndPosition: number = subDocument.getDocumentEndPosition();
            if (interval.start >= documentEndPosition)
                throw "ModelManipulator.copyIntervalTo interval.start >= documentEndPosition";
            if (interval.end() > documentEndPosition)
                throw "ModelManipulator.copyIntervalTo interval.end() > documentEndPosition";
            if (toPosition >= documentEndPosition)
                throw "ModelManipulator.copyIntervalTo toPosition >= documentEndPosition";

            control.history.beginTransaction();
            let rangeCopy = this.createRangeCopy(subDocument, [interval]);
            this.insertRangeCopy(control, subDocument, rangeCopy, toPosition);
            control.history.endTransaction();
        }

        static insertSubDocument(control: IRichEditControl, targetSubDocument: SubDocument, targetPosition: number, sourceSubDocument: SubDocument, sourceInterval: FixedInterval): FixedInterval {
            if(Utils.binaryIndexOf(sourceSubDocument.tables, t => t.getStartPosition() - sourceInterval.start) >= 0) {
                let targetParagraphIndex = targetSubDocument.getParagraphIndexByPosition(targetPosition);
                if(targetSubDocument.paragraphs[targetParagraphIndex].startLogPosition.value !== targetPosition) {
                    ModelManipulator.insertParagraph(control, targetSubDocument, new FixedInterval(targetPosition, 0));
                    targetPosition++;
                }
            }
            control.history.addAndRedo(new InsertSubDocumentHistoryItem(control.modelManipulator, targetSubDocument, targetPosition, sourceSubDocument, sourceInterval));
            return new FixedInterval(targetPosition, sourceInterval.length);
        }

        public static moveIntervalTo(control: IRichEditControl, subDocument: SubDocument, interval: FixedInterval, toPosition: number) {
            var documentEndPosition: number = subDocument.getDocumentEndPosition();
            if (interval.start >= documentEndPosition)
                throw "ModelManipulator.moveIntervalTo interval.start >= documentEndPosition";
            if (interval.end() > documentEndPosition)
                throw "ModelManipulator.moveIntervalTo interval.end() > documentEndPosition";
            if (interval.end() == documentEndPosition)
                interval.length -= 1;
            if (toPosition >= documentEndPosition)
                throw "ModelManipulator.moveIntervalTo toPosition >= documentEndPosition";
            control.history.beginTransaction();
            let rangeCopy = this.createRangeCopy(subDocument, [interval]);
            this.insertRangeCopy(control, subDocument, rangeCopy, toPosition);
            ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start, 0), UpdateInputPositionProperties.No, control.selection.endOfLine);
            if(toPosition < interval.start)
                ModelManipulator.removeInterval(control, subDocument, new FixedInterval(interval.start + interval.length, interval.length), false);
            else
                ModelManipulator.removeInterval(control, subDocument, interval, false);
            control.history.endTransaction();
        }

        /* Range Copy */
        static insertRangeCopy(control: IRichEditControl, subDocument: SubDocument, result: RangeCopy, position: number): FixedInterval {
            let sourceInterval = new FixedInterval(0, result.model.activeSubDocument.getDocumentEndPosition() - (result.addedUselessParagraphMarkInEnd ? 1 : 0));
            return this.insertSubDocument(control, subDocument, position, result.model.activeSubDocument, sourceInterval);
        }
        // Interval MUST consider fields fully!!! Use for that Selection.getLastInterval() (here all intervals correct or Field.correctIntervalDueToFields)
        static createRangeCopy(subDocument: SubDocument, intervals: FixedInterval[]): RangeCopy {
            if(intervals.length === 0)
                throw new Error("intervals.length should be > 0");
            if(intervals[0].length === 0)
                throw new Error("intervals[0].length should be > 0");
            return new CreateRangeCopyOperation(subDocument).execute(intervals);
        }

        static pasteRangeCopy(control: IRichEditControl, subDocument: SubDocument, interval: FixedInterval, rangeCopy: RangeCopy) {
            var documentEndPosition: number = subDocument.getDocumentEndPosition();
            if(interval.start >= documentEndPosition)
                throw "ModelManipulator.pasteSelection interval.start >= documentEndPosition";
            if(interval.length > 0) {
                if(interval.end() > documentEndPosition)
                    throw "ModelManipulator.pasteSelection interval.end() > documentEndPosition";
                var run: TextRun = subDocument.getRunByPosition(interval.end() - 1);
                if(run.type == TextRunType.ParagraphRun || run.type == TextRunType.SectionRun)
                    interval.length -= 1;
            }

            var isSubDocumentActive = control.model.activeSubDocument == subDocument;
            control.history.beginTransaction();
            if(interval.length > 0) {
                if(isSubDocumentActive)
                    ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start, 0), UpdateInputPositionProperties.No, control.selection.endOfLine);
                ModelManipulator.removeInterval(control, subDocument, interval, false);
            }
            let newInterval = ModelManipulator.insertRangeCopy(control, subDocument, rangeCopy, interval.start);
            if(isSubDocumentActive)
                ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(newInterval.end(), 0), UpdateInputPositionProperties.No, control.selection.endOfLine);
            control.history.endTransaction();
        }

        static pasteSelection(control: IRichEditControl, interval: FixedInterval, runsInfo: ImportedRunInfo[]) {
            var subDocument: SubDocument = control.model.activeSubDocument;
            var documentEndPosition: number = subDocument.getDocumentEndPosition();
            if(interval.start >= documentEndPosition)
                throw "ModelManipulator.pasteSelection interval.start >= documentEndPosition";
            if(interval.length > 0) {
                if(interval.end() > documentEndPosition)
                    throw "ModelManipulator.pasteSelection interval.end() > documentEndPosition";
                var run: TextRun = subDocument.getRunByPosition(interval.end() - 1);
                if(run.type == TextRunType.ParagraphRun || run.type == TextRunType.SectionRun)
                    interval.length -= 1;
            }

            var options = subDocument.documentModel.options;
            var characterStyle: CharacterStyle = control.inputPosition.getCharacterStyle();
            var defaultMaskedCharacterProperties = subDocument.documentModel.defaultCharacterProperties.clone();
            defaultMaskedCharacterProperties.useValue = 0;
            var inputPositionMaskedCharacterProperties = ControlOptions.isEnabled(options.characterFormatting) ? control.inputPosition.getMaskedCharacterProperties().clone()
                : defaultMaskedCharacterProperties.clone();

            var defaultMaskedParagraphProperties = subDocument.documentModel.defaultParagraphProperties.clone();
            defaultMaskedParagraphProperties.useValue = 0;

            var historyRuns: HistoryRun[] = [];
            var fieldStackHistory: HistoryRunFieldCodeStart[] = [];
            var imagesToLoad: { guid: string; position: number; sourceUrl: string; scaleX: number; scaleY: number }[] = [];
            var currentPosition = interval.start;
            var pastedListsIndices: { [key: number]: number; } = {};
            var pastedTables: { [position: number]: ImportedTableInfo } = {};
            for(var i = 0, runInfo: ImportedRunInfo; runInfo = runsInfo[i]; i++) {
                var runType = runInfo.runType;
                var length = runInfo.runLength;
                switch(runType) {
                    case TextRunType.TextRun:
                        var textRunInfo = <ImportedTextRunInfo>runInfo;
                        var maskedCharacterProperties = ControlOptions.isEnabled(options.characterFormatting) ? textRunInfo.maskedCharacterProperties : defaultMaskedCharacterProperties.clone();
                        var text = textRunInfo.text;
                        if(!ControlOptions.isEnabled(options.tabSymbol))
                            text = text.replace(/\t/gi, " ");
                        if(options.tabMarker !== Utils.specialCharacters.TabMark)
                            text = text.replace(/\t/gi, options.tabMarker);
                        length = text.length;
                        historyRuns.push(new HistoryRun(runType, characterStyle, currentPosition, maskedCharacterProperties, text));
                        break;
                    case TextRunType.InlinePictureRun:
                        if(ControlOptions.isEnabled(options.inlinePictures)) {
                            var inlinePictureRunInfo = <ImportedInlinePictureRunInfo>runInfo;
                            var guid = "";
                            var isNeedLoading = !!inlinePictureRunInfo.sourceUrl;
                            if(isNeedLoading) {
                                guid = ASPx.CreateGuid();
                                imagesToLoad.push({
                                    guid: guid,
                                    position: currentPosition,
                                    sourceUrl: inlinePictureRunInfo.sourceUrl,
                                    scaleX: inlinePictureRunInfo.scaleX,
                                    scaleY: inlinePictureRunInfo.scaleY
                                });
                            }
                            var historyInlinePictureRun = new HistoryRunInlinePicture(characterStyle, currentPosition, inputPositionMaskedCharacterProperties,
                                inlinePictureRunInfo.id, inlinePictureRunInfo.originalWidth, inlinePictureRunInfo.originalHeight, inlinePictureRunInfo.scaleX, inlinePictureRunInfo.scaleY,
                                inlinePictureRunInfo.lockAspectRatio, !isNeedLoading, guid);
                            historyRuns.push(historyInlinePictureRun);
                        }
                        else
                            length = 0;
                        break;
                    case TextRunType.ParagraphRun:
                        if(ControlOptions.isEnabled(options.paragraphs)) {
                            var paragraphRunInfo = <ImportedParagraphRunInfo>runInfo;

                            var tableInfo = paragraphRunInfo.tableInfo;
                            if(!ControlOptions.isEnabled(options.tables))
                                tableInfo = null;
                            if(tableInfo)
                                pastedTables[currentPosition] = tableInfo;

                            var numberingListIndex = -1;
                            var listLevelIndex = -1;
                            var listInfo = paragraphRunInfo.listInfo;
                            if(!ControlOptions.isEnabled(options.numberingBulleted) && !ControlOptions.isEnabled(options.numberingMultiLevel) && !ControlOptions.isEnabled(options.numberingSimple))
                                listInfo = null;
                            if(listInfo) {
                                if(!ControlOptions.isEnabled(options.numberingBulleted) && listInfo.listType === NumberingType.Bullet) {
                                    var isNumberingSimpleEnabled = ControlOptions.isEnabled(options.numberingSimple);
                                    listInfo.listType = isNumberingSimpleEnabled ? NumberingType.Simple : NumberingType.MultiLevel;
                                    listInfo.listFormat = NumberingFormat.Decimal;
                                    listInfo.displayFormatString = "";
                                }
                                if(!ControlOptions.isEnabled(options.numberingSimple) && listInfo.listType === NumberingType.Simple) {
                                    listInfo.listType = ControlOptions.isEnabled(options.numberingBulleted) ? NumberingType.Bullet : NumberingType.MultiLevel;
                                    listInfo.displayFormatString = "";
                                }
                                if(!ControlOptions.isEnabled(options.numberingMultiLevel) && listInfo.listType === NumberingType.MultiLevel) {
                                    listInfo.listType = ControlOptions.isEnabled(options.numberingBulleted) ? NumberingType.Bullet : NumberingType.Simple;
                                    listInfo.displayFormatString = "";
                                }

                                var targetListIndex = pastedListsIndices[listInfo.listIndex];
                                if(targetListIndex === undefined) {
                                    var model = subDocument.documentModel;
                                    var abstractNumberingList = new AbstractNumberingList(model);
                                    var templateIndex = -1;
                                    for(var j = 0, list: AbstractNumberingList; list = model.abstractNumberingListTemplates[j]; j++) {
                                        if(list.getListType() === listInfo.listType)
                                            templateIndex = j;
                                    }
                                    if(templateIndex > -1) {
                                        abstractNumberingList.copyFrom(model.abstractNumberingListTemplates[templateIndex]);
                                        var abstractNumberingListIndex = control.modelManipulator.numberingListManipulator.addAbstractNumberingList(abstractNumberingList);
                                        var numberingList = new NumberingList(model, abstractNumberingListIndex);
                                        targetListIndex = control.modelManipulator.numberingListManipulator.addNumberingList(numberingList);
                                        pastedListsIndices[listInfo.listIndex] = targetListIndex;
                                    }
                                }
                                if(targetListIndex !== undefined) {
                                    numberingListIndex = targetListIndex;
                                    listLevelIndex = listInfo.listLevel;
                                    var listLevel = model.numberingLists[numberingListIndex].levels[listLevelIndex];
                                    control.modelManipulator.listLevelPropertiesManipulator.format.setValue(model, false, targetListIndex, listLevelIndex, listInfo.listFormat);
                                    if(listInfo.displayFormatString) {
                                        control.modelManipulator.listLevelPropertiesManipulator.displayFormatString.setValue(model, false,
                                            targetListIndex, listLevelIndex, listInfo.displayFormatString);
                                    }
                                    if(listInfo.maskedCharacterProperties) {
                                        listLevel.setCharacterProperties(listInfo.maskedCharacterProperties);
                                        listLevel.onCharacterPropertiesChanged();
                                    }
                                }
                            }
                            var maskedParagraphProperties = ControlOptions.isEnabled(options.paragraphFormatting) ? paragraphRunInfo.maskedParagraphProperties : defaultMaskedParagraphProperties.clone();
                            historyRuns.push(new HistoryRunParagraph(runType, characterStyle, currentPosition, inputPositionMaskedCharacterProperties, subDocument.documentModel.specChars.ParagraphMark,
                                undefined, maskedParagraphProperties, true, numberingListIndex, listLevelIndex, new TabProperties())
                            );
                        }
                        else
                            length = 0;
                        break;
                    case TextRunType.FieldCodeStartRun:
                        var hyperlinkInfo = (<ImportedFieldCodeStartRunInfo>runInfo).hyperlinkInfo;
                        if(ControlOptions.isEnabled(options.fields) && (!hyperlinkInfo || ControlOptions.isEnabled(options.hyperlinks))) {
                            var startPosition = currentPosition; //?
                            var historyRun = new HistoryRunFieldCodeStart(runType, characterStyle, currentPosition, inputPositionMaskedCharacterProperties, subDocument.documentModel.specChars.FieldCodeStartRun, false,
                                startPosition, undefined, undefined, hyperlinkInfo);
                            fieldStackHistory.push(historyRun);
                            historyRuns.push(historyRun);
                            if(hyperlinkInfo) {
                                var codeText = [
                                    " HYPERLINK \"",
                                    hyperlinkInfo.uri,
                                    "\" ",
                                    hyperlinkInfo.tip == "" ? "" : "\\o \"" + hyperlinkInfo.tip + "\" ",
                                    hyperlinkInfo.anchor == "" ? "" : "\\l \"" + hyperlinkInfo.anchor + "\" "
                                ].join("");
                                historyRuns.push(new HistoryRun(TextRunType.TextRun, characterStyle, currentPosition + length, inputPositionMaskedCharacterProperties, codeText));
                                length += codeText.length;
                            }
                        }
                        else
                            length = 0;
                        break;
                    case TextRunType.FieldCodeEndRun:
                        if(fieldStackHistory.length) {
                            historyRuns.push(new HistoryRunFieldCodeEnd(runType, characterStyle, currentPosition, inputPositionMaskedCharacterProperties, subDocument.documentModel.specChars.FieldCodeEndRun));
                            fieldStackHistory[fieldStackHistory.length - 1].separatorPosition = currentPosition;
                        }
                        else
                            length = 0;
                        break;
                    case TextRunType.FieldResultEndRun:
                        if(fieldStackHistory.length) {
                            historyRuns.push(new HistoryRunFieldResultEnd(runType, characterStyle, currentPosition, inputPositionMaskedCharacterProperties, subDocument.documentModel.specChars.FieldResultEndRun));
                            var histFieldCodeStartRun: HistoryRunFieldCodeStart = fieldStackHistory.pop();
                            histFieldCodeStartRun.endPosition = currentPosition + length;
                        }
                        else
                            length = 0;
                        break;
                    default:
                        break;
                }
                currentPosition += length;
            }

            control.history.beginTransaction();
            if(interval.length > 0) {
                ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(interval.start, 0), UpdateInputPositionProperties.No, control.selection.endOfLine);
                ModelManipulator.removeInterval(control, subDocument, interval, false);
            }
            var intervalToPasteIn: FixedInterval = new FixedInterval(interval.start, currentPosition - interval.start);

            control.history.addAndRedo(new PasteSelectionHistoryItem(control.modelManipulator, subDocument, intervalToPasteIn, historyRuns, pastedTables,
                inputPositionMaskedCharacterProperties, characterStyle));

            ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(intervalToPasteIn.end(), 0), UpdateInputPositionProperties.No, control.selection.endOfLine);
            if(imagesToLoad.length)
                control.modelManipulator.dispatcher.notifyLoadInlinePictures(subDocument, imagesToLoad);
            control.history.endTransaction();
        }

        public static getNextWordStartPosition(control: IRichEditControl, currentPosition: number): number {
            var runInfo: { chunkIndex: number; runIndex: number; chunk: Chunk; run: TextRun } = control.model.activeSubDocument.getRunAndIndexesByPosition(currentPosition);

            var groupMask: NextPrevWordGroupMask = NextPrevWordGroupMask.NoOne;
            var previousGroupDiffersFromCurrent = function (mask: NextPrevWordGroupMask): boolean {
                groupMask |= mask;
                return (groupMask & ~mask) != NextPrevWordGroupMask.NoOne;
            }

            var findDiffersGroup: boolean = false;
            var chunks: Chunk[] = control.model.activeSubDocument.chunks;
            var currentChunkIndex: number = runInfo.chunkIndex;
            var currentRunIndex: number = runInfo.runIndex;
            var currentPositionInChunk: number = currentPosition - runInfo.chunk.startLogPosition.value;
            var run: TextRun = runInfo.chunk.textRuns[currentRunIndex];
            var leftCharactersUnwatched: number = runInfo.run.startOffset + runInfo.run.length - currentPositionInChunk;
            for (var currentChunk: Chunk; currentChunk = chunks[currentChunkIndex]; currentChunkIndex++) {
                for (var character: string; character = currentChunk.textBuffer[currentPositionInChunk]; currentPositionInChunk++) {
                    if (!leftCharactersUnwatched) {
                        run = currentChunk.textRuns[++currentRunIndex];
                        leftCharactersUnwatched = run.length;
                    }
                    leftCharactersUnwatched--;
                    if (run.getCharacterMergedProperies().hidden)
                        continue;

                    switch (character) {
                        case Utils.specialCharacters.Space: case Utils.specialCharacters.QmSpace:
                        case Utils.specialCharacters.EmSpace: case Utils.specialCharacters.EnSpace: // here symbols what we ignore
                            previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Space); break;
                        case Utils.specialCharacters.TabMark:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Tab); break;
                        case Utils.specialCharacters.ParagraphMark:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Paragraph); break;

                        case Utils.specialCharacters.LeftSingleQuote:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.LeftSingleQuote); break;
                        case Utils.specialCharacters.RightSingleQuote:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.RightSingleQuote); break;
                        case "'":
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.SingleQuote); break;

                        case Utils.specialCharacters.LeftDoubleQuote:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.LeftDoubleQuote); break;
                        case Utils.specialCharacters.RightDoubleQuote:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.RightDoubleQuote); break;
                        case '"':
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.DoubleQuote); break;

                        case "(": case ")": case "«": case "»": case "<": case ">":
                        case "№": case "%": case "!": case ":": case "?": case "-":
                        case "+": case ",": case ".": case "*": case "=": case "\\":
                        case "/": case "|": case ";":
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.PunctuationMark); break;
                        default:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Others); break;
                    }
                    if (findDiffersGroup)
                        return currentChunk.startLogPosition.value + currentPositionInChunk;
                }
                currentPositionInChunk = 0;
                currentRunIndex = 0;
            }
            return control.model.activeSubDocument.getDocumentEndPosition() - 1;
        }

        public static getPrevWordStartPosition(control: IRichEditControl, currentPosition: number): number {
            if (currentPosition == 0)
                return 0;
            currentPosition--;
            var runInfo: { chunkIndex: number; runIndex: number; chunk: Chunk; run: TextRun } = control.model.activeSubDocument.getRunAndIndexesByPosition(currentPosition);

            var groupMask: NextPrevWordGroupMask = NextPrevWordGroupMask.NoOne;
            var previousGroupDiffersFromCurrent = function (mask: NextPrevWordGroupMask): boolean {
                groupMask |= mask;
                return (groupMask & ~mask & ~NextPrevWordGroupMask.Space) != NextPrevWordGroupMask.NoOne;
            }

            var findDiffersGroup: boolean = false;
            var chunks: Chunk[] = control.model.activeSubDocument.chunks;
            var currentChunkIndex: number = runInfo.chunkIndex;
            var currentRunIndex: number = runInfo.runIndex;
            var currentChunk: Chunk = runInfo.chunk;
            var currentPositionInChunk: number = currentPosition - currentChunk.startLogPosition.value;
            var run: TextRun = currentChunk.textRuns[currentRunIndex];
            var leftCharactersUnwatched: number = currentPositionInChunk - run.startOffset;
            while (true) {
                for (var character: string; character = currentChunk.textBuffer[currentPositionInChunk]; currentPositionInChunk--) {
                    if (leftCharactersUnwatched < 0) { // == -1
                        run = currentChunk.textRuns[--currentRunIndex];
                        leftCharactersUnwatched = run.length;
                    }
                    leftCharactersUnwatched--;
                    if (run.getCharacterMergedProperies().hidden)
                        continue;

                    switch (character) {
                        case Utils.specialCharacters.Space: case Utils.specialCharacters.QmSpace:
                        case Utils.specialCharacters.EmSpace: case Utils.specialCharacters.EnSpace: // here symbols what we ignore
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Space); break;
                        case Utils.specialCharacters.TabMark:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Tab); break;
                        case Utils.specialCharacters.ParagraphMark:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Paragraph); break;

                        case Utils.specialCharacters.LeftSingleQuote:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.LeftSingleQuote); break;
                        case Utils.specialCharacters.RightSingleQuote:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.RightSingleQuote); break;
                        case "'":
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.SingleQuote); break;

                        case Utils.specialCharacters.LeftDoubleQuote:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.LeftDoubleQuote); break;
                        case Utils.specialCharacters.RightDoubleQuote:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.RightDoubleQuote); break;
                        case '"':
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.DoubleQuote); break;

                        case "(": case ")": case "«": case "»": case "<": case ">":
                        case "№": case "%": case "!": case ":": case "?": case "-":
                        case "+": case ",": case ".": case "*": case "=": case "\\":
                        case "/": case "|": case ";":
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.PunctuationMark); break;
                        default:
                            findDiffersGroup = previousGroupDiffersFromCurrent(NextPrevWordGroupMask.Others); break;
                    }
                    if (findDiffersGroup)
                        return currentChunk.startLogPosition.value + currentPositionInChunk + 1;
                }
                if (!(currentChunk = chunks[--currentChunkIndex]))
                    break;
                currentPositionInChunk = currentChunk.textBuffer.length - 1;
                currentRunIndex = currentChunk.textRuns.length;
            }
            return 0;
        }
    }

    enum NextPrevWordGroupMask {
        NoOne = 0x00000000,
        Space = 0x00000001,
        Tab = 0x00000002,
        Paragraph = 0x00000004,

        SingleQuote = 0x00000008,
        LeftSingleQuote = 0x00000010,
        RightSingleQuote = 0x00000020,

        LeftDoubleQuote = 0x00000040,
        RightDoubleQuote = 0x00000080,
        DoubleQuote = 0x00000100,

        PunctuationMark = 0x00000200,

        Others = 0x00000400,
    }
} 