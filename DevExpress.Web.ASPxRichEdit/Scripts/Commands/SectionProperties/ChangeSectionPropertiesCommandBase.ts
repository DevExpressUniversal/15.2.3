module __aspxRichEdit {
    export class ChangeSectionPropertiesCommandBase<T> extends SectionPropertiesCommandBase {
        getValue(): any {
            return this.getSectionPropertiesValue(this.control.inputPosition.getMergedSectionPropertiesRaw());
        }
        setNewValue(intervals: FixedInterval[], newValue: T) {
            for(var i = 0, interval: FixedInterval; interval = intervals[i]; i++)
                this.setNewValueCore(interval, newValue);
        }
        private setNewValueCore(interval: FixedInterval, newValue: T) {
            this.setSectionPropertiesValue(this.control.inputPosition.getMergedSectionPropertiesRaw(), newValue);
            this.control.history.addAndRedo(this.getHistoryItem(interval, newValue));
        }

        // abstract methods
        getHistoryItem(interval: FixedInterval, newValue: T): HistoryItem {
            throw new Error(Errors.NotImplemented);
        }
        setSectionPropertiesValue(properties: SectionProperties, value: T) {
            throw new Error(Errors.NotImplemented);
        }
        getSectionPropertiesValue(properties: SectionProperties): T {
            throw new Error(Errors.NotImplemented);
        }
    }
}   