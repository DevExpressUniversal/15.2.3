module __aspxRichEdit {
    export enum JSONTableBordersProperty {
        TopBorder = 0,
        LeftBorder = 1,
        RightBorder = 2,
        BottomBorder = 3,
        InsideHorizontalBorder = 4,
        InsideVerticalBorder = 5
    }

    export class JSONTableBordersConverter {
        static convertFromJSON(obj: any): TableBorders {
            var result = new TableBorders();
            result.topBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableBordersProperty.TopBorder]);
            result.leftBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableBordersProperty.LeftBorder]);
            result.rightBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableBordersProperty.RightBorder]);
            result.bottomBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableBordersProperty.BottomBorder]);
            result.insideHorizontalBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableBordersProperty.InsideHorizontalBorder]);
            result.insideVerticalBorder = JSONBorderInfoConverter.convertFromJSON(obj[JSONTableBordersProperty.InsideVerticalBorder]);
            return result;
        }

        static convertToJSON(source: TableBorders): any {
            var result = {};
            result[JSONTableBordersProperty.TopBorder] = JSONBorderInfoConverter.convertToJSON(source.topBorder);
            result[JSONTableBordersProperty.LeftBorder] = JSONBorderInfoConverter.convertToJSON(source.leftBorder);
            result[JSONTableBordersProperty.RightBorder] = JSONBorderInfoConverter.convertToJSON(source.rightBorder);
            result[JSONTableBordersProperty.BottomBorder] = JSONBorderInfoConverter.convertToJSON(source.bottomBorder);
            result[JSONTableBordersProperty.InsideHorizontalBorder] = JSONBorderInfoConverter.convertToJSON(source.insideHorizontalBorder);
            result[JSONTableBordersProperty.InsideVerticalBorder] = JSONBorderInfoConverter.convertToJSON(source.insideVerticalBorder);
            return result;
        }
    }
} 