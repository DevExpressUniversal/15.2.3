module __aspxRichEdit {
    export class NumberingListCountersCalculator {
        list: AbstractNumberingList;
        counters: number[] = [];
        usedListIndecies: { [key: number]: boolean } = {};
        currentParagraphIndex: number = -1;

        constructor(list: AbstractNumberingList) {
            this.list = list;
            for(var i = 0, listLevel: IListLevel; listLevel = this.list.levels[i]; i++) {
                this.counters.push(listLevel.getListLevelProperties().start - 1);
            }
        }

        calculateCounters(paragraph: Paragraph): number[]{
            this.currentParagraphIndex = -1;
            return this.calculateNextCounters(paragraph);
        }
        calculateNextCounters(paragraph: Paragraph): number[]{
            var paragraphs = paragraph.subDocument.paragraphs;
            if(paragraphs[this.currentParagraphIndex] !== paragraph) {
                var abstractNumberingList = paragraph.getAbstractNumberingList();
                for(var i = this.currentParagraphIndex + 1, currentParagraph: Paragraph; currentParagraph = paragraph.subDocument.paragraphs[i]; i++) {
                    this.currentParagraphIndex++;
                    if(this.shouldAdvanceListLevelCounters(currentParagraph, abstractNumberingList))
                        this.advanceListLevelCounters(currentParagraph, currentParagraph.getListLevelIndex());
                    if(currentParagraph === paragraph)
                        break;
                }
            }
            return this.getActualRangeCounters(paragraph.getListLevelIndex());
        }
        getLastCounters(paragraph: Paragraph): number[] {
            return this.getActualRangeCounters(paragraph.getListLevelIndex());
        }

        private shouldAdvanceListLevelCounters(paragraph: Paragraph, abstractNumberingList: AbstractNumberingList): boolean {
            return paragraph.getAbstractNumberingList() === abstractNumberingList;
        }

        private advanceListLevelCounters(paragraph: Paragraph, listLevelIndex: number) {
            var numberingListIndex = paragraph.getNumberingListIndex();
            var numberingList = paragraph.subDocument.documentModel.numberingLists[numberingListIndex];
            var level: IOverrideListLevel = <IOverrideListLevel>numberingList.levels[listLevelIndex];
            if(level.overrideStart && !this.usedListIndecies[numberingListIndex]) {
                this.usedListIndecies[numberingListIndex] = true;
                this.counters[listLevelIndex] = level.getNewStart();
            }
            else
                this.counters[listLevelIndex]++;
            this.advanceSkippedLevelCounters(listLevelIndex);
            this.restartNextLevelCounters(listLevelIndex);
        }

        private advanceSkippedLevelCounters(listLevelIndex: number) {
            for(var i = 0; i < listLevelIndex; i++) {
                var listLevel = this.list.levels[i];
                if(this.counters[i] == listLevel.getListLevelProperties().start - 1)
                    this.counters[i]++;
            }
        }
        private restartNextLevelCounters(listLevelIndex: number) {
            var restartedLevels: boolean[] = [];
            restartedLevels[listLevelIndex] = true;
            var countersLength = this.counters.length;
            for(var i = listLevelIndex + 1; i < countersLength; i++) {
                var listLevelProperties = this.list.levels[i].getListLevelProperties();
                if(!listLevelProperties.suppressRestart) {
                    var restartLevel = i - listLevelProperties.relativeRestartLevel - 1;
                    if(restartLevel >= 0 && restartLevel < countersLength && restartedLevels[restartLevel]) {
                        this.counters[i] = listLevelProperties.start - 1;
                        restartedLevels[i] = true;
                    }
                }
            }
        }
        private getActualRangeCounters(listLevelIndex: number) {
            var rangeCounters: number[] = [];
            for (var i = 0; i <= listLevelIndex; i++)
                rangeCounters[i] = this.counters[i];
            return rangeCounters;
        }
    }
} 