module __aspxRichEdit {
    export class NumberingListIndexCalculator {
        subDocument: SubDocument;
        targetType: NumberingType;
        startIndex: number;

        constructor(subDocument: SubDocument, targetType: NumberingType, startIndex: number) {
            this.subDocument = subDocument;
            this.targetType = targetType;
            this.startIndex = startIndex;
        }

        getTargetListInfo(paragraphIndices: number[]): { listIndex: number; listlevelIndex: number } {
            var result: number;
            var prevTargetIndex = -1;
            if(this.startIndex === 1)
                return null;
            var targetParagraph: Paragraph;
            var startParagraphIndex = paragraphIndices[0];
            for(let i = startParagraphIndex - 1; i >= 0; i--) {
                if(this.hasSameParagraphType(i)) {
                    var targetParagraph = this.subDocument.paragraphs[i];
                    if(this.startIndex > 1) {
                        var listCounters = this.subDocument.documentModel.getRangeListCounters(targetParagraph);
                        if(listCounters.length !== 1 || listCounters[0] !== this.startIndex - 1)
                            return null;
                    }
                    break;
                }
            }
            if(!targetParagraph && this.hasSameParagraphType(startParagraphIndex + 1))
                targetParagraph = this.subDocument.paragraphs[startParagraphIndex + 1];
            if(targetParagraph) {
                return {
                    listIndex: targetParagraph.getNumberingListIndex(),
                    listlevelIndex: Math.max(0, targetParagraph.getListLevelIndex())
                }
            }
            return null;
        }

        private hasSameParagraphType(paragraphIndex: number): boolean {
            var paragraph = this.subDocument.paragraphs[paragraphIndex];
            return paragraph && paragraph.isInList() && paragraph.getNumberingList().getListType() === this.targetType;
        }
    }
} 