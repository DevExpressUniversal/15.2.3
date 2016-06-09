module __aspxRichEdit {
    export class TouchHandlerMoveCaretUnderCursorState extends TouchHandlerStateBase {
        constructor(handler: TouchHandler, evt: RichMouseEvent) {
            super(handler);
            //this.handler.control.showLoupe(evt);
            this.setCaretUnderCursor(evt);
        }

        setCaretUnderCursor(evt: RichMouseEvent) {
            var htr: HitTestResult = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Max, this.handler.control.model.activeSubDocument);
            if (htr && htr.exactlyDetailLevel === DocumentLayoutDetailsLevel.Box) {
                var position = Math.min(htr.getPosition(), htr.subDocument.getDocumentEndPosition() - 1);
                this.handler.control.selection.setSelection(position, position, false, -1, UpdateInputPositionProperties.Yes);
            }
        }

        onTouchMove(evt: RichMouseEvent): boolean {
            this.handler.control.moveLoupe(evt);
            this.setCaretUnderCursor(evt);
            return false;
        }
        onTouchEnd(evt: RichMouseEvent) {
            this.handler.control.hideLoupe();
            this.handler.switchToDefaultState();
            this.showPopupMenu();
        }
    }
}