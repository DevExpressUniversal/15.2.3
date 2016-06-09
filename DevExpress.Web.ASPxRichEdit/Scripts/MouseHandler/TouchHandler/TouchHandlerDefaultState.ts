module __aspxRichEdit {
    export var TOUCH_RADIUS_HANDLE: number = 15;

    export class TouchHandlerDefaultState extends TouchHandlerStateBase {
        onTouchStart(evt: RichMouseEvent) {
            if (this.shouldProcessResizeBoxVisualizer(evt))
                this.beginResizeBoxTouchHandler(evt);
            else {
                var htr = this.handler.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Max, this.handler.control.model.activeSubDocument);
                if(htr && (!this.handler.control.selection.isCollapsed() && (this.isAreaToLeftOfText(htr, evt) || this.isAreaToRightOfText(htr, evt))))
                    this.collapseSelection(htr);
                else
                    this.handler.switchState(this.getNextState(evt));
            }
        }
        onTouchMove(evt: RichMouseEvent): boolean {
            return true;
        }
        private getLayoutPosition(logPosition: number): LayoutPosition {
            var subDocument = this.handler.control.model.activeSubDocument;
            return subDocument.isMain()
                ? LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.handler.control, this.handler.control.layout, subDocument, logPosition, DocumentLayoutDetailsLevel.Character,
                    new LayoutPositionCreatorConflictFlags().setDefault(this.handler.control.selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false))
                : new LayoutPositionOtherSubDocumentCreator(this.handler.control.layout, subDocument, logPosition, this.handler.control.selection.pageIndex, DocumentLayoutDetailsLevel.Character)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(this.handler.control.selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
        }
        private getNextState(evt: RichMouseEvent): TouchHandlerStateBase {
            var lpStart = this.getLayoutPosition(this.handler.control.selection.intervals[0].start);
            if(this.canExtendSelection(evt.point, lpStart))
                return new TouchHandlerContinueSelectionState(this.handler);
            for(let i = 0, interval: FixedInterval; interval = this.handler.control.selection.intervals[i]; i++) {
                if(this.canExtendSelectionOnOneSide(evt.point, lpStart, interval))
                    return new TouchHandlerContinueSelectionOnOneSideState(this.handler, evt, this.isHitPoints(evt.point, lpStart.getPositionRelativePage(this.handler.control.measurer), lpStart.row.height));
            }
            return new TouchHandlerBeginTapProcessingState(this.handler, evt);
        }
        private canExtendSelection(mousePoint: LayoutPoint, lpStart: LayoutPosition): boolean {
            return this.handler.control.selection.isInFocus() &&
                this.handler.control.selection.isCollapsed() &&
                this.containsPointInSelectionInterval(mousePoint, lpStart.getPositionRelativePage(this.handler.control.measurer));
        }
        private canExtendSelectionOnOneSide(mousePoint: LayoutPoint, lpStart: LayoutPosition, selectionInterval: FixedInterval): boolean {
            return this.handler.control.selection.isInFocus() && this.isLeftOrRightEdge(mousePoint, selectionInterval);
        }
        private containsPointInSelectionInterval(mousePoint: LayoutPoint, selectionPoint: LayoutPoint): boolean {
            return Math.abs(mousePoint.pageX - selectionPoint.pageX) < TOUCH_RADIUS_HANDLE;
        }
        private isLeftOrRightEdge(mousePoint: LayoutPoint, selectionInterval: FixedInterval) {
            var lpStart = this.getLayoutPosition(selectionInterval.start);
            var lpEnd = this.getLayoutPosition(selectionInterval.end());
            return this.isHitPoints(mousePoint, lpStart.getPositionRelativePage(this.handler.control.measurer), lpStart.row.height) || this.isHitPoints(mousePoint, lpEnd.getPositionRelativePage(this.handler.control.measurer), lpEnd.row.height);
        }
        private isHitPoints(mousePoint: LayoutPoint, selectionPoint: LayoutPoint, height: number): boolean {
            var hitX = Math.abs(mousePoint.pageX - selectionPoint.pageX) < TOUCH_RADIUS_HANDLE;
            var hitY = false;
            if (mousePoint.pageY < selectionPoint.pageY)
                hitY = Math.abs(mousePoint.pageY - selectionPoint.pageY) < TOUCH_RADIUS_HANDLE;
            else
                hitY = (mousePoint.pageY - selectionPoint.pageY) < (TOUCH_RADIUS_HANDLE + height);
            return hitX && hitY;
        }
        private shouldProcessResizeBoxVisualizer(evt: RichMouseEvent): boolean {
            if (this.handler.resizeBoxVisualizer.shouldCapture(evt)) {
                this.beginResizeBoxTouchHandler(evt);
                return true;
            }
            return false;
        }
        private beginResizeBoxTouchHandler(evt: RichMouseEvent) {
            this.handler.switchState(new TouchHandlerResizeBoxState(this.handler));
            this.handler.state.onTouchStart(evt);
        }
        private isAreaToLeftOfText(htr: HitTestResult, evt: RichMouseEvent): boolean {
            return htr.exactlyDetailLevel == DocumentLayoutDetailsLevel.PageArea && htr.deviations[DocumentLayoutDetailsLevel.Column] & HitTestDeviation.Left &&
                evt.point.pageX <= htr.pageArea.x + htr.column.x;
        }
        private isAreaToRightOfText(htr: HitTestResult, evt: RichMouseEvent): boolean {
            return htr.exactlyDetailLevel == DocumentLayoutDetailsLevel.PageArea && htr.deviations[DocumentLayoutDetailsLevel.Column] & HitTestDeviation.Right &&
                evt.point.pageX >= htr.pageArea.x + htr.column.x + htr.column.width;
        }
        private collapseSelection(htr: HitTestResult) {
            var selection: Selection = this.handler.control.selection;
            var position: number = htr.row.getLastVisibleBox().getEndPosition();
            selection.setSelection(position, position, false, -1, UpdateInputPositionProperties.Yes);
        }
    }
}