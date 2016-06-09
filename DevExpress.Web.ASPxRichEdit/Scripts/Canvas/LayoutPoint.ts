module __aspxRichEdit {
    export class LayoutPoint {
        public pageIndex: number;
        public pageX: number;
        public pageY: number;

        constructor(pageIndex: number, pageX: number, pageY: number) {
            this.pageIndex = pageIndex;
            this.pageX = pageX;
            this.pageY = pageY;
        }

        isEmpty(): boolean {
            return this.pageIndex == -1 && this.pageX == -1 && this.pageY == -1;
        }

        static Empty(): LayoutPoint {
            return new LayoutPoint(-1, -1, -1);
        }
    }
}