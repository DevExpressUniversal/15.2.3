/// <reference path="../../layout/documentlayout.ts" />
module __aspxRichEdit {
    // SYMBOLS
    // 1) Text
    // 2) Dash
    // 3) Space
    // 4) Tab
    // 5) Picture
    // BREAKS ( hyphenated indicated that it may take for this symbol on the same line )
    // 6) Section Break - finishRow*
    // 7) Line Break - finishRow*
    // 8) Page Break - Paragraph Mark, Section Break
    // 9) Column Break - finishRow*
    // 10) Paragraph Mark - Section Break
    // That is, one line at most can be completed at Page break -> Paragraph Mark -> Section Break

    export class RowBaseFormatterState {
        rowFormatter: TextRowFormatter;

        public constructor(rowFormatter: TextRowFormatter) {
            this.rowFormatter = rowFormatter;
        }

        protected addTextBoxCustom(box: LayoutBox, nextState: TextRowFormatterState, startNewWord: boolean, addByChar: boolean) {
            if (this.rowFormatter.isPossibleAddFullBox(box)) {
                if(startNewWord)
                    this.rowFormatter.startNewWord();
                this.rowFormatter.addBox(box);
                this.rowFormatter.setState(nextState);
            }
            else {
                if (addByChar || this.rowFormatter.row.isEmpty()) {
                    if (this.rowFormatter.addBoxByChars(box))
                        this.rowFormatter.setState(TextRowFormatterState.Base);
                    else {
                        this.rowFormatter.resetToBoxStart();
                        this.rowFormatter.finishRow();
                    }
                }
                else {
                    this.rowFormatter.resetToBoxStart();
                    this.rowFormatter.finishRow();
                }
            }
        }

        protected endRow() {
            this.rowFormatter.resetToBoxStart();
            this.rowFormatter.finishRow();
        }

        // TEXT
        addTextBox(box: LayoutBox) {
            this.addTextBoxCustom(box, TextRowFormatterState.EndedWithText, true, false); // always start new word exclude state onlyText
        }
        addDashBox(box: LayoutBox) {
            this.addTextBoxCustom(box, TextRowFormatterState.Base, false, true); // try add some dashes
        }
        addPictureBox(box: LayoutBox) {
            this.addTextBoxCustom(box, TextRowFormatterState.Base, false, false);
        }
        addSpaceBox(box: LayoutBox) {
            this.rowFormatter.addBox(box);
            this.rowFormatter.setState(TextRowFormatterState.Base);
        }
        addTabulationBox(box: LayoutBox) {
            if (this.rowFormatter.addTabBox(box))
                this.rowFormatter.setState(TextRowFormatterState.Base);
            else
                this.endRow();
        }
        // BREAKS
        addSectionBox(box: LayoutBox) {
            this.rowFormatter.addBox(box);
            this.rowFormatter.row.flags |= LayoutRowStateFlags.ParagraphEnd | LayoutRowStateFlags.SectionEnd;
            this.rowFormatter.finishRow();
        }
        addLineBreakBox(box: LayoutBox) {
            this.rowFormatter.addBox(box);
            this.rowFormatter.finishRow();
        }
        addPageBreakBox(box: LayoutBox) {
            this.rowFormatter.addBox(box);
            this.rowFormatter.row.flags |= LayoutRowStateFlags.PageEnd;
            this.rowFormatter.setState(TextRowFormatterState.EndedWithPageBreak);
        }
        addColumnBreakBox(box: LayoutBox) {
            this.rowFormatter.addBox(box);
            this.rowFormatter.row.flags |= LayoutRowStateFlags.ColumnEnd;
            this.rowFormatter.finishRow();
        }
        addParagraphBox(box: LayoutBox) {
            this.rowFormatter.addBox(box);
            this.rowFormatter.row.flags |= LayoutRowStateFlags.ParagraphEnd;
            this.rowFormatter.setState(TextRowFormatterState.EndedWithParagraphMark);
        }
    }

    export class RowEndedWithParagraphMarkFormatterState extends RowBaseFormatterState {
        constructor(rowFormatter: TextRowFormatter) {
            super(rowFormatter);
        }

        addTextBox(box: LayoutBox) {
            this.endRow();
        }

        addDashBox(box: LayoutBox) {
            this.endRow();
        }

        addPictureBox(box: LayoutBox) {
            this.endRow();
        }

        addSpaceBox(box: LayoutBox) {
            this.endRow();
        }

        addTabulationBox(box: LayoutBox) {
            this.endRow();
        }

        addLineBreakBox(box: LayoutBox) {
            this.endRow();
        }

        addPageBreakBox(box: LayoutBox) {
            this.endRow();
        }

        addColumnBreakBox(box: LayoutBox) {
            this.endRow();
        }

        addParagraphBox(box: LayoutBox) {
            this.endRow();
        }
    }

    export class RowEndedWithPageBreakState extends RowEndedWithParagraphMarkFormatterState {
        constructor(rowFormatter: TextRowFormatter) {
            super(rowFormatter);
        }

        addParagraphBox(box: LayoutBox) {
            this.rowFormatter.addBox(box);
            this.rowFormatter.row.flags |= LayoutRowStateFlags.ParagraphEnd;
            this.rowFormatter.setState(TextRowFormatterState.EndedWithParagraphMark);
        }
    }

    export class RowWithTextOnlyFormatterState extends RowBaseFormatterState {
        addTextBox(box: LayoutBox): void {
            this.addTextBoxCustom(box, TextRowFormatterState.WithTextOnly, false, true);
        }
    }

    export class RowEndedWithTextFormatterState extends RowBaseFormatterState {
        addTextBox(box: LayoutBox) {
            if (this.rowFormatter.isPossibleAddFullBox(box))
                this.rowFormatter.addBox(box);
            else {
                this.rowFormatter.resetToWordStart();
                this.rowFormatter.finishRow();
            }
        }
    }
}