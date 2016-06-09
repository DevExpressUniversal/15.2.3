module __aspxRichEdit {
    export class TableRowPropertiesMerger<ResultPropertyType> extends TablePropertiesMergerBase<TableRowProperties, ResultPropertyType> {
        private static conditionalTableStyleFormattingPriority: ConditionalTableStyleFormatting[] = [
            ConditionalTableStyleFormatting.FirstRow,
            ConditionalTableStyleFormatting.LastRow,
            ConditionalTableStyleFormatting.OddRowBanding,
            ConditionalTableStyleFormatting.EvenRowBanding,
            ConditionalTableStyleFormatting.WholeTable,
        ];

        protected getContainerFromConditionalStyle(condStyle: TableConditionalStyle): TableRowProperties {
            return condStyle.tableRowProperties;
        }

        protected canUseValue(props: TableRowProperties): boolean {
            return !!(props.mask & this.getPropertyMask());
        }

        protected getCondTableStyleFormattingListForThisContainer(): ConditionalTableStyleFormatting[] {
            return TableRowPropertiesMerger.conditionalTableStyleFormattingPriority;
        }
    }

    export class TableRowPropertiesMergerCellSpacing extends TableRowPropertiesMerger<TableWidthUnit> {
        model: DocumentModel;
        table: Table;

        constructor(model: DocumentModel, table: Table) {
            super();
            this.model = model;
            this.table = table;
        }

        protected getPropertyFromContainer(container: TableRowProperties): TableWidthUnit {
            return container.cellSpacing;
        }

        protected getPropertyMask(): number {
            return TableRowPropertiesMask.UseCellSpacing;
        }

        protected actionBeforeDefaultValue(): boolean {
            this.result = new TablePropertiesMergerCellSpacing().getProperty(this.table.properties, this.table.style, ConditionalTableStyleFormatting.WholeTable, this.model.defaultTableProperties);
            return true;
        }
    }

    export class TableRowPropertiesMergerCantSplit extends TableRowPropertiesMerger<boolean> {
        protected getPropertyFromContainer(container: TableRowProperties): boolean {
            return container.cantSplit;
        }

        protected getPropertyMask(): number {
            return TableRowPropertiesMask.UseCantSplit;
        }
    }

    export class TableRowPropertiesMergerHorizontalAlignment extends TableRowPropertiesMerger<TableRowAlignment> {
        protected getPropertyFromContainer(container: TableRowProperties): TableRowAlignment {
            return container.tableRowAlignment;
        }

        protected getPropertyMask(): number {
            return TableRowPropertiesMask.UseTableRowAlignment;
        }

        protected actionBeforeDefaultValue(): boolean {
            this.result = null;
            return true;
        }
    }
}