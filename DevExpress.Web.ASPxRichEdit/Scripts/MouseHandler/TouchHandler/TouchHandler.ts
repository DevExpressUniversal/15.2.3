module __aspxRichEdit {
    export class TouchHandler extends ManipulatorHandlerBase {
        private defaultState = new TouchHandlerDefaultState(this);
        state: TouchHandlerStateBase;
        control: IRichEditControl;
        dragCaretVisualizer: DragCaretVisualizer;
        resizeBoxVisualizer: ResizeBoxVisualizer;

        constructor(control: IRichEditControl, dragCaretVisualizer: DragCaretVisualizer, resizeBoxVisualizer: ResizeBoxVisualizer) {
            super(control, dragCaretVisualizer, resizeBoxVisualizer);
            this.switchToDefaultState();
        }
        switchState(state: TouchHandlerStateBase) {
            if (this.state)
                this.state.finish();
            this.state = state;
            this.state.start();
        }
        onTouchStart(evt: RichMouseEvent) {
            this.state.onTouchStart(evt);
        }
        onTouchEnd(evt: RichMouseEvent) {
            this.state.onTouchEnd(evt);
        }
        onTouchMove(evt: RichMouseEvent): boolean {
            return this.state.onTouchMove(evt);
        }
        onGestureStart(evt: MouseEvent) {
            this.switchToDefaultState();
        }
        switchToDefaultState() {
            this.switchState(this.defaultState);
        }
    }
}