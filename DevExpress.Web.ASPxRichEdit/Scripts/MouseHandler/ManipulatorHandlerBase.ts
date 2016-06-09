module __aspxRichEdit {
    export class ManipulatorHandlerBase {
        control: IRichEditControl;
        dragCaretVisualizer: DragCaretVisualizer;
        resizeBoxVisualizer: ResizeBoxVisualizer;

        constructor(control: IRichEditControl, dragCaretVisualizer: DragCaretVisualizer, resizeBoxVisualizer: ResizeBoxVisualizer) {
            this.control = control;
            this.dragCaretVisualizer = dragCaretVisualizer;
            this.resizeBoxVisualizer = resizeBoxVisualizer;
        }

        getHyperlinkFieldResult(evt: RichMouseEvent): Field {
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var htr: HitTestResult = this.control.hitTestManager.calculate(evt.point, DocumentLayoutDetailsLevel.Max, subDocument);
            if(htr) {
                var position: number = htr.getPosition();
                var fieldInfos: FieldVisabilityInfo[] = FieldVisabilityInfo.getRelativeVisabilityInfo(position, subDocument.fields);
                for(var i: number = fieldInfos.length - 1, info: FieldVisabilityInfo; info = fieldInfos[i]; i--) {
                    if(info.showResult && info.field.getResultInterval().contains(position) && info.field.isHyperlinkField())
                        return info.field;
                }
            }
            return null;
        }
    }
} 