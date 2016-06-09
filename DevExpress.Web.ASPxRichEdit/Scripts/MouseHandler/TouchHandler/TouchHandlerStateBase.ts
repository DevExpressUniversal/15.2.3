module __aspxRichEdit {
    export class TouchHandlerStateBase {
        TOUCH_SCROLL_SENSITIVITY_IN_ROWS = 2;

        handler: TouchHandler;
        private lastLayoutPosition: LayoutPosition;

        constructor(handler: TouchHandler) {
            this.handler = handler;
        }
        onTouchStart(evt: RichMouseEvent) { }
        onTouchEnd(evt: RichMouseEvent) { }
        onTouchMove(evt: RichMouseEvent): boolean { return true; }

        captureInputFocus(evt: RichMouseEvent) {
            if (this.needUpdateInputPosition(evt))
                this.handler.control.setInputTargetPosition(0, evt.evtY);
            if (this.handler.control.selection.isInFocus())
                this.handler.control.clearKeyboardTips();
            this.handler.control.captureFocus();
        }
        showPopupMenu(): void { //TODO
            var touchBars = document.querySelectorAll(".dxre-sel-bar");
            var element = <HTMLElement>touchBars[1];
            window.setTimeout(() => this.handler.control.showPopupMenu(ASPx.GetAbsolutePositionX(element) + 25, ASPx.GetAbsolutePositionY(element)), 20);
        }
        setLastLayoutPosition() {
            var subDocument = this.handler.control.model.activeSubDocument;
            var logPosition = this.handler.control.selection.getLastSelectedInterval().start;
            this.lastLayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.handler.control.layout, subDocument, logPosition, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(this.handler.control.layout, subDocument, logPosition, this.handler.control.selection.pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(this.handler.control.selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(true));
        }
        private needUpdateInputPosition(evt: RichMouseEvent): boolean {
            var htr: HitTestResult = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Max, this.handler.control.model.activeSubDocument);
            if(htr && this.lastLayoutPosition)
                return !this.handler.control.selection.isInFocus() || Math.abs(htr.rowIndex - this.lastLayoutPosition.rowIndex) > this.TOUCH_SCROLL_SENSITIVITY_IN_ROWS;
            return false;
        }

        start() { }
        finish() { }
    }

    export class TouchHandlerBeginDragHelperState extends TouchHandlerStateBase {
        dragState: TouchHandlerStateBase;

        constructor(handler: TouchHandler, dragState: TouchHandlerStateBase) {
            super(handler);
            this.dragState = dragState;
        }

        start() {
            this.handler.control.bars.beginUpdate();
            this.handler.control.horizontalRulerControl.beginUpdate();
        }
        finish() {
            this.handler.control.bars.endUpdate();
            this.handler.control.horizontalRulerControl.endUpdate();
            this.handler.control.bars.updateItemsState();
            this.handler.control.horizontalRulerControl.update();
        }

        onTouchMove(evt: RichMouseEvent): boolean {
            this.handler.switchState(this.dragState);
            this.dragState.onTouchMove(evt);
            return false;
        }

        onTouchEnd(evt: RichMouseEvent) {
            this.handler.switchToDefaultState();
            this.handler.onTouchEnd(evt);
        }
    }
}