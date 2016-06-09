module __aspxRichEdit {
    export class ParagraphIndentCommandBase extends CommandBase<SimpleCommandState> { // abstract
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.paragraphFormatting);
        }
        getTabs(paragraphIndices: number[]): number[]{
            let endParagraphIndex = paragraphIndices[paragraphIndices.length - 1];
            let startParagraphIndex = paragraphIndices[0];
            let firstParagraph = this.control.model.activeSubDocument.paragraphs[startParagraphIndex];
            let tabInfos = firstParagraph.getTabs();
            let result: number[] = [];
            for(var i = 0, tabInfo: TabInfo; tabInfo = tabInfos[i]; i++) {
                result.push(tabInfo.position);
            }
            if(paragraphIndices[0] === 0 && paragraphIndices.length === 1)
                result = result.concat(this.getParagraphTabs(firstParagraph));
            else {
                if(paragraphIndices[0] > 0)
                    result = result.concat(this.getParagraphTabs(this.control.model.activeSubDocument.paragraphs[startParagraphIndex - 1]));
                if(endParagraphIndex < this.control.model.activeSubDocument.paragraphs.length - 1)
                    result = result.concat(this.getParagraphTabs(this.control.model.activeSubDocument.paragraphs[endParagraphIndex + 1]));
            }
            result.sort();
            return result;
        }
        getNearRightDefaultTab(leftIndent: number): number {
            var defTabWidth = this.control.model.defaultTabWidth;
            return Math.floor((leftIndent / defTabWidth) + 1) * defTabWidth;
        }
        getNearLeftDefaultTab(leftIndent: number): number {
            var defTabWidth = this.control.model.defaultTabWidth;
            var nearestLeftDefaultTab = Math.floor(leftIndent / defTabWidth);
            if(nearestLeftDefaultTab > 0) {
                if(leftIndent % defTabWidth != 0)
                    return nearestLeftDefaultTab * defTabWidth;
                else
                    return (nearestLeftDefaultTab - 1) * defTabWidth;
            }
            return nearestLeftDefaultTab;
        }
        getNearRightTab(leftIndent: number, tabs: number[]): number {
            for(var i = 0; i < tabs.length; i++) {
                if(leftIndent < tabs[i])
                    return tabs[i];
            }
            return leftIndent;
        }
        getNearLeftTab(leftIndent: number, tabs: number[]) {
            for(var i = tabs.length - 1; i >= 0; i--) {
                if(leftIndent > tabs[i])
                    return tabs[i];
            }
            return leftIndent;
        }
        areParagraphsInSameNumberingList(startParagraphIndex: number, endParagraphIndex: number): boolean {
            var abstractNumberingListIndex = this.control.model.activeSubDocument.paragraphs[startParagraphIndex].getAbstractNumberingListIndex();
            for(var i = startParagraphIndex + 1; i <= endParagraphIndex; i++) {
                var paragraph = this.control.model.activeSubDocument.paragraphs[i];
                if(!paragraph.isInList() || paragraph.getAbstractNumberingListIndex() !== abstractNumberingListIndex)
                    return false;
            }
            return true;
        }
        private getParagraphTabs(paragraph: Paragraph): number[] {
            var result: number[] = [];
            var mergedProperties = paragraph.getParagraphMergedProperies();
            result.push(mergedProperties.leftIndent);
            if(mergedProperties.firstLineIndentType === ParagraphFirstLineIndent.Hanging)
                result.push(mergedProperties.leftIndent - mergedProperties.firstLineIndent);
            else if(mergedProperties.firstLineIndentType === ParagraphFirstLineIndent.Indented)
                result.push(mergedProperties.leftIndent + mergedProperties.firstLineIndent);
            return result;
        }
    }
}