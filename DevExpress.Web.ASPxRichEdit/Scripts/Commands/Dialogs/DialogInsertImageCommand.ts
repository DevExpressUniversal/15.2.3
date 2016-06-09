module __aspxRichEdit {
    export class DialogInsertImageCommand extends ShowDialogCommandBase {
        columnsCalculator: ColumnsCalculator;
        constructor(control: IRichEditControl) {
            super(control);
            this.columnsCalculator = new ColumnsCalculator(new TwipsUnitConverter());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.inlinePictures) && this.control.selection.intervals.length === 1;
        }
        createParameters(): InsertImageDialogParameters {
            return new InsertImageDialogParameters();
        }
        // here new params as Twips
        applyParameters(state: SimpleCommandState, newParams: InsertImageDialogParameters) {
            var subDocument = this.control.model.activeSubDocument;
            var logPosition = this.control.selection.intervals[0].start;
            var layoutPosition: LayoutPosition = (subDocument.isMain()
                ? new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, logPosition, DocumentLayoutDetailsLevel.Column)
                : new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, logPosition, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Column))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
            var allowedWidth: number;
            var allowedHeight: number;
            if(layoutPosition) {
                allowedWidth = UnitConverter.pixelsToTwips(layoutPosition.column.width);
                allowedHeight = UnitConverter.pixelsToTwips(layoutPosition.column.height);
            }
            else {
                var section: Section = this.control.model.getSectionByPosition(this.control.selection.intervals[0].start);
                var minColumnSize: Size = this.columnsCalculator.findMinimalColumnSize(section.sectionProperties);
                allowedWidth = UnitConverter.pixelsToTwips(minColumnSize.width);
                allowedHeight = UnitConverter.pixelsToTwips(minColumnSize.height);
            }
            var scaleX: number = 100;
            var scaleY: number = 100;
            if(newParams.originalWidth > allowedWidth)
                scaleX = (allowedWidth / newParams.originalWidth) * 100;
            if(newParams.originalHeight > allowedHeight)
                scaleY = (allowedHeight / newParams.originalHeight) * 100;
            ModelManipulator.insertInlinePicture(this.control, this.control.selection.intervals[0].clone(), newParams.id, newParams.originalWidth, newParams.originalHeight, Math.min(scaleX, scaleY),
                Math.min(scaleX, scaleY));
        }
        getDialogName() {
            return "InsertImage";
        }
    }

    export class InsertImageDialogParameters extends DialogParametersBase {
        id: number = 0;
        originalWidth: number;
        originalHeight: number;

        copyFrom(obj: InsertImageDialogParameters) {
            this.id = obj.id;
            this.originalWidth = obj.originalWidth;
            this.originalHeight = obj.originalHeight;
        }

        getNewInstance(): DialogParametersBase {
            return new InsertImageDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }
}