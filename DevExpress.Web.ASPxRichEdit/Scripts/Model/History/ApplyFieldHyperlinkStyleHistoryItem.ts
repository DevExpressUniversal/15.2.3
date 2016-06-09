module __aspxRichEdit {
    export class ApplyFieldHyperlinkStyleHistoryItem extends IntervalBasedHistoryItem {
        private static mask: number = CharacterPropertiesMask.UseAll & ~(CharacterPropertiesMask.UseFontUnderlineType | CharacterPropertiesMask.UseForeColor);
        private historyItems: HistoryItem[] = [];

        constructor(modelManipulator: ModelManipulator, boundSubDocument: SubDocument, interval: FixedInterval) {
            super(modelManipulator, boundSubDocument, interval);
        }

        public redo() {
            if (ApplyFieldHyperlinkStyleHistoryItem.mask === undefined)
                throw new Error("CharacterPropertiesMask defined later than that class");

            if (this.historyItems.length > 0) {
                for (var i: number = 0, histItem: HistoryItem; histItem = this.historyItems[i]; i++)
                    histItem.redo();
                return;
            }

            var charHyperlinkStyle: CharacterStyle = this.modelManipulator.model.getCharacterStyleByName("Hyperlink");
            var intervalEnd: number = this.interval.end();

            this.boundSubDocument.splitRun(this.interval.start);
            this.boundSubDocument.splitRun(intervalEnd);

            var modelIterator: ModelIterator = new ModelIterator(this.boundSubDocument, false);
            modelIterator.setPosition(this.interval.start);
            var histItem: HistoryItem;
            do {
                var run: TextRun = modelIterator.run;
                var runMergedProperties: CharacterProperties = run.getCharacterMergedProperies();
                var runInterval: FixedInterval = new FixedInterval(modelIterator.chunk.startLogPosition.value + run.startOffset, run.length);

                histItem = new ApplyCharacterStyleHistoryItem(this.modelManipulator, this.boundSubDocument, this.interval, charHyperlinkStyle);
                histItem.redo();
                this.historyItems.push(histItem);

                for (var i: number = 0, histInfo: { historyItemType: any; propertyName: string }; histInfo = PropertiesWhatNeedSetWhenCreateHyperlinkField.info[i]; i++) {
                    histItem = new histInfo.historyItemType(this.modelManipulator, this.boundSubDocument, runInterval, runMergedProperties[histInfo.propertyName], true);
                    histItem.redo();
                    this.historyItems.push(histItem);
                }
                histItem = new FontUseValueHistoryItem(this.modelManipulator, this.boundSubDocument, runInterval, ApplyFieldHyperlinkStyleHistoryItem.mask);
                histItem.redo();
                this.historyItems.push(histItem);

                run.hasCharacterMergedProperies();
            } while(runInterval.start < intervalEnd && modelIterator.moveToNextRun())
        }

        public undo() {
            for (var i: number = this.historyItems.length - 1, histItem: HistoryItem; histItem = this.historyItems[i]; i--)
                histItem.undo();

            this.boundSubDocument.splitRun(this.interval.start);
            this.boundSubDocument.splitRun(this.interval.end());

            var modelIterator: ModelIterator = new ModelIterator(this.boundSubDocument, false);
            modelIterator.setPosition(this.interval.start);
            do {
                modelIterator.run.hasCharacterMergedProperies();
            } while(modelIterator.chunk.startLogPosition.value + modelIterator.run.startOffset < this.interval.end() && modelIterator.moveToNextRun())
        }
    }
} 