module __aspxRichEdit {
    export class ChangeParagraphLeftIndentCommand extends ParagraphIndentCommandBase {
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
            var result = false;
            this.control.history.beginTransaction();
            var tabs = this.getTabs(paragraphIndices);
            let paragraphIndicesLength = paragraphIndices.length;
            for(let i = 0; i < paragraphIndicesLength; i++) {
                let paragraphIndex = paragraphIndices[i];
                result = this.applyLeftIndentToParagraph(this.control.model.activeSubDocument.paragraphs[paragraphIndex], tabs) || result;
            }
            this.control.history.endTransaction();
            return result;
        }

        applyLeftIndentToParagraph(paragraph: Paragraph, tabs: number[]): boolean {
            var newLeftIndent = this.getNewLeftIndent(paragraph, tabs);
            var maxLeftIndent = this.getMaxLeftIndent(paragraph);
            var parInterval = paragraph.getInterval();
            if(newLeftIndent === paragraph.getParagraphMergedProperies().leftIndent)
                return false;
            if(newLeftIndent >= 0) {
                if(paragraph.getParagraphMergedProperies().firstLineIndentType === ParagraphFirstLineIndent.Hanging) {
                    var firstLineLeftIndent = newLeftIndent - paragraph.getParagraphMergedProperies().firstLineIndent;
                    if(firstLineLeftIndent < 0)
                        newLeftIndent -= firstLineLeftIndent;
                }
            }
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            this.control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, parInterval, newLeftIndent, true));
            if(paragraph.getParagraphMergedProperies().firstLineIndentType == ParagraphFirstLineIndent.Indented) {
                var distanceToRight = maxLeftIndent - (paragraph.getParagraphMergedProperies().leftIndent + paragraph.getParagraphMergedProperies().firstLineIndent);
                if(distanceToRight < 0) {
                    this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, parInterval,
                        paragraph.getParagraphMergedProperies().firstLineIndent + distanceToRight, true));
                }
            }
            return true;
        }
        getNewLeftIndent(paragraph: Paragraph, tabs: number[]): number {
            throw new Error(Errors.NotImplemented);
        }
        getMaxLeftIndent(paragraph: Paragraph): number {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class IncrementParagraphLeftIndentCommand extends ChangeParagraphLeftIndentCommand {
        getNewLeftIndent(paragraph: Paragraph, tabs: number[]): number {
            var paragraphProperties = paragraph.getParagraphMergedProperies();
            var nearRightDefaultTab = this.getNearRightDefaultTab(paragraphProperties.leftIndent);
            var nearRightTab = this.getNearRightTab(paragraphProperties.leftIndent, tabs);
            var result = (nearRightDefaultTab < nearRightTab || nearRightTab === paragraphProperties.leftIndent) ? nearRightDefaultTab : nearRightTab;

            var position = this.getPosition(paragraph);
            if(!position) {
                var section = this.control.model.getSectionByPosition(paragraph.startLogPosition.value);
                var minimalColumnSize = this.columnsCalculator.findMinimalColumnSize(section.sectionProperties);
                return Math.min(result, UnitConverter.pixelsToTwips(minimalColumnSize.width));
            }
            return Math.min(result, UnitConverter.pixelsToTwips(position.column.width));
        }
        getMaxLeftIndent(paragraph: Paragraph): number {
            var position = this.getPosition(paragraph);
            if(position)
                return UnitConverter.pixelsToTwips(position.column.width);
            else {
                var section = this.control.model.getSectionByPosition(paragraph.startLogPosition.value);
                return UnitConverter.pixelsToTwips(this.columnsCalculator.findMinimalColumnSize(section.sectionProperties).width);
            }
        }

        private getPosition(paragraph: Paragraph): LayoutPosition {
            var subDocument = this.control.model.activeSubDocument;
            return (subDocument.isMain() ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, paragraph.startLogPosition.value, DocumentLayoutDetailsLevel.Column)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, paragraph.startLogPosition.value, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Column))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
        }
    }

    export class DecrementParagraphLeftIndentCommand extends ChangeParagraphLeftIndentCommand {
        getNewLeftIndent(paragraph: Paragraph, tabs: number[]): number {
            var paragraphProperties = paragraph.getParagraphMergedProperies();
            var nearLeftDefaultTab = this.getNearLeftDefaultTab(paragraphProperties.leftIndent);
            var nearLeftTab = this.getNearLeftTab(paragraphProperties.leftIndent, tabs);
            return (nearLeftDefaultTab > nearLeftTab || nearLeftTab == paragraphProperties.leftIndent) ? nearLeftDefaultTab : nearLeftTab;
        }
        getMaxLeftIndent(paragraph: Paragraph): number {
            return Number.MAX_VALUE;
        }
    }
} 