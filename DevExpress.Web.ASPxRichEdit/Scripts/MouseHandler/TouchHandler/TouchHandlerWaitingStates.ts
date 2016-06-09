module __aspxRichEdit {
    export class TouchHandlerWaitingStateBase extends TouchHandlerStateBase {
        private timerID: number;
        action: () => void;

        constructor(handler: TouchHandler, interval: number, action: () => void) {
            super(handler);
            this.action = action;
            this.timerID = setTimeout(() => {
                this.timerID = -1;
                this.action();
            }, interval);
        }

        onTouchEnd(evt: RichMouseEvent) {
            this.finish();
        }

        onTouchMove(evt: RichMouseEvent): boolean {
            this.finish();
            this.handler.switchToDefaultState();
            return true;
        }

        finish() {
            if (this.timerID != -1)
                clearTimeout(this.timerID);
        }
    }

    export class TouchHandlerBeginTapProcessingState extends TouchHandlerWaitingStateBase {
        constructor(handler: TouchHandler, evt: RichMouseEvent) {
            super(handler, 500,() => {
                if (this.handler.control.isResizeBoxVisible())
                    this.beginDragExistingSelection();
                else
                    handler.switchState(new TouchHandlerBeginWaitForLongTapState(handler, evt));
            });
        }

        onTouchStart(evt: RichMouseEvent) {
            this.finish();
            this.handler.switchState(new TouchHandlerSelectWordOnDblTapState(this.handler, evt));
        }
        onTouchEnd(evt: RichMouseEvent) {
            var htr: HitTestResult = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Character, this.handler.control.model.activeSubDocument);
            if(htr && this.shouldSelectPicture(htr))
                this.selectImage(htr);
            else if (htr) {
                this.setLastLayoutPosition();
                var position = this.getLayoutPosition(htr);
                var wholeWordInterval: FixedInterval = this.handler.control.model.activeSubDocument.getWholeWordInterval(position);
                if (wholeWordInterval.length > 0) {
                    if (position < wholeWordInterval.start + Math.round(wholeWordInterval.length / 2))
                        position = wholeWordInterval.start;
                    else
                        position = wholeWordInterval.end();
                }
                var endOfLine = position === (htr.pageArea.subDocument.isMain() ? htr.page.getStartOffsetContentOfMainSubDocument() : 0)
                    + htr.pageArea.pageOffset + htr.column.pageAreaOffset + htr.row.getEndPosition();
                this.handler.control.selection.setSelection(position, position, endOfLine, -1, UpdateInputPositionProperties.Yes);
                this.captureInputFocus(evt);
            }

            this.action = () => {
                this.handler.switchToDefaultState();
            };
        }
        private getLayoutPosition(htr: HitTestResult): number {
            var position = (htr.pageArea.subDocument.isMain() ? htr.page.getStartOffsetContentOfMainSubDocument() : 0) + htr.pageArea.pageOffset + htr.column.pageAreaOffset + htr.row.columnOffset;
            if (htr.deviations[DocumentLayoutDetailsLevel.Row] & HitTestDeviation.Right) {
                var lastVisibleBoxInRow: LayoutBox = htr.row.getLastVisibleBox();
                if (lastVisibleBoxInRow)
                    position += lastVisibleBoxInRow.getEndPosition();
            } else {
                position += htr.box.rowOffset + htr.charOffset;
                if (htr.boxIndex == htr.row.boxes.length - 1 && htr.charOffset == htr.box.getLength() && !htr.box.isVisible())
                    position -= 1;
            }
            return position;
        }
        private shouldSelectPicture(htr: HitTestResult): boolean {
            if (htr.detailsLevel < DocumentLayoutDetailsLevel.Box)
                return false;
            return htr.box instanceof LayoutPictureBox;
        }
        private selectImage(htr: HitTestResult) {
            var position = htr.getPosition() - htr.charOffset;
            var selection: Selection = this.handler.control.selection;
            selection.setSelection(position, position + 1, false, -1, UpdateInputPositionProperties.Yes);
        }
        private beginDragExistingSelection() {
            var dragState = new TouchHandlerDragContentState(this.handler);
            var state = new TouchHandlerBeginDragHelperState(this.handler, dragState);
            this.handler.switchState(state);
        }
    }

    export class TouchHandlerBeginWaitForLongTapState extends TouchHandlerWaitingStateBase {
        constructor(handler: TouchHandler, evt: RichMouseEvent) {
            super(handler, 300,() => {
                if (!this.handler.control.selection.isInFocus())
                    handler.switchState(new TouchHandlerSelectWordUnderCursorState(handler, evt));
                else
                    handler.switchState(new TouchHandlerMoveCaretUnderCursorState(handler, evt));
            });
        }
    }
}