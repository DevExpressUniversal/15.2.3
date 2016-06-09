module __aspxRichEdit {
    export class BoxAligner {
        public static findLastVisibleBoxIndex(boxes: LayoutBox[]): number {
            for (let i = boxes.length - 1, box: LayoutBox; box = boxes[i]; i--)
                if (box.isVisibleForRowAlign())
                    return i;
            return -1;
        }

        public static align(row: LayoutRow, alignment: ParagraphAlignment, paragraphEndXPosition: number) {
            paragraphEndXPosition -= row.x;
            switch (alignment) {
                case ParagraphAlignment.Right:
                    BoxAligner.alignRightCenter(BoxAligner.getBoxes(row), paragraphEndXPosition, 1);
                    break;
                case ParagraphAlignment.Center:
                    BoxAligner.alignRightCenter(BoxAligner.getBoxes(row), paragraphEndXPosition, 2);
                    break;
                case ParagraphAlignment.Justify:
                    BoxAligner.alignJustify(BoxAligner.getBoxes(row), paragraphEndXPosition);
                    break;
                default:
                    return;
            }
            row.width = row.boxes[row.boxes.length - 1].getRightBoundPosition();
        }

        private static getBoxes(row: LayoutRow): LayoutBox[] {
            if (!row.numberingListBox)
                return row.boxes;
            let boxes: LayoutBox[] = row.boxes.slice();
            if (row.numberingListBox.separatorBox)
                boxes.unshift(row.numberingListBox.separatorBox);
            boxes.unshift(row.numberingListBox.textBox);
            return boxes;
        }

        private static alignRightCenter(boxes: LayoutBox[], paragraphEndXPosition: number, divider: number) {
            let avaliableWidth: number = BoxAligner.calculateFreeSpace(boxes, BoxAligner.findLastVisibleBoxIndex(boxes), paragraphEndXPosition);
            if (divider > 1)
                avaliableWidth = Math.floor(avaliableWidth / divider);
            if (avaliableWidth > 0)
                for (let box of boxes)
                    box.x += avaliableWidth;
        }

        private static alignJustify(boxes: LayoutBox[], paragraphEndXPosition: number) {
            switch (boxes[boxes.length - 1].getType()) {
                case LayoutBoxType.ParagraphMark:
                case LayoutBoxType.ColumnBreak:
                case LayoutBoxType.PageBreak:
                case LayoutBoxType.SectionMark:
                    return;
            }

            const prevBox: LayoutBox = boxes[boxes.length - 2];
            const prevPrevBox: LayoutBox = boxes[boxes.length - 3];
            const lastVisibleBoxIndex: number = BoxAligner.findLastVisibleBoxIndex(boxes);
            
            if (prevBox && (prevBox.getType() == LayoutBoxType.ParagraphMark || prevBox.getType() == LayoutBoxType.PageBreak) ||
                prevBox && prevBox.getType() == LayoutBoxType.PageBreak ||
                lastVisibleBoxIndex < 0)
                return;

            const firstNonSpaceBoxIndex: number = BoxAligner.firstNonSpaceBoxIndex(boxes);
            if (firstNonSpaceBoxIndex < 0)
                return;

            let totalSpaceWidth: number = 0;
            for (let i: number = firstNonSpaceBoxIndex + 1; i <= lastVisibleBoxIndex; i++) {
                const box: LayoutBox = boxes[i];
                if (box.getType() == LayoutBoxType.Space)
                    totalSpaceWidth += box.width;
            }
            const freeSpace: number = BoxAligner.calculateFreeSpace(boxes, lastVisibleBoxIndex, paragraphEndXPosition);
            if (totalSpaceWidth == 0 || freeSpace <= 0)
                return;

            let leftX: number = boxes[firstNonSpaceBoxIndex].getRightBoundPosition();
            for (let i: number = firstNonSpaceBoxIndex + 1; i <= lastVisibleBoxIndex; i++) {
                const box: LayoutBox = boxes[i];
                box.x = leftX;
                if (box.getType() == LayoutBoxType.Space)
                    box.width += Math.floor((freeSpace * box.width) / totalSpaceWidth);
                leftX += box.width;
            }
            for (let i: number = lastVisibleBoxIndex + 1, box: LayoutBox; box = boxes[i]; i++) {
                box.x = leftX;
                leftX += box.width;
            }
        }

        private static calculateFreeSpace(boxes: LayoutBox[], lastVisibleBoxIndex: number, paragraphEndXPosition: number): number {
            return paragraphEndXPosition - (lastVisibleBoxIndex >= 0 ? boxes[lastVisibleBoxIndex].getRightBoundPosition() : boxes[0].x);
        }

        private static firstNonSpaceBoxIndex(boxes: LayoutBox[]) {
            let lastNonSpace: number = -1;
            for (let i: number = boxes.length - 1; i >= 0; i--)
                switch (boxes[i].getType()) {
                    case LayoutBoxType.TabSpace: return lastNonSpace;
                    case LayoutBoxType.Space: break;
                    default: lastNonSpace = i;
                }
            return lastNonSpace;
        }
    }
}