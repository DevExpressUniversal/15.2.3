module __aspxRichEdit {
    export class MouseHandlerStateBase {
        handler: MouseHandler;
        constructor(handler: MouseHandler) {
            this.handler = handler;
        }
        onMouseDoubleClick(evt: RichMouseEvent) { }
        onMouseDown(evt: RichMouseEvent) { }
        onMouseUp(evt: RichMouseEvent) { }
        onMouseMove(evt: RichMouseEvent) { }
        onMouseWheel(evt: RichMouseEvent) { }
        onShortcut(shortcutCode: number) { }

        start() { }
        finish() { }
    }

    export class MouseHandlerBeginDragHelperState extends MouseHandlerStateBase {
        dragState: MouseHandlerStateBase;

        constructor(handler: MouseHandler, dragState: MouseHandlerStateBase) {
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

        cancelOnRightMouseUp(): boolean { return true; }

        onMouseWheel(evt: RichMouseEvent) {
            this.handler.switchState(this.dragState);
            this.dragState.onMouseWheel(evt);
        }
        onMouseMove(evt: RichMouseEvent) {
            this.handler.switchState(this.dragState);
            this.dragState.onMouseMove(evt);
        }

        onMouseUp(evt: RichMouseEvent) {
            if (evt.button == MouseButton.Left || (this.cancelOnRightMouseUp() && evt.button & MouseButton.Right)) {
                this.handler.switchToDefaultState();
                this.handler.onMouseUp(evt);
            }
        }
    }
}