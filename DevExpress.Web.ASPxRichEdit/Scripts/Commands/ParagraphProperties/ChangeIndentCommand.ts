module __aspxRichEdit {
    export class ChangeIndentCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.paragraphFormatting);
        }
        executeCore(state: SimpleCommandState, parameter: number[]): boolean {
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(this.control.selection.intervals);
            if(this.shouldProcessAsNumberingParagraphs(paragraphIndices))
                return this.processNumberingIndents();
            else
                return this.processParagraphIndents();
        }
        processParagraphIndents(): boolean {
            throw new Error(Errors.NotImplemented);
        }
        processNumberingIndents(): boolean {
            throw new Error(Errors.NotImplemented);
        }
        private shouldProcessAsNumberingParagraphs(paragraphIndices: number[]): boolean {
            var abstractNumberingListIndex = this.control.model.activeSubDocument.paragraphs[paragraphIndices[0]].getAbstractNumberingListIndex();
            if(abstractNumberingListIndex < 0)
                return false;
            let paragraphIndicesLength = paragraphIndices.length;
            for(let i = 1; i < paragraphIndicesLength; i++) {
                let paragraphIndex = paragraphIndices[i];
                if(this.control.model.activeSubDocument.paragraphs[paragraphIndex].getAbstractNumberingListIndex() !== abstractNumberingListIndex)
                    return false;
            }
            return true;
        }
    }

    export class DecrementIndentCommand extends ChangeIndentCommandBase {
        processParagraphIndents(): boolean {
            return this.control.commandManager.getCommand(RichEditClientCommand.DecrementParagraphLeftIndent).execute();
        }
        processNumberingIndents(): boolean {
            return this.control.commandManager.getCommand(RichEditClientCommand.DecrementNumberingIndent).execute();
        }
    }

    export class IncrementIndentCommand extends ChangeIndentCommandBase {
        processParagraphIndents(): boolean {
            return this.control.commandManager.getCommand(RichEditClientCommand.IncrementParagraphLeftIndent).execute();
        }
        processNumberingIndents(): boolean {
            return this.control.commandManager.getCommand(RichEditClientCommand.IncrementNumberingIndent).execute();
        }
    }
} 