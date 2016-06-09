module __aspxRichEdit {
    export class InlineObjectManipulator {
        manipulator: ModelManipulator;
        constructor(dispatcher: ModelManipulator) {
            this.manipulator = dispatcher;
        }

        setScale(subDocument: SubDocument, interval: FixedInterval, scaleX: number, scaleY: number): HistoryItemIntervalState<HistoryItemIntervalStateObject> {
            var oldState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            var newState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();

            var run = <InlineObjectRun>subDocument.getRunByPosition(interval.start);
            oldState.register(new HistoryItemIntervalStateObject(interval, [run.scaleX, run.scaleY]));
            newState.register(new HistoryItemIntervalStateObject(interval, [scaleX, scaleY]));

            run.scaleX = scaleX;
            run.scaleY = scaleY;
            this.manipulator.dispatcher.notifyInlineObjectRunPropertyChanged(interval, JSONInlineObjectProperty.Scales, newState, this.manipulator.model.activeSubDocument);
            return oldState;
        }

        restoreScale(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            var run = <InlineObjectRun>subDocument.getRunByPosition(state.interval().start);
            run.scaleX = state.objects[0].value[0];
            run.scaleY = state.objects[0].value[1];
            this.manipulator.dispatcher.notifyInlineObjectRunPropertyChanged(state.interval(), JSONInlineObjectProperty.Scales, state, this.manipulator.model.activeSubDocument);
        }
    }
} 