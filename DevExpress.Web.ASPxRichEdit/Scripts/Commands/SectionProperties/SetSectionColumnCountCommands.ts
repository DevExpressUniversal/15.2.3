module __aspxRichEdit {
    export class SetSectionColumnCountCommand extends SectionPropertiesCommandBase {
        getValue(): boolean {
            var secProps: SectionProperties = this.control.inputPosition.getMergedSectionPropertiesRaw();
            return secProps.equalWidthColumns && secProps.columnCount == this.getColumnCount();
        }
        executeCore(state: IntervalCommandStateEx, parameter: boolean): boolean {
            if(state.value)
                return false;
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.beginTransaction();
            for(let i = 0, interval: FixedInterval; interval = state.intervals[i]; i++) {
                this.control.history.addAndRedo(new SectionEqualWidthColumnsHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, true));
                this.control.history.addAndRedo(new SectionColumnCountHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, this.getColumnCount()));
            }
            this.control.history.endTransaction();
            return true;
        }
        getColumnCount(): number {
            throw new Error(Errors.NotImplemented);
        }
        isEnabled(): boolean {
            return super.isEnabled() && this.control.model.activeSubDocument.isMain();
        }
    }
    export class SetSectionOneColumnCommand extends SetSectionColumnCountCommand {
        getColumnCount(): number {
            return 1;
        }
    }
    export class SetSectionTwoColumnsCommand extends SetSectionColumnCountCommand {
        getColumnCount(): number {
            return 2;
        }
    }
    export class SetSectionThreeColumnsCommand extends SetSectionColumnCountCommand {
        getColumnCount(): number {
            return 3;
        }
    }
}