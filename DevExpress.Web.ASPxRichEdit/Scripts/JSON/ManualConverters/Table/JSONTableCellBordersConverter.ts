module __aspxRichEdit {
    export enum JSONTableCellBordersProperty {
        TopBorder = 0,
        LeftBorder = 1,
        RightBorder = 2,
        BottomBorder = 3,
        TopLeftDiagonalBorder = 4,
        TopRightDiagonalBorder = 5
    }

    export class JSONTableCellBordersConverter {
        static convertFromJSON(obj: any): TableCellBorders {
            var result = new TableCellBorders();
            result.topBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableCellBordersProperty.TopBorder]);
            result.leftBorder= JSONBorderInfoConverter.convertFromJSON(obj[JSONTableCellBordersProperty.LeftBorder]);
            result.rightBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableCellBordersProperty.RightBorder]);
            result.bottomBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableCellBordersProperty.BottomBorder]);
            result.topLeftDiagonalBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableCellBordersProperty.TopLeftDiagonalBorder]);
            result.topRightDiagonalBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableCellBordersProperty.TopRightDiagonalBorder]);
            return result;
        }

        static convertToJSON(source: TableCellBorders): any {
            var result = {};
            result[JSONTableCellBordersProperty.TopBorder] = JSONBorderInfoConverter.convertToJSON(source.topBorder);
            result[JSONTableCellBordersProperty.LeftBorder] = JSONBorderInfoConverter.convertToJSON(source.leftBorder);
            result[JSONTableCellBordersProperty.RightBorder] = JSONBorderInfoConverter.convertToJSON(source.rightBorder);
            result[JSONTableCellBordersProperty.BottomBorder] = JSONBorderInfoConverter.convertToJSON(source.bottomBorder);
            result[JSONTableCellBordersProperty.TopLeftDiagonalBorder] = JSONBorderInfoConverter.convertToJSON(source.topLeftDiagonalBorder);
            result[JSONTableCellBordersProperty.TopRightDiagonalBorder] = JSONBorderInfoConverter.convertToJSON(source.topRightDiagonalBorder);
            return result;
        }
    }
} 