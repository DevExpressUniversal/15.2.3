module __aspxRichEdit {
    export class GoToNextWordCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        getStartPosition(): number {
            var selection: Selection = this.control.selection;
            return selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start;
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }

    export class GoToNextWordCommand extends GoToNextWordCommandBase {
        executeCore(state: SimpleCommandState): boolean {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            var position: number = LayoutWordPositionHelper.getNextWordStartPosition(this.control.layout, this.control.model.activeSubDocument, this.control.selection, this.getStartPosition(), true);
            var selection: Selection = this.control.selection;
            selection.setSelection(position, position, false, selection.keepX, UpdateInputPositionProperties.Yes);
            return true;
        }
    }

    export class ExtendGoToNextWordCommand extends GoToNextWordCommandBase {
        executeCore(state: SimpleCommandState): boolean {
            var selection: Selection = this.control.selection;
            var position: number = LayoutWordPositionHelper.getNextWordStartPosition(this.control.layout, this.control.model.activeSubDocument, this.control.selection, this.getStartPosition(), true);
            selection.extendLastSelection(position, false, -1, UpdateInputPositionProperties.Yes, Field.jumpThroughFieldToRight);
            return true;
        }
    }
} 