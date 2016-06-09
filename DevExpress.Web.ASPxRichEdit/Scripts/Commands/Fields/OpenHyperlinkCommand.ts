module __aspxRichEdit {
    export class OpenHyperlinkCommand extends HyperlinkCommandBase {
        executeCore(state: SimpleCommandState, parameter: Field): boolean {
            var field: Field;
            if (parameter)
                field = parameter; // for case call from mouse handler. When selecter-fieldHyperlinkImage
            else {
                if (!state.visible)
                    return false;
                field = state.value;
            }

            var hyperlinkInfo: HyperlinkInfo = field.getHyperlinkInfo();
            //var url: string = hyperlinkInfo.uri + (hyperlinkInfo.anchor != "" ? "#" + hyperlinkInfo.anchor : "");

            if(!hyperlinkInfo.visited) {
                var newHyperlinkInfo: HyperlinkInfo = hyperlinkInfo.clone();
                newHyperlinkInfo.visited = true;

                var selection: Selection = this.control.selection;
                var oldPos: number = this.control.selection.getLastSelectedInterval().start;
                var resultInterval: FixedInterval = field.getResultInterval();

                // yes. need add to history
                this.control.history.beginTransaction();
                this.control.history.addAndRedo(new ChangeFieldHyperlinkInfoHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, field.index, newHyperlinkInfo));

                selection.setSelection(resultInterval.start, resultInterval.end(), selection.endOfLine, selection.keepX, UpdateInputPositionProperties.No, false);
                this.control.commandManager.getCommand(RichEditClientCommand.ChangeFontForeColor).execute("#483D8B");
                selection.setSelection(oldPos, oldPos, selection.endOfLine, selection.keepX, UpdateInputPositionProperties.No, false);

                this.control.history.endTransaction();
            }

            this.control.serverDispatcher.forceSendingRequest();

            if (hyperlinkInfo.anchor)
                this.control.commandManager.getCommand(RichEditClientCommand.GoToBookmark).execute(hyperlinkInfo.anchor);
            else if (!ASPx.IsUrlContainsClientScript(hyperlinkInfo.uri))
                window.open(hyperlinkInfo.uri, '_blank');
            return true;
        }
    }
}