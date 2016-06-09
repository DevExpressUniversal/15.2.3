module __aspxRichEdit {
    export class TableCellPropertiesMerger<ResultPropertyType> extends TablePropertiesMergerBase<TableCellProperties, ResultPropertyType> {
        private static conditionalTableStyleFormattingPriority: ConditionalTableStyleFormatting[] = [
            ConditionalTableStyleFormatting.TopLeftCell,
            ConditionalTableStyleFormatting.TopRightCell,
            ConditionalTableStyleFormatting.BottomLeftCell,
            ConditionalTableStyleFormatting.BottomRightCell,
            ConditionalTableStyleFormatting.FirstColumn,
            ConditionalTableStyleFormatting.LastColumn,
            ConditionalTableStyleFormatting.FirstRow,
            ConditionalTableStyleFormatting.LastRow,
            ConditionalTableStyleFormatting.EvenRowBanding,
            ConditionalTableStyleFormatting.OddRowBanding,
            ConditionalTableStyleFormatting.EvenColumnBanding,
            ConditionalTableStyleFormatting.OddColumnBanding,
            ConditionalTableStyleFormatting.WholeTable,
        ];

        protected getContainerFromConditionalStyle(condStyle: TableConditionalStyle): TableCellProperties {
            return condStyle.tableCellProperties;
        }

        protected canUseValue(props: TableCellProperties): boolean {
            return !!(props.mask & this.getPropertyMask());
        }

        protected getCondTableStyleFormattingListForThisContainer(): ConditionalTableStyleFormatting[] {
            return TableCellPropertiesMerger.conditionalTableStyleFormattingPriority;
        }
    }

    export class TableCellPropertiesMergerMarginBase extends TableCellPropertiesMerger<TableWidthUnit> {
        protected table: Table;
        protected model: DocumentModel;

        constructor(table: Table, model: DocumentModel) {
            super();
            this.table = table;
            this.model = model;
        }

        protected actionBeforeDefaultValue(): boolean {
            this.result = this.getMarginMerger().getProperty(this.table.properties, this.table.style, ConditionalTableStyleFormatting.WholeTable, this.model.defaultTableProperties);
            return true;
        }

        protected getMarginMerger(): TablePropertiesMerger<TableWidthUnit> {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class TableCellPropertiesMergerMarginLeft extends TableCellPropertiesMergerMarginBase {
        constructor(table: Table, model: DocumentModel) {
            super(table, model);
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseLeftMargin;
        }

        protected getPropertyFromContainer(container: TableCellProperties): TableWidthUnit {
            return container.cellMargins.left;
        }

        protected getMarginMerger(): TablePropertiesMerger<TableWidthUnit> {
            return new TablePropertiesMergerMarginLeft();
        }
    }

    export class TableCellPropertiesMergerMarginRight extends TableCellPropertiesMergerMarginBase {
        constructor(table: Table, model: DocumentModel) {
            super(table, model);
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseRightMargin;
        }

        protected getPropertyFromContainer(container: TableCellProperties): TableWidthUnit {
            return container.cellMargins.right;
        }

        protected getMarginMerger(): TablePropertiesMerger<TableWidthUnit> {
            return new TablePropertiesMergerMarginRight();
        }
    }

    export class TableCellPropertiesMergerMarginTop extends TableCellPropertiesMergerMarginBase {
        constructor(table: Table, model: DocumentModel) {
            super(table, model);
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseTopMargin;
        }

        protected getPropertyFromContainer(container: TableCellProperties): TableWidthUnit {
            return container.cellMargins.top;
        }

        protected getMarginMerger(): TablePropertiesMerger<TableWidthUnit> {
            return new TablePropertiesMergerMarginTop();
        }
    }

    export class TableCellPropertiesMergerMarginBottom extends TableCellPropertiesMergerMarginBase {
        constructor(table: Table, model: DocumentModel) {
            super(table, model);
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseBottomMargin;
        }

        protected getPropertyFromContainer(container: TableCellProperties): TableWidthUnit {
            return container.cellMargins.bottom;
        }

        protected getMarginMerger(): TablePropertiesMerger<TableWidthUnit> {
            return new TablePropertiesMergerMarginBottom();
        }
    }

    export class TableCellPropertiesMergerBorderBase extends TableCellPropertiesMerger <BorderInfo> {
        protected actionBeforeDefaultValue(): boolean {
            this.result = null;
            return true;
        }
    }

    export class TableCellPropertiesMergerBorderLeft extends TableCellPropertiesMergerBorderBase {
        protected getPropertyFromContainer(container: TableCellProperties): BorderInfo {
            return container.borders.leftBorder;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseLeftBorder;
        }
    }

    export class TableCellPropertiesMergerBorderRight extends TableCellPropertiesMergerBorderBase {
        protected getPropertyFromContainer(container: TableCellProperties): BorderInfo {
            return container.borders.rightBorder;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseRightBorder;
        }
    }

    export class TableCellPropertiesMergerBorderTop extends TableCellPropertiesMergerBorderBase {
        protected getPropertyFromContainer(container: TableCellProperties): BorderInfo {
            return container.borders.topBorder;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseTopBorder;
        }
    }

    export class TableCellPropertiesMergerBorderBottom extends TableCellPropertiesMergerBorderBase {
        protected getPropertyFromContainer(container: TableCellProperties): BorderInfo {
            return container.borders.bottomBorder;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseBottomBorder;
        }
    }

    export class TableCellPropertiesMergerBorderTopLeftDiagonal extends TableCellPropertiesMergerBorderBase {
        protected getPropertyFromContainer(container: TableCellProperties): BorderInfo {
            return container.borders.topLeftDiagonalBorder;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseTopLeftDiagonalBorder;
        }
    }

    export class TableCellPropertiesMergerBorderTopRightDiagonal extends TableCellPropertiesMergerBorderBase {
        protected getPropertyFromContainer(container: TableCellProperties): BorderInfo {
            return container.borders.topRightDiagonalBorder;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseTopRightDiagonalBorder;
        }
    }

    export class TableCellPropertiesMergerNoWrap extends TableCellPropertiesMerger<boolean> {
        protected getPropertyFromContainer(container: TableCellProperties): boolean {
            return container.noWrap;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseNoWrap;
        }
    }

    export class TableCellPropertiesMergerBackgroundColor extends TableCellPropertiesMerger<number> {
        protected getPropertyFromContainer(container: TableCellProperties): number {
            return container.backgroundColor;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseBackgroundColor;
        }
    }

    export class TableCellVerticalAlignmentMerger extends TableCellPropertiesMerger<TableCellVerticalAlignment> {
        protected getPropertyFromContainer(container: TableCellProperties): TableCellVerticalAlignment {
            return container.verticalAlignment;
        }

        protected getPropertyMask(): number {
            return TableCellPropertiesMask.UseVerticalAlignment;
        }
    }
}