module __aspxRichEdit {
    // always SET selection and delete old selection
    export class SelectParagraphCommand extends SelectionCommandBase {
        executeCore(state: ICommandState, position: number): boolean {
            var paragraphs: Paragraph[] = this.control.model.activeSubDocument.paragraphs;
            var paragraphIndex: number = Utils.normedBinaryIndexOf(paragraphs, (p: Paragraph) => p.startLogPosition.value - position);
            var paragraph: Paragraph = paragraphs[paragraphIndex];
            this.control.selection.setSelection(paragraph.startLogPosition.value, paragraph.startLogPosition.value + paragraph.length, true, -1, UpdateInputPositionProperties.Yes);
            return true;
        }
    }
} 