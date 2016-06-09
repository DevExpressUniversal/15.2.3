module __aspxRichEdit {
    export class ChangeCharacterPropertiesCommandBase<T> extends CommandBase<IntervalCommandStateEx> {
        getActualIntervals(): FixedInterval[] {
            if(this.control.selection.isCollapsed())
                return [this.control.model.activeSubDocument.getWholeWordInterval(this.control.selection.intervals[0].start)];
            return this.control.selection.getIntervalsClone();
        }
        executeCore(state: IntervalCommandStateEx, parameter: any): boolean {
            var newValue = this.getActualValue(parameter, state.value);
            if(state.value !== newValue)
                this.setNewValue(state.intervals, newValue);
            return true;
        }
        getState(): IntervalCommandStateEx {
            var state = new IntervalCommandStateEx(this.isEnabled(), this.getActualIntervals(), this.getCharacterPropertiesValue(this.control.inputPosition.getMergedCharacterPropertiesRaw()));
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
            this.setCharacterPropertiesValue(this.control.inputPosition.getMaskedCharacterProperties(), newValue); // Masked
            this.setCharacterPropertiesValue(this.control.inputPosition.getMergedCharacterPropertiesFull(), newValue); // merged
            this.setCharacterPropertiesValue(this.control.inputPosition.getMergedCharacterPropertiesRaw(), newValue); // merged
            this.control.inputPosition.getMaskedCharacterProperties().setUseValue(this.getCharacterPropertiesMask(), true);
            if (interval.length > 0)
                this.control.history.addAndRedo(this.getHistoryItem(interval, newValue));
        }
        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.characterFormatting);
        }

        // abstract methods
        getCharacterPropertiesMask(): CharacterPropertiesMask {
            throw new Error(Errors.NotImplemented);
        }
        getHistoryItem(interval: FixedInterval, newValue: T): HistoryItem {
            throw new Error(Errors.NotImplemented);
        }
        setCharacterPropertiesValue(properties: CharacterProperties, value: T) {
            throw new Error(Errors.NotImplemented);
        }
        getCharacterPropertiesValue(properties: CharacterProperties): T {
            throw new Error(Errors.NotImplemented);
        }
        getActualValue(parameter: any, currentValue: T): T {
            throw new Error(Errors.NotImplemented);
        }
    }
} 