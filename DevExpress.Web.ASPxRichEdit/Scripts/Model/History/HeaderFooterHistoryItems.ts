module __aspxRichEdit {
    export class ChangeHeaderFooterIndexHistoryItemBase<T extends HeaderFooterSubDocumentInfoBase> extends HistoryItem {
        sectionIndex: number;
        type: HeaderFooterType;
        newIndex: number;
        oldIndex: number;

        constructor(modelManipulator: ModelManipulator, sectionIndex: number, type: HeaderFooterType, newIndex: number) {
            super(modelManipulator, null);
            this.sectionIndex = sectionIndex;
            this.type = type;
            this.newIndex = newIndex;
        }
        public redo() {
            this.oldIndex = this.getManipulator().changeObjectIndex(this.sectionIndex, this.type, this.newIndex);
        }
        public undo() {
            this.getManipulator().changeObjectIndex(this.sectionIndex, this.type, this.oldIndex);
        }
        protected getManipulator(): HeaderFooterManipulatorBase<T> { // ABSTRACT
            throw new Error(Errors.NotImplemented);
        }
    }

    export class ChangeHeaderIndexHistoryItem extends ChangeHeaderFooterIndexHistoryItemBase<HeaderSubDocumentInfo> {
        protected getManipulator(): HeaderFooterManipulatorBase<HeaderSubDocumentInfo> {
            return this.modelManipulator.headerManipulator;
        }
    }

    export class ChangeFooterIndexHistoryItem extends ChangeHeaderFooterIndexHistoryItemBase<FooterSubDocumentInfo> {
        protected getManipulator(): HeaderFooterManipulatorBase<FooterSubDocumentInfo> {
            return this.modelManipulator.footerManipulator;
        }
    }
}