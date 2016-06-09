module __aspxRichEdit {
    export class RulerParagraphLeftIndentsCommandValue {
        constructor(intervals: FixedInterval[], hanging: number, firstLine: number) {
            this.hanging = hanging;
            this.firstLine = firstLine;
            this.intervals = intervals;
        }
        hanging: number; // distance to second and next rows
        firstLine: number; // distance to first line
        intervals: FixedInterval[];
    }

    export class RulerParagraphLeftIndentsCommand extends CommandBase<SimpleCommandState> {
        // this call not only in executeCore. else in Ruler.js in loadState
        getState(): SimpleCommandState {
            var info: { paragraphs: Paragraph[]; intervals: FixedInterval[] } = Utils.getSelectedParagraphs(this.control.selection, this.control.model.activeSubDocument);

            // get props from first paragraph in selection
            var parProps: ParagraphProperties = info.paragraphs[0].getParagraphMergedProperies();
            var leftIndent: number = UnitConverter.twipsToPixels(parProps.leftIndent);
            var fstLineIndent: number = UnitConverter.twipsToPixels(parProps.firstLineIndent);

            switch (parProps.firstLineIndentType) {
                case ParagraphFirstLineIndent.Indented:
                    return new SimpleCommandState(this.isEnabled(), new RulerParagraphLeftIndentsCommandValue(info.intervals, leftIndent, leftIndent + fstLineIndent));
                case ParagraphFirstLineIndent.None:
                    return new SimpleCommandState(this.isEnabled(), new RulerParagraphLeftIndentsCommandValue(info.intervals, leftIndent, leftIndent));
                case ParagraphFirstLineIndent.Hanging:
                    return new SimpleCommandState(this.isEnabled(), new RulerParagraphLeftIndentsCommandValue(info.intervals, leftIndent, leftIndent - fstLineIndent));
            }
            return new SimpleCommandState(this.isEnabled());
        }

        // calls when user change value in ruler
        // state consider old indents and correct intervals, indents consider correct new value and intervals==undefined
        executeCore(state: SimpleCommandState, indents: RulerParagraphLeftIndentsCommandValue): boolean {
            var hanging = UnitConverter.pixelsToTwips(indents.hanging); // here mean distance to second.. rows
            var fstLine = UnitConverter.pixelsToTwips(indents.firstLine); // here mean distance to first rows

            var firstLineIndent = Math.abs(hanging - fstLine);
            var firstLineIndentType: ParagraphFirstLineIndent = ParagraphFirstLineIndent.None;
            if(hanging < fstLine)
                firstLineIndentType = ParagraphFirstLineIndent.Indented;
            else if(hanging > fstLine)
                firstLineIndentType = ParagraphFirstLineIndent.Hanging;

            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.beginTransaction();
            for (var i: number = 0, interval: FixedInterval; interval = state.value.intervals[i]; i++) {
                this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, firstLineIndent, true));
                this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, firstLineIndentType, true));
                this.control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, hanging, true));
            }
            this.control.history.endTransaction();
            return true;
        }

    }

    export class RulerParagraphRightIndentCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            var info: { paragraphs: Paragraph[]; intervals: FixedInterval[] } = Utils.getSelectedParagraphs(this.control.selection, this.control.model.activeSubDocument);

            // find paragraph with max right indent
            var parWithMaxRightIndent: Paragraph = info.paragraphs[0];
            for (var i: number = 1, paragraph: Paragraph; paragraph = info.paragraphs[i]; i++)
                if(paragraph.getParagraphMergedProperies().rightIndent > parWithMaxRightIndent.getParagraphMergedProperies().rightIndent)
                    parWithMaxRightIndent = paragraph;

            var rightIndent: number = UnitConverter.twipsToPixels(parWithMaxRightIndent.getParagraphMergedProperies().rightIndent);
            return new SimpleCommandState(this.isEnabled(), new RulerParagraphLeftIndentsCommandValue(info.intervals, rightIndent, undefined));
        }

        // state consider correct old value right indent and intervals, parameter - new value right indent
        executeCore(state: SimpleCommandState, parameter: number): boolean {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.beginTransaction();
            for (var i: number = 0, interval: FixedInterval; interval = state.value.intervals[i]; i++)
                this.control.history.addAndRedo(new ParagraphRightIndentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, UnitConverter.pixelsToTwips(parameter), true));
            this.control.history.endTransaction();
            return true;
        }
    }
} 