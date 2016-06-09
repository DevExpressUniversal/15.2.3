module __aspxRichEdit {
    export class TextCaseManipulator {
        manipulator: ModelManipulator;

        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        public applyUpperCase(control: IRichEditControl, subDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval): HistoryItemIntervalState<HistoryItemTextBufferStateObject> {
            return (new UpperCaseModifier(control, subDocument, this.manipulator, layout, interval)).modify();
        }

        public applyLowerCase(control: IRichEditControl, subDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval): HistoryItemIntervalState<HistoryItemTextBufferStateObject> {
            return (new LowerCaseModifier(control, subDocument, this.manipulator, layout, interval)).modify();
        }

        public applyCapitalizeEachWordCase(control: IRichEditControl, subDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval): HistoryItemIntervalState<HistoryItemTextBufferStateObject> {
            return (new CapitalizeEachWordCaseModifier(control, subDocument, this.manipulator, layout, interval)).modify();
        }

        public applyToggleCase(control: IRichEditControl, subDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval): HistoryItemIntervalState<HistoryItemTextBufferStateObject> {
            return (new ToggleCaseModifier(control, subDocument, this.manipulator, layout, interval)).modify();
        }

        public applySentenceCase(control: IRichEditControl, subDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval): HistoryItemIntervalState<HistoryItemTextBufferStateObject> {
            return (new SentenceCaseModifier(control, subDocument, this.manipulator, layout, interval)).modify();
        }

        public applyBufferState(subDocument: SubDocument, oldState: HistoryItemIntervalState<HistoryItemTextBufferStateObject>) {
            var chunks: Chunk[] = subDocument.chunks;
            for (var i = 0, stateValue: HistoryItemTextBufferStateObject; stateValue = oldState.objects[i]; i++) {
                var oldText: string = stateValue.value;
                var oldTextPosition: number = stateValue.interval.start;

                var chunkIndex: number = Utils.normedBinaryIndexOf(chunks, (c: Chunk) => c.startLogPosition.value - oldTextPosition);
                for (var chunk: Chunk; oldText.length > 0 && (chunk = chunks[chunkIndex]); chunkIndex++) {
                    var currPosForInsertInThisChunk: number = oldTextPosition - chunk.startLogPosition.value;
                    var chunkTextBefore: string = chunk.textBuffer.substr(0, currPosForInsertInThisChunk);
                    var chunkTextAfter: string = chunk.textBuffer.substr(currPosForInsertInThisChunk + oldText.length);
                    var lengthInsertedText: number = chunk.textBuffer.length - currPosForInsertInThisChunk - chunkTextAfter.length;

                    chunk.textBuffer = [chunkTextBefore, oldText.substr(0, lengthInsertedText), chunkTextAfter].join("");

                    oldTextPosition += lengthInsertedText;
                    oldText = oldText.substr(lengthInsertedText);
                }
            }
            if (!oldState.isEmpty())
                this.manipulator.dispatcher.notifyTextBufferChanged(oldState, subDocument);
        }
    }

    class TextCaseModifierBase {
        private control: IRichEditControl;
        private layout: DocumentLayout;
        private subDocument: SubDocument;
        private dispatcher: ModelManipulator;

        public interval: FixedInterval;
        public newState: HistoryItemIntervalState<HistoryItemTextBufferStateObject>;
        public oldState: HistoryItemIntervalState<HistoryItemTextBufferStateObject>;

        constructor(control: IRichEditControl, subDocument: SubDocument, dispatcher: ModelManipulator, layout: DocumentLayout, interval: FixedInterval) {
            this.control = control;
            this.subDocument = subDocument;
            this.layout = layout;
            this.interval = interval;
            this.dispatcher = dispatcher;
        }

        public modify(): HistoryItemIntervalState<HistoryItemTextBufferStateObject> {
            this.newState = new HistoryItemIntervalState<HistoryItemTextBufferStateObject>();
            this.oldState = new HistoryItemIntervalState<HistoryItemTextBufferStateObject>();

            var sentences: Sentence[] = SentenceStructureBuilder.getBuilder(this.control, this.subDocument, this.layout, this.interval, true).sentences;
            for (var sentenceIndex: number = 0, sentence: Sentence; sentence = sentences[sentenceIndex]; sentenceIndex++)
                this.modifyCore(sentence);

            this.dispatcher.textCaseManipulator.applyBufferState(this.subDocument, this.newState);

            if (!this.newState.isEmpty())
                this.dispatcher.dispatcher.notifyTextBufferChanged(this.newState, this.subDocument);
            return this.oldState;
        }

        public modifyCore(sentence: Sentence) {
            throw new Error(Errors.NotImplemented);
        }
    }

    class TextCaseSimpleModifier extends TextCaseModifierBase {
        public modifyCore(sentence: Sentence) {
            for (var wordIndex: number = 0, wordInfo: SentenceWord; wordInfo = sentence.words[wordIndex]; wordIndex++) {
                for (var wordPartIndex: number = 0, wordPart: WordPart; wordPart = wordInfo.parts[wordPartIndex]; wordPartIndex++) {
                    if (wordPart.position < this.interval.start)
                        continue;

                    if (wordPart.position >= this.interval.end())
                        return;

                    switch (wordPart.type) {
                        case LayoutBoxType.Text:
                            this.oldState.register(new HistoryItemTextBufferStateObject(wordPart.position, wordPart.text));
                            var newText: string = this.applyModifier(wordIndex, wordPartIndex, wordPart.text);
                            this.newState.register(new HistoryItemTextBufferStateObject(wordPart.position, newText));
                            break;
                    }
                }
            }
        }

        public applyModifier(wordIndex: number, wordPartIndex: number, text: string): string {
            throw new Error(Errors.NotImplemented);
        }
    }

    class LowerCaseModifier extends TextCaseSimpleModifier {
        public applyModifier(wordIndex: number, wordPartIndex: number, text: string): string {
            return text.toLowerCase();
        }
    }

    class UpperCaseModifier extends TextCaseSimpleModifier {
        public applyModifier(wordIndex: number, wordPartIndex: number, text: string): string {
            return text.toUpperCase();
        }
    }

    class CapitalizeEachWordCaseModifier extends TextCaseSimpleModifier {
        public applyModifier(wordIndex: number, wordPartIndex: number, text: string): string {
            return wordPartIndex == 0 ? text[0].toUpperCase() + text.substr(1).toLowerCase() : text.toLowerCase();
        }
    }

    class ToggleCaseModifier extends TextCaseSimpleModifier {
        public applyModifier(wordIndex: number, wordPartIndex: number, text: string): string {
            var result: string = "";
            for (var i: number = 0; i < text.length; i++) {
                var char: string = text[i];
                var lowerChar: string = char.toLowerCase();
                result += lowerChar === char ? char.toUpperCase() : lowerChar;
            }
            return result;
        }
    }

    class SentenceCaseModifier extends TextCaseSimpleModifier {
        public applyModifier(wordIndex: number, wordPartIndex: number, text: string): string {
            return wordIndex == 0 && wordPartIndex == 0 ? text[0].toUpperCase() + text.substr(1).toLowerCase() : text.toLowerCase();
        }
    }
}  