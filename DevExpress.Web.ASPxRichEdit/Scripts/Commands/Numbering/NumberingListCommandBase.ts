module __aspxRichEdit {
    export class NumberingListCommandBase extends CommandBase<IntervalCommandStateEx> {
        getState(): IntervalCommandStateEx {
            var intervals = this.control.selection.getIntervalsClone();
            return new IntervalCommandStateEx(this.isEnabled(), intervals, this.getValue(intervals));
        }
        getValue(intervals: FixedInterval[]): boolean {
            return this.areAllParagraphsHasValidNumberingListType(intervals);
        }
        executeCore(state: IntervalCommandStateEx, parameter: any): boolean {
            this.control.history.beginTransaction();
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(state.intervals);
            if(state.value)
                this.deleteNumberingList(paragraphIndices);
            else
                this.insertNumberingList(paragraphIndices, (typeof (parameter) == "number") ? parameter : -1);
            this.control.history.endTransaction();
            return true;
        }
        deleteNumberingList(paragraphIndices: number[]) {
            for(let i = paragraphIndices.length - 1; i >= 0; i--) {
                let paragraphIndex = paragraphIndices[i];
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                if(paragraph.isInList()) {
                    this.resetParagraphLeftIndent(paragraphIndex);
                    this.deleteNumberingListCore(paragraphIndex);
                }
            }
        }
        private resetParagraphLeftIndent(paragraphIndex: number) {
            var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
            var numberingList = paragraph.getNumberingList();
            var level = numberingList.levels[paragraph.getListLevelIndex()];
            var paragraphMergedProperties = paragraph.getParagraphMergedProperies();
            var interval = paragraph.getInterval();

            var leftIndent = paragraphMergedProperties.leftIndent;

            if(numberingList.getListType() === NumberingType.MultiLevel) {
                if(paragraphMergedProperties.firstLineIndentType == ParagraphFirstLineIndent.Hanging)
                    leftIndent -= paragraphMergedProperties.firstLineIndent;
                else
                    leftIndent -= level.getListLevelProperties().originalLeftIndent;
            }
            else
                leftIndent -= level.getListLevelProperties().originalLeftIndent;

            this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, interval, ParagraphFirstLineIndent.None, true));
            this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, interval, 0, true));
            this.control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, interval, Math.max(0, leftIndent), true));
        }
        private deleteNumberingListCore(paragraphIndex: number) {
            var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
            if(paragraph.numberingListIndex >= 0) {
                this.control.history.addAndRedo(new RemoveParagraphFromListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraphIndex));
            }
            else {
                if(paragraph.numberingListIndex === NumberingList.NumberingListNotSettedIndex)
                    this.control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, new FixedInterval(paragraph.startLogPosition.value, 0), paragraph.getParagraphMergedProperies().leftIndent, true));
                this.control.history.addAndRedo(new AddParagraphToListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraphIndex, NumberingList.NoNumberingListIndex, 0));
            }
        }

        insertNumberingList(paragraphIndices: number[], startIndex: number) {
            var calculator = new NumberingListIndexCalculator(this.control.model.activeSubDocument, this.getNumberingListType(), startIndex === undefined ? -1 : startIndex);
            var targetListInfo = calculator.getTargetListInfo(paragraphIndices);
            var targetListIndex = targetListInfo ? targetListInfo.listIndex : this.createNewList(this.getAbstractNumberingList());
            var targetListLevelIndex = targetListInfo ? targetListInfo.listlevelIndex : -1;

            var paragraphsLayoutPositions = this.getParagraphsLayoutPositions(paragraphIndices);
            var paragraphsLevelIndices = this.getParagraphsLevelIndices(paragraphIndices, paragraphsLayoutPositions, !!targetListInfo, targetListIndex, targetListLevelIndex);
            this.insertNumberingListCore(paragraphIndices, targetListIndex, paragraphsLevelIndices, paragraphsLayoutPositions);
        }

        getAbstractNumberingList() {
            return this.control.model.abstractNumberingListTemplates[this.getNumberingListTemplateIndex(this.getNumberingListType())];
        }

        insertNumberingListCore(paragraphIndices: number[], targetListIndex: number, paragraphsLevelIndices: number[], paragraphsLayoutPositions: LayoutPosition[]) {
            var subDocument = this.control.model.activeSubDocument;
            var paragraphIndicesLength = paragraphIndices.length;
            for(let i = 0; i < paragraphIndicesLength; i++) {
                let paragraphIndex = paragraphIndices[i];
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                this.processOldNumberingList(paragraph);
                var targetListLevel = paragraphsLevelIndices[i];
                this.deleteLeadingWhiteSpaces(paragraph, paragraphsLayoutPositions[i].row.boxes, targetListLevel < 0);
                if(targetListLevel >= 0) {
                    this.control.history.addAndRedo(new AddParagraphToListHistoryItem(this.control.modelManipulator, subDocument, paragraphIndex, targetListIndex, targetListLevel));
                    this.control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(this.control.modelManipulator, subDocument, new FixedInterval(paragraph.startLogPosition.value, 1), paragraph.maskedParagraphProperties.leftIndent, false));
                    this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(this.control.modelManipulator, subDocument, new FixedInterval(paragraph.startLogPosition.value, 1), paragraph.maskedParagraphProperties.firstLineIndent, false));
                }
            }
        }
        
        private processOldNumberingList(paragraph: Paragraph) {
            if(paragraph.isInList()) {
                if(paragraph.numberingListIndex == NumberingList.NumberingListNotSettedIndex) {
                    var leftIndent = paragraph.getParagraphMergedProperies().leftIndent;
                    this.control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, new FixedInterval(paragraph.startLogPosition.value, 1), leftIndent, true));
                }
            }
        }
        private deleteLeadingWhiteSpaces(paragraph: Paragraph, boxes: LayoutBox[], replaceOnIndent: boolean) {
            var length = 0;
            var leftIndent = 0;
            var subDocument = this.control.model.activeSubDocument;
            var manipulator = this.control.modelManipulator;
            for(var i = 0, box: LayoutBox; box = boxes[i]; i++) {
                if(box.isWhitespace()) {
                    length += box.getLength();
                    leftIndent += box.width;
                }
                else
                    break;
            }
            if(length > 0) {
                this.correctSelectionIntervals(new FixedInterval(paragraph.startLogPosition.value, length));
                this.control.history.addAndRedo(new RemoveIntervalHistoryItem(manipulator, subDocument, new FixedInterval(paragraph.startLogPosition.value, length), false));
            }

            if(replaceOnIndent && leftIndent > 0) {
                leftIndent = UnitConverter.pixelsToTwips(leftIndent);
                var properties = paragraph.getParagraphMergedProperies();
                var interval = paragraph.getInterval();

                if(properties.firstLineIndentType === ParagraphFirstLineIndent.Hanging) {
                    if(leftIndent < properties.firstLineIndent)
                        this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(manipulator, subDocument, interval, properties.firstLineIndent - leftIndent, true));
                    else if(properties.firstLineIndent === leftIndent) {
                        this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(manipulator, subDocument, interval, 0, true));
                        this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(manipulator, subDocument, interval, ParagraphFirstLineIndent.None, true));
                    }
                    else {
                        this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(manipulator, subDocument, interval, leftIndent - properties.firstLineIndent, true));
                        this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(manipulator, subDocument, interval, ParagraphFirstLineIndent.Indented, true));
                    }
                }
                else {
                    this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(manipulator, subDocument, interval, properties.firstLineIndent + leftIndent, true));
                    if(properties.firstLineIndentType === ParagraphFirstLineIndent.None)
                        this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(manipulator, subDocument, interval, ParagraphFirstLineIndent.Indented, true));
                }
            }
        }

        private correctSelectionIntervals(removingInterval: FixedInterval) {
            var intervals: FixedInterval[] = this.control.selection.getIntervalsClone();
            for(let i = 0, interval: FixedInterval; interval = intervals[i]; i++) {
                if(interval.start > removingInterval.start) {
                    var newSelectionEnd = Math.max(removingInterval.start, interval.end() - removingInterval.length);
                    var newSelectionStart = Math.max(removingInterval.start, interval.start - removingInterval.length);
                    intervals[i] = FixedInterval.fromPositions(newSelectionStart, newSelectionEnd);
                }
            }
            this.control.history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, intervals, this.control.selection, UpdateInputPositionProperties.Yes, this.control.selection.endOfLine));
        }

        getParagraphsLayoutPositions(paragraphIndices: number[]): LayoutPosition[] {
            var result: LayoutPosition[] = [];
            var paragraphIndicesLength = paragraphIndices.length;
            for(let i = 0; i < paragraphIndicesLength; i++) {
                var paragraphIndex = paragraphIndices[i];
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                var subDocument = this.control.model.activeSubDocument;
                var logPosition = paragraph.startLogPosition.value;
                var endRowConflictTags = new LayoutPositionCreatorConflictFlags().setDefault(false);
                var middleRowConflictTags = new LayoutPositionCreatorConflictFlags().setDefault(false);
                result.push(subDocument.isMain()
                    ? LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout, subDocument, logPosition,
                        DocumentLayoutDetailsLevel.Box, endRowConflictTags, middleRowConflictTags)
                    : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, logPosition, this.control.selection.pageIndex,
                        DocumentLayoutDetailsLevel.Box).create(endRowConflictTags, middleRowConflictTags)
                );
            }
            return result;
        }

        getParagraphsLevelIndices(paragraphIndices: number[], layoutPositions: LayoutPosition[], continueNumberingList: boolean, listIndex: number, listLevelIndex: number): number[]{
            var result: number[] = [];
            var numberingList = this.control.model.numberingLists[listIndex];
            var paragraphIndicesLength = paragraphIndices.length;
            for(let i = 0; i < paragraphIndicesLength; i++) {
                var paragraphIndex = paragraphIndices[i];
                if(listLevelIndex < 0) {
                    var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                    var layoutPosition = layoutPositions[i];
                    var box = layoutPosition.row.numberingListBox ? layoutPosition.row.numberingListBox : this.getStartBox(layoutPosition.row.boxes);
                    let boxX = box instanceof LayoutNumberingListBox ? (<LayoutNumberingListBox>box).textBox.x : box.x;
                    if(box instanceof LayoutParagraphMarkBox && (paragraphIndicesLength > 1 && (paragraph.length <= 1 || i !== 0)))
                        result.push(-1);
                    else
                        result.push(this.calculateParagraphListLevel(boxX, paragraph, numberingList));
                }
                else
                    result.push(listLevelIndex);
            }
            return result;
        }

        calculateParagraphListLevel(layoutLeftIndent: number, paragraph: Paragraph, numberingList: NumberingList): number {
            var modelLeftIndent = UnitConverter.pixelsToTwips(layoutLeftIndent);
            for(var i = 0, level: IOverrideListLevel; level = numberingList.levels[i]; i++) {
                var levelParagraphProperties = level.getParagraphMergedProperies();
                var actualNumberingPosition = levelParagraphProperties.firstLineIndentType == ParagraphFirstLineIndent.Hanging ? (levelParagraphProperties.leftIndent - levelParagraphProperties.firstLineIndent) : levelParagraphProperties.leftIndent;
                if(modelLeftIndent <= actualNumberingPosition)
                    return i;
            }
            return numberingList.levels.length - 1;
        }

        getStartBox(boxes: LayoutBox[]) {
            for(var i = 0, box: LayoutBox; box = boxes[i]; i++) {
                if(!box.isWhitespace())
                    return box;
            }
            return boxes[0];
        }

        createNewList(template: AbstractNumberingList): number {
            var abstractNumberingList = new AbstractNumberingList(this.control.model);
            abstractNumberingList.copyFrom(template);
            this.control.history.addAndRedo(new AddAbstractNumberingListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, abstractNumberingList));
            var abstractNumberingListIndex = this.control.model.abstractNumberingLists.length - 1;

            var numberingList = new NumberingList(this.control.model, abstractNumberingListIndex);
            this.control.history.addAndRedo(new AddNumberingListHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, numberingList));
            return this.control.model.numberingLists.length - 1;
        }

        processParagraphByIndex(paragraphIndex: number): boolean {
            return true;
        }
        getNumberingListTemplateIndex(type: NumberingType) {
            for(var i = 0, abstractNumberingList: AbstractNumberingList; abstractNumberingList = this.control.model.abstractNumberingListTemplates[i]; i++) {
                if(abstractNumberingList.getListType() === type)
                    return i;
            }
            return -1;
        }
        areAllParagraphsHasValidNumberingListType(intervals: FixedInterval[]): boolean {
            var isValid = true;
            var levelType = this.getNumberingListType();
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(intervals);
            for(let i = paragraphIndices.length - 1; i >= 0; i--) {
                let paragraphIndex = paragraphIndices[i];
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                if(!paragraph.isInList() || paragraph.getNumberingList().getLevelType(paragraph.getListLevelIndex()) !== levelType)
                    return false;
            }
            return true;
        }

        getNumberingListType(): NumberingType {
            throw new Error(Errors.NotImplemented);
        }
    }
} 