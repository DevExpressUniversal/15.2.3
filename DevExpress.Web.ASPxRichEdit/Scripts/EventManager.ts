module __aspxRichEdit {
    export enum MouseEventSource {
        Undefined,
        ResizeBox_NW,
        ResizeBox_NE,
        ResizeBox_SE,
        ResizeBox_SW,
        ResizeBox_N,
        ResizeBox_E,
        ResizeBox_S,
        ResizeBox_W
    }
    export enum KeyModifiers {
        None = 0,
        Shift = 1,
        Ctrl = 2,
        Alt = 4,
        Meta = 6
    }
    export enum MouseButton {
        Left = 1,
        Right = 2
    }
    export enum KeyCode {
        Backspace = 8 << 8,
        Tab = 9 << 8,
        Enter = 13 << 8,
        Shift = 16 << 8,
        Ctrl = 17 << 8,
        Alt = 18 << 8,
        Pause = 19 << 8,
        CapsLock = 20 << 8,
        Escape = 27 << 8,
        Space = 32 << 8,
        PageUp = 33 << 8,
        PageDown = 34 << 8,
        End = 35 << 8,
        Home = 36 << 8,
        Left = 37 << 8,
        Up = 38 << 8,
        Right = 39 << 8,
        Down = 40 << 8,
        Insert = 45 << 8,
        Delete = 46 << 8,
        Key_0 = 48 << 8,
        Key_1 = 49 << 8,
        Key_2 = 50 << 8,
        Key_3 = 51 << 8,
        Key_4 = 52 << 8,
        Key_5 = 53 << 8,
        Key_6 = 54 << 8,
        Key_7 = 55 << 8,
        Key_8 = 56 << 8,
        Key_9 = 57 << 8,
        Key_a = 65 << 8,
        Key_b = 66 << 8,
        Key_c = 67 << 8,
        Key_d = 68 << 8,
        Key_e = 69 << 8,
        Key_f = 70 << 8,
        Key_g = 71 << 8,
        Key_h = 72 << 8,
        Key_i = 73 << 8,
        Key_j = 74 << 8,
        Key_k = 75 << 8,
        Key_l = 76 << 8,
        Key_m = 77 << 8,
        Key_n = 78 << 8,
        Key_o = 79 << 8,
        Key_p = 80 << 8,
        Key_q = 81 << 8,
        Key_r = 82 << 8,
        Key_s = 83 << 8,
        Key_t = 84 << 8,
        Key_u = 85 << 8,
        Key_v = 86 << 8,
        Key_w = 87 << 8,
        Key_x = 88 << 8,
        Key_y = 89 << 8,
        Key_z = 90 << 8,
        Select = 93 << 8,
        Numpad_0 = 96 << 8,
        Numpad_1 = 97 << 8,
        Numpad_2 = 98 << 8,
        Numpad_3 = 99 << 8,
        Numpad_4 = 100 << 8,
        Numpad_5 = 101 << 8,
        Numpad_6 = 102 << 8,
        Numpad_7 = 103 << 8,
        Numpad_8 = 104 << 8,
        Numpad_9 = 105 << 8,
        Multiply = 106 << 8,
        Add = 107 << 8,
        Subtract = 109 << 8,
        Decimal = 110 << 8,
        Divide = 111 << 8,
        F1 = 112 << 8,
        F2 = 113 << 8,
        F3 = 114 << 8,
        F4 = 115 << 8,
        F5 = 116 << 8,
        F6 = 117 << 8,
        F7 = 118 << 8,
        F8 = 119 << 8,
        F9 = 120 << 8,
        F10 = 121 << 8,
        F11 = 122 << 8,
        F12 = 123 << 8,
        NumLock = 144 << 8,
        ScrollLock = 145 << 8,
        Semicolon = 186 << 8,
        Equals = 187 << 8,
        Comma = 188 << 8,
        Dash = 189 << 8,
        Period = 190 << 8,
        ForwardSlash = 191 << 8,
        GraveAccent = 192 << 8,
        OpenBracket = 219 << 8,
        BackSlash = 220 << 8,
        CloseBracket = 221 << 8,
        SingleQuote = 222 << 8
    }

    export class EventManager implements IEventManager {
        control: IRichEditControl;
        mouseHandler: MouseHandler;
        touchHandler: TouchHandler;

        moveLocked: boolean = false;

        constructor(control: IRichEditControl, dragCaretVisualizer: DragCaretVisualizer, resizeBoxVisualizer: ResizeBoxVisualizer) {
            this.control = control;
            this.mouseHandler = new MouseHandler(control, dragCaretVisualizer, resizeBoxVisualizer);
            this.touchHandler = new TouchHandler(control, dragCaretVisualizer, resizeBoxVisualizer);
        }

        onShortcut(shortcutCode: number) {
            this.control.commandManager.processShortcut(shortcutCode);
            this.mouseHandler.onShortcut(shortcutCode);
        }
        isShortcut(shortcutCode: number) {
            return this.control.commandManager.isKnownShortcut(shortcutCode);
        }
        isClipboardCommandShortcut(shortcutCode: number) {
            return this.control.commandManager.isClipboardCommandShortcut(shortcutCode);
        }
        onMouseDown(evt: RichMouseEvent) {
            this.lockMouseMove();
            if(this.control.readOnly === ReadOnlyMode.None)
                this.control.selection.setFocusState(true);
            this.mouseHandler.onMouseDown(evt);
        }
        onMouseMove(evt: RichMouseEvent) {
            if (this.moveLocked)
                return;
            this.mouseHandler.onMouseMove(evt);
        }
        onMouseUp(evt: RichMouseEvent) {
            this.lockMouseMove();
            this.mouseHandler.onMouseUp(evt);
            if (evt.point) {
                this.control.setInputTargetPosition(evt.evtX + 2, evt.evtY + 2);
                setTimeout(() => this.control.captureFocus(), 0);
            }
        }
        onTouchStart(evt: RichMouseEvent) {
            this.touchHandler.onTouchStart(evt);
        }
        onTouchEnd(evt: RichMouseEvent) {
            this.touchHandler.onTouchEnd(evt);
        }
        onTouchMove(evt: RichMouseEvent): boolean {
            return this.touchHandler.onTouchMove(evt);
        }
        onGestureStart(evt: MouseEvent) {
            this.touchHandler.onGestureStart(evt);
        }
        onMouseDblClick(evt: RichMouseEvent) {
            this.mouseHandler.onMouseDoubleClick(evt);
            this.control.setInputTargetPosition(evt.evtX, evt.evtY);
        }
        onMouseWheel(evt: RichMouseEvent) {
            this.mouseHandler.onMouseWheel(evt);
        }
        onText(text: string, isUpdated: boolean) {
            if (isUpdated)
                ModelManipulator.modifyLastInsertedSymbol(this.control, text);
            else {
                var insertTextCommand = this.control.commandManager.getCommand(RichEditClientCommand.InsertText);
                this.control.commandManager.executeCommand(insertTextCommand, text); // todo: optimize it (use cmd.execute instead)
            }
        }
        onFocusIn() {
            if(this.control.readOnly === ReadOnlyMode.None)
                this.control.selection.setFocusState(true);
        }
        onFocusOut() {
            this.control.selection.setFocusState(false);
        }
        isFocused(): boolean {
            return this.control.selection.isInFocus();
        }
        private lockMouseMove() {
            this.moveLocked = true;
            setTimeout(() => this.moveLocked = false, 0); // because chrome give 2 events in click: buttonDown and move.
        }
    }

    export class RichMouseEvent {
        point: LayoutPoint;
        modifiers: KeyModifiers;
        button: MouseButton;
        evtX: number;
        evtY: number;
        source: MouseEventSource;
        scrollTop: number;
        scrollLeft: number;
        constructor(evt: MouseEvent, point: LayoutPoint, source: MouseEventSource, scrollTop: number, scrollLeft: number) {
            this.point = point;
            this.button = this.isLeftButtonPressed(evt) ? MouseButton.Left : MouseButton.Right;
            this.modifiers = Utils.getKeyModifiers(evt);
            this.evtX = ASPx.Evt.GetEventX(evt);
            this.evtY = ASPx.Evt.GetEventY(evt);
            this.source = source;
            this.scrollTop = scrollTop;
            this.scrollLeft = scrollLeft;
        }
        private isLeftButtonPressed(evt: MouseEvent): boolean {
            return !ASPx.Browser.MSTouchUI ? ASPx.Evt.IsLeftButtonPressed(evt) : evt.button != 2;
        }
    }
} 