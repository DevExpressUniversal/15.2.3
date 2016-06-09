module __aspxRichEdit {
    export class DialogTablePropertiesCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            let state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.selection.getSelectedCells().length > 0;
            return state;
        }
        createParameters(parameters: TablePropertiesDialogParameters): TablePropertiesDialogParameters {
            let dialogParams: TablePropertiesDialogParameters = new TablePropertiesDialogParameters();
            let selectedCells = this.control.selection.getSelectedCells();
            let startCell = selectedCells[0][0];
            let table = startCell.parentRow.parentTable;
            const subDocument: SubDocument = this.control.model.activeSubDocument;
            const position: number = table.parentCell ? table.parentCell.endParagrapPosition.value : this.control.selection.intervals[0].start;
            let lp: LayoutPosition = new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, position, DocumentLayoutDetailsLevel.Row)
                .create(new LayoutPositionCreatorConflictFlags().setDefault(true), new LayoutPositionCreatorConflictFlags().setDefault(true));
            let maxTableWidth = UnitConverter.pixelsToTwipsF(table.parentCell ? lp.row.width : LayoutColumn.findSectionColumnWithMinimumWidth(lp.pageArea.columns));
            dialogParams.init(table, selectedCells, this.control.modelManipulator.model, maxTableWidth, this.getInitialTab());
            return dialogParams;
        }
        applyParameters(state: SimpleCommandState, newParams: TablePropertiesDialogParameters) {
            let initParams: TablePropertiesDialogParameters = <TablePropertiesDialogParameters>this.initParams;
            let modelManipulator: ModelManipulator = this.control.modelManipulator;
            let subDocument: SubDocument = modelManipulator.model.activeSubDocument;
            let selectedCells = this.control.selection.getSelectedCells();
            let startCell = selectedCells[0][0];
            let table = startCell.parentRow.parentTable;
            let history: IHistory = this.control.history;

            history.beginTransaction();
            //TODO Refactor
            newParams.tablePreferredWidth = this.getActualPreferredWidth(newParams.useDefaultTableWidth, newParams.tablePreferredWidth);
            if(newParams.tablePreferredWidth && !newParams.tablePreferredWidth.equals(initParams.tablePreferredWidth))
                history.addAndRedo(new TablePreferredWidthHistoryItem(modelManipulator, subDocument, table.index, newParams.tablePreferredWidth));
            if(newParams.tableRowAlignment !== undefined && newParams.tableRowAlignment !== null && newParams.tableRowAlignment !== initParams.tableRowAlignment) {
                history.addAndRedo(new TableTableRowAlignmentHistoryItem(modelManipulator, subDocument, table.index, newParams.tableRowAlignment, true));
                for(let i = 0, currentRow: TableRow; currentRow = table.rows[i]; i++)
                    history.addAndRedo(new TableRowTableRowAlignmentHistoryItem(modelManipulator, subDocument, table.index, i, newParams.tableRowAlignment, true));
            }  

                           
            if(!newParams.tableRowAlignment || newParams.tableRowAlignment !== TableRowAlignment.Left)
                newParams.tableIndent = 0;
            if(newParams.tableIndent !== initParams.tableIndent) {
                let newTableIndent: TableWidthUnit = TableWidthUnit.create(newParams.tableIndent, TableWidthUnitType.ModelUnits);  
                history.addAndRedo(new TableIndentHistoryItem(modelManipulator, subDocument, table.index, newTableIndent, true));
            }
           
            if(!newParams.allowCellSpacing)
                newParams.cellSpacing = 0;
            if(newParams.cellSpacing !== undefined && newParams.cellSpacing !== initParams.cellSpacing) {
                let newCellSpacing: TableWidthUnit;
                if(newParams.allowCellSpacing)
                    newCellSpacing = TableWidthUnit.create(newParams.cellSpacing / 2, TableWidthUnitType.ModelUnits);
                else
                    newCellSpacing = subDocument.documentModel.defaultTableProperties.cellSpacing.clone();
                history.addAndRedo(new TableCellSpacingHistoryItem(modelManipulator, subDocument, table.index, newCellSpacing, true));
                for(let i = 0, currentRow: TableRow; currentRow = table.rows[i]; i++)
                    history.addAndRedo(new TableRowCellSpacingHistoryItem(modelManipulator, subDocument, table.index, i, newCellSpacing, true));
            }
            let newlayoutType: TableLayoutType = newParams.resizeToFitContent ? TableLayoutType.Autofit : TableLayoutType.Fixed;
            if(newlayoutType !== table.properties.layoutType)
                history.addAndRedo(new TableLayoutTypeHistoryItem(modelManipulator, subDocument, table.index, newlayoutType, true));

            if(newParams.defaultCellMarginTop !== initParams.defaultCellMarginTop || newParams.defaultCellMarginRight !== initParams.defaultCellMarginRight ||
                newParams.defaultCellMarginBottom !== initParams.defaultCellMarginBottom || newParams.defaultCellMarginLeft !== initParams.defaultCellMarginLeft) {
                let topCellMargin = TableWidthUnit.create(newParams.defaultCellMarginTop, TableWidthUnitType.ModelUnits);
                let rightCellmargin = TableWidthUnit.create(newParams.defaultCellMarginRight, TableWidthUnitType.ModelUnits);
                let bottomCellMargin = TableWidthUnit.create(newParams.defaultCellMarginBottom, TableWidthUnitType.ModelUnits);
                let leftCellMargin = TableWidthUnit.create(newParams.defaultCellMarginLeft, TableWidthUnitType.ModelUnits);
                let newCellMargins = [topCellMargin, rightCellmargin, bottomCellMargin, leftCellMargin];
                history.addAndRedo(new TableCellMarginsHistoryItem(modelManipulator, subDocument, table.index, newCellMargins, [true, true, true, true]));
            }

            newParams.rowHeight = this.getActualRowHeight(newParams.useDefaultRowHeight, newParams.rowHeight);
            if(newParams.rowHeight)
                for(var i = selectedCells.length - 1; i >= 0; i--) {
                    let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[i][0].startParagraphPosition.value);
                    let row = table.rows[rowIndex];
                    if(!row.height.equals(newParams.rowHeight))
                        history.addAndRedo(new TableRowHeightHistoryItem(modelManipulator, subDocument, table.index, rowIndex, newParams.rowHeight));
                }

            newParams.columnPreferredWidth = this.getActualPreferredWidth(newParams.useDefaultColumnWidth, newParams.columnPreferredWidth);
            let columnsRange = TableCellUtils.getColumnsRangeBySelectedCells(selectedCells);
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                let cellIndices = TableCellUtils.getCellIndicesByColumnsRange(row, columnsRange.startColumnIndex, columnsRange.endColumnIndex);
                for(let i = cellIndices.length - 1; i >= 0; i--) {
                    let cell = table.rows[rowIndex].cells[cellIndices[i]];
                    if(newParams.columnPreferredWidth && !cell.preferredWidth.equals(newParams.columnPreferredWidth))
                        history.addAndRedo(new TableCellPreferredWidthHistoryItem(modelManipulator, subDocument, table.index, rowIndex, cellIndices[i], newParams.columnPreferredWidth));
                }
            }

            newParams.cellPreferredWidth = this.getActualPreferredWidth(newParams.useDefaultCellWidth, newParams.cellPreferredWidth);
            for(var i = selectedCells.length - 1; i >= 0; i--) {
                let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[i][0].startParagraphPosition.value);
                let row = table.rows[rowIndex];
                for(var j = 0, cell: TableCell; cell = selectedCells[i][j]; j++) {
                    let cellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - cell.startParagraphPosition.value);
                    if(newParams.cellPreferredWidth && !cell.preferredWidth.equals(newParams.cellPreferredWidth) && !initParams.cellPreferredWidth.equals(newParams.cellPreferredWidth))
                        history.addAndRedo(new TableCellPreferredWidthHistoryItem(modelManipulator, subDocument, table.index, rowIndex, cellIndex, newParams.cellPreferredWidth));
                    if(newParams.cellVerticalAlignment !== undefined && newParams.cellVerticalAlignment !== null && newParams.cellVerticalAlignment !== initParams.cellVerticalAlignment)
                        history.addAndRedo(new TableCellVerticalAlignmentHistoryItem(modelManipulator, subDocument, table.index, rowIndex, cellIndex, newParams.cellVerticalAlignment, true));
                    if(newParams.cellNoWrap !== undefined && newParams.cellNoWrap != initParams.cellNoWrap)
                        history.addAndRedo(new TableCellNoWrapHistoryItem(modelManipulator, subDocument, table.index, rowIndex, cellIndex, newParams.cellNoWrap, true));
                    if(newParams.cellMarginsSameAsTable) {
                        let defaultCellMargins = subDocument.documentModel.defaultTableCellProperties.cellMargins;
                        if(!defaultCellMargins.equals(cell.properties.cellMargins)) {
                            let newCellMargins = [defaultCellMargins.top.clone(), defaultCellMargins.right.clone(), defaultCellMargins.bottom.clone(), defaultCellMargins.left.clone()];
                            history.addAndRedo(new TableCellCellMarginsHistoryItem(modelManipulator, subDocument, table.index, rowIndex, cellIndex, newCellMargins, [false, false, false, false]));
                        }
                    }
                    else {
                        if(newParams.cellMarginTop !== initParams.cellMarginTop || newParams.cellMarginRight !== initParams.cellMarginRight ||
                            newParams.cellMarginBottom !== initParams.cellMarginBottom || newParams.cellMarginLeft !== initParams.cellMarginLeft) {
                            let topCellMargin = TableWidthUnit.create(newParams.cellMarginTop, TableWidthUnitType.ModelUnits);
                            let rightCellmargin = TableWidthUnit.create(newParams.cellMarginRight, TableWidthUnitType.ModelUnits);
                            let bottomCellMargin = TableWidthUnit.create(newParams.cellMarginBottom, TableWidthUnitType.ModelUnits);
                            let leftCellMargin = TableWidthUnit.create(newParams.cellMarginLeft, TableWidthUnitType.ModelUnits);
                            let newCellMargins = [topCellMargin, rightCellmargin, bottomCellMargin, leftCellMargin];
                            history.addAndRedo(new TableCellCellMarginsHistoryItem(modelManipulator, subDocument, table.index, rowIndex, cellIndex, newCellMargins, [true, true, true, true]));
                        }
                    }
                }
            }
            history.endTransaction();
        }
        getActualPreferredWidth(useDefaultValue: boolean, preferredWidth: TableWidthUnit) {
            if(useDefaultValue === null)
                return null;
            if(useDefaultValue === true)
                return TableWidthUnit.create(0, TableWidthUnitType.Auto);
            return preferredWidth;
        }
        getActualRowHeight(useDefaultRowHeight: boolean, rowHeight: TableHeightUnit): TableHeightUnit {
            if(useDefaultRowHeight === null)
                return null;
            if(useDefaultRowHeight === true)
                return TableHeightUnit.createDefault();
            return rowHeight;
        }

        getInitialTab(): TablePropertiesDialogTab {
            return TablePropertiesDialogTab.Table;
        }

        getDialogName() {
            return "TableProperties";
        }
    }

    export class DialogCellPropertiesCommand extends DialogTablePropertiesCommand {
        getInitialTab(): TablePropertiesDialogTab {
            return TablePropertiesDialogTab.Cell;
        }
    }

    export class TablePropertiesDialogParameters extends DialogParametersBase {
        useDefaultTableWidth: boolean;
        tablePreferredWidth: TableWidthUnit;
        tableRowAlignment: TableRowAlignment;
        tableIndent: number;
        cellSpacing: number;
        allowCellSpacing: boolean;
        resizeToFitContent: boolean;
        defaultCellMarginLeft: number;
        defaultCellMarginRight: number;
        defaultCellMarginTop: number;
        defaultCellMarginBottom: number;

        useDefaultRowHeight: boolean;
        rowHeight: TableHeightUnit;

        useDefaultColumnWidth: boolean;
        columnPreferredWidth: TableWidthUnit;

        useDefaultCellWidth: boolean;
        cellPreferredWidth: TableWidthUnit;
        cellVerticalAlignment: TableCellVerticalAlignment;
        cellNoWrap: boolean;
        cellMarginLeft: number;
        cellMarginRight: number;
        cellMarginTop: number;
        cellMarginBottom: number;
        cellMarginsSameAsTable: boolean;

        maxTableWidth: number;
        initialTab: TablePropertiesDialogTab;

        init(table: Table, selectedCells: TableCell[][], model: DocumentModel, maxTableWidth: number, tab: TablePropertiesDialogTab) {
            this.tableInit(table, model);
            this.rowInit(table, selectedCells);
            this.columnInit(table, selectedCells);
            this.cellInit(table, selectedCells, model);
            this.maxTableWidth = maxTableWidth;
            this.initialTab = tab;
        }
        tableInit(table: Table, model: DocumentModel) {           
            this.useDefaultTableWidth = table.preferredWidth.type === TableWidthUnitType.Auto || table.preferredWidth.type === TableWidthUnitType.Nil;
            this.tablePreferredWidth = table.preferredWidth.clone();
            this.tableRowAlignment = this.getTableAlignment(table, model);

            let actualTableIndent = new TablePropertiesMergerIndent()
                .getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties);
            this.tableIndent = actualTableIndent.type === TableWidthUnitType.ModelUnits ? actualTableIndent.value : 0;
            this.cellSpacing = this.getCellSpacing(table, model);
            this.allowCellSpacing = this.cellSpacing !== 0;
            let layoutType = new TablePropertiesMergerLayoutType().getProperty(table.properties);
            this.resizeToFitContent = layoutType === TableLayoutType.Autofit;

            this.defaultCellMarginLeft = new TablePropertiesMergerMarginLeft().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties).value;
            this.defaultCellMarginRight = new TablePropertiesMergerMarginRight().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties).value;
            this.defaultCellMarginTop = new TablePropertiesMergerMarginTop().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties).value;
            this.defaultCellMarginBottom = new TablePropertiesMergerMarginBottom().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties).value;
        }
        getTableAlignment(table: Table, model: DocumentModel): TableRowAlignment {
            let rowAlignmentMerger = new TableRowPropertiesMergerHorizontalAlignment();
            let firstRowAlignment: TableRowAlignment = rowAlignmentMerger.getProperty(table.rows[0].properties, table.style, ConditionalTableStyleFormatting.FirstRow, model.defaultTableRowProperties);
            for(let i = 0, currentRow: TableRow; currentRow = table.rows[i]; i++) {
                let currentRowTableRowAligment = rowAlignmentMerger.getProperty(currentRow.properties, table.style, currentRow.conditionalFormatting, model.defaultTableRowProperties);
                if(firstRowAlignment !== currentRowTableRowAligment)
                    return null;
            }
            return firstRowAlignment ? firstRowAlignment : new TablePropertiesMergerHorizontalAlignment().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProperties);
        }
        getCellSpacing(table: Table, model: DocumentModel): number {
            let cellSpacingMerger = new TableRowPropertiesMergerCellSpacing(model, table);
            let firstRow = table.rows[0];
            let firstRowCellSpacing: TableWidthUnit = cellSpacingMerger.getProperty(firstRow.properties, table.style, ConditionalTableStyleFormatting.FirstRow, model.defaultTableRowProperties);   
            for(let i = 0, currentRow: TableRow; currentRow = table.rows[i]; i++) {
                let currentRowCellSpacing: TableWidthUnit = cellSpacingMerger.getProperty(currentRow.properties, table.style, currentRow.conditionalFormatting,
                    model.defaultTableRowProperties);   
                if(!firstRowCellSpacing.equals(currentRowCellSpacing))
                    return null;
            }
            return firstRowCellSpacing.type === TableWidthUnitType.ModelUnits ? (firstRowCellSpacing.value * 2) : 0;
        }

        rowInit(table: Table, selectedCells: TableCell[][]) {
            let identicalRowHeight: boolean = true;
            let firstRowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[0][0].startParagraphPosition.value);
            let firstRow = table.rows[firstRowIndex];
            let firstRowHeight: TableHeightUnit = firstRow.height.clone();      
            for(var i = selectedCells.length - 1; i >= 1; i--) {
                let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[i][0].startParagraphPosition.value);
                let currentRow = table.rows[rowIndex];
                identicalRowHeight = currentRow.height.equals(firstRowHeight);
            }
            this.useDefaultRowHeight = identicalRowHeight ? firstRowHeight.value === 0 : null;
            this.rowHeight = TableHeightUnit.create(identicalRowHeight ? firstRowHeight.value : 0, firstRowHeight.type === TableHeightUnitType.Exact ? TableHeightUnitType.Exact : TableHeightUnitType.Minimum);
        }
        columnInit(table: Table, selectedCells: TableCell[][]) {
            let identicalColumnWidth: boolean = true;          
            let columnsRange = TableCellUtils.getColumnsRangeBySelectedCells(selectedCells);
            let firstCellInColumnIndex = TableCellUtils.getCellIndexByColumnIndex(table.rows[0], columnsRange.startColumnIndex);
            let firstCellInColumn = table.rows[0].cells[firstCellInColumnIndex];
            let firstCellWidth: TableWidthUnit = firstCellInColumn.preferredWidth.clone();
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                let cellIndices = TableCellUtils.getCellIndicesByColumnsRange(row, columnsRange.startColumnIndex, columnsRange.endColumnIndex);
                for(let i = cellIndices.length - 1; i >= 0; i--) {
                    let cell = table.rows[rowIndex].cells[cellIndices[i]];
                    identicalColumnWidth = identicalColumnWidth && cell.preferredWidth.equals(firstCellWidth);
                }
            }
            this.useDefaultColumnWidth = identicalColumnWidth ? firstCellWidth.type === TableWidthUnitType.Auto : null;
            this.columnPreferredWidth = firstCellWidth.clone();
        }
        cellInit(table: Table, selectedCells: TableCell[][], model: DocumentModel) {
            this.cellMarginsSameAsTable = true;
            let identicalCellWidth: boolean = true;
            let identicalVerticalAlignment: boolean = true;
            let identicalNoWrap: boolean = true;
            let identicalLeftMargins: boolean = true;
            let identicalRightMargins: boolean = true;
            let identicalTopMargins: boolean = true;
            let identicalBottomMargins: boolean = true;

            let noWrapMerger = new TableCellPropertiesMergerNoWrap();
            let verticalAlignmentMerger = new TableCellVerticalAlignmentMerger();

            let firstSelectedCell: TableCell = selectedCells[0][0];
            let firstCellWidth: TableWidthUnit = firstSelectedCell.preferredWidth.clone();
            let firstCellVerticalAlignment: TableCellVerticalAlignment =
                verticalAlignmentMerger.getProperty(firstSelectedCell.properties, table.style, firstSelectedCell.conditionalFormatting, model.defaultTableCellProperties);
            let firstCellNoWrap = noWrapMerger.getProperty(firstSelectedCell.properties, table.style, firstSelectedCell.conditionalFormatting, model.defaultTableCellProperties);
            let firstCellMargins = this.getActualCellMargins(table, firstSelectedCell, model);
            for(var i = selectedCells.length - 1; i >= 0; i--) {
                for(var j = 0, cell: TableCell; cell = selectedCells[i][j]; j++) {
                    identicalCellWidth = identicalCellWidth && (firstCellWidth.equals(cell.preferredWidth));
                    let currentCellVerticalAlignment = 
                        verticalAlignmentMerger.getProperty(cell.properties, table.style, cell.conditionalFormatting, model.defaultTableCellProperties);
                    identicalVerticalAlignment = identicalVerticalAlignment && (firstCellVerticalAlignment === currentCellVerticalAlignment);
                    let currentCellNoWrap = noWrapMerger.getProperty(cell.properties, table.style, cell.conditionalFormatting, model.defaultTableCellProperties);
                    identicalNoWrap = identicalNoWrap && (firstCellNoWrap === currentCellNoWrap);

                    let curreantCellMargins = this.getActualCellMargins(table, cell, model);
                    identicalTopMargins = identicalTopMargins && firstCellMargins.top.equals(curreantCellMargins.top);
                    identicalRightMargins = identicalRightMargins && firstCellMargins.right.equals(curreantCellMargins.right);
                    identicalBottomMargins = identicalBottomMargins && firstCellMargins.bottom.equals(curreantCellMargins.bottom);
                    identicalLeftMargins = identicalLeftMargins && firstCellMargins.left.equals(curreantCellMargins.left);
                    if(cell.properties.getUseValue(TableCellPropertiesMask.UseTopMargin) || cell.properties.getUseValue(TableCellPropertiesMask.UseRightMargin) ||
                        cell.properties.getUseValue(TableCellPropertiesMask.UseBottomMargin) || cell.properties.getUseValue(TableCellPropertiesMask.UseLeftMargin))
                        this.cellMarginsSameAsTable = false;
                }
            }
            this.useDefaultCellWidth = identicalCellWidth ? firstCellWidth.type === TableWidthUnitType.Auto : null;
            this.cellPreferredWidth = firstCellWidth;
            this.cellVerticalAlignment = identicalVerticalAlignment ? firstCellVerticalAlignment : null;
            this.cellNoWrap = identicalNoWrap ? firstCellNoWrap : null;
            this.cellMarginTop = identicalTopMargins ? firstCellMargins.top.value : null;
            this.cellMarginRight = identicalRightMargins ? firstCellMargins.right.value : null;
            this.cellMarginBottom = identicalBottomMargins ? firstCellMargins.bottom.value : null;
            this.cellMarginLeft = identicalLeftMargins ? firstCellMargins.left.value : null;
        }
        getActualCellMargins(table: Table, cell: TableCell, model: DocumentModel) {
            let defaultTableCellProps = model.defaultTableCellProperties;
            let CellMarginLeft = new TableCellPropertiesMergerMarginLeft(table, model).getProperty(cell.properties, table.style, cell.conditionalFormatting, defaultTableCellProps).clone();
            let CellMarginRight = new TableCellPropertiesMergerMarginRight(table, model).getProperty(cell.properties, table.style, cell.conditionalFormatting, defaultTableCellProps).clone();
            let CellMarginTop = new TableCellPropertiesMergerMarginTop(table, model).getProperty(cell.properties, table.style, cell.conditionalFormatting, defaultTableCellProps).clone();
            let CellMarginBottom = new TableCellPropertiesMergerMarginBottom(table, model).getProperty(cell.properties, table.style, cell.conditionalFormatting, defaultTableCellProps).clone();
            return TableCellMargins.create(CellMarginTop, CellMarginRight, CellMarginBottom, CellMarginLeft);
        }

        copyFrom(obj: TablePropertiesDialogParameters) {
            this.useDefaultTableWidth = obj.useDefaultTableWidth;
            this.tablePreferredWidth = obj.tablePreferredWidth.clone();
            this.tableRowAlignment = obj.tableRowAlignment;
            this.tableIndent = obj.tableIndent;
            this.cellSpacing = obj.cellSpacing;
            this.allowCellSpacing = obj.allowCellSpacing;
            this.resizeToFitContent = obj.resizeToFitContent;
            this.defaultCellMarginLeft = obj.defaultCellMarginLeft;
            this.defaultCellMarginRight = obj.defaultCellMarginRight;
            this.defaultCellMarginTop = obj.defaultCellMarginTop;
            this.defaultCellMarginBottom = obj.defaultCellMarginTop;
            this.useDefaultRowHeight = obj.useDefaultRowHeight;
            this.rowHeight = obj.rowHeight.clone();
            this.useDefaultColumnWidth = obj.useDefaultColumnWidth;
            this.columnPreferredWidth = obj.columnPreferredWidth.clone();
            this.useDefaultCellWidth = obj.useDefaultCellWidth;
            this.cellPreferredWidth = obj.cellPreferredWidth.clone();
            this.cellVerticalAlignment = obj.cellVerticalAlignment;
            this.cellNoWrap = obj.cellNoWrap;
            this.cellMarginLeft = obj.cellMarginLeft;
            this.cellMarginRight = obj.cellMarginRight;
            this.cellMarginTop = obj.cellMarginTop;
            this.cellMarginBottom = obj.cellMarginBottom;
            this.cellMarginsSameAsTable = obj.cellMarginsSameAsTable;
        }

        getNewInstance(): DialogParametersBase {
            return new TablePropertiesDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
            if(this.tablePreferredWidth && this.tablePreferredWidth.type === TableWidthUnitType.ModelUnits)
                this.tablePreferredWidth.value = converterFunc(this.tablePreferredWidth.value);
            if(this.tableIndent) this.tableIndent = converterFunc(this.tableIndent);
            if(this.cellSpacing) this.cellSpacing = converterFunc(this.cellSpacing);
            if(this.defaultCellMarginLeft) this.defaultCellMarginLeft = converterFunc(this.defaultCellMarginLeft);
            if(this.defaultCellMarginRight) this.defaultCellMarginRight = converterFunc(this.defaultCellMarginRight);
            if(this.defaultCellMarginTop) this.defaultCellMarginTop = converterFunc(this.defaultCellMarginTop);
            if(this.defaultCellMarginBottom) this.defaultCellMarginBottom = converterFunc(this.defaultCellMarginBottom);
            if(this.rowHeight) this.rowHeight.value = converterFunc(this.rowHeight.value);
            if(this.columnPreferredWidth && this.columnPreferredWidth.type === TableWidthUnitType.ModelUnits)
                this.columnPreferredWidth.value = converterFunc(this.columnPreferredWidth.value);
            if(this.cellPreferredWidth && this.cellPreferredWidth.type === TableWidthUnitType.ModelUnits)
                this.cellPreferredWidth.value = converterFunc(this.cellPreferredWidth.value);
            if(this.cellMarginTop) this.cellMarginTop = converterFunc(this.cellMarginTop);
            if(this.cellMarginRight) this.cellMarginRight = converterFunc(this.cellMarginRight);
            if(this.cellMarginBottom) this.cellMarginBottom = converterFunc(this.cellMarginBottom);
            if(this.cellMarginLeft) this.cellMarginLeft = converterFunc(this.cellMarginLeft);
            if(this.maxTableWidth) this.maxTableWidth = converterFunc(this.maxTableWidth);
        }
    }

    export class TablePropertiesDialogDefaults {
        static MinTableIndentByDefault: number = -15 * 1440;
        static MaxTableIndentByDefault: number = 15 * 1440;
        static MinTableWidthByDefault: number = 0;
        static MaxTableWidthInModelUnitsByDefault: number = 22 * 1440;
        static MaxTableWidthInPercentByDefault: number = 600;

        static MinRowHeightByDefault: number = 0;
        static MaxRowHeightByDefault: number = 22 * 1440;

        static MinColumnWidthByDefault: number = 0;
        static MaxColumnWidthInModelUnitsByDefault: number = 22 * 1440;
        static MaxColumnWidthInPercentByDefault: number = 100;

        static MinCellWidthByDefault: number = 0;
        static MaxCellWidthInModelUnitsByDefault: number = 22 * 1440;
        static MaxCellWidthInPercentByDefault: number = 100;
    }

    export enum TablePropertiesDialogTab {
        Table = 0,
        Row = 1,
        Column = 2,
        Cell = 3
    }
}