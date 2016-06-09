module __aspxRichEdit {
    export class MouseHandler extends ManipulatorHandlerBase {
        static LEFTAREA_COMMANDS_OFFSET = 12;
        static WAIT_FOR_DBLCLICK_INTERVAL = 300;

        private defaultState = new MouseHandlerDefaultState(this);
        state: MouseHandlerStateBase;
        control: IRichEditControl;
        dragCaretVisualizer: DragCaretVisualizer;
        resizeBoxVisualizer: ResizeBoxVisualizer;
        
        constructor(control: IRichEditControl, dragCaretVisualizer: DragCaretVisualizer, resizeBoxVisualizer: ResizeBoxVisualizer) {
            super(control, dragCaretVisualizer, resizeBoxVisualizer);
            this.switchToDefaultState();
        }
        switchState(state: MouseHandlerStateBase) {
            if(this.state)
                this.state.finish();
            this.state = state;
            this.state.start();
        }
        onMouseDoubleClick(evt: RichMouseEvent) {
            this.state.onMouseDoubleClick(evt);
        }
        onMouseDown(evt: RichMouseEvent) {
            this.state.onMouseDown(evt);
        }
        onMouseUp(evt: RichMouseEvent) {
            this.state.onMouseUp(evt);
        }
        onMouseMove(evt: RichMouseEvent) {
            this.state.onMouseMove(evt);
        }
        onMouseWheel(evt: RichMouseEvent) {
            this.state.onMouseWheel(evt);
        }
        onShortcut(shortcutCode: number) {
            this.state.onShortcut(shortcutCode);
        }
        switchToDefaultState() {
            this.switchState(this.defaultState);
        }
        setCursorPointer(pointer: CursorPointer) {
            this.control.setCursorPointer(pointer);
        }
        restoreCursorPointer() {
            this.control.setCursorPointer(CursorPointer.Auto);
        }
    }

    export enum CursorPointer {
        Default,
        Move,
        Copy,
        NoDrop,
        EResize,
        NResize,
        SResize,
        WResize,
        SEResize,
        SWResize,
        NWResize,
        NEResize,
        Auto
    }
} 