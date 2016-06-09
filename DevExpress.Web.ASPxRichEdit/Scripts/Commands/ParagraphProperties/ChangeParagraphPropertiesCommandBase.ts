module __aspxRichEdit {
    export class ChangeParagraphPropertiesCommandBase<T> extends CommandBase<IntervalCommandStateEx> {
        getActualIntervals(): FixedInterval[] {
            return this.control.selection.getIntervalsClone();
        }
        executeCore(state: IntervalCommandStateEx, parameter: any): boolean {
            var newValue = this.getActualValue(parameter, state.value);
            if(newValue !== state.value)
                this.setNewValue(state.intervals, newValue);
            return true;
        }
        getState(): IntervalCommandStateEx {
            var state = new IntervalCommandStateEx(this.isEnabled(), this.getActualIntervals(), this.getParagraphPropertiesValue(this.control.inputPosition.getMergedParagraphPropertiesRaw()));
            state.denyUpdateValue = this.isLockUpdateVaue();
            return state;
        }
        isLockUpdateVaue(): boolean {
            return false;
        }
        setNewValue(intervals: FixedInterval[], newValue: T) {
            this.control.history.beginTransaction();
            for(var i = 0, interval: FixedInterval; interval = intervals[i]; i++) {
                this.setNewValueCore(interval, newValue);
            }
            this.control.history.endTransaction();
        }
        private setNewValueCore(interval: FixedInterval, newValue: T) {
            this.setParagraphPropertiesValue(this.control.inputPosition.getMergedParagraphPropertiesFull(), newValue);
            this.setParagraphPropertiesValue(this.control.inputPosition.getMergedParagraphPropertiesRaw(), newValue);
            var historyItem = this.getHistoryItem(interval, newValue);

            this.control.history.addAndRedo(historyItem);
        }
        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.paragraphFormatting);
        }

        // abstract methods
        getHistoryItem(interval: FixedInterval, newValue: T): HistoryItem {
            throw new Error(Errors.NotImplemented);
        }
        setParagraphPropertiesValue(properties: ParagraphProperties, value: T) {
            throw new Error(Errors.NotImplemented);
        }
        getParagraphPropertiesValue(properties: ParagraphProperties): T {
            throw new Error(Errors.NotImplemented);
        }
        getActualValue(parameter: any, currentValue: T): T {
            throw new Error(Errors.NotImplemented);
        }
    }
}  