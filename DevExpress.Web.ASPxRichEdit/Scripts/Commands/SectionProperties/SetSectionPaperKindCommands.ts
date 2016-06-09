module __aspxRichEdit {
    export class SetSectionPaperKindCommand extends SectionPropertiesCommandBase {
        getValue(): boolean {
            var paperKind = this.getPaperKind();
            var size = PaperSizeConverter.calculatePaperSize(paperKind);
            var mergedProps = this.control.inputPosition.getMergedSectionPropertiesRaw();
            return (mergedProps.pageHeight === size.height && mergedProps.pageWidth === size.width) || (mergedProps.pageHeight === size.width && mergedProps.pageWidth === size.height);
        }
        executeCore(state: IntervalCommandStateEx, parameter: boolean): boolean {
            if(state.value)
                return false;
            var paperKind = this.getPaperKind();
            var size = PaperSizeConverter.calculatePaperSize(paperKind);
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.beginTransaction();
            for(let i = 0, interval: FixedInterval; interval = state.intervals[i]; i++) {
                var sections: Section[] = this.control.model.activeSubDocument.documentModel.getSectionsByInterval(interval);
                for(let j = 0, section: Section; section = sections[j]; j++) {
                    var sectionInterval = new FixedInterval(section.startLogPosition.value, section.getLength());
                    this.control.history.addAndRedo(new SectionPageWidthHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, sectionInterval, section.sectionProperties.landscape ? size.height : size.width));
                    this.control.history.addAndRedo(new SectionPageHeightHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, sectionInterval, section.sectionProperties.landscape ? size.width : size.height));
                }
            }
            this.control.history.endTransaction();
            return true;
        }
        getPaperKind(): PaperKind {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class SetSectionLetterPaperKindCommand extends SetSectionPaperKindCommand {
        getPaperKind(): PaperKind { return PaperKind.Letter; }
    }
    export class SetSectionLegalPaperKindCommand extends SetSectionPaperKindCommand {
        getPaperKind(): PaperKind { return PaperKind.Legal; }
    }
    export class SetSectionFolioPaperKindCommand extends SetSectionPaperKindCommand {
        getPaperKind(): PaperKind { return PaperKind.Folio; }
    }
    export class SetSectionA4PaperKindCommand extends SetSectionPaperKindCommand {
        getPaperKind(): PaperKind { return PaperKind.A4; }
    }
    export class SetSectionA5PaperKindCommand extends SetSectionPaperKindCommand {
        getPaperKind(): PaperKind { return PaperKind.A5; }
    }
    export class SetSectionA6PaperKindCommand extends SetSectionPaperKindCommand {
        getPaperKind(): PaperKind { return PaperKind.A6; }
    }
    export class SetSectionB5PaperKindCommand extends SetSectionPaperKindCommand {
        getPaperKind(): PaperKind { return PaperKind.B5; }
    }
    export class SetSectionExecutivePaperKindCommand extends SetSectionPaperKindCommand {
        getPaperKind(): PaperKind { return PaperKind.Executive; }
    }
} 