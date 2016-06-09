module __aspxRichEdit {
    export class TouchHandlerSelectionStateBase extends TouchHandlerStateBase {
        onTouchEnd(evt: RichMouseEvent) {
            this.handler.switchToDefaultState();
            this.setLastLayoutPosition();
            this.captureInputFocus(evt);
            this.showPopupMenu();
        }
    }

    export class TouchHandlerSelectWordOnDblTapState extends TouchHandlerSelectionStateBase {
        constructor(handler: TouchHandler, evt: RichMouseEvent) {
            super(handler);
            //if (this.needShowLoupe())
            //    this.handler.control.showLoupe(evt);
            this.selectWordUnderCursor(evt);
        }

        selectWordUnderCursor(evt: RichMouseEvent) {
            var htr: HitTestResult = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Max, this.handler.control.model.activeSubDocument);
            if(htr) {
                if(htr.exactlyDetailLevel < DocumentLayoutDetailsLevel.PageArea) {
                    let activeSubDocument = this.handler.control.model.activeSubDocument;
                    if(activeSubDocument.isMain()) {
                        if(htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Top) {
                            this.handler.control.commandManager.getCommand(RichEditClientCommand.InsertHeader).execute(htr.pageIndex);
                            return;
                        }
                        else if(htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Bottom) {
                            this.handler.control.commandManager.getCommand(RichEditClientCommand.InsertFooter).execute(htr.pageIndex);
                            return;
                        }
                    }
                    else if(this.shouldActivateMainArea(activeSubDocument, htr)) {
                        this.handler.control.selection.pageIndex = htr.pageIndex;
                        this.handler.control.commandManager.getCommand(RichEditClientCommand.ChangeActiveSubDocument).execute(this.handler.control.model.mainSubDocument.info);
                        this.handler.control.selection.pageIndex = -1;
                        return;
                    }
                } else if(htr.exactlyDetailLevel === DocumentLayoutDetailsLevel.Box) {
                    var position: number = htr.getRelatedSubDocumentPagePosition() + htr.pageArea.pageOffset + htr.column.pageAreaOffset + htr.row.columnOffset + htr.box.rowOffset + htr.charOffset;
                    var startInterval: number = LayoutWordPositionHelper.getPrevWordEndPosition(this.handler.control.layout, this.handler.control.model.activeSubDocument, this.handler.control.selection, position);
                    var endInterval: number = LayoutWordPositionHelper.getNextWordStartPosition(this.handler.control.layout, this.handler.control.model.activeSubDocument, this.handler.control.selection, position, false);
                    if (endInterval - startInterval > 0)
                        this.handler.control.selection.setSelection(startInterval, endInterval, false, -1, UpdateInputPositionProperties.Yes);
                }
            }
        }

        private shouldActivateMainArea(activeSubDocument: SubDocument, htr: HitTestResult): boolean {
            return (activeSubDocument.isHeader() && !!(htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Bottom)) ||
                (activeSubDocument.isFooter() && !!(htr.deviations[DocumentLayoutDetailsLevel.PageArea] & HitTestDeviation.Top));
        }
        //needShowLoupe(): boolean {
        //    return false;
        //}
    }

    export class TouchHandlerSelectWordUnderCursorState extends TouchHandlerSelectWordOnDblTapState {
        onTouchMove(evt: RichMouseEvent): boolean {
            this.handler.control.moveLoupe(evt);
            this.selectWordUnderCursor(evt);
            return false;
        }
        onTouchEnd(evt: RichMouseEvent) {
            super.onTouchEnd(evt);
            this.handler.control.hideLoupe();
        }
        //needShowLoupe(): boolean {
        //    return true;
        //}
    }

    export class TouchHandlerContinueSelectionStateBase extends TouchHandlerSelectionStateBase {
        constructor(handler: TouchHandler) {
            super(handler);
        }
        onTouchMove(evt: RichMouseEvent): boolean {
            this.handler.control.setTouchBarsVisibile(false);
            var htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Character, this.handler.control.model.activeSubDocument);
            if (htr)
                this.extendSelection(htr);
            return false;
        }
        onTouchEnd(evt: RichMouseEvent) {
            super.onTouchEnd(evt);
            this.handler.control.setTouchBarsVisibile(true);
        }

        extendSelection(htr: HitTestResult): void {
        }
    }

    export class TouchHandlerContinueSelectionState extends TouchHandlerContinueSelectionStateBase {
        extendSelection(htr: HitTestResult): void {
            this.handler.control.selection.extendLastSelection(Math.min(htr.getPosition(), htr.subDocument.getDocumentEndPosition() - 1), false, -1, UpdateInputPositionProperties.No);
        }
    }

    export class TouchHandlerContinueSelectionOnOneSideState extends TouchHandlerContinueSelectionStateBase {
        private isDragLeftEdge: boolean = false;

        constructor(handler: TouchHandler, evt: RichMouseEvent, isDragLeftEdge = true) {
            super(handler);
            this.isDragLeftEdge = isDragLeftEdge;
        }

        extendSelection(htr: HitTestResult): void {
            this.handler.control.selection.extendLastSelectionOnOneSide(Math.min(htr.getPosition(), htr.subDocument.getDocumentEndPosition() - 1), false, this.isDragLeftEdge);
        }
    }
}