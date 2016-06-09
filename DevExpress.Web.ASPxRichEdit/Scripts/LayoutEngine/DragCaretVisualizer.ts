module __aspxRichEdit {
    export interface IDragCaretVisualizerChangesListener {
        NotifyDragCaretShown(layoutDragCaret: LayoutDragCaret);
        NotifyDragCaretHidden();
    }

    export class DragCaretVisualizer {
        onChanged: EventDispatcher<IDragCaretVisualizerChangesListener> = new EventDispatcher<IDragCaretVisualizerChangesListener>();
        private control: IRichEditControl;
        private layoutDragCaret: LayoutDragCaret;

        constructor(control: IRichEditControl) {
            this.control = control;
        }

        show(htr: HitTestResult) {
            var layoutDragCaret: LayoutDragCaret = new LayoutDragCaret(htr.pageIndex);
            layoutDragCaret.y = htr.pageArea.y + htr.column.y + htr.row.y + htr.row.getSpacingBefore();
            layoutDragCaret.x = htr.pageArea.x + htr.column.x + htr.row.x + htr.box.x + htr.box.getCharOffsetXInPixels(this.control.measurer, htr.charOffset);
            layoutDragCaret.height = htr.row.height - htr.row.getSpacingBefore();
            if(!this.layoutDragCaret || !this.layoutDragCaret.equals(layoutDragCaret)) {
                this.layoutDragCaret = layoutDragCaret;
                this.onChanged.raise("NotifyDragCaretShown", layoutDragCaret);
            }
        }
        hide() {
            this.layoutDragCaret = null;
            this.onChanged.raise("NotifyDragCaretHidden");
        }
    }
}