module __aspxRichEdit {
    export enum JSONTableCellMarginsProperty {
        Top = 0,
        Left = 1,
        Right = 2,
        Bottom = 3
    }

    export class JSONTableCellMarginsConverter {
        static convertFromJSON(obj: any): TableCellMargins {
            var result = new TableCellMargins();
            result.top = JSONTableWidthUnitConverter.convertFromJSON(obj[JSONTableCellMarginsProperty.Top]);
            result.left = JSONTableWidthUnitConverter.convertFromJSON(obj[JSONTableCellMarginsProperty.Left]);
            result.right = JSONTableWidthUnitConverter.convertFromJSON(obj[JSONTableCellMarginsProperty.Right]);
            result.bottom = JSONTableWidthUnitConverter.convertFromJSON(obj[JSONTableCellMarginsProperty.Bottom]);
            return result;
        }

        static convertToJSON(source: TableCellMargins): any {
            var result = {};
            result[JSONTableCellMarginsProperty.Top] = JSONTableWidthUnitConverter.convertToJSON(source.top);
            result[JSONTableCellMarginsProperty.Left] = JSONTableWidthUnitConverter.convertToJSON(source.left);
            result[JSONTableCellMarginsProperty.Right] = JSONTableWidthUnitConverter.convertToJSON(source.right);
            result[JSONTableCellMarginsProperty.Bottom] = JSONTableWidthUnitConverter.convertToJSON(source.bottom);
            return result;
        }
    }
} 