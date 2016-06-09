module __aspxRichEdit {
    export class InsertTabCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        executeCore(state: SimpleCommandState): boolean {
            var interval = this.control.selection.getLastSelectedInterval().clone();
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphsIndices(interval);

            var documentEndPosition: number = this.control.model.activeSubDocument.getDocumentEndPosition();
            if(interval.start >= documentEndPosition)
                throw new Error("InsertTabCommandBase interval.start >= documentEndPosition");
            if(interval.length > 0 && interval.end() > documentEndPosition)
                throw new Error("InsertTabCommandBase interval.end() > documentEndPosition");

            if(!this.control.commandManager.assertLastExecutedCommandsChain(true, InsertTabCommandBase)) {
                var startParagraph = this.control.model.activeSubDocument.paragraphs[paragraphIndices.start];
                if(startParagraph.getTabs().positions.length === 0) {
                    var subDocument = this.control.model.activeSubDocument;
                    var startLayoutPosition = (subDocument.isMain()
                        ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, interval.start, DocumentLayoutDetailsLevel.Character)
                        : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, interval.start, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                        .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));
                    if(this.needProcessFirstParagraphRow(interval, startParagraph, startLayoutPosition)) {
                        if(startParagraph.isInList())
                            return this.createIndentNumberingParagraphCommand().execute();
                        return this.createChangeParagraphIndentFromFirstRowCommand().execute();
                    }
                    if(this.needProcessParagraphLeftIndent(interval, startLayoutPosition))
                        return this.createChangeIndentCommand().execute();
                }
            }

            if(ControlOptions.isEnabled(this.control.options.tabSymbol))
                this.control.modelManipulator.insertText(this.control, interval, this.control.options.tabMarker, true);
            return true;
        }

        private needProcessFirstParagraphRow(interval: FixedInterval, startParagraph: Paragraph, startLayoutPosition: LayoutPosition): boolean {
            if(this.control.commandManager.assertLastExecutedCommandsChain(false, InsertTabCommandBase, ChangeParagraphIndentFromFirstRowCommandBase) && this.isIntervalStartInParagraphStart(interval, startParagraph))
                return true;
            return this.isIntervalStartInParagraphStart(interval, startParagraph) && (interval.length === 0 || this.isIntervalEndOnRowEnd(interval, startLayoutPosition));
        }

        private needProcessParagraphLeftIndent(interval: FixedInterval, startLayoutPosition: LayoutPosition): boolean {
            if(this.control.commandManager.assertLastExecutedCommandsChain(false, InsertTabCommandBase, ChangeIndentCommandBase))
                return true;
            var subDocument = this.control.model.activeSubDocument;
            var endLayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, interval.end(), DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, interval.end(), this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(true), new LayoutPositionCreatorConflictFlags().setDefault(false));
            return startLayoutPosition && startLayoutPosition.isPositionBeforeFirstBoxInRow() && this.isIntervalIncludesWholeRow(startLayoutPosition, endLayoutPosition);
        }

        createIndentNumberingParagraphCommand(): ICommand {
            throw new Error(Errors.NotImplemented);
        }
        createChangeParagraphIndentFromFirstRowCommand(): ICommand {
            throw new Error(Errors.NotImplemented);
        }
        createChangeIndentCommand(): ICommand {
            throw new Error(Errors.NotImplemented);
        }

        private isIntervalStartInParagraphStart(interval: FixedInterval, paragraph: Paragraph): boolean {
            return interval.start === paragraph.startLogPosition.value;
        }
        private isIntervalEndOnRowEnd(interval: FixedInterval, layoutPosition: LayoutPosition): boolean {
            if(!layoutPosition)
                return false;
            return layoutPosition.getRelatedSubDocumentPagePosition() + layoutPosition.pageArea.pageOffset + layoutPosition.column.pageAreaOffset + layoutPosition.row.getEndPosition() === interval.end();
        }
        private isIntervalIncludesWholeRow(startLayoutPosition: LayoutPosition, endLayoutPosition: LayoutPosition): boolean {
            return !endLayoutPosition || endLayoutPosition.row !== startLayoutPosition.row || endLayoutPosition.isPositionAfterLastBoxInRow();
        }
    }

    export class InsertTabCommand extends InsertTabCommandBase {
        createIndentNumberingParagraphCommand(): ICommand {
            return this.control.commandManager.getCommand(RichEditClientCommand.IncrementNumberingIndent);
        }
        createChangeParagraphIndentFromFirstRowCommand(): ICommand {
            return this.control.commandManager.getCommand(RichEditClientCommand.IncrementParagraphIndentFromFirstRow);
        }
        createChangeIndentCommand(): ICommand {
            return this.control.commandManager.getCommand(RichEditClientCommand.IncreaseIndent);
        }
    }

    export class InsertShiftTabCommand extends InsertTabCommandBase {
        createIndentNumberingParagraphCommand(): ICommand {
            return this.control.commandManager.getCommand(RichEditClientCommand.DecrementNumberingIndent);
        }
        createChangeParagraphIndentFromFirstRowCommand(): ICommand {
            return this.control.commandManager.getCommand(RichEditClientCommand.DecrementParagraphIndentFromFirstRow);
        }
        createChangeIndentCommand(): ICommand {
            return this.control.commandManager.getCommand(RichEditClientCommand.DecreaseIndent);
        }
    }
}