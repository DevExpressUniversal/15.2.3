module __aspxRichEdit {
    export class RulerSectionMarginLeftCommand extends CommandBase<IntervalCommandState> {
        getActualInterval(): FixedInterval {
            return this.control.selection.getLastSelectedInterval();
        }
        getState(): IntervalCommandState {
            var value = UnitConverter.twipsToPixels(this.control.inputPosition.getMergedSectionPropertiesRaw().marginLeft);
            return new IntervalCommandState(this.isEnabled(), this.getActualInterval(), value);
        }
        executeCore(state: IntervalCommandState, parameter: number): boolean {
            var interval = (<IntervalCommandState>state).interval;
            var value = UnitConverter.pixelsToTwips(parameter);
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.addAndRedo(new SectionMarginLeftHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, value));
            return true;
        }
    }
    export class RulerSectionMarginRightCommand extends CommandBase<IntervalCommandState> {
        getActualInterval(): FixedInterval {
            return this.control.selection.getLastSelectedInterval();
        }
        getState(): IntervalCommandState {
            var value = UnitConverter.twipsToPixels(this.control.inputPosition.getMergedSectionPropertiesRaw().marginRight);
            return new IntervalCommandState(this.isEnabled(), this.getActualInterval(), value);
        }
        executeCore(state: IntervalCommandState, parameter: number): boolean {
            var interval = (<IntervalCommandState>state).interval;
            var value = UnitConverter.pixelsToTwips(parameter);
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.addAndRedo(new SectionMarginRightHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, value));
            return true;
        }
    }
}  