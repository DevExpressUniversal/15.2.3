module __aspxRichEdit {
    export enum JSONTableHeightUnitProperty {
        Type = 0,
        Value = 1
    }

    export enum JSONTableWidthUnitProperty {
        Type = 0,
        Value = 1
    }

    export class JSONTableHeightUnitConverter {
        static convertFromJSON(obj: any): TableHeightUnit {
            var result = new TableHeightUnit();
            result.type = obj[JSONTableHeightUnitProperty.Type];
            result.value = obj[JSONTableHeightUnitProperty.Value];
            return result;
        }

        static convertToJSON(source: TableHeightUnit): any {
            var result = {};
            result[JSONTableHeightUnitProperty.Type] = source.type;
            result[JSONTableHeightUnitProperty.Value] = source.value;
            return result;
        }
    }

    export class JSONTableWidthUnitConverter {
        static convertFromJSON(obj: any): TableWidthUnit {
            var result = new TableWidthUnit();
            result.type = obj[JSONTableWidthUnitProperty.Type];
            result.value = obj[JSONTableWidthUnitProperty.Value];
            return result;
        }

        static convertToJSON(source: TableWidthUnit): any {
            var result = {};
            result[JSONTableWidthUnitProperty.Type] = source.type;
            result[JSONTableWidthUnitProperty.Value] = source.value;
            return result;
        }
    }
} 