module __aspxRichEdit {
    export class TablePropertiesMerger<ResultPropertyType> extends TablePropertiesMergerBase<TableProperties, ResultPropertyType> {
        private static conditionalTableStyleFormattingPriority: ConditionalTableStyleFormatting[] = [
            ConditionalTableStyleFormatting.WholeTable,
        ];

        protected getContainerFromConditionalStyle(condStyle: TableConditionalStyle): TableProperties {
            return condStyle.tableProperties;
        }

        protected canUseValue(props: TableProperties): boolean {
            return !!(props.mask & this.getPropertyMask());
        }

        protected getCondTableStyleFormattingListForThisContainer(): ConditionalTableStyleFormatting[] {
            return TablePropertiesMerger.conditionalTableStyleFormattingPriority;
        }
    }

    export class TablePropertiesMergerIndent extends TablePropertiesMerger<TableWidthUnit> {
        protected getPropertyFromContainer(container: TableProperties): TableWidthUnit {
            return container.indent;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseTableIndent;
        }
    }

    export class TablePropertiesMergerCellSpacing extends TablePropertiesMerger<TableWidthUnit> {
        protected getPropertyFromContainer(container: TableProperties): TableWidthUnit {
            return container.cellSpacing;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseCellSpacing;
        }
    }

    export class TablePropertiesMergerBorderLeft extends TablePropertiesMerger<BorderInfo> {
        protected getPropertyFromContainer(container: TableProperties): BorderInfo {
            return container.borders.leftBorder;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseLeftBorder;
        }
    }

    export class TablePropertiesMergerBorderRight extends TablePropertiesMerger<BorderInfo> {
        protected getPropertyFromContainer(container: TableProperties): BorderInfo {
            return container.borders.rightBorder;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseRightBorder;
        }
    }

    export class TablePropertiesMergerBorderTop extends TablePropertiesMerger<BorderInfo> {
        protected getPropertyFromContainer(container: TableProperties): BorderInfo {
            return container.borders.topBorder;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseTopBorder;
        }
    }

    export class TablePropertiesMergerBorderBottom extends TablePropertiesMerger<BorderInfo> {
        protected getPropertyFromContainer(container: TableProperties): BorderInfo {
            return container.borders.bottomBorder;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseBottomBorder;
        }
    }

    export class TablePropertiesMergerBorderVertical extends TablePropertiesMerger<BorderInfo> {
        protected getPropertyFromContainer(container: TableProperties): BorderInfo {
            return container.borders.insideVerticalBorder;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseInsideVerticalBorder;
        }
    }

    export class TablePropertiesMergerBorderHorizontal extends TablePropertiesMerger<BorderInfo> {
        protected getPropertyFromContainer(container: TableProperties): BorderInfo {
            return container.borders.insideHorizontalBorder;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseInsideHorizontalBorder;
        }
    }

    export class TablePropertiesMergerMarginLeft extends TablePropertiesMerger<TableWidthUnit> {
        protected getPropertyFromContainer(container: TableProperties): TableWidthUnit {
            return container.cellMargins.left;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseLeftMargin;
        }
    }

    export class TablePropertiesMergerMarginRight extends TablePropertiesMerger<TableWidthUnit> {
        protected getPropertyFromContainer(container: TableProperties): TableWidthUnit {
            return container.cellMargins.right;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseRightMargin;
        }
    }

    export class TablePropertiesMergerMarginTop extends TablePropertiesMerger<TableWidthUnit> {
        protected getPropertyFromContainer(container: TableProperties): TableWidthUnit {
            return container.cellMargins.top;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseTopMargin;
        }
    }

    export class TablePropertiesMergerMarginBottom extends TablePropertiesMerger<TableWidthUnit> {
        protected getPropertyFromContainer(container: TableProperties): TableWidthUnit {
            return container.cellMargins.bottom;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseBottomMargin;
        }
    }

    export class TablePropertiesMergerLayoutType {
        public getProperty(container: TableProperties): TableLayoutType {
            return container.getUseValue(TablePropertiesMask.UseTableLayout) ? container.layoutType : TableLayoutType.Autofit;
        }
    }


    export class TablePropertiesMergerBackgroundColor extends TablePropertiesMerger<number> {
        protected getPropertyFromContainer(container: TableProperties): number {
            return container.backgroundColor;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseBackgroundColor;
        }
    }

    export class TablePropertiesMergerStyleColumnBandSize extends TablePropertiesMerger<number> {
        protected getPropertyFromContainer(container: TableProperties): number {
            return container.tableStyleColumnBandSize;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseTableStyleColBandSize;
        }
    }

    export class TablePropertiesMergerStyleRowBandSize extends TablePropertiesMerger<number> {
        protected getPropertyFromContainer(container: TableProperties): number {
            return container.tableStyleRowBandSize;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseTableStyleRowBandSize;
        }
    }

    export class TablePropertiesMergerHorizontalAlignment extends TablePropertiesMerger<TableRowAlignment> {
        protected getPropertyFromContainer(container: TableProperties): TableRowAlignment {
            return container.tableRowAlignment;
        }

        protected getPropertyMask(): number {
            return TablePropertiesMask.UseTableAlignment;
        }

        protected actionBeforeDefaultValue(): boolean {
            this.result = TableRowAlignment.Left;
            return true;
        }
    }
}