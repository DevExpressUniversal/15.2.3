module __aspxRichEdit {
    export class TouchHandlerDragContentState extends TouchHandlerStateBase {
        constructor(handler: TouchHandler) {
            super(handler);
        }
        finish() {
            this.handler.dragCaretVisualizer.hide();
        }
        onTouchMove(evt: RichMouseEvent): boolean {
            this.continueDrag(evt);
            return false;
        }
        onTouchEnd(evt: RichMouseEvent) {
            this.commitDrag(evt);
            this.handler.switchToDefaultState();
        }
        continueDrag(evt: RichMouseEvent) {
            var htr = this.calculateHitTest(evt);
            if (htr.detailsLevel >= DocumentLayoutDetailsLevel.Character)
                this.handler.dragCaretVisualizer.show(htr);
        }
        commitDrag(evt: RichMouseEvent) {
            var htr = this.calculateHitTest(evt);
            if (htr) {
                if (htr.detailsLevel >= DocumentLayoutDetailsLevel.Character) {
                    var layout: DocumentLayout = this.handler.control.layout;
                    var subDocument: SubDocument = this.handler.control.model.activeSubDocument;
                    var interval: FixedInterval = new FixedInterval(htr.getPosition(), 0);
                    Field.correctIntervalDueToFields(layout, subDocument, interval);

                    var commandId: number = RichEditClientCommand.DragMoveContent;
                    this.handler.control.commandManager.getCommand(commandId).execute(interval.start);
                }
            }
        }
        calculateHitTest(evt: RichMouseEvent): HitTestResult {
            const htr: HitTestResult = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Character, this.handler.control.model.activeSubDocument)
            if(htr)
                htr.correctAsVisibleBox();
            return htr;
        }
    }
} 