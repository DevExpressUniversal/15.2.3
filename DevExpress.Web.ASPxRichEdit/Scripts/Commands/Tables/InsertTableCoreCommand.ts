module __aspxRichEdit {
    export class InsertTableCoreCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.tables);
        }

        executeCore(state: SimpleCommandState, parameter: { rowCount: number, cellCount: number }): boolean {
            this.control.history.beginTransaction();
            const subDocument: SubDocument = this.control.model.activeSubDocument;
            const position: number = this.control.selection.intervals[0].start;

            let lp: LayoutPosition = subDocument.isMain() ?
                new LayoutPositionMainSubDocumentCreator(this.control.layout, subDocument, position, DocumentLayoutDetailsLevel.Row)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false)) :
                new LayoutPositionOtherSubDocumentCreator(this.control.layout, subDocument, position, this.control.selection.pageIndex, DocumentLayoutDetailsLevel.Row)
                    .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));

            let currentTable = Table.getTableByPosition(subDocument.tables, position, true);
            let availableWidth = currentTable ? lp.row.width : LayoutColumn.findSectionColumnWithMinimumWidth(lp.pageArea.columns);

            let table = TablesManipulator.insertTable(this.control, subDocument, parameter.rowCount, parameter.cellCount, position, UnitConverter.pixelsToTwipsF(availableWidth));
            let newTableBorder = new BorderInfo();
            newTableBorder.style = BorderLineStyle.Single;
            newTableBorder.width = 15;
            newTableBorder.color = 0;
            this.control.history.addAndRedo(new TableBordersHistoryItem(this.control.modelManipulator, subDocument, table.index, [
                newTableBorder.clone(), newTableBorder.clone(), newTableBorder.clone(), newTableBorder.clone(), newTableBorder.clone(), newTableBorder.clone()
            ], [true, true, true, true, true, true]));
            ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(table.getStartPosition(), 0), UpdateInputPositionProperties.Yes, false);
            TableConditionalFormattingCalculator.updateTable(this.control, table);
            this.control.history.endTransaction();
            return true;
        }
    }
}