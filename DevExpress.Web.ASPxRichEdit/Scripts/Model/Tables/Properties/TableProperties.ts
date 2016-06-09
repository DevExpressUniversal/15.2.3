module __aspxRichEdit {
    export class TableProperties implements IEquatable<TableProperties>, ICloneable<TableProperties>, IMaskedProperties<TablePropertiesMask> {
        // http://workservices01/OpenWiki/ow.asp?p=ASPxRichEdit_Tables#TableProperties
        mask: TablePropertiesMask = 0;

        cellMargins: TableCellMargins = TableCellMargins.create(TableWidthUnit.createDefault(), TableWidthUnit.createDefault(), TableWidthUnit.createDefault(), TableWidthUnit.createDefault()); // (can't be in percent)
        cellSpacing: TableWidthUnit = TableWidthUnit.createDefault(); // between cells (can't be in percent)
        indent: TableWidthUnit = TableWidthUnit.createDefault(); // for outer table - [column -> left bound table], 
                                                                 // for inner table [left bound cell + cellLeftMargin -> left bound table] (can't be in percent)
        borders: TableBorders = TableBorders.create(new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo(), new BorderInfo());

        //generalSettings: TableGeneralSettings;
        tableStyleColumnBandSize: number = 1; // how many column in one group for styles (default - 1)
        tableStyleRowBandSize: number = 1;
        avoidDoubleBorders: boolean = false; //??
        layoutType: TableLayoutType = TableLayoutType.Autofit;
        backgroundColor: number = ColorHelper.NO_COLOR;
        tableRowAlignment: TableRowAlignment = TableRowAlignment.Left;

        //floatingPosition: TableFloatingPosition;
        isTableOverlap: boolean = true; // can intersect with others floating objects
        bottomFromText: number = 0; // for Floating
        leftFromText: number = 0; // for Floating
        topFromText: number = 0; // for Floating
        rightFromText: number = 0; // for Floating
        tableHorizontalPosition: number = 0; // for Floating
        tableVerticalPosition: number = 0; // for Floating
        horizontalAlignMode: HorizontalAlignMode = HorizontalAlignMode.None; // for Floating
        verticalAlignMode: VerticalAlignMode = VerticalAlignMode.None; // for Floating
        horizontalAnchorType: HorizontalAnchorTypes = HorizontalAnchorTypes.Page; // for Floating
        verticalAnchorType: VerticalAnchorTypes = VerticalAnchorTypes.Page; // for Floating
        textWrapping: TextWrapping = TextWrapping.Never; // for Floating

        public equals(obj: TableProperties): boolean {
            if (!obj)
                return false;
            return this.mask == obj.mask &&
                this.cellMargins.equals(obj.cellMargins) &&
                this.cellSpacing.equals(obj.cellSpacing) &&
                this.indent.equals(obj.indent) &&
                this.borders.equals(obj.borders) &&

                this.tableStyleColumnBandSize == obj.tableStyleColumnBandSize &&
                this.tableStyleRowBandSize == obj.tableStyleRowBandSize &&
                this.isTableOverlap == obj.isTableOverlap &&
                this.avoidDoubleBorders == obj.avoidDoubleBorders &&
                this.layoutType == obj.layoutType &&
                this.backgroundColor == obj.backgroundColor &&
                this.tableRowAlignment == obj.tableRowAlignment &&

                this.bottomFromText == obj.bottomFromText &&
                this.leftFromText == obj.leftFromText &&
                this.topFromText == obj.topFromText &&
                this.rightFromText == obj.rightFromText &&
                this.tableHorizontalPosition == obj.tableHorizontalPosition &&
                this.tableVerticalPosition == obj.tableVerticalPosition &&
                this.horizontalAlignMode == obj.horizontalAlignMode &&
                this.verticalAlignMode == obj.verticalAlignMode &&
                this.horizontalAnchorType == obj.horizontalAnchorType &&
                this.verticalAnchorType == obj.verticalAnchorType &&
                this.textWrapping == obj.textWrapping;
        }

        public clone(): TableProperties {
            var result = new TableProperties();
            result.copyFrom(this);
            return result;
        }

        public copyFrom(obj: TableProperties) {
            this.mask = obj.mask;
            this.cellMargins = obj.cellMargins.clone();
            this.cellSpacing = obj.cellSpacing.clone();
            this.indent = obj.indent.clone();
            this.borders = obj.borders.clone();

            this.tableStyleColumnBandSize = obj.tableStyleColumnBandSize;
            this.tableStyleRowBandSize = obj.tableStyleRowBandSize;
            this.isTableOverlap = obj.isTableOverlap;
            this.avoidDoubleBorders = obj.avoidDoubleBorders;
            this.layoutType = obj.layoutType;
            this.backgroundColor = obj.backgroundColor;
            this.tableRowAlignment = obj.tableRowAlignment;

            this.bottomFromText = obj.bottomFromText;
            this.leftFromText = obj.leftFromText;
            this.topFromText = obj.topFromText;
            this.rightFromText = obj.rightFromText;
            this.tableHorizontalPosition = obj.tableHorizontalPosition;
            this.tableVerticalPosition = obj.tableVerticalPosition;
            this.horizontalAlignMode = obj.horizontalAlignMode;
            this.verticalAlignMode = obj.verticalAlignMode;
            this.horizontalAnchorType = obj.horizontalAnchorType;
            this.verticalAnchorType = obj.verticalAnchorType;
            this.textWrapping = obj.textWrapping;
        }

        setUseValue(mask: TablePropertiesMask, value: boolean) {
            if(value)
                this.mask |= mask;
            else
                this.mask &= ~mask;
        }
        getUseValue(mask: TablePropertiesMask): boolean {
            return (this.mask & mask) != 0;
        }
    }

    export enum TablePropertiesMask {
        UseNone = 0x00000000,
        UseLeftMargin = 0x00000001,
        UseRightMargin = 0x00000002,
        UseTopMargin = 0x00000004,
        UseBottomMargin = 0x00000008,
        UseCellSpacing = 0x00000010,
        UseTableIndent = 0x00000020,
        UseTableLayout = 0x00000040,
        //UseTableLook = 0x00000080, // do nothing
        //UsePreferredWidth = 0x00000100, // do nothing
        UseTableStyleColBandSize = 0x00000200,
        UseTableStyleRowBandSize = 0x00000400,
        UseIsTableOverlap = 0x00000800,
        UseFloatingPosition = 0x00001000,
        UseLeftBorder = 0x00002000,
        UseRightBorder = 0x00004000,
        UseTopBorder = 0x00008000,
        UseBottomBorder = 0x00010000,
        UseInsideHorizontalBorder = 0x00020000,
        UseInsideVerticalBorder = 0x00040000,
        UseBackgroundColor = 0x00080000,
        UseTableAlignment = 0x00100000,
        UseAvoidDoubleBorders = 0x00200000,
        UseAll = 0x7FFFFFFF
    }
}