module __aspxRichEdit {
    export class DialogTabsCommand extends ShowDialogCommandBase {
        createParameters(): DialogTabsParameters {
            var parameters: DialogTabsParameters = new DialogTabsParameters();
            parameters.defaultTabStop = this.control.model.defaultTabWidth;
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(this.control.selection.intervals);
            if(this.paragraphsHasEqualTabProperties(paragraphIndices))
                parameters.tabProperties = this.control.model.activeSubDocument.paragraphs[paragraphIndices[0]].tabs.clone();
            else
                parameters.tabProperties = new TabProperties();
            return parameters;
        }
        applyParameters(state: SimpleCommandState, newParams: DialogTabsParameters) {
            var initParams: DialogTabsParameters = <DialogTabsParameters>this.initParams;
            var modelManipulator: ModelManipulator = this.control.modelManipulator;

            this.control.history.beginTransaction();
            if (newParams.defaultTabStop && newParams.defaultTabStop !== initParams.defaultTabStop)
                this.control.history.addAndRedo(new DocumentDefaultTabWidthHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, newParams.defaultTabStop));

            this.deleteAllTabs();
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(this.control.selection.intervals);
            for(let i = 0, tabInfo: TabInfo; tabInfo = newParams.tabProperties.tabsInfo[i]; i++) {
                for(let j = paragraphIndices.length - 1; j >= 0; j--) {
                    let paragraphIndex = paragraphIndices[j];
                    let paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
                    this.control.history.addAndRedo(new InsertTabToParagraphHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, paragraph.getInterval(), tabInfo));
                }
            }
            this.control.history.endTransaction();
        }
        deleteAllTabs() {//TODO refactor
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(this.control.selection.intervals);
            for(let i = paragraphIndices.length - 1; i >= 0; i--) {
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndices[i]];
                var interval = paragraph.getInterval();
                while(paragraph.tabs.tabsInfo[0])
                    this.control.history.addAndRedo(new DeleteTabAtParagraphHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, paragraph.tabs.tabsInfo[0]));
            }
        }
        paragraphsHasEqualTabProperties(paragraphIndices: number[]) {
            var firstParagraph = this.control.model.activeSubDocument.paragraphs[paragraphIndices[0]];
            for(let i = paragraphIndices.length - 1; i > 0; i--) {
                let paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndices[i]];
                if(!firstParagraph.tabs.equals(paragraph.tabs))
                    return false;
            }
            return true;
        }
        getDialogName() {
            return "Tabs";
        }
    }

    export class DialogTabsParameters extends DialogParametersBase {
        defaultTabStop: number;
        tabProperties: TabProperties;

        getNewInstance(): DialogParametersBase {
            return new DialogTabsParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
            if (this.defaultTabStop) this.defaultTabStop = converterFunc(this.defaultTabStop);
        }

        copyFrom(obj: DialogTabsParameters) {
            this.defaultTabStop = obj.defaultTabStop;
            this.tabProperties = obj.tabProperties.clone();
        }
    }
}