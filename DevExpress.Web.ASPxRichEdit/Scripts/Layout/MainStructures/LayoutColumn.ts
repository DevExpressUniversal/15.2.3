module __aspxRichEdit {
    export class LayoutColumn extends LayoutItemBase {
        public rows: LayoutRow[] = [];//coordinates relative to column
        public pageAreaOffset: number;

        public paragraphFrames: ParagraphFrame[] = []; //coordinates relative to column

        public tablesInfo: LayoutTableColumnInfo[] = [];

        public getEndPosition(): number {
            return this.pageAreaOffset + this.rows[this.rows.length - 1].getEndPosition();
        }

        public getLastRow(): LayoutRow {
            return this.rows[this.rows.length - 1];
        }

        public static findSectionColumnWithMinimumWidth(columnBounds: Rectangle[]): number {
            let minCelWidth: number = columnBounds[0].width;
            for (let colIndex: number = 1, colBounds: Rectangle; colBounds = columnBounds[colIndex]; colIndex++)
                minCelWidth = Math.min(minCelWidth, colBounds.width);
            return minCelWidth;
        }
    }

    export class ParagraphFrame extends Rectangle implements IEquatable<ParagraphFrame> {
        public paragraphColor: number = 0;

        equals(obj: ParagraphFrame): boolean {
            return super.equals(obj) && 
                this.paragraphColor == obj.paragraphColor;
        }
    }
} 