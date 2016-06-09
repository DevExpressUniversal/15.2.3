module __aspxRichEdit {
    export enum RulerTabAction {
        None = 0,
        Insert = 1,
        Delete = 2
    }

    export class TabRulerCommandBase extends CommandBase<SimpleCommandState> {
        findTabByPosition(paragraph: Paragraph, position: number): TabInfo {
            var index = Utils.binaryIndexOf(paragraph.tabs.tabsInfo, (a: TabInfo) => a.position - position);
            return index > -1 ? paragraph.tabs.tabsInfo[index] : null;
        }
        executeCore(state: SimpleCommandState, params: any): boolean {
            var info: { paragraphs: Paragraph[]; intervals: FixedInterval[] } = Utils.getSelectedParagraphs(this.control.selection, this.control.model.activeSubDocument);
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var subDocument: SubDocument = this.control.modelManipulator.model.activeSubDocument;

            this.control.history.beginTransaction();
            this.executeHistoryItems(modelManipulator, subDocument, info.intervals[0], info.paragraphs[0], params);
            this.control.history.endTransaction();
            return true;
        }
        executeHistoryItems(modelManipulator: ModelManipulator, subDocument: SubDocument, interval: FixedInterval, firstParagraph: Paragraph, params: any): void {
        }
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.paragraphTabs);
        }
    }

    export class InsertTabToParagraphCommand extends TabRulerCommandBase {
        executeHistoryItems(modelManipulator: ModelManipulator, subDocument: SubDocument, interval: FixedInterval, firstParagraph: Paragraph, params: any): void {
            var obj = <{ position: number; align: TabAlign }>params;
            var tabInfo = new TabInfo(UnitConverter.pixelsToTwips(obj.position), obj.align, TabLeaderType.None, false, false);
            this.control.history.addAndRedo(new InsertTabToParagraphHistoryItem(modelManipulator, subDocument, interval, tabInfo));
        }
    }
    export class DeleteTabAtParagraphCommand extends TabRulerCommandBase {
        executeHistoryItems(modelManipulator: ModelManipulator, subDocument: SubDocument, interval: FixedInterval, firstParagraph: Paragraph, params: any): void {
            var tabInfo = this.findTabByPosition(firstParagraph, UnitConverter.pixelsToTwips(<number>params));
            this.control.history.addAndRedo(new DeleteTabAtParagraphHistoryItem(modelManipulator, subDocument, interval, tabInfo));
        }
    }
    export class MoveTabRulerInParagraphCommand extends TabRulerCommandBase {
        executeHistoryItems(modelManipulator: ModelManipulator, subDocument: SubDocument, interval: FixedInterval, firstParagraph: Paragraph, params: any): void {
            var position = <{ start: number; end: number }>params;

            var oldTabInfo: TabInfo = this.findTabByPosition(firstParagraph, UnitConverter.pixelsToTwips(position.start));
            var newTabInfo: TabInfo = oldTabInfo.clone();
            newTabInfo.position = UnitConverter.pixelsToTwips(position.end);

            this.control.history.addAndRedo(new DeleteTabAtParagraphHistoryItem(modelManipulator, subDocument, interval, oldTabInfo));
            this.control.history.addAndRedo(new InsertTabToParagraphHistoryItem(modelManipulator, subDocument, interval, newTabInfo));
        }
    }
}