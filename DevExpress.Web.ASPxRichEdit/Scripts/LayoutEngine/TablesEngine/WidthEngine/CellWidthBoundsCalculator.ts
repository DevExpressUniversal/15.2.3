//module __aspxRichEdit {
//    export class CellWidthBoundsWithHorizMargins extends WidthBounds {
//        totalHorizontalMargins: number;
//        constructor(min: number, max: number, totalHorizontalMargins: number) {
//            super(min, max);
//            this.totalHorizontalMargins = totalHorizontalMargins;
//        }
//    }

//    export class CellWidthBoundsCalculator {
//        private static MAX_CELL_WIDTH = UnitConverter.twipsToPixels(Math.pow(2, 16));

//        public static calculateCellWidthsBound(percentBaseWidth: number, simpleView: boolean,
//            unitConverter: IUnitConverter, nestedTableIterator: NestedTableIterator): CellWidthBoundsWithHorizMargins {
//            //TODO: if cell has a vertical merging, we must get max from borders on all rows, that span's by this cell.
//            const model: DocumentModel = nestedTableIterator.subDocument.documentModel;
//            let tablePosition = Table.getFirstCellPositionInVerticalMergingGroup(nestedTableIterator.currTablePosition.clone());
//            let cell: TableCell = tablePosition.cell;
//            const preferredWidth: TableWidthUnit = new TableCellPropertiesMergerPreferredWidth().getProperty(cell.properties, tablePosition.table.style,
//                cell.conditionalFormatting, model.defaultTableCellProperties);
//            const preferredWidthInPixels: number = preferredWidth.asNumber(unitConverter, percentBaseWidth);
//            const cellLeftMargin: TableWidthUnit = new TableCellPropertiesMergerMarginLeft(tablePosition.table, model)
//                .getProperty(tablePosition.cell.properties, tablePosition.table.style, tablePosition.cell.conditionalFormatting, model.defaultTableCellProperties);
//            const cellRightMargin: TableWidthUnit = new TableCellPropertiesMergerMarginLeft(tablePosition.table, model)
//                .getProperty(tablePosition.cell.properties, tablePosition.table.style, tablePosition.cell.conditionalFormatting, model.defaultTableCellProperties);
//            const horizontalMargins: number = cellLeftMargin.asNumberNoPercentType(unitConverter) + cellRightMargin.asNumberNoPercentType(unitConverter);

//            const leftBorderInfo: BorderInfo = new TableCellPropertiesMergerBorderLeft(tablePosition, model, false)
//                .getProperty(cell.properties, tablePosition.table.style, cell.conditionalFormatting, model.defaultTableCellProperties);

//            const rightBorderInfo: BorderInfo = new TableCellPropertiesMergerBorderRight(tablePosition, model, false)
//                .getProperty(cell.properties, tablePosition.table.style, cell.conditionalFormatting, model.defaultTableCellProperties);

//            const bordersWidth: number = unitConverter.toPixels(TableBorderCalculator.getActualWidth(leftBorderInfo)) +
//                unitConverter.toPixels(TableBorderCalculator.getActualWidth(rightBorderInfo));
//            let spacing: number = new TableRowPropertiesMergerCellSpacing(model, tablePosition.table)
//                .getProperty(tablePosition.row.properties, tablePosition.table.style, TableRow.calculateConditionalFormattingFlags(tablePosition.table,
//                    tablePosition.rowIndex), model.defaultTableRowProperties)
//                .asNumberNoPercentType(unitConverter);
//            if (spacing > 0)
//                spacing *= (tablePosition.row.gridBefore == 0 && tablePosition.cellIndex == 0) ||
//                    (tablePosition.row.gridAfter == 0 && tablePosition.cellIndex == tablePosition.row.cells.length - 1) ? 3 : 2;

//            const layoutType: TableLayoutType = new TablePropertiesMergerLayoutType().getProperty(tablePosition.table.properties, tablePosition.table.style,
//                ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties);
//            const spaceBordersMarginsWidth: number = horizontalMargins + bordersWidth + spacing;
//            if (layoutType == TableLayoutType.Autofit || simpleView) {
//                let contentWidths: WidthBounds = CellWidthBoundsCalculator.calculateCellContentBounds(percentBaseWidth, simpleView, unitConverter, nestedTableIterator);
//                const noWrap: boolean = new TableCellPropertiesMergerNoWrap().getProperty(cell.properties, tablePosition.table.style, cell.conditionalFormatting,
//                    model.defaultTableCellProperties);
//                if (noWrap)
//                    contentWidths = new WidthBounds(contentWidths.max, contentWidths.max);
//                const resultMinWidth: number = Math.min(CellWidthBoundsCalculator.MAX_CELL_WIDTH, contentWidths.min + spaceBordersMarginsWidth);
//                let resultMaxWidth: number = Math.min(CellWidthBoundsCalculator.MAX_CELL_WIDTH, contentWidths.max + spaceBordersMarginsWidth);
//                if (preferredWidth.type == TableWidthUnitType.ModelUnits || preferredWidth.type == TableWidthUnitType.FiftiethsOfPercent)
//                    resultMaxWidth = Math.max(resultMinWidth, preferredWidthInPixels);
//                return new CellWidthBoundsWithHorizMargins(resultMinWidth, resultMaxWidth, horizontalMargins);
//            }
//            else {
//                nestedTableIterator.skipCell();
//                const outerWidth: number = spaceBordersMarginsWidth;
//                const result: number = Math.max(outerWidth, preferredWidthInPixels);
//                return new CellWidthBoundsWithHorizMargins(result, result, horizontalMargins);
//            }
//        }

//        private static calculateCellContentBounds(percentBaseWidth: number, simpleView: boolean, unitConverter: IUnitConverter, nestedTableIterator: NestedTableIterator):
//            WidthBounds {
//            const maxBounds: WidthBounds = new WidthBounds(0, 0);
//            while (true) {
//                switch (nestedTableIterator.moveNext()) {
//                    case NestedTableIteratorResult.LevelNoChange:
//                        maxBounds.choiseMaxWidthBounds(nestedTableIterator.currParagraph.layoutWidthBounds);
//                        break;
//                    case NestedTableIteratorResult.LevelDown:
//                        maxBounds.choiseMaxWidthBounds(
//                            EstimatedTableWidthCalculator.getNestedTableWidth(percentBaseWidth, simpleView, unitConverter, nestedTableIterator));
//                        break;
//                    default:
//                        return maxBounds;
//                }
//            }
//            throw new Error(Errors.NotImplemented);
//        }
//    }
//}