module __aspxRichEdit {
    export class ApplyStyleCommand extends CommandBase<IntervalCommandState> {
        getState(): IntervalCommandState {
            var interval: FixedInterval = this.control.selection.getLastSelectedInterval().clone();

            var commonParagraphStyle: ParagraphStyle;
            var commonCharacterStyle: CharacterStyle;

            if (interval.length === 0)
                commonCharacterStyle = this.control.inputPosition.getCharacterStyle();

            if (!commonCharacterStyle || commonCharacterStyle.isDefault) {
                var runs: TextRun[] = this.control.model.activeSubDocument.getRunsByInterval(interval);
                var commonParagraphStyle: ParagraphStyle = runs[0].paragraph.paragraphStyle;
                var commonCharacterStyle: CharacterStyle = runs[0].characterStyle;

                for (var i = 1, run: TextRun; run = runs[i]; i++) {
                    if (commonCharacterStyle && run.characterStyle !== commonCharacterStyle)
                        commonCharacterStyle = null;
                    if (commonParagraphStyle && run.paragraph.paragraphStyle !== commonParagraphStyle)
                        commonParagraphStyle = null;
                    if (!commonParagraphStyle && !commonCharacterStyle)
                        break;
                }
            }

            var styleNameWithPrefix: string = "";
            if(commonCharacterStyle && commonCharacterStyle.linkedStyle)
                styleNameWithPrefix = "PS-" + commonCharacterStyle.linkedStyle.styleName;
            else if(commonCharacterStyle && !commonCharacterStyle.isDefault)
                styleNameWithPrefix = "CS-" + commonCharacterStyle.styleName;
            else if(commonParagraphStyle && !commonParagraphStyle.isDefault)
                styleNameWithPrefix = "PS-" + commonParagraphStyle.styleName;
            else
                styleNameWithPrefix = "PS-" + this.control.model.getDefaultParagraphStyle().styleName;

            var state = new IntervalCommandState(this.isEnabled(), interval, styleNameWithPrefix);
            var items = [[], []];
            for(let cs of this.control.model.characterStyles) {
                if(!cs.deleted && !cs.hidden && !cs.semihidden)
                    items[0].push({ value: "CS-" + cs.styleName, text: cs.styleName });
            }
            for(let ps of this.control.model.paragraphStyles) {
                if(!ps.deleted && !ps.hidden && !ps.semihidden)
                    items[1].push({ value: "PS-" + ps.styleName, text: ps.styleName });
            }
            state.items = items;
            return state;
        }
        executeCore(state: IntervalCommandState, parameter: string): boolean {
            var prefix: string = parameter.substr(0, 2);
            var styleName: string = parameter.substr(3);
            var interval: FixedInterval = state.interval.clone();

            var executed: boolean = true;
            this.control.history.beginTransaction();
            switch (prefix) {
                case "CS":
                    if (interval.length == 0)
                        interval = this.control.model.activeSubDocument.getWholeWordInterval(interval.start);
                    if (interval.length == 0) {
                        this.control.inputPosition.setCharacterStyle(this.control.model.getCharacterStyleByName(styleName));
                        executed = false;
                    }
                    else
                        this.applyCharacterStyle(interval, this.control.model.getCharacterStyleByName(styleName));
                    break;
                case "PS":
                    this.applyParagraphStyle(interval, this.control.model.getParagraphStyleByName(styleName));
                    break;
                default: 
                    throw new Error("Parameter must have next structure: (PS/CS)-StyleName");
            }
            this.control.history.endTransaction();
            return executed;
        }
        applyCharacterStyle(interval: FixedInterval, style: CharacterStyle) {
            if(ControlOptions.isEnabled(this.control.options.characterStyle)) 
                StylesManipulator.applyCharacterStyle(this.control.history, this.control.modelManipulator, this.control.model.activeSubDocument, interval, style);
        }
        applyParagraphStyle(interval: FixedInterval, style: ParagraphStyle) {
            var count = this.calculateAffectedParagraphCount(interval);
            if(count > 0 && ControlOptions.isEnabled(this.control.options.paragraphStyle)) {
                var paragraphIndex = Utils.normedBinaryIndexOf(this.control.model.activeSubDocument.paragraphs, p => p.startLogPosition.value - interval.start);
                for(var i = 0; i < count; i++) {
                    var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex + i];
                    var paragraphInterval = new FixedInterval(paragraph.startLogPosition.value, paragraph.length);
                    var modelManipulator: ModelManipulator = this.control.modelManipulator;
                    this.control.history.addAndRedo(new ApplyParagraphStyleHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, paragraphInterval, style));
                    this.control.history.addAndRedo(new ParagraphUseValueHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, paragraphInterval, 0));
                    this.control.history.addAndRedo(new FontUseValueHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, paragraphInterval, 0));
                }
            }
            else
                this.applyParagraphLinkedStyle(interval, style);
        }
        applyParagraphLinkedStyle(interval: FixedInterval, style: ParagraphStyle) {
            if(ControlOptions.isEnabled(this.control.options.characterStyle)) {
                if(!style.linkedStyle)
                    this.createCharacterStyle(style);
                this.applyCharacterStyle(interval, style.linkedStyle);
            }
        }
        private createCharacterStyle(paragraphStyle: ParagraphStyle) {
            var style = new CharacterStyle(paragraphStyle.styleName + " Char", paragraphStyle.localizedName + " Char", false, false, false, false, paragraphStyle.maskedCharacterProperties);
            this.control.history.addAndRedo(new CreateStyleLinkHistoryItem(this.control.modelManipulator, this.control.modelManipulator.model.activeSubDocument, style, paragraphStyle));
        }
        calculateAffectedParagraphCount(interval: FixedInterval): number {
            var paragraphs = this.control.model.activeSubDocument.getParagraphsByInterval(interval);
            if (paragraphs.length > 1)
                return paragraphs.length;
            var paragraph = paragraphs[0];
            var lastParagraphCharSelected: boolean = interval.length >= paragraph.length - 1;
            if (interval.start === paragraph.startLogPosition.value && lastParagraphCharSelected || interval.length === 0)
                return 1;
            return 0;
        }
        isEnabled(): boolean {
            return super.isEnabled() && (ControlOptions.isEnabled(this.control.options.characterStyle) || ControlOptions.isEnabled(this.control.options.paragraphStyle));
        }
    }
} 