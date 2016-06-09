module __aspxRichEdit {
    export class DocumentPropertiesManipulator {
        manipulator: ModelManipulator;

        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        setDefaultTabWidth(subDocument: SubDocument, newDefaultTabWidth: number): number {
            var oldValue: number = subDocument.documentModel.defaultTabWidth;
            subDocument.documentModel.defaultTabWidth = newDefaultTabWidth;
            this.manipulator.dispatcher.notifyDefaultTabWidthChanged(newDefaultTabWidth);
            return oldValue;
        }
        changePageColor(subDocument: SubDocument, newPageColor: number): number {
            var oldValue: number = subDocument.documentModel.pageBackColor;
            subDocument.documentModel.pageBackColor = newPageColor;
            this.manipulator.dispatcher.notifyPageColorChanged(newPageColor);
            return oldValue;
        }

        changeDifferentOddAndEvenPages(documentModel: DocumentModel, newValue: boolean): boolean {
            let oldValue = documentModel.differentOddAndEvenPages;
            documentModel.differentOddAndEvenPages = newValue;
            this.manipulator.dispatcher.notifyDifferentOddAndEvenPagesChanged(newValue);
            return oldValue;
        }
    }
}  