module __aspxRichEdit {
    export class MouseHandlerBeginContentDragHelperState extends MouseHandlerBeginDragHelperState {
        constructor(handler: MouseHandler, dragState: MouseHandlerStateBase) {
            super(handler, dragState);
        }
        resetSelectionOnMouseUp: boolean = true;
        onMouseUp(evt: RichMouseEvent) {
            super.onMouseUp(evt);
            if(this.cancelOnRightMouseUp() && evt.button & MouseButton.Right)
                return;
            if(this.resetSelectionOnMouseUp) {
                var htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Character, this.handler.control.model.activeSubDocument);
                htr.correctAsVisibleBox();
                if(htr) {
                    var selection: Selection = this.handler.control.selection;
                    var position: number = htr.getPosition();
                    selection.setSelection(position, position, false, -1, UpdateInputPositionProperties.Yes);
                }
            }
        }
    }

    export class MouseHandlerCancellableDragStateBase extends MouseHandlerStateBase {
        onShortcut(shortcutCode: number) {
            if(shortcutCode === KeyCode.Escape)
                this.handler.switchToDefaultState();
        }
        calculateHitTest(evt: RichMouseEvent): HitTestResult {
            const htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Character, this.handler.control.model.activeSubDocument);
            htr.correctAsVisibleBox();
            return htr;
        }
        onMouseMove(evt: RichMouseEvent) {
            this.continueDrag(evt);
        }
        onMouseWheel(evt: RichMouseEvent) {
            this.continueDrag(evt);
        }
        onMouseUp(evt: RichMouseEvent) {
            this.commitDrag(evt);
            this.handler.switchToDefaultState();
        }
        continueDrag(evt: RichMouseEvent) { }
        commitDrag(evt: RichMouseEvent) { }
    }

    export class MouseHandlerDragContentState extends MouseHandlerCancellableDragStateBase {
        private allowedToDrag: boolean = this.handler.control.commandManager.getCommand(RichEditClientCommand.DragMoveContent).getState().enabled;

        constructor(handler: MouseHandler) {
            super(handler);
        }
        start() {
            let cursorPointer: CursorPointer = this.allowedToDrag ? CursorPointer.Default : CursorPointer.NoDrop;
            this.handler.setCursorPointer(cursorPointer);
        }
        finish() {
            this.handler.restoreCursorPointer();
            this.handler.dragCaretVisualizer.hide();
        }
        continueDrag(evt: RichMouseEvent) {
            if(this.allowedToDrag) {
                var htr = this.calculateHitTest(evt);
                if(htr.detailsLevel >= DocumentLayoutDetailsLevel.Character)
                    this.handler.dragCaretVisualizer.show(htr);
            }
        }
        commitDrag(evt: RichMouseEvent) {
            if(this.allowedToDrag) {
                var htr = this.calculateHitTest(evt);
                if(htr) {
                    if(htr.detailsLevel >= DocumentLayoutDetailsLevel.Character) {
                        var layout: DocumentLayout = this.handler.control.layout;
                        var subDocument: SubDocument = this.handler.control.model.activeSubDocument;
                        var interval: FixedInterval = new FixedInterval(htr.getPosition(), 0);
                        Field.correctIntervalDueToFields(layout, subDocument, interval, htr.pageIndex);

                        var commandId: number = evt.modifiers & KeyModifiers.Ctrl ? RichEditClientCommand.DragCopyContent : RichEditClientCommand.DragMoveContent;
                        this.handler.control.commandManager.getCommand(commandId).execute(interval.start);
                    }
                }
            }
        }
    }
} 