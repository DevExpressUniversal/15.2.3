module __aspxRichEdit {
    export class ChangeCaseHistoryItemBase extends IntervalBasedHistoryItem {
        oldState: HistoryItemIntervalState<HistoryItemTextBufferStateObject>;
        control: IRichEditControl;
        layout: DocumentLayout;

        // here need control to format some pages
        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval, control: IRichEditControl) {
            super(modelManipulator, boundSubDocument, interval);
            this.control = control;
            this.layout = layout;
        }

        undo() {
            this.modelManipulator.textCaseManipulator.applyBufferState(this.boundSubDocument, this.oldState);
        }
    }

    export class UpperCaseHistoryItem extends ChangeCaseHistoryItemBase {
        redo() {
            this.oldState = this.modelManipulator.textCaseManipulator.applyUpperCase(this.control, this.boundSubDocument, this.layout, this.interval);
        }
    }

    export class LowerCaseHistoryItem extends ChangeCaseHistoryItemBase {
        redo() {
            this.oldState = this.modelManipulator.textCaseManipulator.applyLowerCase(this.control, this.boundSubDocument, this.layout, this.interval);
        }
    }

    export class CapitalizeEachWordCaseHistoryItem extends ChangeCaseHistoryItemBase {
        redo() {
            this.oldState = this.modelManipulator.textCaseManipulator.applyCapitalizeEachWordCase(this.control, this.boundSubDocument, this.layout, this.interval);
        }
    }

    export class ToggleCaseHistoryItem extends ChangeCaseHistoryItemBase {
        redo() {
            this.oldState = this.modelManipulator.textCaseManipulator.applyToggleCase(this.control, this.boundSubDocument, this.layout, this.interval);
        }
    }
    export class SentenceCaseHistoryItem extends ChangeCaseHistoryItemBase {
        redo() {
            this.oldState = this.modelManipulator.textCaseManipulator.applySentenceCase(this.control, this.boundSubDocument, this.layout, this.interval);
        }
    }
    
} 