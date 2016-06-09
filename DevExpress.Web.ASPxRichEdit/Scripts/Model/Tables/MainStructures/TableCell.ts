module __aspxRichEdit {
    export class TableCell {
        constructor(parentRow: TableRow, properties: TableCellProperties) {
            this.parentRow = parentRow;
            this.properties = properties;
        }
        preferredWidth: TableWidthUnit = TableWidthUnit.createDefault(); //  (can be in percent)
        columnSpan: number = 1; // num virtual columns
        verticalMerging: TableCellMergingState = TableCellMergingState.None;
        parentRow: TableRow;
        startParagraphPosition: Position;
        endParagrapPosition: Position;
        properties: TableCellProperties;
        style: TableCellStyle; // Don't use for richEdit. Only for Snap

        // layout (don't synchronize with server)
        conditionalFormatting: ConditionalTableStyleFormatting = ConditionalTableStyleFormatting.WholeTable; // for table.style.conditionalStyles

        destructor(positionManager: PositionManager) {
            if (this.startParagraphPosition)
                positionManager.unregisterPosition(this.startParagraphPosition);
            if (this.endParagrapPosition)
                positionManager.unregisterPosition(this.endParagrapPosition);
        }

        getInterval(): FixedInterval {
            return FixedInterval.fromPositions(this.startParagraphPosition.value, this.endParagrapPosition.value);
        }
    }

    export enum TableAutoFitBehaviorType {
        FixedColumnWidth,
        AutoFitToContents,
        AutoFitToWindow
    }
}