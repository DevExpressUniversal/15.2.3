module __aspxRichEdit {
    export class LayoutDragCaret extends LayoutItemBase implements IEquatable<LayoutDragCaret> {
        pageIndex: number;
        constructor(pageIndex: number) {
            super();
            this.pageIndex = pageIndex;
            this.width = 1;
        }
        equals(obj: LayoutDragCaret): boolean {
            return this.pageIndex === obj.pageIndex
                && this.height === obj.height
                && this.width === obj.width
                && this.x === obj.x
                && this.y === obj.y;
        }
    }
} 