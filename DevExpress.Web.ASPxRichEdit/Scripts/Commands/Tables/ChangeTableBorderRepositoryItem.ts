module __aspxRichEdit {
    export class ChangeTableBorderColorRepositoryItemCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(true, false);
            let selectedCells = this.control.selection.getSelectedCells();
            state.enabled = this.isEnabled() && selectedCells.length > 0;
            state.value = ColorHelper.colorToHash(this.control.model.repositoryBorderItem.color);
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables);
        }
        executeCore(state: SimpleCommandState, parameter: string): boolean {
            this.control.model.repositoryBorderItem.color = this.getColor(parameter);
            return true;
        }
        getColor(value: string): number {
            return value == ColorHelper.NO_COLOR.toString() ? ColorHelper.NO_COLOR : ColorHelper.hashToColor(value);
        }
    }

    export class ChangeTableBorderWidthRepositoryItemCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(true, false);
            let selectedCells = this.control.selection.getSelectedCells();
            state.enabled = this.isEnabled() && selectedCells.length > 0;
            state.value = this.control.model.repositoryBorderItem.width;
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables);
        }
        executeCore(state: SimpleCommandState, parameter: number): boolean {
            this.control.model.repositoryBorderItem.width = parameter;
            return true;
        }
    }

    export class ChangeTableBorderStyleRepositoryItemCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            let state = new SimpleCommandState(true, false);
            let selectedCells = this.control.selection.getSelectedCells();
            state.enabled = this.isEnabled() && selectedCells.length > 0;
            state.value = this.control.model.repositoryBorderItem.style;
            return state;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables);
        }
        executeCore(state: SimpleCommandState, parameter: BorderLineStyle): boolean {
            this.control.model.repositoryBorderItem.style = parameter;
            return true;
        }
    }
}