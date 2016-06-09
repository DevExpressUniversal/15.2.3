module __aspxRichEdit {
    export interface IResizeBoxVisualizerChangesListener {
        NotifyResizeBoxShown(pageIndex: number, layoutItem: LayoutItemBase, hyperlinkTip: string);
        NotifyResizeBoxHidden();
    }

    export class ResizeBoxVisualizer extends BatchUpdatableObject implements ISelectionChangesListener, ILayoutChangesListener {
        public onChanged: EventDispatcher<IResizeBoxVisualizerChangesListener> = new EventDispatcher<IResizeBoxVisualizerChangesListener>();

        private control: IRichEditControl; // replace to selection amd model
        private initRectangle: Rectangle;
        private static shouldCaptureEvents: { [eventSource: number]: boolean } = null;

        constructor(control: IRichEditControl) {
            super();
            this.control = control;

            if (ResizeBoxVisualizer.shouldCaptureEvents === null) {
                ResizeBoxVisualizer.shouldCaptureEvents = {};
                ResizeBoxVisualizer.shouldCaptureEvents[MouseEventSource.ResizeBox_E] = true;
                ResizeBoxVisualizer.shouldCaptureEvents[MouseEventSource.ResizeBox_N] = true;
                ResizeBoxVisualizer.shouldCaptureEvents[MouseEventSource.ResizeBox_NE] = true;
                ResizeBoxVisualizer.shouldCaptureEvents[MouseEventSource.ResizeBox_NW] = true;
                ResizeBoxVisualizer.shouldCaptureEvents[MouseEventSource.ResizeBox_S] = true;
                ResizeBoxVisualizer.shouldCaptureEvents[MouseEventSource.ResizeBox_SE] = true;
                ResizeBoxVisualizer.shouldCaptureEvents[MouseEventSource.ResizeBox_SW] = true;
                ResizeBoxVisualizer.shouldCaptureEvents[MouseEventSource.ResizeBox_W] = true;
            }
        }

        // BatchUpdatableObject
        public onUpdateUnlocked(occurredEvents: number) {
            if (occurredEvents & ResizeBoxVisualizerEvents.SelectionChanged)
                this.onSelectionChanged(this.control.selection);
        }

        // ISelectionChangesListener
        public NotifySelectionChanged(selection: Selection) {
            if (!this.isUpdateLocked())
                this.onSelectionChanged(selection);
            else
                this.registerOccurredEvent(ResizeBoxVisualizerEvents.SelectionChanged);
        }
        public NotifyFocusChanged(inFocus: boolean) { }

        // ILayoutChangesListener
        public NotifyPagesInvalidated(firstInvalidPageIndex: number) { }
        public NotifyPageReady(pageChange: PageChange) {
            if (pageChange.changeType == LayoutChangeType.Deleted)
                return;
            if (this.initRectangle && this.control.selection.selectedInlinePictureRunPosition != -1) {
                var subDocument = this.control.model.activeSubDocument;
                var logPosition = this.control.selection.selectedInlinePictureRunPosition;
                var layoutPosition: LayoutPosition = (subDocument.isMain()
                    ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, logPosition, DocumentLayoutDetailsLevel.Box)
                    : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, logPosition, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Box))
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));
                if (!layoutPosition || layoutPosition.pageIndex !== pageChange.index)
                    return;
                this.show(layoutPosition.pageIndex, layoutPosition.pageArea, layoutPosition.column, layoutPosition.row, layoutPosition.box);
            }
        }

        // public
        public show(pageIndex: number, pageArea: LayoutPageArea, column: LayoutColumn, row: LayoutRow, box: LayoutBox) {
            var boxY: number = Math.max(0, row.baseLine - box.getAscent());
            this.initRectangle = new Rectangle().init(pageArea.x + column.x + row.x + box.x, pageArea.y + column.y + row.y + boxY, box.width, box.height);
            this.recalculate(pageIndex, box.width, box.height, true, true, box.hyperlinkTip);
        }

        public hide() {
            if (this.initRectangle) {
                this.onChanged.raise("NotifyResizeBoxHidden");
                this.initRectangle = null;
            }
        }

        // need better name for sideH(V)
        public recalculate(pageIndex: number, newWidth: number, newHeight: number, sideH: boolean, sideV: boolean, hyperlinkTip: string) {
            var layoutItem: LayoutItemBase = new LayoutItemBase();
            layoutItem.width = newWidth;
            layoutItem.height = newHeight;
            layoutItem.x = sideH ? this.initRectangle.x : this.initRectangle.x + this.initRectangle.width - newWidth;
            layoutItem.y = sideV ? this.initRectangle.y : this.initRectangle.y + this.initRectangle.height - newHeight;
            this.onChanged.raise("NotifyResizeBoxShown", pageIndex, layoutItem, hyperlinkTip);
        }

        public isResizeBoxVisible(): boolean {
            return !!this.initRectangle;
        }

        public shouldCapture(evt: RichMouseEvent): boolean {
            return !!ResizeBoxVisualizer.shouldCaptureEvents[evt.source];
        }

        // private
        private onSelectionChanged(selection: Selection) {
            if(selection.selectedInlinePictureRunPosition != -1) {
                var subDocument = this.control.model.activeSubDocument;
                var logPosition = selection.selectedInlinePictureRunPosition;
                var layoutPosition: LayoutPosition = (subDocument.isMain()
                    ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, logPosition, DocumentLayoutDetailsLevel.Box)
                    : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, logPosition, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Box))
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));
                if(!layoutPosition)
                    return;
                this.show(layoutPosition.pageIndex, layoutPosition.pageArea, layoutPosition.column, layoutPosition.row, layoutPosition.box);
            }
            else
                this.hide();
        }
    }

    enum ResizeBoxVisualizerEvents {
        SelectionChanged = 1
    }
} 