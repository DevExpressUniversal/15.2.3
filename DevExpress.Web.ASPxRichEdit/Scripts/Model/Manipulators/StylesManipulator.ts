module __aspxRichEdit {
    export class StylesManipulator {
        manipulator: ModelManipulator;

        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        setLinkStyle(characterStyle: CharacterStyle, paragraphStyle: ParagraphStyle): void {
            this.manipulator.model.stylesManager.addCharacterStyle(characterStyle);
            this.manipulator.model.stylesManager.registerLink(characterStyle, paragraphStyle);
            this.manipulator.dispatcher.notifyCreateStyleLink(paragraphStyle.styleName);
        }

        restoreLinkStyle(characterStyle: CharacterStyle, paragraphStyle: ParagraphStyle): void {
            this.manipulator.model.stylesManager.removeLastStyle();
            this.manipulator.model.stylesManager.unregisterLink(characterStyle, paragraphStyle);
            this.manipulator.dispatcher.notifyDeleteStyleLink(paragraphStyle.styleName);
        }

        setCharacterStyle(subDocument: SubDocument, interval: FixedInterval, style: CharacterStyle): HistoryItemIntervalState<HistoryItemIntervalStyleStateObject> {
            var oldState = new HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>();
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.characterStyle))
                return oldState;
            var newState = new HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>();
            var iterator = subDocument.getRunIterator(interval);
            while(iterator.moveNext()) {
                oldState.register(new HistoryItemIntervalStyleStateObject(iterator.currentInterval(), iterator.currentRun.characterStyle));
                newState.register(new HistoryItemIntervalStyleStateObject(iterator.currentInterval(), style));
                iterator.currentRun.characterStyle = style;
                iterator.currentRun.onCharacterPropertiesChanged();
            }
            this.manipulator.dispatcher.notifyCharacterStyleApplied(newState, this.manipulator.model.activeSubDocument);
            return oldState;
        }

        restoreCharacterStyle(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>) {
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.characterStyle))
                return;
            for(var stateValue: HistoryItemIntervalStyleStateObject, i = 0; stateValue = state.objects[i]; i++) {
                var iterator = subDocument.getRunIterator(stateValue.interval);
                while(iterator.moveNext()) {
                    iterator.currentRun.characterStyle = <CharacterStyle>stateValue.value;
                    iterator.currentRun.onCharacterPropertiesChanged();
                }
            }
            this.manipulator.dispatcher.notifyCharacterStyleApplied(state, this.manipulator.model.activeSubDocument);
        }

        setParagraphStyle(subDocument: SubDocument, interval: FixedInterval, style: ParagraphStyle): HistoryItemIntervalState<HistoryItemIntervalStyleStateObject> {
            var oldState = new HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>();
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.paragraphStyle))
                return oldState;
            var newState = new HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>();
            var paragraphs = subDocument.getParagraphsByInterval(interval);
            for(var paragraph: Paragraph, i = 0; paragraph = paragraphs[i]; i++) {
                var paragraphInterval = new FixedInterval(paragraph.startLogPosition.value, paragraph.length);
                oldState.register(new HistoryItemIntervalStyleStateObject(paragraphInterval, paragraph.paragraphStyle));
                newState.register(new HistoryItemIntervalStyleStateObject(paragraphInterval, style));
                paragraph.paragraphStyle = style;
                paragraph.onParagraphPropertiesChanged();
                this.resetMergedCharacterProperties(subDocument, paragraphInterval);
            }
            this.manipulator.dispatcher.notifyParagraphStyleApplied(newState, this.manipulator.model.activeSubDocument);
            return oldState;
        }

        restoreParagraphStyle(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>) {
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.paragraphStyle))
                return;
            for(var stateValue: HistoryItemIntervalStyleStateObject, i = 0; stateValue = state.objects[i]; i++) {
                var paragraphs = subDocument.getParagraphsByInterval(stateValue.interval);
                for(var j = 0, paragraph: Paragraph; paragraph = paragraphs[j]; j++) {
                    paragraph.paragraphStyle = <ParagraphStyle>stateValue.value;
                    paragraph.onParagraphPropertiesChanged();
                    this.resetMergedCharacterProperties(subDocument, new FixedInterval(paragraph.startLogPosition.value, paragraph.length));
                }
            }
            this.manipulator.dispatcher.notifyParagraphStyleApplied(state, this.manipulator.model.activeSubDocument);
        }

        private resetMergedCharacterProperties(subDocument: SubDocument, interval: FixedInterval) {
            var runs = subDocument.getRunsByInterval(interval);
            for(var i = 0, run: TextRun; run = runs[i]; i++)
                run.onCharacterPropertiesChanged();
        }

        public static applyCharacterStyle(history: IHistory, modelManipulator: ModelManipulator, subDocument: SubDocument, interval: FixedInterval, style: CharacterStyle) {
            history.addAndRedo(new FontUseValueHistoryItem(modelManipulator, subDocument, interval, 0));
            history.addAndRedo(new ApplyCharacterStyleHistoryItem(modelManipulator, subDocument, interval, style));
        }
    }
}  