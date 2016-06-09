module __aspxRichEdit {
    export class DialogBorderShadingCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            let state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.selection.getSelectedCells().length > 0;
            return state;
        }

        createParameters(): BorderShadingDialogParameters {
            let dialogParams: BorderShadingDialogParameters = new BorderShadingDialogParameters();
            let selectedCells = this.control.selection.getSelectedCells();
            let startCell = selectedCells[0][0];
            let table = startCell.parentRow.parentTable;
            dialogParams.init(selectedCells, this.control.modelManipulator.model);
            return dialogParams;
        }

        applyParameters(state: SimpleCommandState, newParams: BorderShadingDialogParameters) {
            let initParams: BorderShadingDialogParameters = <BorderShadingDialogParameters>this.initParams;
            let modelManipulator: ModelManipulator = this.control.modelManipulator;
            let subDocument: SubDocument = modelManipulator.model.activeSubDocument;
            let selectedCells = this.control.selection.getSelectedCells();
            let startCell = selectedCells[0][0];
            let table = startCell.parentRow.parentTable;
            let history: IHistory = this.control.history;

            history.beginTransaction();


            if(newParams.backgroundColor !== initParams.backgroundColor) {
                let color = newParams.backgroundColor == "Auto" ? ColorHelper.AUTOMATIC_COLOR : ColorHelper.hashToColor(newParams.backgroundColor);
                for(var i = selectedCells.length - 1; i >= 0; i--) {
                    let rowIndex = Utils.normedBinaryIndexOf(table.rows, r => r.getStartPosition() - selectedCells[i][0].startParagraphPosition.value);
                    let row = table.rows[rowIndex];
                    for(var j = 0, cell: TableCell; cell = selectedCells[i][j]; j++) {
                        let cellIndex = Utils.normedBinaryIndexOf(row.cells, c => c.startParagraphPosition.value - cell.startParagraphPosition.value);
                        history.addAndRedo(new TableCellBackgroundColorHistoryItem(modelManipulator, subDocument, table.index, rowIndex, cellIndex, color, true));
                    }
                }
            }
            history.endTransaction();
        }

        getDialogName() {
            return "BorderShading";
        }
    }

    export class BorderShadingDialogParameters extends DialogParametersBase {
        top: DialogBorderInfo;
        right: DialogBorderInfo;
        bottom: DialogBorderInfo;
        left: DialogBorderInfo;
        insideHorizontal: DialogBorderInfo;
        insideVertical: DialogBorderInfo;
        backgroundColor: string;

        init(selectedCells: TableCell[][], model: DocumentModel) {
            let defaultTableCellProps = model.defaultTableCellProperties;
            let defaultTableProps = model.defaultTableProperties;
            let firstInterval = selectedCells[0];
            let lastInterval = selectedCells[selectedCells.length - 1];
            let firstCellInFirstinterval = firstInterval[0];
            let lastCellInFirstInterval = firstInterval[firstInterval.length - 1];
            let firstCellInLastInterval = lastInterval[0];
            let table = firstCellInFirstinterval.parentRow.parentTable;

            let backColorMerger = new TableCellPropertiesMergerBackgroundColor();
            let backgroundColor: number = backColorMerger.getProperty(firstCellInFirstinterval.properties, table.style, firstCellInFirstinterval.conditionalFormatting, model.defaultTableCellProperties);
            for(let i = selectedCells.length - 1; i >= 0; i--) {
                for(let j = 0, cell: TableCell; cell = selectedCells[i][j]; j++) {
                    let currentCellColor = backColorMerger.getProperty(cell.properties, table.style, cell.conditionalFormatting, model.defaultTableCellProperties);
                    if(currentCellColor !== backgroundColor)
                        backgroundColor = null;
                }
            }
            this.backgroundColor = this.getColor(backgroundColor);   

            //TODO
            this.top = DialogBorderInfo.create(new BorderInfo());
            this.right = DialogBorderInfo.create(new BorderInfo());
            this.bottom = DialogBorderInfo.create(new BorderInfo());
            this.left = DialogBorderInfo.create(new BorderInfo());
            this.insideHorizontal = DialogBorderInfo.create(new BorderInfo());
            this.insideVertical = DialogBorderInfo.create(new BorderInfo());
        }

        //getActualTopBorder(table, cell, model): BorderInfo {
        //    let result = new TableCellPropertiesMergerBorderTop().getProperty(cell.properties, table.style, cell.conditionalFormatting, model.defaultTableCellProps) ||
        //        new TablePropertiesMergerBorderTop().getProperty(table.properties, table.style, ConditionalTableStyleFormatting.WholeTable, model.defaultTableProps);
        //    return result;
        //}

        getColor(color: number) {
            if(color == ColorHelper.AUTOMATIC_COLOR)
                return "Auto";
            if(color != undefined)
                return ColorHelper.colorToHash(color).toUpperCase();
            else
                return undefined;
        }

        copyFrom(obj: BorderShadingDialogParameters) {
            this.backgroundColor = obj.backgroundColor;
            this.top = obj.top !== null ? obj.top.clone() : null;
            this.right = obj.right !== null ? obj.right.clone() : null;
            this.bottom = obj.bottom !== null ? obj.bottom.clone() : null;
            this.left = obj.left !== null ? obj.left.clone() : null;
            this.insideHorizontal = obj.insideHorizontal !== null ? obj.insideHorizontal.clone() : null;
            this.insideVertical = obj.insideVertical !== null ? obj.insideVertical.clone() : null;
        }

        getNewInstance(): DialogParametersBase {
            return new BorderShadingDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }

    export class DialogBorderInfo implements IEquatable<DialogBorderInfo>, ISupportCopyFrom<DialogBorderInfo>, ICloneable<DialogBorderInfo> {
        color: string;
        width: number;
        style: BorderLineStyle;

        static create(borderInfo: BorderInfo): DialogBorderInfo {
            var dialogBorderInfo = new DialogBorderInfo();
            dialogBorderInfo.color = ColorHelper.colorToHash(borderInfo.color).toUpperCase();
            dialogBorderInfo.width = UnitConverter.twipsToPointsF(borderInfo.width);
            dialogBorderInfo.style = borderInfo.style;
            return dialogBorderInfo;
        }

        getBorderInfo(): BorderInfo {
            let borderInfo = new BorderInfo();
            borderInfo.color = ColorHelper.hashToColor(this.color);
            borderInfo.width = UnitConverter.pointsToTwips(this.width);
            borderInfo.style = this.style;
            return borderInfo;
        }

        equals(obj: DialogBorderInfo) {
            return this.style == obj.style &&
                this.color == obj.color &&
                this.width == obj.width;
        }

        copyFrom(obj: DialogBorderInfo) {
            this.style = obj.style;
            this.color = obj.color;
            this.width = obj.width;
        }

        public clone(): DialogBorderInfo {
            var result = new DialogBorderInfo();
            result.copyFrom(this);
            return result;
        }
    }
}