module __aspxRichEdit {
    export class ChangeParagraphIndentFromFirstRowCommandBase extends ParagraphIndentCommandBase { // abstract
        columnsCalculator: ColumnsCalculator;
        constructor(control: IRichEditControl) {
            super(control);
            this.columnsCalculator = new ColumnsCalculator(new TwipsUnitConverter());
        }
        getState(): ICommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        executeCore(state: ICommandState): boolean {
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(this.control.selection.intervals);
            var firstParagraph = this.control.model.activeSubDocument.paragraphs[paragraphIndices[0]];
            var tabs = this.getTabs(paragraphIndices);
            if(this.needUpdateFirstLineIndent(firstParagraph, tabs)) {
                this.control.history.beginTransaction();
                var maxFirstLineIndent = this.getMaxFirstLineIndent(firstParagraph);
                var tabs = this.getTabs(paragraphIndices);
                var firstLineIndent = this.getFirstLineIndent(firstParagraph, tabs);
                this.assignParagraphFirstLineIndent(firstParagraph, firstLineIndent, maxFirstLineIndent);
                this.control.history.endTransaction();
                return true;
            }
            else
                return this.getParagraphLeftIndentCommand().execute();
        }

        getMaxFirstLineIndent(paragraph: Paragraph): number {
            throw new Error(Errors.NotImplemented);
        }
        getFirstLineIndent(paragraph: Paragraph, tabs: number[]): number {
            throw new Error(Errors.NotImplemented);
        }
        needUpdateFirstLineIndent(paragraph: Paragraph, tabs: number[]): boolean {
            throw new Error(Errors.NotImplemented);
        }
        getParagraphLeftIndentCommand(): ICommand {
            throw new Error(Errors.NotImplemented);
        }
        private assignParagraphFirstLineIndent(paragraph: Paragraph, firstLineIndent: number, maxValue: number) {
            if(firstLineIndent > 0) {
                this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraph.getInterval(), ParagraphFirstLineIndent.Indented, true));
                var distanceToRight = maxValue - (paragraph.getParagraphMergedProperies().leftIndent + firstLineIndent);
                if(distanceToRight < 0)
                    firstLineIndent += distanceToRight;
                this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraph.getInterval(), firstLineIndent, true));
            }
            else if(firstLineIndent < 0) {
                this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraph.getInterval(), ParagraphFirstLineIndent.Hanging, true));
                this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraph.getInterval(), Math.abs(firstLineIndent), true));
            }
            else {
                this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraph.getInterval(), ParagraphFirstLineIndent.None, true));
                this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(this.control.modelManipulator, this.control.model.activeSubDocument, paragraph.getInterval(), 0, true));
            }
        }

        getFirstLineIndentAbsPosition(paragraphProperties: ParagraphProperties): number {
            switch(paragraphProperties.firstLineIndentType) {
                case ParagraphFirstLineIndent.Indented:
                    return paragraphProperties.leftIndent + paragraphProperties.firstLineIndent;
                case ParagraphFirstLineIndent.Hanging:
                    return paragraphProperties.leftIndent - paragraphProperties.firstLineIndent;
                default:
                    return paragraphProperties.leftIndent;
            }
        }
    }

    export class IncrementParagraphIndentFromFirstRowCommand extends ChangeParagraphIndentFromFirstRowCommandBase {
        getParagraphLeftIndentCommand(): ICommand {
            return this.control.commandManager.getCommand(RichEditClientCommand.IncrementParagraphLeftIndent);
        }
        getMaxFirstLineIndent(paragraph: Paragraph): number {
            var subDocument = this.control.model.activeSubDocument;
            var logPosition = paragraph.startLogPosition.value;
            var layoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, logPosition, DocumentLayoutDetailsLevel.Column)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, logPosition, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Column))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
            if(layoutPosition)
                return UnitConverter.pixelsToTwips(layoutPosition.column.width);
            else {
                var section = this.control.model.getSectionByPosition(paragraph.startLogPosition.value);
                return UnitConverter.pixelsToTwips(this.columnsCalculator.findMinimalColumnSize(section.sectionProperties).width);
            }
        }
        getFirstLineIndent(paragraph: Paragraph, tabs: number[]): number {
            var paragraphProperties = paragraph.getParagraphMergedProperies();
            var firstLineIndentAbsPosition = this.getFirstLineIndentAbsPosition(paragraphProperties);
            var nearestRightTab = this.getNearRightTab(firstLineIndentAbsPosition, tabs);
            var nearestDefaultTab = this.getNearRightDefaultTab(firstLineIndentAbsPosition);
            if(nearestRightTab > firstLineIndentAbsPosition)
                return Math.min(nearestRightTab, nearestDefaultTab) - paragraphProperties.leftIndent;
            return nearestDefaultTab - paragraphProperties.leftIndent;
        }
        needUpdateFirstLineIndent(paragraph: Paragraph, tabs: number[]): boolean {
            var paragraphProperties = paragraph.getParagraphMergedProperies();
            var currentIndent: number = paragraphProperties.leftIndent;
            var rightDefaultTab = this.getNearRightDefaultTab(paragraphProperties.leftIndent + paragraphProperties.firstLineIndentType);
            switch(paragraphProperties.firstLineIndentType) {
                case ParagraphFirstLineIndent.Indented:
                    currentIndent += paragraphProperties.firstLineIndent;
                    break;
                case ParagraphFirstLineIndent.Hanging:
                    currentIndent -= paragraphProperties.firstLineIndent;
            }
            return currentIndent < rightDefaultTab;
        }
    }

    export class DecrementParagraphIndentFromFirstRowCommand extends ChangeParagraphIndentFromFirstRowCommandBase {
        getMaxFirstLineIndent(paragraph: Paragraph): number {
            return Number.MAX_VALUE;
        }
        getFirstLineIndent(paragraph: Paragraph, tabs: number[]): number {
            var paragraphProperties = paragraph.getParagraphMergedProperies();
            var firstLineIndentAbsPosition = this.getFirstLineIndentAbsPosition(paragraphProperties);
            var nearestLeftTab = this.getNearLeftTab(firstLineIndentAbsPosition, tabs);
            var nearestDefaultTab = this.getNearLeftDefaultTab(firstLineIndentAbsPosition);
            return Math.max(0, Math.max(nearestLeftTab, nearestDefaultTab) - paragraphProperties.leftIndent);
        }
        needUpdateFirstLineIndent(paragraph: Paragraph, tabs: number[]): boolean {
            var paragraphProperties = paragraph.getParagraphMergedProperies();
            return paragraphProperties.firstLineIndentType === ParagraphFirstLineIndent.Indented;
        }
        getParagraphLeftIndentCommand(): ICommand {
            return this.control.commandManager.getCommand(RichEditClientCommand.DecrementParagraphLeftIndent);
        }
    }
} 