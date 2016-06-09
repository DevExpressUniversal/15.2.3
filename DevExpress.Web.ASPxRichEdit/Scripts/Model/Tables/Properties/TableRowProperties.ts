module __aspxRichEdit {
    export class TableRowProperties implements IEquatable<TableRowProperties>, ICloneable<TableRowProperties>, IMaskedProperties<TableRowPropertiesMask> {
        mask: TableRowPropertiesMask = 0;

        cellSpacing: TableWidthUnit = TableWidthUnit.createDefault(); //  (can't be in percent)

        cantSplit: boolean = false; // need place all content in column
        hideCellMark: boolean = false; // if True then height paragraphMark not affect on formatting
        header: boolean = false; // this row is table header. set only in one row in table
        
        tableRowAlignment: TableRowAlignment = TableRowAlignment.Left; // very hard implementation. Not done!

        public equals(obj: TableRowProperties): boolean {
            if (!obj)
                return false;
            return this.mask == obj.mask &&
                this.cellSpacing.equals(obj.cellSpacing) &&
                this.cantSplit == obj.cantSplit &&
                this.hideCellMark == obj.hideCellMark &&
                this.header == obj.header &&
                this.tableRowAlignment == obj.tableRowAlignment;
        }

        public clone(): TableRowProperties {
            var result = new TableRowProperties();
            result.copyFrom(this);
            return result;
        }

        public copyFrom(obj: TableRowProperties) {
            this.mask = obj.mask;
            this.cellSpacing = obj.cellSpacing.clone();
            this.cantSplit = obj.cantSplit;
            this.hideCellMark = obj.hideCellMark;
            this.header = obj.header;
            this.tableRowAlignment = obj.tableRowAlignment;
        }

        setUseValue(mask: TableRowPropertiesMask, value: boolean) {
            if(value)
                this.mask |= mask;
            else
                this.mask &= ~mask;
        }
        getUseValue(mask: TableRowPropertiesMask): boolean {
            return (this.mask & mask) != 0;
        }
    }

    export enum TableRowPropertiesMask {
        UseNone = 0x00000000,
        //UseHeight = 0x00000001, // useless
        UseCantSplit = 0x00000002,
        UseHideCellMark = 0x00000004,
        //UseGridBefore = 0x00000008,
        //UseGridAfter = 0x00000010,
        //UseWidthBefore = 0x00000020,
        //UseWidthAfter = 0x00000040,
        UseCellSpacing = 0x00000080,
        UseTableRowAlignment = 0x00000100,
        UseHeader = 0x00000400,
        UseAll = 0x7FFFFFFF
    }
} 
