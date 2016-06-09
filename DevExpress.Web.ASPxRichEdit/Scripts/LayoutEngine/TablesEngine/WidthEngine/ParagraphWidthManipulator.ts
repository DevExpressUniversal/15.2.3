module __aspxRichEdit {
    export class ParagraphWidthManipulator {
        public static calculateTableParagraphsWidth(boxIterator: ITextBoxIterator, paragraphs: Paragraph[], table: Table) {
            const tableFirstParStartPosition: number = table.getStartPosition();
            const tableLastParagraphEndPosition: number = table.getEndPosition() - 1;
            const startParagraphIndex: number = Utils.normedBinaryIndexOf(paragraphs, (p: Paragraph) => p.startLogPosition.value - tableFirstParStartPosition);
            const endParagraphIndex: number = Utils.normedBinaryIndexOf(paragraphs, (p: Paragraph) => p.startLogPosition.value - tableLastParagraphEndPosition);
            boxIterator.setPosition(tableFirstParStartPosition, false);
            for (let parIndex: number = startParagraphIndex; parIndex <= endParagraphIndex; parIndex++) {
                const currPar: Paragraph = paragraphs[parIndex];
                if (currPar.layoutWidthBounds)
                    continue;
                currPar.layoutWidthBounds = ParagraphWidthManipulator.calculateParagraph(boxIterator, currPar); 
            }
        }

        private static calculateParagraph(boxIterator: ITextBoxIterator, paragraph: Paragraph): WidthBounds {
            let maxWidth: number = 0;
            let minWidth: number = 0;
            let currLineWidth: number = 0;
            let wordWidth: number = 0;

            const paragraphs: Paragraph[] = boxIterator.subDocument.paragraphs;

            while (paragraph == paragraphs[boxIterator.getParagraphIndex()]) { // after table ALWAYS placed paragraphMark
                let currBox: LayoutBox = boxIterator.getNextBox();
                if (currBox.getType() == LayoutBoxType.LineBreak) {
                    maxWidth = Math.max(currLineWidth, maxWidth);
                    minWidth = Math.max(wordWidth, minWidth);
                    currLineWidth = 0;
                    wordWidth = 0;
                }
                else {
                    currLineWidth += currBox.width;
                    if (currBox.isWhitespace()) {
                        minWidth = Math.max(wordWidth, minWidth);
                        wordWidth = 0;
                    }
                    else
                        wordWidth += currBox.width;
                }
            }
            return new WidthBounds(minWidth, maxWidth);
        }
    }
}