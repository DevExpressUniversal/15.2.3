module __aspxRichEdit {
    export class SetPredefinedSectionPageMarginsCommandBase extends SectionPropertiesCommandBase {
        getValue(): boolean {
            var mergedSectionProperties = this.control.inputPosition.getMergedSectionPropertiesRaw();
            return mergedSectionProperties.marginBottom === this.getMarginBottom() && mergedSectionProperties.marginLeft === this.getMarginLeft() && mergedSectionProperties.marginRight === this.getMarginRight() && mergedSectionProperties.marginTop === this.getMarginTop();
        }
        executeCore(state: IntervalCommandStateEx, parameter: boolean): boolean {
            if(state.value)
                return false;
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.beginTransaction();
            for(let i = 0, interval: FixedInterval; interval = state.intervals[i]; i++) {
                this.control.history.addAndRedo(new SectionMarginBottomHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, this.getMarginBottom()));
                this.control.history.addAndRedo(new SectionMarginLeftHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, this.getMarginLeft()));
                this.control.history.addAndRedo(new SectionMarginRightHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, this.getMarginRight()));
                this.control.history.addAndRedo(new SectionMarginTopHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, this.getMarginTop()));
            }
            this.control.history.endTransaction();
            return true;
        }

        getMarginLeft(): number {
            throw new Error(Errors.NotImplemented);
        }
        getMarginRight(): number {
            throw new Error(Errors.NotImplemented);
        }
        getMarginTop(): number {
            throw new Error(Errors.NotImplemented);
        }
        getMarginBottom(): number {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class SetNormalSectionPageMarginsCommand extends SetPredefinedSectionPageMarginsCommandBase {
        getMarginLeft(): number {
            return 1700;
        }
        getMarginRight(): number {
            return 850;
        }
        getMarginTop(): number {
            return 1133;
        }
        getMarginBottom(): number {
            return 1133;
        }
    }

    export class SetNarrowSectionPageMarginsCommand extends SetPredefinedSectionPageMarginsCommandBase {
        getMarginLeft(): number {
            return 720;
        }
        getMarginRight(): number {
            return 720;
        }
        getMarginTop(): number {
            return 720;
        }
        getMarginBottom(): number {
            return 720;
        }
    }

    export class SetModerateSectionPageMarginsCommand extends SetPredefinedSectionPageMarginsCommandBase {
        getMarginLeft(): number {
            return 1080;
        }
        getMarginRight(): number {
            return 1080;
        }
        getMarginTop(): number {
            return 1440;
        }
        getMarginBottom(): number {
            return 1440;
        }
    }

    export class SetWideSectionPageMarginsCommand extends SetPredefinedSectionPageMarginsCommandBase {
        getMarginLeft(): number {
            return 2880;
        }
        getMarginRight(): number {
            return 2880;
        }
        getMarginTop(): number {
            return 1440;
        }
        getMarginBottom(): number {
            return 1440;
        }
    }
} 