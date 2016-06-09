module __aspxRichEdit {
    export enum LayoutRowStateFlags {
        NormallyEnd = 0x0000, // not breaks on end of row
        ParagraphEnd = 0x0001,
        PageEnd = 0x0002,
        ColumnEnd = 0x0004,
        SectionEnd = 0x0008,
        DocumentEnd = 0x0010,
        NotEnoughChunks = 0x0020, // don't use this row
        CellTableEnd = 0x0040
        // if DocumentEnd=true && ParagraphEnd=false this mean  that last paragraph mark is hidden
    }

    export class LayoutRow extends LayoutItemBase {
        public boxes: LayoutBox[] = []; //coordinates relative to row
        public bookmarkBoxes: BookmarkBox[] = [];
        public numberingListBox: LayoutNumberingListBox = null;
        public tableCellInfo: LayoutTableCellInfo; // can be undefined
        public flags: LayoutRowStateFlags; // result what return textRowFormatter for DocumentFormatter

        public columnOffset: number;
        public baseLine: number; // in pixels

        private spacingBefore: number;
        private spacingAfter: number;

        public getEndPosition(): number {
            return this.columnOffset + this.getLastBoxEndPositionInRow();
        }

        public getLastBoxEndPositionInRow(): number {
            return this.boxes[this.boxes.length - 1].getEndPosition();
        }

        public getLastBox(): LayoutBox {
            return this.boxes[this.boxes.length - 1];
        }

        public isEmpty(): boolean {
            return this.boxes.length == 0;
        }

        public applySpacingBefore(value: number) {
            this.rollbackSpacingBefore();
            this.spacingBefore = value;
            this.height += this.getSpacingBefore();
            this.baseLine += this.getSpacingBefore();
        }

        public rollbackSpacingBefore() {
            this.height = this.height - this.getSpacingBefore();
            this.baseLine = this.baseLine - this.getSpacingBefore();
            this.spacingBefore = 0;
        }

        public applySpacingAfter(value: number) {
            this.rollbackSpacingAfter();
            this.spacingAfter = value;
            this.height += this.getSpacingAfter();
        }

        public rollbackSpacingAfter() {
            this.height = Math.max(0, this.height - this.getSpacingAfter());
            this.spacingAfter = 0;
        }

        public getSpacingBefore(): number {
            return this.spacingBefore || 0;
        }

        public getSpacingAfter(): number {
            return this.spacingAfter || 0;
        }

        public getLastVisibleBox(): LayoutBox {
            var index: number = this.getLastVisibleBoxIndex();
            return index >= 0 ? this.boxes[index] : null;
        }

        public getLastVisibleBoxIndex(): number {
            for (var lastBoxIndexInRow: number = this.boxes.length - 1, box: LayoutBox; box = this.boxes[lastBoxIndexInRow]; lastBoxIndexInRow--)
                if (box.isVisible())
                    return lastBoxIndexInRow;
            return -1;
        }

        public static getParagraphSpacingBefore(paragraph: Paragraph, prevParagraph: Paragraph, unitConverter: IUnitConverter): number {
            const parProps: ParagraphProperties = paragraph.getParagraphMergedProperies();
            const spacingBefore: number = Math.abs(unitConverter.toPixels(parProps.spacingBefore));
            if (!spacingBefore || !prevParagraph)
                return spacingBefore;

            if (parProps.contextualSpacing && paragraph.paragraphStyle.styleName == prevParagraph.paragraphStyle.styleName)
                return 0;

            const prevParProps: ParagraphProperties = prevParagraph.getParagraphMergedProperies();
            const prevParSpacingAfter: number = unitConverter.toPixels(prevParProps.spacingAfter);
            return prevParSpacingAfter >= spacingBefore ? 0 : Math.abs(spacingBefore - prevParSpacingAfter);
        }

        public static getParagraphSpacingAfter(paragraph: Paragraph, nextParagraph: Paragraph, unitConverter: IUnitConverter): number {
            const parProps: ParagraphProperties = paragraph.getParagraphMergedProperies();
            const spacingAfter = Math.abs(unitConverter.toPixels(parProps.spacingAfter));
            if (!spacingAfter || !nextParagraph)
                return spacingAfter;
            return (parProps.contextualSpacing && paragraph.paragraphStyle.styleName == nextParagraph.paragraphStyle.styleName) ? 0 : spacingAfter;
        }
    }

    export class LayoutRowWithIndex extends LayoutRow {
        indexInColumn: number; // in column
    }
} 