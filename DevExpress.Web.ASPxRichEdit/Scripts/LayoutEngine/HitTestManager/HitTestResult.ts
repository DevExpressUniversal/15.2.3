module __aspxRichEdit {
    // http://workservices01/OpenWiki/ow.asp?p=ASPxRichEdit_HitTestManager
    export class HitTestResult extends LayoutPositionBase {
        subDocument: SubDocument = null;
        deviations: { [detailLevel: number]: HitTestDeviation } = {}; // deviation - indicates which side we're on the nature of the level of detailLevel. 
                                                                      // In case exact hit here None.
                                                                      // char level absent here. box - is last level
        exactlyDetailLevel: DocumentLayoutDetailsLevel; // in this.detailLevel mean requestDetailLevel. But here - lowest level what we exactly hit
                                                        // Example: we axactly find pageArea, but row ca't find, then here PageArea

        constructor(subDocument: SubDocument) {
            super();
            this.subDocument = subDocument;
        }

        isValid(detailsLevel: DocumentLayoutDetailsLevel): boolean {
            return detailsLevel <= this.detailsLevel;
        }

        correctAsVisibleBox() {
            if (this.box.isVisible())
                return;

            this.deviations[DocumentLayoutDetailsLevel.Box] = undefined;
            this.exactlyDetailLevel = Math.min(this.exactlyDetailLevel, DocumentLayoutDetailsLevel.Row);

            const boxIndex: number = this.row.getLastVisibleBoxIndex();

            this.boxIndex = Math.max(0, boxIndex);
            this.box = this.row.boxes[this.boxIndex];
            this.charOffset = boxIndex < 0 ? 0 : this.box.getLength();
        }

        getPosition(): number {
            var result = 0;
            if (this.page && this.subDocument.isMain())
                result += this.page.getStartOffsetContentOfMainSubDocument();
            if (this.pageArea)
                result += this.pageArea.pageOffset;
            if (this.column)
                result += this.column.pageAreaOffset;
            if (this.row)
                result += this.row.columnOffset;
            if (this.box)
                result += this.box.rowOffset;
            if (this.charOffset >= 0)
                result += this.charOffset;
            return result;
        }
    }

    export enum DocumentLayoutDetailsLevel {
        None = -1,
        Page = 0,
        PageArea = 1,
        Column = 2,
        TableRow = 3,
        TableCell = 4,
        Row = 5,
        Box = 6,
        Character = 7,
        Max = 255
    }

    export enum HitTestDeviation {
        None = 0x00000000,
        Top = 0x00000001,
        Bottom = 0x00000002,
        Left = 0x00000004,
        Right = 0x00000008
    }
} 