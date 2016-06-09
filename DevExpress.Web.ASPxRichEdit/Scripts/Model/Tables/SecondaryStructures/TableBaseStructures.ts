module __aspxRichEdit {
    export class TableCellMargins implements IEquatable<TableCellMargins>, ICloneable<TableCellMargins> {
        top: TableWidthUnit;  // (can't be in percent) 
        right: TableWidthUnit;
        left: TableWidthUnit;
        bottom: TableWidthUnit;

        public equals(obj: TableCellMargins): boolean {
            if (!obj)
                return false;
            return this.top.equals(obj.top) &&
                this.right.equals(obj.right) &&
                this.left.equals(obj.left) &&
                this.bottom.equals(obj.bottom);
        }

        public copyFrom(obj: TableCellMargins) {
            this.top = obj.top.clone();
            this.right = obj.right.clone();
            this.left = obj.left.clone();
            this.bottom = obj.bottom.clone();
        }

        public clone(): TableCellMargins {
            var result = new TableCellMargins();
            result.copyFrom(this);
            return result;
        }

        static create(top: TableWidthUnit, right: TableWidthUnit, bottom: TableWidthUnit, left: TableWidthUnit): TableCellMargins {
            let result = new TableCellMargins();
            result.top = top;
            result.right = right;
            result.bottom = bottom;
            result.left = left;
            return result;
        }
    }

    export enum TableLayoutType {
        Fixed,
        Autofit
    }

    export enum TableLookTypes {
        None = 0x0000,
        ApplyFirstRow = 0x0020,
        ApplyLastRow = 0x0040,
        ApplyFirstColumn = 0x0080,
        ApplyLastColumn = 0x0100,
        DoNotApplyRowBanding = 0x0200,
        DoNotApplyColumnBanding = 0x0400
    }

    export enum HorizontalAlignMode {
        None,
        Center, //centered with respect to the anchor settings
        Inside, //inside of the anchor object
        Left, //left aligned with respect to the anchor settings
        Outside, //outside of the anchor object
        Right
    }

    export enum VerticalAlignMode {
        None,
        Bottom, //vertically aligned to the bottom edge of the anchor object
        Center,
        Inline, //vertically aligned in line with the surrounding text (i.e. shall not allow any text wrapping around it when positioned in the document
        Inside, //vertically aligned to the edge of the anchor object, and positioned inside that object
        Outside,
        Top
    }

    export enum HorizontalAnchorTypes {
        Margin, //horizontally anchored to the text margins
        Page, //horizontally anchored to the page edge
        Column
    }

    export enum VerticalAnchorTypes {
        Margin, //horizontally anchored to the text margins
        Page, //horizontally anchored to the page edge
        Paragraph
    }

    export enum TextWrapping {
        Never,
        Around
    }

    export enum TableRowAlignment {
        Both,
        Center,
        Distribute,
        Left,
        NumTab, //Align to List Tab
        Right
    }

    export enum TableCellMergingState {
        None,
        Continue,
        Restart
    }

    export enum TextDirection {
        LeftToRightTopToBottom = 0,
        TopToBottomRightToLeft = 1,
        TopToBottomLeftToRightRotated = 2,
        BottomToTopLeftToRight = 3,
        LeftToRightTopToBottomRotated = 4,
        TopToBottomRightToLeftRotated = 5,
    }

    export enum TableCellVerticalAlignment {
        Top = 0,
        Both = 1, // ?? not done.
        Center = 2,
        Bottom = 3
    }

    // TablePropertiesMerger.conditionalTableStyleFormattingPriority change when changed ConditionalTableStyleFormatting
    export enum ConditionalTableStyleFormatting {
        WholeTable = 4096,
        FirstRow = 2048, //Is this the first row of the table?
        LastRow = 1024, //Is this the last row of the table?
        FirstColumn = 512, //Does this belong to the first column of the table?
        LastColumn = 256, //Does this belong to the last column of the table?
        OddColumnBanding = 128, //Does this belong to a column which should receive band 1 formatting? This property specifies whether the cell should receive the formatting specified for odd-numbered columns (e.g. 1,3,5,...)
        EvenColumnBanding = 64, //Does this belong to a column which should receive band 2 formatting? This property specifies whether the cell should receive the formatting specified for even-numbered columns (e.g. 2,4,6...)
        OddRowBanding = 32, //Does this receive band 1 formatting? This property specifies whether the cell should receive the formatting specified for odd-numbered rows (e.g. 1,3,5,...)
        EvenRowBanding = 16, //Does this receive band 2 formatting? This property specifies whether the cell should receive the formatting specified for even-numbered rows (e.g. 2,4,6...)
        TopRightCell = 8, //Is this part of the top-right corner of the table?
        TopLeftCell = 4, //Is this part of the top-left corner of the table?
        BottomRightCell = 2, //Is this part of the bottom-right corner of the table?
        BottomLeftCell = 1, //Is this part of the bottom-left corner of the table?        
    }

    

    
}