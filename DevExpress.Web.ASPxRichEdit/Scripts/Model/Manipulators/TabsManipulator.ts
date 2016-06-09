module __aspxRichEdit {
    export class TabsManipulator {
        manipulator: ModelManipulator;

        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        public insertTabToParagraph(subDocument: SubDocument, interval: FixedInterval, tabInfo: TabInfo): HistoryItemIntervalState<HistoryItemTabStateObject> {
            const paragraphs: Paragraph[] = subDocument.getParagraphsByInterval(interval);
            const oldState: HistoryItemIntervalState<HistoryItemTabStateObject> = new HistoryItemIntervalState<HistoryItemTabStateObject>();
            const newState: HistoryItemIntervalState<HistoryItemTabStateObject> = new HistoryItemIntervalState<HistoryItemTabStateObject>();

            for (let paragraph of paragraphs) {
                if (this.insertTabInternal(paragraph.tabs, tabInfo.clone())) {
                    const parInterval: FixedInterval = paragraph.getInterval();
                    oldState.register(new HistoryItemTabStateObject(parInterval, tabInfo.clone()));
                    newState.register(new HistoryItemTabStateObject(parInterval, tabInfo.clone()));
                }
            }

            this.manipulator.dispatcher.notifyTabInserted(subDocument, newState);
            return oldState;
        }

        public deleteTabAtParagraph(subDocument: SubDocument, interval: FixedInterval, tabInfo: TabInfo): HistoryItemIntervalState<HistoryItemTabStateObject> {
            const paragraphs: Paragraph[] = subDocument.getParagraphsByInterval(interval);
            const oldState: HistoryItemIntervalState<HistoryItemTabStateObject> = new HistoryItemIntervalState<HistoryItemTabStateObject>();
            const newState: HistoryItemIntervalState<HistoryItemTabStateObject> = new HistoryItemIntervalState<HistoryItemTabStateObject>();

            for (let paragraph of paragraphs) {
                if(this.deleteTabInternal(paragraph.tabs, tabInfo)) {
                    const parInterval: FixedInterval = paragraph.getInterval();
                    oldState.register(new HistoryItemTabStateObject(parInterval, tabInfo.clone()));
                    newState.register(new HistoryItemTabStateObject(parInterval, tabInfo.clone()));
                    if (Utils.binaryIndexOf(paragraph.paragraphStyle.tabs.tabsInfo, (t: TabInfo) => t.position - tabInfo.position) > -1) {
                        tabInfo.deleted = true; // TODO!
                        this.insertTabInternal(paragraph.tabs, tabInfo.clone());
                    }
                }
            }

            this.manipulator.dispatcher.notifyTabDeleted(subDocument, newState);
            return oldState;
        }

        public restoreInsertedTabToParagraph(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemTabStateObject>): void {
            if (state.isEmpty())
                return;

            for (let stateObject of state.objects) {
                const tabInfo: TabInfo = <TabInfo>stateObject.value;
                const paragraphs: Paragraph[] = subDocument.getParagraphsByInterval(stateObject.interval);
                for (let paragraph of paragraphs)
                    this.deleteTabInternal(paragraph.tabs, tabInfo)
            }
            this.manipulator.dispatcher.notifyTabDeleted(subDocument, state);
        }

        public restoreDeletedTabAtParagraph(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemTabStateObject>): void {
            if (state.isEmpty())
                return;

            for (let stateObject of state.objects) {
                const tabInfo: TabInfo = <TabInfo>stateObject.value;
                const paragraphs: Paragraph[] = subDocument.getParagraphsByInterval(stateObject.interval);
                for (let paragraph of paragraphs)
                    this.insertTabInternal(paragraph.tabs, tabInfo.clone())
            }
            this.manipulator.dispatcher.notifyTabInserted(subDocument, state);
        }

        private deleteTabInternal(tabProps: TabProperties, tabInfo: TabInfo): boolean {
            const index: number = Utils.binaryIndexOf(tabProps.tabsInfo, (t: TabInfo) => t.position - tabInfo.position);
            if (index < 0)
                return false;
            tabProps.tabsInfo.splice(index, 1);
            return true;
        }

        private insertTabInternal(tabProps: TabProperties, tabInfo: TabInfo): boolean {
            const index: number = Utils.binaryIndexOf(tabProps.tabsInfo, (t: TabInfo) => t.position - tabInfo.position);
            if (index >= 0)
                return false;
            tabProps.tabsInfo.splice(Math.max(0, Utils.binaryIndexNormalizator(index)), 0, tabInfo);
            return true;
        }
    }
}  