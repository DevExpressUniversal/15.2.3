module __aspxRichEdit {
    export class ChangePageColorCommand extends CommandBase<SimpleCommandState> {
        getState(): ICommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.denyUpdateValue = true;
            return state;
        }

        executeCore(state: ICommandState, parameter: string): boolean {
            this.control.history.addAndRedo(new PageColorHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, this.getColor(parameter)));
            return true;
        }

        getColor(value: string): number {
            return value == ColorHelper.NO_COLOR.toString() ? ColorHelper.NO_COLOR : ColorHelper.hashToColor(value);
        }
    }
}