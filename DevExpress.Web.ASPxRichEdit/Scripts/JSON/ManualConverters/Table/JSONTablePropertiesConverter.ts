module __aspxRichEdit {
    export enum JSONTableProperty {
        CellMargins = 0,
        CellSpacing = 1,
        Indent = 2,
        PreferredWidth = 3,
        Borders = 4,
        TableStyleColBandSize = 5,
        TableStyleRowBandSize = 6,
        IsTableOverlap = 7,
        AvoidDoubleBorders = 8,
        LayoutType = 9,
        TableLookTypes = 10,
        BackgroundColor = 11,
        TableRowAlignment = 12,
        BottomFromText = 13,
        LeftFromText = 14,
        TopFromText = 15,
        RightFromText = 16,
        TableHorizontalPosition = 17,
        TableVerticalPosition = 18,
        HorizontalAlignMode = 19,
        VerticalAlignMode = 20,
        HorizontalAnchorType = 21,
        VerticalAnchorType = 22,
        TextWrapping = 23,
        UseValue = 24
    }

    export class JSONTablePropertiesConverter {
        static convertFromJSON(obj: any): TableProperties {
            var result = new TableProperties();
            result.cellMargins = JSONTableCellMarginsConverter.convertFromJSON(obj[JSONTableProperty.CellMargins]);
            result.cellSpacing = JSONTableWidthUnitConverter.convertFromJSON(obj[JSONTableProperty.CellSpacing]);
            result.indent = JSONTableWidthUnitConverter.convertFromJSON(obj[JSONTableProperty.Indent]);
            result.borders = JSONTableBordersConverter.convertFromJSON(obj[JSONTableProperty.Borders]);

            result.tableStyleColumnBandSize = obj[JSONTableProperty.TableStyleColBandSize];
            result.tableStyleRowBandSize = obj[JSONTableProperty.TableStyleRowBandSize];
            result.isTableOverlap = obj[JSONTableProperty.IsTableOverlap];
            result.avoidDoubleBorders = obj[JSONTableProperty.AvoidDoubleBorders];
            result.layoutType = obj[JSONTableProperty.LayoutType];
            result.backgroundColor = obj[JSONTableProperty.BackgroundColor];
            result.tableRowAlignment = obj[JSONTableProperty.TableRowAlignment];

            result.bottomFromText = obj[JSONTableProperty.BottomFromText];
            result.leftFromText = obj[JSONTableProperty.LeftFromText];
            result.topFromText = obj[JSONTableProperty.TopFromText];
            result.rightFromText = obj[JSONTableProperty.RightFromText];
            result.tableHorizontalPosition = obj[JSONTableProperty.TableHorizontalPosition];
            result.tableVerticalPosition = obj[JSONTableProperty.TableVerticalPosition];
            result.horizontalAlignMode = obj[JSONTableProperty.HorizontalAlignMode];
            result.verticalAlignMode = obj[JSONTableProperty.VerticalAlignMode];
            result.horizontalAnchorType = obj[JSONTableProperty.HorizontalAnchorType];
            result.verticalAnchorType = obj[JSONTableProperty.VerticalAnchorType];
            result.textWrapping = obj[JSONTableProperty.TextWrapping];
            result.mask = obj[JSONTableProperty.UseValue];
            return result;
        }

        static convertToJSON(source: TableProperties): any {
            var result = {};
            result[JSONTableProperty.CellMargins] = JSONTableCellMarginsConverter.convertToJSON(source.cellMargins);
            result[JSONTableProperty.CellSpacing] = JSONTableWidthUnitConverter.convertToJSON(source.cellSpacing);
            result[JSONTableProperty.Indent] = JSONTableWidthUnitConverter.convertToJSON(source.indent);
            result[JSONTableProperty.Borders] = JSONTableBordersConverter.convertToJSON(source.borders);

            result[JSONTableProperty.TableStyleColBandSize] = source.tableStyleColumnBandSize;
            result[JSONTableProperty.TableStyleRowBandSize] = source.tableStyleRowBandSize;
            result[JSONTableProperty.IsTableOverlap] = source.isTableOverlap;
            result[JSONTableProperty.AvoidDoubleBorders] = source.avoidDoubleBorders;
            result[JSONTableProperty.LayoutType] = source.layoutType;
            result[JSONTableProperty.BackgroundColor] = source.backgroundColor;
            result[JSONTableProperty.TableRowAlignment] = source.tableRowAlignment;

            result[JSONTableProperty.BottomFromText] = source.bottomFromText;
            result[JSONTableProperty.LeftFromText] = source.leftFromText;
            result[JSONTableProperty.TopFromText] = source.topFromText;
            result[JSONTableProperty.RightFromText] = source.rightFromText;
            result[JSONTableProperty.TableHorizontalPosition] = source.tableHorizontalPosition;
            result[JSONTableProperty.TableVerticalPosition] = source.tableVerticalPosition;
            result[JSONTableProperty.HorizontalAlignMode] = source.horizontalAlignMode;
            result[JSONTableProperty.VerticalAlignMode] = source.verticalAlignMode;
            result[JSONTableProperty.HorizontalAnchorType] = source.horizontalAnchorType;
            result[JSONTableProperty.VerticalAnchorType] = source.verticalAnchorType;
            result[JSONTableProperty.TextWrapping] = source.textWrapping;
            result[JSONTableProperty.UseValue] = source.mask;

            return result;
        }
    }
} 