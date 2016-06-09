module __aspxRichEdit {
    export class ChangeInlinePictureScaleCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState, parameter: number[]): boolean {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.addAndRedo(new ChangeRectangularObjectScaleHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument,
                new FixedInterval(this.control.selection.selectedInlinePictureRunPosition, 1), parameter[0], parameter[1]));
            return true;
        }

        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return true;
        }
        lockUIUpdating(prevModifiedState: IsModified): boolean {
            return prevModifiedState === this.control.getModifiedState();
        }
    }
}