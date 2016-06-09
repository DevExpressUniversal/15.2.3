module __aspxRichEdit {
    export enum FindReplaceState {
        Start = 0,
        Found = 1,
        DocumentBegin = 2,
        DocumentEnd = 3,
        SearchEnd = 4
    }

    export enum SearchDirection {
        Up = 1,
        Down = 2,
        All = 3
    }

    // Create class when create dialog and destroy when dialog end
    // This class bind with subDocument, so create one copy for main, header and footer, comments
    // if selected interval changed, then need recall setSearchParams to update this.findWithPosition, this.currentPos!!!

    export class FindReplaceHelper {
        private subDocument: SubDocument;
        private layout: DocumentLayout;
        private control: IRichEditControl;

        // SEARCH PARAMS
        private template: string;
        private replaceWith: string; // may be null;
        private searchDirection: SearchDirection;
        private matchCase: boolean;
        private regularExpression: boolean; // don't work now
        private wholeWordsOnly: boolean;

        // HELP VARS
        private findWithPosition: number; // ~selection.getLastInterval().start
        private currentPos: number;
        private lastFound: FixedInterval;
        private supportFunction: number[];
        private templateLength: number;
        private state: FindReplaceState;
        private beginOrStartDocumentReach: boolean;

        constructor(control: IRichEditControl, subDocument: SubDocument, layout: DocumentLayout) {
            this.control = control;
            this.subDocument = subDocument;
            this.layout = layout;
        }

        // this functions can be called many times
        // replaceWith may be null
        public setSearchParams(whatFind: string, replaceWith: string, searchDirection: SearchDirection, matchCase: boolean, regularExpression: boolean,
            findWithPosition: number, wholeWordsOnly: boolean) {
            if (findWithPosition < 0 || findWithPosition > this.subDocument.getDocumentEndPosition())
                throw "In FindReplaceHelper setSearchParams findWithPosition < 0 || findWithPosition > this.subDocument.getDocumentEndPosition()";
            if (wholeWordsOnly && regularExpression)
                throw "In FindReplaceHelper setSearchParams can't set wholeWordsOnly && regularExpression";
            if (wholeWordsOnly && FindReplaceHelper.isCanSetWholeWordsOnlyForThisExpression(whatFind) >= 0)
                throw "In FindReplaceHelper setSearchParams whatFind expression consider not Alphanumeric char[" + FindReplaceHelper.isCanSetWholeWordsOnlyForThisExpression(whatFind) + "]";

            var recalculateSuppFunc: boolean = (this.template != (matchCase ? whatFind : whatFind.toUpperCase())) ||
                (this.regularExpression != regularExpression) || (this.matchCase != matchCase) ||
                (this.searchDirection == SearchDirection.Up && searchDirection != SearchDirection.Up) || // see that later
                (this.searchDirection != SearchDirection.Up && searchDirection == SearchDirection.Up);
            this.replaceWith = replaceWith;
            this.searchDirection = searchDirection;
            this.regularExpression = regularExpression;
            this.matchCase = matchCase;
            this.wholeWordsOnly = wholeWordsOnly;
            this.template = this.matchCase ? whatFind : whatFind.toUpperCase();
            this.lastFound = null;
            this.state = FindReplaceState.Start;
            this.beginOrStartDocumentReach = false;

            this.findWithPosition = findWithPosition;
            this.currentPos = findWithPosition;
            this.templateLength = whatFind.length;
            if (!this.wholeWordsOnly && recalculateSuppFunc || !this.supportFunction) // when enabled wholeWordsOnly, we don't need for supportFunc
                this.crateSupportFunction();
        }

        // return: -1 - ok. ">= 0" - error
        public static isCanSetWholeWordsOnlyForThisExpression(expression: string): number {
            for (var i: number = 0; i < expression.length; i++)
                if (!expression[i].match(Utils.isAlphanumeric))
                    return i;
            return -1;
        }

        public findNext(): FindReplaceState {
            if (this.state == FindReplaceState.SearchEnd)
                this.setSearchParams(this.template, this.replaceWith, this.searchDirection, this.matchCase, this.regularExpression, this.findWithPosition, this.wholeWordsOnly);

            var newState: FindReplaceState;
            switch (this.searchDirection) {
                case SearchDirection.Down: newState = this.findNextDown(); break;
                case SearchDirection.All:  newState = this.findNextAll(); break;
                case SearchDirection.Up:   newState = this.findNextUp(); break;
            }
            if (newState == FindReplaceState.DocumentEnd || newState == FindReplaceState.DocumentBegin)
                this.beginOrStartDocumentReach = true;
            return newState;
        }

        private findNextDown(): FindReplaceState {
            switch (this.state) {
                case FindReplaceState.Start:
                case FindReplaceState.Found:
                    var isBelowStartPos: boolean = this.currentPos >= this.findWithPosition;
                    if (this.findNextDownInternal(this.currentPos, isBelowStartPos ? this.subDocument.getDocumentEndPosition() : this.findWithPosition))
                        this.state = FindReplaceState.Found;
                    else {
                        if (!isBelowStartPos || this.findWithPosition == 0)
                            this.state = FindReplaceState.SearchEnd;
                        else
                            this.state = FindReplaceState.DocumentEnd;
                    }
                    break;
                case FindReplaceState.DocumentEnd:
                    if (this.findWithPosition == 0)
                        this.state = FindReplaceState.SearchEnd;
                    else {
                        if (this.findNextDownInternal(0, this.findWithPosition))
                            this.state = FindReplaceState.Found;
                        else
                            this.state = FindReplaceState.SearchEnd;
                    }
                    break;
            }
            return this.state;
        }

        private findNextUp(): FindReplaceState {
            switch (this.state) {
                case FindReplaceState.Start:
                case FindReplaceState.Found:
                    var isAboveStartPos: boolean = this.currentPos <= this.findWithPosition;
                    if (this.beginOrStartDocumentReach && isAboveStartPos) {
                        this.lastFound = null;
                        this.state = FindReplaceState.SearchEnd;
                        return this.state;
                    }

                    if (this.findNextUpInternal(isAboveStartPos ? 0 : this.findWithPosition, this.currentPos))
                        this.state = FindReplaceState.Found;
                    else {
                        if (!isAboveStartPos || this.findWithPosition == this.subDocument.getDocumentEndPosition() - 1)
                            this.state = FindReplaceState.SearchEnd;
                        else
                            this.state = FindReplaceState.DocumentBegin;
                    }
                    break;
                case FindReplaceState.DocumentBegin:
                    var docEnd: number = this.subDocument.getDocumentEndPosition();
                    if (this.findWithPosition == docEnd - 1)
                        this.state = FindReplaceState.SearchEnd;
                    else {
                        if (this.findNextUpInternal(this.findWithPosition, docEnd - 1))
                            this.state = FindReplaceState.Found;
                        else
                            this.state = FindReplaceState.SearchEnd;
                    }
                    break;
            }
            return this.state;
        }

        private findNextAll(): FindReplaceState {
            while (this.findNextDown() == FindReplaceState.DocumentEnd);
            return this.state;
        }

        // true - found, False = found end of interval
        private findNextDownInternal(lowerPosition: number, greaterPosition: number): boolean {
            var charIterator: ForwardCharacterIterator = new ForwardCharacterIterator(this.control, this.subDocument, lowerPosition, greaterPosition);

            if (this.wholeWordsOnly)
                return this.findNextDownWholeWordsOnly(charIterator, lowerPosition, greaterPosition);
            
            var offset: number = 0;
            var posWhenStartEquivalents: number = -1;
            while (charIterator.nextChar()) {
                if (!this.matchCase)
                    charIterator.char = charIterator.char.toUpperCase();

                while (offset > 0 && this.template[offset] != charIterator.char)
                    offset = this.supportFunction[offset - 1];

                if (this.template[offset] == charIterator.char) {
                    if (offset == 0)
                        posWhenStartEquivalents = charIterator.getCurrLogPosition();
                    offset++;
                }

                if (offset == this.templateLength) {
                    this.lastFound = FixedInterval.fromPositions(posWhenStartEquivalents, charIterator.getCurrLogPosition() + 1);
                    Field.correctIntervalDueToFields(this.layout, this.subDocument, this.lastFound, this.control.selection.pageIndex);
                    this.currentPos = this.lastFound.end();
                    return true;
                }
            }
            this.currentPos = greaterPosition;
            this.lastFound = null;
            return false;
        }

        private findNextDownWholeWordsOnly(charIterator: ForwardCharacterIterator, lowerPosition: number, greaterPosition: number): boolean {
            var offset: number = 0;
            if (this.layout.pages[0].contentIntervals[0].start < lowerPosition &&
                this.isCharCanBeInWord((this.subDocument.isMain() ? new LayoutPositionMainSubDocumentCreator(this.layout, this.subDocument, lowerPosition - 1, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.layout, this.subDocument, lowerPosition - 1, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true))))
                    offset = -1;

            var posWhenStartEquivalents: number = -1;
            while (charIterator.nextChar()) {
                if (offset == this.templateLength) {
                    if (charIterator.char.match(Utils.isAlphanumeric)) {
                        offset = -1;
                        continue;
                    }
                    else {
                        this.lastFound = FixedInterval.fromPositions(posWhenStartEquivalents, charIterator.getCurrLogPosition() + 1);
                        this.currentPos = this.lastFound.end();
                        return true;
                    }
                }

                if (offset < 0 && !charIterator.char.match(Utils.isAlphanumeric) || offset >= 0 && this.template[offset] == (this.matchCase ? charIterator.char : charIterator.char.toUpperCase())) {
                    offset += 1;
                    if (offset == 0)
                        posWhenStartEquivalents = charIterator.getCurrLogPosition();
                }
                else
                    offset = -1;
            }
            this.currentPos = greaterPosition;
            this.lastFound = null;
            return false;
        }

        // true - found, False = found end of interval
        private findNextUpInternal(lowerPosition: number, greaterPosition: number): boolean {
            if (greaterPosition < 1) {
                this.currentPos = 0;
                this.lastFound = null;
                return false;
            }

            var charIterator: BackwardCharacterIterator = new BackwardCharacterIterator(this.control, this.subDocument, lowerPosition, greaterPosition);

            if (this.wholeWordsOnly)
                return this.findNextUpWholeWordsOnly(charIterator, lowerPosition, greaterPosition);

            var offset: number = 0;
            var templateLastInd = this.templateLength - 1;
            var posWhenStartEquivalents: number = -1;
            while (charIterator.prevChar()) {
                if (!this.matchCase)
                    charIterator.char = charIterator.char.toUpperCase();

                while (offset > 0 && this.template[templateLastInd - offset] != charIterator.char)
                    offset = this.supportFunction[offset - 1];

                if (this.template[templateLastInd - offset] == charIterator.char) {
                    if (offset == 0)
                        posWhenStartEquivalents = charIterator.getCurrLogPosition() + 1;
                    offset++;
                }

                if (offset == this.templateLength) {
                    this.lastFound = FixedInterval.fromPositions(charIterator.getCurrLogPosition(), posWhenStartEquivalents);
                    this.currentPos = Math.max(0, this.lastFound.start - 1);
                    return true;
                }
            }
            this.currentPos = 0;
            this.lastFound = null;
            return false;
        }

        private findNextUpWholeWordsOnly(charIterator: BackwardCharacterIterator, lowerPosition: number, greaterPosition: number): boolean {
            var offset: number = 0;

            var layoutPage = this.subDocument.isMain() ? this.layout.getLastValidPage() : this.layout.pages[this.control.selection.pageIndex];
            if (greaterPosition < layoutPage.getEndPosition(this.subDocument) - 1 &&
                this.isCharCanBeInWord((this.subDocument.isMain() ? new LayoutPositionMainSubDocumentCreator(this.layout, this.subDocument, greaterPosition + 1, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.layout, this.subDocument, greaterPosition + 1, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(true), new LayoutPositionCreatorConflictFlags().setDefault(false))))
                offset = -1;
            else
                posWhenStartEquivalents = charIterator.getCurrLogPosition();

            var templateLastInd = this.templateLength - 1;
            var posWhenStartEquivalents: number = -1;
            while (charIterator.prevChar()) {
                if (offset == this.templateLength) {
                    if (charIterator.char.match(Utils.isAlphanumeric)) {
                        offset = -1;
                        continue;
                    }
                    else {
                        this.lastFound = FixedInterval.fromPositions(charIterator.getCurrLogPosition(), posWhenStartEquivalents + 1);
                        this.currentPos = this.lastFound.start;
                        return true;
                    }
                }
                
                if (offset < 0) {
                    offset = charIterator.char.match(Utils.isAlphanumeric) ? -1 : 0;
                    if (offset == 0)
                        posWhenStartEquivalents = charIterator.getCurrLogPosition();
                }
                else {
                    if (this.template[templateLastInd - offset] == (this.matchCase ? charIterator.char : charIterator.char.toUpperCase()))
                        offset += 1;
                }
            }

            if (offset == this.templateLength) {
                this.currentPos = 0;
                this.lastFound = new FixedInterval(0, posWhenStartEquivalents + 1);
                return true;
            }

            this.currentPos = 0;
            this.lastFound = null;
            return false;
        }

        private isCharCanBeInWord(layoutPos: LayoutPosition): boolean {
            if (!layoutPos)
                return false;
            layoutPos.switchToStartNextBoxInRow();
            if (layoutPos.box.getType() != LayoutBoxType.Text && layoutPos.box.getType() != LayoutBoxType.Dash)
                return false;
            var char: string;
            if (layoutPos.charOffset >= layoutPos.box.getLength()) {
                if (layoutPos.advanceToNextRow(this.layout))
                    char = layoutPos.row.boxes[0].renderGetContent(null, null, null)[0];
                else
                    return false;
            }
            else
                char = layoutPos.box.renderGetContent(null, null, null)[layoutPos.charOffset];
            return char.match(Utils.isAlphanumeric).length > 0;
        }

        public getLastFound(): FixedInterval {
            return this.lastFound ? this.lastFound.clone() : null;
        }

        // in top level need call findNext() manually
        public replaceLastFound() {
            if (!this.lastFound || !this.replaceWith)
                return;

            var control: IRichEditControl = this.control;
            var firstRun: TextRun = this.subDocument.getRunByPosition(this.lastFound.start);
            var charStyle: CharacterStyle = firstRun.characterStyle;
            var maskedCharProp: MaskedCharacterProperties = firstRun.maskedCharacterProperties;

            control.history.beginTransaction();
            control.history.addAndRedo(new SetSelectionHistoryItem(control.modelManipulator, this.subDocument, [this.lastFound], control.selection, UpdateInputPositionProperties.No, control.selection.endOfLine));
            control.history.addAndRedo(new RemoveIntervalHistoryItem(control.modelManipulator, this.subDocument, this.lastFound, false));
            control.history.addAndRedo(new InsertTextHistoryItem(control.modelManipulator, this.subDocument, this.lastFound.start, this.replaceWith, maskedCharProp, charStyle));
            control.history.endTransaction();

            var diff: number = this.replaceWith.length - this.templateLength;
            if (this.searchDirection != SearchDirection.Up)
                this.currentPos += diff;
            if (this.findWithPosition >= this.lastFound.end())
                this.findWithPosition += diff;
            this.lastFound = null;
        }

        // replace All matches, not matter what was previously
        public replaceAll() {
            var oldFindWithPosition: number = this.findWithPosition;
            this.setSearchParams(this.template, this.replaceWith, this.searchDirection, this.matchCase, this.regularExpression, 0, this.wholeWordsOnly);

            var control: IRichEditControl = this.control;
            var storedPosition: number = control.selection.getLastSelectedInterval().start;
            var diff: number = this.replaceWith.length - this.templateLength;

            this.currentPos = 0;
            control.history.beginTransaction();
            while (this.findNextAll() == FindReplaceState.Found) {
                this.replaceLastFound();
                if (storedPosition >= this.currentPos - this.replaceWith.length + this.templateLength)
                    storedPosition += diff;
            }

            control.history.addAndRedo(new SetSelectionHistoryItem(control.modelManipulator, this.subDocument, [new FixedInterval(storedPosition, 0)],
                control.selection, UpdateInputPositionProperties.Yes, control.selection.endOfLine));
            control.history.endTransaction();

            this.setSearchParams(this.template, this.replaceWith, this.searchDirection, this.matchCase, this.regularExpression, oldFindWithPosition, this.wholeWordsOnly);
        }

        //  a  b  a  b  a  b  a  b  c  a
        //  0  0  1  2  3  4  5  6  0  1
        private crateSupportFunction() {
            var template: string;
            // reverse template
            if (this.searchDirection == SearchDirection.Up) {
                template = "";
                for (var i: number = this.templateLength - 1; i >= 0; i--)
                    template += this.template[i];
            }
            else
                template = this.template;

            this.supportFunction = [0]; // only 3 or more
            for (var topBound: number = 2; topBound < this.templateLength; topBound++)
                this.supportFunction.push(FindReplaceHelper.calcOneElemSuppFunc(template, topBound));
        }

        // calculate one element for supportFunction
        private static calcOneElemSuppFunc(template: string, topBound: number): number {
            // offset about himself
            for (var offset: number = 1; offset < topBound; offset++) {
                var lenComparePhrase: number = topBound - offset;
                for (var i: number = 0; i < lenComparePhrase; i++) {
                    if (template[i] != template[i + offset])
                        break;
                }
                if (i == lenComparePhrase) // equivalent
                    return topBound - offset;
            }
            return 0;
        }
    }

    class CharacterIteratorBase {
        subDocument: SubDocument;
        iterator: LayoutBoxIteratorBase;
        control: IRichEditControl;
        char: string;
        charIndexInBox: number;

        constructor(control: IRichEditControl, subDocument: SubDocument, startPosition: number, endPosition: number) {
            this.control = control;
            this.subDocument = subDocument;
            this.iterator = subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(subDocument, control.layout, startPosition, endPosition) : new LayoutBoxIteratorOtherSubDocument(subDocument, control.layout, startPosition, endPosition, control.selection.pageIndex);
            while(!this.iterator.isInitialized())
                control.forceFormatPage(control.layout.validPageCount);
            this.char = null;
            this.charIndexInBox = -1;
        }

        public getCurrLogPosition(): number {
            return this.iterator.position.getLogPosition(DocumentLayoutDetailsLevel.Box) + this.charIndexInBox;
        }

        public getCharInternal() {
            switch (this.iterator.position.box.getType()) {
                case LayoutBoxType.Text:
                    this.char = (<LayoutTextBox>this.iterator.position.box).text[this.charIndexInBox];
                    break;
                case LayoutBoxType.Dash:
                    this.char = (<LayoutDashBox>this.iterator.position.box).text[this.charIndexInBox];
                    break;
                case LayoutBoxType.Space:
                    this.char = Utils.specialCharacters.Space;
                    break;
                case LayoutBoxType.TabSpace:
                    this.char = Utils.specialCharacters.TabMark;
                    break;
                default:
                    this.char = String.fromCharCode(0);
            }
        }
    }

    class ForwardCharacterIterator extends CharacterIteratorBase {
        public getCharInternal(): boolean {
            if (this.charIndexInBox >= this.iterator.position.box.getLength())
                return false;
            super.getCharInternal();
            return true;
        }

        nextChar(): boolean {
            if (this.charIndexInBox == -1) {
                this.iterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
                this.charIndexInBox = this.iterator.position.charOffset;
                if (this.getCharInternal())
                    return true;
            }
            else
                this.charIndexInBox++;

            this.char = null;

            while (this.char == null && this.iterator.position.getLogPosition(DocumentLayoutDetailsLevel.Box) + this.charIndexInBox < this.iterator.intervalEnd) {
                if(!this.getCharInternal()) {
                    if (this.iterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true)))
                        this.charIndexInBox = this.iterator.position.charOffset;
                    else
                        return false;
                }
            }
            return !!this.char;
        }
    }

    class BackwardCharacterIterator extends CharacterIteratorBase {
        public getCharInternal(): boolean {
            if (this.charIndexInBox < 0)
                return false;
            super.getCharInternal();
            return true;
        }

        prevChar(): boolean {
            if (this.charIndexInBox == -1) {
                this.iterator.movePrev(new LayoutPositionCreatorConflictFlags().setDefault(true), new LayoutPositionCreatorConflictFlags().setDefault(false));
                this.charIndexInBox = this.iterator.position.charOffset - 1;
                if (this.charIndexInBox == -1) {
                    if (!this.iterator.movePrev(new LayoutPositionCreatorConflictFlags().setDefault(true), new LayoutPositionCreatorConflictFlags().setDefault(false)))
                        return false;
                    this.charIndexInBox = this.iterator.position.box.getLength() - 1;
                }
                if (this.getCharInternal())
                    return true;
                this.charIndexInBox = -1;
            }
            else
                this.charIndexInBox--;

            this.char = null;
            while (this.char == null && this.iterator.position.getLogPosition(DocumentLayoutDetailsLevel.Box) + this.charIndexInBox >= this.iterator.intervalStart) {
                if (!this.getCharInternal()) {
                    if (this.iterator.movePrev(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true)))
                        this.charIndexInBox = this.iterator.position.box.getLength() - 1;
                    else
                        return false;
                }
            }
            return !!this.char;
        }
    }
}