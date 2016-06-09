module __aspxRichEdit {
    export class DragCopyContentCommand extends CommandBase<SimpleCommandState> {
        executeCore(state: SimpleCommandState, positionTo: number): boolean {
            var selectionState = this.control.selection.getSelectionState();
            var intervals = selectionState.intervals;
            let executed = false;
            let selectionIntervals: FixedInterval[] = [];
            this.control.history.beginTransaction();
            for(let i = 0, interval: LinkedInterval; interval = intervals[i]; i++) {
                let fixedInterval = interval.getFixedInterval();
                ModelManipulator.copyIntervalTo(this.control, this.control.model.activeSubDocument, fixedInterval, positionTo);
                selectionIntervals.push(new FixedInterval(positionTo, fixedInterval.length));
                positionTo += fixedInterval.length;
                executed = true;
            }
            if(selectionIntervals.length)
                this.control.history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, selectionIntervals, this.control.selection, UpdateInputPositionProperties.Yes, this.control.selection.endOfLine));
            this.control.history.endTransaction();
            selectionState.destructor();
            return executed;
        }
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.options.drag !== DocumentCapability.Hidden && this.control.options.drop !== DocumentCapability.Hidden;
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.drag) && ControlOptions.isEnabled(this.control.options.drop) && !this.control.selection.isCollapsed();
        }
    }

    export class DragMoveContentCommand extends CommandBase<SimpleCommandState> {
        executeCore(state: SimpleCommandState, positionTo: number): boolean {
            if(this.control.selection.intervals.length == 1 && this.control.model.activeSubDocument.getDocumentEndPosition() - 1 == this.control.selection.intervals[0].start)
                return this.control.commandManager.getCommand(RichEditClientCommand.DragCopyContent).execute(positionTo);
            let selectionState = this.control.selection.getSelectionState();
            let intervals = selectionState.intervals;
            let selectionLinkedIntervals: LinkedInterval[] = [];
            let skipIntervalIndex = -1;
            this.control.history.beginTransaction();
            for(let i = 0, interval: LinkedInterval; interval = intervals[i]; i++) {
                if(interval.start.value <= positionTo && interval.end.value >= positionTo) {
                    positionTo = interval.start.value;
                    skipIntervalIndex = i;
                    break;
                }
            }
            let executed = false;
            let skipInterval = intervals[skipIntervalIndex];
            let skipIntervalLength = skipInterval ? skipInterval.getLength() : 0;
            for(let i = 0, interval: LinkedInterval; interval = intervals[i]; i++) {
                if(i === skipIntervalIndex) {
                    if(intervals.length > 1)
                        selectionLinkedIntervals.push(new LinkedInterval(this.control.model.activeSubDocument.positionManager, positionTo, positionTo + skipIntervalLength));
                    positionTo += skipIntervalLength;
                    continue;
                }
                let fixedInterval = interval.getFixedInterval();
                ModelManipulator.moveIntervalTo(this.control, this.control.model.activeSubDocument, fixedInterval, positionTo);
                let selectionStartPosition = fixedInterval.start < positionTo ? positionTo - fixedInterval.length : positionTo;
                let selectionEndPosition = selectionStartPosition + fixedInterval.length;
                selectionLinkedIntervals.push(new LinkedInterval(this.control.model.activeSubDocument.positionManager, selectionStartPosition, selectionEndPosition));
                if(positionTo <= fixedInterval.start)
                    positionTo += fixedInterval.length;
                executed = true;
            }
            if(selectionLinkedIntervals.length) {
                let selectionIntervals: FixedInterval[] = [];
                for(let i = 0, selectionLinkedInterval: LinkedInterval; selectionLinkedInterval = selectionLinkedIntervals[i]; i++) {
                    selectionIntervals.push(selectionLinkedInterval.getFixedInterval());
                    selectionLinkedInterval.destructor(this.control.model.activeSubDocument.positionManager);
                }
                this.control.history.addAndRedo(new SetSelectionHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, selectionIntervals, this.control.selection, UpdateInputPositionProperties.Yes, this.control.selection.endOfLine));
            }
            this.control.history.endTransaction();
            selectionState.destructor();
            return executed;
        }
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.options.drag !== DocumentCapability.Hidden && this.control.options.drop !== DocumentCapability.Hidden;
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.drag) && ControlOptions.isEnabled(this.control.options.drop) && !this.control.selection.isCollapsed();
        }
    }
}