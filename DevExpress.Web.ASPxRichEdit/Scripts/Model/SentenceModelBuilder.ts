module __aspxRichEdit {
    export class SwitchTextCaseManipulator {
        dispatcher: ModelManipulator;

        constructor(dispatcher: ModelManipulator) {
            this.dispatcher = dispatcher;
        }
    }

    export class WordPart {
        position: number; // absolute
        text: string; // ONLY for text\dash. Other case here it some number of spaces
        type: LayoutBoxType;

        constructor(position: number, text: string, type: LayoutBoxType) {
            this.position = position;
            this.text = text;
            this.type = type;
        }

        public merge(pos: number, text: string, type: LayoutBoxType): boolean {
            if (this.position + this.text.length == pos && this.type == type && (type == LayoutBoxType.Text || type == LayoutBoxType.Dash || type == LayoutBoxType.Space)) {
                this.text += text;
                return true;
            }
            return false;
        }

        getEndPosition(): number {
            return this.position + this.text.length;
        }
    }

    export class SentenceWord {
        parts: WordPart[] = [];

        getLastWordPart(): WordPart {
            return this.parts[this.parts.length - 1];
        }
    }

    export class Sentence {
        words: SentenceWord[] = [];

        getLastWord(): SentenceWord {
            return this.words[this.words.length - 1];
        }
    }

    export class SentenceStructureBuilder {
        sentences: Sentence[];
        interval: FixedInterval;
        layout: DocumentLayout;
        subDocument: SubDocument;
        selection: Selection;

        private layoutBoxIterator: LayoutBoxIteratorBase;
        private currSentence: Sentence = null;
        private currWord: SentenceWord = null;
        private currWordPart: WordPart = null;
        private isSentenceEnd: boolean = false;
        private isWordEnd: boolean = false;
        private findEndLastSentence: boolean = false;
        private lastBox: LayoutBox;

        constructor(subDocument: SubDocument, layout: DocumentLayout, selection: Selection, interval: FixedInterval) {
            this.subDocument = subDocument
            this.layout = layout;
            this.interval = interval;
            this.sentences = [];
            this.selection = selection;
        }

        // splitByInterval by default - true
        public static getBuilder(control: IRichEditControl, subDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval, splitByInterval: boolean): SentenceStructureBuilder {
            var builder: SentenceStructureBuilder = new SentenceStructureBuilder(subDocument, layout, control.selection, interval);
            while (!builder.build())
                control.forceFormatPage(layout.validPageCount);

            if (splitByInterval)
                SentenceStructureBuilder.splitPartsByInterval(builder.sentences, interval);

            SentenceStructureBuilder.correctFirstSentence(builder.sentences);
            return builder;
        }

        // for the case when have text " FirstWord!". After have 2 sentences. " " and "FirstWord!"
        private static correctFirstSentence(sentences: Sentence[]) {
            var firstSentenceWords: SentenceWord[] = sentences[0].words;

            for (var startFirstSentenceWordIndex: number = 0, word: SentenceWord; word = firstSentenceWords[startFirstSentenceWordIndex]; startFirstSentenceWordIndex++) {
                var type: LayoutBoxType = word.parts[0].type;
                if (type != LayoutBoxType.Space && type != LayoutBoxType.LineBreak && type != LayoutBoxType.TabSpace)
                    break;
            }

            if (startFirstSentenceWordIndex > 0 && startFirstSentenceWordIndex < firstSentenceWords.length) {
                var newFirstSentence: Sentence = new Sentence();
                for (; startFirstSentenceWordIndex > 0; startFirstSentenceWordIndex--)
                    newFirstSentence.words.push(firstSentenceWords.shift());
                sentences.unshift(newFirstSentence);
            }
        }

        public static splitPartsByInterval(sentences: Sentence[], interval: FixedInterval) {
            SentenceStructureBuilder.splitPartsByPosition(sentences, interval.start);
            SentenceStructureBuilder.splitPartsByPosition(sentences, interval.end());
        }

        public static splitPartsByPosition(sentences: Sentence[], position: number) {
            var sentenceIndex: number = Math.max(0, Utils.normedBinaryIndexOf(sentences, (s: Sentence) => s.words[0].parts[0].position - position));
            var sentence: Sentence = sentences[sentenceIndex];

            var wordIndex: number = Math.max(0, Utils.normedBinaryIndexOf(sentence.words, (w: SentenceWord) => w.parts[0].position - position));
            var sentenceWord: SentenceWord = sentence.words[wordIndex];

            for (var wordPartIndex: number = 0, part: WordPart; part = sentenceWord.parts[wordPartIndex]; wordPartIndex++) {
                if (part.position > position)
                    break;
            }
            wordPartIndex = wordPartIndex > 0 ? wordPartIndex - 1 : 0;
            part = sentenceWord.parts[wordPartIndex];

            if (position > part.position && position < part.position + part.text.length) {
                sentenceWord.parts.splice(wordPartIndex + 1, 0, new WordPart(position, part.text.substr(position - part.position), part.type));
                part.text = part.text.substr(0, position - part.position);
            }
        }

        // when false - need calculate more layout
        public build(): boolean {
            let endPosition = (this.subDocument.isMain() ? this.layout.getLastValidPage() : this.layout.pages[this.selection.pageIndex]).getEndPosition(this.subDocument);
            if (this.findEndLastSentence) {
                this.interval = FixedInterval.fromPositions(this.interval.end(), endPosition);
                this.layoutBoxIterator.resetToInterval(this.interval.start, this.interval.end());
                this.collect();
            }
            else {
                if (this.needCalculateMoreLayout())
                    return false;
                var firstSentenceStartPosition: number = this.findFirstSentenceStartPosition();

                this.layoutBoxIterator = this.subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(this.subDocument, this.layout, firstSentenceStartPosition, this.interval.end()) : new LayoutBoxIteratorOtherSubDocument(this.subDocument, this.layout, firstSentenceStartPosition, this.interval.end(), this.selection.pageIndex);

                this.addNewSentence();
                this.addNewWord();

                this.collect();
                this.findEndLastSentence = true;
            }

            if (this.currSentence.words.length == 1 && this.currWord.parts.length == 0) { // mean that we get interval that exactly select sentence
                this.sentences.pop();
                return true;
            }

            if (this.interval.end() == endPosition)
                return this.layout.isFullyFormatted;
            else
                return this.build();
        }

        private collect() {
            while (this.layoutBoxIterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false))) {
                var layoutPos: LayoutPosition = this.layoutBoxIterator.position;
                if (this.lastBox && this.lastBox === layoutPos.box)
                    continue;
                this.lastBox = layoutPos.box;
                switch (layoutPos.box.getType()) {
                    case LayoutBoxType.Text:
                        var textBox: LayoutTextBox = <LayoutTextBox>layoutPos.box;
                        for (var charIndex: number = layoutPos.charOffset, currAbsolutePosition: number = layoutPos.getLogPosition() + charIndex, char: string;
                                char = textBox.text[charIndex]; charIndex++, currAbsolutePosition++) {
                            
                            switch (char) {
                                case ".":
                                case ";":
                                    if (this.isSentenceEnd) {
                                        this.addNewSentence();
                                        this.addNewWord();
                                        if (this.findEndLastSentence)
                                            return;
                                    }
                                    this.addNewWordPart(currAbsolutePosition, char, LayoutBoxType.Text);
                                    this.isSentenceEnd = true;
                                    break;
                                case "!":
                                    var prevChar: string = Utils.getSymbolFromEnd(this.currWordPart.text, 1);
                                    if (this.isSentenceEnd && prevChar && prevChar != "!") {
                                        this.addNewSentence();
                                        this.addNewWord();
                                        if (this.findEndLastSentence)
                                            return;
                                    }
                                    this.addNewWordPart(currAbsolutePosition, char, LayoutBoxType.Text);
                                    this.isSentenceEnd = true;
                                    break;
                                case "?":
                                    var prevChar: string = Utils.getSymbolFromEnd(this.currWordPart.text, 1);
                                    if (this.isSentenceEnd && !(prevChar && prevChar == "!")) {
                                        this.addNewSentence();
                                        this.addNewWord();
                                        if (this.findEndLastSentence)
                                            return;
                                    }
                                    this.addNewWordPart(currAbsolutePosition, char, LayoutBoxType.Text);
                                    this.isSentenceEnd = true;
                                    break;
                                // word separators
                                //case Utils.specialCharacters.Underscore: // ??
                                case Utils.specialCharacters.Dot:
                                case Utils.specialCharacters.Hyphen:
                                case Utils.specialCharacters.TrademarkSymbol:
                                case Utils.specialCharacters.CopyrightSymbol:
                                case Utils.specialCharacters.RegisteredTrademarkSymbol:
                                case Utils.specialCharacters.Ellipsis:
                                case Utils.specialCharacters.LeftDoubleQuote:
                                case Utils.specialCharacters.LeftSingleQuote:
                                case Utils.specialCharacters.RightDoubleQuote:
                                case Utils.specialCharacters.RightSingleQuote:
                                case Utils.specialCharacters.OpeningDoubleQuotationMark:
                                case Utils.specialCharacters.OpeningSingleQuotationMark:
                                case Utils.specialCharacters.ClosingDoubleQuotationMark:
                                case Utils.specialCharacters.ClosingSingleQuotationMark:
                                case Utils.specialCharacters.RegisteredTrademarkSymbol:
                                case Utils.specialCharacters.RegisteredTrademarkSymbol:
                                case ",": case "@": case "#": case "$": case "%": case "^": case "&": case "*": case "(":
                                case ")": case "=": case "+": case "[": case "]": case "{": case "}": case "\\": case "|":
                                case ":": case "\'": case "\"": case "<": case ">": case "/": case "~": case "`":
                                    if (this.isWordEnd)
                                        this.addNewWord();
                                    this.addNewWordPart(currAbsolutePosition, char, LayoutBoxType.Text);
                                    this.isWordEnd = true;
                                    break;
                                default:
                                    if (this.isSentenceEnd) {
                                        this.addNewSentence();
                                        this.addNewWord();
                                        if (this.findEndLastSentence)
                                            return;
                                        this.isSentenceEnd = false;
                                        this.isWordEnd = false;
                                    }
                                    else if (this.isWordEnd) {
                                        this.addNewWord();
                                        this.isWordEnd = false;
                                    }

                                    this.addNewWordPart(currAbsolutePosition, char, LayoutBoxType.Text);
                                    break;
                            }
                        }
                        break;
                    case LayoutBoxType.SectionMark:
                    case LayoutBoxType.ParagraphMark:
                        this.addNewWordPart(layoutPos.getLogPosition(), " ", layoutPos.box.getType());
                        this.addNewSentence();
                        this.addNewWord();
                        if (this.findEndLastSentence)
                            return;
                        this.isSentenceEnd = false;
                        this.isWordEnd = false;
                        break;
                    case LayoutBoxType.Dash:
                    case LayoutBoxType.Space:
                    case LayoutBoxType.TabSpace:
                        this.addNewWordPart(layoutPos.getLogPosition(), " ", layoutPos.box.getType());
                        this.isWordEnd = true;
                        break;
                    default:
                        if(this.currWord.parts.length > 0)
                            this.addNewWord();
                        this.addNewWordPart(layoutPos.getLogPosition(), " ", layoutPos.box.getType());
                        this.addNewWord();
                        break;
                }
            }
        }

        private addNewSentence() {
            this.currSentence = new Sentence();
            this.sentences.push(this.currSentence);
            this.currWord = null;
            this.currWordPart = null;
        }

        private addNewWord() {
            this.currWord = new SentenceWord();
            this.currSentence.words.push(this.currWord);
            this.currWordPart = null;
        }

        private addNewWordPart(pos: number, text: string, type: LayoutBoxType) {
            var mergeSuccess: boolean = this.currWordPart && this.currWordPart.merge(pos, text, type);
            if (!mergeSuccess) {
                this.currWordPart = new WordPart(pos, text, type);
                this.currWord.parts.push(this.currWordPart);
            }
        }

        private needCalculateMoreLayout(): boolean {
            var lastValidPage: LayoutPage = this.subDocument.isMain() ? this.layout.getLastValidPage() : this.layout.pages[this.selection.pageIndex];
            return lastValidPage && lastValidPage.getEndPosition(this.subDocument) < this.interval.end();
        }

        private findFirstSentenceStartPosition(): number {
            var layoutBoxIterator: LayoutBoxIteratorBase = this.subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(this.subDocument, this.layout, 0, this.interval.start) : new LayoutBoxIteratorOtherSubDocument(this.subDocument, this.layout, 0, this.interval.start, this.selection.pageIndex);

            var suspiciousPosition: number = -1;
            var isFindSentenceStart: boolean = false;

            while (layoutBoxIterator.movePrev(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true))) {
                var layoutPos: LayoutPosition = layoutBoxIterator.position;
                switch (layoutPos.box.getType()) {
                    case LayoutBoxType.Text:
                        var textBox: LayoutTextBox = <LayoutTextBox>layoutPos.box;
                        var lastIndexBox: number = layoutPos.charOffset > 0 || layoutPos.getLogPosition() >= this.interval.start ? layoutPos.charOffset : textBox.text.length - 1;
                        for (var charIndex: number = lastIndexBox, char: string; char = textBox.text[charIndex]; charIndex--) {
                            switch (char) {
                                case ".": case "!": case "?": case ";":
                                    if (layoutPos.getLogPosition() != this.interval.start)
                                        isFindSentenceStart = true;
                                    break;
                                default:
                                    suspiciousPosition = layoutPos.getLogPosition(); // store only position when new sentence start
                                    break;
                            }
                            if (isFindSentenceStart)
                                break;
                        }
                        break;
                    case LayoutBoxType.SectionMark:
                    case LayoutBoxType.ParagraphMark:
                        layoutBoxIterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));
                        suspiciousPosition = layoutBoxIterator.position.getLogPosition(); // store only position when new sentence start
                        isFindSentenceStart = true;
                        break;
                }
                if (isFindSentenceStart)
                    break;
            }

            if (suspiciousPosition < 0)
                return layoutBoxIterator.position.getLogPosition(); // 0 or first page position
            else
                return suspiciousPosition;
        }
    }


}