module __aspxRichEdit {
    export class GoToPrevWordCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        getStartPosition(): number {
            var selection: Selection = this.control.selection;
            return (selection.forwardDirection ? selection.getLastSelectedInterval().end() : selection.getLastSelectedInterval().start) - 1;
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }

    export class GoToPrevWordCommand extends GoToPrevWordCommandBase {
        executeCore(state: SimpleCommandState): boolean {
            this.control.getDocumentRenderer().updateLastTimeMoveCursor();
            var position: number = LayoutWordPositionHelper.getPrevWordEndPosition(this.control.layout, this.control.model.activeSubDocument, this.control.selection, this.getStartPosition());
            var selection: Selection = this.control.selection;
            selection.setSelection(position, position, selection.endOfLine, selection.keepX, UpdateInputPositionProperties.Yes);
            return true;
        }
    }

    export class ExtendGoToPrevWordCommand extends GoToPrevWordCommandBase {
        executeCore(state: SimpleCommandState): boolean {
            var selection: Selection = this.control.selection;
            var position: number = LayoutWordPositionHelper.getPrevWordEndPosition(this.control.layout, this.control.model.activeSubDocument, this.control.selection, this.getStartPosition());
            selection.extendLastSelection(position, false, -1, UpdateInputPositionProperties.Yes, Field.jumpThroughFieldToLeft);
            return true;
        }
    }
} 