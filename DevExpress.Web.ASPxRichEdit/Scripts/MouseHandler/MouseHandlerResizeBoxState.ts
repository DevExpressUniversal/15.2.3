//TSEmbedded
module __aspxRichEdit {
    export class MouseHandlerResizeBoxState extends MouseHandlerCancellableDragStateBase {
        resizeBoxHelper: ResizeBoxHelper;

        start() {
            this.resizeBoxHelper = new ResizeBoxHelper();
        }
        onMouseDown(evt: RichMouseEvent) {
            var selection: Selection = this.handler.control.selection;
            var subDocument: SubDocument = this.handler.control.model.activeSubDocument;
            this.resizeBoxHelper.start(evt, <InlineObjectRun>subDocument.getRunByPosition(selection.selectedInlinePictureRunPosition));
            this.setCursor();
        }
        onMouseMove(evt: RichMouseEvent) {
            this.resizeBoxHelper.move(evt);
            // undefined - because not need show tip when resize
            this.handler.resizeBoxVisualizer.recalculate(this.resizeBoxHelper.pageIndex, this.resizeBoxHelper.size.width, this.resizeBoxHelper.size.height, this.resizeBoxHelper.sideH, this.resizeBoxHelper.sideV, undefined);
        }
        onMouseUp(evt: RichMouseEvent) {
            super.onMouseUp(evt);
            this.resizeBoxHelper.end(evt, this.handler.control.commandManager.getCommand(RichEditClientCommand.ChangeInlinePictureScale));
            var layoutPosition = this.getPosition();
            this.handler.resizeBoxVisualizer.show(layoutPosition.pageIndex, layoutPosition.pageArea, layoutPosition.column, layoutPosition.row, layoutPosition.box);
        }
        setCursor() {
            if (this.resizeBoxHelper.lockH)
                this.handler.setCursorPointer(CursorPointer.SResize);
            else if (this.resizeBoxHelper.lockV)
                this.handler.setCursorPointer(CursorPointer.WResize);
            else if (this.resizeBoxHelper.sideH && this.resizeBoxHelper.sideV)
                this.handler.setCursorPointer(CursorPointer.SEResize);
            else if (this.resizeBoxHelper.sideH && !this.resizeBoxHelper.sideV)
                this.handler.setCursorPointer(CursorPointer.NEResize);
            else if (!this.resizeBoxHelper.sideH && this.resizeBoxHelper.sideV)
                this.handler.setCursorPointer(CursorPointer.SWResize);
            else if (!this.resizeBoxHelper.sideH && !this.resizeBoxHelper.sideV)
                this.handler.setCursorPointer(CursorPointer.NWResize);
        }
        finish() {
            this.handler.setCursorPointer(CursorPointer.Auto);
            var layoutPosition: LayoutPosition = this.getPosition();
            this.handler.resizeBoxVisualizer.show(layoutPosition.pageIndex, layoutPosition.pageArea, layoutPosition.column, layoutPosition.row, layoutPosition.box);
        }

        private getPosition(): LayoutPosition {
            var subDocument = this.handler.control.model.activeSubDocument;
            var logPosition = this.handler.control.selection.selectedInlinePictureRunPosition;
            return (subDocument.isMain() ? new LayoutPositionMainSubDocumentCreator(this.handler.control.layout, subDocument, logPosition, DocumentLayoutDetailsLevel.Box)
                : new LayoutPositionOtherSubDocumentCreator(this.handler.control.layout, subDocument, logPosition, this.handler.control.selection.pageIndex, DocumentLayoutDetailsLevel.Box))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
        }
    }
} 