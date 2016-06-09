module __aspxRichEdit {
    export class ResizeBoxHelper {
        startX: number;
        startY: number;
        startScrollTop: number;
        startScrollLeft: number;
        run: InlineObjectRun;
        pageIndex: number;
        lockH: boolean;
        lockV: boolean;
        sideH: boolean;
        sideV: boolean;
        objWidth: number;
        objHeight: number;
        size: Size;

        start(evt: RichMouseEvent, run: InlineObjectRun) {
            this.startX = evt.evtX;
            this.startY = evt.evtY;
            this.startScrollLeft = evt.scrollLeft;
            this.startScrollTop = evt.scrollTop;
            this.run = run;
            this.lockH = evt.source == MouseEventSource.ResizeBox_S || evt.source == MouseEventSource.ResizeBox_N;
            this.lockV = evt.source == MouseEventSource.ResizeBox_E || evt.source == MouseEventSource.ResizeBox_W;
            this.sideH = evt.source == MouseEventSource.ResizeBox_E || evt.source == MouseEventSource.ResizeBox_NE || evt.source == MouseEventSource.ResizeBox_SE;
            this.sideV = evt.source == MouseEventSource.ResizeBox_SE || evt.source == MouseEventSource.ResizeBox_S || evt.source == MouseEventSource.ResizeBox_SW;
            this.pageIndex = evt.point.pageIndex;
            this.objWidth = UnitConverter.twipsToPixels(this.run.getActualWidth());
            this.objHeight = UnitConverter.twipsToPixels(this.run.getActualHeight());
        }
        move(evt: RichMouseEvent) {
            this.size = this.getSize(evt);
        }
        end(evt: RichMouseEvent, cmd: ICommand) {
            var size: Size = this.getSize(evt);
            var origWidth: number = UnitConverter.twipsToPixels(this.run.originalWidth);
            var origHeight: number = UnitConverter.twipsToPixels(this.run.originalHeight);
            cmd.execute([100 * size.width / origWidth, 100 * size.height / origHeight]);
        }
        private getSize(evt: RichMouseEvent): Size {
            var deltaX = evt.evtX - (this.startScrollLeft - evt.scrollLeft) - this.startX;
            var deltaY = evt.evtY - (this.startScrollTop - evt.scrollTop) - this.startY;
            var newWidth: number,
                newHeight: number;
            deltaY = !this.sideV && deltaY > 0 ? Math.min(this.objHeight + 1, deltaY) : deltaY;
            deltaX = !this.sideH && deltaX > 0 ? Math.min(this.objWidth + 1, deltaX) : deltaX;
            if (!this.lockH && !this.lockV) {
                if (Math.abs(deltaX) > Math.abs(deltaY)) {
                    newWidth = this.sideH ? Math.max(1, this.objWidth + deltaX) : (this.objWidth - deltaX);
                    newHeight = this.objHeight * (newWidth / this.objWidth);
                }
                else {
                    newHeight = this.sideV ? Math.max(1, this.objHeight + deltaY) : (this.objHeight - deltaY);
                    newWidth = this.objWidth * (newHeight / this.objHeight);
                }
            }
            else {
                deltaX = this.lockH ? 0 : deltaX;
                deltaY = this.lockV ? 0 : deltaY;
                newWidth = Math.max(1, this.sideH ? (this.objWidth + deltaX) : (this.objWidth - deltaX));
                newHeight = Math.max(1, this.sideV ? (this.objHeight + deltaY) : (this.objHeight - deltaY));
            }
            return new Size(newWidth, newHeight);
        }
    }
}  