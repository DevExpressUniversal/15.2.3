module __aspxRichEdit {
    export interface IEventManager {
        onMouseDown(evt: RichMouseEvent);
        onMouseMove(evt: RichMouseEvent);
        onMouseUp(evt: RichMouseEvent);
        onMouseDblClick(evt: RichMouseEvent);
        onMouseWheel(evt: RichMouseEvent);
        onTouchStart(evt: RichMouseEvent);
        onTouchEnd(evt: RichMouseEvent);
        onTouchMove(evt: RichMouseEvent): boolean;
        onGestureStart(evt: MouseEvent);
        onText(text: string, isUpdated: boolean);
        onShortcut(shortcutCode: number);
        isShortcut(shortcutCode: number): boolean;
        isClipboardCommandShortcut(shortcutCode: number): boolean;
        isFocused(): boolean;

        onFocusIn();
        onFocusOut();
    }
}