module __aspxRichEdit {
    export class InsertSpaceCommand extends CommandBase<SimpleCommandState> {
        private numberingRegEx: RegExp = /^[0-9]+\./;

        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState): boolean {
            if(this.tryAddParagraphToNumberingList())
                return true;
            this.control.modelManipulator.insertText(this.control, this.control.selection.getLastSelectedInterval().clone(), Utils.specialCharacters.Space, true);
            return true;
        }

        tryAddParagraphToNumberingList(): boolean {
            if(this.control.selection.intervals.length > 1)
                return false;
            var interval = this.control.selection.intervals[0].clone();
            var paragraphIndex = this.control.model.activeSubDocument.getParagraphIndexByPosition(interval.start);
            var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndex];
            if(paragraph.isInList())
                return false;
            var paragraphText = this.control.model.activeSubDocument.getText(paragraph.getInterval());
            var targetNumberingListType = this.getTargetNumberingListType(paragraphText, interval.start - paragraph.startLogPosition.value);
            if(targetNumberingListType === undefined)
                return false;
            this.control.history.beginTransaction();
            var pointerTextPosition = targetNumberingListType === NumberingType.Simple ? (paragraphText.indexOf(".") + 1) : 1;
            var targetIndex = targetNumberingListType === NumberingType.Simple ? parseInt(paragraphText.substr(0, pointerTextPosition)) : -1;

            ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(paragraph.startLogPosition.value, 0), UpdateInputPositionProperties.No, false);
            if(interval.length > 0)
                ModelManipulator.removeInterval(this.control, this.control.model.activeSubDocument, interval, false);
            ModelManipulator.removeInterval(this.control, this.control.model.activeSubDocument, new FixedInterval(paragraph.startLogPosition.value, pointerTextPosition), false);

            if(targetNumberingListType === NumberingType.Bullet)
                this.control.commandManager.getCommand(RichEditClientCommand.ToggleBulletedListItem).execute();
            else
                this.control.commandManager.getCommand(RichEditClientCommand.ToggleNumberingListItem).execute(targetIndex);
            this.control.history.endTransaction();
            return true;
        }

        getTargetNumberingListType(paragraphText: string, paragraphCursorPosition: number): NumberingType {
            if(paragraphText.indexOf("*") === 0 && paragraphCursorPosition === 1)
                return NumberingType.Bullet;
            if(this.numberingRegEx.test(paragraphText) && paragraphText.indexOf(".") === paragraphCursorPosition - 1)
                return NumberingType.Simple;
            return undefined;
        }
    }

    export class InsertNonBreakingSpaceCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState): boolean {
            this.control.modelManipulator.insertText(this.control, this.control.selection.getLastSelectedInterval().clone(), Utils.specialCharacters.NonBreakingSpace, true);
            return true;
        }
    }
}