module __aspxRichEdit {
    export class PieceTableNumberingListCountersManager {
        calculators: { [abstractNumberingListIndex: number]: NumberingListCountersCalculator } = {};
        subDocument: SubDocument;
        private currentParagraphIndex: number = -1;

        constructor(subDocument: SubDocument) {
            this.subDocument = subDocument;
        }

        calculateCounters(paragraphIndex: number): number[] {
            if(paragraphIndex < this.currentParagraphIndex)
                this.reset();
            this.currentParagraphIndex = paragraphIndex;
            var paragraph = this.subDocument.paragraphs[paragraphIndex];
            var abstractNumberingListIndex = paragraph.getAbstractNumberingListIndex();
            var calculator = this.getCalculator(abstractNumberingListIndex);
            return calculator.calculateNextCounters(paragraph);
        }

        private getCalculator(abstractNumberingListIndex: number): NumberingListCountersCalculator {
            if(!this.calculators[abstractNumberingListIndex])
                this.calculators[abstractNumberingListIndex] = new NumberingListCountersCalculator(this.subDocument.documentModel.abstractNumberingLists[abstractNumberingListIndex]);
            return this.calculators[abstractNumberingListIndex];
        }

        reset() {
            this.calculators = {};
            this.currentParagraphIndex = -1;
        }
    }
} 